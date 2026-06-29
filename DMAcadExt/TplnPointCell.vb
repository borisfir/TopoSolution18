Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnPointCell
	Private mlCenterX_ As ULong
	Private mlCenterY_ As ULong

	'Private mcolAllPoints As Point2dCollection = New Point2dCollection()
	'Private moaAcadEntity() As EntityPoint
	'Private mlstEntityPoints As List(Of EntityPoint)
	Private mcolEntityPoints As System.Collections.ObjectModel.Collection(Of EntityPoint)
	Private miMinPriority As Integer
	Private mtKey As TplnPointKeyULong
	Private moRootCell As TplnPointCell
	Private mhsClusterCells As HashSet(Of TplnPointCell)

	Private moBoundingBox As TPlnBoundingBox
	Private mtCellPrimaryPoint As EntityPoint
	Private mtClusterPrimaryPoint As EntityPoint

	Private Shared moSaltireMarkBlock As DMAcadExt.MarkBlock
	Private Shared mtaKeySteps() As TplnPointKeyULong.KeyStep = {New TplnPointKeyULong.KeyStep(0L, 1L) _
																		 , New TplnPointKeyULong.KeyStep(1L, -1L) _
																		 , New TplnPointKeyULong.KeyStep(1L, 0L) _
																		 , New TplnPointKeyULong.KeyStep(1L, 1L)}
	Private Shared mdCellSize As Double
	Public Shared Sub Init(dCellSize As Double)
		mdCellSize = dCellSize
		moSaltireMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)
	End Sub
	Public Sub New(ByVal tEntityPoint As EntityPoint)
		mtKey = tEntityPoint.Key
		'mlstEntityPoints = New List(Of EntityPoint)
		mhsClusterCells = New HashSet(Of TplnPointCell)()
		mcolEntityPoints = New ObjectModel.Collection(Of EntityPoint)
		'	mtMinKeyPoint = New KeyPoint(tCurvePoint.AcadPoint)
		'	mcolEntityPoints.Add(tEntityPoint)

		moBoundingBox = New TPlnBoundingBox(tEntityPoint.Point)
		mtCellPrimaryPoint = tEntityPoint
		'	mdicIntersectingLines = New ObjectIdCollection()

	End Sub
	Public Shared ReadOnly Property CellSize As Double
		Get
			Return mdCellSize
		End Get

	End Property
	Public Sub AddEntityPoint(ByVal tEntityPoint As EntityPoint)
		Dim tAcadPoint As Point2d = tEntityPoint.AcadPoint
		Dim bPointExists As Boolean = False

		If miMinPriority > tEntityPoint.Priority Then
			miMinPriority = tEntityPoint.Priority
		End If

		'mlstEntityPoints.Add(tEntityPoint)

		' AcadDocument.WriteMessage("PointCell: " & CStr(Me.mtMinKeyPoint.Code) & ":" & Me.mtMinKeyPoint.AbsCoordinates & "||" & moaAcadEntity.GetUpperBound(0))

		'If moBoundingBox Is Nothing Then
		'moBoundingBox = New TPlnBoundingBox(mtMainPoint.Point, tEntityPoint.Point)
		'Else
		moBoundingBox.Union(tEntityPoint.Point)
		'End If

		'mcolEntityPoints.Add(tEntityPoint)

		If mtCellPrimaryPoint.Priority > tEntityPoint.Priority Then
			mcolEntityPoints.Add(mtCellPrimaryPoint)
			mtCellPrimaryPoint = tEntityPoint

		Else
			mcolEntityPoints.Add(tEntityPoint)
		End If
	End Sub

	Public Sub Cleanup(bFix As Boolean, tClusterPrimaryPoint As EntityPoint, ByRef oResMarkPoints As TplnPointArray, ByRef oResFixPoints As TplnPointArray)
		Dim tAcadPoint As Point2d = tClusterPrimaryPoint.AcadPoint
		'If mtCellPrimaryPoint.AcadPoint <> tAcadPoint Then
		If Not mtCellPrimaryPoint.IsEqualTo(tAcadPoint) Then
			If bFix Then
				mtCellPrimaryPoint.UpdatePoint(tAcadPoint, oResFixPoints)
			Else
				mtCellPrimaryPoint.MarkPoint(tAcadPoint, oResMarkPoints)
			End If

		End If

		For Each tEntityPoint As EntityPoint In mcolEntityPoints
			'	If tEntityPoint.AcadPoint <> tAcadPoint Then
			If Not tEntityPoint.IsEqualTo(tAcadPoint) Then

				If bFix Then
					tEntityPoint.UpdatePoint(tAcadPoint, oResFixPoints)
				Else
					tEntityPoint.MarkPoint(tAcadPoint, oResMarkPoints)
				End If
			End If



			'	DMCommon.ExcelLogAW5.SetNextValue(0, "AfterEPoint", mcolEntityPoints.Count, mtMainPoint.Priority, "<><><>", oaPoints.Count)
		Next
	End Sub


	Public ReadOnly Property EntityPoints As System.Collections.ObjectModel.Collection(Of EntityPoint)
		Get
			Return mcolEntityPoints
		End Get
	End Property

	Public ReadOnly Property PointCount As Integer
		Get
			If mcolEntityPoints Is Nothing Then
				Return 0
			Else
				Return mcolEntityPoints.Count + 1
			End If
		End Get
	End Property
	Public Property RootCell() As TplnPointCell
		Get
			Return moRootCell
		End Get
		Set(ByVal oValue As TplnPointCell)
			moRootCell = oValue

		End Set
	End Property
	Public ReadOnly Property Key As TplnPointKeyULong
		Get
			Return mtKey
		End Get
	End Property
	Public ReadOnly Property MyRoot As TplnPointCell
		Get
			If moRootCell Is Nothing Then
				Return Me
			Else
				Return moRootCell
			End If
		End Get
	End Property
	Public ReadOnly Property RootKey As TplnPointKeyULong
		Get
			If moRootCell Is Nothing Then
				Return mtKey
			Else
				Return moRootCell.mtKey
			End If
		End Get
	End Property
	Public ReadOnly Property BoundingBox As TPlnBoundingBox
		Get
			Return moBoundingBox
		End Get
	End Property
	Public ReadOnly Property PrimaryPoint As EntityPoint
		Get
			Return mtCellPrimaryPoint
		End Get
	End Property
	Public ReadOnly Property IsClasterRoot As Boolean
		Get
			Return moRootCell Is Nothing

		End Get
	End Property

	Public Function GetNextNeigbours() As TplnPointKeyULong()
		Dim taResKeys(mtaKeySteps.GetUpperBound(0)) As TplnPointKeyULong
		For iIndex As Integer = 0 To mtaKeySteps.GetUpperBound(0)
			taResKeys(iIndex) = mtKey.AddStep(mtaKeySteps(iIndex))
		Next
		Return taResKeys
	End Function


	Public Sub AddNeighborCell(oCell As TplnPointCell)
		Dim oRootCell As TplnPointCell = oCell.RootCell
		If oRootCell IsNot Nothing Then
			'DMCommon.ExcelLogAW5.SetNextValue(0, "!!!????")
			If oRootCell.Key.Code < Me.RootKey.Code Then
				oRootCell.AddNeighborCell(Me)
			Else
				MyRoot.AddNeighborCell(oRootCell)
			End If
		ElseIf moRootCell Is Nothing Then
			mhsClusterCells.Add(oCell) 'UNION
			oCell.RootCell = Me
			'DMCommon.ExcelLogAW5.SetNextValue(0, "ClasterRoot", oCell.IsClasterRoot)
		Else 'If oRootCell Is Nothing And moRootCell IsNot Nothing Then
			moRootCell.AddNeighborCell(oCell)
		End If
	End Sub
	Public Sub DumpToExcel()

		DMCommon.Debug.ExcelLog.SetNextValue(0, "Dump1", String.Empty, mtKey.Code, mtKey.Point.Coordinates2d, PointCount, PrimaryPoint.Point.Coordinates2d, PrimaryPoint.PointID, PrimaryPoint.Priority)
		For Each tEntityPoint As EntityPoint In mcolEntityPoints
			tEntityPoint.AcadPoint.ToString()
			DMCommon.Debug.ExcelLog.SetNextValue(1, "Dump2", tEntityPoint.PointID, tEntityPoint.Key.Code, tEntityPoint.AcadPoint.ToString(), tEntityPoint.Key.Point.Coordinates, tEntityPoint.Priority)


		Next
		For Each oCell As TplnPointCell In mhsClusterCells
			DMCommon.Debug.ExcelLog.SetNextValue(2, "Dump3", String.Empty, oCell.Key.Code, oCell.Key.Point.Coordinates2d, oCell.PrimaryPoint.PointID, oCell.PrimaryPoint.Point.Coordinates2d)
		Next

	End Sub
	Private Sub zzInputNeigbours()
		'DMCommon.ExcelLogAW5.SetNextValue(0, "mhsClusterCells", mhsClusterCells.Count)
		mtClusterPrimaryPoint = mtCellPrimaryPoint
		For Each oCell As TplnPointCell In mhsClusterCells
			moBoundingBox.Union(oCell.BoundingBox)
			If oCell.PrimaryPoint.Priority < mtCellPrimaryPoint.Priority Then
				mtClusterPrimaryPoint = oCell.PrimaryPoint
			End If
		Next
	End Sub
	Public Sub CleanupCluster(bFix As Boolean, ByRef oResMarkPoints As TplnPointArray, ByRef oResFixPoints As TplnPointArray)
		'	DMCommon.ExcelLogAW5.SetNextValue(0, "CellInfo", mtKey.Point.Coordinates, mtMainPoint.Priority, IsClasterRoot, mhsClusterCells.Count, PointCount)
		If IsClasterRoot Then

			If mhsClusterCells.Count > 0 Then
				zzInputNeigbours()
				'DMCommon.ExcelLogAW5.SetNextValue(0, "BoundBox", moBoundingBox.Width, moBoundingBox.Height, moBoundingBox.MaxSize, mdCellSize, moBoundingBox.AcGePoint(True, True))
				If moBoundingBox IsNot Nothing AndAlso moBoundingBox.MaxSize < 2.0 * mdCellSize Then
					Cleanup(bFix, mtClusterPrimaryPoint, oResMarkPoints, oResFixPoints)
					For Each oPointCell As TplnPointCell In mhsClusterCells
						'	DMCommon.ExcelLogAW5.SetNextValue(0, "CellInfo", mtKey.Point.Coordinates2d, mtCellPrimaryPoint.Priority, mtCellPrimaryPoint.Point.Coordinates2d, oPointCell.Key.Point.Coordinates2d, IsClasterRoot, oPointCell.IsClasterRoot, mhsClusterCells.Count, PointCount)

						oPointCell.Cleanup(bFix, mtClusterPrimaryPoint, oResMarkPoints, oResFixPoints)
					Next
				End If
			ElseIf PointCount = 1 Then
				mtCellPrimaryPoint.MarkSinglePoint(oResMarkPoints)
			Else
				Cleanup(bFix, mtCellPrimaryPoint, oResMarkPoints, oResFixPoints)
			End If

		End If
	End Sub

