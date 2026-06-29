<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditColorSchemeA
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
		Me.pcbPicture = New System.Windows.Forms.PictureBox
		Me.Button1 = New System.Windows.Forms.Button
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'pcbPicture
		'
		Me.pcbPicture.BackColor = System.Drawing.SystemColors.Window
		Me.pcbPicture.Location = New System.Drawing.Point(0, 0)
		Me.pcbPicture.Name = "pcbPicture"
		Me.pcbPicture.Size = New System.Drawing.Size(260, 132)
		Me.pcbPicture.TabIndex = 0
		Me.pcbPicture.TabStop = False
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(309, 11)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(139, 43)
		Me.Button1.TabIndex = 1
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'EditColorSchemeA
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(495, 395)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.pcbPicture)
		Me.Name = "EditColorSchemeA"
		Me.Text = "EditColorScheme"
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub
	Private WithEvents pcbPicture As System.Windows.Forms.PictureBox
	Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
