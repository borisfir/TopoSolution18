Imports System.Data
Public Class frmUD_SelectPlan

	Dim miProjectCode As Integer
	Dim miDetailNo As Integer
	Private moPlanTable As System.Data.DataTable ' = New System.Data.DataTable("Plan")
	Dim moPlanView As System.Data.DataView
	Private mbHasIndex As Boolean
	Private miaResIndexNums() As Integer
	Private miSelectedPlanID As Integer = 0
	Private miSelectedIndexNum As Integer = 0

	Public Sub New(ByRef oPlanTable As System.Data.DataTable)

		' This call is required by the designer.
		InitializeComponent()
		dgvMain.AutoGenerateColumns = False
		If oPlanTable Is Nothing Then
			zzInitUD_Project()
			zzOpenPlanTable()
		Else
			moPlanTable = oPlanTable
		End If




	End Sub

	Public ReadOnly Property ResIndexNums As Integer()
		Get
			Return miaResIndexNums
		End Get
	End Property
	Public ReadOnly Property SelectedPlanID As Integer
		Get
			Return miSelectedPlanID
		End Get
	End Property
	Public ReadOnly Property SelectedIndexNum As Integer
		Get
			Return miSelectedIndexNum
		End Get
	End Property

	Private Sub zzOpenPlanTable()

		Dim sComText As String = "SELECT TOP (100) PERCENT PlanID, OriginalBlockNo, OriginalBlockAddNo, ProcessNo, IndexNum From dbo.Ud_ProjectPlanData Where (ProjectCode = " & miProjectCode.ToString() & ") And (Detail = " & miDetailNo.ToString() & ")		Order By IndexNum, PlanID"

		moPlanTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sComText, CommandType.Text, "Details")
	End Sub
	Private Sub zzInitUD_Project()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TopoManager.TPlanGraph.TplnProject.ServerDataSource, frmUnidiv.ProjectDataBase, True, False)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
	End Sub

	Private Sub frmUD_SelectPlan_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		moPlanView = New DataView(moPlanTable)
		moPlanView.Sort = "IndexNum"
		Me.dgvMain.DataSource = moPlanView
	End Sub

	Private Sub cmdDown_Click(oSender As System.Object, e As EventArgs) Handles cmdDown.Click
		zzMove(1)
	End Sub
	Private Sub zzMove(dStep As Integer)
		Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oNewGridRow As DataGridViewRow

		Dim oCurrentDataRow As DataRowView
		Dim oNewDataRow As DataRowView
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		Dim iCurrentIndex As Integer = oCurrentGridRow.Index
		Dim iNewIndex As Integer = iCurrentIndex + dStep
		'	moPlanView.Sort = ""
		If iNewIndex < moPlanTable.Rows.Count AndAlso iNewIndex >= 0 Then

			oCurrentDataRow = moPlanView.Item(iCurrentIndex)
			oNewGridRow = Me.dgvMain.Rows.Item(iNewIndex)
			oNewDataRow = moPlanView.Item(iNewIndex)
			oCurrentDataRow.Item("IndexNum") = iNewIndex + 1
			oNewDataRow.Item("IndexNum") = iCurrentIndex + 1

			'	Me.dgvMain.DataSource = moPlanView
			'	Me.dgvMain.Refresh()
			Me.dgvMain.CurrentCell = oNewGridRow.Cells.Item("ctxPlan")
			'	moPlanView.Sort = "IndexNum"
			'	DMCommon.Debug.MsgBox("13_035", oCurrentDataRow.Item("IndexNum"), oNewDataRow.Item("IndexNum"))
		End If
	End Sub

	Private Sub cmdCancel_Click(oSender As System.Object, e As EventArgs) Handles cmdCancel.Click
		Me.Close()
	End Sub

	Private Sub cmdUp_Click(oSender As System.Object, e As EventArgs) Handles cmdUp.Click
		zzMove(-1)
	End Sub
	Private Sub zzSaveIndexNums()
		ReDim miaResIndexNums(moPlanTable.Rows.Count - 1)

	End Sub

	Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
		Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oValPlan As System.Object = oCurrentGridRow.Cells.Item("ctxPlan").Value
		Dim oValIndexNum As System.Object = oCurrentGridRow.Cells.Item("ctxIndexNum").Value

		Try
			miSelectedPlanID = DirectCast(oValPlan, Integer)
		Catch ex As Exception

		End Try
		Try
			miSelectedIndexNum = DirectCast(oValIndexNum, Integer)
		Catch oEx As Exception

		End Try


		Me.Close()

	End Sub


	Private Sub dgvMain_DoubleClick(oSender As System.Object, e As EventArgs) Handles dgvMain.DoubleClick
		cmdOK_Click(oSender, e)
	End Sub
End Class