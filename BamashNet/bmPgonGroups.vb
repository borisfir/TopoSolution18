Option Explicit On
Option Strict On
Public Class bmPgonGroups
	Inherits Dictionary(Of Integer, bmPgonGroup)
	Public Sub AddPgon(oPgon As BamashPolygon)
		If oPgon.GroupID <> 0 Then
			Dim oGroup As bmPgonGroup = Nothing
			If Not MyBase.TryGetValue(oPgon.GroupID, oGroup) Then
				oGroup = New bmPgonGroup(oPgon.GroupID)
				Me.Add(oGroup.GroupID, oGroup)
			End If
			oGroup.AddPgon(oPgon)
		End If
	End Sub
End Class
