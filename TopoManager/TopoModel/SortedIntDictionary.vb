Option Explicit On
Option Strict On
Public Class SortedIntDictionary
	Inherits SortedDictionary(Of Integer, System.Object)
	Public Sub AddInt(iValue As Integer)
		MyBase.Add(iValue, Nothing)
	End Sub


End Class
