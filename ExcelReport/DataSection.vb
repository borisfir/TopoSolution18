Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Excel

Public Class DataSection
   Inherits AcadReport.BaseDataSection
	Private miRowLB As Integer
	' Private miFirstDataRowAAA As Integer = 0
   Private miDataRowCount As Integer = -1
   Private mdHeaderHeight As Double
   Private mdFooterHeight As Double
   Private mdReportHeight As Double
   Private mdMaxTableHeight As Double
   Private mbRepContinue As Boolean = False
   '  Private moCurrentRange As Excel.Range = Nothing
   Private WithEvents moCurrentMergeColumn As DataMerge
	Public Sub New()
		MyBase.New()
		dbRightToLeft = True
	End Sub
	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, ByVal oMainView As System.Data.DataView, ByVal bAreaMeter As Boolean, ByVal bMergeRows As Boolean, Optional ByVal iaDataColumns() As Integer = Nothing)
		MyBase.New(oResource, oMainView, True, bMergeRows, iaDataColumns)
		MyBase.diSectionType = AcadReport.enSectionType.Excel Or AcadReport.enSectionType.Data
		dbAreaMeter = bAreaMeter
	End Sub

	Public Sub New(ByVal oMainView As System.Data.DataView, Optional ByVal iaDataColumns() As Integer = Nothing)
		MyBase.New(oMainView, True, iaDataColumns)
		MyBase.diSectionType = AcadReport.enSectionType.Excel Or AcadReport.enSectionType.Data

	End Sub

   Public ReadOnly Property RepContinue() As Boolean
      Get
         Return mbRepContinue
      End Get
   End Property
	Public Overrides Sub Print()
		zzInsertRows()
		zzSetBorderSection()
	End Sub

   Public Sub SetHeaderFooterHeight(ByVal dHeaderHeight As Double, ByVal dFooterHeight As Double)
      mdHeaderHeight = dHeaderHeight
      mdFooterHeight = dFooterHeight
   End Sub
	Public Sub DataAllFormatAAA()

	End Sub
	Private Sub zzInsertRows()
		miRowLB = MyBase.FirstRow
		'	zzDispMergeColumns("01_340 ExcelMergeColumns")
		Try
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!Excel InsertRows", AcadReport.BaseReport.ColumnUB)

			For iRowIndex As Integer = miRowLB To MyBase.diLastRow  'Temp
				zzFillRow(iRowIndex)
			Next
			'	Dim iColIndexUpd As Integer

			For iColIndex As Integer = 0 To AcadReport.BaseReport.ColumnUB

				If doaMergeColumns(iColIndex) IsNot Nothing AndAlso doaMergeColumns(iColIndex).IsMergeBase Then
					moCurrentMergeColumn = doaMergeColumns(iColIndex)
					moCurrentMergeColumn.Close()
				End If
			Next

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzInsertRows")
		End Try


	End Sub
	Private Sub zzFillRow_03022020(ByVal iRowIndex As Integer)

		Dim oTableCell As AcadReport.TableCell

		Dim oDataRowView As System.Data.DataRowView
		Dim iDataRowIndex As Integer = diFirstDataRow + iRowIndex - MyBase.FirstRow
		Dim oRowRange As Range = Nothing
		Dim oValue As System.Object
		Dim sValue As String = "Stam"
		Dim iDataColIndex As Integer
		Dim iColIndexUpd As Integer

		Dim sTest As String = "a"
		Dim oRange As Excel.Range
		Try
			oDataRowView = doMainView.Item(iDataRowIndex)
		Catch oEx As Exception

			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFillRow_11")
			Return
		End Try
		''''''''''''''''	Report.Reverse = True

		'System.Windows.Forms.MessageBox.Show(CStr(Report.Reverse), "02_232")
		For iColIndex As Integer = 0 To AcadReport.BaseReport.ColumnUB
			Try
				oValue = Nothing
				If Me.diCellTypeUB >= 0 Then
					'oTableCell = MyBase.doaTableCell(iColIndex)
					If Report.Reverse Then                   ''''Topo: Not

						oTableCell = MyBase.doaTableCell(iColIndex)
					Else
						oTableCell = MyBase.doaTableCell(AcadReport.BaseReport.ColumnUB - iColIndex)

					End If
				Else
					oTableCell = MyBase.doTableCell
				End If


				oTableCell.DestType = AcadReport.enDestType.WinApp
				'  If oTableCell.GridLineType <> 0 Then
				'     Report.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, oTableCell.GridLineType, oTableCell.GridLineWeight)
				' End If
				sTest = "b"
				If Report.Reverse Then
					'iDataColIndex = iColIndex
					iColIndexUpd = AcadReport.BaseReport.ColumnUB - iColIndex

				Else
					'	iDataColIndex = AcadReport.BaseReport.ColumnUB - iColIndex
					iColIndexUpd = iColIndex
				End If


				sTest = "C"
				If dbDataColumnsExist Then
					sTest = "Da"
					iDataColIndex = diaDataColumns(iColIndexUpd)
				Else
					iDataColIndex = iColIndexUpd
				End If
				If IsDBNull(oDataRowView.Item(iDataColIndex)) Then
					sTest = "Db"
					sValue = String.Empty
				Else
					sTest = "Dx"
					oValue = oDataRowView.Item(iDataColIndex)
				End If
				sTest = "E"

				If doaMergeColumns(iColIndex) IsNot Nothing Then


					If doaMergeColumns(iColIndex).IsMergeBase Then
						moCurrentMergeColumn = doaMergeColumns(iColIndex)
						oRange = Nothing
						sTest = "Eaaa"
					End If

					Select Case oTableCell.Feature
						Case AcadReport.CellFeatures.Default
							If sValue Is Nothing Then sValue = "Nothing"
							If oValue IsNot Nothing Then
								sValue = oTableCell.Format(oValue, True, dbAreaMeter)


							End If


							doaMergeColumns(iColIndex).SetValue(iRowIndex, sValue, Nothing)
						Case AcadReport.CellFeatures.BackgroundFill
							If oValue IsNot Nothing Then
								Dim oDMColor As DMAcadExt.DMColor
								oDMColor = DirectCast(oValue, DMAcadExt.DMColor)
								doaMergeColumns(iColIndex).SetValue(iRowIndex, sValue, oDMColor)

							End If

					End Select
					'		System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex) & ":" & CStr(iColIndexUpd) & vbCrLf & sValue & vbCrLf & DMCommon.Functions.ObjToString(oValue, "Noth"), "01_902")

				Else
					Dim sAAA As String = vbCrLf
					sTest = "Eb"
					oRange = Report.GetInputCell(iRowIndex, iColIndex)
					sTest = "Ebb"
					If oRange IsNot Nothing Then
						zzFormatRange(oRange, oTableCell)


						Select Case oTableCell.Feature
							Case AcadReport.CellFeatures.Default
								If oValue IsNot Nothing Then
									sValue = oTableCell.Format(oValue, True, dbAreaMeter)
								End If
								If sValue.Length <> 0 Then
									If oRange.NumberFormat Is Nothing Then
										sValue = "'" & sValue
									End If
									oRange.Value = sValue '
								End If
							Case AcadReport.CellFeatures.BackgroundFill
								Dim oDMColor As DMAcadExt.DMColor
								Try

									If oValue IsNot Nothing Then
										oDMColor = DirectCast(oValue, DMAcadExt.DMColor)


										'System.Windows.Forms.MessageBox.Show(oDMColor.RGB.ToString(), "02_996")

										oRange.Interior.Color = oDMColor.RGB
									End If
								Catch oEx As Exception

								End Try


						End Select
						''''''''''''''''


						If MyBase.dbAutoFit AndAlso oRowRange Is Nothing Then
							oRowRange = oRange.EntireRow
						End If
					End If

					sTest = "Ebbb"
				End If
				sTest = "F"


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex), "Data Print 5389")
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sValue & vbCrLf & sTest, "DataSection - zzFillRow_3")
			End Try
		Next

		If MyBase.dbAutoFit AndAlso oRowRange IsNot Nothing Then
			Try
				oRowRange.RowHeight = ddaRowHeight(0)
				oRowRange.AutoFit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFillRow_5")
			End Try
			If dRowHeightExcelScale >= 0.0 Then
				If DirectCast(oRowRange.RowHeight, Double) < dRowHeightExcelScale * ddaRowHeight(0) Then
					oRowRange.RowHeight = dRowHeightExcelScale * ddaRowHeight(0)
				End If
			End If
		End If
	End Sub
	Private Sub zzFillRow(ByVal iRowIndex As Integer)

		Dim oTableCell As AcadReport.TableCell

		Dim oDataRowView As System.Data.DataRowView
		Dim iDataRowIndex As Integer = diFirstDataRow + iRowIndex - MyBase.FirstRow
		Dim oRowRange As Range = Nothing
		Dim oValue As System.Object
		Dim sValue As String = "Stam"
		Dim sMergeValue As String = Nothing
		Dim iDataColIndex As Integer
		Dim iColIndexUpd As Integer

		Dim sTest As String = "a"
		Dim oRange As Excel.Range
		Try
			oDataRowView = doMainView.Item(iDataRowIndex)
		Catch oEx As Exception

			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFillRow_11")
			Return
		End Try
		''''''''''''''''	Report.Reverse = True

		'System.Windows.Forms.MessageBox.Show(CStr(Report.Reverse), "02_232")
		For iColIndex As Integer = 0 To AcadReport.BaseReport.ColumnUB
			Try
				oValue = Nothing
				If Me.diCellTypeUB >= 0 Then
					'oTableCell = MyBase.doaTableCell(iColIndex)
					If Report.Reverse Then						  ''''Topo: Not

						oTableCell = MyBase.doaTableCell(iColIndex)
					Else
						oTableCell = MyBase.doaTableCell(AcadReport.BaseReport.ColumnUB - iColIndex)

					End If
				Else
					oTableCell = MyBase.doTableCell
				End If


				oTableCell.DestType = AcadReport.enDestType.WinApp
				'  If oTableCell.GridLineType <> 0 Then
				'     Report.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, oTableCell.GridLineType, oTableCell.GridLineWeight)
				' End If
				sTest = "b"
				If Report.Reverse Then
					'iDataColIndex = iColIndex
					iColIndexUpd = AcadReport.BaseReport.ColumnUB - iColIndex

				Else
					'	iDataColIndex = AcadReport.BaseReport.ColumnUB - iColIndex
					iColIndexUpd = iColIndex
				End If


				sTest = "C"
				If dbDataColumnsExist Then
					sTest = "Da"
					iDataColIndex = diaDataColumns(iColIndexUpd)
				Else
					iDataColIndex = iColIndexUpd
				End If
				If IsDBNull(oDataRowView.Item(iDataColIndex)) Then
					sTest = "Db"
					sValue = String.Empty
				Else
					sTest = "Dx"
					oValue = oDataRowView.Item(iDataColIndex)
				End If
				sTest = "E"

				If doaMergeColumns(iColIndex) IsNot Nothing Then
					'	System.Windows.Forms.MessageBox.Show(CStr(iColIndex) & ":" & CStr(iColIndexUpd), "01_901")

					If doaMergeColumns(iColIndex).IsMergeBase Then
						moCurrentMergeColumn = doaMergeColumns(iColIndex)
						oRange = Nothing
						sTest = "Eaaa"
					End If
					If String.IsNullOrEmpty(sMergeValue) Then
						sMergeValue = oValue.ToString()
					Else
						sMergeValue &= "|" & oValue.ToString()
					End If

					Select Case oTableCell.Feature
						Case AcadReport.CellFeatures.Default
							If sValue Is Nothing Then sValue = "Nothing"
							If oValue IsNot Nothing Then
								sValue = oTableCell.Format(oValue, True, dbAreaMeter)
							End If

							'	DMCommon.Debug.MsgBox("13_341c", iColIndex, sValue)
							doaMergeColumns(iColIndex).SetValue(iRowIndex, sValue, Nothing)
						Case AcadReport.CellFeatures.BackgroundFill
							If oValue IsNot Nothing Then
								Dim oDMColor As DMAcadExt.DMColor
								oDMColor = DirectCast(oValue, DMAcadExt.DMColor)
								doaMergeColumns(iColIndex).SetValue(iRowIndex, sValue, oDMColor)

							End If

					End Select
					'		System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex) & ":" & CStr(iColIndexUpd) & vbCrLf & sValue & vbCrLf & DMCommon.Functions.ObjToString(oValue, "Noth"), "01_902")
				
				Else
					Dim sAAA As String = vbCrLf
					sTest = "Eb"
					oRange = Report.GetInputCell(iRowIndex, iColIndex)
					sTest = "Ebb"
					If oRange IsNot Nothing Then
						zzFormatRange(oRange, oTableCell)


						Select Case oTableCell.Feature
							Case AcadReport.CellFeatures.Default
								If oValue IsNot Nothing Then
                           sValue = oTableCell.Format(oValue, True, dbAreaMeter)
								End If
								If sValue.Length <> 0 Then
									If oRange.NumberFormat Is Nothing Then
										sValue = "'" & sValue
									End If
									oRange.Value = sValue '
								End If
							Case AcadReport.CellFeatures.BackgroundFill
								Dim oDMColor As DMAcadExt.DMColor
								Try

									If oValue IsNot Nothing Then
										oDMColor = DirectCast(oValue, DMAcadExt.DMColor)


										'System.Windows.Forms.MessageBox.Show(oDMColor.RGB.ToString(), "02_996")

										oRange.Interior.Color = oDMColor.RGB
									End If
								Catch oEx As Exception

								End Try


						End Select
						''''''''''''''''


						If MyBase.dbAutoFit AndAlso oRowRange Is Nothing Then
							oRowRange = oRange.EntireRow
						End If
					End If

					sTest = "Ebbb"
				End If
				sTest = "F"


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex), "Data Print 5389")
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sValue & vbCrLf & sTest, "DataSection - zzFillRow_3")
			End Try
		Next

		If MyBase.dbAutoFit AndAlso oRowRange IsNot Nothing Then
			Try
				oRowRange.RowHeight = ddaRowHeight(0)
				oRowRange.AutoFit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFillRow_5")
			End Try
			If dRowHeightExcelScale >= 0.0 Then
				If DirectCast(oRowRange.RowHeight, Double) < dRowHeightExcelScale * ddaRowHeight(0) Then
					oRowRange.RowHeight = dRowHeightExcelScale * ddaRowHeight(0)
				End If
			End If
		End If
	End Sub
	Private Sub zzFormatRange(ByVal oRange As Excel.Range, ByVal oTableCell As AcadReport.TableCell)
		Dim oCellData As AcadReport.CellData
		Dim oRowRange As Range = Nothing
		Dim sTest As String = "a"




		sTest = "b"
		oTableCell.DestType = AcadReport.enDestType.WinApp
		sTest = "c"
		''   If oTableCell.GridLineType <> 0 Then
		'     Report.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, oTableCell.GridLineType, oTableCell.GridLineWeight)
		''   End If
		If oTableCell.TextAlignmentDefined Then
			Try
				oRange.HorizontalAlignment = Report.GetHAlign(oTableCell)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_11/" & CStr(MyBase.diSectionID))
			End Try
			Try
				oRange.VerticalAlignment = Report.GetVAlign(oTableCell)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_12/" & CStr(MyBase.diSectionID))
			End Try
		End If

		Try
			If MyBase.dtExcelFont.Exists Then
				sTest = "d-" & MyBase.dtExcelFont.Name
				oRange.Font.Name = MyBase.dtExcelFont.Name
				sTest = "dq"
				If Not MyBase.dtExcelFont.DefaultSize Then
					sTest = "dr"
					oRange.Font.Size = dtExcelFont.Size
				End If
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_13/" & CStr(MyBase.diSectionID))
		End Try
		sTest = "P"
	 

		If oTableCell.CellValue IsNot Nothing Then
			oCellData = oTableCell.CellValue(0)
		End If

		sTest = "PX"
		If oCellData.CellDataType = AcadReport.enCellDataType.Invert Then
			sTest = "Q"
			Try
				oRange.ReadingOrder = -5004
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_14/" & CStr(MyBase.diSectionID))
			End Try

			sTest = "R"
			'		oRange.NoteText()
			If MyBase.dbAutoFit Then
				Try
					oRange.WrapText = True
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_15/" & CStr(MyBase.diSectionID))
				End Try
				oRange.NoteText()
			End If
		ElseIf oCellData.CellDataType = AcadReport.enCellDataType.String Then
			If MyBase.dbAutoFit Then
				Try
					oRange.WrapText = True
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_16/" & CStr(MyBase.diSectionID))
				End Try
			End If
			oRange.NoteText()

		ElseIf oCellData.CellDataType = AcadReport.enCellDataType.FormatNum Then
			Try
				oRange.NumberFormat = oCellData.GetWinNumFormat(dbAreaMeter)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!FormatNum", oCellData.GetWinNumFormat(dbAreaMeter))
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzFormatRange_17/" & CStr(MyBase.diSectionID))
			End Try
		End If

	End Sub

	Private Sub zzSetBorderSection()
		Dim sExcelBorderDesc As String = MyBase.dsExcelBorderDesc
		Dim oAllDataRange As Excel.Range = Report.GetRange(miRowLB, 0, MyBase.diLastRow, AcadReport.BaseReport.ColumnUB)
		'	System.Windows.Forms.MessageBox.Show(diExcelBorderType.ToString(), "01_066")
		If diExcelBorderType = AcadReport.enExcelBorderType.AllVertical Then
			Try
				With oAllDataRange.Borders.Item(XlBordersIndex.xlInsideVertical)
					.Weight = XlBorderWeight.xlMedium
					.LineStyle = XlLineStyle.xlContinuous
				End With
			Catch oEx As Exception
			End Try

			Try
				With oAllDataRange.Borders.Item(XlBordersIndex.xlEdgeLeft)
					.Weight = XlBorderWeight.xlMedium
					.LineStyle = XlLineStyle.xlContinuous
				End With
			Catch oEx As Exception
			End Try
		

			Try
				With oAllDataRange.Borders.Item(XlBordersIndex.xlEdgeRight)
					.Weight = XlBorderWeight.xlMedium
					.LineStyle = XlLineStyle.xlContinuous
				End With
			Catch oEx As Exception
			End Try
		End If
	End Sub
	Private Sub moCurrentMergeColumn_Cell(ByVal iRow As Integer, ByVal iCol As Integer) Handles moCurrentMergeColumn.Cell
		Dim oExcelRange As Excel.Range = Nothing '''''''''''= Application.Merge(oRect)
		'	System.Windows.Forms.MessageBox.Show(CStr(iRow) & ":" & CStr(iCol), "01_481")
		Try
			oExcelRange = Report.GetInputCell(iRow, iCol)
			If oExcelRange IsNot Nothing Then
				Dim oTableCell As AcadReport.TableCell = MyBase.doaTableCell(iCol)
				Select Case oTableCell.Feature
					Case AcadReport.CellFeatures.Default
						oExcelRange.Value = moCurrentMergeColumn.CurrentValue
					Case AcadReport.CellFeatures.BackgroundFill
						System.Windows.Forms.MessageBox.Show(iRow.ToString() & ":" & iCol.ToString(), "02_998")

				End Select
				zzFormatRange(oExcelRange, oTableCell)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - moCurrentMergeColumn_Cell")
		End Try
	End Sub

	Private Sub moCurrentMergeColumn_Merge(ByVal oRect As System.Drawing.Rectangle, ByVal iSubColIndex As Integer, ByVal iGridLineWeight As Integer, ByVal iGridLineStyle As Integer, ByVal bWholeLine As Boolean) Handles moCurrentMergeColumn.Merge
		Dim oExcelRange As Excel.Range = Nothing '''''''''''= Application.Merge(oRect)
		Dim oSubCol As DataMerge
		Try
			If True Then
				oExcelRange = Report.Merge(oRect)
				oSubCol = doaMergeColumns(iSubColIndex)
				''''''''''	Dim oTableCell As AcadReport.TableCell = MyBase.doaTableCell(oRect.X)
				Dim oTableCell As AcadReport.TableCell = MyBase.doaTableCell(AcadReport.BaseReport.ColumnUB - oSubCol.ColumnNo)
				Dim sTest As String = "DFLT"
				For i As Integer = 0 To doaTableCell.GetUpperBound(0)
					If MyBase.doaTableCell(i).Feature = AcadReport.CellFeatures.BackgroundFill Then
						sTest = CStr(i) & ":FILL"
					End If
				Next
				'	DMCommon.Debug.MsgBox("13_341i", iSubColIndex, oSubCol Is Nothing, oSubCol.CurrentValue, oExcelRange.Row, oExcelRange.Column, oExcelRange.Areas.Count, oExcelRange.Address, oExcelRange.Width, oExcelRange.Height)
				'	System.Windows.Forms.MessageBox.Show(sTest & vbCrLf & oSubCol.CurrentValue & vbCrLf & CStr(oRect.X) & vbCrLf & CStr(AcadReport.BaseReport.ColumnUB - oSubCol.ColumnNo) & vbCrLf & DMCommon.Functions.CStrN(moCurrentMergeColumn.CurrentValue & vbCrLf & oTableCell.Feature.ToString(), "NN"), "02_410")
				If oSubCol Is Nothing Then
					''''''''''''''  System.Windows.Forms.MessageBox.Show(CStr(iSubColIndex), moCurrentMergeColumn.CurrentValue)
				Else
					'	System.Windows.Forms.MessageBox.Show(oSubCol.CurrentValue & vbCrLf & CStr(iSubColIndex) & vbCrLf & CStr(AcadReport.BaseReport.ColumnUB - oSubCol.ColumnNo) & vbCrLf & oTableCell.Feature.ToString(), "04_420")
					Select Case oTableCell.Feature
						Case AcadReport.CellFeatures.Default
							oExcelRange.Value = oSubCol.CurrentValue
						Case AcadReport.CellFeatures.BackgroundFill
							'System.Windows.Forms.MessageBox.Show(oSubCol.CurrentBackColor.RGB.ToString(), "02_997")
							If Not oSubCol.CurrentBackColor.IsEmpty Then
								oExcelRange.Interior.Color = oSubCol.CurrentBackColor.RGB
							End If
					End Select
					' moCurrentMergeColumn.CurrentValue
				End If


				zzFormatRange(oExcelRange, oTableCell)
				oExcelRange = Report.GetRange(oRect.Bottom, 0, oRect.Bottom, oRect.Right + 1)
				If oExcelRange IsNot Nothing Then
					If iGridLineWeight >= 3 Then
						oExcelRange.Borders.Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium
					End If
					If iGridLineStyle = 2 Then
						oExcelRange.Borders.Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlDouble
					End If
				End If
			End If


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(oRect.Bottom) & ":" & CStr(oRect.Right), "Excel DataSection - moCurrentMergeColumn_Merge")
		End Try

	End Sub

   
End Class
