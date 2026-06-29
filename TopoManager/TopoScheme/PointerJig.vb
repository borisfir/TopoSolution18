Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Public Class PointerJig
   Inherits EntityJig
   Private mtCurrentPoint As Point2d
   Sub New(oPline As Polyline)
      MyBase.New(oPline)
   End Sub


   Protected Overrides Function Sampler(prompts As JigPrompts) As SamplerStatus
		Return SamplerStatus.OK
	End Function

   Protected Overrides Function Update() As Boolean
		Return True
	End Function
   Private Function zzGetPointerPline(tPointA As Point2d, tPointB As Point2d) As Polyline
      Dim oNewPline As Polyline = New Polyline(2)
      oNewPline.AddVertexAt(0, tPointA, 0.0, 0.0, 0.0)
      oNewPline.AddVertexAt(1, mtCurrentPoint, 0.0, 0.0, 0.0)
      oNewPline.AddVertexAt(1, tPointB, 0.0, 0.0, 0.0)


      Return oNewPline
   End Function
End Class
