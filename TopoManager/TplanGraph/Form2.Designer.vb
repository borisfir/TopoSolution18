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
		Me.CheckBox1 = New System.Windows.Forms.CheckBox
		Me.GroupBox1 = New System.Windows.Forms.GroupBox
		Me.Label1 = New System.Windows.Forms.Label
		Me.DataGridView1 = New System.Windows.Forms.DataGridView
		Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.GroupBox1.SuspendLayout()
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(21, 19)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(105, 36)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'CheckBox1
		'
		Me.CheckBox1.AutoSize = True
		Me.CheckBox1.Location = New System.Drawing.Point(154, 21)
		Me.CheckBox1.Name = "CheckBox1"
		Me.CheckBox1.Size = New System.Drawing.Size(81, 17)
		Me.CheckBox1.TabIndex = 1
		Me.CheckBox1.Text = "CheckBox1"
		Me.CheckBox1.UseVisualStyleBackColor = True
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.Label1)
		Me.GroupBox1.Location = New System.Drawing.Point(21, 124)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(226, 143)
		Me.GroupBox1.TabIndex = 2
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "GroupBox1"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(103, 41)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(39, 13)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Label1"
		'
		'DataGridView1
		'
		Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2})
		Me.DataGridView1.Location = New System.Drawing.Point(12, 289)
		Me.DataGridView1.Name = "DataGridView1"
		Me.DataGridView1.Size = New System.Drawing.Size(355, 64)
		Me.DataGridView1.TabIndex = 3
		'
		'Column1
		'
		Me.Column1.HeaderText = "Column1"
		Me.Column1.Name = "Column1"
		'
		'Column2
		'
		Me.Column2.HeaderText = "Column2"
		Me.Column2.Name = "Column2"
		'
		'Form2
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(382, 360)
		Me.Controls.Add(Me.DataGridView1)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.CheckBox1)
		Me.Controls.Add(Me.Button1)
		Me.Name = "Form2"
		Me.Text = "Form2"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
	Friend WithEvents Column1 As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents Column2 As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
