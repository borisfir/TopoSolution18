Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop
Imports System.Windows.Forms
<Assembly: CLSCompliant(False)>


Public Class ExcelAppExt_1
	Private Structure Caption
		Dim Label As String
		Dim Value As String
		Sub New(sLabel As String, sValue As String)
			Label = sLabel
			Value = sValue
		End Sub
	End Structure

	Private Const mdK1 As Double = 0.128
	Private Const mdK2 As Double = 0.5
	Private Const miBackColor As Integer = 12632256
	Private Const miLinestyle As Integer = 1

	'Private Shared moExcelApp1 As Excel.Application
	Private moExcelApp As Excel.Application

	'Private Shared moWorkbook1 As Excel.Workbook
	Private Shared moWorkbook As Excel.Workbook

	'Private Shared moWorksheet1 As Excel.Worksheet
	Private Shared moWorksheet As Excel.Worksheet
	Private moWindow As Excel.Window
	'Private Shared miCurrentRow1 As Integer = 0
	Private miCurrentRow As Integer = 0

	Public Delegate Function HeaderValue(ByVal iHeaderRow As Integer, ByVal iColumn As Integer) As String
	Private Shared mbDebug As Boolean = False
	Private Shared mbDebugBoris As Boolean = True
	Private miDataFirstRow As Integer = 0
	Private maCaptions As List(Of Caption)

	Public Enum enBorderWeight
		Missing
		Hairline
		Thin
		Medium
		Thick
	End Enum


	Public Sub New()

	End Sub
	Public ReadOnly Property Worksheet As Excel.Worksheet
		Get
			Return moWorksheet
		End Get
	End Property
	Public Sub Activate()
		If moExcelApp IsNot Nothing Then

			moExcelApp.Interactive = True
			moExcelApp.Application.ActiveWindow.Activate()
			DMCommon.Debug.MsgBox("!Caption", moExcelApp.Application.ActiveWindow.Caption)
		End If
	End Sub
	Public Function IsDead() As Boolean
		'	Dim sResp As String
		Dim sWorkbookName As String
		Dim sWorksheetName As String

		'	DMCommon.Debug.UserMsg("!Test3!!!", moExcelApp Is Nothing, moWorkbook Is Nothing, moWorksheet Is Nothing)
		If moWorksheet Is Nothing Then
			moExcelApp = Nothing
			moWorkbook = Nothing
			'	moWorksheet = Nothing
			IsDead = True
		Else
			Try
				sWorkbookName = moWorkbook.Name
				sWorksheetName = moWorksheet.Name
				If moWorksheet.Rows.Count <= 0 Then
					IsDead = True
					moExcelApp = Nothing
					moWorkbook = Nothing
					moWorksheet = Nothing
				End If

			Catch oEx As Exception
				IsDead = True
				moExcelApp = Nothing
				moWorkbook = Nothing
				moWorksheet = Nothing
			End Try
		End If
	End Function

#Disable Warning IDE1006 ' Naming Styles


	Private Function zzGetRange(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As Excel.Range
#Enable Warning IDE1006 ' Naming Styles
		Dim oRange As Excel.Range = Nothing
		Dim sCell As String = zzGetCellAddress(iRowIndex, iColIndex)
		'DMCommon.Debug.MsgBox("120126_5a", iRowIndex, iColIndex, sCell)
		If moExcelApp IsNot Nothing AndAlso moWorksheet IsNot Nothing Then
			Try
				oRange = moWorksheet.Range(sCell)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "moWorksheet Is Nothing: " & CStr(moWorksheet Is Nothing) & vbCrLf & sCell & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColIndex) & " _ " & sCell, "ExcelLog - GetRange_4")
			End Try
		End If
		If oRange Is Nothing Then
			System.Windows.Forms.MessageBox.Show("oRange Is Nothing!" & vbCrLf & "moWorksheet Is Nothing: " & CStr(moWorksheet Is Nothing) & vbCrLf & sCell & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColIndex) & " _ " & sCell, "ExcelLog - GetRange_5")
		End If
		DMCommon.Debug.ExcelLog.SetNextValue(10, "!GetRange", iRowIndex, iColIndex, sCell, oRange Is Nothing)
		Return oRange
	End Function
