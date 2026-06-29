Option Explicit On
Option Strict On

Public Module modFunctions




	Public Function GetAddDescr(ByVal dArea As Double) As String
		'   Const sPrev = "מרפסת פתוחה בשטח "
		Const sPrev As String = "בשטח "
		Const sPast As String = " מ""ר"
		On Error Resume Next
		Return sPrev & Invert(Format(dArea, gsAreaFmt)) & sPast
	End Function
	Public Function GetFloorDescrAAA(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal sValue As String) As String
		Const sBasement As String = "מרתף "
		Const sFloor As String = "" ' "קומה "
		Const sFloor0 As String = "קרקע" '"קומת קרקע"
		Const sMiddle1 As String = "גלריה"
		Const sMiddle2 As String = "מפלס"
		Const sRoof As String = "גג" '"קומת גג"
		Const sRoofToop As String = "גג עליון" '"קומת גג עליון"
		Dim iPointPos As Integer
		Dim iValue As Integer

		On Error Resume Next
		iPointPos = InStr(sValue, ".")
		If iPointPos = 0 Then
			iValue = Convert.ToInt32(sValue)
		Else
			iValue = 200 + Convert.ToInt32(Mid$(sValue, iPointPos + 1))
		End If
		If Err.Number <> 0& Then
			zzGetAttribErr(iPolygonID, sTag, sValue)
			Return String.Empty
		Else
			Select Case iValue
				Case Is < -1&
					Return sBasement & CStr(iValue)
				Case -1
					Return sBasement
				Case 0
					Return sFloor0
				Case 1 To 50
					Return sFloor & zzGetOrdinal(iValue, True)
				Case 100
					Return sRoof
				Case 101
					Return sRoofToop
				Case 205
					Return sMiddle1
				Case 206
					Return sMiddle2
				Case Else
					Return "שגיאה"
					zzGetAttribErr(iPolygonID, sTag, sValue)
			End Select
		End If
	End Function
	Public Sub ParseBldFloor(sBldFloor As String, ByRef iBldFloor As Integer?, ByRef iBldSubFloor As Integer?)
		Dim saValue() As String = Split(sBldFloor, ".")
		Dim iBldFloorVal As Integer
		Dim iBldSubFloorVal As Integer

		If Integer.TryParse(saValue(0), iBldFloorVal) Then
			iBldFloor = iBldFloorVal
			If (saValue.GetUpperBound(0) > 0 AndAlso Integer.TryParse(saValue(1), iBldSubFloorVal)) Then
				iBldSubFloor = iBldSubFloorVal
			Else
				iBldSubFloor = 0
			End If
		Else
			iBldFloor = -100
		End If
	End Sub
	Public Function GetFloorDescr2015(ByVal iPolygonID As Integer, ByVal sTag As String, sBldFloor As String) As String
		Dim saValue() As String = Split(sBldFloor, ".")
		Dim iBldFloor As Integer?
		Dim iBldSubFloor As Integer?
		'Dim iBldFloorVal As Integer
		'	Dim iBldSubFloorVal As Integer
		'	If iBldFloor.HasValue Then
		'	iBldFloorVal = iBldFloor.Value
		'	End If
		'If iBldSubFloor.HasValue Then
		'iBldSubFloorVal = iBldSubFloor.Value
		'End If
		ParseBldFloor(sBldFloor, iBldFloor, iBldSubFloor)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "GetFloorDescr2015/3", sBldFloor, iBldFloor, iBldSubFloor)
		If iBldFloor.HasValue Then
			Return GetFloorDescr2015(iPolygonID, "Bldfloordesc", iBldFloor.Value, iBldSubFloor.Value)
		Else
			Return Nothing
		End If
	End Function
	Public Function GetFloorDescr2015(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal iBldFloor As Integer, ByVal iBldSubFloor As Integer) As String


		Const sBasement As String = "מרתף "
		Const sFloor As String = "" ' "קומה "
		Const sFloor0 As String = "קרקע" '"קומת קרקע"
		Const sMiddle1 As String = "גלריה"
		Const sMiddle2 As String = "מפלס"
		Const sRoof As String = "גג" '"קומת גג"
		Const sRoofToop As String = "גג עליון" '"קומת גג עליון"

		Dim iValue As Integer
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "GetFloorDescr2015/4", iBldFloor, iBldSubFloor)
		If iBldSubFloor = 0 Then
			iValue = iBldFloor
		Else
			iValue = 200 + iBldSubFloor
		End If

		Select Case iValue
			Case Is < -1
				Return sBasement & CStr(iValue)
			Case -1
				Return sBasement
			Case 0
				Return sFloor0
			Case 1 To 50
				Return sFloor & zzGetOrdinalB(iValue)
				'Return sFloor & zzGetOrdinalA(iValue, False)
			Case 100
				Return sRoof
			Case 101
				Return sRoofToop
			Case 205
				Return sMiddle1
			Case 206
				Return sMiddle2
			Case Else
				Return "שגיאה"
				zzGetAttribErr(iPolygonID, sTag, CStr(iBldFloor) & "." & CStr(iBldSubFloor))
		End Select

	End Function
	Public Function GetFloorDescr2009(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal iFloor As Integer, ByVal iSubFloor As Integer) As String
		Const sBasement As String = "מרתף "
		Const sFloor As String = "" ' "קומה "
		Const sFloor0 As String = "קרקע"	'"קומת קרקע"
		Const sMiddle1 As String = "גלריה"
		Const sMiddle2 As String = "מפלס"
		Const sRoof As String = "גג" '"קומת גג"
		Const sRoofToop As String = "גג עליון"	'"קומת גג עליון"

		Dim iValue As Integer

		If iSubFloor = 0 Then
			iValue = iFloor
		Else
			iValue = 200 + iSubFloor
		End If



		Select Case iValue
			Case Is < -1
				Return sBasement & CStr(iValue)
			Case -1
				Return sBasement
			Case 0
				Return sFloor0
			Case 1 To 50
				Return sFloor & zzGetOrdinal(iValue, True)
			Case 100
				Return sRoof
			Case 101
				Return sRoofToop
			Case 205
				Return sMiddle1
			Case 206
				Return sMiddle2
			Case Else
				Return "שגיאה"
				zzGetAttribErr(iPolygonID, sTag, CStr(iFloor) & "." & CStr(iSubFloor))
		End Select

	End Function
	Private Function zzGetOrdinalOld(ByVal iValue As Integer, ByVal bCrLf As Boolean) As String

		'  Const s10Plus As String = "עשרה"
		Dim sDel As String
		If bCrLf Then
			sDel = vbCrLf
		Else
			sDel = " "
		End If
		Select Case iValue
			Case 1
				Return "ראשונה"
			Case 2
				Return "שנייה"
			Case 3
				Return "שלישית"
			Case 4
				Return "רביעית"
			Case 5
				Return "חמישית"
			Case 6
				Return "שישית"
			Case 7
				Return "שביעית"
			Case 8
				Return "שמינית"
			Case 9
				Return "תשיעית"
			Case 10
				Return "עשירית"
			Case 11
				Return "האחת" & sDel & "עשרה"
			Case 12
				Return "השתים" & sDel & "עשרה"
			Case 13
				Return "השלוש" & sDel & "עשרה"
			Case 14
				Return "הארבע" & sDel & "עשרה"
			Case 15
				Return "החמש" & sDel & "עשרה"
			Case 16
				Return "השש" & sDel & "עשרה"
			Case 17
				Return "השבע" & sDel & "עשרה"
			Case 18
				Return "השמונה" & sDel & "עשרה"
			Case 19
				Return "התשע" & sDel & "עשרה"
			Case 20
				Return "העשרים"
			Case 21
				Return "העשרים ואחת"
			Case 22
				Return "העשרים" & sDel & "ושתים"
			Case 23
				Return "העשרים" & sDel & "ושלוש"
			Case 24
				Return "העשרים" & sDel & "וארבע"
			Case 25
				Return "העשרים" & sDel & "וחמש"
			Case 26
				Return "העשרים" & sDel & "ושש"
			Case 27
				Return "העשרים" & sDel & "ושבע"
			Case 28
				Return "העשרים" & sDel & "ושמונה"
			Case 29
				Return "העשרים" & sDel & "ותשע"
			Case 30
				Return "השלושים"

			Case Else
				Return "שגיאה"
		End Select

	End Function
	Private Function zzGetOrdinalB(ByVal iValue As Integer) As String
		'  Const sPrefix As String = "ה"

		Dim sUnits() As String = {String.Empty, "אחת", "שתים", "שלוש", "ארבע", "חמש", "שש", "שבע", "שמונה", "תשע"}
		Dim sOrdinalUnits() As String = {"", "ראשונה", "שנייה", "שלישית", "רביעית", "חמישית", "שישית", "שביעית", "שמינית", "תשיעית", "עשירית"}
		Dim sTens() As String = {"עשרה", "עשרים", "שלושים", "ארבעים", "חמישים", "שישים", "שבעים", "שמונים", "תשעים"}
		Dim iUnits As Integer = iValue Mod 10
		Dim iTens As Integer = (iValue - iUnits) \ 10



		Const sSpace As String = " "
		Const sAND As String = "ו"

		Dim sDel As String
		Dim sUnion As String
		If iUnits = 0 Then
			sDel = String.Empty
			sUnion = String.Empty
		Else
			sDel = sSpace
			sUnion = sAND
		End If
		Select Case iValue
			Case Is <= 10
				Return sOrdinalUnits(iValue)
			Case Is < 20
				Return sUnits(iUnits) & sDel & sTens(0)
			Case Is < 100
				Return sTens(iTens - 1) & sDel & sUnion & sUnits(iUnits)
			Case Else
				Return "שגיאה"
		End Select

	End Function

	Private Function zzGetOrdinalA(ByVal iValue As Integer, ByVal bCrLf As Boolean) As String
      '  Const sPrefix As String = "ה"
      Const s10Plus As String = "עשרה"
      Const s20Plus As String = "עשרים"
      Const s30Plus As String = "שלושים"


      Dim sDel As String
      If bCrLf Then
         sDel = vbCrLf
      Else
         sDel = " "
      End If
      Select Case iValue
         Case 1
            Return "ראשונה"
         Case 2
            Return "שנייה"
         Case 3
            Return "שלישית"
         Case 4
            Return "רביעית"
         Case 5
            Return "חמישית"
         Case 6
            Return "שישית"
         Case 7
            Return "שביעית"
         Case 8
            Return "שמינית"
         Case 9
            Return "תשיעית"
         Case 10
            Return "עשירית"
         Case 11
            Return "אחת" & sDel & s10Plus
         Case 12
            Return "שתים" & sDel & s10Plus
         Case 13
            Return "שלוש" & sDel & s10Plus
         Case 14
            Return "ארבע" & sDel & s10Plus
         Case 15
            Return "חמש" & sDel & s10Plus
         Case 16
            Return "שש" & sDel & s10Plus
         Case 17
            Return "שבע" & sDel & s10Plus
         Case 18
            Return "שמונה" & sDel & s10Plus
         Case 19
            Return "תשע" & sDel & s10Plus
         Case 20
            Return s20Plus
         Case 21
            Return s20Plus & sDel & "ואחת"
         Case 22
            Return s20Plus & sDel & "ושתים"
         Case 23
            Return s20Plus & sDel & "ושלוש"
         Case 24
            Return s20Plus & sDel & "וארבע"
         Case 25
            Return s20Plus & sDel & "וחמש"
         Case 26
            Return s20Plus & sDel & "ושש"
         Case 27
            Return s20Plus & sDel & "ושבע"
         Case 28
            Return s20Plus & sDel & "ושמונה"
         Case 29
            Return s20Plus & sDel & "ותשע"
         Case 30
            Return s30Plus
         Case 31
            Return sDel & s30Plus & "ואחת"
         Case 32
            Return sDel & s30Plus & "ושתים"
         Case 33
            Return sDel & s30Plus & "ושלוש"
         Case 34
            Return sDel & s30Plus & "וארבע"
         Case 35
            Return sDel & s30Plus & "וחמש"
         Case 36
            Return sDel & s30Plus & "ושש"
         Case 37
            Return sDel & s30Plus & "ושבע"
         Case 38
            Return sDel & s30Plus & "ושמונה"
         Case 39
            Return sDel & s30Plus & "ותשע"
         Case Else
            Return "שגיאה"
      End Select

   End Function
   Private Function zzGetOrdinal(ByVal iValue As Integer, ByVal bCrLf As Boolean) As String

      Const s10Plus As String = "עשרה"
      Const s20Plus As String = "העשרים"
      Const s30Plus As String = "השלושים"


      Dim sDel As String
      If bCrLf Then
         sDel = vbCrLf
      Else
         sDel = " "
      End If
      Select Case iValue
         Case 1
            Return "ראשונה"
         Case 2
            Return "שנייה"
         Case 3
            Return "שלישית"
         Case 4
            Return "רביעית"
         Case 5
            Return "חמישית"
         Case 6
            Return "שישית"
         Case 7
            Return "שביעית"
         Case 8
            Return "שמינית"
         Case 9
            Return "תשיעית"
         Case 10
            Return "עשירית"
         Case 11
            Return s10Plus & sDel & "האחת"
         Case 12
            Return s10Plus & sDel & "השתים"
         Case 13
            Return s10Plus & sDel & "השלוש"
         Case 14
            Return s10Plus & sDel & "הארבע"
         Case 15
            Return s10Plus & sDel & "החמש"
         Case 16
            Return s10Plus & sDel & "השש"
         Case 17
            Return s10Plus & sDel & "השבע"
         Case 18
            Return s10Plus & sDel & "השמונה"
         Case 19
            Return s10Plus & sDel & "התשע"
         Case 20
            Return s20Plus
         Case 21
            Return "ואחת" & sDel & s20Plus
         Case 22
            Return "ושתים" & sDel & s20Plus
         Case 23
            Return "ושלוש" & sDel & s20Plus
         Case 24
            Return "וארבע" & sDel & s20Plus
         Case 25
            Return "וחמש" & sDel & s20Plus
         Case 26
            Return "ושש" & sDel & s20Plus
         Case 27
            Return "ושבע" & sDel & s20Plus
         Case 28
            Return "ושמונה" & sDel & s20Plus
         Case 29
            Return "ותשע" & sDel & s20Plus
         Case 30
            Return s30Plus
         Case 31
            Return "ואחת" & sDel & s30Plus
         Case 32
            Return "ושתים" & sDel & s30Plus
         Case 33
            Return "ושלוש" & sDel & s30Plus
         Case 34
            Return "וארבע" & sDel & s30Plus
         Case 35
            Return "וחמש" & sDel & s30Plus
         Case 36
            Return "ושש" & sDel & s30Plus
         Case 37
            Return "ושבע" & sDel & s30Plus
         Case 38
            Return "ושמונה" & sDel & s30Plus
         Case 39
            Return "ותשע" & sDel & s30Plus
         Case Else
            Return "שגיאה"
      End Select

   End Function

	
	Private Function zzGetOrdinalOld(ByVal lValue As Long, ByVal bCrLf As Boolean) As String
		Dim sDel As String
		If bCrLf Then
			sDel = vbCrLf
		Else
			sDel = " "
		End If
		Select Case lValue
			Case 1&
				zzGetOrdinalOld = "ראשונה"
			Case 2&
				zzGetOrdinalOld = "שנייה"
			Case 3&
				zzGetOrdinalOld = "שלישית"
			Case 4&
				zzGetOrdinalOld = "רביעית"
			Case 5&
				zzGetOrdinalOld = "חמישית"
			Case 6&
				zzGetOrdinalOld = "שישית"
			Case 7&
				zzGetOrdinalOld = "שביעית"
			Case 8&
				zzGetOrdinalOld = "שמינית"
			Case 9&
				zzGetOrdinalOld = "תשיעית"
			Case 10&
				zzGetOrdinalOld = "עשירית"
			Case 11&
				zzGetOrdinalOld = "האחת-עשרה"
			Case 12&
				zzGetOrdinalOld = "השתים-עשרה"
			Case 13&
				zzGetOrdinalOld = "השלוש-עשרה"
			Case 14&
				zzGetOrdinalOld = "הארבע-עשרה"
			Case 15&
				zzGetOrdinalOld = "החמש-עשרה"
			Case 16&
				zzGetOrdinalOld = "השש-עשרה"
			Case 17&
				zzGetOrdinalOld = "השבע-עשרה"
			Case 18&
				zzGetOrdinalOld = "השמונה-עשרה"
			Case 19&
				zzGetOrdinalOld = "התשע-עשרה"
			Case 20&
				zzGetOrdinalOld = "העשרים"
			Case 21&
				zzGetOrdinalOld = "העשרים ואחת"
			Case 22&
				zzGetOrdinalOld = "העשרים ושתים"
			Case 23&
				zzGetOrdinalOld = "העשרים ושלוש"
			Case 24&
				zzGetOrdinalOld = "העשרים וארבע"
			Case 25&
				zzGetOrdinalOld = "העשרים וחמש"
			Case 26&
				zzGetOrdinalOld = "העשרים ושש"
			Case 27&
				zzGetOrdinalOld = "העשרים ושבע"
			Case 28&
				zzGetOrdinalOld = "העשרים ושמונה"
			Case 29&
				zzGetOrdinalOld = "העשרים ותשע"
			Case 30&
				zzGetOrdinalOld = "השלושים"

			Case Else
				zzGetOrdinalOld = "שגיאה"
		End Select

	End Function
	

	Private Sub zzGetHebLetter(ByVal iInput As Integer, ByRef sHeb As String, ByRef iVal As Integer)
		Dim iBase As Integer
		Dim iDec As Integer, iHundred As Integer
		Dim iAddit As Integer
		iBase = Asc("א") - 1
		If iInput <= 10 Then
			sHeb = Chr(iBase + iInput)
			iVal = iInput
		ElseIf iInput = 15 OrElse iInput = 16 Then
			sHeb = "ט" & Chr(iBase + iInput - 9)
			iVal = iInput
		ElseIf iInput <= 100& Then
			iDec = iInput \ 10
			If iDec >= 2 Then iAddit = 1
			If iDec >= 4 Then iAddit = 2
			If iDec >= 5 Then iAddit = 3
			If iDec >= 8 Then iAddit = 4
			If iDec >= 9 Then iAddit = 5
			sHeb = Chr(iBase + 9 + iDec + iAddit)
			iVal = iDec * 10
		Else
			If iInput > 400 Then iInput = 400
			iHundred = iInput \ 100
			sHeb = Chr(iBase + 23 + iHundred + iAddit)
			iVal = iHundred * 100
		End If
	End Sub

	Public Sub zzGetAttribErr(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal sValue As String)
		Dim sMsg As String
		sMsg = "PolygonID=" & CStr(iPolygonID) & " Tag=" & sTag & " Value=" & sValue & " not valid"
		DMAcadExt.AppMessages.AddMessage(False, 0.0, 0.0, "", sMsg, False)
	End Sub
	Public Function CStrN(ByVal oFieldValue As System.Object) As String
		If IsDBNull(oFieldValue) Then
			Return String.Empty
		Else
			Return CStr(oFieldValue)
		End If

	End Function
	Public Function CIntN(ByVal oFieldValue As System.Object, Optional iValue As Integer = 0) As Integer
		If IsDBNull(oFieldValue) Then
			Return iValue
		Else
			Return DirectCast(oFieldValue, Integer)
		End If

	End Function
	Public Sub CIntN(ByVal oFieldValue As System.Object, ByRef iResultValue As Integer?)
		If Not IsDBNull(oFieldValue) Then

			iResultValue = CInt(oFieldValue)
		End If

	End Sub
	Public Function CDblN(ByVal oFieldValue As System.Object) As Double
		If IsDBNull(oFieldValue) Then
			Return 0
		Else
			Return CDbl(oFieldValue)
		End If

	End Function
	Public Function CDblNth(ByVal oFieldValue As System.Object) As Double
		If oFieldValue Is Nothing Then
			Return 0
		Else
			Return CDbl(oFieldValue)
		End If

	End Function
	Public Function CStrNSpace(ByVal oFieldValue As System.Object) As String
		On Error Resume Next
		If IsDBNull(oFieldValue) Then
			Return String.Empty
		ElseIf CInt(oFieldValue) = 0 Then
			Return String.Empty
		Else
			Return CStr(oFieldValue)
		End If

	End Function
	Public Sub GetUpdAttributes(ByRef vsaAttributes(,) As System.Object, ByVal bAttrib0 As Boolean)
		ReDim vsaAttributes(1, 3)
		If bAttrib0 Then vsaAttributes(0&, 0&) = UCase("SubParcelNo")
		vsaAttributes(0&, 1&) = UCase("Polygonid")
		vsaAttributes(0&, 2&) = UCase("Aprtdesc")
		vsaAttributes(0&, 3&) = UCase("Aprtdesc2")
	End Sub
	Public Function GetWinColor(ByVal iColorIndex As Integer) As System.Drawing.Color

	End Function
	Public Function GetFrameworkColor(ByVal iColor As Integer) As System.Drawing.Color

	End Function
	Public Sub InitParams()





		gdPrmZebraAngle = ToDblParam("ZebraAngle", gdPrmZebraAngleDflt)

		gdPrmZebraWidth = ToDblParam("ZebraWidth", gdPrmZebraWidthDflt)

		gdPrmLegendZebraWidth = ToDblParam("LegendZebraWidth", gdPrmLegendZebraWidthDflt)

		gdPrmPaintScale = ToDblParam("PaintScale", gdPrmPaintScaleDflt)

		gdPrmBorderWidth = ToDblParam("BorderWidth", gdPrmBorderWidthDflt)

		gdPrmDissolveBorderWidth = ToDblParam("DissolveBorderWidth", gdPrmDissolveBorderWidthDflt)

		giPrmDissolveBorderColor = Color.Red.ToArgb
		giPrmDissolveBorderColor = ToIntParam("DissolveBorderColor", giPrmDissolveBorderColor)

		gbPrmExproZebra = ToBoolParam("ExproZebra", gbPrmExproZebraDflt)

		If Len(gsInpBlock) = 0& Then
			gsInpBlock = "99999"
			gsInpBlock = GetSetting(gsAppName, gsRegSectionParameters, "Block", gsInpBlock)
		End If
		If Len(gsInpParcel) = 0& Then
			gsInpParcel = "99"
			gsInpParcel = GetSetting(gsAppName, gsRegSectionParameters, "Parcel", gsInpParcel)
		End If

		gdPrmInterval = Convert.ToDouble(GetSetting(gsAppName, gsRegSectionParameters, "Interval", CStr(gdPrmIntervalDflt)))
		gsPrmLayerPaint = GetSetting(gsAppName, gsRegSectionParameters, "LayerPaint", gsPrmLayerPaintDflt)
		gsPrmLayerTable = GetSetting(gsAppName, gsRegSectionParameters, "LayerTable", gsPrmLayerTableDflt)

		giPrmDec = ToIntParam("Decimal", giPrmDecDflt)
		gsAreaFmt = "0." & Strings.StrDup(giPrmDec, "0")


		gbPrmMainNum = ToBoolParam("MainNum", gbPrmMainNumDflt)
		gbPrmSubNum = ToBoolParam("SubNum", gbPrmSubNumDflt)


	End Sub
	Public Function zzGetColorName(ByVal iAcadColor As System.Drawing.Color) As String
		Select Case iAcadColor
			Case System.Drawing.Color.Blue
				Return "כחול"
			Case System.Drawing.Color.Cyan
				'  return "טורקיז"
				Return "תכלת"
			Case System.Drawing.Color.Green
				Return "ירוק"
			Case System.Drawing.Color.Magenta
				Return "סגול"
			Case System.Drawing.Color.Red
				Return "אדום"
			Case System.Drawing.Color.White
				Return "לבן"
			Case System.Drawing.Color.Yellow
				Return "צהוב"
			Case Else
				Return iAcadColor.ToString()
				'	Return "שגיאה"
		End Select
	End Function
	Public Function SplitToInt(ByVal sPropID As String, ByVal iPolygonID As Integer) As Integer()
		Dim saValue() As String = Split(sPropID, ",")
		Dim iUB As Integer = saValue.GetUpperBound(0)
		Dim iaOutput(iUB) As Integer
		Try
			For iIndex As Integer = 0 To iUB
				iaOutput(iIndex) = Convert.ToInt32(saValue(iIndex))
			Next
		Catch oEx As Exception
			zzGetAttribErr(iPolygonID, "Propid", sPropID)
		End Try
		Return iaOutput

	End Function

	Public Function SetActiveLayer(ByVal sLayerName As String, Optional ByVal iColor As Integer = -1&) As Integer

	End Function
	Public Sub DispArray(ByVal saValue() As String, ByVal sCaption As String)
		Dim sOut As String = CStr(saValue.GetUpperBound(0) + 1) & ":"
		For iIndex As Integer = 0 To saValue.GetUpperBound(0)
			sOut &= ","
			sOut &= saValue(iIndex)
		Next
		MessageBox.Show(sOut, sCaption)
	End Sub
	Public Sub DispArray(ByVal iaValue() As Integer, ByVal sCaption As String)
		Dim sOut As String = "Count=" & CStr(iaValue.GetUpperBound(0) + 1) & vbCrLf
		For iIndex As Integer = 0 To iaValue.GetUpperBound(0)
			If iIndex <> 0 Then
				sOut &= ","
			End If

			sOut &= CStr(iaValue(iIndex))
		Next
		MessageBox.Show(sOut, sCaption)
	End Sub
	Public Function zzGetZebraColors(ByVal viaPropID() As Integer) As Integer()

		Dim iColorsUB As Integer
		Dim miColorIndex As Integer
		iColorsUB = viaPropID.GetUpperBound(0)
		Dim vlaColors(iColorsUB) As Integer
		For iIndex As Integer = 0 To iColorsUB
			If gdicColors.ContainsKey(viaPropID(iIndex)) Then
				miColorIndex = gdicColors.Item(viaPropID(iIndex))
				vlaColors(iIndex) = miColorIndex
			Else
				MessageBox.Show(CStr(viaPropID(iIndex)), "zzGetZebraColors 12_391")
			End If
		Next
		Return vlaColors
	End Function
	Public Function zzGetZebraColorsNet(ByVal viaPropID() As Integer) As DMAcadExt.DMColor()

		Dim iColorsUB As Integer
		Dim iColorIndex As Integer
		Dim shColorIndex As Short
		iColorsUB = viaPropID.GetUpperBound(0)
		Dim vlaColors(iColorsUB) As DMAcadExt.DMColor
		For iIndex As Integer = 0 To iColorsUB
			If gdicColors.ContainsKey(viaPropID(iIndex)) Then
				iColorIndex = gdicColors.Item(viaPropID(iIndex))
				shColorIndex = Convert.ToInt16(iColorIndex)
				vlaColors(iIndex) = New DMAcadExt.DMColor(shColorIndex)
			Else
				MessageBox.Show(CStr(viaPropID(iIndex)), "zzGetZebraColors 12_393")
			End If
		Next
		Return vlaColors
	End Function
	Public Function ToDOS(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer

		If Len(sValue) = 0& Then
			Return String.Empty

		End If
		For iIndex As Integer = 1 To Len(sValue)
			iASCCode = Asc(Mid(sValue, iIndex, 1&))
			Select Case iASCCode
				Case 224 To 250
					sOut = Strings.Chr(iASCCode - 96) & sOut
				Case Else
					sOut = sOut & Strings.Chr(iASCCode)
			End Select
		Next
		Return sOut
	End Function
	 
	Public Function TestStr(sInput As String) As String
		Dim sAsc As String = ""
		Dim sAscW As String = ""
		Dim sChr As String = ""
		Dim sChrW As String = ""
		Dim sAlef As String = ""
		Dim sAlefW As String = ""


		Dim iAscCode As Integer
		Dim iAscWCode As Integer
		Dim sLetter As String
		'	Dim iBase As Integer
	 
		'	iBase = Asc("א") - 1

		For iIndex As Integer = 1 To Len(sInput)
			sLetter = Mid(sInput, iIndex, 1)
			iAscCode = Asc(sLetter)
			iAscWCode = AscW(sLetter)
			sAsc &= CStr(iAscCode) & ":"
			sAscW &= Hex(iAscWCode) & "^"
			'		sChrW &= ChrW(iAscWCode)
			'		sAlef &= Chr(228 + iIndex)
			'		sAlefW &= ChrW(iBase + iIndex)
		Next
		Return sAsc & vbCrLf & sAscW
	End Function
	Public Function Invert(ByVal sValue As String) As String

		Dim sOut As String = String.Empty

		Dim sLetter As String

		If Len(sValue) = 0& Then
			Invert = ""
			Exit Function
		End If
		For iIndex As Integer = 1& To Len(sValue)
			sLetter = Mid(sValue, iIndex, 1&)
			sOut = sLetter & sOut
		Next
		Invert = sOut
	End Function
	Public Function GetApartDescrAAA(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal sValue As String) As String
		Dim sMsg As String
		Dim iValue As Integer
		On Error Resume Next
		iValue = Convert.ToInt32(sValue)
		If Err.Number <> 0& Then
			zzGetAttribErr(iPolygonID, sTag, sValue)
			Return Nothing
		Else
			Select Case iValue
				Case 1
					Return "דירה"
				Case 2
					Return "חנות"
				Case 3
					Return "אולם"
				Case 4
					Return "משרד"
				Case 5
					Return "מחסן"
				Case 6
					Return "חניה"
				Case 7
					Return "גג"
				Case 8
					Return "קרקע"
				Case 9
					Return "קרקע לרבות הקרקע מתחת לדירה וכל הבנוי עליה"
					' 11-01-2002 Boris
					'      return "קרקע וכל הבנוי מעליה לרבות הקרקע מתחת למבנה"
				Case 10
					Return "סככה"
				Case 11
					Return "דירת קוטג'"
				Case 12
					Return "מרפסת לא מקורה"
				Case 13
					Return "מרפסת לא מקורה"

				Case 99
					Return String.Empty
				Case Else
					zzGetAttribErr(iPolygonID, sTag, sValue)
					Return "שגיאה"
			End Select
		End If
	End Function
	Public Function GetApartDescr(ByVal iValue As Integer) As String  'New Version 2020
		Select Case iValue
			Case 1
				Return "דירה"
			Case 2
				Return "חנות"
			Case 3
				Return "אולם"
			Case 4
				Return "משרד"
			Case 5
				Return "מחסן"
			Case 6
				Return "חניה"
			Case 7
				Return "גג"
			Case 8
				Return "קרקע"
			Case 9
				Return "קרקע לרבות הקרקע מתחת לדירה וכל הבנוי עליה"
					' 11-01-2002 Boris
					'      Return "קרקע וכל הבנוי מעליה לרבות הקרקע מתחת למבנה"
			Case 10
				Return "סככה"
			Case 11
				Return "דירת קוטג'"
			Case 12
				Return "מרפסת לא מקורה"
			Case 13
				Return "משטח"
			Case 14
				Return "מרפסת"
			Case 99
				Return String.Empty
			Case Else
				Return Nothing
		End Select

	End Function
	Public Function GetApartDescr(ByVal iPolygonID As Integer, ByVal sTag As String, ByVal iValue As Integer) As String
		Dim sMsg As String

		On Error Resume Next
		If Err.Number <> 0 Then
			zzGetAttribErr(iPolygonID, sTag, CStr(iValue))
			Return Nothing
		Else
			Select Case iValue
				Case 1
					Return "דירה"
				Case 2
					Return "חנות"
				Case 3
					Return "אולם"
				Case 4
					Return "משרד"
				Case 5
					Return "מחסן"
				Case 6
					Return "חניה"
				Case 7
					Return "גג"
				Case 8
					Return "קרקע"
				Case 9
					Return "קרקע לרבות הקרקע מתחת לדירה וכל הבנוי עליה"
					' 11-01-2002 Boris
					'      Return "קרקע וכל הבנוי מעליה לרבות הקרקע מתחת למבנה"
				Case 10
					Return "סככה"
				Case 11
					Return "דירת קוטג'"
				Case 12
					Return "מרפסת לא מקורה"
				Case 13
					Return "משטח"
				Case 14
					Return "מרפסת"
				Case 99
					Return String.Empty
				Case Else
					zzGetAttribErr(iPolygonID, sTag, CStr(iValue))
					Return "שגיאה"
			End Select
		End If
	End Function

	Private Function ToDblParam(ByVal sKey As String, ByVal dParamDflt As Double) As Double
		Return Convert.ToDouble(GetSetting(gsAppName, gsRegSectionParameters, sKey, CStr(dParamDflt)))

	End Function
	Private Function ToIntParam(ByVal sKey As String, ByVal iParamDflt As Integer) As Integer
		Return Convert.ToInt32(GetSetting(gsAppName, gsRegSectionParameters, sKey, CStr(iParamDflt)))

	End Function
	Private Function ToBoolParam(ByVal sKey As String, ByVal bParamDflt As Boolean) As Boolean
		Return Convert.ToBoolean(GetSetting(gsAppName, gsRegSectionParameters, sKey, CStr(bParamDflt)))

	End Function
End Module
