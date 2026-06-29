Option Explicit On 
Option Strict On

Public NotInheritable Class DBTableDef
	Private Const msCreateTableSQL As String = "CREATE TABLE"
	Private miTableID As Integer
	Private msTableName As String
	Private moDBFieldDefs As DBFieldDefs
	Private moDBIndexDefs As DBIndexDefs
	Public Property TableID() As Integer
		Get
			TableID = miTableID
		End Get
		Set(ByVal iValue As Integer)
			miTableID = iValue
		End Set
	End Property

	Public Property TableName() As String
		Get
			TableName = msTableName
		End Get
		Set(ByVal sValue As String)
			sValue = msTableName
		End Set
	End Property
	Public ReadOnly Property FieldDefs() As DBFieldDefs
		Get
			FieldDefs = moDBFieldDefs
		End Get
	End Property
	Public ReadOnly Property IndexDefs() As DBIndexDefs
		Get
			IndexDefs = moDBIndexDefs
		End Get
	End Property
	Public Sub New(ByVal iTableID As Integer, ByVal sTableName As String)
		Me.New()
		miTableID = iTableID
		msTableName = sTableName
	End Sub
	Public Sub New()
		moDBFieldDefs = New DBFieldDefs
		moDBIndexDefs = New DBIndexDefs
	End Sub
	Public Function GetCreateTableCmdText(ByVal iProvider As TPlProvider) As String
		Dim sOut As String
		Dim oDBFieldDef As DBFieldDef
		Dim oDBIndexDef As DBIndexDef
		Dim sFieldDelim As String
		sFieldDelim = Space(0)
		sOut = msCreateTableSQL & Strings.Space(1) & msTableName & " ("
		For Each oDBFieldDef In moDBFieldDefs
			If oDBFieldDef.DataType <> 0 Then
				sOut = sOut & sFieldDelim & oDBFieldDef.GetCreateColumnCmdText(iProvider)
				sFieldDelim = [Global].Comma
			End If
		Next
		For Each oDBIndexDef In moDBIndexDefs
			If oDBIndexDef.Constraint Then
				sOut = sOut & sFieldDelim & oDBIndexDef.GetCreatePrimaryCmdText(iProvider)
			End If
		Next

		sOut = sOut & " )"
		GetCreateTableCmdText = sOut
	End Function

End Class
