<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmColorEditor
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
		Try
			If bDisposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(bDisposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.Label2 = New System.Windows.Forms.Label
		Me.txtColorSchemeName = New System.Windows.Forms.TextBox
		Me.grbBorder = New System.Windows.Forms.GroupBox
		Me.grbZebra = New System.Windows.Forms.GroupBox
		Me.grbHatch = New System.Windows.Forms.GroupBox
		Me.tlbTop = New System.Windows.Forms.ToolStrip
		Me.tsbAcadColor = New System.Windows.Forms.ToolStripButton
		Me.tsbWinColor = New System.Windows.Forms.ToolStripButton
		Me.tsbPaint = New System.Windows.Forms.ToolStripButton
		Me.tsbSave = New System.Windows.Forms.ToolStripButton
		Me.tsbUndo = New System.Windows.Forms.ToolStripButton
		Me.tsbDelete = New System.Windows.Forms.ToolStripButton
		Me.tsbClear = New System.Windows.Forms.ToolStripButton
		Me.tsbAddNew = New System.Windows.Forms.ToolStripButton
		Me.tsbExit = New System.Windows.Forms.ToolStripButton
		Me.cmbColorSchemes = New System.Windows.Forms.ComboBox
		Me.grbFill = New System.Windows.Forms.GroupBox
		Me.Label3 = New System.Windows.Forms.Label
		Me.cdlWindow = New System.Windows.Forms.ColorDialog
		Me.txtColorSchemeID = New System.Windows.Forms.TextBox
		Me.pcbPicture = New System.Windows.Forms.PictureBox
		Me.chkStandard = New System.Windows.Forms.CheckBox
		Me.tlbTop.SuspendLayout()
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(390, 64)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(80, 21)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "קוד סכמה"
		'
		'txtColorSchemeName
		'
		Me.txtColorSchemeName.Location = New System.Drawing.Point(298, 114)
		Me.txtColorSchemeName.Multiline = True
		Me.txtColorSchemeName.Name = "txtColorSchemeName"
		Me.txtColorSchemeName.Size = New System.Drawing.Size(172, 60)
		Me.txtColorSchemeName.TabIndex = 3
		'
		'grbBorder
		'
		Me.grbBorder.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbBorder.Location = New System.Drawing.Point(6, 184)
		Me.grbBorder.Name = "grbBorder"
		Me.grbBorder.Size = New System.Drawing.Size(190, 172)
		Me.grbBorder.TabIndex = 6
		Me.grbBorder.TabStop = False
		Me.grbBorder.Text = "קו תוחם שטח"
		'
		'grbZebra
		'
		Me.grbZebra.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbZebra.Location = New System.Drawing.Point(270, 184)
		Me.grbZebra.Name = "grbZebra"
		Me.grbZebra.Size = New System.Drawing.Size(190, 204)
		Me.grbZebra.TabIndex = 7
		Me.grbZebra.TabStop = False
		Me.grbZebra.Text = "מילוי זברה"
		'
		'grbHatch
		'
		Me.grbHatch.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbHatch.Location = New System.Drawing.Point(6, 364)
		Me.grbHatch.Name = "grbHatch"
		Me.grbHatch.Size = New System.Drawing.Size(190, 102)
		Me.grbHatch.TabIndex = 8
		Me.grbHatch.TabStop = False
		Me.grbHatch.Text = "כיסוי עלי"
		'
		'tlbTop
		'
		Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbAcadColor, Me.tsbWinColor, Me.tsbPaint, Me.tsbSave, Me.tsbUndo, Me.tsbDelete, Me.tsbClear, Me.tsbAddNew, Me.tsbExit})
		Me.tlbTop.Location = New System.Drawing.Point(0, 0)
		Me.tlbTop.Name = "tlbTop"
		Me.tlbTop.Size = New System.Drawing.Size(473, 25)
		Me.tlbTop.TabIndex = 9
		Me.tlbTop.Text = "ToolStrip1"
		'
		'tsbAcadColor
		'
		Me.tsbAcadColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbAcadColor.Image = Global.TopoManager.My.Resources.Resources.AStyle
		Me.tsbAcadColor.Name = "tsbAcadColor"
		Me.tsbAcadColor.Size = New System.Drawing.Size(23, 22)
		'
		'tsbWinColor
		'
		Me.tsbWinColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbWinColor.Image = Global.TopoManager.My.Resources.Resources.ColorHS
		Me.tsbWinColor.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbWinColor.Name = "tsbWinColor"
		Me.tsbWinColor.Size = New System.Drawing.Size(23, 22)
		'
		'tsbPaint
		'
		Me.tsbPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbPaint.Image = Global.TopoManager.My.Resources.Resources.Brush
		Me.tsbPaint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tsbPaint.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbPaint.Name = "tsbPaint"
		Me.tsbPaint.Size = New System.Drawing.Size(23, 22)
		'
		'tsbSave
		'
		Me.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbSave.Image = Global.TopoManager.My.Resources.Resources.Save
		Me.tsbSave.Name = "tsbSave"
		Me.tsbSave.Size = New System.Drawing.Size(23, 22)
		Me.tsbSave.Enabled = Not mbReadOnly
		'
		'tsbUndo
		'
		Me.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbUndo.Image = Global.TopoManager.My.Resources.Resources.Undo
		Me.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbUndo.Name = "tsbUndo"
		Me.tsbUndo.Size = New System.Drawing.Size(23, 22)
		'
		'tsbDelete
		'
		Me.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbDelete.Image = Global.TopoManager.My.Resources.Resources.Delete
		Me.tsbDelete.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbDelete.Name = "tsbDelete"
		Me.tsbDelete.Size = New System.Drawing.Size(23, 22)
		'
		'tsbClear
		'
		Me.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbClear.Image = Global.TopoManager.My.Resources.Resources._Erase
		Me.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbClear.Name = "tsbClear"
		Me.tsbClear.Size = New System.Drawing.Size(23, 22)
		'
		'tsbAddNew
		'
		Me.tsbAddNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbAddNew.Image = Global.TopoManager.My.Resources.Resources._New
		Me.tsbAddNew.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbAddNew.Name = "tsbAddNew"
		Me.tsbAddNew.Size = New System.Drawing.Size(23, 22)
		Me.tsbAddNew.Enabled = Not mbReadOnly
		'
		'tsbExit
		'
		Me.tsbExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbExit.Image = Global.TopoManager.My.Resources.Resources.Exit1
		Me.tsbExit.Name = "tsbExit"
		Me.tsbExit.Size = New System.Drawing.Size(23, 22)
		'
		'cmbColorSchemes
		'
		Me.cmbColorSchemes.FormattingEnabled = True
		Me.cmbColorSchemes.Location = New System.Drawing.Point(298, 38)
		Me.cmbColorSchemes.Name = "cmbColorSchemes"
		Me.cmbColorSchemes.Size = New System.Drawing.Size(172, 21)
		Me.cmbColorSchemes.TabIndex = 11
		'
		'grbFill
		'
		Me.grbFill.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbFill.Location = New System.Drawing.Point(270, 396)
		Me.grbFill.Name = "grbFill"
		Me.grbFill.Size = New System.Drawing.Size(190, 50)
		Me.grbFill.TabIndex = 9
		Me.grbFill.TabStop = False
		Me.grbFill.Text = "מילוי מלא צבע"
		'
		'Label3
		'
		Me.Label3.Location = New System.Drawing.Point(390, 94)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(80, 18)
		Me.Label3.TabIndex = 12
		Me.Label3.Text = "שם סכמה"
		'
		'txtColorSchemeID
		'
		Me.txtColorSchemeID.Location = New System.Drawing.Point(298, 64)
		Me.txtColorSchemeID.Name = "txtColorSchemeID"
		Me.txtColorSchemeID.Size = New System.Drawing.Size(53, 21)
		Me.txtColorSchemeID.TabIndex = 13
		'
		'pcbPicture
		'
		Me.pcbPicture.BackColor = System.Drawing.SystemColors.Window
		Me.pcbPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.pcbPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.pcbPicture.Location = New System.Drawing.Point(2, 28)
		Me.pcbPicture.Name = "pcbPicture"
		Me.pcbPicture.Size = New System.Drawing.Size(292, 148)
		Me.pcbPicture.TabIndex = 0
		Me.pcbPicture.TabStop = False
		'
		'chkStandard
		'
		Me.chkStandard.AutoSize = True
		Me.chkStandard.Location = New System.Drawing.Point(298, 91)
		Me.chkStandard.Name = "chkStandard"
		Me.chkStandard.Size = New System.Drawing.Size(79, 17)
		Me.chkStandard.TabIndex = 14
		Me.chkStandard.UseVisualStyleBackColor = True
		'
		'frmColorEditor
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(473, 472)
		Me.Controls.Add(Me.txtColorSchemeID)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.grbFill)
		Me.Controls.Add(Me.grbHatch)
		Me.Controls.Add(Me.grbZebra)
		Me.Controls.Add(Me.grbBorder)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtColorSchemeName)
		Me.Controls.Add(Me.pcbPicture)
		Me.Controls.Add(Me.tlbTop)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "frmColorEditor"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.tlbTop.ResumeLayout(False)
		Me.tlbTop.PerformLayout()
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents pcbPicture As System.Windows.Forms.PictureBox
	Private WithEvents grbBorder As System.Windows.Forms.GroupBox
	Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
	Private WithEvents grbZebra As System.Windows.Forms.GroupBox
	Private WithEvents grbFill As System.Windows.Forms.GroupBox
	Private WithEvents grbHatch As System.Windows.Forms.GroupBox
	Private WithEvents cdlWindow As System.Windows.Forms.ColorDialog
	Private WithEvents tsbWinColor As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbPaint As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbAcadColor As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbSave As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbExit As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbDelete As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbClear As System.Windows.Forms.ToolStripButton
	Private WithEvents tsbUndo As System.Windows.Forms.ToolStripButton
	Private WithEvents txtColorSchemeID As System.Windows.Forms.TextBox
	Private WithEvents txtColorSchemeName As System.Windows.Forms.TextBox
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents cmbColorSchemes As System.Windows.Forms.ComboBox
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents tsbAddNew As System.Windows.Forms.ToolStripButton
	Private WithEvents chkStandard As System.Windows.Forms.CheckBox

End Class
