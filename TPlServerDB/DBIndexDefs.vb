Option Explicit On 
Option Strict On

Public Class DBIndexDefs
	Inherits System.Collections.CollectionBase
	Public Function Add(ByVal oValue As DBIndexDef) As Integer
		Return List.Add(oValue)
	End Function
	Default Public Property Item(ByVal iIndex As Integer) As DBIndexDef
		Get
			Return DirectCast(List(iIndex), DBIndexDef)
		End Get
		Set(ByVal oValue As DBIndexDef)
			List(iIndex) = oValue
		End Set

	End Property
	Public Function IndexOf(ByVal oValue As DBIndexDef) As Integer
		Return List.IndexOf(oValue)
	End Function	'IndexOf


	Public Sub Insert(ByVal iIndex As Integer, ByVal oValue As DBIndexDef)
		List.Insert(iIndex, oValue)
	End Sub	'Insert


	Public Sub Remove(ByVal oValue As DBIndexDef)
		List.Remove(oValue)
	End Sub	'Remove


	Public Function Contains(ByVal oValue As DBIndexDef) As Boolean
		' If value is not of type DBIndexDef, this will return false.
		Return List.Contains(oValue)
	End Function	'Contains


	Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As [Object])
		If Not value.GetType() Is Type.GetType("TPlanLib.TPlanLib.DBIndexDef") Then
			Throw New ArgumentException("Value must be of type DBIndexDef.", "value")
		End If
	End Sub	'OnInsert


	Protected Overrides Sub OnRemove(ByVal iIndex As Integer, ByVal oValue As [Object])
		If Not oValue.GetType() Is Type.GetType("TPlanLib.TPlanLib.DBIndexDef") Then
			Throw New ArgumentException("Value must be of type DBIndexDef.", "value")
		End If
	End Sub	'OnRemove


	Protected Overrides Sub OnSet(ByVal iIndex As Integer, ByVal oldValue As [Object], ByVal newValue As [Object])
		If Not newValue.GetType() Is Type.GetType("TPlanLib.TPlanLib.DBIndexDef") Then
			Throw New ArgumentException("newValue must be of type DBIndexDef.", "newValue")
		End If
	End Sub	'OnSet

	Protected Overrides Sub OnValidate(ByVal oValue As [Object])
		If Not (oValue.GetType() Is Type.GetType("TPlanLib.TPlanLib.DBIndexDef")) Then
			Throw New ArgumentException("Value must be of type DBIndexDef.")
		End If
	End Sub	'OnValidate 

End Class
