<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditColorScheme
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
		Me.Label1 = New System.Windows.Forms.Label
		Me.Label2 = New System.Windows.Forms.Label
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.grbBorder = New System.Windows.Forms.GroupBox
		Me.grbZebra = New System.Windows.Forms.GroupBox
		Me.grbHatch = New System.Windows.Forms.GroupBox
		Me.tlbTop = New System.Windows.Forms.ToolStrip
		Me.tcbWinColor = New System.Windows.Forms.ToolStripButton
		Me.tcbPaint = New System.Windows.Forms.ToolStripButton
		Me.cmbColorSchemes = New System.Windows.Forms.ComboBox
		Me.grbFill = New System.Windows.Forms.GroupBox
		Me.Label3 = New System.Windows.Forms.Label
		Me.cdlWindow = New System.Windows.Forms.ColorDialog
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.tlbTop.SuspendLayout()
		Me.SuspendLayout()
		'
		'pcbPicture
		'
		Me.pcbPicture.BackColor = System.Drawing.SystemColors.Window
		Me.pcbPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.pcbPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.pcbPicture.Location = New System.Drawing.Point(2, 28)
		Me.pcbPicture.Name = "pcbPicture"
		Me.pcbPicture.Size = New System.Drawing.Size(292, 148)
		Me.pcbPicture.TabIndex = 0
		Me.pcbPicture.TabStop = False
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
		Me.Label2.Location = New System.Drawing.Point(412, 45)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(38, 13)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "Label2"
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(300, 116)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(169, 60)
		Me.TextBox1.TabIndex = 3
		'
		'grbBorder
		'
		Me.grbBorder.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbBorder.Location = New System.Drawing.Point(6, 184)
		Me.grbBorder.Name = "grbBorder"
		Me.grbBorder.Size = New System.Drawing.Size(190, 172)
		Me.grbBorder.TabIndex = 6
		Me.grbBorder.TabStop = False
		Me.grbBorder.Text = "קו תוחם שטח"
		'
		'grbZebra
		'
		Me.grbZebra.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbZebra.Location = New System.Drawing.Point(270, 184)
		Me.grbZebra.Name = "grbZebra"
		Me.grbZebra.Size = New System.Drawing.Size(190, 204)
		Me.grbZebra.TabIndex = 7
		Me.grbZebra.TabStop = False
		Me.grbZebra.Text = "מילוי זברה"
		'
		'grbHatch
		'
		Me.grbHatch.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbHatch.Location = New System.Drawing.Point(6, 364)
		Me.grbHatch.Name = "grbHatch"
		Me.grbHatch.Size = New System.Drawing.Size(190, 102)
		Me.grbHatch.TabIndex = 8
		Me.grbHatch.TabStop = False
		Me.grbHatch.Text = "כיסוי עלי"
		'
		'tlbTop
		'
		Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tcbWinColor, Me.tcbPaint})
		Me.tlbTop.Location = New System.Drawing.Point(0, 0)
		Me.tlbTop.Name = "tlbTop"
		Me.tlbTop.Size = New System.Drawing.Size(473, 25)
		Me.tlbTop.TabIndex = 9
		Me.tlbTop.Text = "ToolStrip1"
		'
		'tcbWinColor
		'
		Me.tcbWinColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tcbWinColor.Image = Global.Test.My.Resources.Resources.ColorHS
		Me.tcbWinColor.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tcbWinColor.Name = "tcbWinColor"
		Me.tcbWinColor.Size = New System.Drawing.Size(23, 22)
		'
		'tcbPaint
		'
		Me.tcbPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tcbPaint.Image = Global.Test.My.Resources.Resources.Brush
		Me.tcbPaint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.tcbPaint.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tcbPaint.Name = "tcbPaint"
		Me.tcbPaint.Size = New System.Drawing.Size(23, 22)
		'
		'cmbColorSchemes
		'
		Me.cmbColorSchemes.FormattingEnabled = True
		Me.cmbColorSchemes.Location = New System.Drawing.Point(297, 61)
		Me.cmbColorSchemes.Name = "cmbColorSchemes"
		Me.cmbColorSchemes.Size = New System.Drawing.Size(176, 21)
		Me.cmbColorSchemes.TabIndex = 11
		'
		'grbFill
		'
		Me.grbFill.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.grbFill.Location = New System.Drawing.Point(270, 396)
		Me.grbFill.Name = "grbFill"
		Me.grbFill.Size = New System.Drawing.Size(190, 50)
		Me.grbFill.TabIndex = 9
		Me.grbFill.TabStop = False
		Me.grbFill.Text = "מילוי מלא צבע"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(412, 100)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(38, 13)
		Me.Label3.TabIndex = 12
		Me.Label3.Text = "Label3"
		'
		'EditColorScheme
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(473, 472)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.grbFill)
		Me.Controls.Add(Me.cmbColorSchemes)
		Me.Controls.Add(Me.tlbTop)
		Me.Controls.Add(Me.grbHatch)
		Me.Controls.Add(Me.grbZebra)
		Me.Controls.Add(Me.grbBorder)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.pcbPicture)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "EditColorScheme"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "סכימת צביעה"
		CType(Me.pcbPicture, System.ComponentModel.ISupportInitialize).EndInit()
		Me.tlbTop.ResumeLayout(False)
		Me.tlbTop.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents pcbPicture As System.Windows.Forms.PictureBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Private WithEvents grbBorder As System.Windows.Forms.GroupBox
	Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
	Friend WithEvents cmbColorSchemes As System.Windows.Forms.ComboBox
	Private WithEvents grbZebra As System.Windows.Forms.GroupBox
	Private WithEvents grbFill As System.Windows.Forms.GroupBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents grbHatch As System.Windows.Forms.GroupBox
	Private WithEvents cdlWindow As System.Windows.Forms.ColorDialog
	Private WithEvents tcbWinColor As System.Windows.Forms.ToolStripButton
	Private WithEvents tcbPaint As System.Windows.Forms.ToolStripButton
End Class
