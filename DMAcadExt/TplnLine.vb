Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Imports DMAcadExt

Public Interface IUD_Link
	ReadOnly Property StartPoint As Point2d
	ReadOnly Property EndPoint As Point2d
	ReadOnly Property IsArc As Boolean
	ReadOnly Property Bulge As Double
	ReadOnly Property Radius As Double
	ReadOnly Property TotalAngle As Double


	Function GetMidPoint() As Point2d
	Function GetMidPoint3d() As Point3d

	Sub Extend(oLink As IUD_Link)
	Function IsExtend(oLink As IUD_Link, tTolerance As DMAcadExt.dmTolerance, Optional bPrintDelta As Boolean = False) As Boolean
	Function GetBoundingBox() As TPlnBoundingBox

	Function GetNewEntity() As Entity
	ReadOnly Property Coordinates As String
	ReadOnly Property Length As Double
	ReadOnly Property EntityHandle As Autodesk.AutoCAD.DatabaseServices.Handle
	Function GetInfo(iDigits As Integer) As String
	Sub SetPoint(tPoint As Point2d, bStart As Boolean)
	Sub Reverse()
	Function Copy() As IUD_Link
End Interface
Public Structure PointLink
	Dim PointName As String
	Dim Link As DMAcadExt.IUD_Link
	Public Sub New(sPointName As String, oLink As DMAcadExt.IUD_Link)
		PointName = sPointName
		Link = oLink
	End Sub
