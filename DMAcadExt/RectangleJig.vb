Option Explicit On
Option Strict On

Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Public Class RectangleJig
	Inherits PointJig

	Private mtAcquiredPoint As Point3d
	Private moStartPoint As TPlnPoint
	Private mdWidth, mdHeight As Double
	Private mtaPoints(3) As Point2d
	Public Sub New(dWidth As Double, dHeight As Double)
		MyBase.New(New Polyline())
		mdWidth = dWidth
		mdHeight = dHeight
		Dim oPoint As TPlnPoint = New TPlnPoint(ptCurrentPoint)

		Dim oPolyline As Polyline = DirectCast(MyBase.Entity, Polyline)
		oPolyline.Linetype = "DASHED2"
		oPolyline.LinetypeScale = 100
		oPolyline.Closed = True


		'oPolyline.AddVertexAt(0, oPoint.AcGePoint, 0.0, 0.0, 0.0)
		'oPoint = oPoint.GetMoved(0.0, dHeight)
		'oPolyline.AddVertexAt(1, oPoint.AcGePoint, 0.0, 0.0, 0.0)
		'oPoint = oPoint.GetMoved(dWidth, 0.0)

		'oPolyline.AddVertexAt(2, oPoint.AcGePoint, 0.0, 0.0, 0.0)
		'oPoint = oPoint.GetMoved(0.0, -dHeight)
		'oPolyline.AddVertexAt(3, oPoint.AcGePoint, 0.0, 0.0, 0.0)
		For iIndex As Integer = 0 To 3
			oPolyline.AddVertexAt(iIndex, mtaPoints(iIndex), 0.0, 0.0, 0.0)
		Next

	End Sub


	Protected Overrides Function Update() As Boolean
		Dim oPolyline As Polyline = DirectCast(Entity, Polyline)
		'	Dim tCurrentPoint2d As Point2d = zzToPoint2d(mtCurrentPoint)

		Try
			For iIndex As Integer = 0 To UBound(mtaPoints)
				oPolyline.SetPointAt(iIndex, mtaPoints(iIndex))
			Next

			Return True
		Catch oEx As System.Exception
			Return False
		End Try

	End Function

	Protected Overrides Function Sampler(oPrompts As JigPrompts) As SamplerStatus
		Dim oJigOpts As JigPromptPointOptions = New JigPromptPointOptions()
		'jigOpts.UserInputControls = UserInputControls.Accept3dCoordinates Or UserInputControls.NoZeroResponseAccepted Or UserInputControls.NoNegativeResponseAccepted

		oJigOpts.Message = "N points"
		Dim oPromptResult As PromptPointResult = oPrompts.AcquirePoint(oJigOpts)
		Dim tPointTemp As Point3d = oPromptResult.Value


		If (TPlnPoint.Point3dTo2d(tPointTemp) <> ptCurrentPoint) Then
			ptCurrentPoint = TPlnPoint.Point3dTo2d(tPointTemp)
			moStartPoint = New TPlnPoint(ptCurrentPoint)
			ptCurrentPoint = moStartPoint.AcGePoint
			zzToPointArray()
		Else
			Return SamplerStatus.NoChange
		End If

		If (oPromptResult.Status = PromptStatus.Cancel) Then
			Return SamplerStatus.Cancel
		Else
			Return SamplerStatus.OK
		End If
	End Function
	Private Function zzToPoint2d(tPoint As Point3d) As Point2d
		Return New Point2d(tPoint.X, tPoint.Y)
	End Function
	Private Sub zzToPointArray()
		'	Dim taPoints(3) As Point2d
		mtaPoints(0) = moStartPoint.AcGePoint
		mtaPoints(1) = moStartPoint.GetMoved(0.0, mdHeight).AcGePoint
		mtaPoints(2) = moStartPoint.GetMoved(mdWidth, mdHeight).AcGePoint
		mtaPoints(3) = moStartPoint.GetMoved(mdWidth, 0.0).AcGePoint


	End Sub
End Class
