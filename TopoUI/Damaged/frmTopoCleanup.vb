Option Explicit On
Option Strict On
Imports TopoManager
Imports FDO
Imports System.Data

Public Class frmTopoCleanup
	Private Const miParamType As Integer = 1
	Const miThisStagesUB As Integer = 4
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


	Const miTopoErrIndexNotErr As Integer = -1

	'	Private WithEvents cmdPrepare As TabButton
	Private moaTopoErrors As TopoErrorArray
	'	Private moaTopoErrorsWA As TopoErrorArray

	Private miCurrentTopoErrIndex As Integer = miTopoErrIndexNotErr

	Private miTopoPanelIndex As Integer = -1
	Private mtMapThemeInfo As DMAcadExt.MapThemeInfo
	'	Protected WithEvents dgvMessages As DataGridView
	'	Protected tbpProjectData As System.Windows.Forms.TabPage
	'	Protected tbpMessages As System.Windows.Forms.TabPage



	'Protected moaCheckButtons(miToposUB) As CheckButton
	'	Protected dbLabelTopoLong As Boolean
	'	Protected txtRepBamashSharedScale As TextBox


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
	Private miStepNum As Integer
	'	Private moDataTable(miToposUB) As System.Data.DataTable
	Private moCleanupActionsTable As System.Data.DataTable




	Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
	Private moTopoErrPoints As DMAcadExt.TplnPointArray
	Private moCurrentTopoErrIndex As Integer = -1





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
	Private nudErrors(1) As System.Windows.Forms.NumericUpDown
	Private cmdEraseCleanupErr(1) As System.Windows.Forms.Button
	Private cmdFix(1) As TabButton
	Private cmdMark(1) As TabButton
	Private lblErrors(1) As System.Windows.Forms.Label
	Private moCurrentView(1) As DataView