#Disable Warning IDE1006 ' Naming Styles
	Private Shared Function zzGetCellAddress(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As String
#Enable Warning IDE1006 ' Naming Styles
		If iColIndex <= 25 Then
			Return Chr(65 + iColIndex) & CStr(iRowIndex + 1)
		Else
			iColIndex = iColIndex - 26
			Return Chr(65 + iColIndex \ 26) & Chr(65 + iColIndex Mod 26) & CStr(iRowIndex + 1)
		End If
	End Function
	Public Property CurrentRow As Integer
		Get
			Return miCurrentRow
		End Get
		Set(iValue As Integer)
			miCurrentRow = iValue
		End Set
	End Property
	Public Sub SetFormatValue(iRow As Integer, iCol As Integer, Optional oValue As System.Object = Nothing, Optional sFormat As String = "")
		Dim oRange As Range = zzGetRange(iRow, iCol)
		If oValue IsNot Nothing Then
			oRange.Value = oValue
		End If

		If sFormat.Length <> 0 Then
			oRange.NumberFormat = sFormat
		End If

	End Sub
#Disable Warning IDE1006 ' Naming Styles

	Public Shared Function GetNumFormat(iFormatNumDigits As Integer, iGroupDigits As Microsoft.VisualBasic.TriState) As String

		Dim sGroupSymbol As String
		Dim sFormat As String = Strings.FormatNumber(9990, iFormatNumDigits, TriState.True, , iGroupDigits)
		If InStr(sFormat, ",") = 0 Then
			sGroupSymbol = String.Empty
		Else
			sGroupSymbol = "#"
		End If
		sFormat = Replace(sFormat, "9", sGroupSymbol)
		Return sFormat

	End Function
	Public Sub SetNumberFormat(ByVal tRectangle As System.Drawing.Rectangle, iFormatNumDigits As Integer, iGroupDigits As Microsoft.VisualBasic.TriState)
		Dim oRange As Excel.Range = GetRange(tRectangle)
		Dim sNumberFormat As String = GetNumFormat(iFormatNumDigits, iGroupDigits)
		oRange.NumberFormat = sNumberFormat
		'oRange.Interior.Color = &HAAFFFF
	End Sub
	Public Sub SetBackColor(ByVal tRectangle As System.Drawing.Rectangle, tBackColor As System.Drawing.Color)
		Dim oRange As Excel.Range = GetRange(tRectangle)

		oRange.Interior.Color = tBackColor
	End Sub
	Public Sub SetBorders(ByVal tRectangle As System.Drawing.Rectangle, Optional iLeft As enBorderWeight = enBorderWeight.Missing, Optional iTop As enBorderWeight = enBorderWeight.Missing, Optional iBottom As enBorderWeight = enBorderWeight.Missing, Optional iRight As enBorderWeight = enBorderWeight.Missing)
		Dim oRange As Excel.Range = GetRange(tRectangle)
		If oRange IsNot Nothing Then


			If iLeft <> enBorderWeight.Missing Then
				With oRange.Borders.Item(XlBordersIndex.xlEdgeLeft)

					.Weight = zzToExcelBorderWeight(iLeft)
					.LineStyle = XlLineStyle.xlContinuous
				End With
			End If

			If iTop <> enBorderWeight.Missing Then
				With oRange.Borders.Item(XlBordersIndex.xlEdgeTop)

					.Weight = zzToExcelBorderWeight(iTop)
					.LineStyle = XlLineStyle.xlContinuous
				End With
			End If

			If iBottom <> enBorderWeight.Missing Then
				With oRange.Borders.Item(XlBordersIndex.xlEdgeBottom)

					.Weight = zzToExcelBorderWeight(iBottom)
					.LineStyle = XlLineStyle.xlContinuous
				End With
			End If

			If iRight <> enBorderWeight.Missing Then
				With oRange.Borders.Item(XlBordersIndex.xlEdgeRight)

					.Weight = zzToExcelBorderWeight(iRight)
					.LineStyle = XlLineStyle.xlContinuous
				End With
			End If

		End If


	End Sub
	Private Function zzToExcelBorderWeight(iBorderWeight As enBorderWeight) As XlBorderWeight
		Select Case iBorderWeight
			Case enBorderWeight.Hairline
				Return XlBorderWeight.xlHairline
			Case enBorderWeight.Medium
				Return XlBorderWeight.xlMedium
			Case enBorderWeight.Thick
				Return XlBorderWeight.xlThick
			Case enBorderWeight.Thin
				Return XlBorderWeight.xlThin
		End Select
	End Function

	Private Sub zzFormatHeaderCell(oRange As Excel.Range)
		Const miBackColor As Integer = 12632256
		Const miLinestyle As Integer = 1
		oRange.Font.Bold = True
		oRange.Interior.Color = miBackColor
		oRange.WrapText = True
		oRange.BorderAround(miLinestyle, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic)
	End Sub
	Private Sub zzFormatHeaderCell(oRange As Excel.Range, Optional bLeftBorder As Boolean = False)
		Const iBackColor As Integer = 12632256

		If oRange IsNot Nothing Then
			oRange.HorizontalAlignment = XlHAlign.xlHAlignCenter
			oRange.VerticalAlignment = XlVAlign.xlVAlignCenter
			oRange.Font.Bold = True
			oRange.Interior.Color = iBackColor
			oRange.WrapText = True
			'	oRange.Borders.Creator
			If bLeftBorder Then
				With oRange.Borders.Item(XlBordersIndex.xlEdgeLeft)

					.Weight = XlBorderWeight.xlMedium
					.LineStyle = XlLineStyle.xlContinuous
				End With
			End If
			With oRange.Borders.Item(XlBordersIndex.xlEdgeRight)
				'oAllDataRange.Borders.Item(XlBordersIndex.xlInsideVertical)
				.Weight = XlBorderWeight.xlMedium
				.LineStyle = XlLineStyle.xlContinuous
			End With
			With oRange.Borders.Item(XlBordersIndex.xlEdgeTop)
				.Weight = XlBorderWeight.xlMedium
				.LineStyle = XlLineStyle.xlContinuous
			End With
			With oRange.Borders.Item(XlBordersIndex.xlEdgeBottom)

				.Weight = XlBorderWeight.xlMedium
				.LineStyle = XlLineStyle.xlContinuous
			End With
		End If
	End Sub
	Public Sub SetValueInHeaderCell(ByVal tRectangle As System.Drawing.Rectangle, bLeftBorder As Boolean, oValue As System.Object)

		Dim oRange As Range
		If moWorksheet IsNot Nothing Then

			Try
				oRange = Merge(tRectangle, True)

			Catch oEx As Exception
				oRange = Nothing
			End Try

			If oRange IsNot Nothing AndAlso oValue IsNot Nothing Then
				oRange.Value = oValue
				zzFormatHeaderCell(oRange, bLeftBorder)

			End If
		End If

	End Sub
	Public Sub SetStringInHeaderRow(iRow As Integer, iCol As Integer, bLeftBorder As Boolean, ParamArray saValue() As System.String)
		Dim oRange As Range
		Dim sValue As System.String
		Dim bRangeLeftBorder As Boolean
		If moWorksheet IsNot Nothing Then


			For iValIndex As Integer = 0 To saValue.GetUpperBound(0)
				Try
					oRange = zzGetRange(iRow, iCol + iValIndex)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("120126_4b", iRow, iCol + iValIndex, oEx.Message)
					oRange = Nothing
				End Try


				sValue = saValue(iValIndex)
				If oRange IsNot Nothing Then
					DMCommon.Debug.MsgBox("120126_4c", oRange.Row, oRange.Column)
					If iValIndex = 0 AndAlso bLeftBorder Then
						bRangeLeftBorder = True
					Else
						bRangeLeftBorder = False
					End If
					zzFormatHeaderCell(oRange, bRangeLeftBorder)

					'DMCommon.Debug.MsgBox("120126_4c", oRange.Row, oRange.Column)
					'	oRange.Borders.Creator
					If sValue IsNot Nothing Then

						oRange.Value = sValue

					End If


				Else

				End If
			Next
		End If


	End Sub
	Public Sub SetValueInHeaderRow(iRow As Integer, iCol As Integer, bLeftBorder As Boolean, ParamArray oaValue() As System.Object)
		'Const iBackColor As Integer = 12632256
		Return
		Dim oRange As Range
		Dim oValue As System.Object
		Dim bRangeLeftBorder As Boolean
		If moWorksheet IsNot Nothing Then
			DMCommon.Debug.MsgBox("120126_4a", iRow, iCol, oaValue.GetUpperBound(0))

			For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)
				Try
					oRange = zzGetRange(iRow, iCol + iValIndex)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("120126_4b", iRow, iCol + iValIndex, oEx.Message)
					oRange = Nothing
				End Try


				oValue = oaValue(iValIndex)
				If oRange IsNot Nothing Then
					DMCommon.Debug.MsgBox("120126_4c", oRange.Row, oRange.Column)
					If iValIndex = 0 AndAlso bLeftBorder Then
						bRangeLeftBorder = True
					Else
						bRangeLeftBorder = False
					End If
					zzFormatHeaderCell(oRange, bRangeLeftBorder)

					DMCommon.Debug.MsgBox("120126_4c", oRange.Row, oRange.Column)
					'	oRange.Borders.Creator
					If oValue IsNot Nothing Then
						DMCommon.Debug.MsgBox("120126_4b", oaValue(iValIndex).GetType().ToString(), oaValue(iValIndex).ToString())
						Select Case oaValue(iValIndex).GetType().ToString()
							Case "System.String", "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.Double", "System.Date", "System.DateTime"
								oRange.Value = oaValue(iValIndex)
							Case Else
								oRange.Value = oaValue(iValIndex).ToString()
						End Select
					End If



				End If
			Next
		End If


	End Sub

	Public Sub SetValueInHeaderRow(iHeaderRowMin As Integer, iHeaderRowMax As Integer, iColumnMin As Integer, iColumnMax As Integer, bLeftBorder As Boolean, ByVal dlProcedure As HeaderValue)
		'Const iBackColor As Integer = 12632256

		Dim oRange As Range
		Dim sValue As String
		Dim bRangeLeftBorder As Boolean
		If moWorksheet IsNot Nothing Then

			For iRow As Integer = iHeaderRowMin To iHeaderRowMax
				For iCol As Integer = iColumnMin To iColumnMax
					Try
						oRange = zzGetRange(iRow, iCol)
					Catch oEx As Exception
						oRange = Nothing
					End Try


					sValue = dlProcedure(iRow, iCol)
					If oRange IsNot Nothing Then
						If iColumnMin = 0 AndAlso bLeftBorder Then
							bRangeLeftBorder = True
						Else
							bRangeLeftBorder = False
						End If
						zzFormatHeaderCell(oRange, bRangeLeftBorder)
						'	oRange.Borders.Creator
						If sValue IsNot Nothing Then
							oRange.Value = sValue
						End If

					End If
				Next
			Next
		End If


	End Sub

	Public Sub SetFormatColumns(iCol As Integer, ParamArray daValue() As Double)


		Dim oRange As Range
		For iValIndex As Integer = 0 To daValue.GetUpperBound(0)
			Try
				oRange = zzGetRange(0, iCol + iValIndex)
			Catch oEx As Exception
				oRange = Nothing
			End Try

			If oRange IsNot Nothing AndAlso daValue(iValIndex) <> 0.0 Then

				oRange.ColumnWidth = daValue(iValIndex)

			End If
		Next



	End Sub

	Public Sub SetValueInRow(iCol As Integer, oValue As System.Object)
