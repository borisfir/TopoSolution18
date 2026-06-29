Option Explicit On
Option Strict On

Public Class Class1
   Public Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As Integer
   Public Declare Function SetActiveWindow Lib "user32" Alias "SetActiveWindow" (ByVal hwnd As Integer) As Integer

   Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As Integer, ByVal lpString As String, ByVal cch As Integer) As Integer
   Declare Function GetWindowTextLength Lib "user32" Alias "GetWindowTextLengthA" (ByVal hwnd As Integer) As Integer
   Public Shared Function GetActiveWinText() As String
      Dim iHWnd As Integer = GetActiveWindow()
      Dim iWinTextLen As Integer = GetWindowTextLength(iHWnd)
      Dim cch As Integer = 255
      Dim sBuffer As String = Strings.Space(255)
      Dim iResp As Integer = GetWindowText(iHWnd, sBuffer, cch)
      Dim sOut As String = Left(sBuffer, iWinTextLen)
      Return sOut
   End Function
End Class