#End Region
#Region "Panel1_Declarations"

	Private chkCreateCentroid As System.Windows.Forms.CheckBox
	Private chkHighlightSliver As System.Windows.Forms.CheckBox
	Private lblTolerance As System.Windows.Forms.Label
	Private txtTolerance As System.Windows.Forms.TextBox

	Private grbLinks As System.Windows.Forms.GroupBox
	Private grbCentroids As System.Windows.Forms.GroupBox
	Private grbClosedPgons As System.Windows.Forms.GroupBox
	Private lblCentroidBlocks As System.Windows.Forms.Label
	Private lblCentroidLayers As System.Windows.Forms.Label
	'	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private lblTopoErrors As System.Windows.Forms.Label
	Private txtErrorCount As System.Windows.Forms.TextBox
	Private txtErrorIndex As System.Windows.Forms.TextBox
	Private lblTopoName As System.Windows.Forms.Label
	Private lblTopoNameCap As System.Windows.Forms.Label

	Private WithEvents cmdStartErr As System.Windows.Forms.Button
	Private WithEvents cmdPrevErr As System.Windows.Forms.Button
	Private WithEvents cmdNextErr As System.Windows.Forms.Button
	Private WithEvents cmdEraseErr As System.Windows.Forms.Button

	Private lblTopoExists As System.Windows.Forms.Label
	Private txtPgonCount As System.Windows.Forms.TextBox
	Private txtCentroidCount As System.Windows.Forms.TextBox
	Private txtCentroidBlocks As System.Windows.Forms.TextBox
	Private txtCentroidLayers As System.Windows.Forms.TextBox
	Private lblLinkLayers As System.Windows.Forms.Label
	Private txtLinkLayers As System.Windows.Forms.TextBox
	Private txtLinkCount As System.Windows.Forms.TextBox

	Private lblClosedPgonsLayers As System.Windows.Forms.Label
	Private txtClosedPgonsLayers As System.Windows.Forms.TextBox
	Private txtClosedPgonsCount As System.Windows.Forms.TextBox

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

	'Public Event FormatChangedBBB()
	'	Public Event PaintScaleChangedBBB()
	'	Public Event CalculateBBB()
	'	Public Event AppExitBBB()

	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
		MyBase.New(tMapThemeData)

		'	ptMapThemeData = tMapThemeData


	

		MyBase.SetLabelDim(miThisStagesUB)
		'	MessageBox.Show(CStr(miThisStagesUB) & ":" & CStr(doaPanels.GetUpperBound(0)), "07_030")
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		'
		' Add any initialization after the InitializeComponent() call.

      '    DMAcadExt.AcadDocument.TestAcadDoc("265 Before doc Lock")
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

      '    DMAcadExt.AcadDocument.TestAcadDoc("266 After doc Lock")
      zzCheckTopo(False, False)
      '    DMAcadExt.AcadDocument.TestAcadDoc("310 After Open Topo  ")
      zzCheckTopoWA(False, False) '0100316 
      '   DMAcadExt.AcadDocument.TestAcadDoc("320 After doc Lock")
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


		Dim saCaptions() As String = {"הכנה לטופולוגיה", "טופולוגיה", "יישור קשתות", "הכנה לטופולוגיה ללא קשתות", "טופולוגיה ללא קשתות"}
		Dim saCurrentCaptions() As String = {"", "", "", "הכנה לטופול' ללא קשתות", "טופולוגיה ללא קשתות"}

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


		zzLoadData(0)
		zzTestData("After Load Ini")
		zzLoadData(1)





		zzSetGridColumns(0)
		zzSetGridColumns(1)


	End Sub
	

   Private Sub zzLoadParams()
      moParams = New TPlServerDB.dmParams(piProjectCode, piDetailNo, ptMapThemeData.MapThemeID, miParamType)
      Me.txtTolerance.Text = Convert.ToString(moParams.GetDblValue(0))
      Me.txtToleranceWA.Text = Convert.ToString(moParams.GetDblValue(1))
      Me.txtStraightenTolerance.Text = Convert.ToString(moParams.GetDblValue(2))

   End Sub



	Private Function zzGetCleanupIndex() As Integer
		Select Case LabelCheck.CurrentIndex
			Case 0
				Return 0
			Case 3
				Return 1
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
		AddHandler nudSteps(iCleanupIndex).ValueChanged, AddressOf nudSteps_ValueChanged
		AddHandler dgvActions(iCleanupIndex).RowEnter, AddressOf dgvActions_RowEnter
		AddHandler dgvActions(iCleanupIndex).DataError, AddressOf dgvActions_DataError
		AddHandler Me.cmdFix(iCleanupIndex).Click, AddressOf cmdFix_Click
		AddHandler Me.cmdMark(iCleanupIndex).Click, AddressOf cmdMark_Click
		AddHandler Me.nudErrors(iCleanupIndex).ValueChanged, AddressOf nudErrors_ValueChanged
		AddHandler Me.cmdEraseCleanupErr(iCleanupIndex).Click, AddressOf cmdEraseCleanupErr_Click


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
			.Size = New System.Drawing.Size(294, 224)	'224
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
			.Maximum = Decimal.One + Decimal.One
			.TabIndex = 19
			.Value = Decimal.One
			.Maximum = miStepNum
		End With
		'
		'nudErrors
		'
		With Me.nudErrors(iCleanupIndex)
			.Location = New System.Drawing.Point(216, 4)	 '232, 4
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

		Me.doaPanels(iPanelIndex).ResumeLayout(False)


	End Sub

	Private Sub zzInitPanel1()

		Me.txtPgonCount = New System.Windows.Forms.TextBox()
		Me.lblTopoExists = New System.Windows.Forms.Label()
		Me.cmdStartErr = New System.Windows.Forms.Button()
		Me.cmdPrevErr = New System.Windows.Forms.Button()
		Me.cmdNextErr = New System.Windows.Forms.Button()
		Me.cmdEraseErr = New System.Windows.Forms.Button()

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
		Me.grbLinks = New System.Windows.Forms.GroupBox()
		Me.txtLinkCount = New System.Windows.Forms.TextBox()
		Me.txtLinkLayers = New System.Windows.Forms.TextBox()
		Me.lblLinkLayers = New System.Windows.Forms.Label()

		Me.grbClosedPgons = New System.Windows.Forms.GroupBox()
		Me.txtClosedPgonsCount = New System.Windows.Forms.TextBox()
		Me.txtClosedPgonsLayers = New System.Windows.Forms.TextBox()
		Me.lblClosedPgonsLayers = New System.Windows.Forms.Label()



		Me.txtTolerance = New System.Windows.Forms.TextBox()
		Me.lblTolerance = New System.Windows.Forms.Label()
		Me.chkHighlightSliver = New System.Windows.Forms.CheckBox()
		Me.chkCreateCentroid = New System.Windows.Forms.CheckBox()
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

			.Add(Me.txtPgonCount)
			.Add(Me.lblTopoExists)
			.Add(Me.cmdStartErr)
			.Add(Me.cmdPrevErr)
			.Add(Me.cmdNextErr)
			.Add(Me.cmdEraseErr)

			.Add(Me.txtErrorCount)
			.Add(Me.txtErrorIndex)
			.Add(Me.lblTopoName)
			.Add(Me.lblTopoNameCap)
			'	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154b")
			.Add(Me.tstTopology)
			.Add(Me.lblTopoErrors)
			.Add(Me.grbCentroids)
			.Add(Me.grbLinks)
			.Add(Me.grbClosedPgons)
			.Add(Me.txtTolerance)
			.Add(Me.lblTolerance)
			.Add(Me.chkHighlightSliver)
			.Add(Me.chkCreateCentroid)


		End With
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
			.Location = New System.Drawing.Point(152, 214 + 48)
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
			.Location = New System.Drawing.Point(174, 214 + 48)
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
			.Location = New System.Drawing.Point(192, 214 + 48)
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
			.Location = New System.Drawing.Point(216, 214 + 48)
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
			.Location = New System.Drawing.Point(102, 214 + 48)
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
			.Location = New System.Drawing.Point(60, 214 + 48)
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
			.Location = New System.Drawing.Point(8, 214 + 48)
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
		End With
		'
		'grbLinks
		'
		With Me.grbLinks
			.Controls.Add(Me.txtLinkCount)
			.Controls.Add(Me.txtLinkLayers)
			.Controls.Add(Me.lblLinkLayers)
			.Location = New System.Drawing.Point(8, 96)
			.Name = "grbLinks"
			.Size = New System.Drawing.Size(284, 43)
			.TabIndex = 7
			.TabStop = False
			.Text = "Links"
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
		End With
		'
		'grbClosedPgons
		'
		With Me.grbClosedPgons
			.Controls.Add(Me.txtClosedPgonsCount)
			.Controls.Add(Me.txtClosedPgonsLayers)
			.Controls.Add(Me.lblClosedPgonsLayers)
			.Location = New System.Drawing.Point(8, 213)
			.Name = "grbClosedPgons"
			.Size = New System.Drawing.Size(284, 43 + 24)
			.TabIndex = 17
			.TabStop = False
			.Text = "Closed polygons"
		End With
		'
		'txtClosedPgonsCount
		'
		With Me.txtClosedPgonsCount
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtClosedPgonsCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtClosedPgonsLayers
		'
		With Me.txtClosedPgonsLayers
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtClosedPgonsLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptMapThemeData.ClosedPgonsLayers
			.TabIndex = 16
		End With
		'
		'lblClosedPgonsLayers
		'
		With Me.lblClosedPgonsLayers
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblClosedPgonsLayers"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
		End With

		'
		'txtTolerance
		'
		With Me.txtTolerance
			.Location = New System.Drawing.Point(240, 32)
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
			.Location = New System.Drawing.Point(8, 52)
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
		'''''''''''''''''		Me.Controls.Add(Me.doaPanels(1))
		If False Then
			Me.doaPanels(1).ResumeLayout(False)
			Me.doaPanels(1).PerformLayout()
			Me.grbCentroids.ResumeLayout(False)
			Me.grbCentroids.PerformLayout()
			Me.grbLinks.ResumeLayout(False)
			Me.grbLinks.PerformLayout()
			Me.ResumeLayout(False)
		End If


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
			.Location = New System.Drawing.Point(1, 42)				'18
			.Name = "lblLineLinksExist"
			.Size = New System.Drawing.Size(20, 18)
			.TabIndex = 6
		End With
		'
		'lblLinkLineLayer
		'
		With Me.lblLineLinkLayer
			.Location = New System.Drawing.Point(20, 42)				 '18
			.Name = "lblLinkLineLayer"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layer:"
		End With




		'
		'txtLinkLineLayer
		'
		With Me.txtLineLinkLayer
			.Location = New System.Drawing.Point(66, 39)	 '15
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




		'''''''''''''''''''''''''''''''''''''''''''''''

		'
		'Panel2
		'
		With Me.doaPanels(2)
			.Controls.Add(Me.grbSource)
			.Controls.Add(Me.grbWithoutArcs)
         .Controls.Add(Me.tstTopology)
         .Controls.Add(Me.cmdStraightenLink)
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
			.Location = New System.Drawing.Point(8, 238 + 48 + 24)	 '
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
	 


	

	Private Sub zzRefreshTopoData()
		zzCheckTopo(False, False)

		zzDispTopoOK()

		'	Dim bLineLinksExist As Boolean = TopoManager.TopoCreator.TopologyExists(ptMapThemeData.TopoName)

		zzCheckTopoWA(False, False)

		zzDispTopoOK_WA()

		Me.doaLabelCheck(2).Checked = Not DMAcadExt.AcadTransaction.LayersIsEmpty(ptMapThemeData.LineLinkLayers)


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
						DMAcadExt.AcadDocument.SetView(tTopoError.Point.AcGePoint, 2.0 * tTopoError.Scale, 2.0 * tTopoError.Scale)	'10.0
					Else
						DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
					End If
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - nudErrors_ValueChanged+")
			End Try
		End If
	End Sub
	Private Sub zzTestData(sLabel As String)
		If Me.dgvActions(0) IsNot Nothing Then
			Dim oView As DataView = DirectCast(Me.dgvActions(0).DataSource, DataView)
			Dim oCols As System.Windows.Forms.DataGridViewColumnCollection = Me.dgvActions(0).Columns
			'	System.Windows.Forms.MessageBox.Show(sLabel & vbCrLf & CStr(oView.Count) & vbCrLf & CStr(oCols.Count), "TC:Testdata")
		End If
	End Sub
