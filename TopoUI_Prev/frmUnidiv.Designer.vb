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
      Dim DataGridViewCellStyle10 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim DataGridViewCellStyle9 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUnidiv))
      Me.dgvMain = New System.Windows.Forms.DataGridView()
      Me.ctxStage = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxAction = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxFromParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxFromParcelTemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxToParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxToGush = New System.Windows.Forms.DataGridViewTextBoxColumn()
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
      Me.cmdOpenAction = New System.Windows.Forms.Button()
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
      Me.Label3 = New System.Windows.Forms.Label()
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
      Me.nudStagesView = New System.Windows.Forms.NumericUpDown()
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.GroupBox1.SuspendLayout()
      Me.Panel2.SuspendLayout()
      CType(Me.nudStagesView, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'dgvMain
      '
      Me.dgvMain.AllowUserToAddRows = False
      Me.dgvMain.AllowUserToDeleteRows = False
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxStage, Me.ctxAction, Me.ctxFromParcel, Me.ctxFromParcelTemp, Me.ctxToParcel, Me.ctxToGush, Me.ctxLegalArea, Me.ctxArea, Me.ctxForcedArea, Me.ctxTolerance, Me.ctxDiff, Me.ctxDeviation, Me.ctxPlanName, Me.ctxLotName, Me.txtLanduseID, Me.ctxLanduseName, Me.ctxOper, Me.ctxAcObjID, Me.ctxParcelDbID})
      DataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
      DataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(255, Byte), Integer))
      DataGridViewCellStyle10.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      DataGridViewCellStyle10.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(32, Byte), Integer))
      DataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight
      DataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText
      DataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
      Me.dgvMain.DefaultCellStyle = DataGridViewCellStyle10
      Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
      Me.dgvMain.Location = New System.Drawing.Point(0, 88)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.RowHeadersWidth = 24
      Me.dgvMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
      Me.dgvMain.Size = New System.Drawing.Size(1126, 434)
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
      DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
      DataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
      Me.ctxFromParcel.DefaultCellStyle = DataGridViewCellStyle6
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
      Me.ctxToGush.DividerWidth = 4
      Me.ctxToGush.HeaderText = "לגוש"
      Me.ctxToGush.Name = "ctxToGush"
      Me.ctxToGush.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxToGush.Width = 48
      '
      'ctxLegalArea
      '
      Me.ctxLegalArea.DataPropertyName = "LegalArea"
      DataGridViewCellStyle7.Format = "N3"
      DataGridViewCellStyle7.NullValue = Nothing
      Me.ctxLegalArea.DefaultCellStyle = DataGridViewCellStyle7
      Me.ctxLegalArea.HeaderText = "שטח רשום"
      Me.ctxLegalArea.Name = "ctxLegalArea"
      Me.ctxLegalArea.ReadOnly = True
      Me.ctxLegalArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxLegalArea.Width = 56
      '
      'ctxArea
      '
      Me.ctxArea.DataPropertyName = "AcadArea"
      DataGridViewCellStyle8.Format = "N3"
      DataGridViewCellStyle8.NullValue = Nothing
      Me.ctxArea.DefaultCellStyle = DataGridViewCellStyle8
      Me.ctxArea.HeaderText = "שטח"
      Me.ctxArea.Name = "ctxArea"
      Me.ctxArea.ReadOnly = True
      Me.ctxArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxArea.Width = 56
      '
      'ctxForcedArea
      '
      Me.ctxForcedArea.DataPropertyName = "ForcedArea"
      Me.ctxForcedArea.HeaderText = "שטח כפוי"
      Me.ctxForcedArea.Name = "ctxForcedArea"
      Me.ctxForcedArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
      Me.ctxForcedArea.Width = 60
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
      Me.ctxDiff.Width = 60
      '
      'ctxDeviation
      '
      Me.ctxDeviation.DataPropertyName = "Deviation"
      DataGridViewCellStyle9.Format = "N3"
      DataGridViewCellStyle9.NullValue = Nothing
      Me.ctxDeviation.DefaultCellStyle = DataGridViewCellStyle9
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
      '
      'cmdOpenAction
      '
      Me.cmdOpenAction.Enabled = False
      Me.cmdOpenAction.Image = CType(resources.GetObject("cmdOpenAction.Image"), System.Drawing.Image)
      Me.cmdOpenAction.Location = New System.Drawing.Point(1025, 30)
      Me.cmdOpenAction.Name = "cmdOpenAction"
      Me.cmdOpenAction.Size = New System.Drawing.Size(31, 25)
      Me.cmdOpenAction.TabIndex = 11
      Me.cmdOpenAction.UseVisualStyleBackColor = True
      Me.cmdOpenAction.Visible = False
      '
      'cmdInsertTable
      '
      Me.cmdInsertTable.Location = New System.Drawing.Point(476, 27)
      Me.cmdInsertTable.Name = "cmdInsertTable"
      Me.cmdInsertTable.Size = New System.Drawing.Size(64, 23)
      Me.cmdInsertTable.TabIndex = 12
      Me.cmdInsertTable.Text = "טבלה"
      Me.cmdInsertTable.UseVisualStyleBackColor = True
      '
      'rdbUnion
      '
      Me.rdbUnion.AutoSize = True
      Me.rdbUnion.Location = New System.Drawing.Point(12, 4)
      Me.rdbUnion.Name = "rdbUnion"
      Me.rdbUnion.Size = New System.Drawing.Size(55, 18)
      Me.rdbUnion.TabIndex = 14
      Me.rdbUnion.Text = "איחוד"
      Me.rdbUnion.UseVisualStyleBackColor = True
      '
      'rdbDivide
      '
      Me.rdbDivide.AutoSize = True
      Me.rdbDivide.Location = New System.Drawing.Point(88, 4)
      Me.rdbDivide.Name = "rdbDivide"
      Me.rdbDivide.Size = New System.Drawing.Size(59, 18)
      Me.rdbDivide.TabIndex = 15
      Me.rdbDivide.Text = "חלוקה"
      Me.rdbDivide.UseVisualStyleBackColor = True
      '
      'rdbTransfer
      '
      Me.rdbTransfer.AutoSize = True
      Me.rdbTransfer.Location = New System.Drawing.Point(161, 4)
      Me.rdbTransfer.Name = "rdbTransfer"
      Me.rdbTransfer.Size = New System.Drawing.Size(65, 18)
      Me.rdbTransfer.TabIndex = 16
      Me.rdbTransfer.Text = "העברה"
      Me.rdbTransfer.UseVisualStyleBackColor = True
      '
      'txtGushNo
      '
      Me.txtGushNo.Location = New System.Drawing.Point(787, 4)
      Me.txtGushNo.Name = "txtGushNo"
      Me.txtGushNo.Size = New System.Drawing.Size(65, 22)
      Me.txtGushNo.TabIndex = 18
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(755, 9)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(26, 14)
      Me.Label1.TabIndex = 19
      Me.Label1.Text = "גוש"
      '
      'Label2
      '
      Me.Label2.AutoSize = True
      Me.Label2.Location = New System.Drawing.Point(700, 36)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(81, 14)
      Me.Label2.TabIndex = 21
      Me.Label2.Text = "חלקה אחרונה"
      '
      'txtLastParcel
      '
      Me.txtLastParcel.Location = New System.Drawing.Point(787, 32)
      Me.txtLastParcel.Name = "txtLastParcel"
      Me.txtLastParcel.Size = New System.Drawing.Size(73, 22)
      Me.txtLastParcel.TabIndex = 20
      '
      'cmdCancelAction
      '
      Me.cmdCancelAction.Enabled = False
      Me.cmdCancelAction.Image = Global.TopoUI.My.Resources.Resources.Undo
      Me.cmdCancelAction.Location = New System.Drawing.Point(264, 0)
      Me.cmdCancelAction.Name = "cmdCancelAction"
      Me.cmdCancelAction.Size = New System.Drawing.Size(30, 25)
      Me.cmdCancelAction.TabIndex = 22
      Me.cmdCancelAction.UseVisualStyleBackColor = True
      '
      'cmdAllFragments
      '
      Me.cmdAllFragments.Location = New System.Drawing.Point(1069, 9)
      Me.cmdAllFragments.Name = "cmdAllFragments"
      Me.cmdAllFragments.Size = New System.Drawing.Size(45, 23)
      Me.cmdAllFragments.TabIndex = 23
      Me.cmdAllFragments.Text = "בצע"
      Me.cmdAllFragments.UseVisualStyleBackColor = True
      Me.cmdAllFragments.Visible = False
      '
      'Button2
      '
      Me.Button2.Location = New System.Drawing.Point(901, 2)
      Me.Button2.Name = "Button2"
      Me.Button2.Size = New System.Drawing.Size(35, 23)
      Me.Button2.TabIndex = 25
      Me.Button2.Text = "Button2"
      Me.Button2.UseVisualStyleBackColor = True
      Me.Button2.Visible = False
      '
      'Label3
      '
      Me.Label3.AutoSize = True
      Me.Label3.Location = New System.Drawing.Point(700, 63)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(82, 14)
      Me.Label3.TabIndex = 27
      Me.Label3.Text = "נקודה אחרונה"
      '
      'txtLastPoint
      '
      Me.txtLastPoint.Location = New System.Drawing.Point(787, 60)
      Me.txtLastPoint.Name = "txtLastPoint"
      Me.txtLastPoint.Size = New System.Drawing.Size(73, 22)
      Me.txtLastPoint.TabIndex = 26
      Me.txtLastPoint.Text = "1000"
      '
      'cmdContinue
      '
      Me.cmdContinue.Image = CType(resources.GetObject("cmdContinue.Image"), System.Drawing.Image)
      Me.cmdContinue.Location = New System.Drawing.Point(232, 0)
      Me.cmdContinue.Name = "cmdContinue"
      Me.cmdContinue.Size = New System.Drawing.Size(30, 25)
      Me.cmdContinue.TabIndex = 28
      Me.cmdContinue.UseVisualStyleBackColor = True
      '
      'cmdInsertFLine
      '
      Me.cmdInsertFLine.Location = New System.Drawing.Point(476, 2)
      Me.cmdInsertFLine.Name = "cmdInsertFLine"
      Me.cmdInsertFLine.Size = New System.Drawing.Size(64, 23)
      Me.cmdInsertFLine.TabIndex = 29
      Me.cmdInsertFLine.Text = "חזיתות"
      Me.cmdInsertFLine.UseVisualStyleBackColor = True
      '
      'cmdStageView
      '
      Me.cmdStageView.Location = New System.Drawing.Point(901, 24)
      Me.cmdStageView.Name = "cmdStageView"
      Me.cmdStageView.Size = New System.Drawing.Size(35, 23)
      Me.cmdStageView.TabIndex = 30
      Me.cmdStageView.Text = "Button4"
      Me.cmdStageView.UseVisualStyleBackColor = True
      Me.cmdStageView.Visible = False
      '
      'chkHanitView
      '
      Me.chkHanitView.AutoSize = True
      Me.chkHanitView.Location = New System.Drawing.Point(171, 54)
      Me.chkHanitView.Name = "chkHanitView"
      Me.chkHanitView.Size = New System.Drawing.Size(57, 18)
      Me.chkHanitView.TabIndex = 31
      Me.chkHanitView.Text = "חני""ת"
      Me.chkHanitView.UseVisualStyleBackColor = True
      '
      'cmdSelectPgon
      '
      Me.cmdSelectPgon.Location = New System.Drawing.Point(8, 28)
      Me.cmdSelectPgon.Name = "cmdSelectPgon"
      Me.cmdSelectPgon.Size = New System.Drawing.Size(58, 23)
      Me.cmdSelectPgon.TabIndex = 32
      Me.cmdSelectPgon.Text = "Polygon"
      Me.cmdSelectPgon.UseVisualStyleBackColor = True
      '
      'cmdSelectLinks
      '
      Me.cmdSelectLinks.Location = New System.Drawing.Point(72, 28)
      Me.cmdSelectLinks.Name = "cmdSelectLinks"
      Me.cmdSelectLinks.Size = New System.Drawing.Size(43, 23)
      Me.cmdSelectLinks.TabIndex = 33
      Me.cmdSelectLinks.Text = "Links"
      Me.cmdSelectLinks.UseVisualStyleBackColor = True
      '
      'cmdLoadDBData
      '
      Me.cmdLoadDBData.AutoSize = True
      Me.cmdLoadDBData.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.cmdLoadDBData.Location = New System.Drawing.Point(597, 2)
      Me.cmdLoadDBData.Name = "cmdLoadDBData"
      Me.cmdLoadDBData.Size = New System.Drawing.Size(64, 23)
      Me.cmdLoadDBData.TabIndex = 34
      Me.cmdLoadDBData.Text = "LoadDB"
      Me.cmdLoadDBData.UseVisualStyleBackColor = True
      '
      'cmdSaveDB
      '
      Me.cmdSaveDB.Location = New System.Drawing.Point(597, 27)
      Me.cmdSaveDB.Name = "cmdSaveDB"
      Me.cmdSaveDB.Size = New System.Drawing.Size(64, 23)
      Me.cmdSaveDB.TabIndex = 35
      Me.cmdSaveDB.Text = "SaveDB"
      Me.cmdSaveDB.UseVisualStyleBackColor = True
      '
      'chkDataBound
      '
      Me.chkDataBound.AutoSize = True
      Me.chkDataBound.Location = New System.Drawing.Point(672, 1)
      Me.chkDataBound.Name = "chkDataBound"
      Me.chkDataBound.Size = New System.Drawing.Size(41, 18)
      Me.chkDataBound.TabIndex = 37
      Me.chkDataBound.Text = "DB"
      Me.chkDataBound.UseVisualStyleBackColor = True
      '
      'Panel1
      '
      Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.Panel1.Location = New System.Drawing.Point(1, 24)
      Me.Panel1.Name = "Panel1"
      Me.Panel1.Size = New System.Drawing.Size(227, 2)
      Me.Panel1.TabIndex = 40
      '
      'GroupBox1
      '
      Me.GroupBox1.Controls.Add(Me.rdbNew)
      Me.GroupBox1.Controls.Add(Me.rdbHanit)
      Me.GroupBox1.Controls.Add(Me.rdbTaba)
      Me.GroupBox1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.GroupBox1.Location = New System.Drawing.Point(360, 0)
      Me.GroupBox1.Name = "GroupBox1"
      Me.GroupBox1.Size = New System.Drawing.Size(94, 84)
      Me.GroupBox1.TabIndex = 41
      Me.GroupBox1.TabStop = False
      Me.GroupBox1.Text = "מס' חלקה"
      '
      'rdbNew
      '
      Me.rdbNew.Checked = True
      Me.rdbNew.Location = New System.Drawing.Point(2, 60)
      Me.rdbNew.Name = "rdbNew"
      Me.rdbNew.Size = New System.Drawing.Size(87, 18)
      Me.rdbNew.TabIndex = 2
      Me.rdbNew.TabStop = True
      Me.rdbNew.Text = "New"
      Me.rdbNew.UseVisualStyleBackColor = True
      '
      'rdbHanit
      '
      Me.rdbHanit.Location = New System.Drawing.Point(2, 20)
      Me.rdbHanit.Name = "rdbHanit"
      Me.rdbHanit.Size = New System.Drawing.Size(87, 18)
      Me.rdbHanit.TabIndex = 1
      Me.rdbHanit.Text = "C1603"
      Me.rdbHanit.UseVisualStyleBackColor = True
      '
      'rdbTaba
      '
      Me.rdbTaba.Location = New System.Drawing.Point(2, 40)
      Me.rdbTaba.Name = "rdbTaba"
      Me.rdbTaba.Size = New System.Drawing.Size(87, 18)
      Me.rdbTaba.TabIndex = 0
      Me.rdbTaba.Text = "CellNoDM"
      Me.rdbTaba.UseVisualStyleBackColor = True
      '
      'chkShowHideCol
      '
      Me.chkShowHideCol.AutoSize = True
      Me.chkShowHideCol.Location = New System.Drawing.Point(942, 0)
      Me.chkShowHideCol.Name = "chkShowHideCol"
      Me.chkShowHideCol.Size = New System.Drawing.Size(33, 18)
      Me.chkShowHideCol.TabIndex = 42
      Me.chkShowHideCol.Text = "*"
      Me.chkShowHideCol.UseVisualStyleBackColor = True
      '
      'Panel2
      '
      Me.Panel2.Controls.Add(Me.rdbInsertByPick)
      Me.Panel2.Controls.Add(Me.rdbInsertAuto)
      Me.Panel2.Location = New System.Drawing.Point(240, 31)
      Me.Panel2.Name = "Panel2"
      Me.Panel2.Size = New System.Drawing.Size(112, 52)
      Me.Panel2.TabIndex = 43
      '
      'rdbInsertByPick
      '
      Me.rdbInsertByPick.Location = New System.Drawing.Point(5, 26)
      Me.rdbInsertByPick.Name = "rdbInsertByPick"
      Me.rdbInsertByPick.Size = New System.Drawing.Size(100, 18)
      Me.rdbInsertByPick.TabIndex = 41
      Me.rdbInsertByPick.Text = "Insert By Pick"
      Me.rdbInsertByPick.UseVisualStyleBackColor = True
      '
      'rdbInsertAuto
      '
      Me.rdbInsertAuto.Checked = True
      Me.rdbInsertAuto.Location = New System.Drawing.Point(4, 5)
      Me.rdbInsertAuto.Name = "rdbInsertAuto"
      Me.rdbInsertAuto.Size = New System.Drawing.Size(100, 18)
      Me.rdbInsertAuto.TabIndex = 40
      Me.rdbInsertAuto.TabStop = True
      Me.rdbInsertAuto.Text = "Insert Auto"
      Me.rdbInsertAuto.UseVisualStyleBackColor = True
      '
      'cmdEraseAllStages
      '
      Me.cmdEraseAllStages.Image = CType(resources.GetObject("cmdEraseAllStages.Image"), System.Drawing.Image)
      Me.cmdEraseAllStages.Location = New System.Drawing.Point(296, 0)
      Me.cmdEraseAllStages.Name = "cmdEraseAllStages"
      Me.cmdEraseAllStages.Size = New System.Drawing.Size(30, 25)
      Me.cmdEraseAllStages.TabIndex = 44
      Me.cmdEraseAllStages.UseVisualStyleBackColor = True
      '
      'cmdInsertAreaTable
      '
      Me.cmdInsertAreaTable.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.cmdInsertAreaTable.Location = New System.Drawing.Point(968, 4)
      Me.cmdInsertAreaTable.Name = "cmdInsertAreaTable"
      Me.cmdInsertAreaTable.Size = New System.Drawing.Size(88, 25)
      Me.cmdInsertAreaTable.TabIndex = 45
      Me.cmdInsertAreaTable.Text = "טבלת שטחים"
      Me.cmdInsertAreaTable.UseVisualStyleBackColor = True
      Me.cmdInsertAreaTable.Visible = False
      '
      'Button3
      '
      Me.Button3.Location = New System.Drawing.Point(901, 52)
      Me.Button3.Name = "Button3"
      Me.Button3.Size = New System.Drawing.Size(35, 23)
      Me.Button3.TabIndex = 46
      Me.Button3.Text = "Button4"
      Me.Button3.UseVisualStyleBackColor = True
      '
      'cmdSelectRow
      '
      Me.cmdSelectRow.Location = New System.Drawing.Point(121, 28)
      Me.cmdSelectRow.Name = "cmdSelectRow"
      Me.cmdSelectRow.Size = New System.Drawing.Size(43, 23)
      Me.cmdSelectRow.TabIndex = 47
      Me.cmdSelectRow.Text = "Row"
      Me.cmdSelectRow.UseVisualStyleBackColor = True
      '
      'cmdGeneral
      '
      Me.cmdGeneral.Location = New System.Drawing.Point(597, 54)
      Me.cmdGeneral.Name = "cmdGeneral"
      Me.cmdGeneral.Size = New System.Drawing.Size(64, 23)
      Me.cmdGeneral.TabIndex = 48
      Me.cmdGeneral.Text = "General"
      Me.cmdGeneral.UseVisualStyleBackColor = True
      '
      'cmdRestoreCancelLink
      '
      Me.cmdRestoreCancelLink.AutoSize = True
      Me.cmdRestoreCancelLink.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
      Me.cmdRestoreCancelLink.Location = New System.Drawing.Point(71, 55)
      Me.cmdRestoreCancelLink.Name = "cmdRestoreCancelLink"
      Me.cmdRestoreCancelLink.Size = New System.Drawing.Size(27, 23)
      Me.cmdRestoreCancelLink.TabIndex = 49
      Me.cmdRestoreCancelLink.UseVisualStyleBackColor = True
      '
      'cmdSelectCentroids
      '
      Me.cmdSelectCentroids.Location = New System.Drawing.Point(8, 55)
      Me.cmdSelectCentroids.Name = "cmdSelectCentroids"
      Me.cmdSelectCentroids.Size = New System.Drawing.Size(57, 23)
      Me.cmdSelectCentroids.TabIndex = 50
      Me.cmdSelectCentroids.Text = "Centers"
      Me.cmdSelectCentroids.UseVisualStyleBackColor = True
      '
      'cmdRecalc
      '
      Me.cmdRecalc.Image = Global.TopoUI.My.Resources.Resources.Invert12
      Me.cmdRecalc.Location = New System.Drawing.Point(476, 63)
      Me.cmdRecalc.Name = "cmdRecalc"
      Me.cmdRecalc.Size = New System.Drawing.Size(22, 22)
      Me.cmdRecalc.TabIndex = 51
      Me.cmdRecalc.UseVisualStyleBackColor = True
      '
      'chkActiveParcels
      '
      Me.chkActiveParcels.AutoSize = True
      Me.chkActiveParcels.Location = New System.Drawing.Point(170, 32)
      Me.chkActiveParcels.Name = "chkActiveParcels"
      Me.chkActiveParcels.Size = New System.Drawing.Size(62, 18)
      Me.chkActiveParcels.TabIndex = 52
      Me.chkActiveParcels.Text = "חלקות"
      Me.chkActiveParcels.UseVisualStyleBackColor = True
      '
      'chkPointsBlocking
      '
      Me.chkPointsBlocking.AutoSize = True
      Me.chkPointsBlocking.FlatStyle = System.Windows.Forms.FlatStyle.Flat
      Me.chkPointsBlocking.Location = New System.Drawing.Point(968, 61)
      Me.chkPointsBlocking.Name = "chkPointsBlocking"
      Me.chkPointsBlocking.Size = New System.Drawing.Size(72, 18)
      Me.chkPointsBlocking.TabIndex = 53
      Me.chkPointsBlocking.Text = "All Points"
      Me.chkPointsBlocking.UseVisualStyleBackColor = True
      '
      'Label4
      '
      Me.Label4.AutoSize = True
      Me.Label4.Location = New System.Drawing.Point(1074, 52)
      Me.Label4.Name = "Label4"
      Me.Label4.Size = New System.Drawing.Size(42, 14)
      Me.Label4.TabIndex = 54
      Me.Label4.Text = "Label4"
      '
      'cmdExit
      '
      Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
      Me.cmdExit.Location = New System.Drawing.Point(328, 0)
      Me.cmdExit.Name = "cmdExit"
      Me.cmdExit.Size = New System.Drawing.Size(30, 25)
      Me.cmdExit.TabIndex = 55
      Me.cmdExit.UseVisualStyleBackColor = True
      '
      'nudStagesView
      '
      Me.nudStagesView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.nudStagesView.Location = New System.Drawing.Point(139, 54)
      Me.nudStagesView.Name = "nudStagesView"
      Me.nudStagesView.Size = New System.Drawing.Size(26, 22)
      Me.nudStagesView.TabIndex = 56
      Me.nudStagesView.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left
      '
      'frmUnidiv
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(1126, 522)
      Me.Controls.Add(Me.nudStagesView)
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
      Me.Controls.Add(Me.cmdLoadDBData)
      Me.Controls.Add(Me.cmdSelectLinks)
      Me.Controls.Add(Me.cmdSelectPgon)
      Me.Controls.Add(Me.chkHanitView)
      Me.Controls.Add(Me.cmdStageView)
      Me.Controls.Add(Me.cmdInsertFLine)
      Me.Controls.Add(Me.cmdContinue)
      Me.Controls.Add(Me.Label3)
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
      Me.Controls.Add(Me.cmdOpenAction)
      Me.Controls.Add(Me.dgvMain)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      Me.MaximizeBox = False
      Me.MinimizeBox = False
      Me.Name = "frmUnidiv"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.RightToLeftLayout = True
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.GroupBox1.ResumeLayout(False)
      Me.Panel2.ResumeLayout(False)
      CType(Me.nudStagesView, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub

   Private WithEvents dgvMain As System.Windows.Forms.DataGridView
   Private WithEvents cmdOpenAction As System.Windows.Forms.Button
   Private WithEvents cmdInsertTable As System.Windows.Forms.Button
   Private WithEvents rdbUnion As System.Windows.Forms.RadioButton
   Private WithEvents rdbTransfer As System.Windows.Forms.RadioButton
   Private WithEvents Label1 As System.Windows.Forms.Label
   Private WithEvents Label2 As System.Windows.Forms.Label
   Private WithEvents txtLastParcel As System.Windows.Forms.TextBox
   Private WithEvents cmdCancelAction As System.Windows.Forms.Button
   Private WithEvents cmdAllFragments As System.Windows.Forms.Button
   Friend WithEvents Button2 As System.Windows.Forms.Button
   Private WithEvents Label3 As System.Windows.Forms.Label
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
   Private WithEvents cmdGeneral As System.Windows.Forms.Button
   Private WithEvents cmdRestoreCancelLink As System.Windows.Forms.Button
   Private WithEvents cmdSelectCentroids As System.Windows.Forms.Button
   Private WithEvents cmdRecalc As System.Windows.Forms.Button
   Friend WithEvents ctxStage As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxAction As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxFromParcel As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxFromParcelTemp As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxToParcel As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxToGush As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLegalArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxForcedArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxTolerance As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxDiff As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxDeviation As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxPlanName As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLotName As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents txtLanduseID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLanduseName As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxOper As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxAcObjID As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxParcelDbID As System.Windows.Forms.DataGridViewTextBoxColumn
   Private WithEvents chkActiveParcels As System.Windows.Forms.CheckBox
   Private WithEvents chkPointsBlocking As System.Windows.Forms.CheckBox
   Friend WithEvents Label4 As System.Windows.Forms.Label
   Private WithEvents cmdExit As System.Windows.Forms.Button
   Private WithEvents nudStagesView As System.Windows.Forms.NumericUpDown
End Class
