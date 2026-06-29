Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnArc
	Private mtCenter As Point2d
	Private mdRadius As Double

	Private mdStartAngle As Double
	Private mdEndAngle As Double

	Private mtStartVector As Vector2d
	Private mtEndVector As Vector2d
	Private mbIsClockWise As Boolean

	Public Sub New(ByVal tCenter As Point2d, ByVal dRadius As Double, ByVal dStartAngle As Double, ByVal dEndAngle As Double)
		mtCenter = tCenter
		mdRadius = dRadius
		mdStartAngle = dStartAngle
		mdEndAngle = dEndAngle
		mbIsClockWise = True
	End Sub
	Public Sub New(ByVal tCenter As Point2d, ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d, ByVal bClockWise As Boolean)
		mtCenter = tCenter
		mtStartVector = New Vector2d(tStartPoint.X - tCenter.X, tStartPoint.Y - tCenter.Y)
		mtEndVector = New Vector2d(tEndPoint.X - tCenter.X, tEndPoint.Y - tCenter.Y)
		mdRadius = mtStartVector.Length
		mbIsClockWise = bClockWise
		'	If mbIsClockWise Then
		mdStartAngle = mtStartVector.Angle
		mdEndAngle = mtEndVector.Angle
		'	Else
		'	mdStartAngle = mtEndVector.Angle
		'	mdEndAngle = mtStartVector.Angle
		'	End If


	End Sub
	Public Function GetInnerPoints(ByVal dTolerance As Double) As Point2d()
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / mdRadius)
		Dim dToDegree As Double = 180 / Math.PI
		Dim dTotalAngle As Double
		If mbIsClockWise Then
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Else
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle > 0.0 Then dTotalAngle -= Math.PI + Math.PI
		End If


		'	If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(Math.Abs(dTotalAngle) / dTolerAngle))
		Dim oPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim taRes(iPart - 1) As Point2d
		dTolerAngle = dTotalAngle / iPart
		'	DMAcadExt.AcadDocument.WriteMessage("InnerAngles=" & CStr(mdStartAngle * dToDegree) & "," & CStr(mdEndAngle * dToDegree) & " Toler" & CStr(dTolerAngle * dToDegree) & " Total" & CStr(dTotalAngle * dToDegree))
		For iIndex As Integer = 1 To iPart
			dAngle = mdStartAngle + iIndex * dTolerAngle
			tPoint = New Autodesk.AutoCAD.Geometry.Point2d(mtCenter.X + mdRadius * Math.Cos(dAngle), mtCenter.Y + mdRadius * Math.Sin(dAngle))
			'		DMAcadExt.AcadDocument.WriteMessage("Inner=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
			taRes(iIndex - 1) = tPoint
		Next
		Return taRes
	End Function

	Public Function GetAcadArc() As Arc
		If mbIsClockWise Then
			Return New Arc(TPlnPoint.Point2dTo3d(mtCenter), mdRadius, mdStartAngle, mdEndAngle)
		Else
			Return New Arc(TPlnPoint.Point2dTo3d(mtCenter), mdRadius, mdEndAngle, mdStartAngle)
		End If
	End Function
	Public Property Center() As Point2d
		Get
			Return mtCenter
		End Get
		Set(ByVal tValue As Point2d)
			mtCenter = tValue
		End Set
	End Property
	Public Property StartAngle() As Double
		Get
			Return mdStartAngle
		End Get
		Set(ByVal dValue As Double)
			mdStartAngle = dValue
		End Set
	End Property

	Public Property EndAngle() As Double
		Get
			Return mdEndAngle
		End Get
		Set(ByVal dValue As Double)
			mdEndAngle = dValue
		End Set
	End Property


	Public Property Radius() As Double
		Get
			Return mdRadius
		End Get
		Set(ByVal dValue As Double)
			mdRadius = dValue
		End Set
	End Property

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
