Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Namespace TPlanGraph
	Public Class TplnOwnerTopo
		Private mtMapThemeData As DMAcadExt.MapThemeData
		Private mdicOwnerPgons As IDictionary(Of Integer, TplnOwnerPgon)
		Private msTopologyName As String
		Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
			mtMapThemeData = tMapThemeData
			mdicOwnerPgons = New Dictionary(Of Integer, TplnOwnerPgon)
			Dim oMainTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(mtMapThemeData.LineTopoName, mtMapThemeData.TopoName)
			If oMainTopology IsNot Nothing Then
				msTopologyName = oMainTopology.Name
			End If
			'	msTopologyName = mtMapThemeData.LineTopoName
			TplnOwnerPgon.Initialize(tMapThemeData)
		End Sub
		Public ReadOnly Property TopologyName As String
			Get
				Return msTopologyName
			End Get
		End Property
		Public Function LoadTopo() As Boolean
			Dim oMainTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(msTopologyName)

			If oMainTopology IsNot Nothing Then
				Dim oOwnerPgon As TplnOwnerPgon

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

					TplnOwnerPgon.CreateMainDataTable()
					DMAcadExt.AcadDocument.WriteDebugMessage("01_030: " & CStr(colPolygons.Count))

					For Each oPolygon As Polygon In colPolygons

						oOwnerPgon = New TplnOwnerPgon(oPolygon)

						If oOwnerPgon.Correct Then

							mdicOwnerPgons.Add(oOwnerPgon.TopoID, oOwnerPgon)
							oOwnerPgon.AddDataToMainTable()
							TplnOwner.AddOwnerPgon(oOwnerPgon)

						End If
						oPolygon.Dispose()
						oPolygon = Nothing

						'oLot.Terminate()
						oOwnerPgon = Nothing
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
		Public Function GetPgon(iTopoID As Integer) As TplnOwnerPgon
			Dim oOwnerPgon As TplnOwnerPgon = Nothing
			If Not mdicOwnerPgons.TryGetValue(iTopoID, oOwnerPgon) Then
				System.Windows.Forms.MessageBox.Show("TopoPgon # " & CStr(iTopoID) & " was not found", "TplnOwnerTopo - GetPgon")
			End If
			Return oOwnerPgon

		End Function
	End Class
End Namespace
