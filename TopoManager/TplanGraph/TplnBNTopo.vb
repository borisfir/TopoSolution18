
Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Namespace TPlanGraph
	Public Class TplnBNTopo
		Private mtMapThemeData As DMAcadExt.MapThemeData
		Private mdicBNPgons As IDictionary(Of Integer, TplnBNPgon)
		Private msTopologyName As String

		Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
			mtMapThemeData = tMapThemeData
			mdicBNPgons = New Dictionary(Of Integer, TplnBNPgon)
			Dim oMainTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(mtMapThemeData.LineTopoName, mtMapThemeData.TopoName)
			If oMainTopology IsNot Nothing Then
				msTopologyName = oMainTopology.Name
			End If
			'	msTopologyName = mtMapThemeData.LineTopoName
			TplnBNPgon.Initialize(tMapThemeData)
		End Sub
		Public ReadOnly Property TopologyName As String
			Get
				Return msTopologyName
			End Get
		End Property
		Public Function LoadTopo() As Boolean
			Dim oMainTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(msTopologyName)

			If oMainTopology IsNot Nothing Then
				Dim oBNPgon As TplnBNPgon

				TplnOwner.Init()
				Try
					If oMainTopology.Status = Status.Closed Then
						oMainTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
					End If
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & "", "TplnProject - LoadTopo")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadTopo_01")
					Return False
				End Try
				'    DMCommon.ExcelLogB.Open()
				If oMainTopology.Status <> Status.Closed Then
					Dim colPolygons As PolygonCollection
					Try
						colPolygons = oMainTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_02")
						Return False
					End Try

					TplnBNPgon.CreateMainDataTable()
					DMAcadExt.AcadDocument.WriteDebugMessage("01_038: " & CStr(colPolygons.Count))

					For Each oPolygon As Polygon In colPolygons

						oBNPgon = New TplnBNPgon(oPolygon)

						If oBNPgon.Correct Then

							mdicBNPgons.Add(oBNPgon.TopoID, oBNPgon)
							oBNPgon.AddDataToMainTable()


						End If
						oPolygon.Dispose()
						oPolygon = Nothing

						'oLot.Terminate()
						oBNPgon = Nothing
					Next
					'   System.Windows.Forms.MessageBox.Show(CStr(mdicOwnerPgons.Count) & ":" & DMAcadExt.DMApp.AppID.ToString() & ":" & CStr(TplnOwnerPgon.MainDataTable.Rows.Count), "01_547b")

					'	colPolygons.Dispose()
					colPolygons = Nothing
					oMainTopology.Close()
					oMainTopology = Nothing



					Return True
				Else
					System.Windows.Forms.MessageBox.Show("'" & msTopologyName & "' Topology Is Nothing", "TplnProject - LoadLots")
					Return False
				End If
			Else
				System.Windows.Forms.MessageBox.Show("'" & msTopologyName & "' Topology Is Nothing", "TplnProject - LoadLots_3n")
				Return False
			End If
		End Function
		Public Function GetPgon(iTopoID As Integer) As TplnBNPgon
			Dim oBNPgon As TplnBNPgon = Nothing
			If Not mdicBNPgons.TryGetValue(iTopoID, oBNPgon) Then
				System.Windows.Forms.MessageBox.Show("TopoPgon # " & CStr(iTopoID) & " was not found", "TplnBNTopo - GetPgon")
			End If
			Return oBNPgon

		End Function

	End Class
End Namespace

