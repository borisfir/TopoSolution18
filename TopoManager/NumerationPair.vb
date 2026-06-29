Option Explicit On
Option Strict On
Public Class NumerationPair
   Inherits SortedList
	Public Shared msGroupDelim As String
	Private Const msGroupDelimLToR As String = ", "
   Private Const msGroupDelimRToL As String = " ,"
   Private Const msGroupDelimFW_RToL As String = ","

   Private Const msGroupConjunct As String = "-"

   Public Enum enComplexType
      Entire
      [Partial]
      Undefined
      NotExists
      MultiMerhav
   End Enum
   Public Enum enNumStatus
      [New]
      InputString
      InputList
      Updated
   End Enum

   Private WithEvents moNumerationEntire As Numeration
   Private WithEvents moNumerationPartial As Numeration
   '	Private moComparerAAA As ComplexComp = New ComplexComp
   Private miEnumEntireIndex As Integer = -1
   Private miEnumPartialIndex As Integer = -1
   Private moEnumEntireComplexNum As ComplexNum
   Private moEnumPartialComplexNum As ComplexNum
   Private miStatus As enNumStatus = enNumStatus.[New]
   Private msCurrent As String
   Private msEntirePresentation As String = String.Empty
   Private msPartialPresentation As String = String.Empty
   Private moEnumerator As System.Collections.IDictionaryEnumerator
   Private mbSorted As Boolean = True

   Private Shared miTextDirection As enTextDirection
   Public Sub New(ByVal iTextDirection As enTextDirection)

      MyBase.New(New ComplexComp)
      SetGroupDelim(iTextDirection)
    




      moNumerationEntire = New Numeration(enComplexType.Entire, iTextDirection)
      moNumerationPartial = New Numeration(enComplexType.Partial, iTextDirection)
      AddHandler moNumerationEntire.AddComplexNum, AddressOf Numeration_AddComplexNum
      AddHandler moNumerationPartial.AddComplexNum, AddressOf Numeration_AddComplexNum
   End Sub
   Public Function AddComplexNum(ByVal sValue As String, ByVal iComplexType As enComplexType) As Boolean
      Dim bResp As Boolean
      If (miStatus = enNumStatus.InputString) Or (miStatus = enNumStatus.Updated) Then
         moNumerationEntire.Clear()
         moNumerationPartial.Clear()
         MyBase.Clear()
         mbSorted = True
         miStatus = enNumStatus.[New]
      End If

      If iComplexType = enComplexType.Entire Then
         bResp = moNumerationEntire.Add(sValue)
      Else
         bResp = moNumerationPartial.Add(sValue)
      End If
      Return bResp
   End Function
   Public Shared Sub SetGroupDelim(ByVal iTextDirection As enTextDirection)
      miTextDirection = iTextDirection

      Select Case miTextDirection
         Case enTextDirection.RightToLeft
            msGroupDelim = msGroupDelimRToL
         Case enTextDirection.LeftToRight
            msGroupDelim = msGroupDelimLToR
         Case enTextDirection.FrameWorkRtoL
            msGroupDelim = msGroupDelimFW_RToL
      End Select




   End Sub
   Public Function GetPresentation(ByVal iComplexType As enComplexType, Optional ByVal iMaxLength As Integer = 0) As String
      Dim sRes As String
      zzMakePresentation()

      If iComplexType = enComplexType.Entire Then

         sRes = msEntirePresentation
      Else
         sRes = msPartialPresentation
      End If
      If iMaxLength = 0 Then
         Return sRes
      Else
         Return zzCarry(sRes, msGroupDelim, iMaxLength, miTextDirection)
      End If
   End Function

   Public Function GetItem(ByVal iIndex As Integer, ByRef sValue As String, ByRef bEntire As Boolean) As Boolean
		Dim oValue As System.Object
		Dim oComplexNum As ComplexNum
      Try
         oValue = MyBase.GetKey(iIndex)
         If (oValue Is Nothing) Then
            Return False
         Else
            oComplexNum = DirectCast(oValue, ComplexNum)
            sValue = oComplexNum.GetPresentation()
            bEntire = (oComplexNum.ComplexType = enComplexType.Entire)

            Return True
         End If

      Catch ex As Exception
         Return False
      End Try


   End Function
   Public Function ListNeedUpdate() As Boolean
      Return (miStatus = enNumStatus.InputString) OrElse (Not mbSorted)
   End Function
   Public ReadOnly Property Status() As enNumStatus
      Get
         Return miStatus
      End Get
   End Property
   Public ReadOnly Property CurrentPresentation() As String
      Get
         Try
            Return Me.Current.GetPresentation()
         Catch oEx As Exception
            Return String.Empty
         End Try
      End Get
   End Property

   Private ReadOnly Property Current() As ComplexNum
      Get
         Try
            Dim oDictionaryEntry As System.Collections.DictionaryEntry = DirectCast(moEnumerator.Current, System.Collections.DictionaryEntry)
            Dim oComplexNum As ComplexNum = DirectCast(oDictionaryEntry.Key, ComplexNum)
            Return oComplexNum
         Catch oEx As Exception
            Return Nothing
         End Try

      End Get
   End Property
   Public Function MoveNext() As Boolean
      Dim bResp As Boolean = moEnumerator.MoveNext
      If Not bResp Then
         miStatus = enNumStatus.Updated
      End If
      Return bResp
   End Function

   Public ReadOnly Property Exception() As Boolean
      Get
         Return moNumerationEntire.Exception Or moNumerationPartial.Exception
      End Get
   End Property
   Public Sub Reset()
      moEnumerator = MyBase.GetEnumerator()
      moEnumerator.Reset()
      '   moNumerationEntire()
      '    moNumerationPartial()
   End Sub
   Private Shared Function zzCarry(ByVal sValue As String, ByVal sDelim As String, ByVal iMaxLength As Integer, ByVal bRightToLeft As enTextDirection) As String
      Const sCarry As String = vbLf
      Dim saValue() As String = Strings.Split(sValue, sDelim)
      Dim sRes As String = String.Empty
      Dim sRow As String = String.Empty
      Dim iUB As Integer = saValue.GetUpperBound(0)
      Dim sPart As String


      Select Case bRightToLeft
         Case enTextDirection.RightToLeft

         Case enTextDirection.LeftToRight

      End Select



      For iIndex As Integer = 0 To iUB

         Select Case bRightToLeft
            Case enTextDirection.RightToLeft
               sPart = saValue(iUB - iIndex)
            Case enTextDirection.LeftToRight
               sPart = saValue(iIndex)
            Case Else
               sPart = String.Empty
         End Select


         If sRow.Length = 0 Then
            sRow = sPart
         Else
            If (sRow.Length + sPart.Length) > iMaxLength Then
               If sRes.Length <> 0 Then

                  Select Case bRightToLeft
                     Case enTextDirection.RightToLeft
                        sRes = sRes & sCarry
                        sRow = sDelim.Trim() & sRow
                     Case enTextDirection.LeftToRight
                        sRow &= sDelim.Trim()
                        sRes &= sCarry

                  End Select


               End If
               sRes &= sRow
               sRow = sPart
            Else

               Select Case bRightToLeft
                  Case enTextDirection.RightToLeft
                     sRow = sPart & sDelim & sRow
                  Case enTextDirection.LeftToRight
                     sRow &= sDelim & sPart
               End Select

            End If
         End If
      Next
      If sRes.Length <> 0 Then
         Select Case bRightToLeft
            Case enTextDirection.RightToLeft
               sRes = sDelim.Trim & sRes & sCarry
            Case enTextDirection.LeftToRight
               sRes &= sDelim.Trim & sCarry

         End Select


      End If
      sRes &= sRow
      Return sRes
   End Function
   Private Sub Numeration_AddComplexNum(ByVal oComplexNum As ComplexNum, ByRef bResp As Boolean)
      bResp = Not MyBase.Contains(oComplexNum)
      If bResp Then
         MyBase.Add(oComplexNum, String.Empty)
         miStatus = enNumStatus.InputList
         If mbSorted Then
            If MyBase.Count <> MyBase.IndexOfKey(oComplexNum) + 1 Then
               mbSorted = False
            End If
         End If
      End If
   End Sub
   Private Sub zzMakePresentation()
      If miStatus = enNumStatus.InputList Then
         msEntirePresentation = moNumerationEntire.GetPresentation()
         msPartialPresentation = moNumerationPartial.GetPresentation()
         miStatus = enNumStatus.Updated
      End If
   End Sub
   Private Sub zzClearByTypeOld(ByVal iComplexType As enComplexType)
      Dim oComplexNum As ComplexNum
      moEnumerator = MyBase.GetEnumerator()
      moEnumerator.Reset()

      Do While moEnumerator.MoveNext()
         oComplexNum = Me.Current
         If oComplexNum.ComplexType = iComplexType Then
            MyBase.Remove(oComplexNum)
         End If
      Loop
   End Sub
   Private Sub zzClearByNumeration(ByRef oNumeration As Numeration)
      For iIndex As Integer = 0 To oNumeration.Count - 1
         MyBase.Remove(oNumeration.InnerItem(iIndex))
      Next
   End Sub

   Private Function zzInvert(ByVal sValue As String) As String
      Dim chaValue() As Char = sValue.ToCharArray()
      Dim chV As Char
      Dim iUB As Integer = chaValue.GetUpperBound(0)
      Try
         For iIndex As Integer = 0 To (iUB - 1) \ 2
            chV = chaValue(iIndex)
            chaValue(iIndex) = chaValue(iUB - iIndex)
            chaValue(iUB - iIndex) = chV
         Next
      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "zzInvert")
      End Try

      Return New String(chaValue)
   End Function
   Public Enum enTextDirection
      LeftToRight
      RightToLeft
      FrameWorkRtoL
   End Enum

   Public Class Numeration
      Inherits System.Collections.ArrayList
      '   Private moSortedArray As System.Collections.ArrayList



      Private msInputString As String
      Private moaGroups() As ComplexGroup

		Private moComparer As ComplexComp = New ComplexComp()
		Private mbException As Boolean = False
      Private oCurrentGroup As ComplexGroup
      Private miComplexType As enComplexType
      Private miTextDirection As enTextDirection
      Private msPrefix As String
      Private msSuffix As String

      Friend Event AddComplexNum(ByVal oComplexNum As ComplexNum, ByRef bResp As Boolean)
      Public Sub New(ByVal iComplexType As enComplexType, ByVal iTextDirection As enTextDirection, Optional sPrefix As String = Nothing, Optional sSuffix As String = Nothing)
         miComplexType = iComplexType
         miTextDirection = iTextDirection
         msPrefix = sPrefix
         msSuffix = sSuffix
      End Sub
      Public ReadOnly Property ComplexType() As enComplexType
         Get
            Return miComplexType
         End Get
      End Property
      Public Sub NewAAA_AAA(ByVal sInputString As String)
         msInputString = sInputString
         zzParseString()
      End Sub
      Public Sub Input(ByVal sInputString As String)
         msInputString = sInputString
         zzParseString()
      End Sub
      Public ReadOnly Property Exception() As Boolean
         Get
            Return mbException
         End Get
      End Property
      Friend Shadows ReadOnly Property Item(ByVal iIndex As Integer) As ComplexNum
         Get

            Return Me.InnerItem(iIndex)
         End Get

      End Property
      Public Shadows ReadOnly Property ItemPresentation(ByVal iIndex As Integer) As String
         Get
            Dim oComplexNum As ComplexNum = Me.InnerItem(iIndex)
            Return oComplexNum.GetPresentation()
         End Get

      End Property
      Public Overloads Function Add(ByVal sValue As String) As Boolean
         Dim oNewComplexNum As ComplexNum = New ComplexNum(sValue)
         Dim bResp As Boolean = True
         Dim bException As Boolean = oNewComplexNum.Exception
         If bException Then
            DMAcadExt.AcadDocument.WriteMessage("@@4122 " & sValue)
            Return False
         Else
            oNewComplexNum.ComplexType = miComplexType
            RaiseEvent AddComplexNum(oNewComplexNum, bResp)
            If bResp Then MyBase.Add(oNewComplexNum)
            Return bResp
         End If

      End Function
      Public Function GetPresentation() As String
         MyBase.Sort(moComparer)

         Dim sPresentation As String = String.Empty
         Dim oCurrentNum As ComplexNum
         If Me.Count > 0 Then
            Dim oGroupOpened As ComplexGroup = New ComplexGroup(Me.InnerItem(0), msPrefix, msSuffix)
            For iIndex As Integer = 1 To MyBase.Count - 1
               oCurrentNum = Me.InnerItem(iIndex)
               If Not oGroupOpened.Add(oCurrentNum) Then
                  If sPresentation.Length <> 0 Then
                     Select Case miTextDirection
                        Case enTextDirection.RightToLeft, enTextDirection.FrameWorkRtoL
                           sPresentation = msGroupDelim & sPresentation
                        Case enTextDirection.LeftToRight
                           sPresentation &= msGroupDelim
                     End Select
                  End If
                  Select Case miTextDirection
                     Case enTextDirection.RightToLeft
                        sPresentation = oGroupOpened.GetPresentation(miTextDirection) & sPresentation
                     Case enTextDirection.LeftToRight
                        sPresentation &= oGroupOpened.GetPresentation(miTextDirection)
                     Case enTextDirection.FrameWorkRtoL
                        Dim s As String = oGroupOpened.GetPresentation(miTextDirection)
                        Dim s1 As String = sPresentation & s
                        Dim s2 As String = s & sPresentation

                        sPresentation = s2


                  End Select


                  oGroupOpened = New ComplexGroup(oCurrentNum, msPrefix, msSuffix)
               End If

            Next
            If sPresentation.Length <> 0 Then
               Select Case miTextDirection
                  Case enTextDirection.RightToLeft, enTextDirection.FrameWorkRtoL
                     sPresentation = msGroupDelim & sPresentation
                  Case enTextDirection.LeftToRight
                     sPresentation &= msGroupDelim
               End Select
            End If

            Select Case miTextDirection
               Case enTextDirection.RightToLeft, enTextDirection.FrameWorkRtoL
                  sPresentation = oGroupOpened.GetPresentation(miTextDirection) & sPresentation
               Case enTextDirection.LeftToRight
                  sPresentation &= oGroupOpened.GetPresentation(miTextDirection)
            End Select

            Return sPresentation
         Else
            Return String.Empty
         End If

      End Function
      'Select Case mbRightToLeft
      '    Case enTextDirectional.RightToLeft

      '    Case enTextDirectional.LeftToRight

      ' End Select

      Public Function GetPresentationByRows(Optional ByVal iMaxLength As Integer = 0) As String
         Dim sRes As String = GetPresentation()

         If iMaxLength = 0 Then
            Return sRes
         Else
            Return zzCarry(sRes, msGroupDelim, iMaxLength, miTextDirection)
         End If
      End Function
      Public Function GetBaseArray() As Integer()
         Dim oaRes(MyBase.Count - 1) As Integer
         For iIndex As Integer = 0 To MyBase.Count - 1
            oaRes(iIndex) = Me.InnerItem(iIndex).Base
         Next

         Return oaRes

      End Function
      Public ReadOnly Property InnerItem(ByVal iIndex As Integer) As ComplexNum
         Get
            Return DirectCast(MyBase.Item(iIndex), ComplexNum)
         End Get

      End Property

      Private Sub zzParseString()
         msGroupDelim = ","
         Dim saGroups() As String = Strings.Split(msInputString, ",")
         Dim oNewComplexNum As ComplexNum
         Dim bResp As Boolean = True
         Dim iGroupUB As Integer = saGroups.GetUpperBound(0)
         '  Dim iGroupExistUB As Integer
         ReDim moaGroups(iGroupUB)


         For iIndex As Integer = 0 To iGroupUB
            oCurrentGroup = New ComplexGroup(saGroups(iIndex))
            mbException = oCurrentGroup.Exception
            If mbException Then Return
            oCurrentGroup.Reset()
            Do While oCurrentGroup.MoveNext
               oNewComplexNum = oCurrentGroup.Current
               oNewComplexNum.ComplexType = miComplexType
               RaiseEvent AddComplexNum(oNewComplexNum, bResp)
               If bResp Then
                  MyBase.Add(oNewComplexNum)
               Else
                  mbException = True
               End If

            Loop

         Next

      End Sub
      Public Overloads Overrides Sub Sort()
         MyBase.Sort(moComparer)
      End Sub




      Public Overrides Sub Clear()
         Erase moaGroups
         mbException = False
         MyBase.Clear()

      End Sub

   End Class
   Public Class ComplexComp
      Implements IComparer
      Public Enum enOrderBy
         None
         ByBase
         ByAddition
      End Enum
		Public Function Compare(ByVal oA As System.Object, ByVal oB As System.Object) As Integer Implements System.Collections.IComparer.Compare
			Dim oComplexNumA As ComplexNum = DirectCast(oA, ComplexNum)
			Dim oComplexNumB As ComplexNum = DirectCast(oB, ComplexNum)
			If oComplexNumA.Base > oComplexNumB.Base Then
				Return 1
			ElseIf oComplexNumA.Base < oComplexNumB.Base Then
				Return -1
			ElseIf oComplexNumA.Additition > oComplexNumB.Additition Then
				Return 1
			ElseIf oComplexNumA.Additition < oComplexNumB.Additition Then
				Return -1
			Else
				Return 0
			End If

		End Function
		Public Shared Function GetCount(ByVal oComplexNumFrom As ComplexNum, ByVal oComplexNumTo As ComplexNum, ByRef iResult As Integer) As enOrderBy
         '  Subtract
         If oComplexNumTo.Base > oComplexNumFrom.Base Then
            iResult = oComplexNumTo.Base - oComplexNumFrom.Base + 1
            Return enOrderBy.ByBase
         ElseIf oComplexNumTo.Base = oComplexNumFrom.Base Then

            iResult = oComplexNumTo.Additition - oComplexNumFrom.Additition + 1
            Return enOrderBy.ByAddition
         Else
            Return enOrderBy.None
         End If
      End Function


   End Class
   Private Class ComplexGroup


      Private msStringPresentation As String
      Private moFirstComplexNum As ComplexNum
      Private moLastComplexNum As ComplexNum
      Private miCount As Integer
      Private miOrderBy As ComplexComp.enOrderBy = ComplexComp.enOrderBy.None
      Private moaItems() As ComplexNum
      Private miEnumeratorIndex As Integer = 0
      Private mbException As Boolean = False
      Private msPrefix As String
      Private msSuffix As String
      Public Sub New(ByVal sStringPresentation As String)
         msStringPresentation = sStringPresentation
         zzParseString()
      End Sub
      Public Sub New(ByVal oComplexNum As ComplexNum, Optional sPrefix As String = Nothing, Optional sSuffix As String = Nothing)
         miCount = 1
         moFirstComplexNum = oComplexNum
         msPrefix = sPrefix
         msSuffix = sSuffix
      End Sub
      Public Function Add(ByVal oComplexNum As ComplexNum) As Boolean
         If miCount = 1 Then
            miOrderBy = moFirstComplexNum.GetOrder(oComplexNum)
            If miOrderBy = ComplexComp.enOrderBy.None Then
               Return False
            Else
               miCount = 2
               moLastComplexNum = oComplexNum
               Return True
            End If
         Else
            If miOrderBy = moLastComplexNum.GetOrder(oComplexNum) Then
               miCount += 1
               moLastComplexNum = oComplexNum
               Return True
            Else
               Return False
            End If
         End If


      End Function
      Public ReadOnly Property Exception() As Boolean
         Get
            Return mbException
         End Get
      End Property
      Public Function GetPresentation(ByVal iTextDirection As enTextDirection) As String
         Dim sPresentation As String = moFirstComplexNum.GetPresentation(msPrefix, msSuffix)
         If miCount = 2 Then


            Select Case iTextDirection
               Case enTextDirection.RightToLeft
                  sPresentation = moLastComplexNum.GetPresentation(msPrefix, msSuffix) & msGroupDelim & sPresentation
               Case enTextDirection.LeftToRight
                  sPresentation &= msGroupDelim & moLastComplexNum.GetPresentation(msPrefix, msSuffix)
               Case enTextDirection.FrameWorkRtoL
                  sPresentation = moLastComplexNum.GetPresentation(msPrefix, msSuffix) & msGroupDelim & sPresentation
            End Select



         ElseIf miCount > 2 Then

            Select Case iTextDirection
               Case enTextDirection.RightToLeft, enTextDirection.FrameWorkRtoL
                  sPresentation = moLastComplexNum.GetPresentation(msPrefix, msSuffix) & msGroupConjunct & sPresentation
               Case enTextDirection.LeftToRight
                  sPresentation &= msGroupConjunct & moLastComplexNum.GetPresentation(msPrefix, msSuffix)
            End Select
         End If
         Return sPresentation
      End Function
      Private Sub zzParseString()
         Dim saComplexNum() As String = Strings.Split(msStringPresentation, msGroupConjunct)

         Dim oNextComplexNum As ComplexNum = Nothing
         moFirstComplexNum = New ComplexNum(saComplexNum(0))
         mbException = moFirstComplexNum.Exception
         If Not mbException Then
            If saComplexNum.GetUpperBound(0) > 0 Then
               moLastComplexNum = New ComplexNum(saComplexNum(1))
               mbException = moLastComplexNum.Exception
               If mbException Then Return
               miOrderBy = ComplexComp.GetCount(moFirstComplexNum, moLastComplexNum, miCount)
            Else
               miCount = 1
            End If
            ReDim moaItems(miCount - 1)
            moaItems(0) = moFirstComplexNum
            If miCount > 1 Then
               oNextComplexNum = moFirstComplexNum
            End If
            For iIndex As Integer = 1 To miCount - 1
               oNextComplexNum = oNextComplexNum.GetNext(miOrderBy)
               moaItems(iIndex) = oNextComplexNum
            Next
         End If
      End Sub

      Public ReadOnly Property Current() As ComplexNum
         Get
            If (miEnumeratorIndex <> -1) AndAlso (miEnumeratorIndex < miCount) Then
               Return moaItems(miEnumeratorIndex)
            Else
               Return Nothing
            End If

         End Get
      End Property

      Public Function MoveNext() As Boolean
         If miEnumeratorIndex < miCount Then
            miEnumeratorIndex += 1
         End If
         Return (miEnumeratorIndex < miCount)
      End Function

      Public Sub Reset()
         miEnumeratorIndex = -1
      End Sub
   End Class
   Public Class ComplexNum
      Public Enum enAdditionStyle
         None
         Num
         CapsLat
         SmallLat
         Heb

      End Enum
      Private msStringPresentation As String
      Private msSource As String
      Private miBase As Integer
      Private msAdditition As String = String.Empty
      Private miAddititionEqv As Integer = 0
      Private miAdditionStyle As enAdditionStyle = enAdditionStyle.None
      Private msDelim As String = String.Empty
      ' Private msPrefixDelim As String


      Private mbException As Boolean = False
      Friend ComplexType As enComplexType = enComplexType.Undefined
      Public Sub New(ByVal sStringPresentation As String)
         msSource = Trim(sStringPresentation)
         msStringPresentation = msSource
         zzParseString()
      End Sub
      Public Sub New(ByVal iBase As Integer, Optional ByVal iAddition As Integer = 0, Optional ByVal iAdditionStyle As enAdditionStyle = enAdditionStyle.None, Optional ByVal sDelim As String = "")
         miBase = iBase
         miAddititionEqv = iAddition
         miAdditionStyle = iAdditionStyle
         msDelim = sDelim
      End Sub
     
      Public Function GetPresentation(Optional sPrefix As String = Nothing, Optional sSuffix As String = Nothing) As String
         Dim sRes As String = miBase.ToString() & msDelim & zzGetAdditionPresentation()

         If Not String.IsNullOrEmpty(sPrefix) Then
            sRes = sPrefix & sRes
         End If
         If Not String.IsNullOrEmpty(sSuffix) Then
            sRes = sRes & sSuffix
         End If
         Return sRes
      End Function
      Public ReadOnly Property Base() As Integer
         Get
            Return miBase
         End Get
      End Property
      Public ReadOnly Property AddititionStyle() As enAdditionStyle
         Get
            Return miAdditionStyle
         End Get
      End Property
      Public ReadOnly Property Additition() As Integer
         Get
            Return miAddititionEqv
         End Get
      End Property
      Public ReadOnly Property Order() As Integer
         Get
            If miBase < (Integer.MaxValue - miAddititionEqv) * 0.001 Then
               Return miBase * 1000 + miAddititionEqv
            Else
               Return miBase
            End If

         End Get
      End Property
      Public ReadOnly Property Source() As String
         Get
            Return msSource
         End Get
      End Property
      Public Function GetNext(ByVal iOrderBy As ComplexComp.enOrderBy) As ComplexNum
         Dim oNextComplexNum As ComplexNum = Nothing
         Select Case iOrderBy
            Case ComplexComp.enOrderBy.ByBase
               oNextComplexNum = New ComplexNum(miBase + 1, miAddititionEqv, miAdditionStyle, msDelim)
            Case ComplexComp.enOrderBy.ByAddition
               oNextComplexNum = New ComplexNum(miBase, miAddititionEqv + 1, miAdditionStyle, msDelim)
         End Select
         Return oNextComplexNum
      End Function
      Public Function GetOrder(ByVal oNextComplexNum As ComplexNum) As ComplexComp.enOrderBy
         If miAdditionStyle = enAdditionStyle.None AndAlso oNextComplexNum.AddititionStyle = enAdditionStyle.None AndAlso oNextComplexNum.Base = miBase + 1 Then
            Return ComplexComp.enOrderBy.ByBase
         ElseIf miAdditionStyle = oNextComplexNum.AddititionStyle AndAlso oNextComplexNum.Base = miBase AndAlso oNextComplexNum.Additition = miAddititionEqv + 1 Then
            Return ComplexComp.enOrderBy.ByAddition
         Else
            Return ComplexComp.enOrderBy.None
         End If

      End Function
      Public ReadOnly Property Exception() As Boolean
         Get
            Return mbException
         End Get
      End Property
      Private Sub zzParseString()
         '  Dim chLetter As Char
         Dim iAsc As Integer
         Dim sSecondPart As String
         '   Dim chaAddition() As Char
         If Not String.IsNullOrEmpty(msStringPresentation) Then

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StringPres", msStringPresentation, Microsoft.VisualBasic.Conversion.Val(msStringPresentation))
				msStringPresentation = Strings.Replace(msStringPresentation, ".", "/")
            msStringPresentation = Strings.Replace(msStringPresentation, "-", "/")
            msStringPresentation = Strings.Replace(msStringPresentation, "_", "/")
            msStringPresentation = msStringPresentation.Trim()
            Dim dValue As Double = Microsoft.VisualBasic.Conversion.Val(msStringPresentation)
            miBase = Convert.ToInt32(dValue)
            If miBase <> 0 Then
               Dim iBaseLen As Integer = miBase.ToString().Length()
               If iBaseLen <> msStringPresentation.Length() Then
                  sSecondPart = msStringPresentation.Substring(miBase.ToString().Length())
                  sSecondPart = sSecondPart.Trim()
                  If sSecondPart.Length <> 0 Then


                     Dim sMaybeDelim As String = sSecondPart.Substring(0, 1)
                     Select Case sMaybeDelim
                        Case "\", "/"
                           sSecondPart = sSecondPart.Substring(1)
                           msDelim = sMaybeDelim
                     End Select
                     msAdditition = sSecondPart
                     If IsNumeric(msAdditition) Then
                        miAddititionEqv = CType(msAdditition, Integer)
                        miAdditionStyle = enAdditionStyle.Num
                     Else

                        '   chaAddition = msAdditition.ToCharArray
                        '  chLetter = chaAddition(0)
                        iAsc = Asc(msAdditition)
                        Select Case iAsc
                           Case 65 To 90
                              miAddititionEqv = iAsc - 64
                              miAdditionStyle = enAdditionStyle.CapsLat
                           Case 97 To 122
                              miAddititionEqv = iAsc - 96
                              miAdditionStyle = enAdditionStyle.SmallLat
                           Case 224 To 233
                              miAddititionEqv = iAsc - 223
                              miAdditionStyle = enAdditionStyle.Heb
                           Case 235, 236
                              miAddititionEqv = iAsc - 224
                              miAdditionStyle = enAdditionStyle.Heb
                           Case 238
                              miAddititionEqv = iAsc - 225
                              miAdditionStyle = enAdditionStyle.Heb
                           Case 240 To 242
                              miAddititionEqv = iAsc - 226
                              miAdditionStyle = enAdditionStyle.Heb
                           Case 244
                              miAddititionEqv = iAsc - 227
                              miAdditionStyle = enAdditionStyle.Heb
                           Case 246 To 250
                              miAddititionEqv = iAsc - 228
                              miAdditionStyle = enAdditionStyle.Heb
                           Case Else
                              mbException = True
                        End Select
                     End If
                  Else
                     miAdditionStyle = enAdditionStyle.None
                  End If
               End If
            End If
         End If
      End Sub

      Private Function zzGetAdditionPresentation() As String
         Dim oChar As Char
         Select Case miAdditionStyle
            Case enAdditionStyle.CapsLat
               oChar = Chr(miAddititionEqv + 64)
               Return oChar.ToString()
            Case enAdditionStyle.SmallLat
               oChar = Chr(miAddititionEqv + 96)
               Return oChar.ToString()
            Case enAdditionStyle.Heb
               Dim iDelta As Integer
               Select Case miAddititionEqv
                  Case 11, 12
                     iDelta = 1
                  Case 13
                     iDelta = 2
                  Case 14 To 16
                     iDelta = 3
                  Case 17
                     iDelta = 4
                  Case 18 To 22
                     iDelta = 5

               End Select
               oChar = Chr(miAddititionEqv + iDelta + 223)
               Return oChar.ToString()
            Case enAdditionStyle.Num
               Return miAddititionEqv.ToString
            Case Else
               Return Nothing
         End Select

      End Function
   End Class

End Class
