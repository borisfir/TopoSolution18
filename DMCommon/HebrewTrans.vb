Option Explicit On
Option Strict On
Public Class HebrewTrans
	Private Enum enDirection
		NotSet
		LeftToRight
		RightToLeft
		Punctuation
	End Enum
	Private Enum enCharType
		NotSet
		Hebrew
		English
		DigitDec
		Punctuation
		DigitSymbol
	End Enum
	Private Enum enInputSource
		FormRtoL
		FormLtoR
	End Enum
	Private msSourceText As String
	Private mcolWords As System.Collections.ObjectModel.Collection(Of Word)
	Private mcolSnippets As System.Collections.ObjectModel.Collection(Of Snippet)

	Private moCurrentWord As Word
	Private miCurrentDirection As enDirection
	Private mbSourceIsWin As Boolean
   Public Sub New(sText As String, bSourceIsWin As Boolean, Optional bIsCode As Boolean = False)
      If bIsCode Then
         Dim saASCCodes() As String = Split(sText, "|")
         msSourceText = String.Empty
         For iIndex As Integer = 0 To saASCCodes.GetUpperBound(0)
            msSourceText &= Chr(Convert.ToInt32(saASCCodes(iIndex)))
         Next
      Else
         msSourceText = sText
      End If

      mbSourceIsWin = bSourceIsWin
      If mbSourceIsWin Then
         zzInputWin()
      Else
         zzInputDos()
      End If

   End Sub
	Public Function MLineText() As String
		Dim oWord As Word
		Dim sRes As String = Nothing
		For iIndex As Integer = 0 To mcolWords.Count - 1
			If sRes IsNot Nothing Then
				sRes &= vbCrLf
			End If
			oWord = mcolWords.Item(iIndex)
			sRes &= oWord.Text
		Next
		Return sRes
	End Function
	Public Function MultiLineSource() As String
		Dim oWord As Snippet
		Dim sRes As String = Nothing
		For iIndex As Integer = 0 To mcolSnippets.Count - 1
			If sRes IsNot Nothing Then
				sRes &= vbCrLf
			End If
			oWord = mcolSnippets.Item(iIndex)
			sRes &= oWord.GetText(True, True)
		Next
		Return sRes
	End Function
	Public Function GetWinDest(bMultiLine As Boolean) As String
		Dim oSnippet As Snippet
		Dim oSegment As Segment = Nothing
		Dim sRes As String = String.Empty
		Dim sDelim As String
		If bMultiLine Then
			sDelim = vbCrLf
		Else
			sDelim = String.Empty
		End If
		For iIndex As Integer = 0 To mcolSnippets.Count - 1
			oSnippet = mcolSnippets.Item(iIndex)
			If iIndex = 0 Then
				oSegment = New Segment(mbSourceIsWin, True, True, oSnippet)
			Else
				If oSegment.TryAdd(oSnippet) Then
				Else
					If sRes.Length <> 0 Then
						sRes &= sDelim
					End If
					sRes &= oSegment.Text
					oSegment = New Segment(mbSourceIsWin, True, True, oSnippet)
				End If
			End If


		Next
		If oSegment IsNot Nothing Then
			oSegment.Close()
			If sRes IsNot Nothing AndAlso sRes.Length <> 0 Then
				sRes &= sDelim
			End If
         sRes &= oSegment.Text
         ' sRes = oSegment.Text & sRes

		End If
		Return sRes
	End Function
	Public Function MultiLineDest() As String

		Dim oSnippet As Snippet
		Dim oSegment As Segment = Nothing

		Dim sRes As String = String.Empty
		For iIndex As Integer = 0 To mcolSnippets.Count - 1
			oSnippet = mcolSnippets.Item(iIndex)
			If iIndex = 0 Then
				oSegment = New Segment(mbSourceIsWin, True, True, oSnippet)
			Else
				If oSegment.TryAdd(oSnippet) Then
				Else
					If sRes.Length <> 0 Then
						sRes &= vbCrLf
					End If
					sRes &= oSegment.Text
					oSegment = New Segment(mbSourceIsWin, True, True, oSnippet)
				End If
			End If


		Next
		If oSegment IsNot Nothing Then
			oSegment.Close()
			If sRes IsNot Nothing AndAlso sRes.Length <> 0 Then
				sRes &= vbCrLf
			End If
			sRes &= oSegment.Text
		End If
		Return sRes
	End Function
	Public Function GetDOSDest() As String
		Dim sRes As String = String.Empty
		If mcolSnippets IsNot Nothing Then
			Dim oSnippet As Snippet
			Dim oSegment As Segment = Nothing


			For iIndex As Integer = 0 To mcolSnippets.Count - 1
				oSnippet = mcolSnippets.Item(iIndex)
				If iIndex = 0 Then
					oSegment = New Segment(mbSourceIsWin, False, False, oSnippet)
				Else
					If oSegment.TryAdd(oSnippet) Then
					Else
						If sRes.Length <> 0 Then
							'sRes &= vbCrLf
						End If
						sRes = oSegment.Text & sRes
						oSegment = New Segment(mbSourceIsWin, False, False, oSnippet)
					End If
				End If


			Next
			If oSegment IsNot Nothing Then
				oSegment.Close()

				sRes = oSegment.Text & sRes
			End If

		End If

		Return sRes
	End Function
	Public Function GetDOSDestInv() As String
		Dim sRes As String = String.Empty
		If mcolSnippets IsNot Nothing Then
			Dim oSnippet As Snippet
			Dim oSegment As Segment = Nothing


			For iIndex As Integer = 0 To mcolSnippets.Count - 1
				oSnippet = mcolSnippets.Item(iIndex)
				If iIndex = 0 Then
					oSegment = New Segment(mbSourceIsWin, False, False, oSnippet)
				Else
					If oSegment.TryAdd(oSnippet) Then
					Else
						If sRes.Length <> 0 Then
							'sRes &= vbCrLf
						End If
						sRes &= oSegment.Text
						oSegment = New Segment(mbSourceIsWin, False, False, oSnippet)
					End If
				End If
			Next
			If oSegment IsNot Nothing Then
				oSegment.Close()

				sRes = oSegment.Text & sRes
			End If

		End If
		Return sRes
	End Function

	Private Function zzCharToWin(chValue As Char) As Char
		Dim iASCCode As Integer
		Dim iASCCodeW As Integer
		Dim iCharCode As Integer = Asc(chValue)
		iASCCode = Asc(chValue)
		iASCCodeW = AscW(chValue)

		Select Case iASCCode
			Case 128 To 154
				Return ChrW(iASCCode + 1360)
			Case 63

				Select Case iASCCodeW
					Case &H161
						Return ChrW(154 + 1360)
					Case &H152
						Return ChrW(140 + 1360)
					Case &H17D
						Return ChrW(142 + 1360)
					Case Else
						Return chValue
				End Select
			Case Else
				Return chValue
		End Select


	End Function

	Private Sub zzInputA()

		Dim chaVal() As Char = msSourceText.ToCharArray()
        'Dim chRes As Char
        '	Dim iWordCategory As System.Globalization.UnicodeCategory
		Dim iCharCategory As System.Globalization.UnicodeCategory
		Dim sRes As String = String.Empty
		'	Dim sWord As String = String.Empty
		Dim oSnippet As Snippet = Nothing
		Dim iCharDirection As enDirection = enDirection.NotSet
		Dim iCharType As enCharType = enCharType.NotSet
		Dim bLastCharIsDigit As Boolean = False
		'	Dim iWordDirection As enDirection = enDirection.NotSet

		mcolSnippets = New System.Collections.ObjectModel.Collection(Of Snippet)()
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.OtherLetter
					iCharType = enCharType.Hebrew
					bLastCharIsDigit = False
				Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter
					iCharType = enCharType.English
					bLastCharIsDigit = False
				Case Globalization.UnicodeCategory.DecimalDigitNumber
					iCharType = enCharType.DigitDec
					bLastCharIsDigit = True
				Case Else
					Select Case chaVal(iIndex)
						Case "#"c, "$"c, "%"c
							If bLastCharIsDigit Then
								iCharType = enCharType.DigitDec
							Else
								iCharType = enCharType.Punctuation
							End If

						Case Else
							iCharType = enCharType.Punctuation
							bLastCharIsDigit = False
					End Select

			End Select
			If oSnippet Is Nothing Then
				oSnippet = New Snippet(chaVal(iIndex), iCharType)
				oSnippet.Direction = iCharDirection

			ElseIf iCharType <> oSnippet.Type Then
				mcolSnippets.Add(oSnippet)

				'	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
				oSnippet = New Snippet(chaVal(iIndex), iCharType)
				oSnippet.Direction = iCharDirection
				'
			Else
				oSnippet.Add(chaVal(iIndex), False)
			End If





			'
		Next
		'	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
		'
		If oSnippet IsNot Nothing Then
			mcolSnippets.Add(oSnippet)
		End If



	End Sub
	Private Sub zzInputWin()
		If Not String.IsNullOrEmpty(msSourceText) Then

			Dim chaVal() As Char = msSourceText.ToCharArray()
			'	Dim chRes As Char
			'	Dim iWordCategory As System.Globalization.UnicodeCategory
			Dim iCharCategory As System.Globalization.UnicodeCategory
			Dim sRes As String = String.Empty
			'	Dim sWord As String = String.Empty
			Dim oSnippet As Snippet = Nothing
			Dim iCharDirection As enDirection = enDirection.NotSet
			Dim iCharType As enCharType = enCharType.NotSet
			'	Dim bLastCharIsDigit As Boolean = False
			'	Dim iWordDirection As enDirection = enDirection.NotSet

			mcolSnippets = New System.Collections.ObjectModel.Collection(Of Snippet)()
			For iIndex As Integer = 0 To chaVal.GetUpperBound(0)

				iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
				Select Case iCharCategory
					Case Globalization.UnicodeCategory.OtherLetter
						iCharType = enCharType.Hebrew

					Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter
						iCharType = enCharType.English

					Case Globalization.UnicodeCategory.DecimalDigitNumber
						iCharType = enCharType.DigitDec

					Case Else
						Select Case chaVal(iIndex)
							Case "#"c, "$"c, "%"c
								iCharType = enCharType.DigitSymbol
							Case Else
								iCharType = enCharType.Punctuation

						End Select

				End Select
				If oSnippet Is Nothing Then
					oSnippet = New Snippet(chaVal(iIndex), iCharType)
					oSnippet.Direction = iCharDirection

				ElseIf iCharType <> oSnippet.Type Then
					mcolSnippets.Add(oSnippet)

					'	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
					oSnippet = New Snippet(chaVal(iIndex), iCharType)
					oSnippet.Direction = iCharDirection
					'
				Else
					oSnippet.Add(chaVal(iIndex), False)
				End If





				'
			Next
			'	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
			'
			If oSnippet IsNot Nothing Then
				mcolSnippets.Add(oSnippet)
			End If




		End If

	End Sub

   Private Sub zzInputDos()
      Dim i As Char = New Char()
      Dim chaVal() As Char = msSourceText.ToCharArray()
      Dim iCharCategory As System.Globalization.UnicodeCategory
      Dim sRes As String = String.Empty
      '	Dim sWord As String = String.Empty
      Dim oSnippet As Snippet = Nothing
      Dim iCharDirection As enDirection = enDirection.NotSet
      Dim iCharType As enCharType = enCharType.NotSet
      '	Dim iCharCode As Integer
      '	Dim bLastCharIsDigit As Boolean = False
      '	Dim iWordDirection As enDirection = enDirection.NotSet

      mcolSnippets = New System.Collections.ObjectModel.Collection(Of Snippet)()
      For iIndex As Integer = chaVal.GetUpperBound(0) To 0 Step -1
         'iCharCode = Asc(chaVal(iIndex))
         chaVal(iIndex) = zzCharToWin(chaVal(iIndex))
         iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
         Select Case iCharCategory
            Case Globalization.UnicodeCategory.OtherLetter
               iCharType = enCharType.Hebrew

            Case Globalization.UnicodeCategory.UppercaseLetter, Globalization.UnicodeCategory.LowercaseLetter
               iCharType = enCharType.English

            Case Globalization.UnicodeCategory.DecimalDigitNumber
               iCharType = enCharType.DigitDec

            Case Else
               Select Case chaVal(iIndex)
                  Case "#"c, "$"c, "%"c
                     iCharType = enCharType.DigitSymbol
                  Case Else
                     iCharType = enCharType.Punctuation

               End Select

         End Select
         If oSnippet Is Nothing Then
            oSnippet = New Snippet(chaVal(iIndex), iCharType)
            oSnippet.Direction = iCharDirection

         ElseIf iCharType <> oSnippet.Type Then
            mcolSnippets.Add(oSnippet)

            '	System.Windows.Forms.MessageBox.Show("'" & sRes & "'" & vbCrLf & "'" & sWord & "'" & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_234")
            oSnippet = New Snippet(chaVal(iIndex), iCharType)
            oSnippet.Direction = iCharDirection
            '
         Else

            oSnippet.Add(chaVal(iIndex), iCharType <> enCharType.Hebrew)
         End If





         '
      Next
      '	System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & sWord & vbCrLf & CStr(sWord.Length) & vbCrLf & iWordDirection.ToString(), "04_237")
      '
      If oSnippet IsNot Nothing Then
         mcolSnippets.Add(oSnippet)
      End If



   End Sub
	Private Shared Function zzIsOtherDirection1(iDirection1 As enDirection, iDirection2 As enDirection) As Boolean
		Return (iDirection1 = enDirection.LeftToRight AndAlso iDirection2 = enDirection.RightToLeft) OrElse (iDirection1 = enDirection.RightToLeft AndAlso iDirection2 = enDirection.LeftToRight)
	End Function
	Private Shared Function zzIsOtherDirection(iDirection1 As enDirection, iDirection2 As enDirection) As Boolean
		Return (iDirection1 = enDirection.Punctuation) OrElse (iDirection2 = enDirection.Punctuation) OrElse zzIsOtherDirection1(iDirection1, iDirection2)

	End Function
	Private Class Word


		Private miDirection As enDirection
		Private miType As enCharType

		Private msText As String
		Public Sub New()

		End Sub
		Public Sub New(chValue As Char, iType As enCharType)
			msText = Char.ToString(chValue)
			miType = iType
		End Sub
		Public Property Type As enCharType
			Get
				Return miType
			End Get
			Set(iValue As enCharType)
				miType = iValue
			End Set
		End Property

		Public Property Direction As enDirection
			Get
				Return miDirection
			End Get
			Set(iValue As enDirection)
				miDirection = iValue
			End Set
		End Property
		Public Property Text As String
			Get
				Return msText
			End Get
			Set(sValue As String)
				msText = sValue
			End Set
		End Property
		Public Sub AddLeft(chValue As Char)
			msText = Char.ToString(chValue) & msText
		End Sub
		Public Sub AddRight(chValue As Char)
			msText = msText & Char.ToString(chValue)
		End Sub
	End Class
	Private Class Snippet


		Private miDirection As enDirection
		Private miType As enCharType

		'	Private msText As String
		Private mcolChars As System.Collections.ObjectModel.Collection(Of Char)
		Public Sub New(chValue As Char, iType As enCharType)

			miType = iType
			mcolChars = New System.Collections.ObjectModel.Collection(Of Char)()
			Me.Add(chValue, False)
		End Sub
		Public Property Type As enCharType
			Get
				Return miType
			End Get
			Set(iValue As enCharType)
				miType = iValue
			End Set
		End Property

		Public Property Direction As enDirection
			Get
				Return miDirection
			End Get
			Set(iValue As enDirection)
				miDirection = iValue
			End Set
		End Property

		Public Sub Add(chValue As Char, bBackward As Boolean)
			If bBackward Then
				mcolChars.Insert(0, chValue)
			Else
				mcolChars.Add(chValue)
			End If

		End Sub
		Private Function zzGetLtoRText() As String
			Dim sRes As String = String.Empty
			For iIndex As Integer = 0 To mcolChars.Count - 1
				sRes &= Char.ToString(mcolChars.Item(iIndex))
			Next
			Return sRes
		End Function

		Private Function zzGetRtoLText() As String
			Dim sRes As String = String.Empty
			For iIndex As Integer = mcolChars.Count - 1 To 0 Step -1
				sRes = Char.ToString(mcolChars.Item(iIndex)) & sRes
			Next
			Return sRes
		End Function
		Private Function zzGetRtoLDOSText() As String
			Dim sRes As String = String.Empty
			For iIndex As Integer = mcolChars.Count - 1 To 0 Step -1
				'sRes = Char.ToString(Chr(AscW(mcolChars.Item(iIndex)) - 1360)) & sRes
				sRes &= Char.ToString(Chr(AscW(mcolChars.Item(iIndex)) - 1360))


			Next
			Return sRes
		End Function
		Public Function GetText(bWin As Boolean, bRtoL As Boolean) As String
			Select Case miType
				Case enCharType.Hebrew
					If bWin Then
						Return zzGetRtoLText()
					Else
						Return zzGetRtoLDOSText()
					End If

				Case enCharType.English
					Return zzGetLtoRText()
				Case enCharType.DigitDec
					Return zzGetLtoRText()
				Case enCharType.Punctuation
					If bRtoL Then
						Return zzGetRtoLText()
					Else
						Return zzGetLtoRText()
					End If
				Case Else
					Return String.Empty
			End Select
		End Function
		Public Overrides Function ToString() As String
			Return GetText(True, True)

		End Function
	End Class
	Private Class Segment
		Private Enum enSegmentType
			Hebrew
			English
			Punctuation
			DigitDec
		End Enum
		Private msText As String = String.Empty
		Private moLastSnippet As Snippet
		Private msVariableText As String = String.Empty
		Private miVariableType As enCharType
		Private miSegmentType As enSegmentType
		Private mbWin As Boolean
		Private mbRtoL As Boolean
		Private mbSourceIsWin As Boolean
		Public Sub New(bSourceIsWin As Boolean, bWin As Boolean, bRtoL As Boolean, oSnippet As Snippet)
			mbSourceIsWin = bSourceIsWin
			mbWin = bWin
			Select Case oSnippet.Type
				Case enCharType.Hebrew
					miSegmentType = enSegmentType.Hebrew
				Case enCharType.Punctuation
					miSegmentType = enSegmentType.Punctuation
				Case enCharType.DigitDec
					miSegmentType = enSegmentType.DigitDec
			End Select
			msText = oSnippet.GetText(mbWin, bRtoL)
		End Sub
		Public Property Text As String
			Get
				Return msText
			End Get
			Set(sValue As String)
				msText = sValue
			End Set
		End Property
		Public Function TryAdd(oSnippet As Snippet) As Boolean
			If miSegmentType = enSegmentType.Hebrew OrElse miSegmentType = enSegmentType.Punctuation OrElse oSnippet.Type = enCharType.Hebrew Then
				Close()
				Return False
			ElseIf miSegmentType = enSegmentType.DigitDec Then
				If oSnippet.Type = enCharType.Punctuation Then
					msVariableText = oSnippet.GetText(mbWin, mbRtoL)
					miVariableType = enCharType.Punctuation
				ElseIf oSnippet.Type = enCharType.DigitSymbol Then
					If miVariableType = enCharType.Punctuation Then
						Close()
						Return False
					ElseIf mbSourceIsWin Then
						msText &= oSnippet.GetText(mbWin, mbRtoL)
					Else
						msText = oSnippet.GetText(mbWin, mbRtoL) & msText
					End If
				ElseIf oSnippet.Type = enCharType.English Then
					If msVariableText.Length = 0 Then
						If mbSourceIsWin Then
							msText &= oSnippet.GetText(mbWin, mbRtoL)
						Else
							msText = oSnippet.GetText(mbWin, mbRtoL) & msText
						End If

					Else
						Close()
						Return False
					End If
				ElseIf oSnippet.Type = enCharType.DigitDec Then
					If msVariableText.Length = 0 Then
						If mbSourceIsWin Then
							msText &= oSnippet.GetText(mbWin, mbRtoL)
						Else
							msText = oSnippet.GetText(mbWin, mbRtoL) & msText
						End If

					Else
						If mbSourceIsWin Then
							msText &= msVariableText & oSnippet.GetText(mbWin, mbRtoL)
						Else
							msText = oSnippet.GetText(mbWin, mbRtoL) & msVariableText & msText
						End If
						msVariableText = String.Empty
					End If
				End If
			ElseIf miSegmentType = enSegmentType.English Then
				If oSnippet.Type = enCharType.Punctuation OrElse oSnippet.Type = enCharType.DigitSymbol Then
					''???????????????????????????
					msVariableText &= oSnippet.GetText(mbWin, mbRtoL)
				ElseIf oSnippet.Type = enCharType.DigitDec OrElse oSnippet.Type = enCharType.English Then
					If mbSourceIsWin Then
						msText &= msVariableText & oSnippet.GetText(mbWin, mbRtoL)
					Else
						msText = msVariableText & oSnippet.GetText(mbWin, mbRtoL) & msText
					End If
					msVariableText = String.Empty
				End If
			End If
			Return True
		End Function
		Public Sub AddLeft(sText As String)
			msText = sText & msText
		End Sub
		Public Sub Add(oSnippet As Snippet)
			If moLastSnippet.Type = enCharType.Punctuation AndAlso (oSnippet.Type = enCharType.DigitDec OrElse oSnippet.Type = enCharType.English) Then

				msText = msText & moLastSnippet.GetText(mbWin, mbRtoL)
			End If
			moLastSnippet = oSnippet
			'	msText = sText & msText
		End Sub
		Public Sub Close()
			msText = msVariableText & msText
		End Sub
	End Class

End Class