End Structure
Public Class TplnLine
	Inherits TplnCurveSegment

	Implements IUD_Link


	'  Protected dtStartPoint As Point2d
	'   Protected dtEndPoint As Point2d
	Protected dtHandle As Autodesk.AutoCAD.DatabaseServices.Handle
	Private moDirVector As Vector3d
	Public Sub New(oLine As Line, Optional bSameDirection As Boolean = True)
		If bSameDirection Then
			dtStartPoint = TPlnPoint.Point3dTo2d(oLine.StartPoint)
			dtEndPoint = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		Else
			dtEndPoint = TPlnPoint.Point3dTo2d(oLine.StartPoint)
			dtStartPoint = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		End If

		dtHandle = oLine.Handle
		moDirVector = oLine.Delta
	End Sub
	Public Sub New(oPolyline As Polyline, Optional bSameDirection As Boolean = True)
		If bSameDirection Then
			dtStartPoint = oPolyline.GetPoint2dAt(0)
			dtEndPoint = oPolyline.GetPoint2dAt(1)
		Else
			dtEndPoint = oPolyline.GetPoint2dAt(0)
			dtStartPoint = oPolyline.GetPoint2dAt(1)
		End If

		dtHandle = oPolyline.Handle
		Dim oLine As Line = New Line()
		'moDirVector = oPolyline.
		moDirVector = New Vector3d(oPolyline.GetPoint2dAt(1).X - oPolyline.GetPoint2dAt(0).X, oPolyline.GetPoint2dAt(1).Y - oPolyline.GetPoint2dAt(0).Y, 0)
	End Sub

	Public Sub New(tStartPoint As Point2d, tEndPoint As Point2d)
		dtStartPoint = tStartPoint
		dtEndPoint = tEndPoint

	End Sub
	Public Sub New(tStartPoint As Point3d, tEndPoint As Point3d)

		dtStartPoint = New Point2d(tStartPoint.X, tStartPoint.Y)

		dtEndPoint = New Point2d(tEndPoint.X, tEndPoint.Y)

	End Sub
	Public Sub New(tStartPoint As Point2d, oLine As Line)
		Dim tStartLinePoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		Dim tEndLinePoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)

		dtStartPoint = tStartPoint
		If dtStartPoint.IsEqualTo(tStartLinePoint) Then
			dtEndPoint = tEndLinePoint
		ElseIf dtStartPoint.IsEqualTo(tEndLinePoint) Then
			dtEndPoint = tStartLinePoint
		Else
			dtStartPoint = Point2d.Origin
			dtEndPoint = Point2d.Origin
			'  DMAcadExt.AcadDocument.WriteMessage("Error #1873")
		End If


	End Sub
	Public Function GetMidPoint() As Point2d Implements IUD_Link.GetMidPoint
		Dim oPoint As TPlnPoint = New TPlnPoint(dtStartPoint, dtEndPoint)
		Return oPoint.AcGePoint
	End Function
	Public Function GetMidPoint3d() As Point3d Implements IUD_Link.GetMidPoint3d
		Dim oPoint As TPlnPoint = New TPlnPoint(dtStartPoint, dtEndPoint)
		Return oPoint.AcGePoint3d
	End Function
	Public ReadOnly Property StartPointAAA() As Point2d Implements IUD_Link.StartPoint
		Get
			Return dtStartPoint
		End Get
	End Property
	Public ReadOnly Property EndPointAAA() As Point2d Implements IUD_Link.EndPoint
		Get
			Return dtEndPoint
		End Get
	End Property
	Public ReadOnly Property StartPoint3d() As Point3d
		Get
			Return TPlnPoint.Point2dTo3d(dtStartPoint)
		End Get
	End Property
	Public ReadOnly Property EndPoint3d() As Point3d
		Get
			Return TPlnPoint.Point2dTo3d(dtEndPoint)
		End Get
	End Property
	Public ReadOnly Property Vector2d() As Vector2d
		Get
			Return New Vector2d(dtEndPoint.X - dtStartPoint.X, dtEndPoint.Y - dtStartPoint.Y)
		End Get
	End Property
	Public Function GetYNew(dX As Double) As Double
		Return dtStartPoint.Y + Me.Tangent * (dX - dtStartPoint.X)
	End Function
	Public Function GetYNewAAA(lX As Long) As Long
		Dim dY As Double = GetYNew(Convert.ToDouble(lX))
		Return Convert.ToInt64(Math.Floor(dY))
	End Function
	Public Function GetAngleTo(oLine As TplnLine) As Double
		Return Me.Vector2d.GetAngleTo(oLine.Vector2d)
	End Function
	Public ReadOnly Property IsParallelToY(dTolerance As Double) As Boolean
		Get
			Dim dDeltaX As Double = Math.Abs(dtEndPoint.X - dtStartPoint.X)
			Dim dDeltaY As Double = Math.Abs(dtEndPoint.Y - dtStartPoint.Y)
			Return dDeltaX < dTolerance * dDeltaY

		End Get
	End Property
	Public ReadOnly Property Tangent() As Double
		Get
			Dim dDeltaX As Double = dtEndPoint.X - dtStartPoint.X
			Dim dDeltaY As Double = dtEndPoint.Y - dtStartPoint.Y
			Return dDeltaY / dDeltaX

		End Get
	End Property

	Public Sub Extend(oLink As IUD_Link) Implements IUD_Link.Extend
		If dtEndPoint.IsEqualTo(oLink.StartPoint) Then
			dtEndPoint = oLink.EndPoint
		ElseIf dtEndPoint.IsEqualTo(oLink.EndPoint) Then
			dtEndPoint = oLink.StartPoint
		ElseIf dtStartPoint.IsEqualTo(oLink.EndPoint) Then
			dtStartPoint = oLink.StartPoint
		ElseIf dtStartPoint.IsEqualTo(oLink.StartPoint) Then
			dtStartPoint = oLink.EndPoint
		Else
			System.Windows.Forms.MessageBox.Show("!!!????", "07_121a")
		End If

	End Sub
	Private Function zzGetDeviation(oTplnLine As TplnLine) As Double
		Dim oStraightenerLine As Line = New Line(Me.StartPoint3d, oTplnLine.EndPoint3d)
		Dim tPoint As Point3d = oStraightenerLine.GetClosestPointTo(Me.EndPoint3d, False)
		Return tPoint.DistanceTo(Me.EndPoint3d)
	End Function
	Private Sub zzGetDeviation(oTplnLine As TplnLine, ByRef dAngle As Double, ByRef dDistance As Double, ByRef dArea As Double)
		Dim oStraightenerLine As Line = New Line(Me.StartPoint3d, oTplnLine.EndPoint3d)
		Dim tPoint As Point3d = oStraightenerLine.GetClosestPointTo(Me.EndPoint3d, False)
		'   DMAcadExt.AcadDocument.WriteMessage("ClosestPoint: " & tPoint.ToString())

		dAngle = Me.GetAngleTo(oTplnLine)
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "_Deviat", New TPlnPoint(Me.StartPoint3d()).Coordinates2d, New TPlnPoint(Me.EndPoint3d()).Coordinates2d, oTplnLine.Coordinates, dAngle)
		dDistance = tPoint.DistanceTo(Me.EndPoint3d)
		dArea = 0.5 * oStraightenerLine.Length * dDistance
	End Sub
	Public Function IsParallelTo(oTplnLine As TplnLine, dTolerance As Double, Optional bPrintDelta As Boolean = False) As Boolean
		If Me.IsParallelToY(dTolerance) Then
			Return oTplnLine.IsParallelToY(dTolerance)
		ElseIf oTplnLine.IsParallelToY(dTolerance) Then
			Return False
		Else
			Dim dTangentDelta As Double = Me.Tangent - oTplnLine.Tangent
			DMAcadExt.AcadDocument.WriteMessage("DeltaK=" & CStr(dTangentDelta))
			If bPrintDelta Then
				DMAcadExt.AcadDocument.WriteMessage("DeltaK=" & CStr(dTangentDelta))
			End If

			Return Math.Abs(dTangentDelta) < dTolerance
		End If
	End Function


	Public ReadOnly Property IsArc As Boolean Implements IUD_Link.IsArc
		Get
			Return False
		End Get
	End Property

	Public Function IsExtendByAngle(oLink As IUD_Link, dTolerance As Double, Optional bPrintDelta As Boolean = False) As Boolean
		If oLink.IsArc Then
			Return False
		Else
			Dim oTplnLine As TplnLine = DirectCast(oLink, TplnLine)
			Return Me.IsParallelTo(oTplnLine, dTolerance, bPrintDelta)
		End If
	End Function

	Public Function IsExtend(oLink As IUD_Link, tTolerance As DMAcadExt.dmTolerance, Optional bPrintDelta As Boolean = False) As Boolean Implements IUD_Link.IsExtend
		If oLink.IsArc Then
			Return False
		Else
			Dim oTplnLine As TplnLine = DirectCast(oLink, TplnLine)
			' DMAcadExt.AcadDocument.WriteMessage("Line A: " & Me.StartPoint.ToString() & " => " & Me.EndPoint.ToString())
			' DMAcadExt.AcadDocument.WriteMessage("Line B: " & oTplnLine.StartPoint.ToString() & " => " & oTplnLine.EndPoint3d.ToString())


			Dim dAngleDeviation As Double
			Dim dDistanceDeviation As Double
			Dim dAreaDeviation As Double

			zzGetDeviation(oTplnLine, dAngleDeviation, dDistanceDeviation, dAreaDeviation)
			If bPrintDelta Then
				DMAcadExt.AcadDocument.WriteMessage("Delta Angle=" & FormatNumber(dAngleDeviation * 180.0 / Math.PI, 3) & " Tol=" & FormatNumber(tTolerance.AngleTolerance * 180.0 / Math.PI), 3)
				DMAcadExt.AcadDocument.WriteMessage("Delta Distance=" & FormatNumber(dDistanceDeviation, 3) & " Tol=" & CStr(tTolerance.DistanceTolerance))
				DMAcadExt.AcadDocument.WriteMessage("Delta Area=" & FormatNumber(dAreaDeviation, 3) & " Tol=" & CStr(tTolerance.AreaTolerance))

			End If

			Return (dAngleDeviation < tTolerance.AngleTolerance) AndAlso (dDistanceDeviation < tTolerance.DistanceTolerance) AndAlso (dAreaDeviation < tTolerance.AreaTolerance)
		End If
	End Function

	Private Shared Function zzHasNode(oPrevLine As Line, oNextLine As Line) As Boolean
		Dim tPrevDelta As Autodesk.AutoCAD.Geometry.Vector3d = oPrevLine.Delta
		Dim tNextDelta As Autodesk.AutoCAD.Geometry.Vector3d = oNextLine.Delta
		'  DMAcadExt.AcadDocument.WriteMessage("Paral? " & tPrevDelta.ToString() & " <> " & tNextDelta.ToString())
		Return Not tPrevDelta.IsParallelTo(tNextDelta, New Autodesk.AutoCAD.Geometry.Tolerance(0.001, 0.001))
	End Function


	Public ReadOnly Property Coordinates As String Implements IUD_Link.Coordinates
		Get
			Return dtStartPoint.ToString & " ; " & dtEndPoint.ToString
		End Get
	End Property

	Public ReadOnly Property Bulge As Double Implements IUD_Link.Bulge
		Get
			Return 0.0
		End Get
	End Property

	Public ReadOnly Property EntityHandle As Handle Implements IUD_Link.EntityHandle
		Get
			Return dtHandle
		End Get
	End Property
	Public Function GetInfo(iDigits As Integer) As String Implements IUD_Link.GetInfo
		Return zzGetFormat(StartPoint, iDigits) & "=>" & zzGetFormat(EndPoint, iDigits)
	End Function
	Private Shared Function zzGetFormat(tPoint As Point2d, iDigits As Integer) As String
		Return FormatNumber(tPoint.X, iDigits, TriState.False, TriState.False, TriState.False) & "," & FormatNumber(tPoint.Y, iDigits, TriState.False, TriState.False, TriState.False)
	End Function

	Public Sub Reverse() Implements IUD_Link.Reverse
		Dim tTempPoint As Point2d = dtStartPoint
		dtStartPoint = dtEndPoint
		dtEndPoint = tTempPoint
	End Sub

	Public Function Copy() As IUD_Link Implements IUD_Link.Copy
		Return New TplnLine(dtStartPoint, dtEndPoint)

	End Function

	Public ReadOnly Property Length As Double Implements IUD_Link.Length
		Get
			Return dtStartPoint.GetDistanceTo(dtEndPoint)
		End Get
	End Property

	Public ReadOnly Property Radius As Double Implements IUD_Link.Radius
		Get
			Return 0.0
		End Get
	End Property

	Public Function GetBoundingBox() As TPlnBoundingBox Implements IUD_Link.GetBoundingBox


		Return New TPlnBoundingBox(New TPlnPoint(dtStartPoint), New TPlnPoint(dtEndPoint))
	End Function

	Public ReadOnly Property TotalAngle As Double Implements IUD_Link.TotalAngle
		Get
			Return 0.0
		End Get
	End Property



	Public Function GetNewEntity() As Entity Implements IUD_Link.GetNewEntity
		Return Nothing
	End Function

	Public Overrides Function GetBulge() As Double
		Return 0.0
	End Function

	Public Sub SetPoint(tPoint As Point2d, bStart As Boolean) Implements IUD_Link.SetPoint
		If bStart Then
			dtStartPoint = tPoint

		Else

			dtEndPoint = tPoint
		End If

	End Sub
End Class
