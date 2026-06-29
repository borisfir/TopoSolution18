Option Explicit On
Option Strict On
Public Class SubNumeration
	Private Shared mlstEmptyNum As Generic.List(Of Integer)
	Private Shared mdicSubPgons As Generic.Dictionary(Of Integer, Integer)
	Private Shared mdicNums As Generic.Dictionary(Of Integer, Integer)
	Public Shared Sub AddPgon(ByVal iTopoID As Integer, ByVal iNo As Integer)
		If iNo = 0 Then
			mlstEmptyNum.Add(iTopoID)
		Else
			mdicSubPgons.Add(iTopoID, iNo)
			mdicNums.Add(iNo, iTopoID)
		End If
	End Sub
	Public Shared Sub Numerate()
		Dim iNum As Integer = 1
		Dim iListIndex As Integer = 0
		Do
			If Not mdicNums.ContainsKey(iNum) Then
				mdicSubPgons.Add(mlstEmptyNum.Item(iListIndex), iNum)
				iListIndex += 1
				If iListIndex = mlstEmptyNum.Count Then
					Exit Do
				End If
			End If
			iNum += 1
		Loop
	End Sub
	Public Shared Function GetNum(ByVal iTopoID As Integer) As Integer
		If mdicNums.ContainsKey(iTopoID) Then
			Return mdicNums.Item(iTopoID)
		Else
			Return 0
		End If
	End Function
End Class
