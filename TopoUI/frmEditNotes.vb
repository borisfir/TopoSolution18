Option Explicit On
Option Strict On
Imports System.Data
Public Class frmEditNotes

	Private miProjectCode As Integer
	Private miDetailNo As Integer

	Private moMainData As DataRow
	Private moMainDataViewTest As DataRowView

	'Private moPrjOwnersTable As System.Data.DataTable
	Private moMainDataView As System.Data.DataView
	Private moMainDataAdapter As Data.Common.DbDataAdapter
	Private miBlockNo As Integer
	Private miBlockAddNo As Integer
	Private miParcelNo As Integer
	Private WithEvents moBoundTable As DataTable
	'Private moBoundDataView As DataView
	Private moSumDataView As DataView
	Private mhsOwners As HashSet(Of Integer)
	Private mdDecTotal As Double
	Private mdDecCount As Integer
	Private mdaTerms() As Double
	Private miaRowIndecis() As Integer
	Private mdMaxDeviation As Double
	Private miMaxDeviationIndex As Integer
	Private moSimpleTotal As DMCommon.IntMat.Fraction = Nothing
	Private mhsOwnersBefore As HashSet(Of Integer)
	Private mbOwnersInvalid As Boolean
	Private miNewOwnerIndex As Integer
	Private mbEventsEnabled As Boolean = False
	Private mbDirty As Boolean
	'Private mdicSumRows As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
	'Private miCurrentOwnerID As Integer
	'Private miCurrentOwnerRowIndex As Integer

	Public Sub New(ByRef oDataRow As DataRow, ByRef oDataRowView As DataRowView)
		moMainData = oDataRow
		moMainDataViewTest = oDataRowView
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub

	Private Sub zzMyInitializeComponent()
		Me.bnsParcel.DataSource = moMainDataViewTest 'moMainData
		Dim b As Boolean = DMCommon.Functions.CBoolN(moMainDataViewTest.Item("SharedHouse"))
		DMCommon.Debug.MsgBox("13_127f", b)

		Me.txtNote.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Note", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkLeasing.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Leasing"))

		Me.chkParagraph5.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph5"))

		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))
		Me.chkParagraph19.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Paragraph19"))


		Me.chkSharedHouse.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "SharedHouse"))


	End Sub
	Public Property MainDataView As System.Data.DataView
		Get
			Return moMainDataView
		End Get
		Set(oValue As System.Data.DataView)
			moMainDataView = oValue
		End Set
	End Property


	Public Property ParcelNo As Integer
		Get
			Return miParcelNo
		End Get
		Set(iValue As Integer)
			miParcelNo = iValue
		End Set
	End Property
	Public Property BlockNo As Integer
		Get
			Return miBlockNo
		End Get
		Set(iValue As Integer)
			miBlockNo = iValue
		End Set
	End Property
	Public Property BlockAddNo As Integer
		Get
			Return miBlockAddNo
		End Get
		Set(iValue As Integer)
			miBlockAddNo = iValue
		End Set

	End Property











	Private Sub zzLoad()
		zzInitUD_Project()

		'zzCreateBoundTable()
		'zzFillBoundTable()


		'DMCommon.Debug.MsgBox("13_127f", "Load", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
	End Sub
	Private Sub zzCreateBoundTable()
		Dim oOwnerIndexColumn As DataColumn
		Dim oRowIndexColumn As DataColumn

		moBoundTable = New System.Data.DataTable("Data")
		oOwnerIndexColumn = New DataColumn("OwnerIndex", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add(oOwnerIndexColumn)
		oRowIndexColumn = New DataColumn("RowIndex", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add(oRowIndexColumn)


		moBoundTable.Columns.Add("OwnerID", System.Type.GetType("System.Int32"))

		moBoundTable.Columns.Add("PartPct", System.Type.GetType("System.Double"))

		moBoundTable.Columns.Add("Dividend", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Divisor", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Sum", System.Type.GetType("System.Boolean"))
		moBoundTable.Columns.Add("RowType", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Source", System.Type.GetType("System.String"))
		Dim oPrimaryKey() As DataColumn = {oOwnerIndexColumn, oRowIndexColumn}
		moBoundTable.PrimaryKey = oPrimaryKey



	End Sub

	Private Sub zzInitUD_Project()
		'TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		'TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub

	Private Sub zzUpdate()
		'	DMCommon.Debug.MsgBox("13_127c", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
		Dim oNewDataRowView As DataRowView
		For Each oBoundRow As DataRowView In moSumDataView

			oNewDataRowView = moMainDataView.AddNew

			oNewDataRowView.Item("ProjectCode") = miProjectCode
			oNewDataRowView.Item("Detail") = miDetailNo
			oNewDataRowView.Item("BlockNo") = miBlockNo
			oNewDataRowView.Item("BlockAddNo") = miBlockAddNo
			oNewDataRowView.Item("ParcelNo") = miParcelNo


			oNewDataRowView.Item("OwnerID") = oBoundRow.Item("OwnerID")
			oNewDataRowView.Item("PartPct") = oBoundRow.Item("PartPct")
			oNewDataRowView.Item("Dividend") = oBoundRow.Item("Dividend")
			oNewDataRowView.Item("Divisor") = oBoundRow.Item("Divisor")
			oNewDataRowView.EndEdit()

		Next
		'	DMCommon.Debug.MsgBox("13_127e", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
	End Sub






	Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click

		For Each oRow As DataRowView In moMainDataView
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!Ok", oRow.Row.RowState, oRow.Item("OwnerID"))
		Next
		Try
			moMainDataAdapter.Update(moMainDataView.Table)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, Me.Name & "_1")
		End Try


		mbDirty = False
	End Sub
























	Private Sub cmdExit_Click(sender As Object, e As EventArgs) Handles cmdExit.Click
		If Not mbDirty Then
			Me.Close()
		End If
		Me.Close()
	End Sub






	Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
		If mbDirty Then
			mbDirty = False
			For Each oRow As DataRowView In moMainDataView
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Cancel", oRow.Row.RowState, oRow.Item("OwnerID"))
				oRow.Row.RejectChanges()
			Next
			Me.Close()
		End If
	End Sub

	Private Sub bnsParcel_CurrentChanged(sender As Object, e As EventArgs) Handles bnsParcel.CurrentChanged

	End Sub

	Private Sub frmEditNotes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Dim iBlockNo As Integer
		Dim iBlockAddNo As Integer
		Dim iParcelNo As Integer

		iBlockNo = DMCommon.Functions.CIntN(moMainData.Item("BlockNo"))
		iBlockAddNo = DMCommon.Functions.CIntN(moMainData.Item("BlockAddNo"))
		iParcelNo = DMCommon.Functions.CIntN(moMainData.Item("ParcelNo"))
		Me.txtBlockNo.Text = iBlockNo.ToString()
		If iBlockAddNo <> 0 Then
			Me.txtBlockAddNo.Text = iBlockAddNo.ToString()
		End If

		Me.txtParcelNo.Text = iParcelNo.ToString()
	End Sub

	Private Sub chkSharedHouse_CheckedChanged(sender As Object, e As EventArgs) Handles chkSharedHouse.CheckedChanged

	End Sub

	Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles chkTreasurerNote.CheckedChanged

	End Sub






End Class