#Region "CleanupProcedures"
	Private Function zzSetGridColumns(iIndex As Integer) As System.Windows.Forms.DataGridViewComboBoxColumn
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
			.HeaderText = "Tolerance"
			.Width = 58
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
			.HeaderText = "Errors"
			.Width = 40
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
		Return oCmbColumn
	End Function
	Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles nudSteps.ValueChanged
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		Me.zzLoadData(iCleanupIndex)
		zzTestData("After Load St")
	End Sub
	Private Sub dgvActions_RowEnter(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)	'Handles dgvActions.RowEnter
		'	If miCurrentAction <> -1 Then
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		miCurrentAction = e.RowIndex
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction), "21_459")
		zzSetCleanupErrPoints(iCleanupIndex)
		'	End If
	End Sub
	Private Sub dgvActions_DataError(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs)	'Handles dgvActions.DataError
		Dim sMsg As String = "dvgActionsDataErr:" & CStr(e.RowIndex) & "," & CStr(e.ColumnIndex) & "-" & e.Exception.Message
		e.ThrowException = False
		DMAcadExt.AcadDocument.WriteMessage(sMsg)
	End Sub
	Private Sub cmdFix_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()	'' 0 or 1
		zzCleanup(iCleanupIndex, True)
	End Sub
	Private Sub cmdMark_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		zzCleanup(iCleanupIndex, False)
	End Sub
	Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)		'Handles nudErrors.ValueChanged
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Value)
		Dim iMax As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Maximum)
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
	Private Sub cmdEraseCleanupErr_Click(oSender As System.Object, e As System.EventArgs)
		'	MessageBox.Show("", "02_261")
		zzEraseCleanupErrors()
	End Sub
	Protected Sub zzCleanup(ByVal iCleanupIndex As Integer, ByVal bFix As Boolean)
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
               tCleanupOptions.AddAction(iActionID, dTolerance, iIndex)
            End If
         End If
      Next
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

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
                        '  DMCommon.Debug.MsgBox("12_250", oaActionVar.GetUpperBound(0))
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
											End If
											.EndEdit()
										Else
											'' System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
										End If
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
							End If
							.EndEdit()
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
      '     System.Windows.Forms.MessageBox.Show("miRoundingRowIndex = " & tCleanupOptions.RoundingRowIndex.ToString() & vbCrLf & "PointLineRowindex = " & tCleanupOptions.PointLineRowindex.ToString() & vbCrLf & "NetPointsLines = " & tCleanupOptions.NetPointsLines.ToString() & vbCrLf & "RoundingLines = " & tCleanupOptions.RoundingLines.ToString() & vbCrLf & "Rounding = " & tCleanupOptions.Rounding.ToString() & vbCrLf & "RemoveDuplicates = " & tCleanupOptions.RemoveDuplicates & vbCrLf & "RoundingSPoints = " & tCleanupOptions.RoundingSPoints.ToString() & vbCrLf & "PointsNearLines = " & tCleanupOptions.PointsNearLines.ToString() & vbCrLf & tCleanupOptions.RoundingTolerance & vbCrLf & tCleanupOptions.PointLineTolerance.ToString() & vbCrLf & CStr(iCleanupIndex), "05_001")
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      If iCleanupIndex = 0 Then
         If tCleanupOptions.Rounding = DMAcadExt.enMerging.RoundingSPoints Then
            tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupSPoints(bFix, tCleanupOptions)
         ElseIf tCleanupOptions.RoundingLines Then
            ' New Rounding
            tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupRoundingLines(bFix, tCleanupOptions)

         ElseIf tCleanupOptions.NetPointsLines Then
            'QQQQQQQQQQQQQQQQQQQ
            tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupPointsLines(bFix, tCleanupOptions)
         Else
            DMAcadExt.dmLineCleanup.moProgress = prbPaint
            tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupA(bFix, tCleanupOptions)
         End If



      Else
         tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupNew(bFix, tCleanupOptions)
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      Return tCleanupResult
   End Function
	
	Private Sub zzLoadData(iCleanupIndex As Integer)
		Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps(iCleanupIndex).Value)
		Dim sStepNo As String = Convert.ToString(Me.nudSteps(iCleanupIndex).Value)
		Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
		Dim sSelectComText As String
		sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"
		moCleanupActionsTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sSelectComText, CommandType.Text, "CleanupActions")
		moCleanupActionsTable.Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
		moCleanupActionsTable.Columns.Add(msPointsFldName, oPointArray.GetType())
		Dim iRowCount As Integer = moCleanupActionsTable.Rows.Count
      If iRowCount > 0 AndAlso moCleanupActionsTable IsNot Nothing Then
         Dim oRow As DataRow = moCleanupActionsTable.Rows.Item(iRowCount - 1)
         miStepNum = DirectCast(oRow.Item("Step"), Integer)
      End If
		'	MessageBox.Show(CStr(moMapThemeData.CleanupType) & ":" & CStr(iRowCount), "05_200")
		Dim oColumn As DataColumn = moCleanupActionsTable.Columns("Step")
		oColumn.DefaultValue = iStepNo

		moCurrentView(iCleanupIndex) = New DataView(moCleanupActionsTable, "Step=" & sStepNo & "", "", DataViewRowState.CurrentRows)
		Me.dgvActions(iCleanupIndex).DataSource = moCurrentView(iCleanupIndex)
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






	Private Sub tmrInactive_Tick(ByVal oSender As System.Object, ByVal e As System.EventArgs)	'''''''''''''''''''''''' Handles tmrInactive.Tick
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


	Private Sub txtStraightenTolerance_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs)	'Handles txtStraightenTolerance.Leave
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
						zzCheckDispTopo(True, True)
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
				Select Case LabelCheck.CurrentIndex
					Case 1
					Case 2
						zzEraseLines()
					Case 4
				End Select
		End Select
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

	Private Sub zzCreateTopo()
		Dim dTolerance As Double
      Dim bCreateCentroids As Boolean
      Dim bCreateNodes As Boolean

		'	Dim tTopoRes As DMAcadExt.TopoRes

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		If Information.IsNumeric(Me.txtTolerance.Text) Then
			dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
         bCreateCentroids = Me.chkCreateCentroid.Checked
         bCreateNodes = bCreateCentroids
			Try
            '  MessageBox.Show(ptMapThemeData.NodeBlocks & vbCrLf & ptMapThemeData.NodeLayers, "03_324")
            mtTopoRes = TopoCreator.CreateTopology(ptMapThemeData.TopoName, ptMapThemeData.LinkLayers, "", ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.NodeLayers, bCreateNodes, Me.chkHighlightSliver.Checked, dTolerance)
				zzDispTopoOK()
				If mtTopoRes.TopoExists Then
					'	Me.txtPgonCount.Text = CStr(mtTopoRes.PgonCount)
					'	Me.txtLinkCount.Text = CStr(mtTopoRes.LinkCount)
					'	Me.txtCentroidCount.Text = CStr(mtTopoRes.CentroidCount)

					moParams.SetValue(0, dTolerance)
					moParams.Update()
					moaTopoErrors = TopoCreator.GetTopoErrors(enTopoErrType.RefRMark)
					If moaTopoErrors IsNot Nothing Then
						'	System.Windows.Forms.MessageBox.Show(CStr(moaTopoErrors.UpperBound) & ":" & CStr(mtTopoRes.MissingCntrCount), "02_540")
						moaTopoErrors.SourceTopo = True
					End If

				Else
					'zzDispTopoOK()
					moaTopoErrors = TopoCreator.GetAllTopoErrors()
					moaTopoErrors.SourceTopo = True
				End If
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
            mtTopoResWA = TopoCreator.CreateTopology(ptMapThemeData.LineTopoName, ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, ptMapThemeData.NodeBlocks, ptMapThemeData.CentroidLayers, bCreateNodes, Me.chkHighlightSliverWA.Checked, dTolerance)
				zzDispTopoOK_WA()
				If mtTopoResWA.TopoExists Then
					'Me.txtPgonCountWA.Text = CStr(mtTopoResWA.PgonCount)
					'	Me.txtLinkCountWA.Text = CStr(mtTopoResWA.LinkCount)
					'Me.txtCentroidCountWA.Text = CStr(mtTopoResWA.CentroidCount)
					'	Me.doaLabelCheck(1).Checked = True

					moParams.SetValue(1, dTolerance)
					moParams.Update()

					moaTopoErrors = TopoCreator.GetTopoErrors(enTopoErrType.RefRMark)

					If False AndAlso moaTopoErrors IsNot Nothing Then
						System.Windows.Forms.MessageBox.Show(CStr(moaTopoErrors.UpperBound) & ":" & CStr(mtTopoResWA.MissingCntrCount), "02_544")
					End If

				Else
					'Me.moaLabelCheck(4).Checked = False
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
		Dim bOK As Boolean = mtTopoRes.IsOK
		Me.doaLabelCheck(1).Checked = bOK
		Me.lblTopoExists.Visible = bOK
		Me.txtPgonCount.Enabled = bOK
		If bOK Then
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoName.Font = doLabelBoldFont

		Else
			Me.lblTopoName.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoName.Font = doLabelFont
		End If
		Me.txtPgonCount.Text = Convert.ToString(mtTopoRes.PgonCount)
		If mtTopoRes.HasElements Then
			Me.txtLinkCount.Text = CStr(mtTopoRes.LinkCount)
			Me.txtCentroidCount.Text = CStr(mtTopoRes.CentroidCount)
		End If

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
		zzDispTopoOK()

	End Sub
   Private Sub zzCheckTopo(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
      '  MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & bLockDoc, "05_338")
      mtTopoRes = TopoCreator.CheckTopo(ptMapThemeData.TopoName, bLockDoc, bMsgBox)
      '  MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & bLockDoc & ":" & CStr(mtTopoRes.IsOK), "05_339")
   End Sub

	Private Sub zzCheckDispTopoWA(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
		zzCheckTopoWA(bLockDoc, bMsgBox)
		zzDispTopoOK_WA()
	End Sub

   Private Sub zzCheckTopoWA(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
      '   MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & "+", "05_438")
      mtTopoResWA = TopoCreator.CheckTopo(ptMapThemeData.LineTopoName, bLockDoc, bMsgBox)
      '   MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString & ":" & CStr(mtTopoResWA.IsOK), "05_439")
   End Sub

	Private Sub zzDeleteTopo(ByVal bDeleteEntities As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		TopoCreator.DeleteTopology(ptMapThemeData.TopoName, bDeleteEntities, False)
		zzCheckDispTopo(False, False)
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzDeleteTopoWA(ByVal bDeleteEntities As Boolean)

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		TopoCreator.DeleteTopology(ptMapThemeData.LineTopoName, bDeleteEntities, False)
		zzCheckDispTopoWA(False, False)
		DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzShowTopo()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      TopoCreator.ShowTopology(ptMapThemeData.TopoName)

      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzShowTopoWA()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      TopoCreator.ShowTopology(ptMapThemeData.LineTopoName)

      DMAcadExt.AcadDocument.Unlock()
   End Sub

	Private Sub zzEraseLines()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		DMAcadExt.AcadTransaction.ClearLayerList(ptMapThemeData.LineLinkLayers)
		zzGetStatisticsA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzGetStatistics()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

		zzGetStatisticsA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

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

	Private Sub zzStraighten()	'Straighten

		If mtMapThemeInfo.LineLinkCount = 0 Then


			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

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
            '  MessageBox.Show(CStr(dStraightenTolerance) & ":" & CStr(9999), "01_936")
				DMAcadExt.dmLineCleanup.Straighten(ptMapThemeData.LinkLayers, tList.First, dStraightenTolerance)
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


		If False Then	'Temp !Add
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
		'	zzRefreshTopoData()
		zzGetStatistics()
		zzDispTopoOK()
		zzDispTopoOK_WA()
		zzLoadParams()
	End Sub
	Public Overrides ReadOnly Property IsDone As Boolean
		Get
            '   System.Windows.Forms.MessageBox.Show(CStr(mtTopoRes.IsComplete) & ":" & CStr(mtTopoResWA.IsComplete) & vbCrLf & ptMapThemeData.TopoName & vbCrLf & CStr(ptMapThemeData.TopoPriority), "!IsDone")
			'	Return mtTopoRes.IsComplete AndAlso mtTopoResWA.IsComplete
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

		End Get
	End Property

	Public Overrides ReadOnly Property IsDoneA As Boolean
		Get
			Return mtTopoResWA.IsOK
		End Get
	End Property




	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

   Private Sub cmdStraightenLink_Click(oSender As System.Object, e As EventArgs) Handles cmdStraightenLink.Click

      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Me.Visible = False
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      DMAcadExt.AcadDocument.OpenLog(False)
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
End Class