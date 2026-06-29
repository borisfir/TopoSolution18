Option Explicit On 
Option Strict On

Public Class DBIndexDef
	Private Const msConstraint As String = "CONSTRAINT"
	Private Const msPrimaryKey As String = "PRIMARY KEY"
	Private Const msUnique As String = "UNIQUE"



	Private msName As String
	Private msFieldList As String
	Private mbPrimaryKey As Boolean
	Private mbUnique As Boolean

	Public Property Name() As String
		Get
			Name = msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
		End Set
	End Property
	Public Property FieldList() As String
		Get
			FieldList = msFieldList
		End Get
		Set(ByVal sValue As String)
			msFieldList = sValue
		End Set
	End Property
	Public Property PrimaryKey() As Boolean
		Get
			PrimaryKey = mbPrimaryKey
		End Get
		Set(ByVal bValue As Boolean)
			mbPrimaryKey = bValue
		End Set
	End Property
	Public Property Unique() As Boolean
		Get
			Unique = mbUnique
		End Get
		Set(ByVal bValue As Boolean)
			mbUnique = bValue
		End Set
	End Property
	Public ReadOnly Property Constraint() As Boolean
		Get
			Constraint = mbPrimaryKey Or mbUnique
		End Get
	End Property
	Public Function GetCreatePrimaryCmdText(ByVal iProvider As TPlProvider) As String
      Dim sOut As String = String.Empty
		Dim sConstraintType As String
		If mbPrimaryKey Then
			sConstraintType = msPrimaryKey
		ElseIf mbUnique Then
			sConstraintType = msUnique
      Else
         sConstraintType = String.Empty
      End If


		If (msFieldList.Length <> 0) And (sConstraintType.Length <> 0) Then
			sOut = msConstraint & Strings.Space(1) & msName & Strings.Space(1) & sConstraintType & " (" & msFieldList & ")"
		End If
		Return sOut
	End Function

	Public Sub New(ByVal sName As String)
		msName = sName
	End Sub
End Class
