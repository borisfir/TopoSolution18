Option Explicit On
Option Strict On
Public Class ShopData
	Private Shared miProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderSQLServer
   '  Private Shared miProvider As TPlProvider = TPlProvider.ProviderJet
   Private msDataSource As String
   Private mbOpened As Boolean = False

   Private Shared mSqlCnn As System.Data.SqlClient.SqlConnection
   Private mOleDbSelectCommand As System.Data.SqlClient.SqlCommand
   Private mOleDbUpdateCommand As System.Data.SqlClient.SqlCommand
   Private mOleDbDataAdapter As System.Data.SqlClient.SqlDataAdapter
   Private mOleDbCommandBuilder As System.Data.SqlClient.SqlCommandBuilder
   Private Shared Function zzOpenOleDbConnection() As Integer
      Dim sConnectionString As String
      If mSqlCnn Is Nothing Then
			sConnectionString = TPlServerDB.ServerDB.GetConnectionString(miProvider)
         mSqlCnn = New System.Data.SqlClient.SqlConnection(sConnectionString)
         mSqlCnn.Open()
      End If
   End Function
   Public Shared Sub CloseOleDbConnection()

      If Not mSqlCnn Is Nothing Then
         Try
            mSqlCnn.Close()
         Catch ex As Exception

         End Try

         mSqlCnn = Nothing
      End If
   End Sub

   Public Shared Function GetDataReaderAAA(ByVal sComText As String) As System.Data.SqlClient.SqlDataReader
      Dim pSqlCommand As System.Data.SqlClient.SqlCommand
      zzOpenOleDbConnection()
      pSqlCommand = New System.Data.SqlClient.SqlCommand(sComText, mSqlCnn)
      Try
         Return pSqlCommand.ExecuteReader()
      Catch ex As Exception

         CloseOleDbConnection()
         Return Nothing
      End Try
   End Function
   Public Shared Function GetDataAdapter(ByVal sComText As String) As System.Data.SqlClient.SqlDataAdapter
      Dim pSqlCommand As System.Data.SqlClient.SqlCommand
      zzOpenOleDbConnection()
      Try
         pSqlCommand = New System.Data.SqlClient.SqlCommand(sComText, mSqlCnn)
         Dim oSqlDataAdapter As System.Data.SqlClient.SqlDataAdapter = New System.Data.SqlClient.SqlDataAdapter(pSqlCommand)
         Dim oSqlCommandBuilder As System.Data.SqlClient.SqlCommandBuilder = New System.Data.SqlClient.SqlCommandBuilder(oSqlDataAdapter)
         oSqlDataAdapter.InsertCommand = oSqlCommandBuilder.GetInsertCommand()
         oSqlDataAdapter.UpdateCommand = oSqlCommandBuilder.GetUpdateCommand()
         oSqlDataAdapter.DeleteCommand = oSqlCommandBuilder.GetDeleteCommand()
         Return oSqlDataAdapter
      Catch ex As Exception

         CloseOleDbConnection()
         Return Nothing
      End Try

   End Function
   Public Shared Function GetScalar(ByVal sComText As String) As Object
      Dim pSqlCommand As System.Data.SqlClient.SqlCommand
      zzOpenOleDbConnection()
      pSqlCommand = New System.Data.SqlClient.SqlCommand(sComText, mSqlCnn)
      Try
         Return pSqlCommand.ExecuteScalar()
      Catch ex As Exception

         CloseOleDbConnection()
         Return Nothing
      End Try
   End Function
   Public Shared Function ExecuteCommand(ByVal sComText As String) As Integer
      Dim pSqlCommand As System.Data.SqlClient.SqlCommand
      'Dim iAffectedRecords As Integer
      zzOpenOleDbConnection()
      pSqlCommand = New System.Data.SqlClient.SqlCommand(sComText, mSqlCnn)
      Try
         Return pSqlCommand.ExecuteNonQuery()
      Catch ex As Exception
         Stop
         CloseOleDbConnection()
      End Try
   End Function
End Class

