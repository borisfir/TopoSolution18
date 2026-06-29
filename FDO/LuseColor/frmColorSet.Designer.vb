<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmColorSet
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
		Me.cmbColorSetList = New System.Windows.Forms.ComboBox
		Me.DataGridView1 = New System.Windows.Forms.DataGridView
		Me.cmbLanduse = New System.Windows.Forms.DataGridViewTextBoxColumn
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'cmbColorSetList
		'
		Me.cmbColorSetList.FormattingEnabled = True
		Me.cmbColorSetList.Location = New System.Drawing.Point(115, 15)
		Me.cmbColorSetList.Name = "cmbColorSetList"
		Me.cmbColorSetList.Size = New System.Drawing.Size(126, 21)
		Me.cmbColorSetList.TabIndex = 0
		'
		'DataGridView1
		'
		Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cmbLanduse})
		Me.DataGridView1.Location = New System.Drawing.Point(13, 101)
		Me.DataGridView1.Name = "DataGridView1"
		Me.DataGridView1.Size = New System.Drawing.Size(275, 160)
		Me.DataGridView1.TabIndex = 1
		'
		'cmbLanduse
		'
		Me.cmbLanduse.HeaderText = "йтег"
		Me.cmbLanduse.Name = "cmbLanduse"
		'
		'frmColorSet
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(292, 271)
		Me.Controls.Add(Me.DataGridView1)
		Me.Controls.Add(Me.cmbColorSetList)
		Me.Name = "frmColorSet"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "frmColorSet"
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents cmbColorSetList As System.Windows.Forms.ComboBox
	Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
	Friend WithEvents cmbLanduse As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