#Enable Warning IDE1006 ' Naming Styles
		If moWorksheet IsNot Nothing Then
			Dim oRange As Range

			Try
				oRange = zzGetRange(miCurrentRow, iCol)
			Catch oEx As Exception
				oRange = Nothing
			End Try

			If oRange IsNot Nothing AndAlso oValue IsNot Nothing Then
				Select Case oValue.GetType().ToString()
					Case "System.String", "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.Double", "System.Date", "System.DateTime"
						oRange.Value = oValue
					Case Else
						oRange.Value = oValue.ToString()
				End Select

			End If

		End If


	End Sub


	Public Sub SetValueInRow(iRow As Integer, iCol As Integer, ParamArray oaValue() As System.Object)
#Enable Warning IDE1006 ' Naming Styles
		If moWorksheet IsNot Nothing Then
			Dim oRange As Range
			For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)
				Try
					oRange = zzGetRange(iRow, iCol + iValIndex)
				Catch oEx As Exception
					oRange = Nothing
				End Try

				If oRange IsNot Nothing AndAlso oaValue(iValIndex) IsNot Nothing Then
					Select Case oaValue(iValIndex).GetType().ToString()
						Case "System.String", "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.UInt64", "System.Double", "System.Date", "System.DateTime"
							oRange.Value = oaValue(iValIndex)
						Case Else

							oRange.Value = oaValue(iValIndex).ToString()
					End Select

				End If
			Next
		End If


	End Sub
	Public Sub SetFormulaInRow(iRow As Integer, iCol As Integer, bBold As Boolean, ParamArray oaValue() As System.Object)

		If moWorksheet IsNot Nothing Then
			Dim oRange As Range
			Dim oValue As System.Object
			For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)
				Try
					oRange = zzGetRange(iRow, iCol + iValIndex)
				Catch oEx As Exception
					oRange = Nothing
				End Try
				If bBold Then
					oRange.Font.Bold = True
				End If


				oValue = oaValue(iValIndex)
				If oRange IsNot Nothing AndAlso oValue IsNot Nothing Then

					oRange.Value = oValue


				End If
			Next
		End If


	End Sub
	Public Sub SetBorders(iStartRow As Integer, iStartColumn As Integer, iRowNumber As Integer, iColumnNumber As Integer)
		Dim oRange As Range
		For iRow As Integer = iStartRow To iStartRow + iRowNumber - 1
			For iColumn As Integer = iStartColumn To iStartColumn + iColumnNumber - 1
				Try
					oRange = zzGetRange(iRow, iColumn)
					If iColumn = iStartColumn Then
						With oRange.Borders.Item(XlBordersIndex.xlEdgeLeft)
							.Weight = XlBorderWeight.xlThin
							.LineStyle = XlLineStyle.xlContinuous
						End With
					End If
					With oRange.Borders.Item(XlBordersIndex.xlEdgeRight)
						.Weight = XlBorderWeight.xlThin
						.LineStyle = XlLineStyle.xlContinuous
					End With
					With oRange.Borders.Item(XlBordersIndex.xlEdgeBottom)

						.Weight = XlBorderWeight.xlThin
						.LineStyle = XlLineStyle.xlContinuous
					End With


				Catch oEx As Exception
					oRange = Nothing
				End Try
			Next
		Next

	End Sub


	Public Sub SetValueInRowByCol(iRow As Integer, iRowNumber As Integer, iCol As Integer, ParamArray oaValue() As System.Object)
#Enable Warning IDE1006 ' Naming Styles
		If moWorksheet IsNot Nothing Then
			'	DMCommon.Debug.MsgBox("190604_1", iRow, iRowNumber)
			If iRowNumber > 1 Then
				Dim oRange As Range
				Dim tRect As System.Drawing.Rectangle
				For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)
					Try
						tRect = New System.Drawing.Rectangle(iCol + iValIndex, iRow, 0, iRowNumber - 1)
						oRange = Merge(tRect, True)
						oRange.Value = oaValue(iValIndex)
					Catch oEx As Exception
						oRange = Nothing
					End Try



				Next
			Else
				SetValueInRow(iRow, iCol, oaValue)
			End If
		End If



	End Sub
	Public Sub SetValueInCol(iRow As Integer, iCol As Integer, ParamArray oaValue() As System.Object)
