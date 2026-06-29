Option Explicit On
Option Strict On
Imports System.Data
Public Class UD_ExpImpDataAAA
	Const msJournalSPName As String = "GetJournalData"
	Const msFragmentsSPName As String = "GetFragments"
	Const msParcelFragmentsSPName As String = "GetParcelFragments"


	Private Shared miProjectCode As Integer
	Private Shared miDetailNo As Integer
	Private Shared moJournalTable As UD_Table = New UD_Table("Journal", msJournalSPName)
	Private Shared moFragmentsTable As UD_Table = New UD_Table("Fragments", msFragmentsSPName)
	Private Shared moParcelFragmentsTable As UD_Table = New UD_Table("ParcelFragments", msParcelFragmentsSPName)


	Public Shared Sub ExportData()
		zzInitUD_Project()
		zzLoadTables()
	End Sub
	Public Shared Function ClearData() As Boolean
		Const sMsg0 As String = " אתה עומד למחוק אותם"
		Const sMsg1 As String = "? האם הינך בטוח שברצונך להמשיך"
		Dim sBlockList As String = zzGetBlockList()
		Dim sMsgText As String
		Dim bRes As Boolean
		zzInitUD_Project()
		Dim sProjectLabel As String = TopoManager.TPlanGraph.TplnBlock.GetBlockName(miProjectCode, miDetailNo)
		'  DMCommon.Debug.MsgBox("12_890a", miProjectCode, miDetailNo, sBlockList)
		If sBlockList IsNot Nothing Then
			sMsgText = "הגושים הקיימים בפרויקט " & sProjectLabel & vbCrLf
			sMsgText &= sBlockList & vbCrLf
			sMsgText &= sMsg0 & vbCrLf
			sMsgText &= sMsg1
			If System.Windows.Forms.MessageBox.Show(sMsgText, "Datamap", Windows.Forms.MessageBoxButtons.OKCancel, Windows.Forms.MessageBoxIcon.Warning, Windows.Forms.MessageBoxDefaultButton.Button2, Windows.Forms.MessageBoxOptions.RightAlign) = Windows.Forms.DialogResult.OK Then  'And Windows.Forms.MessageBoxOptions.RtlReading
				bRes = True
				zzClearTables()
			End If
		Else
			bRes = True
		End If
		Return bRes


	End Function
	Public Shared Sub ImportData()
		zzInitUD_Project()
		If ClearData() Then
			zzUpdateTables()
		End If

	End Sub
	Private Shared Sub zzInitUD_Project()
		'    TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		'   TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub


	Private Shared Sub zzLoadTables()
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		Const sJournalSPName As String = "GetJournalData"
		Const sFragmentsSPName As String = "GetFragments"
		Dim sFileName As String = "D:\Test.XML"
		'  Dim sSchemaFileName As String = "D:\TestSchema.XML"

		'    Dim oJournalTable As UD_Table = New UD_Table("Journal", sJournalSPName)
		'   Dim oFragmentsTable As UD_Table = New UD_Table("Fragments", sFragmentsSPName)


		moJournalTable.Load()
		moFragmentsTable.Load()
		moParcelFragmentsTable.Load()




		Dim oDataSet As DataSet = New DataSet("Export")
		'  oJournalDataAdapter.Fill(oDataSet)
		oDataSet.Tables.Add(moJournalTable.Table)
		oDataSet.Tables.Add(moFragmentsTable.Table)
		oDataSet.Tables.Add(moParcelFragmentsTable.Table)


		DMCommon.Debug.MsgBox("11_577M", oDataSet.SchemaSerializationMode, oDataSet.Tables, DMCommon.Debug.ColCount(oDataSet.Tables))
		'    oDataSet.WriteXml(sFileName, XmlWriteMode.WriteSchema)
		' oDataSet.WriteXmlSchema(sSchemaFileName)


	End Sub
	Private Shared Sub zzUpdateTables()
		Dim sFileName As String = "D:\Test.XML"
		Dim oDataSet As DataSet = New DataSet("Import")
		oDataSet.ReadXml(sFileName)
		DMCommon.Debug.MsgBox("11_577P", oDataSet.Tables, DMCommon.Debug.ColCount(oDataSet.Tables))
		moJournalTable.LoadSchema()
		moFragmentsTable.LoadSchema()
		moParcelFragmentsTable.LoadSchema()

		moJournalTable.AddData(oDataSet.Tables(0))
		moFragmentsTable.AddData(oDataSet.Tables(1))
		moParcelFragmentsTable.AddData(oDataSet.Tables(2))

		moJournalTable.UpdateTable()
		moFragmentsTable.UpdateTable()
		moParcelFragmentsTable.UpdateTable()

	End Sub
	Private Shared Function zzGetBlockList() As String
		Const sBlockListSPName As String = "GetBlockList"
		Dim sBlockName As String
		Dim iBlockNo, iBlockAdd As Integer
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sBlockListSPName, CommandType.StoredProcedure, zzGetParameters(False))
		Dim sBlockList As String = Nothing


		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				'  DMCommon.Debug.MsgBox("12_890d")
				While oDataReader.Read
					iBlockNo = oDataReader.GetInt32(0)
					iBlockAdd = oDataReader.GetInt32(1)
					sBlockName = TopoManager.TPlanGraph.TplnBlock.GetBlockName(iBlockNo, iBlockAdd)
					If sBlockList Is Nothing Then
						sBlockList = sBlockName
					Else
						sBlockList &= sBlockName & ", "
					End If

				End While
			End If
			oDataReader.Close()
		End If

		Return sBlockList
	End Function
	Private Shared Sub zzClearTables()
		Const sSPName As String = "ClearBlockData"
		Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())
	End Sub
	Private Shared Function zzGetParameters(Optional bBlockParameter As Boolean = True) As System.Data.Common.DbParameter()
		Dim iParamUB As Integer
		If bBlockParameter Then
			iParamUB = 3
		Else
			iParamUB = 1
		End If
		Dim oaParams(iParamUB) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		If bBlockParameter Then
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, 0)
			oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, 0)
		End If

		'DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function
	Private Class UD_Table
		Private msTableName As String
		Private msLoadSPName As String
		Private moDataTable As System.Data.DataTable
		Dim moDataAdapter As Data.Common.DbDataAdapter
		Public Sub New(sTableName As String, sLoadSPName As String)
			msTableName = sTableName
			msLoadSPName = sLoadSPName
		End Sub
		Public Sub Load()
			moDataTable = New System.Data.DataTable(msTableName)
			moDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(msLoadSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)
			moDataAdapter.Fill(moDataTable)
		End Sub
		Public Sub LoadSchema()
			moDataTable = New System.Data.DataTable(msTableName)
			moDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(msLoadSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)
			moDataAdapter.FillSchema(moDataTable, SchemaType.Source)
		End Sub
		Public ReadOnly Property Table As DataTable
			Get
				Return moDataTable
			End Get
		End Property
		Public Sub AddData(oAdditionalTable As DataTable)
			moDataTable.Merge(oAdditionalTable)
		End Sub
		Public Sub UpdateTable()
			moDataAdapter.Update(moDataTable)
		End Sub
	End Class

	Public Sub New()

	End Sub
End Class
