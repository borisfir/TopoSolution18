Option Explicit On
Option Strict On

Public Class Summator
	Private mdicSums As Dictionary(Of String, NumSummator)

	Public Sub New()
		mdicSums = New Dictionary(Of String, NumSummator)()






	End Sub
	Public Sub AddValue(sKey As String, dValue As Double)
		Dim oNumSummator As NumSummator = Nothing
		If Not mdicSums.TryGetValue(sKey, oNumSummator) Then
			oNumSummator = New NumSummator()
			mdicSums.Add(sKey, oNumSummator)
		End If
		oNumSummator.AddValue(dValue)
	End Sub
	Public Function GetValue(sKey As String) As Double
		Dim oNumSummator As NumSummator = Nothing

		If mdicSums.TryGetValue(sKey, oNumSummator) Then
			Return oNumSummator.DoubleValue
		Else
			Return 0.0
		End If
	End Function
	Public Sub GetAllValue(ByRef saKeys() As String, ByRef daValues() As Double, ByRef dTotal As Double)
		Dim iUB As Integer = mdicSums.Count - 1
		Dim iIndex As Integer = 0
		dTotal = 0.0
		If iUB >= 0 Then
			ReDim saKeys(iUB)
			ReDim daValues(iUB)
			For Each sKey As String In mdicSums.Keys
				saKeys(iIndex) = sKey
				daValues(iIndex) = mdicSums.Item(sKey).DoubleValue
				dTotal += daValues(iIndex)
				iIndex += 1
			Next
		End If



	End Sub
	Private Class NumSummator
		Private mdDoubleValue As Double
		Public Shared Operator +(oValueA As NumSummator, oValueB As NumSummator) As Double
			Return oValueA.DoubleValue + oValueB.DoubleValue
		End Operator
		Public Sub AddValue(dValue As Double)
			mdDoubleValue += dValue
		End Sub
		Property DoubleValue As Double
			Get
				Return mdDoubleValue
			End Get
			Set(dValue As Double)
				mdDoubleValue = dValue
			End Set
		End Property
	End Class

End Class
