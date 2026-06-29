Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TPlanGraph


	Public MustInherit Class TplnDissolvePgon
		Inherits TPlanGraph.TplnTopoPgon
		Private mbPgonExists As Boolean
		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon)
			mbPgonExists = True
		End Sub
		Public Sub New(ByVal tBlockAcObjId As ObjectId)
			MyBase.New(0)
			mbPgonExists = False
			dtCentroidAcObjID = tBlockAcObjId
		End Sub
		Public Sub New(ByVal iTopoID As Integer)
			MyBase.New(iTopoID)
			mbPgonExists = False
		End Sub
		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, iFeatureID As Integer, tCentroidAcObjID As ObjectId)
			MyBase.New(oPolygon, iFeatureID, tCentroidAcObjID)
		End Sub

		Public MustOverride ReadOnly Property BlockExists As Boolean


		Public Property PgonExists As Boolean
			Get
				Return mbPgonExists
			End Get
			Set(bValue As Boolean)
				mbPgonExists = bValue
			End Set
		End Property


		Protected MustOverride Overrides ReadOnly Property AddBlockAttribIndex As Integer()


		Protected MustOverride Overrides ReadOnly Property BlockAttribIndex As Integer()

		Public MustOverride Overrides Sub Terminate()


	End Class
End Namespace
