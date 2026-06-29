Option Explicit On
Option Strict On
Public Structure GraphMessageAAA
	Dim Text As String
	Dim Point As TPlnPoint
	Public Sub New(ByVal sText As String, ByVal oPoint As TPlnPoint)
		Text = sText
		Point = oPoint
	End Sub
End Structure

