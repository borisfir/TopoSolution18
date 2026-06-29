Option Explicit On
Option Strict On
Imports System.Drawing
Public MustInherit Class BaseHeaderSection
	Inherits Section
	Protected doaData() As System.Object = Nothing
	Protected doMerges() As Rectangle
	Protected diMinUB As Integer


	Public Sub New() ' If Not exists
		MyBase.New()
	End Sub
	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, Optional ByVal oaData() As System.Object = Nothing)

		MyBase.New(oResource)
		If diSectionID >= 0 Then
			doMerges = oResource.GetRectangleArray(2)
			MyBase.RowCount = ddaRowHeight.GetUpperBound(0) + 1
			MyBase.FirstRow = 0
			doaData = oaData
			'DMCommon.Debug.MsgBox("13_130f", doaData)
			'	DMCommon.Functions.DispArray("!!doaData", doaData, True)
		Else
			'DMCommon.Debug.MsgBox("04_193s", CStr(diSectionID), oResource.Level, oResource.ObjectNo)
		End If

	End Sub
	Public Overrides Sub Format()
      ' System.Windows.Forms.MessageBox.Show(CStr(BaseReport.ColumnResUB) & ":" & CStr(BaseReport.GroupColumnUB) & ":" & CStr(BaseReport.ColumnUB) & vbCrLf & CStr(diSectionID) & ":" & CStr(MyBase.RowCount), "19_300!")
		If MyBase.RowCount > 0 Then  'diSectionID >= 0 AndAlso
			MyBase.diCellTypeResUB = MyBase.RowCount * (BaseReport.ColumnResUB + 1) - 1
			MyBase.diCellTypeUB = (MyBase.RowCount * (BaseReport.ColumnUB + 1) - 1)

         '   System.Windows.Forms.MessageBox.Show(CStr(MyBase.RowCount) & ":" & CStr(BaseReport.ColumnUB) & vbCrLf & CStr(MyBase.diCellTypeResUB) & ":" & CStr(MyBase.diCellTypeUB), "19_301a")
			If doMerges IsNot Nothing Then
				diMinUB = MyBase.diCellTypeUB
				If diMinUB > doMerges.GetUpperBound(0) Then
					'	MessageBox.Show(CStr(diMinUB) & ":" & CStr(doMerges.GetUpperBound(0)), "19_302")
					diMinUB = doMerges.GetUpperBound(0)
				End If
			Else
				diMinUB = -1
			End If

			For iIndex As Integer = 0 To diMinUB
				MyBase.Frmt(doMerges(iIndex))
			Next
         '   System.Windows.Forms.MessageBox.Show(CStr(diCellTypeResUB) & ":" & CStr(diCellTypeUB), "05_320 BaseH")
			OnFormatting()
      End If
      '   System.Windows.Forms.MessageBox.Show("End HeaderFormat", "04_341")
	End Sub

	Public Sub ThisHeader()
		MyBase.diSectionType = MyBase.diSectionType Or enSectionType.Header
	End Sub

	Public Sub ThisFooter()
		MyBase.diSectionType = MyBase.diSectionType Or enSectionType.Footer
	End Sub
	Public Sub BeforeFormatting()
		If MyBase.RowCount() > 0 Then
			MyBase.diCellTypeUB = (MyBase.RowCount * (BaseReport.ColumnUB + 1)) - 1

			If doMerges IsNot Nothing Then
				diMinUB = MyBase.diCellTypeUB
				If diMinUB > doMerges.GetUpperBound(0) Then
					diMinUB = doMerges.GetUpperBound(0)
				End If
			Else
				diMinUB = -1
			End If
		End If
	End Sub



	Public MustOverride Overrides Sub Print()


End Class
