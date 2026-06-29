Option Explicit On
Option Strict On
Public Class CopyOfNumerationPair
	Inherits SortedList
	Private Const msGroupDelim As String = ", "
	Private Const msGroupConjunct As String = "-"

	Public Enum enComplexType
		Entire
		[Partial]
		Undefined
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


	Public Sub New()

		MyBase.New(New ComplexComp)
		moNumerationEntire = New Numeration(enComplexType.Entire)
		moNumerationPartial = New Numeration(enComplexType.Partial)
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
	Public Function GetPresentation(ByVal iComplexType As enComplexType, ByVal bRightToLeft As Boolean) As String
		zzMakePresentation()
		If iComplexType = enComplexType.Entire Then
			If bRightToLeft Then
				Return zzInvert(msEntirePresentation)
			Else
				Return msEntirePresentation
			End If
		Else
			If bRightToLeft Then
				Return zzInvert(msPartialPresentation)
			Else
				Return msPartialPresentation
			End If

		End If
	End Function
	Public Function Input(ByVal sInputString As String, ByVal iComplexType As enComplexType) As Boolean
		Dim sNewPresentation As String
		zzMakePresentation()

		If iComplexType = enComplexType.Entire Then
			If miStatus <> enNumStatus.[New] Then
				zzClearByNumeration(moNumerationEntire)
				moNumerationEntire.Clear()

			End If

			moNumerationEntire.Input(sInputString)
			If moNumerationEntire.Exception Then
				Return False
			Else
				sNewPresentation = moNumerationEntire.GetPresentation()
				If sNewPresentation <> msEntirePresentation Then
					msEntirePresentation = sNewPresentation
					miStatus = enNumStatus.InputString
				End If
				Return True
			End If
		Else
			If miStatus <> enNumStatus.[New] Then
				zzClearByNumeration(moNumerationPartial)
				moNumerationPartial.Clear()
			End If

			moNumerationPartial.Input(sInputString)
			If moNumerationPartial.Exception Then
				Return False
			Else
				sNewPresentation = moNumerationPartial.GetPresentation()
				If sNewPresentation <> msPartialPresentation Then
					msPartialPresentation = sNewPresentation
					miStatus = enNumStatus.InputString
				End If
				Return True
			End If
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
			Catch ex As Exception
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
			Catch ex As Exception
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
	Private Class Numeration
		Inherits System.Collections.ArrayList
		'   Private moSortedArray As System.Collections.ArrayList



		Private msInputString As String
		Private moaGroups() As ComplexGroup

		Private moComparer As ComplexComp = New ComplexComp
		Private mbException As Boolean = False
		Private oCurrentGroup As ComplexGroup
		Private miComplexType As enComplexType
		Friend Event AddComplexNum(ByVal oComplexNum As ComplexNum, ByRef bResp As Boolean)
		Public Sub New(ByVal iComplexType As enComplexType)
			miComplexType = iComplexType
		End Sub
		Public ReadOnly Property ComplexType() As enComplexType
			Get
				Return miComplexType
			End Get
		End Property



		Public Sub New(ByVal sInputString As String)
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
			Dim bResp As Boolean
			Dim bException As Boolean = oNewComplexNum.Exception
			If bException Then
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
				Dim oGroupOpened As ComplexGroup = New ComplexGroup(Me.InnerItem(0))
				For iIndex As Integer = 1 To MyBase.Count - 1
					oCurrentNum = Me.InnerItem(iIndex)
					If Not oGroupOpened.Add(oCurrentNum) Then
						If sPresentation.Length <> 0 Then sPresentation &= msGroupDelim
						sPresentation &= oGroupOpened.GetPresentation()
						oGroupOpened = New ComplexGroup(oCurrentNum)
					End If

				Next
				If sPresentation.Length <> 0 Then sPresentation &= msGroupDelim
				sPresentation &= oGroupOpened.GetPresentation()
				Return sPresentation
			Else
				Return String.Empty
			End If

		End Function
		Public ReadOnly Property InnerItem(ByVal iIndex As Integer) As ComplexNum
			Get
				Return DirectCast(MyBase.Item(iIndex), ComplexNum)
			End Get

		End Property
		Private Sub zzParseString()
			Dim saGroups() As String = Split(msInputString, msGroupDelim)
			Dim oNewComplexNum As ComplexNum
			Dim bResp As Boolean
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
		Public Sub New(ByVal sStringPresentation As String)
			msStringPresentation = sStringPresentation
			zzParseString()
		End Sub
		Public Sub New(ByVal oComplexNum As ComplexNum)
			miCount = 1
			moFirstComplexNum = oComplexNum

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
		Public Function GetPresentation() As String
			Dim sPresentation As String = moFirstComplexNum.GetPresentation()
			If miCount = 2 Then
				sPresentation &= msGroupDelim & moLastComplexNum.GetPresentation()
			ElseIf miCount > 2 Then
				sPresentation &= msGroupConjunct & moLastComplexNum.GetPresentation()
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
		Private miBase As Integer
		Private msAdditition As String = String.Empty
		Private miAddititionEqv As Integer = 0
		Private miAdditionStyle As enAdditionStyle = enAdditionStyle.None
		Private msDelim As String = String.Empty
		Private mbException As Boolean = False
		Friend ComplexType As enComplexType = enComplexType.Undefined
		Public Sub New(ByVal sStringPresentation As String)
			msStringPresentation = sStringPresentation
			zzParseString()
		End Sub
		Public Sub New(ByVal iBase As Integer, Optional ByVal iAddition As Integer = 0, Optional ByVal iAdditionStyle As enAdditionStyle = enAdditionStyle.None, Optional ByVal sDelim As String = "")
			miBase = iBase
			miAddititionEqv = iAddition
			miAdditionStyle = iAdditionStyle
			msDelim = sDelim
		End Sub
		Public Function GetPresentation() As String

			Return miBase.ToString() & msDelim & zzGetAdditionPresentation()

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
				Return miBase * 1000 + miAddititionEqv
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
			msStringPresentation = Strings.Replace(msStringPresentation, ".", "/")
			msStringPresentation = Strings.Replace(msStringPresentation, "-", "/")
			msStringPresentation = msStringPresentation.Trim()
			miBase = CType(Val(msStringPresentation), Integer)
			Dim iBaseLen As Integer = miBase.ToString().Length()
			If iBaseLen <> msStringPresentation.Length() Then
				sSecondPart = msStringPresentation.Substring(miBase.ToString().Length())
				sSecondPart = sSecondPart.Trim()
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
