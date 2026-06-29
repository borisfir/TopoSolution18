Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry

Public Structure TplnPointKeyULong
	Private Const ShiftX As ULong = 10000000000UL


	Public Const RoundDigit As Integer = 4
	'Public Const RoundShift As Double = 10000.0
	' Private Shared mlCenterX As ULong
	' Private Shared mlCenterY As ULong
	Private Shared mdOriginX As Double
	Private Shared mdOriginY As Double

	Private Shared mdMinX As Double
	Private Shared mdMinY As Double

	Private Shared mdMaxX As Double
	Private Shared mdMaxY As Double
	Private Shared mdRoundScale As Double = 1.0
	Private Shared mdRoundScaleInv As Double = 1.0
	Private Shared mtPointTolerance As Tolerance
	Private mlX As ULong
	Private mlY As ULong
	Private mlCode As ULong
	Public Sub New(lX As ULong, lY As ULong)
		mlX = lX
		mlY = lY
		zzCalcCode()
	End Sub

	Public Sub New(dX As Double, dY As Double)
		mlX = zzCoordToInteger(dX, mdOriginX)
		mlY = zzCoordToInteger(dY, mdOriginY)
		zzCalcCode()
	End Sub
	Public Sub New(tPoint As Point2d)
		'197881,628196
		mlX = zzCoordToInteger(tPoint.X, mdOriginX)
		mlY = zzCoordToInteger(tPoint.Y, mdOriginY)
		zzCalcCode()
	End Sub
	Public Sub New(tPoint As Point2d, bMinX As Boolean, bMinY As Boolean)
		'197881,628196
		mlX = zzCoordToInteger(tPoint.X, mdOriginX, bMinX)
		mlY = zzCoordToInteger(tPoint.Y, mdOriginY, bMinY)
		zzCalcCode()
	End Sub

	Public Shared Sub SetOrigin(dX As Double, dY As Double)
		mdOriginX = Math.Floor(dX)
		mdOriginY = Math.Floor(dY)
		DMAcadExt.AcadDocument.WriteMessage("Viewport Origin: " & CStr(mdOriginX) & "," & CStr(mdOriginY))

	End Sub
	Public Shared Sub SetOrigin(tPoint As Point2d)
		SetOrigin(tPoint.X, tPoint.Y)
	End Sub
	Public Shared Property RoundScale() As Double
		Get
			Return mdRoundScale
		End Get
		Set(ByVal dValue As Double)
			mdRoundScale = dValue

		End Set
	End Property
	Public Shared Property Tolerance() As Double
		Get
			Return 1.0 / mdRoundScale
		End Get
		Set(ByVal dValue As Double)
			mdRoundScaleInv = dValue
			mdRoundScale = Math.Round(1.0 / dValue, MidpointRounding.AwayFromZero)
			mtPointTolerance = New Tolerance(0.0, dValue)
			AcadDocument.WriteMessage("RoundScale=" & CStr(mdRoundScale) & ";  RoundScaleInv=" & CStr(mdRoundScaleInv))
		End Set
	End Property

	Public Shared Sub SetExtension(tMinPoint As Autodesk.AutoCAD.Geometry.Point2d, tMaxPoint As Autodesk.AutoCAD.Geometry.Point2d)
		mdMinX = Math.Floor(tMinPoint.X) - 1.0
		mdMinY = Math.Floor(tMinPoint.Y) - 1.0

		mdMaxX = Math.Floor(tMaxPoint.X) + 1.0
		mdMaxY = Math.Floor(tMaxPoint.Y) + 1.0

		DMAcadExt.AcadDocument.WriteMessage("Extension: " & tMinPoint.ToString() & " ==> " & tMaxPoint.ToString())
	End Sub


	Public Shared Function PointFromKey(tPointKey As ULong) As TPlnPoint
		'   Dim decY As Decimal = Math.Floor(tPointKey / ShiftX)
		'   Dim decX As Decimal = tPointKey - ShiftX * decY

		'   Return New TPlnPoint(Convert.ToDouble(decX + mdcCenterX), Convert.ToDouble(decY + mdcCenterY))



		Dim lX As ULong = tPointKey \ ShiftX
		Dim lY As ULong = tPointKey - (lX * ShiftX)
		Dim dX As Double = Convert.ToDouble(lX) * mdRoundScaleInv
		Dim dY As Double = Convert.ToDouble(lY) * mdRoundScaleInv



		Return New TPlnPoint(mdOriginX + dX, mdOriginY + dY)
	End Function

	Public Shared Function CoordToKey(dX As Double, dY As Double) As ULong
		Dim bErrExt As Boolean = False
		If dX >= mdMinX AndAlso dX <= mdMaxX Then
			dX = Math.Round(mdRoundScale * (dX - mdOriginX), 0, MidpointRounding.AwayFromZero)
		Else
			bErrExt = True
		End If
		If dY >= mdMinY AndAlso dY <= mdMaxY Then
			dY = Math.Round(mdRoundScale * (dY - mdOriginY), 0, MidpointRounding.AwayFromZero)
		Else
			bErrExt = True
		End If


		If bErrExt Then
			Dim sMsg As String = "Point is out of drawing extension " & dX.ToString() & "," & dY.ToString() & vbCrLf & mdMinX.ToString() & "," & mdMinY.ToString()
			DMAcadExt.AppMessages.AddMessage(True, dX, dY, "", sMsg, False)
			Return 0UL
		Else


			Return ShiftX * Convert.ToUInt64(dX) + Convert.ToUInt64(dY)
		End If


	End Function
	Public Shared Function GetRoundedPoint(dX As Double, dY As Double) As TPlnPoint
		'dX = Math.Round(RoundShift * (dX - mdCenterX), 0, MidpointRounding.AwayFromZero)
		'dY = Math.Round(RoundShift * (dY - mdCenterY), 0, MidpointRounding.AwayFromZero)



		Return New TPlnPoint(dX, dY)

	End Function
	Public Shared Function GetPointByOrigin(tPoint As Point2d) As Point2d
		Return New Point2d(tPoint.X - mdOriginX, tPoint.Y - mdOriginY)
	End Function
	Public Function AddStep(tKeyStep As KeyStep) As TplnPointKeyULong
		Dim lNewX As ULong
		Dim lNewY As ULong

		If tKeyStep.X > 0 Then
			lNewX = mlX + Convert.ToUInt64(tKeyStep.X)
		ElseIf tKeyStep.X < 0 Then
			lNewX = mlX - Convert.ToUInt64(-tKeyStep.X)
		Else
			lNewX = mlX
		End If
		If tKeyStep.Y > 0 Then
			lNewY = mlY + Convert.ToUInt64(tKeyStep.Y)
		ElseIf tKeyStep.Y < 0 Then
			lNewY = mlY - Convert.ToUInt64(-tKeyStep.Y)
		Else
			lNewY = mlY
		End If
		Return New TplnPointKeyULong(lNewX, lNewY)

	End Function

	Public ReadOnly Property Point As TPlnPoint
		Get
			Dim dX As Double = Convert.ToDouble(mlX) * mdRoundScaleInv + mdOriginX
			Dim dY As Double = Convert.ToDouble(mlY) * mdRoundScaleInv + mdOriginY
			Return New TPlnPoint(dX, dY)
		End Get
	End Property
	Public ReadOnly Property X As ULong
		Get
			Return mlX
		End Get
	End Property
	Public ReadOnly Property y As ULong
		Get
			Return mlY
		End Get
	End Property
	Public ReadOnly Property Code As ULong
		Get
			Return mlCode
		End Get
	End Property
	Private Sub zzCalcCode()
		mlCode = ShiftX * mlX + mlY
	End Sub
	Private Function zzCoordToInteger(dCoord As Double, dOrigin As Double) As ULong
		Dim dRelative As Double = dCoord - dOrigin
		Dim lRes As ULong
		If dRelative >= 0.0 Then
			lRes = Convert.ToUInt64(Math.Round(mdRoundScale * dRelative, 0, MidpointRounding.AwayFromZero))
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Coord2ULong", lRes, dRelative, mdRoundScale * dRelative)
			Return lRes
		Else
			Return 0UL

		End If

	End Function
	Private Function zzCoordToInteger(dCoord As Double, dOrigin As Double, bMin As Boolean) As ULong
		Dim dRelative As Double = dCoord - dOrigin
		Dim lRes As ULong
		If dRelative >= 0.0 Then
			If bMin Then
				lRes = Convert.ToUInt64(Math.Floor(mdRoundScale * dRelative))
			Else
				lRes = Convert.ToUInt64(Math.Ceiling(mdRoundScale * dRelative))

			End If

			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Coord2ULong", lRes, dRelative, mdRoundScale * dRelative)
			Return lRes
		Else
			Return 0UL

		End If

	End Function

	Public Structure KeyStep
		Public X As Long
		Public Y As Long
		Public Sub New(lX As Long, lY As Long)
			X = lX
			Y = lY
		End Sub

	End Structure
End Structure
