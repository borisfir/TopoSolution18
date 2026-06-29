Option Explicit On
Option Strict On
Public Structure ItemSetting
	Private miItemNo As Integer
	Private msItemName As String

	Private msText As String
	Private msTextA As String
	Private miSize As Integer
	Private mbIsNotEmpty As Boolean
	Public Sub New(ByRef oDataReader As System.Data.Common.DbDataReader)
		miItemNo = oDataReader.GetInt32(0)
		If Not oDataReader.IsDBNull(1) Then
			msItemName = oDataReader.GetString(1)
		End If
		If Not oDataReader.IsDBNull(2) Then
			msText = oDataReader.GetString(2)
		End If

		If Not oDataReader.IsDBNull(3) Then
			msTextA = oDataReader.GetString(3)
		End If

		If Not oDataReader.IsDBNull(4) Then
			miSize = DMCommon.Functions.CIntN(oDataReader.GetInt32(4))
		End If

		mbIsNotEmpty = True
	End Sub
	Public ReadOnly Property ItemNo As Integer
		Get
			Return miItemNo
		End Get
	End Property
	Public ReadOnly Property ItemName As String
		Get
			Return msItemName
		End Get
	End Property
	Public ReadOnly Property Text As String
		Get
			Return msText
		End Get
	End Property
	Public ReadOnly Property TextA As String
		Get
			Return msTextA
		End Get
	End Property
	Public ReadOnly Property Size As Integer
		Get
			Return miSize
		End Get
	End Property
	Public ReadOnly Property IsNotEmpty As Boolean
		Get
			Return mbIsNotEmpty
		End Get
	End Property
End Structure
