Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme
   Public Class tsRingUnion
      Inherits tsRing
      Public Sub New(ByVal iElementsCount As Integer)
         MyBase.New(iElementsCount)
      End Sub

      Public Sub UnionWith(oRing As tsRingUnion)

      End Sub

   End Class
End Namespace
