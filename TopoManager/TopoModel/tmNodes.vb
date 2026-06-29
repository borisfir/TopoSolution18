Option Explicit On
Option Strict On
Public Class tmNodes
	Inherits List(Of tmNode)

	Public Function AddNodes(iRingA_ID As Integer, iRingB_ID As Integer, colIntersectPoints As Autodesk.AutoCAD.Geometry.Point3dCollection) As tmNode()
		Dim iStartIndex As Integer = Me.Count
		Dim iResIndex As Integer = 0
		Dim oaResNodes(colIntersectPoints.Count - 1) As tmNode

		For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colIntersectPoints

			oaResNodes(iResIndex) = New tmNode(iStartIndex + iResIndex, tPoint, iRingA_ID, iRingB_ID)
			Me.Add(oaResNodes(iResIndex))
			'	DMAcadExt.AcadDocument.WriteMessage("%%  " & CStr(iResIndex) & "|" & CStr(oaResNodes(iResIndex).ID) & ":" & DMAcadExt.TPlnPoint.DispPoint(tPoint))
			iResIndex += 1
		Next

		Return oaResNodes
	End Function
	Public Function AddPoint(iRingA_ID As Integer, iRingB_ID As Integer, tPoint As Autodesk.AutoCAD.Geometry.Point3d) As tmNode
		Return New tmNode(Me.Count, tPoint, iRingA_ID, iRingB_ID)
	End Function
	Public Function AddNodes(iRingA_ID As Integer, iRingB_ID As Integer, oaVertices() As tmVertex) As tmNode()
		Dim iStartIndex As Integer = Me.Count
		'	Dim iResIndex As Integer = 0
		Dim oVertex As tmVertex
		Dim oaResNodes(oaVertices.GetUpperBound(0)) As tmNode


		For iIndex As Integer = 0 To oaVertices.GetUpperBound(0)
			oVertex = oaVertices(iIndex)
			If oVertex IsNot Nothing Then
				oaResNodes(iIndex) = New tmNode(iStartIndex + iIndex, oVertex.Point3d, iRingA_ID, iRingB_ID)
				oVertex.ConnectNode(oaResNodes(iIndex))
				Me.Add(oaResNodes(iIndex))
			Else
				MessageBox.Show(CStr(oaVertices.GetUpperBound(0)) & vbCrLf & CStr(iIndex), "06_437")
			End If



		Next

		Return oaResNodes
	End Function

	Public Sub DebugWriteAll()
		Dim oNode As tmNode
		DMAcadExt.AcadDocument.WriteMessage("---*** N O D E S ***---  N=" & CStr(MyBase.Count))

		For iIndex As Integer = 0 To MyBase.Count - 1
			oNode = Me.Item(iIndex)
			oNode.DebugWrite()
		Next

	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	
End Class
