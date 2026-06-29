Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms

Public Class Form1
	Private msServerDataSource As String
	Private WithEvents moProjectPlanTable As System.Data.DataTable
	Private moProjectPlanDataAdapter As Data.Common.DbDataAdapter

	Private miProjectCode As Integer = 170578
	Private miDetailNo As Integer = 1
	'Private moDataRow As DataRow
	Public Sub New()
		'Const msServerDataBase As String = "ProjectData" ' "ProjectDataTest"
		Const msProjectDatabase As String = "UD_Projects" ' "ProjectDataTest"
		Const sBaseServerName As String = TPlServerDB.ServerDB.DBServerName
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		TPlServerDB.ServerDB.InitCurrentProject()
		msServerDataSource = sBaseServerName
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, msProjectDatabase, True, False)

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

	Private Sub Form1_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		zzLoadProjectPlanTable(False)
		Me.bnsPlans.DataSource = moProjectPlanTable
		Me.bnnPlans.BindingSource = Me.bnsPlans
		'	Me.bnsProject.DataSource = moProjectDataTable
		'	Me.mskProjectCode.DataBindings.Add(New Binding("Text", Me.bnsPlans, "ProjectCode"))
		'	Me.mskDetail.DataBindings.Add(New Binding("Text", Me.bnsPlans, "Detail"))

		'	oNewRow.Item("ProjectCode") = miProjectCode
		'	oNewRow.Item("Detail") = miDetailNo


		Me.mskPlanID.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanID"))
		Me.txtGush.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalBlockNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru
		Me.txtNormalParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru
		'	Me.txtTempParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "NewParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
	End Sub

	Private Function zzGetParameters() As System.Data.Common.DbParameter()
		Dim oaParams(1) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)

		' DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function

	Private Sub cmdSave_Click(oSender As System.Object, e As EventArgs) Handles cmdSave.Click
		Dim oDataRowView As DataRowView = DirectCast(bnsPlans.Current, DataRowView)
		oDataRowView.EndEdit()
		If False Then


			For Each oRow As DataRow In moProjectPlanTable.Rows
				'	oRow.EndEdit()
				DMCommon.Debug.MsgBox("12_190a", oRow.RowState, moProjectPlanTable.Rows.Count)
			Next
			'moProjectPlanTable.Rows.Item(0).EndEdit()
		End If

		DMCommon.Debug.MsgBox("12_190d", Me.bnsPlans.Count, Me.moProjectPlanTable.Rows.Count)

		moProjectPlanDataAdapter.Update(moProjectPlanTable)
	End Sub

	Private Sub BindingNavigatorAddNewItem_Click(oSender As System.Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click

	End Sub



	Private Sub bnsPlans_PositionChanged(oSender As System.Object, e As EventArgs) Handles bnsPlans.PositionChanged
		Dim iProjectCode As Integer
		Dim iDataRowState As DataRowState
		Dim oDataRowView As DataRowView = DirectCast(bnsPlans.Current, DataRowView)
		iDataRowState = oDataRowView.Row.RowState
		iProjectCode = DMCommon.Functions.CIntN(oDataRowView("ProjectCode"))
		If iProjectCode = 0 Then

			oDataRowView.Item("ProjectCode") = miProjectCode
			oDataRowView.Item("Detail") = miDetailNo
			oDataRowView.Item("PlanID") = zzGetMaxPlanID() + 1
			oDataRowView.EndEdit()
		Else

		End If
	End Sub
	Private Function zzGetMaxPlanID() As Integer
		Dim iPlanID As Integer
		Dim iResMaxPlanID As Integer = 0

		For Each oRow As DataRow In moProjectPlanTable.Rows
			iPlanID = DirectCast(oRow.Item("PlanID"), Integer)
			If iResMaxPlanID < iPlanID Then
				iResMaxPlanID = iPlanID
			End If
		Next
		Return iResMaxPlanID
	End Function


	Private Sub moProjectPlanTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moProjectPlanTable.TableNewRow
		'	Dim i As Integer = 0
		'	Dim o  As System.Object = e.Row
		Dim moDataRow As DataRow = e.Row
		moDataRow.Item("ProjectCode") = miProjectCode
		moDataRow.Item("Detail") = miDetailNo
		moDataRow.Item("PlanID") = zzGetMaxPlanID() + 1
		moDataRow.EndEdit()
	End Sub

	Private Sub mskProjectCode_MaskInputRejected(oSender As System.Object, e As MaskInputRejectedEventArgs) Handles mskProjectCode.MaskInputRejected

	End Sub
End Class