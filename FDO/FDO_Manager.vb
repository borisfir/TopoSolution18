Option Explicit On
Option Strict On

Imports Autodesk.Gis.Map.DataConnect.UI
Imports OSGeo.FDO.Commands.SpatialContext
Imports OSGeo.FDO
Imports OverlayMgd
Imports OSGeo.FDO.Geometry
Imports OSGeo.FDO.Commands.DataStore
Imports Autodesk.Gis.Map.Platform
'Imports OSGeo.MapGuide.Schema.LayerDefinition
Public Class FDO_Manager
	Inherits System.Windows.Forms.Form
	Public Const SHPProviderName As String = "OSGeo.SHP.4.2" '"OSGeo.SHP.3.6"
	'	Public Const SHPProviderName As String = "OSGeo.SHP.3.6"



	'	Dim msParcelShapePath As String = "D:\aWork\ArcGIS\D\Parcel\topoParcels.shp"
	'	Dim msLotShapePath As String = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
	Private WithEvents moDataConnectUI As dmDataConnectUI
	Private WithEvents moSHPConnectionControl As dmSHPConnectionControl
	Private WithEvents moGenericBrowseSchemaControl As dmGenericBrowseSchemaControl
	Private moSHPConnectionPlugin As SHPConnectionPlugin

   Private moDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
	Private msConnectionName As String
	Private moImporter As Autodesk.Gis.Map.ImportExport.Importer 'WithEvents

   Private mtaShapeFOData() As ShapeFOData
   Private mdicGushFromParcelData As Dictionary(Of Integer, GushData)

	Private miPoligonCount As Integer
	Private msRootPath As String, msaParcelFolders() As String
	Private mtaGushData() As GushData
	Private miaCdBlocks() As Integer
	Private moUnionBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox()
	Private moaBoundingBoxes() As DMAcadExt.TPlnBoundingBox
	Private mcolDisPlines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private mdWorkAreaTotal As Double
	Private mdicMPgonColByGush As IDictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
	Private msShapeFolderName As String
	Private mdXY_Tolerance As Double = 0.001
	Public Sub New()
		'''''''''''moDataConnectUI = New dmDataConnectUI()
		''''''''''	moDataConnectUI.AddProviderNode(msSHPProviderName)
		'''''''''''	moSHPConnectionControl = New dmSHPConnectionControl()
      miPoligonCount = 0
      '  System.Windows.Forms.MessageBox.Show("", "04_502BB")
	End Sub
	Public Sub New(sRootPath As String, saParcelFolders() As String)
		msRootPath = sRootPath
		msaParcelFolders = saParcelFolders
   End Sub
    
	Public Shared Sub ClearAllLayers()
		Dim colLayers As OSGeo.MapGuide.MgLayerCollection

		colLayers = AcMapMap.GetCurrentMap().GetLayers()
		If colLayers IsNot Nothing Then

			colLayers.Clear()
		Else
			System.Windows.Forms.MessageBox.Show("Nothing", "01_005")
		End If
	End Sub
	Public Shared Sub NewTestB()
		Dim colLayers As OSGeo.MapGuide.MgLayerCollection
		Dim oMgLayerBase As OSGeo.MapGuide.MgLayerBase
		Dim oAcMapLayer As Autodesk.Gis.Map.Platform.AcMapLayer
		Dim sMsg As String

		colLayers = AcMapMap.GetCurrentMap().GetLayers()
		If colLayers Is Nothing Then
			System.Windows.Forms.MessageBox.Show("colLayers Is Nothing ", "01_470")

		Else
			System.Windows.Forms.MessageBox.Show(CStr(colLayers.Count), "01_471")
			For iIndex As Integer = 0 To colLayers.Count - 1
				oMgLayerBase = colLayers.Item(iIndex)
				oAcMapLayer = DirectCast(oMgLayerBase, Autodesk.Gis.Map.Platform.AcMapLayer)

				sMsg = zzNN(oMgLayerBase.FeatureClassName) & vbCrLf
				sMsg &= zzNN(oMgLayerBase.FeatureSourceId) & vbCrLf
				sMsg &= zzNN(oMgLayerBase.GetClassName) & vbCrLf
				sMsg &= zzNN(oMgLayerBase.GetName) & vbCrLf
				sMsg &= zzNN(oMgLayerBase.GetType().ToString()) & vbCrLf

				System.Windows.Forms.MessageBox.Show(sMsg, "01_472")
			Next

		End If
	End Sub
	Private Shared Function zzNN(sVal As String) As String
		If sVal Is Nothing Then
			Return "<Nothing>"
		Else
			Return sVal
		End If
	End Function

	Public Sub CreateDissolveMapLayer(coPlines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal sFeatureClass As String, sFDOConnectionName As String)


		If FDO_Manager.LayerExists(sFeatureClass) Then
			System.Windows.Forms.MessageBox.Show("השכבה כבר קיימת") '
		Else
			'Dim oExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
			Dim sShapeFileName As String
			'	Dim oDirectoryInfo As IO.DirectoryInfo = New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFeatureClass)

			'If Not oDirectoryInfo.Exists Then
			'oDirectoryInfo.Create()
			'End If

			'	sShapeFileName = oTopoDef.GetLocalFileName(".shp")
			''''''''''''''''''''sShapeFileName = DMCommon.Functions.GetLocalFileNameInDir(sFeatureClass, ".shp")
			sShapeFileName = DMCommon.Functions.GetLocalFileNameInEmptyDir(sFeatureClass, ".shp")
			If Not String.IsNullOrEmpty(sShapeFileName) Then
				'	System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf, "02_524")
				Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
            '  DMCommon.Debug.MsgBox("01_439", sShapeFileName, CStr(coPlines.Count))
				oShapeExpImp.FromPoligons(coPlines)
            '	oShapeExpImp.AddShapeData()

				'	System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & "", "02_533")
				Try
					oShapeExpImp.Exec()
				Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
					System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_848")
				End Try
            '      System.Windows.Forms.MessageBox.Show(sFeatureClass & vbCrLf & sFDOConnectionName, "02_537")
				Try
					ConnectToShape(sFDOConnectionName, sShapeFileName)

					'	AddLayerToMap(sFDOConnectionName)
					zzCreateWorkAreaMapLayers("")
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_871")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
				End Try

			Else
				System.Windows.Forms.MessageBox.Show(sFeatureClass & vbCrLf & "NOTHING", "02_987a")
			End If
      End If

	End Sub
	Public Property FOData As ShapeFOData()
		Get
			Return mtaShapeFOData

		End Get
		Set(taValue As ShapeFOData())
			mtaShapeFOData = taValue
		End Set
	End Property


	Public Sub CreateMapLayer(ByVal sExpCondition As String, ByVal iGraphType As DMAcadExt.enGraphType, ByVal sFeatureClass As String, sFDOConnectionName As String, sDataSuffix As String, tColor As System.Drawing.Color) ', sFDOLayerName As String
		'System.Windows.Forms.MessageBox.Show("'" & sExpCondition & "'" & vbCrLf & iGraphType.ToString() & vbCrLf & "F='" & sFeatureClass & "'" & vbCrLf & sFDOConnectionName, "03_544")
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		'	System.Windows.Forms.MessageBox.Show("STOP", "08_200") '
		'	Stop
		If FDO_Manager.LayerExists(sFeatureClass) Then  'sFDOLayerName?????????????
			System.Windows.Forms.MessageBox.Show("השכבה כבר קיימת") '
		Else

			'	Dim tRes As Autodesk.Gis.Map.ImportExport.ExportResults

			Dim sShapeFileName As String = System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFeatureClass & "\" & sFeatureClass & ".shp"
			Dim oDirectoryInfo As IO.DirectoryInfo = New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFeatureClass)

			If Not oDirectoryInfo.Exists Then
				oDirectoryInfo.Create()
			End If

			'	sShapeFileName = oTopoDef.GetLocalFileName(".shp")
			sShapeFileName = DMCommon.Functions.GetLocalFileNameInDir(sFeatureClass, ".shp")
			Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
			If iGraphType = DMAcadExt.enGraphType.Topology OrElse iGraphType = DMAcadExt.enGraphType.TopoOverlay Then
				oShapeExpImp.FromTopology(sExpCondition)
				oShapeExpImp.AddTopoData("ID_" & sDataSuffix, "AREA_" & sDataSuffix)
			ElseIf iGraphType = DMAcadExt.enGraphType.ClosedPolygons Then
				oShapeExpImp.FromPoligons(sExpCondition)
				oShapeExpImp.AddShapeData(1)
			End If

			Try
				oShapeExpImp.Exec()
			Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
				System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName & vbCrLf & iGraphType.ToString() & vbCrLf & "'" & sExpCondition & "'", "01_841ax")

			End Try

			Try
				'	DMCommon.Debug.MsgBox("13_030A", sFeatureClass, sShapeFileName)
				ConnectToShape(sFDOConnectionName, sShapeFileName)
				DMCommon.Debug.ExcelLog.SetNextValue(0, "021220_3", sFDOConnectionName, sShapeFileName, sFeatureClass)
				zzCreateMapLayersNew(sShapeFileName, sFeatureClass, tColor)
				'SSSSSSSSSSSSSSSSSSSSSSS	AddLayerToMap(sFDOConnectionName)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
			End Try
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadTransaction.Terminate()
	End Sub
	Public Sub ConnectToShape_081220(sConnectionName As String, sShapeFileName As String)
		'	Dim oNode As System.Windows.Forms.TreeNode
		'	Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim iConnectionState As OSGeo.FDO.Connections.ConnectionState
		'''''''''''		Dim moDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		''''''''''		Dim moSHPConnectionPlugin As SHPConnectionPlugin

		'	System.Windows.Forms.MessageBox.Show(sConnectionName & vbCrLf & sShapeFileName, "ConnectToShape 01_899_5")
		'	oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		'	moDataConnectUI.OnProviderNodeSelected(oNode)
		'	If moSHPConnectionPlugin Is Nothing Then
		moDataConnectUI = New dmDataConnectUI()
		moDataConnectUI.ForceRereshOnFeatureSourceNodeSelected = True
		moDataConnectUI.AddProviderNode(SHPProviderName)     'new
		'''''''''''''''''''''''''''	moSHPConnectionControl = New dmSHPConnectionControl()	' new
		'	DMCommon.Debug.MsgBox("13_030B", SHPProviderName, sConnectionName, sShapeFileName)

		moDataConnectUI.SelectProvider(SHPProviderName)

		'	EEEEEEEEEE

		moSHPConnectionPlugin = moDataConnectUI.GetSHPPluginConnection()

		'	moSHPConnectionPlugin = DirectCast(moDataConnectPlugin, SHPConnectionPlugin)
		moSHPConnectionPlugin.SourceFileName = sShapeFileName
		'''''''''''		moSHPConnectionPlugin.ConnectionName = sConnectionName

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName & vbCrLf & sConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString(), "05_800")
		'	End If

		iConnectionState = moSHPConnectionPlugin.ConnectWithName(sConnectionName)
		'DMCommon.Debug.MsgBox("13_030D", sConnectionName, CInt(iConnectionState), iConnectionState.ToString())
		moDataConnectUI.SelectFeatureSource(sConnectionName)
		'System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_801")
		'
		If False Then
			moDataConnectUI.RemoveProviderNode(SHPProviderName)

			moDataConnectUI.AddProviderNode(SHPProviderName)

		End If

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_807")
	End Sub
	Public Sub ConnectToShape(sConnectionName As String, sShapeFileName As String)
		zzConnectToShape(sConnectionName, sShapeFileName)
	End Sub
	Public Sub ConnectToShape(sResultLayer As String, tColor As Drawing.Color)
		Dim sShapeConnectionName As String = Nothing
		Dim sFCName As String = Nothing
		Dim sShapeFileName As String = Nothing

		zzGetSHPFileName(sResultLayer, sShapeFileName, sFCName, sShapeConnectionName, False)
		zzConnectToShape(sShapeConnectionName, sShapeFileName)
		zzCreateMapLayersNew(sShapeFileName, sFCName, tColor)
	End Sub

	Private Sub zzConnectToShape(sConnectionName As String, sShapeFileName As String)
		'	Dim oNode As System.Windows.Forms.TreeNode
		'	Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim iConnectionState As OSGeo.FDO.Connections.ConnectionState
		'''''''''''		Dim moDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		''''''''''		Dim moSHPConnectionPlugin As SHPConnectionPlugin

		'	System.Windows.Forms.MessageBox.Show(sConnectionName & vbCrLf & sShapeFileName, "ConnectToShape 01_899_5")
		'	oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		'	moDataConnectUI.OnProviderNodeSelected(oNode)
		'	If moSHPConnectionPlugin Is Nothing Then
		DMCommon.Debug.MsgBox("081220", sConnectionName, sShapeFileName)
		moDataConnectUI = New dmDataConnectUI()
		moDataConnectUI.ForceRereshOnFeatureSourceNodeSelected = True
		moDataConnectUI.AddProviderNode(SHPProviderName)     'new
		'''''''''''''''''''''''''''	moSHPConnectionControl = New dmSHPConnectionControl()	' new
		'	DMCommon.Debug.MsgBox("13_030B", SHPProviderName, sConnectionName, sShapeFileName)

		moDataConnectUI.SelectProvider(SHPProviderName)

		'	EEEEEEEEEE

		moSHPConnectionPlugin = moDataConnectUI.GetSHPPluginConnection()

		'	moSHPConnectionPlugin = DirectCast(moDataConnectPlugin, SHPConnectionPlugin)
		moSHPConnectionPlugin.SourceFileName = sShapeFileName
		'''''''''''		moSHPConnectionPlugin.ConnectionName = sConnectionName

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName & vbCrLf & sConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString(), "05_800")
		'	End If

		iConnectionState = moSHPConnectionPlugin.ConnectWithName(sConnectionName)
		'DMCommon.Debug.MsgBox("13_030D", sConnectionName, CInt(iConnectionState), iConnectionState.ToString())
		moDataConnectUI.SelectFeatureSource(sConnectionName)
		'System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_801")
		'
		If False Then
			moDataConnectUI.RemoveProviderNode(SHPProviderName)

			moDataConnectUI.AddProviderNode(SHPProviderName)

		End If

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_807")
	End Sub
	Public Sub AddLayerToMapSrc(sConnectionName As String)
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		'	Dim moGenericBrowseSchemaControl As dmGenericBrowseSchemaControl
		moDataConnectUI.SelectFeatureSource(sConnectionName)

		oDataConnectPlugin = moDataConnectUI.GetPluginBrowseSchema()

		moGenericBrowseSchemaControl = New dmGenericBrowseSchemaControl()

		moGenericBrowseSchemaControl.SetPluginB(oDataConnectPlugin)

		'	System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & CStr(moGenericBrowseSchemaControl.Created), "09_108")
		moGenericBrowseSchemaControl.AttachB(moDataConnectUI)
		moGenericBrowseSchemaControl.AddToMap()

	End Sub
	Public Sub AddLayerToMap(sConnectionName As String)
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim bRes As Boolean
		Dim iState As OSGeo.FDO.Connections.ConnectionState
		'	Dim moGenericBrowseSchemaControl As dmGenericBrowseSchemaControl
		DMCommon.Debug.MsgBox("13_031A", sConnectionName)
		moDataConnectUI.SelectFeatureSource(sConnectionName)
		moDataConnectUI.SelectLayerSource("ParcelsLine")
		'	DMCommon.Debug.MsgBox("13_031B")
		oDataConnectPlugin = moDataConnectUI.GetPluginBrowseSchema()
		'	DMCommon.Debug.MsgBox("13_031C", oDataConnectPlugin.ConnectionName)
		moGenericBrowseSchemaControl = New dmGenericBrowseSchemaControl()
		'	DMCommon.Debug.MsgBox("13_031D")
		iState = moGenericBrowseSchemaControl.SetPluginB(oDataConnectPlugin)
		'	DMCommon.Debug.MsgBox("13_031E", iState)
		'	System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & CStr(moGenericBrowseSchemaControl.Created), "09_108")
		moGenericBrowseSchemaControl.AttachB(moDataConnectUI)

		'	DMCommon.Debug.MsgBox("13_031F")
		If True Then
			bRes = moGenericBrowseSchemaControl.AddToMap()
			DMCommon.Debug.MsgBox("13_031G", sConnectionName, iState, bRes)
		End If

	End Sub

	Public Sub Disconnect(sFeatureSource As String)



		'''''''''''		System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString, "05_802")

		'	moSHPConnectionPlugin.Disconnect()
		'	moDataConnectPlugin.Disconnect()
		'	moSHPConnectionPlugin.Detach()
		'	moDataConnectUI.SelectProvider(msSHPProviderName)
		'	moDataConnectUI.SelectFeatureSource(msConnectionName)
		moDataConnectUI.SelectFeatureSource(sFeatureSource)
		moDataConnectUI.DisconnectedFromFeatureSource(sFeatureSource)

		'''''''''	System.Windows.Forms.MessageBox.Show(msConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString, "05_810")
	End Sub
	Public Sub RemoveConnectionA(sConnectionName As String, sLayerName As String)

		'''''''''''		Dim moDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		''''''''''		Dim moSHPConnectionPlugin As SHPConnectionPlugin


		moDataConnectUI = New dmDataConnectUI()


		'		moDataConnectUI.SelectProvider(msSHPProviderName)
		moDataConnectUI.SelectFeatureSource(sConnectionName)
		moDataConnectUI.ConnectedToFeatureSource(sConnectionName)

		moDataConnectPlugin = moDataConnectUI.GetSHPPluginConnection()

		'	System.Windows.Forms.MessageBox.Show(moDataConnectPlugin.ToString(), "04_101")


		moDataConnectPlugin.Attach(moDataConnectUI, SHPProviderName, sConnectionName, sLayerName)
		'	System.Windows.Forms.MessageBox.Show(moDataConnectPlugin.FeatureSourceName & ":" & moDataConnectPlugin.LayerSourceName, "05_881")

		'	moDataConnectPlugin.Disconnect()
		'	moDataConnectPlugin.Detach()

		'''''''''''''''''''Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		'	Dim moGenericBrowseSchemaControl As dmGenericBrowseSchemaControl
		'''''''''''''''''''''	oDataConnectPlugin = moDataConnectUI.GetPluginBrowseSchema()




		moGenericBrowseSchemaControl = New dmGenericBrowseSchemaControl()
		moGenericBrowseSchemaControl.SetPlugin(moDataConnectPlugin)


		Dim oSchemaClassTreeView As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeView = moGenericBrowseSchemaControl.GetSchemaClassTreeView()

		System.Windows.Forms.MessageBox.Show(CStr(oSchemaClassTreeView.Items.Count), "05_893 before")
		Dim oItem As System.Windows.Forms.ListViewItem
		For i As Integer = 0 To oSchemaClassTreeView.Items.Count - 1
			oItem = oSchemaClassTreeView.Items(i)
			System.Windows.Forms.MessageBox.Show(CStr(oItem.Text), "05_895")
		Next
		Dim lstSchemaClassTreeViewItem As System.Collections.Generic.List(Of Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem) = oSchemaClassTreeView.GetFeatureClass()
		Dim oSchemaClassItem As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem
		For i As Integer = 0 To lstSchemaClassTreeViewItem.Count - 1
			oSchemaClassItem = lstSchemaClassTreeViewItem.Item(i)
			'''''''''''''''''''''''''''	oSchemaClassItem.Remove()
		Next

		'	oSchemaClassTreeView.DeselectAll()
		'	System.Windows.Forms.MessageBox.Show(CStr(oSchemaClassTreeView.Items.Count), "05_893 after")
		moDataConnectPlugin.Disconnect()
		moDataConnectPlugin.Detach()
		'		moDataConnectUI.FeatureSourceRemoved(sConnectionName)
		Return

		'	moDataConnectUI.PostRemoveFeatureSourceNode(sConnectionName)
		moDataConnectUI.LayerSourceRemoved("p2792")
		moDataConnectUI.PostRemoveLayerSourceNode("p2792")


		moDataConnectUI.RemoveFeatureSourceNode(sConnectionName)

		moDataConnectUI.PostRemoveFeatureSourceNode(sConnectionName)


		moDataConnectPlugin.Detach()
		moDataConnectUI.Dispose()


		'''''''''''		moSHPConnectionPlugin.ConnectionName = sConnectionName

		'	System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName & vbCrLf & sConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString(), "05_800")
		'	End If

		moDataConnectUI.RemoveProviderNode(SHPProviderName)
		moDataConnectUI.AddProviderNode(SHPProviderName)

	End Sub
	Public Shared Sub RemoveConnectionB(sConnectionName As String)
		Dim oDataConnectUI As dmDataConnectUI = New dmDataConnectUI()
		oDataConnectUI.ConnectedToFeatureSource(sConnectionName)
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin = oDataConnectUI.GetSHPPluginConnection()

		oDataConnectPlugin.Attach(oDataConnectUI, SHPProviderName, sConnectionName, "LayerName")
      oDataConnectPlugin.Disconnect()
      
	End Sub
	Public Shared Sub RemoveAllConnections()
		Dim oDataConnectUI = New dmDataConnectUI()
		Dim oSHPProviderNode As System.Windows.Forms.TreeNode = oDataConnectUI.AddProviderNode(SHPProviderName)	  'new
		Dim oSHPConnNodes As System.Windows.Forms.TreeNodeCollection = oSHPProviderNode.Nodes

      For Each oNode As System.Windows.Forms.TreeNode In oSHPConnNodes
			' 
			'DMCommon.Debug.MsgBox("01_832U", oNode.Name)
			FDO_Manager.RemoveConnectionB(oNode.Name)

      Next

	End Sub
	Public Shared Sub DeleteRecource(sResource As String)
		Dim rs As OSGeo.MapGuide.MgResourceService
		Dim oResourceID As OSGeo.MapGuide.MgResourceIdentifier

		rs = CType(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)
		sResource = "Library://" & sResource & ".FeatureSource"
		oResourceID = New OSGeo.MapGuide.MgResourceIdentifier(sResource)
		rs.DeleteResource(oResourceID)
	End Sub
	Public Sub RemoveConnection(sConnectionName As String)
		Dim s As String
		System.Windows.Forms.MessageBox.Show(sConnectionName, "RemoveConnection 01_860")
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin = New SHPConnectionPlugin()

		System.Windows.Forms.MessageBox.Show(sConnectionName, "RemoveConnection 01_861")
		moDataConnectUI = New dmDataConnectUI()


		'	moDataConnectUI.SelectProvider(msSHPProviderName)
		moDataConnectUI.SelectFeatureSource(sConnectionName)

		''''''''''	moDataConnectUI.ConnectedToFeatureSource(sConnectionName)


		Try
			moDataConnectPlugin = moDataConnectUI.GetPluginBrowseSchema()
			s = moDataConnectPlugin.FeatureSourceName
			'	moDataConnectPlugin.Disconnect()

			'	moDataConnectPlugin.Detach()

			'		System.Windows.Forms.MessageBox.Show(oDataConnectPlugin.ToString() & vbCrLf & s, "04_003")





			moGenericBrowseSchemaControl = New dmGenericBrowseSchemaControl()
			moGenericBrowseSchemaControl.SetPluginA(oDataConnectPlugin)
			moGenericBrowseSchemaControl.AttachA(moDataConnectUI)
			moDataConnectUI.SelectFeatureSource(sConnectionName)
			moDataConnectUI.DisconnectedFromFeatureSource(sConnectionName)


			'	moGenericBrowseSchemaControl.DisconnectA()
			'	Disconnect(sConnectionName)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_118")
		End Try

		'	moSHPConnectionPlugin = DirectCast(moDataConnectPlugin, SHPConnectionPlugin)
		'	moSHPConnectionPlugin.SourceFileName = sShapeFileName
		'	moSHPConnectionPlugin.ConnectionName = sConnectionName

		'	System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName & vbCrLf & sConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString(), "05_800")


		'	moSHPConnectionPlugin.Disconnect()

		'	System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_801")
		' 

		'		moDataConnectUI.RemoveProviderNode(msSHPProviderName)
		'		moDataConnectUI.AddProviderNode(msSHPProviderName)
	End Sub
	Private Shared Function zzUnion(sSourceLayer As String, sOverlayLayer As String, sOutputFile As String, sOutputFCName As String, iOutputProperties As Integer, dXY_Tolerance As Double, dSliverFloorArea As Double, dSliverCeilingArea As Double) As Boolean
		Const sOverlayType As String = "Union"
		Try
			'   System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & dXY_Tolerance & vbCrLf & dSliverFloorArea & vbCrLf & dSliverCeilingArea, "FeatureEditor 1248xW")
			FeatureEditor.Overlay(sSourceLayer, sOverlayLayer, sOverlayType, sOutputFile, sOutputFCName, iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)
			'''''''''''''FeatureEditor.Overlay(sOverlayLayer, sSourceLayer, sOverlayType, sOutputFile, sOutputFCName, iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)
			'	FeatureEditor.Overlay(sSourceLayer, sOverlayLayer, sOverlayType, "D:\aWork\ArcGIS\E_Acad\topoLots_UnionMx.shp", "topoLots_Union_Mx", iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)

			Return True
		Catch oEx As Exception
			'	System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & iOutputProperties.ToString() & vbCrLf & dXY_Tolerance.ToString() & vbCrLf & dSliverFloorArea.ToString() & vbCrLf & dSliverCeilingArea.ToString(), "Failed  1299x")
			'System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOverlayType & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & CStr(iOutputProperties) & vbCrLf & CStr(dXY_Tolerance) & vbCrLf & CStr(dSliverFloorArea) & vbCrLf & CStr(dSliverCeilingArea), "FeatureEditor 1123ca")

			DMCommon.Debug.UserMsg("FeatureEditor 1123ca", oEx.Message, sSourceLayer, sOverlayLayer, sOverlayType, sOutputFile, sOutputFCName, iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)
			Return False
		End Try
	End Function
	Private Shared Function zzPaste(sSourceLayer As String, sOverlayLayer As String, sOutputFile As String, sOutputFCName As String, iOutputProperties As Integer, dXY_Tolerance As Double, dSliverFloorArea As Double, dSliverCeilingArea As Double) As Boolean
      Const sOverlayType As String = "Paste"
      Try
         '   System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & dXY_Tolerance & vbCrLf & dSliverFloorArea & vbCrLf & dSliverCeilingArea, "FeatureEditor 1248xW")
         FeatureEditor.Overlay(sSourceLayer, sOverlayLayer, sOverlayType, sOutputFile, sOutputFCName, iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)
			'''''''''''''FeatureEditor.Overlay(sOverlayLayer, sSourceLayer, sOverlayType, sOutputFile, sOutputFCName, iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)
			'	FeatureEditor.Overlay(sSourceLayer, sOverlayLayer, sOverlayType, "D:\aWork\ArcGIS\E_Acad\topoLots_UnionMx.shp", "topoLots_Union_Mx", iOutputProperties, dXY_Tolerance, dSliverFloorArea, dSliverCeilingArea)

			Return True
      Catch oEx As Exception
         Return False
         System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOutputFile, "Failed  1298x")
         DMAcadExt.AcadDocument.WriteMessage("!!!NB_4:" & sOutputFile)
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sOverlayType & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & CStr(iOutputProperties) & vbCrLf & CStr(dXY_Tolerance) & vbCrLf & CStr(dSliverFloorArea) & vbCrLf & CStr(dSliverCeilingArea), "FeatureEditor 1124c")

      End Try
   End Function
   Public Function Union(ByVal sSourceLayer As String, ByVal sOverlayLayer As String, ByVal sResultLayer As String, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim sSHPFileName As String = Nothing
      Dim oFileInfo As IO.FileInfo
		'	Dim iIndex As Integer = 0
		Dim sFCName As String = Nothing
		Dim sShapeConnection As String = Nothing

		zzGetSHPFileName(sResultLayer, sSHPFileName, sFCName, sShapeConnection, True)
		DMCommon.Debug.MsgBox("zzGetSHPFileName", sSourceLayer, sOverlayLayer, sResultLayer, sSHPFileName, sFCName, sShapeConnection)

		If dMaxFDOSliverTolerance < 0.0 Then
         dMinFDOSliverTolerance = 0.0
      End If
		If zzUnion(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 2, mdXY_Tolerance, dMinFDOSliverTolerance, dMaxFDOSliverTolerance) Then ' 0.01, 0.1, 0.1
			oFileInfo = New IO.FileInfo(sSHPFileName)
			If oFileInfo.Exists Then
				DMAcadExt.AcadTransaction.ClearLayerByClassName(sResultLayer, String.Empty)
				zzImportA(sSHPFileName, sResultLayer)
			End If
			Return sSHPFileName
		Else
			Return Nothing
      End If

   End Function
	Public Function Paste(ByVal sSourceLayer As String, ByVal sOverlayLayer As String, ByVal sResultLayer As String, ByVal sOutputDir As String, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
		'  Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sSHPFileName As String = Nothing
		Dim oFileInfo As IO.FileInfo
		'	Dim iIndex As Integer = 0
		Dim sFCName As String = Nothing

		sSHPFileName = sOutputDir & "\" & sResultLayer & ".SHP"
		sFCName = sResultLayer
		If dMaxFDOSliverTolerance < 0.0 Then
			dMinFDOSliverTolerance = 0.0
		End If
		'  System.Windows.Forms.MessageBox.Show("'" & sSourceLayer & "'" & vbCrLf & sOverlayLayer & vbCrLf & sSHPFileName & vbCrLf & sFCName, "zzPARSEadd 199UH")

		If zzPaste(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 1, 0.1, dMinFDOSliverTolerance, dMaxFDOSliverTolerance) Then ' 0.01, 0.1, 0.1

			oFileInfo = New IO.FileInfo(sSHPFileName)
			If Not oFileInfo.Exists Then
				' DMAcadExt.AcadTransaction.ClearLayerByClassName(sResultLayer, String.Empty)
				'zzImportA(sSHPFileName, sResultLayer)
				System.Windows.Forms.MessageBox.Show("Output File" & vbCrLf & sSHPFileName & vbCrLf & "not exists", "Error")
				Return Nothing

			End If
			Return sSHPFileName
		Else
			Return Nothing
		End If

	End Function
	Public Function UnionOnly(ByVal sSourceLayer As String, ByVal sOverlayLayer As String, ByVal sResultLayer As String, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sSHPFileName As String = Nothing
		Dim oFileInfo As IO.FileInfo
		'	Dim iIndex As Integer = 0
		Dim sFCName As String = Nothing
		'Dim tColor As Autodesk.AutoCAD.Colors.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 6S)
		Dim tColor As Drawing.Color = Drawing.Color.FromArgb(0, 255, 127)
		Dim sShapeConnection As String = Nothing ' "shp_" & sFCName
		zzGetSHPFileName(sResultLayer, sSHPFileName, sFCName, sShapeConnection, True)

		If dMaxFDOSliverTolerance < 0.0 Then
			dMinFDOSliverTolerance = 0.0
		End If
		If FDO_Manager.zzUnion(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 2, mdXY_Tolerance, dMinFDOSliverTolerance, dMaxFDOSliverTolerance) Then ' 0.01, 0.1, 0.1
			oFileInfo = New IO.FileInfo(sSHPFileName)
			If oFileInfo.Exists Then
				'DMAcadExt.AcadTransaction.ClearLayerByClassName(sResultLayer, String.Empty)
				'zzImportA(sSHPFileName, sResultLayer)



				If True Then
					Try
						'	Dim o As AcMapDataConnectUI = New AcMapDataConnectUI()
						'	System.Windows.Forms.MessageBox.Show(sFDOConnectionName & vbCrLf & sShapeFileName, "08_232ss")
						'	Return
						'	DMCommon.Debug.MsgBox("021220_1", sShapeConnection, sSHPFileName, sFCName)
						ConnectToShape(sShapeConnection, sSHPFileName)
						'Ac12 AddLayerToMap(sShapeConnection)   'Ac12
						'DMCommon.Debug.MsgBox("021220_2", sShapeConnection, sSHPFileName, sFCName)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "021220_2", sShapeConnection, sSHPFileName, sFCName)
						'	zzCreateMapLayersNew(sSHPFileName, sFeatureClass, tColor)
						If True Then
							zzCreateMapLayersNew(sSHPFileName, sFCName, tColor)
						End If

					Catch oMapEx As Autodesk.Gis.Map.MapException
						System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
					End Try

				End If
			Else
				DMCommon.Debug.UserMsg("Err #2361", " File " & sSHPFileName & " was not found")

			End If
			Return sSHPFileName
		Else
			DMCommon.Debug.MsgBox("Err #2361", sShapeConnection, sSHPFileName, sFCName)
			Return Nothing
		End If

	End Function
	Public Function UnionOnly_021220(ByVal sSourceLayer As String, ByVal sOverlayLayer As String, ByVal sResultLayer As String, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sSHPFileName As String = Nothing
		Dim oFileInfo As IO.FileInfo
		'	Dim iIndex As Integer = 0
		Dim sFCName As String = Nothing
		Dim sShapeConnection As String = Nothing
		zzGetSHPFileName(sResultLayer, sSHPFileName, sFCName, sShapeConnection, True)

		If dMaxFDOSliverTolerance < 0.0 Then
			dMinFDOSliverTolerance = 0.0
		End If
		If FDO_Manager.zzUnion(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 2, mdXY_Tolerance, dMinFDOSliverTolerance, dMaxFDOSliverTolerance) Then ' 0.01, 0.1, 0.1
			oFileInfo = New IO.FileInfo(sSHPFileName)
			If oFileInfo.Exists Then
				'DMAcadExt.AcadTransaction.ClearLayerByClassName(sResultLayer, String.Empty)
				'zzImportA(sSHPFileName, sResultLayer)

				If True Then
					Try
						'	Dim o As AcMapDataConnectUI = New AcMapDataConnectUI()
						'	System.Windows.Forms.MessageBox.Show(sFDOConnectionName & vbCrLf & sShapeFileName, "08_232ss")
						'	Return

						ConnectToShape(sShapeConnection, sSHPFileName)
						AddLayerToMap(sShapeConnection)

					Catch oMapEx As Autodesk.Gis.Map.MapException
						System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
					End Try

				End If


			End If
			Return sSHPFileName
		Else
			Return Nothing
		End If

	End Function
	Public Function Union(ByVal sSourceLayer As String, ByVal sOverlayLayer As String, ByVal sOverlayLayer_A As String, ByVal sResultLayer As String, ByVal sResultLayer_A As String, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sSHPFileName As String = Nothing
		Dim sFCName As String = Nothing
		Dim sSHPFileName_A As String = Nothing
		Dim sFCName_A As String = Nothing
		Dim oFileInfo As IO.FileInfo
		If dMaxFDOSliverTolerance < 0.0 Then
			dMinFDOSliverTolerance = 0.0
		End If
		Dim sShapeConnection As String = Nothing
		zzGetSHPFileName(sResultLayer, sSHPFileName, sFCName, sShapeConnection, True)
		System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sSHPFileName & vbCrLf & sFCName, "04_340")

		If FDO_Manager.zzUnion(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 4, mdXY_Tolerance, dMinFDOSliverTolerance, dMaxFDOSliverTolerance) Then ' 0.01, 0.1, 0.1
			oFileInfo = New IO.FileInfo(sSHPFileName)
			If oFileInfo.Exists Then
				If True Then
					Try
						'	Dim o As AcMapDataConnectUI = New AcMapDataConnectUI()
						'	System.Windows.Forms.MessageBox.Show(sFDOConnectionName & vbCrLf & sShapeFileName, "08_232ss")
						'	Return

						ConnectToShape(sFCName, sSHPFileName)
						AddLayerToMap(sFCName)

					Catch oMapEx As Autodesk.Gis.Map.MapException
						System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
					End Try

				End If

			End If
			Return sSHPFileName
		Else
			Return Nothing
		End If

	End Function
	Public Function Union(ByVal oSrcTopDef As DMAcadExt.TopoDef, ByVal oOvlTopDef As DMAcadExt.TopoDef, ByVal oResTopoDef As DMAcadExt.TopoDef, dMinFDOSliverTolerance As Double, dMaxFDOSliverTolerance As Double) As String
		Dim sSourceLayer As String, sOverlayLayer As String
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim iResTopDefID As DMAcadExt.TopoDefID = oResTopoDef.ID
		Dim stSrcTopDefID As DMAcadExt.TopoDefID = iResTopDefID.SourceID
		Dim stOvlTopDefID As DMAcadExt.TopoDefID = iResTopDefID.OverlayID

		'	Dim oSrcTopDef As DMAcadExt.TopoDef = New DMAcadExt.TopoDef(DMAcadExt.DMApp.AppID, stSrcTopDefID)
		'	Dim oOvlTopDef As DMAcadExt.TopoDef = New DMAcadExt.TopoDef(DMAcadExt.DMApp.AppID, stOvlTopDefID)
		'	System.Windows.Forms.MessageBox.Show(oSrcTopDef.Name & vbCrLf & oOvlTopDef.Name, "06_252")


		'	Dim sSDFFileName As String = System.Windows.Forms.Application.LocalUserAppDataPath & "\" & "Parcels_LotsK" & "\" & "Parcels_LotsK" & ".sdf"
		Dim sSHPFileName As String	'= "D:\aWork\ArcGIS\E_Acad\ShpD\Parcels_LotsK.shp"
		Dim oFileInfo As IO.FileInfo
		Dim oFileStream As IO.FileStream
		Dim iIndex As Integer = 0
		Do
			sSHPFileName = oResTopoDef.GetLocalFileName(".shp", iIndex)
			'	System.Windows.Forms.MessageBox.Show(sSHPFileName, "06_200")
			oFileInfo = New IO.FileInfo(sSHPFileName)

			Try
				oFileStream = IO.File.OpenWrite(oFileInfo.FullName)
				oFileStream.Close()
				Exit Do
			Catch oEx As IO.IOException
				System.Windows.Forms.MessageBox.Show(oEx.Message, "05_890")
				iIndex += 1
			End Try
		Loop


		Dim saParseVal() As String = Split(oFileInfo.Name, ".")
		Dim sFCName As String = saParseVal(0)

		sSourceLayer = oSrcTopDef.FDOLayerName
		sOverlayLayer = oOvlTopDef.FDOLayerName
		'		System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer, "01_844")
		'		System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sSHPFileName & vbCrLf & sFCName & vbCrLf & oResTopoDef.Name, "01_845")
		'Dim dMaxFDOSliverTolerance As Double = MaxFDOSliverTolerance
		'	Dim dMinFDOSliverTolerance As Double = MinFDOSliverTolerance

		If dMaxFDOSliverTolerance < 0.0 Then
			dMinFDOSliverTolerance = 0.0
		End If
		FDO_Manager.zzUnion(sSourceLayer, sOverlayLayer, sSHPFileName, sFCName, 2, mdXY_Tolerance, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)   ' 0.01, 0.1, 0.1



		Dim oFDO_Manager As FDO_Manager = New FDO_Manager()

		If oFileInfo.Exists Then

			DMAcadExt.AcadTransaction.ClearLayerByClassName(oResTopoDef.Name, String.Empty)
			zzImportA(sSHPFileName, oResTopoDef.Name)
			'oFDO_Manager.GetData(sSHPFileName, "LotKParcel")
		End If
		Return sSHPFileName

	End Function
	Private Sub zzGetSHPFileName(ByVal sResultLayer As String, ByRef sSHPFileName As String, ByRef sFCName As String, ByRef sShapeConnection As String, bCheckAccess As Boolean)
		Dim oFileInfo As IO.FileInfo
		Dim oFileStream As IO.FileStream
		Dim iIndex As Integer = 0
		Do
			sSHPFileName = DMCommon.Functions.GetLocalFileNameInDir(sResultLayer, ".shp", iIndex)
			'DMAcadExt.AcadDocument.WriteMessage("!!!NB_3:" & System.Windows.Forms.Application.LocalUserAppDataPath)
			oFileInfo = New IO.FileInfo(sSHPFileName)
			Try
				If bCheckAccess Then
					oFileStream = IO.File.OpenWrite(oFileInfo.FullName)
					oFileStream.Close()
				End If

				Exit Do
			Catch oEx As IO.IOException
				'System.Windows.Forms.MessageBox.Show(oEx.Message, "05_845")
				iIndex += 1
			End Try
		Loop
		Dim saParseVal() As String = Strings.Split(oFileInfo.Name, ".")
		sFCName = saParseVal(0)
		sShapeConnection = "shp_" & sFCName
	End Sub

	'	MinFDOSliverTolerance

	Private Sub zzzz(ByVal oResTopoDef As DMAcadExt.TopoDef)
		Dim sSHPFileName As String	'= "D:\aWork\ArcGIS\E_Acad\ShpD\Parcels_LotsK.shp"
		Dim oFileInfo As IO.FileInfo

		sSHPFileName = oResTopoDef.GetLocalFileName(".shp", 0)
		'	System.Windows.Forms.MessageBox.Show(sSHPFileName, "06_200")
		oFileInfo = New IO.FileInfo(sSHPFileName)
	End Sub
   Private Function zzGetParcelDataAAA(sSHPFileName As String, sClassName As String) As Dictionary(Of Integer, ParcelData)

      Dim sSHPPropFileName As String = "DefaultFileLocation"
      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim connState As Connections.ConnectionState
      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         'oEditor.WriteMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next

      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Try
         connState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259zz")
         Return Nothing
      End Try

      If connState <> OSGeo.FDO.Connections.ConnectionState.ConnectionState_Open Then
         DMAcadExt.AcadDocument.WriteMessage("64!" & connState.ToString() & vbCrLf)
      End If


      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)


      Dim oClasses As Schema.ClassCollection = oSchema.Classes

      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
      Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)
      Dim oClassName As Expression.Identifier = New Expression.Identifier(sClassName)
      oSelect.FeatureClassName = oClassName

      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()

      Dim iGeo As Integer = 0

      Dim sText As String
      Dim iPgonID As Integer
      Dim iBlock As Integer
      Dim iBlockAdd As Integer
      Dim iParcelName As Integer

      Dim sParcelName As String
      Dim dLegalArea As Double
      Dim iStatus As Integer

      Dim iPgonIDMin, iPgonIDMax As Integer
      iPgonID = 0
      While oFeatureReader.ReadNext()
         oFeatureClass = oFeatureReader.GetClassDefinition
         iTestCounter += 1
         Try

            iPgonID += 1
            If iPgonIDMin = 0 Then
               iPgonIDMin = iPgonID
               iPgonIDMax = iPgonID
            Else
               iPgonIDMin = Math.Min(iPgonIDMin, iPgonID)
               iPgonIDMax = Math.Max(iPgonIDMax, iPgonID)

            End If
            sText = CStr(iPgonID) & ":"
            iBlock = oFeatureReader.GetInt32(2)
            iBlockAdd = oFeatureReader.GetInt32(3)
            iParcelName = oFeatureReader.GetInt32(4)
            dLegalArea = oFeatureReader.GetDouble(5)
            iStatus = oFeatureReader.GetInt32(6)
            sParcelName = Convert.ToString(iParcelName)


            '		oEditor.WriteMessage("31-- " & ":" & oFeatureClass.Name & " : " & CStr(iValue) & vbCrLf)
            dicRes.Add(iPgonID, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("23err 1!! " & oEx.Message & ":" & vbCrLf)

         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)

         '	Exit While
      End While
      '	System.Windows.Forms.MessageBox.Show(CStr(iOverlayMin) & ":" & CStr(iOverlayMax), "04_001 Overlay Min:Max")
      Return dicRes


   End Function
   Public Sub SchemeModify(sSHPFolder As String, sSHPFileName As String, sClassName As String)
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim sSHPPropFileNameA As String = "TemporaryFileLocation"

      Dim oEditorA As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor



      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim connState As Connections.ConnectionState
      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString


      Dim isRequired As Boolean
      '    Dim iTestCounter As Integer
      '  Dim dicRes As Dictionary(Of Integer, OverlayIDs) = New Dictionary(Of Integer, OverlayIDs)()
      DMAcadExt.AcadDocument.WriteDebugMessage("!saNames.GetUpperBound(0)=" & CStr(saNames.GetUpperBound(0)))

      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         DMAcadExt.AcadDocument.WriteDebugMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next
      System.Windows.Forms.MessageBox.Show("", "04_301")
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
      System.Windows.Forms.MessageBox.Show("", "04_302")
      Try
         connState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259m")

      End Try
      System.Windows.Forms.MessageBox.Show("", "04_303")
      If True Then


         Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
         Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
         Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
         Dim oClasses As Schema.ClassCollection = oSchema.Classes
         Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)

         DMAcadExt.AcadDocument.WriteDebugMessage("oFeatureClass.Name=" & oFeatureClass.Name & "; Attr=" & oFeatureClass.Attributes.Count.ToString() & "; Prop=" & oFeatureClass.Properties.Count.ToString())

         Dim colProperties As Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropertyType As Schema.PropertyType '= property.PropertyType;
         Dim sPropertyName As String
         Dim oPropertyDefinitionDel_A As Schema.PropertyDefinition = Nothing
         For Each oPropertyDefinition As Schema.PropertyDefinition In colProperties
            sPropertyName = oPropertyDefinition.Name
            oPropertyType = oPropertyDefinition.PropertyType
            DMAcadExt.AcadDocument.WriteDebugMessage("Props=" & sPropertyName & "; Type=" & oPropertyType.ToString())
            If oPropertyDefinition.Name = "Date_2" Then
               oPropertyDefinitionDel_A = oPropertyDefinition
            End If

         Next
         If oPropertyDefinitionDel_A IsNot Nothing Then
            colProperties.Remove(oPropertyDefinitionDel_A)
         End If
         oFeatureClass.Delete()
         DMAcadExt.AcadDocument.WriteDebugMessage("PropsAfter " & "; N=" & colProperties.Count.ToString())
         oDescSchema.Execute()
      End If
      If False Then


         Dim destroySchema As Commands.Schema.IDestroySchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroySchema), Commands.Schema.IDestroySchema)
         If destroySchema.SchemaName Is Nothing Then
            System.Windows.Forms.MessageBox.Show(" destroySchema.SchemaName Is Nothing", "04_281")
         Else
            System.Windows.Forms.MessageBox.Show(destroySchema.SchemaName, "04_282")
         End If
         System.Windows.Forms.MessageBox.Show("", "04_311")
         destroySchema.SchemaName = sClassName '"P_19930624_Paste" 'oFeatureClass.Name
         'System.Windows.Forms.MessageBox.Show(destroySchema.ParameterValues.Count.ToString(), "04_319")
         'For Each oParameterValue As OSGeo.FDO.Commands.ParameterValue In destroySchema.ParameterValues
         '   System.Windows.Forms.MessageBox.Show(oParameterValue.Name, "04_320")
         'Next

         System.Windows.Forms.MessageBox.Show("", "04_312")
         destroySchema.Execute()
      End If
      System.Windows.Forms.MessageBox.Show("", "04_313")


      'Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_RollbackLongTransactionCheckpoint), Commands.Feature.ISelect)
      'Dim oClassName As Expression.Identifier = New Expression.Identifier(sClassName)
      'oSelect.FeatureClassName = oClassName

   End Sub
   Public Shared Sub ReadSchema(sSHPFileName As String)
      '  Const sFeatureClassName As String = "AFeatureClass"
      ' Dim sClassName As Expression.Identifier = New Expression.Identifier(sFeatureClassName)
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim iConnState As Connections.ConnectionState
      Dim sSpatialContext As String
      '   DMCommon.ExcelLogG.Open()
      '   Dim connState As Connections.ConnectionState
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      DMCommon.Debug.MsgBox("09_371x", sSHPPropFileName, sSHPFileName)
      Try
         iConnState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259r")
      End Try
      DMCommon.Debug.MsgBox("09_371z", iConnState.ToString())

      'describe a schema
      Dim descSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim schemas As Schema.FeatureSchemaCollection = descSchema.Execute()
      Dim schema As Schema.FeatureSchema = schemas.Item(0)
      Dim classes As OSGeo.FDO.Schema.ClassCollection = schema.Classes
      Dim oFeatureClass As OSGeo.FDO.Schema.ClassDefinition = classes.Item(0)

      Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
      '  Dim oProperty As OSGeo.FDO.Schema.PropertyDefinition
      Dim iPropertyType As OSGeo.FDO.Schema.PropertyType ' = oProperty.PropertyType
      Dim dataPropDef As Schema.DataPropertyDefinition
      Dim oGeoPropDef As Schema.GeometricPropertyDefinition
      Dim sPropName As String
      Dim oDataType As OSGeo.FDO.Schema.DataType
      Dim sDataType As String

      Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition


      For iIndex As Integer = 0 To colProperties.Count - 1
         oPropDef = colProperties.Item(iIndex)
         sPropName = oPropDef.Name
         iPropertyType = oPropDef.PropertyType
         If (iPropertyType = OSGeo.FDO.Schema.PropertyType.PropertyType_DataProperty) Then
            dataPropDef = DirectCast(oPropDef, Schema.DataPropertyDefinition)
            oDataType = dataPropDef.DataType
            sDataType = oDataType.ToString
         ElseIf (iPropertyType = OSGeo.FDO.Schema.PropertyType.PropertyType_GeometricProperty) Then
            oGeoPropDef = DirectCast(oPropDef, Schema.GeometricPropertyDefinition)
            sSpatialContext = oGeoPropDef.SpatialContextAssociation
            sDataType = oGeoPropDef.SpatialContextAssociation
         Else
            sDataType = iPropertyType.ToString()
         End If
         '  DMCommon.ExcelLogG.SetNextValue(0, iIndex, sPropName, iPropertyType.ToString(), sDataType)
      Next


   End Sub
   Public Shared Sub CreateSchema(sSHPFileName As String)
      ' create and apply a schema
      Const sFeatureClassName As String = "FeatureClass"
      Dim sClassName As Expression.Identifier = New Expression.Identifier(sFeatureClassName)
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim iConnState As Connections.ConnectionState

      '   Dim connState As Connections.ConnectionState
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      DMCommon.Debug.MsgBox("09_371e", sSHPPropFileName, sSHPFileName)
      Try
         iConnState = oConnection.Open()
      Catch oEx As Exception

         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259c")
      End Try
      DMCommon.Debug.MsgBox("09_371z", iConnState.ToString())

      Dim schema As Schema.FeatureSchema = New Schema.FeatureSchema("AnApplicationSchema", "This schema contains one feature class.")

      Dim featClass As Schema.FeatureClass = New Schema.FeatureClass(sFeatureClassName, "This feature class contains one identity property one data property and a feature geometry.")

      Dim colProperties As Schema.PropertyDefinitionCollection = featClass.Properties
      Dim idProperties As Schema.DataPropertyDefinitionCollection = featClass.IdentityProperties
      Dim idProp As Schema.DataPropertyDefinition = New Schema.DataPropertyDefinition("ID", "This is the identity property")
      idProp.DataType = OSGeo.FDO.Schema.DataType.DataType_Int32
      idProp.IsAutoGenerated = True
      idProp.Nullable = False
      idProp.ReadOnly = True
      colProperties.Add(idProp)
      idProperties.Add(idProp)
      Dim int32Prop As Schema.DataPropertyDefinition = New Schema.DataPropertyDefinition("Int32Prop", "This is an Int32 property")
      int32Prop.DataType = OSGeo.FDO.Schema.DataType.DataType_Int32
      colProperties.Add(int32Prop)

      ' add the feature geometry
      Dim featGeomProp As Schema.GeometricPropertyDefinition = New Schema.GeometricPropertyDefinition("FeatGeomProp", "This is the feature geometry.")

      'associate this geometric property with a spatial context
      'you must have already added the named context to the data store
      ' if this property is not set, it is assigned the default spatial       context()
      ' see the Spatial Context topic to read a description of the default spatial context
      '''''''''''''''''''''''''''      featGeomProp.SpatialContextAssociation = "XY-M Spatial Context" '"Israel_TM_Grid" '
      ' by default this geometric property can contain geometries that may be classified as point, curve, surface or solid
      ' also by default this geometric property cannot contain geometries that have a Z ordinate or a measure attribute
      colProperties.Add(featGeomProp)
      ' without the following line you would be adding a non-feature Geometry()
      featClass.GeometryProperty = featGeomProp
      Dim classes As Schema.ClassCollection = schema.Classes
      classes.Add(featClass)

      Dim applySchema As Commands.Schema.IApplySchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ApplySchema), Commands.Schema.IApplySchema)

      '  Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)



      applySchema.FeatureSchema = schema
      Try
         applySchema.Execute()
      Catch oEx As OSGeo.FDO.Common.Exception ' Exception
         DMCommon.Debug.MsgBox("09_448a", oEx.GetType().ToString, oEx.Message, oEx.GetNativeErrorCode())
      End Try



      Dim oInsert As Commands.Feature.IInsert = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
      ' oInsert.
      ' oInsert.FeatureClassName = featClass. 'sClassName
      oInsert.SetFeatureClassName(sFeatureClassName)                                                                                                '  sFeatureClassName") '= featClass. 'sClassName

      Dim values As OSGeo.FDO.Commands.PropertyValueCollection = oInsert.PropertyValues
      ' add the Int32 value to the insert command
      Dim iInt32Value As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(5)
      Dim int32PropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Int32Prop", iInt32Value)
      values.Add(int32PropVal)
      ' add the feature geometry to the insert command
      Dim geomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New OSGeo.FDO.Geometry.FgfGeometryFactory()
      Dim position1 As OSGeo.FDO.Geometry.DirectPositionImpl = New OSGeo.FDO.Geometry.DirectPositionImpl(1.0, 1.0)
      Dim point As OSGeo.FDO.Geometry.IPoint = geomFactory.CreatePoint(position1)
      Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(geomFactory.GetFgf(point))
      Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("FeatGeomProp", geomVal)
      '''''''''''''''''''''  values.Add(geomPropVal)
      ' insert the feature
      Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = oInsert.Execute()
      reader.Close()

   End Sub
   Public Shared Sub InsertData(sSHPFileName As String)
      Dim className As Expression.Identifier = New Expression.Identifier("AFeatureClass")
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oConnManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(oConnManager.CreateConnection(FDO_Manager.SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim iConnState As Connections.ConnectionState
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      DMCommon.Debug.MsgBox("09_341x", sSHPPropFileName, sSHPFileName)
      Try
         iConnState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259i")
      End Try
      DMCommon.Debug.MsgBox("09_341z", iConnState.ToString())

      If False Then


         Dim createDS As OSGeo.FDO.Commands.DataStore.ICreateDataStore = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore), OSGeo.FDO.Commands.DataStore.ICreateDataStore)
         DMCommon.Debug.MsgBox("09_342", "01")
         Dim properties As OSGeo.FDO.Commands.DataStore.IDataStorePropertyDictionary = createDS.DataStoreProperties
         DMCommon.Debug.MsgBox("09_343", "02a")
         Try
            createDS.Execute()
         Catch ex As Exception

         End Try

      End If

      Dim insert As Commands.Feature.IInsert = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
      insert.FeatureClassName = className
      Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
      ' add the Int32 value to the insert command
      Dim iInt32Value As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(5)
      Dim int32PropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Int32Prop", iInt32Value)
      values.Add(int32PropVal)
      ' add the feature geometry to the insert command
      Dim geomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New OSGeo.FDO.Geometry.FgfGeometryFactory()
      Dim position1 As OSGeo.FDO.Geometry.DirectPositionImpl = New OSGeo.FDO.Geometry.DirectPositionImpl(1.0, 1.0)
      Dim point As OSGeo.FDO.Geometry.IPoint = geomFactory.CreatePoint(position1)
      Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(geomFactory.GetFgf(point))
      Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("FeatGeomProp", geomVal)
      values.Add(geomPropVal)
      ' insert the feature
      Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
      reader.Close()
   End Sub
   Public Function GetOverlayData(sSHPFileName As String, sClassName As String, bTopoParcel As Boolean, bIDAddExists As Boolean) As Dictionary(Of Integer, OverlayIDs)

      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oEditorA As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim connState As Connections.ConnectionState
      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      Dim dicRes As Dictionary(Of Integer, OverlayIDs) = New Dictionary(Of Integer, OverlayIDs)()

      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         DMAcadExt.AcadDocument.WriteDebugMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next

      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Try
         connState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259o")
         Return Nothing
      End Try

      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes
      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
      Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)
      Dim oClassName As Expression.Identifier = New Expression.Identifier(sClassName)
      oSelect.FeatureClassName = oClassName

		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()
      Dim iGeo As Integer = 0
      Dim sText As String
      Dim iOverlay, iParcelID, iLotID, iAddID As Integer
      Dim sParcelHandle As String
      oFeatureClass = oFeatureReader.GetClassDefinition()
      While oFeatureReader.ReadNext()
         iTestCounter += 1
         Try
            Try
               iOverlay = oFeatureReader.GetInt32(0)
            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("41err!! " & oEx.Message & ":" & vbCrLf)
               iOverlay = 0
            End Try
            sText = CStr(iOverlay) & ":"

            If oFeatureReader.IsNull(1) Then
               iParcelID = 0
               sParcelHandle = "NULL"
            ElseIf bTopoParcel Then
               sParcelHandle = "N/E"
               Try
                  iParcelID = oFeatureReader.GetInt32(1)
               Catch oEx As Exception
                  DMAcadExt.AcadDocument.WriteMessage("42err!! " & oEx.Message & ":" & vbCrLf)
               End Try
            Else
               'iParcelID = zzGetFeatureID(oFeatureReader.GetInt64(1))
               Try
                  sParcelHandle = oFeatureReader.GetString(1)
                  iParcelID = zzGetFeatureID(sParcelHandle)
               Catch oEx As Exception
                  sParcelHandle = "Err"
                  DMAcadExt.AcadDocument.WriteMessage("43err!! " & oEx.Message & ":" & vbCrLf)
               End Try
            End If
            sText &= CStr(iParcelID) & ":"
            If oFeatureReader.IsNull(3) Then
               sText &= "N"
               iLotID = 0
            Else
               Try
                  iLotID = oFeatureReader.GetInt32(3)
               Catch oEx As Exception
                  DMAcadExt.AcadDocument.WriteMessage("44err!! " & oEx.Message & ":" & vbCrLf)
               End Try

               sText &= CStr(iLotID)
            End If
            If bIDAddExists Then
               If oFeatureReader.IsNull(5) Then
                  sText &= "N"
                  iAddID = 0
               Else
                  Try
                     iAddID = oFeatureReader.GetInt32(5)
                  Catch oEx As Exception
                     iAddID = 999999
                     DMAcadExt.AcadDocument.WriteMessage("45err!! " & oEx.Message & ":" & vbCrLf)
                  End Try

                  sText &= CStr(iAddID)
               End If
            End If

            ''oEditor.WriteMessage("31-- " & ":" & sParcelHandle & "|" & CStr(iParcelID) & " : " & CStr(iLotID) & vbCrLf)
            dicRes.Add(iOverlay, New OverlayIDs(iOverlay, iParcelID, iLotID, iAddID))
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("49err!! " & oEx.Message & ":" & vbCrLf)
         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)
      End While
      '	System.Windows.Forms.MessageBox.Show(CStr(iOverlayMin) & ":" & CStr(iOverlayMax), "04_001 Overlay Min:Max")
      Return dicRes
   End Function
  

   Private Function zzGetFeatureID(lHandle As Long) As Integer

      Dim tHandle As Autodesk.AutoCAD.DatabaseServices.Handle = New Autodesk.AutoCAD.DatabaseServices.Handle(lHandle)
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = DMAcadExt.AcadTransaction.GetXData(DMAcadExt.TplnXDataParcel.XDataAppName, tHandle)
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel = New DMAcadExt.TplnXDataParcel(oResBuffer)

      Dim oDinValue As System.Object = Nothing
      Dim sTest As String = ""
      If oXDataParcel IsNot Nothing Then
         'Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
         Try
            '	oDinValue = taTypedValues(3).Value
            Dim iTopoID As Integer = CType(oDinValue, Integer)
            sTest = "!**" & oXDataParcel.ID.ToString() & "|" & oXDataParcel.DataID.ToString() & "|" & oXDataParcel.Block.ToString() & "|" & oXDataParcel.Name
            'DMAcadExt.AcadDocument.WriteMessage(sTest)
            Return oXDataParcel.ID
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage(oDinValue.GetType().ToString() & ":" & oEx.Message)
            Return -2
         End Try
      Else
         Return -3
      End If
   End Function

   Private Function zzGetFeatureID(sHandle As String) As Integer
      Dim lHandle As Long
      Try
         lHandle = CLng("&H" & sHandle)
      Catch oEx As Exception
         Return 0
      End Try
      Return zzGetFeatureID(lHandle)

   End Function
   Public Sub GetData(sSHPFileName As String, sClassName As String)

      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties

      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         'oEditor.WriteMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next

      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Dim connState As Connections.ConnectionState = oConnection.Open()
      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes
      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
      Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)
      sClassName = "LotKParcel"
      Dim className As Expression.Identifier = New Expression.Identifier(sClassName)

      oSelect.FeatureClassName = className

      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()

      Dim iGeo As Integer = 0
      Dim iValue As Integer
      Dim sText As String
      Dim sPropName As String
      While oFeatureReader.ReadNext()
         oFeatureClass = oFeatureReader.GetClassDefinition
         iTestCounter += 1
         Try
            sPropName = oFeatureReader.GetPropertyName(2)
            iValue = oFeatureReader.GetInt32(0)
            sText = CStr(iValue) & ":"
            iValue = oFeatureReader.GetInt32(1)
            sText &= CStr(iValue) & ":"
            If oFeatureReader.IsNull(3) Then
               sText &= "N"
            Else
               iValue = oFeatureReader.GetInt32(3)
               sText &= CStr(iValue)
            End If

            '		oEditor.WriteMessage("31-- " & ":" & oFeatureClass.Name & " : " & CStr(iValue) & vbCrLf)
            If iTestCounter < 24 Then
               oEditor.WriteMessage("32-- " & sPropName & "**" & ":" & sText & vbCrLf)

            End If

         Catch oEx As Exception
            oEditor.WriteMessage("36err 1!! " & oEx.Message & ":" & vbCrLf)

         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)

         '	Exit While
      End While
      Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
      Dim oLinkTable As DMAcadExt.ODTable
      Dim dValue As Double
      Dim sHandle As String
      Dim sTableName As String = "LotKParcel"
      Dim sPolylineLayer As String = "LotKParcel"
      Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)

      '	Dim iParcelTopoID As Integer
      oLinkTable = New DMAcadExt.ODTable(sTableName)
      If oLinkTable.Exists Then
         iTestCounter = 0

         lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolylines

            iTestCounter += 1
            sHandle = oPolygon.Handle.ToString()
            Try
               oODRec = oLinkTable.GetODRecord(oPolygon.ObjectId)

            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadFDO_Overlay_9")
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sTableName, "04_200as")
            End Try
            If oODRec IsNot Nothing Then
               '	oODRec.Init()
               Dim iFeatureID As Integer
               Dim iParcelID As Integer
               Dim iLotID As Integer

               Try
                  dValue = oODRec.Item(0).DoubleValue
                  iFeatureID = Convert.ToInt32(dValue)
                  dValue = oODRec.Item(1).DoubleValue
                  iParcelID = Convert.ToInt32(dValue)
                  dValue = oODRec.Item(3).DoubleValue
                  iLotID = Convert.ToInt32(dValue)
                  If iTestCounter < 3 Then
                     oEditor.WriteMessage("27-- " & ":" & CStr(iFeatureID) & ":" & CStr(iParcelID) & ":" & CStr(iLotID) & vbCrLf)
                  End If

               Catch oEx As Exception

                  System.Windows.Forms.MessageBox.Show(oEx.Message, "04_208")
               End Try

            End If

         Next
      Else
         oEditor.WriteMessage("25-- Not exists" & ":" & CStr(sTableName) & vbCrLf)
      End If
   End Sub
   Public Function GetSourceData(sSHPFileName As String, sClassName As String) As System.Data.DataTable

      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties

      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         'oEditor.WriteMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next
      '  System.Windows.Forms.MessageBox.Show(sSHPFileName & vbCrLf & sClassName, "04_101")
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Dim connState As Connections.ConnectionState = oConnection.Open()
      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes
      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)

		'Dim oDataPropAAA As OSGeo.FDO.Schema.DataPropertyDefinition
		'For Each oProp As OSGeo.FDO.Schema.PropertyDefinition In oFeatureClass.Properties
		'   If oProp.PropertyType = Schema.PropertyType.PropertyType_DataProperty Then
		'      oDataProp = DirectCast(oProp, OSGeo.FDO.Schema.DataPropertyDefinition)
		'      DMCommon.ExcelLog.SetNextValue(iRow, 15, oDataProp.DataType.ToString())
		'   End If



		'Next


		Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)

      Dim className As Expression.Identifier = New Expression.Identifier(sClassName)

      oSelect.FeatureClassName = className

      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()
      Dim oNewRow As System.Data.DataRow
      Dim iGeo As Integer = 0
      '  Dim dValue As Double
      '  Dim iValue As Integer

      Dim oDataTable As System.Data.DataTable = zzCreateSourceTable()

      'DataType_Int32()
      'DataType_String()
      'DataType_Decimal()
      'DataType_String()
      'DataType_String()



      While oFeatureReader.ReadNext()
         oNewRow = oDataTable.NewRow()
         oFeatureClass = oFeatureReader.GetClassDefinition
         iTestCounter += 1
         Try

            If Not oFeatureReader.IsNull(0) Then
               oNewRow.Item(0) = oFeatureReader.GetInt32(0)
            End If

            If Not oFeatureReader.IsNull(1) Then
               oNewRow.Item(1) = oFeatureReader.GetString(1)
            End If

            If Not oFeatureReader.IsNull(2) Then
               oNewRow.Item(2) = Convert.ToInt32(oFeatureReader.GetDouble(2))
            End If
            If Not oFeatureReader.IsNull(3) Then
               oNewRow.Item(3) = oFeatureReader.GetString(3)
            End If
            'Beit Shemesh
            '        If Not oFeatureReader.IsNull(4) Then
            'oNewRow.Item(4) = oFeatureReader.GetString(4)
            '      End If


            oDataTable.Rows.Add(oNewRow)
         Catch oEx As Exception
            oEditor.WriteMessage("57err 1!! " & oEx.Message & ":" & vbCrLf)

         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)

         '	Exit While
      End While
      Return oDataTable

   End Function

   Public Function GetIdentifyData(sSHPFileName As String, sClassName As String) As System.Data.DataTable

      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties

      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         'oEditor.WriteMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next

      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Dim connState As Connections.ConnectionState = oConnection.Open()
      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes
      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
      Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)

      Dim className As Expression.Identifier = New Expression.Identifier(sClassName)

      oSelect.FeatureClassName = className

      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()
      Dim oNewRow As System.Data.DataRow
      Dim iGeo As Integer = 0

      Dim oDataTable As System.Data.DataTable = zzCreateIdentifyTable()
      While oFeatureReader.ReadNext()
         oNewRow = oDataTable.NewRow()
         oFeatureClass = oFeatureReader.GetClassDefinition
         iTestCounter += 1
         Try

            If Not oFeatureReader.IsNull(0) Then
               oNewRow.Item(0) = oFeatureReader.GetInt32(0)
            End If

            If Not oFeatureReader.IsNull(1) Then
               oNewRow.Item(1) = oFeatureReader.GetInt32(1)
            End If

            If Not oFeatureReader.IsNull(2) Then
               oNewRow.Item(2) = oFeatureReader.GetInt32(2)
            End If





            oDataTable.Rows.Add(oNewRow)
         Catch oEx As Exception
            oEditor.WriteMessage("57err 1!! " & oEx.Message & ":" & vbCrLf)

         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)

         '	Exit While
      End While
      Return oDataTable

   End Function
   Private Function zzCreateIdentifyTable() As DataTable
      Dim oDataTable As System.Data.DataTable = New DataTable("Identifiers")

      '		Else
      '	oOrder = GetType(System.STRING)
      '	End If
      With oDataTable.Columns

         .Add("FeatID", GetType(System.Int32))
         .Add("SourceFeatID", GetType(System.Int32))
         .Add("OverlayFeatID", GetType(System.Int32))



      End With
      Return oDataTable
   End Function
   Private Function zzCreateSourceTable() As DataTable
      Dim oDataTable As System.Data.DataTable = New DataTable("Identifiers")

      '		Else
      '	oOrder = GetType(System.STRING)
      '	End If
      With oDataTable.Columns

         .Add("FeatID", GetType(System.Int32))
         .Add("Taba_Num", GetType(System.String))
         .Add("Taba_Numer", GetType(System.Int32))
         .Add("taba_Name", GetType(System.String))
         .Add("Date", GetType(System.String))


      End With
      Return oDataTable
   End Function
   Private Sub zzExportBoundingPolygon(oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline, sShapeFileName As String)
      Dim tRes As Autodesk.Gis.Map.ImportExport.ExportResults
      Dim sMsg As String = "Topologia " & "" & ": "
      Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
      Dim coPlines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      If Not oPolyline.Closed Then
         oPolyline.Closed = True
      End If
      coPlines.Add(oPolyline.ObjectId)
      oShapeExpImp.FromPoligons(coPlines)
      Try
         oShapeExpImp.Exec()
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_918")
      End Try


      Dim iEntitiesExported As UInteger = tRes.EntitiesExported
      If iEntitiesExported = Convert.ToUInt32(1) Then
         sMsg = "one polygon"
      Else
         sMsg = CStr(iEntitiesExported) & " polygons"
      End If
      sMsg &= " exported"
      '	System.Windows.Forms.MessageBox.Show(sMsg, "01_800exp")
      DMAcadExt.AcadDocument.WriteMessage(sMsg)

   End Sub

	Private Sub zzExportBoundingPolygonByTopo(sTopoName As String, sShapeFileName As String)
		Dim oExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
		Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
		Dim tRes As Autodesk.Gis.Map.ImportExport.ExportResults
		Try
			oExporter.Init("SHP", sShapeFileName)
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_877")
		End Try




		Try
			oExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, String.Empty)
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_851")
		End Try
		'	System.Windows.Forms.MessageBox.Show(sTopoName & vbCrLf & sShapeFileName, "01_892")
		Try
			oExporter.ExportAll = False
			oExporter.LayerFilter = ""
			oExporter.SetExportFromPolygonTopology(False, sTopoName)
			oExporter.SetSelectionSet(New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection())
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sTopoName & vbCrLf & sShapeFileName, "01_812")
		End Try

		Try
			colExpressionTarget = oExporter.GetExportDataMappings()
			'		System.Windows.Forms.MessageBox.Show(CStr(colExpressionTarget.Count), "01_813")
			colExpressionTarget.Clear()
			oExporter.SetExportDataMappings(colExpressionTarget)
			'		colExpressionTarget.Add(":ID@TPMCNTR_" & sTopoName, "ID")
			'		colExpressionTarget.Add(":AREA@TPMCNTR_" & sTopoName, "AREA")
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_814")
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			Return
		End Try




		Try
			tRes = oExporter.Export()
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & oMapImpExpEx.InnerException.Message, "01_816")
		End Try
		Dim iEntitiesExported As UInteger = tRes.EntitiesExported
		Dim sMsg As String = "Topologia " & sTopoName & ": "
		If iEntitiesExported = Convert.ToUInt32(1) Then
			sMsg = "one polygon"
		Else
			sMsg = CStr(iEntitiesExported) & " polygons"
		End If
		sMsg &= " exported"
		'	System.Windows.Forms.MessageBox.Show(sMsg, "01_800exp")
		DMAcadExt.AcadDocument.WriteMessage(sMsg)

	End Sub
	Private Shared Function zzGetActualLayerDef(sSHPFileFullName As String, sFileName As String, tColor As System.Drawing.Color) As String
		Const sPath As String = "M:\Dm_Work\Template's\LayerDef\p2793.layer"

		'R:\Gushim\Gushim-MMG-ISRAEL\2018\2018.06\GIS\parcel\
		'	Const sOutputPath As String = "D:\Parcel1.layer"
		'	Dim iGushNo As Integer = 2793
		Dim oMainXmlNode As System.Xml.XmlNode
		Dim sMain As String
		Dim oXmlNode() As System.Xml.XmlNode
		Dim oXmlNode1 As System.Xml.XmlNode

		Dim oXmlNode2 As System.Xml.XmlNode
		Dim oXmlNode3 As System.Xml.XmlNode
		Dim oXmlNode4 As System.Xml.XmlNode
		Dim oXmlNode5 As System.Xml.XmlNode
		Dim oXmlNode6 As System.Xml.XmlNode



		'	Dim s0 As String
		Dim s1 As String '= "fsd://shp_p2792"
		Dim s1a As String '= "Library://shp_p2792.FeatureSource"

		Dim s2 As String '= "shp_p2792"
		Dim s3 As String '= "p2792"
		Dim s4 As String ' = "Default:p2792"
		's0 = sBlockNo
		s3 = sFileName '"p" & s0
		s2 = "shp_" & s3 ' SHP Connection
		's2 = s3 ' SHP Connection

		s1 = "fsd://" & s2
		s1a = "Library://" & s2 & ".FeatureSource"
		s4 = "Default:" & s3
		'	Dim sSHPFileName As String = sSHPFileFolder & s0 & "\" & s3 & ".shp"

		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(sPath)
		oMainXmlNode = LayerDef.LastChild()
		sMain = oMainXmlNode.Name
		Dim i As Integer = oMainXmlNode.ChildNodes.Count
		ReDim oXmlNode(oMainXmlNode.ChildNodes.Count - 1)
		For iIndex As Integer = 0 To oXmlNode.GetUpperBound(0)
			oXmlNode(iIndex) = oMainXmlNode.ChildNodes.Item(iIndex)
		Next
		'	DMCommon.Debug.MsgBox("05_379s!", "sSHPFileFullName=", sSHPFileFullName, sFileName, "s1=", s1, "s1a=", s1a, "s2=", s2, "s3=", s3, "s4=", s4)

		oXmlNode1 = oXmlNode(1).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode2.InnerText = s4

		oXmlNode1 = oXmlNode(0).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.Attributes.Item(1)
		oXmlNode2.InnerText = s1


		oXmlNode1 = oXmlNode(0).Attributes.Item(0)
		oXmlNode1.InnerText = s4


		oXmlNode1 = oXmlNode(1).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.Attributes.Item(0)

		oXmlNode1 = oXmlNode(2).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode1 = oXmlNode(2).ChildNodes.Item(0)
		oXmlNode1.InnerText = s3

		oXmlNode1 = oXmlNode(3).Attributes.Item(0)
		oXmlNode1.InnerText = s4


		oXmlNode1 = oXmlNode(4).Attributes.Item(0)
		oXmlNode1.InnerText = s4



		oXmlNode1 = oXmlNode(5).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode1 = oXmlNode(5).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode3 = oXmlNode2.ChildNodes.Item(0)


		'Dim oTestA As System.Xml.XmlNode = oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Item(0)
		'Dim oTestB As System.Xml.XmlNode = oTestA.ChildNodes.Item(1)
		'Dim oTestC As System.Xml.XmlNode = oTestB.ChildNodes.Item(0)
		'Dim oTestD As System.Xml.XmlNode = oTestC.ChildNodes.Item(1)
		'	Dim oTestE As System.Xml.XmlNode = oTestD.ChildNodes.Item(0)




		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$1 ", oXmlNode4.ChildNodes.Count, oXmlNode3.ChildNodes.Count, oXmlNode3.Attributes.Count, oXmlNode4.Attributes.Count, oXmlNode4.ChildNodes.Item(0).Value, oXmlNode4.ChildNodes.Item(0).InnerText, oXmlNode4.ChildNodes.Item(0).LocalName, oXmlNode4.ChildNodes.Item(0).Prefix, oXmlNode4.ChildNodes.Item(0).InnerXml)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$2 ", oXmlNode3.ChildNodes.Item(0).ChildNodes.Count, oXmlNode3.ChildNodes.Item(0).Attributes.Count)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$3 ", oXmlNode3.ChildNodes.Item(4).ChildNodes.Count, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).Value, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Count, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).Attributes.Count)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$4 ", oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).Value, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Count, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).Attributes.Count)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$5 ", oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Item(0).Value, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Item(0).InnerXml, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Item(0).ChildNodes.Count, oXmlNode3.ChildNodes.Item(4).ChildNodes.Item(0).ChildNodes.Item(0).Attributes.Count)

		'DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$8 ", oTestB.ChildNodes.Item(0).ChildNodes.Count, oTestB.ChildNodes.Item(0).Attributes.Count, oTestB.ChildNodes.Item(0).InnerXml)
		'DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$9 ", oTestB.ChildNodes.Item(1).ChildNodes.Count, oTestB.ChildNodes.Item(1).Attributes.Count, oTestB.ChildNodes.Item(1).InnerXml)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$81 ", oTestC.ChildNodes.Item(0).ChildNodes.Count, oTestC.ChildNodes.Item(0).Attributes.Count, oTestC.ChildNodes.Item(0).InnerXml)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$82 ", oTestC.ChildNodes.Item(1).ChildNodes.Count, oTestC.ChildNodes.Item(1).Attributes.Count, oTestC.ChildNodes.Item(1).InnerXml)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$83 ", oTestC.ChildNodes.Item(2).ChildNodes.Count, oTestC.ChildNodes.Item(2).Attributes.Count, oTestC.ChildNodes.Item(2).InnerXml)





		'	DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$821 ", oTestE.ChildNodes.Item(0).ChildNodes.Count, oTestE.ChildNodes.Item(0).Attributes.Count, oTestE.ChildNodes.Item(0).InnerText, oTestE.ChildNodes.Item(0).InnerXml)
		'DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$822", oTestE.ChildNodes.Item(1).ChildNodes.Count, oTestE.ChildNodes.Item(1).Attributes.Count, oTestE.ChildNodes.Item(1).InnerText, oTestE.ChildNodes.Item(1).InnerXml)
		'DMAcadExt.AcadDocument.WriteDebugMessageN("$$$$823 ", oTestE.ChildNodes.Item(2).ChildNodes.Count, oTestE.ChildNodes.Item(2).Attributes.Count, oTestE.ChildNodes.Item(2).InnerText, oTestE.ChildNodes.Item(2).InnerXml)

		'	oTestE.ChildNodes.Item(2).InnerText = "FFFF0000"
		zzSetLayerDefColor(oXmlNode3, tColor)
		oXmlNode4 = oXmlNode3.ChildNodes.Item(0)
		oXmlNode4.InnerText = s1a

		oXmlNode5 = oXmlNode3.ChildNodes.Item(1)
		oXmlNode6 = oXmlNode5.ChildNodes.Item(0)
		oXmlNode6.InnerText = s4

		oXmlNode1 = oXmlNode(6).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		If False Then
			oXmlNode1 = oXmlNode(6).ChildNodes.Item(0)
			oXmlNode4 = oXmlNode3.ChildNodes.Item(0)
			oXmlNode4.InnerText = s1
		End If


		oXmlNode1 = oXmlNode(7).Attributes.Item(0)
		oXmlNode1.InnerText = s1

		oXmlNode1 = oXmlNode(7).ChildNodes.Item(0)
		oXmlNode1.InnerText = s2

		oXmlNode1 = oXmlNode(8).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		oXmlNode1 = oXmlNode(9).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		oXmlNode1 = oXmlNode(10).Attributes.Item(0)
		oXmlNode1.InnerText = s1
		oXmlNode1 = oXmlNode(10).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode3 = oXmlNode2.ChildNodes.Item(1)
		oXmlNode4 = oXmlNode3.ChildNodes.Item(1)
		oXmlNode4.InnerText = sSHPFileFullName
		If DMCommon.Debug.Debug Then
			'LayerDef.Save("D:\" & sFileName & ".xml")
		End If

		Return LayerDef.OuterXml
	End Function
	Private Shared Sub zzSetLayerDefColor(oXmlNode As System.Xml.XmlNode, tColor As System.Drawing.Color)
		If Not tColor.IsEmpty Then
			Dim sColor As String = Hex(tColor.ToArgb)
			Dim oChildNode As System.Xml.XmlNode = oXmlNode.ChildNodes.Item(4)
			oChildNode = oChildNode.ChildNodes.Item(0)
			oChildNode = oChildNode.ChildNodes.Item(0)

			oChildNode = oChildNode.ChildNodes.Item(1)
			oChildNode = oChildNode.ChildNodes.Item(0)
			oChildNode = oChildNode.ChildNodes.Item(1)
			oChildNode = oChildNode.ChildNodes.Item(0)

			oChildNode = oChildNode.ChildNodes.Item(2)
			oChildNode.InnerText = sColor
		End If
	End Sub

	Private Shared Function zzGetActualXML(sSHPFileFullName As String, sBlockNo As String) As String
		Const sPath As String = "M:\Dm_Work\Template's\LayerDef\p2793.layer"

		'R:\Gushim\Gushim-MMG-ISRAEL\2018\2018.06\GIS\parcel\
		'	Const sOutputPath As String = "D:\Parcel1.layer"
		'	Dim iGushNo As Integer = 2793
		Dim oMainXmlNode As System.Xml.XmlNode
		Dim sMain As String
		Dim oXmlNode() As System.Xml.XmlNode
		Dim oXmlNode1 As System.Xml.XmlNode

		Dim oXmlNode2 As System.Xml.XmlNode
		Dim oXmlNode3 As System.Xml.XmlNode
		Dim oXmlNode4 As System.Xml.XmlNode
		Dim oXmlNode5 As System.Xml.XmlNode
		Dim oXmlNode6 As System.Xml.XmlNode



		Dim s0 As String
		Dim s1 As String '= "fsd://shp_p2792"
		Dim s1a As String '= "Library://shp_p2792.FeatureSource"

		Dim s2 As String '= "shp_p2792"
		Dim s3 As String '= "p2792"
		Dim s4 As String ' = "Default:p2792"
		s0 = sBlockNo
		s3 = "p" & s0
		s2 = "shp_" & s3 ' SHP Connection
		s1 = "fsd://" & s2
		s1a = "Library://" & s2 & ".FeatureSource"
		s4 = "Default:" & s3
		'	Dim sSHPFileName As String = sSHPFileFolder & s0 & "\" & s3 & ".shp"

		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(sPath)
		oMainXmlNode = LayerDef.LastChild()
		sMain = oMainXmlNode.Name
		Dim i As Integer = oMainXmlNode.ChildNodes.Count
		ReDim oXmlNode(oMainXmlNode.ChildNodes.Count - 1)
		For iIndex As Integer = 0 To oXmlNode.GetUpperBound(0)
			oXmlNode(iIndex) = oMainXmlNode.ChildNodes.Item(iIndex)
		Next
		DMCommon.Debug.MsgBox("05_379s!", "sSHPFileFullName=", sSHPFileFullName, "s0=", s0, "s1=", s1, "s1a=", s1a, "s2=", s2, "s3=", s3, "s4=", s4)
		'i = oXmlNode(0).Attributes.Count
		'oXmlNode1 = oXmlNode(0).Attributes.Item(0)

		oXmlNode1 = oXmlNode(1).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode2.InnerText = s4

		oXmlNode1 = oXmlNode(0).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.Attributes.Item(1)
		oXmlNode2.InnerText = s1


		oXmlNode1 = oXmlNode(0).Attributes.Item(0)
		oXmlNode1.InnerText = s4


		oXmlNode1 = oXmlNode(1).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.Attributes.Item(0)

		oXmlNode1 = oXmlNode(2).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode1 = oXmlNode(2).ChildNodes.Item(0)
		oXmlNode1.InnerText = s3

		oXmlNode1 = oXmlNode(3).Attributes.Item(0)
		oXmlNode1.InnerText = s4


		oXmlNode1 = oXmlNode(4).Attributes.Item(0)
		oXmlNode1.InnerText = s4



		oXmlNode1 = oXmlNode(5).Attributes.Item(0)
		oXmlNode1.InnerText = s4

		oXmlNode1 = oXmlNode(5).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode3 = oXmlNode2.ChildNodes.Item(0)

		oXmlNode4 = oXmlNode3.ChildNodes.Item(0)
		oXmlNode4.InnerText = s1a

		oXmlNode5 = oXmlNode3.ChildNodes.Item(1)
		oXmlNode6 = oXmlNode5.ChildNodes.Item(0)
		oXmlNode6.InnerText = s4

		oXmlNode1 = oXmlNode(6).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		If False Then
			oXmlNode1 = oXmlNode(6).ChildNodes.Item(0)
			oXmlNode4 = oXmlNode3.ChildNodes.Item(0)
			oXmlNode4.InnerText = s1
		End If


		oXmlNode1 = oXmlNode(7).Attributes.Item(0)
		oXmlNode1.InnerText = s1

		oXmlNode1 = oXmlNode(7).ChildNodes.Item(0)
		oXmlNode1.InnerText = s2

		oXmlNode1 = oXmlNode(8).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		oXmlNode1 = oXmlNode(9).Attributes.Item(0)
		oXmlNode1.InnerText = s1


		oXmlNode1 = oXmlNode(10).Attributes.Item(0)
		oXmlNode1.InnerText = s1
		oXmlNode1 = oXmlNode(10).ChildNodes.Item(0)
		oXmlNode2 = oXmlNode1.ChildNodes.Item(0)
		oXmlNode3 = oXmlNode2.ChildNodes.Item(1)
		oXmlNode4 = oXmlNode3.ChildNodes.Item(1)
		oXmlNode4.InnerText = sSHPFileFullName
		Return LayerDef.OuterXml
	End Function
	Private Sub zzCreateTopo(sTopoName As String, oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim colLines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      colLines.Add(oPolyline.ObjectId)
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      System.Windows.Forms.MessageBox.Show(CStr(oTopos.Exists(sTopoName)) & vbCrLf & sTopoName & vbCrLf & CStr(colLines.Count), "04_031")
      Try
         oTopos.Create(sTopoName, colLines, colNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)
      Catch oMapEx As Autodesk.Gis.Map.MapException

         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode) & ": " & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode) & vbCrLf & sTopoName & ":" & CStr(colLines.Count) & vbCrLf & oMapEx.StackTrace, "01_832a")
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - zzCreateTopo" & ": ")


      End Try
      '	System.Windows.Forms.MessageBox.Show(CStr(oTopos.Exists(sTopoName)) & vbCrLf & "BB", "04_035")
   End Sub
   Private Sub zzImportNew(sShapeFileName As String, Optional sDestLayer As String = Nothing)

      '	Dim oInputLayer As Autodesk.Gis.Map.ImportExport.InputLayer = Nothing
      Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Import, sShapeFileName)
      oShapeExpImp.ImportPolygonsAsClosedPolylines = True
      If sDestLayer IsNot Nothing Then
         oShapeExpImp.SetDestLayer(sDestLayer)
      End If
      oShapeExpImp.Exec()
   End Sub


   Private Sub zzImportA(sShapeFileName As String, Optional sDestLayer As String = Nothing)
      Dim oImporter As Autodesk.Gis.Map.ImportExport.Importer
      Dim oInputLayer As Autodesk.Gis.Map.ImportExport.InputLayer = Nothing
      Dim oEnum As IEnumerator
		Dim oDynamic As System.Object
		Dim iLayerNameType As Autodesk.Gis.Map.ImportExport.LayerNameType
		'   Dim iImportDataMapping As Autodesk.Gis.Map.ImportExport.ImportDataMapping
		Dim sTableNameDefault As String = Nothing
      Dim tRes As Autodesk.Gis.Map.ImportExport.ImportResults
      Dim sLayerName1 As String
      Dim sLayerName2 As String = Nothing

      oImporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Importer
      Try
         oImporter.Init("SHP", sShapeFileName)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(sShapeFileName, "01_540")
      End Try

      Try
         oImporter.ImportPolygonsAsClosedPolylines = True

         If True Then
            oEnum = oImporter.GetEnumerator()
            oEnum.Reset()
            Do While oEnum.MoveNext
               oDynamic = oEnum.Current
               If oDynamic Is Nothing Then
                  DMAcadExt.AcadDocument.WriteMessage(" 130: " & " oDynamic Is Nothing:" & vbCrLf)
               Else
                  oInputLayer = DirectCast(oDynamic, Autodesk.Gis.Map.ImportExport.InputLayer)
                  sLayerName1 = oInputLayer.Name
						'From 2012 oInputLayer.LayerName(iLayerNameType, sLayerName2)
						If sDestLayer IsNot Nothing AndAlso sDestLayer.Length <> 0 Then
                     'System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & sDestLayer & vbCrLf & iLayerNameType.ToString(), "01_832q")
                     oInputLayer.SetLayerName(iLayerNameType, sDestLayer)
                  End If
						'	System.Windows.Forms.MessageBox.Show(iLayerNameType.ToString() & vbCrLf & sLayerName1 & vbCrLf & sLayerName2, "01_869")
						'From 2012  oInputLayer.DataMapping(iImportDataMapping, sTableNameDefault)
						'	System.Windows.Forms.MessageBox.Show(iImportDataMapping.ToString() & vbCrLf & ":" & sTableNameDefault & ":" & vbCrLf & sLayerName2, "01_870")
						'		oEditor.WriteMessage("-131x: " & oInputLayer.Name & "|:|" & iLayerNameType.ToString & "|:|" & oInputLayer.FeatureClassName & "!!" & vbCrLf)
						'		oEditor.WriteMessage("-132: " & sTableNameDefault & ":" & iImportDataMapping.ToString & ":" & oInputLayer.Name & "!")
						''''''''	oInputLayer.SetDataMapping(Autodesk.Gis.Map.ImportExport.ImportDataMapping.NoImportMapping, sTableName)
						oInputLayer.ImportFromInputLayerOn = True
               End If
            Loop
         End If

         tRes = oImporter.Import(True)


      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & sShapeFileName & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & oMapImpExpEx.StackTrace, "01_856")
      End Try
   End Sub
	Private Sub zzClipCalculate(sPolylineLayer As String, iBlockIndex As Integer, ByVal sBlockName As String, ByVal sBlockLayer As String)
		Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
		Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline

		'	Dim bBoundingBox As Boolean
		Dim colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim oBlockMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()
		Dim dDifArea As Double
		lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		' System.Windows.Forms.MessageBox.Show("גוש: " & CStr(mtaShapeFOData(iBlockIndex).Gush.Block) & vbCrLf & sPolylineLayer & vbCrLf & CStr(lstPolylines.Count) & vbCrLf & "'" & sBlockName & "'" & vbCrLf & "'" & sBlockLayer & "'", "02_329E")
		If lstPolylines.Count >= 1 Then
			Dim iaMPgonLoopIndices(lstPolylines.Count - 1) As Integer
			Dim iMPgonLoopIndex As Integer = 0
			For iIndex As Integer = 0 To lstPolylines.Count - 1
				'	System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & CStr(iIndex), "02_328!!!")

				oPolyline = lstPolylines.Item(iIndex)  'lstPolylines.Count - 1 -
				'	System.Windows.Forms.MessageBox.Show(CStr(oPolyline.NumberOfVertices) & vbCrLf & CStr(iIndex), "02_329!AA")

				oPolyline.Layer = "1601"
				oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
				colPolylines.Add(oPolyline.ObjectId)
				oBlockMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)

				iaMPgonLoopIndices(iMPgonLoopIndex) = iIndex
				iMPgonLoopIndex += 1
				mcolDisPlines.Add(oPolyline.ObjectId)
			Next
			mdWorkAreaTotal += oBlockMPolygon.Area
			dDifArea = mtaShapeFOData(iBlockIndex).SumMPgonArea - oBlockMPolygon.Area
			If dDifArea < -0.0001 OrElse dDifArea > 0.0001 Then
				DMAcadExt.AcadDocument.WriteMessage("גוש: " & CStr(mtaShapeFOData(iBlockIndex).Gush.Block) & "הפרש שטחים: " & CStr(dDifArea))
			End If
			'	System.Windows.Forms.MessageBox.Show("גוש: " & CStr(mtaShapeFOData(iBlockIndex).Gush.Block) & vbCrLf & CStr(mtaShapeFOData(iBlockIndex).SumMPgonArea - oBlockMPolygon.Area) & vbCrLf & CStr(mtaShapeFOData(iBlockIndex).SumMPgonArea) & ":" & oBlockMPolygon.Area, "02_328D")
			Dim sLayerName As String = sBlockLayer
			'	Dim sBlockName As String = "1601"
			Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
			'Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
			Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			'Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
			Dim sTest As String = CStr(oBlockMPolygon.NumMPolygonLoops & ":" & CStr(oBlockMPolygon.Area)) & vbCrLf
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			Dim bPosit As Boolean = (oBlockMPolygon.Area > 0.0)
			colPolylines.Clear()
			For iIndex As Integer = 0 To oBlockMPolygon.NumMPolygonLoops - 1
				sTest &= oBlockMPolygon.GetLoopDirection(iIndex).ToString() & ":" & lstPolylines.Item(iaMPgonLoopIndices(iIndex)).Handle.ToString() & vbCrLf
				If (bPosit AndAlso oBlockMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Exterior) OrElse (Not bPosit AndAlso oBlockMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Interior) Then
					colPolylines.Add(lstPolylines.Item(iaMPgonLoopIndices(iIndex)).ObjectId)
					'	lstPolylines.Item(iaMPgonLoopIndices(iIndex)).Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
				End If
			Next
			Dim sTest1 As String = "Nothing"

			If colPolylines IsNot Nothing Then
				sTest1 = colPolylines.Count.ToString
			End If
			'   System.Windows.Forms.MessageBox.Show("zzClipCalculate" & vbCrLf & sTest1 & vbCrLf & sLayerName & vbCrLf & sBlockName, "02_331a")
			'גושים
			Dim bCentroidSuccess As Boolean
			Try
				'   DMCommon.Debug.MsgBox("05_377!!!", "CreateCentroids", DMCommon.Debug.ColCount(colCentroids), DMCommon.Debug.ColCount(colPolylines), sLayerName, sBlockName)
				oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
				bCentroidSuccess = True
			Catch oMapEx As Autodesk.Gis.Map.MapException
				bCentroidSuccess = False
				System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sLayerName & vbCrLf & sBlockName & vbCrLf & CStr(colPolylines.Count), "01_863")
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - zzEraseBoudingBox" & ": ")
			End Try
			'   System.Windows.Forms.MessageBox.Show(CStr(colCentroids.Count), "02_555")
			Dim tGushData As GushData
			sTest = "a"
			'  fffffffffff()
			'    Dim ia1601AttribIndices() As Integer = {0, 1}
			'   Dim sa1601AttribText(ia1601AttribIndices.GetUpperBound(0)) As String
			'    System.Windows.Forms.MessageBox.Show(CStr(colCentroids.Count) & vbCrLf & CStr(colPolylines.Count), "01_141_***")
			If bCentroidSuccess Then
				For iIndex As Integer = 0 To colPolylines.Count - 1
					tCentroidAcObjID = colCentroids.Item(iIndex)
					oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					If oBlockRef IsNot Nothing Then
						oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(10.0)
						'  oBlockRef.ResetScaleDependentProperties()
					End If
					tPgonAcObjID = colPolylines.Item(iIndex)
					tGushData = mtaShapeFOData(iBlockIndex).Gush

					'	sa1601AttribText(0) = CStr(mtaShapeFOData(iBlockIndex).Block)
					'	sa1601AttribText(1) = CStr(mtaShapeFOData(iBlockIndex).BlockAdd)
					'	DMCommon.Debug.MsgBox("12_420b", sTest)
					sTest = "b"
					'	DMCommon.Debug.MsgBox("12_420c", tGushData.Key, tGushData.Block, tGushData.BlockAdd, tGushData.Status, tGushData.IsAnality, sTest)
					sTest = "c"
					If mdicGushFromParcelData IsNot Nothing AndAlso mdicGushFromParcelData.TryGetValue(tGushData.Key, tGushData) Then
						sTest = "d"
						'DMCommon.Debug.MsgBox("12_420d", tGushData.Block, tGushData.BlockAdd, tGushData.Status, tGushData.IsAnality, sTest)
						'  DMCommon.Functions.DispArray(tGushData.GetAttribText(), "tGushData.GetAttribText", True)
					Else
						sTest = "e"
						DMCommon.Debug.MsgBox("12_420N", mdicGushFromParcelData, tGushData.Block, tGushData.BlockAdd, tGushData.Status, tGushData.IsAnality, sTest)
					End If
					DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, GushData.mia1601AttribIndices, tGushData.GetAttribText())

					'  oEntity = DMAcadExt.AcadTransaction.GetEntity(tPgonAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					''''''''	oEntity.Layer = "1601"
				Next
			End If

		End If
	End Sub
	Public Shared Sub zzCreateMapLayersNew(sSHPFileFullName As String, sLayerName As String, tColor As System.Drawing.Color)

		'Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shp_Parcel"
		Dim sFeatureSrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim sLayerPath As String = "Library://" & sLayerName & ".LayerDefinition"
		DMCommon.Debug.MsgBox("13_106", sFeatureSrc, sLayerPath, sLayerName)

		Dim oResourceService As OSGeo.MapGuide.MgResourceService = DirectCast(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)
		'.GetClassName
		'	DMCommon.Debug.MsgBox("021220_3a", oResourceService.GetClassName, oResourceService.GetClassId)
		'load template xml file
		'Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		'Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()


		Dim featuresrcid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(sFeatureSrc)

		Dim oMgLayerBase As OSGeo.MapGuide.MgLayerBase = Nothing
		Dim oLayerDefID As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(sLayerPath)

		'make mods to xml
		'set feature source


		'convert xml content into byte data
		'	Dim strlayerdef As String = zzGetActualXML(sSHPFileFullName, sBlockNo)
		Dim sLayerDef As String = zzGetActualLayerDef(sSHPFileFullName, sLayerName, tColor)




		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(sLayerDef)

		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim oByteSource As OSGeo.MapGuide.MgByteSource = New OSGeo.MapGuide.MgByteSource(bytes, bytes.Length)
		'	DMCommon.Debug.MsgBox("13_032j", sBlockNo)
		'dd layer to definitation
		oResourceService.SetResource(oLayerDefID, oByteSource.GetReader(), Nothing)
		'DMCommon.Debug.MsgBox("13_032k", oLayerDefID.GetName, oLayerDefID.GetClassName, oLayerDefID.GetClassId, oLayerDefID.GetResourceType())
		Try

			'create a New layer based on the def
			'	layer = New MgLayerBase(layerdefid, rs)
			'Dim layername As String = sLayerName
			'layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			'	AcMapMap.GetCurrentMap().GetLayers().Add(layer)

			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(oLayerDefID, oResourceService)

			oMapLayer.Name = sLayerName
			'	DMCommon.Debug.MsgBox("13_032m", oMapLayer.Name, oMapLayer.FeatureSourceId)
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As OSGeo.MapGuide.MgLayerCollection = oMap.GetLayers()
			DMCommon.Debug.MsgBox("13_032NN", colLayers.Count, oMapLayer.FeatureClassName, oMapLayer.FeatureSourceId, oMapLayer.GetName())
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch oEx As OSGeo.MapGuide.MgException


			DMCommon.Debug.UserMsg("zzCreateMapLayersNew", oEx.Message, oEx.StackTrace)
		End Try

	End Sub

	Public Shared Sub zzCreateMapLayers(sSHPFileFullName As String, sBlockNo As String)
		Dim sLayerName As String = "p" & sBlockNo
		'Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shp_Parcel"

		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"


		Dim rs As OSGeo.MapGuide.MgResourceService = DirectCast(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)
		Dim tColor As System.Drawing.Color
		'load template xml file
		'Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		'Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()


		Dim featuresrcid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(featuresrc)

		Dim layer As OSGeo.MapGuide.MgLayerBase = Nothing
		Dim layerdefid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source


		'convert xml content into byte data
		'	Dim strlayerdef As String = zzGetActualXML(sSHPFileFullName, sBlockNo)
		Dim strlayerdef As String = zzGetActualLayerDef(sSHPFileFullName, sLayerName, tColor)




		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)
		'DMCommon.Debug.MsgBox("13_032i")
		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As OSGeo.MapGuide.MgByteSource = New OSGeo.MapGuide.MgByteSource(bytes, bytes.Length)
		'	DMCommon.Debug.MsgBox("13_032j", sBlockNo)
		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		'	DMCommon.Debug.MsgBox("13_032k", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try

			'create a New layer based on the def
			'	layer = New MgLayerBase(layerdefid, rs)
			'Dim layername As String = sLayerName
			'layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			'	AcMapMap.GetCurrentMap().GetLayers().Add(layer)

			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)

			oMapLayer.Name = sLayerName
			'	DMCommon.Debug.MsgBox("13_032m", oMapLayer.Name, oMapLayer.FeatureSourceId)
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As OSGeo.MapGuide.MgLayerCollection = oMap.GetLayers()
			'	DMCommon.Debug.MsgBox("13_032NN", colLayers.Count)
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch oEx As OSGeo.MapGuide.MgException

			System.Windows.Forms.MessageBox.Show(oEx.Message)
		End Try

	End Sub
	Public Shared Sub zzCreateBoundBoxWorkAreaMapLayers(sSHPFileFullName As String)
		Dim sLayerName As String = "BoundBox_WorkArea"
		'Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shp_Parcel"
		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"


		Dim rs As OSGeo.MapGuide.MgResourceService = DirectCast(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)

		'load template xml file
		'Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		'Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()


		Dim featuresrcid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(featuresrc)

		Dim layer As OSGeo.MapGuide.MgLayerBase = Nothing
		Dim layerdefid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source
		Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\BoundBox_WorkArea.layer"

		'	Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)


		Dim strlayerdef As String = LayerDef.OuterXml
		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)

		'convert xml content into byte data




		'DMCommon.Debug.MsgBox("13_032i")
		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As OSGeo.MapGuide.MgByteSource = New OSGeo.MapGuide.MgByteSource(bytes, bytes.Length)
		'DMCommon.Debug.MsgBox("13_032wj")
		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		''''''''''''''''''	DMCommon.Debug.MsgBox("13_032wk", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try



			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)

			oMapLayer.Name = sLayerName
			'DMCommon.Debug.MsgBox("13_032wm", oMapLayer.Name, oMapLayer.FeatureSourceId)
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As OSGeo.MapGuide.MgLayerCollection = oMap.GetLayers()
			'	DMCommon.Debug.MsgBox("13_032wNN", colLayers.Count)
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch oEx As OSGeo.MapGuide.MgException

			System.Windows.Forms.MessageBox.Show(oEx.Message)
		End Try

	End Sub
	Public Shared Sub zzCreateWorkAreaMapLayers(sSHPFileFullName As String)
		Dim sLayerName As String = "WorkArea"
		'Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shp_Parcel"
		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"


		Dim rs As OSGeo.MapGuide.MgResourceService = DirectCast(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)

		'load template xml file
		'Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		'Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()


		Dim featuresrcid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(featuresrc)

		Dim layer As OSGeo.MapGuide.MgLayerBase = Nothing
		Dim layerdefid As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source
		Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\WorkArea.layer"

		'	Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)


		Dim strlayerdef As String = LayerDef.OuterXml
		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)

		'convert xml content into byte data




		'DMCommon.Debug.MsgBox("13_032i")
		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As OSGeo.MapGuide.MgByteSource = New OSGeo.MapGuide.MgByteSource(bytes, bytes.Length)
		'DMCommon.Debug.MsgBox("13_032wj")
		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		'DMCommon.Debug.MsgBox("13_032wSUMk", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try



			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)

			oMapLayer.Name = sLayerName
			'DMCommon.Debug.MsgBox("13_032wm", oMapLayer.Name, oMapLayer.FeatureSourceId)
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As OSGeo.MapGuide.MgLayerCollection = oMap.GetLayers()

			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch oEx As OSGeo.MapGuide.MgException

			System.Windows.Forms.MessageBox.Show(oEx.Message)
		End Try

	End Sub
	Private Sub zzClipWorkAreaCalculate(sPolylineLayer As String)
      Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline

      '	Dim bBoundingBox As Boolean
      Dim colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oWorkAreaMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()
      lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      System.Windows.Forms.MessageBox.Show("תחום עבודה" & vbCrLf & sPolylineLayer & vbCrLf & CStr(lstPolylines.Count), "02_328B")
      If lstPolylines.Count >= 1 Then
         Dim iaMPgonLoopIndices(lstPolylines.Count - 1) As Integer
         Dim iMPgonLoopIndex As Integer = 0
         For iIndex As Integer = 0 To lstPolylines.Count - 1
            '	System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & CStr(iIndex), "02_328!!!")

            oPolyline = lstPolylines.Item(iIndex)  'lstPolylines.Count - 1 -
            '	System.Windows.Forms.MessageBox.Show(CStr(oPolyline.NumberOfVertices) & vbCrLf & CStr(iIndex), "02_329!AA")


            oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.LightCyan)

            oWorkAreaMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)

            iaMPgonLoopIndices(iMPgonLoopIndex) = iIndex
            iMPgonLoopIndex += 1

         Next


         System.Windows.Forms.MessageBox.Show("תחום עבודה" & vbCrLf & CStr(mdWorkAreaTotal - oWorkAreaMPolygon.Area) & vbCrLf & CStr(mdWorkAreaTotal) & ":" & oWorkAreaMPolygon.Area, "02_377")


      End If
   End Sub
   Private Sub zzEraseBoundingBox(sPolylineLayer As String, iBlockIndex As Integer, ByVal sBlockName As String, ByVal sBlockLayer As String)
      Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim oPoint As DMAcadExt.TPlnPoint
      Dim oCurrentBoundingBox As DMAcadExt.TPlnBoundingBox
      Dim bBoundingBox As Boolean
      Dim colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oBlockMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()
      lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & CStr(lstPolylines.Count), "02_327!!!")
      If lstPolylines.Count >= 2 Then
         Dim iaMPgonLoopIndices(lstPolylines.Count - 2) As Integer
         Dim iMPgonLoopIndex As Integer = 0
         For iIndex As Integer = 0 To lstPolylines.Count - 1
            '	System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & CStr(iIndex), "02_328!!!")
            bBoundingBox = False
            oPolyline = lstPolylines.Item(iIndex)  'lstPolylines.Count - 1 -
            '	System.Windows.Forms.MessageBox.Show(CStr(oPolyline.NumberOfVertices) & vbCrLf & CStr(iIndex), "02_329!AA")
            oCurrentBoundingBox = New DMAcadExt.TPlnBoundingBox()

            If oPolyline.NumberOfVertices = 5 Then
               For iVertix As Integer = 0 To 4
                  oPoint = New DMAcadExt.TPlnPoint(oPolyline.GetPoint2dAt(iVertix))
                  oCurrentBoundingBox.Union(oPoint)
               Next
               If moUnionBoundingBox IsNot Nothing Then
                  If moUnionBoundingBox.IsEqualTo(oCurrentBoundingBox, 0.001) Then
                     bBoundingBox = True
                  End If
               ElseIf moaBoundingBoxes IsNot Nothing Then
                  If moaBoundingBoxes(iBlockIndex).IsEqualTo(oCurrentBoundingBox, 0.001) Then
                     bBoundingBox = True
                  End If
               End If
               If bBoundingBox Then
                  oPolyline.Erase()
               End If
            End If
            If Not bBoundingBox Then
               oPolyline.Layer = "1601"
               colPolylines.Add(oPolyline.ObjectId)
               oBlockMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)

               iaMPgonLoopIndices(iMPgonLoopIndex) = iIndex
               iMPgonLoopIndex += 1
            End If
         Next

         System.Windows.Forms.MessageBox.Show(CStr(mtaShapeFOData(iBlockIndex).SumMPgonArea) & ":" & oBlockMPolygon.Area, "02_329")
         Dim sLayerName As String = sBlockLayer
         '	Dim sBlockName As String = "1601"
         Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
         'Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
         Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
         Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
         Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
         Dim sTest As String = CStr(oBlockMPolygon.NumMPolygonLoops & ":" & CStr(oBlockMPolygon.Area)) & vbCrLf
         Dim bPosit As Boolean = (oBlockMPolygon.Area > 0.0)
         colPolylines.Clear()
         For iIndex As Integer = 0 To oBlockMPolygon.NumMPolygonLoops - 1
            sTest &= oBlockMPolygon.GetLoopDirection(iIndex).ToString() & ":" & lstPolylines.Item(iaMPgonLoopIndices(iIndex)).Handle.ToString() & vbCrLf
            If (bPosit AndAlso oBlockMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Exterior) OrElse (Not bPosit AndAlso oBlockMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Interior) Then
               colPolylines.Add(lstPolylines.Item(iaMPgonLoopIndices(iIndex)).ObjectId)
               '	lstPolylines.Item(iaMPgonLoopIndices(iIndex)).Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
            End If
         Next
         'System.Windows.Forms.MessageBox.Show("zzEraseBoundingBox" & vbCrLf & sLayerName & vbCrLf & sBlockName, "02_331a")
         Try
            oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sLayerName & vbCrLf & sBlockName & vbCrLf & CStr(colPolylines.Count), "01_867")
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - zzEraseBoudingBox" & ": ")
            Return
         End Try
         System.Windows.Forms.MessageBox.Show(CStr(colCentroids.Count), "02_333")
         Dim tGushData As GushData
         Dim ia1601AttribIndices() As Integer = {0, 1}
         Dim sa1601AttribText(ia1601AttribIndices.GetUpperBound(0)) As String
         'System.Windows.Forms.MessageBox.Show(CStr(colCentroids.Count) & vbCrLf & CStr(colPolylines.Count), "01_141_***")
         For iIndex As Integer = 0 To colPolylines.Count - 1
            tCentroidAcObjID = colCentroids.Item(iIndex)
            tPgonAcObjID = colPolylines.Item(iIndex)
            tGushData = mtaShapeFOData(iBlockIndex).Gush
            '	sa1601AttribText(0) = CStr(mtaShapeFOData(iBlockIndex).Block)
            '	sa1601AttribText(1) = CStr(mtaShapeFOData(iBlockIndex).BlockAdd)

            DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, GushData.mia1601AttribIndices, tGushData.GetAttribText())
            oEntity = DMAcadExt.AcadTransaction.GetEntity(tPgonAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            ''''''''	oEntity.Layer = "1601"
         Next
      End If
   End Sub




   Public Function DBF2XDataCP(sSHPFileName As String, sAppName As String, bFromTopologia As Boolean) As Integer
      'Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      '	Dim sTableName As String = "LotKParcel"
      '	Dim sPolylineLayer As String = "LotKParcel"
      Dim iTestCounter As Integer
      Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sSHPFileName)
      Dim saParseVal() As String = Split(oFileInfo.Name, ".")
      Dim sClassName As String = saParseVal(0)

      '	System.Windows.Forms.MessageBox.Show(sAppName, "05_235")
      '	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      '	DMAcadExt.AcadTransaction.Start()
      Dim dicRes As Dictionary(Of Integer, OverlayIDs)
      Dim tOverlayIDs As OverlayIDs
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim bRegAppOK As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(sAppName)
      Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
      '	oResBuffer.Add()

      Dim oValue As System.Object
      Dim iFeatureID, iFeatureIDTrim As Integer
      Dim iFeatureIDMin, iFeatureIDMax As Integer
      Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()
      Dim tObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Try
			DMAcadExt.AcadTransaction.OpenHandleDictionary()
			dicRes = GetOverlayData(sSHPFileName, sClassName, bFromTopologia, False)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSHPFileName & vbCrLf & sClassName, "04_242")
         dicRes = Nothing
      End Try

      If dicRes IsNot Nothing Then
         If bRegAppOK Then
            iTestCounter = 0
            lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sClassName, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            '	DMAcadExt.AcadTransaction.GetLayerObjects(sPolylineLayer)
            For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolygons
               iTestCounter += 1
               If oPolygon IsNot Nothing Then
                  oResBuffer = oPolygon.XData
                  Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
                  oValue = taTypedValues(2).Value
                  iFeatureID = CInt(oValue)
                  If iFeatureIDMin = 0 Then
                     iFeatureIDMin = iFeatureID
                     iFeatureIDMax = iFeatureID
                  Else
                     iFeatureIDMin = Math.Min(iFeatureIDMin, iFeatureID)
                     iFeatureIDMax = Math.Max(iFeatureIDMax, iFeatureID)
                  End If
               End If
            Next
            iTestCounter = 0
            Dim oDBobjTest As Autodesk.AutoCAD.DatabaseServices.DBObject
            Dim oEntTest As Autodesk.AutoCAD.DatabaseServices.Entity
            Dim oUnionBoundingBox, oBoundingBox As DMAcadExt.TPlnBoundingBox
            oUnionBoundingBox = New DMAcadExt.TPlnBoundingBox()

            System.Windows.Forms.MessageBox.Show(CStr(lstPolygons.Count) & vbCrLf & sClassName, "04_700")
            For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolygons
               iTestCounter += 1
               If oPolygon IsNot Nothing Then
                  oBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
                  oUnionBoundingBox.Union(oBoundingBox)
                  oResBuffer = oPolygon.XData
                  Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
                  oValue = taTypedValues(2).Value
                  iFeatureID = CInt(oValue)
                  iFeatureIDTrim = iFeatureID - iFeatureIDMin + 1
                  If dicTest.ContainsKey(iFeatureIDTrim) Then
                     tObjID = dicTest.Item(iFeatureIDTrim)
							'	oPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
							oDBobjTest = DMAcadExt.AcadTransaction.GetDBObject(tObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                     oEntTest = DirectCast(oDBobjTest, Autodesk.AutoCAD.DatabaseServices.Entity)
                     '	oEntTest.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Magenta)
                  Else
                     dicTest.Add(iFeatureIDTrim, oPolygon.ObjectId)
                  End If
                  '	DMAcadExt.AcadTransaction.GetXrecord()
                  If dicRes.TryGetValue(iFeatureIDTrim, tOverlayIDs) Then

							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1001, sAppName)
                     oResBuffer.Add(oVal)
                     oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "{")
                     oResBuffer.Add(oVal)
                     oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iFeatureIDTrim)
                     oResBuffer.Add(oVal)
                     oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, tOverlayIDs.ID1)
                     oResBuffer.Add(oVal)
                     oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, tOverlayIDs.ID2)
                     oResBuffer.Add(oVal)
                     oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "}")
                     oResBuffer.Add(oVal)
                     oPolygon.XData = oResBuffer
                  Else
                     If iTestCounter < 3 Then
                        System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iFeatureID) & vbCrLf & CStr(dicRes.Count), "04_201a")
                     End If
                  End If
               End If
            Next
            Return dicRes.Count
            '	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
         Else
            System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & sAppName, "04_202")
            Return 0
         End If
      Else
         System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & sSHPFileName, "04_203")
         DMAcadExt.AcadDocument.WriteMessage("25-- Not exists" & ":" & CStr(sClassName))
         Return 0
      End If
   End Function
   Public Function DBF2XDataTopo(sSHPFileName As String, sAppName As String, bFromTopologia As Boolean, bIDAddExists As Boolean) As Integer
      Dim iTestCounter As Integer
      Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sSHPFileName)
      Dim saParseVal() As String = Split(oFileInfo.Name, ".")
      Dim sClassName As String = saParseVal(0)
      Dim dicRes As Dictionary(Of Integer, OverlayIDs) '= GetOverlayData(sSHPFileName, sClassName)
      Dim tOverlayIDs As OverlayIDs
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(sAppName)
      Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
      Dim oValue As System.Object
      Dim iFeatureID, iFeatureIDTrim As Integer
      Dim iFeatureIDMin, iFeatureIDMax As Integer
      Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()
      Dim tObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Try
         dicRes = GetOverlayData(sSHPFileName, sClassName, True, bIDAddExists)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSHPFileName & vbCrLf & sClassName, "04_239")
         dicRes = Nothing
      End Try

		If dicRes IsNot Nothing Then
			DMCommon.Debug.MsgBox("081220_1", dicRes.Count, sClassName)
			If bRes Then
				iTestCounter = 0
				lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sClassName, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				DMCommon.Debug.MsgBox("081220_2", lstPolygons.Count, sClassName)
				'	DMAcadExt.AcadTransaction.GetLayerObjects(sPolylineLayer)

				For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolygons
					iTestCounter += 1
					If oPolygon IsNot Nothing Then
						oResBuffer = oPolygon.XData
						Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
						oValue = taTypedValues(2).Value
						iFeatureID = CInt(oValue)
						If iFeatureIDMin = 0 Then
							iFeatureIDMin = iFeatureID
							iFeatureIDMax = iFeatureID
						Else
							iFeatureIDMin = Math.Min(iFeatureIDMin, iFeatureID)
							iFeatureIDMax = Math.Max(iFeatureIDMax, iFeatureID)
						End If
					End If
				Next
				iTestCounter = 0
				Dim oDBobjTest As Autodesk.AutoCAD.DatabaseServices.DBObject
				Dim oEntTest As Autodesk.AutoCAD.DatabaseServices.Entity
				Dim oUnionBoundingBox, oBoundingBox As DMAcadExt.TPlnBoundingBox
				oUnionBoundingBox = New DMAcadExt.TPlnBoundingBox()

				For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolygons
					iTestCounter += 1
					If oPolygon IsNot Nothing Then
						oBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
						oUnionBoundingBox.Union(oBoundingBox)
						oResBuffer = oPolygon.XData
						Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
						oValue = taTypedValues(2).Value
						iFeatureID = CInt(oValue)
						iFeatureIDTrim = iFeatureID - iFeatureIDMin + 1
						If dicTest.ContainsKey(iFeatureIDTrim) Then
							tObjID = dicTest.Item(iFeatureIDTrim)

							'	oPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
							oDBobjTest = DMAcadExt.AcadTransaction.GetDBObject(tObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
							oEntTest = DirectCast(oDBobjTest, Autodesk.AutoCAD.DatabaseServices.Entity)
							'	oEntTest.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Magenta)
						Else
							dicTest.Add(iFeatureIDTrim, oPolygon.ObjectId)
						End If

						If dicRes.TryGetValue(iFeatureIDTrim, tOverlayIDs) Then
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1001, sAppName)
							oResBuffer.Add(oVal)
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "{")
							oResBuffer.Add(oVal)
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iFeatureIDTrim)
							oResBuffer.Add(oVal)
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, tOverlayIDs.ID1)
							oResBuffer.Add(oVal)
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, tOverlayIDs.ID2)
							oResBuffer.Add(oVal)
							If bIDAddExists Then
								oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, tOverlayIDs.ID3)
								oResBuffer.Add(oVal)
							End If
							oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "}")
							oResBuffer.Add(oVal)
							oPolygon.XData = oResBuffer

							Dim oaVal() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
							'	DMAcadExt.AcadDocument.WriteMessage("!@@! " & CStr(oaVal.GetUpperBound(0)) & "||" & CStr(iFeatureIDTrim) & ":" & CStr(tOverlayIDs.ID1) & ":" & CStr(tOverlayIDs.ID2) & ":" & CStr(tOverlayIDs.ID3))
						Else
							If iTestCounter < 3 Then
								System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iFeatureID) & vbCrLf & CStr(dicRes.Count), "04_201")
							End If
						End If
					End If
				Next
				Return lstPolygons.Count
				'	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
			Else

				System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & sAppName, "04_202")
				Return 0
			End If
		Else
			System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & sSHPFileName, "04_203")
         DMAcadExt.AcadDocument.WriteMessage("25-- Not exists" & ":" & CStr(sClassName) & vbCrLf)
         Return 0
      End If
   End Function
   Private Function zzGetExtent(sSHPFileName As String) As DMAcadExt.TPlnBoundingBox
      Dim oConnectionManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(oConnectionManager.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim connInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo

      Dim oProperties As Connections.IConnectionPropertyDictionary = connInfo.ConnectionProperties
      Dim coordSys As String = Nothing
      Dim wellKnownText As String = Nothing
      Dim desc As String = Nothing
      Dim btaBYTE() As Byte
      Dim oGeometry As IGeometry = Nothing
      Dim geomFactory As FgfGeometryFactory = New FgfGeometryFactory()
      Dim sExtent As String = Nothing
      Dim iExtentType As SpatialContextExtentType
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      '     DMCommon.Debug.MsgBox("09_177z", sSHPPropFileName, sSHPFileName)
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Dim connState As Connections.ConnectionState = oConnection.Open()
      Dim oSpatialContextReader As ISpatialContextReader

      Try
         Dim oGetSpatialContexts As IGetSpatialContexts = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_GetSpatialContexts), IGetSpatialContexts)
         oSpatialContextReader = oGetSpatialContexts.Execute()
      Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(connState.ToString() & vbCrLf & sSHPFileName & vbCrLf & sSHPPropFileName & vbCrLf & oEx.Message, "01_231 Extent")
			Return Nothing
      End Try
      '    DMCommon.Debug.MsgBox("09_178k", sSHPPropFileName, sSHPFileName)
      Dim xyTolerance As Double
      Dim zTolerance As Double
      Dim oEnvelope As IEnvelope
      Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = Nothing
      Dim oMinPoint, oMaxPoint As DMAcadExt.TPlnPoint

		While oSpatialContextReader.ReadNext()
         Name = oSpatialContextReader.GetName()
         coordSys = oSpatialContextReader.GetCoordinateSystem()
         wellKnownText = oSpatialContextReader.GetCoordinateSystemWkt()
         desc = oSpatialContextReader.GetDescription()
         btaBYTE = oSpatialContextReader.GetExtent()

         oGeometry = geomFactory.CreateGeometryFromFgf(btaBYTE)
         sExtent = oGeometry.Text

         iExtentType = oSpatialContextReader.GetExtentType()
         xyTolerance = oSpatialContextReader.GetXYTolerance()
         zTolerance = oSpatialContextReader.GetZTolerance()
         DMAcadExt.AcadDocument.WriteMessage(Name & ":" & iExtentType.ToString() & ":" & CStr(desc))
         Try
            oEnvelope = oGeometry.Envelope
            
            If oEnvelope IsNot Nothing Then
               DMAcadExt.AcadDocument.WriteDebugMessage(oEnvelope.MaxX.ToString(), oEnvelope.MinY.ToString)
            End If
            oMinPoint = New DMAcadExt.TPlnPoint(oEnvelope.MinX, oEnvelope.MinY)
            oMaxPoint = New DMAcadExt.TPlnPoint(oEnvelope.MaxX, oEnvelope.MaxY)

            ' DMCommon.Debug.MsgBox("05_399!!!", oMinPoint, oMaxPoint)

            oBoundingBox = New DMAcadExt.TPlnBoundingBox(oMinPoint, oMaxPoint)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oEx.GetType().ToString(), "01_854s")
         End Try

       


      End While
      '    DMCommon.Debug.MsgBox("09_178w", oBoundingBox.AcGePoint(True, True), sSHPFileName)
      ' DMAcadExt.AcadDocument.WriteMessage(oGeometry.Text)

      oSpatialContextReader.Dispose()
      Return oBoundingBox
   End Function
   Public Sub TestS()
      zzCreateDataStore()
   End Sub
	Public Sub imp_Set(sShapeFileName As String, sShapeFolderName As String, sFileName As String, hsGush As HashSet(Of Integer), dicGushFromParcelData As Dictionary(Of Integer, GushData), iMapThemeID As DMAcadExt.enMapTheme, tFilterList As DMCommon.dmList, bInnerPolygons As Boolean)
		'DMCommon.Debug.MsgBox("01_000Ad", sShapeFileName, sShapeFolderName, sFileName, iMapThemeID, tFilterList.Exists, tFilterList.List, hsGush.Count, dicGushFromParcelData.Count)
		'05/07/2017!!!
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim tShapeFOData As ShapeFOData
		tShapeFOData = New ShapeFOData(sShapeFolderName, sFileName)
		'   System.Windows.Forms.MessageBox.Show(iMapThemeID.ToString() & vbCrLf & sShapeFolderName & vbCrLf & sShapeFileName, "07_032")
		Dim oPolygonSet As TplnPolygonSet = New TplnPolygonSet(iMapThemeID)
		If False And Not tFilterList.Exists Then
			oPolygonSet.Import(sShapeFileName)
			Try
				ConnectToShape(tShapeFOData.ConnectionName, tShapeFOData.ShapeFileName)

				AddLayerToMap(tShapeFOData.ConnectionName)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
			End Try
		Else

			'!!!!!!!!!!!!!!!!!!!!! 26/07/2018 main proc.

			msRootPath = sShapeFolderName

			msaParcelFolders = tFilterList.Values
			ReDim msaParcelFolders(dicGushFromParcelData.Count - 1)
			ReDim mtaGushData(dicGushFromParcelData.Count - 1)
			ReDim miaCdBlocks(dicGushFromParcelData.Count - 1)
			Dim iIndex As Integer = 0
			For Each tGushData As GushData In dicGushFromParcelData.Values
				msaParcelFolders(iIndex) = tGushData.Folder
				mtaGushData(iIndex) = tGushData
				miaCdBlocks(iIndex) = tGushData.Block
				iIndex += 1
			Next
			'  DMCommon.Debug.MsgBox("09_185", DMCommon.Debug.GetListArray(miaCdBlocks))
			'  ReDim miaCdBlocks(msaParcelFolders.GetUpperBound(0))
			'For iIndex As Integer = 0 To msaParcelFolders.GetUpperBound(0)
			'   miaCdBlocks(iIndex) = Integer.Parse(msaParcelFolders(iIndex))
			'Next
		End If



		' If hsGush IsNot Nothing Then
		oPolygonSet.SetDataInsertBlock(hsGush, dicGushFromParcelData, tShapeFOData, tFilterList, bInnerPolygons)
		mdicGushFromParcelData = oPolygonSet.GushFromParcelData
		'   DMCommon.Debug.MsgBox("12_700!!!!", DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData), DMCommon.Debug.ColCount(mdicGushFromParcelData))
		'dicGushFromParcelData
		' End If
		If msaParcelFolders IsNot Nothing Then
			'!!!!!!!!!!!!!!!!!!!!! 26/07/2018 main proc.
			'	DMCommon.Debug.MsgBox("12_701b", msaParcelFolders)
			ConnectCdBlocks() '''''''12/09/18''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		End If

		'	zzSetDataInsertBlock(sShapeFoldername, sFileName)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		mdicMPgonColByGush = oPolygonSet.GetMPgonColByGush()

		msShapeFolderName = sShapeFolderName
	End Sub

	Private Sub zzSet()

   End Sub
   Public Sub ExportToShapeByGush()
      Dim oShapeExpImp As ShapeExpImp
      Dim sShapeFileName As String = ""
      Dim oShapeFolder As IO.DirectoryInfo = New IO.DirectoryInfo(msShapeFolderName)
      Dim colGush As System.Collections.Generic.ICollection(Of Integer)
      Dim colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim sGush As String
      Dim iGushIndex As Integer
      'For Each colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection In moMPgonColByGush.Values
      If mdicMPgonColByGush IsNot Nothing Then
         colGush = mdicMPgonColByGush.Keys()
         For Each iGush As Integer In colGush
            If iGushIndex >= 90 Then
               colMPgonObjIDs = mdicMPgonColByGush.Item(iGush)
               sGush = CStr(iGush) & ""
               '	TODO(yygyhy)

               oShapeFolder.CreateSubdirectory(sGush)
               sShapeFileName = msShapeFolderName & "\" & sGush & "\p" & sGush & ".shp"

               oShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
               DMAcadExt.AcadDocument.WriteMessage(CStr(iGush) & ": " & CStr(colMPgonObjIDs.Count))

               oShapeExpImp.FromPoligons(colMPgonObjIDs)
               '	oShapeExpImp.AddShapeData() For Future
               Try
                  oShapeExpImp.Exec()
               Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
                  System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_919")
               End Try

               Try
                  ConnectToShape("shp" & sGush, sShapeFileName)
                  AddLayerToMap("shp" & sGush)

               Catch oMapEx As Autodesk.Gis.Map.MapException
                  System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & sShapeFileName, "01_819g")
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
               End Try
            End If
            iGushIndex += 1

         Next


      End If



   End Sub
   Public Sub ExportToShapeByGushAfter()
      Dim sShapeFileName As String = ""
      Dim oShapeFolder As IO.DirectoryInfo = New IO.DirectoryInfo(msShapeFolderName)
      Dim colGush As System.Collections.Generic.ICollection(Of Integer)
      Dim colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim sGush As String
      'For Each colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection In moMPgonColByGush.Values
      If mdicMPgonColByGush IsNot Nothing Then
         colGush = mdicMPgonColByGush.Keys()
         For Each iGush As Integer In colGush

            If False And iGush = 7920 Then
               Exit For
            End If

            colMPgonObjIDs = mdicMPgonColByGush.Item(iGush)
            sGush = CStr(iGush) & ""

            sShapeFileName = msShapeFolderName & "\" & sGush & "\p" & sGush & ".shp"
            DMAcadExt.AcadDocument.WriteMessage("53: " & sShapeFileName)

            Try
               ConnectToShape("shp" & sGush, sShapeFileName)
               AddLayerToMap("shp" & sGush)

            Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & sShapeFileName, "01_819g")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
            End Try
            Exit For
         Next
      End If
   End Sub
   Public Sub ConnectAddToMap()
      Dim iUB As Integer = mtaShapeFOData.GetUpperBound(0)
      For iIndex As Integer = 0 To iUB
         If mtaShapeFOData(iIndex).FileExists Then
            Try
               ConnectToShape(mtaShapeFOData(iIndex).ConnectionName, mtaShapeFOData(iIndex).ShapeFileName)
               AddLayerToMap(mtaShapeFOData(iIndex).ConnectionName)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
            End Try
         Else
            System.Windows.Forms.MessageBox.Show(mtaShapeFOData(iIndex).ShapeFileName, "01_843 Not Exists")
         End If
      Next
   End Sub
   Public Sub ConnectCdBlocks() '!!!New 160817
      'System.Windows.Forms.MessageBox.Show(CStr(miPoligonCount) & ":" & "imp_Parcels", "03_007s")
      Const sNamePrefix As String = "p"

		Dim iUB As Integer = msaParcelFolders.GetUpperBound(0)
		'    Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
		Dim sLayerName As String
		Dim tColor As System.Drawing.Color = New Drawing.Color()
		ReDim mtaShapeFOData(iUB)
      ReDim moaBoundingBoxes(iUB)

      For iIndex As Integer = 0 To iUB
         mtaShapeFOData(iIndex) = New ShapeFOData(msRootPath, msaParcelFolders(iIndex), sNamePrefix & msaParcelFolders(iIndex))
         '    DMCommon.Debug.MsgBox("09_339", mtaShapeFOData(iIndex).Gush.Block)

         mtaShapeFOData(iIndex) = New ShapeFOData(msRootPath, msaParcelFolders(iIndex), "p" & msaParcelFolders(iIndex))
			'	mtaShapeFOData(iIndex).Gush = New GushData(iIndex, miaCdBlocks(iIndex), 0, 80, 866, 0.0)
			mtaShapeFOData(iIndex).Gush = mtaGushData(iIndex)
			'  DMCommon.Debug.MsgBox("09_340a", msaParcelFolders(iIndex), mtaShapeFOData(iIndex).Gush.Block, mtaShapeFOData(iIndex).Gush.LegalArea, mtaShapeFOData(iIndex).Gush.IsAnality, mtaShapeFOData(iIndex).Gush.Status, mtaShapeFOData(iIndex).ShapeFileName)
			' DMCommon.ExcelLogY.SetNextValue(10, iIndex, msaParcelFolders(iIndex))
		Next
		'  xxxxxxxxx()
		'ZDES Bil 29/07
		'Dim oPolygonSet As TplnPolygonSet '= New TplnPolygonSet(True)

		For iIndex As Integer = 0 To iUB
         If mtaShapeFOData(iIndex).FileExists Then

            '	System.Windows.Forms.MessageBox.Show(CStr(miPoligonCount) & ":" & CStr(oPolygonSet.PgonImported), "03_007s")
            Try
					ConnectToShape(mtaShapeFOData(iIndex).ConnectionName, mtaShapeFOData(iIndex).ShapeFileName)
					'	DMCommon.Debug.MsgBox("09_339K", mtaShapeFOData(iIndex).ConnectionName, mtaShapeFOData(iIndex).ShapeFileName, mtaShapeFOData(iIndex).Gush.Block)
					'AddLayerToMap(mtaShapeFOData(iIndex).ConnectionName)
					sLayerName = "p" & msaParcelFolders(iIndex)
					zzCreateMapLayersNew(mtaShapeFOData(iIndex).ShapeFileName, sLayerName, tColor)

				Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ":  ")
            End Try


            '	Dim dicPolygons As System.Collections.Generic.IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Autodesk.AutoCAD.DatabaseServices.Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylinesDic(mtaShapeFOData(iIndex).FeatureClass, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)	'"pclp004"

            '  Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtaShapeFOData(iIndex).FeatureClass)


            '     mtaShapeFOData(iIndex).SumMPgonArea = oPolygonSet.SumMPgonArea
            '     mtaShapeFOData(iIndex).Gush = oPolygonSet.ShapeData.Gush

         Else
            System.Windows.Forms.MessageBox.Show(mtaShapeFOData(iIndex).ShapeFileName, "01_843 Not Exists")
         End If
      Next

		'zzCreateBoundingBoxMapLayer()
		CreateBoundingBoxesMapLayer()



	End Sub
	Public Sub imp_Parcels() '!!!Actual 17/07/2018
		'System.Windows.Forms.MessageBox.Show(CStr(miPoligonCount) & ":" & "imp_Parcels", "03_007s")
		Const sNamePrefix As String = "p"
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		Dim iUB As Integer = msaParcelFolders.GetUpperBound(0)
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)

		ReDim mtaShapeFOData(iUB)
		ReDim moaBoundingBoxes(iUB)

		For iIndex As Integer = 0 To iUB
			mtaShapeFOData(iIndex) = New ShapeFOData(msRootPath, msaParcelFolders(iIndex), sNamePrefix & msaParcelFolders(iIndex))
			If Not mtaShapeFOData(iIndex).FileExists Then
				mtaShapeFOData(iIndex) = New ShapeFOData(msRootPath, msaParcelFolders(iIndex), msaParcelFolders(iIndex))
			End If
		Next

		'zzCreateBoundingBoxMapLayer()
		'CreateBoundingBoxesMapLayer()


		Dim oPolygonSet As TplnPolygonSet '= New TplnPolygonSet(True)

		For iIndex As Integer = 0 To iUB
			If mtaShapeFOData(iIndex).FileExists Then
				oPolygonSet = New TplnPolygonSet(mtaShapeFOData(iIndex), True)

				oPolygonSet.Import(mtaShapeFOData(iIndex).ShapeFileName)
				miPoligonCount += oPolygonSet.PgonImported
				'	System.Windows.Forms.MessageBox.Show(CStr(miPoligonCount) & ":" & CStr(oPolygonSet.PgonImported), "03_007s")
				Try
					ConnectToShape(mtaShapeFOData(iIndex).ConnectionName, mtaShapeFOData(iIndex).ShapeFileName)
					'AddLayerToMap(mtaShapeFOData(iIndex).ConnectionName)
					zzCreateMapLayers(mtaShapeFOData(iIndex).ShapeFileName, msaParcelFolders(iIndex))
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
				End Try



				'	oPolygonSet = New TplnPolygonSet(mtaShapeFOData(iIndex), True)
				oPolygonSet.LoadParcelData()

				'	Dim dicPolygons As System.Collections.Generic.IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Autodesk.AutoCAD.DatabaseServices.Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylinesDic(mtaShapeFOData(iIndex).FeatureClass, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)	'"pclp004"

				Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtaShapeFOData(iIndex).FeatureClass)
				'   DMCommon.Debug.MsgBox("01_923", mtaShapeFOData(iIndex).FeatureClass, colPolygonIDs.Count)
				If colPolygonIDs Is Nothing Then
					System.Windows.Forms.MessageBox.Show("Nothing" & CStr(iIndex) & ":" & CStr(iUB), "03_548a")
					colPolygonIDs = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
				End If
				'	oPolygonSet.AddPolylines(dicPolygons)
				If False Then
					oPolygonSet.AddPolylineIDs(colPolygonIDs)
					oPolygonSet.SetFeatureIDMin()
					oPolygonSet.CreateCentroids()
					'		System.Windows.Forms.MessageBox.Show(CStr(0) & vbCrLf & "", "04_400")
					oPolygonSet.CreateMPolygons()
				Else
					oPolygonSet.ImportParcels(colPolygonIDs)
				End If
				mtaShapeFOData(iIndex).SumMPgonArea = oPolygonSet.SumMPgonArea
				mtaShapeFOData(iIndex).Gush = oPolygonSet.ShapeData.Gush
				'17/01/18
			Else
				System.Windows.Forms.MessageBox.Show(mtaShapeFOData(iIndex).ShapeFileName, "01_843 Not Exists")
			End If
		Next

		CreateBoundingBoxesMapLayer()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Public Sub CreateBlocksUnion(sBlockName As String, sBlockLayer As String)
		'System.Windows.Forms.MessageBox.Show(" CreateBlocksUnion_A" & vbCrLf & sBlockName & vbCrLf & sBlockLayer & vbCrLf & CStr(mtaShapeFOData IsNot Nothing), "01_295q")
		If mtaShapeFOData IsNot Nothing Then
			'!!!!
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         '	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

         '	Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         Dim sBoundingTopoName As String = ShapeFOData.GetBoundingBoxWorkAreaTopoName()
         mcolDisPlines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			'DMCommon.Debug.MsgBox("13_029a", mtaShapeFOData.GetUpperBound(0))
			'!!!!!!!!!!!!!!!!!!!!! 26/07/2018 main proc.
			For iBlockIndex As Integer = 0 To mtaShapeFOData.GetUpperBound(0)
            If mtaShapeFOData(iBlockIndex).FileExists Then
               'zzEraseImport(sBoundingTopoName, iBlockIndex, sBlockName, sBlockLayer)
               '  System.Windows.Forms.MessageBox.Show(iBlockIndex.ToString() & vbCrLf & "CreateBlocksUnion_X" & vbCrLf & sBoundingTopoName & vbCrLf & sBlockName & vbCrLf & sBlockLayer, "02_471a")
               zzClipImport(sBoundingTopoName, iBlockIndex, sBlockName, sBlockLayer, True)
            End If
         Next
         CreateDissolveMapLayer(mcolDisPlines, "WorkArea", "shpWorkArea")
         '	System.Windows.Forms.MessageBox.Show("  CreateBlocksUnion" & vbCrLf & sBlockName, "02_471")
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()

      End If
   End Sub
   Public Sub CreateBlocks(sBlockName As String, sBlockLayer As String)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      '	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      '	Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

      For iBlockIndex As Integer = 0 To mtaShapeFOData.GetUpperBound(0)
         If mtaShapeFOData(iBlockIndex).FileExists Then
            '''''''''''zzEraseImport(mtaShapeFOData(iBlockIndex).GetBoundingBoxTopoName(), iBlockIndex, sBlockName, sBlockLayer)
            zzClipImport(mtaShapeFOData(iBlockIndex).GetBoundingBoxTopoName(), iBlockIndex, sBlockName, sBlockLayer, True)
            '	colPolylines = DMAcadExt.AcadTransaction.GetEntitiesByLayer(mtaShapeFOData(iBlockIndex).FeatureClass)
            '	zzInsertBlock(colPolylines, "1601", "1601")
         End If
      Next

      If False Then   '300613
         Dim sBoundingTopoName As String
         For iBlockIndex As Integer = 0 To mtaShapeFOData.GetUpperBound(0)
            If mtaShapeFOData(iBlockIndex).FileExists Then

               RemoveConnectionB(mtaShapeFOData(iBlockIndex).ConnectionName)
               sBoundingTopoName = "shp" & mtaShapeFOData(iBlockIndex).GetBoundingBoxTopoName()
               RemoveConnectionB(sBoundingTopoName)

               Util.ClearResource(mtaShapeFOData(iBlockIndex).ConnectionName, mtaShapeFOData(iBlockIndex).FeatureClass)
               Util.ClearResource(sBoundingTopoName, mtaShapeFOData(iBlockIndex).GetBoundingBoxTopoName)

            End If
         Next
      End If

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Public Sub CreateWorkArea()
      Dim sBoundingTopoName As String = ShapeFOData.GetBoundingBoxWorkAreaTopoName()
      '		Dim sOutputFCName As String = "WorkArea_Boundary"

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      Dim sOutputFile As String = DMCommon.Functions.GetLocalFileNameInDir(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer, ".shp")
		'	System.Windows.Forms.MessageBox.Show(sOutputFile & vbCrLf & sParcelsLayer, "01_740")
		'	DMAcadExt.AcadDocument.WriteMessage(sOutputFile)
		Try

			'   System.Windows.Forms.MessageBox.Show(sBoundingTopoName & vbCrLf & "WorkArea" & vbCrLf & sOutputFile & vbCrLf & DMAcadExt.MapThemeData.WorkAreaBoundaryLayer, "01_637Clip")
			DMCommon.Debug.MsgBox("01_637Clip", sBoundingTopoName, DMAcadExt.MapThemeData.WorkAreaBoundaryLayer, sOutputFile)
			If FDO_Manager.LayerPairExist(sBoundingTopoName, "WorkArea") Then
				FeatureEditor.Overlay(sBoundingTopoName, "WorkArea", "Clip", sOutputFile, DMAcadExt.MapThemeData.WorkAreaBoundaryLayer, 2, 0.001, 0.001, 0.001)
				'FeatureEditor.Overlay(sBoundingTopoName, "WorkArea", "Clip", sOutputFile, DMAcadExt.MapThemeData.WorkAreaBoundaryLayer, 2, 0.05, 0.05, 0.05)

			End If
		Catch oMapEx As Autodesk.Gis.Map.MapException
			System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sBoundingTopoName & vbCrLf & "WorkArea", "01_766")
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         '	FDO_Manager.vb:line 2367
      End Try

      zzImportA(sOutputFile)


      '''''''''''	zzClipWorkAreaCalculate(sOutputFCName)
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   '100685-100688                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        

   Public Sub CreateBoundingBoxMapLayer(sShapeFileName As String)
      Dim oPolyLine As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim sResFileName As String
      '	Dim iUB As Integer = msaParcelFolders.GetUpperBound(0)
      Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
      Dim sBoundingTopoName As String = ShapeFOData.GetBoundingBoxWorkAreaTopoName
      sResFileName = DMCommon.Functions.GetLocalFileNameInDir(sBoundingTopoName, ".shp")
      System.Windows.Forms.MessageBox.Show(sResFileName & vbCrLf & sShapeFileName, "04_015")
      Try
         oBoundingBox = zzGetExtent(sShapeFileName)
         DMCommon.Debug.MsgBox("05_397!!!", oBoundingBox.MinPoint, oBoundingBox.MaxPoint)
         If oBoundingBox IsNot Nothing Then


            '	System.Windows.Forms.MessageBox.Show(oBoundingBox.Coordinates & vbCrLf & "!!!", "04_018")
            oBoundingBox.Buffer(20.0)
            '	System.Windows.Forms.MessageBox.Show(oBoundingBox.Coordinates & vbCrLf & "!!!", "04_022")
            oPolyLine = oBoundingBox.GetClosedPolyline()
            '	System.Windows.Forms.MessageBox.Show(oPolyLine.ToString() & vbCrLf & sBoundingTopoName, "04_025")
            ''''''''''''''''''''''	zzCreateTopo(sBoundingTopoName, oPolyLine)

            DMAcadExt.AcadDocument.WriteMessage("19Before_ " & sBoundingTopoName)
            'zzExportBoundingPolygonByTopo(sBoundingTopoName, sResFileName)
            '	Dim sResFileName As String = DMCommon.Functions.GetLocalFileNameInDir(sBoundingTopoName, ".shp")
            Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Export, sResFileName)
            Dim colDisPlines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
            colDisPlines.Add(oPolyLine.ObjectId)
            oShapeExpImp.FromPoligons(colDisPlines)

            Try
               oShapeExpImp.Exec()
            Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException

					System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_918")
            End Try
         End If
         DMAcadExt.AcadDocument.WriteMessage("21After_ " & sBoundingTopoName)
         '''''''''''''''	TopoManager.TopoCreator.DeleteTopology(sBoundingTopoName, True, False)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "01_849b")
      End Try



      Try
         ConnectToShape("shp" & sBoundingTopoName, sResFileName)
         AddLayerToMap("shp" & sBoundingTopoName)

      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
      End Try

   End Sub

   Public Sub CreateBoundingBoxesMapLayer()
      Dim sBoundingTopoName As String
      Dim oPolyLine As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim sShapeFile As String
      Dim sResFileName As String
      Dim iUB As Integer = mtaShapeFOData.GetUpperBound(0)
		Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
		moUnionBoundingBox = New DMAcadExt.TPlnBoundingBox()
      sBoundingTopoName = ShapeFOData.GetBoundingBoxWorkAreaTopoName
      sResFileName = DMCommon.Functions.GetLocalFileNameInDir(sBoundingTopoName, ".shp")


      '   DMCommon.Debug.MsgBox("12_429a", iUB)
      For iIndex As Integer = 0 To iUB
			sShapeFile = mtaShapeFOData(iIndex).ShapeFileName
			oBoundingBox = zzGetExtent(sShapeFile)
			If oBoundingBox IsNot Nothing Then
				moUnionBoundingBox.Union(oBoundingBox)
			End If

		Next
      '     DMCommon.Debug.MsgBox("12_429b", iUB)
      Try

			moUnionBoundingBox.Buffer(100.0)
			oPolyLine = moUnionBoundingBox.GetClosedPolyline()
			If False Then
				zzCreateTopo(sBoundingTopoName, oPolyLine)
				zzExportBoundingPolygonByTopo(sBoundingTopoName, sResFileName)
				TopoManager.TopoCreator.DeleteTopology(sBoundingTopoName, True, False)
			End If
			zzExportBoundingPolygon(oPolyLine, sResFileName)
			oPolyLine.Erase()


			'	DMAcadExt.AcadDocument.WriteMessage("211After_ " & sBoundingTopoName)

		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sResFileName, "01_851b")
      End Try
      '    DMCommon.Debug.MsgBox("12_429c", iUB)

      Try
         ConnectToShape("shp" & sBoundingTopoName, sResFileName)
			'	AddLayerToMap("shp" & sBoundingTopoName)
			zzCreateBoundBoxWorkAreaMapLayers("")
			'ttttttttttttttttttttttt26/07
		Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
      End Try


   End Sub
   Public ReadOnly Property PoligonCount As Integer
      Get
         Return miPoligonCount
      End Get
   End Property

   Public Sub Test()
      '	Dim sShapeFileName As String = "C:\Users\boris\AppData\Local\Autodesk, Inc\AutoCAD\R18.2.51.0.0\LotKParcel\LotKParcel.shp"
      Dim sShapeFileName_1 As String = "\\NETAPP\DM_APP\Tababuild\ProjectsNet\Shape\shp\p2792.shp"
      Dim sShapeFileName_2 As String = "\\NETAPP\DM_APP\Tababuild\ProjectsNet\Shape\shp\p2793.shp"

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ''''''''''	zzImport(sShapeFileName_1, "testImp1")
      '''''''''''''	zzImport(sShapeFileName_2, "testImp2")
      Try
         ConnectToShape("shp1", sShapeFileName_1)
         AddLayerToMap("shp1")

      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
      End Try
      If False Then
         Try
            ConnectToShape("shp1277", sShapeFileName_2)
            AddLayerToMap("shp1277")

         Catch oMapEx As Autodesk.Gis.Map.MapException
            System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
         End Try
      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      '	Return

   End Sub
   Public Sub TestA()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      If True Then
         Dim sSHPFileName_1 As String = "D:\Outline\OutlineErase1.shp"
         Dim sFCName_1 As String = "OutlineErase1"
         Dim sSHPFileName_2 As String = "D:\Outline\OutlineErase2.shp"
         Dim sFCName_2 As String = "OutlineErase2"


         FeatureEditor.Overlay("Outline", "p2792", "Erase", sSHPFileName_1, sFCName_1, 2, 0.5, 0.5, 0.5)
         zzImportA(sSHPFileName_1)

         FeatureEditor.Overlay("Outline", "p2793", "Erase", sSHPFileName_2, sFCName_2, 2, 0.5, 0.5, 0.5)
         zzImportA(sSHPFileName_2)
      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Sub TestB()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      Dim sSHPFileName As String = "D:\Outline\OutlineErase.shp"
      Dim sFCName As String = "OutlineErase"


      FeatureEditor.Overlay("Outline", "p2792", "Erase", sSHPFileName, sFCName, 2, 0.5, 0.5, 0.5)
      zzImportA(sSHPFileName)
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Shared Sub RemoveResource(sConnection As String, sLayerName As String)
      Util.ClearResource(sConnection, sLayerName)

   End Sub
   Public Shared Sub RemoveAllResources()
      Util.ClearAllResources()
   End Sub
   Public Shared Function LayerPairExist(sMapLayerNameA As String, sMapLayerNameB As String) As Boolean
      Dim colLayers As OSGeo.MapGuide.MgLayerCollection
      Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap
      Try
         oMap = AcMapMap.GetCurrentMap()
      Catch oEx As Exception
         oMap = Nothing
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "FDO_Manager - LayerExists")
      End Try
      If oMap IsNot Nothing Then
         Try
            colLayers = oMap.GetLayers()
            '	System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & CStr(colLayers.Count) & vbCrLf & "", "08_501")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "FDO_Manager - LayerExists_1")
            colLayers = Nothing
         End Try
         If colLayers IsNot Nothing Then
            If colLayers.Count > 0 Then
					Try
						If colLayers.Contains(sMapLayerNameA) AndAlso colLayers.Contains(sMapLayerNameB) Then
							Return True
						Else
							DMCommon.Debug.MsgBox("13_042", colLayers.Count, sMapLayerNameA, sMapLayerNameB)
							Return False
						End If

					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString() & vbCrLf & CStr(colLayers.Count) & vbCrLf & "'" & sMapLayerNameA & "'" & vbCrLf & "'" & sMapLayerNameB & "'", "FDO_Manager - LayerExists_2")
                  Return False
               End Try
            Else
               Return False
            End If
         Else
            Return False
         End If
      Else
         Return False
      End If
   End Function
   Public Shared Function LayerExists(sMapLayerName As String) As Boolean
      Dim colLayers As OSGeo.MapGuide.MgLayerCollection
      Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap
      Try
         oMap = AcMapMap.GetCurrentMap()
      Catch oEx As Exception
         oMap = Nothing
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "FDO_Manager - LayerExists")
      End Try
      If oMap IsNot Nothing Then
         Try
            colLayers = oMap.GetLayers()
            '	System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & CStr(colLayers.Count) & vbCrLf & "", "08_501")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "FDO_Manager - LayerExists_1")
            colLayers = Nothing
         End Try
         If colLayers IsNot Nothing Then
            If colLayers.Count > 0 Then
               Try
                  Return colLayers.Contains(sMapLayerName)
               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString() & vbCrLf & CStr(colLayers.Count) & vbCrLf & "'" & sMapLayerName & "'", "FDO_Manager - LayerExists_2")
                  Return False
               End Try
            Else
               Return False
            End If
         Else
            Return False
         End If
      Else
         Return False
      End If
   End Function
   Public Shared Sub RemoveFirstLayer()
      Dim colLayers As OSGeo.MapGuide.MgLayerCollection
      '	Dim a As OSGeo.MapGuide.MgUnclassifiedException
      colLayers = AcMapMap.GetCurrentMap().GetLayers()
      If colLayers.Count > 0 Then
         colLayers.RemoveAt(0)
      End If
   End Sub
   Public Shared Sub RemoveLayer(sLayerName As String)
      Dim colLayers As OSGeo.MapGuide.MgLayerCollection
      Dim oMgLayerBase As OSGeo.MapGuide.MgLayerBase

      colLayers = AcMapMap.GetCurrentMap().GetLayers()
      If colLayers Is Nothing Then
         System.Windows.Forms.MessageBox.Show("colLayers Is Nothing ", "01_470")

      Else
         If colLayers.Contains(sLayerName) Then
            oMgLayerBase = colLayers.GetItem(sLayerName)
            System.Windows.Forms.MessageBox.Show(CStr(colLayers.Count), "01_498before")
            colLayers.Remove(oMgLayerBase)
            oMgLayerBase.Dispose()
            System.Windows.Forms.MessageBox.Show(CStr(colLayers.Count), "01_499after")
         End If

      End If
   End Sub


   Private Sub zzEraseImportAAA(sBoundingBoxLayer As String, iBlockIndex As Integer, sBlockName As String, sBlocklayer As String)
      Dim sOutputFCName As String = mtaShapeFOData(iBlockIndex).FeatureClass & "_Boundary"
      Dim sParcelsLayer As String = mtaShapeFOData(iBlockIndex).FeatureClass
      Dim sOutputFile As String = DMCommon.Functions.GetLocalFileNameInDir(sOutputFCName, ".shp")
      '	System.Windows.Forms.MessageBox.Show(sOutputFile & vbCrLf & sParcelsLayer, "01_740")
      '	DMAcadExt.AcadDocument.WriteMessage(sOutputFile)
      Try
         System.Windows.Forms.MessageBox.Show(sBoundingBoxLayer & vbCrLf & sParcelsLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName, "01_671")
         '''''''''''''''''		FeatureEditor.Overlay(sBoundingBoxLayer, sParcelsLayer, "Erase", sOutputFile, sOutputFCName, 2, 0.5, 0.5, 0.5)
         '	System.Windows.Forms.MessageBox.Show("After", "01_671AA")
      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sBoundingBoxLayer & vbCrLf & sParcelsLayer, "01_757")
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         Return
      End Try

      ''''''''''''''''''''''''	zzImportA(sOutputFile)
      zzEraseBoundingBox(sOutputFCName, iBlockIndex, sBlockName, sBlocklayer)
   End Sub

	Private Sub zzClipImport(sBoundingBoxLayer As String, iBlockIndex As Integer, sBlockName As String, sBlockLayer As String, bCalc As Boolean)
		Dim sParcelsLayer As String = mtaShapeFOData(iBlockIndex).FeatureClass
		Dim oFileInfo As IO.FileInfo

		Dim sOutputFCName As String
		Dim sOutputFile As String

		'	
		'	System.Windows.Forms.MessageBox.Show(sOutputFCName & vbCrLf & sOutputFile & vbCrLf & sParcelsLayer, "01_744v")
		'	DMAcadExt.AcadDocument.WriteMessage(sOutputFile)
		Dim iIndex As Integer = 0
		Do
			sOutputFCName = mtaShapeFOData(iBlockIndex).FeatureClass & "_Boundary"
			sOutputFile = DMCommon.Functions.GetLocalFileNameInDir(sOutputFCName, ".shp", iIndex)
			sOutputFile = DMCommon.Functions.GetLocalFileNameInEmptyDir(sOutputFCName, ".shp")
			oFileInfo = New IO.FileInfo(sOutputFile)
			'   System.Windows.Forms.MessageBox.Show(sOutputFile & vbCrLf & sParcelsLayer, "01_740s")
			'	DMCommon.Debug.MsgBox("13_029ba", sOutputFCName, sOutputFile)
			Try
				If oFileInfo.Exists Then
					oFileInfo.Delete()
				End If

			Catch oEx As Exception
				'  System.IO.IOException 
				'  System.Security.SecurityException:
				'  System.UnauthorizedAccessException
				If oEx.GetType().ToString() <> "System.IO.IOException" Then
					System.Windows.Forms.MessageBox.Show(oEx.GetType().ToString() & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "zzClipImport 16")
				End If

			End Try
			iIndex += 1
		Loop While oFileInfo.Exists AndAlso iIndex < 100
		'    If iIndex < 100 Then

		Try
			' System.Windows.Forms.MessageBox.Show("iIndex=" & iIndex.ToString() & vbCrLf & sBoundingBoxLayer & vbCrLf & sParcelsLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName & vbCrLf & "bCalc=" & bCalc.ToString(), "01_655Clip")
			'	If sBoundingBoxLayer EXISTS
			'DMCommon.Debug.MsgBox("13_029bF", sBoundingBoxLayer, sParcelsLayer, FDO_Manager.LayerPairExist(sBoundingBoxLayer, sParcelsLayer))
			If FDO_Manager.LayerPairExist(sBoundingBoxLayer, sParcelsLayer) Then
				'DMCommon.Debug.MsgBox("13_029bG", sBoundingBoxLayer, sParcelsLayer, "Clip", sOutputFile, sOutputFCName)
				'	FeatureEditor.Overlay(sBoundingBoxLayer, sParcelsLayer, "Clip", sOutputFile, sOutputFCName, 2, 0.5, 0.1, 0.5)
				FeatureEditor.Overlay(sBoundingBoxLayer, sParcelsLayer, "Clip", sOutputFile, sOutputFCName, 2, 0.001, 0.001, 0.5)


			End If
			'  Catch oMapEx As Autodesk.Gis.Map.MapException
		Catch oEx As Exception

			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sBoundingBoxLayer & vbCrLf & sParcelsLayer & vbCrLf & sOutputFile, "01_758")
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Return ''
		End Try

		'  System.Windows.Forms.MessageBox.Show(sOutputFile, "zzImportA_10")
		If True Then


			''''''''''''zzImportA(sOutputFile)
			zzImportNew(sOutputFile)
			If bCalc Then
				zzClipCalculate(sOutputFCName, iBlockIndex, sBlockName, sBlockLayer)
			End If
		End If
		''''''''''''''''	zzEraseBoundingBox(sOutputFCName, iBlockIndex, sBlockName, sBlocklayer)
		'   End If
	End Sub
	Public Function GetClipAcObjIds(sBaseMapLayer As String, sClipMapLayer As String) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim sOutputFCNameTemp As String = "PrcAnalitic"
      Dim sOutputFileTemp As String = DMCommon.Functions.GetLocalFileNameInDir(sOutputFCNameTemp, ".shp")
      System.Windows.Forms.MessageBox.Show(sOutputFileTemp & vbCrLf & sOutputFCNameTemp, "08_384Clip")
      FeatureEditor.Overlay(sBaseMapLayer, sClipMapLayer, "Clip", sOutputFileTemp, sOutputFCNameTemp, 3, 0.5, 0.5, 0.5)
      Dim connMgr As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(connMgr.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties

      Dim saNames() As String = oProperties.PropertyNames
      Dim sConnectionString As String = oConnection.ConnectionString

      Dim isRequired As Boolean
      Dim iTestCounter As Integer
      Dim colRes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      For iIndex As Integer = 0 To saNames.GetUpperBound(0)
         isRequired = oProperties.IsPropertyRequired(saNames(iIndex))
         'oEditor.WriteMessage("6_" & CStr(iIndex) & " !! " & CStr(isRequired) & " - " & saNames(iIndex) & vbCrLf)
      Next

      oProperties.SetProperty(sSHPPropFileName, sOutputFileTemp)

      Dim connState As Connections.ConnectionState = oConnection.Open()
      Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
      Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
      Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes
      Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
      Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)
      Dim sClassName As Expression.Identifier = New Expression.Identifier(sOutputFCNameTemp)

      oSelect.FeatureClassName = sClassName

      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = oSelect.Execute()

      Dim iGeo As Integer = 0
      Dim iValue As Integer
      Dim dValue As Double


      Dim sText As String

      Dim lHandle As Long
      Dim sHandle As String
      Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim tHandle As Autodesk.AutoCAD.DatabaseServices.Handle

      While oFeatureReader.ReadNext()
         oFeatureClass = oFeatureReader.GetClassDefinition
         iTestCounter += 1
         Try
            'sPropName = oFeatureReader.GetPropertyName(2)
            iValue = oFeatureReader.GetInt32(0)
            sText = CStr(iValue) & ":"
            dValue = oFeatureReader.GetDouble(1)
            sHandle = oFeatureReader.GetString(2)
            sText &= ":" & sHandle & ":" & CStr(dValue)

            'oEditor.WriteMessage("71-- " & ":" & sText & " : " & CStr(CInt(dValue)) & vbCrLf)

            lHandle = CLng("&H" & sHandle)
            tHandle = New Autodesk.AutoCAD.DatabaseServices.Handle(lHandle)
            tAcObjID = DMAcadExt.AcadTransaction.GetObjectID(tHandle)
            If Not tAcObjID.IsNull Then
               colRes.Add(tAcObjID)
            End If

         Catch oEx As Exception
            oEditor.WriteMessage("355err 1!! " & oEx.Message & ":" & vbCrLf)

         End Try

         'oEditor.WriteMessage("29-- " & CStr(oFeatureReader.GetDepth()) & ":" & featureClass.Name & vbCrLf)

         '	Exit While
      End While
      Return colRes
   End Function
   Public Sub ClipImportAll()
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim colGush As System.Collections.Generic.ICollection(Of Integer)
      '	Dim colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim sGush As String
      Dim sParcelsLayer As String
      Dim iGushIndex As Integer
		'For Each colMPgonObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection In moMPgonColByGush.Values
		'	System.Windows.Forms.MessageBox.Show(sShapeFileA & vbCrLf & "BB", "04_012")

		'	CreateBoundingBoxMapLayer(sShapeFile, "BB")
		Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim sa1601AttribText(1) As String
      Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
      Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon
      Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId

		If mdicMPgonColByGush IsNot Nothing Then
         colGush = mdicMPgonColByGush.Keys()
         For Each iGush As Integer In colGush
            If False And iGush = 7920 Then
               System.Windows.Forms.MessageBox.Show(CStr(iGushIndex), "04_882")
               Exit For
            End If
				If iGushIndex >= 0 Then
					sGush = CStr(iGush)
					sParcelsLayer = "p" & sGush
					zzClipImportA(sParcelsLayer, "1601", "1601")

					Dim sMsg As String = ""
					colPolylines = DMAcadExt.AcadTransaction.GetEntitiesByLayer(sParcelsLayer & "_Boundary")
					If colPolylines.Count > 1 Then
						oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()
						For iIndex As Integer = 0 To colPolylines.Count - 1
							tAcObjID = colPolylines(iIndex)

							oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
							If oEntity IsNot Nothing Then
								oEntity.Layer = "1601"
								oPolyline = DirectCast(oEntity, Autodesk.AutoCAD.DatabaseServices.Polyline)
								sMsg &= vbCrLf & tAcObjID.ToString() & ":" & CStr(oPolyline.Area)
								If oPolyline.Closed AndAlso oPolyline.Area >= 0.001 Then


									Try
										oMPolygon.AppendLoopFromBoundary(oPolyline, True, 0.001)
									Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
										System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sGush & ":" & CStr(colPolylines.Count) & vbCrLf & sMsg, "FDO_Manager - ClipImportAll")
									End Try
								End If
							End If

						Next
						Dim bPosit As Boolean = oMPolygon.Area > 0.0

						For iIndex As Integer = 0 To oMPolygon.NumMPolygonLoops - 1
							If (bPosit AndAlso oMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Exterior) OrElse (Not bPosit AndAlso oMPolygon.GetLoopDirection(iIndex) = Autodesk.AutoCAD.DatabaseServices.LoopDirection.Interior) Then
								tAcObjID = colPolylines(iIndex)
							End If
						Next
						colPolylines.Clear()
						colPolylines.Add(tAcObjID)
					Else
						oEntity = DMAcadExt.AcadTransaction.GetEntity(colPolylines(0), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
						If oEntity IsNot Nothing Then
							oEntity.Layer = "1601"
						End If
					End If
					colCentroids.Clear()

					Try
						oMapUtility.CreateCentroids(colCentroids, colPolylines, "1601", "1601")
					Catch oMapEx As Autodesk.Gis.Map.MapException
						System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & vbCrLf & CStr(colPolylines.Count), "01_869")
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - zzEraseBoudingBox" & ": ")
					End Try

					DMCommon.Debug.MsgBox("02_444", CStr(colCentroids.Count))
					'	Dim tGushData As GushData
					Dim ia1601AttribIndices() As Integer = {0, 1}
					Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
					'System.Windows.Forms.MessageBox.Show(sParcelsLayer & "_Boundary" & vbCrLf & sGush & vbCrLf & CStr(colCentroids.Count) & vbCrLf & CStr(colPolylines.Count), "01_141a")
					For iIndex As Integer = 0 To colPolylines.Count - 1
						tCentroidAcObjID = colCentroids.Item(iIndex)
						tPgonAcObjID = colPolylines.Item(iIndex)

						oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
						If oBlockRef IsNot Nothing Then
							oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(10.0)
						End If
						'	sa1601AttribText(0) = CStr(mtaShapeFOData(iBlockIndex).Block)
						'	sa1601AttribText(1) = CStr(mtaShapeFOData(iBlockIndex).BlockAdd)
						sa1601AttribText(0) = sGush
						sa1601AttribText(1) = "0"
						'	DMCommon.Functions.DispArray(GushData.mia1601AttribIndices, "iaTopoIDs_325q")
						'	System.Windows.Forms.MessageBox.Show(CStr(iIndex) & vbCrLf & tCentroidAcObjID.ToString() & vbCrLf & DMCommon.Functions.DispArray(sa1601AttribText, "", False), "01_129z")
						DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, GushData.mia1601AttribIndices, sa1601AttribText)
						'	oEntity = DMAcadExt.AcadTransaction.GetEntity(tPgonAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
						'	oEntity.Layer = "1601"
					Next

				End If
				iGushIndex += 1

         Next
      End If






   End Sub
   Private Sub zzClipImportA(sParcelsLayer As String, sBlockName As String, sBlocklayer As String)
      Dim sBoundingBoxLayer As String = ShapeFOData.GetBoundingBoxWorkAreaTopoName()
      Dim oFile As IO.FileInfo
      Dim sOutputFCName As String = sParcelsLayer & "_Boundary"
      Dim sOutputFile As String = DMCommon.Functions.GetLocalFileNameInDir(sOutputFCName, ".shp")
		DMCommon.Debug.MsgBox("13_029D", sOutputFCName)
		sOutputFCName = sParcelsLayer & "_Boundary"
      sOutputFile = DMCommon.Functions.GetLocalFileNameInDir(sOutputFCName, ".shp")
      oFile = New IO.FileInfo(sOutputFile)
      Try
         If oFile.Exists Then
            oFile.Delete()
         End If

      Catch oEx As Exception
         '  System.IO.IOException 
         '  System.Security.SecurityException:
         '  System.UnauthorizedAccessException
         System.Windows.Forms.MessageBox.Show(oEx.GetType().ToString() & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "zzClipImportA 16 ")
      End Try

      '	System.Windows.Forms.MessageBox.Show(sBoundingBoxLayer & vbCrLf & sOutputFCName & vbCrLf & sOutputFile & vbCrLf & sParcelsLayer, "01_737a")
      '	DMAcadExt.AcadDocument.WriteMessage(sOutputFile)
      Try
         System.Windows.Forms.MessageBox.Show(sBoundingBoxLayer & vbCrLf & sParcelsLayer & vbCrLf & sOutputFile & vbCrLf & sOutputFCName, "01_644Clip")
         FeatureEditor.Overlay(sBoundingBoxLayer, sParcelsLayer, "Clip", sOutputFile, sOutputFCName, 2, 0.5, 0.5, 0.5)
         System.Windows.Forms.MessageBox.Show("AFTER!", "01_644AA")
      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sBoundingBoxLayer & vbCrLf & sParcelsLayer, "01_759d")
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         Return
      End Try
      Try
         If False Then
            ConnectToShape(sOutputFCName, sOutputFile)
            AddLayerToMap(sOutputFCName)
         End If

      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_848s")
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")


      End Try

      zzImportA(sOutputFile)


      ''''''''''''''''	zzEraseBoundingBox(sOutputFCName, iBlockIndex, sBlockName, sBlocklayer)

   End Sub

   Private Sub zzDrawFDORectangle(oBoundingBox As DMAcadExt.TPlnBoundingBox)
      Dim oMinPoint As DMAcadExt.TPlnPoint = oBoundingBox.MinPoint
      Dim oMaxPoint As DMAcadExt.TPlnPoint = oBoundingBox.MaxPoint


      Dim pos00 As DirectPositionImpl = New DirectPositionImpl(oMinPoint.X, oMinPoint.Y)

      Dim oConnectionManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(oConnectionManager.CreateConnection(SHPProviderName), Connections.IConnectionImp)
      Dim connInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo

      Dim oProperties As Connections.IConnectionPropertyDictionary = connInfo.ConnectionProperties
      Dim coordSys As String = Nothing
      Dim wellKnownText As String = Nothing
      Dim desc As String = Nothing
      '	Dim btaBYTE() As Byte
      Dim oGeometry As IGeometry = Nothing
      Dim geomFactory As FgfGeometryFactory = New FgfGeometryFactory()
      Dim sExtent As String = Nothing
      '	Dim extentType As SpatialContextExtentType

      Dim sSHPFileName As String = "D:\TestShape\TestShape.shp"

      Dim sSHPPropFileName As String = "DefaultFileLocation"

      '	oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      '	Dim connState As Connections.ConnectionState = oConnection.Open()
      geomFactory.CreateEnvelopeXY(oMinPoint.X, oMinPoint.Y, oMaxPoint.X, oMaxPoint.Y)


      Dim createDS As ICreateDataStore = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore), ICreateDataStore)

      Dim properties As IDataStorePropertyDictionary = createDS.DataStoreProperties

      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
      createDS.Execute()


      '	Return
      Dim listDS As IListDataStores = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_ListDataStores), IListDataStores)

      Dim reader As IDataStoreReader = listDS.Execute()

      Dim destroyDS As IDestroyDataStore = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DestroyDataStore), IDestroyDataStore)

      Dim propertiesA As IDataStorePropertyDictionary = destroyDS.DataStoreProperties

      '	oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
      destroyDS.Execute()

   End Sub
	Private Sub zzCreateDataStore()


		Dim oConnectionManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
		Dim oConnection As Connections.IConnectionImp = CType(oConnectionManager.CreateConnection(SHPProviderName), Connections.IConnectionImp)
		Dim connInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo

		Dim oProperties As Connections.IConnectionPropertyDictionary = connInfo.ConnectionProperties
		Dim coordSys As String = Nothing
		Dim wellKnownText As String = Nothing
		Dim desc As String = Nothing

		Dim oGeometry As IGeometry = Nothing
		Dim geomFactory As FgfGeometryFactory = New FgfGeometryFactory()
		Dim sExtent As String = Nothing


		Dim sSHPFileName As String = "D:\TestShape\TestShape.shp"

		Dim sSHPPropFileName As String = "DefaultFileLocation"

		'	oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

		'	Dim connState As Connections.ConnectionState = oConnection.Open()



		Dim createDS As ICreateDataStore = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore), ICreateDataStore)

		Dim properties As IDataStorePropertyDictionary = createDS.DataStoreProperties

		oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
		Try
			createDS.Execute()
		Catch ex As Exception

		End Try


	End Sub

	Private Class dmDataConnectUI
      Inherits AcMapDataConnectUI
      '	Private moConnectionAddControl As Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl
      Public Sub New()

      End Sub

      Public Function GetSHPPluginConnection() As Autodesk.Gis.Map.DataConnect.UI.SHPConnectionPlugin
         Return DirectCast(MyBase.LookupConnectionPlugin(SHPProviderName), SHPConnectionPlugin)
      End Function
      Public Function GetPluginConnection(ByVal sSHPProviderName As String) As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
         Return MyBase.LookupConnectionPlugin(sSHPProviderName)
      End Function
      Public Function GetPluginConfigureSource(ByVal sSHPProviderName As String) As Autodesk.Gis.Map.DataConnect.UI.GenericConfigureLayerPlugin

         Return DirectCast(MyBase.LookupConfigureSourcePlugin(SHPProviderName), GenericConfigureLayerPlugin)
      End Function
      Public Function GetPluginBrowseSchema() As GenericBrowseSchemaPlugin 'Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
         Return DirectCast(MyBase.LookupBrowseSchemaPlugin(SHPProviderName), GenericBrowseSchemaPlugin)
      End Function
      Public Sub RefreshSHRProviderAAA(ByVal oNode As System.Windows.Forms.TreeNode)
         If oNode IsNot Nothing Then
            MyBase.RefreshProvider(oNode)
         End If
      End Sub
   End Class

   Private Class dmSHPConnectionControl
      Inherits SHPConnectionControl
      Private moSHPConnectionPlugin As SHPConnectionPlugin
      Public Function SetPluginA() As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
         Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin = New SHPConnectionPlugin()
         MyBase.SetPlugin(oDataConnectPlugin)
         Return oDataConnectPlugin
      End Function
      Public Sub SetPluginAAA(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
         Try
            MyBase.SetPlugin(oDataConnectPlugin)
            moSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "331 SetPlugin ")
         End Try


      End Sub
      Public Sub Test()
         Dim sMsg As String = "Nothing"
			Dim plugin As System.Object = MyBase._plugin
			If plugin IsNot Nothing Then
            sMsg = plugin.ToString()
         End If
         System.Windows.Forms.MessageBox.Show(sMsg, "341 Test ")
         Dim oMsg As System.Windows.Forms.Message
         MyBase.ConnectionName = "SHP_2"
         System.Windows.Forms.MessageBox.Show(sMsg, "341 AA Test ")
         MyBase.SourceFileName = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
         System.Windows.Forms.MessageBox.Show(sMsg, "341 BB Test ")
         '''''''	moSHPConnectionPlugin.ConnectionName = "SHP_2"
         moSHPConnectionPlugin.SourceFileName = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
         System.Windows.Forms.MessageBox.Show("", "342 Test ")
         MyBase.ProcessCmdKey(oMsg, Windows.Forms.Keys.Enter)
         System.Windows.Forms.MessageBox.Show("", "343 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.LayerSourceName, "351 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.FeatureSourceName, "352 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ProviderName, "353 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.Configuration, "354 Test ")

         'moSHPConnectionPlugin.Attach(
         '''''''''''''	moSHPConnectionPlugin.Attach(msSHPProviderName, "SHP_2")


      End Sub
      Public Sub AttachAAA(oDataConnectUI As AcMapDataConnectUI)
         Dim sFeatureSource As String = moSHPConnectionPlugin.FeatureSourceName
         Dim sLayerSource As String = "topoLots"
         moSHPConnectionPlugin.Attach(oDataConnectUI, SHPProviderName, sFeatureSource, sLayerSource)

         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.LayerSourceName, "351 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.FeatureSourceName, "352 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ProviderName, "353 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.Configuration, "354 Test ")
         '			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.PluginControl.ToString(), "355 Test ")


      End Sub
   End Class
   Private Class dmConnectionAddControl_AAA
      Inherits Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl
      Public Sub New(oControl As Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl)

      End Sub
   End Class
   Private Class dmGenericBrowseSchemaControl
      Inherits GenericBrowseSchemaControl
      Private moGenericBrowseSchemaPlugin As GenericBrowseSchemaPlugin
      Private moSHPConnectionPlugin As SHPConnectionPlugin
      Private moSchemaClassTreeView As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeView
      Public Sub SetPluginC(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
         MyBase.SetPlugin(oDataConnectPlugin)
      End Sub
      Public Sub SetPluginA(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
         Dim sMsg As String = "Nothing"
         Try

            '	MyBase._strConnectionName = "SHP_2"	'"topLots"
            MyBase.SetPlugin(oDataConnectPlugin)
            moSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
            '	System.Windows.Forms.MessageBox.Show(moGenericConfigureLayerPlugin.ToString(), "377 SetPluginB ")
            MyBase._plugin = oDataConnectPlugin
            moSHPConnectionPlugin.Disconnect()

				'	MyBase.CoordinateSystemColumnVisible = True
				Dim plugin As System.Object = MyBase._plugin
				If plugin IsNot Nothing Then
               sMsg = plugin.ToString()
            End If


         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "379 SetPlugin ")
         End Try
      End Sub
		Public Function SetPluginB(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin) As OSGeo.FDO.Connections.ConnectionState
			Dim sMsg As String = "Nothing"
			Try

				'	MyBase._strConnectionName = "SHP_2"	'"topLots"
				MyBase.SetPlugin(oDataConnectPlugin)
				moGenericBrowseSchemaPlugin = DirectCast(oDataConnectPlugin, GenericBrowseSchemaPlugin)
				'	System.Windows.Forms.MessageBox.Show(moGenericConfigureLayerPlugin.ToString(), "377 SetPluginB ")
				MyBase._plugin = oDataConnectPlugin
				Return moGenericBrowseSchemaPlugin.Connect()



			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "335 SetPlugin ")
				Return OSGeo.FDO.Connections.ConnectionState.ConnectionState_Closed
			End Try
		End Function
		Public Sub AttachB(oDataConnectUI As AcMapDataConnectUI)
         Dim sFeatureSource As String = moGenericBrowseSchemaPlugin.FeatureSourceName
			Dim sLayerSource As String = "ParcelsLine"
			moGenericBrowseSchemaPlugin.Attach(oDataConnectUI, SHPProviderName, sFeatureSource, sLayerSource)
         '	MyBase._strMapCoordSystem



      End Sub
      Public Sub AttachA(oDataConnectUI As AcMapDataConnectUI)
         Dim sFeatureSource As String = moSHPConnectionPlugin.FeatureSourceName
         Dim sLayerSource As String = "topoParcels"
         moSHPConnectionPlugin.Attach(oDataConnectUI, SHPProviderName, sFeatureSource, sLayerSource)
         '	MyBase._strMapCoordSystem


      End Sub
      Public Sub DisconnectA()

         moSHPConnectionPlugin.Disconnect()
         '	MyBase._strMapCoordSystem


      End Sub
      Public Sub ABX()
         Dim oSchemaTreeViewItem As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem


         Dim oFeatureClassTreeViewItem As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem

         moSchemaClassTreeView = MyBase._schemaClassTreeView
         oSchemaTreeViewItem = New SchemaClassTreeViewItem("Default", SchemaClassTreeViewItem.ItemType.Schema)
         moSchemaClassTreeView.AddSchema(oSchemaTreeViewItem)

         oFeatureClassTreeViewItem = New SchemaClassTreeViewItem("topoParcels", SchemaClassTreeViewItem.ItemType.Class)
         moSchemaClassTreeView.AddFeatureClass(oSchemaTreeViewItem, oFeatureClassTreeViewItem)
      End Sub
      Public Sub ABY()
         moSchemaClassTreeView = MyBase._schemaClassTreeView
         moSchemaClassTreeView.Clear()
         '	moSchemaClassTreeView.Remov
      End Sub
      Public Sub SetView()
         MyBase.CreateAndAddListViewSchema()
         Dim oListViewEx As Autodesk.Gis.Map.DataConnect.UI.ListViewEx
         Dim oItem As SchemaClassTreeViewItem = New SchemaClassTreeViewItem("Default", SchemaClassTreeViewItem.ItemType.Schema)
         Dim oItemA As System.Windows.Forms.ListViewItem
         Dim oSubItemA As System.Windows.Forms.ListViewItem

         MyBase._listViewSchema.CheckBoxes = True
         '	oItem.DefaultGeometry
         Try
            oListViewEx = MyBase._listViewSchema
            oItemA = oListViewEx.Items.Add(oItem)
            oSubItemA = New System.Windows.Forms.ListViewItem("topoLots")
            MyBase.AddSubItems(oSubItemA)

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "387 Test ")
         End Try

      End Sub
		Public Function AddToMap() As Boolean
			Return MyBase.AddClassesToMap(False)
			'		MyBase.UncheckAllItems()
		End Function
		Public Function GetSchemaClassTreeView() As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeView
         Return MyBase._schemaClassTreeView
      End Function

      Public Sub New()

      End Sub
   End Class

   Protected Overrides Sub Finalize()
      moDataConnectUI = Nothing
      moSHPConnectionControl = Nothing
      '	moGenericBrowseSchemaControl = Nothing
      '     System.Windows.Forms.MessageBox.Show("", "04_599")
      MyBase.Finalize()
   End Sub
   Private Sub moImporter_RecordImported(oSender As System.Object, e As Autodesk.Gis.Map.ImportExport.RecordImportedEventArgs) 'Handles moImporter.RecordImported
      System.Windows.Forms.MessageBox.Show("@@@@@@", "06_999")
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      oEditor.WriteMessage("#@# " & oSender.GetType().ToString & ":" & e.ObjectId.ToString())
   End Sub
   Private Sub moImporter_RecordReadyForImport(oSender As System.Object, e As Autodesk.Gis.Map.ImportExport.RecordReadyForImportEventArgs) 'Handles moImporter.RecordReadyForImport
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      oEditor.WriteMessage("!!@# " & oSender.GetType().ToString & ":" & e.Entity.GetType().ToString())
   End Sub
   Public Structure ShapeFOData
      Const WorkArea As String = "WorkArea"

      Const Extension As String = ".shp"
      Const PrefixConnection As String = "shp_"
      Const PrefixXDataApp As String = "CPTopo_"
      Const PrefixBoundingBoxTopo As String = "BoundBox_"
      Dim ConnectionName As String
      Dim FeatureClass As String
      Dim ShapeFileName As String
      Dim SumMPgonArea As Double
      '	Dim Block As Integer
      '	Dim BlockAdd As Integer
      Dim Gush As GushData
      Public Sub New(sFolderName As String, sFileName As String)

         ConnectionName = PrefixConnection & sFileName
         FeatureClass = sFileName
         ShapeFileName = sFolderName & "\" & sFileName & Extension
      End Sub

      Public Sub New(sRootPath As String, sFolderName As String, sFileName As String)
         'System.Windows.Forms.MessageBox.Show(sRootPath & vbCrLf & sFolderName & vbCrLf & sFileName, "02_350")
         ConnectionName = PrefixConnection & sFileName
         FeatureClass = sFileName
         ShapeFileName = sRootPath & "\" & sFolderName & "\" & sFileName & Extension
      End Sub
      Public Function FileExists() As Boolean
         Dim oFile As IO.FileInfo = New IO.FileInfo(ShapeFileName)
         Return oFile.Exists
      End Function
      Public Function GetXDataAppName() As String
         Return PrefixXDataApp & FeatureClass
      End Function
      Public Shared Function GetBoundingBoxWorkAreaTopoName() As String
         Return PrefixBoundingBoxTopo & WorkArea
      End Function
      Public Function GetBoundingBoxTopoName() As String
         Return PrefixBoundingBoxTopo & FeatureClass
      End Function
      Public Shared Function GetCommonXDataAppName(sTopoName As String) As String
         Return PrefixXDataApp & sTopoName
      End Function
   End Structure
   Shared Sub NewTestQ(sName As String)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
		Dim sLayerTest As String = "C:\Users\Boris\AppData\Local\Autodesk, Inc\AutoCAD\R22.0.49.0.0\ParcelsLine\ParcelsLine.shp"
		Dim sShapeFileName As String = System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sName & "\" & sName & "AA" & ".shp"


		Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
		oMap.BeginLoadingLayers()
		DMCommon.Debug.MsgBox("02_701", sShapeFileName, oMap.GetCoordinateSystemId())
		Try
			oMap.LoadLayer(sLayerTest)
			'oMap.LoadLayer(sShapeFileName)
			oMap.EndLoadingLayers()
		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_222")
      End Try



      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
	Shared Sub NewTestK()
		Const sFeatureSource As String = "Library://shpParcelsLine.FeatureSource"

		Dim layerDefName As String = "NewLayer"
		'Dim layerId As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(layerDefName)
		'"Library://feature3.FeatureSource"
		' Use classes from xsd.exe to build the layer definition
		'Dim rasterId As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier("Library://rasterFeature.FeatureSource")
		Dim oFeatureId As OSGeo.MapGuide.MgResourceIdentifier = New OSGeo.MapGuide.MgResourceIdentifier(sFeatureSource)

		Dim rs As OSGeo.MapGuide.MgResourceService

		'		Dim oLayerDefType As LayerDefinitionType = New LayerDefinitionType()
		'		Dim oGridLayerDef As GridLayerDefinitionType = New GridLayerDefinitionType()
		'		oLayerDefType.Item = oGridLayerDef
		'		oGridLayerDef.ResourceId = rasterId.ToString()
		'		oGridLayerDef.FeatureName = "rasters:classname"
		'		oGridLayerDef.Geometry = "Image"
		'		Dim ranges() As GridScaleRangeType = {New GridScaleRangeType()}

		'		oGridLayerDef.GridScaleRange = ranges

		'	ranges(0).ColorStyle = New GridColorStylizationType()
		'		Dim colorRules() As GridColorRuleType = {New GridColorRuleType()}
		'		ranges(0).ColorStyle.ColorRule = colorRules
		'		colorRules(0) = New GridColorRuleType()
		'		colorRules(0).LegendLabel = ""
		'		colorRules(0).Color = New GridColorType()
		'		colorRules(0).Color.ItemElementName = ItemChoiceType.Band
		'		colorRules(0).Color.Item = "1"
		'		ranges(0).RebuildFactor = 1


		Dim layerDefString As String
		rs = CType(AcMapServiceFactory.GetService(OSGeo.MapGuide.MgServiceType.ResourceService), OSGeo.MapGuide.MgResourceService)
		Using writer As System.IO.StringWriter = New System.IO.StringWriter()

			'			Dim xs As System.Xml.Serialization.XmlSerializer = New System.Xml.Serialization.XmlSerializer(oLayerDefType.GetType())
			'			xs.Serialize(writer, oLayerDefType)
			layerDefString = writer.ToString()
		End Using
		' Convert Unicode to UTF-8

		Dim unicodeBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(layerDefString)


		Dim utf8Bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodeBytes)
		Dim xmlSource As OSGeo.MapGuide.MgByteSource = New OSGeo.MapGuide.MgByteSource(utf8Bytes, utf8Bytes.Length)
		rs.SetResource(oFeatureId, xmlSource.GetReader(), Nothing)

		'Dim oLayer As OSGeo.MapGuide.MgLayerBase = AcMapLayer.Create(layerId, rs)
		'oLayer.SetName("NewLayerA")
		'Dim oCurrentMap As AcMapMap = AcMapMap.GetCurrentMap()
		'oCurrentMap.GetLayers().Add(oLayer)

	End Sub

	Private Sub FDO_Manager_FormClosed(oSender As System.Object, e As Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
      Me.Finalize()
   End Sub
End Class
   Public Structure OverlayIDs
      Dim IDNew As Integer
      Dim ID1 As Integer
      Dim Handle1 As Autodesk.AutoCAD.DatabaseServices.Handle
      Dim ID2 As Integer
      Dim Handle2 As Autodesk.AutoCAD.DatabaseServices.Handle
      Dim ID3 As Integer
      Dim Handle3 As Autodesk.AutoCAD.DatabaseServices.Handle
      Public Sub New(iIDNew As Integer, iID1 As Integer, iID2 As Integer)
         IDNew = iIDNew
         ID1 = iID1
         ID2 = iID2
      End Sub
      Public Sub New(iIDNew As Integer, iID1 As Integer, iID2 As Integer, iID3 As Integer)
         IDNew = iIDNew
         ID1 = iID1
         ID2 = iID2
         ID3 = iID3
      End Sub
   End Structure
Public Structure ParcelData
   Dim PgonID As Integer
   Dim Block As Integer
   Dim BlockAdd As Integer
   Dim ParcelName As String
   Dim LegalArea As Double
   Dim Status As Integer
	Shared mia1603AttribIndices() As Integer = {0, 1, 2, 3}

	Public Sub New(iPgonID As Integer, iBlock As Integer, iBlockAdd As Integer, sParcelName As String, dLegalArea As Double, iStatus As Integer)
		PgonID = iPgonID
		Block = iBlock
		BlockAdd = iBlockAdd
		ParcelName = sParcelName
		LegalArea = dLegalArea
		Status = iStatus

	End Sub
	Public ReadOnly Property BlockKey As Integer
		Get
			Return GushData.GetBlockKey(Block, BlockAdd)
		End Get
	End Property
	Public ReadOnly Property BlockName As String
		Get
			Return TopoManager.TPlanGraph.TplnBlock.GetBlockName(Block, BlockAdd)
		End Get
	End Property

	Public Function GetAttribText() As String()
      Dim sa1603AttribText(mia1603AttribIndices.GetUpperBound(0)) As String
      sa1603AttribText(0) = Me.ParcelName
      sa1603AttribText(1) = CStr(Me.Block)
      sa1603AttribText(2) = CStr(Me.BlockAdd)
      sa1603AttribText(3) = CStr(Me.LegalArea * 0.001)
      '   DMCommon.ExcelLogG.SetValue(10, sa1603AttribText(0))
      Return sa1603AttribText
   End Function
End Structure
Public Structure GushData
   Const msSlash As String = "/"
   Dim PgonID As Integer
   Dim Block As Integer
   Dim BlockAdd As Integer
   Dim ParcelName As String
   Dim LegalArea As Double
   Dim Status As Integer
   Dim IsAnality As Integer
	Shared mia1601AttribIndices() As Integer = {0, 1, 2, 3, 4, 5}
	Public Shared Function GetBlockKey(iBlock As Integer, iBlockAdd As Integer) As Integer
		Return iBlock * 1000 + iBlockAdd
	End Function

	Public Sub New(iPgonID As Integer, iBlock As Integer, iBlockAdd As Integer, iStatus As Integer, iIsAnality As Integer, dLegalArea As Double)
      PgonID = iPgonID
      Block = iBlock
      BlockAdd = iBlockAdd
      Status = iStatus
      IsAnality = iIsAnality
      LegalArea = dLegalArea
   End Sub
	Public ReadOnly Property Key As Integer
		Get
			Return GetBlockKey(Block, BlockAdd)
		End Get
	End Property
	Public ReadOnly Property Folder As String
		Get
			Return TopoManager.TPlanGraph.TplnBlock.GetBlockFolder(Block, BlockAdd)
		End Get
	End Property

	Public Shared Operator +(tGushDataA As GushData, tGushDataB As GushData) As GushData
      If tGushDataA.Key = tGushDataB.Key Then
         Dim iStatus As Integer = zzIdenParam(tGushDataA.Status, tGushDataB.Status)
         Dim iIsAnality As Integer = zzIdenParam(tGushDataA.IsAnality, tGushDataB.IsAnality)
         Dim dLegalArea As Double = tGushDataA.LegalArea + tGushDataB.LegalArea
         Return New GushData(0, tGushDataA.Block, tGushDataA.BlockAdd, iStatus, iIsAnality, dLegalArea)
      Else
         Return New GushData()
      End If
   End Operator
   Private Shared Function zzIdenParam(iParamA As Integer, iParamB As Integer) As Integer
      Const iErrNum As Integer = 99
      If iParamA = iParamB Then
         Return iParamA
      ElseIf iParamA = 0 Then
         Return iParamB
      ElseIf iParamB = 0 Then
         Return iParamA
      Else
         Return iErrNum
      End If
   End Function
   Public Function GetAttribText() As String()
      Dim sa1601AttribText(mia1601AttribIndices.GetUpperBound(0)) As String
      sa1601AttribText(0) = CStr(Me.Block)
      If Me.BlockAdd <> 0 Then
         sa1601AttribText(1) = msSlash
         sa1601AttribText(2) = CStr(Me.BlockAdd)
      End If

      If Me.LegalArea > 0.0 Then
         sa1601AttribText(3) = CStr(Me.LegalArea * 0.001)
      Else
         sa1601AttribText(3) = String.Empty
      End If

      sa1601AttribText(4) = CStr(Me.Status)
      sa1601AttribText(5) = CStr(Me.IsAnality)
		'	DMCommon.Debug.MsgBox("13_103", DMCommon.Debug.GetListArray(sa1601AttribText))

		Return sa1601AttribText
   End Function
   Public Sub DebugWrite(sCaption As String)
      DMAcadExt.AcadDocument.WriteMessage(sCaption & " Gush " & CStr(Block) & "Status-" & CStr(Status))
   End Sub
End Structure

   Public Structure BlueLineData
      Dim PgonID As Integer
      Dim Block As Integer
      Dim BlockAdd As Integer
      Dim ParcelName As String
      Dim LegalArea As Double
      Dim Status As Integer
      Shared mia1603AttribIndices() As Integer = {0, 1, 2, 3}
      Public Sub New(iPgonID As Integer, iBlock As Integer, iBlockAdd As Integer, sParcelName As String, dLegalArea As Double, iStatus As Integer)
         PgonID = iPgonID
         Block = iBlock
         BlockAdd = iBlockAdd
         ParcelName = sParcelName
         LegalArea = dLegalArea
         Status = iStatus

      End Sub
      Public Function GetAttribText() As String()
         Dim sa1603AttribText(mia1603AttribIndices.GetUpperBound(0)) As String
         sa1603AttribText(0) = Me.ParcelName
         sa1603AttribText(1) = CStr(Me.Block)
         sa1603AttribText(2) = CStr(Me.BlockAdd)
         sa1603AttribText(3) = CStr(Me.LegalArea * 0.001)
         Return sa1603AttribText
      End Function
   End Structure
   Public Structure LotData_Complot
      Dim PgonID As Integer
      Dim LAYER As String
      Dim Gush As Integer
      Dim Helka As Integer
      Dim Ykd2 As Integer
      Dim Ystr As String
      Dim THICKNESS As Double
      Dim GAR_KEY As Double
      ' Shared miaCellnoAttribIndices() As Integer = {0, 1, 2, 3}




      Shared miaCellnoAttribIndices() As Integer = {1}
      Public Sub New(iPgonID As Integer, sLAYER As String, iGush As Integer, iHelka As Integer, iYkd2 As Integer, sYstr As String)
         PgonID = iPgonID
         LAYER = sLAYER
         Gush = iGush
         Helka = iHelka
         Ykd2 = iYkd2
         Ystr = sYstr


      End Sub
      Public Function GetAttribText() As String()
         Dim saCellnoAttribText(miaCellnoAttribIndices.GetUpperBound(0)) As String
         saCellnoAttribText(0) = CStr(Me.Ykd2)
         Return saCellnoAttribText
      End Function
   End Structure
