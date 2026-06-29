Option Explicit On
Option Strict On
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmHanitGeneral
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


	Private Const msStampBlockName As String = "C1643"
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



	Dim miProjectCode As Integer
	Dim miDetailNo As Integer
	Private moProjectDataTable As System.Data.DataTable
	Private moProjectDataAdapter As Data.Common.DbDataAdapter


	Private moDataRow As DataRow
	Private miPlanType As Integer




	Private msGBDOSEnd As String = New String({Chr(46), Chr(129), Chr(46), Chr(130)})

	Private moaNoteChecks(17) As NoteCheck

	'	Dim c As Char = New Char()

	Dim chars() As Char = {ChrW(&H61), ChrW(&H308)}




	Private Const mdDeltaPosX As Double = 400
	Private Const mdDeltaPosY As Double = 250
	Private mcolInvisibleAttribs As ObjectIdCollection = New ObjectIdCollection()
	Private mtGenBlockRecObjID As ObjectId
	Private moaAttribDefs() As AttributeDefinition = Nothing

	Private Shared moFrameInsPoint As DMAcadExt.TPlnPoint


	Private mtGenBlockRefObjID As ObjectId
	Private mtGenInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtEllipseBlockRefObjID As ObjectId
	Private mtEllipseInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtStampBlockRefObjID As ObjectId
	Private mtStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtNotesInsPoint As Autodesk.AutoCAD.Geometry.Point3d



	' Private mbFrameExists As Boolean = False
	Private Shared moNoteInsPoint As DMAcadExt.TPlnPoint
	Private mdNoteMarginX As Double = 44.2
	Private mdNoteMarginY As Double = 18.8
	Private mdNoteRowHeight As Double = 6

	Private moFrameAcadBlock As DMAcadExt.AcadBlock

	Private moGenAcadBlock As DMAcadExt.AcadBlock

	Private moEllipseAcadBlock As DMAcadExt.AcadBlock
	Private moEllipseTTGAcadBlock As DMAcadExt.AcadBlock
	Private moPlanNumberAcadBlock As DMAcadExt.AcadBlock

	Private moStampAcadBlock As DMAcadExt.AcadBlock
	Private moDmAddressAcadBlock As DMAcadExt.AcadBlock

	Private moDirArrowAcadBlock As DMAcadExt.AcadBlock


	'  Private moScaleAcadBlock As DMAcadExt.AcadBlock
	Private msGush As String
	Private msParcels As String
	Private mbDBSource As Boolean
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
	Public Sub New(bDBSource As Boolean)

		' This call is required by the designer.
		InitializeComponent()
		zzMyInitializeComponent()
		mbDBSource = bDBSource
		If mbDBSource Then
			zzInitUD_Project()
			zzLoadProjectDataTable(False)
		End If
		' Add any initialization after the InitializeComponent() call.

		zzInit()
		zzSetAttribTags()
		zzInitAcadBlocks()
		zzSetToolTip()
		AcadReport.RepApp.InitDWGScaleFactor()
		mdCurrentFrameWidth = mdFrameWidth * AcadReport.RepApp.DrawingScaleFactor
		mdCurrentFrameHeight = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor

	End Sub
	Private Sub zzInitUD_Project()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TopoManager.TPlanGraph.TplnProject.ServerDataSource, frmUnidiv.ProjectDataBase, True, False)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
	End Sub
	Private Sub zzInitAcadBlocks()
		moGenAcadBlock = New DMAcadExt.AcadBlock(msGenBlockName, msBlockPath)
		moGenAcadBlock.Fields = New String() {msDistrictAttribTag, msSubdistrictAttribTag, msLocalityNameAttribTag, msGushAttribTag, msParcelsAttribTag, msOrdererAttribTag _
							  , msPlanTypeAttribTag, msNetAttribTag, msScaleAttribTag, msGushLegalAreaAttribTag, msGushRegStatusNameAttribTag, msGushLastParcelNameAttribTag _
							  , msLocalityCodeAttribTag}

		moEllipseAcadBlock = New DMAcadExt.AcadBlock(msEllipseBlockName, msBlockPath)
		moEllipseAcadBlock.Fields = New String() {msProcessNameAttribTag, msSerialNumAttribTag}

		moEllipseTTGAcadBlock = New DMAcadExt.AcadBlock(msEllipseTTGBlockName, msBlockPath)
		moEllipseTTGAcadBlock.Fields = New String() {msProcessNameAttribTag, msSerialNumAttribTag}

		moPlanNumberAcadBlock = New DMAcadExt.AcadBlock(msPlanNumberBlockName, msBlockPath)
		moPlanNumberAcadBlock.Fields = New String() {msPlanNumberAttribTag}



		moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockName, msBlockPath)
		moStampAcadBlock.Fields = New String() {msSurveyDate1AttribTag, msSurveyDate2AttribTag, msEndDateAttribTag, msUpdateDateAttribTag, msPlaceAttribTag, msSurveyorIDAttribTag, msSurveyorNameAttribTag}
		moDmAddressAcadBlock = New DMAcadExt.AcadBlock(msDmAddressBlockName, msBlockPath)

		moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName, msBlockPath)
		moDirArrowAcadBlock = New DMAcadExt.AcadBlock(msTazarDirArrowBlockName, msBlockPath)

		'    moScaleAcadBlock = New DMAcadExt.AcadBlock(zzGetScaleBlockName(), msBlockPath)
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
	Private Function zzGetScaleBlockName() As String
		Dim iScaleValue As Integer = Convert.ToInt32(1000.0 * AcadReport.RepApp.DrawingScaleFactor)
		iScaleValue = 625
		Return msTazarScaleBaseBlockName & Convert.ToString(iScaleValue)
	End Function
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
		zzSetMainData()

	End Sub
	Private Sub zzSetMainData()

		Me.cmbDistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("District"))
		Me.cmbSubdistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("Subdistrict"))
		Me.cmbLocality.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("Locality"))
		Me.txtOrderer.Text = DMCommon.Functions.CStrN(moDataRow.Item("Orderer"))
		DMCommon.Debug.MsgBox("13_023", DMCommon.Functions.CIntN(moDataRow.Item("District")), Me.cmbDistrict.SelectedValue, Me.cmbSubdistrict.SelectedValue)

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

	Private Sub frmUD_General_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub frmUD_General_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		'	Me.cmbDistrict.SelectedIndex = -1
		If False Then
			Me.cmbDistrict.Text = String.Empty
			'	Me.cmbSubdistrict.Focus()
			Me.cmbSubdistrict.Text = String.Empty
			Me.cmbLocality.Text = String.Empty
		End If

		zzLoadBlockRefData(True)
		zzSetScale()

	End Sub
	Private Sub zzSetInsertionPointNew()




		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim oBorderAcadBlock As DMAcadExt.AcadBlock


		moFrameAcadBlock.OpenForRead()
		moFrameAcadBlock.LoadAllReferences()

		If moFrameAcadBlock.ReferenceCount = 0 Then
			oBorderAcadBlock = New DMAcadExt.AcadBlock(msBorderBlockName)
			oBorderAcadBlock.OpenForRead()
			oBorderAcadBlock.LoadAllReferences()
			If oBorderAcadBlock.ReferenceCount = 1 Then
				oBlockRefData = oBorderAcadBlock.GetBlockRefData(0)
				moFrameInsPoint = New DMAcadExt.TPlnPoint(oBlockRefData.Position)
			ElseIf oBorderAcadBlock.ReferenceCount > 1 Then

				'Error
				MessageBox.Show("Number of Old Frames: " & CStr(oBorderAcadBlock.ReferenceCount), "04_928")
				Return
			End If
			If moFrameInsPoint IsNot Nothing Then
				moFrameAcadBlock.OpenForRight()
				Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()

				tBlockRefData.Position = moFrameInsPoint.AcGePoint3d
				DMAcadExt.AcadTransaction.CreateLayer(msNoteBlockLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False)
				tBlockRefData.Layer = msNoteBlockLayer 'msTazarMapBlockLayer  
				tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
				moFrameAcadBlock.InsertRefNew(tBlockRefData)
			End If
		ElseIf moFrameAcadBlock.ReferenceCount = 1 Then
			oBlockRefData = moFrameAcadBlock.GetBlockRefData(0)
			moFrameInsPoint = New DMAcadExt.TPlnPoint(oBlockRefData.Position)

		ElseIf moFrameAcadBlock.ReferenceCount <> 1 Then
			'Error
			MessageBox.Show("Number of New Frames: " & CStr(moFrameAcadBlock.ReferenceCount), "04_928")
			Return
		End If




		' MessageBox.Show("Insert Frame: " & moFrameInsPoint.AcGePoint.ToString(), "04_920")


	End Sub
	Private Sub zzCalcInsertPoints()
		Dim oInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint

		mtStampInsPoint = oInsPoint.AcGePoint3d

		oBlockInsPoint = oInsPoint.GetMoved(0.0, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
		'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
		mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


		oBlockInsPoint = oInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
		'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
		mtGenInsPoint = oBlockInsPoint.AcGePoint3d


		oBlockInsPoint = oInsPoint.GetMoved(mdCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
		'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
		mtNotesInsPoint = oBlockInsPoint.AcGePoint3d




		' mbFrameExists = True

		moNoteInsPoint = oInsPoint.GetMoved(mdCurrentFrameWidth - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
	End Sub
	Private Sub zzSetInsertionPoint()



		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim dCurrentFrameWidth As Double = mdFrameWidth * AcadReport.RepApp.DrawingScaleFactor
		Dim dCurrentFrameHeight As Double = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor


		moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName)
		moFrameAcadBlock.OpenForRead()
		moFrameAcadBlock.LoadAllReferences()
		If moFrameAcadBlock.ReferenceCount = 0 Then
			moFrameAcadBlock = New DMAcadExt.AcadBlock(msBorderBlockName)
			moFrameAcadBlock.OpenForRead()
			moFrameAcadBlock.LoadAllReferences()
			If moFrameAcadBlock.ReferenceCount <> 1 Then
				'Error
				MessageBox.Show("Number of Frames: " & CStr(moFrameAcadBlock.ReferenceCount) & vbCrLf & msTazarFrameBlockName, "04_928")
				Return
			End If

		ElseIf moFrameAcadBlock.ReferenceCount <> 1 Then
			'Error

			Return
		End If


		oBlockRefData = moFrameAcadBlock.GetBlockRefData(0)


		If oBlockRefData.IsNotEmpty Then




			Dim oInsPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBlockRefData.Position)
			'	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint

			mtStampInsPoint = oInsPoint.AcGePoint3d

			oBlockInsPoint = oInsPoint.GetMoved(0.0, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtGenInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtNotesInsPoint = oBlockInsPoint.AcGePoint3d




			'   mbFrameExists = True

			moNoteInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
			'  MessageBox.Show(CStr(mtStampInsPoint.ToString()) & vbCrLf & mtEllipseInsPoint.ToString() & vbCrLf & mtGenInsPoint.ToString() & vbCrLf & mtNotesInsPoint.ToString(), "04_929")



			'   HebrewTrans.vb() : Line 334


		End If







	End Sub
	Private Sub zzSetInsertionPointOld()

		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName)
		moFrameAcadBlock.OpenForRead()
		moFrameAcadBlock.LoadAllReferences()
		If moFrameAcadBlock.ReferenceCount = 0 Then
			moFrameAcadBlock = New DMAcadExt.AcadBlock(msBorderBlockName)
			moFrameAcadBlock.OpenForRead()
			moFrameAcadBlock.LoadAllReferences()
			If moFrameAcadBlock.ReferenceCount <> 1 Then
				'Error
				MessageBox.Show("Number of Frames: " & CStr(moFrameAcadBlock.ReferenceCount), "04_928")
			End If

		ElseIf moFrameAcadBlock.ReferenceCount <> 1 Then
			'Error


		End If


		oBlockRefData = moFrameAcadBlock.GetBlockRefData(0)


		If oBlockRefData.IsNotEmpty Then




			Dim oInsPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBlockRefData.Position)
			'	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint

			mtStampInsPoint = oInsPoint.AcGePoint3d

			oBlockInsPoint = oInsPoint.GetMoved(0.0, oBlockRefData.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(oBlockRefData.ScaleFactors.X, oBlockRefData.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtGenInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(oBlockRefData.ScaleFactors.X, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtNotesInsPoint = oBlockInsPoint.AcGePoint3d




			'  mbFrameExists = True

			moNoteInsPoint = oInsPoint.GetMoved(oBlockRefData.ScaleFactors.X - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
			'  MessageBox.Show(CStr(mtStampInsPoint.ToString()) & vbCrLf & mtEllipseInsPoint.ToString() & vbCrLf & mtGenInsPoint.ToString() & vbCrLf & mtNotesInsPoint.ToString(), "04_929")



			'   HebrewTrans.vb() : Line 334


		End If







	End Sub
	Private Sub zzSetInsertionPointOldOld()
		Dim tBorderBlockRecObjID As ObjectId
		Dim tBorderBlockRefObjID As ObjectId
		Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(msBorderBlockName, tBorderBlockRefObjID, tBorderBlockRecObjID, oaAttribDefs)


		If iBlockRefCount = 1 Then
			Dim oBorderBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBorderBlockRefObjID, OpenMode.ForRead)

			Dim oInsPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBorderBlockRef.Position)
			'	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint

			mtStampInsPoint = oInsPoint.AcGePoint3d

			oBlockInsPoint = oInsPoint.GetMoved(0.0, oBorderBlockRef.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(oBorderBlockRef.ScaleFactors.X, oBorderBlockRef.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtGenInsPoint = oBlockInsPoint.AcGePoint3d


			'  mbFrameExists = True

			moNoteInsPoint = oInsPoint.GetMoved(oBorderBlockRef.ScaleFactors.X - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
			MessageBox.Show(CStr(moNoteInsPoint.Coordinates) & vbCrLf & "", "04_929")
		Else
			MessageBox.Show("number of Frames: " & CStr(iBlockRefCount), "04_928")
		End If

	End Sub
	Private Sub zzSetScale()
		Dim dScale As Double = 1000 * AcadReport.RepApp.DrawingScaleFactor
		Dim sScale As String = "1:" & Convert.ToString(dScale)
		Me.cmbScales.Text = sScale
	End Sub


	Private Sub cmdSaveDB_Click(oSender As System.Object, e As System.EventArgs) Handles cmdSaveDB.Click

		zzGetPlanType()
		moDataRow.Item("PlanType") = miPlanType
		moDataRow.Item("District") = Me.cmbDistrict.SelectedValue
		moDataRow.Item("Subdistrict") = Me.cmbSubdistrict.SelectedValue
		moDataRow.Item("Locality") = Me.cmbLocality.SelectedValue
		moDataRow.Item("Orderer") = Me.txtOrderer.Text



		'  DMCommon.Debug.MsgBox("12_190", moDataRow.RowState)
		If moDataRow.RowState = DataRowState.Detached Then
			moDataRow.Item("ProjectCode") = miProjectCode
			moDataRow.Item("Detail") = miDetailNo

			moProjectDataTable.Rows.Add(moDataRow)
		End If

		moProjectDataAdapter.Update(moProjectDataTable)
		'	MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "04_806")
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
	Private Function zzGetGenData() As Dictionary(Of String, String)

		Dim oDynValue As System.Object
		Dim oHebText As DMCommon.HebrewTrans

		'	Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(colBorderBlockRefs.Item(0), OpenMode.ForWrite)
		'MessageBox.Show(CStr(oBlockRef Is Nothing) & vbCrLf & colBorderBlockRefs.Item(0).ToString(), "04_130")


		Dim oaAttribDefs() As AttributeDefinition = Nothing
		'	Dim sBlockName As String = "C1640"
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		If cmbDistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msDistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbDistrict.Text, True))
		End If
		If cmbSubdistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msSubdistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, True)) '  DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, 
		End If
		If cmbLocality.SelectedIndex <> -1 Then
			dicAttribValues.Add(msLocalityNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbLocality.Text, True))
			oDynValue = cmbLocality.SelectedValue
			dicAttribValues.Add(msLocalityCodeAttribTag, oDynValue.ToString())

		End If


		dicAttribValues.Add(msGushAttribTag, Me.txtGush.Text)

		Dim sParcelString As String = DMCommon.Functions.JoinString(DMCommon.Hebrew.InsBracketsList(Me.txtTempParcels.Text), Me.txtNormalParcels.Text)
		If Not String.IsNullOrEmpty(sParcelString) Then
			dicAttribValues.Add(msParcelsAttribTag, sParcelString)
		End If




		'''''''''''''''''	dicAttribValues.Add(msOrdererAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtOrderer.Text, True))
		oHebText = New DMCommon.HebrewTrans(Me.txtOrderer.Text, True)
		dicAttribValues.Add(msOrdererAttribTag, oHebText.GetDOSDest())



		'''''''''''''''''''''''''''''''''''''''''''dicAttribValues.Add(msProcessNameAttribTag, Me.txtProcessName.Text)


		dicAttribValues.Add(msPlanTypeAttribTag, zzGetPlanType())


		'	dicAttribValues.Add(msSerialNumAttribTag, Me.txtSerialNum.Text & " " & DMCommon.Hebrew.WordToDOS(msGBPrefix, True))
		'''''''''''''''''''''''''''''''''''''''''''''''''''dicAttribValues.Add(msSerialNumAttribTag, Me.txtSerialNum.Text & " " & msGBDOSEnd)


		'''''''''''''''''''''''''''''''''''''''''''''''''''dicAttribValues.Add(msWorkOrderNumAttribTag, Me.txtWorkOrder.Text)
		''''''''''''''''''''''''''  
		dicAttribValues.Add(msNetAttribTag, zzGetNetType())
		dicAttribValues.Add(msScaleAttribTag, Me.cmbScales.Text)


		'''''''''''''''''''' dicAttribValues.Add(msSurveyorIDAttribTag, zzGetLicenseNo())

		'''''''''''''''''''  oHebText = New DMCommon.HebrewTrans(Me.txtPlanNum.Text, True)
		''''''''''''''''''  dicAttribValues.Add(msTabaNameAttribTag, oHebText.GetDOSDest())
		'	dicAttribValues.Add(msTabaNameAttribTag, DMCommon.Hebrew.WinToAcadC(Me.txtPlanNum.Text))
		'     If dtpSurveyDate.Checked Then
		'      dicAttribValues.Add(msSurveyDateAttribTag, dtpSurveyDate.Text)
		'     End If
		'    If dtpEndDate.Checked Then
		'dicAttribValues.Add(msEndDateAttribTag, Me.dtpEndDate.Text)
		'    End If


		'     If Me.dtpUpdateDate.Checked Then
		'dicAttribValues.Add(msUpdateDateAttribTag, Me.dtpUpdateDate.Text)
		'    End If

		dicAttribValues.Add(msGushLegalAreaAttribTag, Me.txtGushLegalArea.Text)

		dicAttribValues.Add(msGushRegStatusNameAttribTag, DMCommon.Hebrew.WordToDOS(zzGetGushStatus(), True))

		dicAttribValues.Add(msGushLastParcelNameAttribTag, Me.txtLastParcelName.Text)



		''''''''''''''''''''''  dicAttribValues.Add(msGeneralCommentNameAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtGeneralComment.Text, True))


		Return dicAttribValues


	End Function

	Private Function zzGetEllipseData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		dicAttribValues.Add(msProcessNameAttribTag, Me.txtProcessName.Text)
		dicAttribValues.Add(msSerialNumAttribTag, Me.txtSerialNum.Text & " " & msGBDOSEnd)
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
		Dim saSerialNumber() As String = Split(Me.txtSerialNum.Text, "/")
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
			' "מוסדר"

			Return TopoManager.TPlanGraph.TplnBlock.StatusNameYes
		Else
			'"לא מוסדר"
			Return TopoManager.TPlanGraph.TplnBlock.StatusNameNo
		End If
	End Function
	Private Sub zzLoadAttribData()
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim sLocalityCode As String
		Dim sPlace As String
		Dim sSurveyorName As String
		'    Dim oHebText As DMCommon.HebrewTrans


		moGenAcadBlock.OpenForRead()
		moGenAcadBlock.LoadAllReferences()
		' DMCommon.Debug.MsgBox("02_230", moGenAcadBlock.ReferenceCount)

		If moGenAcadBlock.ReferenceCount = 1 Then

			oBlockRefData = moGenAcadBlock.GetBlockRefData(0)
			Dim sBlockParcelString As String = oBlockRefData.GetAttribValue(msParcelsAttribTag)
			Me.cmbDistrict.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msDistrictAttribTag))
			Me.cmbSubdistrict.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msSubdistrictAttribTag))
			Me.cmbLocality.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msLocalityNameAttribTag))
			Me.txtGush.Text = oBlockRefData.GetAttribValue(msGushAttribTag)
			'    System.Windows.Forms.MessageBox.Show(mbDBSource.ToString & vbCrLf & DMCommon.Functions.CStrN(Me.txtParcels.Text, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(sBlockParcelString, "Nothing"), "04_215")
			If mbDBSource AndAlso Not String.IsNullOrEmpty(Me.txtNormalParcels.Text) Then
				If Me.txtNormalParcels.Text <> sBlockParcelString Then
					'  Me.erpParcels.SetError(Me.txtNormalParcels, sBlockParcelString)
				End If
			Else
				Me.txtNormalParcels.Text = sBlockParcelString
			End If


			'	Me.txtOrderer.Text = DMCommon.Hebrew.ToUnicode(oAttribRef.TextString) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, True) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, False)
			'
			'	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
			'''''''''''''''''''''''''''''''''''''   oHebText = New DMCommon.HebrewTrans(oAttribRef.TextString, False)
			Me.txtOrderer.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msOrdererAttribTag))

			zzParsePlanType(oBlockRefData.GetAttribValue(msPlanTypeAttribTag))
			zzParseNetType(oBlockRefData.GetAttribValue(msNetAttribTag))
			Me.cmbScales.Text = oBlockRefData.GetAttribValue(msScaleAttribTag)
			Me.txtGushLegalArea.Text = oBlockRefData.GetAttribValue(msGushLegalAreaAttribTag)
			zzParseGushStatus(oBlockRefData.GetAttribValue(msGushRegStatusNameAttribTag))
			Me.txtLastParcelName.Text = oBlockRefData.GetAttribValue(msGushLastParcelNameAttribTag)
			sLocalityCode = oBlockRefData.GetAttribValue(msLocalityCodeAttribTag)




			'''''''''''''''''''''''''''''''''''??????????????/////   Me.txtWorkOrder.Text = oBlockRefData.GetAttribValue(msWorkOrderNumAttribTag)




			'	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
			'    oHebText = New DMCommon.HebrewTrans(oBlockRefData.GetAttribValue(msTabaNameAttribTag), False)
			'	Me.txtPlanNum.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
			'   Me.txtPlanNum.Text = oHebText.GetWinDest(False)


			'	dtpSurveyDate.Text = oAttribRef.TextString


			'  Me.txtGeneralComment.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msGeneralCommentNameAttribTag))



		Else

		End If


		moEllipseAcadBlock.OpenForRead()
		moEllipseAcadBlock.LoadAllReferences()
		moEllipseTTGAcadBlock.OpenForRead()
		moEllipseTTGAcadBlock.LoadAllReferences()


		moPlanNumberAcadBlock.OpenForRead()
		moPlanNumberAcadBlock.LoadAllReferences()


		'  MessageBox.Show(CStr(Me.rdbPlanType2.Checked) & vbCrLf & CStr(Me.rdbPlanType12.Checked) & vbCrLf & CStr(mbDBSource) & vbCrLf & CStr(moEllipseAcadBlock.ReferenceCount) & ":" & CStr(moEllipseTTGAcadBlock.ReferenceCount), "05_470")
		If moEllipseAcadBlock.ReferenceCount = 1 AndAlso moEllipseTTGAcadBlock.ReferenceCount = 0 Then
			oBlockRefData = moEllipseAcadBlock.GetBlockRefData(0)
			Me.txtProcessName.Text = oBlockRefData.GetAttribValue(msProcessNameAttribTag)
			Me.txtSerialNum.Text = oBlockRefData.GetAttribValue(msSerialNumAttribTag)
			zzParseSerialNum(oBlockRefData.GetAttribValue(msSerialNumAttribTag))
			If mbDBSource Then
				If Not Me.rdbPlanType2.Checked Then
					' MessageBox.Show("", "05_380")
					erpPlanType.SetError(grbPlanType, "תצ""ר")
				End If
			Else
				Me.rdbPlanType2.Checked = True
			End If

		ElseIf moEllipseAcadBlock.ReferenceCount = 0 AndAlso moEllipseTTGAcadBlock.ReferenceCount = 1 Then
			oBlockRefData = moEllipseTTGAcadBlock.GetBlockRefData(0)
			Me.txtProcessName.Text = oBlockRefData.GetAttribValue(msProcessNameAttribTag)
			Me.txtSerialNum.Text = oBlockRefData.GetAttribValue(msSerialNumAttribTag)
			zzParseSerialNum(oBlockRefData.GetAttribValue(msSerialNumAttribTag))
			If mbDBSource Then
				If Not Me.rdbPlanType12.Checked Then
					'  MessageBox.Show("", "05_381")
					erpPlanType.SetError(grbPlanType, "תת""ג")
				End If
			Else
				Me.rdbPlanType12.Checked = True
			End If


		End If



		moStampAcadBlock.OpenForRead()
		moStampAcadBlock.LoadAllReferences()
		If moStampAcadBlock.ReferenceCount = 1 Then
			oBlockRefData = moStampAcadBlock.GetBlockRefData(0)
			zzParseDate(oBlockRefData.GetAttribValue(msSurveyDate1AttribTag), Me.dtpSurveyDate1)
			zzParseDate(oBlockRefData.GetAttribValue(msSurveyDate2AttribTag), Me.dtpSurveyDate2)
			If String.IsNullOrEmpty(oBlockRefData.GetAttribValue(msSurveyDate1AttribTag)) Then
				MessageBox.Show(msSurveyDate1AttribTag & vbCrLf & "", "NULL! ")
			End If
			zzParseDate(oBlockRefData.GetAttribValue(msEndDateAttribTag), Me.dtpEndDate)
			If String.IsNullOrEmpty(oBlockRefData.GetAttribValue(msEndDateAttribTag)) Then
				MessageBox.Show(msEndDateAttribTag & vbCrLf & "", "NULL! ")
			End If
			zzParseDate(oBlockRefData.GetAttribValue(msUpdateDateAttribTag), Me.dtpSurveyDate)
			If String.IsNullOrEmpty(oBlockRefData.GetAttribValue(msUpdateDateAttribTag)) Then
				'  MessageBox.Show(msUpdateDateAttribTag & vbCrLf & "", "NULL! ")
			End If
			sPlace = oBlockRefData.GetAttribValue(msPlaceAttribTag)

			Me.cmbSurveyor.SelectedValue = oBlockRefData.GetAttribValue(msSurveyorIDAttribTag)
			sSurveyorName = oBlockRefData.GetAttribValue(msSurveyorNameAttribTag)


		End If



	End Sub
	Private Sub zzOKGeneral()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		Do


			If moGenAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGenAcadBlock.UpdateAttribData(0, zzGetGenData())
				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGenAcadBlock.LoadAllReferences()
				End If
			ElseIf moGenAcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				moGenAcadBlock.OpenForRight()
				If moGenAcadBlock.DefinitionExists Then
					Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = oBlockInsPoint.AcGePoint3d
					tBlockRefData.AtribValuesDic = zzGetGenData()
					tBlockRefData.Layer = msGenBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)

					moGenAcadBlock.InsertRefNew(tBlockRefData)
				End If
				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub
	Private Sub zzOKEllipse()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		Dim sPlanType As String = zzGetPlanType()
		Dim oEllipseAcadBlock As DMAcadExt.AcadBlock
		Dim oOtherEllipseAcadBlock As DMAcadExt.AcadBlock

		Select Case sPlanType
			Case "1"
				Return
			Case "2"
				oEllipseAcadBlock = moEllipseAcadBlock
				oOtherEllipseAcadBlock = moEllipseTTGAcadBlock
			Case "9"
				oEllipseAcadBlock = moEllipseTTGAcadBlock
				oOtherEllipseAcadBlock = moEllipseAcadBlock
			Case "12"
				oEllipseAcadBlock = moEllipseTTGAcadBlock
				oOtherEllipseAcadBlock = moEllipseAcadBlock
			Case Else
				Return
		End Select

		oEllipseAcadBlock.LoadAllReferences()
		oOtherEllipseAcadBlock.LoadAllReferences()


		If oEllipseAcadBlock.ReferenceCount = 1 Then
			oEllipseAcadBlock.OpenForRight()
			'update
			iRes = oEllipseAcadBlock.UpdateAttribData(0, zzGetEllipseData())
			If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
				oEllipseAcadBlock.LoadAllReferences()
			End If



		ElseIf oEllipseAcadBlock.ReferenceCount = 0 Then
			'insert
			If moFrameInsPoint IsNot Nothing Then
				Dim tEllipseInsPoint As Autodesk.AutoCAD.Geometry.Point3d
				Dim oBlockInsPoint As DMAcadExt.TPlnPoint

				oEllipseAcadBlock.OpenForRight()
				If oEllipseAcadBlock.DefinitionExists Then
					oBlockInsPoint = moFrameInsPoint.GetMoved(0.0, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
					tEllipseInsPoint = oBlockInsPoint.AcGePoint3d


					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = tEllipseInsPoint
					tBlockRefData.AtribValuesDic = zzGetEllipseData()
					tBlockRefData.Layer = msEllipseBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					'  tBlockRefData.AttribValues = {Me.UD_Name, mtParcelArea.LegalArea.ToString(), mtParcelArea.CalcArea.ToString(), Me.BlockFull, Me.Cross, Me.PreviousParcelsStr, Me.Stage.ToString()}



					oEllipseAcadBlock.InsertRefNew(tBlockRefData)

					iRes = DMAcadExt.AcadBlock.enResults.Success

				End If
			End If


		End If
		If oOtherEllipseAcadBlock.ReferenceCount = 1 Then
			oOtherEllipseAcadBlock.DeleteRef(0)
		End If
	End Sub
	Private Sub zzOKStamp()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		Dim tStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d


		Do
			If moStampAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moStampAcadBlock.UpdateAttribData(0, zzGetStampData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moStampAcadBlock.LoadAllReferences()
				End If



			ElseIf moStampAcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
				'insert
				tStampInsPoint = moFrameInsPoint.AcGePoint3d

				moStampAcadBlock.OpenForRight()
				If moStampAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = tStampInsPoint
					tBlockRefData.AtribValuesDic = zzGetStampData()
					tBlockRefData.Layer = msStampBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moStampAcadBlock.InsertRefNew(tBlockRefData)
				End If


				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Sub zzOKPlanNumber()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'  Dim tPlanNumberInsPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint
		If moFrameInsPoint IsNot Nothing Then
			oBlockInsPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)
			Do
				If moPlanNumberAcadBlock.ReferenceCount = 1 Then
					'update
					iRes = moPlanNumberAcadBlock.UpdateAttribData(0, zzGetPlanNumberData())

					If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
						moPlanNumberAcadBlock.LoadAllReferences()
					End If



				ElseIf moStampAcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
					'insert
					oBlockInsPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)

					moPlanNumberAcadBlock.OpenForRight()
					If moPlanNumberAcadBlock.DefinitionExists Then
						Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
						tBlockRefData.Position = oBlockInsPoint.AcGePoint3d
						tBlockRefData.AtribValuesDic = zzGetStampData()
						tBlockRefData.Layer = msStampBlockLayer
						tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
						moPlanNumberAcadBlock.InsertRefNew(tBlockRefData)
					End If


					iRes = DMAcadExt.AcadBlock.enResults.Success
				Else
					' Error
					iRes = DMAcadExt.AcadBlock.enResults.Success
				End If
			Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
		End If

	End Sub
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

	Private Sub zzOKFrame()

		moDirArrowAcadBlock.LoadAllReferences()

		If moDirArrowAcadBlock.ReferenceCount = 0 Then

			moDirArrowAcadBlock.OpenForRight()
			If moDirArrowAcadBlock.DefinitionExists Then
				Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
				tBlockRefData.Position = mtGenInsPoint

				tBlockRefData.Layer = msTazarMapBlockLayer
				tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
				moDirArrowAcadBlock.InsertRefNew(tBlockRefData)
			End If
		End If

	End Sub

	Private Sub zzOKDmAddress()

		moDmAddressAcadBlock.LoadAllReferences()

		If moDmAddressAcadBlock.ReferenceCount = 0 Then
			'   MessageBox.Show(CStr(moFrameInsPoint IsNot Nothing), "04_332")
			If moFrameInsPoint IsNot Nothing Then
				moDmAddressAcadBlock.OpenForRight()
				If moDmAddressAcadBlock.DefinitionExists Then

					'   Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY

					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = moFrameInsPoint.AcGePoint3d

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moDmAddressAcadBlock.InsertRefNew(tBlockRefData)

				End If
			End If
		End If

	End Sub
	Private Sub zzOKDirArrow()

		moDirArrowAcadBlock.LoadAllReferences()

		If moDirArrowAcadBlock.ReferenceCount = 0 Then
			If moFrameInsPoint IsNot Nothing Then
				moDirArrowAcadBlock.OpenForRight()
				If moDirArrowAcadBlock.DefinitionExists Then

					Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = oBlockInsPoint.AcGePoint3d

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moDirArrowAcadBlock.InsertRefNew(tBlockRefData)
				End If
			End If
		End If

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
	Private Sub zzOKCorners()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzChangeCornerLayers)

		DMAcadExt.AcadTransaction.ProcAll(dlEntityProc, True)

	End Sub
	Private Sub zzChangeCornerLayers(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		Dim sRXClassName As String = oEntity.GetRXClass().Name
		Dim sEntityLayer As String = oEntity.Layer



		If sEntityLayer = msCornerLineLayer AndAlso sRXClassName = DMAcadExt.AcadConst.AcadLineName OrElse sEntityLayer = msCornerTextLayer AndAlso sRXClassName = DMAcadExt.AcadConst.AcadTextName Then
			oEntity.Layer = msTazarMapBlockLayer
		End If

	End Sub

	Private Sub zzParseSerialNum(sSerialNum As String)
		Dim oHebText As DMCommon.HebrewTrans

		sSerialNum = sSerialNum.Trim()

		'  System.Windows.Forms.MessageBox.Show(sSerialNum & vbCrLf & msGBDOSEnd & vbCrLf & "Index=" & CStr(234) & vbCrLf & sSerialNum.EndsWith(msGBDOSEnd).ToString(), "Serial!!")
		If sSerialNum.EndsWith(msGBDOSEnd) Then
			Dim sOut As String = sSerialNum.Substring(0, sSerialNum.Length - 4)
			oHebText = New DMCommon.HebrewTrans(sOut, False)
			Me.txtSerialNum.Text = oHebText.GetWinDest(False)
		Else
			oHebText = New DMCommon.HebrewTrans(sSerialNum, False)
			Me.txtSerialNum.Text = oHebText.GetWinDest(False)
		End If


	End Sub
	Private Sub zzCheckLocality(sLocalityCode As String, sLocalityName As String)
		If sLocalityCode IsNot Nothing AndAlso sLocalityName IsNot Nothing Then
			If cmbLocality.SelectedValue.ToString() <> sLocalityCode Then
				MessageBox.Show(cmbLocality.SelectedValue.ToString() & vbCrLf & sLocalityCode, "Err: " & msLocalityCodeAttribTag)
			End If
		End If
	End Sub
	Private Sub zzLoadBlockRefData(bLoadData As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)


		'	MessageBox.Show(CStr(iGenBlockRefCount) & vbCrLf & msDataBlockName, "04_925")
		'    MessageBox.Show(CStr("before zzLoadBlockRefData"), "04_925")
		zzLoadAttribData()

		If moGenAcadBlock.ReferenceCount = 1 Then
			Me.chkAllAttribVisible.Enabled = True
		Else
			Me.chkAllAttribVisible.Enabled = False
		End If

		mcolInvisibleAttribs = New ObjectIdCollection()
		zzLoadNotesRefData()
		zzLoadNoteData()
		zzFillData()
		'    MessageBox.Show(CStr("after zzLoadBlockRefData"), "04_926")
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()



	End Sub
	Private Sub zzLoadNotesRefData()
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)

			moaNoteChecks(iIndex).LoadBlockRef()
		Next
	End Sub

	Private Sub chkAllAttribVisible_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkAllAttribVisible.CheckedChanged
		'Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		'	Dim oAttribRef As AttributeReference
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		If Me.chkAllAttribVisible.Checked Then
			moGenAcadBlock.SetAllAttribVisibility(0, False)
		Else
			moGenAcadBlock.OpenForRight(True)
			moGenAcadBlock.SetDefaultAttribVisibility(0)
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub

	Private Sub chkAllAttribVisible_CheckedChangedOld(sender As System.Object, e As System.EventArgs)
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oAttribRef As AttributeReference
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		If Me.chkAllAttribVisible.Checked Then
			For Each tAttribObjID As ObjectId In mcolInvisibleAttribs
				'	DMAcadExt.AcadDocument.WriteMessage("#191a " & tAttribObjID.ToString())
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForWrite)
				oAttribRef = DirectCast(oDBObject, AttributeReference)
				If oAttribRef.Invisible Then
					oAttribRef.Invisible = False
				End If
			Next
		Else
			For Each tAttribObjID As ObjectId In mcolInvisibleAttribs
				DMAcadExt.AcadDocument.WriteMessage("#192a " & tAttribObjID.ToString())
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForWrite)
				oAttribRef = DirectCast(oDBObject, AttributeReference)
				If Not oAttribRef.Invisible Then
					oAttribRef.Invisible = True
				End If
			Next
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub



	Private Sub cmdLoadBlockData_Click(sender As System.Object, e As System.EventArgs) Handles cmdLoadBlockData.Click
		zzLoadBlockRefData(True)
	End Sub

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		If False Then
			MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "04_807")
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadTransaction.CreateLayer("1602")

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'924  464
		'	Me.Width = Me.Width + Me.Width
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
		Private Shared mtTextStyleTableRecordID As ObjectId
		Private Shared mdLeft As Double
		Private Shared mdTextHeight As Double
		Private moCheckControl As CheckBox
		Private miControlIndex As Integer
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

		Private mtGenBlockRecObjID As ObjectId
		Private mtGenBlockRefObjID As ObjectId
		Public Shared Sub SetParam(dLeft As Double, dTextHeight As Double)
			'	zzInitTextSile(msFontStyle)
			mdLeft = dLeft
			mdTextHeight = dTextHeight
		End Sub

		Public Shared Sub InitPrint()
			'	zzInitTextSile(msFontStyle)
			mdLeft = moNoteInsPoint.X
			mdTextHeight = 2.0
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
		Public Sub InsertNoteBlockRef(ByRef mdTop As Double)
			'	Dim iGenBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(msDataBlockName, mtGenBlockRefObjID, mtGenBlockRecObjID)
			'	MessageBox.Show(CStr(iGenBlockRefCount) & vbCrLf & msDataBlockName, "04_925")

			Dim tBlockRefObjID, tBlockRecObjID As ObjectId
			'	MessageBox.Show(CStr(msBlockPath) & vbCrLf & Me.BlockName, "04_924")
			Dim oaAttribDefs() As AttributeDefinition = Nothing
			Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(Me.BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)
			Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
			'Dim oaAttribDefs() As AttributeDefinition = Nothing
			Dim mtInsPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim bSuccess As Boolean
			Dim dTopBefore As Double = mdTop
			mdTop += mdNoteRowHeight * (RowsNumber - 1) * AcadReport.RepApp.DrawingScaleFactor
			If msaAcadValue Is Nothing Then
				For iIndex As Integer = 0 To Math.Min(msaAttribTag.GetUpperBound(0), msaValue.GetUpperBound(0))
					dicAttribValues.Add(msaAttribTag(iIndex), msaValue(iIndex))
				Next

			Else
				For iIndex As Integer = 0 To Math.Min(msaAttribTag.GetUpperBound(0), msaValue.GetUpperBound(0))
					dicAttribValues.Add(msaAttribTag(iIndex), msaAcadValue(iIndex))
				Next
			End If

			mtInsPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0.0)

			If tBlockRefObjID.IsNull Then

				If tBlockRecObjID.IsNull Then
					'	
					tBlockRecObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, Me.BlockName, oaAttribDefs)
				End If

				If Not tBlockRecObjID.IsNull Then
					Dim tNewBlockRefObjID As ObjectId = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockRecObjID, mtInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor * 0.4)
					If Not tNewBlockRefObjID.IsNull Then
						bSuccess = True
					End If

				End If
			Else
				DMAcadExt.AcadTransaction.EraseDBObject(tBlockRefObjID)
				Dim tNewBlockRefObjID As ObjectId = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockRecObjID, mtInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor * 0.4)
				If Not tNewBlockRefObjID.IsNull Then
					bSuccess = True
				End If

			End If

			If bSuccess = True Then
				mdTop += mdNoteRowHeight * AcadReport.RepApp.DrawingScaleFactor
			Else
				mdTop = dTopBefore
			End If

		End Sub
		Public Sub LoadBlockRef()
			Dim tBlockRefData As DMAcadExt.BlockRefData
			Dim saValues() As String
			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(Me.BlockName)
			oAcadBlock.Open()
			oAcadBlock.LoadAllReferences()

			If oAcadBlock.ReferenceCount = 1 Then
				tBlockRefData = oAcadBlock.GetBlockRefData(0, False)
				'  MessageBox.Show(CStr(tBlockRefData.AttribValues.GetUpperBound(0)) & vbCrLf & Me.BlockName, "04_172")
				saValues = tBlockRefData.AttribValues
				'    DMCommon.Functions.DispArray(saValues, "AttribValue", True)
				mdicAtribValues = tBlockRefData.AtribValuesDic
				'  MessageBox.Show(CStr(mdicAtribValues.Count) & vbCrLf & CStr(tBlockRefData.AttribValues.GetUpperBound(0)), "04_173")
				moCheckControl.Checked = True
			End If
		End Sub
		Public Sub EraseNoteBlockRef()
			Dim tBlockRefObjID, tBlockRecObjID As ObjectId
			'	MessageBox.Show(CStr(msBlockPath) & vbCrLf & Me.BlockName, "04_924")
			Dim oaAttribDefs() As AttributeDefinition = Nothing
			Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(Me.BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)

			If Not tBlockRecObjID.IsNull Then
				DMAcadExt.AcadTransaction.EraseDBObject(tBlockRefObjID)
			End If

		End Sub
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
		Public Sub DrawMText(mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.MText = New Autodesk.AutoCAD.DatabaseServices.MText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Location = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			oText.Direction = New Autodesk.AutoCAD.Geometry.Vector3d(-1.0, 0.0, 0)
			oText.FlowDirection = FlowDirection.TopToBottom
			'oText.AlignChange = ""
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.Contents = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			'	oText.HorizontalMode = TextHorizontalMode.TextRight
			'	oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor * 0.1
			Try
				'	oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)



		End Sub

		Public Sub DrawTextA(mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.TextString = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			oText.HorizontalMode = TextHorizontalMode.TextRight
			oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor
			Try
				oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)



		End Sub
		Public Sub InsertBlock(ByRef mdTop As Double)

			Dim saValue() As String = GetAcadText()  ' msaValue	'

			Dim sText As String = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)


			Dim sDel As String = vbCrLf
			Dim caCharDel() As Char = sDel.ToCharArray()
			Dim saText() As String = sText.Split(caCharDel, StringSplitOptions.RemoveEmptyEntries)    'Split(sText, vbCrLf)
			MessageBox.Show(CStr(saText.GetUpperBound(0)), "04_455")
			For iIndex As Integer = 0 To saText.GetUpperBound(0)
				zzDrawLine(mdTop, saText(iIndex))
			Next

		End Sub
		Public Sub DrawTextByLine(ByRef mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			Dim sText As String = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)


			Dim sDel As String = vbCrLf
			Dim caCharDel() As Char = sDel.ToCharArray()
			Dim saText() As String = sText.Split(caCharDel, StringSplitOptions.RemoveEmptyEntries)    'Split(sText, vbCrLf)
			MessageBox.Show(CStr(saText.GetUpperBound(0)), "04_455")
			For iIndex As Integer = 0 To saText.GetUpperBound(0)
				zzDrawLine(mdTop, saText(iIndex))
			Next

		End Sub
		Private Sub zzDrawLine(ByRef mdTop As Double, sText As String)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.TextString = sText
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			oText.HorizontalMode = TextHorizontalMode.TextRight
			oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor
			Try
				oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)
			mdTop -= mdNoteRowHeight * AcadReport.RepApp.DrawingScaleFactor


		End Sub

		Private Function zzGetLeftTop(ByVal dShiftY As Double) As Autodesk.AutoCAD.Geometry.Point3d
			'	Dim dX As Double = moBasePoint.X
			'	Dim dY As Double = mdCurrentY - dShiftY
			'	Return New Autodesk.AutoCAD.Geometry.Point3d(dX, dY, 0.0)
		End Function

		Private Shared Sub zzInitTextSile(ByVal sAcadFont As String)
			mtTextStyleTableRecordID = DMAcadExt.AcadTransaction.GetTextStyle(sAcadFont)   '
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

		Dim saValue1() As String = {String.Empty, zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		moaNoteChecks(1).AddVarData(saValue1)


		Dim saValue2() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		moaNoteChecks(2).AddVarData(saValue2)
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
			zzParseDate(sValue, Me.dtpBaseApproveDate)
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
			zzParseDate(sValue, Me.dtpBaseApproveDate)
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
	Private Sub zzInsertNote(iIndex As Integer)
		Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim tBlockRefObjID, tBlockRecObjID As ObjectId
		Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(moaNoteChecks(iIndex).BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		If tBlockRefObjID.IsNull Then
			If tBlockRecObjID.IsNull Then
				tBlockRecObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, moaNoteChecks(iIndex).BlockName, oaAttribDefs)
			End If
			mtGenBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtGenBlockRecObjID, mtGenInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor)
		Else
			Dim oGenBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(mtGenBlockRefObjID, OpenMode.ForWrite)
			If oGenBlockRef IsNot Nothing Then
				DMAcadExt.AcadTransaction.UpdateAttribText(oGenBlockRef, dicAttribValues)
			End If
		End If
	End Sub

	Private Sub cmdInsertNotes_Click(sender As System.Object, e As System.EventArgs) Handles cmdInsertNotes.Click
		zzFillData()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim tNotesInsPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bCurrentLayerOK As Boolean

		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msTazarMapBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		zzSetInsertionPointNew()
		'  oBlockInsPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
		'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
		If bCurrentLayerOK AndAlso moFrameInsPoint IsNot Nothing Then







			'    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msNotesLayer, DMAcadExt.DMApp.AppID, True, True)

			tNotesInsPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor).AcGePoint3d


			' NoteCheck.InitPrint()
			NoteCheck.SetParam(tNotesInsPoint.X, 2.0)
			Dim dTop As Double = tNotesInsPoint.Y

			Dim iNo As Integer = 0
			Dim iRowsCount As Integer = 0
			For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
				If moaNoteChecks(iIndex).Checked Then
					iNo += 1
					moaNoteChecks(iIndex).NoteNo = iNo

				End If
			Next
			For iIndex As Integer = moaNoteChecks.GetUpperBound(0) To 0 Step -1
				If moaNoteChecks(iIndex).Checked Then
					moaNoteChecks(iIndex).InsertNoteBlockRef(dTop)
				Else
					moaNoteChecks(iIndex).EraseNoteBlockRef()
				End If
			Next
			If moaNoteChecks.GetUpperBound(0) > 0 Then
				zzSetCaption(tNotesInsPoint.X, dTop)
			End If
		End If
		'	DMAcadExt.AcadDocument.WriteMessage("09:" & moNoteInsPoint.Coordinates2d)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub
	Private Sub zzSetCaption(dLeft As Double, ByRef mdTop As Double)
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("UD_NoteCaption", msBlockPath)

		oAcadBlock.OpenForRight()

		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()


		oAcadBlock.LoadAllReferences()
		'   DMAcadExt.AcadDocument.WriteMessage("$$$Ref_COUNT " & oAcadBlock.ReferenceCount.ToString() & ", " & "")
		If oAcadBlock.ReferenceCount > 0 Then
			tBlockRefData = oAcadBlock.GetBlockRefData(0)

		ElseIf oAcadBlock.ReferenceCount = 0 Then
			tBlockRefData = New DMAcadExt.BlockRefData()

		End If

		tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(dLeft, mdTop, 0.0)
		tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor * 0.4)

		If oAcadBlock.ReferenceCount > 0 Then
			oAcadBlock.UpdateBlockRef(tBlockRefData.AcObjID, tBlockRefData)
		Else
			oAcadBlock.InsertRef(tBlockRefData)
		End If

	End Sub
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

	Private Sub cmdDrawFrame_Click(oSender As System.Object, e As EventArgs) Handles cmdDrawFrame.Click
		Dim oAfterCom As DMAcadExt.AcadDocument.AfterCommandProc = New DMAcadExt.AcadDocument.AfterCommandProc(AddressOf zzAfterAcad)
		DMAcadExt.AcadDocument.SetOneTimeActiveDoc("UNDO", 2, oAfterCom)
		Me.Visible = False
		DMAcadExt.AcadDocument.SendExec(msFrameComLine, True)
		'  MessageBox.Show(msFrameComLine, "01_521m")
	End Sub


	Private Sub cmdSaveToDWG_Click(oSender As System.Object, e As EventArgs) Handles cmdSaveToDWG.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim bCurrentLayerOK As Boolean

		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msTazarMapBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msGenBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msEllipseBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msStampBlockLayer, DMAcadExt.DMApp.AppID, True, True)

		zzSetInsertionPointNew()

		' MessageBox.Show(CStr(moFrameInsPoint.Coordinates) & vbCrLf & msBorderBlockName, "04_944")


		zzOKGeneral()

		zzOKEllipse()
		zzOKPlanNumber()
		zzOKStamp()
		zzOKDmAddress()

		'    zzOKDirArrow()
		zzOKScale()
		zzOKCorners()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub

	Private Sub txtOrderer_TextChanged(oSender As System.Object, e As EventArgs) Handles txtOrderer.TextChanged

	End Sub

	Private Sub txtBaseSerialNumber_TextChanged(oSender As System.Object, e As EventArgs) Handles txtBaseSerialNumber.TextChanged

	End Sub

	Private Sub rdbPlanType1_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPlanType1.CheckedChanged

	End Sub
End Class