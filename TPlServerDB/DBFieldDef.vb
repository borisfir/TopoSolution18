Option Explicit On 
Option Strict On
Public Class DBFieldDef
	Private msName As String
	Private miDataType As Integer
	Private miSize As Integer
	Public Property Name() As String
		Get
			Name = msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
		End Set
	End Property
	Public Property DataType() As Integer
		Get
			DataType = miDataType
		End Get
		Set(ByVal iValue As Integer)
			miDataType = iValue
		End Set
	End Property
	Public Property Size() As Integer
		Get
			Size = miSize
		End Get
		Set(ByVal iValue As Integer)
			miSize = iValue
		End Set
	End Property
	Public Function GetCreateColumnCmdText(ByVal iProvider As TPlProvider) As String
      Dim sOut As String = String.Empty
		If miDataType <> 0 Then
			sOut = msName & Strings.Space(1) & zzGetDataTypeStr(iProvider)
			If miSize <> 0 Then
				sOut = sOut & "(" & miSize.ToString & ")"
			End If
		End If
		Return sOut
	End Function
	Private Function zzGetDataTypeStr(ByVal iProvider As TPlProvider) As String
		'	Dim iDataType As System.Data.DbType
		'	Dim iOleDbdataType As System.Data.SqlClient.OleDbType
		Dim iOdbcDataType As System.Data.Odbc.OdbcType

		Select Case iProvider
			Case TPlProvider.ProviderJet
				iOdbcDataType = CType(miDataType, System.Data.Odbc.OdbcType)
			Case TPlProvider.ProviderSQLServer
				'	iOleDbDataType = CType(miDataType, System.Data.DbType)
		End Select
		zzGetDataTypeStr = iOdbcDataType.ToString()
	End Function
End Class
