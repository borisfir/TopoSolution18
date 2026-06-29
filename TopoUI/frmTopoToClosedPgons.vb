Option Explicit On
Option Strict On
Imports TopoManager
Imports System.Data
Public Class frmTopoToClosedPgons
	Private Const miParamType As Integer = 4
	Private Const mdBaseScale As Double = 1000.0

	Private Const msMarkBlockName As String = "1620"
	Private Const msMarkBlockLayerName As String = "1620"

	'	Private mtMapThemeData As DMAcadExt.MapThemeData
	Private mtSourceMapThemeData As DMAcadExt.MapThemeData
	Private mtDissolveMapThemeData As DMAcadExt.MapThemeData
	Private msSourceTopoName As String
	Private msDissolveTopoName As String
	Private mtSourceTopoRes As DMAcadExt.TopoRes
	Private mtSourceTopoWARes As DMAcadExt.TopoRes

	Private mtDissolveTopoRes As DMAcadExt.TopoRes

	Private mdicBlocks As TPlanGraph.TplnBlocks
	Private mdicRegions As TPlanGraph.TplnRegions

	Private moBlockTable As System.Data.DataTable
	Private moBlockView As System.Data.DataView
	Private mdTotalByPolygons As Double
	Private mdTotalByBorder As Double

	Private msaLayers() As String = Nothing
	Private mdaValues() As Double = Nothing


	Private WithEvents ctxBlock As System.Windows.Forms.DataGridViewTextBoxColumn
	Private WithEvents cchStatus As System.Windows.Forms.DataGridViewCheckBoxColumn
	Private WithEvents cchIsAnalytical As System.Windows.Forms.DataGridViewCheckBoxColumn

	Private WithEvents ctxMitham As System.Windows.Forms.DataGridViewTextBoxColumn
	Private WithEvents ctxMithamName As System.Windows.Forms.DataGridViewTextBoxColumn



	'Private lblTopoExists As System.Windows.Forms.Label
	Protected doBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doLabelBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Protected Shared doCaptionBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte)) '9.75
	Private Shared moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
#Region "ToolStrip_Topo"
	Private tsbCreateTopo As System.Windows.Forms.ToolStripButton
	Private tsbCheckTopo As System.Windows.Forms.ToolStripButton
	Private tsbDeleteTopo As System.Windows.Forms.ToolStripButton
	Private tsbShowTopo As System.Windows.Forms.ToolStripButton
	Private WithEvents ddbLayers As System.Windows.Forms.ToolStripDropDownButton
	Private tsbGetStatistics As System.Windows.Forms.ToolStripButton
	Private tsbExec As System.Windows.Forms.ToolStripButton
	Private tsiThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
	Private tsiAllVisible As System.Windows.Forms.ToolStripMenuItem
	'	Global.TopoUI.My.Resources.Resources.Layers16Tr
