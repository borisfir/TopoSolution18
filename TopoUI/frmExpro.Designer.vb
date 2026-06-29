Namespace Expro
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class frmExpro
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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExpro))
			Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Me.dgvMain = New System.Windows.Forms.DataGridView()
			Me.txtTotalAreaIn = New System.Windows.Forms.TextBox()
			Me.Label1 = New System.Windows.Forms.Label()
			Me.Label2 = New System.Windows.Forms.Label()
			Me.TextBox1 = New System.Windows.Forms.TextBox()
			Me.chkHasDeviation = New System.Windows.Forms.CheckBox()
			Me.cmbBlockNames = New System.Windows.Forms.ComboBox()
			Me.cmbVersions = New System.Windows.Forms.ComboBox()
			Me.cmdExit = New System.Windows.Forms.Button()
			Me.cmdCancel = New System.Windows.Forms.Button()
			Me.cmdOK = New System.Windows.Forms.Button()
			Me.Label3 = New System.Windows.Forms.Label()
			Me.Label4 = New System.Windows.Forms.Label()
			Me.cmdExcelReport = New System.Windows.Forms.Button()
			Me.cmdExcelAddReport = New System.Windows.Forms.Button()
			Me.cmdRefresh = New System.Windows.Forms.Button()
			Me.chkHasOutPgonsDeviation = New System.Windows.Forms.CheckBox()
			Me.chkSetAllRowsCond = New System.Windows.Forms.CheckBox()
			Me.cmdOpenEntConnected = New System.Windows.Forms.Button()
			Me.cmdCalcEntConnected = New System.Windows.Forms.Button()
			Me.cmdOpenEditlayerList = New System.Windows.Forms.Button()
			Me.chkAcadArea = New System.Windows.Forms.CheckBox()
			Me.cmdExcelNewReport = New System.Windows.Forms.Button()
			Me.cmdSave = New System.Windows.Forms.Button()
			Me.chbEdit = New System.Windows.Forms.CheckBox()
			Me.ctxBlockNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxBlockAddNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxParcelNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxPolygonTypeName = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.cchExproCalcType = New System.Windows.Forms.DataGridViewCheckBoxColumn()
			Me.ctxLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxAcadArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxTolerance = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxDif = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxTopoID = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxPolygonTypeID = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbCalcType = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbHasDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbHasOutDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxVersion = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxExproPgonTypeID = New System.Windows.Forms.DataGridViewTextBoxColumn()
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'dgvMain
			'
			Me.dgvMain.AllowUserToAddRows = False
			Me.dgvMain.AllowUserToDeleteRows = False
			Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxBlockNo, Me.ctxBlockAddNo, Me.ctxParcelNo, Me.ctxPolygonTypeName, Me.cchExproCalcType, Me.ctxLegalArea, Me.ctxAcadArea, Me.ctxTolerance, Me.ctxDif, Me.ctxDeviation, Me.ctxTopoID, Me.ctxPolygonTypeID, Me.ccbCalcType, Me.ccbHasDeviation, Me.ccbHasOutDeviation, Me.ctxVersion, Me.ctxExproPgonTypeID})
			Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.dgvMain.Location = New System.Drawing.Point(0, 66)
			Me.dgvMain.Name = "dgvMain"
			Me.dgvMain.RowHeadersWidth = 23
			Me.dgvMain.Size = New System.Drawing.Size(832, 228)
			Me.dgvMain.TabIndex = 0
			'
			'txtTotalAreaIn
			'
			Me.txtTotalAreaIn.Location = New System.Drawing.Point(136, 3)
			Me.txtTotalAreaIn.Name = "txtTotalAreaIn"
			Me.txtTotalAreaIn.ReadOnly = True
			Me.txtTotalAreaIn.Size = New System.Drawing.Size(116, 22)
			Me.txtTotalAreaIn.TabIndex = 1
			'
			'Label1
			'
			Me.Label1.AutoSize = True
			Me.Label1.Location = New System.Drawing.Point(40, 2)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(96, 28)
			Me.Label1.TabIndex = 2
			Me.Label1.Text = "שטח הכלול " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "בחלוקה החדשה"
			'
			'Label2
			'
			Me.Label2.AutoSize = True
			Me.Label2.Location = New System.Drawing.Point(53, 37)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(42, 14)
			Me.Label2.TabIndex = 4
			Me.Label2.Text = "Label2"
			Me.Label2.Visible = False
			'
			'TextBox1
			'
			Me.TextBox1.Location = New System.Drawing.Point(12, 34)
			Me.TextBox1.Name = "TextBox1"
			Me.TextBox1.ReadOnly = True
			Me.TextBox1.Size = New System.Drawing.Size(35, 22)
			Me.TextBox1.TabIndex = 3
			Me.TextBox1.Visible = False
			'
			'chkHasDeviation
			'
			Me.chkHasDeviation.AutoSize = True
			Me.chkHasDeviation.Location = New System.Drawing.Point(436, 0)
			Me.chkHasDeviation.Name = "chkHasDeviation"
			Me.chkHasDeviation.Size = New System.Drawing.Size(140, 18)
			Me.chkHasDeviation.TabIndex = 5
			Me.chkHasDeviation.Text = "חלקות חריגות בלבד"
			Me.chkHasDeviation.UseVisualStyleBackColor = True
			'
			'cmbBlockNames
			'
			Me.cmbBlockNames.FormattingEnabled = True
			Me.cmbBlockNames.Location = New System.Drawing.Point(313, 30)
			Me.cmbBlockNames.Name = "cmbBlockNames"
			Me.cmbBlockNames.Size = New System.Drawing.Size(107, 22)
			Me.cmbBlockNames.TabIndex = 6
			'
			'cmbVersions
			'
			Me.cmbVersions.FormattingEnabled = True
			Me.cmbVersions.Location = New System.Drawing.Point(313, 3)
			Me.cmbVersions.Name = "cmbVersions"
			Me.cmbVersions.Size = New System.Drawing.Size(47, 22)
			Me.cmbVersions.TabIndex = 7
			'
			'cmdExit
			'
			Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
			Me.cmdExit.Location = New System.Drawing.Point(800, 4)
			Me.cmdExit.Name = "cmdExit"
			Me.cmdExit.Size = New System.Drawing.Size(28, 27)
			Me.cmdExit.TabIndex = 13
			Me.cmdExit.UseVisualStyleBackColor = True
			'
			'cmdCancel
			'
			Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.cmdCancel.FlatAppearance.BorderSize = 0
			Me.cmdCancel.Image = Global.TopoUI.My.Resources.Resources.Cancel16
			Me.cmdCancel.Location = New System.Drawing.Point(762, 4)
			Me.cmdCancel.Name = "cmdCancel"
			Me.cmdCancel.Size = New System.Drawing.Size(33, 30)
			Me.cmdCancel.TabIndex = 12
			Me.cmdCancel.UseVisualStyleBackColor = True
			'
			'cmdOK
			'
			Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.cmdOK.Image = Global.TopoUI.My.Resources.Resources.OK16
			Me.cmdOK.Location = New System.Drawing.Point(724, 4)
			Me.cmdOK.Name = "cmdOK"
			Me.cmdOK.Size = New System.Drawing.Size(33, 30)
			Me.cmdOK.TabIndex = 11
			Me.cmdOK.UseVisualStyleBackColor = True
			'
			'Label3
			'
			Me.Label3.AutoSize = True
			Me.Label3.Location = New System.Drawing.Point(267, 6)
			Me.Label3.Name = "Label3"
			Me.Label3.Size = New System.Drawing.Size(40, 14)
			Me.Label3.TabIndex = 14
			Me.Label3.Text = "גירסה"
			'
			'Label4
			'
			Me.Label4.AutoSize = True
			Me.Label4.Location = New System.Drawing.Point(274, 35)
			Me.Label4.Name = "Label4"
			Me.Label4.Size = New System.Drawing.Size(37, 14)
			Me.Label4.TabIndex = 15
			Me.Label4.Text = "גושים"
			'
			'cmdExcelReport
			'
			Me.cmdExcelReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdExcelReport.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.cmdExcelReport.Image = Global.TopoUI.My.Resources.Resources.Excel
			Me.cmdExcelReport.Location = New System.Drawing.Point(580, 4)
			Me.cmdExcelReport.Name = "cmdExcelReport"
			Me.cmdExcelReport.Size = New System.Drawing.Size(33, 30)
			Me.cmdExcelReport.TabIndex = 16
			Me.cmdExcelReport.UseVisualStyleBackColor = True
			'
			'cmdExcelAddReport
			'
			Me.cmdExcelAddReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdExcelAddReport.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.cmdExcelAddReport.Image = Global.TopoUI.My.Resources.Resources.ExcelB
			Me.cmdExcelAddReport.Location = New System.Drawing.Point(620, 4)
			Me.cmdExcelAddReport.Name = "cmdExcelAddReport"
			Me.cmdExcelAddReport.Size = New System.Drawing.Size(30, 30)
			Me.cmdExcelAddReport.TabIndex = 17
			Me.cmdExcelAddReport.UseVisualStyleBackColor = True
			'
			'cmdRefresh
			'
			Me.cmdRefresh.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdRefresh.DialogResult = System.Windows.Forms.DialogResult.OK
			Me.cmdRefresh.Image = Global.TopoUI.My.Resources.Resources.RefreshA
			Me.cmdRefresh.Location = New System.Drawing.Point(688, 4)
			Me.cmdRefresh.Name = "cmdRefresh"
			Me.cmdRefresh.Size = New System.Drawing.Size(33, 30)
			Me.cmdRefresh.TabIndex = 18
			Me.cmdRefresh.UseVisualStyleBackColor = True
			Me.cmdRefresh.Visible = False
			'
			'chkHasOutPgonsDeviation
			'
			Me.chkHasOutPgonsDeviation.AutoSize = True
			Me.chkHasOutPgonsDeviation.Location = New System.Drawing.Point(436, 22)
			Me.chkHasOutPgonsDeviation.Name = "chkHasOutPgonsDeviation"
			Me.chkHasOutPgonsDeviation.Size = New System.Drawing.Size(135, 18)
			Me.chkHasOutPgonsDeviation.TabIndex = 19
			Me.chkHasOutPgonsDeviation.Text = " עם פוליגונים חריגים"
			Me.chkHasOutPgonsDeviation.UseVisualStyleBackColor = True
			'
			'chkSetAllRowsCond
			'
			Me.chkSetAllRowsCond.AutoSize = True
			Me.chkSetAllRowsCond.Checked = True
			Me.chkSetAllRowsCond.CheckState = System.Windows.Forms.CheckState.Checked
			Me.chkSetAllRowsCond.Location = New System.Drawing.Point(436, 44)
			Me.chkSetAllRowsCond.Name = "chkSetAllRowsCond"
			Me.chkSetAllRowsCond.Size = New System.Drawing.Size(90, 18)
			Me.chkSetAllRowsCond.TabIndex = 20
			Me.chkSetAllRowsCond.Text = "חישוב יחסי "
			Me.chkSetAllRowsCond.UseVisualStyleBackColor = True
			'
			'cmdOpenEntConnected
			'
			Me.cmdOpenEntConnected.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.cmdOpenEntConnected.Location = New System.Drawing.Point(740, 38)
			Me.cmdOpenEntConnected.Name = "cmdOpenEntConnected"
			Me.cmdOpenEntConnected.Size = New System.Drawing.Size(60, 23)
			Me.cmdOpenEntConnected.TabIndex = 21
			Me.cmdOpenEntConnected.Text = "מחוברים"
			Me.cmdOpenEntConnected.UseVisualStyleBackColor = True
			'
			'cmdCalcEntConnected
			'
			Me.cmdCalcEntConnected.AutoSize = True
			Me.cmdCalcEntConnected.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdCalcEntConnected.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me.cmdCalcEntConnected.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.cmdCalcEntConnected.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
			Me.cmdCalcEntConnected.Location = New System.Drawing.Point(806, 38)
			Me.cmdCalcEntConnected.Name = "cmdCalcEntConnected"
			Me.cmdCalcEntConnected.Size = New System.Drawing.Size(15, 21)
			Me.cmdCalcEntConnected.TabIndex = 22
			Me.cmdCalcEntConnected.UseVisualStyleBackColor = True
			'
			'cmdOpenEditlayerList
			'
			Me.cmdOpenEditlayerList.AutoSize = True
			Me.cmdOpenEditlayerList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdOpenEditlayerList.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.cmdOpenEditlayerList.Location = New System.Drawing.Point(684, 38)
			Me.cmdOpenEditlayerList.Name = "cmdOpenEditlayerList"
			Me.cmdOpenEditlayerList.Size = New System.Drawing.Size(53, 23)
			Me.cmdOpenEditlayerList.TabIndex = 23
			Me.cmdOpenEditlayerList.Text = "שכבות"
			Me.cmdOpenEditlayerList.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
			Me.cmdOpenEditlayerList.UseVisualStyleBackColor = True
			'
			'chkAcadArea
			'
			Me.chkAcadArea.AutoSize = True
			Me.chkAcadArea.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.chkAcadArea.Location = New System.Drawing.Point(578, 38)
			Me.chkAcadArea.Name = "chkAcadArea"
			Me.chkAcadArea.Size = New System.Drawing.Size(90, 17)
			Me.chkAcadArea.TabIndex = 24
			Me.chkAcadArea.Text = "שטח מחושב"
			Me.chkAcadArea.UseVisualStyleBackColor = True
			'
			'cmdExcelNewReport
			'
			Me.cmdExcelNewReport.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdExcelNewReport.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.cmdExcelNewReport.Location = New System.Drawing.Point(652, 4)
			Me.cmdExcelNewReport.Name = "cmdExcelNewReport"
			Me.cmdExcelNewReport.Size = New System.Drawing.Size(30, 30)
			Me.cmdExcelNewReport.TabIndex = 25
			Me.cmdExcelNewReport.Text = "N"
			Me.cmdExcelNewReport.UseVisualStyleBackColor = True
			'
			'cmdSave
			'
			Me.cmdSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
			Me.cmdSave.FlatAppearance.BorderSize = 0
			Me.cmdSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdSave.Image = Global.TopoUI.My.Resources.Resources.Save18Tr
			Me.cmdSave.Location = New System.Drawing.Point(210, 33)
			Me.cmdSave.Name = "cmdSave"
			Me.cmdSave.Size = New System.Drawing.Size(33, 30)
			Me.cmdSave.TabIndex = 26
			Me.cmdSave.UseVisualStyleBackColor = True
			'
			'chbEdit
			'
			Me.chbEdit.Appearance = System.Windows.Forms.Appearance.Button
			Me.chbEdit.AutoSize = True
			Me.chbEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.chbEdit.Image = Global.TopoUI.My.Resources.Resources.EditInformationHS
			Me.chbEdit.Location = New System.Drawing.Point(182, 37)
			Me.chbEdit.Name = "chbEdit"
			Me.chbEdit.Size = New System.Drawing.Size(22, 22)
			Me.chbEdit.TabIndex = 28
			Me.chbEdit.UseVisualStyleBackColor = True
			'
			'ctxBlockNo
			'
			Me.ctxBlockNo.DataPropertyName = "BlockNameView"
			Me.ctxBlockNo.HeaderText = "גוש"
			Me.ctxBlockNo.Name = "ctxBlockNo"
			Me.ctxBlockNo.ReadOnly = True
			Me.ctxBlockNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxBlockNo.Width = 60
			'
			'ctxBlockAddNo
			'
			Me.ctxBlockAddNo.DataPropertyName = "BlockAddNo"
			Me.ctxBlockAddNo.HeaderText = "ת"
			Me.ctxBlockAddNo.Name = "ctxBlockAddNo"
			Me.ctxBlockAddNo.ReadOnly = True
			Me.ctxBlockAddNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxBlockAddNo.Visible = False
			Me.ctxBlockAddNo.Width = 5
			'
			'ctxParcelNo
			'
			Me.ctxParcelNo.DataPropertyName = "ParcelNoView"
			Me.ctxParcelNo.HeaderText = "חלקה"
			Me.ctxParcelNo.Name = "ctxParcelNo"
			Me.ctxParcelNo.ReadOnly = True
			Me.ctxParcelNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxParcelNo.Width = 60
			'
			'ctxPolygonTypeName
			'
			Me.ctxPolygonTypeName.DataPropertyName = "ExproPgonTypeName"
			Me.ctxPolygonTypeName.HeaderText = "פוליגון"
			Me.ctxPolygonTypeName.Name = "ctxPolygonTypeName"
			Me.ctxPolygonTypeName.ReadOnly = True
			Me.ctxPolygonTypeName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxPolygonTypeName.Width = 150
			'
			'cchExproCalcType
			'
			Me.cchExproCalcType.DataPropertyName = "ExproCalcType"
			Me.cchExproCalcType.FalseValue = "Unchecked"
			Me.cchExproCalcType.HeaderText = "חישוב יחסי "
			Me.cchExproCalcType.IndeterminateValue = "Indeterminate"
			Me.cchExproCalcType.Name = "cchExproCalcType"
			Me.cchExproCalcType.TrueValue = "Checked"
			Me.cchExproCalcType.Width = 48
			'
			'ctxLegalArea
			'
			Me.ctxLegalArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
			Me.ctxLegalArea.DataPropertyName = "LegalArea"
			DataGridViewCellStyle1.Format = "N0"
			DataGridViewCellStyle1.NullValue = Nothing
			Me.ctxLegalArea.DefaultCellStyle = DataGridViewCellStyle1
			Me.ctxLegalArea.HeaderText = "שטח רשום"
			Me.ctxLegalArea.Name = "ctxLegalArea"
			Me.ctxLegalArea.ReadOnly = True
			Me.ctxLegalArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxLegalArea.Width = 64
			'
			'ctxAcadArea
			'
			Me.ctxAcadArea.DataPropertyName = "AcadArea"
			DataGridViewCellStyle2.Format = "N1"
			DataGridViewCellStyle2.NullValue = Nothing
			Me.ctxAcadArea.DefaultCellStyle = DataGridViewCellStyle2
			Me.ctxAcadArea.HeaderText = "שטח מח'"
			Me.ctxAcadArea.Name = "ctxAcadArea"
			Me.ctxAcadArea.ReadOnly = True
			Me.ctxAcadArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxAcadArea.Width = 80
			'
			'ctxTolerance
			'
			Me.ctxTolerance.DataPropertyName = "Tolerance"
			Me.ctxTolerance.HeaderText = "סטיה מותרת"
			Me.ctxTolerance.Name = "ctxTolerance"
			Me.ctxTolerance.ReadOnly = True
			Me.ctxTolerance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxTolerance.Width = 64
			'
			'ctxDif
			'
			Me.ctxDif.DataPropertyName = "DeltaArea"
			DataGridViewCellStyle3.Format = "N1"
			DataGridViewCellStyle3.NullValue = "0"
			Me.ctxDif.DefaultCellStyle = DataGridViewCellStyle3
			Me.ctxDif.HeaderText = "הפרש"
			Me.ctxDif.Name = "ctxDif"
			Me.ctxDif.ReadOnly = True
			Me.ctxDif.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxDif.Width = 64
			'
			'ctxDeviation
			'
			Me.ctxDeviation.DataPropertyName = "Deviation"
			Me.ctxDeviation.HeaderText = "חריגה"
			Me.ctxDeviation.Name = "ctxDeviation"
			Me.ctxDeviation.ReadOnly = True
			Me.ctxDeviation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxDeviation.ToolTipText = "חריגה מהסטיה המותרת"
			Me.ctxDeviation.Width = 64
			'
			'ctxTopoID
			'
			Me.ctxTopoID.DataPropertyName = "TopoID"
			Me.ctxTopoID.HeaderText = "TopoID"
			Me.ctxTopoID.Name = "ctxTopoID"
			Me.ctxTopoID.ReadOnly = True
			Me.ctxTopoID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxTopoID.Visible = False
			Me.ctxTopoID.Width = 40
			'
			'ctxPolygonTypeID
			'
			Me.ctxPolygonTypeID.DataPropertyName = "ExproPgonTypeID"
			Me.ctxPolygonTypeID.HeaderText = "PgonTypeID"
			Me.ctxPolygonTypeID.Name = "ctxPolygonTypeID"
			Me.ctxPolygonTypeID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxPolygonTypeID.Visible = False
			Me.ctxPolygonTypeID.Width = 40
			'
			'ccbCalcType
			'
			Me.ccbCalcType.DataPropertyName = "ExproCalcType"
			Me.ccbCalcType.HeaderText = "סוג חישוב "
			Me.ccbCalcType.Name = "ccbCalcType"
			Me.ccbCalcType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ccbCalcType.Visible = False
			Me.ccbCalcType.Width = 40
			'
			'ccbHasDeviation
			'
			Me.ccbHasDeviation.DataPropertyName = "HasDeviation"
			Me.ccbHasDeviation.HeaderText = "Dev"
			Me.ccbHasDeviation.Name = "ccbHasDeviation"
			Me.ccbHasDeviation.ReadOnly = True
			Me.ccbHasDeviation.Visible = False
			'
			'ccbHasOutDeviation
			'
			Me.ccbHasOutDeviation.DataPropertyName = "HasOutPgonsDeviation"
			Me.ccbHasOutDeviation.HeaderText = "OutDev"
			Me.ccbHasOutDeviation.Name = "ccbHasOutDeviation"
			Me.ccbHasOutDeviation.ReadOnly = True
			Me.ccbHasOutDeviation.Visible = False
			'
			'ctxVersion
			'
			Me.ctxVersion.DataPropertyName = "Version"
			Me.ctxVersion.HeaderText = "Version"
			Me.ctxVersion.Name = "ctxVersion"
			'
			'ctxExproPgonTypeID
			'
			Me.ctxExproPgonTypeID.DataPropertyName = "ExproPgonTypeID"
			Me.ctxExproPgonTypeID.HeaderText = "TypeID"
			Me.ctxExproPgonTypeID.Name = "ctxExproPgonTypeID"
			'
			'frmExpro
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
			Me.ClientSize = New System.Drawing.Size(832, 294)
			Me.Controls.Add(Me.chbEdit)
			Me.Controls.Add(Me.cmdSave)
			Me.Controls.Add(Me.cmdExcelNewReport)
			Me.Controls.Add(Me.chkAcadArea)
			Me.Controls.Add(Me.cmdOpenEditlayerList)
			Me.Controls.Add(Me.cmdCalcEntConnected)
			Me.Controls.Add(Me.cmdOpenEntConnected)
			Me.Controls.Add(Me.chkSetAllRowsCond)
			Me.Controls.Add(Me.chkHasOutPgonsDeviation)
			Me.Controls.Add(Me.cmdRefresh)
			Me.Controls.Add(Me.cmdExcelAddReport)
			Me.Controls.Add(Me.cmdExcelReport)
			Me.Controls.Add(Me.Label4)
			Me.Controls.Add(Me.Label3)
			Me.Controls.Add(Me.cmdExit)
			Me.Controls.Add(Me.cmdCancel)
			Me.Controls.Add(Me.cmdOK)
			Me.Controls.Add(Me.cmbVersions)
			Me.Controls.Add(Me.cmbBlockNames)
			Me.Controls.Add(Me.chkHasDeviation)
			Me.Controls.Add(Me.Label2)
			Me.Controls.Add(Me.TextBox1)
			Me.Controls.Add(Me.Label1)
			Me.Controls.Add(Me.txtTotalAreaIn)
			Me.Controls.Add(Me.dgvMain)
			Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.Name = "frmExpro"
			Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.RightToLeftLayout = True
			Me.Text = "הפקעות"
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		Friend WithEvents dgvMain As DataGridView
		Friend WithEvents cmbBlockNames As ComboBox
		Private WithEvents Label2 As Label
		Private WithEvents TextBox1 As TextBox
		Friend WithEvents cmbVersions As ComboBox
		Private WithEvents cmdExit As Button
		Private WithEvents cmdCancel As Button
		Private WithEvents cmdOK As Button
		Private WithEvents Label3 As Label
		Private WithEvents txtTotalAreaIn As TextBox
		Private WithEvents Label1 As Label
		Private WithEvents Label4 As Label
		Private WithEvents cmdExcelReport As Button
		Private WithEvents cmdExcelAddReport As Button
		Private WithEvents cmdRefresh As Button
		Private WithEvents chkHasOutPgonsDeviation As CheckBox
		Private WithEvents chkSetAllRowsCond As CheckBox
		Public WithEvents cmdOpenEntConnected As Button
		Private WithEvents cmdCalcEntConnected As Button
		Public WithEvents cmdOpenEditlayerList As Button
		Private WithEvents chkAcadArea As CheckBox
		Private WithEvents chkHasDeviation As CheckBox
		Private WithEvents cmdExcelNewReport As Button
		Private WithEvents cmdSave As Button
		Private WithEvents chbEdit As CheckBox
		Friend WithEvents ctxBlockNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxBlockAddNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxParcelNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxPolygonTypeName As DataGridViewTextBoxColumn
		Friend WithEvents cchExproCalcType As DataGridViewCheckBoxColumn
		Friend WithEvents ctxLegalArea As DataGridViewTextBoxColumn
		Friend WithEvents ctxAcadArea As DataGridViewTextBoxColumn
		Friend WithEvents ctxTolerance As DataGridViewTextBoxColumn
		Friend WithEvents ctxDif As DataGridViewTextBoxColumn
		Friend WithEvents ctxDeviation As DataGridViewTextBoxColumn
		Friend WithEvents ctxTopoID As DataGridViewTextBoxColumn
		Friend WithEvents ctxPolygonTypeID As DataGridViewTextBoxColumn
		Friend WithEvents ccbCalcType As DataGridViewTextBoxColumn
		Friend WithEvents ccbHasDeviation As DataGridViewTextBoxColumn
		Friend WithEvents ccbHasOutDeviation As DataGridViewTextBoxColumn
		Friend WithEvents ctxVersion As DataGridViewTextBoxColumn
		Friend WithEvents ctxExproPgonTypeID As DataGridViewTextBoxColumn
	End Class
End Namespace
