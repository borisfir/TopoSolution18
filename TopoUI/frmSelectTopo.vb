Imports Autodesk.Gis.Map.Topology
Imports System.Data


Public Class frmSelectTopo

	Private Sub zz()
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies

		Dim sComText As String = "SELECT DISTINCT TopoName FROM dbo.MapThemeData WHERE (GraphTypeID = 1)"
		Dim sTopoName As String
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				'	DMCommon.Debug.ExcelLog.SetDataTable(0, "moProjectPlanTable", moProjectPlanTable)
				sTopoName = oDataReader.GetString(0)
				If oTopos.Exists(sTopoName) Then
					cmbTopoList.Items.Add(sTopoName)
				End If
				'	oItem = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))


			End While

			oDataReader.Close()
			'	System.Windows.Forms.MessageBox.Show(CStr(oDataReader.RecordsAffected), "18_998 DataReader.Close")
		End If





	End Sub



End Class