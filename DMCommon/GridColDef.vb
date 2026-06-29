Option Explicit On
Option Strict On
Public Class GridColDef
	Public FieldName As String
	Public HeaderText As String
	Public HeaderTextSetting As String
	Public ToolTipText As String
	Public ColumnWidth As Integer
	Public ColumnWidthSetting As Integer
	Public ExcelColumnWidth As Double
	Public [ReadOnly] As Boolean
	Public CellStyleName As String
	Public IsMoney As Boolean
	Public Sub New(sFieldName As String)
		FieldName = sFieldName
	End Sub
	Public Function HasSetting() As Boolean
		Return (HeaderTextSetting IsNot Nothing) OrElse (ColumnWidthSetting <> 0)
	End Function
	Public Function GetHeaderText() As String
		If HeaderTextSetting Is Nothing Then
			Return HeaderText
		Else
			Return HeaderTextSetting
		End If
	End Function
	Public Function GetColumnWidth() As Integer
		If ColumnWidthSetting = 0 Then
			Return ColumnWidth
		Else
			Return ColumnWidthSetting
		End If
	End Function
	Public Sub New(ByVal sHeaderText As String, ByVal sToolTipText As String, ByVal iColumnWidth As Integer, ByVal bReadOnly As Boolean, ByVal sCellStyleName As String, ByVal bIsMoney As Boolean)
		HeaderText = sHeaderText
		ToolTipText = sToolTipText
		ColumnWidth = iColumnWidth
		[ReadOnly] = bReadOnly
		CellStyleName = sCellStyleName
		IsMoney = bIsMoney
	End Sub
End Class
