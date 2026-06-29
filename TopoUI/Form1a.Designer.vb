<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1a
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
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1a))
      Me.PanelX = New System.Windows.Forms.Panel()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.PictureBox1 = New System.Windows.Forms.PictureBox()
      Me.lblMapLayerExists = New System.Windows.Forms.Label()
      Me.lblMapLayerCap = New System.Windows.Forms.Label()
      Me.lblMapLayer = New System.Windows.Forms.Label()
      Me.lblPgonCount = New System.Windows.Forms.Label()
      Me.lblThemeCaption = New System.Windows.Forms.Label()
      Me.lblTopoExists = New System.Windows.Forms.Label()
      Me.txtPgonCount = New System.Windows.Forms.TextBox()
      Me.lblTopoNameCap = New System.Windows.Forms.Label()
      Me.lblTopoName = New System.Windows.Forms.Label()
      Me.tstTopology = New System.Windows.Forms.ToolStrip()
      Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
      Me.PanelX.SuspendLayout()
      CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.tstTopology.SuspendLayout()
      Me.SuspendLayout()
      '
      'PanelX
      '
      Me.PanelX.BackColor = System.Drawing.SystemColors.ControlLight
      Me.PanelX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.PanelX.Controls.Add(Me.Label1)
      Me.PanelX.Controls.Add(Me.PictureBox1)
      Me.PanelX.Controls.Add(Me.lblMapLayerExists)
      Me.PanelX.Controls.Add(Me.lblMapLayerCap)
      Me.PanelX.Controls.Add(Me.lblMapLayer)
      Me.PanelX.Controls.Add(Me.lblPgonCount)
      Me.PanelX.Controls.Add(Me.lblThemeCaption)
      Me.PanelX.Controls.Add(Me.lblTopoExists)
      Me.PanelX.Controls.Add(Me.txtPgonCount)
      Me.PanelX.Controls.Add(Me.lblTopoNameCap)
      Me.PanelX.Controls.Add(Me.lblTopoName)
      Me.PanelX.Controls.Add(Me.tstTopology)
      Me.PanelX.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.PanelX.Location = New System.Drawing.Point(0, 0)
      Me.PanelX.Margin = New System.Windows.Forms.Padding(4)
      Me.PanelX.Name = "PanelX"
      Me.PanelX.Size = New System.Drawing.Size(449, 442)
      Me.PanelX.TabIndex = 1
      '
      'Label1
      '
      Me.Label1.BackColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
      Me.Label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.Label1.Location = New System.Drawing.Point(20, 260)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(206, 28)
      Me.Label1.TabIndex = 29
      Me.Label1.Text = "פרויקט 150023"
      Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
      '
      'PictureBox1
      '
      Me.PictureBox1.BackColor = System.Drawing.Color.Red
      Me.PictureBox1.Location = New System.Drawing.Point(222, 260)
      Me.PictureBox1.Name = "PictureBox1"
      Me.PictureBox1.Size = New System.Drawing.Size(222, 28)
      Me.PictureBox1.TabIndex = 28
      Me.PictureBox1.TabStop = False
      '
      'lblMapLayerExists
      '
      Me.lblMapLayerExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblMapLayerExists.Location = New System.Drawing.Point(396, 136)
      Me.lblMapLayerExists.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblMapLayerExists.Name = "lblMapLayerExists"
      Me.lblMapLayerExists.Size = New System.Drawing.Size(30, 25)
      Me.lblMapLayerExists.TabIndex = 27
      '
      'lblMapLayerCap
      '
      Me.lblMapLayerCap.BackColor = System.Drawing.SystemColors.ControlLight
      Me.lblMapLayerCap.ForeColor = System.Drawing.SystemColors.ControlText
      Me.lblMapLayerCap.Location = New System.Drawing.Point(300, 136)
      Me.lblMapLayerCap.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblMapLayerCap.Name = "lblMapLayerCap"
      Me.lblMapLayerCap.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.lblMapLayerCap.Size = New System.Drawing.Size(100, 25)
      Me.lblMapLayerCap.TabIndex = 26
      Me.lblMapLayerCap.Text = "שכבת מפה:"
      Me.lblMapLayerCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      '
      'lblMapLayer
      '
      Me.lblMapLayer.Location = New System.Drawing.Point(184, 136)
      Me.lblMapLayer.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblMapLayer.Name = "lblMapLayer"
      Me.lblMapLayer.Size = New System.Drawing.Size(108, 25)
      Me.lblMapLayer.TabIndex = 25
      Me.lblMapLayer.Text = "Parcels"
      Me.lblMapLayer.TextAlign = System.Drawing.ContentAlignment.MiddleRight
      '
      'lblPgonCount
      '
      Me.lblPgonCount.BackColor = System.Drawing.SystemColors.ControlLight
      Me.lblPgonCount.ForeColor = System.Drawing.SystemColors.ControlText
      Me.lblPgonCount.Location = New System.Drawing.Point(300, 108)
      Me.lblPgonCount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblPgonCount.Name = "lblPgonCount"
      Me.lblPgonCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.lblPgonCount.Size = New System.Drawing.Size(100, 25)
      Me.lblPgonCount.TabIndex = 24
      Me.lblPgonCount.Text = "מס' פוליגונים:"
      Me.lblPgonCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      '
      'lblThemeCaption
      '
      Me.lblThemeCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.lblThemeCaption.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.lblThemeCaption.Location = New System.Drawing.Point(209, 36)
      Me.lblThemeCaption.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblThemeCaption.Name = "lblThemeCaption"
      Me.lblThemeCaption.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.lblThemeCaption.Size = New System.Drawing.Size(180, 25)
      Me.lblThemeCaption.TabIndex = 23
      Me.lblThemeCaption.Text = "חלקות"
      '
      'lblTopoExists
      '
      Me.lblTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblTopoExists.Location = New System.Drawing.Point(396, 80)
      Me.lblTopoExists.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoExists.Name = "lblTopoExists"
      Me.lblTopoExists.Size = New System.Drawing.Size(30, 25)
      Me.lblTopoExists.TabIndex = 21
      '
      'txtPgonCount
      '
      Me.txtPgonCount.Location = New System.Drawing.Point(228, 108)
      Me.txtPgonCount.Margin = New System.Windows.Forms.Padding(4)
      Me.txtPgonCount.Name = "txtPgonCount"
      Me.txtPgonCount.ReadOnly = True
      Me.txtPgonCount.Size = New System.Drawing.Size(64, 22)
      Me.txtPgonCount.TabIndex = 22
      '
      'lblTopoNameCap
      '
      Me.lblTopoNameCap.BackColor = System.Drawing.SystemColors.ControlLight
      Me.lblTopoNameCap.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.lblTopoNameCap.ForeColor = System.Drawing.SystemColors.ControlText
      Me.lblTopoNameCap.Location = New System.Drawing.Point(300, 80)
      Me.lblTopoNameCap.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoNameCap.Name = "lblTopoNameCap"
      Me.lblTopoNameCap.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.lblTopoNameCap.Size = New System.Drawing.Size(100, 25)
      Me.lblTopoNameCap.TabIndex = 20
      Me.lblTopoNameCap.Text = "פוליגונים סגורים:"
      Me.lblTopoNameCap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
      '
      'lblTopoName
      '
      Me.lblTopoName.Location = New System.Drawing.Point(184, 80)
      Me.lblTopoName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoName.Name = "lblTopoName"
      Me.lblTopoName.Size = New System.Drawing.Size(108, 25)
      Me.lblTopoName.TabIndex = 13
      Me.lblTopoName.Text = "Parcels"
      Me.lblTopoName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
      '
      'tstTopology
      '
      Me.tstTopology.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1})
      Me.tstTopology.Location = New System.Drawing.Point(0, 0)
      Me.tstTopology.Name = "tstTopology"
      Me.tstTopology.Size = New System.Drawing.Size(447, 25)
      Me.tstTopology.TabIndex = 11
      '
      'ToolStripButton1
      '
      Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
      Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton1.Name = "ToolStripButton1"
      Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton1.Text = "ToolStripButton1"
      '
      'Form1a
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.BackColor = System.Drawing.SystemColors.GrayText
      Me.ClientSize = New System.Drawing.Size(1170, 455)
      Me.Controls.Add(Me.PanelX)
      Me.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Margin = New System.Windows.Forms.Padding(4)
      Me.Name = "Form1a"
      Me.Text = "Form1a"
      Me.PanelX.ResumeLayout(False)
      Me.PanelX.PerformLayout()
      CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
      Me.tstTopology.ResumeLayout(False)
      Me.tstTopology.PerformLayout()
      Me.ResumeLayout(False)

   End Sub
	Private WithEvents PanelX As System.Windows.Forms.Panel
	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private WithEvents lblTopoName As System.Windows.Forms.Label
	Private WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
	Private WithEvents txtPgonCount As System.Windows.Forms.TextBox
	Private WithEvents lblTopoExists As System.Windows.Forms.Label
	Private WithEvents lblTopoNameCap As System.Windows.Forms.Label
	Private WithEvents lblMapLayerExists As System.Windows.Forms.Label
	Private WithEvents lblMapLayerCap As System.Windows.Forms.Label
	Private WithEvents lblMapLayer As System.Windows.Forms.Label
	Private WithEvents lblPgonCount As System.Windows.Forms.Label
    Private WithEvents lblThemeCaption As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
End Class
