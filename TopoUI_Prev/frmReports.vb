Option Explicit On
Option Strict On
Imports TopoManager
Imports AcadReport
Imports System.Data
Public Class frmReports

   Private miaTabaReports() As TPlServerDB.enResourceTheme = { _
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
   , TPlServerDB.enResourceTheme.AcRepSumLuse _
   , TPlServerDB.enResourceTheme.AcRepSumLuse _
   , TPlServerDB.enResourceTheme.AcRepJointLuse _
   , TPlServerDB.enResourceTheme.AcRepLegendK _
   , TPlServerDB.enResourceTheme.AcRepLegendK _
   , TPlServerDB.enResourceTheme.AcRepLegalParcels _
   , TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock}
	'	  , TPlServerDB.enResourceTheme.AcRepBlockArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContent _
	'	 , TPlServerDB.enResourceTheme.AcRepLotContentArea _
	' , TPlServerDB.enResourceTheme.AcRepLotContentAreaByParcel _
	Private miaOwnerReports() As TPlServerDB.enResourceTheme = { _
	 TPlServerDB.enResourceTheme.AcRepOwners}

   Private miaReportOptions() As enReportOptions = { _
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
   , enReportOptions.OverlayEnabled _
   , enReportOptions.OverlayEnabled _
   , enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled _
   , enReportOptions.AllAreaOptionsEnabled Or enReportOptions.OverlayEnabled}

   Private miaReportTopoPurpose() As DMAcadExt.enTopoPurpose = { _
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
   , DMAcadExt.enTopoPurpose.Approved _
   , DMAcadExt.enTopoPurpose.Proposed _
   , DMAcadExt.enTopoPurpose.Undefined _
   , DMAcadExt.enTopoPurpose.Approved _
   , DMAcadExt.enTopoPurpose.Proposed _
   , DMAcadExt.enTopoPurpose.Undefined _
   , DMAcadExt.enTopoPurpose.Approved}

   Private miaReportModification() As enReportModifications = { _
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
   , enReportModifications.Default _
   , enReportModifications.Default _
   , enReportModifications.Default _
   , enReportModifications.Default _
   , enReportModifications.Default _
   , enReportModifications.Default}
	Private WithEvents cmdInsertRep As Button
   Private WithEvents cmdExpExcel As Button
   Private WithEvents cmdRecalculate As Button


	Private WithEvents rdbAcad As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc As System.Windows.Forms.RadioButton
	Private WithEvents rdbCalc2 As System.Windows.Forms.RadioButton
   Private WithEvents rdbRounded As System.Windows.Forms.RadioButton
   Private WithEvents chkAreaMeter As System.Windows.Forms.CheckBox
   Private WithEvents chkFirstBlueLine As System.Windows.Forms.CheckBox

	Private WithEvents rdbMerge As System.Windows.Forms.RadioButton
	Private WithEvents rdbUnion As System.Windows.Forms.RadioButton
	Private WithEvents rdbFDO As System.Windows.Forms.RadioButton

	Private WithEvents lstReports As ListBox

	Private Shared diResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan


	Private grbCalcOption As System.Windows.Forms.GroupBox
	Private grbOverlayOption As System.Windows.Forms.GroupBox
   Private cmbReportScale As ComboBox
   Private lblReportScale As Label
   Private lblRegionList As Label
   Private lstRegions As ListBox
   Private miCurrentRegion As Integer = 0
	'''''''''''''''Private Shared diResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan

	Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTPlan

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
		zzMyInitializeComponent()
		' Add any initialization after the InitializeComponent() call.

	End Sub
	Private Sub zzMyInitializeComponent()
		Dim oReportItem As ReportItem
		Dim iReportsUB As Integer
		Select Case DMAcadExt.DMApp.AppID
			Case DMAcadExt.enApplications.Taba
				iReportsUB = miaTabaReports.GetUpperBound(0)
			Case DMAcadExt.enApplications.Ownership
				iReportsUB = miaOwnerReports.GetUpperBound(0)
		End Select
		Me.cmdInsertRep = New Button()
      Me.cmdExpExcel = New Button
      Me.cmdRecalculate = New Button
		Me.lstReports = New ListBox()

		Me.grbCalcOption = New System.Windows.Forms.GroupBox()
		Me.grbOverlayOption = New System.Windows.Forms.GroupBox()

      Me.cmbReportScale = New System.Windows.Forms.ComboBox()
      Me.lblReportScale = New System.Windows.Forms.Label()
      Me.lblRegionList = New System.Windows.Forms.Label()
      Me.lstRegions = New ListBox()

      Me.rdbAcad = New System.Windows.Forms.RadioButton()
      Me.rdbCalc = New System.Windows.Forms.RadioButton()
      Me.rdbCalc2 = New System.Windows.Forms.RadioButton()
      Me.rdbRounded = New System.Windows.Forms.RadioButton()

      Me.chkAreaMeter = New System.Windows.Forms.CheckBox()
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
      'cmdRecalculate
      '
      With Me.cmdRecalculate
         .Location = New System.Drawing.Point(360, 220)
         .Name = "cmdRecalculate"
         '.Enabled = False
         .Size = New System.Drawing.Size(64, 24)
         .TabIndex = 19
         '    .Font = moLabelFont
         .Text = "Calc"
      End With

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
			'	MessageBox.Show(CStr(miaReports.GetUpperBound(0)) & ":" & CStr(miaReportTopoPurpose.GetUpperBound(0)) & ":" & CStr(miaReportOptions.GetUpperBound(0)), "01_333")
			For iIndex As Integer = 0 To iReportsUB
				Select Case DMAcadExt.DMApp.AppID
					Case DMAcadExt.enApplications.Taba
                        oReportItem = New ReportItem(miaTabaReports(iIndex), GetReportName(miaTabaReports(iIndex), miaReportTopoPurpose(iIndex), miaReportModification(iIndex)), miaReportOptions(iIndex), miaReportTopoPurpose(iIndex), miaReportModification(iIndex), True, True, True)
					Case DMAcadExt.enApplications.Ownership
						oReportItem = New ReportItem(miaOwnerReports(iIndex), GetReportName(miaOwnerReports(iIndex)), enReportOptions.Default, True, True, True)
					Case Else
						Return
				End Select

				.Items.Add(oReportItem)
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
         .Location = New System.Drawing.Point(48, 152)
			.Name = "grbCalcOption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(132, 88)
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
      'chkAreaMeter
      '
      With Me.chkAreaMeter
         .Location = New System.Drawing.Point(48, 244)
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
         .Size = New System.Drawing.Size(80, 21)
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
         '	.Enabled = False
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(80, 126)
         .TabIndex = 14

         .Font = moLabelFont
         '  .ValueMember = DMCommon.ItemData.ValueMember
         '  .DisplayMember = DMCommon.ItemData.DisplayMember
         '	MessageBox.Show(CStr(miaReports.GetUpperBound(0)) & ":" & CStr(miaReportTopoPurpose.GetUpperBound(0)) & ":" & CStr(miaReportOptions.GetUpperBound(0)), "01_333")
         If TopoManager.TPlanGraph.TplnProject.Regions IsNot Nothing AndAlso TopoManager.TPlanGraph.TplnProject.Regions.Count > 1 Then
            For Each RegionID As Integer In TopoManager.TPlanGraph.TplnProject.Regions.Keys
               .Items.Add(RegionID)
            Next
         End If

      End With

      Me.Controls.Add(Me.lstRegions)


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
      Me.Controls.Add(Me.cmdRecalculate)


      '	ffffffffffff()



      Me.Location = New Point(200, 200)
      Select Case TopoManager.TPlanGraph.TplnProject.OverlayMethod
         Case DMAcadExt.enOverlayMethod.FDO_Overlay
            Me.rdbFDO.Checked = True
         Case DMAcadExt.enOverlayMethod.Merge
            Me.rdbMerge.Checked = True
      End Select
   End Sub

   Private Sub cmdInsertRep_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdInsertRep.Click
      Dim oRepApp As AcadReport.Report = New AcadReport.Report
      mbAutocad = True
      zzLaunchReport(oRepApp)
   End Sub
   Private Sub cmdRecalculate_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdRecalculate.Click
      zzRecalculate()
   End Sub
   Private Sub zzLaunchReport(ByVal oReport As AcadReport.BaseReport)
      Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.Default
      Dim iOverlayMethod As DMAcadExt.enOverlayMethod
      Dim iOverlayIndex As DMAcadExt.enOverlayIndex '= TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
      Dim iOptionIndex As Integer = -1
      Dim bTopoPurpose As Boolean = False
      Dim bAllArea As Boolean = False

      Dim oaOptionValues(1) As System.Object
      Dim oaMultiOptionValues()() As System.Object = Nothing
      Dim iSelectedRegion As Integer
      Dim bRegion As Boolean
      Dim sTopoPurposeText As String = Nothing
      '  DMCommon.ExcelLogF.Open()
      If moSelectedReportItem IsNot Nothing Then
         iSelectedRegion = zzGetSelectedRegion()
         If iSelectedRegion = 0 Then
            bRegion = False
         ElseIf iSelectedRegion = miCurrentRegion Then
            bRegion = True
         End If


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

         End If

         If (moSelectedReportItem.Options And enReportOptions.AllAreaOptionsEnabled) <> 0 Then
            iOptionIndex += 1
            bAllArea = True
         End If

         Me.Cursor = Cursors.WaitCursor
         If Me.lstReports.SelectedItem IsNot Nothing Then
            '	MessageBox.Show(moSelectedReportItem.ListDispData & vbCrLf & moSelectedReportItem.RepIndex.ToString(), "01_118")
            If Me.grbCalcOption.Enabled Then
               If Me.rdbAcad.Checked Then
                  iDataOption = TPlanGraph.enDataOptions.AcadArea

                  oaOptionValues(iOptionIndex) = Me.rdbAcad.Text
               ElseIf Me.rdbCalc.Checked Then
                  iDataOption = TPlanGraph.enDataOptions.CalcMergeArea
                  'rdbCalc.Enabled
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

            '   MessageBox.Show("AFTER", "06_300")
         End If
         Dim oDataView As DataView = Nothing
         Dim oaDataView() As DataView = Nothing
         Dim saModData() As String = Nothing
         Dim iaDataColumns() As Integer = Nothing
         Dim iGroupColumnStart As Integer = -1
         Dim iGroupColumnUB As Integer = 0


         Dim oaCaptions() As System.Object = Nothing
         Dim oaTotals() As System.Object = Nothing
         '    DMCommon.ExcelLogG.Open()
         If oReport.AcadModel Then
            AcadReport.RepApp.InitDWGScaleFactor()
         End If
         '  MessageBox.Show(moSelectedReportItem.RepIndex.ToString() & ":" & moSelectedReportItem.TopoPurpose.ToString() & ":" & iDataOption.ToString(), "01_666")
         '	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & iOverlayMethod.ToString() & vbCrLf & moSelectedReportItem.RepIndex.ToString() & vbCrLf & CStr(CInt(moSelectedReportItem.RepIndex)), "01_399d")
         Select Case moSelectedReportItem.RepIndex
            Case TPlServerDB.enResourceTheme.AcRepContent
               '  MessageBox.Show(moSelectedReportItem.ToString() & vbCrLf & moSelectedReportItem.ReportModification.ToString(), "01_443b")
               'Dim iOverlayIndex As DMAcadExt.enOverlayIndex = TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
               If moSelectedReportItem.ReportModification = enReportModifications.Default Then
                  If miCurrentRegion = 0 Then
                     oDataView = TPlanGraph.TplnParcel.BlockView(iOverlayIndex)
                  Else
                     oDataView = TPlanGraph.TplnParcel.BlockRegionView
                  End If

                  ' frmReports.vb:line 520
               ElseIf moSelectedReportItem.ReportModification = enReportModifications.Multi Then
                  '  DMCommon.Debug.MsgBox("09_881x", False, "Before")
                  '  DMCommon.Debug.MsgBox("09_882a", False, TPlanGraph.TplnProject.MerhavDic )
                  '  DMCommon.Debug.MsgBox("09_882b", False, TPlanGraph.TplnProject.MerhavDic, TPlanGraph.TplnProject.MerhavDic.Count)

                  Dim iMerhavIndex As Integer = 0

                  ReDim oaDataView(TPlanGraph.TplnProject.MerhavDic.Count - 1)
                  ReDim oaOptionValues(TPlanGraph.TplnProject.MerhavDic.Count - 1)
                  ReDim oaMultiOptionValues(TPlanGraph.TplnProject.MerhavDic.Count - 1)
                  Dim oaValue(0) As System.Object
                  For Each oMerhav As TPlanGraph.TplnMerhav In TPlanGraph.TplnProject.MerhavDic.Values
                     If oMerhav IsNot Nothing Then
                        oMerhav.CalculateBlocks(iOverlayIndex)
                        oaDataView(iMerhavIndex) = oMerhav.BlockView(iOverlayIndex)
                        '   DMCommon.ExcelLogG.SetDataTable(oaDataView(iMerhavIndex), 0)
                        '  DMCommon.ExcelLogG.SetNextValue(i, 1, "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----")

                        '  MessageBox.Show(oaDataView(iMerhavIndex).Count.ToString() & vbCrLf & oMerhav.Name & vbCrLf & iOverlayIndex.ToString(), "01_637q")
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

               If oDataView Is Nothing Then
                  '	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(" Is Nothing"), "01_322a")
               Else
                  'MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oDataView.Count), "01_322b")

               End If
               ' frmReports.vb:line 531

               '053 621 2985
            Case TPlServerDB.enResourceTheme.AcRepPlanParcels
               TPlanGraph.TplnParcel.GetInPlanData(moSelectedReportItem.TopoPurpose, True, iDataOption, iOverlayMethod, bRegion, oDataView, iaDataColumns, oaTotals)

            Case TPlServerDB.enResourceTheme.AcRepLotsK
               TPlanGraph.TplnLot.GetMainData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, miCurrentRegion, oDataView, iaDataColumns, oaTotals)

            Case TPlServerDB.enResourceTheme.AcRepLotsM
               '	TPlanGraph.TplnLot.GetMainData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, oDataView, iaDataColumns, oaTotals)
               oDataView = TopoManager.TPlanGraph.TplnProject.OverlayGroupView(iOverlayIndex)
               TPlanGraph.TplnProject.GetOverlayData(iOverlayIndex, iDataOption, iOverlayMethod, iSelectedRegion, oDataView, iaDataColumns, oaTotals)

               '  DMCommon.ExcelLogG.SetDataTable(oDataView, 0)
            Case TPlServerDB.enResourceTheme.AcRepParcelLuseK
               TPlanGraph.TplnParcel.GetLanduseData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegion, oDataView, iaDataColumns, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepParcelLuseCol
               TPlanGraph.TplnParcel.GetLanduseDataByCol(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, iSelectedRegion, oDataView, iaDataColumns, oaTotals, iGroupColumnUB, oaCaptions)
               ' oDataView = TPlanGraph.TplnOwner.GetView()
               If oDataView IsNot Nothing Then
                  '   DMCommon.ExcelLogD.SetDataTable(oDataView, 0, 0)
                  iGroupColumnStart = 0
               End If
              

               '  oaCaptions = TPlanGraph.TplnOwner.GetCaptions(True)

            Case TPlServerDB.enResourceTheme.AcRepParcelLuseM
               TPlanGraph.TplnParcel.GetLanduseData(DMAcadExt.enTopoPurpose.Proposed, iDataOption, iOverlayMethod, iSelectedRegion, oDataView, iaDataColumns, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepSumLuse
               TPlanGraph.TplnLot.GetLanduseDataNew(moSelectedReportItem.TopoPurpose, iOverlayMethod, iDataOption, iSelectedRegion, oDataView, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepJointLuse
               TPlanGraph.TplnLot.GetJointLanduseData(iOverlayMethod, iDataOption, oDataView, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepBlockArea
               oDataView = TPlanGraph.TplnProject.GetBlockLegalArea
            Case TPlServerDB.enResourceTheme.AcRepLotContent
               oDataView = TPlanGraph.TplnParcel.LotContentView
            Case TPlServerDB.enResourceTheme.AcRepLotContentArea
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
               '	System.Windows.Forms.MessageBox.Show(doSelectedReportItem.TopoPurpose.ToString(), "01_386j")
               TPlanGraph.TplnParcel.GetInPlanData(DMAcadExt.enTopoPurpose.Approved, False, iDataOption, iOverlayMethod, bRegion, oDataView, iaDataColumns, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepLotContentAreaByBlock
               '	TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, moSelectedReportItem.TopoPurpose)
               '	System.Windows.Forms.MessageBox.Show(doSelectedReportItem.TopoPurpose.ToString() & ":" & iOverlayIndex.ToString(), "01_386j")
               TPlanGraph.TplnProject.LotContentByBlockAreaView(iOverlayIndex, iDataOption, oDataView, iaDataColumns, oaTotals)
            Case TPlServerDB.enResourceTheme.AcRepOwners
               oDataView = TPlanGraph.TplnOwner.GetView()
               '  DMCommon.ExcelLogB.SetDataTable(oDataView, 0)
               iGroupColumnStart = 0
               iGroupColumnUB = TPlanGraph.TplnOwner.OwnerPgonUB
               oaCaptions = TPlanGraph.TplnOwner.GetCaptions(True)
               ' System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count) & vbCrLf & CStr(iGroupColumnUB), "02_000")
         End Select
         BaseReport.GroupColumnStart = iGroupColumnStart
         BaseReport.GroupColumnUB = iGroupColumnUB
         If Me.chkAreaMeter.Checked Then
            oReport.AreaMeter = True
         End If
         If False And oDataView IsNot Nothing Then

            '''''''''''''''''	oRepApp.Open(doSelectedReportItem.RepIndex) obsolete
            If False AndAlso iaDataColumns IsNot Nothing Then
               Dim sTest As String = String.Empty
               Dim oTestTable As DataTable = oDataView.Table

               For i As Integer = 0 To iaDataColumns.GetUpperBound(0)
                  sTest &= vbCrLf & CStr(iaDataColumns(i)) & ":" & oTestTable.Columns.Item(iaDataColumns(i)).ColumnName
               Next
               MessageBox.Show(sTest, "04_348t")
            End If


            oReport.MainView = oDataView
            System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count), "01_385y")
            If iaDataColumns IsNot Nothing Then
               oReport.DataColumns = iaDataColumns
            End If

            '	


            If oaTotals IsNot Nothing Then
               oReport.Totals = oaTotals
            End If

            If oaOptionValues(0) IsNot Nothing Then
               oReport.OptionValues = oaOptionValues
            End If
            


            System.Windows.Forms.MessageBox.Show(CStr(BaseReport.ColumnResUB) & ":" & CStr(BaseReport.GroupColumnUB) & ":" & CStr(BaseReport.ColumnUB), "19_301!")

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
               'System.Windows.Forms.MessageBox.Show(CStr(bCurrentLayerOK And False), "05_010")
               If bCurrentLayerOK And False Then
                  ' System.Windows.Forms.MessageBox.Show(CStr(999), "05_024")
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
         Else
            '  MessageBox.Show("Data was not found", "01_430")

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

         '  System.Windows.Forms.MessageBox.Show(moSelectedReportItem.ReportModification.ToString(), "05_028")
         If moSelectedReportItem.ReportModification = enReportModifications.Multi Then
            If oaDataView IsNot Nothing Then
               zzInsertMultiReport(oaDataView, saModData, iaDataColumns, oaCaptions, oaTotals, oaMultiOptionValues, , iGroupColumnStart, iGroupColumnUB)
            End If

         Else
            If oDataView IsNot Nothing Then

               zzInsertReport(oDataView, iaDataColumns, oaCaptions, oaTotals, oaOptionValues, , iGroupColumnStart, iGroupColumnUB)
            End If

         End If




      End If
      Me.Cursor = Cursors.Default
   End Sub
   Private Sub zzInsertReport(ByVal oDataView As DataView, ByVal iaDataColumns() As Integer, ByVal oaCaptions() As System.Object, ByVal oaTotals() As System.Object, ByVal oaOptionValues() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing, Optional ByVal iGroupColumnStart As Integer = -1, Optional ByVal iGroupColumnUB As Integer = 0)
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
      If oRepApp.Open(moSelectedReportItem.RepIndex) Then
         oRepApp.MainView = oDataView
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
            '  If oaOptionValues(0) IsNot Nothing Then
            'DMCommon.Functions.DispArray(oaOptionValues, "OptionValues", True)
            ' oRepApp.OptionValues = oaOptionValues
            '   End If
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

               Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
               Dim taColorScheme() As DMAcadExt.ColorScheme
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

   Private Shared Function GetReportName(ByVal iResourceTheme As TPlServerDB.enResourceTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iModification As enReportModifications) As String
      Dim iElement As Integer
      Select Case iTopoPurpose
         Case DMAcadExt.enTopoPurpose.Approved
            iElement = 3
         Case DMAcadExt.enTopoPurpose.Proposed
            iElement = 4
         Case Else
            iElement = 2
      End Select
      If iModification = enReportModifications.Multi Then
         iElement = 5
      End If
      Return TPlServerDB.TextResource.GetText(iElement, iResourceTheme, 0)
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
         If ((moSelectedReportItem.Options And enReportOptions.CalcMergeArea2Dflt) = enReportOptions.CalcMergeArea2Dflt) Then
            ' MessageBox.Show(moSelectedReportItem.Options.ToString(), "16_788")
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
   Private Function zzGetSelectedRegion() As Integer
      If Me.lstRegions.SelectedItem IsNot Nothing Then
         Return DirectCast(Me.lstRegions.SelectedItem, Integer)
      Else
         Return 0
      End If
   End Function
   Private Sub zzRecalculate()
      '  Dim iRegion As Integer = 0


      Dim bText As Boolean = Me.lstRegions.Text Is Nothing
      Dim sText As String = Me.lstRegions.Text
      Dim bSelectedValue As Boolean = Me.lstRegions.SelectedValue Is Nothing

      Dim bSelectedItem As Boolean = Me.lstRegions.SelectedItem Is Nothing






      ' ' MessageBox.Show(bText.ToString & vbCrLf & bSelectedValue.ToString & vbCrLf & bSelectedItem.ToString)
      '   MessageBox.Show(sText & vbCrLf & sSelectedItem & vbCrLf & bSelectedItem.ToString)
      miCurrentRegion = zzGetSelectedRegion()
      RaiseEvent Recalculate(miCurrentRegion)

   End Sub

   Private Sub frmReports_Shown(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Shown
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
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
End Class