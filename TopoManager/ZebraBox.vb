Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD
Public Class ZebraBox
	Private Const mdAngleTolerance As Double = 0.05
	Private mtAngle As DMAcadExt.LineAngle
	Private mbQuadrantI As Boolean
	Private mdHeight As Double
	Private miPeriod As Integer
	Private mdWidth() As Double
	Private moBoundingBoxBase As DMAcadExt.TPlnBoundingBox
	Private moLeftTopLine As Line2d
	Private moRightTopLine As Line2d
	Private moLeftBottomLine As Line2d
	Private moRightBottomLine As Line2d

	Private moDirectLine1 As Line2d
	Private moDirectLine2 As Line2d

	Private moTopPoint As Point2d
	Private moRightPoint As Point2d
	Private moBottomPoint As Point2d
	Private moLeftPoint As Point2d
	Private moaLeftBottomPoints() As Point3d

	Private mtaTopRightPoints() As Point3d


	Private mtaZebraStrips() As ZebraStrip
	'	Private miaPgonTopoID() As Integer

	Private mcolLines As ObjectIdCollection
	Private miStripUB As Integer
	Private mcolStrips As Dictionary(Of Integer, ZebraStrip)
	Private miColorNum As Integer
	Public Sub New(ByVal oBoundingBoxBase As DMAcadExt.TPlnBoundingBox, ByVal tZebra As DMAcadExt.ColorZebra, ByVal dTolerance As Double)
		Try
			oBoundingBoxBase.Scale(1 + dTolerance)
			mtAngle = tZebra.Angle
			mbQuadrantI = (mtAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI)
			miColorNum = tZebra.StripUB + 1
			moBoundingBoxBase = oBoundingBoxBase

			Dim tDirLB2RT As Vector2d

			If mtAngle.XDirection Then
				tDirLB2RT = New Vector2d(Math.Cos(mtAngle.BaseAngle(True)) / Math.Sin(mtAngle.BaseAngle(True)), 1.0)
			Else
				tDirLB2RT = New Vector2d(1.0, Math.Sin(mtAngle.BaseAngle(True)) / Math.Cos(mtAngle.BaseAngle(True)))
			End If
			tDirLB2RT = mtAngle.GetDirection()

			Dim tDirLT2RB As Vector2d = tDirLB2RT.GetPerpendicularVector()

			Dim dParam1a, dParam1b, dParam2a, dParam2b As Double
			Dim dParam1Start, dParam1End, dParam2Start, dParam2End As Double
			Dim dParamDiff As Double
			DMAcadExt.AcadDocument.WriteDebugMessage("Angle=" & mtAngle.AngleDegree)
			DMAcadExt.AcadDocument.WriteDebugMessage("Toler Box=" & moBoundingBoxBase.Coordinates)

			If True Then
				DMAcadExt.AcadDocument.WriteDebugMessage("LeftTopPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(True, False)))
				moLeftTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, False), tDirLB2RT)
				DMAcadExt.AcadDocument.WriteDebugMessage("RightTopPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(False, False)))
				moRightTopLine = New Line2d(moBoundingBoxBase.AcGePoint(False, False), tDirLT2RB)
				DMAcadExt.AcadDocument.WriteDebugMessage("RightBottomPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(False, True)))
				moRightBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, True), tDirLB2RT)
				DMAcadExt.AcadDocument.WriteDebugMessage("LeftBottomPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(True, True)))
				moLeftBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(True, True), tDirLT2RB)
			Else
				moLeftTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, True), moBoundingBoxBase.AcGePoint(True, False))
				moRightTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, False), moBoundingBoxBase.AcGePoint(False, False))
				moRightBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, False), moBoundingBoxBase.AcGePoint(False, True))
				moLeftBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, True), moBoundingBoxBase.AcGePoint(True, True))

			End If

			moTopPoint = zzIntersect(moLeftTopLine, moRightTopLine)
			moLeftPoint = zzIntersect(moLeftTopLine, moLeftBottomLine)
			moRightPoint = zzIntersect(moRightTopLine, moRightBottomLine)
			moBottomPoint = zzIntersect(moLeftBottomLine, moRightBottomLine)

			Dim dLen As Double
			Dim dParamScale As Double
			If mbQuadrantI Then
				moDirectLine1 = New Line2d(moLeftPoint, moBottomPoint)
				moDirectLine2 = New Line2d(moTopPoint, moRightPoint)
				dParam1a = moDirectLine1.GetParameterOf(moLeftPoint)
				dParam1b = moDirectLine1.GetParameterOf(moBottomPoint)
				dLen = moLeftPoint.GetDistanceTo(moBottomPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("+++1 " & CStr(mbQuadrantI) & " DirectLine1 a,b: " & CStr(dParam1a) & "," & CStr(dParam1b))
				dParam2a = moDirectLine2.GetParameterOf(moTopPoint)
				dParam2b = moDirectLine2.GetParameterOf(moRightPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("+++2 " & CStr(mbQuadrantI) & " DirectLine2 a,b: " & CStr(dParam2a) & "," & CStr(dParam2b))

			Else
				moDirectLine1 = New Line2d(moLeftPoint, moTopPoint)
				moDirectLine2 = New Line2d(moBottomPoint, moRightPoint)
				dParam1a = moDirectLine1.GetParameterOf(moLeftPoint)
				dParam1b = moDirectLine1.GetParameterOf(moTopPoint)
				dLen = moLeftPoint.GetDistanceTo(moTopPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("+++-1" & CStr(mbQuadrantI) & " DirectLine1 a,b: " & CStr(dParam1a) & "," & CStr(dParam1b))
				dParam2a = moDirectLine2.GetParameterOf(moBottomPoint)
				dParam2b = moDirectLine2.GetParameterOf(moRightPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("+++-2" & CStr(mbQuadrantI) & " DirectLine2 a,b: " & CStr(dParam2a) & "," & CStr(dParam2b))

			End If

			dParam1Start = dParam1a
			dParam1End = dParam1b
			dParam2Start = dParam2a
			dParam2End = dParam2b

			dParamDiff = dParam1End - dParam1Start
			dParamScale = dParamDiff / dLen
			'	DMAcadExt.AcadDocument.WriteDebugMessage("dParam1a,dParam2a: " & CStr(dParam1a) & "," & CStr(dParam2a))
			'	DMAcadExt.AcadDocument.WriteDebugMessage("dParam1b,dParam2a: " & CStr(dParam1b) & "," & CStr(dParam2b))

			DMAcadExt.AcadDocument.WriteDebugMessage("Left Point: " & CStr(moLeftPoint.X) & "," & CStr(moLeftPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Top Point: " & CStr(moTopPoint.X) & "," & CStr(moTopPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Right Point: " & CStr(moRightPoint.X) & "," & CStr(moRightPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Bottom Point: " & CStr(moBottomPoint.X) & "," & CStr(moBottomPoint.Y))

			Dim iStripIndex As Integer = 0
			Dim iColorIndex As Integer = 0
			Dim iNextColorIndex As Integer = 0


			Dim dSum As Double
			Dim dMinStrip As Double = System.Double.MaxValue
			Dim dStrip As Double
			mcolLines = New ObjectIdCollection()

			Do
				dStrip = tZebra.Strip(iColorIndex).ScalingWidth
				'DMAcadExt.AcadDocument.WriteDebugMessage("dStrip: " & CStr(dStrip) & "," & CStr(tZebra.Scale))
				If dStrip < dMinStrip Then
					dMinStrip = dStrip
				End If
				dSum += dStrip
				'	DMAcadExt.AcadDocument.WriteLog("-**Label 12 " & dStrip.ToString() & "; " & iColorIndex.ToString() & "; " & miColorNum.ToString() & "; " & dSum.ToString() & "; " & dLen.ToString(), True, 0)

				If dSum >= dLen * 1.02 Then Exit Do
				iStripIndex += 1
				iColorIndex = iStripIndex Mod miColorNum
			Loop
			miStripUB = iStripIndex		'iStripIndex
			ReDim moaLeftBottomPoints(miStripUB + 1)
			ReDim mtaTopRightPoints(miStripUB + 1)
			ReDim mtaZebraStrips(miStripUB)

			dSum = 0.0 '- dMinStrip * 1.5


			For iLineIndex As Integer = 0 To miStripUB + 1
				If True Then
					moaLeftBottomPoints(iLineIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moDirectLine1.EvaluatePoint(dParam1Start + dSum))
					mtaTopRightPoints(iLineIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moDirectLine2.EvaluatePoint(dParam2Start + dSum))
				Else
					moaLeftBottomPoints(iLineIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moLeftTopLine.EvaluatePoint(dParam1Start + dSum))
					mtaTopRightPoints(iLineIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moRightBottomLine.EvaluatePoint(dParam2Start + dSum))
				End If
				iNextColorIndex = iLineIndex Mod miColorNum
				DMCommon.Debug.ExcelLog.SetNextValue(0, "LB,TR", iLineIndex, moaLeftBottomPoints(iLineIndex), mtaTopRightPoints(iLineIndex), dSum, miColorNum)
				dSum += tZebra.Strip(iNextColorIndex).ScalingWidth * dParamScale
				DMCommon.Debug.ExcelLog.SetNextValue(0, "iNextColorIndex=", iNextColorIndex, tZebra.Strip(iNextColorIndex).ScalingWidth * dParamScale, tZebra.Strip(iNextColorIndex).ScalingWidth)
				If iLineIndex > 0 Then
					iColorIndex = (iLineIndex - 1) Mod miColorNum
					mtaZebraStrips(iLineIndex - 1).CenterPoint = zzGetMiddlePoint(moaLeftBottomPoints(iLineIndex - 1), mtaTopRightPoints(iLineIndex))
					mtaZebraStrips(iLineIndex - 1).Color = tZebra.Strip(iColorIndex).Color
					mtaZebraStrips(iLineIndex - 1).Width = tZebra.Strip(iColorIndex).Width

				End If
				DMCommon.Debug.ExcelLog.SetNextValue(0, "iColorIndex=", iColorIndex, tZebra.Strip(iColorIndex).ScalingWidth * dParamScale)
			Next
			mcolStrips = New Dictionary(Of Integer, ZebraStrip)
		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & oEx.StackTrace, "ZebraBox - New")
		End Try

	End Sub
	Public Sub New_171209(ByVal oBoundingBoxBase As DMAcadExt.TPlnBoundingBox, ByVal tZebra As DMAcadExt.ColorZebra, ByVal dTolerance As Double)
		Dim sTest As String = "a"
		Try
			oBoundingBoxBase.Scale(1 + dTolerance)
			mtAngle = tZebra.Angle
			mbQuadrantI = (mtAngle.Quadrant = DMAcadExt.enQuadrants.QuadrantI)
			sTest = "b"
			miColorNum = tZebra.StripUB + 1
			sTest = "c"
			moBoundingBoxBase = oBoundingBoxBase


			Dim tDirLB2RT As Vector2d
			If mtAngle.XDirection Then
				tDirLB2RT = New Vector2d(Math.Cos(mtAngle.BaseAngle(True)) / Math.Sin(mtAngle.BaseAngle(True)), 1.0)
			Else
				tDirLB2RT = New Vector2d(1.0, Math.Sin(mtAngle.BaseAngle(True)) / Math.Cos(mtAngle.BaseAngle(True)))
			End If
			MessageBox.Show(CStr(tDirLB2RT.GetAngleTo(New Vector2d(1.0, 0.0))), "30_400")
			Dim tDirLT2RB As Vector2d = tDirLB2RT.GetPerpendicularVector()
			MessageBox.Show(CStr(tDirLT2RB.GetAngleTo(New Vector2d(1.0, 0.0))), "30_401")
			Dim dParam1a, dParam1b, dParam1c, dParam2a, dParam2b, dParam2c As Double
			Dim dParam1Start, dParam1End, dParam2Start, dParam2End As Double
			Dim dParamDiff As Double
			DMAcadExt.AcadDocument.WriteDebugMessage("Angle=" & mtAngle.AngleDegree)
			DMAcadExt.AcadDocument.WriteDebugMessage("Toler Box=" & moBoundingBoxBase.Coordinates)

			sTest = "k"
			If mtAngle.AngleRad < mdAngleTolerance Then
				'	moBoundingBoxBase.MinPoint
			End If
			If True Then
				DMAcadExt.AcadDocument.WriteDebugMessage("LeftTopPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(True, False)))
				moLeftTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, False), tDirLB2RT)

				DMAcadExt.AcadDocument.WriteDebugMessage("RightTopPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(False, False)))
				moRightTopLine = New Line2d(moBoundingBoxBase.AcGePoint(False, False), tDirLT2RB)
				DMAcadExt.AcadDocument.WriteDebugMessage("RightBottomPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(False, True)))
				moRightBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, True), tDirLB2RT)
				DMAcadExt.AcadDocument.WriteDebugMessage("LeftBottomPoint=" & DMAcadExt.TPlnPoint.DispPoint(moBoundingBoxBase.AcGePoint(True, True)))
				moLeftBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(True, True), tDirLT2RB)
			Else
				moLeftTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, True), moBoundingBoxBase.AcGePoint(True, False))
				moRightTopLine = New Line2d(moBoundingBoxBase.AcGePoint(True, False), moBoundingBoxBase.AcGePoint(False, False))
				moRightBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, False), moBoundingBoxBase.AcGePoint(False, True))
				moLeftBottomLine = New Line2d(moBoundingBoxBase.AcGePoint(False, True), moBoundingBoxBase.AcGePoint(True, True))

			End If

			moTopPoint = zzIntersect(moLeftTopLine, moRightTopLine)
			moLeftPoint = zzIntersect(moLeftTopLine, moLeftBottomLine)
			moRightPoint = zzIntersect(moRightTopLine, moRightBottomLine)
			moBottomPoint = zzIntersect(moLeftBottomLine, moRightBottomLine)
			If mbQuadrantI Then
				moDirectLine1 = New Line2d(moLeftPoint, moBottomPoint)
				moDirectLine2 = New Line2d(moTopPoint, moRightPoint)
				dParam1a = moDirectLine1.GetParameterOf(moLeftPoint)
				dParam1b = moDirectLine1.GetParameterOf(moBottomPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("++++" & CStr(mbQuadrantI) & " DirectLine1 a,b: " & CStr(dParam1a) & "," & CStr(dParam1b))

			Else
				moDirectLine1 = New Line2d(moLeftPoint, moTopPoint)
				moDirectLine2 = New Line2d(moBottomPoint, moRightPoint)
				dParam1a = moDirectLine1.GetParameterOf(moLeftPoint)
				dParam1b = moDirectLine1.GetParameterOf(moTopPoint)
				DMAcadExt.AcadDocument.WriteDebugMessage("+++-" & CStr(mbQuadrantI) & " DirectLine1 a,b: " & CStr(dParam1a) & "," & CStr(dParam1b))

			End If

			If mbQuadrantI Then
				dParam1a = moLeftBottomLine.GetParameterOf(moLeftPoint)
				dParam1b = moLeftBottomLine.GetParameterOf(moBottomPoint)
				dParam1c = moLeftBottomLine.GetParameterOf(moBoundingBoxBase.AcGePoint(True, True))

				dParam2a = moRightTopLine.GetParameterOf(moTopPoint)
				dParam2b = moRightTopLine.GetParameterOf(moRightPoint)
				dParam2c = moRightTopLine.GetParameterOf(moBoundingBoxBase.AcGePoint(False, False))
			Else
				dParam1a = moLeftTopLine.GetParameterOf(moLeftPoint)
				dParam1b = moLeftTopLine.GetParameterOf(moTopPoint)
				dParam1c = moLeftBottomLine.GetParameterOf(moBoundingBoxBase.AcGePoint(True, False))

				dParam2a = moRightBottomLine.GetParameterOf(moBottomPoint)
				dParam2b = moRightBottomLine.GetParameterOf(moRightPoint)
				dParam2c = moRightTopLine.GetParameterOf(moBoundingBoxBase.AcGePoint(False, True))

			End If
			DMAcadExt.AcadDocument.WriteDebugMessage("!!!" & CStr(mbQuadrantI) & " LeftBottom 1a,2a: " & CStr(dParam1a) & "," & CStr(dParam1c) & "," & CStr(dParam1b))
			DMAcadExt.AcadDocument.WriteDebugMessage("!!!RightTop dParam1b,dParam2a: " & CStr(dParam2a) & "," & CStr(dParam2c) & "," & CStr(dParam2b))


			dParam1Start = Math.Min(dParam1a, dParam1b)
			dParam1End = Math.Max(dParam1a, dParam1b)
			dParam2Start = Math.Min(dParam2a, dParam2b)
			dParam2End = Math.Max(dParam2a, dParam2b)

			dParam1Start = dParam1b
			dParam1End = dParam1a
			dParam2Start = dParam2b
			dParam2End = dParam2a

			dParamDiff = dParam1End - dParam1Start
			DMAcadExt.AcadDocument.WriteDebugMessage("dParam1a,dParam2a: " & CStr(dParam1a) & "," & CStr(dParam2a))
			DMAcadExt.AcadDocument.WriteDebugMessage("dParam1b,dParam2a: " & CStr(dParam1b) & "," & CStr(dParam2b))

			DMAcadExt.AcadDocument.WriteDebugMessage("Left Point: " & CStr(moLeftPoint.X) & "," & CStr(moLeftPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Top Point: " & CStr(moTopPoint.X) & "," & CStr(moTopPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Right Point: " & CStr(moRightPoint.X) & "," & CStr(moRightPoint.Y))
			DMAcadExt.AcadDocument.WriteDebugMessage("Bottom Point: " & CStr(moBottomPoint.X) & "," & CStr(moBottomPoint.Y))

			Dim iStripIndex As Integer = 0
			Dim iColorIndex As Integer = 0

			Dim dSum As Double
			Dim dMinStrip As Double = System.Double.MaxValue
			Dim dStrip As Double
			mcolLines = New ObjectIdCollection()
			'	zzDrawLine(moBoundingBoxBase.AcGePoint(True, False), moBoundingBoxBase.AcGePoint(False, False))
			'	zzDrawLine(moBoundingBoxBase.AcGePoint(False, False), moBoundingBoxBase.AcGePoint(False, True))
			'	zzDrawLine(moBoundingBoxBase.AcGePoint(False, True), moBoundingBoxBase.AcGePoint(True, True))
			'	zzDrawLine(moBoundingBoxBase.AcGePoint(True, True), moBoundingBoxBase.AcGePoint(True, False))

			Do
				dStrip = tZebra.Strip(iColorIndex).ScalingWidth
				If dStrip < dMinStrip Then
					dMinStrip = dStrip
				End If
				dSum += dStrip
				If dSum >= dParamDiff * 1.02 Then Exit Do
				iStripIndex += 1
				iColorIndex = iStripIndex Mod miColorNum
			Loop
			miStripUB = iStripIndex		'iStripIndex
			ReDim moaLeftBottomPoints(miStripUB + 1)
			ReDim mtaTopRightPoints(miStripUB + 1)
			ReDim mtaZebraStrips(miStripUB)



			'	moaLeftBottomPoints(0) = GeoUtilites.Point2dTo3d(moLeftPoint)
			'	moaTopRightPoints(0) = GeoUtilites.Point2dTo3d(moTopPoint)
			dSum = 0.0 '- dMinStrip * 1.5
			DMAcadExt.AcadDocument.WriteDebugMessage("dSum=" & CStr(dSum))
			'Dim tCenterPoint As Point3d
			For iIndex As Integer = 0 To miStripUB + 1
				If mbQuadrantI Then
					moaLeftBottomPoints(iIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moLeftBottomLine.EvaluatePoint(dParam1Start + dSum))
					mtaTopRightPoints(iIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moRightTopLine.EvaluatePoint(dParam2Start + dSum))

				Else
					moaLeftBottomPoints(iIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moLeftTopLine.EvaluatePoint(dParam1Start + dSum))
					mtaTopRightPoints(iIndex) = DMAcadExt.TPlnPoint.Point2dTo3d(moRightBottomLine.EvaluatePoint(dParam2Start + dSum))
				End If
				dSum += tZebra.Strip(iColorIndex).ScalingWidth
				If iIndex > 0 Then
					iColorIndex = (iIndex - 1) Mod miColorNum
					mtaZebraStrips(iIndex - 1).CenterPoint = zzGetMiddlePoint(moaLeftBottomPoints(iIndex - 1), mtaTopRightPoints(iIndex))
					mtaZebraStrips(iIndex - 1).Color = tZebra.Strip(iColorIndex).Color
				End If
			Next
			mcolStrips = New Dictionary(Of Integer, ZebraStrip)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "Zebrabox - New")
		End Try
		'	moaLeftBottomPoints(iStripIndex + 2) = GeoUtilites.Point2dTo3d(moBottomPoint)
		'	moaTopRightPoints(iStripIndex + 2) = GeoUtilites.Point2dTo3d(moRightPoint)
		'	zzDrawLine(moLeftPoint, moTopPoint)
		'	zzDrawLine(moTopPoint, moRightPoint)
		'	zzDrawLine(moRightPoint, moBottomPoint)
		'	zzDrawLine(moBottomPoint, moLeftPoint)


	End Sub
	Public Property PgonTopoID(ByVal iIndex As Integer) As Integer
		Get
			Return mtaZebraStrips(iIndex).PgonTopoID
		End Get
		Set(ByVal iValue As Integer)
			mtaZebraStrips(iIndex).PgonTopoID = iValue
			mcolStrips.Add(iValue, mtaZebraStrips(iIndex))

			Dim tStrip As ZebraStrip = mtaZebraStrips(iIndex)

			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "3_Strip", tStrip.Width, tStrip.Color.AcadColor, iValue)


		End Set
	End Property
	Public Function GetColor(ByVal iPgonTopoID As Integer) As DMAcadExt.DMColor
		If mcolStrips.ContainsKey(iPgonTopoID) Then
			Return mcolStrips.Item(iPgonTopoID).Color
		Else
			DMAcadExt.AcadDocument.WriteMessage("TopoID Not Found: " & CStr(iPgonTopoID))
			Return Nothing
		End If
	End Function
	Public ReadOnly Property PolygonUB() As Integer
		Get
			Return miStripUB
		End Get
	End Property
	Public ReadOnly Property CenterPoint(ByVal iIndex As Integer) As Point3d
		Get
			Return mtaZebraStrips(iIndex).CenterPoint
		End Get
	End Property

	Private Sub zzDrawLine(ByVal oFirstPoint As Point3d, ByVal oLastPoint As Point3d)
		Dim oLine As Line
		oLine = New Line(oFirstPoint, oLastPoint)
		If oLine IsNot Nothing Then
			mcolLines.Add(DMAcadExt.AcadTransaction.AppendEntity(oLine))
		End If
	End Sub
	Private Sub zzDrawLine(ByVal oFirstPoint As Point3d, ByVal oLastPoint As Point3d, ByVal oColor As Colors.Color)
		Dim oLine As Line
		oLine = New Line(oFirstPoint, oLastPoint)
		If oLine IsNot Nothing Then
			oLine.Color = oColor
			mcolLines.Add(DMAcadExt.AcadTransaction.AppendEntity(oLine))
		End If
	End Sub
	Public Sub zzDrawLine(ByVal oFirstPoint As Point2d, ByVal oLastPoint As Point2d)
		Dim oLine As Line

		oLine = New Line(DMAcadExt.TPlnPoint.Point2dTo3d(oFirstPoint), DMAcadExt.TPlnPoint.Point2dTo3d(oLastPoint))
		If oLine IsNot Nothing Then
			'	oLine.Color = Colors.Color.FromRgb(128, 0, 0)
			mcolLines.Add(DMAcadExt.AcadTransaction.AppendEntity(oLine))
		End If
	End Sub
	Public Sub DrawLines()
		Try
			zzDrawLine(moaLeftBottomPoints(0), mtaTopRightPoints(0), Colors.Color.FromRgb(0, 128, 0))
		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessageLog(oEx.Message, "ZebraBox - DrawLines_1")
		End Try

		For iIndex As Integer = 0 To miStripUB
			Try
				zzDrawLine(moaLeftBottomPoints(iIndex), moaLeftBottomPoints(iIndex + 1))
				zzDrawLine(moaLeftBottomPoints(iIndex + 1), mtaTopRightPoints(iIndex + 1))
				zzDrawLine(mtaTopRightPoints(iIndex + 1), mtaTopRightPoints(iIndex))
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessageLog(oEx.Message, "ZebraBox - DrawLines_1")
			End Try
		
		Next
	End Sub
	Public Sub DeleteLines()
		DMAcadExt.AcadTransaction.EraseDBObjects(mcolLines)
	End Sub
	Public ReadOnly Property Lines() As ObjectIdCollection
		Get
			Return mcolLines
		End Get
	End Property
	Private Sub zzDispPoint(ByVal tPoint As Point2d, ByVal sCaption As String)
		MessageBox.Show(CStr(tPoint.X) & "," & CStr(tPoint.Y), sCaption)
	End Sub
	Private Function zzIntersect(ByVal oFirstLine As Line2d, ByVal oSecondLine As Line2d) As Point2d
		Dim taIntersectPoints() As Point2d = oFirstLine.IntersectWith(oSecondLine)
		If taIntersectPoints IsNot Nothing AndAlso taIntersectPoints.GetUpperBound(0) = 0 Then
			Return taIntersectPoints(0)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzGetMiddlePoint(ByVal iPointA As Point3d, ByVal iPointB As Point3d) As Point3d
		Dim dX As Double = 0.5 * (iPointA.X + iPointB.X)
		Dim dY As Double = 0.5 * (iPointA.Y + iPointB.Y)
		Return New Point3d(dX, dY, 0.0)
	End Function
	Private Structure ZebraStrip
		Dim CenterPoint As Point3d
		Dim Index As Integer
		Dim PgonTopoID As Integer
		Dim Color As DMAcadExt.DMColor
		Dim Width As Double
	End Structure

End Class
