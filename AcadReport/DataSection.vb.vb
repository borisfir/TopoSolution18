Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Friend Class DataSection
   Inherits BaseDataSection

	' Private moDataTypeAAA(BaseReport.ColumnUB) As System.Type
	Private WithEvents moCurrentMergeColumn As DataMerge
   Private miRowLB As Integer
	'  Private miFirstDataRowAAA As Integer = 0
   ''''  Private miDataRowCount As Integer = -1
   Private mdHeaderHeight As Double
   Private mdFooterHeight As Double
   Private mdReportHeight As Double
   Private mdMaxTableHeight As Double
   Private mbRepContinue As Boolean = False
	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, ByVal oMainView As System.Data.DataView, ByVal bAreaMeter As Boolean, ByVal bMergeRows As Boolean, Optional ByVal iaDataColumns() As Integer = Nothing)
		MyBase.New(oResource, oMainView, False, bMergeRows, iaDataColumns)
		MyBase.diSectionType = enSectionType.Autocad Or enSectionType.Data
		mdMaxTableHeight = RepApp.MaxTableHeight
		dbRightToLeft = False
		dbAreaMeter = bAreaMeter
		'    DMAcadExt.AcadDocument.WriteDebugMessage("?????????????**+!! " & dbAreaMeter.ToString & "; " & bAreaMeter.ToString())
	End Sub

	Public ReadOnly Property RepContinue() As Boolean
		Get
			Return mbRepContinue
		End Get
	End Property

	Public ReadOnly Property SectionType() As enSectionType
		Get
			Return Me.diSectionType
		End Get
	End Property


	Public Overrides Sub Print()
		'Dim sTest As String

		mbRepContinue = False

		zzInsertRows()
		Try
			'	AcadReport.Report.AcadTable.Draw()
			'	DMAcadExt.AcadDocument.Regen()
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "DataSection - Print_2")
		End Try
		If Not mbRepContinue AndAlso (mdReportHeight + mdFooterHeight > mdMaxTableHeight) Then

			zzDeleteRow(diLastRow)
			mbRepContinue = True
			diFirstDataRow = diFirstDataRow + MyBase.diLastRow - MyBase.FirstRow
			MyBase.RowCount = diDataRowCount - diFirstDataRow

		End If


	End Sub

	
	Public Sub SetHeaderFooterHeight(ByVal dHeaderHeight As Double, ByVal dFooterHeight As Double)
		mdHeaderHeight = dHeaderHeight
		mdFooterHeight = dFooterHeight
	End Sub
	
	Private Sub zzInsertRows()
		Dim sTest As String
		Dim dRowHeight As Double
		mdReportHeight = mdHeaderHeight
		

		If MyBase.FirstRow = 0 Then
			'Report.AcadTable.SetRowHeight(0, ddaRowHeight(0) * Report.DrawingScaleFactor) obs
			RepApp.AcadTable.Rows(0).Height = ddaRowHeight(0) * RepApp.DrawingScaleFactor
			miRowLB = MyBase.FirstRow + 1
			zzFillRow(0)
			
		Else
			miRowLB = MyBase.FirstRow
		End If
		'System.Windows.Forms.MessageBox.Show(CStr(RowCount) & ":" & CStr(diDataRowCount) & ":" & CStr(miRowLB) & ":" & CStr(MyBase.diLastRow) & vbCrLf & CStr(RepApp.AcadTable.Rows.Count), "RowCount,miDataRowCount,miRowLB,diLastRow zzInsert 21_676")

		Try
			dRowHeight = ddaRowHeight(0) * RepApp.DrawingScaleFactor
			For iRowIndex As Integer = miRowLB To MyBase.diLastRow  '
				RepApp.AcadTable.InsertRows(iRowIndex, dRowHeight, 1)

				If True And iRowIndex = miRowLB Then
					Dim oCell As Autodesk.AutoCAD.DatabaseServices.Cell
					Dim oCellBorder As CellBorder
					For iCol As Integer = 0 To BaseReport.ColumnUB

						oCell = RepApp.AcadTable.Cells.Item(miRowLB, iCol)
						oCellBorder = oCell.Borders.Top

						If Not oCellBorder.IsVisible Then
							oCellBorder.IsVisible = True
						End If
					Next
				End If

				zzFillRow(iRowIndex)
				If mdReportHeight > mdMaxTableHeight Then
					sTest = mdReportHeight.ToString() & vbCrLf
					sTest &= mdMaxTableHeight.ToString()
					mbRepContinue = True
					zzDeleteRow(iRowIndex)
					diFirstDataRow = diFirstDataRow + iRowIndex - MyBase.FirstRow
					MyBase.RowCount = diDataRowCount - diFirstDataRow
					Exit For
				End If
			Next
			For iColIndex As Integer = 0 To BaseReport.ColumnUB
				If doaMergeColumns(iColIndex) IsNot Nothing Then
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "MergeColVal", iColIndex, doaMergeColumns(iColIndex).CurrentValue)
					moCurrentMergeColumn = doaMergeColumns(iColIndex)
					moCurrentMergeColumn.Close()
				End If
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Err:DataSection-InsertRows")
		End Try

	End Sub

	Private Sub zzFillRow(ByVal iRowIndex As Integer)
		Dim oTableCell As TableCell
		Dim oDataRowView As System.Data.DataRowView
		Dim iDataRowIndex As Integer = diFirstDataRow + iRowIndex - MyBase.FirstRow
      Dim sTextString As String
      Dim sMergeValue As String = Nothing

		Dim iDataColIndex As Integer
      Dim sTest As String = "a"
      Dim oTestVal As Object


		Try
			oDataRowView = doMainView.Item(iDataRowIndex)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "RowIndex=" & CStr(iRowIndex) & vbCrLf & "doMainView.Count=" & CStr(doMainView.Count), "DataSection - zzFillRow_1")
			Return
		End Try
      '  System.Windows.Forms.MessageBox.Show(MyBase.diCellTypeUB.ToString(), "09_989")
      '  DMCommon.ExcelLogD.SetNextValue(iERow, 0, "-", "-", "-", "-", "-", "-", "-")
      '   DMCommon.ExcelLogD.SetNextValue(iERow, 0)

      ' For iColIndex As Integer = 0 To BaseReport.ColumnUB
      For iColIndex As Integer = BaseReport.ColumnUB To 0 Step -1

         oTestVal = "*"
         Try
            sTest = "xb" & CStr(iColIndex)
            If MyBase.diCellTypeUB >= 0 Then
               oTableCell = MyBase.doaTableCell(iColIndex)
            Else
               oTableCell = MyBase.doTableCell
            End If
            'DMAcadExt.AcadDocument.WriteMessageLog(CStr(iDataRowIndex) & "," & CStr(iColIndex) & ": " & CStr(MyBase.diCellTypeUB) & " - " & oTableCell.Description)
            sTest = "xc" & CStr(iColIndex)
            ''''''''''''''''''''''''
            '		DMAcadExt.AcadDocument.WriteMessage("TextHeight=" & CStr(oTableCell.TextHeight) & "*" & CStr(Report.DrawingScaleFactor))
            '	Report.AcadTable.SetTextHeight(iRowIndex, iColIndex, oTableCell.TextHeight * Report.DrawingScaleFactor)
            sTest = "xd" & CStr(iColIndex)
            RepApp.SetAlignment(iRowIndex, iColIndex, oTableCell.TextAlignment)
            sTest = "xe" & CStr(iColIndex)
            Dim oCell As Autodesk.AutoCAD.DatabaseServices.Cell
            Dim oCellBorder As CellBorder

            If oTableCell.GridLinesExist Then
               sTest = "xf" & CStr(iColIndex)
               For iIndex As Integer = 0 To oTableCell.GridLines.GetUpperBound(0)
                  '	RepApp.AcadTable.SetGridLineWeight(iRowIndex, iColIndex, oTableCell.GridLines(iIndex).Type, oTableCell.GridLines(iIndex).Weight)
                  '		RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).Borders.
                  oCell = RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex)
                  oCellBorder = oCell.Borders.Bottom
                  '	oCellBorder.LineStyle = ieGridLineStyle
                  oCellBorder.LineWeight = LineWeight.LineWeight050
                  sTest = "xk" & CStr(iColIndex)
               Next

            End If
            If oTableCell.ContentColor IsNot Nothing AndAlso oTableCell.ContentColor <> Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 0S) Then
               'Report.AcadTable.SetContentColor(iRowIndex, iColIndex, oTableCell.ContentColor)	'obs	'oTableCell.ContentColor
               RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).ContentColor = oTableCell.ContentColor
            End If
            If oTableCell.BackgroundColor IsNot Nothing AndAlso oTableCell.BackgroundColor <> Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 0S) Then
               'Report.AcadTable.SetBackgroundColor(iRowIndex, iColIndex, oTableCell.BackgroundColor)  obs
               RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).BackgroundColor = oTableCell.BackgroundColor
            End If
            '			Report.AcadTable.SetMargin(iRowIndex, iColIndex, CellMargins.Left, 2.7)
            '			Report.AcadTable.SetMargin(iRowIndex, iColIndex, CellMargins.Right, 2.6)
            '			Report.AcadTable.SetMargin(iRowIndex, iColIndex, CellMargins.Top, 2.5)
            '			Report.AcadTable.SetMargin(iRowIndex, iColIndex, CellMargins.Bottom, 2.8)

            sTest = "xl" & CStr(iColIndex)
            If oTableCell.TextStyleIndex <> -1 Then
               sTest = "xm" & CStr(iColIndex)
            End If
            If oTableCell.TextHeight * RepApp.DrawingScaleFactor = 0.0 Then
               DMAcadExt.AcadDocument.WriteMessage(oTableCell.TextHeight.ToString() & ":" & CStr(RepApp.DrawingScaleFactor), "19_257a")
               oTableCell.TextHeight = 0.8
            End If
            sTest = "xn" & CStr(iColIndex)

            If oTableCell.TextStyleIndex >= 0 Then
               sTest = "xp" & CStr(iColIndex)
               RepApp.SetTextStyle(iRowIndex, iColIndex, oTableCell.TextStyleIndex, oTableCell.TextHeight * RepApp.DrawingScaleFactor)
            Else
               RepApp.SetTextStyle(iRowIndex, iColIndex, 0, oTableCell.TextHeight * RepApp.DrawingScaleFactor)
            End If
            sTest = "xq" & CStr(iColIndex)

            iDataColIndex = BaseReport.ColumnUB - iColIndex
            If dbDataColumnsExist Then
               iDataColIndex = diaDataColumns(iDataColIndex)
            End If
            sTextString = String.Empty

            Select Case oTableCell.Feature
               Case CellFeatures.Default
                  If IsDBNull(oDataRowView.Item(iDataColIndex)) Then
                     sTextString = String.Empty
                     sTest = "ba"
                     oTestVal = String.Empty
                     ''  MessageBox.Show("DBNull", oDataRowView.DataView.Table.Columns.Item(iDataColIndex).Caption)
                  Else
                     sTest = "bb"
                     sTextString = oTableCell.Format(oDataRowView.Item(iDataColIndex), False, dbAreaMeter)
                     oTestVal = oDataRowView.Item(iDataColIndex)
                     '' MessageBox.Show(CStr(oDataRowView.Item(iDataColIndex)), oDataRowView.DataView.Table.Columns.Item(iDataColIndex).Caption)
                     sTest = "bd"

                  End If
                  sTest = "c"
                  ''	Report.AcadTable.SetTextString(iRowIndex, iColIndex, sTextString) obs

                  RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).TextString = sTextString
               Case CellFeatures.BackgroundFill
                  sTextString = String.Empty
                  Dim oDMColor As DMAcadExt.DMColor
                  Try
                     Dim sTest1 As String
                     If Not IsDBNull(oDataRowView.Item(iDataColIndex)) Then
                        oDMColor = DirectCast(oDataRowView.Item(iDataColIndex), DMAcadExt.DMColor)
                        sTest1 = oDMColor.CommonString & ":" & CStr(oDMColor.AcadColor.ColorIndex)

                        '	Report.AcadTable.SetBackgroundColor(iRowIndex, iColIndex, oDMColor.AcadColor) ' obs
                        RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).BackgroundColor = oDMColor.AcadColor
                     End If
                  Catch oEx As Exception

                  End Try
               Case CellFeatures.Zebra
                  RepApp.AcadTable.RecomputeTableBlock(True)
                  Dim tColorScheme As DMAcadExt.ColorScheme
                  Dim iColor As Short = 2

                  sTextString = String.Empty
                  Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
                  'RepApp.AcadTable.GetCellExtents(iRowIndex, iColIndex, True, colPoints) - obsolete
                  colPoints = RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).GetExtents()

                  If Not IsDBNull(oDataRowView.Item(iDataColIndex)) Then
                     tColorScheme = DirectCast(oDataRowView.Item(iDataColIndex), DMAcadExt.ColorScheme)
                     '	System.Windows.Forms.MessageBox.Show(tColorScheme.ID_Name & vbCrLf & CStr(tColorScheme.IsInstance) & vbCrLf & CStr(tColorScheme.Scale), "01_881")
                     '	System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex) & vbCrLf & tColorScheme.ID_Name & vbCrLf & CStr(tColorScheme.IsInstance) & vbCrLf & CStr(tColorScheme.Scale), "01_881")
                     RepApp.AddColorSchemeCell(iRowIndex, iColIndex, tColorScheme)
                  End If

               Case Else
                  sTextString = String.Empty
            End Select
            '  DMCommon.ExcelLogD.SetValue(iERow, 5 * iColIndex + 1, iDataColIndex, oTableCell.Description, oTestVal, sTextString)
            sTest = "d"
				' uuuu3004()
				'DMCommon.Debug.MsgBox("13_341k DatasectioAC", iColIndex, sTextString)
				If doaMergeColumns(iColIndex) IsNot Nothing AndAlso sTextString.Length <> 0 Then  'AndAlso doaMergeColumns(iColIndex).IsMergeBase
               moCurrentMergeColumn = doaMergeColumns(iColIndex)
               If String.IsNullOrEmpty(sMergeValue) Then
                  sMergeValue = sTextString
               Else
                  sMergeValue &= "|" & sTextString
               End If
               moCurrentMergeColumn.SetValue(iRowIndex, sMergeValue, Nothing)

				End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(CStr(iRowIndex) & ":" & CStr(iColIndex), "Data Print 5379x")
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest, "DataSection - zzFillRow")
         End Try
      Next
      mdReportHeight += RepApp.AcadTable.Rows(iRowIndex).Height
   End Sub
	Private Sub zzDeleteRow(ByVal iRowIndex As Integer)
		Dim dRowHeight As Double
		Try
			dRowHeight = RepApp.AcadTable.Rows(iRowIndex).Height
			RepApp.AcadTable.DeleteRows(iRowIndex, 1)
			mdReportHeight -= dRowHeight
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - zzDeleteRow")
		End Try
	End Sub
	Sub zzTestDispPointsAAA(ByVal colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection, ByVal iTestColor As Integer)
		For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colPoints
			DMAcadExt.AcadTransaction.InsertPoint(tPoint, , iTestColor)
		Next

	End Sub




	Private Function zzToGridLineType(ByVal iGridLineType As Integer) As GridLineType
		If [Enum].IsDefined(GetType(GridLineType), iGridLineType) Then
			Return CType(iGridLineType, GridLineType)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzToLineWeight(ByVal iGridLineWeight As Integer) As LineWeight
		If [Enum].IsDefined(GetType(GridLineType), iGridLineWeight) Then
			Return CType(iGridLineWeight, LineWeight)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzToGridLineStyle(ByVal iGridLineStyle As Integer) As GridLineStyle
		If [Enum].IsDefined(GetType(GridLineStyle), iGridLineStyle) Then
			Return CType(iGridLineStyle, GridLineStyle)
		Else
			Return Nothing
		End If
	End Function
	Private Sub moCurrentMergeColumn_Merge(ByVal oRect As System.Drawing.Rectangle, ByVal iSubColindex As Integer, ByVal iGridLineWeight As Integer, ByVal iGridLineStyle As Integer, ByVal bWholeLine As Boolean) Handles moCurrentMergeColumn.Merge
		'	Dim oCellRange As CellRange = New CellRange(oRect.Top, oRect.Left, oRect.Bottom, oRect.Right)
		Dim oCellRange As CellRange = CellRange.Create(RepApp.AcadTable, oRect.Top, oRect.Left, oRect.Bottom, oRect.Right)
		'    Dim oTableRegion As TableRegion = New TableRegion(oRect.Top, oRect.Left, oRect.Bottom, oRect.Right)
		Dim iRow As Integer = oRect.Bottom
		Dim iColRight As Integer
		If bWholeLine Then
			iColRight = BaseReport.ColumnUB
		Else
			iColRight = oRect.Right
		End If

		Dim bGridLineWeightExists As Boolean = [Enum].IsDefined(GetType(LineWeight), iGridLineWeight)
		Dim bGridLineStyleExists As Boolean = [Enum].IsDefined(GetType(GridLineStyle), iGridLineStyle)

		Try
			RepApp.AcadTable.MergeCells(oCellRange)
		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage("Merge " & CStr(oRect.Top) & ":" & CStr(oRect.Left) & ":" & CStr(oRect.Bottom) & ":" & CStr(oRect.Right))
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Acad DataSection - moCurrentMergeColumn_Merge")
			System.Windows.Forms.MessageBox.Show(CStr(oRect.Top) & ":" & CStr(oRect.Left) & ":" & CStr(oRect.Bottom) & ":" & CStr(oRect.Right), "21_287")
		End Try
		'	DMAcadExt.AcadDocument.WriteMessage("@@ bGridLineWeightExists iGridLineWeight @@" & CStr(bGridLineWeightExists) & ":" & iGridLineWeight.ToString())


		If bGridLineStyleExists OrElse bGridLineWeightExists Then
			Try
				Dim ieGridLineStyle As GridLineStyle = CType(iGridLineStyle, GridLineStyle)
				Dim ieGridLineWeight As LineWeight = CType(iGridLineWeight, LineWeight)
				'	System.Windows.Forms.MessageBox.Show(ieGridLineStyle.ToString(), "02_378")
				Dim oCell As Autodesk.AutoCAD.DatabaseServices.Cell
				Dim oCellBorder As CellBorder
				For iCol As Integer = 0 To iColRight
					'	RepApp.AcadTable.SetGridLineStyle(iRow, iCol, GridLineType.HorizontalBottom, ieGridLineStyle)
					oCell = RepApp.AcadTable.Cells.Item(iRow, iCol)
					oCellBorder = oCell.Borders.Bottom
					oCellBorder.LineStyle = ieGridLineStyle
					oCellBorder.LineWeight = ieGridLineWeight
					oCellBorder.IsVisible = True
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataSection - moCurrentMergeColumn_Merge_2")
			End Try
		End If

	End Sub


End Class



