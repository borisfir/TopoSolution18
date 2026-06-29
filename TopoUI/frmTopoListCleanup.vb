Option Explicit On
Option Strict On
Imports TopoManager
Imports FDO
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices

Public Class frmTopoListCleanup
	Private Const miParamType As Integer = 1
	Const miThisStagesUB As Integer = 5
	Const msSwitchCleanupText As String = "  Cleanup"
	Const msCreateTopoText As String = "יצירת טופולוגיה"
	Const msCheckTopoText As String = "בדיקת טופולוגיה"

	Const msDeleteTopoText As String = "מחיקת טופולוגיה"
	Const msShowGeometryText As String = "הצגת גיאומטריה"
	Const msEraseTopoGeoText As String = "מחיקת טופולוגיה וגיאומטריה"
	Const msTopoPropertiesText As String = "מאפייני טופולוגיה"
	Const msTopoLayersText As String = "שכבות"
	Const msStraightenText As String = "יצירת קווים במקום קשתות"
	Const msGetStatisticsText As String = "סטטיסטיקה"

	Const msXDataAppName As String = "BreakPointApp"
	Const miTopoErrIndexNotErr As Integer = -1

	'	Private WithEvents cmdPrepare As TabButton
	Private moaTopoErrors As TopoErrorArray
	'	Private moaTopoErrorsWA As TopoErrorArray
	Private mbEventsEnabled As Boolean = False
	Private miCurrentTopoErrIndex As Integer = miTopoErrIndexNotErr

	Private miTopoPanelIndex As Integer = -1
	Private mtMapThemeInfo As DMAcadExt.MapThemeInfo
	Private moTopoDataTable As Data.DataTable
	'	Protected WithEvents dgvMessages As DataGridView
	'	Protected tbpProjectData As System.Windows.Forms.TabPage
	'	Protected tbpMessages As System.Windows.Forms.TabPage
	Private miChectActionsUB As Integer = -1
	Private miCheckIndex As Integer = 2

	'Protected moaCheckButtons(miToposUB) As CheckButton
	'	Protected dbLabelTopoLong As Boolean
	'	Protected txtRepBamashSharedScale As TextBox
	Private WithEvents mfMessages As frmMessages = Nothing
	Private WithEvents mfTplnView As frmTplnView = Nothing
	'Private WithEvents cmdEraseTopoGeometria As TabButton

	'Private WithEvents tsbSwitchCleanup As System.Windows.Forms.ToolStripButton
	'Private WithEvents tsbEraseTopoGeometria As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbCopyFromOverlay As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbToClosedPolygons As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbExportToShape As System.Windows.Forms.ToolStripButton


	Private WithEvents cmsMain As System.Windows.Forms.ContextMenuStrip
	Private WithEvents tsiCreateTopo As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiDeleteTopo As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiShowGeometry As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiEraseTopoGeo As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiTopoProperties As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiCloseForm As System.Windows.Forms.ToolStripMenuItem

	'	Private WithEvents tsiMapClearAll As System.Windows.Forms.ToolStripMenuItem
	'	Private WithEvents tsiMapClearLayer As System.Windows.Forms.ToolStripMenuItem
	'	Private WithEvents tsiParcelPgonsExp As System.Windows.Forms.ToolStripMenuItem



	Private tsiDel1 As System.Windows.Forms.ToolStripMenuItem



	Private WithEvents tsbMapPlatf As System.Windows.Forms.ToolStripDropDownButton
	'Private WithEvents tstTopo As System.Windows.Forms.ToolStrip

	'	Protected WithEvents chkCheckTopo As CheckBox
	'''''''''''''	Private WithEvents tmrInactive As System.Windows.Forms.Timer
	Private miActionDflt As Integer = 0

	Private Shared miBaseResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmActionsBase

	Private moPriorView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = Nothing

	Private txtTopoErrors As TextBox

	Private lblCaption As Label

	Private txtTopologyName As TextBox
	Private lblTopologyName As Label


	Private WithEvents chkSourceTopologia As CheckBox
	Private chkStraightenArcs As CheckBox
	Private chkLineTopologia As CheckBox

	'Private lblSourceTopologiaCap As Label
	'	Private lblSourceTopologiaV As Label





	Private moaErrorPoints() As DMAcadExt.TplnPointArray = Nothing

	Private miCurrentAction As Integer = -1
	Private mdcStepNum As Decimal

	Private moCleanupActionsTable As System.Data.DataTable




	Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
	Private moTopoErrPoints As DMAcadExt.TplnPointArray
	Private moCurrentTopoErrIndex As Integer = -1


	Private mhsDBDWGLayers As HashSet(Of String)


	Private WithEvents chkTopoLayersOn As CheckBox

	Private mbCodeExecuting As Boolean = False
	Private mbInactive As Boolean
	Private mdInactiveTiks As Double = 0.0

	Private moLabelColor As System.Drawing.Color = Color.DimGray
	Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue
	'														  New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	'Private moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private Shared moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))

	Private moParams As TPlServerDB.dmParams

	Private moFDO_Manager As FDO_Manager
	Private mtTopoRes As DMAcadExt.TopoRes
	Private mtTopoResWA As DMAcadExt.TopoRes

	Private txtProjectData() As TextBox
	Private cmbProjectData() As ComboBox
	Private lblProjectData() As Label

	Private iTestIndex As Integer
	Private mcolNewSPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private miParcelNumMax As Integer = 10
	Private WithEvents cmdSaveProjectData As TabButton
	Private WithEvents cmdSaveParameters As TabButton
#Region "Main_Declarations"
	'Private WithEvents cmdFirst As System.Windows.Forms.Button
	'Private WithEvents cmdPrev As System.Windows.Forms.Button
	'Private WithEvents cmdNext As System.Windows.Forms.Button
	'	Private WithEvents cmdLast As System.Windows.Forms.Button
#End Region
#Region "Panel0_3_Declarations"
	Private dgvActions(1) As DataGridView
	Private nudSteps(1) As System.Windows.Forms.NumericUpDown
	Private nudErrors(2) As System.Windows.Forms.NumericUpDown
	Private cmdEraseCleanupErr(2) As System.Windows.Forms.Button
	Private cmdFix(1) As TabButton
	Private cmdMark(1) As TabButton
	Private lblErrors(2) As System.Windows.Forms.Label
	Private moCurrentView(1) As DataView
#End Region
#Region "Panel1_Declarations"

	Private chkCreateCentroid As System.Windows.Forms.CheckBox
	Private chkCreateNode As System.Windows.Forms.CheckBox

	Private chkHighlightSliver As System.Windows.Forms.CheckBox
	Private lblTolerance As System.Windows.Forms.Label
	Private txtTolerance As System.Windows.Forms.TextBox

	Private grbLinks As System.Windows.Forms.GroupBox
	Private grbCentroids As System.Windows.Forms.GroupBox
	Private grbNodes As System.Windows.Forms.GroupBox
	Private lblCentroidBlocks As System.Windows.Forms.Label
	Private lblCentroidLayers As System.Windows.Forms.Label
	'	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private lblTopoErrors As System.Windows.Forms.Label
	Private txtErrorCount As System.Windows.Forms.TextBox
	Private txtErrorIndex As System.Windows.Forms.TextBox
	Private lblTopoName As System.Windows.Forms.Label
	Private lblTopoNameCap As System.Windows.Forms.Label
	Private lblNodeBlocks As System.Windows.Forms.Label


	'Private WithEvents cmdTest As Button

	Private WithEvents dgvTopoList As DataGridView
	Private WithEvents ctxTopoName As DataGridViewTextBoxColumn
	Private WithEvents ctxPolygonCount As DataGridViewTextBoxColumn
	Private WithEvents ctxLinkCount As DataGridViewTextBoxColumn
	Private WithEvents ctxCentroidCount As DataGridViewTextBoxColumn
	Private moDataGridViewCellStyleTopoName As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
	'Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()

	Private WithEvents cmdStartErr As System.Windows.Forms.Button
	Private WithEvents cmdPrevErr As System.Windows.Forms.Button
	Private WithEvents cmdNextErr As System.Windows.Forms.Button
	Private WithEvents cmdEraseErr As System.Windows.Forms.Button

	Private moGridCellStyleBold As System.Windows.Forms.DataGridViewCellStyle ' = New System.Windows.Forms.DataGridViewCellStyle()
	Private moDefaultCellStyle As System.Windows.Forms.DataGridViewCellStyle
	Private moErrorDefaultCellStyle As System.Windows.Forms.DataGridViewCellStyle








	Private lblTopoExists As System.Windows.Forms.Label
	Private txtPgonCount As System.Windows.Forms.TextBox
	Private txtCentroidCount As System.Windows.Forms.TextBox
	Private txtCentroidBlocks As System.Windows.Forms.TextBox
	Private txtCentroidLayers As System.Windows.Forms.TextBox
	Private txtNodeBlocks As System.Windows.Forms.TextBox
	Private lblLinkLayers As System.Windows.Forms.Label
	Private txtLinkLayers As System.Windows.Forms.TextBox
	Private txtLinkCount As System.Windows.Forms.TextBox

	Private lblNodeLayers As System.Windows.Forms.Label
	Private txtNodeLayers As System.Windows.Forms.TextBox
	Private txtNodeCount As System.Windows.Forms.TextBox

#End Region
#Region "ToolStrip_Topo"
	'	Private WithEvents tsbCreateTopo As System.Windows.Forms.ToolStripButton
	'	Private WithEvents tsbCheckTopo As System.Windows.Forms.ToolStripButton
	'Private WithEvents tsbDeleteTopo As System.Windows.Forms.ToolStripButton
	'	Private WithEvents tsbShowTopo As System.Windows.Forms.ToolStripButton
	'	Private WithEvents ddbLayers As System.Windows.Forms.ToolStripDropDownButton
	'	Private WithEvents tsbGetStatistics As System.Windows.Forms.ToolStripButton
	'	Private WithEvents tsbStraighten As System.Windows.Forms.ToolStripButton

	'	Private WithEvents tsiThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
	'	Private WithEvents tsiThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
	'Private WithEvents tsiAllVisible As System.Windows.Forms.ToolStripMenuItem

	'	Global.TopoUI.My.Resources.Resources.Layers16Tr
#End Region
#Region "Panel2_Declarations"
	Private grbSource As System.Windows.Forms.GroupBox
	Private grbWithoutArcs As System.Windows.Forms.GroupBox

	Private lblLinkLayersP2 As System.Windows.Forms.Label
	Private txtLinkLayersP2 As System.Windows.Forms.TextBox
	Private txtLinkCountP2 As System.Windows.Forms.TextBox

	Private lblLinkWithArcCount As System.Windows.Forms.Label
	Private txtLinkWithArcCount As System.Windows.Forms.TextBox
	Private lblArcCount As System.Windows.Forms.Label
	Private txtArcCount As System.Windows.Forms.TextBox

	Private lblLineLinksExist As System.Windows.Forms.Label
	Private lblLineLinkLayer As System.Windows.Forms.Label
	Private txtLineLinkLayer As System.Windows.Forms.TextBox
	Private txtLineLinkCount As System.Windows.Forms.TextBox
	Private txtStraightenTolerance As System.Windows.Forms.TextBox
	Private lblStraightenTolerance As System.Windows.Forms.Label
	Private WithEvents cmdStraightenLink As System.Windows.Forms.Button
	Private WithEvents cmdInsertBreakPoint As System.Windows.Forms.Button


#End Region
#Region "Panel4_Declarations"

	Private chkCreateCentroidWA As System.Windows.Forms.CheckBox
	Private chkHighlightSliverWA As System.Windows.Forms.CheckBox
	Private lblToleranceWA As System.Windows.Forms.Label
	Private txtToleranceWA As System.Windows.Forms.TextBox

	Private grbLinksWA As System.Windows.Forms.GroupBox

	Private grbCentroidsWA As System.Windows.Forms.GroupBox
	Private lblCentroidBlocksWA As System.Windows.Forms.Label
	Private lblCentroidLayersWA As System.Windows.Forms.Label
	Private grbClosedPgonsWA As System.Windows.Forms.GroupBox

	Private lblTopoErrorsWA As System.Windows.Forms.Label
	Private txtErrorCountWA As System.Windows.Forms.TextBox
	Private txtErrorIndexWA As System.Windows.Forms.TextBox
	Private lblTopoNameWA As System.Windows.Forms.Label
	Private lblTopoNameCapWA As System.Windows.Forms.Label
	Private WithEvents cmdNextErrWA As System.Windows.Forms.Button
	Private WithEvents cmdPrevErrWA As System.Windows.Forms.Button
	Private WithEvents cmdStartErrWA As System.Windows.Forms.Button
	Private WithEvents cmdEraseErrWA As System.Windows.Forms.Button


	Private lblTopoExistsWA As System.Windows.Forms.Label
	Private txtPgonCountWA As System.Windows.Forms.TextBox
	Private txtCentroidCountWA As System.Windows.Forms.TextBox
	Private txtCentroidBlocksWA As System.Windows.Forms.TextBox
	Private txtCentroidLayersWA As System.Windows.Forms.TextBox

	Private lblLinkLayersWA As System.Windows.Forms.Label
	Private txtLinkLayersWA As System.Windows.Forms.TextBox
	Private txtLinkCountWA As System.Windows.Forms.TextBox

	Private lblLineLinkLayers As System.Windows.Forms.Label
	Private txtLineLinkLayers As System.Windows.Forms.TextBox
	Private txtLineLinkCountB As System.Windows.Forms.TextBox

	Private lblClosedPgonsLayersWA As System.Windows.Forms.Label
	Private txtClosedPgonsLayersWA As System.Windows.Forms.TextBox
	Private txtClosedPgonsCountWA As System.Windows.Forms.TextBox

#End Region
#Region "Panel5_Declarations"
	Private dgvCheckActions As DataGridView
	Private moCheckActionsTable As System.Data.DataTable

	Private cmdExec As TabButton
	Private cmdFixCheck As TabButton
	Private cmdClear As TabButton

	Private cmdMessages As TabButton
	Private cmdDispTable As TabButton

	Private moMessagesView As DataView


	Private txtDescription As System.Windows.Forms.TextBox

	Private moPrjThemesResources As System.ComponentModel.ComponentResourceManager
