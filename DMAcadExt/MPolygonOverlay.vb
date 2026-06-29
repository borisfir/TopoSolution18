Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class MPolygonOverlay
	Inherits MPolygon
	Private miFeatureID As Integer
	Private miSourceID As Integer
	Private miOverlayID As Integer
	Private miOverlayID_Add As Integer
   Private mdicEntities As ObjectIdCollection
   Private mtFirstHandle As Handle = New Handle()
	Public Property FeatureID As Integer
		Get
			Return miFeatureID
		End Get
		Set(iValue As Integer)
			miFeatureID = iValue
		End Set
	End Property

	Public Property SourceID As Integer
		Get
			Return miSourceID
		End Get
		Set(iValue As Integer)
			miSourceID = iValue
		End Set
	End Property
	Public Property OverlayID As Integer
		Get
			Return miOverlayID
		End Get
		Set(iValue As Integer)
			miOverlayID = iValue
		End Set
	End Property
	Public Property OverlayID_Add As Integer
		Get
			Return miOverlayID_Add
		End Get
		Set(iValue As Integer)
			miOverlayID_Add = iValue
		End Set
	End Property
	Public Sub New(oEntity As ObjectId)
		mdicEntities = New ObjectIdCollection()
		mdicEntities.Add(oEntity)
	End Sub
	Public Sub AddEntity(oEntity As ObjectId)

		mdicEntities.Add(oEntity)
	End Sub
	Public ReadOnly Property Entities As ObjectIdCollection
		Get
			Return mdicEntities
		End Get
   End Property
	Public Property FirstHandle As Handle
		Get
			Return mtFirstHandle
		End Get
		Set(tValue As Handle)

			mtFirstHandle = tValue


		End Set
	End Property
	Public Function GetAllBulgeVertices() As BulgeVertexCollection
		Dim oMPolygonLoop As MPolygonLoop
		Dim oResBulgeVertexCollection As BulgeVertexCollection = Nothing
		For iLoopIndex As Integer = 0 To Me.NumMPolygonLoops - 1
			oMPolygonLoop = Me.GetMPolygonLoopAt(iLoopIndex)
			If iLoopIndex = 0 Then
				oResBulgeVertexCollection = oMPolygonLoop
			Else
				For iIndex As Integer = 0 To oMPolygonLoop.Count - 1
					oResBulgeVertexCollection.Add(oMPolygonLoop.Item(iIndex))
				Next
			End If
		Next


		Return oResBulgeVertexCollection
	End Function
End Class
