<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUnidiv
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
		Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUnidiv))
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.ctxStage = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxAction = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxFromParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxFromParcelTemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxToParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxToGush = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxToGushAdd = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxForcedArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxTolerance = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDiff = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPlanName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLotName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.txtLanduseID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxLanduseName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxOper = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxAcObjID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxParcelDbID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxParcelSourceDbID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxRowStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxStageNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxActionNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxActionType = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cmdInsertTable = New System.Windows.Forms.Button()
		Me.rdbUnion = New System.Windows.Forms.RadioButton()
		Me.rdbDivide = New System.Windows.Forms.RadioButton()
		Me.rdbTransfer = New System.Windows.Forms.RadioButton()
		Me.txtGushNo = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtLastParcel = New System.Windows.Forms.TextBox()
		Me.cmdCancelAction = New System.Windows.Forms.Button()
		Me.cmdAllFragments = New System.Windows.Forms.Button()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.txtLastPoint = New System.Windows.Forms.TextBox()
		Me.cmdContinue = New System.Windows.Forms.Button()
		Me.cmdInsertFLine = New System.Windows.Forms.Button()
		Me.cmdStageView = New System.Windows.Forms.Button()
		Me.chkHanitView = New System.Windows.Forms.CheckBox()
		Me.cmdSelectPgon = New System.Windows.Forms.Button()
		Me.cmdSelectLinks = New System.Windows.Forms.Button()
		Me.cmdLoadDBData = New System.Windows.Forms.Button()
		Me.cmdSaveDB = New System.Windows.Forms.Button()
		Me.chkDataBound = New System.Windows.Forms.CheckBox()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.rdbNew = New System.Windows.Forms.RadioButton()
		Me.rdbHanit = New System.Windows.Forms.RadioButton()
		Me.rdbTaba = New System.Windows.Forms.RadioButton()
		Me.chkShowHideCol = New System.Windows.Forms.CheckBox()
		Me.Panel2 = New System.Windows.Forms.Panel()
		Me.rdbInsertByPick = New System.Windows.Forms.RadioButton()
		Me.rdbInsertAuto = New System.Windows.Forms.RadioButton()
		Me.cmdEraseAllStages = New System.Windows.Forms.Button()
		Me.cmdInsertAreaTable = New System.Windows.Forms.Button()
		Me.Button3 = New System.Windows.Forms.Button()
		Me.cmdSelectRow = New System.Windows.Forms.Button()
		Me.cmdGeneral = New System.Windows.Forms.Button()
		Me.cmdRestoreCancelLink = New System.Windows.Forms.Button()
		Me.cmdSelectCentroids = New System.Windows.Forms.Button()
		Me.cmdRecalc = New System.Windows.Forms.Button()
		Me.chkActiveParcels = New System.Windows.Forms.CheckBox()
		Me.chkPointsBlocking = New System.Windows.Forms.CheckBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdBlockBorder = New System.Windows.Forms.Button()
		Me.cmdZoom = New System.Windows.Forms.Button()
		Me.chkHideCanceledRows = New System.Windows.Forms.CheckBox()
		Me.txtParcelCount = New System.Windows.Forms.TextBox()
		Me.cmdClearLayers = New System.Windows.Forms.Button()
		Me.chkAddToSelect = New System.Windows.Forms.CheckBox()
		Me.chkCalcByRange = New System.Windows.Forms.CheckBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.txtPlanID_AAA = New System.Windows.Forms.TextBox()
		Me.txtStartPoint = New System.Windows.Forms.TextBox()
		Me.chkLastPoint = New System.Windows.Forms.CheckBox()
		Me.txtGushAddNo = New System.Windows.Forms.TextBox()
		Me.Label28 = New System.Windows.Forms.Label()
		Me.cmbPlanID = New System.Windows.Forms.ComboBox()
		Me.cmdSelectionSet = New System.Windows.Forms.Button()
		Me.cmdLoadScript = New System.Windows.Forms.Button()
		Me.chkRestoreCancelLink = New System.Windows.Forms.CheckBox()
		Me.cmdLoadByStage = New System.Windows.Forms.Button()
		Me.cmdLoadByAction = New System.Windows.Forms.Button()
		Me.GroupBox2 = New System.Windows.Forms.GroupBox()
		Me.cmdClearDB = New System.Windows.Forms.Button()
		Me.chkSelectUser = New System.Windows.Forms.CheckBox()
		Me.chkExcelLog = New System.Windows.Forms.CheckBox()
		Me.cmdExcelJournal = New System.Windows.Forms.Button()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.GroupBox1.SuspendLayout()
		Me.Panel2.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxStage, Me.ctxAction, Me.ctxFromParcel, Me.ctxFromParcelTemp, Me.ctxToParcel, Me.ctxToGush, Me.ctxToGushAdd, Me.ctxLegalArea, Me.ctxArea, Me.ctxForcedArea, Me.ctxTolerance, Me.ctxDiff, Me.ctxDeviation, Me.ctxPlanName, Me.ctxLotName, Me.txtLanduseID, Me.ctxLanduseName, Me.ctxOper, Me.ctxAcObjID, Me.ctxParcelDbID, Me.ctxParcelSourceDbID, Me.ctxRowStatus, Me.ctxStageNo, Me.ctxActionNo, Me.ctxActionType})
		DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
		DataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(255, Byte), Integer))
		DataGridViewCellStyle5.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		DataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(32, Byte), Integer))
		DataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
		Me.dgvMain.DefaultCellStyle = DataGridViewCellStyle5
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 84)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 24
		Me.dgvMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.dgvMain.Size = New System.Drawing.Size(1051, 397)
		Me.dgvMain.TabIndex = 10
		'
		'ctxStage
		'
		Me.ctxStage.DataPropertyName = "StageCaption"
		Me.ctxStage.HeaderText = "שלב"
		Me.ctxStage.MinimumWidth = 2
		Me.ctxStage.Name = "ctxStage"
		Me.ctxStage.ReadOnly = True
		Me.ctxStage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxStage.Width = 38
		'
		'ctxAction
		'
		Me.ctxAction.DataPropertyName = "ActionName"
		Me.ctxAction.DividerWidth = 4
		Me.ctxAction.HeaderText = "פעולה"
		Me.ctxAction.Name = "ctxAction"
		Me.ctxAction.ReadOnly = True
		Me.ctxAction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxAction.Width = 56
		'
		'ctxFromParcel
		'
		Me.ctxFromParcel.DataPropertyName = "OriginalParcelNo"
		DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ctxFromParcel.DefaultCellStyle = DataGridViewCellStyle1
		Me.ctxFromParcel.HeaderText = "מחלקה רשומה"
		Me.ctxFromParcel.Name = "ctxFromParcel"
		Me.ctxFromParcel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxFromParcel.Width = 56
		'
		'ctxFromParcelTemp
		'
		Me.ctxFromParcelTemp.DataPropertyName = "NewParcelNo"
		Me.ctxFromParcelTemp.DividerWidth = 2
		Me.ctxFromParcelTemp.HeaderText = "מחלקה ארעית"
		Me.ctxFromParcelTemp.Name = "ctxFromParcelTemp"
		Me.ctxFromParcelTemp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxFromParcelTemp.Width = 56
		'
		'ctxToParcel
		'
		Me.ctxToParcel.DataPropertyName = "DestParcelNo"
		Me.ctxToParcel.HeaderText = "לחלקה"
		Me.ctxToParcel.Name = "ctxToParcel"
		Me.ctxToParcel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxToParcel.Width = 50
		'
		'ctxToGush
		'
		Me.ctxToGush.DataPropertyName = "DestBlockNo"
		Me.ctxToGush.HeaderText = "לגוש"
		Me.ctxToGush.Name = "ctxToGush"
		Me.ctxToGush.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxToGush.Width = 48
		'
		'ctxToGushAdd
		'
		Me.ctxToGushAdd.DataPropertyName = "DestBlockAddNo"
		Me.ctxToGushAdd.DividerWidth = 4
		Me.ctxToGushAdd.HeaderText = "#"
		Me.ctxToGushAdd.Name = "ctxToGushAdd"
		Me.ctxToGushAdd.Width = 24
		'
		'ctxLegalArea
		'
		Me.ctxLegalArea.DataPropertyName = "LegalArea"
		DataGridViewCellStyle2.Format = "N3"
		DataGridViewCellStyle2.NullValue = Nothing
		Me.ctxLegalArea.DefaultCellStyle = DataGridViewCellStyle2
		Me.ctxLegalArea.HeaderText = "שטח רשום"
		Me.ctxLegalArea.Name = "ctxLegalArea"
		Me.ctxLegalArea.ReadOnly = True
		Me.ctxLegalArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxLegalArea.Width = 66
		'
		'ctxArea
		'
		Me.ctxArea.DataPropertyName = "AcadArea"
		DataGridViewCellStyle3.Format = "N3"
		DataGridViewCellStyle3.NullValue = Nothing
		Me.ctxArea.DefaultCellStyle = DataGridViewCellStyle3
		Me.ctxArea.HeaderText = "שטח"
		Me.ctxArea.Name = "ctxArea"
		Me.ctxArea.ReadOnly = True
		Me.ctxArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxArea.Width = 66
		'
		'ctxForcedArea
		'
		Me.ctxForcedArea.DataPropertyName = "ForcedArea"
		Me.ctxForcedArea.HeaderText = "שטח כפוי"
		Me.ctxForcedArea.Name = "ctxForcedArea"
		Me.ctxForcedArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxForcedArea.Width = 66
		'
		'ctxTolerance
		'
		Me.ctxTolerance.DataPropertyName = "Tolerance"
		Me.ctxTolerance.HeaderText = "טולרנס"
		Me.ctxTolerance.Name = "ctxTolerance"
		Me.ctxTolerance.ReadOnly = True
		Me.ctxTolerance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxTolerance.Width = 56
		'
		'ctxDiff
		'
		Me.ctxDiff.DataPropertyName = "DiffArea"
		Me.ctxDiff.HeaderText = "רשום - מחושב"
		Me.ctxDiff.Name = "ctxDiff"
		Me.ctxDiff.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDiff.Width = 56
		'
		'ctxDeviation
		'
		Me.ctxDeviation.DataPropertyName = "Deviation"
		DataGridViewCellStyle4.Format = "N3"
		DataGridViewCellStyle4.NullValue = Nothing
		Me.ctxDeviation.DefaultCellStyle = DataGridViewCellStyle4
		Me.ctxDeviation.HeaderText = "סטיה מטולרנס"
		Me.ctxDeviation.Name = "ctxDeviation"
		Me.ctxDeviation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDeviation.Width = 56
		'
		'ctxPlanName
		'
		Me.ctxPlanName.DataPropertyName = "PlanName"
		Me.ctxPlanName.HeaderText = "תב""ע"
		Me.ctxPlanName.Name = "ctxPlanName"
		Me.ctxPlanName.Visible = False
		Me.ctxPlanName.Width = 60
		'
		'ctxLotName
		'
		Me.ctxLotName.DataPropertyName = "LotName"
		Me.ctxLotName.HeaderText = "מגרש"
		Me.ctxLotName.Name = "ctxLotName"
		Me.ctxLotName.Width = 60
		'
		'txtLanduseID
		'
		Me.txtLanduseID.DataPropertyName = "LanduseID"
		Me.txtLanduseID.HeaderText = "מס' יעוד"
		Me.txtLanduseID.Name = "txtLanduseID"
		Me.txtLanduseID.Visible = False
		Me.txtLanduseID.Width = 80
		'
		'ctxLanduseName
		'
		Me.ctxLanduseName.DataPropertyName = "LanduseName"
		Me.ctxLanduseName.HeaderText = "שם יעוד"
		Me.ctxLanduseName.Name = "ctxLanduseName"
		'
		'ctxOper
		'
		Me.ctxOper.DataPropertyName = "Oper"
		Me.ctxOper.HeaderText = "Oper"
		Me.ctxOper.Name = "ctxOper"
		Me.ctxOper.ReadOnly = True
		Me.ctxOper.Visible = False
		Me.ctxOper.Width = 40
		'
		'ctxAcObjID
		'
		Me.ctxAcObjID.DataPropertyName = "AcObjID"
		Me.ctxAcObjID.HeaderText = "ObjID"
		Me.ctxAcObjID.Name = "ctxAcObjID"
		Me.ctxAcObjID.ReadOnly = True
		Me.ctxAcObjID.Visible = False
		Me.ctxAcObjID.Width = 120
		'
		'ctxParcelDbID
		'
		Me.ctxParcelDbID.DataPropertyName = "ParcelDbID"
		Me.ctxParcelDbID.HeaderText = "ParcelID"
		Me.ctxParcelDbID.Name = "ctxParcelDbID"
		Me.ctxParcelDbID.ReadOnly = True
		Me.ctxParcelDbID.Visible = False
		Me.ctxParcelDbID.Width = 40
		'
		'ctxParcelSourceDbID
		'
		Me.ctxParcelSourceDbID.DataPropertyName = "ParcelSourceDbID"
		Me.ctxParcelSourceDbID.HeaderText = "ParcSrcID"
		Me.ctxParcelSourceDbID.Name = "ctxParcelSourceDbID"
		Me.ctxParcelSourceDbID.Visible = False
		Me.ctxParcelSourceDbID.Width = 40
		'
		'ctxRowStatus
		'
		Me.ctxRowStatus.DataPropertyName = "RowStatus"
		Me.ctxRowStatus.HeaderText = "Status"
		Me.ctxRowStatus.Name = "ctxRowStatus"
		Me.ctxRowStatus.Visible = False
		Me.ctxRowStatus.Width = 40
		'
		'ctxStageNo
		'
		Me.ctxStageNo.DataPropertyName = "Stage"
		Me.ctxStageNo.HeaderText = "Stage"
		Me.ctxStageNo.Name = "ctxStageNo"
		Me.ctxStageNo.Visible = False
		Me.ctxStageNo.Width = 40
		'
		'ctxActionNo
		'
		Me.ctxActionNo.DataPropertyName = "Action"
		Me.ctxActionNo.HeaderText = "ActNo"
		Me.ctxActionNo.Name = "ctxActionNo"
		Me.ctxActionNo.Visible = False
		Me.ctxActionNo.Width = 40
		'
		'ctxActionType
		'
		Me.ctxActionType.DataPropertyName = "ActionType"
		Me.ctxActionType.HeaderText = "ActType"
		Me.ctxActionType.Name = "ctxActionType"
		Me.ctxActionType.Visible = False
		Me.ctxActionType.Width = 40
		'
		'cmdInsertTable
		'
		Me.cmdInsertTable.Location = New System.Drawing.Point(439, 50)
		Me.cmdInsertTable.Name = "cmdInsertTable"
		Me.cmdInsertTable.Size = New System.Drawing.Size(52, 21)
		Me.cmdInsertTable.TabIndex = 12
		Me.cmdInsertTable.Text = "טבלה"
		Me.cmdInsertTable.UseVisualStyleBackColor = True
		'
		'rdbUnion
		'
		Me.rdbUnion.AutoSize = True
		Me.rdbUnion.Location = New System.Drawing.Point(10, 4)
		Me.rdbUnion.Name = "rdbUnion"
		Me.rdbUnion.Size = New System.Drawing.Size(55, 17)
		Me.rdbUnion.TabIndex = 14
		Me.rdbUnion.Text = "איחוד"
		Me.rdbUnion.UseVisualStyleBackColor = True
		'
		'rdbDivide
		'
		Me.rdbDivide.AutoSize = True
		Me.rdbDivide.Location = New System.Drawing.Point(75, 4)
		Me.rdbDivide.Name = "rdbDivide"
		Me.rdbDivide.Size = New System.Drawing.Size(58, 17)
		Me.rdbDivide.TabIndex = 15
		Me.rdbDivide.Text = "חלוקה"
		Me.rdbDivide.UseVisualStyleBackColor = True
		'
		'rdbTransfer
		'
		Me.rdbTransfer.AutoSize = True
		Me.rdbTransfer.Location = New System.Drawing.Point(138, 4)
		Me.rdbTransfer.Name = "rdbTransfer"
		Me.rdbTransfer.Size = New System.Drawing.Size(63, 17)
		Me.rdbTransfer.TabIndex = 16
		Me.rdbTransfer.Text = "העברה"
		Me.rdbTransfer.UseVisualStyleBackColor = True
		'
		'txtGushNo
		'
		Me.txtGushNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtGushNo.Location = New System.Drawing.Point(706, 21)
		Me.txtGushNo.Name = "txtGushNo"
		Me.txtGushNo.ReadOnly = True
		Me.txtGushNo.Size = New System.Drawing.Size(42, 21)
		Me.txtGushNo.TabIndex = 18
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New System.Drawing.Point(658, 22)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(26, 14)
		Me.Label1.TabIndex = 19
		Me.Label1.Text = "גוש"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label2.Location = New System.Drawing.Point(614, 43)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(78, 13)
		Me.Label2.TabIndex = 21
		Me.Label2.Text = "חלקה אחרונה"
		'
		'txtLastParcel
		'
		Me.txtLastParcel.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtLastParcel.Location = New System.Drawing.Point(693, 42)
		Me.txtLastParcel.Name = "txtLastParcel"
		Me.txtLastParcel.Size = New System.Drawing.Size(55, 21)
		Me.txtLastParcel.TabIndex = 20
		'
		'cmdCancelAction
		'
		Me.cmdCancelAction.Image = Global.TopoUI.My.Resources.Resources.Undo
		Me.cmdCancelAction.Location = New System.Drawing.Point(279, 1)
		Me.cmdCancelAction.Name = "cmdCancelAction"
		Me.cmdCancelAction.Size = New System.Drawing.Size(22, 23)
		Me.cmdCancelAction.TabIndex = 22
		Me.cmdCancelAction.UseVisualStyleBackColor = True
		'
		'cmdAllFragments
		'
		Me.cmdAllFragments.Location = New System.Drawing.Point(916, 8)
		Me.cmdAllFragments.Name = "cmdAllFragments"
		Me.cmdAllFragments.Size = New System.Drawing.Size(39, 21)
		Me.cmdAllFragments.TabIndex = 23
		Me.cmdAllFragments.Text = "בצע"
		Me.cmdAllFragments.UseVisualStyleBackColor = True
		Me.cmdAllFragments.Visible = False
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(772, 2)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(30, 21)
		Me.Button2.TabIndex = 25
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		Me.Button2.Visible = False
		'
		'txtLastPoint
		'
		Me.txtLastPoint.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtLastPoint.Location = New System.Drawing.Point(676, 63)
		Me.txtLastPoint.Name = "txtLastPoint"
		Me.txtLastPoint.ReadOnly = True
		Me.txtLastPoint.Size = New System.Drawing.Size(36, 21)
		Me.txtLastPoint.TabIndex = 26
		'
		'cmdContinue
		'
		Me.cmdContinue.Image = CType(resources.GetObject("cmdContinue.Image"), System.Drawing.Image)
		Me.cmdContinue.Location = New System.Drawing.Point(207, 1)
		Me.cmdContinue.Name = "cmdContinue"
		Me.cmdContinue.Size = New System.Drawing.Size(22, 23)
		Me.cmdContinue.TabIndex = 28
		Me.cmdContinue.UseVisualStyleBackColor = True
		'
		'cmdInsertFLine
		'
		Me.cmdInsertFLine.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdInsertFLine.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdInsertFLine.Location = New System.Drawing.Point(439, 2)
		Me.cmdInsertFLine.Name = "cmdInsertFLine"
		Me.cmdInsertFLine.Size = New System.Drawing.Size(52, 21)
		Me.cmdInsertFLine.TabIndex = 29
		Me.cmdInsertFLine.Text = "חזיתות"
		Me.cmdInsertFLine.UseVisualStyleBackColor = True
		'
		'cmdStageView
		'
		Me.cmdStageView.Location = New System.Drawing.Point(772, 22)
		Me.cmdStageView.Name = "cmdStageView"
		Me.cmdStageView.Size = New System.Drawing.Size(30, 21)
		Me.cmdStageView.TabIndex = 30
		Me.cmdStageView.Text = "Button4"
		Me.cmdStageView.UseVisualStyleBackColor = True
		Me.cmdStageView.Visible = False
		'
		'chkHanitView
		'
		Me.chkHanitView.AutoSize = True
		Me.chkHanitView.Checked = True
		Me.chkHanitView.CheckState = System.Windows.Forms.CheckState.Checked
		Me.chkHanitView.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkHanitView.Location = New System.Drawing.Point(153, 44)
		Me.chkHanitView.Name = "chkHanitView"
		Me.chkHanitView.Size = New System.Drawing.Size(54, 17)
		Me.chkHanitView.TabIndex = 31
		Me.chkHanitView.Text = "חני""ת"
		Me.chkHanitView.UseVisualStyleBackColor = True
		'
		'cmdSelectPgon
		'
		Me.cmdSelectPgon.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSelectPgon.Location = New System.Drawing.Point(0, 26)
		Me.cmdSelectPgon.Name = "cmdSelectPgon"
		Me.cmdSelectPgon.Size = New System.Drawing.Size(54, 21)
		Me.cmdSelectPgon.TabIndex = 32
		Me.cmdSelectPgon.Text = "Polygon"
		Me.cmdSelectPgon.UseVisualStyleBackColor = True
		'
		'cmdSelectLinks
		'
		Me.cmdSelectLinks.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSelectLinks.Location = New System.Drawing.Point(51, 26)
		Me.cmdSelectLinks.Name = "cmdSelectLinks"
		Me.cmdSelectLinks.Size = New System.Drawing.Size(33, 21)
		Me.cmdSelectLinks.TabIndex = 33
		Me.cmdSelectLinks.Text = "Links"
		Me.cmdSelectLinks.UseVisualStyleBackColor = True
		'
		'cmdLoadDBData
		'
		Me.cmdLoadDBData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdLoadDBData.Enabled = False
		Me.cmdLoadDBData.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdLoadDBData.Location = New System.Drawing.Point(51, 12)
		Me.cmdLoadDBData.Name = "cmdLoadDBData"
		Me.cmdLoadDBData.Size = New System.Drawing.Size(23, 20)
		Me.cmdLoadDBData.TabIndex = 34
		Me.cmdLoadDBData.Text = "ה'"
		Me.cmdLoadDBData.UseVisualStyleBackColor = True
		'
		'cmdSaveDB
		'
		Me.cmdSaveDB.AutoSize = True
		Me.cmdSaveDB.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSaveDB.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSaveDB.Image = Global.TopoUI.My.Resources.Resources.Save18Tr
		Me.cmdSaveDB.Location = New System.Drawing.Point(502, 37)
		Me.cmdSaveDB.Name = "cmdSaveDB"
		Me.cmdSaveDB.Size = New System.Drawing.Size(24, 24)
		Me.cmdSaveDB.TabIndex = 35
		Me.cmdSaveDB.UseVisualStyleBackColor = True
		'
		'chkDataBound
		'
		Me.chkDataBound.AutoSize = True
		Me.chkDataBound.Location = New System.Drawing.Point(833, 31)
		Me.chkDataBound.Name = "chkDataBound"
		Me.chkDataBound.Size = New System.Drawing.Size(39, 17)
		Me.chkDataBound.TabIndex = 37
		Me.chkDataBound.Text = "DB"
		Me.chkDataBound.UseVisualStyleBackColor = True
		Me.chkDataBound.Visible = False
		'
		'Panel1
		'
		Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.Panel1.Location = New System.Drawing.Point(1, 22)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(195, 2)
		Me.Panel1.TabIndex = 40
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.rdbNew)
		Me.GroupBox1.Controls.Add(Me.rdbHanit)
		Me.GroupBox1.Controls.Add(Me.rdbTaba)
		Me.GroupBox1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.GroupBox1.Location = New System.Drawing.Point(353, 0)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(81, 78)
		Me.GroupBox1.TabIndex = 41
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "מס' חלקה"
		'
		'rdbNew
		'
		Me.rdbNew.Checked = True
		Me.rdbNew.Location = New System.Drawing.Point(2, 56)
		Me.rdbNew.Name = "rdbNew"
		Me.rdbNew.Size = New System.Drawing.Size(75, 17)
		Me.rdbNew.TabIndex = 2
		Me.rdbNew.TabStop = True
		Me.rdbNew.Text = "New"
		Me.rdbNew.UseVisualStyleBackColor = True
		'
		'rdbHanit
		'
		Me.rdbHanit.Location = New System.Drawing.Point(2, 19)
		Me.rdbHanit.Name = "rdbHanit"
		Me.rdbHanit.Size = New System.Drawing.Size(75, 17)
		Me.rdbHanit.TabIndex = 1
		Me.rdbHanit.Text = "C1603"
		Me.rdbHanit.UseVisualStyleBackColor = True
		'
		'rdbTaba
		'
		Me.rdbTaba.Location = New System.Drawing.Point(2, 37)
		Me.rdbTaba.Name = "rdbTaba"
		Me.rdbTaba.Size = New System.Drawing.Size(75, 17)
		Me.rdbTaba.TabIndex = 0
		Me.rdbTaba.Text = "CellNoDM"
		Me.rdbTaba.UseVisualStyleBackColor = True
		'
		'chkShowHideCol
		'
		Me.chkShowHideCol.AutoSize = True
		Me.chkShowHideCol.Location = New System.Drawing.Point(807, 0)
		Me.chkShowHideCol.Name = "chkShowHideCol"
		Me.chkShowHideCol.Size = New System.Drawing.Size(32, 17)
		Me.chkShowHideCol.TabIndex = 42
		Me.chkShowHideCol.Text = "*"
		Me.chkShowHideCol.UseVisualStyleBackColor = True
		Me.chkShowHideCol.Visible = False
		'
		'Panel2
		'
		Me.Panel2.Controls.Add(Me.rdbInsertByPick)
		Me.Panel2.Controls.Add(Me.rdbInsertAuto)
		Me.Panel2.Location = New System.Drawing.Point(207, 26)
		Me.Panel2.Name = "Panel2"
		Me.Panel2.Size = New System.Drawing.Size(96, 37)
		Me.Panel2.TabIndex = 43
		'
		'rdbInsertByPick
		'
		Me.rdbInsertByPick.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.rdbInsertByPick.Location = New System.Drawing.Point(3, 20)
		Me.rdbInsertByPick.Name = "rdbInsertByPick"
		Me.rdbInsertByPick.Size = New System.Drawing.Size(86, 15)
		Me.rdbInsertByPick.TabIndex = 41
		Me.rdbInsertByPick.Text = "Insert By Pick"
		Me.rdbInsertByPick.UseVisualStyleBackColor = True
		'
		'rdbInsertAuto
		'
		Me.rdbInsertAuto.Checked = True
		Me.rdbInsertAuto.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.rdbInsertAuto.Location = New System.Drawing.Point(3, 3)
		Me.rdbInsertAuto.Name = "rdbInsertAuto"
		Me.rdbInsertAuto.Size = New System.Drawing.Size(86, 15)
		Me.rdbInsertAuto.TabIndex = 40
		Me.rdbInsertAuto.TabStop = True
		Me.rdbInsertAuto.Text = "Insert Auto"
		Me.rdbInsertAuto.UseVisualStyleBackColor = True
		'
		'cmdEraseAllStages
		'
		Me.cmdEraseAllStages.Image = CType(resources.GetObject("cmdEraseAllStages.Image"), System.Drawing.Image)
		Me.cmdEraseAllStages.Location = New System.Drawing.Point(303, 1)
		Me.cmdEraseAllStages.Name = "cmdEraseAllStages"
		Me.cmdEraseAllStages.Size = New System.Drawing.Size(22, 23)
		Me.cmdEraseAllStages.TabIndex = 44
		Me.cmdEraseAllStages.UseVisualStyleBackColor = True
		'
		'cmdInsertAreaTable
		'
		Me.cmdInsertAreaTable.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdInsertAreaTable.Location = New System.Drawing.Point(830, 4)
		Me.cmdInsertAreaTable.Name = "cmdInsertAreaTable"
		Me.cmdInsertAreaTable.Size = New System.Drawing.Size(75, 23)
		Me.cmdInsertAreaTable.TabIndex = 45
		Me.cmdInsertAreaTable.Text = "טבלת שטחים"
		Me.cmdInsertAreaTable.UseVisualStyleBackColor = True
		Me.cmdInsertAreaTable.Visible = False
		'
		'Button3
		'
		Me.Button3.Location = New System.Drawing.Point(772, 44)
		Me.Button3.Name = "Button3"
		Me.Button3.Size = New System.Drawing.Size(30, 21)
		Me.Button3.TabIndex = 46
		Me.Button3.Text = "B3"
		Me.Button3.UseVisualStyleBackColor = True
		Me.Button3.Visible = False
		'
		'cmdSelectRow
		'
		Me.cmdSelectRow.Font = New System.Drawing.Font("Miriam", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSelectRow.Location = New System.Drawing.Point(120, 26)
		Me.cmdSelectRow.Margin = New System.Windows.Forms.Padding(0)
		Me.cmdSelectRow.Name = "cmdSelectRow"
		Me.cmdSelectRow.Size = New System.Drawing.Size(35, 21)
		Me.cmdSelectRow.TabIndex = 47
		Me.cmdSelectRow.Text = "Row"
		Me.cmdSelectRow.UseVisualStyleBackColor = True
		'
		'cmdGeneral
		'
		Me.cmdGeneral.AutoEllipsis = True
		Me.cmdGeneral.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdGeneral.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdGeneral.Location = New System.Drawing.Point(527, 60)
		Me.cmdGeneral.Name = "cmdGeneral"
		Me.cmdGeneral.Size = New System.Drawing.Size(52, 22)
		Me.cmdGeneral.TabIndex = 48
		Me.cmdGeneral.Text = "General"
		Me.cmdGeneral.UseVisualStyleBackColor = True
		'
		'cmdRestoreCancelLink
		'
		Me.cmdRestoreCancelLink.AutoSize = True
		Me.cmdRestoreCancelLink.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
		Me.cmdRestoreCancelLink.Location = New System.Drawing.Point(317, 56)
		Me.cmdRestoreCancelLink.Name = "cmdRestoreCancelLink"
		Me.cmdRestoreCancelLink.Size = New System.Drawing.Size(23, 21)
		Me.cmdRestoreCancelLink.TabIndex = 49
		Me.cmdRestoreCancelLink.UseVisualStyleBackColor = True
		Me.cmdRestoreCancelLink.Visible = False
		'
		'cmdSelectCentroids
		'
		Me.cmdSelectCentroids.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSelectCentroids.Location = New System.Drawing.Point(0, 50)
		Me.cmdSelectCentroids.Name = "cmdSelectCentroids"
		Me.cmdSelectCentroids.Size = New System.Drawing.Size(54, 21)
		Me.cmdSelectCentroids.TabIndex = 50
		Me.cmdSelectCentroids.Text = "Centers"
		Me.cmdSelectCentroids.UseVisualStyleBackColor = True
		'
		'cmdRecalc
		'
		Me.cmdRecalc.Image = Global.TopoUI.My.Resources.Resources.Invert12
		Me.cmdRecalc.Location = New System.Drawing.Point(231, 1)
		Me.cmdRecalc.Name = "cmdRecalc"
		Me.cmdRecalc.Size = New System.Drawing.Size(22, 23)
		Me.cmdRecalc.TabIndex = 51
		Me.cmdRecalc.UseVisualStyleBackColor = True
		'
		'chkActiveParcels
		'
		Me.chkActiveParcels.AutoSize = True
		Me.chkActiveParcels.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkActiveParcels.Location = New System.Drawing.Point(153, 27)
		Me.chkActiveParcels.Name = "chkActiveParcels"
		Me.chkActiveParcels.Size = New System.Drawing.Size(60, 17)
		Me.chkActiveParcels.TabIndex = 52
		Me.chkActiveParcels.Text = "חלקות"
		Me.chkActiveParcels.UseVisualStyleBackColor = True
		'
		'chkPointsBlocking
		'
		Me.chkPointsBlocking.AutoSize = True
		Me.chkPointsBlocking.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.chkPointsBlocking.Location = New System.Drawing.Point(830, 57)
		Me.chkPointsBlocking.Name = "chkPointsBlocking"
		Me.chkPointsBlocking.Size = New System.Drawing.Size(66, 17)
		Me.chkPointsBlocking.TabIndex = 53
		Me.chkPointsBlocking.Text = "All Points"
		Me.chkPointsBlocking.UseVisualStyleBackColor = True
		Me.chkPointsBlocking.Visible = False
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(921, 48)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(38, 13)
		Me.Label4.TabIndex = 54
		Me.Label4.Text = "Label4"
		Me.Label4.Visible = False
		'
		'cmdExit
		'
		Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
		Me.cmdExit.Location = New System.Drawing.Point(327, 1)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(22, 23)
		Me.cmdExit.TabIndex = 55
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdBlockBorder
		'
		Me.cmdBlockBorder.Location = New System.Drawing.Point(439, 26)
		Me.cmdBlockBorder.Name = "cmdBlockBorder"
		Me.cmdBlockBorder.Size = New System.Drawing.Size(52, 21)
		Me.cmdBlockBorder.TabIndex = 56
		Me.cmdBlockBorder.Text = "גושים"
		Me.cmdBlockBorder.UseVisualStyleBackColor = True
		'
		'cmdZoom
		'
		Me.cmdZoom.Image = Global.TopoUI.My.Resources.Resources._65
		Me.cmdZoom.Location = New System.Drawing.Point(255, 1)
		Me.cmdZoom.Name = "cmdZoom"
		Me.cmdZoom.Size = New System.Drawing.Size(22, 23)
		Me.cmdZoom.TabIndex = 58
		Me.cmdZoom.UseVisualStyleBackColor = True
		'
		'chkHideCanceledRows
		'
		Me.chkHideCanceledRows.AutoSize = True
		Me.chkHideCanceledRows.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkHideCanceledRows.Location = New System.Drawing.Point(153, 60)
		Me.chkHideCanceledRows.Name = "chkHideCanceledRows"
		Me.chkHideCanceledRows.Size = New System.Drawing.Size(57, 17)
		Me.chkHideCanceledRows.TabIndex = 59
		Me.chkHideCanceledRows.Text = "טבלה"
		Me.chkHideCanceledRows.UseVisualStyleBackColor = True
		Me.chkHideCanceledRows.Visible = False
		'
		'txtParcelCount
		'
		Me.txtParcelCount.BackColor = System.Drawing.SystemColors.Control
		Me.txtParcelCount.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtParcelCount.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtParcelCount.Location = New System.Drawing.Point(206, 65)
		Me.txtParcelCount.Name = "txtParcelCount"
		Me.txtParcelCount.ReadOnly = True
		Me.txtParcelCount.Size = New System.Drawing.Size(79, 14)
		Me.txtParcelCount.TabIndex = 60
		Me.txtParcelCount.WordWrap = False
		'
		'cmdClearLayers
		'
		Me.cmdClearLayers.BackColor = System.Drawing.Color.Red
		Me.cmdClearLayers.FlatStyle = System.Windows.Forms.FlatStyle.Popup
		Me.cmdClearLayers.Image = Global.TopoUI.My.Resources.Resources._Erase
		Me.cmdClearLayers.Location = New System.Drawing.Point(585, 6)
		Me.cmdClearLayers.Name = "cmdClearLayers"
		Me.cmdClearLayers.Size = New System.Drawing.Size(21, 23)
		Me.cmdClearLayers.TabIndex = 61
		Me.cmdClearLayers.UseVisualStyleBackColor = False
		'
		'chkAddToSelect
		'
		Me.chkAddToSelect.Appearance = System.Windows.Forms.Appearance.Button
		Me.chkAddToSelect.AutoSize = True
		Me.chkAddToSelect.Enabled = False
		Me.chkAddToSelect.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkAddToSelect.Image = Global.TopoUI.My.Resources.Resources.Multiple_Selection20x20
		Me.chkAddToSelect.Location = New System.Drawing.Point(89, 49)
		Me.chkAddToSelect.Name = "chkAddToSelect"
		Me.chkAddToSelect.Size = New System.Drawing.Size(26, 26)
		Me.chkAddToSelect.TabIndex = 62
		Me.chkAddToSelect.Text = "1"
		Me.chkAddToSelect.TextAlign = System.Drawing.ContentAlignment.BottomRight
		Me.chkAddToSelect.UseVisualStyleBackColor = True
		Me.chkAddToSelect.Visible = False
		'
		'chkCalcByRange
		'
		Me.chkCalcByRange.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkCalcByRange.Location = New System.Drawing.Point(916, 32)
		Me.chkCalcByRange.Name = "chkCalcByRange"
		Me.chkCalcByRange.Size = New System.Drawing.Size(57, 16)
		Me.chkCalcByRange.TabIndex = 63
		Me.chkCalcByRange.Text = "חישוב"
		Me.chkCalcByRange.UseVisualStyleBackColor = True
		Me.chkCalcByRange.Visible = False
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label5.Location = New System.Drawing.Point(650, 2)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(64, 13)
		Me.Label5.TabIndex = 65
		Me.Label5.Text = "מס' תוכנית"
		'
		'txtPlanID_AAA
		'
		Me.txtPlanID_AAA.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtPlanID_AAA.Location = New System.Drawing.Point(780, 63)
		Me.txtPlanID_AAA.Name = "txtPlanID_AAA"
		Me.txtPlanID_AAA.Size = New System.Drawing.Size(25, 21)
		Me.txtPlanID_AAA.TabIndex = 64
		Me.txtPlanID_AAA.Visible = False
		'
		'txtStartPoint
		'
		Me.txtStartPoint.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtStartPoint.Location = New System.Drawing.Point(712, 63)
		Me.txtStartPoint.Name = "txtStartPoint"
		Me.txtStartPoint.Size = New System.Drawing.Size(36, 21)
		Me.txtStartPoint.TabIndex = 66
		'
		'chkLastPoint
		'
		Me.chkLastPoint.AutoSize = True
		Me.chkLastPoint.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkLastPoint.Location = New System.Drawing.Point(597, 63)
		Me.chkLastPoint.Name = "chkLastPoint"
		Me.chkLastPoint.Size = New System.Drawing.Size(77, 17)
		Me.chkLastPoint.TabIndex = 67
		Me.chkLastPoint.Text = "נקודה אח'"
		Me.chkLastPoint.UseVisualStyleBackColor = True
		'
		'txtGushAddNo
		'
		Me.txtGushAddNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.txtGushAddNo.Location = New System.Drawing.Point(683, 21)
		Me.txtGushAddNo.Name = "txtGushAddNo"
		Me.txtGushAddNo.ReadOnly = True
		Me.txtGushAddNo.Size = New System.Drawing.Size(21, 21)
		Me.txtGushAddNo.TabIndex = 68
		'
		'Label28
		'
		Me.Label28.AutoSize = True
		Me.Label28.Location = New System.Drawing.Point(639, 7)
		Me.Label28.Name = "Label28"
		Me.Label28.Size = New System.Drawing.Size(11, 13)
		Me.Label28.TabIndex = 234
		Me.Label28.Text = "/"
		Me.Label28.Visible = False
		'
		'cmbPlanID
		'
		Me.cmbPlanID.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmbPlanID.FormattingEnabled = True
		Me.cmbPlanID.Location = New System.Drawing.Point(713, 0)
		Me.cmbPlanID.Name = "cmbPlanID"
		Me.cmbPlanID.Size = New System.Drawing.Size(35, 21)
		Me.cmbPlanID.TabIndex = 235
		'
		'cmdSelectionSet
		'
		Me.cmdSelectionSet.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdSelectionSet.Location = New System.Drawing.Point(51, 50)
		Me.cmdSelectionSet.Name = "cmdSelectionSet"
		Me.cmdSelectionSet.Size = New System.Drawing.Size(37, 21)
		Me.cmdSelectionSet.TabIndex = 236
		Me.cmdSelectionSet.Text = "SSet"
		Me.cmdSelectionSet.UseVisualStyleBackColor = True
		'
		'cmdLoadScript
		'
		Me.cmdLoadScript.Enabled = False
		Me.cmdLoadScript.Font = New System.Drawing.Font("Miriam", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdLoadScript.Location = New System.Drawing.Point(82, 26)
		Me.cmdLoadScript.Name = "cmdLoadScript"
		Me.cmdLoadScript.Size = New System.Drawing.Size(40, 21)
		Me.cmdLoadScript.TabIndex = 237
		Me.cmdLoadScript.Text = "Script"
		Me.cmdLoadScript.UseVisualStyleBackColor = True
		'
		'chkRestoreCancelLink
		'
		Me.chkRestoreCancelLink.Appearance = System.Windows.Forms.Appearance.Button
		Me.chkRestoreCancelLink.AutoSize = True
		Me.chkRestoreCancelLink.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkRestoreCancelLink.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
		Me.chkRestoreCancelLink.Location = New System.Drawing.Point(129, 48)
		Me.chkRestoreCancelLink.Name = "chkRestoreCancelLink"
		Me.chkRestoreCancelLink.Size = New System.Drawing.Size(22, 21)
		Me.chkRestoreCancelLink.TabIndex = 238
		Me.chkRestoreCancelLink.TextAlign = System.Drawing.ContentAlignment.BottomRight
		Me.chkRestoreCancelLink.UseVisualStyleBackColor = True
		'
		'cmdLoadByStage
		'
		Me.cmdLoadByStage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdLoadByStage.Enabled = False
		Me.cmdLoadByStage.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdLoadByStage.Location = New System.Drawing.Point(26, 12)
		Me.cmdLoadByStage.Name = "cmdLoadByStage"
		Me.cmdLoadByStage.Size = New System.Drawing.Size(24, 20)
		Me.cmdLoadByStage.TabIndex = 239
		Me.cmdLoadByStage.Text = "ש'"
		Me.cmdLoadByStage.UseVisualStyleBackColor = True
		'
		'cmdLoadByAction
		'
		Me.cmdLoadByAction.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdLoadByAction.Enabled = False
		Me.cmdLoadByAction.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdLoadByAction.Location = New System.Drawing.Point(2, 12)
		Me.cmdLoadByAction.Name = "cmdLoadByAction"
		Me.cmdLoadByAction.Size = New System.Drawing.Size(22, 20)
		Me.cmdLoadByAction.TabIndex = 240
		Me.cmdLoadByAction.Text = "פ'"
		Me.cmdLoadByAction.UseVisualStyleBackColor = True
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.cmdLoadByAction)
		Me.GroupBox2.Controls.Add(Me.cmdLoadByStage)
		Me.GroupBox2.Controls.Add(Me.cmdLoadDBData)
		Me.GroupBox2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.GroupBox2.Location = New System.Drawing.Point(502, 0)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(77, 37)
		Me.GroupBox2.TabIndex = 241
		Me.GroupBox2.TabStop = False
		Me.GroupBox2.Text = "Load DB"
		'
		'cmdClearDB
		'
		Me.cmdClearDB.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdClearDB.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdClearDB.Image = Global.TopoUI.My.Resources.Resources.Stop_18
		Me.cmdClearDB.Location = New System.Drawing.Point(555, 37)
		Me.cmdClearDB.Name = "cmdClearDB"
		Me.cmdClearDB.Size = New System.Drawing.Size(24, 22)
		Me.cmdClearDB.TabIndex = 242
		Me.cmdClearDB.UseVisualStyleBackColor = True
		'
		'chkSelectUser
		'
		Me.chkSelectUser.Appearance = System.Windows.Forms.Appearance.Button
		Me.chkSelectUser.Font = New System.Drawing.Font("Wingdings", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
		Me.chkSelectUser.Location = New System.Drawing.Point(129, 67)
		Me.chkSelectUser.Name = "chkSelectUser"
		Me.chkSelectUser.Size = New System.Drawing.Size(19, 17)
		Me.chkSelectUser.TabIndex = 243
		Me.chkSelectUser.Text = "i"
		Me.chkSelectUser.TextAlign = System.Drawing.ContentAlignment.BottomRight
		Me.chkSelectUser.UseVisualStyleBackColor = True
		'
		'chkExcelLog
		'
		Me.chkExcelLog.AutoSize = True
		Me.chkExcelLog.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.chkExcelLog.Location = New System.Drawing.Point(613, 23)
		Me.chkExcelLog.Name = "chkExcelLog"
		Me.chkExcelLog.Size = New System.Drawing.Size(43, 17)
		Me.chkExcelLog.TabIndex = 244
		Me.chkExcelLog.Text = "Log"
		Me.chkExcelLog.UseVisualStyleBackColor = True
		Me.chkExcelLog.Visible = False
		'
		'cmdExcelJournal
		'
		Me.cmdExcelJournal.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdExcelJournal.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdExcelJournal.FlatAppearance.BorderSize = 0
		Me.cmdExcelJournal.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExcelJournal.Image = Global.TopoUI.My.Resources.Resources.Excel
		Me.cmdExcelJournal.Location = New System.Drawing.Point(585, 32)
		Me.cmdExcelJournal.Name = "cmdExcelJournal"
		Me.cmdExcelJournal.Size = New System.Drawing.Size(24, 24)
		Me.cmdExcelJournal.TabIndex = 245
		Me.cmdExcelJournal.UseVisualStyleBackColor = True
		'
		'frmUnidiv
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.AutoSize = True
		Me.ClientSize = New System.Drawing.Size(1051, 481)
		Me.Controls.Add(Me.cmdExcelJournal)
		Me.Controls.Add(Me.chkExcelLog)
		Me.Controls.Add(Me.chkSelectUser)
		Me.Controls.Add(Me.cmdClearDB)
		Me.Controls.Add(Me.GroupBox2)
		Me.Controls.Add(Me.chkRestoreCancelLink)
		Me.Controls.Add(Me.cmdLoadScript)
		Me.Controls.Add(Me.cmdSelectionSet)
		Me.Controls.Add(Me.cmbPlanID)
		Me.Controls.Add(Me.Label28)
		Me.Controls.Add(Me.txtGushAddNo)
		Me.Controls.Add(Me.chkLastPoint)
		Me.Controls.Add(Me.txtStartPoint)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.txtPlanID_AAA)
		Me.Controls.Add(Me.chkCalcByRange)
		Me.Controls.Add(Me.chkAddToSelect)
		Me.Controls.Add(Me.cmdClearLayers)
		Me.Controls.Add(Me.txtParcelCount)
		Me.Controls.Add(Me.chkHideCanceledRows)
		Me.Controls.Add(Me.cmdZoom)
		Me.Controls.Add(Me.cmdBlockBorder)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.chkPointsBlocking)
		Me.Controls.Add(Me.chkActiveParcels)
		Me.Controls.Add(Me.cmdRecalc)
		Me.Controls.Add(Me.cmdSelectCentroids)
		Me.Controls.Add(Me.cmdRestoreCancelLink)
		Me.Controls.Add(Me.cmdGeneral)
		Me.Controls.Add(Me.cmdSelectRow)
		Me.Controls.Add(Me.Button3)
		Me.Controls.Add(Me.cmdInsertAreaTable)
		Me.Controls.Add(Me.cmdEraseAllStages)
		Me.Controls.Add(Me.Panel2)
		Me.Controls.Add(Me.chkShowHideCol)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me.chkDataBound)
		Me.Controls.Add(Me.cmdSaveDB)
		Me.Controls.Add(Me.cmdSelectLinks)
		Me.Controls.Add(Me.cmdSelectPgon)
		Me.Controls.Add(Me.chkHanitView)
		Me.Controls.Add(Me.cmdStageView)
		Me.Controls.Add(Me.cmdInsertFLine)
		Me.Controls.Add(Me.cmdContinue)
		Me.Controls.Add(Me.txtLastPoint)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.cmdAllFragments)
		Me.Controls.Add(Me.cmdCancelAction)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtLastParcel)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtGushNo)
		Me.Controls.Add(Me.rdbTransfer)
		Me.Controls.Add(Me.rdbDivide)
		Me.Controls.Add(Me.rdbUnion)
		Me.Controls.Add(Me.cmdInsertTable)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.MaximizeBox = False
		Me.Name = "frmUnidiv"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupBox1.ResumeLayout(False)
		Me.Panel2.ResumeLayout(False)
		Me.GroupBox2.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents dgvMain As System.Windows.Forms.DataGridView
	Private WithEvents cmdInsertTable As System.Windows.Forms.Button
	Private WithEvents rdbUnion As System.Windows.Forms.RadioButton
	Private WithEvents rdbTransfer As System.Windows.Forms.RadioButton
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents txtLastParcel As System.Windows.Forms.TextBox
	Private WithEvents cmdCancelAction As System.Windows.Forms.Button
	Private WithEvents cmdAllFragments As System.Windows.Forms.Button
	Friend WithEvents Button2 As System.Windows.Forms.Button
	Private WithEvents txtLastPoint As System.Windows.Forms.TextBox
	Private WithEvents txtGushNo As System.Windows.Forms.TextBox
	Private WithEvents cmdContinue As System.Windows.Forms.Button
	Private WithEvents cmdInsertFLine As System.Windows.Forms.Button
	Private WithEvents cmdStageView As System.Windows.Forms.Button
	Private WithEvents chkHanitView As System.Windows.Forms.CheckBox
	Private WithEvents cmdSelectPgon As System.Windows.Forms.Button
	Private WithEvents cmdSelectLinks As System.Windows.Forms.Button
	Private WithEvents cmdLoadDBData As System.Windows.Forms.Button
	Private WithEvents cmdSaveDB As System.Windows.Forms.Button
	Private WithEvents chkDataBound As System.Windows.Forms.CheckBox
	Private WithEvents Panel1 As System.Windows.Forms.Panel
	Private WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Private WithEvents rdbNew As System.Windows.Forms.RadioButton
	Friend WithEvents chkShowHideCol As System.Windows.Forms.CheckBox
	Private WithEvents rdbHanit As System.Windows.Forms.RadioButton
	Private WithEvents rdbTaba As System.Windows.Forms.RadioButton
	Private WithEvents rdbInsertByPick As System.Windows.Forms.RadioButton
	Private WithEvents rdbInsertAuto As System.Windows.Forms.RadioButton
	Private WithEvents cmdEraseAllStages As System.Windows.Forms.Button
	Private WithEvents cmdInsertAreaTable As System.Windows.Forms.Button
	Private WithEvents Button3 As System.Windows.Forms.Button
	Private WithEvents Panel2 As System.Windows.Forms.Panel
	Private WithEvents cmdSelectRow As System.Windows.Forms.Button
	Private WithEvents rdbDivide As System.Windows.Forms.RadioButton
	Private WithEvents cmdRestoreCancelLink As System.Windows.Forms.Button
	Private WithEvents cmdSelectCentroids As System.Windows.Forms.Button
	Private WithEvents cmdRecalc As System.Windows.Forms.Button
	Private WithEvents chkActiveParcels As System.Windows.Forms.CheckBox
	Private WithEvents chkPointsBlocking As System.Windows.Forms.CheckBox
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents cmdExit As System.Windows.Forms.Button
	Private WithEvents cmdBlockBorder As System.Windows.Forms.Button
	Private WithEvents cmdZoom As System.Windows.Forms.Button
	Private WithEvents chkHideCanceledRows As System.Windows.Forms.CheckBox
	Private WithEvents txtParcelCount As System.Windows.Forms.TextBox
	Private WithEvents cmdClearLayers As System.Windows.Forms.Button
	Private WithEvents chkAddToSelect As CheckBox
	Private WithEvents chkCalcByRange As CheckBox
	Private WithEvents Label5 As Label
	Private WithEvents txtPlanID_AAA As TextBox
	Private WithEvents txtStartPoint As TextBox
	Private WithEvents chkLastPoint As CheckBox
	Private WithEvents txtGushAddNo As TextBox
	Private WithEvents Label28 As Label
	Private WithEvents cmbPlanID As ComboBox
	Private WithEvents cmdSelectionSet As Button
	Private WithEvents cmdLoadScript As Button
	Private WithEvents chkRestoreCancelLink As CheckBox
	Private WithEvents cmdLoadByStage As Button
	Private WithEvents cmdLoadByAction As Button
	Private WithEvents GroupBox2 As GroupBox
	Friend WithEvents ctxStage As DataGridViewTextBoxColumn
	Friend WithEvents ctxAction As DataGridViewTextBoxColumn
	Friend WithEvents ctxFromParcel As DataGridViewTextBoxColumn
	Friend WithEvents ctxFromParcelTemp As DataGridViewTextBoxColumn
	Friend WithEvents ctxToParcel As DataGridViewTextBoxColumn
	Friend WithEvents ctxToGush As DataGridViewTextBoxColumn
	Friend WithEvents ctxToGushAdd As DataGridViewTextBoxColumn
	Friend WithEvents ctxLegalArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxForcedArea As DataGridViewTextBoxColumn
	Friend WithEvents ctxTolerance As DataGridViewTextBoxColumn
	Friend WithEvents ctxDiff As DataGridViewTextBoxColumn
	Friend WithEvents ctxDeviation As DataGridViewTextBoxColumn
	Friend WithEvents ctxPlanName As DataGridViewTextBoxColumn
	Friend WithEvents ctxLotName As DataGridViewTextBoxColumn
	Friend WithEvents txtLanduseID As DataGridViewTextBoxColumn
	Friend WithEvents ctxLanduseName As DataGridViewTextBoxColumn
	Friend WithEvents ctxOper As DataGridViewTextBoxColumn
	Friend WithEvents ctxAcObjID As DataGridViewTextBoxColumn
	Friend WithEvents ctxParcelDbID As DataGridViewTextBoxColumn
	Friend WithEvents ctxParcelSourceDbID As DataGridViewTextBoxColumn
	Friend WithEvents ctxRowStatus As DataGridViewTextBoxColumn
	Friend WithEvents ctxStageNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxActionNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxActionType As DataGridViewTextBoxColumn
	Private WithEvents cmdClearDB As Button
	Private WithEvents chkSelectUser As CheckBox
	Private WithEvents chkExcelLog As CheckBox
	Protected Friend WithEvents cmdGeneral As Button
	Private WithEvents cmdExcelJournal As Button
End Class
