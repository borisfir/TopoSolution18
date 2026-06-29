<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form3
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
      Me.Button1 = New System.Windows.Forms.Button()
      Me.txtDisp = New System.Windows.Forms.TextBox()
      Me.Button2 = New System.Windows.Forms.Button()
      Me.LabelA = New System.Windows.Forms.Label()
      Me.LabelB = New System.Windows.Forms.Label()
      Me.cmbFind = New System.Windows.Forms.ComboBox()
      Me.Button3 = New System.Windows.Forms.Button()
      Me.lstFind = New System.Windows.Forms.ListBox()
      Me.TextBox1 = New System.Windows.Forms.TextBox()
      Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
      Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
      Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
      Me.GroupBox1 = New System.Windows.Forms.GroupBox()
      Me.TextBox2 = New System.Windows.Forms.TextBox()
      Me.Button4 = New System.Windows.Forms.Button()
      Me.dgvArea = New System.Windows.Forms.DataGridView()
      Me.ctxSource = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGroup = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxBasicPgon = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxOutput1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxOutput2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.Button5 = New System.Windows.Forms.Button()
      Me.Button6 = New System.Windows.Forms.Button()
      Me.txtDest = New System.Windows.Forms.TextBox()
      Me.dgvGroups = New System.Windows.Forms.DataGridView()
      Me.ctxGroupNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGrSource = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGrOutput1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGrOutput2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGrConst = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.cmbRound = New System.Windows.Forms.ComboBox()
      Me.dgvPgons = New System.Windows.Forms.DataGridView()
      Me.ctxPgonID = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxPgonSource = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.DataGridViewTextBoxColumn4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.DataGridViewTextBoxColumn5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.Button7 = New System.Windows.Forms.Button()
      Me.ToolStrip1.SuspendLayout()
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.GroupBox1.SuspendLayout()
      CType(Me.dgvArea, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.dgvGroups, System.ComponentModel.ISupportInitialize).BeginInit()
      CType(Me.dgvPgons, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'Button1
      '
      Me.Button1.Location = New System.Drawing.Point(15, 36)
      Me.Button1.Name = "Button1"
      Me.Button1.Size = New System.Drawing.Size(87, 25)
      Me.Button1.TabIndex = 13
      Me.Button1.Text = "Button1"
      Me.Button1.UseVisualStyleBackColor = True
      '
      'txtDisp
      '
      Me.txtDisp.Location = New System.Drawing.Point(633, 3)
      Me.txtDisp.Multiline = True
      Me.txtDisp.Name = "txtDisp"
      Me.txtDisp.Size = New System.Drawing.Size(158, 36)
      Me.txtDisp.TabIndex = 1
      '
      'Button2
      '
      Me.Button2.Location = New System.Drawing.Point(15, 70)
      Me.Button2.Name = "Button2"
      Me.Button2.Size = New System.Drawing.Size(87, 25)
      Me.Button2.TabIndex = 2
      Me.Button2.Text = "Button2"
      Me.Button2.UseVisualStyleBackColor = True
      '
      'LabelA
      '
      Me.LabelA.AutoSize = True
      Me.LabelA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.LabelA.Location = New System.Drawing.Point(406, 36)
      Me.LabelA.Name = "LabelA"
      Me.LabelA.Size = New System.Drawing.Size(45, 16)
      Me.LabelA.TabIndex = 3
      Me.LabelA.Text = "LabelA"
      '
      'LabelB
      '
      Me.LabelB.Location = New System.Drawing.Point(406, 75)
      Me.LabelB.Name = "LabelB"
      Me.LabelB.Size = New System.Drawing.Size(117, 25)
      Me.LabelB.TabIndex = 4
      Me.LabelB.Text = "LabelB"
      '
      'cmbFind
      '
      Me.cmbFind.DisplayMember = "Name"
      Me.cmbFind.FormattingEnabled = True
      Me.cmbFind.Location = New System.Drawing.Point(152, 88)
      Me.cmbFind.Name = "cmbFind"
      Me.cmbFind.Size = New System.Drawing.Size(312, 22)
      Me.cmbFind.TabIndex = 16
      Me.cmbFind.ValueMember = "ID"
      '
      'Button3
      '
      Me.Button3.Location = New System.Drawing.Point(136, 36)
      Me.Button3.Name = "Button3"
      Me.Button3.Size = New System.Drawing.Size(87, 25)
      Me.Button3.TabIndex = 12
      Me.Button3.Text = "Button3"
      Me.Button3.UseVisualStyleBackColor = True
      '
      'lstFind
      '
      Me.lstFind.DisplayMember = "Name"
      Me.lstFind.FormattingEnabled = True
      Me.lstFind.ItemHeight = 14
      Me.lstFind.Location = New System.Drawing.Point(152, 129)
      Me.lstFind.Name = "lstFind"
      Me.lstFind.Size = New System.Drawing.Size(325, 32)
      Me.lstFind.TabIndex = 1
      Me.lstFind.ValueMember = "ID"
      '
      'TextBox1
      '
      Me.TextBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
      Me.TextBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList
      Me.TextBox1.BackColor = System.Drawing.SystemColors.WindowText
      Me.TextBox1.Location = New System.Drawing.Point(629, 88)
      Me.TextBox1.Name = "TextBox1"
      Me.TextBox1.Size = New System.Drawing.Size(161, 22)
      Me.TextBox1.TabIndex = 8
      '
      'ToolStrip1
      '
      Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripTextBox1})
      Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
      Me.ToolStrip1.Name = "ToolStrip1"
      Me.ToolStrip1.Size = New System.Drawing.Size(968, 25)
      Me.ToolStrip1.TabIndex = 9
      Me.ToolStrip1.Text = "ToolStrip1"
      '
      'ToolStripTextBox1
      '
      Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
      Me.ToolStripTextBox1.Size = New System.Drawing.Size(100, 25)
      '
      'NumericUpDown1
      '
      Me.NumericUpDown1.Location = New System.Drawing.Point(656, 60)
      Me.NumericUpDown1.Name = "NumericUpDown1"
      Me.NumericUpDown1.Size = New System.Drawing.Size(76, 22)
      Me.NumericUpDown1.TabIndex = 10
      '
      'GroupBox1
      '
      Me.GroupBox1.Controls.Add(Me.TextBox2)
      Me.GroupBox1.Location = New System.Drawing.Point(532, 128)
      Me.GroupBox1.Name = "GroupBox1"
      Me.GroupBox1.Size = New System.Drawing.Size(131, 71)
      Me.GroupBox1.TabIndex = 11
      Me.GroupBox1.TabStop = False
      Me.GroupBox1.Text = "GroupBox1"
      '
      'TextBox2
      '
      Me.TextBox2.Location = New System.Drawing.Point(16, 21)
      Me.TextBox2.Name = "TextBox2"
      Me.TextBox2.Size = New System.Drawing.Size(100, 22)
      Me.TextBox2.TabIndex = 0
      '
      'Button4
      '
      Me.Button4.Location = New System.Drawing.Point(15, 113)
      Me.Button4.Name = "Button4"
      Me.Button4.Size = New System.Drawing.Size(87, 25)
      Me.Button4.TabIndex = 12
      Me.Button4.Text = "Button4"
      Me.Button4.UseVisualStyleBackColor = True
      '
      'dgvArea
      '
      Me.dgvArea.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvArea.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxSource, Me.ctxGroup, Me.ctxBasicPgon, Me.ctxOutput1, Me.ctxOutput2})
      Me.dgvArea.Location = New System.Drawing.Point(0, 248)
      Me.dgvArea.Name = "dgvArea"
      Me.dgvArea.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
      Me.dgvArea.Size = New System.Drawing.Size(520, 253)
      Me.dgvArea.TabIndex = 17
      '
      'ctxSource
      '
      Me.ctxSource.HeaderText = "Source"
      Me.ctxSource.Name = "ctxSource"
      Me.ctxSource.Width = 84
      '
      'ctxGroup
      '
      Me.ctxGroup.HeaderText = "Group"
      Me.ctxGroup.Name = "ctxGroup"
      Me.ctxGroup.Width = 60
      '
      'ctxBasicPgon
      '
      Me.ctxBasicPgon.HeaderText = "Pgon"
      Me.ctxBasicPgon.Name = "ctxBasicPgon"
      Me.ctxBasicPgon.Width = 60
      '
      'ctxOutput1
      '
      Me.ctxOutput1.HeaderText = "Output1"
      Me.ctxOutput1.Name = "ctxOutput1"
      Me.ctxOutput1.Width = 60
      '
      'ctxOutput2
      '
      Me.ctxOutput2.HeaderText = "Output2"
      Me.ctxOutput2.Name = "ctxOutput2"
      Me.ctxOutput2.Width = 60
      '
      'Button5
      '
      Me.Button5.Location = New System.Drawing.Point(0, 206)
      Me.Button5.Name = "Button5"
      Me.Button5.Size = New System.Drawing.Size(76, 25)
      Me.Button5.TabIndex = 14
      Me.Button5.Text = "AddData"
      Me.Button5.UseVisualStyleBackColor = True
      '
      'Button6
      '
      Me.Button6.Location = New System.Drawing.Point(82, 206)
      Me.Button6.Name = "Button6"
      Me.Button6.Size = New System.Drawing.Size(61, 25)
      Me.Button6.TabIndex = 15
      Me.Button6.Text = "Calc"
      Me.Button6.UseVisualStyleBackColor = True
      '
      'txtDest
      '
      Me.txtDest.Location = New System.Drawing.Point(174, 206)
      Me.txtDest.Name = "txtDest"
      Me.txtDest.Size = New System.Drawing.Size(49, 22)
      Me.txtDest.TabIndex = 1
      '
      'dgvGroups
      '
      Me.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvGroups.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxGroupNo, Me.ctxGrSource, Me.ctxGrOutput1, Me.ctxGrOutput2, Me.ctxGrConst})
      Me.dgvGroups.Location = New System.Drawing.Point(532, 248)
      Me.dgvGroups.Name = "dgvGroups"
      Me.dgvGroups.Size = New System.Drawing.Size(406, 88)
      Me.dgvGroups.TabIndex = 16
      '
      'ctxGroupNo
      '
      Me.ctxGroupNo.HeaderText = "GroupNo"
      Me.ctxGroupNo.Name = "ctxGroupNo"
      Me.ctxGroupNo.Width = 60
      '
      'ctxGrSource
      '
      Me.ctxGrSource.HeaderText = "Source"
      Me.ctxGrSource.Name = "ctxGrSource"
      Me.ctxGrSource.Width = 84
      '
      'ctxGrOutput1
      '
      Me.ctxGrOutput1.HeaderText = "Output"
      Me.ctxGrOutput1.Name = "ctxGrOutput1"
      Me.ctxGrOutput1.Width = 60
      '
      'ctxGrOutput2
      '
      Me.ctxGrOutput2.HeaderText = "Output2"
      Me.ctxGrOutput2.Name = "ctxGrOutput2"
      Me.ctxGrOutput2.Width = 60
      '
      'ctxGrConst
      '
      Me.ctxGrConst.HeaderText = "Const"
      Me.ctxGrConst.Name = "ctxGrConst"
      Me.ctxGrConst.Width = 60
      '
      'cmbRound
      '
      Me.cmbRound.DisplayMember = "Name"
      Me.cmbRound.FormattingEnabled = True
      Me.cmbRound.Items.AddRange(New Object() {"1", "10", "100", "1000"})
      Me.cmbRound.Location = New System.Drawing.Point(238, 208)
      Me.cmbRound.Name = "cmbRound"
      Me.cmbRound.Size = New System.Drawing.Size(59, 22)
      Me.cmbRound.TabIndex = 0
      Me.cmbRound.ValueMember = "ID"
      '
      'dgvPgons
      '
      Me.dgvPgons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvPgons.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxPgonID, Me.ctxPgonSource, Me.DataGridViewTextBoxColumn3, Me.DataGridViewTextBoxColumn4, Me.DataGridViewTextBoxColumn5})
      Me.dgvPgons.Location = New System.Drawing.Point(532, 342)
      Me.dgvPgons.Name = "dgvPgons"
      Me.dgvPgons.Size = New System.Drawing.Size(406, 159)
      Me.dgvPgons.TabIndex = 18
      '
      'ctxPgonID
      '
      Me.ctxPgonID.HeaderText = "Pgon"
      Me.ctxPgonID.Name = "ctxPgonID"
      Me.ctxPgonID.Width = 60
      '
      'ctxPgonSource
      '
      Me.ctxPgonSource.HeaderText = "Source"
      Me.ctxPgonSource.Name = "ctxPgonSource"
      Me.ctxPgonSource.Width = 84
      '
      'DataGridViewTextBoxColumn3
      '
      Me.DataGridViewTextBoxColumn3.HeaderText = "Output"
      Me.DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3"
      Me.DataGridViewTextBoxColumn3.Width = 60
      '
      'DataGridViewTextBoxColumn4
      '
      Me.DataGridViewTextBoxColumn4.HeaderText = "Output2"
      Me.DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4"
      Me.DataGridViewTextBoxColumn4.Width = 60
      '
      'DataGridViewTextBoxColumn5
      '
      Me.DataGridViewTextBoxColumn5.HeaderText = "Const"
      Me.DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5"
      Me.DataGridViewTextBoxColumn5.Width = 60
      '
      'Button7
      '
      Me.Button7.Location = New System.Drawing.Point(15, 149)
      Me.Button7.Name = "Button7"
      Me.Button7.Size = New System.Drawing.Size(87, 25)
      Me.Button7.TabIndex = 19
      Me.Button7.Text = "Button7"
      Me.Button7.UseVisualStyleBackColor = True
      '
      'Form3
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
      Me.ClientSize = New System.Drawing.Size(968, 513)
      Me.Controls.Add(Me.Button7)
      Me.Controls.Add(Me.dgvPgons)
      Me.Controls.Add(Me.cmbRound)
      Me.Controls.Add(Me.dgvGroups)
      Me.Controls.Add(Me.txtDest)
      Me.Controls.Add(Me.Button6)
      Me.Controls.Add(Me.Button5)
      Me.Controls.Add(Me.dgvArea)
      Me.Controls.Add(Me.Button4)
      Me.Controls.Add(Me.GroupBox1)
      Me.Controls.Add(Me.NumericUpDown1)
      Me.Controls.Add(Me.ToolStrip1)
      Me.Controls.Add(Me.TextBox1)
      Me.Controls.Add(Me.lstFind)
      Me.Controls.Add(Me.Button3)
      Me.Controls.Add(Me.cmbFind)
      Me.Controls.Add(Me.LabelB)
      Me.Controls.Add(Me.LabelA)
      Me.Controls.Add(Me.Button2)
      Me.Controls.Add(Me.txtDisp)
      Me.Controls.Add(Me.Button1)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Name = "Form3"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.Text = "3"
      Me.ToolStrip1.ResumeLayout(False)
      Me.ToolStrip1.PerformLayout()
      CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
      Me.GroupBox1.ResumeLayout(False)
      Me.GroupBox1.PerformLayout()
      CType(Me.dgvArea, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.dgvGroups, System.ComponentModel.ISupportInitialize).EndInit()
      CType(Me.dgvPgons, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents Button1 As System.Windows.Forms.Button
   Friend WithEvents Button2 As System.Windows.Forms.Button
   Private WithEvents LabelA As System.Windows.Forms.Label
   Private WithEvents LabelB As System.Windows.Forms.Label
   Friend WithEvents Button3 As System.Windows.Forms.Button
   Private WithEvents cmbFind As System.Windows.Forms.ComboBox
   Private WithEvents lstFind As System.Windows.Forms.ListBox
   Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
   Private WithEvents txtDisp As System.Windows.Forms.TextBox
   Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
   Friend WithEvents ToolStripTextBox1 As System.Windows.Forms.ToolStripTextBox
   Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
   Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
   Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
   Friend WithEvents Button4 As System.Windows.Forms.Button
   Friend WithEvents Button5 As System.Windows.Forms.Button
   Private WithEvents dgvArea As System.Windows.Forms.DataGridView
   Friend WithEvents Button6 As System.Windows.Forms.Button
   Private WithEvents txtDest As System.Windows.Forms.TextBox
   Private WithEvents dgvGroups As System.Windows.Forms.DataGridView
   Private WithEvents cmbRound As System.Windows.Forms.ComboBox
   Friend WithEvents ctxGroupNo As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGrSource As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGrOutput1 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGrOutput2 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGrConst As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxSource As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGroup As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxBasicPgon As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxOutput1 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxOutput2 As System.Windows.Forms.DataGridViewTextBoxColumn
   Private WithEvents dgvPgons As System.Windows.Forms.DataGridView
   Friend WithEvents ctxPgonID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxPgonSource As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents DataGridViewTextBoxColumn4 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents DataGridViewTextBoxColumn5 As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents Button7 As System.Windows.Forms.Button
End Class
