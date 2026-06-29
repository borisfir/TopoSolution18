<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditColorSet
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditColorSet))
		Me.lvwLanduses = New System.Windows.Forms.ListView()
		Me.lvcLanduseCode = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.lvcLanduseName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.lvcLanduseIndex = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.lvcColorSchemeID = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.lvcColorSchemeName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.imlLarge = New System.Windows.Forms.ImageList(Me.components)
		Me.imlSmall = New System.Windows.Forms.ImageList(Me.components)
		Me.tlbTop = New System.Windows.Forms.ToolStrip()
		Me.tsbViews = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tsiTile = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiExtraLarge = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiLargeIcons = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiSmallIcons = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiList = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiDetails = New System.Windows.Forms.ToolStripMenuItem()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsiColorScale = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiColorScale_1 = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiColorScale_2 = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiColorScale_4 = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiColorScale_6 = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiColorScale_8 = New System.Windows.Forms.ToolStripMenuItem()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsiCheckBox = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiSelection = New System.Windows.Forms.ToolStripMenuItem()
		Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsbSelect = New System.Windows.Forms.ToolStripButton()
		Me.ddbAddSet = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tsiAddStandard = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiAddAll = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiPrjLanduses = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiAddLanduse = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiAddProject = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiAddExpro = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsbSet = New System.Windows.Forms.ToolStripButton()
		Me.tsbUpdateSet = New System.Windows.Forms.ToolStripButton()
		Me.tsbOpenNewSet = New System.Windows.Forms.ToolStripButton()
		Me.tsbRefresh = New System.Windows.Forms.ToolStripButton()
		Me.tsbEditColorScheme = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
		Me.tsbUp = New System.Windows.Forms.ToolStripButton()
		Me.tsbRight = New System.Windows.Forms.ToolStripButton()
		Me.tsbDown = New System.Windows.Forms.ToolStripButton()
		Me.tsbLeft = New System.Windows.Forms.ToolStripButton()
		Me.tsbCheckAll = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSplitButton1 = New System.Windows.Forms.ToolStripSplitButton()
		Me.tsbUncheckAll = New System.Windows.Forms.ToolStripButton()
		Me.tsbClose = New System.Windows.Forms.ToolStripButton()
		Me.imlExtraLarge = New System.Windows.Forms.ImageList(Me.components)
		Me.pcbImage = New System.Windows.Forms.PictureBox()
		Me.pcbLargeImage = New System.Windows.Forms.PictureBox()
		Me.tlbTop.SuspendLayout()
		CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.pcbLargeImage, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'lvwLanduses
		'
		Me.lvwLanduses.AutoArrange = False
		Me.lvwLanduses.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.lvwLanduses.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.lvcLanduseCode, Me.lvcLanduseName, Me.lvcLanduseIndex, Me.lvcColorSchemeID, Me.lvcColorSchemeName})
		Me.lvwLanduses.HideSelection = False
		Me.lvwLanduses.LargeImageList = Me.imlLarge
		Me.lvwLanduses.Location = New System.Drawing.Point(0, 48)
		Me.lvwLanduses.Margin = New System.Windows.Forms.Padding(4)
		Me.lvwLanduses.MultiSelect = False
		Me.lvwLanduses.Name = "lvwLanduses"
		Me.lvwLanduses.RightToLeftLayout = True
		Me.lvwLanduses.ShowGroups = False
		Me.lvwLanduses.ShowItemToolTips = True
		Me.lvwLanduses.Size = New System.Drawing.Size(794, 460)
		Me.lvwLanduses.SmallImageList = Me.imlSmall
		Me.lvwLanduses.TabIndex = 0
		Me.lvwLanduses.TileSize = New System.Drawing.Size(64, 64)
		Me.lvwLanduses.UseCompatibleStateImageBehavior = False
		'
		'lvcLanduseCode
		'
		Me.lvcLanduseCode.Tag = "lvcLanduseCode"
		Me.lvcLanduseCode.Text = "קוד יעוד"
		Me.lvcLanduseCode.Width = 72
		'
		'lvcLanduseName
		'
		Me.lvcLanduseName.Tag = "lvcLanduseName"
		Me.lvcLanduseName.Text = "שם יעוד מקורי"
		Me.lvcLanduseName.Width = 240
		'
		'lvcLanduseIndex
		'
		Me.lvcLanduseIndex.Tag = "lvcLanduseIndex"
		Me.lvcLanduseIndex.Text = "מס' סידורי"
		Me.lvcLanduseIndex.Width = 76
		'
		'lvcColorSchemeID
		'
		Me.lvcColorSchemeID.Tag = "lvcColorSchemeID"
		Me.lvcColorSchemeID.Text = "מס' סט צביעה"
		Me.lvcColorSchemeID.Width = 92
		'
		'lvcColorSchemeName
		'
		Me.lvcColorSchemeName.Tag = "lvcColorSchemeName"
		Me.lvcColorSchemeName.Text = "שם יעוד בפרויקט"
		Me.lvcColorSchemeName.Width = 240
		'
		'imlLarge
		'
		Me.imlLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit
		Me.imlLarge.ImageSize = New System.Drawing.Size(64, 64)
		Me.imlLarge.TransparentColor = System.Drawing.Color.Transparent
		'
		'imlSmall
		'
		Me.imlSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
		Me.imlSmall.ImageSize = New System.Drawing.Size(24, 24)
		Me.imlSmall.TransparentColor = System.Drawing.Color.Transparent
		'
		'tlbTop
		'
		Me.tlbTop.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.tlbTop.ImageScalingSize = New System.Drawing.Size(32, 32)
		Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbViews, Me.tsbSelect, Me.ddbAddSet, Me.tsbSet, Me.tsbUpdateSet, Me.tsbOpenNewSet, Me.tsbRefresh, Me.tsbEditColorScheme, Me.ToolStripSeparator4, Me.tsbUp, Me.tsbRight, Me.tsbDown, Me.tsbLeft, Me.tsbCheckAll, Me.ToolStripSplitButton1, Me.tsbUncheckAll, Me.tsbClose})
		Me.tlbTop.Location = New System.Drawing.Point(0, 0)
		Me.tlbTop.Name = "tlbTop"
		Me.tlbTop.Padding = New System.Windows.Forms.Padding(0)
		Me.tlbTop.Size = New System.Drawing.Size(794, 44)
		Me.tlbTop.TabIndex = 3
		'
		'tsbViews
		'
		Me.tsbViews.AutoSize = False
		Me.tsbViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbViews.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiTile, Me.tsiExtraLarge, Me.tsiLargeIcons, Me.tsiSmallIcons, Me.tsiList, Me.tsiDetails, Me.ToolStripSeparator1, Me.tsiColorScale, Me.ToolStripSeparator2, Me.tsiCheckBox, Me.tsiSelection, Me.ToolStripSeparator3})
		Me.tsbViews.Image = Global.TopoUI.My.Resources.Resources.View
		Me.tsbViews.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbViews.Name = "tsbViews"
		Me.tsbViews.Size = New System.Drawing.Size(42, 41)
		Me.tsbViews.Text = "אפשרויות תצוגה "
		'
		'tsiTile
		'
		Me.tsiTile.CheckOnClick = True
		Me.tsiTile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiTile.Name = "tsiTile"
		Me.tsiTile.Size = New System.Drawing.Size(180, 22)
		Me.tsiTile.Text = "Tiles"
		'
		'tsiExtraLarge
		'
		Me.tsiExtraLarge.CheckOnClick = True
		Me.tsiExtraLarge.Name = "tsiExtraLarge"
		Me.tsiExtraLarge.Size = New System.Drawing.Size(180, 22)
		Me.tsiExtraLarge.Text = "Extra Large"
		'
		'tsiLargeIcons
		'
		Me.tsiLargeIcons.CheckOnClick = True
		Me.tsiLargeIcons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiLargeIcons.Name = "tsiLargeIcons"
		Me.tsiLargeIcons.Size = New System.Drawing.Size(180, 22)
		Me.tsiLargeIcons.Text = "Large Icons"
		'
		'tsiSmallIcons
		'
		Me.tsiSmallIcons.CheckOnClick = True
		Me.tsiSmallIcons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiSmallIcons.Name = "tsiSmallIcons"
		Me.tsiSmallIcons.Size = New System.Drawing.Size(180, 22)
		Me.tsiSmallIcons.Text = "Small Icons"
		'
		'tsiList
		'
		Me.tsiList.CheckOnClick = True
		Me.tsiList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiList.Name = "tsiList"
		Me.tsiList.Size = New System.Drawing.Size(180, 22)
		Me.tsiList.Text = "List"
		'
		'tsiDetails
		'
		Me.tsiDetails.CheckOnClick = True
		Me.tsiDetails.Name = "tsiDetails"
		Me.tsiDetails.Size = New System.Drawing.Size(180, 22)
		Me.tsiDetails.Text = "Details"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(177, 6)
		'
		'tsiColorScale
		'
		Me.tsiColorScale.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiColorScale_1, Me.tsiColorScale_2, Me.tsiColorScale_4, Me.tsiColorScale_6, Me.tsiColorScale_8})
		Me.tsiColorScale.Name = "tsiColorScale"
		Me.tsiColorScale.Size = New System.Drawing.Size(180, 22)
		Me.tsiColorScale.Text = "Scale"
		'
		'tsiColorScale_1
		'
		Me.tsiColorScale_1.CheckOnClick = True
		Me.tsiColorScale_1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiColorScale_1.Name = "tsiColorScale_1"
		Me.tsiColorScale_1.Size = New System.Drawing.Size(83, 22)
		Me.tsiColorScale_1.Text = "1"
		'
		'tsiColorScale_2
		'
		Me.tsiColorScale_2.CheckOnClick = True
		Me.tsiColorScale_2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiColorScale_2.Name = "tsiColorScale_2"
		Me.tsiColorScale_2.Size = New System.Drawing.Size(83, 22)
		Me.tsiColorScale_2.Text = "2"
		'
		'tsiColorScale_4
		'
		Me.tsiColorScale_4.CheckOnClick = True
		Me.tsiColorScale_4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiColorScale_4.Name = "tsiColorScale_4"
		Me.tsiColorScale_4.Size = New System.Drawing.Size(83, 22)
		Me.tsiColorScale_4.Text = "4"
		'
		'tsiColorScale_6
		'
		Me.tsiColorScale_6.CheckOnClick = True
		Me.tsiColorScale_6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiColorScale_6.Name = "tsiColorScale_6"
		Me.tsiColorScale_6.Size = New System.Drawing.Size(83, 22)
		Me.tsiColorScale_6.Text = "6"
		'
		'tsiColorScale_8
		'
		Me.tsiColorScale_8.CheckOnClick = True
		Me.tsiColorScale_8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiColorScale_8.Name = "tsiColorScale_8"
		Me.tsiColorScale_8.Size = New System.Drawing.Size(83, 22)
		Me.tsiColorScale_8.Text = "8"
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(177, 6)
		'
		'tsiCheckBox
		'
		Me.tsiCheckBox.Checked = True
		Me.tsiCheckBox.CheckOnClick = True
		Me.tsiCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
		Me.tsiCheckBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiCheckBox.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsiCheckBox.Name = "tsiCheckBox"
		Me.tsiCheckBox.Size = New System.Drawing.Size(180, 22)
		Me.tsiCheckBox.Text = "CheckBox"
		'
		'tsiSelection
		'
		Me.tsiSelection.CheckOnClick = True
		Me.tsiSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiSelection.Name = "tsiSelection"
		Me.tsiSelection.Size = New System.Drawing.Size(180, 22)
		Me.tsiSelection.Text = "Selection"
		'
		'ToolStripSeparator3
		'
		Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
		Me.ToolStripSeparator3.Size = New System.Drawing.Size(177, 6)
		'
		'tsbSelect
		'
		Me.tsbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbSelect.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbSelect.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbSelect.Name = "tsbSelect"
		Me.tsbSelect.Size = New System.Drawing.Size(23, 41)
		Me.tsbSelect.Text = "בחירת יעוד"
		'
		'ddbAddSet
		'
		Me.ddbAddSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ddbAddSet.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiAddStandard, Me.tsiAddAll, Me.tsiPrjLanduses, Me.tsiAddLanduse, Me.tsiAddProject, Me.tsiAddExpro})
		Me.ddbAddSet.Image = Global.TopoUI.My.Resources.Resources.Plus32Tr
		Me.ddbAddSet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.ddbAddSet.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbAddSet.Name = "ddbAddSet"
		Me.ddbAddSet.Size = New System.Drawing.Size(45, 41)
		Me.ddbAddSet.ToolTipText = "הוספה"
		'
		'tsiAddStandard
		'
		Me.tsiAddStandard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsiAddStandard.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.tsiAddStandard.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsiAddStandard.Name = "tsiAddStandard"
		Me.tsiAddStandard.Size = New System.Drawing.Size(162, 22)
		Me.tsiAddStandard.Text = "מבא""ת"
		'
		'tsiAddAll
		'
		Me.tsiAddAll.Name = "tsiAddAll"
		Me.tsiAddAll.Size = New System.Drawing.Size(162, 22)
		Me.tsiAddAll.Text = "כל היעודים"
		'
		'tsiPrjLanduses
		'
		Me.tsiPrjLanduses.Name = "tsiPrjLanduses"
		Me.tsiPrjLanduses.Size = New System.Drawing.Size(162, 22)
		Me.tsiPrjLanduses.Text = "יעודי הפרויקט"
		'
		'tsiAddLanduse
		'
		Me.tsiAddLanduse.Name = "tsiAddLanduse"
		Me.tsiAddLanduse.Size = New System.Drawing.Size(162, 22)
		Me.tsiAddLanduse.Text = "יעוד"
		'
		'tsiAddProject
		'
		Me.tsiAddProject.Name = "tsiAddProject"
		Me.tsiAddProject.Size = New System.Drawing.Size(162, 22)
		Me.tsiAddProject.Text = "פרויקט"
		Me.tsiAddProject.ToolTipText = "יבוא מפרויקט"
		'
		'tsiAddExpro
		'
		Me.tsiAddExpro.Name = "tsiAddExpro"
		Me.tsiAddExpro.Size = New System.Drawing.Size(162, 22)
		Me.tsiAddExpro.Text = "הפקעות"
		'
		'tsbSet
		'
		Me.tsbSet.CheckOnClick = True
		Me.tsbSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbSet.Image = Global.TopoUI.My.Resources.Resources.Edit32Tr
		Me.tsbSet.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbSet.Name = "tsbSet"
		Me.tsbSet.Size = New System.Drawing.Size(36, 41)
		Me.tsbSet.ToolTipText = "עריכת סט צביעה"
		'
		'tsbUpdateSet
		'
		Me.tsbUpdateSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbUpdateSet.Image = Global.TopoUI.My.Resources.Resources.Save32Tr
		Me.tsbUpdateSet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbUpdateSet.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbUpdateSet.Name = "tsbUpdateSet"
		Me.tsbUpdateSet.Size = New System.Drawing.Size(36, 41)
		Me.tsbUpdateSet.ToolTipText = "Update Set"
		'
		'tsbOpenNewSet
		'
		Me.tsbOpenNewSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbOpenNewSet.Image = Global.TopoUI.My.Resources.Resources.Clear32Tr
		Me.tsbOpenNewSet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbOpenNewSet.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbOpenNewSet.Name = "tsbOpenNewSet"
		Me.tsbOpenNewSet.Size = New System.Drawing.Size(35, 41)
		Me.tsbOpenNewSet.ToolTipText = "Erase new items"
		'
		'tsbRefresh
		'
		Me.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbRefresh.Image = Global.TopoUI.My.Resources.Resources.Refresh32Tr
		Me.tsbRefresh.Name = "tsbRefresh"
		Me.tsbRefresh.Size = New System.Drawing.Size(36, 41)
		Me.tsbRefresh.ToolTipText = "Refresh"
		'
		'tsbEditColorScheme
		'
		Me.tsbEditColorScheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbEditColorScheme.Image = CType(resources.GetObject("tsbEditColorScheme.Image"), System.Drawing.Image)
		Me.tsbEditColorScheme.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbEditColorScheme.Name = "tsbEditColorScheme"
		Me.tsbEditColorScheme.Size = New System.Drawing.Size(36, 41)
		Me.tsbEditColorScheme.ToolTipText = "Schema Editing"
		'
		'ToolStripSeparator4
		'
		Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
		Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 44)
		'
		'tsbUp
		'
		Me.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbUp.Image = Global.TopoUI.My.Resources.Resources.UpTr
		Me.tsbUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbUp.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbUp.Name = "tsbUp"
		Me.tsbUp.Size = New System.Drawing.Size(23, 41)
		Me.tsbUp.ToolTipText = "Up"
		'
		'tsbRight
		'
		Me.tsbRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbRight.Image = Global.TopoUI.My.Resources.Resources.RightTr
		Me.tsbRight.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbRight.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbRight.Name = "tsbRight"
		Me.tsbRight.Size = New System.Drawing.Size(26, 41)
		Me.tsbRight.ToolTipText = "Right \ Start"
		'
		'tsbDown
		'
		Me.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbDown.Image = Global.TopoUI.My.Resources.Resources.DownTr
		Me.tsbDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbDown.Name = "tsbDown"
		Me.tsbDown.Size = New System.Drawing.Size(23, 41)
		Me.tsbDown.ToolTipText = "Down"
		'
		'tsbLeft
		'
		Me.tsbLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbLeft.Image = Global.TopoUI.My.Resources.Resources.LeftTr
		Me.tsbLeft.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbLeft.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbLeft.Name = "tsbLeft"
		Me.tsbLeft.Size = New System.Drawing.Size(26, 41)
		Me.tsbLeft.ToolTipText = "Left \ End"
		'
		'tsbCheckAll
		'
		Me.tsbCheckAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbCheckAll.Image = Global.TopoUI.My.Resources.Resources.CheckP32Tr
		Me.tsbCheckAll.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbCheckAll.Name = "tsbCheckAll"
		Me.tsbCheckAll.Size = New System.Drawing.Size(36, 41)
		Me.tsbCheckAll.Text = "ToolStripButton1"
		Me.tsbCheckAll.ToolTipText = "Check all Items"
		'
		'ToolStripSplitButton1
		'
		Me.ToolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripSplitButton1.Name = "ToolStripSplitButton1"
		Me.ToolStripSplitButton1.Size = New System.Drawing.Size(16, 41)
		Me.ToolStripSplitButton1.Text = "ToolStripSplitButton1"
		'
		'tsbUncheckAll
		'
		Me.tsbUncheckAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbUncheckAll.Image = Global.TopoUI.My.Resources.Resources.UncheckP32Tr
		Me.tsbUncheckAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbUncheckAll.ImageTransparentColor = System.Drawing.Color.Transparent
		Me.tsbUncheckAll.Name = "tsbUncheckAll"
		Me.tsbUncheckAll.Size = New System.Drawing.Size(36, 41)
		Me.tsbUncheckAll.ToolTipText = "Uncheck all Items"
		'
		'tsbClose
		'
		Me.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbClose.Image = Global.TopoUI.My.Resources.Resources.Exit32Tr
		Me.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbClose.Name = "tsbClose"
		Me.tsbClose.Size = New System.Drawing.Size(36, 41)
		'
		'imlExtraLarge
		'
		Me.imlExtraLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit
		Me.imlExtraLarge.ImageSize = New System.Drawing.Size(128, 128)
		Me.imlExtraLarge.TransparentColor = System.Drawing.Color.Transparent
		'
		'pcbImage
		'
		Me.pcbImage.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.pcbImage.Location = New System.Drawing.Point(378, 1500)
		Me.pcbImage.Margin = New System.Windows.Forms.Padding(0)
		Me.pcbImage.Name = "pcbImage"
		Me.pcbImage.Size = New System.Drawing.Size(80, 80)
		Me.pcbImage.TabIndex = 1
		Me.pcbImage.TabStop = False
		'
		'pcbLargeImage
		'
		Me.pcbLargeImage.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.pcbLargeImage.Location = New System.Drawing.Point(528, 241)
		Me.pcbLargeImage.Margin = New System.Windows.Forms.Padding(0)
		Me.pcbLargeImage.Name = "pcbLargeImage"
		Me.pcbLargeImage.Size = New System.Drawing.Size(160, 160)
		Me.pcbLargeImage.TabIndex = 4
		Me.pcbLargeImage.TabStop = False
		'
		'frmEditColorSet
		'
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
		Me.ClientSize = New System.Drawing.Size(794, 505)
		Me.Controls.Add(Me.tlbTop)
		Me.Controls.Add(Me.lvwLanduses)
		Me.Controls.Add(Me.pcbImage)
		Me.Controls.Add(Me.pcbLargeImage)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.Margin = New System.Windows.Forms.Padding(5)
		Me.MaximumSize = New System.Drawing.Size(810, 544)
		Me.Name = "frmEditColorSet"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "סט צביעה / בחירת היעוד"
		Me.tlbTop.ResumeLayout(False)
		Me.tlbTop.PerformLayout()
		CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.pcbLargeImage, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents lvwLanduses As System.Windows.Forms.ListView
   Private WithEvents imlLarge As System.Windows.Forms.ImageList
   Private WithEvents pcbImage As System.Windows.Forms.PictureBox
   Private WithEvents imlSmall As System.Windows.Forms.ImageList
   Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
   Private WithEvents tsbViews As System.Windows.Forms.ToolStripDropDownButton
   Private WithEvents tsiTile As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiLargeIcons As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiSmallIcons As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiList As System.Windows.Forms.ToolStripMenuItem
   Friend WithEvents tsbSelect As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbClose As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbSet As System.Windows.Forms.ToolStripButton
   Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents tsiCheckBox As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsbUpdateSet As System.Windows.Forms.ToolStripButton
   Private WithEvents tsiSelection As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsbOpenNewSet As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbEditColorScheme As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbUp As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbDown As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbRight As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbLeft As System.Windows.Forms.ToolStripButton
   Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents ddbAddSet As System.Windows.Forms.ToolStripDropDownButton
   Private WithEvents tsiAddStandard As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiAddAll As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiAddProject As System.Windows.Forms.ToolStripMenuItem
   Friend WithEvents ToolStripSplitButton1 As System.Windows.Forms.ToolStripSplitButton
   Private WithEvents tsbUncheckAll As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbCheckAll As System.Windows.Forms.ToolStripButton
   Private WithEvents tsiPrjLanduses As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents imlExtraLarge As System.Windows.Forms.ImageList
   Private WithEvents pcbLargeImage As System.Windows.Forms.PictureBox
   Private WithEvents tsiExtraLarge As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiAddLanduse As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale_1 As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale_2 As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale_4 As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale_6 As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiColorScale_8 As System.Windows.Forms.ToolStripMenuItem
   Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents tsbRefresh As System.Windows.Forms.ToolStripButton
   Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
   Friend WithEvents tsiDetails As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents lvcLanduseCode As System.Windows.Forms.ColumnHeader
   Private WithEvents lvcLanduseName As System.Windows.Forms.ColumnHeader
   Private WithEvents lvcLanduseIndex As System.Windows.Forms.ColumnHeader
   Private WithEvents lvcColorSchemeID As System.Windows.Forms.ColumnHeader
   Private WithEvents lvcColorSchemeName As System.Windows.Forms.ColumnHeader
	Friend WithEvents tsiAddExpro As ToolStripMenuItem
End Class
