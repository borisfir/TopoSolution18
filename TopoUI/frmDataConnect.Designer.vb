<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDataConnect
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
		Me.btnFind = New System.Windows.Forms.Button()
		Me.oTest = New AcMapDataConnectUI()
		Me.SuspendLayout()
		'
		'btnFind
		'
		Me.btnFind.Location = New System.Drawing.Point(279, 45)
		Me.btnFind.Name = "btnFind"
		Me.btnFind.Size = New System.Drawing.Size(75, 23)
		Me.btnFind.TabIndex = 0
		Me.btnFind.Text = "Button"
		Me.btnFind.UseVisualStyleBackColor = True
		'
		'frmDataConnect
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(391, 387)
		Me.Controls.Add(Me.btnFind)
		Me.Name = "frmDataConnect"
		Me.Text = "frmDataConnect"
		Me.ResumeLayout(False)

	End Sub
	Private WithEvents btnFind As System.Windows.Forms.Button
	Private WithEvents oTest As System.Windows.Forms.UserControl
End Class
