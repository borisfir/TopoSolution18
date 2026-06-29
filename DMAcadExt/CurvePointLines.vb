Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices


Public Structure CurvePointLine
   Private mtCurvePoint As CurvePoint
   Private mtLineObjID As ObjectId
   Public Sub New(tCurvePoint As CurvePoint, tLineObjID As ObjectId)
      mtCurvePoint = tCurvePoint
      mtLineObjID = tLineObjID
   End Sub
   Public Property CurvePoint As CurvePoint
      Get
         Return mtCurvePoint
      End Get
      Set(tValue As CurvePoint)
         mtCurvePoint = tValue
      End Set
   End Property
   Public Property LineObjID As ObjectId
      Get
         Return mtLineObjID
      End Get
      Set(tValue As ObjectId)
         mtLineObjID = tValue
      End Set
   End Property
End Structure
Public Class CurvePointLines
   Inherits HashSet(Of CurvePointLine)
   Public Sub New()

   End Sub

   Public Function AddItem(tCurvePoint As CurvePoint, tLineObjID As ObjectId) As Boolean
      Return MyBase.Add(New CurvePointLine(tCurvePoint, tLineobjID))
   End Function
End Class
