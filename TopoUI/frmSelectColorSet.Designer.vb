<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectColorSet
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
		Me.rdbStandard = New System.Windows.Forms.RadioButton()
		Me.rdbAllLanduses = New System.Windows.Forms.RadioButton()
		Me.rdbProject = New System.Windows.Forms.RadioButton()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'lblProjectCode
		'
		Me.lblProjectCode.AutoSize = True
		Me.lblProjectCode.Location = New System.Drawing.Point(205, 128)
		Me.lblProjectCode.Name = "lblProjectCode"
		Me.lblProjectCode.Size = New System.Drawing.Size(44, 14)
		Me.lblProjectCode.TabIndex = 0
		Me.lblProjectCode.Text = "פרויקט"
		Me.lblProjectCode.TextAlign = System.Drawing.ContentAlignment.TopRight
		'
		'txtProjectCode
		'
		Me.txtProjectCode.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
		Me.txtProjectCode.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource
		Me.txtProjectCode.Enabled = False
		Me.txtProjectCode.Location = New System.Drawing.Point(93, 128)
		Me.txtProjectCode.Name = "txtProjectCode"
		Me.txtProjectCode.Size = New System.Drawing.Size(100, 22)
		Me.txtProjectCode.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(205, 165)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(37, 14)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "גרסה"
		Me.Label1.TextAlign = System.Drawing.ContentAlignment.TopRight
		'
		'cmbDetail
		'
		Me.cmbDetail.DisplayMember = "DetailName"
		Me.cmbDetail.Enabled = False
		Me.cmbDetail.FormattingEnabled = True
		Me.cmbDetail.Location = New System.Drawing.Point(93, 164)
		Me.cmbDetail.Name = "cmbDetail"
		Me.cmbDetail.Size = New System.Drawing.Size(99, 22)
		Me.cmbDetail.TabIndex = 3
		Me.cmbDetail.ValueMember = "Detail"
		'
		'cmbTheme
		'
		Me.cmbTheme.DisplayMember = "MapThemeName"
		Me.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cmbTheme.Enabled = False
		Me.cmbTheme.FormattingEnabled = True
		Me.cmbTheme.Location = New System.Drawing.Point(94, 205)
		Me.cmbTheme.Name = "cmbTheme"
		Me.cmbTheme.Size = New System.Drawing.Size(99, 22)
		Me.cmbTheme.TabIndex = 5
		Me.cmbTheme.ValueMember = "MapThemeID"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(206, 206)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(33, 14)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "נושא"
		Me.Label2.TextAlign = System.Drawing.ContentAlignment.TopRight
		'
		'rdbStandard
		'
		Me.rdbStandard.AutoSize = True
		Me.rdbStandard.Checked = True
		Me.rdbStandard.Location = New System.Drawing.Point(233, 17)
		Me.rdbStandard.Name = "rdbStandard"
		Me.rdbStandard.Size = New System.Drawing.Size(65, 18)
		Me.rdbStandard.TabIndex = 6
		Me.rdbStandard.TabStop = True
		Me.rdbStandard.Text = "מבא""ת"
		Me.rdbStandard.UseVisualStyleBackColor = True
		'
		'rdbAllLanduses
		'
		Me.rdbAllLanduses.AutoSize = True
		Me.rdbAllLanduses.Location = New System.Drawing.Point(214, 52)
		Me.rdbAllLanduses.Name = "rdbAllLanduses"
		Me.rdbAllLanduses.Size = New System.Drawing.Size(84, 18)
		Me.rdbAllLanduses.TabIndex = 7
		Me.rdbAllLanduses.Text = "כל היעודים"
		Me.rdbAllLanduses.UseVisualStyleBackColor = True
		'
		'rdbProject
		'
		Me.rdbProject.AutoSize = True
		Me.rdbProject.Location = New System.Drawing.Point(225, 87)
		Me.rdbProject.Name = "rdbProject"
		Me.rdbProject.Size = New System.Drawing.Size(73, 18)
		Me.rdbProject.TabIndex = 8
		Me.rdbProject.Text = "פרויקטים"
		Me.rdbProject.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Location = New System.Drawing.Point(19, 12)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(75, 23)
		Me.cmdOK.TabIndex = 9
		Me.cmdOK.Text = "OK"
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Location = New System.Drawing.Point(112, 12)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
		Me.cmdCancel.TabIndex = 10
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'frmSelectColorSet
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(324, 241)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.rdbProject)
		Me.Controls.Add(Me.rdbAllLanduses)
		Me.Controls.Add(Me.rdbStandard)
		Me.Controls.Add(Me.cmbTheme)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.cmbDetail)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtProjectCode)
		Me.Controls.Add(Me.lblProjectCode)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmSelectColorSet"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "בחר"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents lblProjectCode As System.Windows.Forms.Label
	Private WithEvents txtProjectCode As System.Windows.Forms.TextBox
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents rdbStandard As System.Windows.Forms.RadioButton
	Private WithEvents rdbAllLanduses As System.Windows.Forms.RadioButton
	Private WithEvents rdbProject As System.Windows.Forms.RadioButton
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Private WithEvents cmbDetail As System.Windows.Forms.ComboBox
	Private WithEvents cmbTheme As System.Windows.Forms.ComboBox
End Class
