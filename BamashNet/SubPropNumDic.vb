Option Explicit On
Option Strict On
Public Class SubPropNumDic
	Inherits Generic.Dictionary(Of Integer, Integer)
	Private miCounter As Integer
	Private miMaxNumber As Integer = 0

	Public Function GetNext() As Integer
		miCounter += 1
		Return miCounter

	End Function
	Public Function GetNextFree() As Integer

		Do
			miCounter += 1
			If Not MyBase.ContainsKey(miCounter) Then
				Me.Add(miCounter)
				Return miCounter
			End If
		Loop
	End Function
	Public Function GetMinFree(ByVal iNumber As Integer) As Integer
		Do
			If Not MyBase.ContainsKey(iNumber) Then
				Me.Add(iNumber)
				Return iNumber
			End If
		Loop
	End Function
	Public Overloads Sub Add(ByVal iNumber As Integer)
		MyBase.Add(iNumber, 0)
		If iNumber > miMaxNumber Then
			miMaxNumber = iNumber
		End If
	End Sub
	Public Sub ResetCounter()
		miCounter = 0
	End Sub
	Public Sub GoToMax()
		miCounter = miMaxNumber
	End Sub
End Class
