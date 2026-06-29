<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDesign
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
		Me.lblTopology = New System.Windows.Forms.Label()
		Me.txtTopoName = New System.Windows.Forms.TextBox()
		Me.SuspendLayout()
		'
		'cmbPaintScale
		'
		Me.cmbPaintScale.DisplayMember = "Name"
		Me.cmbPaintScale.FormattingEnabled = True
		Me.cmbPaintScale.Location = New System.Drawing.Point(12, 55)
		Me.cmbPaintScale.Name = "cmbPaintScale"
		Me.cmbPaintScale.Size = New System.Drawing.Size(88, 22)
		Me.cmbPaintScale.TabIndex = 0
		Me.cmbPaintScale.ValueMember = "ID"
		'
		'cmdPaintByLanduse
		'
		Me.cmdPaintByLanduse.Location = New System.Drawing.Point(142, 55)
		Me.cmdPaintByLanduse.Name = "cmdPaintByLanduse"
		Me.cmdPaintByLanduse.Size = New System.Drawing.Size(71, 23)
		Me.cmdPaintByLanduse.TabIndex = 1
		Me.cmdPaintByLanduse.Text = "Paint"
		Me.cmdPaintByLanduse.UseVisualStyleBackColor = True
		'
		'cmdEditColorSet
		'
		Me.cmdEditColorSet.Location = New System.Drawing.Point(86, 106)
		Me.cmdEditColorSet.Name = "cmdEditColorSet"
		Me.cmdEditColorSet.Size = New System.Drawing.Size(71, 23)
		Me.cmdEditColorSet.TabIndex = 2
		Me.cmdEditColorSet.Text = "ColorSet"
		Me.cmdEditColorSet.UseVisualStyleBackColor = True
		'
		'lblTopology
		'
		Me.lblTopology.AccessibleRole = System.Windows.Forms.AccessibleRole.Alert
		Me.lblTopology.AutoSize = True
		Me.lblTopology.Location = New System.Drawing.Point(298, 17)
		Me.lblTopology.Name = "lblTopology"
		Me.lblTopology.Size = New System.Drawing.Size(56, 14)
		Me.lblTopology.TabIndex = 3
		Me.lblTopology.Text = "טופולוגיה"
		'
		'txtTopoName
		'
		Me.txtTopoName.Location = New System.Drawing.Point(106, 9)
		Me.txtTopoName.Name = "txtTopoName"
		Me.txtTopoName.ReadOnly = True
		Me.txtTopoName.Size = New System.Drawing.Size(100, 22)
		Me.txtTopoName.TabIndex = 4
		'
		'frmDesign
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(419, 262)
		Me.Controls.Add(Me.txtTopoName)
		Me.Controls.Add(Me.lblTopology)
		Me.Controls.Add(Me.cmdEditColorSet)
		Me.Controls.Add(Me.cmdPaintByLanduse)
		Me.Controls.Add(Me.cmbPaintScale)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Name = "frmDesign"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.Text = "צביעת המפה"
		Me.TopMost = True
		Me.ResumeLayout(False)
		Me.PerformLayout()

End Sub
	Private WithEvents cmbPaintScale As System.Windows.Forms.ComboBox
	Private WithEvents cmdPaintByLanduse As System.Windows.Forms.Button
	Private WithEvents cmdEditColorSet As System.Windows.Forms.Button
	Private WithEvents lblTopology As System.Windows.Forms.Label
	Private WithEvents txtTopoName As System.Windows.Forms.TextBox
End Class
