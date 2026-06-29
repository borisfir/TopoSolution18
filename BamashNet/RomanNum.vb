Option Explicit On
Option Strict On
Public Class RomanNum
	Const sRom1 As String = "I"
	Const sRom5 As String = "V"
	Const sRom10 As String = "X"
	Const sRom50 As String = "L"
	Const sRom100 As String = "C"
	Const sRom500 As String = "D"
	Const sRom1000 As String = "M"

	Public Shared Function ToRoman(ByVal iValue As Integer) As String
		Dim iU1 As Integer
		Dim iU10 As Integer = Math.DivRem(iValue, 10, iU1)
		Dim sRes As String = String.Empty
		If iU10 <> 0 Then
			sRes = zz10(iU10, sRom10, sRom50, sRom100)
		End If
		If iU1 <> 0 Then
			sRes &= zz10(iU1, sRom1, sRom5, sRom10)
		End If
		Return sRes
	End Function
	Private Shared Function zz10(ByVal iValue As Integer, ByVal s1 As String, ByVal s5 As String, ByVal s10 As String) As String
		Select Case iValue
			Case 1
				Return s1
			Case 2
				Return s1 & s1
			Case 3
				Return s1 & s1 & s1
			Case 4
				Return s1 & s5
			Case 5
				Return s5
			Case 6 To 8
				Return s5 & zz10(iValue - 5, s1, String.Empty, String.Empty)
			Case 9, 10
				Return zz10(iValue - 5, s1, s10, String.Empty)
			Case Else
				Return String.Empty
		End Select

	End Function

End Class
