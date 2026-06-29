Option Explicit On
Option Strict On
'Imports  
Public Class bmBamash

	Public Enum AttributeNo
		miUserIDAttrNo = 0
		miPropidAttrNo = 1
		miSubParcelNoAttrNo = 2
		miAprtdescAttrNo = 3
		miAprtdesc2AttrNo = 4
		miProptypeAttrNo = 5
		miAprtdescnumAttrNo = 6
		miBldfloorAttrNo = 7
		miPolygoncolorAttrNo = 8
		miBldentrAttrNo = 9
		miBldpartAttrNo = 10
		miBldnoAttrNo = 11
		miPolygonidAttrNo = 12
		miBldfloordescAttrNo = 13
		miPolygonAreaAttrNo = 14
	End Enum
	Public Enum SubNumerationOptions
		All
		EmptyFirst
		EmptyMax
		None
	End Enum
	Public Enum CalculateOptions
		Renum = 0
		NumEmpty = 1
		None = 2
	End Enum
	Private Const msCaptionOptionSettingKey As String = "CaptionOption"
	Private Const msZebraWidthDrawingSettingKey As String = "ZebraWidthDrawing"
	Private Const msZebraWidthTableSettingKey As String = "ZebraWidthTable"
	Private Const msBufferOffsetSettingKey As String = "BufferOffset"
	Private Const msZoomRadiusSettingKey As String = "ZoomRadius"

	Private Const mdZebraWidthDrawingDflt As Double = 0.5
	Private Const mdZebraWidthTableDflt As Double = 0.1
	Private Const mdBufferOffsetDflt As Double = 0.2
	Private Const mdZoomRadiusDflt As Double = 1.0

	Private Shared msParcel As String
	Private Shared moPaintLayerDef As DMAcadExt.AcadLayerDef
	Private Shared mbOpened As Boolean = False
	Private Shared mbBldNoExist As Boolean
	Private Shared mbBldPartExist As Boolean
	Private Shared mbBldEntranceExist As Boolean
	Private Shared msTopoName As String
	Private Shared mpgaShared As bmPolygonArray
	Private Shared miSubNumeration As SubNumerationOptions

	'	Private mlID As Long
	'	Private mlPropID As Long
	'	Private mlPropType As Long
	'	Private mlBldNo As Long
	'	Private mlBldPart As Long
	'	Private mlBldEntrance As Long
	'	Private mlBldFloor As Long
	'	Private mdPolygonArea As Double

	Private Const miUserIDAttrNo As Integer = 0
	Private Const miPropidAttrNo As Integer = 1
	Private Const miSubParcelNoAttrNo As Integer = 2
	Private Const miAprtdescAttrNo As Integer = 3
	Private Const miAprtdesc2AttrNo As Integer = 4
	Private Const miProptypeAttrNo As Integer = 5
	Private Const miAprtdescnumAttrNo As Integer = 6
	Private Const miBldfloorAttrNo As Integer = 7
	Private Const miPolygoncolorAttrNo As Integer = 8
	Private Const miBldentrAttrNo As Integer = 9
	Private Const miBldpartAttrNo As Integer = 10
	Private Const miBldnoAttrNo As Integer = 11
	Private Const miPolygonidAttrNo As Integer = 12
	Private Const miBldfloordescAttrNo As Integer = 13
	Private Const miPolygonAreaAttrNo As Integer = 14

	'Private Const mlAreaUnitsAttrNo As Long

	Private Shared msBamashTopoName As String
	Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmBamash
	Private Shared moResource As TPlServerDB.TPlResource
	Private Shared moPropTypeResource As TPlServerDB.TPlResource



	Private Shared miSubPropCounter As Integer

	'Private mvTopoRefs
	Private Shared mrsMain As DataTable
	'Private Shared mrsPrint As DataTable

	Private Shared iCurrentRowIndex As Integer
	'Private mrsPrint As ADODB.Recordset
	Private Shared msPaintLayer As String
	Private Shared mdSumExproArea As Double
   Private Shared moBamashTopo As Autodesk.Gis.Map.Topology.TopologyModel

	Private Shared mdicHebNumbers As Generic.Dictionary(Of String, Integer)
	Private Shared miCaptionOption As CalculateOptions = CalculateOptions.Renum
	'	Private Shared miColorOption As CalculateOptions
	Private Shared moSharedRepTable As DataTable

	Private Shared mdZebraWidthDrawing As DMCommon.DMValue
	Private Shared mdZebraWidthTable As DMCommon.DMValue
	Private Shared mdBufferOffset As DMCommon.DMValue
	Private Shared mdZoomRadius As DMCommon.DMValue




