<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditColorSchemeOld
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
		Me.txtName = New System.Windows.Forms.TextBox
		Me.Label1 = New System.Windows.Forms.Label
		Me.Label2 = New System.Windows.Forms.Label
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.grbBorder = New System.Windows.Forms.GroupBox
		Me.grbZebra = New System.Windows.Forms.GroupBox
		Me.grbHatch = New System.Windows.Forms.GroupBox
		Me.tlbTop = New System.Windows.Forms.ToolStrip
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'pcbPicture
		'
		Me.pcbPicture.BackColor = System.Drawing.SystemColors.Window
		Me.pcbPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.pcbPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.pcbPicture.Location = New System.Drawing.Point(2, 28)
		Me.pcbPicture.Name = "pcbPicture"
		Me.pcbPicture.Size = New System.Drawing.Size(297, 153)
		Me.pcbPicture.TabIndex = 0
		Me.pcbPicture.TabStop = False
		'
		'txtName
		'
		Me.txtName.Location = New System.Drawing.Point(318, 28)
		Me.txtName.Name = "txtName"
		Me.txtName.Size = New System.Drawing.Size(116, 21)
		Me.txtName.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(443, 8)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(38, 13)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "Label1"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(443, 28)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(38, 13)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "Label2"
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(318, 54)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(116, 60)
		Me.TextBox1.TabIndex = 3
		'
		'grbBorder
		'
		Me.grbBorder.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbBorder.Location = New System.Drawing.Point(2, 188)
		Me.grbBorder.Name = "grbBorder"
		Me.grbBorder.Size = New System.Drawing.Size(287, 198)
		Me.grbBorder.TabIndex = 6
		Me.grbBorder.TabStop = False
		Me.grbBorder.Text = "קו תוחם שטח"
		'
		'grbZebra
		'
		Me.grbZebra.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbZebra.Location = New System.Drawing.Point(318, 177)
		Me.grbZebra.Name = "grbZebra"
		Me.grbZebra.Size = New System.Drawing.Size(163, 162)
		Me.grbZebra.TabIndex = 7
		Me.grbZebra.TabStop = False
		Me.grbZebra.Text = "מילוי זברה"
		'
		'grbHatch
		'
		Me.grbHatch.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbHatch.Location = New System.Drawing.Point(12, 392)
		Me.grbHatch.Name = "grbHatch"
		Me.grbHatch.Size = New System.Drawing.Size(272, 74)
		Me.grbHatch.TabIndex = 8
		Me.grbHatch.TabStop = False
		Me.grbHatch.Text = "כיסוי עלי"
		'
		'tlbTop
		'
		Me.tlbTop.Location = New System.Drawing.Point(0, 0)
		Me.tlbTop.Name = "tlbTop"
		Me.tlbTop.Size = New System.Drawing.Size(497, 25)
		Me.tlbTop.TabIndex = 9
		Me.tlbTop.Text = "ToolStrip1"
		'
		'EditColorScheme
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(497, 493)
		Me.Controls.Add(Me.tlbTop)
		Me.Controls.Add(Me.grbHatch)
		Me.Controls.Add(Me.grbZebra)
		Me.Controls.Add(Me.grbBorder)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtName)
		Me.Controls.Add(Me.pcbPicture)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "EditColorScheme"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "EditColorScheme"
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents pcbPicture As System.Windows.Forms.PictureBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Private WithEvents grbBorder As System.Windows.Forms.GroupBox
	Friend WithEvents grbZebra As System.Windows.Forms.GroupBox
	Friend WithEvents grbHatch As System.Windows.Forms.GroupBox
	Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
	Private WithEvents txtName As System.Windows.Forms.TextBox
End Class
