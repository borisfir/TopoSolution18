<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DispModelSpace
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
      Dim SelectByCurrent As System.Windows.Forms.ToolStripButton
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DispModelSpace))
      Me.dgvMain = New System.Windows.Forms.DataGridView
      Me.toolstrUp = New System.Windows.Forms.ToolStrip
      SelectByCurrent = New System.Windows.Forms.ToolStripButton
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.toolstrUp.SuspendLayout()
      Me.SuspendLayout()
      '
      'SelectByCurrent
      '
      SelectByCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      SelectByCurrent.Image = CType(resources.GetObject("SelectByCurrent.Image"), System.Drawing.Image)
      SelectByCurrent.ImageTransparentColor = System.Drawing.Color.Magenta
      SelectByCurrent.Name = "SelectByCurrent"
      SelectByCurrent.Size = New System.Drawing.Size(23, 22)
      SelectByCurrent.Text = "Select"
      AddHandler SelectByCurrent.Click, AddressOf Me.SelectByCurrent_Click
      '
      'dgvMain
      '
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.dgvMain.Location = New System.Drawing.Point(0, 27)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.Size = New System.Drawing.Size(694, 306)
      Me.dgvMain.TabIndex = 0
      '
      'toolstrUp
      '
      Me.toolstrUp.Items.AddRange(New System.Windows.Forms.ToolStripItem() {SelectByCurrent})
      Me.toolstrUp.Location = New System.Drawing.Point(0, 0)
      Me.toolstrUp.Name = "toolstrUp"
      Me.toolstrUp.Size = New System.Drawing.Size(694, 25)
      Me.toolstrUp.TabIndex = 1
      '
      'DispModelSpace
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(694, 333)
      Me.Controls.Add(Me.toolstrUp)
      Me.Controls.Add(Me.dgvMain)
      Me.Name = "DispModelSpace"
      Me.Text = "DispModelSpace"
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.toolstrUp.ResumeLayout(False)
      Me.toolstrUp.PerformLayout()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents dgvMain As System.Windows.Forms.DataGridView
   Friend WithEvents toolstrUp As System.Windows.Forms.ToolStrip
End Class
