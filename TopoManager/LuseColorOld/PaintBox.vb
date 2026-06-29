Option Explicit On
Option Strict On
Imports DMAcadExt
Public Class PaintBox
	Private moGraph As Graphics
	Private mtRectPoint As PointExt
	Private mtColorScheme As ColorScheme
	Public Sub New(ByRef oGraph As Graphics, ByVal tSize As Size)
		moGraph = oGraph
		mtRectPoint = New PointExt(tSize.Width, tSize.Height)
	End Sub
	Public Property ColorScheme() As ColorScheme
		Get
			Return mtColorScheme
		End Get
		Set(ByVal tValue As ColorScheme)
			mtColorScheme = tValue
		End Set
	End Property
	Public Sub Draw()
		If mtColorScheme.HasHatch Then
			Me.DrawHatch(mtColorScheme.Hatch)
		Else
			moGraph.Clear(SystemColors.Window)
		End If
		If mtColorScheme.HasZebra Then
			Me.DrawZebra(mtColorScheme.Zebra)
		End If
		If mtColorScheme.HasBorder Then
			Me.DrawBorder(mtColorScheme.Border)
		End If

	End Sub
	Public Sub DrawZebraL(ByVal tColorZebra As ColorZebra)
		Dim iStripUB As Integer = tColorZebra.StripUB
		Dim oaPens(iStripUB) As Pen
		Dim iaStep(iStripUB) As Integer
		Dim tStrip, tSripNext As ColorStrip
		Dim dAngle As Double = tColorZebra.Angle.AngleRad
		Dim dTnA As Double = Math.Tan(dAngle)
		Dim dXStep As Double = 9

		Dim tTopPoint, tBottomPoint As Point
		Dim iNext As Integer

		For iIndex As Integer = 0 To iStripUB
			iNext = (iIndex + 1) Mod (iStripUB + 1)
			tStrip = tColorZebra.Strip(iIndex)
			tSripNext = tColorZebra.Strip(iNext)
			oaPens(iIndex) = New Pen(tStrip.Color.FrameworkColor, tStrip.ScalingWidthSingle)
			iaStep(iIndex) = CInt(0.5 * (tStrip.ScalingWidthSingle + tSripNext.ScalingWidthSingle) / 0.78)
		Next
		tTopPoint = New Point(CInt(-mtRectPoint.Y * dTnA), 0)
		tBottomPoint = New Point(0, mtRectPoint.Y)

		Dim iStripIndex As Integer = 0
		Dim iTest As Integer = 0
		Do
			If iTest Mod 2 >= 0 Then
				moGraph.DrawLine(oaPens(iStripIndex), tTopPoint, tBottomPoint)
			End If
			iTest += 1
			tTopPoint.X = tTopPoint.X + CInt(iaStep(iStripIndex))
			If tTopPoint.X - CInt(0.5 * oaPens(iStripIndex).Width) > mtRectPoint.X Then Exit Do
			tBottomPoint.X = tBottomPoint.X + CInt(iaStep(iStripIndex))
			iStripIndex = (iStripIndex + 1) Mod (iStripUB + 1)
		Loop
	End Sub
	Private Sub zzDrawHatchLineSource(ByVal tDMHatch As DMHatch)
		Dim iTest As Integer = 0
		Dim iLineWeight As Integer = CInt(0.1 * tDMHatch.LineWeightInt) + 1
		If iLineWeight < 1 Then iLineWeight = 1
		Dim oPen As Pen = New Pen(tDMHatch.PatternColor.FrameworkColor, iLineWeight)
		Dim tAngle As LineAngle = tDMHatch.Angle
		Dim dTnA As Double = tDMHatch.Angle.DirectTan
		Dim iStep As Integer = CInt(10.0 * tDMHatch.ScalingPatternScale)
		If iStep = 0 Then iStep = 1
		PointExt.XDirection = tAngle.XDirection
		Dim tTopPoint, tBottomPoint As PointExt
		Dim tTopLeftLimitedPoint As PointExt
		If tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantII Then
			tTopPoint = New PointExt(New PointExt(0, 0), -CInt(mtRectPoint.NoneDirectCoord * dTnA))
			tBottomPoint = New PointExt(New PointExt(0, 0), 0, mtRectPoint.NoneDirectCoord)
			tTopLeftLimitedPoint = New PointExt(tTopPoint, mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA), 0)

		ElseIf tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI Then
			tTopPoint = New PointExt(0, 0)
			tBottomPoint = New PointExt(New PointExt(0, 0), -CInt(mtRectPoint.NoneDirectCoord * dTnA), mtRectPoint.NoneDirectCoord)
			tTopLeftLimitedPoint = New PointExt(tTopPoint, tTopPoint.DirectCoord + mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA))

		End If
		AcadDocument.WriteMessage("Angle=" & CStr(tAngle.AngleDegree) & "," & CStr(dTnA) & "," & CStr(tAngle.XDirection) & "; (" & tTopPoint.Coordinates & ") : (" & tBottomPoint.Coordinates & ") : (" & tTopLeftLimitedPoint.Coordinates & ")")

		Do While tTopPoint.DirectCoord <= tTopLeftLimitedPoint.DirectCoord
			moGraph.DrawLine(oPen, tTopPoint.FWPoint, tBottomPoint.FWPoint)
			If iTest = 90 Then Exit Do
			If iTest <= 5 Then
				AcadDocument.WriteMessage("(" & tTopPoint.Coordinates & ") : (" & tBottomPoint.Coordinates & ") ")

			End If
			tTopPoint = New PointExt(tTopPoint, iStep)
			tBottomPoint = New PointExt(tBottomPoint, iStep)
			iTest += 1
		Loop
	End Sub
	Private Sub zzDrawHatchLine(ByVal tDMHatch As DMHatch)
		Dim iLineWeight As Integer = CInt(0.1 * tDMHatch.LineWeightInt) + 1
		Dim tAngle As LineAngle = tDMHatch.Angle
		Dim iStep As Integer = CInt(10.0 * tDMHatch.ScalingPatternScale)
		If iLineWeight < 1 Then iLineWeight = 1
		If iStep = 0 Then iStep = 1
		zzDrawLine(tDMHatch.PatternColor.FrameworkColor, iLineWeight, iStep, tAngle)
	End Sub
	Private Sub zzDrawLine(ByVal tColor As System.Drawing.Color, ByVal iLineWeight As Integer, ByVal iStep As Integer, ByVal tAngle As LineAngle)
		Dim iTest As Integer = 0
		Dim oPen As Pen = New Pen(tColor, iLineWeight)
		Dim dTnA As Double = tAngle.DirectTan
		Dim tTopPoint, tBottomPoint As PointExt
		Dim tTopLeftLimitedPoint As PointExt

		PointExt.XDirection = tAngle.XDirection
	
		If tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantII Then
			tTopPoint = New PointExt(New PointExt(0, -iLineWeight), -CInt(mtRectPoint.NoneDirectCoord * dTnA))
			tBottomPoint = New PointExt(New PointExt(0, -iLineWeight), 0, mtRectPoint.NoneDirectCoord + iLineWeight)
			tTopLeftLimitedPoint = New PointExt(tTopPoint, mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA) + iLineWeight, 0)

		ElseIf tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI Then
			tTopPoint = New PointExt(0, -iLineWeight)	  '= New PointExt(0, -iLineWeight)  24/05/09
			tBottomPoint = New PointExt(New PointExt(0, 0), -CInt(mtRectPoint.NoneDirectCoord * dTnA) + iLineWeight, mtRectPoint.NoneDirectCoord + iLineWeight)
			tTopLeftLimitedPoint = New PointExt(tTopPoint, tTopPoint.DirectCoord + mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA) + iLineWeight)

		End If
		'	AcadDocument.WriteMessage("Angle=" & CStr(tAngle.AngleDegree) & "," & CStr(dTnA) & "," & CStr(tAngle.XDirection) & "; (" & tTopPoint.Coordinates & ") : (" & tBottomPoint.Coordinates & ") : (" & tTopLeftLimitedPoint.Coordinates & ")")

		Do While tTopPoint.DirectCoord <= tTopLeftLimitedPoint.DirectCoord
			moGraph.DrawLine(oPen, tTopPoint.FWPoint, tBottomPoint.FWPoint)
			If iTest = 90 Then Exit Do
			tTopPoint = New PointExt(tTopPoint, iStep)
			tBottomPoint = New PointExt(tBottomPoint, iStep)
			iTest += 1
		Loop
	End Sub
	Private Sub zzDrawHatchNet(ByVal tDMHatch As DMHatch)
		Dim iLineWeight As Integer = CInt(0.1 * tDMHatch.LineWeightInt) + 1
		Dim tAngle As LineAngle = tDMHatch.Angle
		Dim tPerpAngle As LineAngle = tAngle.GetPerpendicular()
		Dim iStep As Integer = CInt(10.0 * tDMHatch.ScalingPatternScale)
		Dim tColor As System.Drawing.Color = tDMHatch.PatternColor.FrameworkColor
		If iLineWeight < 1 Then iLineWeight = 1
		If iStep = 0 Then iStep = 1
		zzDrawLine(tColor, iLineWeight, iStep, tAngle)
		zzDrawLine(tColor, iLineWeight, iStep, tPerpAngle)
	End Sub
	Public Sub DrawZebraM(ByVal tColorZebra As ColorZebra)
		Dim iStripUB As Integer = tColorZebra.StripUB
		'	Dim oaPens(iStripUB) As Pen
		Dim oaBrushes(iStripUB) As SolidBrush

		Dim iaStep(iStripUB) As Integer
		Dim tStrip As ColorStrip
		Dim tAngle As LineAngle = tColorZebra.Angle
		Dim dTnA As Double = tAngle.Tangent
		Dim dXStepAAA As Double = 9

		Dim tTopLeftPoint, tTopRightPoint, tBottomLeftPoint, tBottomRightPoint As Point
		Dim tTopLeftLimitedPoint As Point
		For iIndex As Integer = 0 To iStripUB
			tStrip = tColorZebra.Strip(iIndex)
			iaStep(iIndex) = CInt(tStrip.ScalingWidth)
			oaBrushes(iIndex) = New SolidBrush(tStrip.Color.FrameworkColor)
			'MessageBox.Show(CStr(iaStep(iIndex)) & ":" & CStr(tStrip.Color.FrameworkColor.ToArgb), "18_514")
		Next
		If tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantII Then
			tTopLeftPoint = New Point(-CInt(mtRectPoint.Y / dTnA), 0)
			tBottomLeftPoint = New Point(0, mtRectPoint.Y)
			tTopLeftLimitedPoint = New Point(0 + mtRectPoint.X + 1, 0)
		ElseIf tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI Then
			tTopLeftPoint = New Point(0, 0)
			tBottomLeftPoint = New Point(0 - CInt(mtRectPoint.Y / dTnA), mtRectPoint.Y)
			tTopLeftLimitedPoint = New Point(0 + mtRectPoint.X + CInt(mtRectPoint.Y / dTnA) + 1, 0)
		Else
			MessageBox.Show("Design Error", "18_800")
		End If

		Dim iStripIndex As Integer = 0
		Dim iTest As Integer = 0
		Dim oBrush As Brush
		'	Dim oPen As Pen
		Dim oPgonPoints(3) As Point
		AcadDocument.WriteMessage(CStr(0) & ":" & CStr(mtRectPoint.X) & ":" & CStr(CInt(mtRectPoint.Y / dTnA)))
		AcadDocument.WriteMessage("Lim: " & CStr(tTopLeftLimitedPoint.X))


		Do While tTopLeftPoint.X <= tTopLeftLimitedPoint.X
			'	oPen = oaPens(iStripIndex)
			tTopRightPoint = New Point(tTopLeftPoint.X + iaStep(iStripIndex), tTopLeftPoint.Y)
			tBottomRightPoint = New Point(tBottomLeftPoint.X + iaStep(iStripIndex), tBottomLeftPoint.Y)
			oPgonPoints(0) = tTopLeftPoint
			oPgonPoints(1) = tTopRightPoint
			oPgonPoints(2) = tBottomRightPoint
			oPgonPoints(3) = tBottomLeftPoint
			oBrush = oaBrushes(iStripIndex)
			If iTest < 160 Then
				moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Alternate)
				AcadDocument.WriteMessage(CStr(oPgonPoints(0).X) & ":" & CStr(oPgonPoints(1).X))
			End If
			tTopLeftPoint = New Point(tTopRightPoint.X, tTopRightPoint.Y)

			tBottomLeftPoint = New Point(tBottomRightPoint.X, tBottomRightPoint.Y)

			iTest += 1

			iStripIndex = (iStripIndex + 1) Mod (iStripUB + 1)
		Loop
	End Sub
	Public Sub DrawZebra(ByVal tColorZebra As ColorZebra)
		Dim iStripUB As Integer = tColorZebra.StripUB
		'	Dim oaPens(iStripUB) As Pen
		Dim oaBrushes(iStripUB) As SolidBrush

		Dim iaStep(iStripUB) As Integer
		Dim tStrip As ColorStrip
		Dim tAngle As LineAngle = tColorZebra.Angle
		Dim dTnA As Double = tAngle.DirectTan


		Dim tTopLeftPoint, tTopRightPoint, tBottomLeftPoint, tBottomRightPoint As PointExt
		Dim tTopLeftLimitedPoint As PointExt
		For iIndex As Integer = 0 To iStripUB
			tStrip = tColorZebra.Strip(iIndex)
			iaStep(iIndex) = CInt(tStrip.ScalingWidth * tAngle.DirectSec)
			oaBrushes(iIndex) = New SolidBrush(tStrip.Color.FrameworkColor)
		Next
		PointExt.XDirection = tAngle.XDirection
		If tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantII Then
			tTopLeftPoint = New PointExt(New PointExt(0, 0), -CInt(mtRectPoint.NoneDirectCoord * dTnA))
			tBottomLeftPoint = New PointExt(New PointExt(0, 0), 0, mtRectPoint.NoneDirectCoord)
			tTopLeftLimitedPoint = New PointExt(tTopLeftPoint, mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA) + 1)
		ElseIf tAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI Then
			tTopLeftPoint = New PointExt(0, 0)
			tBottomLeftPoint = New PointExt(tTopLeftPoint, -CInt(mtRectPoint.NoneDirectCoord * dTnA), mtRectPoint.NoneDirectCoord)
			tTopLeftLimitedPoint = New PointExt(tTopLeftPoint, mtRectPoint.DirectCoord + CInt(mtRectPoint.NoneDirectCoord * dTnA) + 1)
		Else
			MessageBox.Show("Design Error", "18_800")
		End If

		Dim iStripIndex As Integer = 0
		Dim iTest As Integer = 0
		Dim oBrush As Brush
		'	Dim oPen As Pen
		Dim oPgonPoints(3) As Point
		'	AcadDocument.WriteMessage(CStr(mtRectPoint.DirectCoord) & ":" & CStr(CInt(mtRectPoint.NoneDirectCoord * dTnA)))
		'	AcadDocument.WriteMessage("Lim: " & CStr(tTopLeftLimitedPoint.X))

		'	AcadDocument.WriteMessage("Angle=" & CStr(tAngle.AngleDegree) & "," & CStr(dTnA) & "," & CStr(tAngle.XDirection) & "; (" & tTopLeftPoint.Coordinates & ") : (" & tBottomLeftPoint.Coordinates & ") : (" & tTopLeftLimitedPoint.Coordinates & ")")
		Do While tTopLeftPoint.DirectCoord <= tTopLeftLimitedPoint.DirectCoord
			'	oPen = oaPens(iStripIndex)

			tTopRightPoint = New PointExt(tTopLeftPoint, iaStep(iStripIndex))
			tBottomRightPoint = New PointExt(tBottomLeftPoint, iaStep(iStripIndex))
			oPgonPoints(0) = tTopLeftPoint.FWPoint
			oPgonPoints(1) = tTopRightPoint.FWPoint
			oPgonPoints(2) = tBottomRightPoint.FWPoint
			oPgonPoints(3) = tBottomLeftPoint.FWPoint
			oBrush = oaBrushes(iStripIndex)
			If iTest < 160 Then
				moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Alternate)
				'AcadDocument.WriteMessage(CStr(oPgonPoints(0).X) & ":" & CStr(oPgonPoints(1).X))
			End If
			tTopLeftPoint = New PointExt(tTopRightPoint.X, tTopRightPoint.Y)

			tBottomLeftPoint = New PointExt(tBottomRightPoint.X, tBottomRightPoint.Y)

			iTest += 1

			iStripIndex = (iStripIndex + 1) Mod (iStripUB + 1)
		Loop
	End Sub

	Public Sub DrawHatch(ByVal tHatch As DMHatch)
		Dim oPen As Pen = New Pen(tHatch.BackColor.FrameworkColor)
		Dim oBrush As Brush = oPen.Brush
		moGraph.FillRectangle(oBrush, 0, 0, mtRectPoint.X, mtRectPoint.Y)
		If Not tHatch.SolidOnly Then
			Select Case tHatch.PatternName
				Case DMHatch.LineHatchName
					zzDrawHatchLine(tHatch)
				Case DMHatch.NetHatchName
					zzDrawHatchNet(tHatch)
				Case Else
					AcadDocument.WriteMessage("Pattern: '" & tHatch.PatternName & "'")
			End Select
		End If

	End Sub
	Public Sub DrawBorder(ByVal tBorder As ColorBorder)
		Dim iStripUB As Integer = tBorder.StripUB
		Dim oaBrushes(iStripUB) As Brush
		Dim iaStep(iStripUB) As Integer
		Dim tStrip As ColorStrip

		Dim dXStep As Double = 9

		Dim tTopLeftStartPoint, tTopLeftEndHorPoint, tTopLeftEndVertPoint, tBottomLeftStartPoint, tBottomLeftEndHorPoint, tBottomLeftEndVertPoint As Point
		Dim tTopRightStartPoint, tTopRightEndHorPoint, tTopRightEndVertPoint, tBottomRightStartPoint, tBottomRightEndHorPoint, tBottomRightEndVertPoint As Point

		For iIndex As Integer = 0 To iStripUB
			tStrip = tBorder.Strip(iIndex)
			iaStep(iIndex) = CInt(tStrip.ScalingWidth)
			oaBrushes(iIndex) = New SolidBrush(tStrip.Color.FrameworkColor)
		Next

		tTopLeftStartPoint = New Point(0, 0)
		tBottomLeftStartPoint = New Point(0, mtRectPoint.Y)

		tTopRightStartPoint = New Point(mtRectPoint.X, 0)
		tBottomRightStartPoint = New Point(mtRectPoint.X, mtRectPoint.Y)

		Dim oBrush As Brush
		Dim oPgonPoints(3) As Point
		For iStripIndex As Integer = 0 To iStripUB
			oBrush = oaBrushes(iStripIndex)

			tTopLeftEndHorPoint = New Point(tTopLeftStartPoint.X + iaStep(iStripIndex), tTopLeftStartPoint.Y)
			tTopLeftEndVertPoint = New Point(tTopLeftStartPoint.X, tTopLeftStartPoint.Y + iaStep(iStripIndex))

			tBottomLeftEndHorPoint = New Point(tBottomLeftStartPoint.X + iaStep(iStripIndex), tBottomLeftStartPoint.Y)
			tBottomLeftEndVertPoint = New Point(tBottomLeftStartPoint.X, tBottomLeftStartPoint.Y - iaStep(iStripIndex))

			oPgonPoints(0) = tTopLeftStartPoint
			oPgonPoints(1) = tTopLeftEndHorPoint
			oPgonPoints(2) = tBottomLeftEndHorPoint
			oPgonPoints(3) = tBottomLeftStartPoint

			moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Winding)
			''''''''''''''
			tTopRightEndHorPoint = New Point(tTopRightStartPoint.X - iaStep(iStripIndex), tTopRightStartPoint.Y)
			tTopRightEndVertPoint = New Point(tTopRightStartPoint.X, tTopRightStartPoint.Y + iaStep(iStripIndex))

			tBottomRightEndHorPoint = New Point(tBottomRightStartPoint.X - iaStep(iStripIndex), tBottomRightStartPoint.Y)
			tBottomRightEndVertPoint = New Point(tBottomRightStartPoint.X, tBottomLeftStartPoint.Y - iaStep(iStripIndex))

			oPgonPoints(0) = tTopRightStartPoint
			oPgonPoints(1) = tTopRightEndHorPoint
			oPgonPoints(2) = tBottomRightEndHorPoint
			oPgonPoints(3) = tBottomRightStartPoint

			moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Winding)

			''''''''''''''
			oPgonPoints(0) = tTopLeftStartPoint
			oPgonPoints(1) = tTopLeftEndVertPoint
			oPgonPoints(2) = tTopRightEndVertPoint
			oPgonPoints(3) = tTopRightStartPoint

			moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Winding)



			''''''''''''''
			oPgonPoints(0) = tBottomLeftStartPoint
			oPgonPoints(1) = tBottomLeftEndVertPoint
			oPgonPoints(2) = tBottomRightEndVertPoint
			oPgonPoints(3) = tBottomRightStartPoint

			moGraph.FillPolygon(oBrush, oPgonPoints, Drawing2D.FillMode.Winding)

			tTopLeftStartPoint = New Point(tTopLeftEndHorPoint.X, tTopLeftEndVertPoint.Y)
			tBottomLeftStartPoint = New Point(tBottomLeftEndHorPoint.X, tBottomLeftEndVertPoint.Y)
			tTopRightStartPoint = New Point(tTopRightEndHorPoint.X, tTopRightEndVertPoint.Y)
			tBottomRightStartPoint = New Point(tBottomRightEndHorPoint.X, tBottomRightEndVertPoint.Y)
		Next
	End Sub
	Private Structure PointExt
		Dim FWPoint As System.Drawing.Point
		Public Shared XDirection As Boolean
		Public Sub New(ByVal tPoint As PointExt, ByVal iStep As Integer)
			If XDirection Then
				FWPoint = New Point(tPoint.X + iStep, tPoint.Y)
			Else
				FWPoint = New Point(tPoint.X, tPoint.Y + iStep)
			End If
		End Sub
		Public Sub New(ByVal tPoint As PointExt, ByVal iDirectStep As Integer, ByVal iNoneDirectStep As Integer)
			If XDirection Then
				FWPoint = New Point(tPoint.X + iDirectStep, tPoint.Y + iNoneDirectStep)
			Else
				FWPoint = New Point(tPoint.X + iNoneDirectStep, tPoint.Y + iDirectStep)
			End If
		End Sub
		Public Sub New(ByVal tPoint As Point)
			FWPoint = tPoint
		End Sub
		Public Sub New(ByVal iX As Integer, ByVal iY As Integer)
			FWPoint = New Point(iX, iY)
		End Sub
		Public ReadOnly Property X() As Integer
			Get
				Return FWPoint.X
			End Get
		End Property
		Public ReadOnly Property Y() As Integer
			Get
				Return FWPoint.Y
			End Get
		End Property
		Public ReadOnly Property Coordinates() As String
			Get
				Return Convert.ToString(FWPoint.X) & "," & Convert.ToString(FWPoint.Y)
			End Get
		End Property
		Public ReadOnly Property DirectCoord() As Integer
			Get
				If XDirection Then
					Return FWPoint.X
				Else
					Return FWPoint.Y
				End If

			End Get
		End Property
		Public ReadOnly Property NoneDirectCoord() As Integer
			Get
				If XDirection Then
					Return FWPoint.Y
				Else
					Return FWPoint.X
				End If
			End Get
		End Property
	End Structure
End Class

