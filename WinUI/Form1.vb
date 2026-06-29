Option Explicit On
Option Strict On
Imports System.ComponentModel
Imports DMCommon
Public Class Form1
	Private mbInitializedDB As Boolean
	Private myBindingManagerBase As BindingManagerBase

	Private myCurrencyManager As CurrencyManager
	Private moPlanMainDataAdapter As Common.DbDataAdapter
	Private moPlanMainTable As DataTable
	Private moPlanTypeDataAdapter As Common.DbDataAdapter
	Private moPlanTypeTable As DataTable

	Private moPlanLocalitiesDataAdapter As Common.DbDataAdapter
	Private moPlanLocalitiesTable As DataTable


	Private miObjectID As Integer
	Private mdgvPlaceDetails As System.Windows.Forms.DataGridView
	Private mdicLocalities As Generic.IDictionary(Of Integer, LocalityItem)
	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzInitDB()
		zzFillPlanMain()
		zzFillPlanLocalities()
	End Sub
	Private Sub zzInitDB()
		' 	Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
		Const sDBResourceFile As String = "\\Poseidon\Project\AppData\TopoSolution\tblData.mdb"
		'	Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
		'	Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
		Const sServerName As String = "neptune"
		Const sDatabaseName As String = "Ashqelon"

		components = New System.ComponentModel.Container
		If Not mbInitializedDB Then
			TPlServerDB.ServerDB.InitCurrentServer()
			TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, "")
			mbInitializedDB = TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Closed
			If mbInitializedDB Then
				'	TPlServerDB.ServerDB.AddInitialize()
			End If

			TPlServerDB.ServerDB.InitCurrentProject()
			TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName)
			mbInitializedDB = TPlServerDB.ServerDB.CurrentProjectDB.DBConnectionState <> ConnectionState.Closed
		End If
		miObjectID = 1
		Return
		moPlanMainDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter("SELECT * FROM PlanMainPlus", True)
		moPlanMainTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable("SELECT * FROM PlanMain", "PlanMain")
		moPlanMainTable = New DataTable("PlanMain")
		moPlanMainDataAdapter.Fill(moPlanMainTable)

		Dim oBindingSource As BindingSource = New BindingSource(Me.components)
		oBindingSource.DataSource = moPlanMainTable

		'	Dim oBinding1 As Binding = New Binding("Text", oBindingSource, "ObjectID")
		'		Dim oBinding2 As Binding = New Binding("Text", oBindingSource, "PlanNum")


		'	Stop
		myBindingManagerBase = Me.BindingContext(moPlanMainTable, "PlanNum")


		'	Me.txtObjectID.DataBindings.Add(oBinding1)
		Me.txtPlanName.DataBindings.Add(New Binding("Text", oBindingSource, "PlanName"))
		Me.txtPlanNum.DataBindings.Add(New Binding("Text", oBindingSource, "PlanNum"))
		Me.txtPlanAreaLegal.DataBindings.Add(New Binding("Text", oBindingSource, "PlanAreaLegal"))
		Me.cmbPlanPhase.DataBindings.Add(New Binding("SelectedValue", oBindingSource, "PlanPhase"))
		Me.cmbPlanPhase.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbPlanPhase.DisplayMember = DMCommon.ItemData.DisplayMember

		'	Me.cmbPlanPhase.SelectedValue
		Me.txtEditionNo.DataBindings.Add(New Binding("Text", oBindingSource, "EditionNo"))
		Me.mskEditionDate.DataBindings.Add(New Binding("Text", oBindingSource, "EditionDate"))
		'	Me.txtPlanNum.DataBindings.Add(New Binding("Text", oBindingSource, "PlanNum"))
		'	Me.txtPlanNum.DataBindings.Add(New Binding("Text", oBindingSource, "PlanNum"))
		Me.cmbPlanPhase.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetGeneralList(101, 1)



		'	Me.cmbPlanTypeID.DataBindings.Add(New Binding("SelectedValue", oBindingSource, "PlanTypeID"))
		'	Me.cmbPlanTypeID.ValueMember = DMCommon.ItemData.ValueMember
		'	Me.cmbPlanTypeID.DisplayMember = DMCommon.ItemData.DisplayMember
		'	Me.cmbPlanTypeID.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetGeneralList(3, 1)
		'		Me.chkContDetailInstr.DataBindings.Add(New Binding("Checked", oBindingSource, "ContDetailInstr"))

		'	Me.bnvMain.BindingContext = Me.BindingContext
		myCurrencyManager = oBindingSource.CurrencyManager
		'	Me.bnvMain.BindingSource = oBindingSource

		'	Me.bnvMain.Enabled = True
		'	Me.bnvMain.BindingSource.Add = myCurrencyManager.Binding
		' Set the initial Position of the control.
		myCurrencyManager.Position = 0
		'		myBindingManagerBase.Position = 2
		'	Me.txtPlanName.DataBindings.Add(myBindingManagerBase.Bindings.Item(0))


	End Sub
	Private Sub zzFillPlanMain()
		'	moPlanMain = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable("SELECT * FROM PlanMain", "PlanMain")
		moPlanMainDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter("SELECT * FROM PlanMain WHERE ObjectID=" & CStr(miObjectID), True)
		moPlanMainTable = New DataTable("PlanMain")
		moPlanMainDataAdapter.Fill(moPlanMainTable)
		Dim oBindingSource As BindingSource = New BindingSource(Me.components)
		oBindingSource.DataSource = moPlanMainTable
		Dim oBinding1 As Binding = New Binding("Text", oBindingSource, "ObjectID")
		myBindingManagerBase = Me.BindingContext(moPlanMainTable, "PlanNum")

		Me.txtObjectID.DataBindings.Add(oBinding1)
		Me.txtPlanName.DataBindings.Add(New Binding("Text", oBindingSource, "PlanName"))
		Me.txtPlanNum.DataBindings.Add(New Binding("Text", oBindingSource, "PlanNum"))
		Me.txtPlanAreaLegal.DataBindings.Add(New Binding("Text", oBindingSource, "PlanAreaLegal"))
		Me.cmbPlanPhase.DataBindings.Add(New Binding("SelectedValue", oBindingSource, "PlanPhase"))
		Me.cmbPlanPhase.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbPlanPhase.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.txtEditionNo.DataBindings.Add(New Binding("Text", oBindingSource, "EditionNo"))
		Me.mskEditionDate.DataBindings.Add(New Binding("Text", oBindingSource, "EditionDate"))

		Me.cmbPlanPhase.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetGeneralList(101, 1)


		myCurrencyManager = oBindingSource.CurrencyManager
		'	Me.bnvMain.BindingSource = oBindingSource

	End Sub

	Private Sub zzFillPlanType()
		'	moPlanMain = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable("SELECT * FROM PlanMain", "PlanMain")
		moPlanMainDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter("SELECT * FROM PlanMain WHERE ObjectID=" & CStr(miObjectID), True)
		moPlanMainTable = New DataTable("PlanMain")
		moPlanMainDataAdapter.Fill(moPlanMainTable)
		Dim oBindingSource As BindingSource = New BindingSource(Me.components)
		oBindingSource.DataSource = moPlanMainTable
		Dim oBinding1 As Binding = New Binding("Text", oBindingSource, "ObjectID")
		myBindingManagerBase = Me.BindingContext(moPlanMainTable, "PlanNum")

		'	Me.txtObjectID.DataBindings.Add(oBinding1)
		Me.txtPlanName.DataBindings.Add(New Binding("Text", oBindingSource, "PlanName"))
		Me.txtPlanNum.DataBindings.Add(New Binding("Text", oBindingSource, "PlanNum"))
		Me.txtPlanAreaLegal.DataBindings.Add(New Binding("Text", oBindingSource, "PlanAreaLegal"))
		Me.cmbPlanPhase.DataBindings.Add(New Binding("SelectedValue", oBindingSource, "PlanPhase"))
		Me.cmbPlanPhase.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbPlanPhase.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.txtEditionNo.DataBindings.Add(New Binding("Text", oBindingSource, "EditionNo"))
		Me.mskEditionDate.DataBindings.Add(New Binding("Text", oBindingSource, "EditionDate"))

		Me.cmbPlanPhase.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetGeneralList(101, 1)


		myCurrencyManager = oBindingSource.CurrencyManager
		'	Me.bnvMain.BindingSource = oBindingSource

	End Sub
	Private Sub zzFillPlanLocalities()
		Dim oLocalities As DMCommon.Localities = TPlServerDB.ServerDB.CurrentServerDB.GetLocalities()
		mdicLocalities = oLocalities.Dictionary
		moPlanLocalitiesDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter("SELECT * FROM PlanLocalities WHERE ObjectID=" & CStr(miObjectID), True)
		moPlanLocalitiesTable = New DataTable("PlanLocalities")
		moPlanLocalitiesDataAdapter.Fill(moPlanLocalitiesTable)



		'	Me.ccbLocality.DataBindings.Add(New Binding("SelectedValue", oBindingSource, "PlanPhase"))
		Me.ccbLocality.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbLocality.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbLocality.DataSource = oLocalities.List

		Me.ccbMunicipialStatus.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbMunicipialStatus.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbMunicipialStatus.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetMunicipialStatusesList()

		Me.ccbCommittee.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbCommittee.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbCommittee.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetCommitteesList()

		Me.ccbDistrict.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbDistrict.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbDistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDistrictsList()

		Me.ccbSubdistrict.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbSubdistrict.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbSubdistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSubdistrictsList()

		Me.ccbAuthorityEntire.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbAuthorityEntire.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.ccbAuthorityEntire.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetGeneralList(113, 1)



		Me.dgvPlaceDetailsA.AutoGenerateColumns = False
		Me.dgvPlaceDetailsA.DataSource = moPlanLocalitiesTable
	End Sub
	Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		TPlServerDB.ServerDB.CurrentProjectDB.Close()
	End Sub




	Private Sub cmbPlanPhase_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbPlanPhase.SelectedValueChanged
		Dim o As Object = Me.cmbPlanPhase.SelectedValue
		Dim oItem As DMCommon.ItemData = DirectCast(Me.cmbPlanPhase.SelectedItem, DMCommon.ItemData)

	End Sub





	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Try
			moPlanMainTable.Rows(0).EndEdit()
			moPlanMainDataAdapter.Update(moPlanMainTable)
		Catch ex As Exception
			Stop
		End Try

	End Sub

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

	End Sub

	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
		Dim oDataRowView As DataRowView = DirectCast(Me.mdgvPlaceDetails.Rows.Item(0).DataBoundItem, DataRowView)
		Dim oVal As Object = oDataRowView.Item("LocalityID")
		Dim oCol As DataGridViewComboBoxColumn = DirectCast(Me.mdgvPlaceDetails.Columns.Item(0), DataGridViewComboBoxColumn)
		Dim oColA As DataGridViewComboBoxColumn = DirectCast(Me.mdgvPlaceDetails.Columns.Item(1), DataGridViewComboBoxColumn)
		Dim oList As IList(Of ItemData) = DirectCast(oColA.DataSource, IList(Of ItemData))

		Stop

	End Sub

	 
	Private Sub dgvPlaceDetailsA_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
		Dim oDataGridViewRow As DataGridViewRow
		Dim iValue As Integer
		Dim oLocalityItem As LocalityItem
		Dim oDataRowView As DataRowView
		If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
			oDataGridViewRow = Me.dgvPlaceDetailsA.Rows(e.RowIndex)
			oDataRowView = DirectCast(oDataGridViewRow.DataBoundItem, DataRowView)
			iValue = DirectCast(oDataGridViewRow.Cells.Item(0).Value, Integer)
			oLocalityItem = mdicLocalities.Item(iValue)
			'	oDataGridViewRow.Cells.Item(3).Value = oLocalityItem.District
			oDataRowView.Item(2) = oLocalityItem.MunicipialStatus
			oDataRowView.Item(4) = oLocalityItem.Committee
			oDataRowView.Item(5) = oLocalityItem.District
			oDataRowView.Item(6) = oLocalityItem.Subdistrict
		
		End If


	End Sub
End Class
