Imports Autodesk.AutoCAD.Geometry
Public Class tmPgonIntersect
	Public Sub New(oRingA As tmRing, oRingB As tmRing, i As Integer)

		Dim oCurveA As Curve2d = oRingA.Vertices.GetCurve()
		Dim oCurveB As Curve2d = oRingB.Vertices.GetCurve()
		Dim tPoint As Point2d
		Dim oInterval As Interval
		Dim tPointA As Point2d
		tPoint = oCurveA.StartPoint()
		DMAcadExt.AcadTransaction.InsertPoint(tPoint)
		tPointA = oCurveA.EndPoint()
		DMAcadExt.AcadTransaction.InsertPoint(tPointA)
		Dim oIntersector As CurveCurveIntersector2d = New CurveCurveIntersector2d(oCurveA, oCurveB)
		If oIntersector IsNot Nothing Then


			For iIndex As Integer = 0 To oIntersector.NumberOfIntersectionPoints() - 1
				tPoint = oIntersector.GetIntersectionPoint(iIndex)
				''''''''''''''''''	DMAcadExt.AcadTransaction.InsertPoint(tPoint)
			Next
			Dim oaIntersectionIntervals() As Interval = oIntersector.GetIntersectionRanges()
			Dim oaOverlapIntervals() As Interval
			For iIndex As Integer = 0 To oIntersector.OverlapCount - 1
				oaOverlapIntervals = oIntersector.GetOverlapRanges(iIndex)
				DMAcadExt.AcadDocument.WriteMessage("3.." & CStr(oaOverlapIntervals.GetUpperBound(0)))
			Next
			Dim daRes() As Double
			DMAcadExt.AcadDocument.WriteMessage("1.." & CStr(oIntersector.NumberOfIntersectionPoints))
			DMAcadExt.AcadDocument.WriteMessage("2.." & CStr(oaIntersectionIntervals.GetUpperBound(0)))
			DMAcadExt.AcadDocument.WriteMessage("4.." & CStr(oIntersector.OverlapCount))
			For iIndex As Integer = 0 To oaIntersectionIntervals.GetUpperBound(0)
				oInterval = oaIntersectionIntervals(iIndex)
				daRes = oInterval.GetBounds()

				For iIndex1 As Integer = 0 To daRes.GetUpperBound(0)
					DMAcadExt.AcadDocument.WriteMessage("5.." & CStr(daRes(iIndex1)))
				Next
				DMAcadExt.AcadDocument.WriteMessage("6.." & CStr(oInterval.LowerBound) & "<>" & CStr(oInterval.UpperBound))
			Next
		End If


	End Sub
	Public Sub New(oRingA As tmRing, oRingB As tmRing)
		Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		oRingA.Polyline.IntersectWith(oRingB.Polyline, Autodesk.AutoCAD.DatabaseServices.Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
		Dim colEdges As System.Collections.ObjectModel.Collection(Of tmEdge) = oRingA.AddIntersectPoints(colPoints, oRingB)
		For Each oEdge As tmEdge In colEdges
			oEdge.DebugWrite(1, "")
			oRingA.CreateDBPolyline(oEdge)
		Next

	End Sub
End Class
