<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCalcAreas
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCalcAreas))
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.rdbParcels = New System.Windows.Forms.RadioButton()
		Me.rdbLots = New System.Windows.Forms.RadioButton()
		Me.rdbRegions = New System.Windows.Forms.RadioButton()
		Me.lblRecCount = New System.Windows.Forms.Label()
		Me.lblNotProperRecCount = New System.Windows.Forms.Label()
		Me.cmdExcelReport = New System.Windows.Forms.Button()
		Me.cmdZoom = New System.Windows.Forms.Button()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.ctxBlockFull = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxParcelNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxRegionName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLot = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxCalcAreaFO = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDeltaArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxTolerance = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchProper = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxTopoID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		DataGridViewCellStyle1.BackColor = System.Drawing.Color.GhostWhite
		Me.dgvMain.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxBlockFull, Me.ctxParcelNo, Me.ctxRegionName, Me.ctxLot, Me.ctxLegalArea, Me.ctxCalcAreaFO, Me.ctxArea, Me.ctxDeltaArea, Me.ctxTolerance, Me.ctxDeviation, Me.cchProper, Me.ctxTopoID})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 37)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.ReadOnly = True
		Me.dgvMain.RowHeadersWidth = 24
		Me.dgvMain.Size = New System.Drawing.Size(795, 413)
		Me.dgvMain.TabIndex = 2
		'
		'rdbParcels
		'
		Me.rdbParcels.AutoSize = True
		Me.rdbParcels.Location = New System.Drawing.Point(53, 12)
		Me.rdbParcels.Name = "rdbParcels"
		Me.rdbParcels.Size = New System.Drawing.Size(60, 19)
		Me.rdbParcels.TabIndex = 7
		Me.rdbParcels.TabStop = True
		Me.rdbParcels.Text = "חלקות"
		Me.rdbParcels.UseVisualStyleBackColor = True
		'
		'rdbLots
		'
		Me.rdbLots.AutoSize = True
		Me.rdbLots.Location = New System.Drawing.Point(119, 12)
		Me.rdbLots.Name = "rdbLots"
		Me.rdbLots.Size = New System.Drawing.Size(67, 19)
		Me.rdbLots.TabIndex = 8
		Me.rdbLots.TabStop = True
		Me.rdbLots.Text = "מגרשים"
		Me.rdbLots.UseVisualStyleBackColor = True
		'
		'rdbRegions
		'
		Me.rdbRegions.AutoSize = True
		Me.rdbRegions.Location = New System.Drawing.Point(195, 12)
		Me.rdbRegions.Name = "rdbRegions"
		Me.rdbRegions.Size = New System.Drawing.Size(69, 19)
		Me.rdbRegions.TabIndex = 9
		Me.rdbRegions.TabStop = True
		Me.rdbRegions.Text = "מתחמים"
		Me.rdbRegions.UseVisualStyleBackColor = True
		'
		'lblRecCount
		'
		Me.lblRecCount.AutoSize = True
		Me.lblRecCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.lblRecCount.Location = New System.Drawing.Point(313, 14)
		Me.lblRecCount.Name = "lblRecCount"
		Me.lblRecCount.Size = New System.Drawing.Size(40, 15)
		Me.lblRecCount.TabIndex = 10
		Me.lblRecCount.Text = "Label1"
		'
		'lblNotProperRecCount
		'
		Me.lblNotProperRecCount.AutoSize = True
		Me.lblNotProperRecCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.lblNotProperRecCount.Location = New System.Drawing.Point(492, 14)
		Me.lblNotProperRecCount.Name = "lblNotProperRecCount"
		Me.lblNotProperRecCount.Size = New System.Drawing.Size(42, 15)
		Me.lblNotProperRecCount.TabIndex = 11
		Me.lblNotProperRecCount.Text = "Label2"
		'
		'cmdExcelReport
		'
		Me.cmdExcelReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdExcelReport.Enabled = False
		Me.cmdExcelReport.Image = CType(resources.GetObject("cmdExcelReport.Image"), System.Drawing.Image)
		Me.cmdExcelReport.Location = New System.Drawing.Point(724, 3)
		Me.cmdExcelReport.Name = "cmdExcelReport"
		Me.cmdExcelReport.Size = New System.Drawing.Size(28, 28)
		Me.cmdExcelReport.TabIndex = 28
		Me.cmdExcelReport.UseVisualStyleBackColor = True
		'
		'cmdZoom
		'
		Me.cmdZoom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdZoom.Image = CType(resources.GetObject("cmdZoom.Image"), System.Drawing.Image)
		Me.cmdZoom.Location = New System.Drawing.Point(688, 3)
		Me.cmdZoom.Name = "cmdZoom"
		Me.cmdZoom.Size = New System.Drawing.Size(28, 28)
		Me.cmdZoom.TabIndex = 29
		Me.cmdZoom.UseVisualStyleBackColor = True
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
		Me.cmdExit.Location = New System.Drawing.Point(760, 3)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(28, 27)
		Me.cmdExit.TabIndex = 30
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(655, 3)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(27, 23)
		Me.Button1.TabIndex = 31
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		Me.Button1.Visible = False
		'
		'ctxBlockFull
		'
		Me.ctxBlockFull.DataPropertyName = "BlockFull"
		Me.ctxBlockFull.HeaderText = "גוש"
		Me.ctxBlockFull.Name = "ctxBlockFull"
		Me.ctxBlockFull.ReadOnly = True
		Me.ctxBlockFull.Width = 72
		'
		'ctxParcelNo
		'
		Me.ctxParcelNo.DataPropertyName = "ParcelNo"
		Me.ctxParcelNo.HeaderText = "חלקה"
		Me.ctxParcelNo.Name = "ctxParcelNo"
		Me.ctxParcelNo.ReadOnly = True
		Me.ctxParcelNo.Width = 72
		'
		'ctxRegionName
		'
		Me.ctxRegionName.DataPropertyName = "RegionName"
		Me.ctxRegionName.HeaderText = "מתחם"
		Me.ctxRegionName.Name = "ctxRegionName"
		Me.ctxRegionName.ReadOnly = True
		'
		'ctxLot
		'
		Me.ctxLot.DataPropertyName = "LotName"
		Me.ctxLot.HeaderText = "מגרש"
		Me.ctxLot.Name = "ctxLot"
		Me.ctxLot.ReadOnly = True
		Me.ctxLot.Width = 72
		'
		'ctxLegalArea
		'
		Me.ctxLegalArea.DataPropertyName = "LegalArea"
		DataGridViewCellStyle2.Format = "N0"
		DataGridViewCellStyle2.NullValue = Nothing
		Me.ctxLegalArea.DefaultCellStyle = DataGridViewCellStyle2
		Me.ctxLegalArea.HeaderText = "שטח רשום"
		Me.ctxLegalArea.Name = "ctxLegalArea"
		Me.ctxLegalArea.ReadOnly = True
		Me.ctxLegalArea.Width = 76
		'
		'ctxCalcAreaFO
		'
		Me.ctxCalcAreaFO.DataPropertyName = "CalcAreaFO"
		DataGridViewCellStyle3.Format = "N0"
		Me.ctxCalcAreaFO.DefaultCellStyle = DataGridViewCellStyle3
		Me.ctxCalcAreaFO.HeaderText = "שטח רשום"
		Me.ctxCalcAreaFO.Name = "ctxCalcAreaFO"
		Me.ctxCalcAreaFO.ReadOnly = True
		Me.ctxCalcAreaFO.Width = 76
		'
		'ctxArea
		'
		Me.ctxArea.DataPropertyName = "Area"
		DataGridViewCellStyle4.Format = "N0"
		DataGridViewCellStyle4.NullValue = Nothing
		Me.ctxArea.DefaultCellStyle = DataGridViewCellStyle4
		Me.ctxArea.HeaderText = "שטח גרפי"
		Me.ctxArea.Name = "ctxArea"
		Me.ctxArea.ReadOnly = True
		Me.ctxArea.Width = 72
		'
		'ctxDeltaArea
		'
		Me.ctxDeltaArea.DataPropertyName = "DeltaArea"
		DataGridViewCellStyle5.Format = "N0"
		DataGridViewCellStyle5.NullValue = Nothing
		Me.ctxDeltaArea.DefaultCellStyle = DataGridViewCellStyle5
		Me.ctxDeltaArea.HeaderText = "רשום - מחושב"
		Me.ctxDeltaArea.Name = "ctxDeltaArea"
		Me.ctxDeltaArea.ReadOnly = True
		Me.ctxDeltaArea.Width = 72
		'
		'ctxTolerance
		'
		Me.ctxTolerance.DataPropertyName = "Tolerance"
		DataGridViewCellStyle6.Format = "N0"
		DataGridViewCellStyle6.NullValue = Nothing
		Me.ctxTolerance.DefaultCellStyle = DataGridViewCellStyle6
		Me.ctxTolerance.HeaderText = "טולרנס"
		Me.ctxTolerance.Name = "ctxTolerance"
		Me.ctxTolerance.ReadOnly = True
		Me.ctxTolerance.Width = 72
		'
		'ctxDeviation
		'
		Me.ctxDeviation.DataPropertyName = "Deviation"
		DataGridViewCellStyle7.Format = "N1"
		DataGridViewCellStyle7.NullValue = Nothing
		Me.ctxDeviation.DefaultCellStyle = DataGridViewCellStyle7
		Me.ctxDeviation.HeaderText = "סטיה מטולרנס"
		Me.ctxDeviation.Name = "ctxDeviation"
		Me.ctxDeviation.ReadOnly = True
		Me.ctxDeviation.Width = 72
		'
		'cchProper
		'
		Me.cchProper.FalseValue = ""
		Me.cchProper.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cchProper.HeaderText = "תקין?"
		Me.cchProper.Name = "cchProper"
		Me.cchProper.ReadOnly = True
		Me.cchProper.TrueValue = ""
		Me.cchProper.Width = 42
		'
		'ctxTopoID
		'
		Me.ctxTopoID.DataPropertyName = "TopoID"
		Me.ctxTopoID.HeaderText = "TopoID"
		Me.ctxTopoID.Name = "ctxTopoID"
		Me.ctxTopoID.ReadOnly = True
		Me.ctxTopoID.Visible = False
		'
		'frmCalcAreas
		'
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
		Me.ClientSize = New System.Drawing.Size(795, 450)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdZoom)
		Me.Controls.Add(Me.cmdExcelReport)
		Me.Controls.Add(Me.lblNotProperRecCount)
		Me.Controls.Add(Me.lblRecCount)
		Me.Controls.Add(Me.rdbRegions)
		Me.Controls.Add(Me.rdbLots)
		Me.Controls.Add(Me.rdbParcels)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmCalcAreas"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "בדיקת שטחים"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents dgvMain As DataGridView
	Private WithEvents rdbParcels As RadioButton
	Private WithEvents rdbLots As RadioButton
	Private WithEvents rdbRegions As RadioButton
	Private WithEvents lblRecCount As Label
	Private WithEvents lblNotProperRecCount As Label
	Private WithEvents cmdExcelReport As Button
	Private WithEvents cmdZoom As Button
	Private WithEvents cmdExit As Button
	Private WithEvents Button1 As Button
	Friend WithEvents ctxBlockFull As DataGridViewTextBoxColumn
	Friend WithEvents ctxParcelNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxRegionName As DataGridViewTextBoxColumn
	Friend WithEvents ctxLot As DataGridViewTextBoxColumn
	Friend WithEvents ctxLegalArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxCalcAreaFO As DataGridViewTextBoxColumn
	Friend WithEvents ctxArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxDeltaArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxTolerance As DataGridViewTextBoxColumn
	Friend WithEvents ctxDeviation As DataGridViewTextBoxColumn
	Friend WithEvents cchProper As DataGridViewCheckBoxColumn
	Friend WithEvents ctxTopoID As DataGridViewTextBoxColumn
End Class
