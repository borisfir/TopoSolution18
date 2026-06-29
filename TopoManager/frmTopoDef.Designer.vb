<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTopoDef
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
		Me.Label1 = New System.Windows.Forms.Label
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.TextBox2 = New System.Windows.Forms.TextBox
		Me.Label2 = New System.Windows.Forms.Label
		Me.DataGridView1 = New System.Windows.Forms.DataGridView
		Me.BlockName = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.Layer = New System.Windows.Forms.DataGridViewTextBoxColumn
		Me.chkCreateCentroid = New System.Windows.Forms.CheckBox
		Me.txtMissingCentroidLayer = New System.Windows.Forms.TextBox
		Me.Label3 = New System.Windows.Forms.Label
		Me.txtMissingCentroidBlock = New System.Windows.Forms.TextBox
		Me.Label4 = New System.Windows.Forms.Label
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(194, 31)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(42, 14)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Label1"
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(22, 28)
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(151, 22)
		Me.TextBox1.TabIndex = 1
		'
		'TextBox2
		'
		Me.TextBox2.Location = New System.Drawing.Point(22, 65)
		Me.TextBox2.Name = "TextBox2"
		Me.TextBox2.Size = New System.Drawing.Size(151, 22)
		Me.TextBox2.TabIndex = 3
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(255, 73)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(42, 14)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Label2"
		'
		'DataGridView1
		'
		Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.BlockName, Me.Layer})
		Me.DataGridView1.Location = New System.Drawing.Point(12, 142)
		Me.DataGridView1.Name = "DataGridView1"
		Me.DataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
		Me.DataGridView1.RowHeadersVisible = False
		Me.DataGridView1.Size = New System.Drawing.Size(261, 140)
		Me.DataGridView1.TabIndex = 4
		'
		'BlockName
		'
		Me.BlockName.HeaderText = "שם"
		Me.BlockName.Name = "BlockName"
		'
		'Layer
		'
		Me.Layer.HeaderText = "Layer"
		Me.Layer.Name = "Layer"
		'
		'chkCreateCentroid
		'
		Me.chkCreateCentroid.AutoSize = True
		Me.chkCreateCentroid.Location = New System.Drawing.Point(12, 299)
		Me.chkCreateCentroid.Name = "chkCreateCentroid"
		Me.chkCreateCentroid.Size = New System.Drawing.Size(108, 18)
		Me.chkCreateCentroid.TabIndex = 5
		Me.chkCreateCentroid.Text = "CreateCentroid"
		Me.chkCreateCentroid.UseVisualStyleBackColor = True
		'
		'txtMissingCentroidLayer
		'
		Me.txtMissingCentroidLayer.Location = New System.Drawing.Point(22, 340)
		Me.txtMissingCentroidLayer.Name = "txtMissingCentroidLayer"
		Me.txtMissingCentroidLayer.Size = New System.Drawing.Size(151, 22)
		Me.txtMissingCentroidLayer.TabIndex = 7
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(255, 348)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(42, 14)
		Me.Label3.TabIndex = 6
		Me.Label3.Text = "Label3"
		'
		'txtMissingCentroidBlock
		'
		Me.txtMissingCentroidBlock.Location = New System.Drawing.Point(22, 368)
		Me.txtMissingCentroidBlock.Name = "txtMissingCentroidBlock"
		Me.txtMissingCentroidBlock.Size = New System.Drawing.Size(151, 22)
		Me.txtMissingCentroidBlock.TabIndex = 9
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(255, 376)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(42, 14)
		Me.Label4.TabIndex = 8
		Me.Label4.Text = "Label4"
		'
		'frmTopoDef
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(381, 431)
		Me.Controls.Add(Me.txtMissingCentroidBlock)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.txtMissingCentroidLayer)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.chkCreateCentroid)
		Me.Controls.Add(Me.DataGridView1)
		Me.Controls.Add(Me.TextBox2)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TextBox1)
		Me.Controls.Add(Me.Label1)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmTopoDef"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "הגדרת טופולוגיה"
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
	Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
	Friend WithEvents BlockName As System.Windows.Forms.DataGridViewTextBoxColumn
	Friend WithEvents Layer As System.Windows.Forms.DataGridViewTextBoxColumn
	Private WithEvents chkCreateCentroid As System.Windows.Forms.CheckBox
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents txtMissingCentroidLayer As System.Windows.Forms.TextBox
	Private WithEvents txtMissingCentroidBlock As System.Windows.Forms.TextBox
	Private WithEvents Label4 As System.Windows.Forms.Label
End Class
