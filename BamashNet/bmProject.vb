Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports TopoManager

Public Class bmProject
	Public Shared Sub CalculateAAA()
		Dim sTopoName As String = TopoDefs.GetTopoName(New TopoDefID(TPlanGraph.enTopoPurpose.Bamash))
		AcadTransaction.Start()
		AcadDocument.OpenLog()
		BamashPolygon.Initialize()

		Dim oBamashTopology As TopologyModel = TopoManager.Common.GetTopology(sTopoName)
		If oBamashTopology IsNot Nothing Then
			Try
				If oBamashTopology.Status = Status.Closed Then
					oBamashTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & sTopoName & "'", "TplnProject - LoadLots")
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_01")

			End Try
		End If
		Dim oBamashPolygon As BamashPolygon
		Dim dicLots As TPlanGraph.TplnLots

		If oBamashTopology.Status <> Status.Closed Then
			Dim oPoligons As PolygonCollection
			Try
				oPoligons = oBamashTopology.GetPolygons()
			Catch oMapEx As Autodesk.Gis.Map.MapException
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_02")
				Exit Sub
			End Try
			dicLots = New TPlanGraph.TplnLots
			BamashPolygon.OpenMainDataTable()
			For Each oPolygon As Polygon In oPoligons
				oBamashPolygon = New BamashPolygon(oPolygon)
				If oBamashPolygon.Correct Then
					oBamashPolygon.AddDataToMainTable()
				End If
			Next
			oBamashTopology.Close()

		End If

		oBamashTopology = Nothing

		AcadTransaction.Terminate()
		AcadDocument.CloseMessage()
		AcadDocument.CloseLog()

	End Sub

End Class
