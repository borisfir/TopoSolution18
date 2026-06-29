
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
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTplnView))
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.tlbTop = New System.Windows.Forms.ToolStrip()
		Me.tbbClose = New System.Windows.Forms.ToolStripButton()
		Me.tbbExcel = New System.Windows.Forms.ToolStripButton()
		Me.tbbSaveAll = New System.Windows.Forms.ToolStripButton()
		Me.tbbSaveSelected = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.tbbErasePaint = New System.Windows.Forms.ToolStripButton()
		Me.tbbPaintAll = New System.Windows.Forms.ToolStripButton()
		Me.ddbPaintScale = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tsmiLayerByLanduse = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiBypassLine = New System.Windows.Forms.ToolStripMenuItem()
		Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
		Me.tbbFill = New System.Windows.Forms.ToolStripButton()
		Me.tbbColorScheme = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.tbbZoomPgon = New System.Windows.Forms.ToolStripButton()
		Me.ddbZoomPgon = New System.Windows.Forms.ToolStripDropDownButton()
		Me.tsmiParcel = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsmiLot = New System.Windows.Forms.ToolStripMenuItem()
		Me.tbbSetInitView = New System.Windows.Forms.ToolStripButton()
		Me.tbbPolygonPrevious = New System.Windows.Forms.ToolStripButton()
		Me.tbbPolygonNext = New System.Windows.Forms.ToolStripButton()
		Me.tsbRemoveFilter = New System.Windows.Forms.ToolStripButton()
		Me.tsbFilterBySelect = New System.Windows.Forms.ToolStripButton()
		Me.tbbBaseSort = New System.Windows.Forms.ToolStripButton()
		Me.tcbData = New System.Windows.Forms.ToolStripComboBox()
		Me.tbbInPlan = New System.Windows.Forms.ToolStripButton()
		Me.tcbFilter = New System.Windows.Forms.ToolStripComboBox()
		Me.txtTolerance = New System.Windows.Forms.ToolStripTextBox()
		Me.tbbApplyFilter = New System.Windows.Forms.ToolStripButton()
		Me.cmdFindPgonLoop = New System.Windows.Forms.Button()
		Me.cmdFindPgon = New System.Windows.Forms.Button()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.tlbTop.SuspendLayout()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		DataGridViewCellStyle1.BackColor = System.Drawing.Color.GhostWhite
		Me.dgvMain.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 26)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 24
		Me.dgvMain.Size = New System.Drawing.Size(843, 346)
		Me.dgvMain.TabIndex = 1
		'
		'tlbTop
		'
		Me.tlbTop.Dock = System.Windows.Forms.DockStyle.None
		Me.tlbTop.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tbbClose, Me.tbbExcel, Me.tbbSaveAll, Me.tbbSaveSelected, Me.ToolStripSeparator2, Me.tbbErasePaint, Me.tbbPaintAll, Me.ddbPaintScale, Me.tbbFill, Me.tbbColorScheme, Me.ToolStripSeparator1, Me.tbbZoomPgon, Me.ddbZoomPgon, Me.tbbSetInitView, Me.tbbPolygonPrevious, Me.tbbPolygonNext, Me.tsbRemoveFilter, Me.tsbFilterBySelect, Me.tbbBaseSort, Me.tcbData, Me.tbbInPlan, Me.tcbFilter, Me.txtTolerance, Me.tbbApplyFilter})
		Me.tlbTop.Location = New System.Drawing.Point(0, 0)
		Me.tlbTop.Name = "tlbTop"
		Me.tlbTop.Size = New System.Drawing.Size(775, 25)
		Me.tlbTop.TabIndex = 2
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
		'tbbExcel
		'
		Me.tbbExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbExcel.Image = CType(resources.GetObject("tbbExcel.Image"), System.Drawing.Image)
		Me.tbbExcel.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbExcel.Name = "tbbExcel"
		Me.tbbExcel.Size = New System.Drawing.Size(23, 22)
		'
		'tbbSaveAll
		'
		Me.tbbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbSaveAll.Image = CType(resources.GetObject("tbbSaveAll.Image"), System.Drawing.Image)
		Me.tbbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbSaveAll.Name = "tbbSaveAll"
		Me.tbbSaveAll.Size = New System.Drawing.Size(23, 22)
		'
		'tbbSaveSelected
		'
		Me.tbbSaveSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbSaveSelected.Image = CType(resources.GetObject("tbbSaveSelected.Image"), System.Drawing.Image)
		Me.tbbSaveSelected.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbSaveSelected.Name = "tbbSaveSelected"
		Me.tbbSaveSelected.Size = New System.Drawing.Size(23, 22)
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
		'
		'tbbErasePaint
		'
		Me.tbbErasePaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbErasePaint.Image = CType(resources.GetObject("tbbErasePaint.Image"), System.Drawing.Image)
		Me.tbbErasePaint.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbErasePaint.Name = "tbbErasePaint"
		Me.tbbErasePaint.Size = New System.Drawing.Size(23, 22)
		'
		'tbbPaintAll
		'
		Me.tbbPaintAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbPaintAll.Image = CType(resources.GetObject("tbbPaintAll.Image"), System.Drawing.Image)
		Me.tbbPaintAll.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbPaintAll.Name = "tbbPaintAll"
		Me.tbbPaintAll.Size = New System.Drawing.Size(23, 22)
		'
		'ddbPaintScale
		'
		Me.ddbPaintScale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.ddbPaintScale.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiLayerByLanduse, Me.tsmiBypassLine, Me.ToolStripSeparator3})
		Me.ddbPaintScale.Image = CType(resources.GetObject("ddbPaintScale.Image"), System.Drawing.Image)
		Me.ddbPaintScale.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbPaintScale.Name = "ddbPaintScale"
		Me.ddbPaintScale.Size = New System.Drawing.Size(13, 22)
		Me.ddbPaintScale.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal
		'
		'tsmiLayerByLanduse
		'
		Me.tsmiLayerByLanduse.Checked = True
		Me.tsmiLayerByLanduse.CheckOnClick = True
		Me.tsmiLayerByLanduse.CheckState = System.Windows.Forms.CheckState.Checked
		Me.tsmiLayerByLanduse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsmiLayerByLanduse.Name = "tsmiLayerByLanduse"
		Me.tsmiLayerByLanduse.Size = New System.Drawing.Size(163, 22)
		Me.tsmiLayerByLanduse.Text = "שכבות לפי יעודים"
		'
		'tsmiBypassLine
		'
		Me.tsmiBypassLine.Checked = True
		Me.tsmiBypassLine.CheckOnClick = True
		Me.tsmiBypassLine.CheckState = System.Windows.Forms.CheckState.Checked
		Me.tsmiBypassLine.Name = "tsmiBypassLine"
		Me.tsmiBypassLine.Size = New System.Drawing.Size(163, 22)
		Me.tsmiBypassLine.Text = "קו עקפי"
		'
		'ToolStripSeparator3
		'
		Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
		Me.ToolStripSeparator3.Size = New System.Drawing.Size(160, 6)
		'
		'tbbFill
		'
		Me.tbbFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbFill.Image = CType(resources.GetObject("tbbFill.Image"), System.Drawing.Image)
		Me.tbbFill.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbFill.Name = "tbbFill"
		Me.tbbFill.Size = New System.Drawing.Size(23, 22)
		Me.tbbFill.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
		Me.tbbFill.ToolTipText = "Paint polygon"
		'
		'tbbColorScheme
		'
		Me.tbbColorScheme.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tbbColorScheme.Image = CType(resources.GetObject("tbbColorScheme.Image"), System.Drawing.Image)
		Me.tbbColorScheme.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tbbColorScheme.Name = "tbbColorScheme"
		Me.tbbColorScheme.Size = New System.Drawing.Size(23, 22)
		Me.tbbColorScheme.ToolTipText = "Color Scheme"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
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
		'ddbZoomPgon
		'
		Me.ddbZoomPgon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None
		Me.ddbZoomPgon.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmiParcel, Me.tsmiLot})
		Me.ddbZoomPgon.Image = CType(resources.GetObject("ddbZoomPgon.Image"), System.Drawing.Image)
		Me.ddbZoomPgon.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
		Me.ddbZoomPgon.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ddbZoomPgon.Name = "ddbZoomPgon"
		Me.ddbZoomPgon.Size = New System.Drawing.Size(13, 22)
		'
		'tsmiParcel
		'
		Me.tsmiParcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
		Me.tsmiParcel.Name = "tsmiParcel"
		Me.tsmiParcel.Size = New System.Drawing.Size(180, 22)
		Me.tsmiParcel.Text = "חלקה"
		'
		'tsmiLot
		'
		Me.tsmiLot.Name = "tsmiLot"
		Me.tsmiLot.Size = New System.Drawing.Size(180, 22)
		Me.tsmiLot.Text = "מגרש"
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
		'tsbRemoveFilter
		'
		Me.tsbRemoveFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbRemoveFilter.Image = CType(resources.GetObject("tsbRemoveFilter.Image"), System.Drawing.Image)
		Me.tsbRemoveFilter.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbRemoveFilter.Name = "tsbRemoveFilter"
		Me.tsbRemoveFilter.Size = New System.Drawing.Size(23, 22)
		'
		'tsbFilterBySelect
		'
		Me.tsbFilterBySelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbFilterBySelect.Image = CType(resources.GetObject("tsbFilterBySelect.Image"), System.Drawing.Image)
		Me.tsbFilterBySelect.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbFilterBySelect.Name = "tsbFilterBySelect"
		Me.tsbFilterBySelect.Size = New System.Drawing.Size(23, 22)
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
		'tcbData
		'
		Me.tcbData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
		Me.tcbData.AutoSize = False
		Me.tcbData.AutoToolTip = True
		Me.tcbData.DropDownWidth = 168
		Me.tcbData.MaxDropDownItems = 99
		Me.tcbData.Name = "tcbData"
		Me.tcbData.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.tcbData.Size = New System.Drawing.Size(144, 23)
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
		'cmdFindPgonLoop
		'
		Me.cmdFindPgonLoop.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdFindPgonLoop.Enabled = False
		Me.cmdFindPgonLoop.FlatStyle = System.Windows.Forms.FlatStyle.Popup
		Me.cmdFindPgonLoop.Image = CType(resources.GetObject("cmdFindPgonLoop.Image"), System.Drawing.Image)
		Me.cmdFindPgonLoop.Location = New System.Drawing.Point(780, 2)
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
		Me.cmdFindPgon.Location = New System.Drawing.Point(804, 2)
		Me.cmdFindPgon.Name = "cmdFindPgon"
		Me.cmdFindPgon.Size = New System.Drawing.Size(24, 22)
		Me.cmdFindPgon.TabIndex = 3
		Me.cmdFindPgon.UseVisualStyleBackColor = True
		'
		'frmTplnView
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(843, 372)
		Me.Controls.Add(Me.cmdFindPgonLoop)
		Me.Controls.Add(Me.cmdFindPgon)
		Me.Controls.Add(Me.tlbTop)
		Me.Controls.Add(Me.dgvMain)
		Me.Name = "frmTplnView"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
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
   Private WithEvents tcbData As System.Windows.Forms.ToolStripComboBox
   Private WithEvents txtTolerance As System.Windows.Forms.ToolStripTextBox
   Private WithEvents tcbFilter As System.Windows.Forms.ToolStripComboBox
   Private WithEvents tbbPolygonNext As System.Windows.Forms.ToolStripButton
   Private WithEvents cmdFindPgon As System.Windows.Forms.Button
   Private WithEvents tbbZoomPgon As System.Windows.Forms.ToolStripButton
   Private WithEvents cmdFindPgonLoop As System.Windows.Forms.Button
   '     Friend WithEvents BBBBBB As System.Windows.Forms.ToolStripButton


   Private WithEvents tbbFill As System.Windows.Forms.ToolStripButton
   Private WithEvents tbbSaveSelected As System.Windows.Forms.ToolStripButton
   Private WithEvents tbbSaveAll As System.Windows.Forms.ToolStripButton
   Private tmiPaintScale() As System.Windows.Forms.ToolStripMenuItem
   Friend WithEvents tbbPaintAll As System.Windows.Forms.ToolStripButton
   Private WithEvents ddbPaintScale As System.Windows.Forms.ToolStripDropDownButton
   Private WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents tbbColorScheme As System.Windows.Forms.ToolStripButton
   Private WithEvents tbbExcel As System.Windows.Forms.ToolStripButton
   Private WithEvents ddbZoomPgon As System.Windows.Forms.ToolStripDropDownButton
   Private WithEvents tsmiParcel As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsmiLot As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents tsbFilterBySelect As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbRemoveFilter As System.Windows.Forms.ToolStripButton
   Private WithEvents tbbErasePaint As System.Windows.Forms.ToolStripButton
   Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
   Private WithEvents tsmiLayerByLanduse As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsmiBypassLine As System.Windows.Forms.ToolStripMenuItem

   Private Sub tcbData_TextChanged(oSender As System.Object, e As EventArgs) Handles tcbData.TextChanged

   End Sub
End Class
