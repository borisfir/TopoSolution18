Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Data
Public Enum enAttachType
   PointToPoint
   PointToPointObject
   PointToEntity2D
   PointToCurve

End Enum
Public Class AttachPair
   Public Point As TPlnPoint
   Public SourceObjID As ObjectId
   Public AttachedObjID As ObjectId
   Public Sub New(oPoint As TPlnPoint, tSourceObjID As ObjectId)
      Point = oPoint
      SourceObjID = tSourceObjID
   End Sub
End Class
Public Class AttachedSegment
    Inherits AttachPair
    Public Segment As TplnSegment
    Public Sub New(oPoint As TPlnPoint, tSourceObjID As ObjectId)
        MyBase.New(oPoint, tSourceObjID)

    End Sub

End Class
Public Class AttachEntities

   Public Enum enStatus
      NotFound
      OneEntry
      ManyEntries
      Status1
   End Enum
   Private Const msXFldName As String = "X"
   Private Const msYFldName As String = "Y"
   Private Const msXMinFldName As String = "XMin"
   Private Const msXMaxFldName As String = "XMax"
   Private Const msYMinFldName As String = "YMin"
   Private Const msYMaxFldName As String = "YMax"

   Private Const msKeyNameFldName As String = "KeyName"
   Private Const msStatusFldName As String = "Status"
   Private Const msEntityFldName As String = "Entity"
   Private moaSourcePoints As TplnPointArray
   Private mcolAttachedPairs As ICollection(Of AttachPair)
   Private mdicSegments As IDictionary(Of ObjectId, TplnSegment)
   Private mdicCurves As IDictionary(Of ObjectId, Curve)

   Private mtPoints As System.Data.DataTable
   Private mtLines As System.Data.DataTable
   Private miAttachType As enAttachType
   Private moTriangleX As Polyline
   'Private moTriangle As Polyline
   Private moOutput() As AttachPair
   Private moOutputAcadObjects() As ObjectId
	Private moOutputObjects() As System.Object

	Private moOutputStatuses() As enStatus

   Private mdTolerance As Double
   Private miAttachedCount As Integer
   Private miPointDataCount As Integer
   Private miEntriesCount As Integer
   Private miFailsCount As Integer
   Private moaResPoints As TplnPointArray
   Private mdicNetPoints As Dictionary(Of Decimal, TPlnPoint)
   Private mdicNetPointsPlus As Dictionary(Of ULong, ObjectIdCollection)
	Private moCircleMarkBlock As DMAcadExt.MarkBlock
	Private moEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
   Public Sub New(iAttachType As enAttachType, Optional dTolerance As Double = 0.01)
      miAttachType = iAttachType
      mcolAttachedPairs = New System.Collections.ObjectModel.Collection(Of AttachPair)

      mdTolerance = dTolerance
      zzCreateTable()
   End Sub
   Public Sub New(iAttachType As enAttachType, colAttachedPairs As ICollection(Of AttachPair), Optional dTolerance As Double = 0.01)
      miAttachType = iAttachType
      mcolAttachedPairs = colAttachedPairs
      mdTolerance = dTolerance
      zzCreateTable()
   End Sub
   Public Sub New(iAttachType As enAttachType, moaPoints() As TPlnPoint, Optional dTolerance As Double = 0.01)
      ReDim moOutputAcadObjects(moaPoints.GetUpperBound(0))
      ReDim moOutputStatuses(moaPoints.GetUpperBound(0))
      zzCreateTable()
   End Sub
   Public Shared Function GetPoint(oDataRow As DataRowView) As Point3d
      Dim dX As Double = DirectCast(oDataRow.Item(msXFldName), Double)
      Dim dY As Double = DirectCast(oDataRow.Item(msYFldName), Double)
      Return New Point3d(dX, dY, 0.0)
   End Function
   Public Sub AddPointEntity(oPoint As TPlnPoint, tAcObjId As ObjectId, bIsSource As Boolean)
      If bIsSource Then
         mcolAttachedPairs.Add(New AttachPair(oPoint, tAcObjId))
      Else
         Dim oNewRow As DataRow = mtPoints.NewRow()

         oNewRow.Item(msXFldName) = oPoint.X
         oNewRow.Item(msYFldName) = oPoint.Y

         oNewRow.Item(msStatusFldName) = 0
         oNewRow.Item(msEntityFldName) = tAcObjId
         mtPoints.Rows.Add(oNewRow)
      End If

   End Sub
   Private Sub zzAddPointToDic(oPoint As TPlnPoint, tAcObjId As ObjectId)
      Dim oNetPoint As TPlnPoint = Nothing
      Dim colObjectIds As ObjectIdCollection = Nothing
      If mdicNetPointsPlus.TryGetValue(oPoint.PointKey, colObjectIds) Then
         colObjectIds.Add(tAcObjId)
         'DMAcadExt.AcadDocument.WriteMessageLog("1----------1:" & CStr(colObjectIds.Count))
      Else
         colObjectIds = New ObjectIdCollection
         colObjectIds.Add(tAcObjId)
         mdicNetPointsPlus.Add(oPoint.PointKey, colObjectIds)
         '  DMAcadExt.AcadDocument.WriteMessageLog("2+++2: " & CStr(mdicNetPointsPlus.Count) & " *** " & CStr(mdicNetPoints.Count))
      End If
   End Sub
   Private Sub zzAddPointNotNetA(oPoint As TPlnPoint, tAcObjId As ObjectId)
      Dim oNetPoint As TPlnPoint = Nothing
      If Not mdicNetPoints.ContainsKey(oPoint.PointKey) Then
         mdicNetPoints.Add(oPoint.PointKey, oPoint)
         ' AddPointEntity(oPoint, tAcObjId, False)
      End If
   End Sub
   Private Sub AddCurve_Points(oCurve As DBObject)
      If oCurve IsNot Nothing Then
         Select Case oCurve.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadLineName

               Me.zzAddLine(DirectCast(oCurve, Line))

            Case DMAcadExt.AcadConst.AcadPolylineName

               zzAddPolyline(DirectCast(oCurve, Polyline))
            Case DMAcadExt.AcadConst.AcadArcName

               Me.zzAddArc(DirectCast(oCurve, Arc))
            Case DMAcadExt.AcadConst.Acad2dPolylineName

               ''	DMAcadExt.AcadDocument.WriteMessage("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
               ' zzAddPolyline2d(DirectCast(oDBObject, Polyline2d))
            Case Else
               DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
               Return
         End Select
      End If
   End Sub
   Private Sub zzAddLine(oLine As Line)
      '  zzAddPointNotNet(New TPlnPoint(oLine.StartPoint), oLine.ObjectId)
      ' zzAddPointNotNet(New TPlnPoint(oLine.EndPoint), oLine.ObjectId)

      zzAddPointToDic(New TPlnPoint(oLine.StartPoint), oLine.ObjectId)
      zzAddPointToDic(New TPlnPoint(oLine.EndPoint), oLine.ObjectId)


      Add2DEntity(oLine.GeometricExtents, oLine.ObjectId)
   End Sub
   Private Sub zzAddArc(oArc As Arc)
      ' zzAddPointNotNet(New TPlnPoint(oArc.StartPoint), oArc.ObjectId)
      ' zzAddPointNotNet(New TPlnPoint(oArc.EndPoint), oArc.ObjectId)

      zzAddPointToDic(New TPlnPoint(oArc.StartPoint), oArc.ObjectId)
      zzAddPointToDic(New TPlnPoint(oArc.EndPoint), oArc.ObjectId)
      Add2DEntity(oArc.GeometricExtents, oArc.ObjectId)
   End Sub


   Private Sub zzAddPolyline(oPolyline As Polyline)
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim tCurrentPoint As Point2d
      Dim tNextPoint As Point2d
      Dim dBulge As Double

      Dim oTplnArc As TplnArc
      Dim oTplnLine As TplnLine
      Dim iSegmentType As SegmentType
      Dim tBox As TPlnBoundingBox
      For iIndex As Integer = 0 To iVerticesUB
         tBox = Nothing
         tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
         iSegmentType = oPolyline.GetSegmentType(iIndex)
         'AddPointEntity(New TPlnPoint(tCurrentPoint), oPolyline.ObjectId, False)
         '  zzAddPointNotNet(New TPlnPoint(tCurrentPoint), oPolyline.ObjectId)
         zzAddPointToDic(New TPlnPoint(tCurrentPoint), oPolyline.ObjectId)
         Select Case iSegmentType
            Case SegmentType.Arc
               dBulge = oPolyline.GetBulgeAt(iIndex)
               tNextPoint = oPolyline.GetPoint2dAt(iIndex + 1)
               oTplnArc = New TplnArc(tCurrentPoint, tNextPoint, dBulge)
               tBox = oTplnArc.GetBoundingBox()
            Case SegmentType.Line
               tNextPoint = oPolyline.GetPoint2dAt(iIndex + 1)
               oTplnLine = New TplnLine(tCurrentPoint, tNextPoint)
               tBox = oTplnLine.GetBoundingBox()
         End Select
         If tBox IsNot Nothing Then
            Add2DEntity(tBox, oPolyline.ObjectId)
         End If
      Next
   End Sub

   Public Sub Add2DEntity(oBox As Autodesk.AutoCAD.DatabaseServices.Extents3d, tAcObjId As ObjectId)
      Dim oNewRow As DataRow = mtLines.NewRow()

      oNewRow.Item(msXMinFldName) = oBox.MinPoint.X
      oNewRow.Item(msYMinFldName) = oBox.MinPoint.Y
      oNewRow.Item(msXMaxFldName) = oBox.MaxPoint.X
      oNewRow.Item(msYMaxFldName) = oBox.MaxPoint.Y
      oNewRow.Item(msStatusFldName) = 0
      oNewRow.Item(msEntityFldName) = tAcObjId
      mtLines.Rows.Add(oNewRow)
   End Sub
   Public Sub Add2DEntity(oBox As TPlnBoundingBox, tAcObjId As ObjectId)
      Dim oNewRow As DataRow = mtLines.NewRow()

      oNewRow.Item(msXMinFldName) = oBox.MinPoint.X
      oNewRow.Item(msYMinFldName) = oBox.MinPoint.Y
      oNewRow.Item(msXMaxFldName) = oBox.MaxPoint.X
      oNewRow.Item(msYMaxFldName) = oBox.MaxPoint.Y
      oNewRow.Item(msStatusFldName) = 0
      oNewRow.Item(msEntityFldName) = tAcObjId
      mtLines.Rows.Add(oNewRow)
   End Sub
   Public Property SourcePoints As TplnPointArray
      Get
         Return moaSourcePoints
      End Get
      Set(oValue As TplnPointArray)
         moaSourcePoints = oValue
      End Set
   End Property
   Public Sub AddCurvesIdCol(colCurvesIds As ObjectIdCollection)
      Dim oDBObject As DBObject
      For Each tCurveObjID As ObjectId In colCurvesIds
         oDBObject = AcadTransaction.GetDBObject(tCurveObjID, OpenMode.ForRead)
         AddCurve_Points(oDBObject)
      Next
      'System.Windows.Forms.MessageBox.Show(mdicNetPoints.Count.ToString & vbCrLf & mdicNetPointsPlus.Count.ToString, "05_177")
   End Sub
   Public Sub AddCurves(colSegments As ICollection(Of Curve))
      mdicSegments = New Dictionary(Of ObjectId, TplnSegment)
      For Each oSegment As Curve In colSegments
         Add2DEntity(oSegment.GeometricExtents, oSegment.ObjectId)
         '''''''  mdicSegments.Add(oSegment.ObjectId, oSegment)
      Next
   End Sub
   Public Sub AddSegments(colSegments As ICollection(Of TplnSegment))
      mdicSegments = New Dictionary(Of ObjectId, TplnSegment)
      For Each oSegment As TplnSegment In colSegments
         Add2DEntity(oSegment.Extents, oSegment.AcObjID)
         mdicSegments.Add(oSegment.AcObjID, oSegment)
      Next
   End Sub
   Public Sub AddNetPoints(dicNetPoints As Dictionary(Of Decimal, TPlnPoint))
      mdicNetPoints = dicNetPoints
      mdicNetPointsPlus = New Dictionary(Of ULong, ObjectIdCollection)()
      For Each tPointKey As ULong In dicNetPoints.Keys
         mdicNetPointsPlus.Add(tPointKey, New ObjectIdCollection())

      Next
   End Sub
   Public Sub AddPointArray(oaPoints As TplnPointArray, bIsSource As Boolean)
      If bIsSource Then

      Else
         For iIndex As Integer = 0 To oaPoints.UpperBound
            AddPointEntity(oaPoints.Item(iIndex), oaPoints.Item(iIndex).AcObjID, False)
         Next
      End If

   End Sub

   Public Sub AddPointCollection(colPoints As System.Collections.Generic.ICollection(Of TPlnPoint), bIsSource As Boolean)

      For Each oPoint As TPlnPoint In colPoints
         AddPointEntity(oPoint, oPoint.AcObjID, bIsSource)
      Next




   End Sub
   Public Function GetPointLocation(oPoint As TPlnPoint) As enStatus


      Dim tAcObjID As ObjectId
      Dim iStatus As enStatus = zzFindPointByPoint(oPoint, False, tAcObjID)
      Return iStatus
   End Function
   Public Sub Calculate(bStatus As Boolean)
      '  System.Windows.Forms.MessageBox.Show(miAttachType.ToString() & vbCrLf & CStr(moaSourcePoints.Count) & vbCrLf & CStr(mtPoints.Rows.Count), "Lines 03_431")
      Select Case miAttachType
         Case enAttachType.PointToPoint

            zzCalculationPointToPoint(bStatus)
         Case enAttachType.PointToPointObject
            zzCalculationPointObjectToPoint(bStatus)
         Case enAttachType.PointToEntity2D

            zzCalculationPontToLine()
         Case enAttachType.PointToCurve
            moaResPoints = New TplnPointArray()
            moCircleMarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Circle)
            '  zzCalcPointToCurve()
            zzCalcPointToCurveNew()
      End Select
   End Sub
   Public ReadOnly Property Output As AttachPair()
      Get
         Return moOutput
      End Get
   End Property
   Public ReadOnly Property PointDataCount As Integer
      Get
         Return miPointDataCount
      End Get
   End Property

   Public ReadOnly Property EntriesCount As Integer
      Get
         Return miEntriesCount
      End Get
   End Property

   Public ReadOnly Property FailsCount As Integer
      Get
         Return miFailsCount
      End Get
   End Property
   Public ReadOnly Property ResPoints As TplnPointArray
      Get
         Return moaResPoints
      End Get
   End Property

   Private Sub zzCalcPointToCurveNew()
      Dim oPoint As TPlnPoint
      Dim oTestPoint As TPlnPoint = Nothing

      Dim colObjectIDs As ObjectIdCollection

      For Each tPointKey As ULong In mdicNetPointsPlus.Keys
         oPoint = TplnPointKeyLong.PointFromKey(tPointKey)
         ' oTestPoint = mdicNetPoints.Item(tPointKey)
         If Not mdicNetPoints.TryGetValue(tPointKey, oTestPoint) Then

            oTestPoint = oPoint
         End If
         ' moEditor.WriteMessage("???TEST:" & oPoint.Coordinates & " | " & CStr(oTestPoint.Coordinates) & "; ")
         colObjectIDs = mdicNetPointsPlus.Item(tPointKey)
         zzFindCurveClosePointNew(oTestPoint, colObjectIDs)
      Next
   End Sub
 
   Private Sub zzCalcPointToCurve()
      For Each oPoint As TPlnPoint In mdicNetPoints.Values
         zzFindCurveClosePoint(oPoint)
      Next
   End Sub


   Public ReadOnly Property OutputAcadObjects As ObjectId()
      Get
         Return moOutputAcadObjects
      End Get
   End Property
   Public ReadOnly Property OutputObjects As System.Object()
      Get
         Return moOutputObjects
      End Get
   End Property
   Public ReadOnly Property OutputStatuses As enStatus()
      Get
         Return moOutputStatuses
      End Get
   End Property
   Private Sub zzCreateTable()
      Select Case miAttachType
         Case enAttachType.PointToPoint, enAttachType.PointToPointObject
            zzCreatePointTable()
         Case enAttachType.PointToEntity2D, enAttachType.PointToCurve
            zzCreateLineTable()
      End Select
   End Sub

  
   Private Sub zzCalculationPointObjectToPoint(bStatus As Boolean)
      Dim tAttachedAcObjID As ObjectId
      Dim oPoint As TPlnPoint
      Dim oPointObject As TplnPointObject

      ' System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound), "03_284b")
      ReDim moOutputAcadObjects(moaSourcePoints.UpperBound)
      ReDim moOutputObjects(moaSourcePoints.UpperBound)
      ReDim moOutputStatuses(moaSourcePoints.UpperBound)
      '  System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound), "03_291b")

      For iIndex As Integer = 0 To moaSourcePoints.UpperBound
         '  System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound) & ":" & CStr(iIndex), "03_420b")
         oPointObject = moaSourcePoints.ItemObject(iIndex)
         '  System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound) & ":" & CStr(iIndex), "03_421b")
         If oPointObject Is Nothing Then
            System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound) & ":" & iIndex.ToString(), "03_311b")
         Else
            oPoint = oPointObject.Point
            If oPoint IsNot Nothing Then

               moOutputStatuses(iIndex) = zzFindPointByPoint(oPoint, bStatus, tAttachedAcObjID)
               moOutputObjects(iIndex) = oPointObject.ThisObject

               If Not tAttachedAcObjID.IsNull Then
                  moOutputAcadObjects(iIndex) = tAttachedAcObjID
               End If
            Else
               System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound) & ":" & CStr(iIndex), "03_501b")
            End If
         End If
      Next
      '  System.Windows.Forms.MessageBox.Show("!!!!" & CStr(moaSourcePoints.UpperBound), "03_298b")
   End Sub
   Private Sub zzCalculationPointToPoint(bStatus As Boolean)
      Dim tAttachedAcObjID As ObjectId
      Dim oPoint As TPlnPoint
      System.Windows.Forms.MessageBox.Show(CStr(moaSourcePoints.UpperBound), "03_284b")
      ReDim moOutputAcadObjects(moaSourcePoints.UpperBound)
      ReDim moOutputStatuses(moaSourcePoints.UpperBound)

      For iIndex As Integer = 0 To moaSourcePoints.UpperBound
         oPoint = moaSourcePoints.Item(iIndex)
         If oPoint IsNot Nothing Then
            moOutputStatuses(iIndex) = zzFindPointByPoint(oPoint, bStatus, tAttachedAcObjID)
            If Not tAttachedAcObjID.IsNull Then
               moOutputAcadObjects(iIndex) = tAttachedAcObjID
            End If
         End If

      Next
   End Sub
   Private Sub zzCalculationPontToLine()

      Dim tAttachedAcObjID As ObjectId
      Dim oSegment As TplnSegment = Nothing
      Dim oAttachedSegment As AttachedSegment
      Dim iIndex As Integer
      ReDim moOutput(mcolAttachedPairs.Count - 1)
      For Each oAttachPair As AttachPair In mcolAttachedPairs
         tAttachedAcObjID = zzFindLineEntityByPoint(oAttachPair.Point)
         oAttachedSegment = New AttachedSegment(oAttachPair.Point, oAttachPair.SourceObjID)
         If Not tAttachedAcObjID.IsNull Then

            If mdicSegments.TryGetValue(tAttachedAcObjID, oSegment) Then
               oAttachedSegment.Segment = oSegment
            End If
            '	oAttachPair.AttachedObjID = tAttachedAcObjID
            miAttachedCount += 1
         End If

         moOutput(iIndex) = oAttachedSegment
         iIndex += 1
      Next
   End Sub
   Public ReadOnly Property AttachedCount As Integer
      Get
         Return miAttachedCount
      End Get
   End Property
   Public Function GetAttachedPairs() As ICollection(Of AttachPair)
      Return mcolAttachedPairs
   End Function
   Private Sub zzCreatePointTable()
      Dim oDataColumn As System.Data.DataColumn
      Dim t As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim oDataType As System.Type = t.GetType()

      'Dim tPoints As System.Data.DataTable
      mtPoints = New System.Data.DataTable("Points")

      oDataColumn = New System.Data.DataColumn(msXFldName, GetType(System.Double))
      mtPoints.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn(msYFldName, GetType(System.Double))
      mtPoints.Columns.Add(oDataColumn)

      '	oDataColumn = New System.Data.DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      '	mtPoints.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn(msStatusFldName, GetType(System.Int32))
      mtPoints.Columns.Add(oDataColumn)

      '	oDataColumn = New System.Data.DataColumn(msEntityFldName, System.Type.GetType("Autodesk.AutoCAD.DatabaseServices.ObjectId"))
      oDataColumn = New System.Data.DataColumn(msEntityFldName, oDataType)
      mtPoints.Columns.Add(oDataColumn)

   End Sub
   Private Sub zzCreateLineTable()

      Dim oDataColumn As System.Data.DataColumn
      Dim t As Autodesk.AutoCAD.DatabaseServices.ObjectId

      Dim oDataType As System.Type = t.GetType()
      '	Dim tLines As System.Data.DataTable
      mtLines = New System.Data.DataTable("Lines")

      oDataColumn = New System.Data.DataColumn(msXMinFldName, GetType(System.Double))
      mtLines.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn(msXMaxFldName, GetType(System.Double))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn(msYMinFldName, GetType(System.Double))
      mtLines.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn(msYMaxFldName, GetType(System.Double))
      mtLines.Columns.Add(oDataColumn)

      '	oDataColumn = New System.Data.DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      '	mtLines.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn(msStatusFldName, System.Type.GetType("System.Int32"))
      mtLines.Columns.Add(oDataColumn)

      '	oDataColumn = New System.Data.DataColumn(msEntityFldName, System.Type.GetType("Autodesk.AutoCAD.DatabaseServices.ObjectId"))
      oDataColumn = New System.Data.DataColumn(msEntityFldName, oDataType)
      mtLines.Columns.Add(oDataColumn)

   End Sub
   Private Function zzFindSegmentByPoint(ByVal oPoint As TPlnPoint) As ObjectId
      Dim bResp As Boolean
      Dim sRowFilter As String
      Dim oLinesView As DataView
      Dim tLineObjID As ObjectId
      Dim oEntity As Entity
      ''OLD 19/07/05   Dim sRowFilter As String = sStatusFldName & "=0 AND " & sKeyNameFldName & "='" & sKeyName & "' AND " & sXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & sXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & sYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & sYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
      Try
         sRowFilter = msStatusFldName & "=0 AND " & msXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & msXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & msYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & msYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
         oLinesView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_9")
         Return Nothing

      End Try

      If oLinesView.Count > 0 Then
         Try
            For Each oDataRow As DataRowView In oLinesView
               Try
                  tLineObjID = DirectCast(oDataRow.Item(msEntityFldName), ObjectId)
                  oEntity = DMAcadExt.AcadTransaction.GetEntity(tLineObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                  '	oLine.ClearElevation()


                  bResp = zzIsIntersectWithPointX(oEntity, oPoint, mdTolerance)
                  If bResp Then
                     oDataRow.Item(msStatusFldName) = 1
                     Return tLineObjID
                  Else
                     moEditor.WriteMessage("L:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
                  End If
               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_3")
                  Return Nothing
               End Try
            Next
            moEditor.WriteMessage("Not Point:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_1")
         End Try
         Return Nothing
      Else
         moEditor.WriteMessage("Not List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
      End If
      Return Nothing
   End Function
   Private Function zzFindCurveClosePointNew(ByVal oPoint As TPlnPoint, dicPointObjIds As ObjectIdCollection) As ObjectId
      '  Dim bResp As Boolean
      Dim sRowFilter As String = String.Empty
      Dim oLinesView As DataView
      Dim tLineObjID As ObjectId
      Dim oCurve As Curve
      Dim dDist As Double
      Dim tClosestPointOnCurve As Point3d
      Dim hsCurvesObjIDs As HashSet(Of ObjectId) = New HashSet(Of ObjectId)()
      Dim dMinDist As Double
      ''OLD 19/07/05   Dim sRowFilter As String = sStatusFldName & "=0 AND " & sKeyNameFldName & "='" & sKeyName & "' AND " & sXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & sXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & sYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & sYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
      Try
         sRowFilter = msXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & msXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & msYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & msYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
         oLinesView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(mtLines Is Nothing) & vbCrLf & sRowFilter, "IstrAttachBuffer - zzFindLineByPoint_8")
         Return ObjectId.Null

      End Try

    
      If oLinesView.Count > 0 Then
         hsCurvesObjIDs.Clear()
         dMinDist = 9999.0
         '  moEditor.WriteMessage("Yes List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; " & vbCrLf)
         Try
            For Each oDataRow As DataRowView In oLinesView
               Try
                  tLineObjID = DirectCast(oDataRow.Item(msEntityFldName), ObjectId)
                  
                  If Not dicPointObjIds.Contains(tLineObjID) AndAlso Not hsCurvesObjIDs.Contains(tLineObjID) Then
                     hsCurvesObjIDs.Add(tLineObjID)
                     oCurve = DMAcadExt.AcadTransaction.GetCurve(tLineObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                     If oPoint.X = 201569.6 And oPoint.Y = 683097.3 Then
                        '  moEditor.WriteMessage("Curve Exists: " & CStr(oCurve IsNot Nothing) & vbCrLf)
                     End If
                     ' dDist = oCurve.GetDistAtPoint(oPoint.AcGePoint3d)
                     Try
                        tClosestPointOnCurve = oCurve.GetClosestPointTo(oPoint.AcGePoint3d, False)
                        dDist = oPoint.AcGePoint3d.DistanceTo(tClosestPointOnCurve)
                         
                        If dDist > 0 AndAlso dMinDist > dDist Then
                           dMinDist = dDist

                        End If

                     Catch oEx As Exception
                        System.Windows.Forms.MessageBox.Show(oEx.Message, "IstrAttachBuffer - zzFindLineByPoint_4")
                     End Try




                  End If

               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_3")
                  Return ObjectId.Null
               End Try
            Next
            If dMinDist < mdTolerance Then
               moaResPoints.Add(oPoint)
               moCircleMarkBlock.MarkPoint(oPoint.AcGePoint, 4S)
               '''''''moEditor.WriteMessage("Point:" & oPoint.Coordinates & "|" & CStr(oLinesView.Count) & "/" & hsCurvesObjIDs.Count.ToString() & "|" & dMinDist.ToString() & vbCrLf)
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_1")
         End Try
         Return Nothing
      Else
         'moEditor.WriteMessage("Not List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
      End If
      Return Nothing
   End Function

   Private Function zzFindCurveClosePoint(ByVal oPoint As TPlnPoint) As ObjectId
      '  Dim bResp As Boolean
      Dim sRowFilter As String
      Dim oLinesView As DataView
      Dim tLineObjID As ObjectId
      Dim oCurve As Curve
      Dim dDist As Double
      Dim tClosestPointOnCurve As Point3d
      Dim hsCurvesObjIDs As HashSet(Of ObjectId) = New HashSet(Of ObjectId)()
      Dim dMinDist As Double
      ''OLD 19/07/05   Dim sRowFilter As String = sStatusFldName & "=0 AND " & sKeyNameFldName & "='" & sKeyName & "' AND " & sXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & sXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & sYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & sYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
      Try
         sRowFilter = msXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & msXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & msYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & msYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
         oLinesView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_7")
         Return Nothing

      End Try

      If oLinesView.Count > 0 Then
         hsCurvesObjIDs.Clear()
         dMinDist = 9999.0
         '  moEditor.WriteMessage("Yes List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; " & vbCrLf)
         Try
            For Each oDataRow As DataRowView In oLinesView
               Try
                  tLineObjID = DirectCast(oDataRow.Item(msEntityFldName), ObjectId)
                  If tLineObjID.OldIdPtr.ToInt64 = 8796082942576 Then
                     moEditor.WriteMessage("------------------------------------------------------" & vbCrLf)
                  End If
                  If oPoint.AcObjID <> tLineObjID AndAlso Not hsCurvesObjIDs.Contains(tLineObjID) Then
                     hsCurvesObjIDs.Add(tLineObjID)
                     oCurve = DMAcadExt.AcadTransaction.GetCurve(tLineObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                     ' dDist = oCurve.GetDistAtPoint(oPoint.AcGePoint3d)
                     Try
                        tClosestPointOnCurve = oCurve.GetClosestPointTo(oPoint.AcGePoint3d, False)
                        dDist = oPoint.AcGePoint3d.DistanceTo(tClosestPointOnCurve)
                        If dDist > 0 AndAlso dMinDist > dDist Then
                           dMinDist = dDist
                        End If
                        
                     Catch oEx As Exception
                        System.Windows.Forms.MessageBox.Show(oEx.Message, "IstrAttachBuffer - zzFindLineByPoint_4")
                     End Try



                    
                  End If

               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_3")
                  Return Nothing
               End Try
            Next
            If dMinDist < mdTolerance Then
               moaResPoints.Add(oPoint)
               moEditor.WriteMessage("Point:" & oPoint.Coordinates & "|" & CStr(oLinesView.Count) & "/" & hsCurvesObjIDs.Count.ToString() & "|" & dMinDist.ToString() & vbCrLf)
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_1")
         End Try
         Return Nothing
      Else
         'moEditor.WriteMessage("Not List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
      End If
      Return Nothing
   End Function

   Private Function zzFindLineEntityByPoint(ByVal oPoint As TPlnPoint) As ObjectId
      Dim bResp As Boolean
      Dim sRowFilter As String
      Dim oLinesView As DataView
      Dim tLineObjID As ObjectId
      Dim oEntity As Entity
      ''OLD 19/07/05   Dim sRowFilter As String = sStatusFldName & "=0 AND " & sKeyNameFldName & "='" & sKeyName & "' AND " & sXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & sXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & sYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & sYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
      Try
         sRowFilter = msStatusFldName & "=0 AND " & msXMinFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & msXMaxFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & msYMinFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & msYMaxFldName & ">" & CStr(oPoint.Y - mdTolerance)
         oLinesView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_9")
         Return Nothing

      End Try

      If oLinesView.Count > 0 Then
         Try
            For Each oDataRow As DataRowView In oLinesView
               Try
                  tLineObjID = DirectCast(oDataRow.Item(msEntityFldName), ObjectId)
                  oEntity = DMAcadExt.AcadTransaction.GetEntity(tLineObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                  '	oLine.ClearElevation()


                  bResp = zzIsIntersectWithPointX(oEntity, oPoint, mdTolerance)
                  If bResp Then
                     oDataRow.Item(msStatusFldName) = 1
                     Return tLineObjID
                  Else
                     moEditor.WriteMessage("L:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
                  End If
               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_3")
                  Return Nothing
               End Try
            Next
            moEditor.WriteMessage("Not Point:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_1")
         End Try
         Return Nothing
      Else
         moEditor.WriteMessage("Not List:" & oPoint.Coordinates & " | " & CStr(oLinesView.Count) & "; ")
      End If
      Return Nothing
   End Function
   Public Function GetPointData(ByVal oPoint As TPlnPoint, bStatus As Boolean) As DataRowView
      Dim sStatusFilter As String
      If bStatus Then
         sStatusFilter = msStatusFldName & "=0 AND "
      Else
         sStatusFilter = String.Empty
      End If
      Dim sRowFilter As String = sStatusFilter & msXFldName & "<=" & CStr(oPoint.X + mdTolerance) & " AND " & msXFldName & ">=" & CStr(oPoint.X - mdTolerance) & " AND " & msYFldName & "<=" & CStr(oPoint.Y + mdTolerance) & " AND " & msYFldName & ">=" & CStr(oPoint.Y - mdTolerance)
      Dim oPointView As DataView = New DataView(mtPoints, sRowFilter, String.Empty, DataViewRowState.Added)
      Dim oDataRow As DataRowView
      Dim oRow As DataRow = mtPoints.Rows.Item(0)

      '    moEditor.WriteMessage("!@Point:" & oPoint.Coordinates & " | " & CStr(oPointView.Count) & ";" & CStr(mtPoints.Rows.Count) & " ~~ " & sRowFilter)
      '    moEditor.WriteMessage("!$:ROW" & oRow.Item(0).ToString() & " | " & CStr(oRow.Item(1).ToString()) & ";" & CStr(oRow.Item(2).ToString()) & " <> " & oRow.RowState.ToString())
      miEntriesCount += 1
      If oPointView.Count = 1 Then
         oDataRow = oPointView.Item(0)
         If DirectCast(oDataRow.Item(msStatusFldName), Integer) <> 1 Then
            miPointDataCount += 1
         End If
         oDataRow.Item(msStatusFldName) = 1


			Return oDataRow

      ElseIf oPointView.Count = 0 Then
         miFailsCount += 1
         Return Nothing
      Else
         Return Nothing
      End If
   End Function

   Private Function zzFindPointByPoint(ByVal oPoint As TPlnPoint, bStatus As Boolean, ByRef tAcObjID As ObjectId) As enStatus

      Dim sStatusFilter As String
      Dim sHandleMsg As String
      Dim sEntityMsg As String

      If bStatus Then
         sStatusFilter = msStatusFldName & "=0 AND "
      Else
         sStatusFilter = String.Empty
      End If
      Dim sRowFilter As String = sStatusFilter & msXFldName & "<" & CStr(oPoint.X + mdTolerance) & " AND " & msXFldName & ">" & CStr(oPoint.X - mdTolerance) & " AND " & msYFldName & "<" & CStr(oPoint.Y + mdTolerance) & " AND " & msYFldName & ">" & CStr(oPoint.Y - mdTolerance)
      Dim oPointView As DataView = New DataView(mtPoints, sRowFilter, String.Empty, DataViewRowState.Added)
      Dim oDataRow As DataRowView
		'moEditor.WriteMessage("#177 " & mtPoints.Rows.Count.ToString() & ":" & oPointView.Count.ToString() & "-" & sRowFilter & "; " & "mdTol=" & mdTolerance.ToString())
		If oPointView.Count = 1 Then
         oDataRow = oPointView.Item(0)
         tAcObjID = DirectCast(oDataRow.Item(msEntityFldName), ObjectId)
         If DirectCast(oDataRow.Item(msStatusFldName), Integer) = 0 Then
            oDataRow.Item(msStatusFldName) = enStatus.OneEntry
            Return enStatus.OneEntry
         Else
            ' System.Windows.Forms.MessageBox.Show(enStatus.Status1.ToString(), "09_120")
            Return enStatus.Status1
         End If
      ElseIf oPointView.Count = 0 Then
         If miAttachType = enAttachType.PointToPointObject Then
            '  AcadDocument.WriteMessage("18157:" & oPoint.ToString())  '& ":" & sRowFilter
         End If
         Return enStatus.NotFound
      Else
         Dim tPointObjID As ObjectId
         Dim oDbObj As DBObject
         Dim oEnt As Entity

         AcadDocument.WriteMessage("Multi: " & mdTolerance.ToString() & "; " & oPoint.Coordinates2d() & "; " & mtPoints.Rows.Count.ToString() & ":" & oPointView.Count.ToString()) '& vbCrLf & "- " & sRowFilter
         For Each oDataRowView As DataRowView In oPointView
            tPointObjID = DirectCast(oDataRowView.Item(msEntityFldName), ObjectId)
            oDbObj = AcadTransaction.GetDBObject(tPointObjID, OpenMode.ForRead)
            oEnt = AcadTransaction.GetEntity(tPointObjID, OpenMode.ForRead)
            If oDbObj IsNot Nothing Then
               sHandleMsg = oDbObj.Handle.ToString()
            Else
               sHandleMsg = " oDbObj Is Nothing  "
            End If
            If oEnt IsNot Nothing Then
               sEntityMsg = oEnt.Handle.ToString()
            Else
               sEntityMsg = " oEnt Is Nothing  "

            End If

            AcadDocument.WriteMessage("Handle#: " & sHandleMsg & "; " & tPointObjID.ToString() & "; " & sEntityMsg)
         Next

      
         '  System.Windows.Forms.MessageBox.Show(enStatus.ManyEntries.ToString(), "09_131")
         Return enStatus.ManyEntries
      End If
   End Function

   Public Function GetPointsCount(bStatus As Boolean) As Integer
      If bStatus Then
         Dim sRowFilter As String = msStatusFldName & "=0"
         Dim oPointView As DataView = New DataView(mtPoints, sRowFilter, String.Empty, DataViewRowState.Added)
         Return oPointView.Count
      Else
         Return mtPoints.Rows.Count
      End If

   End Function
   Public Function GePointsEntityCol(bStatus As Boolean) As ObjectIdCollection
      Dim sRowFilter As String
      If bStatus Then
         sRowFilter = msStatusFldName & "=0"


      Else
         sRowFilter = String.Empty

      End If
      Dim oPointView As DataView = New DataView(mtPoints, sRowFilter, String.Empty, DataViewRowState.Added)
      Dim colEntities As ObjectIdCollection = New ObjectIdCollection()
      Dim tAcObjId As ObjectId
      For iIndex As Integer = 0 To oPointView.Count - 1
         tAcObjId = DirectCast(oPointView.Item(iIndex).Item(msEntityFldName), ObjectId)
         colEntities.Add(tAcObjId)
      Next
      Return colEntities
   End Function
   Public Sub PointTest()
      Dim iRes As Integer
      Dim iaSum(2) As Integer
      For Each oDataRow As DataRow In mtPoints.Rows
         iRes = DMCommon.Functions.CIntN(oDataRow.Item(msStatusFldName), -1)
         Select Case iRes
            Case 0
               iaSum(0) += 1
            Case 1
               iaSum(1) += 1
            Case Else
               iaSum(2) += 1
         End Select
      Next
		DMCommon.Functions.DispArray("STAT", iaSum)
	End Sub
   Private Function zzIsIntersectWithPointX(ByVal oAcadEntity As Entity, ByVal oPoint As TPlnPoint, ByVal dTolerance As Double) As Boolean
      zzSetTriangleX(oPoint, dTolerance)
      Dim oPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      Try
         moTriangleX.BoundingBoxIntersectWith(oAcadEntity, Intersect.ExtendThis, oPoints, New IntPtr(0), New IntPtr(0))

         Return oPoints.Count <> 0
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "IstrDrawing - IsIntersectWithPoint_1")
         Return False
      End Try
      '''''''''''''''''''	moTriangleX.Erase()
   End Function


   Private Sub zzSetTriangleX(ByVal oPoint As TPlnPoint, ByVal dTolerance As Double)
      Dim dDX As Double = 0.8660254
      Dim dDY As Double = 0.5
      Dim dX As Double = oPoint.X
      Dim dY As Double = oPoint.Y
      '   dTolerance = 0.5
      Dim tPoint2d1 As Point2d = New Point2d(dX, dY + dTolerance)
      Dim tPoint2d2 As Point2d = New Point2d(dX - dDX * dTolerance, dY - dDY * dTolerance)
      Dim tPoint2d3 As Point2d = New Point2d(dX + dDX * dTolerance, dY - dDY * dTolerance)

      If True OrElse moTriangleX Is Nothing Then

         Try
            moTriangleX = New Polyline(2)
            moTriangleX.AddVertexAt(0, tPoint2d1, 0.0, 0.0, 0.0)
            moTriangleX.AddVertexAt(1, tPoint2d2, 0.0, 0.0, 0.0)
            moTriangleX.AddVertexAt(1, tPoint2d3, 0.0, 0.0, 0.0)

            moTriangleX.Elevation = oPoint.Z
            moTriangleX.Closed = True
            AcadTransaction.AppendEntity(moTriangleX)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrDrawing - zzSetTriangleX")
         End Try
      End If

   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub
End Class


