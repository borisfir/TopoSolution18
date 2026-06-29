Option Explicit On
Option Strict On
Imports System.Data
Public MustInherit Class ExpImpData
   ' Const msJournalSPName As String = "GetJournalData"
   ' Const msFragmentsSPName As String = "GetFragments"
   ' Const msParcelFragmentsSPName As String = "GetParcelFragments"

   Protected doConnection As dmDBManager
   Private Shared miProjectCode As Integer
   Private Shared miDetailNo As Integer

   Private moTables() As ExpImpTable
   Private miTablesUB As Integer
   Protected doDataSet As DataSet
   Public Sub New(oConnection As dmDBManager, iProjectCode As Integer, iDetailNo As Integer, saTableName() As String, saSPName() As String)

      miProjectCode = iProjectCode
      miDetailNo = iDetailNo

      zzNew(oConnection, saTableName, saSPName)
   End Sub
   Public Sub New(oConnection As dmDBManager, saTableName() As String, saSPName() As String)
      zzNew(oConnection, saTableName, saSPName)
   End Sub
   Public Property ProjectCode As Integer
      Get
         Return miProjectCode
      End Get
      Set(iValue As Integer)
         miProjectCode = iValue
      End Set
   End Property
   Public Property DetailNo As Integer
      Get
         Return miDetailNo
      End Get
      Set(iValue As Integer)
         miDetailNo = iValue
      End Set
   End Property

   Public Sub ExportData(sFileName As String)
      zzLoadTables()
      doDataSet.WriteXml(sFileName, XmlWriteMode.WriteSchema)
   End Sub

   Public Function ImportData(sFileName As String) As Boolean
      Dim sList As String = Nothing
      doDataSet = New DataSet("Import")
      doDataSet.ReadXml(sFileName)
      UpdateProjectDetail()
      If DataExists(sList) Then
         If ClearWarning(sList) Then
            ClearTables()
            zzUpdateTables()
         Else
            Return False
         End If
      Else
         zzUpdateTables()
      End If
      Return True



   End Function

   Private Sub zzNew(oConnection As dmDBManager, saTableName() As String, saSPName() As String)
      doConnection = oConnection

      miTablesUB = saTableName.GetUpperBound(0)

      If miTablesUB = saSPName.GetUpperBound(0) Then
         ReDim moTables(miTablesUB)
         For iIndex As Integer = 0 To miTablesUB
            moTables(iIndex) = New ExpImpTable(doConnection, saTableName(iIndex), saSPName(iIndex), GetLoadParameters(saTableName(iIndex)))

         Next
      End If

   End Sub


   Private Sub zzLoadTables()
      doDataSet = New DataSet("Export")
      For iIndex As Integer = 0 To miTablesUB
         moTables(iIndex).Load()
         doDataSet.Tables.Add(moTables(iIndex).Table)
      Next
   End Sub
   Private Sub zzUpdateTables()

      For iIndex As Integer = 0 To miTablesUB
         moTables(iIndex).LoadSchema()
        

         '  DMCommon.Debug.MsgBox("12_901B", iIndex, moTables(iIndex).Table.TableName)
         moTables(iIndex).AddData(doDataSet.Tables(iIndex))
         moTables(iIndex).UpdateTable()
      Next



   End Sub

  
   Protected Function GetParameters(Optional iParametersUB As Integer = 1) As System.Data.Common.DbParameter()

      Dim oaParams(iParametersUB) As System.Data.Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing

      oaParams(0) = doConnection.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = doConnection.GetParameter("@prDetail", DbType.Int32, miDetailNo)

      '   DMCommon.Debug.MsgBox("11_572", oaParams.GetUpperBound(0), oaParams(0).Value, oaParams(1).Value)

      Return oaParams
   End Function
   Protected MustOverride Function GetLoadParameters(sTableName As String) As System.Data.Common.DbParameter()

   Protected MustOverride Function ClearWarning(sList As String) As Boolean
   Protected MustOverride Function DataExists(ByRef sList As String) As Boolean

   Protected MustOverride Sub ClearTables()
   Protected MustOverride Sub UpdateProjectDetail()





   Private Class ExpImpTable
      Private msTableName As String
      Private msLoadSPName As String
      Private moConnection As dmDBManager
      Private moDataTable As System.Data.DataTable
      Private moaLoadParameters() As System.Data.Common.DbParameter
      Dim moDataAdapter As Data.Common.DbDataAdapter
      Public Sub New(oConnection As dmDBManager, sTableName As String, sLoadSPName As String, oaLoadParameters() As System.Data.Common.DbParameter)
         moConnection = oConnection
         msTableName = sTableName
         msLoadSPName = sLoadSPName
         moaLoadParameters = oaLoadParameters
         '    DMCommon.Debug.MsgBox("11_572RR", moConnection.Database, sTableName, moaLoadParameters.GetUpperBound(0), moaLoadParameters(0).Value, moaLoadParameters(1).Value)
      End Sub
      Public Sub Load()
         moDataTable = New System.Data.DataTable(msTableName)
         moDataAdapter = moConnection.GetDataAdapter(msLoadSPName, CommandType.StoredProcedure, moaLoadParameters, True, "", True, True)
         moDataAdapter.Fill(moDataTable)
      End Sub
      Public Sub LoadSchema()
         moDataTable = New System.Data.DataTable(msTableName)
         moDataAdapter = moConnection.GetDataAdapter(msLoadSPName, CommandType.StoredProcedure, moaLoadParameters, True, "", True, True)
         moDataAdapter.FillSchema(moDataTable, SchemaType.Source)
      End Sub
  
      Public ReadOnly Property Table As DataTable
         Get
            Return moDataTable
         End Get
      End Property
      Public Sub AddData(oAdditionalTable As DataTable)
         '       DMCommon.Debug.MsgBox("12_906", moConnection.Database, moDataTable.TableName, moDataTable.Rows.Count, oAdditionalTable.TableName, oAdditionalTable.Rows.Count)
         Try
            moDataTable.Merge(oAdditionalTable)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "ExImpData - AddData")
         End Try

      End Sub
      Public Sub UpdateTable()
         moDataAdapter.Update(moDataTable)
      End Sub
   End Class



End Class
