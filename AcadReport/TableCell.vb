Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Colors
Public Enum enCellDataType
   [String] = 0
   Invert = 1
   FormatNum = 2
	Num = 3
	Keyboard = 4
	KeyboardInvert = 5
End Enum

Public Enum enDestType
   AcadTable
   AcadText
   WinApp
End Enum
Public Structure CellData
   Public CellDataType As enCellDataType
	Public FormatNumDigits As Integer
	Public FormatNumDigitsM As Integer

	Public GroupDigits As TriState
	Public DFactor As Integer
   Public Sub Open(ByVal sDescr As String)
      Dim iPos As Integer = 0
		FormatNumDigits = 0
      Try
         Select Case sDescr.Substring(0, 1)
            Case "I", "i"
					CellDataType = enCellDataType.Invert
				Case "K", "k"
					CellDataType = enCellDataType.Keyboard
				Case "Q", "q"
					CellDataType = enCellDataType.KeyboardInvert
            Case "F", "f"
               CellDataType = enCellDataType.FormatNum
					Dim sNumOptions As String
					If sDescr.Length > 1 Then
						Dim sDFactor As String = sDescr.Substring(1, 1)
						Select Case sDFactor
							Case "D", "d"
								DFactor = -1
								iPos += 1
							Case "O", "o" 'function of meter
								DFactor = 1
								iPos += 1

						End Select
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!FRMT", sDescr, CellDataType, sDFactor, DFactor)
					End If

					If sDescr.Length > iPos + 1 Then
						sNumOptions = sDescr.Substring(iPos + 1, 1)
						Select Case sNumOptions
							Case "C", "c"
								GroupDigits = TriState.True
								iPos += 1
							Case "N", "n"
								GroupDigits = TriState.False
								iPos += 1
							Case Else
								GroupDigits = TriState.True
						End Select
					End If
					If (sDescr.Length > iPos + 1) Then
						Dim sNum As String = sDescr.Substring(iPos + 1)
						Dim sNumR As String = Nothing
						If sNum.Length = 2 Then
							sNumR = Right(sNum, 1)
							sNum = Left(sNum, 1)
						End If
						If IsNumeric(sNum) Then
							FormatNumDigits = Convert.ToInt32(sNum)
						End If
						If sNumR IsNot Nothing AndAlso IsNumeric(sNumR) Then
							FormatNumDigitsM = Convert.ToInt32(sNumR)
						End If
					End If
            Case "S", "s"
						CellDataType = enCellDataType.String
            Case "N", "n"
						CellDataType = enCellDataType.Num
         End Select

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "CellData - Open")
      End Try

   End Sub
	Public Function GetWinNumFormat(bAreaMeter As Boolean) As String
		If CellDataType = enCellDataType.FormatNum Then
			Dim sGroupSymbol As String
			Dim iCurrentNumDigits As Integer
			If bAreaMeter Then
				iCurrentNumDigits = FormatNumDigitsM
			Else
				iCurrentNumDigits = FormatNumDigits
			End If
			Dim sFormat As String = Strings.FormatNumber(9990, iCurrentNumDigits, TriState.True, , GroupDigits)
			If InStr(sFormat, ",") = 0 Then
				sGroupSymbol = String.Empty
			Else
				sGroupSymbol = "#"
			End If
			sFormat = Replace(sFormat, "9", sGroupSymbol)
			Return sFormat
		Else
			Return String.Empty
		End If
	End Function
