Option Explicit On
Option Strict On
Public Class DBFieldDefs
		Inherits System.Collections.CollectionBase
		Public Function Add(ByVal oFieldDef As DBFieldDef) As Integer
			Return List.Add(oFieldDef)
		End Function
		Default Public Property Item(ByVal iIndex As Integer) As DBFieldDef
			Get
				Return DirectCast(List(iIndex), DBFieldDef)
			End Get
			Set(ByVal oValue As DBFieldDef)
				List(iIndex) = oValue
			End Set
		End Property
		Public Function IndexOf(ByVal oValue As DBFieldDef) As Integer
			Return List.IndexOf(oValue)
		End Function		 'IndexOf


		Public Sub Insert(ByVal iIndex As Integer, ByVal oValue As DBFieldDef)
			List.Insert(iIndex, oValue)
		End Sub		 'Insert


		Public Sub Remove(ByVal oValue As DBFieldDef)
			List.Remove(oValue)
		End Sub		 'Remove


		Public Function Contains(ByVal oValue As DBFieldDef) As Boolean
			' If value is not of type DBFieldDef, this will return false.
			Return MyBase.List.Contains(oValue)
		End Function		 'Contains


		Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As [Object])
			If Not value.GetType() Is Type.GetType("TPlanLib.DBFieldDef") Then
				Throw New ArgumentException("Value must be of type DBFieldDef.", "value")
			End If
		End Sub		 'OnInsert


		Protected Overrides Sub OnRemove(ByVal iIndex As Integer, ByVal oValue As [Object])
			If Not oValue.GetType() Is Type.GetType("TPlanLib.DBFieldDef") Then
				Throw New ArgumentException("Value must be of type DBFieldDef.", "value")
			End If
		End Sub		 'OnRemove


		Protected Overrides Sub OnSet(ByVal iIndex As Integer, ByVal oldValue As [Object], ByVal newValue As [Object])
			If Not newValue.GetType() Is Type.GetType("TPlanLib.DBFieldDef") Then
				Throw New ArgumentException("newValue must be of type DBFieldDef.", "newValue")
			End If
		End Sub		 'OnSet

		Protected Overrides Sub OnValidate(ByVal oValue As [Object])
			If Not oValue.GetType() Is Type.GetType("TPlanLib.DBFieldDef") Then
				Throw New ArgumentException("Value must be of type DBFieldDef.")
			End If
		End Sub		 'OnValidate 

	End Class