#Region "Net"

	'Private Shared moMainDataTable As System.Data.DataTable
	Private Shared mdicBamashPgons As BamashPgons
	'Private Shared mdicBamashPgonsByEntity As BamashPgons

	Private Shared mdicPgonGroups As bmPgonGroups

	Private Shared mdicProperties As Generic.SortedDictionary(Of Integer, bmProperty)
	Private Shared mdicPropertyCounter As Generic.Dictionary(Of bmPropUnitKey, Integer)
	Private Shared moBamashProjectData As BamashProjectData
	Private Shared mdicSubPropNum As SubPropNumDic

	Public Shared Function GetAfricaTitleView() As DataView
		Dim oAfricaTitleTable As DataTable = New DataTable("AfricaTitle")
		Dim oNewRow As System.Data.DataRow
		With oAfricaTitleTable.Columns
			.Add("Block", GetType(System.String))
			.Add("Parcel", GetType(System.String))
			.Add("Street", GetType(System.String))
			.Add("BuildNum", GetType(System.String))
			.Add("City", GetType(System.String))
			.Add("PropertiesNum", GetType(System.Int32))
		End With
		oNewRow = oAfricaTitleTable.NewRow()
		With oNewRow
			.Item("Block") = moBamashProjectData.BlockNo
			.Item("Parcel") = moBamashProjectData.Parcel
			.Item("Street") = moBamashProjectData.Street
			.Item("BuildNum") = moBamashProjectData.BuildNum
			.Item("City") = moBamashProjectData.LocalityName
			.Item("PropertiesNum") = PropertyCount
		End With
		oAfricaTitleTable.Rows.Add(oNewRow)
		Return New DataView(oAfricaTitleTable)
	End Function
	Public Shared Function GetAfricaDataView() As DataView
		Dim oAfricaTable As DataTable = zzCreateAfricaTable()
		Dim oNewRow As System.Data.DataRow
		Dim oSubproperties As bmSubproperties
		Dim oPgon As BamashPolygon
		Dim dArea As Double
		Dim oaBalconies() As BamashPolygon
		Dim sIndex As String

		Dim iTemp As Integer = 0
		For Each oProperty As bmProperty In mdicProperties.Values
			If oProperty.HasID Then
				oNewRow = oAfricaTable.NewRow()
				With oNewRow
					.Item("Company") = oProperty.UserCompany
					.Item("Site") = oProperty.UserSite
					.Item("Build") = oProperty.UserBuild
					.Item("Apart") = oProperty.UserApart
					.Item("Parcel") = oProperty.PropID
					.Item("PropArea") = oProperty.MainArea
					oSubproperties = oProperty.SubProperties
					dArea = 0
					For iIndex As Integer = 0 To oSubproperties.PgonGroundCount - 1
						oPgon = oSubproperties.PgonGround(iIndex)
						dArea += oPgon.Area
						sIndex = Convert.ToString(iIndex + 1)
						.Item("GroundMark" & sIndex) = oPgon.Caption
					Next
					If oSubproperties.PgonGroundCount > 0 Then
						.Item("GroundArea") = dArea
					End If
					oaBalconies = oProperty.Balconies
					If oaBalconies IsNot Nothing Then
						For iIndex As Integer = 0 To oaBalconies.GetUpperBound(0)
							If iIndex < 3 Then
								sIndex = Convert.ToString(iIndex + 1)
								.Item("BalconyArea" & sIndex) = oaBalconies(iIndex).Area
								.Item("BalconyMark" & sIndex) = oaBalconies(iIndex).Caption
							Else
								'Print Error
							End If
						Next
					End If
					oPgon = oSubproperties.PgonRoof
					If oPgon IsNot Nothing Then
						.Item("RoofArea") = oPgon.Area
						.Item("RoofMark") = oPgon.Caption
					End If

					For iIndex As Integer = 0 To oSubproperties.PgonOthersCount - 1
						sIndex = Convert.ToString(iIndex + 1)
						oPgon = oSubproperties.PgonOthers(iIndex)
						If oPgon IsNot Nothing Then
							.Item("SubpropType" & sIndex) = oPgon.ApartDesc
							.Item("SubpropUserID" & sIndex) = oPgon.UserID
							.Item("SubpropArea" & sIndex) = oPgon.Area
							.Item("SubpropMark" & sIndex) = oPgon.Caption
						End If
					Next
					oAfricaTable.Rows.Add(oNewRow)
				End With
				iTemp += 1
				If iTemp = 5 Then
					'' Exit For
				End If
			End If
		Next
		Return New DataView(oAfricaTable, String.Empty, "Parcel", DataViewRowState.CurrentRows)
	End Function
	Public Shared Function GetSumData() As Dictionary(Of String, String)
		Dim dicData As Dictionary(Of String, String) = New Dictionary(Of String, String)
		dicData.Add("BLOCKNO", moBamashProjectData.BlockNo)
		dicData.Add("PARCELNO", moBamashProjectData.Parcel)
		dicData.Add("TOTALAREA", Convert.ToString(moBamashProjectData.TotalArea))
		dicData.Add("CITY", ToDOS(moBamashProjectData.LocalityName))
		dicData.Add("STREET", ToDOS(moBamashProjectData.Street))
		dicData.Add("BLDNUM", ToDOS(moBamashProjectData.BuildNum))
		Return dicData
	End Function
	Public Shared Function GetStampData() As Dictionary(Of String, String)
		Dim dicData As Dictionary(Of String, String) = New Dictionary(Of String, String)
		If moBamashProjectData.BlockNo IsNot Nothing Then
			dicData.Add("BLOCKNO", moBamashProjectData.BlockNo)
		End If

		If moBamashProjectData.Parcel IsNot Nothing Then
			dicData.Add("PARCELNO", moBamashProjectData.Parcel)
		End If

		If moBamashProjectData.LocalityName IsNot Nothing Then
			dicData.Add("CITY", ToDOS(moBamashProjectData.LocalityName))
		End If
		If moBamashProjectData.Street IsNot Nothing Then
			dicData.Add("STREET", DMCommon.Hebrew.ToDOS(moBamashProjectData.Street))
		End If
		If moBamashProjectData.BuildNum IsNot Nothing Then
			dicData.Add("BLDNUM", ToDOS(moBamashProjectData.BuildNum))
		End If

		Return dicData
	End Function

	Public Shared Function GetBamashPgon(ByVal iBamashTopoID As Integer) As BamashPolygon
		Dim oBamashPolygon As BamashPolygon = Nothing
		If Not mdicBamashPgons.TryGetValue(iBamashTopoID, oBamashPolygon) Then
			MessageBox.Show("TopoPgon # " & CStr(iBamashTopoID) & " was not found", "12_491")
		End If
		Return oBamashPolygon

	End Function
	Public Shared ReadOnly Property BamashPgons As BamashPgons
		Get
			Return mdicBamashPgons
		End Get
	End Property
	Public Shared Function GetBamashPgon(ByVal tCentroidObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId) As BamashPolygon
		Dim oBamashPolygon As BamashPolygon = Nothing
		If Not mdicBamashPgons.TryGetValueByEntity(tCentroidObjId, oBamashPolygon) Then
			'MessageBox.Show("TopoPgon ID= " & tCentroidObjId.ToString() & " was not found", "12_491")
		End If
		Return oBamashPolygon

	End Function


