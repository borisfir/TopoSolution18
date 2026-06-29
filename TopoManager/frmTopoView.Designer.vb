<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTopoView
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
      Me.dgvMain = New System.Windows.Forms.DataGridView
      Me.cmbTopoNames = New System.Windows.Forms.ComboBox
      Me.cmbUnions = New System.Windows.Forms.ComboBox
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'dgvMain
      '
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.dgvMain.Location = New System.Drawing.Point(0, 29)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.Size = New System.Drawing.Size(583, 306)
      Me.dgvMain.TabIndex = 1
      '
      'cmbTopoNames
      '
      Me.cmbTopoNames.FormattingEnabled = True
      Me.cmbTopoNames.Location = New System.Drawing.Point(76, 4)
      Me.cmbTopoNames.Name = "cmbTopoNames"
      Me.cmbTopoNames.Size = New System.Drawing.Size(127, 21)
      Me.cmbTopoNames.TabIndex = 2
      '
      'cmbUnions
      '
      Me.cmbUnions.FormattingEnabled = True
      Me.cmbUnions.Location = New System.Drawing.Point(234, 4)
      Me.cmbUnions.Name = "cmbUnions"
      Me.cmbUnions.Size = New System.Drawing.Size(127, 21)
      Me.cmbUnions.TabIndex = 3
      '
      'frmTopoView
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(583, 335)
      Me.Controls.Add(Me.cmbUnions)
      Me.Controls.Add(Me.cmbTopoNames)
      Me.Controls.Add(Me.dgvMain)
      Me.Name = "frmTopoView"
      Me.Text = "frmTopoView"
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

   End Sub
   Friend WithEvents dgvMain As System.Windows.Forms.DataGridView
   Private WithEvents cmbTopoNames As System.Windows.Forms.ComboBox
   Private WithEvents cmbUnions As System.Windows.Forms.ComboBox
End Class
