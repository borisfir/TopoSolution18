<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditDetails
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
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.colDetailNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.colDescript = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.Source = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.CreatorName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.colDateCreated = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.txtProjectCode = New System.Windows.Forms.TextBox()
		Me.txtProjectName = New System.Windows.Forms.TextBox()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colDetailNo, Me.colDescript, Me.Source, Me.CreatorName, Me.colDateCreated})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 74)
		Me.dgvMain.MultiSelect = False
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.Size = New System.Drawing.Size(523, 235)
		Me.dgvMain.TabIndex = 0
		'
		'colDetailNo
		'
		Me.colDetailNo.DataPropertyName = "Detail"
		Me.colDetailNo.HeaderText = "מס'"
		Me.colDetailNo.Name = "colDetailNo"
		Me.colDetailNo.ReadOnly = True
		Me.colDetailNo.Width = 36
		'
		'colDescript
		'
		Me.colDescript.DataPropertyName = "DetailName"
		Me.colDescript.HeaderText = "שם"
		Me.colDescript.Name = "colDescript"
		Me.colDescript.Width = 200
		'
		'Source
		'
		Me.Source.HeaderText = "מקור"
		Me.Source.Name = "Source"
		Me.Source.Width = 40
		'
		'CreatorName
		'
		Me.CreatorName.DataPropertyName = "CreatorName"
		Me.CreatorName.HeaderText = "משתמש"
		Me.CreatorName.Name = "CreatorName"
		Me.CreatorName.ReadOnly = True
		'
		'colDateCreated
		'
		Me.colDateCreated.DataPropertyName = "DateCreated"
		DataGridViewCellStyle3.Format = "dd/MM/yy"
		DataGridViewCellStyle3.NullValue = Nothing
		Me.colDateCreated.DefaultCellStyle = DataGridViewCellStyle3
		Me.colDateCreated.HeaderText = "תאריך"
		Me.colDateCreated.Name = "colDateCreated"
		Me.colDateCreated.ReadOnly = True
		Me.colDateCreated.Width = 64
		'
		'txtProjectCode
		'
		Me.txtProjectCode.Location = New System.Drawing.Point(440, 8)
		Me.txtProjectCode.Name = "txtProjectCode"
		Me.txtProjectCode.ReadOnly = True
		Me.txtProjectCode.Size = New System.Drawing.Size(72, 22)
		Me.txtProjectCode.TabIndex = 1
		'
		'txtProjectName
		'
		Me.txtProjectName.Location = New System.Drawing.Point(199, 42)
		Me.txtProjectName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
		Me.txtProjectName.Name = "txtProjectName"
		Me.txtProjectName.ReadOnly = True
		Me.txtProjectName.Size = New System.Drawing.Size(313, 22)
		Me.txtProjectName.TabIndex = 2
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Image = Global.TopoUI.My.Resources.Resources.Cancel16
		Me.cmdCancel.Location = New System.Drawing.Point(48, 8)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(28, 28)
		Me.cmdCancel.TabIndex = 4
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Image = Global.TopoUI.My.Resources.Resources.OK16
		Me.cmdOK.Location = New System.Drawing.Point(12, 8)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(28, 28)
		Me.cmdOK.TabIndex = 3
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'frmEditDetails
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(523, 309)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.txtProjectName)
		Me.Controls.Add(Me.txtProjectCode)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmEditDetails"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "פרויקט"
		Me.TopMost = True
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents txtProjectCode As System.Windows.Forms.TextBox
	Private WithEvents dgvMain As System.Windows.Forms.DataGridView
	Private WithEvents txtProjectName As System.Windows.Forms.TextBox
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Friend WithEvents colDetailNo As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents colDescript As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents Source As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents CreatorName As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents colDateCreated As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
