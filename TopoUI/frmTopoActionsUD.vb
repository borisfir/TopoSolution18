Option Explicit On
Option Strict On
Imports TopoManager
Imports BamashNet
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Imports AcadReport
Imports System.Data
Public Class frmTopoActionsUD

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
	Private WithEvents cmdSelectPgons As TabButton
	Private WithEvents cmdExec As TabButton
	Private WithEvents cmdSave As TabButton
	Private WithEvents cmdOK As TabButton


	Private WithEvents cmdBamashPaint As TabButton 'Input data old
	Private WithEvents cmdBamashAcadReport As TabButton	'Insert old

	Private WithEvents cmdInsertRep As TabButton
	Private WithEvents cmdExpExcel As TabButton
	Private WithEvents cmdPaintTable As TabButton

	Private lblParcelATitle As Label
	Private txtBlockANo As TextBox
	Private lblBlockANo As Label
	Private txtParcelAName As TextBox
	Private lblParcelAName As Label

	Private txtParcelALegalArea As TextBox
	Private lblParcelALegalArea As Label
	Private txtParcelAAcadArea As TextBox
	Private lblParcelAAcadArea As Label
	Private txtParcelAPlusTolerance As TextBox
	Private lblParcelAPlusTolerance As Label
	Private txtParcelAErrArea As TextBox
	Private lblParcelAErrArea As Label

	Private txtParcelAMaxAlter As TextBox
	Private lblParcelAMaxAlter As Label
	Private WithEvents txtAreaDif As TextBox
	Private lblAreaDif As Label
	Private WithEvents txtSubtrahendA As TextBox
	Private lblSubtrahendA As Label
	Private txtParcelAEstimateErrArea As TextBox
	Private lblParcelAEstimateErrArea As Label
	Private txtParcelBMaxAlter As TextBox
	Private lblParcelBMaxAlter As Label

	Private lblParcelBTitle As Label
	Private txtBlockBNo As TextBox
	Private lblBlockBNo As Label
	Private txtParcelBName As TextBox
	Private lblParcelBName As Label
	Private txtParcelBLegalArea As TextBox
	Private lblParcelBLegalArea As Label
	Private txtParcelBAcadArea As TextBox
	Private lblParcelBAcadArea As Label
	Private txtParcelBMinusTolerance As TextBox
	Private lblParcelBPlusTolerance As Label
	Private txtParcelBErrArea As TextBox
	Private lblParcelBErrArea As Label
	Private txtParcelBEstimateErrArea As TextBox
	Private lblParcelBEstimateErrArea As Label
	Private WithEvents txtSubtrahendB As TextBox
	Private lblSubtrahendB As Label
	Private txtMovingLinkCounter As TextBox
	Friend moShapeContainer As System.Object 'Microsoft.VisualBasic.PowerPacks.ShapeContainer
	Private LineShape1 As System.Object 'Microsoft.VisualBasic.PowerPacks.LineShape
	Private LineShape2 As System.Object ' Microsoft.VisualBasic.PowerPacks.LineShape

	'	Private WithEvents nudSteps As System.Windows.Forms.NumericUpDown


	Private WithEvents chkTopoLayersOn As CheckBox




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




	Private WithEvents cmbAreaFormat As System.Windows.Forms.ComboBox

	Private WithEvents rdbSubNumerationAll As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationEmptyFirst As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationEmptyMax As System.Windows.Forms.RadioButton
	Private WithEvents rdbSubNumerationNone As System.Windows.Forms.RadioButton
	Private txtDisplay As System.Windows.Forms.TextBox
	Private mtParcelArea As TopoManager.TPlanGraph.ParcelArea
	Private mtNeigborArea As TopoManager.TPlanGraph.ParcelArea

	Private mbCodeExecuting As Boolean = False
	Private moMovingLink As UnidivNet.MovingLink
	Private moaMovingLinks() As UnidivNet.MovingLink

	Private miTextBoxHeight As Integer = 18
	Private miTextBoxFirstY As Integer = 24
	Private miRowSpace As Integer = 24
	Private miTextBoxA_X As Integer = 232
	Private miTextBoxB_X As Integer = 440

	Public Sub New()
		Const sMaterBlockFolder As String = "\\Olympus\dm_app\dm_work\blocks\FrameTempl"
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		' Add any initialization after the InitializeComponent() call.

		diResourceTheme = TPlServerDB.enResourceTheme.AcFrmUnidiv

      System.Windows.Forms.MessageBox.Show("frmTopoActionsUD_New", "")

		zzMyInitializeComponent()
		MyBase.dsReportBlockFolder = sMaterBlockFolder
		MyBase.OnNew()
		'	MessageBox.Show(CStr(MyBase.doaLabels(0).Width), "29_001")
		'MyBase.doaLabels(0).Width = 120
		MyBase.doaLabels(0).WidthToLeft(120)
	End Sub

	Private Sub frmTopoActionsUD_AppExit() Handles Me.AppExit

		If moMovingLink IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage("###302 ")
			moMovingLink.Terminate()
		End If
	End Sub
	'




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
				System.Windows.Forms.MessageBox.Show(iCloseReason.ToString(), "frmTopoActionsM CloseReason 29_290")
		End Select
	End Sub

	Private Sub zzMyInitializeComponent()

		Me.cmdCalculate = New TabButton(moLabelFont, False)
		Me.cmdSelectPgons = New TabButton(moLabelFont, False)
		Me.cmdExec = New TabButton(moLabelFont, False)
		Me.cmdSave = New TabButton(moLabelFont, False)
		Me.cmdOK = New TabButton(moLabelFont, False)



		Me.cmdBamashPaint = New TabButton(moLabelFont, False)
		Me.cmdInsertRep = New TabButton(moLabelFont, False)
		Me.cmdExpExcel = New TabButton(moLabelFont, False)
		Me.cmdPaintTable = New TabButton(moLabelFont, False)

		Me.lblParcelATitle = New Label
		Me.txtBlockANo = New TextBox
		Me.lblBlockANo = New Label
		Me.txtParcelAName = New TextBox
		Me.lblParcelAName = New Label
		Me.txtParcelALegalArea = New TextBox
		Me.lblParcelALegalArea = New Label
		Me.txtParcelAAcadArea = New TextBox
		Me.lblParcelAAcadArea = New Label

		Me.txtParcelAPlusTolerance = New TextBox
		Me.lblParcelAPlusTolerance = New Label
		Me.txtParcelAErrArea = New TextBox
		Me.lblParcelAErrArea = New Label
		Me.txtParcelAMaxAlter = New TextBox
		Me.lblParcelAMaxAlter = New Label

		Me.txtAreaDif = New TextBox
		Me.lblAreaDif = New Label
		Me.txtSubtrahendA = New TextBox
		Me.lblSubtrahendA = New Label
		Me.txtParcelAEstimateErrArea = New TextBox
		Me.lblParcelAEstimateErrArea = New Label

		Me.lblParcelBTitle = New Label
		Me.txtBlockBNo = New TextBox
		Me.lblBlockBNo = New Label
		Me.txtParcelBName = New TextBox
		Me.lblParcelBName = New Label

		Me.txtParcelBLegalArea = New TextBox
		Me.lblParcelBLegalArea = New Label
		Me.txtParcelBAcadArea = New TextBox
		Me.lblParcelBAcadArea = New Label

		Me.txtParcelBMinusTolerance = New TextBox
		Me.lblParcelBPlusTolerance = New Label
		Me.txtParcelBErrArea = New TextBox
		Me.lblParcelBErrArea = New Label
		Me.txtParcelBEstimateErrArea = New TextBox
		Me.lblParcelBEstimateErrArea = New Label
		Me.txtParcelBMaxAlter = New TextBox
		Me.lblParcelBMaxAlter = New Label
		Me.txtSubtrahendB = New TextBox
		Me.lblSubtrahendB = New Label
		Me.txtParcelBEstimateErrArea = New TextBox
		Me.lblParcelBEstimateErrArea = New Label
		Me.txtMovingLinkCounter = New TextBox
        '	moShapeContainer = New Microsoft.VisualBasic.PowerPacks.ShapeContainer
        '	LineShape1 = New Microsoft.VisualBasic.PowerPacks.LineShape()
        '	LineShape2 = New Microsoft.VisualBasic.PowerPacks.LineShape()



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
	Private Function zzGetTextBoxLocation(ByVal iRow As Integer, ByVal iX As Integer) As System.Drawing.Point

		Dim iY As Integer = miTextBoxFirstY + iRow * miRowSpace
		Return New System.Drawing.Point(iX, iY)
	End Function
	Protected Overrides Sub zzInitTabApplication()
		'
		'cmdCalculate
		'
		With Me.cmdCalculate
			.Location = New System.Drawing.Point(8, 20)
			.Name = "cmdCalculate"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 12

			'    .Font = moLabelFont
			.Text = zzGetText(11, 2)
		End With
		Me.tbpApplication.Controls.Add(Me.cmdCalculate)
		'
		'cmdSelectPgons
		'
		With Me.cmdSelectPgons
			.Location = New System.Drawing.Point(8, 52)
			.Name = "cmdSelectPgons"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 13
			.Text = "בחר"
		End With
		Me.tbpApplication.Controls.Add(Me.cmdSelectPgons)
		'
		'cmdExec
		'
		With Me.cmdExec
			.Location = New System.Drawing.Point(8, 84)
			.Name = "cmdExec"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "בצע "
		End With
		Me.tbpApplication.Controls.Add(Me.cmdExec)

		'
		'cmdSave
		'
		With Me.cmdSave
			.Location = New System.Drawing.Point(8, 116)
			.Name = "cmdSave"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "שמור"
		End With
		Me.tbpApplication.Controls.Add(Me.cmdSave)

		'
		'cmdOK
		'
		With Me.cmdOK
			.Location = New System.Drawing.Point(8, 148)
			.Name = "cmdOK"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 14
			'    .Font = moLabelFont
			.Text = "OK "
		End With
		Me.tbpApplication.Controls.Add(Me.cmdOK)



		'
		'lblParcelATitle
		'
		With Me.lblParcelATitle
			.Size = New System.Drawing.Size(120, 24)
			.Location = New System.Drawing.Point(miTextBoxA_X - .Width \ 2, 0)
			.Name = "lblParcelATitle"
			.TabIndex = 19
			.Font = moLabelBoldFont
			.Text = "'חלקה א"
			.TextAlign = ContentAlignment.MiddleCenter
			'.BorderStyle = BorderStyle.FixedSingle
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelATitle)

		'
		'txtBlockANo
		'
		With Me.txtBlockANo
			.Size = New System.Drawing.Size(48, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(0, miTextBoxA_X - .Width) ' New System.Drawing.Point(140, 40)
			.Name = "txtBlockANo"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtBlockANo)

		'
		'lblBlockANo
		'
		With Me.lblBlockANo
			.Location = zzGetTextBoxLocation(0, miTextBoxA_X) ' New System.Drawing.Point(190, 40)
			.Name = "lblBlockANo"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "מס' גוש"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblBlockANo)

		'
		'txtParcelAName
		'
		With Me.txtParcelAName
			.Size = New System.Drawing.Size(48, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(1, miTextBoxA_X - .Width) ' New System.Drawing.Point(140, 68)
			.Name = "txtParcelAName"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAName)

		'
		'lblParcelAName
		'
		With Me.lblParcelAName
			.Location = zzGetTextBoxLocation(1, miTextBoxA_X) 'New System.Drawing.Point(190, 68)
			.Name = "lblParcelAName"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "מס' חלקה"
			.TextAlign = ContentAlignment.MiddleRight

		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAName)



		'
		'txtParcelALegalArea
		'
		With Me.txtParcelALegalArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(2, miTextBoxA_X - .Width) ' New System.Drawing.Point(100, 96)
			.Name = "txtParcelALegalArea"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelALegalArea)

		'
		'lblParcelALegalArea
		'
		With Me.lblParcelALegalArea
			.Location = zzGetTextBoxLocation(2, miTextBoxA_X) 'New System.Drawing.Point(190, 96)
			.Name = "lblParcelALegalArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח רשום"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelALegalArea)

		'
		'txtParcelAAcadArea
		'
		With Me.txtParcelAAcadArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(3, miTextBoxA_X - .Width) 'New System.Drawing.Point(100, 124)
			.Name = "txtParcelAAcadArea"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAAcadArea)

		'
		'lblParcelAAcadArea
		'
		With Me.lblParcelAAcadArea
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(3, miTextBoxA_X) ' New System.Drawing.Point(190, 124)
			.Name = "lblParcelAAcadArea"
			.AutoSize = False
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח מחושב"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAAcadArea)
		'
		'txtParcelAPlusTolerance
		'
		With Me.txtParcelAPlusTolerance
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(4, miTextBoxA_X - .Width) 'New System.Drawing.Point(100, 152)
			.Name = "txtParcelAPlusTolerance"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAPlusTolerance)

		'
		'lblParcelAPlusTolerance
		'
		With Me.lblParcelAPlusTolerance
			.Location = zzGetTextBoxLocation(4, miTextBoxA_X) 'New System.Drawing.Point(190, 152)
			.Name = "lblParcelAPlusTolerance"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "+Tolerance"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAPlusTolerance)

		'
		'txtParcelAErrArea
		'
		With Me.txtParcelAErrArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(5, miTextBoxA_X - .Width) 'New System.Drawing.Point(100, 180)
			.Name = "txtParcelAErrArea"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAErrArea)

		'
		'lblParcelAErrArea
		'
		With Me.lblParcelAErrArea
			.Location = zzGetTextBoxLocation(5, miTextBoxA_X) 'New System.Drawing.Point(190, 180)
			.Name = "lblParcelAErrArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "'הפחתה מינ"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAErrArea)

		'
		'txtParcelAMaxAlter
		'
		With Me.txtParcelAMaxAlter
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(6, miTextBoxA_X - .Width) 'New System.Drawing.Point(100, 208)
			.Name = "txtParcelAMaxAlter"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAMaxAlter)

		'
		'lblParcelAMaxAlter
		'
		With Me.lblParcelAMaxAlter
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(6, miTextBoxA_X) 'New System.Drawing.Point(190, 208)
			.Name = "lblParcelAMaxAlter"
			.AutoSize = False

			.TabIndex = 18
			.Font = moLabelFont
			.Text = "'הפחתה מקס"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAMaxAlter)
		'
		'txtAreaDif
		'
		With Me.txtAreaDif
			.Size = New System.Drawing.Size(60, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(7, 8)
			.Name = "txtAreaDif"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
		End With
		Me.tbpApplication.Controls.Add(Me.txtAreaDif)

		'
		'lblAreaDif
		'
		With Me.lblAreaDif
			.Location = zzGetTextBoxLocation(7, 68) 'New System.Drawing.Point(190, 208)
			.Name = "lblAreaDif"
			.AutoSize = False
			.Size = New System.Drawing.Size(48, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "העברה"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblAreaDif)


		'
		'txtSubtrahendA
		'
		With Me.txtSubtrahendA
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(7, miTextBoxA_X - .Width)
			.Name = "txtSubtrahendA"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
		End With
		Me.tbpApplication.Controls.Add(Me.txtSubtrahendA)

		'
		'lblSubtrahendA
		'
		With Me.lblSubtrahendA
			.Location = zzGetTextBoxLocation(7, miTextBoxA_X) 'New System.Drawing.Point(190, 208)
			.Name = "lblSubtrahendA"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח רצוי"	'"הקליד שטח"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblSubtrahendA)

		'
		'txtParcelAEstimateErrArea
		'
		With Me.txtParcelAEstimateErrArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(8, miTextBoxA_X - .Width) 'New System.Drawing.Point(100, 180)
			.Name = "txtParcelAEstimateErrArea"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelAEstimateErrArea)

		'
		'lblParcelAEstimateErrArea
		'
		With Me.lblParcelAEstimateErrArea
			.Location = zzGetTextBoxLocation(8, miTextBoxA_X) 'New System.Drawing.Point(190, 180)
			.Name = "lblParcelAEstimateErrArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "סטיה צפויה"
			.TextAlign = ContentAlignment.MiddleRight

		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelAEstimateErrArea)


		'	;;;;;;;;;;;'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'	;;;;;;;;;;;''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		'
		'lblParcelBTitle
		'
		With Me.lblParcelBTitle
			.Size = New System.Drawing.Size(120, 24)
			.Location = New System.Drawing.Point(miTextBoxB_X - .Width \ 2, 0)
			.Name = "lblParcelBTitle"
			.TabIndex = 18
			.Font = moLabelBoldFont
			.Text = "'חלקה ב"
			.TextAlign = ContentAlignment.MiddleCenter
			'.BorderStyle = BorderStyle.FixedSingle
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBTitle)
		'
		'txtBlockBNo
		'
		With Me.txtBlockBNo
			.Size = New System.Drawing.Size(48, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(0, miTextBoxB_X - .Width) ' New System.Drawing.Point(140, 40)
			.Name = "txtBlockBNo"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Left
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtBlockBNo)
		'
		'lblBlockBNo
		'
		With Me.lblBlockBNo
			.Location = zzGetTextBoxLocation(0, miTextBoxB_X) ' New System.Drawing.Point(190, 40)
			.Name = "lblBlockBNo"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "מס' גוש"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblBlockBNo)
		'
		'txtParcelBName
		'
		With Me.txtParcelBName
			.Size = New System.Drawing.Size(48, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(1, miTextBoxB_X - .Width)
			.Name = "txtParcelBName"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBName)

		'
		'lblParcelBName
		'
		With Me.lblParcelBName
			.Location = zzGetTextBoxLocation(1, miTextBoxB_X) 'New System.Drawing.Point(190, 68)
			.Name = "lblParcelBName"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "מס' חלקה"
			.TextAlign = ContentAlignment.MiddleRight

		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBName)

		'
		'txtParcelBLegalArea
		'
		With Me.txtParcelBLegalArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(2, miTextBoxB_X - .Width)
			.Name = "txtParcelBLegalArea"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBLegalArea)

		'
		'lblParcelBLegalArea
		'
		With Me.lblParcelBLegalArea
			.Location = zzGetTextBoxLocation(2, miTextBoxB_X) 'New System.Drawing.Point(190, 96)
			.Name = "lblParcelBLegalArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח רשום"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBLegalArea)

		'
		'txtParcelBAcadArea
		'
		With Me.txtParcelBAcadArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(3, miTextBoxB_X - .Width) 'New System.Drawing.Point(100, 124)
			.Name = "txtParcelBAcadArea"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBAcadArea)

		'
		'lblParcelBAcadArea
		'
		With Me.lblParcelBAcadArea
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(3, miTextBoxB_X) ' New System.Drawing.Point(190, 124)
			.Name = "lblParcelBAcadArea"
			.AutoSize = False
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח מחושב"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBAcadArea)
		'
		'txtParcelBMinusTolerance
		'
		With Me.txtParcelBMinusTolerance
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(4, miTextBoxB_X - .Width)
			.Name = "txtParcelBMinusTolerance"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBMinusTolerance)

		'
		'lblParcelBPlusTolerance
		'
		With Me.lblParcelBPlusTolerance
			.Location = zzGetTextBoxLocation(4, miTextBoxB_X) 'New System.Drawing.Point(190, 152)
			.Name = "lblParcelBPlusTolerance"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "-Tolerance"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBPlusTolerance)

		'
		'txtParcelBErrArea
		'
		With Me.txtParcelBErrArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(5, miTextBoxB_X - .Width) 'New System.Drawing.Point(100, 180)
			.Name = "txtParcelBErrArea"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBErrArea)

		'
		'lblParcelBErrArea
		'
		With Me.lblParcelBErrArea
			.Location = zzGetTextBoxLocation(5, miTextBoxB_X) 'New System.Drawing.Point(190, 180)
			.Name = "lblParcelBErrArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "'הוספה מינ"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBErrArea)



		'
		'txtParcelBMaxAlter
		'
		With Me.txtParcelBMaxAlter
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(6, miTextBoxB_X - .Width) 'New System.Drawing.Point(100, 208)
			.Name = "txtParcelBMaxAlter"
			.AutoSize = False
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBMaxAlter)

		'
		'lblParcelBMaxAlter
		'
		With Me.lblParcelBMaxAlter
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(6, miTextBoxB_X) 'New System.Drawing.Point(190, 208)
			.Name = "lblParcelBMaxAlter"
			.AutoSize = False

			.TabIndex = 18
			.Font = moLabelFont
			.Text = "'הוספה מקס"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBMaxAlter)



		'
		'txtSubtrahendB
		'
		With Me.txtSubtrahendB
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(7, miTextBoxB_X - .Width)
			.Name = "txtSubtrahendB"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
		End With
		Me.tbpApplication.Controls.Add(Me.txtSubtrahendB)

		'
		'lblSubtrahendB
		'
		With Me.lblSubtrahendB
			.Location = zzGetTextBoxLocation(7, miTextBoxB_X) 'New System.Drawing.Point(190, 208)
			.Name = "lblSubtrahendB"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "שטח רצוי"
			.TextAlign = ContentAlignment.MiddleRight
		End With
		Me.tbpApplication.Controls.Add(Me.lblSubtrahendB)

		'
		'txtParcelBEstimateErrArea
		'
		With Me.txtParcelBEstimateErrArea
			.Size = New System.Drawing.Size(88, miTextBoxHeight)
			.Location = zzGetTextBoxLocation(8, miTextBoxB_X - .Width)
			.Name = "txtParcelBEstimateErrArea"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtParcelBEstimateErrArea)

		'
		'lblParcelBEstimateErrArea
		'
		With Me.lblParcelBEstimateErrArea
			.Location = zzGetTextBoxLocation(8, miTextBoxB_X)
			.Name = "lblParcelBEstimateErrArea"
			.AutoSize = False
			.Size = New System.Drawing.Size(80, miTextBoxHeight)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "סטיה צפויה"
			.TextAlign = ContentAlignment.MiddleRight

		End With
		Me.tbpApplication.Controls.Add(Me.lblParcelBEstimateErrArea)
		'
		'txtMovingLinkCounter
		'
		With Me.txtMovingLinkCounter
			.Size = New System.Drawing.Size(24, miTextBoxHeight)
			.Location = New Point(88, Me.cmdSave.Top + 3)
			.Name = "txtMovingLinkCounter"
			.AutoSize = False

			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Right
			.ReadOnly = True
		End With
		Me.tbpApplication.Controls.Add(Me.txtMovingLinkCounter)

		Dim tPoint1 As Point = zzGetTextBoxLocation(2, miTextBoxA_X - 88)
		'
		'LineShape1
		'
        'With Me.LineShape1
        '	.BorderColor = System.Drawing.Color.Maroon
        '	.BorderWidth = 3
        '	.Name = "LineShape1"
        '	.X1 = tPoint1.X - 4
        '	.X2 = miTextBoxB_X + 80
        '	.Y1 = tPoint1.Y - .BorderWidth - 2
        '	.Y2 = tPoint1.Y - .BorderWidth - 2
        'End With

		'
		'LineShape2
		'
        'With Me.LineShape2
        '	.BorderColor = System.Drawing.Color.Maroon
        '	.BorderWidth = 3
        '	.Name = "LineShape2"
        '	.X1 = 4
        '	.X2 = miTextBoxB_X + 80
        '	.Y1 = 186
        '	.Y2 = 186
        'End With


		'
		'moShapeContainer
		'
        'With moShapeContainer
        '	.Location = New System.Drawing.Point(0, 0)
        '	.Margin = New System.Windows.Forms.Padding(0)
        '	.Name = "ShapeContainer"
        '	.Shapes.AddRange(New Microsoft.VisualBasic.PowerPacks.Shape() {Me.LineShape1, Me.LineShape2})
        '	.Size = New System.Drawing.Size(284, 278)
        '	.TabIndex = 0
        '	.TabStop = False
        'End With
        'Me.tbpApplication.Controls.Add(moShapeContainer)


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
		cmdCalculate.Cursor = Cursors.WaitCursor
		UnidivNet.Unidiv.Calculate()
		MyBase.OnCalculate()
		cmdCalculate.ResetCursor()
	End Sub
	Private Sub cmdSelectPgons_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdSelectPgons.Click
		'	UnidivNet.Unidiv.Start()
		zzClear()

		Me.Focus()
		'	zzTest()
		moMovingLink = New UnidivNet.MovingLink()

		Dim iUB As Integer = zzGetMovingLinkUB()

		moMovingLink.Number = iUB + 2

		'	zzTest()
		moMovingLink.SelectPolygons()

		Dim oParcel As UnidivNet.UD_Parcel = moMovingLink.Parcel
		Dim dAmin, dAmax, dBmin, dBMax As Double
		Dim dMaxOfMins, dMinOfMaxes As Double
		Dim dResDif As Double

		'Private mtParcelArea As UnidivNet.ParcelArea
		'Private mtNeigborArea As UnidivNet.ParcelArea
		If oParcel IsNot Nothing Then
			Me.txtBlockANo.Text = Convert.ToString(oParcel.BlockNo)
			Me.txtParcelAName.Text = Convert.ToString(oParcel.Name)
			mtParcelArea = oParcel.ParcelArea
			Me.txtParcelALegalArea.Text = UnidivNet.ShrinkPolygon.DispArea(mtParcelArea.LegalArea)
			Me.txtParcelAAcadArea.Text = UnidivNet.ShrinkPolygon.DispArea(mtParcelArea.CalcArea)
			Me.txtParcelAPlusTolerance.Text = UnidivNet.ShrinkPolygon.DispArea(mtParcelArea.LegalArea + mtParcelArea.Tolerance)
			dAmin = mtParcelArea.CalcArea - mtParcelArea.LegalArea - mtParcelArea.Tolerance
			dAmax = mtParcelArea.CalcArea - mtParcelArea.LegalArea + mtParcelArea.Tolerance

			Me.txtParcelAErrArea.Text = UnidivNet.ShrinkPolygon.DispArea(dAmin)
			Me.txtParcelAMaxAlter.Text = UnidivNet.ShrinkPolygon.DispArea(dAmax)

			'Me.txtSubtrahendA.Text = UnidivNet.ShrinkPolygon.DispArea(oParcel.LegalArea + oParcel.Tolerance)

			Dim oNeighborParcel As UnidivNet.UD_Parcel = moMovingLink.NeighborParcel
			If oNeighborParcel IsNot Nothing Then
				Me.txtBlockBNo.Text = Convert.ToString(oNeighborParcel.BlockNo)
				Me.txtParcelBName.Text = Convert.ToString(oNeighborParcel.Name)
				mtNeigborArea = oNeighborParcel.ParcelArea
				Me.txtParcelBLegalArea.Text = UnidivNet.ShrinkPolygon.DispArea(mtNeigborArea.LegalArea)
				Me.txtParcelBAcadArea.Text = UnidivNet.ShrinkPolygon.DispArea(mtNeigborArea.CalcArea)
				Me.txtParcelBMinusTolerance.Text = UnidivNet.ShrinkPolygon.DispArea(mtNeigborArea.LegalArea - mtNeigborArea.Tolerance)
				dBmin = mtNeigborArea.LegalArea - mtNeigborArea.Tolerance - mtNeigborArea.CalcArea
				dBMax = mtNeigborArea.LegalArea + mtNeigborArea.Tolerance - mtNeigborArea.CalcArea

				Me.txtParcelBErrArea.Text = UnidivNet.ShrinkPolygon.DispArea(dBmin)
				Me.txtParcelBMaxAlter.Text = UnidivNet.ShrinkPolygon.DispArea(dBMax)

				dMaxOfMins = Math.Max(dAmin, dBmin)
				dMinOfMaxes = Math.Min(dAmax, dBMax)
				dResDif = Math.Min(dMaxOfMins, dMinOfMaxes)
				zzCalcErr(dResDif)
				oNeighborParcel.Terminate()
			End If
			oParcel.Terminate()

		Else
			moMovingLink = Nothing
		End If
	End Sub
	Private Sub zzClear()
		Me.txtBlockANo.Text = String.Empty
		Me.txtParcelAName.Text = String.Empty

		Me.txtParcelALegalArea.Text = String.Empty
		Me.txtParcelAAcadArea.Text = String.Empty
		Me.txtParcelAPlusTolerance.Text = String.Empty

		Me.txtParcelAErrArea.Text = String.Empty
		Me.txtParcelAMaxAlter.Text = String.Empty

		Me.txtSubtrahendA.Text = String.Empty

		Me.txtBlockBNo.Text = String.Empty
		Me.txtParcelBName.Text = String.Empty

		Me.txtParcelBLegalArea.Text = String.Empty
		Me.txtParcelBAcadArea.Text = String.Empty
		Me.txtParcelBMinusTolerance.Text = String.Empty
		Me.txtParcelBErrArea.Text = String.Empty
		Me.txtParcelBMaxAlter.Text = String.Empty
		Me.txtSubtrahendB.Text = String.Empty
		Me.txtAreaDif.Text = String.Empty
		Me.txtParcelAEstimateErrArea.Text = String.Empty
		Me.txtParcelAEstimateErrArea.BackColor = Me.txtParcelAErrArea.BackColor
		Me.txtParcelBEstimateErrArea.Text = String.Empty
		Me.txtParcelBEstimateErrArea.BackColor = Me.txtParcelAErrArea.BackColor
	End Sub
	Private Sub zzCalcErr(ByVal dResDif As Double)
		Me.txtAreaDif.Text = UnidivNet.ShrinkPolygon.DispArea(dResDif)
		Me.txtSubtrahendA.Text = UnidivNet.ShrinkPolygon.DispArea(mtParcelArea.CalcArea - dResDif)
		Me.txtSubtrahendB.Text = UnidivNet.ShrinkPolygon.DispArea(mtNeigborArea.CalcArea + dResDif)

		Dim dEstimAreaA, dEstimAreaB As Double
		Dim dEstimErrA, dEstimErrB As Double
		dEstimAreaA = mtParcelArea.CalcArea - dResDif
		If dEstimAreaA > mtParcelArea.CalcArea + mtParcelArea.Tolerance Then
			dEstimErrA = dEstimAreaA - mtParcelArea.LegalArea - mtParcelArea.Tolerance
		ElseIf dEstimAreaA < mtParcelArea.LegalArea - mtParcelArea.Tolerance Then
			dEstimErrA = mtParcelArea.LegalArea - mtParcelArea.Tolerance - dEstimAreaA
		Else
			dEstimErrA = 0
		End If
		Me.txtParcelAEstimateErrArea.Text = UnidivNet.ShrinkPolygon.DispArea(dEstimErrA)
		If dEstimErrA > 0.0 Then
			Me.txtParcelAEstimateErrArea.BackColor = Color.Red
		Else
			Me.txtParcelAEstimateErrArea.BackColor = Me.txtParcelAErrArea.BackColor
		End If

		dEstimAreaB = mtNeigborArea.CalcArea + dResDif
		If dEstimAreaB > mtNeigborArea.LegalArea + mtNeigborArea.Tolerance Then
			dEstimErrB = dEstimAreaB - mtNeigborArea.LegalArea - mtNeigborArea.Tolerance
		ElseIf dEstimAreaB < mtNeigborArea.LegalArea - mtNeigborArea.Tolerance Then
			dEstimErrB = mtNeigborArea.LegalArea - mtNeigborArea.Tolerance - dEstimAreaB
		Else
			dEstimErrB = 0
		End If
		Me.txtParcelBEstimateErrArea.Text = UnidivNet.ShrinkPolygon.DispArea(dEstimErrB)
		If dEstimErrB > 0.0 Then
			Me.txtParcelBEstimateErrArea.BackColor = Color.Red
		Else
			Me.txtParcelBEstimateErrArea.BackColor = Me.txtParcelBErrArea.BackColor
		End If
		Me.cmdExec.Enabled = (dResDif > 0.0)
	End Sub
	Private Sub cmdExec_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExec.Click
		If IsNumeric(Me.txtSubtrahendA.Text) AndAlso moMovingLink IsNot Nothing Then
			Dim dArea As Double = Convert.ToDouble(Me.txtSubtrahendA.Text)
			moMovingLink.ShrinkTo(dArea)
		End If
	End Sub
	Private Function zzGetMovingLinkUB() As Integer
		If moaMovingLinks Is Nothing Then
			Return -1
		Else
			Return moaMovingLinks.GetUpperBound(0)
		End If
	End Function

	Private Sub cmdOK_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
		Dim iUB As Integer = zzGetMovingLinkUB()
		'	zzTest()
		For iIndex As Integer = 0 To iUB
			If moaMovingLinks(iIndex) Is Nothing Then
				'MessageBox.Show("Nothing", CStr(iIndex))
			Else
				'	MessageBox.Show("Num=" & moaMovingLinks(iIndex).Number, CStr(iIndex))
				moaMovingLinks(iIndex).OK()
			End If

		Next

		If moMovingLink IsNot Nothing AndAlso moMovingLink.Status = UnidivNet.MovingLink.enStatus.Executed Then
			moMovingLink.OK()
		End If
		Erase moaMovingLinks
		Me.txtMovingLinkCounter.Text = "0"
	End Sub
	Private Sub zzTest()
		Dim iUB As Integer = zzGetMovingLinkUB()
		Dim sMsg As String = String.Empty
		For iIndex As Integer = 0 To iUB
			If moaMovingLinks(iIndex) Is Nothing Then
				sMsg &= "Nothing-" & CStr(iIndex) & vbCrLf

			Else
				sMsg &= "Num=" & moaMovingLinks(iIndex).Number & " -" & CStr(iIndex) & vbCrLf
			End If

		Next
		MessageBox.Show(sMsg, "345 " & CStr(iUB))
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
				'''''''''''''''''''''	oPgon.PaintZebra(taColorScheme(iIndex).Zebra)
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
			End Select

			If oDataView IsNot Nothing Then
				MyBase.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues)
			ElseIf dicAttribValues IsNot Nothing Then
				MyBase.InsertBlockReport(dicAttribValues, sBlockName)
			End If
		End If
		Me.Cursor = Cursors.Default
	End Sub

   Private Sub txtZebraWidthDrawing_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZebraWidthDrawing.Leave
      If Me.txtZebraWidthDrawing.Text.Length <> 0 Then
         Try
            bmBamash.ZebraWidthDrawing = Convert.ToDouble(Me.txtZebraWidthDrawing.Text)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsUD - txtZebraWidthDrawing_Leave")
         End Try
      End If
   End Sub
   Private Sub txtZebraWidthTable_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZebraWidthTable.Leave
      If Me.txtZebraWidthDrawing.Text.Length <> 0 Then
         Try
            bmBamash.ZebraWidthTable = Convert.ToDouble(Me.txtZebraWidthTable.Text)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsUD - txtZebraWidthTable_Leave")
         End Try
      End If
   End Sub
	Private Sub txtBufferOffset_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtBufferOffset.Leave
		If Me.txtBufferOffset.Text.Length <> 0 Then
			Try
				bmBamash.BufferOffset = Convert.ToDouble(Me.txtBufferOffset.Text)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsUd - txtBufferOffset_Leave")
			End Try
		End If
	End Sub
	Private Sub txtZoomRadius_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZoomRadius.Leave
		If Me.txtBufferOffset.Text.Length <> 0 Then
			Try
				DMAcadExt.AppMessages.ZoomRadius = Convert.ToDouble(Me.txtZoomRadius.Text)
				'		System.Windows.Forms.MessageBox.Show(CStr(Convert.ToDouble(Me.txtZoomRadius.Text)) & ":" & CStr(bmBamash.ZoomRadius), "12_410n")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsUD - txtZoomRadius_Leave")
			End Try
		End If
	End Sub




	Private Sub txtAreaDif_LostFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtAreaDif.LostFocus
		Try
			Dim dResDif As Double = Convert.ToDouble(Me.txtAreaDif.Text)
			zzCalcErr(dResDif)
		Catch oEx As Exception

		End Try

	End Sub

	Private Sub txtSubtrahendA_LostFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtSubtrahendA.LostFocus
		Try
			Dim dAreaA As Double = Convert.ToDouble(Me.txtSubtrahendA.Text)
			Dim dResDif As Double = mtParcelArea.CalcArea - dAreaA
			If dResDif < 0.0 Then
				dResDif = 0.0
			End If
			zzCalcErr(dResDif)
		Catch oEx As Exception

		End Try
	End Sub

	Private Sub txtSubtrahendB_LostFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtSubtrahendB.LostFocus
		Try
			Dim dAreaB As Double = Convert.ToDouble(Me.txtSubtrahendB.Text)
			Dim dResDif As Double = dAreaB - mtParcelArea.CalcArea
			If dResDif < 0.0 Then
				dResDif = 0.0
			End If
			zzCalcErr(dResDif)
		Catch oEx As Exception

		End Try
	End Sub

	Private Sub cmdSave_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
		'	zzTest()
		If moMovingLink.Status = UnidivNet.MovingLink.enStatus.Executed Then
			Dim iUB As Integer = zzGetMovingLinkUB()

			iUB += 1
			ReDim Preserve moaMovingLinks(iUB)
			moaMovingLinks(iUB) = moMovingLink
			moMovingLink = Nothing
			If iUB >= 0 Then
				Me.txtMovingLinkCounter.Text = CStr(iUB + 1)
			Else
				Me.txtMovingLinkCounter.Text = "0"
			End If
		Else

		End If
		'	zzTest()

	End Sub
End Class

