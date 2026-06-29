Public Class Hebrew
	Public Shared Function ToDOS(ByVal sValue As String) As String
		Dim sTest As String = ""
		Dim sOut As String = String.Empty
		Dim sHebWord As String = String.Empty

		Dim iASCCode As Integer

		If sValue.Length = 0& Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))

				Select Case iASCCode
					Case 34, 224 To 250
						sHebWord = sHebWord & Chr(iASCCode)

						'' Case 40, 41

					Case Else
						sOut &= WordToDOS(sHebWord, True) & Chr(iASCCode)
						sTest &= sHebWord & vbCrLf
						sHebWord = String.Empty
				End Select
			Next
			sOut &= WordToDOS(sHebWord, True)
			sTest &= sHebWord & vbCrLf
			Return sOut
		End If
	End Function
	Public Shared Function WordToDOS(ByVal sValue As String, ByVal bExcludeAleph As Boolean) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim iAscMin As Integer
		If sValue Is Nothing OrElse sValue.Length = 0 Then
			Return String.Empty
		Else
			If bExcludeAleph Then
				iAscMin = 224 ''''''''''''''''''''''225
			Else
				iAscMin = 224
			End If
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				Select Case iASCCode
					Case iAscMin To 250
						iASCCode -= 96
					Case 40
						iASCCode = 41
					Case 41
						iASCCode = 40
					Case 91
						iASCCode = 93
					Case 93
						iASCCode = 91
					Case 123
						iASCCode = 125
					Case 125
						iASCCode = 123
				End Select
				sOut = Chr(iASCCode) & sOut
			Next
			Return sOut
		End If
	End Function
	Public Shared Function InvertHeb(ByVal sValue As String) As String
		Dim iIndex As Integer
		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim bHebExists As Boolean
		sValue = Trim(sValue)
		If Len(sValue) = 0 Then
			Return ""
		Else

			For iIndex = 1 To Len(sValue)
				iASCCode = Asc(Mid(sValue, iIndex, 1))
				Select Case iASCCode
					Case 224 To 250
						bHebExists = True
						sOut = Chr(iASCCode - 96) & sOut

					Case 32, 34, 39
						If bHebExists Then
							sOut = Chr(iASCCode) & sOut
						Else
							sOut = sOut & Chr(iASCCode)
						End If

					Case Else
						sOut = sOut & Chr(iASCCode)
				End Select
			Next
			Return sOut
		End If
	End Function
	Public Shared Function Invert(ByVal sValue As String) As String
		Dim iIndex As Integer
		Dim sOut As String = String.Empty
		Dim sChar As String

		sValue = Trim(sValue)
		If Len(sValue) = 0 Then
			Return ""
		Else
			For iIndex = 1 To Len(sValue)
				sChar = Mid(sValue, iIndex, 1)
				sOut = sChar & sOut
			Next
			Return sOut
		End If
	End Function

	Public Shared Function InvertLines(ByVal sValue As String) As String
		Dim sTest As String = ""
		Dim sOut As String = String.Empty
		Dim sHebWord As String = String.Empty
		Dim saLines() As String



		If sValue.Length = 0& Then
			Return String.Empty
		Else
			saLines = Strings.Split(sValue, "|*|")

			For iIndex As Integer = 0 To saLines.GetUpperBound(0)
				If sOut.Length <> 0 Then
					sOut &= vbCrLf
				End If
				sOut &= WordToDOS(saLines(iIndex), True)
				'sOut &= saLines(iIndex)
			Next


			Return sOut
		End If
	End Function
	Public Shared Function FromDOS(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer

		If sValue.Length = 0& Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To Len(sValue) - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				Select Case iASCCode
					Case 128 To 154
						sOut = Chr(iASCCode + 96) & sOut
					Case Else
						sOut = Chr(iASCCode) & sOut
				End Select
			Next
			Return sOut
		End If
	End Function

End Class
