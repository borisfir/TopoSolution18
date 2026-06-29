Option Explicit On
Option Strict On
Public Class GridLayout

	Private mdicGridColDefs As Generic.Dictionary(Of String, GridColDef)
	Private mbDirty As Boolean
	Public Function TryGetValue(sFieldName As String, ByRef oGridColDef As GridColDef) As Boolean
		Return mdicGridColDefs.TryGetValue(sFieldName, oGridColDef)
	End Function
	Public Sub LoadDefs(ByVal iObjectID As Integer)
		'mdicGridColDefs = dmResource.GetDBResource(iObjectID)

	End Sub

	Public Sub LoadSettings(sSettingFileName As String)
		Dim oFile As IO.FileInfo = New IO.FileInfo(sSettingFileName)
		If oFile.Exists Then
			Dim oXmlReader As Xml.XmlTextReader
			Dim oXMLDoc As Xml.XmlDocument
			'Dim oXMLColumns As Xml.XmlElement
			Dim oXMLColumns, oXMLColumn As Xml.XmlNode
			Dim oXmlAttributeCollection As Xml.XmlAttributeCollection
			Dim oXmlAttribute As Xml.XmlAttribute
			Dim sAttribName, sAttribValue As String

			Dim sColName As String
			Dim oGridColDef As GridColDef = Nothing
			Dim iColumnWidth As Integer
			oXmlReader = New Xml.XmlTextReader(sSettingFileName)
			oXMLDoc = New Xml.XmlDocument
			oXMLDoc.Load(oXmlReader)
			Dim oRootElement As Xml.XmlElement = oXMLDoc.DocumentElement()
			oXMLColumns = oRootElement.FirstChild
			For Each oXMLColumn In oXMLColumns.ChildNodes
				sColName = oXMLColumn.Name
				If mdicGridColDefs.TryGetValue(sColName, oGridColDef) Then

					oXmlAttributeCollection = oXMLColumn.Attributes
					For Each oXmlAttribute In oXmlAttributeCollection
						sAttribName = oXmlAttribute.Name
						sAttribValue = oXmlAttribute.Value
						Select Case sAttribName
							Case "Caption"
							Case "Width"
								Try
									iColumnWidth = Convert.ToInt32(sAttribValue)
									oGridColDef.ColumnWidthSetting = iColumnWidth
								Catch oEx As Exception

								End Try

						End Select

					Next
				End If
			Next
		End If

	End Sub
	Public Sub SetColumnWidthSetting(sFieldName As String, iColWidth As Integer)
		Dim oGridColDef As GridColDef = Nothing
		Dim iTest As Integer
		If mdicGridColDefs.TryGetValue(sFieldName, oGridColDef) Then
			oGridColDef.ColumnWidthSetting = iColWidth
			mbDirty = True
		End If
		If mdicGridColDefs.TryGetValue(sFieldName, oGridColDef) Then
			iTest = oGridColDef.ColumnWidthSetting

		End If
	End Sub

	Public Sub SaveSettingsIfDirty(sSettingFileName As String)
		If mbDirty Then
			Dim oXMLDoc As Xml.XmlDocument
			Dim oXMLColumns, oXMLColumn As Xml.XmlNode
			Dim oXmlAttributeCollection As Xml.XmlAttributeCollection
			Dim oXmlAttribute As Xml.XmlAttribute
			oXMLDoc = New Xml.XmlDocument()
			oXMLDoc.LoadXml("<Settings></Settings>")
			Dim oRootElement As Xml.XmlElement = oXMLDoc.DocumentElement()
			oXMLColumns = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Columns", String.Empty)

			For Each oGridColDef As GridColDef In mdicGridColDefs.Values
				If oGridColDef.HasSetting Then
					oXMLColumn = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, oGridColDef.FieldName, String.Empty)
					oXMLColumns.AppendChild(oXMLColumn)
					oXmlAttributeCollection = oXMLColumn.Attributes
					If oGridColDef.HeaderText IsNot Nothing Then
						oXmlAttribute = oXMLDoc.CreateAttribute("Caption")
						oXmlAttribute.Value = oGridColDef.HeaderText
						oXmlAttributeCollection.Append(oXmlAttribute)
					End If
					If oGridColDef.ColumnWidthSetting <> 0 Then
						oXmlAttribute = oXMLDoc.CreateAttribute("Width")
						oXmlAttribute.Value = Convert.ToString(oGridColDef.ColumnWidthSetting)
						oXmlAttributeCollection.Append(oXmlAttribute)
					End If

				End If

				'	oXMLColumn.InnerText = "abcdefgh"
			Next
			oRootElement.AppendChild(oXMLColumns)
			Try
				oXMLDoc.Save(sSettingFileName)
			Catch oEx As Exception

			End Try
		End If



	End Sub

	 
	Public Sub New()

	End Sub
End Class
