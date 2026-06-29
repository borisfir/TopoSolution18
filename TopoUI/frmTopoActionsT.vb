Option Explicit On
Option Strict On
Imports TopoManager
Imports AcadReport
Imports System.Data
Public Class frmTopoActionsT
	Private miaReports() As TPlServerDB.enResourceTheme = {
	TPlServerDB.enResourceTheme.AcRepContent _
	, TPlServerDB.enResourceTheme.AcRepPlanParcels _
	, TPlServerDB.enResourceTheme.AcRepPlanParcels _
	, TPlServerDB.enResourceTheme.AcRepLotsK _
	, TPlServerDB.enResourceTheme.AcRepLotsK _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseK _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseK _
	, TPlServerDB.enResourceTheme.AcRepSumLuse _
	 , TPlServerDB.enResourceTheme.AcRepSumLuse _
	, TPlServerDB.enResourceTheme.AcRepJointLuse _
	 , TPlServerDB.enResourceTheme.AcRepLegendK _
	 , TPlServerDB.enResourceTheme.AcRepLegendK _
	 , TPlServerDB.enResourceTheme.AcRepLegalParcels _
	 , TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock _
	 , TPlServerDB.enResourceTheme.AcRepOwnership}


	'	  , TPlServerDB.enResourceTheme.AcRepBlockArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContent _
	'	 , TPlServerDB.enResourceTheme.AcRepLotContentArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel _


	Private miaReportOptions() As enReportOptions = { _
	enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	  , enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.OverlayEnabled _
	, enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled}

	Private miaReportTopoPurpose() As DMAcadExt.enTopoPurpose = {
	 DMAcadExt.enTopoPurpose.Undefined _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Proposed _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Proposed _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Proposed _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Proposed _
	 , DMAcadExt.enTopoPurpose.Undefined _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Proposed _
	 , DMAcadExt.enTopoPurpose.Undefined _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Approved _
	 , DMAcadExt.enTopoPurpose.Approved}



	'new System.Data.DataTable(msTableName);



	Private WithEvents cmdCalculate As TabButton
	Private WithEvents cmdColorEditor As TabButton
	Private WithEvents cmdSelectLanduse As TabButton


	Private WithEvents cmdInsertRep As TabButton
	Private WithEvents cmdExpExcel As TabButton

	Private WithEvents cmdUpdateTopo As TabButton

	Private WithEvents cmdPaintByLanduse As TabButton
	Private WithEvents cmdErasePaint As TabButton
	Private WithEvents cmdClearPaint As TabButton



	








	Private WithEvents chkApproved As CheckBox
	Private WithEvents chkProposed As CheckBox
	Private WithEvents chkParcel As CheckBox
	Private WithEvents chkParcelClPgon As CheckBox

	Private WithEvents chkMerge As CheckBox
	Private WithEvents chkUnion As CheckBox
	Private WithEvents chkFDO_Overlay As CheckBox


	Private WithEvents chkLotNameNum As CheckBox
	Private WithEvents chkBlueLine As CheckBox
	Private WithEvents chkPaintStraighten As CheckBox

	Private WithEvents chkRemoveSlivers As CheckBox

	Private lblBlockFormat As Label
	Private lblTopoFormat As Label
	Private lblLanduseFormat As Label
	Private lblCoordinateFormat As Label
	Private lblAreaFormat As Label




	Private WithEvents lstReports As ListBox
	Private grbCalcOption As System.Windows.Forms.GroupBox
	Private grbOverlayOption As System.Windows.Forms.GroupBox

	Private grbPaintDest As System.Windows.Forms.GroupBox
	Private WithEvents rdbAcad As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc2 As System.Windows.Forms.RadioButton
	Private WithEvents rdbRounded As System.Windows.Forms.RadioButton

	Private WithEvents rdbMerge As System.Windows.Forms.RadioButton
	Private WithEvents rdbUnion As System.Windows.Forms.RadioButton
	Private WithEvents rdbFDO As System.Windows.Forms.RadioButton


	Private WithEvents rdbPaintAppr As System.Windows.Forms.RadioButton
	Private WithEvents rdbPaintProp As System.Windows.Forms.RadioButton


	Private grbFormats As System.Windows.Forms.GroupBox
	Private grbNumberFormat As System.Windows.Forms.GroupBox
	Private grbSliverTolerance As System.Windows.Forms.GroupBox

	Private lblStraightTolerance As Label
	Private lblLegendPaintFactor As Label

	Private lblMinFDOSliverTolerance As Label
	Private lblMaxFDOSliverTolerance As Label
	Private miDefaultTopoPurpose As DMAcadExt.enTopoPurpose
	Private WithEvents cmbBlockFormat As ComboBox
	Private WithEvents cmbTopoFormat As ComboBox
	Private WithEvents cmbLanduseFormat As ComboBox
	Private WithEvents cmbCoordFormat As ComboBox
	Private WithEvents cmbAreaFormat As ComboBox
	Private WithEvents cmbPaintScale As ComboBox

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()
		' Add any initialization after the InitializeComponent() call.
		diResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan
		zzMyInitializeComponent()
		MyBase.OnNew()
		pbEventsEnabled = True
	End Sub

	Private Sub frmTopoActions_Disposed(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Disposed
		Dim sTest As String = "a"
		Try
			Dim baCalculateSetting() As Boolean = {Me.chkParcel.Checked _
	 , Me.chkApproved.Checked _
	 , Me.chkProposed.Checked _
	 , Me.chkMerge.Checked _
	 , Me.chkUnion.Checked _
	 , Me.chkFDO_Overlay.Checked}
			sTest = "b"
			zzCalculateSetting = baCalculateSetting
			sTest = "ba"
			zzLotNameSetting = Me.chkLotNameNum.Checked()
			sTest = "bb"
			zzCheckTopoSetting = Me.chkCheckTopo.Checked()
			sTest = "bc"
			zzBlueLineSetting = Me.chkBlueLine.Checked()
			sTest = "bd"
			zzPaintStraightenSetting = Me.chkPaintStraighten.Checked()
 



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
			Parameters.Save()

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "frmTopoActionsT - Disposed")
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



	Private Sub zzMyInitializeComponent()

		Me.cmdCalculate = New TabButton(moLabelFont, False)
		Me.cmdColorEditor = New TabButton(moLabelFont, False)
		Me.cmdSelectLanduse = New TabButton(moLabelFont, False)

		Me.cmdInsertRep = New TabButton(moLabelFont, False)
		Me.cmdExpExcel = New TabButton(moLabelFont, False)
		Me.cmdUpdateTopo = New TabButton(moLabelFont, False)


		Me.chkApproved = New CheckBox
		Me.chkProposed = New CheckBox
		Me.chkParcel = New CheckBox
		Me.chkParcelClPgon = New CheckBox
		Me.chkMerge = New CheckBox
		Me.chkUnion = New CheckBox
		Me.chkFDO_Overlay = New CheckBox



		'	CType(Me.dgvActions, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()

		'
		'cmdCalculate
		'
		With Me.cmdCalculate
			.Location = New System.Drawing.Point(8, 148)
			.Name = "cmdCalculate"
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 12
			.Text = zzGetText(14, 2)
			'    .Font = moLabelFont
			'	.Text = "Calculate!"
		End With
		'
		'cmdColorset
		'
		With Me.cmdColorEditor
			.Location = New System.Drawing.Point(88, 148)
			.Name = "cmdColorEditor"
			.Size = New System.Drawing.Size(100, 24)
			.TabIndex = 12
			.Text = zzGetText(15, 2)
			'    .Font = moLabelFont

		End With
		'
		'cmdSelectL
		'
		With Me.cmdSelectLanduse
			.Location = New System.Drawing.Point(4, 10)
			.Name = "cmdSelectL"
			.Size = New System.Drawing.Size(72, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = zzGetText(16, 2)
		End With

		'
		'cmdInsertRep
		'
		With Me.cmdInsertRep
			.Location = New System.Drawing.Point(444 - 24, 4)
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
			.Location = New System.Drawing.Point(288 + 24, 4)
			.Name = "cmdExpExcel"
			.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 19
			'    .Font = moLabelFont
			.Text = "Excel"
		End With
		'
		'cmdUpdateTopo
		'
		With Me.cmdUpdateTopo
			.Location = New System.Drawing.Point(220, 160)
			.Name = "cmdUpdateTopo"
			.Enabled = True
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 22
			'    .Font = moLabelFont
			.Text = "Update"
			.Visible = False
		End With


		'
		'chkParcel
		'
		With Me.chkParcel
			.Location = New System.Drawing.Point(144, 8)
			.Name = "chkParcel"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 22
			.Font = moLabelFont
			.Text = zzGetText(0, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With		  '
		'
		'chkParcelClPgon
		'
		With Me.chkParcelClPgon
			.Location = New System.Drawing.Point(136, 32)
			.Name = "chkParcelClPgon"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 22
			.Font = moLabelFont
			.Text = zzGetText(13, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With

		'
		'chkApproved
		'
		With Me.chkApproved
			.Location = New System.Drawing.Point(144, 60)
			.Name = "chkApproved"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 16
			.Font = moLabelFont
			.Text = zzGetText(1, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With



		'
		'chkProposed
		'
		With Me.chkProposed
			.Location = New System.Drawing.Point(144, 88)
			.Name = "chkProposed"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 16
			.Font = moLabelFont
			.Text = zzGetText(2, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'chkMerge
		'
		With Me.chkMerge
			.Location = New System.Drawing.Point(4, 8)
			.Name = "chkMerge"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 22
			.Font = moLabelFont
			.Text = zzGetText(3, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With

		'
		'chkUnion
		'
		With Me.chkUnion
			.Location = New System.Drawing.Point(4, 36)
			.Name = "chkUnion"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 23
			.Font = moLabelFont
			.Text = zzGetText(4, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
			.Enabled = False
		End With

		'
		'chkFDO_Overlay
		'
		With Me.chkFDO_Overlay
			.Location = New System.Drawing.Point(4, 64)
			.Name = "chkFDO_Overlay"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 23
			.Font = moLabelFont
			.Text = zzGetText(5, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With


		Me.chkLotNameNum = New System.Windows.Forms.CheckBox
		Me.chkLotNameNum.Checked = zzLotNameSetting
		Me.chkCheckTopo = New System.Windows.Forms.CheckBox
		Me.chkCheckTopo.Checked = zzCheckTopoSetting
		Me.chkBlueLine = New System.Windows.Forms.CheckBox
		Me.chkBlueLine.Checked = zzBlueLineSetting
		Me.chkPaintStraighten = New System.Windows.Forms.CheckBox
		Me.chkPaintStraighten.Checked = zzPaintStraightenSetting

		Me.chkRemoveSlivers = New System.Windows.Forms.CheckBox



		'	Me.tbpTopo.Controls.Add(Me.cmdRegen)
		'	Me.tbpTopo.Controls.Add(Me.cmdClose)
		'	Me.tbpTopo.Controls.Add(Me.cmdExit)

		Me.Location = New Point(200, 200)
		' zzTopoLayersIsOn()


	End Sub

	Private Sub zzDispArray(ByVal iaVal() As Integer)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & iaVal(iIndex).ToString()
		Next
		System.Windows.Forms.MessageBox.Show(sOut, "Errors")
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


	Private Sub cmdInsertRep_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdInsertRep.Click
		Dim oRepApp As Report = New Report
		MyBase.dbAutocad = True
		zzInsertReport(oRepApp)
	End Sub
	Private Sub cmdExpExcel_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExpExcel.Click
		MyBase.dbAutocad = False
		zzInsertReport(New ExcelReport.Report(False))
	End Sub
	Private Sub cmdUpdateTopo_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdateTopo.Click
		zzUpdateTopo(DMAcadExt.enTopoPurpose.Approved)
	End Sub
	Private Sub zzUpdateTopo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

	End Sub
	Private Sub zzInsertReport(ByVal oReport As BaseReport)
		Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod
		Dim iOptionIndex As Integer = -1
		Dim bTopoPurpose As Boolean = False
		Dim bAllArea As Boolean = False

		Dim oaOptionValues(1) As System.Object
		Dim sTopoPurposeText As String = Nothing
		Select Case doSelectedReportItem.TopoPurpose
			Case DMAcadExt.enTopoPurpose.Approved
				iOptionIndex = 0
				bTopoPurpose = True

				sTopoPurposeText = zzGetText(0, 12)
			Case DMAcadExt.enTopoPurpose.Proposed
				iOptionIndex = 0
				bTopoPurpose = True

				sTopoPurposeText = zzGetText(1, 12)
			Case Else
				iOptionIndex = -1

		End Select
		If bTopoPurpose Then
			oaOptionValues(0) = sTopoPurposeText

		End If

		If (doSelectedReportItem.Options And enReportOptions.AllAreaOptionsEnabled) <> 0 Then
			iOptionIndex += 1
			bAllArea = True
		End If




		Me.Cursor = Cursors.WaitCursor
		If Me.lstReports.SelectedItem IsNot Nothing Then
			'MessageBox.Show(doSelectedReportItem.ListDispData & vbCrLf & doSelectedReportItem.RepIndex.ToString(), "01_118")
			If Me.grbCalcOption.Enabled Then
				If Me.rdbAcad.Checked Then
					iDataOption = TPlanGraph.enDataOptions.AcadArea

					oaOptionValues(iOptionIndex) = Me.rdbAcad.Text
				ElseIf Me.rdbCalc.Checked Then
					iDataOption = TPlanGraph.enDataOptions.CalcMergeArea

					oaOptionValues(iOptionIndex) = Me.rdbCalc.Text
				ElseIf Me.rdbCalc2.Checked Then
					iDataOption = TPlanGraph.enDataOptions.CalcMergeArea2

					oaOptionValues(iOptionIndex) = Me.rdbCalc2.Text
				ElseIf Me.rdbRounded.Checked Then
					iDataOption = TPlanGraph.enDataOptions.RoundedArea

					oaOptionValues(iOptionIndex) = Me.rdbRounded.Text
				Else
					iDataOption = TPlanGraph.enDataOptions.Default
				End If
			End If

			If Me.grbOverlayOption.Enabled Then
				If Me.rdbMerge.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Merge
				ElseIf Me.rdbUnion.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Union
				ElseIf Me.rdbFDO.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
				End If
			Else
			End If
		End If
		Dim oDataView As DataView = Nothing
		Dim iaDataColumns() As Integer = Nothing


		Dim oaTotals() As System.Object = Nothing
		If oReport.AcadModel Then
			RepApp.InitDWGScaleFactor()
		End If
		'	MessageBox.Show(doSelectedReportItem.RepIndex.ToString() & ":" & doSelectedReportItem.TopoPurpose.ToString() & ":" & iDataOption.ToString(), "01_666")
		Select Case doSelectedReportItem.RepIndex
			Case TPlServerDB.enResourceTheme.AcRepContent
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, miDefaultTopoPurpose)
				oDataView = TPlanGraph.TplnParcel.BlockView(iOverlayIndex)
			Case TPlServerDB.enResourceTheme.AcRepPlanParcels
            TPlanGraph.TplnParcel.GetInPlanData(doSelectedReportItem.TopoPurpose, True, iDataOption, iOverlayMethod, False, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepLotsK
				TPlanGraph.TplnLot.GetMainData(doSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, 0, Nothing, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepLotsM
				TPlanGraph.TplnLot.GetMainData(DMAcadExt.enTopoPurpose.Proposed, iDataOption, iOverlayMethod, 0, Nothing, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepParcelLuseK
            TPlanGraph.TplnParcel.GetLanduseData(doSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, 0, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepParcelLuseM
            TPlanGraph.TplnParcel.GetLanduseData(DMAcadExt.enTopoPurpose.Proposed, iDataOption, iOverlayMethod, 0, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepSumLuse
				TPlanGraph.TplnLot.GetLanduseData(doSelectedReportItem.TopoPurpose, iOverlayMethod, iDataOption, oDataView, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepJointLuse
				TPlanGraph.TplnLot.GetJointLanduseData(iOverlayMethod, iDataOption, oDataView, oaTotals)

			Case TPlServerDB.enResourceTheme.AcRepLotContent
				oDataView = TPlanGraph.TplnParcel.LotContentView
			Case TPlServerDB.enResourceTheme.AcRepLotContentArea
				TPlanGraph.TplnProject.LotContentAreaView(DMAcadExt.enOverlayIndex.ApprMerge, True, oDataView, iaDataColumns)
			Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel
				TPlanGraph.TplnProject.LotContentAreaView(DMAcadExt.enOverlayIndex.ApprMerge, False, oDataView, iaDataColumns)
			Case TPlServerDB.enResourceTheme.AcRepLegendK
				DMAcadExt.AcadDocument.WriteMessage("125 UColorDB: " & CStr(RepApp.DrawingScaleFactor) & ":" & CStr(zzGetSelectedScale()))

				DMAcadExt.AcadDocument.WriteMessage("^612 " & CStr(Parameters.LegendPaintFactor) & "; " & CStr(RepApp.DrawingScaleFactor))
				TPlanGraph.TplnLot.GetLanduseList(doSelectedReportItem.TopoPurpose, oDataView, (Parameters.LegendPaintFactor * RepApp.DrawingScaleFactor))
				'Case TPlServerDB.enResourceTheme.AcRepLegendM
				'TPlanGraph.TplnLot.GetLanduseList(DMAcadExt.enTopoPurpose.Proposed, oDataView, Parameters.LegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor * zzGetSelectedScale())
			Case TPlServerDB.enResourceTheme.AcRepLegalParcels
				'	System.Windows.Forms.MessageBox.Show(doSelectedReportItem.TopoPurpose.ToString(), "01_386j")
            TPlanGraph.TplnParcel.GetInPlanData(DMAcadExt.enTopoPurpose.Approved, False, iDataOption, iOverlayMethod, False, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, doSelectedReportItem.TopoPurpose)
				'	System.Windows.Forms.MessageBox.Show(doSelectedReportItem.TopoPurpose.ToString() & ":" & iOverlayIndex.ToString(), "01_386j")
				TPlanGraph.TplnProject.LotContentByBlockAreaView(iOverlayIndex, iDataOption, oDataView, iaDataColumns, oaTotals)
			Case TPlServerDB.enResourceTheme.AcRepOwnership

		End Select
		If oDataView IsNot Nothing Then

			'''''''''''''''''	oRepApp.Open(doSelectedReportItem.RepIndex) obsolete

			oReport.MainView = oDataView
			'	System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count), "01_385y")
			If iaDataColumns IsNot Nothing Then
				oReport.DataColumns = iaDataColumns
			End If
			If oaTotals IsNot Nothing Then
				oReport.Totals = oaTotals
			End If
			If oaOptionValues(0) IsNot Nothing Then
				oReport.OptionValues = oaOptionValues
			End If

			If oReport.AcadModel Then
				Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
				Dim bCurrentLayerOK As Boolean = True
				Try
					oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, True)
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					DMAcadExt.AcadErrCode.ShowAcadError(oAcadEx, False, "frmTopoActions - zzInsertReport_02")
				End Try
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, True)

				If bCurrentLayerOK And False Then
					Me.Hide()
					Common.SetAcadFocus()
					If oReport.Insert() Then
						RepApp.AcadTable.RecomputeTableBlock(True)
						''''''''''		DMAcadExt.AcadDocument.Regen()
						RepApp.AcadTable.Draw()
						RepApp.PaintCells()
					End If
					Me.Show()
				End If
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				If oDocLock IsNot Nothing Then
					oDocLock.Dispose()
					oDocLock = Nothing
				End If
			Else
				''''''''''''''	oRepApp.Insert()
			End If
		Else
         MessageBox.Show("Data was not found", "01_431d")

		End If

		If oDataView IsNot Nothing Then
			MyBase.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues)
		End If

		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzGetSelectedScale() As Double
		Dim sScale As String
		Dim dBaseScale As Double = 1000.0
		Dim dScale As Double = 0.0

		If Me.cmbPaintScale.SelectedIndex >= 0 Then
			sScale = Me.cmbPaintScale.SelectedItem.ToString()
			dScale = DMCommon.Functions.TextToScale(sScale, dBaseScale) / dBaseScale
		Else
			MessageBox.Show(Me.cmbPaintScale.SelectedIndex.ToString(), "16_711")
		End If
		Return dScale
	End Function
	Protected Overrides Sub OpenProjectData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)

		If doProjectData Is Nothing Then
			doProjectData = New TplanProjectData()
			TopoManager.TPlanGraph.TplnProject.ProjectData = doProjectData
		End If
		If Not doProjectData.Opened OrElse (Not bReadOnly AndAlso doProjectData.Mode = Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) Then

			'''''''''''	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)

			'''''''''''''''DMAcadExt.AcadTransaction.Start()
			doProjectData.OpenData(bCreateValues, bReadOnly)
			'''''''''''''''''''''	DMAcadExt.AcadTransaction.Terminate()
			''''''''''''	DMAcadExt.AcadDocument.Unlock()

		End If
	End Sub
	Protected Overrides Sub SetProjectData()

	End Sub
	Protected Overrides Sub zzInitTabParameters()
		pbEventsEnabled = False
		Dim iaFormatsSetting() As Integer = TPlanGraph.TplnProject.FormatsSetting()
		Me.grbFormats = New System.Windows.Forms.GroupBox
		Me.grbNumberFormat = New System.Windows.Forms.GroupBox
		Me.grbSliverTolerance = New System.Windows.Forms.GroupBox

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

		Me.lblStraightTolerance = New System.Windows.Forms.Label
		Me.lblLegendPaintFactor = New System.Windows.Forms.Label

		Me.lblMinFDOSliverTolerance = New System.Windows.Forms.Label
		Me.lblMaxFDOSliverTolerance = New System.Windows.Forms.Label


		Me.txtLegendPaintFactor = New System.Windows.Forms.TextBox
		Me.txtStraightenTolerance = New System.Windows.Forms.TextBox
		Me.txtMaxFDOSliverTolerance = New System.Windows.Forms.TextBox
		Me.txtMinFDOSliverTolerance = New System.Windows.Forms.TextBox


		Me.txtMinFDOSliverTolerance = New System.Windows.Forms.TextBox



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
		'grbSliverTolerance
		'
		With Me.grbSliverTolerance
			.BackColor = System.Drawing.Color.Transparent
			.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			.Location = New System.Drawing.Point(160, 150)
			.Name = "grbSliverTolerance"
			.Text = zzGetText(20, 11)
			.Size = New System.Drawing.Size(160, 100)
			.TabStop = False
		End With



		'
		'cmbBlockFormat
		'
		With Me.cmbBlockFormat
			.FormattingEnabled = True
			.Location = New System.Drawing.Point(6, 18)
			.Name = "cmbBlockFormat"
			.Size = New System.Drawing.Size(115, 21)
			.TabIndex = 22
			FillFormatRow(Me.cmbBlockFormat, 3)
			Try
				.SelectedIndex = iaFormatsSetting(TPlanGraph.enFormatType.BlockLayer) - 1
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
		End With



		'
		'cmbTopoFormat
		'
		With Me.cmbTopoFormat
			.FormattingEnabled = True
			.Location = New System.Drawing.Point(6, 50)
			.Name = "cmbTopoFormat"
			.Size = New System.Drawing.Size(115, 21)
			.TabIndex = 23
			FillFormatRow(Me.cmbTopoFormat, 4)
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
			FillFormatRow(Me.cmbLanduseFormat, 5)
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
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With
		'
		'chkCheckTopo
		'
		With Me.chkCheckTopo
			.Location = New System.Drawing.Point(12, 192)
			.Name = "chkCheckTopo"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 43
			.Font = moLabelFont
			.Text = zzGetText(17, 11)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With
		'
		'chkBlueLine
		'
		With Me.chkBlueLine
			.Location = New System.Drawing.Point(12, 224)
			.Name = "chkBlueLine"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 43
			.Font = moLabelFont
			.Text = zzGetText(18, 11)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With
		'
		'txtLegendPaintFactor
		'
		With Me.txtLegendPaintFactor
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(160, 120)
			.Name = "txtLegendPaintFactor"
			.Size = New System.Drawing.Size(40, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = Convert.ToString(Parameters.LegendPaintFactor)
			.TextAlign = HorizontalAlignment.Left
		End With
		Dim dStraightenTolerance As Double
		Try
			dStraightenTolerance = zzGetDoubleSetting("StraightenTolerance", 0.2)

		Catch oEx As Exception

		End Try

		'
		'txtTolerance
		'
		With Me.txtStraightenTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(4, 120)
			.Name = "txtTolerance"
			.Size = New System.Drawing.Size(48, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = Convert.ToString(dStraightenTolerance)
			.TextAlign = HorizontalAlignment.Left
		End With

		'
		'txtMinFDOSliverTolerance
		'
		With Me.txtMinFDOSliverTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(6, 18)
			.Name = "txtMinFDOSliverTolerance"
			.Size = New System.Drawing.Size(40, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.TextAlign = HorizontalAlignment.Left
		End With

		'
		'txtMaxFDOSliverTolerance
		'
		With Me.txtMaxFDOSliverTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(6, 48)
			.Name = "txtMaxFDOSliverTolerance"
			.Size = New System.Drawing.Size(40, 16)
			.TabIndex = 17
			.Font = moLabelFont

			.TextAlign = HorizontalAlignment.Left
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
		'
		'lblStraightTolerance
		'
		With Me.lblStraightTolerance
			.Location = New System.Drawing.Point(56, 120)
			.Name = "lblStraightTolerance"
			.Size = New System.Drawing.Size(80, 21)
			.TabIndex = 31
			.Font = moLabelFont
			.Text = zzGetText(13, 11)
		End With
		'
		'lblLegendPaintFactor
		'
		With Me.lblLegendPaintFactor
			.Location = New System.Drawing.Point(200, 120)
			.Name = "lblLegendPaintFactor"
			.Size = New System.Drawing.Size(120, 21)
			.TabIndex = 32
			.Font = moLabelFont
			.Text = zzGetText(14, 11)
		End With

		'
		'lblMinFDOSliverTolerance
		'
		With Me.lblMinFDOSliverTolerance
			.Location = New System.Drawing.Point(50, 20)
			.Name = "lblMinFDOSliverTolerance"
			.Size = New System.Drawing.Size(120, 21)
			.TabIndex = 32
			.Font = moLabelFont
			.Text = zzGetText(21, 11)
		End With

		'
		'lblMaxFDOSliverTolerance
		'
		With Me.lblMaxFDOSliverTolerance
			.Location = New System.Drawing.Point(50, 50)
			.Name = "lblMaxFDOSliverTolerance"
			.Size = New System.Drawing.Size(120, 21)
			.TabIndex = 32
			.Font = moLabelFont
			.Text = zzGetText(22, 11)
		End With

		'
		'chkRemoveSlivers
		'
		With Me.chkRemoveSlivers
			.Location = New System.Drawing.Point(6, 78)
			.Name = "chkRemoveSlivers"
			.Size = New System.Drawing.Size(100, 24)
			.TabIndex = 43
			.Font = moLabelFont
			.Text = zzGetText(23, 11)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
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

		Me.grbSliverTolerance.Controls.Add(Me.txtMinFDOSliverTolerance)
		Me.grbSliverTolerance.Controls.Add(Me.txtMaxFDOSliverTolerance)
		Me.grbSliverTolerance.Controls.Add(Me.lblMinFDOSliverTolerance)
		Me.grbSliverTolerance.Controls.Add(Me.lblMaxFDOSliverTolerance)
		Me.grbSliverTolerance.Controls.Add(Me.chkRemoveSlivers)


		Me.tbpParameters.Controls.Add(Me.lblStraightTolerance)
		Me.tbpParameters.Controls.Add(Me.lblLegendPaintFactor)
		Me.tbpParameters.Controls.Add(Me.grbFormats)
		Me.tbpParameters.Controls.Add(Me.grbNumberFormat)
		Me.tbpParameters.Controls.Add(Me.grbSliverTolerance)



		Me.tbpParameters.Controls.Add(Me.chkLotNameNum)
		Me.tbpParameters.Controls.Add(Me.chkCheckTopo)
		Me.tbpParameters.Controls.Add(Me.chkBlueLine)

		Me.tbpParameters.Controls.Add(Me.txtLegendPaintFactor)
		Me.tbpParameters.Controls.Add(Me.txtStraightenTolerance)
		pbEventsEnabled = False
		zzGetFDOSliverToleranceSetting()
		pbEventsEnabled = True
		If False Then
			Me.tbpParameters.Controls.Add(Me.txtMaxFDOSliverTolerance)
			Me.tbpParameters.Controls.Add(Me.txtMinFDOSliverTolerance)
			Me.tbpParameters.Controls.Add(Me.lblMinFDOSliverTolerance)
			Me.tbpParameters.Controls.Add(Me.lblMaxFDOSliverTolerance)
		End If





	End Sub
	Protected Overrides Sub zzInitTabApplication()
		DMAcadExt.AcadTransaction.Start()
		OpenProjectData(False, True)
		DMAcadExt.AcadTransaction.Terminate()

		Me.lstReports = New ListBox()
		Me.grbCalcOption = New System.Windows.Forms.GroupBox
		Me.grbOverlayOption = New System.Windows.Forms.GroupBox

		Me.rdbAcad = New System.Windows.Forms.RadioButton
		Me.rdbCalc = New System.Windows.Forms.RadioButton
		Me.rdbCalc2 = New System.Windows.Forms.RadioButton
		Me.rdbRounded = New System.Windows.Forms.RadioButton

		Me.rdbMerge = New System.Windows.Forms.RadioButton
		Me.rdbUnion = New System.Windows.Forms.RadioButton
		Me.rdbFDO = New System.Windows.Forms.RadioButton


		Me.grbPaintDest = New System.Windows.Forms.GroupBox
		Me.rdbPaintAppr = New System.Windows.Forms.RadioButton
		Me.rdbPaintProp = New System.Windows.Forms.RadioButton

		Me.cmbPaintScale = New System.Windows.Forms.ComboBox
		Me.cmdPaintByLanduse = New TabButton(moLabelFont, False)
		Me.cmdErasePaint = New TabButton(moLabelFont, False)
		Me.cmdClearPaint = New TabButton(moLabelFont, False)



		Me.grbCalcOption.SuspendLayout()
		Me.grbOverlayOption.SuspendLayout()
		Me.grbPaintDest.SuspendLayout()



		'
		'lstReports
		'
		With Me.lstReports
			.Location = New System.Drawing.Point(288, 32)
			.Name = "lstReports"
			.Enabled = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(220, 126)
			.TabIndex = 13
			.Text = "Erase"
			.Font = moLabelFont
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember
			'	MessageBox.Show(CStr(miaReports.GetUpperBound(0)) & ":" & CStr(miaReportTopoPurpose.GetUpperBound(0)) & ":" & CStr(miaReportOptions.GetUpperBound(0)), "01_333")
			For iIndex As Integer = 0 To miaReports.GetUpperBound(0)
                .Items.Add(New ReportItem(miaReports(iIndex), GetReportName(miaReports(iIndex), miaReportTopoPurpose(iIndex)), miaReportOptions(iIndex), miaReportTopoPurpose(iIndex), enReportModifications.Default, True, True, True))
			Next
		End With

		'
		'grbCalcOption
		'
		With Me.grbCalcOption
			.Controls.Add(Me.rdbCalc)
			.Controls.Add(Me.rdbCalc2)

			.Controls.Add(Me.rdbRounded)
			.Controls.Add(Me.rdbAcad)

			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(288, 152)
			.Name = "grbCalcOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(132, 88)
			.TabIndex = 1
			.TabStop = False
			.Enabled = False
		End With

		'
		'grbOverlayOption
		'
		With Me.grbOverlayOption
			.Controls.Add(Me.rdbMerge)
			.Controls.Add(Me.rdbUnion)
			.Controls.Add(Me.rdbFDO)


			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(424, 152)
			.Name = "grbOverlayOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(84, 70)
			.TabIndex = 2
			.TabStop = False
			.Enabled = False
		End With

		'
		'rdbAcad
		'
		With rdbAcad
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 8)
			.Name = "rdbAcad"
			.Size = New System.Drawing.Size(126, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(6, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'rdbCalc
		'
		With Me.rdbCalc
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 28)
			.Name = "rdbCalc"
			.Size = New System.Drawing.Size(126, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(7, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = True
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'rdbCalc2
		'
		With Me.rdbCalc2
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 48)
			.Name = "rdbCalc2"
			.Size = New System.Drawing.Size(126, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(8, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = False
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'rdbRounded
		'
		With Me.rdbRounded
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 68)
			.Name = "rdbRounded"
			.Size = New System.Drawing.Size(126, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(9, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = False
			.TextAlign = ContentAlignment.MiddleLeft
		End With

		'''''''''''''''''''
		Dim iOverlayMethodIndex As Integer = zzGetIntSetting("OverlayMethodIndex")
		'
		'rdbMerge
		'
		With rdbMerge
			.AutoSize = False
			.Checked = (iOverlayMethodIndex = 0)
			.Location = New System.Drawing.Point(0, 8)
			.Name = "rdbMerge"
			.Size = New System.Drawing.Size(80, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(10, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'rdbUnion
		'
		With Me.rdbUnion
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 28)
			.Name = "rdbUnion"
			.Size = New System.Drawing.Size(80, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(11, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = (iOverlayMethodIndex = 1)
			.TextAlign = ContentAlignment.MiddleLeft
			.Enabled = False
		End With
		'
		'rdbFDO
		'
		With Me.rdbFDO
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 48)
			.Name = "rdbFDO"
			.Size = New System.Drawing.Size(80, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(12, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = (iOverlayMethodIndex = 2)
			.TextAlign = ContentAlignment.MiddleLeft
		End With


		'
		'grbPaintDest
		'
		With Me.grbPaintDest
			.Controls.Add(Me.rdbPaintAppr)
			.Controls.Add(Me.rdbPaintProp)
			.Controls.Add(Me.cmbPaintScale)
			.Controls.Add(Me.cmdPaintByLanduse)
			.Controls.Add(Me.cmdErasePaint)
			.Controls.Add(Me.cmdSelectLanduse)
			.Controls.Add(Me.chkPaintStraighten)



			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(4, 180)
			.Name = "grbPaintDest"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(224, 88)
			.TabIndex = 11
			.TabStop = False
		End With

		'
		'rdbPaintAppr
		'
		With Me.rdbPaintAppr
			.Location = New System.Drawing.Point(96, 10)
			.Name = "rdbPaintAppr"
			.Size = New System.Drawing.Size(116, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(1, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
			.Checked = True
		End With
		'
		'rdbPaintProp
		'
		With rdbPaintProp
			.Location = New System.Drawing.Point(96, 32)
			.Name = "rdbPaintProp"
			.Size = New System.Drawing.Size(116, 17)
			.TabIndex = 1
			.TabStop = True
			.Text = zzGetText(2, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With

		'
		'cmbPaintScale
		'
		With Me.cmbPaintScale
			.FormattingEnabled = True
			.Location = New System.Drawing.Point(4, 60)
			.Name = "cmbPaintScale"
			.Size = New System.Drawing.Size(88, 21)
			.TabIndex = 22
			TPlanGraph.TplnProject.FillScales(Me.cmbPaintScale)

			Try
				.Text = "1:1000"
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
		End With

		'
		'cmdPaintByLanduse
		'
		With Me.cmdPaintByLanduse
			.Location = New System.Drawing.Point(112, 60)
			.Name = "cmdPaintByLanduse"
			.Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			.Enabled = False
			'    .Font = moLabelFont
			.Text = "Paint"
		End With
		'
		'cmdErasePaint
		'
		With Me.cmdErasePaint
			.Location = New System.Drawing.Point(168, 60)
			.Name = "cmdErasePaint"
			.Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Erase"
		End With
		'
		'cmdClearPaint
		'
		With Me.cmdClearPaint
			.Location = New System.Drawing.Point(240, 244)
			.Name = "cmdClearPaint"
			.Size = New System.Drawing.Size(48, 24)
			.TabIndex = 12
			'    .Font = moLabelFont
			.Text = "Clear"
		End With
		'
		'chkPaintStraighten
		'
		With Me.chkPaintStraighten
			.Location = New System.Drawing.Point(4, 36)
			.Name = "PaintStraighten"
			.Size = New System.Drawing.Size(92, 24)
			.TabIndex = 43
			.Font = moLabelFont
			.Text = zzGetText(19, 11)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.TextAlign = ContentAlignment.MiddleLeft
		End With

	



		With Me.tbpApplication.Controls
			.Add(Me.lstReports)
			.Add(Me.cmdCalculate)
			.Add(Me.cmdColorEditor)
			.Add(Me.cmdColorEditor)
			.Add(Me.cmdClearPaint)
			.Add(Me.cmdInsertRep)
			.Add(Me.cmdExpExcel)
			.Add(Me.cmdUpdateTopo)


			.Add(Me.chkApproved)
			.Add(Me.chkProposed)
			.Add(Me.chkParcel)
			.Add(Me.chkParcelClPgon)

			.Add(Me.chkMerge)
			.Add(Me.chkUnion)
			.Add(Me.chkFDO_Overlay)


			.Add(Me.grbCalcOption)
			.Add(Me.grbOverlayOption)


			.Add(Me.grbPaintDest)
		End With


		'	Me.tbpApplication.Controls.Add(Me.cmdSelectLanduse)
		'	Me.tbpApplication.Controls.Add(Me.chkPaintStraighten)


		Me.grbCalcOption.ResumeLayout(False)
		Me.grbOverlayOption.ResumeLayout(False)
		Me.grbCalcOption.PerformLayout()
		Me.grbOverlayOption.PerformLayout()

		Me.grbPaintDest.ResumeLayout(False)
		Me.grbPaintDest.PerformLayout()

		Dim baCalculateSetting() As Boolean = zzCalculateSetting()


		Me.chkParcel.Checked = baCalculateSetting(0)
		Me.chkApproved.Checked = baCalculateSetting(1)
		Me.chkProposed.Checked = baCalculateSetting(2)
		Me.chkMerge.Checked = baCalculateSetting(3)
		Me.chkUnion.Checked = baCalculateSetting(4)
		Me.chkFDO_Overlay.Checked = baCalculateSetting(5)

		If Me.chkApproved.Checked Then
			Me.rdbPaintAppr.Checked = True
		ElseIf Me.chkProposed.Checked Then
			Me.rdbPaintProp.Checked = True
		Else
			Me.rdbPaintAppr.Checked = True
			'Add !!!
		End If
		'	zzGetPaintScale()

	End Sub


	Private Shared Function GetReportName(ByVal iResourceTheme As TPlServerDB.enResourceTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
		Dim iElement As Integer
		Select Case iTopoPurpose
			Case DMAcadExt.enTopoPurpose.Approved
				iElement = 3
			Case DMAcadExt.enTopoPurpose.Proposed
				iElement = 4
			Case Else
				iElement = 2
		End Select
		Return TPlServerDB.TextResource.GetText(iElement, iResourceTheme, 0)
	End Function
	Private Class ReportItemAAA
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
	Private Shared Property zzCheckTopoSetting() As Boolean
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msCheckTopoSettingKey, "True")
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
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msCheckTopoSettingKey, sSetting)
		End Set
	End Property

	Private Shared Property zzBlueLineSetting() As Boolean
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msBlueLineSettingKey, "True")
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
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msBlueLineSettingKey, sSetting)
		End Set
	End Property
	Private Shared Property zzPaintStraightenSetting() As Boolean
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msPaintStraightenSettingKey, "True")
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
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msPaintStraightenSettingKey, sSetting)
		End Set
	End Property




	Private Shared Property zzCalculateSetting() As Boolean()
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msCalculateSettingKey, "63")
			Dim iSetting As Integer
			Try
				iSetting = Convert.ToInt32(sSettting)
			Catch
				iSetting = 63
			End Try
			Dim baOut(5) As Boolean
			Dim iDivisor As Integer = 1
			For iIndex As Integer = 0 To 5
				baOut(iIndex) = (iSetting And iDivisor) = iDivisor
				iDivisor *= 2
			Next
			Return baOut
		End Get
		Set(ByVal baValue() As Boolean)
			Dim iSetting As Integer
			Dim sSetting As String
			Dim iDivisor As Integer = 1

			For iIndex As Integer = 0 To 5
				If baValue(iIndex) Then iSetting += iDivisor
				iDivisor *= 2
			Next
			sSetting = Convert.ToString(iSetting)
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msCalculateSettingKey, sSetting)
		End Set
	End Property


	Private Sub zzCheckCalculateOptions()
		Dim bLotNotExists As Boolean = (Not Me.chkApproved.Checked Or Not Me.moaChecks(3).CheckState = CheckState.Checked) And (Not Me.chkProposed.Checked Or Not Me.moaChecks(4).CheckState = CheckState.Checked)
		Dim bParcelNotExists As Boolean = Not Me.chkParcel.Checked
		bLotNotExists = (Not Me.chkApproved.Checked) And (Not Me.chkProposed.Checked)
		'	MessageBox.Show(CStr(Me.chkApproved.Checked) & ":" & Me.moaChecks(3).CheckState.ToString(), CStr(Me.chkProposed.Checked) & ":" & Me.moaChecks(4).CheckState.ToString())
		''''''''''''''''	MessageBox.Show(CStr(bLotNotExists) & ":" & CStr(bParcelNotExists), "01_800")

		If (bLotNotExists Or bParcelNotExists) Then
			If Me.chkMerge.Checked Then Me.chkMerge.Checked = False
			Me.chkMerge.Enabled = False
			If Me.chkUnion.Checked Then Me.chkUnion.Checked = False
			Me.chkUnion.Enabled = False
		Else
			Me.chkMerge.Enabled = True
			'	Me.chkUnion.Enabled = True
		End If
	End Sub
	Private Sub cmdCalculate_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click
		Dim iColorSetID As Integer = 0
		Dim sLanduseList As String = Nothing
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose
		Me.Cursor = Cursors.WaitCursor
		Dim oTplanProjectData As TplanProjectData = DirectCast(doProjectData, TplanProjectData)
		If Me.chkApproved.Checked Then
			iTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			iColorSetID = oTplanProjectData.ColorSetID(iTopoPurpose)
			TPlanGraph.TplnLanduse.SetNamedColorSet(iTopoPurpose, iColorSetID)
			miDefaultTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			'	zzGetColorSet(DMAcadExt.enTopoPurpose.Approved, iColorSetID, sLanduseList)
		End If
		If Me.chkProposed.Checked Then
			iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
			iColorSetID = oTplanProjectData.ColorSetID(iTopoPurpose)
			TPlanGraph.TplnLanduse.SetNamedColorSet(iTopoPurpose, iColorSetID)
			miDefaultTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
		End If
		If Me.chkMerge.Checked Then
			TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TPlanGraph.enGeoMethod.Topologia
		End If
		TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
      TPlanGraph.TplnProject.CalculateOldVer(Me.chkApproved.Checked, Me.chkProposed.Checked, Me.chkParcel.Checked, Me.chkParcelClPgon.Checked, Me.chkMerge.Checked, Me.chkUnion.Checked, Me.chkFDO_Overlay.Checked)
		MyBase.OnCalculate()


		'	zzGetColorSet(iTopoPurpose, iColorSetID, sLanduseList)

		Me.lstReports.Enabled = True
		Me.cmdExpExcel.Enabled = True
		Me.cmdInsertRep.Enabled = True
		Me.cmdPaintByLanduse.Enabled = True

		Me.Cursor = Cursors.Default
		If Me.chkMerge.Checked AndAlso Not Me.chkFDO_Overlay.Checked Then
			Me.rdbMerge.Checked = True
		ElseIf Not Me.chkMerge.Checked AndAlso Me.chkFDO_Overlay.Checked Then
			Me.rdbFDO.Checked = True
		End If
		'   End If
	End Sub




	Private Sub chkApproved_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles chkApproved.CheckedChanged
		zzCheckCalculateOptions()
	End Sub

	Private Sub chkParcel_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles chkParcel.CheckedChanged
		zzCheckCalculateOptions()
		Me.chkParcelClPgon.Enabled = Me.chkParcel.Checked
	End Sub

	Private Sub chkProposed_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles chkProposed.CheckedChanged
		zzCheckCalculateOptions()
	End Sub

	Private Sub lstReports_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles lstReports.SelectedIndexChanged
		If Me.lstReports.SelectedItem Is Nothing Then
			doSelectedReportItem = Nothing
			Me.grbCalcOption.Enabled = False
			Me.grbOverlayOption.Enabled = False
		Else
			doSelectedReportItem = DirectCast(Me.lstReports.SelectedItem, ReportItem)
			Me.grbCalcOption.Enabled = ((doSelectedReportItem.Options And enReportOptions.AllAreaOptionsEnabled) = enReportOptions.AllAreaOptionsEnabled)
			Me.grbOverlayOption.Enabled = ((doSelectedReportItem.Options And enReportOptions.OverlayEnabled) = enReportOptions.OverlayEnabled)

		End If
	End Sub

	Private Sub zzTestInsert()
		'   Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
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
		Dim bResp As Boolean = TPlanGraph.TplnProject.GetPoint("start point", tPoint)
		'   Me.Show()

	End Sub

	Private Sub cmbBlockFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbBlockFormat.SelectedIndexChanged
		DMAcadExt.AcadBlockDef.Format = Me.cmbBlockFormat.SelectedIndex + 1
	End Sub

	Private Sub cmbTopoFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbTopoFormat.SelectedIndexChanged
		DMAcadExt.TopoDef.Format = Me.cmbTopoFormat.SelectedIndex + 1
	End Sub
	Private Sub cmbLanduseFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbLanduseFormat.SelectedIndexChanged

		TPlanGraph.TplnProject.LandusesFormatID = Me.cmbLanduseFormat.SelectedIndex + 1

	End Sub

	Private Sub cmbCoordFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbCoordFormat.SelectedIndexChanged
		Try

			TPlanGraph.TplnProject.CoordinateFormat = Me.cmbCoordFormat.SelectedItem.ToString()
			MyBase.OnFormatChanged()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbCoordFormat_SelectedIndexChanged")

		End Try

	End Sub
	Private Sub cmbAreaFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbAreaFormat.SelectedIndexChanged
		Try

			TPlanGraph.TplnProject.AreaFormat = Me.cmbAreaFormat.SelectedItem.ToString()
			MyBase.OnFormatChanged()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbAreaFormat_SelectedIndexChanged")
		End Try
	End Sub

	Private Function zzGetPaintTopoPurpose() As DMAcadExt.enTopoPurpose
		If rdbPaintAppr IsNot Nothing AndAlso Me.rdbPaintProp IsNot Nothing Then
			If Me.rdbPaintAppr.Checked Then
				Return DMAcadExt.enTopoPurpose.Approved
			ElseIf Me.rdbPaintProp.Checked Then
				Return DMAcadExt.enTopoPurpose.Proposed
			End If
		Else
			Return DMAcadExt.enTopoPurpose.Approved
		End If

	End Function
	Private Sub cmdPaintByLanduse_Click(ByVal oSender As System.Object, ByVal oEventArgs As System.EventArgs) Handles cmdPaintByLanduse.Click
		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim dBaseScale As Double = 1000.0
		Dim dScale As Double = zzGetSelectedScale()
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim sPaintLayer As String
		If iTopoPurpose <> DMAcadExt.enTopoPurpose.Undefined AndAlso dScale > 0.0 Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


			'	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_201")
			DMAcadExt.AcadDocument.SaveVarCmdDia(0S)
			If zzCreateLuseTopology(iTopoPurpose, Me.chkPaintStraighten.Checked) Then

				TopoManager.TPlanGraph.TplnProject.LoadLusePgons(iTopoPurpose)
            sPaintLayer = TopoManager.TPlanGraph.TplnProject.SetPaintLayers(DMAcadExt.enMapTheme.LotApproved, iTopoPurpose)
				If sPaintLayer IsNot Nothing Then
					TPlanGraph.TplnLot.PaintByLanduseTopo(iTopoPurpose, dScale, sPaintLayer)
				End If
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
			Me.Cursor = Cursors.Default
		End If
	End Sub
	Private Function zzCreateLuseTopology(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bPaintStraighten As Boolean) As Boolean
		Dim tDisTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose, DMAcadExt.enOverlayMethod.Dissolve)
		Dim oDisTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(tDisTopoDefID)

		Dim tSrcTopDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
		Dim tAltTopDefID As DMAcadExt.TopoDefID = tSrcTopDefID.AdditionalID
		Dim sLanduseTopoName As String = TopoDefs.GetTopoName(tDisTopoDefID)
		If TopoCreator.TopologyExists(oDisTopoDef) Then
			Return True
		Else
			TopoCreator.DissolveTopo(oDisTopoDef)
			If TopoCreator.TopologyExists(oDisTopoDef) Then
				ResetCheck(tDisTopoDefID)
				Return True
			Else
				Return False
			End If
		End If

	End Function
	Private Sub cmdColorset_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdColorEditor.Click
		If dfEditColorScheme Is Nothing Then
			dfEditColorScheme = New frmColorEditor(0, TopoManager.enColorEditorMode.Landuse, False)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, dfEditColorScheme)
		End If
	End Sub
	Private Sub zzGetColorSet(ByRef iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef iColorSetID As Integer, ByRef sLanduseList As String)

		Dim oTplanProjectData As TplanProjectData
		Dim dPaintScale As Double
		Dim sPaintScale As String
		iTopoPurpose = zzGetPaintTopoPurpose()
		iColorSetID = 0

		sLanduseList = Nothing
		''''''	OpenProjectData(False, True)

		If doProjectData.Opened Then

			oTplanProjectData = DirectCast(doProjectData, TplanProjectData)
			iColorSetID = oTplanProjectData.ColorSetID(iTopoPurpose)
			sLanduseList = oTplanProjectData.LanduseList(iTopoPurpose)
			sPaintScale = oTplanProjectData.PaintScale(iTopoPurpose)
			dPaintScale = DMCommon.Functions.TextToScale(sPaintScale, 1250.0)
			Me.cmbPaintScale.SelectedItem = "1:" & CStr(dPaintScale)
			'oTplanProjectData.Close()
		End If



	End Sub

	Private Sub zzGetPaintScale()	 'ByRef iTopoPurpose As DMAcadExt.enTopoPurpose
		Dim oTplanProjectData As TplanProjectData
		Dim dPaintScale As Double
		Dim sPaintScale As String
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()

		If doProjectData.Opened Then
			oTplanProjectData = DirectCast(doProjectData, TplanProjectData)

			sPaintScale = oTplanProjectData.PaintScale(iTopoPurpose)
			dPaintScale = DMCommon.Functions.TextToScale(sPaintScale, 1250.0)
			If Me.cmbPaintScale IsNot Nothing Then
				Me.cmbPaintScale.SelectedItem = "1:" & CStr(dPaintScale)
			End If
			'oTplanProjectData.Close()
		End If

	End Sub
	Private Sub dfSelectLanduse_ChangeColorSet() Handles dfSelectLanduse.ChangeColorSet
		If dfSelectLanduse.Dirty Then
			TPlanGraph.TplnLot.RefreshColorSchemes(zzGetPaintTopoPurpose())
		End If
	End Sub
   Private Sub dfSelectLanduse_FormClosed(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles dfSelectLanduse.FormClosed
      Try
         Me.dfSelectLanduse.Dispose()
         Me.dfSelectLanduse = Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActions - dfSelectLanduse_FormClosed")
      End Try

   End Sub

   Private Sub cmdSelectLanduse_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdSelectLanduse.Click
      If dfSelectLanduse Is Nothing Then
         Dim iTopoPurpose As DMAcadExt.enTopoPurpose
         Dim iColorSetID As Integer = 0
         Dim sLanduseList As String = Nothing

         zzGetColorSet(iTopoPurpose, iColorSetID, sLanduseList)
         dfSelectLanduse = New frmSelectLanduse(iTopoPurpose, iColorSetID, sLanduseList)
         Me.AddOwnedForm(dfSelectLanduse)
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, dfSelectLanduse)
      End If

   End Sub


	Private Sub mfSelectLanduse_SetColorSet(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iColorSetID As Integer, ByVal sLanduseList As String) Handles dfSelectLanduse.SetColorSet
		Dim oTplanProjectData As TplanProjectData

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(iColorSetID) & ":" & sLanduseList, "18_219a")
		If doProjectData.Opened AndAlso doProjectData.Mode = Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead Then
			doProjectData.CloseDictionary()
		End If
		If Not doProjectData.Opened Then

			doProjectData.OpenData(True, False)
		End If

		'MessageBox.Show(CStr(iColorSetID), "18_220")
		If doProjectData.Opened Then
			'	MessageBox.Show(doProjectData.Mode.ToString(), "18_220a")
			oTplanProjectData = DirectCast(doProjectData, TplanProjectData)
			oTplanProjectData.ColorSetID(iTopoPurpose) = iColorSetID
			oTplanProjectData.LanduseList(iTopoPurpose) = sLanduseList
			'	oTplanProjectData.PaintScale(iTopoPurpose) = sLanduseList


			'	oTplanProjectData.Close()

		End If

		TPlanGraph.TplnLot.RefreshColorSchemes(iTopoPurpose)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	Private Sub cmdErasePaint_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdErasePaint.Click
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      'Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
      TPlanGraph.TplnProject.ClearPaint(DMAcadExt.enMapTheme.LotApproved) ', iTopoPurpose

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdClearPaint_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClearPaint.Click
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		TPlanGraph.TplnProject.ClearPaintTemp()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub
	Private Sub rdbPaintAppr_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles rdbPaintAppr.CheckedChanged
		zzGetPaintScale()
	End Sub
	Private Function zzGetIntSetting(ByVal sSettingName As String, Optional ByVal iDefaultValue As Integer = 0) As Integer
		Try
			Dim oDynamic As System.Object = My.Settings.Item(sSettingName)
			If oDynamic IsNot Nothing Then
				Return DirectCast(oDynamic, Integer)
			Else
				Return iDefaultValue
			End If


		Catch oEx As Exception
			'StopA
			Return 0
		End Try
	End Function
	Private Sub zzSaveIntSetting(ByVal sSettingName As String, ByVal iValue As Integer)
		Try
			My.Settings.Item(sSettingName) = iValue
		Catch oEx As Exception
			'StopA
		End Try
	End Sub
	Private Sub zzSaveMySettings()
		Dim iOverlayMethodIndex As Integer
		Try
			If Me.rdbMerge.Checked Then
				iOverlayMethodIndex = 0
			ElseIf Me.rdbUnion.Checked Then
				iOverlayMethodIndex = 1
			ElseIf Me.rdbFDO.Checked Then
				iOverlayMethodIndex = 2
			End If
			zzSaveIntSetting("OverlayMethodIndex", iOverlayMethodIndex)
		Catch oEx As Exception
		End Try
		Dim dStraightenTolerance As Double
		Try
			dStraightenTolerance = Convert.ToDouble(Me.txtStraightenTolerance.Text)
			zzSaveDoubleSetting("StraightenTolerance", dStraightenTolerance)
		Catch oEx As Exception


		End Try

		Try
			dStraightenTolerance = Convert.ToDouble(Me.txtStraightenTolerance.Text)
			zzSaveDoubleSetting("RepBamashSharedScale", dStraightenTolerance)
		Catch oEx As Exception
		End Try
	End Sub

	Private Sub zzSaveDoubleSetting(ByVal sSettingName As String, ByVal dValue As Double)
		Try

			My.Settings.Item(sSettingName) = dValue
		Catch oEx As Exception
			'StopA
		End Try
	End Sub

	Private Sub frmTopoActionsT_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		zzSaveMySettings()
	End Sub

	Private Sub chkRemoveSlivers_CheckedChanged(oSender As System.Object, e As System.EventArgs) Handles chkRemoveSlivers.CheckedChanged
		zzSaveFDOSliverToleranceSetting()
		Dim bEnabled As Boolean = Not Me.chkRemoveSlivers.Checked
		Me.txtMinFDOSliverTolerance.Enabled = bEnabled
		Me.txtMaxFDOSliverTolerance.Enabled = bEnabled
	End Sub

	Private Sub txtMinFDOSliverTolerance_Leave(oSender As System.Object, e As System.EventArgs) Handles txtMinFDOSliverTolerance.Leave
		zzSaveFDOSliverToleranceSetting()
	End Sub

	Private Sub txtMaxFDOSliverTolerance_Leave(oSender As System.Object, e As System.EventArgs) Handles txtMaxFDOSliverTolerance.Leave
		zzSaveFDOSliverToleranceSetting()
	End Sub

	Private Sub zzGetFDOSliverToleranceSetting()
		Me.chkRemoveSlivers.Checked = AllSlivers
		Me.txtMinFDOSliverTolerance.Enabled = Not AllSlivers
		Me.txtMaxFDOSliverTolerance.Enabled = Not AllSlivers
		Me.txtMinFDOSliverTolerance.Text = CStr(MinFDOSliverTolerance)
		Me.txtMaxFDOSliverTolerance.Text = CStr(MaxFDOSliverTolerance)
	End Sub
	Private Sub zzSaveFDOSliverToleranceSetting()
		If pbEventsEnabled Then
			AllSlivers = Me.chkRemoveSlivers.Checked
		 
			If IsNumeric(Me.txtMinFDOSliverTolerance.Text) Then
				MinFDOSliverTolerance = Convert.ToDouble(Me.txtMinFDOSliverTolerance.Text)
			End If
			If IsNumeric(Me.txtMaxFDOSliverTolerance.Text) Then
				MaxFDOSliverTolerance = Convert.ToDouble(Me.txtMaxFDOSliverTolerance.Text)
			End If
			My.Settings.Save()
		End If
	End Sub
End Class