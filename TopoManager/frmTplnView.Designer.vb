Namespace TPlanGraph
   <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
   Partial Class frmTplnView
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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTplnView))
			Me.dgvMain = New System.Windows.Forms.DataGridView
			Me.tlbTop = New System.Windows.Forms.ToolStrip
			Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
			Me.tcbData = New System.Windows.Forms.ToolStripComboBox
			Me.tcbFilter = New System.Windows.Forms.ToolStripComboBox
			Me.txtTolerance = New System.Windows.Forms.ToolStripTextBox
			Me.cmdFindPgonLoop = New System.Windows.Forms.Button
			Me.cmdFindPgon = New System.Windows.Forms.Button
			Me.tbbClose = New System.Windows.Forms.ToolStripButton
			Me.tbbSaveAll = New System.Windows.Forms.ToolStripButton
			Me.tbbSaveSelected = New System.Windows.Forms.ToolStripButton
			Me.tbbFill = New System.Windows.Forms.ToolStripButton
			Me.tbbZoomPgon = New System.Windows.Forms.ToolStripButton
			Me.tbbSetInitView = New System.Windows.Forms.ToolStripButton
			Me.tbbPolygonPrevious = New System.Windows.Forms.ToolStripButton
			Me.tbbPolygonNext = New System.Windows.Forms.ToolStripButton
			Me.tbbBaseSort = New System.Windows.Forms.ToolStripButton
			Me.tbbInPlan = New System.Windows.Forms.ToolStripButton
			Me.tbbApplyFilter = New System.Windows.Forms.ToolStripButton
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.tlbTop.SuspendLayout()
			Me.SuspendLayout()
			'
			'dgvMain
			'
			Me.dgvMain.AllowUserToAddRows = False
			Me.dgvMain.AllowUserToDeleteRows = False
			Me.dgvMain.AllowUserToOrderColumns = True
			Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.dgvMain.Location = New System.Drawing.Point(0, 26)
			Me.dgvMain.Name = "dgvMain"
			Me.dgvMain.RowHeadersWidth = 24
			Me.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
			Me.dgvMain.Size = New System.Drawing.Size(767, 346)
			Me.dgvMain.TabIndex = 1
			'
			'tlbTop
			'
			Me.tlbTop.Dock = System.Windows.Forms.DockStyle.None
			Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tbbClose, Me.tbbSaveAll, Me.tbbSaveSelected, Me.tbbFill, Me.ToolStripSeparator1, Me.tbbZoomPgon, Me.tbbSetInitView, Me.tbbPolygonPrevious, Me.tbbPolygonNext, Me.tbbBaseSort, Me.tcbData, Me.tbbInPlan, Me.tcbFilter, Me.txtTolerance, Me.tbbApplyFilter})
			Me.tlbTop.Location = New System.Drawing.Point(0, 0)
			Me.tlbTop.Name = "tlbTop"
			Me.tlbTop.Size = New System.Drawing.Size(574, 25)
			Me.tlbTop.TabIndex = 2
			'
			'ToolStripSeparator1
			'
			Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
			Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
			'
			'tcbData
			'
			Me.tcbData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
			Me.tcbData.AutoSize = False
			Me.tcbData.AutoToolTip = True
			Me.tcbData.DropDownWidth = 168
			Me.tcbData.MaxDropDownItems = 99
			Me.tcbData.Name = "tcbData"
			Me.tcbData.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.tcbData.Size = New System.Drawing.Size(144, 21)
			'
			'tcbFilter
			'
			Me.tcbFilter.DropDownWidth = 192
			Me.tcbFilter.Name = "tcbFilter"
			Me.tcbFilter.Size = New System.Drawing.Size(121, 25)
			'
			'txtTolerance
			'
			Me.txtTolerance.Name = "txtTolerance"
			Me.txtTolerance.RightToLeft = System.Windows.Forms.RightToLeft.No
			Me.txtTolerance.Size = New System.Drawing.Size(32, 25)
			'
			'cmdFindPgonLoop
			'
			Me.cmdFindPgonLoop.Cursor = System.Windows.Forms.Cursors.Hand
			Me.cmdFindPgonLoop.Enabled = False
			Me.cmdFindPgonLoop.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			Me.cmdFindPgonLoop.Image = CType(resources.GetObject("cmdFindPgonLoop.Image"), System.Drawing.Image)
			Me.cmdFindPgonLoop.Location = New System.Drawing.Point(617, 2)
			Me.cmdFindPgonLoop.Name = "cmdFindPgonLoop"
			Me.cmdFindPgonLoop.Size = New System.Drawing.Size(24, 22)
			Me.cmdFindPgonLoop.TabIndex = 4
			Me.cmdFindPgonLoop.UseVisualStyleBackColor = True
			'
			'cmdFindPgon
			'
			Me.cmdFindPgon.Cursor = System.Windows.Forms.Cursors.Hand
			Me.cmdFindPgon.Enabled = False
			Me.cmdFindPgon.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			Me.cmdFindPgon.Image = CType(resources.GetObject("cmdFindPgon.Image"), System.Drawing.Image)
			Me.cmdFindPgon.Location = New System.Drawing.Point(647, 2)
			Me.cmdFindPgon.Name = "cmdFindPgon"
			Me.cmdFindPgon.Size = New System.Drawing.Size(24, 22)
			Me.cmdFindPgon.TabIndex = 3
			Me.cmdFindPgon.UseVisualStyleBackColor = True
			'
			'tbbClose
			'
			Me.tbbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbClose.Image = CType(resources.GetObject("tbbClose.Image"), System.Drawing.Image)
			Me.tbbClose.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbClose.Name = "tbbClose"
			Me.tbbClose.Size = New System.Drawing.Size(23, 22)
			Me.tbbClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbClose.ToolTipText = "סגור"
			'
			'tbbSaveAll
			'
			Me.tbbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbSaveAll.Image = Global.TopoUI.My.Resources.Resources.SaveAllHS
			Me.tbbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbSaveAll.Name = "tbbSaveAll"
			Me.tbbSaveAll.Size = New System.Drawing.Size(23, 22)
			'
			'tbbSaveSelected
			'
			Me.tbbSaveSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbSaveSelected.Image = Global.TopoUI.My.Resources.Resources.saveHS
			Me.tbbSaveSelected.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbSaveSelected.Name = "tbbSaveSelected"
			Me.tbbSaveSelected.Size = New System.Drawing.Size(23, 22)
			'
			'tbbFill
			'
			Me.tbbFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbFill.Image = CType(resources.GetObject("tbbFill.Image"), System.Drawing.Image)
			Me.tbbFill.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbFill.Name = "tbbFill"
			Me.tbbFill.Size = New System.Drawing.Size(23, 22)
			Me.tbbFill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbFill.ToolTipText = "Fil lColor"
			'
			'tbbZoomPgon
			'
			Me.tbbZoomPgon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbZoomPgon.Enabled = False
			Me.tbbZoomPgon.Image = CType(resources.GetObject("tbbZoomPgon.Image"), System.Drawing.Image)
			Me.tbbZoomPgon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			Me.tbbZoomPgon.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbZoomPgon.Name = "tbbZoomPgon"
			Me.tbbZoomPgon.Size = New System.Drawing.Size(23, 22)
			Me.tbbZoomPgon.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbZoomPgon.ToolTipText = "Zoom"
			'
			'tbbSetInitView
			'
			Me.tbbSetInitView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbSetInitView.Image = CType(resources.GetObject("tbbSetInitView.Image"), System.Drawing.Image)
			Me.tbbSetInitView.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbSetInitView.Name = "tbbSetInitView"
			Me.tbbSetInitView.Size = New System.Drawing.Size(23, 22)
			Me.tbbSetInitView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbSetInitView.ToolTipText = "Zoom "
			'
			'tbbPolygonPrevious
			'
			Me.tbbPolygonPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbPolygonPrevious.Enabled = False
			Me.tbbPolygonPrevious.Image = CType(resources.GetObject("tbbPolygonPrevious.Image"), System.Drawing.Image)
			Me.tbbPolygonPrevious.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbPolygonPrevious.Name = "tbbPolygonPrevious"
			Me.tbbPolygonPrevious.Size = New System.Drawing.Size(23, 22)
			Me.tbbPolygonPrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbPolygonPrevious.ToolTipText = "פוליגון קודם"
			'
			'tbbPolygonNext
			'
			Me.tbbPolygonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbPolygonNext.Enabled = False
			Me.tbbPolygonNext.Image = CType(resources.GetObject("tbbPolygonNext.Image"), System.Drawing.Image)
			Me.tbbPolygonNext.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbPolygonNext.Name = "tbbPolygonNext"
			Me.tbbPolygonNext.Size = New System.Drawing.Size(23, 22)
			Me.tbbPolygonNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbPolygonNext.ToolTipText = "פוליגון הבא"
			'
			'tbbBaseSort
			'
			Me.tbbBaseSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbBaseSort.Image = CType(resources.GetObject("tbbBaseSort.Image"), System.Drawing.Image)
			Me.tbbBaseSort.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			Me.tbbBaseSort.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbBaseSort.Name = "tbbBaseSort"
			Me.tbbBaseSort.Size = New System.Drawing.Size(23, 22)
			Me.tbbBaseSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbBaseSort.ToolTipText = "מיון מקורי"
			'
			'tbbInPlan
			'
			Me.tbbInPlan.CheckOnClick = True
			Me.tbbInPlan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbInPlan.Image = CType(resources.GetObject("tbbInPlan.Image"), System.Drawing.Image)
			Me.tbbInPlan.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbInPlan.Name = "tbbInPlan"
			Me.tbbInPlan.Size = New System.Drawing.Size(23, 22)
			Me.tbbInPlan.Text = "מ' מוצע"
			Me.tbbInPlan.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbInPlan.ToolTipText = "שייך לתכנית"
			'
			'tbbApplyFilter
			'
			Me.tbbApplyFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			Me.tbbApplyFilter.Image = CType(resources.GetObject("tbbApplyFilter.Image"), System.Drawing.Image)
			Me.tbbApplyFilter.ImageTransparentColor = System.Drawing.Color.Magenta
			Me.tbbApplyFilter.Name = "tbbApplyFilter"
			Me.tbbApplyFilter.Size = New System.Drawing.Size(23, 22)
			Me.tbbApplyFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
			Me.tbbApplyFilter.ToolTipText = " "
			'
			'frmTplnView
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(767, 372)
			Me.Controls.Add(Me.cmdFindPgonLoop)
			Me.Controls.Add(Me.cmdFindPgon)
			Me.Controls.Add(Me.tlbTop)
			Me.Controls.Add(Me.dgvMain)
			Me.Name = "frmTplnView"
			Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.Text = "טבלאות שטחים"
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
			Me.tlbTop.ResumeLayout(False)
			Me.tlbTop.PerformLayout()
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub
      Private WithEvents dgvMain As System.Windows.Forms.DataGridView
      '   Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
      Private WithEvents tbbInPlan As System.Windows.Forms.ToolStripButton
      Private WithEvents tbbApplyFilter As System.Windows.Forms.ToolStripButton
      Private WithEvents tbbSetInitView As System.Windows.Forms.ToolStripButton
      Private WithEvents tbbClose As System.Windows.Forms.ToolStripButton
      Private WithEvents tbbPolygonPrevious As System.Windows.Forms.ToolStripButton
      Private WithEvents tlbTop As System.Windows.Forms.ToolStrip
      Private WithEvents tbbBaseSort As System.Windows.Forms.ToolStripButton
      Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
      Private WithEvents tcbData As System.Windows.Forms.ToolStripComboBox
      Private WithEvents txtTolerance As System.Windows.Forms.ToolStripTextBox
      Private WithEvents tcbFilter As System.Windows.Forms.ToolStripComboBox
      Private WithEvents tbbPolygonNext As System.Windows.Forms.ToolStripButton
      Private WithEvents cmdFindPgon As System.Windows.Forms.Button
      Private WithEvents tbbZoomPgon As System.Windows.Forms.ToolStripButton
      Private WithEvents cmdFindPgonLoop As System.Windows.Forms.Button
      '     Friend WithEvents BBBBBB As System.Windows.Forms.ToolStripButton

      Private Sub frmTplnView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click

		End Sub
		Private WithEvents tbbFill As System.Windows.Forms.ToolStripButton
		Private WithEvents tbbSaveSelected As System.Windows.Forms.ToolStripButton
		Private WithEvents tbbSaveAll As System.Windows.Forms.ToolStripButton
   End Class
End Namespace