Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Public Class ZebStripe

	Private mdXBasePoint As Double
	Private mdYBasePoint As Double

	Private mdXEndPoint As Double
	Private mdYEndPoint As Double


	Private mdBaseWidth As Double
	Private mdBaseHeight As Double

	Private mdYStep As Double
	Private miNumber As Integer


	Private mvdaPoints() As TPlnPoint
	Public Sub Draw()

	End Sub

	Public Sub SetBasePoint(ByVal dXBasePoint As Double, ByVal dYBasePoint As Double)
		mdXBasePoint = dXBasePoint
		mdYBasePoint = dYBasePoint
	End Sub

	Public Sub SetBasePolygon(ByVal dBaseWidth As Double, ByVal dBaseHeght As Double)
		mdBaseWidth = dBaseWidth
		mdBaseHeight = dBaseHeght

		mdXEndPoint = mdXBasePoint - dBaseWidth
		mdYEndPoint = mdYBasePoint + mdBaseHeight
	End Sub

	Public Property Number() As Integer
		Get
			Return miNumber
		End Get
		Set(ByVal iValue As Integer)
			miNumber = iValue
		End Set
	End Property


 
	Public Function GetIntersect(ByVal oBasePolygon As Polyline2d) As Integer ' As AutoCAD.AcadPolyline
		Dim daVertixesList(3&) As Double
		Dim oAcadLWPolyline As Polyline2d
		Dim mdaBasePoint(2&) As Double
		Dim oOutPolygon As Polyline2d
		Dim vdaAddPoints

		Dim iIndex As Integer
		Dim colIntersectPoints As Point3dCollection

		mvdaPoints = New TPlnPoint(mdXBasePoint, mdYBasePoint)
		daVertixesList(0&) = mdXBasePoint - mdBaseWidth
		daVertixesList(2&) = mdXBasePoint
		Do
			iIndex += 1
			daVertixesList(1&) = mdYBasePoint + iIndex * mdYStep
			daVertixesList(3&) = daVertixesList(1&)

			'   daVertixesList(4&) = mdXBasePoint
			'   daVertixesList(5&) = daVertixesList(3&)
			'   daVertixesList(6&) = mdXBasePoint
			'   daVertixesList(7&) = daVertixesList(1&)

			oAcadLWPolyline = AcadTransaction.AddPolyline(daVertixesList)

			mdaBasePoint(0&) = mdXBasePoint : mdaBasePoint(1&) = mdYBasePoint : mdaBasePoint(2&) = 0.0#
			oAcadLWPolyline.Rotate(mdaBasePoint, gdPrmZebraAngle / (2 * 3.14159265358))
			vdaAddPoints = Nothing
			oAcadLWPolyline.Visible = False
			'vdaAddPoints = 
			oAcadLWPolyline.IntersectWith(oBasePolygon, Intersect.ExtendThis, colIntersectPoints, 0, 0)
			If colIntersectPoints.Count = 0 Then
				ReDim Preserve mvdaPoints(6 * iIndex - 1)
				mvdaPoints(6 * iIndex - 3) = mdXEndPoint
				mvdaPoints(6 * iIndex - 2) = mdYEndPoint
				mvdaPoints(3 * iIndex - 1) = 0.0#
				Exit Do
			End If
			zzAddPoints3(mvdaPoints, vdaAddPoints)
			If colIntersectPoints.Count = 3 Then Exit Do
		Loop
		miNumber = iIndex
		GetIntersect = iIndex
	End Function


	Public Property YStep() As Double
		Get

		End Get
		Set(ByVal dValue As Double)
			mdYStep = dValue
		End Set
	End Property

 

	Public Sub zzAddPoints3(ByVal vdaPoints, ByVal vdaAddPoints)
		Dim lAddUB As Long
		Dim lSourceUB As Long, lDestUB As Long
		Dim lIndex As Long
		If IsArray(vdaAddPoints) Then
			lAddUB = UBound(vdaAddPoints)
			lSourceUB = UBound(vdaPoints)
			lDestUB = lSourceUB + lAddUB + 1&
      ReDim Preserve vdaPoints(lDestUB) As Double
			For lIndex = 0& To lAddUB
				vdaPoints(lSourceUB + lIndex + 1&) = vdaAddPoints(lIndex)
			Next
		End If
	End Sub

	Public Function GetPolygon(ByVal iNumber As Integer) As AutoCAD.AcadPolyline
		Dim vdaPoints
		Dim iStart As Integer, iLen As Integer
		Dim lIndex As Long
		Dim lNewIndex As Long
		Dim oOutPolygon As AutoCAD.AcadPolyline
		Select Case iNumber
			Case 1&

				iStart = 0
				iLen = 8
			Case 2& To miNumber - 1
				iStart = 6 * iNumber - 9
				iLen = 11

			Case miNumber
				iStart = 6 * iNumber - 9
				iLen = 8

		End Select
   ReDim vdaPoints(lLen) As Double
		For lIndex = lStart To lStart + 5&
			vdaPoints(lNewIndex) = mvdaPoints(lIndex)
			lNewIndex = lNewIndex + 1&
		Next
		For lIndex = lStart + 9& To lStart + lLen
			vdaPoints(lNewIndex) = mvdaPoints(lIndex)
			lNewIndex = lNewIndex + 1&
		Next
		For lIndex = lStart + 6& To lStart + 8&
			vdaPoints(lNewIndex) = mvdaPoints(lIndex)
			lNewIndex = lNewIndex + 1&
		Next
		If iNumber > 1& And iNumber < miNumber Then
			If vdaPoints(3&) = mdXBasePoint And vdaPoints(7&) = mdYBasePoint + mdBaseHeight Then
         ReDim vdaPoints(lLen + 3&) As Double
				lNewIndex = 0&
				For lIndex = lStart To lStart + 5&
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next
				vdaPoints(lNewIndex) = mdXBasePoint
				lNewIndex = lNewIndex + 1&
				vdaPoints(lNewIndex) = mdYBasePoint + mdBaseHeight
				lNewIndex = lNewIndex + 1&
				vdaPoints(lNewIndex) = 0.0#
				lNewIndex = lNewIndex + 1&
				For lIndex = lStart + 9& To lStart + lLen
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next
				For lIndex = lStart + 6& To lStart + 8&
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next
			End If
			If vdaPoints(1&) = mdYBasePoint And vdaPoints(9&) = mdXBasePoint - mdBaseWidth Then
         ReDim vdaPoints(lLen + 3&) As Double
				lNewIndex = 0&
				For lIndex = lStart To lStart + 5&
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next

				For lIndex = lStart + 9& To lStart + lLen
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next
				For lIndex = lStart + 6& To lStart + 8&
					vdaPoints(lNewIndex) = mvdaPoints(lIndex)
					lNewIndex = lNewIndex + 1&
				Next

				vdaPoints(lNewIndex) = mdXBasePoint - mdBaseWidth
				lNewIndex = lNewIndex + 1&
				vdaPoints(lNewIndex) = mdYBasePoint
				lNewIndex = lNewIndex + 1&
				vdaPoints(lNewIndex) = 0.0#
				lNewIndex = lNewIndex + 1&
			End If
		End If

		oOutPolygon = ThisDrawing.ModelSpace.AddPolyline(vdaPoints)
		oOutPolygon.Closed = True
		oOutPolygon.Visible = False
		GetPolygon = oOutPolygon
	End Function

End Class
