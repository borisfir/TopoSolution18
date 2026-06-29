Option Explicit On
Option Strict On
Imports TopoManager
Imports AcadReport
Imports System.Data
Public Class frmReports


	Private miaTabaReports() As TPlServerDB.enResourceTheme = {
	TPlServerDB.enResourceTheme.AcRepContent _
	, TPlServerDB.enResourceTheme.AcRepContent _
	, TPlServerDB.enResourceTheme.AcRepPlanParcels _
	, TPlServerDB.enResourceTheme.AcRepPlanParcels _
	, TPlServerDB.enResourceTheme.AcRepLotsK _
	, TPlServerDB.enResourceTheme.AcRepLotsK _
	, TPlServerDB.enResourceTheme.AcRepLotsM _
	, TPlServerDB.enResourceTheme.AcRepLotsM _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseK _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseK _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseCol _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseCol _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseCol _
	, TPlServerDB.enResourceTheme.AcRepParcelLuseCol _
	, TPlServerDB.enResourceTheme.AcRepSumLuse _
	, TPlServerDB.enResourceTheme.AcRepSumLuse _
	, TPlServerDB.enResourceTheme.AcRepJointLuse _
	, TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel _
	, TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel _
	, TPlServerDB.enResourceTheme.AcRepLegendK _
	, TPlServerDB.enResourceTheme.AcRepLegendK _
	, TPlServerDB.enResourceTheme.AcRepLegalParcels _
	, TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock _
	, TPlServerDB.enResourceTheme.AcRepOwnership _
	, TPlServerDB.enResourceTheme.AcRepOwnersSum _
	, TPlServerDB.enResourceTheme.AcRepOwnersSum _
	, TPlServerDB.enResourceTheme.AcRepExproLuse}





	'	  , TPlServerDB.enResourceTheme.AcRepBlockArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContent _
	'	 , TPlServerDB.enResourceTheme.AcRepLotContentArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel _
	Private miaOwnerReports() As TPlServerDB.enResourceTheme = { _
	 TPlServerDB.enResourceTheme.AcRepOwners}





	Private miaReportOptions() As enReportOptions = {
	enReportOptions.OverlayEnabled _
	, enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled Or enReportOptions.CalcMergeArea2Dflt _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled Or enReportOptions.CalcMergeArea2Dflt _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
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
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
	, enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled}





	Private miaReportTopoPurpose() As DMAcadExt.enTopoPurpose = {
	 DMAcadExt.enTopoPurpose.Undefined _
	, DMAcadExt.enTopoPurpose.Undefined _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Expro _
	, DMAcadExt.enTopoPurpose.Expro _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Undefined _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Proposed _
	, DMAcadExt.enTopoPurpose.Undefined _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Approved _
	, DMAcadExt.enTopoPurpose.Expro}





	Private miaReportModification() As enReportModifications = {
	 enReportModifications.Default _
	, enReportModifications.Multi _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
		, enReportModifications.Default _
		, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Additional _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Default _
	, enReportModifications.Additional _
	, enReportModifications.Template}


	Private WithEvents cmdInsertRep As Button
	Private WithEvents cmdExpExcel As Button
	Private WithEvents cmdAreaToBlock As Button
	Private WithEvents cmdRegionSet As Button

	Private WithEvents mfRegionSet As frmRegionSet



	Private WithEvents rdbAcad As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc2 As System.Windows.Forms.RadioButton
	Private WithEvents rdbRounded As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalcRounded As System.Windows.Forms.RadioButton


	Private WithEvents chkAreaMeter As System.Windows.Forms.CheckBox
	Private WithEvents chkMergeRows As System.Windows.Forms.CheckBox


	Private WithEvents chkFirstBlueLine As System.Windows.Forms.CheckBox

	Private WithEvents rdbMerge As System.Windows.Forms.RadioButton
	Private WithEvents rdbUnion As System.Windows.Forms.RadioButton
	Private WithEvents rdbFDO As System.Windows.Forms.RadioButton

	Private WithEvents lstReports As ListBox

	Private Shared diResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan
	Private mbEventsEnabled As Boolean = False

	Private grbCalcOption As System.Windows.Forms.GroupBox
	Private grbOverlayOption As System.Windows.Forms.GroupBox
   Private cmbReportScale As ComboBox
   Private lblReportScale As Label
   Private lblRegionList As Label
	Private lstRegions As ListBox
	'Private WithEvents clbRegions As CheckedListBox

	Private miCurrentRegionNo As Integer = TPlanGraph.TplnRegion.RegionAllIn
	Private mhsCurrentRegions As HashSet(Of Integer)
	Private miCurrentRegionIndex As Integer = 0
	Private msCurrentRegionName As String
	'''''''''''''''Private Shared diResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan

	Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan
	Private mfOwnership As frmOwnership
	''''''''''''''''''''''''''''''''''''Private miDefaultTopoPurpose As DMAcadExt.enTopoPurpose
	'	Private Shared diResourceTheme As TPlServerDB.enResourceTheme
	Private mbAutocad As Boolean
	Private moSelectedReportItem As ReportItem
	Protected doReportApp As AcadReport.Report
   Private moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Public Event Recalculate(ByVal iRegion As Integer)
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()
		'miCurrentRegionNo = iRegion
		zzMyInitializeComponent()
		' Add any initialization after the InitializeComponent() call.

	End Sub
	Public Property FormOwnership As frmOwnership
		Get
			Return mfOwnership
		End Get
		Set(oValue As frmOwnership)
			mfOwnership = oValue
		End Set
	End Property
	Private Sub zzMyInitializeComponent()


		Me.cmdInsertRep = New Button()
		Me.cmdExpExcel = New Button
		Me.cmdAreaToBlock = New Button
		Me.cmdRegionSet = New Button
		Me.lstReports = New ListBox()

		Me.grbCalcOption = New System.Windows.Forms.GroupBox()
		Me.grbOverlayOption = New System.Windows.Forms.GroupBox()

		Me.cmbReportScale = New System.Windows.Forms.ComboBox()
		Me.lblReportScale = New System.Windows.Forms.Label()
		Me.lblRegionList = New System.Windows.Forms.Label()
		Me.lstRegions = New ListBox()
		'Me.clbRegions = New CheckedListBox()
		Me.rdbAcad = New System.Windows.Forms.RadioButton()
		Me.rdbCalc = New System.Windows.Forms.RadioButton()
		Me.rdbCalc2 = New System.Windows.Forms.RadioButton()
		Me.rdbRounded = New System.Windows.Forms.RadioButton()
		Me.rdbCalcRounded = New System.Windows.Forms.RadioButton()
		Me.chkAreaMeter = New System.Windows.Forms.CheckBox()
		Me.chkMergeRows = New System.Windows.Forms.CheckBox()


		Me.chkFirstBlueLine = New System.Windows.Forms.CheckBox()



		Me.rdbMerge = New System.Windows.Forms.RadioButton()
		Me.rdbUnion = New System.Windows.Forms.RadioButton()
		Me.rdbFDO = New System.Windows.Forms.RadioButton()



		'	CType(Me.dgvActions, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()





		'
		'cmdInsertRep
		'
		With Me.cmdInsertRep
			.Location = New System.Drawing.Point(204 - 24, 4)
			.Name = "cmdInsertRep"
			'.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 13
			'    .Font = moLabelFont
			.Text = "Insert"
		End With
		'
		'cmdExpExcel
		'
		With Me.cmdExpExcel
			.Location = New System.Drawing.Point(48 + 24, 4)
			.Name = "cmdExpExcel"
			'.Enabled = False
			.Size = New System.Drawing.Size(64, 24)
			.TabIndex = 19
			'    .Font = moLabelFont
			.Text = "Excel"
		End With
		'
		'cmdAreaToBlock
		'
		With Me.cmdAreaToBlock
			.Location = New System.Drawing.Point(360, 240)
			.Name = "cmdAreaToBlock"
			'.Image = Global.TopoUI.My.Resources.Resources.Invert12
			'Global.TopoUI.My.Resources.Resources.Refresh22Tr
			'Global.TopoUI.My.Resources.Resources.Calc321

			'.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
			.Size = New System.Drawing.Size(80, 23)
			.Text = "Area-->Block"
			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText

			.TabIndex = 19
			'    .Font = moLabelFont
			'.Text = "Calc"

			.UseVisualStyleBackColor = True

		End With

		'
		'cmdRegionSet
		'

		With Me.cmdRegionSet
			.Location = New System.Drawing.Point(420, 66)
			.Name = "cmdRegionSet"
			.Image = Global.TopoUI.My.Resources.Resources.Edit16
			'Global.TopoUI.My.Resources.Resources.Refresh22Tr
			'Global.TopoUI.My.Resources.Resources.Calc321

			'.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
			.Size = New System.Drawing.Size(25, 25)
			.Text = ""
			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
			.TabIndex = 20
			.UseVisualStyleBackColor = True
		End With




		'zzFillReportList()
		zzFillReportListByDB()
		'
		'grbCalcOption
		'
		With Me.grbCalcOption
			.Controls.Add(Me.rdbCalc)
			.Controls.Add(Me.rdbCalc2)

			.Controls.Add(Me.rdbRounded)
			.Controls.Add(Me.rdbAcad)
			.Controls.Add(Me.rdbCalcRounded)


			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(48, 152)
			.Name = "grbCalcOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(132, 108)
			.TabIndex = 1
			.TabStop = False
			'.Enabled = False
		End With

		'
		'grbOverlayOption
		'
		With Me.grbOverlayOption
			.Controls.Add(Me.rdbMerge)
			.Controls.Add(Me.rdbUnion)
			.Controls.Add(Me.rdbFDO)


			.FlatStyle = System.Windows.Forms.FlatStyle.Popup
			.Location = New System.Drawing.Point(240, 152)
			.Name = "grbOverlayOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(84, 70)
			.TabIndex = 2
			.TabStop = False
			'	.Enabled = False
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
		'
		'rdbCalcRounded
		'
		With Me.rdbCalcRounded
			.AutoSize = False
			.Location = New System.Drawing.Point(0, 88)
			.Name = "rdbCalcRounded"
			.Size = New System.Drawing.Size(126, 17)
			.TabIndex = 0
			.TabStop = True
			.Text = zzGetText(10, 2)
			.UseVisualStyleBackColor = True
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Checked = False
			.TextAlign = ContentAlignment.MiddleLeft
		End With


		'
		'chkAreaMeter
		'
		With Me.chkAreaMeter
			.Location = New System.Drawing.Point(48, 264)
			.Name = "chkAreaMeter"
			.Size = New System.Drawing.Size(132, 24)
			.TabIndex = 17
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Text = zzGetText(18, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Visible = True
		End With
		Me.Controls.Add(Me.chkAreaMeter)
		'
		'chkMergeRows
		'
		With Me.chkMergeRows
			.Location = New System.Drawing.Point(240, 244)
			.Name = "chkMergeRows"
			.Size = New System.Drawing.Size(96, 24)
			.TabIndex = 17
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Text = zzGetText(19, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Checked = True
			.Visible = True
		End With
		Me.Controls.Add(Me.chkMergeRows)


		'
		'chkFirstBlueLine
		'
		With Me.chkFirstBlueLine
			.Location = New System.Drawing.Point(48, 274)
			.Name = "chkFirstBlueLine"
			.Size = New System.Drawing.Size(132, 24)
			.TabIndex = 17
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Text = zzGetText(17, 2)
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Checked = TPlanGraph.TplnProject.FirstBlueLine
			.Visible = False
		End With
		Me.Controls.Add(Me.chkFirstBlueLine)
		zzSetFirstBlueLineEnabled()

		'''''''''''''''''''
		Dim iOverlayMethodIndex As Integer = 2   'zzGetIntSetting("OverlayMethodIndex")

		'
		'rdbMerge
		'
		With rdbMerge
			.AutoSize = False
			'	.Checked = (iOverlayMethodIndex = 0)
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
			'	.Checked = (iOverlayMethodIndex = 1)
			.TextAlign = ContentAlignment.MiddleLeft
			'	.Enabled = False
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
			'	.Checked = (iOverlayMethodIndex = 2)
			.TextAlign = ContentAlignment.MiddleLeft
		End With
		'
		'cmbReportScale
		'
		With Me.cmbReportScale
			.FormattingEnabled = True
			.Location = New System.Drawing.Point(340, 32)
			.Name = "cmbReportScale"
			.Size = New System.Drawing.Size(120, 21)
			.TabIndex = 22
			TPlanGraph.TplnProject.FillScales(Me.cmbReportScale)

			Try
				.Text = "1:1000"
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
		End With

		Me.Controls.Add(Me.cmbReportScale)

		'
		'lblReportScale
		'
		With Me.lblReportScale
			.Location = New System.Drawing.Point(340, 12)
			.Name = "lblReportScale"
			.Size = New System.Drawing.Size(64, 21)
			.TabIndex = 16
			.Text = "קנ""ם:"


		End With

		Me.Controls.Add(Me.lblReportScale)

		'
		'lblRegionList
		'
		With Me.lblRegionList
			.AutoSize = True
			.Location = New System.Drawing.Point(340, 65)
			.Name = "lblRegionList"
			.Size = New System.Drawing.Size(64, 21)
			.TabIndex = 16
			.Text = "מתחמים:"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes

		End With

		Me.Controls.Add(Me.lblRegionList)

		'
		'lstRegions
		'
		With Me.lstRegions

			.Location = New System.Drawing.Point(340, 84)
			.Name = "lstRegions"
			'.Enabled = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(80, 126)
			.TabIndex = 14

			.Font = moLabelFont
			'  .ValueMember = DMCommon.ItemData.ValueMember
			'  .DisplayMember = DMCommon.ItemData.DisplayMember
			'	MessageBox.Show(CStr(miaReports.GetUpperBound(0)) & ":" & CStr(miaReportTopoPurpose.GetUpperBound(0)) & ":" & CStr(miaReportOptions.GetUpperBound(0)), "01_333")
			If TopoManager.TPlanGraph.TplnProject.Regions IsNot Nothing AndAlso TopoManager.TPlanGraph.TplnProject.Regions.Count > 1 Then
				For Each RegionNo As Integer In TopoManager.TPlanGraph.TplnProject.Regions.Keys
					''''''''''''''''''''''''''''''''''''''''''''''''''''19GETALL/.Items.Add(RegionNo)
				Next
				'.Items.Add(TPlanGraph.TplnRegion.GetAllRegion())
				.Items.Add(TPlanGraph.TplnRegion.GetAllInRegion())
				For Each oRegion As TPlanGraph.TplnRegion In TopoManager.TPlanGraph.TplnProject.Regions.Values
					.Items.Add(oRegion)
				Next

			End If
			.Visible = True
			If .Items.Count > 0 Then
				.SelectedIndex = 0
			End If


		End With

		Me.Controls.Add(Me.lstRegions)
		zzLoadRegionSet()
		'
		'clbRegions
		'
		'With Me.clbRegions

		'	.Location = New System.Drawing.Point(340, 84)
		'	.Name = "clbRegions"
		'	'	.Enabled = False
		'	.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		'	.Size = New System.Drawing.Size(120, 144)
		'	.TabIndex = 14

		'	.Font = moLabelFont

		'	If TopoManager.TPlanGraph.TplnProject.Regions IsNot Nothing AndAlso TopoManager.TPlanGraph.TplnProject.Regions.Count > 1 Then
		'		For Each RegionNo As Integer In TopoManager.TPlanGraph.TplnProject.Regions.Keys
		'			''''''''''''''''''''''''''''''''''''''''''''''''''''19GETALL/.Items.Add(RegionNo)
		'		Next
		'		.Items.Add(TPlanGraph.TplnRegion.GetAllInRegion())
		'		If miCurrentRegionNo = 0 Then
		'			miCurrentRegionIndex = 0
		'		End If
		'		For Each oRegion As TPlanGraph.TplnRegion In TopoManager.TPlanGraph.TplnProject.Regions.Values
		'			.Items.Add(oRegion)
		'			iRegionIndex += 1
		'			If oRegion.RegionNo = miCurrentRegionNo Then
		'				miCurrentRegionIndex = iRegionIndex
		'			End If

		'		Next
		'		.SetItemChecked(miCurrentRegionIndex, True)
		'		Me.Controls.Add(Me.clbRegions)

		'	End If



		'End With




		Me.Controls.Add(Me.lstReports)
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba Then
			Me.Controls.Add(Me.grbCalcOption)
			Me.Controls.Add(Me.grbOverlayOption)
		Else
			Me.grbCalcOption.Enabled = False
			Me.grbOverlayOption.Enabled = False
		End If

		Me.Controls.Add(Me.cmdInsertRep)
		Me.Controls.Add(Me.cmdExpExcel)
		Me.Controls.Add(Me.cmdAreaToBlock)
		Me.Controls.Add(Me.cmdRegionSet)




		'	ffffffffffff()



		Me.Location = New Point(200, 200)
		Select Case TopoManager.TPlanGraph.TplnProject.OverlayMethod
			Case DMAcadExt.enOverlayMethod.FDO_Overlay
				Me.rdbFDO.Checked = True
			Case DMAcadExt.enOverlayMethod.Merge
				Me.rdbMerge.Checked = True
			Case Else
				Me.rdbFDO.Checked = True
		End Select
		mbEventsEnabled = True
	End Sub
	Private Sub zzLoadRegionSet()


		'
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		TopoManager.TPlanGraph.TplnRegionSet.Load()
		'	TopoManager.TPlanGraph.TplnRegionSet.Add("אבגדה", "1,2")
		Dim oRegionSet() As TopoManager.TPlanGraph.TplnRegionSet = TopoManager.TPlanGraph.TplnRegionSet.GetArray()
		TopoManager.TPlanGraph.TplnRegionSet.HasPrefix = True
		If oRegionSet IsNot Nothing AndAlso oRegionSet.GetUpperBound(0) >= 0 Then
			For iIndex As Integer = 0 To oRegionSet.GetUpperBound(0)
				Me.lstRegions.Items.Add(oRegionSet(iIndex))
			Next

		End If

		'	TopoManager.TPlanGraph.TplnRegionSet.Test()

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzClearRegionSet()

		For iIndex As Integer = Me.lstRegions.Items.Count - 1 To TopoManager.TPlanGraph.TplnProject.Regions.Values.Count + 1 Step -1
			Me.lstRegions.Items.RemoveAt(iIndex)
		Next
	End Sub
	Private Sub zzFillReportListByDB()
		Dim oReportItem As ReportItem
		Dim iResourceTheme As TPlServerDB.enResourceTheme
		Dim iReportOption As enReportOptions
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose
		Dim iReportModifications As enReportModifications
		Dim sReportName As String
		'
		'lstReports
		'iResourceTheme
		With Me.lstReports
			.Location = New System.Drawing.Point(48, 32)
			.Name = "lstReports"
			'	.Enabled = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(280, 126)
			.TabIndex = 13
			.Text = "Erase"
			.Font = moLabelFont
			'	.DrawMode
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember
			Dim iIndex As Integer = 0
			'	DMCommon.Debug.MsgBox("01_333", miaTabaReports.GetUpperBound(0), miaReportTopoPurpose.GetUpperBound(0), miaReportOptions.GetUpperBound(0))
			Dim sComText As String = "SELECT [ResourceTheme],[Options],[TopoPurpose],[Modification],[Name] FROM [Reports] ORDER BY ItemOrder"



			Dim oDataReader As Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			If oDataReader.HasRows Then

				While oDataReader.Read()
					iResourceTheme = DMCommon.Functions.CEnumN(Of TPlServerDB.enResourceTheme)(oDataReader.GetInt32(0), TPlServerDB.enResourceTheme.Undefined)
					'	iReportOption = DMCommon.Functions.CEnumN(Of enReportOptions)(oDataReader.GetInt32(1), enReportOptions.Default)
					iReportOption = CType(DMCommon.Functions.CIntN(oDataReader.GetInt32(1), 1), enReportOptions)

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ReportOption", iReportOption, CInt(iReportOption), CType(CInt(iReportOption), enReportOptions))
					iTopoPurpose = DMCommon.Functions.CEnumN(Of DMAcadExt.enTopoPurpose)(oDataReader.GetInt32(2), DMAcadExt.enTopoPurpose.Undefined)
					iReportModifications = DMCommon.Functions.CEnumN(Of enReportModifications)(oDataReader.GetInt32(3), enReportModifications.Default)
					If oDataReader.IsDBNull(4) Then
						sReportName = Nothing
					Else
						sReportName = DMCommon.Functions.CStrN(oDataReader.GetString(4))
					End If

					oReportItem = New ReportItem(iResourceTheme, GetReportName(sReportName, iResourceTheme, iTopoPurpose, iReportModifications), iReportOption, iTopoPurpose, iReportModifications, True, True, True)

					.Items.Add(oReportItem)
					iIndex += 1
				End While
			End If
			oDataReader.Close()



		End With
	End Sub
	Private Sub zzFillReportList()
		Dim oReportItem As ReportItem
		Dim iReportsUB As Integer
		Dim iRegionIndex As Integer = 0

		Select Case DMAcadExt.DMApp.AppID
			Case DMAcadExt.enApplications.Taba


				iReportsUB = miaTabaReports.GetUpperBound(0)
			Case DMAcadExt.enApplications.Ownership
				iReportsUB = miaOwnerReports.GetUpperBound(0)

		End Select
		'
		'lstReports
		'
		With Me.lstReports
			.Location = New System.Drawing.Point(48, 32)
			.Name = "lstReports"
			'	.Enabled = False
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(280, 126)
			.TabIndex = 13
			.Text = "Erase"
			.Font = moLabelFont
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember
			'	DMCommon.Debug.MsgBox("01_333", miaTabaReports.GetUpperBound(0), miaReportTopoPurpose.GetUpperBound(0), miaReportOptions.GetUpperBound(0))
			For iIndex As Integer = 0 To iReportsUB
				Select Case DMAcadExt.DMApp.AppID
					Case DMAcadExt.enApplications.Taba
						'DMCommon.Debug.MsgBox("01_333g", iIndex, miaTabaReports(iIndex), miaReportTopoPurpose(iIndex), miaReportOptions(iIndex), miaReportModification(iIndex))
						oReportItem = New ReportItem(miaTabaReports(iIndex), GetReportName(String.Empty, miaTabaReports(iIndex), miaReportTopoPurpose(iIndex), miaReportModification(iIndex)), miaReportOptions(iIndex), miaReportTopoPurpose(iIndex), miaReportModification(iIndex), True, True, True)
					Case DMAcadExt.enApplications.Ownership
						oReportItem = New ReportItem(miaOwnerReports(iIndex), GetReportName(miaOwnerReports(iIndex)), enReportOptions.Default, True, True, True)
					Case Else
						Return
				End Select

				.Items.Add(oReportItem)
			Next
		End With
	End Sub
	Private Sub cmdInsertRep_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdInsertRep.Click
		Dim oRepApp As AcadReport.Report = New AcadReport.Report()
		mbAutocad = True
      zzLaunchReport(oRepApp)
   End Sub
	Private Sub cmdAreaToBlock_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdAreaToBlock.Click
		zzAreaToBlock()

	End Sub
	Private Sub zzLaunchReport(ByVal oReport As AcadReport.BaseReport)
		Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
		Dim sAreaOptionText As String = Nothing
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod
		Dim iOverlayIndex As DMAcadExt.enOverlayIndex '= TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
		Dim iOptionIndex As Integer = -1
		Dim bTopoPurpose As Boolean = False
		Dim bAllArea As Boolean = False

		'	Dim oaOptionValues(2) As System.Object
		Dim oaMultiOptionValues()() As System.Object = Nothing
		Dim iSelectedRegionNo As Integer
		Dim bRegion As Boolean
		Dim bEntirety As Boolean

		Dim sTopoPurposeText As String = Nothing
		Dim iOptionValuesUB As Integer
		If TopoManager.TPlanGraph.TplnProject.Regions.Count > 1 Then
			iOptionValuesUB = 2
		ElseIf TopoManager.TPlanGraph.TplnProject.MerhavExists Then
			iOptionValuesUB = 2
		Else
			iOptionValuesUB = 1
		End If
		DMCommon.Debug.MsgBox("!MerhavExists", TopoManager.TPlanGraph.TplnProject.MerhavExists)




		Dim oaOptionValues(iOptionValuesUB) As System.Object
		Dim dicOptionValues As Dictionary(Of Integer, Object) = New Dictionary(Of Integer, Object)()

		'		miCurrentRegionNo = zzGetSelectedRegionNo()
		zzSetSelectedRegion()

		If moSelectedReportItem IsNot Nothing Then
			'iSelectedRegionNo = zzGetSelectedRegion()
			iSelectedRegionNo = miCurrentRegionNo
			If iSelectedRegionNo = 0 Then
				bRegion = False
			ElseIf iSelectedRegionNo = miCurrentRegionNo Then
				bRegion = True
			End If
			'DMCommon.Debug.MsgBox("!iSelectedRegion", iSelectedRegionNo, bRegion)

			Select Case moSelectedReportItem.TopoPurpose
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
				dicOptionValues.Add(0, sTopoPurposeText)
			End If

			If (moSelectedReportItem.Options And enReportOptions.AllAreaOptionsEnabled) <> 0 Then
				iOptionIndex += 1
				bAllArea = True
			End If

			Me.Cursor = Cursors.WaitCursor
			If Me.lstReports.SelectedItem IsNot Nothing Then

				'DMCommon.Debug.MsgBox("!moSelectedReportItem", moSelectedReportItem.ListDispData, moSelectedReportItem.RepIndex)
				If Me.grbCalcOption.Enabled Then
					If Me.rdbAcad.Checked Then
						iDataOption = TPlanGraph.enDataOptions.AcadArea
						sAreaOptionText = Me.rdbAcad.Text
						oaOptionValues(iOptionIndex) = Me.rdbAcad.Text
					ElseIf Me.rdbCalc.Checked Then
						iDataOption = TPlanGraph.enDataOptions.CalcMergeArea
						sAreaOptionText = Me.rdbCalc.Text

						oaOptionValues(iOptionIndex) = Me.rdbCalc.Text
					ElseIf Me.rdbCalc2.Checked Then
						iDataOption = TPlanGraph.enDataOptions.CalcMergeArea2
						sAreaOptionText = Me.rdbCalc2.Text
						oaOptionValues(iOptionIndex) = Me.rdbCalc2.Text
					ElseIf Me.rdbRounded.Checked Then
						iDataOption = TPlanGraph.enDataOptions.RoundedArea
						sAreaOptionText = Me.rdbRounded.Text
						oaOptionValues(iOptionIndex) = Me.rdbRounded.Text
					ElseIf Me.rdbCalcRounded.Checked Then
						iDataOption = TPlanGraph.enDataOptions.CalcRoundedArea
						sAreaOptionText = Me.rdbRounded.Text
						oaOptionValues(iOptionIndex) = Me.rdbCalcRounded.Text
					Else
						iDataOption = TPlanGraph.enDataOptions.Default
					End If
				End If

				DMCommon.Debug.MsgBox("!frmreports", iDataOption, TPlanGraph.enDataOptions.CalcRoundedArea, iOptionValuesUB, oaOptionValues.GetUpperBound(0), iOptionIndex, sAreaOptionText)

				If iDataOption <> TPlanGraph.enDataOptions.CalcRoundedArea AndAlso iOptionIndex >= 0 Then
					oaOptionValues(iOptionIndex) = sAreaOptionText
					dicOptionValues.Add(1, sAreaOptionText)
				End If
				If TopoManager.TPlanGraph.TplnProject.Regions.Count > 1 Then

					iOptionIndex += 1
					oaOptionValues(iOptionIndex) = msCurrentRegionName
					dicOptionValues.Add(2, msCurrentRegionName)
				End If


				If Me.grbOverlayOption.Enabled Then
					If Me.rdbMerge.Checked Then
						iOverlayMethod = DMAcadExt.enOverlayMethod.Merge
					ElseIf Me.rdbUnion.Checked Then
						iOverlayMethod = DMAcadExt.enOverlayMethod.Union
					ElseIf Me.rdbFDO.Checked Then
						iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
					Else
						iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined
					End If
				Else
					iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined
				End If
				'  MessageBox.Show(iOverlayMethod.ToString, "06_200") 
				If iOverlayMethod <> DMAcadExt.enOverlayMethod.Undefined Then
					iOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
				End If
			End If
			Dim oDataView As DataView = Nothing
			Dim oaDataView() As DataView = Nothing
			Dim saModData() As String = Nothing
			Dim iaDataColumns() As Integer = Nothing
			Dim iGroupColumnStart As Integer = -1
			Dim iGroupColumnUB As Integer = 0


			Dim oaCaptions() As System.Object = Nothing
			Dim oaTotals() As System.Object = Nothing

			If oReport.AcadModel Then
				AcadReport.RepApp.InitDWGScaleFactor()
			End If
			If miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn Then
				bEntirety = True
			End If

			DMCommon.Debug.ExcelLog.SetEnumerable(0, "!OptionVal", oaOptionValues)
			'	DMCommon.Debug.MsgBox("Rep!!", moSelectedReportItem.RepIndex, TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock, miCurrentRegionNo)
			Select Case moSelectedReportItem.RepIndex
				Case TPlServerDB.enResourceTheme.AcRepContent
					'  MessageBox.Show(moSelectedReportItem.ToString() & vbCrLf & moSelectedReportItem.ReportModification.ToString(), "01_443b")
					'Dim iOverlayIndex As DMAcadExt.enOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
					If moSelectedReportItem.ReportModification = enReportModifications.Default Then
						If miCurrentRegionNo = 0 OrElse bEntirety Then
							'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''oDataView = TPlanGraph.TplnParcel.BlockView(iOverlayIndex)

						Else
							''''''''''''TPlanGraph.TplnParcel.CalculateRegionBlocks(iOverlayIndex, miCurrentRegionNo)
							'''''''''''''''''''''''''oDataView = TPlanGraph.TplnParcel.BlockRegionView
						End If
						TPlanGraph.TplnProject.GetInPlanParcelByGush(moSelectedReportItem.TopoPurpose, bEntirety, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
						' frmReports.vb:line 520
					ElseIf moSelectedReportItem.ReportModification = enReportModifications.Multi Then


						Dim iMerhavIndex As Integer = 0

						ReDim oaDataView(TPlanGraph.TplnProject.MerhavDic.Count - 1)
						ReDim oaOptionValues(TPlanGraph.TplnProject.MerhavDic.Count - 1)
						ReDim oaMultiOptionValues(TPlanGraph.TplnProject.MerhavDic.Count - 1)
						Dim oaValue(0) As System.Object
						For Each oMerhav As TPlanGraph.TplnMerhav In TPlanGraph.TplnProject.MerhavDic.Values
							If oMerhav IsNot Nothing Then
								oMerhav.CalculateBlocks(iOverlayIndex)
								oaDataView(iMerhavIndex) = oMerhav.BlockView(iOverlayIndex)


								oaValue(0) = oMerhav.Name
								ReDim oaMultiOptionValues(iMerhavIndex)(0)
								oaMultiOptionValues(iMerhavIndex)(0) = oMerhav.Name
								oaOptionValues(iMerhavIndex) = "oMerhav.Name"
							Else
								MessageBox.Show("Merhav was not found" & vbCrLf & CStr(" Is Nothing"), "01_355s")
							End If
							iMerhavIndex += 1
						Next


					ElseIf moSelectedReportItem.ReportModification = enReportModifications.Multi Then


						Dim oMerhav As TPlanGraph.TplnMerhav = TPlanGraph.TplnProject.GetMerhav(553)
						If oMerhav IsNot Nothing Then
							oMerhav.CalculateBlocks(iOverlayIndex)
							oDataView = oMerhav.BlockView(iOverlayIndex)
						Else
							MessageBox.Show("Merhav was not found" & vbCrLf & CStr(" Is Nothing"), "01_355s")
						End If
					End If

					' frmReports.vb:line 531

					'053 621 2985

				Case TPlServerDB.enResourceTheme.AcRepPlanParcels
					If TopoManager.TPlanGraph.TplnProject.Regions IsNot Nothing AndAlso TopoManager.TPlanGraph.TplnProject.Regions.Count > 0 Then
						TPlanGraph.TplnProject.GetInPlanDataNew(moSelectedReportItem.TopoPurpose, bEntirety, True, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, False, oDataView, iaDataColumns, oaTotals)
					Else
						DMCommon.Debug.MsgBox("171124_2", moSelectedReportItem.TopoPurpose, miCurrentRegionNo, oDataView.Count)
						TPlanGraph.TplnProject.GetInPlanDataNew(moSelectedReportItem.TopoPurpose, True, True, iDataOption, iOverlayMethod, miCurrentRegionNo, Nothing, False, oDataView, iaDataColumns, oaTotals)
					End If
					'	TPlanGraph.TplnProject.GetInPlanDataNew(moSelectedReportItem.TopoPurpose, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo, oDataView, iaDataColumns, oaTotals)
					'	TPlanGraph.TplnProject.GetInPlanDataNew(moSelectedReportItem.TopoPurpose, True, iDataOption, iOverlayMethod, miCurrentRegionNo, oDataView, iaDataColumns, oaTotals)

				Case TPlServerDB.enResourceTheme.AcRepLotsK
					DMCommon.Debug.MsgBox("!ParcelApprMapThemeData", TopoManager.TPlanGraph.TplnProject.ParcelApprMapThemeData, TopoManager.TPlanGraph.TplnProject.ParcelApprMapThemeData.IsNotEmpty, miCurrentRegionNo)
					If TopoManager.TPlanGraph.TplnProject.ParcelApprMapThemeData.IsNotEmpty Then
						TPlanGraph.TplnProject.GetInPlanDataByLot(moSelectedReportItem.TopoPurpose, bEntirety, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
					Else
						TPlanGraph.TplnLot.GetMainData(moSelectedReportItem.TopoPurpose, iDataOption, bEntirety, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
						DMCommon.Debug.ExcelLog.SetEnumerable(0, "!DataColumns", iaDataColumns)
						DMCommon.Debug.ExcelLog.SetDataTable(0, "MainData", oDataView)
					End If

				Case TPlServerDB.enResourceTheme.AcRepLotsM
					'	TPlanGraph.TplnLot.GetMainData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, oDataView, iaDataColumns, oaTotals)
					oDataView = TopoManager.TPlanGraph.TplnProject.OverlayGroupView(iOverlayIndex)
					TPlanGraph.TplnProject.GetOverlayData(iOverlayIndex, iDataOption, iOverlayMethod, iSelectedRegionNo, oDataView, iaDataColumns, oaTotals)
					TPlanGraph.TplnProject.GetLotData(moSelectedReportItem.TopoPurpose, bEntirety, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)


				Case TPlServerDB.enResourceTheme.AcRepParcelLuseK

					TPlanGraph.TplnProject.GetLanduseDataNew(moSelectedReportItem.TopoPurpose, bEntirety, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
					'TPlanGraph.TplnParcel.GetLanduseData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegionNo, oDataView, iaDataColumns, oaTotals)

				Case TPlServerDB.enResourceTheme.AcRepParcelLuseCol

					Select Case TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose
						Case DMAcadExt.enTopoPurpose.Approved
							TPlanGraph.TplnParcel.GetLanduseDataByCol(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals, iGroupColumnUB, oaCaptions)
							DMCommon.Functions.DispArray("oaCaptions_0", oaCaptions, True)
						Case DMAcadExt.enTopoPurpose.Expro
							If moSelectedReportItem.ReportModification = enReportModifications.Default Then
								TPlanGraph.TplnParcel.GetExproDataByCol(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegionNo, oDataView, iaDataColumns, oaTotals, iGroupColumnUB, oaCaptions)
							ElseIf moSelectedReportItem.ReportModification = enReportModifications.Additional Then
								TPlanGraph.TplnParcel.GetExproDataByColExt(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegionNo, oDataView, iaDataColumns, oaTotals, iGroupColumnUB, oaCaptions)

							End If
					End Select

					' oDataView = TPlanGraph.TplnOwner.GetView()
					'   DMCommon.Debug.MsgBox("12_641", oDataView.Count, iaDataColumns.GetUpperBound(0))
					If oDataView IsNot Nothing Then

						iGroupColumnStart = 0
					End If
														'  oaCaptions = TPlanGraph.TplnOwner.GetCaptions(True)
				Case TPlServerDB.enResourceTheme.AcRepParcelLuseM
					TPlanGraph.TplnParcel.GetLanduseData(DMAcadExt.enTopoPurpose.Proposed, iDataOption, iOverlayMethod, iSelectedRegionNo, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepSumLuse
					If TopoManager.TPlanGraph.TplnProject.ParcelApprMapThemeData.IsNotEmpty Then
						TPlanGraph.TplnProject.GetLanduseDataNewNew(moSelectedReportItem.TopoPurpose, iOverlayMethod, iDataOption, bEntirety, iSelectedRegionNo, mhsCurrentRegions, oDataView, oaTotals)
					Else
						TPlanGraph.TplnLot.GetLanduseDataNew(moSelectedReportItem.TopoPurpose, DMAcadExt.enOverlayMethod.Undefined, iDataOption, iSelectedRegionNo, oDataView, oaTotals)
						DMCommon.Debug.ExcelLog.SetDataTable(0, "!LanduseData", oDataView)
					End If
														'	
				Case TPlServerDB.enResourceTheme.AcRepJointLuse
					TPlanGraph.TplnLot.GetJointLanduseData(iOverlayMethod, iDataOption, oDataView, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel
					TPlanGraph.TplnProject.LotContentAreaByParcelNew(iOverlayIndex, bEntirety, iDataOption, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepLotContent
					TPlanGraph.TplnProject.GetInPlanDataByLotParcel(moSelectedReportItem.TopoPurpose, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)
					''''''''''''''''''''''''''''''''''''''''oDataView = TPlanGraph.TplnParcel.LotContentView
				Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock
					DMCommon.Debug.MsgBox("!!GetInPlanDataByLotParcel", moSelectedReportItem.TopoPurpose, iOverlayIndex, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo)
					TPlanGraph.TplnProject.GetInPlanDataByLotParcel(moSelectedReportItem.TopoPurpose, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)

					TPlanGraph.TplnProject.LotContentAreaView(DMAcadExt.enOverlayIndex.ApprMerge, True, oDataView, iaDataColumns)
				Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel
					TPlanGraph.TplnProject.LotContentAreaView(DMAcadExt.enOverlayIndex.ApprMerge, False, oDataView, iaDataColumns)
				Case TPlServerDB.enResourceTheme.AcRepLegendK

					If True Then
						TPlanGraph.TplnLot.GetLanduseList(moSelectedReportItem.TopoPurpose, oDataView, (Parameters.LegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor))
					End If

					'Case TPlServerDB.enResourceTheme.AcRepLegendM
					'TPlanGraph.TplnLot.GetLanduseList(DMAcadExt.enTopoPurpose.Proposed, oDataView, Parameters.LegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor * zzGetSelectedScale())
				Case TPlServerDB.enResourceTheme.AcRepLegalParcels

					'TPlanGraph.TplnParcel.GetInPlanData(DMAcadExt.enTopoPurpose.Approved, False, iDataOption, iOverlayMethod, bRegion, oDataView, iaDataColumns, oaTotals)
					TPlanGraph.TplnProject.GetInPlanDataNew(moSelectedReportItem.TopoPurpose, bEntirety, False, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, False, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock
					'	TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, moSelectedReportItem.TopoPurpose)
					DMCommon.Debug.MsgBox("!!GetInPlanDataByLotParcel", moSelectedReportItem.TopoPurpose, iOverlayIndex, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo)
					TPlanGraph.TplnProject.GetInPlanDataByLotParcel(moSelectedReportItem.TopoPurpose, miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn, iDataOption, iOverlayMethod, miCurrentRegionNo, mhsCurrentRegions, oDataView, iaDataColumns, oaTotals)

					TPlanGraph.TplnProject.LotContentByBlockAreaView(iOverlayIndex, iDataOption, oDataView, iaDataColumns, oaTotals)
				Case TPlServerDB.enResourceTheme.AcRepOwnership
					If mfOwnership Is Nothing Then
						mfOwnership = New frmOwnership()
					End If
					mfOwnership.GetDataByDB(iDataOption, oDataView, iaDataColumns, oaTotals)

				'	DMCommon.Debug.MsgBox("01_399P", DMCommon.Debug.ColCount(oDataView))
				Case TPlServerDB.enResourceTheme.AcRepOwnersSum

					If mfOwnership Is Nothing Then
						mfOwnership = New frmOwnership()
					End If
					If moSelectedReportItem.ReportModification = enReportModifications.Default Then
						mfOwnership.GetOwnersSumNew(oDataView, iaDataColumns, oaCaptions, oaTotals)
					ElseIf moSelectedReportItem.ReportModification = enReportModifications.Additional Then
						mfOwnership.GetStateOwnersSum(oDataView, iaDataColumns, oaCaptions, oaTotals)
					End If


					'DMCommon.Debug.MsgBox("01_399Q", DMCommon.Debug.ColCount(oDataView))
				Case TPlServerDB.enResourceTheme.AcRepOwners
					oDataView = TPlanGraph.TplnOwner.GetView()

					iGroupColumnStart = 0
					iGroupColumnUB = TPlanGraph.TplnOwner.OwnerPgonUB
					oaCaptions = TPlanGraph.TplnOwner.GetCaptions(True)
					' System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count) & vbCiBaseUB + iCaptionIndex + 1chkrLf & CStr(iGroupColumnUB), "02_000")
			End Select
			BaseReport.GroupColumnStart = iGroupColumnStart
			BaseReport.GroupColumnUB = iGroupColumnUB
			If Me.chkAreaMeter.Checked Then
				oReport.AreaMeter = True
			End If
			'	DMCommon.Debug.MsgBox("13_130d", oaCaptions)
			'	DMCommon.Debug.ExcelLog.SetDataTable(0, "outView", oDataView)
			If Me.chkMergeRows.Checked Then
				oReport.MergeRows = True
			End If
			If False And oDataView IsNot Nothing Then
				'''''''''''''''''	oRepApp.Open(doSelectedReportItem.RepIndex) obsolete
				oReport.MainView = oDataView

				DMCommon.Debug.MsgBox("01_385y", DMCommon.Debug.ColCount(oDataView))
				If iaDataColumns IsNot Nothing Then
					oReport.DataColumns = iaDataColumns
				End If
				If oaTotals IsNot Nothing Then
					oReport.Totals = oaTotals
				End If



				DMCommon.Debug.ExcelLog.SetEnumerable(0, "!A_OptionVal", oaOptionValues)
				'	If oaOptionValues(0) IsNot Nothing Then
				oReport.OptionValues = oaOptionValues
				oReport.OptionValuesDic = dicOptionValues

				'End If
				If oReport.AcadModel Then
					Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
					Dim bCurrentLayerOK As Boolean = True
					Try
						oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, True)
					Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
						DMAcadExt.AcadErrCode.ShowAcadError(oAcadEx, False, "frmReports - zzInsertReport_02")
					End Try
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, True)
					'System.Windows.Forms.MessageBox.Show(CStr(bCurrentLayerOK And False), "05_010")
					If bCurrentLayerOK And False Then

						Me.Hide()
						Common.SetAcadFocus()
						If oReport.Insert() Then
							AcadReport.RepApp.AcadTable.RecomputeTableBlock(True)
							''''''''''		DMAcadExt.AcadDocument.Regen()
							AcadReport.RepApp.AcadTable.Draw()
							AcadReport.RepApp.PaintCells()
						End If
						Me.Show()
					End If
					'  System.Windows.Forms.MessageBox.Show("", "05_040")
					DMAcadExt.AcadTransaction.CloseModelSpace()
					DMAcadExt.AcadTransaction.Terminate()
					If oDocLock IsNot Nothing Then
						oDocLock.Dispose()
						oDocLock = Nothing
					End If
				Else
					''''''''''oRepApp.Insert()
				End If
			End If
			'If oaCaptions IsNot Nothing Then
			'   DMCommon.Functions.DispArray(oaCaptions, "01_882", True)
			'   oReport.Captions = oaCaptions
			'Else
			'   MessageBox.Show("oaCaptions Is Nothing", "01_882no")
			'End If
			If oaOptionValues IsNot Nothing Then
				'   DMCommon.Functions.DispArray(oaOptionValues, "01_887b", True)
			End If
			'DMCommon.Debug.MsgBox("01_399M", moSelectedReportItem.ReportModification, moSelectedReportItem.TopoPurpose, oDataView IsNot Nothing)

			If moSelectedReportItem.ReportModification = enReportModifications.Multi Then
				If oaDataView IsNot Nothing Then
					zzInsertMultiReport(oaDataView, saModData, iaDataColumns, oaCaptions, oaTotals, oaMultiOptionValues, , iGroupColumnStart, iGroupColumnUB)
				End If

			ElseIf moSelectedReportItem.ReportModification = enReportModifications.Template Then
				DMCommon.Debug.ExcelLog.SetEnumerable(0, "befInsertREp", oaOptionValues)
				zzInsertReport(moSelectedReportItem.RepIndex, oaOptionValues, iaDataColumns)
			Else

				If oDataView IsNot Nothing Then
					'DMCommon.Functions.DispArray("13_130K", oaOptionValues, True)
					oaOptionValues = {"אאאא", "בבבב"}
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "1befInsertR", oaOptionValues)
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "2befInsertR", dicOptionValues.Values)

					zzInsertReport(oDataView, iaDataColumns, oaCaptions, oaTotals, oaOptionValues, dicOptionValues, , iGroupColumnStart, iGroupColumnUB)
				End If

			End If




		End If

		Me.Cursor = Cursors.Default
	End Sub

	Private Sub zzInsertReport(ByVal iRepIndex As TPlServerDB.enResourceTheme, ByVal oaOptionValues() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing)
		Dim oRepApp As AcadReport.BaseReport
		Dim bCurrentLayerOK As Boolean = True
		If mbAutocad Then
			oRepApp = New AcadReport.Report
		Else
			oRepApp = New ExcelReport.Report(False)
		End If
		If Me.chkAreaMeter.Checked Then
			oRepApp.AreaMeter = True
		End If
		If Me.chkMergeRows.Checked Then
			oRepApp.MergeRows = True
		End If

		If oRepApp.Open(moSelectedReportItem.RepIndex) Then

			If oaOptionValues(0) IsNot Nothing Then
				' DMCommon.Functions.DispArrayN(oaOptionValues, "oaOptionValues", True)
				oRepApp.OptionValues = oaOptionValues
			End If
			If iaEmptyColumns IsNot Nothing Then
				oRepApp.EmptyColumns = iaEmptyColumns
			End If


			'	oRepApp.GroupColumnStart = iGroupColumnStart

			If oRepApp.AcadModel Then
				DMAcadExt.AcadDocument.SaveVarCmdDia(0)
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				AcadReport.RepApp.InitDWGScaleFactor()
				AcadReport.RepApp.Init(False)
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

				AcadReport.RepApp.GetStartPoint()
				Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
				Dim taColorScheme() As DMAcadExt.ColorScheme
				Dim oReportApp As AcadReport.Report
				If bCurrentLayerOK Then
					Me.Hide()
					'    System.Windows.Forms.MessageBox.Show(oRepApp.GetType().ToString, "05_029")
					Common.SetAcadFocus()
					oRepApp.Insert()



					oReportApp = DirectCast(oRepApp, AcadReport.Report)
					'at AcadReport.Section.zzLoadCellProp() in D:\NetProjects2012\TopoSolution12\AcadReport\Section.vb:line 208
					'at AcadReport.BaseHeaderSection.Format() in D:\NetProjects2012\TopoSolution12\AcadReport\BaseHeaderSection.vb:line 50
					'at AcadReport.Report.zzLayout2009() in D:\NetProjects2012\TopoSolution12\AcadReport\Report.vb:line 187
					'at AcadReport.Report.Insert() in D:\NetProjects2012\TopoSolution12\AcadReport\Report.vb:line 88
					'at TopoUI.frmReports.zzInsertReport(DataView oDataView, Int32[] iaDataColumns, Object[] oaCaptions, Object[] oaTotals, Object[] oaOptionValues, Int32[] iaEmptyColumns, Int32 iGroupColumnStart, Int32 iGroupColumnUB) in D:\NetProjects2012\TopoSolution12\TopoUI\frmReports.vb:line 920
					'at TopoUI.frmReports.zzLaunchReport(BaseReport oReport) in D:\NetProjects2012\TopoSolution12\TopoUI\frmReports.vb:line 856



					If AcadReport.RepApp.AcadTable IsNot Nothing Then
						''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
						'''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
						'''''''''''''''''	AcadReport.Report.ReDrawTable()
						colaPoints = oReportApp.GetColorCells()
						doReportApp = oReportApp
						If colaPoints IsNot Nothing Then
							taColorScheme = oReportApp.GetColorScheme()
							Dim oPgon As SimplePgon


							For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
								oPgon = New SimplePgon(colaPoints(iIndex))
								oPgon.SetTagNum("Table", iIndex)
								If iIndex < 0 Then
									Dim s As String = ""
									For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colaPoints(iIndex)
										s &= DMAcadExt.TPlnPoint.DispPoint(tPoint) & vbCrLf
									Next

									DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "; " & s)
								End If
								DMAcadExt.AcadTransaction.OpenNewAnonymBlock()

								'	taColorScheme(iIndex).Scale = dRepBamashSharedScale * AcadReport.RepApp.DrawingScaleFactor
								oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, taColorScheme(iIndex), False)

								DMAcadExt.AcadTransaction.InsertNewBlock(False)
							Next
							doReportApp.ClearZebraCells()
						End If

					End If
					Me.Show()
					DMAcadExt.AcadTransaction.CloseModelSpace()
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
					DMAcadExt.AcadDocument.RestoreVarCmdDia()
					'''''''''''''''	zzPaintTable()

				End If
			Else
				Select Case iRepIndex
					Case TPlServerDB.enResourceTheme.AcRepExproLuse
						'	DMCommon.Debug.MsgBox("190530_1", iRepIndex)
						TopoManager.TPlanGraph.TplnProject.SetExproLuseReport()

				End Select

			End If
		End If

	End Sub
	Private Sub zzInsertReport(ByVal oDataView As DataView, ByVal iaDataColumns() As Integer, ByVal oaCaptions() As System.Object, ByVal oaTotals() As System.Object _
										, ByVal oaOptionValues() As System.Object, ByVal dicOptionValues As Dictionary(Of Integer, System.Object), Optional ByVal iaEmptyColumns() As Integer = Nothing, Optional ByVal iGroupColumnStart As Integer = -1, Optional ByVal iGroupColumnUB As Integer = 0)
		Dim oRepApp As AcadReport.BaseReport
		Dim bCurrentLayerOK As Boolean = True
		If mbAutocad Then
			oRepApp = New AcadReport.Report
		Else
			oRepApp = New ExcelReport.Report(False)
		End If
		If Me.chkAreaMeter.Checked Then
			oRepApp.AreaMeter = True
		End If
		If Me.chkMergeRows.Checked Then
			oRepApp.MergeRows = True
		End If

		If oRepApp.Open(moSelectedReportItem.RepIndex) Then
			oRepApp.MainView = oDataView
			If iaDataColumns IsNot Nothing Then
				oRepApp.DataColumns = iaDataColumns
			End If
			If iaDataColumns IsNot Nothing Then
				oRepApp.DataColumns = iaDataColumns
			End If
			If oaCaptions IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("13_130c", oaCaptions)
				oRepApp.Captions = oaCaptions
			End If
			If oaTotals IsNot Nothing Then
				oRepApp.Totals = oaTotals
			End If
			'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "InsertR", oaOptionValues)
			'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "InsertD", dicOptionValues.Values)

			If oaOptionValues(0) IsNot Nothing Then
				' DMCommon.Functions.DispArrayN(oaOptionValues, "oaOptionValues", True)
				oRepApp.OptionValues = oaOptionValues
			End If
			oRepApp.OptionValuesDic = dicOptionValues
			If iaEmptyColumns IsNot Nothing Then
				oRepApp.EmptyColumns = iaEmptyColumns
			End If


			'	oRepApp.GroupColumnStart = iGroupColumnStart

			If oRepApp.AcadModel Then
				DMAcadExt.AcadDocument.SaveVarCmdDia(0)
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				AcadReport.RepApp.InitDWGScaleFactor()
				AcadReport.RepApp.Init(False)
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

				AcadReport.RepApp.GetStartPoint()
				Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
				Dim taColorScheme() As DMAcadExt.ColorScheme
				Dim oReportApp As AcadReport.Report
				If bCurrentLayerOK Then
					Me.Hide()
					'    System.Windows.Forms.MessageBox.Show(oRepApp.GetType().ToString, "05_029")
					Common.SetAcadFocus()
					oRepApp.Insert()



					oReportApp = DirectCast(oRepApp, AcadReport.Report)
					'at AcadReport.Section.zzLoadCellProp() in D:\NetProjects2012\TopoSolution12\AcadReport\Section.vb:line 208
					'at AcadReport.BaseHeaderSection.Format() in D:\NetProjects2012\TopoSolution12\AcadReport\BaseHeaderSection.vb:line 50
					'at AcadReport.Report.zzLayout2009() in D:\NetProjects2012\TopoSolution12\AcadReport\Report.vb:line 187
					'at AcadReport.Report.Insert() in D:\NetProjects2012\TopoSolution12\AcadReport\Report.vb:line 88
					'at TopoUI.frmReports.zzInsertReport(DataView oDataView, Int32[] iaDataColumns, Object[] oaCaptions, Object[] oaTotals, Object[] oaOptionValues, Int32[] iaEmptyColumns, Int32 iGroupColumnStart, Int32 iGroupColumnUB) in D:\NetProjects2012\TopoSolution12\TopoUI\frmReports.vb:line 920
					'at TopoUI.frmReports.zzLaunchReport(BaseReport oReport) in D:\NetProjects2012\TopoSolution12\TopoUI\frmReports.vb:line 856



					If AcadReport.RepApp.AcadTable IsNot Nothing Then
						''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
						'''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
						'''''''''''''''''	AcadReport.Report.ReDrawTable()
						colaPoints = oReportApp.GetColorCells()
						doReportApp = oReportApp
						If colaPoints IsNot Nothing Then
							taColorScheme = oReportApp.GetColorScheme()
							Dim oPgon As SimplePgon

							'		Dim dRepBamashSharedScale As Double

							'		Try
							'dRepBamashSharedScale = Convert.ToDouble(Me.txtRepBamashSharedScale.Text)
							'	If dRepBamashSharedScale < 0.00001 Then
							'dRepBamashSharedScale = 1.0
							'	End If
							'	Catch oEx As Exception
							'dRepBamashSharedScale = 1.0
							'			End Try



							For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
								oPgon = New SimplePgon(colaPoints(iIndex))
								oPgon.SetTagNum("Table", iIndex)
								If iIndex < 0 Then
									Dim s As String = ""
									For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colaPoints(iIndex)
										s &= DMAcadExt.TPlnPoint.DispPoint(tPoint) & vbCrLf
									Next

									DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "; " & s)
								End If
								DMAcadExt.AcadTransaction.OpenNewAnonymBlock()

								'	taColorScheme(iIndex).Scale = dRepBamashSharedScale * AcadReport.RepApp.DrawingScaleFactor
								oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, taColorScheme(iIndex), False)

								DMAcadExt.AcadTransaction.InsertNewBlock(False)
							Next
							doReportApp.ClearZebraCells()
						End If

					End If
					Me.Show()
					DMAcadExt.AcadTransaction.CloseModelSpace()
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
					DMAcadExt.AcadDocument.RestoreVarCmdDia()
					'''''''''''''''	zzPaintTable()

				End If
			Else

				oRepApp.Insert()
			End If
		End If


	End Sub
	Private Sub zzInsertMultiReport(ByVal oaDataView() As DataView, ByVal saModData() As String, ByVal iaDataColumns() As Integer, ByVal oaCaptions() As System.Object, ByVal oaTotals() As System.Object, ByVal oaMultiOptionValues()() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing, Optional ByVal iGroupColumnStart As Integer = -1, Optional ByVal iGroupColumnUB As Integer = 0)
      If oaDataView IsNot Nothing Then
         Dim oRepApp As AcadReport.BaseReport


         Dim bCurrentLayerOK As Boolean = True
         If mbAutocad Then
            oRepApp = New AcadReport.Report
         Else
            oRepApp = New ExcelReport.Report(False)
         End If
         If Me.chkAreaMeter.Checked Then
            oRepApp.AreaMeter = True
         End If
			If Me.chkMergeRows.Checked Then
				oRepApp.MergeRows = True
			End If
			' System.Windows.Forms.MessageBox.Show(oRepApp.AcadModel.ToString(), "02_915")
			If oRepApp.Open(moSelectedReportItem.RepIndex) Then

            If iaDataColumns IsNot Nothing Then
               oRepApp.DataColumns = iaDataColumns
            End If
            If iaDataColumns IsNot Nothing Then
               oRepApp.DataColumns = iaDataColumns
            End If
            If oaCaptions IsNot Nothing Then
               oRepApp.Captions = oaCaptions
            End If
            If oaTotals IsNot Nothing Then
               oRepApp.Totals = oaTotals
            End If

				If iaEmptyColumns IsNot Nothing Then
               oRepApp.EmptyColumns = iaEmptyColumns
            End If
            '	oRepApp.GroupColumnStart = iGroupColumnStart
            If oRepApp.AcadModel Then
               DMAcadExt.AcadDocument.SaveVarCmdDia(0)
               DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
               DMAcadExt.AcadTransaction.Start()
               DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
               AcadReport.RepApp.InitDWGScaleFactor()
               AcadReport.RepApp.Init(True)
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

					' Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
					'Dim taColorScheme() As DMAcadExt.ColorScheme
					Dim oReportApp As AcadReport.Report
               If bCurrentLayerOK Then

                  Me.Hide()

                  Common.SetAcadFocus()
                  AcadReport.RepApp.GetStartPoint()
                  For iRepIndex As Integer = 0 To oaDataView.GetUpperBound(0)
                     oRepApp.MainView = oaDataView(iRepIndex)
                     oRepApp.OptionValues = oaMultiOptionValues(iRepIndex)
                     oRepApp.Insert()
                     AcadReport.RepApp.NextTable()
                  Next

                  '  frmReports.vb:line 880


                  oReportApp = DirectCast(oRepApp, AcadReport.Report)


               End If
               Me.Show()
               DMAcadExt.AcadTransaction.CloseModelSpace()
               DMAcadExt.AcadTransaction.Terminate()
               DMAcadExt.AcadDocument.Unlock()
               DMAcadExt.AcadDocument.RestoreVarCmdDia()
               '''''''''''''''	zzPaintTable()
            Else
               '  System.Windows.Forms.MessageBox.Show(CStr(oaDataView.Count), "02_987")
               For iRepIndex As Integer = 0 To oaDataView.GetUpperBound(0)
                  oRepApp.MainView = oaDataView(iRepIndex)
                  oRepApp.OptionValues = oaMultiOptionValues(iRepIndex)
                  oRepApp.Insert()
                  oRepApp.NextTable()
                  ' AcadReport.RepApp.NextTable(20.0)
               Next

            End If

         End If
      End If


   End Sub
   Private Sub cmdExpExcel_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExpExcel.Click
      '	MyBase.dbAutocad = False
      mbAutocad = False
      zzLaunchReport(New ExcelReport.Report(False))
   End Sub
   Private Function zzGetSelectedScale() As Double
      Dim sScale As String
      Dim dBaseScale As Double = 1000.0
      Dim dScale As Double = 0.0

      If Me.cmbReportScale.SelectedIndex >= 0 Then
         sScale = Me.cmbReportScale.SelectedItem.ToString()
         dScale = DMCommon.Functions.TextToScale(sScale, dBaseScale) / dBaseScale
      Else
         MessageBox.Show(Me.cmbReportScale.SelectedIndex.ToString(), "16_711")
      End If
      Return dScale
   End Function
   Protected Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
      Try
         Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, bReturnEmpty)
      Catch oEx As Exception
         Return String.Empty
      End Try

   End Function
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

	Private Shared Function GetReportName(ByVal sName As String, ByVal iResourceTheme As TPlServerDB.enResourceTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iModification As enReportModifications) As String
		If String.IsNullOrEmpty(sName) Then


			Dim iElement As Integer
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					iElement = 3
				Case DMAcadExt.enTopoPurpose.Proposed
					iElement = 4
				Case DMAcadExt.enTopoPurpose.Expro
					If iModification = enReportModifications.Default Then
						iElement = 5
					ElseIf iModification = enReportModifications.Additional Then
						iElement = 6
					End If

				Case Else
					iElement = 2
			End Select
			If iModification = enReportModifications.Multi Then
				iElement = 5
			ElseIf iModification = enReportModifications.Additional Then
				iElement = 6
			End If
			Dim iTest1 As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcRepLotContent
			Dim iTest2 As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcRepOwnership
			Dim iTest3 As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcRepAAAAAAAAAAAAA


			'DMCommon.Debug.MsgBox("01_333dX", iElement, iResourceTheme, CInt(iResourceTheme), iTopoPurpose, iModification, iTest1, iTest2, iTest3, iTest1.ToString(), iTest2.ToString(), iTest3.ToString())
			Return TPlServerDB.TextResource.GetText(iElement, iResourceTheme, 0)
		Else
			Return sName
		End If

	End Function
	Private Shared Function GetReportName(ByVal iResourceTheme As TPlServerDB.enResourceTheme) As String

      Return TPlServerDB.TextResource.GetText(0, iResourceTheme, 0)
   End Function

   Private Sub lstReports_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles lstReports.SelectedIndexChanged
      If Me.lstReports.SelectedItem Is Nothing Then
         moSelectedReportItem = Nothing
         Me.grbCalcOption.Enabled = False
         Me.grbOverlayOption.Enabled = False
      Else
         moSelectedReportItem = DirectCast(Me.lstReports.SelectedItem, ReportItem)
         Me.grbCalcOption.Enabled = ((moSelectedReportItem.Options And enReportOptions.AllAreaOptionsEnabled) = enReportOptions.AllAreaOptionsEnabled)
			Me.grbOverlayOption.Enabled = ((moSelectedReportItem.Options And enReportOptions.OverlayEnabled) = enReportOptions.OverlayEnabled)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!ReportItem", moSelectedReportItem.Options, moSelectedReportItem.ToString(), moSelectedReportItem.RepIndex, moSelectedReportItem.TopoPurpose)
			If ((moSelectedReportItem.Options And enReportOptions.CalcMergeArea2Dflt) = enReportOptions.CalcMergeArea2Dflt) Then

				If Me.rdbCalc2.Enabled Then
					Me.rdbCalc2.Checked = True
				End If
         Else
            If Me.rdbCalc.Enabled Then
               Me.rdbCalc.Checked = True
            End If
         End If

		End If
   End Sub
	Private Sub zzSetSelectedRegion()
		If Me.lstRegions.SelectedItem IsNot Nothing Then
			Dim oRegion As TPlanGraph.TplnRegion
			Dim oRegionSet As TPlanGraph.TplnRegionSet
			If Me.lstRegions.SelectedItem.GetType() Is GetType(TPlanGraph.TplnRegion) Then
				oRegion = DirectCast(Me.lstRegions.SelectedItem, TPlanGraph.TplnRegion)
				msCurrentRegionName = oRegion.RegionName
				miCurrentRegionNo = oRegion.RegionNo
				mhsCurrentRegions = Nothing
			ElseIf Me.lstRegions.SelectedItem.GetType() Is GetType(TPlanGraph.TplnRegionset) Then
				oRegionSet = DirectCast(Me.lstRegions.SelectedItem, TPlanGraph.TplnRegionSet)
				msCurrentRegionName = oRegionSet.Name
				miCurrentRegionNo = 0
				mhsCurrentRegions = oRegionSet.RegionSet
			End If
			'	DMCommon.Debug.MsgBox("01_337", oRegion, oRegion.RegionNo, oRegion.RegionName)
		'	DMCommon.Debug.MsgBox("01_336", Me.lstRegions.SelectedItem.GetType(), Me.lstRegions.SelectedItem.GetType() Is GetType(TPlanGraph.TplnRegion), Me.lstRegions.SelectedItem.GetType() Is GetType(TPlanGraph.TplnRegionSet))
			'oRegion = DirectCast(Me.lstRegions.SelectedItem, TPlanGraph.TplnRegion)
			'	msCurrentRegionName = oRegion.RegionName.GetType(),
			'	miCurrentRegionNo = oRegion.RegionNo
		Else
			miCurrentRegionNo = TPlanGraph.TplnRegion.RegionAllIn
		End If
	End Sub
	Private Function zzGetSelectedRegionNo() As Integer
		'If Me.clbRegions.SelectedItem IsNot Nothing Then
		'	Dim oRegion As TPlanGraph.TplnRegion
		'	oRegion = DirectCast(Me.clbRegions.SelectedItem, TPlanGraph.TplnRegion)
		'	Return oRegion.RegionNo
		'Else
		'	Return 0
		'End If
	End Function
	Private Function zzGetSelectedRegionIndex() As Integer
		'If Me.clbRegions.SelectedItem IsNot Nothing Then

		'	Return Me.clbRegions.SelectedIndex
		'Else
		'	Return 0
		'End If
	End Function
	Private Sub zzRecalculateForlistBox()








		'	miCurrentRegion = zzGetSelectedRegion()
		RaiseEvent Recalculate(miCurrentRegionNo)


	End Sub
	Private Sub zzAreaToBlock()
		Const dToDunam As Double = 0.001
		Dim sTopoName As String = "LotsK"
		Me.Cursor = Cursors.WaitCursor

		Dim tApprMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		Dim oTopology As Autodesk.Gis.Map.Topology.TopologyModel
		Dim oPgons As Autodesk.Gis.Map.Topology.PolygonCollection
		Dim oCentroidObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim oLot As TPlanGraph.TplnLot
		Dim oLotTopoID As Integer
		Dim tAreaSet As TPlanGraph.TplnAreaSet
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Approved
		Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
		Dim iOverlayIndex As DMAcadExt.enOverlayIndex
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod
		Dim sArea As String
		Dim dicNameValue As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		If frmPrjThemes.MapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tApprMapThemeData) Then
			sTopoName = tApprMapThemeData.TopoName
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		oTopology = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oTopology IsNot Nothing Then

			oPgons = oTopology.GetPolygons()
			If Me.grbOverlayOption.Enabled Then
				If Me.rdbMerge.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Merge
				ElseIf Me.rdbUnion.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Union
				ElseIf Me.rdbFDO.Checked Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
				Else
					iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined
				End If
			Else
				iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined
			End If

			If iOverlayMethod <> DMAcadExt.enOverlayMethod.Undefined Then
				iOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
			End If
			''DMCommon.Debug.ExcelLog.SetNextValue(0, "!Debug01", iOverlayMethod, iOverlayIndex, oPgons.Count)
			For Each oPgon As Autodesk.Gis.Map.Topology.Polygon In oPgons
				oCentroidObjID = oPgon.Entity
				oLotTopoID = oPgon.ID
				oLot = TPlanGraph.TplnProject.GetLot(iTopoPurpose, oLotTopoID)
				tAreaSet = oLot.AreaSet(iOverlayIndex)

				''''''''''''''''''''''''''''''
				If Me.rdbAcad.Checked Then
					sArea = FormatNumber(tAreaSet.AcadArea * dToDunam, 3)
				ElseIf Me.rdbCalc.Checked Then
					sArea = FormatNumber(tAreaSet.CalcArea * dToDunam, 3)
				ElseIf Me.rdbCalc2.Checked Then
					sArea = FormatNumber(tAreaSet.CalcArea2 * dToDunam, 3)
				ElseIf Me.rdbRounded.Checked Then
					sArea = FormatNumber(tAreaSet.RoundedArea * dToDunam, 3)
				Else
					sArea = String.Empty
				End If




				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Debug02", oLotTopoID, sArea)
				dicNameValue.Clear()
				dicNameValue.Add("AREA", sArea)
				DMAcadExt.AcadTransaction.UpdateAttribText(oCentroidObjID, dicNameValue)

			Next

			oTopology.Close()
		Else
			DMCommon.Debug.MsgBox("Tplanner", "Topology " & sTopoName & "' was not found")
		End If


		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default
	End Sub




	Private Sub zzRecalculate()





		Me.Cursor = Cursors.WaitCursor

		miCurrentRegionNo = zzGetSelectedRegionNo()
		miCurrentRegionIndex = zzGetSelectedRegionIndex()


		RaiseEvent Recalculate(miCurrentRegionNo)
		mbEventsEnabled = False
		'For iIndex As Integer = 0 To Me.clbRegions.Items.Count - 1
		'	If Me.clbRegions.GetItemChecked(iIndex) Then
		'		Me.clbRegions.SetItemChecked(iIndex, False)
		'	End If
		'Next
		'Me.clbRegions.SetItemChecked(miCurrentRegionIndex, True)
		mbEventsEnabled = True
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub frmReports_Shown(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
		End If
	End Sub

   Private Sub rdbCalc_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbCalc.CheckedChanged
      zzSetFirstBlueLineEnabled()
   End Sub
   Private Sub zzSetFirstBlueLineEnabled()
      Me.chkFirstBlueLine.Enabled = Me.rdbCalc.Checked OrElse Me.rdbCalc2.Checked
   End Sub

   Private Sub rdbCalc2_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbCalc2.CheckedChanged
      zzSetFirstBlueLineEnabled()
   End Sub

   Private Sub rdbAcad_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbAcad.CheckedChanged
      zzSetFirstBlueLineEnabled()
   End Sub

   Private Sub chkFirstBlueLine_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkFirstBlueLine.CheckedChanged
      TPlanGraph.TplnProject.FirstBlueLine = Me.chkFirstBlueLine.Checked
      '''''''''''''''''   zzRecalculate()


   End Sub



	Private Sub clbRegions_ItemCheck(oSender As System.Object, e As ItemCheckEventArgs) ' Handles clbRegions.ItemCheck
		If mbEventsEnabled Then
			e.NewValue = e.CurrentValue
		End If
	End Sub


	Private Sub cmdRegionSet_Click(oSender As System.Object, e As EventArgs) Handles cmdRegionSet.Click
		mfRegionSet = New frmRegionSet()

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, mfRegionSet)
		'mfRegionSet.Owner = Me
		'	mfRegionSet.Visible = True
	End Sub

	Private Sub zzRegionSetReload()
		zzClearRegionSet()
		zzLoadRegionSet()
	End Sub
	Private Sub mfRegionSet_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfRegionSet.FormClosed

		If mfRegionSet.Updated Then
			zzRegionSetReload()
		End If
	End Sub
End Class