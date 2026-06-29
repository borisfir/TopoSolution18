Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Public Class PointerPlineJig
	Inherits EntityJig
	Private mtaPoints() As Point3d
	Private miPLineVertUB As Integer
	Dim mtCurrentPoint, mtAcquiredPoint As Point3d
	Public Sub New(taPoints() As Point3d)
		MyBase.New(New Polyline())
		Dim ed As Editor = Application.DocumentManager.MdiActiveDocument.Editor
		mtaPoints = taPoints
		Dim oPolyline As Polyline = DirectCast(MyBase.Entity, Polyline)
		Dim iUB As Integer = taPoints.GetUpperBound(0)
		Dim tPointIndex As Integer = 0
		Dim tVertex As Point2d
		'''''''''	oPolyline.AddVertexAt(0, zzToPoint2d(taPoints(0)), 0.0, 0.0, 0.0)
		'''''''''''''''	oPolyline.AddVertexAt(1, Point2d.Origin, 0.0, 0.0, 0.0)
		oPolyline.Linetype = "DASHED2"
		oPolyline.LinetypeScale = 1000
		oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, 252S)
		'	oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
		For iIndex As Integer = 0 To taPoints.GetUpperBound(0)
			ed.WriteMessage("Input Pt: " & iIndex.ToString() & " " & taPoints(iIndex).ToString() & vbCrLf)
		Next
		If iUB = 0 Then
			miPLineVertUB = 1
		Else
			miPLineVertUB = 2 * iUB
		End If
		'08-9219749
		'052 222 7156

		For iPLineIndex As Integer = 0 To miPLineVertUB
			If iPLineIndex Mod 2 = 0 Then
				tVertex = zzToPoint2d(taPoints(tPointIndex))
				tPointIndex += 1
			Else
				tVertex = Point2d.Origin
			End If
			oPolyline.AddVertexAt(iPLineIndex, tVertex, 0.0, 0.0, 0.0)
		Next

	End Sub
	Public Function GetEntity() As Entity

		Return Entity
	End Function
	Public Function GetPoint() As Point3d

		Return mtCurrentPoint
	End Function

	Protected Overrides Function Sampler(oPrompts As JigPrompts) As SamplerStatus
		Dim oJigOpts As JigPromptPointOptions = New JigPromptPointOptions()
		'jigOpts.UserInputControls = UserInputControls.Accept3dCoordinates Or UserInputControls.NoZeroResponseAccepted Or UserInputControls.NoNegativeResponseAccepted

		oJigOpts.Message = "3 points"
		Dim oPromptResult As PromptPointResult = oPrompts.AcquirePoint(oJigOpts)
		Dim tPointTemp As Point3d = oPromptResult.Value

		If (tPointTemp <> mtCurrentPoint) Then
			mtCurrentPoint = tPointTemp
		Else
			Return SamplerStatus.NoChange
		End If

		If (oPromptResult.Status = PromptStatus.Cancel) Then
			Return SamplerStatus.Cancel
		Else
			Return SamplerStatus.OK
		End If
	End Function
	Protected Overrides Function Update() As Boolean
		Dim oPolyline As Polyline = DirectCast(Entity, Polyline)
		Dim tCurrentPoint2d As Point2d = zzToPoint2d(mtCurrentPoint)

		Try
			For iIndex As Integer = 1 To miPLineVertUB Step 2
				oPolyline.SetPointAt(iIndex, tCurrentPoint2d)
			Next

			Return True
		Catch oEx As System.Exception
			Return False
		End Try



	End Function
	Private Function zzToPoint2d(tPoint As Point3d) As Point2d
		Return New Point2d(tPoint.X, tPoint.Y)
	End Function
End Class
