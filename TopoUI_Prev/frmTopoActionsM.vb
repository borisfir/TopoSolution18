Option Explicit On
Option Strict On
Imports TopoManager
Imports BamashNet
Imports AcadReport
Imports System.Data
Public Class frmTopoActionsM

	'Private moReportApp As AcadReport.Application
	Private miActionDflt As Integer = 0
	'Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTopoMaster


	'Private moaChecks(miToposUB) As TopoCheck

	'''''  Private miaTopoIDs(miToposUB) As Integer
	'''''  Private moaTopoDefs(miToposUB) As TopoDef
	'''''''Private miCurrentTopoDefID As DMAcadExt.TopoDefID


	'''''''Private moLabelColor As System.Drawing.Color = Color.DimGray
	''''''''Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue
	''''''''Private moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	'	'''''Private moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	''''''''Private moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.75!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	'Private moSelectedReportItem As ReportItem

	''''''''''''Private moaErrorPoints() As DMAcadExt.TplnPointArray = Nothing


	''''''''''''''''Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
	''''''''''''''''Private moCurrentView As DataView
	Private miaReports() As TPlServerDB.enResourceTheme = { _
 TPlServerDB.enResourceTheme.AcRepBamashSum _
 , TPlServerDB.enResourceTheme.AcRepBamashMain _
 , TPlServerDB.enResourceTheme.AcRepBamashShared _
 , TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle _
 , TPlServerDB.enResourceTheme.AcRepBamashAfricaData}


	Private miaReportOption() As TopoManager.TPlanGraph.enDataOptions = {TopoManager.TPlanGraph.enDataOptions.Default _
	 , TopoManager.TPlanGraph.enDataOptions.Default _
	 , TopoManager.TPlanGraph.enDataOptions.AcadArea _
	 , TopoManager.TPlanGraph.enDataOptions.AcadArea _
	 , TopoManager.TPlanGraph.enDataOptions.AcadArea _
	 , TopoManager.TPlanGraph.enDataOptions.AcadArea _
	 , TopoManager.TPlanGraph.enDataOptions.AcadArea _
	 , TopoManager.TPlanGraph.enDataOptions.Default _
	 , TopoManager.TPlanGraph.enDataOptions.Default _
	 , TopoManager.TPlanGraph.enDataOptions.Default _
	 , TopoManager.TPlanGraph.enDataOptions.Default}

	'''''''''Private moDataTable(miToposUB) As System.Data.DataTable
	'''''''''Private miStepNum(miToposUB) As Integer


	'new System.Data.DataTable(msTableName);
	'	Private WithEvents dgvMain As DataGridView

	Private WithEvents cmdCalculate As TabButton
	Private WithEvents cmdUpdateCentroids As TabButton
	Private WithEvents cmdClearPaint As TabButton
	Private WithEvents cmdBamashPaint As TabButton 'Input data old
	Private WithEvents cmdBamashAcadReport As TabButton	'Insert old

	Private WithEvents cmdInsertRep As TabButton
	Private WithEvents cmdExpExcel As TabButton
	Private WithEvents cmdPaintTable As TabButton

	'	Private WithEvents nudSteps As System.Windows.Forms.NumericUpDown




	Private WithEvents chkBamash As CheckBox


	'	Private WithEvents chkApproved As CheckBox
	'	Private WithEvents chkProposed As CheckBox
	'	Private WithEvents chkParcel As CheckBox
	'	Private WithEvents chkMerge As CheckBox
	'	Private WithEvents chkUnion As CheckBox



	'''''''Private txtTolerance As TextBox
	'''''''Private lblTolerance As Label
	'''''''''''	Private lblErrors As Label
	Private lblBlockFormat As Label
	Private lblTopoFormat As Label
	Private lblLanduseFormataaaa As Label
	Private lblCoordinateFormat As Label
	Private lblAreaFormat As Label




	Private WithEvents lstReports As ListBox
	Private grbCalcOption As System.Windows.Forms.GroupBox
	Private WithEvents rdbCalc As System.Windows.Forms.RadioButton
	Private WithEvents rdbAcad As System.Windows.Forms.RadioButton
	Private grbFormats As System.Windows.Forms.GroupBox
	Private grbNumberFormat As System.Windows.Forms.GroupBox

	Private WithEvents txtZebraWidthDrawing As TextBox
	Private WithEvents txtZebraWidthTable As TextBox
	Private WithEvents txtBufferOffset As TextBox
	Private WithEvents txtZoomRadius As TextBox



	Private lblZebraWidthDrawing As Label
	Private lblZebraWidthTable As Label
	Private lblBufferOffset As Label
	Private lblZoomRadius As Label

	Private lblRepBamashSharedScale As Label



	Private WithEvents cmbAreaFormat As System.Windows.Forms.ComboBox

	Private WithEvents rdbSubNumerationAll As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationEmptyFirst As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationEmptyMax As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationNone As System.Windows.Forms.RadioButton
	'	Private chkFullColor As CheckBox


	Private txtDisplay As System.Windows.Forms.TextBox

	Private mbCodeExecuting As Boolean = False




	Public Sub New()
		Const sMaterBlockFolder As String = "\\Olympus\dm_app\dm_work\blocks\FrameTempl"
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		' Add any initialization after the InitializeComponent() call.

		diResourceTheme = TPlServerDB.enResourceTheme.AcFrmTopoMaster
		bmBamash.InitParams()


		zzMyInitializeComponent()
		MyBase.dsReportBlockFolder = sMaterBlockFolder
		MyBase.OnNew()
	End Sub
	'


	Private Sub frmTopoActions_Disposed(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Disposed
		Dim sTest As String = "a"


		Try
			bmBamash.CaptionOptionSetting = bmBamash.CaptionOption
			'	Dim baCalculateSetting() As Boolean = {Me.chkBamash.Checked}
			sTest = "b"
			'	zzCalculateSetting = baCalculateSetting



			sTest = "K"
			If Me.txtZebraWidthDrawing IsNot Nothing Then

				Try
					bmBamash.ZebraWidthDrawingSetting = bmBamash.ZebraWidthDrawing
				Catch oEx As Exception
				End Try


				Try
					bmBamash.ZebraWidthTableSetting = bmBamash.ZebraWidthTable
				Catch oEx As Exception
				End Try


				Try
					If bmBamash.BufferOffset > 0.0 Then
						bmBamash.BufferOffsetSetting = bmBamash.BufferOffset
					End If
				Catch oEx As Exception
				End Try


				Try
					bmBamash.ZoomRadiusSetting = DMAcadExt.AppMessages.ZoomRadius
				Catch oEx As Exception
				End Try

			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "frmTopoActionsM - frmTopoActions_Disposed")
		End Try

	End Sub

	Private Sub frmTopoActions_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs)
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
				System.Windows.Forms.MessageBox.Show(iCloseReason.ToString(), "frmTopoActionsM CloseReason 29_290")
		End Select
	End Sub

	Private Sub zzMyInitializeComponent()

		Me.cmdCalculate = New TabButton(moLabelFont, False)
		Me.cmdUpdateCentroids = New TabButton(moLabelFont, False)
		Me.cmdClearPaint = New TabButton(moLabelFont, False)
		Me.cmdBamashPaint = New TabButton(moLabelFont, False)
		Me.cmdInsertRep = New TabButton(moLabelFont, False)
		Me.cmdExpExcel = New TabButton(moLabelFont, False)
		Me.cmdPaintTable = New TabButton(moLabelFont, False)
		'	Me.cmdSaveProjectData = New TabButton(moLabelFont)

		Me.chkBamash = New CheckBox
		'	Me.chkApproved = New CheckBox
		'	Me.chkProposed = New CheckBox
		'	Me.chkParcel = New CheckBox
		'	Me.chkMerge = New CheckBox
		'	Me.chkUnion = New CheckBox
		'  Me.lstReports = New ListBox()
	 


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




	
	Protected Overrides Sub OpenProjectData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)
		If doProjectData Is Nothing Then
			doProjectData = New BamashProjectData()
			TopoManager.TPlanGraph.TplnProject.ProjectData = doProjectData
		End If

	End Sub

	Protected Overrides Sub SetProjectData()
		bmBamash.ProjectData = DirectCast(doProjectData, BamashProjectData)
	End Sub

	Protected Overrides Sub zzInitTabParameters()

		Me.txtZebraWidthDrawing = New System.Windows.Forms.TextBox
		Me.txtZebraWidthTable = New System.Windows.Forms.TextBox
		Me.txtBufferOffset = New System.Windows.Forms.TextBox
		Me.txtZoomRadius = New System.Windows.Forms.TextBox




		Me.lblZebraWidthDrawing = New System.Windows.Forms.Label
		Me.lblZebraWidthTable = New System.Windows.Forms.Label
		Me.lblBufferOffset = New System.Windows.Forms.Label
		Me.lblZoomRadius = New System.Windows.Forms.Label


		'
		'txtZebraWidthDrawing
		'
		With Me.txtZebraWidthDrawing
			.Location = New System.Drawing.Point(12, 12)
			.Name = "txtZebraWidthDrawing"
			.Size = New System.Drawing.Size(60, 20)
			.TabIndex = 5
			Try
				.Text = Convert.ToString(bmBamash.ZebraWidthDrawingSetting)
			Catch oEx As Exception

			End Try

		End With

		'
		'txtZebraWidthTable
		'
		With Me.txtZebraWidthTable
			.Location = New System.Drawing.Point(12, 44)
			.Name = "txtZebraWidthTable"
			.Size = New System.Drawing.Size(60, 20)
			.TabIndex = 6
			Try
				.Text = Convert.ToString(bmBamash.ZebraWidthTableSetting)
			Catch oEx As Exception

			End Try
		End With

		'
		'txtBufferOffset
		'
		With Me.txtBufferOffset
			.Location = New System.Drawing.Point(12, 76)
			.Name = "txtBufferOffset"
			.Size = New System.Drawing.Size(60, 20)
			.TabIndex = 7
			Try
				.Text = Convert.ToString(bmBamash.BufferOffsetSetting)
			Catch oEx As Exception

			End Try
		End With

		'
		'txtZoomRadius
		'
		With Me.txtZoomRadius
			.Location = New System.Drawing.Point(12, 108)
			.Name = "txtZoomRadius"
			.Size = New System.Drawing.Size(60, 20)
			.TabIndex = 17
			Try
				.Text = Convert.ToString(bmBamash.ZoomRadiusSetting)
			Catch oEx As Exception

			End Try
		End With
	


		'
		'lblZebraWidthDrawing
		'
		With Me.lblZebraWidthDrawing
			.Location = New System.Drawing.Point(80, 12)
			.Name = "lblZebraWidthDrawing"
			.Size = New System.Drawing.Size(144, 20)
			.TabIndex = 6
			.Text = zzGetText(1, 8)
		End With

		'
		'lblZebraWidthTable
		'
		With Me.lblZebraWidthTable
			.Location = New System.Drawing.Point(80, 44)
			.Name = "lblZebraWidthTable"
			.Size = New System.Drawing.Size(144, 20)
			.TabIndex = 6
			.Text = zzGetText(2, 8)
		End With

		'
		'lblBufferOffset
		'
		With Me.lblBufferOffset
			.Location = New System.Drawing.Point(80, 76)
			.Name = "lblBufferOffset"
			.Size = New System.Drawing.Size(144, 20)
			.TabIndex = 6
			.Text = zzGetText(0, 8)
		End With

		'
		'lblZoomRadius
		'
		With Me.lblZoomRadius
			.Location = New System.Drawing.Point(80, 108)
			.Name = "lblZoomRadius"
			.Size = New System.Drawing.Size(144, 20)
			.TabIndex = 16
			.Text = zzGetText(3, 8)
		End With


		With Me.tbpParameters.Controls
			.Add(Me.txtZebraWidthDrawing)
			.Add(Me.txtZebraWidthTable)
			.Add(Me.txtBufferOffset)
			.Add(Me.txtZoomRadius)

			.Add(Me.lblZebraWidthDrawing)
			.Add(Me.lblZebraWidthTable)
			.Add(Me.lblBufferOffset)
			.Add(Me.lblZoomRadius)
		End With
		MyBase.OnInitTabParameters()



		Me.grbFormats = New System.Windows.Forms.GroupBox
		Me.grbNumberFormat = New System.Windows.Forms.GroupBox

		Me.cmbAreaFormat = New System.Windows.Forms.ComboBox

		Me.lblBlockFormat = New System.Windows.Forms.Label
		Me.lblTopoFormat = New System.Windows.Forms.Label
		'''''''''''	Me.lblLanduseFormat = New System.Windows.Forms.Label
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
			.SelectedText = TopoManager.TPlanGraph.TplnProject.AreaFormat
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


		'		Me.grbFormats.Controls.Add(Me.lblBlockFormat)
		'		Me.grbFormats.Controls.Add(Me.lblTopoFormat)
		'		Me.grbFormats.Controls.Add(Me.lblLanduseFormat)
		'
		'		Me.grbNumberFormat.Controls.Add(Me.cmbAreaFormat)
		'		Me.grbNumberFormat.Controls.Add(Me.lblCoordinateFormat)
		'		Me.grbNumberFormat.Controls.Add(Me.lblAreaFormat)
		'
		'
		'		Me.tbpParameters.Controls.Add(Me.grbFormats)
		'		Me.tbpParameters.Controls.Add(Me.grbNumberFormat)


	End Sub
	Protected Overrides Sub zzInitTabApplication()

		Me.lstReports = New ListBox()
		Me.grbCalcOption = New System.Windows.Forms.GroupBox
		Me.rdbCalc = New System.Windows.Forms.RadioButton
		Me.rdbAcad = New System.Windows.Forms.RadioButton
		Me.rdbSubNumerationAll = New System.Windows.Forms.RadioButton
		Me.rdbSubNumerationEmptyFirst = New System.Windows.Forms.RadioButton
		Me.rdbSubNumerationEmptyMax = New System.Windows.Forms.RadioButton
		Me.rdbSubNumerationNone = New System.Windows.Forms.RadioButton
		Me.txtRepBamashSharedScale = New System.Windows.Forms.TextBox

		Me.grbCalcOption.SuspendLayout()
		'
		'lstReports
		'
		With Me.lstReports
			.Location = New System.Drawing.Point(328, 8)	 '8
			.Name = "lstReports"
			.Enabled = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(180, 135)
			.TabIndex = 13
			.Text = "Erase"
			.Font = moLabelFont
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember

            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashSum, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashSum), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, False, True, False))
            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashMain, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashMain), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, True, True))
            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashShared, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashShared), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, True, True))
            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashStamp, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashStamp), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, False, True, False))
            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, False, True))
            .Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashAfricaData, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashAfricaData), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, False, True))
		End With

		'
		'grbCalcOption
		'
		With Me.grbCalcOption
			.Controls.Add(Me.rdbSubNumerationAll)
			.Controls.Add(Me.rdbSubNumerationEmptyFirst)
			.Controls.Add(Me.rdbSubNumerationEmptyMax)
			.Controls.Add(Me.rdbSubNumerationNone)

			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(8, 8)
			.Name = "grbCalcOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(170, 104)
			.TabIndex = 1
			.TabStop = False
		End With
		'
		'rdbAcad
		'
		With rdbAcad
			.AutoSize = True
			.Location = New System.Drawing.Point(4, 8)
			.Name = "rdbAcad"
			.Size = New System.Drawing.Size(96, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(6, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		End With


		'
		'rdbCalc
		'
		With Me.rdbCalc
			.AutoSize = True
			.Location = New System.Drawing.Point(4, 28)
			.Name = "rdbCalc"
			.Size = New System.Drawing.Size(96, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(5, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = True
		End With


		'
		'rdbSubNumerationAll
		'
		With rdbSubNumerationAll
			.AutoSize = False
			.Location = New System.Drawing.Point(4, 12)
			.Name = "rdbSubNumerationAll"
			.Size = New System.Drawing.Size(156, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(0, 7)
			.UseVisualStyleBackColor = True
			.Checked = False
			.CheckAlign = ContentAlignment.MiddleLeft
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		End With

		'
		'rdbSubNumerationEmptyFirst
		'
		With rdbSubNumerationEmptyFirst
			.AutoSize = False
			.Location = New System.Drawing.Point(4, 34)
			.Name = "rdbSubNumerationEmptyFirst"
			.Size = New System.Drawing.Size(156, 17)
			.TabIndex = 11
			.TabStop = True
			.Text = zzGetText(1, 7)
			.UseVisualStyleBackColor = True
			.Checked = True
			.CheckAlign = ContentAlignment.MiddleLeft
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		End With

		'
		'rdbSubNumerationEmptyMax
		'
		With rdbSubNumerationEmptyMax
			.AutoSize = False
			.Location = New System.Drawing.Point(4, 56)
			.Name = "rdbSubNumerationEmptyMax"
			.Size = New System.Drawing.Size(156, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(2, 7)
			.UseVisualStyleBackColor = True
			.Checked = False
			.CheckAlign = ContentAlignment.MiddleLeft
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		End With
		'
		'rdbSubNumerationNone
		'
		With rdbSubNumerationNone
			.AutoSize = False
			.Location = New System.Drawing.Point(4, 78)
			.Name = "rdbSubNumerationNone"
			.Size = New System.Drawing.Size(156, 17)
			.TabIndex = 22
			.TabStop = True
			.Text = zzGetText(3, 7)
			.UseVisualStyleBackColor = True
			.Checked = False
			.CheckAlign = ContentAlignment.MiddleLeft
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		End With
 

		'
		'cmdUpdateCentroids
		'
		With Me.cmdUpdateCentroids
			.Location = New System.Drawing.Point(64, 160)
			.Name = "cmdUpdateCentroids"
			.Size = New System.Drawing.Size(92, 24)
			.TabIndex = 13
			.Text = "עדכון נתונים"
		End With

		'
		'cmdClearPaint
		'
		With Me.cmdClearPaint
			.Location = New System.Drawing.Point(12, 200)
			.Name = "cmdClearPaint"
			.Size = New System.Drawing.Size(92, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "מחיקת צביעה"
		End With
		'
		'cmdBamashPaint
		'
		With Me.cmdBamashPaint
			.Location = New System.Drawing.Point(120, 200)
			.Name = "cmdBamashPaint"
			.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 15
			'    .Font = moLabelFont
			.Text = "צביעת במ""ש"
		End With
		'
		'cmdCalculate
		'
		With Me.cmdCalculate
			.Location = New System.Drawing.Point(64, 120)
			.Name = "cmdCalculate"
			.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12

			'    .Font = moLabelFont
			.Text = zzGetText(11, 2)
		End With



		'
		'cmdInsertRep
		'
		With Me.cmdInsertRep
			.Location = New System.Drawing.Point(444, 155)
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
			.Location = New System.Drawing.Point(360, 155)
			.Name = "cmdExpExcel"
			.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 19
			'    .Font = moLabelFont
			.Text = "Excel"
		End With

		'
		'cmdPaintTable
		'
		With Me.cmdPaintTable
			.Location = New System.Drawing.Point(356, 190)
			.Name = "cmdPaintTable"
			.Enabled = True
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 19
			.Visible = False
			'    .Font = moLabelFont
			.Text = "Paint "
		End With
		Me.txtRepBamashSharedScale = New System.Windows.Forms.TextBox

		'
		'txtRepBamashSharedScale
		'
		With Me.txtRepBamashSharedScale
			.Location = New System.Drawing.Point(220, 8)
			.Name = "txtRepBamashSharedScale"
			.Size = New System.Drawing.Size(32, 20)
			.TabIndex = 5
			Try
				.Text = Convert.ToString(zzGetDoubleSetting("RepBamashSharedScale", 0.4))
			Catch oEx As Exception

			End Try

		End With

		lblRepBamashSharedScale = New Label

		'
		'lblRepBamashSharedScale
		'
		With Me.lblRepBamashSharedScale
			.Location = New System.Drawing.Point(252, 8)
			.Name = "lblRepBamashSharedScale"
			.Size = New System.Drawing.Size(72, 20)
			.TabIndex = 6
			.TextAlign = ContentAlignment.MiddleRight
			.Text = "קנ""מ זברה"

		End With


		With Me.tbpApplication.Controls
			.Add(Me.lstReports)
			.Add(Me.cmdCalculate)
			.Add(Me.cmdUpdateCentroids)
			.Add(Me.cmdClearPaint)
			.Add(Me.cmdBamashPaint)
			.Add(Me.cmdInsertRep)
			.Add(Me.cmdExpExcel)
			.Add(Me.cmdPaintTable)
			.Add(Me.txtRepBamashSharedScale)
			.Add(Me.lblRepBamashSharedScale)

		End With

	



		'	Me.tbpReports.Controls.Add(Me.rdbSubNumerationAll)
		'	Me.tbpReports.Controls.Add(Me.rdbSubNumerationEmptyFirst)
		'	Me.tbpReports.Controls.Add(Me.rdbSubNumerationEmptyMax)
		'	Me.tbpReports.Controls.Add(Me.rdbSubNumerationNone)


		'	Me.tbpReports.Controls.Add(Me.chkBamash)
		'	Me.tbpReports.Controls.Add(Me.chkApproved)
		'	Me.tbpReports.Controls.Add(Me.chkProposed)
		'	Me.tbpReports.Controls.Add(Me.chkParcel)
		'	Me.tbpReports.Controls.Add(Me.chkMerge)
		'	Me.tbpReports.Controls.Add(Me.chkUnion)
		Me.tbpApplication.Controls.Add(Me.grbCalcOption)

		Me.grbCalcOption.ResumeLayout(False)
		Me.grbCalcOption.PerformLayout()
		Dim baCalculateSetting() As Boolean = zzCalculateSetting()
		Me.chkBamash.Checked = baCalculateSetting(0)
		'	Me.chkApproved.Checked = baCalculateSetting(1)
		'	Me.chkProposed.Checked = baCalculateSetting(2)
		'	Me.chkMerge.Checked = baCalculateSetting(3)
		'	Me.chkUnion.Checked = baCalculateSetting(4)

	End Sub


	Private Shared Function zzGetReportName(ByVal iResourceTheme As TPlServerDB.enResourceTheme) As String
		Return TPlServerDB.TextResource.GetText(2, iResourceTheme, 0)
	End Function
	
	
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

	Private Sub lstReports_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles lstReports.SelectedIndexChanged
		If Me.lstReports.SelectedItem Is Nothing Then
			doSelectedReportItem = Nothing
		Else
			doSelectedReportItem = DirectCast(Me.lstReports.SelectedItem, ReportItem)
			Me.cmdExpExcel.Enabled = doSelectedReportItem.Excel
			Me.cmdInsertRep.Enabled = doSelectedReportItem.Acad
		End If
	End Sub

	 

	
	Private Sub zzTestInsert()

		'	Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
		Dim saPrompt(1) As String

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
			Common.GetEditor().WriteMessage("TopoActions: " & CStr(iHWin) & ":" & sTitle & ":" & CStr(iFocus) & ":" & CStr(iInputState) & ":" & CStr(iWinDC))


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "39_210")
		End Try

		Common.SetAcadFocus()
		Dim bResp As Boolean = TopoManager.TPlanGraph.TplnProject.GetPoint("start point", tPoint)
		'   Me.Show()

	End Sub

	Private Sub cmbAreaFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbAreaFormat.SelectedIndexChanged
		Try
			TopoManager.TPlanGraph.TplnProject.AreaFormat = Me.cmbAreaFormat.SelectedItem.ToString()
			MyBase.OnFormatChanged()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbAreaFormat_SelectedIndexChanged")
		End Try
	End Sub



	Private Sub cmdCalculate_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click
		Dim iCalcOption As bmBamash.SubNumerationOptions

		If Me.rdbSubNumerationAll.Checked Then
			iCalcOption = bmBamash.SubNumerationOptions.All
		ElseIf Me.rdbSubNumerationEmptyFirst.Checked Then
			iCalcOption = bmBamash.SubNumerationOptions.EmptyFirst
		ElseIf Me.rdbSubNumerationEmptyMax.Checked Then
			iCalcOption = bmBamash.SubNumerationOptions.EmptyMax
		ElseIf Me.rdbSubNumerationNone.Checked Then
			iCalcOption = bmBamash.SubNumerationOptions.None
		Else
			iCalcOption = bmBamash.SubNumerationOptions.EmptyFirst
		End If
		'	bmProperty.OutputFullColor = Me.chkFullColor.Checked
		BamashNet.bmBamash.Calculate(iCalcOption)
		Me.lstReports.Enabled = True
		Me.cmdExpExcel.Enabled = True
		Me.cmdInsertRep.Enabled = True
		MyBase.OnCalculate()
	End Sub
	Private Sub cmdUpdateCentroids_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdateCentroids.Click
		BamashNet.bmBamash.UpdateCentroids()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub cmdClearPaint_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClearPaint.Click
		BamashNet.bmBamash.ClearPaint()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub cmdBamashPaint_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdBamashPaint.Click
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.SaveVarCmdDia(0)
		BamashNet.bmBamash.PaintAllProperties()
		DMAcadExt.AcadDocument.RestoreVarCmdDia()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdInsertRep_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdInsertRep.Click
		dbAutocad = True
		zzInsertReport()
	End Sub
	Private Sub cmdPaintTable_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdPaintTable.Click
		zzPaintTable()
	End Sub
	Private Sub zzPaintTable()
		Dim oRepApp As AcadReport.BaseReport = New ExcelReport.Report(False)
		'	Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection

		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing

		Try
			oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, True)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			DMAcadExt.AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "frmTopoActions - zzInsertReport_02")
		End Try
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)


		Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection = doReportApp.GetColorCells()
		Dim taColorScheme() As DMAcadExt.ColorScheme
		If colaPoints IsNot Nothing Then
			taColorScheme = doReportApp.GetColorScheme()

			Dim oPgon As SimplePgon
			Dim dBefore As Double
			For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
				oPgon = New SimplePgon(colaPoints(iIndex))
				oPgon.SetTagNum("Table", iIndex)
				dBefore = taColorScheme(iIndex).Scale
				taColorScheme(iIndex).Scale = bmBamash.ZebraWidthTable / bmBamash.ZebraWidthDrawing
				MessageBox.Show(CStr(dBefore) & ":" & CStr(bmBamash.ZebraWidthTable / bmBamash.ZebraWidthDrawing))
				oPgon.PaintZebra(taColorScheme(iIndex).Zebra)
			Next

		End If


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		If oDocLock IsNot Nothing Then
			oDocLock.Dispose()
			oDocLock = Nothing
		End If

	End Sub
	Private Sub cmdExpExcel_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExpExcel.Click

		dbAutocad = False
		zzInsertReport()

	End Sub

	Private Sub zzInsertReport()
		'	Const sMaterBlockFolder As String = "\\Zeus\dm_app\dm_work\blocks\FrameTempl"
		'	Dim iDataOptions As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
		'ByVal oRepApp As AcadReport.BaseReport
		Dim oaOptionValues(0) As System.Object

		Me.Cursor = Cursors.WaitCursor
		If doSelectedReportItem IsNot Nothing Then
			Dim oDataView As DataView = Nothing
			Dim dicAttribValues As Dictionary(Of String, String) = Nothing
			Dim iaDataColumns() As Integer = Nothing
			Dim oaTotals() As System.Object = Nothing

			'		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
			Dim sBlockName As String = String.Empty
			Dim bCurrentLayerOK As Boolean = True
			Select Case doSelectedReportItem.RepIndex
				Case TPlServerDB.enResourceTheme.AcRepBamashSum
					oDataView = Nothing
					dicAttribValues = bmBamash.GetSumData()
					sBlockName = "DRWS027"
				Case TPlServerDB.enResourceTheme.AcRepBamashMain
					oDataView = bmProperty.MainRepView
					ExcelReport.Report.RightToLeft = True
					iaDataColumns = bmProperty.GetDataColumns(Not dbAutocad)
 
					'	ExcelReport.Report.Reverse = True
				Case TPlServerDB.enResourceTheme.AcRepBamashShared
					oDataView = bmBamash.SharedRepView
				Case TPlServerDB.enResourceTheme.AcRepBamashStamp
					oDataView = Nothing
					dicAttribValues = bmBamash.GetStampData()
					sBlockName = "DRWS026"
				Case TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle
					oDataView = bmBamash.GetAfricaTitleView()
					ExcelReport.Report.RightToLeft = True
				Case TPlServerDB.enResourceTheme.AcRepBamashAfricaData
					oDataView = bmBamash.GetAfricaDataView()
					ExcelReport.Report.RightToLeft = True
					ExcelReport.Report.Reverse = False

			End Select
			Dim iaEmptyColumns() As Integer = BamashPolygon.GetEmptyColumns()
			If oDataView IsNot Nothing Then
				MyBase.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues, iaEmptyColumns)
			ElseIf dicAttribValues IsNot Nothing Then
				MyBase.InsertBlockReport(dicAttribValues, sBlockName)
			End If
		End If
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub txtZebraWidthDrawing_Leave(ByVal oSender As Object, ByVal e As System.EventArgs) Handles txtZebraWidthDrawing.Leave
		If Me.txtZebraWidthDrawing.Text.Length <> 0 Then
			Try
				bmBamash.ZebraWidthDrawing = Convert.ToDouble(Me.txtZebraWidthDrawing.Text)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsM - txtZebraWidthDrawing_Leave")
			End Try
		End If
	End Sub
	Private Sub txtZebraWidthTable_Leave(ByVal oSender As Object, ByVal e As System.EventArgs) Handles txtZebraWidthTable.Leave
		If Me.txtZebraWidthDrawing.Text.Length <> 0 Then
			Try
				bmBamash.ZebraWidthTable = Convert.ToDouble(Me.txtZebraWidthTable.Text)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsM - txtZebraWidthTable_Leave")
			End Try
		End If
	End Sub
	Private Sub txtBufferOffset_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtBufferOffset.Leave
		If Me.txtBufferOffset.Text.Length <> 0 Then
			Try
				bmBamash.BufferOffset = Convert.ToDouble(Me.txtBufferOffset.Text)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsM - txtBufferOffset_Leave")
			End Try
		End If
	End Sub
	Private Sub txtZoomRadius_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZoomRadius.Leave
		If Me.txtBufferOffset.Text.Length <> 0 Then
			Try
				DMAcadExt.AppMessages.ZoomRadius = Convert.ToDouble(Me.txtZoomRadius.Text)
				'		System.Windows.Forms.MessageBox.Show(CStr(Convert.ToDouble(Me.txtZoomRadius.Text)) & ":" & CStr(bmBamash.ZoomRadius), "12_410n")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsM - txtZoomRadius_Leave")
			End Try
		End If
	End Sub




	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class

