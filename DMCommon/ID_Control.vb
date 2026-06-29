Option Explicit On
Option Strict On
Public Structure ID_Control
	Private miMinID As Integer
	Private miMaxID As Integer
	Private miCurrentMaxID As Integer
	Public Sub New(iMinID As Integer, iMaxID As Integer)
		miMinID = iMinID
		miMaxID = iMaxID

		miCurrentMaxID = miMinID - 1
	End Sub
	Public ReadOnly Property MinID As Integer
		Get
			Return miMinID
		End Get
	End Property
	Public ReadOnly Property MaxID As Integer
		Get
			Return miMaxID
		End Get
	End Property
	Public Sub AddID(iValue As Integer)
		If iValue >= miMinID AndAlso iValue <= miMaxID AndAlso iValue > miCurrentMaxID Then
			miCurrentMaxID = iValue
		End If
	End Sub
	Public Sub SetID(iValue As Integer)
		If iValue >= miMinID AndAlso iValue <= miMaxID Then
			miCurrentMaxID = iValue
		End If
	End Sub

	Public Sub Reset()
		miCurrentMaxID = miMinID - 1
	End Sub
	Public Function GetNextID() As Integer
		If miCurrentMaxID < miMaxID Then
			miCurrentMaxID += 1
			Return miCurrentMaxID
		Else
			System.Windows.Forms.MessageBox.Show("CurrentMaxID=" & CStr(miCurrentMaxID) & vbCrLf & CStr(miMinID) & vbCrLf & CStr(miMaxID), "Common - ID_Control")
		End If
	End Function
	Public Function BelongRange(iValue As Integer) As Boolean
		Return (iValue >= miMinID) AndAlso (iValue <= miMaxID)
	End Function
End Structure
