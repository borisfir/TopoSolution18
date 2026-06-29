Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop
'<Assembly: CLSCompliant(False)> 

Public Class ExcelImport
   Private Shared moExcelApp As Excel.Application
   Private Shared moWorkbook As Excel.Workbook
   Private Shared moWorksheet As Excel.Worksheet
   Private Shared moWindow As Microsoft.Office.Interop.Excel.Window
   Private Shared miCurrentRow As Integer = 0
   Public Shared Function OpenExcelApp() As Boolean
      Const sExcelClass As String = "Excel.Application" '= "Excel.Application.10"
      Dim oExcelObject As System.Object
      'Dim oSelectionObject As System.Object

      Dim stest As String = "Z"
      Try
         oExcelObject = GetObject(, sExcelClass)

      Catch oEx As Exception
         Return False
        
      End Try

      Try
         moExcelApp = DirectCast(oExcelObject, Excel.Application)

         moExcelApp.Visible = True
			System.Windows.Forms.MessageBox.Show(moExcelApp.Name & vbCrLf & moExcelApp.Workbooks.Count.ToString(), "1:OpenExcelApp-ExcelImport")
			stest = "C"
         moWorkbook = moExcelApp.ActiveWorkbook
         oExcelObject = moExcelApp.ActiveSheet
         moWindow = moExcelApp.ActiveWindow

			System.Windows.Forms.MessageBox.Show(CStr(moWorkbook Is Nothing) & vbCrLf & CStr(oExcelObject Is Nothing) & vbCrLf & CStr(moWindow Is Nothing), "2:Application - zzInitExcel_55")
			stest = "D"
         If moWorkbook Is Nothing AndAlso moExcelApp.Workbooks.Count > 0 Then
            stest = "E"
            moWorkbook = moExcelApp.Workbooks.Item(0)
            stest = "F"
         End If
         If moExcelApp.Workbooks.Count = 0 Then
            moExcelApp.Quit()
            Return False
         Else
            If moWorkbook IsNot Nothing AndAlso oExcelObject Is Nothing Then
               oExcelObject = moWorkbook.Worksheets.Item(1)
            End If

            stest = "G"

            stest = "H"
            moWorksheet = DirectCast(oExcelObject, Excel.Worksheet)
            stest = "I"
            '''''''''''''''''''''''''''	moWorksheet.PageSetup.PrintGridlines = True
            stest = "J"
            '  moWorksheet.DisplayRightToLeft = False
            '	System.Windows.Forms.MessageBox.Show(CStr(moWorksheet.DisplayRightToLeft) & "" & CStr(RightToLeft), "Application - zzInitExcel_2")
            stest = "K"
            '    Dim oRange As Range = GetRange(2, 2)

            '
            '   System.Windows.Forms.MessageBox.Show(oRange.Value.ToString(), "08_709")
            '   moWorksheet.Select()
            Return True

         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & stest, "2:OpenExcelApp-ExcelImport")
         Return False
      End Try

   End Function
   Public Shared Function GetSelectionValue() As System.Object(,)
      Dim oSelectionObject As System.Object
      Dim oSelRange As Range
		Dim oaValues As System.Object(,) = Nothing
		If moWindow IsNot Nothing Then
         oSelectionObject = moWindow.Selection
         If oSelectionObject IsNot Nothing Then

            oSelRange = TryCast(oSelectionObject, Range)
            If oSelRange IsNot Nothing Then
               '  System.Windows.Forms.MessageBox.Show(oSelRange.GetType().ToString() & vbCrLf & oSelRange.AddressLocal, "08_723")

               '  System.Windows.Forms.MessageBox.Show(oSelRange.Row.ToString() & vbCrLf & oSelRange.Column.ToString() & vbCrLf & oSelRange.Address, "08_723")
               '   System.Windows.Forms.MessageBox.Show(oSelRange.Rows.Row.ToString() & vbCrLf & oSelRange.Columns.Column.ToString() & vbCrLf & oSelRange.AddressLocal & vbCrLf & oSelRange.Value.ToString(), "08_724")
               '  System.Windows.Forms.MessageBox.Show(moWorksheet.Range(oSelRange.AddressLocal).Row.ToString() & vbCrLf & moWorksheet.Range(oSelRange.AddressLocal).Column.ToString() & vbCrLf & oSelRange.AddressLocal & vbCrLf & oSelRange.Value.ToString(), "08_725")

               oaValues = TryCast(oSelRange.Value, Object(,))

            Else
               System.Windows.Forms.MessageBox.Show("oSelRange Is Nothing", "08_902")
            End If

         End If


      End If
      Return oaValues
   End Function
   Public Shared Function GetRange(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As Excel.Range
      Dim oRange As Excel.Range = Nothing
      Dim sCell As String = GetCellAddress(iRowIndex, iColIndex)
      Try
         oRange = moWorksheet.Range(sCell)
      Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sCell & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColIndex), "Application - GetRange_6")
		End Try
      Return oRange
   End Function
   Public Shared Function GetCellAddress(ByVal iRowIndex As Integer, ByVal iColIndex As Integer) As String
      If iColIndex <= 25 Then
         Return Chr(65 + iColIndex) & CStr(iRowIndex + 1)
      Else
         iColIndex = iColIndex - 26
         Return Chr(65 + iColIndex \ 26) & Chr(65 + iColIndex Mod 26) & CStr(iRowIndex + 1)
      End If
   End Function
End Class
