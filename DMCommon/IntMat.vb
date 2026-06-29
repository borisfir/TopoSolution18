Option Explicit On
Option Strict On

Public Class IntMat
	Public Shared SimpleNumUB As Integer = 39 '499
	Public Shared IsLoaded As Boolean
	'Public Sshared AllDivisors() As Integer = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173}
	Public Shared AllDivisors() As Integer = {2L, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409}

	Private Structure Divisor
		Public Value As Long
		Public Count As Integer
		Public Sub New(iValue As Long, iCount As Integer)
			Value = iValue
			Count = iCount
		End Sub
	End Structure
	Private Class IntVal
		'Private miDividersCount As Integer
		Private mdicDivisors As Dictionary(Of Long, Divisor) = New Dictionary(Of Long, Divisor)()
		Private mhsDivisorNumbers As HashSet(Of Long) = New HashSet(Of Long)()

		Private miValue As Long
		Public Sub New(iValue As Long)
			miValue = iValue
			Dim iDivisor As Long
			Dim iCount As Integer
			Dim iQuotient As Long = iValue
			Dim iTempQuotient As Long = iValue
			Dim iRemainder As Long
			For iIndex As Integer = 0 To SimpleNumUB
				iDivisor = AllDivisors(iIndex)
				iCount = 0
				'Do While Math.DivRem(iQuotient, iDivisor, iTempQuotient) = 0
				Do
					iTempQuotient = Math.DivRem(iQuotient, iDivisor, iRemainder)
					If iRemainder = 0 Then
						iQuotient = iTempQuotient
						iCount += 1
					Else
						Exit Do
					End If

				Loop
				If iCount > 0 Then
					mdicDivisors.Add(iDivisor, New Divisor(iDivisor, iCount))
					mhsDivisorNumbers.Add(iDivisor)
				End If
				If iQuotient = 1 Then
					Exit For
				End If
				If iDivisor * iDivisor > iQuotient Then
					mdicDivisors.Add(iQuotient, New Divisor(iQuotient, 1))
					mhsDivisorNumbers.Add(iQuotient)
					Exit For
				End If
			Next
		End Sub
		Public Sub New(oIntVal1 As IntVal, oIntVal2 As IntVal)
			Dim iCount1, iCount2 As Integer
			Dim oNewDivisor As Divisor
			Dim iCountMax As Integer
			miValue = 1

			mhsDivisorNumbers = New HashSet(Of Long)(oIntVal1.DivisorNumbers)
			mhsDivisorNumbers.UnionWith(oIntVal2.DivisorNumbers)
			For Each iDivisorNumber As Integer In mhsDivisorNumbers

				iCount1 = oIntVal1.GetDivisorCount(iDivisorNumber)
				iCount2 = oIntVal2.GetDivisorCount(iDivisorNumber)
				iCountMax = Math.Max(iCount1, iCount2)
				If iCountMax > 0 Then
					oNewDivisor = New Divisor(iDivisorNumber, iCountMax)
					mdicDivisors.Add(iDivisorNumber, oNewDivisor)
					For iDivizorIndex As Integer = 1 To iCountMax
						miValue *= iDivisorNumber
					Next


				End If

			Next
		End Sub
		Public ReadOnly Property Clone As IntVal
			Get
				Return New IntVal(miValue)

			End Get
		End Property

		Public Sub Reduce(iValue As Integer)
			Dim tDivisor As Divisor
			If mdicDivisors.TryGetValue(iValue, tDivisor) Then
				mdicDivisors.Remove(iValue)
				If tDivisor.Count = 1 Then
					mhsDivisorNumbers.Remove(iValue)
				ElseIf tDivisor.Count > 1 Then
					tDivisor.Count -= 1
					mdicDivisors.Add(iValue, tDivisor)
				End If
				miValue \= iValue
			End If

		End Sub
		Public Sub Reduce(tRedDivisor As Divisor)
			Dim tDivisor As Divisor
			Dim iValue As Long = tRedDivisor.Value
			If mdicDivisors.TryGetValue(iValue, tDivisor) AndAlso tDivisor.Count >= tRedDivisor.Count Then
				mdicDivisors.Remove(iValue)
				If tDivisor.Count = tRedDivisor.Count Then
					mhsDivisorNumbers.Remove(iValue)
				Else
					tDivisor.Count -= tRedDivisor.Count
					mdicDivisors.Add(iValue, tDivisor)
				End If
				For iIndex As Integer = 1 To tRedDivisor.Count
					miValue \= iValue
				Next

			End If

		End Sub

		Public ReadOnly Property DivisorNumbers As HashSet(Of Long)
			Get
				Return mhsDivisorNumbers
			End Get
		End Property
		Public ReadOnly Property Value As Long
			Get
				Return miValue
			End Get
		End Property
		Public ReadOnly Property ValueL As Long
			Get
				Return Convert.ToInt64(miValue)
			End Get
		End Property

		Public Function GetDivisorCount(iDivisor As Integer) As Integer
			Dim tDivisor As Divisor
			If mdicDivisors.TryGetValue(iDivisor, tDivisor) Then
				Return tDivisor.Count
			Else
				Return 0
			End If
		End Function

		Public Shared Function LCM(oMultiple1 As IntVal, oMultiple2 As IntVal) As IntVal
			Return New IntVal(oMultiple1, oMultiple2)

		End Function

	End Class
	Public Class Fraction
		'Private miNumerator As Integer
		Private moNumerator As IntVal

		Private moDenominator As IntVal
		Private mbIsZero As Boolean
		Public ReadOnly Property Numerator As Long
			Get
				If mbIsZero Then
					Return 0L
				Else
					Return moNumerator.Value
				End If
			End Get
		End Property
		Public ReadOnly Property NumeratorL As Long
			Get
				If mbIsZero Then
					Return 0L
				Else
					Return moNumerator.ValueL
				End If
			End Get
		End Property

		Private ReadOnly Property Denominator As IntVal
			Get
				Return moDenominator
			End Get
		End Property
		Public ReadOnly Property IntDenominator As Long
			Get
				If mbIsZero Then
					Return 1L
				Else
					Return moDenominator.Value
				End If

			End Get
		End Property
		Public ReadOnly Property Clone As Fraction
			Get
				Return New Fraction(Me.Numerator, moDenominator.Clone)

			End Get
		End Property

		Public ReadOnly Property IsZero As Boolean
			Get
				Return mbIsZero
			End Get
		End Property

		Public Sub New(iNumerator As Long, iDenominator As Long)
			'	miNumerator = iNumerator
			If iDenominator > 0L Then

				If iNumerator = 0L Then
					mbIsZero = True
					moDenominator = New IntVal(1L)
				Else
					moNumerator = New IntVal(iNumerator)

					moDenominator = New IntVal(iDenominator)

				End If

			End If

		End Sub
		Public Overrides Function ToString() As String
			If moDenominator.Value > 0 Then
				If mbIsZero Then
					Return "0"
				Else
					If moDenominator.Value = 1 Then
						Return moNumerator.Value.ToString()
					Else
						Return moNumerator.Value.ToString() & "/" & moDenominator.Value.ToString()
					End If
				End If


			End If
			Return String.Empty
		End Function
		Public ReadOnly Property IsOne As Boolean
			Get
				'DMCommon.Debug.MsgBox("13_348k", moDenominator.Value, moNumerator.Value)
				Return (moNumerator.Value = moDenominator.Value)
			End Get
		End Property
		Public Function AdditionToOne() As Fraction
			'DMCommon.Debug.MsgBox("13_348c", moDenominator.Value, moNumerator.Value, moDenominator.Value)
			If moNumerator.Value < moDenominator.Value Then
				Return New Fraction(moDenominator.Value - moNumerator.Value, moDenominator)
			Else
				Return Nothing
			End If
		End Function
		Private Sub New(iNumerator As Long, oDenominator As IntVal)
			'	miNumerator = iNumerator
			If oDenominator.Value > 0 Then
				moNumerator = New IntVal(iNumerator)
				moDenominator = oDenominator

			End If

		End Sub
		Public Sub Reduction()
			Dim iCount1, iCount2 As Integer
			Dim oNewDivisor As Divisor
			Dim iCountMin As Integer
			Dim hsIntersectDivisorNumbers As HashSet(Of Long) = New HashSet(Of Long)(moNumerator.DivisorNumbers)
			hsIntersectDivisorNumbers.IntersectWith(moDenominator.DivisorNumbers)
			For Each iDivisorNumber As Integer In hsIntersectDivisorNumbers
				iCount1 = moNumerator.GetDivisorCount(iDivisorNumber)
				iCount2 = moDenominator.GetDivisorCount(iDivisorNumber)
				iCountMin = Math.Min(iCount1, iCount2)
				If iCountMin > 0 Then
					oNewDivisor = New Divisor(iDivisorNumber, iCountMin)
					moNumerator.Reduce(oNewDivisor)
					moDenominator.Reduce(oNewDivisor)



				End If
			Next
			'miNumerator = moNumerator.Value
		End Sub
		Public Shared Operator +(oFraction1 As Fraction, oFraction2 As Fraction) As Fraction
			Dim oLCM As IntVal = IntVal.LCM(oFraction1.Denominator, oFraction2.Denominator)
			Dim lNumerator1 As Long = oFraction1.Numerator * oLCM.Value \ oFraction1.Denominator.Value
			Dim lNumerator2 As Long = oFraction2.Numerator * oLCM.Value \ oFraction2.Denominator.Value
			Dim iSumNumerator As Long = lNumerator1 + lNumerator2
			Return New Fraction(iSumNumerator, oLCM)
		End Operator
		Public ReadOnly Property DecVaue As Double
			Get
				Return moNumerator.Value / moDenominator.Value
			End Get
		End Property
	End Class
End Class
