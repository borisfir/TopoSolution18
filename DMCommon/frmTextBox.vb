Public Class frmTextBox
	Private msaLineArray() As String
	Private moaCellArray()() As System.Object
	Private moaCells() As CellRow
	Private miDefaultRowUB As Integer
	Private moaCellRow() As System.Object
	Private Class CellRow
		Private moaRow() As System.Object
		Public Sub New(oaRow() As System.Object)
			moaRow = oaRow
		End Sub
		Public Sub New(iUB As Integer)
			ReDim moaRow(iUB)

		End Sub
		Public Property CellValue(iColumn As Integer) As System.Object
			Get
				If iColumn >= 0 AndAlso iColumn <= RowDim() Then
					Return moaRow(iColumn)
				Else
					Return Nothing
				End If


			End Get
			Set(oValue As System.Object)
				If iColumn >= 0 AndAlso iColumn <= RowDim() Then
					moaRow(iColumn) = oValue
				ElseIf iColumn > RowDim() Then
					ReDim Preserve moaRow(iColumn)
					moaRow(iColumn) = oValue

				End If
			End Set
		End Property
		Public ReadOnly Property Array() As System.Object()
			Get
				Return moaRow
			End Get
		End Property
		Public Function CellValueToString() As String
			Return ""
		End Function
		Public Function ToLineString() As String
			Dim sRes As String = Nothing
			If moaRow Is Nothing Then
				Return String.Empty
			Else
				For iColumn As Integer = 0 To UBound(moaRow)
					If iColumn = 0 Then
						sRes = zzCellValueToString(iColumn)
					Else
						sRes &= zzCellValueToString(iColumn) & ","
					End If
				Next
				Return sRes
			End If
		End Function

		Public Function RowDim() As Integer
			If moaRow Is Nothing Then
				Return -1
			Else
				Return UBound(moaRow)
			End If
		End Function
		Private Function zzCellValueToString(iColumn As Integer) As String
			If moaRow(iColumn) Is Nothing Then
				Return String.Empty
			Else
				Return moaRow(iColumn).ToString()
			End If
		End Function
	End Class
	Public Sub New(iLineArrayUB As Integer, iDefaultRowUB As Integer)

		' This call is required by the designer.
		InitializeComponent()
		ReDim msaLineArray(iLineArrayUB)
		' Add any initialization after the InitializeComponent() call.
		' Me.txtBox.Lines = New String() {"", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""}
		ReDim moaCellArray(iLineArrayUB)
		ReDim moaCells(iLineArrayUB)
		miDefaultRowUB = iDefaultRowUB
		'zzSetToTextbox()
		Dim iLen As Integer = Me.txtBox.Lines.Length
	End Sub

	Public ReadOnly Property TextBox As Windows.Forms.TextBox
		Get
			Return Me.txtBox
		End Get
	End Property
	Public Sub Test()
		'saLineArray(5) = New String() = {"AAAAAAAAAA"}

		'Dim oaRow() As Object = zzGetRow(3)
		Dim oCellRow As CellRow = zzGetRow(3)
		For i As Integer = 0 To 12
			CellValue(i, i) = 100 + 10 * i
		Next
		Dim oValues() As Object = {11, 12, 15, 18, 22}
		SetValuesToRow(14, 2, oValues)
	End Sub
	Public Sub ToBox()
		zzSetToLineArray()
		zzSetToTextbox()
		'	zzSetToLineArray()
	End Sub
	Public Property CellValue(iRow As Integer, iColumn As Integer) As System.Object
		Get
			Dim oCellRow As CellRow = zzGetRow(iRow)
			If oCellRow IsNot Nothing Then
				If iColumn >= 0 AndAlso iColumn <= oCellRow.RowDim Then
					Return oCellRow.CellValue(iColumn)
				Else
					Return Nothing
				End If
			Else
				Return Nothing
			End If
			'oaCellRow() As System.Object

		End Get
		Set(oValue As System.Object)

			Dim oCellRow As CellRow = zzGetRow(iRow)
			If oCellRow IsNot Nothing Then
				If iColumn >= 0 Then  ' AndAlso iColumn <= oCellRow.RowDim 
					oCellRow.CellValue(iColumn) = oValue
				End If
			End If
		End Set
	End Property
	Public Sub SetValuesToRow(iRow As Integer, iColumn As Integer, ParamArray oaValue() As System.Object)
		For iValIndex As Integer = 0 To oaValue.GetUpperBound(0)

			Me.CellValue(iRow, iColumn + iValIndex + 1) = oaValue(iValIndex)

		Next
	End Sub

	Private Sub zzSetToTextbox()
		zzSetToLineArray()
		Me.txtBox.Lines = msaLineArray

	End Sub
	Private Sub zzSetToLineArray()
		Dim oCellRow As CellRow
		If UBound(msaLineArray) <> UBound(moaCellArray) Then
			ReDim Preserve msaLineArray(UBound(moaCellArray))
		End If
		If False Then
			For iRow As Integer = 0 To UBound(moaCellArray)
				oCellRow = New CellRow(moaCellArray(iRow))
				msaLineArray(iRow) = oCellRow.ToLineString()

			Next
		End If

		For iRow As Integer = 0 To UBound(moaCells)
			oCellRow = moaCells(iRow)
			If oCellRow IsNot Nothing Then
				msaLineArray(iRow) = oCellRow.ToLineString()
			End If


		Next
	End Sub
	Private Function zzGetRowOld(iRowNo As Integer) As System.Object()
		Dim iRowsUB As Integer = UBound(moaCellArray)
		If iRowNo >= 0 Then
			If iRowNo > iRowsUB Then

				ReDim Preserve moaCellArray(iRowNo)
				ReDim Preserve moaCells(iRowNo)





			End If
			Return moaCellArray(iRowNo)
		Else
			Return Nothing
		End If
	End Function

	Private Function zzGetRow(iRowNo As Integer) As CellRow
		Dim iRowsUB As Integer = UBound(moaCells)
		'	Dim oResCellRow As CellRow
		If iRowNo >= 0 Then
			If iRowNo > iRowsUB Then
				ReDim Preserve moaCells(iRowNo)
			End If

			If moaCells(iRowNo) Is Nothing Then
				moaCells(iRowNo) = New CellRow(miDefaultRowUB)
			End If
			Return moaCells(iRowNo)
		Else
			Return Nothing
		End If
	End Function

End Class
