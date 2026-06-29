Option Explicit On
Option Strict On
Public Class BamashPolygon
	Implements System.IComparable(Of bmPropUnitKey)
	Private mtPropUnitKey As bmPropUnitKey
	Public Function CompareTo(ByVal tOtherPropUnitKey As bmPropUnitKey) As Integer Implements System.IComparable(Of bmPropUnitKey).CompareTo
		Return mtPropUnitKey.CompareTo(tOtherPropUnitKey)
	End Function
	Public ReadOnly Property PropUnitKey() As bmPropUnitKey
		Get
			Return mtPropUnitKey
		End Get
	End Property
	Public Sub New(ByVal iBldNo As Integer, ByVal iBldPart As Integer, ByVal iBldEntr As Integer, ByVal iBldFloor As Integer, ByVal iBldSubFloor As Integer)
		mtPropUnitKey.BldNo = iBldNo
		mtPropUnitKey.BldPart = iBldPart
		mtPropUnitKey.BldEntr = iBldEntr
		mtPropUnitKey.BldFloor = iBldFloor
		mtPropUnitKey.BldSubFloor = iBldSubFloor
	End Sub
	Public Function Disp() As String
		Return CStr(mtPropUnitKey.BldNo) & "," & CStr(mtPropUnitKey.BldPart) & "," & CStr(mtPropUnitKey.BldEntr) & "," & CStr(mtPropUnitKey.BldFloor) & "," & CStr(mtPropUnitKey.BldSubFloor)
	End Function
End Class
