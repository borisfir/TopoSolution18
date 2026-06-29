<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConstUserData
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
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtLT_Name = New System.Windows.Forms.TextBox()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.bnsUserData = New System.Windows.Forms.BindingSource(Me.components)
		CType(Me.bnsUserData, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label1.Location = New System.Drawing.Point(86, 36)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(42, 14)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "NAME"
		'
		'txtLT_Name
		'
		Me.txtLT_Name.Location = New System.Drawing.Point(134, 32)
		Me.txtLT_Name.Name = "txtLT_Name"
		Me.txtLT_Name.Size = New System.Drawing.Size(216, 22)
		Me.txtLT_Name.TabIndex = 1
		'
		'cmdCancel
		'
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Location = New System.Drawing.Point(214, 146)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
		Me.cmdCancel.TabIndex = 12
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Location = New System.Drawing.Point(89, 146)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(75, 23)
		Me.cmdOK.TabIndex = 11
		Me.cmdOK.Text = "OK"
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'bnsUserData
		'
		'
		'frmConstUserData
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(362, 215)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.txtLT_Name)
		Me.Controls.Add(Me.Label1)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmConstUserData"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "נתוני פרויקט"
		CType(Me.bnsUserData, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents bnsUserData As System.Windows.Forms.BindingSource
	Private WithEvents txtLT_Name As System.Windows.Forms.TextBox
End Class
