Option Explicit On
Option Strict On
Imports System.ComponentModel
Imports System.Data
Public Class frmPrjThemes
	Private Enum enTask
		Unidiv
		Settlement
		Expropriation
		EntityConnected
		EntityConnectedLot
		Bamash
		LotApproved
	End Enum





	Private Const miParamType As Integer = 0
	Public Shared moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Public Shared moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmProjectThemes
	Private moPrjMapThemes As DataTable
	Private Shared mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
	Private Shared mhsThemeUpLayers As HashSet(Of String)
	Private mdicDissolves As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.enMapTheme)



	' iMapThemeID As DMAcadExt.enMapTheme
	Private miCurrentRow As Integer
	Private miProjectCode As Integer
	Private miDetailNo As Integer = 0
	Private moDWGProjectData As TopoManager.DWGProjectData
	Private moIndeterminateGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle
	Private moDefaultCheckGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle
	Private miTask As enTask
	'Private miCurrentRegionNo As Integer = 0
	'	Private WithEvents mfTopoCleanup As frmTopoCleanup
	'	Private WithEvents mfFDO_Overlay As frmFDO_Overlay
	Private WithEvents mfMapThemeBase As frmMapThemeBase = Nothing
	'	Private WithEvents mfCheckThemeAAA As ICheckTheme = Nothing
	Private WithEvents mfDesign As Form = Nothing
	Private WithEvents mfTopoToClosedPgons As frmTopoToClosedPgons = Nothing
	Private WithEvents mfrmPgonSetView As frmPgonSetView = Nothing

	Private WithEvents mfMessages As frmMessages = Nothing
	Private WithEvents mfProperty As frmMapThemeProperty = Nothing
	Private WithEvents mfTplnView As frmTplnView = Nothing

	Private WithEvents mfReports As frmReports

	Private WithEvents mfOwnership As frmOwnership
	Private WithEvents mfUnidiv As frmUnidiv
	Private WithEvents mfExpro As Expro.frmExpro
	Private WithEvents mfBamash As frmBamashM
	Private WithEvents fMapThemeBase As frmMapThemeBase = Nothing
	Private WithEvents mfCalcAreas As frmCalcAreas

	Private WithEvents mfUD_General As frmUD_General
	Private WithEvents mfUD_ProjectData As frmUD_ProjectData
	Private WithEvents mfUD_SelectPlan As frmUD_SelectPlan
	Private WithEvents mfEntConnected As Expro.frmEntConnected
	Private WithEvents mfEntConnectedLot As Expro.frmEntConnectedLot
	Private WithEvents mfEditLayerList As Expro.frmEditLayerList

	Private WithEvents mfLotPoints As frmLotPoints
	Private WithEvents smiLotApproved_CalcAreas As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiLotApproved_Points As ToolStripMenuItem


	Private WithEvents smiUnidiv_General As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiUnidiv_ProjectData As ToolStripMenuItem
	Private WithEvents smiUnidiv_ExportData As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiUnidiv_ImportData As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiUnidiv_EraseHanit As System.Windows.Forms.ToolStripMenuItem
	'Private WithEvents smiUnidiv_EraseDBStages As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiUnidiv_EraseDB As ToolStripMenuItem
	Private WithEvents smiExpro_EntConnected As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiExpro_EntConnectedLot As System.Windows.Forms.ToolStripMenuItem
	Private WithEvents smiExpro_EntConnectedLayer As System.Windows.Forms.ToolStripMenuItem

	Private moDetailMenuItems() As System.Windows.Forms.ToolStripItem ' DetailMenuItem
	Private miDetailRowsCount As Integer
	Private moParams As TPlServerDB.dmParams
	'	Private moCurrentDataRow As DataRow
	Private moCurrentRowIndex As Integer
	'  Private miSourceMapThemeID As DMAcadExt.enMapTheme
	'  Private miDissolveMapThemeID As DMAcadExt.enMapTheme
	Private miOverlayAddMapThemeID As DMAcadExt.enMapTheme

	Private mbEventsEnabled As Boolean = False
	Private mfMPgonView As frmMPgonView
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		DMCommon.Debug.ExcelLog = New DMCommon.ExcelAppExt()
		If True Then
			If DMCommon.Debug.Debug Then
				DMCommon.Debug.ExcelLog.Open()
			End If
		End If
		If False Then
			'	If moExcelApp Is Nothing Then
			'	moExcelApp = New ExcelApp

			'end If

			'	moExcelApp.OpenApplication()
			'	moExcelApp.AddNewWorkBook()
		End If



		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()

		zzGetEmplData()

	End Sub
	Public Shared ReadOnly Property MapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
		Get
			Return mdicMapThemes
		End Get
	End Property
	Public Shared ReadOnly Property ThemeUpLayers As HashSet(Of String)
		Get
			Return mhsThemeUpLayers
		End Get
	End Property

	Public Sub ActiveFormView(bVisible As Boolean)
		If mfUnidiv IsNot Nothing Then
			mfUnidiv.ActiveFormView(bVisible)
		End If

	End Sub

	Private Sub zzMyInitializeComponent()

		If TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState = ConnectionState.Open Then
			TPlServerDB.ServerDB.AddInitialize()
		End If

		'''''''''''''''	TopoManager.TPlanGraph.TplnProject.InitializeList()

		Me.dgvPrjThemes.AutoGenerateColumns = False
		Me.dgvPrjThemes.RowHeadersWidth = 23

		'''''''''	zzOpenProjectData()
		'  DMAcadExt.AcadDocument.OpenLog(False)
		''''''''''''''''''''''''''''''''''''	DMAcadExt.AcadTransaction.Start()
		zzGetProjectData()
		'  oDWGProjectData.CloseDictionary()
		DMAcadExt.AcadTransaction.Terminate()
		If miProjectCode <> 0 Then
			Me.tstProjectCode.Text = Convert.ToString(miProjectCode)
			zzDispDetailNo()
		End If
		moDefaultCheckGridViewCellStyle = Me.dgvPrjThemes.DefaultCellStyle.Clone
		moDefaultCheckGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
		moIndeterminateGridViewCellStyle = Me.dgvPrjThemes.DefaultCellStyle.Clone
		moIndeterminateGridViewCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
		moIndeterminateGridViewCellStyle.BackColor = System.Drawing.Color.Silver
		moIndeterminateGridViewCellStyle.ForeColor = System.Drawing.Color.Silver
		moIndeterminateGridViewCellStyle.NullValue = True
		moIndeterminateGridViewCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
		moIndeterminateGridViewCellStyle.SelectionForeColor = System.Drawing.Color.Silver
		'   zzGetProjectList()
		zzGetMyProjectList()
	End Sub
	Private Sub zzGetEmplData()
		'Dim sComText As String = "SELECT TOP (100) PERCENT PrjString FROM dbo.UserPrjList WHERE (UserName = '" & System.Environment.UserName & "') GROUP BY PrjString ORDER BY MAX(RecID) DESC"
		Dim sComText As String = " SELECT   Empl_id FROM ShopData.dbo.Employees AS Employees_1 WHERE ({fn UCASE(Empl_username) } = '" & UCase(System.Environment.UserName) & "')"
		Dim oEmplID As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, CommandType.Text)
		If oEmplID IsNot Nothing Then
			TPlServerDB.ServerDB.CurrentProjectDB.EmployeeID = Convert.ToInt32(oEmplID)
		End If


	End Sub
	Private Sub zzGetProjectData()
		Dim bOpenTransaction As Boolean
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		bOpenTransaction = False
		'DMCommon.Debug.MsgBox("081120_1", "31")
		zzOpenProjectData()
		'DMCommon.Debug.MsgBox("081120_1", "32", moDWGProjectData Is Nothing)
		'  DMAcadExt.AcadDocument.OpenLog(False)
		''''''''''''''''''''''''''''''''''''	DMAcadExt.AcadTransaction.Start()
		Dim iDWGProjectCode As Integer = moDWGProjectData.ProjectCode
		'DMCommon.Debug.MsgBox("251020_2", iDWGProjectCode)
		If iDWGProjectCode <> 0 Then
			miProjectCode = iDWGProjectCode
			miDetailNo = moDWGProjectData.DetailNo
		Else
			Dim oMySettings As My.MySettings = New My.MySettings()
			Dim iLastProjectCode As Integer = oMySettings.LastProjectCode
			If iLastProjectCode <> 0 Then
				miProjectCode = iLastProjectCode
				miDetailNo = oMySettings.LastDetailNo

			End If

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			bOpenTransaction = True
			zzSetProjectData(bOpenTransaction)

		End If
		moDWGProjectData.CloseDictionary()


		DMAcadExt.AcadTransaction.Terminate()

		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzSetDoneGridViewCellStyle()

		Me.dgvPrjThemes.Columns.Item(3).DefaultCellStyle = moIndeterminateGridViewCellStyle
		'Me.dgvPrjThemes.Columns.Item(3).FlatStyle = System.Windows.Forms.FlatStyle.Flat
	End Sub
	Private Sub zzOpenProjectData()



		moDWGProjectData = New TopoManager.DWGProjectData()

		moDWGProjectData.OpenData(False, True)



	End Sub
	Private Sub zzLoadParams()
		Try
			moParams = New TPlServerDB.dmParams(miProjectCode, miDetailNo, 0, miParamType)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzLoadParams")
		End Try


	End Sub

	Private Sub zzDispDetailNo()
		If miDetailNo = 0 Then
			Me.tslDetailNo.Text = String.Empty
		Else
			Me.tslDetailNo.Text = "/" & Convert.ToString(miDetailNo)
		End If
	End Sub
	Private Sub zzGetProjectList()

		Dim sComText As String = Nothing '= "SELECT TOP 40000 [ProjectCode] FROM [ProjectData].[dbo].[PrjList]"
		Select Case TPlServerDB.ServerDB.CurrentServerDB.Provider
			Case TPlServerDB.TPlProvider.ProviderSQLServer
				sComText = "SELECT TOP 40000 [ProjectCode] FROM [ProjectData].[dbo].[PrjList]"
			Case TPlServerDB.TPlProvider.ProviderJet
				sComText = "SELECT TOP 40000 [ProjectCode] FROM [PrjList]"
		End Select
		If sComText IsNot Nothing Then
			'	MessageBox.Show(sComText, "05_452")
			Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			Dim i As Integer
			If oDataReader IsNot Nothing Then

				While oDataReader.Read
					Me.tstProjectCode.AutoCompleteCustomSource.Add(Convert.ToString(oDataReader.GetInt32(0)))
					i += 1
				End While

				oDataReader.Close()
			End If
		End If

	End Sub
	Private Sub zzGetMyProjectList()
		Dim sComText As String

		sComText = "SELECT TOP (100) PERCENT PrjString FROM dbo.UserPrjList WHERE (UserName = 'boris') GROUP BY PrjString ORDER BY MAX(RecID) DESC"
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then

			While oDataReader.Read
				Me.tstProjectCode.AutoCompleteCustomSource.Add(Convert.ToString(oDataReader.GetString(0)))

			End While

			oDataReader.Close()
		End If

		'  Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
	End Sub


	Private Function zzGetInputProjectCode() As Integer
		Dim iProjectCode As Integer
		If Integer.TryParse(Me.tstProjectCode.Text, iProjectCode) Then
			Return iProjectCode
		Else
			Return 0
		End If


	End Function
	Private Function zzSetInputProject() As Boolean
		Dim iInputProjectCode As Integer = zzGetInputProjectCode()
		If iInputProjectCode <> 0 Then
			If miProjectCode <> iInputProjectCode Then
				miProjectCode = iInputProjectCode
				DMAcadExt.AppMessages.ClearAll()
			End If
			Return True
		Else
			Return False
		End If
	End Function

	Private Sub zzSetProject()
		zzSetDetails()

		TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode = miProjectCode
		TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo

		zzSetProjectCodeForRead()
		zzLoadParams()
	End Sub
	Private Sub zzSaveLastProjectSetting()
		Dim oMySettings As My.MySettings = New My.MySettings()
		oMySettings.LastProjectCode = miProjectCode
		oMySettings.LastDetailNo = miDetailNo
		oMySettings.Save()
	End Sub

	Private Sub tstProjectCode_Validating(oSender As System.Object, e As CancelEventArgs) Handles tstProjectCode.Validating
		If zzSetInputProject() Then
			zzSetProject()
			zzSetProjectData(True)
		Else
			e.Cancel = True
		End If

	End Sub
	Private Sub zzSetReadOnly(bValue As Boolean)
		Me.txtTopoName.ReadOnly = bValue
		Me.txtLinkLayers.ReadOnly = bValue
		Me.txtCentroidLayer.ReadOnly = bValue
		Me.txtCentroidBlockName.ReadOnly = bValue
		Me.txtNodeBlockName.ReadOnly = bValue
		Me.txtNodeLayer.ReadOnly = bValue
		Me.txtLineTopoName.ReadOnly = bValue
		Me.txtLineLinkLayer.ReadOnly = bValue
		Me.txtClosedPgonsLayer.ReadOnly = bValue


	End Sub

	Private Sub zzSetProjectCodeForRead()

		Dim sComText As String = "SELECT [MapThemeID],[MapThemeName],[GraphTypeID],[GraphTypeName],[GraphTypeShortName],[GroupID],[FileName],[TopoName],[LinkLayers],[CentroidBlocks],[CentroidLayers],[ClosedPgonsLayers], [LineTopoName],[LineLinkLayer],[NodeBlocks],[NodeLayers],[MPgonLayers],[DissolveTopoName],[DissolveAttribExpr],[DissolveClosedPgonsLayers],[CleanupType],[LineCleanupType],[DesignSet],[SourceMapThemeID],[OverlayMapThemeID],[OverlayMapThemeID_A],[TabaPurpose],[DesignElementType],[TopoPriority],[SPointsBlocks],[SPointsLayers] FROM PrjMapThemesExt WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(miDetailNo) & ")"

		'  DMAcadExt.AcadDocument.WriteMessage("!!38 ComText: " & sComText)

		moPrjMapThemes = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "PrjMapThemes")
		Me.dgvPrjThemes.Columns.Clear()

		If moPrjMapThemes IsNot Nothing Then
			miCurrentRow = -1
			Me.dgvPrjThemes.DataSource = moPrjMapThemes

			'	Me.txtLinkLayers.DataBindings.Add("Text", moPrjMapThemes, "LinkLayers")
			'	Me.txtCentroidLayer.DataBindings.Add("Text", moPrjMapThemes, "CentroidLayers")
			zzSetDataBinding()

			zzSetPanelReadOnly(True)

			zzSetGridColumnsForRead()
			zzClearCheckColumn()
			Me.dgvPrjThemes.ReadOnly = True ''''''''''''TEMP = False	'
			zzSetReadOnly(True)

			zzCreateMapThemesDictionary()

			zzSaveLastProjectSetting()
			zzSetDoneGridViewCellStyle()
		End If
		'	052 461 24 01
		'	052 455 50 81
	End Sub
	Private Function zzGetMapThemeID(oRow As DataRow) As DMAcadExt.enMapTheme
		Const sFieldName As String = "MapThemeID"
		Dim iValue As Integer = DirectCast(oRow.Item(sFieldName), Integer)
		If [Enum].IsDefined(GetType(DMAcadExt.enMapTheme), iValue) Then
			Return CType(iValue, DMAcadExt.enMapTheme)
		Else
			System.Windows.Forms.MessageBox.Show("MapTheme=" & iValue.ToString(), "Err #428")
			Return DMAcadExt.enMapTheme.Undefined
		End If

	End Function

	Private Sub zzCreateMapThemesDictionary()
		Dim oRow As DataRow
		Dim iMapThemeID As DMAcadExt.enMapTheme
		Dim tMapThemeData As DMAcadExt.MapThemeData
		Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim oDataGridRow As System.Windows.Forms.DataGridViewRow

		mdicMapThemes = New Dictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)()
		mdicDissolves = New Dictionary(Of DMAcadExt.enMapTheme, DMAcadExt.enMapTheme)()
		mhsThemeUpLayers = New HashSet(Of String)()

		TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.Undefined

		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oRow = moPrjMapThemes.Rows.Item(iIndex)
			iMapThemeID = zzGetMapThemeID(oRow)
			tMapThemeData = New DMAcadExt.MapThemeData(oRow)
			If tMapThemeData.LineTopoName Is Nothing Then
				MessageBox.Show(CInt(tMapThemeData.MapThemeID).ToString(), "08_495")
			End If
			Try
				mdicMapThemes.Add(iMapThemeID, tMapThemeData)
				tMapThemeData.AddAllUpLayers(mhsThemeUpLayers)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iMapThemeID), "frmPrjThemes - zzCreateMapThemesDictionary_3")
			End Try
		Next
		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oRow = moPrjMapThemes.Rows.Item(iIndex)
			iMapThemeID = zzGetMapThemeID(oRow)
			'  tMapThemeData = New DMAcadExt.MapThemeData(oRow)
			tMapThemeData = mdicMapThemes.Item(iMapThemeID)
			' "frmPrjThemes - zzCreateMapThemesDictionary_3")
			If tMapThemeData.LineTopoName Is Nothing Then
				MessageBox.Show(CInt(tMapThemeData.MapThemeID).ToString(), "08_497")
			End If
			oDataGridRow = Me.dgvPrjThemes.Rows.Item(iIndex)
			If tMapThemeData.GraphType = DMAcadExt.enGraphType.MapLayerOverlay Then
				TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
			ElseIf iMapThemeID = DMAcadExt.enMapTheme.Parcels OrElse iMapThemeID = DMAcadExt.enMapTheme.UD_Parcels Then
				If tMapThemeData.GraphType = 1 Then
					TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.Topologia
				ElseIf tMapThemeData.GraphType = 2 Then
					TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.ClosedPolygons
				End If

			ElseIf iMapThemeID = DMAcadExt.enMapTheme.Ownership Then
				DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Ownership
			ElseIf iMapThemeID = DMAcadExt.enMapTheme.BN Then
				DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.BN
			End If

			Dim s As String = iMapThemeID.ToString & ":" & tMapThemeData.GraphType.ToString
			s &= vbCrLf & TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod.ToString()
			s &= vbCrLf & DMAcadExt.DMApp.AppID.ToString()
			'	DMCommon.Debug.MsgBox("13_011", iMapThemeID, tMapThemeData.GraphType, TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod, DMAcadExt.DMApp.AppID)

			If tMapThemeData.SourceMapThemeID <> 0 AndAlso (tMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons OrElse tMapThemeData.GraphType = DMAcadExt.enGraphType.Topology) Then
				If Not mdicDissolves.ContainsKey(tMapThemeData.SourceMapThemeID) Then
					mdicDissolves.Add(tMapThemeData.SourceMapThemeID, iMapThemeID)
				End If

				'   miDissolveMapThemeID = iMapThemeID
				'   miSourceMapThemeID = tMapThemeData.SourceMapThemeID
			End If
			If tMapThemeData.OverlayMapThemeID_A <> 0 Then
				'TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.Undefined
				'	MessageBox.Show(tMapThemeData.TopoName & vbCrLf & tMapThemeData.LineTopoName & vbCrLf & CStr(tMapThemeData.OverlayMapThemeID_A), "05_560")
				miOverlayAddMapThemeID = tMapThemeData.OverlayMapThemeID_A
			End If
			Try
				''''''''''''''''''''''''''''''''oDataGridRow.Cells.Item(3).Value = zzOpenThemeForm(tMapThemeData, False)


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzCreateMapThemesDictionary_2")
			End Try
		Next
		Me.ssbApplication.DropDownItems.Clear()
		If mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.UD_Parcels) AndAlso mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Fragments) OrElse mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.ParcelSettlement) Then
			miTask = enTask.Unidiv
			If Me.ssbApplication.DropDownItems.Count = 0 Then
				zzSetUnidivTask()
			End If
		ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.ParcelSettlement) Then
			miTask = enTask.Settlement
			If Me.ssbApplication.DropDownItems.Count = 0 Then
				zzSetUnidivTask()
			End If
			'ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Expropriation) AndAlso (mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Parcels)) Then
		ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Expropriation) Then
			If mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Parcels) Then
				miTask = enTask.Expropriation
			Else
				miTask = enTask.EntityConnectedLot
			End If
			zzSetExproTask()
			'		ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Expropriation) AndAlso (mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.LotApproved)) Then
			'			zzSetEntConnectedTask()
		ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.Bamash) Then
			miTask = enTask.Bamash
			zzSetBamashTask()
		ElseIf mdicMapThemes.ContainsKey(DMAcadExt.enMapTheme.LotApproved) Then
			miTask = enTask.LotApproved
			zzSetLotApprovedTask()
		Else

		End If


		TopoManager.TPlanGraph.TplnProject.MapThemes = mdicMapThemes
		DMAcadExt.AppMessages.MapThemes = mdicMapThemes
		'   System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_544")
	End Sub
	Private Sub zzSetLotApprovedTask()
		Me.smiLotApproved_CalcAreas = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiLotApproved_Points = New System.Windows.Forms.ToolStripMenuItem()

		'
		'smiLotApproved_CalcAreas
		'
		Me.smiLotApproved_CalcAreas.Name = "smiLotApproved_CalcAreas"
		Me.smiLotApproved_CalcAreas.Size = New System.Drawing.Size(142, 22)
		Me.smiLotApproved_CalcAreas.Text = "בדיקת שטחים"
		'
		'smiLotApproved_Points
		'
		Me.smiLotApproved_Points.Name = "smiLotApproved_Points"
		Me.smiLotApproved_Points.Size = New System.Drawing.Size(142, 22)
		Me.smiLotApproved_Points.Text = "נקודות"


		Me.ssbApplication.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.smiLotApproved_CalcAreas, Me.smiLotApproved_Points})
		'	Me.ssbApplication.ToolTipText = "AAAAA"

	End Sub

	Private Sub zzSetUnidivTask()
		Me.smiUnidiv_General = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiUnidiv_ProjectData = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiUnidiv_EraseHanit = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiUnidiv_EraseDB = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiUnidiv_ExportData = New System.Windows.Forms.ToolStripMenuItem()
		Me.smiUnidiv_ImportData = New System.Windows.Forms.ToolStripMenuItem()
		'
		'smiUnidiv_General
		'
		Me.smiUnidiv_General.Name = "smiUnidiv_General"
		Me.smiUnidiv_General.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_General.Text = "General"
		'
		'smiUnidiv_ProjectData
		'
		Me.smiUnidiv_ProjectData.Name = "smiUnidiv_ProjectData"
		Me.smiUnidiv_ProjectData.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_ProjectData.Text = "Project Data"
		'
		'smiUnidiv_EraseHanit
		'
		Me.smiUnidiv_EraseHanit.Name = "smiUnidiv_EraseHanit"
		Me.smiUnidiv_EraseHanit.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_EraseHanit.Text = "Erase Hanit"
		'
		'smiUnidiv_EraseDB
		'
		Me.smiUnidiv_EraseDB.Name = "smiUnidiv_EraseDB"
		Me.smiUnidiv_EraseDB.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_EraseDB.Text = "Erase DB"
		'
		'smiUnidiv_ExportData
		'
		Me.smiUnidiv_ExportData.Name = "smiUnidiv_ExportData"
		Me.smiUnidiv_ExportData.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_ExportData.Text = "Export Data"
		'
		'smiUnidiv_ImportData
		'
		Me.smiUnidiv_ImportData.Name = "smiUnidiv_ImportData"
		Me.smiUnidiv_ImportData.Size = New System.Drawing.Size(142, 22)
		Me.smiUnidiv_ImportData.Text = "Import Data"
		'
		Me.ssbApplication.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.smiUnidiv_General, Me.smiUnidiv_ProjectData, Me.smiUnidiv_EraseHanit, Me.smiUnidiv_EraseDB, Me.smiUnidiv_ExportData, Me.smiUnidiv_ImportData})
		Me.ssbApplication.ToolTipText = "חני""ת"
	End Sub
	Private Sub zzSetExproTask()
		smiExpro_EntConnectedLayer = New ToolStripMenuItem()
		'
		'smiExpro_EntConnectedLayer
		'
		Me.smiExpro_EntConnectedLayer.Name = "smiExpro_EntConnectedLayer"
		Me.smiExpro_EntConnectedLayer.Size = New System.Drawing.Size(142, 22)
		Me.smiExpro_EntConnectedLayer.Text = "שכבות מחוברים"

		If miTask = enTask.Expropriation Then
			smiExpro_EntConnected = New ToolStripMenuItem()
			'
			'smiExpro_EntConnected
			'
			Me.smiExpro_EntConnected.Name = "smiExpro_EntConnected"
			Me.smiExpro_EntConnected.Size = New System.Drawing.Size(142, 22)
			Me.smiExpro_EntConnected.Text = "מחוברים"
			Me.ssbApplication.DropDownItems.Clear()
			Me.ssbApplication.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.smiExpro_EntConnectedLayer, Me.smiExpro_EntConnected})
		ElseIf miTask = enTask.EntityConnectedLot Then
			smiExpro_EntConnectedLot = New ToolStripMenuItem()
			'smiExpro_EntConnectedLot
			Me.ssbApplication.DropDownItems.Clear()
			Me.smiExpro_EntConnectedLot.Name = "smiExpro_EntConnectedLot"
			Me.smiExpro_EntConnectedLot.Size = New System.Drawing.Size(142, 22)
			Me.smiExpro_EntConnectedLot.Text = "מחוברים"

			Me.ssbApplication.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.smiExpro_EntConnectedLayer, Me.smiExpro_EntConnectedLot})
			'	שטחים מחוברים
		End If

		'


		Me.ssbApplication.ToolTipText = "הפקעות"
	End Sub
	Private Sub zzSetEntConnectedTask()
		Me.ssbApplication.ToolTipText = "מחוברים"
	End Sub
	Private Sub zzSetLotApprovedTask_171124()
		Me.ssbApplication.ToolTipText = "נקודות"
	End Sub
	Private Sub zzSetBamashTask()
		Me.ssbApplication.ToolTipText = "במ""ש"
	End Sub

	Private Sub zzCheckMapThemes()
		Dim oRow As DataRow
		Dim iMapThemeID As DMAcadExt.enMapTheme
		Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim oDataGridRow As System.Windows.Forms.DataGridViewRow
		Dim tMapThemeData As DMAcadExt.MapThemeData

		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oRow = moPrjMapThemes.Rows.Item(iIndex)
			iMapThemeID = zzGetMapThemeID(oRow)
			tMapThemeData = New DMAcadExt.MapThemeData(oRow)
			oDataGridRow = Me.dgvPrjThemes.Rows.Item(iIndex)
			oDataGridRow.Cells.Item(3).Value = zzOpenThemeForm(tMapThemeData, False)
		Next

		Me.dgvPrjThemes.Columns.Item(3).DefaultCellStyle = moDefaultCheckGridViewCellStyle
	End Sub
	Private Sub zzExecDefaultActions()
		Dim oRow As DataRow
		Dim iMapThemeID As DMAcadExt.enMapTheme
		Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim oDataGridRow As System.Windows.Forms.DataGridViewRow
		Dim tMapThemeData As DMAcadExt.MapThemeData

		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oRow = moPrjMapThemes.Rows.Item(iIndex)
			iMapThemeID = zzGetMapThemeID(oRow)
			tMapThemeData = New DMAcadExt.MapThemeData(oRow)
			oDataGridRow = Me.dgvPrjThemes.Rows.Item(iIndex)
			oDataGridRow.Cells.Item(3).Value = zzOpenThemeForm(tMapThemeData, False)
		Next

		Me.dgvPrjThemes.Columns.Item(3).DefaultCellStyle = moDefaultCheckGridViewCellStyle


	End Sub
	Private Sub zzCheckTopos()
		Dim oRow As DataRow
		Dim iMapThemeID As DMAcadExt.enMapTheme

		'	Dim oDataGridRow As System.Windows.Forms.DataGridViewRow
		Dim tMapThemeData As DMAcadExt.MapThemeData
		Dim tTopoRes As DMAcadExt.TopoRes
		Dim fTopoStatus As frmTopoStatus = New frmTopoStatus()
		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oRow = moPrjMapThemes.Rows.Item(iIndex)
			iMapThemeID = zzGetMapThemeID(oRow)

			tMapThemeData = New DMAcadExt.MapThemeData(oRow)
			If tMapThemeData.GraphType = DMAcadExt.enGraphType.Topology Then
				tTopoRes = TopoManager.TopoCreator.CheckTopo(tMapThemeData.TopoName, True, False, tMapThemeData.MapThemeID)
				fTopoStatus.AddTopo(tTopoRes, tMapThemeData.MapThemeName, tMapThemeData.TopoName, True)
				tTopoRes = TopoManager.TopoCreator.CheckTopo(tMapThemeData.LineTopoName, True, False, tMapThemeData.MapThemeID)
				fTopoStatus.AddTopo(tTopoRes, tMapThemeData.MapThemeName, tMapThemeData.LineTopoName, False)
			End If
		Next
		fTopoStatus.ShowDialog()
		'	Me.dgvPrjThemes.Columns.Item(3).DefaultCellStyle = moDefaultCheckGridViewCellStyle

		If fTopoStatus.DialogResult = DialogResult.Yes Then
			Me.Close()
		End If

		'   System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_544")
	End Sub

	Private Sub zzSetDataBinding()
		Me.txtTopoName.DataBindings.Clear()
		Me.txtTopoName.DataBindings.Add("Text", moPrjMapThemes, "TopoName")

		Me.txtLinkLayers.DataBindings.Clear()
		Me.txtLinkLayers.DataBindings.Add("Text", moPrjMapThemes, "LinkLayers")

		Me.txtCentroidLayer.DataBindings.Clear()
		Me.txtCentroidLayer.DataBindings.Add("Text", moPrjMapThemes, "CentroidLayers")

		Me.txtCentroidBlockName.DataBindings.Clear()
		Me.txtCentroidBlockName.DataBindings.Add("Text", moPrjMapThemes, "CentroidBlocks")

		Me.txtNodeLayer.DataBindings.Clear()
		Me.txtNodeLayer.DataBindings.Add("Text", moPrjMapThemes, "NodeLayers")

		Me.txtNodeBlockName.DataBindings.Clear()
		Me.txtNodeBlockName.DataBindings.Add("Text", moPrjMapThemes, "NodeBlocks")

		Me.txtLineTopoName.DataBindings.Clear()
		Me.txtLineTopoName.DataBindings.Add("Text", moPrjMapThemes, "LineTopoName")

		Me.txtLineLinkLayer.DataBindings.Clear()
		Me.txtLineLinkLayer.DataBindings.Add("Text", moPrjMapThemes, "LineLinkLayer")

		Me.txtClosedPgonsLayer.DataBindings.Clear()
		Me.txtClosedPgonsLayer.DataBindings.Add("Text", moPrjMapThemes, "ClosedPgonsLayers")


	End Sub
	Private Sub zzSetPanelReadOnly(bReadOnly As Boolean)
		Me.txtLinkLayers.ReadOnly = bReadOnly
		Me.txtCentroidBlockName.ReadOnly = bReadOnly
		Me.txtCentroidLayer.ReadOnly = bReadOnly
		Me.txtNodeBlockName.ReadOnly = bReadOnly
		Me.txtNodeLayer.ReadOnly = bReadOnly
	End Sub
	Private Sub zzSetGridColumnsForRead()
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oColumn As DataGridViewTextBoxColumn
		Dim oChkColumn As DataGridViewCheckBoxColumn
		Dim oUI_Settings As TPlServerDB.UI_Settings = New TPlServerDB.UI_Settings(TPlServerDB.enResourceTheme.AcFrmProjectThemes, 1, True, False)
		Dim oItemSetting As TPlServerDB.ItemSetting

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		oItemSetting = oUI_Settings.GetItem(0)
		With oColumn
			.Name = ""
			.HeaderText = oItemSetting.Text
			.Width = oItemSetting.Size
			.Name = oItemSetting.ItemName
			.DataPropertyName = oItemSetting.ItemName
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvPrjThemes.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
		End Try


		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		oItemSetting = oUI_Settings.GetItem(1)
		With oColumn
			.HeaderText = oItemSetting.Text
			.Width = oItemSetting.Size
			If oItemSetting.ItemName Is Nothing Then
				.Name = "Col" & Convert.ToString(oItemSetting.Size)

			Else
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
			End If

			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvPrjThemes.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
		End Try

		' Column:

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		oItemSetting = oUI_Settings.GetItem(2)
		With oColumn
			.HeaderText = oItemSetting.Text
			.Width = oItemSetting.Size
			If oItemSetting.ItemName Is Nothing Then
				.Name = "Col" & Convert.ToString(oItemSetting.ItemNo)

			Else
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
			End If

			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvPrjThemes.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
		End Try



		oChkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		oItemSetting = oUI_Settings.GetItem(3)
		With oChkColumn
			.HeaderText = oItemSetting.Text
			.Width = oItemSetting.Size
			If oItemSetting.ItemName Is Nothing Then
				.Name = "Col" & Convert.ToString(oItemSetting.ItemNo)

			Else
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
			End If
			.FlatStyle = FlatStyle.Flat
			.SortMode = DataGridViewColumnSortMode.NotSortable
			.ThreeState = False
		End With

		Try
			Me.dgvPrjThemes.Columns.Add(oChkColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
		End Try
	End Sub

	Private Sub zzSetGridColumnsForEdit()
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oTxtColumn As DataGridViewTextBoxColumn

		Dim oCmbColumn As DataGridViewComboBoxColumn
		Dim oChkColumn As DataGridViewCheckBoxColumn
		Dim oUI_Settings As TPlServerDB.UI_Settings = New TPlServerDB.UI_Settings(TPlServerDB.enResourceTheme.AcFrmProjectThemes, 2, True, False)
		Dim oItemSetting As TPlServerDB.ItemSetting
		If oUI_Settings IsNot Nothing Then
			oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
			oItemSetting = oUI_Settings.GetItem(0)
			With oTxtColumn
				.HeaderText = oItemSetting.Text
				.Width = oItemSetting.Size
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
				.SortMode = DataGridViewColumnSortMode.NotSortable
			End With

			Try
				Me.dgvPrjThemes.Columns.Add(oTxtColumn)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
			End Try

			oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
			oItemSetting = oUI_Settings.GetItem(1)
			With oTxtColumn
				.HeaderText = oItemSetting.Text
				.Width = oItemSetting.Size
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
				.SortMode = DataGridViewColumnSortMode.NotSortable
			End With

			Try
				Me.dgvPrjThemes.Columns.Add(oTxtColumn)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
			End Try



			If False Then
				Dim oDataItems As DMCommon.DataItems
				Dim sComText As String = "SELECT [GraphTypeID],[GraphTypeName] FROM [GraphTypes]"
				Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)

				oCmbColumn = New System.Windows.Forms.DataGridViewComboBoxColumn
				oItemSetting = oUI_Settings.GetItem(1)
				With oCmbColumn
					.HeaderText = oItemSetting.Text
					.Width = oItemSetting.Size
					If oItemSetting.ItemName Is Nothing Then
						.Name = "Col" & Convert.ToString(oItemSetting.Size)

					Else
						.Name = oItemSetting.ItemName
						.DataPropertyName = oItemSetting.ItemName
					End If
					.FlatStyle = FlatStyle.Flat
					'.Items.Add( New DMCommon.ItemData()

					'	End If
					.MaxDropDownItems = 2
					.ValueMember = DMCommon.ItemData.ValueMember
					.DisplayMember = DMCommon.ItemData.DisplayMember
					If oDataReader IsNot Nothing Then
						oDataItems = New DMCommon.DataItems(oDataReader)
						.DataSource = oDataItems.ItemList
					End If


					.SortMode = DataGridViewColumnSortMode.NotSortable
				End With

				Try
					Me.dgvPrjThemes.Columns.Add(oCmbColumn)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
				End Try
			End If


			oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
			oItemSetting = oUI_Settings.GetItem(2)
			With oTxtColumn
				.HeaderText = oItemSetting.Text
				.Width = oItemSetting.Size
				.Name = oItemSetting.ItemName
				.DataPropertyName = oItemSetting.ItemName
				.SortMode = DataGridViewColumnSortMode.NotSortable
			End With

			Try
				Me.dgvPrjThemes.Columns.Add(oTxtColumn)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
			End Try


			oChkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
			oItemSetting = oUI_Settings.GetItem(3)
			With oChkColumn
				.HeaderText = oItemSetting.Text
				.Width = oItemSetting.Size
				If oItemSetting.ItemName Is Nothing Then
					.Name = "Col" & Convert.ToString(oItemSetting.Size)

				Else
					.Name = oItemSetting.ItemName
					.DataPropertyName = oItemSetting.ItemName
				End If

				.SortMode = DataGridViewColumnSortMode.NotSortable
			End With

			Try
				Me.dgvPrjThemes.Columns.Add(oChkColumn)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
			End Try
		End If
		zzSetPanelReadOnly(False)
	End Sub

	Private Sub tstMainTop_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstMainTop.ItemClicked
		Dim tMapThemeData As DMAcadExt.MapThemeData

		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbCleanup.Name
				tMapThemeData = zzGetCurrentMapThemeData()

				If tMapThemeData.IsNotEmpty Then
					'zzSetProjectData()
					'DMCommon.Debug.MsgBox("08_277", "!OK1", tMapThemeData.MapThemeName)
					zzOpenThemeForm(tMapThemeData, True)
				End If

				'''''''''''''''''''''''''''''''tMapThemeData = zzGetCurrentMapThemeData()
			Case Me.tsbEditProject.Name
				If Me.tsbEditProject.Checked Then
					zzSetProjectCodeForRead()
					Me.tsbUpdate.Enabled = False
				Else
					zzEditProject()
					Me.tsbUpdate.Enabled = True
				End If

				'zzExportToShape()
			Case Me.tsbUpdate.Name
				If Me.tsbEditProject.Checked Then
					If zzUpdate() Then
						Me.tsbEditProject.Checked = False
						Me.tsbUpdate.Enabled = False
					End If

				End If
			Case Me.tsbCheckThemes.Name
				zzCheckMapThemes()
			Case Me.tsbCalculate.Name

				'zzSetProjectData()
				'DMCommon.Debug.MsgBox("!DMAcadExt.DMApp.AppID", DMAcadExt.DMApp.AppID)
				Select Case DMAcadExt.DMApp.AppID
					Case DMAcadExt.enApplications.Taba
						zzCalculateTabaProject(0)
					Case DMAcadExt.enApplications.Ownership
						zzCalculateOwnerProject()
					Case DMAcadExt.enApplications.BN
						zzCalculateBNProject()
				End Select

			Case Me.tsbReports.Name
				zzOpenReports()
			Case Me.tsbPaint.Name

				tMapThemeData = zzGetCurrentMapThemeData()
				If tMapThemeData.IsNotEmpty Then
					zzOpenDesignForm(tMapThemeData)
				End If
			Case Me.ssbApplication.Name

				Select Case miTask
					Case enTask.Unidiv
						zzOpenUnidiv()
					Case enTask.Expropriation
						zzOpenExpro()
					Case enTask.EntityConnected
						zzOpenEntConnected()

					Case enTask.EntityConnectedLot
						zzOpenEntConnectedLot()

					Case enTask.Bamash
						tMapThemeData = zzGetCurrentMapThemeData()
						zzOpenBamash(tMapThemeData)
					Case enTask.LotApproved
						tMapThemeData = zzGetCurrentMapThemeData()
						zzOpenLotPoints(tMapThemeData)

				End Select

			Case Me.tsbToClosedPgons.Name
				tMapThemeData = zzGetCurrentMapThemeData()

				'    MessageBox.Show(tMapThemeData.TopoName, "02_374")
				If tMapThemeData.IsNotEmpty Then
					zzOpenTopoToClosedPgons(tMapThemeData)
				End If

			Case Me.tsbCheckPgons.Name
				tMapThemeData = zzGetCurrentMapThemeData()
				If tMapThemeData.IsNotEmpty Then
					zzOpenCheckPgons(tMapThemeData)
				End If
			Case Me.tsbDispTable.Name
				zzDispTpln()
			Case Me.tsbMessages.Name
				zzOpenMsgForm()
			Case Me.tsbProperties.Name
				'	Dim o As System.Object = New System.Windows.Documents.Serialization.SerializerDescriptor()
				'Dim oAssemblyInfo As Microsoft.VisualBasic.ApplicationServices.AssemblyInfo = My.Application.Info

				'DMCommon.Debug.MsgBox("13_217T", oAssemblyInfo.Version.Revision, oAssemblyInfo.Version.Build)
				TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode = miProjectCode
				TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo

				tMapThemeData = zzGetCurrentMapThemeData()
				' zzOpenPropertyForm(tMapThemeData)
				zzOpenPropertyForm()
			Case Me.tsbOpenDWG.Name
				zzOpenDWG()
			Case Me.tsbSaveDWG.Name
				'zzCheckMapThemes()
				zzCheckTopos()
			'	zzSaveDWG()

			Case Me.tsbExit.Name
				Me.Close()
		End Select
		Me.Cursor = Cursors.Default




		'''''''''''''''		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzSetProjectData(ByVal bOpenTransaction As Boolean)
		'	DMCommon.Debug.MsgBox("081120_1", "55", bOpenTransaction)
		If bOpenTransaction Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
		End If
		moDWGProjectData.OpenData(True, False)
		moDWGProjectData.UpdateData(miProjectCode, miDetailNo)


		If bOpenTransaction Then
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.Unlock()
		End If

	End Sub
	Private Sub zzDispTpln()
		' mfTplnView = New TPlanGraph.frmTplnView
		'  mfTplnView.ShowDialog()
		'  mfTplnView.Dispose()
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
			If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
				mfTplnView = New frmTplnView(zzGetCurrentMapThemeData())
				'''''''''''''''''''''''''''''''''''''''???????????????????   mfTplnView.MapThemes = mdicMapThemes
				' mfTplnView.InitMapThemeData = zzGetCurrentMapThemeData()

			End If
			If mfTplnView IsNot Nothing AndAlso Not mfTplnView.IsDisposed Then

				mfTplnView.InitMapThemeData = zzGetCurrentMapThemeData()
				'	DMCommon.Debug.MsgBox("13_014", zzGetCurrentMapThemeData(), mfTplnView.InitMapThemeData)
			End If
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)


			Me.Visible = False
		End If
	End Sub
	Private Sub zzOpenDWG()
		Dim sDWGName As String = moParams.GetStrValue(101)
		If Not String.IsNullOrEmpty(sDWGName) Then
			DMAcadExt.AcadDocument.OpenDocument(sDWGName, False)
			zzSetProject()
		End If
	End Sub

	Private Sub zzCalculateTabaProject(iRegion As Integer)
		Dim tPlanMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tApprMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tPropMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		Dim tParcelMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tBlockMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tMithamMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tMithamProxMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		Dim tParcelApprMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tParcelPropMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()


		Dim tExproMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tMerhavMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tZoneMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tExproZoneMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tExproLotMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		Dim tFragmentMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		Dim tUD_ParcelMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		Dim tOwnershipNoteMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()



		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.PlanApproved, tPlanMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tApprMapThemeData)

		'	DMCommon.Debug.MsgBox("!!tPlanMapThemeData", tApprMapThemeData.IsNotEmpty, mdicMapThemes.Count, bTest1)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotProposed, tPropMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.ParcelsXApproved, tParcelApprMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.ParcelsXProposed, tParcelPropMapThemeData)


		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Blocks, tBlockMapThemeData)
		Dim bMitham As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Mitham, tMithamMapThemeData)
		Dim bMithamProx As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.MithamProx, tMithamProxMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Merhav, tMerhavMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Zone, tZoneMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.ExproZoneOverlay, tExproZoneMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.ParcelsXExproXLots, tExproLotMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Fragments, tFragmentMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.UD_Parcels, tUD_ParcelMapThemeData)
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.OwnershipNotes, tOwnershipNoteMapThemeData)

		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Expropriation, tExproMapThemeData)

		TopoManager.TPlanGraph.TplnProject.ParcelMapThemeData = tParcelMapThemeData
		TopoManager.TPlanGraph.TplnProject.BlockMapThemeData = tBlockMapThemeData
		'	DMCommon.Debug.MsgBox("13_302e", bMitham, bMithamProx)
		If bMitham Then
			TopoManager.TPlanGraph.TplnProject.MithamMapThemeData = tMithamMapThemeData
		End If
		If bMithamProx Then
			TopoManager.TPlanGraph.TplnProject.MithamProxMapThemeData = tMithamProxMapThemeData
		End If


		'DMCommon.Debug.MsgBox("13_302c", tApprMapThemeData.MapThemeID)

		TopoManager.TPlanGraph.TplnProject.PlanMapThemeData = tPlanMapThemeData
		TopoManager.TPlanGraph.TplnProject.ApprMapThemeData = tApprMapThemeData
		TopoManager.TPlanGraph.TplnProject.PropMapThemeData = tPropMapThemeData
		TopoManager.TPlanGraph.TplnProject.ParcelApprMapThemeData = tParcelApprMapThemeData
		TopoManager.TPlanGraph.TplnProject.ParcelPropMapThemeData = tParcelPropMapThemeData

		TopoManager.TPlanGraph.TplnProject.ExproMapThemeData = tExproMapThemeData
		TopoManager.TPlanGraph.TplnProject.MerhavMapThemeData = tMerhavMapThemeData
		TopoManager.TPlanGraph.TplnProject.ZoneMapThemeData = tZoneMapThemeData
		TopoManager.TPlanGraph.TplnProject.ExproZoneMapThemeData = tExproZoneMapThemeData
		TopoManager.TPlanGraph.TplnProject.ExproLotThemeData = tExproLotMapThemeData

		TopoManager.TPlanGraph.TplnProject.MithamProxMapThemeData = tMithamProxMapThemeData



		TopoManager.TPlanGraph.TplnProject.OwnershipNoteMapThemeData = tOwnershipNoteMapThemeData





		TopoManager.TPlanGraph.TplnProject.FragmentMapThemeData = tFragmentMapThemeData

		TopoManager.TPlanGraph.TplnProject.UD_ParcelMapThemeData = tUD_ParcelMapThemeData



		'	Dim bTest As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Merhav, tMerhavMapThemeData)
		'	DMCommon.Debug.MsgBox("02_499", mdicMapThemes.Count, iRegion)



		'		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)
		Dim iColorSetID As Integer = 0
		Dim sLanduseList As String = Nothing
		' Dim iTopoPurpose As DMAcadExt.enTopoPurpose


		'		TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.Topologia
		If tExproMapThemeData.IsNotEmpty Then
			TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Expro
		ElseIf tApprMapThemeData.IsNotEmpty Then
			TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Approved
		ElseIf tPropMapThemeData.IsNotEmpty Then
			TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Proposed

		Else
			TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Undefined

		End If
		Dim bPlan As Boolean, bApproved As Boolean, bProposed As Boolean, bParcel As Boolean, bBlock As Boolean, bExpro As Boolean, bMerhav As Boolean 'bMitham As Boolean,
		'     Dim bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean
		bPlan = tPlanMapThemeData.IsNotEmpty
		bApproved = tApprMapThemeData.IsNotEmpty
		bProposed = tPropMapThemeData.IsNotEmpty
		bParcel = tParcelMapThemeData.IsNotEmpty
		bBlock = tBlockMapThemeData.IsNotEmpty
		bMitham = tMithamMapThemeData.IsNotEmpty


		bMerhav = tMerhavMapThemeData.IsNotEmpty

		If tParcelMapThemeData.IsNotEmpty Then
			bExpro = True
		Else
			bExpro = False
		End If
		bExpro = tExproMapThemeData.IsNotEmpty  ''''''''''''''''NB!


		Dim dicTopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)
		Dim oPgonSet As FDO.TplnPolygonSet = Nothing

		If bApproved Then

			If tApprMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
				oPgonSet = zzCreatePgonSet(tApprMapThemeData)
			End If
		End If
		If oPgonSet IsNot Nothing Then
			dicTopoPolygons = oPgonSet.TopoPolygons
			TopoManager.TPlanGraph.TplnProject.TopoPolygons = dicTopoPolygons
		End If

		'	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
		'	DMAcadExt.AcadDocument.WriteDebugMessage("@54  iRegion =" & iRegion.ToString())

		If iRegion <> 0 Then
			TopoManager.TPlanGraph.TplnProject.CalculateRegion(iRegion)
		Else
			TopoManager.TPlanGraph.TplnProject.Calculate()
		End If


		'  zzSaveDWG()

	End Sub
	Private Function zzCreatePgonSet(mtMapThemeData As DMAcadExt.MapThemeData) As FDO.TplnPolygonSet
		'   MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370d")
		Dim bCurrentLayerOK As Boolean = False
		Dim mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		' System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString & vbCrLf & mtSourceMapThemeData.TopoPurpose.ToString & vbCrLf & mtSourceMapThemeData.GraphType.ToString() & vbCrLf & mtSourceMapThemeData.TopoName & vbCrLf & mtSourceMapThemeData.ClosedPgonsLayers, "07_001t")
		Dim oPgonSet As FDO.TplnPolygonSet = New FDO.TplnPolygonSet(mtMapThemeData.MapThemeID, mtMapThemeData.TopoPurpose, mtMapThemeData.TopoName)
		'	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		Select Case mtMapThemeData.MapThemeID
			Case DMAcadExt.enMapTheme.Blocks
				TopoManager.TPlanGraph.TplnBlock.Initialize(mtMapThemeData)
			Case DMAcadExt.enMapTheme.Parcels
				TopoManager.TPlanGraph.TplnParcel.Initialize(mtMapThemeData)
			Case DMAcadExt.enMapTheme.UD_Parcels
				''''''''''''    UnidivNet.UD_Parcel.Initialize(mtMapThemeData)
			Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
				TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
		End Select







		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

		'    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
		' System.Windows.Forms.MessageBox.Show(mtMapThemeData.ClosedPgonsLayers & vbCrLf & "", "07_002")
		Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtMapThemeData.ClosedPgonsLayers)
		'
		Dim dicCentroids As System.Collections.Generic.IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Autodesk.AutoCAD.DatabaseServices.BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(mtMapThemeData.CentroidBlock, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)   '"pclp004"
		'
		mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers)
		DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count) & "; " & mtMapThemeData.CentroidBlocks & "; " & mtMapThemeData.CentroidLayers & "; " & mtMapThemeData.ClosedPgonsLayers)
		oPgonSet.AddPolylineIDs(colPolygonIDs)
		'    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msIntersectionPointsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
		'
		'oPgonSet.AddBlocks(dicCentroids)
		oPgonSet.AddCentroids(mcolCentroids)
		'
		'TopoManager.TPlanGraph.TplnBlock.Initialize(mtSourceMapThemeData)
		'TopoManager.TPlanGraph.TplnParcel.Initialize(mtSourceMapThemeData)
		'TopoManager.TPlanGraph.TplnLot.Initialize(mtSourceMapThemeData)
		'UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)


		'

		'   Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)

		'''''''''''''	
		oPgonSet.CalculateNewF(True)





		'  moPgonSet.InfoToExcel()


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		Return oPgonSet

	End Function

	Private Sub zzSaveDWG()
		Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
		If False AndAlso sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
			moParams.SetIdData(sFileName)
			moParams.Update()
			DMAcadExt.AcadDocument.WriteMessage("File Name: '" & sFileName & "' saved")
			DMAcadExt.AcadDocument.CloseMessage()
		End If
		DMAcadExt.AcadDocument.SaveCurrentDocument()
	End Sub
	Private Sub zzCalculateOwnerProject()
		Dim tOwnerMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)


		'MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345")
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Ownership, tOwnerMapThemeData)


		'	MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345after")
		'		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)


		'	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
		TopoManager.TPlanGraph.TplnOwnerProject.Init(tOwnerMapThemeData)
		TopoManager.TPlanGraph.TplnOwnerProject.Calculate()
		Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
		If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
			moParams.SetIdData(sFileName)
			moParams.Update()
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzCalculateBNProject()
		Dim tBNMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)


		'MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345")
		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.BN, tBNMapThemeData)


		'	MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345after")
		'		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)


		'	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
		TopoManager.TPlanGraph.TplnBNProject.Init(tBNMapThemeData)
		TopoManager.TPlanGraph.TplnBNProject.Calculate()
		Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
		If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
			moParams.SetIdData(sFileName)
			moParams.Update()
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzOpenMsgForm()
		'	Dim fMapThemeBase As frmMapThemeBase = Nothing

		Try
			mfMessages = New frmMessages()
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfMessages)
			Me.Visible = False
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzOpenMsgForm")
		End Try
	End Sub

	Private Function zzOpenThemeForm(tMapThemeData As DMAcadExt.MapThemeData, ByVal bLoad As Boolean) As Boolean
		Dim bIsDone As Boolean = False
		DMCommon.Debug.MsgBox("05_342a", tMapThemeData.GraphType.ToString(), CStr(tMapThemeData.MapThemeID))

		mfMapThemeBase = Nothing
		Try
			Select Case tMapThemeData.GraphType
				Case DMAcadExt.enGraphType.Topology
					'	mfTopoCleanup = New frmTopoCleanup(tMapThemeData)
					mfMapThemeBase = New frmTopoCleanup(tMapThemeData)

					'	fMapThemeBase = mfTopoCleanup

				Case DMAcadExt.enGraphType.ClosedPolygons
					DMCommon.Debug.MsgBox("05_355c", tMapThemeData.GraphType.ToString() & vbCrLf & tMapThemeData.MapThemeID.ToString())
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
					Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
					If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
						'MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & CStr(tMapThemeData.MapThemeID), "05_361")
						mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)      '''''''''''''''''' Prev Version 	
						'''''''''''''''''' Design  VersionmfMapThemeBase = New frmPgonSet(tSourceMapThemeData) 
					Else
						mfMapThemeBase = New frmClosedPgons(tMapThemeData)
					End If
				Case DMAcadExt.enGraphType.TopologyList
					mfMapThemeBase = New frmTopoListCleanup(tMapThemeData)
				Case DMAcadExt.enGraphType.Undefined
					MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & CStr(tMapThemeData.MapThemeID), "05_979")
					Dim tAddMapThemeData As DMAcadExt.MapThemeData = Nothing

				Case DMAcadExt.enGraphType.MapLayerOverlay
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					Dim tOverlayMapThemeData_A As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
						If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
							If tMapThemeData.OverlayMapThemeID_A <> 0 Then
								mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID_A, tOverlayMapThemeData_A)
							End If
							'	DMCommon.Debug.MsgBox("07_510", tMapThemeData.MapThemeID, tMapThemeData.SourceMapThemeID.ToString(), tMapThemeData.OverlayMapThemeID.ToString())
							mfMapThemeBase = New frmFDO_Overlay(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData, tOverlayMapThemeData_A)
						End If
					End If
				Case DMAcadExt.enGraphType.AnalyticClip
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					Dim tOverlayMapThemeData_A As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
						If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
							If tMapThemeData.OverlayMapThemeID_A <> 0 Then
								mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID_A, tOverlayMapThemeData_A)
							End If
							mfMapThemeBase = New frmAnaliticClip(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData, tOverlayMapThemeData_A)
						End If
					End If
				Case DMAcadExt.enGraphType.TopoOverlay
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
					'  MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString() & vbCrLf & mdicMapThemes.Count, "05_400")
					If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
						If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
							'  System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString() & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.TopoName, "TOPONAME") & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.LineTopoName, "?LineTopoName"), "07_520")
							mfMapThemeBase = New frmTopoOverlay(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData)
						End If
					End If
			End Select
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenThemeForm")
		End Try


		If mfMapThemeBase IsNot Nothing AndAlso Not mfMapThemeBase.IsDisposed Then
			bIsDone = mfMapThemeBase.IsDone
		End If

		If bLoad AndAlso (mfMapThemeBase IsNot Nothing) AndAlso (Not mfMapThemeBase.IsDisposed) Then
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfMapThemeBase)
			Me.Visible = False
		End If
		Return bIsDone
	End Function
	Private Function zzGetDissolveMapThemeData(tMapThemeData As DMAcadExt.MapThemeData, ByRef tSourceMapThemeData As DMAcadExt.MapThemeData, ByRef tDissolveMapThemeData As DMAcadExt.MapThemeData) As Boolean
		Dim iDissolveMapThemeID As DMAcadExt.enMapTheme
		Dim bRes As Boolean = False
		'	DMCommon.Debug.MsgBox("02_372", tMapThemeData.SourceMapThemeID, DMAcadExt.enMapTheme.Undefined, tMapThemeData.TopoName, tMapThemeData.ClosedPgonsLayer, tDissolveMapThemeData.TopoName)
		If tMapThemeData.SourceMapThemeID = DMAcadExt.enMapTheme.Undefined Then
			If mdicDissolves.TryGetValue(tMapThemeData.MapThemeID, iDissolveMapThemeID) AndAlso mdicMapThemes.TryGetValue(iDissolveMapThemeID, tDissolveMapThemeData) Then
				tSourceMapThemeData = tMapThemeData
				bRes = True
			End If
		Else
			If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
				tDissolveMapThemeData = tMapThemeData
				bRes = True
			End If

		End If





		Return bRes

	End Function


	Private Function zzOpenDesignForm(tMapThemeData As DMAcadExt.MapThemeData) As Boolean
		'	Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim bIsDone As Boolean = False
		Try
			'DMCommon.Debug.MsgBox("13_128m", tMapThemeData.DesignElement, tMapThemeData.TopoName, tMapThemeData.MapThemeID)
			Select Case tMapThemeData.DesignElement
				Case 1
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
					Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
					If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
						'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

					End If
					' MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tMapThemeData.GraphType.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")
					'	DMCommon.Debug.MsgBox("12_502", tMapThemeData.MapThemeID, tMapThemeData.MapThemeName, tDissolveMapThemeData.MapThemeName)
					mfDesign = New frmPaintLanduse(tMapThemeData, tDissolveMapThemeData)
				Case 2
					If TopoManager.TPlanGraph.TplnProject.Parcels Is Nothing OrElse TopoManager.TPlanGraph.TplnProject.Parcels.Count = 0 Then
						''''''''''''''''zzCalculateTabaProject(0)
					End If

					If TopoManager.TPlanGraph.TplnProject.Parcels IsNot Nothing AndAlso TopoManager.TPlanGraph.TplnProject.Parcels.Count > 0 Then
						mfOwnership = New frmOwnership()
						mfDesign = mfOwnership
					End If
				Case 3
					Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
					Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
					If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
						'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

					End If
					' MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tMapThemeData.GraphType.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")
					'    DMCommon.Debug.MsgBox("12_502", tMapThemeData.MapThemeName, tDissolveMapThemeData.MapThemeName)
					mfDesign = New frmPaintLanduse(tMapThemeData, tDissolveMapThemeData)
			End Select
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenDesignForm")
		End Try
		If mfDesign IsNot Nothing AndAlso Not mfDesign.IsDisposed Then
			Try
				'	DMCommon.Debug.MsgBox("12_502d", tMapThemeData.MapThemeName)
				Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
				Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDesign)
				Me.Visible = False
				mfDesign.Owner = Me
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenDesignForm_1")
			End Try
		End If
		Return bIsDone
	End Function

	Private Function zzOpenPropertyForm() As Boolean
		'	Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim bIsDone As Boolean = False
		Try
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()

			zzOpenProjectData()
			Dim oAssemblyInfo As Microsoft.VisualBasic.ApplicationServices.AssemblyInfo = My.Application.Info

			DMCommon.Debug.MsgBox("13_217K", oAssemblyInfo.Version.Major, oAssemblyInfo.Version.Minor, oAssemblyInfo.Version.MajorRevision, oAssemblyInfo.Version.MinorRevision, oAssemblyInfo.Version.Build, oAssemblyInfo.Version.Revision)




			Dim oTopoPropertyView As TopoManager.TopoPropertyView = moDWGProjectData.GetPropertyView()
			oTopoPropertyView.SetProgram(oAssemblyInfo.Version, Date.Today)
			mfProperty = New frmMapThemeProperty(oTopoPropertyView)
			DMAcadExt.AcadTransaction.Terminate()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm")
		End Try
		If mfProperty IsNot Nothing Then
			Try
				Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
				Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProperty)
				Me.Visible = False
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm_1")
			End Try
		End If
		DMAcadExt.AcadTransaction.Terminate()

		DMAcadExt.AcadDocument.Unlock()

		Return bIsDone
	End Function
	Private Function zzOpenPropertyFormOld(tMapThemeData As DMAcadExt.MapThemeData) As Boolean
		'	Dim fMapThemeBase As frmMapThemeBase = Nothing
		Dim bIsDone As Boolean = False
		Try

			'	MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")
			mfProperty = New frmMapThemeProperty(tMapThemeData)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm")
		End Try
		If mfProperty IsNot Nothing Then
			Try
				Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
				Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProperty)
				Me.Visible = False
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm_1")
			End Try
		End If
		Return bIsDone
	End Function

	Private Sub zzOpenTopoToClosedPgons(tMapThemeData As DMAcadExt.MapThemeData)
		'Dim bIsDone As Boolean = False
		Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
		Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
		If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
			'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)
			'  MessageBox.Show(tMapThemeData.TopoName, "02_376")
			'   MessageBox.Show(DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.LinkLayer, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.LinkLayer, "Nothing"), "02_364")
		Else
			tSourceMapThemeData = tMapThemeData
		End If
		Try

			DMCommon.Debug.MsgBox("02_370", tMapThemeData.TopoName, tSourceMapThemeData.TopoName, tSourceMapThemeData.ClosedPgonsLayer, tDissolveMapThemeData.TopoName, tDissolveMapThemeData.LinkLayer, tDissolveMapThemeData.ClosedPgonsLayer)
			mfTopoToClosedPgons = New frmTopoToClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoToClosedPgons)
			Me.Visible = False
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenTopoToClosedPgons")
		End Try
	End Sub
	Private Sub zzOpenCheckPgons(tMapThemeData As DMAcadExt.MapThemeData)
		'Dim bIsDone As Boolean = False
		Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
		Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
		Dim bDissolve As Boolean
		'  MessageBox.Show(tMapThemeData.TopoName, "02_376")
		If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
			'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

		Else
			tSourceMapThemeData = tMapThemeData
		End If

		mfrmPgonSetView = New frmPgonSetView(tMapThemeData, tDissolveMapThemeData, bDissolve)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfrmPgonSetView)
		Me.Visible = False
	End Sub
	Private Sub zzOpenCheckPgonsOld(tMapThemeData As DMAcadExt.MapThemeData)
		'Dim bIsDone As Boolean = False
		Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
		Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
		MessageBox.Show(tMapThemeData.TopoName, "02_377")
		If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
			'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

		Else
			tSourceMapThemeData = tMapThemeData
		End If
		Try
			'   MessageBox.Show(DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.LinkLayer, "Nothing"), "02_370")
			Select Case tSourceMapThemeData.MapThemeID
				Case DMAcadExt.enMapTheme.Parcels, DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
					mfrmPgonSetView = New frmPgonSetView(tSourceMapThemeData, tDissolveMapThemeData, False)
					Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
					Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfrmPgonSetView)
					Me.Visible = False

			End Select

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenTopoToClosedPgons")
		End Try
	End Sub
	Private Sub zzOpenOwnership()
		Me.Cursor = Cursors.WaitCursor
		If mfOwnership Is Nothing Then
			mfOwnership = New frmOwnership()  'Autodesk.AutoCAD.ApplicationServices.Application.MainWindow
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfOwnership)
		Me.Visible = False
		mfOwnership.Owner = Me

		Me.Cursor = Cursors.Default
	End Sub
	Private Sub zzOpenOwnershipAB()
		Me.Cursor = Cursors.WaitCursor
		If mfDesign Is Nothing Then
			'mfDesign.Name
			mfDesign = New frmOwnership()  'Autodesk.AutoCAD.ApplicationServices.Application.MainWindow
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDesign)
		Me.Visible = False
		mfDesign.Owner = Me

		Me.Cursor = Cursors.Default
	End Sub


	Private Sub zzOpenUnidiv()
		mfUnidiv = New frmUnidiv(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUnidiv)
		Me.Visible = False

	End Sub
	Private Sub zzOpenEntConnected()
		mfEntConnected = New Expro.frmEntConnected()
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEntConnected)
		Me.Visible = False

	End Sub

	Private Sub zzOpenEntConnectedLot()
		mfEntConnectedLot = New Expro.frmEntConnectedLot
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEntConnectedLot)
		Me.Visible = False

	End Sub

	Private Sub zzOpenExpro()
		mfExpro = New Expro.frmExpro()
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfExpro)
		Me.Visible = False
	End Sub
	Private Sub zzOpenEditLayerList()
		mfEditLayerList = New Expro.frmEditLayerList
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditLayerList)
		Me.Visible = False
	End Sub



	Private Sub zzOpenBamash(tMapThemeData As DMAcadExt.MapThemeData)
		mfBamash = New frmBamashM(tMapThemeData)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfBamash)
		Me.Visible = False
	End Sub

	Private Sub zzOpenLotPoints(tMapThemeData As DMAcadExt.MapThemeData)
		mfLotPoints = New frmLotPoints(tMapThemeData)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'	DMCommon.Debug.MsgBox("11_090x", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfLotPoints)
		Me.Visible = False
	End Sub
	Private Sub zzOpenUD_General()
		mfUD_General = New frmUD_General(True)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
		Me.Visible = False

	End Sub
	Private Sub zzOpenUD_ProjectData()
		mfUD_ProjectData = New frmUD_ProjectData()
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_ProjectData)
		Me.Visible = False

	End Sub
	Private Sub zzOpenUD_SelectPlan()
		mfUD_SelectPlan = New frmUD_SelectPlan(Nothing)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_SelectPlan)
		Me.Visible = False

	End Sub




	Private Sub zzOpenReports()

		'MessageBox.Show(CStr(oMapThemeData.CleanupType), "05_100")

		mfReports = New frmReports()
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfReports)
		Me.Visible = False
		If mfOwnership IsNot Nothing Then
			mfReports.FormOwnership = mfOwnership
		End If

	End Sub
	Private Sub zzEditProject()

		Dim sComText As String = "SELECT [MapThemeID],[MapThemeName],[GraphTypeID],[GraphTypeName],[GraphTypeShortName],[GroupID],[FileName],[SelRow],[TopoName],[LinkLayers],[CentroidBlocks],[CentroidLayers],[ClosedPgonsLayers],[LineTopoName],[LineLinkLayer],[LineTolerance],[NodeBlocks],[NodeLayers],[MPgonLayers],[DissolveTopoName],[DissolveAttribExpr],[DissolveClosedPgonsLayers],[CleanupType],[LineCleanupType],[DesignSet],[SourceMapThemeID],[OverlayMapThemeID],[OverlayMapThemeID_A],[SPointsBlocks],[SPointsLayers] FROM PrjMapThemesExt WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(miDetailNo) & ") ORDER BY [MapThemeName],[GraphTypeName]"

		moPrjMapThemes = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "PrjMapThemes")
		'   MessageBox.Show(CStr(moPrjMapThemes.Rows.Count), "05_110")
		Me.dgvPrjThemes.Columns.Clear()
		miCurrentRow = -1
		Me.dgvPrjThemes.DataSource = moPrjMapThemes


		sComText = "AddPrjThemes" '"GetProjectData"	'"SELECT * FROM GetProjectData" '
		Dim oaParams(1) As Common.DbParameter
		oaParams(0) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText, CommandType.StoredProcedure, oaParams)
		Dim i As Integer = -1
		'	Dim oValue As System.Object
		Dim sFieldName As String
		If oDataReader IsNot Nothing Then
			i = 0
			Dim oDataRow As DataRow
			While oDataReader.Read
				i += 1
				oDataRow = moPrjMapThemes.NewRow()
				For iFieldIndex As Integer = 0 To oDataReader.FieldCount - 1
					If Not oDataReader.IsDBNull(iFieldIndex) Then
						sFieldName = oDataReader.GetName(iFieldIndex)
						oDataRow.Item(sFieldName) = oDataReader.GetValue(iFieldIndex)
					End If
				Next

				oDataRow.Item("SelRow") = 0
				moPrjMapThemes.Rows.Add(oDataRow)
			End While
			oDataReader.Close()
			zzSaveLastProjectSetting()
		End If

		''''''''''''	System.Windows.Forms.MessageBox.Show(CStr(i), "iii")
		''''''''''''''''	Me.dgvPrjThemes.Columns.Clear()
		zzSetGridColumnsForEdit()
		Me.bnsPrjThemes.DataSource = moPrjMapThemes
		zzSetDataBinding()
		Me.dgvPrjThemes.ReadOnly = False
		zzSetReadOnly(False)
	End Sub


	Private Function zzGetCurrentMapThemeData() As DMAcadExt.MapThemeData
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvPrjThemes.CurrentCell
		If oCurrentCell IsNot Nothing Then
			moCurrentRowIndex = oCurrentCell.RowIndex
			Return zzGetMapThemeData(oCurrentCell.RowIndex)
		Else
			Return New DMAcadExt.MapThemeData()
		End If

	End Function

	Private Function zzGetMapThemeData(iRowIndex As Integer) As DMAcadExt.MapThemeData
		Dim oDataRow As DataRow = moPrjMapThemes.Rows.Item(iRowIndex)
		Dim iMapThemeID As DMAcadExt.enMapTheme = zzGetMapThemeID(oDataRow)

		'
		Dim tMapThemeData As DMAcadExt.MapThemeData = Nothing

		mdicMapThemes.TryGetValue(iMapThemeID, tMapThemeData)
		'	DMCommon.Debug.MsgBox("08_308", iMapThemeID, mdicMapThemes.Count, tMapThemeData.TopoName, tMapThemeData.ClosedPgonsLayer)

		Return tMapThemeData
	End Function

	Private Sub dgvPrjThemes_RowEnter(oSender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvPrjThemes.RowEnter
		If miCurrentRow <> e.RowIndex Then
			miCurrentRow = e.RowIndex

			'''''''''''''''temp		Me.txtLinkLayers.Text = DMCommon.Functions.CStrN(moPrjMapThemes.Rows.Item(miCurrentRow).Item("LinkLayers"))
		End If
	End Sub

	Private Function zzUpdate() As Boolean
		Dim sComText As String = "SELECT * FROM PrjMapThemes"
		Dim oDataAdapter As Common.DbDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, CommandType.Text, , , True, True)
		Dim oDestDataTable As DataTable = New DataTable("Project")
		Dim oSourceRow As DataRow
		Dim oNewRow As DataRow
		Dim bSelRow As Boolean = True
		Dim sFieldName As String
		Dim oValue As System.Object
		Dim bError As Boolean
		oDataAdapter.FillSchema(oDestDataTable, SchemaType.Source)
		'		DMCommon.Functions.DispDataTableCols(oDestDataTable, "oDestDataTable", True)
		'		DMCommon.Functions.DispDataTableCols(moPrjMapThemes, "moPrjMapThemes", True)

		For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
			oSourceRow = moPrjMapThemes.Rows.Item(iIndex)
			bSelRow = DirectCast(oSourceRow.Item("SelRow"), Boolean)

			If bSelRow Then
				oNewRow = oDestDataTable.NewRow()
				For iFieldIndex As Integer = 0 To oDestDataTable.Columns.Count - 1
					sFieldName = oDestDataTable.Columns.Item(iFieldIndex).ColumnName
					Select Case sFieldName
						Case "ProjectCode"
							oNewRow.Item(sFieldName) = miProjectCode
						Case "Detail"
							oNewRow.Item(sFieldName) = miDetailNo
						Case "ClosedPgonsLayers"
							oValue = oSourceRow.Item(sFieldName)
							If Not IsDBNull(oValue) Then
								oNewRow.Item(sFieldName) = oValue
							End If
						Case Else
							'
							oValue = oSourceRow.Item(sFieldName)
							'   DMCommon.Debug.MsgBox("09_864s", sFieldName, oValue)
							If Not IsDBNull(oValue) Then
								oNewRow.Item(sFieldName) = oValue
							End If

					End Select
				Next
				Try
					oDestDataTable.Rows.Add(oNewRow)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes-zzUpdate1")
					bError = True
				End Try

			End If
		Next
		If Not bError AndAlso zzEraseProject() Then
			Try
				oDataAdapter.Update(oDestDataTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzUpdate")
				bError = True
			End Try
			If Not bError Then
				zzSetProjectCodeForRead()
			End If

		End If
		Return Not bError
	End Function



	Private Function zzEraseProject() As Boolean
		Dim oCommandErr As System.Data.Common.DbException = Nothing
		Dim sComText As String = "DELETE FROM dbo.PrjMapThemes WHERE (ProjectCode = " & Convert.ToString(miProjectCode) & ") AND (Detail = " & Convert.ToString(miDetailNo) & ")"
		If TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text, , oCommandErr) = -1 Then
			Return False
		Else
			Return True
		End If

	End Function



	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub






	Private Sub frmPrjThemes_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		DMAcadExt.AppMessages.ClearAll()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		TopoManager.TPlanGraph.TplnProject.Close()

	End Sub
	Private Sub zzClearCheckColumn()
		Dim oDataGridRow As System.Windows.Forms.DataGridViewRow
		For iIndex As Integer = 0 To dgvPrjThemes.Rows.Count - 1

			oDataGridRow = dgvPrjThemes.Rows.Item(iIndex)
			'DMCommon.Debug.MsgBox("13_134x", i, oDataGridRow.Cells.Item(3).Value)
			oDataGridRow.Cells.Item(3).Value = CheckState.Unchecked
			'	DMCommon.Debug.MsgBox("13_134y", i, CheckState.Indeterminate, oDataGridRow.Cells.Item(3).Value)
		Next
	End Sub

	Private Sub frmPrjThemes_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		zzSetProject()
		' MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "03_117a")


	End Sub
	Private Sub mfMapThemeBase_FormClosed(oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfMapThemeBase.FormClosed
		zzOnFormClose()
	End Sub
	Private Sub tstProjectCode_KeyDown(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tstProjectCode.KeyDown
		'	frmPrjThemes.vb:line 1338
		If e.KeyCode = Keys.Escape Then
			zzEscProjectCode()
		ElseIf e.KeyCode = Keys.Enter Then
			Me.dgvPrjThemes.Focus()
		End If
	End Sub
	Private Sub zzEscProjectCode()
		Dim iInputProjectCode As Integer = zzGetInputProjectCode()
		If miProjectCode = 0 Then
			Me.tstProjectCode.Text = String.Empty
		ElseIf iInputProjectCode <> miProjectCode Then
			Me.tstProjectCode.Text = Convert.ToString(miProjectCode)
		End If
		MessageBox.Show(CStr(iInputProjectCode) & ":" & CStr(miProjectCode) & ":" & Me.tstProjectCode.Text, "01_740")
	End Sub
	Private Sub ddbDetails_Click(sender As System.Object, e As System.EventArgs) Handles ddbDetails.Click
		If miDetailRowsCount = 0 AndAlso moPrjMapThemes.Rows.Count > 0 Then
			zzEditDetails()
		End If

	End Sub
	Private Sub zzEditDetails()
		Dim fEditDetails As frmEditDetails = New frmEditDetails(miProjectCode)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fEditDetails)
		If fEditDetails.DialogResult = Windows.Forms.DialogResult.OK Then
			zzSetDetails()
		End If
	End Sub

	Private Sub zzSetDetails()
		Dim sComText As String = "SELECT Detail, DetailName FROM PrjDetails WHERE ProjectCode=" & CStr(miProjectCode)
		Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
		Dim oDataRow As DataRow
		Dim iDetailNo As Integer
		Dim sDetailNo As String
		Dim bCheck As Boolean
		Me.ddbDetails.DropDownItems.Clear()
		If oDataTable IsNot Nothing Then
			miDetailRowsCount = oDataTable.Rows.Count
			If miDetailRowsCount > 0 Then
				ReDim moDetailMenuItems(miDetailRowsCount + 2)
				moDetailMenuItems(0) = New DetailMenuItem(0, , (miDetailNo = 0))
				'	zzAddDetailMenuItem(0, "DetailItem_0", " - ", (miDetailNo = 0))
				For iIndex As Integer = 0 To miDetailRowsCount - 1

					oDataRow = oDataTable.Rows.Item(iIndex)
					iDetailNo = DirectCast(oDataRow.Item(0), Integer)
					sDetailNo = Convert.ToString(iDetailNo)
					bCheck = (miDetailNo = iDetailNo)

					'	moDetailMenuItems(iIndex + 1) = New DetailMenuItem(iIndex + 1, oDataRow.Item(1).ToString(), bCheck)
					moDetailMenuItems(iIndex + 1) = New DetailMenuItem(iDetailNo, oDataRow.Item(1).ToString(), bCheck)

					'	MessageBox.Show(sItemName, "05_477")
					'zzAddDetailMenuItem(iIndex + 1, "DetailItem_" & sDetailNo, sDetailNo & " - " & oDataRow.Item(1).ToString(), bCheck)
				Next
				moDetailMenuItems(miDetailRowsCount + 1) = New System.Windows.Forms.ToolStripSeparator
				moDetailMenuItems(miDetailRowsCount + 2) = New DetailMenuItem()
				'	zzAddDetailMenuItem(miDetailRowsCount + 1, "DetailItemEdit", "Add/Edit")
				Me.ddbDetails.DropDownItems.AddRange(moDetailMenuItems)
				zzAddPrjStr(CStr(miProjectCode))

			Else
				'DMCommon.Debug.MsgBox("060920_1", miDetailNo)
				miDetailNo = 0
				Me.ddbDetails.DropDownItems.Add(New DetailMenuItem(-1))

				zzDispDetailNo()
			End If
		Else
			DMCommon.Debug.MsgBox("060920_2", miDetailNo)
			miDetailNo = 0
			Me.ddbDetails.DropDownItems.Add(New DetailMenuItem(-1))
			zzDispDetailNo()
		End If
	End Sub
	Private Sub zzAddPrjStr(sPrjStr As String)
		Dim sComText As String = "AddUserPrjStr" '"GetProjectData"	'"SELECT * FROM GetProjectData" '
		Dim oaParams(1) As Common.DbParameter
		oaParams(0) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prUserName", DbType.String, System.Environment.UserName)
		oaParams(1) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prPrjStr", DbType.String, sPrjStr)
		TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.StoredProcedure, oaParams)

	End Sub
	Private Sub zzAddDetailMenuItem(iIndex As Integer, sName As String, sText As String, Optional bChecked As Boolean = False)
		'	moDetailMenuItems(iIndex) = New System.Windows.Forms.ToolStripMenuItem()
		With moDetailMenuItems(iIndex)
			.Name = sName
			.Size = New System.Drawing.Size(152, 22)
			.Text = sText
			'''''''''''''''''''''''		.Checked = True ' bChecked
		End With
		'	AddHandler moDetailMenuItems(iIndex).Click, AddressOf Details_DropDownItemClicked
	End Sub
	Private Sub ddbDetails_DropDownItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbDetails.DropDownItemClicked
		Dim oToolStripMenuItem As DetailMenuItem = DirectCast(e.ClickedItem, DetailMenuItem)
		Dim sItemName As String = oToolStripMenuItem.Name
		'MessageBox.Show(sItemName, "05_430")
		Select Case sItemName
			Case DetailMenuItem.EditItemName
				zzEditDetails()
			Case Else
				miDetailNo = oToolStripMenuItem.DetailNo
				TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo
				zzDispDetailNo()
				zzSetMenuItemChecked()
				zzSetProjectCodeForRead()
				zzSetProjectData(True)
				'	MessageBox.Show(e.ClickedItem.GetType().ToString() & vbCrLf & sItemName & ":" & oToolStripMenuItem.Checked.ToString(), "04_329")
		End Select

	End Sub
	Private Sub zzSetMenuItemChecked()
		Dim oDetailMenuItem As DetailMenuItem
		'	Dim s As String = ""
		'	MessageBox.Show(CStr(moDetailMenuItems.GetUpperBound(0)) & ":" & CStr(miDetailNo), "05_300")
		For iIndex As Integer = 0 To moDetailMenuItems.GetUpperBound(0)
			If moDetailMenuItems(iIndex).GetType().ToString() <> "System.Windows.Forms.ToolStripSeparator" Then
				oDetailMenuItem = DirectCast(moDetailMenuItems(iIndex), DetailMenuItem)
				oDetailMenuItem.SetChecked(miDetailNo)
				'	s &= CStr(oDetailMenuItem.Checked) & vbCrLf
			End If

			'	MessageBox.Show(moDetailMenuItems(iIndex).GetType().ToString(), "03_210")
			'''''''''''''moDetailMenuItems(iIndex).SetChecked(miDetailNo)
		Next
		'	MessageBox.Show(s, "05_301")
	End Sub
	Private Class DetailMenuItem
		Inherits System.Windows.Forms.ToolStripMenuItem
		Public Const EditItemName As String = "DetailItemEdit"
		Const msNamePrefix As String = "DetailItem"
		Const msNameDelim As String = "_"

		Const msDefaultItemText As String = " - "
		Const msEditItemText As String = "Add/Edit"
		Private Shared mtSize As System.Drawing.Size = New System.Drawing.Size(152, 22)
		Private miDetailNo As Integer
		Public Sub New(Optional iIndex As Integer = -1, Optional sText As String = "", Optional bChecked As Boolean = False)
			miDetailNo = iIndex
			Dim sName As String
			Select Case iIndex
				Case -1
					sName = EditItemName
					sText = msEditItemText
				Case 0
					sName = zzGetNumName(iIndex)
					sText = msDefaultItemText
				Case Else
					sName = zzGetNumName(iIndex)
					sText = Convert.ToString(iIndex) & " - " & sText
			End Select
			MyBase.Name = sName
			MyBase.Text = sText
			MyBase.Size = mtSize
			MyBase.Checked = bChecked

		End Sub
		Public ReadOnly Property DetailNo As Integer
			Get
				Return miDetailNo
			End Get
		End Property
		Public Sub SetChecked(iCurrentDetailNo As Integer)
			If miDetailNo = iCurrentDetailNo Then
				Me.Checked = True
			Else
				Me.Checked = False
			End If

		End Sub
		Private Function zzGetNumName(iIndex As Integer) As String
			Return msNamePrefix & msNameDelim & Convert.ToString(iIndex)
		End Function
	End Class






	Private Sub mfDesign_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfDesign.FormClosed

		Me.Visible = True
	End Sub


	Private Sub mfMapThemeBase_ToClose() Handles mfMapThemeBase.OnClose
		zzOnFormClose()
	End Sub
	Private Sub zzOnFormClose()

		Me.Visible = True

		'DirectCast(moCurrentDataRow.Item(3), Boolean)	' oDataGridRow.Cells.Item(3).Value
		Dim bNew As Boolean = mfMapThemeBase.IsDone
		'	MessageBox.Show(CStr(moCurrentRowIndex) & ":" & CStr(bNew), "04_777")

		Dim oDataGridRow As System.Windows.Forms.DataGridViewRow = Me.dgvPrjThemes.Rows.Item(moCurrentRowIndex)
		oDataGridRow.Cells.Item(3).Value = bNew
		oDataGridRow.Cells.Item(3).Style = moDefaultCheckGridViewCellStyle
	End Sub

	Private Sub mfUnidiv_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfUnidiv.FormClosed
		' TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL(True)
		Me.Visible = True
	End Sub
	Private Sub mfExpro_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfExpro.FormClosed
		' TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL(True)
		Me.Visible = True
	End Sub
	Private Sub mfUD_General_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfUD_General.FormClosed
		Me.Visible = True
	End Sub
	Private Sub mfUD_ProjectData_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfUD_ProjectData.FormClosed
		Me.Visible = True
	End Sub



	Private Sub mfReports_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfReports.FormClosed
		Me.Visible = True
	End Sub

	Private Sub mfMessages_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfMessages.FormClosed
		Me.Visible = True
	End Sub
	Private Sub mfTopoToClosedPgons_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfTopoToClosedPgons.FormClosed
		Me.Visible = True
	End Sub



	Private Sub mfProperty_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfProperty.FormClosed
		Me.Visible = True
	End Sub


	Private Sub mfrmPgonSetView_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfrmPgonSetView.FormClosed
		Me.Visible = True
	End Sub




	Private Sub frmPrjThemes_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
		Me.dgvPrjThemes.Height = Me.Panel1.Location.Y - Me.dgvPrjThemes.Location.Y
	End Sub




	Private Sub mfTplnView_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles mfTplnView.FormClosing
		Me.Visible = True
	End Sub

	Private Function AllNames(sTopoName As String) As Boolean
		Return True

	End Function

	Private Sub ddbUtilities_DropDownItemClicked(oSender As System.Object, e As ToolStripItemClickedEventArgs) Handles ddbUtilities.DropDownItemClicked
		MessageBox.Show(e.ClickedItem.Name, "04_328")
		Dim tLotMapThemeData As DMAcadExt.MapThemeData = Nothing
		Dim tPlanMapThemeData As DMAcadExt.MapThemeData = Nothing

		Select Case e.ClickedItem.Name
			Case Me.tmiCreateAllTopologies.Name
				zzExecDefaultActions()
			Case Me.tmiEraseAllTopologies.Name
				Dim pTopoNameCriteria As TopoManager.TopoCreator.TopoNameCriteria = New TopoManager.TopoCreator.TopoNameCriteria(AddressOf AllNames)
				Try
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
					If DMAcadExt.AcadDocument.IsLocked Then
						DMAcadExt.AcadTransaction.Start()
						TopoManager.TopoCreator.DeleteTopologies(pTopoNameCriteria)
						DMAcadExt.AcadTransaction.Terminate()
						DMAcadExt.AcadDocument.Unlock()
					End If



					Dim oDataGridRow As System.Windows.Forms.DataGridViewRow

					For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
						oDataGridRow = Me.dgvPrjThemes.Rows.Item(iIndex)
						oDataGridRow.Cells.Item(3).Value = False
					Next
					Me.dgvPrjThemes.Columns.Item(3).DefaultCellStyle = moDefaultCheckGridViewCellStyle

				Catch oEx As Exception

				End Try

			Case Me.tmiPlanName.Name

				If mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tLotMapThemeData) Then
					MessageBox.Show(tLotMapThemeData.TopoName, "04_329")
				End If
				If mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.PlanApproved, tPlanMapThemeData) Then
					MessageBox.Show(tPlanMapThemeData.TopoName, "04_331")
				End If
				TopoManager.TPlanGraph.TplnProject.UpdateLotsByPlans(DMAcadExt.enTopoPurpose.Approved, tPlanMapThemeData.TopoName, tLotMapThemeData.TopoName)
			Case Me.tmiRegion.Name

				TopoManager.TPlanGraph.TplnProject.UpdateLotsByRegions(DMAcadExt.enTopoPurpose.Approved)

		End Select
	End Sub


	Private Sub mfReports_Recalculate(iRegion As Integer) 'Handles mfReports.Recalculate
		'	zzCalculateTabaProject(iRegion)
		'miCurrentRegionNo = iRegion
	End Sub


	Private Sub smiUnidiv_General_Click(oSender As System.Object, e As EventArgs) Handles smiUnidiv_General.Click
		zzOpenUD_General()
	End Sub

	Private Sub smiUnidiv_ProjectData_Click(oSender As System.Object, e As EventArgs) Handles smiUnidiv_ProjectData.Click
		'	zzOpenUD_ProjectData()
		zzOpenUD_SelectPlan()
	End Sub
	Private Sub smiLotApproved_CalcAreas_Click(oSender As System.Object, e As EventArgs) Handles smiLotApproved_CalcAreas.Click
		zzCalcAreas()
	End Sub
	Private Sub zzCalcAreas()
		Me.Cursor = Cursors.WaitCursor
		If mfCalcAreas Is Nothing OrElse mfCalcAreas.IsDisposed Then
			mfCalcAreas = New frmCalcAreas()   'Autodesk.AutoCAD.ApplicationServices.Application.MainWindow
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfCalcAreas)
		Me.Visible = False
		mfCalcAreas.Owner = Me

		Me.Cursor = Cursors.Default
	End Sub
	Private Sub smiLotApproved_Points_Click(oSender As System.Object, e As EventArgs) Handles smiLotApproved_Points.Click
		'	zzOpenUD_ProjectData()
		Dim tMapThemeData As DMAcadExt.MapThemeData
		tMapThemeData = zzGetCurrentMapThemeData()
		If tMapThemeData.IsNotEmpty Then
			zzOpenDesignForm(tMapThemeData)
		End If

		zzOpenLotPoints(tMapThemeData)
	End Sub

	Private Sub smiUnidiv_ExportData_Click(oSender As System.Object, e As EventArgs) Handles smiUnidiv_ExportData.Click

		'   Dim sFileName As String = "D:\TestA.XML"
		Dim sPrjDataFileName As String = GetExportDir(True, "PrjData.XML")
		Dim oPrj_ExpImpData As TPlServerDB.Prj_ExpImpData = New TPlServerDB.Prj_ExpImpData(miProjectCode, miDetailNo)

		oPrj_ExpImpData.ExportData(sPrjDataFileName)

		Dim sUD_DataFileName As String = GetExportDir(True, "UD_Data.XML")

		Dim oUD_ExpImpData As UnidivNet.UD_ExpImpData = New UnidivNet.UD_ExpImpData(miProjectCode, miDetailNo)


		oUD_ExpImpData.ExportData(sUD_DataFileName)


	End Sub
	Private Sub smiUnidiv_ImportData_Click(oSender As System.Object, e As EventArgs) Handles smiUnidiv_ImportData.Click
		'  Dim sFileName As String = "D:\TestPrj.XML"
		Dim sPrjDataFileName As String = GetExportDir(True, "PrjData.XML")
		Dim sUD_DataFileName As String = GetExportDir(True, "UD_Data.XML")


		Dim oPrj_ExpImpData As TPlServerDB.Prj_ExpImpData = New TPlServerDB.Prj_ExpImpData()
		If oPrj_ExpImpData.ImportData(sPrjDataFileName) Then


			'   DMCommon.Debug.MsgBox("12_900", oPrj_ExpImpData.ProjectCode, oPrj_ExpImpData.DetailNo)
			Dim oUD_ExpImpData As UnidivNet.UD_ExpImpData = New UnidivNet.UD_ExpImpData(oPrj_ExpImpData.ProjectCode, oPrj_ExpImpData.DetailNo)
			'  DMCommon.Debug.MsgBox("12_901AA", sUD_DataFileName, oPrj_ExpImpData.ProjectCode, oPrj_ExpImpData.DetailNo)
			oUD_ExpImpData.ImportData(sUD_DataFileName)
		End If


	End Sub
	'12-11-87=1*-11-** +  *2-**-87
	Private Sub smiUnidiv_EraseHanit_Click(oSender As System.Object, e As EventArgs) Handles smiUnidiv_EraseHanit.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		If DMAcadExt.AcadDocument.IsLocked Then
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
			'    DMAcadExt.AcadDocument.OpenLog(False)
			DMAcadExt.AcadTransaction.ClearLayerSet(UnidivNet.UD_App.GetHanitLayersNew(True))
			UnidivNet.UD_App.DeleteHanitTopos()
			UnidivNet.UD_App.ClearHanitCentroids()
			'	UnidivNet.UD_App.NewPointsToInitLayer()
			UnidivNet.UD_App.ComeBackSPoints(True)
			UnidivNet.UD_App.ToSourceLayer()
			UnidivNet.UD_App.ClearCdBlockMarks()
			UnidivNet.UD_App.DeleteScriptTable()
			DMAcadExt.AcadDocument.CloseLog()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End If
	End Sub
	Private Sub smiExpro_EntConnected_Click(oSender As System.Object, e As EventArgs) Handles smiExpro_EntConnected.Click
		zzOpenEntConnected()
	End Sub

	Public Shared Function GetExportDir(bExport As Boolean, sFileName As String) As String
		Const sExpDataDirName As String = "ExpData"
		Dim sResFullName As String
		Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(DMAcadExt.AcadDocument.GetCurrentDWGName)
		'   Dim sFileName As String = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length)
		Dim oDWGDirectory As IO.DirectoryInfo = oFileInfo.Directory
		Dim oExportDirectory As IO.DirectoryInfo = New IO.DirectoryInfo(oDWGDirectory.FullName & "\" & sExpDataDirName)
		If Not oExportDirectory.Exists Then
			If bExport Then
				oDWGDirectory.CreateSubdirectory(sExpDataDirName)
			Else
				Return Nothing
			End If
		End If
		sResFullName = oExportDirectory.FullName & "\" & sFileName
		If Not bExport Then
			oFileInfo = New IO.FileInfo(sResFullName)
			If Not oFileInfo.Exists Then
				sResFullName = Nothing
			End If
		End If
		Return sResFullName
	End Function

	Private Sub mfUD_SelectPlan_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfUD_SelectPlan.FormClosed
		Me.Visible = True
	End Sub
	Private Sub mfLotPoints_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfLotPoints.FormClosed
		Me.Visible = True
	End Sub




	Private Sub mfOwnership_FormHided() Handles mfOwnership.FormHided
		Me.Visible = True
	End Sub

	Private Sub frmPrjThemes_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
			Me.Location = New System.Drawing.Point(240, 240)
			Me.WindowState = FormWindowState.Normal
		End If
	End Sub

	Private Sub smiUnidiv_EraseDB_Click(oSender As System.Object, e As EventArgs)


		'	Const sSPName As String = "ClearBlockData"
		'Const sSPName As String = "ClearPlanData"


		'	Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())
		'   DMCommon.Debug.MsgBox("zzClearGushData", iRes)

	End Sub


	Private Sub mfBamash_FormClosed(sender As System.Object, e As FormClosedEventArgs) Handles mfBamash.FormClosed
		Me.Visible = True
	End Sub



	Private Sub smiExpro_EntConnectedLayer_Click(oSender As System.Object, e As EventArgs) Handles smiExpro_EntConnectedLayer.Click
		zzOpenEditLayerList()
	End Sub

	Private Sub smiExpro_EntConnectedLot_Click(oSender As System.Object, e As EventArgs) Handles smiExpro_EntConnectedLot.Click
		zzOpenEntConnectedLot()
	End Sub

	Private Sub mfEntConnectedLot_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfEntConnectedLot.FormClosed
		Me.Visible = True
	End Sub

	Private Sub mfEntConnected_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfEntConnected.FormClosed
		Me.Visible = True
	End Sub

	Private Sub mfCalcAreas_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfCalcAreas.FormClosed
		Me.Visible = True
	End Sub

	Private Sub ssbApplication_ButtonClick(sender As Object, e As EventArgs) Handles ssbApplication.ButtonClick

	End Sub

	Private Sub tsbCalculate_Click(sender As Object, e As EventArgs) Handles tsbCalculate.Click

	End Sub

	Private Sub tsbCleanup_Click(sender As Object, e As EventArgs) Handles tsbCleanup.Click

	End Sub
End Class