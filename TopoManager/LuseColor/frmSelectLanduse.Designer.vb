<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectLanduse
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
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSelectLanduse))
      Me.lvwLanduses = New System.Windows.Forms.ListView()
      Me.imlLarge = New System.Windows.Forms.ImageList(Me.components)
      Me.imlSmall = New System.Windows.Forms.ImageList(Me.components)
      Me.tlbTop = New System.Windows.Forms.ToolStrip()
      Me.tsbViews = New System.Windows.Forms.ToolStripDropDownButton()
      Me.tsiTile = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiLargeIcons = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiSmallIcons = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiList = New System.Windows.Forms.ToolStripMenuItem()
      Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
      Me.tsiCheckBox = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiSelection = New System.Windows.Forms.ToolStripMenuItem()
      Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
      Me.tsiStandardOnly = New System.Windows.Forms.ToolStripMenuItem()
      Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
      Me.tsiAppr = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiProp = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsbSelect = New System.Windows.Forms.ToolStripButton()
      Me.tsbSet = New System.Windows.Forms.ToolStripButton()
      Me.cmbColorSets = New System.Windows.Forms.ToolStripComboBox()
      Me.tsbAddColorSet = New System.Windows.Forms.ToolStripButton()
      Me.tsbUpdateSet = New System.Windows.Forms.ToolStripButton()
      Me.tsbOpenNewSet = New System.Windows.Forms.ToolStripButton()
      Me.tsbEditColorScheme = New System.Windows.Forms.ToolStripButton()
      Me.tsbClose = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
      Me.tsbUp = New System.Windows.Forms.ToolStripButton()
      Me.tsbDown = New System.Windows.Forms.ToolStripButton()
      Me.tsbRight = New System.Windows.Forms.ToolStripButton()
      Me.tsbLeft = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
      Me.pcbImage = New System.Windows.Forms.PictureBox()
      Me.tlbTop.SuspendLayout()
      CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'lvwLanduses
      '
      Me.lvwLanduses.AutoArrange = False
      Me.lvwLanduses.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.lvwLanduses.LargeImageList = Me.imlLarge
      Me.lvwLanduses.Location = New System.Drawing.Point(0, 43)
      Me.lvwLanduses.MultiSelect = False
      Me.lvwLanduses.Name = "lvwLanduses"
      Me.lvwLanduses.RightToLeftLayout = True
      Me.lvwLanduses.Size = New System.Drawing.Size(474, 346)
      Me.lvwLanduses.SmallImageList = Me.imlSmall
      Me.lvwLanduses.TabIndex = 0
      Me.lvwLanduses.UseCompatibleStateImageBehavior = False
      '
      'imlLarge
      '
      Me.imlLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
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
      Me.tlbTop.AutoSize = False
      Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbViews, Me.tsbSelect, Me.tsbSet, Me.cmbColorSets, Me.tsbAddColorSet, Me.tsbUpdateSet, Me.tsbOpenNewSet, Me.tsbEditColorScheme, Me.tsbClose, Me.ToolStripSeparator4, Me.tsbUp, Me.tsbDown, Me.tsbRight, Me.tsbLeft, Me.ToolStripButton1})
      Me.tlbTop.Location = New System.Drawing.Point(0, 0)
      Me.tlbTop.Name = "tlbTop"
      Me.tlbTop.Size = New System.Drawing.Size(474, 24)
      Me.tlbTop.TabIndex = 3
      '
      'tsbViews
      '
      Me.tsbViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbViews.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiTile, Me.tsiLargeIcons, Me.tsiSmallIcons, Me.tsiList, Me.ToolStripSeparator1, Me.tsiCheckBox, Me.tsiSelection, Me.ToolStripSeparator2, Me.tsiStandardOnly, Me.ToolStripSeparator3, Me.tsiAppr, Me.tsiProp})
      Me.tsbViews.Image = Global.TopoManager.My.Resources.Resources.Views
      Me.tsbViews.Name = "tsbViews"
      Me.tsbViews.Size = New System.Drawing.Size(29, 21)
      Me.tsbViews.Text = "אפשרויות תצוגה "
      '
      'tsiTile
      '
      Me.tsiTile.CheckOnClick = True
      Me.tsiTile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiTile.Name = "tsiTile"
      Me.tsiTile.Size = New System.Drawing.Size(134, 22)
      Me.tsiTile.Text = "Tiles"
      '
      'tsiLargeIcons
      '
      Me.tsiLargeIcons.CheckOnClick = True
      Me.tsiLargeIcons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiLargeIcons.Name = "tsiLargeIcons"
      Me.tsiLargeIcons.Size = New System.Drawing.Size(134, 22)
      Me.tsiLargeIcons.Text = "Large Icons"
      '
      'tsiSmallIcons
      '
      Me.tsiSmallIcons.CheckOnClick = True
      Me.tsiSmallIcons.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiSmallIcons.Name = "tsiSmallIcons"
      Me.tsiSmallIcons.Size = New System.Drawing.Size(134, 22)
      Me.tsiSmallIcons.Text = "Small Icons"
      '
      'tsiList
      '
      Me.tsiList.CheckOnClick = True
      Me.tsiList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiList.Name = "tsiList"
      Me.tsiList.Size = New System.Drawing.Size(134, 22)
      Me.tsiList.Text = "List"
      '
      'ToolStripSeparator1
      '
      Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
      Me.ToolStripSeparator1.Size = New System.Drawing.Size(131, 6)
      '
      'tsiCheckBox
      '
      Me.tsiCheckBox.CheckOnClick = True
      Me.tsiCheckBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiCheckBox.Name = "tsiCheckBox"
      Me.tsiCheckBox.Size = New System.Drawing.Size(134, 22)
      Me.tsiCheckBox.Text = "CheckBox"
      '
      'tsiSelection
      '
      Me.tsiSelection.CheckOnClick = True
      Me.tsiSelection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiSelection.Name = "tsiSelection"
      Me.tsiSelection.Size = New System.Drawing.Size(134, 22)
      Me.tsiSelection.Text = "Selection"
      '
      'ToolStripSeparator2
      '
      Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
      Me.ToolStripSeparator2.Size = New System.Drawing.Size(131, 6)
      '
      'tsiStandardOnly
      '
      Me.tsiStandardOnly.Checked = True
      Me.tsiStandardOnly.CheckOnClick = True
      Me.tsiStandardOnly.CheckState = System.Windows.Forms.CheckState.Checked
      Me.tsiStandardOnly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiStandardOnly.Name = "tsiStandardOnly"
      Me.tsiStandardOnly.Size = New System.Drawing.Size(134, 22)
      '
      'ToolStripSeparator3
      '
      Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
      Me.ToolStripSeparator3.Size = New System.Drawing.Size(131, 6)
      '
      'tsiAppr
      '
      Me.tsiAppr.CheckOnClick = True
      Me.tsiAppr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiAppr.Name = "tsiAppr"
      Me.tsiAppr.Size = New System.Drawing.Size(134, 22)
      '
      'tsiProp
      '
      Me.tsiProp.CheckOnClick = True
      Me.tsiProp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
      Me.tsiProp.Name = "tsiProp"
      Me.tsiProp.Size = New System.Drawing.Size(134, 22)
      '
      'tsbSelect
      '
      Me.tsbSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbSelect.Image = Global.TopoManager.My.Resources.Resources.[Select]
		Me.tsbSelect.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbSelect.Name = "tsbSelect"
      Me.tsbSelect.Size = New System.Drawing.Size(23, 21)
      Me.tsbSelect.Text = "בחירת יעוד"
      '
      'tsbSet
      '
      Me.tsbSet.CheckOnClick = True
      Me.tsbSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbSet.Image = Global.TopoManager.My.Resources.Resources.Table
      Me.tsbSet.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbSet.Name = "tsbSet"
      Me.tsbSet.Size = New System.Drawing.Size(23, 21)
      Me.tsbSet.ToolTipText = "עריכת סט צביעה"
      '
      'cmbColorSets
      '
      Me.cmbColorSets.Name = "cmbColorSets"
      Me.cmbColorSets.Size = New System.Drawing.Size(121, 24)
      Me.cmbColorSets.ToolTipText = "בחירת סט צביעה"
      '
      'tsbAddColorSet
      '
      Me.tsbAddColorSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbAddColorSet.Image = Global.TopoManager.My.Resources.Resources.AddTable
      Me.tsbAddColorSet.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbAddColorSet.Name = "tsbAddColorSet"
      Me.tsbAddColorSet.Size = New System.Drawing.Size(23, 21)
      Me.tsbAddColorSet.ToolTipText = "הוספת סט צביעה"
      '
      'tsbUpdateSet
      '
      Me.tsbUpdateSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbUpdateSet.Enabled = False
      Me.tsbUpdateSet.Image = Global.TopoManager.My.Resources.Resources.Save
      Me.tsbUpdateSet.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbUpdateSet.Name = "tsbUpdateSet"
      Me.tsbUpdateSet.Size = New System.Drawing.Size(23, 21)
      Me.tsbUpdateSet.ToolTipText = "Update"
      '
      'tsbOpenNewSet
      '
      Me.tsbOpenNewSet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbOpenNewSet.Image = Global.TopoManager.My.Resources.Resources.[New]
		Me.tsbOpenNewSet.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbOpenNewSet.Name = "tsbOpenNewSet"
      Me.tsbOpenNewSet.Size = New System.Drawing.Size(23, 21)
      '
      'tsbEditColorScheme
      '
      Me.tsbEditColorScheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbEditColorScheme.Image = CType(resources.GetObject("tsbEditColorScheme.Image"), System.Drawing.Image)
      Me.tsbEditColorScheme.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbEditColorScheme.Name = "tsbEditColorScheme"
      Me.tsbEditColorScheme.Size = New System.Drawing.Size(23, 21)
      '
      'tsbClose
      '
      Me.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbClose.Image = Global.TopoManager.My.Resources.Resources.[Exit]
		Me.tsbClose.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbClose.Name = "tsbClose"
      Me.tsbClose.Size = New System.Drawing.Size(23, 21)
      '
      'ToolStripSeparator4
      '
      Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
      Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 24)
      '
      'tsbUp
      '
      Me.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbUp.Image = CType(resources.GetObject("tsbUp.Image"), System.Drawing.Image)
      Me.tsbUp.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbUp.Name = "tsbUp"
      Me.tsbUp.Size = New System.Drawing.Size(23, 21)
      '
      'tsbDown
      '
      Me.tsbDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbDown.Image = CType(resources.GetObject("tsbDown.Image"), System.Drawing.Image)
      Me.tsbDown.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbDown.Name = "tsbDown"
      Me.tsbDown.Size = New System.Drawing.Size(23, 21)
      '
      'tsbRight
      '
      Me.tsbRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbRight.Image = CType(resources.GetObject("tsbRight.Image"), System.Drawing.Image)
      Me.tsbRight.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbRight.Name = "tsbRight"
      Me.tsbRight.Size = New System.Drawing.Size(23, 21)
      Me.tsbRight.Text = "ToolStripButton1"
      '
      'tsbLeft
      '
      Me.tsbLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbLeft.Image = CType(resources.GetObject("tsbLeft.Image"), System.Drawing.Image)
      Me.tsbLeft.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbLeft.Name = "tsbLeft"
      Me.tsbLeft.Size = New System.Drawing.Size(23, 21)
      '
      'ToolStripButton1
      '
      Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
      Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton1.Name = "ToolStripButton1"
      Me.ToolStripButton1.Size = New System.Drawing.Size(23, 21)
      Me.ToolStripButton1.Text = "ToolStripButton1"
      '
      'pcbImage
      '
      Me.pcbImage.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer))
      Me.pcbImage.Location = New System.Drawing.Point(12, 343)
      Me.pcbImage.Name = "pcbImage"
      Me.pcbImage.Size = New System.Drawing.Size(64, 64)
      Me.pcbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
      Me.pcbImage.TabIndex = 1
      Me.pcbImage.TabStop = False
      Me.pcbImage.Visible = False
      '
      'frmSelectLanduse
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(474, 389)
      Me.Controls.Add(Me.tlbTop)
      Me.Controls.Add(Me.lvwLanduses)
      Me.Controls.Add(Me.pcbImage)
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      Me.Name = "frmSelectLanduse"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.Text = "בחר יעוד"
      Me.tlbTop.ResumeLayout(False)
      Me.tlbTop.PerformLayout()
      CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

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
	Private WithEvents cmbColorSets As System.Windows.Forms.ToolStripComboBox
	Private WithEvents tsbAddColorSet As System.Windows.Forms.ToolStripButton
	Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Private WithEvents tsiCheckBox As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsbUpdateSet As System.Windows.Forms.ToolStripButton
	Private WithEvents tsiSelection As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsbOpenNewSet As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbEditColorScheme As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
	Private WithEvents tsiStandardOnly As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
	Private WithEvents tsiAppr As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsiProp As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents tsbUp As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbDown As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbRight As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbLeft As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
End Class
