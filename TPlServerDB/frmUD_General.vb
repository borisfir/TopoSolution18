Option Explicit On
Option Strict On
Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Public Class frmUD_General
	Private Enum enNetType
		Undefined
		NetNew
		Net2005
		Net2012
	End Enum

	Private Enum enPlanType
		Undefined = 0
		PlanType1 = 1
		PlanType2 = 2
		PlanType9 = 9
		PlanType12 = 12

	End Enum


	Private miProjectCode As Integer = 170578
	Private miDetailNo As Integer = 1

	Const msProjectDatabase As String = "UD_Projects" ' "ProjectDataTest"
	'Const sBaseServerName As String = TPlServerDB.ServerDB.DBServerName
	'IG05/12  IGD05/12
	'P:\2006\060169\060169_מסמכי מחלקת תצר\hanit_blocks_20141118\tzr_hanit_sample2.dwg
	Private Const msBorderBlockName As String = "BORDER"
	Private Const msOldBlockName As String = "SU77_2"
	Private Const msDirArrowBlockName As String = "SUNORTHR"
	Private Const msCornerLineLayer As String = "DRWP002"
	Private Const msCornerTextLayer As String = "DRWT004"




	Private Const msTazarFrameBlockName As String = "TAZAR_RISH-MAX"
	Private Const msTazarDirArrowBlockName As String = "TAZAR_ZAFON"
	Private Const msTazarScaleBaseBlockName As String = "TAZAR_SCL"

	Private Const msTazarMapBlockLayer As String = "C1680"
	Private Const msNotesLayer As String = "C1682"


	Private Const msGenBlockName As String = "C1640"
	Private Const msGenBlockLayer As String = "C1640"
	Private Const msNoteBlockLayer As String = "C1641"


	Private Const msEllipseBlockName As String = "C1642"
	Private Const msEllipseBlockLayer As String = "C1642"


	Private Const msEllipseTTGBlockName As String = "C1644"
	Private Const msEllipseTTGBlockLayer As String = "C1644"


	Private Const msStampBlockNameAAA As String = "C1643"
	Private Const msDmAddressBlockName As String = "DM_Address"
	Private Const msPlanNumberBlockName As String = "SU77_11"


	Private Const msStampBlockLayer As String = "C1643"



	Private Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit"
	' Private Const msFrameComLine As String = "(load \""M:/Dm_Work/Support2/SURVEY2.LSP\"")(C:SURFG) "
	Private Const msFrameComLine1 As String = "(load ""M:/Dm_Work/Support2/SURVEY2.LSP"")(setq MYCALLBACK ""FormView"")(C:SURFG)(setq MYCALLBACK nil)"
	Private Const msFrameComLine As String = "(setq MYCALLBACK ""FormView"")(load ""M:/Dm_Work/Support2/SURVEY2.LSP"")(C:SURFG)(setq MYCALLBACK nil)"
	Private Const msDistrictAttribTag As String = "REGION"
	Private Const msSubdistrictAttribTag As String = "COUNTY"
	Private Const msLocalityNameAttribTag As String = "SETTLEMENT"
	Private Const msLocalityCodeAttribTag As String = "SETTLEMENT_CODE"
	Private Const msGushAttribTag As String = "GUSH_NUM"
	Private Const msParcelsAttribTag As String = "PARCELS"
	Private Const msOrdererAttribTag As String = "ORDERER"

	Private Const msPlanTypeAttribTag As String = "PROCESS_TYPE"

	Private Const msWorkOrderNumAttribTag As String = "WORK_ORDER"

	Private Const msNetAttribTag As String = "GRID_NAME"
	Private Const msScaleAttribTag As String = "SCALE"

	Private Const msTabaNameAttribTag As String = "TABA_NAMES"
	Private Const msSurveyDate1AttribTag As String = "SURVEY_DATE_1"
	Private Const msSurveyDate2AttribTag As String = "SURVEY_DATE_2"

	Private Const msEndDateAttribTag As String = "FINISH_DATE"
	Private Const msUpdateDateAttribTagOld As String = "UPDATE_DATE"
	Private Const msUpdateDateAttribTag As String = "SURVEY_DATE"

	Private Const msGushLegalAreaAttribTag As String = "GUSH_LEGAL_AREA"
	Private Const msGushRegStatusNameAttribTag As String = "GUSH_REGISTER_STATUS"

	Private Const msGushLastParcelNameAttribTag As String = "GUSH_LAST_PARCEL"
	Private Const msGeneralCommentNameAttribTag As String = "GENERAL_COMMENT"
	Private Const msGBPrefix As String = "ג.ב."
	Private Const msProcessNameAttribTag As String = "PROCESS_NAME"
	Private Const msSerialNumAttribTag As String = "SERIAL"

	Private Const msPlanNumberAttribTag As String = "PLANNUMBER"



	Private Const msPlaceAttribTag As String = "PLACE"
	Private Const msSurveyorIDAttribTag As String = "SURVEYOR"
	Private Const msSurveyorNameAttribTag As String = "SURVEYOR_NAME"

	Private Const msPlanType9 As String = "קמ""ק"
	Private Const msPlanType12 As String = "תת""ג"
	Private Const msPlanType2 As String = "תצ""ר"
	Private Const msPlanType1 As String = "הסדר"

	Private Const mdFrameHeight As Double = 600
	Private Const mdFrameWidth As Double = 700
	Private mdCurrentFrameWidth As Double
	Private mdCurrentFrameHeight As Double



	'Dim miProjectCode As Integer
	'Dim miDetailNo As Integer
	Private moProjectDataTable As System.Data.DataTable
	Private moProjectDataAdapter As Data.Common.DbDataAdapter


	Private moDataRow As DataRow
	Private miPlanType As Integer
	Private mbEventsEnabled As Boolean



	Private msGBDOSEnd As String = New String({Chr(46), Chr(129), Chr(46), Chr(130)})

	Private moaNoteChecks(17) As NoteCheck

	'	Dim c As Char = New Char()

	Dim chars() As Char = {ChrW(&H61), ChrW(&H308)}




	Private Const mdDeltaPosX As Double = 400
	Private Const mdDeltaPosY As Double = 250

	Private mdNoteMarginX As Double = 44.2
	Private mdNoteMarginY As Double = 18.8
	Private mdNoteRowHeight As Double = 6


	Private WithEvents moProjectPlanTable As System.Data.DataTable
	Private moProjectPlanDataAdapter As Data.Common.DbDataAdapter
	'Private moProjectDataAdapter As Data.Common.DbDataAdapter
	Private moNewRow As DataRow
	Private moNewObject As System.Object


	'  Private moScaleAcadBlock As DMAcadExt.AcadBlock
	Private msGush As String
	Private msParcels As String
	Private mbDBSource As Boolean

	Private msServerDataSource As String = TPlServerDB.ServerDB.DBServerName
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

	Public Shared Function GetPlanTypeName(iPlanType As Integer) As String
		Select Case iPlanType
			Case 1
				Return msPlanType1
			Case 2
				Return msPlanType2
			Case 9
				Return msPlanType9
			Case 12
				Return msPlanType12
			Case Else
				Return Nothing
		End Select
	End Function
	Public Sub New()
		Const sBaseServerName As String = TPlServerDB.ServerDB.DBServerName
		Const msServerDataBase As String = "ProjectData"
		' This call is required by the designer.
		InitializeComponent()
		TPlServerDB.ServerDB.InitCurrentServer()
		TPlServerDB.ServerDB.InitCurrentProject()
		msServerDataSource = sBaseServerName
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, msProjectDatabase, True, False)
		TPlServerDB.ServerDB.CurrentServerDB.SetSQL(msServerDataSource, msServerDataBase, True, False)
		zzMyInitializeComponent()
		' Add any initialization after the InitializeComponent() call.







	End Sub


	Public Sub New(bDBSource As Boolean)
		Const msServerDataBase As String = "ProjectData"
		'TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		'TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		' This call is required by the designer.
		InitializeComponent()
		TPlServerDB.ServerDB.InitCurrentServer()
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, msProjectDatabase, True, False)
		TPlServerDB.ServerDB.CurrentServerDB.SetSQL(msServerDataSource, msServerDataBase, True, False)
		zzMyInitializeComponent()
		mbDBSource = bDBSource
		If mbDBSource Then
			'	zzInitUD_Project()
			zzLoadProjectDataTable(False)
		End If
		' Add any initialization after the InitializeComponent() call.
		'sServerDataSource = sBaseServerName

		zzInit()
		zzSetAttribTags()

		zzSetToolTip()


	End Sub
	Private Sub zzInitUD_Project()
		'	topoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		'	TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TopoManager.TPlanGraph.TplnProject.ServerDataSource, frmUnidiv.ProjectDataBase, True)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
	End Sub

	Public Property GushName As String
		Get
			Return Me.txtGush.Text
		End Get
		Set(sValue As String)
			Me.txtGush.Text = sValue
		End Set
	End Property
	Public Property NormalParcels As String
		Get
			Return Me.txtNormalParcels.Text
		End Get
		Set(sValue As String)
			Me.txtNormalParcels.Text = sValue
		End Set
	End Property
	Public Property TempParcels As String
		Get
			Return Me.txtTempParcels.Text
		End Get
		Set(sValue As String)
			Me.txtTempParcels.Text = sValue
		End Set
	End Property
	Public Sub SetPlanType(bIsTTG As Boolean)
		If bIsTTG Then
			Me.rdbPlanType12.Checked = True
		Else
			Me.rdbPlanType2.Checked = True
		End If
		'   MessageBox.Show(CStr(Me.rdbPlanType2.Checked) & vbCrLf & CStr(Me.rdbPlanType12.Checked), "05_460")
	End Sub
	Private Sub zzSetPlanType()
		Dim oDynValue As System.Object
		oDynValue = moDataRow.Item("PlanType")
		miPlanType = DMCommon.Functions.CIntN(oDynValue)
		Select Case miPlanType
			Case 1
				Me.rdbPlanType1.Checked = True
			Case 0, 2
				Me.rdbPlanType2.Checked = True
			Case 9
				Me.rdbPlanType9.Checked = True
			Case 12
				Me.rdbPlanType12.Checked = True
		End Select

		'   MessageBox.Show(CStr(Me.rdbPlanType2.Checked) & vbCrLf & CStr(Me.rdbPlanType12.Checked), "05_460")
	End Sub

	''Public Sub zzInitOld()
	''  Dim oCheck As System.Windows.Forms.CheckBox
	'   moaNoteChecks(0) = New NoteCheck(Me.chkNote01)
	'   moaNoteChecks(1) = New NoteCheck(Me.chkNote02)
	'   moaNoteChecks(2) = New NoteCheck(Me.chkNote03)
	'   moaNoteChecks(3) = New NoteCheck(Me.chkNote04)
	'   moaNoteChecks(4) = New NoteCheck(Me.chkNote05)
	'   moaNoteChecks(5) = New NoteCheck(Me.chkNote06)
	'   moaNoteChecks(6) = New NoteCheck(Me.chkNote07)
	'   moaNoteChecks(7) = New NoteCheck(Me.chkNote08)
	'   moaNoteChecks(8) = New NoteCheck(Me.chkNote09)
	'   moaNoteChecks(9) = New NoteCheck(Me.chkNote10)
	'   moaNoteChecks(10) = New NoteCheck(Me.chkNote11)
	'   moaNoteChecks(11) = New NoteCheck(Me.chkNote12)
	'   moaNoteChecks(12) = New NoteCheck(Me.chkNote13)
	'   moaNoteChecks(13) = New NoteCheck(Me.chkNote14)
	'   moaNoteChecks(14) = New NoteCheck(Me.chkNote15)
	'   moaNoteChecks(15) = New NoteCheck(Me.chkNote16)
	'   moaNoteChecks(16) = New NoteCheck(Me.chkNote17)
	'   moaNoteChecks(17) = New NoteCheck(Me.chkNote18)

	'End Sub
	Private Sub zzInit()
		Dim oCheck As System.Windows.Forms.CheckBox
		'moaNoteChecks(0) = New NoteCheck(Me.chkNote01)
		'moaNoteChecks(1) = New NoteCheck(Me.chkNote02)
		'moaNoteChecks(2) = New NoteCheck(Me.chkNote03)
		'moaNoteChecks(3) = New NoteCheck(Me.chkNote04)
		'moaNoteChecks(4) = New NoteCheck(Me.chkNote05)
		'moaNoteChecks(5) = New NoteCheck(Me.chkNote06)
		'moaNoteChecks(6) = New NoteCheck(Me.chkNote07)
		'moaNoteChecks(7) = New NoteCheck(Me.chkNote08)
		'moaNoteChecks(8) = New NoteCheck(Me.chkNote09)
		'moaNoteChecks(9) = New NoteCheck(Me.chkNote10)
		'moaNoteChecks(10) = New NoteCheck(Me.chkNote11)
		'moaNoteChecks(11) = New NoteCheck(Me.chkNote12)
		'moaNoteChecks(12) = New NoteCheck(Me.chkNote13)
		'moaNoteChecks(13) = New NoteCheck(Me.chkNote14)
		'moaNoteChecks(14) = New NoteCheck(Me.chkNote15)
		'moaNoteChecks(15) = New NoteCheck(Me.chkNote16)
		'moaNoteChecks(16) = New NoteCheck(Me.chkNote17)
		'moaNoteChecks(17) = New NoteCheck(Me.chkNote18)
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			oCheck = DirectCast(Me.Controls.Item("chkNote" & Format(iIndex + 1, "00")), CheckBox)
			moaNoteChecks(iIndex) = New NoteCheck(oCheck)
		Next






	End Sub
	Private Sub zzLoadProjectDataTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetProjectData"

		moProjectDataTable = New System.Data.DataTable("ProjectData")
		moProjectDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)

		If bSchemaOnly Then
			moProjectDataAdapter.FillSchema(moProjectDataTable, SchemaType.Source)
		Else
			moProjectDataAdapter.Fill(moProjectDataTable)
		End If
		'   moFragmentTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))


		If moProjectDataTable.Rows.Count = 0 Then
			moDataRow = moProjectDataTable.NewRow()
		Else
			moDataRow = moProjectDataTable.Rows.Item(0)

		End If

		zzSetPlanType()
		'	zzSetMainData()

	End Sub
	Private Sub zzSetBinding()
		Me.cmbDistrict.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "DistrictID"))
		Me.cmbSubdistrict.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "SubdistrictID"))
		Me.cmbLocality.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "LocalityID"))
		Me.cmbScales.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "ScaleID"))


		Me.mskPlanID.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanID"))
		'	Dim oBinding As Binding = New Binding("Text", Me.bnsPlans, "OriginalBlockNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)

		Me.txtGush.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalBlockNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing)) ' Tru

		Me.txtNormalParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru
		Me.txtTempParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "NewParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtPlanNum.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanNum", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.txtPlanPHNum.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanPHNum", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))



		Me.txtWorkOrder.DataBindings.Add(New Binding("Text", Me.bnsPlans, "WorkOrder", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		'	Me.txtProcessName.DataBindings.Add(New Binding("Text", Me.bnsPlans, "ProcessName", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtGushLegalArea.DataBindings.Add(New Binding("Text", Me.bnsPlans, "LegalArea", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtLastParcelName.DataBindings.Add(New Binding("Text", Me.bnsPlans, "LastParcel", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.txtTaskNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "TaskNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtBaseSerialNumber.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseSerialNumber", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		'	Me.txtBaseProcessName.DataBindings.Add(New Binding("Text", Me.bnsPlans, "ProcessName", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtBaseGush.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseBlockNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.mskEstimateDate.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseApproveDate", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "d"))


		Me.chkGushStatus.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "BlockIsRegulated", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

			Me.txtOrderer.DataBindings.Add(New Binding("Text", Me.bnsProject, "Orderer"))

			Me.cmbSurveyor.DataBindings.Add(New Binding("SelectedValue", Me.bnsProject, "SurveyorID"))

		Me.chkNote01.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note01"))
			Me.chkNote02.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note02"))
			Me.chkNote03.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note03"))
			Me.chkNote04.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note04"))

			Me.chkNote05.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note05"))
			Me.chkNote06.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note06"))

			Me.chkNote07.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note07"))
			Me.chkNote08.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note08"))
			Me.chkNote09.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note09"))
			Me.chkNote10.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note10"))
			Me.chkNote11.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note11"))
			Me.chkNote12.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note12"))
			Me.chkNote13.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note13"))
			Me.chkNote14.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note14"))
			Me.chkNote15.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note15"))
			Me.chkNote16.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note16"))

			Me.chkNote17.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "Note17"))

			Me.chkNote18.DataBindings.Add(New Binding("Checked", Me.bnsProject, "Note18"))

	End Sub
	Private Sub zzSetMainData()

		Me.cmbDistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("DistrictID"))
		Me.cmbSubdistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("SubdistrictID"))
		Me.cmbLocality.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("Locality"))
		'	Me.txtOrderer.Text = DMCommon.Functions.CStrN(moDataRow.Item("Orderer"))
		'	DMCommon.Debug.MsgBox("13_023", DMCommon.Functions.CIntN(moDataRow.Item("District")), Me.cmbDistrict.SelectedValue, Me.cmbSubdistrict.SelectedValue)

	End Sub
	Private Function zzGetParameters() As System.Data.Common.DbParameter()
		Dim oaParams(1) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)

		' DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function

	Private Sub zzMyInitializeComponent()
		'	System.Windows.Forms.MessageBox.Show("OK:", "ServerDB - GetLocalityList")
		'Dim oDistricts As System.Collections.Generic.IList(Of DMCommon.ItemData) = TPlServerDB.ServerDB.CurrentServerDB.GetDistrictsList()

		Me.cmbDistrict.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbDistrict.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbDistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDistrictsList()


		Me.cmbSubdistrict.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbSubdistrict.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbSubdistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSubdistrictsList()

		Me.cmbLocality.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbLocality.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbLocality.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetLocalityList()


		Me.cmbScales.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbScales.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbScales.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetScalesList()


		Me.cmbSurveyor.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSurveyorLicenseView
		Me.cmbSurveyor.ValueMember = "LicenseNo"
		Me.cmbSurveyor.DisplayMember = "Empl_NameFamily"
		'חדשה/2005/2012
		If False Then
			Me.dtpEndDate.Value = DateTime.Today
			Me.dtpEndDate.Checked = False
			MessageBox.Show(Me.dtpSurveyDate1.Value.ToString(), "04_230")
			Me.dtpSurveyDate1.Value = DateTime.Today.AddMonths(-12)

			MessageBox.Show(Me.dtpSurveyDate1.Value.ToString(), "04_240")
			Me.dtpSurveyDate.Value = DateTime.Today
		End If

	End Sub



	Private Sub frmUD_General_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		'	Me.cmbDistrict.SelectedIndex = -1
		zzLoadProjectDataTable(False)
		zzLoadProjectPlanTable(False)
		Me.bnsPlans.DataSource = moProjectPlanTable
		Me.bnnPlans.BindingSource = Me.bnsPlans
		Me.bnsProject.DataSource = moProjectDataTable

		'	zzLoadBlockRefData(True)
		'zzSetScale()
		zzSetBinding()
	End Sub








	Private Sub cmdSaveDB_Click(oSender As System.Object, e As System.EventArgs) Handles cmdSaveDB.Click

		zzGetPlanType()
		If False Then
			moDataRow.Item("PlanType") = miPlanType
			moDataRow.Item("District") = Me.cmbDistrict.SelectedValue
			moDataRow.Item("Subdistrict") = Me.cmbSubdistrict.SelectedValue
			moDataRow.Item("Locality") = Me.cmbLocality.SelectedValue
			moDataRow.Item("Orderer") = Me.txtOrderer.Text
			If moDataRow.RowState = DataRowState.Detached Then
				moDataRow.Item("ProjectCode") = miProjectCode
				moDataRow.Item("Detail") = miDetailNo

				moProjectDataTable.Rows.Add(moDataRow)
			End If
		End If




		'DMCommon.Debug.MsgBox("12_190c", moDataRow.RowState)
		'DMCommon.Debug.MsgBox("12_190", moProjectDataTable.Rows.Count, moProjectPlanTable.Rows.Count)

		For Each oRow As DataRow In moProjectDataTable.Rows
			oRow.EndEdit()
			DMCommon.Debug.MsgBox("12_190a", oRow.RowState)
		Next
		moProjectPlanTable.Rows.Item(0).EndEdit()
		DMCommon.Debug.MsgBox("12_190d", moProjectPlanTable.Rows.Item(0).RowState)
		moProjectDataAdapter.Update(moProjectDataTable)
		moProjectPlanDataAdapter.Update(moProjectPlanTable)

	End Sub



	Private Function zzGetLicenseNo(ByRef sLicenseNo As String, ByRef sLicenseName As String) As Boolean
		If cmbSurveyor.SelectedIndex <> -1 Then
			Dim oRow As System.Data.DataRowView = TryCast(cmbSurveyor.SelectedItem, System.Data.DataRowView)
			If oRow IsNot Nothing Then
				sLicenseNo = oRow.Item("LicenseNo").ToString()
				sLicenseName = oRow.Item("Empl_NameFamily").ToString()


				Return True
			Else
				MessageBox.Show(cmbSurveyor.SelectedItem.ToString(), "04_840")
				Return False
			End If


		Else
			Return False
		End If
	End Function
	Private Function zzGetPlanType() As String

		If Me.rdbPlanType1.Checked Then
			miPlanType = 1
		ElseIf Me.rdbPlanType2.Checked Then
			miPlanType = 2
		ElseIf Me.rdbPlanType9.Checked Then
			miPlanType = 9
		ElseIf Me.rdbPlanType12.Checked Then
			miPlanType = 12
		End If

		If miPlanType <> 0 Then
			Return Convert.ToString(miPlanType)
		Else
			Return String.Empty
		End If
	End Function


	Private Function zzGetEllipseData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		dicAttribValues.Add(msProcessNameAttribTag, Me.txtProcessName.Text)
		dicAttribValues.Add(msSerialNumAttribTag, Me.mskSerialNum.Text & " " & msGBDOSEnd)
		Return dicAttribValues

	End Function
	Private Function zzGetStampData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim oDynValue As System.Object
		If dtpSurveyDate1.Checked Then
			dicAttribValues.Add(msSurveyDate1AttribTag, dtpSurveyDate1.Text)
		End If
		If dtpSurveyDate2.Checked Then
			dicAttribValues.Add(msSurveyDate2AttribTag, dtpSurveyDate2.Text)
		End If
		If dtpEndDate.Checked Then
			dicAttribValues.Add(msEndDateAttribTag, Me.dtpEndDate.Text)
		End If


		If Me.dtpSurveyDate.Checked Then
			dicAttribValues.Add(msUpdateDateAttribTag, Me.dtpSurveyDate.Text)
		End If

		If cmbLocality.SelectedIndex <> -1 Then
			dicAttribValues.Add(msLocalityNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbLocality.Text, True))
			oDynValue = cmbLocality.SelectedValue
			dicAttribValues.Add(msPlaceAttribTag, oDynValue.ToString())

		End If

		If cmbSurveyor.SelectedIndex <> -1 Then
			dicAttribValues.Add(msSurveyorNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbSurveyor.Text, True))
			oDynValue = cmbSurveyor.SelectedValue
			dicAttribValues.Add(msSurveyorIDAttribTag, oDynValue.ToString())

		End If








		Return dicAttribValues

	End Function
	Private Function zzGetPlanNumberData() As Dictionary(Of String, String)
		Dim saSerialNumber() As String = Split(Me.mskSerialNum.Text, "/") 'Continue
		Dim iUB As Integer = saSerialNumber.GetUpperBound(0)

		'  DMCommon.Functions.DispArray(saSerialNumber, "saSerialNumber", True)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		If iUB > 0 Then
			dicAttribValues.Add(msPlanNumberAttribTag, saSerialNumber(iUB))
		End If


		Return dicAttribValues
	End Function
	Private Sub zzParseDate(sAttribValue As String, ByRef oDatePic As DateTimePicker)
		If sAttribValue.Length = 0 Then
			oDatePic.Checked = False
		Else
			Dim cultures() As System.Globalization.CultureInfo = {New System.Globalization.CultureInfo("en-US"),
													New System.Globalization.CultureInfo("fr-FR"),
													New System.Globalization.CultureInfo("it-IT"),
													New System.Globalization.CultureInfo("de-DE")}

			Dim dtValue As DateTime
			If DateTime.TryParse(sAttribValue, dtValue) Then
				oDatePic.Value = dtValue
				' MessageBox.Show(sAttribValue & vbCrLf & oDatePic.Value.ToString() & vbCrLf & oDatePic.Value.ToString() & vbCrLf & oDatePic.Name, "OK!!! ")
			Else
				sAttribValue = Replace(sAttribValue, ".", "/")
				If DateTime.TryParse(sAttribValue, cultures(1), Globalization.DateTimeStyles.None, dtValue) Then
					oDatePic.Value = dtValue
				Else
					MessageBox.Show(sAttribValue & vbCrLf & sAttribValue, "Err Date")
				End If

			End If
		End If

	End Sub
	Private Sub zzParsePlanType(sAttribValue As String)
		Select Case sAttribValue
			Case "1"
				Me.rdbPlanType1.Checked = True
			Case "2"
				Me.rdbPlanType2.Checked = True
			Case "12"
				Me.rdbPlanType12.Checked = True
			Case String.Empty
				If Me.rdbPlanType1.Checked Then
					Me.rdbPlanType1.Checked = False
				ElseIf Me.rdbPlanType2.Checked Then
					Me.rdbPlanType2.Checked = False
				ElseIf Me.rdbPlanType12.Checked Then
					Me.rdbPlanType12.Checked = False
				End If
			Case Else
				MessageBox.Show(sAttribValue & vbCrLf & "", "Err: " & msPlanTypeAttribTag)
		End Select

	End Sub
	Private Function zzGetNetType() As String
		'IG05/12
		If Me.rdbNetNew.Checked Then
			Return "ITM"
		ElseIf Me.rdbNet2005.Checked Then
			Return "IG12"
		ElseIf Me.rdbNet2012.Checked Then
			Return "IG05"
		Else
			Return String.Empty
		End If
	End Function
	Private Function zzGetNetTypeOld() As String
		'IG05/12
		If Me.rdbNetNew.Checked Then
			Return DMCommon.Hebrew.WordToDOS("חדשה", True)
		ElseIf Me.rdbNet2005.Checked Then
			Return "2012"
		ElseIf Me.rdbNet2012.Checked Then
			Return "2005"
		Else
			Return String.Empty
		End If

	End Function
	Private Sub zzParseNetType(sAttribValue As String)

		Select Case sAttribValue.ToUpper()
			Case DMCommon.Hebrew.WordToDOS("חדשה", True), "ITM"
				Me.rdbNetNew.Checked = True

			Case "2012", "IG12"
				Me.rdbNet2012.Checked = True
			Case "2005", "IG05"
				Me.rdbNet2005.Checked = True
			Case String.Empty
				If Me.rdbNet2005.Checked Then
					Me.rdbNet2005.Checked = False
				ElseIf Me.rdbNet2012.Checked Then
					Me.rdbNet2012.Checked = False
				ElseIf Me.rdbNetNew.Checked Then
					Me.rdbNetNew.Checked = False
				End If
			Case Else
				MessageBox.Show(DMCommon.Hebrew.FromAcad(sAttribValue) & vbCrLf & "חדשה" & vbCrLf & "2012" & vbCrLf & "2005", "Err: " & msNetAttribTag)
		End Select
	End Sub
	Private Sub zzParseGushStatus(sAttribValue As String)
		Dim sWinValue As String = DMCommon.Hebrew.FromAcad(sAttribValue)
		Select Case sWinValue
			Case String.Empty
				Me.chkGushStatus.CheckState = CheckState.Indeterminate
			Case "מוסדר"
				Me.chkGushStatus.Checked = True
			Case "לא מוסדר"
				Me.chkGushStatus.Checked = False
			Case Else
				MessageBox.Show(sWinValue & vbCrLf & "מוסדר" & vbCrLf & "לא מוסדר", "Err: " & msGushRegStatusNameAttribTag)
		End Select
	End Sub
	Private Function zzGetGushStatus() As String
		If Me.chkGushStatus.Checked Then
			Return "מוסדר"
		Else
			Return "לא מוסדר"
		End If
	End Function






	Private Sub zzSetToolTip()
		'  Const sOKTip1 As String = "הכנסת נתונים למערכת"
		'   Const sCloseTip As String = "יציאה ללא שמירה"

		Const sSurveyDateTip As String = "תאריך מדידת הפרטים"
		Const sSurveyDate1Tip As String = "תאריך מדידת סימני הגבול הישנים שנמצאו או שוחזרו בשדה"
		Const sSurveyDate2Tip As String = "תאריך מדידת סימני הגבול האחרים"


		' Create the ToolTip and associate with the Form container.
		Dim oToolTip As New ToolTip()

		' Set up the delays for the ToolTip.
		oToolTip.AutoPopDelay = 5000
		oToolTip.InitialDelay = 1000
		oToolTip.ReshowDelay = 500
		' Force the ToolTip text to be displayed whether or not the form is active.
		oToolTip.ShowAlways = True
		oToolTip.SetToolTip(Me.dtpSurveyDate, sSurveyDateTip)
		oToolTip.SetToolTip(Me.lblSurveyDate, sSurveyDateTip)

		oToolTip.SetToolTip(Me.dtpSurveyDate1, sSurveyDate1Tip)
		oToolTip.SetToolTip(Me.lblSurveyDate1, sSurveyDate1Tip)


		oToolTip.SetToolTip(Me.dtpSurveyDate2, sSurveyDate2Tip)
		oToolTip.SetToolTip(Me.lblSurveyDate2, sSurveyDate2Tip)




	End Sub





	Private Sub zzOKScale()

		'moScaleAcadBlock.LoadAllReferences()

		'If moScaleAcadBlock.ReferenceCount = 0 Then
		'   If moFrameInsPoint IsNot Nothing Then
		'      moScaleAcadBlock.OpenForRight()
		'      If moScaleAcadBlock.DefinitionExists Then

		'         Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
		'         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		'         tBlockRefData.Position = oBlockInsPoint.AcGePoint3d

		'         tBlockRefData.Layer = msTazarMapBlockLayer
		'         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
		'         moScaleAcadBlock.InsertRefNew(tBlockRefData)
		'      End If
		'   End If
		'End If

	End Sub



	Private Sub zzParseSerialNum(sSerialNum As String)
		Dim oHebText As DMCommon.HebrewTrans

		sSerialNum = sSerialNum.Trim()

		'  System.Windows.Forms.MessageBox.Show(sSerialNum & vbCrLf & msGBDOSEnd & vbCrLf & "Index=" & CStr(234) & vbCrLf & sSerialNum.EndsWith(msGBDOSEnd).ToString(), "Serial!!")
		If sSerialNum.EndsWith(msGBDOSEnd) Then
			Dim sOut As String = sSerialNum.Substring(0, sSerialNum.Length - 4)
			oHebText = New DMCommon.HebrewTrans(sOut, False)
			'Me.txtSerialNum.Text = oHebText.GetWinDest(False)
		Else
			oHebText = New DMCommon.HebrewTrans(sSerialNum, False)
			'Me.txtSerialNum.Text = oHebText.GetWinDest(False)
		End If


	End Sub
	Private Sub zzCheckLocality(sLocalityCode As String, sLocalityName As String)
		If sLocalityCode IsNot Nothing AndAlso sLocalityName IsNot Nothing Then
			If cmbLocality.SelectedValue.ToString() <> sLocalityCode Then
				MessageBox.Show(cmbLocality.SelectedValue.ToString() & vbCrLf & sLocalityCode, "Err: " & msLocalityCodeAttribTag)
			End If
		End If
	End Sub








	Private Sub cmdLoadBlockData_Click(sender As System.Object, e As System.EventArgs) Handles cmdLoadBlockData.Click
		Dim oControlBindingsCollection As ControlBindingsCollection = Me.txtGush.DataBindings
		Dim oBindings As Binding = oControlBindingsCollection.Item(0)
		Dim o1 As System.Object = oBindings.DataSource

		Dim oBS As BindingSource = TryCast(o1, BindingSource)
		Dim s1 As String = oBindings.PropertyName
		Dim oBMI As BindingMemberInfo = oBindings.BindingMemberInfo
		Dim sFld As String = oBMI.BindingField
		'oBindings.DataSource
		Dim oRow As DataRowView = TryCast(oBS.Current, DataRowView)
		Dim oRes As System.Object = oRow.Item(sFld)
	End Sub






	Private Sub cmdExit_Click(sender As System.Object, e As System.EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Function zzFilterList(ByVal sValue As String) As String
		Dim sRes As String = String.Empty
		Dim saGroups() As String = Split(sValue, ",")

		For iIndex As Integer = 0 To saGroups.GetUpperBound(0)
			saGroups(iIndex).Trim()
			If (Not saGroups(iIndex).StartsWith("[")) AndAlso (Not saGroups(iIndex).EndsWith("]")) Then
				If sRes.Length <> 0 Then
					sRes &= ","
				End If
				sRes &= saGroups(iIndex)
			End If
		Next
		Return sRes
	End Function



	Private Sub cmdTempParcelInvert_Click(oSender As System.Object, e As System.EventArgs) Handles cmdTempParcelInvert.Click
		Me.txtTempParcels.Text = DMCommon.Hebrew.InvertList(Me.txtTempParcels.Text)
	End Sub
	Private Class NoteCheck
		Const msPlaceHolderChar As String = "_"
		Const miVarPlacesMax As Integer = 5
		Const msFontStyle As String = "HEBTEXT"
		'rivate Shared mtTextStyleTableRecordID As ObjectId
		Private Shared mdLeft As Double
		Private Shared mdTextHeight As Double
		Private moCheckControl As CheckBox
		Private ReadOnly miControlIndex As Integer
		Private msText As String
		Private miPlaceHolderMin As Integer = 3
		Private msPlaceHolder As String = StrDup(miPlaceHolderMin, msPlaceHolderChar)
		Private miCurrentPosition As Integer = 1
		Private miCurrentPlace As Integer
		Dim taVarPlaces(miVarPlacesMax) As VarPlace
		Private miVarPlacesCount As Integer
		Private msaValue() As String
		Private msaAcadValue() As String
		Private mdicAtribValues As Generic.Dictionary(Of String, String)




		Private msaAttribTag() As String

		Private mdNoteRowHeight As Double = 5.6

		'Private mtGenBlockRecObjID As ObjectId
		'Private mtGenBlockRefObjID As ObjectId
		Public Shared Sub SetParam(dLeft As Double, dTextHeight As Double)
			'	zzInitTextSile(msFontStyle)
			mdLeft = dLeft
			mdTextHeight = dTextHeight
		End Sub


		Public Sub New(oCheckControl As CheckBox)
			Dim sName As String = oCheckControl.Name
			miControlIndex = Convert.ToInt32(sName.Substring(7)) - 1
			moCheckControl = oCheckControl
			msText = moCheckControl.Text


			For miCurrentPlace = 0 To miVarPlacesMax
				If Not CalcPlace() Then

					miVarPlacesCount = miCurrentPlace
					Return
				End If
			Next
			miVarPlacesCount = miVarPlacesMax + 1
		End Sub
		Private Function CalcPlace() As Boolean
			Dim sChar As String
			Dim sTest As String = DMCommon.Hebrew.DispASC(msText, False)
			Dim iPos As Integer = InStr(miCurrentPosition, msText, msPlaceHolder)


			If iPos > 0 Then
				taVarPlaces(miCurrentPlace).StartPosition = iPos

				For iIndex As Integer = iPos + miPlaceHolderMin To msText.Length
					sChar = Mid(msText, iIndex, 1)
					If sChar <> msPlaceHolderChar Then
						taVarPlaces(miCurrentPlace).EndPosition = iIndex - 1
						miCurrentPosition = iIndex
						Return True
					End If
				Next
				taVarPlaces(miCurrentPlace).EndPosition = msText.Length
				miCurrentPosition = msText.Length + 1
				Return True

			Else
				Return False
			End If






		End Function



		Public ReadOnly Property RowsNumber As Integer
			Get
				Return (moCheckControl.Height - 4) \ 16
			End Get
		End Property
		Public ReadOnly Property BlockNameOld As String
			Get
				Return "HEAN" & CStr(miControlIndex)
			End Get
		End Property
		Public ReadOnly Property BlockName As String
			Get
				Return "UD_Note" & Format(miControlIndex + 1, "00")
			End Get
		End Property
		Public ReadOnly Property Item(iIndex As Integer) As String
			Get
				If mdicAtribValues IsNot Nothing Then
					Return mdicAtribValues.Item(msaAttribTag(iIndex))
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public ReadOnly Property AttributeCount As Integer
			Get
				If mdicAtribValues Is Nothing Then
					Return 0
				Else
					Return mdicAtribValues.Count
				End If
				Return mdicAtribValues.Count
			End Get
		End Property
		Public Property Checked As Boolean
			Get
				Return moCheckControl.Checked
			End Get
			Set(bValue As Boolean)
				moCheckControl.Checked = bValue
			End Set
		End Property
		Public Property NoteNo As Integer
			Get
				If String.IsNullOrEmpty(msaValue(0)) Then
					Return 0
				Else
					Return Convert.ToInt32(msaValue(0))
				End If

			End Get
			Set(iValue As Integer)
				If msaValue IsNot Nothing AndAlso msaValue.GetUpperBound(0) >= 0 Then
					If msaAcadValue Is Nothing Then
						msaValue(0) = Convert.ToString(iValue)
					Else
						msaAcadValue(0) = Convert.ToString(iValue)
					End If


				End If

			End Set
		End Property

		Public Sub AddVarData(saValue() As String)
			msaValue = saValue

			If miControlIndex > 105 Then
				DMCommon.Functions.DispArray(msaValue, "msaValue", True)

				MessageBox.Show(zzGetFullText(msaValue), "04_984")
			End If
			moCheckControl.Text = zzGetFullText(msaValue)

		End Sub
		Public Sub AddHebVarData(saValue() As String)
			ReDim msaAcadValue(msaValue.GetUpperBound(0))
			For iIndex As Integer = 0 To saValue.GetUpperBound(0)
				If Not String.IsNullOrEmpty(saValue(iIndex)) Then
					msaAcadValue(iIndex) = saValue(iIndex)
				Else
					msaAcadValue(iIndex) = msaValue(iIndex)
				End If
			Next
			If miControlIndex > 105 Then
				DMCommon.Functions.DispArray(msaValue, "msaValue", True)

				MessageBox.Show(zzGetFullText(msaValue), "04_984")
			End If


		End Sub
		Public Sub AddHebVarDataAAA(iIndex As Integer, sValue As String)
			If msaAcadValue Is Nothing Then
				ReDim msaAcadValue(msaValue.GetUpperBound(0))
			End If


			If Not String.IsNullOrEmpty(sValue) Then
				msaAcadValue(iIndex) = sValue(iIndex)
			Else
				msaAcadValue(iIndex) = msaValue(iIndex)
			End If



		End Sub

		Public Sub AddAttribTag(saValue() As String)
			msaAttribTag = saValue


		End Sub
		Private Function zzGetFullText(saValue() As String) As String '
			Dim iShift As Integer = 0
			Dim sRes As String = msText
			If miControlIndex = 15 Then
				'MessageBox.Show(CStr(msaValue.GetUpperBound(0)) & vbCrLf & sRes, "04_988a")

			End If
			For iIndex As Integer = 1 To msaValue.GetUpperBound(0)
				Dim iPrevLen As Integer = taVarPlaces(iIndex - 1).PlaceHolderLen
				If iPrevLen > 0 Then
					If msaValue(iIndex) Is Nothing Then
						taVarPlaces(iIndex - 1).Clear()
					Else
						DMCommon.Functions.Mid(sRes, taVarPlaces(iIndex - 1).StartPosition + iShift, iPrevLen, msaValue(iIndex))
						taVarPlaces(iIndex - 1).CurrentLen = msaValue(iIndex).Length
						iShift += taVarPlaces(iIndex - 1).Shift
					End If

				End If
			Next
			Return sRes
		End Function
		Private Function GetAcadText() As String()
			Dim saRes(msaValue.GetUpperBound(0)) As String
			For iIndex As Integer = 0 To msaValue.GetUpperBound(0)
				saRes(iIndex) = DMCommon.Hebrew.Invert(msaValue(iIndex))
			Next
			Return saRes
		End Function
		Public Sub Clear()
			For iIndex As Integer = 0 To miVarPlacesCount - 1
				'	taVarPlaces(iIndex).Clear()
			Next

			moCheckControl.Text = msText
		End Sub









		Private Structure VarPlace
			Public StartPosition As Integer
			Public PlaceHolderLen As Integer
			Public CurrentLen As Integer

			Public Property EndPosition As Integer
				Get
					Return StartPosition + PlaceHolderLen - 1
				End Get
				Set(iValue As Integer)
					PlaceHolderLen = iValue - StartPosition + 1
					CurrentLen = PlaceHolderLen
				End Set
			End Property
			Public Function Shift() As Integer
				Return CurrentLen - PlaceHolderLen
			End Function
			Public Sub Clear()
				CurrentLen = PlaceHolderLen
			End Sub
		End Structure
	End Class

	Private Sub cmdCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckAll.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Checked = True
		Next
	End Sub

	Private Sub cmdCheckClear_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckClear.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Checked = False
		Next
	End Sub
	Private Sub zzSetAttribTags()
		Const sNoteNo As String = "NO"
		Dim saAttribTag0() As String = {sNoteNo, "N_GHOSH"}
		moaNoteChecks(0).AddAttribTag(saAttribTag0)

		Dim saAttribTag1() As String = {sNoteNo, "N_PROJ", "DATE", "N_ELLIPSE", "N_GHOSH"}
		moaNoteChecks(1).AddAttribTag(saAttribTag1)


		Dim saAttribTag2() As String = {sNoteNo, "N_GHOSH", "N_PROJ", "DATE", "N_ELLIPSE", "N_PLAN_GHOSH"}
		moaNoteChecks(2).AddAttribTag(saAttribTag2)


		Dim saAttribTag3() As String = {sNoteNo, "BORDER", "N_GHOSH"}
		moaNoteChecks(3).AddAttribTag(saAttribTag3)


		Dim saAttribTag4() As String = {sNoteNo}
		moaNoteChecks(4).AddAttribTag(saAttribTag4)


		Dim saAttribTag5() As String = {sNoteNo}
		moaNoteChecks(5).AddAttribTag(saAttribTag5)

		Dim saAttribTag6() As String = {sNoteNo}
		moaNoteChecks(6).AddAttribTag(saAttribTag6)

		Dim saAttribTag7() As String = {sNoteNo}
		moaNoteChecks(7).AddAttribTag(saAttribTag7)

		Dim saAttribTag8() As String = {sNoteNo}
		moaNoteChecks(8).AddAttribTag(saAttribTag8)

		Dim saAttribTag9() As String = {sNoteNo}
		moaNoteChecks(9).AddAttribTag(saAttribTag9)

		Dim saAttribTag10() As String = {sNoteNo}
		moaNoteChecks(10).AddAttribTag(saAttribTag10)

		Dim saAttribTag11() As String = {sNoteNo}
		moaNoteChecks(11).AddAttribTag(saAttribTag11)

		Dim saAttribTag12() As String = {sNoteNo}
		moaNoteChecks(12).AddAttribTag(saAttribTag12)

		Dim saAttribTag13() As String = {sNoteNo}
		moaNoteChecks(13).AddAttribTag(saAttribTag13)

		Dim saAttribTag14() As String = {sNoteNo, "N_TABA"}
		moaNoteChecks(14).AddAttribTag(saAttribTag14)

		Dim saAttribTag15() As String = {sNoteNo, "N_TASHAZ"}
		moaNoteChecks(15).AddAttribTag(saAttribTag15)

		Dim saAttribTag16() As String = {sNoteNo, "N_JOB"}
		moaNoteChecks(16).AddAttribTag(saAttribTag16)

		Dim saAttribTag17() As String = {sNoteNo, "N_PROJECT"}
		moaNoteChecks(17).AddAttribTag(saAttribTag17)

	End Sub
	Private Sub cmdPreview_Click(sender As System.Object, e As System.EventArgs) Handles cmdPreview.Click

		zzFillData()


	End Sub
	Private Sub zzFillData()

		Dim saValue0() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text)}
		moaNoteChecks(0).AddVarData(saValue0)

		'   Dim saValue1() As String = {String.Empty, zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		'moaNoteChecks(1).AddVarData(saValue1)


		'  Dim saValue2() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		'moaNoteChecks(2).AddVarData(saValue2)
		' MessageBox.Show(moaNoteChecks(2).AttributeCount.ToString(), "07_163X")
		Dim saValue3() As String = {String.Empty, zzTextToNothing(zzFilterList(Me.txtNormalParcels.Text)), zzTextToNothing(Me.txtGush.Text)}
		moaNoteChecks(3).AddVarData(saValue3)

		Dim saValue4() As String = {String.Empty}
		moaNoteChecks(4).AddVarData(saValue4)
		Dim saValue5() As String = {String.Empty}
		moaNoteChecks(5).AddVarData(saValue5)
		Dim saValue6() As String = {String.Empty}
		moaNoteChecks(6).AddVarData(saValue6)
		Dim saValue7() As String = {String.Empty}
		moaNoteChecks(7).AddVarData(saValue7)
		Dim saValue8() As String = {String.Empty}
		moaNoteChecks(8).AddVarData(saValue8)
		Dim saValue9() As String = {String.Empty}
		moaNoteChecks(9).AddVarData(saValue9)
		Dim saValue10() As String = {String.Empty}
		moaNoteChecks(10).AddVarData(saValue10)
		Dim saValue11() As String = {String.Empty}
		moaNoteChecks(11).AddVarData(saValue11)
		Dim saValue12() As String = {String.Empty}
		moaNoteChecks(12).AddVarData(saValue12)
		Dim saValue13() As String = {String.Empty}
		moaNoteChecks(13).AddVarData(saValue13)

		Dim saValue14() As String = {String.Empty, zzTextToNothing(Me.txtPlanNum.Text, False)}
		moaNoteChecks(14).AddVarData(saValue14)
		saValue14(1) = zzTextToNothing(Me.txtPlanNum.Text, True)
		moaNoteChecks(14).AddHebVarData(saValue14)

		Dim saValue15() As String = {String.Empty, zzTextToNothing(Me.txtPlanPHNum.Text, False)}
		moaNoteChecks(15).AddVarData(saValue15)
		saValue15(1) = zzTextToNothing(Me.txtPlanPHNum.Text, True)
		moaNoteChecks(15).AddHebVarData(saValue15)


		Dim saValue16() As String = {String.Empty, zzTextToNothing(Me.txtTaskNo.Text)}
		moaNoteChecks(16).AddVarData(saValue16)

		Dim saValue17() As String = {String.Empty, zzTextToNothing(Me.txtProjectNo.Text)}
		moaNoteChecks(17).AddVarData(saValue17)

	End Sub
	Private Sub zzLoadNoteData()
		Dim sValue As String
		'Dim saValue1() As String = {String.Empty, zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}

		sValue = moaNoteChecks(1).Item(1)
		If sValue IsNot Nothing Then
			Me.txtBaseSerialNumber.Text = sValue
		End If

		sValue = moaNoteChecks(1).Item(2)
		If sValue IsNot Nothing Then
			''''''''''' zzParseDate(sValue, Me.dtpBaseApproveDate)
		End If
		'   MessageBox.Show(sValue & vbCrLf & moaNoteChecks(1).AttributeCount.ToString(), "07_021b")
		sValue = moaNoteChecks(1).Item(3)
		If sValue IsNot Nothing Then
			Me.txtBaseProcessName.Text = sValue
		End If

		sValue = moaNoteChecks(1).Item(4)
		If sValue IsNot Nothing Then
			Me.txtBaseGush.Text = sValue
		End If
		'   Dim saValue2() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}

		sValue = moaNoteChecks(2).Item(1)
		If sValue IsNot Nothing Then
			Me.txtGush.Text = sValue
		End If

		sValue = moaNoteChecks(2).Item(2)
		If sValue IsNot Nothing Then
			Me.txtBaseSerialNumber.Text = sValue
		End If

		sValue = moaNoteChecks(2).Item(3)
		If sValue IsNot Nothing Then
			'''''''''''''''zzParseDate(sValue, Me.dtpBaseApproveDate)
		End If

		sValue = moaNoteChecks(2).Item(4)
		If sValue IsNot Nothing Then
			Me.txtBaseProcessName.Text = sValue
		End If

		sValue = moaNoteChecks(2).Item(5)
		If sValue IsNot Nothing Then
			Me.txtBaseGush.Text = sValue
		End If

		'  Dim saValue3() As String = {String.Empty, zzTextToNothing(zzFilterList(Me.txtParcels.Text)), zzTextToNothing(Me.txtGush.Text)}

		'sValue = moaNoteChecks(3).Item(1)
		'If sValue IsNot Nothing Then
		'   Me.txtParcels.Text = sValue
		'End If

		sValue = moaNoteChecks(3).Item(2)
		If sValue IsNot Nothing Then
			Me.txtGush.Text = sValue
		End If


		sValue = moaNoteChecks(14).Item(1)
		If sValue IsNot Nothing Then
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sValue, False)
			Me.txtPlanNum.Text = oHebText.GetWinDest(False)
		End If

		sValue = moaNoteChecks(15).Item(1)
		If sValue IsNot Nothing Then
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sValue, False)
			Me.txtPlanPHNum.Text = oHebText.GetWinDest(False)
		End If



		sValue = moaNoteChecks(16).Item(1)
		If sValue IsNot Nothing Then
			Me.txtTaskNo.Text = sValue
		End If

		sValue = moaNoteChecks(17).Item(1)
		If sValue IsNot Nothing Then
			Me.txtProjectNo.Text = sValue
		End If


	End Sub
	Private Sub cmdUndo_Click(oSender As System.Object, e As System.EventArgs) Handles cmdUndo.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Clear()
		Next
	End Sub
	Private Function zzTextToNothing(sText As String, Optional bHeb As Boolean = False) As String
		If String.IsNullOrEmpty(sText) Then
			Return Nothing
		ElseIf bHeb Then
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText, True)

			Return oHebText.GetDOSDest


		Else

			Return sText
		End If
	End Function
	Private Function zzDatePickerToNothing(dtpControl As System.Windows.Forms.DateTimePicker) As String
		If dtpControl.Checked Then
			Return dtpControl.Text
		Else
			Return Nothing
		End If
	End Function



	Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
		Dim sComText As String = "SELECT * FROM Journal"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		Dim iBlock As Short
		Dim s As String = String.Empty
		If oDataReader IsNot Nothing Then


			'	MessageBox.Show("", "01_518a")


			Do While oDataReader.Read
				iBlock = oDataReader.GetInt16(0)
				s &= CStr(iBlock)

			Loop
			MessageBox.Show(s, "01_519a")
		End If
	End Sub

	Private Sub zzAfterAcad(bClose As Boolean)
		'   DMCommon.Debug.MsgBox("12_090AA")
		Me.Visible = True
	End Sub


	Private Sub cmdParcelInvert_Click_1(oSender As System.Object, e As EventArgs) Handles cmdParcelInvert.Click
		Me.txtNormalParcels.Text = DMCommon.Hebrew.InvertList(Me.txtNormalParcels.Text)
	End Sub






	Private Sub Button4_Click(oSender As System.Object, e As EventArgs)

	End Sub



	Private Sub bnsProject_CurrentChanged(oSender As System.Object, e As EventArgs) Handles bnsProject.CurrentChanged
		Dim oDynValue As System.Object = Me.bnsProject.Current
		Dim iValue As Integer
		Dim iNetType As enNetType
		Dim iPlanType As enPlanType
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		mbEventsEnabled = False
		If oDynValue IsNot Nothing Then
			Dim oRowView As DataRowView = TryCast(oDynValue, DataRowView)

			If oRowView IsNot Nothing Then
				iValue = DMCommon.Functions.CIntN(oRowView.Item("NetType"))
				If [Enum].IsDefined(GetType(enNetType), iValue) Then
					iNetType = CType(iValue, enNetType)
					Select Case iNetType
						Case enNetType.NetNew
							Me.rdbNetNew.Checked = True
						Case enNetType.Net2005
							Me.rdbNet2005.Checked = True
						Case enNetType.Net2012
							Me.rdbNet2012.Checked = True
					End Select
				End If
				iValue = DMCommon.Functions.CIntN(oRowView.Item("PlanType"))
				If [Enum].IsDefined(GetType(enPlanType), iValue) Then
					iPlanType = CType(iValue, enPlanType)
					Select Case iPlanType
						Case enPlanType.PlanType1
							Me.rdbPlanType1.Checked = True
						Case enPlanType.PlanType2
							Me.rdbPlanType2.Checked = True
						Case enPlanType.PlanType9
							Me.rdbPlanType9.Checked = True
						Case enPlanType.PlanType12
							Me.rdbPlanType12.Checked = True
					End Select
				End If
			End If
		End If
		mbEventsEnabled = bEventsEnabled
	End Sub

	Private Sub rdbNetNew_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbNetNew.CheckedChanged
		zzGetRdbNetType(Me.rdbNetNew, enNetType.NetNew)
	End Sub
	Private Sub zzGetRdbNetType(rdbNet As RadioButton, iNetType As enNetType)
		If mbEventsEnabled AndAlso rdbNet.Checked Then
			Dim oDynValue As System.Object = Me.bnsProject.Current
			If oDynValue IsNot Nothing Then
				Dim oRowView As DataRowView = TryCast(oDynValue, DataRowView)
				If oRowView IsNot Nothing Then
					oRowView.Item("NetType") = iNetType
				End If
			End If
		End If
	End Sub
	Private Sub zzGetRdbPlanType(rdbPlan As RadioButton, iPlanType As enPlanType)
		If mbEventsEnabled AndAlso rdbPlan.Checked Then
			Dim oDynValue As System.Object = Me.bnsProject.Current
			If oDynValue IsNot Nothing Then
				Dim oRowView As DataRowView = TryCast(oDynValue, DataRowView)
				If oRowView IsNot Nothing Then
					oRowView.Item("PlanType") = iPlanType
				End If
			End If
		End If
	End Sub

	Private Sub rdbNet2012_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbNet2012.CheckedChanged
		zzGetRdbNetType(Me.rdbNet2012, enNetType.Net2012)
	End Sub

	Private Sub rdbNet2005_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbNet2005.CheckedChanged
		zzGetRdbNetType(Me.rdbNet2005, enNetType.Net2005)
	End Sub

	Private Sub frmUD_General_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		mbEventsEnabled = True
	End Sub

	Private Sub rdbPlanType1_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPlanType1.CheckedChanged
		zzGetRdbPlanType(Me.rdbPlanType1, enPlanType.PlanType1)
	End Sub

	Private Sub rdbPlanType2_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPlanType2.CheckedChanged
		zzGetRdbPlanType(Me.rdbPlanType2, enPlanType.PlanType2)
	End Sub

	Private Sub rdbPlanType9_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPlanType9.CheckedChanged
		zzGetRdbPlanType(Me.rdbPlanType9, enPlanType.PlanType9)
	End Sub

	Private Sub rdbPlanType12_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPlanType12.CheckedChanged
		zzGetRdbPlanType(Me.rdbPlanType12, enPlanType.PlanType12)
	End Sub

	Private Sub BindingNavigatorAddNewItem_Click(oSender As System.Object, e As EventArgs) Handles BindingNavigatorAddNewItem.Click
		'DMCommon.Debug.MsgBox("12_190f", sender, bnsPlans.Count, moProjectPlanTable.Rows.Count)
		If False Then
			Dim oNewRow As DataRow = moProjectPlanTable.NewRow
			oNewRow.Item("ProjectCode") = miProjectCode
			oNewRow.Item("Detail") = miDetailNo
			oNewRow.Item("PlanID") = zzGetMaxPlanID() + 1

			moProjectPlanTable.Rows.Add(oNewRow)
			DMCommon.Debug.MsgBox("12_190g", moProjectPlanTable.Rows.Count)
		End If

	End Sub
	Private Function zzGetMaxPlanID() As Integer
		Dim iPlanID As Integer
		Dim iResMaxPlanID As Integer = 0
		Try
			For Each oRow As DataRow In moProjectPlanTable.Rows
				iPlanID = DirectCast(oRow.Item("PlanID"), Integer)
				If iResMaxPlanID < iPlanID Then
					iResMaxPlanID = iPlanID
				End If
			Next
		Catch ex As Exception

		End Try

		Return iResMaxPlanID
	End Function

	Private Sub moProjectPlanTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moProjectPlanTable.TableNewRow
		Dim moDataRow As DataRow = e.Row
		moDataRow.Item("ProjectCode") = miProjectCode
		moDataRow.Item("Detail") = miDetailNo
		moDataRow.Item("PlanID") = zzGetMaxPlanID() + 1
		moDataRow.Item("BlockIsRegulated") = False

		moDataRow.Item("Note01") = False
		moDataRow.Item("Note02") = False
		moDataRow.Item("Note03") = False
		moDataRow.Item("Note04") = False
		moDataRow.Item("Note15") = False
		moDataRow.Item("Note16") = False
		moDataRow.Item("Note17") = False
		moDataRow.Item("OriginalBlockNo") = 0
		moDataRow.EndEdit()
		'	DMCommon.Debug.MsgBox("12_190h", moNewRow.RowState, bnsPlans.Count, moProjectPlanTable.Rows.Count, DMCommon.Functions.CIntN(moNewRow.Item("ProjectCode"), -2), DMCommon.Functions.CIntN(moNewRow.Item("Detail"), -2))
		'oRow.EndEdit()
	End Sub





	Private Sub bnsPlans_BindingComplete(oSender As System.Object, e As BindingCompleteEventArgs) Handles bnsPlans.BindingComplete

	End Sub

	Private Sub BindingNavigatorMoveNextItem_Click(oSender As System.Object, e As EventArgs) Handles BindingNavigatorMoveNextItem.Click

	End Sub

	Private Sub bnsPlans_DataError(oSender As System.Object, e As BindingManagerDataErrorEventArgs) Handles bnsPlans.DataError
		Stop
	End Sub

	Private Sub txtProjectNo_TextChanged(oSender As System.Object, e As EventArgs) Handles txtProjectNo.TextChanged

	End Sub

	Private Sub cmbSurveyor_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbSurveyor.SelectedIndexChanged

	End Sub
End Class