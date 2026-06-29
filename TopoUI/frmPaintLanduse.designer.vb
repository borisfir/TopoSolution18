<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPaintLanduse
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPaintLanduse))
		Me.cmbPaintScale = New System.Windows.Forms.ComboBox()
		Me.cmdPaint = New System.Windows.Forms.Button()
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
		Me.prbPaint = New System.Windows.Forms.ProgressBar()
		Me.cmdCreatePgonset = New System.Windows.Forms.Button()
		Me.chkLayerByLanduse = New System.Windows.Forms.CheckBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.rdbClosedPgons = New System.Windows.Forms.RadioButton()
		Me.lblSourcePgonLayers = New System.Windows.Forms.Label()
		Me.txtSourceClosePgonCount = New System.Windows.Forms.TextBox()
		Me.lblSourceClosedPgonsCap = New System.Windows.Forms.Label()
		Me.lblPgonsetExists = New System.Windows.Forms.Label()
		Me.chkBypassLine = New System.Windows.Forms.CheckBox()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.cmdAddID = New System.Windows.Forms.Button()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.txtException = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtToNumber = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.txtFromNumber = New System.Windows.Forms.TextBox()
		Me.chkLotsInOnly = New System.Windows.Forms.CheckBox()
		Me.grbLanduseTopo.SuspendLayout()
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'cmbPaintScale
		'
		Me.cmbPaintScale.DisplayMember = "Name"
		Me.cmbPaintScale.FormattingEnabled = True
		Me.cmbPaintScale.Location = New System.Drawing.Point(80, 188)
		Me.cmbPaintScale.Name = "cmbPaintScale"
		Me.cmbPaintScale.Size = New System.Drawing.Size(82, 22)
		Me.cmbPaintScale.TabIndex = 0
		Me.cmbPaintScale.ValueMember = "ID"
		'
		'cmdPaint
		'
		Me.cmdPaint.Location = New System.Drawing.Point(10, 284)
		Me.cmdPaint.Name = "cmdPaint"
		Me.cmdPaint.Size = New System.Drawing.Size(58, 23)
		Me.cmdPaint.TabIndex = 1
		Me.cmdPaint.Text = "צביעה"
		Me.cmdPaint.UseVisualStyleBackColor = True
		'
		'cmdEditColorSet
		'
		Me.cmdEditColorSet.Location = New System.Drawing.Point(7, 313)
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
		Me.lblSourceTopoExists.Image = CType(resources.GetObject("lblSourceTopoExists.Image"), System.Drawing.Image)
		Me.lblSourceTopoExists.Location = New System.Drawing.Point(396, 12)
		Me.lblSourceTopoExists.Name = "lblSourceTopoExists"
		Me.lblSourceTopoExists.Size = New System.Drawing.Size(16, 14)
		Me.lblSourceTopoExists.TabIndex = 6
		Me.lblSourceTopoExists.Visible = False
		'
		'cmdClearPaint
		'
		Me.cmdClearPaint.Location = New System.Drawing.Point(98, 284)
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
		Me.lblDissolveTopoExists.Image = CType(resources.GetObject("lblDissolveTopoExists.Image"), System.Drawing.Image)
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
		Me.cmdColorSchemeEditor.Location = New System.Drawing.Point(86, 313)
		Me.cmdColorSchemeEditor.Name = "cmdColorSchemeEditor"
		Me.cmdColorSchemeEditor.Size = New System.Drawing.Size(96, 23)
		Me.cmdColorSchemeEditor.TabIndex = 16
		Me.cmdColorSchemeEditor.Text = "סכימות צביעה"
		Me.cmdColorSchemeEditor.UseVisualStyleBackColor = True
		'
		'cmdClose
		'
		Me.cmdClose.Location = New System.Drawing.Point(348, 313)
		Me.cmdClose.Name = "cmdClose"
		Me.cmdClose.Size = New System.Drawing.Size(62, 23)
		Me.cmdClose.TabIndex = 17
		Me.cmdClose.Text = "סגירה"
		Me.cmdClose.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(164, 189)
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
		Me.cmdRefresh.Location = New System.Drawing.Point(348, 284)
		Me.cmdRefresh.Name = "cmdRefresh"
		Me.cmdRefresh.Size = New System.Drawing.Size(62, 23)
		Me.cmdRefresh.TabIndex = 20
		Me.cmdRefresh.Text = "Refresh"
		Me.cmdRefresh.UseVisualStyleBackColor = True
		'
		'cmdDrawLegend
		'
		Me.cmdDrawLegend.Location = New System.Drawing.Point(272, 284)
		Me.cmdDrawLegend.Name = "cmdDrawLegend"
		Me.cmdDrawLegend.Size = New System.Drawing.Size(64, 23)
		Me.cmdDrawLegend.TabIndex = 21
		Me.cmdDrawLegend.Text = "מקרא"
		Me.cmdDrawLegend.UseVisualStyleBackColor = True
		'
		'txtLegendPaintFactor
		'
		Me.txtLegendPaintFactor.Location = New System.Drawing.Point(282, 188)
		Me.txtLegendPaintFactor.Name = "txtLegendPaintFactor"
		Me.txtLegendPaintFactor.Size = New System.Drawing.Size(54, 22)
		Me.txtLegendPaintFactor.TabIndex = 22
		Me.txtLegendPaintFactor.Text = "0.5"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(339, 189)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(66, 14)
		Me.Label4.TabIndex = 21
		Me.Label4.Text = "יחס מקרא:"
		'
		'cmdReadLog
		'
		Me.cmdReadLog.Location = New System.Drawing.Point(272, 313)
		Me.cmdReadLog.Name = "cmdReadLog"
		Me.cmdReadLog.Size = New System.Drawing.Size(64, 23)
		Me.cmdReadLog.TabIndex = 23
		Me.cmdReadLog.Text = "Log"
		Me.cmdReadLog.UseVisualStyleBackColor = True
		'
		'prbPaint
		'
		Me.prbPaint.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.prbPaint.Location = New System.Drawing.Point(0, 355)
		Me.prbPaint.Name = "prbPaint"
		Me.prbPaint.Size = New System.Drawing.Size(432, 23)
		Me.prbPaint.Step = 1
		Me.prbPaint.TabIndex = 24
		Me.prbPaint.Visible = False
		'
		'cmdCreatePgonset
		'
		Me.cmdCreatePgonset.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdCreatePgonset.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
		Me.cmdCreatePgonset.Location = New System.Drawing.Point(32, 128)
		Me.cmdCreatePgonset.Name = "cmdCreatePgonset"
		Me.cmdCreatePgonset.Size = New System.Drawing.Size(22, 22)
		Me.cmdCreatePgonset.TabIndex = 25
		Me.cmdCreatePgonset.UseVisualStyleBackColor = True
		'
		'chkLayerByLanduse
		'
		Me.chkLayerByLanduse.AutoSize = True
		Me.chkLayerByLanduse.Checked = True
		Me.chkLayerByLanduse.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkLayerByLanduse.Location = New System.Drawing.Point(295, 216)
		Me.chkLayerByLanduse.Name = "chkLayerByLanduse"
		Me.chkLayerByLanduse.Size = New System.Drawing.Size(124, 18)
		Me.chkLayerByLanduse.TabIndex = 26
		Me.chkLayerByLanduse.Text = "שכבות לפי יעודים"
		Me.chkLayerByLanduse.UseVisualStyleBackColor = True
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(82, 152)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(56, 14)
		Me.Label5.TabIndex = 31
		Me.Label5.Text = "מס' פול':"
		'
		'rdbClosedPgons
		'
		Me.rdbClosedPgons.AutoSize = True
		Me.rdbClosedPgons.Location = New System.Drawing.Point(8, 156)
		Me.rdbClosedPgons.Name = "rdbClosedPgons"
		Me.rdbClosedPgons.Size = New System.Drawing.Size(14, 13)
		Me.rdbClosedPgons.TabIndex = 30
		Me.rdbClosedPgons.UseVisualStyleBackColor = True
		'
		'lblSourcePgonLayers
		'
		Me.lblSourcePgonLayers.Location = New System.Drawing.Point(156, 152)
		Me.lblSourcePgonLayers.Name = "lblSourcePgonLayers"
		Me.lblSourcePgonLayers.Size = New System.Drawing.Size(120, 14)
		Me.lblSourcePgonLayers.TabIndex = 29
		'
		'txtSourceClosePgonCount
		'
		Me.txtSourceClosePgonCount.Location = New System.Drawing.Point(42, 152)
		Me.txtSourceClosePgonCount.Name = "txtSourceClosePgonCount"
		Me.txtSourceClosePgonCount.ReadOnly = True
		Me.txtSourceClosePgonCount.Size = New System.Drawing.Size(40, 22)
		Me.txtSourceClosePgonCount.TabIndex = 28
		'
		'lblSourceClosedPgonsCap
		'
		Me.lblSourceClosedPgonsCap.Location = New System.Drawing.Point(280, 152)
		Me.lblSourceClosedPgonsCap.Name = "lblSourceClosedPgonsCap"
		Me.lblSourceClosedPgonsCap.Size = New System.Drawing.Size(112, 14)
		Me.lblSourceClosedPgonsCap.TabIndex = 27
		Me.lblSourceClosedPgonsCap.Text = "שכבת מגרשים:"
		'
		'lblPgonsetExists
		'
		Me.lblPgonsetExists.Image = CType(resources.GetObject("lblPgonsetExists.Image"), System.Drawing.Image)
		Me.lblPgonsetExists.Location = New System.Drawing.Point(396, 152)
		Me.lblPgonsetExists.Name = "lblPgonsetExists"
		Me.lblPgonsetExists.Size = New System.Drawing.Size(16, 14)
		Me.lblPgonsetExists.TabIndex = 32
		Me.lblPgonsetExists.Visible = False
		'
		'chkBypassLine
		'
		Me.chkBypassLine.Checked = True
		Me.chkBypassLine.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkBypassLine.Location = New System.Drawing.Point(295, 236)
		Me.chkBypassLine.Name = "chkBypassLine"
		Me.chkBypassLine.Size = New System.Drawing.Size(124, 18)
		Me.chkBypassLine.TabIndex = 33
		Me.chkBypassLine.Text = "קו היקפי"
		Me.chkBypassLine.UseVisualStyleBackColor = True
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.cmdAddID)
		Me.Panel1.Controls.Add(Me.Label8)
		Me.Panel1.Controls.Add(Me.txtException)
		Me.Panel1.Controls.Add(Me.Label7)
		Me.Panel1.Controls.Add(Me.txtToNumber)
		Me.Panel1.Controls.Add(Me.Label6)
		Me.Panel1.Controls.Add(Me.txtFromNumber)
		Me.Panel1.Location = New System.Drawing.Point(4, 217)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(246, 60)
		Me.Panel1.TabIndex = 34
		'
		'cmdAddID
		'
		Me.cmdAddID.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdAddID.Image = Global.TopoUI.My.Resources.Resources.Plus16Tr
		Me.cmdAddID.Location = New System.Drawing.Point(4, 8)
		Me.cmdAddID.Name = "cmdAddID"
		Me.cmdAddID.Size = New System.Drawing.Size(22, 22)
		Me.cmdAddID.TabIndex = 35
		Me.cmdAddID.UseVisualStyleBackColor = True
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(168, 34)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(68, 14)
		Me.Label8.TabIndex = 27
		Me.Label8.Text = "למעט (ID):"
		'
		'txtException
		'
		Me.txtException.Location = New System.Drawing.Point(4, 32)
		Me.txtException.Name = "txtException"
		Me.txtException.Size = New System.Drawing.Size(164, 22)
		Me.txtException.TabIndex = 28
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(134, 6)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(23, 14)
		Me.Label7.TabIndex = 25
		Me.Label7.Text = "עד"
		'
		'txtToNumber
		'
		Me.txtToNumber.Location = New System.Drawing.Point(90, 4)
		Me.txtToNumber.Name = "txtToNumber"
		Me.txtToNumber.Size = New System.Drawing.Size(42, 22)
		Me.txtToNumber.TabIndex = 26
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(217, 6)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(19, 14)
		Me.Label6.TabIndex = 23
		Me.Label6.Text = "מ-"
		'
		'txtFromNumber
		'
		Me.txtFromNumber.Location = New System.Drawing.Point(174, 4)
		Me.txtFromNumber.Name = "txtFromNumber"
		Me.txtFromNumber.Size = New System.Drawing.Size(42, 22)
		Me.txtFromNumber.TabIndex = 24
		'
		'chkLotsInOnly
		'
		Me.chkLotsInOnly.Location = New System.Drawing.Point(295, 256)
		Me.chkLotsInOnly.Name = "chkLotsInOnly"
		Me.chkLotsInOnly.Size = New System.Drawing.Size(124, 18)
		Me.chkLotsInOnly.TabIndex = 35
		Me.chkLotsInOnly.Text = "מגרשים IN בלבד"
		Me.chkLotsInOnly.UseVisualStyleBackColor = True
		'
		'frmPaintLanduse
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(432, 378)
		Me.Controls.Add(Me.chkLotsInOnly)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me.chkBypassLine)
		Me.Controls.Add(Me.lblPgonsetExists)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.rdbClosedPgons)
		Me.Controls.Add(Me.lblSourcePgonLayers)
		Me.Controls.Add(Me.txtSourceClosePgonCount)
		Me.Controls.Add(Me.lblSourceClosedPgonsCap)
		Me.Controls.Add(Me.chkLayerByLanduse)
		Me.Controls.Add(Me.cmdCreatePgonset)
		Me.Controls.Add(Me.prbPaint)
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
		Me.Controls.Add(Me.cmdPaint)
		Me.Controls.Add(Me.cmbPaintScale)
		Me.Controls.Add(Me.grbLanduseTopo)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Name = "frmPaintLanduse"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "צביעת המפה"
		Me.grbLanduseTopo.ResumeLayout(False)
		Me.grbLanduseTopo.PerformLayout()
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmbPaintScale As System.Windows.Forms.ComboBox
   Private WithEvents cmdPaint As System.Windows.Forms.Button
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
   Private WithEvents prbPaint As System.Windows.Forms.ProgressBar
   Private WithEvents cmdCreatePgonset As System.Windows.Forms.Button
   Private WithEvents chkLayerByLanduse As System.Windows.Forms.CheckBox
   Private WithEvents Label5 As System.Windows.Forms.Label
   Private WithEvents rdbClosedPgons As System.Windows.Forms.RadioButton
   Private WithEvents lblSourcePgonLayers As System.Windows.Forms.Label
   Private WithEvents txtSourceClosePgonCount As System.Windows.Forms.TextBox
   Private WithEvents lblSourceClosedPgonsCap As System.Windows.Forms.Label
   Private WithEvents lblPgonsetExists As System.Windows.Forms.Label
   Private WithEvents chkBypassLine As System.Windows.Forms.CheckBox
   Private WithEvents Label8 As System.Windows.Forms.Label
   Private WithEvents txtException As System.Windows.Forms.TextBox
   Private WithEvents Label7 As System.Windows.Forms.Label
   Private WithEvents txtToNumber As System.Windows.Forms.TextBox
   Private WithEvents Label6 As System.Windows.Forms.Label
   Private WithEvents txtFromNumber As System.Windows.Forms.TextBox
   Private WithEvents cmdAddID As System.Windows.Forms.Button
   Private WithEvents Panel1 As System.Windows.Forms.Panel
	Private WithEvents chkLotsInOnly As CheckBox
End Class
