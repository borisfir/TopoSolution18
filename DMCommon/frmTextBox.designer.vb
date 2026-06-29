<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTextBox
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
		Me.txtBox = New System.Windows.Forms.TextBox()
		Me.SuspendLayout()
		'
		'txtBox
		'
		Me.txtBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me.txtBox.Location = New System.Drawing.Point(0, 0)
		Me.txtBox.Multiline = True
		Me.txtBox.Name = "txtBox"
		Me.txtBox.Size = New System.Drawing.Size(800, 450)
		Me.txtBox.TabIndex = 0
		Me.txtBox.WordWrap = False
		'
		'frmTextBox
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(800, 450)
		Me.Controls.Add(Me.txtBox)
		Me.Name = "frmTextBox"
		Me.Text = "frmTextBox"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents txtBox As Windows.Forms.TextBox
End Class
