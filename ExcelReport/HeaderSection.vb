Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Excel
Friend Class HeaderSection
   Inherits AcadReport.BaseHeaderSection
   Private Const miBackColor As Integer = 12632256
   Private Const miLinestyle As Integer = 1
	Private Const miWeight As Integer = 2
	Private mbReverse As Boolean
   'color 0
   ' Private moMergeCell() As Excel.Range
	Public Sub New(ByVal iSectionType As AcadReport.enSectionType, ByVal bReverse As Boolean)	 ' If Not exists
		MyBase.New()
		MyBase.diSectionType = iSectionType
		'	MyBase.diLastRow = 1

		MyBase.RowCount = 1
	End Sub
	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, ByVal bReverse As Boolean, ByVal bAreaMeter As Boolean, Optional ByVal oaData() As System.Object = Nothing)

		MyBase.New(oResource, oaData)
		MyBase.diSectionType = AcadReport.enSectionType.Excel Or AcadReport.enSectionType.HeaderBase
		dbAreaMeter = bAreaMeter
		mbReverse = bReverse
	End Sub


	Public Overrides Sub Print()
		'System.Windows.Forms.MessageBox.Show(CStr(Me.diSectionID) & vbCrLf & CStr(MyBase.FirstRow) & ":" & CStr(MyBase.diLastRow) & "=" & CStr(MyBase.RowCount), "01_128 Print")
		If MyBase.RowCount > 0 Then
			Dim oMergeRegion As System.Drawing.Rectangle
			'  zzInsertRows()

			If doMerges IsNot Nothing Then
				For iIndex As Integer = 0 To doMerges.GetUpperBound(0)
					oMergeRegion = doMerges(iIndex)
					If Not Report.Reverse Then
						'		System.Windows.Forms.MessageBox.Show(CStr(oMergeRegion.X) & ":" & CStr(oMergeRegion.Y) & "=" & CStr(oMergeRegion.Width) & ":" & CStr(oMergeRegion.Height), "01_141 Print")
						''''''''''''''''''''''''''''''''''''	oMergeRegion = zzMirror(oMergeRegion)
					End If
					oMergeRegion = zzMirror(oMergeRegion)
					zzMerge(oMergeRegion)
				Next
			End If
			zzEnumCell()
		End If
	End Sub
	Private Function zzMirror(ByVal oRegion As System.Drawing.Rectangle) As System.Drawing.Rectangle
		Return New System.Drawing.Rectangle(AcadReport.BaseReport.ColumnUB - oRegion.X - oRegion.Width, oRegion.Y, oRegion.Width, oRegion.Height)
	End Function

	Private Sub zzEnumCell()
		'System.Windows.Forms.MessageBox.Show(CStr(Me.diSectionID) & vbCrLf & CStr(MyBase.FirstRow) & ":" & CStr(MyBase.diLastRow) & "=" & CStr(MyBase.RowCount), "01_157 Enumcell")
		Dim oRange As Excel.Range
		Dim iCellIndex As Integer = 0
		Dim sValue As String = String.Empty
		Dim oTableCell As AcadReport.TableCell
		Dim oCellData As AcadReport.CellData
		Dim sTestA As String
		Dim iColIndexUpd As Integer
		Dim bCellIsInput As Boolean = True
		sTestA = "A"
		Dim sTestVal As String = ""
		'	System.Windows.Forms.MessageBox.Show(CStr(diCellTypeUB) & vbCrLf & CStr(MyBase.FirstRow) & ":" & CStr(MyBase.diLastRow) & vbCrLf & CStr(Me.diSectionID), "01_142 zzEnumCell")
		For iRowIndex As Integer = MyBase.FirstRow To MyBase.diLastRow

			For iColIndex As Integer = 0 To AcadReport.BaseReport.ColumnUB
				sTestA = "B"
				If Report.Reverse Then
					iColIndexUpd = iColIndex

				Else
					iColIndexUpd = AcadReport.BaseReport.ColumnUB - iColIndex
				End If
				'System.Windows.Forms.MessageBox.Show(CStr(mbReverse) & ":" & CStr(MyBase.doaTableCell.GetUpperBound(0)) & ":" & CStr(iColIndexUpd) & ":" & CStr(AcadReport.BaseReport.ColumnUB), "01_178y")
				oRange = Report.GetInputCell(iRowIndex, iColIndexUpd)
				sTestA = "C"
				If bCellIsInput AndAlso iCellIndex <= diCellTypeUB Then


					If oRange IsNot Nothing Then
						Try
							sTestA = "D"
							'	System.Windows.Forms.MessageBox.Show(CStr(MyBase.doaTableCell.GetUpperBound(0)) & ":" & CStr(Me.diCellTypeUB) & ":" & CStr(AcadReport.BaseReport.ColumnUB), "01_141w")
							If Me.diCellTypeUB >= 0 Then
								oTableCell = MyBase.doaTableCell(iCellIndex)
							Else
								oTableCell = MyBase.doTableCell
							End If
							'		System.Windows.Forms.MessageBox.Show(CStr(iCellIndex) & ":" & CStr(diCellTypeUB) & vbCrLf & CStr(doaTableCell.GetUpperBound(0)) & vbCrLf & CStr(oTableCell.ResIndex) & ":" & CStr(oTableCell.GroupIndex), "02_550ex!")


						Catch oEx As Exception
							Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf
							sMsg &= sTestA & vbCrLf
							sMsg &= CStr(iCellIndex) & vbCrLf
							If doaTableCell IsNot Nothing Then
								sMsg &= CStr(doaTableCell.GetUpperBound(0)) & vbCrLf
							End If

							sMsg &= CStr(MyBase.FirstRow) & ":" & CStr(diLastRow) & vbCrLf
							sMsg &= sTestVal & ":" & sValue & vbCrLf
							sMsg &= oRange.RowHeight.GetType().ToString()
							System.Windows.Forms.MessageBox.Show(sMsg, "ExA.HeaderSection - zzEnumCell_" & CStr(MyBase.diSectionID))
							Return
						End Try

						Try


							sTestA = "E"
							oTableCell.DestType = AcadReport.enDestType.WinApp
							sTestA = "F"
							'	System.Windows.Forms.MessageBox.Show(CStr(Me.diCellTypeUB), "01_151 zzEnumCell")
							If diSectionID >= 0 Then

								sValue = AcadReport.BaseReport.GetText(oTableCell.ResIndex, MyBase.diSectionID)

                        sValue = oTableCell.Format(sValue, True, dbAreaMeter, doaData)
								'	DMAcadExt.AcadDocument.WriteMessage("zzEnumCell_A=" & sValue)
							Else
								sValue = AcadReport.BaseReport.ColumnCaptions(iColIndex)
								'	DMAcadExt.AcadDocument.WriteMessage("zzEnumCell_B=" & sValue)
								'	System.Windows.Forms.MessageBox.Show(sValue, "01_147 zzEnumCell")
							End If


							'sValue = BaseReport.GetText(oTableCell.ResIndex, MyBase.diSectionID)
							'sValue = oTableCell.Format(sValue, False, doaData)


							''''''''''''''	System.Windows.Forms.MessageBox.Show(CStr(iCellIndex) & vbCrLf & sValue & vbCrLf & CStr(doaData.GetUpperBound(0)), "01_149 Ex zzEnumCell")
							sTestA = "G"

							sTestVal = sValue
							sTestA = "H"

							oRange.Value = sValue
							sTestA = "K"

							If oTableCell.CellValue IsNot Nothing Then
								oCellData = oTableCell.CellValue(0)
							End If

							oRange.HorizontalAlignment = XlHAlign.xlHAlignCenter
							oRange.VerticalAlignment = XlVAlign.xlVAlignCenter
							If MyBase.dtExcelFont.Exists Then
								oRange.Font.Name = MyBase.dtExcelFont.Name
								If Not MyBase.dtExcelFont.DefaultSize Then
									oRange.Font.Size = dtExcelFont.Size
								End If

							End If

							oRange.Font.Bold = True
							oRange.Interior.Color = miBackColor
							oRange.WrapText = True
							oRange.BorderAround(miLinestyle, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic)
							sTestA = "L"
							If dRowHeightExcelScale >= 0.0 Then
                                If DMCommon.Functions.CDblN(oRange.RowHeight) <> dRowHeightExcelScale * ddaRowHeight(iRowIndex - MyBase.FirstRow) Then
                                    sTestA = "LA"
                                    oRange.RowHeight = dRowHeightExcelScale * ddaRowHeight(iRowIndex - MyBase.FirstRow)
                                End If
								sTestA = "M"
							End If

							oRange.ReadingOrder = -5004
							sTestA = "P"

							If oCellData.CellDataType = AcadReport.enCellDataType.FormatNum Then
								sTestA = "Pa"
								oRange.NumberFormat = oCellData.GetWinNumFormat(dbAreaMeter)
								sTestA = "Pb"
							ElseIf oCellData.CellDataType = AcadReport.enCellDataType.Num Then
								sTestA = "Pc"
								oRange.NumberFormat = oCellData.GetWinNumFormat(dbAreaMeter)
								sTestA = "Pd"
							ElseIf IsNumeric(sValue) OrElse sValue.Contains("/") Then
								sTestA = "Pe"
								oRange.Value = "'" & sValue
								sTestA = "Pf"
							End If
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "headerFormatNum", dbAreaMeter, oCellData.CellDataType, oCellData.GetWinNumFormat(dbAreaMeter))

							iCellIndex += 1
						Catch oEx As Exception
							Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf
							sMsg &= sTestA & vbCrLf
							sMsg &= CStr(iCellIndex) & vbCrLf
							If doaTableCell IsNot Nothing Then
								sMsg &= CStr(doaTableCell.GetUpperBound(0)) & vbCrLf
							End If

							sMsg &= CStr(MyBase.FirstRow) & ":" & CStr(diLastRow) & vbCrLf
							sMsg &= sTestVal & ":" & sValue & vbCrLf
							sMsg &= oRange.RowHeight.GetType().ToString()
							System.Windows.Forms.MessageBox.Show(sMsg, "Ex.HeaderSection - zzEnumCell_" & CStr(MyBase.diSectionID))
						End Try
					End If
				End If
			Next
		Next
	End Sub

	Private Function zzMerge(ByVal oRect As System.Drawing.Rectangle) As Excel.Range
		Dim oExcelRange As Excel.Range = Nothing '''''''''''= Application.Merge(oRect)
		'  zzDispRegion(oRect, "Merge-Rect")
		'Dim oValue=
		For iCol As Integer = oRect.Left To oRect.Right
			For iRow As Integer = oRect.Top To oRect.Bottom

			Next
		Next
		oRect.Location = New System.Drawing.Point(oRect.Location.X, oRect.Location.Y + Me.FirstRow)

		Try
			oExcelRange = Report.Merge(oRect)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "HeaderSection-zzMerge")
		End Try
		Return oExcelRange
	End Function
	Private Sub zzDispRegion(ByVal oRect As System.Drawing.Rectangle, ByVal sTitle As String)
		Dim sTest As String
		sTest = "Row: " & CStr(oRect.Top) & "-" & CStr(oRect.Bottom) & vbCrLf
		sTest &= "Col: " & CStr(oRect.Left) & "-" & CStr(oRect.Right)
		System.Windows.Forms.MessageBox.Show(sTest, sTitle)
	End Sub
End Class
