<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
		Me.Button1 = New System.Windows.Forms.Button
		Me.Button2 = New System.Windows.Forms.Button
		Me.Button3 = New System.Windows.Forms.Button
		Me.Button4 = New System.Windows.Forms.Button
		Me.txtNum = New System.Windows.Forms.TextBox
		Me.txtHeb = New System.Windows.Forms.TextBox
		Me.Button5 = New System.Windows.Forms.Button
		Me.txtHebB = New System.Windows.Forms.TextBox
		Me.SuspendLayout()
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(5, 10)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(61, 32)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(84, 10)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(52, 32)
		Me.Button2.TabIndex = 1
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Button3
		'
		Me.Button3.Location = New System.Drawing.Point(155, 11)
		Me.Button3.Name = "Button3"
		Me.Button3.Size = New System.Drawing.Size(56, 30)
		Me.Button3.TabIndex = 2
		Me.Button3.Text = "Button3"
		Me.Button3.UseVisualStyleBackColor = True
		'
		'Button4
		'
		Me.Button4.Location = New System.Drawing.Point(217, 11)
		Me.Button4.Name = "Button4"
		Me.Button4.Size = New System.Drawing.Size(56, 30)
		Me.Button4.TabIndex = 3
		Me.Button4.Text = "Button4"
		Me.Button4.UseVisualStyleBackColor = True
		'
		'txtNum
		'
		Me.txtNum.Location = New System.Drawing.Point(28, 64)
		Me.txtNum.Name = "txtNum"
		Me.txtNum.Size = New System.Drawing.Size(154, 20)
		Me.txtNum.TabIndex = 4
		'
		'txtHeb
		'
		Me.txtHeb.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtHeb.Location = New System.Drawing.Point(31, 126)
		Me.txtHeb.Name = "txtHeb"
		Me.txtHeb.Size = New System.Drawing.Size(150, 26)
		Me.txtHeb.TabIndex = 5
		'
		'Button5
		'
		Me.Button5.Location = New System.Drawing.Point(201, 64)
		Me.Button5.Name = "Button5"
		Me.Button5.Size = New System.Drawing.Size(36, 19)
		Me.Button5.TabIndex = 6
		Me.Button5.Text = "Button5"
		Me.Button5.UseVisualStyleBackColor = True
		'
		'txtHebB
		'
		Me.txtHebB.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtHebB.Location = New System.Drawing.Point(32, 166)
		Me.txtHebB.Name = "txtHebB"
		Me.txtHebB.Size = New System.Drawing.Size(150, 26)
		Me.txtHebB.TabIndex = 7
		'
		'Form2
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(356, 325)
		Me.Controls.Add(Me.txtHebB)
		Me.Controls.Add(Me.Button5)
		Me.Controls.Add(Me.txtHeb)
		Me.Controls.Add(Me.txtNum)
		Me.Controls.Add(Me.Button4)
		Me.Controls.Add(Me.Button3)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Name = "Form2"
		Me.Text = "Form2"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Friend WithEvents Button3 As System.Windows.Forms.Button
	Friend WithEvents Button4 As System.Windows.Forms.Button
	Friend WithEvents txtNum As System.Windows.Forms.TextBox
	Friend WithEvents txtHeb As System.Windows.Forms.TextBox
	Friend WithEvents Button5 As System.Windows.Forms.Button
	Friend WithEvents txtHebB As System.Windows.Forms.TextBox
End Class
