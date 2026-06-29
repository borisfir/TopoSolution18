Option Strict On
Option Explicit On
Imports System.Data
Imports TopoManager.TPlanGraph

Public Class frmUD_ProjectData
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private miPlanID As Integer
	Private WithEvents moProjectPlanTable As System.Data.DataTable
	Private moProjectPlanDataAdapter As Data.Common.DbDataAdapter

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()
		zzInitUD_Project()
		' Add any initialization after the InitializeComponent() call.

	End Sub

	Private Sub zzLoadProjectPlanTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetProjectPlanData"

		moProjectPlanTable = New System.Data.DataTable("ProjectPlan")
		moProjectPlanDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)

		If bSchemaOnly Then
			moProjectPlanDataAdapter.FillSchema(moProjectPlanTable, SchemaType.Source)
		Else
			moProjectPlanDataAdapter.Fill(moProjectPlanTable)
		End If



	End Sub
	Private Sub zzInitUD_Project()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()

		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, frmUnidiv.ProjectDataBase, True, False)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub

	Private Sub frmUD_ProjectData_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		zzLoadProjectPlanTable(False)
		Me.dgvPlanData.DataSource = moProjectPlanTable
	End Sub

	Private Function zzGetParameters() As System.Data.Common.DbParameter()
		Dim oaParams(2) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prPlanID", DbType.Int32, miPlanID)
		'oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, miBlockNo)

		'oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)
		'DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function
	Private Sub zzUpdateProjectPlanTable()
		Dim oRow As DataRow = Nothing
		Dim sOriginalParcelList As String = Nothing
		Dim sNewParcelList As String = Nothing

		' Dim oFragmentA As Fragment

		moProjectPlanDataAdapter.Update(moProjectPlanTable)
	End Sub
	Private Sub cmdSave_Click(oSender As System.Object, e As EventArgs) Handles cmdSave.Click
		zzUpdateProjectPlanTable()
	End Sub

	Private Sub moProjectPlanTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moProjectPlanTable.TableNewRow
		Dim oDataRow As DataRow = e.Row
		If oDataRow IsNot Nothing Then
			oDataRow.Item("ProjectCode") = miProjectCode
			oDataRow.Item("Detail") = miDetailNo
		End If



	End Sub

	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Sub dgvPlanData_CellContentClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvPlanData.CellContentClick

	End Sub
End Class