Option Explicit On
Option Strict On
Public Class Form1
	Private Structure Test
		Dim Num As Integer
		Dim Str As String

	End Structure
	Private mtMain() As Test
	Private mtCopy() As Test
	Private mtCopyA(2) As Test
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		'	Application.zzInitExcel()
	End Sub

	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
		ReDim mtMain(2)
		mtMain(0).Num = 10
		mtMain(0).Str = "a10"
		mtMain(1).Num = 11
		mtMain(1).Str = "a11"
		mtCopy = mtMain
		For i As Integer = 0 To 2
			mtCopyA(i) = mtMain(i)
		Next

		mtMain(1).Num = 200
		mtMain(1).Str = "b11"
		Stop
	End Sub
End Class