Option Explicit On
Option Strict On
Namespace TPlanGraph

   Public Class TplnLotTopology
      Inherits TPlanGraph.TplnTopology
      Public Sub New(ByVal sTopoName As String)
         MyBase.New(sTopoName)
      End Sub
      Public Overrides Sub LoadTopo()


			For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In MyBase.dcolPolygons
				' oLot = New TplnLot(oPolygon)

			Next

      End Sub

   End Class
End Namespace

