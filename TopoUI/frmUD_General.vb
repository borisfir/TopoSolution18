
Option Explicit On
Option Strict On
Imports System.ComponentModel
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmUD_General
	Private Enum enNetType
		Undefined
		NetNew
		Net2005
		Net2012
	End Enum

	Public Enum enPlanType
		Undefined = 0
		PlanType1 = 1
		PlanType2 = 2
		PlanType9 = 9
		PlanType12 = 12

	End Enum
	Private Enum enMapStatus
		Undefined = 0
		MapStatusEarly = 1
		MapStatusTemp = 2
		MapStatusFinal = 3
	End Enum

	'IG05/12  IGD05/12
	'P:\2006\060169\060169_מסמכי מחלקת תצר\hanit_blocks_20141118\tzr_hanit_sample2.dwg
	Private Const msBorderBlockName As String = "BORDER"
	Private Const msOldBlockName As String = "SU77_2"
	Private Const msDirArrowBlockName As String = "SUNORTHR"
	Private Const msCornerLineLayer As String = "DRWP002"
	Private Const msCornerTextLayer As String = "DRWT004"




	Private Const msTazarFrameBlockName As String = "TAZAR_RISH-MAX"
	Private Const msTazarDirArrowBlockName As String = "TAZAR_ZAFON"
	Private Const msType1DirArrowBlockName As String = "ZAFON-G"

	Private Const msTazarScaleBaseBlockName As String = "TAZAR_SCL"
	Private Const msDMScaleBaseBlockName As String = "SU_"

	Private Const msPageNoBlockName As String = "UD_PageNo"
	Private Const msCaptionBlockName As String = "NUM_PR"



	Private Const msFrameBlockNameType1_Hor As String = "GUSHHORZ"
	Private Const msFrameBlockNameType1_Ver As String = "GUSHVERT"

	Private Const msCoordinateBlockName As String = "dmCoordinate"

	Private Const msLegendBlockName As String = "LEGAND"

	Private Const msUpdateTableBlockName As String = "UPDATE_TABLE"



	Private Const msTazarMapBlockLayer As String = "C1680"
	Private Const msNotesLayer As String = "C1682"


	Private Const msGenBlockName As String = "C1640"
	Private Const msGenBlockLayer As String = "C1640"
	Private Const msNoteBlockLayer As String = "C1641"


	Private Const msEllipseBlockName As String = "C1642"
	Private Const msEllipseBlockLayer As String = "C1642"


	Private Const msEllipseTTGBlockName As String = "C1644"
	Private Const msEllipseTTGBlockLayer As String = "C1644"

	Private Const msGushPropBlockName As String = "C1645"
	Private Const msGushPropBlockLayer As String = "C1645"

	Private Const msGenType1BlockName As String = "C1646"
	Private Const msGenType1BlockLayer As String = "C1646"

	Private Const msScaleTextBlockName As String = "C1647"
	Private Const msScaleTextBlockLayer As String = "C1647"

	Private Const msGushRemarkBlockName As String = "GUSH_REMARK"
	Private Const msGushRemarkBlockLayer As String = "C1648"
	Private Const msMAPIBlockName As String = "MAPI"

	Private Const msTitleTmpBlockName As String = "G_TITLE"
	Private Const msTitleTmpLayerName As String = "TITLE_TMP"




	Private Const msStampBlockNameType2 As String = "C1643_FORM1"
	Private Const msStampBlockNameType12 As String = "C1639_FORM1"
	Private Const msStampBlockNameType1 As String = "C1643_FORM1"

	Private Const msDmAddressBlockName As String = "DM_Address"
	Private Const msPlanNumberBlockName As String = "SU77_11"


	Private Const msStampBlockType2Layer As String = "C1643"
	Private Const msStampBlockType12Layer As String = "C1639"
	Private Const msStampBlockType1Layer As String = "C1643"

	Private Const msHeaderBlockName As String = "UD_ComStampHeader"
	Private Const msFooterBlockName As String = "UD_ComStampFooter"
	Private Const msRowBlockName As String = "UD_ComStampRow"

	Private Const msQ As String = "תולובג דועיתל טירשת"
	Private Const msW As String = "תוכנית לצרכי רישום"
	Private msStampBlockLayer As String


	'Private Const msmsBlockPathBlockPath As String = "M:\Dm_Work\Blocks\Hanit"
	Private Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit_1.3"


	' Private Const msFrameComLine As String = "(load \""M:/Dm_Work/Support2/SURVEY2.LSP\"")(C:SURFG) "
	'	Private Const msFrameComLine1 As String = "(load ""M:/Dm_Work/Support2/SURVEY2.LSP"")(setq MYCALLBACK ""FormView"")(C:SURFG)(setq MYCALLBACK nil)"
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

	Private Const msMAPIDateAttribTag As String = "DATE"


	Private Const msGushLegalAreaAttribTag As String = "GUSH_LEGAL_AREA"
	Private Const msGushRegStatusNameAttribTag As String = "REGISTER_STATUS"

	Private Const msGushLastParcelNameAttribTag As String = "GUSH_LAST_PARCEL"
	Private Const msGeneralCommentNameAttribTag As String = "GENERAL_COMMENT"
	Private Const msGBPrefix As String = "ג.ב."
	Private Const msProcessNameAttribTag As String = "PROCESS_NAME"
	Private Const msSerialNumAttribTag As String = "SERIAL"

	Private Const msGushNumAttribTag As String = "GUSH_NUM"
	Private Const msGridNameAttribTag As String = "GRID_NAME"
	Private Const msWorkOrderAttribTag As String = "WORK_ORDER"
	Private Const msRegisterStatusAttribTag As String = "REGISTER_STATUS"
	Private Const msProcessTypeAttribTag As String = "PROCESS_TYPE"

	Private Const msCountyAttribTag As String = "COUNTY"
	Private Const msRegionAttribTag As String = "REGION"
	Private Const msGushAreaAttribTag As String = "GUSH_AREA"
	Private Const msSettlementAttribTag As String = "SETTLEMENT"

	'Private Const msScaleAttribTag As String = "SCALE"


	'	'לא הוסדר
	Private Const msPlanNumberAttribTag As String = "PLANNUMBER"
	Private Const msPageNoAttribTag As String = "NO"
	Private Const msCaptionAttribTag As String = "TEXT"



	Private Const msPlaceAttribTag As String = "PLACE"
	Private Const msSurveyorIDAttribTag As String = "SURVEYOR"
	Private Const msSurveyorNameAttribTag As String = "SURVEYOR_NAME"

	Private Const msCoordinateTopPartAttribTag As String = "TOPPART"
	Private Const msCoordinateBottomPartAttribTag As String = "BOTTOMPART"

	Private Const msMapStatusAttribTag As String = "GIL_STATUS"
	Private Const msSheetNoAttribTag As String = "GIL_NAME1"

	Private Const msPlanNumInGushAttribTag As String = "PLAN_NUM_IN_GUSH"
	Private Const msTazarInGushAttribTag As String = "TZR_IN_GUSH_HESDER"
	Private Const msGushShumaAttribTag As String = "GUSH_SHUMA_HESDER"
	Private Const msMarkDescrAttribTag As String = "MARK_DESC"
	Private Const msPT_TransferMethodAttribTag As String = "PT_TRANSFER_METHOD"
	Private Const msEngSurveyMethodAttribTag As String = "ENG_SURVEY_METHOD"

	Private Const mdLandscapePageWidth As Double = 640.0
	Private Const mdLandscapePageHeight As Double = 480.0






	Private Const msPlanType9 As String = "קמ""ק"
	Private Const msPlanType12 As String = "תת""ג"
	Private Const msPlanType2 As String = "תצ""ר"
	Private Const msPlanType1 As String = "הסדר"

	Private Const msPlanningPlace As String = "בני ברק"
	Private Const mdFrameHeight As Double = 600
	Private Const mdFrameWidth As Double = 700
	Private mdCurrentFrameWidth As Double
	Private mdCurrentFrameHeight As Double



	Dim miProjectCode As Integer
	Dim miDetailNo As Integer
	Private WithEvents moProjectDataTable As System.Data.DataTable
	Private moProjectDataAdapter As Data.Common.DbDataAdapter

	Private WithEvents moPortionDataTable As System.Data.DataTable
	Private moPortionDataAdapter As Data.Common.DbDataAdapter

	'Private moProjectDataRow As System.Data.DataRow


	Private moDataRow As DataRow
	Private miPlanType As Integer
	Private msPlanTypeName As String

	Private mbEventsEnabled As Boolean
	Private miInitPlanID As Integer
	Private miOriginalBlockNo As Integer
	Private miOriginalBlockAddNo As Integer

	Private msNormalParcels As String
	Private msTempParcels As String


	' Me.mskOriginalBlockNo.Text


	'	Me.mskOriginalBlockNo.Text = sValue






	Private msGBDOSEnd As String = New String({Chr(46), Chr(129), Chr(46), Chr(130)})

	Private moaNoteChecks(17) As NoteCheck

	'	Dim c As Char = New Char()

	Dim chars() As Char = {ChrW(&H61), ChrW(&H308)}




	Private Const mdDeltaPosX As Double = 400
	Private Const mdDeltaPosY As Double = 250
	Private mcolInvisibleAttribs As ObjectIdCollection = New ObjectIdCollection()
	Private mtGenBlockRecObjID As ObjectId
	Private moaAttribDefs() As AttributeDefinition = Nothing
	''' <summary>
	''' '''''''''
	''' </summary>
	Private Shared moFrameInsPoint As DMAcadExt.TPlnPoint


	Private mtGenBlockRefObjID As ObjectId
	Private mtGenInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtEllipseBlockRefObjID As ObjectId
	Private mtEllipseInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtStampBlockRefObjID As ObjectId
	Private mtStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d

	Private mtNotesInsPoint As Autodesk.AutoCAD.Geometry.Point3d
	Private mtCaptionInsPoint As Autodesk.AutoCAD.Geometry.Point3d




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
	Private moDataMapAcadBlock As DMAcadExt.AcadBlock


	Private moStampAcadBlock As DMAcadExt.AcadBlock
	Private moPageNoAcadBlock As DMAcadExt.AcadBlock

	Private moDmAddressAcadBlock As DMAcadExt.AcadBlock

	Private moDirArrowAcadBlock As DMAcadExt.AcadBlock
	Private moType1DirArrowAcadBlock As DMAcadExt.AcadBlock


	Private moCaptionAcadBlock As DMAcadExt.AcadBlock
	Private moGushPropAcadBlock As DMAcadExt.AcadBlock
	Private moGenType1AcadBlock As DMAcadExt.AcadBlock

	Private moScaleTextAcadBlock As DMAcadExt.AcadBlock
	Private moMAPIAcadBlock As DMAcadExt.AcadBlock

	Private moTitleAcadBlock As DMAcadExt.AcadBlock



	Private moType1FrameAcadBlock As DMAcadExt.AcadBlock
	Private moCoordinateAcadBlock As DMAcadExt.AcadBlock

	Private moLegendAcadBlock As DMAcadExt.AcadBlock
	Private moGushRemarkAcadBlock As DMAcadExt.AcadBlock
	Private moUpdateTableAcadBlock As DMAcadExt.AcadBlock



	Private WithEvents moProjectPlanTable As System.Data.DataTable
	Private moProjectPlanDataAdapter As Data.Common.DbDataAdapter

	Private WithEvents moBasePlansTable As System.Data.DataTable
	Private moBasePlansAdapter As Data.Common.DbDataAdapter
	Private moBasePlansView As System.Data.DataView
	Private miPlanID As Integer
	'Private moProjectDataAdapter As Data.Common.DbDataAdapter
	Private moNewRow As DataRow
	Private mfUD_SelectPlan As frmUD_SelectPlan


	Private WithEvents moBaseApproveDateBinding As Binding

	Private WithEvents moSurveyDateBinding As Binding
	Private WithEvents moSurveyDate1Binding As Binding
	Private WithEvents moSurveyDate2Binding As Binding
	Private WithEvents moEndDateBinding As Binding

	'Survey
	Private moScaleAcadBlock As DMAcadExt.AcadBlock
	Private msGush As String
	Private msParcels As String
	Private mbDBSource As Boolean
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
	Private Sub zzLoadBasePlanTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetBasePlanData"

		moBasePlansTable = New System.Data.DataTable("BasePlan")
		moBasePlansAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)

		If bSchemaOnly Then
			moBasePlansAdapter.FillSchema(moBasePlansTable, SchemaType.Source)
		Else
			moBasePlansAdapter.Fill(moBasePlansTable)
		End If


	End Sub

	Private Sub zzOpenSelectPlanForm()
		Dim tFormLocation As Point
		Dim tFormLocation1 As Point
		Dim tFormLocation2 As Point

		mfUD_SelectPlan = New frmUD_SelectPlan(moProjectPlanTable)
		mfUD_SelectPlan.Owner = Me
		mfUD_SelectPlan.StartPosition = FormStartPosition.Manual
		tFormLocation1 = Me.PointToScreen(mskPlanID.Location)
		tFormLocation2 = Me.PointToScreen(cmdOpenSelectPlanForm.Location)
		tFormLocation2.X = tFormLocation2.X - cmdOpenSelectPlanForm.Size.Width
		tFormLocation2.Y = tFormLocation2.Y + cmdOpenSelectPlanForm.Size.Height

		tFormLocation.X = Me.Location.X + Me.Size.Width - mfUD_SelectPlan.Size.Width
		tFormLocation.Y = Me.Location.Y + mskPlanID.Location.Y + mskPlanID.Size.Height
		mfUD_SelectPlan.Location = tFormLocation2
		mfUD_SelectPlan.ShowDialog()
		If False Then
			If mfUD_SelectPlan.SelectedPlanID <> 0 Then
				zzMoveTo(mfUD_SelectPlan.SelectedPlanID)
			End If
		End If
		If mfUD_SelectPlan.SelectedIndexNum <> 0 Then
			zzMoveTo(mfUD_SelectPlan.SelectedIndexNum)
		End If

	End Sub
	Private Sub zzMoveTo(iSelectedIndexNum As Integer)
		Dim oPlanView As System.Data.DataView = New DataView(moProjectPlanTable, "IndexNum<" & iSelectedIndexNum.ToString(), "IndexNum", DataViewRowState.CurrentRows)
		Dim oCurrencyManager As CurrencyManager = bnsPlans.CurrencyManager
		oCurrencyManager.Position = oPlanView.Count
	End Sub

	Private Sub zzMoveToOld(iSelectedPlanID As Integer)
		Dim oPlanView As System.Data.DataView = New DataView(moProjectPlanTable, "PlanID<" & iSelectedPlanID.ToString(), "PlanID", DataViewRowState.CurrentRows)
		Dim oCurrencyManager As CurrencyManager = bnsPlans.CurrencyManager
		oCurrencyManager.Position = oPlanView.Count
	End Sub
	Private Sub zzMoveToLast()
		If moProjectPlanTable.Rows.Count > 1 Then
			Dim oCurrencyManager As CurrencyManager = bnsPlans.CurrencyManager
			oCurrencyManager.Position = moProjectPlanTable.Rows.Count - 1
		End If

	End Sub

	Public Shared Function GetPlanTypeName(iPlanType As enPlanType) As String
		Select Case iPlanType
			Case enPlanType.PlanType1
				Return msPlanType1
			Case enPlanType.PlanType2
				Return msPlanType2
			Case enPlanType.PlanType9
				Return msPlanType9
			Case enPlanType.PlanType12
				Return msPlanType12
			Case Else
				Return Nothing
		End Select
	End Function
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()
		zzMyInitializeComponent()
		' Add any initialization after the InitializeComponent() call.

	End Sub


	Public Sub New(bDBSource As Boolean, Optional iPlanID As Integer = 0)
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		' This call is required by the designer.
		InitializeComponent()
		zzMyInitializeComponent()
		mbDBSource = bDBSource
		If mbDBSource Then
			zzInitUD_Project()
			zzLoadProjectDataTable(False)
			zzLoadPortionDataTable(False)
		End If
		' Add any initialization after the InitializeComponent() call.
		'Dim sNormalParcelsString, sTempParcelsString As String
		zzInit()
		zzSetAttribTags()
		zzInitAcadBlocks()
		zzSetToolTip()

		AcadReport.RepApp.InitDWGScaleFactor()
		mdCurrentFrameWidth = mdFrameWidth * AcadReport.RepApp.DrawingScaleFactor
		mdCurrentFrameHeight = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor
		'DMCommon.Debug.MsgBox("08_932", iPlanID, sNormalParcelsString, sTempParcelsString)
		If iPlanID <> 0 Then
			miInitPlanID = iPlanID

		End If
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

		'moPlanNumberAcadBlock = New DMAcadExt.AcadBlock(msPlanNumberBlockName, msBlockPath)
		'moPlanNumberAcadBlock.Fields = New String() {msPlanNumberAttribTag}



		'moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockName, msBlockPath)
		'	moStampAcadBlock.Fields = New String() {msSurveyDate1AttribTag, msSurveyDate2AttribTag, msEndDateAttribTag, msUpdateDateAttribTag, msPlaceAttribTag, msSurveyorIDAttribTag, msSurveyorNameAttribTag}

		moDmAddressAcadBlock = New DMAcadExt.AcadBlock(msDmAddressBlockName, msBlockPath)



		moCoordinateAcadBlock = New DMAcadExt.AcadBlock(msCoordinateBlockName, msBlockPath)

		moDirArrowAcadBlock = New DMAcadExt.AcadBlock(msTazarDirArrowBlockName, msBlockPath)
		moType1DirArrowAcadBlock = New DMAcadExt.AcadBlock(msType1DirArrowBlockName, msBlockPath)
		moPageNoAcadBlock = New DMAcadExt.AcadBlock(msPageNoBlockName, msBlockPath)
		moPageNoAcadBlock.Fields = New String() {msPageNoAttribTag}
		moCaptionAcadBlock = New DMAcadExt.AcadBlock(msCaptionBlockName, msBlockPath)
		moCaptionAcadBlock.Fields = New String() {msCaptionAttribTag}
		moGushPropAcadBlock = New DMAcadExt.AcadBlock(msGushPropBlockName, msBlockPath)
		moGushPropAcadBlock.Fields = New String() {msGushNumAttribTag, msGridnameAttribTag, msWorkOrderAttribTag, msRegisterStatusAttribTag, msProcessTypeAttribTag}
		moGenType1AcadBlock = New DMAcadExt.AcadBlock(msGenType1BlockName, msBlockPath)
		moGenType1AcadBlock.Fields = New String() {msCountyAttribTag, msRegionAttribTag, msGushAreaAttribTag, msSettlementAttribTag}
		moScaleTextAcadBlock = New DMAcadExt.AcadBlock(msScaleTextBlockName, msBlockPath)
		moScaleTextAcadBlock.Fields = New String() {msScaleAttribTag}

		moMAPIAcadBlock = New DMAcadExt.AcadBlock(msMAPIBlockName, msBlockPath)
		moMAPIAcadBlock.Fields = New String() {msMAPIDateAttribTag}

		moTitleAcadBlock = New DMAcadExt.AcadBlock(msTitleTmpBlockName, msBlockPath)
		moTitleAcadBlock.Fields = New String() {msMapStatusAttribTag, msSheetNoAttribTag}

		moLegendAcadBlock = New DMAcadExt.AcadBlock(msLegendBlockName, msBlockPath)
		'	moLegendAcadBlock.Fields = New String() {msGushNumAttribTag, msGridNameAttribTag, msWorkOrderAttribTag, msRegisterStatusAttribTag, msProcessTypeAttribTag}
		moGushRemarkAcadBlock = New DMAcadExt.AcadBlock(msGushRemarkBlockName, msBlockPath)
		moGushRemarkAcadBlock.Fields = New String() {msGushNumAttribTag, msGridNameAttribTag, msWorkOrderAttribTag, msRegisterStatusAttribTag, msProcessTypeAttribTag}
		moUpdateTableAcadBlock = New DMAcadExt.AcadBlock(msUpdateTableBlockName, msBlockPath)
		'	moUpdateTableAcadBlock.Fields = New String() {msGushNumAttribTag, msGridNameAttribTag, msWorkOrderAttribTag, msRegisterStatusAttribTag, msProcessTypeAttribTag}


		'Private moLegendAcadBlock As DMAcadExt.AcadBlock
		'	Private moGushRemarkAcadBlock As DMAcadExt.AcadBlock
		'Private moUpdateTableAcadBlock As DMAcadExt.AcadBlock


		'Private Const msLegendBlockName As String = "LEGAND"
		'Private Const msGushRemarkBlockName As String = "GUSH_REMARK"
		'Private Const msUpdateTableBlockName As String = "UPDATE_TABLE"


	End Sub
	Private Sub zzInitStampAcadBlock()

		zzGetPlanType()

		If miPlanType = 2 Then
			moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockNameType2, msBlockPath)
			msStampBlockLayer = msStampBlockType2Layer
		ElseIf miPlanType = 12 Then
			moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockNameType12, msBlockPath)
			msStampBlockLayer = msStampBlockType12Layer
		ElseIf miPlanType = 1 Then
			moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockNameType1, msBlockPath)
			msStampBlockLayer = msStampBlockType12Layer
		End If
		If moStampAcadBlock IsNot Nothing Then
			moStampAcadBlock.Fields = New String() {msSurveyDate1AttribTag, msSurveyDate2AttribTag, msEndDateAttribTag, msUpdateDateAttribTag, msPlaceAttribTag, msSurveyorIDAttribTag, msSurveyorNameAttribTag}
		End If

	End Sub
	Public Property OriginalBlockNo As Integer
		Get
			Return DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockNo))
		End Get
		Set(iValue As Integer)
			miOriginalBlockNo = iValue
		End Set
	End Property
	Public Property OriginalBlockAddNo As Integer
		Get
			Return DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockAddNo))
		End Get
		Set(iValue As Integer)
			miOriginalBlockAddNo = iValue
		End Set
	End Property

	Public Property NormalParcels As String
		Get
			Return Me.txtNormalParcels.Text
		End Get
		Set(sValue As String)
			msNormalParcels = sValue
		End Set
	End Property
	Public Property TempParcels As String
		Get
			Return Me.txtTempParcels.Text
		End Get
		Set(sValue As String)
			msTempParcels = sValue
		End Set
	End Property
	Public Sub SetPlanType(bIsTTG As Boolean)
		If bIsTTG Then
			Me.rdbPlanType12.Checked = True
		Else
			Me.rdbPlanType2.Checked = True
		End If

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
	Private Function zzGetScaleBlockName1() As String
		Dim iScaleValue As Integer = Convert.ToInt32(1000.0 * AcadReport.RepApp.DrawingScaleFactor)
		iScaleValue = 625
		Return msTazarScaleBaseBlockName & Convert.ToString(iScaleValue)
	End Function
	Private Function zzGetScaleBlockName() As String
		Dim iScaleValue As Integer = Convert.ToInt32(1000.0 * AcadReport.RepApp.DrawingScaleFactor)

		Return msDMScaleBaseBlockName & Convert.ToString(iScaleValue)
	End Function
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
		moaNoteChecks(0).ByPlan = True
		moaNoteChecks(1).ByPlan = True
		moaNoteChecks(2).ByPlan = True
		moaNoteChecks(3).ByPlan = True
		moaNoteChecks(14).ByPlan = True
		moaNoteChecks(15).ByPlan = True
		moaNoteChecks(16).ByPlan = True




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

		If False Then
			If moProjectDataTable.Rows.Count = 0 Then
				moDataRow = moProjectDataTable.NewRow()
			Else
				moDataRow = moProjectDataTable.Rows.Item(0)

			End If
		End If


		'	zzSetPlanType()
		'	zzSetMainData()

	End Sub

	Private Sub zzLoadPortionDataTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetProjectPortionData"

		moPortionDataTable = New System.Data.DataTable("PortionDataTable")
		moPortionDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)

		If bSchemaOnly Then
			moPortionDataAdapter.FillSchema(moPortionDataTable, SchemaType.Source)
		Else
			moPortionDataAdapter.Fill(moPortionDataTable)
		End If
		'   moFragmentTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))

		If False Then
			If moProjectDataTable.Rows.Count = 0 Then
				moDataRow = moProjectDataTable.NewRow()
			Else
				moDataRow = moProjectDataTable.Rows.Item(0)

			End If
		End If


		'	zzSetPlanType()
		'	zzSetMainData()

	End Sub

	Private Sub zzSetBinding()
		Me.cmbDistrict.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "DistrictID"))
		Me.cmbSubdistrict.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "SubdistrictID"))
		Me.cmbLocality.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "LocalityID"))

		Me.cmbCommittee.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "CommitteeID"))

		Me.cmbScales.DataBindings.Add(New Binding("SelectedValue", Me.bnsPlans, "ScaleID"))



		Me.mskPlanID.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanID", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.mskIndexNum.DataBindings.Add(New Binding("Text", Me.bnsPlans, "IndexNum", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.mskSerialNumYear.DataBindings.Add(New Binding("Text", Me.bnsProject, "SerialNumYear"))
		Me.mskSerialNum.DataBindings.Add(New Binding("Text", Me.bnsProject, "SerialNum"))



		Me.mskOriginalBlockNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalBlockNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru
		Me.mskOriginalBlockAddNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalBlockAddNo", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru

		Me.txtNormalParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "OriginalParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing)) ' Tru
		Me.txtTempParcels.DataBindings.Add(New Binding("Text", Me.bnsPlans, "NewParcelList", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtPlanNum.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanNum", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.txtPlanPHNum.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PlanPHNum", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtWorkOrder.DataBindings.Add(New Binding("Text", Me.bnsPlans, "WorkOrder", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))
		Me.mskProcessNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "ProcessNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.mskProcessYear.DataBindings.Add(New Binding("Text", Me.bnsProject, "ProcessYear", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.txtGushLegalArea.DataBindings.Add(New Binding("Text", Me.bnsPlans, "LegalArea", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtLastParcelName.DataBindings.Add(New Binding("Text", Me.bnsPlans, "LastParcel", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtTaskNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "TaskNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.txtBaseSerialNumber.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseSerialNumber", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.dtpBaseApproveDate.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseApproveDate", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, "d"))

		Me.mskBaseProcessNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseProcessNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.mskBaseProcessYear.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseProcessYear", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.mskBaseBlockNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseBlockNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.mskBaseBlockAddNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "BaseBlockAddNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))



		Me.chkGushStatus.DataBindings.Add(New Binding("Checked", Me.bnsPlans, "BlockIsRegulated", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtOrderer.DataBindings.Add(New Binding("Text", Me.bnsProject, "Orderer", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtProjectNo.DataBindings.Add(New Binding("Text", Me.bnsProject, "ProjectNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.txtMAPIDate.DataBindings.Add(New Binding("Text", Me.bnsProject, "MAPI_Date", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))



		Me.cmbSurveyor.DataBindings.Add(New Binding("SelectedValue", Me.bnsProject, "SurveyorID", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtMoleNodules.DataBindings.Add(New Binding("Text", Me.bnsProject, "MoleNodules", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtRegistrationProgramNos.DataBindings.Add(New Binding("Text", Me.bnsProject, "RegistrationProgramNos", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.cmbMeasureMethod.DataBindings.Add(New Binding("SelectedValue", Me.bnsProject, "MeasureMethod"))
		Me.cmbBoundaryMeasureMethod.DataBindings.Add(New Binding("SelectedValue", Me.bnsProject, "BoundaryMeasureMethod"))
		Me.cmbBoundariesKind.DataBindings.Add(New Binding("Text", Me.bnsProject, "BoundariesKind"))

		moBaseApproveDateBinding = New Binding("Value", Me.bnsPlans, "BaseApproveDate", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		AddHandler moBaseApproveDateBinding.Parse, AddressOf zzParseDateTimePicker
		AddHandler moBaseApproveDateBinding.Format, AddressOf zzFormatDateTimePicker
		Me.dtpBaseApproveDate.DataBindings.Add(moBaseApproveDateBinding)

		moSurveyDateBinding = New Binding("Value", Me.bnsProject, "SurveyDate", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		AddHandler moSurveyDateBinding.Parse, AddressOf zzParseDateTimePicker
		AddHandler moSurveyDateBinding.Format, AddressOf zzFormatDateTimePicker
		Me.dtpSurveyDate.DataBindings.Add(moSurveyDateBinding)

		moSurveyDate1Binding = New Binding("Value", Me.bnsProject, "SurveyDate1", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		AddHandler moSurveyDate1Binding.Parse, AddressOf zzParseDateTimePicker
		AddHandler moSurveyDate1Binding.Format, AddressOf zzFormatDateTimePicker
		Me.dtpSurveyDate1.DataBindings.Add(moSurveyDate1Binding)

		moSurveyDate2Binding = New Binding("Value", Me.bnsProject, "SurveyDate2", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		AddHandler moSurveyDate2Binding.Parse, AddressOf zzParseDateTimePicker
		AddHandler moSurveyDate2Binding.Format, AddressOf zzFormatDateTimePicker
		Me.dtpSurveyDate2.DataBindings.Add(moSurveyDate2Binding)

		moEndDateBinding = New Binding("Value", Me.bnsProject, "EndDate", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		AddHandler moEndDateBinding.Parse, AddressOf zzParseDateTimePicker
		AddHandler moEndDateBinding.Format, AddressOf zzFormatDateTimePicker
		Me.dtpEndDate.DataBindings.Add(moEndDateBinding)

		Me.txtPortionNo.DataBindings.Add(New Binding("Text", Me.bnsPlans, "PortionNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))

		Me.txtOnlineFormNo.DataBindings.Add(New Binding("Text", Me.bnsPortions, "OnlineFormNo", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.dtpOnlineFormDate.DataBindings.Add(New Binding("Text", Me.bnsPortions, "OnlineFormDate", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtWorkChecker.DataBindings.Add(New Binding("Text", Me.bnsPortions, "WorkChecker", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtQualityScore.DataBindings.Add(New Binding("Text", Me.bnsPortions, "QualityScore", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtAuditCost.DataBindings.Add(New Binding("Text", Me.bnsPortions, "AuditCost", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))
		Me.txtPlanCost.DataBindings.Add(New Binding("Text", Me.bnsPortions, "PlanCost", True, System.Windows.Forms.DataSourceUpdateMode.OnValidation, String.Empty, Nothing))







		zzSetBindingNoteCheck()
		If False Then

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
		End If


	End Sub
	Private Sub zzSetBindingNoteCheck()
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).SetBinding()
		Next

	End Sub
	Private Sub zzParseDateTimePicker(oSender As System.Object, e As System.Windows.Forms.ConvertEventArgs)
		Dim oBinding As Binding = DirectCast(oSender, Binding)
		Dim oDateTimePicker As DateTimePicker = DirectCast(oBinding.Control, DateTimePicker)
		Dim tPickerVal As Date = oDateTimePicker.Value
		Dim tArgVal As System.Object = e.Value


		If Not oDateTimePicker.Checked Then
			e.Value = DBNull.Value
		Else


			Dim oDate As Date = DirectCast(e.Value, Date)

			e.Value = New Nullable(Of Date)(oDate)


		End If


	End Sub
	Private Sub zzFormatDateTimePicker(oSender As System.Object, e As System.Windows.Forms.ConvertEventArgs)
		Dim oBinding As Binding = DirectCast(oSender, Binding)
		Dim oDateTimePicker As DateTimePicker = DirectCast(oBinding.Control, DateTimePicker)
		Dim tPickerVal As Date = oDateTimePicker.Value
		Dim tArgVal As System.Object = e.Value

		If IsDBNull(e.Value) Then
			oDateTimePicker.Checked = False
		Else
			If True Then
				oDateTimePicker.Value = Convert.ToDateTime(e.Value)
				oDateTimePicker.Checked = True
				oDateTimePicker.Invalidate()
				oDateTimePicker.Refresh()

			End If

		End If

	End Sub


	Private Sub zzSetMainData()

		Me.cmbDistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("DistrictID"))
		Me.cmbSubdistrict.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("SubdistrictID"))
		Me.cmbLocality.SelectedValue = DMCommon.Functions.CIntN(moDataRow.Item("Locality"))
		'	Me.txtOrderer.Text = DMCommon.Functions.CStrN(moDataRow.Item("Orderer"))
		'	DMCommon.Debug.MsgBox("13_023", DMCommon.Functions.CIntN(moDataRow.Item("District")), Me.cmbDistrict.SelectedValue, Me.cmbSubdistrict.SelectedValue)

	End Sub
	Private Function zzGetParameters(Optional bPlanID As Boolean = False) As System.Data.Common.DbParameter()
		Dim iParamUB As Integer
		If bPlanID Then
			iParamUB = 2
		Else
			iParamUB = 1
		End If
		Dim oaParams(iParamUB) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		If bPlanID Then
			Dim iParamPlanID As Integer = zzGetCurrentPlanID()
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prPlanID", DbType.Int32, iParamPlanID)
		End If

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

		Me.cmbCommittee.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbCommittee.DisplayMember = DMCommon.ItemData.DisplayMember
		Me.cmbCommittee.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetCommitteesList()


		Me.cmbScales.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbScales.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbScales.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetScalesList()


		Me.cmbMeasureMethod.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbMeasureMethod.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbMeasureMethod.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetMeasureMethods(False)


		Me.cmbParcelingType.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbParcelingType.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbParcelingType.DataSource = TPlServerDB.ServerDB.CurrentProjectDB.GetParcelingTypes()

		Me.cmbBoundaryMeasureMethod.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbBoundaryMeasureMethod.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.cmbBoundaryMeasureMethod.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetMeasureMethods(True)


		Me.cmbSurveyor.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSurveyorLicenseView()
		Me.cmbSurveyor.ValueMember = "LicenseNo"
		Me.cmbSurveyor.DisplayMember = "Empl_NameFamily"

		Me.ccbPlanType.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetPlanTypeList()
		Me.ccbPlanType.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbPlanType.DisplayMember = DMCommon.ItemData.DisplayMember

		Me.dgvBasePlans.AutoGenerateColumns = False
		'חדשה/2005/2012
		NoteCheck.SetBindingSource(bnsProject, bnsPlans)
	End Sub

	Private Sub frmUD_General_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub frmUD_General_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		'	Me.cmbDistrict.SelectedIndex = -1
		Dim oAdded As System.Object
		zzLoadProjectPlanTable(False)
		zzLoadBasePlanTable(False)
		Me.bnsPlans.DataSource = moProjectPlanTable
		Me.bnnPlans.BindingSource = Me.bnsPlans
		Me.bnsProject.DataSource = moProjectDataTable
		If moProjectDataTable.Rows.Count = 0 Then
			'DMCommon.Debug.MsgBox("13_036f", bnsProject.Count)
			oAdded = bnsProject.AddNew()
			'DMCommon.Debug.MsgBox("13_036g", bnsProject.Count, moProjectPlanTable.Rows.Count, bnsProject.Current, bnsProject.Current.GetType(), oAdded)

		End If
		Me.bnsPortions.DataSource = moPortionDataTable
		moBasePlansView = New DataView(moBasePlansTable)
		Me.dgvBasePlans.DataSource = moBasePlansView
		'	zzLoadBlockRefData(True)
		zzSetScale()
		zzSetBinding()
		If miInitPlanID <> 0 Then

			zzMoveTo(miInitPlanID)
			Dim iBlockNo As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockNo))
			Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockAddNo))
			Dim sNormalParcelsString As String = DMCommon.Functions.CStrN(zzGetBindingSourceVal(Me.txtNormalParcels))
			Dim sTempParcelsString As String = DMCommon.Functions.CStrN(zzGetBindingSourceVal(Me.txtTempParcels))
			'	DMCommon.Debug.MsgBox("08_932", miInitPlanID, iBlockNo, iBlockAddNo, sNormalParcelsString, sTempParcelsString, "______________", miOriginalBlockNo, miOriginalBlockAddNo, msNormalParcels, msTempParcels)
			If (miOriginalBlockNo = iBlockNo) AndAlso (miOriginalBlockAddNo = iBlockAddNo) AndAlso String.IsNullOrEmpty(sNormalParcelsString) AndAlso String.IsNullOrEmpty(sTempParcelsString) Then
				Me.txtNormalParcels.Text = msNormalParcels
				Me.txtTempParcels.Text = msTempParcels

			End If
		Else
			zzMoveToLast()
			'	
		End If
	End Sub
	Private Sub zzSetInsertionPointNew()




		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim oBorderAcadBlock As DMAcadExt.AcadBlock
		moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName, msBlockPath)
		DMCommon.Debug.MsgBox("15_001", "zzSetInsertionPointNew", moFrameAcadBlock Is Nothing)
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
			MessageBox.Show("Number of New Frames: " & CStr(moFrameAcadBlock.ReferenceCount), "04_929")
			Return
		End If


		DMCommon.Debug.MsgBox("15_001a", "zzSetInsertionPointNew_End", moFrameAcadBlock Is Nothing)




	End Sub
	Private Sub zzCalcInsertPoints()


		Dim oInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint
		'	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
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
		'  MessageBox.Show(CStr(mtStampInsPoint.ToString()) & vbCrLf & mtEllipseInsPoint.ToString() & vbCrLf & mtGenInsPoint.ToString() & vbCrLf & mtNotesInsPoint.ToString(), "04_929")

	End Sub
	Private Sub zzSetInsertionPointType1()



		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim dCurrentFrameWidth As Double = mdFrameWidth * AcadReport.RepApp.DrawingScaleFactor
		Dim dCurrentFrameHeight As Double = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor


		moFrameAcadBlock = New DMAcadExt.AcadBlock(msFrameBlockNameType1_Hor)
		moFrameAcadBlock.OpenForRead()
		moFrameAcadBlock.LoadAllReferences()

		If moFrameAcadBlock.ReferenceCount <= 0 Then
			moFrameAcadBlock = New DMAcadExt.AcadBlock(msFrameBlockNameType1_Ver)
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
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint
			moFrameInsPoint = oInsPoint
			mtStampInsPoint = oInsPoint.AcGePoint3d

			oBlockInsPoint = oInsPoint.GetMoved(0.0, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY

			mtGenInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtNotesInsPoint = oBlockInsPoint.AcGePoint3d

			moFrameInsPoint = oInsPoint


			'   mbFrameExists = True

			moNoteInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)

			'   HebrewTrans.vb() : Line 334


		End If







	End Sub
	Private Sub zzSetInsertionPoint()



		'     Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim oBlockRefData As DMAcadExt.BlockRefData
		Dim dCurrentFrameWidth As Double = mdFrameWidth * AcadReport.RepApp.DrawingScaleFactor
		Dim dCurrentFrameHeight As Double = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor


		moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName)
		moFrameAcadBlock.OpenForRead()
		moFrameAcadBlock.LoadAllReferences()

		If moFrameAcadBlock.ReferenceCount <= 0 Then
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
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint

			mtStampInsPoint = oInsPoint.AcGePoint3d

			oBlockInsPoint = oInsPoint.GetMoved(0.0, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, dCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtGenInsPoint = oBlockInsPoint.AcGePoint3d


			oBlockInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth, 0.0)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
			'	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
			mtNotesInsPoint = oBlockInsPoint.AcGePoint3d

			moFrameInsPoint = oInsPoint


			'   mbFrameExists = True

			moNoteInsPoint = oInsPoint.GetMoved(dCurrentFrameWidth - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)

			'   HebrewTrans.vb() : Line 334


		End If







	End Sub


	Private Sub zzSetScale()
		Dim dScale As Double = 1000 * AcadReport.RepApp.DrawingScaleFactor
		Dim sScale As String = "1:" & Convert.ToString(dScale)
		Me.cmbScales.Text = sScale
	End Sub
	Private Sub cmdSaveDB_Click()
		zzGetPlanType()
		moDataRow.Item("PlanType") = miPlanType
		If False Then

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

		Dim oRow As DataRowView = zzGetProjectDataRow()
		If oRow IsNot Nothing Then
			oRow.EndEdit()
		End If
		oRow = zzGetPlanDataRow()
		If oRow IsNot Nothing Then
			oRow.EndEdit()
		End If
		moProjectDataAdapter.Update(moProjectDataTable)
		moProjectPlanDataAdapter.Update(moProjectPlanTable)
	End Sub

	Private Sub cmdEraseDB_Click(oSender As System.Object, e As System.EventArgs) Handles cmdEraseDB.Click
		Const sSPName As String = "ClearPlanData"
		Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters(True))
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
			msPlanTypeName = "הסדר קרקעות"
		ElseIf Me.rdbPlanType2.Checked Then
			miPlanType = 2
			msPlanTypeName = "תכנית לצרכי רישום"
		ElseIf Me.rdbPlanType9.Checked Then
			miPlanType = 9
			msPlanTypeName = "קמ""ק"
		ElseIf Me.rdbPlanType12.Checked Then
			miPlanType = 12
			msPlanTypeName = "תשריט לתיעוד גבולות"
		End If

		If miPlanType <> 0 Then
			Return Convert.ToString(miPlanType)
		Else
			Return String.Empty
		End If
	End Function
	Private Function zzGetGenData(bUpdate As Boolean) As Dictionary(Of String, String)

		Dim oDynValue As System.Object
		Dim oHebText As DMCommon.HebrewTrans

		'	Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(colBorderBlockRefs.Item(0), OpenMode.ForWrite)
		'MessageBox.Show(CStr(oBlockRef Is Nothing) & vbCrLf & colBorderBlockRefs.Item(0).ToString(), "04_130")


		Dim oaAttribDefs() As AttributeDefinition = Nothing
		'	Dim sBlockName As String = "C1640"
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		If cmbDistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msDistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbDistrict.Text, True))
		ElseIf bUpdate Then
			dicAttribValues.Add(msDistrictAttribTag, String.Empty)
		End If
		If cmbSubdistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msSubdistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, True)) '  DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, 
		ElseIf bUpdate Then
			dicAttribValues.Add(msSubdistrictAttribTag, String.Empty)
		End If
		If cmbLocality.SelectedIndex <> -1 Then
			dicAttribValues.Add(msLocalityNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbLocality.Text, True))
			oDynValue = cmbLocality.SelectedValue
			dicAttribValues.Add(msLocalityCodeAttribTag, oDynValue.ToString())
		ElseIf bUpdate Then

			dicAttribValues.Add(msLocalityNameAttribTag, String.Empty)

			dicAttribValues.Add(msLocalityCodeAttribTag, String.Empty)


		End If
		Dim iBlockNo As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockNo))
		Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.mskOriginalBlockAddNo))
		Dim sBlockFullName As String
		If iBlockNo <> 0 Then
			sBlockFullName = TopoManager.TPlanGraph.TplnBlock.GetBlockName(iBlockNo, iBlockAddNo)
			dicAttribValues.Add(msGushAttribTag, sBlockFullName)
		ElseIf bUpdate Then

			dicAttribValues.Add(msGushAttribTag, String.Empty)
		End If




		Dim sParcelString As String = DMCommon.Functions.JoinString(DMCommon.Hebrew.InsBracketsList(Me.txtTempParcels.Text), Me.txtNormalParcels.Text)
		If Not String.IsNullOrEmpty(sParcelString) Then
			dicAttribValues.Add(msParcelsAttribTag, sParcelString)
		ElseIf bUpdate Then
			dicAttribValues.Add(msParcelsAttribTag, String.Empty)
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
		Dim dArea As Double = DMCommon.Functions.CDblN(zzGetBindingSourceVal(Me.txtGushLegalArea))

		If dArea <> 0.0 Then


			'dicAttribValues.Add(msGushLegalAreaAttribTag, Me.txtGushLegalArea.Text)
			dicAttribValues.Add(msGushLegalAreaAttribTag, FormatNumber(dArea, 3))
		ElseIf bUpdate Then
			dicAttribValues.Add(msGushLegalAreaAttribTag, String.Empty)
		End If



		dicAttribValues.Add(msGushRegStatusNameAttribTag, DMCommon.Hebrew.WordToDOS(zzGetGushStatus(), True))

		Dim iLastParcel As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.txtLastParcelName))

		If iLastParcel <> 0 Then
			dicAttribValues.Add(msGushLastParcelNameAttribTag, Me.txtLastParcelName.Text)
		End If




		''''''''''''''''''''''  dicAttribValues.Add(msGeneralCommentNameAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtGeneralComment.Text, True))


		Return dicAttribValues


	End Function

	Private Function zzGetEllipseData() As Dictionary(Of String, String)
		Const sSlash As String = "/"
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		dicAttribValues.Add(msProcessNameAttribTag, Me.mskProcessNo.Text & sSlash & Me.mskProcessYear.Text)
		dicAttribValues.Add(msSerialNumAttribTag, mskSerialNumYear.Text & sSlash & Me.mskSerialNum.Text & sSlash & Me.mskIndexNum.Text & " " & msGBDOSEnd)
		Return dicAttribValues

	End Function
	Private Function zzGetPageNoData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		dicAttribValues.Add(msPageNoAttribTag, Me.mskIndexNum.Text)
		Return dicAttribValues
	End Function
	Private Function zzGetCaptionData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		DMCommon.Debug.MsgBox("!ReferenceCount, msPlanTypeName", moCaptionAcadBlock.ReferenceCount, msPlanTypeName)
		dicAttribValues.Add(msCaptionAttribTag, DMCommon.Hebrew.WordToDOS(msPlanTypeName, False))
		Return dicAttribValues
	End Function

	Private Function zzGetScaleTextData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		dicAttribValues.Add(msScaleAttribTag, FormatNumber(1000.0 * AcadReport.RepApp.DrawingScaleFactor, 0,,, TriState.False))
		Return dicAttribValues
	End Function

	Private Function zzGetGenType1Data(bUpdate As Boolean) As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim dArea As Double = DMCommon.Functions.CDblN(zzGetBindingSourceVal(Me.txtGushLegalArea))
		Dim sArea As String

		If dArea <> 0.0 Then
			sArea = FormatNumber(dArea, 3)
		Else
			sArea = String.Empty
		End If

		If cmbDistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msRegionAttribTag, DMCommon.Hebrew.WordToDOS(cmbDistrict.Text, True))
		ElseIf bUpdate Then
			dicAttribValues.Add(msRegionAttribTag, String.Empty)

		End If

		If cmbSubdistrict.SelectedIndex <> -1 Then
			dicAttribValues.Add(msCountyAttribTag, DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, True)) '  DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, 
		ElseIf bUpdate Then
			dicAttribValues.Add(msCountyAttribTag, String.Empty)
		End If




		dicAttribValues.Add(msGushAreaAttribTag, sArea)
		dicAttribValues.Add(msSettlementAttribTag, DMCommon.Hebrew.WordToDOS(zzGetLocalityName(), True))




		Return dicAttribValues
	End Function
	Private Function zzGetGushPropData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		dicAttribValues.Add(msGushNumAttribTag, zzToFullBlockName(Me.mskOriginalBlockNo.Text, Me.mskOriginalBlockAddNo.Text))
		dicAttribValues.Add(msGridNameAttribTag, zzGetNetType())

		dicAttribValues.Add(msRegisterStatusAttribTag, "11")
		dicAttribValues.Add(msProcessTypeAttribTag, "1")

		Return dicAttribValues

	End Function
	Private Function zzGetGushRemarkData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim sValue As String
		Dim oItemData As DMCommon.ItemData
		Dim oHebText As DMCommon.HebrewTrans

		If Me.cmbMeasureMethod.SelectedItem IsNot Nothing Then
			oItemData = TryCast(Me.cmbMeasureMethod.SelectedItem, DMCommon.ItemData)
			sValue = oItemData.ListDispData
		Else
			sValue = String.Empty
		End If
		If sValue IsNot Nothing Then
			oHebText = New DMCommon.HebrewTrans(sValue, True)

			dicAttribValues.Add(msEngSurveyMethodAttribTag, oHebText.GetDOSDest())

		End If


		If Me.cmbBoundaryMeasureMethod.SelectedItem IsNot Nothing Then
			oItemData = TryCast(Me.cmbBoundaryMeasureMethod.SelectedItem, DMCommon.ItemData)
			sValue = oItemData.ListDispData
		Else
			sValue = String.Empty
		End If
		If sValue IsNot Nothing Then
			oHebText = New DMCommon.HebrewTrans(sValue, True)
			dicAttribValues.Add(msPT_TransferMethodAttribTag, oHebText.GetDOSDest())

		End If



		If Me.cmbBoundariesKind.Text IsNot Nothing Then
			sValue = Me.cmbBoundariesKind.Text
		Else
			sValue = String.Empty
		End If
		oHebText = New DMCommon.HebrewTrans(sValue, True)
		dicAttribValues.Add(msMarkDescrAttribTag, oHebText.GetDOSDest())
		oHebText = New DMCommon.HebrewTrans(Me.txtMoleNodules.Text, True)
		dicAttribValues.Add(msGushShumaAttribTag, oHebText.GetDOSDest())
		oHebText = New DMCommon.HebrewTrans(Me.txtRegistrationProgramNos.Text, True)

		dicAttribValues.Add(msTazarInGushAttribTag, oHebText.GetDOSDest()) 'DMCommon.Hebrew.WordToDOSInv(
		oHebText = New DMCommon.HebrewTrans(zzGetBasePlanNameList(), True)
		dicAttribValues.Add(msPlanNumInGushAttribTag, oHebText.GetDOSDest()) 'DMCommon.Hebrew.WordToDOSInv(

		Return dicAttribValues
		'	PLAN_NUM_IN_GUSH	TZR_IN_GUSH_HESDER	GUSH_SHUMA_HESDER	MARK_DESC	PT_TRANSFER_METHOD	ENG_SURVEY_METHOD
	End Function
	Private Function zzGetBasePlanNameList() As String
		Dim oDataRowView As DataRowView
		Dim sRes As String = Nothing
		Dim sValue As String

		For iIndex As Integer = 0 To moBasePlansView.Count - 1
			oDataRowView = moBasePlansView.Item(iIndex)
			sValue = DMCommon.Functions.CStrN(oDataRowView.Item("PlanNum"))
			If String.IsNullOrEmpty(sValue) Then
				sRes = sValue
			Else
				sRes &= "," & sValue
			End If

		Next
		Return sRes

	End Function

	Private Function zzGetMAPIDateData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		If Not String.IsNullOrEmpty(Me.txtMAPIDate.Text) Then
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(Me.txtMAPIDate.Text, True)

			DMCommon.Debug.MsgBox("19_040", Me.txtMAPIDate.Text, oHebText.GetDOSDestInv(), oHebText.GetDOSDest())
			dicAttribValues.Add(msMAPIDateAttribTag, oHebText.GetDOSDest())

		End If

		'dicAttribValues.Add(msMAPIDateAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtMAPIDate.Text, False))


		Return dicAttribValues
	End Function
	Private Function zzGetTitleData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		dicAttribValues.Add(msMapStatusAttribTag, DMCommon.Hebrew.WordToDOS(zzGetMapStatusName(), False))
		dicAttribValues.Add(msSheetNoAttribTag, "1")

		Return dicAttribValues
	End Function
	Private Function zzGetMapStatusName() As String
		If Me.rdbMapStatusEarly.Checked Then
			Return Me.rdbMapStatusEarly.Text
		ElseIf Me.rdbMapStatusTemp.Checked Then
			Return Me.rdbMapStatusTemp.Text
		ElseIf Me.rdbMapStatusFinal.Checked Then
			Return Me.rdbMapStatusFinal.Text
		Else
			Return String.Empty
		End If
	End Function
	Private Function zzGetMapStatusLayerDef() As DMAcadExt.AcadLayerDef
		Dim oAcadLayerDef As DMAcadExt.AcadLayerDef
		Dim iLayerFunction As DMAcadExt.enLayerFunction
		If Me.rdbMapStatusEarly.Checked Then
			iLayerFunction = DMAcadExt.enLayerFunction.TitleBlockEarly
		ElseIf Me.rdbMapStatusTemp.Checked Then
			iLayerFunction = DMAcadExt.enLayerFunction.TitleBlockTemp
		ElseIf Me.rdbMapStatusFinal.Checked Then
			iLayerFunction = DMAcadExt.enLayerFunction.TitleBlockFinal
		Else
			iLayerFunction = DMAcadExt.enLayerFunction.Default
		End If
		oAcadLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, iLayerFunction)
		Return oAcadLayerDef

	End Function


	Private Function zzGetStampData(bUpdate As Boolean) As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim oDynValue As System.Object


		If dtpSurveyDate1.Checked Then
			dicAttribValues.Add(msSurveyDate1AttribTag, dtpSurveyDate1.Text)
		ElseIf bUpdate Then
			dicAttribValues.Add(msSurveyDate1AttribTag, String.Empty)

		End If
		If dtpSurveyDate2.Checked Then
			dicAttribValues.Add(msSurveyDate2AttribTag, dtpSurveyDate2.Text)
		ElseIf bUpdate Then
			dicAttribValues.Add(msSurveyDate2AttribTag, String.Empty)
		End If

		If dtpEndDate.Checked Then
			dicAttribValues.Add(msEndDateAttribTag, Me.dtpEndDate.Text)
		ElseIf bUpdate Then
			dicAttribValues.Add(msEndDateAttribTag, String.Empty)
		End If


		If Me.dtpSurveyDate.Checked Then
			dicAttribValues.Add(msUpdateDateAttribTag, Me.dtpSurveyDate.Text)
		ElseIf bUpdate Then
			dicAttribValues.Add(msUpdateDateAttribTag, String.Empty)
		End If
		If False Then
			If cmbLocality.SelectedIndex <> -1 Then
				dicAttribValues.Add(msPlaceAttribTag, DMCommon.Hebrew.WordToDOS(zzGetLocalityName(), True))

			ElseIf bUpdate Then

				dicAttribValues.Add(msPlaceAttribTag, String.Empty)
			End If
		Else
			dicAttribValues.Add(msPlaceAttribTag, DMCommon.Hebrew.WordToDOS(msPlanningPlace, True))
		End If


		If cmbSurveyor.SelectedIndex <> -1 Then
			dicAttribValues.Add(msSurveyorNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbSurveyor.Text, True))
			oDynValue = cmbSurveyor.SelectedValue
			dicAttribValues.Add(msSurveyorIDAttribTag, oDynValue.ToString())

		End If



		'	DMCommon.Debug.MsgBox("13_039", dicAttribValues.Count, cmbLocality.SelectedText, cmbLocality.SelectedItem)




		Return dicAttribValues

	End Function
	Private Function zzGetLocalityName() As String
		If cmbLocality.SelectedItem IsNot Nothing Then
			Dim oItemData As DMCommon.ItemData = TryCast(cmbLocality.SelectedItem, DMCommon.ItemData)
			If oItemData IsNot Nothing Then
				Return oItemData.ListDispData
			Else
				Return Nothing
			End If
		Else
			Return Nothing

		End If

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
			Return "IG05"
		ElseIf Me.rdbNet2012.Checked Then
			Return "IG05/12"
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

	Private Sub zzOKGeneralNew()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		DMCommon.Debug.MsgBox("15_002b", moFrameInsPoint IsNot Nothing, moGenAcadBlock.ReferenceCount, moGenAcadBlock.Folder)
		moGenAcadBlock.LoadAllReferences()
		DMCommon.Debug.MsgBox("15_002a", moGenAcadBlock.ReferenceCount, moGenAcadBlock.Folder)
		Do


			If moGenAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGenAcadBlock.UpdateAttribData(0, zzGetGenData(True))
				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGenAcadBlock.LoadAllReferences()
				End If
			ElseIf moGenAcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				moGenAcadBlock.OpenForRight()
				If moGenAcadBlock.DefinitionExists Then
					'Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()
					tBlockRefData.AtribValuesDic = zzGetGenData(False)
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
		Dim sEllipseLayer As String
		Dim oOtherEllipseAcadBlock As DMAcadExt.AcadBlock

		Select Case sPlanType
			Case "1"
				Return
			Case "2"
				oEllipseAcadBlock = moEllipseAcadBlock
				sEllipseLayer = msEllipseBlockLayer
				oOtherEllipseAcadBlock = moEllipseTTGAcadBlock
			Case "9"
				oEllipseAcadBlock = moEllipseTTGAcadBlock
				sEllipseLayer = msEllipseTTGBlockLayer
				oOtherEllipseAcadBlock = moEllipseAcadBlock
			Case "12"
				oEllipseAcadBlock = moEllipseTTGAcadBlock
				sEllipseLayer = msEllipseTTGBlockLayer
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
					tBlockRefData.Layer = sEllipseLayer
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
	Private Sub zzOKStampNew()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		Dim tStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d


		moStampAcadBlock.LoadAllReferences()




		'	DMCommon.Debug.MsgBox("13_037", moStampAcadBlock.ReferenceCount, moStampAcadBlock.BlockName)

		Do
			If moStampAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moStampAcadBlock.UpdateAttribData(0, zzGetStampData(True))

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
					tBlockRefData.AtribValuesDic = zzGetStampData(False)
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
	Private Sub zzOKPageNo()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'	Dim tPageNoInsPoint As Autodesk.AutoCAD.Geometry.Point3d


		moPageNoAcadBlock.LoadAllReferences()




		'	DMCommon.Debug.MsgBox("13_037", moStampAcadBlock.ReferenceCount, moStampAcadBlock.BlockName)

		Do
			If moPageNoAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moPageNoAcadBlock.UpdateAttribData(0, zzGetPageNoData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moPageNoAcadBlock.LoadAllReferences()
				End If



			ElseIf moPageNoAcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
				'insert
				'	tPageNoInsPoint = moFrameInsPoint.AcGePoint3d



				'- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY

				moPageNoAcadBlock.OpenForRight()
				If moPageNoAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()
					tBlockRefData.AtribValuesDic = zzGetPageNoData()
					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moPageNoAcadBlock.InsertRefNew(tBlockRefData)
				End If


				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Sub zzOKCaption()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'Dim tCaptionInsPoint
		DMCommon.Debug.MsgBox("15_219")
		moCaptionAcadBlock.LoadAllReferences()

		Do
			If moCaptionAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moCaptionAcadBlock.UpdateAttribData(0, zzGetCaptionData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moCaptionAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moCaptionAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then

				'insert
				DMCommon.Debug.MsgBox("15_220", moCaptionAcadBlock.ReferenceCount)
				'	Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.96 * mdCurrentFrameHeight)
				moCaptionAcadBlock.OpenForRight()
				If moCaptionAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetCaptionPoint()
					tBlockRefData.AtribValuesDic = zzGetCaptionData()
					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor * 3.4)

					DMCommon.Debug.MsgBox("15_221", moCaptionAcadBlock.InsertRefNew(tBlockRefData))
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Sub zzOKGushProp()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'Dim tCaptionInsPoint
		moGushPropAcadBlock.LoadAllReferences()

		Do
			If moGushPropAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGushPropAcadBlock.UpdateAttribData(0, zzGetGushPropData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGushPropAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moGushPropAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				'	Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.96 * mdCurrentFrameHeight)
				moGushPropAcadBlock.OpenForRight()
				If moGushPropAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()
					tBlockRefData.AtribValuesDic = zzGetGushPropData()
					If DMAcadExt.AcadTransaction.CreateLayer(msGushPropBlockLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False) Then
						tBlockRefData.Layer = msGushPropBlockLayer
					End If

					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moGushPropAcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub
	Private Sub zzOKGenType1()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'Dim tCaptionInsPoint
		moGenType1AcadBlock.LoadAllReferences()

		Do
			If moGenType1AcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGenType1AcadBlock.UpdateAttribData(0, zzGetGenType1Data(True))

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGenType1AcadBlock.LoadAllReferences()
				End If

			ElseIf Not moGenType1AcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				'	Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.96 * mdCurrentFrameHeight)
				moGenType1AcadBlock.OpenForRight()
				If moGenType1AcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetCenterTopPoint()
					tBlockRefData.AtribValuesDic = zzGetGenType1Data(False)


					If DMAcadExt.AcadTransaction.CreateLayer(msGenType1BlockLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False) Then
						tBlockRefData.Layer = msGenType1BlockLayer
					End If

					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moGenType1AcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Sub zzOKLegend()


		moLegendAcadBlock.LoadAllReferences()


		If moLegendAcadBlock.ReferenceCount >= -1 Then
			If moFrameInsPoint IsNot Nothing Then
				Dim oAcadLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Legend)

				moLegendAcadBlock.OpenForRight()
				If moLegendAcadBlock.DefinitionExists Then

					Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightDownPoint()
					If DMAcadExt.AcadTransaction.CreateLayer(oAcadLayerDef, False) Then
						tBlockRefData.Layer = oAcadLayerDef.Name
					End If

					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moLegendAcadBlock.InsertRefNew(tBlockRefData)
				End If
			End If
		End If


	End Sub


	Private Sub zzOKGushRemark()
		Dim iRes As DMAcadExt.AcadBlock.enResults

		moGushRemarkAcadBlock.LoadAllReferences()

		Do
			If moGushRemarkAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGushRemarkAcadBlock.UpdateAttribData(0, zzGetGushRemarkData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGushRemarkAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moGushRemarkAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert


				moGushRemarkAcadBlock.OpenForRight()
				If moGushRemarkAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightDownPoint()
					tBlockRefData.AtribValuesDic = zzGetGushRemarkData()
					If DMAcadExt.AcadTransaction.CreateLayer(msGushRemarkBlockLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False) Then
						tBlockRefData.Layer = msGushRemarkBlockLayer
					End If


					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moGushRemarkAcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub



	Private Sub zzOKGenType1c()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		'Dim tCaptionInsPoint
		moGenType1AcadBlock.LoadAllReferences()

		Do
			If moGenType1AcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moGenType1AcadBlock.UpdateAttribData(0, zzGetGenType1Data(True))

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moGenType1AcadBlock.LoadAllReferences()
				End If

			ElseIf moGenType1AcadBlock.ReferenceCount = 0 AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				'	Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.96 * mdCurrentFrameHeight)
				moGenType1AcadBlock.OpenForRight()
				If moGenType1AcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetCenterTopPoint()
					tBlockRefData.AtribValuesDic = zzGetGenType1Data(False)
					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moGenType1AcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub





	Private Sub zzOKScaleText()
		Dim iRes As DMAcadExt.AcadBlock.enResults

		moScaleTextAcadBlock.LoadAllReferences()

		Do
			If moScaleTextAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moScaleTextAcadBlock.UpdateAttribData(0, zzGetScaleTextData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moCaptionAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moScaleTextAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				'	Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.96 * mdCurrentFrameHeight)
				moScaleTextAcadBlock.OpenForRight()
				If moScaleTextAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetLeftUpPoint()
					tBlockRefData.AtribValuesDic = zzGetScaleTextData()

					If DMAcadExt.AcadTransaction.CreateLayer(msScaleTextBlockLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False) Then
						tBlockRefData.Layer = msScaleTextBlockLayer
					End If


					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moScaleTextAcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Sub zzOK_MAPI()
		Dim iRes As DMAcadExt.AcadBlock.enResults

		moMAPIAcadBlock.LoadAllReferences()

		Do
			If moMAPIAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moMAPIAcadBlock.UpdateAttribData(0, zzGetMAPIDateData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moMAPIAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moMAPIAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				moMAPIAcadBlock.OpenForRight()
				If moMAPIAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = moFrameInsPoint.AcGePoint3d
					tBlockRefData.AtribValuesDic = zzGetMAPIDateData()
					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moMAPIAcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	'moTitleAcadBlock = New DMAcadExt.AcadBlock(msTitleTmpBlockName, msBlockPath)
	'moTitleAcadBlock.Fields = New String() {msMapStatusAttribTag, msSheetNoAttribTag}
	Private Sub zzOK_Title()
		Dim iRes As DMAcadExt.AcadBlock.enResults

		moTitleAcadBlock.LoadAllReferences()

		Do
			If moTitleAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moTitleAcadBlock.UpdateAttribData(0, zzGetTitleData())

				If iRes = DMAcadExt.AcadBlock.enResults.BlockRefWasNotFound Then
					moTitleAcadBlock.LoadAllReferences()
				End If

			ElseIf Not moTitleAcadBlock.HasReferences AndAlso moFrameInsPoint IsNot Nothing Then
				'insert

				moTitleAcadBlock.OpenForRight()
				If moTitleAcadBlock.DefinitionExists Then
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()
					tBlockRefData.AtribValuesDic = zzGetTitleData()
					Dim oAcadLayerDef As DMAcadExt.AcadLayerDef = zzGetMapStatusLayerDef()
					If DMAcadExt.AcadTransaction.CreateLayer(oAcadLayerDef, False) Then
						tBlockRefData.Layer = oAcadLayerDef.Name
					End If
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moTitleAcadBlock.InsertRefNew(tBlockRefData)
				End If

				iRes = DMAcadExt.AcadBlock.enResults.Success
			Else
				' Error
				iRes = DMAcadExt.AcadBlock.enResults.Success
			End If
		Loop Until iRes = DMAcadExt.AcadBlock.enResults.Success
	End Sub

	Private Function zzGetRightUpPoint() As Autodesk.AutoCAD.Geometry.Point3d
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)
		Return oBlockInsPoint.AcGePoint3d
	End Function
	Private Function zzGetRightDownPoint() As Autodesk.AutoCAD.Geometry.Point3d
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, 0.0)
		Return oBlockInsPoint.AcGePoint3d
	End Function

	Private Function zzGetLeftUpPoint() As Autodesk.AutoCAD.Geometry.Point3d
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.0, mdCurrentFrameHeight)
		Return oBlockInsPoint.AcGePoint3d
	End Function
	Private Function zzGetCaptionPoint() As Autodesk.AutoCAD.Geometry.Point3d
		If moFrameInsPoint IsNot Nothing Then
			Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.968 * mdCurrentFrameHeight)
			Return oBlockInsPoint.AcGePoint3d
		Else
			Return New Autodesk.AutoCAD.Geometry.Point3d()
		End If


	End Function
	Private Function zzGetCenterTopPoint() As Autodesk.AutoCAD.Geometry.Point3d
		Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, mdCurrentFrameHeight)
		Return oBlockInsPoint.AcGePoint3d
	End Function

	Private Sub zzOKStamp()
		Dim iRes As DMAcadExt.AcadBlock.enResults
		Dim tStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d

		DMCommon.Debug.MsgBox("13_037", moStampAcadBlock.ReferenceCount, moStampAcadBlock.BlockName)
		Do
			If moStampAcadBlock.ReferenceCount = 1 Then
				'update
				iRes = moStampAcadBlock.UpdateAttribData(0, zzGetStampData(True))

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
					tBlockRefData.AtribValuesDic = zzGetStampData(False)
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
						tBlockRefData.AtribValuesDic = zzGetStampData(False)
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
	Private Sub zzOKDataMap()
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
						tBlockRefData.AtribValuesDic = zzGetStampData(False)
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
	Private Sub zzOKType1Frame()
		moType1FrameAcadBlock.LoadAllReferences()

		If moType1FrameAcadBlock.ReferenceCount <= 0 Then

			If moFrameInsPoint IsNot Nothing Then
				moType1FrameAcadBlock.OpenForRight()
				If moType1FrameAcadBlock.DefinitionExists Then

					'   Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(mdCurrentFrameWidth, mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY

					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = moFrameInsPoint.AcGePoint3d

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moType1FrameAcadBlock.InsertRefNew(tBlockRefData)

				End If
			End If
		End If

	End Sub
	Private Sub zzOKType1DirArrow()
		moType1DirArrowAcadBlock.LoadAllReferences()

		If moType1DirArrowAcadBlock.ReferenceCount >= -1 Then

			If moFrameInsPoint IsNot Nothing Then
				moType1DirArrowAcadBlock.OpenForRight()
				If moType1DirArrowAcadBlock.DefinitionExists Then



					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moType1DirArrowAcadBlock.InsertRefNew(tBlockRefData)

				End If
			End If
		End If

	End Sub
	Private Sub zzOKScale1()
		moScaleAcadBlock.LoadAllReferences()

		If moType1DirArrowAcadBlock.ReferenceCount > -1 Then

			If moFrameInsPoint IsNot Nothing Then
				moType1DirArrowAcadBlock.OpenForRight()
				If moType1DirArrowAcadBlock.DefinitionExists Then



					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightUpPoint()

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moType1DirArrowAcadBlock.InsertRefNew(tBlockRefData)

				End If
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
		If False Then

			If moScaleAcadBlock IsNot Nothing Then
				moScaleAcadBlock.LoadAllReferences()
				moScaleAcadBlock.DeleteAll()
			End If
		End If


		moScaleAcadBlock = New DMAcadExt.AcadBlock(zzGetScaleBlockName(), msBlockPath)
		moScaleAcadBlock.LoadAllReferences()
		'	DMCommon.Debug.MsgBox("19_011", moScaleAcadBlock IsNot Nothing, zzGetScaleBlockName(), moScaleAcadBlock.ReferenceCount)
		If moScaleAcadBlock.ReferenceCount > 0 Then
			If moFrameInsPoint IsNot Nothing Then
				moScaleAcadBlock.OpenForRight()
				If moScaleAcadBlock.DefinitionExists Then

					Dim oBlockInsPoint As DMAcadExt.TPlnPoint = moFrameInsPoint.GetMoved(0.5 * mdCurrentFrameWidth, 0.075 * mdCurrentFrameHeight)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = oBlockInsPoint.AcGePoint3d

					tBlockRefData.Layer = msTazarMapBlockLayer
					'	tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moScaleAcadBlock.InsertRefNew(tBlockRefData)
				End If
			End If
		End If

	End Sub

	Private Sub zzOKUpdateTable()

		If False Then
			If moUpdateTableAcadBlock IsNot Nothing Then
				DMCommon.Debug.MsgBox("19_002", moUpdateTableAcadBlock.BlockRefsCount)
				moUpdateTableAcadBlock.LoadAllReferences()
				moUpdateTableAcadBlock.DeleteAll()
				DMCommon.Debug.MsgBox("19_003", moUpdateTableAcadBlock.BlockRefsCount)
			End If

		End If


		'	moUpdateTableAcadBlock = New DMAcadExt.AcadBlock(msUpdateTableBlockName, msBlockPath)

		moUpdateTableAcadBlock.LoadAllReferences()

		If moUpdateTableAcadBlock.ReferenceCount >= -1 Then
			If moFrameInsPoint IsNot Nothing Then

				moUpdateTableAcadBlock.OpenForRight()
				If moUpdateTableAcadBlock.DefinitionExists Then

					Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
					tBlockRefData.Position = zzGetRightDownPoint()

					tBlockRefData.Layer = msTazarMapBlockLayer
					tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
					moUpdateTableAcadBlock.InsertRefNew(tBlockRefData)
				End If
			End If
		End If


	End Sub


	Private Sub zzOKCorners()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzChangeCornerLayers)

		DMAcadExt.AcadTransaction.ProcAll(dlEntityProc, True)

	End Sub
	Private Sub zzInsertLine(tInsertPoint As DMAcadExt.TPlnPoint, iAxis As DMAcadExt.TPlnPoint.enAxis, dLength As Double, Optional dShift As Double = 0.0)
		If moFrameInsPoint IsNot Nothing Then
			Dim oLine As Line
			If iAxis = DMAcadExt.TPlnPoint.enAxis.X Then
				oLine = New Line(tInsertPoint.AcGePoint3d, tInsertPoint.GetMoved(0.0, dLength).AcGePoint3d)
			Else
				oLine = New Line(tInsertPoint.AcGePoint3d, tInsertPoint.GetMoved(dLength, 0.0).AcGePoint3d)
			End If
			oLine.Layer = msTazarMapBlockLayer
			DMAcadExt.AcadTransaction.AppendEntity(oLine)
		End If

	End Sub
	Private Sub zzInsertCoordinate(tInsertPoint As DMAcadExt.TPlnPoint, iAxis As DMAcadExt.TPlnPoint.enAxis, Optional dShift As Double = 0.0)

		If moFrameInsPoint IsNot Nothing Then
			moCoordinateAcadBlock.OpenForRight()
			If moType1FrameAcadBlock.DefinitionExists Then

				Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
				Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
				Dim sTopValue As String = Nothing
				Dim sBottomValue As String = Nothing

				zzGetCoordinateValue(tInsertPoint, iAxis, sTopValue, sBottomValue)

				dicAttribValues.Add(msCoordinateTopPartAttribTag, sTopValue)
				dicAttribValues.Add(msCoordinateBottomPartAttribTag, sBottomValue)  '  DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, 
				If dShift <> 0.0 Then
					If iAxis = DMAcadExt.TPlnPoint.enAxis.X Then
						tInsertPoint.Move(0.0, dShift)
					Else
						tInsertPoint.Move(dShift, 0.0)
					End If

				End If
				tBlockRefData.Position = tInsertPoint.AcGePoint3d

				tBlockRefData.Layer = msTazarMapBlockLayer
				tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
				If iAxis = DMAcadExt.TPlnPoint.enAxis.X Then
					tBlockRefData.Rotation = Math.PI * 1.5
				End If
				tBlockRefData.AtribValuesDic = dicAttribValues
				moCoordinateAcadBlock.InsertRefNew(tBlockRefData)

			End If
		End If


	End Sub
	Private Sub zzGetCoordinateValue(oInsertPoint As DMAcadExt.TPlnPoint, iAxis As DMAcadExt.TPlnPoint.enAxis, ByRef sTopValue As String, ByRef sBottomValue As String)
		Dim iValue As Integer = Convert.ToInt32(oInsertPoint.Value(iAxis))
		Dim iBottomValue As Integer = iValue Mod (1000)
		Dim iTopValue As Integer = iValue \ 1000
		sBottomValue = String.Format("{0}00", iBottomValue)
		sBottomValue = sBottomValue.Substring(0, 3)

		sTopValue = FormatNumber(iTopValue, 0)

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

	Private Sub zzLoadNotesRefData()
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)

			moaNoteChecks(iIndex).LoadBlockRef()
		Next
	End Sub

	Private Sub chkAllAttribVisible_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkAllAttribVisible.CheckedChanged

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



	Private Sub cmdLoadBlockData_Click(sender As System.Object, e As System.EventArgs)
		'  zzLoadBlockRefData(True)
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
		Private mbByPlan As Boolean = False



		Private msaAttribTag() As String

		Private mdNoteRowHeight As Double = 5.6

		Private mtGenBlockRecObjID As ObjectId
		Private mtGenBlockRefObjID As ObjectId
		Private Shared moProjectBindingSource As BindingSource
		Private Shared moPlanBindingSource As BindingSource

		Public Shared Sub SetParam(dLeft As Double, dTextHeight As Double)
			'	zzInitTextSile(msFontStyle)
			mdLeft = dLeft
			mdTextHeight = dTextHeight
		End Sub
		Public Shared Sub SetBindingSource(ByRef oProjectBindingSource As BindingSource, ByRef oPlanBindingSource As BindingSource)
			moProjectBindingSource = oProjectBindingSource
			moPlanBindingSource = oPlanBindingSource
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
		Public ReadOnly Property CheckControl As CheckBox
			Get
				Return moCheckControl
			End Get
		End Property
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
		Public Sub SetBinding()

			Dim oBindingSource As BindingSource
			If mbByPlan Then
				oBindingSource = moPlanBindingSource
			Else
				oBindingSource = moProjectBindingSource
			End If
			moCheckControl.DataBindings.Add(New Binding("Checked", oBindingSource, FieldName))
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
				Return (moCheckControl.Height - 4) \ 14

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
		Public ReadOnly Property FieldName As String
			Get
				Return "Note" & Format(miControlIndex + 1, "00")

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
		Public Property ByPlan As Boolean
			Get
				Return mbByPlan

			End Get
			Set(bValue As Boolean)
				mbByPlan = bValue
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
		If False Then
			For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
				moaNoteChecks(iIndex).Checked = True
			Next
		End If
		zzSetCheckAll(True)










	End Sub
	Private Sub zzSetCheckAll(bChecked As Boolean)
		Dim oProjectRow As DataRowView = zzGetProjectDataRow()
		Dim oPlanRow As DataRowView = zzGetPlanDataRow()

		Dim oBS As BindingSource
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Checked = bChecked

			If moaNoteChecks(iIndex).ByPlan Then
				oPlanRow.Item(moaNoteChecks(iIndex).FieldName) = bChecked
				moaNoteChecks(iIndex).Checked = bChecked
			Else
				oProjectRow.Item(moaNoteChecks(iIndex).FieldName) = bChecked
			End If
			oBS = TryCast(moaNoteChecks(iIndex).CheckControl.DataBindings.Item(0).DataSource, BindingSource)

		Next
	End Sub

	Private Sub cmdCheckClear_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckClear.Click
		If False Then
			For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
				moaNoteChecks(iIndex).Checked = False
			Next
		End If

		zzSetCheckAll(False)
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

		Dim oBS As BindingSource

		Dim saValue0() As String = {String.Empty, zzToFullBlockName(Me.mskOriginalBlockNo.Text, Me.mskOriginalBlockAddNo.Text)}
		moaNoteChecks(0).AddVarData(saValue0)



		Dim saValue1() As String = {String.Empty, zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(zzGetFullName(Me.mskBaseProcessNo.Text, Me.mskBaseProcessYear.Text)), zzToFullBlockName(Me.mskBaseBlockNo.Text, Me.mskBaseBlockAddNo.Text), zzToFullBlockName(Me.mskBaseBlockNo.Text, Me.mskBaseBlockAddNo.Text)}
		moaNoteChecks(1).AddVarData(saValue1)


		Dim saValue2() As String = {String.Empty, zzToFullBlockName(Me.mskOriginalBlockNo.Text, Me.mskOriginalBlockAddNo.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(zzGetFullName(Me.mskBaseProcessNo.Text, Me.mskBaseProcessYear.Text)), zzToFullBlockName(Me.mskBaseBlockNo.Text, Me.mskBaseBlockAddNo.Text)}
		moaNoteChecks(2).AddVarData(saValue2)
		' MessageBox.Show(moaNoteChecks(2).AttributeCount.ToString(), "07_163X")
		Dim saValue3() As String = {String.Empty, zzTextToNothing(zzFilterList(Me.txtNormalParcels.Text)), zzTextToNothing(Me.mskOriginalBlockNo.Text)}
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


		Dim saValue16() As String = {String.Empty, zzTextToNothing(Me.txtTaskNo.Text,, True)}
		moaNoteChecks(16).AddVarData(saValue16)

		oBS = TryCast(Me.txtProjectNo.DataBindings.Item(0).DataSource, BindingSource)


		'DMCommon.Debug.MsgBox("13_038", Me.txtProjectNo.DataBindings.Item(0).PropertyName, Me.txtProjectNo.DataBindings.Item(0).DataSource, Me.txtProjectNo.DataBindings.Item(0).DataSource.GetType(), oBS.Item(0), oBS.Item(0).GetType(), oBS.DataSource, oBS.DataSource.GetType(), oBS.DataMember, "END")
		Dim iProjectNo As Integer = DMCommon.Functions.CIntN(zzGetBindingSourceVal(Me.txtProjectNo))

		Dim saValue17() As String = {String.Empty, zzTextToNothing(Me.txtProjectNo.Text,, True)}
		moaNoteChecks(17).AddVarData(saValue17)



	End Sub
	Private Function zzGetBindingSourceVal(oControl As Control) As System.Object
		Dim oControlBindingsCollection As ControlBindingsCollection = oControl.DataBindings
		If oControlBindingsCollection IsNot Nothing Then
			Dim oBindings As Binding = oControlBindingsCollection.Item(0)
			If oBindings IsNot Nothing Then
				Dim oBindingMemberInfo As BindingMemberInfo = oBindings.BindingMemberInfo
				Dim oDataSource As System.Object = oBindings.DataSource
				Dim oBindingSource As BindingSource = TryCast(oDataSource, BindingSource)
				If oBindingSource IsNot Nothing Then
					Dim oRow As DataRowView = TryCast(oBindingSource.Current, DataRowView)
					Dim sField As String = oBindingMemberInfo.BindingField

					Dim oRes As System.Object = oRow.Item(sField)

					Return oRes
				End If
			End If
		End If
		Return Nothing

	End Function
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
			'	Me.txtBaseProcessName.Text = sValue
		End If

		sValue = moaNoteChecks(1).Item(4)
		If sValue IsNot Nothing Then
			'	Me.txtBaseGush.Text = sValue
		End If
		'   Dim saValue2() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}

		sValue = moaNoteChecks(2).Item(1)
		If sValue IsNot Nothing Then
			Me.mskOriginalBlockNo.Text = sValue
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
			'	Me.txtBaseProcessName.Text = sValue
		End If

		sValue = moaNoteChecks(2).Item(5)
		If sValue IsNot Nothing Then
			'	Me.txtBaseGush.Text = sValue
		End If

		'  Dim saValue3() As String = {String.Empty, zzTextToNothing(zzFilterList(Me.txtParcels.Text)), zzTextToNothing(Me.txtGush.Text)}

		'sValue = moaNoteChecks(3).Item(1)
		'If sValue IsNot Nothing Then
		'   Me.txtParcels.Text = sValue
		'End If

		sValue = moaNoteChecks(3).Item(2)
		If sValue IsNot Nothing Then
			Me.mskOriginalBlockNo.Text = sValue
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
	Private Function zzToFullBlockName(sBlockNo As String, sBlockAddNo As String) As String
		Dim iBlockNo As Integer
		Dim iBlockAddNo As Integer

		If Integer.TryParse(sBlockNo, iBlockNo) AndAlso iBlockNo > 0 Then
			Integer.TryParse(sBlockAddNo, iBlockAddNo)

			Return TopoManager.TPlanGraph.TplnBlock.GetBlockName(iBlockNo, iBlockAddNo)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzGetFullName(sNameA As String, sNameB As String) As String
		Return Strings.Trim(sNameA) & "/" & Strings.Trim(sNameB)
	End Function
	Private Function zzTextToNothing(sText As String, Optional bHeb As Boolean = False, Optional bStopZero As Boolean = False) As String
		If String.IsNullOrEmpty(sText) Then
			Return Nothing
		ElseIf bHeb Then
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText, True)

			Return oHebText.GetDOSDest


		ElseIf bStopZero Then
			sText = sText.Trim()
			If sText = "0" Then
				Return Nothing
			Else
				Return sText
			End If
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
	Private Sub zzFillEditors()
		Dim sComText As String = "SELECT EmplID, Empl_NameFamily FROM ShopData.dbo.Empl_Tazar"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		Dim iEmplID As Integer
		Dim sEmplName As String
		Dim oList As System.Collections.Generic.IList(Of DMCommon.ItemData) = New System.Collections.Generic.List(Of DMCommon.ItemData)
		Dim oItemData As DMCommon.ItemData = DirectCast(Me.cmbCommittee.SelectedItem, DMCommon.ItemData)

		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				iEmplID = oDataReader.GetInt32(0)
				sEmplName = oDataReader.GetString(1)
				oItemData = New DMCommon.ItemData(iEmplID, sEmplName)
				oList.Add(oItemData)
			Loop
			Me.cmbEmplEditors.DataSource = oList

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
		Me.Visible = True
	End Sub


	Private Sub cmdParcelInvert_Click_1(oSender As System.Object, e As EventArgs) Handles cmdParcelInvert.Click
		Me.txtNormalParcels.Text = DMCommon.Hebrew.InvertList(Me.txtNormalParcels.Text)
	End Sub

	Private Sub cmdDrawFrame_Click(oSender As System.Object, e As EventArgs) Handles cmdDrawFrameVert.Click
		AcadReport.RepApp.InitDWGScaleFactor()
		zzGetPlanType()

		Select Case miPlanType
			Case 1
				zzMyDrawFrame(False)
			Case 2, 9, 12
				zzDrawFrameByLisp()
		End Select


	End Sub
	Private Sub zzDrawFrameByLisp()
		Dim oAfterCom As DMAcadExt.AcadDocument.AfterCommandProc = New DMAcadExt.AcadDocument.AfterCommandProc(AddressOf zzAfterAcad)
		DMAcadExt.AcadDocument.SetOneTimeActiveDoc("UNDO", 2, oAfterCom)
		Me.Visible = False
		DMAcadExt.AcadDocument.SendExec(msFrameComLine, True)
	End Sub
	Private Sub zzMyDrawFrame(bLandscape As Boolean)
		Dim dPageWidth As Double '= 480.0
		Dim dPageHeight As Double '= 640.0
		Dim dNumberPerSize As Double = 8.0
		Dim dFrameWidth As Double = 10.0
		Dim dLineLength As Double = 5.0
		Dim sFrameBlockName As String
		If bLandscape Then
			dPageWidth = mdLandscapePageWidth
			dPageHeight = mdLandscapePageHeight
			sFrameBlockName = msFrameBlockNameType1_Hor
		Else
			dPageWidth = mdLandscapePageHeight
			dPageHeight = mdLandscapePageWidth
			sFrameBlockName = msFrameBlockNameType1_Ver
		End If

		Dim dUnit As Double = zzGetUnit(mdLandscapePageWidth, dNumberPerSize)

		moType1FrameAcadBlock = New DMAcadExt.AcadBlock(sFrameBlockName, msBlockPath)


		mdCurrentFrameHeight = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor


		mdCurrentFrameWidth = dUnit * Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageWidth / dUnit, 0)
		mdCurrentFrameHeight = dUnit * Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageHeight / dUnit, 0)

		mdCurrentFrameWidth = Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageWidth, 0)
		mdCurrentFrameHeight = Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageHeight, 0)
		dFrameWidth *= AcadReport.RepApp.DrawingScaleFactor
		dLineLength *= AcadReport.RepApp.DrawingScaleFactor
		Dim iRequiredSnapMode As Integer = 1

		Dim tRequiredSnapUnitPoint As Autodesk.AutoCAD.Geometry.Point2d = New Autodesk.AutoCAD.Geometry.Point2d(dUnit, dUnit)
		Dim dDeltaX As Double
		Dim dDeltaY As Double

		Dim dDeltaXStart As Double
		Dim dDeltaYEnd As Double

		Dim iSnapMode As System.Int32 = CInt(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName))
		Dim tSnapUnitPoint As Autodesk.AutoCAD.Geometry.Point2d = DirectCast(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName), Autodesk.AutoCAD.Geometry.Point2d)
		If iSnapMode <> iRequiredSnapMode Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName, iRequiredSnapMode)
		End If

		If tSnapUnitPoint <> tRequiredSnapUnitPoint Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName, tRequiredSnapUnitPoint)
		End If
		Me.Visible = False

		Dim oJig As DMAcadExt.PointJig = New DMAcadExt.FrameJig(mdCurrentFrameWidth, mdCurrentFrameHeight, dFrameWidth)

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptResult As Autodesk.AutoCAD.EditorInput.PromptResult = oEditor.Drag(oJig)

		'DMCommon.Debug.MsgBox("mdCurrentFrameWidth, mdCurrentFrameHeight", mdCurrentFrameWidth, mdCurrentFrameHeight)

		If oPromptResult.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				moFrameInsPoint = New DMAcadExt.TPlnPoint(oJig.GetPoint())
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
				'moType1FrameAcadBlock = New DMAcadExt.AcadBlock(msStampBlockNameType2, msBlockPath)

				zzOKType1Frame()
				zzOKType1DirArrow()
				zzOKScale()
				zzOKLegend()
				'zzOKGushRemark()
				zzOKUpdateTable()

				For dDeltaX = 0.0 To mdCurrentFrameWidth - 0.1 * dUnit Step dUnit

					zzInsertCoordinate(moFrameInsPoint.GetMoved(dDeltaX, 0.0), DMAcadExt.TPlnPoint.enAxis.X)
					If dDeltaX <> 0.0 Then
						zzInsertLine(moFrameInsPoint.GetMoved(dDeltaX, 0.0), DMAcadExt.TPlnPoint.enAxis.X, dLineLength)
					End If
				Next


				For dDeltaY = dUnit To mdCurrentFrameHeight Step dUnit

					zzInsertCoordinate(moFrameInsPoint.GetMoved(0.0, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, -dFrameWidth)
					zzInsertLine(moFrameInsPoint.GetMoved(0.0, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, dLineLength)
				Next

				If dDeltaY > mdCurrentFrameHeight + 0.9 * dUnit Then
					dDeltaXStart = dUnit
				Else
					dDeltaXStart = 0.0
				End If

				'Top
				For dDeltaX = dDeltaXStart To mdCurrentFrameWidth Step dUnit
					zzInsertCoordinate(moFrameInsPoint.GetMoved(dDeltaX, mdCurrentFrameHeight), DMAcadExt.TPlnPoint.enAxis.X, dFrameWidth)
					If dDeltaX <> 0.0 Then
						zzInsertLine(moFrameInsPoint.GetMoved(dDeltaX, mdCurrentFrameHeight), DMAcadExt.TPlnPoint.enAxis.X, -dLineLength)
					End If
				Next


				If dDeltaX > mdCurrentFrameWidth + 0.9 * dUnit Then
					dDeltaYEnd = mdCurrentFrameHeight - 0.1 * dUnit
				Else
					dDeltaYEnd = mdCurrentFrameHeight
				End If

				'Right
				For dDeltaY = 0.0 To dDeltaYEnd Step dUnit
					zzInsertCoordinate(moFrameInsPoint.GetMoved(mdCurrentFrameWidth, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y)
					If dDeltaY <> mdCurrentFrameHeight Then
						zzInsertLine(moFrameInsPoint.GetMoved(mdCurrentFrameWidth, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, -dLineLength)
					End If
				Next


				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()


				'Dim oPline As Polyline = New Polyline()


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Unidiv - SelectPoint")
			End Try
		End If

		If iSnapMode <> iRequiredSnapMode Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName, iSnapMode)
		End If

		If tSnapUnitPoint <> tRequiredSnapUnitPoint Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName, tSnapUnitPoint)
		End If


		'DMCommon.Debug.MsgBox("291122_11")
		Me.Visible = True
		'	Return oPromptResult.Status

	End Sub

	Private Sub zzMyDrawFrame()
		Dim dPageWidth As Double = 640.0
		Dim dPageHeight As Double = 480.0
		Dim dNumberPerSize As Double = 8.0
		Dim dFrameWidth As Double = 10.0
		Dim dLineLength As Double = 5.0
		Dim dUnit As Double = zzGetUnit(dPageWidth, dNumberPerSize)
		'	Dim dPageWidthInScale As Double
		'	Dim dPageHeightInScale As Double



		mdCurrentFrameHeight = mdFrameHeight * AcadReport.RepApp.DrawingScaleFactor


		mdCurrentFrameWidth = dUnit * Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageWidth / dUnit, 0)
		mdCurrentFrameHeight = dUnit * Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageHeight / dUnit, 0)

		mdCurrentFrameWidth = Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageWidth, 0)
		mdCurrentFrameHeight = Math.Round(AcadReport.RepApp.DrawingScaleFactor * dPageHeight, 0)


		Dim iRequiredSnapMode As Integer = 1

		Dim tRequiredSnapUnitPoint As Autodesk.AutoCAD.Geometry.Point2d = New Autodesk.AutoCAD.Geometry.Point2d(dUnit, dUnit)
		Dim dDeltaX As Double
		Dim dDeltaY As Double

		Dim dDeltaXStart As Double
		Dim dDeltaYEnd As Double

		Dim iSnapMode As System.Int32 = CInt(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName))
		Dim tSnapUnitPoint As Autodesk.AutoCAD.Geometry.Point2d = DirectCast(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName), Autodesk.AutoCAD.Geometry.Point2d)
		If iSnapMode <> iRequiredSnapMode Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName, iRequiredSnapMode)
		End If

		If tSnapUnitPoint <> tRequiredSnapUnitPoint Then
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName, tRequiredSnapUnitPoint)
		End If
		Me.Visible = False
		'	DMCommon.Debug.MsgBox("291122_1", AcadReport.RepApp.DrawingScaleFactor, iSnapMode, tSnapUnitPoint)
		Dim oJig As DMAcadExt.PointJig = New DMAcadExt.FrameJig(mdCurrentFrameWidth, mdCurrentFrameHeight, dFrameWidth)

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptResult As Autodesk.AutoCAD.EditorInput.PromptResult = oEditor.Drag(oJig)

		DMCommon.Debug.MsgBox("291122_3", mdCurrentFrameWidth, mdCurrentFrameHeight)
		DMAcadExt.AcadDocument.WriteMessage("after select Ss12" & vbCrLf)
		If oPromptResult.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				moFrameInsPoint = New DMAcadExt.TPlnPoint(oJig.GetPoint())
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
				'moType1FrameAcadBlock = New DMAcadExt.AcadBlock(msStampBlockNameType2, msBlockPath)

				zzOKType1Frame()
				zzOKType1DirArrow()
				zzOKScale()
				zzOKLegend()
				'zzOKGushRemark()
				zzOKUpdateTable()

				For dDeltaX = 0.0 To mdCurrentFrameWidth - 0.1 * dUnit Step dUnit

					zzInsertCoordinate(moFrameInsPoint.GetMoved(dDeltaX, 0.0), DMAcadExt.TPlnPoint.enAxis.X)
					If dDeltaX <> 0.0 Then
						zzInsertLine(moFrameInsPoint.GetMoved(dDeltaX, 0.0), DMAcadExt.TPlnPoint.enAxis.X, AcadReport.RepApp.DrawingScaleFactor * dLineLength)
					End If
				Next


				For dDeltaY = dUnit To mdCurrentFrameHeight Step dUnit

					zzInsertCoordinate(moFrameInsPoint.GetMoved(0.0, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, -AcadReport.RepApp.DrawingScaleFactor * dFrameWidth)
					zzInsertLine(moFrameInsPoint.GetMoved(0.0, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, AcadReport.RepApp.DrawingScaleFactor * dLineLength)
				Next

				If dDeltaY > mdCurrentFrameHeight + 0.9 * dUnit Then
					dDeltaXStart = dUnit
				Else
					dDeltaXStart = 0.0
				End If

				'Top
				For dDeltaX = dDeltaXStart To mdCurrentFrameWidth Step dUnit
					zzInsertCoordinate(moFrameInsPoint.GetMoved(dDeltaX, mdCurrentFrameHeight), DMAcadExt.TPlnPoint.enAxis.X, AcadReport.RepApp.DrawingScaleFactor * dFrameWidth)
					If dDeltaX <> 0.0 Then
						zzInsertLine(moFrameInsPoint.GetMoved(dDeltaX, mdCurrentFrameHeight), DMAcadExt.TPlnPoint.enAxis.X, -AcadReport.RepApp.DrawingScaleFactor * dLineLength)
					End If
				Next


				If dDeltaX > mdCurrentFrameWidth + 0.9 * dUnit Then
					dDeltaYEnd = mdCurrentFrameHeight - 0.1 * dUnit
				Else
					dDeltaYEnd = mdCurrentFrameHeight
				End If

				'Right
				For dDeltaY = 0.0 To dDeltaYEnd Step dUnit
					zzInsertCoordinate(moFrameInsPoint.GetMoved(mdCurrentFrameWidth, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y)
					If dDeltaY <> mdCurrentFrameHeight Then
						zzInsertLine(moFrameInsPoint.GetMoved(mdCurrentFrameWidth, dDeltaY), DMAcadExt.TPlnPoint.enAxis.Y, -AcadReport.RepApp.DrawingScaleFactor * dLineLength)
					End If
				Next


				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()


				'Dim oPline As Polyline = New Polyline()





			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Unidiv - SelectPoint")
			End Try
		End If
		'DMCommon.Debug.MsgBox("291122_11")
		Me.Visible = True
		'	Return oPromptResult.Status

	End Sub

	Private Function zzGetUnit(dSize As Double, dNumberPerSize As Double) As Double
		'
		Dim dDrawingScaleFactor As Double = AcadReport.RepApp.DrawingScaleFactor
		Dim daRange() As Double = {100.0, 50.0, 25.0, 20.0, 10.0}
		Dim d1 As Double = dSize * dDrawingScaleFactor / dNumberPerSize
		Dim d2 As Double, d3 As Double
		Dim dFactor As Double = 10.0
		Dim iFactorIndex As Integer = 0
		Do
			d3 = d1 * dFactor
			If d3 < 100.0 Then

				Exit Do
			Else
				dFactor *= 0.1

			End If
		Loop

		For iIndex As Integer = 0 To daRange.GetUpperBound(0) - 1
			If d3 > 0.5 * (daRange(iIndex) + daRange(iIndex + 1)) Then
				d2 = daRange(iIndex)
				Exit For
			End If
		Next
		If d2 = 0.0 Then
			d2 = daRange(daRange.GetUpperBound(0))
		End If

		d2 /= dFactor

		Return Math.Round(d2, 0)
	End Function
	Private Function zzGetUnitA(dSize As Double, dNumberPerSize As Double) As Double
		'AcadReport.RepApp.DrawingScaleFactor
		Dim dDrawingScaleFactor As Double = 0.25 '1.0 ' 0.625 ' 100 '20 '10 ' 5 ' 2.5 '1.25 '1.0 ' 0.625 '0.5 '0.25 ' 0.1 ' 0.25 ' 0.625
		Dim daRange() As Double = {100.0, 50.0, 25.0, 20.0, 10.0}
		Dim d1 As Double = dSize * dDrawingScaleFactor / dNumberPerSize
		Dim d2 As Double, d3 As Double
		Dim dFactor As Double = 10.0
		Dim iFactorIndex As Integer = 0
		Do
			d3 = d1 * dFactor
			If d3 < 100.0 Then

				Exit Do
			Else
				dFactor *= 0.1

			End If
		Loop
		If True Then
			If d3 > 0.5 * (daRange(0) + daRange(1)) Then
				d2 = daRange(0)
			ElseIf d3 > 0.5 * (daRange(1) + daRange(2)) Then
				d2 = daRange(1)
			ElseIf d3 > 0.5 * (daRange(2) + daRange(3)) Then
				d2 = daRange(2)
			ElseIf d3 > 0.5 * (daRange(3) + daRange(4)) Then
				d2 = daRange(3)
			Else
				d2 = daRange(4)
			End If
		End If
		For iIndex As Integer = 0 To daRange.GetUpperBound(0) - 1
			If d3 > 0.5 * (daRange(iIndex) + daRange(iIndex + 1)) Then
				d2 = daRange(iIndex)
			End If
			If d2 = 0.0 Then
				d2 = daRange(iIndex + 1)
			Else
				Exit For
			End If
		Next
		d2 /= dFactor
		'Stop
		Return Math.Round(d2, 0)
	End Function
	Private Function zzGetUnit1(dSize As Double, dNumberPerSize As Double) As Double
		'AcadReport.RepApp.DrawingScaleFactor
		Dim dFactor As Double = 0.1
		Dim d1 As Double = dSize * AcadReport.RepApp.DrawingScaleFactor / dNumberPerSize

	End Function
	Private Sub cmdSaveToDWG_Click(oSender As System.Object, e As EventArgs) Handles cmdSaveToDWG.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim bCurrentLayerOK As Boolean
		AcadReport.RepApp.InitDWGScaleFactor()
		zzInitStampAcadBlock()

		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msTazarMapBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msGenBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msEllipseBlockLayer, DMAcadExt.DMApp.AppID, True, True)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msStampBlockLayer, DMAcadExt.DMApp.AppID, True, True)

		If miPlanType = 1 Then

			zzOKGushProp()
			zzOKGenType1()
			zzOKScaleText()
			zzOK_MAPI()
			zzOK_Title()
			'	zzOKScale()
			zzOKGushRemark()

		Else
			zzSetInsertionPoint()
			DMCommon.Debug.MsgBox("15_002", mtStampInsPoint)
			zzOKGeneralNew()

			zzOKEllipse()

			'	zzOKPlanNumber()
			zzOKStampNew()
			'zzOKDmAddress()
			'    zzOKDirArrow()

			zzOKPageNo()

			zzOKDmAddress()
			DMCommon.Debug.MsgBox("15_007")
			zzOKCaption()
			DMCommon.Debug.MsgBox("15_008")
		End If



		'zzOKCorners()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
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
	Private Sub zzGetRdbMapStatus(rdbStatus As RadioButton, iMapStatus As enMapStatus)
		If mbEventsEnabled AndAlso rdbStatus.Checked Then
			Dim oDynValue As System.Object = Me.bnsPlans.Current
			If oDynValue IsNot Nothing Then
				Dim oRowView As DataRowView = TryCast(oDynValue, DataRowView)
				If oRowView IsNot Nothing Then
					oRowView.Item("MapStatus") = iMapStatus
				End If
			End If
		End If
	End Sub
	Private Sub frmUD_General_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			'	Me.Size = New System.Drawing.Size(934, 654)
			Me.WindowState = FormWindowState.Normal
		End If
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

	Private Function zzGetMaxPlanID() As Integer
		Dim iPlanID As Integer
		Dim iResMaxPlanID As Integer = 0

		For Each oRow As DataRow In moProjectPlanTable.Rows
			If oRow.RowState <> DataRowState.Deleted Then
				iPlanID = DirectCast(oRow.Item("PlanID"), Integer)
				If iResMaxPlanID < iPlanID Then
					iResMaxPlanID = iPlanID
				End If
			End If


		Next
		Return iResMaxPlanID
	End Function
	Private Sub zzSetMaxPlanID(ByRef iMaxPlanID As Integer, ByRef iMaxIndexNum As Integer)
		Dim iPlanID As Integer
		Dim iIndexNum As Integer

		Dim iResMaxPlanID As Integer = 0
		Dim iResMaxIndexNum As Integer = 0


		For Each oRow As DataRow In moProjectPlanTable.Rows
			If oRow.RowState <> DataRowState.Deleted Then
				iPlanID = DirectCast(oRow.Item("PlanID"), Integer)
				iIndexNum = DirectCast(oRow.Item("IndexNum"), Integer)

				If iResMaxPlanID < iPlanID Then
					iResMaxPlanID = iPlanID
				End If
				If iResMaxIndexNum < iIndexNum Then
					iResMaxIndexNum = iIndexNum
				End If
			End If


		Next
		iMaxPlanID = iResMaxPlanID
		iMaxIndexNum = iResMaxIndexNum
	End Sub
	Private Sub moProjectPlanTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moProjectPlanTable.TableNewRow
		'Dim iNewNumber As Integer
		Dim iNewPlanID As Integer
		Dim iNewIndexNum As Integer

		moNewRow = e.Row
		moNewRow.Item("ProjectCode") = miProjectCode
		moNewRow.Item("Detail") = miDetailNo
		'iNewNumber = zzGetMaxPlanID() + 1
		zzSetMaxPlanID(iNewPlanID, iNewIndexNum)
		iNewPlanID += 1
		iNewIndexNum += 1

		moNewRow.Item("PlanID") = iNewPlanID
		moNewRow.Item("IndexNum") = iNewIndexNum

		moNewRow.Item("BlockIsRegulated") = False

		moNewRow.Item("Note01") = False
		moNewRow.Item("Note02") = False
		moNewRow.Item("Note03") = False
		moNewRow.Item("Note04") = False
		moNewRow.Item("Note15") = False
		moNewRow.Item("Note16") = False
		moNewRow.Item("Note17") = False
		moNewRow.Item("OriginalBlockAddNo") = 0

		''''''''''''''''''moNewRow.EndEdit()

	End Sub


	Private Sub bnnPlans_Validating(oSender As System.Object, e As CancelEventArgs) Handles bnnPlans.Validating
		Try
			DMCommon.Debug.MsgBox("13_033s")
			Dim oDataRowView As DataRowView = DirectCast(bnsPlans.Current, DataRowView)
			Dim iBlokNo As Integer = DMCommon.Functions.CIntN("OriginalBlockNo")
			e.Cancel = (iBlokNo = 0)
			DMCommon.Debug.MsgBox("13_033", e.Cancel)
		Catch oEx As Exception

		End Try
	End Sub




	Private Sub tsbSaveData_Click(oSender As System.Object, e As EventArgs) Handles tsbSaveData.Click
		'Dim iRowStateBefore As DataRowState
		'Dim iRowStateAfter As DataRowState
		Dim iReturnProjectDataAdapter As Integer
		Dim iReturnProjectPlanDataAdapter As Integer

		Dim bProjectDataSuccess As Boolean
		Dim bProjectPlanDataSuccess As Boolean

		Me.Cursor = Cursors.WaitCursor
		mskPlanID.Focus()
		'DMCommon.Debug.MsgBox("mskPlanID.Focus", mskPlanID.Focus())
		zzGetPlanType()
		Dim oRow As DataRowView = zzGetProjectDataRow()
		If oRow IsNot Nothing Then
			Dim iPlanType As Integer = DMCommon.Functions.CIntN(oRow.Item("PlanType"))
			If iPlanType <> miPlanType Then
				oRow.Item("PlanType") = miPlanType
			End If

			oRow.EndEdit()
		End If
		zzEndEditProjectPlanTable()


		'	DMCommon.Debug.MsgBox("Status", iRowStateBefore, iRowStateAfter)
		Try
			iReturnProjectDataAdapter = moProjectDataAdapter.Update(moProjectDataTable)
			bProjectDataSuccess = True
		Catch oEx As Exception
			Dim oInnerEx As Exception = oEx.InnerException
			Dim sInnerMsg As String
			If oInnerEx IsNot Nothing Then
				sInnerMsg = oInnerEx.Message
			Else
				sInnerMsg = String.Empty
			End If

			DMCommon.Debug.MsgBox("ProjectData", DMCommon.Debug.ColCount(oEx.Data), Hex(oEx.HResult), iReturnProjectDataAdapter, sInnerMsg, oEx.Message, oEx.Source)
		End Try
		'DMCommon.Debug.ExcelLog.SetDataTable(0, "moProjectPlanTable", moProjectPlanTable)
		If zzCheckIndexNum() Then

			Try
				iReturnProjectPlanDataAdapter = moProjectPlanDataAdapter.Update(moProjectPlanTable)
				bProjectPlanDataSuccess = True
				'	DMCommon.Debug.MsgBox("OK", iReturnProjectDataAdapter, iReturnProjectPlanDataAdapter)
			Catch oEx As Exception
				Dim oInnerEx As Exception = oEx.InnerException
				Dim sInnerMsg As String
				If oInnerEx IsNot Nothing Then
					sInnerMsg = oInnerEx.Message
				Else
					sInnerMsg = String.Empty
				End If


				DMCommon.Debug.MsgBox("ProjectPlanData", DMCommon.Debug.ColCount(oEx.Data), Hex(oEx.HResult), iReturnProjectDataAdapter, iReturnProjectPlanDataAdapter, sInnerMsg, oEx.Message, oEx.Source)
				'	MessageBox.Show(oEx.Data.Count)
			End Try
		Else
			iReturnProjectPlanDataAdapter = 0
		End If

		moBasePlansAdapter.Update(moBasePlansTable)

		Dim sRecordsRes As String = iReturnProjectDataAdapter.ToString & "/" & iReturnProjectPlanDataAdapter.ToString()
		If bProjectPlanDataSuccess AndAlso bProjectPlanDataSuccess Then

			If iReturnProjectDataAdapter + iReturnProjectPlanDataAdapter > 0 Then
				Me.tslModifyDate.Text = Date.Now.ToString()
				Me.tslResult.Text = "Success " & sRecordsRes
				Me.tslResult.BackColor = Color.FromArgb(0, 255, 0)
			ElseIf iReturnProjectDataAdapter + iReturnProjectPlanDataAdapter = 0 Then
				Me.tslModifyDate.Text = String.Empty
				Me.tslResult.Text = "Nothing " & sRecordsRes
				Me.tslResult.BackColor = Color.FromArgb(255, 255, 0)
			Else
				Me.tslModifyDate.Text = String.Empty
				Me.tslResult.Text = "Failed " & sRecordsRes
				Me.tslResult.BackColor = Color.FromArgb(255, 0, 0)
			End If
		Else
			Me.tslModifyDate.Text = String.Empty
			Me.tslResult.Text = "Failed " & sRecordsRes
			Me.tslResult.BackColor = Color.FromArgb(255, 0, 0)
		End If


		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzCheckIndexNum() As Boolean
		Dim iPlanID As Integer
		Dim iIndexNum As Integer
		Dim hsIndexNums As HashSet(Of Integer) = New HashSet(Of Integer)()

		For Each oRow As System.Data.DataRow In moProjectPlanTable.Rows
			iIndexNum = DMCommon.Functions.CIntN(oRow.Item("IndexNum"))
			If iIndexNum = 0 Then
				iPlanID = DMCommon.Functions.CIntN(oRow.Item("PlanID"))
				DMCommon.Debug.UserMsg("Err #217", DMCommon.dmMessages.Message(322, iPlanID))
				Return False
			ElseIf hsIndexNums.Contains(iIndexNum) Then
				iPlanID = DMCommon.Functions.CIntN(oRow.Item("PlanID"))
				DMCommon.Debug.UserMsg("Err #218", DMCommon.dmMessages.Message(323, iIndexNum, iPlanID))


				Return False
			Else
				hsIndexNums.Add(iIndexNum)
			End If
		Next
		Return True
	End Function
	Private Sub zzEndEditProjectPlanTable()
		'	Dim iaRowStateBefore(moProjectPlanTable.Rows.Count - 1) As DataRowState
		'	Dim iaRowStateAfter(moProjectPlanTable.Rows.Count - 1) As DataRowState
		'	Dim i As Integer = 0

		For Each oRow As DataRow In moProjectPlanTable.Rows
			If oRow IsNot Nothing Then
				'iaRowStateBefore(i) = oRow.RowState
				oRow.EndEdit()
				'	iaRowStateAfter(i) = oRow.RowState
			End If
			'	i += 1
		Next



		'	DMCommon.Debug.MsgBox("moNewRowB", moProjectPlanTable.Rows.Count, bnsPlans.CurrencyManager.Count, moNewRow.RowState)
		If moNewRow IsNot Nothing AndAlso moNewRow.RowState = DataRowState.Detached Then
			moProjectPlanTable.Rows.Add(moNewRow)
		End If

	End Sub
	Private Sub cmdOpenSelectPlanForm_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenSelectPlanForm.Click
		zzOpenSelectPlanForm()
	End Sub

	Private Sub moProjectDataTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moProjectDataTable.TableNewRow
		'	DMCommon.Debug.MsgBox("13_036d")
		Dim oProjectDataRow As DataRow = e.Row

		oProjectDataRow.Item("ProjectCode") = miProjectCode
		oProjectDataRow.Item("Detail") = miDetailNo
		oProjectDataRow.Item("UserName") = System.Environment.UserName
		oProjectDataRow.Item("CreateDate") = Date.Now


		'	oProjectDataRow.Item("Orderer") = "אאאאאאא"



		oProjectDataRow.Item("Note05") = False
		oProjectDataRow.Item("Note06") = False
		oProjectDataRow.Item("Note07") = False
		oProjectDataRow.Item("Note08") = False
		oProjectDataRow.Item("Note09") = False
		oProjectDataRow.Item("Note10") = False
		oProjectDataRow.Item("Note11") = False
		oProjectDataRow.Item("Note12") = False
		oProjectDataRow.Item("Note13") = False
		oProjectDataRow.Item("Note14") = False
		oProjectDataRow.Item("Note18") = False

		'moDataRow.Item("OriginalBlockNo") = 0

		'	DMCommon.Debug.MsgBox("13_036a", moProjectDataRow.RowState, moProjectDataTable.Rows.Count)
		'moProjectDataTable.Rows.Add(moDataRow)
		'	DMCommon.Debug.MsgBox("13_036aa", moDataRow.RowState, moProjectDataTable.Rows.Count)
	End Sub
	Public Function zzGetProjectDataRow() As DataRowView
		Try
			Return TryCast(bnsProject.Current, DataRowView)

		Catch oEx As Exception
			Return Nothing
		End Try
	End Function
	Private Function zzGetPlanDataRow() As DataRowView
		Try
			Return TryCast(bnsPlans.Current, DataRowView)

		Catch oEx As Exception
			Return Nothing
		End Try
	End Function

	Private Function zzGetCurrentPlanID() As Integer
		Dim oPlanRow As DataRowView = zzGetPlanDataRow()
		Return DMCommon.Functions.CIntN(oPlanRow.Item("PlanID"))

	End Function

	Private Sub mskPlanID_KeyDown(oSender As System.Object, e As KeyEventArgs) Handles mskPlanID.KeyDown
		Dim iRowStateBefore As DataRowState
		Dim iRowStateAfter As DataRowState

		If e.KeyCode = Keys.Escape Then

			Dim oPlanRow As DataRowView = zzGetPlanDataRow()


			iRowStateBefore = oPlanRow.Row.RowState
			oPlanRow.CancelEdit()
			iRowStateAfter = oPlanRow.Row.RowState
			'DMCommon.Debug.MsgBox("StatusValidating", iRowStateBefore, iRowStateAfter)


		End If
	End Sub

	Private Sub bnsPlans_PositionChanged(oSender As System.Object, e As EventArgs) Handles bnsPlans.PositionChanged
		Dim bDelEnabled As Boolean = (bnsPlans.CurrencyManager.Position + 1 = bnsPlans.CurrencyManager.Count)
		Dim bCopyEnabled As Boolean = (bnsPlans.CurrencyManager.Position > 0)
		Dim iPortionNo As Integer
		If Me.BindingNavigatorDeleteItem.Enabled <> bDelEnabled Then
			Me.BindingNavigatorDeleteItem.Enabled = bDelEnabled
		End If

		If Me.tsbCopyFromPrev.Enabled <> bCopyEnabled Then
			Me.tsbCopyFromPrev.Enabled = bCopyEnabled
		End If

		Dim oPlanRow As DataRowView = zzGetPlanDataRow()
		miPlanID = DirectCast(oPlanRow.Item("PlanID"), Integer)
		iPortionNo = DMCommon.Functions.CIntN( oPlanRow.Item("PortionNo"))


		Dim iPosition As Integer = bnsPortions.Find("PortionNo", iPortionNo)
		If iPosition >= 0 Then
			bnsPortions.Position = iPosition
		End If


		moBasePlansView = New DataView(moBasePlansTable, "PlanID=" & CStr(miPlanID), String.Empty, DataViewRowState.CurrentRows)
		Me.dgvBasePlans.DataSource = moBasePlansView
		'	DMCommon.Debug.MsgBox("CurrencyManager", bnsPlans.CurrencyManager.Position, bnsPlans.CurrencyManager.Count, moBasePlansView.Count)
	End Sub

	Private Sub cmdScaleFromDWG_Click(oSender As System.Object, e As EventArgs) Handles cmdScaleFromDWG.Click
		Try
			Me.cmbScales.Text = DMAcadExt.AcadDocument.GetDWGScaleText
			Me.cmbScales.Focus()
		Catch oEx As Exception

		End Try

	End Sub


	Private Sub tsbCopyFromPrev_Click(oSender As System.Object, e As EventArgs) Handles tsbCopyFromPrev.Click
		Dim iCurrentRecNo As Integer = bnsPlans.CurrencyManager.Position
		If iCurrentRecNo > 0 Then
			Dim oPrevRow As DataRow = moProjectPlanTable.Rows.Item(iCurrentRecNo - 1)
			Dim oDataRowView As DataRowView = DirectCast(bnsPlans.Current, DataRowView)
			Dim oCurrentRow As DataRow = oDataRowView.Row
			oCurrentRow.Item("DistrictID") = oPrevRow.Item("DistrictID")
			oCurrentRow.Item("SubdistrictID") = oPrevRow.Item("SubdistrictID")
			oCurrentRow.Item("LocalityID") = oPrevRow.Item("LocalityID")
			oCurrentRow.Item("PlanNum") = oPrevRow.Item("PlanNum")
			oCurrentRow.Item("PlanPHNum") = oPrevRow.Item("PlanPHNum")
			oCurrentRow.Item("BlockIsRegulated") = oPrevRow.Item("BlockIsRegulated")
			oCurrentRow.EndEdit()
			bnsPlans.EndEdit()
			bnsPlans.ResetCurrentItem()

			If False Then
				Me.cmbDistrict.SelectedValue = oPrevRow.Item("DistrictID")
				Me.cmbSubdistrict.SelectedValue = oPrevRow.Item("SubdistrictID")
				Me.cmbLocality.SelectedValue = oPrevRow.Item("LocalityID")
				Me.txtPlanNum.Text = DMCommon.Functions.CStrN(oPrevRow.Item("PlanNum"))
				Me.txtPlanPHNum.Text = DMCommon.Functions.CStrN(oPrevRow.Item("PlanPHNum"))
				Me.chkGushStatus.Checked = DirectCast(oPrevRow.Item("BlockIsRegulated"), Boolean)
			End If

		End If



	End Sub


	Private Sub Button4_Click_1(oSender As System.Object, e As EventArgs)
		Dim oDataRowView As DataRowView = DirectCast(bnsPlans.Current, DataRowView)

	End Sub

	Private Sub moBasePlansTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moBasePlansTable.TableNewRow
		Dim oBasePlanRow As DataRow = e.Row

		oBasePlanRow.Item("ProjectCode") = miProjectCode
		oBasePlanRow.Item("Detail") = miDetailNo
		oBasePlanRow.Item("PlanID") = miPlanID

		oBasePlanRow.Item("UserName") = System.Environment.UserName
		oBasePlanRow.Item("CreateDate") = Date.Now
	End Sub

	Private Sub cmdInsertComStamp_Click(sender As System.Object, e As EventArgs) Handles cmdInsertComStamp.Click

		'Const dX As Double = 181.6
		'Const dY As Double = 70.7
		Const dX As Double = 54.0
		Const dY As Double = -66.0

		Const msTazarMapBlockLayer As String = "C1680"
		Const sX As String = "„—…‡ ˆ‰˜™/‰‹ ˜” '"

		Const dX1 As Double = -92.63
		Const dRowHeight As Double = 6.37
		Const dRowWidth As Double = 92.63
		AcadReport.RepApp.InitDWGScaleFactor()
		Dim oBlockRef As BlockReference
		Dim oaHeaderAttribDefs() As AttributeDefinition = Nothing
		Dim oaRowAttribDefs() As AttributeDefinition = Nothing
		Dim oaFooterAttribDefs() As AttributeDefinition = Nothing

		Dim dicAttributes As Dictionary(Of String, String) = Nothing
		Dim tCaptionBlockInsPoint As Autodesk.AutoCAD.Geometry.Point3d = zzGetCaptionPoint()

		Dim tTableBlockInsPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(tCaptionBlockInsPoint.X + dX * AcadReport.RepApp.DrawingScaleFactor, tCaptionBlockInsPoint.Y + dY * AcadReport.RepApp.DrawingScaleFactor, 0.0)

		'Dim oTable As DataTable
		'222679.606 754374.644

		Dim tHeaderInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = tTableBlockInsPoint
		zzGetPlanType()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		'	Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(1, 1, 0)
		DMAcadExt.AcadTransaction.SetCurrentLayer(msTazarMapBlockLayer, 1, True, True)
		'Dim tHeaderInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(222600.0, 754500.0, 0)
		Dim tRowInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim tFirstRowLeft As Autodesk.AutoCAD.Geometry.Point3d
		Dim tFirstRowRight As Autodesk.AutoCAD.Geometry.Point3d

		Dim tHeaderBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msHeaderBlockName, oaHeaderAttribDefs)
		Dim tFooterBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msFooterBlockName, oaFooterAttribDefs)
		Dim tRowBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msRowBlockName, oaRowAttribDefs)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oaAttribDefs", oaAttribDefs Is Nothing)

		Dim colAcObjectIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs("Table")
		If colAcObjectIDs IsNot Nothing Then
			DMAcadExt.AcadTransaction.EraseDBObjects(colAcObjectIDs)
		End If

		DMAcadExt.AcadTransaction.OpenNewBlock("Table")


		dicAttributes = zzGetHeaderData()

		DMAcadExt.AcadTransaction.InsertBlockRef(tHeaderBlockObjId, tHeaderInsertPoint, oaHeaderAttribDefs, dicAttributes, 1.2,,,,, True)
		tFirstRowLeft = New Autodesk.AutoCAD.Geometry.Point3d(tHeaderInsertPoint.X - dRowWidth * AcadReport.RepApp.DrawingScaleFactor, tHeaderInsertPoint.Y + 30.0 * AcadReport.RepApp.DrawingScaleFactor, 0)
		tFirstRowRight = New Autodesk.AutoCAD.Geometry.Point3d(tHeaderInsertPoint.X, tHeaderInsertPoint.Y + 30.0 * AcadReport.RepApp.DrawingScaleFactor, 0)

		tRowInsertPoint = New Autodesk.AutoCAD.Geometry.Point3d(tFirstRowRight.X, tFirstRowRight.Y - dRowHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)

		Dim oLine As Line = New Line(tFirstRowLeft, tFirstRowRight)
		DMAcadExt.AcadTransaction.AddToNewBlock(oLine)
		dicAttributes = zzGetRowData()
		DMAcadExt.AcadTransaction.InsertBlockRef(tRowBlockObjId, tRowInsertPoint, oaRowAttribDefs, dicAttributes,  ,,,, AcadReport.RepApp.DrawingScaleFactor, True)
		For Each oGridRow As DataGridViewRow In dgvBasePlans.Rows
			If Not oGridRow.IsNewRow Then
				dicAttributes = zzGetRowData(oGridRow)
				tRowInsertPoint = New Autodesk.AutoCAD.Geometry.Point3d(tRowInsertPoint.X, tRowInsertPoint.Y - dRowHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)
				DMAcadExt.AcadTransaction.InsertBlockRef(tRowBlockObjId, tRowInsertPoint, oaRowAttribDefs, dicAttributes,,,,,, True)
			End If
		Next


		''''''DMAcadExt.AcadTransaction.InsertBlockRef(tHeaderBlockObjId, tHeaderInsertPoint, oaHeaderAttribDefs, dicAttributes,,,,,, True)

		dicAttributes = New Dictionary(Of String, String)
		dicAttributes.Add("PlanNum", "!אבג")
		dicAttributes.Add("PlanType", "!דה")
		dicAttributes.Add("InForceDate", "!כלמ")

		'tRowInsertPoint = New Autodesk.AutoCAD.Geometry.Point3d(tHeaderInsertPoint.X + 100.0 - 7.369, tFirstRowRight.Y - 2.0 * dY, 0.0)







		'oBlockRef = New BlockReference(tHeaderInsertPoint, tFooterBlockObjId)
		oBlockRef = New BlockReference(tRowInsertPoint, tFooterBlockObjId)

		DMAcadExt.AcadTransaction.AddToNewBlock(oBlockRef)

		DMAcadExt.AcadTransaction.InsertNewBlock(False)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub
	Private Function zzGetHeaderData() As Dictionary(Of String, String)
		Dim dicAttributes As Dictionary(Of String, String) = New Dictionary(Of String, String)
		If Me.cmbCommittee.SelectedItem IsNot Nothing Then
			Dim oItemData As DMCommon.ItemData = DirectCast(Me.cmbCommittee.SelectedItem, DMCommon.ItemData)
			Dim sText As String = oItemData.ListDispData
			Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText, True)
			dicAttributes.Add("ComName", oHebText.GetDOSDest())
		End If

		dicAttributes.Add("MeetingDate", "")
		dicAttributes.Add("MeetingNum", "")
		Return dicAttributes
	End Function
	Private Function zzGetRowData(Optional oGridRow As DataGridViewRow = Nothing) As Dictionary(Of String, String)
		Const sText1 As String = "מספר תכנית/תשרית חלוקה"  'מספר
		Const sText2 As String = "סוג תכנית"
		Const sText3 As String = "בתוקף מיום"
		Dim sPlanNum As String
		Dim sPlanType As String
		Dim sInForceDate As String
		If oGridRow Is Nothing Then
			Dim oHebText1 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText1, True)
			Dim oHebText2 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText2, True)
			Dim oHebText3 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sText3, True)
			sPlanNum = oHebText1.GetDOSDest()
			sPlanType = oHebText2.GetDOSDest()
			sInForceDate = oHebText3.GetDOSDest()

		Else
			sPlanNum = DMCommon.Hebrew.WordToDOS(DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPlanNum").Value), False)
			sPlanType = DMCommon.Hebrew.WordToDOS(DMCommon.Functions.CStrN(oGridRow.Cells.Item("ccbPlanType").FormattedValue), False)

			Dim dtInForceDate As Date = DMCommon.Functions.CDateN(oGridRow.Cells.Item("ctxInForceDate").Value)
			sInForceDate = FormatDateTime(dtInForceDate, DateFormat.ShortDate)
		End If


		Dim dicAttributes As Dictionary(Of String, String) = New Dictionary(Of String, String)


		dicAttributes.Add("PlanNum", sPlanNum)
		dicAttributes.Add("PlanType", sPlanType)
		dicAttributes.Add("InForceDate", sInForceDate)
		Return dicAttributes
	End Function
	Private Function zzGetRowData(oDataRow As DataRowView) As Dictionary(Of String, String)
		Dim dicAttributes As Dictionary(Of String, String) = New Dictionary(Of String, String)
		Dim sPlanNum As String = DMCommon.Functions.CStrN(oDataRow.Item("PlanNum"))
		dicAttributes.Add("PlanNum", sPlanNum)


		Dim oItemData As DMCommon.ItemData = DirectCast(Me.cmbCommittee.SelectedItem, DMCommon.ItemData)
		dicAttributes.Add("ComName", oItemData.ListDispData)


		dicAttributes.Add("MeetingDate", "")
		dicAttributes.Add("MeetingNum", "")
		Return dicAttributes
	End Function



	Private Sub cmdDrawFrameHor_Click(sender As System.Object, e As EventArgs) Handles cmdDrawFrameHor.Click
		AcadReport.RepApp.InitDWGScaleFactor()
		zzGetPlanType()

		Select Case miPlanType
			Case 1
				zzMyDrawFrame(True)
			Case 2, 9, 12
				zzDrawFrameByLisp()
		End Select
	End Sub



	Private Sub bnsPlans_CurrentChanged(sender As System.Object, e As EventArgs) Handles bnsPlans.CurrentChanged
		Dim oDynValue As System.Object = Me.bnsProject.Current
		Dim iValue As Integer
		Dim iMapStatus As enMapStatus

		Dim bEventsEnabled As Boolean = mbEventsEnabled
		mbEventsEnabled = False
		If oDynValue IsNot Nothing Then
			Dim oRowView As DataRowView = TryCast(oDynValue, DataRowView)

			If oRowView IsNot Nothing Then
				iValue = DMCommon.Functions.CIntN(oRowView.Item("MapStatus"))
				If [Enum].IsDefined(GetType(enMapStatus), iValue) Then
					iMapStatus = CType(iValue, enMapStatus)
					Select Case iMapStatus
						Case enMapStatus.MapStatusEarly
							Me.rdbMapStatusEarly.Checked = True
						Case enMapStatus.MapStatusTemp
							Me.rdbMapStatusTemp.Checked = True
						Case enMapStatus.MapStatusFinal
							Me.rdbMapStatusFinal.Checked = True
					End Select
				End If



			End If
		End If
		mbEventsEnabled = bEventsEnabled
	End Sub

	Private Sub cmbBoundariesKind_SelectedIndexChanged(sender As System.Object, e As EventArgs) Handles cmbBoundariesKind.SelectedIndexChanged
		If Me.cmbBoundariesKind.SelectedIndex = 3 Then
			Me.cmbBoundariesKind.Text = String.Empty
		End If
	End Sub

	Private Sub BindingNavigatorMoveNextItem_Click(sender As Object, e As EventArgs) Handles BindingNavigatorMoveNextItem.Click

	End Sub
End Class
