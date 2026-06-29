Option Explicit On
Option Strict On
Public Class tmVertexSet
	Inherits SortedSet(Of Integer)
	Private miMyRingID As Integer
	Private miAdjRingID As Integer

	Private mdicVertices As IDictionary(Of Integer, Integer)
	Public Sub New(iMyRingID As Integer, iAdjRingID As Integer)
		mdicVertices = New Dictionary(Of Integer, Integer)
		miMyRingID = iMyRingID
		miAdjRingID = iAdjRingID
	End Sub
	Public Sub AddVertex(oVertex As tmVertex)
		MyBase.Add(oVertex.Index)
		mdicVertices.Add(oVertex.Node.ID, oVertex.Index)
	End Sub
	'Dim oVertexSet As SortedSet(Of Integer) = New SortedSet(Of Integer)
End Class