#Enable Warning IDE1006 ' Naming Styles
		If Debug.Debug AndAlso moWorksheet IsNot Nothing Then
			Dim oRange As Range
			For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)
				Try
					oRange = zzGetRange(iRow + iValIndex, iCol)
				Catch oEx As Exception
					oRange = Nothing
				End Try



				If oRange IsNot Nothing AndAlso oaValue(iValIndex) IsNot Nothing Then
					Select Case oaValue(iValIndex).GetType().ToString()
						Case "System.String", "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.Double", "System.Date", "System.DateTime"
							oRange.Value = oaValue(iValIndex)
						Case Else

							oRange.Value = oaValue(iValIndex).ToString()
					End Select

				End If
			Next
		End If


	End Sub

	Public Sub SetValue(iFirstColumn As Integer, sCaption As String, ParamArray oaValue() As System.Object)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			SetValueInRow(miCurrentRow, iFirstColumn, oaValue)
		Else
			DMCommon.Debug.MsgBox("190530_3", "moWorksheet Is Nothing", moWorksheet IsNot Nothing)
		End If


	End Sub
	Public Sub SetSheetName(sBaseName As String)
		Dim dicNames As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)





		Dim sResName As String = sBaseName

		Try
			For Each oSheet As Worksheet In moWorkbook.Sheets
				dicNames.Add(oSheet.Name, 0)
			Next
			Dim iIndex As Integer = 0
			Do
				If dicNames.ContainsKey(sResName) Then
					iIndex += 1
					sResName = sBaseName & "_" & Chr(223 + iIndex)
				Else
					Exit Do
				End If
			Loop
			moWorksheet.Name = sResName
			moWorksheet.DisplayRightToLeft = True
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ExcelAppExt - SetSheetName")
		End Try

	End Sub
	Public Function FromDataGrid(ByVal oDataGridView As DataGridView, oGridLayout As GridLayout, Optional iMergeCol As Integer = -1, Optional iNotEmptyCol As Integer = -1) As Boolean
		Dim iaRows(40000) As Integer
		Dim iRowIndErr As Integer
		If moWorkbook IsNot Nothing Then
			Try
				Dim oDataGridViewColumn As DataGridViewColumn
				Dim oRange As Range
				Dim oDataGridViewRow As System.Windows.Forms.DataGridViewRow
				Dim oDataGridViewCell As System.Windows.Forms.DataGridViewCell
				Dim iSheetColIndex As Integer = 0
				Dim iColVisibleUB As Integer = 0
				Dim iMidColIndex As Integer

				Dim dFormWidth As Double
				Dim dColWidthSum As Double

				Dim oCellStyle As DataGridViewCellStyle
				Dim sNumberFormat As String
				Dim sFieldName As String
				Dim oColumnType As Type
				Dim tGridColDef As GridColDef = Nothing
				Dim iSheetRow As Integer
				Dim iBorderIndex As XlBordersIndex = CType(9, XlBordersIndex)
				moWorksheet = DirectCast(moWorkbook.Sheets.Add(), Excel.Worksheet)
				Dim oPageSetup As PageSetup = moWorksheet.PageSetup
				oPageSetup.LeftMargin = 18
				oPageSetup.RightMargin = 18
				oPageSetup.BottomMargin = 18
				oPageSetup.TopMargin = 18
				Dim oMergeRange As Range
				'Dim oApostRange(99) As Range

				Dim iaNotEmptyRows(oDataGridView.RowCount - 1) As Integer
				Dim oRow As Range
				Dim iNotEmptyIndex As Integer = -1
				Dim iRowInd As Integer
				Dim oValue As System.Object
				Dim iOutRowUB As Integer
				Dim iOutRowIndex As Integer
				Dim oaTest(4) As System.Object
				'	DMCommon.Debug.MsgBox("091224_3", iNotEmptyCol, moWorksheet Is Nothing, moWorksheet.Columns.Count)
				If iNotEmptyCol <> -1 Then
					For iRowIndex As Integer = 0 To oDataGridView.Rows.Count - 1
						oDataGridViewRow = oDataGridView.Rows.Item(iRowIndex)
						oDataGridViewCell = oDataGridViewRow.Cells.Item(iNotEmptyCol)

						oValue = oDataGridViewCell.Value

						If oValue IsNot Nothing AndAlso Not IsDBNull(oValue) Then
							iNotEmptyIndex += 1
							iaNotEmptyRows(iNotEmptyIndex) = iRowIndex
						End If
					Next
					iOutRowUB = iNotEmptyIndex
				Else
					iOutRowUB = oDataGridView.Rows.Count - 1
				End If
				'	DMCommon.Debug.MsgBox("091224_4", oDataGridView.Columns.Count, moWorksheet.Columns.Count)
				For iColIndex As Integer = 0 To oDataGridView.Columns.Count - 1
					oDataGridViewColumn = oDataGridView.Columns.Item(iColIndex)
					If oDataGridViewColumn.Visible Then

						oRange = GetColumn(iSheetColIndex + 1)
						If oRange IsNot Nothing Then
							oRange.VerticalAlignment = XlVAlign.xlVAlignTop
						End If


						iSheetColIndex += 1
					End If
				Next
				iSheetColIndex = 0
				'	DMCommon.Debug.MsgBox("091224_5", oDataGridView.Columns.Count)
				For iColIndex As Integer = 0 To oDataGridView.Columns.Count - 1
					oMergeRange = Nothing
					oDataGridViewColumn = oDataGridView.Columns.Item(iColIndex)
					If oDataGridViewColumn.Visible Then  'System.Windows.Forms.DataGridViewCheckBoxColumn
						sFieldName = oDataGridViewColumn.DataPropertyName
						oCellStyle = oDataGridViewColumn.DefaultCellStyle
						oColumnType = oDataGridViewColumn.GetType()
						oRange = GetRange(miDataFirstRow, iSheetColIndex)
						With oRange
							.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter
							.VerticalAlignment = XlVAlign.xlVAlignCenter
							.WrapText = True
							.Font.Bold = True
							.Interior.Color = miBackColor
							.BorderAround(miLinestyle, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic)
							If oGridLayout IsNot Nothing AndAlso oGridLayout.TryGetValue(sFieldName, tGridColDef) Then
								.ColumnWidth = tGridColDef.ExcelColumnWidth
								If tGridColDef.ExcelColumnWidth <> 0 Then
									iColVisibleUB += 1
								End If
							Else
								.ColumnWidth = oDataGridViewColumn.Width * mdK1
								iColVisibleUB += 1
								dFormWidth += CDbl(.ColumnWidth)
							End If
							.Value = oDataGridViewColumn.HeaderText
							sNumberFormat = zzGetNumberFormat(oCellStyle.Format)
						End With

						For iCurrentIndex As Integer = 0 To iOutRowUB
							If iNotEmptyCol <> -1 Then
								iOutRowIndex = iaNotEmptyRows(iCurrentIndex)
							Else
								iOutRowIndex = iCurrentIndex
							End If

							iSheetRow = iCurrentIndex + 1 + miDataFirstRow
							oRange = GetRange(iSheetRow, iSheetColIndex)
							oDataGridViewRow = oDataGridView.Rows.Item(iOutRowIndex)
							oDataGridViewCell = oDataGridViewRow.Cells.Item(iColIndex)
							If CellIsVisible(oDataGridViewCell.Tag) Then
								If oMergeRange IsNot Nothing Then
									oMergeRange.Merge()
									oMergeRange.Font.Bold = True
									oMergeRange.VerticalAlignment = XlVAlign.xlVAlignTop
									iRowIndErr = iRowInd
									iaRows(iRowInd) = iSheetRow - 1

									iRowInd += 1


									'oRow.Borders.Item(9).Weight = 4
								End If
								If oColumnType Is GetType(DataGridViewCheckBoxColumn) Then
									oRange.Value = zzDispCheckColumnValue(oDataGridViewCell.Value)
								Else
									oRange.Value = oDataGridViewCell.Value
									'	Dim o As system.Object = oRange.Orientation
								End If

								oRange.WrapText = True
								If iColIndex = iMergeCol Then
									oMergeRange = oRange
								End If

							ElseIf iColIndex = iMergeCol AndAlso oMergeRange IsNot Nothing Then
								oMergeRange = GetRangeSum(oMergeRange, oRange)
							End If

							oRange.BorderAround(miLinestyle, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic)
							If sNumberFormat IsNot Nothing Then
								oRange.NumberFormat = sNumberFormat
							End If
						Next
						If iColIndex = iMergeCol AndAlso oMergeRange IsNot Nothing Then
							oMergeRange.Merge()
							oMergeRange.Font.Bold = True
							oMergeRange.VerticalAlignment = XlVAlign.xlVAlignTop
							iaRows(iRowInd) = iSheetRow
						End If
						iSheetColIndex += 1
						'	oRange.Borders.Item( Microsoft.Office.Interop.Excel.XlBordersIndex)) = 1
					End If

				Next iColIndex
				If dFormWidth > 0.0 Then
					For iColIndex As Integer = 0 To oDataGridView.Columns.Count - 1
						oDataGridViewColumn = oDataGridView.Columns.Item(iColIndex)
						If oDataGridViewColumn.Visible Then
							If dColWidthSum + CDbl(oDataGridViewColumn.Width) > dFormWidth * 0.5 Then
								iMidColIndex = iColIndex
								Exit For
							End If
						End If

					Next

				End If
				Dim iDividerHeight As Integer
				For iRowIndex As Integer = 0 To iRowInd
					oDataGridViewRow = oDataGridView.Rows.Item(iRowIndex)
					iDividerHeight = oDataGridViewRow.DividerHeight
					If iDividerHeight = 2 Then
						oRow = GetRange(iaRows(iRowIndex), 0, iaRows(iRowIndex), iSheetColIndex - 1)
						oRow.Borders.Item(iBorderIndex).Weight = XlBorderWeight.xlThick
					End If

					'	oRow.Interior.Color = Color.Green
				Next
				zzSetPrintTitleRows()
				zzSetPageOrientation()
				zzInsertCaptions(iColVisibleUB, iMidColIndex)

				If moWindow IsNot Nothing Then
					Try
						moWindow.Activate()
					Catch oEx As Exception

					End Try
				End If

				Return True
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & CStr(iaRows.GetUpperBound(0)) & ":" & CStr(iRowIndErr), "Application - OpenFile")
				Return False
			End Try
		End If
	End Function
	<CLSCompliant(False)>
	Public Shared Function GetRangeSum(ByVal oRangA As Excel.Range, ByVal oRangB As Excel.Range) As Excel.Range
		Dim oRange As Excel.Range = Nothing
		Dim iRowIndexMin As Integer = Math.Min(oRangA.Row, oRangB.Row) - 1
		Dim iRowIndexMax As Integer = Math.Max(oRangA.Row, oRangB.Row) - 1
		Dim iColIndexMin As Integer = Math.Min(oRangA.Column, oRangB.Column) - 1
		Dim iColIndexMax As Integer = Math.Max(oRangA.Column, oRangB.Column) - 1

		Dim sCellMin As String = zzGetCellAddress(iRowIndexMin, iColIndexMin)
		Dim sCellMax As String = zzGetCellAddress(iRowIndexMax, iColIndexMax)
		Try
			oRange = moWorksheet.Range(sCellMin, sCellMax)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iRowIndexMin) & "," & CStr(iColIndexMin) & ":" & CStr(iColIndexMin) & "," & CStr(iColIndexMax), "Application - GetRange_2")
		End Try
		Return oRange
	End Function
	Private Sub zzSetNextValueInRow(iCol As Integer, ParamArray oaValue() As System.Object)
		If Debug.Debug AndAlso moWorksheet IsNot Nothing Then

			SetValueInRow(miCurrentRow, iCol, oaValue)
			miCurrentRow += 1
		End If
	End Sub


	Public Sub SetNextValue(iFirstColumn As Integer, sCaption As String, ParamArray oaValue() As System.Object)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			SetValueInRow(miCurrentRow, iFirstColumn, oaValue)
			miCurrentRow += 1
		End If
	End Sub
	Public Sub SetNextValue(iFirstColumn As Integer, sCaption As String, saValue() As Long)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			For iIndex As Integer = 0 To saValue.GetUpperBound(0)
				SetValueInRow(miCurrentRow, iFirstColumn + iIndex, saValue(iIndex))
			Next

			miCurrentRow += 1
		End If
	End Sub
	Public Sub SetNextValue(iFirstColumn As Integer, sCaption As String, saValue() As String)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			For iIndex As Integer = 0 To saValue.GetUpperBound(0)
				SetValueInRow(miCurrentRow, iFirstColumn + iIndex, saValue(iIndex))
			Next

			miCurrentRow += 1
		End If
	End Sub

	Public Sub SetNextValue(iFirstColumn As Integer, sCaption As String, iaValue() As Integer)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			For iIndex As Integer = 0 To iaValue.GetUpperBound(0)
				SetValueInRow(miCurrentRow, iFirstColumn + iIndex, iaValue(iIndex))
			Next

			miCurrentRow += 1
		End If
	End Sub



	Public Sub SetArray(iFirstColumn As Integer, sCaption As String, bNextRow As Boolean, oArray As System.Array)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			For iIndex As Integer = 0 To oArray.GetUpperBound(0)
				'oArrayItem.GetType()
				'	DirectCast(oArray.GetValue(iIndex), oArrayItemType.tostring)
				SetValueInRow(miCurrentRow, iFirstColumn + iIndex, oArray.GetValue(iIndex))
			Next
			If bNextRow Then
				miCurrentRow += 1
			End If

		End If


	End Sub
	Public Sub SetValueByCond(iFirstColumn As Integer, sCaption As String, bNextRow As Boolean, bCondition As Boolean, ParamArray oaValue() As System.Object)
		If moWorksheet IsNot Nothing Then
			zzSetCaption(iFirstColumn, sCaption)
			If bCondition Then
				SetValueInRow(miCurrentRow, iFirstColumn, oaValue)
			Else
				SetValueInRow(miCurrentRow, iFirstColumn, "Cond Is False")
			End If

			If bNextRow Then
				miCurrentRow += 1
			End If

		End If


	End Sub


	Public Sub SetEnumerable(iFirstColumn As Integer, sCaption As String, oEnumerable As IEnumerable)
		If moWorksheet IsNot Nothing AndAlso oEnumerable IsNot Nothing Then
			Dim iIndex As Integer = 0
			zzSetCaption(iFirstColumn, sCaption)
			For Each oValue As System.Object In oEnumerable
				SetValueInRow(miCurrentRow, iFirstColumn + iIndex, zzToString(oValue))
				iIndex += 1
			Next
			miCurrentRow += 1
		End If

	End Sub
	Public Function Merge(ByVal tRectangle As System.Drawing.Rectangle, bCenter As Boolean, Optional oValue As System.Object = Nothing, Optional bBold As Boolean = False) As Excel.Range
		Dim oExcelObject As System.Object
		Dim oRange As Excel.Range = Nothing
		Dim sTopLeftCell As String = zzGetCellAddress(tRectangle.Top, tRectangle.Left)
		Dim sBottomRightCell As String = zzGetCellAddress(tRectangle.Bottom, tRectangle.Right)
		Dim sTest As String = sTopLeftCell & ":" & sBottomRightCell
		Dim sTag As String = "a"
		Try
			'oExcelObject = moWorksheet.Range(sTopLeftCell, sBottomRightCell)
			oExcelObject = moWorksheet.Range(sTest)

		Catch oEx As Exception
			oExcelObject = Nothing
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Application - Merge_1")
		End Try

		If oExcelObject IsNot Nothing Then
			Try
				oRange = CType(oExcelObject, Excel.Range)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Application - Merge_2")
			End Try


			Try
				'oRange.Select()
				sTag = "b"
				oRange.Merge()
				If bCenter Then
					oRange.HorizontalAlignment = XlHAlign.xlHAlignCenter
					oRange.VerticalAlignment = XlVAlign.xlVAlignCenter
				End If
				If oValue IsNot Nothing Then
					oRange.Value = oValue
				End If
				If bBold Then
					oRange.Font.Bold = True
				End If
			Catch oEx As Exception
				DMCommon.Debug.UserMsg("Application - Merge_3", oEx.Message, oEx.GetType(), sTag, sTest, sTopLeftCell, sBottomRightCell, oRange.Count, oRange.Column, oRange.Columns.Count, oRange.Row, oRange.Rows.Count)
				Try

				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - Merge_4")
				End Try


			End Try


		Else
			System.Windows.Forms.MessageBox.Show("ExcelObject Is  Nothing", "Application - Merge_5")
		End If
		Return oRange
	End Function
	Public Function GetRectangleAddress(ByVal tRectangle As System.Drawing.Rectangle) As String
		Dim sTopLeftCell As String = zzGetCellAddress(tRectangle.Top, tRectangle.Left)
		Dim sBottomRightCell As String = zzGetCellAddress(tRectangle.Bottom, tRectangle.Right)
		Return sTopLeftCell & ":" & sBottomRightCell

	End Function
	Public Function GetRectangleAddress(ByVal iFirstRow As Integer, ByVal iFirstColumn As Integer, ByVal iRowNumber As Integer, ByVal iColumnNumber As Integer) As String
		Dim sTopLeftCell As String = zzGetCellAddress(iFirstRow, iFirstColumn)
		Dim sBottomRightCell As String = zzGetCellAddress(iFirstRow + iRowNumber - 1, iFirstColumn + iColumnNumber - 1)
		Return sTopLeftCell & ":" & sBottomRightCell

	End Function
	Public Function GetCellAddress(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As String
		Return zzGetCellAddress(iRowIndex, iColIndex)
	End Function
	Public Sub PaintRange(ByVal iRowIndexMin As Integer, ByVal iColIndexMin As Integer, ByVal iRowIndexMax As Integer, ByVal iColIndexMax As Integer, tColor As System.Drawing.Color)

		Dim oRange As Excel.Range = GetRange(iRowIndexMin, iColIndexMin, iRowIndexMax, iColIndexMax)
		'DMCommon.Debug.MsgBox("091224_12", iRowIndexMin, iColIndexMin, iRowIndexMax, iColIndexMax, oRange Is Nothing)
		oRange.Interior.Color = tColor
	End Sub


	<CLSCompliant(False)>
	Public Shared Function GetRange(ByVal iRowIndexMin As Integer, ByVal iColIndexMin As Integer, ByVal iRowIndexMax As Integer, ByVal iColIndexMax As Integer) As Excel.Range
		Dim oRange As Excel.Range = Nothing
		Dim sCellMin As String = zzGetCellAddress(iRowIndexMin, iColIndexMin)
		Dim sCellMax As String = zzGetCellAddress(iRowIndexMax, iColIndexMax)
		Try
			oRange = moWorksheet.Range(sCellMin, sCellMax)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iRowIndexMin) & "," & CStr(iColIndexMin) & ":" & CStr(iColIndexMin) & "," & CStr(iColIndexMax), "Application - GetRange_12")
		End Try
		Return oRange
	End Function
	Public Function GetRange(ByVal tRectangle As System.Drawing.Rectangle) As Excel.Range
		Dim oExcelObject As System.Object
		Dim oRange As Excel.Range = Nothing
		Dim sTopLeftCell As String = zzGetCellAddress(tRectangle.Top, tRectangle.Left)
		Dim sBottomRightCell As String = zzGetCellAddress(tRectangle.Bottom, tRectangle.Right)
		Dim sRectangleAddress As String = sTopLeftCell & ":" & sBottomRightCell
		If moWorksheet IsNot Nothing Then
			Try
				oExcelObject = moWorksheet.Range(sRectangleAddress)

			Catch oEx As Exception
				oExcelObject = Nothing
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Application - GetRange_5")
			End Try

			If oExcelObject IsNot Nothing Then
				Try
					oRange = CType(oExcelObject, Excel.Range)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Application - GetRange_6")
				End Try

			End If
		End If

		Return oRange
	End Function
	Public Function GetColumn(ByVal iColumnIndex As Integer) As Excel.Range
		'Dim tRectangle As System.Drawing.Rectangle
		Dim oExcelObject As System.Object
		Dim oRange As Excel.Range = Nothing
		'Dim sTopLeftCell As String = zzGetCellAddress(tRectangle.Top, tRectangle.Left)
		'Dim sBottomRightCell As String = zzGetCellAddress(tRectangle.Bottom, tRectangle.Right)
		'Dim sRectangleAddress As String = sTopLeftCell & ":" & sBottomRightCell
		If moWorksheet IsNot Nothing Then
			Try
				oExcelObject = moWorksheet.Columns.Item(iColumnIndex)
			Catch oEx As Exception
				oExcelObject = Nothing
				DMCommon.Debug.MsgBox("!GetColumn_1", oEx.Message, moWorksheet.Columns.Count, iColumnIndex)
			End Try

			If oExcelObject IsNot Nothing Then
				Try
					oRange = CType(oExcelObject, Excel.Range)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Application - GetColumn_2")
				End Try
			End If
		End If
		Return oRange
	End Function
	Public Function GetValue(ByVal iRow As Integer, ByVal iColumn As Integer) As System.Object
		Dim oRes As System.Object = Nothing
		If moWorksheet IsNot Nothing Then
			Dim oRange As Range
			Try
				oRange = zzGetRange(iRow, iColumn)
			Catch oEx As Exception
				oRange = Nothing
			End Try

			If oRange IsNot Nothing Then
				oRes = oRange.Value
			End If

		End If
		Return oRes
	End Function
	Public Function GetRowValues(ByVal iRow As Integer, ByVal iFirstColumn As Integer, ByVal iColumnNumber As Integer) As System.Object()
		Dim oRes(iColumnNumber - 1) As System.Object
		If moWorksheet IsNot Nothing Then
			Dim oRange As Range

			For iColumn As Integer = iFirstColumn To iFirstColumn + iColumnNumber - 1
				Try
					oRange = zzGetRange(iRow, iColumn)
				Catch oEx As Exception
					oRange = Nothing
				End Try



				If oRange IsNot Nothing Then
					oRes(iColumn - iFirstColumn) = oRange.Value


				End If
			Next

		End If
		Return oRes
	End Function

	Public Sub NextRow()

		miCurrentRow += 1


	End Sub
	Public Sub MoveRow(iCount As Integer)
		miCurrentRow += iCount
	End Sub

	Public Sub SetDataRow(iFirstColumn As Integer, sCaption As String, oDataRow As System.Data.DataRow)
		If Debug.Debug Then
			If oDataRow IsNot Nothing Then
				Dim oDataTable As System.Data.DataTable = oDataRow.Table
				Dim iColIndex As Integer = 0
				Dim iColumnsUB As Integer = oDataTable.Columns.Count - 1
				Dim baExcelValue(iColumnsUB) As Boolean
				Dim oValue As System.Object
				Dim iTableRow As Integer = 0
				zzSetCaption(iFirstColumn, sCaption, "Rows=" & oDataTable.Rows.Count.ToString())
				miCurrentRow += 1
				'  System.Windows.Forms.MessageBox.Show(iColumnsUB.ToString(), "09_540")
				For Each oCol As System.Data.DataColumn In oDataTable.Columns
					SetValueInRow(miCurrentRow, iFirstColumn + iColIndex, oCol.ColumnName)
					SetValueInRow(miCurrentRow + 1, iFirstColumn + iColIndex, oCol.DataType.ToString())

					baExcelValue(iColIndex) = zzIsExcelValue(oCol.DataType)
					iColIndex += 1
				Next
				miCurrentRow += 2

				For iColumnIndex As Integer = 0 To iColumnsUB
					oValue = oDataRow.Item(iColumnIndex)
					If Not baExcelValue(iColumnIndex) Then
						oValue = oValue.ToString()
					End If
					SetValueInRow(miCurrentRow, iFirstColumn + iColumnIndex, oValue)
				Next
				SetValueInRow(miCurrentRow, iFirstColumn + iColumnsUB + 1, oDataRow.RowState.ToString)
				miCurrentRow += 1
				iTableRow += 1

			Else
				zzSetCaption(iFirstColumn, sCaption, "Table is Nothing")
			End If

		End If
	End Sub
	Public Sub SetDataTable(iFirstColumn As Integer, sCaption As String, oDataTable As System.Data.DataTable, Optional iMaxRow As Integer = 0)
		If Debug.Debug Then
			If oDataTable IsNot Nothing Then
				Dim iColIndex As Integer = 0
				Dim iColumnsUB As Integer = oDataTable.Columns.Count - 1
				Dim baExcelValue(iColumnsUB) As Boolean
				Dim oValue As System.Object
				Dim iTableRow As Integer = 0
				zzSetCaption(iFirstColumn, sCaption, "Rows=" & oDataTable.Rows.Count.ToString())
				miCurrentRow += 1
				'  System.Windows.Forms.MessageBox.Show(iColumnsUB.ToString(), "09_540")
				For Each oCol As System.Data.DataColumn In oDataTable.Columns
					SetValueInRow(miCurrentRow, iFirstColumn + iColIndex, oCol.ColumnName)
					SetValueInRow(miCurrentRow + 1, iFirstColumn + iColIndex, oCol.DataType.ToString())

					baExcelValue(iColIndex) = zzIsExcelValue(oCol.DataType)
					iColIndex += 1
				Next
				miCurrentRow += 2
				For Each oRow As System.Data.DataRow In oDataTable.Rows
					For iColumnIndex As Integer = 0 To iColumnsUB
						oValue = oRow.Item(iColumnIndex)
						If Not baExcelValue(iColumnIndex) Then
							oValue = oValue.ToString()
						End If
						SetValueInRow(miCurrentRow, iFirstColumn + iColumnIndex, oValue)
					Next
					SetValueInRow(miCurrentRow, iFirstColumn + iColumnsUB + 1, oRow.RowState.ToString)
					miCurrentRow += 1
					iTableRow += 1
					If iMaxRow > 0 AndAlso iMaxRow < iTableRow Then
						Exit For
					End If
				Next
			Else
				zzSetCaption(iFirstColumn, sCaption, "Table is Nothing")
			End If

		End If
	End Sub
	Public Sub SetDataTable(iFirstColumn As Integer, sCaption As String, oDataView As System.Data.DataView, Optional iMaxRow As Integer = 0)
		If Debug.Debug Then
			If oDataView IsNot Nothing Then
				Dim oDataTable As System.Data.DataTable = oDataView.Table
				Dim iColIndex As Integer = 0
				Dim iColumnsUB As Integer = oDataTable.Columns.Count - 1
				Dim baExcelValue(oDataTable.Columns.Count - 1) As Boolean
				Dim oValue As System.Object
				Dim iTableRow As Integer = 0
				zzSetCaption(iFirstColumn, sCaption)
				For Each oCol As System.Data.DataColumn In oDataTable.Columns
					SetValueInRow(miCurrentRow, iFirstColumn + iColIndex, oCol.ColumnName)
					baExcelValue(iColIndex) = zzIsExcelValue(oCol.DataType)
					iColIndex += 1
				Next
				miCurrentRow += 1
				For Each oRow As System.Data.DataRowView In oDataView
					For iColIndex = 0 To iColumnsUB
						Try

							'If miCurrentRow = 1 Then
							'   SetValue(miCurrentRow + 1, iFirstColumn + iColIndex, IsReference(oRow.Item(iColIndex)))
							'End If
							oValue = oRow.Item(iColIndex)
							If Not baExcelValue(iColIndex) Then
								oValue = oValue.ToString()
							End If
							SetValueInRow(miCurrentRow, iFirstColumn + iColIndex, oValue)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & miCurrentRow.ToString() & ":" & (iFirstColumn + iColIndex).ToString() & vbCrLf & oRow.Item(iColIndex).ToString(), "Application - zzInitExcel_24")
						End Try


					Next
					iTableRow += 1
					If iMaxRow > 0 AndAlso iMaxRow < iTableRow Then
						Exit For
					End If

					If miCurrentRow = 1 Then
						'  miCurrentRow += 1
					End If
					miCurrentRow += 1
				Next
			Else
				zzSetCaption(iFirstColumn, sCaption, "DataView is Nothing")
				DMCommon.Debug.MsgBox("ExcelLog", "oDataView Is Nothing")
			End If
			miCurrentRow += 1
		End If
	End Sub
	Private Shared Function zzToIndex(ByVal sLetter As String) As Integer
		Return Asc(sLetter.ToUpper()) - 65
	End Function
