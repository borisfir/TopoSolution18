Option Explicit On
Option Strict On

Public Class Hebrew
	Public Const MaxHebNum As Integer = 9999
	Public Shared Function ToDOS(ByVal sValue As String) As String
		Dim sTest As String = ""
		Dim sOut As String = String.Empty
		Dim sHebWord As String = String.Empty
		Dim sSymbol As String
		Dim iASCCode As Integer

		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				sSymbol = sValue.Substring(iIndex, 1)
				iASCCode = Asc(sSymbol)

				Select Case iASCCode
					Case 34, 39, 224 To 250
						sHebWord = sHebWord & sSymbol

						'' Case 40, 41

					Case Else
						sOut &= Chr(iASCCode) & WordToDOS(sHebWord, True)
						sTest &= sHebWord & vbCrLf
						sHebWord = String.Empty
				End Select
			Next
			'System.Windows.Forms.MessageBox.Show(sHebWord & vbCrLf & CStr(sValue.Length), "04_348")
			sOut = WordToDOS(sHebWord, True) & sOut
			sTest &= sHebWord & vbCrLf
			Return sOut
		End If
	End Function

	Public Shared Function ToDOS_UD(ByVal sValue As String) As String
		Dim sTest As String = ""
		Dim sOut As String = String.Empty
		Dim sHebWord As String = String.Empty
		Dim sEngWord As String = String.Empty

		Dim sSymbol As String
		Dim iASCCode As Integer

		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				sSymbol = sValue.Substring(iIndex, 1)
				iASCCode = Asc(sSymbol)

				Select Case iASCCode
					Case 34, 39, 224 To 250
						sHebWord = sHebWord & sSymbol

						'' Case 40, 41
						sOut = sOut & sEngWord
						sEngWord = String.Empty
					Case Else
						sEngWord = sEngWord & sSymbol

						sOut &= WordToDOS(sHebWord, True)
						sTest &= sHebWord & vbCrLf
						sHebWord = String.Empty
				End Select
			Next
			'	System.Windows.Forms.MessageBox.Show(sHebWord & vbCrLf & CStr(sValue.Length), "04_348")
			sOut = WordToDOS(sHebWord, True) & sOut
			sOut = sEngWord & sOut
			sTest &= sHebWord & vbCrLf
			Return sOut
		End If
	End Function
	Public Shared Function WinToAcadA(ByVal sValue As String, Optional bToWin As Boolean = False) As String
		Dim chaVal() As Char = sValue.ToCharArray()
		Dim chRes As Char
		Dim iCharCategory As System.Globalization.UnicodeCategory
		Dim sRes As String = String.Empty
		Dim sWord As String = String.Empty
		Dim iWordCategory As System.Globalization.UnicodeCategory

		'	Dim dNum As Double
		'	Dim bControl, bDigit, bHighSurrogate, bLetter, bLower, bNumber, bPunctuation, bSeparator, bSurrogate, bSymbol, bUpper, bWhiteSpace As Boolean
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))

			If sWord.Length <> 0 AndAlso iCharCategory <> iWordCategory Then
				sRes = sWord & sRes
				sWord = String.Empty
			End If
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherPunctuation
					chRes = chaVal(iIndex)
					sRes = Char.ToString(chRes) & sRes
					'System.Windows.Forms.MessageBox.Show(Char.ToString(chRes) & ":" & sRes, "04_218")
				Case Globalization.UnicodeCategory.OtherLetter
					If bToWin Then
						chRes = chaVal(iIndex)
					Else
						chRes = Chr(AscW(chaVal(iIndex)) - 1360)
					End If

					sWord = Char.ToString(chRes) & sWord
					iWordCategory = Globalization.UnicodeCategory.OtherLetter
				Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter, Globalization.UnicodeCategory.DecimalDigitNumber
					chRes = chaVal(iIndex)
					sWord = sWord & Char.ToString(chRes)

					iWordCategory = iCharCategory
				Case Else
					System.Windows.Forms.MessageBox.Show(iCharCategory.ToString() & ":" & sRes, "04_219")
			End Select



		Next
		sRes = sWord & sRes
		Return sRes
	End Function
	Private Enum enDirection
		NotSet
		LeftToRight
		RightToLeft
		Punctuation
	End Enum
	Public Shared Function WinToAcadB(ByVal sValue As String, Optional bToWin As Boolean = False) As String
		Dim chaVal() As Char = sValue.ToCharArray()
		Dim chRes As Char
		Dim iWordCategory As System.Globalization.UnicodeCategory
		Dim iCharCategory As System.Globalization.UnicodeCategory
		Dim sRes As String = String.Empty
		Dim sWord As String = String.Empty
		Dim iCharDirection As enDirection = enDirection.NotSet
		Dim iWordDirection As enDirection = enDirection.NotSet


		'	Dim dNum As Double
		'	Dim bControl, bDigit, bHighSurrogate, bLetter, bLower, bNumber, bPunctuation, bSeparator, bSurrogate, bSymbol, bUpper, bWhiteSpace As Boolean
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					iCharDirection = enDirection.RightToLeft
				Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter
					iCharDirection = enDirection.LeftToRight
				Case Else
					iCharDirection = enDirection.NotSet
			End Select

			If zzIsOtherDirection(iCharDirection, iWordDirection) Then
				sRes = sWord & sRes
				'	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
				sWord = String.Empty
				iWordDirection = iCharDirection
			End If
			If iWordDirection = enDirection.NotSet Then
				iWordDirection = iCharDirection
			End If
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					If bToWin Then
						chRes = chaVal(iIndex)
					Else
						chRes = Chr(AscW(chaVal(iIndex)) - 1360)
					End If
					'	sWord = Char.ToString(chRes) & sWord
					iWordCategory = Globalization.UnicodeCategory.OtherLetter
				Case Else
					chRes = chaVal(iIndex)
			End Select
			If iWordDirection = enDirection.RightToLeft Then
				sWord = Char.ToString(chRes) & sWord
			ElseIf iWordDirection = enDirection.LeftToRight Then
				sWord = sWord & Char.ToString(chRes)
			End If
		Next
		'	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
		sRes = sWord & sRes
		Return sRes
	End Function

	Public Shared Function WinToAcadC(ByVal sValue As String, Optional bToWin As Boolean = False) As String
		Dim chaVal() As Char = sValue.ToCharArray()
		Dim chRes As Char
		Dim iWordCategory As System.Globalization.UnicodeCategory
		Dim iCharCategory As System.Globalization.UnicodeCategory
		Dim sRes As String = String.Empty
		Dim sWord As String = String.Empty
		Dim iCharDirection As enDirection = enDirection.NotSet
		Dim iWordDirection As enDirection = enDirection.NotSet
        'Dim sPunctuation As String

		'	Dim dNum As Double
		'	Dim bControl, bDigit, bHighSurrogate, bLetter, bLower, bNumber, bPunctuation, bSeparator, bSurrogate, bSymbol, bUpper, bWhiteSpace As Boolean
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					iCharDirection = enDirection.RightToLeft
				Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter, Globalization.UnicodeCategory.DecimalDigitNumber
					iCharDirection = enDirection.LeftToRight
				Case Globalization.UnicodeCategory.OtherPunctuation
					iCharDirection = enDirection.Punctuation
				Case Else
					iCharDirection = enDirection.NotSet
			End Select

			If zzIsOtherDirection(iCharDirection, iWordDirection) Then
				Select Case iWordDirection
					Case enDirection.LeftToRight, enDirection.RightToLeft
						sRes = sWord & sRes
					Case enDirection.Punctuation
						If iCharDirection = enDirection.RightToLeft Then
							sRes = sWord & sRes
						Else
							sRes = sWord & sRes
						End If
				End Select

				If iWordDirection = enDirection.LeftToRight Then

				End If

				'	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
				sWord = String.Empty
				iWordDirection = iCharDirection
			End If
			If iWordDirection = enDirection.NotSet Then
				iWordDirection = iCharDirection
			End If
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					If bToWin Then
						chRes = chaVal(iIndex)
					Else
						chRes = Chr(AscW(chaVal(iIndex)) - 1360)
					End If
					'	sWord = Char.ToString(chRes) & sWord
					iWordCategory = Globalization.UnicodeCategory.OtherLetter
				Case Else
					chRes = chaVal(iIndex)
			End Select
			If iWordDirection = enDirection.RightToLeft Then
				sWord = Char.ToString(chRes) & sWord
			ElseIf iWordDirection = enDirection.LeftToRight Then
				sWord = sWord & Char.ToString(chRes)
			ElseIf iWordDirection = enDirection.Punctuation Then
				sWord = Char.ToString(chRes) & sWord
			End If
		Next
		'	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
		sRes = sWord & sRes
		Return sRes
	End Function
	Public Shared Function WinToAcadD(ByVal sValue As String, Optional bToWin As Boolean = False) As String
		Dim chaVal() As Char = sValue.ToCharArray()
		Dim chRes As Char
		Dim iWordCategory As System.Globalization.UnicodeCategory
		Dim iCharCategory As System.Globalization.UnicodeCategory
		Dim sRes As String = String.Empty
		Dim sWord As String = String.Empty
		Dim iCharDirection As enDirection = enDirection.NotSet
		Dim iWordDirection As enDirection = enDirection.NotSet
        '	Dim sPunctuation As String

		'	Dim dNum As Double
		'	Dim bControl, bDigit, bHighSurrogate, bLetter, bLower, bNumber, bPunctuation, bSeparator, bSurrogate, bSymbol, bUpper, bWhiteSpace As Boolean
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					iCharDirection = enDirection.RightToLeft
				Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter
					iCharDirection = enDirection.LeftToRight
				Case Globalization.UnicodeCategory.OtherPunctuation
					iCharDirection = enDirection.Punctuation
				Case Else
					iCharDirection = enDirection.NotSet
			End Select

			If zzIsOtherDirection(iCharDirection, iWordDirection) Then
				Select Case iWordDirection
					Case enDirection.LeftToRight, enDirection.RightToLeft
						sRes = sWord & sRes
					Case enDirection.Punctuation
						If iCharDirection = enDirection.RightToLeft Then
							sRes = sRes & sWord
						Else
							sRes = sRes & sWord
						End If
				End Select

				If iWordDirection = enDirection.LeftToRight Then

				End If

				'	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
				sWord = String.Empty
				iWordDirection = iCharDirection
			End If
			If iWordDirection = enDirection.NotSet Then
				iWordDirection = iCharDirection
			End If
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					If bToWin Then
						chRes = chaVal(iIndex)
					Else
						chRes = Chr(AscW(chaVal(iIndex)) - 1360)
					End If
					'	sWord = Char.ToString(chRes) & sWord
					iWordCategory = Globalization.UnicodeCategory.OtherLetter
				Case Else
					chRes = chaVal(iIndex)
			End Select
			If iWordDirection = enDirection.RightToLeft Then
				sWord = Char.ToString(chRes) & sWord
			ElseIf iWordDirection = enDirection.LeftToRight Then
				sWord = sWord & Char.ToString(chRes)
			ElseIf iWordDirection = enDirection.Punctuation Then
			End If
		Next
		'	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
		sRes = sWord & sRes
		Return sRes
	End Function
	Private Shared Function zzIsOtherDirection1(iDirection1 As enDirection, iDirection2 As enDirection) As Boolean
		Return (iDirection1 = enDirection.LeftToRight AndAlso iDirection2 = enDirection.RightToLeft) OrElse (iDirection1 = enDirection.RightToLeft AndAlso iDirection2 = enDirection.LeftToRight)
	End Function
	Private Shared Function zzIsOtherDirection(iDirection1 As enDirection, iDirection2 As enDirection) As Boolean
		Return (iDirection1 = enDirection.Punctuation) OrElse (iDirection2 = enDirection.Punctuation) OrElse zzIsOtherDirection1(iDirection1, iDirection2)

	End Function
    Public Shared Function zzParseCharAAA(ByVal sValue As String) As String
        Dim chaVal() As Char = sValue.ToCharArray()
        Dim iCat As System.Globalization.UnicodeCategory
        Dim dNum As Double
        Dim bControl, bDigit, bHighSurrogate, bLetter, bLower, bNumber, bPunctuation, bSeparator, bSurrogate, bSymbol, bUpper, bWhiteSpace As Boolean
        Dim iAsc, iAscW As Integer
        For iIndex As Integer = 0 To chaVal.GetUpperBound(0)
            iCat = Char.GetUnicodeCategory(chaVal(iIndex))

            bControl = Char.IsControl(chaVal(iIndex))
            bDigit = Char.IsDigit(chaVal(iIndex))
            bHighSurrogate = Char.IsHighSurrogate(chaVal(iIndex))
            bLetter = Char.IsLetter(chaVal(iIndex))
            bLower = Char.IsLower(chaVal(iIndex))
            Char.IsLowSurrogate(chaVal(iIndex))
            bNumber = Char.IsNumber(chaVal(iIndex))
            bPunctuation = Char.IsPunctuation(chaVal(iIndex))
            bSeparator = Char.IsSeparator(chaVal(iIndex))
            bSurrogate = Char.IsSurrogate(chaVal(iIndex))
            bSymbol = Char.IsSymbol(chaVal(iIndex))
            bUpper = Char.IsUpper(chaVal(iIndex))
            bWhiteSpace = Char.IsWhiteSpace(chaVal(iIndex))
            dNum = Char.GetNumericValue(chaVal(iIndex))

            iAsc = Asc(chaVal(iIndex))
            iAscW = AscW(chaVal(iIndex))



            Stop
        Next
        Return Nothing
    End Function
	Public Shared Function WordToUnicode(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim iAscMin As Integer
		If sValue Is Nothing OrElse sValue.Length = 0 Then
			Return String.Empty
		Else

			iAscMin = 224 ''''''''''''''''''''''225

			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				Select Case iASCCode
					Case iAscMin To 250
						iASCCode -= 96
						iASCCode += 1360
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
				sOut = ChrW(iASCCode) & sOut
			Next
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
	Public Shared Function WordToDOSInv(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim iAscMin As Integer = 129
		DMCommon.Debug.MsgBox("19_030", sValue)

		If String.IsNullOrEmpty(sValue) Then
			Return String.Empty
		Else
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
				Try
					sOut = sOut & Strings.Chr(iASCCode)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("19_031", sOut, iASCCode)
				End Try

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
	Public Shared Function InvertInt(ByVal iValue As Integer) As String
		Dim iRemainder As Integer = iValue
		Dim iQuotient As Integer

		Dim sResult As String = String.Empty
		iQuotient = iValue

		Do While iQuotient > 0
			iRemainder = iQuotient Mod 10
			iQuotient = (iQuotient - iRemainder) \ 10
			sResult = sResult & iRemainder.ToString()
		Loop
		Return sResult
		'= iValue Mod 10
		'Math.DivRem()
	End Function



	Public Shared Function Invert(ByVal sValue As String) As String
		Dim iIndex As Integer
		Dim sOut As String = String.Empty
		Dim sChar As String

		sValue = Trim(sValue)
		If Len(sValue) = 0 Then
			Return String.Empty
		Else
			For iIndex = 1 To Len(sValue)
				sChar = Mid(sValue, iIndex, 1)
				sOut = sChar & sOut
			Next
			Return sOut
		End If
	End Function
	Public Shared Function InvertList(ByVal sValue As String) As String
		Dim saGroups() As String = Split(sValue, ",")
		Dim sRes As String = String.Empty
		For iIndex As Integer = saGroups.GetUpperBound(0) To 0 Step -1
         '	zzInvertGroup(saGroups(iIndex))
			If sRes.Length <> 0 Then
				sRes &= ","
			End If
			sRes &= zzInvertGroup(saGroups(iIndex))
		Next
		Return sRes
	End Function
	Private Shared Function zzInvertGroup(ByVal sValue As String) As String
		Dim saElem() As String = Split(sValue, "-")
		Select Case saElem.GetUpperBound(0)
			Case 0
				Return sValue.Trim()
			Case 1
				Return saElem(1).Trim() & "-" & saElem(0).Trim()
			Case Else
				Return sValue
		End Select
		 
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
   Public Shared Function InsBracketsList(ByVal sValue As String) As String
      Dim sRes As String = String.Empty
      If Not String.IsNullOrEmpty(sValue) Then
         Dim saGroups() As String = Split(sValue, ",")
         For iIndex As Integer = 0 To saGroups.GetUpperBound(0)
            If sRes.Length <> 0 Then
               sRes &= ","
            End If
            sRes &= zzInsBracketsGroup(saGroups(iIndex))
         Next
      End If
      Return sRes
   End Function
   Private Shared Function zzInsBracketsGroup(ByVal sValue As String) As String
      Dim saElem() As String = Split(sValue, "-")
      Select Case saElem.GetUpperBound(0)
         Case 0
            Return zzInsBracketsNum(sValue.Trim())
         Case 1
            Return zzInsBracketsNum(saElem(0).Trim()) & "-" & zzInsBracketsNum(saElem(1).Trim())
         Case Else
            Return sValue
      End Select
   End Function
   Private Shared Function zzInsBracketsNum(ByVal sValue As String) As String
      Return "[" & sValue & "]"
   End Function
	Public Shared Function ToUnicode(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iAscCode As Integer
		Dim sChar, sCharW As String
		Dim sCharOut As String

		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				sChar = sValue.Substring(iIndex, 1)
				iAscCode = Asc(sChar)
				Select Case iAscCode
					Case 128 To 154
						sCharW = Chr(iAscCode + 96)
					Case Else
						sCharW = sChar
				End Select
				sCharOut = ChrW(AscW(sCharW))
				sOut &= sCharOut
			Next
			Return sOut
		End If
	End Function
	Public Shared Function FromAcad(ByVal sValue As String) As String
		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim iASCCodeW As Integer

		Dim chChar As Char

		If String.IsNullOrEmpty(sValue) Then
			Return (String.Empty)
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Chars(iIndex))
				iASCCodeW = AscW(sValue.Chars(iIndex))
				'	s1 = "*" & CStr(iIndex) & "_" & CStr(iASCCode) & "_" & CStr(iASCCodeW)
				Select Case iASCCode
					Case 128 To 154
						chChar = ChrW(iASCCode + 1360)
					Case 63

						Select Case iASCCodeW
							Case &H161
								chChar = ChrW(154 + 1360)
							Case &H152
								chChar = ChrW(140 + 1360)
							Case &H17D
								chChar = ChrW(142 + 1360)
							Case Else
								chChar = sValue.Chars(iIndex)
								'1514
						End Select
					Case Else
						chChar = sValue.Chars(iIndex)
				End Select

				sOut = chChar & sOut
			Next
			
		End If
	 
		Return sOut
	End Function
	Public Shared Function FromAcadA(ByVal sValue As String) As String
		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim iASCCodeW As Integer
		Dim s1, sMsg As String
		Dim chChar As Char
		Dim bRtoL As Boolean
		sMsg = ""

		If String.IsNullOrEmpty(sValue) Then
			Return (String.Empty)
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Chars(iIndex))
				iASCCodeW = AscW(sValue.Chars(iIndex))
				s1 = "*" & CStr(iIndex) & "_" & CStr(iASCCode) & "_" & CStr(iASCCodeW)
				If iIndex = 0 Then
					sMsg = s1
				Else
					sMsg &= vbCrLf & s1
				End If
				Select Case iASCCode
					Case 128 To 154
						chChar = ChrW(iASCCode + 1360)
						bRtoL = True
					Case 63

						Select Case iASCCodeW
							Case &H152
								chChar = ChrW(140 + 1360)
							Case &H17D
								chChar = ChrW(142 + 1360)
							Case Else
								chChar = sValue.Chars(iIndex)
						End Select
						bRtoL = False
					Case Else
						chChar = sValue.Chars(iIndex)
						bRtoL = False
				End Select
				If bRtoL Then
					sOut = chChar & sOut
				Else
					sOut = sOut & chChar
				End If

			Next

		End If
		'	System.Windows.Forms.MessageBox.Show(sMsg, "04_343")
		Return sOut
	End Function

	Public Shared Function DispASC(ByVal sValue As String, bMsg As Boolean) As String

		Dim sOut As String = String.Empty
		Dim sOutW As String = String.Empty

		Dim iASCCode As Integer
		Dim iASCWCode As Integer

		Dim sTest As String = ""
		If sValue.Length <> 0 Then
			 
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				iASCWCode = AscW(sValue.Substring(iIndex, 1))


				sOut &= "|" & CStr(iASCCode)
				sOutW &= "|" & CStr(iASCWCode)

				 
			Next


		End If
		If bMsg Then
			System.Windows.Forms.MessageBox.Show(sOut & vbCrLf & sOutW, "04_217")
		End If

		Return sOut
	End Function
	Public Shared Function FromDOS(ByVal sValue As String, bUnicode As Boolean) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		Dim sTest As String = ""
		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				sTest &= "|" & CStr(iASCCode)
				Select Case iASCCode
					Case 128 To 154
						sOut = zzMyChr(iASCCode + 96, bUnicode) & sOut
					Case Else
						sOut = zzMyChr(iASCCode, bUnicode) & sOut
				End Select
			Next
			'	System.Windows.Forms.MessageBox.Show(sTest, "04_217")
			Return sOut
		End If
	End Function
	Public Shared Function Spell(ByVal sValue As String) As String

		Dim sOut As String = String.Empty
		Dim iASCCode As Integer

		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				iASCCode = Asc(sValue.Substring(iIndex, 1))
				If sOut.Length <> 0 Then
					sOut &= ","
				End If
				sOut &= Convert.ToString(iASCCode)
			Next
			Return sOut
		End If
	End Function
	Public Shared Function WordToKeyboard(ByVal sValue As String, bInvert As Boolean) As String
		Dim sOut As String = String.Empty
		Dim sLetterIn As String
		Dim sLetterOut As String
		Dim sNumber As String = String.Empty

		Dim bInSet As Boolean
		Dim bDigit As Boolean
		If sValue Is Nothing OrElse sValue.Length = 0 Then
			Return String.Empty
		Else
			'DMCommon.ExcelLogAW5.SetNextValue(0, "WordToKeyboard", sValue)
			For iIndex As Integer = 0 To sValue.Length - 1
				bDigit = False
				sLetterIn = sValue.Substring(iIndex, 1)
				bInSet = True
				Select Case sLetterIn
					Case "א"
						sLetterOut = "t"
					Case "ב"
						sLetterOut = "c"
					Case "ג"
						sLetterOut = "d"
					Case "ד"
						sLetterOut = "s"
					Case "ה"
						sLetterOut = "v"

					Case "ו"
						sLetterOut = "u"
					Case "ז"
						sLetterOut = "z"
					Case "ח"
						sLetterOut = "j"
					Case "ט"
						sLetterOut = "y"
					Case "י"
						sLetterOut = "h"

					Case "ך"
						sLetterOut = "l"
					Case "כ"
						sLetterOut = "f"
					Case "ל"
						sLetterOut = "k"
					Case "ם"
						sLetterOut = "o"
					Case "מ"
						sLetterOut = "n"

					Case "ן"
						sLetterOut = "i"
					Case "נ"
						sLetterOut = "b"
					Case "ס"
						sLetterOut = "x"
					Case "ע"
						sLetterOut = "g"
					Case "ף"
						sLetterOut = ";"

					Case "פ"
						sLetterOut = "p"
					Case "ץ"
						sLetterOut = "."
					Case "צ"
						sLetterOut = "m"
					Case "ק"
						sLetterOut = "e"
					Case "ר"
						sLetterOut = "r"

					Case "ש"
						sLetterOut = "a"
					Case "ת"
						sLetterOut = ","
					Case ","
						sLetterOut = "/"
					Case "."
						sLetterOut = "/"
					Case "'"
						sLetterOut = "w"
					Case "/"
						sLetterOut = "q"
					Case "\", "-"
						sLetterOut = sLetterIn
					Case "0" To "9"
						sLetterOut = sLetterIn
						bDigit = True
					Case Else

						sLetterOut = sLetterIn
						bInSet = False
				End Select
				If bDigit Then
					sNumber = sLetterOut & sNumber
				Else
					If sNumber.Length > 0 Then
						If bInvert Then
							sOut = sNumber & sOut
						Else
							sOut = sOut & sNumber
						End If
						sNumber = String.Empty
					End If

					If bInvert And bInSet Then
						sOut = sLetterOut & sOut
					Else
						sOut = sOut & sLetterOut
					End If
				End If

				'DMCommon.ExcelLogAW5.SetNextValue(2, "WordToK", sLetterOut, sNumber, sOut)
			Next
			If sNumber.Length > 0 Then
				If bInvert Then
					sOut = sNumber & sOut
				Else
					sOut = sOut & sNumber
				End If

			End If

			Return sOut
		End If
	End Function
	Public Shared Function GetHebNum(ByVal iInput As Integer, bUnicode As Boolean, Optional bDOS As Boolean = False) As String
		Const sP1 As String = "'"
		Const sP0 As String = ""
		Const sP2 As String = """"

		Dim sOutput As String = String.Empty
		Dim iThousands As Integer
		Dim sThousands As String = String.Empty
		Dim iThousandsRetVal As Integer
		If False Then
			If bUnicode Then
				Return CStr(iInput + 10000)
			Else
				Return CStr(iInput)
			End If
		End If
		iThousands = iInput \ 1000

		If iThousands > 0 AndAlso iThousands <= 9 Then
			zzGetHebLetter(iThousands, bUnicode, sThousands, iThousandsRetVal)

			sThousands &= sP1

			iInput = iInput - 1000 * iThousandsRetVal
		End If
		If iInput > 0 AndAlso iInput <= MaxHebNum Then
			Dim sHeb As String = String.Empty
			Dim sLeft, sRight As String
			Dim iLen As Integer
			Dim iRetVal As Integer
			Do
				zzGetHebLetter(iInput, bUnicode, sHeb, iRetVal)
				iInput = iInput - iRetVal
				sOutput &= sHeb
			Loop While iInput > 0
			iLen = sOutput.Length
			If iLen = 1 Then
				'sOutput = sHeb & sP1 211024
				sOutput = sHeb
			Else
				sLeft = Strings.Left(sOutput, sOutput.Length - 1)
				sRight = Strings.Right(sOutput, 1)
				If bDOS Then
					sOutput = sRight & sP2 & sLeft
				Else
					sOutput = sLeft & sP2 & sRight
				End If

			End If
			sOutput = sThousands & sOutput
		ElseIf iInput = 0 AndAlso iThousands > 0 Then
			sOutput = sThousands
		End If


		Return sOutput
	End Function
	Public Shared Function GetHebNum_201024(ByVal iInput As Integer, bUnicode As Boolean, Optional bDOS As Boolean = False) As String

		Dim sOutput As String = String.Empty
		If False Then
			If bUnicode Then
				Return CStr(iInput + 10000)
			Else
				Return CStr(iInput)
			End If
		End If


		If iInput > 0 AndAlso iInput <= MaxHebNum Then
			Const sP1 As String = "'"
			Const sP2 As String = """"

			Dim sHeb As String = String.Empty
			Dim sLeft, sRight As String
			Dim iLen As Integer
			Dim iRetVal As Integer
			Do
				zzGetHebLetter(iInput, bUnicode, sHeb, iRetVal)
				iInput = iInput - iRetVal
				sOutput &= sHeb
			Loop While iInput > 0
			iLen = sOutput.Length
			If iLen = 1 Then
				sOutput = sHeb & sP1
			Else
				sLeft = Strings.Left(sOutput, sOutput.Length - 1)
				sRight = Strings.Right(sOutput, 1)
				If bDOS Then
					sOutput = sRight & sP2 & sLeft
				Else
					sOutput = sLeft & sP2 & sRight
				End If

			End If
		End If
		Return sOutput
	End Function
	Public Shared Function GetHebNumRev(ByVal iInput As Integer, bUnicode As Boolean, Optional bDOS As Boolean = False) As String

		Dim sOutput As String = String.Empty
		If False Then
			If bUnicode Then
				Return CStr(iInput + 10000)
			Else
				Return CStr(iInput)
			End If
		End If


		If iInput > 0 AndAlso iInput <= MaxHebNum Then
			Const sP1 As String = "'"
			Const sP2 As String = """"

			Dim sHeb As String = String.Empty
			Dim sLeft, sRight As String
			Dim iLen As Integer
			Dim iRetVal As Integer
			Do
				zzGetHebLetter(iInput, bUnicode, sHeb, iRetVal)
				iInput = iInput - iRetVal
				sOutput &= sHeb
			Loop While iInput > 0
			iLen = sOutput.Length
			If iLen = 1 Then
				sOutput = sP1 & sHeb
			Else
				sLeft = Strings.Left(sOutput, sOutput.Length - 1)
				sRight = Right(sOutput, 1)
				If bDOS Then
					sOutput = sRight & sP2 & sLeft
				Else
					sOutput = sLeft & sP2 & sRight
				End If

			End If
		End If
		Return sOutput
	End Function
	Public Shared Function ToASCCode(ByVal sValue As String, bAscW As Boolean) As String
		Dim sOut As String = String.Empty
		Dim iASCCode As Integer
		'   Dim sTest As String = ""
		If sValue.Length = 0 Then
			Return String.Empty
		Else
			For iIndex As Integer = 0 To sValue.Length - 1
				If bAscW Then
					iASCCode = AscW(sValue.Substring(iIndex, 1))
				Else
					iASCCode = Asc(sValue.Substring(iIndex, 1))
				End If
				If sOut.Length = 0 Then
					sOut &= CStr(iASCCode)
				Else
					sOut &= "|" & CStr(iASCCode)
				End If


			Next
			'	System.Windows.Forms.MessageBox.Show(sTest, "04_217")
			Return sOut
		End If

	End Function
	Private Shared Sub zzGetHebLetter(ByVal iInput As Integer, bUnicode As Boolean, ByRef sHeb As String, ByRef iVal As Integer)
		Dim iBase As Integer
		Dim iDec As Integer, iHundred As Integer
		Dim iAddit As Integer = 0
		iBase = zzMyAsc("א", bUnicode) - 1


		If iInput <= 10 Then
			sHeb = zzMyChr(iBase + iInput, bUnicode)
			iVal = iInput
		ElseIf iInput = 15 OrElse iInput = 16 Then
			sHeb = zzMyChr(iBase + 9, bUnicode) & zzMyChr(iBase + iInput - 9, bUnicode)
			iVal = iInput
		ElseIf iInput < 100 Then
			iDec = iInput \ 10
			If iDec >= 9 Then
				iAddit = 5
			ElseIf iDec >= 8 Then
				iAddit = 4
			ElseIf iDec >= 5 Then
				iAddit = 3
			ElseIf iDec >= 4 Then
				iAddit = 2
			ElseIf iDec >= 2 Then
				iAddit = 1
			End If
			sHeb = zzMyChr(iBase + 9 + iDec + iAddit, bUnicode)
			iVal = iDec * 10
		Else
			iHundred = iInput \ 100
			If iHundred > 4 Then
				iHundred = 4
			End If
			sHeb = zzMyChr(iBase + 23 + iHundred + iAddit, bUnicode)
			iVal = iHundred * 100
		End If
	End Sub


	Public Shared Function GetBaseNum(iCode As Integer) As Integer	'ללא אתיות סופיות
		Dim iAdj As Integer
		Select Case iCode
			Case 0 To 9	 ' א-י
				iAdj = 0
			Case 11, 12	' כ,ל
				iAdj = 1
			Case 14	' מ
				iAdj = 2
			Case 16 To 18 ' נ-ע
				iAdj = 3
			Case 20	  'פ
				iAdj = 4
			Case 22 To 26	'צ-ת
				iAdj = 5
			Case Else
				Return -1
		End Select

		Return (iCode - iAdj)

	End Function
	Private Shared Function zzMyAsc(sValue As String, bUnicode As Boolean) As Integer
		If bUnicode Then
			Return AscW(sValue)
		Else
			Return Asc(sValue)
		End If

	End Function
	Private Shared Function zzMyChr(iCharCode As Integer, bUnicode As Boolean) As String
		If bUnicode Then
			Return ChrW(iCharCode)
		Else
			Return Chr(iCharCode)
		End If

	End Function

End Class
