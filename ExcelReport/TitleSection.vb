Option Explicit On
Option Strict On
Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Excel
Friend Class TitleSection
   Inherits AcadReport.Section 'SectionAAA
	Private moaData() As System.Object = Nothing
	Private msLastColumnAddress As String
	Public Sub New()

	End Sub
   Public Sub New(ByVal oResource As TPlServerDB.TPlResource, Optional ByVal oaData() As System.Object = Nothing)

		MyBase.New(oResource)
		MyBase.diSectionType = AcadReport.enSectionType.Excel Or AcadReport.enSectionType.Title
		If Me.diSectionID > 0 Then
			MyBase.RowCount = ddaRowHeight.GetUpperBound(0) + 1
			MyBase.FirstRow = 0
			moaData = oaData
		End If

   End Sub

   Public Overrides Sub Format()
		If MyBase.diCellTypeUB >= 0 AndAlso MyBase.RowCount > 0 Then
			Try
				MyBase.diCellTypeUB = MyBase.RowCount - 1
				MyBase.OnFormatting()

			Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TitleSection - Format")
			End Try
		End If
	End Sub

	Public Overrides Sub Print()
		Me.zzSetSheetName()
		For iRowIndex As Integer = MyBase.FirstRow To MyBase.diLastRow
			zzInsertTitleText(iRowIndex)
		Next


	End Sub
	Private Sub zzInsertTitleText(ByVal iRowIndex As Integer)
		Dim sTitleText As String = Report.GetText(iRowIndex, MyBase.diSectionID)
		Dim oRange As Excel.Range
		Dim oTableCell As AcadReport.TableCell

		Try
			oTableCell = MyBase.doaTableCell(iRowIndex)
			oTableCell.DestType = AcadReport.enDestType.WinApp
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection-zzInsertTitleText_1")
			Exit Sub
		End Try
		oRange = Report.Merge(iRowIndex, iRowIndex, 0, Report.ColumnUB)
		If oRange IsNot Nothing Then
         sTitleText = oTableCell.Format(sTitleText, True, dbAreaMeter, moaData)
			System.Windows.Forms.MessageBox.Show(sTitleText, "01_310")
			oRange.Value = sTitleText
			oRange.HorizontalAlignment = Report.GetHAlign(oTableCell)
			oRange.VerticalAlignment = Report.GetVAlign(oTableCell)


			If MyBase.dtExcelFont.Exists Then
				oRange.Font.Name = MyBase.dtExcelFont.Name
				If Not MyBase.dtExcelFont.DefaultSize Then
					oRange.Font.Size = dtExcelFont.Size
				End If
				oRange.Font.Bold = True
			Else
				Try
					oRange.Font.Bold = True
				Catch oEx As Exception
				End Try
			End If
			oRange.RowHeight = ddaRowHeight(iRowIndex)
			oRange.ReadingOrder = -5004
		End If

	End Sub
	Private Sub zzSetSheetName()
		Dim dicNames As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
		Dim sBaseName As String

		If Me.diSectionID >= 0 Then
         sBaseName = Report.GetText(11, MyBase.diSectionID)
		Else
			sBaseName = Report.Caption
      End If
      ' TitleSection.vb() : Line 87
		If sBaseName Is Nothing OrElse sBaseName.Length = 0 Then
			sBaseName = Report.GetText(0, MyBase.diSectionID)
		End If

		Dim sResName As String = sBaseName

		Try
			For Each oSheet As Worksheet In Report.moWorkbook.Sheets
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
			Report.moWorksheet.Name = sResName
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TitleSection - SetSheetName")
		End Try

	End Sub
End Class
