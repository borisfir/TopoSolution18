Option Explicit On
Option Strict On
Imports OSGeo.MapGuide
Imports Autodesk.Gis.Map.Platform
'Imports Topobase.Data.Provider.FDO.FeatureServiceWrapper
Imports OSGeo.MapGuide.Schema.FeatureSource


Public Class GeoSpatPl
	Private Shared msResource As String = "Library://feature3.FeatureSource"
	Sub Ex_DefiningVectorfeatureSource()
		' Get the services
		Dim SDFpath As String = "D:\aWork\Geo\ParcelTopo.sdf"
		Dim sParcelShapePath As String = "D:\aWork\ArcGIS\D\Parcel\topoParcels.shp"
		Dim sSHPPath As String = "\\NETAPP\DM_APP\Tababuild\ProjectsNet\Shape\shp\p2792.shp"
		Dim rs As MgResourceService
		rs = CType(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)
		Dim fs As MgFeatureService
		fs = CType(AcMapServiceFactory.GetService(MgServiceType.FeatureService), MgFeatureService)
		Dim fsId As MgResourceIdentifier = New MgResourceIdentifier(msResource)

		' Create the feature source definition with a required
		' parameter of File and an optional parameter of ReadOnly
		''
		Dim xmlString As String

		Dim fsType As OSGeo.MapGuide.Schema.FeatureSource.FeatureSourceType = New FeatureSourceType()
		fsType.Provider = "OSGeo.SDF.3.6" 'frmDataConnect.msSHPProviderName	 
		Dim param As NameValuePairType = New NameValuePairType()

		param.Name = "File"
		param.Value = SDFpath  'sSHPPath '
		Dim param2 As NameValuePairType = New NameValuePairType()
		param2.Name = "ReadOnly"
		param2.Value = "false"
		fsType.Parameter = New NameValuePairType() {param, param2}


		' Serialize the feature source object model to xml string
		Using writer As System.IO.StringWriter = New System.IO.StringWriter()

			Dim xs As System.Xml.Serialization.XmlSerializer = New System.Xml.Serialization.XmlSerializer(fsType.GetType())
			xs.Serialize(writer, fsType)
			xmlString = writer.ToString()
		End Using

		' Convert the Unicode string to UTF8 bytes for Resource Service
		Dim unicodeBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(xmlString)
		Dim utf8Bytes = System.Text.Encoding.Convert(System.Text.Encoding.Unicode, System.Text.Encoding.UTF8, unicodeBytes)
		' Create a byte reader containing the XML feature
		' source definition. Store the definition in the repository

		Dim xmlSource As MgByteSource = New MgByteSource(utf8Bytes, utf8Bytes.Length)
		rs.SetResource(fsId, xmlSource.GetReader(), Nothing)


	End Sub
	Public Shared Sub DeleteRecource()
		Dim rs As MgResourceService
		Dim resource As OSGeo.MapGuide.MgResourceIdentifier

		rs = CType(AcMapServiceFactory.GetService(MgServiceType.ResourceService), MgResourceService)
		resource = New MgResourceIdentifier(msResource)
		rs.DeleteResource(resource)
	End Sub

End Class
