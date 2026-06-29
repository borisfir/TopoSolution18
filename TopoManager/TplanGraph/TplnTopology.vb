Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Namespace TPlanGraph
   Public MustInherit Class TplnTopology
      Protected dsTopoName As String
      Protected doTopoModel As TopologyModel
		Protected dcolPolygons As PolygonCollection
      Public Sub New(ByVal sTopoName As String)
         dsTopoName = sTopoName
			doTopoModel = DMAcadExt.AcadMapApp.GetTopology(sTopoName)

      End Sub
      Public MustOverride Sub LoadTopo()
      Sub zzz()
         '  doTopoModel.FindPolygon()
      End Sub
   End Class
End Namespace