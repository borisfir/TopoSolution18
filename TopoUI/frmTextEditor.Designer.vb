<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTextEditor
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
      Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
      Me.tsbSaveAsTxt = New System.Windows.Forms.ToolStripButton()
      Me.tsbOpenFolder = New System.Windows.Forms.ToolStripButton()
      Me.tsbCreatePDF = New System.Windows.Forms.ToolStripButton()
      Me.tsbExit = New System.Windows.Forms.ToolStripButton()
      Me.txtMain = New System.Windows.Forms.TextBox()
      Me.ToolStrip1.SuspendLayout()
      Me.SuspendLayout()
      '
      'ToolStrip1
      '
      Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbSaveAsTxt, Me.tsbOpenFolder, Me.tsbCreatePDF, Me.tsbExit})
      Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
      Me.ToolStrip1.Name = "ToolStrip1"
      Me.ToolStrip1.Size = New System.Drawing.Size(643, 25)
      Me.ToolStrip1.TabIndex = 0
      Me.ToolStrip1.Text = "ToolStrip1"
      '
      'tsbSaveAsTxt
      '
      Me.tsbSaveAsTxt.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbSaveAsTxt.Image = Global.TopoUI.My.Resources.Resources.Save
      Me.tsbSaveAsTxt.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbSaveAsTxt.Name = "tsbSaveAsTxt"
      Me.tsbSaveAsTxt.Size = New System.Drawing.Size(23, 22)
      '
      'tsbOpenFolder
      '
      Me.tsbOpenFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbOpenFolder.Image = Global.TopoUI.My.Resources.Resources.openfolderHS
      Me.tsbOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbOpenFolder.Name = "tsbOpenFolder"
      Me.tsbOpenFolder.Size = New System.Drawing.Size(23, 22)
      '
      'tsbCreatePDF
      '
      Me.tsbCreatePDF.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbCreatePDF.Image = Global.TopoUI.My.Resources.Resources.Acrobat16
      Me.tsbCreatePDF.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbCreatePDF.Name = "tsbCreatePDF"
      Me.tsbCreatePDF.Size = New System.Drawing.Size(23, 22)
      Me.tsbCreatePDF.Text = "ToolStripButton1"
      '
      'tsbExit
      '
      Me.tsbExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbExit.Image = Global.TopoUI.My.Resources.Resources._Exit
      Me.tsbExit.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.tsbExit.Name = "tsbExit"
      Me.tsbExit.Size = New System.Drawing.Size(23, 22)
      '
      'txtMain
      '
      Me.txtMain.Dock = System.Windows.Forms.DockStyle.Fill
      Me.txtMain.Font = New System.Drawing.Font("Courier New", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.txtMain.Location = New System.Drawing.Point(0, 25)
      Me.txtMain.Multiline = True
      Me.txtMain.Name = "txtMain"
      Me.txtMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
      Me.txtMain.Size = New System.Drawing.Size(643, 333)
      Me.txtMain.TabIndex = 1
      '
      'frmTextEditor
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(643, 358)
      Me.Controls.Add(Me.txtMain)
      Me.Controls.Add(Me.ToolStrip1)
      Me.Name = "frmTextEditor"
      Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
      Me.Text = "frmTextEditor"
      Me.ToolStrip1.ResumeLayout(False)
      Me.ToolStrip1.PerformLayout()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Private WithEvents txtMain As System.Windows.Forms.TextBox
   Private WithEvents tsbSaveAsTxt As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbOpenFolder As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbExit As System.Windows.Forms.ToolStripButton
   Private WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
   Private WithEvents tsbCreatePDF As System.Windows.Forms.ToolStripButton
End Class
