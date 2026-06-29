<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Me.RadioButton1 = New System.Windows.Forms.RadioButton
		Me.GroupBox1 = New System.Windows.Forms.GroupBox
		Me.RadioButton3 = New System.Windows.Forms.RadioButton
		Me.RadioButton2 = New System.Windows.Forms.RadioButton
		Me.ComboBox1 = New System.Windows.Forms.ComboBox
		Me.CheckBox1 = New System.Windows.Forms.CheckBox
		Me.GroupBox2 = New System.Windows.Forms.GroupBox
		Me.Panel1 = New System.Windows.Forms.Panel
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.Label1 = New System.Windows.Forms.Label
		Me.GroupBox1.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.SuspendLayout()
		'
		'RadioButton1
		'
		Me.RadioButton1.AutoSize = True
		Me.RadioButton1.Location = New System.Drawing.Point(26, 14)
		Me.RadioButton1.Name = "RadioButton1"
		Me.RadioButton1.Size = New System.Drawing.Size(90, 17)
		Me.RadioButton1.TabIndex = 0
		Me.RadioButton1.TabStop = True
		Me.RadioButton1.Text = "RadioButton1"
		Me.RadioButton1.UseVisualStyleBackColor = True
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.RadioButton3)
		Me.GroupBox1.Controls.Add(Me.RadioButton2)
		Me.GroupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup
		Me.GroupBox1.Location = New System.Drawing.Point(26, 58)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.GroupBox1.Size = New System.Drawing.Size(196, 112)
		Me.GroupBox1.TabIndex = 1
		Me.GroupBox1.TabStop = False
		'
		'RadioButton3
		'
		Me.RadioButton3.AutoSize = True
		Me.RadioButton3.Location = New System.Drawing.Point(19, 55)
		Me.RadioButton3.Name = "RadioButton3"
		Me.RadioButton3.Size = New System.Drawing.Size(90, 17)
		Me.RadioButton3.TabIndex = 1
		Me.RadioButton3.TabStop = True
		Me.RadioButton3.Text = "RadioButton3"
		Me.RadioButton3.UseVisualStyleBackColor = True
		'
		'RadioButton2
		'
		Me.RadioButton2.AutoSize = True
		Me.RadioButton2.Location = New System.Drawing.Point(19, 22)
		Me.RadioButton2.Name = "RadioButton2"
		Me.RadioButton2.Size = New System.Drawing.Size(90, 17)
		Me.RadioButton2.TabIndex = 0
		Me.RadioButton2.TabStop = True
		Me.RadioButton2.Text = "RadioButton2"
		Me.RadioButton2.UseVisualStyleBackColor = True
		'
		'ComboBox1
		'
		Me.ComboBox1.FormattingEnabled = True
		Me.ComboBox1.Location = New System.Drawing.Point(273, 18)
		Me.ComboBox1.Name = "ComboBox1"
		Me.ComboBox1.Size = New System.Drawing.Size(115, 21)
		Me.ComboBox1.TabIndex = 2
		'
		'CheckBox1
		'
		Me.CheckBox1.Appearance = System.Windows.Forms.Appearance.Button
		Me.CheckBox1.AutoSize = True
		Me.CheckBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.CheckBox1.Image = Global.TopoManager.My.Resources.Resources.QuestionMark
		Me.CheckBox1.Location = New System.Drawing.Point(270, 89)
		Me.CheckBox1.Name = "CheckBox1"
		Me.CheckBox1.Size = New System.Drawing.Size(72, 23)
		Me.CheckBox1.TabIndex = 3
		Me.CheckBox1.Text = "CheckBox1"
		Me.CheckBox1.UseVisualStyleBackColor = True
		'
		'GroupBox2
		'
		Me.GroupBox2.BackColor = System.Drawing.Color.Transparent
		Me.GroupBox2.Controls.Add(Me.Panel1)
		Me.GroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.GroupBox2.Location = New System.Drawing.Point(160, 0)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(200, 164)
		Me.GroupBox2.TabIndex = 4
		Me.GroupBox2.TabStop = False
		'
		'Panel1
		'
		Me.Panel1.BackColor = System.Drawing.Color.Red
		Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
		Me.Panel1.Location = New System.Drawing.Point(199, 19)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(110, 172)
		Me.Panel1.TabIndex = 5
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(141, 195)
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(132, 20)
		Me.TextBox1.TabIndex = 5
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(289, 195)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(39, 13)
		Me.Label1.TabIndex = 6
		Me.Label1.Text = "Label1"
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(477, 416)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.GroupBox2)
		Me.Controls.Add(Me.CheckBox1)
		Me.Controls.Add(Me.ComboBox1)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.RadioButton1)
		Me.Name = "Form1"
		Me.Text = "Form1"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.GroupBox2.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
   Friend WithEvents RadioButton1 As System.Windows.Forms.RadioButton
   Friend WithEvents RadioButton2 As System.Windows.Forms.RadioButton
   Private WithEvents GroupBox1 As System.Windows.Forms.GroupBox
   Friend WithEvents RadioButton3 As System.Windows.Forms.RadioButton

   Private Sub GroupBox1_CausesValidationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GroupBox1.CausesValidationChanged

   End Sub
   Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
   Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
   Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
