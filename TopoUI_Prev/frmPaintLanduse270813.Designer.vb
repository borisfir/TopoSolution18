<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPaintLanduse270813
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
		Me.cmbPaintScale = New System.Windows.Forms.ComboBox()
		Me.cmdPaintByLanduse = New System.Windows.Forms.Button()
		Me.cmdEditColorSet = New System.Windows.Forms.Button()
		Me.lblSourceTopologyCap = New System.Windows.Forms.Label()
		Me.txtSourcePgonCount = New System.Windows.Forms.TextBox()
		Me.lblSourceTopoName = New System.Windows.Forms.Label()
		Me.rdbSource = New System.Windows.Forms.RadioButton()
		Me.lblDissolveTopoName = New System.Windows.Forms.Label()
		Me.lblSourceTopoExists = New System.Windows.Forms.Label()
		Me.cmdClearPaint = New System.Windows.Forms.Button()
		Me.grbLanduseTopo = New System.Windows.Forms.GroupBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtDissolvePgonCount = New System.Windows.Forms.TextBox()
		Me.lblDissolveTopoExists = New System.Windows.Forms.Label()
		Me.lblDissolveTopologyCap = New System.Windows.Forms.Label()
		Me.tstLanduseTopology = New System.Windows.Forms.ToolStrip()
		Me.tstBlueLineTopology = New System.Windows.Forms.ToolStrip()
		Me.rdbDissolve = New System.Windows.Forms.RadioButton()
		Me.cmdColorSchemeEditor = New System.Windows.Forms.Button()
		Me.cmdClose = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.cmdRefresh = New System.Windows.Forms.Button()
		Me.cmdDrawLegend = New System.Windows.Forms.Button()
		Me.txtLegendPaintFactor = New System.Windows.Forms.TextBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.cmdReadLog = New System.Windows.Forms.Button()
		Me.grbBlueLineTopo = New System.Windows.Forms.GroupBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.txtBlueLinePgonCount = New System.Windows.Forms.TextBox()
		Me.lblBlueLineTopoExists = New System.Windows.Forms.Label()
		Me.lblBlueLineTopologyCap = New System.Windows.Forms.Label()
		Me.lblBlueLineTopoName = New System.Windows.Forms.Label()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.grbLanduseTopo.SuspendLayout()
		Me.grbBlueLineTopo.SuspendLayout()
		Me.SuspendLayout()
		'
		'cmbPaintScale
		'
		Me.cmbPaintScale.DisplayMember = "Name"
		Me.cmbPaintScale.FormattingEnabled = True
		Me.cmbPaintScale.Location = New System.Drawing.Point(8, 233)
		Me.cmbPaintScale.Name = "cmbPaintScale"
		Me.cmbPaintScale.Size = New System.Drawing.Size(80, 22)
		Me.cmbPaintScale.TabIndex = 0
		Me.cmbPaintScale.ValueMember = "ID"
		'
		'cmdPaintByLanduse
		'
		Me.cmdPaintByLanduse.Location = New System.Drawing.Point(8, 263)
		Me.cmdPaintByLanduse.Name = "cmdPaintByLanduse"
		Me.cmdPaintByLanduse.Size = New System.Drawing.Size(48, 23)
		Me.cmdPaintByLanduse.TabIndex = 1
		Me.cmdPaintByLanduse.Text = "צביעה"
		Me.cmdPaintByLanduse.UseVisualStyleBackColor = True
		'
		'cmdEditColorSet
		'
		Me.cmdEditColorSet.Location = New System.Drawing.Point(8, 309)
		Me.cmdEditColorSet.Name = "cmdEditColorSet"
		Me.cmdEditColorSet.Size = New System.Drawing.Size(71, 23)
		Me.cmdEditColorSet.TabIndex = 2
		Me.cmdEditColorSet.Text = "סט צביעה"
		Me.cmdEditColorSet.UseVisualStyleBackColor = True
		'
		'lblSourceTopologyCap
		'
		Me.lblSourceTopologyCap.Location = New System.Drawing.Point(280, 12)
		Me.lblSourceTopologyCap.Name = "lblSourceTopologyCap"
		Me.lblSourceTopologyCap.Size = New System.Drawing.Size(112, 14)
		Me.lblSourceTopologyCap.TabIndex = 3
		Me.lblSourceTopologyCap.Text = "טופולוגיית מגרשים:"
		'
		'txtSourcePgonCount
		'
		Me.txtSourcePgonCount.Location = New System.Drawing.Point(42, 12)
		Me.txtSourcePgonCount.Name = "txtSourcePgonCount"
		Me.txtSourcePgonCount.ReadOnly = True
		Me.txtSourcePgonCount.Size = New System.Drawing.Size(40, 22)
		Me.txtSourcePgonCount.TabIndex = 4
		'
		'lblSourceTopoName
		'
		Me.lblSourceTopoName.Location = New System.Drawing.Point(156, 12)
		Me.lblSourceTopoName.Name = "lblSourceTopoName"
		Me.lblSourceTopoName.Size = New System.Drawing.Size(120, 14)
		Me.lblSourceTopoName.TabIndex = 5
		'
		'rdbSource
		'
		Me.rdbSource.AutoSize = True
		Me.rdbSource.Location = New System.Drawing.Point(8, 16)
		Me.rdbSource.Name = "rdbSource"
		Me.rdbSource.Size = New System.Drawing.Size(14, 13)
		Me.rdbSource.TabIndex = 7
		Me.rdbSource.UseVisualStyleBackColor = True
		'
		'lblDissolveTopoName
		'
		Me.lblDissolveTopoName.Location = New System.Drawing.Point(128, 48)
		Me.lblDissolveTopoName.Name = "lblDissolveTopoName"
		Me.lblDissolveTopoName.Size = New System.Drawing.Size(120, 14)
		Me.lblDissolveTopoName.TabIndex = 10
		'
		'lblSourceTopoExists
		'
		Me.lblSourceTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
		Me.lblSourceTopoExists.Location = New System.Drawing.Point(396, 12)
		Me.lblSourceTopoExists.Name = "lblSourceTopoExists"
		Me.lblSourceTopoExists.Size = New System.Drawing.Size(16, 14)
		Me.lblSourceTopoExists.TabIndex = 6
		Me.lblSourceTopoExists.Visible = False
		'
		'cmdClearPaint
		'
		Me.cmdClearPaint.Location = New System.Drawing.Point(60, 263)
		Me.cmdClearPaint.Name = "cmdClearPaint"
		Me.cmdClearPaint.Size = New System.Drawing.Size(50, 23)
		Me.cmdClearPaint.TabIndex = 13
		Me.cmdClearPaint.Text = "מחיקה"
		Me.cmdClearPaint.UseVisualStyleBackColor = True
		'
		'grbLanduseTopo
		'
		Me.grbLanduseTopo.Controls.Add(Me.Label3)
		Me.grbLanduseTopo.Controls.Add(Me.txtDissolvePgonCount)
		Me.grbLanduseTopo.Controls.Add(Me.lblDissolveTopoExists)
		Me.grbLanduseTopo.Controls.Add(Me.lblDissolveTopologyCap)
		Me.grbLanduseTopo.Controls.Add(Me.lblDissolveTopoName)
		Me.grbLanduseTopo.Controls.Add(Me.tstLanduseTopology)
		Me.grbLanduseTopo.Location = New System.Drawing.Point(32, 44)
		Me.grbLanduseTopo.Name = "grbLanduseTopo"
		Me.grbLanduseTopo.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.grbLanduseTopo.Size = New System.Drawing.Size(396, 78)
		Me.grbLanduseTopo.TabIndex = 14
		Me.grbLanduseTopo.TabStop = False
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(54, 51)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(56, 14)
		Me.Label3.TabIndex = 20
		Me.Label3.Text = "מס' פול':"
		'
		'txtDissolvePgonCount
		'
		Me.txtDissolvePgonCount.Location = New System.Drawing.Point(14, 48)
		Me.txtDissolvePgonCount.Name = "txtDissolvePgonCount"
		Me.txtDissolvePgonCount.ReadOnly = True
		Me.txtDissolvePgonCount.Size = New System.Drawing.Size(40, 22)
		Me.txtDissolvePgonCount.TabIndex = 15
		'
		'lblDissolveTopoExists
		'
		Me.lblDissolveTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
		Me.lblDissolveTopoExists.Location = New System.Drawing.Point(364, 48)
		Me.lblDissolveTopoExists.Name = "lblDissolveTopoExists"
		Me.lblDissolveTopoExists.Size = New System.Drawing.Size(16, 14)
		Me.lblDissolveTopoExists.TabIndex = 14
		'
		'lblDissolveTopologyCap
		'
		Me.lblDissolveTopologyCap.Location = New System.Drawing.Point(248, 48)
		Me.lblDissolveTopologyCap.Name = "lblDissolveTopologyCap"
		Me.lblDissolveTopologyCap.Size = New System.Drawing.Size(112, 14)
		Me.lblDissolveTopologyCap.TabIndex = 13
		Me.lblDissolveTopologyCap.Text = "טופולוגיית יעודים:"
		'
		'tstLanduseTopology
		'
		Me.tstLanduseTopology.Dock = System.Windows.Forms.DockStyle.None
		Me.tstLanduseTopology.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.tstLanduseTopology.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
		Me.tstLanduseTopology.Location = New System.Drawing.Point(11, 10)
		Me.tstLanduseTopology.Name = "tstLanduseTopology"
		Me.tstLanduseTopology.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.tstLanduseTopology.Size = New System.Drawing.Size(1, 0)
		Me.tstLanduseTopology.TabIndex = 16
		'
		'tstBlueLineTopology
		'
		Me.tstBlueLineTopology.Location = New System.Drawing.Point(3, 18)
		Me.tstBlueLineTopology.Name = "tstBlueLineTopology"
		Me.tstBlueLineTopology.Size = New System.Drawing.Size(390, 25)
		Me.tstBlueLineTopology.TabIndex = 0
		'
		'rdbDissolve
		'
		Me.rdbDissolve.AutoSize = True
		Me.rdbDissolve.Location = New System.Drawing.Point(8, 96)
		Me.rdbDissolve.Name = "rdbDissolve"
		Me.rdbDissolve.Size = New System.Drawing.Size(14, 13)
		Me.rdbDissolve.TabIndex = 15
		Me.rdbDissolve.UseVisualStyleBackColor = True
		'
		'cmdColorSchemeEditor
		'
		Me.cmdColorSchemeEditor.Location = New System.Drawing.Point(86, 309)
		Me.cmdColorSchemeEditor.Name = "cmdColorSchemeEditor"
		Me.cmdColorSchemeEditor.Size = New System.Drawing.Size(96, 23)
		Me.cmdColorSchemeEditor.TabIndex = 16
		Me.cmdColorSchemeEditor.Text = "סכימות צביעה"
		Me.cmdColorSchemeEditor.UseVisualStyleBackColor = True
		'
		'cmdClose
		'
		Me.cmdClose.Location = New System.Drawing.Point(348, 309)
		Me.cmdClose.Name = "cmdClose"
		Me.cmdClose.Size = New System.Drawing.Size(61, 23)
		Me.cmdClose.TabIndex = 17
		Me.cmdClose.Text = "סגירה"
		Me.cmdClose.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(88, 233)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(76, 14)
		Me.Label1.TabIndex = 18
		Me.Label1.Text = "קנ""מ צביעה:"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(82, 12)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(56, 14)
		Me.Label2.TabIndex = 19
		Me.Label2.Text = "מס' פול':"
		'
		'cmdRefresh
		'
		Me.cmdRefresh.Location = New System.Drawing.Point(348, 263)
		Me.cmdRefresh.Name = "cmdRefresh"
		Me.cmdRefresh.Size = New System.Drawing.Size(61, 23)
		Me.cmdRefresh.TabIndex = 20
		Me.cmdRefresh.Text = "Refresh"
		Me.cmdRefresh.UseVisualStyleBackColor = True
		'
		'cmdDrawLegend
		'
		Me.cmdDrawLegend.Location = New System.Drawing.Point(269, 263)
		Me.cmdDrawLegend.Name = "cmdDrawLegend"
		Me.cmdDrawLegend.Size = New System.Drawing.Size(73, 23)
		Me.cmdDrawLegend.TabIndex = 21
		Me.cmdDrawLegend.Text = "מקרא"
		Me.cmdDrawLegend.UseVisualStyleBackColor = True
		'
		'txtLegendPaintFactor
		'
		Me.txtLegendPaintFactor.Location = New System.Drawing.Point(209, 233)
		Me.txtLegendPaintFactor.Name = "txtLegendPaintFactor"
		Me.txtLegendPaintFactor.Size = New System.Drawing.Size(54, 22)
		Me.txtLegendPaintFactor.TabIndex = 22
		Me.txtLegendPaintFactor.Text = "0.5"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(269, 233)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(66, 14)
		Me.Label4.TabIndex = 21
		Me.Label4.Text = "יחס מקרא:"
		'
		'cmdReadLog
		'
		Me.cmdReadLog.Location = New System.Drawing.Point(269, 309)
		Me.cmdReadLog.Name = "cmdReadLog"
		Me.cmdReadLog.Size = New System.Drawing.Size(63, 23)
		Me.cmdReadLog.TabIndex = 23
		Me.cmdReadLog.Text = "Log"
		Me.cmdReadLog.UseVisualStyleBackColor = True
		'
		'grbBlueLineTopo
		'
		Me.grbBlueLineTopo.Controls.Add(Me.Label5)
		Me.grbBlueLineTopo.Controls.Add(Me.tstBlueLineTopology)
		Me.grbBlueLineTopo.Controls.Add(Me.txtBlueLinePgonCount)
		Me.grbBlueLineTopo.Controls.Add(Me.lblBlueLineTopoExists)
		Me.grbBlueLineTopo.Controls.Add(Me.lblBlueLineTopologyCap)
		Me.grbBlueLineTopo.Controls.Add(Me.lblBlueLineTopoName)
		Me.grbBlueLineTopo.Location = New System.Drawing.Point(32, 128)
		Me.grbBlueLineTopo.Name = "grbBlueLineTopo"
		Me.grbBlueLineTopo.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.grbBlueLineTopo.Size = New System.Drawing.Size(396, 78)
		Me.grbBlueLineTopo.TabIndex = 21
		Me.grbBlueLineTopo.TabStop = False
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(54, 51)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(56, 14)
		Me.Label5.TabIndex = 20
		Me.Label5.Text = "מס' פול':"
		'
		'txtBlueLinePgonCount
		'
		Me.txtBlueLinePgonCount.Location = New System.Drawing.Point(14, 48)
		Me.txtBlueLinePgonCount.Name = "txtBlueLinePgonCount"
		Me.txtBlueLinePgonCount.ReadOnly = True
		Me.txtBlueLinePgonCount.Size = New System.Drawing.Size(40, 22)
		Me.txtBlueLinePgonCount.TabIndex = 15
		'
		'lblBlueLineTopoExists
		'
		Me.lblBlueLineTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
		Me.lblBlueLineTopoExists.Location = New System.Drawing.Point(364, 48)
		Me.lblBlueLineTopoExists.Name = "lblBlueLineTopoExists"
		Me.lblBlueLineTopoExists.Size = New System.Drawing.Size(16, 14)
		Me.lblBlueLineTopoExists.TabIndex = 14
		'
		'lblBlueLineTopologyCap
		'
		Me.lblBlueLineTopologyCap.Location = New System.Drawing.Point(248, 48)
		Me.lblBlueLineTopologyCap.Name = "lblBlueLineTopologyCap"
		Me.lblBlueLineTopologyCap.Size = New System.Drawing.Size(112, 14)
		Me.lblBlueLineTopologyCap.TabIndex = 13
		Me.lblBlueLineTopologyCap.Text = "טופולוגיית קו כחול:"
		'
		'lblBlueLineTopoName
		'
		Me.lblBlueLineTopoName.Location = New System.Drawing.Point(128, 48)
		Me.lblBlueLineTopoName.Name = "lblBlueLineTopoName"
		Me.lblBlueLineTopoName.Size = New System.Drawing.Size(120, 14)
		Me.lblBlueLineTopoName.TabIndex = 10
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(168, 263)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(50, 23)
		Me.Button1.TabIndex = 25
		Me.Button1.Text = "מחיקה"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(116, 263)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(48, 23)
		Me.Button2.TabIndex = 24
		Me.Button2.Text = "צביעה"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'frmPaintLanduse
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(432, 414)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.grbBlueLineTopo)
		Me.Controls.Add(Me.cmdReadLog)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.txtLegendPaintFactor)
		Me.Controls.Add(Me.cmdDrawLegend)
		Me.Controls.Add(Me.cmdRefresh)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.cmdClose)
		Me.Controls.Add(Me.cmdColorSchemeEditor)
		Me.Controls.Add(Me.rdbDissolve)
		Me.Controls.Add(Me.cmdClearPaint)
		Me.Controls.Add(Me.rdbSource)
		Me.Controls.Add(Me.lblSourceTopoExists)
		Me.Controls.Add(Me.lblSourceTopoName)
		Me.Controls.Add(Me.txtSourcePgonCount)
		Me.Controls.Add(Me.lblSourceTopologyCap)
		Me.Controls.Add(Me.cmdEditColorSet)
		Me.Controls.Add(Me.cmdPaintByLanduse)
		Me.Controls.Add(Me.cmbPaintScale)
		Me.Controls.Add(Me.grbLanduseTopo)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Name = "frmPaintLanduse"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "***"
		Me.grbLanduseTopo.ResumeLayout(False)
		Me.grbLanduseTopo.PerformLayout()
		Me.grbBlueLineTopo.ResumeLayout(False)
		Me.grbBlueLineTopo.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmbPaintScale As System.Windows.Forms.ComboBox
	Private WithEvents cmdPaintByLanduse As System.Windows.Forms.Button
	Private WithEvents cmdEditColorSet As System.Windows.Forms.Button
	Private WithEvents lblSourceTopologyCap As System.Windows.Forms.Label
	Private WithEvents txtSourcePgonCount As System.Windows.Forms.TextBox
	Private WithEvents lblSourceTopoName As System.Windows.Forms.Label
	Private WithEvents lblSourceTopoExists As System.Windows.Forms.Label
	Private WithEvents rdbSource As System.Windows.Forms.RadioButton
	Private WithEvents lblDissolveTopoName As System.Windows.Forms.Label
	Private WithEvents cmdClearPaint As System.Windows.Forms.Button
	Private WithEvents lblDissolveTopologyCap As System.Windows.Forms.Label
	Private WithEvents tstLanduseTopology As System.Windows.Forms.ToolStrip
	Private WithEvents tstBlueLineTopology As System.Windows.Forms.ToolStrip
	Private WithEvents txtDissolvePgonCount As System.Windows.Forms.TextBox
	Private WithEvents lblDissolveTopoExists As System.Windows.Forms.Label
	Private WithEvents grbLanduseTopo As System.Windows.Forms.GroupBox
	Private WithEvents rdbDissolve As System.Windows.Forms.RadioButton
	Private WithEvents cmdColorSchemeEditor As System.Windows.Forms.Button
	Private WithEvents cmdClose As System.Windows.Forms.Button
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents txtLegendPaintFactor As System.Windows.Forms.TextBox
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents cmdDrawLegend As System.Windows.Forms.Button
	Private WithEvents cmdRefresh As System.Windows.Forms.Button
	Private WithEvents cmdReadLog As System.Windows.Forms.Button
	Private WithEvents grbBlueLineTopo As System.Windows.Forms.GroupBox
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents txtBlueLinePgonCount As System.Windows.Forms.TextBox
	Private WithEvents lblBlueLineTopoExists As System.Windows.Forms.Label
	Private WithEvents lblBlueLineTopologyCap As System.Windows.Forms.Label
	Private WithEvents lblBlueLineTopoName As System.Windows.Forms.Label
	Private WithEvents Button1 As System.Windows.Forms.Button
	Private WithEvents Button2 As System.Windows.Forms.Button
End Class