#Disable Warning IDE1006 ' Naming Styles
	Private Shared Sub zzInitDebug()
#Enable Warning IDE1006 ' Naming Styles
		If System.Environment.UserName.ToUpper = "BORIS" Then
			mbDebug = mbDebugBoris
		End If
	End Sub
	Private Function zzToString(oValue As System.Object) As String
		If oValue Is Nothing Then
			Return "'Nothing'"
		Else
			Return oValue.ToString()
		End If
	End Function


	Private Sub zzSetCaption(ByRef iFirstColumn As Integer, sCaption As String, Optional sAddInfo As String = Nothing)
		If Not String.IsNullOrEmpty(sCaption) Then
			SetValueInRow(miCurrentRow, iFirstColumn, sCaption, sAddInfo)
			iFirstColumn += 1
		End If
	End Sub

	Private Shared Function zzIsExcelValue(oType As System.Type) As Boolean

		Select Case oType
			Case GetType(System.Int16), GetType(System.Int32), GetType(System.Int64)
				Return True
			Case GetType(System.UInt16), GetType(System.UInt32), GetType(System.UInt64)
				Return True
			Case GetType(System.Double), GetType(System.Decimal)
				Return True
			Case GetType(System.Boolean)
				Return True
			Case GetType(System.DateTime), GetType(System.Boolean), GetType(System.String)

				Return True
			Case Else
				Return False
		End Select

	End Function
	Public Function GetSelectionValue() As System.Object(,)
		Try
			moWindow = moExcelApp.ActiveWindow
		Catch oEx As Exception

		End Try
		DMCommon.Debug.MsgBox("L01", moExcelApp IsNot Nothing, moWindow IsNot Nothing)
		If moWindow IsNot Nothing Then
			Dim oSelectionObject As System.Object
			Dim oSelRange As Range
			Dim oaValues As System.Object(,) = Nothing
			oSelectionObject = moWindow.Selection
			If oSelectionObject IsNot Nothing Then

				oSelRange = TryCast(oSelectionObject, Range)
				If oSelRange IsNot Nothing Then

					oaValues = TryCast(oSelRange.Value, Object(,))

				End If
			End If
			Return oaValues
		Else
			Return Nothing
		End If

	End Function
	Public Sub Reset()
		miCurrentRow = 0
	End Sub
	Public Function Open_231224() As Boolean
		Const sExcelClass As String = "Excel.Application" '= "Excel.Application.10"

		Dim oExcelObject As System.Object
		Dim sTest As String = "Z"


		If IsDead() Then
			Try
				oExcelObject = GetObject(, sExcelClass)
			Catch oEx As Exception
				Try
					sTest = "Y"
					oExcelObject = GetObject(String.Empty, sExcelClass)
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - zzInitExcel_1")
					Return False
				End Try
			End Try
			'   DMCommon.Debug.UserMsg("09_881", oExcelObject)
			sTest = "KL"
			'	DMCommon.Debug.UserMsg("09_882", oExcelObject Is Nothing)
			Try
				moExcelApp = DirectCast(oExcelObject, Excel.Application)
				'	DMCommon.Debug.UserMsg("09_882c", oExcelObject Is Nothing)
				moExcelApp.Visible = True

				'moExcelApp.Application.ActiveWindow.Activate()

				'	System.Windows.Forms.MessageBox.Show(moExcelApp.Name, "Application - zzInitExcel_2")
				sTest = "C"
				moWorkbook = moExcelApp.ActiveWorkbook
				sTest = "D"
				If moWorkbook Is Nothing Then
					sTest = "E"
					'DMCommon.Debug.UserMsg("09_883mid", moExcelApp.Workbooks.Count)
					moWorkbook = moExcelApp.Workbooks.Add()
					sTest = "F"
				End If
				sTest = "G"
				'	DMCommon.Debug.MsgBox("09_883after", moWorkbook.Worksheets.Count)
				oExcelObject = moWorkbook.Worksheets.Add()
				sTest = "H"
				moWorksheet = DirectCast(oExcelObject, Excel.Worksheet)
				sTest = "I"
				'''''''''''''''''''''''''''	moWorksheet.PageSetup.PrintGridlines = True
				sTest = "J"
				moWorksheet.DisplayRightToLeft = False

				sTest = "K"

				Return True
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "Application - zzInitExcel_23")
				Return False
			End Try
		Else
			Return True
		End If
	End Function
	Public Function Open() As Boolean
		Const sExcelClass As String = "Excel.Application" '= "Excel.Application.10"

		Dim oExcelObject As System.Object
		Dim sTest As String = "Z"


		If IsDead() Then
			Try
				oExcelObject = GetObject(, sExcelClass)
			Catch oEx As Exception
				Try
					sTest = "Y"
					oExcelObject = GetObject(String.Empty, sExcelClass)
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - zzInitExcel_1")
					Return False
				End Try
			End Try

			sTest = "KL"


			Try
				moExcelApp = DirectCast(oExcelObject, Excel.Application)
				'	DMCommon.Debug.UserMsg("09_882c", oExcelObject Is Nothing)


				'moExcelApp.Application.ActiveWindow.Activate()

				'	System.Windows.Forms.MessageBox.Show(moExcelApp.Name, "Application - zzInitExcel_2")
				sTest = "C"

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "Application - zzInitExcel_23")
				Return False
			End Try
		End If
		If moExcelApp IsNot Nothing Then


			moExcelApp.Visible = True

			'DMCommon.Debug.UserMsg("!WindowState!!!", moExcelApp.WindowState.ToString())
			'moExcelApp.WindowState
			If moExcelApp.WindowState = XlWindowState.xlMinimized Then
				moExcelApp.WindowState = XlWindowState.xlNormal
			End If
			moWorkbook = moExcelApp.ActiveWorkbook

			sTest = "D"
			If moWorkbook Is Nothing Then
				sTest = "E"

				moWorkbook = moExcelApp.Workbooks.Add()

				sTest = "F"
			End If
			sTest = "G"

		End If
		If moWorkbook IsNot Nothing Then
			sTest = "K"
			oExcelObject = moWorkbook.Worksheets.Add()
			sTest = "H"
			If oExcelObject IsNot Nothing Then
				moWorksheet = DirectCast(oExcelObject, Excel.Worksheet)
				If moWorksheet IsNot Nothing Then
					moWorksheet.DisplayRightToLeft = False
					Return True

				End If

			End If

			sTest = "I"
			'''''''''''''''''''''''''''	moWorksheet.PageSetup.PrintGridlines = True
			sTest = "J"


		End If
		Return False
	End Function
	Public ReadOnly Property WorksheetName() As String
		Get
			If moWorksheet IsNot Nothing Then
				Return moWorksheet.Name
			Else
				Return Nothing
			End If
		End Get
	End Property


	Public Sub SetHeader(sLeft As String, sCenter As String, sRight As String)
		Dim oPageSetup As PageSetup
		oPageSetup = moWorksheet.PageSetup
		If Not String.IsNullOrEmpty(sLeft) Then
			oPageSetup.LeftHeader = sLeft
		End If
		If Not String.IsNullOrEmpty(sCenter) Then
			oPageSetup.CenterHeader = sCenter

		End If
		If Not String.IsNullOrEmpty(sRight) Then
			oPageSetup.RightHeader = sRight
		End If

	End Sub
	Public Sub SetHeaderTest(sLeft As String, sCenter As String, sRight As String)
		Dim oPageSetup As PageSetup
		oPageSetup = moWorksheet.PageSetup
		If Not String.IsNullOrEmpty(sLeft) Then
			moWorksheet.PageSetup.LeftHeader = sLeft
		End If
		If Not String.IsNullOrEmpty(sCenter) Then
			moWorksheet.PageSetup.CenterHeader = sCenter
		End If
		If Not String.IsNullOrEmpty(sRight) Then
			moWorksheet.PageSetup.RightHeader = sRight
		End If

	End Sub
	Public Sub SetFooter(sLeft As String, sCenter As String, sRight As String)
		Dim oPageSetup As PageSetup
		oPageSetup = moWorksheet.PageSetup
		If Not String.IsNullOrEmpty(sLeft) Then
			oPageSetup.LeftFooter = sLeft
		End If
		If Not String.IsNullOrEmpty(sCenter) Then
			oPageSetup.CenterFooter = sCenter
		End If
		If Not String.IsNullOrEmpty(sRight) Then
			oPageSetup.RightFooter = sRight
		End If
	End Sub
	Public Shared Function GetRowAddress(ByVal iRowIndex As Integer) As String
		Dim sRowIndex As String = Convert.ToString(iRowIndex + 1)
		Return sRowIndex & ":" & sRowIndex
	End Function
	Public Sub PrintGridlines(bValue As Boolean)
		Dim oPageSetup As PageSetup
		oPageSetup = moWorksheet.PageSetup
		oPageSetup.PrintGridlines = bValue
	End Sub
	<CLSCompliant(False)>
	Public Shared Function GetRange(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As Excel.Range
		Dim oRange As Excel.Range = Nothing
		Dim sCell As String = zzGetCellAddress(iRowIndex, iColIndex)
		Try
			If moWorksheet IsNot Nothing Then
				oRange = moWorksheet.Range(sCell)
			End If

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sCell & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColIndex), "Application - GetRange_7")
		End Try
		Return oRange
	End Function
	Private Function zzGetNumberFormat(sDataGridFormat As String) As String
		If sDataGridFormat.StartsWith("N") Then
			Dim sNumDigit As String = sDataGridFormat.Substring(1, 1)
			Dim iNumDigit As Integer
			Dim sRes As String

			Try
				iNumDigit = Convert.ToInt32(sNumDigit)
				sRes = Strings.FormatNumber(9990, iNumDigit, TriState.True, , TriState.True)
				sRes = Strings.Replace(sRes, "9", "#")
				Return sRes
			Catch oEx As Exception
				Return Nothing
			End Try


		Else
			Return Nothing
		End If
	End Function
	Private Function CellIsVisible(oTag As System.Object) As Boolean
		If oTag IsNot Nothing Then
			Dim sTag As String = DirectCast(oTag, String)
			Return sTag <> "Hide"
		Else
			Return True
		End If
	End Function
	Private Function zzDispCheckColumnValue(oValue As System.Object) As String
		Dim bValue As Boolean
		Try

			miCurrentRow += 1
			bValue = DirectCast(oValue, Boolean)
			Select Case bValue
				Case True
					Return "כן"
				Case False
					Return "לא"
				Case Else
					Return String.Empty
			End Select
		Catch ex As Exception
			Return String.Empty
		End Try
	End Function
	Private Sub zzSetPrintTitleRows()
		moWorksheet.PageSetup.PrintTitleRows = GetRowAddress(miDataFirstRow)
	End Sub
	Private Sub zzSetPageOrientation()
		moWorksheet.PageSetup.Orientation = XlPageOrientation.xlLandscape
	End Sub
	Public Sub Close()
		moWorksheet = Nothing
		If moWorkbook IsNot Nothing Then
			Try
				moWorkbook.Close()
			Catch oEx As Exception
			End Try
		End If
		moWorkbook = Nothing
		If moExcelApp IsNot Nothing Then
			Try
				moExcelApp.Quit()
			Catch oEx As Exception
			End Try
		End If
		moExcelApp = Nothing
	End Sub
	Public Sub InsertCaptions(ByVal iColVisibleUB As Integer, iMidColIndex As Integer, Optional bInsertRows As Boolean = False)
		zzInsertCaptions(iColVisibleUB, iMidColIndex, bInsertRows)

	End Sub
	Private Sub zzInsertCaptions(ByVal iColVisibleUB As Integer, ByVal iMidColumnIndex As Integer, Optional bInsertRows As Boolean = False)
		'   Dim iMidIndex As Integer
		Dim iLabelColIndex As Integer
		Dim iValueColIndex As Integer
		Dim tCaption As Caption
		Dim oRange As Excel.Range
		'    If moWorksheet Is Nothing Then
		moWorksheet = DirectCast(moWorkbook.Sheets.Item(1), Excel.Worksheet)
		'   End If

		' iMidIndex = iColVisibleUB \ 2
		For iIndex As Integer = iMidColumnIndex To 0 Step -1
			oRange = GetRange(0, iMidColumnIndex)
			If CInt(oRange.ColumnWidth) <> 0 Then
				iLabelColIndex = iIndex
				Exit For
			End If
		Next
		For iIndex As Integer = iMidColumnIndex + 1 To iColVisibleUB
			oRange = GetRange(0, iIndex)
			If CInt(oRange.ColumnWidth) <> 0 Then
				iValueColIndex = iIndex
				Exit For
			End If
		Next
		Dim oRowRange As Range



		'	iLabelColIndex = iColVisibleUB \ 2

		'	iValueColIndex = iLabelColIndex + 1
		For iIndex As Integer = 0 To miDataFirstRow - 1
			If bInsertRows Then
				oRowRange = DirectCast(moWorksheet.Rows.Item(iIndex + 1), Range)
				oRowRange.Insert(XlInsertShiftDirection.xlShiftDown)
			End If
			If iIndex = 0 Then
				oRange = GetRange(iIndex, iColVisibleUB - 1)
				oRange.Value = Date.Today
				'  oRange.ClearFormats()
			End If
			tCaption = maCaptions.Item(iIndex)
			oRange = GetRange(iIndex, iLabelColIndex)
			oRange.Value = tCaption.Label
			oRange.Font.Bold = True
			oRange.WrapText = False

			oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft

			oRange = GetRange(iIndex, iValueColIndex)
			oRange.Value = tCaption.Value
			oRange.Font.Bold = True
			oRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight
			oRange.WrapText = False
			''''''''''''''''''''''''''''   oRange.Insert()
			'  Exit For
		Next
		zzClearCaptions()
	End Sub
	Private Sub zzClearCaptions()
		If maCaptions IsNot Nothing Then
			maCaptions.Clear()
		End If

		miDataFirstRow = 0
	End Sub
End Class
