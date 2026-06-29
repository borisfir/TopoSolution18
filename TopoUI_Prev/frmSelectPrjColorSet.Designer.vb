<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectPrjColorSet
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
		Me.lblProjectCode = New System.Windows.Forms.Label()
		Me.txtProjectCode = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.cmbDetail = New System.Windows.Forms.ComboBox()
		Me.cmbTheme = New System.Windows.Forms.ComboBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'lblProjectCode
		'
		Me.lblProjectCode.Location = New System.Drawing.Point(180, 12)
		Me.lblProjectCode.Name = "lblProjectCode"
		Me.lblProjectCode.Size = New System.Drawing.Size(60, 22)
		Me.lblProjectCode.TabIndex = 0
		Me.lblProjectCode.Text = "פרויקט"
		'
		'txtProjectCode
		'
		Me.txtProjectCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
		Me.txtProjectCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
		Me.txtProjectCode.Location = New System.Drawing.Point(20, 12)
		Me.txtProjectCode.Name = "txtProjectCode"
		Me.txtProjectCode.Size = New System.Drawing.Size(102, 22)
		Me.txtProjectCode.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(180, 48)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(60, 22)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "גרסה"
		'
		'cmbDetail
		'
		Me.cmbDetail.DisplayMember = "DetailName"
		Me.cmbDetail.FormattingEnabled = True
		Me.cmbDetail.Location = New System.Drawing.Point(20, 48)
		Me.cmbDetail.Name = "cmbDetail"
		Me.cmbDetail.Size = New System.Drawing.Size(102, 22)
		Me.cmbDetail.TabIndex = 3
		Me.cmbDetail.ValueMember = "Detail"
		'
		'cmbTheme
		'
		Me.cmbTheme.DisplayMember = "MapThemeName"
		Me.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cmbTheme.FormattingEnabled = True
		Me.cmbTheme.Location = New System.Drawing.Point(20, 84)
		Me.cmbTheme.Name = "cmbTheme"
		Me.cmbTheme.Size = New System.Drawing.Size(132, 22)
		Me.cmbTheme.TabIndex = 5
		Me.cmbTheme.ValueMember = "MapThemeID"
		'
		'Label2
		'
		Me.Label2.Location = New System.Drawing.Point(180, 84)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(60, 22)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "נושא"
		'
		'cmdOK
		'
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Location = New System.Drawing.Point(37, 152)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(75, 23)
		Me.cmdOK.TabIndex = 9
		Me.cmdOK.Text = "OK"
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Location = New System.Drawing.Point(162, 152)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
		Me.cmdCancel.TabIndex = 10
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'frmSelectPrjColorSet
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(272, 208)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.cmbTheme)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.cmbDetail)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtProjectCode)
		Me.Controls.Add(Me.lblProjectCode)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Margin = New System.Windows.Forms.Padding(4)
		Me.Name = "frmSelectPrjColorSet"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "בחר"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents lblProjectCode As System.Windows.Forms.Label
	Private WithEvents txtProjectCode As System.Windows.Forms.TextBox
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Private WithEvents cmbDetail As System.Windows.Forms.ComboBox
	Private WithEvents cmbTheme As System.Windows.Forms.ComboBox
End Class
