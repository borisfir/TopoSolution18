Option Explicit On
Option Strict On
Imports OSGeo.MapGuide
Imports Autodesk.Gis.Map.Platform
'C:\Program Files\Autodesk\AutoCAD Map 3D 2012\OSGeo.MapGuide.PlatformBase.dll
Public Class Util
   Public Const FeatureSourceExtention As String = ".FeatureSource"
   Shared msSourceLayer As String = String.Empty
   Shared msOverlayLayer As String


   Private Structure AdditBLineInfo
      Dim PlanID As Integer
      Dim PlaNum As String
      Dim PlanDate As Date

   End Structure
   Public Shared Sub ClearResources(keyword As String)
      Dim rs As MgResourceService = ResourceService

      '' Remove the layers whose names contain the keyword
      Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
      Dim layers As MgLayerCollection = oMap.GetLayers()

      For Each layer As MgLayerBase In layers
         If (layer.Name.IndexOf(keyword) >= 0) Then
            System.Windows.Forms.MessageBox.Show(layer.Name & vbCrLf & keyword, "01_470 Remove Layer")
            layers.Remove(layer)
         End If
      Next

      '	Return

      Dim oDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

      '' Remove the layer definitions whose names contain the keyword
      Dim reader As MgByteReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.LayerDefinition)
      '	System.Windows.Forms.MessageBox.Show(keyword, "01_400")
      oDoc.LoadXml(reader.ToString())
      For Each node As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
         Dim resId As String = node.InnerText
         System.Windows.Forms.MessageBox.Show(resId, "01_401")
         If (Not String.IsNullOrEmpty(resId) AndAlso resId.IndexOf(keyword) >= 0) Then
            '	System.Windows.Forms.MessageBox.Show(node.InnerText, "01_402")
            System.Windows.Forms.MessageBox.Show(resId & vbCrLf & keyword, "01_472 DeleteResource A")
            rs.DeleteResource(New MgResourceIdentifier(resId))
         End If
      Next

      '' Remove the layer feature sources whose names contain the keyword
      reader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.FeatureSource)
      oDoc.LoadXml(reader.ToString())
      For Each node As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")

         Dim resId As String = node.InnerText
         '	System.Windows.Forms.MessageBox.Show(resId, "01_411")
         If (Not String.IsNullOrEmpty(resId) AndAlso resId.IndexOf(keyword) >= 0) Then
            System.Windows.Forms.MessageBox.Show(resId & vbCrLf & keyword, "01_474 DeleteResource B")
            rs.DeleteResource(New MgResourceIdentifier(resId))
            '			System.Windows.Forms.MessageBox.Show(node.InnerText, "01_412")
         End If
      Next
   End Sub

	Public Shared Sub ClearResource(sConnection As String, sLayerName As String)
		Dim rs As MgResourceService = ResourceService
		'		System.Windows.Forms.MessageBox.Show(sConnection & ":" & sLayerName, "01_460 ClearResource")
		'' Remove the layers whose names contain the keyword
		Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
		Dim layers As MgLayerCollection = oMap.GetLayers()
		Dim sResID As String

		'System.Windows.Forms.MessageBox.Show(, "01_470")
		'	Return

		Dim oDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

		'' Remove the layer definitions whose names contain the keyword
		Dim reader As MgByteReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.LayerDefinition)
		'	System.Windows.Forms.MessageBox.Show(keyword, "01_400")
		oDoc.LoadXml(reader.ToString())
		For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
			sResID = oNode.InnerText

			DMCommon.Debug.MsgBox("01_401", sResID, sConnection, sLayerName) ' "Library://" & sLayerName & ".LayerDefinition"
			If (Not String.IsNullOrEmpty(sResID) AndAlso (sResID = "Library://" & sLayerName & ".LayerDefinition")) Then
				'	System.Windows.Forms.MessageBox.Show(oNode.InnerText, "01_402")

				Try
					rs.DeleteResource(New MgResourceIdentifier(sResID))
				Catch oEx As Exception
					DMCommon.Debug.UserMsg("01_401c", sResID, sConnection, sLayerName, oEx.Message, oEx.GetType())
				End Try

			End If
		Next
		If True Then
			'' Remove the layer feature sources whose names contain the keyword
			reader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.FeatureSource)
			oDoc.LoadXml(reader.ToString())
			For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
				sResID = oNode.InnerText
				DMCommon.Debug.MsgBox("01_401E", sResID, sConnection, sLayerName)
				'System.Windows.Forms.MessageBox.Show(sResID & vbCrLf & "Library://" & sConnection & ".FeatureSource", "01_411")
				If (Not String.IsNullOrEmpty(sResID) AndAlso (sResID = "Library://" & sConnection & ".FeatureSource")) Then
					'	System.Windows.Forms.MessageBox.Show(oNode.InnerText, "01_412")
					Try
						rs.DeleteResource(New MgResourceIdentifier(sResID))
					Catch oEx As Exception
						DMCommon.Debug.UserMsg("01_401f", sResID, oEx.Message, oEx.GetType())

					End Try

					'			System.Windows.Forms.MessageBox.Show(node.InnerText, "01_412")
				End If
			Next
		End If


		For Each layer As MgLayerBase In layers
			If (layer.Name = sLayerName) Then
				DMCommon.Debug.MsgBox("01_470 Del Layer", sLayerName)
				layers.Remove(layer)
			End If
		Next
	End Sub
	Public Shared Sub EnumAllResources()
		Try
			Dim rs As MgResourceService = ResourceService

			'' Remove the layers whose names contain the keyword
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim oDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

			'' Remove the layer definitions whose names contain the keyword
			Dim oReader As MgByteReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.LayerDefinition)

			Dim sResID As String
			oDoc.LoadXml(oReader.ToString())
			For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
				sResID = oNode.InnerText
				'  System.Windows.Forms.MessageBox.Show(sResID, "sResID 01_833")
				If (Not String.IsNullOrEmpty(sResID)) Then
					'	rs.DeleteResource(New MgResourceIdentifier(sResID))
					DMAcadExt.AcadDocument.WriteDebugMessageN(MgResourceType.LayerDefinition, sResID)
				End If
			Next

			'' Remove the layer feature sources whose names contain the keyword
			oReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.FeatureSource)
			oDoc.LoadXml(oReader.ToString())
			For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
				sResID = oNode.InnerText
				'   System.Windows.Forms.MessageBox.Show(sResID, "sResID 01_837")
				If (Not String.IsNullOrEmpty(sResID)) Then
					'rs.DeleteResource(New MgResourceIdentifier(sResID))
					DMAcadExt.AcadDocument.WriteDebugMessageN(MgResourceType.FeatureSource, sResID)
				End If
			Next
			Dim oLayerResourceIdentifier As MgResourceIdentifier
			Dim colLayers As MgLayerCollection = oMap.GetLayers()
			For Each oMgLayerBase As OSGeo.MapGuide.MgLayerBase In colLayers
				DMAcadExt.AcadDocument.WriteDebugMessageN("Layers:", oMgLayerBase.Name)
				zzDispLayerInfo(oMgLayerBase)
				oLayerResourceIdentifier = oMgLayerBase.GetLayerDefinition()
				DMAcadExt.AcadDocument.WriteDebugMessageN("LayerDef", oLayerResourceIdentifier.Name, oLayerResourceIdentifier.Path, oLayerResourceIdentifier.RepositoryName, oLayerResourceIdentifier.RepositoryType) 'oMapLayer.CoordinateSystemId,
			Next
			'colLayers.Clear()
			Dim oResourceIdentifier As MgResourceIdentifier = New MgResourceIdentifier("Library://ParcelsLine.LayerDefinition")
			'Dim oResourceIdentifier As MgResourceIdentifier = New MgResourceIdentifier("Library://")

			Dim oResourceService As MgResourceService = ResourceService()

			'	Dim oNewMgLayerBase As MgLayerBase = New MgLayerBase(
			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(oResourceIdentifier, oResourceService)
			DMCommon.Debug.MsgBox("MapLayer", oMapLayer)
			DMAcadExt.AcadDocument.WriteDebugMessageN("AcMapLayer", oMapLayer.FeatureClassName, oMapLayer.FeatureSourceId, oMapLayer.Name) 'oMapLayer.CoordinateSystemId,

			oMapLayer.Visible = False
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Clear All Resources")
		End Try
		'LayerDefinition: Library : //ParcelsLine.LayerDefinition
		'FeatureSource: Library : //shpParcelsLine.FeatureSource
		'Layers: ParcelsLine
		'	08 9788231  SVETA 
	End Sub
	Public Shared Sub TestAddLayer()
		Dim oResourceService As MgResourceService = ResourceService()

		Dim oLayerDefResourceId As MgResourceIdentifier = New MgResourceIdentifier("Library://ParcelsLine.LayerDefinition")
		Dim oResourceIdentifier As MgResourceIdentifier = New MgResourceIdentifier("Library://ParcelsLine.LayerDefinition")
		Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
		Dim colLayers As MgLayerCollection = oMap.GetLayers()

		Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(oLayerDefResourceId, oResourceService)

		oMapLayer.Name = "ParcelsNew"

		AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)
	End Sub
	Public Sub AutoCreateLayersSource()

		Dim featuresrc As String = "Library://Oracle_1.FeatureSource"
		Dim layerpath As String = "Library://REGION.LayerDefinition"

		Dim rs As MgResourceService = DirectCast(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)

		'load template xml file
		Dim LayerDefXML As String = "layer.xml"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)
		Dim featuresrcid As MgResourceIdentifier = New MgResourceIdentifier(featuresrc)
		Dim layer As MgLayerBase = Nothing
		Dim layerdefid As MgResourceIdentifier = New MgResourceIdentifier(layerpath)
		'make mods to xml
		'set feature source
		Dim residnode As System.Xml.XmlNode = LayerDef.DocumentElement.SelectSingleNode("//ResourceId")
		residnode.InnerText = featuresrc
		'convert xml content into byte data
		Dim strlayerdef As String = LayerDef.OuterXml
		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)
		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As MgByteSource = New MgByteSource(bytes, bytes.Length)
		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		Try

			'create a New layer based on the def
			layer = New MgLayerBase(layerdefid, rs)
			Dim layername As String = "region"
			layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			AcMapMap.GetCurrentMap().GetLayers().Add(layer)

		Catch ex As MgException

			System.Windows.Forms.MessageBox.Show(ex.Message)
		End Try

	End Sub
	Public Shared Sub AutoCreateLayersMyA(sSHPFileFullName As String, sFeature As String, sLayerName As String)

		'	Dim sNewLayerName As String = "NewParcels"

		DMCommon.Debug.MsgBox("AutoCreateLayersMyA", sSHPFileFullName, sFeature, sLayerName)
		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"

		Dim rs As MgResourceService = DirectCast(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)

		'load template xml file
		'	Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)

		Dim featuresrcid As MgResourceIdentifier = New MgResourceIdentifier(featuresrc)

		Dim layer As MgLayerBase = Nothing
		Dim layerdefid As MgResourceIdentifier = New MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source
		Dim residnode As System.Xml.XmlNode = LayerDef.DocumentElement.SelectSingleNode("//ResourceId")

		residnode.InnerText = featuresrc

		'convert xml content into byte data
		'Dim strlayerdef As String = LayerDef.OuterXml
		Dim strlayerdef As String = GetLayerDef(sSHPFileFullName, sFeature) '

		Const sOutputPath As String = "D:\Parcel11.xml"

		Dim oNewLayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		oNewLayerDef.LoadXml(strlayerdef)
		oNewLayerDef.Save(sOutputPath)



		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)

		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As MgByteSource = New MgByteSource(bytes, bytes.Length)

		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		DMCommon.Debug.MsgBox("13_032k", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try

			'create a New layer based on the def
			'	layer = New MgLayerBase(layerdefid, rs)
			'Dim layername As String = sLayerName
			'layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			'	AcMapMap.GetCurrentMap().GetLayers().Add(layer)

			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)
			'	DMCommon.Debug.MsgBox("13_032L")
			'	oMapLayer.Name = sNewLayerName
			'	DMCommon.Debug.MsgBox("13_032m")
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As MgLayerCollection = oMap.GetLayers()
			DMCommon.Debug.MsgBox("13_032N", colLayers.Count)
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch ex As MgException

			System.Windows.Forms.MessageBox.Show(ex.Message)
		End Try

	End Sub
	Public Shared Sub AutoCreateLayersMy()
		Dim sLayerName As String = "ParcelsLine"
		'	Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shpParcelsLine"
		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"

		Dim rs As MgResourceService = DirectCast(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)

		'load template xml file
		'	Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		Dim LayerDefXML As String = "T:\Boris\p2792.layer" '"layer.xml"

		'	Dim LayerDefXML As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)

		Dim featuresrcid As MgResourceIdentifier = New MgResourceIdentifier(featuresrc)

		Dim layer As MgLayerBase = Nothing
		Dim layerdefid As MgResourceIdentifier = New MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source
		Dim residnode As System.Xml.XmlNode = LayerDef.DocumentElement.SelectSingleNode("//ResourceId")

		residnode.InnerText = featuresrc

		'convert xml content into byte data
		Dim strlayerdef As String = LayerDef.OuterXml
		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)

		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As MgByteSource = New MgByteSource(bytes, bytes.Length)

		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		DMCommon.Debug.MsgBox("13_032wk", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try

			'create a New layer based on the def
			'	layer = New MgLayerBase(layerdefid, rs)
			'Dim layername As String = sLayerName
			'layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			'	AcMapMap.GetCurrentMap().GetLayers().Add(layer)

			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)
			'	DMCommon.Debug.MsgBox("13_032L")
			'	oMapLayer.Name = sNewLayerName
			'	DMCommon.Debug.MsgBox("13_032m")
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As MgLayerCollection = oMap.GetLayers()
			DMCommon.Debug.MsgBox("13_032N", colLayers.Count)
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch ex As MgException

			System.Windows.Forms.MessageBox.Show(ex.Message)
		End Try

	End Sub
	Public Shared Sub AutoCreateLayers()
		Dim sLayerName As String = "ParcelsLine"
		Dim sNewLayerName As String = "NewParcels"

		Dim sFeature As String = "shpParcelsLine"
		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"
		Dim layerpath As String = "Library://" & sLayerName & ".LayerDefinition"


		Dim rs As MgResourceService = DirectCast(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)

		'load template xml file
		Dim LayerDefXML As String = "T:\Boris\ParcelsLine.layer" '"layer.xml"
		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(LayerDefXML)

		Dim featuresrcid As MgResourceIdentifier = New MgResourceIdentifier(featuresrc)

		Dim layer As MgLayerBase = Nothing
		Dim layerdefid As MgResourceIdentifier = New MgResourceIdentifier(layerpath)

		'make mods to xml
		'set feature source
		If False Then
			Dim residnode As System.Xml.XmlNode = LayerDef.DocumentElement.SelectSingleNode("//ResourceId")
			DMCommon.Debug.MsgBox("13_032g")
			residnode.InnerText = featuresrc
			DMCommon.Debug.MsgBox("13_032h")
		End If

		'convert xml content into byte data
		'	Dim strlayerdef As String = LayerDef.OuterXml
		Dim strlayerdef As String = GetLayerDef("", "")

		Dim unicodebyte() As Byte = System.Text.Encoding.Unicode.GetBytes(strlayerdef)
		'DMCommon.Debug.MsgBox("13_032i")
		Dim bytes() As Byte = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodebyte)
		Dim source As MgByteSource = New MgByteSource(bytes, bytes.Length)
		DMCommon.Debug.MsgBox("13_032j")
		'dd layer to definitation
		rs.SetResource(layerdefid, source.GetReader(), Nothing)
		DMCommon.Debug.MsgBox("13_032k", layerdefid.Path, layerdefid.RepositoryName, layerdefid.GetPath, layerdefid.GetName, layerdefid.GetClassName)
		Try

			'create a New layer based on the def
			'	layer = New MgLayerBase(layerdefid, rs)
			'Dim layername As String = sLayerName
			'layer.SetName(layername)
			'layer.SetLayerDefinition(layerdefid, rs);
			'	AcMapMap.GetCurrentMap().GetLayers().Add(layer)

			Dim oMapLayer As AcMapLayer = Autodesk.Gis.Map.Platform.AcMapLayer.Create(layerdefid, rs)
			DMCommon.Debug.MsgBox("13_032L")
			oMapLayer.Name = sNewLayerName
			DMCommon.Debug.MsgBox("13_032m", oMapLayer.Name, oMapLayer.FeatureSourceId)
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim colLayers As MgLayerCollection = oMap.GetLayers()
			DMCommon.Debug.MsgBox("13_032NN", colLayers.Count)
			colLayers.Add(oMapLayer)
			'AcMapMap.GetCurrentMap().GetLayers().Add(oMapLayer)

		Catch ex As MgException

			System.Windows.Forms.MessageBox.Show(ex.Message)
		End Try

	End Sub
	Public Shared Function GetLayerDef(sSHPFileFullName As String, sFeature As String) As String
		'Const sPath As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"
		'	Const sPath As String = "T:\Boris\ParcelsLine.layer"
		Const sPath As String = "T:\Boris\p2792.layer"


		Dim featuresrc As String = "Library://" & sFeature & ".FeatureSource"


		Dim oMainXmlNode As System.Xml.XmlNode
		Dim sMain As String

		Dim oXmlNodePath As System.Xml.XmlNode
		Dim oXmlNodePathIn1 As System.Xml.XmlNode
		Dim oXmlNodePathIn2 As System.Xml.XmlNode
		Dim oXmlNodePathIn3 As System.Xml.XmlNode
		Dim oXmlNodePathIn4 As System.Xml.XmlNode



		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(sPath)
		Dim residnode As System.Xml.XmlNode = LayerDef.DocumentElement.SelectSingleNode("//ResourceId")

		residnode.InnerText = featuresrc

		If Not String.IsNullOrEmpty(sSHPFileFullName) Then


			oMainXmlNode = LayerDef.LastChild()
			sMain = oMainXmlNode.Name
			Dim i As Integer = oMainXmlNode.ChildNodes.Count

			oXmlNodePath = oMainXmlNode.ChildNodes.Item(10)

			oXmlNodePathIn1 = oXmlNodePath.FirstChild
			oXmlNodePathIn2 = oXmlNodePathIn1.FirstChild
			oXmlNodePathIn3 = oXmlNodePathIn2.ChildNodes.Item(1)
			oXmlNodePathIn4 = oXmlNodePathIn3.ChildNodes.Item(1)
			DMCommon.Debug.MsgBox("13_032P", oXmlNodePathIn4.InnerText, sSHPFileFullName, String.Compare(oXmlNodePathIn4.InnerText, sSHPFileFullName))
			oXmlNodePathIn4.InnerText = sSHPFileFullName

		End If



		Return LayerDef.OuterXml


	End Function
	Public Shared Sub ClearAllResources()
		Try
			Dim oMgResourceIdentifier As MgResourceIdentifier
			Dim rs As MgResourceService = ResourceService
			'' Remove the layers whose names contain the keyword
			Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
			Dim oDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()

			Dim colLayers As MgLayerCollection = oMap.GetLayers()
			'	DMCommon.Debug.MsgBox("13_044Before Clear", colLayers.Count)
			Try
				colLayers.Clear()
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("13_044Y", oEx.Message, oEx.GetType())
			End Try


			'' Remove the layer definitions whose names contain the keyword
			Dim oReader As MgByteReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.LayerDefinition)

			Dim sResID As String
			oDoc.LoadXml(oReader.ToString())
			For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
				sResID = oNode.InnerText
				'  System.Windows.Forms.MessageBox.Show(sResID, "sResID 01_833")
				'	DMCommon.Debug.MsgBox("13_044R", sResID)
				If (Not String.IsNullOrEmpty(sResID)) Then
					Try
						oMgResourceIdentifier = New MgResourceIdentifier(sResID)
						'	DMCommon.Debug.MsgBox("13_044Y", oMgResourceIdentifier.Name, oMgResourceIdentifier.Path, oMgResourceIdentifier.RepositoryName)
						rs.DeleteResource(oMgResourceIdentifier)
					Catch oEx As MgException
						DMCommon.Debug.MsgBox("13_044W", sResID, oEx.Message, "------", oEx.GetExceptionMessage, "------", oEx.GetDetails(), oEx.GetType())
					End Try

				End If
			Next

			'' Remove the layer feature sources whose names contain the keyword
			oReader = rs.EnumerateResources(New MgResourceIdentifier("Library://"), 0, MgResourceType.FeatureSource)
			oDoc.LoadXml(oReader.ToString())
			For Each oNode As System.Xml.XmlNode In oDoc.GetElementsByTagName("ResourceId")
				sResID = oNode.InnerText

				If (Not String.IsNullOrEmpty(sResID)) Then
					Try
						rs.DeleteResource(New MgResourceIdentifier(sResID))
					Catch oEx As Exception
						DMCommon.Debug.MsgBox("13_044X", sResID, oEx.Message, oEx.GetType())
					End Try

				End If
			Next

			'''''ZDES BIL

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Clear All Resources")
      End Try
   End Sub

	Public Shared Sub TestFeature()
		Dim oFeatureService As MgFeatureService = FeatureService
		Dim oMap As Autodesk.Gis.Map.Platform.AcMapMap = AcMapMap.GetCurrentMap()
		Dim oDoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		Dim oReader As MgByteReader = oFeatureService.GetFeatureProviders()

	End Sub

	Public Shared ReadOnly Property ResourceService As MgResourceService
      Get
         Dim rs As MgResourceService = CType(Autodesk.Gis.Map.Platform.AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)
         Return rs

      End Get
   End Property

	Public Shared ReadOnly Property FeatureService As MgFeatureService
		Get
			Dim fs As MgFeatureService = CType(Autodesk.Gis.Map.Platform.AcMapServiceFactory.GetService(MgServiceType.FeatureService), MgFeatureService)
			Return fs
		End Get
	End Property

	''' <summary>
	'''Create feature source for a sdf file and connect to it.
	'''</summary>
	''' <param name="libraryPath">User defined library path</param>
	''' <param name="dataFileRelativePath">Relative path of the SDF file to the Library path</param>
	'''<param name="bReadOnly">Open mode</param>
	''' <returns>The resource id of the feature source created</returns>
	Public Shared Function ConnectToSdfFile(libraryPath As String, dataFileRelativePath As String, bReadOnly As Boolean) As MgResourceIdentifier


      ' Get the file path
      Dim info As IO.FileInfo = New IO.FileInfo(libraryPath + dataFileRelativePath)

      ' Check if the specified file exists
      If (Not info.Exists) Then
         Throw New IO.FileNotFoundException(String.Format("The specified file {1} doesn't exist.", info.FullName))
      End If


      ' Check if the file format is supported
      ' Only SDF is supported so far
      If (String.Compare(info.Extension, Autodesk.Gis.Map.Platform.Utils.Util.SdfFileExtention, True) <> 0) Then
         Throw New InvalidOperationException("Only SDF files are supported.")
      End If


      ' Populate feature source definition string
      Dim featureSourceDef As String = CreateSdfFeatureSourceDefinition(info.FullName, bReadOnly)

      ' Create the resource id
      Dim resId As MgResourceIdentifier = New MgResourceIdentifier(PopulateFeatureSourceName(dataFileRelativePath))

      ' Save the feature source definition
      ' For debug only
      SaveXml(GetCurrentDir() & "\\" & resId.Name + Autodesk.Gis.Map.Platform.Utils.Util.FeatureSourceExtention, featureSourceDef)

      ' Add the resource into Map 3D
      Dim bytes() As Byte = StringToBytes(featureSourceDef)
      Dim source As MgByteSource = New MgByteSource(bytes, bytes.Length)
      ResourceService.SetResource(resId, source.GetReader(), Nothing)

      Return resId
   End Function

   ''' <summary>
   ''' Populate xml string presents feature source definition for the resource file.
   '''Only SDF files are supported in this assembly.
   ''' </summary>
   ''' <param name="fileName">The path of the resource file</param>
   ''' <param name="bReadOnly">Open mode of the resource file</param>
   ''' <returns>The feature source definition string</returns>
   Public Shared Function CreateSdfFeatureSourceDefinition(fileName As String, bReadOnly As Boolean) As String

      ' Validation
      Dim info As IO.FileInfo = New IO.FileInfo(fileName)
      Debug.Assert(info.Exists)

		' Build the feature source object model
		Dim fsType As OSGeo.MapGuide.Schema.FeatureSource.FeatureSourceType = New OSGeo.MapGuide.Schema.FeatureSource.FeatureSourceType()

		If (String.Compare(info.Extension, Autodesk.Gis.Map.Platform.Utils.Util.SdfFileExtention, True) = 0) Then ' SDF file

         fsType.Provider = Autodesk.Gis.Map.Platform.Utils.Util.SDFProviderName ' FDO provider name, case sensitive


         Dim p1 As OSGeo.MapGuide.Schema.FeatureSource.NameValuePairType = New OSGeo.MapGuide.Schema.FeatureSource.NameValuePairType()

         p1.Name = "ReadOnly"
         p1.Value = bReadOnly.ToString() ' non case sensitive

         Dim p2 As OSGeo.MapGuide.Schema.FeatureSource.NameValuePairType = New OSGeo.MapGuide.Schema.FeatureSource.NameValuePairType()
         p2.Name = "File"
         p2.Value = info.FullName ' Either double backslash or single backslash is OK for the file path

         fsType.Parameter = New OSGeo.MapGuide.Schema.FeatureSource.NameValuePairType() {p2, p1}

      Else  ' Not supported

         Debugger.Break()
         Throw New System.NotSupportedException("Not supported file type: " + info.Extension)
      End If

      ' Serialize the feature source object model to xml string
      Using writer As IO.StringWriter = New IO.StringWriter()

         Dim xs As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(fsType.GetType())
         xs.Serialize(writer, fsType)
         Return writer.ToString()
      End Using
   End Function


   ''' <summary>
   ''' Temporory solution to populate the feature source name from the 
   ''' SDF file relative path to the user defined library path.
   ''' </summary>
   ''' <remarks>
   ''' The path in a feature source name doesn't affect Map's behaviors 
   ''' so far.
   ''' </remarks>
   ''' <param name="relativePath">Relative path of the SDF file to library path</param>
   ''' <returns>Feature source name</returns>
   Public Shared Function PopulateFeatureSourceName(relativePath As String) As String

      Dim info As IO.FileInfo = New IO.FileInfo(relativePath)
      relativePath = relativePath.TrimStart(New Char() {"."(0), "\\"(0), "/"(0)})
      relativePath = NormalizeResourceName(relativePath)
      Return ("Library://" & relativePath.Substring(0, relativePath.Length - info.Extension.Length) & FeatureSourceExtention)
   End Function

   '''  <summary>
   '''  Temporory solution to populate the feature source name from the 
   '''  SDF file full path and the user defined library path.
   '''  </summary>
   '''  <param name="fullPath">Full path of the SDF file</param>
   '''  <param name="libraryPath">User defined library path</param>
   '''  <returns>Feature source name</returns>
   Public Shared Function PopulateFeatureSourceNameAAA(fullPath As String, libraryPath As String) As String

      Dim fullPathInfo As IO.FileInfo = New IO.FileInfo(fullPath)
      Dim libraryPathInfo As IO.DirectoryInfo = New IO.DirectoryInfo(libraryPath)

      Dim relativePath As String = fullPathInfo.FullName.Substring(libraryPathInfo.FullName.Length)

      ' relativePath may contains a string like "\Data\SDF\A.sdf"
      Dim fsName As String = "Library://" + relativePath.Substring(1, relativePath.Length - fullPathInfo.Extension.Length) & FeatureSourceExtention

      Return NormalizeResourceName(fsName)
   End Function
   Public Shared Function NormalizeResourceName(resourceName As String) As String

      Return resourceName.Replace("\\", "/")
   End Function



   '''  <summary>
   '''  Temporory solution to populate the feature source name from the 
   '''  SDF file full path and the user defined library path.
   '''  </summary>
   '''  <param name="fullPath">Full path of the SDF file</param>
   '''  <param name="libraryPath">User defined library path</param>
   '''  <returns>Feature source name</returns>
   Public Shared Function PopulateFeatureSourceName(fullPath As String, libraryPath As String) As String
      Dim fullPathInfo As IO.FileInfo = New IO.FileInfo(fullPath)
      Dim libraryPathInfo As IO.DirectoryInfo = New IO.DirectoryInfo(libraryPath)
      Dim relativePath As String = fullPathInfo.FullName.Substring(libraryPathInfo.FullName.Length)
      ' relativePath may contains a string like "\Data\SDF\A.sdf"
      Dim fsName As String = "Library://" & relativePath.Substring(1, relativePath.Length - fullPathInfo.Extension.Length) & FeatureSourceExtention

      Return NormalizeResourceName(fsName)
   End Function

   ''' <summary>
   ''' Convert a byte array to .Net string (Unicode encoding).
   ''' </summary>
   ''' <param name="buffer">The byte array to be converted</param>
   ''' <returns>The .Net string</returns>
   Public Shared Function BytesToString(buffer() As Byte) As String
      Return System.Text.Encoding.UTF8.GetString(buffer)
   End Function


   Public Shared Sub SaveXml(fileName As String, buffer() As Byte)

      Dim xml As String = BytesToString(buffer)
      Trace.WriteLine(xml)

      Using sw As IO.StreamWriter = New IO.StreamWriter(fileName, False)

         sw.Write(xml)
      End Using
   End Sub

   Public Shared Sub SaveXml(fileName As String, xml As String)
      Try

      Catch ex As Exception

      End Try
      Try

         Dim reader As System.Xml.XmlReader = System.Xml.XmlReader.Create(New IO.StringReader(xml))
         Dim doc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
         doc.Load(reader)
         Using writer As System.Xml.XmlTextWriter = New System.Xml.XmlTextWriter(fileName, System.Text.Encoding.UTF8)

            writer.Formatting = System.Xml.Formatting.Indented
            writer.Indentation = 4
            writer.IndentChar = Char.Parse(" ")
            doc.Save(writer)
         End Using
         reader.Close()

      Catch oEx As Exception

         Trace.WriteLine(oEx.Message)
      End Try
   End Sub


   ''' <summary>
   ''' The path of the current executing assembly.
   '''</summary>
   Public Shared Function GetCurrentDir() As String

      Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
      Dim info As IO.FileInfo = New IO.FileInfo(asm.Location)
      Return info.DirectoryName
   End Function
   ''' <summary>
   ''' A temporary solution for string -> byte[] conversion.
   ''' The assumption of this function is that the input is encoded in Unicode mode.
   ''' </summary>
   ''' <param name="buffer">The string to be converted</param>
   ''' <returns>The byte array</returns>
   Public Shared Function StringToBytes(buffer As String) As Byte()

      'Convert the string into a byte[].
      Dim unicodeBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(buffer)

      'Perform the conversion from one encoding to the other.
      Return System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodeBytes)
   End Function
   Public Shared Sub EnumShapePgons()
      '   Const sRoot As String = "P:\2016\160590\Plan\Compil\BlueLines\P\"
      Const sRoot As String = "P:\2016\160470\Plan\Blue_Lines\P\"

      Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRoot)
      Dim sShapeFile As String
      Dim sShapeFileFullName As String
      Dim i As Integer
      Dim sFCName As String
      Dim oFDO_Manager As FDO_Manager = New FDO_Manager()
      Dim sShapeConnection As String
      Dim sDirName As String


      For Each oDirectoryInfo As IO.DirectoryInfo In oRootFolder.GetDirectories()
         sDirName = oDirectoryInfo.Name
         sFCName = sDirName.Substring(0, 10)
         sShapeFile = sFCName & ".SHP"
         sShapeFileFullName = oDirectoryInfo.FullName & "\" & sShapeFile
         sShapeConnection = "shp_" & sDirName
         DMAcadExt.AcadDocument.WriteMessage(sShapeFileFullName)
         oFDO_Manager.ConnectToShape(sShapeConnection, sShapeFileFullName)
         oFDO_Manager.AddLayerToMap(sShapeConnection)
         If i = 0 Then
            msSourceLayer = sFCName
         Else
            msOverlayLayer = sFCName

         End If

         '  
         i += 1
         If i = 2 Then
            Exit For
         End If
      Next
   End Sub
   Public Shared Sub SeparateShapePgons_RamatGan()
      Const sRoot As String = "P:\2016\160590\Plan\Compil\BlueLines\P\"
      '   Dim o As Integer() = {1, 2, 3}
      Dim colMPgonsIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colMPgonID As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oShapeExpImp As ShapeExpImp
      Dim sShapeFileName As String


      Dim oMPgonTable As DMAcadExt.ODTable
      Dim oRec As Autodesk.Gis.Map.ObjectData.Record
      Dim oValue As Autodesk.Gis.Map.Utilities.MapValue
      Dim sDate As String
      Dim sDateNum As String

      Dim i As Integer
      Dim iFolderNum As Integer
      Dim iMin As Integer = 0 '1400
      Dim iMax As Integer = 20


      Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRoot)
      Dim hsDates As HashSet(Of String) = New HashSet(Of String)
      Dim oAcadObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim oaSubFolders() As IO.DirectoryInfo
      oMPgonTable = New DMAcadExt.ODTable("BlueLines")
      oaSubFolders = oRootFolder.GetDirectories()
      For iIndex As Integer = 0 To oaSubFolders.GetUpperBound(0)
         hsDates.Add(oaSubFolders(iIndex).Name.Substring(2))
         '  DMAcadExt.AcadDocument.WriteMessage("oaSubFolders(iIndex).Name =" & CStr(oaSubFolders(iIndex).Name.Substring(2)))
      Next

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)

      colMPgonsIDs = DMAcadExt.AcadTransaction.GetAllDBObjectsByRxClass(DMAcadExt.AcadConst.AcadMPolygonName)
      colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      DMAcadExt.AcadDocument.WriteMessage("Total = " & CStr(colMPgonsIDs.Count))
      For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colMPgonsIDs
         If i >= iMin Then
            iFolderNum = 1
            oRec = oMPgonTable.GetODRecord(tAcObjID)
            oValue = oRec.Item(3)
            sDate = oValue.StrValue
            Do
               sDateNum = sDate & "_" & iFolderNum.ToString()
               If hsDates.Contains(sDateNum) Then
                  iFolderNum += 1

               Else
                  hsDates.Add(sDateNum)
                  Exit Do
               End If
            Loop

            oRootFolder.CreateSubdirectory("P_" & sDateNum)
            sShapeFileName = sRoot & "P_" & sDateNum & "\" & "P_" & sDate
            '   DMAcadExt.AcadDocument.WriteMessage("sShapeFileName =" & CStr(sShapeFileName))
            '  colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection(New Autodesk.AutoCAD.DatabaseServices.ObjectId() {tAcObjID})
            colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection(New Autodesk.AutoCAD.DatabaseServices.ObjectId() {tAcObjID})
            colMPgonID.Clear()
            colMPgonID.Add(tAcObjID)

            oAcadObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
            oShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
            oShapeExpImp.AddShapeData(2)

            If oShapeExpImp.FromPoligons(colMPgonID) Then
               oShapeExpImp.Exec()
            Else


               '  System.Windows.Forms.MessageBox.Show(oAcadObject.Handle.ToString(), "01_762")
               '  Exit For

            End If





         End If




         i += 1
         If i > iMax Then
            Exit For
         End If
      Next




      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()




   End Sub
   Public Shared Sub SeparateShapePgons()
      '  Const sRoot As String = "P:\2016\160590\Plan\Compil\BlueLines\P\"
      Const sRoot As String = "P:\2016\160470\Plan\Blue_Lines\P\"

      '   Dim o As Integer() = {1, 2, 3}
      Dim colMPgonsIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colMPgonID As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oShapeExpImp As ShapeExpImp
      Dim sShapeFileName As String


      Dim oMPgonTable As DMAcadExt.ODTable
      Dim oRec As Autodesk.Gis.Map.ObjectData.Record
      Dim oValue As Autodesk.Gis.Map.Utilities.MapValue
      Dim iTabaNumer As Integer
      Dim dtDate As Date
      Dim sDate As String
      Dim sDateNum As String

      Dim i As Integer
      Dim iFolderNum As Integer
      Dim iMin As Integer = 250 '1400
      Dim iMax As Integer = 400


      Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRoot)
      Dim hsDates As HashSet(Of String) = New HashSet(Of String)
      Dim oAcadObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim oaSubFolders() As IO.DirectoryInfo
      Dim dicRes As IDictionary(Of Integer, Date) = zzGetDateDictionary()
      If dicRes.Count > 0 Then


         oMPgonTable = New DMAcadExt.ODTable("BlueLines")
         oaSubFolders = oRootFolder.GetDirectories()
         For iIndex As Integer = 0 To oaSubFolders.GetUpperBound(0)
            hsDates.Add(oaSubFolders(iIndex).Name.Substring(2))
            '  DMAcadExt.AcadDocument.WriteMessage("oaSubFolders(iIndex).Name =" & CStr(oaSubFolders(iIndex).Name.Substring(2)))
         Next

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)

         colMPgonsIDs = DMAcadExt.AcadTransaction.GetAllDBObjectsByRxClass(DMAcadExt.AcadConst.AcadMPolygonName)
         colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         DMAcadExt.AcadDocument.WriteMessage("Total = " & CStr(colMPgonsIDs.Count))
         For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colMPgonsIDs
            If i >= iMin Then
               iFolderNum = 1
               oRec = oMPgonTable.GetODRecord(tAcObjID)
               oValue = oRec.Item(1)
               iTabaNumer = oValue.Int32Value
               If dicRes.TryGetValue(iTabaNumer, dtDate) Then
                  sDate = Format(dtDate, "yyyyMMdd")
                  Do
                     sDateNum = sDate & "_" & iFolderNum.ToString()
                     If hsDates.Contains(sDateNum) Then
                        iFolderNum += 1

                     Else
                        hsDates.Add(sDateNum)
                        Exit Do
                     End If
                  Loop

                  oRootFolder.CreateSubdirectory("P_" & sDateNum)
                  sShapeFileName = sRoot & "P_" & sDateNum & "\" & "P_" & sDate
                  '   DMAcadExt.AcadDocument.WriteMessage("sShapeFileName =" & CStr(sShapeFileName))
                  '  colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection(New Autodesk.AutoCAD.DatabaseServices.ObjectId() {tAcObjID})
                  colMPgonID = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection() ' {tAcObjID})
                  colMPgonID.Clear()
                  colMPgonID.Add(tAcObjID)

                  oAcadObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                  oShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
                  oShapeExpImp.AddShapeData(2)
                  System.Windows.Forms.MessageBox.Show(i.ToString() & ":" & sDateNum & vbCrLf & oAcadObject.Handle.ToString(), "01_772")
                  If oShapeExpImp.FromPoligons(colMPgonID) Then
                     oShapeExpImp.Exec()
                  Else


                     '  System.Windows.Forms.MessageBox.Show(oAcadObject.Handle.ToString(), "01_762")
                     '  Exit For
                  End If
               Else
                  System.Windows.Forms.MessageBox.Show(i.ToString() & ":" & iTabaNumer.ToString(), "01_780s")
               End If
            End If
            i += 1
            If i > iMax Then
               Exit For
            End If
         Next
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If
   End Sub
   Private Shared Function zzGetDateDictionary() As IDictionary(Of Integer, Date)
      Dim oValue As System.Object
      Dim dTabaNum As Double
      Dim iTabaNum As Integer

      Dim dtTabaDate As Date
      Dim dicRes As IDictionary(Of Integer, Date) = New Dictionary(Of Integer, Date)
      If DMCommon.ExcelImport.OpenExcelApp() Then
         Dim oaValues As System.Object(,) = DMCommon.ExcelImport.GetSelectionValue()
         For iInputRowIndex As Integer = 1 To oaValues.GetUpperBound(0)
            oValue = oaValues(iInputRowIndex, 1)
            dTabaNum = DMCommon.Functions.CDblN(oValue)
            iTabaNum = Convert.ToInt32(oValue)
            oValue = oaValues(iInputRowIndex, 4)
            dtTabaDate = DMCommon.Functions.CDateN(oValue)
            If iTabaNum <> 0 AndAlso dtTabaDate <> Date.MinValue Then
               dicRes.Add(iTabaNum, dtTabaDate)
            End If
         Next

      End If
      DMCommon.Debug.MsgBox("GetDateDictionary", dicRes.Count)
      Return dicRes
   End Function
   Public Shared Sub SetScheme()
      '  Const sShapeFile As String = "P:\2016\160590\Plan\Compil\BlueLines\P\P_19930624_Paste.shp"
      Const sShapeFile As String = "P:\2016\160590\Plan\Compil\BlueLines\Paste\P_19930624_Paste.shp"
      Const sShapeFolder As String = "P:\2016\160590\Plan\Compil\BlueLines\Paste"

      Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
      oFDO_Manager.SchemeModify(sShapeFolder, sShapeFile, "P_19930624_Paste")

      oFDO_Manager.Close()

      oFDO_Manager.Dispose()
      oFDO_Manager = Nothing
   End Sub

	Private Shared Sub zzDispLayerInfo(oMgLayerBase As MgLayerBase)
      Dim oAcMapLayer As Autodesk.Gis.Map.Platform.AcMapLayer
		Dim sMsg As String
		oAcMapLayer = DirectCast(oMgLayerBase, Autodesk.Gis.Map.Platform.AcMapLayer)

		sMsg = zzNN(oMgLayerBase.FeatureClassName) & vbCrLf
		sMsg &= zzNN(oMgLayerBase.FeatureSourceId) & vbCrLf
		sMsg &= zzNN(oMgLayerBase.GetClassName) & vbCrLf
		sMsg &= zzNN(oMgLayerBase.GetName) & vbCrLf
		sMsg &= zzNN(oMgLayerBase.GetType().ToString()) & vbCrLf

		'System.Windows.Forms.MessageBox.Show(sMsg, "01_477")
		DMAcadExt.AcadDocument.WriteDebugMessageN("MgLayerBase", "FeatureClassName: " & zzNN(oMgLayerBase.FeatureClassName) & vbCrLf, "FeatureSourceId: " & zzNN(oMgLayerBase.FeatureSourceId) & vbCrLf, "Name: " & zzNN(oMgLayerBase.GetName) & vbCrLf, oMgLayerBase.GetType())
	End Sub
   Private Function zzGetAdditionalInfo() As Dictionary(Of Integer, AdditBLineInfo)
      Dim oaValues As System.Object(,) = DMCommon.ExcelImport.GetSelectionValue()
      Dim tInfo As AdditBLineInfo
      Dim iPlanID As Integer
      Dim oValue As System.Object
      Dim dicAdditBLineInfo As Dictionary(Of Integer, AdditBLineInfo) = New Dictionary(Of Integer, AdditBLineInfo)()
      For iRowIndex As Integer = 1 To oaValues.GetUpperBound(0)
         oValue = oaValues(iRowIndex, 0)
         iPlanID = DirectCast(oValue, Integer)
         tInfo = New AdditBLineInfo()
         dicAdditBLineInfo.Add(iPlanID, tInfo)
      Next
      Return dicAdditBLineInfo
   End Function
	Private Shared Function zzNN(sVal As String) As String
		If sVal Is Nothing Then
			Return "<Nothing>"
		Else
			Return sVal
		End If
	End Function
End Class
