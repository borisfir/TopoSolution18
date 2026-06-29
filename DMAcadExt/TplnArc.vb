Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Imports DMAcadExt

Public Class TplnArc
	Inherits TplnCurveSegment
	Implements IUD_Link

	Private Shared mdRadToDeg As Double = 180.0 / Math.PI
	Private mtCenter As Point2d
	Private mdRadius As Double

	Private mdStartAngle As Double
	Private mdEndAngle As Double

	Private mtStartVector As Vector2d
	Private mtEndVector As Vector2d
	Private mbIsClockWise As Boolean
	Private mdTotalAngle As Double
	Private mdBulge As Double

	Private mbSameDirection As Boolean
	'Private mGeoExt As GE

	Protected mtHandle As Autodesk.AutoCAD.DatabaseServices.Handle
	Private mtTestPoint As Point2d = New Point2d(181716.926, 647279.982)
	'  Private moArc As Arc
	'	Both the X and Y displacement and the bulge, which specifies the curvature of the arc, can range from -127 to +127. 
	'  If the line segment specified by the displacement has length D, and the perpendicular distance from the midpoint of that segment has height H,
	'  the magnitude of the bulge is ((2* H / D) * 127). The sign is negative if the arc from the current location to the new location is clockwise.
	Public Sub New(ByVal oArc As Arc, Optional ByVal bSameDirection As Boolean = True)
		Me.New(TPlnPoint.Point3dTo2d(oArc.Center), oArc.Radius, oArc.StartAngle, oArc.EndAngle, bSameDirection)
		'  moArc = oArc
		Me.mbSameDirection = bSameDirection
		If bSameDirection Then
			MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oArc.StartPoint)
			MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oArc.EndPoint)
			Me.mdStartAngle = oArc.StartAngle
			Me.mdEndAngle = oArc.EndAngle
			Me.mbIsClockWise = False
		Else
			MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oArc.EndPoint)
			MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oArc.StartPoint)
			Me.mdStartAngle = oArc.EndAngle
			Me.mdEndAngle = oArc.StartAngle

			Me.mbIsClockWise = True
		End If
		mdTotalAngle = oArc.TotalAngle

		'	DMCommon.ExcelLogAW5.SetNextValue(0, "???Arc", mdStartAngle, mdEndAngle, mdTotalAngle)

		If mtCenter.GetDistanceTo(mtTestPoint) < 2.0 Then
			'  DMAcadExt.AcadDocument.WriteMessage("Total:" & mdTotalAngle.ToString() & "; Center=" & mtCenter.ToString())
		End If
		'  oArc.GeometricExtents

		Me.mtHandle = oArc.Handle
		'  AcadDocument.WriteDebugMessage(("!Center:" & TPlnPoint.DispPoint(Me.mtCenter) & "; Radius=" & Convert.ToString(Me.mdRadius)) & "; Bulge=" & Convert.ToString(Me.Bulge))
		'  AcadDocument.WriteDebugMessage(("!StartPoint:" & TPlnPoint.DispPoint(Me.dtStartPoint) & "; EndPoint=" & Convert.ToString(Me.dtEndPoint) & "; ClockWise=" & Convert.ToString(Me.mbIsClockWise)))

		AcadDocument.WriteDebugMessage("!StartAngle:" & Me.zzDispAngle(Me.mdStartAngle, 1) & "; EndAngle:" & Me.zzDispAngle(Me.mdEndAngle, 1) & "; TotalAngle:" & Me.zzDispAngle(Me.mdTotalAngle, 1) & "; VAngle:" & Me.zzDispAngle(MyBase.ddVectorAngle, 1))
	End Sub
	Public Sub New(ByVal oPolyline As Polyline)
		Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
		Dim oArc As Arc = Nothing


		mdBulge = oPolyline.GetBulgeAt(0)
		dtStartPoint = oPolyline.GetPoint2dAt(0)
		dtEndPoint = oPolyline.GetPoint2dAt(1)



		oPolyline.Explode(colDBObjects)
		For Each oDBObject As DBObject In colDBObjects
			If oArc Is Nothing Then
				oArc = TryCast(oDBObject, Arc)
			End If
		Next
		If oArc IsNot Nothing Then
			mtCenter = TPlnPoint.Point3dTo2d(oArc.Center)
			mdRadius = oArc.Radius
			mdTotalAngle = oArc.TotalAngle
			mbIsClockWise = (mdBulge < 0.0)
			zzCalcAngles()
		End If
		'	DMCommon.ExcelLog.SetNextValue(0, "New oArc1", MyBase.dtStartPoint.ToString(), MyBase.dtEndPoint.ToString(), mbIsClockWise, mdBulge, zzCalculateBulgeNew(), mbIsClockWise, mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle * mdRadToDeg)

	End Sub
	Public Sub New(ByVal oPolyline As Polyline, ByVal bSameDirection As Boolean)
		Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
		Dim oArc As Arc = Nothing
		mbSameDirection = bSameDirection
		If bSameDirection Then
			mdBulge = oPolyline.GetBulgeAt(0)
			dtStartPoint = oPolyline.GetPoint2dAt(0)
			dtEndPoint = oPolyline.GetPoint2dAt(1)
		Else
			mdBulge = -oPolyline.GetBulgeAt(0)
			dtStartPoint = oPolyline.GetPoint2dAt(1)
			dtEndPoint = oPolyline.GetPoint2dAt(0)

		End If


		oPolyline.Explode(colDBObjects)
		For Each oDBObject As DBObject In colDBObjects
			If oArc Is Nothing Then
				oArc = TryCast(oDBObject, Arc)
			End If
		Next
		If oArc IsNot Nothing Then
			mtCenter = TPlnPoint.Point3dTo2d(oArc.Center)
			mdRadius = oArc.Radius
			mdTotalAngle = oArc.TotalAngle
			mbIsClockWise = (mdBulge < 0.0)
			zzCalcAngles()
		End If
		'DMCommon.ExcelLogAW5.SetNextValue(0, "New oArc2", MyBase.dtStartPoint.ToString(), MyBase.dtEndPoint.ToString(), bSameDirection, mbSameDirection, mbIsClockWise, mdBulge, zzCalculateBulgeNew(), mbIsClockWise, mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle * mdRadToDeg)

	End Sub

	Public Sub New251218(ByVal oPolyline As Polyline, Optional ByVal bSameDirection As Boolean = True)
		Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
		Dim oArc As Arc = Nothing
		mbSameDirection = bSameDirection
		mdBulge = oPolyline.GetBulgeAt(0)
		dtStartPoint = oPolyline.GetPoint2dAt(0)
		dtEndPoint = oPolyline.GetPoint2dAt(1)

		oPolyline.Explode(colDBObjects)
		For Each oDBObject As DBObject In colDBObjects
			If oArc Is Nothing Then
				oArc = TryCast(oDBObject, Arc)
			End If
		Next
		If oArc IsNot Nothing Then
			mtCenter = TPlnPoint.Point3dTo2d(oArc.Center)
			mdRadius = oArc.Radius
			mdTotalAngle = oArc.TotalAngle
			mbIsClockWise = (mdBulge < 0.0)
			zzCalcAngles()
		End If


	End Sub
	Public Sub New241218(ByVal oPolyline As Polyline, Optional ByVal bSameDirection As Boolean = True)
		'	Me.New(TPlnPoint.Point3dTo2d(oArc.Center), oArc.Radius, oArc.StartAngle, oArc.EndAngle, bSameDirection)
		'  moArc = oArc

		'	DMAcadExt.AcadDocument.WriteDebugMessageN("Pl Start<->End", oArc.StartPoint, oArc.EndPoint)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("Sgm Start<->End", oArc.GetPoint2dAt(0), oArc.GetPoint2dAt(1), oArc.GetBulgeAt(0))


		mbSameDirection = bSameDirection
		If True Then

			If bSameDirection Then
				MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oPolyline.StartPoint)
				MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oPolyline.EndPoint)
				'	Me.mdStartAngle = oArc.StartAngle
				'	Me.mdEndAngle = oArc.EndAngle
				mbIsClockWise = True
				mdBulge = oPolyline.GetBulgeAt(0)
			ElseIf True Then
				MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oPolyline.EndPoint)
				MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oPolyline.StartPoint)
				'	Me.mdStartAngle = oArc.EndAngle
				'Me.mdEndAngle = oArc.StartAngle

				mbIsClockWise = False
				mdBulge = -oPolyline.GetBulgeAt(0)
			End If

		End If
		'zzNew(oPolyline.GetPoint2dAt(0), oPolyline.GetPoint2dAt(1), mdBulge)
		zzNew(MyBase.dtStartPoint, MyBase.dtEndPoint, mdBulge)



		'  oArc.GeometricExtents

		Me.mtHandle = oPolyline.Handle


		'  AcadDocument.WriteDebugMessage(("!Center:" & TPlnPoint.DispPoint(Me.mtCenter) & "; Radius=" & Convert.ToString(Me.mdRadius)) & "; Bulge=" & Convert.ToString(Me.Bulge))
		'  AcadDocument.WriteDebugMessage(("!StartPoint:" & TPlnPoint.DispPoint(Me.dtStartPoint) & "; EndPoint=" & Convert.ToString(Me.dtEndPoint) & "; ClockWise=" & Convert.ToString(Me.mbIsClockWise)))

		'  AcadDocument.WriteDebugMessage("!StartAngle:" & Me.zzDispAngle(Me.mdStartAngle, 1) & "; EndAngle:" & Me.zzDispAngle(Me.mdEndAngle, 1) & "; TotalAngle:" & Me.zzDispAngle(Me.mdTotalAngle, 1) & "; VAngle:" & Me.zzDispAngle(MyBase.ddVectorAngle, 1))
	End Sub
	Public Sub New201218(ByVal oPolyline As Polyline, Optional ByVal bSameDirection As Boolean = True)
		'	Me.New(TPlnPoint.Point3dTo2d(oArc.Center), oArc.Radius, oArc.StartAngle, oArc.EndAngle, bSameDirection)
		'  moArc = oArc

		'	DMAcadExt.AcadDocument.WriteDebugMessageN("Pl Start<->End", oArc.StartPoint, oArc.EndPoint)
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("Sgm Start<->End", oArc.GetPoint2dAt(0), oArc.GetPoint2dAt(1), oArc.GetBulgeAt(0))


		Me.mbSameDirection = bSameDirection
		If True Then

			If bSameDirection Then
				MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oPolyline.StartPoint)
				MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oPolyline.EndPoint)
				'	Me.mdStartAngle = oArc.StartAngle
				'	Me.mdEndAngle = oArc.EndAngle
				Me.mbIsClockWise = True
				mdBulge = oPolyline.GetBulgeAt(0)
			ElseIf True Then
				MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oPolyline.EndPoint)
				MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oPolyline.StartPoint)
				'	Me.mdStartAngle = oArc.EndAngle
				'Me.mdEndAngle = oArc.StartAngle

				Me.mbIsClockWise = False
				mdBulge = -oPolyline.GetBulgeAt(0)
			End If

		End If
		zzNew(oPolyline.GetPoint2dAt(0), oPolyline.GetPoint2dAt(1), mdBulge)
		'	DMCommon.ExcelLogAW4.SetNextValue(0, "oArc", MyBase.dtStartPoint.ToString(), MyBase.dtEndPoint.ToString(), mdBulge)

		'  oArc.GeometricExtents

		Me.mtHandle = oPolyline.Handle


		'  AcadDocument.WriteDebugMessage(("!Center:" & TPlnPoint.DispPoint(Me.mtCenter) & "; Radius=" & Convert.ToString(Me.mdRadius)) & "; Bulge=" & Convert.ToString(Me.Bulge))
		'  AcadDocument.WriteDebugMessage(("!StartPoint:" & TPlnPoint.DispPoint(Me.dtStartPoint) & "; EndPoint=" & Convert.ToString(Me.dtEndPoint) & "; ClockWise=" & Convert.ToString(Me.mbIsClockWise)))

		'  AcadDocument.WriteDebugMessage("!StartAngle:" & Me.zzDispAngle(Me.mdStartAngle, 1) & "; EndAngle:" & Me.zzDispAngle(Me.mdEndAngle, 1) & "; TotalAngle:" & Me.zzDispAngle(Me.mdTotalAngle, 1) & "; VAngle:" & Me.zzDispAngle(MyBase.ddVectorAngle, 1))
	End Sub

	Public Sub New(ByVal tCenter As Point2d, ByVal dRadius As Double, ByVal dStartAngle As Double, ByVal dEndAngle As Double, Optional ByVal bSameDirection As Boolean = True)
		Me.mbSameDirection = bSameDirection
		mtCenter = tCenter
		mdRadius = dRadius

		If bSameDirection Then
			'   MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oArc.StartPoint)
			'  MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oArc.EndPoint)
			mdStartAngle = dStartAngle
			mdEndAngle = dEndAngle

			mbIsClockWise = True
		Else
			'  MyBase.dtStartPoint = TPlnPoint.Point3dTo2d(oArc.EndPoint)
			'  MyBase.dtEndPoint = TPlnPoint.Point3dTo2d(oArc.StartPoint)
			Me.mdStartAngle = mdEndAngle
			Me.mdEndAngle = dStartAngle

			Me.mbIsClockWise = False
		End If

		zzCalcTotalAngleA()

		zzCalcPoints()
		MyBase.Init()
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
		zzCalcTotalAngleA()
		zzCalcPoints()
		MyBase.Init()
		'	Else
		'	mdStartAngle = mtEndVector.Angle
		'	mdEndAngle = mtStartVector.Angle
		'	End If


	End Sub


	Public Sub New(ByVal oBulgeVertex As BulgeVertex, ByVal tNextPoint As Point2d)
		zzNew(oBulgeVertex.Vertex, tNextPoint, oBulgeVertex.Bulge)
	End Sub



	Public Sub New(ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d, ByVal dBulge As Double)
		zzNew(tStartPoint, tEndPoint, dBulge)



	End Sub
	Private Sub zzNew(ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d, ByVal dBulge As Double)
		mdBulge = dBulge
		If dBulge <> 0.0 Then
			Dim tMidPoint As Point2d = New Point2d(0.5 * (tStartPoint.X + tEndPoint.X), 0.5 * (tStartPoint.Y + tEndPoint.Y))
			Dim dAlfa As Double = Math.Atan(dBulge)
			Dim dDeltaX As Double = tEndPoint.X - tStartPoint.X
			Dim dDeltaY As Double = tEndPoint.Y - tStartPoint.Y
			Dim dDist As Double = Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY)
			Dim dA12 As Double
			Dim dHeight As Double
			Dim dRH As Double
			If dDist > 0.0001 Then
				dA12 = Math.Atan2(dDeltaY, dDeltaX)
				dHeight = dDist * dBulge * 0.5
				mdRadius = Math.Abs(dDist / 4.0 * (dBulge + 1.0 / dBulge))
				dRH = mdRadius - Math.Abs(dHeight)
				If dBulge < 0.0 Then
					dRH = -dRH
				End If
				mtCenter = New Point2d(tMidPoint.X + dRH * Math.Cos(dA12 + 0.5 * Math.PI), tMidPoint.Y + dRH * Math.Sin(dA12 + 0.5 * Math.PI))
				mdStartAngle = Math.Atan2((tStartPoint.Y - mtCenter.Y), (tStartPoint.X - mtCenter.X))
				mdEndAngle = Math.Atan2((tEndPoint.Y - mtCenter.Y), (tEndPoint.X - mtCenter.X))
				If mdStartAngle < 0 Then
					mdStartAngle += 2.0 * Math.PI
				End If
				If mdEndAngle < 0 Then
					mdEndAngle += 2.0 * Math.PI
				End If
				'	DMAcadExt.AcadDocument.WriteDebugMessageN("Angle Start<->End", mdStartAngle, mdEndAngle)
				mbIsClockWise = True
				mbIsClockWise = (mdBulge < 0.0)
				mbSameDirection = True

				zzCalcTotalAngleA()
				'	DMCommon.ExcelLogAW5.SetNextValue(0, "???zzNew", mbIsClockWise, mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle * mdRadToDeg)
				zzCalcPoints()
				'	DMAcadExt.AcadDocument.WriteDebugMessageN("CalcPoint Start<->End", MyBase.dtStartPoint, MyBase.dtEndPoint, mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle, mbSameDirection)
				MyBase.Init()
			End If
		End If

	End Sub
	Public Sub SetDir(bIsClockWise As Boolean, bSameDirection As Boolean)
		mbIsClockWise = bIsClockWise
		mbSameDirection = bSameDirection
	End Sub

	Public ReadOnly Property EntityHandle As Handle Implements IUD_Link.EntityHandle
		Get
			Return mtHandle
		End Get
	End Property
	Public Overrides Function GetX(ByVal dY As Double) As Double   '
		Dim dPY As Double = dY - mtCenter.Y
		Dim dPX As Double = Math.Sqrt(mdRadius * mdRadius - dPY * dPY)
		If db Then
			dPX = -dPX
		End If
		Return mtCenter.X + dPX
	End Function

	Public Overrides Function GetY(ByVal dX As Double) As Double   '
		Dim dPX As Double = dX - mtCenter.X
		Dim dPY As Double = Math.Sqrt(mdRadius * mdRadius - dPX * dPX)
		If db Then
			dPY = -dPY
		End If
		Return mtCenter.Y + dPY
	End Function
	Public Function GetInnerPoints(ByVal dTolerance As Double) As Point2d()


		Dim dTolerAngle As Double = 2.0 * Math.Acos(1.0 - dTolerance / mdRadius)

		Dim dTotalAngle As Double
		If mbIsClockWise Then
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Else
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle > 0.0 Then dTotalAngle -= Math.PI + Math.PI
		End If


		'	If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Dim iPart As Integer
		Dim dPart As Double = Math.Ceiling(Math.Abs(dTotalAngle) / dTolerAngle)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

		Try
			iPart = Convert.ToInt32(dPart)
			'Alla
			'Dim taRes1(-1) As Point2d
		Catch oEx As Exception

			DMAcadExt.DMApp.MsgBox("21_380", mdRadius, dTolerance, dTolerAngle, dPart, dTotalAngle, iPart, dtStartPoint, dtEndPoint)

		End Try
		If iPart < 2 Then
			'Return Nothing
			DMCommon.Debug.MsgBox("", iPart, dTotalAngle, dTolerAngle)
			iPart = 2
		End If

		Try
			Dim taRes(iPart - 2) As Point2d
			If iPart >= 2 Then
				dTolerAngle = dTotalAngle / iPart
				'	DMAcadExt.AcadDocument.WriteMessage("InnerAngles=" & CStr(mdStartAngle * dToDegree) & "," & CStr(mdEndAngle * dToDegree) & " Toler" & CStr(dTolerAngle * dToDegree) & " Total" & CStr(dTotalAngle * dToDegree))
				For iIndex As Integer = 1 To iPart - 1
					dAngle = mdStartAngle + iIndex * dTolerAngle
					tPoint = zzPointByAngle(dAngle)
					'tPoint = New Autodesk.AutoCAD.Geometry.Point2d(mtCenter.X + mdRadius * Math.Cos(dAngle), mtCenter.Y + mdRadius * Math.Sin(dAngle))
					'DMAcadExt.AcadDocument.WriteMessage("Inner=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					taRes(iIndex - 1) = tPoint
				Next
			End If
			'AcadDocument.WriteDebugMessageN("GetInnerPoints", iPart, taRes.GetUpperBound(0), dTotalAngle, dTolerAngle, mdRadius)
			Return taRes
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "GetInnerPoints-TplnArc")
			Return Nothing
		End Try


	End Function
	Public Function GetInnerPointsNew(ByVal dTolerance As Double) As Point2d()


		Dim dTolerAngle As Double = 2.0 * Math.Acos(1.0 - dTolerance / mdRadius)

		Dim dTotalAngle As Double = mdTotalAngle



		'	If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Dim iPart As Integer
		Dim dPart As Double = Math.Ceiling(Math.Abs(dTotalAngle) / dTolerAngle)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

		Try
			iPart = Convert.ToInt32(dPart)
			'Alla
		Catch oEx As Exception

			DMAcadExt.DMApp.MsgBox("21_380", mdRadius, dTolerance, dTolerAngle, dPart, dTotalAngle, iPart, dtStartPoint, dtEndPoint)

		End Try
		If iPart < 3 Then
			Return Nothing
		End If

		Try
			Dim taRes(iPart - 2) As Point2d
			dTolerAngle = dTotalAngle / iPart
			'	DMAcadExt.AcadDocument.WriteMessage("InnerAngles=" & CStr(mdStartAngle * dToDegree) & "," & CStr(mdEndAngle * dToDegree) & " Toler" & CStr(dTolerAngle * dToDegree) & " Total" & CStr(dTotalAngle * dToDegree))
			For iIndex As Integer = 1 To iPart - 1
				dAngle = mdStartAngle + iIndex * dTolerAngle
				tPoint = zzPointByAngle(dAngle)
				'tPoint = New Autodesk.AutoCAD.Geometry.Point2d(mtCenter.X + mdRadius * Math.Cos(dAngle), mtCenter.Y + mdRadius * Math.Sin(dAngle))
				'DMAcadExt.AcadDocument.WriteMessage("Inner=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
				taRes(iIndex - 1) = tPoint
			Next
			'AcadDocument.WriteDebugMessageN("GetInnerPoints", iPart, taRes.GetUpperBound(0), dTotalAngle, dTolerAngle, mdRadius)
			Return taRes
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "GetInnerPoints-TplnArc")
			Return Nothing
		End Try


	End Function
	Public Overrides Function GetBulge() As Double
		If mdBulge = 0.0 Then

			Return zzCalculateBulgeNew()
		Else
			Return mdBulge
		End If

	End Function

	Private Function zzCalculateBulgeAAA() As Double

		Dim dSign As Double
		Dim dTotalAngle As Double = mdTotalAngle
		If mdTotalAngle = 0.0 Then
			dTotalAngle = Me.mdEndAngle - Me.mdStartAngle
		Else
			dTotalAngle = mdTotalAngle
		End If
		If Me.mbIsClockWise Then
			'''''''''''''''''''' dSign = 1.0



			If (dTotalAngle < 0) Then
				dTotalAngle = (dTotalAngle + 2 * Math.PI)
			End If

		Else
			If Me.mbSameDirection Then
				dSign = 1.0
			Else
				dSign = 1.0
			End If
			dTotalAngle = -dTotalAngle
			If (dTotalAngle < 0) Then
				dTotalAngle = (dTotalAngle + 2 * Math.PI)
			Else

			End If
			dSign = -1.0
			' DMAcadExt.AcadDocument.WriteMessage("ISCW-=" & mbIsClockWise.ToString() & "Dir=" & mbSameDirection.ToString() & "TA=" & dTotalAngle.ToString() & "B=" & (dSign * Math.Tan((dTotalAngle * 0.25))).ToString())
		End If
		'   DMAcadExt.AcadDocument.WriteMessage("An= " & (dTotalAngle * 180 / Math.PI).ToString() & " T=" & CStr(Math.Tan(dTotalAngle * 0.25)))
		If dTotalAngle > 10 * Math.PI Then
			If mbIsClockWise Then
				Return (dSign * Math.Tan((dTotalAngle * 0.25)))
			Else
				Return 1.0 / (dSign * Math.Tan((dTotalAngle * 0.25)))
			End If

		Else
			Return (dSign * Math.Tan((dTotalAngle * 0.25)))
		End If



	End Function
	Private Function zzCalculateBulgeNew() As Double
		Dim dSign As Double
		If Me.mbIsClockWise Then
			dSign = -1.0
		Else
			dSign = 1.0
		End If
		Return dSign * Math.Tan((mdTotalAngle * 0.25))
	End Function

	Public Overloads Function GetBulgeZ() As Double
		Dim dSign As Double
		Dim dTotalAngle As Double
		If mbIsClockWise Then
			dSign = 1.0
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Else
			dSign = -1.0
			dTotalAngle = mdEndAngle - mdStartAngle
			If dTotalAngle > 0.0 Then dTotalAngle -= Math.PI + Math.PI
		End If
		Return dSign * Math.Tan(dTotalAngle * 0.25) '* 127.0
	End Function

	Public Function GetAcadArc() As Arc
		If mbIsClockWise Then
			Return New Arc(TPlnPoint.Point2dTo3d(mtCenter), mdRadius, mdStartAngle, mdEndAngle)
		Else
			Return New Arc(TPlnPoint.Point2dTo3d(mtCenter), mdRadius, mdEndAngle, mdStartAngle)
		End If


	End Function
	Public Function GetBoundingBox() As TPlnBoundingBox Implements IUD_Link.GetBoundingBox

		Dim oArc As Arc = GetAcadArc()
		Return New TPlnBoundingBox(oArc.GeometricExtents)



	End Function
	Public Function GetQuarterPoint(ByVal bFirst As Boolean) As Point2d
		Dim dMidAngle As Double
		Dim dK As Double
		If bFirst Then
			dK = 0.25
		Else
			dK = 0.75
		End If
		dMidAngle = dK * (mdStartAngle + mdEndAngle)
		Return zzPointByAngle(dMidAngle)
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
	Public Property RadiusSrc As Double
		Get
			Return Me.mdRadius
		End Get
		Set(ByVal dValue As Double)
			Me.mdRadius = dValue
		End Set
	End Property
	Private Function zzPointByAngle(ByVal dAngle As Double) As Point2d
		Return New Autodesk.AutoCAD.Geometry.Point2d(mtCenter.X + mdRadius * Math.Cos(dAngle), mtCenter.Y + mdRadius * Math.Sin(dAngle))
	End Function
	Private Function zzCalcAngleByPoint(tPoint As Point2d) As Double
		Dim tVector2d As Vector2d = New Vector2d(tPoint.X - mtCenter.X, tPoint.Y - mtCenter.Y)
		Return tVector2d.Angle

	End Function
	Private Function zzCalcPoint(ByVal dAngle As Double) As Point2d
		Return New Point2d(mtCenter.X + Math.Cos(dAngle) * mdRadius, mtCenter.Y + Math.Sin(dAngle) * mdRadius)
	End Function
	Private Sub zzCalcAngles()
		mdStartAngle = zzCalcAngleByPoint(MyBase.dtStartPoint)
		mdEndAngle = zzCalcAngleByPoint(MyBase.dtEndPoint)

	End Sub
	Private Sub zzCalcPoints()
		If mbSameDirection Then
			MyBase.dtStartPoint = zzCalcPoint(mdStartAngle)
			MyBase.dtEndPoint = zzCalcPoint(mdEndAngle)
		Else
			MyBase.dtStartPoint = zzCalcPoint(mdEndAngle)
			MyBase.dtEndPoint = zzCalcPoint(mdStartAngle)
		End If
		dbHasBulge = True
		'System.Windows.Forms.MessageBox.Show(TPlnPoint.DispPoint(dtStartPoint), "24_690")
	End Sub
	Private Sub zzCalcTotalAngleA()
		If Not mbIsClockWise Then
			mdTotalAngle = (mdEndAngle + 2 * Math.PI - mdStartAngle) Mod (2 * Math.PI)
		Else
			mdTotalAngle = (mdStartAngle + 2 * Math.PI - mdEndAngle) Mod (2 * Math.PI)
		End If

	End Sub
	Private Sub zzCalcTotalAngle()
		mdTotalAngle = (mdEndAngle + 2 * Math.PI - mdStartAngle) Mod (2 * Math.PI)
	End Sub
	Private Function zzDispAngle(ByVal dAngle As Double, ByVal iDigits As Integer) As String
		'	Dim dToDegree As Double = 180 / Math.PI
		'	Return CStr(Math.Round(dAngle * dToDegree, 1))
		Return FormatNumber(dAngle * mdRadToDeg, iDigits)

	End Function
	Public ReadOnly Property Bulge As Double Implements IUD_Link.Bulge
		Get
			Return Me.GetBulge
		End Get
	End Property

	Public ReadOnly Property Coordinates As String Implements IUD_Link.Coordinates
		Get
			Return (Me.dtStartPoint.ToString & " ; " & Me.dtEndPoint.ToString)
		End Get
	End Property

	Public Function Copy() As IUD_Link Implements IUD_Link.Copy
		'  Return New TplnArc(moArc, mbSameDirection)
		Dim oNewArc As TplnArc
		If mdBulge = 0.0 Then
			oNewArc = New TplnArc(Me.mtCenter, Me.mdRadius, Me.mdStartAngle, Me.mdEndAngle, True)
		Else
			oNewArc = New TplnArc(Me.StartPoint, Me.EndPoint, mdBulge)

			''''''''''''''DMCommon.ExcelLogAW4.SetNextValue(0, "Copy 1", MyBase.dtStartPoint.ToString(), MyBase.dtEndPoint.ToString(), mdBulge, oNewArc.Bulge)
		End If

		oNewArc.SetDir(mbIsClockWise, mbSameDirection)
		''''''''''''''DMCommon.ExcelLogAW4.SetNextValue(0, "Copy 2", MyBase.dtStartPoint.ToString(), MyBase.dtEndPoint.ToString(), mdBulge, oNewArc.Bulge)
		Return oNewArc


	End Function




	Public Overloads ReadOnly Property EndPoint() As Point2d Implements IUD_Link.EndPoint
		Get
			Return dtEndPoint
		End Get
	End Property
	Public Overloads ReadOnly Property StartPoint() As Point2d Implements IUD_Link.StartPoint
		Get
			Return dtStartPoint
		End Get
	End Property
	Public Sub PrintInfo()
		AcadDocument.WriteMessage(String.Concat(New String() {"Handle=", Me.mtHandle.ToString, "; Center:", TPlnPoint.DispPoint(Me.mtCenter), "; Radius=", Convert.ToString(Me.mdRadius)}))
		AcadDocument.WriteMessage(String.Concat(New String() {"StartPoint=", Me.dtStartPoint.ToString, "; EndPoint:", Me.dtEndPoint.ToString, "; Radius=", Convert.ToString(Me.mdRadius)}))
		AcadDocument.WriteMessage(String.Concat(New String() {"StartAngle:", Me.zzDispAngle(Me.mdStartAngle, 2), "; EndAngle:", Me.zzDispAngle(Me.mdEndAngle, 2), "; TotalAngle:", Me.zzDispAngle(Me.mdTotalAngle, 2), "; VAngle:", Me.zzDispAngle(MyBase.ddVectorAngle, 2)}))
	End Sub


	Public Sub Extend201215(ByVal oLink As IUD_Link) ''''''''''''''Implements UD_Link.Extend

		Dim oArc As TplnArc = TryCast(oLink, TplnArc)
		Dim dK As Double = 180.0 / Math.PI
		If (oArc IsNot Nothing) Then
			If mtCenter.IsEqualTo(mtTestPoint, New Tolerance(0.1, 0.1)) Then
				DMAcadExt.AcadDocument.WriteMessage(dtStartPoint.ToString() & "<>" & dtEndPoint.ToString())
				DMAcadExt.AcadDocument.WriteMessage("A=" & (Me.StartAngle * dK).ToString() & "<>" & (Me.mdEndAngle * dK).ToString())
				DMAcadExt.AcadDocument.WriteMessage("B=" & (oArc.StartAngle * dK).ToString() & "<>" & (oArc.EndAngle * dK).ToString())
			End If
			Me.mdEndAngle = oArc.EndAngle
			Me.mdTotalAngle = (Me.TotalAngle + oArc.TotalAngle)


			Me.mdTotalAngle = (Me.mdTotalAngle Mod Math.PI)
		End If
	End Sub
	Public Sub Extend_090516(ByVal oLink As IUD_Link)
		'Dim tTestPoint As Point2d = New Point2d(181716.926, 647279.982)
		Dim oArc As TplnArc = TryCast(oLink, TplnArc)

		Dim bTest As Boolean
		If (oArc IsNot Nothing) Then
			Dim tNewStartPoint As Point2d
			Dim tNewEndPoint As Point2d

			If dtEndPoint.IsEqualTo(oLink.StartPoint) Then
				tNewStartPoint = dtStartPoint
				tNewEndPoint = oLink.EndPoint
			ElseIf dtEndPoint.IsEqualTo(oLink.EndPoint) Then
				System.Windows.Forms.MessageBox.Show("!!!????", "07_151a")
				tNewStartPoint = dtStartPoint
				tNewEndPoint = oLink.StartPoint
			ElseIf dtStartPoint.IsEqualTo(oLink.EndPoint) Then

				tNewStartPoint = oLink.StartPoint
				tNewEndPoint = dtEndPoint
				' mbIsClockWise = Not mbIsClockWise
			ElseIf dtStartPoint.IsEqualTo(oLink.StartPoint) Then
				System.Windows.Forms.MessageBox.Show("!!!????", "07_153a")
				tNewStartPoint = oLink.EndPoint
				tNewEndPoint = dtEndPoint
				'  mbIsClockWise = Not mbIsClockWise
			Else
				System.Windows.Forms.MessageBox.Show("!!!????", "07_144a")
			End If




			If False AndAlso mtCenter.GetDistanceTo(mtTestPoint) < 2.0 Then
				bTest = True
				DMAcadExt.AcadDocument.WriteMessage(dtStartPoint.ToString() & " - " & dtEndPoint.ToString())
				DMAcadExt.AcadDocument.WriteMessage("A=" & (Me.StartAngle * mdRadToDeg).ToString() & " - " & (Me.EndAngle * mdRadToDeg).ToString() & " T=" & (Me.TotalAngle * mdRadToDeg).ToString())
				DMAcadExt.AcadDocument.WriteMessage("B=" & (oArc.StartAngle * mdRadToDeg).ToString() & " - " & (oArc.EndAngle * mdRadToDeg).ToString() & " T=" & (oArc.TotalAngle * mdRadToDeg).ToString())
			End If
			zzUpdateByPoints(tNewStartPoint, tNewEndPoint)
			' zzUpdateByPoints(tNewStartPoint, tNewEndPoint)

			If bTest Then
				DMAcadExt.AcadDocument.WriteMessage("ANew=" & (Me.StartAngle * mdRadToDeg).ToString() & " - " & (Me.EndAngle * mdRadToDeg).ToString() & " T=" & (Me.TotalAngle * mdRadToDeg).ToString())
			End If
		End If
	End Sub
	Public Sub Extend(ByVal oLink As IUD_Link) Implements IUD_Link.Extend
		'Dim tTestPoint As Point2d = New Point2d(181716.926, 647279.982)
		Dim oArc As TplnArc = TryCast(oLink, TplnArc)
		mdBulge = 0.0

		If (oArc IsNot Nothing) Then
			Dim tNewStartPoint As Point2d
			Dim tNewEndPoint As Point2d

			If dtEndPoint.IsEqualTo(oLink.StartPoint) Then
				'DMCommon.ExcelLogAW5.SetNextValue(0, "Ext1", Me.mdStartAngle * mdRadToDeg, Me.mdEndAngle * mdRadToDeg, mbIsClockWise, Me.Bulge, oArc.StartAngle * mdRadToDeg, oArc.EndAngle * mdRadToDeg, oArc.IsClockWise, oArc.Bulge)
				'  dtStartPoint = dtStartPoint
				dtEndPoint = oLink.EndPoint
				Me.mdEndAngle = Me.zzAngleByPoint(dtEndPoint)
				'	Me.mdEndAngle = Me.zzAngleByPoint(dtEndPoint)

			ElseIf dtEndPoint.IsEqualTo(oLink.EndPoint) Then
				System.Windows.Forms.MessageBox.Show("!!!????", "07_151aa")
				tNewStartPoint = dtStartPoint
				tNewEndPoint = oLink.StartPoint
				dtEndPoint = oLink.StartPoint
				Me.mdEndAngle = Me.zzAngleByPoint(dtEndPoint)
			ElseIf dtStartPoint.IsEqualTo(oLink.EndPoint) Then
				'DMCommon.ExcelLogAW5.SetNextValue(0, "Ext2", Me.mdStartAngle * mdRadToDeg, Me.mdEndAngle * mdRadToDeg, mbIsClockWise, Me.Bulge, oArc.StartAngle * mdRadToDeg, oArc.EndAngle * mdRadToDeg, oArc.IsClockWise, oArc.Bulge)
				dtStartPoint = oLink.StartPoint
				Me.mdStartAngle = Me.zzAngleByPoint(dtStartPoint)
				'  tNewEndPoint = dtEndPoint
				'	mbIsClockWise = Not mbIsClockWise
			ElseIf dtStartPoint.IsEqualTo(oLink.StartPoint) Then
				System.Windows.Forms.MessageBox.Show("!!!????", "07_153dd")
				tNewStartPoint = oLink.EndPoint
				tNewEndPoint = dtEndPoint
				dtStartPoint = oLink.StartPoint
				Me.mdStartAngle = Me.zzAngleByPoint(dtStartPoint)
				'  mbIsClockWise = Not mbIsClockWise
			Else
				System.Windows.Forms.MessageBox.Show("!!!????", "07_144z")
			End If




			'  zzUpdateByPoints(tNewStartPoint, tNewEndPoint)
			' zzUpdateByPoints(tNewStartPoint, tNewEndPoint)


			'DMAcadExt.AcadDocument.WriteMessage("Extend=" & (Me.StartAngle * mdRadToDeg).ToString() & " - " & (Me.EndAngle * mdRadToDeg).ToString() & " T=" & (Me.TotalAngle * mdRadToDeg).ToString())
			If mbIsClockWise Then
				mdTotalAngle = mdStartAngle - mdEndAngle
			Else
				mdTotalAngle = mdEndAngle - mdStartAngle
			End If

			If mdTotalAngle < 0.0 Then

				mdTotalAngle += 2 * Math.PI
				''''''''''''''''mdTotalAngle = -mdTotalAngle
				'	mbIsClockWise = Not mbIsClockWise
			End If

			Dim dBulge As Double = zzCalculateBulgeNew()
			'DMCommon.ExcelLogAW5.SetNextValue(0, "ArcExt", mbIsClockWise, mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle * mdRadToDeg, dBulge)
			'DMCommon.ExcelLogAW5.SetNextValue(0, "ArcExtend", ToDegree(mdStartAngle), ToDegree(mdEndAngle), ToDegree(mdTotalAngle), dBulge)
		End If
	End Sub


	Public Sub Round231215(ByVal iDigits As Integer)
		Dim tStartPoint As Point2d = TPlnPoint.RoundPointTo2d(MyBase.dtStartPoint, iDigits)
		Dim tEndPoint As Point2d = TPlnPoint.RoundPointTo2d(MyBase.dtEndPoint, iDigits)
		Dim dOldRadius As Double = Me.mdRadius
		Me.mdRadius = Math.Round(Me.mdRadius, iDigits)

		Dim num6 As Double = (0.5 * tStartPoint.GetDistanceTo(tEndPoint))
		Dim d As Double = (num6 / Me.mdRadius)
		Dim num5 As Double = Math.Sqrt((1 - (d * d)))
		Dim num As Double = Math.Asin(d)
		Dim dBulge As Double = Math.Tan((0.5 * num))
		'   AcadDocument.WriteMessage("!!Bulges=" & Me.GetBulge() & "; " & dBulge)
		'   AcadDocument.WriteMessage("!!Radius=" & dOldRadius & "; " & mdRadius)

		Dim oArc2dFlagTrue As New CircularArc2d(tStartPoint, tEndPoint, dBulge, True)
		Dim oArc2dFlagFalse As New CircularArc2d(tStartPoint, tEndPoint, dBulge, False)
		Dim dDistanceTo As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagTrue.Center)
		Dim num3 As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagFalse.Center)
		If (dDistanceTo < num3) Then
			Me.mtCenter = oArc2dFlagTrue.Center
		Else
			Me.mtCenter = oArc2dFlagFalse.Center
		End If
		MyBase.dtStartPoint = tStartPoint
		MyBase.dtEndPoint = tEndPoint
		Me.mdStartAngle = Me.zzAngleByPoint(tStartPoint)
		Me.mdEndAngle = Me.zzAngleByPoint(tEndPoint)
	End Sub
	Public Sub Round(ByVal iDigits As Integer, tTolerance As Autodesk.AutoCAD.Geometry.Tolerance, ByRef oResStartPoint As TPlnPoint, ByRef oResEndPoint As TPlnPoint)

		Dim tStartPoint As Point2d = TPlnPoint.RoundPointTo2d(MyBase.dtStartPoint, iDigits)
		Dim tEndPoint As Point2d = TPlnPoint.RoundPointTo2d(MyBase.dtEndPoint, iDigits)

		Dim dOldRadius As Double = Me.mdRadius
		Me.mdRadius = Math.Round(Me.mdRadius, iDigits)

		Dim num6 As Double = (0.5 * tStartPoint.GetDistanceTo(tEndPoint))
		Dim d As Double = (num6 / Me.mdRadius)
		Dim num5 As Double = Math.Sqrt((1 - (d * d)))
		Dim num As Double = Math.Asin(d)
		Dim dBulge As Double = Math.Tan((0.5 * num))
		'   AcadDocument.WriteMessage("!!Bulges=" & Me.GetBulge() & "; " & dBulge)
		'   AcadDocument.WriteMessage("!!Radius=" & dOldRadius & "; " & mdRadius)

		Dim oArc2dFlagTruePlus As New CircularArc2d(tStartPoint, tEndPoint, dBulge, True)
		Dim oArc2dFlagFalsePlus As New CircularArc2d(tStartPoint, tEndPoint, dBulge, False)
		Dim oArc2dFlagTrueMinus As New CircularArc2d(tStartPoint, tEndPoint, -dBulge, True)
		Dim oArc2dFlagFalseMinus As New CircularArc2d(tStartPoint, tEndPoint, -dBulge, False)

		Dim dDistanceToTruePlus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagTruePlus.Center))
		Dim dDistanceToFalsePlus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagFalsePlus.Center))

		'  AcadDocument.WriteMessage("!!DD+ " & dDistanceToTruePlus.ToString() & "; -!" & dDistanceToFalsePlus.ToString())
		Dim tCenterPlus, tCenterMinus As Point2d
		Dim tRadiusPlus, tRadiusMinus As Double

		Dim dDistancePlus, dDistanceMinus As Double
		If (dDistanceToTruePlus < dDistanceToFalsePlus) Then
			dDistancePlus = dDistanceToTruePlus
			tCenterPlus = oArc2dFlagTruePlus.Center
			tRadiusPlus = oArc2dFlagTruePlus.Radius
		Else
			dDistancePlus = dDistanceToFalsePlus
			tCenterPlus = oArc2dFlagFalsePlus.Center
			tRadiusPlus = oArc2dFlagFalsePlus.Radius
		End If
		'  AcadDocument.WriteMessage("!!tCenterPlus= " & tCenterPlus.ToString() & "; dDistancePlus!= " & dDistancePlus.ToString() & "; dRadiusPlus!= " & dDistancePlus.ToString())
		Dim dDistanceToTrueMinus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagTrueMinus.Center))
		Dim dDistanceToFalseMinus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagFalseMinus.Center))
		'  AcadDocument.WriteMessage("!!MINUS! + " & dDistanceToTrueMinus.ToString() & "; -" & dDistanceToFalseMinus.ToString())
		If (dDistanceToTrueMinus < dDistanceToFalseMinus) Then
			dDistanceMinus = dDistanceToTrueMinus
			tCenterMinus = oArc2dFlagTrueMinus.Center
			tRadiusMinus = oArc2dFlagTrueMinus.Radius
		Else
			dDistanceMinus = dDistanceToFalseMinus
			tCenterMinus = oArc2dFlagFalseMinus.Center
			tRadiusMinus = oArc2dFlagFalseMinus.Radius
		End If
		'   AcadDocument.WriteMessage("!!tCenterMinus  " & tCenterMinus.ToString() & "; dDistanceMinus! " & dDistanceMinus.ToString() & "; dRadiusMinus!= " & dDistancePlus.ToString())
		If (dDistancePlus < dDistanceMinus) Then
			Me.mtCenter = tCenterPlus
			Me.mdRadius = tRadiusPlus
		Else
			Me.mtCenter = tCenterMinus
			Me.mdRadius = tRadiusMinus
		End If
		'  AcadDocument.WriteMessage("!!Radius  " & Me.mdRadius.ToString() & ";  " & oArc2dFlagTruePlus.Radius.ToString())
		'   AcadDocument.WriteMessage("!!Dist Start " & Me.mtCenter.GetDistanceTo(tStartPoint) & "  !!End  " & Me.mtCenter.GetDistanceTo(tEndPoint))



		If Not MyBase.dtStartPoint.IsEqualTo(tStartPoint, tTolerance) Then
			oResStartPoint = New TPlnPoint(tStartPoint)
		End If
		If Not MyBase.dtEndPoint.IsEqualTo(tEndPoint, tTolerance) Then
			oResStartPoint = New TPlnPoint(tStartPoint)
		End If



		MyBase.dtStartPoint = tStartPoint
		MyBase.dtEndPoint = tEndPoint



		Me.mdStartAngle = Me.zzAngleByPoint(tStartPoint)
		Me.mdEndAngle = Me.zzAngleByPoint(tEndPoint)
	End Sub
	Private Sub zzUpdateByPointsPrev(tStartPoint As Point2d, ByRef tEndPoint As Point2d)
		Dim dHalfChord As Double = (0.5 * tStartPoint.GetDistanceTo(tEndPoint))
		Dim dCosAddAngle As Double = (dHalfChord / mdRadius)
		'    Dim num5 As Double = Math.Sqrt((1 - (dCosAddAngle * dCosAddAngle)))
		Dim dAngleTeta As Double = Math.Asin(dCosAddAngle)
		Dim dBulge As Double = Math.Tan((0.5 * dAngleTeta))
		'   AcadDocument.WriteMessage("!!Bulges=" & Me.GetBulge() & "; " & dBulge)
		'   AcadDocument.WriteMessage("!!Radius=" & dOldRadius & "; " & mdRadius)

		Dim oArc2dFlagTrue As New CircularArc2d(tStartPoint, tEndPoint, dBulge, True)
		Dim oArc2dFlagFalse As New CircularArc2d(tStartPoint, tEndPoint, dBulge, False)
		Dim dDistanceToTrue As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagTrue.Center)
		Dim dDistanceToFalse As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagFalse.Center)
		If (dDistanceToTrue < dDistanceToFalse) Then
			Me.mtCenter = oArc2dFlagTrue.Center
		Else
			Me.mtCenter = oArc2dFlagFalse.Center
		End If
		MyBase.dtStartPoint = tStartPoint
		MyBase.dtEndPoint = tEndPoint
		Me.mdStartAngle = Me.zzAngleByPoint(tStartPoint)
		Me.mdEndAngle = Me.zzAngleByPoint(tEndPoint)
		mdTotalAngle = (mdEndAngle + 2 * Math.PI - mdStartAngle) Mod 2 * Math.PI

	End Sub

	Private Sub zzUpdateByPoints(tStartPoint As Point2d, ByRef tEndPoint As Point2d)
		Dim dHalfChord As Double = (0.5 * tStartPoint.GetDistanceTo(tEndPoint))
		Dim dCosAddAngle As Double = (dHalfChord / mdRadius)
		'    Dim num5 As Double = Math.Sqrt((1 - (dCosAddAngle * dCosAddAngle)))
		Dim dAngleTeta As Double = Math.Asin(dCosAddAngle)
		Dim dBulge As Double = Math.Tan((0.5 * dAngleTeta))
		'   AcadDocument.WriteMessage("!!Bulges=" & Me.GetBulge() & "; " & dBulge)
		'   AcadDocument.WriteMessage("!!Radius=" & dOldRadius & "; " & mdRadius)

		Dim oArc2dFlagTrue As New CircularArc2d(tStartPoint, tEndPoint, dBulge, True)
		Dim oArc2dFlagFalse As New CircularArc2d(tStartPoint, tEndPoint, dBulge, False)
		Dim oArc2dFlagTrueMinus As New CircularArc2d(tStartPoint, tEndPoint, -dBulge, True)
		Dim oArc2dFlagFalseMinus As New CircularArc2d(tStartPoint, tEndPoint, -dBulge, False)

		Dim dDistanceToTrue As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagTrue.Center))
		Dim dDistanceToFalse As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagFalse.Center))

		'   AcadDocument.WriteMessage("!!DD+ " & dDistanceToTrue.ToString() & "; -!" & dDistanceToFalse.ToString())
		Dim tCenterPlus, tCenterMinus As Point2d
		Dim dDistancePlus, dDistanceMinus As Double
		If (dDistanceToTrue < dDistanceToFalse) Then
			dDistancePlus = dDistanceToTrue
			tCenterPlus = oArc2dFlagTrue.Center
		Else
			dDistancePlus = dDistanceToFalse
			tCenterPlus = oArc2dFlagFalse.Center
		End If
		'  AcadDocument.WriteMessage("!!tCenterPlus  " & tCenterPlus.ToString() & "; dDistancePlus! " & dDistancePlus.ToString())
		Dim dDistanceToTrueMinus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagTrueMinus.Center))
		Dim dDistanceToFalseMinus As Double = Math.Abs(Me.mtCenter.GetDistanceTo(oArc2dFlagFalseMinus.Center))
		'   AcadDocument.WriteMessage("!!MINUS! + " & dDistanceToTrueMinus.ToString() & "; -" & dDistanceToFalseMinus.ToString())
		If (dDistanceToTrueMinus < dDistanceToFalseMinus) Then
			dDistanceMinus = dDistanceToTrueMinus
			tCenterMinus = oArc2dFlagTrueMinus.Center
		Else
			dDistanceMinus = dDistanceToFalseMinus
			tCenterMinus = oArc2dFlagFalseMinus.Center
		End If
		'   AcadDocument.WriteMessage("!!tCenterMinus  " & tCenterMinus.ToString() & "; dDistanceMinus! " & dDistanceMinus.ToString())
		If (dDistancePlus < dDistanceMinus) Then
			Me.mtCenter = tCenterPlus
		Else
			Me.mtCenter = tCenterMinus
		End If



		MyBase.dtStartPoint = tStartPoint
		MyBase.dtEndPoint = tEndPoint
		Me.mdStartAngle = Me.zzAngleByPoint(tStartPoint)
		Me.mdEndAngle = Me.zzAngleByPoint(tEndPoint)
		'  mdTotalAngle = (mdEndAngle + 2 * Math.PI - mdStartAngle) Mod 2 * Math.PI

	End Sub
	Public Sub RoundPoint(iVertexIndex As Integer, ByVal iDigits As Integer)
		Dim tStartPoint As Point2d = Me.StartPoint
		Dim tEndPoint As Point2d = Me.EndPoint
		Dim dRadius As Double = Math.Round(Me.mdRadius, iDigits)
		Select Case iVertexIndex
			Case 0
				tStartPoint = TPlnPoint.RoundPointTo2d(MyBase.dtStartPoint, iDigits)
			Case 1
				tEndPoint = TPlnPoint.RoundPointTo2d(MyBase.dtEndPoint, iDigits)
		End Select
		SetValue(tStartPoint, tEndPoint, dRadius)
	End Sub
	Public Sub SetValue(tStartPoint As Point2d, tEndPoint As Point2d, dRadius As Double)
		Me.mdRadius = dRadius
		Dim num6 As Double = (0.5 * tStartPoint.GetDistanceTo(tEndPoint))
		Dim d As Double = (num6 / Me.mdRadius)
		Dim num5 As Double = Math.Sqrt((1 - (d * d)))
		Dim num As Double = Math.Asin(d)
		Dim dBulge As Double = Math.Tan((0.5 * num))
		'   AcadDocument.WriteMessage("!!Bulges=" & Me.GetBulge() & "; " & dBulge)
		'   AcadDocument.WriteMessage("!!Radius=" & dOldRadius & "; " & mdRadius)

		Dim oArc2dFlagTrue As New CircularArc2d(tStartPoint, tEndPoint, dBulge, True)
		Dim oArc2dFlagFalse As New CircularArc2d(tStartPoint, tEndPoint, dBulge, False)
		Dim dDistanceTo As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagTrue.Center)
		Dim num3 As Double = Me.mtCenter.GetDistanceTo(oArc2dFlagFalse.Center)
		If (dDistanceTo < num3) Then
			Me.mtCenter = oArc2dFlagTrue.Center
		Else
			Me.mtCenter = oArc2dFlagFalse.Center
		End If
		MyBase.dtStartPoint = tStartPoint
		MyBase.dtEndPoint = tEndPoint
		Me.mdStartAngle = Me.zzAngleByPoint(tStartPoint)
		Me.mdEndAngle = Me.zzAngleByPoint(tEndPoint)
	End Sub

	Public Function GetInfo(ByVal iDigits As Integer) As String Implements IUD_Link.GetInfo
		Return String.Concat(New String() {TplnArc.zzGetFormat(Me.StartPoint, iDigits), "=>", TplnArc.zzGetFormat(Me.EndPoint, iDigits), " B=", Strings.FormatNumber(Me.Bulge, iDigits, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " A: ", zzGetFormat(mdStartAngle, iDigits), "=>", zzGetFormat(mdEndAngle, iDigits)})
	End Function

	Public Function GetMidPoint() As Point2d Implements IUD_Link.GetMidPoint
		Dim tTest As Point2d = New Point2d(180831.488077996, 671739.0092164)
		'	zzCalcTotalAngleA()
		Dim dMidAngle As Double
		'   dMidAngle = 0.5 * (mdStartAngle + mdEndAngle)
		If Not mbIsClockWise Then
			dMidAngle = (mdStartAngle + 0.5 * mdTotalAngle) Mod (2 * Math.PI)
		Else
			dMidAngle = (mdEndAngle + 0.5 * mdTotalAngle) Mod (2 * Math.PI)
		End If
		'	DMCommon.ExcelLogAW5.SetNextValue(0, "Angles!", mdStartAngle * mdRadToDeg, mdEndAngle * mdRadToDeg, mdTotalAngle * mdRadToDeg, dMidAngle * mdRadToDeg)
		Dim tPoint As Point2d = zzPointByAngle(dMidAngle)
		'    DMAcadExt.AcadTransaction.InsertPoint(tPoint)
		If tTest.GetDistanceTo(tPoint) < 0.2 Then
			' DMAcadExt.AcadDocument.WriteMessage(dtStartPoint.ToString() & " - " & dtEndPoint.ToString())
			DMAcadExt.AcadDocument.WriteMessage("Dir=" & mbSameDirection.ToString & "; S=" & (StartAngle * mdRadToDeg).ToString() & "; E=" & (EndAngle * mdRadToDeg).ToString() & "; T=" & (TotalAngle * mdRadToDeg).ToString() & "; M=" & (dMidAngle * mdRadToDeg).ToString())

		End If
		Return tPoint
	End Function
	Public Function GetMidPoint3d() As Point3d Implements IUD_Link.GetMidPoint3d
		Return TPlnPoint.Point2dTo3d(GetMidPoint())
	End Function
	Public ReadOnly Property TotalAngle As Double Implements IUD_Link.TotalAngle
		Get
			Return mdTotalAngle
		End Get
	End Property
	Public ReadOnly Property IsArc As Boolean Implements IUD_Link.IsArc
		Get
			Return True
		End Get
	End Property
	Public ReadOnly Property IsClockWise As Boolean
		Get
			Return mbIsClockWise
		End Get
	End Property
	Public Function IsExtend(ByVal oLink As IUD_Link, tTolerance As DMAcadExt.dmTolerance, Optional bPrintDelta As Boolean = False) As Boolean Implements IUD_Link.IsExtend
		If oLink.IsArc Then
			Dim oArc As TplnArc = DirectCast(oLink, TplnArc)
			Dim tAcadTolerance As New Tolerance(tTolerance.DistanceTolerance, tTolerance.DistanceTolerance)
			Dim dCenterDistanceDeviation As Double = Me.Center.GetDistanceTo(oArc.Center)

			Dim bRes As Boolean = Me.Center.IsEqualTo(oArc.Center, tAcadTolerance)
			''''''''''''''''''''''''Return False
			If False AndAlso bRes Then '
				Dim oRhombusMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Rhombus)
				oRhombusMarkBlock.MarkPoint(zzGetSharedPoint(oLink), 1S)
			End If
			'DMCommon.ExcelLogAW5.SetNextValue(0, "ArcIsExtend?", bRes, Me.Center, oArc.Center)
			'''''''''''''''Return False
			Return bRes
		Else
			Return False
		End If

	End Function

	Public ReadOnly Property Length As Double Implements IUD_Link.Length
		Get
			Return Me.GetAcadArc().Length
		End Get
	End Property



	Public ReadOnly Property Radius As Double Implements IUD_Link.Radius
		Get
			Return Me.mdRadius
		End Get

	End Property

	Public Sub Reverse() Implements IUD_Link.Reverse
		Dim dtTempPoint As Point2d = MyBase.dtStartPoint
		Dim mdTempAngle As Double = Me.mdStartAngle
		MyBase.dtStartPoint = MyBase.dtEndPoint
		MyBase.dtEndPoint = dtTempPoint
		' 	Me.mdStartAngle = Me.mdEndAngle
		'	Me.mdEndAngle = mdTempAngle
		mbIsClockWise = Not mbIsClockWise
		mdBulge = -mdBulge
	End Sub
	Public Shared Function ToDegree(dRadAngle As Double) As Double
		Return dRadAngle * mdRadToDeg
	End Function




	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
	Private Shared Function zzGetFormat(ByVal tPoint As Point2d, ByVal iDigits As Integer) As String
		Return (Strings.FormatNumber(tPoint.X, iDigits, TriState.False, TriState.False, TriState.False) & "," & Strings.FormatNumber(tPoint.Y, iDigits, TriState.False, TriState.False, TriState.False))
	End Function
	Private Shared Function zzGetFormat(ByVal dAngle As Double, ByVal iDigits As Integer) As String
		Return (Strings.FormatNumber(dAngle * mdRadToDeg, iDigits, TriState.False, TriState.False, TriState.False))
	End Function
	Private Function zzAngleByPoint(ByVal tPoint As Point2d) As Double
		Dim dDeltaX As Double = ((tPoint.X - Me.mtCenter.X) / Me.mdRadius)
		Dim dDeltaY As Double = ((tPoint.Y - Me.mtCenter.Y) / Me.mdRadius)
		'  AcadDocument.WriteMessage("DeltaX=" & dDeltaX.ToString() & "; dDeltaY=" & dDeltaY.ToString())
		Dim dCos_QI As Double = Math.Acos(Math.Abs(dDeltaX))
		If ((dDeltaX >= 0) AndAlso (dDeltaY >= 0)) Then
			Return dCos_QI
		End If
		If ((dDeltaX >= 0) AndAlso (dDeltaY < 0)) Then
			Return (Math.PI + Math.PI - dCos_QI)
		End If
		If ((dDeltaX < 0) AndAlso (dDeltaY >= 0)) Then
			Return (Math.PI - dCos_QI)
		End If
		Return (Math.PI + dCos_QI)
	End Function
	Private Function zzGetSharedPoint(ByVal oLink As IUD_Link) As Point2d

		If dtEndPoint.IsEqualTo(oLink.StartPoint) Then
			'  dtStartPoint = dtStartPoint
			Return dtEndPoint
		ElseIf dtEndPoint.IsEqualTo(oLink.EndPoint) Then
			'System.Windows.Forms.MessageBox.Show("!!!????", "07_151h")
			Return dtEndPoint
		ElseIf dtStartPoint.IsEqualTo(oLink.EndPoint) Then
			Return dtStartPoint


		ElseIf dtStartPoint.IsEqualTo(oLink.StartPoint) Then
			'System.Windows.Forms.MessageBox.Show("!!!????", "07_153h")
			Return dtStartPoint
		Else
			Return Point2d.Origin
			System.Windows.Forms.MessageBox.Show("!!!????", "07_144h")
		End If


	End Function

	Private Function zzAngleByPoint(ByVal tPoint As Point3d) As Double
		Dim dKX As Double = ((tPoint.X - Me.mtCenter.X) / Me.mdRadius)
		Dim dKY As Double = ((tPoint.Y - Me.mtCenter.Y) / Me.mdRadius)
		Dim num As Double = Math.Acos(Math.Abs(dKX))
		If ((dKX >= 0) AndAlso (dKY >= 0)) Then
			Return num
		End If
		If ((dKX >= 0) AndAlso (dKY < 0)) Then
			Return (2 * Math.PI - num)
		End If
		If ((dKX < 0) AndAlso (dKY >= 0)) Then
			Return (Math.PI - num)
		End If
		Return (Math.PI + num)
	End Function

	Public Function GetNewEntity() As Entity Implements IUD_Link.GetNewEntity
		Return Me.GetAcadArc()
	End Function

	Public Sub SetPoint(tPoint As Point2d, bStart As Boolean) Implements IUD_Link.SetPoint
		If bStart Then
			SetValue(tPoint, EndPoint, Radius)
		Else
			SetValue(StartPoint, tPoint, Radius)
		End If

	End Sub
End Class