End Structure
Public Structure GridLine
	Public Type As GridLineType
	Public Weight As LineWeight
	Public Sub New(ByVal sDescr As String)
		Dim saElem() As String = Strings.Split(sDescr, "/")
		Dim iElemUB As Integer = saElem.GetUpperBound(0)
		Try
			If iElemUB >= 0 Then
				Me.Type = CType(Convert.ToInt32(saElem(0)), GridLineType)
				'		Me.Type = 0	''''''''''''''''''''''''''''''''''''''''''''''' TEMP
				'Me.Type = Convert.ToInt32(saElem(0))
			End If
			If iElemUB >= 1 Then
				Dim iGridLineWeight As Integer = Convert.ToInt32(saElem(1))
				Me.Weight = CType(iGridLineWeight, LineWeight)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "GridLine - New")
		End Try
	End Sub
End Structure
Public Enum CellFeatures
	[Default] = 0
	BackgroundFill = 1
	Zebra = 2
End Enum
Public Structure TableCell
	Const msInsDelim As String = "||"
	Const TextHeightDflt As Double = 1.0
	Public TextHeight As Double								  'Place 1
	Public TextAlignmentDefined As Boolean
	Public TextAlignment As CellAlignment							'Place 2
	Public CellValue() As CellData									'Place 3
	Public TextStyleIndex As Integer									'Place 4
	Public GridLines() As GridLine									'Place 5
	Public ContentColor As Color										'Place 6
	Public BackgroundColor As Color									'Place 7
	Public Feature As CellFeatures									'Place 8
	Public FormatNumDigits As Integer
	Public DestType As enDestType
	Public GroupIndex As Integer
	Public ResIndex As Integer
	Private msDescr As String
	Public Sub Open(ByVal sDescr As String, iGroupIndex As Integer, iResIndex As Integer)
		Dim saProp() As String = Nothing
		Dim iPropUB As Integer = -1
		Dim iTextAlighment As Integer
		Dim sInput As String = String.Empty
		GroupIndex = iGroupIndex
		ResIndex = iResIndex
		msDescr = sDescr

		'	System.Windows.Forms.MessageBox.Show(sDescr, "21_100 TableCell.Open(...)")
		DestType = enDestType.AcadTable
		Try
			saProp = Strings.Split(sDescr, ",")
			iPropUB = saProp.GetUpperBound(0)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - Open_1")
		End Try

		If iPropUB >= 0 Then                'TextHeight
			Try
				sInput = (saProp(0))
				If sInput.Length <> 0 Then
					TextHeight = Convert.ToDouble(sInput)
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sInput, "TableCell - Open_2")
			End Try
		End If
		If TextHeight = 0.0 Then
			TextHeight = TextHeightDflt
		End If
		If iPropUB >= 1 Then              'TextAlignment
			Try
				sInput = saProp(1)
				If sInput.Length = 0 Then
					TextAlignmentDefined = False
				Else
					iTextAlighment = Convert.ToInt32(sInput)
					TextAlignment = CType(iTextAlighment, CellAlignment)
					TextAlignmentDefined = True
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sInput, "TableCell - Open_3")
			End Try
		Else
			TextAlignment = CellAlignment.MiddleRight
			TextAlignmentDefined = False
		End If
		If iPropUB >= 2 Then                       'CellData
			Try
				If saProp(2).Length > 0 Then
					Dim saValue() As String = Strings.Split(saProp(2), "|")
					ReDim CellValue(saValue.GetUpperBound(0))
					For iIndex As Integer = 0 To saValue.GetUpperBound(0)
						CellValue(iIndex) = New CellData()
						CellValue(iIndex).Open(saValue(iIndex))
					Next
				Else
					ReDim CellValue(0)
					CellValue(0).CellDataType = enCellDataType.String
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - OPEN_4")
				ReDim CellValue(0)
				CellValue(0).CellDataType = enCellDataType.String

			End Try
		Else
			ReDim CellValue(0)
			CellValue(0).CellDataType = enCellDataType.String
		End If
		If iPropUB >= 3 Then

			Try
				sInput = saProp(3)	'TextStyleIndex
				If sInput.Length <> 0 Then
					TextStyleIndex = Convert.ToInt32(sInput)
				Else
					TextStyleIndex = 0
				End If

			Catch oEx As Exception
				TextStyleIndex = -1
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sInput, "TableCell - Open_5")
			End Try
		Else
			TextStyleIndex = 0
		End If
		If iPropUB >= 4 Then
			Try
				sInput = saProp(4)
				If sInput.Length <> 0 Then
					Dim sInnerInput() As String = Strings.Split(sInput, "|")
					ReDim GridLines(sInnerInput.GetUpperBound(0))
					For iIndex As Integer = 0 To sInnerInput.GetUpperBound(0)
						GridLines(iIndex) = New GridLine(sInnerInput(iIndex))
					Next
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - OPEN_6")
			End Try
		End If
		If iPropUB >= 5 Then
			Try
				Dim shInput As Short = 0
				sInput = saProp(5)
				If sInput.Length <> 0 Then
					shInput = Convert.ToInt16(sInput)
					ContentColor = Color.FromColorIndex(ColorMethod.ByAci, shInput)
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - OPEN_7")
				DMCommon.Debug.UserMsg("TableCell - OPEN_7", oEx.Message, sInput, oEx.StackTrace)
			End Try

		End If

		If iPropUB >= 6 Then
			Try
				Dim shInput As Short = 0
				sInput = saProp(6)
				If sInput.Length <> 0 Then
					shInput = Convert.ToInt16(sInput)
					BackgroundColor = Color.FromColorIndex(ColorMethod.ByAci, shInput)
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - OPEN_8")
			End Try
		End If
		If iPropUB >= 7 Then
			Try
				Dim iInput As Integer = 0
				sInput = saProp(7)
				If sInput.Length <> 0 Then
					iInput = Convert.ToInt32(sInput)
					If [Enum].IsDefined(GetType(CellFeatures), iInput) Then
						Feature = CType(iInput, CellFeatures)
					End If
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TableCell - OPEN_9")
			End Try
		End If
	End Sub
	Public ReadOnly Property GridLinesExist() As Boolean
		Get
			Return GridLines IsNot Nothing
		End Get
	End Property
	Public ReadOnly Property XlHAlign() As Integer
		Get
			Select Case TextAlignment
				Case CellAlignment.TopLeft, CellAlignment.MiddleLeft, CellAlignment.BottomLeft
					Return -4131
				Case CellAlignment.TopCenter, CellAlignment.MiddleCenter, CellAlignment.BottomCenter
					Return -4108
				Case CellAlignment.TopRight, CellAlignment.MiddleRight, CellAlignment.BottomRight
					Return -4152
				Case Else
					Return 0
			End Select

		End Get

	End Property
	Public ReadOnly Property XlVAlign() As Integer
		Get
			Select Case TextAlignment
				Case CellAlignment.TopLeft, CellAlignment.TopCenter, CellAlignment.TopRight
					Return -4160
				Case CellAlignment.MiddleLeft, CellAlignment.MiddleCenter, CellAlignment.MiddleRight
					Return -4108

				Case CellAlignment.BottomLeft, CellAlignment.BottomCenter, CellAlignment.BottomRight
					Return -4107


			End Select
		End Get
	End Property
	Public Function Format(ByVal oValue As System.Object, ByVal bExcel As Boolean, ByVal bAreaMeter As Boolean, Optional ByVal oaInsValue() As System.Object = Nothing, Optional dicData As Dictionary(Of Integer, System.Object) = Nothing) As String

		If oValue IsNot Nothing Then
			Dim sOut As String = oValue.ToString()



			If sOut.Contains(msInsDelim) AndAlso CellValue IsNot Nothing AndAlso CellValue.GetUpperBound(0) > 0 AndAlso oaInsValue IsNot Nothing Then
				If dicData Is Nothing Then
					sOut = zzGetVarValue(sOut, oaInsValue, bExcel, bAreaMeter)
				Else
					sOut = zzGetVarValue(sOut, dicData, bExcel, bAreaMeter)
				End If
				'''''''''''''''''''''''''



				sOut = zzFormat(sOut, 0, bExcel, bAreaMeter)
			Else
				sOut = zzFormat(oValue, 0, bExcel, bAreaMeter)
			End If

			Return sOut
		Else
			Return "---"
		End If

	End Function
	Public ReadOnly Property Description() As String
		Get
			If msDescr Is Nothing Then
				Return "Empty"
			Else
				Return msDescr
			End If
		End Get
	End Property
	Public ReadOnly Property IsEmpty() As Boolean
		Get
			Return (msDescr Is Nothing)
		End Get
	End Property
   Private Function zzFormat(ByVal oValue As System.Object, ByVal iDataIndex As Integer, ByVal bExcel As Boolean, ByVal bAreaMeter As Boolean) As String
      Dim sOut As String
      Dim iFormatNumDigits As Integer
      If IsDBNull(oValue) Then
         sOut = String.Empty & "DBNull: " & iDataIndex.ToString()
      ElseIf CellValue IsNot Nothing Then
         Try
            Select Case CellValue(iDataIndex).CellDataType
               Case enCellDataType.Invert, enCellDataType.Keyboard, enCellDataType.KeyboardInvert
                  sOut = DirectCast(oValue, String)
                  sOut = zzConvertLines(sOut, CellValue(iDataIndex).CellDataType)
                  If bExcel Then
                     sOut = zzToExcelText(sOut)
                  End If
                  If DestType = enDestType.AcadTable Then
                     ''''''''''''''''''''''''''  080812	sOut = RepApp.ToMText(sOut)  
                  End If

               Case enCellDataType.FormatNum
						If IsNumeric(oValue) Then
							iFormatNumDigits = CellValue(iDataIndex).FormatNumDigits
							If CellValue(iDataIndex).DFactor = -1 OrElse ((CellValue(iDataIndex).DFactor = 1) AndAlso (Not bAreaMeter)) Then
								oValue = 0.001 * Convert.ToDouble(oValue)
							End If
							If (CellValue(iDataIndex).DFactor = 1) AndAlso bAreaMeter Then
								iFormatNumDigits = Math.Max(0, iFormatNumDigits - 3)
							End If
							sOut = FormatNumber(oValue, iFormatNumDigits, TriState.True, , CellValue(iDataIndex).GroupDigits)
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ValFrm", oValue, sOut, CellValue(iDataIndex).FormatNumDigits, iFormatNumDigits, CellValue(iDataIndex).DFactor, bAreaMeter)
						Else
							sOut = String.Empty
						End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Desc=", oValue, iDataIndex, CellValue(iDataIndex).DFactor, bAreaMeter)
						'   DMAcadExt.AcadDocument.WriteDebugMessage("*--+!! " & CellValue(iDataIndex).DFactor & "; " & bAreaMeter.ToString() & "; " & sOut)
					Case enCellDataType.String

                  sOut = DMCommon.Functions.CStrN(oValue)  '''''''''''sValue = "'" & sValue
                  If bExcel Then
                     sOut = zzToExcelText(sOut)
                  End If
               Case enCellDataType.Num
						sOut = DMCommon.Functions.ValToString(oValue)
					Case Else
                  sOut = DMCommon.Functions.CStrN(oValue)
            End Select

         Catch oEx As Exception
            sOut = "TypeErr:" & oValue.ToString() & ","
            sOut &= oValue.GetType().ToString() & "#"
            sOut &= CStr(CellValue(iDataIndex).CellDataType)
         End Try
      Else
         sOut = oValue.ToString()
      End If

      Return sOut
   End Function
	Private Function zzGetVarValue(ByVal sValue As String, ByVal oaInsValue() As System.Object, ByVal bExcel As Boolean, ByVal bAreaMeter As Boolean) As String
		Dim saValue() As String = Strings.Split(sValue, msInsDelim)
		Dim iPartUB As Integer = saValue.GetUpperBound(0)
		Dim sNumber As String, iNumber As Integer
		Dim iIndex As Integer = 0
		Dim iPartIndex As Integer

		If iPartUB >= 2 Then
			Do
				iPartIndex = iIndex + iIndex + 1
				If iPartIndex > iPartUB Then Exit Do
				sNumber = saValue(iPartIndex)
				iNumber = -1
				Try
					iNumber = Convert.ToInt32(sNumber) + GroupIndex
				Catch oEx As Exception

				End Try
				If iNumber >= 0 AndAlso iNumber <= oaInsValue.GetUpperBound(0) Then
					If oaInsValue(iNumber) IsNot Nothing Then
						saValue(iPartIndex) = zzFormat(oaInsValue(iNumber), iIndex + 1, bExcel, bAreaMeter)
					Else
						saValue(iPartIndex) = String.Empty ' "zzGetVar"
					End If

				End If
				iIndex += 1
			Loop
			Return Join(saValue, String.Empty)
		Else
			Return sValue
		End If
	End Function
	Private Function zzGetVarValue(ByVal sValue As String, ByVal dicInsValues As Dictionary(Of Integer, System.Object), ByVal bExcel As Boolean, ByVal bAreaMeter As Boolean) As String
		Dim saValue() As String = Strings.Split(sValue, msInsDelim)
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AVarValueDic", sValue)
		'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!BVarValueDic", dicInsValues.Keys)
		'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!CVarValueDic", dicInsValues.Values)
		'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!EVarValueDic", saValue)


		Dim bInsExists As Boolean
		Dim iPartUB As Integer = saValue.GetUpperBound(0)
		Dim sNumber As String, iNumber As Integer
		Dim iIndex As Integer = 0
		Dim iPartIndex As Integer
		Dim oInsValue As System.Object = Nothing
		If iPartUB >= 2 Then
			Do
				iPartIndex = iIndex + iIndex + 1
				If iPartIndex > iPartUB Then Exit Do
				sNumber = saValue(iPartIndex)
				iNumber = -1
				Try
					iNumber = Convert.ToInt32(sNumber) + GroupIndex
				Catch oEx As Exception

				End Try

				If dicInsValues.TryGetValue(iNumber, oInsValue) Then
					saValue(iPartIndex) = zzFormat(oInsValue, iIndex + 1, bExcel, bAreaMeter)
					If oInsValue Is Nothing OrElse IsDBNull(oInsValue) Then
						bInsExists = False
					Else
						bInsExists = True
					End If
				Else
					bInsExists = False
					'saValue(iPartIndex) = String.Empty ' "zzGetVar"
				End If


				iIndex += 1
			Loop
			If bInsExists Then
				Return Join(saValue, String.Empty)
			Else
				Return String.Empty
			End If

		Else
			Return sValue
		End If
	End Function
	Private Function zzConvertLines(ByVal sValue As String, ByVal iCellDataType As enCellDataType) As String
		Dim sTest As String = ""
		Dim sOut As String = String.Empty
		Dim sHebWord As String = String.Empty
		Dim saLines() As String

		If sValue.Length = 0& Then
			Return String.Empty
		Else
			saLines = Strings.Split(sValue, "|*|")


			For iIndex As Integer = 0 To saLines.GetUpperBound(0)
				If DestType = enDestType.WinApp Then
					If sOut.Length <> 0 Then
						sOut &= Strings.Space(1)
					End If

					sOut &= saLines(iIndex)
				Else
					If sOut.Length <> 0 Then
						sOut &= vbCrLf
					End If
					If iCellDataType = enCellDataType.Invert Then
                  sOut &= DMCommon.Hebrew.WordToDOS(saLines(iIndex), True)    ''''''''''''080812 \ saLines(iIndex)	 '
					ElseIf iCellDataType = enCellDataType.Keyboard Then
						sOut &= DMCommon.Hebrew.WordToKeyboard(saLines(iIndex), False)
					ElseIf iCellDataType = enCellDataType.KeyboardInvert Then
						sOut = DMCommon.Hebrew.WordToKeyboard(saLines(iIndex), True) & sOut
					End If
				End If
			Next
			Return sOut
		End If
	End Function
	Private Function zzToExcelText(ByVal sValue As String) As String
		
		If IsNumeric(sValue) OrElse IsDate(sValue) OrElse zzIsExcelDate(sValue) Then
		
			Return "'" & sValue
		Else
			
			Return sValue
		End If

	End Function
	Private Function zzIsExcelDate(ByVal sValue As String) As Boolean
		Dim saDelim() As String = {"-", "/"}
		Dim sCurrentDelim As String = Nothing
		For iIndex As Integer = 0 To saDelim.GetUpperBound(0)
			If sValue.Contains(saDelim(iIndex)) Then
				sCurrentDelim = saDelim(iIndex)
				Exit For
			End If
		Next
		If sCurrentDelim IsNot Nothing Then
			Dim saValue() As String = Strings.Split(sValue, sCurrentDelim)
			If saValue.GetUpperBound(0) = 1 Then
				Return IsNumeric(saValue(0)) AndAlso IsNumeric(saValue(1))
			ElseIf saValue.GetUpperBound(0) = 2 Then
				Return IsNumeric(saValue(0)) AndAlso IsNumeric(saValue(1)) AndAlso IsNumeric(saValue(2))
			Else
				Return False
			End If
		Else
			Return False
		End If
	End Function
	Public Overrides Function ToString() As String
		Return msDescr & "|" & CStr(ResIndex) & "|" & CStr(GroupIndex)
	End Function
End Structure
