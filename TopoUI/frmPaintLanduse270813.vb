Option Explicit On
Option Strict On
Public Class frmPaintLanduse270813
	Private Const miParamType As Integer = 4
	Private Const mdBaseScale As Double = 1000.0
	Private mtMapThemeData As DMAcadExt.MapThemeData
	Private mtBlueLineMapThemeData As DMAcadExt.MapThemeData
	'Private lblTopoExists As System.Windows.Forms.Label
	Protected doBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doLabelBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doCaptionBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))	'9.75
	Private Shared moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
#Region "ToolStrip_LanduseTopo"
	Private tsbLanduseCreateTopo As System.Windows.Forms.ToolStripButton
	Private tsbLanduseCheckTopo As System.Windows.Forms.ToolStripButton
	Private tsbLanduseDeleteTopo As System.Windows.Forms.ToolStripButton
	Private tsbLanduseShowTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents ddbLanduseLayers As System.Windows.Forms.ToolStripDropDownButton
	Private tsbLanduseGetStatistics As System.Windows.Forms.ToolStripButton
	Private tsbLanduseExec As System.Windows.Forms.ToolStripButton
	Private tsiLanduseThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiLanduseThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiLanduseAllVisible As System.Windows.Forms.ToolStripMenuItem
#End Region
#Region "ToolStrip_BlueLineTopo"
	Private tsbBlueLineCreateTopo As System.Windows.Forms.ToolStripButton
	Private tsbBlueLineCheckTopo As System.Windows.Forms.ToolStripButton
	Private tsbBlueLineDeleteTopo As System.Windows.Forms.ToolStripButton
	Private tsbBlueLineShowTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents ddbBlueLineLayers As System.Windows.Forms.ToolStripDropDownButton
	Private tsbBlueLineGetStatistics As System.Windows.Forms.ToolStripButton
	Private tsbBlueLineExec As System.Windows.Forms.ToolStripButton
	Private tsiBlueLineThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiBlueLineThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiBlueLineAllVisible As System.Windows.Forms.ToolStripMenuItem
