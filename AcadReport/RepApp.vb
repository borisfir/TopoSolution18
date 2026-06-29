Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices.Application
Imports Autodesk.AutoCAD.Geometry
Imports System.Windows.Forms
Public Class RepApp
	Const msScaleSysVarName As String = "USERR1"
	Const mdDrawingScaleDfltAAA As Double = 2500.0
    Const msLineDelim As String = "|*|"
    Public Enum enReportType
        [Default] = 1
        MultiTable = 2
    End Enum
	Public Shared AcadTable As Autodesk.AutoCAD.DatabaseServices.Table
	Private Shared mtAcadTableObjID As ObjectId
	' Public Shared ResourceTheme As TPlServerDB.enResourceTheme
	' Public Shared NumColumns As Integer
	' Public Shared ColumnUB As Integer


	Private Shared mdPageWidth As Double
	Private Shared mdPageMargin As Double = 40.0

	Private Shared moaPoints() As Autodesk.AutoCAD.Geometry.Point3d
	Private Shared mtTopRightPoint As Autodesk.AutoCAD.Geometry.Point3d
	Private Shared mtBasePoint As Autodesk.AutoCAD.Geometry.Point3d
	Private Shared mtTableBasePoint As Autodesk.AutoCAD.Geometry.Point3d

	Private Shared mdMaxReportHeight As Double = 1.0E+15
    Private Shared mdMinReportHeight As Double = 80.0
    Private Shared mdMMultiReportSpace As Double = 50.0


	Private Shared mdVerticalCellMargin As Double = -1.0
	Private Shared mdHorizontalCellMargin As Double = -1.0

	Private Shared miHideGridLineType As Integer
	Private Shared miHideGridRowType As Integer
    ' Private Shared mbIsMulti As Boolean
    Private Shared miReportType As enReportType

	''Private Shared mdMaxTableHeight As Double
	'' Private Shared mdMinTableHeight As Double = 80.0

	Private Shared mdTitleHeight As Double

	Private Shared moTextStyleTableRecordID As ObjectId
	Private Shared mtaTextStyleTableRecordID() As ObjectId

	Private Shared mtTableStyleRecordID As ObjectId

	Private Shared mdSubTitleHeight As Double

	Private Shared msTitleText As String = String.Empty
	Private Shared msSubTitleText As String = String.Empty

	Private Shared moTitleTableCell As TableCell = Nothing
	Private Shared moSubTitleTableCell As TableCell = Nothing


	Private Shared mdDrawingScaleFactor As Double = 1.0 '0.1
	Private Shared msHebAcadFontName As String = "HEBTEXT" '"oron"
	Private Shared msExcelFontName As ExcelFont '= "Tahoma"
	'Protected dtExcelFont As ExcelFont
	Private Shared msTableStyleName As String = "Standard" '''' "Legend" ''''''
	Private Shared msaTextStyles() As String = Nothing
	Private Shared miaTableTextStyles() As Integer = {-1, -1, -1, -1}
	Private Shared miaColorCells(,) As Integer
	Private Shared mtaColorScheme() As DMAcadExt.ColorScheme

	Public Shared Sub Init(bIsMultiTable As Boolean)

		'mbIsMulti = bIsMulti
		If bIsMultiTable Then
			miReportType = enReportType.MultiTable
		Else
			miReportType = enReportType.Default
		End If
		If BaseReport.ColWidths IsNot Nothing Then
			mdPageWidth = 0.0
			For iIndex As Integer = 0 To BaseReport.ColumnUB
				mdPageWidth += BaseReport.ColWidths(iIndex) * mdDrawingScaleFactor
			Next
		End If
		Dim sRes As String = String.Empty
		Try
			sRes = BaseReport.BaseResource.ResItems(2)
			mdMinReportHeight = CDbl(sRes) * mdDrawingScaleFactor
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sRes, "Report - Init_3")
		End Try
		Dim sResItem As String = String.Empty
		Try
			sResItem = BaseReport.BaseResource.ResItems(3)
			mdPageMargin = CDbl(sResItem) * mdDrawingScaleFactor
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sResItem, "Report - Init_4")
		End Try
		Try
			msaTextStyles = BaseReport.BaseResource.GetStrItem(4)
			'   DMCommon.Functions.DispArray(msaTextStyles, "textstyle-22_120", True)
			ReDim mtaTextStyleTableRecordID(msaTextStyles.GetUpperBound(0))
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(4), "Report - Init_5")
		End Try
		Try
			msExcelFontName = New ExcelFont(BaseReport.BaseResource.ResItems(5))
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(5), "Report - Init_6")
		End Try
		Try
			msTableStyleName = BaseReport.BaseResource.ResItems(6)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(6), "Report - Init_7")
		End Try
		Try
			Dim iaValues() As Integer
			iaValues = BaseReport.BaseResource.GetIntItem(7)
			If iaValues IsNot Nothing Then
				miaTableTextStyles = iaValues
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(7), "Report - Init_8")
		End Try

		Try
			Dim oValues() As Double
			oValues = BaseReport.BaseResource.GetDblItem(8)
			If oValues IsNot Nothing Then
				If oValues.GetUpperBound(0) >= 0 Then
					mdVerticalCellMargin = oValues(0)
					If oValues.GetUpperBound(0) >= 1 Then
						mdHorizontalCellMargin = oValues(1)
					End If
				End If
			End If
			'	System.Windows.Forms.MessageBox.Show(CStr(mdHorizontalCellMargin) & ":" & CStr(mdVerticalCellMargin), "26_570")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(8), "Report - Init_9")
		End Try

		Try
			miHideGridLineType = -1
			miHideGridRowType = -1
			Dim iaValues() As Integer
			iaValues = BaseReport.BaseResource.GetIntItem(9)
			If iaValues IsNot Nothing Then
				If iaValues.GetUpperBound(0) >= 0 Then
					miHideGridLineType = iaValues(0)
					If iaValues.GetUpperBound(0) >= 1 Then
						miHideGridRowType = iaValues(1)
					End If
				End If
			End If
			'	System.Windows.Forms.MessageBox.Show(CStr(mdHorizontalCellMargin) & ":" & CStr(mdVerticalCellMargin), "26_570")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & BaseReport.BaseResource.ResItems(8), "Report - Init_9")
		End Try

		Try
			If BaseReport.BaseResource.ItemsUB >= 10 Then
				sRes = BaseReport.BaseResource.ResItems(10)
				mdMMultiReportSpace = CDbl(sRes) * mdDrawingScaleFactor

			End If


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sRes, "Report - Init_10")
		End Try




		zzAddTableStyle()
		zzInitStyles()
	End Sub
	Public Shared Sub SetBreak(dBreakHeight As Double, dSpacing As Double)


		'None = 0
		'EnableBreaking = 1
		'RepeatTopLabels = 2
		'RepeatBottomLabels = 4
		'AllowManualPositions = 8
		'AllowManualHeights = 16

		'BreakOptions As TableBreakOptions
		'Public Overridable Property BreakFlowDirection A

		Dim tOffset As Vector3d = New Vector3d(10.0, 10.0, 0.0)
		DMCommon.Debug.MsgBox("dBreakHeightNew, dSpacing", dBreakHeight, dSpacing)
		AcadTable.BreakEnabled = True

		AcadTable.BreakFlowDirection = TableBreakFlowDirection.Left
		AcadTable.BreakOptions = CType(TableBreakOptions.AllowManualHeights + TableBreakOptions.AllowManualPositions + TableBreakOptions.RepeatTopLabels + TableBreakOptions.EnableBreaking, TableBreakOptions)
		AcadTable.SetBreakHeight(0, dBreakHeight)
		AcadTable.SetBreakSpacing(dSpacing)
		'	AcadTable.SetBreakOffset(1, tOffset)

	End Sub
	Public Shared Sub GetStartPoint()
      Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
      Dim saPrompt(1) As String
      '  Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock

      '''''''''''''''''''''''''	DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      saPrompt(0) = "Enter start point of the report"
      saPrompt(1) = vbCrLf & "Enter end point of the report"

      oaPoints = DMAcadExt.AcadUtil.GetTwoPoints(saPrompt)
      If oaPoints IsNot Nothing Then
         RepApp.SetPoints(oaPoints)
      End If
   End Sub
    Public Shared Sub NextTable()
		AcadTable.RecomputeTableBlock(False)
		Dim dNewY As Double = AcadTable.Position.Y - mdMMultiReportSpace - AcadTable.Height
        Dim taPoints(moaPoints.GetUpperBound(0)) As Point3d
        For iIndex As Integer = 0 To moaPoints.GetUpperBound(0)
            taPoints(iIndex) = New Point3d(moaPoints(iIndex).X, dNewY, 0.0)
        Next
        SetPoints(taPoints)
    End Sub

    Public Shared Sub ReDrawTable()
        Try
            AcadTable.Draw()
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - ReDrawTable ")
        End Try
    End Sub
    Public Shared ReadOnly Property TitleHeight() As Double
        Get
            Return mdTitleHeight * mdDrawingScaleFactor
        End Get

    End Property
    Public Shared ReadOnly Property ExcelFontName() As ExcelFont
        Get
            Return msExcelFontName
        End Get
    End Property

    Public Shared ReadOnly Property TableBasePoint() As Autodesk.AutoCAD.Geometry.Point3d
        Get
            Return mtTableBasePoint
        End Get
    End Property
    Public Shared ReadOnly Property DrawingScaleFactor() As Double
        Get
            Return mdDrawingScaleFactor
        End Get

    End Property
    Public Shared Function GetColorScheme() As DMAcadExt.ColorScheme()
        Return mtaColorScheme
    End Function
    Public Shared Function GetColorCells() As Autodesk.AutoCAD.Geometry.Point3dCollection()
        Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection
        Dim iCellUB As Integer
        Dim iRow, iCol As Integer
        If miaColorCells IsNot Nothing Then
            Try
                iCellUB = miaColorCells.GetUpperBound(1)
                Dim colaPoints(iCellUB) As Autodesk.AutoCAD.Geometry.Point3dCollection

                For iCellIndex As Integer = 0 To iCellUB
                    colPoints = New Autodesk.AutoCAD.Geometry.Point3dCollection()
                    iRow = miaColorCells(0, iCellIndex)
                    iCol = miaColorCells(1, iCellIndex)
                    '	RepApp.AcadTable.GetCellExtents(iRow, iCol, False, colPoints)
                    '	DMAcadExt.AcadDocument.WriteMessage("!@! " & CStr(iCellIndex) & ": " & CStr(iRow) & "-" & CStr(iCol))
                    colPoints = RepApp.AcadTable.Cells.Item(iRow, iCol).GetExtents()
                    colaPoints(iCellIndex) = colPoints
                Next
                Return colaPoints
            Catch oEx As Exception
                DMCommon.Functions.ShowEx(oEx, "Report - GetColorCells", "Row=" & CStr(iRow) & ", Col=" & CStr(iCol))
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
    End Function
    Public Shared ReadOnly Property TopRightPoint() As Autodesk.AutoCAD.Geometry.Point3d
        Get
            Return mtTopRightPoint
        End Get
    End Property
    Public Shared ReadOnly Property BasePoint() As Autodesk.AutoCAD.Geometry.Point3d
        Get
            Return mtBasePoint
        End Get
    End Property
   Private Shared Sub zzInitStyles()

      '  DMAcadExt.AcadDocument.WriteMessage("-----+-----+----zzInitStyles")
      '	Dim oTableStyle As TableStyle = DMAcadExt.AcadTransaction.GetTableStyle(msTableStyleName)
      msTableStyleName = DMAcadExt.DMApp.GetDefaultTableStyleName()
      Dim oTableStyle As TableStyle = Nothing
      If DMAcadExt.DMApp.GetDataGridLineHidden() Then

         oTableStyle = DMAcadExt.AcadTransaction.GetTableStyle(msTableStyleName, OpenMode.ForWrite)
         oTableStyle.SetGridVisibility(True, 7, 1)


         If miHideGridLineType > 0 Then
            oTableStyle.SetGridVisibility(False, miHideGridLineType, miHideGridRowType) ' 1  4
         End If


      ElseIf Not String.IsNullOrEmpty(msTableStyleName) Then
         oTableStyle = DMAcadExt.AcadTransaction.GetTableStyle(msTableStyleName, OpenMode.ForRead)
         '  oTableStyle.IsTitleSuppressed = True
         '  DMAcadExt.AcadDocument.WriteMessage("IsTitleSuppressed: " & CStr(oTableStyle.IsTitleSuppressed))
      End If
      If oTableStyle IsNot Nothing Then
         mtTableStyleRecordID = oTableStyle.ObjectId '   DMAcadExt.AcadTransaction.GetTableStyleID(msTableStyleName)
      End If

      Dim sTest As String = ""
      For iIndex As Integer = 0 To msaTextStyles.GetUpperBound(0)
         Try
            mtaTextStyleTableRecordID(iIndex) = DMAcadExt.AcadTransaction.GetTextStyle(msaTextStyles(iIndex))
            sTest &= mtaTextStyleTableRecordID(iIndex).ToString & "|"
            '	oTableStyle.SetTextStyle()
            'moaTextStyleTableRecordID(iIndex).T()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & msaTextStyles(iIndex), "Report - zzInitStyles_1")
         End Try
      Next

   End Sub
   Public Shared Sub SetTextStyle(ByVal iRowIndex As Integer, ByVal iColIndex As Integer, ByVal iTextStyleIndex As Integer, ByVal dHeight As Double)

      Dim sTest As String = "!"
      Try

         If AcadTable Is Nothing Then
            System.Windows.Forms.MessageBox.Show("OOOOOO", "AcadTable Is Nothing")
         End If
         sTest = "a"
         If mtaTextStyleTableRecordID IsNot Nothing AndAlso mtaTextStyleTableRecordID.GetUpperBound(0) >= iTextStyleIndex Then
            Dim tAcObjID As ObjectId = mtaTextStyleTableRecordID(iTextStyleIndex)
            '   DMAcadExt.AcadDocument.WriteMessage("TextStyle " & tAcObjID.ToString() & "," & CStr(iRowIndex) & ":" & CStr(iColIndex) & "," & CStr(iTextStyleIndex))
            sTest = "b"
            If Not tAcObjID.IsNull Then
               sTest = "c"
               'AcadTable.SetTextStyle(iRowIndex, iColIndex, tAcObjID) obs

               AcadTable.Cells.Item(iRowIndex, iColIndex).TextStyleId = tAcObjID
               sTest = "d"
            End If
         End If

         sTest = "e"
         'AcadTable.SetTextHeight(iRowIndex, iColIndex, dHeight) obs
         If dHeight <> 0.0 Then
            sTest = "f"
            AcadTable.Cells.Item(iRowIndex, iColIndex).TextHeight = dHeight
            sTest = "g"
         End If
         sTest = "p"

      Catch oEx As Exception
         Dim sMsg As String = ""
         If IsArray(mtaTextStyleTableRecordID) Then
            sMsg = CStr(mtaTextStyleTableRecordID.GetUpperBound(0))
         End If
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & dHeight & vbCrLf & sTest, "Report - SetTextStyle_" & CStr(iRowIndex) & ":" & CStr(iColIndex) & ":" & CStr(iTextStyleIndex) & ":" & sMsg)
      End Try

   End Sub
    Public Shared Sub SetTextStyle(ByVal iRowIndex As Integer, ByVal iTextStyleIndex As Integer, ByVal dHeight As Double)
        Try
            For iColIndex As Integer = 0 To BaseReport.ColumnUB
                '	AcadTable.SetTextStyle(iRowIndex, iColIndex, mtaTextStyleTableRecordID(iTextStyleIndex)) obs
                '	AcadTable.SetTextHeight(iRowIndex, iColIndex, dHeight)   obs

                AcadTable.Cells.Item(iRowIndex, iColIndex).TextStyleId = mtaTextStyleTableRecordID(iTextStyleIndex)
                AcadTable.Cells.Item(iRowIndex, iColIndex).TextHeight = dHeight

            Next

        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "Report_2 - SetTextStyle_" & CStr(iRowIndex))
        End Try

    End Sub
    Public Shared Sub SetAlignment(ByVal iRowIndex As Integer, ByVal iColIndex As Integer, ByVal iCellAlignment As CellAlignment)
        Try
            '	AcadTable.SetAlignment(iRowIndex, iColIndex, iCellAlignment) obs
            If CType(iCellAlignment, Integer) <> 0 Then
                AcadTable.Cells.Item(iRowIndex, iColIndex).Alignment = iCellAlignment
            End If

        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - SetAlignment_" & CStr(iRowIndex) & ":" & CStr(iColIndex) & ":" & iCellAlignment.ToString() & ":" & CStr(mtaTextStyleTableRecordID.GetUpperBound(0)))
        End Try

    End Sub
    Public Shared Sub SetPoints(ByVal taPoints() As Autodesk.AutoCAD.Geometry.Point3d)

        moaPoints = taPoints
        mtTopRightPoint = taPoints(0)
        ' DMAcadExt.AcadDocument.WriteMessage("mtTopRightPoint=" & CStr(mtTopRightPoint.X) & "," & CStr(mtTopRightPoint.Y))
        mtBasePoint = New Autodesk.AutoCAD.Geometry.Point3d(mtTopRightPoint.X - mdPageWidth, mtTopRightPoint.Y, mtTopRightPoint.Z)

        Dim oBottomPoint As Autodesk.AutoCAD.Geometry.Point3d = taPoints(1)

        mdMaxReportHeight = mtTopRightPoint.Y - oBottomPoint.Y

        If mdMaxReportHeight < mdMinReportHeight Then
            mdMaxReportHeight = mdMinReportHeight
        End If
        '	MessageBox.Show(CStr(mdMinReportHeight) & ":" & CStr(mdMaxReportHeight), "12_437 mdMinReportHeight:mdMaxReportHeight")
    End Sub

    Public Shared ReadOnly Property HasStartPoint As Boolean
        Get
            Return moaPoints IsNot Nothing
        End Get
    End Property
    Public Shared ReadOnly Property ReportType As enReportType
        Get
            Return miReportType
        End Get
    End Property

    Public Shared Sub CloseTable()
        If AcadTable IsNot Nothing Then
            AcadTable = Nothing
        End If

    End Sub
    Public Shared Sub NewTable()
        Dim iColumnLB As Integer = 0
        '    Dim iRowLB As Integer = 0

        Try
            CloseTable()
            '	Dim oAcadTable As Table
            AcadTable = New Autodesk.AutoCAD.DatabaseServices.Table()

            'MessageBox.Show(mtTableStyleRecordID.ToString(), "19_091")
            AcadTable.TableStyle = mtTableStyleRecordID

         '  AcadTable.ClearTableStyleOverrides(TableStyleOverride.TitleSuppressed)
            ''''''''''''''''''''''	AcadTable.NumColumns = BaseReport.ColumnUB + 1



            '	AcadTable.Draw()
            '''''''''''	AcadTable.BreakEnabled = True
            '	 Report.AcadTable.UnmergeCells(New CellRange(0, 0, 0, Report.AcadTable.NumColumns - 1))
            '''''''''''''''''		AcadTable.BreakFlowDirection = TableBreakFlowDirection.Left
            Try
                '''''''''''''''	AcadTable.SetBreakHeight(1, 1200)
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_2a")
            End Try
            AcadTable.SetBreakSpacing(40)

            Try
                'AcadTable.SetCellStyle(0, 0, "Header")
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g1")
            End Try
            Try
                'AcadTable.SetCellStyle(1, 0, "Header")
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g2")
            End Try
            Try
                'AcadTable.SetCellStyle(2, 0, "Header")
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g3")
            End Try


            Try
                AcadTable.Position = mtTableBasePoint
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_1")
            End Try

        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_2")
        End Try
        '''''''''''''''''''''Temp
        '	System.Windows.Forms.MessageBox.Show(CStr(moaTextStyleTableRecordID Is Nothing), "18_531")
        'If Not moaTextStyleTableRecordID(0).IsNull Then

        'AcadTable.SetTextStyle(moaTextStyleTableRecordID(0), RowType.UnknownRow)
        'AcadTable.SetTextStyle(moaTextStyleTableRecordID(0), RowType.DataRow)
        'AcadTable.SetTextStyle(moaTextStyleTableRecordID(0), RowType.TitleRow)
        'AcadTable.SetTextStyle(moaTextStyleTableRecordID(0), RowType.HeaderRow)
        '	End If


        '  Else
        '  System.Windows.Forms.MessageBox.Show("TextStyle was not found", "Report - NewTable_3")
        '	End If
        ''''''''''''''''''''''
        '	MessageBox.Show(CStr(BaseReport.ColWidths(0)) & ":" & CStr(mdDrawingScaleFactor) & "  NewTable", "19_520")
        Try
            If RepApp.AcadTable.Columns.Count = 1 Then
                '	Report.AcadTable.SetColumnWidth(0, BaseReport.ColWidths(0) * mdDrawingScaleFactor)
                RepApp.AcadTable.Columns.Item(0).Width = BaseReport.ColWidths(0) * mdDrawingScaleFactor
                iColumnLB = 1
            End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!NewTable", RepApp.AcadTable.Columns.Count, iColumnLB, BaseReport.ColumnUB, mdDrawingScaleFactor)
			For iColIndex As Integer = iColumnLB To BaseReport.ColumnUB

				'''''''	Report.AcadTable.SetColumnWidth(iColIndex, BaseReport.ColWidths(iColIndex) * mdDrawingScaleFactor)
				RepApp.AcadTable.InsertColumns(iColIndex, BaseReport.ColWidths(iColIndex) * mdDrawingScaleFactor, 1)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Column ", iColIndex, BaseReport.ColWidths(iColIndex), RepApp.AcadTable.Columns.Count)
			Next
			'Debug Excel


			'	System.Windows.Forms.MessageBox.Show(CStr(Report.AcadTable.NumColumns), "19_598 NumColumns")
			RepApp.AcadTable.UnmergeCells(CellRange.Create(RepApp.AcadTable, 0, 0, 0, RepApp.AcadTable.Columns.Count - 1))

            '	System.Windows.Forms.MessageBox.Show(CStr(Report.AcadTable.NumColumns), "19_599 NumColumns")
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Report - NewTable_4")
        End Try

    End Sub

    Public Shared ReadOnly Property MaxTableHeight() As Double
        Get
            Return mdMaxReportHeight - TitleHeight
        End Get
    End Property
    Public Shared Sub CalcBasePoint(ByVal dTitleHeight As Double)
        mdTitleHeight = dTitleHeight
        mtTableBasePoint = New Point3d(mtBasePoint.X, mtBasePoint.Y - dTitleHeight, 0)
    End Sub
    Public Shared Sub EraseTable()
        AcadTable.Erase()
        AcadTable = Nothing
    End Sub
    Public Shared Sub NextPage()
        mtTableBasePoint = New Autodesk.AutoCAD.Geometry.Point3d(mtTableBasePoint.X - mdPageWidth - mdPageMargin, mtTableBasePoint.Y, mtTableBasePoint.Z)
    End Sub

	Public Shared Sub ReopenAcadTable()
		AcadTable = DMAcadExt.AcadTransaction.GetAcadTable(mtAcadTableObjID, OpenMode.ForWrite)
	End Sub


	Public Shared Sub DrawTable()
        Dim sTest As String = "a"
        Dim oTextStyleTableRecordID As ObjectId
        Dim iTextStyleIndex As Integer
        Dim iRowType As RowType




		'MessageBox.Show(AcadTable.TableStyle.ToString(), "17_111")
		mtAcadTableObjID = DMAcadExt.AcadTransaction.AppendEntity(AcadTable)
		sTest = "b"
        Try
            Try
                ''''''''''''''	Report.AcadTable.Draw()
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - DrawTable_1")
            End Try

            '	DMAcadExt.AcadTransaction.AppendEntity(AcadTable)
            '	MessageBox.Show("After AppendEntity", "17_114")
            '	System.Windows.Forms.MessageBox.Show(CStr(miaTableTextStyles.GetUpperBound(0)), "12_240 miaTableTextStyles.GetUpperBound")
            sTest = "c"
            For iRowTypeIndex As Integer = 0 To miaTableTextStyles.GetUpperBound(0)
                sTest = "d_" & CStr(iRowTypeIndex)
                If [Enum].IsDefined(GetType(RowType), iRowTypeIndex) Then
                    iRowType = CType(iRowTypeIndex, RowType)

                    '	System.Windows.Forms.MessageBox.Show(iRowType.ToString(), "12_241 iRowType.ToString")
                    iTextStyleIndex = miaTableTextStyles(iRowTypeIndex)
                    sTest = "e_" & CStr(iRowTypeIndex)
                    If iTextStyleIndex >= 0 Then
                        sTest = "g_" & CStr(iRowTypeIndex)
                        oTextStyleTableRecordID = mtaTextStyleTableRecordID(iTextStyleIndex)
                        sTest = "f_" & CStr(iRowTypeIndex)
                        If Not oTextStyleTableRecordID.IsNull Then
                            'System.Windows.Forms.MessageBox.Show(oTextStyleTableRecordID.ToString(), "12_242 iRowType.ToString")
                            'AcadTable.SetTextStyle(oTextStyleTableRecordID, iRowType)
                        End If
                    End If
                End If
            Next
            sTest = "s"
            '	System.Windows.Forms.MessageBox.Show(mdDrawingScaleFactor.ToString(), "23_220 mdDrawingScaleFactor") 'April
            '	System.Windows.Forms.MessageBox.Show(CStr(AcadTable.Cells.Borders.Vertical.Margin) & ":" & CStr(AcadTable.Cells.Borders.Horizontal.Margin), "01_171obs")	'April
            '	System.Windows.Forms.MessageBox.Show(CStr(AcadTable.VerticalCellMargin) & ":" & CStr(AcadTable.HorizontalCellMargin), "01_171new")	'April

            If mdVerticalCellMargin >= 0.0 Then
                AcadTable.VerticalCellMargin = mdVerticalCellMargin * mdDrawingScaleFactor  'obs

				''''	AcadTable.Cells.Borders.Vertical.Margin = mdVerticalCellMargin * mdDrawingScaleFactor 


			End If
            If mdHorizontalCellMargin >= 0.0 Then

				AcadTable.HorizontalCellMargin = mdHorizontalCellMargin * mdDrawingScaleFactor  'obs
				'	System.Windows.Forms.MessageBox.Show(CStr(AcadTable.HorizontalCellMargin) & vbCrLf & CStr(mdHorizontalCellMargin * mdDrawingScaleFactor), "01_381  ")	  'April
				''''	AcadTable.Cells.Borders.Horizontal.Margin = mdHorizontalCellMargin * mdDrawingScaleFactor

			End If


		Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - DrawTable")
        End Try
    End Sub
    Private Shared Sub zzAddTableStyle()
        Dim sTableStylName As String = DMAcadExt.DMApp.GetDefaultTableStyleName()
        If Not String.IsNullOrEmpty(sTableStylName) Then
            mtTableStyleRecordID = DMAcadExt.AcadTransaction.AddTableStyle(sTableStylName)
        End If

    End Sub
    Private Shared Sub zzTestExtents(ByVal iRowIndex As Integer, ByVal iColIndex As Integer)
        Dim oColor As Autodesk.AutoCAD.Colors.Color
        Dim iColor As Short = 2
        oColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, iColor)
        'Report.AcadTable.SetBackgroundColor(iRowIndex, iColIndex, oColor)
        RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).BackgroundColor = oColor
        Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection '= New Autodesk.AutoCAD.Geometry.Point3dCollection()
        colPoints = RepApp.AcadTable.Cells.Item(iRowIndex, iColIndex).GetExtents()
        Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
        MessageBox.Show(CStr(colPoints.Count), "12_956c colPoints.Count")
        For iIndex As Integer = 0 To colPoints.Count - 1
            tPoint = colPoints.Item(iIndex)
            DMAcadExt.AcadDocument.WriteMessage(CStr(tPoint.X) & "," & CStr(tPoint.Y) & vbNewLine)
        Next

    End Sub
    Public Shared Function zzDispPoint(ByVal oPoint As Autodesk.AutoCAD.Geometry.Point3d) As String
        Return oPoint.X & "," & oPoint.Y
    End Function
    Public Shared Sub NewTableTest(ByRef oAcadTable As Autodesk.AutoCAD.DatabaseServices.Table)
        Dim iColumnLB As Integer = 0
        '    Dim iRowLB As Integer = 0

        Try

            oAcadTable.BreakEnabled = True
            oAcadTable.TableStyle = mtTableStyleRecordID
            oAcadTable.BreakFlowDirection = TableBreakFlowDirection.Left

            Try
                oAcadTable.SetBreakHeight(1, 1200)
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_2a")
            End Try

            oAcadTable.SetBreakSpacing(40)

            Try
                MessageBox.Show(oAcadTable.TableStyleName, "13_002ccccc")
            Catch oEx As Exception
                MessageBox.Show(oEx.Message, "Report - NewTable_002xccccccccccc")
            End Try

            Try
                'oAcadTable.SetCellStyle(0, 0, "Header")
                oAcadTable.Cells.Item(0, 0).Style = "Header"
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g1")
            End Try
            Try
                ''''''''''''	oAcadTable.SetCellStyle(1, 0, "Header")
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g2")
            End Try
            Try
                '''''''''''''	oAcadTable.SetCellStyle(2, 0, "Header")
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_12g3")
            End Try


            Try
                oAcadTable.Position = mtTableBasePoint
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_1")
            End Try
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_2")
        End Try
        '''''''''''''''''''''Temp
        '   If moTextStyleTableRecordID.OldId <> 0 Then
        '   oAcadTable.SetTextStyle(moTextStyleTableRecordID, RowType.UnknownRow)
        '   oAcadTable.SetTextStyle(moTextStyleTableRecordID, RowType.DataRow)
        '   oAcadTable.SetTextStyle(moTextStyleTableRecordID, RowType.TitleRow)
        '   oAcadTable.SetTextStyle(moTextStyleTableRecordID, RowType.HeaderRow)

        '  Else
        '  System.Windows.Forms.MessageBox.Show("TextStyle was not found", "Report - NewTable_3")
        '   End If
        ''''''''''''''''''''''

        Try
            If oAcadTable.Columns.Count = 1 Then
                oAcadTable.Columns(0).Width = BaseReport.ColWidths(0) * mdDrawingScaleFactor
                iColumnLB = 1
            End If
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2", iCurrentPropID, iPropertyType, -1, iCurrentPolygonColor)
			For iColIndex As Integer = iColumnLB To BaseReport.ColumnUB
                oAcadTable.InsertColumns(iColIndex, BaseReport.ColWidths(iColIndex) * mdDrawingScaleFactor, 1)
            Next

            oAcadTable.UnmergeCells(CellRange.Create(RepApp.AcadTable, 0, 0, 0, RepApp.AcadTable.Columns.Count - 1))
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Report - NewTable_4")
        End Try
    End Sub
    Public Shared Sub DelEmptyColumns(iaCols() As Integer)
        For iIndex As Integer = 0 To iaCols.GetUpperBound(0)

            Try
                '	MessageBox.Show(CStr(iaCols(iIndex)), "01_422")
                AcadTable.DeleteColumns(iaCols(iIndex), 1)
            Catch oEx As Exception

            End Try

        Next

    End Sub


    Public Shared Sub AddColorSchemeCell(ByVal iRow As Integer, ByVal iColumn As Integer, ByVal tColorScheme As DMAcadExt.ColorScheme)
        Dim iUB As Integer = -1
        If miaColorCells IsNot Nothing Then
            iUB = miaColorCells.GetUpperBound(1)
        End If
        iUB += 1
        ReDim Preserve miaColorCells(1, iUB)
        ReDim Preserve mtaColorScheme(iUB)
        miaColorCells(0, iUB) = iRow
        miaColorCells(1, iUB) = iColumn
        mtaColorScheme(iUB) = tColorScheme
    End Sub
    Public Shared Sub ClearZebraCells()
        If miaColorCells IsNot Nothing Then
            Erase miaColorCells
        End If

    End Sub
    Public Shared Sub PaintCells()
        MessageBox.Show("", "02_600")
        If miaColorCells IsNot Nothing Then
            Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection
            Dim oPgon As TopoManager.SimplePgon
            Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
            'AcadReport.RepApp.InitDWGScaleFactor()
            For iCellIndex As Integer = 0 To miaColorCells.GetUpperBound(1)
                colPoints = New Autodesk.AutoCAD.Geometry.Point3dCollection()
                'NotErase	AcadTable.GetCellExtents(miaColorCells(0, iCellIndex), miaColorCells(1, iCellIndex), True, colPoints)	' obsolete
                colPoints = AcadTable.Cells.Item(miaColorCells(0, iCellIndex), miaColorCells(1, iCellIndex)).GetExtents()

                '	colPoints=
                colPoints = AcadTable.Cells.Item(miaColorCells(0, iCellIndex), miaColorCells(1, iCellIndex)).GetExtents()
                Dim sTest As String = ""
                If colPoints IsNot Nothing Then
                    oPgon = New TopoManager.SimplePgon(colPoints)
                    oPgon.SetTagNum("Table", iCellIndex)
                    Try
                        sTest = CStr(mtaColorScheme(iCellIndex).Zebra.Strip(0).Width)
                    Catch ex As Exception
                        sTest = "Err1329"
                    End Try


                    mtaColorScheme(iCellIndex).Scale = 0.05 * AcadReport.RepApp.DrawingScaleFactor
                    Try
                        sTest &= vbCrLf & CStr(mtaColorScheme(iCellIndex).Zebra.Strip(0).Width)
                    Catch ex As Exception
                        sTest &= vbCrLf & "Err1338"
                    End Try
                    MessageBox.Show(sTest, "02_671")
                    oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, mtaColorScheme(iCellIndex), True)
                    '	oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme Or DMAcadExt.PaintMethod.ZebraByTopo Or DMAcadExt.PaintMethod.BorderByBuffer, mtaColorScheme(iCellIndex), True)

                End If
                For iIndex As Integer = 0 To colPoints.Count - 1
                    tPoint = colPoints.Item(iIndex)
                    '	DMAcadExt.AcadDocument.WriteMessage(CStr(tPoint.X) & "," & CStr(tPoint.Y) & vbNewLine)
                Next
            Next
        End If
    End Sub
    Public Shared Function ToMText(ByVal sValue As String) As String
        Try
            Dim sRes As String = String.Empty
            Dim saPart() As String = Strings.Split(sValue, "€")
            For iIndex As Integer = 0 To saPart.GetUpperBound(0)
                If iIndex = 0 Then
                    sRes = zzToMText(saPart(0))
                Else
                    sRes &= zzToMText("€") & zzToMText(saPart(iIndex))
                End If
            Next
            '    Return "{\f" & msHebAcadFontName & "|b0|i0|c238|p34;" & sValue & "}"
            Return sRes
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "ToMText")
            Return String.Empty
        End Try
    End Function
    Public Shared Function zzToMText(ByVal sValue As String) As String
        Try
            '''''02/2008  Autocad2007 Return "{\f" & msHebAcadFontName & "|b0|i0|c238|p34;" & sValue & "}"
            Return sValue
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "ToMText")
            Return String.Empty
        End Try
    End Function

    Public Shared Sub InitDWGScaleFactor()
        Try
            Dim dDWGScale As System.Double = CDbl(GetSystemVariable(msScaleSysVarName))
            Dim dDrawingScaleFactorBefore As Double = mdDrawingScaleFactor
            If dDWGScale >= 10.0 Then
                mdDrawingScaleFactor = dDWGScale / DMAcadExt.DMApp.GetDefaultRepScale()
            End If

        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "InitDWGScaleFactor")
        End Try
    End Sub

    Public Sub New()
    End Sub

    '150046
End Class
Public Structure ExcelFont
	Dim Name As String
	Dim DefaultSize As Boolean
	Dim Size As Double
	Dim Bold As Boolean
	Dim Exists As Boolean
	Public Sub New(ByVal sResourceDescr As String)
		Dim saDescription() As String = Strings.Split$(sResourceDescr)
		Dim iUB As Integer = saDescription.GetUpperBound(0)

		If iUB >= 0 Then
			Name = saDescription(0)
		Else
			Return
		End If
		If iUB >= 1 Then
			Try
				Size = Convert.ToDouble(saDescription(1))
			Catch ex As Exception
			End Try
		Else
			DefaultSize = True
			Return
		End If
		If iUB >= 2 Then
			Select Case saDescription(2)
				Case "B", "b"
					Bold = True
			End Select
		Else
			Return
		End If
	End Sub

End Structure