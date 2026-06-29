Public Class Form7
	Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles Button1.Click
		Const sPath As String = "M:\Dm_Work\Template's\LayerDef\Parcel.layer"

		Dim LayerDef As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		LayerDef.Load(sPath)
		Stop

	End Sub
End Class