<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPrjThemes
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPrjThemes))
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.tstMainTop = New System.Windows.Forms.ToolStrip()
		Me.tstProjectCode = New System.Windows.Forms.ToolStripTextBox()
		Me.tslDetailNo = New System.Windows.Forms.ToolStripLabel()
		Me.ddbDetails = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tsbEditProject = New System.Windows.Forms.ToolStripButton()
		Me.tsbUpdate = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsbCleanup = New System.Windows.Forms.ToolStripButton()
		Me.tsbCheckThemes = New System.Windows.Forms.ToolStripButton()
		Me.tsbCalculate = New System.Windows.Forms.ToolStripButton()
		Me.tsbReports = New System.Windows.Forms.ToolStripButton()
		Me.tsbPaint = New System.Windows.Forms.ToolStripButton()
		Me.tsbToClosedPgons = New System.Windows.Forms.ToolStripButton()
		Me.tsbCheckPgons = New System.Windows.Forms.ToolStripButton()
		Me.ssbApplication = New System.Windows.Forms.ToolStripSplitButton()
		Me.tsbMessages = New System.Windows.Forms.ToolStripButton()
		Me.tsbDispTable = New System.Windows.Forms.ToolStripButton()
		Me.ddbUtilites = New System.Windows.Forms.ToolStripDropDownButton()
		Me.ddbUtilities = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tmiCreateAllTopologies = New System.Windows.Forms.ToolStripMenuItem()
		Me.tmiEraseAllTopologies = New System.Windows.Forms.ToolStripMenuItem()
		Me.tmiPlanName = New System.Windows.Forms.ToolStripMenuItem()
		Me.tmiRegion = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsbProperties = New System.Windows.Forms.ToolStripButton()
		Me.tsbOpenDWG = New System.Windows.Forms.ToolStripButton()
		Me.tsbSaveDWG = New System.Windows.Forms.ToolStripButton()
		Me.tsbExit = New System.Windows.Forms.ToolStripButton()
		Me.dgvPrjThemes = New System.Windows.Forms.DataGridView()
		Me.ctxMapTheme = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxGraphType = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchDone = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.txtNodeLayer = New System.Windows.Forms.TextBox()
		Me.txtNodeBlockName = New System.Windows.Forms.TextBox()
		Me.txtClosedPgonsLayer = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtLineLinkLayer = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.txtLineTopoName = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.txtTopoName = New System.Windows.Forms.TextBox()
		Me.txtCentroidLayer = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtCentroidBlockName = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtLinkLayers = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.bnsPrjThemes = New System.Windows.Forms.BindingSource(Me.components)
		Me.tstMainTop.SuspendLayout()
		CType(Me.dgvPrjThemes, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.Panel1.SuspendLayout()
		CType(Me.bnsPrjThemes, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'tstMainTop
		'
		Me.tstMainTop.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.tstMainTop.ImageScalingSize = New System.Drawing.Size(20, 20)
		Me.tstMainTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tstProjectCode, Me.tslDetailNo, Me.ddbDetails, Me.tsbEditProject, Me.tsbUpdate, Me.ToolStripSeparator1, Me.ToolStripLabel1, Me.ToolStripSeparator2, Me.tsbCleanup, Me.tsbCheckThemes, Me.tsbCalculate, Me.tsbReports, Me.tsbPaint, Me.tsbToClosedPgons, Me.tsbCheckPgons, Me.ssbApplication, Me.tsbMessages, Me.tsbDispTable, Me.ddbUtilites, Me.ddbUtilities, Me.tsbProperties, Me.tsbOpenDWG, Me.tsbSaveDWG, Me.tsbExit})
		Me.tstMainTop.Location = New System.Drawing.Point(0, 0)
		Me.tstMainTop.Name = "tstMainTop"
		Me.tstMainTop.Size = New System.Drawing.Size(566, 32)
		Me.tstMainTop.TabIndex = 0
		'
		'tstProjectCode
		'
		Me.tstProjectCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
		Me.tstProjectCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
		Me.tstProjectCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.tstProjectCode.Name = "tstProjectCode"
		Me.tstProjectCode.Size = New System.Drawing.Size(48, 32)
		'
		'tslDetailNo
		'
		Me.tslDetailNo.Name = "tslDetailNo"
		Me.tslDetailNo.Size = New System.Drawing.Size(0, 29)
		'
		'ddbDetails
		'
		Me.ddbDetails.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.ddbDetails.DoubleClickEnabled = True
		Me.ddbDetails.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.ddbDetails.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbDetails.Name = "ddbDetails"
		Me.ddbDetails.Size = New System.Drawing.Size(13, 29)
		'
		'tsbEditProject
		'
		Me.tsbEditProject.CheckOnClick = True
		Me.tsbEditProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbEditProject.Image = Global.TopoUI.My.Resources.Resources.EditInformationHS
		Me.tsbEditProject.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbEditProject.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbEditProject.Name = "tsbEditProject"
		Me.tsbEditProject.Size = New System.Drawing.Size(23, 29)
		Me.tsbEditProject.ToolTipText = "עריכת פרויקט"
		'
		'tsbUpdate
		'
		Me.tsbUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbUpdate.Enabled = False
		Me.tsbUpdate.Image = CType(resources.GetObject("tsbUpdate.Image"), System.Drawing.Image)
		Me.tsbUpdate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbUpdate.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbUpdate.Name = "tsbUpdate"
		Me.tsbUpdate.Size = New System.Drawing.Size(24, 29)
		Me.tsbUpdate.ToolTipText = "שמירת פרויקט"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 32)
		'
		'ToolStripLabel1
		'
		Me.ToolStripLabel1.Name = "ToolStripLabel1"
		Me.ToolStripLabel1.Size = New System.Drawing.Size(11, 29)
		Me.ToolStripLabel1.Text = " "
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 32)
		'
		'tsbCleanup
		'
		Me.tsbCleanup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbCleanup.Image = Global.TopoUI.My.Resources.Resources.Table19x17
		Me.tsbCleanup.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbCleanup.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbCleanup.Name = "tsbCleanup"
		Me.tsbCleanup.Size = New System.Drawing.Size(23, 29)
		Me.tsbCleanup.ToolTipText = "עריכת נושא"
		'
		'tsbCheckThemes
		'
		Me.tsbCheckThemes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbCheckThemes.Image = Global.TopoUI.My.Resources.Resources.Check16Tr
		Me.tsbCheckThemes.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbCheckThemes.Name = "tsbCheckThemes"
		Me.tsbCheckThemes.Size = New System.Drawing.Size(24, 29)
		Me.tsbCheckThemes.ToolTipText = "בדיקות"
		'
		'tsbCalculate
		'
		Me.tsbCalculate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbCalculate.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
		Me.tsbCalculate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbCalculate.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbCalculate.Name = "tsbCalculate"
		Me.tsbCalculate.Size = New System.Drawing.Size(23, 29)
		Me.tsbCalculate.ToolTipText = "חישוב"
		'
		'tsbReports
		'
		Me.tsbReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbReports.Image = Global.TopoUI.My.Resources.Resources.book_notebook20
		Me.tsbReports.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbReports.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbReports.Name = "tsbReports"
		Me.tsbReports.Size = New System.Drawing.Size(23, 29)
		Me.tsbReports.ToolTipText = "דוחות"
		'
		'tsbPaint
		'
		Me.tsbPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbPaint.Image = Global.TopoUI.My.Resources.Resources.Paint20Tr
		Me.tsbPaint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbPaint.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbPaint.Name = "tsbPaint"
		Me.tsbPaint.Size = New System.Drawing.Size(24, 29)
		Me.tsbPaint.Tag = ""
		Me.tsbPaint.ToolTipText = "צביעת מפה"
		'
		'tsbToClosedPgons
		'
		Me.tsbToClosedPgons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbToClosedPgons.Image = Global.TopoUI.My.Resources.Resources.ToClosedPgons
		Me.tsbToClosedPgons.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbToClosedPgons.Name = "tsbToClosedPgons"
		Me.tsbToClosedPgons.Size = New System.Drawing.Size(23, 29)
		Me.tsbToClosedPgons.ToolTipText = "סגירת פוליגונים"
		'
		'tsbCheckPgons
		'
		Me.tsbCheckPgons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbCheckPgons.Image = Global.TopoUI.My.Resources.Resources.Rect16
		Me.tsbCheckPgons.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbCheckPgons.Name = "tsbCheckPgons"
		Me.tsbCheckPgons.Size = New System.Drawing.Size(24, 29)
		'
		'ssbApplication
		'
		Me.ssbApplication.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ssbApplication.Image = Global.TopoUI.My.Resources.Resources.Unidiv
		Me.ssbApplication.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.ssbApplication.Name = "ssbApplication"
		Me.ssbApplication.Size = New System.Drawing.Size(32, 29)
		Me.ssbApplication.ToolTipText = "חני""ת"
		'
		'tsbMessages
		'
		Me.tsbMessages.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbMessages.Image = CType(resources.GetObject("tsbMessages.Image"), System.Drawing.Image)
		Me.tsbMessages.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbMessages.Name = "tsbMessages"
		Me.tsbMessages.Size = New System.Drawing.Size(24, 29)
		Me.tsbMessages.ToolTipText = "הודעות"
		'
		'tsbDispTable
		'
		Me.tsbDispTable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbDispTable.Image = Global.TopoUI.My.Resources.Resources.Form15
		Me.tsbDispTable.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbDispTable.Name = "tsbDispTable"
		Me.tsbDispTable.Size = New System.Drawing.Size(24, 29)
		Me.tsbDispTable.ToolTipText = "טבלאות נתונים"
		'
		'ddbUtilites
		'
		Me.ddbUtilites.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
		Me.ddbUtilites.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ddbUtilites.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbUtilites.Name = "ddbUtilites"
		Me.ddbUtilites.Size = New System.Drawing.Size(13, 29)
		Me.ddbUtilites.Text = "ddbUtilites"
		'
		'ddbUtilities
		'
		Me.ddbUtilities.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ddbUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tmiCreateAllTopologies, Me.tmiEraseAllTopologies, Me.tmiPlanName, Me.tmiRegion})
		Me.ddbUtilities.Image = Global.TopoUI.My.Resources.Resources.Util16Tr
		Me.ddbUtilities.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbUtilities.Name = "ddbUtilities"
		Me.ddbUtilities.Size = New System.Drawing.Size(33, 29)
		'
		'tmiCreateAllTopologies
		'
		Me.tmiCreateAllTopologies.Name = "tmiCreateAllTopologies"
		Me.tmiCreateAllTopologies.Size = New System.Drawing.Size(195, 22)
		Me.tmiCreateAllTopologies.Text = "יצירת כל הטופולוגיות"
		'
		'tmiEraseAllTopologies
		'
		Me.tmiEraseAllTopologies.Name = "tmiEraseAllTopologies"
		Me.tmiEraseAllTopologies.Size = New System.Drawing.Size(195, 22)
		Me.tmiEraseAllTopologies.Text = "מחיקת כל הטופולוגיות"
		'
		'tmiPlanName
		'
		Me.tmiPlanName.Name = "tmiPlanName"
		Me.tmiPlanName.Size = New System.Drawing.Size(195, 22)
		Me.tmiPlanName.Text = "שם תוכנית-->מגרשים"
		'
		'tmiRegion
		'
		Me.tmiRegion.Name = "tmiRegion"
		Me.tmiRegion.Size = New System.Drawing.Size(195, 22)
		Me.tmiRegion.Text = "מתחם-->מגרשים"
		'
		'tsbProperties
		'
		Me.tsbProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbProperties.Image = Global.TopoUI.My.Resources.Resources.Properties1
		Me.tsbProperties.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbProperties.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbProperties.Name = "tsbProperties"
		Me.tsbProperties.Size = New System.Drawing.Size(36, 29)
		Me.tsbProperties.Text = "tsbProperty"
		'
		'tsbOpenDWG
		'
		Me.tsbOpenDWG.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbOpenDWG.Image = Global.TopoUI.My.Resources.Resources.DWG16Tr
		Me.tsbOpenDWG.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbOpenDWG.Name = "tsbOpenDWG"
		Me.tsbOpenDWG.Size = New System.Drawing.Size(23, 29)
		'
		'tsbSaveDWG
		'
		Me.tsbSaveDWG.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbSaveDWG.Image = Global.TopoUI.My.Resources.Resources.SaveDWG32Tr
		Me.tsbSaveDWG.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbSaveDWG.Name = "tsbSaveDWG"
		Me.tsbSaveDWG.Size = New System.Drawing.Size(24, 29)
		'
		'tsbExit
		'
		Me.tsbExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbExit.Image = Global.TopoUI.My.Resources.Resources._Exit
		Me.tsbExit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbExit.Name = "tsbExit"
		Me.tsbExit.Size = New System.Drawing.Size(23, 29)
		Me.tsbExit.ToolTipText = "יציאה"
		'
		'dgvPrjThemes
		'
		Me.dgvPrjThemes.AllowUserToAddRows = False
		Me.dgvPrjThemes.AllowUserToDeleteRows = False
		Me.dgvPrjThemes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvPrjThemes.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxMapTheme, Me.ctxGraphType, Me.cchDone})
		Me.dgvPrjThemes.Dock = System.Windows.Forms.DockStyle.Top
		Me.dgvPrjThemes.Location = New System.Drawing.Point(0, 32)
		Me.dgvPrjThemes.Name = "dgvPrjThemes"
		Me.dgvPrjThemes.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.dgvPrjThemes.Size = New System.Drawing.Size(566, 348)
		Me.dgvPrjThemes.TabIndex = 1
		'
		'ctxMapTheme
		'
		Me.ctxMapTheme.HeaderText = "נושא"
		Me.ctxMapTheme.Name = "ctxMapTheme"
		'
		'ctxGraphType
		'
		Me.ctxGraphType.HeaderText = "סוג גרפי"
		Me.ctxGraphType.Name = "ctxGraphType"
		'
		'cchDone
		'
		DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
		DataGridViewCellStyle1.BackColor = System.Drawing.Color.Red
		DataGridViewCellStyle1.NullValue = System.Windows.Forms.CheckState.Indeterminate
		Me.cchDone.DefaultCellStyle = DataGridViewCellStyle1
		Me.cchDone.HeaderText = "בוצע"
		Me.cchDone.Name = "cchDone"
		Me.cchDone.ThreeState = True
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.Label8)
		Me.Panel1.Controls.Add(Me.Label9)
		Me.Panel1.Controls.Add(Me.txtNodeLayer)
		Me.Panel1.Controls.Add(Me.txtNodeBlockName)
		Me.Panel1.Controls.Add(Me.txtClosedPgonsLayer)
		Me.Panel1.Controls.Add(Me.Label7)
		Me.Panel1.Controls.Add(Me.txtLineLinkLayer)
		Me.Panel1.Controls.Add(Me.Label6)
		Me.Panel1.Controls.Add(Me.txtLineTopoName)
		Me.Panel1.Controls.Add(Me.Label5)
		Me.Panel1.Controls.Add(Me.Label4)
		Me.Panel1.Controls.Add(Me.txtTopoName)
		Me.Panel1.Controls.Add(Me.txtCentroidLayer)
		Me.Panel1.Controls.Add(Me.Label3)
		Me.Panel1.Controls.Add(Me.txtCentroidBlockName)
		Me.Panel1.Controls.Add(Me.Label2)
		Me.Panel1.Controls.Add(Me.txtLinkLayers)
		Me.Panel1.Controls.Add(Me.Label1)
		Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.Panel1.Location = New System.Drawing.Point(0, 386)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(566, 135)
		Me.Panel1.TabIndex = 2
		'
		'Label8
		'
		Me.Label8.Location = New System.Drawing.Point(142, 82)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(116, 22)
		Me.Label8.TabIndex = 17
		Me.Label8.Text = "שכבת בלוק בצומת"
		Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label9
		'
		Me.Label9.Location = New System.Drawing.Point(142, 56)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(116, 22)
		Me.Label9.TabIndex = 16
		Me.Label9.Text = "שם בלוק בצומת"
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtNodeLayer
		'
		Me.txtNodeLayer.Location = New System.Drawing.Point(8, 82)
		Me.txtNodeLayer.Name = "txtNodeLayer"
		Me.txtNodeLayer.Size = New System.Drawing.Size(134, 22)
		Me.txtNodeLayer.TabIndex = 15
		'
		'txtNodeBlockName
		'
		Me.txtNodeBlockName.Location = New System.Drawing.Point(8, 56)
		Me.txtNodeBlockName.Name = "txtNodeBlockName"
		Me.txtNodeBlockName.Size = New System.Drawing.Size(134, 22)
		Me.txtNodeBlockName.TabIndex = 14
		'
		'txtClosedPgonsLayer
		'
		Me.txtClosedPgonsLayer.Location = New System.Drawing.Point(280, 108)
		Me.txtClosedPgonsLayer.Name = "txtClosedPgonsLayer"
		Me.txtClosedPgonsLayer.Size = New System.Drawing.Size(134, 22)
		Me.txtClosedPgonsLayer.TabIndex = 13
		'
		'Label7
		'
		Me.Label7.Location = New System.Drawing.Point(414, 108)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(134, 22)
		Me.Label7.TabIndex = 12
		Me.Label7.Text = "שכבת פוליגונים סגורים"
		Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtLineLinkLayer
		'
		Me.txtLineLinkLayer.Location = New System.Drawing.Point(280, 82)
		Me.txtLineLinkLayer.Name = "txtLineLinkLayer"
		Me.txtLineLinkLayer.Size = New System.Drawing.Size(134, 22)
		Me.txtLineLinkLayer.TabIndex = 11
		'
		'Label6
		'
		Me.Label6.Location = New System.Drawing.Point(462, 82)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(86, 22)
		Me.Label6.TabIndex = 10
		Me.Label6.Text = "שכבות יישור"
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtLineTopoName
		'
		Me.txtLineTopoName.Location = New System.Drawing.Point(280, 56)
		Me.txtLineTopoName.Name = "txtLineTopoName"
		Me.txtLineTopoName.Size = New System.Drawing.Size(134, 22)
		Me.txtLineTopoName.TabIndex = 9
		'
		'Label5
		'
		Me.Label5.AutoEllipsis = True
		Me.Label5.Location = New System.Drawing.Point(414, 56)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(134, 22)
		Me.Label5.TabIndex = 8
		Me.Label5.Text = "טופולוגיה ללא קשתות"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label4
		'
		Me.Label4.Location = New System.Drawing.Point(414, 4)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(134, 22)
		Me.Label4.TabIndex = 7
		Me.Label4.Text = "טופולוגיה"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtTopoName
		'
		Me.txtTopoName.Location = New System.Drawing.Point(280, 4)
		Me.txtTopoName.Name = "txtTopoName"
		Me.txtTopoName.Size = New System.Drawing.Size(134, 22)
		Me.txtTopoName.TabIndex = 6
		'
		'txtCentroidLayer
		'
		Me.txtCentroidLayer.Location = New System.Drawing.Point(8, 30)
		Me.txtCentroidLayer.Name = "txtCentroidLayer"
		Me.txtCentroidLayer.Size = New System.Drawing.Size(134, 22)
		Me.txtCentroidLayer.TabIndex = 5
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(142, 30)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(116, 22)
		Me.Label3.TabIndex = 4
		Me.Label3.Text = "שכבת בלוק במרכז"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtCentroidBlockName
		'
		Me.txtCentroidBlockName.Location = New System.Drawing.Point(8, 4)
		Me.txtCentroidBlockName.Name = "txtCentroidBlockName"
		Me.txtCentroidBlockName.Size = New System.Drawing.Size(134, 22)
		Me.txtCentroidBlockName.TabIndex = 3
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(142, 4)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(116, 22)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "שם בלוק במרכז"
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'txtLinkLayers
		'
		Me.txtLinkLayers.Location = New System.Drawing.Point(280, 30)
		Me.txtLinkLayers.Name = "txtLinkLayers"
		Me.txtLinkLayers.Size = New System.Drawing.Size(134, 22)
		Me.txtLinkLayers.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(414, 30)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(134, 22)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "שכבות"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'bnsPrjThemes
		'
		Me.bnsPrjThemes.AllowNew = False
		'
		'frmPrjThemes
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(566, 521)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me.dgvPrjThemes)
		Me.Controls.Add(Me.tstMainTop)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmPrjThemes"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "פרויקט"
		Me.tstMainTop.ResumeLayout(False)
		Me.tstMainTop.PerformLayout()
		CType(Me.dgvPrjThemes, System.ComponentModel.ISupportInitialize).EndInit()
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		CType(Me.bnsPrjThemes, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents dgvPrjThemes As System.Windows.Forms.DataGridView
	Private WithEvents tstProjectCode As System.Windows.Forms.ToolStripTextBox
	Private WithEvents tsbCleanup As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbEditProject As System.Windows.Forms.ToolStripButton
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents txtCentroidLayer As System.Windows.Forms.TextBox
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents txtCentroidBlockName As System.Windows.Forms.TextBox
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents txtLinkLayers As System.Windows.Forms.TextBox
	Private WithEvents bnsPrjThemes As System.Windows.Forms.BindingSource
	Private WithEvents tsbUpdate As System.Windows.Forms.ToolStripButton
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents txtTopoName As System.Windows.Forms.TextBox
	Private WithEvents txtLineLinkLayer As System.Windows.Forms.TextBox
	Private WithEvents Label6 As System.Windows.Forms.Label
	Private WithEvents txtLineTopoName As System.Windows.Forms.TextBox
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents tsbCalculate As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbReports As System.Windows.Forms.ToolStripButton
	Private WithEvents tstMainTop As System.Windows.Forms.ToolStrip
	Private WithEvents tsbMessages As System.Windows.Forms.ToolStripButton
	Private WithEvents ddbDetails As System.Windows.Forms.ToolStripDropDownButton
	Private WithEvents ssbApplication As System.Windows.Forms.ToolStripSplitButton
	Private WithEvents tsbExit As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Private WithEvents tslDetailNo As System.Windows.Forms.ToolStripLabel
	Friend WithEvents ToolStripLabel1 As System.Windows.Forms.ToolStripLabel
	Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
	Private WithEvents tsbOpenDWG As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbToClosedPgons As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbSaveDWG As System.Windows.Forms.ToolStripButton
	Private WithEvents Panel1 As System.Windows.Forms.Panel
	Private WithEvents tsbProperties As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbDispTable As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbCheckPgons As System.Windows.Forms.ToolStripButton
	Private WithEvents txtClosedPgonsLayer As System.Windows.Forms.TextBox
	Private WithEvents Label7 As System.Windows.Forms.Label
	Private WithEvents ddbUtilites As System.Windows.Forms.ToolStripDropDownButton
	' Friend WithEvents שםתוכניתToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents ddbUtilities As System.Windows.Forms.ToolStripDropDownButton
	Private WithEvents tmiPlanName As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tmiRegion As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents txtNodeLayer As System.Windows.Forms.TextBox
	Private WithEvents txtNodeBlockName As System.Windows.Forms.TextBox
	Private WithEvents Label8 As System.Windows.Forms.Label
	Private WithEvents Label9 As System.Windows.Forms.Label
	Private WithEvents tsbPaint As System.Windows.Forms.ToolStripButton
	Private WithEvents smiUnidiv_EraseDBStages As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ctxMapTheme As DataGridViewTextBoxColumn
	Friend WithEvents ctxGraphType As DataGridViewTextBoxColumn
	Friend WithEvents cchDone As DataGridViewCheckBoxColumn
	Private WithEvents tsbCheckThemes As ToolStripButton
	Friend WithEvents tmiCreateAllTopologies As ToolStripMenuItem
	Friend WithEvents tmiEraseAllTopologies As ToolStripMenuItem

End Class
