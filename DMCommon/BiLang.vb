Option Explicit On
Option Strict On
Public Class BiLang
	Private Declare Function LoadKeyboardLayout Lib "user32" Alias "LoadKeyboardLayoutA" (ByVal pwszKLID As String, ByVal flags As Integer) As Integer
	Private Declare Function ActivateKeyboardLayout Lib "user32" (ByVal HKL As Integer, ByVal flags As Integer) As Integer
	Private Declare Function GetKeyboardLayoutList Lib "user32" (ByVal nBuff As Integer, ByVal lpList As Integer) As Integer
	Private Declare Function GetKeyboardLayoutName Lib "user32" Alias "GetKeyboardLayoutNameA" (ByVal pwszKLID As String) As Integer
	Private Declare Function UnloadKeyboardLayout Lib "user32" (ByVal HKL As Integer) As Integer
	Private Declare Function GetKeyboardLayout Lib "user32" (ByVal dwLayout As Integer) As Integer
	Public Const KLF_ACTIVATE As Integer = &H1
	Public Const KLF_REORDER As Integer = &H8
	Public Const KLF_SUBSTITUTE_OK As Integer = &H2
	Public Const KLF_UNLOADPREVIOUS As Integer = &H4
	Public Const LANG_ENGLISH As Integer = &H9
   Public Const KB_HEBREW As Integer = 1037
	Public Const KB_ENGLISH As Integer = 1033
	Public Const KB_HEBREW_STANDARD As Integer = -4034

	Public Const KB_RUSSIAN As Integer = 1049&
	Public Const KB_ENGLISH_UK As Integer = 2057&
	Public Shared Sub SetEnglish()
		Dim lResp As Long

		If GetKeyboardLang() <> KB_ENGLISH Then
			lResp = LoadKeyboardLayout(Format$(Hex(KB_ENGLISH), "00000000"), KLF_ACTIVATE)
		End If
	End Sub

	Public Shared Sub SetHebrew()
		Dim lResp As Long

		If GetKeyboardLang() <> KB_HEBREW Then
			lResp = LoadKeyboardLayout("00000" & Hex$(KB_HEBREW), KLF_ACTIVATE)
		End If
	End Sub

	Public Shared Sub SetLang(ByVal iLang As Integer)
		Dim iResp As Integer

		If GetKeyboardLang() <> iLang Then
			iResp = LoadKeyboardLayout("00000" & Hex$(iLang), KLF_ACTIVATE)
		End If
	End Sub

	Public Shared Function GetKeyboardLang() As Integer
		Return GetKeyboardLayout(0) \ 65537
	End Function
	Public Shared Function GetRightToLeft() As Boolean
		Dim iLang As Integer = GetKeyboardLayout(0) \ 65537
		Select Case iLang
			Case KB_ENGLISH
				Return False
			Case KB_HEBREW, KB_HEBREW_STANDARD
				Return True
			Case Else
				Return False
		End Select
	End Function


End Class
