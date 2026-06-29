Imports BamashNet
Imports AcadReport
Imports TopoManager
Imports System.Data
Imports System.Drawing
Imports System.ComponentModel
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD  'DatabaseServices
Public Class frmBamashM
	Private Enum enNumerationType
		NumerationAll
		EmptyFirst
		EmptyMax
		AAAA
		BBBB
		CCCCCC
		None
	End Enum
	'1. Numer PropId,Main,PropType=1; +?Color; + Consts; ByPick Or BySelSet  
	'1a.  +Color; + Consts; ByPick Or BySelSet   

	'2 Whole Property: Main,PropType=1; +?Color; +? cmidut NUMERation; ByPick+KeyWords
	'3 completion Property same 2 --MAIN
	'4 nUMERATION cmiduiot + Consts; ByPick Or BySelSet
	Private Const msCalculateSettingKey As String = "Calculate"
	Private Const msDictionaryName As String = "Bamash"
	Private Const msDictionaryKey As String = "Parcel"

	Private mtMapThemeData As DMAcadExt.MapThemeData
	Private Shared diResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmTopoMaster
	Private doSelectedReportItem As AcadReport.ReportItem
	Protected dbAutocad As Boolean 'False - Excel
	Protected doReportApp As AcadReport.Report
	Protected dsReportBlockFolder As String
	Private mtaColors() As Color = {Color.Red}
	Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmView
	Private moResource As TPlServerDB.TPlResource
	Private mdicMain As DatabaseServices.DBDictionary = New DatabaseServices.DBDictionary()
	Private mbDicIsNew As Boolean
	Private msDictionaryParcel As String
	Private mdBreakHeight As Double
	Private mdSpacing As Double
	Private miApartDescrUB As Integer = 14
	Private WithEvents mfBamashBlockRefs As frmBamashBlockRefs
	Private miaReports() As TPlServerDB.enResourceTheme = {
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
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
		mtMapThemeData = tMapThemeData
		' This call is required by the designer.
		InitializeComponent()
		bmBamash.InitParams()
		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
		zzOpenDictionary(True, False)

	End Sub
	Private Sub zzMyInitializeComponent()

		Me.txtZebraWidthDrawing.Text = Convert.ToString(bmBamash.ZebraWidthDrawingSetting)
		Me.txtZebraWidthTable.Text = Convert.ToString(bmBamash.ZebraWidthTableSetting)
		Me.txtBufferOffset.Text = Convert.ToString(bmBamash.BufferOffsetSetting)
		Me.txtZoomRadius.Text = Convert.ToString(bmBamash.ZoomRadiusSetting)

		Convert.ToString(bmBamash.ZebraWidthTableSetting)
		zzFillDoubleFormatRow(Me.cmbAreaFormat)
		Me.cmbAreaFormat.SelectedIndex = 2
		Me.lblAreaFormat.Text = "פורמט שטח"
		Try
			Me.txtRepBamashSharedScale.Text = Convert.ToString(zzGetDoubleSetting("RepBamashSharedScale", 0.4))
		Catch oEx As Exception

		End Try
		With lstReports
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember

			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashSum, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashSum), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, False, True, False))
			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashMain, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashMain), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, True, True))
			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashShared, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashShared), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, True, True))
			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashStamp, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashStamp), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, False, True, False))
			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashAfricaTitle), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, False, True))
			.Items.Add(New ReportItem(TPlServerDB.enResourceTheme.AcRepBamashAfricaData, zzGetReportName(TPlServerDB.enResourceTheme.AcRepBamashAfricaData), enReportOptions.Default, DMAcadExt.enTopoPurpose.Undefined, enReportModifications.Default, True, False, True))
		End With


		With Me.lblZebraWidthDrawing
			'.Location = New System.Drawing.Point(80, 12)
			'.Name = "lblZebraWidthDrawing"
			'.Size = New System.Drawing.Size(144, 20)
			'.TabIndex = 6
			.Text = zzGetText(1, 8)
		End With
		With Me.lblZebraWidthTable
			.Text = zzGetText(2, 8)
		End With

		With Me.lblBufferOffset
			.Text = zzGetText(0, 8)

		End With

		With Me.lblZoomRadius
			.Text = zzGetText(3, 8)
		End With
		With Me.lblRepBamashSharedScale

			.Text = "קנ""מ זברה"

		End With


		For iIndex As Integer = 0 To 5
			Me.cmbChoiceColor.Items.Add(iIndex)
		Next

		Dim iaPropertyTypes() As enPropertyTypes = DirectCast([Enum].GetValues(GetType(enPropertyTypes)), enPropertyTypes())
		Dim oItemData As DMCommon.ItemData


		Me.cmbPropertyType.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbPropertyType.DisplayMember = DMCommon.ItemData.DisplayMember
		For iIndex As Integer = 0 To iaPropertyTypes.GetUpperBound(0)
			oItemData = New DMCommon.ItemData(iaPropertyTypes(iIndex), bmBamash.GetPropTypesName(iaPropertyTypes(iIndex)))
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PropertyTypes", iaPropertyTypes(iIndex), bmBamash.GetPropTypesName(iaPropertyTypes(iIndex)))
			Me.cmbPropertyType.Items.Add(oItemData)
		Next

		For iIndex As Integer = 0 To miApartDescrUB
			Me.cmbAprtDesc.Items.Add(iIndex.ToString() & "-" & GetApartDescr(0, "", iIndex))
		Next
		zzFillNumerationList(Me.cmbNumerationPropID)
		Me.cmbNumerationPropID.SelectedIndex = 0
	End Sub
	Private Sub zzGetDictionaryParcel()
		Dim oXrecord As DatabaseServices.Xrecord
		Dim tAcObjID As DatabaseServices.ObjectId
		'Dim sParcel As String = Nothing
		If Not mbDicIsNew AndAlso mdicMain.Contains(msDictionaryKey) Then


			tAcObjID = DirectCast(mdicMain.Item(msDictionaryKey), DatabaseServices.ObjectId)
			oXrecord = DMAcadExt.AcadTransaction.GetXrecord(tAcObjID)

			If oXrecord IsNot Nothing Then
				Dim oResBuffer As DatabaseServices.ResultBuffer = oXrecord.Data
				Dim oaTypedValue() As DatabaseServices.TypedValue = oResBuffer.AsArray()
				For iIndex As Integer = 0 To oaTypedValue.GetUpperBound(0)

					'	msDictionaryParcel = DirectCast(oaTypedValue(iIndex).Value, String)
					Exit For
				Next

				msDictionaryParcel = DirectCast(oaTypedValue(0).Value, String)
				If oaTypedValue.GetUpperBound(0) >= 2 Then
					mdBreakHeight = DirectCast(oaTypedValue(1).Value, Double)
					mdSpacing = DirectCast(oaTypedValue(2).Value, Double)
				End If



			End If

		Else
			msDictionaryParcel = Nothing

			mdBreakHeight = 0.0
			mdSpacing = 0.0

		End If

	End Sub
	Private Sub zzUpdateData()

		Dim sParcelNew As String = Me.txtParcelNo.Text
		Dim dBreakHeightNew As Double = 0.0
		Dim dSpacingNew As Double = 0.0
		If Not String.IsNullOrEmpty(Me.txtBreakHeight.Text) AndAlso Double.TryParse(Me.txtBreakHeight.Text, dBreakHeightNew) Then
		End If
		If Not String.IsNullOrEmpty(Me.txtSpacing.Text) AndAlso Double.TryParse(Me.txtSpacing.Text, dSpacingNew) Then
		End If


		Dim oXrecord As DatabaseServices.Xrecord
		Dim bAddNew As Boolean = False
		Dim oNewData() As System.Object = {sParcelNew, dBreakHeightNew, dSpacingNew}
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		Try
			mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(mdicMain.ObjectId, DatabaseServices.OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DBDictionary)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & msDictionaryParcel, "Error #2931")

		End Try

		If Not String.IsNullOrEmpty(msDictionaryParcel) Then
			If sParcelNew <> msDictionaryParcel OrElse dBreakHeightNew <> mdBreakHeight OrElse dSpacingNew <> mdSpacing Then
				mdicMain.Remove(msDictionaryKey)
				bAddNew = True
			End If
		Else
			bAddNew = True
		End If
		If bAddNew Then

			oXrecord = New DatabaseServices.Xrecord()
			Try
				oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oNewData, False)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & msDictionaryKey, "Error #2915")

			End Try
			DMCommon.Debug.MsgBox("ParcelNew, dBreakHeightNew, dSpacing", sParcelNew, dBreakHeightNew, dSpacingNew)
			Try
				mdicMain.SetAt(msDictionaryKey, oXrecord)
				DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)
				mbDicIsNew = False
				zzGetDictionaryParcel()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & mdicMain.Count.ToString() & vbCrLf & msDictionaryKey, "Error #2916")
			End Try

		End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public Sub zzAdd(sParcel As String)
		Dim oXrecord As DatabaseServices.Xrecord = New DatabaseServices.Xrecord()
		Dim sKey As String = "Parcel"
		Dim oValue As System.Object = sParcel
		Try
			oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oValue, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2715")

		End Try
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+RegS03", sKey, oXrecord.Count, mdicMain Is Nothing, "--- -------")
		Try
			mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(mdicMain.ObjectId, DatabaseServices.OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DBDictionary)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2908")
		End Try

		Try
			mdicMain.SetAt(sKey, oXrecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2916")
		End Try
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!RegS04", "--- -------")
		DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)

		DMCommon.Debug.ExcelLog.SetNextValue(0, "!RegS05", "--- -------")
		'	MessageBox.Show(sKey & ":" & tTypedValue.ToString, "ADD  18_355")


	End Sub
	Private Sub zzFillNumerationList(ByRef oComboBox As ComboBox)
		Dim oItemData As DMCommon.ItemData
		Dim iaNumerationType() As enNumerationType = DirectCast([Enum].GetValues(GetType(enNumerationType)), enNumerationType())
		With oComboBox
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember
			For iIndex As Integer = 0 To iaNumerationType.GetUpperBound(0)
				oItemData = New DMCommon.ItemData(iaNumerationType(iIndex), zzGetText(iaNumerationType(iIndex), 7))
				oComboBox.Items.Add(oItemData)
			Next
		End With
	End Sub
	Protected Function zzGetDoubleSetting(ByVal sSettingName As String, Optional ByVal dDefaultValue As Double = 0.0) As Double
		Try
			Dim oDynamic As System.Object = My.Settings.Item(sSettingName)
			If oDynamic IsNot Nothing Then
				Return DirectCast(oDynamic, Double)
			Else
				Return dDefaultValue
			End If
		Catch oEx As Exception

			Return 0.0
		End Try
	End Function
	Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
		Try
			Return TPlServerDB.TextResource.GetText(iItemID, bmBamash.ResourceTheme, iSectionID, bReturnEmpty)
		Catch oEx As Exception
			Return String.Empty
		End Try

	End Function


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
		saPrompt(0) = "Enter start point Of the report"
		saPrompt(1) = vbCrLf & "Enter End point Of the report"

		Dim bResp As Boolean = TopoManager.TPlanGraph.TplnProject.GetPoint("start point", tPoint)
		'   Me.Show()

	End Sub

	Private Sub cmbAreaFormat_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbAreaFormat.SelectedIndexChanged
		Try
			TopoManager.TPlanGraph.TplnProject.AreaFormat = Me.cmbAreaFormat.SelectedItem.ToString()

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActions - cmbAreaFormat_SelectedIndexChanged")
		End Try
	End Sub

	Private Sub zzOpenDictionary(ByVal bCreate As Boolean, ByVal bReadOnly As Boolean)
		Dim tBamashDicObjID As DatabaseServices.ObjectId
		Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary = Nothing
		Dim iNamedMode As DatabaseServices.OpenMode
		Dim iMode As DatabaseServices.OpenMode
		If bCreate Then
			iNamedMode = DatabaseServices.OpenMode.ForWrite
		Else
			iNamedMode = DatabaseServices.OpenMode.ForRead
		End If
		If bReadOnly Then '
			iMode = DatabaseServices.OpenMode.ForRead
		Else
			iMode = DatabaseServices.OpenMode.ForWrite
		End If
		'	DMCommon.Debug.MsgBox("081120_1", "4", dicNamed Is Nothing, bCreate, bReadOnly, iNamedMode, iMode)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(iNamedMode)
		'	DMCommon.Debug.MsgBox("081120_1", "41", msDictionaryName, dicNamed Is Nothing)
		If dicNamed IsNot Nothing Then
			'	DMCommon.Debug.MsgBox("070720_1", dsDictionaryName, dicNamed.Contains(dsDictionaryName))
			If dicNamed.Contains(msDictionaryName) Then
				'DMCommon.Debug.MsgBox("081120_1", "52a")
				Try
					tBamashDicObjID = DirectCast(dicNamed.Item(msDictionaryName), DatabaseServices.ObjectId)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("180421_1", oEx.Message, msDictionaryName, dicNamed.Contains(msDictionaryName), dicNamed.Item(msDictionaryName), dicNamed.Item(msDictionaryName).GetType())
				End Try

				mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tBamashDicObjID, iMode), Autodesk.AutoCAD.DatabaseServices.DBDictionary)

				'mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tRegionSetDicObjID, iMode), RegionSetDictionary)
				'	mbOpened = True
				'	miMode = iMode
			ElseIf bCreate Then
				'	DMCommon.Debug.MsgBox("081120_1", "52b")
				mdicMain = New Autodesk.AutoCAD.DatabaseServices.DBDictionary()
				'mdicMain = New RegionSetDictionary()
				dicNamed.SetAt(msDictionaryName, mdicMain)
				'	mbOpened = True
				DMAcadExt.AcadTransaction.AppendDBObject(mdicMain)
				dicNamed.Contains(msDictionaryName)
				mbDicIsNew = True

			End If
			dicNamed = Nothing
		Else
			MessageBox.Show("NamedDictionary is nothing " & iNamedMode.ToString(), "13_181")
		End If
		zzGetDictionaryParcel()
		Me.txtParcelNo.Text = msDictionaryParcel
		If mdBreakHeight <> 0.0 Then
			Me.txtBreakHeight.Text = mdBreakHeight.ToString()
		End If
		If mdSpacing <> 0.0 Then
			Me.txtSpacing.Text = mdSpacing.ToString()
		End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub


	Private Sub cmdCalculate_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdCalculate.Click
		Dim iCalcOption As bmBamash.SubNumerationOptions
		Dim iParcelNo As Integer = 0
		Me.Cursor = Cursors.WaitCursor
		If Integer.TryParse(Me.txtParcelNo.Text, iParcelNo) Then
			bmBamash.Parcel = iParcelNo.ToString()
		Else
			bmBamash.Parcel = Me.txtParcelNo.Text
		End If


		'	bmProperty.OutputFullColor = Me.chkFullColor.Checked
		iCalcOption = bmBamash.SubNumerationOptions.None 'Boris 19/05/20
		BamashNet.bmBamash.Calculate(iCalcOption, mtMapThemeData)
		zzSetMinMax()
		Me.lstReports.Enabled = True
		Me.cmdExpExcel.Enabled = True
		Me.cmdInsertRep.Enabled = True
		Me.Cursor = Cursors.Default
		'MyBase.OnCalculate()
	End Sub
	Private Sub cmdUpdateCentroids_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		BamashNet.bmBamash.UpdateCentroids()
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
		Me.lstReports.Focus()
	End Sub
	Private Sub cmdPaintTable_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
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
				'''''''''''''''''''''''''''''''	oPgon.PaintZebra(taColorScheme(iIndex).Zebra)
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
			If oDataView IsNot Nothing AndAlso oDataView.Count > 0 Then
				zzInsertTableReport(oDataView, iaDataColumns, oaTotals, oaOptionValues, iaEmptyColumns)
			ElseIf dicAttribValues IsNot Nothing Then
				InsertBlockReport(dicAttribValues, sBlockName)
			End If
		End If
		Me.Cursor = Cursors.Default
	End Sub
	Protected Sub InsertBlockReport(ByVal dicAttribValues As Dictionary(Of String, String), ByVal sBlockName As String)
		If dicAttribValues IsNot Nothing Then
			Dim bCurrentLayerOK As Boolean = True
			AcadReport.RepApp.InitDWGScaleFactor()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, 2, DMAcadExt.enLayerFunction.Report, True, True, True)

			Dim oBlockReport As AcadReport.BlockReport = New AcadReport.BlockReport(dsReportBlockFolder, sBlockName)
			oBlockReport.AttribValues = dicAttribValues
			Me.Hide()
			Common.SetAcadFocus()
			oBlockReport.InsertBlockRef()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.RestoreVarCmdDia()
		End If

	End Sub
	Private Sub zzInsertTableReport(ByVal oDataView As DataView, ByVal iaDataColumns() As Integer, ByVal oaTotals() As System.Object, ByVal oaOptionValues() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing)
		If oDataView IsNot Nothing Then
			Dim oRepApp As AcadReport.BaseReport
			Dim bCurrentLayerOK As Boolean = True
			If dbAutocad Then
				oRepApp = New AcadReport.Report
			Else
				oRepApp = New ExcelReport.Report(False)
			End If
			DMCommon.Debug.MsgBox("!300123", oDataView.Count)
			If oRepApp.Open(doSelectedReportItem.RepIndex) Then

				DMCommon.Debug.ExcelLog.SetDataTable(0, "!MainRep" & CStr(dbAutocad), oDataView)
				DMCommon.Debug.ExcelLog.SetEnumerable(0, "!iaDataColumns " & CStr(dbAutocad), iaDataColumns)
				oRepApp.MainView = oDataView
				If iaDataColumns IsNot Nothing Then
					oRepApp.DataColumns = iaDataColumns
				End If
				If iaDataColumns IsNot Nothing Then
					oRepApp.DataColumns = iaDataColumns
				End If

				If oaTotals IsNot Nothing Then
					oRepApp.Totals = oaTotals
				End If
				If oaOptionValues(0) IsNot Nothing Then
					oRepApp.OptionValues = oaOptionValues
				End If
				If iaEmptyColumns IsNot Nothing Then
					oRepApp.EmptyColumns = iaEmptyColumns
				End If
				If oRepApp.AcadModel Then
					DMAcadExt.AcadDocument.SaveVarCmdDia(0)
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					AcadReport.RepApp.InitDWGScaleFactor()

					AcadReport.RepApp.Init(False)
					bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

					Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
					Dim taColorScheme() As DMAcadExt.ColorScheme
					Dim oReportApp As AcadReport.Report
					If bCurrentLayerOK Then
						Me.Hide()
						Common.SetAcadFocus()
						AcadReport.RepApp.GetStartPoint()
						oRepApp.Insert()
						oReportApp = DirectCast(oRepApp, AcadReport.Report)
						If AcadReport.RepApp.AcadTable IsNot Nothing Then
							''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
							'''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
							'''''''''''''''''	AcadReport.Report.ReDrawTable()
							If mdBreakHeight > 0.0 Then
								AcadReport.RepApp.SetBreak(mdBreakHeight, mdSpacing)
							End If
							colaPoints = oReportApp.GetColorCells()
							doReportApp = oReportApp
							If colaPoints IsNot Nothing Then
								taColorScheme = oReportApp.GetColorScheme()
								Dim oPgon As SimplePgon
								'	Dim tColorZebra As DMAcadExt.ColorZebra
								Dim dRepBamashSharedScale As Double
								Try
									dRepBamashSharedScale = Convert.ToDouble(Me.txtRepBamashSharedScale.Text)
									If dRepBamashSharedScale < 0.00001 Then
										dRepBamashSharedScale = 1.0
									End If
								Catch oEx As Exception
									dRepBamashSharedScale = Parameters.LegendPaintFactor
								End Try



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
									'	DMAcadExt.AcadDocument.WriteMessage("^709 " & CStr(taColorScheme(iIndex).Scale) & ":" & CStr(dRepBamashSharedScale) & ":" & CStr(AcadReport.RepApp.DrawingScaleFactor))
									taColorScheme(iIndex).Scale = dRepBamashSharedScale * AcadReport.RepApp.DrawingScaleFactor
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
		End If

	End Sub
	Private Sub txtZebraWidthDrawing_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZebraWidthDrawing.Leave
		If Me.txtZebraWidthDrawing.Text.Length <> 0 Then
			Try
				bmBamash.ZebraWidthDrawing = Convert.ToDouble(Me.txtZebraWidthDrawing.Text)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsM - txtZebraWidthDrawing_Leave")
			End Try
		End If
	End Sub
	Private Sub txtZebraWidthTable_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtZebraWidthTable.Leave
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



	Private Sub cmbChoiceColor_DrawItem(oSender As System.Object, e As DrawItemEventArgs) Handles cmbChoiceColor.DrawItem
		Dim cboForeColor As Color = Color.Black
		Dim cboBackColor As Color = Color.WhiteSmoke
		If (e.Index < 0) Then Return
		Dim ForeColor As Color = e.ForeColor
		Dim tColor As DMAcadExt.DMColor = New DMAcadExt.DMColor(Convert.ToInt16(e.Index + 1))
		e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
		e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit

		If (e.State.HasFlag(DrawItemState.Focus) AndAlso Not e.State.HasFlag(DrawItemState.ComboBoxEdit)) Then
			e.DrawBackground()
			e.DrawFocusRectangle()

		Else
			Dim backgbrush As Brush = New SolidBrush(cboBackColor)

			e.Graphics.FillRectangle(backgbrush, e.Bounds)
			ForeColor = cboForeColor

		End If
		Dim textbrush As Brush = New SolidBrush(ForeColor)

		e.Graphics.DrawString(tColor.HebColorName, e.Font, textbrush, e.Bounds.Height + 30, e.Bounds.Y, StringFormat.GenericTypographic)

		Dim o As Font


		Dim oPen As Pen = New Pen(tColor.FrameworkColor, 4)
		e.Graphics.DrawRectangle(oPen, New Rectangle(e.Bounds.Location,
								 New Size(e.Bounds.Height - 4, e.Bounds.Height - 4)))
	End Sub
	Private Function zzGetColorByIndex(iIndex As Integer) As Color
		Dim tColor As DMAcadExt.DMColor = New DMAcadExt.DMColor(Convert.ToInt16(iIndex))
		Return tColor.FrameworkColor
	End Function


	Private Sub cmdOpenBlockRefTable_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenBlockRefTable.Click
		Me.Cursor = Cursors.WaitCursor
		mfBamashBlockRefs = New frmBamashBlockRefs(BamashPolygon.MainView)
		DMCommon.Debug.ExcelLog.SetDataTable(0, "!MainView", BamashPolygon.MainView)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfBamashBlockRefs)
		Me.Visible = False
		mfBamashBlockRefs.Owner = Me
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub mfBamashBlockRefs_Closed(oSender As System.Object, e As EventArgs) Handles mfBamashBlockRefs.Closed
		Me.Visible = True
	End Sub
	Private Sub cmdSelectByPick_Click_210720(oSender As System.Object, e As EventArgs) 'Handles cmdSelectByPick.Click
		Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iAprtDescNum As Integer = 0
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		Dim iCurrentPgonColor As Integer = 1
		'Dim bCurrentMain As Boolean = True
		Dim bNext As Boolean = False
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If String.IsNullOrEmpty(Me.txtPropID.Text) Then
			iCurrentPropID = 1
		ElseIf Not Integer.TryParse(Me.txtPropID.Text, iCurrentPropID) Then
			iCurrentPropID = 1
		End If
		If String.IsNullOrEmpty(Me.cmbChoiceColor.Text) Then
			iCurrentPgonColor = 1
		ElseIf Integer.TryParse(Me.cmbChoiceColor.Text, iCurrentPgonColor) Then
			iCurrentPgonColor += 1
		Else
			iCurrentPgonColor = 1
		End If

		If String.IsNullOrEmpty(Me.txtSubpropNum.Text) Then
			iCurrentSubpropNum = 1
		ElseIf Not Integer.TryParse(Me.txtSubpropNum.Text, iCurrentSubpropNum) Then
			iCurrentSubpropNum = 1
		End If




		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectPoint(tPoint, bOneOnly)
				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
					Try
						oPolygon = oBamashTopology.FindPolygon(tPoint)
						If oPolygon IsNot Nothing Then
							oBamashPgon = bmBamash.GetBamashPgon(oPolygon.ID)
							'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)
							oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, -1, iCurrentPgonColor)
							If iPropertyType = enPropertyTypes.SubPrivateType Then
								iCurrentSubpropNum += 1
							End If
							oBamashPgon.UpdateMainData()
						End If
					Catch oEx As Exception

					End Try
					iSelectKeywordStatus = zzGetPropertyType(1, iPropertyType, iAprtDescNum, bNext)
					'DMCommon.Debug.MsgBox("!ByPick1", iSelectPointStatus, iSelectKeywordStatus, iPropertyType, bNext)
					If iSelectKeywordStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						If bNext Then
							iPropertyType = enPropertyTypes.ApartType
							iCurrentPropID += 1
							If iCurrentPgonColor = 6 Then
								iCurrentPgonColor = 1
							Else
								iCurrentPgonColor += 1
							End If
						Else

						End If
					ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
						Exit Do
					End If
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do
				End If
			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Function zzGetPropertyType(bSubOnly As Boolean) As enPropertyTypes
		Dim oItem As DMCommon.ItemData
		Dim iResPropertyType As enPropertyTypes
		If chkPropertyType.Checked AndAlso Me.cmbPropertyType.SelectedIndex >= 0 Then
			oItem = DirectCast(cmbPropertyType.SelectedItem, DMCommon.ItemData)

			If [Enum].IsDefined(GetType(enPropertyTypes), oItem.ListIndex) Then
				iResPropertyType = CType(oItem.ListIndex, enPropertyTypes)
			Else
				iResPropertyType = enPropertyTypes.Default


			End If

		Else
			iResPropertyType = enPropertyTypes.Default
			iResPropertyType = enPropertyTypes.Ignore
		End If
		If bSubOnly AndAlso iResPropertyType <> enPropertyTypes.SubShareType Then
			Return enPropertyTypes.SubPrivateType
		Else
			Return iResPropertyType
		End If
	End Function
	Private Function zzGetUserValues(ByRef tBamashData As bmPolygonData) As Boolean
		Dim sVal As String
		Dim saVal() As String
		Dim bResp As Boolean
		Dim oItem As DMCommon.ItemData
		tBamashData = New bmPolygonData(enPropertyTypes.Default)
		If chkPropertyType.Checked AndAlso Me.cmbPropertyType.SelectedIndex >= 0 Then
			oItem = DirectCast(cmbPropertyType.SelectedItem, DMCommon.ItemData)

			If [Enum].IsDefined(GetType(enPropertyTypes), oItem.ListIndex) Then
				tBamashData.PropertyType = CType(oItem.ListIndex, enPropertyTypes)
			End If
			bResp = True


		End If
		If Me.chkAprtDesc.Checked AndAlso Me.cmbAprtDesc.SelectedIndex >= 0 Then
			Dim iAprtDesc As Integer
			sVal = Me.cmbAprtDesc.Text
			saVal = Split(sVal, "-")
			If saVal.GetUpperBound(0) > 0 AndAlso Integer.TryParse(saVal(0), iAprtDesc) Then
				tBamashData.AprtDescNum = iAprtDesc
				tBamashData.AprtDesc = saVal(1)
				bResp = True
			End If
			'DMCommon.Debug.MsgBox("230720_1", iAprtDesc, sVal, Me.cmbAprtDesc.SelectedItem)
		End If

		If chkBldFloor.Checked Then
			tBamashData.BldFloor = Me.txtBldFloor.Text.Trim()
			bResp = True
		End If


		If chkBldPart.Checked Then
			Dim iBldPart As Integer
			Dim sBldPart As String = Me.txtBldPart.Text.Trim()
			If sBldPart.Length = 0 OrElse Integer.TryParse(sBldPart, iBldPart) Then
				tBamashData.BldPart = iBldPart
				bResp = True
			End If
		End If

		If chkBldEntr.Checked Then
			Dim iBldEntr As Integer
			Dim sBldEntr As String = Me.txtBldEntr.Text.Trim()
			If sBldEntr.Length = 0 OrElse Integer.TryParse(sBldEntr, iBldEntr) Then
				tBamashData.BldEntr = iBldEntr
				bResp = True
			End If
		End If

		If chkBldNo.Checked Then
			Dim iBldNo As Integer
			Dim sBldNo As String = Me.txtBldNo.Text.Trim()
			If sBldNo.Length = 0 OrElse Integer.TryParse(sBldNo, iBldNo) Then
				tBamashData.BldNo = iBldNo
				bResp = True
			End If
		End If




		Return bResp
	End Function
	Private Sub zzSetConstValues()

		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus

		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		'Dim iCurrentPropID As Integer
		'Dim iCurrentSubpropNum As Integer
		Dim iCurrentPgonColor As Integer = 1
		Dim tBamashData As bmPolygonData = New bmPolygonData()

		If zzGetUserValues(tBamashData) Then
			Dim bNext As Boolean = False
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			'	DMCommon.Debug.MsgBox("", iCurrentPropID, iCurrentPgonColor, iCurrentSubpropNum, Me.txtPropID.Text, Me.cmbChoiceColor.Text)

			Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oBamashTopology IsNot Nothing Then
				Me.Visible = False
				Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
				Do
					iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology)
					If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then

						'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)
						'oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, iCurrentPgonColor)

						oBamashPgon.UpdateNewData(tBamashData)

					ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
						Exit Do
					End If
				Loop

				oBamashTopology.Close()
				Me.Show()
				Common.SetAcadFocus()
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()

			End If
		End If

	End Sub
	Private Sub zzSetConstValues(oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet)

		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing


		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim oaSelectedObjects(oSelSet.Count - 1) As Autodesk.AutoCAD.EditorInput.SelectedObject
		'Dim iCurrentPropID As Integer
		'Dim iCurrentSubpropNum As Integer
		Dim iCurrentPgonColor As Integer = 1
		Dim tBamashData As bmPolygonData = New bmPolygonData()

		If zzGetUserValues(tBamashData) Then
			Dim bNext As Boolean = False
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)


			oSelSet.CopyTo(oaSelectedObjects, 0)

			If oaSelectedObjects IsNot Nothing Then
				DMCommon.Debug.MsgBox("220629_1", oaSelectedObjects.GetUpperBound(0))

				'	Me.Visible = False
				'Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
				For iIndex As Integer = 0 To oaSelectedObjects.GetUpperBound(0)
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oaSelectedObjects", oaSelectedObjects(iIndex).ObjectId)
					oBamashPgon = bmBamash.GetBamashPgon(oaSelectedObjects(iIndex).ObjectId)

					'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)
					'oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, iCurrentPgonColor)
					If oBamashPgon IsNot Nothing Then
						oBamashPgon.UpdateNewData(tBamashData)
					End If




				Next
			End If

			'Me.Show()
			'Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If

	End Sub
	Private Sub cmdSelectByPick_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectByPick.Click
		Me.Cursor = Cursors.WaitCursor
		Select Case Me.cmbNumerationPropID.SelectedIndex
			Case 0
				zzSelectByPick_Al11(False)
			Case 1
				zzSelectByPick_Al1(True)
			Case 2
				zzSelectByPick_Al2(False)
			Case 3
				bmBamash.SubpropNumeration()
			Case 4
				zzSelectByPick_Al3()
			Case 5
				bmBamash.SubpropNumerationClear()
			Case 6
				zzSelectByPick_Al4(False)
			Case Else
				zzSelectByPick_Al1(True)
		End Select
		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzAddMsgFromPropID(iPropID As Integer, iSubPropNum As Integer, iPropertyType As enPropertyTypes) As String
		Dim sPropIDMsg As String
		Dim sSubPropNumMsg As String
		Dim sDelim As String
		If iPropID > 0 Then
			sPropIDMsg = iPropID.ToString()
		Else
			sPropIDMsg = String.Empty
		End If
		If iSubPropNum > 0 AndAlso (iPropertyType = enPropertyTypes.SubPrivateType OrElse iPropertyType = enPropertyTypes.SubShareType) Then
			sSubPropNumMsg = DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
		Else
			sSubPropNumMsg = String.Empty
		End If
		If iPropID > 0 AndAlso Not String.IsNullOrEmpty(sSubPropNumMsg) Then
			sDelim = " "
		Else
			sDelim = String.Empty
		End If
		If iPropID > 0 OrElse Not String.IsNullOrEmpty(sSubPropNumMsg) Then
			Return " (" & sPropIDMsg & sDelim & sSubPropNumMsg & ")"
		Else
			Return Nothing
		End If

	End Function
	Private Sub zzSelectByPick_Al11(bNewPropID As Boolean)
		'FALSE מספור מחדש --  
		'TRUE  השלמת המספור  
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iAprtDescNum As Integer = 0
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		Dim iCurrentPolygonColor As Integer = 1
		Dim bNext As Boolean = False
		If bmBamash.BamashPgons Is Nothing Then
			BamashNet.bmBamash.Calculate(bmBamash.SubNumerationOptions.EmptyFirst, mtMapThemeData)
			zzSetMinMax()
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If String.IsNullOrEmpty(Me.txtPropID.Text) Then
			iCurrentPropID = 1
		ElseIf Not Integer.TryParse(Me.txtPropID.Text, iCurrentPropID) Then
			iCurrentPropID = 1
		End If
		If bNewPropID Then
			iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
		End If

		If String.IsNullOrEmpty(Me.cmbChoiceColor.Text) Then
			iCurrentPolygonColor = 1
		ElseIf Integer.TryParse(Me.cmbChoiceColor.Text, iCurrentPolygonColor) Then
			iCurrentPolygonColor += 1
		Else
			iCurrentPolygonColor = 1
		End If

		If String.IsNullOrEmpty(Me.txtSubpropNum.Text) Then
			iCurrentSubpropNum = 1
		ElseIf Not Integer.TryParse(Me.txtSubpropNum.Text, iCurrentSubpropNum) Then
			iCurrentSubpropNum = 1
		End If

		'	DMCommon.Debug.MsgBox("", iCurrentPropID, iCurrentPgonColor, iCurrentSubpropNum, Me.txtPropID.Text, Me.cmbChoiceColor.Text)
		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology, zzAddMsgFromPropID(iCurrentPropID, iCurrentSubpropNum, iPropertyType))
				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK OrElse iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then

					iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
					If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, iAprtDescNum, iCurrentPolygonColor)

						oBamashPgon.UpdateMainData()

					End If

					'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)

					If iPropertyType = enPropertyTypes.SubPrivateType Then
						iCurrentSubpropNum += 1
					End If

					iSelectKeywordStatus = zzGetPropertyType(2, iPropertyType, iAprtDescNum, bNext)
					'DMCommon.Debug.MsgBox("!ByPick1", iSelectPointStatus, iSelectKeywordStatus, iPropertyType, bNext)
					If iSelectKeywordStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						If bNext Then
							iPropertyType = enPropertyTypes.ApartType
							iAprtDescNum = 1
							iCurrentPropID += 1
							If bNewPropID Then
								iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
							End If
							If iCurrentPolygonColor = 6 Then
								iCurrentPolygonColor = 1
							Else
								iCurrentPolygonColor += 1
							End If
						Else
							'	DMCommon.Debug.MsgBox("!ByPick2_Not Next", iSelectPointStatus, iSelectKeywordStatus, iPropertyType, iAprtDescNum, bNext)
						End If
					ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
						Exit Do
					End If
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do
				End If
			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzSelectByPick_Al1(bNewPropID As Boolean)
		'FALSE מספור מחדש --  
		'TRUE  השלמת המספור  
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iAprtDescNum As Integer = 0
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		Dim iCurrentPolygonColor As Integer = 1
		Dim bNext As Boolean = False
		If bmBamash.BamashPgons Is Nothing Then
			BamashNet.bmBamash.Calculate(bmBamash.SubNumerationOptions.EmptyFirst, mtMapThemeData)
			zzSetMinMax()
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If String.IsNullOrEmpty(Me.txtPropID.Text) Then
			iCurrentPropID = 1
		ElseIf Not Integer.TryParse(Me.txtPropID.Text, iCurrentPropID) Then
			iCurrentPropID = 1
		End If
		If bNewPropID Then
			iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
		End If

		If String.IsNullOrEmpty(Me.cmbChoiceColor.Text) Then
			iCurrentPolygonColor = 1
		ElseIf Integer.TryParse(Me.cmbChoiceColor.Text, iCurrentPolygonColor) Then
			iCurrentPolygonColor += 1
		Else
			iCurrentPolygonColor = 1
		End If

		If String.IsNullOrEmpty(Me.txtSubpropNum.Text) Then
			iCurrentSubpropNum = 1
		ElseIf Not Integer.TryParse(Me.txtSubpropNum.Text, iCurrentSubpropNum) Then
			iCurrentSubpropNum = 1
		End If

		'	DMCommon.Debug.MsgBox("", iCurrentPropID, iCurrentPgonColor, iCurrentSubpropNum, Me.txtPropID.Text, Me.cmbChoiceColor.Text)
		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology, zzAddMsgFromPropID(iCurrentPropID, iCurrentSubpropNum, iPropertyType))
				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK OrElse iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then

					iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
					If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, -1, iCurrentPolygonColor)

						oBamashPgon.UpdateMainData()

					End If

					'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)

					If iPropertyType = enPropertyTypes.SubPrivateType Then
						iCurrentSubpropNum += 1
					End If

					iSelectKeywordStatus = zzGetPropertyType(1, iPropertyType, iAprtDescNum, bNext)
					'DMCommon.Debug.MsgBox("!ByPick1", iSelectPointStatus, iSelectKeywordStatus, iPropertyType, bNext)
					If iSelectKeywordStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						If bNext Then
							iPropertyType = enPropertyTypes.ApartType
							iCurrentPropID += 1
							If bNewPropID Then
								iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
							End If
							If iCurrentPolygonColor = 6 Then
								iCurrentPolygonColor = 1
							Else
								iCurrentPolygonColor += 1
							End If
						Else

						End If
					ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
						Exit Do
					End If
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do
				End If
			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzSelectByPick_Al2(bNewPropID As Boolean)
		'	 מספור' נכסים  
		'Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		'	Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		'Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = zzGetPropertyType(False)
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		'	Dim iCurrentSubpropNum As Integer
		Dim iCurrentPolygonColor As Integer = 1

		'Dim bCurrentMain As Boolean = True
		'	Dim bNext As Boolean = False
		If bmBamash.BamashPgons Is Nothing Then
			BamashNet.bmBamash.Calculate(bmBamash.SubNumerationOptions.EmptyFirst, mtMapThemeData)
			zzSetMinMax()
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If String.IsNullOrEmpty(Me.txtPropID.Text) Then
			iCurrentPropID = 1
		ElseIf Not Integer.TryParse(Me.txtPropID.Text, iCurrentPropID) Then
			iCurrentPropID = 1
		End If
		If bNewPropID Then
			iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
		End If
		If Me.chkColor.Checked Then
			If String.IsNullOrEmpty(Me.cmbChoiceColor.Text) Then
				iCurrentPolygonColor = 1
			ElseIf Integer.TryParse(Me.cmbChoiceColor.Text, iCurrentPolygonColor) Then
				iCurrentPolygonColor += 1
			End If
		Else
			iCurrentPolygonColor = -1
		End If




		'	DMCommon.Debug.MsgBox("", iCurrentPropID, iCurrentPgonColor, iCurrentSubpropNum, Me.txtPropID.Text, Me.cmbChoiceColor.Text)
		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology, zzAddMsgFromPropID(iCurrentPropID, 0, iPropertyType))

				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2", iCurrentPropID, iPropertyType, -1, iCurrentPolygonColor)
					oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, -1, -1, iCurrentPolygonColor)
					oBamashPgon.UpdateMainData()
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then
					iCurrentPropID += 1
					If bNewPropID Then
						iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
					End If

					If iCurrentPolygonColor = 6 Then
						iCurrentPolygonColor = 1
					Else
						iCurrentPolygonColor += 1
					End If
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do

				End If

				'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)

			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub

	Private Sub zzSelectByPick_Al3()
		'  צמידויות מספור  
		'Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		'	Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		'Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = zzGetPropertyType(True)
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		'Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		'	Dim iCurrentPolygonColor As Integer = 1
		'Dim bCurrentMain As Boolean = True
		Dim bNext As Boolean = False
		If bmBamash.BamashPgons Is Nothing Then
			BamashNet.bmBamash.Calculate(bmBamash.SubNumerationOptions.EmptyFirst, mtMapThemeData)
			zzSetMinMax()
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		'

		'

		If String.IsNullOrEmpty(Me.txtSubpropNum.Text) Then
			iCurrentSubpropNum = 1
		ElseIf Not Integer.TryParse(Me.txtSubpropNum.Text, iCurrentSubpropNum) Then
			iCurrentSubpropNum = 1
		End If

		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology, zzAddMsgFromPropID(0, iCurrentSubpropNum, iPropertyType))
				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then

					iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
					oBamashPgon.SetMainData(-1, iPropertyType, iCurrentSubpropNum, -1, -1)
					oBamashPgon.UpdateMainData()

					'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)


					iCurrentSubpropNum += 1



				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do
				End If
			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub





	'
	Private Sub zzSelectByPick_Al4(bNewPropID As Boolean)
		''''  מספור' נכסים וצמידויות


		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = enPropertyTypes.SubPrivateType
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		Dim iPolygonColor As Integer = 0

		'Dim bCurrentMain As Boolean = True
		'	Dim bNext As Boolean = False
		If bmBamash.BamashPgons Is Nothing Then
			BamashNet.bmBamash.Calculate(bmBamash.SubNumerationOptions.EmptyFirst, mtMapThemeData)
			zzSetMinMax()
		End If
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		If String.IsNullOrEmpty(Me.txtPropID.Text) Then
			iCurrentPropID = 1
		ElseIf Not Integer.TryParse(Me.txtPropID.Text, iCurrentPropID) Then
			iCurrentPropID = 1
		End If
		If bNewPropID Then
			iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
		End If

		If String.IsNullOrEmpty(Me.txtSubpropNum.Text) Then
			iCurrentSubpropNum = 1
		ElseIf Not Integer.TryParse(Me.txtSubpropNum.Text, iCurrentSubpropNum) Then
			iCurrentSubpropNum = 1
		End If



		'	DMCommon.Debug.MsgBox("", iCurrentPropID, iCurrentPgonColor, iCurrentSubpropNum, Me.txtPropID.Text, Me.cmbChoiceColor.Text)
		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology, zzAddMsgFromPropID(iCurrentPropID, iCurrentSubpropNum, iPropertyType))

				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2", iCurrentPropID, iPropertyType, -1, iCurrentPolygonColor)
					iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
					oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, -1, iPolygonColor)
					oBamashPgon.UpdateMainData()
					iCurrentSubpropNum += 1
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then
					iCurrentPropID += 1
					If bNewPropID Then
						iCurrentPropID = BamashPolygon.CheckPropertyID(iCurrentPropID)
					End If


				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do

				End If

				'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)

			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Sub zzSetDataByPick()
		'Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		'	Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = False
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iSelectKeywordStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iPropertyType As enPropertyTypes = enPropertyTypes.ApartType
		Dim iAprtDescNum As Integer = 0
		Dim iLastPropID As Integer = BamashPolygon.MaxPropertyID
		Dim iCurrentPropID As Integer
		Dim iCurrentSubpropNum As Integer
		Dim iCurrentPgonColor As Integer = 1
		'Dim bCurrentMain As Boolean = True
		Dim bNext As Boolean = False
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)





		'Dim iRowIndex As Integer
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do


				iSelectPointStatus = zzSelectBamashPgon(oBamashPgon, oBamashTopology)

				If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then


					'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)
					oBamashPgon.SetMainData(iCurrentPropID, iPropertyType, iCurrentSubpropNum, -1, iCurrentPgonColor)
					If iPropertyType = enPropertyTypes.SubPrivateType Then
						iCurrentSubpropNum += 1
					End If
					oBamashPgon.UpdateMainData()


					iSelectKeywordStatus = zzGetPropertyType(1, iPropertyType, iAprtDescNum, bNext)
					'DMCommon.Debug.MsgBox("!ByPick1", iSelectPointStatus, iSelectKeywordStatus, iPropertyType, bNext)
					If iSelectKeywordStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						If bNext Then
							iPropertyType = enPropertyTypes.ApartType
							iCurrentPropID += 1
							If iCurrentPgonColor = 6 Then
								iCurrentPgonColor = 1
							Else
								iCurrentPgonColor += 1
							End If
						Else

						End If
					ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
						Exit Do
					End If
				ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					Exit Do
				End If
			Loop

			oBamashTopology.Close()
			Me.Show()
			Common.SetAcadFocus()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub

	Private Function zzSelectPoint(ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d, bOneOnly As Boolean, Optional sAddMsg As String = Nothing) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOptDebug As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("")
		Dim sBaseMsg As String = " Select Point"
		If Not String.IsNullOrEmpty(sAddMsg) Then
			sBaseMsg &= sAddMsg
		End If
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(sBaseMsg)

		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		oPromptOpt.AllowNone = Not bOneOnly

		ptRes = oEditor.GetPoint(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = ptRes.Value
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "1:Unidiv-SelectPoint")
			End Try
		End If
		Return ptRes.Status
	End Function
	Private Function zzSelectBamashPgon(ByRef oBamashPgon As BamashPolygon, ByRef oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel, Optional sAddMsg As String = Nothing) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = False
		Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		Do
			iSelectPointStatus = zzSelectPoint(tPoint, bOneOnly, sAddMsg)
			'''''''''''''''''''''	DMCommon.Debug.MsgBox("!zzSelectBamashPgon", iSelectPointStatus, bOneOnly, sAddMsg)
			If iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then

				Try
					oPolygon = oBamashTopology.FindPolygon(tPoint)
					If oPolygon IsNot Nothing Then
						oBamashPgon = bmBamash.GetBamashPgon(oPolygon.ID)
						'	DMCommon.Debug.MsgBox("!BamashPgon", oBamashPgon.PropertyID, oBamashPgon.PropertyType)
						If oBamashPgon IsNot Nothing Then
							Return iSelectPointStatus
						Else
							DMCommon.Debug.MsgBox("!SelectBamashPgon", "Err #2178")
						End If
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "1:Bamash-SelectPoint")
				End Try
			ElseIf iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel OrElse iSelectPointStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then
				oBamashPgon = Nothing
				Return iSelectPointStatus
			Else
				DMCommon.Debug.MsgBox("!zzSelectPoint", iSelectPointStatus)
			End If
		Loop


	End Function
	'zzGetPropertyType
	Private Function zzGetPropertyType(iOption As Integer, ByRef iPropertyType As enPropertyTypes, ByRef iAprtDescNum As Integer, ByRef bNext As Boolean) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Const sPromtMsgH As String = "בחר סוג נכס:"
		Const sNextProp As String = "Next"
		'Const sNextProp As String = "נכס-הבא"
		'Const sPromtMsgE As String = "Select Property Type"
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptKeywordOpt As Autodesk.AutoCAD.EditorInput.PromptKeywordOptions = New Autodesk.AutoCAD.EditorInput.PromptKeywordOptions(vbLf & sPromtMsgH)
		Dim saKeyWordList() As String '= {"2-מרפסת", "3-מחסן", "4-צמ'פרטית", "5-צמ'משותפת", sNextProp}  '"נכס-הבא"
		Dim oKeywordRes As Autodesk.AutoCAD.EditorInput.PromptResult
		Dim saValue() As String
		Dim oDynObject As System.Object
		Select Case iOption
			Case 1
				saKeyWordList = {"2-מרפסת", "3-מחסן", "4-צמ'פרטית", "5-צמ'משותפת", sNextProp}  '"נכס-הבא"
			Case 2
				saKeyWordList = {"14-מרפסת", "5-מחסן", sNextProp}  '"נכס-הבא"
			Case Else
				saKeyWordList = {}
		End Select
		For iIndex As Integer = 0 To saKeyWordList.GetUpperBound(0)
			oPromptKeywordOpt.Keywords.Add(saKeyWordList(iIndex))
		Next
		oPromptKeywordOpt.AllowNone = False

		oKeywordRes = oEditor.GetKeywords(oPromptKeywordOpt)


		If oKeywordRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then

			Try
				If oKeywordRes.StringResult.ToUpper() = sNextProp.ToUpper() Then '  "NEXT"
					bNext = True
				Else
					bNext = False
					saValue = Split(oKeywordRes.StringResult, "-")

					oDynObject = [Enum].Parse(GetType(enPropertyTypes), saValue(0))
					'DMCommon.Debug.MsgBox("!PropType", oKeywordRes.StringResult, saValue(0), oDynObject, oDynObject.GetType())
					Select Case iOption
						Case 1
							iPropertyType = DirectCast(oDynObject, enPropertyTypes)
						Case 2
							iPropertyType = enPropertyTypes.SubPrivateType
							iAprtDescNum = DirectCast(oDynObject, enPropertyTypes)
					End Select

				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "1:Unidiv-SelectPoint")
			End Try
		End If
		Return oKeywordRes.Status
	End Function
	Private Sub zzSetMinMax()
		If BamashPolygon.MinPropertyID <> 0 Then
			Me.txtMinPropertyID.Text = BamashPolygon.MinPropertyID.ToString()
		Else
			Me.txtMinPropertyID.Text = String.Empty
		End If
		If BamashPolygon.MaxPropertyID <> 0 Then
			Me.txtMaxPropertyID.Text = BamashPolygon.MaxPropertyID.ToString()
		Else
			Me.txtMaxPropertyID.Text = String.Empty
		End If

		If BamashPolygon.MinSubPropNum <> 0 Then
			Me.txtMinSubpropNum.Text = BamashPolygon.MinSubPropNum.ToString()
		Else
			Me.txtMinSubpropNum.Text = String.Empty
		End If
		If BamashPolygon.MaxSubPropNum <> 0 Then
			Me.txtMaxSubpropNum.Text = BamashPolygon.MaxSubPropNum.ToString()
		Else
			Me.txtMaxSubpropNum.Text = String.Empty
		End If
	End Sub


	Private Sub cmdRefreshMinMaxProprtyID_Click(oSender As System.Object, e As EventArgs)
		bmBamash.LoadPolygons(mtMapThemeData)
		zzSetMinMax()
		If False Then
			If BamashPolygon.MinPropertyID <> 0 Then
				Me.txtMinPropertyID.Text = BamashPolygon.MinPropertyID.ToString()
			End If
			If BamashPolygon.MaxPropertyID <> 0 Then
				Me.txtMaxPropertyID.Text = BamashPolygon.MaxPropertyID.ToString()
			End If
		End If
	End Sub

	Private Sub cmdSetDataByPick_Click(oSender As System.Object, e As EventArgs) Handles cmdSetDataByPick.Click
		'''''Me.Visible = False
		'Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
		zzSetConstValues()
		'Me.Visible = True
	End Sub
	Private Sub txtPropID_Validating(oSender As System.Object, e As CancelEventArgs) Handles txtPropID.Validating
		Dim iPropID As Integer
		Dim sText As String = Me.txtPropID.Text.Trim()
		If String.IsNullOrEmpty(sText) Then
			Me.chkPropID.Checked = False
		ElseIf Integer.TryParse(sText, iPropID) Then
			Me.chkPropID.Checked = True
		Else
			e.Cancel = True
		End If
	End Sub

	Private Sub mfBamashBlockRefs_SelectBamahPgon(ByRef oBamashPgon As BamashPolygon, ByRef oBamashTopology As TopologyModel) Handles mfBamashBlockRefs.SelectBamahPgon
		zzSelectBamashPgon(oBamashPgon, oBamashTopology)
	End Sub

	Private Sub cmbAprtDesc_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbAprtDesc.SelectedIndexChanged
		If Not Me.chkAprtDesc.Checked Then
			Me.chkAprtDesc.Checked = True
		End If
	End Sub

	Private Sub txtBldFloor_Validated(oSender As System.Object, e As EventArgs) Handles txtBldFloor.Validated
		If Me.txtBldFloor.Text.Length <> 0 AndAlso Not Me.chkBldFloor.Checked Then
			Me.chkBldFloor.Checked = True
		End If
	End Sub
	Private Sub txtBldPart_Validated(oSender As System.Object, e As EventArgs) Handles txtBldPart.Validated
		If Me.txtBldPart.Text.Length <> 0 AndAlso Not Me.chkBldPart.Checked Then
			Me.chkBldPart.Checked = True
		End If
	End Sub
	Private Sub txtBldEntr_Validated(oSender As System.Object, e As EventArgs) Handles txtBldEntr.Validated
		If Me.txtBldEntr.Text.Length <> 0 AndAlso Not Me.chkBldEntr.Checked Then
			Me.chkBldEntr.Checked = True
		End If
	End Sub
	Private Sub txtBldNo_Validated(oSender As System.Object, e As EventArgs) Handles txtBldNo.Validated
		If Me.txtBldNo.Text.Length <> 0 AndAlso Not Me.chkBldNo.Checked Then
			Me.chkBldNo.Checked = True
		End If
	End Sub

	Private Sub cmbPropertyType_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbPropertyType.SelectedIndexChanged
		If Not Me.chkPropertyType.Checked Then
			Me.chkPropertyType.Checked = True
		End If

	End Sub

	Private Sub cmdSetDataBySelSet_Click(oSender As System.Object, e As EventArgs) Handles cmdSetDataBySelSet.Click
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet

		oRes = oEditor.GetSelection()

		oSelSet = oRes.Value
		DMCommon.Debug.MsgBox("290720_1", oRes.Status, oSelSet.Count)
		zzSetConstValues(oSelSet)
	End Sub

	Private Sub cmbChoiceColor_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbChoiceColor.SelectedIndexChanged

		If Not Me.chkColor.Checked Then
			Me.chkColor.Checked = True
		End If
	End Sub

	Private Sub txtSubpropNum_Validated(oSender As System.Object, e As EventArgs) Handles txtSubpropNum.Validated
		If Me.txtSubpropNum.Text.Length <> 0 AndAlso Not Me.chkSubproperty.Checked Then
			Me.chkSubproperty.Checked = True
		End If
	End Sub



	Private Sub frmBamash_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			'	Me.Size = New System.Drawing.Size(1324, 488)
			Me.WindowState = FormWindowState.Normal
		End If
	End Sub


	Private Sub txtParcelNo_Validated(oSender As System.Object, e As EventArgs) Handles txtParcelNo.Validated
		If Not String.IsNullOrEmpty(Me.txtParcelNo.Text) Then
			'zzUpdateParcel(Me.txtParcelNo.Text)
		End If

	End Sub
	Private Sub cmdClearPaint_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClearPaint.Click
		BamashNet.bmBamash.ClearPaint()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub

	Private Sub cmdExit_Click(sender As System.Object, e As EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub


	Private Sub txtBreakHeight_Validated(sender As System.Object, e As EventArgs) Handles txtBreakHeight.Validated
		If Not Double.TryParse(Me.txtBreakHeight.Text, mdBreakHeight) Then
			mdBreakHeight = 0.0
		End If
	End Sub

	Private Sub txtSpacing_Validated(sender As System.Object, e As EventArgs) Handles txtSpacing.Validated
		If Not Double.TryParse(Me.txtSpacing.Text, mdSpacing) Then
			mdSpacing = 1.0
		End If
	End Sub

	Private Sub cmdGetDistBreakHeight_Click(sender As System.Object, e As EventArgs) Handles cmdGetDistBreakHeight.Click
		Const sPrompt As String = "בחר גובה טבלה"
		zzGetDistance(sPrompt, Me.txtBreakHeight)
	End Sub
	Private Sub zzGetDistance(sPrompt As String, oTextBox As TextBox)
		Dim bDistance As Double
		Me.Visible = False
		If DMAcadExt.AcadUtil.GetDistance(sPrompt, bDistance) Then
			oTextBox.Text = FormatNumber(bDistance, 1)

		End If
		Me.Visible = True
	End Sub

	Private Sub cmdGetDistSpacing_Click(sender As System.Object, e As EventArgs) Handles cmdGetDistSpacing.Click
		Const sPrompt As String = "בחר מרווח בין טבלאות"
		zzGetDistance(sPrompt, Me.txtSpacing)

	End Sub



	Private Sub frmBamashM_FormClosed(sender As System.Object, e As FormClosedEventArgs) Handles Me.FormClosed
		zzUpdateData()
	End Sub



	Private Sub cmdRecomputeTable_Click(sender As System.Object, e As EventArgs) Handles cmdRecomputeTable.Click
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		If mdBreakHeight > 0.0 Then
			AcadReport.RepApp.ReopenAcadTable()
			AcadReport.RepApp.SetBreak(mdBreakHeight, mdSpacing)

		End If
		AcadReport.RepApp.AcadTable.RecomputeTableBlock(True)
		DMAcadExt.AcadDocument.UpdateScreen()


		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdSelectBlocks_Click(senEditorInputder As System.Object, e As EventArgs) Handles cmdSelectBlocks.Click
		Const sLayer As String = "blds027"
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions
		Dim oPromptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim oaValues() As DatabaseServices.TypedValue = {New DatabaseServices.TypedValue(DatabaseServices.DxfCode.LayerName, sLayer)}
		'	oaValues(0) = New TypedValue(DxfCode.Color, iSelectedColorIndex)
		'	oaValues(1) = New TypedValue(DxfCode.LayerName, sLayers)

		Dim oFilter As EditorInput.SelectionFilter = New EditorInput.SelectionFilter(oaValues)
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult = Nothing
		Dim taEmptyObjectIDs() As DatabaseServices.ObjectId = {}
		Dim taCurrentAcObjIds() As DatabaseServices.ObjectId
		Dim taAddAcObjIds() As DatabaseServices.ObjectId
		Dim taNewAcObjIds() As DatabaseServices.ObjectId
		Dim iPtResCount As Integer = 0
		Dim colCurrentAcObjIds As DatabaseServices.ObjectIdCollection
		Try
			ptRes = oEditor.SelectImplied()
		Catch oEx As Exception
			'System.Windows.Forms.MessageBox.Show("Error #2716" & vbCrLf & oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & msDictionaryParcel, "TPlanner")
			System.Windows.Forms.MessageBox.Show("Error #2716" & vbCrLf & oEx.Message, "TPlanner")

		End Try
		If ptRes IsNot Nothing AndAlso ptRes.Status = EditorInput.PromptStatus.OK Then

			System.Windows.Forms.MessageBox.Show(ptRes.Status.ToString())
			taCurrentAcObjIds = ptRes.Value.GetObjectIds()
			colCurrentAcObjIds = New DatabaseServices.ObjectIdCollection(taCurrentAcObjIds)


			oEditor.SetImpliedSelection(taEmptyObjectIDs)
			For Each tAcObjID As DatabaseServices.ObjectId In taCurrentAcObjIds
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!+1 taCurrentAcObjIds", tAcObjID)
			Next
		Else
			colCurrentAcObjIds = New DatabaseServices.ObjectIdCollection()
			taCurrentAcObjIds = {}
		End If
		oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		oPromptOpt.AllowDuplicates = False

		'	oPromptOpt.SingleOnly = bSingleOnly
		oPromptOpt.SinglePickInSpace = False ' bSingleOnly '
		oPromptOpt.AllowSubSelections = True
		Me.Visible = False
		'	ptRes.Value.GetObjectIds()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		oPromptRes = oEditor.GetSelection(oPromptOpt, oFilter)

		'	DMCommon.Debug.MsgBox("!Impl", iPtResCount, oPromptRes.Value.Count, colCurrentAcObjIds.Count)

		If (oPromptRes IsNot Nothing) AndAlso oPromptRes.Status = EditorInput.PromptStatus.OK Then
			'oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(oPromptRes.Value.)
			If taCurrentAcObjIds IsNot Nothing AndAlso taCurrentAcObjIds.GetUpperBound(0) >= 0 Then
				taAddAcObjIds = oPromptRes.Value.GetObjectIds()
				For Each tAcObjID As DatabaseServices.ObjectId In taAddAcObjIds
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!-1 taAddAcObjIds", tAcObjID)
				Next
				DMAcadExt.AcadUtil.AddObjectIDCollection(colCurrentAcObjIds, taAddAcObjIds)
				ReDim taNewAcObjIds(colCurrentAcObjIds.Count - 1)
				For Each tAcObjID As DatabaseServices.ObjectId In taNewAcObjIds
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!taNewAcObjIds", tAcObjID)
				Next

				colCurrentAcObjIds.CopyTo(taNewAcObjIds, 0)
				For Each tAcObjID As DatabaseServices.ObjectId In taNewAcObjIds
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!taNewAcObjIds", tAcObjID)
				Next


				oEditor.SetImpliedSelection(taNewAcObjIds)

			Else
				oEditor.SetImpliedSelection(oPromptRes.Value)
			End If

		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

		Me.Visible = True
	End Sub

	Private Sub cmbNumerationPropID_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbNumerationPropID.SelectedIndexChanged

	End Sub
End Class