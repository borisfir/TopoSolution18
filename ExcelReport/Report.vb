Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop
'Imports TopoManager
Public Enum TplnReportID
   LotsM = 1
   Parcels = 11
End Enum

Public Class Report
    Inherits AcadReport.BaseReport


    Const mdK1 As Double = 0.2
    Const mdK2 As Double = 0.5
    Private Shared moExcelApp As Excel.Application
    Friend Shared moWorkbook As Excel.Workbook
    Friend Shared moWorksheet As Excel.Worksheet
    Public Shared RightToLeft As Boolean = True
    Public Shared Reverse As Boolean
    Private moSheetRange As Excel.Range



	Private Shared miResourceTheme As TPlServerDB.enResourceTheme
    Private Shared moResource As TPlServerDB.TPlResource

    Private miReportID As TplnReportID
    Private msTitle As String
    Private msColHeaders() As String
    Private mdaColWidths() As Double
    Private miColumnUB As Integer
    Private miCurrentRow As Integer = 0
    Private mdDataRowHeight As Double = 5
    Private mdDataTextHeight As Double = 4
    Private miHeaderRowCount As Integer
    '  Private moBasePoint As Point3d
    Private mbReverse As Boolean
	Private mdReportHeight As Double





	Public Sub New(ByVal bReverse As Boolean)
        MyBase.dbAcadModel = False
        mbReverse = bReverse
        '	mbReverse = True
    End Sub
    Public Shared Function GetCellAddress(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As String
        If iColIndex <= 25 Then
            Return Chr(65 + iColIndex) & CStr(iRowIndex + 1)
        Else
            iColIndex = iColIndex - 26
            Return Chr(65 + iColIndex \ 26) & Chr(65 + iColIndex Mod 26) & CStr(iRowIndex + 1)
        End If
    End Function
    Public Shared Function GetRange(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As Excel.Range
		Dim oRange As Excel.Range = Nothing
		If moWorksheet IsNot Nothing Then
			Dim sCell As String = GetCellAddress(iRowIndex, iColIndex)
			Try
				oRange = moWorksheet.Range(sCell)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sCell & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColIndex), "Report - GetRange_1")
			End Try
		End If
		Return oRange
    End Function
    Public Shared Function GetRange(ByVal iRowIndexMin As Integer, ByVal iColIndexMin As Integer, ByVal iRowIndexMax As Integer, ByVal iColIndexMax As Integer) As Excel.Range
        Dim oRange As Excel.Range = Nothing
        Dim sCellMin As String = GetCellAddress(iRowIndexMin, iColIndexMin)
        Dim sCellMax As String = GetCellAddress(iRowIndexMax, iColIndexMax)
        Try
            oRange = moWorksheet.Range(sCellMin, sCellMax)
        Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iRowIndexMin) & "," & CStr(iColIndexMin) & ":" & CStr(iColIndexMin) & "," & CStr(iColIndexMax), "Application - GetRange_2")
        End Try
        Return oRange
    End Function
    Public Shared Function GetInputCell(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As Excel.Range
        Dim oRange As Excel.Range = GetRange(iRowIndex, iColIndex)
        If oRange IsNot Nothing Then
            Dim oMergeRange As Excel.Range = oRange.MergeArea()
            If (oMergeRange.Row = iRowIndex + 1) AndAlso (oMergeRange.Column = iColIndex + 1) Then
                Return oMergeRange
            End If
        End If
        Return Nothing
    End Function
    Public Shared Function Merge(ByVal iFirstRowIndex As Integer, ByVal iLastRowIndex As Integer, ByVal iFirstColIndex As Integer, ByVal iLastColIndex As Integer) As Excel.Range
        Dim oRange As Excel.Range = Nothing
        Dim sTopLeftCell As String = GetCellAddress(iFirstRowIndex, iFirstColIndex)
        Dim sBottomRightCell As String = GetCellAddress(iLastRowIndex, iLastColIndex)
        Dim sTest As String = sTopLeftCell & ":" & sBottomRightCell
        Try
            oRange = moWorksheet.Range(sTopLeftCell, sBottomRightCell)
            oRange.Merge()
        Catch oEx As Exception
            DMCommon.Functions.ShowEx(oEx, "Application - Merge", sTopLeftCell & ":" & sBottomRightCell)
        End Try
        Return oRange
    End Function
    Public Shared Function Merge(ByVal oRectangle As System.Drawing.Rectangle) As Excel.Range
		Dim oExcelObject As System.Object
		Dim oRange As Excel.Range = Nothing
		Dim sTopLeftCell As String = GetCellAddress(oRectangle.Top, oRectangle.Left)
		Dim sBottomRightCell As String = GetCellAddress(oRectangle.Bottom, oRectangle.Right)
		Dim sTest As String = sTopLeftCell & ":" & sBottomRightCell

		Try
			oExcelObject = moWorksheet.Range(sTopLeftCell, sBottomRightCell)
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
				'oRange.Merge(True) ' Merge by Rows
				oRange.Merge()

				'DMCommon.Debug.MsgBox("After Merge", oRange.MergeCells)
			Catch oEx As Exception
				DMCommon.Debug.UserMsg("Application - Merge_3", oEx.Message, oEx.GetType(), sTopLeftCell, sBottomRightCell, oRange.Count, oRange.Column, oRange.Columns.Count, oRange.Row, oRange.Rows.Count)
				Try
					oRange = CType(oExcelObject, Excel.Range)
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - Merge_4")
				End Try

			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Worksheet.Range Is Nothing", "Application - Merge_5")
		End If
		Return oRange
    End Function
	Public Shared Function zzInitExcel() As Boolean
		' excelApp = Marshal.GetActiveObject("Excel.Application") as Excel.Application
		Const sExcelClass As String = "Excel.Application" '= "Excel.Application.10"
		Dim oExcelObject As System.Object
		Dim stest As String = "Z"
		'	DMCommon.Debug.MsgBox("190528_2", "zzInitExcel")
		Try
			oExcelObject = GetObject(, sExcelClass)
		Catch oEx As Exception
			Try
				oExcelObject = GetObject(String.Empty, sExcelClass)
				'oExcelObject = System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application")
			Catch oExA As Exception
				System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - zzInitExcel_1")
				Return False
			End Try
		End Try

		Try
			moExcelApp = DirectCast(oExcelObject, Excel.Application)

			moExcelApp.Visible = True
			'	System.Windows.Forms.MessageBox.Show(moExcelApp.Name, "Application - zzInitExcel_2")
			stest = "C"
			moWorkbook = moExcelApp.ActiveWorkbook
			stest = "D"
			If moWorkbook Is Nothing Then
				stest = "E"
				moWorkbook = moExcelApp.Workbooks.Add()
				stest = "F"
			End If
			stest = "G"
			oExcelObject = moWorkbook.Worksheets.Add()
			stest = "H"
			moWorksheet = DirectCast(oExcelObject, Excel.Worksheet)
			stest = "I"
			'''''''''''''''''''''''''''	moWorksheet.PageSetup.PrintGridlines = True
			stest = "J"
			moWorksheet.DisplayRightToLeft = RightToLeft
			'	System.Windows.Forms.MessageBox.Show(CStr(moWorksheet.DisplayRightToLeft) & "" & CStr(RightToLeft), "Application - zzInitExcel_2")
			stest = "K"

			Return True
		Catch oEx As Exception
			''''''''System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & stest, "Application - zzInitExcel_33")
			Return False
		End Try

	End Function

	Public Shared Sub NextSheet()
     
    End Sub

    Public Shared Function GetHAlign(ByVal oTableCell As AcadReport.TableCell) As XlHAlign
        If [Enum].IsDefined(GetType(XlHAlign), oTableCell.XlHAlign) Then
            Return CType(oTableCell.XlHAlign, XlHAlign)
        End If
    End Function
    Public Shared Function GetVAlign(ByVal oTableCell As AcadReport.TableCell) As XlVAlign
        If [Enum].IsDefined(GetType(XlVAlign), oTableCell.XlVAlign) Then
            Return CType(oTableCell.XlVAlign, XlVAlign)
        End If
    End Function

   Public Overrides Function Insert() As Boolean
      'Dim s As String
      'If doMainView Is Nothing Then
      '   s = "Nothing"
      'Else
      '   s = CStr(doMainView.Count)
      'End If

      '  System.Windows.Forms.MessageBox.Show(s, "07_040")



      Dim oTitleResource As TPlServerDB.TPlResource = Nothing
      Dim oHeaderResource As TPlServerDB.TPlResource = Nothing
      Dim oDataResource As TPlServerDB.TPlResource = Nothing
      Dim oFooterResource As TPlServerDB.TPlResource = Nothing
      Dim oTitle As TitleSection = Nothing
      Dim oHeader As AcadReport.Section
      Dim oData As DataSection
      Dim iLastRow As Integer
      Dim bResourceExists As Boolean
      If BaseResource IsNot Nothing Then

         oTitleResource = BaseResource.GetChild(0)
         oHeaderResource = BaseResource.GetChild(1)
         oDataResource = BaseResource.GetChild(2)
         oFooterResource = BaseResource.GetChild(3)
         bResourceExists = True
      Else
         '	System.Windows.Forms.MessageBox.Show("BaseResource is Nothing", "07_121")
      End If


		If oTitleResource IsNot Nothing Then
			oTitle = New TitleSection(oTitleResource, doaOptionValues)
		Else
			oTitle = New TitleSection()
		End If
		'	System.Windows.Forms.MessageBox.Show(CStr(mbReverse), "02_238")

		If oHeaderResource IsNot Nothing Then
			oHeader = New HeaderSection(oHeaderResource, mbReverse, dbAreaMeter, doaCaptions)
		Else
			oHeader = New HeaderSection(AcadReport.enSectionType.Header, mbReverse)
			oHeader.TableCell = dtDataTableCell

		End If
		'   System.Windows.Forms.MessageBox.Show(CStr(doMainView.Count), "02_981")

		'   System.Windows.Forms.MessageBox.Show(CStr(doMainView.Count), "02_981")
		If oDataResource IsNot Nothing Then
			oData = New DataSection(oDataResource, doMainView, dbAreaMeter, dbMergeRows, diaDataColumns)
		Else
         oData = New DataSection(doMainView, diaDataColumns)
         oData.TableCell = dtDataTableCell
      End If
      Dim oFooter As AcadReport.Section
      If oFooterResource Is Nothing Then
         oFooter = Nothing
         'oFooter = New HeaderSection(AcadReport.enSectionType.Footer)
      Else
			oFooter = New HeaderSection(oFooterResource, mbReverse, dbAreaMeter, doaTotals)
		End If

		'  oHeader.Format()
		If oTitle IsNot Nothing Then
         oTitle.Format()
      End If

      zzSetColumns(bResourceExists)
      If oTitle IsNot Nothing Then
         oTitle.Print()
         iLastRow = oTitle.LastRow
      Else
         iLastRow = -1
      End If
      '	System.Windows.Forms.MessageBox.Show("", "19_260")
      oHeader.FirstRow = iLastRow + 1
      '	System.Windows.Forms.MessageBox.Show("", "19_269")
      oHeader.Format()
      '	System.Windows.Forms.MessageBox.Show("", "19_270")
      oHeader.Print()
      '    System.Windows.Forms.MessageBox.Show("", "19_280")

      oData.FirstRow = oHeader.LastRow + 1

      oData.Format()
      oData.Print()
      If oFooter IsNot Nothing Then
         oFooter.FirstRow = oData.LastRow + 1
         oFooter.Format()
         oFooter.Print()

      End If


   End Function

    Public Overrides Function Open(Optional ByVal iResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.Undefined) As Boolean
        Dim bRes As Boolean = True

        bRes = MyBase.OnOpen(iResourceTheme)


        Return bRes AndAlso zzInitExcel()
    End Function
	Public Sub SetValue(iRow As Integer, iCol As Integer, oValue As System.Object, Optional sFormat As String = "")
		Dim oRange As Range = GetRange(iRow, iCol)
		oRange.Value = oValue
		If sFormat.Length <> 0 Then
			oRange.NumberFormat = sFormat
		End If

	End Sub
	Public Sub SetHeaderFormat(iRowMax As Integer, iColumnMax As Integer)
		'  Const iBackColor As Integer = 12632256
		Const iLinestyle As Integer = 1

		Dim oHeaderRange As Excel.Range = GetRange(0, 0, iRowMax, iColumnMax)
		Dim oCellRange As Excel.Range
		If oHeaderRange IsNot Nothing Then
			oHeaderRange.HorizontalAlignment = XlHAlign.xlHAlignCenter
			oHeaderRange.VerticalAlignment = XlVAlign.xlVAlignCenter
			oHeaderRange.Font.Bold = True
			oHeaderRange.Interior.Color = &HA0C0C0

			oHeaderRange.WrapText = True

			For iRow As Integer = 0 To iRowMax
				For iColumn As Integer = 0 To iColumnMax
					oCellRange = GetRange(iRow, iColumn)
					oCellRange.BorderAround(iLinestyle, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic)
				Next
			Next
		End If

	End Sub
    Public Function GetWinNumFormat(iFormatNumDigits As Integer, iGroupDigits As Microsoft.VisualBasic.TriState) As String

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
    Private Sub zzSetColumns(ByVal bLeftToRight As Boolean)
        If ColumnUB >= 0 Then
            Dim oRange As Range
            Try
                For iColIndex As Integer = 0 To ColumnUB
                    If bLeftToRight Then
                        oRange = moWorksheet.Range(GetCellAddress(0, ColumnUB - iColIndex))
                        oRange.ColumnWidth = mdK2 * ColWidths(iColIndex)
                    Else
                        oRange = moWorksheet.Range(GetCellAddress(0, iColIndex))
                        oRange.ColumnWidth = mdK1 * ColWidths(iColIndex)
                    End If

                Next
            Catch oEx As Exception
                System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf, "ApplicationEx - zzSetColumns")
            End Try

        End If
    End Sub

    Public Overrides Function InsertMulti() As Boolean

    End Function

    Public Overrides Sub NextTable()
        Dim oExcelObject As System.Object
        If moWorkbook Is Nothing Then

            moWorkbook = moExcelApp.Workbooks.Add()

        End If

        oExcelObject = moWorkbook.Worksheets.Add()

        moWorksheet = DirectCast(oExcelObject, Excel.Worksheet)

        '''''''''''''''''''''''''''	moWorksheet.PageSetup.PrintGridlines = True

        moWorksheet.DisplayRightToLeft = RightToLeft
    End Sub

	Public Overrides Function InsertSpec() As Boolean
		DMCommon.Debug.MsgBox("190528_1", diResourceTheme)
		Select Case diResourceTheme
			Case TPlServerDB.enResourceTheme.AcRepExproLuse
				zzExproLuseReport
		End Select

	End Function
	Private Sub zzExproLuseReport()

	End Sub
End Class
