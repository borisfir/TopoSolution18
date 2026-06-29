Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Class TplnPointCells
	Inherits Generic.SortedDictionary(Of ULong, TplnPointCell)

	Private Const ShiftX As ULong = 10000000000UL
	Public Const RoundDigit As Integer = 4
	Public Const RoundShift As Double = 10000.0
	Private Shared mdOriginX As Double
	Private Shared mdOriginY As Double

	Private Shared mdMinX As Double
	Private Shared mdMinY As Double

	Private Shared mdMaxX As Double
	Private Shared mdMaxY As Double
	Dim miTestCurves, miTestArc, miLineCounter, miPolylineCounter, miBlockRefCounter, miTestPoints As Integer
	Dim miLastPointNumber As Integer = 0



	Public Sub AddLine(ByVal oLine As Line, ByVal iPriority As Integer)
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		Dim oSegment As DMAcadExt.TplnLine = New DMAcadExt.TplnLine(oLine)
		Dim tAcObjID As ObjectId = oLine.ObjectId
		Me.zzAddEntityPoint(tStartPoint, tAcObjID, 0, False, iPriority)
		Me.zzAddEntityPoint(tEndPoint, tAcObjID, 1, False, iPriority)
		miLineCounter += 1
	End Sub
	Public Sub AddPolyline(ByVal oPolyline As Polyline, ByVal iArcPriority As Integer, ByVal iLinePriority As Integer)
		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices
		Dim bInner As Boolean
		Dim tCurrentPoint As Point2d

		For iIndex As Integer = 0 To iVerticesUB
			If Not oPolyline.Closed AndAlso (iIndex = 0 OrElse iIndex = iVerticesUB) Then
				bInner = False
			Else
				bInner = True
			End If
			tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
			Me.zzAddEntityPoint(tCurrentPoint, oPolyline.ObjectId, iIndex, bInner, iArcPriority)
		Next

		miPolylineCounter += 1
	End Sub
	Public Sub AddGeoPoint(ByVal oPoint As Point2d)
		Me.zzAddEntityPoint(oPoint, ObjectId.Null, 0, False, 0)
		miPolylineCounter += 1
	End Sub
	Public Sub AddBlockRef(ByVal oBlockRef As BlockReference, ByVal iPriority As Integer)
		'Dim tCurvePoint As CurvePoint

		Try

			Me.zzAddEntityPoint(TPlnPoint.Point3dTo2d(oBlockRef.Position), oBlockRef.ObjectId, -1, False, iPriority)
		Catch oEx As Exception
			DMCommon.Debug.ExcelLog.SetNextValue(0, "AddBlockRef", oEx.Message)
			Return
		End Try
		miBlockRefCounter += 1
	End Sub
	Public Sub DumpToExcel()

		For Each oPointCell As TplnPointCell In MyBase.Values
			oPointCell.DumpToExcel()
		Next
	End Sub
	Public Sub CalculateCluster()
		Dim bTest As Boolean = False
		Dim oNeighborCell As TplnPointCell = Nothing
		Dim bNeighborCellExists As Boolean
		Dim oCurrentRoot As TplnPointCell
		Dim taNeigbourKeys() As TplnPointKeyULong

		For Each oPointCell As TplnPointCell In MyBase.Values
			'   AcadDocument.WriteMessage("CCC:" & oEntityCell.MyKeyPoint.AbsCoordinates & "----" & tNeighborKeyPoint.AbsCoordinates)

			bNeighborCellExists = False
			taNeigbourKeys = oPointCell.GetNextNeigbours()
			oCurrentRoot = oPointCell.RootCell
			If taNeigbourKeys IsNot Nothing Then
				For iIndex As Integer = 0 To taNeigbourKeys.GetUpperBound(0)
					'DMCommon.ExcelLogAW5.SetNextValue(0, "Neigbour", oEntityCell.Key.Point.Coordinates2d, taNeigbourKeys(iIndex).Point.Coordinates2d, taNeigbourKeys(iIndex).Code)
					If MyBase.TryGetValue(taNeigbourKeys(iIndex).Code, oNeighborCell) Then

						'DMCommon.ExcelLogAW5.SetNextValue(0, "!!!Neigb", oPointCell.Key.Point.Coordinates2d, taNeigbourKeys(iIndex).Point.Coordinates2d, oNeighborCell.Key.Point.Coordinates2d)
						bNeighborCellExists = True
						oPointCell.AddNeighborCell(oNeighborCell)
					End If
				Next
			End If
		Next

		For Each oEntityCell As TplnPointCell In MyBase.Values
			'If oEntityCell.HasNeighboursNew Then
			'  
			'oEntityCell.CalculateN()
			'End If
		Next
	End Sub
	Public Sub CleanupClusters(bFix As Boolean, ByRef oResMarkPoints As TplnPointArray, ByRef oResFixPoints As TplnPointArray)
		'	Dim oResMarkPoints As TplnPointArray = New TplnPointArray()
		'	Dim oResFixPoints As TplnPointArray


		For Each oPointCell As TplnPointCell In MyBase.Values
			oPointCell.CleanupCluster(bFix, oResMarkPoints, oResFixPoints)
			'	DMCommon.ExcelLogAW5.SetNextValue(0, "AfterCluster", "***", oaPoints.Count)
		Next
	End Sub
	Public Function AnalysisPoint(tPoint As Point2d) As Integer
		Dim tKey As TplnPointKeyULong
		tKey = New TplnPointKeyULong(tPoint)
		Dim lCode As ULong = tKey.Code
		Dim oPointCell As TplnPointCell = Nothing
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!Analisis2", tPoint, lCode)
		Dim colEntityPoints As System.Collections.ObjectModel.Collection(Of EntityPoint) = Nothing
		If MyBase.TryGetValue(lCode, oPointCell) Then
			oPointCell = MyBase.Item(lCode)
			colEntityPoints = oPointCell.EntityPoints
			Return oPointCell.PointCount
		Else
			Return -9
		End If
	End Function
	Public Sub PrintSummary()
		Dim iEntityPoints As Integer, iClusters As Integer
		'	AcadDocument.WriteMessage("Points:" & CStr(miTestPoints))
		'	AcadDocument.WriteMessage("Arcs:" & CStr(miTestArc))
		AcadDocument.WriteMessage("Lines:" & CStr(miLineCounter))
		AcadDocument.WriteMessage("Polylines:" & CStr(miPolylineCounter))
		AcadDocument.WriteMessage("BlockRefs:" & CStr(miBlockRefCounter))
		zzGetPointsCount(iEntityPoints, iClusters)
		AcadDocument.WriteMessage("Last Number:" & CStr(EntityPoint.LastPointNumber))
		'	AcadDocument.WriteMessage("2dPolylines:" & CStr(miTestPolyline2d))

		DMAcadExt.AcadDocument.WriteMessage("EntityPoints:" & CStr(miLastPointNumber))
		DMAcadExt.AcadDocument.WriteMessage("Points:" & CStr(iEntityPoints))


		DMAcadExt.AcadDocument.WriteMessage("Cells:" & CStr(Me.Count))
		DMAcadExt.AcadDocument.WriteMessage("Clusters:" & CStr(iClusters))

	End Sub
	Private Sub zzGetPointsCount(ByRef iEntityPoints As Integer, ByRef iClusters As Integer)
		iEntityPoints = 0
		iClusters = 0
		For Each oPointCell As TplnPointCell In MyBase.Values
			iEntityPoints += oPointCell.PointCount
			If oPointCell.IsClasterRoot Then
				iClusters += 1
			End If
		Next

	End Sub

	Private Sub zzAddEntityPoint(ByVal tPoint2d As Point2d, ByVal tAcObjID As ObjectId, ByVal iVertexIndex As Integer, ByVal bInner As Boolean, ByVal iPriority As Integer)
		Dim tEntityPoint As EntityPoint
		Dim oPointCell As TplnPointCell = Nothing
		'Dim k As ULong = 3828330000332400111

		Try
			tEntityPoint = New EntityPoint(tPoint2d, tAcObjID, iVertexIndex, bInner, iPriority)
		Catch oEx As Exception
			DMCommon.Debug.ExcelLog.SetNextValue(0, "AddEntityErr", MyBase.Count, oEx.Message)
			Return
		End Try

		Dim lCode As ULong = tEntityPoint.Key.Code
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddPoint", tPoint2d, lCode)
		If MyBase.TryGetValue(lCode, oPointCell) Then
			oPointCell = MyBase.Item(lCode)
			'  DMAcadExt.AcadDocument.WriteMessage("^^^^^^^ Pts:" & oEntityCell.HasCurvePoints & ": Lines:" & oEntityCell.HasIntersectingLines & "; " & tPoint2d.ToString())
			'    oEntityCell.AddCurvePoint(tCurvePoint, oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2)
			oPointCell.AddEntityPoint(tEntityPoint)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!After AddEntityPoint")
		Else
			'	DMAcadExt.AcadDocument.WriteMessage("-00:" & tKeyPoint.AbsCoordinates)
			oPointCell = New TplnPointCell(tEntityPoint)
			'DMAcadExt.AcadDocument.WriteMessage("!!Point:" & tKeyPoint.AbsCoordinates() & "@@@ " & tKeyPoint.Coordinates())
			MyBase.Add(lCode, oPointCell)
			'    DMAcadExt.AcadDocument.WriteMessage("zzAddEntity:" & CStr(tKeyPoint.Code) & "; Cnt=" & MyBase.Count)

		End If
		miLastPointNumber += 1
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!PointCount", oPointCell.Key.Point.Coordinates, oPointCell.PointCount)
	End Sub
	Private Function zzCoordToKey(dX As Double, dY As Double) As ULong
		Dim bErrExt As Boolean = False
		If dX >= mdMinX AndAlso dX <= mdMaxX Then
			dX = Math.Round(RoundShift * (dX - mdOriginX), 0, MidpointRounding.AwayFromZero)
		Else
			bErrExt = True
		End If
		If dY >= mdMinY AndAlso dY <= mdMaxY Then
			dY = Math.Round(RoundShift * (dY - mdOriginY), 0, MidpointRounding.AwayFromZero)
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
End Class
