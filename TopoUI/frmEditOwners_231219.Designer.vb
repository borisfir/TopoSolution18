<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEditOwners_231219
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
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.txtBlockNo = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtBlockAddNo = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtParcelNo = New System.Windows.Forms.TextBox()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.ccbOwner = New System.Windows.Forms.DataGridViewComboBoxColumn()
		Me.ctxDividend = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDivisor = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPartPct = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchSum = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxOwner = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxRowr = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtSumDec = New System.Windows.Forms.TextBox()
		Me.cmdComplete = New System.Windows.Forms.Button()
		Me.txtSumSimple = New System.Windows.Forms.TextBox()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
		Me.cmdExit.Location = New System.Drawing.Point(398, 4)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(26, 26)
		Me.cmdExit.TabIndex = 13
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Image = Global.TopoUI.My.Resources.Resources.Cancel16
		Me.cmdCancel.Location = New System.Drawing.Point(368, 4)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(26, 26)
		Me.cmdCancel.TabIndex = 12
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Image = Global.TopoUI.My.Resources.Resources.OK16
		Me.cmdOK.Location = New System.Drawing.Point(338, 4)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(26, 26)
		Me.cmdOK.TabIndex = 11
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.Button1.Location = New System.Drawing.Point(406, 28)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(28, 30)
		Me.Button1.TabIndex = 14
		Me.Button1.UseVisualStyleBackColor = True
		Me.Button1.Visible = False
		'
		'txtBlockNo
		'
		Me.txtBlockNo.Location = New System.Drawing.Point(42, 4)
		Me.txtBlockNo.Name = "txtBlockNo"
		Me.txtBlockNo.Size = New System.Drawing.Size(48, 22)
		Me.txtBlockNo.TabIndex = 15
		Me.txtBlockNo.Text = "123456"
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(0, 6)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(42, 14)
		Me.Label1.TabIndex = 16
		Me.Label1.Text = "גוש:"
		'
		'txtBlockAddNo
		'
		Me.txtBlockAddNo.Location = New System.Drawing.Point(96, 4)
		Me.txtBlockAddNo.Name = "txtBlockAddNo"
		Me.txtBlockAddNo.Size = New System.Drawing.Size(24, 22)
		Me.txtBlockAddNo.TabIndex = 17
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(0, 34)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(42, 14)
		Me.Label2.TabIndex = 19
		Me.Label2.Text = "חלקה:"
		'
		'txtParcelNo
		'
		Me.txtParcelNo.Location = New System.Drawing.Point(42, 32)
		Me.txtParcelNo.Name = "txtParcelNo"
		Me.txtParcelNo.Size = New System.Drawing.Size(48, 22)
		Me.txtParcelNo.TabIndex = 18
		Me.txtParcelNo.Text = "123456"
		'
		'dgvMain
		'
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ccbOwner, Me.ctxDividend, Me.ctxDivisor, Me.ctxPartPct, Me.cchSum, Me.ctxOwner, Me.ctxRowr})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 59)
		Me.dgvMain.MultiSelect = False
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.Size = New System.Drawing.Size(474, 348)
		Me.dgvMain.TabIndex = 0
		'
		'ccbOwner
		'
		Me.ccbOwner.DataPropertyName = "OwnerID"
		Me.ccbOwner.HeaderText = "בעלים"
		Me.ccbOwner.Name = "ccbOwner"
		Me.ccbOwner.Width = 120
		'
		'ctxDividend
		'
		Me.ctxDividend.DataPropertyName = "Dividend"
		DataGridViewCellStyle3.Format = "N0"
		Me.ctxDividend.DefaultCellStyle = DataGridViewCellStyle3
		Me.ctxDividend.HeaderText = "מונה"
		Me.ctxDividend.Name = "ctxDividend"
		Me.ctxDividend.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDividend.Width = 64
		'
		'ctxDivisor
		'
		Me.ctxDivisor.DataPropertyName = "Divisor"
		Me.ctxDivisor.HeaderText = "מכנה"
		Me.ctxDivisor.Name = "ctxDivisor"
		Me.ctxDivisor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDivisor.Width = 64
		'
		'ctxPartPct
		'
		Me.ctxPartPct.DataPropertyName = "PartPct"
		DataGridViewCellStyle4.Format = "N4"
		Me.ctxPartPct.DefaultCellStyle = DataGridViewCellStyle4
		Me.ctxPartPct.HeaderText = "אחוז"
		Me.ctxPartPct.Name = "ctxPartPct"
		Me.ctxPartPct.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPartPct.Width = 80
		'
		'cchSum
		'
		Me.cchSum.DataPropertyName = "Sum"
		Me.cchSum.FalseValue = ""
		Me.cchSum.HeaderText = "סיכום"
		Me.cchSum.Name = "cchSum"
		Me.cchSum.TrueValue = ""
		Me.cchSum.Width = 42
		'
		'ctxOwner
		'
		Me.ctxOwner.DataPropertyName = "OwnerIndex"
		Me.ctxOwner.HeaderText = "O"
		Me.ctxOwner.Name = "ctxOwner"
		Me.ctxOwner.ReadOnly = True
		Me.ctxOwner.Width = 36
		'
		'ctxRowr
		'
		Me.ctxRowr.DataPropertyName = "RowIndex"
		Me.ctxRowr.HeaderText = "R"
		Me.ctxRowr.Name = "ctxRowr"
		Me.ctxRowr.ReadOnly = True
		Me.ctxRowr.Width = 36
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(136, 6)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(49, 14)
		Me.Label3.TabIndex = 21
		Me.Label3.Text = "סך הכל"
		'
		'txtSumDec
		'
		Me.txtSumDec.Location = New System.Drawing.Point(188, 4)
		Me.txtSumDec.Name = "txtSumDec"
		Me.txtSumDec.Size = New System.Drawing.Size(73, 22)
		Me.txtSumDec.TabIndex = 20
		'
		'cmdComplete
		'
		Me.cmdComplete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdComplete.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdComplete.Location = New System.Drawing.Point(264, 4)
		Me.cmdComplete.Name = "cmdComplete"
		Me.cmdComplete.Size = New System.Drawing.Size(50, 22)
		Me.cmdComplete.TabIndex = 22
		Me.cmdComplete.Text = "100%"
		Me.cmdComplete.UseVisualStyleBackColor = True
		'
		'txtSumSimple
		'
		Me.txtSumSimple.Location = New System.Drawing.Point(188, 32)
		Me.txtSumSimple.Name = "txtSumSimple"
		Me.txtSumSimple.Size = New System.Drawing.Size(105, 22)
		Me.txtSumSimple.TabIndex = 23
		'
		'frmEditOwners
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(474, 407)
		Me.Controls.Add(Me.txtSumSimple)
		Me.Controls.Add(Me.cmdComplete)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.txtSumDec)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtParcelNo)
		Me.Controls.Add(Me.txtBlockAddNo)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtBlockNo)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmEditOwners"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "בעלי חלקה"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmdExit As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdOK As Button
	Private WithEvents Button1 As Button
	Private WithEvents txtBlockNo As TextBox
	Private WithEvents Label1 As Label
	Private WithEvents txtBlockAddNo As TextBox
	Private WithEvents Label2 As Label
	Private WithEvents txtParcelNo As TextBox
	Private WithEvents dgvMain As DataGridView
	Private WithEvents Label3 As Label
	Private WithEvents txtSumDec As TextBox
	Private WithEvents cmdComplete As Button
	Friend WithEvents ccbOwner As DataGridViewComboBoxColumn
	Friend WithEvents ctxDividend As DataGridViewTextBoxColumn
	Friend WithEvents ctxDivisor As DataGridViewTextBoxColumn
	Friend WithEvents ctxPartPct As DataGridViewTextBoxColumn
	Friend WithEvents cchSum As DataGridViewCheckBoxColumn
	Friend WithEvents ctxOwner As DataGridViewTextBoxColumn
	Friend WithEvents ctxRowr As DataGridViewTextBoxColumn
	Private WithEvents txtSumSimple As TextBox
End Class
