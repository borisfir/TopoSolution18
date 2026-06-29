Option Explicit On
Option Strict On
Imports DMAcadExt
Imports System.Drawing
Public Class PainZebra
	Private moGraph As Graphics
	Private mtRect As Rectangle
	Private mtColorZebra As ColorZebra
	Public Sub New(ByVal oGraph As Graphics, ByVal tRect As Rectangle)
		moGraph = oGraph
		mtRect = tRect

	End Sub
	Public Property ZebraScheme() As ColorZebra
		Get
			Return mtColorZebra
		End Get
		Set(ByVal tValue As ColorZebra)
			mtColorZebra = tValue
		End Set
	End Property
	Public Sub DrawL()
		Dim iStripUB As Integer = mtColorZebra.StripUB
		Dim oaPens(iStripUB) As Pen
		Dim iaStep(iStripUB) As Integer
		Dim tStrip, tSripNext As ColorStrip
		Dim dAngle As Double = mtColorZebra.Angle.AngleRad
		Dim dTnA As Double = Math.Tan(dAngle)
		Dim dXStep As Double = 9

		Dim tTopPoint, tBottomPoint As Point
		Dim iNext As Integer

		For iIndex As Integer = 0 To iStripUB
			iNext = (iIndex + 1) Mod (iStripUB + 1)
			tStrip = mtColorZebra.Strip(iIndex)
			tSripNext = mtColorZebra.Strip(iNext)
			oaPens(iIndex) = New Pen(tStrip.Color.FrameworkColor, tStrip.ScalingWidthSingle)
			iaStep(iIndex) = CInt(0.5 * (tStrip.ScalingWidthSingle + tSripNext.ScalingWidthSingle) / 0.78)
		Next
		tTopPoint = New Point(CInt(-mtRect.Height * dTnA), mtRect.Y)
		tBottomPoint = New Point(mtRect.X, mtRect.Height)

		Dim iStripIndex As Integer = 0
		Dim iTest As Integer = 0
		Do
			If iTest Mod 2 >= 0 Then
				moGraph.DrawLine(oaPens(iStripIndex), tTopPoint, tBottomPoint)
			End If
			iTest += 1
			tTopPoint.X = tTopPoint.X + CInt(iaStep(iStripIndex))
			If tTopPoint.X - CInt(0.5 * oaPens(iStripIndex).Width) > mtRect.Width Then Exit Do
			tBottomPoint.X = tBottomPoint.X + CInt(iaStep(iStripIndex))
			iStripIndex = (iStripIndex + 1) Mod (iStripUB + 1)
		Loop
	End Sub
	Public Sub Draw()
		Dim iStripUB As Integer = mtColorZebra.StripUB
		Dim oaPens(iStripUB) As Pen
		Dim oaBrushes(iStripUB) As Brush

		Dim iaStep(iStripUB) As Integer
		Dim tStrip As ColorStrip
		Dim dAngle As Double = mtColorZebra.Angle.AngleRad
		Dim dTnA As Double = Math.Tan(dAngle)
		Dim dXStep As Double = 9

		Dim tTopLeftPoint, tTopRightPoint, tBottomLeftPoint, tBottomRightPoint As Point


		For iIndex As Integer = 0 To iStripUB
			tStrip = mtColorZebra.Strip(iIndex)
			iaStep(iIndex) = CInt(tStrip.ScalingWidth)
			oaPens(iIndex) = New Pen(tStrip.Color.FrameworkColor)
			oaBrushes(iIndex) = oaPens(iIndex).Brush
		Next
		tTopLeftPoint = New Point(CInt(-mtRect.Height * dTnA), mtRect.Y)

		tBottomLeftPoint = New Point(mtRect.X, mtRect.Height)

		Dim iStripIndex As Integer = 0
		Dim iTest As Integer = 0
		Dim oBrush As Brush
		Dim oPen As Pen
		Dim oPgonPoints(3) As Point
		Do
			oPen = oaPens(iStripIndex)
			tTopRightPoint = New Point(tTopLeftPoint.X + iaStep(iStripIndex), tTopLeftPoint.Y)
			tBottomRightPoint = New Point(tBottomLeftPoint.X + iaStep(iStripIndex), tBottomLeftPoint.Y)
			oPgonPoints(0) = tTopLeftPoint
			oPgonPoints(1) = tTopRightPoint
			oPgonPoints(2) = tBottomRightPoint
			oPgonPoints(3) = tBottomLeftPoint
			oBrush = oPen.Brush
			If iTest < 12 Then
				moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Winding)
			End If
			tTopLeftPoint = New Point(tTopRightPoint.X + 1, tTopRightPoint.Y)

			tBottomLeftPoint = New Point(tBottomRightPoint.X + 1, tBottomRightPoint.Y)
			If tTopLeftPoint.X > mtRect.X + mtRect.Width Then
				Exit Do
			End If
			iTest += 1

			iStripIndex = (iStripIndex + 1) Mod (iStripUB + 1)
		Loop
	End Sub
End Class

