<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMsgBox
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
      Me.txtMain = New System.Windows.Forms.TextBox()
      Me.SuspendLayout()
      '
      'txtMain
      '
      Me.txtMain.Dock = System.Windows.Forms.DockStyle.Fill
      Me.txtMain.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.txtMain.Location = New System.Drawing.Point(0, 0)
      Me.txtMain.Multiline = True
      Me.txtMain.Name = "txtMain"
      Me.txtMain.Size = New System.Drawing.Size(284, 262)
      Me.txtMain.TabIndex = 0
      '
      'frmMsgBox
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(284, 262)
      Me.Controls.Add(Me.txtMain)
      Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      Me.Name = "frmMsgBox"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Private WithEvents txtMain As System.Windows.Forms.TextBox
End Class