#End Region
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private moParams As TPlServerDB.dmParams
	Private Shared mdicColorSchemes As IDictionary(Of Integer, DMAcadExt.ColorScheme)
	Private WithEvents mfEditColorSet As frmEditColorSet
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tBlueLineMapThemeData As DMAcadExt.MapThemeData)
		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode

		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		mtMapThemeData = tMapThemeData
		mtBlueLineMapThemeData = tBlueLineMapThemeData
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()

		Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName

		Me.lblSourceTopoName.Text = mtMapThemeData.LineTopoName
		Me.lblDissolveTopoName.Text = sLanduseTopoName
		Me.lblBlueLineTopoName.Text = mtBlueLineMapThemeData.LineTopoName


		mdicColorSchemes = New Dictionary(Of Integer, DMAcadExt.ColorScheme)
		zzLoadParams()

	End Sub
	Private Sub zzLoadParams()

		moParams = New TPlServerDB.dmParams(miProjectCode, miDetailNo, mtMapThemeData.MapThemeID, miParamType)
		Dim iPaintTopoOpt As Integer = moParams.GetIntValue(0)
		If iPaintTopoOpt = 0 Then
			Me.rdbDissolve.Checked = True
		ElseIf iPaintTopoOpt = 1 Then
			Me.rdbSource.Checked = True
		Else
			Me.rdbDissolve.Checked = True
		End If
		Dim dScale As Double = moParams.GetDblValue(1)
		Dim iScale As Integer = CInt(dScale)
		Dim sScale As String = "1:" & CStr(iScale)
		Try
			Me.cmbPaintScale.Text = sScale
		Catch oEx As Exception

		End Try
		Dim dLegendScale As Double = moParams.GetDblValue(2)
		Me.txtLegendPaintFactor.Text = CStr(dLegendScale)
	End Sub
	Private Sub zzSaveParams()
		Try
			Dim iPaintTopoOpt As Integer = -1 '= moParams.GetIntValue(0)
			If Me.rdbDissolve.Checked Then
				iPaintTopoOpt = 0
			ElseIf Me.rdbSource.Checked Then
				iPaintTopoOpt = 1
			End If
			moParams.SetValue(0, iPaintTopoOpt)

			Dim dScale As Double = zzGetSelectedScale()
			moParams.SetValue(1, dScale)
		Catch oEx As Exception

		End Try




		Dim dLegendScale As Double
		Try
			dLegendScale = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
			moParams.SetValue(2, dLegendScale)
		Catch oEx As Exception

		End Try
		Try
			moParams.Update()
		Catch ex As Exception

		End Try



	End Sub
	Private Sub zzFillPaintScale()
		Const sComText As String = "SELECT ID,Name FROM Scales"
		Me.cmbPaintScale.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Scales")
	End Sub

	Private Sub zzCheckSourceTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.LineTopoName, bLockDoc, bMsg)
		zzDispSourceTopoOK(tTopoRes)
	End Sub
	Private Sub zzCheckDissolveTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.DissolveTopoName, bLockDoc, bMsg)

		zzDispDissolveTopoOK(tTopoRes)
	End Sub
	Private Sub zzCheckBlueLineTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtBlueLineMapThemeData.LineTopoName, bLockDoc, bMsg)

		zzDispBlueLineTopoOK(tTopoRes)
	End Sub
	Private Sub zzDispSourceTopoOK(tTopoRes As DMAcadExt.TopoRes)
		Dim bOK As Boolean = tTopoRes.IsOK
		Me.lblSourceTopoExists.Visible = bOK
		Me.txtSourcePgonCount.Enabled = bOK
		With lblSourceTopoName
			If bOK Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With
		Me.txtSourcePgonCount.Text = Convert.ToString(tTopoRes.PgonCount)

	End Sub
	Private Sub zzDispSourceTopoExists(bExists As Boolean)
		Me.lblSourceTopoExists.Visible = bExists
		Me.txtSourcePgonCount.Enabled = bExists
		With lblSourceTopoName
			If bExists Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With

	End Sub

	Private Sub zzDispDissolveTopoOK(tTopoRes As DMAcadExt.TopoRes)
		Dim bOK As Boolean = tTopoRes.IsOK
		Me.lblDissolveTopoExists.Visible = bOK
		Me.txtDissolvePgonCount.Enabled = bOK
		With lblDissolveTopoName
			If bOK Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With
		Me.txtDissolvePgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
	End Sub
	Private Sub zzDispBlueLineTopoOK(tTopoRes As DMAcadExt.TopoRes)
		Dim bOK As Boolean = tTopoRes.IsOK
		Me.lblBlueLineTopoExists.Visible = bOK
		Me.txtBlueLinePgonCount.Enabled = bOK
		With lblBlueLineTopoName
			If bOK Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With
		Me.txtBlueLinePgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
	End Sub


	Private Sub zzDispDissolveTopoExists(bExists As Boolean)
		Me.lblDissolveTopoExists.Visible = bExists
		Me.txtDissolvePgonCount.Enabled = bExists
		With lblDissolveTopoName
			If bExists Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With

	End Sub
	Private Sub zzMyInitializeComponent()
		zzFillPaintScale()
		zzInitLanduseToolStrip()
		zzInitBlueLineToolStrip()
	End Sub
	Private Sub zzInitLanduseToolStrip()
		Me.tsbLanduseCreateTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbLanduseCheckTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbLanduseDeleteTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbLanduseShowTopo = New System.Windows.Forms.ToolStripButton
		Me.ddbLanduseLayers = New System.Windows.Forms.ToolStripDropDownButton

		Me.tsbLanduseGetStatistics = New System.Windows.Forms.ToolStripButton
		Me.tsbLanduseExec = New System.Windows.Forms.ToolStripButton

		Me.tsiLanduseThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiLanduseThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiLanduseAllVisible = New System.Windows.Forms.ToolStripMenuItem()

		'
		'tsbLanduseCreateTopo
		'
		With Me.tsbLanduseCreateTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbLanduseCreateTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
			'msCreateTopoText
		End With
		'
		'tsbLanduseCheckTopo
		'
		With Me.tsbLanduseCheckTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Validate1
			.Name = "tsbLanduseCheckTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
		End With
		'
		'tsbLanduseDeleteTopo
		'
		With tsbLanduseDeleteTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Delete
			.Name = "tsbLanduseDeleteTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrDeleteTopo
		End With
		'
		'tsbLanduseShowTopo
		'
		With tsbLanduseShowTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
			.Name = "tsbLanduseShowTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
		End With

		'
		'tsiLanduseThisTopoOnlyVisible
		'
		With Me.tsiLanduseThisTopoOnlyVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiLanduseThisTopoOnlyVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Only"
		End With
		'
		'tsiLanduseAllVisible
		'
		With Me.tsiLanduseAllVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiLanduseAllVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "All Visible"
		End With
		'
		'tsiLanduseThisTopoVisible
		'
		With Me.tsiLanduseThisTopoVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiLanduseThisTopoVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Visible"
		End With

		'
		'ddbLanduseLayers
		'
		With ddbLanduseLayers
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
			.Name = "ddbLanduseLayers"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoLayers
			.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiLanduseThisTopoOnlyVisible, Me.tsiLanduseThisTopoVisible, Me.tsiLanduseAllVisible})
		End With
		'
		'tsbLanduseGetStatistics
		'
		With Me.tsbLanduseGetStatistics
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.QuestionMark
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbLanduseGetStatistics"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoStatistics
		End With

		'
		'tstLanduseTopology
		'
		'	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		With Me.tstLanduseTopology
			.Dock = System.Windows.Forms.DockStyle.None

			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbLanduseGetStatistics, Me.tsbLanduseExec, Me.ddbLanduseLayers, Me.tsbLanduseShowTopo, Me.tsbLanduseDeleteTopo, Me.tsbLanduseCheckTopo, Me.tsbLanduseCreateTopo})
			', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
			'.Location = New System.Drawing.Point(300, 2)
			.Name = "tstLanduseTopology"
			'	.Size = New System.Drawing.Size(87, 25)
			.TabIndex = 0
		End With

	End Sub
	Private Sub zzCreateLanduseTopology()
		Dim sAttribExpr As String = mtMapThemeData.DissolveAttribExpr	 '"@CODE"
		Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		zzCreateDissolveTopology(sLanduseTopoName, sAttribExpr)
		Me.zzCheckDissolveTopo(False, False)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzCreateBlueLineTopology()
		Dim sAttribExpr As String = TopoManager.TPlanGraph.TplnLot.PlanDissolveAttribExpr	 '"@PLAN"
		Dim sLanduseTopoName As String = mtBlueLineMapThemeData.LineTopoName
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(mtBlueLineMapThemeData.LineLinkLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
		If bCurrentLayerOK Then
			zzCreateDissolveTopology(sLanduseTopoName, sAttribExpr)
		End If
		Me.zzCheckBlueLineTopo(False, False)
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzCreateDissolveTopology(sDissolveTopoName As String, sAttribExpr As String)
		Dim sLotTopoName As String = mtMapThemeData.LineTopoName
		'	Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName


		If TopoManager.TopoCreator.TopologyExists(sDissolveTopoName) Then
			System.Windows.Forms.MessageBox.Show("הטופולוגיה כבר קיימת", "21_400")
		Else


			TopoManager.TopoCreator.DissolveTopo(sLotTopoName, sAttribExpr, sDissolveTopoName)


		End If

	End Sub
	Private Sub zzThisOnlyVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		oEntity.Visible = bCond
	End Sub
	Private Sub zzThisVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		If bCond Then
			oEntity.Visible = True
		End If
	End Sub
	Private Sub zzLanduseOnlyVisible()
		Dim iLine, iCenter As Integer
		Dim sLotTopoName As String = mtMapThemeData.LineTopoName
		Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
		Dim oLanduseTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLanduseTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim resOBjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = oLanduseTopology.GetEntityIds()
		iLine = resOBjIDs.Count
		Dim oLotTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLotTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim colLotPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oLotTopology.GetPolygons()
		For Each oPgon As Autodesk.Gis.Map.Topology.Polygon In colLotPolygons
			resOBjIDs.Add(oPgon.Entity)
		Next
		iCenter = resOBjIDs.Count - iLine

		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)

		'	MessageBox.Show(CStr(iLine) & ":" & CStr(iCenter), "01_677")

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		'	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & sLineLinkLayers & vbCrLf & ptMapThemeData.CentroidBlocks & vbCrLf & ptMapThemeData.CentroidLayers, "02_102")
		DMAcadExt.AcadTransaction.ProcByList(dlEntityProc, resOBjIDs)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()


	End Sub
	Private Sub zzInitBlueLineToolStrip()
		Me.tsbBlueLineCreateTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbBlueLineCheckTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbBlueLineDeleteTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbBlueLineShowTopo = New System.Windows.Forms.ToolStripButton
		Me.ddbBlueLineLayers = New System.Windows.Forms.ToolStripDropDownButton

		Me.tsbBlueLineGetStatistics = New System.Windows.Forms.ToolStripButton
		Me.tsbBlueLineExec = New System.Windows.Forms.ToolStripButton

		Me.tsiBlueLineThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiBlueLineThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiBlueLineAllVisible = New System.Windows.Forms.ToolStripMenuItem()

		'
		'tsbBlueLineCreateTopo
		'
		With Me.tsbBlueLineCreateTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbBlueLineCreateTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
			'msCreateTopoText
		End With
		'
		'tsbBlueLineCheckTopo
		'
		With Me.tsbBlueLineCheckTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Validate1
			.Name = "tsbBlueLineCheckTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
		End With
		'
		'tsbBlueLineDeleteTopo
		'
		With tsbBlueLineDeleteTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Delete
			.Name = "tsbBlueLineDeleteTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrDeleteTopo
		End With
		'
		'tsbBlueLineShowTopo
		'
		With tsbBlueLineShowTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
			.Name = "tsbBlueLineShowTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
		End With

		'
		'tsiBlueLineThisTopoOnlyVisible
		'
		With Me.tsiBlueLineThisTopoOnlyVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiBlueLineThisTopoOnlyVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Only"
		End With
		'
		'tsiBlueLineAllVisible
		'
		With Me.tsiBlueLineAllVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiBlueLineAllVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "All Visible"
		End With
		'
		'tsiBlueLineThisTopoVisible
		'
		With Me.tsiBlueLineThisTopoVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiBlueLineThisTopoVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Visible"
		End With

		'
		'ddbBlueLineLayers
		'
		With ddbBlueLineLayers
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
			.Name = "ddbBlueLineLayers"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoLayers
			.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiBlueLineThisTopoOnlyVisible, Me.tsiBlueLineThisTopoVisible, Me.tsiBlueLineAllVisible})
		End With
		'
		'tsbBlueLineGetStatistics
		'
		With Me.tsbBlueLineGetStatistics
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.QuestionMark
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbBlueLineGetStatistics"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoStatistics
		End With



		'
		'tstBlueLineTopology
		'
		'	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		With Me.tstBlueLineTopology
			.Dock = System.Windows.Forms.DockStyle.None

			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbBlueLineGetStatistics, Me.tsbBlueLineExec, Me.ddbBlueLineLayers, Me.tsbBlueLineShowTopo, Me.tsbBlueLineDeleteTopo, Me.tsbBlueLineCheckTopo, Me.tsbBlueLineCreateTopo})
			', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
			'.Location = New System.Drawing.Point(300, 2)
			.Name = "tstBlueLineTopology"
			'	.Size = New System.Drawing.Size(87, 25)
			.TabIndex = 0
		End With

	End Sub
	Private Sub zzPaintByLanduse(sTopoName As String)
		Dim oTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopology.GetPolygons()
		Dim oPgon As TopoManager.TPlanGraph.TplnTopoPgon = Nothing
		For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
			'		oPgon = New TopoManager.TPlanGraph.TplnLusePgon(oPolygon,
		Next
	End Sub
	Private Sub cmdPaintByLanduse_Click(ByVal oSender As System.Object, ByVal oEventArgs As System.EventArgs) Handles cmdPaintByLanduse.Click
		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim dBaseScale As Double = 1000.0

		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim sPaintLayer As String
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose

		If iTopoPurpose <> DMAcadExt.enTopoPurpose.Undefined Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

			'	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_201")
			DMAcadExt.AcadDocument.SaveVarCmdDia(0S)
			''''''''	Dim sLotTopoName As String = mtMapThemeData.LineTopoName
			Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
			sPaintLayer = TopoManager.TPlanGraph.TplnProject.SetPaintLayers(iTopoPurpose)

			zzFillColorSchemesDic()
			If sPaintLayer IsNot Nothing Then

				If rdbDissolve.Checked Then
					''''''''''''''''''''''''''TopoManager.TPlanGraph.TplnProject.LoadLusePgons(iTopoPurpose, sLotTopoName, sLanduseTopoName)
					'MessageBox.Show(sPaintLayer, "04_254")
					zzPaintByLanduseTopo(iTopoPurpose, sPaintLayer)

				ElseIf rdbSource.Checked Then

					'MessageBox.Show(sPaintLayer, "04_254")
					'	TopoManager.TPlanGraph.TplnLot.PaintAll(mtMapThemeData.MapThemeID, iTopoPurpose, dScale, sPaintLayer)
					zzPaintAllLots(mtMapThemeData.MapThemeID, iTopoPurpose, sPaintLayer)
				End If
				Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
				If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
					moParams.SetIdData(sFileName)
					moParams.Update()
				End If


			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
			Me.Cursor = Cursors.Default
		End If
	End Sub

	Private Function zzGetSelectedScale() As Double
		Dim sScale As String

		Dim dScale As Double = 0.0
		Dim oDyn As System.Object
		If Me.cmbPaintScale.SelectedIndex >= 0 Then
			oDyn = Me.cmbPaintScale.SelectedItem
			If oDyn IsNot Nothing Then
				sScale = Me.cmbPaintScale.GetItemText(oDyn)
				dScale = DMCommon.Functions.TextToScale(sScale, mdBaseScale)
			End If
		Else
			MessageBox.Show(Me.cmbPaintScale.SelectedIndex.ToString(), "16_711")
		End If
		Return dScale
	End Function

	Private Sub zzPaintAllLots(iMapThemeID As DMAcadExt.enMapTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal sLayer As String)
		Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		'	Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
		Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
		Dim sLanduseNotFoundList As String = String.Empty
		If dicLots Is Nothing Then
			zzLoadGraph()
			dicLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
		End If
		If dicLots IsNot Nothing Then
			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
			For Each oLot As TopoManager.TPlanGraph.TplnLot In dicLots.Values
				'tColorScheme = mdicColorSchemes.Item(oLot.LanduseID)

				If mdicColorSchemes.TryGetValue(oLot.LanduseID, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
					If oLot.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayer) <> DMAcadExt.PaintException.OK Then
						Exit For
					End If
				Else
					If sLanduseNotFoundList.Length <> 0 Then
						sLanduseNotFoundList &= ","
					End If
					sLanduseNotFoundList &= CStr(oLot.LanduseID)
				End If



			Next
			'	MessageBox.Show(iPaintException.ToString(), "01_780")
			If sLanduseNotFoundList.Length <> 0 Then
				MessageBox.Show(sLanduseNotFoundList, "#187: Not Found")
			End If
		End If
	End Sub
	Private Sub zzPaintByLanduseTopo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal sLayer As String)
		Dim dicLusePgons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnLusePgon) = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
		Dim sLanduseNotFoundList As String = String.Empty
		'	MessageBox.Show(CStr(dicLusePgons Is Nothing), "01_213a")
		If dicLusePgons Is Nothing Then
			zzLoadGraph()
			dicLusePgons = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
		End If

		If dicLusePgons IsNot Nothing Then
			MessageBox.Show(CStr(dicLusePgons.Count) & ":" & CStr(mdicColorSchemes.Count), "04_500")
			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
			For Each oLusePgon As TopoManager.TPlanGraph.TplnLusePgon In dicLusePgons.Values
				'	tColorScheme = TopoManager.TPlanGraph.TplnLot.GetColorScheme(iTopoPurpose, oLusePgon.LanduseID, dScale)
				'	tColorScheme = mdicColorSchemes.Item(oLusePgon.LanduseID)
				If oLusePgon.LanduseID <> 0 Then
					If mdicColorSchemes.TryGetValue(oLusePgon.LanduseID, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
						oLusePgon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayer)
					Else
						If sLanduseNotFoundList.Length <> 0 Then
							sLanduseNotFoundList &= ","
						End If
						sLanduseNotFoundList &= CStr(oLusePgon.LanduseID)
					End If
				End If
			Next

			If sLanduseNotFoundList.Length <> 0 Then
				MessageBox.Show(sLanduseNotFoundList, "#188: Not Found")
			End If
		End If
	End Sub
	Public Sub GetLanduseData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef oDataView As System.Data.DataView, ByVal dColorSchemeScale As Double)
		Const msColorFieldName As String = "Color"
		Const msLanduseOrderFieldName As String = "LanduseOrder"
		Const msNameFieldName As String = "Name"
		Const sSPName As String = "GetColorSchemeSet"
		Const bFirstColorSchemeField As Integer = 3
		Dim oDataTable As System.Data.DataTable = New DataTable("LanduseList")
		Dim iLanduseOrder As Integer
		'Dim dicLusePgons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnLusePgon) = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
		Dim dicLanduses As TopoManager.TPlanGraph.TplnLanduses = TopoManager.TPlanGraph.TplnLot.Landuses

		If dicLanduses Is Nothing Then
			zzLoadGraphA()
			dicLanduses = TopoManager.TPlanGraph.TplnLot.Landuses
		End If
		'	MessageBox.Show(CStr(dicLanduses.Count) & vbCrLf & CStr(dColorSchemeScale) & vbCrLf & iTopoPurpose.ToString(), "01_328h")
		Dim s As String = ""



		With oDataTable.Columns
			.Add(msNameFieldName, GetType(System.String))	'System.Type.GetType("System.Decimal"
			.Add(msColorFieldName, GetType(DMAcadExt.ColorScheme))
			.Add(msLanduseOrderFieldName, GetType(System.Int32))
		End With
		Dim oNewRow As System.Data.DataRow
		Dim tColorScheme As DMAcadExt.ColorScheme



		Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
		Dim oaParams(2) As Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing
		'	MessageBox.Show(CStr(miProjectCode) & vbCrLf & CStr(miDetailNo) & vbCrLf & CStr(mtMapThemeData.MapThemeID), "01_331q")
		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, mtMapThemeData.MapThemeID)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
		Dim oLanduse As TopoManager.TPlanGraph.TplnLanduse = Nothing
		If oDataReader IsNot Nothing Then
			Dim iLanduseID, iColorSchemeID As Integer
			Dim sLanduseName As String
			Dim iSchemeOrder As Integer

			Do While oDataReader.Read
				iLanduseID = oDataReader.GetInt32(0)
				If dicLanduses.TryGetValue(iLanduseID, oLanduse) AndAlso oLanduse.HasLots(iTopoPurpose) Then
					If oDataReader.IsDBNull(bFirstColorSchemeField) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(bFirstColorSchemeField)
					End If
					If Not oDataReader.IsDBNull(2) Then
						iSchemeOrder = oDataReader.GetInt32(2)
					End If

					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
					Else
						sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
					End If

					If iColorSchemeID <> 0 Then
						tColorScheme = New DMAcadExt.ColorScheme(oDataReader, bFirstColorSchemeField, dScale)


						oNewRow = oDataTable.NewRow()
						oNewRow.Item(msNameFieldName) = tColorScheme.Name
						oNewRow.Item(msColorFieldName) = tColorScheme
						oNewRow.Item(msLanduseOrderFieldName) = iLanduseOrder
						oDataTable.Rows.Add(oNewRow)
					End If
				Else
					''''''''''''''	MessageBox.Show(CStr(iLanduseID) & ":" & CStr(iColorSchemeID), "01_334x")
				End If
			Loop
			oDataReader.Close()
		Else
			MessageBox.Show("", "01_173g")
		End If

		oDataView = New DataView(oDataTable)
		oDataView.Sort = msLanduseOrderFieldName

	End Sub
	Private Sub cmdEditColorSet_Click(oSender As System.Object, e As System.EventArgs) Handles cmdEditColorSet.Click
		mfEditColorSet = New frmEditColorSet(mtMapThemeData, 0)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditColorSet)
		Me.Visible = False
	End Sub
	Private Sub zzFillColorSchemesDic()
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
		MessageBox.Show("", "05_276")

		mdicColorSchemes = TopoManager.TPlanGraph.TplnLanduse.GetColorSchemeDic(iTopoPurpose)
		MessageBox.Show(CStr(mdicColorSchemes.Count) & vbCrLf & iTopoPurpose.ToString(), "05_281")
		'''''''''''''''''''''''''''''	TopoManager.TPlanGraph.TplnLanduse.FillColorSchemesDic(iTopoPurpose, mdicColorSchemes, dScale)
	End Sub
	Private Sub cmdClearPaint_Click(sender As System.Object, e As System.EventArgs) Handles cmdClearPaint.Click
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		Dim sPaintLayer As String = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef().Name
		'	MessageBox.Show(sPaintLayer, "04_360")
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		TopoManager.TPlanGraph.TplnProject.ClearPaint(iTopoPurpose)
		'	DMAcadExt.AcadTransaction.ClearLayerList(sPaintLayer)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub


	Private Sub cmdClose_Click(sender As System.Object, e As System.EventArgs) Handles cmdClose.Click
		Me.Close()
	End Sub


	Private Sub cmdColorSchemeEditor_Click(sender As System.Object, e As System.EventArgs) Handles cmdColorSchemeEditor.Click
		Dim fColorEditor As frmColorSchemeEditor = New frmColorSchemeEditor(enColorEditorMode.Landuse, False)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, fColorEditor)
	End Sub
	Private Sub tstLanduseTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstLanduseTopology.ItemClicked
		'	MessageBox.Show(e.ClickedItem.Name & vbCrLf & mtMapThemeData.DissolveTopoName, "01_098")


		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbLanduseCreateTopo.Name
				zzCreateLanduseTopology()

			Case Me.tsbLanduseCheckTopo.Name

				zzCheckDissolveTopo(True)

			Case Me.tsbLanduseDeleteTopo.Name
				zzDeleteLanduseTopo(True)
			Case Me.tsbLanduseExec.Name





		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub tstBlueLineTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstBlueLineTopology.ItemClicked
		'	MessageBox.Show(e.ClickedItem.Name & vbCrLf & mtMapThemeData.DissolveTopoName, "01_098")


		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbBlueLineCreateTopo.Name
				zzCreateBlueLineTopology()

			Case Me.tsbBlueLineCheckTopo.Name
				zzCheckBlueLineTopo(True)


			Case Me.tsbBlueLineDeleteTopo.Name
				zzDeleteBlueLineTopo(True)
			Case Me.tsbBlueLineExec.Name





		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzCheckDispTopo()
		zzCheckDissolveTopo(True)
		zzDispTopo()
	End Sub
	Private Sub zzCheckDissolveTopo(ByVal bMsgBox As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.DissolveTopoName, True, bMsgBox)
	End Sub
	Private Sub zzCheckBlueLineTopo(ByVal bMsgBox As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtBlueLineMapThemeData.LineTopoName, True, bMsgBox)
	End Sub
	Private Sub zzDispTopo()
		'	Me.txtPgonCount.Text = Convert.ToString(mtTopoRes.PgonCount)
		'	zzDispTopoExists(mtTopoRes.TopoExists)
	End Sub
	Private Sub zzDeleteLanduseTopo(ByVal bDeleteEntities As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

		TopoManager.TopoCreator.DeleteTopology(mtMapThemeData.DissolveTopoName, bDeleteEntities, False)
		zzCheckDissolveTopo(False, False)

		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzDeleteBlueLineTopo(ByVal bDeleteEntities As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

		TopoManager.TopoCreator.DeleteTopology(mtBlueLineMapThemeData.LineTopoName, bDeleteEntities, False)
		zzCheckBlueLineTopo(False, False)

		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub cmdRefresh_Click(oSender As System.Object, e As System.EventArgs) Handles cmdRefresh.Click
		Me.Cursor = Cursors.WaitCursor
		zzLoadGraphA()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub zzLoadGraph()
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
		TopoManager.TPlanGraph.TplnProject.LoadLotsNew(iTopoPurpose)

		TopoManager.TPlanGraph.TplnProject.LoadLusePgonsNew(mtMapThemeData)





	End Sub
	Private Sub zzLoadGraphA()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

		zzLoadGraph()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub cmdDrawLegend_Click(sender As System.Object, e As System.EventArgs) Handles cmdDrawLegend.Click
		Const sTopoApprText As String = "מצב קיים"
		Const sTopoPropText As String = "מצב מוצע"
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		Dim iaDataColumns() As Integer = Nothing
		Dim oaTotals() As System.Object = Nothing
		'	Dim oaOptionValues() As System.Object = Nothing
		Dim oaOptionValues(0) As System.Object

		Dim dLegendPaintFactor As Double
		Dim sTopoPurposeText As String
		Select Case iTopoPurpose
			Case DMAcadExt.enTopoPurpose.Approved
				sTopoPurposeText = sTopoApprText
			Case DMAcadExt.enTopoPurpose.Proposed
				sTopoPurposeText = sTopoPropText
			Case Else
				sTopoPurposeText = String.Empty
		End Select
		oaOptionValues(0) = sTopoPurposeText
		Try
			dLegendPaintFactor = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
		Catch ex As Exception
			dLegendPaintFactor = 1
		End Try

		Dim oDataView As DataView = Nothing

		GetLanduseData(iTopoPurpose, oDataView, (dLegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor))
		Dim oLaunchReport As LaunchReport = New LaunchReport(True, TPlServerDB.enResourceTheme.AcRepLegendK)
		oLaunchReport.PaintFactor = dLegendPaintFactor
		Me.Hide()
		oLaunchReport.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues)
		Me.Show()
	End Sub

	Private Sub ddbLayers_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbLanduseLayers.DropDownItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tsiLanduseThisTopoOnlyVisible.Name

				zzLanduseOnlyVisible()


			Case Me.tsiLanduseThisTopoVisible.Name

				'	zzSourceVisible(False)

			Case tsiLanduseAllVisible.Name
				zzAllVisible()

		End Select
	End Sub
	Private Sub zzAllVisible()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ProcAll(dlEntityProc, True)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub frmPaintLanduse_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		zzSaveParams()
	End Sub


	Private Sub mfEditColorSet_FormClosed(sender As Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfEditColorSet.FormClosed
		Me.Visible = True
	End Sub


	Private Sub cmdReadLog_Click(sender As System.Object, e As System.EventArgs) Handles cmdReadLog.Click
		Dim sLogName As String = """" & DMAcadExt.AcadDocument.LogName & """"
		'	Dim iID As Integer = Shell("""notepad"" -a -q", , True, 100000)
		Dim iID As Integer = Shell("notepad " & sLogName, AppWinStyle.NormalFocus, False, 100000)
		DMAcadExt.AcadDocument.WriteMessage("notepad " & sLogName)
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	Private Sub frmPaintLanduse_Load(sender As Object, e As System.EventArgs) Handles Me.Load

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		Dim bMsgBox As Boolean = False


		zzCheckSourceTopo(False, bMsgBox)

		zzCheckDissolveTopo(False, bMsgBox)
		zzCheckBlueLineTopo(False, bMsgBox)


		DMAcadExt.AcadDocument.Unlock()
	End Sub
End Class