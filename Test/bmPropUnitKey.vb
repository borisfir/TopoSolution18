Option Explicit On
Option Strict On
Public Structure bmPropUnitKey
	Implements System.Collections.Generic.IComparer(Of bmPropUnitKey)
	Implements System.IComparable(Of bmPropUnitKey)

	Public BldNo As Integer
	Public BldPart As Integer
	Public BldEntr As Integer
	Public BldFloor As Integer
	Public BldSubFloor As Integer
	Public Sub New(ByVal sBldNo As String, ByVal sBldPart As String, ByVal sBldEntr As String, ByVal sBldFloor As String)

	End Sub
	Public ReadOnly Property FloorKey() As Integer
		Get
			Return 10 * BldFloor + BldSubFloor
		End Get
	End Property
	Public Function Compare(ByVal oKeyA As bmPropUnitKey, ByVal oKeyB As bmPropUnitKey) As Integer Implements System.Collections.Generic.IComparer(Of bmPropUnitKey).Compare
		Const bLess As Integer = -1
		Const bMore As Integer = 1
		Const bEq As Integer = 0
		If oKeyA.BldNo < oKeyB.BldNo Then
			Return bLess
		ElseIf oKeyA.BldNo > oKeyB.BldNo Then
			Return bMore
		ElseIf oKeyA.BldPart < oKeyB.BldPart Then
			Return bLess
		ElseIf oKeyA.BldPart > oKeyB.BldPart Then
			Return bMore
		ElseIf oKeyA.BldEntr < oKeyB.BldEntr Then
			Return bLess
		ElseIf oKeyA.BldEntr > oKeyB.BldEntr Then
			Return bMore
		ElseIf oKeyA.FloorKey < oKeyB.FloorKey Then
			Return bLess
		ElseIf oKeyA.FloorKey > oKeyB.FloorKey Then
			Return bMore
		Else
			Return bEq
		End If
	End Function
	Public Overrides Function ToString() As String
		Const sDelim As String = ","
		Return BldNo.ToString() & sDelim & BldPart.ToString() & sDelim & BldEntr.ToString() & sDelim & BldFloor.ToString() & sDelim & BldSubFloor.ToString()
	End Function
	Public Function CompareTo(ByVal oPropUnitKey As bmPropUnitKey) As Integer Implements System.IComparable(Of bmPropUnitKey).CompareTo
		Return Compare(Me, oPropUnitKey)
	End Function
End Structure

