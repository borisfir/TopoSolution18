<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTopoToClosedPgons
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
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTopoToClosedPgons))
      Me.lblDissolveTopoName = New System.Windows.Forms.Label()
      Me.grbDissolveTopo = New System.Windows.Forms.GroupBox()
      Me.cmdClearMarkBlocks = New System.Windows.Forms.Button()
      Me.cmdSetMarkBlocks = New System.Windows.Forms.Button()
      Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
      Me.chkDisExteriorRingOnly = New System.Windows.Forms.CheckBox()
      Me.cmdEraseDisClosedPgons = New System.Windows.Forms.Button()
      Me.cmdDisToClosedPgons = New System.Windows.Forms.Button()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.txtDissolvePLinesCount = New System.Windows.Forms.TextBox()
      Me.lblDissolveCPgonsExist = New System.Windows.Forms.Label()
      Me.Label5 = New System.Windows.Forms.Label()
      Me.lblDissolveCPgonsName = New System.Windows.Forms.Label()
      Me.Label3 = New System.Windows.Forms.Label()
      Me.tstTopology = New System.Windows.Forms.ToolStrip()
      Me.txtDissolvePgonCount = New System.Windows.Forms.TextBox()
      Me.lblDissolveTopoExists = New System.Windows.Forms.Label()
      Me.lblDissolveTopologyCap = New System.Windows.Forms.Label()
      Me.cmdClose = New System.Windows.Forms.Button()
      Me.dgvBlocks = New System.Windows.Forms.DataGridView()
      Me.ctxiParcelCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.chkBlock = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      Me.cchPgonExists = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      Me.grbSourceTopo = New System.Windows.Forms.GroupBox()
      Me.chkSrcExteriorRingOnly = New System.Windows.Forms.CheckBox()
      Me.rdbTopoWA = New System.Windows.Forms.RadioButton()
      Me.rdbTopo = New System.Windows.Forms.RadioButton()
      Me.Label2 = New System.Windows.Forms.Label()
      Me.txtSourceWAPgonCount = New System.Windows.Forms.TextBox()
      Me.lblSourceTopoWAExists = New System.Windows.Forms.Label()
      Me.Label6 = New System.Windows.Forms.Label()
      Me.lblSourceTopoWAName = New System.Windows.Forms.Label()
      Me.lblSourceCPgonsName = New System.Windows.Forms.Label()
      Me.cmdEraseSrcClosedPgons = New System.Windows.Forms.Button()
      Me.cmdSrcToClosedPgons = New System.Windows.Forms.Button()
      Me.Label7 = New System.Windows.Forms.Label()
      Me.txtSourcePLinesCount = New System.Windows.Forms.TextBox()
      Me.lblSourceCPgonsExist = New System.Windows.Forms.Label()
      Me.Label9 = New System.Windows.Forms.Label()
      Me.Label11 = New System.Windows.Forms.Label()
      Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
      Me.txtSourcePgonCount = New System.Windows.Forms.TextBox()
      Me.lblSourceTopoExists = New System.Windows.Forms.Label()
      Me.Label13 = New System.Windows.Forms.Label()
      Me.lblSourceTopoName = New System.Windows.Forms.Label()
      Me.cmdUpdateData = New System.Windows.Forms.Button()
      Me.cmdExportShape = New System.Windows.Forms.Button()
      Me.cmdFillODTable = New System.Windows.Forms.Button()
      Me.sfdShapeFile = New System.Windows.Forms.SaveFileDialog()
      Me.cmdZoom = New System.Windows.Forms.Button()
      Me.cmbODType = New System.Windows.Forms.ComboBox()
      Me.grbDissolveTopo.SuspendLayout()
      CType(Me.dgvBlocks, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.grbSourceTopo.SuspendLayout()
      Me.SuspendLayout()
      '
      'lblDissolveTopoName
      '
      Me.lblDissolveTopoName.Location = New System.Drawing.Point(200, 48)
      Me.lblDissolveTopoName.Name = "lblDissolveTopoName"
      Me.lblDissolveTopoName.Size = New System.Drawing.Size(120, 14)
      Me.lblDissolveTopoName.TabIndex = 10
      '
      'grbDissolveTopo
      '
      Me.grbDissolveTopo.Controls.Add(Me.cmdClearMarkBlocks)
      Me.grbDissolveTopo.Controls.Add(Me.cmdSetMarkBlocks)
      Me.grbDissolveTopo.Controls.Add(Me.chkDisExteriorRingOnly)
      Me.grbDissolveTopo.Controls.Add(Me.cmdEraseDisClosedPgons)
      Me.grbDissolveTopo.Controls.Add(Me.cmdDisToClosedPgons)
      Me.grbDissolveTopo.Controls.Add(Me.Label1)
      Me.grbDissolveTopo.Controls.Add(Me.txtDissolvePLinesCount)
      Me.grbDissolveTopo.Controls.Add(Me.lblDissolveCPgonsExist)
      Me.grbDissolveTopo.Controls.Add(Me.Label5)
      Me.grbDissolveTopo.Controls.Add(Me.lblDissolveCPgonsName)
      Me.grbDissolveTopo.Controls.Add(Me.Label3)
      Me.grbDissolveTopo.Controls.Add(Me.tstTopology)
      Me.grbDissolveTopo.Controls.Add(Me.txtDissolvePgonCount)
      Me.grbDissolveTopo.Controls.Add(Me.lblDissolveTopoExists)
      Me.grbDissolveTopo.Controls.Add(Me.lblDissolveTopologyCap)
      Me.grbDissolveTopo.Controls.Add(Me.lblDissolveTopoName)
      Me.grbDissolveTopo.Location = New System.Drawing.Point(6, 160)
      Me.grbDissolveTopo.Name = "grbDissolveTopo"
      Me.grbDissolveTopo.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.grbDissolveTopo.Size = New System.Drawing.Size(480, 136)
      Me.grbDissolveTopo.TabIndex = 14
      Me.grbDissolveTopo.TabStop = False
      Me.grbDissolveTopo.Text = " גושים"
      '
      'cmdClearMarkBlocks
      '
      Me.cmdClearMarkBlocks.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
      Me.cmdClearMarkBlocks.Location = New System.Drawing.Point(122, 104)
      Me.cmdClearMarkBlocks.Name = "cmdClearMarkBlocks"
      Me.cmdClearMarkBlocks.Size = New System.Drawing.Size(24, 22)
      Me.cmdClearMarkBlocks.TabIndex = 39
      Me.cmdClearMarkBlocks.UseVisualStyleBackColor = True
      '
      'cmdSetMarkBlocks
      '
      Me.cmdSetMarkBlocks.AutoSize = True
      Me.cmdSetMarkBlocks.ImageIndex = 0
      Me.cmdSetMarkBlocks.ImageList = Me.ImageList1
      Me.cmdSetMarkBlocks.Location = New System.Drawing.Point(152, 104)
      Me.cmdSetMarkBlocks.Name = "cmdSetMarkBlocks"
      Me.cmdSetMarkBlocks.Size = New System.Drawing.Size(28, 22)
      Me.cmdSetMarkBlocks.TabIndex = 38
      Me.cmdSetMarkBlocks.UseVisualStyleBackColor = True
      '
      'ImageList1
      '
      Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
      Me.ImageList1.TransparentColor = System.Drawing.Color.White
      Me.ImageList1.Images.SetKeyName(0, "Block1620")
      '
      'chkDisExteriorRingOnly
      '
      Me.chkDisExteriorRingOnly.AutoSize = True
      Me.chkDisExteriorRingOnly.Checked = True
      Me.chkDisExteriorRingOnly.CheckState = System.Windows.Forms.CheckState.Checked
      Me.chkDisExteriorRingOnly.Location = New System.Drawing.Point(12, 104)
      Me.chkDisExteriorRingOnly.Name = "chkDisExteriorRingOnly"
      Me.chkDisExteriorRingOnly.Size = New System.Drawing.Size(71, 18)
      Me.chkDisExteriorRingOnly.TabIndex = 37
      Me.chkDisExteriorRingOnly.Text = "בלי איים"
      Me.chkDisExteriorRingOnly.UseVisualStyleBackColor = True
      '
      'cmdEraseDisClosedPgons
      '
      Me.cmdEraseDisClosedPgons.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
      Me.cmdEraseDisClosedPgons.Location = New System.Drawing.Point(12, 76)
      Me.cmdEraseDisClosedPgons.Name = "cmdEraseDisClosedPgons"
      Me.cmdEraseDisClosedPgons.Size = New System.Drawing.Size(24, 22)
      Me.cmdEraseDisClosedPgons.TabIndex = 27
      Me.cmdEraseDisClosedPgons.UseVisualStyleBackColor = True
      '
      'cmdDisToClosedPgons
      '
      Me.cmdDisToClosedPgons.AutoSize = True
      Me.cmdDisToClosedPgons.Image = Global.TopoUI.My.Resources.Resources.ToClosedPgons
      Me.cmdDisToClosedPgons.Location = New System.Drawing.Point(42, 76)
      Me.cmdDisToClosedPgons.Name = "cmdDisToClosedPgons"
      Me.cmdDisToClosedPgons.Size = New System.Drawing.Size(27, 22)
      Me.cmdDisToClosedPgons.TabIndex = 26
      Me.cmdDisToClosedPgons.UseVisualStyleBackColor = True
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(126, 79)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(56, 14)
      Me.Label1.TabIndex = 25
      Me.Label1.Text = "מס' פול':"
      '
      'txtDissolvePLinesCount
      '
      Me.txtDissolvePLinesCount.Location = New System.Drawing.Point(86, 76)
      Me.txtDissolvePLinesCount.Name = "txtDissolvePLinesCount"
      Me.txtDissolvePLinesCount.ReadOnly = True
      Me.txtDissolvePLinesCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtDissolvePLinesCount.Size = New System.Drawing.Size(40, 22)
      Me.txtDissolvePLinesCount.TabIndex = 24
      '
      'lblDissolveCPgonsExist
      '
      Me.lblDissolveCPgonsExist.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblDissolveCPgonsExist.Location = New System.Drawing.Point(454, 76)
      Me.lblDissolveCPgonsExist.Name = "lblDissolveCPgonsExist"
      Me.lblDissolveCPgonsExist.Size = New System.Drawing.Size(16, 14)
      Me.lblDissolveCPgonsExist.TabIndex = 23
      Me.lblDissolveCPgonsExist.Visible = False
      '
      'Label5
      '
      Me.Label5.Location = New System.Drawing.Point(332, 76)
      Me.Label5.Name = "Label5"
      Me.Label5.Size = New System.Drawing.Size(120, 14)
      Me.Label5.TabIndex = 22
      Me.Label5.Text = "שכבת פוליגונים סג':"
      '
      'lblDissolveCPgonsName
      '
      Me.lblDissolveCPgonsName.Location = New System.Drawing.Point(200, 76)
      Me.lblDissolveCPgonsName.Name = "lblDissolveCPgonsName"
      Me.lblDissolveCPgonsName.Size = New System.Drawing.Size(120, 14)
      Me.lblDissolveCPgonsName.TabIndex = 21
      '
      'Label3
      '
      Me.Label3.AutoSize = True
      Me.Label3.Location = New System.Drawing.Point(126, 51)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(56, 14)
      Me.Label3.TabIndex = 20
      Me.Label3.Text = "מס' פול':"
      '
      'tstTopology
      '
      Me.tstTopology.Dock = System.Windows.Forms.DockStyle.None
      Me.tstTopology.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.tstTopology.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
      Me.tstTopology.Location = New System.Drawing.Point(11, 10)
      Me.tstTopology.Name = "tstTopology"
      Me.tstTopology.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.tstTopology.Size = New System.Drawing.Size(1, 0)
      Me.tstTopology.TabIndex = 16
      '
      'txtDissolvePgonCount
      '
      Me.txtDissolvePgonCount.Location = New System.Drawing.Point(86, 48)
      Me.txtDissolvePgonCount.Name = "txtDissolvePgonCount"
      Me.txtDissolvePgonCount.ReadOnly = True
      Me.txtDissolvePgonCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtDissolvePgonCount.Size = New System.Drawing.Size(40, 22)
      Me.txtDissolvePgonCount.TabIndex = 15
      '
      'lblDissolveTopoExists
      '
      Me.lblDissolveTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblDissolveTopoExists.Location = New System.Drawing.Point(454, 48)
      Me.lblDissolveTopoExists.Name = "lblDissolveTopoExists"
      Me.lblDissolveTopoExists.Size = New System.Drawing.Size(16, 14)
      Me.lblDissolveTopoExists.TabIndex = 14
      '
      'lblDissolveTopologyCap
      '
      Me.lblDissolveTopologyCap.Location = New System.Drawing.Point(332, 48)
      Me.lblDissolveTopologyCap.Name = "lblDissolveTopologyCap"
      Me.lblDissolveTopologyCap.Size = New System.Drawing.Size(120, 14)
      Me.lblDissolveTopologyCap.TabIndex = 13
      Me.lblDissolveTopologyCap.Text = "טופולוגייה:"
      '
      'cmdClose
      '
      Me.cmdClose.Location = New System.Drawing.Point(401, 300)
      Me.cmdClose.Name = "cmdClose"
      Me.cmdClose.Size = New System.Drawing.Size(85, 23)
      Me.cmdClose.TabIndex = 17
      Me.cmdClose.Text = "סגירה"
      Me.cmdClose.UseVisualStyleBackColor = True
      '
      'dgvBlocks
      '
      Me.dgvBlocks.AllowUserToAddRows = False
      Me.dgvBlocks.AllowUserToDeleteRows = False
      Me.dgvBlocks.AllowUserToResizeRows = False
      Me.dgvBlocks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvBlocks.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxiParcelCount, Me.chkBlock, Me.cchPgonExists})
      Me.dgvBlocks.Location = New System.Drawing.Point(6, 302)
      Me.dgvBlocks.MultiSelect = False
      Me.dgvBlocks.Name = "dgvBlocks"
      Me.dgvBlocks.RowHeadersWidth = 23
      Me.dgvBlocks.Size = New System.Drawing.Size(389, 208)
      Me.dgvBlocks.TabIndex = 25
      '
      'ctxiParcelCount
      '
      Me.ctxiParcelCount.DataPropertyName = "ParcelCount"
      Me.ctxiParcelCount.HeaderText = "מס' חל'"
      Me.ctxiParcelCount.Name = "ctxiParcelCount"
      Me.ctxiParcelCount.ReadOnly = True
      Me.ctxiParcelCount.Width = 72
      '
      'chkBlock
      '
      Me.chkBlock.DataPropertyName = "BlockExists"
      Me.chkBlock.HeaderText = "בלוק"
      Me.chkBlock.Name = "chkBlock"
      Me.chkBlock.ReadOnly = True
      Me.chkBlock.Width = 48
      '
      'cchPgonExists
      '
      Me.cchPgonExists.DataPropertyName = "PgonExists"
      Me.cchPgonExists.FillWeight = 60.0!
      Me.cchPgonExists.HeaderText = "פוליגון"
      Me.cchPgonExists.Name = "cchPgonExists"
      Me.cchPgonExists.ReadOnly = True
      Me.cchPgonExists.Width = 48
      '
      'grbSourceTopo
      '
      Me.grbSourceTopo.Controls.Add(Me.chkSrcExteriorRingOnly)
      Me.grbSourceTopo.Controls.Add(Me.rdbTopoWA)
      Me.grbSourceTopo.Controls.Add(Me.rdbTopo)
      Me.grbSourceTopo.Controls.Add(Me.Label2)
      Me.grbSourceTopo.Controls.Add(Me.txtSourceWAPgonCount)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceTopoWAExists)
      Me.grbSourceTopo.Controls.Add(Me.Label6)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceTopoWAName)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceCPgonsName)
      Me.grbSourceTopo.Controls.Add(Me.cmdEraseSrcClosedPgons)
      Me.grbSourceTopo.Controls.Add(Me.cmdSrcToClosedPgons)
      Me.grbSourceTopo.Controls.Add(Me.Label7)
      Me.grbSourceTopo.Controls.Add(Me.txtSourcePLinesCount)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceCPgonsExist)
      Me.grbSourceTopo.Controls.Add(Me.Label9)
      Me.grbSourceTopo.Controls.Add(Me.Label11)
      Me.grbSourceTopo.Controls.Add(Me.ToolStrip1)
      Me.grbSourceTopo.Controls.Add(Me.txtSourcePgonCount)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceTopoExists)
      Me.grbSourceTopo.Controls.Add(Me.Label13)
      Me.grbSourceTopo.Controls.Add(Me.lblSourceTopoName)
      Me.grbSourceTopo.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.grbSourceTopo.Location = New System.Drawing.Point(6, 12)
      Me.grbSourceTopo.Name = "grbSourceTopo"
      Me.grbSourceTopo.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.grbSourceTopo.Size = New System.Drawing.Size(480, 140)
      Me.grbSourceTopo.TabIndex = 28
      Me.grbSourceTopo.TabStop = False
      Me.grbSourceTopo.Text = "חלקות"
      '
      'chkSrcExteriorRingOnly
      '
      Me.chkSrcExteriorRingOnly.AutoSize = True
      Me.chkSrcExteriorRingOnly.Checked = True
      Me.chkSrcExteriorRingOnly.CheckState = System.Windows.Forms.CheckState.Checked
      Me.chkSrcExteriorRingOnly.Location = New System.Drawing.Point(12, 108)
      Me.chkSrcExteriorRingOnly.Name = "chkSrcExteriorRingOnly"
      Me.chkSrcExteriorRingOnly.Size = New System.Drawing.Size(71, 18)
      Me.chkSrcExteriorRingOnly.TabIndex = 36
      Me.chkSrcExteriorRingOnly.Text = "בלי איים"
      Me.chkSrcExteriorRingOnly.UseVisualStyleBackColor = True
      '
      'rdbTopoWA
      '
      Me.rdbTopoWA.AutoSize = True
      Me.rdbTopoWA.Location = New System.Drawing.Point(52, 56)
      Me.rdbTopoWA.Name = "rdbTopoWA"
      Me.rdbTopoWA.Size = New System.Drawing.Size(14, 13)
      Me.rdbTopoWA.TabIndex = 35
      Me.rdbTopoWA.TabStop = True
      Me.rdbTopoWA.UseVisualStyleBackColor = True
      '
      'rdbTopo
      '
      Me.rdbTopo.AutoSize = True
      Me.rdbTopo.Location = New System.Drawing.Point(52, 28)
      Me.rdbTopo.Name = "rdbTopo"
      Me.rdbTopo.Size = New System.Drawing.Size(14, 13)
      Me.rdbTopo.TabIndex = 34
      Me.rdbTopo.TabStop = True
      Me.rdbTopo.UseVisualStyleBackColor = True
      '
      'Label2
      '
      Me.Label2.AutoSize = True
      Me.Label2.Location = New System.Drawing.Point(126, 52)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(56, 14)
      Me.Label2.TabIndex = 33
      Me.Label2.Text = "מס' פול':"
      '
      'txtSourceWAPgonCount
      '
      Me.txtSourceWAPgonCount.Location = New System.Drawing.Point(86, 52)
      Me.txtSourceWAPgonCount.Name = "txtSourceWAPgonCount"
      Me.txtSourceWAPgonCount.ReadOnly = True
      Me.txtSourceWAPgonCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtSourceWAPgonCount.Size = New System.Drawing.Size(40, 22)
      Me.txtSourceWAPgonCount.TabIndex = 32
      '
      'lblSourceTopoWAExists
      '
      Me.lblSourceTopoWAExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblSourceTopoWAExists.Location = New System.Drawing.Point(454, 52)
      Me.lblSourceTopoWAExists.Name = "lblSourceTopoWAExists"
      Me.lblSourceTopoWAExists.Size = New System.Drawing.Size(16, 14)
      Me.lblSourceTopoWAExists.TabIndex = 31
      '
      'Label6
      '
      Me.Label6.Location = New System.Drawing.Point(320, 52)
      Me.Label6.Name = "Label6"
      Me.Label6.Size = New System.Drawing.Size(132, 14)
      Me.Label6.TabIndex = 30
      Me.Label6.Text = "טופולוגיה ללא קשתות:"
      '
      'lblSourceTopoWAName
      '
      Me.lblSourceTopoWAName.Location = New System.Drawing.Point(200, 52)
      Me.lblSourceTopoWAName.Name = "lblSourceTopoWAName"
      Me.lblSourceTopoWAName.Size = New System.Drawing.Size(120, 14)
      Me.lblSourceTopoWAName.TabIndex = 29
      '
      'lblSourceCPgonsName
      '
      Me.lblSourceCPgonsName.Location = New System.Drawing.Point(200, 80)
      Me.lblSourceCPgonsName.Name = "lblSourceCPgonsName"
      Me.lblSourceCPgonsName.Size = New System.Drawing.Size(120, 14)
      Me.lblSourceCPgonsName.TabIndex = 28
      '
      'cmdEraseSrcClosedPgons
      '
      Me.cmdEraseSrcClosedPgons.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
      Me.cmdEraseSrcClosedPgons.Location = New System.Drawing.Point(12, 80)
      Me.cmdEraseSrcClosedPgons.Name = "cmdEraseSrcClosedPgons"
      Me.cmdEraseSrcClosedPgons.Size = New System.Drawing.Size(24, 22)
      Me.cmdEraseSrcClosedPgons.TabIndex = 27
      Me.cmdEraseSrcClosedPgons.UseVisualStyleBackColor = True
      '
      'cmdSrcToClosedPgons
      '
      Me.cmdSrcToClosedPgons.AutoSize = True
      Me.cmdSrcToClosedPgons.Image = Global.TopoUI.My.Resources.Resources.ToClosedPgons
      Me.cmdSrcToClosedPgons.Location = New System.Drawing.Point(42, 80)
      Me.cmdSrcToClosedPgons.Name = "cmdSrcToClosedPgons"
      Me.cmdSrcToClosedPgons.Size = New System.Drawing.Size(27, 22)
      Me.cmdSrcToClosedPgons.TabIndex = 26
      Me.cmdSrcToClosedPgons.UseVisualStyleBackColor = True
      '
      'Label7
      '
      Me.Label7.AutoSize = True
      Me.Label7.Location = New System.Drawing.Point(126, 80)
      Me.Label7.Name = "Label7"
      Me.Label7.Size = New System.Drawing.Size(56, 14)
      Me.Label7.TabIndex = 25
      Me.Label7.Text = "מס' פול':"
      '
      'txtSourcePLinesCount
      '
      Me.txtSourcePLinesCount.Location = New System.Drawing.Point(86, 80)
      Me.txtSourcePLinesCount.Name = "txtSourcePLinesCount"
      Me.txtSourcePLinesCount.ReadOnly = True
      Me.txtSourcePLinesCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtSourcePLinesCount.Size = New System.Drawing.Size(40, 22)
      Me.txtSourcePLinesCount.TabIndex = 24
      '
      'lblSourceCPgonsExist
      '
      Me.lblSourceCPgonsExist.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblSourceCPgonsExist.Location = New System.Drawing.Point(454, 80)
      Me.lblSourceCPgonsExist.Name = "lblSourceCPgonsExist"
      Me.lblSourceCPgonsExist.Size = New System.Drawing.Size(16, 14)
      Me.lblSourceCPgonsExist.TabIndex = 23
      Me.lblSourceCPgonsExist.Visible = False
      '
      'Label9
      '
      Me.Label9.Location = New System.Drawing.Point(332, 80)
      Me.Label9.Name = "Label9"
      Me.Label9.Size = New System.Drawing.Size(120, 14)
      Me.Label9.TabIndex = 22
      Me.Label9.Text = "שכבת פוליגונים סג':"
      '
      'Label11
      '
      Me.Label11.AutoSize = True
      Me.Label11.Location = New System.Drawing.Point(126, 24)
      Me.Label11.Name = "Label11"
      Me.Label11.Size = New System.Drawing.Size(56, 14)
      Me.Label11.TabIndex = 20
      Me.Label11.Text = "מס' פול':"
      '
      'ToolStrip1
      '
      Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
      Me.ToolStrip1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.ToolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow
      Me.ToolStrip1.Location = New System.Drawing.Point(11, 10)
      Me.ToolStrip1.Name = "ToolStrip1"
      Me.ToolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.ToolStrip1.Size = New System.Drawing.Size(1, 0)
      Me.ToolStrip1.TabIndex = 16
      '
      'txtSourcePgonCount
      '
      Me.txtSourcePgonCount.Location = New System.Drawing.Point(86, 24)
      Me.txtSourcePgonCount.Name = "txtSourcePgonCount"
      Me.txtSourcePgonCount.ReadOnly = True
      Me.txtSourcePgonCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtSourcePgonCount.Size = New System.Drawing.Size(40, 22)
      Me.txtSourcePgonCount.TabIndex = 15
      '
      'lblSourceTopoExists
      '
      Me.lblSourceTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblSourceTopoExists.Location = New System.Drawing.Point(454, 24)
      Me.lblSourceTopoExists.Name = "lblSourceTopoExists"
      Me.lblSourceTopoExists.Size = New System.Drawing.Size(16, 14)
      Me.lblSourceTopoExists.TabIndex = 14
      '
      'Label13
      '
      Me.Label13.Location = New System.Drawing.Point(332, 24)
      Me.Label13.Name = "Label13"
      Me.Label13.Size = New System.Drawing.Size(120, 14)
      Me.Label13.TabIndex = 13
      Me.Label13.Text = "טופולוגיה:"
      '
      'lblSourceTopoName
      '
      Me.lblSourceTopoName.Location = New System.Drawing.Point(200, 24)
      Me.lblSourceTopoName.Name = "lblSourceTopoName"
      Me.lblSourceTopoName.Size = New System.Drawing.Size(120, 14)
      Me.lblSourceTopoName.TabIndex = 10
      '
      'cmdUpdateData
      '
      Me.cmdUpdateData.Location = New System.Drawing.Point(401, 329)
      Me.cmdUpdateData.Name = "cmdUpdateData"
      Me.cmdUpdateData.Size = New System.Drawing.Size(85, 23)
      Me.cmdUpdateData.TabIndex = 29
      Me.cmdUpdateData.Text = "עדכון נתונים"
      Me.cmdUpdateData.UseVisualStyleBackColor = True
      '
      'cmdExportShape
      '
      Me.cmdExportShape.Location = New System.Drawing.Point(401, 450)
      Me.cmdExportShape.Name = "cmdExportShape"
      Me.cmdExportShape.Size = New System.Drawing.Size(85, 23)
      Me.cmdExportShape.TabIndex = 30
      Me.cmdExportShape.Text = "Shape"
      Me.cmdExportShape.UseVisualStyleBackColor = True
      '
      'cmdFillODTable
      '
      Me.cmdFillODTable.Location = New System.Drawing.Point(403, 421)
      Me.cmdFillODTable.Name = "cmdFillODTable"
      Me.cmdFillODTable.Size = New System.Drawing.Size(85, 23)
      Me.cmdFillODTable.TabIndex = 31
      Me.cmdFillODTable.Text = "מילוי  טבלה"
      Me.cmdFillODTable.UseVisualStyleBackColor = True
      '
      'cmdZoom
      '
      Me.cmdZoom.Location = New System.Drawing.Point(401, 358)
      Me.cmdZoom.Name = "cmdZoom"
      Me.cmdZoom.Size = New System.Drawing.Size(85, 23)
      Me.cmdZoom.TabIndex = 32
      Me.cmdZoom.Text = "Zoom"
      Me.cmdZoom.UseVisualStyleBackColor = True
      '
      'cmbODType
      '
      Me.cmbODType.FormattingEnabled = True
      Me.cmbODType.Items.AddRange(New Object() {"שכבות", "יעודים"})
      Me.cmbODType.Location = New System.Drawing.Point(401, 387)
      Me.cmbODType.Name = "cmbODType"
      Me.cmbODType.Size = New System.Drawing.Size(85, 22)
      Me.cmbODType.TabIndex = 33
      '
      'frmTopoToClosedPgons
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.AutoSize = True
      Me.ClientSize = New System.Drawing.Size(492, 522)
      Me.Controls.Add(Me.cmbODType)
      Me.Controls.Add(Me.cmdZoom)
      Me.Controls.Add(Me.cmdFillODTable)
      Me.Controls.Add(Me.cmdExportShape)
      Me.Controls.Add(Me.cmdUpdateData)
      Me.Controls.Add(Me.grbSourceTopo)
      Me.Controls.Add(Me.dgvBlocks)
      Me.Controls.Add(Me.cmdClose)
      Me.Controls.Add(Me.grbDissolveTopo)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Name = "frmTopoToClosedPgons"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.RightToLeftLayout = True
      Me.Text = "פוליגונים סגורים"
      Me.grbDissolveTopo.ResumeLayout(False)
      Me.grbDissolveTopo.PerformLayout()
      CType(Me.dgvBlocks, System.ComponentModel.ISupportInitialize).EndInit()
      Me.grbSourceTopo.ResumeLayout(False)
      Me.grbSourceTopo.PerformLayout()
      Me.ResumeLayout(False)

   End Sub
   Private WithEvents lblDissolveTopoName As System.Windows.Forms.Label
   Private WithEvents lblDissolveTopologyCap As System.Windows.Forms.Label
   Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
   Private WithEvents txtDissolvePgonCount As System.Windows.Forms.TextBox
   Private WithEvents lblDissolveTopoExists As System.Windows.Forms.Label
   Private WithEvents grbDissolveTopo As System.Windows.Forms.GroupBox
   Private WithEvents cmdClose As System.Windows.Forms.Button
   Private WithEvents Label3 As System.Windows.Forms.Label
   Private WithEvents dgvBlocks As System.Windows.Forms.DataGridView
   Private WithEvents cmdEraseDisClosedPgons As System.Windows.Forms.Button
   Private WithEvents cmdDisToClosedPgons As System.Windows.Forms.Button
   Private WithEvents Label1 As System.Windows.Forms.Label
   Private WithEvents txtDissolvePLinesCount As System.Windows.Forms.TextBox
   Private WithEvents lblDissolveCPgonsExist As System.Windows.Forms.Label
   Private WithEvents Label5 As System.Windows.Forms.Label
   Private WithEvents lblDissolveCPgonsName As System.Windows.Forms.Label
   Private WithEvents grbSourceTopo As System.Windows.Forms.GroupBox
   Private WithEvents cmdEraseSrcClosedPgons As System.Windows.Forms.Button
   Private WithEvents cmdSrcToClosedPgons As System.Windows.Forms.Button
   Private WithEvents Label7 As System.Windows.Forms.Label
   Private WithEvents txtSourcePLinesCount As System.Windows.Forms.TextBox
   Private WithEvents lblSourceCPgonsExist As System.Windows.Forms.Label
   Private WithEvents Label9 As System.Windows.Forms.Label
   Private WithEvents Label11 As System.Windows.Forms.Label
   Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
   Private WithEvents txtSourcePgonCount As System.Windows.Forms.TextBox
   Private WithEvents lblSourceTopoExists As System.Windows.Forms.Label
   Private WithEvents Label13 As System.Windows.Forms.Label
   Private WithEvents lblSourceTopoName As System.Windows.Forms.Label
   Private WithEvents lblSourceCPgonsName As System.Windows.Forms.Label
   Private WithEvents Label2 As System.Windows.Forms.Label
   Private WithEvents txtSourceWAPgonCount As System.Windows.Forms.TextBox
   Private WithEvents lblSourceTopoWAExists As System.Windows.Forms.Label
   Private WithEvents Label6 As System.Windows.Forms.Label
   Private WithEvents lblSourceTopoWAName As System.Windows.Forms.Label
   Private WithEvents chkSrcExteriorRingOnly As System.Windows.Forms.CheckBox
   Private WithEvents rdbTopoWA As System.Windows.Forms.RadioButton
   Private WithEvents rdbTopo As System.Windows.Forms.RadioButton
   Private WithEvents chkDisExteriorRingOnly As System.Windows.Forms.CheckBox
   Private WithEvents cmdUpdateData As System.Windows.Forms.Button
   Private WithEvents cmdExportShape As System.Windows.Forms.Button
   Private WithEvents cmdFillODTable As System.Windows.Forms.Button
   Private WithEvents sfdShapeFile As System.Windows.Forms.SaveFileDialog
   Private WithEvents cmdClearMarkBlocks As System.Windows.Forms.Button
   Private WithEvents cmdSetMarkBlocks As System.Windows.Forms.Button
   Private WithEvents ImageList1 As System.Windows.Forms.ImageList
   Private WithEvents cmdZoom As System.Windows.Forms.Button
   Private WithEvents cmbODType As System.Windows.Forms.ComboBox
   Friend WithEvents ctxiParcelCount As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents chkBlock As System.Windows.Forms.DataGridViewCheckBoxColumn
   Friend WithEvents cchPgonExists As System.Windows.Forms.DataGridViewCheckBoxColumn
End Class
