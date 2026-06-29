<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmOwnership
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()>
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
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOwnership))
		Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdClearPaint = New System.Windows.Forms.Button()
		Me.cmdPaintParcels = New System.Windows.Forms.Button()
		Me.Button3 = New System.Windows.Forms.Button()
		Me.cmdRefreshData = New System.Windows.Forms.Button()
		Me.chkAllParcels = New System.Windows.Forms.CheckBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.cmbPaintScale = New System.Windows.Forms.ComboBox()
		Me.cmdOpenEditor = New System.Windows.Forms.Button()
		Me.chkOwnersColumnsVisible = New System.Windows.Forms.CheckBox()
		Me.chkNoteColumnsVisible = New System.Windows.Forms.CheckBox()
		Me.cmdDrawLegend = New System.Windows.Forms.Button()
		Me.cmdCheckData = New System.Windows.Forms.Button()
		Me.cmdExcelReport = New System.Windows.Forms.Button()
		Me.txtTotalArea = New System.Windows.Forms.TextBox()
		Me.cmdZoom = New System.Windows.Forms.Button()
		Me.cmdCalc = New System.Windows.Forms.Button()
		Me.cmbPaintAngle = New System.Windows.Forms.ComboBox()
		Me.cmdSelectParcelByPoint = New System.Windows.Forms.Button()
		Me.cmdSelectByPick = New System.Windows.Forms.Button()
		Me.chkUseBalance = New System.Windows.Forms.CheckBox()
		Me.chkPaintingA = New System.Windows.Forms.CheckBox()
		Me.ctxBlockNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBlockAddNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxParcelNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxParcelLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchIsAnalitic = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxParcelInArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchDevelAuthority = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchKKL = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchIsrState = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchLocalAuthority = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchPrivate = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchKKLAdd = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchRegulation = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchIsNotRegulated = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxOwnerIDs = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxOwnersAdd = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchNoteEdited = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchParagraph19 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxParagraph19Area = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchLeasing = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxLeasingArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchParagraph5 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchParagraph126 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchSharedHouse = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchMortgage = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchForeclosure = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchVerdict = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchAntiqueSite = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxPaintAngle = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchParagraph123 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchRegulation29 = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchRoadOrdinance = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchDemolitionOrder = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchParagraph11a = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxBlockNo, Me.ctxBlockAddNo, Me.ctxParcelNo, Me.ctxParcelLegalArea, Me.cchIsAnalitic, Me.ctxParcelInArea, Me.cchDevelAuthority, Me.cchKKL, Me.cchIsrState, Me.cchLocalAuthority, Me.cchPrivate, Me.cchKKLAdd, Me.cchRegulation, Me.cchIsNotRegulated, Me.ctxOwnerIDs, Me.ctxOwnersAdd, Me.ctxStatus, Me.ctxNote, Me.cchNoteEdited, Me.cchParagraph19, Me.ctxParagraph19Area, Me.cchLeasing, Me.ctxLeasingArea, Me.cchParagraph5, Me.cchParagraph126, Me.cchSharedHouse, Me.cchMortgage, Me.cchForeclosure, Me.cchVerdict, Me.cchAntiqueSite, Me.ctxPaintAngle, Me.cchParagraph123, Me.cchRegulation29, Me.cchRoadOrdinance, Me.cchDemolitionOrder, Me.cchParagraph11a})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 44)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.Size = New System.Drawing.Size(1308, 405)
		Me.dgvMain.TabIndex = 0
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Image = Global.TopoUI.My.Resources.Resources.Cancel16
		Me.cmdCancel.Location = New System.Drawing.Point(1238, 8)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(28, 28)
		Me.cmdCancel.TabIndex = 6
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Image = Global.TopoUI.My.Resources.Resources.OK16
		Me.cmdOK.Location = New System.Drawing.Point(1204, 8)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(28, 28)
		Me.cmdOK.TabIndex = 5
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
		Me.cmdExit.Location = New System.Drawing.Point(1272, 9)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(24, 25)
		Me.cmdExit.TabIndex = 10
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdClearPaint
		'
		Me.cmdClearPaint.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdClearPaint.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdClearPaint.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
		Me.cmdClearPaint.Location = New System.Drawing.Point(1004, 7)
		Me.cmdClearPaint.Name = "cmdClearPaint"
		Me.cmdClearPaint.Size = New System.Drawing.Size(28, 28)
		Me.cmdClearPaint.TabIndex = 11
		Me.cmdClearPaint.UseVisualStyleBackColor = True
		'
		'cmdPaintParcels
		'
		Me.cmdPaintParcels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdPaintParcels.Enabled = False
		Me.cmdPaintParcels.Image = CType(resources.GetObject("cmdPaintParcels.Image"), System.Drawing.Image)
		Me.cmdPaintParcels.Location = New System.Drawing.Point(970, 7)
		Me.cmdPaintParcels.Name = "cmdPaintParcels"
		Me.cmdPaintParcels.Size = New System.Drawing.Size(28, 28)
		Me.cmdPaintParcels.TabIndex = 12
		Me.cmdPaintParcels.UseVisualStyleBackColor = True
		'
		'Button3
		'
		Me.Button3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.Button3.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.Button3.Image = Global.TopoUI.My.Resources.Resources.Copy16
		Me.Button3.Location = New System.Drawing.Point(1170, 9)
		Me.Button3.Name = "Button3"
		Me.Button3.Size = New System.Drawing.Size(28, 28)
		Me.Button3.TabIndex = 13
		Me.Button3.UseVisualStyleBackColor = True
		Me.Button3.Visible = False
		'
		'cmdRefreshData
		'
		Me.cmdRefreshData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdRefreshData.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdRefreshData.Image = Global.TopoUI.My.Resources.Resources.Refresh22Tr
		Me.cmdRefreshData.Location = New System.Drawing.Point(1134, 8)
		Me.cmdRefreshData.Name = "cmdRefreshData"
		Me.cmdRefreshData.Size = New System.Drawing.Size(30, 30)
		Me.cmdRefreshData.TabIndex = 14
		Me.cmdRefreshData.UseVisualStyleBackColor = True
		'
		'chkAllParcels
		'
		Me.chkAllParcels.AutoSize = True
		Me.chkAllParcels.Location = New System.Drawing.Point(158, 13)
		Me.chkAllParcels.Name = "chkAllParcels"
		Me.chkAllParcels.Size = New System.Drawing.Size(85, 17)
		Me.chkAllParcels.TabIndex = 15
		Me.chkAllParcels.Text = "כל החלקות"
		Me.chkAllParcels.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(707, 18)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(70, 13)
		Me.Label1.TabIndex = 20
		Me.Label1.Text = "קנ""מ צביעה:"
		'
		'cmbPaintScale
		'
		Me.cmbPaintScale.DisplayMember = "Name"
		Me.cmbPaintScale.FormattingEnabled = True
		Me.cmbPaintScale.Location = New System.Drawing.Point(783, 14)
		Me.cmbPaintScale.Name = "cmbPaintScale"
		Me.cmbPaintScale.Size = New System.Drawing.Size(82, 21)
		Me.cmbPaintScale.TabIndex = 19
		Me.cmbPaintScale.ValueMember = "ID"
		'
		'cmdOpenEditor
		'
		Me.cmdOpenEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOpenEditor.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOpenEditor.Image = CType(resources.GetObject("cmdOpenEditor.Image"), System.Drawing.Image)
		Me.cmdOpenEditor.Location = New System.Drawing.Point(1038, 9)
		Me.cmdOpenEditor.Name = "cmdOpenEditor"
		Me.cmdOpenEditor.Size = New System.Drawing.Size(28, 28)
		Me.cmdOpenEditor.TabIndex = 21
		Me.cmdOpenEditor.UseVisualStyleBackColor = True
		'
		'chkOwnersColumnsVisible
		'
		Me.chkOwnersColumnsVisible.AutoSize = True
		Me.chkOwnersColumnsVisible.Checked = True
		Me.chkOwnersColumnsVisible.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkOwnersColumnsVisible.Location = New System.Drawing.Point(40, 24)
		Me.chkOwnersColumnsVisible.Name = "chkOwnersColumnsVisible"
		Me.chkOwnersColumnsVisible.Size = New System.Drawing.Size(100, 17)
		Me.chkOwnersColumnsVisible.TabIndex = 22
		Me.chkOwnersColumnsVisible.Text = "עמודות בעלים"
		Me.chkOwnersColumnsVisible.UseVisualStyleBackColor = True
		'
		'chkNoteColumnsVisible
		'
		Me.chkNoteColumnsVisible.AutoSize = True
		Me.chkNoteColumnsVisible.Checked = True
		Me.chkNoteColumnsVisible.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkNoteColumnsVisible.Location = New System.Drawing.Point(40, 2)
		Me.chkNoteColumnsVisible.Name = "chkNoteColumnsVisible"
		Me.chkNoteColumnsVisible.Size = New System.Drawing.Size(101, 17)
		Me.chkNoteColumnsVisible.TabIndex = 23
		Me.chkNoteColumnsVisible.Text = "עמודות הערות"
		Me.chkNoteColumnsVisible.UseVisualStyleBackColor = True
		'
		'cmdDrawLegend
		'
		Me.cmdDrawLegend.Location = New System.Drawing.Point(541, 10)
		Me.cmdDrawLegend.Name = "cmdDrawLegend"
		Me.cmdDrawLegend.Size = New System.Drawing.Size(51, 23)
		Me.cmdDrawLegend.TabIndex = 25
		Me.cmdDrawLegend.Text = "מקרא"
		Me.cmdDrawLegend.UseVisualStyleBackColor = True
		'
		'cmdCheckData
		'
		Me.cmdCheckData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCheckData.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdCheckData.Image = Global.TopoUI.My.Resources.Resources.Check16Tr
		Me.cmdCheckData.Location = New System.Drawing.Point(1098, 8)
		Me.cmdCheckData.Name = "cmdCheckData"
		Me.cmdCheckData.Size = New System.Drawing.Size(30, 30)
		Me.cmdCheckData.TabIndex = 26
		Me.cmdCheckData.UseVisualStyleBackColor = True
		'
		'cmdExcelReport
		'
		Me.cmdExcelReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdExcelReport.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdExcelReport.Enabled = False
		Me.cmdExcelReport.Image = CType(resources.GetObject("cmdExcelReport.Image"), System.Drawing.Image)
		Me.cmdExcelReport.Location = New System.Drawing.Point(936, 7)
		Me.cmdExcelReport.Name = "cmdExcelReport"
		Me.cmdExcelReport.Size = New System.Drawing.Size(28, 28)
		Me.cmdExcelReport.TabIndex = 27
		Me.cmdExcelReport.UseVisualStyleBackColor = True
		'
		'txtTotalArea
		'
		Me.txtTotalArea.Location = New System.Drawing.Point(262, 9)
		Me.txtTotalArea.Name = "txtTotalArea"
		Me.txtTotalArea.ReadOnly = True
		Me.txtTotalArea.Size = New System.Drawing.Size(58, 21)
		Me.txtTotalArea.TabIndex = 28
		'
		'cmdZoom
		'
		Me.cmdZoom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdZoom.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdZoom.Image = Global.TopoUI.My.Resources.Resources.PrintPreviewHL1
		Me.cmdZoom.Location = New System.Drawing.Point(347, 5)
		Me.cmdZoom.Name = "cmdZoom"
		Me.cmdZoom.Size = New System.Drawing.Size(30, 30)
		Me.cmdZoom.TabIndex = 29
		Me.cmdZoom.UseVisualStyleBackColor = True
		'
		'cmdCalc
		'
		Me.cmdCalc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCalc.Image = Global.TopoUI.My.Resources.Resources.Invert121
		Me.cmdCalc.Location = New System.Drawing.Point(1069, 10)
		Me.cmdCalc.Margin = New System.Windows.Forms.Padding(0)
		Me.cmdCalc.Name = "cmdCalc"
		Me.cmdCalc.Size = New System.Drawing.Size(26, 24)
		Me.cmdCalc.TabIndex = 31
		Me.cmdCalc.UseVisualStyleBackColor = True
		'
		'cmbPaintAngle
		'
		Me.cmbPaintAngle.FormattingEnabled = True
		Me.cmbPaintAngle.Location = New System.Drawing.Point(648, 12)
		Me.cmbPaintAngle.Name = "cmbPaintAngle"
		Me.cmbPaintAngle.Size = New System.Drawing.Size(40, 21)
		Me.cmbPaintAngle.TabIndex = 32
		'
		'cmdSelectParcelByPoint
		'
		Me.cmdSelectParcelByPoint.Image = Global.TopoUI.My.Resources.Resources._Select
		Me.cmdSelectParcelByPoint.Location = New System.Drawing.Point(613, 10)
		Me.cmdSelectParcelByPoint.Name = "cmdSelectParcelByPoint"
		Me.cmdSelectParcelByPoint.Size = New System.Drawing.Size(29, 23)
		Me.cmdSelectParcelByPoint.TabIndex = 33
		Me.cmdSelectParcelByPoint.Text = "מקרא"
		Me.cmdSelectParcelByPoint.UseVisualStyleBackColor = True
		'
		'cmdSelectByPick
		'
		Me.cmdSelectByPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSelectByPick.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdSelectByPick.Image = Global.TopoUI.My.Resources.Resources.XSDSchema_ImportIcon
		Me.cmdSelectByPick.Location = New System.Drawing.Point(383, 5)
		Me.cmdSelectByPick.Name = "cmdSelectByPick"
		Me.cmdSelectByPick.Size = New System.Drawing.Size(30, 30)
		Me.cmdSelectByPick.TabIndex = 34
		Me.cmdSelectByPick.UseVisualStyleBackColor = True
		'
		'chkUseBalance
		'
		Me.chkUseBalance.AutoSize = True
		Me.chkUseBalance.CheckAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.chkUseBalance.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.chkUseBalance.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.chkUseBalance.Location = New System.Drawing.Point(882, 13)
		Me.chkUseBalance.Name = "chkUseBalance"
		Me.chkUseBalance.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.chkUseBalance.Size = New System.Drawing.Size(53, 17)
		Me.chkUseBalance.TabIndex = 35
		Me.chkUseBalance.Text = " % איזון"
		Me.chkUseBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		Me.chkUseBalance.UseVisualStyleBackColor = True
		'
		'chkPaintingA
		'
		Me.chkPaintingA.AutoSize = True
		Me.chkPaintingA.Location = New System.Drawing.Point(434, 13)
		Me.chkPaintingA.Name = "chkPaintingA"
		Me.chkPaintingA.Size = New System.Drawing.Size(71, 17)
		Me.chkPaintingA.TabIndex = 36
		Me.chkPaintingA.Text = "צביעת א"
		Me.chkPaintingA.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
		Me.chkPaintingA.UseVisualStyleBackColor = True
		'
		'ctxBlockNo
		'
		Me.ctxBlockNo.DataPropertyName = "BlockNo"
		Me.ctxBlockNo.Frozen = True
		Me.ctxBlockNo.HeaderText = "גוש"
		Me.ctxBlockNo.Name = "ctxBlockNo"
		Me.ctxBlockNo.ReadOnly = True
		Me.ctxBlockNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBlockNo.Width = 60
		'
		'ctxBlockAddNo
		'
		Me.ctxBlockAddNo.DataPropertyName = "BlockAddNo"
		Me.ctxBlockAddNo.Frozen = True
		Me.ctxBlockAddNo.HeaderText = "ת'"
		Me.ctxBlockAddNo.Name = "ctxBlockAddNo"
		Me.ctxBlockAddNo.ReadOnly = True
		Me.ctxBlockAddNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBlockAddNo.Width = 5
		'
		'ctxParcelNo
		'
		Me.ctxParcelNo.DataPropertyName = "ParcelNo"
		Me.ctxParcelNo.Frozen = True
		Me.ctxParcelNo.HeaderText = "חלקה"
		Me.ctxParcelNo.Name = "ctxParcelNo"
		Me.ctxParcelNo.ReadOnly = True
		Me.ctxParcelNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxParcelNo.Width = 60
		'
		'ctxParcelLegalArea
		'
		Me.ctxParcelLegalArea.DataPropertyName = "ParcelLegalArea"
		DataGridViewCellStyle5.Format = "N0"
		DataGridViewCellStyle5.NullValue = Nothing
		Me.ctxParcelLegalArea.DefaultCellStyle = DataGridViewCellStyle5
		Me.ctxParcelLegalArea.Frozen = True
		Me.ctxParcelLegalArea.HeaderText = "שטח חל' רשום"
		Me.ctxParcelLegalArea.Name = "ctxParcelLegalArea"
		Me.ctxParcelLegalArea.ReadOnly = True
		Me.ctxParcelLegalArea.ToolTipText = "שטח חלקה רשום"
		Me.ctxParcelLegalArea.Width = 78
		'
		'cchIsAnalitic
		'
		Me.cchIsAnalitic.DataPropertyName = "IsAnalitic"
		Me.cchIsAnalitic.Frozen = True
		Me.cchIsAnalitic.HeaderText = "אנלי טית"
		Me.cchIsAnalitic.Name = "cchIsAnalitic"
		Me.cchIsAnalitic.ToolTipText = "אנליטית"
		Me.cchIsAnalitic.Width = 40
		'
		'ctxParcelInArea
		'
		Me.ctxParcelInArea.DataPropertyName = "ParcelInLegalArea"
		DataGridViewCellStyle6.Format = "N0"
		DataGridViewCellStyle6.NullValue = Nothing
		Me.ctxParcelInArea.DefaultCellStyle = DataGridViewCellStyle6
		Me.ctxParcelInArea.Frozen = True
		Me.ctxParcelInArea.HeaderText = "שטח חלקה בחל' חדשה"
		Me.ctxParcelInArea.Name = "ctxParcelInArea"
		Me.ctxParcelInArea.ReadOnly = True
		Me.ctxParcelInArea.ToolTipText = "שטח חלקה בחוקה חדשה"
		Me.ctxParcelInArea.Width = 88
		'
		'cchDevelAuthority
		'
		Me.cchDevelAuthority.DataPropertyName = "DevelAuthority"
		Me.cchDevelAuthority.FalseValue = ""
		Me.cchDevelAuthority.Frozen = True
		Me.cchDevelAuthority.HeaderText = "רשות הפיתוח"
		Me.cchDevelAuthority.Name = "cchDevelAuthority"
		Me.cchDevelAuthority.ToolTipText = "רשות הפיתוח"
		Me.cchDevelAuthority.TrueValue = ""
		Me.cchDevelAuthority.Width = 48
		'
		'cchKKL
		'
		Me.cchKKL.DataPropertyName = "KKL"
		Me.cchKKL.FalseValue = ""
		Me.cchKKL.Frozen = True
		Me.cchKKL.HeaderText = "קק""ל"
		Me.cchKKL.Name = "cchKKL"
		Me.cchKKL.ToolTipText = "קרן קיימת לישראל"
		Me.cchKKL.TrueValue = ""
		Me.cchKKL.Width = 48
		'
		'cchIsrState
		'
		Me.cchIsrState.DataPropertyName = "IsrState"
		Me.cchIsrState.Frozen = True
		Me.cchIsrState.HeaderText = "מדינת ישראל"
		Me.cchIsrState.Name = "cchIsrState"
		Me.cchIsrState.ToolTipText = "מדינת ישראל"
		Me.cchIsrState.Width = 48
		'
		'cchLocalAuthority
		'
		Me.cchLocalAuthority.DataPropertyName = "LocalAuthority"
		Me.cchLocalAuthority.Frozen = True
		Me.cchLocalAuthority.HeaderText = "רשות מקומית"
		Me.cchLocalAuthority.Name = "cchLocalAuthority"
		Me.cchLocalAuthority.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchLocalAuthority.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchLocalAuthority.ToolTipText = "רשות מקומית"
		Me.cchLocalAuthority.Width = 48
		'
		'cchPrivate
		'
		Me.cchPrivate.DataPropertyName = "Private"
		Me.cchPrivate.Frozen = True
		Me.cchPrivate.HeaderText = "פרטיים"
		Me.cchPrivate.Name = "cchPrivate"
		Me.cchPrivate.ToolTipText = "פרטיים"
		Me.cchPrivate.Width = 48
		'
		'cchKKLAdd
		'
		Me.cchKKLAdd.DataPropertyName = "KKLAdd"
		Me.cchKKLAdd.Frozen = True
		Me.cchKKLAdd.HeaderText = "הימ נותא"
		Me.cchKKLAdd.Name = "cchKKLAdd"
		Me.cchKKLAdd.ToolTipText = "הימנותא"
		Me.cchKKLAdd.Width = 48
		'
		'cchRegulation
		'
		Me.cchRegulation.DataPropertyName = "Regulation"
		Me.cchRegulation.Frozen = True
		Me.cchRegulation.HeaderText = "בהליך רישום"
		Me.cchRegulation.Name = "cchRegulation"
		Me.cchRegulation.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchRegulation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchRegulation.ToolTipText = "בהליך רישום"
		Me.cchRegulation.Width = 50
		'
		'cchIsNotRegulated
		'
		Me.cchIsNotRegulated.DataPropertyName = "IsNotRegulated"
		Me.cchIsNotRegulated.DividerWidth = 3
		Me.cchIsNotRegulated.Frozen = True
		Me.cchIsNotRegulated.HeaderText = "לא ידועה"
		Me.cchIsNotRegulated.Name = "cchIsNotRegulated"
		Me.cchIsNotRegulated.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchIsNotRegulated.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchIsNotRegulated.ToolTipText = "לא ידועה"
		Me.cchIsNotRegulated.Width = 48
		'
		'ctxOwnerIDs
		'
		Me.ctxOwnerIDs.DataPropertyName = "OwnerIDs"
		Me.ctxOwnerIDs.Frozen = True
		Me.ctxOwnerIDs.HeaderText = "*Own"
		Me.ctxOwnerIDs.Name = "ctxOwnerIDs"
		Me.ctxOwnerIDs.Visible = False
		Me.ctxOwnerIDs.Width = 40
		'
		'ctxOwnersAdd
		'
		Me.ctxOwnersAdd.DataPropertyName = "OwnerText"
		Me.ctxOwnersAdd.Frozen = True
		Me.ctxOwnersAdd.HeaderText = "בעלים נוספים"
		Me.ctxOwnersAdd.Name = "ctxOwnersAdd"
		Me.ctxOwnersAdd.Width = 72
		'
		'ctxStatus
		'
		Me.ctxStatus.DividerWidth = 3
		Me.ctxStatus.Frozen = True
		Me.ctxStatus.HeaderText = "תקין"
		Me.ctxStatus.Name = "ctxStatus"
		Me.ctxStatus.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ctxStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxStatus.Width = 40
		'
		'ctxNote
		'
		Me.ctxNote.DataPropertyName = "Note"
		Me.ctxNote.Frozen = True
		Me.ctxNote.HeaderText = "הערה"
		Me.ctxNote.Name = "ctxNote"
		'
		'cchNoteEdited
		'
		Me.cchNoteEdited.DataPropertyName = "NoteEdited"
		Me.cchNoteEdited.Frozen = True
		Me.cchNoteEdited.HeaderText = "ערוך"
		Me.cchNoteEdited.Name = "cchNoteEdited"
		Me.cchNoteEdited.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchNoteEdited.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchNoteEdited.Width = 36
		'
		'cchParagraph19
		'
		Me.cchParagraph19.DataPropertyName = "Paragraph19"
		Me.cchParagraph19.Frozen = True
		Me.cchParagraph19.HeaderText = "סע' 19"
		Me.cchParagraph19.Name = "cchParagraph19"
		Me.cchParagraph19.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchParagraph19.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchParagraph19.ToolTipText = "סעיף 19"
		Me.cchParagraph19.Width = 48
		'
		'ctxParagraph19Area
		'
		Me.ctxParagraph19Area.DataPropertyName = "Paragraph19Area"
		DataGridViewCellStyle7.Format = "N0"
		Me.ctxParagraph19Area.DefaultCellStyle = DataGridViewCellStyle7
		Me.ctxParagraph19Area.Frozen = True
		Me.ctxParagraph19Area.HeaderText = "שטח סע' 19"
		Me.ctxParagraph19Area.Name = "ctxParagraph19Area"
		Me.ctxParagraph19Area.ToolTipText = "שטח סעיף 19"
		Me.ctxParagraph19Area.Width = 66
		'
		'cchLeasing
		'
		Me.cchLeasing.DataPropertyName = "Leasing"
		Me.cchLeasing.Frozen = True
		Me.cchLeasing.HeaderText = "חכירה"
		Me.cchLeasing.Name = "cchLeasing"
		Me.cchLeasing.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchLeasing.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchLeasing.Width = 48
		'
		'ctxLeasingArea
		'
		Me.ctxLeasingArea.DataPropertyName = "LeasingArea"
		DataGridViewCellStyle8.Format = "N0"
		Me.ctxLeasingArea.DefaultCellStyle = DataGridViewCellStyle8
		Me.ctxLeasingArea.Frozen = True
		Me.ctxLeasingArea.HeaderText = "שטח החכירה"
		Me.ctxLeasingArea.Name = "ctxLeasingArea"
		Me.ctxLeasingArea.Width = 66
		'
		'cchParagraph5
		'
		Me.cchParagraph5.DataPropertyName = "Paragraph5"
		Me.cchParagraph5.Frozen = True
		Me.cchParagraph5.HeaderText = "סעיף 5 ו-7"
		Me.cchParagraph5.Name = "cchParagraph5"
		Me.cchParagraph5.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchParagraph5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchParagraph5.Width = 48
		'
		'cchParagraph126
		'
		Me.cchParagraph126.DataPropertyName = "Paragraph126"
		Me.cchParagraph126.Frozen = True
		Me.cchParagraph126.HeaderText = "סעיף 126"
		Me.cchParagraph126.Name = "cchParagraph126"
		Me.cchParagraph126.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchParagraph126.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchParagraph126.Width = 48
		'
		'cchSharedHouse
		'
		Me.cchSharedHouse.DataPropertyName = "SharedHouse"
		Me.cchSharedHouse.Frozen = True
		Me.cchSharedHouse.HeaderText = "בית משותף"
		Me.cchSharedHouse.Name = "cchSharedHouse"
		Me.cchSharedHouse.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchSharedHouse.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchSharedHouse.Width = 50
		'
		'cchMortgage
		'
		Me.cchMortgage.DataPropertyName = "Mortgage"
		Me.cchMortgage.Frozen = True
		Me.cchMortgage.HeaderText = "משכנתא"
		Me.cchMortgage.Name = "cchMortgage"
		Me.cchMortgage.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchMortgage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchMortgage.Width = 48
		'
		'cchForeclosure
		'
		Me.cchForeclosure.DataPropertyName = "Foreclosure"
		Me.cchForeclosure.Frozen = True
		Me.cchForeclosure.HeaderText = "צו עיקול"
		Me.cchForeclosure.Name = "cchForeclosure"
		Me.cchForeclosure.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchForeclosure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchForeclosure.Width = 48
		'
		'cchVerdict
		'
		Me.cchVerdict.DataPropertyName = "Verdict"
		Me.cchVerdict.Frozen = True
		Me.cchVerdict.HeaderText = "פסק דין"
		Me.cchVerdict.Name = "cchVerdict"
		Me.cchVerdict.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchVerdict.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchVerdict.Width = 48
		'
		'cchAntiqueSite
		'
		Me.cchAntiqueSite.DataPropertyName = "AntiqueSite"
		Me.cchAntiqueSite.Frozen = True
		Me.cchAntiqueSite.HeaderText = "אתר עתיקות"
		Me.cchAntiqueSite.Name = "cchAntiqueSite"
		Me.cchAntiqueSite.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchAntiqueSite.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchAntiqueSite.ToolTipText = "הערה בדבר אתר עתיקות"
		Me.cchAntiqueSite.Width = 48
		'
		'ctxPaintAngle
		'
		Me.ctxPaintAngle.DataPropertyName = "PaintAngle"
		Me.ctxPaintAngle.HeaderText = "זווית"
		Me.ctxPaintAngle.Name = "ctxPaintAngle"
		Me.ctxPaintAngle.Width = 48
		'
		'cchParagraph123
		'
		Me.cchParagraph123.DataPropertyName = "Paragraph123"
		Me.cchParagraph123.HeaderText = "סעיף 123"
		Me.cchParagraph123.Name = "cchParagraph123"
		Me.cchParagraph123.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchParagraph123.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchParagraph123.Width = 48
		'
		'cchRegulation29
		'
		Me.cchRegulation29.DataPropertyName = "Regulation29"
		Me.cchRegulation29.HeaderText = "תקנה 29"
		Me.cchRegulation29.Name = "cchRegulation29"
		Me.cchRegulation29.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchRegulation29.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchRegulation29.Width = 48
		'
		'cchRoadOrdinance
		'
		Me.cchRoadOrdinance.DataPropertyName = "RoadOrdinance"
		Me.cchRoadOrdinance.HeaderText = "פקודת הדרכים"
		Me.cchRoadOrdinance.Name = "cchRoadOrdinance"
		Me.cchRoadOrdinance.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchRoadOrdinance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
		Me.cchRoadOrdinance.Width = 64
		'
		'cchDemolitionOrder
		'
		Me.cchDemolitionOrder.DataPropertyName = "DemolitionOrder"
		Me.cchDemolitionOrder.HeaderText = "צו הריסה"
		Me.cchDemolitionOrder.Name = "cchDemolitionOrder"
		Me.cchDemolitionOrder.Width = 48
		'
		'cchParagraph11a
		'
		Me.cchParagraph11a.DataPropertyName = "Paragraph11a"
		Me.cchParagraph11a.HeaderText = "סעיף 11א"
		Me.cchParagraph11a.Name = "cchParagraph11a"
		Me.cchParagraph11a.Width = 48
		'
		'frmOwnership
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1308, 449)
		Me.Controls.Add(Me.chkPaintingA)
		Me.Controls.Add(Me.chkUseBalance)
		Me.Controls.Add(Me.cmdSelectByPick)
		Me.Controls.Add(Me.cmdSelectParcelByPoint)
		Me.Controls.Add(Me.cmbPaintAngle)
		Me.Controls.Add(Me.cmdCalc)
		Me.Controls.Add(Me.cmdZoom)
		Me.Controls.Add(Me.txtTotalArea)
		Me.Controls.Add(Me.cmdExcelReport)
		Me.Controls.Add(Me.cmdCheckData)
		Me.Controls.Add(Me.cmdDrawLegend)
		Me.Controls.Add(Me.chkNoteColumnsVisible)
		Me.Controls.Add(Me.chkOwnersColumnsVisible)
		Me.Controls.Add(Me.cmdOpenEditor)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.cmbPaintScale)
		Me.Controls.Add(Me.chkAllParcels)
		Me.Controls.Add(Me.cmdRefreshData)
		Me.Controls.Add(Me.Button3)
		Me.Controls.Add(Me.cmdPaintParcels)
		Me.Controls.Add(Me.cmdClearPaint)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmOwnership"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "בעלויות"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents dgvMain As DataGridView
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdOK As Button
	Private WithEvents cmdExit As Button
	Private WithEvents cmdClearPaint As Button
	Private WithEvents cmdPaintParcels As Button
	Private WithEvents Button3 As Button
	Private WithEvents cmdRefreshData As Button
	Private WithEvents chkAllParcels As CheckBox
	Private WithEvents Label1 As Label
	Private WithEvents cmbPaintScale As ComboBox
	Private WithEvents cmdOpenEditor As Button
	Private WithEvents chkOwnersColumnsVisible As CheckBox
	Private WithEvents chkNoteColumnsVisible As CheckBox
	Private WithEvents cmdDrawLegend As Button
	Private WithEvents cmdCheckData As Button
	Private WithEvents cmdExcelReport As Button
	Private WithEvents txtTotalArea As TextBox
	Private WithEvents cmdZoom As Button
	Private WithEvents cmdCalc As Button
	Private WithEvents cmbPaintAngle As ComboBox
	Private WithEvents cmdSelectParcelByPoint As Button
	Private WithEvents cmdSelectByPick As Button
	Private WithEvents chkUseBalance As CheckBox
	Private WithEvents chkPaintingA As CheckBox
	Friend WithEvents ctxBlockNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxBlockAddNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxParcelNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxParcelLegalArea As DataGridViewTextBoxColumn
	Friend WithEvents cchIsAnalitic As DataGridViewCheckBoxColumn
	Friend WithEvents ctxParcelInArea As DataGridViewTextBoxColumn
	Friend WithEvents cchDevelAuthority As DataGridViewCheckBoxColumn
	Friend WithEvents cchKKL As DataGridViewCheckBoxColumn
	Friend WithEvents cchIsrState As DataGridViewCheckBoxColumn
	Friend WithEvents cchLocalAuthority As DataGridViewCheckBoxColumn
	Friend WithEvents cchPrivate As DataGridViewCheckBoxColumn
	Friend WithEvents cchKKLAdd As DataGridViewCheckBoxColumn
	Friend WithEvents cchRegulation As DataGridViewCheckBoxColumn
	Friend WithEvents cchIsNotRegulated As DataGridViewCheckBoxColumn
	Friend WithEvents ctxOwnerIDs As DataGridViewTextBoxColumn
	Friend WithEvents ctxOwnersAdd As DataGridViewTextBoxColumn
	Friend WithEvents ctxStatus As DataGridViewTextBoxColumn
	Friend WithEvents ctxNote As DataGridViewTextBoxColumn
	Friend WithEvents cchNoteEdited As DataGridViewCheckBoxColumn
	Friend WithEvents cchParagraph19 As DataGridViewCheckBoxColumn
	Friend WithEvents ctxParagraph19Area As DataGridViewTextBoxColumn
	Friend WithEvents cchLeasing As DataGridViewCheckBoxColumn
	Friend WithEvents ctxLeasingArea As DataGridViewTextBoxColumn
	Friend WithEvents cchParagraph5 As DataGridViewCheckBoxColumn
	Friend WithEvents cchParagraph126 As DataGridViewCheckBoxColumn
	Friend WithEvents cchSharedHouse As DataGridViewCheckBoxColumn
	Friend WithEvents cchMortgage As DataGridViewCheckBoxColumn
	Friend WithEvents cchForeclosure As DataGridViewCheckBoxColumn
	Friend WithEvents cchVerdict As DataGridViewCheckBoxColumn
	Friend WithEvents cchAntiqueSite As DataGridViewCheckBoxColumn
	Friend WithEvents ctxPaintAngle As DataGridViewTextBoxColumn
	Friend WithEvents cchParagraph123 As DataGridViewCheckBoxColumn
	Friend WithEvents cchRegulation29 As DataGridViewCheckBoxColumn
	Friend WithEvents cchRoadOrdinance As DataGridViewCheckBoxColumn
	Friend WithEvents cchDemolitionOrder As DataGridViewTextBoxColumn
	Friend WithEvents cchParagraph11a As DataGridViewTextBoxColumn
End Class
