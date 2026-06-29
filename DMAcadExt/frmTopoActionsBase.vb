Option Explicit On
Option Strict On
Imports TopoManager
Public Class frmTopoActionsBase

	Const msSwitchCleanupText As String = "  Cleanup"
	Const msCreateTopoText As String = "יצירת טופולוגיה"
	Const msCheckTopoText As String = "בדיקת טופולוגיה"

	Const msDeleteTopoText As String = "מחיקת טופולוגיה"
	Const msShowGeometryText As String = "הצגת גיאומטריה"
	Const msEraseTopoGeoText As String = "מחיקת טופולוגיה וגיאומטריה"
	Const msTopoPropertiesText As String = "מאפייני טופולוגיה"


	Protected WithEvents dgvActions As DataGridView
	Protected WithEvents tabMain As TabControl
	Protected tbpTopo As System.Windows.Forms.TabPage
	Protected tbpApplication As System.Windows.Forms.TabPage
	Protected tbpParameters As System.Windows.Forms.TabPage
	Protected WithEvents dgvMessages As DataGridView
	Protected tbpProjectData As System.Windows.Forms.TabPage
	Protected tbpMessages As System.Windows.Forms.TabPage
	Protected dbTabsInit() As Boolean
	Protected miToposUB As Integer = TopoDefs.giToposUB
	Protected WithEvents dfSelectLanduse As frmSelectLanduse
	Protected WithEvents dfEditColorScheme As frmColorEditor
	Protected moOleDbDataAdapter(miToposUB) As System.Data.Common.DbDataAdapter
	Protected moaCheckButtons(miToposUB) As CheckButton
	Protected dbLabelTopoLong As Boolean
	Private WithEvents nudSteps As System.Windows.Forms.NumericUpDown
	Private WithEvents nudErrors As System.Windows.Forms.NumericUpDown
	Private WithEvents cmdEraseTopoGeometria As TabButton
	Private WithEvents cmdCopyFromOverlay As TabButton
	Private WithEvents tsbSwitchCleanup As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbCreateTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbCheckTopo As System.Windows.Forms.ToolStripButton


	Private WithEvents tsbDeleteTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbShowTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbEraseTopoGeometria As System.Windows.Forms.ToolStripButton
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
	Private tsiDel1 As System.Windows.Forms.ToolStripMenuItem



	Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
	Private WithEvents tstTopo As System.Windows.Forms.ToolStrip

	Protected WithEvents chkCheckTopo As CheckBox
	'''''''''''''	Private WithEvents tmrInactive As System.Windows.Forms.Timer
	Private miActionDflt As Integer = 0
	Protected Const msActionIDFldName As String = "ActionID"
	Protected Const msToleranceFldName As String = "Tolerance"
	Protected Const msErrorsFldName As String = "Errors"
	Protected Const msPointsFldName As String = "Points"
	Protected Const msCalculateSettingKey As String = "Calculate"
	Protected Const msLotNameNumSettingKey As String = "LotNameNum"
	Protected Const msCheckTopoSettingKey As String = "CheckTopo"
	Protected Const msBlueLineSettingKey As String = "BlueLine"
	Protected Const msPaintStraightenSettingKey As String = "PaintStraighten"
	Protected Const msStraightenToleranceSettingKey As String = "StraightenTolerance"



	Private Shared miBaseResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmActionsBase
	Protected Shared diResourceTheme As TPlServerDB.enResourceTheme

	Protected doSelectedReportItem As ReportItem
	Protected dsReportBlockFolder As String
	Protected doReportApp As AcadReport.Application
	Protected dbAutocad As Boolean 'False - Excel

	Private moPriorView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = Nothing
	Private chkCreateCentroid As CheckBox
	Private chkHighlightSliver As CheckBox
	Private txtTolerance As TextBox
	Private txtTopoErrors As TextBox
	Private lblTolerance As Label
	Private lblErrors As Label

	Private moaErrorPoints() As DMAcadExt.TplnPointArray = Nothing
	Protected miCurrentTopoDefID As DMAcadExt.TopoDefID
	Private miCurrentAction As Integer = -1
	Private miStepNum(miToposUB) As Integer
	Private moDataTable(miToposUB) As System.Data.DataTable
	Protected doaLabels(miToposUB) As LabelInd
	Private WithEvents cmdRegen As TabButton
	Private WithEvents cmdClose As TabButton	 '
	Private WithEvents cmdExit As TabButton

	Protected moaChecks(miToposUB) As TopoCheck
	Protected doProjectData As ProjectData	'''''''''''''''''''''''''''''
	Protected txtStraightTolerance As TextBox


	Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
	Private moTopoErrPoints As DMAcadExt.TplnPointArray
	Private moCurrentTopoErrIndex As Integer = -1

	Private WithEvents cmdFix As TabButton
	Private WithEvents cmdMark As TabButton
	Private WithEvents cmdPrepare As TabButton

	Private WithEvents cmdBuild As TabButton
	Private WithEvents cmdKill As TabButton
	Private WithEvents cmdShow As TabButton
	Private WithEvents cmdToClosedPgons As TabButton
	Private WithEvents chkTopoLayersOn As CheckBox

	Private mbCodeExecuting As Boolean = False
	Private mbInactive As Boolean
	Private mdInactiveTiks As Double = 0.0
	Private moCurrentLabel As LabelInd = Nothing
	Private moLabelColor As System.Drawing.Color = Color.DimGray
	Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue
	Private moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private moCurrentView As DataView

	Private txtProjectData() As TextBox
	Private cmbProjectData() As ComboBox
	Private lblProjectData() As Label

	Private WithEvents cmdSaveProjectData As TabButton
	Private WithEvents cmdSaveParameters As TabButton

	Private WithEvents cmdZoomMsg As TabButton
	Private WithEvents cmdClearMsg As TabButton

	Public Event FormatChanged()
	Public Event Calculate()
	Public Event AppExit()
	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()
		'
		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
		DMAcadExt.AcadDocument.SetLogName()
		Me.DialogResult = System.Windows.Forms.DialogResult.No
	End Sub

	Protected Sub OnCalculate()
		RaiseEvent Calculate()
	End Sub
	Protected Sub OnFormatChanged()
		RaiseEvent FormatChanged()
	End Sub
	Private Sub zzMyInitializeComponent()
		Me.moComponents = New System.ComponentModel.Container
		Me.ClientSize = New System.Drawing.Size(540, 480)	';(540, 300)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Location = New System.Drawing.Point(200, 200)
		Me.tabMain = New System.Windows.Forms.TabControl
		Me.tbpTopo = New System.Windows.Forms.TabPage
		Me.tbpApplication = New System.Windows.Forms.TabPage
		Me.tbpParameters = New System.Windows.Forms.TabPage
		Me.tbpProjectData = New System.Windows.Forms.TabPage
		Me.tbpMessages = New System.Windows.Forms.TabPage
		Me.chkCreateCentroid = New CheckBox
		Me.chkHighlightSliver = New CheckBox
		Me.txtTolerance = New TextBox
		Me.txtTopoErrors = New TextBox
		Me.lblTolerance = New Label
		Me.lblErrors = New Label

		Me.dgvActions = New System.Windows.Forms.DataGridView
		Me.nudSteps = New System.Windows.Forms.NumericUpDown
		Me.nudErrors = New System.Windows.Forms.NumericUpDown
		Me.cmdFix = New TabButton(moLabelFont, False)
		Me.cmdMark = New TabButton(moLabelFont, False)
		Me.cmdPrepare = New TabButton(moLabelFont, False)
		Me.cmdBuild = New TabButton(moLabelFont, True)
		Me.cmdKill = New TabButton(moLabelFont, True)
		Me.cmdShow = New TabButton(moLabelFont, True)
		Me.cmdToClosedPgons = New TabButton(moLabelFont, True)

		Me.chkTopoLayersOn = New CheckBox

		Me.cmdRegen = New TabButton(moLabelFont, False)
		Me.cmdClose = New TabButton(moLabelFont, False)
		Me.cmdExit = New TabButton(moLabelFont, False)
		Me.cmdEraseTopoGeometria = New TabButton(moLabelFont, True)
		Me.cmdCopyFromOverlay = New TabButton(moLabelFont, True)

		Me.tsbSwitchCleanup = New System.Windows.Forms.ToolStripButton
		Me.tsbCreateTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbCheckTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbDeleteTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbShowTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbEraseTopoGeometria = New System.Windows.Forms.ToolStripButton
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

		Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton
		Me.tstTopo = New System.Windows.Forms.ToolStrip
		Try
			'''''''''''Me.tmrInactive = New System.Windows.Forms.Timer(Me.moComponents)
		Catch oEx As Exception

		End Try
		Me.tstTopo.SuspendLayout()
		Me.tbpTopo.SuspendLayout()
		Me.tabMain.SuspendLayout()
		CType(Me.dgvActions, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()





		Me.Controls.Add(Me.tabMain)

		Me.tabMain.Controls.Add(Me.tbpTopo)
		Me.tabMain.Controls.Add(Me.tbpApplication)
		Me.tabMain.Controls.Add(Me.tbpProjectData)
		Me.tabMain.Controls.Add(Me.tbpParameters)
		Me.tabMain.Controls.Add(Me.tbpMessages)



		Me.tbpTopo.Controls.Add(Me.cmdFix)
		Me.tbpTopo.Controls.Add(Me.cmdMark)
		Me.tbpTopo.Controls.Add(Me.cmdPrepare)
		Me.tbpTopo.Controls.Add(Me.tstTopo)
		'	Me.tbpTopo.Controls.Add(Me.cmdBuild)
		'	Me.tbpTopo.Controls.Add(Me.cmdKill)
		'	Me.tbpTopo.Controls.Add(Me.cmdShow)
		'	Me.tbpTopo.Controls.Add(Me.cmdToClosedPgons)
		'	Me.tbpTopo.Controls.Add(Me.cmdEraseTopoGeometria)
		'	Me.tbpTopo.Controls.Add(Me.cmdCopyFromOverlay)

		Me.tbpTopo.Controls.Add(Me.chkTopoLayersOn)
		Me.tbpTopo.Controls.Add(Me.chkHighlightSliver)
		Me.tbpTopo.Controls.Add(Me.chkCreateCentroid)
		Me.tbpTopo.Controls.Add(Me.txtTolerance)
		Me.tbpTopo.Controls.Add(Me.txtTopoErrors)
		Me.tbpTopo.Controls.Add(Me.nudSteps)
		Me.tbpTopo.Controls.Add(Me.nudErrors)
		Me.tbpTopo.Controls.Add(Me.lblTolerance)
		Me.tbpTopo.Controls.Add(Me.lblErrors)
		ReDim dbTabsInit(Me.tabMain.Controls.Count - 1)
		'
		'tabMain
		'
		Me.tabMain.Location = New System.Drawing.Point(0, 0)
		Me.tabMain.Name = "tabMain"
		Me.tabMain.SelectedIndex = 0
		Me.tabMain.Size = New System.Drawing.Size(549, 300)
		Me.tabMain.Font = moLabelFont
		Me.tabMain.TabIndex = 7
		dbTabsInit(Me.tabMain.SelectedIndex) = True
		'
		'tbpTopo
		'
		With Me.tbpTopo
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpTopo"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 8
			.Text = "Topology"
			.Font = moLabelFont
			.UseVisualStyleBackColor = True
			'.ContextMenuStrip = Me.cmsMain
		End With
		'
		'tbpApplication
		'
		With Me.tbpApplication
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpApplication"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 6
			.Text = "Application"
			.UseVisualStyleBackColor = True
		End With
		'
		'tbpProjectData
		'
		With Me.tbpProjectData
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpProjectData"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 12
			.Text = "Project Data"
			.UseVisualStyleBackColor = True
		End With

		'
		'tbpParameters
		'
		With Me.tbpParameters
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpParameters"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 9
			.Text = "Parameters"
			.UseVisualStyleBackColor = True
		End With


		'
		'tbpMessages
		'
		With Me.tbpMessages
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpWinText"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 10
			.Text = "Text"
			.UseVisualStyleBackColor = True
		End With

		'
		'tsbSwitchCleanup
		'
		With Me.tsbSwitchCleanup
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Cleanup
			.Name = "tsbSwitchCleanup"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = msCreateTopoText
		End With
		'
		'tsbCreateTopo
		'
		With Me.tsbCreateTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
			.Name = "tsbCreateTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = msCreateTopoText
		End With
		'
		'tsbCheckTopo
		'
		With Me.tsbCheckTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Validate1
			.Name = "tsbCheckTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = msCheckTopoText
		End With


		'
		'tsbDeleteTopo
		'
		With tsbDeleteTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Delete
			.Name = "tsbDeleteTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = msDeleteTopoText
		End With
		'
		'tsbShowTopo
		'
		With tsbShowTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
			.Name = "tsbShowTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = msShowGeometryText
		End With
		'
		'tsbEraseTopoGeometria
		'
		With tsbEraseTopoGeometria
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources._Erase
			.Name = "tsbEraseTopoGeometria"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = "Delete topologia and geometria"
		End With
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
			.Image = Global.TopoUI.My.Resources.Resources.Export16
			.Name = "tsbExportToShape"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = "Export Shape"
		End With

		'
		'ToolStripDropDownButton1
		'
		Me.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		'	Me.ToolStripDropDownButton1.Image = CType(Resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
		Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
		Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(29, 22)
		Me.ToolStripDropDownButton1.Text = ""
		'
		'tstTopo
		'
		With Me.tstTopo
			.Dock = System.Windows.Forms.DockStyle.None
			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbSwitchCleanup, Me.tsbCreateTopo, Me.tsbCheckTopo, Me.tsbDeleteTopo, Me.tsbShowTopo, Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.ToolStripDropDownButton1})
			.Location = New System.Drawing.Point(300, 2)
			.Name = "tstTopo"
			.Size = New System.Drawing.Size(87, 25)
			.TabIndex = 0
		End With
		'
		'dgvActions
		'
		With Me.dgvActions
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(4, 32)
			.Name = "dgvActions"
			.Font = moLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 24
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = moBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(294, 224)	'224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False
		End With

		Me.tbpTopo.Controls.Add(Me.dgvActions)

		'
		'txtTolerance
		'
		With Me.txtTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(438, 38)
			.Name = "txtTolerance"
			.Size = New System.Drawing.Size(36, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "0.01"
			.TextAlign = HorizontalAlignment.Left
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
			.Font = moLabelFont
			.Text = ""
			.TextAlign = HorizontalAlignment.Left
			.ReadOnly = True
		End With
		'
		'lblTolerance
		'
		With Me.lblTolerance
			'.Location = New System.Drawing.Point(342, 86)
			.Location = New System.Drawing.Point(474, 40)
			.Name = "lblTolerance"
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "Tolerance"

		End With
		'
		'lblErrors
		'
		With Me.lblErrors
			.Location = New System.Drawing.Point(188, 6)
			.Name = "lblErrors"
			.Size = New System.Drawing.Size(44, 24)
			.TabIndex = 29
			.Font = moLabelFont
			.Text = "Err:"

		End With
		'
		'nudSteps
		'
		With Me.nudSteps
			.Location = New System.Drawing.Point(4, 4)
			.Name = "nudSteps"
			.Size = New System.Drawing.Size(32, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.One
			.Maximum = Decimal.One + Decimal.One
			.TabIndex = 19
		End With
		'
		'nudErrors
		'
		With Me.nudErrors
			.Location = New System.Drawing.Point(210, 4)	 '232, 4
			.Name = "nudErrors"
			.Size = New System.Drawing.Size(42, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.Zero
			.Maximum = Decimal.Zero
			.TabIndex = 20
		End With

		'
		'cmdShow
		'
		With Me.cmdShow
			.Location = New System.Drawing.Point(370, 52)
			.Name = "cmdShow"
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			'.Text = "Show"
		End With

		'
		'cmdEraseTopoGeometria
		'

		With Me.cmdEraseTopoGeometria
			.Location = New System.Drawing.Point(406, 52)
			.Name = "cmdEraseTopoGeometria"
			.Image = Global.TopoUI.My.Resources.Resources._Erase
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			'	.Text = "Erase"
		End With
		'
		'cmdCopyFromOverlay
		'

		With Me.cmdCopyFromOverlay
			.Location = New System.Drawing.Point(442, 52)
			.Name = "cmdCopyFromOverlay"
			.Image = Global.TopoUI.My.Resources.Resources.SaveFormDesignHS
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 32
			'    .Font = moLabelFont
			'.Text = "Save"
		End With

		'
		'cmdFix
		'
		With Me.cmdFix
			.Location = New System.Drawing.Point(40, 4)
			.Name = "cmdFix"
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 9
			'   .Font = moLabelFont
			.Text = "Fix"
		End With
		'
		'cmdMark
		'
		With Me.cmdMark
			.Location = New System.Drawing.Point(88, 4)
			.Name = "cmdMark"
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			.Text = "Mark"
		End With

		'
		'cmdPrepare
		'
		With Me.cmdPrepare
			.Location = New System.Drawing.Point(136, 4)
			.Name = "cmdPrepare"
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'     .Font = moLabelFont
			.Text = "Prep"

		End With
		'
		'cmdBuild
		'
		With Me.cmdBuild
			.Location = New System.Drawing.Point(298, 52)
			.Name = "cmdBuild"
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 11

			'   .Font = moLabelFont
			'.Text = "Build"
		End With

		'
		'cmdKill
		'
		With Me.cmdKill
			.Location = New System.Drawing.Point(334, 52)
			.Name = "cmdKill"
			.Image = Global.TopoUI.My.Resources.Resources.Delete
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Kill"
		End With
		'
		'cmdKill
		'
		With Me.cmdToClosedPgons
			.Location = New System.Drawing.Point(478, 52)
			.Name = "cmdToClosedPgons"
			.Image = Global.TopoUI.My.Resources.Resources.ToClosedPgons
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "clPgon"
		End With

		'
		'chkTopoLayersOn
		'
		With Me.chkTopoLayersOn
			.Location = New System.Drawing.Point(402, 36)
			.Name = "chkTopoLayersOn"
			.Size = New System.Drawing.Size(96, 24)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "Layers On"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = False
		End With
		'
		'chkCreateCentroid
		'
		With Me.chkCreateCentroid
			' .Location = New System.Drawing.Point(302, 60)
			.Location = New System.Drawing.Point(376, 36)
			.Name = "chkIgnoreIncompleteArea"
			.Size = New System.Drawing.Size(60, 24)
			.TabIndex = 16
			.Font = moLabelFont
			.Text = "Insert"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With
		'
		'chkHighlightSliver
		'
		With Me.chkHighlightSliver
			.Location = New System.Drawing.Point(302, 36)
			.Name = "chkHighlightSliver"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "Highlight"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Checked = True
		End With
		'
		'cmdRegen
		'
		With Me.cmdRegen
			.Location = New System.Drawing.Point(336, 244)
			.Name = "cmdRegen"
			.Size = New System.Drawing.Size(52, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "Regen"
			.ContextMenuStrip = Me.cmsMain
		End With

		'
		'cmdClose
		'
		With Me.cmdClose
			.Location = New System.Drawing.Point(396, 244)
			.Name = "cmdClose"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "Close"
			.ContextMenuStrip = Me.cmsMain
		End With
		'
		'cmdExit
		'
		With Me.cmdExit
			.Location = New System.Drawing.Point(456, 244)
			.Name = "cmdExit"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 15
			'    .Font = moLabelFont
			.Text = "Exit"
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

		zzSetCommonButtons(0)
		miCurrentTopoDefID = TopoDefs.miaTopoIDs(miActionDflt)
		Me.nudSteps.Value = Decimal.One

		zzLoadData(miActionDflt)
		Me.nudSteps.Maximum = miStepNum(miActionDflt)
		zzSetGridColumns()

	End Sub
	Protected Sub OnNew()
		Const iXTop As Integer = 72
		Dim oTopoDef As DMAcadExt.TopoDef
		'	Dim iYPosIndex As Integer
		'	Dim iXShift As Integer
		'	Dim iLabelLength As Integer
		'	Dim iLabelRightX As Integer
		'	Dim bLongLabel As Boolean
		Dim tGroupLocation(3) As GroupLocation
		Dim tTopoLocation(miToposUB) As TopoLocation
		tGroupLocation(0).GroupXShift = 0
		tGroupLocation(0).GroupYShiftIndex = 0
		tGroupLocation(0).LabelLength = 68
		tGroupLocation(0).LabelRightX = 492

		tGroupLocation(1).GroupXShift = 112
		tGroupLocation(1).GroupYShiftIndex = -3
		tGroupLocation(1).LabelLength = 80
		tGroupLocation(1).LabelRightX = 380


		tGroupLocation(2).GroupXShift = 0
		tGroupLocation(2).GroupYShiftIndex = -3
		tGroupLocation(2).LabelLength = 192
		tGroupLocation(2).LabelRightX = 492

		tGroupLocation(3).GroupXShift = 240
		tGroupLocation(3).GroupYShiftIndex = -8
		tGroupLocation(3).LabelLength = 192
		tGroupLocation(3).LabelRightX = 252






		For iIndex As Integer = 0 To miToposUB
			oTopoDef = TopoDefs.moaTopoDefs(iIndex)
			If iIndex < 3 Then
				tTopoLocation(iIndex).BaseLocation = tGroupLocation(0)
			ElseIf iIndex < 6 Then
				tTopoLocation(iIndex).BaseLocation = tGroupLocation(1)
			ElseIf iIndex < 8 Then
				tTopoLocation(iIndex).BaseLocation = tGroupLocation(2)
			Else
				tTopoLocation(iIndex).BaseLocation = tGroupLocation(3)
			End If
			tTopoLocation(iIndex).YPosIndex = iIndex

			Me.doaLabels(iIndex) = New LabelInd(iIndex, tTopoLocation(iIndex).LabelLength, tTopoLocation(iIndex).LabelRightX, iXTop + 4 + 24 * tTopoLocation(iIndex).GetYPosIndex())
			Me.doaLabels(iIndex).ContextMenuStrip = Me.cmsMain
			If iIndex = miActionDflt Then
				Me.doaLabels(iIndex).Font = moLabelSelectFont
				Me.doaLabels(iIndex).ForeColor = moLabelSelectColor
			Else
				Me.doaLabels(iIndex).Font = moLabelFont
				Me.doaLabels(iIndex).ForeColor = moLabelColor
			End If
			Me.moaChecks(iIndex) = New TopoCheck(Me.cmsMain)
			Me.moaChecks(iIndex).ContextMenuStrip = Me.cmsMain
			Me.moaCheckButtons(iIndex) = New CheckButton(iIndex)

			'	Me.moaLabels(iIndex).Location = New System.Drawing.Point(iLabelX, iXTop + 4 + 24 * iYPosIndex)
			Me.moaChecks(iIndex).Location = New System.Drawing.Point(496 - tTopoLocation(iIndex).GroupXShift, iXTop + 24 * tTopoLocation(iIndex).GetYPosIndex())
			Me.moaCheckButtons(iIndex).Location = New System.Drawing.Point(514 - tTopoLocation(iIndex).GroupXShift, iXTop + 2 + 24 * tTopoLocation(iIndex).GetYPosIndex())

			Me.tbpTopo.Controls.Add(Me.doaLabels(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaChecks(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaCheckButtons(iIndex))

			AddHandler doaLabels(iIndex).Click, AddressOf lblTopo_Click
			AddHandler doaLabels(iIndex).DoubleClick, AddressOf lblTopo_DoubleClick
			AddHandler moaCheckButtons(iIndex).CheckStateChanged, AddressOf CheckButton_CheckStateChanged
			Me.moaChecks(iIndex).Checked = False
			Me.moaChecks(iIndex).Checked = TopoManager.TopoCreator.TopologyExists(oTopoDef)
		Next

		moCurrentLabel = Me.doaLabels(miActionDflt)
		Me.zzTopologiesLayersIsOn()
		oTopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			'Me.cmdEraseTopoGeometria.Enabled = oTopoDef.LinkLayersExists
			Me.cmdCopyFromOverlay.Enabled = oTopoDef.ID.TopoIsMerge
			zzSetButtonActionEnable(oTopoDef)

		End If
		Me.tstTopo.ResumeLayout(False)
		Me.tstTopo.PerformLayout()

		Me.tbpTopo.ResumeLayout(False)
		Me.tbpTopo.PerformLayout()

		Me.tabMain.ResumeLayout(False)
		Me.tabMain.PerformLayout()
		Me.dgvActions.Visible = True
		CType(Me.dgvActions, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Protected Sub OnNewSource()
		Const iXTop As Integer = 72
		Dim oTopoDef As DMAcadExt.TopoDef
		Dim iYPosIndex As Integer
		Dim iXShift As Integer
		Dim iLabelLength As Integer
		Dim iLabelRightX As Integer
		Dim bLongLabel As Boolean


		For iIndex As Integer = 0 To miToposUB
			oTopoDef = TopoDefs.moaTopoDefs(iIndex)
			bLongLabel = (iIndex >= 3) AndAlso (iIndex <= 6)
			If iIndex >= 7 Then
				iYPosIndex = iIndex - 7
				iXShift = 112
				iLabelRightX = 380
				iLabelLength = 80
			Else
				iYPosIndex = iIndex
				iXShift = 0
				If bLongLabel Then
					iLabelRightX = 492
					iLabelLength = 192
				Else
					iLabelRightX = 492
					iLabelLength = 68
				End If
			End If
			Me.doaLabels(iIndex) = New LabelInd(iIndex, iLabelLength, iLabelRightX, iXTop + 4 + 24 * iYPosIndex)
			Me.doaLabels(iIndex).ContextMenuStrip = Me.cmsMain
			If iIndex = miActionDflt Then
				Me.doaLabels(iIndex).Font = moLabelSelectFont
				Me.doaLabels(iIndex).ForeColor = moLabelSelectColor
			Else
				Me.doaLabels(iIndex).Font = moLabelFont
				Me.doaLabels(iIndex).ForeColor = moLabelColor
			End If
			Me.moaChecks(iIndex) = New TopoCheck(Me.cmsMain)
			Me.moaChecks(iIndex).ContextMenuStrip = Me.cmsMain
			Me.moaCheckButtons(iIndex) = New CheckButton(iIndex)

			'	Me.moaLabels(iIndex).Location = New System.Drawing.Point(iLabelX, iXTop + 4 + 24 * iYPosIndex)
			Me.moaChecks(iIndex).Location = New System.Drawing.Point(496 - iXShift, iXTop + 24 * iYPosIndex)
			Me.moaCheckButtons(iIndex).Location = New System.Drawing.Point(514 - iXShift, iXTop + 2 + 24 * iYPosIndex)

			Me.tbpTopo.Controls.Add(Me.doaLabels(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaChecks(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaCheckButtons(iIndex))

			AddHandler doaLabels(iIndex).Click, AddressOf lblTopo_Click
			AddHandler doaLabels(iIndex).DoubleClick, AddressOf lblTopo_DoubleClick
			AddHandler moaCheckButtons(iIndex).CheckStateChanged, AddressOf CheckButton_CheckStateChanged
			Me.moaChecks(iIndex).Checked = False
			Me.moaChecks(iIndex).Checked = TopoManager.TopoCreator.TopologyExists(oTopoDef)
		Next

		moCurrentLabel = Me.doaLabels(miActionDflt)
		Me.zzTopologiesLayersIsOn()
		oTopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			'Me.cmdEraseTopoGeometria.Enabled = oTopoDef.LinkLayersExists
			Me.cmdCopyFromOverlay.Enabled = oTopoDef.ID.TopoIsMerge
			zzSetButtonActionEnable(oTopoDef)
		End If
		Me.tstTopo.ResumeLayout(False)
		Me.tstTopo.PerformLayout()

		Me.tbpTopo.ResumeLayout(False)
		Me.tbpTopo.PerformLayout()

		Me.tabMain.ResumeLayout(False)
		Me.tabMain.PerformLayout()

		CType(Me.dgvActions, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private Sub zzSetButtonActionEnable(ByVal oTopoDef As DMAcadExt.TopoDef)
		Dim bEnabled As Boolean
		If Me.dgvActions.Visible Then
			bEnabled = oTopoDef.CleanupEnabled
		Else
			bEnabled = False
		End If
		Me.cmdFix.Enabled = bEnabled
		Me.cmdMark.Enabled = bEnabled
		Me.cmdPrepare.Enabled = bEnabled
	End Sub
	Private Sub dgvActions_DataError(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgvActions.DataError
		Dim sMsg As String = "dvgActionsDataErr:" & CStr(e.RowIndex) & "," & CStr(e.ColumnIndex) & "-" & e.Exception.Message
		e.ThrowException = False
		DMAcadExt.AcadDocument.WriteMessage(sMsg)
	End Sub
	Private Sub dgvActions_RowEnter(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvActions.RowEnter
		'	If miCurrentAction <> -1 Then
		miCurrentAction = e.RowIndex
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction), "21_459")
		zzSetCleanupErrPoints()
		'	End If
	End Sub
	Private Sub zzCalcTopoErrPoints()
		Dim saTopoErrBlockRefs() As String = {DMAcadExt.AcadConst.TopoErrBlockRefOMark _
		, DMAcadExt.AcadConst.TopoErrBlockRefRMark _
		, DMAcadExt.AcadConst.TopoErrBlockRefSMark _
		, DMAcadExt.AcadConst.TopoErrBlockRefTMark}

		Dim oAddPointArray As DMAcadExt.TplnPointArray
		moTopoErrPoints = New DMAcadExt.TplnPointArray()
		moCurrentTopoErrIndex = moCurrentLabel.Index
		For iIndex As Integer = 0 To saTopoErrBlockRefs.GetUpperBound(0)
			oAddPointArray = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint(saTopoErrBlockRefs(iIndex))
			If oAddPointArray IsNot Nothing Then
				moTopoErrPoints.Add(oAddPointArray)
			End If
		Next
		Me.zzSetTopoErr()
	End Sub
	Private Sub zzSetCleanupErrPoints()
		Dim oDataRow As DataRow = moCurrentView.Item(miCurrentAction).Row
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
			With Me.nudErrors
				.Maximum = Convert.ToDecimal(iErrorCount)
				.Value = Decimal.Zero
			End With
			Me.txtTopoErrors.Text = " - "
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetCleanupErrPoints")
		End Try
	End Sub
	Public Sub RefreshTopo()
		zzResetChecks()
	End Sub
	Protected Sub InsertReport(ByVal oDataView As DataView, ByVal iaDataColumns() As Integer, ByVal oaTotals() As System.Object, ByVal oaOptionValues() As System.Object)
		If oDataView IsNot Nothing Then
			Dim oRepApp As AcadReport.BaseReport
			Dim bCurrentLayerOK As Boolean = True
			If dbAutocad Then
				oRepApp = New AcadReport.Application
			Else
				oRepApp = New ExcelReport.Application
			End If
			If oRepApp.Open(doSelectedReportItem.RepIndex) Then
				oRepApp.MainView = oDataView
				If iaDataColumns IsNot Nothing Then
					oRepApp.DataColumns = iaDataColumns
				End If
				If oaTotals IsNot Nothing Then
					oRepApp.Totals = oaTotals
				End If
				If oaOptionValues(0) IsNot Nothing Then
					oRepApp.OptionValues = oaOptionValues
				End If
				If oRepApp.AcadModel Then
					DMAcadExt.AcadDocument.SaveVarCmdDia(0)

					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					AcadReport.Report.InitDWGScaleFactor()
					AcadReport.Report.Init()
					bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

					Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
					Dim taColorScheme() As DMAcadExt.ColorScheme
					Dim oReportApp As AcadReport.Application
					If bCurrentLayerOK Then
						Me.Hide()
						Common.SetAcadFocus()
						oRepApp.Insert()
						oReportApp = DirectCast(oRepApp, AcadReport.Application)
						If AcadReport.Report.AcadTable IsNot Nothing Then
							''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
							'''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
							'''''''''''''''''	AcadReport.Report.ReDrawTable()
							colaPoints = oReportApp.GetColorCells()
							doReportApp = oReportApp
							If colaPoints IsNot Nothing Then
								taColorScheme = oReportApp.GetColorScheme()
								Dim oPgon As SimplePgon
								'	Dim tColorZebra As DMAcadExt.ColorZebra

								For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
									oPgon = New SimplePgon(colaPoints(iIndex))
									oPgon.SetTagNum("Table", iIndex)
									DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
									'		tColorZebra = taColorScheme(iIndex).Zebra
									'	tColorZebra.Scale = bmBamash.ZebraWidthTable / bmBamash.ZebraWidthDrawing 'AcadReport.Report.DrawingScaleFactor *
									'	tColorZebra.Scale = 0.25
									'		oPgon.PaintZebra(tColorZebra)

									oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, taColorScheme(iIndex), False)
									DMAcadExt.AcadTransaction.InsertNewBlock(False)
								Next
								doReportApp.ClearZebraCells()
							End If

						End If
						Me.Show()
						DMAcadExt.AcadTransaction.CloseModelSpace()
						DMAcadExt.AcadTransaction.Terminate()
						DMAcadExt.AcadDocument.Unlock()
						DMAcadExt.AcadDocument.RestoreVarCmdDia()
						'''''''''''''''	zzPaintTable()

					End If
				Else
					oRepApp.Insert()
				End If
			End If
		End If

	End Sub
	Protected Sub InsertBlockReport(ByVal dicAttribValues As Dictionary(Of String, String), ByVal sBlockName As String)
		If dicAttribValues IsNot Nothing Then
			Dim bCurrentLayerOK As Boolean = True
			AcadReport.Report.InitDWGScaleFactor()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, 2, DMAcadExt.enLayerFunction.Report, True, True, True)

			Dim oBlockReport As AcadReport.BlockReport = New AcadReport.BlockReport(dsReportBlockFolder, sBlockName)
			oBlockReport.AttribValues = dicAttribValues
			Me.Hide()
			Common.SetAcadFocus()
			oBlockReport.InsertBlockRef()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.RestoreVarCmdDia()
		End If

	End Sub
	Private Sub UpdateTopoByMerge()
		''''''''''	UpdateTopoByMerge()
	End Sub
	Private Sub zzResetChecks()
		For iIndex As Integer = 0 To miToposUB
			If TopoManager.TopoDefs.moaTopoDefs(iIndex) IsNot Nothing Then
				moaChecks(iIndex).Checked = TopoManager.TopoCreator.TopologyExists(TopoManager.TopoDefs.moaTopoDefs(iIndex))
			End If
		Next
	End Sub

	Private Sub lblTopo_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)

		Try
			Dim oLabelInd As LabelInd
			oLabelInd = DirectCast(oSender, LabelInd)
			If moCurrentLabel IsNot Nothing AndAlso moCurrentLabel IsNot oLabelInd Then
				moCurrentLabel.Font = moLabelFont
				moCurrentLabel.ForeColor = moLabelColor
			End If
			moCurrentLabel = oLabelInd
			oLabelInd.Font = moLabelSelectFont
			oLabelInd.ForeColor = moLabelSelectColor
			miCurrentTopoDefID = TopoDefs.miaTopoIDs(oLabelInd.Index)

			Me.dgvActions.Columns.Clear()
			Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
			zzSetButtonActionEnable(oTopoDef)

			If miCurrentTopoDefID.TopoIsUnion OrElse miCurrentTopoDefID.TopoIsAdditional Then
				Me.nudSteps.Enabled = False
			Else
				zzLoadData(oLabelInd.Index)
				With Me.nudSteps
					.Maximum = miStepNum(oLabelInd.Index)
					Try
						.Value = Decimal.One
					Catch oEx As Exception
					End Try
					.Enabled = (.Maximum > 1)
				End With
				zzSetGridColumns()


				If oTopoDef IsNot Nothing Then
					'	Me.cmdEraseTopoGeometria.Enabled = oTopoDef.LinkLayersExists
					Me.cmdCopyFromOverlay.Enabled = oTopoDef.ID.TopoIsMerge
				End If
			End If
			If moCurrentTopoErrIndex = moCurrentLabel.Index Then
				zzSetTopoErr()
			End If
			'   zzTopoLayersIsOn()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - lblTopo_Click")
		End Try

	End Sub
	Private Sub zzSetTopoErr()
		moCurrentPoints = moTopoErrPoints
		Dim iErrorCount As Integer = moCurrentPoints.UpperBound + 1
		Try
			moPriorView = DMAcadExt.AcadDocument.GetCurrentView()
			With Me.nudErrors
				.Maximum = Convert.ToDecimal(iErrorCount)
				.Value = Decimal.Zero
			End With
			Me.txtTopoErrors.Text = Convert.ToString(iErrorCount)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetTopoErrPoints")
		End Try
	End Sub
	Private Sub zzShowTopo()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			TopoCreator.ShowTopology(oTopoDef)
		End If

	End Sub
	Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles nudSteps.ValueChanged
		If moCurrentLabel IsNot Nothing Then
			Me.zzLoadData(moCurrentLabel.Index)
		End If
	End Sub
	Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles nudErrors.ValueChanged
		Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors.Value)
		If iErrIndex = 0 OrElse (moCurrentPoints Is Nothing) Then
			If moPriorView Is Nothing Then
				TPlanGraph.TplnProject.SetInitView()
			Else
				DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
			End If
		Else
			Try
				If moCurrentPoints Is Nothing Then
					DMAcadExt.AcadDocument.WriteMessage("_11 moCurrentPoints Is Nothing")
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
	Private Sub cmdShow_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdShow.Click
		Me.zzShowTopo()
	End Sub
	Public Sub Mark()
		zzCleanup(False)
	End Sub
	Private Sub cmdPrepare_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdPrepare.Click
		zzPrepare()
	End Sub
	Private Sub cmdMark_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdMark.Click
		zzCleanup(False) 'zzCleanup141108
	End Sub
	Private Sub cmdToClosedPgons_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdToClosedPgons.Click
		zzToClosedPgons()
	End Sub
	Private Sub cmdBuild_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdBuild.Click
		Me.zzCreateTopo()
	End Sub
	Private Sub cmdKill_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdKill.Click
		Me.Cursor = Cursors.WaitCursor
		Me.zzDeleteTopo(False)
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdCopyFromOverlay_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdCopyFromOverlay.Click
		Me.Cursor = Cursors.WaitCursor
		Me.zzCopyFromOverlay()

		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub cmdEraseTopoGeometria_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdEraseTopoGeometria.Click
		Me.Cursor = Cursors.WaitCursor
		Me.zzEraseTopoGeometria()
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzCopyFromOverlay()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		If oTopoDef IsNot Nothing Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			DMAcadExt.dmLineCleanup.UpdateTopoByMerge(oTopoDef.Name)
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub


	Private Sub zzCreateTopo()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim tTopoDefID As DMAcadExt.TopoDefID
		Dim iTopoTypeIndex As Integer

		If oTopoDef IsNot Nothing Then

			Me.Cursor = Cursors.WaitCursor
			tTopoDefID = oTopoDef.ID
			Dim dTolerance As Double
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			If tTopoDefID.TopoIsUnion Then
				TopoCreator.UnionTopo(oTopoDef)
			ElseIf tTopoDefID.TopoIsDissolve Then
				TopoCreator.DissolveTopo(oTopoDef)
			Else
				If Information.IsNumeric(Me.txtTolerance.Text) Then
					dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
					oTopoDef.CreateCentroid = Me.chkCreateCentroid.Checked
					Try
						TopoCreator.CreateTopology(DMAcadExt.DMApp.AppID, oTopoDef, False, Me.chkHighlightSliver.Checked, dTolerance, False)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzCreateTopo_01")
					End Try
				Else
					Beep()
				End If
			End If

			iTopoTypeIndex = moCurrentLabel.Index
			Me.moaChecks(iTopoTypeIndex).Checked = TopoCreator.TopologyExists(oTopoDef)
			If Me.moaChecks(iTopoTypeIndex).Checked Then
				Me.txtTopoErrors.Text = " - "
			Else
				zzCalcTopoErrPoints()
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End If

	End Sub
	Public Sub RefreshLayers()
		Me.zzTopologiesLayersIsOn()
	End Sub
	Public Sub CreateTopo()
		Me.zzCreateTopo()
	End Sub
	Public Sub DeleteTopo()
		Me.zzDeleteTopo(False)
	End Sub
	Private Sub cmdFix_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdFix.Click
		zzCleanup(True)
	End Sub
	Private Sub lblTopo_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Me.zzCreateTopo()
	End Sub
	Private Sub chkTopoLayersOn_CheckStateChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles chkTopoLayersOn.CheckStateChanged
		zzSetCheckState()
	End Sub
	Private Sub cmdZoomMsg_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdZoomMsg.Click
		Dim oDataRowView As DataRowView
		Dim oDataRow As DataRow
		Try
			Dim oViewRow As DataGridViewRow = Me.dgvMessages.CurrentRow
			If oViewRow IsNot Nothing Then
				oDataRowView = DirectCast(oViewRow.DataBoundItem, DataRowView)
				oDataRow = oDataRowView.Row
				DMAcadExt.AppMessages.Zoom(oDataRow)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdZoomMsg_Click")
		End Try
	End Sub
	Private Sub cmdClearMsg_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClearMsg.Click
		Try
			DMAcadExt.AppMessages.Clear()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdClearMsg_Click")
		End Try
	End Sub

	Protected Overridable Sub zzInitTabApplication()

	End Sub
	Private Sub zzToClosedPgons()
		Dim bCurrentLayerOK As Boolean = False
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim oColorPgon As ColorPolygon
		If oTopoDef IsNot Nothing Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			If oTopoDef.ClosedPgonsLayer.Length <> 0 Then
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(oTopoDef.ClosedPgonsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
			End If

			If bCurrentLayerOK Then
				Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(oTopoDef.Name, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				If oTopoModel IsNot Nothing Then
					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						oColorPgon = New ColorPolygon(oPolygon)
						oColorPgon.CreateClosedPolygon()
					Next
				End If
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End If

	End Sub
	Private Sub zzExportToShape()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		If TopoCreator.TopologyExists(oTopoDef) Then
			Dim oExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
			Dim oRes As Autodesk.Gis.Map.ImportExport.ExportResults
			Dim sFileName As String = "E:\aWork\TestMapExp\p105.shp"
			Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
			Dim sTopoName As String = oTopoDef.Name

			'		System.Windows.Forms.MessageBox.Show(sFileName & vbCrLf & sTopoName, "01_807")
			Try
				oExporter.Init("SHP", sFileName)
				'		oMyExporter.LoadExportFormat()	'FileOneEntityType
				oExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, "")
				oExporter.SetExportFromPolygonTopology(False, sTopoName)

				colExpressionTarget = oExporter.GetExportDataMappings()
				colExpressionTarget.Add(":ID@TPMCNTR_" & sTopoName, "ID")
				colExpressionTarget.Add("'2679", "TopoID")
				'		System.Windows.Forms.MessageBox.Show(CStr(colExpressionTarget.Count), "01_808")

				oExporter.SetExportDataMappings(colExpressionTarget)
				'	oMyExporter.ExportFromPolygonTopology(False, sTopoName)
				oRes = oExporter.Export()
				System.Windows.Forms.MessageBox.Show(CStr(oRes.EntitiesExported), "01_815")

			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_817")
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")

			End Try
		End If
	End Sub
	Private Sub cmdClose_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
		Me.Hide()
	End Sub

	Private Sub cmdExit_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click
		Me.DialogResult = System.Windows.Forms.DialogResult.Yes
		Me.Cursor = Cursors.WaitCursor
		If doProjectData IsNot Nothing AndAlso doProjectData.Opened Then
			doProjectData.CloseDictionary()
		End If

		If dfSelectLanduse IsNot Nothing Then
			Me.dfSelectLanduse.Close()
		End If
		If dfEditColorScheme IsNot Nothing Then
			Me.dfEditColorScheme.Close()
		End If
		'	MessageBox.Show("", "01_988")
		TopoManager.TPlanGraph.TplnProject.Close()
		BamashNet.bmBamash.Close()
		RaiseEvent AppExit()
		Me.Cursor = Cursors.Default
		Me.Close()

	End Sub
	Private Sub cmdSaveProjectData_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveProjectData.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		Dim iTextBoxUB As Integer = doProjectData.ControlTypes.GetUpperBound(0)
		Dim iControlsUB As Integer = doProjectData.ControlTypes.GetUpperBound(0)
		Dim oItemData As DMCommon.ItemData

		For iIndex As Integer = 0 To iControlsUB
			Select Case doProjectData.ControlTypes(iIndex)
				Case ProjectData.enControlType.TextBox
					doProjectData.Item(iIndex) = Me.txtProjectData(iIndex).Text
					doProjectData.dmItem(iIndex) = New DMCommon.DMValue(Me.txtProjectData(iIndex).Text)
				Case Is >= ProjectData.enControlType.ComboBox
					Dim sTest As String
					sTest = CStr(Me.cmbProjectData(iIndex).SelectedIndex)
					If Me.cmbProjectData(iIndex).SelectedIndex <> -1 Then
						oItemData = DirectCast(Me.cmbProjectData(iIndex).SelectedItem, DMCommon.ItemData)
						Select Case doProjectData.ControlTypes(iIndex)
							Case ProjectData.enControlType.ComboBoxColorSet
								doProjectData.dmItem(iIndex) = New DMCommon.DMValue(oItemData.ListIndex, DMCommon.DMValue.enDataType.Integer)
							Case ProjectData.enControlType.ComboBoxScale
								doProjectData.dmItem(iIndex) = New DMCommon.DMValue(oItemData.ListDispData, DMCommon.DMValue.enDataType.String)
						End Select

					End If
					'doProjectData.Item(iIndex) = Me.cmbProjectData(iIndex).SelectedValue
			End Select

		Next

		doProjectData.Update()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadTransaction.Terminate()
	End Sub
	Private Sub zzRegen()
		Dim oAcadEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Try
			DMAcadExt.AcadDocument.Regen()
			'	Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzRegen")
		End Try


	End Sub
	Private Sub cmdRegen_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdRegen.Click
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadDocument.Unlock()

		DMAcadExt.AcadTransaction.Terminate()

		Me.zzRegen()
		Me.Cursor = Cursors.Default
	End Sub

	Protected MustOverride Sub OpenProjectData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)
	Protected MustOverride Sub SetProjectData()


	Protected Sub InitTabProjectData()
		Dim sTest As String = "a"
		Dim tStripTopLeft As Drawing.Point = New Point(Convert.ToInt32(Me.tbpProjectData.Width / 2), 6)
		Dim iStripIndex As Integer
		Dim iStripNum As Integer = 0

		Dim iLocationX As Integer
		Dim iLocationY As Integer
		Dim iControlValWidth As Integer = 100

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		sTest = "b"

		OpenProjectData(True, True)
		If Not doProjectData.Opened Then
			System.Windows.Forms.MessageBox.Show("??????????????????", "01_780")
			doProjectData.OpenData(True, True)
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		Dim iControlsUB As Integer = doProjectData.ControlTypes.GetUpperBound(0)

		Try
			sTest = "ba"

			ReDim Me.txtProjectData(iControlsUB)
			ReDim Me.cmbProjectData(iControlsUB)
			ReDim Me.lblProjectData(iControlsUB)
			For iIndex As Integer = 0 To iControlsUB
				sTest = "c"

				sTest = "ca"
				If iIndex = 8 Then
					iStripNum = 1
					tStripTopLeft = New Point(4, 6)
				End If
				If iStripNum = 0 Then
					iStripIndex = iIndex
				Else
					iStripIndex = iIndex - 8
				End If
				Me.lblProjectData(iIndex) = New Label
				sTest = "cb"
				iLocationX = tStripTopLeft.X + 4
				iLocationY = tStripTopLeft.Y + 28 * iStripIndex
				Select Case doProjectData.ControlTypes(iIndex)
					Case ProjectData.enControlType.TextBox

						Me.txtProjectData(iIndex) = New TextBox
						sTest = "cbt"
						'
						'txtProjectData
						'
						With Me.txtProjectData(iIndex)
							.Location = New System.Drawing.Point(iLocationX, iLocationY)	 'iLocationX
							.Name = "txtProjectData_" & Convert.ToString(iIndex)
							.Size = New System.Drawing.Size(iControlValWidth, 20)
							.TabIndex = 20 + iIndex
							.Font = moLabelFont
							.Text = doProjectData.dmItem(iIndex).GetString()
							.TextAlign = HorizontalAlignment.Right
						End With
						sTest = "ck"
						Me.tbpProjectData.Controls.Add(Me.txtProjectData(iIndex))
						sTest = "cl"
					Case Is >= ProjectData.enControlType.ComboBox
						Me.cmbProjectData(iIndex) = New ComboBox
						'
						'cmbProjectData
						'
						sTest = "cbc"
						'MessageBox.Show(CStr(doProjectData.dmItem(iIndex).IntValue), "18_778")
						Select Case doProjectData.ControlTypes(iIndex)
							Case ProjectData.enControlType.ComboBoxColorSet
								TPlanGraph.TplnProject.FillColorSets(Me.cmbProjectData(iIndex), doProjectData.dmItem(iIndex).IntValue)
							Case ProjectData.enControlType.ComboBoxScale
								TPlanGraph.TplnProject.FillScalesA(Me.cmbProjectData(iIndex))
								Me.cmbProjectData(iIndex).SelectedText = doProjectData.dmItem(iIndex).StrValue
						End Select



						With Me.cmbProjectData(iIndex)
							sTest = "cbc1"
							.Location = New System.Drawing.Point(iLocationX, iLocationY)
							.Name = "cmbProjectData_" & Convert.ToString(iIndex)
							.Size = New System.Drawing.Size(iControlValWidth, 20)
							.TabIndex = 20 + iIndex
							sTest = "cbc2"
							.Font = moLabelFont
							sTest = "cbc3"
							.SelectedValue = doProjectData.dmItem(iIndex).IntValue
							sTest = "cbc4"
							.RightToLeft = Windows.Forms.RightToLeft.Yes
							'	.TextAlig = HorizontalAlignment.Right
						End With
						sTest = "ck"
						Me.tbpProjectData.Controls.Add(Me.cmbProjectData(iIndex))
						sTest = "cl"
				End Select


				'
				'lblProjectData
				'
				sTest = "d" & Convert.ToString(iIndex)
				With Me.lblProjectData(iIndex)
					.Location = New System.Drawing.Point(iLocationX + iControlValWidth, iLocationY)
					.Name = "lblTolerance_" & Convert.ToString(iIndex)
					.Size = New System.Drawing.Size(160, 20)
					.TabIndex = 30 + iIndex
					.Font = moLabelFont
					.Text = zzGetText(iIndex, 6)
					.TextAlign = ContentAlignment.MiddleLeft
					.RightToLeft = Windows.Forms.RightToLeft.Yes
				End With
				Me.tbpProjectData.Controls.Add(Me.lblProjectData(iIndex))
				sTest = "e"
			Next
			sTest = "f"
			Me.cmdSaveProjectData = New TabButton(moLabelFont, False)
			'
			'cmdSaveProjectData
			'
			With Me.cmdSaveProjectData
				.Location = New System.Drawing.Point(100, 244)
				.Name = "cmdSaveProjectData"
				'	.Size = New System.Drawing.Size(88, 24)
				.TabIndex = 32

				.Text = "Save"
			End With

			Me.tbpProjectData.Controls.Add(Me.cmdSaveProjectData)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "frmTopoActionsBase - zzInitTabProjectData")
		End Try

		SetProjectData()
	End Sub
	Protected Overridable Sub zzInitTabParameters()

	End Sub
	Protected Sub OnInitTabParameters()
		Me.cmdSaveParameters = New TabButton(moLabelFont, False)
		'cmdSaveParameters
		'cmdSaveProjectData
		'
		With Me.cmdSaveParameters
			.Location = New System.Drawing.Point(100, 244)
			.Name = "cmdSaveParameters"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 32
			.Text = "Save"
		End With
		Me.tbpParameters.Controls.Add(Me.cmdSaveProjectData)
	End Sub
	Private Sub zzSetCommonButtons(ByVal iTabIndex As Integer)
		Select Case iTabIndex
			Case 0
				Me.tbpTopo.Controls.Add(Me.cmdRegen)
				Me.tbpTopo.Controls.Add(Me.cmdClose)
				Me.tbpTopo.Controls.Add(Me.cmdExit)
			Case 1
				Me.tbpApplication.Controls.Add(Me.cmdRegen)
				Me.tbpApplication.Controls.Add(Me.cmdClose)
				Me.tbpApplication.Controls.Add(Me.cmdExit)
			Case 2
				Me.tbpProjectData.Controls.Add(Me.cmdRegen)
				Me.tbpProjectData.Controls.Add(Me.cmdClose)
				Me.tbpProjectData.Controls.Add(Me.cmdExit)
			Case 3
				Me.tbpParameters.Controls.Add(Me.cmdRegen)
				Me.tbpParameters.Controls.Add(Me.cmdClose)
				Me.tbpParameters.Controls.Add(Me.cmdExit)
			Case 4
				Me.tbpMessages.Controls.Add(Me.cmdRegen)
				Me.tbpMessages.Controls.Add(Me.cmdClose)
				Me.tbpMessages.Controls.Add(Me.cmdExit)
		End Select
	End Sub
	Private Sub tabMain_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
		Dim sTest As String = "a"
		Dim iSelectedTabIndex As Integer = Me.tabMain.SelectedIndex
		sTest = "b"
		Try
			If Not dbTabsInit(iSelectedTabIndex) Then
				sTest = "b"
				Select Case iSelectedTabIndex
					Case 1
						sTest = "b"
						Me.zzInitTabApplication()
					Case 2
						sTest = "c"
						InitTabProjectData()
						sTest = "d"
					Case 3
						zzInitTabParameters()
					Case 4
						zzInitTabMessages()
				End Select
				sTest = "q"
				dbTabsInit(iSelectedTabIndex) = True
				sTest = "t"
			End If
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try
		zzSetCommonButtons(iSelectedTabIndex)
	End Sub
	Protected Overridable Sub zzInitTabMessages()
		Me.dgvMessages = New System.Windows.Forms.DataGridView

		dbTabsInit(Me.tabMain.SelectedIndex) = True
		'
		'dgvMessages
		'
		With Me.dgvMessages
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(2, 2)
			.Name = "dgvMessages"
			.Font = moLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 24
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = moBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(530, 224)	'224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False
			.AllowUserToAddRows = False
			.AllowUserToDeleteRows = False
		End With
		Me.tbpMessages.Controls.Add(Me.dgvMessages)
		zzSetMessagesGridColumns()

		Me.dgvMessages.DataSource = DMAcadExt.AppMessages.MsgTable

		Me.cmdZoomMsg = New TabButton(moLabelFont, False)
		'
		'cmdZoomMsg
		'
		With Me.cmdZoomMsg
			.Location = New System.Drawing.Point(100, 244)
			.Name = "cmdZoomMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12
			.Text = "Zoom"
		End With
		Me.tbpMessages.Controls.Add(Me.cmdZoomMsg)

		Me.cmdClearMsg = New TabButton(moLabelFont, False)

		'
		'cmdClearMsg
		'
		With Me.cmdClearMsg
			.Location = New System.Drawing.Point(160, 244)
			.Name = "cmdClearMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 13
			.Text = "Clear"
		End With
		Me.tbpMessages.Controls.Add(Me.cmdClearMsg)
	End Sub
	Private Sub zzSetMessagesGridColumns()	'''''As System.Windows.Forms.DataGridViewComboBoxColumn
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oColumn As DataGridViewTextBoxColumn

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.Name = msToleranceFldName
			.HeaderText = "X"
			.Width = 70
			.Name = "X"
			.DataPropertyName = "X"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvMessages.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
		End Try


		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.Name = msToleranceFldName
			.HeaderText = "Y"
			.Width = 70
			.Name = "Y"
			.DataPropertyName = "Y"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvMessages.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
		End Try

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.HeaderText = "Text"
			.Width = 346
			.Name = "Text"
			.ReadOnly = True
			.DataPropertyName = "Text"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvMessages.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_1")
		End Try

	End Sub
	Private Sub CheckButton_CheckStateChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oCheckButton As CheckButton
		Dim iTopoIndex As Integer
		Dim iCheckState As System.Windows.Forms.CheckState

		oCheckButton = DirectCast(oSender, CheckButton)
		iTopoIndex = oCheckButton.Index
		oCheckButton.SetImage()
		If Not mbCodeExecuting Then
			Dim saTopoLayers() As String = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()
			iCheckState = DMAcadExt.AcadTransaction.SetLayersOn(1, saTopoLayers, oCheckButton.Checked, True)
			If oCheckButton.CheckState <> iCheckState Then
				mbCodeExecuting = True
				oCheckButton.CheckState = iCheckState
				mbCodeExecuting = False
			End If
			DMAcadExt.AcadDocument.UpdateScreen()
			zzTopologiesLayersIsOn()
		End If
	End Sub
	Private Sub zzTopologiesLayersIsOn(Optional ByVal iExcept As Integer = -1)
		Dim saTopoLayers() As String
		mbCodeExecuting = True
		DMAcadExt.AcadTransaction.Start()
		For iTopoIndex As Integer = 0 To miToposUB
			If iTopoIndex <> iExcept Then
				saTopoLayers = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()
				moaCheckButtons(iTopoIndex).CheckState = DMAcadExt.AcadTransaction.LayersIsOn(1, saTopoLayers, False)
			End If
		Next
		DMAcadExt.AcadTransaction.Terminate()
		mbCodeExecuting = False
	End Sub
	Private Sub zzDeleteTopo(ByVal bDeleteEntities As Boolean)
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		If oTopoDef IsNot Nothing Then
			TopoCreator.DeleteTopology(oTopoDef, bDeleteEntities)
			Dim iTopoTypeIndex As Integer
			iTopoTypeIndex = moCurrentLabel.Index
			Me.moaChecks(iTopoTypeIndex).Checked = TopoCreator.TopologyExists(oTopoDef)
		End If
	End Sub
	Private Sub zzCheckTopo()
		'בדיקה
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim sMsg As String
		If oTopoDef IsNot Nothing Then
			sMsg = "Topology " & oTopoDef.Name & " is "
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)

			Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoCreator.GetOpenedTopology(oTopoDef.Name, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			If oTopoModel IsNot Nothing Then
				Try
					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					Next
					If oTopoModel.IsComplete Then
						sMsg &= "correct and complete"
					Else
						sMsg &= "correct and incomplete"
					End If
					oTopoModel.Close()
				Catch oEx As Exception
					sMsg &= "incorrect"
				End Try
			Else
				sMsg &= "incorrect"
			End If
			MessageBox.Show(sMsg, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzSetCheckState()
		If Not mbCodeExecuting Then
			Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
			Dim saTopoLayers() As String = oTopoDef.GetAllLayers()
			mbCodeExecuting = True

			Me.chkTopoLayersOn.CheckState = DMAcadExt.AcadTransaction.SetLayersOn(1, saTopoLayers, chkTopoLayersOn.Checked, True)
			DMAcadExt.AcadDocument.UpdateScreen()
			mbCodeExecuting = False
		End If
	End Sub
	Private Sub zzEraseTopoGeometria()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim sTopoName As String = oTopoDef.Name
		Dim bTopologyExists As Boolean = TopoCreator.TopologyExists(oTopoDef)
		Dim sColLinksMsg As String = String.Empty

		If oTopoDef IsNot Nothing Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			If bTopologyExists Then
				If oTopoDef.CentroidBlockExists AndAlso oTopoDef.LayersOption = DMAcadExt.enTopoLayersOption.Default Then
					Dim iLinkCount As Integer = TopoManager.TopoCreator.DeleteTopoLinks(sTopoName)
					'	colLinks = TopoManager.TopoCreator.GetTopoLinks(sTopoName)
					'	TopoCreator.DeleteTopology(sTopoName, False)
					'	DMAcadExt.AcadTransaction.EraseDBObjects(colLinks)
					sColLinksMsg = ":" & Convert.ToString(iLinkCount) & " links"
				Else
					TopoCreator.DeleteTopology(sTopoName, True)
				End If
				DMAcadExt.AcadDocument.WriteMessage("Erased topology '" & sTopoName & "'" & sColLinksMsg)
				Me.moaChecks(moCurrentLabel.Index).Checked = TopoCreator.TopologyExists(oTopoDef)
			End If
			If oTopoDef.LinkLayersExists Then
				DMAcadExt.AcadTransaction.ClearLayerByClassName(oTopoDef.LinkAndDuplicateLayers, String.Empty)
			End If
			'	DMAcadExt.AcadTransaction.EraseLinks(oTopoDef.LinkLayer, False)
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'	AcadTransaction.Terminate()
		End If
	End Sub
	Private Sub zzPrepare()
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If oTopoDef.ID.TopoIsMerge Then
			DMAcadExt.dmLineCleanup.BuildOverlay(oTopoDef)
		ElseIf oTopoDef.ID.TopoIsAdditional Then
			Dim dStraightTolerance As Double
			Try
				dStraightTolerance = Convert.ToDouble(Me.txtStraightTolerance.Text)
			Catch oEx As Exception
				dStraightTolerance = 0.1
			End Try
			DMAcadExt.dmLineCleanup.Straighten(oTopoDef, dStraightTolerance)
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Protected Sub FillFormatRow(ByRef oFormatCombo As ComboBox, ByVal iSectionID As Integer)
		Dim iItemIndex As Integer = 0
		Dim sItemText As String
		Do
			sItemText = zzGetText(iItemIndex, iSectionID, True)
			If sItemText.Length = 0 Then Exit Do
			oFormatCombo.Items.Add(New DMCommon.ItemData(iItemIndex, sItemText))
			iItemIndex += 1
		Loop
	End Sub
	Private Sub zzCleanup(ByVal bFix As Boolean)
		Dim oDataRowView As DataRowView
		Dim iActionID As Integer
		Dim iAcadActionUB As Integer = -1
		Dim tCleanupOptions As DMAcadExt.dmCleanupOptions = Nothing
		Dim dTolerance As Double
		Dim bDmCleanupFirst As Boolean
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = Nothing
		Dim oDataGridViewRow As DataGridViewRow
		Dim dicSelectedIndices As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)

		If Me.dgvActions.SelectedRows.Count > 0 Then
			For iSelectedIndex As Integer = 0 To Me.dgvActions.SelectedRows.Count - 1
				oDataGridViewRow = Me.dgvActions.SelectedRows.Item(iSelectedIndex)
				dicSelectedIndices.Add(oDataGridViewRow.Index, 0)
			Next
		End If
		For iIndex As Integer = 0 To moCurrentView.Count - 1
			If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
				oDataRowView = moCurrentView.Item(iIndex)
				iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
				dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))

				If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
					If iIndex = 0 Then
						bDmCleanupFirst = False
					End If
					iAcadActionUB += 1
				Else
					If iIndex = 0 Then
						bDmCleanupFirst = True
					End If

					tCleanupOptions.AddAction(iActionID, dTolerance, iIndex)

				End If
			End If
		Next
		'	MessageBox.Show(CStr(iAcadActionUB) & ":" & CStr(moCurrentView.Count) & ":" & CStr(dicSelectedIndices.Count), "21_701")
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Dim oDataRow As DataRow
		Dim bCurrentLayerOK As Boolean
		Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()


		If iAcadActionUB >= 0 OrElse tCleanupOptions.HasAction Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			If tCleanupOptions.HasAction Then
				tCleanupOptions.TopoDef = oTopoDef
			End If
			DMAcadExt.AcadDocument.WriteMessage("Layers:" & tCleanupOptions.SourceLayers & ";" & tCleanupOptions.DestLayers & "!")
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
			DMAcadExt.AcadDocument.WriteMessage("&&&Layer:" & bCurrentLayerOK.ToString())
			If bCurrentLayerOK Then
				Me.Cursor = Cursors.WaitCursor
				Dim oDataTable As DataTable = moDataTable(moCurrentLabel.Index)
				If bDmCleanupFirst Then
					tCleanupResult = zzDMCleanup(bFix, tCleanupOptions)
				End If
				If iAcadActionUB >= 0 Then
					Dim oaActionVar(iAcadActionUB) As ActionVar
					Dim iaAcadCleanupRowIndex(iAcadActionUB) As Integer
					ReDim moaErrorPoints(iAcadActionUB)
					Dim iaErrors() As Integer
					Try
						Dim iVarIndex As Integer = 0
						For iIndex As Integer = 0 To moCurrentView.Count - 1
							If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
								oDataRowView = moCurrentView.Item(iIndex)
								iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
								If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
									dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
									oaActionVar(iVarIndex) = New ActionVar(iActionID, dTolerance)
									iaAcadCleanupRowIndex(iVarIndex) = iIndex
									iVarIndex += 1
								End If
							End If
						Next
						If oTopoDef IsNot Nothing Then
							Try
								iaErrors = TopoManager.TopoCreator.CleanupByTopoDef(oaActionVar, oTopoDef, bFix, moaErrorPoints)
							Catch oEx As Exception
								MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "26_997")
								Common.GetMapTopoEx(oEx, "C919aMM_")
								DMAcadExt.AcadTransaction.Terminate()
								DMAcadExt.AcadDocument.CloseMessage()
								DMAcadExt.AcadDocument.Unlock()
								Exit Sub
							End Try
							Dim iResIndex As Integer = 0
							For iIndex As Integer = 0 To moCurrentView.Count - 1
								'oDataRow = oDataTable.Rows(iaAcadCleanupRowIndex(iIndex))
								If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
									oDataRow = moCurrentView.Item(iaAcadCleanupRowIndex(iResIndex)).Row
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
						tCleanupResult = zzDMCleanup(bFix, tCleanupOptions)
					End If
				End If  'iAcadActionUB >= 0

				Do While tCleanupResult.NextAction
					'	oDataRow = oDataTable.Rows(tCleanupResult.RowIndex)
					oDataRow = moCurrentView.Item(tCleanupResult.RowIndex).Row
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
				miCurrentAction = Me.dgvActions.CurrentRow.Index
				zzSetCleanupErrPoints()
			Catch oEx As Exception
				miCurrentAction = -1
			End Try
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Function zzDMCleanup(ByVal bFix As Boolean, ByVal tCleanupOptions As DMAcadExt.dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim tCleanupResult As DMAcadExt.dmCleanupResult
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanup(bFix, tCleanupOptions)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		Return tCleanupResult
	End Function
	Private Function zzSetGridColumns() As System.Windows.Forms.DataGridViewComboBoxColumn
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
			Me.dgvActions.Columns.Add(oCmbColumn)
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
			Me.dgvActions.Columns.Add(oTxtColumn)
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
			Me.dgvActions.Columns.Add(oTxtColumn)
		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, Me.Name)
		End Try
		Return oCmbColumn
	End Function
	Private Sub zzLoadData(ByVal iTopoTypeIndex As Integer)
		Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps.Value)
		Dim sStepNo As String = Convert.ToString(Me.nudSteps.Value)
		If moDataTable(iTopoTypeIndex) Is Nothing Then
			moDataTable(iTopoTypeIndex) = New DataTable("TopoType" & Convert.ToString(miCurrentTopoDefID.ID))
			Dim sSelectComText As String = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM(CleanupActions) WHERE (CleanupActions.TopologyType=" & Convert.ToString(miCurrentTopoDefID.ID) & ") AND (CleanupActions.Step=" & sStepNo & ") ORDER BY CleanupActions.ActionNo"
			sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM(CleanupActions) WHERE (CleanupActions.TopologyType=" & Convert.ToString(miCurrentTopoDefID.ID) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

			Dim sUpdateComTextAAA As String = "UPDATE Localities SET Actual = ? WHERE ID = ?"
			Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()

			moOleDbDataAdapter(iTopoTypeIndex) = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSelectComText, True)
			If moOleDbDataAdapter(iTopoTypeIndex) IsNot Nothing Then
				moOleDbDataAdapter(iTopoTypeIndex).Fill(moDataTable(iTopoTypeIndex))
				moDataTable(iTopoTypeIndex).Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
				moDataTable(iTopoTypeIndex).Columns.Add(msPointsFldName, oPointArray.GetType)
				Dim iRowCount As Integer = moDataTable(iTopoTypeIndex).Rows.Count
				If iRowCount > 0& Then
					Dim oRow As DataRow = moDataTable(iTopoTypeIndex).Rows.Item(iRowCount - 1)
					miStepNum(iTopoTypeIndex) = DirectCast(oRow.Item("Step"), Integer)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("DataAdapter was not found", "27_514")
			End If
		End If
		Dim oColumn As DataColumn = moDataTable(iTopoTypeIndex).Columns("Step")
		oColumn.DefaultValue = iStepNo

		moCurrentView = New DataView(moDataTable(iTopoTypeIndex), "Step=" & sStepNo & "", "", DataViewRowState.CurrentRows)
		Me.dgvActions.DataSource = moCurrentView
	End Sub
	Private Function zzLoadTopoDef() As DMAcadExt.TopoDef
		Dim iTopoIndex As Integer
		Dim bChecked As Boolean = False
		iTopoIndex = moCurrentLabel.Index

		If TopoDefs.moaTopoDefs(iTopoIndex) Is Nothing Then
			System.Windows.Forms.MessageBox.Show("", "33_155")
			TopoDefs.moaTopoDefs(iTopoIndex) = New DMAcadExt.TopoDef(DMAcadExt.DMApp.AppID, TopoDefs.miaTopoIDs(iTopoIndex))
			System.Windows.Forms.MessageBox.Show(TopoDefs.miaTopoIDs(iTopoIndex).BaseID.ToString(), "33_200")
		End If
		moaChecks(iTopoIndex).Checked = False
		bChecked = TopoManager.TopoCreator.TopologyExists(TopoManager.TopoDefs.moaTopoDefs(iTopoIndex))
		moaChecks(iTopoIndex).Checked = bChecked
		Return TopoDefs.moaTopoDefs(iTopoIndex)
	End Function
	Protected Shared Function zzGetBaseText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
		Try
			Return TPlServerDB.TextResource.GetText(iItemID, miBaseResourceTheme, iSectionID, bReturnEmpty)
		Catch oEx As Exception
			Return String.Empty
		End Try

	End Function
	Protected Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
		Try
			Return TPlServerDB.TextResource.GetText(iItemID, diResourceTheme, iSectionID, bReturnEmpty)
		Catch oEx As Exception
			Return String.Empty
		End Try

	End Function
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
	Protected Class TabButton
		Inherits Button
		Public Sub New(ByVal oFont As Font, ByVal bImage As Boolean)
			Dim iWidth As Integer
			If bImage Then
				iWidth = 24
			Else
				iWidth = 48
			End If
			MyBase.Size = New System.Drawing.Size(iWidth, 24)
			MyBase.Font = oFont
			MyBase.Cursor = Cursors.Hand
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Standard
			MyBase.UseVisualStyleBackColor = True
		End Sub
	End Class

	Protected Class LabelInd
		Inherits System.Windows.Forms.Label
		Private miIndex As Integer
		Public Sub New(ByVal iIndex As Integer, ByVal iLength As Integer, ByVal iRight As Integer, ByVal iY As Integer)
			miIndex = iIndex
			MyBase.AutoSize = False
			MyBase.Cursor = System.Windows.Forms.Cursors.Hand
			MyBase.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			MyBase.Name = "lblTopo" & Convert.ToString(iIndex)
			If True Then
				MyBase.Size = New System.Drawing.Size(188, 18)
			Else
				MyBase.Size = New System.Drawing.Size(76, 18)
			End If
			MyBase.Size = New System.Drawing.Size(iLength, 18)
			MyBase.Location = New System.Drawing.Point(iRight - iLength, iY)
			MyBase.TabIndex = iIndex

			MyBase.Text = zzGetText(iIndex, 1)
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			MyBase.TextAlign = ContentAlignment.MiddleLeft
			MyBase.BorderStyle = System.Windows.Forms.BorderStyle.None
		End Sub
		Public Property Index() As Integer
			Get
				Return miIndex
			End Get
			Set(ByVal iValue As Integer)
				miIndex = iValue
			End Set
		End Property
		Public Sub WidthToLeft(ByVal iValue As Integer)
			MyBase.Left -= iValue
			MyBase.Width += iValue
		End Sub



	End Class
	Protected Class TopoCheck
		Inherits CheckBox
		Public Sub New(ByVal cmsControl As System.Windows.Forms.ContextMenuStrip)
			MyBase.Size = New System.Drawing.Size(16, 24)
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Standard
			MyBase.Enabled = False
			MyBase.ThreeState = False
			MyBase.CheckState = System.Windows.Forms.CheckState.Indeterminate
			MyBase.ContextMenuStrip = cmsControl
		End Sub
	End Class

	Protected Class CheckButton
		Inherits CheckBox
		Private miIndex As Integer
		Public Sub New(ByVal iIndex As Integer)
			miIndex = iIndex
			MyBase.AutoSize = False
			MyBase.Cursor = System.Windows.Forms.Cursors.Hand
			MyBase.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			MyBase.Appearance = System.Windows.Forms.Appearance.Button
			MyBase.ThreeState = False
			MyBase.Name = "chkTopoLayer" & Convert.ToString(iIndex)
			MyBase.Size = New System.Drawing.Size(15, 18)
			MyBase.TabIndex = iIndex
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			MyBase.ImageAlign = ContentAlignment.MiddleCenter
			SetImage()
		End Sub
		Public Sub SetImage()
			If MyBase.CheckState = System.Windows.Forms.CheckState.Checked Then
				MyBase.Image = My.Resources.LayerOn
			ElseIf MyBase.CheckState = System.Windows.Forms.CheckState.Unchecked Then
				MyBase.Image = My.Resources.LayerOff
			Else
				MyBase.Image = My.Resources.QuestionMark
			End If

		End Sub
		Public Property Index() As Integer
			Get
				Return miIndex
			End Get
			Set(ByVal iValue As Integer)
				miIndex = iValue
			End Set
		End Property
	End Class

	Private Sub frmTopoActionsBase_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
		mbInactive = False
		Me.Opacity = 1.0
		''''''''''''''''''	Me.tmrInactive.Stop()
	End Sub

	Private Sub frmTopoActionsBase_Deactivate(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Deactivate
		mbInactive = True
		mdInactiveTiks = 0.0
		'		Me.tmrInactive.Start()
	End Sub

	Private Sub frmTopoActionsBase_FormClosing(ByVal oSender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		Dim iCloseReason As CloseReason = e.CloseReason
		DMAcadExt.AcadDocument.RestoreVarCmdDia()
		'	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_202")

		If Me.DialogResult = Windows.Forms.DialogResult.None AndAlso iCloseReason = CloseReason.UserClosing Then
			Me.Hide()
			e.Cancel = True

		End If
		Common.SetAcadFocus()
		'	System.Windows.Forms.MessageBox.Show(e.CloseReason.ToString() & ":" & Me.DialogResult.ToString(), "12_888 Base frmTopoActions_FormClosing")
	End Sub
	Protected Class ReportItem
		Inherits DMCommon.ItemData
		Private miOptions As TopoManager.TPlanGraph.enDataOptions
		Private miTopoPurpose As DMAcadExt.enTopoPurpose

		Private mbExcel As Boolean
		Private mbAcad As Boolean
		Private mbAcadTable As Boolean
		Public Sub New(ByVal iListIndex As TPlServerDB.enResourceTheme, ByVal sListDispData As String, ByVal iOptions As TopoManager.TPlanGraph.enDataOptions, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bExcel As Boolean, ByVal bAcad As Boolean, ByVal bAcadTable As Boolean)
			MyBase.New(iListIndex, sListDispData)
			miOptions = iOptions
			miTopoPurpose = iTopoPurpose

			mbExcel = bExcel
			mbAcad = bAcad
			mbAcadTable = bAcadTable
		End Sub
		Public Property RepIndex() As TPlServerDB.enResourceTheme
			Get
				Try
					Return CType(MyBase.ListIndex, TPlServerDB.enResourceTheme)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - RepIndex")
				End Try
			End Get
			Set(ByVal iValue As TPlServerDB.enResourceTheme)
				MyBase.ListIndex = iValue
			End Set
		End Property
		Public Property Options() As TopoManager.TPlanGraph.enDataOptions
			Get
				Return miOptions
			End Get
			Set(ByVal iValue As TopoManager.TPlanGraph.enDataOptions)
				miOptions = iValue
			End Set
		End Property
		Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
			Get
				Return miTopoPurpose
			End Get
			Set(ByVal iValue As DMAcadExt.enTopoPurpose)
				miTopoPurpose = iValue
			End Set
		End Property

		Public Property Excel() As Boolean
			Get
				Return mbExcel
			End Get
			Set(ByVal bValue As Boolean)
				mbExcel = bValue
			End Set
		End Property
		Public Property Acad() As Boolean
			Get
				Return mbAcad
			End Get
			Set(ByVal bValue As Boolean)
				mbAcad = bValue
			End Set
		End Property
		Public Property AcadTable() As Boolean
			Get
				Return mbAcadTable
			End Get
			Set(ByVal bValue As Boolean)
				mbAcadTable = bValue
			End Set
		End Property
		Public ReadOnly Property AcadBlock() As Boolean
			Get
				Return mbAcad AndAlso (Not mbAcadTable)
			End Get
		End Property
	End Class

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


	Private Sub dfEditColorScheme_FormClosed(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles dfEditColorScheme.FormClosed
		Try
			Me.dfEditColorScheme.Dispose()
			Me.dfEditColorScheme = Nothing
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - dfEditColorScheme_FormClosed")
		End Try
	End Sub

	Private Sub tstTopo_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopo.ItemClicked
		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbSwitchCleanup.Name
				Me.dgvActions.Visible = Not Me.dgvActions.Visible
				Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
				zzSetButtonActionEnable(oTopoDef)
			Case Me.tsbCreateTopo.Name
				zzCreateTopo()
			Case Me.tsbCheckTopo.Name
				zzCheckTopo()
			Case Me.tsbDeleteTopo.Name
				Me.zzDeleteTopo(False)
			Case Me.tsbShowTopo.Name
				Me.zzShowTopo()
			Case Me.tsbEraseTopoGeometria.Name
				Me.zzEraseTopoGeometria()
			Case Me.tsbCopyFromOverlay.Name
				Me.zzCopyFromOverlay()
			Case Me.tsbToClosedPolygons.Name
				zzToClosedPgons()
			Case Me.tsbToClosedPolygons.Name
				zzExportToShape()
		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub tsiCloseForm_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsiCloseForm.Click
		Me.Hide()
	End Sub

	Private Sub tsiCreateTopo_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsiCreateTopo.Click
		zzCreateTopo()
	End Sub



	Private Sub tsbExportToShape_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsbExportToShape.Click
		zzExportToShape()
	End Sub
End Class