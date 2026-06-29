<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHanitControl
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
      Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmHanitControl))
      Me.dgvStages = New System.Windows.Forms.DataGridView()
      Me.ctxStageNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxParcelCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.cchLayerOn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      Me.cmdCreateHanit = New System.Windows.Forms.Button()
      Me.cmdNextStage = New System.Windows.Forms.Button()
      Me.cmdPreviousStage = New System.Windows.Forms.Button()
      Me.chkAllLayers = New System.Windows.Forms.CheckBox()
      Me.cmdOnlyOne = New System.Windows.Forms.Button()
      Me.cmdClearLayers = New System.Windows.Forms.Button()
      Me.cmdExit = New System.Windows.Forms.Button()
      Me.cmdEraseUnusedPoints = New System.Windows.Forms.Button()
      Me.cmdOpen_Tr_Book = New System.Windows.Forms.Button()
      Me.cmdOpen_Tr_Result = New System.Windows.Forms.Button()
      Me.cmdOpenGen = New System.Windows.Forms.Button()
      Me.cmdInsertRep = New System.Windows.Forms.Button()
      Me.cmdExpExcel = New System.Windows.Forms.Button()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.cmdOpen_PointsSrv = New System.Windows.Forms.Button()
      Me.cmdInsertPointBlocking = New System.Windows.Forms.Button()
      Me.cmdInputPrevVersion = New System.Windows.Forms.Button()
      Me.cmdCleanup = New System.Windows.Forms.Button()
      Me.chkPointsBlocking = New System.Windows.Forms.CheckBox()
      CType(Me.dgvStages, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'dgvStages
      '
      Me.dgvStages.AllowUserToAddRows = False
      Me.dgvStages.AllowUserToDeleteRows = False
      Me.dgvStages.AllowUserToOrderColumns = True
      DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
      DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
      DataGridViewCellStyle1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
      DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
      DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
      DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
      Me.dgvStages.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
      Me.dgvStages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvStages.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxStageNo, Me.ctxParcelCount, Me.cchLayerOn})
      Me.dgvStages.Location = New System.Drawing.Point(14, 90)
      Me.dgvStages.Name = "dgvStages"
      Me.dgvStages.RowHeadersWidth = 24
      Me.dgvStages.Size = New System.Drawing.Size(280, 162)
      Me.dgvStages.TabIndex = 0
      '
      'ctxStageNo
      '
      Me.ctxStageNo.HeaderText = "שלב"
      Me.ctxStageNo.Name = "ctxStageNo"
      Me.ctxStageNo.ReadOnly = True
      Me.ctxStageNo.Width = 60
      '
      'ctxParcelCount
      '
      Me.ctxParcelCount.HeaderText = "מס' חלקות"
      Me.ctxParcelCount.Name = "ctxParcelCount"
      Me.ctxParcelCount.ReadOnly = True
      Me.ctxParcelCount.Width = 92
      '
      'cchLayerOn
      '
      Me.cchLayerOn.FalseValue = System.Windows.Forms.CheckState.Unchecked
      Me.cchLayerOn.HeaderText = "On"
      Me.cchLayerOn.IndeterminateValue = System.Windows.Forms.CheckState.Indeterminate
      Me.cchLayerOn.Name = "cchLayerOn"
      Me.cchLayerOn.TrueValue = System.Windows.Forms.CheckState.Checked
      Me.cchLayerOn.Width = 60
      '
      'cmdCreateHanit
      '
      Me.cmdCreateHanit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdCreateHanit.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
      Me.cmdCreateHanit.Location = New System.Drawing.Point(14, 12)
      Me.cmdCreateHanit.Name = "cmdCreateHanit"
      Me.cmdCreateHanit.Size = New System.Drawing.Size(24, 25)
      Me.cmdCreateHanit.TabIndex = 1
      Me.cmdCreateHanit.UseVisualStyleBackColor = True
      '
      'cmdNextStage
      '
      Me.cmdNextStage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdNextStage.Image = Global.TopoUI.My.Resources.Resources.DataContainer_MoveNextHS_901
      Me.cmdNextStage.Location = New System.Drawing.Point(300, 113)
      Me.cmdNextStage.Name = "cmdNextStage"
      Me.cmdNextStage.Size = New System.Drawing.Size(27, 25)
      Me.cmdNextStage.TabIndex = 2
      Me.cmdNextStage.UseVisualStyleBackColor = True
      '
      'cmdPreviousStage
      '
      Me.cmdPreviousStage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdPreviousStage.Image = Global.TopoUI.My.Resources.Resources.DataContainer_MoveNextHS_80
      Me.cmdPreviousStage.Location = New System.Drawing.Point(300, 80)
      Me.cmdPreviousStage.Name = "cmdPreviousStage"
      Me.cmdPreviousStage.Size = New System.Drawing.Size(27, 25)
      Me.cmdPreviousStage.TabIndex = 4
      Me.cmdPreviousStage.UseVisualStyleBackColor = True
      '
      'chkAllLayers
      '
      Me.chkAllLayers.AutoSize = True
      Me.chkAllLayers.Checked = True
      Me.chkAllLayers.CheckState = System.Windows.Forms.CheckState.Indeterminate
      Me.chkAllLayers.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.chkAllLayers.Location = New System.Drawing.Point(14, 68)
      Me.chkAllLayers.Name = "chkAllLayers"
      Me.chkAllLayers.Size = New System.Drawing.Size(73, 18)
      Me.chkAllLayers.TabIndex = 6
      Me.chkAllLayers.Text = "All Layers"
      Me.chkAllLayers.UseVisualStyleBackColor = True
      '
      'cmdOnlyOne
      '
      Me.cmdOnlyOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdOnlyOne.Image = CType(resources.GetObject("cmdOnlyOne.Image"), System.Drawing.Image)
      Me.cmdOnlyOne.Location = New System.Drawing.Point(142, 48)
      Me.cmdOnlyOne.Name = "cmdOnlyOne"
      Me.cmdOnlyOne.Size = New System.Drawing.Size(24, 25)
      Me.cmdOnlyOne.TabIndex = 7
      Me.cmdOnlyOne.UseVisualStyleBackColor = True
      '
      'cmdClearLayers
      '
      Me.cmdClearLayers.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdClearLayers.Image = Global.TopoUI.My.Resources.Resources._Erase
      Me.cmdClearLayers.Location = New System.Drawing.Point(63, 12)
      Me.cmdClearLayers.Name = "cmdClearLayers"
      Me.cmdClearLayers.Size = New System.Drawing.Size(24, 25)
      Me.cmdClearLayers.TabIndex = 8
      Me.cmdClearLayers.UseVisualStyleBackColor = True
      '
      'cmdExit
      '
      Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
      Me.cmdExit.Location = New System.Drawing.Point(142, 12)
      Me.cmdExit.Name = "cmdExit"
      Me.cmdExit.Size = New System.Drawing.Size(24, 25)
      Me.cmdExit.TabIndex = 9
      Me.cmdExit.UseVisualStyleBackColor = True
      '
      'cmdEraseUnusedPoints
      '
      Me.cmdEraseUnusedPoints.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdEraseUnusedPoints.Image = CType(resources.GetObject("cmdEraseUnusedPoints.Image"), System.Drawing.Image)
      Me.cmdEraseUnusedPoints.Location = New System.Drawing.Point(103, 12)
      Me.cmdEraseUnusedPoints.Name = "cmdEraseUnusedPoints"
      Me.cmdEraseUnusedPoints.Size = New System.Drawing.Size(24, 25)
      Me.cmdEraseUnusedPoints.TabIndex = 10
      Me.cmdEraseUnusedPoints.UseVisualStyleBackColor = True
      '
      'cmdOpen_Tr_Book
      '
      Me.cmdOpen_Tr_Book.Location = New System.Drawing.Point(183, 12)
      Me.cmdOpen_Tr_Book.Name = "cmdOpen_Tr_Book"
      Me.cmdOpen_Tr_Book.Size = New System.Drawing.Size(57, 23)
      Me.cmdOpen_Tr_Book.TabIndex = 11
      Me.cmdOpen_Tr_Book.Text = "Book"
      Me.cmdOpen_Tr_Book.UseVisualStyleBackColor = True
      '
      'cmdOpen_Tr_Result
      '
      Me.cmdOpen_Tr_Result.Location = New System.Drawing.Point(247, 12)
      Me.cmdOpen_Tr_Result.Name = "cmdOpen_Tr_Result"
      Me.cmdOpen_Tr_Result.Size = New System.Drawing.Size(57, 23)
      Me.cmdOpen_Tr_Result.TabIndex = 12
      Me.cmdOpen_Tr_Result.Text = "Result"
      Me.cmdOpen_Tr_Result.UseVisualStyleBackColor = True
      '
      'cmdOpenGen
      '
      Me.cmdOpenGen.Location = New System.Drawing.Point(247, 41)
      Me.cmdOpenGen.Name = "cmdOpenGen"
      Me.cmdOpenGen.Size = New System.Drawing.Size(57, 23)
      Me.cmdOpenGen.TabIndex = 13
      Me.cmdOpenGen.Text = "General"
      Me.cmdOpenGen.UseVisualStyleBackColor = True
      '
      'cmdInsertRep
      '
      Me.cmdInsertRep.Location = New System.Drawing.Point(328, 26)
      Me.cmdInsertRep.Name = "cmdInsertRep"
      Me.cmdInsertRep.Size = New System.Drawing.Size(62, 23)
      Me.cmdInsertRep.TabIndex = 14
      Me.cmdInsertRep.Text = "Autocad"
      Me.cmdInsertRep.UseVisualStyleBackColor = True
      '
      'cmdExpExcel
      '
      Me.cmdExpExcel.Location = New System.Drawing.Point(328, 55)
      Me.cmdExpExcel.Name = "cmdExpExcel"
      Me.cmdExpExcel.Size = New System.Drawing.Size(62, 23)
      Me.cmdExpExcel.TabIndex = 15
      Me.cmdExpExcel.Text = "Excel"
      Me.cmdExpExcel.UseVisualStyleBackColor = True
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(325, 6)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(82, 14)
      Me.Label1.TabIndex = 16
      Me.Label1.Text = "טבלת שטחים"
      '
      'cmdOpen_PointsSrv
      '
      Me.cmdOpen_PointsSrv.Location = New System.Drawing.Point(183, 41)
      Me.cmdOpen_PointsSrv.Name = "cmdOpen_PointsSrv"
      Me.cmdOpen_PointsSrv.Size = New System.Drawing.Size(57, 23)
      Me.cmdOpen_PointsSrv.TabIndex = 17
      Me.cmdOpen_PointsSrv.Text = "Points"
      Me.cmdOpen_PointsSrv.UseVisualStyleBackColor = True
      '
      'cmdInsertPointBlocking
      '
      Me.cmdInsertPointBlocking.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.cmdInsertPointBlocking.Image = Global.TopoUI.My.Resources.Resources._27
      Me.cmdInsertPointBlocking.Location = New System.Drawing.Point(103, 48)
      Me.cmdInsertPointBlocking.Name = "cmdInsertPointBlocking"
      Me.cmdInsertPointBlocking.Size = New System.Drawing.Size(24, 25)
      Me.cmdInsertPointBlocking.TabIndex = 18
      Me.cmdInsertPointBlocking.UseVisualStyleBackColor = True
      '
      'cmdInputPrevVersion
      '
      Me.cmdInputPrevVersion.Location = New System.Drawing.Point(314, 153)
      Me.cmdInputPrevVersion.Name = "cmdInputPrevVersion"
      Me.cmdInputPrevVersion.Size = New System.Drawing.Size(93, 23)
      Me.cmdInputPrevVersion.TabIndex = 19
      Me.cmdInputPrevVersion.Text = "Prev Version"
      Me.cmdInputPrevVersion.UseVisualStyleBackColor = True
      '
      'cmdCleanup
      '
      Me.cmdCleanup.Location = New System.Drawing.Point(314, 192)
      Me.cmdCleanup.Name = "cmdCleanup"
      Me.cmdCleanup.Size = New System.Drawing.Size(93, 23)
      Me.cmdCleanup.TabIndex = 20
      Me.cmdCleanup.Text = "Cleanup"
      Me.cmdCleanup.UseVisualStyleBackColor = True
      Me.cmdCleanup.Visible = False
      '
      'chkPointsBlocking
      '
      Me.chkPointsBlocking.AutoSize = True
      Me.chkPointsBlocking.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.chkPointsBlocking.Location = New System.Drawing.Point(14, 44)
      Me.chkPointsBlocking.Name = "chkPointsBlocking"
      Me.chkPointsBlocking.Size = New System.Drawing.Size(72, 18)
      Me.chkPointsBlocking.TabIndex = 21
      Me.chkPointsBlocking.Text = "All Points"
      Me.chkPointsBlocking.UseVisualStyleBackColor = True
      '
      'frmHanitControl
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(423, 275)
      Me.Controls.Add(Me.chkPointsBlocking)
      Me.Controls.Add(Me.cmdCleanup)
      Me.Controls.Add(Me.cmdInputPrevVersion)
      Me.Controls.Add(Me.cmdInsertPointBlocking)
      Me.Controls.Add(Me.cmdOpen_PointsSrv)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.cmdExpExcel)
      Me.Controls.Add(Me.cmdInsertRep)
      Me.Controls.Add(Me.cmdOpenGen)
      Me.Controls.Add(Me.cmdOpen_Tr_Result)
      Me.Controls.Add(Me.cmdOpen_Tr_Book)
      Me.Controls.Add(Me.cmdEraseUnusedPoints)
      Me.Controls.Add(Me.cmdExit)
      Me.Controls.Add(Me.cmdClearLayers)
      Me.Controls.Add(Me.cmdOnlyOne)
      Me.Controls.Add(Me.chkAllLayers)
      Me.Controls.Add(Me.cmdPreviousStage)
      Me.Controls.Add(Me.cmdNextStage)
      Me.Controls.Add(Me.cmdCreateHanit)
      Me.Controls.Add(Me.dgvStages)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Name = "frmHanitControl"
      Me.Text = "Format Hanit"
      CType(Me.dgvStages, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Private WithEvents cmdCreateHanit As System.Windows.Forms.Button
   Private WithEvents dgvStages As System.Windows.Forms.DataGridView
   Private WithEvents cmdNextStage As System.Windows.Forms.Button
   Private WithEvents cmdPreviousStage As System.Windows.Forms.Button
   Private WithEvents chkAllLayers As System.Windows.Forms.CheckBox
   Private WithEvents cmdOnlyOne As System.Windows.Forms.Button
   Private WithEvents cmdClearLayers As System.Windows.Forms.Button
   Private WithEvents cmdExit As System.Windows.Forms.Button
   Private WithEvents cmdEraseUnusedPoints As System.Windows.Forms.Button
   Private WithEvents cmdOpen_Tr_Book As System.Windows.Forms.Button
   Private WithEvents cmdOpen_Tr_Result As System.Windows.Forms.Button
   Friend WithEvents ctxStageNo As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxParcelCount As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents cchLayerOn As System.Windows.Forms.DataGridViewCheckBoxColumn
   Private WithEvents cmdOpenGen As System.Windows.Forms.Button
   Private WithEvents cmdInsertRep As System.Windows.Forms.Button
   Private WithEvents cmdExpExcel As System.Windows.Forms.Button
   Private WithEvents Label1 As System.Windows.Forms.Label
   Private WithEvents cmdOpen_PointsSrv As System.Windows.Forms.Button
   Private WithEvents cmdInsertPointBlocking As System.Windows.Forms.Button
   Private WithEvents cmdInputPrevVersion As System.Windows.Forms.Button
   Private WithEvents cmdCleanup As System.Windows.Forms.Button
   Private WithEvents chkPointsBlocking As System.Windows.Forms.CheckBox

   
End Class
