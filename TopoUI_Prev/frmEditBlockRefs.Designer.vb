<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditBlockRefs
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
      Me.cmbBlockList = New System.Windows.Forms.ComboBox()
      Me.dgvMain = New System.Windows.Forms.DataGridView()
      Me.cmdZoom = New System.Windows.Forms.Button()
      Me.txtValue = New System.Windows.Forms.TextBox()
      Me.cmdSetValue = New System.Windows.Forms.Button()
      Me.cmdFindBlockRef = New System.Windows.Forms.Button()
      Me.cmdUpdateCurrent = New System.Windows.Forms.Button()
      Me.cmdUpdateAll = New System.Windows.Forms.Button()
      Me.Button1 = New System.Windows.Forms.Button()
      Me.rdbPrompt = New System.Windows.Forms.RadioButton()
      Me.rdbTag = New System.Windows.Forms.RadioButton()
      Me.cmdDispSelectionSet = New System.Windows.Forms.Button()
      Me.GroupBox1 = New System.Windows.Forms.GroupBox()
      Me.rdbFilter = New System.Windows.Forms.RadioButton()
      Me.rdbSelection = New System.Windows.Forms.RadioButton()
      Me.cmdRemoveFilter = New System.Windows.Forms.Button()
      Me.cmdDispSelSet = New System.Windows.Forms.Button()
      Me.Button2 = New System.Windows.Forms.Button()
      Me.lblRecCount = New System.Windows.Forms.Label()
      Me.cmbBlockLayers = New System.Windows.Forms.ComboBox()
      Me.Button3 = New System.Windows.Forms.Button()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.Label2 = New System.Windows.Forms.Label()
      Me.chkFirstRow_Tag = New System.Windows.Forms.CheckBox()
      Me.cmdImportExcel = New System.Windows.Forms.Button()
      Me.chkAdditionalBlock = New System.Windows.Forms.CheckBox()
      Me.Button4 = New System.Windows.Forms.Button()
      Me.ctxObjectID = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxAddObjectID = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxPositionX = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxPositionY = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxScale = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.cchChanged = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      Me.ccbLayer = New System.Windows.Forms.DataGridViewComboBoxColumn()
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.GroupBox1.SuspendLayout()
      Me.SuspendLayout()
      '
      'cmbBlockList
      '
      Me.cmbBlockList.FormattingEnabled = True
      Me.cmbBlockList.Location = New System.Drawing.Point(42, 6)
      Me.cmbBlockList.Name = "cmbBlockList"
      Me.cmbBlockList.Size = New System.Drawing.Size(180, 22)
      Me.cmbBlockList.Sorted = True
      Me.cmbBlockList.TabIndex = 0
      '
      'dgvMain
      '
      Me.dgvMain.AllowUserToAddRows = False
      Me.dgvMain.AllowUserToDeleteRows = False
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxObjectID, Me.ctxAddObjectID, Me.ctxPositionX, Me.ctxPositionY, Me.ctxScale, Me.cchChanged, Me.ccbLayer})
      Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.dgvMain.Location = New System.Drawing.Point(0, 62)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.RowHeadersWidth = 24
      Me.dgvMain.Size = New System.Drawing.Size(1116, 411)
      Me.dgvMain.TabIndex = 1
      '
      'cmdZoom
      '
      Me.cmdZoom.AutoSize = True
      Me.cmdZoom.Location = New System.Drawing.Point(745, 8)
      Me.cmdZoom.Name = "cmdZoom"
      Me.cmdZoom.Size = New System.Drawing.Size(48, 24)
      Me.cmdZoom.TabIndex = 2
      Me.cmdZoom.Text = "Zoom"
      Me.cmdZoom.UseVisualStyleBackColor = True
      '
      'txtValue
      '
      Me.txtValue.Location = New System.Drawing.Point(228, 8)
      Me.txtValue.Name = "txtValue"
      Me.txtValue.Size = New System.Drawing.Size(88, 22)
      Me.txtValue.TabIndex = 3
      '
      'cmdSetValue
      '
      Me.cmdSetValue.Location = New System.Drawing.Point(322, 9)
      Me.cmdSetValue.Name = "cmdSetValue"
      Me.cmdSetValue.Size = New System.Drawing.Size(40, 23)
      Me.cmdSetValue.TabIndex = 4
      Me.cmdSetValue.Text = "OK"
      Me.cmdSetValue.UseVisualStyleBackColor = True
      '
      'cmdFindBlockRef
      '
      Me.cmdFindBlockRef.AutoSize = True
      Me.cmdFindBlockRef.Location = New System.Drawing.Point(554, 8)
      Me.cmdFindBlockRef.Name = "cmdFindBlockRef"
      Me.cmdFindBlockRef.Size = New System.Drawing.Size(41, 24)
      Me.cmdFindBlockRef.TabIndex = 5
      Me.cmdFindBlockRef.Text = "Find"
      Me.cmdFindBlockRef.UseVisualStyleBackColor = True
      '
      'cmdUpdateCurrent
      '
      Me.cmdUpdateCurrent.AutoSize = True
      Me.cmdUpdateCurrent.Location = New System.Drawing.Point(368, 8)
      Me.cmdUpdateCurrent.Name = "cmdUpdateCurrent"
      Me.cmdUpdateCurrent.Size = New System.Drawing.Size(69, 24)
      Me.cmdUpdateCurrent.TabIndex = 6
      Me.cmdUpdateCurrent.Text = "Save Rec"
      Me.cmdUpdateCurrent.UseVisualStyleBackColor = True
      '
      'cmdUpdateAll
      '
      Me.cmdUpdateAll.AutoSize = True
      Me.cmdUpdateAll.Location = New System.Drawing.Point(443, 8)
      Me.cmdUpdateAll.Name = "cmdUpdateAll"
      Me.cmdUpdateAll.Size = New System.Drawing.Size(61, 24)
      Me.cmdUpdateAll.TabIndex = 7
      Me.cmdUpdateAll.Text = "Save All"
      Me.cmdUpdateAll.UseVisualStyleBackColor = True
      '
      'Button1
      '
      Me.Button1.Location = New System.Drawing.Point(644, 7)
      Me.Button1.Name = "Button1"
      Me.Button1.Size = New System.Drawing.Size(25, 23)
      Me.Button1.TabIndex = 8
      Me.Button1.Text = "?"
      Me.Button1.UseVisualStyleBackColor = True
      Me.Button1.Visible = False
      '
      'rdbPrompt
      '
      Me.rdbPrompt.AutoSize = True
      Me.rdbPrompt.Checked = True
      Me.rdbPrompt.Location = New System.Drawing.Point(806, 11)
      Me.rdbPrompt.Name = "rdbPrompt"
      Me.rdbPrompt.Size = New System.Drawing.Size(65, 18)
      Me.rdbPrompt.TabIndex = 9
      Me.rdbPrompt.TabStop = True
      Me.rdbPrompt.Text = "Prompt"
      Me.rdbPrompt.UseVisualStyleBackColor = True
      '
      'rdbTag
      '
      Me.rdbTag.AutoSize = True
      Me.rdbTag.Location = New System.Drawing.Point(806, 35)
      Me.rdbTag.Name = "rdbTag"
      Me.rdbTag.Size = New System.Drawing.Size(46, 18)
      Me.rdbTag.TabIndex = 10
      Me.rdbTag.TabStop = True
      Me.rdbTag.Text = "Tag"
      Me.rdbTag.UseVisualStyleBackColor = True
      '
      'cmdDispSelectionSet
      '
      Me.cmdDispSelectionSet.AutoSize = True
      Me.cmdDispSelectionSet.Location = New System.Drawing.Point(512, 8)
      Me.cmdDispSelectionSet.Name = "cmdDispSelectionSet"
      Me.cmdDispSelectionSet.Size = New System.Drawing.Size(36, 24)
      Me.cmdDispSelectionSet.TabIndex = 11
      Me.cmdDispSelectionSet.Text = "Set"
      Me.cmdDispSelectionSet.UseVisualStyleBackColor = True
      '
      'GroupBox1
      '
      Me.GroupBox1.Controls.Add(Me.rdbFilter)
      Me.GroupBox1.Controls.Add(Me.rdbSelection)
      Me.GroupBox1.Location = New System.Drawing.Point(880, 0)
      Me.GroupBox1.Name = "GroupBox1"
      Me.GroupBox1.Size = New System.Drawing.Size(85, 56)
      Me.GroupBox1.TabIndex = 12
      Me.GroupBox1.TabStop = False
      '
      'rdbFilter
      '
      Me.rdbFilter.Location = New System.Drawing.Point(4, 32)
      Me.rdbFilter.Name = "rdbFilter"
      Me.rdbFilter.Size = New System.Drawing.Size(75, 18)
      Me.rdbFilter.TabIndex = 1
      Me.rdbFilter.Text = "Filter"
      Me.rdbFilter.UseVisualStyleBackColor = True
      '
      'rdbSelection
      '
      Me.rdbSelection.Checked = True
      Me.rdbSelection.Location = New System.Drawing.Point(4, 12)
      Me.rdbSelection.Name = "rdbSelection"
      Me.rdbSelection.Size = New System.Drawing.Size(75, 18)
      Me.rdbSelection.TabIndex = 0
      Me.rdbSelection.TabStop = True
      Me.rdbSelection.Text = "Selection"
      Me.rdbSelection.UseVisualStyleBackColor = True
      '
      'cmdRemoveFilter
      '
      Me.cmdRemoveFilter.Image = Global.TopoUI.My.Resources.Resources.RemoveFilter
      Me.cmdRemoveFilter.Location = New System.Drawing.Point(968, 6)
      Me.cmdRemoveFilter.Name = "cmdRemoveFilter"
      Me.cmdRemoveFilter.Size = New System.Drawing.Size(28, 24)
      Me.cmdRemoveFilter.TabIndex = 112
      Me.cmdRemoveFilter.UseVisualStyleBackColor = True
      '
      'cmdDispSelSet
      '
      Me.cmdDispSelSet.AutoSize = True
      Me.cmdDispSelSet.Location = New System.Drawing.Point(690, 8)
      Me.cmdDispSelSet.Name = "cmdDispSelSet"
      Me.cmdDispSelSet.Size = New System.Drawing.Size(49, 24)
      Me.cmdDispSelSet.TabIndex = 113
      Me.cmdDispSelSet.Text = "Graph"
      Me.cmdDispSelSet.UseVisualStyleBackColor = True
      '
      'Button2
      '
      Me.Button2.Location = New System.Drawing.Point(644, 35)
      Me.Button2.Name = "Button2"
      Me.Button2.Size = New System.Drawing.Size(25, 23)
      Me.Button2.TabIndex = 114
      Me.Button2.Text = "?"
      Me.Button2.UseVisualStyleBackColor = True
      Me.Button2.Visible = False
      '
      'lblRecCount
      '
      Me.lblRecCount.AutoSize = True
      Me.lblRecCount.Location = New System.Drawing.Point(971, 36)
      Me.lblRecCount.Name = "lblRecCount"
      Me.lblRecCount.RightToLeft = System.Windows.Forms.RightToLeft.No
      Me.lblRecCount.Size = New System.Drawing.Size(14, 14)
      Me.lblRecCount.TabIndex = 115
      Me.lblRecCount.Text = "0"
      '
      'cmbBlockLayers
      '
      Me.cmbBlockLayers.FormattingEnabled = True
      Me.cmbBlockLayers.Location = New System.Drawing.Point(42, 32)
      Me.cmbBlockLayers.Name = "cmbBlockLayers"
      Me.cmbBlockLayers.Size = New System.Drawing.Size(180, 22)
      Me.cmbBlockLayers.Sorted = True
      Me.cmbBlockLayers.TabIndex = 116
      '
      'Button3
      '
      Me.Button3.Location = New System.Drawing.Point(601, 6)
      Me.Button3.Name = "Button3"
      Me.Button3.Size = New System.Drawing.Size(26, 23)
      Me.Button3.TabIndex = 117
      Me.Button3.Text = "Button3"
      Me.Button3.UseVisualStyleBackColor = True
      Me.Button3.Visible = False
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(2, 6)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(25, 14)
      Me.Label1.TabIndex = 118
      Me.Label1.Text = "שם"
      '
      'Label2
      '
      Me.Label2.AutoSize = True
      Me.Label2.Location = New System.Drawing.Point(2, 32)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(41, 14)
      Me.Label2.TabIndex = 119
      Me.Label2.Text = "שכבה"
      '
      'chkFirstRow_Tag
      '
      Me.chkFirstRow_Tag.AutoSize = True
      Me.chkFirstRow_Tag.Location = New System.Drawing.Point(690, 40)
      Me.chkFirstRow_Tag.Name = "chkFirstRow_Tag"
      Me.chkFirstRow_Tag.Size = New System.Drawing.Size(105, 18)
      Me.chkFirstRow_Tag.TabIndex = 120
      Me.chkFirstRow_Tag.Text = "FirstRow - Tag"
      Me.chkFirstRow_Tag.UseVisualStyleBackColor = True
      '
      'cmdImportExcel
      '
      Me.cmdImportExcel.Image = Global.TopoUI.My.Resources.Resources.ImpExcel15Tr
      Me.cmdImportExcel.Location = New System.Drawing.Point(1011, 5)
      Me.cmdImportExcel.Name = "cmdImportExcel"
      Me.cmdImportExcel.Size = New System.Drawing.Size(28, 24)
      Me.cmdImportExcel.TabIndex = 121
      Me.cmdImportExcel.UseVisualStyleBackColor = True
      '
      'chkAdditionalBlock
      '
      Me.chkAdditionalBlock.AutoSize = True
      Me.chkAdditionalBlock.Location = New System.Drawing.Point(238, 40)
      Me.chkAdditionalBlock.Name = "chkAdditionalBlock"
      Me.chkAdditionalBlock.Size = New System.Drawing.Size(51, 18)
      Me.chkAdditionalBlock.TabIndex = 122
      Me.chkAdditionalBlock.Text = "נוסף"
      Me.chkAdditionalBlock.UseVisualStyleBackColor = True
      '
      'Button4
      '
      Me.Button4.Image = Global.TopoUI.My.Resources.Resources.ImpExcel15Tr
      Me.Button4.Location = New System.Drawing.Point(1045, 4)
      Me.Button4.Name = "Button4"
      Me.Button4.Size = New System.Drawing.Size(147, 26)
      Me.Button4.TabIndex = 123
      Me.Button4.Text = "שטח רשום-->חלקות"
      Me.Button4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
      Me.Button4.UseVisualStyleBackColor = True
      '
      'ctxObjectID
      '
      Me.ctxObjectID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
      Me.ctxObjectID.DataPropertyName = "ObjectID"
      Me.ctxObjectID.Frozen = True
      Me.ctxObjectID.HeaderText = "ObjectID"
      Me.ctxObjectID.Name = "ctxObjectID"
      Me.ctxObjectID.ReadOnly = True
      Me.ctxObjectID.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
      Me.ctxObjectID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxObjectID.Visible = False
      Me.ctxObjectID.Width = 105
      '
      'ctxAddObjectID
      '
      Me.ctxAddObjectID.DataPropertyName = "AddObjectID"
      Me.ctxAddObjectID.Frozen = True
      Me.ctxAddObjectID.HeaderText = "AddObjectID"
      Me.ctxAddObjectID.Name = "ctxAddObjectID"
      Me.ctxAddObjectID.Visible = False
      '
      'ctxPositionX
      '
      Me.ctxPositionX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
      Me.ctxPositionX.DataPropertyName = "PositionX"
      Me.ctxPositionX.Frozen = True
      Me.ctxPositionX.HeaderText = "X"
      Me.ctxPositionX.Name = "ctxPositionX"
      Me.ctxPositionX.ReadOnly = True
      Me.ctxPositionX.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
      Me.ctxPositionX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxPositionX.Visible = False
      Me.ctxPositionX.Width = 5
      '
      'ctxPositionY
      '
      Me.ctxPositionY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
      Me.ctxPositionY.DataPropertyName = "PositionY"
      Me.ctxPositionY.Frozen = True
      Me.ctxPositionY.HeaderText = "Y"
      Me.ctxPositionY.Name = "ctxPositionY"
      Me.ctxPositionY.ReadOnly = True
      Me.ctxPositionY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxPositionY.Visible = False
      Me.ctxPositionY.Width = 5
      '
      'ctxScale
      '
      Me.ctxScale.DataPropertyName = "Scale"
      Me.ctxScale.HeaderText = "Scale"
      Me.ctxScale.Name = "ctxScale"
      Me.ctxScale.ReadOnly = True
      Me.ctxScale.Visible = False
      '
      'cchChanged
      '
      Me.cchChanged.DataPropertyName = "Changed"
      Me.cchChanged.FalseValue = System.Windows.Forms.CheckState.Unchecked
      Me.cchChanged.HeaderText = "שונה"
      Me.cchChanged.IndeterminateValue = System.Windows.Forms.CheckState.Indeterminate
      Me.cchChanged.Name = "cchChanged"
      Me.cchChanged.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
      Me.cchChanged.TrueValue = System.Windows.Forms.CheckState.Checked
      Me.cchChanged.Width = 48
      '
      'ccbLayer
      '
      Me.ccbLayer.AutoComplete = False
      Me.ccbLayer.DataPropertyName = "Layer"
      Me.ccbLayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.ccbLayer.HeaderText = "שכבה"
      Me.ccbLayer.Name = "ccbLayer"
      Me.ccbLayer.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
      '
      'frmEditBlockRefs
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(1116, 473)
      Me.Controls.Add(Me.Button4)
      Me.Controls.Add(Me.chkAdditionalBlock)
      Me.Controls.Add(Me.cmdImportExcel)
      Me.Controls.Add(Me.chkFirstRow_Tag)
      Me.Controls.Add(Me.Label2)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.Button3)
      Me.Controls.Add(Me.cmbBlockLayers)
      Me.Controls.Add(Me.lblRecCount)
      Me.Controls.Add(Me.Button2)
      Me.Controls.Add(Me.cmdDispSelSet)
      Me.Controls.Add(Me.cmdRemoveFilter)
      Me.Controls.Add(Me.GroupBox1)
      Me.Controls.Add(Me.cmdDispSelectionSet)
      Me.Controls.Add(Me.rdbTag)
      Me.Controls.Add(Me.rdbPrompt)
      Me.Controls.Add(Me.Button1)
      Me.Controls.Add(Me.cmdUpdateAll)
      Me.Controls.Add(Me.cmdUpdateCurrent)
      Me.Controls.Add(Me.cmdFindBlockRef)
      Me.Controls.Add(Me.cmdSetValue)
      Me.Controls.Add(Me.txtValue)
      Me.Controls.Add(Me.cmdZoom)
      Me.Controls.Add(Me.dgvMain)
      Me.Controls.Add(Me.cmbBlockList)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Name = "frmEditBlockRefs"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.RightToLeftLayout = True
      Me.Text = "BlockRefs"
      Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.GroupBox1.ResumeLayout(False)
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
	Private WithEvents cmbBlockList As System.Windows.Forms.ComboBox
	Private WithEvents dgvMain As System.Windows.Forms.DataGridView
	Private WithEvents cmdZoom As System.Windows.Forms.Button
	Private WithEvents cmdSetValue As System.Windows.Forms.Button
	Private WithEvents txtValue As System.Windows.Forms.TextBox
	Private WithEvents cmdFindBlockRef As System.Windows.Forms.Button
	Private WithEvents cmdUpdateCurrent As System.Windows.Forms.Button
	Private WithEvents cmdUpdateAll As System.Windows.Forms.Button
	Private WithEvents Button1 As System.Windows.Forms.Button
	Private WithEvents rdbPrompt As System.Windows.Forms.RadioButton
	Private WithEvents rdbTag As System.Windows.Forms.RadioButton
	Private WithEvents cmdDispSelectionSet As System.Windows.Forms.Button
	Private WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Private WithEvents rdbFilter As System.Windows.Forms.RadioButton
	Private WithEvents rdbSelection As System.Windows.Forms.RadioButton
	Private WithEvents cmdRemoveFilter As System.Windows.Forms.Button
	Private WithEvents cmdDispSelSet As System.Windows.Forms.Button
	Private WithEvents Button2 As System.Windows.Forms.Button
    Private WithEvents lblRecCount As System.Windows.Forms.Label
   Private WithEvents cmbBlockLayers As System.Windows.Forms.ComboBox
   Friend WithEvents Button3 As System.Windows.Forms.Button
   Friend WithEvents Label1 As System.Windows.Forms.Label
   Friend WithEvents Label2 As System.Windows.Forms.Label
   Private WithEvents chkFirstRow_Tag As System.Windows.Forms.CheckBox
   Private WithEvents cmdImportExcel As System.Windows.Forms.Button
   Private WithEvents chkAdditionalBlock As System.Windows.Forms.CheckBox
   Private WithEvents Button4 As System.Windows.Forms.Button
   Friend WithEvents ctxObjectID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxAddObjectID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxPositionX As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxPositionY As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxScale As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents cchChanged As System.Windows.Forms.DataGridViewCheckBoxColumn
   Friend WithEvents ccbLayer As System.Windows.Forms.DataGridViewComboBoxColumn
End Class
