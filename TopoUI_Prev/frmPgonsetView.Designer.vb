<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPgonSetView
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
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPgonSetView))
      Me.dgvPolygons = New System.Windows.Forms.DataGridView()
      Me.cmdZoom = New System.Windows.Forms.Button()
      Me.chkSortByName = New System.Windows.Forms.CheckBox()
      Me.lblBlocksOutsideCount = New System.Windows.Forms.Label()
      Me.rdbIntersections = New System.Windows.Forms.RadioButton()
      Me.rdbBlocksOutside = New System.Windows.Forms.RadioButton()
      Me.rdbPolygons = New System.Windows.Forms.RadioButton()
      Me.lblIntersectionCount = New System.Windows.Forms.Label()
      Me.lblVoidCount = New System.Windows.Forms.Label()
      Me.rdbVoids = New System.Windows.Forms.RadioButton()
      Me.cmdExit = New System.Windows.Forms.Button()
      Me.cmdEraseErrors = New System.Windows.Forms.Button()
      Me.cmdMarkErrors = New System.Windows.Forms.Button()
      Me.cmdRun = New System.Windows.Forms.Button()
      Me.cmdPaint = New System.Windows.Forms.Button()
      Me.chkLayers = New System.Windows.Forms.CheckBox()
      Me.chkWorkAreaBorder = New System.Windows.Forms.CheckBox()
      Me.ctxIsProper = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      Me.ctxEntityNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxCentroidCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGroupID = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGroupAddID = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxName = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxAttribA = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxEntityNo_C = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGroupID_C = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxGroupAddID_C = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxName_C = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxIntersectCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxLength = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxOrder = New System.Windows.Forms.DataGridViewTextBoxColumn()
      CType(Me.dgvPolygons, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'dgvPolygons
      '
      Me.dgvPolygons.AllowUserToAddRows = False
      Me.dgvPolygons.AllowUserToDeleteRows = False
      DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
      DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
      DataGridViewCellStyle1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
      DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
      DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
      DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
      Me.dgvPolygons.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
      Me.dgvPolygons.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvPolygons.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxIsProper, Me.ctxEntityNo, Me.ctxCentroidCount, Me.ctxGroupID, Me.ctxGroupAddID, Me.ctxName, Me.ctxLegalArea, Me.ctxAttribA, Me.ctxEntityNo_C, Me.ctxGroupID_C, Me.ctxGroupAddID_C, Me.ctxName_C, Me.ctxIntersectCount, Me.ctxArea, Me.ctxLength, Me.ctxOrder})
      Me.dgvPolygons.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.dgvPolygons.Location = New System.Drawing.Point(0, 39)
      Me.dgvPolygons.Name = "dgvPolygons"
      Me.dgvPolygons.ReadOnly = True
      Me.dgvPolygons.Size = New System.Drawing.Size(931, 512)
      Me.dgvPolygons.TabIndex = 0
      '
      'cmdZoom
      '
      Me.cmdZoom.Image = Global.TopoUI.My.Resources.Resources._65
      Me.cmdZoom.Location = New System.Drawing.Point(42, 8)
      Me.cmdZoom.Name = "cmdZoom"
      Me.cmdZoom.Size = New System.Drawing.Size(25, 25)
      Me.cmdZoom.TabIndex = 3
      Me.cmdZoom.UseVisualStyleBackColor = True
      '
      'chkSortByName
      '
      Me.chkSortByName.AutoSize = True
      Me.chkSortByName.Checked = True
      Me.chkSortByName.CheckState = System.Windows.Forms.CheckState.Checked
      Me.chkSortByName.Location = New System.Drawing.Point(124, 8)
      Me.chkSortByName.Name = "chkSortByName"
      Me.chkSortByName.Size = New System.Drawing.Size(66, 18)
      Me.chkSortByName.TabIndex = 4
      Me.chkSortByName.Text = "לפי שם"
      Me.chkSortByName.UseVisualStyleBackColor = True
      '
      'lblBlocksOutsideCount
      '
      Me.lblBlocksOutsideCount.AutoSize = True
      Me.lblBlocksOutsideCount.BackColor = System.Drawing.SystemColors.Control
      Me.lblBlocksOutsideCount.Location = New System.Drawing.Point(472, 8)
      Me.lblBlocksOutsideCount.Name = "lblBlocksOutsideCount"
      Me.lblBlocksOutsideCount.Size = New System.Drawing.Size(14, 14)
      Me.lblBlocksOutsideCount.TabIndex = 6
      Me.lblBlocksOutsideCount.Text = "0"
      '
      'rdbIntersections
      '
      Me.rdbIntersections.AutoSize = True
      Me.rdbIntersections.Location = New System.Drawing.Point(495, 8)
      Me.rdbIntersections.Name = "rdbIntersections"
      Me.rdbIntersections.Size = New System.Drawing.Size(101, 18)
      Me.rdbIntersections.TabIndex = 8
      Me.rdbIntersections.Text = "נקודות חיתוך:"
      Me.rdbIntersections.UseVisualStyleBackColor = True
      '
      'rdbBlocksOutside
      '
      Me.rdbBlocksOutside.AutoSize = True
      Me.rdbBlocksOutside.Location = New System.Drawing.Point(315, 8)
      Me.rdbBlocksOutside.Name = "rdbBlocksOutside"
      Me.rdbBlocksOutside.Size = New System.Drawing.Size(154, 18)
      Me.rdbBlocksOutside.TabIndex = 9
      Me.rdbBlocksOutside.Text = "בלוקים מחוץ לפוליגונים:"
      Me.rdbBlocksOutside.UseVisualStyleBackColor = True
      '
      'rdbPolygons
      '
      Me.rdbPolygons.AutoSize = True
      Me.rdbPolygons.Checked = True
      Me.rdbPolygons.Location = New System.Drawing.Point(224, 8)
      Me.rdbPolygons.Name = "rdbPolygons"
      Me.rdbPolygons.Size = New System.Drawing.Size(71, 18)
      Me.rdbPolygons.TabIndex = 10
      Me.rdbPolygons.TabStop = True
      Me.rdbPolygons.Text = "פוליגונים"
      Me.rdbPolygons.UseVisualStyleBackColor = True
      '
      'lblIntersectionCount
      '
      Me.lblIntersectionCount.AutoSize = True
      Me.lblIntersectionCount.BackColor = System.Drawing.SystemColors.Control
      Me.lblIntersectionCount.Location = New System.Drawing.Point(598, 8)
      Me.lblIntersectionCount.Name = "lblIntersectionCount"
      Me.lblIntersectionCount.Size = New System.Drawing.Size(14, 14)
      Me.lblIntersectionCount.TabIndex = 12
      Me.lblIntersectionCount.Text = "0"
      '
      'lblVoidCount
      '
      Me.lblVoidCount.AutoSize = True
      Me.lblVoidCount.BackColor = System.Drawing.SystemColors.Control
      Me.lblVoidCount.Location = New System.Drawing.Point(700, 8)
      Me.lblVoidCount.Name = "lblVoidCount"
      Me.lblVoidCount.Size = New System.Drawing.Size(14, 14)
      Me.lblVoidCount.TabIndex = 15
      Me.lblVoidCount.Text = "0"
      '
      'rdbVoids
      '
      Me.rdbVoids.AutoSize = True
      Me.rdbVoids.Location = New System.Drawing.Point(635, 8)
      Me.rdbVoids.Name = "rdbVoids"
      Me.rdbVoids.Size = New System.Drawing.Size(62, 18)
      Me.rdbVoids.TabIndex = 14
      Me.rdbVoids.Text = "חללים:"
      Me.rdbVoids.UseVisualStyleBackColor = True
      '
      'cmdExit
      '
      Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
      Me.cmdExit.Location = New System.Drawing.Point(73, 8)
      Me.cmdExit.Name = "cmdExit"
      Me.cmdExit.Size = New System.Drawing.Size(33, 25)
      Me.cmdExit.TabIndex = 16
      Me.cmdExit.UseVisualStyleBackColor = True
      '
      'cmdEraseErrors
      '
      Me.cmdEraseErrors.Image = CType(resources.GetObject("cmdEraseErrors.Image"), System.Drawing.Image)
      Me.cmdEraseErrors.Location = New System.Drawing.Point(889, 5)
      Me.cmdEraseErrors.Name = "cmdEraseErrors"
      Me.cmdEraseErrors.Size = New System.Drawing.Size(33, 25)
      Me.cmdEraseErrors.TabIndex = 13
      Me.cmdEraseErrors.UseVisualStyleBackColor = True
      '
      'cmdMarkErrors
      '
      Me.cmdMarkErrors.Image = Global.TopoUI.My.Resources.Resources._Select
      Me.cmdMarkErrors.Location = New System.Drawing.Point(850, 5)
      Me.cmdMarkErrors.Name = "cmdMarkErrors"
      Me.cmdMarkErrors.Size = New System.Drawing.Size(33, 25)
      Me.cmdMarkErrors.TabIndex = 11
      Me.cmdMarkErrors.UseVisualStyleBackColor = True
      '
      'cmdRun
      '
      Me.cmdRun.Image = Global.TopoUI.My.Resources.Resources.RefreshC
      Me.cmdRun.Location = New System.Drawing.Point(12, 8)
      Me.cmdRun.Name = "cmdRun"
      Me.cmdRun.Size = New System.Drawing.Size(24, 25)
      Me.cmdRun.TabIndex = 1
      Me.cmdRun.UseVisualStyleBackColor = True
      '
      'cmdPaint
      '
      Me.cmdPaint.Image = CType(resources.GetObject("cmdPaint.Image"), System.Drawing.Image)
      Me.cmdPaint.Location = New System.Drawing.Point(815, 5)
      Me.cmdPaint.Name = "cmdPaint"
      Me.cmdPaint.Size = New System.Drawing.Size(29, 25)
      Me.cmdPaint.TabIndex = 17
      Me.cmdPaint.UseVisualStyleBackColor = True
      '
      'chkLayers
      '
      Me.chkLayers.Appearance = System.Windows.Forms.Appearance.Button
      Me.chkLayers.AutoSize = True
      Me.chkLayers.Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
      Me.chkLayers.Location = New System.Drawing.Point(787, 6)
      Me.chkLayers.Name = "chkLayers"
      Me.chkLayers.Size = New System.Drawing.Size(22, 22)
      Me.chkLayers.TabIndex = 19
      Me.chkLayers.UseVisualStyleBackColor = True
      '
      'chkWorkAreaBorder
      '
      Me.chkWorkAreaBorder.Appearance = System.Windows.Forms.Appearance.Button
      Me.chkWorkAreaBorder.Checked = True
      Me.chkWorkAreaBorder.CheckState = System.Windows.Forms.CheckState.Checked
      Me.chkWorkAreaBorder.Image = Global.TopoUI.My.Resources.Resources.Rect16
      Me.chkWorkAreaBorder.Location = New System.Drawing.Point(754, 5)
      Me.chkWorkAreaBorder.Name = "chkWorkAreaBorder"
      Me.chkWorkAreaBorder.Size = New System.Drawing.Size(25, 25)
      Me.chkWorkAreaBorder.TabIndex = 20
      Me.chkWorkAreaBorder.UseVisualStyleBackColor = True
      '
      'ctxIsProper
      '
      Me.ctxIsProper.DataPropertyName = "IsProper"
      Me.ctxIsProper.HeaderText = "תקין"
      Me.ctxIsProper.Name = "ctxIsProper"
      Me.ctxIsProper.ReadOnly = True
      Me.ctxIsProper.Width = 40
      '
      'ctxEntityNo
      '
      Me.ctxEntityNo.DataPropertyName = "EntityNo"
      Me.ctxEntityNo.HeaderText = "מס'"
      Me.ctxEntityNo.Name = "ctxEntityNo"
      Me.ctxEntityNo.ReadOnly = True
      Me.ctxEntityNo.Width = 56
      '
      'ctxCentroidCount
      '
      Me.ctxCentroidCount.DataPropertyName = "CentroidCount"
      Me.ctxCentroidCount.HeaderText = "מס' בל'"
      Me.ctxCentroidCount.Name = "ctxCentroidCount"
      Me.ctxCentroidCount.ReadOnly = True
      Me.ctxCentroidCount.Width = 76
      '
      'ctxGroupID
      '
      Me.ctxGroupID.DataPropertyName = "GroupID"
      Me.ctxGroupID.HeaderText = "מס' גוש"
      Me.ctxGroupID.Name = "ctxGroupID"
      Me.ctxGroupID.ReadOnly = True
      Me.ctxGroupID.Width = 76
      '
      'ctxGroupAddID
      '
      Me.ctxGroupAddID.DataPropertyName = "GroupAddID"
      Me.ctxGroupAddID.HeaderText = "תוספת"
      Me.ctxGroupAddID.Name = "ctxGroupAddID"
      Me.ctxGroupAddID.ReadOnly = True
      Me.ctxGroupAddID.Visible = False
      Me.ctxGroupAddID.Width = 60
      '
      'ctxName
      '
      Me.ctxName.DataPropertyName = "Name"
      Me.ctxName.HeaderText = "חלקה"
      Me.ctxName.Name = "ctxName"
      Me.ctxName.ReadOnly = True
      Me.ctxName.Width = 64
      '
      'ctxLegalArea
      '
      Me.ctxLegalArea.DataPropertyName = "LegalArea"
      Me.ctxLegalArea.HeaderText = "שטח רשום"
      Me.ctxLegalArea.Name = "ctxLegalArea"
      Me.ctxLegalArea.ReadOnly = True
      Me.ctxLegalArea.Width = 88
      '
      'ctxAttribA
      '
      Me.ctxAttribA.HeaderText = "---"
      Me.ctxAttribA.Name = "ctxAttribA"
      Me.ctxAttribA.ReadOnly = True
      '
      'ctxEntityNo_C
      '
      Me.ctxEntityNo_C.DataPropertyName = "EntityNo_C"
      Me.ctxEntityNo_C.HeaderText = "מס'"
      Me.ctxEntityNo_C.Name = "ctxEntityNo_C"
      Me.ctxEntityNo_C.ReadOnly = True
      Me.ctxEntityNo_C.Visible = False
      Me.ctxEntityNo_C.Width = 56
      '
      'ctxGroupID_C
      '
      Me.ctxGroupID_C.DataPropertyName = "GroupID_C"
      Me.ctxGroupID_C.HeaderText = "מס' גוש"
      Me.ctxGroupID_C.Name = "ctxGroupID_C"
      Me.ctxGroupID_C.ReadOnly = True
      Me.ctxGroupID_C.Visible = False
      Me.ctxGroupID_C.Width = 76
      '
      'ctxGroupAddID_C
      '
      Me.ctxGroupAddID_C.DataPropertyName = "GroupAddID_C"
      Me.ctxGroupAddID_C.HeaderText = "תוספת"
      Me.ctxGroupAddID_C.Name = "ctxGroupAddID_C"
      Me.ctxGroupAddID_C.ReadOnly = True
      Me.ctxGroupAddID_C.Visible = False
      Me.ctxGroupAddID_C.Width = 60
      '
      'ctxName_C
      '
      Me.ctxName_C.DataPropertyName = "Name_C"
      Me.ctxName_C.HeaderText = "חלקה"
      Me.ctxName_C.Name = "ctxName_C"
      Me.ctxName_C.ReadOnly = True
      Me.ctxName_C.Visible = False
      Me.ctxName_C.Width = 64
      '
      'ctxIntersectCount
      '
      Me.ctxIntersectCount.DataPropertyName = "IntersectCount"
      Me.ctxIntersectCount.HeaderText = "מס' נק'"
      Me.ctxIntersectCount.Name = "ctxIntersectCount"
      Me.ctxIntersectCount.ReadOnly = True
      Me.ctxIntersectCount.Visible = False
      Me.ctxIntersectCount.Width = 60
      '
      'ctxArea
      '
      Me.ctxArea.DataPropertyName = "Area"
      Me.ctxArea.HeaderText = "שטח"
      Me.ctxArea.Name = "ctxArea"
      Me.ctxArea.ReadOnly = True
      Me.ctxArea.Width = 88
      '
      'ctxLength
      '
      Me.ctxLength.DataPropertyName = "Length"
      Me.ctxLength.HeaderText = "אורך הקו"
      Me.ctxLength.Name = "ctxLength"
      Me.ctxLength.ReadOnly = True
      Me.ctxLength.Width = 88
      '
      'ctxOrder
      '
      Me.ctxOrder.DataPropertyName = "Order"
      Me.ctxOrder.HeaderText = "Order"
      Me.ctxOrder.Name = "ctxOrder"
      Me.ctxOrder.ReadOnly = True
      Me.ctxOrder.Visible = False
      '
      'frmPgonSetView
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(931, 551)
      Me.Controls.Add(Me.chkWorkAreaBorder)
      Me.Controls.Add(Me.chkLayers)
      Me.Controls.Add(Me.cmdPaint)
      Me.Controls.Add(Me.cmdExit)
      Me.Controls.Add(Me.lblVoidCount)
      Me.Controls.Add(Me.rdbVoids)
      Me.Controls.Add(Me.cmdEraseErrors)
      Me.Controls.Add(Me.lblIntersectionCount)
      Me.Controls.Add(Me.cmdMarkErrors)
      Me.Controls.Add(Me.rdbPolygons)
      Me.Controls.Add(Me.rdbBlocksOutside)
      Me.Controls.Add(Me.rdbIntersections)
      Me.Controls.Add(Me.lblBlocksOutsideCount)
      Me.Controls.Add(Me.chkSortByName)
      Me.Controls.Add(Me.cmdZoom)
      Me.Controls.Add(Me.cmdRun)
      Me.Controls.Add(Me.dgvPolygons)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Name = "frmPgonSetView"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.RightToLeftLayout = True
      Me.Text = "בדיקת פוליגונים"
      CType(Me.dgvPolygons, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Private WithEvents dgvPolygons As System.Windows.Forms.DataGridView
   Private WithEvents cmdZoom As System.Windows.Forms.Button
   Private WithEvents chkSortByName As System.Windows.Forms.CheckBox
   Private WithEvents cmdRun As System.Windows.Forms.Button
   Private WithEvents lblBlocksOutsideCount As System.Windows.Forms.Label
   Private WithEvents rdbIntersections As System.Windows.Forms.RadioButton
   Private WithEvents rdbBlocksOutside As System.Windows.Forms.RadioButton
   Private WithEvents rdbPolygons As System.Windows.Forms.RadioButton
   Private WithEvents cmdMarkErrors As System.Windows.Forms.Button
   Private WithEvents lblIntersectionCount As System.Windows.Forms.Label
   Private WithEvents cmdEraseErrors As System.Windows.Forms.Button
   Private WithEvents lblVoidCount As System.Windows.Forms.Label
   Private WithEvents rdbVoids As System.Windows.Forms.RadioButton
   Private WithEvents cmdExit As System.Windows.Forms.Button
   Private WithEvents cmdPaint As System.Windows.Forms.Button
   Private WithEvents chkLayers As System.Windows.Forms.CheckBox
   Private WithEvents chkWorkAreaBorder As System.Windows.Forms.CheckBox
   Friend WithEvents ctxIsProper As System.Windows.Forms.DataGridViewCheckBoxColumn
   Friend WithEvents ctxEntityNo As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxCentroidCount As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGroupID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGroupAddID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxName As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLegalArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxAttribA As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxEntityNo_C As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGroupID_C As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxGroupAddID_C As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxName_C As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxIntersectCount As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLength As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxOrder As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
