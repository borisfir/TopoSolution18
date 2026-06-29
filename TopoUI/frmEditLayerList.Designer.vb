Namespace Expro
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class frmEditLayerList
		Inherits System.Windows.Forms.Form

		'Form overrides dispose to clean up the component list.
		<System.Diagnostics.DebuggerNonUserCode()>
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
		<System.Diagnostics.DebuggerStepThrough()>
		Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditLayerList))
			Me.pnlTop = New System.Windows.Forms.Panel()
			Me.chkFilter = New System.Windows.Forms.CheckBox()
			Me.cmdAddLayers = New System.Windows.Forms.Button()
			Me.cmdRefresh = New System.Windows.Forms.Button()
			Me.cmdExit = New System.Windows.Forms.Button()
			Me.Label1 = New System.Windows.Forms.Label()
			Me.cmdCancel = New System.Windows.Forms.Button()
			Me.cmdOK = New System.Windows.Forms.Button()
			Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
			Me.dgvDBLayers = New System.Windows.Forms.DataGridView()
			Me.ctxLayer = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxDBDescription = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbDBGeometricType = New System.Windows.Forms.DataGridViewComboBoxColumn()
			Me.cchDBIsProper = New System.Windows.Forms.DataGridViewCheckBoxColumn()
			Me.ctxDBNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.UpLayer = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ID = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.dgvDWGLayers = New System.Windows.Forms.DataGridView()
			Me.ctxDWGLayer = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxDWGDescription = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbDWGGeometricType = New System.Windows.Forms.DataGridViewComboBoxColumn()
			Me.cchDWGIsProper = New System.Windows.Forms.DataGridViewCheckBoxColumn()
			Me.ctxDWGNumber = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.Panel1 = New System.Windows.Forms.Panel()
			Me.Label2 = New System.Windows.Forms.Label()
			Me.pnlTop.SuspendLayout()
			CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SplitContainer1.Panel1.SuspendLayout()
			Me.SplitContainer1.Panel2.SuspendLayout()
			Me.SplitContainer1.SuspendLayout()
			CType(Me.dgvDBLayers, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.dgvDWGLayers, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.Panel1.SuspendLayout()
			Me.SuspendLayout()
			'
			'pnlTop
			'
			Me.pnlTop.Controls.Add(Me.chkFilter)
			Me.pnlTop.Controls.Add(Me.cmdAddLayers)
			Me.pnlTop.Controls.Add(Me.cmdRefresh)
			Me.pnlTop.Controls.Add(Me.cmdExit)
			Me.pnlTop.Controls.Add(Me.Label1)
			Me.pnlTop.Controls.Add(Me.cmdCancel)
			Me.pnlTop.Controls.Add(Me.cmdOK)
			Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
			Me.pnlTop.Location = New System.Drawing.Point(0, 0)
			Me.pnlTop.Name = "pnlTop"
			Me.pnlTop.Size = New System.Drawing.Size(568, 33)
			Me.pnlTop.TabIndex = 0
			'
			'chkFilter
			'
			Me.chkFilter.AutoSize = True
			Me.chkFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.chkFilter.Location = New System.Drawing.Point(160, 0)
			Me.chkFilter.Name = "chkFilter"
			Me.chkFilter.Size = New System.Drawing.Size(76, 35)
			Me.chkFilter.TabIndex = 19
			Me.chkFilter.Text = "רק אלה" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "שבשרטוט"
			Me.chkFilter.UseCompatibleTextRendering = True
			Me.chkFilter.UseVisualStyleBackColor = True
			'
			'cmdAddLayers
			'
			Me.cmdAddLayers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdAddLayers.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.cmdAddLayers.Image = Global.TopoUI.My.Resources.Resources.Plus16Tr
			Me.cmdAddLayers.Location = New System.Drawing.Point(127, 4)
			Me.cmdAddLayers.Name = "cmdAddLayers"
			Me.cmdAddLayers.Size = New System.Drawing.Size(26, 26)
			Me.cmdAddLayers.TabIndex = 18
			Me.cmdAddLayers.UseVisualStyleBackColor = True
			'
			'cmdRefresh
			'
			Me.cmdRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdRefresh.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.cmdRefresh.Image = Global.TopoUI.My.Resources.Resources.Invert12
			Me.cmdRefresh.Location = New System.Drawing.Point(95, 4)
			Me.cmdRefresh.Name = "cmdRefresh"
			Me.cmdRefresh.Size = New System.Drawing.Size(26, 26)
			Me.cmdRefresh.TabIndex = 17
			Me.cmdRefresh.UseVisualStyleBackColor = True
			'
			'cmdExit
			'
			Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
			Me.cmdExit.Location = New System.Drawing.Point(3, 3)
			Me.cmdExit.Name = "cmdExit"
			Me.cmdExit.Size = New System.Drawing.Size(26, 26)
			Me.cmdExit.TabIndex = 16
			Me.cmdExit.UseVisualStyleBackColor = True
			'
			'Label1
			'
			Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.Label1.AutoSize = True
			Me.Label1.Location = New System.Drawing.Point(367, 15)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(153, 14)
			Me.Label1.TabIndex = 0
			Me.Label1.Text = "רשימת השכבות למחוברים"
			'
			'cmdCancel
			'
			Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.cmdCancel.Image = CType(resources.GetObject("cmdCancel.Image"), System.Drawing.Image)
			Me.cmdCancel.Location = New System.Drawing.Point(31, 3)
			Me.cmdCancel.Name = "cmdCancel"
			Me.cmdCancel.Size = New System.Drawing.Size(26, 26)
			Me.cmdCancel.TabIndex = 15
			Me.cmdCancel.UseVisualStyleBackColor = True
			'
			'cmdOK
			'
			Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.cmdOK.Image = CType(resources.GetObject("cmdOK.Image"), System.Drawing.Image)
			Me.cmdOK.Location = New System.Drawing.Point(63, 4)
			Me.cmdOK.Name = "cmdOK"
			Me.cmdOK.Size = New System.Drawing.Size(26, 26)
			Me.cmdOK.TabIndex = 14
			Me.cmdOK.UseVisualStyleBackColor = True
			'
			'SplitContainer1
			'
			Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
			Me.SplitContainer1.Location = New System.Drawing.Point(0, 33)
			Me.SplitContainer1.Name = "SplitContainer1"
			Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
			'
			'SplitContainer1.Panel1
			'
			Me.SplitContainer1.Panel1.Controls.Add(Me.dgvDBLayers)
			Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			'
			'SplitContainer1.Panel2
			'
			Me.SplitContainer1.Panel2.Controls.Add(Me.dgvDWGLayers)
			Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
			Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.SplitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.SplitContainer1.Size = New System.Drawing.Size(568, 452)
			Me.SplitContainer1.SplitterDistance = 200
			Me.SplitContainer1.TabIndex = 1
			'
			'dgvDBLayers
			'
			Me.dgvDBLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dgvDBLayers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxLayer, Me.ctxDBDescription, Me.ccbDBGeometricType, Me.cchDBIsProper, Me.ctxDBNumber, Me.UpLayer, Me.ID})
			Me.dgvDBLayers.Dock = System.Windows.Forms.DockStyle.Fill
			Me.dgvDBLayers.Location = New System.Drawing.Point(0, 0)
			Me.dgvDBLayers.MultiSelect = False
			Me.dgvDBLayers.Name = "dgvDBLayers"
			Me.dgvDBLayers.RowHeadersWidth = 23
			Me.dgvDBLayers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.dgvDBLayers.Size = New System.Drawing.Size(568, 200)
			Me.dgvDBLayers.StandardTab = True
			Me.dgvDBLayers.TabIndex = 0
			'
			'ctxLayer
			'
			Me.ctxLayer.DataPropertyName = "Layer"
			Me.ctxLayer.HeaderText = "שם שכבה"
			Me.ctxLayer.Name = "ctxLayer"
			Me.ctxLayer.Width = 140
			'
			'ctxDBDescription
			'
			Me.ctxDBDescription.DataPropertyName = "Description"
			Me.ctxDBDescription.HeaderText = "תיאור"
			Me.ctxDBDescription.Name = "ctxDBDescription"
			Me.ctxDBDescription.Width = 140
			'
			'ccbDBGeometricType
			'
			Me.ccbDBGeometricType.DataPropertyName = "GeometricType"
			Me.ccbDBGeometricType.HeaderText = "סוג גיאומ'"
			Me.ccbDBGeometricType.Name = "ccbDBGeometricType"
			Me.ccbDBGeometricType.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
			Me.ccbDBGeometricType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
			Me.ccbDBGeometricType.Width = 72
			'
			'cchDBIsProper
			'
			Me.cchDBIsProper.DataPropertyName = "IsProper"
			Me.cchDBIsProper.HeaderText = "מחוב'?"
			Me.cchDBIsProper.Name = "cchDBIsProper"
			Me.cchDBIsProper.Width = 40
			'
			'ctxDBNumber
			'
			Me.ctxDBNumber.DataPropertyName = "Number"
			Me.ctxDBNumber.HeaderText = "מס' בשרטוט"
			Me.ctxDBNumber.Name = "ctxDBNumber"
			Me.ctxDBNumber.Width = 60
			'
			'UpLayer
			'
			Me.UpLayer.DataPropertyName = "UpLayer"
			Me.UpLayer.HeaderText = "Up"
			Me.UpLayer.Name = "UpLayer"
			Me.UpLayer.Width = 64
			'
			'ID
			'
			Me.ID.DataPropertyName = "ID"
			Me.ID.HeaderText = "ID"
			Me.ID.Name = "ID"
			Me.ID.Width = 48
			'
			'dgvDWGLayers
			'
			Me.dgvDWGLayers.AllowUserToAddRows = False
			Me.dgvDWGLayers.AllowUserToDeleteRows = False
			Me.dgvDWGLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dgvDWGLayers.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxDWGLayer, Me.ctxDWGDescription, Me.ccbDWGGeometricType, Me.cchDWGIsProper, Me.ctxDWGNumber})
			Me.dgvDWGLayers.Dock = System.Windows.Forms.DockStyle.Fill
			Me.dgvDWGLayers.Location = New System.Drawing.Point(0, 24)
			Me.dgvDWGLayers.Name = "dgvDWGLayers"
			Me.dgvDWGLayers.RowHeadersWidth = 23
			Me.dgvDWGLayers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.dgvDWGLayers.Size = New System.Drawing.Size(568, 224)
			Me.dgvDWGLayers.TabIndex = 0
			'
			'ctxDWGLayer
			'
			Me.ctxDWGLayer.DataPropertyName = "Layer"
			Me.ctxDWGLayer.HeaderText = "שם שכבה"
			Me.ctxDWGLayer.Name = "ctxDWGLayer"
			Me.ctxDWGLayer.Width = 140
			'
			'ctxDWGDescription
			'
			Me.ctxDWGDescription.DataPropertyName = "Description"
			Me.ctxDWGDescription.HeaderText = "תיאור"
			Me.ctxDWGDescription.Name = "ctxDWGDescription"
			Me.ctxDWGDescription.Width = 140
			'
			'ccbDWGGeometricType
			'
			Me.ccbDWGGeometricType.DataPropertyName = "GeometricType"
			Me.ccbDWGGeometricType.HeaderText = "סוג גיאומ'"
			Me.ccbDWGGeometricType.Name = "ccbDWGGeometricType"
			Me.ccbDWGGeometricType.Width = 80
			'
			'cchDWGIsProper
			'
			Me.cchDWGIsProper.DataPropertyName = "IsProper"
			Me.cchDWGIsProper.HeaderText = "מחוב'?"
			Me.cchDWGIsProper.Name = "cchDWGIsProper"
			Me.cchDWGIsProper.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
			Me.cchDWGIsProper.Width = 40
			'
			'ctxDWGNumber
			'
			Me.ctxDWGNumber.DataPropertyName = "Number"
			Me.ctxDWGNumber.HeaderText = "מס' בשרטוט"
			Me.ctxDWGNumber.Name = "ctxDWGNumber"
			Me.ctxDWGNumber.Width = 60
			'
			'Panel1
			'
			Me.Panel1.Controls.Add(Me.Label2)
			Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
			Me.Panel1.Location = New System.Drawing.Point(0, 0)
			Me.Panel1.Name = "Panel1"
			Me.Panel1.Size = New System.Drawing.Size(568, 24)
			Me.Panel1.TabIndex = 1
			'
			'Label2
			'
			Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.Label2.AutoSize = True
			Me.Label2.Location = New System.Drawing.Point(277, 8)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(128, 14)
			Me.Label2.TabIndex = 1
			Me.Label2.Text = "שכבות שרטוט נוספות"
			'
			'frmEditLayerList
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
			Me.ClientSize = New System.Drawing.Size(568, 485)
			Me.Controls.Add(Me.SplitContainer1)
			Me.Controls.Add(Me.pnlTop)
			Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.Name = "frmEditLayerList"
			Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.RightToLeftLayout = True
			Me.Text = "עריכת רשימת השכבות"
			Me.pnlTop.ResumeLayout(False)
			Me.pnlTop.PerformLayout()
			Me.SplitContainer1.Panel1.ResumeLayout(False)
			Me.SplitContainer1.Panel2.ResumeLayout(False)
			CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
			Me.SplitContainer1.ResumeLayout(False)
			CType(Me.dgvDBLayers, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.dgvDWGLayers, System.ComponentModel.ISupportInitialize).EndInit()
			Me.Panel1.ResumeLayout(False)
			Me.Panel1.PerformLayout()
			Me.ResumeLayout(False)

		End Sub

		Private WithEvents pnlTop As Panel
		Private WithEvents SplitContainer1 As SplitContainer
		Private WithEvents dgvDBLayers As DataGridView
		Private WithEvents Panel1 As Panel
		Private WithEvents dgvDWGLayers As DataGridView
		Private WithEvents Label1 As Label
		Private WithEvents cmdRefresh As Button
		Private WithEvents cmdExit As Button
		Private WithEvents cmdCancel As Button
		Private WithEvents cmdOK As Button
		Private WithEvents cmdAddLayers As Button
		Private WithEvents Label2 As Label
		Friend WithEvents ctxDWGLayer As DataGridViewTextBoxColumn
		Friend WithEvents ctxDWGDescription As DataGridViewTextBoxColumn
		Friend WithEvents ccbDWGGeometricType As DataGridViewComboBoxColumn
		Friend WithEvents cchDWGIsProper As DataGridViewCheckBoxColumn
		Friend WithEvents ctxDWGNumber As DataGridViewTextBoxColumn
		Friend WithEvents ctxLayer As DataGridViewTextBoxColumn
		Friend WithEvents ctxDBDescription As DataGridViewTextBoxColumn
		Friend WithEvents ccbDBGeometricType As DataGridViewComboBoxColumn
		Friend WithEvents cchDBIsProper As DataGridViewCheckBoxColumn
		Friend WithEvents ctxDBNumber As DataGridViewTextBoxColumn
		Friend WithEvents UpLayer As DataGridViewTextBoxColumn
		Friend WithEvents ID As DataGridViewTextBoxColumn
		Private WithEvents chkFilter As CheckBox
	End Class
End Namespace