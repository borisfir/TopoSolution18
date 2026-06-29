Option Explicit On
Option Strict On
Imports System.Drawing
Imports Autodesk.AutoCAD.DatabaseServices
' GridLineType
Friend Class HeaderSection
   Inherits BaseHeaderSection
   Public Sub New(ByVal bAreaMeter As Boolean)
      MyBase.diSectionType = enSectionType.Autocad Or enSectionType.HeaderBase
      MyBase.RowCount = 1
      dbAreaMeter = bAreaMeter
   End Sub


   Public Sub New(ByVal oResource As TPlServerDB.TPlResource, ByVal bAreaMeter As Boolean, Optional ByVal oaData() As System.Object = Nothing)
		MyBase.New(oResource, oaData)
		'DMCommon.Debug.MsgBox("13_130E", oaData)
		'  DMCommon.Functions.DispArray(oaData, "#X doaData-HSection", True)
		MyBase.diSectionType = enSectionType.Autocad Or enSectionType.HeaderBase
      dbAreaMeter = bAreaMeter
   End Sub
   

	Public Overrides Sub Print()

		If diSectionID >= 0 AndAlso MyBase.RowCount > 0 Then
			Dim oMergeRegion As Rectangle
			zzInsertRows()
			'''''''''''''	AcadReport.Report.ReDrawTable()
			If doMerges IsNot Nothing Then
				For iIndex As Integer = 0 To doMerges.GetUpperBound(0)
					oMergeRegion = doMerges(iIndex)
					zzMerge(oMergeRegion, True)
				Next
			End If
			zzEnumCell()
		End If
	End Sub
   Private Sub zzInsertRows()
      Dim iRowLB As Integer
      Dim dRowHeight As Double

      If MyBase.FirstRow = 0 Then
			Try

				'	MessageBox.Show(Report.AcadTable.NumRows.ToString() & ":" & Report.AcadTable.NumColumns.ToString(), "17_402r")
				'	MessageBox.Show(CStr(ddaRowHeight(0)) & "*" & CStr(Report.DrawingScaleFactor), "17_404s")
				'Report.AcadTable.SetRowHeight(0, ddaRowHeight(0) * Report.DrawingScaleFactor)	'obs
				RepApp.AcadTable.Rows.Item(0).Height = ddaRowHeight(0) * RepApp.DrawingScaleFactor
				'	Report.SetTextStyle(0, 0, 0.25)
				'	Report.AcadTable.SetCellStyle(0, 0, "Header")
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "HeaderSection - zzInsertRows_10")
				System.Windows.Forms.MessageBox.Show(CStr(ddaRowHeight(0)) & ":" & CStr(RepApp.DrawingScaleFactor), "HeaderSection - InsertRows_az_" & CStr(diSectionID))
			End Try
			iRowLB = 1
		Else
			iRowLB = MyBase.FirstRow

		End If
		Dim iRowIndex As Integer

		Try
			For iRowIndex = iRowLB To diLastRow
				dRowHeight = ddaRowHeight(iRowIndex - MyBase.FirstRow)
				RepApp.AcadTable.InsertRows(RepApp.AcadTable.Rows.Count, dRowHeight * RepApp.DrawingScaleFactor, 1)
				'	MessageBox.Show(CStr(Report.AcadTable.NumRows) & ":" & CStr(iRowIndex) & ":" & CStr(dRowHeight) & "*" & CStr(Report.DrawingScaleFactor), "19_800")
				'	Report.SetTextStyle(iRowIndex, 0, 0.25)
				'		Report.SetTextStyle(iRowIndex, iColIndex, oTableCell.TextStyleIndex, 0.15)
				'	Report.SetTextStyle(iRowIndex, 0, 0.15)
				'	Report.AcadTable.SetCellStyle(iRowIndex, 0, "Header")
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "HeaderSection - InsertRows_" & CStr(diSectionID))
			System.Windows.Forms.MessageBox.Show(CStr(RepApp.AcadTable.Rows.Count) & ":" & CStr(iRowIndex), "NumRows:RowIndex-" & CStr(diSectionID))

		End Try


	End Sub
	Private Sub zzEnumCell()
		Const iBorderLineWeight As Integer = 40
		Dim bRes As Boolean?
		Dim oTableRegion As CellRange = Nothing 'TableRegion
		Dim iCellIndex As Integer = 0
		Dim sValue As String = String.Empty
		Dim oTableCell As TableCell
		Dim bCellIsInput As Boolean
		Dim iLineWeight As LineWeight
		If [Enum].IsDefined(GetType(LineWeight), iBorderLineWeight) Then
			iLineWeight = CType(iBorderLineWeight, LineWeight)
		End If
		'	Dim sTest As String = "doaData=Nothing"
		Dim iColUB As Integer = BaseReport.ColumnUB
		'  DMCommon.Functions.DispArray(doaData, "#1 doaData-HSection", True)
		'  System.Windows.Forms.MessageBox.Show(iColUB.ToString(), "01_773a")
		For iRowIndex As Integer = MyBase.FirstRow To MyBase.diLastRow

			For iColIndex As Integer = 0 To iColUB
				If doMerges IsNot Nothing Then
					Try
						'
						If RepApp.AcadTable Is Nothing Then
							System.Windows.Forms.MessageBox.Show("Report.AcadTable Is Nothing", "01_768a")
						End If
						'oTableRegion = Report.AcadTable.IsMergedCell(iRowIndex, iColIndex)
						'	bRes = Report.AcadTable.GetMergeAllEnabled(iRowIndex, iColIndex) 'obs
						bRes = RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).IsMergeAllEnabled
						'	Report.AcadTable.SetMergeAllEnabled(iRowIndex, iColIndex, True)  obs
						'
						RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).IsMergeAllEnabled = True
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "!!B!zzEnumCell-" & CStr(MyBase.diSectionID))
						bCellIsInput = True
					End Try


					'	oTableRegion = Report.AcadTable.GetMergeRange(iRowIndex, iColIndex)  obs
					Try
						oTableRegion = RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).GetMergeRange()
					Catch oEx As Exception
						oTableRegion = Nothing
					End Try

					Dim bNothing As Boolean
					Dim iRow As Integer = -2
					Dim iCol As Integer = -2
					bNothing = oTableRegion Is Nothing

					Try
						If oTableRegion Is Nothing OrElse (oTableRegion.TopRow = -1 AndAlso oTableRegion.LeftColumn = -1) Then
							bCellIsInput = True

						Else
							bCellIsInput = (oTableRegion.TopRow = iRowIndex) AndAlso (oTableRegion.LeftColumn = iColIndex)
						End If


						'		System.Windows.Forms.MessageBox.Show(CStr(bCellIsInput) & ":" & sTest, "12_089z")
						'		MessageBox.Show(CStr(oTableRegion.TopRow) & ":" & CStr(oTableRegion.LeftColumn), "12_797")

					Catch oEx As Exception
						'System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "!!C!zzEnumCell-" & CStr(MyBase.diSectionID))
						bCellIsInput = True
					End Try
				Else
					'	System.Windows.Forms.MessageBox.Show(CStr(Me.diSectionID) & vbCrLf & Me.diSectionType.ToString(), "01_768b")
					bCellIsInput = True
				End If


				Try
					If bCellIsInput AndAlso iCellIndex <= diCellTypeUB Then    '''''diCellTypeUB 
						oTableCell = MyBase.doaTableCell(iCellIndex)

						'   System.Windows.Forms.MessageBox.Show(CStr(iCellIndex) & ":" & CStr(diCellTypeUB) & vbCrLf & CStr(doaTableCell.GetUpperBound(0)) & vbCrLf & CStr(oTableCell.ResIndex) & ":" & CStr(oTableCell.GroupIndex) & vbCrLf & CStr(Report.GroupColumnStart) & ":" & Report.GroupColumnUB.ToString(), "02_500a!")
						'	Report.AcadTable.SetTextHeight(iRowIndex, iColIndex, oTableCell.TextHeight * Report.DrawingScaleFactor)
						Dim iTextStyleIndex As Integer = oTableCell.TextStyleIndex
						Dim d1 As Double = oTableCell.TextHeight
						Dim d2 As Double = RepApp.DrawingScaleFactor
						Dim i2 As CellAlignment = oTableCell.TextAlignment
						'	DMAcadExt.AcadDocument.WriteMessage("TextHeight-*" & CStr(oTableCell.TextHeight * Report.DrawingScaleFactor) & ":" & CStr(iRowIndex) & "," & CStr(iColIndex))
						Try
							RepApp.SetTextStyle(iRowIndex, iColIndex, iTextStyleIndex, d1 * d2)
						Catch oEx As Exception
							Dim sMsg As String = oEx.Message & vbCrLf
							sMsg &= CStr(MyBase.FirstRow) & ":" & CStr(diLastRow) & vbCrLf
							sMsg &= CStr(iTextStyleIndex) & ":H=" & CStr(d1 * d2)
							System.Windows.Forms.MessageBox.Show(sMsg, "EnumCell_1 - " & CStr(MyBase.diSectionID))
						End Try
						RepApp.SetAlignment(iRowIndex, iColIndex, oTableCell.TextAlignment)
						If oTableCell.GridLinesExist Then
							For iIndex As Integer = 0 To oTableCell.GridLines.GetUpperBound(0)
								'	RepApp.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, oTableCell.GridLines(iIndex).Type, oTableCell.GridLines(iIndex).Weight)
								RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).Borders.Left.LineWeight = oTableCell.GridLines(iIndex).Weight

								' , oTableCell.GridLines(iIndex).Type, oTableCell.GridLines(iIndex).Weight)

								'	Dim oCell As Cell = Report.AcadTable.Cells.Item(iRowIndex, iColIndex)
								'	oCell.Borders.Left.LineWeight = oTableCell.GridLines(iIndex).Weight
								''	Report.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, GridLineType.HorizontalBottom, LineWeight.LineWeight040)
								'Correction Bug
								If oTableRegion IsNot Nothing AndAlso (oTableRegion.TopRow <> oTableRegion.BottomRow) AndAlso ((oTableCell.GridLines(iIndex).Type And GridLineType.HorizontalBottom) = GridLineType.HorizontalBottom) Then
									For iRegionCol As Integer = oTableRegion.LeftColumn To oTableRegion.RightColumn
										RepApp.AcadTable.SetGridLineWeight(oTableRegion.BottomRow, iRegionCol, GridLineType.HorizontalBottom, oTableCell.GridLines(iIndex).Weight)
										RepApp.AcadTable.Cells.Item(oTableRegion.BottomRow, iRegionCol).Borders.Bottom.LineWeight = oTableCell.GridLines(iIndex).Weight
									Next
								End If
								'	Report.AcadTable.SetGridLineStyle()

							Next
							'	Report.AcadTable.SetGridLineWeight(1, 4, GridLineType.HorizontalBottom, LineWeight.LineWeight040)

						End If

						If oTableCell.ContentColor IsNot Nothing AndAlso oTableCell.ContentColor <> Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 0S) Then
							'Report.AcadTable.SetContentColor(iRowIndex, iColIndex, oTableCell.ContentColor)	 obs	'oTableCell.ContentColor
							RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).ContentColor = oTableCell.ContentColor
						End If
						If oTableCell.BackgroundColor IsNot Nothing AndAlso oTableCell.BackgroundColor <> Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 0S) Then
							'Report.AcadTable.SetBackgroundColor(iRowIndex, iColIndex, oTableCell.BackgroundColor) obs
							RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).BackgroundColor = oTableCell.BackgroundColor
						End If

						sValue = BaseReport.GetText(oTableCell.ResIndex, MyBase.diSectionID)
						'DMCommon.Debug.ExcelLog.SetValue(0, "REPheader", iCellIndex, diCellTypeUB, oTableCell.Description, sValue, doaData.GetUpperBound(0))
						'DMCommon.Debug.ExcelLog.SetArray(7, "+doaData", True, doaData)
						sValue = oTableCell.Format(sValue, False, dbAreaMeter, doaData)

						'DMCommon.Debug.ExcelLog.SetNextValue(18, "!repHd", iCellIndex, diCellTypeUB, sValue)
						'	System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & "," & CStr(iColIndex) & "||" & sValue & vbCrLf & CStr(doaData.GetUpperBound(0)), "02_436")
						'System.Windows.Forms.MessageBox.Show(CStr(iCellIndex) & vbCrLf & sValue & vbCrLf & CStr(doaData.GetUpperBound(0)), "01_149 Ex zzEnumCell")
						'	Report.AcadTable.SetCellStyle(iRowIndex, iColIndex, "Header")
						'Report.AcadTable.SetTextStyle(iRowIndex, iColIndex, sValue)
						'	Report.AcadTable.SetTextString(iRowIndex, iColIndex, sValue)  'obs
						RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).TextString = sValue
						iCellIndex += 1

					End If
				Catch oEx As Exception
					Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf
					sMsg &= CStr(MyBase.FirstRow) & ":" & CStr(diLastRow) & vbCrLf
					sMsg &= sValue
					System.Windows.Forms.MessageBox.Show(sMsg, "EnumCell_2 - " & CStr(MyBase.diSectionID))
				End Try
			Next
		Next


		For iColIndex As Integer = 0 To iColUB
			RepApp.AcadTable.Cells.Item(MyBase.diLastRow, iColIndex).Borders.Bottom.LineWeight = iLineWeight
		Next
		'	Report.AcadTable.SetTextString(0, 0, "íÅñ")
		'	Report.AcadTable.SetTextString(0, 2, "2")
		'	Report.AcadTable.SetTextString(0, 3, "a")

		'	Report.AcadTable.SetTextHeight(0, 5, oTableCell.TextHeight * Report.DrawingScaleFactor)
		'	Report.AcadTable.SetAlignment(0, 5, oTableCell.TextAlighment)
		'	Report.AcadTable.SetTextString(0, 5, "ò"éÅ ÑîñòÑ áàô")

	End Sub
	Private Sub zzMerge(ByVal oRect As Rectangle, ByVal bPrint As Boolean)
		'		Dim oTableRegion As CellRange = New CellRange(oRect.Top + MyBase.FirstRow, oRect.Left, oRect.Bottom + MyBase.FirstRow, oRect.Right)
		'Dim oTableRegion As CellRange = CellRange.Create(RepApp.AcadTable, oRect.Top, oRect.Left, oRect.Bottom, oRect.Right)
		Dim oTableRegion As CellRange = CellRange.Create(RepApp.AcadTable, oRect.Top + MyBase.FirstRow, oRect.Left, oRect.Bottom + MyBase.FirstRow, oRect.Right)

		If bPrint Then
			Try
			 
				oTableRegion.IsMergeAllEnabled = True
				RepApp.AcadTable.MergeCells(oTableRegion)
			Catch oEx As Exception
				Dim sRect As String = CStr(oTableRegion.TopRow) & ":" & CStr(oTableRegion.BottomRow) & ":" & CStr(oTableRegion.RightColumn) & ":" & CStr(oTableRegion.LeftColumn)
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(oTableRegion.TopRow) & ":" & CStr(oTableRegion.BottomRow) & ":" & CStr(oTableRegion.RightColumn) & ":" & CStr(oTableRegion.LeftColumn), "HeaderSection - zzMerge:Top,Bottom Right Left!")
			End Try
		Else
			MyBase.Frmt(oRect)
		End If
	End Sub
   
End Class
