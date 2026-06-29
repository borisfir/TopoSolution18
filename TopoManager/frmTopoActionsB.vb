Option Explicit On
Option Strict On
Public Class frmTopoActionsB

	Private Const msActionIDFldName As String = "ActionID"
	Private Const msToleranceFldName As String = "Tolerance"
	Private Const msErrorsFldName As String = "Errors"
	Private Const msPointsFldName As String = "Points"
	Private Const msCalculateSettingKey As String = "Calculate"
	Private Const msLotNameNumSettingKey As String = "LotNameNum"

	Private miActionDflt As Integer = 0
	Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmActions
	Private miToposUB As Integer = 0 ''''TopoDefs.giToposUB
	Private moaLabels(miToposUB) As LabelInd
	Private moaChecks(miToposUB) As TopoCheck
	Private moaCheckButtons(miToposUB) As CheckButton
	'''''  Private miaTopoIDs(miToposUB) As Integer
	'''''  Private moaTopoDefs(miToposUB) As TopoDef
	Private miCurrentTopoID_AAA As Integer
	Private miCurrentTopoDefID As TopoDefID

	Private moCurrentLabel As LabelInd = Nothing
	Private moLabelColor As System.Drawing.Color = Color.DimGray
	Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue
	Private moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private moSelectedReportItem As ReportItem
	Private moaErrorPoints() As TplnPointArray = Nothing
	Private miCurrentAction As Integer = -1

	Private moCurrentPoints As TplnPointArray = Nothing
	Private moCurrentView As DataView
	Private miaReports() As TPlServerDB.enResourceTheme = { _
	TPlServerDB.enResourceTheme.AcRepContent _
	, TPlServerDB.enResourceTheme.AcRepPlanParcels _
	, TPlServerDB.enResourceTheme.AcRepLotsK _
	, TPlServerDB.enResourceTheme.AcRepLotsM _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseK _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseM _
	, TPlServerDB.enResourceTheme.AcRepSumLuse _
, TPlServerDB.enResourceTheme.AcRepBlockArea _
, TPlServerDB.enResourceTheme.AcRepLotContent _
, TPlServerDB.enResourceTheme.AcRepLotContentArea _
, TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel}
	Private miaReportOption() As TPlanGraph.enDataOptions = {TPlanGraph.enDataOptions.Default _
, TPlanGraph.enDataOptions.Default _
, TPlanGraph.enDataOptions.AcadArea _
, TPlanGraph.enDataOptions.AcadArea _
, TPlanGraph.enDataOptions.AcadArea _
, TPlanGraph.enDataOptions.AcadArea _
, TPlanGraph.enDataOptions.AcadArea _
, TPlanGraph.enDataOptions.Default _
, TPlanGraph.enDataOptions.Default _
, TPlanGraph.enDataOptions.Default _
, TPlanGraph.enDataOptions.Default}

	Private moDataTable(miToposUB) As System.Data.DataTable
	Private miStepNum(miToposUB) As Integer

	Private moOleDbDataAdapter(miToposUB) As System.Data.OleDb.OleDbDataAdapter
	'new System.Data.DataTable(msTableName);
	Private WithEvents dgvMain As DataGridView

	Private WithEvents tabMain As TabControl
	Private tbpTopo As System.Windows.Forms.TabPage
	Private tbpReports As System.Windows.Forms.TabPage
	Private tbpParameters As System.Windows.Forms.TabPage

	Private WithEvents cmdFix As TabButton
	Private WithEvents cmdMark As TabButton
	Private WithEvents cmdBuild As TabButton
	Private WithEvents cmdKill As TabButton
	Private WithEvents cmdShow As TabButton

	Private WithEvents cmdEraseTopoGeometria As TabButton
	Private WithEvents cmdCalculate As TabButton
	Private WithEvents cmdInsertRep As TabButton
	Private WithEvents cmdExpExcel As TabButton


	Private WithEvents cmdRegen As TabButton
	Private WithEvents cmdClose As TabButton
	Private WithEvents cmdExit As TabButton
	Private WithEvents nudSteps As System.Windows.Forms.NumericUpDown
	Private WithEvents nudErrors As System.Windows.Forms.NumericUpDown




	Private chkIgnoreIncompleteArea As CheckBox
	Private chkHighlightSliver As CheckBox
	Private WithEvents chkTopoLayersOn As CheckBox


	Private WithEvents chkBamash As CheckBox

	'	Private WithEvents chkApproved As CheckBox
	'	Private WithEvents chkProposed As CheckBox
	'	Private WithEvents chkParcel As CheckBox
	'	Private WithEvents chkMerge As CheckBox
	'	Private WithEvents chkUnion As CheckBox

	Private WithEvents chkLotNameNum As CheckBox

	Private txtTolerance As TextBox
	Private lblTolerance As Label
	Private lblErrors As Label
	Private lblBlockFormat As Label
	Private lblTopoFormat As Label
	Private lblLanduseFormat As Label
	Private lblCoordinateFormat As Label
	Private lblAreaFormat As Label




	Private WithEvents lstReports As ListBox
	Private grbCalcOption As System.Windows.Forms.GroupBox
	Private WithEvents rdbCalc As System.Windows.Forms.RadioButton
	Private WithEvents rdbAcad As System.Windows.Forms.RadioButton
	Private grbFormats As System.Windows.Forms.GroupBox
	Private grbNumberFormat As System.Windows.Forms.GroupBox


	Private WithEvents cmbBlockFormat As System.Windows.Forms.ComboBox
	Private WithEvents cmbTopoFormat As System.Windows.Forms.ComboBox
	Private WithEvents cmbLanduseFormat As System.Windows.Forms.ComboBox
	Private WithEvents cmbCoordFormat As System.Windows.Forms.ComboBox
	Private WithEvents cmbAreaFormat As System.Windows.Forms.ComboBox


	Private mbCodeExecuting As Boolean = False
	Public Event Calculate()
	Public Event FormatChanged()

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()
		' Add any initialization after the InitializeComponent() call.
		TopoDefs.InitTopoMaster()
		zzMyInitializeComponent()
		Me.DialogResult = System.Windows.Forms.DialogResult.No
	End Sub

	Private Sub lblTopo_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)

		Try
			Dim oLabelInd As LabelInd
			oLabelInd = DirectCast(oSender, LabelInd)
			If moCurrentLabel IsNot Nothing AndAlso moCurrentLabel IsNot oLabelInd Then
				moCurrentLabel.Font = moLabelFont
				moCurrentLabel.ForeColor = moLabelColor
			End If
			moCurrentLabel = oLabelInd
			oLabelInd.Font = moLabelSelectFont
			oLabelInd.ForeColor = moLabelSelectColor
			miCurrentTopoDefID = TopoDefs.miaTopoIDs(oLabelInd.Index)
			Me.dgvMain.Columns.Clear()
			Dim oTopoDef As TopoDef = zzLoadTopoDef()
			Me.cmdFix.Enabled = oTopoDef.CleanupEnabled
			Me.cmdMark.Enabled = oTopoDef.CleanupEnabled

			If miCurrentTopoDefID.TopoIsUnion Then
				Me.nudSteps.Enabled = False
			Else
				zzLoadData(oLabelInd.Index)
				With Me.nudSteps
					.Value = Decimal.One
					.Maximum = miStepNum(oLabelInd.Index)
					.Enabled = (.Maximum > 1)
				End With
				zzSetGridColumns()


				If oTopoDef IsNot Nothing Then
					Me.cmdEraseTopoGeometria.Enabled = oTopoDef.LinkLayersExists
				End If
			End If

			'   zzTopoLayersIsOn()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - lblTopo_Click")
		End Try

	End Sub

	Private Sub lblTopo_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Me.zzCreateTopo()
	End Sub


	Private Sub frmTopoActions_Disposed(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Disposed
		Dim sTest As String = "a"
		Try

			Dim baCalculateSetting() As Boolean = {Me.chkBamash.Checked}
			sTest = "b"
			zzCalculateSetting = baCalculateSetting
			zzLotNameSetting = Me.chkLotNameNum.Checked()

			sTest = "c"
			If Me.cmbBlockFormat IsNot Nothing Then
				sTest = "d"
				Dim iaFormatsSetting() As Integer = {Me.cmbBlockFormat.SelectedIndex + 1 _
				  , Me.cmbTopoFormat.SelectedIndex + 1 _
				  , Me.cmbLanduseFormat.SelectedIndex + 1}
				sTest = "e"
				TPlanGraph.TplnProject.FormatsSetting = iaFormatsSetting
				sTest = "f"
				If Me.cmbCoordFormat.SelectedItem IsNot Nothing Then
					sTest = "fa"
					TPlanGraph.TplnProject.CoordinateFormatSetting = Me.cmbCoordFormat.SelectedItem.ToString()
				End If

				sTest = "g"
			End If
			sTest = "K"
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "frmTopoActions - frmTopoActions_Disposed")
		End Try

	End Sub

	Private Sub frmTopoActions_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.DoubleClick
		System.Windows.Forms.MessageBox.Show("frmTopoActions_DoubleClick", "")
		Try
			Me.Hide()
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CTABLESTYLE", "Legend")
			System.Windows.Forms.MessageBox.Show(CStr(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CTABLESTYLE")), "CTABLESTYLE")
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.Message, "Report - zzInitTextSile_1xab")
		End Try

	End Sub

	Private Sub frmTopoActions_FormClosing(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		Dim iCloseReason As CloseReason = e.CloseReason

		Select Case iCloseReason
			Case CloseReason.UserClosing
				e.Cancel = True
				Me.Hide()
			Case Else
				System.Windows.Forms.MessageBox.Show(iCloseReason.ToString(), "frmTopoActions CloseReason 29_290")

		End Select

	End Sub

	Private Sub zzMyInitializeComponent()

		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow

		Me.tbpTopo = New System.Windows.Forms.TabPage
		Me.tbpReports = New System.Windows.Forms.TabPage
		Me.tbpParameters = New System.Windows.Forms.TabPage

		Me.cmdFix = New TabButton(moLabelFont)
		Me.cmdMark = New TabButton(moLabelFont)
		Me.cmdBuild = New TabButton(moLabelFont)
		Me.cmdKill = New TabButton(moLabelFont)
		Me.cmdShow = New TabButton(moLabelFont)

		Me.cmdEraseTopoGeometria = New TabButton(moLabelFont)

		Me.cmdCalculate = New TabButton(moLabelFont)
		Me.cmdInsertRep = New TabButton(moLabelFont)
		Me.cmdExpExcel = New TabButton(moLabelFont)


		Me.cmdRegen = New TabButton(moLabelFont)
		Me.cmdClose = New TabButton(moLabelFont)
		Me.cmdExit = New TabButton(moLabelFont)


		Me.chkIgnoreIncompleteArea = New CheckBox
		Me.chkHighlightSliver = New CheckBox
		Me.chkTopoLayersOn = New CheckBox

		Me.chkBamash = New CheckBox
		'	Me.chkApproved = New CheckBox
		'	Me.chkProposed = New CheckBox
		'	Me.chkParcel = New CheckBox
		'	Me.chkMerge = New CheckBox
		'	Me.chkUnion = New CheckBox

		Me.txtTolerance = New TextBox
		Me.lblTolerance = New Label
		Me.lblErrors = New Label


		'  Me.lstReports = New ListBox()
		Me.nudSteps = New System.Windows.Forms.NumericUpDown
		Me.nudErrors = New System.Windows.Forms.NumericUpDown
		Me.tabMain = New System.Windows.Forms.TabControl

		Me.dgvMain = New System.Windows.Forms.DataGridView
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		Me.tabMain.Controls.Add(Me.tbpTopo)
		'	Me.tabMain.Controls.Add(Me.tbpReports)
		Me.tabMain.Controls.Add(Me.tbpParameters)

		'
		'tabMain
		'
		Me.tabMain.Location = New System.Drawing.Point(0, 0)
		Me.tabMain.Name = "tabMain"
		Me.tabMain.SelectedIndex = 0
		Me.tabMain.Size = New System.Drawing.Size(549, 300)
		Me.tabMain.Font = moLabelFont
		Me.tabMain.TabIndex = 7

		'
		'dgvMain
		'
		With Me.dgvMain
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(4, 32)
			.Name = "dgvMain"
			.Font = moLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 24
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = moBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(294, 224)	'224

			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False
		End With



		Me.tbpTopo.Controls.Add(Me.dgvMain)
		'
		'tbpTopo
		'
		With Me.tbpTopo
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpTopo"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 8
			.Text = "Topology"
			.Font = moLabelFont
			.UseVisualStyleBackColor = True
		End With
		'
		'tbpReports
		'
		With Me.tbpReports
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpReports"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 6
			.Text = "Reports"
			.UseVisualStyleBackColor = True
		End With
		'
		'tbpReports
		'
		With Me.tbpParameters
			.Location = New System.Drawing.Point(4, 22)
			.Name = "tbpParameters"
			.Padding = New System.Windows.Forms.Padding(3)
			.Size = New System.Drawing.Size(576, 304)
			.TabIndex = 9
			.Text = "Parameters"
			.UseVisualStyleBackColor = True
		End With

		'
		'cmdFix
		'
		With Me.cmdFix
			.Location = New System.Drawing.Point(64, 4)
			.Name = "cmdFix"
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 9
			'   .Font = moLabelFont
			.Text = "Fix"
		End With

		'
		'cmdMark
		'
		With Me.cmdMark
			.Location = New System.Drawing.Point(112, 4)
			.Name = "cmdMark"
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			.Text = "Mark"

		End With
		'
		'cmdBuild
		'
		With Me.cmdBuild
			.Location = New System.Drawing.Point(336, 4)
			.Name = "cmdBuild"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 11
			'   .Font = moLabelFont
			.Text = "Build"
		End With

		'
		'cmdKill
		'
		With Me.cmdKill
			.Location = New System.Drawing.Point(384, 4)
			.Name = "cmdKill"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Kill"
		End With

		'
		'cmdShow
		'
		With Me.cmdShow
			.Location = New System.Drawing.Point(432, 4)
			.Name = "cmdShow"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Show"
		End With

		'
		'cmdEraseTopoGeometria
		'
		With Me.cmdEraseTopoGeometria
			.Location = New System.Drawing.Point(480, 4)
			.Name = "cmdEraseTopoGeometria"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Erase"
		End With

		'
		'cmdCalculate
		'
		With Me.cmdCalculate
			.Location = New System.Drawing.Point(40, 168)
			.Name = "cmdCalculate"
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Calculate"
		End With


		'
		'cmdInsertRep
		'
		With Me.cmdInsertRep
			.Location = New System.Drawing.Point(356, 155)
			.Name = "cmdInsertRep"
			.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 13
			'    .Font = moLabelFont
			.Text = "Insert"
		End With
		'
		'cmdExpExcel
		'
		With Me.cmdExpExcel
			.Location = New System.Drawing.Point(356, 183)
			.Name = "cmdExpExcel"
			.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 19
			'    .Font = moLabelFont
			.Text = "Excel"
		End With


		'
		'cmdRegen
		'
		With Me.cmdRegen
			.Location = New System.Drawing.Point(336, 244)
			.Name = "cmdRegen"
			.Size = New System.Drawing.Size(52, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "Regen"
		End With

		'
		'cmdClose
		'
		With Me.cmdClose
			.Location = New System.Drawing.Point(396, 244)
			.Name = "cmdClose"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "Close"
		End With
		'
		'cmdExit
		'
		With Me.cmdExit
			.Location = New System.Drawing.Point(456, 244)
			.Name = "cmdExit"
			'   .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 15
			'    .Font = moLabelFont
			.Text = "Exit"
		End With

		'
		'chkHighlightSliver
		'
		With Me.chkHighlightSliver
			.Location = New System.Drawing.Point(302, 36)
			.Name = "chkHighlightSliver"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "Highlight"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With

		'
		'chkIgnoreIncompleteArea
		'
		With Me.chkIgnoreIncompleteArea
			' .Location = New System.Drawing.Point(302, 60)
			.Location = New System.Drawing.Point(376, 36)

			.Name = "chkIgnoreIncompleteArea"
			' .Size = New System.Drawing.Size(64, 24)
			.Size = New System.Drawing.Size(60, 24)
			.TabIndex = 16
			.Font = moLabelFont
			.Text = "Ignore"
			.FlatStyle = FlatStyle.Popup
			.ThreeState = False
		End With

		'
		'chkTopoLayersOn
		'
		With Me.chkTopoLayersOn
			.Location = New System.Drawing.Point(402, 36)
			.Name = "chkTopoLayersOn"
			.Size = New System.Drawing.Size(96, 24)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "Layers On"
			.FlatStyle = FlatStyle.Popup
			.ThreeState = False
			.Visible = False
		End With
		'
		'chkBamash
		'
		With Me.chkBamash
			.Location = New System.Drawing.Point(8, 8)
			.Name = "chkBamash"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 22
			.Font = moLabelFont
			.Text = zzGetText(2, 2)
			.FlatStyle = FlatStyle.Popup
			.ThreeState = False
		End With		  '






	 


		'
		'txtTolerance
		'
		With Me.txtTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(436, 38)
			.Name = "txtTolerance"
			.Size = New System.Drawing.Size(36, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "0.01"
			.TextAlign = HorizontalAlignment.Left
		End With
		'
		'lblTolerance
		'
		With Me.lblTolerance
			'.Location = New System.Drawing.Point(342, 86)
			.Location = New System.Drawing.Point(474, 40)
			.Name = "lblTolerance"
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "Tolerance"

		End With
		'
		'lblErrors
		'
		With Me.lblErrors
			.Location = New System.Drawing.Point(212, 6)
			.Name = "lblErrors"
			.Size = New System.Drawing.Size(44, 24)
			.TabIndex = 29
			.Font = moLabelFont
			.Text = "Errors:"

		End With

		'
		'nudSteps
		'
		With Me.nudSteps
			.Location = New System.Drawing.Point(4, 4)
			.Name = "nudSteps"
			.Size = New System.Drawing.Size(32, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.One
			.Maximum = Decimal.One + Decimal.One
			.TabIndex = 19
		End With
		'
		'nudErrors
		'
		With Me.nudErrors
			.Location = New System.Drawing.Point(256, 4)
			.Name = "nudErrors"
			.Size = New System.Drawing.Size(42, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.Zero
			.Maximum = Decimal.Zero
			.TabIndex = 20
		End With
		Me.chkLotNameNum = New System.Windows.Forms.CheckBox
		Me.chkLotNameNum.Checked = zzLotNameSetting

		Me.Controls.Add(Me.tabMain)
		Me.tbpTopo.Controls.Add(Me.cmdFix)
		Me.tbpTopo.Controls.Add(Me.cmdMark)
		Me.tbpTopo.Controls.Add(Me.cmdBuild)
		Me.tbpTopo.Controls.Add(Me.cmdKill)
		Me.tbpTopo.Controls.Add(Me.cmdShow)

		Me.tbpTopo.Controls.Add(Me.cmdEraseTopoGeometria)
		Me.tbpTopo.Controls.Add(Me.cmdRegen)
		Me.tbpTopo.Controls.Add(Me.cmdClose)
		Me.tbpTopo.Controls.Add(Me.cmdExit)

		Me.tbpTopo.Controls.Add(Me.chkHighlightSliver)
		Me.tbpTopo.Controls.Add(Me.chkIgnoreIncompleteArea)
		Me.tbpTopo.Controls.Add(Me.chkTopoLayersOn)

		Me.tbpTopo.Controls.Add(Me.txtTolerance)
		Me.tbpTopo.Controls.Add(Me.lblTolerance)
		Me.tbpTopo.Controls.Add(Me.lblErrors)

		Me.tbpTopo.Controls.Add(Me.nudSteps)
		Me.tbpTopo.Controls.Add(Me.nudErrors)


		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Const iXTop As Integer = 72
		For iIndex As Integer = 0 To miToposUB
			Me.moaLabels(iIndex) = New LabelInd(iIndex)
			If iIndex = miActionDflt Then
				Me.moaLabels(iIndex).Font = moLabelSelectFont
				Me.moaLabels(iIndex).ForeColor = moLabelSelectColor
			Else
				Me.moaLabels(iIndex).Font = moLabelFont
				Me.moaLabels(iIndex).ForeColor = moLabelColor
			End If
			Me.moaLabels(iIndex).Location = New System.Drawing.Point(288, iXTop + 4 + 24 * iIndex)
			Me.moaChecks(iIndex) = New TopoCheck()
			Me.moaChecks(iIndex).Location = New System.Drawing.Point(492, iXTop + 24 * iIndex)
			Me.moaCheckButtons(iIndex) = New CheckButton(iIndex)
			Me.moaCheckButtons(iIndex).Location = New System.Drawing.Point(510, iXTop + 2 + 24 * iIndex)
			Me.tbpTopo.Controls.Add(Me.moaLabels(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaChecks(iIndex))
			Me.tbpTopo.Controls.Add(Me.moaCheckButtons(iIndex))

			AddHandler moaLabels(iIndex).Click, AddressOf lblTopo_Click
			AddHandler moaLabels(iIndex).DoubleClick, AddressOf lblTopo_DoubleClick
			AddHandler moaCheckButtons(iIndex).CheckStateChanged, AddressOf CheckButton_CheckStateChanged

		Next
		miCurrentTopoDefID = TopoDefs.miaTopoIDs(miActionDflt)
		Me.nudSteps.Value = Decimal.One
		zzLoadData(miActionDflt)
		Me.nudSteps.Maximum = miStepNum(miActionDflt)
		zzSetGridColumns()
		moCurrentLabel = Me.moaLabels(miActionDflt)
		Dim oTopoDef As TopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			Me.cmdEraseTopoGeometria.Enabled = oTopoDef.LinkLayersExists
			Me.cmdFix.Enabled = oTopoDef.CleanupEnabled
			Me.cmdMark.Enabled = oTopoDef.CleanupEnabled
		End If

		' zzTopoLayersIsOn()
		Me.zzTopologiesLayersIsOn()


	End Sub


	Private Function zzSetGridColumns() As System.Windows.Forms.DataGridViewComboBoxColumn
		Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oColumn As DataGridViewTextBoxColumn

		Try
			With oCmbColumn
				.Name = msActionIDFldName
				.DataPropertyName = "ActionID"
				.HeaderText = "Actions"
				.Width = 152
				.Items.Clear()
				.FlatStyle = FlatStyle.Standard
				If miCurrentTopoDefID.TopoIsMerge Then
					.Items.AddRange(TopoCreator.CleanupActionItemsForMerge())
				Else
					.Items.AddRange(TopoCreator.CleanupActionItems())
				End If
				.MaxDropDownItems = .Items.Count
				.ValueMember = DMCommon.ItemData.ValueMember
				.DisplayMember = DMCommon.ItemData.DisplayMember
				.SortMode = DataGridViewColumnSortMode.NotSortable
				'.ReadOnly = TPlanGraph.TplnProject.TopoIsUnion(miCurrentTopoID)
			End With
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - zzSetGridColumns_12")
		End Try

		Try
			Me.dgvMain.Columns.Add(oCmbColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - zzSetGridColumns_10")
		End Try

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.Name = msToleranceFldName
			.HeaderText = "Tolerance"
			.Width = 58
			.Name = "Tolerance"
			.DataPropertyName = "Tolerance"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvMain.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - zzSetGridColumns_2")
		End Try

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.HeaderText = "Errors"
			.Width = 40
			.Name = msErrorsFldName
			.ReadOnly = True
			.DataPropertyName = msErrorsFldName
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvMain.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - zzSetGridColumns_1")
		End Try
		Return oCmbColumn
	End Function

	Private Sub zzLoadData(ByVal iTopoTypeIndex As Integer)

		Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps.Value)
		Dim sStepNo As String = Convert.ToString(Me.nudSteps.Value)

		If moDataTable(iTopoTypeIndex) Is Nothing Then
			moDataTable(iTopoTypeIndex) = New DataTable("TopoType" & CStr(miCurrentTopoDefID.ID))
			Dim sSelectComText As String = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM(CleanupActions) WHERE (CleanupActions.TopologyType=" & CStr(miCurrentTopoDefID.ID) & ") AND (CleanupActions.Step=" & sStepNo & ") ORDER BY CleanupActions.ActionNo"
			sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM(CleanupActions) WHERE (CleanupActions.TopologyType=" & CStr(miCurrentTopoDefID.ID) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

			Dim sUpdateComTextAAA As String = "UPDATE Localities SET Actual = ? WHERE ID = ?"
			moOleDbDataAdapter(iTopoTypeIndex) = TPlServerDB.ServerDB.GetDataAdapter(sSelectComText, True)
			If moOleDbDataAdapter(iTopoTypeIndex) IsNot Nothing Then
				moOleDbDataAdapter(iTopoTypeIndex).Fill(moDataTable(iTopoTypeIndex))
				moDataTable(iTopoTypeIndex).Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
				moDataTable(iTopoTypeIndex).Columns.Add(msPointsFldName, System.Type.GetType("TopoManager.TplnPointArray"))
				Dim iRowCount As Integer = moDataTable(iTopoTypeIndex).Rows.Count
				If iRowCount > 0& Then
					Dim oRow As DataRow = moDataTable(iTopoTypeIndex).Rows.Item(iRowCount - 1)
					miStepNum(iTopoTypeIndex) = DirectCast(oRow.Item("Step"), Integer)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("DataAdapter was not found", "27_514")
			End If
		End If
		Dim oColumn As DataColumn = moDataTable(iTopoTypeIndex).Columns("Step")
		oColumn.DefaultValue = iStepNo

		moCurrentView = New DataView(moDataTable(iTopoTypeIndex), "Step=" & sStepNo & "", "", DataViewRowState.CurrentRows)
		Me.dgvMain.DataSource = moCurrentView


	End Sub

	Private Sub zzCleanup(ByVal bFix As Boolean)
		Dim iActionUB As Integer = Me.dgvMain.RowCount - 2
		Dim iActionID As Integer
		Dim dTolerance As Double
		If iActionUB >= 0 Then
			Dim oaActionVar(iActionUB) As ActionVar
			ReDim moaErrorPoints(iActionUB)
			Dim iaErrors() As Integer
			Me.Cursor = Cursors.WaitCursor
			Dim sTest As String = "a"
			Try
				For iIndex As Integer = 0 To iActionUB
					If Not Me.dgvMain.Rows.Item(iIndex).IsNewRow Then
						iActionID = DirectCast(Me.dgvMain.Rows.Item(iIndex).Cells(msActionIDFldName).Value, Integer)
						dTolerance = Common.CDblN(Me.dgvMain.Rows.Item(iIndex).Cells(msToleranceFldName).Value)
						oaActionVar(iIndex) = New ActionVar(iActionID, dTolerance)
					End If
				Next
				If iActionUB >= 0 Then

					sTest = "ea"
					Dim oTopoDef As TopoDef = zzLoadTopoDef()
					sTest = "eb"
					If oTopoDef IsNot Nothing Then
						sTest = "ec"
						Try
							iaErrors = TopoCreator.Cleanup(oaActionVar, oTopoDef, bFix, moaErrorPoints)
						Catch oEx As Exception
							'  System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - Cleanup-C918")
							Common.GetMapTopoEx(oEx, "C919a")
							Exit Sub
						End Try

						sTest = "eca"
						'    iaErrors = TopoCreator.Cleanup(oaActionVar, oTopoDef, bFix, moaErrorPoints)
						sTest = "ed"
						Dim oDataTable As DataTable = moDataTable(moCurrentLabel.Index)
						sTest = "ee"
						Dim oDataRow As DataRow
						'  System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count - 1), "AfterCleanup")
						For iIndex As Integer = 0 To iaErrors.GetUpperBound(0)
							sTest = "ek" & CStr(iIndex)
							oDataRow = oDataTable.Rows(iIndex)
							sTest = "em" & CStr(iIndex)
							If oDataRow.RowState <> DataRowState.Deleted Then
								sTest = "en" & CStr(iIndex)
								oDataRow.BeginEdit()
								sTest = "ep" & CStr(iIndex)
								oDataRow.Item(msErrorsFldName) = iaErrors(iIndex)
								sTest = "eq" & CStr(iIndex)
								oDataRow.Item(msPointsFldName) = moaErrorPoints(iIndex)
								sTest = "eqb" & CStr(iIndex)

								oDataRow.EndEdit()
								sTest = "er" & CStr(iIndex)
							Else
								System.Windows.Forms.MessageBox.Show("RowDeleted", "Design")
							End If
							sTest = "es" & CStr(iIndex)
						Next
						Try
							miCurrentAction = Me.dgvMain.CurrentRow.Index
						Catch ex As Exception
							miCurrentAction = -1
						End Try

					End If

					sTest = "g"
					''  zzDispArray(iaErrors)
					'T   Me.dgvMain.BeginEdit(True)
					'T    For iIndex As Integer = 0 To iActionUB
					'T If Not Me.dgvMain.Rows.Item(iIndex).IsNewRow Then
					'T Me.dgvMain.Rows.Item(iIndex).Cells(msErrorsFldName).Value = iaErrors(iIndex)
					'T  Me.dgvMain.UpdateCellValue(2, iIndex)
					'T  End If
					'T      Next
					'T   Me.dgvMain.EndEdit()

				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "frmTopoActions - zzCleanup")
			End Try
			Me.Cursor = Cursors.Default

		End If
	End Sub
	Private Sub zzEraseTopoGeometria()
		Dim oTopoDef As TopoDef = zzLoadTopoDef()
		If oTopoDef IsNot Nothing Then
			'	AcadTransaction.Start()
			AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)

			AcadTransaction.ClearAcadPoints(oTopoDef.LinkLayer, False)
			AcadTransaction.EraseLinks(oTopoDef.LinkLayer, False)
			AcadDocument.Unlock()
			'	AcadTransaction.Terminate()
		End If



	End Sub

	Private Sub zzDispArray(ByVal iaVal() As Integer)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & iaVal(iIndex).ToString()
		Next
		System.Windows.Forms.MessageBox.Show(sOut, "Errors")
	End Sub
	Public Sub RefreshLayers()
		Me.zzTopologiesLayersIsOn()
	End Sub
	Public Sub CreateTopo()
		Me.zzCreateTopo()
	End Sub
	Public Sub DeleteTopo()
		Me.zzDeleteTopo()
	End Sub
	Public Sub RefreshTopo()
		zzResetChecks()
	End Sub

	Private Sub zzCreateTopo()

		Dim oTopoDef As TopoDef = zzLoadTopoDef()
		Dim stTopoDefID As TopoDefID
		Dim iTopoTypeIndex As Integer
		If oTopoDef IsNot Nothing Then
			Me.Cursor = Cursors.WaitCursor
			stTopoDefID = oTopoDef.ID
			Dim dTolerance As Double
			If stTopoDefID.TopoIsUnion Then
				TopoCreator.UnionTopo(oTopoDef)
			Else
				If Information.IsNumeric(Me.txtTolerance.Text) Then
					dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
					TopoCreator.CreateTopology(oTopoDef, Me.chkIgnoreIncompleteArea.Checked, Me.chkHighlightSliver.Checked, dTolerance)
				Else
					Beep()
				End If
			End If

			iTopoTypeIndex = moCurrentLabel.Index
			Me.moaChecks(iTopoTypeIndex).Checked = TopoCreator.TopologyExists(oTopoDef)
			Me.Cursor = Cursors.Default
		End If

	End Sub
	Private Sub zzTopoLayersIsOnAAA_BBB()
		Dim iTopoIndex As Integer
		iTopoIndex = moCurrentLabel.Index

		Dim saTopoLayers() As String = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()
		mbCodeExecuting = True
		Me.chkTopoLayersOn.CheckState = AcadTransaction.LayersIsOn(saTopoLayers, True)
		mbCodeExecuting = False
	End Sub
	Private Sub zzTopologiesLayersIsOn(Optional ByVal iExcept As Integer = -1)
		Dim saTopoLayers() As String
		mbCodeExecuting = True
		AcadTransaction.Start()
		For iTopoIndex As Integer = 0 To miToposUB
			If iTopoIndex <> iExcept Then
				saTopoLayers = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()
				moaCheckButtons(iTopoIndex).CheckState = AcadTransaction.LayersIsOn(saTopoLayers, False)
			End If
		Next
		AcadTransaction.Terminate()
		mbCodeExecuting = False

	End Sub
	Private Function zzLoadTopoDef() As TopoDef
		Dim iTopoIndex As Integer
		Dim bChecked As Boolean = False
		iTopoIndex = moCurrentLabel.Index

		If TopoDefs.moaTopoDefs(iTopoIndex) Is Nothing Then
			System.Windows.Forms.MessageBox.Show("", "33_155")
			TopoDefs.moaTopoDefs(iTopoIndex) = New TopoDef(TopoDefs.miaTopoIDs(iTopoIndex))
			System.Windows.Forms.MessageBox.Show(TopoDefs.miaTopoIDs(iTopoIndex).BaseID.ToString(), "33_200")
		End If
		moaChecks(iTopoIndex).Checked = False
		bChecked = TopoCreator.TopologyExists(TopoDefs.moaTopoDefs(iTopoIndex))
		moaChecks(iTopoIndex).Checked = bChecked
		Return TopoDefs.moaTopoDefs(iTopoIndex)
	End Function
	Private Sub zzResetChecks()
		For iIndex As Integer = 0 To miToposUB
			If TopoDefs.moaTopoDefs(iIndex) IsNot Nothing Then
				moaChecks(iIndex).Checked = TopoCreator.TopologyExists(TopoDefs.moaTopoDefs(iIndex))
			End If
		Next
	End Sub
	Private Sub zzDeleteTopo()
		Dim oTopoDef As TopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			TopoCreator.DeleteTopology(oTopoDef)
			Dim iTopoTypeIndex As Integer
			iTopoTypeIndex = moCurrentLabel.Index
			Me.moaChecks(iTopoTypeIndex).Checked = TopoCreator.TopologyExists(oTopoDef)
		End If
	End Sub
	Private Sub zzShowTopo()
		Dim oTopoDef As TopoDef = zzLoadTopoDef()

		If oTopoDef IsNot Nothing Then
			TopoCreator.ShowTopology(oTopoDef)
		End If

	End Sub
	Private Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
		Try
			Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, bReturnEmpty)
		Catch oEx As Exception
			Return String.Empty
		End Try

	End Function
	Private Sub zzFillFormatRow(ByRef oFormatCombo As ComboBox, ByVal iSectionID As Integer)
		Dim iItemIndex As Integer = 0
		Dim sItemText As String
		Do
			sItemText = zzGetText(iItemIndex, iSectionID, True)
			If sItemText.Length = 0 Then Exit Do
			oFormatCombo.Items.Add(New DMCommon.ItemData(iItemIndex, sItemText))
			iItemIndex += 1
		Loop
	End Sub
	Private Sub zzFillDoubleFormatRow(ByRef oFormatCombo As ComboBox)

		Dim sItemText As String = String.Empty

		For iItemIndex As Integer = 0 To 4
			sItemText &= "0"
			oFormatCombo.Items.Add(New DMCommon.ItemData(iItemIndex, sItemText))
			If iItemIndex = 0 Then
				sItemText &= "."
			End If
		Next



	End Sub
	Private Sub zzRegen()
		Dim oAcadEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Try
			Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - zzRegen")
		End Try


	End Sub

	Private Sub zzActualizeAAAA()
		For iIndex As Integer = 0 To miToposUB
			If TopoDefs.moaTopoDefs(iIndex).LinkLayersExists Then
				DWGInfo.AddLayer(iIndex, TopoDefs.moaTopoDefs(iIndex).LinkLayer)
			End If
		Next
		DWGInfo.Open()
		Dim iLayerIndex() As Integer = Nothing, bHasEntity() As Boolean = Nothing
		DWGInfo.LayerStatus(iLayerIndex, bHasEntity)
		For iIndex As Integer = 0 To iLayerIndex.GetUpperBound(0)
			TopoDefs.moaTopoDefs(iLayerIndex(iIndex)).LinkLayerEmpty = Not bHasEntity(iIndex)
		Next
	End Sub
	Private Sub cmdInsertRep_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdInsertRep.Click


		Dim oRepApp As AcadReport.Application = New AcadReport.Application
		zzInsertReport(oRepApp)

	End Sub
	Private Sub cmdExpExcel_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExpExcel.Click
		Dim oRepApp As AcadReport.BaseReport = New ExcelReport.Application

		zzInsertReport(oRepApp)

	End Sub
	Private Sub zzInsertReport(ByVal oRepApp As AcadReport.BaseReport)
		Dim iDataOptions As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
		Dim oaOptionValues(0) As System.Object
		Me.Cursor = Cursors.WaitCursor
		If Me.lstReports.SelectedItem IsNot Nothing Then
			If Me.grbCalcOption.Enabled Then
				If Me.rdbCalc.Checked Then
					iDataOptions = TPlanGraph.enDataOptions.CalcMergeArea
					oaOptionValues(0) = Me.rdbCalc.Text
				ElseIf Me.rdbAcad.Checked Then
					iDataOptions = TPlanGraph.enDataOptions.AcadArea
					oaOptionValues(0) = Me.rdbAcad.Text
				End If
			End If

			Dim oDataView As DataView = Nothing
			Dim iaDataColumns() As Integer = Nothing
			Dim oaTotals() As System.Object = Nothing
			Select Case moSelectedReportItem.RepIndex
				Case TPlServerDB.enResourceTheme.AcRepContent
					oDataView = TPlanGraph.TplnParcel.BlockView
				Case TPlServerDB.enResourceTheme.AcRepPlanParcels
					TPlanGraph.TplnParcel.GetInPlanData(oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepLotsK
					TPlanGraph.TplnLot.GetMainData(TPlanGraph.enTopoPurpose.Approved, iDataOptions, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepLotsM
					TPlanGraph.TplnLot.GetMainData(TPlanGraph.enTopoPurpose.Proposed, iDataOptions, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepParcelLuseK
					TPlanGraph.TplnParcel.GetLanduseData(TPlanGraph.enTopoPurpose.Approved, iDataOptions, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepParcelLuseM
					TPlanGraph.TplnParcel.GetLanduseData(TPlanGraph.enTopoPurpose.Proposed, iDataOptions, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepSumLuse
					TPlanGraph.TplnLot.GetSumLanduseData(oDataView, iDataOptions, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepBlockArea
					oDataView = TPlanGraph.TplnProject.GetBlockLegalArea
				Case TPlServerDB.enResourceTheme.AcRepLotContent
					oDataView = TPlanGraph.TplnParcel.LotContentView
				Case TPlServerDB.enResourceTheme.AcRepLotContentArea
					TPlanGraph.TplnProject.LotContentAreaView(TPlanGraph.enOverlayIndex.ApprMerge, True, oDataView, iaDataColumns)
				Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel
					TPlanGraph.TplnProject.LotContentAreaView(TPlanGraph.enOverlayIndex.ApprMerge, False, oDataView, iaDataColumns)

			End Select

			If oDataView IsNot Nothing Then
				oRepApp.Open(moSelectedReportItem.RepIndex)
				oRepApp.MainView = oDataView

				If iaDataColumns IsNot Nothing Then
					oRepApp.DataColumns = iaDataColumns
				End If
				If oaTotals IsNot Nothing Then
					oRepApp.Totals = oaTotals
				End If
				If oaOptionValues(0) IsNot Nothing Then
					oRepApp.OptionValues = oaOptionValues
				End If


				If oRepApp.AcadModel Then
					Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
					Dim bCurrentLayerOK As Boolean = True
					Try
						oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, True)
					Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
						AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "frmTopoActions - zzInsertReport_02")
					End Try

					bCurrentLayerOK = AcadTransaction.SetCurrentLayer(String.Empty, enLayerFunction.Report, True, True, True)
					If oDocLock IsNot Nothing Then
						oDocLock.Dispose()
						oDocLock = Nothing
					End If
					If bCurrentLayerOK Then
						Me.Hide()
						Common.SetAcadFocus()
						oRepApp.Insert()
						Me.Show()
					End If
				Else
					oRepApp.Insert()
				End If
			End If
		End If
		Me.Cursor = Cursors.Default
	End Sub
	Public Sub Mark()
		zzCleanup(False)
	End Sub
	Private Sub cmdMark_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdMark.Click
		zzCleanup(False)
	End Sub
	Private Sub cmdBuild_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdBuild.Click
		Me.zzCreateTopo()
	End Sub
	Private Sub cmdKill_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdKill.Click
		Me.Cursor = Cursors.WaitCursor
		Me.zzDeleteTopo()
		Me.Cursor = Cursors.Arrow
	End Sub
	Private Sub cmdEraseTopoGeometria_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdEraseTopoGeometria.Click
		Me.Cursor = Cursors.WaitCursor
		Me.zzEraseTopoGeometria()
		Me.Cursor = Cursors.Arrow
	End Sub
	Private Sub cmdFix_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdFix.Click
		zzCleanup(True)
	End Sub
	Private Class TabButton
		Inherits Button
		Public Sub New(ByVal oFont As Font)
			MyBase.Size = New System.Drawing.Size(44, 24)
			MyBase.Font = oFont
			MyBase.Cursor = Cursors.Hand
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Standard
			MyBase.UseVisualStyleBackColor = True
		End Sub
	End Class
	Private Class TopoCheck
		Inherits CheckBox
		Public Sub New()
			MyBase.Size = New System.Drawing.Size(16, 24)
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Standard
			MyBase.Enabled = False
			MyBase.ThreeState = True
			MyBase.CheckState = System.Windows.Forms.CheckState.Indeterminate
		End Sub
	End Class
	Private Class LabelInd
		Inherits System.Windows.Forms.Label
		Private miIndex As Integer
		Public Sub New(ByVal iIndex As Integer)
			miIndex = iIndex
			MyBase.AutoSize = False
			MyBase.Cursor = System.Windows.Forms.Cursors.Hand
			MyBase.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			MyBase.Name = "lblTopo" & CStr(iIndex)
			MyBase.Size = New System.Drawing.Size(200, 18)
			MyBase.TabIndex = iIndex
			MyBase.Text = zzGetText(iIndex, 6)
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			MyBase.TextAlign = ContentAlignment.MiddleLeft
			MyBase.BorderStyle = System.Windows.Forms.BorderStyle.None
		End Sub
		Public Property Index() As Integer
			Get
				Return miIndex
			End Get
			Set(ByVal iValue As Integer)
				miIndex = iValue
			End Set
		End Property
	End Class
	Private Class CheckButton
		Inherits CheckBox
		Private miIndex As Integer
		Public Sub New(ByVal iIndex As Integer)
			miIndex = iIndex
			MyBase.AutoSize = False
			MyBase.Cursor = System.Windows.Forms.Cursors.Hand
			MyBase.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			MyBase.Appearance = System.Windows.Forms.Appearance.Button
			MyBase.ThreeState = False
			MyBase.Name = "chkTopoLayer" & CStr(iIndex)
			MyBase.Size = New System.Drawing.Size(15, 18)
			MyBase.TabIndex = iIndex
			MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			MyBase.ImageAlign = ContentAlignment.MiddleCenter
			SetImage()
		End Sub
		Public Sub SetImage()
			If MyBase.CheckState = System.Windows.Forms.CheckState.Checked Then
				MyBase.Image = TopoManager.My.Resources.Resources.LayerOn
			ElseIf MyBase.CheckState = System.Windows.Forms.CheckState.Unchecked Then
				MyBase.Image = Global.TopoManager.My.Resources.Resources.LayerOff
			Else
				MyBase.Image = Global.TopoManager.My.Resources.Resources.QuestionMark
			End If

		End Sub
		Public Property Index() As Integer
			Get
				Return miIndex
			End Get
			Set(ByVal iValue As Integer)
				miIndex = iValue
			End Set
		End Property
	End Class
	Private Sub tabMain_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tabMain.SelectedIndexChanged
		Try
			If Me.lstReports Is Nothing AndAlso Me.tabMain.SelectedIndex = 1 Then
				Me.lstReports = New ListBox()
				Me.grbCalcOption = New System.Windows.Forms.GroupBox
				Me.rdbCalc = New System.Windows.Forms.RadioButton
				Me.rdbAcad = New System.Windows.Forms.RadioButton
				Me.grbCalcOption.SuspendLayout()
				'
				'lstReports
				'
				With Me.lstReports
					.Location = New System.Drawing.Point(240, 8)
					.Name = "lstReports"
					.Enabled = False
					.RightToLeft = System.Windows.Forms.RightToLeft.Yes
					.Size = New System.Drawing.Size(180, 135)
					.TabIndex = 13
					.Text = "Erase"
					.Font = moLabelFont
					.ValueMember = DMCommon.ItemData.ValueMember
					.DisplayMember = DMCommon.ItemData.DisplayMember
					For iIndex As Integer = 0 To miaReports.GetUpperBound(0)
						.Items.Add(New ReportItem(miaReports(iIndex), GetReportName(miaReports(iIndex)), miaReportOption(iIndex)))
					Next
				End With

				'
				'grbCalcOption
				'
				With Me.grbCalcOption
					.Controls.Add(Me.rdbCalc)
					.Controls.Add(Me.rdbAcad)
					.FlatStyle = System.Windows.Forms.FlatStyle.Popup
					.Location = New System.Drawing.Point(240, 149)
					.Name = "grbCalcOption"
					.RightToLeft = System.Windows.Forms.RightToLeft.Yes
					.Size = New System.Drawing.Size(108, 48)
					.TabIndex = 1
					.TabStop = False
					.Enabled = False
				End With

				'
				'rdbCalc
				'
				With Me.rdbCalc
					.AutoSize = True
					.Location = New System.Drawing.Point(4, 8)
					.Name = "rdbCalc"
					.Size = New System.Drawing.Size(90, 17)
					.TabIndex = 0
					.TabStop = True
					.Text = zzGetText(5, 2)
					.UseVisualStyleBackColor = True
					.RightToLeft = System.Windows.Forms.RightToLeft.Yes
					.Checked = True
				End With
				'
				'rdbAcad
				'
				With rdbAcad
					.AutoSize = True
					.Location = New System.Drawing.Point(4, 28)
					.Name = "rdbAcad"
					.Size = New System.Drawing.Size(90, 17)
					.TabIndex = 1
					.TabStop = True
					.Text = zzGetText(6, 2)
					.UseVisualStyleBackColor = True
					.RightToLeft = System.Windows.Forms.RightToLeft.Yes
				End With
				Me.tbpReports.Controls.Add(Me.lstReports)
				Me.tbpReports.Controls.Add(Me.cmdCalculate)
				Me.tbpReports.Controls.Add(Me.cmdInsertRep)
				Me.tbpReports.Controls.Add(Me.cmdExpExcel)
				Me.tbpReports.Controls.Add(Me.chkBamash)
				'	Me.tbpReports.Controls.Add(Me.chkApproved)
				'	Me.tbpReports.Controls.Add(Me.chkProposed)
				'	Me.tbpReports.Controls.Add(Me.chkParcel)
				'	Me.tbpReports.Controls.Add(Me.chkMerge)
				'	Me.tbpReports.Controls.Add(Me.chkUnion)
				Me.tbpReports.Controls.Add(Me.grbCalcOption)

				Me.grbCalcOption.ResumeLayout(False)
				Me.grbCalcOption.PerformLayout()
				Dim baCalculateSetting() As Boolean = zzCalculateSetting()


				Me.chkBamash.Checked = baCalculateSetting(0)
				'	Me.chkApproved.Checked = baCalculateSetting(1)
				'	Me.chkProposed.Checked = baCalculateSetting(2)
				'	Me.chkMerge.Checked = baCalculateSetting(3)
				'	Me.chkUnion.Checked = baCalculateSetting(4)

			ElseIf cmbBlockFormat Is Nothing AndAlso Me.tabMain.SelectedIndex = 2 Then
				Dim iaFormatsSetting() As Integer = TPlanGraph.TplnProject.FormatsSetting()
				Me.grbFormats = New System.Windows.Forms.GroupBox
				Me.grbNumberFormat = New System.Windows.Forms.GroupBox
				Me.cmbBlockFormat = New System.Windows.Forms.ComboBox
				Me.cmbTopoFormat = New System.Windows.Forms.ComboBox
				Me.cmbLanduseFormat = New System.Windows.Forms.ComboBox
				Me.cmbCoordFormat = New System.Windows.Forms.ComboBox
				Me.cmbAreaFormat = New System.Windows.Forms.ComboBox

				Me.lblBlockFormat = New System.Windows.Forms.Label
				Me.lblTopoFormat = New System.Windows.Forms.Label
				Me.lblLanduseFormat = New System.Windows.Forms.Label
				Me.lblCoordinateFormat = New System.Windows.Forms.Label
				Me.lblAreaFormat = New System.Windows.Forms.Label


				'
				'grbFormats
				'
				With Me.grbFormats
					.BackColor = System.Drawing.Color.Transparent
					.FlatStyle = System.Windows.Forms.FlatStyle.Flat
					.Location = New System.Drawing.Point(4, 4)
					.Name = "grbFormats"
					.Text = zzGetText(0, 11)
					.Size = New System.Drawing.Size(220, 112)
					.TabStop = False
				End With

				'
				'grbNumberFormat
				'
				With Me.grbNumberFormat
					.BackColor = System.Drawing.Color.Transparent
					.FlatStyle = System.Windows.Forms.FlatStyle.Flat
					.Location = New System.Drawing.Point(240, 4)
					.Name = "grbNumberFormat"
					.Text = zzGetText(10, 11)
					.Size = New System.Drawing.Size(160, 80)
					.TabStop = False
				End With

				'
				'cmbBlockFormat
				'
				Me.cmbBlockFormat.FormattingEnabled = True
				Me.cmbBlockFormat.Location = New System.Drawing.Point(6, 18)
				Me.cmbBlockFormat.Name = "cmbBlockFormat"
				Me.cmbBlockFormat.Size = New System.Drawing.Size(115, 21)
				Me.cmbBlockFormat.TabIndex = 22
				zzFillFormatRow(Me.cmbBlockFormat, 3)
				Me.cmbBlockFormat.SelectedIndex = iaFormatsSetting(TPlanGraph.enFormatType.BlockLayer) - 1

				'
				'cmbTopoFormat
				'
				With Me.cmbTopoFormat
					.FormattingEnabled = True
					.Location = New System.Drawing.Point(6, 50)
					.Name = "cmbTopoFormat"
					.Size = New System.Drawing.Size(115, 21)
					.TabIndex = 23
					zzFillFormatRow(Me.cmbTopoFormat, 4)
					.SelectedIndex = iaFormatsSetting(TPlanGraph.enFormatType.Topology) - 1
				End With


				'
				'cmbLanduseFormat
				'
				With Me.cmbLanduseFormat
					.FormattingEnabled = True
					.Location = New System.Drawing.Point(6, 82)
					.Name = "cmbLanduseFormat"
					.Size = New System.Drawing.Size(115, 21)
					.TabIndex = 24
					zzFillFormatRow(Me.cmbLanduseFormat, 5)
					.SelectedIndex = iaFormatsSetting(TPlanGraph.enFormatType.Landuse) - 1
				End With
				'
				'cmbCoordFormat
				'
				With Me.cmbCoordFormat
					.FormattingEnabled = True
					.Location = New System.Drawing.Point(6, 18)
					.Name = "cmbCoordFormat"
					.Size = New System.Drawing.Size(72, 21)
					.ValueMember = DMCommon.ItemData.ValueMember
					.DisplayMember = DMCommon.ItemData.DisplayMember
					.TabIndex = 27
					zzFillDoubleFormatRow(Me.cmbCoordFormat)
					.SelectedText = TPlanGraph.TplnProject.CoordinateFormat
				End With
				'
				'cmbAreaFormat
				'
				With Me.cmbAreaFormat
					.FormattingEnabled = True
					.Location = New System.Drawing.Point(6, 50)
					.Name = "cmbAreaFormat"
					.Size = New System.Drawing.Size(72, 21)
					.ValueMember = DMCommon.ItemData.ValueMember
					.DisplayMember = DMCommon.ItemData.DisplayMember
					.TabIndex = 28
					zzFillDoubleFormatRow(Me.cmbAreaFormat)
					.SelectedText = TPlanGraph.TplnProject.AreaFormat
				End With
				'
				'chkLotNameNum
				'
				With Me.chkLotNameNum
					.Location = New System.Drawing.Point(12, 160)
					.Name = "chkLotNameNum"
					.Size = New System.Drawing.Size(120, 24)
					.TabIndex = 43
					.Font = moLabelFont
					.Text = zzGetText(16, 11)
					.FlatStyle = FlatStyle.Popup
					.ThreeState = False

				End With

				'
				'lblBlockFormat
				'
				With Me.lblBlockFormat
					.Location = New System.Drawing.Point(126, 18)
					.Name = "lblBlockFormat"
					.Size = New System.Drawing.Size(96, 21)
					.TabIndex = 29
					.Font = moLabelFont
					.Text = "Blocks & Layers"
					.Text = zzGetText(1, 11)
				End With
				'
				'lblTopoFormat
				'
				With Me.lblTopoFormat
					.Location = New System.Drawing.Point(126, 50)
					.Name = "lblTopoFormat"
					.Size = New System.Drawing.Size(96, 21)
					.TabIndex = 31
					.Font = moLabelFont
					.Text = "Topologies"
					.Text = zzGetText(2, 11)
				End With
				'
				'lblLanduseFormat
				'
				With Me.lblLanduseFormat
					.Location = New System.Drawing.Point(126, 82)
					.Name = "lblLanduseFormat"
					.Size = New System.Drawing.Size(96, 21)
					.TabIndex = 31
					.Font = moLabelFont
					.Text = zzGetText(3, 11)
				End With

				'
				'lblCoordinateFormat
				'
				With Me.lblCoordinateFormat
					.Location = New System.Drawing.Point(88, 18)
					.Name = "lblCoordinateFormat"
					.Size = New System.Drawing.Size(120, 21)
					.TabIndex = 31
					.Font = moLabelFont
					.Text = zzGetText(11, 11)
				End With
				'
				'lblAreaFormat
				'
				With Me.lblAreaFormat
					.Location = New System.Drawing.Point(88, 50)
					.Name = "lblAreaFormat"
					.Size = New System.Drawing.Size(120, 21)
					.TabIndex = 31
					.Font = moLabelFont
					.Text = zzGetText(12, 11)
				End With

				Me.grbFormats.Controls.Add(Me.cmbBlockFormat)
				Me.grbFormats.Controls.Add(Me.cmbTopoFormat)
				Me.grbFormats.Controls.Add(Me.cmbLanduseFormat)
				Me.grbFormats.Controls.Add(Me.lblBlockFormat)
				Me.grbFormats.Controls.Add(Me.lblTopoFormat)
				Me.grbFormats.Controls.Add(Me.lblLanduseFormat)
				Me.grbNumberFormat.Controls.Add(Me.cmbCoordFormat)
				Me.grbNumberFormat.Controls.Add(Me.cmbAreaFormat)
				Me.grbNumberFormat.Controls.Add(Me.lblCoordinateFormat)
				Me.grbNumberFormat.Controls.Add(Me.lblAreaFormat)



				Me.tbpParameters.Controls.Add(Me.grbFormats)
				Me.tbpParameters.Controls.Add(Me.grbNumberFormat)
				Me.tbpParameters.Controls.Add(Me.chkLotNameNum)


				'    Me.tbpParameters.Controls.Add(Me.cmbBlockFormat)
				'     Me.tbpParameters.Controls.Add(Me.cmbTopoFormat)
				'    Me.tbpParameters.Controls.Add(Me.cmbLanduseFormat)
				'    Me.tbpParameters.Controls.Add(Me.cmbCoordFormat)
				'  Me.tbpParameters.Controls.Add(Me.lblBlockFormat)
				'   Me.tbpParameters.Controls.Add(Me.lblTopoFormat)
				' Me.tbpParameters.Controls.Add(Me.lblLanduseFormat)
				'    Me.tbpParameters.Controls.Add(Me.lblCoordinateFormat)
			End If

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, Me.Name & " - tabMain_SelectedIndexChanged")
		End Try
		Select Case Me.tabMain.SelectedIndex
			Case 0
				Me.tbpTopo.Controls.Add(Me.cmdRegen)
				Me.tbpTopo.Controls.Add(Me.cmdClose)
				Me.tbpTopo.Controls.Add(Me.cmdExit)
			Case 1
				Me.tbpReports.Controls.Add(Me.cmdRegen)
				Me.tbpReports.Controls.Add(Me.cmdClose)
				Me.tbpReports.Controls.Add(Me.cmdExit)
			Case 2
				Me.tbpParameters.Controls.Add(Me.cmdRegen)
				Me.tbpParameters.Controls.Add(Me.cmdClose)
				Me.tbpParameters.Controls.Add(Me.cmdExit)
		End Select
		Select Case Me.tabMain.SelectedIndex
	
		End Select

	End Sub


	Private Shared Function GetReportName(ByVal iResourceTheme As TPlServerDB.enResourceTheme) As String
		Return TPlServerDB.TextResource.GetText(2, iResourceTheme, 0)
	End Function
	Private Class ReportItem
		Inherits DMCommon.ItemData
		Private miOptions As TPlanGraph.enDataOptions
		Public Sub New(ByVal iListIndex As TPlServerDB.enResourceTheme, ByVal sListDispData As String, ByVal iOptions As TPlanGraph.enDataOptions)
			MyBase.New(iListIndex, sListDispData)
			miOptions = iOptions
		End Sub
		Public Property RepIndex() As TPlServerDB.enResourceTheme
			Get
				Try
					Return CType(MyBase.ListIndex, TPlServerDB.enResourceTheme)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "ReportItem - RepIndex")
				End Try
			End Get
			Set(ByVal iValue As TPlServerDB.enResourceTheme)
				MyBase.ListIndex = iValue
			End Set
		End Property
		Public Property Options() As TPlanGraph.enDataOptions
			Get
				Return miOptions
			End Get
			Set(ByVal iValue As TPlanGraph.enDataOptions)
				miOptions = iValue
			End Set
		End Property
	End Class
	Private Shared Property zzLotNameSetting() As Boolean
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msLotNameNumSettingKey, "True")
			Dim bSetting As Boolean
			Try
				bSetting = Convert.ToBoolean(sSettting)
			Catch
				bSetting = True
			End Try

			Return bSetting
		End Get
		Set(ByVal bValue As Boolean)
			Dim sSetting As String
			sSetting = Convert.ToString(bValue)
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msLotNameNumSettingKey, sSetting)
		End Set
	End Property
	Private Shared Property zzCalculateSetting() As Boolean()
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msCalculateSettingKey, "31")
			Dim iSetting As Integer
			Try
				iSetting = Convert.ToInt32(sSettting)
			Catch
				iSetting = 31
			End Try
			Dim baOut(4) As Boolean
			Dim iDivisor As Integer = 1
			For iIndex As Integer = 0 To 4
				baOut(iIndex) = (iSetting And iDivisor) = iDivisor
				iDivisor *= 2
			Next
			Return baOut
		End Get
		Set(ByVal baValue() As Boolean)
			Dim iSetting As Integer
			Dim sSetting As String
			Dim iDivisor As Integer = 1

			For iIndex As Integer = 0 To 4
				If baValue(iIndex) Then iSetting += iDivisor
				iDivisor *= 2
			Next
			sSetting = Convert.ToString(iSetting)
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msCalculateSettingKey, sSetting)
		End Set
	End Property

	


	Private Sub cmdClose_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
		Me.Hide()
	End Sub

	Private Sub cmdExit_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExit.Click

		Me.DialogResult = System.Windows.Forms.DialogResult.Yes
		Me.Close()
		Me.Dispose()
	End Sub

	Private Sub cmdShow_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdShow.Click
		Me.zzShowTopo()
	End Sub

	Private Sub cmdRegen_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdRegen.Click

		Me.zzRegen()
	End Sub

	Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles nudSteps.ValueChanged
		If moCurrentLabel IsNot Nothing Then
			Me.zzLoadData(moCurrentLabel.Index)
		End If

	End Sub

	
	Private Sub lstReports_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles lstReports.SelectedIndexChanged
		If Me.lstReports.SelectedItem Is Nothing Then
			moSelectedReportItem = Nothing
			Me.grbCalcOption.Enabled = False
		Else
			moSelectedReportItem = DirectCast(Me.lstReports.SelectedItem, ReportItem)
			Me.grbCalcOption.Enabled = (moSelectedReportItem.Options <> TPlanGraph.enDataOptions.Default)
		End If
	End Sub

	Private Sub chkTopoLayersOn_CheckStateChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles chkTopoLayersOn.CheckStateChanged
		'   zzSetCheckState()
	End Sub
	Private Sub zzSetCheckState()
		If Not mbCodeExecuting Then
			Dim iTopoIndex As Integer
			iTopoIndex = moCurrentLabel.Index
			Dim saTopoLayers() As String = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()
			mbCodeExecuting = True

			Me.chkTopoLayersOn.CheckState = AcadTransaction.SetLayersOn(saTopoLayers, chkTopoLayersOn.Checked, True)
			AcadReport.AcadUtil.GetEditor().UpdateScreen()
			mbCodeExecuting = False
		End If
	End Sub

	Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles nudErrors.ValueChanged
		Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors.Value)
		If iErrIndex = 0 OrElse (moCurrentPoints Is Nothing) Then
			TPlanGraph.TplnProject.SetInitView()
		Else
			Try
				Dim oPoint As TPlnPoint = moCurrentPoints.Item(iErrIndex - 1)
				If oPoint IsNot Nothing Then
					zzZoomPoint(oPoint)
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - nudErrors_ValueChanged")
			End Try
		End If
	End Sub

	Private Sub zzZoomPoint(ByVal oPoint As TPlnPoint)
		Try
			Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
			oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
			oViewTableRecord.CenterPoint = oPoint.AcGePoint
			oViewTableRecord.Width = 10.0
			oViewTableRecord.Height = 10.0
			AcadReport.AcadUtil.GetEditor().SetCurrentView(oViewTableRecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - nudErrors_ValueChanged")
		End Try
	End Sub
	Private Sub CheckButton_CheckStateChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oCheckButton As CheckButton
		Dim iTopoIndex As Integer
		Dim iCheckState As System.Windows.Forms.CheckState

		oCheckButton = DirectCast(oSender, CheckButton)
		iTopoIndex = oCheckButton.Index
		oCheckButton.SetImage()
		If Not mbCodeExecuting Then
			Dim saTopoLayers() As String = TopoDefs.moaTopoDefs(iTopoIndex).GetAllLayers()

			iCheckState = AcadTransaction.SetLayersOn(saTopoLayers, oCheckButton.Checked, True)

			If oCheckButton.CheckState <> iCheckState Then
				mbCodeExecuting = True
				oCheckButton.CheckState = iCheckState
				mbCodeExecuting = False
			End If
			AcadReport.AcadUtil.GetEditor().UpdateScreen()
			zzTopologiesLayersIsOn()
		End If
	End Sub
	Private Sub dgvMain_RowEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMain.RowEnter
		If miCurrentAction <> -1 Then
			miCurrentAction = e.RowIndex
			Dim oDataRow As DataRow = moCurrentView.Item(miCurrentAction).Row
			Dim iErrorCount As Integer
			If oDataRow.IsNull(msPointsFldName) Then
				moCurrentPoints = Nothing
				iErrorCount = 0
			Else
				moCurrentPoints = DirectCast(oDataRow.Item(msPointsFldName), TplnPointArray)
				iErrorCount = moCurrentPoints.UpperBound + 1
			End If
			Try
				With Me.nudErrors
					.Maximum = Convert.ToDecimal(iErrorCount)
					.Value = Decimal.Zero
				End With
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - dgvMain_RowEnter")
			End Try
		End If
	End Sub
	Private Sub zzTestInsert()

		Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
		Dim saPrompt(1) As String
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		saPrompt(0) = "Enter start point of the report"
		saPrompt(1) = vbCrLf & "Enter end point of the report"
		Me.Hide()
		Try
			Dim iHWin As Integer = Common.GetActiveWindow()
			'   System.Windows.Forms.MessageBox.Show(CStr(iHWin), "GetActiveWindow")
			Dim sTitle As String = Common.GetActiveWinText()
			Dim iFocus As Integer = Common.GetFocus
			Dim iInputState As Integer = Common.GetInputState
			Dim iWinDC As Integer = Common.GetWindowDC(iHWin)
			AcadReport.AcadUtil.GetEditor().WriteMessage("TopoActions: " & CStr(iHWin) & ":" & sTitle & ":" & CStr(iFocus) & ":" & CStr(iInputState) & ":" & CStr(iWinDC))


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "39_210")
		End Try

		Common.SetAcadFocus()
		Dim bResp As Boolean = TPlanGraph.TplnProject.GetPoint("start point", tPoint)
		'   Me.Show()

	End Sub

	Private Sub cmbBlockFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbBlockFormat.SelectedIndexChanged
		AcadBlockDef.Format = Me.cmbTopoFormat.SelectedIndex + 1
	End Sub

	Private Sub cmbTopoFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbTopoFormat.SelectedIndexChanged
		TopoDef.Format = Me.cmbTopoFormat.SelectedIndex + 1
	End Sub
	Private Sub cmbLanduseFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbLanduseFormat.SelectedIndexChanged

		TPlanGraph.TplnProject.LandusesFormatID = Me.cmbLanduseFormat.SelectedIndex + 1

	End Sub

	Private Sub cmbCoordFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbCoordFormat.SelectedIndexChanged
		Try

			TPlanGraph.TplnProject.CoordinateFormat = Me.cmbCoordFormat.SelectedItem.ToString()
			RaiseEvent FormatChanged()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbCoordFormat_SelectedIndexChanged")

		End Try

	End Sub
	Private Sub cmbAreaFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbAreaFormat.SelectedIndexChanged
		Try

			TPlanGraph.TplnProject.AreaFormat = Me.cmbAreaFormat.SelectedItem.ToString()
			RaiseEvent FormatChanged()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbAreaFormat_SelectedIndexChanged")

		End Try

	End Sub
	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	Private Sub cmdCalculate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click

	End Sub
End Class