#End Region
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private moParams As TPlServerDB.dmParams
	Private Shared mdicColorSchemes As IDictionary(Of Integer, DMAcadExt.ColorScheme)
	'	Private WithEvents mfEditColorSet As frmEditColorSet
	Public Sub New(tSourceMapThemeData As DMAcadExt.MapThemeData, tDissolveMapThemeData As DMAcadExt.MapThemeData)
		mtSourceMapThemeData = tSourceMapThemeData
		mtDissolveMapThemeData = tDissolveMapThemeData

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode

		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		'		mtMapThemeData = tMapThemeData

		' This call is required by the designer.
		InitializeComponent()
		'  DMCommon.Debug.MsgBox("04_400", tSourceMapThemeData.TopoName, tDissolveMapThemeData.TopoName)

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
		'   DMCommon.Debug.MsgBox("12_500")

		If tDissolveMapThemeData.IsNotEmpty AndAlso tDissolveMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Blocks Then
			'  MessageBox.Show(tSourceMapThemeData.TopoName & ":" & tDissolveMapThemeData.TopoName, "04_401")
			TopoManager.TPlanGraph.TplnBlock.CreateBlockTable()
		End If
		'   DMCommon.Debug.MsgBox("12_500")
		If mtDissolveMapThemeData.MapThemeName IsNot Nothing Then
			Me.grbDissolveTopo.Text = mtDissolveMapThemeData.MapThemeName
		End If

		If False Then


			If mtSourceMapThemeData.IsNotEmpty Then
				If TopoManager.TopoCreator.TopologyExists(mtSourceMapThemeData.LineTopoName) Then
					msSourceTopoName = mtSourceMapThemeData.LineTopoName
				ElseIf TopoManager.TopoCreator.TopologyExists(mtSourceMapThemeData.TopoName) Then
					msSourceTopoName = mtSourceMapThemeData.TopoName
				End If

			End If

		End If
		'	MessageBox.Show(CStr(mtDissolveMapThemeData.IsNotEmpty) & ":" & tDissolveMapThemeData.TopoName, "04_406")
		If mtDissolveMapThemeData.IsNotEmpty Then
			msDissolveTopoName = mtDissolveMapThemeData.TopoName
		Else
			Me.grbDissolveTopo.Visible = False
			Me.dgvBlocks.Visible = False
			Me.cmdUpdateData.Visible = False

			Me.cmdZoom.Visible = False
			Me.cmbODType.Visible = False
			Me.cmdFillODTable.Visible = False
			Me.cmdExportShape.Visible = False
			Me.Size = New Size(Me.Size.Width, Me.Size.Height - 200)

			Me.cmdClose.Top = Me.grbDissolveTopo.Top
			Me.Height = 140
		End If
		zzLoadParams()
		Dim oMySettings As My.MySettings = New My.MySettings()

		Dim iLastODTypeIndex As Integer = oMySettings.LastODTypeIndex
		If iLastODTypeIndex >= 0 AndAlso iLastODTypeIndex < Me.cmbODType.Items.Count Then
			Me.cmbODType.SelectedIndex = iLastODTypeIndex
		End If
		'    DMCommon.Debug.MsgBox("12_510")
	End Sub


	Private Sub zzCheckClosedPgons()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

		zzCheckClosedPgonsA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub zzCheckClosedPgonsA()

		Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline
		Dim colDBObjects As Autodesk.AutoCAD.DatabaseServices.DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
		Dim iSourceCount As Integer = 0
		Dim iDissolveCount As Integer = 0
		Dim sSourceLayer As String = mtSourceMapThemeData.ClosedPgonsLayer
		Dim sDissolveLayer As String = mtDissolveMapThemeData.ClosedPgonsLayer
		Dim tBlockLayersList As DMCommon.dmList = New DMCommon.dmList(mtSourceMapThemeData.CentroidLayers)
		'	MessageBox.Show(CStr(colDBObjects.Count), "05_209")
		For Each oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject In colDBObjects

			If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
				Try
					oPolyline = DirectCast(oDBObject, Autodesk.AutoCAD.DatabaseServices.Polyline)
					If oPolyline.Closed Then
						If mtSourceMapThemeData.ClosedPgonsMethod = "BN" Then
							If tBlockLayersList.Contains(oPolyline.Layer) Then
								iSourceCount += 1
							End If
						Else
							Select Case oPolyline.Layer
								Case sSourceLayer
									iSourceCount += 1
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckClosedS", mtSourceMapThemeData.ClosedPgonsMethod, oPolyline.Closed, oPolyline.Layer, sSourceLayer, sDissolveLayer)
								Case sDissolveLayer
									iDissolveCount += 1
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckClosedD", mtSourceMapThemeData.ClosedPgonsMethod, oPolyline.Closed, oPolyline.Layer, sSourceLayer, sDissolveLayer)
							End Select
						End If

					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oDBObject.GetType().ToString(), "frmTopoToClosedPgons - zzCheckClosedPgons")
				End Try
			End If

		Next

		Me.lblSourceCPgonsExist.Visible = mtSourceTopoRes.IsOK AndAlso (mtSourceTopoRes.PgonCount <= iSourceCount)
		Me.lblDissolveCPgonsExist.Visible = mtDissolveTopoRes.IsOK AndAlso (mtDissolveTopoRes.PgonCount <= iDissolveCount)



		Me.txtSourcePLinesCount.Text = Convert.ToString(iSourceCount)
		Me.txtDissolvePLinesCount.Text = Convert.ToString(iDissolveCount)


	End Sub
	Private Sub zzLoadParams()


	End Sub
	Private Sub zzSaveParams()




	End Sub


	Private Sub zzCheckSourceTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
		mtSourceTopoRes = TopoManager.TopoCreator.CheckTopo(mtSourceMapThemeData.TopoName, bLockDoc, bMsg, mtSourceMapThemeData.MapThemeID)
		zzDispSourceTopoOK()
		mtSourceTopoWARes = TopoManager.TopoCreator.CheckTopo(mtSourceMapThemeData.LineTopoName, bLockDoc, bMsg, mtSourceMapThemeData.MapThemeID)
		zzDispSourceTopoWAOK()
	End Sub
	Private Sub zzCheckDissolveTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
		mtDissolveTopoRes = TopoManager.TopoCreator.CheckTopo(msDissolveTopoName, bLockDoc, bMsg, mtSourceMapThemeData.MapThemeID)

		zzDispDissolveTopoOK()
	End Sub
	Private Sub zzDispSourceTopoOK()
		Dim bOK As Boolean = mtSourceTopoRes.IsOK
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
		Me.txtSourcePgonCount.Text = Convert.ToString(mtSourceTopoRes.PgonCount)
		If bOK Then
			Me.rdbTopo.Enabled = True
			Me.rdbTopo.Checked = True
		Else
			Me.rdbTopo.Enabled = False
			Me.rdbTopo.Checked = False
		End If

	End Sub
	Private Sub zzDispSourceTopoWAOK()
		Dim bOK As Boolean = mtSourceTopoWARes.IsOK
		Me.lblSourceTopoWAExists.Visible = bOK
		Me.txtSourceWAPgonCount.Enabled = bOK
		With lblSourceTopoWAName
			If bOK Then
				.ForeColor = System.Drawing.SystemColors.ControlText
				.Font = doLabelBoldFont
			Else
				.ForeColor = System.Drawing.SystemColors.GrayText
				.Font = doLabelFont
			End If
		End With
		Me.txtSourceWAPgonCount.Text = Convert.ToString(mtSourceTopoRes.PgonCount)
		If bOK Then
			Me.rdbTopoWA.Enabled = True
			If Not Me.rdbTopo.Checked Then
				Me.rdbTopoWA.Checked = True
			End If
		Else
			Me.rdbTopoWA.Enabled = False
			Me.rdbTopoWA.Checked = False
		End If
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

	Private Sub zzDispDissolveTopoOK()
		Dim bOK As Boolean = mtDissolveTopoRes.IsInstance AndAlso mtDissolveTopoRes.IsOK
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
		Me.txtDissolvePgonCount.Text = Convert.ToString(mtDissolveTopoRes.PgonCount)
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

		zzInitToolStrip()
		If mtDissolveMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Blocks Then
			zzInsertBlockColumns()
		ElseIf mtDissolveMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Mitham Then
			zzInsertColumnMitham()
		End If
		grbSourceTopo.Text = mtSourceMapThemeData.MapThemeName

	End Sub
	Private Sub zzInitToolStrip()
		Me.tsbCreateTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbCheckTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbDeleteTopo = New System.Windows.Forms.ToolStripButton
		Me.tsbShowTopo = New System.Windows.Forms.ToolStripButton
		Me.ddbLayers = New System.Windows.Forms.ToolStripDropDownButton

		Me.tsbGetStatistics = New System.Windows.Forms.ToolStripButton
		Me.tsbExec = New System.Windows.Forms.ToolStripButton

		Me.tsiThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
		Me.tsiAllVisible = New System.Windows.Forms.ToolStripMenuItem()

		'
		'tsbCreateTopo
		'
		With Me.tsbCreateTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
			.Image = Global.TopoUI.My.Resources.Resources.CreateTopo
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbCreateTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
			'msCreateTopoText
		End With
		'
		'tsbCheckTopo
		'
		With Me.tsbCheckTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Validate1
			.Name = "tsbCheckTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
		End With
		'
		'tsbDeleteTopo
		'
		With tsbDeleteTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Delete
			.Name = "tsbDeleteTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrDeleteTopo
		End With
		'
		'tsbShowTopo
		'
		With tsbShowTopo
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.ShowTopo
			.Name = "tsbShowTopo"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
		End With

		'
		'tsiThisTopoOnlyVisible
		'
		With Me.tsiThisTopoOnlyVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiThisTopoOnlyVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Only"
		End With
		'
		'tsiAllVisible
		'
		With Me.tsiAllVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiAllVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "All Visible"
		End With
		'
		'tsiThisTopoVisible
		'
		With Me.tsiThisTopoVisible
			.CheckOnClick = False
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
			.Name = "tsiThisTopoVisible"
			.Size = New System.Drawing.Size(134, 22)
			.Text = "Visible"
		End With

		'
		'ddbLayers
		'
		With ddbLayers
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
			.Name = "ddbLayers"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoLayers
			.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiThisTopoOnlyVisible, Me.tsiThisTopoVisible, Me.tsiAllVisible})
		End With
		'
		'tsbGetState
		'
		With Me.tsbGetStatistics
			.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
			.Image = Global.TopoUI.My.Resources.Resources.QuestionMark
			.ImageTransparentColor = System.Drawing.Color.Magenta
			.Name = "tsbGetStatistics"
			.Size = New System.Drawing.Size(23, 22)
			.ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoStatistics
		End With
		If False Then
			'
			'tsbExec
			'
			With Me.tsbExec
				.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
				.Image = Global.TopoUI.My.Resources.Resources.Run15Tr
				.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
				.ImageTransparentColor = System.Drawing.Color.Magenta
				.Name = "tsbExec"
				.Size = New System.Drawing.Size(23, 22)
				''''''''''''	.ToolTipText = msExecText
			End With
		End If


		'
		'tstTopology
		'
		'	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		With Me.tstTopology
			.Dock = System.Windows.Forms.DockStyle.None

			.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbGetStatistics, Me.tsbExec, Me.ddbLayers, Me.tsbShowTopo, Me.tsbDeleteTopo, Me.tsbCheckTopo, Me.tsbCreateTopo})
			', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
			'.Location = New System.Drawing.Point(300, 2)
			.Name = "tstTopology"
			'	.Size = New System.Drawing.Size(87, 25)
			.TabIndex = 0
		End With

	End Sub
	Private Sub zzCreateDissolveTopology()
		Dim sAttribExpr As String = mtSourceMapThemeData.DissolveAttribExpr
		'	Dim sSourceTopoName As String = mtSourceMapThemeData.LineTopoName
		'	Dim sDissolveTopoName As String = mtDissolveMapThemeData.LineTopoName
		Dim bCurrentLayerOK As Boolean = False
		Dim sSourceTopoName As String
		If Me.rdbTopo.Checked Then
			sSourceTopoName = mtSourceMapThemeData.TopoName
		ElseIf Me.rdbTopoWA.Checked Then
			sSourceTopoName = mtSourceMapThemeData.LineTopoName
		Else
			Return

		End If
		'	System.Windows.Forms.MessageBox.Show(msDissolveTopoName, "21_444")
		If TopoManager.TopoCreator.TopologyExists(msDissolveTopoName) Then
			Dim sMsg As String = "הטופולוגיה '" & msDissolveTopoName & "' כבר קיימת"
			'	System.Windows.Forms.MessageBox.Show("הטופולוגיה כבר קיימת", "21_400")
			System.Windows.Forms.MessageBox.Show(sMsg, "21_401")
		Else
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			TopoManager.TPlanGraph.TplnBlock.Initialize(mtDissolveMapThemeData)
			DMCommon.Debug.MsgBox("06_008", sSourceTopoName, sAttribExpr, msDissolveTopoName, mtDissolveMapThemeData.ClosedPgonsLayer, mtDissolveMapThemeData.CentroidBlock, mtDissolveMapThemeData.CentroidLayer)
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(mtDissolveMapThemeData.ClosedPgonsLayer, DMAcadExt.DMApp.AppID, True, True)
			If bCurrentLayerOK Then
				'	System.Windows.Forms.MessageBox.Show(sSourceTopoName & vbCrLf & msDissolveTopoName & vbCrLf & mtDissolveMapThemeData.CentroidBlock & vbCrLf & mtDissolveMapThemeData.CentroidLayer, "21_445")
				TopoManager.TopoCreator.DissolveTopo(sSourceTopoName, sAttribExpr, msDissolveTopoName, mtDissolveMapThemeData.CentroidBlock, mtDissolveMapThemeData.CentroidLayer)
				'TopoManager.TopoCreator.DissolveTopo(sSourceTopoName, sAttribExpr, sDissolveTopoName)


				Me.zzCheckDissolveTopo(False, False)

				zzUpdateBlocksDic()
				If mdicBlocks IsNot Nothing Then
					zzFillBlocks(sSourceTopoName)
					zzFillGridA()
				End If


			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If



	End Sub

	Private Sub zzCreateDissolveTopologyNew()
		'  '  donepezil hcl
		'memorit
		'arisept
		'donepicil
		Dim sAttribExpr As String = mtSourceMapThemeData.DissolveAttribExpr
		'	Dim sSourceTopoName As String = mtSourceMapThemeData.LineTopoName
		'	Dim sDissolveTopoName As String = mtDissolveMapThemeData.LineTopoName
		Dim bCurrentLayerOK As Boolean = False
		Dim sSourceTopoName As String
		If Me.rdbTopo.Checked Then
			sSourceTopoName = mtSourceMapThemeData.TopoName
		ElseIf Me.rdbTopoWA.Checked Then
			sSourceTopoName = mtSourceMapThemeData.LineTopoName
		Else
			Return

		End If
		'   System.Windows.Forms.MessageBox.Show(msDissolveTopoName & vbCrLf & mtDissolveMapThemeData.MapThemeID.ToString(), "21_444")
		If TopoManager.TopoCreator.TopologyExists(msDissolveTopoName) Then
			Dim sMsg As String = "הטופולוגיה '" & msDissolveTopoName & "' כבר קיימת"
			'	System.Windows.Forms.MessageBox.Show("הטופולוגיה כבר קיימת", "21_400")
			System.Windows.Forms.MessageBox.Show(sMsg, "21_401")
		Else
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			TopoManager.TPlanGraph.TplnBlock.Initialize(mtDissolveMapThemeData)
			'   System.Windows.Forms.MessageBox.Show(sSourceTopoName & vbCrLf & sAttribExpr ,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,                                                                                    msDissolveTopoName, mtDissolveMapThemeData.LinkLayer , mtDissolveMapThemeData.ClosedPgonsLayer , mtDissolveMapThemeData.CentroidBlock & vbCrLf & mtDissolveMapThemeData.CentroidLayer, "06_008")
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(mtDissolveMapThemeData.LinkLayer, DMAcadExt.DMApp.AppID, True, True)
			If bCurrentLayerOK Then
				Dim oSourceTopo As TopoScheme.tsTopology = New TopoScheme.tsTopology(sSourceTopoName, True)
				oSourceTopo.Load(False)
				Dim oDissolveEComp As IEqualityComparer(Of TopoScheme.tsPolygon) = Nothing
				'   mtDissolveMapThemeData.MapThemeID
				Select Case mtDissolveMapThemeData.MapThemeID
					Case DMAcadExt.enMapTheme.Blocks
						oDissolveEComp = New BlockEq()
					Case DMAcadExt.enMapTheme.Mitham
						oDissolveEComp = New RegionEq
				End Select
				Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = oSourceTopo.GetDissolvedLinks(oDissolveEComp)

				'   System.Windows.Forms.MessageBox.Show(colLinks.Count.ToString(), "21_404")
				'    System.Windows.Forms.MessageBox.Show(mtDissolveMapThemeData.CentroidBlock & ":" & mtDissolveMapThemeData.CentroidLayer, "21_405")
				DMCommon.Debug.MsgBox("02_385", msDissolveTopoName, colLinks.Count, mtDissolveMapThemeData.CentroidBlock, mtDissolveMapThemeData.CentroidLayer)
				TopoManager.TopoCreator.CreateTopology(msDissolveTopoName, colLinks, mtDissolveMapThemeData.CentroidBlock, mtDissolveMapThemeData.CentroidLayer, True, True, 0.001)



				'	System.Windows.Forms.MessageBox.Show(sSourceTopoName & vbCrLf & msDissolveTopoName & vbCrLf & mtDissolveMapThemeData.CentroidBlock & vbCrLf & mtDissolveMapThemeData.CentroidLayer, "21_445")
				'  TopoManager.TopoCreator.DissolveTopo(sSourceTopoName, sAttribExpr, msDissolveTopoName, mtDissolveMapThemeData.CentroidBlock, mtDissolveMapThemeData.CentroidLayer)
				'TopoManager.TopoCreator.DissolveTopo(sSourceTopoName, sAttribExpr, sDissolveTopoName)


				Me.zzCheckDissolveTopo(False, False)

				' zzFillBlocksDic()
				If mdicBlocks IsNot Nothing Then
					' zzFillBlocks(sSourceTopoName)
					''''''''''''''''''''''''''''  zzFillGridA()
				End If


				zzUpdateAfterDissTopo()

			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If



	End Sub
	Private Sub zzUpdateBlocksDic()
		Dim oRow As DataRow
		Dim oBlock As TopoManager.TPlanGraph.TplnBlock = Nothing
		Dim iBlockNo As Integer, iBlockNoAdd As Integer
		If moBlockTable IsNot Nothing Then
			For iRowIndex As Integer = 0 To moBlockTable.Rows.Count - 1
				oRow = moBlockTable.Rows.Item(iRowIndex)
				'	DMCommon.Functions.DispArray(oRow.ItemArray, "oRow12", True)
				iBlockNo = DirectCast(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName), Integer)
				iBlockNoAdd = DirectCast(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName), Integer)

				oBlock = mdicBlocks.Block(iBlockNo, iBlockNoAdd)
				'	

				If oBlock IsNot Nothing Then
					' System.Windows.Forms.MessageBox.Show(CStr(mdicBlocks.Count) & vbCrLf & oBlock.BlockName, "21_904")
					Try
						oBlock.BlockStatusStr = CType(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockStatusFieldName), String)
						' System.Windows.Forms.MessageBox.Show(CStr(CType(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msBlockStatusFieldName), Integer)) & vbCrLf & oBlock.BlockStatus.ToString(), "21_907")
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoToClosedPgons - zzFillBlocksDic_1")
					End Try
					Try
						If Not IsDBNull(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockIsAnalyticFieldName)) Then
							oBlock.IsAnalytic = Convert.ToBoolean(DirectCast(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockIsAnalyticFieldName), Integer))
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoToClosedPgons - zzFillBlocksDic_2")
					End Try
					oBlock.UpdateCentroid()
					' System.Windows.Forms.MessageBox.Show(CStr(oBlock.BlockStatus) & ":" & CStr(oBlock.IsAnalytic), "02_433")
				Else
					System.Windows.Forms.MessageBox.Show(CStr(iBlockNo) & "  was not found", "21_817 Err")
				End If
			Next
		End If
	End Sub
	Private Sub zzFillBlocksNew(sSourceTopoName As String)
		Dim sAttribExpr As String = mtSourceMapThemeData.DissolveAttribExpr
		Dim sAttribTag As String = Nothing
		'   MessageBox.Show(sSourceTopoName & ":" & DMCommon.Functions.CStrN(sAttribExpr, "NN"), "07_100")
		If sAttribExpr.StartsWith("@") Then
			sAttribTag = sAttribExpr.Substring(1)
		End If
		If sAttribTag IsNot Nothing Then
			Dim bPoint As Boolean
			Dim tPgonCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim oSourcePgon As Autodesk.Gis.Map.Topology.Polygon = Nothing
			Dim tCentroidObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim iAttribIndex As Integer = DMAcadExt.AcadTransaction.GetAttribIndex(mtSourceMapThemeData.CentroidBlock, sAttribTag)
			Dim sValue As String
			Dim iBlockNo As Integer
			Dim iBlockNoAdd As Integer

			Dim oBlock As TopoManager.TPlanGraph.TplnBlock = Nothing
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			'	MessageBox.Show(sAttribTag & ":" & CStr(iAttribIndex), "03_231")
			Dim oDissolveTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oDissolveTopology IsNot Nothing Then
				Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oDissolveTopology.GetPolygons()

				For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					Try
						tPgonCenterPoint = oPolygon.Centroid
						oSourcePgon = oSourceTopology.FindPolygon(tPgonCenterPoint)
						tCentroidObjID = oSourcePgon.Entity

						sValue = DMAcadExt.AcadTransaction.GetAttribText(tCentroidObjID, True, bPoint, iAttribIndex)
						If Not String.IsNullOrEmpty(sValue) Then
							Try
								iBlockNo = Convert.ToInt32(sValue)
								oBlock = mdicBlocks.Block(iBlockNo, iBlockNoAdd)

								If oBlock IsNot Nothing Then
									oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
									If oBlockRef IsNot Nothing Then
										oBlock.AddCentroid(oBlockRef)
										oBlock.UpdateCentroid()
									Else
										MessageBox.Show(oPolygon.Entity.ToString(), "03_297 Err")
									End If
								Else
									MessageBox.Show(CStr(iBlockNo), "03_296 Err")
								End If

							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoToClosedPgons - zzFillBlocksDic_1")
							End Try
						End If


					Catch oEx As Exception
						MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_304")
					End Try
				Next


			Else
				MessageBox.Show("", "03_303")
			End If
		Else
			MessageBox.Show("", "03_302")
		End If
	End Sub
	Private Sub zzFillBlocks(sSourceTopoName As String)
		Dim sAttribExpr As String = mtSourceMapThemeData.DissolveAttribExpr
		Dim sAttribTag As String = Nothing
		'  MessageBox.Show(sSourceTopoName & ":" & DMCommon.Functions.CStrN(sAttribExpr, "NN"), "07_100")
		If sAttribExpr.StartsWith("@") Then
			sAttribTag = sAttribExpr.Substring(1)
		End If
		If sAttribTag IsNot Nothing Then
			Dim bPoint As Boolean
			Dim tPgonCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim oSourcePgon As Autodesk.Gis.Map.Topology.Polygon = Nothing
			Dim tCentroidObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim iAttribIndex As Integer = DMAcadExt.AcadTransaction.GetAttribIndex(mtSourceMapThemeData.CentroidBlock, sAttribTag)
			Dim sValue As String
			Dim iBlockNo As Integer
			Dim iBlockNoAdd As Integer

			Dim oBlock As TopoManager.TPlanGraph.TplnBlock = Nothing
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			'	MessageBox.Show(sAttribTag & ":" & CStr(iAttribIndex), "03_231")
			Dim oDissolveTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oDissolveTopology IsNot Nothing Then
				Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oDissolveTopology.GetPolygons()

				For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					Try
						tPgonCenterPoint = oPolygon.Centroid
						oSourcePgon = oSourceTopology.FindPolygon(tPgonCenterPoint)
						tCentroidObjID = oSourcePgon.Entity

						sValue = DMAcadExt.AcadTransaction.GetAttribText(tCentroidObjID, True, bPoint, iAttribIndex)
						If Not String.IsNullOrEmpty(sValue) Then
							Try
								iBlockNo = Convert.ToInt32(sValue)
								oBlock = mdicBlocks.Block(iBlockNo, iBlockNoAdd)
								' MessageBox.Show(oBlock.BlockStatus.ToString() & ":" & oBlock.IsAnalytic.ToString(), "03_940")
								If oBlock IsNot Nothing Then
									oBlock.IsCentroid = True
									oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
									If oBlockRef IsNot Nothing Then
										oBlock.AddCentroid(oBlockRef)
										oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
										oBlock.UpdateCentroid()
									Else
										MessageBox.Show(oPolygon.Entity.ToString(), "03_297 Err")
									End If
								Else
									MessageBox.Show(CStr(iBlockNo), "03_296 Err")
								End If

							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoToClosedPgons - zzFillBlocksDic_1")
							End Try
						End If


					Catch oEx As Exception
						MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_304")
					End Try
				Next


			Else
				MessageBox.Show("", "03_303")
			End If
		Else
			MessageBox.Show("", "03_302")
		End If
	End Sub
	Private Sub zzFillBlockGrid()
		Dim s As String
		mdicBlocks = TopoManager.TPlanGraph.TplnProject.Blocks
		moBlockTable = TopoManager.TPlanGraph.TplnBlock.BlockTable()
		mdicRegions = TopoManager.TPlanGraph.TplnProject.Regions
		moBlockTable = TopoManager.TPlanGraph.TplnBlock.BlockTable
		'If moBlockTable IsNot Nothing Then
		'   For Each oBlock As TopoManager.TPlanGraph.TplnBlock In mdicBlocks.Values
		'      '  MessageBox.Show(CStr(oBlock.HasCentroid), "03_397")
		'   Next
		'Else
		'End If
		'  \frmTopoToClosedPgons.vb:line 700

		If mdicBlocks Is Nothing Then
			s = "mdicBlocks Is Nothing"
		Else
			s = CStr(mdicBlocks.Count)
		End If
		If moBlockTable Is Nothing Then
			s &= ":" & "moBlockTable Is Nothing"
		Else
			s &= ":" & CStr(moBlockTable.Rows.Count)
		End If
		'  MessageBox.Show(s, "02_477")
		Me.dgvBlocks.AutoGenerateColumns = False
		'	Me.dgvBlocks.VirtualMode = True

		If moBlockTable IsNot Nothing Then

			moBlockView = New DataView(moBlockTable, String.Empty, TPlanGraph.TplnParcel.BlockFieldName & "," & TPlanGraph.TplnParcel.BlockAddFieldName, DataViewRowState.CurrentRows)
			Me.dgvBlocks.DataSource = moBlockView
		ElseIf mtSourceMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Parcels Then
			MessageBox.Show("Block table is Empty" & vbCrLf & mtSourceMapThemeData.MapThemeID.ToString(), "04_487")
		End If
		'  MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString(), "04_489")
	End Sub
	Private Sub zzFillGridA()
		Dim oRow As DataRowView
		Dim oGridRow As DataGridViewRow
		Dim oBlock As TopoManager.TPlanGraph.TplnBlock = Nothing
		Dim iBlockNo As Integer, iBlockNoAdd As Integer
		Dim bPgonExists As Boolean
		If moBlockView IsNot Nothing Then
			If TopoManager.TopoCreator.TopologyExists(msDissolveTopoName) Then
				bPgonExists = True
			Else
				bPgonExists = False
			End If
			'	MessageBox.Show(CStr(moBlockView.Count) & ":" & CStr(Me.dgvBlocks.Rows.Count), "02_463")
			For iRowIndex As Integer = 0 To moBlockView.Count - 1
				oRow = moBlockView.Item(iRowIndex)
				'	DMCommon.Functions.DispArray(oRow.ItemArray, "oRow12", True)
				iBlockNo = DirectCast(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName), Integer)
				iBlockNoAdd = DirectCast(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName), Integer)
				oBlock = mdicBlocks.Block(iBlockNo, iBlockNoAdd)

				'    System.Windows.Forms.MessageBox.Show(CStr(iBlockNo) & vbCrLf & CStr(iBlockNoAdd) & vbCrLf & CStr(oBlock.HasCentroid), "04_810")
				If oBlock IsNot Nothing Then
					oGridRow = Me.dgvBlocks.Rows.Item(iRowIndex)
					If bPgonExists Then
						oGridRow.Cells.Item("cchPgonExists").Value = oBlock.IsCentroid
					Else
						oGridRow.Cells.Item("cchPgonExists").Value = False
					End If
					'frmTopoToClosedPgons.vb:line 598
				Else
					System.Windows.Forms.MessageBox.Show(CStr(iBlockNo) & "  was not found", "21_811 Err")
				End If
			Next
		End If
	End Sub


	Private Sub zzTopoToClosedPgons(tMapThemeData As DMAcadExt.MapThemeData, bDissolve As Boolean, bExteriorRingOnly As Boolean)
		Dim bCurrentLayerOK As Boolean = False
		Dim sMethod As String = Nothing
		' 
		If (String.IsNullOrEmpty(tMapThemeData.ClosedPgonsMethod) OrElse tMapThemeData.ClosedPgonsMethod = "MAVAT") AndAlso Not String.IsNullOrEmpty(tMapThemeData.ClosedPgonsLayers) Then
			sMethod = "MAVAT"
		ElseIf tMapThemeData.ClosedPgonsMethod = "BN" Then
			sMethod = "BN"
		End If
		DMCommon.Debug.MsgBox("02_388", tMapThemeData.ClosedPgonsMethod, tMapThemeData.ClosedPgonsLayers, sMethod, tMapThemeData.TopoName)
		If sMethod IsNot Nothing Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			'   MessageBox.Show(tMapThemeData.ClosedPgonsLayers, "03_120")
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tMapThemeData.ClosedPgonsLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

			'	MessageBox.Show(tMapThemeData.ClosedPgonsLayers & ":" & tMapThemeData.TopoName, "02_388")
			If bCurrentLayerOK Then
				Dim sSourceTopoName As String
				If Me.rdbTopo.Checked OrElse bDissolve Then
					sSourceTopoName = tMapThemeData.TopoName
				ElseIf Me.rdbTopoWA.Checked Then
					sSourceTopoName = tMapThemeData.LineTopoName
				Else
					Return
				End If


				Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sSourceTopoName)

				If sMethod = "BN" Then
					oTopoScheme.SimplexLink = True
				End If

				oTopoScheme.Load(False)

				'  MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "03_124")
				If sMethod = "MAVAT" Then
					oTopoScheme.CreateDBPolylines()
				ElseIf sMethod = "BN" Then
					oTopoScheme.CalcIsthmus()
					oTopoScheme.CreateDBPolylineMPlus(True, True)
					oTopoScheme.CalcAreaByCentroidLayer(msaLayers, mdaValues, mdTotalByPolygons)
				End If

			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End If

	End Sub
	Private Sub zzTopoToClosedPgonsOld(tMapThemeData As DMAcadExt.MapThemeData, bDissolve As Boolean, bExteriorRingOnly As Boolean)
		Dim bCurrentLayerOK As Boolean = False
		'	Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
		Dim oColorPgon As ColorPolygon
		'	MessageBox.Show(tMapThemeData.ClosedPgonsLayers, "02_388")
		If Not String.IsNullOrEmpty(tMapThemeData.ClosedPgonsLayers) Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			'	MessageBox.Show(tMapThemeData.ClosedPgonsLayers, "03_120")
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tMapThemeData.ClosedPgonsLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

			MessageBox.Show(tMapThemeData.ClosedPgonsLayers & ":" & tMapThemeData.TopoName, "02_388V")
			If bCurrentLayerOK Then
				Dim sSourceTopoName As String
				If Me.rdbTopo.Checked OrElse bDissolve Then
					sSourceTopoName = tMapThemeData.TopoName
				ElseIf Me.rdbTopoWA.Checked Then
					sSourceTopoName = tMapThemeData.LineTopoName
				Else
					Return
				End If











				Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

				'MessageBox.Show(tMapThemeData.ClosedPgonsLayers & ":" & tMapThemeData.TopoName & vbCrLf & bExteriorRingOnly.ToString(), "02_389")
				If oTopoModel IsNot Nothing Then


					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						oColorPgon = New ColorPolygon(oPolygon)
						oColorPgon.CreateClosedPolygon(bExteriorRingOnly)
						oColorPgon.Terminate()

						oPolygon.Dispose()
						oPolygon = Nothing
					Next
					colPolygons.Dispose()
					colPolygons = Nothing
					oTopoModel.Close()
				Else
					MessageBox.Show("Topology '" & tMapThemeData.TopoName & "' Error", "03_123")
				End If
			Else
				MessageBox.Show("Layer '" & tMapThemeData.ClosedPgonsLayers & "' Error", "03_122Y")
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
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
		Dim sLotTopoName As String = msSourceTopoName
		Dim sLanduseTopoName As String = msDissolveTopoName
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
	Private Sub zzPaintByLanduse(sTopoName As String)
		Dim oTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopology.GetPolygons()
		Dim oPgon As TopoManager.TPlanGraph.TplnTopoPgon = Nothing
		For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
			'		oPgon = New TopoManager.TPlanGraph.TplnLusePgon(oPolygon,
		Next
	End Sub







	Private Sub cmdClose_Click(sender As System.Object, e As System.EventArgs) Handles cmdClose.Click
		Me.Close()
	End Sub


	Private Sub cmdColorSchemeEditor_Click(sender As System.Object, e As System.EventArgs)
		Dim fColorEditor As frmColorSchemeEditor = New frmColorSchemeEditor(mtSourceMapThemeData.MapThemeID, enColorEditorMode.Landuse, False)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, fColorEditor)
	End Sub
	Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
		'	MessageBox.Show(e.ClickedItem.Name & vbCrLf & mtMapThemeData.DissolveTopoName, "01_098")


		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbCreateTopo.Name
				zzCreateDissolveTopologyNew()

			Case Me.tsbCheckTopo.Name

				zzCheckTopo(True)

			Case Me.tsbDeleteTopo.Name
				zzDeleteTopo(False)
			Case Me.tsbExec.Name





		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzUpdateAfterDissTopo()
		TopoManager.TPlanGraph.TplnProject.InitBlockDic()
		'  TopoManager.TPlanGraph.TplnProject.TestBlockDic("IAfter nitBlockDic")
		TopoManager.TPlanGraph.TplnProject.LoadBlockTopology(True)
		'  TopoManager.TPlanGraph.TplnProject.TestBlockDic("After LoadBlockTopo")
		TopoManager.TPlanGraph.TplnProject.LoadBlocks()
		'  TopoManager.TPlanGraph.TplnProject.TestBlockDic("After LoadBlocksNew")
		TopoManager.TPlanGraph.TplnProject.LoadParcels()
		'  TopoManager.TPlanGraph.TplnProject.TestBlockDic("After LoadParcelsNew")
		zzUpdateBlocksDic()
		TopoManager.TPlanGraph.TplnBlock.CreateBlockTable()


	End Sub
	Private Sub zzCheckDispTopo()
		zzCheckTopo(True)
		zzDispTopo()
	End Sub
	Private Sub zzCheckTopo(ByVal bMsgBox As Boolean)
		Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(msDissolveTopoName, True, bMsgBox, mtSourceMapThemeData.MapThemeID)
	End Sub
	Private Sub zzDispTopo()
		'	Me.txtPgonCount.Text = Convert.ToString(mtTopoRes.PgonCount)
		'	zzDispTopoExists(mtTopoRes.TopoExists)
	End Sub
	Private Sub zzDeleteTopo(ByVal bDeleteEntities As Boolean)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

		TopoManager.TopoCreator.DeleteTopology(msDissolveTopoName, bDeleteEntities, False)
		zzCheckDissolveTopo(False, False)
		zzFillGridA()
		DMAcadExt.AcadDocument.Unlock()
	End Sub







	Private Sub ddbLayers_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbLayers.DropDownItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tsiThisTopoOnlyVisible.Name

				zzLanduseOnlyVisible()


			Case Me.tsiThisTopoVisible.Name

				'	zzSourceVisible(False)

			Case tsiAllVisible.Name
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
	Private Sub frmPaintLanduse_FormClosing(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		zzSaveParams()
	End Sub





	Private Sub cmdReadLog_Click(sender As System.Object, e As System.EventArgs)
		Dim sLogName As String = """" & DMAcadExt.AcadDocument.LogName & """"
		'	Dim iID As Integer = Shell("""notepad"" -a -q", , True, 100000)
		Dim iID As Integer = Microsoft.VisualBasic.Interaction.Shell("notepad " & sLogName, AppWinStyle.NormalFocus, False, 100000)
		DMAcadExt.AcadDocument.WriteMessage("notepad " & sLogName)
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	Private Sub frmTopo_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		Dim bMsgBox As Boolean = False

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

		zzCheckSourceTopo(False, bMsgBox)
		zzCheckDissolveTopo(False, bMsgBox)
		Me.lblSourceTopoName.Text = mtSourceMapThemeData.TopoName
		Me.lblSourceTopoWAName.Text = mtSourceMapThemeData.LineTopoName


		Me.lblSourceCPgonsName.Text = mtSourceMapThemeData.ClosedPgonsLayer

		Me.lblDissolveTopoName.Text = msDissolveTopoName
		Me.lblDissolveCPgonsName.Text = mtDissolveMapThemeData.ClosedPgonsLayer
		zzCheckClosedPgonsA()

		zzFillBlockGrid()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub dgvBlocks_CellValuePushed(oSender As System.Object, e As System.Windows.Forms.DataGridViewCellValueEventArgs) Handles dgvBlocks.CellValuePushed
		' MessageBox.Show(CStr(e.RowIndex) & ":" & CStr(e.ColumnIndex) & vbCrLf & e.Value.ToString(), "04_400")
		Select Case e.ColumnIndex
			Case 1
				Dim oDataRow As DataRow = moBlockTable.Rows.Item(e.RowIndex)
				oDataRow.Item("BlockType") = CType(e.Value, Integer)
		End Select

	End Sub



	Private Sub cmdDisToClosedPgons_Click(oSender As System.Object, e As System.EventArgs) Handles cmdDisToClosedPgons.Click
		Dim bExteriorRingOnly As Boolean = Me.chkDisExteriorRingOnly.Checked
		zzTopoToClosedPgons(mtDissolveMapThemeData, True, bExteriorRingOnly)
		zzCheckClosedPgons()
	End Sub

	Private Sub cmdEraseDisClosedPgons_Click(sender As System.Object, e As System.EventArgs) Handles cmdEraseDisClosedPgons.Click
		Dim sDissolveLayer As String = mtDissolveMapThemeData.ClosedPgonsLayer
		zzEraseClosedPgons(sDissolveLayer)
	End Sub

	Private Sub cmdSrcToClosedPgons_Click(sender As System.Object, e As System.EventArgs) Handles cmdSrcToClosedPgons.Click
		Dim bExteriorRingOnly As Boolean = Me.chkSrcExteriorRingOnly.Checked
		zzTopoToClosedPgons(mtSourceMapThemeData, False, bExteriorRingOnly) '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		zzCheckClosedPgons()
		zzCompareAreas()
	End Sub

	Private Sub cmdEraseSrcClosedPgons_Click(sender As System.Object, e As System.EventArgs) Handles cmdEraseSrcClosedPgons.Click
		Dim sSourceLayer As String = Nothing
		If mtSourceMapThemeData.ClosedPgonsMethod = "MAVAT" Then
			sSourceLayer = mtSourceMapThemeData.ClosedPgonsLayer
		ElseIf mtSourceMapThemeData.ClosedPgonsMethod = "BN" Then
			sSourceLayer = mtSourceMapThemeData.CentroidLayers
		End If
		If sSourceLayer IsNot Nothing Then
			zzEraseClosedPgons(sSourceLayer)
			zzCheckClosedPgons()
		End If

	End Sub

	Private Sub zzEraseClosedPgons(sLayer As String)
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ClearLayerByClassName(sLayer, DMAcadExt.AcadConst.AcadPolylineName)
		zzCheckClosedPgonsA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub



	Private Sub cmdEditData_Click(sender As System.Object, e As System.EventArgs)
		Dim fConstUserData As frmConstUserData = New frmConstUserData()
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

		Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fConstUserData)
	End Sub

	Private Sub cmdFillODTable_Click(sender As System.Object, e As System.EventArgs) Handles cmdFillODTable.Click
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Select Case Me.cmbODType.SelectedIndex
			Case 0
				TPlanGraph.TplnProject.FillMapLayerODTable()
			Case 1
				zzCreateLanduseODTable()
				TPlanGraph.TplnProject.FillMapLanduseODTable()
		End Select

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub
	Private Sub zzCreateLanduseODTable()
		Dim oODTable As DMAcadExt.ODTable = New DMAcadExt.ODTable("LotLanduse")
		Dim oaFieldDef(3) As Autodesk.Gis.Map.ObjectData.FieldDefinition
		If Not oODTable.Exists Then
			oaFieldDef(0) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("LanduseID", "", 0)
			oaFieldDef(1) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("LuseName", "", "")
			oaFieldDef(2) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("Area", "", 0.0)
			oaFieldDef(3) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("PlanName", "", "")

			oODTable.CreateTable(oaFieldDef)


		End If
	End Sub
	Private Sub cmdExportShape_Click(oSender As System.Object, e As System.EventArgs) Handles cmdExportShape.Click
		If Me.cmbODType.SelectedIndex >= 0 Then



			Dim sShapeFileName As String
			'	Dim oDirectoryInfo As IO.DirectoryInfo	'= New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFeatureClass)
			'     Dim sTopoName As String = "LotsKLine"
			sShapeFileName = "T:\Boris\Lots.shp"
			sfdShapeFile.OverwritePrompt = True

			Me.sfdShapeFile.DefaultExt = "shp"
			Me.sfdShapeFile.Filter = "Shape Files(*.shp)|*.shp|All Files (*.*)|*.*"
			If Me.rdbTopo.Checked Then
				msSourceTopoName = mtSourceMapThemeData.TopoName
			ElseIf Me.rdbTopoWA.Checked Then
				msSourceTopoName = mtSourceMapThemeData.LineTopoName


			End If

			If sfdShapeFile.ShowDialog() = Windows.Forms.DialogResult.OK Then
				sShapeFileName = sfdShapeFile.FileName
				Dim oShapeExpImp As FDO.ShapeExpImp = New FDO.ShapeExpImp(FDO.enExpImp.Export, sShapeFileName)
				oShapeExpImp.FromTopology(msSourceTopoName)
				Select Case Me.cmbODType.SelectedIndex
					Case 0
						oShapeExpImp.AddAgamLayerData("LayerDef")
					Case 1
						oShapeExpImp.AddLotLanduseData("LotLanduse", msSourceTopoName)

				End Select


				'    System.Windows.Forms.MessageBox.Show(msSourceTopoName & vbCrLf & sShapeFileName, "08_230")
				Try
					oShapeExpImp.Exec()
				Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
					System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName & vbCrLf & msSourceTopoName, "01_857b")

				End Try


				Dim oMySettings As My.MySettings = New My.MySettings()
				oMySettings.LastODTypeIndex = Me.cmbODType.SelectedIndex

				oMySettings.Save()
			End If
		End If
	End Sub
	Private Shared Function zzStrToInt(sValue As String) As Integer
		If String.IsNullOrEmpty(sValue) Then
			Return 0
		Else
			Dim iRes As Integer = 0

			If Not Integer.TryParse(sValue, iRes) Then
				iRes = 0
			End If
			Return iRes
		End If
	End Function
	Private Class BlockEq
		Implements IEqualityComparer(Of TopoScheme.tsPolygon)

		Public Function PgonEquals(oPgonA As TopoScheme.tsPolygon, oPgonB As TopoScheme.tsPolygon) As Boolean Implements IEqualityComparer(Of TopoScheme.tsPolygon).Equals
			Dim sBlockA As String = oPgonA.CentroidData.GetAttribValue("LOT_NUM")
			Dim sBlockAddA As String = oPgonA.CentroidData.GetAttribValue("GUSH_SUFFI")
			Dim iBlockA As Integer = zzStrToInt(sBlockA)
			Dim iBlockAddA As Integer = zzStrToInt(sBlockAddA)
			Dim sBlockB As String = oPgonB.CentroidData.GetAttribValue("LOT_NUM")
			Dim sBlockAddB As String = oPgonB.CentroidData.GetAttribValue("GUSH_SUFFI")
			Dim iBlockB As Integer = zzStrToInt(sBlockB)
			Dim iBlockAddB As Integer = zzStrToInt(sBlockAddB)
			If iBlockA = iBlockB AndAlso iBlockAddA = iBlockAddB Then
				Return True
			Else
				Return False
			End If
		End Function

		Public Function GetHashCode1(obj As TopoScheme.tsPolygon) As Integer Implements IEqualityComparer(Of TopoScheme.tsPolygon).GetHashCode

		End Function

	End Class

	Private Class RegionEq
		Implements IEqualityComparer(Of TopoScheme.tsPolygon)

		Public Function PgonEquals(oPgonA As TopoScheme.tsPolygon, oPgonB As TopoScheme.tsPolygon) As Boolean Implements IEqualityComparer(Of TopoScheme.tsPolygon).Equals
			Dim sRegionA As String = oPgonA.CentroidData.GetAttribValue(TopoManager.TPlanGraph.TplnLot.InPlanAttribTag)
			Dim iRegionA As Integer = zzStrToInt(sRegionA)
			Dim sRegionB As String = oPgonB.CentroidData.GetAttribValue(TopoManager.TPlanGraph.TplnLot.InPlanAttribTag)
			Dim iRegionB As Integer = zzStrToInt(sRegionB)
			Return iRegionA.Equals(iRegionB)


		End Function

		Public Function GetHashCode1(obj As TopoScheme.tsPolygon) As Integer Implements IEqualityComparer(Of TopoScheme.tsPolygon).GetHashCode

		End Function

	End Class

	Private Sub dgvBlocks_DataError(oSender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvBlocks.DataError
		e.Cancel = True
	End Sub

	Private Sub cmdUpdateData_Click(oSender As System.Object, e As EventArgs) Handles cmdUpdateData.Click




		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		zzUpdateBlocksDic()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub

	Private Sub cmdSetMarkBlocks_Click(oSender As System.Object, e As EventArgs) Handles cmdSetMarkBlocks.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'   MessageBox.Show(DMCommon.Functions.CStrN(msDissolveTopoName, "nothing71"), "08_230")
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(msDissolveTopoName)

		DMAcadExt.AcadDocument.SetLogName()
		Dim bCurrentLayerOK As Boolean


		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msMarkBlockLayerName, DMAcadExt.DMApp.AppID, True, True)
		If bCurrentLayerOK Then
			oTopoScheme.Load(False)

			' MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "08_100")
			Dim oResList As List(Of InitInsertData) = oTopoScheme.GetMarkPoints(14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor)

			' MessageBox.Show(CStr(oResList.Count), "08_110")

			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msMarkBlockName, "M:\Dm_Work\Blocks\Mavat-2010")
			Dim tBlockRefData As DMAcadExt.BlockRefData
			Dim iParity As Integer = 0
			oAcadBlock.OpenForRight()
			For Each tInitInsertData As InitInsertData In oResList
				'  DMAcadExt.AcadTransaction.InsertPoint(tInitInsertData.Position)
				tBlockRefData = New DMAcadExt.BlockRefData()

				tBlockRefData.Position = tInitInsertData.Position
				'If iParity = 0 Then
				'   tBlockRefData.ColorIndex = 2S
				'Else
				'   tBlockRefData.ColorIndex = 3S
				'End If

				'    tBlockRefData.Layer = msTazarMapBlockLayer
				tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor)
				tBlockRefData.Rotation = tInitInsertData.Rotation + Convert.ToDouble(iParity) * Math.PI
				oAcadBlock.InsertRefNewNew(tBlockRefData)
				iParity = (iParity + 1) Mod 2

			Next
		End If



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub

	Private Sub lblDissolveTopoName_Click(oSender As System.Object, e As EventArgs) Handles lblDissolveTopoName.Click

	End Sub

	Private Sub cmdZoom_Click(oSender As System.Object, e As EventArgs) Handles cmdZoom.Click
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvBlocks.CurrentCell

		If oCurrentCell IsNot Nothing Then
			Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
			Dim dCentroidX, dCentroidY As Double
			Dim oDataRow As DataRowView = moBlockView.Item(iCurrentRowIndex)
			Dim oGridRow As DataGridViewRow = Me.dgvBlocks.Rows.Item(iCurrentRowIndex)
			dCentroidX = DMCommon.Functions.CDblN(oDataRow.Item(TopoReader.msCentroidXFldName))
			dCentroidY = DMCommon.Functions.CDblN(oDataRow.Item(TopoReader.msCentroidYFldName))
			DMAcadExt.AcadDocument.Zoom(New DMAcadExt.TPlnPoint(dCentroidX, dCentroidY), 20.0)

		End If

	End Sub

	Private Sub cmdClearMarkBlocks_Click(oSender As System.Object, e As EventArgs) Handles cmdClearMarkBlocks.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		If DMAcadExt.AcadDocument.IsLocked Then
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msMarkBlockName)
			oAcadBlock.LoadAllReferencesByLayers(msMarkBlockLayerName)
			oAcadBlock.DeleteAll()


			UnidivNet.UD_App.DeleteHanitTopos()
			DMAcadExt.AcadDocument.CloseLog()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End If
	End Sub
	Private Sub zzInsertBlockColumns()
		Me.ctxBlock = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchStatus = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchIsAnalytical = New System.Windows.Forms.DataGridViewCheckBoxColumn()

		'
		'ctxBlock
		'
		With Me.ctxBlock
			.DataPropertyName = "BlockFull"
			.HeaderText = "גוש"
			.Name = "ctxBlock"
			.ReadOnly = True
			.Width = 74
		End With
		Me.dgvBlocks.Columns.Insert(0, Me.ctxBlock)

		'
		'cchStatus
		'
		With Me.cchStatus
			.DataPropertyName = "BlockStatus"
			.FalseValue = "20"
			.FillWeight = 60.0!
			.HeaderText = "מוסדר"
			.IndeterminateValue = "0"
			.Name = "cchStatus"
			.TrueValue = "6"
			.Width = 48
		End With
		Me.dgvBlocks.Columns.Insert(1, Me.cchStatus)

		'
		'cchIsAnalytical
		'
		With Me.cchIsAnalytical
			.DataPropertyName = "IsAnalytic"
			.FalseValue = "0"
			.HeaderText = "אנליטי"
			.Name = "cchIsAnalytical"
			.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
			.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic
			.TrueValue = "-1"
			.Width = 48
		End With
		Me.dgvBlocks.Columns.Insert(2, Me.cchIsAnalytical)
	End Sub
	Private Sub zzInsertColumnMitham()
		Me.ctxMitham = New System.Windows.Forms.DataGridViewTextBoxColumn()
		'
		'ctxMitham
		'
		With Me.ctxMitham
			.DataPropertyName = "BlockFull"
			.HeaderText = "מתחם"
			.Name = "ctxMitham"
			.ReadOnly = True
			.Width = 60
		End With
		Me.dgvBlocks.Columns.Insert(0, Me.ctxMitham)

		Me.ctxMithamName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		'
		'ctxMitham
		'
		With Me.ctxMithamName
			.DataPropertyName = "BlockFull"
			.HeaderText = "שם מתחם"
			.Name = "ctxMitham"
			.ReadOnly = True
			.Width = 110
		End With
		Me.dgvBlocks.Columns.Insert(1, Me.ctxMithamName)

	End Sub
	Private Sub zzDrawBorder_Lots()
		Const sLayerName As String = "Pgvul1"
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtSourceMapThemeData.TopoPurpose
		Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = dicLots.GetBorderLinks(iTopoPurpose)
		'	DMCommon.Debug.MsgBox("zzDrawBorder_Lots", iTopoPurpose, dicLots.Count, colLinks.Count, mtSourceMapThemeData.TopoName)
		DMAcadExt.AcadTransaction.AddToLayer(colLinks, sLayerName)

		DMAcadExt.AcadTransaction.CloseModelSpace()

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.SendRegenAll()

	End Sub
	Private Sub zzDrawBorder_BN()
		Const sAttribExpr As String = ".DWGName"
		Const sBorderTopoName As String = "Border"

		Dim dicPolygonBorders As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		If TopoManager.TopoCreator.TopologyExists(mtSourceMapThemeData.TopoName) Then

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)



			TopoManager.TopoCreator.DissolveTopo(mtSourceMapThemeData.TopoName, sAttribExpr, sBorderTopoName)

			Dim oBorderTopo As TopoScheme.tsTopology = New TopoScheme.tsTopology(sBorderTopoName)
			Dim oBorderPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline
			oBorderTopo.Load(False)
			DMAcadExt.AcadTransaction.SetCurrentLayer("BN1200", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
			oBorderTopo.CreateDBPolylines()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			dicPolygonBorders = oBorderTopo.PolygonBorders
			If dicPolygonBorders.Count = 1 Then
				oBorderPolyline = DMAcadExt.AcadTransaction.GetPolyline(dicPolygonBorders.Item(0), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
				mdTotalByBorder = oBorderPolyline.Area
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			zzCompareAreas()
		End If
	End Sub


	Private Sub cmdDrawBorder_Click(oSender As System.Object, e As EventArgs) Handles cmdDrawBorder.Click
		Const sAttribExpr As String = ".DWGName"
		Const sBorderTopoName As String = "Border"

		Dim dicPolygonBorders As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		If False Then
			zzDrawBorder_Lots()
		End If



		If TopoManager.TopoCreator.TopologyExists(mtSourceMapThemeData.TopoName) Then

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			TopoManager.TopoCreator.DissolveTopo(mtSourceMapThemeData.TopoName, sAttribExpr, sBorderTopoName)

			Dim oBorderTopo As TopoScheme.tsTopology = New TopoScheme.tsTopology(sBorderTopoName)
			Dim oBorderPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline
			oBorderTopo.Load(False)
			DMAcadExt.AcadTransaction.SetCurrentLayer("BN1200", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
			oBorderTopo.CreateDBPolylines()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			dicPolygonBorders = oBorderTopo.PolygonBorders
			If dicPolygonBorders.Count = 1 Then
				oBorderPolyline = DMAcadExt.AcadTransaction.GetPolyline(dicPolygonBorders.Item(0), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
				mdTotalByBorder = oBorderPolyline.Area
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			zzCompareAreas()
		End If



	End Sub
	Private Sub zzCompareAreas()
		Const dTolerance As Double = 0.001
		If mdTotalByPolygons > Double.Epsilon AndAlso mdTotalByBorder > Double.Epsilon Then
			For iIndex As Integer = 0 To msaLayers.GetUpperBound(0)
				DMAcadExt.AcadDocument.WriteMessage("Layer: " & msaLayers(iIndex) & " --- Area = " & mdaValues(iIndex).ToString())
			Next
			DMAcadExt.AcadDocument.WriteMessage(" Total Area: " & mdTotalByPolygons.ToString())
			DMAcadExt.AcadDocument.WriteMessage(" Border Area: " & mdTotalByBorder.ToString())
			If Math.Abs(mdTotalByPolygons - mdTotalByBorder) > dTolerance Then
				MessageBox.Show(DMCommon.dmMessages.Message(308, Name), "Datamap")

			End If

		End If
	End Sub

End Class

