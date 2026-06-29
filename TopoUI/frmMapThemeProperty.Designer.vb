<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMapThemeProperty
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
      Me.prgMain = New System.Windows.Forms.PropertyGrid()
      Me.SuspendLayout()
      '
      'prgMain
      '
      Me.prgMain.Dock = System.Windows.Forms.DockStyle.Fill
      Me.prgMain.Location = New System.Drawing.Point(0, 0)
      Me.prgMain.Name = "prgMain"
      Me.prgMain.Size = New System.Drawing.Size(367, 407)
      Me.prgMain.TabIndex = 0
      '
      'frmMapThemeProperty
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(367, 407)
      Me.Controls.Add(Me.prgMain)
      Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      Me.Name = "frmMapThemeProperty"
      Me.ShowInTaskbar = False
      Me.Text = "Properties"
      Me.ResumeLayout(False)

   End Sub
   Private WithEvents prgMain As System.Windows.Forms.PropertyGrid
End Class
