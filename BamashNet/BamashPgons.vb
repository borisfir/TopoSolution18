Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Public Class BamashPgons
	Inherits Dictionary(Of Integer, BamashPolygon)
	Implements TopoManager.TPlanGraph.PgonDictionary
	Private mdicBamashPgonsbyEntity As Dictionary(Of ObjectId, BamashPolygon)
	Public Sub New()
		'MyBase.New()
		mdicBamashPgonsbyEntity = New Dictionary(Of ObjectId, BamashPolygon)()
	End Sub
	Public Sub AddBamashPgon(ByVal oBamashPolygon As BamashPolygon)
		MyBase.Add(oBamashPolygon.TopoID, oBamashPolygon)
		mdicBamashPgonsbyEntity.Add(oBamashPolygon.CentroidAcObjID, oBamashPolygon)
	End Sub
	Public Function TryGetValueByEntity(tAcObjID As ObjectId, ByRef oBamashPolygon As BamashPolygon) As Boolean
		Return mdicBamashPgonsbyEntity.TryGetValue(tAcObjID, oBamashPolygon)
	End Function
	Public Sub Terminate() Implements TopoManager.TPlanGraph.PgonDictionary.Terminate
		For Each oBamashPolygon As BamashPolygon In MyBase.Values
			oBamashPolygon.Terminate()
		Next
		MyBase.Clear()
		mdicBamashPgonsbyEntity.Clear()
		mdicBamashPgonsbyEntity = Nothing
	End Sub
End Class