End Class
Public Structure EntityPoint
	' Private moCurve As Curve
	Private mtEntityObjID As ObjectId
	Private miPointID As Integer

	'	Private moTopoDef As TopoDef
	Private miPointIndex As Integer
	Private mbInner As Boolean
	Private mtPoint2d As Point2d

	Private miPriority As Integer
	Private mtKey As TplnPointKeyULong
	Private Shared miLastPointNumber As Integer
	Private Shared moSaltireMarkBlock As DMAcadExt.MarkBlock  'point <> MainPoint
	Private Shared moCircleMarkBlock As DMAcadExt.MarkBlock  'Single point
	Private Shared moSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square) 'Large cluster

	Private Shared mcolZeroLenPolylines As System.Collections.ObjectModel.Collection(Of Polyline)
	'	Private mbOriginExists As Boolean

	Public Sub New(ByVal tPoint2d As Point2d, ByVal tEntityObjID As ObjectId, ByVal iPointIndex As Integer, ByVal bInner As Boolean, ByVal iPriority As Integer)
		'   moCurve = oCurve
		miLastPointNumber += 1
		miPointID = miLastPointNumber

		mtEntityObjID = tEntityObjID
		'	moTopoDef = oTopoDef
		miPointIndex = iPointIndex
		mbInner = bInner

		mtPoint2d = tPoint2d
		mtKey = New TplnPointKeyULong(tPoint2d)
		miPriority = iPriority
	End Sub
	Public Shared Operator =(tEntityPoint1 As EntityPoint, tEntityPoint2 As EntityPoint) As Boolean
		Return tEntityPoint1.EntityObjID = tEntityPoint2.EntityObjID AndAlso tEntityPoint1.PointIndex = tEntityPoint2.PointIndex
	End Operator
	Public Shared ReadOnly Property LastPointNumber As Integer
		Get
			Return miLastPointNumber
		End Get
	End Property

	Public Shared Sub Init()
		miLastPointNumber = 0
		moSaltireMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)
		moCircleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
		moSquareMarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square)


		mcolZeroLenPolylines = New ObjectModel.Collection(Of Polyline)()

	End Sub
	Public Shared Operator <>(tEntityPoint1 As EntityPoint, tEntityPoint2 As EntityPoint) As Boolean
		Return Not (tEntityPoint1 = tEntityPoint2)
	End Operator
	Public Shared Sub RemoveZeroLenSegments()
		If mcolZeroLenPolylines IsNot Nothing Then
			Dim dDist As Double
			Dim baVert() As Boolean
			Dim bHasLen As Boolean
			DMAcadExt.AcadDocument.WriteDebugMessage("!!mcolZeroLenPolylines= " & mcolZeroLenPolylines.Count.ToString())

			For Each oPolyline As Polyline In mcolZeroLenPolylines
				ReDim baVert(oPolyline.NumberOfVertices - 1)
				bHasLen = False
				For iIndex As Integer = oPolyline.NumberOfVertices - 1 To 1 Step -1
					dDist = oPolyline.GetDistanceAtParameter(iIndex)
					If dDist > 0.0 Then
						bHasLen = True
						baVert(iIndex) = True
					End If
					'   DMAcadExt.AcadDocument.WriteDebugMessage("??rem Ind= " & iIndex.ToString() & "; " & dDist.ToString() & "; " & oPolyline.GetPoint2dAt(0).ToString())
				Next
				If bHasLen Then
					For iIndex As Integer = oPolyline.NumberOfVertices - 1 To 1 Step -1

						If Not baVert(iIndex) Then

							DMAcadExt.AcadDocument.WriteDebugMessage("!!rem Ind= " & iIndex.ToString() & "; " & dDist.ToString() & "; " & oPolyline.GetPoint2dAt(0).ToString())
							oPolyline.RemoveVertexAt(iIndex - 1)
						End If

					Next
				Else
					oPolyline.Erase()
				End If
			Next
		End If
	End Sub

	Public ReadOnly Property AcadPoint() As Point2d
		Get
			Return mtPoint2d
		End Get
	End Property
	Public Function IsEqualTo(tAcadPoint As Point2d) As Boolean
		Dim tResVector As Vector2d = mtPoint2d - tAcadPoint
		'DMCommon.ExcelLogAW5.SetNextValue(2, "IsEqualTo", mtPoint2d, tAcadPoint, tResVector.X, tResVector.Y, tResVector.IsZeroLength(), mtPoint2d.X - tAcadPoint.X, mtPoint2d.Y - tAcadPoint.Y)
		'	Return tResVector.IsZeroLength()
		Return (tResVector.X = 0.0) AndAlso (tResVector.Y = 0.0)


	End Function

	Public ReadOnly Property AcadPoint3d() As Point3d
		Get
			Return TPlnPoint.Point2dTo3d(mtPoint2d)
		End Get
	End Property
	Public ReadOnly Property Point() As TPlnPoint
		Get
			Return New TPlnPoint(mtPoint2d)
		End Get
	End Property


	Public ReadOnly Property AcadPointCoordinates() As String
		Get
			Return TPlnPoint.DispPoint(mtPoint2d)
		End Get
	End Property

	Public ReadOnly Property Inner() As Boolean
		Get
			Return mbInner
		End Get
	End Property
	Public ReadOnly Property EntityObjID() As ObjectId
		Get
			Return mtEntityObjID
		End Get
	End Property
	Public ReadOnly Property PointIndex As Integer
		Get
			Return miPointIndex
		End Get
	End Property
	Public ReadOnly Property Key As TplnPointKeyULong
		Get
			Return mtKey
		End Get
	End Property
	Public ReadOnly Property Priority As Integer
		Get
			Return miPriority
		End Get
	End Property
	Public ReadOnly Property PointID As Integer
		Get
			Return miPointID
		End Get
	End Property

	Public Function GetDistanceTo(ByVal tPoint2d As Point2d) As Double
		Return tPoint2d.GetDistanceTo(tPoint2d)
	End Function
	''' <summary>
	''' Mark argument (param) tPoint
	''' </summary>
	''' <param name="tPoint"></param> 
	''' <param name="oaPoints"></param>
	Public Sub MarkPoint(ByVal tPoint As Point2d, ByRef oaPoints As TplnPointArray)
		If (mtPoint2d <> tPoint) Then
			oaPoints.Add(mtPoint2d)
			moSaltireMarkBlock.MarkPoint(mtPoint2d, 131S)
		End If

	End Sub
	Public Sub MarkSinglePoint(ByRef oaPoints As TplnPointArray)

		oaPoints.Add(mtPoint2d)
		moCircleMarkBlock.MarkPoint(mtPoint2d, 61S)


	End Sub
	Public Sub UpdatePoint(ByVal tNewPoint As Point2d, ByRef oaPoints As TplnPointArray)
		'	DMAcadExt.AcadDocument.WriteMessage(CStr(mtPoint2d.X) & "," & CStr(mtPoint2d.Y) & " UpdatePoint to:" & CStr(tNewPoint.X) & "," & CStr(tNewPoint.Y))
		Dim oDBObject As DBObject = AcadTransaction.GetDBObject(mtEntityObjID, OpenMode.ForWrite)

		Select Case oDBObject.GetRXClass().Name
			Case AcadConst.AcadPolylineName
				Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
				If oPolyline.NumberOfVertices = 2 Then
					zzUpdatePolylineSegmentPoint(oPolyline, tNewPoint, oaPoints)
				Else
					zzUpdatePolylinePoint(oPolyline, tNewPoint)
				End If

			Case AcadConst.Acad2dPolylineName
				Dim oPolyline2d As Polyline2d = DirectCast(oDBObject, Polyline2d)
				zzUpdatePolyline2dPoint(oPolyline2d, tNewPoint)
			Case AcadConst.AcadLineName
				Dim oLine As Line = DirectCast(oDBObject, Line)
				zzUpdateLinePoint(oLine, tNewPoint, oaPoints)
			Case AcadConst.AcadArcName
				Dim oArc As Arc = DirectCast(oDBObject, Arc)
				zzUpdateArcPoint(oArc, tNewPoint)
			Case AcadConst.AcadBlockRefName
				Dim oBlockRef As BlockReference = DirectCast(oDBObject, BlockReference)
				zzUpdatBlockRefPoint(oBlockRef, tNewPoint, oaPoints)
		End Select
	End Sub
	Private Sub zzUpdateArcPoint(oArc As Arc, ByVal tNewPoint As Point2d)
		'  Dim oArc As Arc = DirectCast(moCurve, Arc)
		Dim oTplnArc As TplnArc = New TplnArc(oArc, True)
		Dim tMidPoint As Point2d = oTplnArc.GetMidPoint()
		Dim oNewArc As CircularArc2d

		If miPointIndex = 0 AndAlso tNewPoint.IsEqualTo(TPlnPoint.Point3dTo2d(oArc.EndPoint)) Then
			oArc.Erase()
		ElseIf miPointIndex = 1 AndAlso tNewPoint.IsEqualTo(TPlnPoint.Point3dTo2d(oArc.StartPoint)) Then
			oArc.Erase()
		Else
			If tNewPoint.IsEqualTo(tMidPoint) Then
				If miPointIndex = 0 Then
					tMidPoint = oTplnArc.GetQuarterPoint(True)
				Else
					tMidPoint = oTplnArc.GetQuarterPoint(False)
				End If
			End If
			'	DMAcadExt.AcadDocument.WriteMessage("B Mid:" & CStr(tMidPoint.X) & "," & CStr(tMidPoint.Y))
			'	Dim tVector As Vector2d = New Vector2d(tNewPoint.X - tCenter.X, tNewPoint.Y - tCenter.Y)
			If miPointIndex = 0 Then
				oNewArc = New CircularArc2d(tNewPoint, tMidPoint, TPlnPoint.Point3dTo2d(oArc.EndPoint))
			Else
				oNewArc = New CircularArc2d(TPlnPoint.Point3dTo2d(oArc.StartPoint), tMidPoint, tNewPoint)
				'	DMAcadExt.AcadDocument.WriteMessage("Angle before:" & CStr(oArc.StartAngle) & "," & CStr(oArc.Radius))
			End If
			Dim tCenter As Point2d = oNewArc.Center
			Dim tVector As Vector2d = New Vector2d(oNewArc.StartPoint.X - tCenter.X, oNewArc.StartPoint.Y - tCenter.Y)
			oArc.Radius = oNewArc.Radius
			oArc.Center = TPlnPoint.Point2dTo3d(tCenter)
			oArc.StartAngle = tVector.Angle
			tVector = New Vector2d(oNewArc.EndPoint.X - tCenter.X, oNewArc.EndPoint.Y - tCenter.Y)
			oArc.EndAngle = tVector.Angle
			DMAcadExt.AcadDocument.WriteDebugMessage("new " & TPlnPoint.DispPoint(oNewArc.StartPoint) & "; " & TPlnPoint.DispPoint(oNewArc.EndPoint))

		End If
	End Sub
	Private Sub zzUpdateLinePoint(oLine As Line, ByVal tNewPoint As Point2d, ByRef oaFixPoints As TplnPointArray)
		Dim tOldPoint As Point3d
		tOldPoint = oLine.StartPoint
		If miPointIndex = 0 Then
			If tOldPoint <> TPlnPoint.Point2dTo3d(tNewPoint) Then
				oaFixPoints.Add(tOldPoint)
			End If

			oLine.StartPoint = TPlnPoint.Point2dTo3d(tNewPoint)
			'DMCommon.ExcelLogAW5.SetNextValue(0, "LineI", tOldPoint, tNewPoint, oLine.StartPoint, oaPoints.Count)

		ElseIf miPointIndex = 1 Then
			tOldPoint = oLine.EndPoint
			If tOldPoint <> TPlnPoint.Point2dTo3d(tNewPoint) Then
				oaFixPoints.Add(oLine.EndPoint)
			End If

			oLine.EndPoint = TPlnPoint.Point2dTo3d(tNewPoint)
			'DMCommon.ExcelLogAW5.SetNextValue(0, "LineII", tOldPoint, tNewPoint, oLine.EndPoint, oaFixPoints.Count)


		End If


	End Sub
	Private Sub zzUpdatLinePoint(oLine As Line, ByVal tNewPoint As Point2d)
		' Dim oLine As Line = DirectCast(moCurve, Line)
		Dim tAdjacentPoint As Point2d
		If miPointIndex = 0 Then
			tAdjacentPoint = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		ElseIf miPointIndex = 1 Then
			tAdjacentPoint = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		End If
		If tAdjacentPoint.IsEqualTo(tNewPoint, KeyPoint.PointTolerance) Then
			oLine.Erase()
		ElseIf miPointIndex = 0 Then
			oLine.StartPoint = TPlnPoint.Point2dTo3d(tNewPoint)
		ElseIf miPointIndex = 1 Then
			oLine.EndPoint = TPlnPoint.Point2dTo3d(tNewPoint)
		End If
	End Sub
	Private Sub zzUpdatBlockRefPoint(oBlockRef As BlockReference, ByVal tNewPoint As Point2d, ByRef oaPoints As TplnPointArray)
		Dim tOldPoint As Point3d = oBlockRef.Position
		If tOldPoint <> TPlnPoint.Point2dTo3d(tNewPoint) Then
			oaPoints.Add(tOldPoint)
		End If

		oBlockRef.Position = TPlnPoint.Point2dTo3d(tNewPoint)
		'DMCommon.ExcelLogAW5.SetNextValue(0, "BlockRef", tOldPoint, tNewPoint, oBlockRef.Position, oaPoints.Count)
		'	End If


	End Sub

	Private Sub zzUpdatePolylineSegmentPoint(oPolyline As Polyline, ByVal tNewPoint As Point2d, ByRef oaPoints As TplnPointArray)
		Dim tOldPoint As Point2d = oPolyline.GetPoint2dAt(miPointIndex)
		If tOldPoint.X <> tNewPoint.X OrElse tOldPoint.Y <> tNewPoint.Y Then
			oaPoints.Add(tOldPoint)
		End If

		oPolyline.SetPointAt(miPointIndex, tNewPoint)
		'DMCommon.ExcelLogAW5.SetNextValue(0, "PlSegment", tOldPoint, tNewPoint, oPolyline.GetPoint2dAt(miPointIndex), oaPoints.Count)
		'End If


	End Sub
	Private Sub zzUpdatePolylinePoint(oPolyline As Polyline, ByVal tNewPoint As Point2d)
		'   Dim oPolyline As Polyline = DirectCast(moCurve, Polyline)
		Dim bRemovePoint As Boolean = False
		Dim tAdjacentPoint As Point2d
		If miPointIndex > 0 Then
			tAdjacentPoint = oPolyline.GetPoint2dAt(miPointIndex - 1)
			If tAdjacentPoint.IsEqualTo(tNewPoint, KeyPoint.PointTolerance) Then
				bRemovePoint = True
			End If
		End If
		If Not bRemovePoint AndAlso miPointIndex < oPolyline.NumberOfVertices - 1 Then
			Try
				tAdjacentPoint = oPolyline.GetPoint2dAt(miPointIndex + 1)
			Catch oEx As Exception

			End Try

			If tAdjacentPoint.IsEqualTo(tNewPoint, KeyPoint.PointTolerance) Then
				bRemovePoint = True
			End If
		End If
		oPolyline.SetPointAt(miPointIndex, tNewPoint)
		If bRemovePoint Then
			Try
				DMAcadExt.AcadDocument.WriteDebugMessage("Beff " & oPolyline.GetPoint2dAt(0).ToString() & "; " & oPolyline.GetPoint2dAt(1).ToString() & "; " & oPolyline.Handle.ToString() & "; " & oPolyline.NumberOfVertices.ToString())

				If Not mcolZeroLenPolylines.Contains(oPolyline) Then
					mcolZeroLenPolylines.Add(oPolyline)
				End If

				'''''''''''''''''''''''''  oPolyline.RemoveVertexAt(miPointIndex)
			Catch ex As Exception
				DMAcadExt.AcadDocument.WriteDebugMessage("Rem " & oPolyline.GetPoint2dAt(0).ToString() & "; " & oPolyline.GetPoint2dAt(1).ToString() & "; " & oPolyline.Handle.ToString() & "; " & miPointIndex.ToString())
			End Try



		End If
	End Sub
	Private Sub zzUpdatePolyline2dPoint(oPolyline2d As Polyline2d, ByVal tNewPoint As Point2d)

		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim tAcObjID As ObjectId
		Dim oVertex2d As Vertex2d = Nothing
		Dim oThisVertex2d As Vertex2d = Nothing
		Dim bRemovePoint As Boolean = False
		Dim oDBObj As DBObject
		Dim tAdjacentPoint As Point2d
		Dim iPointIndex As Integer = 0
		'	DMAcadExt.AcadDocument.WriteMessage("2d:Start" & TPlnPoint.DispPoint(oPolyline2d.StartPoint))
		Try
			Do While oColEnum.MoveNext()
				tAcObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForWrite)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				If miPointIndex = iPointIndex Then
					oThisVertex2d = oVertex2d
				ElseIf Math.Abs(miPointIndex - iPointIndex) = 1 Then
					tAdjacentPoint = TPlnPoint.Point3dTo2d(oVertex2d.Position)
					If tAdjacentPoint.IsEqualTo(tNewPoint, KeyPoint.PointTolerance) Then
						bRemovePoint = True
					End If
				Else

				End If
				iPointIndex += 0
				'	DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iVert) & ":" & TPlnPoint.DispPoint(oVertex2d.Position) & ";" & CStr(oVertex2d.Bulge) & "-" & oVertex2d.VertexType.ToString())
			Loop
		Catch oEx As Exception
			'		DMAcadExt.AcadDocument.WriteMessage("Reset:" & oEx.Message)
		End Try
		'	DMAcadExt.AcadDocument.WriteMessage("2d:End" & TPlnPoint.DispPoint(oPolyline2d.EndPoint))

		If oVertex2d IsNot Nothing Then
			If bRemovePoint Then
				oVertex2d.Erase()
			Else
				oVertex2d.Position = TPlnPoint.Point2dTo3d(tNewPoint)
			End If
		End If
	End Sub
End Structure