#End Region

	Private Shared Sub zzCreateShareTable()
		mrsShare = New DataTable("Share")

		With mrsShare.Columns
			.Add("ID", GetType(System.Int32))
			.Add("PropID", GetType(System.String))
			.Add("PropType", GetType(System.Int32))
			.Add("BldNo", GetType(System.Int32))
			.Add("BldPart", GetType(System.Double))	 'ADODB.DataTypeEnum.adInteger
			.Add("BldEntrance", GetType(System.Int32))
			.Add("BldFloor", GetType(System.Int32))
			.Add("ApartDescrID", GetType(System.Int32))
			.Add("Color", GetType(System.Int32))
			.Add("PolygonArea", GetType(System.Double))
			.Add("Caption", GetType(System.String))
			.Add("ApartDescr", GetType(System.String))
			.Add("FloorDescr", GetType(System.String))
			.Add("TopoID", GetType(System.Int32))
			.Add("UserID", GetType(System.String))
		End With


	End Sub
	Public Shared ReadOnly Property PropertyCount() As Integer
		Get
			Dim iPropZero As Integer
			If mdicProperties.ContainsKey(0) Then
				iPropZero = 1
			Else
				iPropZero = 0
			End If
			Return mdicProperties.Count - iPropZero

		End Get
	End Property
	
	Public Shared ReadOnly Property BamashTopo() As Autodesk.Gis.Map.Topology.TopologyModel
		Get
			Return moBamashTopo
		End Get
	End Property
	Public Shared Property ZebraWidthDrawing() As Double
		Get
			Return mdZebraWidthDrawing.DoubleValue
		End Get
		Set(ByVal dValue As Double)
			mdZebraWidthDrawing.Update(dValue)
		End Set
	End Property

	Public Shared Property ZebraWidthTable() As Double
		Get
			Return mdZebraWidthTable.DoubleValue
		End Get
		Set(ByVal dValue As Double)
			mdZebraWidthTable.Update(dValue)
		End Set
	End Property
	Public Shared Property BufferOffset() As Double
		Get
			Return mdBufferOffset.DoubleValue
		End Get
		Set(ByVal dValue As Double)
			mdBufferOffset.Update(dValue)
		End Set
	End Property
	Public Shared Property ZoomRadiusAAA() As Double
		Get
			Return mdZoomRadius.DoubleValue
		End Get
		Set(ByVal dValue As Double)
			mdZoomRadius.Update(dValue)
		End Set
	End Property
	Private Shared Sub zzOpenBamashTopo(ByVal iOpenMode As Autodesk.Gis.Map.Topology.OpenMode)
		moBamashTopo = TopoManager.TopoCreator.GetOpenedTopology(msTopoName, iOpenMode, False, True)
	End Sub
	Private Shared Sub zzCloseBamashTopo()
		If moBamashTopo IsNot Nothing Then
			Try
				If moBamashTopo.Status <> Autodesk.Gis.Map.Topology.Status.Closed Then
					moBamashTopo.Close()
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Bamash - zzCloseBamashTopo")
			End Try
		End If
	End Sub

	Public Shared Property CaptionOption() As CalculateOptions
		Get
			Return miCaptionOption
		End Get
		Set(ByVal iValue As CalculateOptions)
			miCaptionOption = iValue
		End Set
	End Property
	Public Shared Property CaptionOptionSetting() As CalculateOptions
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msCaptionOptionSettingKey, "1")
			Dim iSetting As Integer
			Try
				iSetting = Convert.ToInt32(sSettting)
			Catch oEx As Exception
				iSetting = -1
			End Try
			If iSetting = -1 Then
				Return CalculateOptions.NumEmpty
			Else
				Return CType(iSetting, CalculateOptions)
			End If
		End Get
		Set(ByVal iValue As CalculateOptions)
			Microsoft.VisualBasic.SaveSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msCaptionOptionSettingKey, CStr(iValue))
		End Set
	End Property
	Private Shared Sub zzCreateMainTable()
		mrsMain = New DataTable("Main")
		With mrsMain.Columns
			.Add("ID", GetType(System.Int32))
			.Add("PropID", GetType(System.Int32))
			.Add("PropType", GetType(System.Int32))
			.Add("BldNo", GetType(System.Int32))
			.Add("BldPart", GetType(System.Int32))
			.Add("BldEntrance", GetType(System.Int32))
			.Add("BldFloor", GetType(System.Double))
			.Add("ApartDescrID", GetType(System.Int32))
			.Add("Color", GetType(System.Int32))
			.Add("PolygonArea", GetType(System.Double))
			.Add("Caption", GetType(System.String))
			.Add("ApartDescr", GetType(System.String))
			.Add("FloorDescr", GetType(System.String))
			.Add("TopoID", GetType(System.Int32))
			.Add("UserID", GetType(System.String))
		End With
	End Sub
	Public Shared Sub Close()

		If mbOpened Then
			Try
				If mrsMain IsNot Nothing Then
					mrsMain.Dispose()
					mrsMain = Nothing
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Bamash - Close")
			End Try
			If mdicBamashPgons IsNot Nothing Then
				mdicBamashPgons.Terminate()
				mdicBamashPgons.Clear()
				mdicBamashPgons = Nothing
			End If
			mbOpened = False
		End If
	End Sub
	Private Shared Sub zzAddPolygon(ByVal oBamashPolygon As BamashPolygon)
		Dim iPropType As enPropertyTypes
		Try
			iPropType = oBamashPolygon.PropertyType

			Select Case iPropType
				Case enPropertyTypes.ZeroType, enPropertyTypes.ZeroFreezeType

				Case enPropertyTypes.SubShareType

					zzAddSharePgon2009(oBamashPolygon)
				Case Else

					zzAddPrivatePgon2009(oBamashPolygon)
			End Select
			If iPropType = enPropertyTypes.ExproType Then
				mdSumExproArea += oBamashPolygon.AcadArea(False)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & oEx.StackTrace, "Bamash - zzAddPolygonNet")
		End Try
	End Sub


	Private Shared Function zzCreateAfricaTable() As DataTable
		Dim oDataTable As DataTable = New DataTable("AfricaData")
		With oDataTable.Columns
			.Add("Company", GetType(System.String))
			.Add("Site", GetType(System.String))
			.Add("Build", GetType(System.String))
			.Add("Apart", GetType(System.String))
			.Add("Parcel", GetType(System.Int32))
			.Add("PropArea", GetType(System.Double))
			.Add("GroundArea", GetType(System.Double))
			.Add("GroundMark1", GetType(System.String))
			.Add("GroundMark2", GetType(System.String))
			.Add("GroundMark3", GetType(System.String))

			.Add("BalconyArea1", GetType(System.Double))
			.Add("BalconyMark1", GetType(System.String))
			.Add("BalconyArea2", GetType(System.Double))
			.Add("BalconyMark2", GetType(System.String))
			.Add("BalconyArea3", GetType(System.Double))
			.Add("BalconyMark3", GetType(System.String))

			.Add("AtticArea", GetType(System.Double))
			.Add("AtticMark", GetType(System.String))
			.Add("RoofArea", GetType(System.Double))
			.Add("RoofMark", GetType(System.String))

			Dim sIndex As String
			For iIndex As Integer = 0 To 6
				sIndex = Convert.ToString(iIndex + 1)
				.Add("SubpropType" & sIndex, GetType(System.String))
				.Add("SubpropUserID" & sIndex, GetType(System.String))
				.Add("SubpropArea" & sIndex, GetType(System.Double))
				.Add("SubpropMark" & sIndex, GetType(System.String))
			Next
			Return oDataTable
		End With
	End Function

	Private Shared Sub zzAddPrivatePgon2009(ByVal oBamashPolygon As BamashPolygon)
		Dim oProperty As bmProperty
		Try
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicPropAddPgon")
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicPropAddPgon", oBamashPolygon.PropertyID, oBamashPolygon.PropertyType)

			If mdicProperties.ContainsKey(oBamashPolygon.PropertyID) Then
				oProperty = mdicProperties.Item(oBamashPolygon.PropertyID)
			Else
				oProperty = New bmProperty(oBamashPolygon.PropertyID)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicPropAddPgon2", oProperty.PropID)
				mdicProperties.Add(oBamashPolygon.PropertyID, oProperty)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicPropCnt2", mdicProperties.Count)
			End If
			If oBamashPolygon.MainData Then
				oProperty.ColorIndex = oBamashPolygon.ColorIndex
				oProperty.ApartDescr = oBamashPolygon.ApartDesc
				oProperty.ApartDescrDOS = oBamashPolygon.ApartDescDOS

				oProperty.UserID = oBamashPolygon.UserID
			End If
			oProperty.AddPolygon2009(oBamashPolygon)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & oEx.StackTrace, "Bamash - zzAddPrivatePgon2009")
		End Try
	End Sub

	Private Shared Function zzAddSharePgon2009(ByVal oBamashPolygon As BamashPolygon) As Integer
		mpgaShared.AddPolygon2009(oBamashPolygon)
		Dim iPropID As Integer
		For iIndex As Integer = 0 To oBamashPolygon.PropertiesUB
			iPropID = oBamashPolygon.PropertyID(iIndex)
			If Not mdicProperties.ContainsKey(iPropID) Then
				mdicProperties.Add(iPropID, New bmProperty(iPropID))
			End If
		Next

	End Function

	Public Shared Sub PaintAllProperties()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		zzPaintPrivate()
		zzPaintShare()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
	End Sub
	Public Shared Sub UpdateCentroids()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		zzUpdateBlock_Net()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
	End Sub
	Public Shared Function Heb2Num(ByVal sValue As String) As Integer
		If mdicHebNumbers Is Nothing Then
			Dim sHebNum As String = String.Empty
			mdicHebNumbers = New Generic.Dictionary(Of String, Integer)
			For iIndex As Integer = 1 To DMCommon.Hebrew.MaxHebNum
				Try
					'''''''''''''''''''''''''''''''''''''''''''	mdicHebNumbers.Add(DMCommon.Hebrew.GetHebNum(iIndex, False), iIndex)
					sHebNum = DMCommon.Hebrew.GetHebNum(iIndex, True)
					mdicHebNumbers.Add(sHebNum, iIndex)



				Catch oEx As Exception
					'System.Windows.Forms.MessageBox.Show("Err #1893" & vbCrLf & CStr(iIndex), "bmBamash")
					DMCommon.Debug.UserMsg("Err #1893", oEx.Message, oEx.StackTrace, sValue, sHebNum, iIndex, mdicHebNumbers.Count)
				End Try
			Next
		End If
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Heb2N", sValue, mdicHebNumbers.ContainsKey(sValue))
		If mdicHebNumbers.ContainsKey(sValue) Then
			Return mdicHebNumbers.Item(sValue)
		Else
			Return 0
		End If
	End Function

	Public Shared Sub Calculate(ByVal iCalcOption As bmBamash.SubNumerationOptions, Optional tMapThemeData As DMAcadExt.MapThemeData = Nothing)
		'	DMAcadExt.AcadDocument.OpenLog()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		bmBamash.InitMemory()
		'	DMAcadExt.AppMessages.Init()
		BamashPolygon.Initialize(tMapThemeData)
		BamashPolygon.Reset()
		msBamashTopoName = tMapThemeData.TopoName
		If zzLoadPolygons(msBamashTopoName) Then


			zzAddAllPolygons()
			''SubNumeration()
			If iCalcOption = SubNumerationOptions.EmptyFirst Then
				mdicSubPropNum.ResetCounter()
			ElseIf iCalcOption = SubNumerationOptions.EmptyMax Then
				mdicSubPropNum.GoToMax()
			End If

			zzCalculateProperties(iCalcOption)
			zzCalcSharedSubproperties(iCalcOption)
			zzCalcGroups()
			zzCalcSharedSubpropertiesII()
			'	DMCommon.Debug.ExcelLog.SetDataTable(0, "MainRepView", bmProperty.MainRepView)

			'	bmBamash.FloorPropEnum()
			'	bmBamash.SubShareEnum()
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'	DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()

		'   RestoreCommandLine()
	End Sub

	Public Shared Sub LoadPolygons(Optional tMapThemeData As DMAcadExt.MapThemeData = Nothing)
		'	DMAcadExt.AcadDocument.OpenLog()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		bmBamash.InitMemory()
		'	DMAcadExt.AppMessages.Init()
		BamashPolygon.Initialize(tMapThemeData)
		BamashPolygon.Reset()
		zzLoadPolygons(tMapThemeData.TopoName)

		'	DMCommon.Debug.ExcelLog.SetDataTable(0, "MainRepView", bmProperty.MainRepView)

		'	bmBamash.FloorPropEnum()
		'	bmBamash.SubShareEnum()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'	DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()

		'   RestoreCommandLine()
	End Sub

	Public Shared Sub SubpropNumerationClear()
		'	DMAcadExt.AcadDocument.OpenLog()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		bmBamash.InitMemory()
		'	DMAcadExt.AppMessages.Init()

		zzSubpropNumerationErase()

		'	DMCommon.Debug.ExcelLog.SetDataTable(0, "MainRepView", bmProperty.MainRepView)

		'	bmBamash.FloorPropEnum()
		'	bmBamash.SubShareEnum()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'	DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
		DMAcadExt.AcadDocument.UpdateScreen()
		'   RestoreCommandLine()
	End Sub


	Public Shared Sub SubpropNumeration(ByVal bByProperty As Boolean)
		'	DMAcadExt.AcadDocument.OpenLog()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		'bmBamash.InitMemory()
		'	DMAcadExt.AppMessages.Init()

		'zzSubpropNumeration()
		DMCommon.Debug.ExcelLog.SetValue(0, "!EmpyColA ", bByProperty)
		If bByProperty Then
			zzSubpropNumerationByProp()
		Else
			zzSubpropNumeration()
		End If


		'	DMCommon.Debug.ExcelLog.SetDataTable(0, "MainRepView", bmProperty.MainRepView)

		'	bmBamash.FloorPropEnum()
		'	bmBamash.SubShareEnum()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		'	DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
		DMAcadExt.AcadDocument.UpdateScreen()
		'   RestoreCommandLine()
	End Sub
	Private Shared Sub zzSubpropNumeration()
		Dim iCurrentSubpropNum As Integer = 1
		Dim colBamashPolygon As IEnumerable(Of BamashPolygon) = From oBamashPolygon As BamashPolygon In mdicBamashPgons.Values
																				  Order By oBamashPolygon.GetSubpropKey
																				  Select oBamashPolygon
		DMCommon.Debug.ExcelLog.SetNextValue(1, "!BamashPolygon.Count", colBamashPolygon.Count)
		For Each oBamashPolygon As BamashPolygon In colBamashPolygon
			DMCommon.Debug.ExcelLog.SetNextValue(2, "!CPolgon_ ", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.IsSubProp, oBamashPolygon.Coordinates, iCurrentSubpropNum)
			If oBamashPolygon.IsProperty AndAlso oBamashPolygon.IsSubProp AndAlso oBamashPolygon.SubPropNum = 0 Then
				iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
				oBamashPolygon.SetMainData(-1, enPropertyTypes.Ignore, iCurrentSubpropNum, oBamashPolygon.AprtDescNum, -1)
				oBamashPolygon.UpdateMainData()
				DMCommon.Debug.ExcelLog.SetNextValue(4, "!BPCont", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.AprtDescNum, iCurrentSubpropNum)

				iCurrentSubpropNum += 1
			End If
		Next

	End Sub
	Private Shared Sub zzSubpropNumeration1()
		Dim iCurrentSubpropNum As Integer = 1
		For Each oBamashPolygon As BamashPolygon In mdicBamashPgons.Values
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!BPolgon_ ", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.IsSubProp, oBamashPolygon.Coordinates)
			If oBamashPolygon.IsProperty AndAlso oBamashPolygon.IsSubProp AndAlso oBamashPolygon.SubPropNum = 0 Then
				iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
				oBamashPolygon.SetMainData(-1, enPropertyTypes.Ignore, iCurrentSubpropNum, oBamashPolygon.AprtDescNum, -1)
				oBamashPolygon.UpdateMainData()
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!BPCont", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, iCurrentSubpropNum)
				iCurrentSubpropNum += 1
			End If
		Next
	End Sub

	Private Shared Sub zzSubpropNumerationByProp()
		bmProperty.Initialize()

		Dim iTest As Integer = 0
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicProperties ", mdicProperties IsNot Nothing)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicProperCnt", mdicProperties.Count)

		For Each oProperty As bmProperty In mdicProperties.Values
			'''''''''oProperty.Calculate(iCalcOption)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!SubpropNum", oProperty.PropID, oProperty.HasID)
			If oProperty.HasID Then
				oProperty.SubpropNumerate()
			End If


			iTest += 1
		Next
	End Sub
	Private Shared Sub zzSubpropNumerationClear()
		Dim iCurrentSubpropNum As Integer = 1

		Dim colBamashPolygon As IEnumerable(Of BamashPolygon) = From oBamashPolygon As BamashPolygon In mdicBamashPgons.Values
																				  Order By oBamashPolygon.GetSubpropKey
																				  Select oBamashPolygon


		For Each oBamashPolygon As BamashPolygon In colBamashPolygon
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!DPolgon_ ", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.IsSubProp, oBamashPolygon.Coordinates, iCurrentSubpropNum)
			If oBamashPolygon.IsProperty AndAlso oBamashPolygon.IsSubProp AndAlso oBamashPolygon.SubPropNum = 0 Then
				iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
				oBamashPolygon.SetMainData(-1, enPropertyTypes.Ignore, iCurrentSubpropNum, oBamashPolygon.AprtDescNum, -1)
				oBamashPolygon.UpdateMainData()
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!BPCont", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.AprtDescNum, iCurrentSubpropNum)

				iCurrentSubpropNum += 1
			End If
		Next

	End Sub
	Private Shared Sub zzSubpropNumerationErase()
		'Dim iCurrentSubpropNum As Integer = 1
		For Each oBamashPolygon As BamashPolygon In mdicBamashPgons.Values
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!FPolgon_ ", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, oBamashPolygon.IsSubProp, oBamashPolygon.Coordinates)
			If oBamashPolygon.IsProperty AndAlso oBamashPolygon.IsSubProp AndAlso oBamashPolygon.SubPropNum <> 0 Then
				'iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
				oBamashPolygon.SetMainData(-1, enPropertyTypes.Ignore, 0, oBamashPolygon.AprtDescNum, -1)
				oBamashPolygon.UpdateMainData()
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!BPCont", oBamashPolygon.PropertyID, oBamashPolygon.SubPropNum, 0)

				'iCurrentSubpropNum += 1
			End If

		Next
	End Sub


	Public Shared Sub ClearPaint()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		If msPaintLayer Is Nothing OrElse msPaintLayer.Length = 0 Then
			Dim tLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.BamashPaint)
			'	MessageBox.Show(CStr(tLayerDef.Correct) & ":" & tLayerDef.Name, "26_781")
			If tLayerDef.Correct Then
				msPaintLayer = tLayerDef.Name
			End If
		End If
		'	DMCommon.Debug.MsgBox("0408_1", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.BamashPaint, msPaintLayer)
		DMAcadExt.AcadTransaction.ClearLayerByClassName(msPaintLayer, String.Empty)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Public Shared Function GetTopoName() As String
		Return msTopoName
	End Function
	Private Shared Function zzLoadPolygons(sTopoName As String) As Boolean
		mbOpened = True
		Dim iTest As Integer
		Dim tTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Bamash)
		'sTopoName = TopoManager.TopoDefs.Item(tTopoDefID).Name
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sTopoName)
		If oBamashTopology IsNot Nothing Then
			Dim oBamashPgon As BamashPolygon
			msTopoName = oBamashTopology.Name
			If oBamashTopology.Status = Autodesk.Gis.Map.Topology.Status.Closed Then
				Try
					oBamashTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & sTopoName & "'", "TplnProject - LoadPolygons")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadPolygons")
					Return False
				End Try
			End If
			'	Dim oTestBox As DMAcadExt.TPlnBoundingBox
			If oBamashTopology.Status = Autodesk.Gis.Map.Topology.Status.OpenForRead Then
				Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oBamashTopology.GetPolygons()
				mdicBamashPgons = New BamashPgons()
				mdicPgonGroups = New bmPgonGroups
				BamashPolygon.OpenMainDataTable()

				For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					oBamashPgon = New BamashPolygon(oPolygon)
					If oBamashPgon.Correct Then
						mdicBamashPgons.AddBamashPgon(oBamashPgon)
						mdicPgonGroups.AddPgon(oBamashPgon)

						'''''''''''''''
					Else
						System.Windows.Forms.MessageBox.Show("Polygon #" & Convert.ToString(oPolygon.ID) & " Error", "19_200")
					End If
					iTest += 1
					If iTest > 8 Then
						'Exit For
					End If
				Next

				DMAcadExt.AcadDocument.WriteMessage("Calculation is finished successfully")
				oBamashPgon = Nothing
				oBamashTopology.Close()
				oBamashTopology = Nothing
				Return True
			Else
				oBamashTopology = Nothing
				Return False
			End If
		Else
			System.Windows.Forms.MessageBox.Show("Bamash Topology Is Nothing")
			Return False
		End If
	End Function
	Private Shared Sub zzCalcGroups()
		Dim oGroup As bmPgonGroup = Nothing
		Dim iBasePgonID As Integer
		Dim oBasePgon As BamashPolygon = Nothing

		For Each oBmPgon As BamashPolygon In mdicBamashPgons.Values


			If oBmPgon.GroupID <> 0 Then
				If mdicPgonGroups.TryGetValue(oBmPgon.GroupID, oGroup) Then
					iBasePgonID = oGroup.BasePgonID

					If mdicBamashPgons.TryGetValue(iBasePgonID, oBasePgon) Then

						oGroup.SubPropNum = oBasePgon.SubPropNum
					End If
					oBmPgon.SetGroupData(oGroup)
				End If
			End If


			oBmPgon.AddDataToMainTable()

			'	DMAcadExt.AcadDocument.WriteMessage("^40 " & CStr(oBmPgon.TopoID) & ":" & CStr(oBmPgon.IsSubProp) & "," & CStr(oBmPgon.SubPropNum) & "," & CStr(oBmPgon.IsGroupNotBase))
			If oBmPgon.IsSubProp AndAlso oBmPgon.SubPropNum <> 0 AndAlso (Not oBmPgon.IsGroupNotBase) Then
				'	DMAcadExt.AcadDocument.WriteMessage("^41 " & CStr(oBmPgon.TopoID))
				If Not BamashPolygon.NumberingSeparately Then
					If mdicSubPropNum.ContainsKey(oBmPgon.SubPropNum) Then
						DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.GetHebNum(oBmPgon.SubPropNum, True) & " already exists")
					Else
						mdicSubPropNum.Add(oBmPgon.SubPropNum)
					End If
				End If


			End If



		Next

	End Sub
	Private Shared Sub zzAddAllPolygons()
		For Each oBamashPolygon As BamashPolygon In mdicBamashPgons.Values
			zzAddPolygon(oBamashPolygon)
		Next

	End Sub
	
	Public Shared Sub InitMemory()
		mbOpened = True
		moBamashProjectData = New BamashProjectData()
		moBamashProjectData.OpenData(False, True)
		gdicColors = New Dictionary(Of Integer, Integer)()
		mdicProperties = New Generic.SortedDictionary(Of Integer, bmProperty)()
		mdicSubPropNum = New SubPropNumDic()
		mpgaShared = New bmPolygonArray
		CaptionOption = CaptionOptionSetting
		zzCreateMainTable()
		zzCreateShareTable()
	End Sub
	Public Shared Sub InitParams()
		mdZebraWidthDrawing = New DMCommon.DMValue(ZebraWidthDrawingSetting)
		mdZebraWidthTable = New DMCommon.DMValue(ZebraWidthTableSetting)
		mdBufferOffset = New DMCommon.DMValue(BufferOffsetSetting)
		mdZoomRadius = New DMCommon.DMValue(ZoomRadiusSetting)
	End Sub
	Public Shared Sub InitResource()
		Try
			moResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(miResourceTheme)
			moPropTypeResource = moResource.GetChild(0)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzMyInitializeComponent")
		End Try
	End Sub
	Public Shared Function GetPropTypesName(iPropTypeID As enPropertyTypes) As String
		Const iPropTypesSectionID As Integer = 0
		If False Then
			If moPropTypeResource IsNot Nothing Then
				moPropTypeResource.GetStrItem(0)
			End If
		End If



		Return TPlServerDB.TextResource.GetText(iPropTypeID, miResourceTheme, iPropTypesSectionID, True)

	End Function
	Public Shared ReadOnly Property ResourceTheme As TPlServerDB.enResourceTheme
		Get
			Return miResourceTheme
		End Get
	End Property
	Private Shared Function zzGetCaptionAAA(ByVal lPropType As Long, ByVal iPropertyID As Integer) As String
      Select Case lPropType
         Case enPropertyTypes.ApartType
            Return CStr(iPropertyID)
         Case enPropertyTypes.SubPrivateType, enPropertyTypes.SubShareType, enPropertyTypes.UnderBldTypeColor ', enPropertyTypes.UnderBldTypeWhite
            Return DMCommon.Hebrew.GetHebNum(iPropertyID, True)
         Case Else
            Return "שגיאה"
      End Select
   End Function




	
	Private Shared Sub zzCalculateProperties(ByVal iCalcOption As bmBamash.SubNumerationOptions)
		'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		bmProperty.Initialize()
		'	DMAcadExt.AcadDocument.Unlock()
		Dim iTest As Integer = 0
		For Each oProperty As bmProperty In mdicProperties.Values
			oProperty.Calculate(iCalcOption)
			If oProperty.HasID Then
				oProperty.SubNumerate(iCalcOption)
			End If
			'''''''''''''''''	oProperty.AddToPgonTable()
			If oProperty.HasID Then
				oProperty.OutputData()
			End If

			iTest += 1
		Next
		bmProperty.AddCommonData()
		zzCreatePrintTable()
	End Sub

	Public Shared ReadOnly Property SharedRepView() As DataView
		Get

			Dim oDataView As DataView

			Try
				oDataView = New DataView(moSharedRepTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Property - SharedRepView_1")
				Return Nothing
			End Try
			'	oDataView.Table = moSharedRepTable
			Return oDataView
		End Get
	End Property
	Private Shared Sub zzCalcSharedSubproperties(ByVal iCalcOption As bmBamash.SubNumerationOptions)

		Dim oSharedPolygon As BamashPolygon
		Dim iPropID As Integer
		Dim oProperty As bmProperty
		'mpgaShared.SortPgons()
		mpgaShared.SortSubprops(iCalcOption)

		For iIndex As Integer = 0 To mpgaShared.Count - 1
			oSharedPolygon = mpgaShared(iIndex)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "^04_ ", oSharedPolygon.PropertyID, oSharedPolygon.SharedPropertyID)
			For iPropIndex As Integer = 0 To oSharedPolygon.PropertiesUB
				iPropID = oSharedPolygon.PropertyID(iPropIndex)
				If mdicProperties.ContainsKey(iPropID) Then
					oProperty = mdicProperties.Item(iPropID)

					oSharedPolygon.SetColor(oProperty.ColorIndex)
					'	oSharedPolygon.PolygonColor(iPropIndex) = oProperty.ColorIndex
					'	oSharedPolygon.AddPolygonColor(oProperty.ColorIndex)
				Else
					DMAcadExt.AcadDocument.WriteMessage("PropId Wrong TopoID=" & CStr(oSharedPolygon.TopoID))
				End If
			Next
			oSharedPolygon.CreateColorScheme()
			Select Case iCalcOption
				Case bmBamash.SubNumerationOptions.All

					oSharedPolygon.SubPropNum = bmBamash.GetNext

				Case bmBamash.SubNumerationOptions.EmptyFirst, SubNumerationOptions.EmptyMax
					If oSharedPolygon.SubPropNum = 0 Then
						oSharedPolygon.SubPropNum = bmBamash.GetNextFree()
					End If
			End Select
			'''''''''''	oSharedPolygon.AddDataToMainTable()

		Next

		'	zzOutputShared()
	End Sub
	Private Shared Sub zzCalcSharedSubpropertiesII()

		Dim oSharedPolygon As BamashPolygon

		For iIndex As Integer = 0 To mpgaShared.Count - 1
			oSharedPolygon = mpgaShared(iIndex)
			If oSharedPolygon.IsGroupBase OrElse oSharedPolygon.GroupID = 0 Then
				zzOutputSharedPgon(oSharedPolygon)
			End If

		Next

		'	zzOutputShared()
	End Sub
	Private Shared Function zzCLng(ByVal sValue As String, ByVal iPolygonID As Integer, ByVal sTag As String) As Long
		Try
			If sValue.Length = 0 Then
				Return 0&
			Else
				Return CLng(sValue)
			End If
		Catch oEx As Exception
			zzGetAttribErr(iPolygonID, sTag, sValue)
		End Try

	End Function

	Public Shared Function Text2Int(ByVal sValue As String, Optional ByVal sTag As String = "") As Integer
		If sValue.Length = 0 Then
			Return 0
		Else
			Try
				Return Convert.ToInt32(sValue)
			Catch oEx As Exception
				If sTag.Length <> 0 Then
					DMAcadExt.AcadDocument.WriteMessage("T2I:" & sTag & "='" & sValue & "';" & oEx.Message)
				End If
				Return 0
			End Try
		End If
	End Function
	Private Shared Function zzCInt(ByVal sValue As String, ByVal iPolygonID As Integer, ByVal sTag As String) As Integer
		Dim sMsg As String
		On Error Resume Next
		If Len(sValue) = 0& Then
			Return 0
		Else
			Return CInt(sValue)
			If Err.Number <> 0& Then zzGetAttribErr(iPolygonID, sTag, sValue)
		End If
	End Function
	Private Function zzCStr(ByVal sValue As String, ByVal iPolygonID As Integer, ByVal sTag As String) As String
		Dim sMsg As String
		On Error Resume Next
		If sValue.Length = 0& Then
			Return String.Empty
		Else
			zzCStr = sValue
			If Err.Number <> 0& Then zzGetAttribErr(iPolygonID, sTag, sValue)
		End If
	End Function
	Public Shared Property ZebraWidthDrawingSetting() As Double
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZebraWidthDrawingSettingKey, Convert.ToString(mdZebraWidthDrawingDflt))
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSettting)
			Catch oEx As Exception
				dSetting = mdZebraWidthDrawingDflt
			End Try
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sValue As String = CStr(dValue)
			Microsoft.VisualBasic.SaveSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZebraWidthDrawingSettingKey, sValue)
		End Set
	End Property
	Public Shared Property ZebraWidthTableSetting() As Double
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZebraWidthTableSettingKey, Convert.ToString(mdZebraWidthTableDflt))
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSettting)
			Catch oEx As Exception
				dSetting = mdZebraWidthTableDflt
			End Try
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sValue As String = CStr(dValue)
			Microsoft.VisualBasic.SaveSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZebraWidthTableSettingKey, sValue)
		End Set
	End Property
	Public Shared Property BufferOffsetSetting() As Double
		Get
			Dim sSetting As String = Microsoft.VisualBasic.GetSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msBufferOffsetSettingKey, Convert.ToString(mdBufferOffsetDflt))
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSetting)
			Catch oEx As Exception
				dSetting = mdBufferOffsetDflt
			End Try
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sValue As String = CStr(dValue)
			Microsoft.VisualBasic.SaveSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msBufferOffsetSettingKey, sValue)
		End Set
	End Property
	Public Shared Property ZoomRadiusSetting() As Double
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZoomRadiusSettingKey, Convert.ToString(mdZoomRadiusDflt))
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSettting)
			Catch oEx As Exception
				dSetting = mdBufferOffsetDflt
			End Try
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sValue As String = CStr(dValue)
			Microsoft.VisualBasic.SaveSetting(TopoManager.Common.AppName, TopoManager.Common.SettingSectionName, msZoomRadiusSettingKey, sValue)
		End Set
	End Property
	Public Shared Property ProjectData() As BamashProjectData
		Get
			Return moBamashProjectData
		End Get
		Set(ByVal oValue As BamashProjectData)
			moBamashProjectData = oValue
		End Set
	End Property
	Public Shared Property Parcel() As String
		Get
			If msParcel Is Nothing Then
				Return moBamashProjectData.Parcel
			Else
				Return msParcel
			End If

		End Get
		Set(sValue As String)
			msParcel = sValue
		End Set
	End Property
	Public Shared ReadOnly Property PaintLayerDef() As DMAcadExt.AcadLayerDef
		Get
			If Not moPaintLayerDef.Exists Then
				moPaintLayerDef = New DMAcadExt.AcadLayerDef(1, DMAcadExt.enLayerFunction.Default)
			End If
			Return moPaintLayerDef
		End Get
	End Property

	Private Shared Sub zzPaintPrivate()
		Dim bCurrentLayerOK As Boolean = True
		msPaintLayer = String.Empty
		Dim iTest As Integer = 0
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msPaintLayer, 2, DMAcadExt.enLayerFunction.PaintHatchDefault, True, True, True)
		If bCurrentLayerOK Then
			If mdicProperties IsNot Nothing Then
				zzOpenBamashTopo(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				For Each oProperty As bmProperty In mdicProperties.Values()
					''Debug TopoManager.AcadDocument.WriteMessage("Upd PropoID=" & CStr(oProperty.PropID))
					oProperty.Paint()
					iTest += 1

					''''''''''''''''''''old system	oProperty.UpdateBlocksOOO()
					'	oProperty.PrintTable()
					'	oProperty.OutputAfrica()
				Next
				zzCloseBamashTopo()
			End If
		End If
	End Sub

	Private Shared Sub zzOutputSharedPgon(ByVal oSharedPgon As BamashPolygon)
		Dim oDataRow As DataRow
		oDataRow = moSharedRepTable.NewRow
		With oDataRow
			.Item("ApartDescr") = oSharedPgon.ApartDesc
			.Item("MainCaption") = oSharedPgon.CaptionDOS
			.Item("Color") = oSharedPgon.ColorScheme
			If oSharedPgon.IsGroupBase Then
				.Item("Area") = oSharedPgon.GroupArea
			Else
				.Item("Area") = oSharedPgon.CalcArea
			End If

			.Item("Owners") = oSharedPgon.SharedPropertyID
		End With
		moSharedRepTable.Rows.Add(oDataRow)
	End Sub
	Private Shared Sub zzCreatePrintTable()

		moSharedRepTable = New DataTable("MainPrint")
		With moSharedRepTable.Columns
			.Add("ApartDescr", GetType(System.String))
			.Add("MainCaption", GetType(System.String))
			.Add("Color", GetType(DMAcadExt.ColorScheme))
			.Add("Area", GetType(System.String))
			.Add("Owners", GetType(System.String))
		End With

	End Sub

	Private Shared Sub zzPaintShare()
		Dim oPolygon As BamashPolygon
		If mpgaShared IsNot Nothing Then
			For iIndex As Integer = 0 To mpgaShared.Count - 1
				oPolygon = mpgaShared.Item(iIndex)
				oPolygon.Paint()
			Next
		End If
	End Sub

	Private Function GetHebNumber(ByVal lVal As Long) As String
		Return CStr(lVal)
	End Function
	Public Shared Function GetNext() As Integer
		Return mdicSubPropNum.GetNext()
	End Function
	Public Shared Function GetMinFree(ByVal iNumber As Integer) As Integer
		Return mdicSubPropNum.GetMinFree(iNumber)
	End Function
	Public Shared Function GetNextFree() As Integer
		Return mdicSubPropNum.GetNextFree()
	End Function





	Private Shared Sub zzUpdateBlock_Net()
		If mdicBamashPgons IsNot Nothing Then
			Dim oBamashPolygon As BamashPolygon
			For Each oKeyValuePair As KeyValuePair(Of Integer, BamashPolygon) In mdicBamashPgons
				oBamashPolygon = oKeyValuePair.Value
				If oBamashPolygon.IsProperty Then
					oBamashPolygon.UpdateDrawing()
				End If
			Next
		End If
	End Sub




End Class
