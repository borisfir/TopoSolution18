Option Explicit On
Module modHeb


   Const lhbEnglish As Integer = 0&, lhbDigit As Integer = 1&, lhbMark As Integer = 2&, lhbHebrew As Integer = 3&
   Const iDirNeutral As Integer = 0&, iDirLeftToRight As Integer = 1&, iDirRightToLeft As Integer = 2&
   Public Const iWindowsToDos As Integer = -1, lNeutral As Integer = 0, lDosToWindows As Integer = 1

   Private Function InvertAAA(ByVal Source As String) As String

      Dim tempStr As String = String.Empty, i As Integer

      For i = Source.Length To 1 Step -1
         tempStr &= Mid(Source, i, 1)
      Next
      Return tempStr
   End Function

   Public Function InvertWX(ByVal sVal As String, ByVal iWinDOS As Integer) As String

      Dim iPos As Integer, iStart As Integer
      Dim sWord As String
      Dim sInput As String, sOutput As String = String.Empty
      Try
         If Len(sVal) = 0 Then
            Return String.Empty
         Else
            sInput = Trim$(sVal)

            Do
               iStart = iPos + 1
               iPos = InStr(iStart, sInput, " ")
               If iPos = 0 Then
                  sWord = Mid$(sInput, iStart)
               Else
                  sWord = Mid$(sInput, iStart, iPos - iStart)
               End If
               If sWord.Length <> 0 Then sOutput = InvWordX(sWord, iWinDOS) & " " & sOutput
            Loop Until iPos = 0
            Return sOutput
         End If
      Catch ex As Exception
         Return String.Empty
      End Try
 
   End Function

   Private Function InvWord(ByVal sWord As String) As String

      Dim iFirst As Integer
      Try
         iFirst = Asc(Left$(sWord, 1))
         Select Case iFirst
            Case 48 To 57, 65 To 90, 97 To 122
               Return sWord
            Case Else
               Return zzWordToDOS(sWord)
         End Select
      Catch ex As Exception
         Return String.Empty
      End Try

   End Function

   Public Function InvTlf(ByVal sWord As String) As String
      Dim iPos As Integer

      Try
         sWord = CStr(sWord)
         iPos = InStr(sWord, "-")
         If iPos > 0 Then
            Return Mid$(sWord, iPos + 1) & "-" & Left$(sWord, iPos - 1)
         Else
            Return sWord
         End If
      Catch ex As Exception
         Return String.Empty
      End Try



   End Function

   Public Function InvWordX(ByVal sWord As String, ByVal iWinDOS As Integer) As String

      Dim iSymb As Integer

      Dim bRightToLeft As Boolean
      Dim sSymb As String
      Dim iSymbType As Integer, iDirection As Integer, iIndex As Integer
      Try
         For iIndex = 1& To sWord.Length
            sSymb = Mid(sWord, iIndex, 1)
            iSymb = Asc(sSymb)
            iSymbType = zzGetSymbolType(iSymb, iWinDOS)
            iDirection = zzGetDirection(iSymbType)
            If iDirection <> iDirNeutral Then
               If iDirection = iDirLeftToRight Then
                  bRightToLeft = False
               Else
                  bRightToLeft = True
               End If
               Exit For
            End If
         Next
      Catch ex As Exception

      End Try
     
      Return zzInvSegment(sWord, bRightToLeft, iWinDOS)
   End Function

   Private Function zzGetSymbolType(ByVal iAscii As Integer, ByVal iWinDOS As Integer) As Integer
      On Error Resume Next
      Select Case iAscii
         Case 32 To 47
            Return lhbMark
         Case 48 To 57
            Return lhbDigit
         Case 58 To 64
            Return lhbMark
         Case 65 To 90
            Return lhbEnglish
         Case 91 To 96
            Return lhbMark
         Case 97 To 122
            Return lhbEnglish
         Case 123 To 127
            Return lhbMark
         Case 128 To 154
            If (iWinDOS = lDosToWindows) Then
               Return lhbHebrew
            Else                           
            End If
         Case 155 To 223
            zzGetSymbolType = lhbMark
         Case 224 To 250
            If (iWinDOS = iWindowsToDos) Then
               Return lhbHebrew
            Else
               Return lhbMark
            End If

         Case 251 To 255
            Return lhbMark
      End Select
   End Function

   Private Function zzGetDirection(ByVal iSymbType As Integer) As Integer
      Try
         Select Case iSymbType
            Case lhbEnglish, lhbDigit
               Return iDirLeftToRight
            Case lhbHebrew
               Return iDirRightToLeft
            Case Else
               Return iDirNeutral
         End Select
      Catch ex As Exception

      End Try
     
   End Function

   Private Function zzInvSegment(ByVal sInput As String, ByVal bRightToLeft As Boolean, ByVal iWinDOS As Integer) As String

      Dim sOut As String = String.Empty, sAdd As String, sSymb As String
      Dim iSymbType As Integer, iDirection As Integer
      Dim iSymb As Integer
      On Error Resume Next
      For iIndex As Integer = 1& To Len(sInput)
         sSymb = Mid$(sInput, iIndex, 1)
         iSymb = Asc(sSymb)
         iSymbType = zzGetSymbolType(iSymb, iWinDOS)
         iDirection = zzGetDirection(iSymbType)
         If (bRightToLeft And (iDirection = iDirLeftToRight)) Or (Not bRightToLeft And (iDirection = iDirRightToLeft)) Then
            sAdd = zzInvSegment(Mid(sInput, iIndex), Not bRightToLeft, iWinDOS)
            iIndex = Len(sInput)
         Else
            If (iWinDOS = iWindowsToDos) And iSymbType = lhbHebrew Then
               sAdd = Chr(iSymb - 96)
            ElseIf (iWinDOS = lDosToWindows) And iSymbType = lhbHebrew Then
               sAdd = Chr(iSymb + 96)
            Else
               sAdd = sSymb
            End If

         End If
         If bRightToLeft Then
            sOut = sAdd & sOut
         Else
            sOut &= sOut
         End If

      Next
      Return sOut
   End Function

   Private Function zzWordToDOS(ByVal sValue As String) As String

      Dim sOut As String = String.Empty
      Dim iASCCode As Integer

      If sValue.Length = 0& Then
         Return String.Empty
      Else
         For iIndex As Integer = 0 To Len(sValue) - 1
            iASCCode = Asc(sValue.Substring(iIndex, 1))

            Select Case iASCCode
               Case 224 To 250
                  sOut = Chr(iASCCode - 96) & sOut
               Case Else
                  sOut = Chr(iASCCode) & sOut
            End Select
         Next
         Return sOut
      End If
   End Function



End Module
