Option Explicit On
Option Strict On
Imports System.Data
Public Class dmDBManager_Topo
	Inherits System.Data.Common.DbConnection

	Private miProvider As TPlProvider
	Protected moDbConnection As System.Data.Common.DbConnection
	Private moDbProviderFactory As Common.DbProviderFactory
	Public Shared Sub TT()

		Dim dbProviderFactory As Common.DbProviderFactory
		Try
			dbProviderFactory = Common.DbProviderFactories.GetFactory("OleDb Data Provider")
		Catch ex As Exception
			Stop
		End Try
		Try
			dbProviderFactory = Common.DbProviderFactories.GetFactory(".Net Framework Data Provider for OleDb")
		Catch ex As Exception
			Stop
		End Try
		Try
			dbProviderFactory = Common.DbProviderFactories.GetFactory("System.Data.OleDb")
		Catch ex As Exception
			Stop
		End Try
		Try
			dbProviderFactory = Common.DbProviderFactories.GetFactory("System.Data.OleDb.OleDbFactory")
		Catch ex As Exception
		End Try

		'DbConnection dbConnection = dbProviderFactory.CreateConnection(); 
		'dbConnection.ConnectionString = myConnectionString; 
		Stop
	End Sub
	Public Sub New()
		MyBase.New()
	End Sub
	Public Sub New(ByVal iProvider As TPlProvider)
		miProvider = iProvider
	End Sub
	Public Sub SetSQL(ByVal sServerName As String, ByVal sDatabaseName As String)
		miProvider = TPlProvider.ProviderSQLServer
		Dim oList As DataTable = Common.DbProviderFactories.GetFactoryClasses()
		Try
			moDbProviderFactory = Common.DbProviderFactories.GetFactory("System.Data.SqlClient")

		Catch oEx As Exception

		End Try

		zzOpenConnection(zzGetSQLConnectionString(sServerName, sDatabaseName))
	End Sub
	Public Sub SetOleDB(ByVal sDatabaseFileName As String, ByVal sSystemDB As String)
		miProvider = TPlProvider.ProviderJet
		moDbProviderFactory = Common.DbProviderFactories.GetFactory("System.Data.OleDb")
		zzOpenConnection(zzGetOleDbConnectionString(sDatabaseFileName, sSystemDB))
	End Sub
	Public ReadOnly Property DBConnectionState() As ConnectionState
		Get
			If moDbConnection Is Nothing Then
				Return ConnectionState.Closed
			Else
				Return moDbConnection.State
			End If
		End Get

	End Property
	Public Function GetDataReader(ByVal sComText As String) As Common.DbDataReader
		Dim oDbCommand As Common.DbCommand = moDbProviderFactory.CreateCommand() ' zzGetNewCommand(sComText)
		oDbCommand.Connection = moDbConnection
		oDbCommand.CommandText = sComText
		Try
			Return oDbCommand.ExecuteReader
		Catch oEx As Exception
			Return Nothing
		End Try
	End Function

	Public Function GetDataAdapter(ByVal sSelectComText As String, Optional ByVal bUseBuilder As Boolean = True, Optional ByVal sUpdateComText As String = "") As Common.DbDataAdapter
		Dim oDbCommand As System.Data.Common.DbCommand
		Dim oDbDataAdapter As System.Data.Common.DbDataAdapter
		Dim oDbCommandBuilder As System.Data.Common.DbCommandBuilder

		Try
			oDbDataAdapter = zzGetNewAdapter()
			oDbCommand = zzGetNewCommand(sSelectComText)
			oDbDataAdapter.SelectCommand() = oDbCommand
			If sUpdateComText.Length <> 0 Then
				oDbCommand = zzGetNewCommand(sUpdateComText)
				oDbDataAdapter.UpdateCommand = oDbCommand
			End If
			If bUseBuilder Then
				oDbCommandBuilder = zzGetNewCommandBuilder()
				oDbCommandBuilder.DataAdapter = oDbDataAdapter
				If sUpdateComText.Length = 0 Then
					oDbDataAdapter.InsertCommand = oDbCommandBuilder.GetInsertCommand()
				End If
				oDbDataAdapter.DeleteCommand = oDbCommandBuilder.GetDeleteCommand()
				oDbDataAdapter.UpdateCommand = oDbCommandBuilder.GetUpdateCommand()
			End If
			Return oDbDataAdapter
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSelectComText, "dmDBManager - GetDataAdapter")
			Return Nothing
		End Try

	End Function
	Public Function RunCommand(ByVal sComText As String, Optional ByRef oErrOut As System.Data.Common.dbException = Nothing) As Integer
		Dim oDbCommand As System.Data.Common.DbCommand = zzGetNewCommand(sComText)
		Try
			Return oDbCommand.ExecuteNonQuery()
		Catch oEx As System.Data.Common.DbException
			oErrOut = oEx
			'	System.Windows.Forms.MessageBox.Show(oEx.Message, "dmDBManager - RunCommand")
			'	System.Windows.Forms.MessageBox.Show(sComText, "18_873a")
			Return -1
		End Try
	End Function
	Public Function GetDataTable(ByVal sComText As String, ByVal sTableName As String) As System.Data.DataTable
		Dim oDbSelectCommand As System.Data.Common.DbCommand
		Dim pDbDataAdapter As System.Data.Common.DbDataAdapter
		Dim oDataTable As DataTable
		oDbSelectCommand = zzGetNewCommand(sComText)
		pDbDataAdapter = zzGetNewAdapter()
		pDbDataAdapter.SelectCommand() = oDbSelectCommand
		oDataTable = New DataTable(sTableName)
		pDbDataAdapter.Fill(oDataTable)
		Return oDataTable
	End Function
	Public Function GetDataScalar(ByVal sComText As String) As System.Object
		Dim oDbCommand As System.Data.Common.DbCommand
		oDbCommand = zzGetNewCommand(sComText)
		Return oDbCommand.ExecuteScalar()
	End Function
	Public Overrides Sub Close()
		Try
			If moDbConnection IsNot Nothing Then
				moDbConnection.Close()
				moDbConnection.Dispose()
				moDbConnection = Nothing
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "dmDBManager - Close_1")
		End Try
		moDbConnection = Nothing
		Try
			TextResource.Close()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "dmDBManager - Close_2")
		End Try

	End Sub
	Private Function zzGetSQLConnectionString(ByVal sServerName As String, ByVal sDatabaseName As String) As String
		'Return "Data Source=" & sServerName & ";Initial Catalog=" & sDatabaseName & ";User ID=sa;Password="
		Return "Data Source=" & sServerName & ";Initial Catalog=" & sDatabaseName & ";Integrated Security=True"
	End Function
	Private Function zzGetOleDbConnectionString(ByVal sDatabaseFileName As String, ByVal sSystemDBName As String) As String
		Dim sProviderName As String
		sProviderName = [Global].GetProviderName(miProvider)

		Return "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=0;Data Source=" & sDatabaseFileName & ";Jet OLEDB:Engine " & _
		"Type=5;Provider=""" & sProviderName & """;Jet OLEDB:System database=" & sSystemDBName & ";Jet OLEDB:SFP=False;persist security info=False;Extended Properties=;Mode=Share Deny None;" & _
		"Jet OLEDB:Encrypt Database=False;Jet OLEDB:Create System Database=False;Jet OLEDB:" & _
		"Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;" & _
		"User ID=Admin;Jet OLEDB:Global Bulk Transactions=1"
	End Function

	Private Function zzGetNewConnection() As Common.DbConnection
		If miProvider = TPlProvider.ProviderJet Then
			Return New System.Data.OleDb.OleDbConnection()
		ElseIf miProvider = TPlProvider.ProviderSQLServer Then
			Return New System.Data.SqlClient.SqlConnection()
		Else
			Return Nothing
		End If
	End Function
	Private Sub zzOpenConnection(ByVal sConnectionString As String)
		moDbConnection = moDbProviderFactory.CreateConnection()	'zzGetNewConnection()
		moDbConnection.ConnectionString = sConnectionString

		Try
			moDbConnection.Open()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "dmDBManager - zzOpenConnection")
		End Try
	End Sub
	Private Function zzGetNewCommandA(ByVal sComText As String) As Common.DbCommand
		Dim oDbCommand As Common.DbCommand = Nothing
		If miProvider = TPlProvider.ProviderJet Then
			Dim oOleDbCnn As System.Data.OleDb.OleDbConnection = DirectCast(moDbConnection, OleDb.OleDbConnection)
			oDbCommand = New System.Data.OleDb.OleDbCommand()
		ElseIf miProvider = TPlProvider.ProviderSQLServer Then
			Dim oSQLCnn As System.Data.SqlClient.SqlConnection = DirectCast(moDbConnection, SqlClient.SqlConnection)
			oDbCommand = New System.Data.SqlClient.SqlCommand()
		End If
		If oDbCommand IsNot Nothing Then
			oDbCommand.CommandText = sComText
			oDbCommand.Connection = moDbConnection
			Return oDbCommand
		Else
			Return Nothing
		End If
	End Function
	Protected Function zzGetNewCommand(ByVal sComText As String) As Common.DbCommand
		Dim oDbCommand As Common.DbCommand = moDbConnection.CreateCommand()
		If oDbCommand IsNot Nothing Then
			oDbCommand.CommandText = sComText

			Return oDbCommand
		Else
			Return Nothing
		End If
	End Function


	Private Function zzGetNewCommandBuilder() As Common.DbCommandBuilder
		If miProvider = TPlProvider.ProviderJet Then
			Dim oOleDbCnn As System.Data.OleDb.OleDbConnection = DirectCast(moDbConnection, OleDb.OleDbConnection)
			Return New System.Data.OleDb.OleDbCommandBuilder()
		ElseIf miProvider = TPlProvider.ProviderSQLServer Then
			Dim oSQLCnn As System.Data.SqlClient.SqlConnection = DirectCast(moDbConnection, SqlClient.SqlConnection)
			Return New System.Data.SqlClient.SqlCommandBuilder()
		Else
			Return Nothing
		End If
	End Function
	Private Function zzGetNewAdapter() As Common.DbDataAdapter
		If miProvider = TPlProvider.ProviderJet Then
			Return New System.Data.OleDb.OleDbDataAdapter()
		ElseIf miProvider = TPlProvider.ProviderSQLServer Then
			Return New System.Data.SqlClient.SqlDataAdapter()
		Else
			Return Nothing
		End If
	End Function

	Protected Overrides Function BeginDbTransaction(ByVal oIsolationLevel As System.Data.IsolationLevel) As System.Data.Common.DbTransaction
		Return moDbConnection.BeginTransaction(oIsolationLevel)
	End Function

	Public Overrides Sub ChangeDatabase(ByVal sDatabaseName As String)
		moDbConnection.ChangeDatabase(sDatabaseName)
	End Sub



	Public Overrides Property ConnectionString() As String
		Get
			Return moDbConnection.ConnectionString
		End Get
		Set(ByVal sValue As String)
			moDbConnection.ConnectionString = sValue
		End Set
	End Property

	Protected Overrides Function CreateDbCommand() As System.Data.Common.DbCommand
		Return moDbConnection.CreateCommand()
	End Function

	Public Overrides ReadOnly Property Database() As String
		Get
			Return moDbConnection.Database
		End Get
	End Property

	Public Overrides ReadOnly Property DataSource() As String
		Get
			Return moDbConnection.DataSource
		End Get
	End Property

	Public Overrides Sub Open()
		moDbConnection.Open()
	End Sub

	Public Overrides ReadOnly Property ServerVersion() As String
		Get
			Return moDbConnection.ServerVersion
		End Get
	End Property

	Public Overrides ReadOnly Property State() As System.Data.ConnectionState
		Get
			Return moDbConnection.State
		End Get
	End Property
End Class
