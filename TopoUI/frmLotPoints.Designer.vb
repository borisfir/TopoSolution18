<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLotPoints
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
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.cmdMarkPseudoVertices = New System.Windows.Forms.Button()
		Me.cmdDeleteTopo = New System.Windows.Forms.Button()
		Me.CreateTopo = New System.Windows.Forms.Button()
		Me.cmdInsertBlocks = New System.Windows.Forms.Button()
		Me.cmdNumber = New System.Windows.Forms.Button()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.ctxBlockName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLayer = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cmdZoom = New System.Windows.Forms.Button()
		Me.cmdUpdateAll = New System.Windows.Forms.Button()
		Me.cmdUpdateCurrent = New System.Windows.Forms.Button()
		Me.txtNumberFrom = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.chkReplace = New System.Windows.Forms.CheckBox()
		Me.cmdRefresh = New System.Windows.Forms.Button()
		Me.txtNumberMax = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtRowNumber = New System.Windows.Forms.TextBox()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(838, -2)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(23, 25)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		Me.Button1.Visible = False
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(838, 29)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(24, 25)
		Me.Button2.TabIndex = 1
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		Me.Button2.Visible = False
		'
		'cmdMarkPseudoVertices
		'
		Me.cmdMarkPseudoVertices.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdMarkPseudoVertices.Location = New System.Drawing.Point(12, 4)
		Me.cmdMarkPseudoVertices.Name = "cmdMarkPseudoVertices"
		Me.cmdMarkPseudoVertices.Size = New System.Drawing.Size(103, 25)
		Me.cmdMarkPseudoVertices.TabIndex = 2
		Me.cmdMarkPseudoVertices.Text = "PseudoVertices"
		Me.cmdMarkPseudoVertices.UseVisualStyleBackColor = True
		'
		'cmdDeleteTopo
		'
		Me.cmdDeleteTopo.Location = New System.Drawing.Point(133, 4)
		Me.cmdDeleteTopo.Name = "cmdDeleteTopo"
		Me.cmdDeleteTopo.Size = New System.Drawing.Size(81, 25)
		Me.cmdDeleteTopo.TabIndex = 3
		Me.cmdDeleteTopo.Text = "DeleteTopo"
		Me.cmdDeleteTopo.UseVisualStyleBackColor = True
		'
		'CreateTopo
		'
		Me.CreateTopo.Location = New System.Drawing.Point(220, 4)
		Me.CreateTopo.Name = "CreateTopo"
		Me.CreateTopo.Size = New System.Drawing.Size(89, 25)
		Me.CreateTopo.TabIndex = 4
		Me.CreateTopo.Text = "Create Topo"
		Me.CreateTopo.UseVisualStyleBackColor = True
		'
		'cmdInsertBlocks
		'
		Me.cmdInsertBlocks.Location = New System.Drawing.Point(317, 4)
		Me.cmdInsertBlocks.Name = "cmdInsertBlocks"
		Me.cmdInsertBlocks.Size = New System.Drawing.Size(89, 25)
		Me.cmdInsertBlocks.TabIndex = 5
		Me.cmdInsertBlocks.Text = "Insert Blocks"
		Me.cmdInsertBlocks.UseVisualStyleBackColor = True
		'
		'cmdNumber
		'
		Me.cmdNumber.Location = New System.Drawing.Point(731, 2)
		Me.cmdNumber.Name = "cmdNumber"
		Me.cmdNumber.Size = New System.Drawing.Size(51, 25)
		Me.cmdNumber.TabIndex = 6
		Me.cmdNumber.Text = "מספור"
		Me.cmdNumber.UseVisualStyleBackColor = True
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
		Me.dgvMain.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxBlockName, Me.ctxLayer})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 62)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.Size = New System.Drawing.Size(861, 423)
		Me.dgvMain.TabIndex = 7
		'
		'ctxBlockName
		'
		Me.ctxBlockName.DataPropertyName = "BlockName"
		Me.ctxBlockName.HeaderText = "שם בלוק"
		Me.ctxBlockName.Name = "ctxBlockName"
		Me.ctxBlockName.ReadOnly = True
		Me.ctxBlockName.Width = 80
		'
		'ctxLayer
		'
		Me.ctxLayer.DataPropertyName = "Layer"
		Me.ctxLayer.HeaderText = "שכבה"
		Me.ctxLayer.Name = "ctxLayer"
		Me.ctxLayer.ReadOnly = True
		Me.ctxLayer.Width = 80
		'
		'cmdZoom
		'
		Me.cmdZoom.AutoSize = True
		Me.cmdZoom.Location = New System.Drawing.Point(593, 4)
		Me.cmdZoom.Name = "cmdZoom"
		Me.cmdZoom.Size = New System.Drawing.Size(48, 24)
		Me.cmdZoom.TabIndex = 8
		Me.cmdZoom.Text = "Zoom"
		Me.cmdZoom.UseVisualStyleBackColor = True
		'
		'cmdUpdateAll
		'
		Me.cmdUpdateAll.AutoSize = True
		Me.cmdUpdateAll.Location = New System.Drawing.Point(526, 4)
		Me.cmdUpdateAll.Name = "cmdUpdateAll"
		Me.cmdUpdateAll.Size = New System.Drawing.Size(61, 24)
		Me.cmdUpdateAll.TabIndex = 10
		Me.cmdUpdateAll.Text = "Save All"
		Me.cmdUpdateAll.UseVisualStyleBackColor = True
		'
		'cmdUpdateCurrent
		'
		Me.cmdUpdateCurrent.AutoSize = True
		Me.cmdUpdateCurrent.Location = New System.Drawing.Point(451, 4)
		Me.cmdUpdateCurrent.Name = "cmdUpdateCurrent"
		Me.cmdUpdateCurrent.Size = New System.Drawing.Size(69, 24)
		Me.cmdUpdateCurrent.TabIndex = 9
		Me.cmdUpdateCurrent.Text = "Save Rec"
		Me.cmdUpdateCurrent.UseVisualStyleBackColor = True
		'
		'txtNumberFrom
		'
		Me.txtNumberFrom.Location = New System.Drawing.Point(802, 2)
		Me.txtNumberFrom.Name = "txtNumberFrom"
		Me.txtNumberFrom.Size = New System.Drawing.Size(36, 22)
		Me.txtNumberFrom.TabIndex = 11
		Me.txtNumberFrom.Text = "1"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(783, 7)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(19, 14)
		Me.Label1.TabIndex = 12
		Me.Label1.Text = "מ-"
		'
		'chkReplace
		'
		Me.chkReplace.AutoSize = True
		Me.chkReplace.Location = New System.Drawing.Point(781, 38)
		Me.chkReplace.Name = "chkReplace"
		Me.chkReplace.Size = New System.Drawing.Size(57, 18)
		Me.chkReplace.TabIndex = 13
		Me.chkReplace.Text = "החלף"
		Me.chkReplace.UseVisualStyleBackColor = True
		'
		'cmdRefresh
		'
		Me.cmdRefresh.Image = Global.TopoUI.My.Resources.Resources.RefreshC
		Me.cmdRefresh.Location = New System.Drawing.Point(647, 6)
		Me.cmdRefresh.Name = "cmdRefresh"
		Me.cmdRefresh.Size = New System.Drawing.Size(36, 23)
		Me.cmdRefresh.TabIndex = 14
		Me.cmdRefresh.UseVisualStyleBackColor = True
		'
		'txtNumberMax
		'
		Me.txtNumberMax.Location = New System.Drawing.Point(739, 36)
		Me.txtNumberMax.Name = "txtNumberMax"
		Me.txtNumberMax.Size = New System.Drawing.Size(36, 22)
		Me.txtNumberMax.TabIndex = 15
		Me.txtNumberMax.Text = "0"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(664, 40)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(70, 14)
		Me.Label2.TabIndex = 16
		Me.Label2.Text = "מספר מרבי"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(520, 40)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(76, 14)
		Me.Label3.TabIndex = 18
		Me.Label3.Text = "מספר שורות"
		'
		'txtRowNumber
		'
		Me.txtRowNumber.Location = New System.Drawing.Point(598, 36)
		Me.txtRowNumber.Name = "txtRowNumber"
		Me.txtRowNumber.Size = New System.Drawing.Size(36, 22)
		Me.txtRowNumber.TabIndex = 17
		Me.txtRowNumber.Text = "0"
		'
		'frmLotPoints
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(861, 485)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.txtRowNumber)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtNumberMax)
		Me.Controls.Add(Me.cmdRefresh)
		Me.Controls.Add(Me.chkReplace)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtNumberFrom)
		Me.Controls.Add(Me.cmdUpdateAll)
		Me.Controls.Add(Me.cmdUpdateCurrent)
		Me.Controls.Add(Me.cmdZoom)
		Me.Controls.Add(Me.dgvMain)
		Me.Controls.Add(Me.cmdNumber)
		Me.Controls.Add(Me.cmdInsertBlocks)
		Me.Controls.Add(Me.CreateTopo)
		Me.Controls.Add(Me.cmdDeleteTopo)
		Me.Controls.Add(Me.cmdMarkPseudoVertices)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmLotPoints"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "גבולות תכנון - נקודות"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents Button1 As Button
	Friend WithEvents Button2 As Button
	Friend WithEvents cmdMarkPseudoVertices As Button
	Private WithEvents cmdInsertBlocks As Button
	Private WithEvents CreateTopo As Button
	Private WithEvents cmdNumber As Button
	Private WithEvents cmdDeleteTopo As Button
	Friend WithEvents dgvMain As DataGridView
	Private WithEvents cmdZoom As Button
	Private WithEvents cmdUpdateAll As Button
	Private WithEvents cmdUpdateCurrent As Button
	Private WithEvents txtNumberFrom As TextBox
	Private WithEvents chkReplace As CheckBox
	Private WithEvents cmdRefresh As Button
	Private WithEvents txtNumberMax As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents ctxBlockName As DataGridViewTextBoxColumn
	Friend WithEvents ctxLayer As DataGridViewTextBoxColumn
	Private WithEvents Label3 As Label
	Private WithEvents txtRowNumber As TextBox
	Private WithEvents Label1 As Label
End Class