#End Region

	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
		MyBase.New(tMapThemeData)

		'	ptMapThemeData = tMapThemeData




		MyBase.SetLabelDim(miThisStagesUB)
		'    DMCommon.Debug.MsgBox("07_030", miThisStagesUB, tMapThemeData.MapThemeID, doaPanels.GetUpperBound(0))
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		'
		' Add any initialization after the InitializeComponent() call.

		'    DMAcadExt.AcadDocument.TestAcadDoc("265 Before doc Lock")
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		'    DMAcadExt.AcadDocument.TestAcadDoc("266 After doc Lock")
		zzCheckTopo(False, False)

		'    DMAcadExt.AcadDocument.TestAcadDoc("310 After Open Topo  ")
		zzCheckTopoWA(False, False) '0100316 

		'  TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
		'  TopoManager.TPlanGraph.TplnProject.InitBlockDic()
		zzThemeCalculate()

		'   DMAcadExt.AcadDocument.TestAcadDoc("320 After doc Lock")
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'    DMAcadExt.AcadDocument.TestAcadDoc("330 After Unlock")
		'  MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "03_002aft")
		'	DMAcadExt.AcadDocument.TestAcadDoc("268After doc Lock")
		'	doaLabelCheck(0).SetCurrent()
		''''''''''	zzAfterChangeCurrent(0)
		DMAcadExt.AcadDocument.SetLogName()
		Me.DialogResult = System.Windows.Forms.DialogResult.No

	End Sub

	Private Sub zzMyInitializeComponent()


		Me.txtTopoErrors = New TextBox
		Me.lblTopoErrors = New Label
		Me.lblCaption = New Label




		Me.chkTopoLayersOn = New CheckBox
		Me.chkSourceTopologia = New CheckBox
		Me.chkStraightenArcs = New CheckBox
		Me.chkLineTopologia = New CheckBox


		Dim saCaptions() As String = {"הכנה לטופולוגיה", "טופולוגיה", "יישור קשתות", "הכנה לטופולוגיה ללא קשתות", "טופולוגיה ללא קשתות", "בדיקות"}
		Dim saCurrentCaptions() As String = {"", "", "", "הכנה לטופול' ללא קשתות", "טופולוגיה ללא קשתות", ""}

		MyBase.psaCaptions = saCaptions
		MyBase.psaCurrentCaptions = saCurrentCaptions

		MyBase.OnNew()
		zzInitToolStrip()
		'	Me.cmdEraseTopoGeometria = New TabButton(moLabelFont, True)
		'	Me.cmdCopyFromOverlay = New TabButton(moLabelFont, True)

		'
		Me.tsbCopyFromOverlay = New System.Windows.Forms.ToolStripButton
		Me.tsbToClosedPolygons = New System.Windows.Forms.ToolStripButton
		Me.tsbExportToShape = New System.Windows.Forms.ToolStripButton

		Me.cmsMain = New System.Windows.Forms.ContextMenuStrip(Me.moComponents)

		Me.tsiCreateTopo = New System.Windows.Forms.ToolStripMenuItem
		Me.tsiDeleteTopo = New System.Windows.Forms.ToolStripMenuItem
		Me.tsiShowGeometry = New System.Windows.Forms.ToolStripMenuItem
		Me.tsiEraseTopoGeo = New System.Windows.Forms.ToolStripMenuItem
		Me.tsiTopoProperties = New System.Windows.Forms.ToolStripMenuItem
		Me.tsiDel1 = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiDel1.Text = "-"
		Me.tsiCloseForm = New System.Windows.Forms.ToolStripMenuItem



		Me.tsbMapPlatf = New System.Windows.Forms.ToolStripDropDownButton


		'	Me.Controls.Add(Me.cmdPrepare)
		'	Me.Controls.Add(Me.tstTopo)


		Me.Controls.Add(Me.chkTopoLayersOn)
		doaLabelCheck(2).Enabled = False
		doaLabelCheck(3).Enabled = False
		doaLabelCheck(4).Enabled = False



		''''''''''''''		Me.Controls.Add(Me.chkSourceTopologia)
		'''''''''''''''''Me.Controls.Add(Me.lblSourceTopologiaCap)
		'''''''''''''Me.Controls.Add(Me.lblSourceTopologiaV)

		'
		'lblCaption
		'
		With Me.lblCaption
			.Location = New System.Drawing.Point(302, 16)  '36
			.Name = "lblCaption"
			.Size = New System.Drawing.Size(208, 24)
			.TabIndex = 29
			.Font = doCaptionBoldFont
			.Text = ptMapThemeData.MapThemeName
			'	.BorderStyle = BorderStyle.FixedSingle
			.TextAlign = ContentAlignment.MiddleCenter
		End With
		'
		'txtTopoErrors
		'
		With Me.txtTopoErrors
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(256, 4)
			.Name = "txtTopoErrors"
			.Size = New System.Drawing.Size(36, 16)
			.TabIndex = 27
			.Font = doLabelFont
			.Text = ""
			.TextAlign = HorizontalAlignment.Left
			.ReadOnly = True
		End With
		'	Me.RightToLeft = Windows.Forms.RightToLeft.No
		zzInitPanel0_3(0)
		zzInitPanel1()
		zzInitPanel2()
		zzInitPanel0_3(1)
		zzInitPanel4()
		zzInitPanel5()
		'	DMCommon.Functions.DispArray(moaPanels, "moaPanels", True)
		'	Me.RightToLeft = Windows.Forms.RightToLeft.Yes

		'	zzInitMoveButtons()



		'	AddHandler LabelCheck.ChangeCurrent, AddressOf zzAfterChangeCurrent




		Me.Controls.Add(Me.txtTopoErrors)
		Me.Controls.Add(Me.lblCaption)
		'
		'tsbCopyFromOverlay
		'
		With tsbCopyFromOverlay
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.SaveFormDesignHS
			.Name = "tsbCopyFromOverlay"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = "Update topologia"
		End With
		'
		'tsbToClosedPolygons
		'
		With tsbToClosedPolygons
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.ToClosedPgons
			.Name = "tsbToClosedPolygons"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = "Create closed polylines"
		End With
		'
		'tsbExportToShape
		'
		With tsbExportToShape
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
			.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			.Name = "tsbExportToShape"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = "Export/Import Shape"

		End With

		'
		'ToolStripDropDownButton1
		'
		Me.tsbMapPlatf.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		'	Me.ToolStripDropDownButton1.Image = CType(Resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
		Me.tsbMapPlatf.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbMapPlatf.Name = "ToolStripDropDownButton1"
		Me.tsbMapPlatf.Size = New System.Drawing.Size(29, 22)
		Me.tsbMapPlatf.Text = ""
		''''''''Me.tsbMapPlatf.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiMapClearAll, Me.tsiMapClearLayer, Me.tsiParcelPgonsExp})
		'
		'chkTopoLayersOn
		'
		With Me.chkTopoLayersOn
			.Location = New System.Drawing.Point(402, 36)
			.Name = "chkTopoLayersOn"
			.Size = New System.Drawing.Size(96, 24)
			.TabIndex = 17
			.Font = doLabelFont
			.Text = "Layers On"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = False
		End With
		'
		'chkSourceTopologia
		'
		With Me.chkSourceTopologia
			.Location = New System.Drawing.Point(360, 72)
			.Name = "chkSourceTopologia"
			.Size = New System.Drawing.Size(150, 24)
			.TabIndex = 17
			.Font = doLabelFont
			.Text = "טופולוגיה"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = True
			.Enabled = False
		End With
		'
		'chkStraightenArcs
		'
		With Me.chkStraightenArcs
			.Location = New System.Drawing.Point(360, 108)
			.Name = "chkStraightenArcs"
			.Size = New System.Drawing.Size(150, 24)
			.TabIndex = 17
			.Font = doLabelFont
			.Text = "יישור קשתות"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = True
		End With
		'
		'chkLineTopologia
		'
		With Me.chkLineTopologia
			.Location = New System.Drawing.Point(360, 144)
			.Name = "chkLineTopologia"
			.Size = New System.Drawing.Size(150, 24)
			.TabIndex = 17
			.Font = doLabelFont
			.Text = "טופולוגיה ללא קשתות"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = True
		End With
		'
		'cmsMain
		'
		With Me.cmsMain
			.AllowDrop = True
			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiCreateTopo, Me.tsiDeleteTopo, Me.tsiShowGeometry, Me.tsiEraseTopoGeo, Me.tsiTopoProperties})
			.Items.Add("-")
			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiCloseForm})

			.Name = "cmsMain"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(95, 26)
			.Text = "aaaaaaaaaa"
		End With
		'
		'tsiCreateTopo
		'
		With Me.tsiCreateTopo
			.Name = "tsiCreateTopo"
			.Size = New System.Drawing.Size(94, 22)
			.Text = msCreateTopoText
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
		End With
		'
		'tsiDeleteTopo
		'
		With Me.tsiDeleteTopo
			.Name = "tsiDeleteTopo"
			.Size = New System.Drawing.Size(94, 22)
			.Text = msDeleteTopoText
			.Image = Global.TopoUI.My.Resources.Resources.Delete
		End With
		'
		'tsiShowGeometry
		'
		With Me.tsiShowGeometry
			.Name = "tsiShowGeometry"
			.Size = New System.Drawing.Size(94, 22)
			.Text = msShowGeometryText
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
		End With
		'
		'tsiEraseTopoGeo
		'
		With Me.tsiEraseTopoGeo
			.Name = "tsiEraseTopoGeo"
			.Size = New System.Drawing.Size(94, 22)
			.Text = msEraseTopoGeoText
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
		End With
		'
		'tsiTopoProperties
		'
		With Me.tsiTopoProperties
			.Name = "tsiTopoProperties"
			.Size = New System.Drawing.Size(94, 22)
			.Text = msTopoPropertiesText
			.Image = Global.TopoUI.My.Resources.Resources.Properties
		End With

		'הצגת גיאומטריה
		'
		'tsiClose
		'
		With Me.tsiCloseForm
			.Name = "tsiClose"
			.Size = New System.Drawing.Size(94, 22)
			.Text = "סגירת חלון"
			.Image = Global.TopoUI.My.Resources.Resources.CloseForm
		End With

		'
		'tmrInactive
		'
		'''''''''''''''''''	Me.tmrInactive.Interval = 10000


		'		miCurrentTopoDefID = TopoDefs.miaTopoIDs(miActionDflt)

		zzSetGridColumns(0)
		zzSetGridColumns(1)
		zzLoadCleanupData(0)

		zzLoadCleanupData(1)






		zzCreateCheckActionsTable()
		zzLoadCheckData()
	End Sub


	Private Sub zzLoadParams()
		moParams = New TPlServerDB.dmParams(piProjectCode, piDetailNo, ptMapThemeData.MapThemeID, miParamType)
		Me.txtTolerance.Text = Convert.ToString(moParams.GetDblValue(0))
		Me.txtToleranceWA.Text = Convert.ToString(moParams.GetDblValue(1))
		Me.txtStraightenTolerance.Text = Convert.ToString(moParams.GetDblValue(2))
	End Sub
	Private Sub zzTest()
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(192, Byte), Integer))
		'''''''''''''''''''''	Me.ctxTopoName.DefaultCellStyle = DataGridViewCellStyle1
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		DataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))


		Dim oGridRow As DataGridViewRow = Me.dgvTopoList.Rows.Item(0)
		oGridRow.Cells.Item(2).Style = DataGridViewCellStyle2
		oGridRow = Me.dgvTopoList.Rows.Item(2)
		'	oGridRow.Cells.Item(2).Style = DataGridViewCellStyle2
		'	oGridRow = Me.dgvTopoList.Rows.Item(4)
		oGridRow.Cells.Item(3).Style = DataGridViewCellStyle2
		oGridRow = Me.dgvTopoList.Rows.Item(1)
		oGridRow.Cells.Item(1).Style = DataGridViewCellStyle2

	End Sub

	Private Sub zzLoadData_030924()
		Dim sComText As String = "SELECT Layer FROM EntConnectedLayers WHERE GeometricType=3 ORDER BY Description"
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sTopoName As String
		Dim iPgonCount As Integer
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		DataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(127, Byte), Integer))

		'moEntConnectedLayersTable = New Data.DataTable("EntConnectedLayers")
		'moEntConnectedLayersDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, System.Data.CommandType.Text, True, "", True, True)
		'moEntConnectedLayersDataAdapter.Fill(moEntConnectedLayersTable)
		'moEntConnectedLayersTable.Columns.Add("Number", GetType(System.Int32))
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		Dim colDBLayers As System.Collections.ObjectModel.Collection(Of String) = New ObjectModel.Collection(Of String)
		'	Dim hsDBDWGLayers As HashSet(Of String)

		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				colDBLayers.Add(oDataReader.GetString(0))
			Loop
			oDataReader.Close()
		End If

		mhsDBDWGLayers = DMAcadExt.AcadTransaction.GetLayersExist(colDBLayers)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadData", ptMapThemeData.GraphType, colDBLayers.Count, mhsDBDWGLayers.Count, "-", "-", "-", "-")
		Dim oaMapThemeInfo As DMAcadExt.MapThemeInfo() = DMAcadExt.AcadTransaction.GetStatistics(ptMapThemeData, mhsDBDWGLayers)
		If True Then
			If oaMapThemeInfo IsNot Nothing Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterStatistics", oaMapThemeInfo.GetUpperBound(0), mhsDBDWGLayers.Count)
				If oaMapThemeInfo.GetUpperBound(0) >= 0 Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterStat_1", oaMapThemeInfo(0).LinkCount)
				End If
			End If

			Dim oDataRow As DataRow
			'Dim oGridRow As DataGridViewRow

			Dim saLayers() As String = mhsDBDWGLayers.ToArray()
			moTopoDataTable.Clear()
			For iIndex As Integer = 0 To oaMapThemeInfo.GetUpperBound(0)
				sTopoName = saLayers(iIndex)
				iPgonCount = TopoManager.TopoCreator.GetTopoPolygonCount(sTopoName)

				oDataRow = moTopoDataTable.NewRow()
				oDataRow.Item("TopoName") = sTopoName
				oDataRow.Item("PolygonCount") = iPgonCount
				oDataRow.Item("LinkCount") = oaMapThemeInfo(iIndex).LinkCount
				oDataRow.Item("CentroidCount") = oaMapThemeInfo(iIndex).CentroidBlocksCount

				moTopoDataTable.Rows.Add(oDataRow)
			Next
		End If
		Me.dgvTopoList.DataSource = moTopoDataTable
		zzSetStyle()
	End Sub
	Private Sub zzCreateLayerList()
		Dim sComText As String = "SELECT Layer FROM EntConnectedLayers WHERE GeometricType=3 ORDER BY Description"
		Dim colDBLayers As System.Collections.ObjectModel.Collection(Of String) = New ObjectModel.Collection(Of String)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				colDBLayers.Add(oDataReader.GetString(0))
			Loop
			oDataReader.Close()
		End If
		mhsDBDWGLayers = DMAcadExt.AcadTransaction.GetLayersExist(colDBLayers)
		Dim oaMapThemeInfo As DMAcadExt.MapThemeInfo() = DMAcadExt.AcadTransaction.GetStatistics(ptMapThemeData, mhsDBDWGLayers)
		'	DMCommon.Debug.MsgBox("12_317K", colDBLayers.Count, mhsDBDWGLayers.Count)
	End Sub
	Private Sub zzLoadData()
		'''''''''''Dim sComText As String = "SELECT Layer FROM EntConnectedLayers WHERE GeometricType=3 ORDER BY Description"
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sTopoName As String
		Dim iPgonCount As Integer
		'	Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		'	DataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(127, Byte), Integer))

		'moEntConnectedLayersTable = New Data.DataTable("EntConnectedLayers")
		'moEntConnectedLayersDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, System.Data.CommandType.Text, True, "", True, True)
		'moEntConnectedLayersDataAdapter.Fill(moEntConnectedLayersTable)
		'moEntConnectedLayersTable.Columns.Add("Number", GetType(System.Int32))
		''''''''''''''''	Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		Dim colDBLayers As System.Collections.ObjectModel.Collection(Of String) = New ObjectModel.Collection(Of String)
		'	Dim hsDBDWGLayers As HashSet(Of String)

		'	If oDataReader IsNot Nothing Then
		'	Do While oDataReader.Read
		'	colDBLayers.Add(oDataReader.GetString(0))
		'	Loop
		'	oDataReader.Close()
		'	End If

		'	hsDBDWGLayers = DMAcadExt.AcadTransaction.GetLayersExist(colDBLayers)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadData", ptMapThemeData.GraphType, colDBLayers.Count, hsDBDWGLayers.Count, "-", "-", "-", "-")
		'DMCommon.Debug.MsgBox("12_317c", mhsDBDWGLayers.Count)

		Dim oaMapThemeInfo As DMAcadExt.MapThemeInfo() = DMAcadExt.AcadTransaction.GetStatistics(ptMapThemeData, mhsDBDWGLayers)
		If True Then
			If oaMapThemeInfo IsNot Nothing Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterStatistics", oaMapThemeInfo.GetUpperBound(0), mhsDBDWGLayers.Count)
				If oaMapThemeInfo.GetUpperBound(0) >= 0 Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterStat_1", oaMapThemeInfo(0).LinkCount)
				End If
			End If

			Dim oDataRow As DataRow
			'Dim oGridRow As DataGridViewRow

			Dim saLayers() As String = mhsDBDWGLayers.ToArray()
			If oaMapThemeInfo IsNot Nothing Then


				moTopoDataTable.Clear()
				For iIndex As Integer = 0 To oaMapThemeInfo.GetUpperBound(0)
					sTopoName = saLayers(iIndex)
					iPgonCount = TopoManager.TopoCreator.GetTopoPolygonCount(sTopoName)

					oDataRow = moTopoDataTable.NewRow()
					oDataRow.Item("TopoName") = sTopoName
					oDataRow.Item("PolygonCount") = iPgonCount
					oDataRow.Item("LinkCount") = oaMapThemeInfo(iIndex).LinkCount
					oDataRow.Item("CentroidCount") = oaMapThemeInfo(iIndex).CentroidBlocksCount

					moTopoDataTable.Rows.Add(oDataRow)
				Next
			End If

		End If
		Me.dgvTopoList.DataSource = moTopoDataTable
		zzSetStyle()
	End Sub
	Private Sub zzSetStyle()
		Dim oDataRow As DataRow
		Dim iPgonCount As Integer
		For iRowIndex As Integer = 0 To moTopoDataTable.Rows.Count - 1
			'oGridRow = dgvTopoList.Rows.Item(iRowIndex)
			oDataRow = moTopoDataTable.Rows.Item(iRowIndex)
			iPgonCount = DMCommon.Functions.CIntN(oDataRow.Item("PolygonCount"))
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadData", moTopoDataTable.Rows.Count, iRowIndex, iPgonCount, moGridCellStyleBold.BackColor, moErrorDefaultCellStyle.BackColor)
			If iPgonCount > 0 Then
				dgvTopoList.Rows.Item(iRowIndex).Cells.Item("ctxTopoName").Style = moGridCellStyleBold
			End If
		Next
	End Sub

	Private Function zzGetCleanupIndex() As Integer
		Select Case LabelCheck.CurrentIndex
			Case 0
				Return 0
			Case 3
				Return 1
			Case 5
				Return 2
		End Select
	End Function

	Private Sub zzInitPanel0_3(iCleanupIndex As Integer)
		Dim iPanelIndex As Integer
		If iCleanupIndex = 0 Then
			iPanelIndex = 0
		Else
			iPanelIndex = 3
		End If
		'	Me.doaPanels(iPanelIndex) = New System.Windows.Forms.Panel()

		Me.doaPanels(iPanelIndex).SuspendLayout()
		'	Me.SuspendLayout()
		Me.dgvActions(iCleanupIndex) = New System.Windows.Forms.DataGridView
		Me.nudSteps(iCleanupIndex) = New System.Windows.Forms.NumericUpDown
		Me.nudErrors(iCleanupIndex) = New System.Windows.Forms.NumericUpDown
		Me.cmdEraseCleanupErr(iCleanupIndex) = New System.Windows.Forms.Button()
		Me.cmdFix(iCleanupIndex) = New TabButton(doLabelFont, False)
		Me.cmdMark(iCleanupIndex) = New TabButton(doLabelFont, False)
		Me.lblErrors(iCleanupIndex) = New System.Windows.Forms.Label



		'
		'moaPanels(iPanelIndex)
		'
		With Me.doaPanels(iPanelIndex).Controls
			.Add(Me.nudSteps(iCleanupIndex))
			.Add(Me.nudErrors(iCleanupIndex))
			.Add(Me.cmdEraseCleanupErr(iCleanupIndex))
			.Add(Me.cmdFix(iCleanupIndex))
			.Add(Me.cmdMark(iCleanupIndex))
			.Add(Me.lblErrors(iCleanupIndex))
			.Add(Me.dgvActions(iCleanupIndex))
		End With




		'
		'dgvActions
		'
		With Me.dgvActions(iCleanupIndex)
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(4, 32)
			.Name = "dgvActions" & CStr(iCleanupIndex)
			.Font = doLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 24
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = doBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(294, 224) '224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False





		End With


		'
		'cmdFix
		'
		With Me.cmdFix(iCleanupIndex)
			.Location = New System.Drawing.Point(48, 4)
			.Name = "cmdFix" & CStr(iCleanupIndex)
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 9
			'   .Font = moLabelFont
			.Text = "Fix"
		End With
		'
		'cmdMark
		'
		With Me.cmdMark(iCleanupIndex)
			.Location = New System.Drawing.Point(96, 4)
			.Name = "cmdMark" & CStr(iCleanupIndex)
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			.Text = "Mark"
		End With

		'
		'lblErrors
		'
		With Me.lblErrors(iCleanupIndex)
			.Location = New System.Drawing.Point(176, 6)
			.Name = "lblErrors" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(44, 24)
			.TabIndex = 29
			.Font = doLabelFont
			.Text = "Errors:"
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With



		'
		'nudSteps
		'
		With Me.nudSteps(iCleanupIndex)
			.Location = New System.Drawing.Point(4, 4)
			.Name = "nudSteps" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(32, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.One
			'.Maximum = Decimal.One + Decimal.One
			.TabIndex = 19
			.Value = Decimal.One
			'	.Maximum = miStepNum
		End With
		'
		'nudErrors
		'
		With Me.nudErrors(iCleanupIndex)
			.Location = New System.Drawing.Point(216, 4)  '232, 4
			.Name = "nudErrors" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(42, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.Zero
			.Maximum = Decimal.Zero
			.TabIndex = 20
		End With
		'
		'cmdEraseCleanupErr
		'
		With Me.cmdEraseCleanupErr(iCleanupIndex)
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
			.Location = New System.Drawing.Point(260, 4)
			.Name = "cmdEraseCleanupErr"
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With
		AddHandler Me.nudSteps(iCleanupIndex).ValueChanged, AddressOf nudSteps_ValueChanged
		AddHandler Me.dgvActions(iCleanupIndex).RowEnter, AddressOf dgvActions_RowEnter
		AddHandler Me.dgvActions(iCleanupIndex).DataError, AddressOf dgvActions_DataError
		AddHandler Me.cmdFix(iCleanupIndex).Click, AddressOf cmdFix_Click
		AddHandler Me.cmdMark(iCleanupIndex).Click, AddressOf cmdMark_Click
		AddHandler Me.nudErrors(iCleanupIndex).ValueChanged, AddressOf nudErrors_ValueChanged
		AddHandler Me.cmdEraseCleanupErr(iCleanupIndex).Click, AddressOf cmdEraseCleanupErr_Click
		Me.doaPanels(iPanelIndex).ResumeLayout(False)


	End Sub

	Private Sub zzInitPanel1()
		'moDataGridViewCellStyleTopoName

		Me.txtPgonCount = New System.Windows.Forms.TextBox()
		Me.lblTopoExists = New System.Windows.Forms.Label()
		Me.cmdStartErr = New System.Windows.Forms.Button()
		Me.cmdPrevErr = New System.Windows.Forms.Button()
		Me.cmdNextErr = New System.Windows.Forms.Button()
		Me.cmdEraseErr = New System.Windows.Forms.Button()

		Me.dgvTopoList = New System.Windows.Forms.DataGridView()

		Me.ctxTopoName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPolygonCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLinkCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxCentroidCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.dgvTopoList, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()

		Me.txtErrorCount = New System.Windows.Forms.TextBox()
		Me.txtErrorIndex = New System.Windows.Forms.TextBox()
		Me.lblTopoName = New System.Windows.Forms.Label()
		Me.lblTopoNameCap = New System.Windows.Forms.Label()
		'''''''''''''''''''''''	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		Me.lblTopoErrors = New System.Windows.Forms.Label()
		Me.grbCentroids = New System.Windows.Forms.GroupBox()
		Me.txtCentroidCount = New System.Windows.Forms.TextBox()
		Me.txtCentroidBlocks = New System.Windows.Forms.TextBox()
		Me.txtCentroidLayers = New System.Windows.Forms.TextBox()
		Me.lblCentroidBlocks = New System.Windows.Forms.Label()
		Me.lblCentroidLayers = New System.Windows.Forms.Label()
		Me.lblNodeBlocks = New System.Windows.Forms.Label()
		Me.grbLinks = New System.Windows.Forms.GroupBox()
		Me.txtLinkCount = New System.Windows.Forms.TextBox()
		Me.txtLinkLayers = New System.Windows.Forms.TextBox()
		Me.lblLinkLayers = New System.Windows.Forms.Label()
		Me.txtNodeBlocks = New System.Windows.Forms.TextBox()
		Me.grbNodes = New System.Windows.Forms.GroupBox()
		Me.txtNodeCount = New System.Windows.Forms.TextBox()
		Me.txtNodeLayers = New System.Windows.Forms.TextBox()
		Me.lblNodeLayers = New System.Windows.Forms.Label()



		Me.txtTolerance = New System.Windows.Forms.TextBox()
		Me.lblTolerance = New System.Windows.Forms.Label()
		Me.chkHighlightSliver = New System.Windows.Forms.CheckBox()
		Me.chkCreateCentroid = New System.Windows.Forms.CheckBox()
		Me.chkCreateNode = New System.Windows.Forms.CheckBox()
		'	CType(Me.dgvTopoList, System.ComponentModel.ISupportInitialize).BeginInit()
		'	Me.SuspendLayout()

		'	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154a")
		If False Then
			Me.doaPanels(1).SuspendLayout()
			Me.grbCentroids.SuspendLayout()
			Me.grbLinks.SuspendLayout()
			Me.SuspendLayout()
		End If

		'
		'PanelX1
		'
		With Me.doaPanels(1).Controls
			'	.RightToLeft = Windows.Forms.RightToLeft.No
			'	.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle



			.Add(Me.cmdStartErr)
			.Add(Me.cmdPrevErr)
			.Add(Me.cmdNextErr)
			.Add(Me.cmdEraseErr)

			.Add(Me.txtErrorCount)
			.Add(Me.txtErrorIndex)

			'	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154b")
			.Add(Me.tstTopology)
			.Add(Me.lblTopoErrors)
			If False Then
				.Add(Me.lblTopoName)
				.Add(Me.lblTopoNameCap)
				.Add(Me.lblTopoExists)
				.Add(Me.txtPgonCount)
				.Add(Me.grbCentroids)
				.Add(Me.grbLinks)
				.Add(Me.grbNodes)
			End If

			.Add(Me.dgvTopoList)
			.Add(Me.txtTolerance)
			.Add(Me.lblTolerance)
			.Add(Me.chkHighlightSliver)
			.Add(Me.chkCreateCentroid)
			.Add(Me.chkCreateNode)



		End With
		'
		'dgvTopoList
		'
		Me.dgvTopoList.AllowUserToAddRows = False
		Me.dgvTopoList.AllowUserToDeleteRows = False
		Me.dgvTopoList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells
		Me.dgvTopoList.BackgroundColor = System.Drawing.SystemColors.Control
		Me.dgvTopoList.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.dgvTopoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvTopoList.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxTopoName, Me.ctxPolygonCount, Me.ctxLinkCount, Me.ctxCentroidCount})
		Me.dgvTopoList.Location = New System.Drawing.Point(8, 72)
		Me.dgvTopoList.MultiSelect = True

		Me.dgvTopoList.Name = "dgvTopoList"
		Me.dgvTopoList.ReadOnly = True
		Me.dgvTopoList.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.dgvTopoList.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
		Me.dgvTopoList.RowHeadersVisible = True
		Me.dgvTopoList.RowHeadersWidth = 12 '23
		Me.dgvTopoList.Size = New System.Drawing.Size(292, 208) '(478, 330)
		Me.dgvTopoList.TabIndex = 8
		Me.dgvTopoList.ScrollBars = ScrollBars.Both
		Me.dgvTopoList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
		moDataGridViewCellStyleTopoName.ForeColor = System.Drawing.SystemColors.GrayText
		moDataGridViewCellStyleTopoName.Font = New System.Drawing.Font("Segoe UI Light", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))



		moGridCellStyleBold = Me.dgvTopoList.DefaultCellStyle.Clone


		moGridCellStyleBold.Font = New System.Drawing.Font("Segoe UI Semibold", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		'moGridCellStyleBold.BackColor = Color.Blue
		'	moGridCellStyleBold.ForeColor = Color.Green

		moDefaultCellStyle = Me.dgvTopoList.DefaultCellStyle


		moErrorDefaultCellStyle = moDefaultCellStyle.Clone
		moErrorDefaultCellStyle.BackColor = Color.Red
		moErrorDefaultCellStyle.SelectionBackColor = Color.DarkRed
		'
		'ctxTopoName
		'
		Me.ctxTopoName.DataPropertyName = "TopoName"
		Me.ctxTopoName.HeaderText = "TopoName/Layer"
		Me.ctxTopoName.Name = "ctxTopoName"
		Me.ctxTopoName.ReadOnly = True
		Me.ctxTopoName.Width = 128
		Me.ctxTopoName.DefaultCellStyle = moDataGridViewCellStyleTopoName
		Me.ctxTopoName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		'
		'ctxPolygonCount
		'
		Me.ctxPolygonCount.DataPropertyName = "PolygonCount"
		Me.ctxPolygonCount.HeaderText = "Pgons"
		Me.ctxPolygonCount.Name = "ctxPolygonCount"
		Me.ctxPolygonCount.ReadOnly = True
		Me.ctxPolygonCount.Width = 46
		Me.ctxPolygonCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		'
		'ctxLinkCount
		'
		Me.ctxLinkCount.DataPropertyName = "LinkCount"
		Me.ctxLinkCount.HeaderText = "Links"
		Me.ctxLinkCount.Name = "ctxLinkCount"
		Me.ctxLinkCount.ReadOnly = True
		Me.ctxLinkCount.Width = 42
		Me.ctxLinkCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		'
		'ctxCentroidCount
		'
		Me.ctxCentroidCount.DataPropertyName = "CentroidCount"
		Me.ctxCentroidCount.FillWeight = 60.0!
		Me.ctxCentroidCount.HeaderText = "Blocks"
		Me.ctxCentroidCount.Name = "ctxCentroidCount"
		Me.ctxCentroidCount.ReadOnly = True
		Me.ctxCentroidCount.Width = 46
		Me.ctxCentroidCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable

		'
		'txtPgonCount
		'
		With Me.txtPgonCount
			.Location = New System.Drawing.Point(252, 78)
			.Name = "txtPgonCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 20
			.RightToLeft = Windows.Forms.RightToLeft.No
			.BackColor = Color.Aqua
		End With
		'
		'lblTopoExists
		'
		With Me.lblTopoExists
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(8, 76)
			.Name = "lblTopoExists"
			.Size = New System.Drawing.Size(20, 18)
			.TabIndex = 19
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'cmdStartErr
		'
		With Me.cmdStartErr
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveFirstTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(152, 284)
			.Name = "cmdStartErr"
			.Size = New System.Drawing.Size(20, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'cmdPrevErr
		'
		With Me.cmdPrevErr
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MovePrevTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(174, 284)
			.Name = "cmdPrevErr"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 17
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'cmdNextErr
		'
		With Me.cmdNextErr
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveNextTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(192, 284)
			.Name = "cmdNextErr"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 18
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'cmdEraseErr
		'
		With Me.cmdEraseErr
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(216, 284)
			.Name = "cmdEraseErr"
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'txtErrorCount
		'
		With Me.txtErrorCount
			.Location = New System.Drawing.Point(102, 284)
			.Name = "txtErrorCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(42, 22)
			.TabIndex = 15
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'txtErrorIndex
		'
		With Me.txtErrorIndex
			.Location = New System.Drawing.Point(60, 284)
			.Name = "txtErrorIndex"
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 14
			.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
		End With
		'
		'lblTopoName
		'
		With Me.lblTopoName
			.Location = New System.Drawing.Point(126, 78)
			.Name = "lblTopoName"
			.Size = New System.Drawing.Size(120, 18)
			.Text = ptMapThemeData.TopoName
			.TabIndex = 13
			.BackColor = Color.Aqua
		End With
		'
		'lblTopoNameCap
		'
		With Me.lblTopoNameCap
			.Location = New System.Drawing.Point(28, 78)
			.Name = "lblTopoNameCap"
			.Size = New System.Drawing.Size(96, 18)
			.TabIndex = 12
			.Text = "Topology name:"
			.RightToLeft = Windows.Forms.RightToLeft.No
			.BackColor = Color.Aqua
		End With
		'
		'tstTopology
		'
		With Me.tstTopology
			.Location = New System.Drawing.Point(0, 0)
			.Name = "tstTopology"
			.Size = New System.Drawing.Size(300, 25)
			.TabIndex = 11
		End With
		'
		'lblTopoErrors
		'
		With Me.lblTopoErrors
			.Location = New System.Drawing.Point(8, 284)
			.Name = "lblTopoErrors"
			.Size = New System.Drawing.Size(50, 18)
			.TabIndex = 9
			.Text = "Errors:"
		End With
		'
		'grbCentroids
		'
		With Me.grbCentroids
			.Controls.Add(Me.txtCentroidCount)
			.Controls.Add(Me.txtCentroidBlocks)
			.Controls.Add(Me.txtCentroidLayers)
			.Controls.Add(Me.lblCentroidBlocks)
			.Controls.Add(Me.lblCentroidLayers)
			.Location = New System.Drawing.Point(8, 144)
			.Name = "grbCentroids"
			.Size = New System.Drawing.Size(284, 64)
			.TabIndex = 8
			.TabStop = False
			.Text = "Centroids"
			.BackColor = Color.Aqua
		End With
		'
		'txtCentroidCount
		'
		With Me.txtCentroidCount
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtCentroidCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 19
			.BackColor = Color.Aqua
		End With
		'
		'txtCentroidBlocks
		'
		With Me.txtCentroidBlocks
			.Location = New System.Drawing.Point(99, 39)
			.Name = "txtCentroidBlocks"
			.ReadOnly = True
			.Size = New System.Drawing.Size(135, 22)
			.Text = ptMapThemeData.CentroidBlocks
			.TabIndex = 18
			.BackColor = Color.Aqua
		End With
		'
		'txtCentroidLayers
		'
		With Me.txtCentroidLayers
			.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtCentroidLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.CentroidLayers
			.TabIndex = 17
			.BackColor = Color.Aqua
		End With
		'
		'lblCentroidBlocks
		'
		With Me.lblCentroidBlocks
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblCentroidBlocks"
			.Size = New System.Drawing.Size(78, 18)
			.TabIndex = 7
			.Text = "Block names:"
			.BackColor = Color.Aqua
		End With
		'
		'lblCentroidLayers
		'
		With Me.lblCentroidLayers
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblCentroidLayers"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.BackColor = Color.Aqua
		End With
		'
		'grbLinks
		'
		With Me.grbLinks
			.Controls.Add(Me.txtLinkCount)
			.Controls.Add(Me.txtLinkLayers)
			.Controls.Add(Me.lblLinkLayers)
			.Location = New System.Drawing.Point(8, 96) '!!!!!!!!!!!!!!!!!!!!!!
			.Name = "grbLinks"
			.Size = New System.Drawing.Size(284, 43)
			.TabIndex = 7
			.TabStop = False
			.Text = "Links"
			.BackColor = Color.Aqua
		End With
		'
		'txtLinkCount
		'
		With Me.txtLinkCount
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtLinkCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
			.BackColor = Color.Aqua
		End With
		'
		'txtLinkLayers
		'
		With Me.txtLinkLayers
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtLinkLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.LinkLayers
			.TabIndex = 16
			.BackColor = Color.Aqua
		End With
		'
		'lblLinkLayers
		'
		With Me.lblLinkLayers
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblLinkLayers"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.BackColor = Color.Aqua
		End With
		'
		'grbClosedPgons
		'
		With Me.grbNodes
			.Controls.Add(Me.txtNodeCount)
			.Controls.Add(Me.txtNodeLayers)
			.Controls.Add(Me.lblNodeLayers)
			.Controls.Add(Me.lblNodeBlocks)
			.Controls.Add(Me.txtNodeBlocks)


			.Location = New System.Drawing.Point(8, 213)
			.Name = "grbNodes"
			.Size = New System.Drawing.Size(284, 43 + 24)
			.TabIndex = 17
			.TabStop = False
			.Text = "Nodes"
			.BackColor = Color.Aqua
		End With
		'
		'txtClosedPgonsCount
		'
		With Me.txtNodeCount
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtClosedPgonsCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
			.BackColor = Color.Aqua
		End With
		'
		'txtNodeLayers
		'
		With Me.txtNodeLayers
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtNodeLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.NodeLayers
			.TabIndex = 16
			.BackColor = Color.Aqua
		End With

		'
		'lblNodeLayers
		'
		With Me.lblNodeLayers
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblNodeLayers"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.BackColor = Color.Aqua
		End With
		'
		'lblNodeBlocks
		'
		With Me.lblNodeBlocks
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblNodeBlocks"
			.Size = New System.Drawing.Size(78, 18)
			.TabIndex = 7
			.Text = "Block names:"
			.BackColor = Color.Aqua
		End With
		'
		'txtNodeBlocks
		'
		With Me.txtNodeBlocks
			.Location = New System.Drawing.Point(99, 42)
			.Name = "txtNodeBlocks"
			.ReadOnly = True
			.Size = New System.Drawing.Size(135, 22)
			.Text = ptMapThemeData.NodeBlocks
			.TabIndex = 23
			.BackColor = Color.Aqua
		End With

		'
		'txtTolerance
		'
		With Me.txtTolerance
			.Location = New System.Drawing.Point(240, 30)
			.Name = "txtTolerance"
			.Size = New System.Drawing.Size(52, 22)
			.TabIndex = 3
			.Text = "0.01"
		End With
		'
		'lblTolerance
		'
		With Me.lblTolerance
			.Location = New System.Drawing.Point(170, 32)
			.Name = "lblTolerance"
			.Size = New System.Drawing.Size(68, 18)
			.TabIndex = 2
			.Text = "Tolerance"
		End With
		'
		'chkHighlightSliver
		'
		With Me.chkHighlightSliver
			.AutoSize = True
			.Location = New System.Drawing.Point(170, 52)
			.Name = "chkHighlightSliver"
			.Size = New System.Drawing.Size(105, 18)
			.TabIndex = 1
			.Text = "Highlight Sliver"
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With

		'
		'chkCreateCentroid
		'
		With Me.chkCreateCentroid
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 32)
			.Name = "chkCreateCentroid"
			.Size = New System.Drawing.Size(108, 18)
			.TabIndex = 0
			.Text = "Insert Centroid"
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With

		'
		'chkCreateNode
		'
		With Me.chkCreateNode
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 52)
			.Name = "chkCreateNode"
			.Size = New System.Drawing.Size(108, 18)
			.TabIndex = 0
			.Text = "Insert Node"
			.Enabled = ptMapThemeData.HasNodes
			.Checked = True
			.UseVisualStyleBackColor = True
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		CType(Me.dgvTopoList, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()



		'''''''''''''''''		Me.Controls.Add(Me.doaPanels(1))


	End Sub

	Private Sub zzInitPanel2()
		Me.grbSource = New System.Windows.Forms.GroupBox()
		Me.grbWithoutArcs = New System.Windows.Forms.GroupBox()

		Me.lblLinkLayersP2 = New System.Windows.Forms.Label()
		Me.txtLinkLayersP2 = New System.Windows.Forms.TextBox()
		Me.txtLinkCountP2 = New System.Windows.Forms.TextBox()

		Me.lblLinkWithArcCount = New System.Windows.Forms.Label
		Me.txtLinkWithArcCount = New System.Windows.Forms.TextBox
		Me.lblArcCount = New System.Windows.Forms.Label
		Me.txtArcCount = New System.Windows.Forms.TextBox


		Me.lblLineLinksExist = New System.Windows.Forms.Label()
		Me.lblLineLinkLayer = New System.Windows.Forms.Label()
		Me.txtLineLinkLayer = New System.Windows.Forms.TextBox()
		Me.txtLineLinkCount = New System.Windows.Forms.TextBox()

		Me.txtStraightenTolerance = New System.Windows.Forms.TextBox()
		Me.lblStraightenTolerance = New System.Windows.Forms.Label()
		Me.cmdStraightenLink = New System.Windows.Forms.Button()
		Me.cmdInsertBreakPoint = New System.Windows.Forms.Button()



		Me.grbSource.SuspendLayout()
		Me.grbWithoutArcs.SuspendLayout()
		Me.doaPanels(2).SuspendLayout()
		Me.SuspendLayout()


		'
		'grbSource
		'
		With Me.grbSource
			With .Controls


				.Add(Me.lblLinkLayersP2)
				.Add(Me.txtLinkLayersP2)
				.Add(Me.txtLinkCountP2)

				.Add(Me.lblLinkWithArcCount)
				.Add(Me.txtLinkWithArcCount)
				.Add(Me.lblArcCount)
				.Add(Me.txtArcCount)

			End With

			.Location = New System.Drawing.Point(8, 32)
			.Name = "grbSource"
			.Size = New System.Drawing.Size(284, 88)
			.TabIndex = 7
			.TabStop = False
			.Text = "מקור"
		End With

		'
		'grbWithoutArcs
		'
		With Me.grbWithoutArcs
			.Controls.Add(Me.lblLineLinksExist)
			.Controls.Add(Me.lblLineLinkLayer)
			.Controls.Add(Me.txtLineLinkLayer)
			.Controls.Add(Me.txtLineLinkCount)
			.Controls.Add(Me.txtStraightenTolerance)
			.Controls.Add(Me.lblStraightenTolerance)





			.Location = New System.Drawing.Point(8, 132)
			.Name = "grbWithoutArcs"
			.Size = New System.Drawing.Size(284, 86)
			.TabIndex = 7
			.TabStop = False
			.Text = "ללא קשתות"
		End With
		'
		'lblLinkLayersP2
		'
		With Me.lblLinkLayersP2
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblLinkLayersP2"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
		End With
		'
		'txtLinkLayersP2
		'
		With Me.txtLinkLayersP2
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtLinkLayersP2"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.LinkLayers
			.TabIndex = 16
		End With
		'
		'txtLinkCountP2
		'
		With Me.txtLinkCountP2
			.Location = New System.Drawing.Point(238, 15) ''''''''''''''''''''''''''''''''''''''
			.Name = "txtLinkCountP2"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'lblLinkWithArcCount
		'
		With Me.lblLinkWithArcCount
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblLinkLayersP2"
			.Size = New System.Drawing.Size(120, 18)
			.TabIndex = 6
			.Text = "Arc Entities Count:"
		End With
		'
		'txtLinkWithArcCount
		'
		With Me.txtLinkWithArcCount
			.Location = New System.Drawing.Point(238, 39)  ''''''''''''''''''''''''''''''''''''''
			.Name = "txtLinkWithArcCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			'.TabIndex = 17
		End With

		'
		'lblArcCount
		'
		With Me.lblArcCount
			.Location = New System.Drawing.Point(20, 66)
			.Name = "lblArcCount"
			.Size = New System.Drawing.Size(120, 18)
			.TabIndex = 6
			.Text = "Arc Count:"
		End With

		''
		'txtArcCount
		'
		With Me.txtArcCount
			.Location = New System.Drawing.Point(238, 63)
			.Name = "txtArcCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With

		'''''''''''''''''''''''''''''''''''''''''
		'
		'lblLineLinksExist
		'
		With Me.lblLineLinksExist
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(1, 42)           '18
			.Name = "lblLineLinksExist"
			.Size = New System.Drawing.Size(20, 18)
			.TabIndex = 6
		End With
		'
		'lblLinkLineLayer
		'
		With Me.lblLineLinkLayer
			.Location = New System.Drawing.Point(20, 42)           '18
			.Name = "lblLinkLineLayer"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layer:"
		End With




		'
		'txtLinkLineLayer
		'
		With Me.txtLineLinkLayer
			.Location = New System.Drawing.Point(66, 39)  '15
			.Name = "txtLinkLineLayer"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.LineLinkLayers
			.TabIndex = 16
		End With
		'
		'txtLinkLineCount
		'
		With Me.txtLineLinkCount
			.Location = New System.Drawing.Point(238, 39)  '15
			.Name = "txtLinkLineCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtStraightenTolerance
		'
		With Me.txtStraightenTolerance
			.BackColor = System.Drawing.SystemColors.Window
			.Location = New System.Drawing.Point(238, 15)  ' 39
			.Name = "txtStraightenTolerance"
			.ReadOnly = False
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 18
		End With
		'
		'lblStraightenTolerance
		'
		With Me.lblStraightenTolerance
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblStraightenTolerance"
			.Size = New System.Drawing.Size(72, 18)
			.TabIndex = 6
			.Text = "Tolerance:"
		End With
		'
		'cmdStraightenLink
		'
		With Me.cmdStraightenLink

			.Location = New System.Drawing.Point(16, 228)
			.Name = "cmdStraightenLink"
			.Size = New System.Drawing.Size(80, 22)
			.TabIndex = 17
			.UseVisualStyleBackColor = True
			.Text = "יישור קו"
		End With
		'
		'cmdInsertBreakPoint
		'
		With Me.cmdInsertBreakPoint
			.Location = New System.Drawing.Point(144, 228)
			.Name = "cmdInsertBreakPoint"
			.Size = New System.Drawing.Size(80, 22)
			.TabIndex = 18
			.UseVisualStyleBackColor = True
			.Text = "הוסף נקודה"
		End With



		'''''''''''''''''''''''''''''''''''''''''''''''

		'
		'Panel2
		'
		With Me.doaPanels(2)
			.Controls.Add(Me.grbSource)
			.Controls.Add(Me.grbWithoutArcs)
			.Controls.Add(Me.tstTopology)
			.Controls.Add(Me.cmdStraightenLink)
			.Controls.Add(Me.cmdInsertBreakPoint)

			.Location = New System.Drawing.Point(0, 0)
		End With


		Me.grbSource.ResumeLayout(False)
		Me.grbSource.PerformLayout()
		Me.grbWithoutArcs.ResumeLayout(False)
		Me.grbWithoutArcs.PerformLayout()
		Me.doaPanels(2).ResumeLayout(False)
		Me.doaPanels(2).PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()


	End Sub

	Private Sub zzInitPanel4()

		Me.txtPgonCountWA = New System.Windows.Forms.TextBox()
		Me.lblTopoExistsWA = New System.Windows.Forms.Label()
		Me.cmdStartErrWA = New System.Windows.Forms.Button()
		Me.cmdPrevErrWA = New System.Windows.Forms.Button()
		Me.cmdNextErrWA = New System.Windows.Forms.Button()
		Me.cmdEraseErrWA = New System.Windows.Forms.Button()

		Me.txtErrorCountWA = New System.Windows.Forms.TextBox()
		Me.txtErrorIndexWA = New System.Windows.Forms.TextBox()
		Me.lblTopoNameWA = New System.Windows.Forms.Label()
		Me.lblTopoNameCapWA = New System.Windows.Forms.Label()
		'	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		Me.lblTopoErrorsWA = New System.Windows.Forms.Label()
		Me.grbCentroidsWA = New System.Windows.Forms.GroupBox()
		Me.txtCentroidCountWA = New System.Windows.Forms.TextBox()
		Me.txtCentroidBlocksWA = New System.Windows.Forms.TextBox()
		Me.txtCentroidLayersWA = New System.Windows.Forms.TextBox()
		Me.lblCentroidBlocksWA = New System.Windows.Forms.Label()
		Me.lblCentroidLayersWA = New System.Windows.Forms.Label()
		Me.grbLinksWA = New System.Windows.Forms.GroupBox()

		Me.txtLinkCountWA = New System.Windows.Forms.TextBox()
		Me.txtLinkLayersWA = New System.Windows.Forms.TextBox()
		Me.lblLinkLayersWA = New System.Windows.Forms.Label()

		Me.txtLineLinkCountB = New System.Windows.Forms.TextBox()
		Me.txtLineLinkLayers = New System.Windows.Forms.TextBox()
		Me.lblLineLinkLayers = New System.Windows.Forms.Label()

		Me.grbClosedPgonsWA = New System.Windows.Forms.GroupBox()
		Me.txtClosedPgonsCountWA = New System.Windows.Forms.TextBox()
		Me.txtClosedPgonsLayersWA = New System.Windows.Forms.TextBox()
		Me.lblClosedPgonsLayersWA = New System.Windows.Forms.Label()

		Me.txtToleranceWA = New System.Windows.Forms.TextBox()
		Me.lblToleranceWA = New System.Windows.Forms.Label()
		Me.chkHighlightSliverWA = New System.Windows.Forms.CheckBox()
		Me.chkCreateCentroidWA = New System.Windows.Forms.CheckBox()
		Me.doaPanels(4).SuspendLayout()
		Me.grbCentroidsWA.SuspendLayout()
		Me.grbLinksWA.SuspendLayout()
		Me.SuspendLayout()
		'
		'PanelX1
		'
		With Me.doaPanels(4).Controls
			.Add(Me.txtPgonCountWA)
			.Add(Me.lblTopoExistsWA)
			.Add(Me.cmdStartErrWA)
			.Add(Me.cmdPrevErrWA)
			.Add(Me.cmdNextErrWA)
			.Add(Me.cmdEraseErrWA)

			.Add(Me.txtErrorCountWA)
			.Add(Me.txtErrorIndexWA)
			.Add(Me.lblTopoNameWA)
			.Add(Me.lblTopoNameCapWA)
			.Add(Me.tstTopology)
			.Add(Me.lblTopoErrorsWA)
			.Add(Me.grbLinksWA)
			.Add(Me.grbCentroidsWA)
			.Add(Me.grbClosedPgonsWA)
			.Add(Me.txtToleranceWA)
			.Add(Me.lblToleranceWA)
			.Add(Me.chkHighlightSliverWA)
			.Add(Me.chkCreateCentroidWA)
		End With
		'
		'txtPgonCountWA
		'
		With Me.txtPgonCountWA
			.Location = New System.Drawing.Point(252, 78)
			.Name = "txtPgonCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 20
		End With
		'
		'lblTopoExistsWA
		'
		With Me.lblTopoExistsWA
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(8, 76)
			.Name = "lblTopoExistsWA"
			.Size = New System.Drawing.Size(20, 18)
			.TabIndex = 19
		End With
		'
		'cmdStartErrWA
		'
		With Me.cmdStartErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveFirstTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(152, 238 + 48)
			.Name = "cmdStartErrWA"
			.Size = New System.Drawing.Size(20, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdPrevErrWA
		'
		With Me.cmdPrevErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MovePrevTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(174, 238 + 48)
			.Name = "cmdPrevErrWA"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 17
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdNextErrWA
		'
		With Me.cmdNextErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveNextTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(192, 238 + 48)
			.Name = "cmdNextErrWA"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 18
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdEraseErrWA
		'
		With Me.cmdEraseErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(216, 238 + 48)
			.Name = "cmdEraseErrWA"
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With
		'
		'txtErrorCountWA
		'
		With Me.txtErrorCountWA
			.Location = New System.Drawing.Point(102, 238 + 48)
			.Name = "txtErrorCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(42, 22)
			.TabIndex = 15
		End With
		'
		'txtErrorIndexWA
		'
		With Me.txtErrorIndexWA
			.Location = New System.Drawing.Point(60, 238 + 48)
			.Name = "txtErrorIndexWA"
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 14
			.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
		End With
		'
		'lblTopoNameWA
		'
		With Me.lblTopoNameWA
			.Location = New System.Drawing.Point(126, 78)
			.Name = "lblTopoNameWA"
			.Size = New System.Drawing.Size(120, 18)
			.Text = ptMapThemeData.LineTopoName
			.TabIndex = 13
		End With
		'
		'lblTopoNameCapWA
		'
		With Me.lblTopoNameCapWA
			.Location = New System.Drawing.Point(28, 78)
			.Name = "lblTopoNameCapWA"
			.Size = New System.Drawing.Size(96, 18)
			.TabIndex = 12
			.Text = "Topology name:"
		End With

		'
		'lblTopoErrorsWA
		'
		With Me.lblTopoErrorsWA
			.Location = New System.Drawing.Point(8, 238 + 48 + 24)    '
			.Name = "lblTopoErrorsWA"
			.Size = New System.Drawing.Size(50, 18)
			.TabIndex = 9
			.Text = "Errors:"
			.BorderStyle = BorderStyle.FixedSingle
		End With
		'
		'grbCentroidsWA
		'
		With Me.grbCentroidsWA
			.Controls.Add(Me.txtCentroidCountWA)
			.Controls.Add(Me.txtCentroidBlocksWA)
			.Controls.Add(Me.txtCentroidLayersWA)
			.Controls.Add(Me.lblCentroidBlocksWA)
			.Controls.Add(Me.lblCentroidLayersWA)
			.Location = New System.Drawing.Point(8, 168)
			.Name = "grbCentroidsWA"
			.Size = New System.Drawing.Size(284, 64)
			.TabIndex = 8
			.TabStop = False
			.Text = "Centroids"
		End With
		'
		'txtCentroidCountWA
		'
		With Me.txtCentroidCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtCentroidCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 19
		End With
		'
		'txtCentroidBlocksWA
		'
		With Me.txtCentroidBlocksWA
			.Location = New System.Drawing.Point(99, 39)
			.Name = "txtCentroidBlocksWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(135, 22)
			.Text = ptMapThemeData.CentroidBlocks
			.TabIndex = 18
		End With
		'
		'txtCentroidLayersWA
		'
		With Me.txtCentroidLayersWA
			.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtCentroidLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.CentroidLayers
			.TabIndex = 17
		End With
		'
		'lblCentroidBlocksWA
		'
		With Me.lblCentroidBlocksWA
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblCentroidBlocksWA"
			.Size = New System.Drawing.Size(78, 18)
			.TabIndex = 7
			.Text = "Block names:"
		End With
		'
		'lblCentroidLayersWA
		'
		With Me.lblCentroidLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblCentroidLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
		End With
		'
		'grbLinksWA
		'
		With Me.grbLinksWA
			.Controls.Add(Me.txtLinkCountWA)
			.Controls.Add(Me.txtLinkLayersWA)
			.Controls.Add(Me.lblLinkLayersWA)
			.Controls.Add(Me.txtLineLinkCountB)
			.Controls.Add(Me.txtLineLinkLayers)
			.Controls.Add(Me.lblLineLinkLayers)

			.Location = New System.Drawing.Point(8, 96)
			.Name = "grbLinksWA"
			.Size = New System.Drawing.Size(284, 67)
			.TabIndex = 7
			.TabStop = False
			.Text = "Links"
		End With
		'
		'txtLinkCountWA
		'
		With Me.txtLinkCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtLinkCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtLinkLayersWA
		'
		With Me.txtLinkLayersWA
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtLinkLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.LinkLayers
			.TabIndex = 16
		End With
		'
		'lblLinkLayersWA
		'
		With Me.lblLinkLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblLinkLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
		End With

		'
		'txtLineLinkCountB
		'
		With Me.txtLineLinkCountB
			.Location = New System.Drawing.Point(238, 39)
			.Name = "txtLineLinkCountB"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtLineLinkLayers
		'
		With Me.txtLineLinkLayers
			.Location = New System.Drawing.Point(84, 39)
			.Name = "txtLineLinkLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(160, 46)
			.Text = ptMapThemeData.LineLinkLayers
			.TabIndex = 16
		End With
		'
		'lblLineLinkLayers
		'
		With Me.lblLineLinkLayers
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblLineLinkLayers"
			.Size = New System.Drawing.Size(66, 18)
			.TabIndex = 6
			.Text = "Line Layer:"
		End With
		''''''''''''''''''''''''''''''''''''''''''''''''''
		'
		'grbClosedPgonsWA
		'
		With Me.grbClosedPgonsWA
			.Controls.Add(Me.txtClosedPgonsCountWA)
			.Controls.Add(Me.txtClosedPgonsLayersWA)
			.Controls.Add(Me.lblClosedPgonsLayersWA)
			.Location = New System.Drawing.Point(8, 213 + 24)
			.Name = "grbClosedPgonsWA"
			.Size = New System.Drawing.Size(284, 43)
			.TabIndex = 17
			.TabStop = False
			.Text = "Closed polygons"
		End With
		'
		'txtClosedPgonsCountWA
		'
		With Me.txtClosedPgonsCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtClosedPgonsCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtClosedPgonsLayersWA
		'
		With Me.txtClosedPgonsLayersWA
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtClosedPgonsLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.ClosedPgonsLayers
			.TabIndex = 16
		End With
		'
		'lblClosedPgonsLayersWA
		'
		With Me.lblClosedPgonsLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblClosedPgonsLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
		End With

		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		'
		'txtToleranceWA
		'
		With Me.txtToleranceWA
			.Location = New System.Drawing.Point(240, 32)
			.Name = "txtToleranceWA"
			.Size = New System.Drawing.Size(52, 22)
			.TabIndex = 3
			.Text = "0.01"
		End With
		'
		'lblToleranceWA
		'
		With Me.lblToleranceWA
			.Location = New System.Drawing.Point(170, 32)
			.Name = "lblToleranceWA"
			.Size = New System.Drawing.Size(68, 18)
			.TabIndex = 2
			.Text = "Tolerance"
		End With
		'
		'chkHighlightSliverWA
		'
		With Me.chkHighlightSliverWA
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 52)
			.Name = "chkHighlightSliverWA"
			.Size = New System.Drawing.Size(105, 18)
			.TabIndex = 1
			.Text = "Highlight Sliver"
			.UseVisualStyleBackColor = True
		End With
		'
		'chkCreateCentroidWA
		'

		With Me.chkCreateCentroidWA
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 32)
			.Name = "chkCreateCentroidWA"
			.Size = New System.Drawing.Size(108, 18)
			.TabIndex = 0
			.Text = "Insert Centroid"
			.UseVisualStyleBackColor = True
		End With
		Me.doaPanels(4).ResumeLayout(False)
		Me.doaPanels(4).PerformLayout()
		Me.grbCentroidsWA.ResumeLayout(False)
		Me.grbCentroidsWA.PerformLayout()
		Me.grbLinksWA.ResumeLayout(False)
		Me.grbLinksWA.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Private Sub zzInitPanel5()
		Dim iPanelIndex As Integer = 5
		Dim oTxtColumn As DataGridViewTextBoxColumn
		Dim oChkColumn As DataGridViewCheckBoxColumn
		'	Me.doaPanels(iPanelIndex) = New System.Windows.Forms.Panel()
		moPrjThemesResources = New System.ComponentModel.ComponentResourceManager(GetType(frmPrjThemes))
		Me.doaPanels(iPanelIndex).SuspendLayout()
		'	Me.SuspendLayout()
		Me.dgvCheckActions = New System.Windows.Forms.DataGridView

		'   Me.cmdEraseCleanupErr(iCleanupIndex) = New System.Windows.Forms.Button()
		'   Me.cmdFix(iCleanupIndex) = New TabButton(doLabelFont, False)
		'  Me.cmdMark(iCleanupIndex) = New TabButton(doLabelFont, False)
		'    Me.lblErrors(iCleanupIndex) = New System.Windows.Forms.Label
		'AddHandler nudSteps(iCleanupIndex).ValueChanged, AddressOf nudSteps_ValueChanged
		'AddHandler dgvActions(iCleanupIndex).RowEnter, AddressOf dgvActions_RowEnter
		'AddHandler dgvActions(iCleanupIndex).DataError, AddressOf dgvActions_DataError
		'AddHandler Me.cmdFix(iCleanupIndex).Click, AddressOf cmdFix_Click
		'AddHandler Me.cmdMark(iCleanupIndex).Click, AddressOf cmdMark_Click
		'AddHandler Me.nudErrors(iCleanupIndex).ValueChanged, AddressOf nudErrors_ValueChanged
		'AddHandler Me.cmdEraseCleanupErr(iCleanupIndex).Click, AddressOf cmdEraseCleanupErr_Click


		'
		'moaPanels(iPanelIndex)
		'





		'
		'dgvCheckActions
		'
		With Me.dgvCheckActions
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(4, 32)
			.Name = "dgvCheckActions"
			.Font = doLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 23
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = doBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(294, 228) '224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False
			.AllowUserToAddRows = False
			.AllowUserToDeleteRows = False
		End With

		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oTxtColumn
			.Name = msToleranceFldName
			.HeaderText = "שם בדיקה"
			.Width = 160
			.Name = "ctxActionName"
			.DataPropertyName = "ActionName"
			.ReadOnly = True
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvCheckActions.Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		oChkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn(False)
		With oChkColumn
			.Name = msCheckErrorFldName
			.HeaderText = "שג'?"
			.Width = 38
			.Name = "cchError"
			.DataPropertyName = "Exec"
			.SortMode = DataGridViewColumnSortMode.NotSortable
			.ReadOnly = True
		End With

		Try
			Me.dgvCheckActions.Columns.Add(oChkColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oTxtColumn
			.Name = msToleranceFldName
			.HeaderText = "שגיאות"
			.Width = 50
			.Name = "ctxErrNumber"
			.DataPropertyName = "ErrNumber"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvCheckActions.Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try



		Me.cmdExec = New TabButton(doLabelFont, False)

		'
		' cmdExec 
		'
		With Me.cmdExec
			.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
			.Location = New System.Drawing.Point(4, 4)
			.Name = "cmdExec"
			.Size = New System.Drawing.Size(28, 24)
			.TabIndex = 9
			'   .Font = moLabelFont
			'  .Text = "בצע"
		End With

		Me.cmdFixCheck = New TabButton(doLabelFont, False)
		'
		'cmdFixCheck
		'
		With Me.cmdFixCheck
			.Image = Global.TopoUI.My.Resources.Resources.OK16
			.Location = New System.Drawing.Point(32, 4)
			.Name = "cmdFixCheck"
			.Size = New System.Drawing.Size(28, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			'  .Text = "FixCheck"
		End With


		Me.cmdClear = New TabButton(doLabelFont, False)
		'
		'cmdClear
		'
		With Me.cmdClear
			.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
			.Location = New System.Drawing.Point(68, 4)
			.Name = "cmdClear"
			.Size = New System.Drawing.Size(28, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			'  .Text = "Clear"
		End With
		Me.cmdMessages = New TabButton(doLabelFont, False)
		'
		'cmdMessages
		'
		With Me.cmdMessages
			If moPrjThemesResources IsNot Nothing Then
				.Image = CType(moPrjThemesResources.GetObject("tsbMessages.Image"), System.Drawing.Image)
			Else
				DMCommon.Debug.MsgBox("12_316", "?????????")
			End If

			.Location = New System.Drawing.Point(100, 4)
			.Name = "cmdMessages"
			.Size = New System.Drawing.Size(28, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			'  .Text = "Clear"
		End With

		Me.cmdDispTable = New TabButton(doLabelFont, False)
		'
		'cmdDispTable
		'
		With Me.cmdDispTable

			.Image = Global.TopoUI.My.Resources.Resources.Form15


			.Location = New System.Drawing.Point(132, 4)
			.Name = "cmdDispTable"
			.Size = New System.Drawing.Size(28, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			'  .Text = "Clear"
		End With



		'  = CType(resources.GetObject("tsbMessages.Image"), System.Drawing.Image)
		'    CType(resources.GetObject("tsbUpdate.Image"), System.Drawing.Image)
		Me.txtDescription = New System.Windows.Forms.TextBox()

		'
		'txtDescription
		'
		With Me.txtDescription
			.Location = New System.Drawing.Point(4, 264)
			.Name = "txtDescription"
			.Size = New System.Drawing.Size(294, 48)   '92 104
			.TabIndex = 3
			.Text = ""
			.Multiline = True
			.RightToLeft = Windows.Forms.RightToLeft.Yes
			.BackColor = System.Drawing.SystemColors.ControlLight
			.BorderStyle = BorderStyle.FixedSingle
			'    .BackColor = System.Drawing.SystemColors.Control
		End With


		Me.lblErrors(miCheckIndex) = New System.Windows.Forms.Label
		Me.nudErrors(miCheckIndex) = New System.Windows.Forms.NumericUpDown
		Me.cmdEraseCleanupErr(miCheckIndex) = New System.Windows.Forms.Button()

		'
		'lblErrors
		'
		With Me.lblErrors(miCheckIndex)
			.Location = New System.Drawing.Point(176, 6)
			.Name = "lblErrors" & CStr(miCheckIndex)
			.Size = New System.Drawing.Size(44, 24)
			.TabIndex = 29
			.Font = doLabelFont
			.Text = "Errors:"
			.RightToLeft = Windows.Forms.RightToLeft.No
			.AutoSize = True
		End With

		'
		'nudErrors
		'
		With Me.nudErrors(miCheckIndex)
			.Location = New System.Drawing.Point(216, 4)  '232, 4
			.Name = "nudErrors" & CStr(miCheckIndex)
			.Size = New System.Drawing.Size(42, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.Zero
			.Maximum = Decimal.Zero
			.TabIndex = 20
		End With
		'
		'cmdEraseCleanupErr
		'
		With Me.cmdEraseCleanupErr(miCheckIndex)
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
			.Location = New System.Drawing.Point(260, 4)
			.Name = "cmdEraseCleanupErr" & CStr(miCheckIndex)
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With



		'
		'lblErrors
		'
		'With Me.lblErrors(iCleanupIndex)
		'   .Location = New System.Drawing.Point(176, 6)
		'   .Name = "lblErrors" & CStr(iCleanupIndex)
		'   .Size = New System.Drawing.Size(44, 24)
		'   .TabIndex = 29
		'   .Font = doLabelFont
		'   .Text = "Errors:"
		'   .RightToLeft = Windows.Forms.RightToLeft.No
		'End With




		'
		'cmdEraseCleanupErr
		'
		'With Me.cmdEraseCleanupErr(iCleanupIndex)
		'   .BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
		'   .BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
		'   .Location = New System.Drawing.Point(260, 4)
		'   .Name = "cmdEraseCleanupErr"
		'   .Size = New System.Drawing.Size(24, 22)
		'   .TabIndex = 16
		'   .UseVisualStyleBackColor = True
		'End With
		With Me.doaPanels(iPanelIndex).Controls
			'.Add(Me.nudSteps(iCleanupIndex))
			'.Add(Me.nudErrors(iCleanupIndex))
			'.Add(Me.cmdEraseCleanupErr(iCleanupIndex))
			'.Add(Me.cmdFix(iCleanupIndex))
			'.Add(Me.cmdMark(iCleanupIndex))

			.Add(Me.dgvCheckActions)
			.Add(Me.cmdExec)

			.Add(Me.cmdFixCheck)
			.Add(Me.cmdClear)


			.Add(Me.cmdMessages)

			.Add(Me.cmdDispTable)
			.Add(Me.txtDescription)


			.Add(Me.nudErrors(miCheckIndex))
			.Add(Me.cmdEraseCleanupErr(miCheckIndex))
			.Add(Me.lblErrors(miCheckIndex))
		End With





		Me.doaPanels(iPanelIndex).ResumeLayout(False)
		AddHandler Me.dgvCheckActions.RowEnter, AddressOf dgvCheckActions_RowEnter
		AddHandler Me.cmdExec.Click, AddressOf cmdExec_Click
		AddHandler Me.cmdFixCheck.Click, AddressOf cmdFixCheck_Click


		AddHandler Me.cmdClear.Click, AddressOf cmdClear_Click
		AddHandler Me.cmdMessages.Click, AddressOf cmdMessages_Click

		AddHandler Me.cmdDispTable.Click, AddressOf cmdDispTpln_Click

		AddHandler Me.nudErrors(miCheckIndex).ValueChanged, AddressOf nudErrors_ValueChanged
		AddHandler Me.cmdEraseCleanupErr(miCheckIndex).Click, AddressOf cmdEraseCleanupErr_Click
		' zzSetCheckDescription()
	End Sub

	Private Sub dgvCheckActions_RowEnter(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
		zzSetCheckDescription(e.RowIndex)
		zzSetNumericUpDown(e.RowIndex)
	End Sub
	Private Sub zzSetNumericUpDown(iRowIndex As Integer)
		Dim oDataRow As DataRow = moCheckActionsTable.Rows.Item(iRowIndex)
		Dim iActionID As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ActionID"))
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		moMessagesView = DMAcadExt.AppMessages.GetMessagesView(ptMapThemeData.MapThemeID, iActionID)
		If moMessagesView IsNot Nothing Then
			mbEventsEnabled = False
			Me.nudErrors(miCheckIndex).Maximum = Convert.ToDecimal(moMessagesView.Count)
			Me.nudErrors(miCheckIndex).Value = 0
			mbEventsEnabled = bEventsEnabled
		End If
	End Sub
	Private Sub cmdExec_Click(oSender As System.Object, e As System.EventArgs)
		zzExec()
	End Sub
	Private Sub cmdFixCheck_Click(oSender As System.Object, e As System.EventArgs)
		zzFixCheck()
	End Sub
	Private Sub zzExec()

		Dim bMerge As Boolean = False
		Dim bUnion As Boolean = False
		Dim bFDO_Overlay As Boolean = False
		Dim bApproved As Boolean = False
		Dim bProposed As Boolean = False

		'   DMCommon.Debug.MsgBox("12_320", ptMapThemeData.MapThemeID, ptMapThemeData.MapThemeName, ptMapThemeData.TopoName)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, False)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
		TopoManager.TPlanGraph.TplnProject.InitBlockDic()

		zzThemeCalculate()
		zzSetErrValues()

		zzSetNumericUpDown(Me.dgvCheckActions.CurrentRow.Index)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzExecOld()
		Dim bRes As Boolean
		Dim bMerge As Boolean = False
		Dim bUnion As Boolean = False
		Dim bFDO_Overlay As Boolean = False
		Dim bApproved As Boolean = False
		Dim bProposed As Boolean = False

		DMCommon.Debug.MsgBox("12_320", ptMapThemeData.MapThemeID, ptMapThemeData.MapThemeName, ptMapThemeData.TopoName)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, False)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

		TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
		TopoManager.TPlanGraph.TplnProject.InitBlockDic()

		bRes = TopoManager.TPlanGraph.TplnProject.LoadParcels()
		If bRes Then

			TPlanGraph.TplnProject.CalculateParcels(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, False)
		End If
		'		qqqqqqqqqqqqqqqqqqqqqqqqqq

		If bRes Then ''''''''''''gggggggggggg
			'**************************************************** _429
			TPlanGraph.TplnProject.UpdateParcelTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, False)      '''''''''''''''''''''''''''''   02/06/09
			'System.Windows.Forms.MessageBox.Show("", "PrjCalc_0400")
		End If



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub cmdClear_Click(oSender As System.Object, e As System.EventArgs)
		'  zzOpenMsgForm()
	End Sub

	Private Sub cmdMessages_Click(oSender As System.Object, e As System.EventArgs)
		zzOpenMsgForm()
	End Sub
	Private Sub zzOpenMsgForm()
		'	Dim fMapThemeBase As frmMapThemeBase = Nothing

		Try
			mfMessages = New frmMessages(ptMapThemeData.MapThemeID, miChectActionsUB)

			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfMessages)
			Me.Visible = False
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzOpenMsgForm")
		End Try
	End Sub
	Private Sub cmdDispTpln_Click(oSender As System.Object, e As System.EventArgs)
		zzDispTpln()
	End Sub
	Private Sub zzDispTpln()
		' mfTplnView = New TPlanGraph.frmTplnView
		'  mfTplnView.ShowDialog()
		'  mfTplnView.Dispose()
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
			If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
				mfTplnView = New frmTplnView(New DMAcadExt.MapThemeData())
				'  mfTplnView.MapThemes = mdicMapThemes
				' mfTplnView.InitMapThemeData = zzGetCurrentMapThemeData()

			End If
			If mfTplnView IsNot Nothing AndAlso Not mfTplnView.IsDisposed Then

				mfTplnView.InitMapThemeData = ptMapThemeData

			End If
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)


			Me.Visible = False
		End If
	End Sub
	Private Sub mfMessages_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfMessages.FormClosed
		Me.Visible = True
	End Sub
	Private Sub zzSetErrValue(iActionID As Integer, iErrCount As Integer)
		Dim oRow As System.Data.DataRow = moCheckActionsTable.Rows.Find(iActionID)
		'	DMCommon.Debug.MsgBox("13_134d", iActionID, iErrCount, oRow IsNot Nothing)
		If oRow IsNot Nothing Then
			' oRow.Item("ErrNumber") = Math.Max(DMCommon.Functions.CIntN(oRow.Item("ErrNumber")), iErrNumber)
			oRow.Item("ErrNumber") = iErrCount

		End If
	End Sub

	Private Sub zzSetErrValue(oDataRow As System.Data.DataRow)
		If oDataRow IsNot Nothing Then
			Dim iActionID As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ActionID"))
			Dim iErrCount As Integer = DMAcadExt.AppMessages.GetMessagesCount(ptMapThemeData.MapThemeID, iActionID, False)
			' ????  oDataRow.Item("ErrNumber") = Math.Max(DMCommon.Functions.CIntN(oDataRow.Item("ErrNumber")), iErrNumber)
			'DMCommon.Debug.MsgBox("12_861s", ptMapThemeData.MapThemeID, iActionID, iErrCount)
			oDataRow.Item("ErrNumber") = iErrCount

		End If
	End Sub




	Private Sub zzSetCleanupErrPoints(ByVal iCleanupIndex As Integer)

		Dim oDataRow As DataRow = moCurrentView(iCleanupIndex).Item(miCurrentAction).Row
		Dim iErrorCount As Integer
		If oDataRow.IsNull(msPointsFldName) Then
			moCurrentPoints = Nothing
			iErrorCount = 0
		Else
			moCurrentPoints = DirectCast(oDataRow.Item(msPointsFldName), DMAcadExt.TplnPointArray)
			'	MessageBox.Show(CStr(moCurrentPoints.UpperBound), "21_654f")
			iErrorCount = moCurrentPoints.UpperBound + 1
		End If
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction) & ":" & CStr(iErrorCount), "21_461")
		Try

			moPriorView = DMAcadExt.AcadDocument.GetCurrentView()
			With Me.nudErrors(iCleanupIndex)
				.Maximum = Convert.ToDecimal(iErrorCount)
				.Value = Decimal.Zero
			End With
			Me.txtTopoErrors.Text = " - "
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - zzSetCleanupErrPoints")
		End Try
	End Sub

	Private Sub UpdateTopoByMerge()
		''''''''''	UpdateTopoByMerge()
	End Sub

	Private Sub zzShowTopoError()

		If miCurrentTopoErrIndex = miTopoErrIndexNotErr OrElse (moaTopoErrors Is Nothing) Then
			If moPriorView Is Nothing Then
				TPlanGraph.TplnProject.SetInitView()
			Else
				DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
			End If
		Else
			Try
				If moaTopoErrors Is Nothing Then
					DMAcadExt.AcadDocument.WriteMessage("_17 moaTopoErrors Is Nothing")
				Else
					'	DMAcadExt.AcadDocument.WriteMessage("moCurrentPoints.UpperBound=" & CStr(moCurrentPoints.UpperBound) & "," & CStr(iErrIndex - 1))
					Dim tTopoError As TopoError = moaTopoErrors.Item(miCurrentTopoErrIndex)
					If tTopoError.Exists Then
						DMAcadExt.AcadDocument.WriteMessage("TPlnPoint(" & CStr(miCurrentTopoErrIndex + 1) & ")=" & tTopoError.Point.Coordinates)
						DMAcadExt.AcadDocument.SetView(tTopoError.Point.AcGePoint, 2.0 * tTopoError.Scale, 2.0 * tTopoError.Scale)  '10.0
					Else
						DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
					End If
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ShowTopoError-frmTopoCleanup")
			End Try
		End If
	End Sub

#Region "CleanupProcedures"
	Private Sub zzSetGridColumns(iIndex As Integer)
		Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oTxtColumn As DataGridViewTextBoxColumn
		Try
			With oCmbColumn
				.Name = msActionIDFldName
				.DataPropertyName = msActionIDFldName '"ActionID"
				.HeaderText = "Actions"
				.Width = 152
				.Items.Clear()
				.FlatStyle = FlatStyle.Standard
				'	If miCurrentTopoDefID.TopoIsMerge Then
				'.Items.AddRange(TopoManager.TopoCreator.CleanupActionItemsForMerge())
				'Else
				.Items.AddRange(TopoManager.TopoCreator.CleanupActionItems())
				'	End If
				.MaxDropDownItems = .Items.Count
				.ValueMember = DMCommon.ItemData.ValueMember
				.DisplayMember = DMCommon.ItemData.DisplayMember
				.SortMode = DataGridViewColumnSortMode.NotSortable
				'.ReadOnly = TPlanGraph.TplnProject.TopoIsUnion(miCurrentTopoID)
			End With
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		Try
			Me.dgvActions(iIndex).Columns.Add(oCmbColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oTxtColumn
			.Name = msToleranceFldName
			.HeaderText = "Toler"
			.Width = 40
			.Name = "Tolerance"
			.DataPropertyName = "Tolerance"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvActions(iIndex).Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oTxtColumn
			.HeaderText = "Err"
			.Width = 38
			.Name = msErrorsFldName
			.ReadOnly = True
			.DataPropertyName = msErrorsFldName
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvActions(iIndex).Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try

		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oTxtColumn
			.HeaderText = "Fixed"
			.Width = 38
			.Name = msErrorsAddFldName
			.ReadOnly = True
			.DataPropertyName = msErrorsAddFldName
			.SortMode = DataGridViewColumnSortMode.NotSortable
			.Visible = False

		End With
		Try
			Me.dgvActions(iIndex).Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try
		'	DMCommon.Debug.MsgBox("13_143e", dgvActions(iIndex).Columns.Count)
		'	DMCommon.Debug.MsgBox("13_143a", dgvActions(0).Columns.Count, dgvActions(0).Columns.Item(0).Name, dgvActions(0).Columns.Item(1).Name, dgvActions(0).Columns.Item(2).Name, dgvActions(0).Columns.Item(3).Name)
		'Return oCmbColumn
	End Sub
	Private Function zzGetBreakPointResBuffer(tLinkHandle As Autodesk.AutoCAD.DatabaseServices.Handle) As ResultBuffer
		Dim oResBuffer As ResultBuffer = New ResultBuffer()

		With oResBuffer
			.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1001, msXDataAppName))
			.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1005, tLinkHandle))
		End With
		Return oResBuffer
	End Function


	Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles nudSteps.ValueChanged
		'If mbEventsEnabled Then
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()

		Me.zzLoadCleanupData(iCleanupIndex)
		'	End If


	End Sub
	Private Sub dgvActions_RowEnter(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) 'Handles dgvActions.RowEnter
		'	If miCurrentAction <> -1 Then
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		miCurrentAction = e.RowIndex
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction), "21_459")
		zzSetCleanupErrPoints(iCleanupIndex)
		'	End If
	End Sub
	Private Sub dgvActions_DataError(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) 'Handles dgvActions.DataError
		Dim sMsg As String = "dvgActionsDataErr:" & CStr(e.RowIndex) & "," & CStr(e.ColumnIndex) & "-" & e.Exception.Message
		e.ThrowException = False
		DMAcadExt.AcadDocument.WriteMessage(sMsg)
	End Sub
	Private Sub cmdFix_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim iCleanupIndex As Integer = zzGetCleanupIndex() '' 0 or 1
		zzCleanup(iCleanupIndex, True)
	End Sub
	Private Sub cmdMark_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		zzCleanup(iCleanupIndex, False)
	End Sub

	Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)    'Handles nudErrors.ValueChanged
		If mbEventsEnabled Then


			Dim iCleanupIndex As Integer = zzGetCleanupIndex()
			'   DMCommon.Debug.MsgBox("12_460", iCleanupIndex, Me.nudErrors(iCleanupIndex).Minimum, Me.nudErrors(iCleanupIndex).Maximum, Me.nudErrors(iCleanupIndex).Value)

			Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Value)

			Dim iMax As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Maximum)

			If iErrIndex = 0 Then  'OrElse (moCurrentPoints Is Nothing)
				If moPriorView Is Nothing Then
					TPlanGraph.TplnProject.SetInitView()
				Else
					DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
				End If
			Else
				Try
					'	DMAcadExt.AcadDocument.WriteMessage("moCurrentPoints.UpperBound=" & CStr(moCurrentPoints.UpperBound) & "," & CStr(iErrIndex - 1))
					Dim oPoint As DMAcadExt.TPlnPoint = zzGetCurrentErrPoint(iErrIndex - 1)
					If oPoint IsNot Nothing Then
						DMAcadExt.AcadDocument.WriteMessage("TPlnPoint(" & CStr(iErrIndex) & ")=" & oPoint.Coordinates)
						DMAcadExt.AcadDocument.SetView(oPoint.AcGePoint, 100.0, 100.0)
						DMAcadExt.AcadDocument.DrawShape(DMAcadExt.MarkBlock.enMarkBlockType.Square, 4.0, oPoint, 1, True)

					Else
						DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "nudErrors_ValueChanged-frmTopoVleanup")
				End Try
			End If

		End If
	End Sub
	Private Sub nudErrors_ValueChangedOld(ByVal oSender As System.Object, ByVal e As System.EventArgs)    'Handles nudErrors.ValueChanged

		Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors(miCheckIndex).Value)
		Dim iMax As Integer = Convert.ToInt32(Me.nudErrors(miCheckIndex).Maximum)
		If iErrIndex = 0 OrElse (moCurrentPoints Is Nothing) Then
			If moPriorView Is Nothing Then
				TPlanGraph.TplnProject.SetInitView()
			Else
				DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
			End If
		Else
			Try
				If moCurrentPoints Is Nothing Then
					DMAcadExt.AcadDocument.WriteMessage("_17 moCurrentPoints Is Nothing")
				Else
					'	DMAcadExt.AcadDocument.WriteMessage("moCurrentPoints.UpperBound=" & CStr(moCurrentPoints.UpperBound) & "," & CStr(iErrIndex - 1))
					Dim oPoint As DMAcadExt.TPlnPoint = moCurrentPoints.Item(iErrIndex - 1)
					If oPoint IsNot Nothing Then
						DMAcadExt.AcadDocument.WriteMessage("TPlnPoint(" & CStr(iErrIndex) & ")=" & oPoint.Coordinates)
						DMAcadExt.AcadDocument.SetView(oPoint.AcGePoint, 100.0, 100.0)
					Else
						DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
					End If
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - nudErrors_ValueChanged+")
			End Try
		End If
	End Sub
	Private Function zzGetCurrentErrPoint(iErrIndex As Integer) As DMAcadExt.TPlnPoint
		Dim bCleanup As Boolean
		Select Case LabelCheck.CurrentIndex
			Case 0, 3
				bCleanup = True
			Case 5
				bCleanup = False
		End Select

		If bCleanup AndAlso moCurrentPoints IsNot Nothing Then
			Return moCurrentPoints.Item(iErrIndex)
		ElseIf Not bCleanup AndAlso moMessagesView IsNot Nothing Then
			Dim oDataViewRow As DataRowView = moMessagesView.Item(iErrIndex)
			With oDataViewRow
				Return New DMAcadExt.TPlnPoint(DMCommon.Functions.CDblN(.Item("X")), DMCommon.Functions.CDblN(.Item("Y")))
			End With
		Else
			Return Nothing
		End If

	End Function
	Private Sub cmdEraseCleanupErr_Click(oSender As System.Object, e As System.EventArgs)
		'	MessageBox.Show("", "02_261")
		zzEraseCleanupErrors()
	End Sub
	Protected Sub zzCleanupRestore(ByVal iCleanupIndex As Integer, ByVal bFix As Boolean)
		Dim sDebug As String
		Dim sDebug1 As String
		Dim oDataRowView As DataRowView
		Dim iActionID As Integer
		Dim iAcadActionUB As Integer = -1
		Dim tCleanupOptions As DMAcadExt.dmCleanupOptions = Nothing
		Dim dTolerance As Double
		Dim bDmCleanupFirst As Boolean
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = Nothing
		Dim oDataGridViewRow As DataGridViewRow
		Dim dicSelectedIndices As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
		If Me.dgvActions(iCleanupIndex).SelectedRows.Count > 0 Then
			For iSelectedIndex As Integer = 0 To Me.dgvActions(iCleanupIndex).SelectedRows.Count - 1
				oDataGridViewRow = Me.dgvActions(iCleanupIndex).SelectedRows.Item(iSelectedIndex)
				dicSelectedIndices.Add(oDataGridViewRow.Index, 0)
			Next
		End If
		For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
			If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
				oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
				iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
				dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
				'  MessageBox.Show(CStr(iActionID) & ":" & CStr(DMAcadExt.enCleanupAction.First_dmAction) & vbCrLf & dTolerance.ToString(), "04_659")
				If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
					'acad Cleanup
					If iIndex = 0 Then
						bDmCleanupFirst = False
					End If
					iAcadActionUB += 1
				Else
					' DMap  Cleanup
					If iIndex = 0 Then
						bDmCleanupFirst = True
					End If
					tCleanupOptions.AddDmAction(iActionID, dTolerance, iIndex)
				End If
			End If
		Next
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'	DMCommon.Debug.MsgBox("13_143c", iCleanupIndex, bFix, iAcadActionUB, tCleanupOptions.HasAction)
		Dim oDataRow As DataRow
		Dim bCurrentLayerOK As Boolean
		'  MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers & vbCrLf & CStr(tCleanupOptions.HasAction) & vbCrLf & CStr(iAcadActionUB) & vbCrLf & CStr(tCleanupOptions.RoundingTolerance), "05_011z")
		If iAcadActionUB >= 0 OrElse tCleanupOptions.HasAction Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			If tCleanupOptions.HasAction Then
				tCleanupOptions.MapThemeData = ptMapThemeData
			End If
			DMAcadExt.AcadDocument.WriteMessage("Layers:" & tCleanupOptions.SourceLayers & ";" & tCleanupOptions.DestLayers & "!")
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
			DMAcadExt.AcadDocument.WriteMessage("&&&Layer:" & bCurrentLayerOK.ToString())
			If bCurrentLayerOK Then
				Me.Cursor = Cursors.WaitCursor

				If bDmCleanupFirst Then
					tCleanupResult = zzDMCleanup(bFix, tCleanupOptions, iCleanupIndex)
				End If
				If iAcadActionUB >= 0 Then
					Dim oaActionVar(iAcadActionUB) As ActionVar
					Dim iaAcadCleanupRowIndex(iAcadActionUB) As Integer
					ReDim moaErrorPoints(iAcadActionUB)
					Dim iaErrors() As Integer
					Try
						Dim iVarIndex As Integer = 0
						'	DMCommon.Debug.MsgBox("13_143d", iCleanupIndex, bFix, moCurrentView(iCleanupIndex).Count)
						For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
							If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
								oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
								iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
								If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
									dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
									oaActionVar(iVarIndex) = New ActionVar(iActionID, dTolerance)
									iaAcadCleanupRowIndex(iVarIndex) = iIndex
									iVarIndex += 1
								End If
							End If
						Next
						If ptMapThemeData.IsNotEmpty Then
							Try
								Dim sBaseLayers As String
								Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
								If iCleanupIndex = 0 Then
									sBaseLayers = ptMapThemeData.LinkLayers
									colLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
									colLinks = DMAcadExt.AcadTransaction.GetLinksNew(ptMapThemeData.LinkLayers)
									'  MessageBox.Show(CStr(iCleanupIndex) & vbCrLf & ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers & vbCrLf & CStr(colLinks.Count), "05_013")
								ElseIf iCleanupIndex = 1 Then
									sBaseLayers = String.Empty
									'	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers, "05_014")
									colLinks = DMAcadExt.AcadTransaction.GetLinksNew(ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers)
									'	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers & vbCrLf & CStr(colLinks.Count), "05_014")
								Else
									Return
								End If
								'	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers & vbCrLf & sBaseLayers & vbCrLf & CStr(colLinks.Count), "05_015")
								'	DMCommon.Debug.MsgBox("12_250", oaActionVar.GetUpperBound(0), oaActionVar(0).ActionID, oaActionVar(0).Tolerance, sBaseLayers, colLinks.Count)
								iaErrors = TopoManager.TopoCreator.Cleanup(oaActionVar, sBaseLayers, "", colLinks, bFix, moaErrorPoints)
								'	MessageBox.Show(CStr(colLinks.Count) & ":" & CStr(iaErrors.GetUpperBound(0)), "05_016")
							Catch oEx As Exception
								MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "26_997")
								Common.GetMapTopoEx(oEx, "C919aMM_")
								DMAcadExt.AcadTransaction.Terminate()
								DMAcadExt.AcadDocument.CloseMessage()
								DMAcadExt.AcadDocument.Unlock()
								Return
							End Try
							Dim iResIndex As Integer = 0
							For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
								'oDataRow = oDataTable.Rows(iaAcadCleanupRowIndex(iIndex))
								If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
									oDataRow = moCurrentView(iCleanupIndex).Item(iaAcadCleanupRowIndex(iResIndex)).Row
									'	MessageBox.Show(CStr(iIndex) & ":" & CStr(iaErrors(iResIndex)), "21_472")
									With oDataRow
										If .RowState <> DataRowState.Deleted Then
											.BeginEdit()
											.Item(msErrorsFldName) = iaErrors(iResIndex)
											If moaErrorPoints(iResIndex) IsNot Nothing Then
												.Item(msPointsFldName) = moaErrorPoints(iResIndex)
												sDebug = moaErrorPoints(iResIndex).Count.ToString
											Else
												sDebug = " moaErrorPoints() Is Nothing"

											End If
											.EndEdit()
										Else
											sDebug = "DELETED"
											'' System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
										End If
										sDebug1 = ""
										If False Then
											For i As Integer = 0 To moaErrorPoints(iResIndex).Count - 1
												If moaErrorPoints(iResIndex).Item(i) Is Nothing Then
													sDebug1 &= CStr(i) & ":" & "Nothing"
												Else
													sDebug1 &= CStr(i) & ":" & moaErrorPoints(iResIndex).Item(i).Coordinates2d
												End If
												sDebug1 &= vbCrLf
											Next
										End If


										'	DMCommon.Debug.MsgBox("13_143L", iResIndex, moaErrorPoints.Count, sDebug, sDebug1)


									End With
									iResIndex += 1
								End If
							Next
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - zzCleanup")
					End Try
					If Not bDmCleanupFirst AndAlso tCleanupOptions.HasAction Then
						tCleanupResult = zzDMCleanup(bFix, tCleanupOptions, iCleanupIndex)
					End If
				End If  'iAcadActionUB >= 0

				Do While tCleanupResult.NextAction
					'	oDataRow = oDataTable.Rows(tCleanupResult.RowIndex)
					oDataRow = moCurrentView(iCleanupIndex).Item(tCleanupResult.RowIndex).Row
					With oDataRow
						'	MessageBox.Show(.RowState.ToString() & ":" & CStr(tCleanupResult.RowIndex), "21_458")
						If .RowState <> DataRowState.Deleted Then

							.BeginEdit()
							.Item(msErrorsFldName) = tCleanupResult.ErrNums
							If tCleanupResult.ErrorPoints IsNot Nothing Then
								'	MessageBox.Show(CStr(tCleanupResult.ErrorPoints.UpperBound), "21_652bb")
								.Item(msPointsFldName) = tCleanupResult.ErrorPoints
								sDebug = tCleanupResult.ErrorPoints.Count.ToString
							Else
								sDebug = "ErrorPoints Is Nothing"
							End If
							.EndEdit()
							'	DMCommon.Debug.MsgBox("13_143k", tCleanupResult.ErrNums, sDebug)

							'.AcceptChanges()
						Else
							System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
						End If
					End With
				Loop
				Me.Cursor = Cursors.Default
			End If 'If bCurrentLayerOK Then
			Try
				miCurrentAction = Me.dgvActions(iCleanupIndex).CurrentRow.Index
				zzSetCleanupErrPoints(iCleanupIndex)
			Catch oEx As Exception
				miCurrentAction = -1
			End Try
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzArraySum(ByRef iaSumValues() As Integer, iaValues() As Integer)
		'Dim iaErrors() As Integer
		'Dim iaSumErrors() As Integer
		Dim iUB As Integer
		If iaSumValues Is Nothing Then
			iaSumValues = iaValues
		Else
			iUB = Math.Min(iaSumValues.GetUpperBound(0), iaValues.GetUpperBound(0))
			For iIndex As Integer = 0 To iUB
				iaSumValues(iIndex) += iaValues(iIndex)
			Next
		End If
	End Sub

	Protected Sub zzCleanup(ByVal iCleanupIndex As Integer, ByVal bFix As Boolean)
		Dim sDebug As String
		Dim sDebug1 As String
		Dim oDataRowView As DataRowView
		Dim iActionID As Integer
		Dim iAcadActionUB As Integer = -1
		Dim tCleanupOptions As DMAcadExt.dmCleanupOptions = Nothing
		Dim dTolerance As Double
		Dim bDmCleanupFirst As Boolean
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = Nothing
		Dim oDataGridViewRow As DataGridViewRow
		Dim dicSelectedIndices As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
		If Me.dgvActions(iCleanupIndex).SelectedRows.Count > 0 Then
			For iSelectedIndex As Integer = 0 To Me.dgvActions(iCleanupIndex).SelectedRows.Count - 1
				oDataGridViewRow = Me.dgvActions(iCleanupIndex).SelectedRows.Item(iSelectedIndex)
				dicSelectedIndices.Add(oDataGridViewRow.Index, 0)
			Next
		End If
		For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
			If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
				oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
				iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
				dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
				'	DMCommon.Debug.MsgBox("12_433c", iIndex, iActionID, dicSelectedIndices.Count)
				If iActionID < CType(DMAcadExt.enCleanupAction.First_dmAction, Integer) Then
					'acad Cleanup

					If iIndex = 0 Then
						bDmCleanupFirst = False
					End If
					iAcadActionUB += 1
				Else
					' DMap  Cleanup
					If iIndex = 0 Then
						bDmCleanupFirst = True
					End If
					DMCommon.Debug.MsgBox("13_143k", iActionID, iIndex, bDmCleanupFirst)
					tCleanupOptions.AddDmAction(iActionID, dTolerance, iIndex)
				End If
			End If
		Next
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'	DMCommon.Debug.MsgBox("13_143c", iCleanupIndex, bFix, iAcadActionUB, tCleanupOptions.HasAction)
		Dim oDataRow As DataRow
		Dim bCurrentLayerOK As Boolean

		If iAcadActionUB >= 0 OrElse tCleanupOptions.HasAction Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			'DMCommon.Debug.MsgBox("13_143m", iCleanupIndex, bFix, iAcadActionUB, tCleanupOptions.HasAction)
			If tCleanupOptions.HasAction Then
				tCleanupOptions.MapThemeData = ptMapThemeData
			End If

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
			If bCurrentLayerOK Then
				Me.Cursor = Cursors.WaitCursor

				If bDmCleanupFirst Then
					tCleanupResult = zzDMCleanup(bFix, tCleanupOptions, iCleanupIndex)
				End If
				If iAcadActionUB >= 0 Then
					Dim oaActionVar(iAcadActionUB) As ActionVar
					Dim iaAcadCleanupRowIndex(iAcadActionUB) As Integer
					ReDim moaErrorPoints(iAcadActionUB)
					Dim iaErrors() As Integer
					Dim iaSumErrors() As Integer = Nothing

					Dim iVarIndex As Integer = 0
					Try

						'DMCommon.Debug.MsgBox("13_143d", iCleanupIndex, bFix, moCurrentView(iCleanupIndex).Count, iAcadActionUB, oaActionVar.GetUpperBound(0), iaAcadCleanupRowIndex.GetUpperBound(0), mhsDBDWGLayers.Count, moCurrentView(iCleanupIndex).Count)
						For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
							If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
								oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
								iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
								'DMCommon.Debug.MsgBox("13_143e", iVarIndex, iActionID, DMAcadExt.enCleanupAction.First_dmAction, CInt(DMAcadExt.enCleanupAction.First_dmAction), iActionID < DMAcadExt.enCleanupAction.First_dmAction, ptMapThemeData.IsNotEmpty)
								If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
									dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
									oaActionVar(iVarIndex) = New ActionVar(iActionID, dTolerance)
									iaAcadCleanupRowIndex(iVarIndex) = iIndex
									iVarIndex += 1
								End If
							End If
						Next
						'DMCommon.Debug.MsgBox("13_143f", ptMapThemeData.IsNotEmpty, iCleanupIndex, mhsDBDWGLayers.Count)
						If ptMapThemeData.IsNotEmpty Then
							For Each sLayer As String In mhsDBDWGLayers
								Try
									Dim sBaseLayers As String
									Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
									If iCleanupIndex = 0 Then
										sBaseLayers = ptMapThemeData.LinkLayers
										'colLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
										colLinks = DMAcadExt.AcadTransaction.GetLinksNew(sLayer)
										'ElseIf iCleanupIndex = 1 Then
										'	sBaseLayers = String.Empty
										'	colLinks = DMAcadExt.AcadTransaction.GetLinksNew(ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers)
									Else
										colLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
										Return
									End If
									sBaseLayers = sLayer
									'DMCommon.Debug.MsgBox("12_250", sLayer, sBaseLayers, oaActionVar.GetUpperBound(0), iCleanupIndex, oaActionVar(0).ActionID, oaActionVar(0).Tolerance, colLinks.Count)
									iaErrors = TopoManager.TopoCreator.Cleanup(oaActionVar, sBaseLayers, "", colLinks, bFix, moaErrorPoints)
									zzArraySum(iaSumErrors, iaErrors)
								Catch oEx As Exception
									MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "26_997")
									Common.GetMapTopoEx(oEx, "C919aMM_")
									DMAcadExt.AcadTransaction.Terminate()
									DMAcadExt.AcadDocument.CloseMessage()
									DMAcadExt.AcadDocument.Unlock()
									Return
								End Try
							Next
							'	DMCommon.Debug.MsgBox("13_143g", iaErrors.GetUpperBound(0), moaErrorPoints.GetUpperBound(0), moCurrentView(iCleanupIndex).Count)
							Dim iResIndex As Integer = 0
							For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
								'oDataRow = oDataTable.Rows(iaAcadCleanupRowIndex(iIndex))
								If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
									oDataRow = moCurrentView(iCleanupIndex).Item(iaAcadCleanupRowIndex(iResIndex)).Row

									With oDataRow
										If .RowState <> DataRowState.Deleted Then
											.BeginEdit()
											If iaSumErrors IsNot Nothing Then
												.Item(msErrorsFldName) = iaSumErrors(iResIndex)
											End If

											If moaErrorPoints(iResIndex) IsNot Nothing Then
												.Item(msPointsFldName) = moaErrorPoints(iResIndex)
												sDebug = moaErrorPoints(iResIndex).Count.ToString
											Else
												sDebug = " moaErrorPoints() Is Nothing"

											End If
											.EndEdit()
										Else
											sDebug = "DELETED"
											'' System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
										End If
										sDebug1 = ""
										If False Then
											For i As Integer = 0 To moaErrorPoints(iResIndex).Count - 1
												If moaErrorPoints(iResIndex).Item(i) Is Nothing Then
													sDebug1 &= CStr(i) & ":" & "Nothing"
												Else
													sDebug1 &= CStr(i) & ":" & moaErrorPoints(iResIndex).Item(i).Coordinates2d
												End If
												sDebug1 &= vbCrLf
											Next
										End If


										'	DMCommon.Debug.MsgBox("13_143L", iResIndex, moaErrorPoints.Count, sDebug, sDebug1)

										'DMCommon.Debug.MsgBox("13_143h", iResIndex, moaErrorPoints.Count, sDebug, sDebug1)
									End With
									iResIndex += 1
								End If
							Next
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzCleanup")
					End Try
					If Not bDmCleanupFirst AndAlso tCleanupOptions.HasAction Then
						tCleanupResult = zzDMCleanup(bFix, tCleanupOptions, iCleanupIndex)
					End If
				End If  'iAcadActionUB >= 0

				Do While tCleanupResult.NextAction
					'	oDataRow = oDataTable.Rows(tCleanupResult.RowIndex)
					oDataRow = moCurrentView(iCleanupIndex).Item(tCleanupResult.RowIndex).Row
					With oDataRow
						'	MessageBox.Show(.RowState.ToString() & ":" & CStr(tCleanupResult.RowIndex), "21_458")
						If .RowState <> DataRowState.Deleted Then

							.BeginEdit()
							.Item(msErrorsFldName) = tCleanupResult.ErrNums
							If tCleanupResult.ErrorPoints IsNot Nothing Then
								'	MessageBox.Show(CStr(tCleanupResult.ErrorPoints.UpperBound), "21_652bb")
								.Item(msPointsFldName) = tCleanupResult.ErrorPoints
								sDebug = tCleanupResult.ErrorPoints.Count.ToString
							Else
								sDebug = "ErrorPoints Is Nothing"
							End If
							.Item(msErrorsAddFldName) = tCleanupResult.ErrAddNums
							.EndEdit()
							'	DMCommon.Debug.MsgBox("13_143k", tCleanupResult.ErrNums, sDebug)

							'.AcceptChanges()
						Else
							System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
						End If
					End With
				Loop
				Me.Cursor = Cursors.Default
			End If 'If bCurrentLayerOK Then
			Try
				miCurrentAction = Me.dgvActions(iCleanupIndex).CurrentRow.Index
				zzSetCleanupErrPoints(iCleanupIndex)
			Catch oEx As Exception
				miCurrentAction = -1
			End Try
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			If tCleanupResult.HasErrorAddPoints Then
				Me.dgvActions(iCleanupIndex).Columns.Item(msErrorsAddFldName).Visible = True
			End If
		End If
	End Sub
#End Region




	Private Sub zzZoomPoint(ByVal oPoint As DMAcadExt.TPlnPoint)
		Try
			Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
			oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
			oViewTableRecord.CenterPoint = oPoint.AcGePoint
			oViewTableRecord.Width = 10.0
			oViewTableRecord.Height = 10.0
			Common.GetEditor().SetCurrentView(oViewTableRecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzZoomPoint")
		End Try
	End Sub


	Private Sub zzRegen()
		'	Dim oAcadEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Try
			DMAcadExt.AcadDocument.Regen()
			'	Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzRegen")
		End Try
	End Sub


	Private Function zzDMCleanup(ByVal bFix As Boolean, ByVal tCleanupOptions As DMAcadExt.dmCleanupOptions, ByVal iCleanupIndex As Integer) As DMAcadExt.dmCleanupResult
		'MergingPoints - Yes
		Dim tCleanupResult As DMAcadExt.dmCleanupResult
		DMCommon.Debug.MsgBox("12_410", "zzDMCleanup", "miRoundingRowIndex = " & tCleanupOptions.RoundingRowIndex.ToString(), "PointLineRowindex = " & tCleanupOptions.PointLineRowindex.ToString(), "NetPointsLines = " & tCleanupOptions.NetPointsLines.ToString(), "RoundingLines = " & tCleanupOptions.RoundingLines.ToString(), "Rounding = " & tCleanupOptions.Rounding.ToString() & vbCrLf & "RemoveDuplicates = " & tCleanupOptions.RemoveDuplicates & vbCrLf & "RoundingSPoints = " & tCleanupOptions.RoundingSPoints.ToString(), "PointsNearLines = " & tCleanupOptions.PointsNearLines, "ShortLines = " & tCleanupOptions.ShortLinesRowIndex, "RoundinAll = " & tCleanupOptions.RoundAll, "tCleanupOptions.ConvertArcs = " & tCleanupOptions.ConvertArcs.ToString(), tCleanupOptions.RoundAllDecimals, tCleanupOptions.RoundAll, tCleanupOptions.PointLineTolerance.ToString(), iCleanupIndex)
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If iCleanupIndex = 0 Then

			If tCleanupOptions.Rounding = DMAcadExt.enMerging.RoundingSPoints Then

				tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupSPoints(bFix, tCleanupOptions)
			ElseIf tCleanupOptions.RoundingLines Then
				' New Rounding
				DMCommon.Debug.MsgBox("12_411")
				tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupRoundingLines(bFix, tCleanupOptions)

			ElseIf tCleanupOptions.NetPointsLines Then
				DMCommon.Debug.MsgBox("12_412")
				tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupPointsLines(bFix, tCleanupOptions)

			ElseIf tCleanupOptions.RoundAll Then

				tCleanupResult = DMAcadExt.dmLineCleanup.RoundAll(bFix, tCleanupOptions)
			ElseIf tCleanupOptions.ExplodePLines Then

				tCleanupResult = DMAcadExt.dmLineCleanup.ExplodePLines(bFix, tCleanupOptions)
			ElseIf tCleanupOptions.ConvertArcs Then
				tCleanupOptions.StraightenMinRadius = 4000.0
				tCleanupOptions.StraightenMaxArea = 0.5

				tCleanupResult = DMAcadExt.dmLineCleanup.ConvertArcs(bFix, tCleanupOptions)
			Else
				DMCommon.Debug.MsgBox("12_413", tCleanupOptions.PointMerging, tCleanupOptions.Rounding)
				DMAcadExt.dmLineCleanup.moProgress = prbPaint
				If tCleanupOptions.Rounding = DMAcadExt.enMerging.Merging AndAlso Not tCleanupOptions.PointLine Then
					DMCommon.Debug.MsgBox("12_413c")
					tCleanupResult = DMAcadExt.dmLineCleanup.MergingPoints(bFix, tCleanupOptions)
				Else
					tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupA(bFix, tCleanupOptions)
				End If

			End If



		Else
			tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupNew(bFix, tCleanupOptions)
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		Return tCleanupResult
	End Function

	Private Sub zzLoadCleanupData(iCleanupIndex As Integer)
		Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps(iCleanupIndex).Value)
		Dim sStepNo As String = Convert.ToString(iStepNo)
		Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
		Dim sSelectComText As String

		sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"
		moCleanupActionsTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sSelectComText, CommandType.Text, "CleanupActions")
		'   moCleanupActionsTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSelectComText, CommandType.Text, "CleanupActions")

		moCleanupActionsTable.Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
		moCleanupActionsTable.Columns.Add(msPointsFldName, oPointArray.GetType())
		moCleanupActionsTable.Columns.Add(msErrorsAddFldName, System.Type.GetType("System.Int32"))
		Dim iRowCount As Integer = moCleanupActionsTable.Rows.Count
		If iRowCount > 0 AndAlso moCleanupActionsTable IsNot Nothing Then
			Dim oRow As DataRow = moCleanupActionsTable.Rows.Item(iRowCount - 1)
			mdcStepNum = Convert.ToDecimal(DirectCast(oRow.Item("Step"), Integer))
		End If
		Me.nudSteps(iCleanupIndex).Maximum = mdcStepNum

		'	MessageBox.Show(CStr(moMapThemeData.CleanupType) & ":" & CStr(iRowCount), "05_200")
		Dim oColumn As System.Data.DataColumn = moCleanupActionsTable.Columns("Step")
		oColumn.DefaultValue = iStepNo
		'	DMCommon.Debug.MsgBox("13_143f", dgvActions(0).Columns.Count, dgvActions(0).Columns.Item(msErrorsAddFldName).Visible)
		Dim oDataGridViewColumn As DataGridViewColumn = dgvActions(iCleanupIndex).Columns.Item(msErrorsAddFldName)
		If oDataGridViewColumn.Visible Then
			oDataGridViewColumn.Visible = False
		End If

		moCurrentView(iCleanupIndex) = New DataView(moCleanupActionsTable, "Step=" & sStepNo & "", "", DataViewRowState.CurrentRows)
		Me.dgvActions(iCleanupIndex).DataSource = moCurrentView(iCleanupIndex)
	End Sub

	Private Sub zzLoadCheckData()

		Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
		Dim sComText As String = "SELECT TOP 1000  [ActionID],[ActionOrder],[ActionName],[ActionDescription],[DefaultExec] FROM dbo.CheckActionsExt WHERE (CheckType = 1) AND (MapThemeID = " & Convert.ToString(ptMapThemeData.MapThemeID) & ") ORDER BY ActionOrder"
		'      sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		Dim oNewRow As DataRow

		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				oNewRow = moCheckActionsTable.NewRow
				oNewRow.Item("ActionID") = oDataReader.GetInt32(0)
				oNewRow.Item("ActionName") = oDataReader.GetString(2)
				If Not oDataReader.IsDBNull(3) Then
					oNewRow.Item("ActionDescription") = oDataReader.GetString(3)
				End If
				oNewRow.Item("Exec") = oDataReader.GetBoolean(4)
				moCheckActionsTable.Rows.Add(oNewRow)
				miChectActionsUB += 1
			Loop
			oDataReader.Close()

		End If

		Me.dgvCheckActions.DataSource = moCheckActionsTable

	End Sub


	Private Structure GroupLocation
		Dim GroupXShift As Integer
		Dim GroupYShiftIndex As Integer
		Dim LabelRightX As Integer
		Dim LabelLength As Integer
	End Structure
	Private Structure TopoLocation
		Dim BaseLocation As GroupLocation
		Dim YPosIndex As Integer
		Public ReadOnly Property GroupXShift() As Integer
			Get
				Return BaseLocation.GroupXShift
			End Get
		End Property
		Public Function GetYPosIndex() As Integer
			Return BaseLocation.GroupYShiftIndex + YPosIndex
		End Function
		Public ReadOnly Property LabelRightX() As Integer
			Get
				Return BaseLocation.LabelRightX
			End Get
		End Property
		Public ReadOnly Property LabelLength() As Integer
			Get
				Return BaseLocation.LabelLength
			End Get
		End Property
	End Structure
	Private Sub frmTopoActionsBase_Activated(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Activated
		mbInactive = False
		Me.Opacity = 1.0
		''''''''''''''''''	Me.tmrInactive.Stop()
	End Sub

	Private Sub frmTopoActionsBase_Deactivate(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Deactivate
		mbInactive = True
		mdInactiveTiks = 0.0
		'		Me.tmrInactive.Start()
	End Sub
	Private Sub cmdExit_Click(sender As System.Object, e As System.EventArgs) Handles cmdExit.Click
		'	RaiseEvent AppExit()
	End Sub






	Private Sub tmrInactive_Tick(ByVal oSender As System.Object, ByVal e As System.EventArgs) '''''''''''''''''''''''' Handles tmrInactive.Tick
		Const dMinTiks As Double = 2.0
		'DMAcadExt.AcadDocument.WriteMessage(CStr(mdInactiveTiks))
		If mbInactive Then
			mdInactiveTiks += 1.0
			If mdInactiveTiks <= dMinTiks Then
				Me.Opacity = 1.0
			ElseIf mdInactiveTiks < 10.0 Then
				Me.Opacity = 1.2 - 0.1 * mdInactiveTiks
			Else
				Me.Opacity = 0.2
			End If
		End If
	End Sub


	Private Sub txtStraightenTolerance_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles txtStraightenTolerance.Leave
		If IsNumeric(txtStraightenTolerance) Then
			Parameters.LegendPaintFactor = Convert.ToDouble(Me.txtStraightenTolerance.Text)
		Else
			Me.txtStraightenTolerance.Text = Convert.ToString(Parameters.StraightenTolerance)
		End If
	End Sub
	Private Sub txtLegendPaintFactor_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtLegendPaintFactor.Leave
		If IsNumeric(Me.txtLegendPaintFactor.Text) Then
			Parameters.LegendPaintFactor = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
		Else
			Me.txtLegendPaintFactor.Text = Convert.ToString(Parameters.LegendPaintFactor)
		End If
	End Sub



	Private Sub cmdFirst_Click(sender As System.Object, e As System.EventArgs) Handles cmdFirst.Click
		doaLabelCheck(0).SetCurrent()
	End Sub

	Private Sub cmdLast_Click(sender As System.Object, e As System.EventArgs) ''''''''''''' Handles cmdLast.Click
		doaLabelCheck(piStagesUB).SetCurrent()
	End Sub

	Private Sub cmdPrev_Click(sender As System.Object, e As System.EventArgs) Handles cmdPrev.Click
		Dim iCurrentIndex As Integer = LabelCheck.CurrentIndex
		If iCurrentIndex > 0 Then
			doaLabelCheck(iCurrentIndex - 1).SetCurrent()
		End If
	End Sub




	Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
		Select Case e.ClickedItem.Name
			Case Me.tsbCreateTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzCreateTopo()

					Case 4
						zzCreateTopoWA()
				End Select
				miTopoPanelIndex = LabelCheck.CurrentIndex


			Case Me.tsbCheckTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						'zzCheckDispTopo(True, True)
						zzLoadDataLock()
					Case 4
						zzCheckDispTopoWA(True, True)
				End Select

			Case Me.tsbDeleteTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						Me.zzDeleteTopo(False)
					Case 2
						zzEraseLines()
					Case 4
						Me.zzDeleteTopoWA(False)
				End Select

			Case Me.tsbShowTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzShowTopo()
					Case 4
						zzShowTopoWA()
				End Select
			Case Me.ddbLayers.Name

			Case Me.tsbGetStatistics.Name
				zzGetStatistics()
			Case Me.tsbExec.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzToClosedPgons(False)
					Case 2
						zzStraighten()
					Case 4
						zzToClosedPgonsWA(False)
				End Select
			Case Me.tsbExecA.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzToClosedPgons(False)

					Case 4
						zzToClosedPgonsWA(True)
				End Select


			Case Me.tsbClear.Name
				'  DMCommon.Debug.MsgBox("13_012a", LabelCheck.CurrentIndex)
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzClearPointsNodes()
					Case 2
						zzEraseLines()
					Case 4
				End Select
		End Select
	End Sub
	Private Sub zzLoadDataLock()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		zzLoadData()


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub ddbLayers_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) ''''''''''''Handles ddbLayers.DropDownItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tsiThisTopoOnlyVisible.Name
				Select Case LabelCheck.CurrentIndex
					Case 1
						zzSourceOnlyVisible(False)
					Case 4
						zzSourceOnlyVisible(True)
				End Select
			Case Me.tsiThisTopoVisible.Name

			Case tsiAllVisible.Name
				'MessageBox.Show("", "01_870 Inherit")
				zzAllVisible()

		End Select
	End Sub
	Private Sub zzClearPointsNodes()

	End Sub
	Private Sub zzSourceOnlyVisible(bLineLinks As Boolean)
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)
		Dim sLineLinkLayers As String
		If bLineLinks Then
			sLineLinkLayers = ptMapThemeData.LineLinkLayers
		Else
			sLineLinkLayers = String.Empty
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, ptMapThemeData.LinkLayers, sLineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzThisOnlyVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		oEntity.Visible = bCond
	End Sub

	Private Sub zzAllVisible()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ProcAll(dlEntityProc, True)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzCreateCheckActionsTable()
		Dim oDataColumn As System.Data.DataColumn
		Dim t As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim oDataType As System.Type = t.GetType()


		moCheckActionsTable = New System.Data.DataTable("CheckActions")

		oDataColumn = New System.Data.DataColumn("ActionID", GetType(System.Int32))
		moCheckActionsTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("ActionName", GetType(System.String))
		moCheckActionsTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("ActionDescription", GetType(System.String))
		moCheckActionsTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("Exec", GetType(System.Boolean))
		moCheckActionsTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("ErrNumber", GetType(System.Int32))
		moCheckActionsTable.Columns.Add(oDataColumn)
		Dim oPrimaryKey() As System.Data.DataColumn = {moCheckActionsTable.Columns.Item("ActionID")}
		moCheckActionsTable.PrimaryKey = oPrimaryKey



	End Sub
	Private Sub zzCreateTopo()
		Dim dTolerance As Double
		Dim bCreateCentroids As Boolean
		Dim bCreateNodes As Boolean
		'Dim oDataRow As DataRow
		Dim oDataRowView As DataRowView

		Dim sTopoLayerName As String
		Dim iPgonCount As Integer
		'	Dim tTopoRes As DMAcadExt.TopoRes

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		If Information.IsNumeric(Me.txtTolerance.Text) Then
			dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
			bCreateCentroids = Me.chkCreateCentroid.Checked
			bCreateNodes = Me.chkCreateNode.Checked
			Try
				'moTopoDataTable
				'	DMCommon.Debug.MsgBox("12_290c", ptMapThemeData.TopoName, ptMapThemeData.MapThemeID, ptMapThemeData.LinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.NodeLayers, bCreateNodes)
				For Each oDataGridViewRow As DataGridViewRow In Me.dgvTopoList.SelectedRows
					oDataRowView = DirectCast(oDataGridViewRow.DataBoundItem, DataRowView)
					iPgonCount = DMCommon.Functions.CIntN(oDataRowView.Item("PolygonCount"))
					If iPgonCount = 0 Then
						sTopoLayerName = DMCommon.Functions.CStrN(oDataRowView.Item("TopoName"))
						mtTopoRes = TopoCreator.CreateTopology(sTopoLayerName, ptMapThemeData.MapThemeID, sTopoLayerName, "", ptMapThemeData.CentroidBlocks, sTopoLayerName, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.NodeLayers, bCreateNodes, Me.chkHighlightSliver.Checked, dTolerance)
						'zzDispTopoOK()
						If mtTopoRes.TopoExists Then
							oDataRowView.Item("PolygonCount") = mtTopoRes.PgonCount
							'Me.txtPgonCount.Text = CStr(mtTopoRes.PgonCount)
							'Me.txtLinkCount.Text = CStr(mtTopoRes.LinkCount)
							'Me.txtCentroidCount.Text = CStr(mtTopoRes.CentroidCount)

							moParams.SetValue(0, dTolerance)
							moParams.Update()

						End If

					End If

				Next

				zzSetStyle()

				moaTopoErrors = TopoCreator.GetTopoErrors(enTopoErrType.RefRMark)
				If moaTopoErrors IsNot Nothing Then

					moaTopoErrors.SourceTopo = True
				End If



				'zzDispTopoOK()
				moaTopoErrors = TopoCreator.GetAllTopoErrors()
				moaTopoErrors.SourceTopo = True
				If moaTopoErrors IsNot Nothing Then
					Me.txtErrorCount.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
					miCurrentTopoErrIndex = miTopoErrIndexNotErr
					zzDispErrIndex()
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzCreateTopo_01")
			End Try
		Else
			Beep()
		End If
		zzThemeCalculate()
		zzSetErrValues()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default


	End Sub
	Private Sub zzDispTopoErrors()
		If moaTopoErrors IsNot Nothing Then
			Me.txtErrorCount.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
		Else
			Me.txtErrorCount.Text = "0"
		End If
		miCurrentTopoErrIndex = miTopoErrIndexNotErr
		zzDispErrIndex()
	End Sub

	Private Sub zzCreateTopoWA()
		Dim dTolerance As Double
		Dim bCreateCentroids As Boolean
		Dim bCreateNodes As Boolean
		'	Dim tTopoRes As DMAcadExt.TopoRes

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		If Information.IsNumeric(Me.txtToleranceWA.Text) Then
			dTolerance = Convert.ToDouble(Me.txtToleranceWA.Text)
			bCreateCentroids = Me.chkCreateCentroidWA.Checked
			Try
				mtTopoResWA = TopoCreator.CreateTopology(ptMapThemeData.LineTopoName, ptMapThemeData.MapThemeID, ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.CentroidLayers, bCreateNodes, Me.chkHighlightSliverWA.Checked, dTolerance)
				zzDispTopoOK_WA()
				If mtTopoResWA.TopoExists Then
					Me.txtPgonCountWA.Text = CStr(mtTopoResWA.PgonCount)
					Me.txtLinkCountWA.Text = CStr(mtTopoResWA.LinkCount)
					Me.txtCentroidCountWA.Text = CStr(mtTopoResWA.CentroidCount)
					'	Me.doaLabelCheck(1).Checked = True

					moParams.SetValue(1, dTolerance)
					moParams.Update()

					moaTopoErrors = TopoCreator.GetTopoErrors(enTopoErrType.RefRMark)

					If False AndAlso moaTopoErrors IsNot Nothing Then
						System.Windows.Forms.MessageBox.Show(CStr(moaTopoErrors.UpperBound) & ":" & CStr(mtTopoResWA.MissingCntrCount), "02_544")
					End If
					'	DMCommon.Debug.MsgBox("13_134cAA", mtTopoResWA.MissingCntrCount, mtTopoResWA.OutsideCntrCount)
					zzSetErrValue(1, mtTopoResWA.MissingCntrCount)
					zzSetErrValue(2, mtTopoResWA.OutsideCntrCount)
				Else
					'  Me.doaLabelCheck(4).Checked = False
					'	Me.lblTopoExistsWA.Visible = False

					moaTopoErrors = TopoCreator.GetAllTopoErrors()

				End If

				If moaTopoErrors IsNot Nothing Then
					Me.txtErrorCountWA.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
					miCurrentTopoErrIndex = miTopoErrIndexNotErr
					zzDispErrIndex()
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzCreateTopoWA_01")
			End Try
		Else
			Beep()
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default


	End Sub
	Private Sub zzDispTopoOK()
		Dim bTopoOK As Boolean = mtTopoRes.IsTopoOK
		Dim bOK As Boolean = mtTopoRes.IsOK

		Me.doaLabelCheck(1).Checked = bTopoOK
		Me.lblTopoExists.Visible = bTopoOK
		Me.txtPgonCount.Enabled = bTopoOK
		If bTopoOK Then
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoName.Font = doLabelBoldFont
		Else
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoName.Font = doLabelFont
			Me.doaLabelCheck(5).Checked = False

		End If
		'	DMCommon.Debug.MsgBox("12_621ZZ", bTopoOK, mtTopoRes.PgonCount, mtTopoRes.CentroidCount, mtTopoRes.MissingCntrCount, mtTopoRes.InsertedCntrCount)
		Me.txtPgonCount.Text = Convert.ToString(mtTopoRes.PgonCount)
		If mtTopoRes.HasElements Then
			Me.txtLinkCount.Text = CStr(mtTopoRes.LinkCount)
			Me.txtCentroidCount.Text = CStr(mtTopoRes.CentroidCount)
			Me.txtNodeCount.Text = CStr(mtTopoRes.NodeCount)

		End If
		'	DMCommon.Debug.MsgBox("13_134c", mtTopoRes.MissingCntrCount, mtTopoRes.OutsideCntrCount, Me.chkCreateCentroid.Checked, Me.txtCentroidCount.Text, mtTopoRes.CentroidCount)


		zzSetErrValue(1, mtTopoRes.MissingCntrCount)
		zzSetErrValue(2, mtTopoRes.OutsideCntrCount)
		'DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.Y, "", DMCommon.dmMessages.Message(310, sName), False, iMapTheme, 31)
	End Sub
	Private Sub zzDispTopoExists(bExists As Boolean)
		Me.doaLabelCheck(1).Checked = bExists
		Me.lblTopoExists.Visible = bExists
		Me.txtPgonCount.Enabled = bExists
		If bExists Then
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoName.Font = doLabelBoldFont

		Else
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoName.Font = doLabelFont
		End If
	End Sub

	Private Sub zzDispTopoOK_WA()
		Dim bOK As Boolean = mtTopoResWA.IsOK
		Me.doaLabelCheck(4).Checked = bOK
		Me.lblTopoExistsWA.Visible = bOK
		Me.txtPgonCountWA.Enabled = bOK
		If bOK Then
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoNameWA.Font = doLabelBoldFont
		Else
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoNameWA.Font = doLabelFont
		End If
		Me.txtPgonCountWA.Text = Convert.ToString(mtTopoResWA.PgonCount)
		If mtTopoResWA.HasElements Then
			Me.txtLinkCountWA.Text = CStr(mtTopoResWA.LinkCount)
			Me.txtCentroidCountWA.Text = CStr(mtTopoResWA.CentroidCount)
		End If

	End Sub
	Private Sub zzDispTopoExistsWA(bExists As Boolean)
		Me.doaLabelCheck(4).Checked = bExists
		Me.lblTopoExistsWA.Visible = bExists
		Me.txtPgonCountWA.Enabled = bExists
		If bExists Then
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoNameWA.Font = doLabelBoldFont


		Else
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoNameWA.Font = doLabelFont
		End If


	End Sub
	Private Sub zzCheckDispTopo(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
		zzCheckTopo(bLockDoc, bMsgBox)
		'  DMCommon.Debug.MsgBox("12_600", bLockDoc)
		zzDispTopoOK()

	End Sub
	Private Function zzCheckTopoList(sTopoName As String) As Integer
		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oTopoModel IsNot Nothing Then
			Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
			Dim iPgonCount As Integer = colPolygons.Count
			colPolygons.Dispose()
			colPolygons = Nothing
			oTopoModel.Close()
			Return iPgonCount
		Else
			Return 0
		End If

	End Function
	Private Sub zzCheckTopo(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
		'  MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & bLockDoc, "05_338")
		If bLockDoc Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
		End If
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim colCentroidBlockRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)
		Dim colBlockRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		'DMCommon.Debug.MsgBox("12_440", ptMapThemeData.TopoName, colBlockRefs.Count, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)
		mtTopoRes = TopoCreator.CheckTopo(ptMapThemeData.TopoName, False, bMsgBox, ptMapThemeData.MapThemeID, colBlockRefs)
		If bLockDoc Then
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'  MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & bLockDoc & ":" & CStr(mtTopoRes.IsOK), "05_339")
	End Sub

	Private Sub zzCheckDispTopoWA(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
		If ptMapThemeData.LineTopoExists Then
			zzCheckTopoWA(bLockDoc, bMsgBox)
			zzDispTopoOK_WA()
			zzSetErrValue(1, mtTopoResWA.MissingCntrCount)
			zzSetErrValue(2, mtTopoResWA.OutsideCntrCount)
		End If


	End Sub

	Private Sub zzCheckTopoWA(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
		If bLockDoc Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
		End If
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim colBlockRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)
		DMAcadExt.AcadTransaction.CloseModelSpace()

		'   MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & "+", "05_438")
		mtTopoResWA = TopoCreator.CheckTopo(ptMapThemeData.LineTopoName, False, bMsgBox, ptMapThemeData.MapThemeID, colBlockRefs)
		'   MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & CStr(mtTopoResWA.IsOK), "05_439")

		If bLockDoc Then
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If

	End Sub
	Private Sub zzThemeCalculate()

		'	'''''''''''''''''''''''''''DMAcadExt.AppMessages.ClearMessages(ptMapThemeData.MapThemeID)
		'  DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		' DMAcadExt.AcadTransaction.Start()
		'   DMCommon.Debug.MsgBox("07_040", ptMapThemeData.MapThemeID)
		Select Case ptMapThemeData.MapThemeID
			Case DMAcadExt.enMapTheme.Parcels
				zzCheckParcels()     ''''''''''''''''''1104
			Case DMAcadExt.enMapTheme.UD_Parcels
				zzCheckUDParcels()
			Case DMAcadExt.enMapTheme.Fragments
				zzCheckMapPoints()
			Case DMAcadExt.enMapTheme.BN
				zzCheckBN()
			Case DMAcadExt.enMapTheme.LotApproved
				zzCheckLots(DMAcadExt.enTopoPurpose.Approved)
			Case DMAcadExt.enMapTheme.LotProposed
				zzCheckLots(DMAcadExt.enTopoPurpose.Proposed)
			Case DMAcadExt.enMapTheme.Expropriation
				zzCheckExpros()

		End Select
		Dim oMsgTable As System.Data.DataTable = DMAcadExt.AppMessages.MsgTable

		' DMAcadExt.AcadTransaction.Terminate()
		'  DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzFixCheck()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		'   DMCommon.Debug.MsgBox("07_040", ptMapThemeData.MapThemeID)
		Select Case ptMapThemeData.MapThemeID
			Case DMAcadExt.enMapTheme.Parcels

			Case DMAcadExt.enMapTheme.UD_Parcels
				zzFixCheckUDParcels()
			Case DMAcadExt.enMapTheme.Fragments

			Case DMAcadExt.enMapTheme.BN

			Case DMAcadExt.enMapTheme.LotApproved

			Case DMAcadExt.enMapTheme.LotProposed

		End Select
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub zzCheckParcels()
		Dim bRes As Boolean
		Dim bMerge As Boolean = False
		Dim bUnion As Boolean = False
		Dim bFDO_Overlay As Boolean = False
		Dim bApproved As Boolean = False
		Dim bProposed As Boolean = False
		Dim bExpro As Boolean = False

		'   Dim iErrNumber As Integer
		'	DMCommon.Debug.MsgBox("12_461", "zzCheckParcels", ptMapThemeData.MapThemeID)
		TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
		TopoManager.TPlanGraph.TplnProject.InitBlockDic()

		bRes = TopoManager.TPlanGraph.TplnProject.LoadParcels(False)
		If bRes Then
			TPlanGraph.TplnProject.CalculateParcels(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro)
		End If


		If bRes Then ''''''''''''gggggggggggg
			'**************************************************** _429
			TPlanGraph.TplnProject.UpdateParcelTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro)      '''''''''''''''''''''''''''''   02/06/09
		End If


	End Sub
	Private Sub zzCheckFragments()


		Dim iMapThemeID As DMAcadExt.enMapTheme = ptMapThemeData.MapThemeID
		Dim oFragmentsTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oFragmentsTopology IsNot Nothing Then
			'Dim oParcel As UnidivNet.UD_Parcel
			'	DMCommon.Debug.MsgBox("12_480", oParcelTopology.Name)
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oFragmentsTopology.GetPolygons()
			Dim oIdenticalNames As CheckDoubleString = New CheckDoubleString()

			'   DMCommon.Debug.MsgBox("12_480a", colParcelPgons.Count)







			oIdenticalNames = Nothing
			Dim colParcelNodes As Autodesk.Gis.Map.Topology.NodeCollection = oFragmentsTopology.GetNodes()
			Dim tNodeObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			Dim bAcadPoint As Boolean


			Dim saBlockName() As String = UnidivNet.UD_App.GetOldPointBlocks()
			Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology()
			Dim colNodes As System.Collections.ObjectModel.Collection(Of TopoScheme.tsNode)
			Dim colNodePoints As System.Collections.Generic.ICollection(Of DMAcadExt.TPlnPoint) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.TPlnPoint)
			Dim colNodeIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim oTriangleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
			Dim saAttributeText() As String
			Dim sBlockName As String
			Dim sName As String
			Dim iNameNum As Integer

			DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 32)
			oTriangleMarkBlock.EraseMyReferences()
			oTopoScheme.Load(True, oFragmentsTopology)
			mcolNewSPoints = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

			miParcelNumMax = 10
			colNodes = oTopoScheme.Nodes
			For Each oNodeScheme As TopoScheme.tsNode In colNodes
				sBlockName = Nothing
				tNodeObjId = oNodeScheme.AcObjID
				bAcadPoint = False
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRefForRead(tNodeObjId, False, bAcadPoint)
				If oBlockRef IsNot Nothing Then
					sBlockName = oBlockRef.Name
					saAttributeText = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef)
					If saAttributeText IsNot Nothing AndAlso saAttributeText.GetUpperBound(0) >= 0 Then
						sName = saAttributeText(0)

						sName = Strings.Replace(sName, "E", "X")
						sName = Strings.Replace(sName, "e", "X")


						iNameNum = CInt(Val(sName))

						If iNameNum > miParcelNumMax Then
							miParcelNumMax = iNameNum
						End If
						'DMAcadExt.AcadDocument.WriteDebugMessageN("#296", sName, iNameNum, miParcelNumMax)
					End If
				End If


				If Not oNodeScheme.IsPseudoGeo AndAlso (oBlockRef IsNot Nothing OrElse bAcadPoint) AndAlso (sBlockName Is Nothing OrElse sBlockName = UnidivNet.UD_App.NewPointBlockName) Then
					'   DMCommon.Debug.MsgBox("12_861b", DMAcadExt.AppMessages.RecordCount, iMapThemeID, 2)
					mcolNewSPoints.Add(tNodeObjId)
					DMAcadExt.AppMessages.AddMessage(True, oNodeScheme.Location.X, oNodeScheme.Location.Y, "", DMCommon.dmMessages.Message(312, Name), False, iMapThemeID, 32)

					oTriangleMarkBlock.MarkPoint(oNodeScheme.Location, 4S)
					'  DMCommon.Debug.MsgBox("12_861a", DMAcadExt.AppMessages.RecordCount)
				End If
				colNodeIDs.Add(tNodeObjId)
				colNodePoints.Add(New DMAcadExt.TPlnPoint(oNodeScheme.Location))
			Next oNodeScheme
			colNodes = Nothing

			oTopoScheme.Close()

			CheckCloseSPoints(colNodePoints, colNodeIDs, saBlockName)

			oFragmentsTopology.Close()
		End If


		'oParcelTopology IsNot Nothing
	End Sub
	Private Sub zzCheckUDParcels()

		Dim iMapThemeID As DMAcadExt.enMapTheme = ptMapThemeData.MapThemeID
		Dim oParcelTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oParcelTopology IsNot Nothing Then
			Dim oParcel As UnidivNet.UD_Parcel
			'	DMCommon.Debug.MsgBox("12_480", oParcelTopology.Name)
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oParcelTopology.GetPolygons()
			Dim oIdenticalNames As CheckDoubleString = New CheckDoubleString()
			Dim sParcelName As String
			Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
			Dim oaIntegerQueue() As Integer
			'   DMCommon.Debug.MsgBox("12_480a", colParcelPgons.Count)
			UnidivNet.UD_Parcel.Initialize()
			'''''''''''''''''1104
			For Each oPolygon In colParcelPgons
				oParcel = New UnidivNet.UD_Parcel(oPolygon)
				If oParcel IsNot Nothing Then
					sParcelName = oParcel.Name
					If Not String.IsNullOrEmpty(sParcelName) Then
						oIdenticalNames.Add(sParcelName, True, oPolygon.ID)
					End If
				End If
				oPolygon.Dispose()
				oPolygon = Nothing
			Next
			colParcelPgons.Dispose()
			colParcelPgons = Nothing



			For Each oKeyValuePair As KeyValuePair(Of String, Queue(Of Integer)) In oIdenticalNames.DoubleValues
				oaIntegerQueue = oKeyValuePair.Value.ToArray()
				sParcelName = oKeyValuePair.Key
				For Each iTopoID As Integer In oaIntegerQueue
					oPolygon = oParcelTopology.GetPolygon(iTopoID)
					If oPolygon IsNot Nothing Then
						DMAcadExt.AppMessages.AddMessage(True, oPolygon.Centroid.X, oPolygon.Centroid.Y, "", DMCommon.dmMessages.Message(309, sParcelName, "3947"), False, iMapThemeID, 11)
					End If
				Next iTopoID
				oaIntegerQueue = Nothing
			Next oKeyValuePair

			oIdenticalNames = Nothing
			Dim colParcelNodes As Autodesk.Gis.Map.Topology.NodeCollection = oParcelTopology.GetNodes()
			Dim tNodeObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			Dim bAcadPoint As Boolean


			Dim saBlockName() As String = UnidivNet.UD_App.GetOldPointBlocks()
			Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology()
			Dim colNodes As System.Collections.ObjectModel.Collection(Of TopoScheme.tsNode)
			Dim colNodePoints As System.Collections.Generic.ICollection(Of DMAcadExt.TPlnPoint) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.TPlnPoint)
			Dim colNodeIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim oTriangleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
			Dim saAttributeText() As String
			Dim sBlockName As String
			Dim sName As String
			Dim iNameNum As Integer

			DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 32)
			oTriangleMarkBlock.EraseMyReferences()
			oTopoScheme.Load(True, oParcelTopology)
			mcolNewSPoints = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

			miParcelNumMax = 10
			colNodes = oTopoScheme.Nodes
			For Each oNodeScheme As TopoScheme.tsNode In colNodes
				sBlockName = Nothing
				tNodeObjId = oNodeScheme.AcObjID
				bAcadPoint = False
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRefForRead(tNodeObjId, False, bAcadPoint)
				If oBlockRef IsNot Nothing Then
					sBlockName = oBlockRef.Name
					saAttributeText = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef)
					If saAttributeText IsNot Nothing AndAlso saAttributeText.GetUpperBound(0) >= 0 Then
						sName = saAttributeText(0)
						sName = Strings.Replace(sName, "E", "X")
						sName = Strings.Replace(sName, "e", "X")


						iNameNum = CInt(Val(sName))

						If iNameNum > miParcelNumMax Then
							miParcelNumMax = iNameNum
						End If

					End If
				End If


				If Not oNodeScheme.IsPseudoGeo AndAlso (oBlockRef IsNot Nothing OrElse bAcadPoint) AndAlso (sBlockName Is Nothing OrElse sBlockName = UnidivNet.UD_App.NewPointBlockName) Then
					'   DMCommon.Debug.MsgBox("12_861b", DMAcadExt.AppMessages.RecordCount, iMapThemeID, 2)
					mcolNewSPoints.Add(tNodeObjId)
					DMAcadExt.AppMessages.AddMessage(True, oNodeScheme.Location.X, oNodeScheme.Location.Y, "", DMCommon.dmMessages.Message(312, Name), False, iMapThemeID, 32)
					oTriangleMarkBlock.MarkPoint(oNodeScheme.Location, 4S)
					'  DMCommon.Debug.MsgBox("12_861a", DMAcadExt.AppMessages.RecordCount)
				End If
				colNodeIDs.Add(tNodeObjId)
				colNodePoints.Add(New DMAcadExt.TPlnPoint(oNodeScheme.Location))
			Next oNodeScheme
			colNodes = Nothing

			oTopoScheme.Close()
			If True Then
				CheckCloseSPoints(colNodePoints, colNodeIDs, saBlockName)
			End If
			oParcelTopology.Close()
		End If


		'oParcelTopology IsNot Nothing
	End Sub

	Private Sub zzCheckBN()
		Dim iMapThemeID As DMAcadExt.enMapTheme = ptMapThemeData.MapThemeID
		Dim oBNTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(ptMapThemeData.TopoName)
		Dim oTriangleMarkBlock As DMAcadExt.MarkBlock

		'    DMCommon.Debug.MsgBox("07_040a", ptMapThemeData.TopoName)
		DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 34)
		If oBNTopoScheme.Load(False) Then
			oBNTopoScheme.CalcIsthmus()
			oTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
			oTriangleMarkBlock.EraseMyReferences()
			For Each oPgon As TopoScheme.tsPolygon In oBNTopoScheme.Polygons
				'   If iPgonID = 58 Or iPgonID = 61 Then

				oPgon.CheckIslands(iMapThemeID, oTriangleMarkBlock)
				'  End If
			Next
		End If

	End Sub
	Public Sub CheckCloseSPoints(colNodePoints As System.Collections.Generic.ICollection(Of DMAcadExt.TPlnPoint), colNodeIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, saBlockName() As String)
		' Mark:  Triangle, 6
		Dim iMapThemeID As DMAcadExt.enMapTheme = ptMapThemeData.MapThemeID
		Dim oAttachEntities As DMAcadExt.AttachEntities = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPoint, 1.5)
		' Dim oPoint As TPlnPoint
		Dim iStatus As DMAcadExt.AttachEntities.enStatus
		Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
		Dim oaBlockRefPoints As DMAcadExt.TplnPointArray
		Dim oPoint As DMAcadExt.TPlnPoint
		'	DMCommon.ExcelLog.Open()
		'   Dim iErrNumber As Integer = DMAcadExt.AppMessages.GetMessagesCount(iMapThemeID, 33)
		DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 33)
		oSquareMarkBlock.EraseMyReferences()

		'    Dim iErrNumberA As Integer = DMAcadExt.AppMessages.GetMessagesCount(iMapThemeID, 33)
		For Each oPoint In colNodePoints
			oAttachEntities.AddPointEntity(oPoint, Autodesk.AutoCAD.DatabaseServices.ObjectId.Null, False)
			'DMCommon.ExcelLog.SetNextValue(2, oPoint.Coordinates2d)
		Next

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'	DMCommon.Debug.MsgBox("13_003", DMCommon.Debug.ColCount(saBlockName))

		For iBlockIndex As Integer = 0 To saBlockName.GetUpperBound(0)
			' 
			oaBlockRefPoints = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint(saBlockName(iBlockIndex))
			'  DMCommon.Debug.MsgBox("13_003a", DMCommon.Debug.ColCount(oaBlockRefPoints))
			'DMCommon.Debug.MsgBox("13_003e", oaBlockRefPoints)
			'   Dim iLoop As Integer
			If oaBlockRefPoints IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("13_003f", saBlockName(iBlockIndex), oaBlockRefPoints.UpperBound)
				For iIndex As Integer = 0 To oaBlockRefPoints.UpperBound
					oPoint = oaBlockRefPoints.Item(iIndex)
					If Not colNodeIDs.Contains(oPoint.AcObjID) Then
						iStatus = oAttachEntities.GetPointLocation(oPoint)
						'DMCommon.ExcelLog.SetNextValue(0, iStatus, iStatus.ToString(), oPoint.Coordinates2d)
						'   DMCommon.Debug.MsgBox("13_003f", DMCommon.Debug.ColCount(oaBlockRefPoints), oAttachEntities.GetPointsCount(False), iStatus)
						If iStatus <> DMAcadExt.AttachEntities.enStatus.NotFound Then
							oSquareMarkBlock.MarkPoint(oPoint.AcGePoint, 2S)
							DMAcadExt.AppMessages.AddMessage(True, oPoint.X, oPoint.Y, "", DMCommon.dmMessages.Message(315), False, iMapThemeID, 33, True)

						End If
					End If
				Next
			End If


		Next
		DMAcadExt.AcadTransaction.CloseModelSpace()


		'   System.Windows.Forms.MessageBox.Show(mdicNetPoints.Count.ToString & vbCrLf & moaResPoints.UpperBound.ToString, "05_132")
	End Sub
	Private Sub zzCheckLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
		Dim bRes As Boolean
		Dim bMerge As Boolean = False
		Dim bUnion As Boolean = False
		Dim bFDO_Overlay As Boolean = False
		Dim bApproved As Boolean = False
		Dim bProposed As Boolean = False
		'   Dim iErrNumber As Integer
		'	DMCommon.Debug.MsgBox("12_460k", "zzCheckLots", ptMapThemeData.MapThemeID, iTopoPurpose)
		TopoManager.TPlanGraph.TplnLot.Initialize(ptMapThemeData)
		TopoManager.TPlanGraph.TplnProject.InitRegionDic()


		bRes = TopoManager.TPlanGraph.TplnProject.LoadLots(iTopoPurpose, False)

		'		qqqqqqqqqqqqqqqqqqqqqqqqqq

		If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
			TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
			'  TPlanGraph.TplnProject.LoadLusePgonsNew(ptMapThemeData)
		End If

		If iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then

			TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
			' TPlanGraph.TplnProject.LoadLusePgonsNew(ptMapThemeData)
		End If

	End Sub

	Private Sub zzCheckExpros()
		Dim bRes As Boolean
		Dim bMerge As Boolean = False
		Dim bUnion As Boolean = False
		Dim bFDO_Overlay As Boolean = False

		TopoManager.TPlanGraph.TplnExpro.Initialize(ptMapThemeData)



		bRes = TopoManager.TPlanGraph.TplnProject.LoadExpros


		'		qqqqqqqqqqqqqqqqqqqqqqqqqq


		'TPlanGraph.TplnProject.UpdateeXPRO(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)




	End Sub

	Private Sub zzFixCheckUDParcels()
		Dim dTolerance As Double

		DMCommon.Debug.MsgBox("07_047", ptMapThemeData.MapThemeID, ptMapThemeData.TopoName, DMCommon.Debug.ColCount(mcolNewSPoints), miParcelNumMax)
		UnidivNet.UD_Point.Init()

		If mcolNewSPoints IsNot Nothing AndAlso mcolNewSPoints.Count > 0 Then

			TopoCreator.DeleteTopology(ptMapThemeData.TopoName, False, False)
			For Each tBlockRefObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In mcolNewSPoints
				zzNewSPointToOldSPoint(tBlockRefObjID)
			Next
			dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
			TopoCreator.CreateTopology(ptMapThemeData.TopoName, ptMapThemeData.MapThemeID, ptMapThemeData.LinkLayers, "", ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, False, ptMapThemeData.NodeBlocks, ptMapThemeData.NodeLayers, False, Me.chkHighlightSliver.Checked, dTolerance)
		End If
	End Sub
	Private Sub zzNewSPointToOldSPoint(tBlockRefObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
		Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = oBlockRef.Position
		miParcelNumMax += 1
		Dim oUD_Point As UnidivNet.UD_Point = New UnidivNet.UD_Point(miParcelNumMax.ToString())
		oUD_Point.Insert(tInsertPoint, UnidivNet.UD_App.GetStageUDPointLayer(0, False))
		oBlockRef.Erase()
	End Sub
	Private Function zzNodeEntityExists(oNode As Autodesk.Gis.Map.Topology.Node, ByRef tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As Boolean
		Try
			tAcObjID = oNode.Entity
			Return Not tAcObjID.IsNull
		Catch oMapEx As Autodesk.Gis.Map.MapException
			Return False
		End Try
	End Function
	Private Sub zzCheckMapPoints()

		Const sPointNameAttribTag As String = "POINT_NAME"
		Dim iMapTheme As DMAcadExt.enMapTheme = DMAcadExt.enMapTheme.Fragments

		DMAcadExt.AppMessages.ClearMessages(iMapTheme, 31)
		Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oFragmentTopology IsNot Nothing Then
			Dim colNodes As Autodesk.Gis.Map.Topology.NodeCollection = oFragmentTopology.GetNodes()
			Dim oIdenticalNames As CheckDoubleString = New CheckDoubleString()
			Dim bAcadPoint As Boolean
			Dim sPointName As String
			Dim oNode As Autodesk.Gis.Map.Topology.Node
			Dim tNodeAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim colNodeIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim colNodePoints As System.Collections.Generic.ICollection(Of DMAcadExt.TPlnPoint) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.TPlnPoint)
			Dim saBlockName() As String = UnidivNet.UD_App.GetOldPointBlocks()
			Dim oTplnLine As DMAcadExt.TplnLine
			For Each oNode In colNodes

				If zzNodeEntityExists(oNode, tNodeAcObjID) Then
					sPointName = DMAcadExt.AcadTransaction.GetAttribText(tNodeAcObjID, True, bAcadPoint, sPointNameAttribTag)
					If Not String.IsNullOrEmpty(sPointName) Then
						oIdenticalNames.Add(sPointName, True, oNode.ID)
						'  DMAcadExt.AcadDocument.WriteDebugMessage("21_27:" & oNode.ID() & ", " & sPointName)
					End If
					colNodeIDs.Add(tNodeAcObjID)
					colNodePoints.Add(New DMAcadExt.TPlnPoint(oNode.Location))

				Else

					DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.Y, "", DMCommon.dmMessages.Message(310), False, iMapTheme, 35)

					'DMCommon.Debug.GetMsg("09_110", "Node.Entity Is Null", oNode.Location)
					'DMAcadExt.DMApp.MsgBox("09_110", " oNode.AcObjID.IsNull", oNode.Location)
				End If
			Next
			'	DMCommon.Debug.MsgBox("09_110E", DMCommon.Debug.ColCount(oIdenticalNames.DoubleValues))

			'  DMAcadExt.AcadDocument.SetDrawVectorSet(New DMAcadExt.DrawVectorSet(tVectorSet))

			Dim oQueue As Queue(Of Integer)
			Dim dicDuplicateNames As Generic.Dictionary(Of String, Queue(Of Integer)) = oIdenticalNames.DoubleValues
			Dim tFirstPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim oaLines() As DMAcadExt.TplnLine = Nothing

			Dim colLines As System.Collections.Generic.ICollection(Of DMAcadExt.TplnLine) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.TplnLine)

			Dim iLineIndex As Integer
			If dicDuplicateNames IsNot Nothing Then
				For Each sName As String In dicDuplicateNames.Keys
					oQueue = dicDuplicateNames.Item(sName)
					If oQueue.Count > 1 Then
						ReDim oaLines(oQueue.Count - 1)
						iLineIndex = 0
						tFirstPoint = Autodesk.AutoCAD.Geometry.Point3d.Origin
						For Each iTopoID As Integer In oQueue.ToArray()
							oNode = oFragmentTopology.GetNode(iTopoID)
							If oNode IsNot Nothing Then
								If tFirstPoint <> Autodesk.AutoCAD.Geometry.Point3d.Origin Then
									oTplnLine = New DMAcadExt.TplnLine(tFirstPoint, oNode.Location)
									colLines.Add(oTplnLine)
									iLineIndex += 1
								End If
								DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.Y, "", DMCommon.dmMessages.Message(310, sName), False, iMapTheme, 31)
								tFirstPoint = oNode.Location
							End If
						Next
					End If
				Next

			End If
			oFragmentTopology.Close()
			'MCommon.Debug.MsgBox("09_111", DMCommon.Debug.GetListArray(saBlockName))
			If colLines.Count > 0 Then
				oaLines = colLines.ToArray()
				If oaLines IsNot Nothing Then
					Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(oaLines, oaLines.GetUpperBound(0), 1, True)
					DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, -1)
				End If
			End If


			CheckCloseSPoints(colNodePoints, colNodeIDs, saBlockName)


		End If


	End Sub
	Private Sub zzSetErrValues()

		If moCheckActionsTable IsNot Nothing Then
			For Each oDataRow As DataRow In moCheckActionsTable.Rows
				zzSetErrValue(oDataRow)
			Next
		End If

		'  Dim iErrNumber As Integer = zzGetErrMessagesCount()
		'   DMCommon.Debug.MsgBox("12_421c", ptMapThemeData.MapThemeID, mtTopoRes.TopoExists, IsDoneA, DMAcadExt.AppMessages.GetMessagesCount(ptMapThemeData.MapThemeID))
		If Me.doaLabelCheck(5) IsNot Nothing AndAlso Me.IsDoneA Then
			Me.doaLabelCheck(5).Checked = True
		Else
			Me.doaLabelCheck(5).Checked = False
		End If
	End Sub
	Private Sub zzDeleteTopo(ByVal bDeleteEntities As Boolean)
		Dim oDataRowView As DataRowView
		Dim iPgonCount As Integer
		Dim sTopoLayerName As String
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		Try
			'moTopoDataTable
			'DMCommon.Debug.MsgBox("12_290c", ptMapThemeData.TopoName, ptMapThemeData.MapThemeID, ptMapThemeData.LinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.NodeLayers, bCreateNodes)
			For Each oDataGridViewRow As DataGridViewRow In Me.dgvTopoList.SelectedRows
				oDataRowView = DirectCast(oDataGridViewRow.DataBoundItem, DataRowView)
				iPgonCount = DMCommon.Functions.CIntN(oDataRowView.Item("PolygonCount"))
				If iPgonCount > 0 Then
					sTopoLayerName = DMCommon.Functions.CStrN(oDataRowView.Item("TopoName"))
					TopoCreator.DeleteTopology(sTopoLayerName, bDeleteEntities, False)
					iPgonCount = TopoManager.TopoCreator.GetTopoPolygonCount(sTopoLayerName)
					If iPgonCount = 0 Then
						'oDataRowView.Item("PolygonCount") = 0
						oDataGridViewRow.Cells.Item("ctxPolygonCount").Value = 0
						oDataGridViewRow.Cells.Item("ctxTopoName").Style = Nothing

					End If
					'	zzCheckDispTopo(False, False)
					'zzDispTopoOK()

					If moaTopoErrors IsNot Nothing Then
						Me.txtErrorCount.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
						miCurrentTopoErrIndex = miTopoErrIndexNotErr
						zzDispErrIndex()
					End If
				End If

			Next

			zzSetStyle()


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzCreateTopo_01")
		End Try

		zzCheckDispTopo(False, False)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Function zzGetErrMessagesCount() As Integer
		Return DMAcadExt.AppMessages.GetMessagesCount(ptMapThemeData.MapThemeID, 0, True)
	End Function
	Private Sub zzDeleteTopo_030924(ByVal bDeleteEntities As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		TopoCreator.DeleteTopology(ptMapThemeData.TopoName, bDeleteEntities, False)
		zzCheckDispTopo(False, False)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzDeleteTopoWA(ByVal bDeleteEntities As Boolean)

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		TopoCreator.DeleteTopology(ptMapThemeData.LineTopoName, bDeleteEntities, False)
		zzCheckDispTopoWA(False, False)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzShowTopo()
		'    DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		TopoCreator.ShowTopology(ptMapThemeData.TopoName)

		'    DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzShowTopoWA()
		'  DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		TopoCreator.ShowTopology(ptMapThemeData.LineTopoName)

		'   DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub zzEraseLines()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		DMAcadExt.AcadTransaction.ClearLayerByClassName(ptMapThemeData.LineLinkLayers, DMAcadExt.AcadConst.AcadPolylineName)
		zzGetStatisticsA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzGetStatisticsAAA()

	End Sub
	Private Sub zzGetStatistics()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		zzCreateLayerList()
		zzGetStatisticsA()
		'Me.dgvTopoList.Rows.Add(12)
		'	Me.dgvTopoList.re

		zzCreateTable()


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'zzTest()
	End Sub
	Private Sub zzGetStatisticsA()
		mtMapThemeInfo = DMAcadExt.AcadTransaction.GetStatistics(ptMapThemeData)
		Me.txtArcCount.Text = CStr(mtMapThemeInfo.ArcCount)
		Me.txtCentroidCount.Text = CStr(mtMapThemeInfo.CentroidBlocksCount)
		Me.txtCentroidCountWA.Text = CStr(mtMapThemeInfo.CentroidBlocksCount)
		Me.txtLinkCount.Text = CStr(mtMapThemeInfo.LinkCount)
		Me.txtLinkCountP2.Text = CStr(mtMapThemeInfo.LinkCount)
		Me.txtLineLinkCount.Text = CStr(mtMapThemeInfo.LineLinkCount)
		Me.txtLineLinkCountB.Text = CStr(mtMapThemeInfo.LineLinkCount)
		Me.txtLinkWithArcCount.Text = CStr(mtMapThemeInfo.LinkWithArcCount)
		Me.txtLinkCountWA.Text = CStr(mtMapThemeInfo.LinkCount - mtMapThemeInfo.LineLinkCount)

		'	MessageBox.Show(CStr(tMapThemeInfo.LinkWithArcCount) & ":" & CStr(tMapThemeInfo.LineLinkCount) & ":" & CStr(tMapThemeInfo.LineLinksOK), "01_988")

		zzDispLineLinksExist(mtMapThemeInfo.LineLinksOK)
		If mtMapThemeInfo.CentroidBlocksCount = 0 Then
			Me.chkCreateCentroid.Checked = True
		End If
	End Sub
	Private Sub zzDispLineLinksExist(bExists As Boolean)
		Me.doaLabelCheck(2).Checked = bExists
		Me.lblLineLinksExist.Visible = bExists
		'	Me.txtPgonCount.Enabled = bExists
		If bExists Then
			'	Me.lblTopoName.ForeColor = System.Drawing.SystemColors.ControlText
			'	Me.lblTopoName.Font = doLabelBoldFont
		Else
			'Me.lblTopoName.ForeColor = System.Drawing.SystemColors.GrayText
			'	Me.lblTopoName.Font = doLabelFont
		End If

	End Sub
	Private Function zzGetBreakPointBlocks(ByVal sLayer As String) As IDictionary(Of Long, System.Collections.ObjectModel.Collection(Of BlockReference))
		Dim oVMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.V)
		Dim colBlockRefObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs(oVMarkBlock.BlockName, sLayer)
		Dim oBlockRef As BlockReference
		Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
		Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
		Dim oEmptyBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
		'Dim tLinkHandle As Autodesk.AutoCAD.DatabaseServices.Handle
		Dim sHandleVal As String
		Dim lHandleVal As Long

		Dim dicRes As IDictionary(Of Long, System.Collections.ObjectModel.Collection(Of BlockReference)) = New Dictionary(Of Long, System.Collections.ObjectModel.Collection(Of BlockReference))
		Dim colBlockRefs As System.Collections.ObjectModel.Collection(Of BlockReference) = Nothing
		For Each tAcObjID As ObjectId In colBlockRefObjIDs
			oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead, True)
			oResBuffer = oBlockRef.XData
			If oResBuffer IsNot Nothing Then

				taTypedValues = oResBuffer.AsArray()
				For iIndex As Integer = 0 To taTypedValues.GetUpperBound(0)
					Select Case taTypedValues(iIndex).TypeCode
						Case 1005
							sHandleVal = DirectCast(taTypedValues(iIndex).Value, String)
							lHandleVal = Convert.ToInt64(Val("&H" & sHandleVal))
							If lHandleVal <> 0L Then
								If Not dicRes.TryGetValue(lHandleVal, colBlockRefs) Then
									colBlockRefs = New ObjectModel.Collection(Of BlockReference)()
									dicRes.Add(lHandleVal, colBlockRefs)
								End If
								colBlockRefs.Add(oBlockRef)
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!BreakPointBl", lHandleVal, oBlockRef.Layer, colBlockRefs.Count)
							End If

					End Select
				Next
			End If
		Next
		Return dicRes
	End Function
	Private Sub zzStraighten() 'Straighten

		If mtMapThemeInfo.LineLinkCount = 0 Then


			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			Dim dicBreakPointBlocks As IDictionary(Of Long, System.Collections.ObjectModel.Collection(Of BlockReference)) = zzGetBreakPointBlocks(ptMapThemeData.LineLinkLayers)
			Dim dStraightenTolerance As Double
			Try
				If IsNumeric(Me.txtStraightenTolerance.Text) Then
					dStraightenTolerance = Convert.ToDouble(Me.txtStraightenTolerance.Text)
				Else
					dStraightenTolerance = zzGetDoubleSetting("StraightenTolerance", 0.2)
				End If
			Catch oEx As Exception
				dStraightenTolerance = zzGetDoubleSetting("StraightenTolerance", 0.2)
			End Try

			If dStraightenTolerance > 0.00001 Then
				Dim tList As DMCommon.dmList = New DMCommon.dmList(ptMapThemeData.LineLinkLayers)

				'DMCommon.Debug.MsgBox("151122_1", ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers, dicBreakPointBlocks.Count)
				DMAcadExt.dmLineCleanup.Straighten(ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers, dStraightenTolerance, 0.0, dicBreakPointBlocks)
				'DMAcadExt.dmLineCleanup.Straighten(ptMapThemeData.LinkLayers, tList.First, dStraightenTolerance, 0.0)  17/02/20

				moParams.SetValue(2, dStraightenTolerance)
				moParams.Update()
				zzGetStatisticsA()
			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzNextErr()
		'MessageBox.Show(MyBase.ClientSize.Width & ":" & MyBase.ClientSize.Height & vbCrLf & Me.ClientSize.Width & ":" & Me.ClientSize.Height, "02_422")
		If miTopoPanelIndex = LabelCheck.CurrentIndex Then
			If miCurrentTopoErrIndex < moaTopoErrors.UpperBound Then
				miCurrentTopoErrIndex += 1
				zzDispErrIndex()
				zzShowTopoError()
			End If
		End If
	End Sub

	Private Sub cmdNextErr_Click(oSender As System.Object, e As System.EventArgs) Handles cmdNextErr.Click
		zzNextErr()
	End Sub
	Private Sub cmdNextErrWA_Click(oSender As System.Object, e As System.EventArgs) Handles cmdNextErrWA.Click
		zzNextErr()
	End Sub
	Private Sub zzPrevErr()
		If miTopoPanelIndex = LabelCheck.CurrentIndex Then
			If miCurrentTopoErrIndex >= 0 Then
				miCurrentTopoErrIndex -= 1
				zzDispErrIndex()
				zzShowTopoError()
			End If
		End If
	End Sub
	Private Sub zzSetCheckDescriptionOld()
		Dim oCurrentGridRow As DataGridViewRow = Me.dgvCheckActions.CurrentRow
		If oCurrentGridRow IsNot Nothing Then
			Dim oCurrentDataRow As DataRowView = DirectCast(oCurrentGridRow.DataBoundItem, DataRowView)
			Me.txtDescription.Text = DMCommon.Functions.CStrN(oCurrentDataRow.Item("ActionDescription"))
			'   DMCommon.Debug.MsgBox("12_310", oCurrentGridRow.DataBoundItem.GetType())
		End If

	End Sub
	Private Sub zzSetCheckDescription(iRowIndex As Integer)

		Dim oDataRow As DataRow = moCheckActionsTable.Rows.Item(iRowIndex)
		Me.txtDescription.Text = DMCommon.Functions.CStrN(oDataRow.Item("ActionDescription"))
		'   DMCommon.Debug.MsgBox("12_310", oCurrentGridRow.DataBoundItem.GetType())


	End Sub
	Private Sub cmdPrevErr_Click(oSender As System.Object, e As System.EventArgs) Handles cmdPrevErr.Click
		zzPrevErr()
	End Sub
	Private Sub cmdPrevErrWA_Click(oSender As System.Object, e As System.EventArgs) Handles cmdPrevErrWA.Click
		zzPrevErr()
	End Sub

	Private Sub zzStartErr()
		If miTopoPanelIndex = LabelCheck.CurrentIndex Then
			If moaTopoErrors IsNot Nothing AndAlso moaTopoErrors.UpperBound >= 0 Then
				miCurrentTopoErrIndex = -1
				zzDispErrIndex()
				zzShowTopoError()
			End If
		End If

	End Sub
	Private Sub cmdStartErr_Click(oSender As System.Object, e As System.EventArgs) Handles cmdStartErr.Click
		zzStartErr()
	End Sub
	Private Sub cmdStartErrWA_Click(oSender As System.Object, e As System.EventArgs) Handles cmdStartErrWA.Click
		zzStartErr()
	End Sub

	Private Sub cmdEraseErr_Click(oSender As System.Object, e As System.EventArgs) Handles cmdEraseErr.Click
		zzEraseTopoErrors()

	End Sub
	Private Sub zzEraseCleanupErrors()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzDeleteBlockRefs)
		Dim sErrBlockNameList As String = DMAcadExt.MarkBlock.GetMarkBlockList()
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, , , sErrBlockNameList)
		'	Me.nudErrors(iCleanupIndex).Minimum = Decimal.Zero
		Me.nudErrors(iCleanupIndex).Value = Decimal.Zero
		Me.nudErrors(iCleanupIndex).Maximum = Decimal.Zero
		DMAcadExt.AcadDocument.ClearDrawVectorSet()

		If False Then  'Temp !Add
			If moaTopoErrors IsNot Nothing Then
				moaTopoErrors.Clear()
			End If
		End If

		'	zzDispTopoErrors()
		zzRegen()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzEraseTopoErrors()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzDeleteBlockRefs)

		Dim sErrBlockNameList As String = Join(TopoCreator.TopoErrBlockNames, ",")

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		DMCommon.Debug.MsgBox("13_012b", LabelCheck.CurrentIndex)
		DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, , , sErrBlockNameList)
		If moaTopoErrors IsNot Nothing Then
			moaTopoErrors.Clear()
		End If
		zzDispTopoErrors()
		zzRegen()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzDeleteBlockRefs(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		If bCond Then
			oEntity.Erase()
		End If

	End Sub

	Protected Overrides Sub AfterChangeCurrent(iNewIndex As Integer)
		MyBase.OnChangeCurrent(iNewIndex)
		Dim bTopo As Boolean
		Select Case iNewIndex
			Case 1, 2, 4
				Me.doaPanels(iNewIndex).Controls.Add(Me.tstTopology)
				If iNewIndex = 2 Then
					bTopo = False
				Else
					bTopo = True
				End If
				If True Then
					tsbCreateTopo.Enabled = bTopo
					tsbCheckTopo.Enabled = bTopo
					tsbDeleteTopo.Enabled = bTopo
					tsbShowTopo.Enabled = bTopo
					'	Me.tsbExec.Enabled = bTopo	 'Not
					'	Me.tsbClear.Enabled = bTopo 'Not
				End If
				If bTopo Then
					Me.tsbExec.ToolTipText = msToClosedPgonsText
					Me.tsbClear.ToolTipText = msEraseClosedPgonsText
				Else
					Me.tsbExec.ToolTipText = msExecText
					Me.tsbExecA.ToolTipText = msExecText

					Me.tsbClear.ToolTipText = msEraseText
				End If
				If iNewIndex = 1 Then
					'DMCommon.Debug.MsgBox("!SetTopoTable")
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
					zzCreateLayerList()
					zzLoadData()


					DMAcadExt.AcadTransaction.CloseModelSpace()
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()

					'zzSetStyle()
				End If
		End Select
	End Sub
	Private Sub zzToClosedPgons(bExteriorRingOnly As Boolean)
		Dim bCurrentLayerOK As Boolean = False
		'	Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim oColorPgon As ColorPolygon
		If Not String.IsNullOrEmpty(ptMapThemeData.ClosedPgonsLayers) Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(ptMapThemeData.ClosedPgonsLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)


			If bCurrentLayerOK Then
				Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				If oTopoModel IsNot Nothing Then
					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						oColorPgon = New ColorPolygon(oPolygon)
						oColorPgon.CreateClosedPolygon(bExteriorRingOnly)
						oColorPgon.Terminate()
						oPolygon.Dispose()
						oPolygon = Nothing
					Next
					colPolygons.Dispose()
					colPolygons = Nothing
					oTopoModel.Close()
				End If
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End If

	End Sub

	Private Sub zzToClosedPgonsWA(bExteriorRingOnly As Boolean)
		Dim bCurrentLayerOK As Boolean = False
		'	Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim oColorPgon As ColorPolygon
		If Not String.IsNullOrEmpty(ptMapThemeData.LineClosedPgonsLayers) Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(ptMapThemeData.ClosedPgonsLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)


			If bCurrentLayerOK Then
				Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.LineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				If oTopoModel IsNot Nothing Then
					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						oColorPgon = New ColorPolygon(oPolygon)
						oColorPgon.CreateClosedPolygon(bExteriorRingOnly)
						oColorPgon.Terminate()
						oPolygon.Dispose()
						oPolygon = Nothing
					Next
					colPolygons.Dispose()
					colPolygons = Nothing
					oTopoModel.Close()
				End If
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End If

	End Sub
	Private Sub zzDispErrIndex()
		Try
			Me.txtErrorIndex.Text = Convert.ToString(miCurrentTopoErrIndex + 1)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzDispErrIndex")
		End Try
	End Sub

	Private Sub frmTopoCleanup_Paint(oSender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
		zzPaintForm(e.Graphics)
	End Sub
	Private Sub zzPaintForm(oGraphics As Graphics)
		Dim oPen As Pen = New Pen(Color.Black, 2.0!)
		Dim tPoint1 As Point = New Point(301, 0)
		Dim tPoint2 As Point = New Point(301, Me.ClientSize.Height)

		oGraphics.DrawLine(oPen, tPoint1, tPoint2)
	End Sub

	Private Sub frmTopoCleanup_Load(oSender As System.Object, e As System.EventArgs) Handles MyBase.Load
		zzMyInitializeComponent()
		If False Then
			'	zzRefreshTopoData()
			zzGetStatisticsAAA()
			zzDispTopoOK()
			zzGetStatisticsAAA()
			zzSetErrValue(1, mtTopoRes.MissingCntrCount)
			zzSetErrValue(2, mtTopoRes.OutsideCntrCount)

			zzDispTopoOK_WA()
			zzSetErrValue(1, mtTopoResWA.MissingCntrCount)
			zzSetErrValue(2, mtTopoResWA.OutsideCntrCount)
		End If

		'	zzLoadData()






		zzGetStatistics()

		zzLoadParams()
		zzSetErrValues()
		mbEventsEnabled = True
		'	zzSetStyle()
	End Sub


	Public Overrides ReadOnly Property IsDone As Boolean
		Get
			'   System.Windows.Forms.MessageBox.Show(CStr(mtTopoRes.IsComplete) & ":" & CStr(mtTopoResWA.IsComplete) & vbCrLf & ptMapThemeData.TopoName & vbCrLf & CStr(ptMapThemeData.TopoPriority), "!IsDone")
			'	Return mtTopoRes.IsComplete AndAlso mtTopoResWA.IsComplete
			Dim bTopologyExists As Boolean
			Select Case ptMapThemeData.TopoPriority
				Case 1
					bTopologyExists = mtTopoResWA.IsOK
				Case 2
					If mtTopoResWA.TopoExists Then
						bTopologyExists = mtTopoResWA.IsOK
					Else
						bTopologyExists = mtTopoRes.IsOK
					End If
				Case Else
					Return False
			End Select
			Return bTopologyExists AndAlso (zzGetErrMessagesCount() = 0)


		End Get
	End Property

	Public Overrides ReadOnly Property IsDoneA As Boolean
		Get
			Return zzTopologyExists() AndAlso (zzGetErrMessagesCount() = 0)
		End Get
	End Property
	Public Overrides Sub ExecDefaultAction()
		zzCreateTopo()
	End Sub
	Private Function zzTopologyExists() As Boolean
		Select Case ptMapThemeData.TopoPriority
			Case 1
				Return mtTopoResWA.IsOK
			Case 2
				If mtTopoResWA.TopoExists Then
					Return mtTopoResWA.IsOK
				Else
					Return mtTopoRes.IsOK
				End If
			Case Else
				Return False
		End Select

	End Function



	Protected Overrides Sub Finalize()
		MyBase.
			Finalize()
	End Sub

	Private Sub cmdStraightenLink_Click(oSender As System.Object, e As EventArgs) Handles cmdStraightenLink.Click

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Me.Visible = False
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'  DMAcadExt.AcadDocument.OpenLog(False)
		Dim dTolerance As Double = 0.001
		If Me.txtStraightenTolerance.Text.Length <> 0 Then
			Double.TryParse(Me.txtStraightenTolerance.Text, dTolerance)
		End If

		Do
			oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)

			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				DMAcadExt.dmLineCleanup.StraightenCurve(ptRes.ObjectId, dTolerance, True, "")

			Else
				Exit Do
			End If

		Loop


		DMAcadExt.AcadDocument.CloseLog()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


		Me.Visible = True
	End Sub
	Private Sub cmdInsertBreakPoint_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertBreakPoint.Click

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptEntityOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		Dim oPromptPointOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point ...")

		Dim ptEntityRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult

		Me.Visible = False
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'  DMAcadExt.AcadDocument.OpenLog(False)
		Dim dTolerance As Double = 0.001
		Dim tLinkAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim tLinkHandle As Autodesk.AutoCAD.DatabaseServices.Handle
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oVMarkBlock As DMAcadExt.MarkBlock

		Dim oBlockRef As BlockReference
		If Me.txtStraightenTolerance.Text.Length <> 0 Then
			Double.TryParse(Me.txtStraightenTolerance.Text, dTolerance)
		End If

		'Do
		oEditor.WriteMessage("Select Link ...: " & vbCrLf)
		ptEntityRes = oEditor.GetEntity(oPromptEntityOpt)

		If ptEntityRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			tLinkAcObjID = ptEntityRes.ObjectId
			tLinkHandle = DMAcadExt.AcadTransaction.GetEntity(tLinkAcObjID, OpenMode.ForRead).Handle
		Else
			'Exit Do
		End If

		'Loop


		If tLinkHandle.Value <> 0L Then

			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(ptMapThemeData.LineLinkLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

			oEditor.WriteMessage("Select Link ...: " & vbCrLf)
			ptPointRes = oEditor.GetPoint(oPromptPointOpt)

			If ptPointRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				tInsertPoint = ptPointRes.Value
				'	DMCommon.Debug.MsgBox("141122_1", tInsertPoint.ToString())
				oVMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.V)
				'	DMCommon.Debug.MsgBox("141122_2", oVMarkBlock.BlockName)
				oBlockRef = oVMarkBlock.InsertPoint(tInsertPoint, 2S)
				'	DMCommon.Debug.MsgBox("141122_11", oBlockRef Is Nothing)
				Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
				If bRes Then
					oBlockRef.XData = zzGetBreakPointResBuffer(tLinkHandle)
				End If

				'
			End If
		End If


		'Loop



		DMAcadExt.AcadDocument.CloseLog()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


		Me.Visible = True
	End Sub
	Private Sub zzCreateTable()
		moTopoDataTable = New Data.DataTable("Main")

		moTopoDataTable.Columns.Add("TopoName", System.Type.GetType("System.String"))
		moTopoDataTable.Columns.Add("PolygonCount", System.Type.GetType("System.Int32"))
		moTopoDataTable.Columns.Add("LinkCount", System.Type.GetType("System.Int32"))
		moTopoDataTable.Columns.Add("CentroidCount", System.Type.GetType("System.Int32"))

		'   oDataTable.Columns.Add(msAcObjIDFldName, System.Type.GetType("System.Int32"))
		'   oDataTable.Columns.Add(msAreaFldName, System.Type.GetType("System.Double"))
		'   oDataTable.Columns.Add(msCentroidXFldName, System.Type.GetType("System.Double"))
		'   oDataTable.Columns.Add(msCentroidYFldName, System.Type.GetType("System.Double"))
	End Sub


	Private Sub mfTplnView_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfTplnView.FormClosed
		Me.Visible = True
	End Sub



	Private Sub mfTplnView_VisibleChanged(oSender As System.Object, e As EventArgs) Handles mfTplnView.VisibleChanged
		If Not mfTplnView.Visible Then
			Me.Visible = True
		End If
	End Sub

	Private Sub frmTopoCleanup_Shown(sender As System.Object, e As EventArgs) Handles Me.Shown
		'DMCommon.Debug.MsgBox("!FShownB", Me.Location, Me.Size, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100)
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			Me.Size = New System.Drawing.Size(590, 353)
			Me.WindowState = FormWindowState.Normal
		End If
		'zzSetStyle()
		'DMCommon.Debug.MsgBox("!FShownA", Me.Location, Me.Size)
	End Sub


End Class