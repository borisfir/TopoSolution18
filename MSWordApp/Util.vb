Imports Microsoft.Office.Interop.Word
Imports Microsoft.Office.Interop
Public Class Util
   Public Shared Sub CreatePDF(sFolder As String, sSourceFileName As String, sOutputFullFileName As String)
      '   System.Windows.Forms.MessageBox.Show(sFolder & "\" & sSourceFileName, "04_201")
      Dim oWordApp As Word.Application
      Dim oWordDoc As Word.Document
      Dim oPageSetup As PageSetup
      oWordApp = New Word.Application
      'oWordApp.Visible = True
      oWordDoc = oWordApp.Documents.Open(sFolder & "\" & sSourceFileName)
      For iIndex As Integer = 1 To oWordDoc.Paragraphs.Count
         oWordDoc.Paragraphs.Item(iIndex).ReadingOrder = WdReadingOrder.wdReadingOrderLtr
      Next

      '    System.Windows.Forms.MessageBox.Show(CStr(oWordDoc.Paragraphs.Count), "04_201")
      oPageSetup = oWordDoc.PageSetup
      oPageSetup.SectionDirection = WdSectionDirection.wdSectionDirectionLtr
      oWordDoc.ExportAsFixedFormat(sOutputFullFileName, WdExportFormat.wdExportFormatPDF, True)
      oWordDoc.Close()
      oWordApp.Quit()
      'If oWordApp.OpenFile(sSourceFullName, True) = 0 Then
      '   oWordApp.CreatePDF True
      'End If
      'oWordApp.CloseApp()
   End Sub
 
End Class
