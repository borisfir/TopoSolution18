Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry

Public Class CleanupCurves
   Private mcolPoints As System.Collections.ObjectModel.Collection(Of CurvePoint) = New System.Collections.ObjectModel.Collection(Of CurvePoint)()
   Private mdicNetPoints As Dictionary(Of Decimal, TPlnPoint)
   Private mcolCurves As ObjectIdCollection
   Private mcolOutOfNetPoints As System.Collections.ObjectModel.Collection(Of TPlnPoint)
   Private moaResPoints As TplnPointArray

   Public Property NetPoints As Dictionary(Of Decimal, TPlnPoint)
      Get
         Return mdicNetPoints
      End Get
      Set(dicValue As Dictionary(Of Decimal, TPlnPoint))
         mdicNetPoints = dicValue
      End Set
   End Property
   Public Property Curves As ObjectIdCollection
      Get
         Return mcolCurves
      End Get
      Set(colValue As ObjectIdCollection)
         mcolCurves = colValue
      End Set
   End Property
   Public Sub RoundByNet() 'ByRef oResPointsArray As TplnPointArray
      Dim oDBobject As DBObject
      Dim oSegment As TplnSegment
      Dim oaVertexArray As TplnPointArray
      Dim oPoint As TPlnPoint
      Dim oNetPoint As TPlnPoint = Nothing
      Dim bUpdate As Boolean
      mcolOutOfNetPoints = New System.Collections.ObjectModel.Collection(Of TPlnPoint)()
      For Each tCurveObjID As ObjectId In mcolCurves
         oDBobject = AcadTransaction.GetDBObject(tCurveObjID, OpenMode.ForWrite)
         oSegment = New TplnSegment(tCurveObjID)
         oSegment.OpenForWrite()
         oaVertexArray = oSegment.Vertices
         bUpdate = False
         For iVertex As Integer = 0 To oaVertexArray.UpperBound
            oPoint = oaVertexArray.Item(iVertex)
            If mdicNetPoints.TryGetValue(oPoint.PointKey, oNetPoint) Then
               oaVertexArray.Item(iVertex) = oNetPoint
               bUpdate = True
            Else
               mcolOutOfNetPoints.Add(oPoint)
            End If
         Next
         If bUpdate Then
            If oSegment.Radius > 0.0 Then
               oSegment.SetRadius(Math.Round(oSegment.Radius, TplnPointKey.RoundDigit))
            End If
            oSegment.Vertices = oaVertexArray
         End If

      Next
      ' System.Windows.Forms.MessageBox.Show(mcolOutOfNetPoints.Count.ToString, "05_129")
   End Sub
   
   Public Sub CheckCloseVertices()
      ' Mark:  Triangle, 6
      Dim oAttachEntities As AttachEntities = New AttachEntities(enAttachType.PointToPoint, 0.5)
      ' Dim oPoint As TPlnPoint
      Dim iStatus As AttachEntities.enStatus
      Dim oTriangleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Triangle)
      moaResPoints = New TplnPointArray()
      oAttachEntities.AddPointCollection(mdicNetPoints.Values, False)
      For Each oPoint As TPlnPoint In mcolOutOfNetPoints
         iStatus = oAttachEntities.GetPointLocation(oPoint)

         If iStatus <> AttachEntities.enStatus.NotFound Then
            oTriangleMarkBlock.MarkPoint(oPoint.AcGePoint, 2S)
            moaResPoints.Add(oPoint)
            DMAcadExt.AcadDocument.WriteMessage("!!+Pt:" & oPoint.ToString() & " ! " & iStatus.ToString())
         End If

      Next

      '   System.Windows.Forms.MessageBox.Show(mdicNetPoints.Count.ToString & vbCrLf & moaResPoints.UpperBound.ToString, "05_132")
   End Sub

   Public Sub CheckNetPointsLines()
      Dim oAttachEntities As AttachEntities = New AttachEntities(enAttachType.PointToCurve, 0.5)
		Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Circle)
		moaResPoints = New TplnPointArray()
      oAttachEntities.AddNetPoints(mdicNetPoints)
      oAttachEntities.AddCurvesIdCol(mcolCurves)
      oAttachEntities.Calculate(False)
		moaResPoints = oAttachEntities.ResPoints

		'   System.Windows.Forms.MessageBox.Show(mdicNetPoints.Count.ToString & vbCrLf & oAttachEntities.ResPoints.UpperBound.ToString & vbCrLf & moaResPoints.UpperBound.ToString, "05_139")
	End Sub
   Public ReadOnly Property ResPoints As TplnPointArray
      Get
         Return moaResPoints
      End Get
   End Property
   Public Sub AddEntities(colAcObjIDs As ObjectIdCollection)
      For Each tAcObjID As ObjectId In colAcObjIDs
         AddEntity(tAcObjID)
      Next

   End Sub
   Public Sub AddEntity(tAcObjID As ObjectId)
      Dim oDBObject As DBObject = AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
      If oDBObject IsNot Nothing Then

         Select Case oDBObject.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadLineName

               Me.zzAddLine(DirectCast(oDBObject, Line))

            Case DMAcadExt.AcadConst.AcadPolylineName

               zzAddPolyline(DirectCast(oDBObject, Polyline))
            Case DMAcadExt.AcadConst.AcadArcName

               Me.zzAddArc(DirectCast(oDBObject, Arc))
            Case DMAcadExt.AcadConst.Acad2dPolylineName

               ''	DMAcadExt.AcadDocument.WriteMessage("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
               zzAddPolyline2d(DirectCast(oDBObject, Polyline2d))
            Case Else
               DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oDBObject.GetType().ToString() & "!")
               Return
         End Select
      End If

   End Sub
   Public ReadOnly Property Points As System.Collections.ObjectModel.Collection(Of CurvePoint)
      Get
         Return mcolPoints
      End Get
   End Property

   Private Sub zzAddLine(oLine As Line)
      mcolPoints.Add(New CurvePoint(oLine.StartPoint, oLine.ObjectId, 0))
      mcolPoints.Add(New CurvePoint(oLine.EndPoint, oLine.ObjectId, 1))


   End Sub
   Private Sub zzAddPolyline(oPolyline As Polyline)
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim bInner As Boolean
      Dim tCurrentPoint As Point2d
      For iIndex As Integer = 0 To iVerticesUB
         If iIndex = 0 AndAlso iIndex = iVerticesUB Then
            bInner = False
         Else
            bInner = True
         End If
         tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
         mcolPoints.Add(New CurvePoint(tCurrentPoint, oPolyline.ObjectId, iIndex))

      Next
   End Sub
   Private Sub zzAddArc(oArc As Arc)
      mcolPoints.Add(New CurvePoint(oArc.StartPoint, oArc.ObjectId, 0))
      mcolPoints.Add(New CurvePoint(oArc.EndPoint, oArc.ObjectId, 1))
   End Sub
   Private Sub zzAddPolyline2d(oPolyline2d As Polyline2d)

   End Sub
   Public Enum enPointStatus
      [Default]
      NeedRounding
   End Enum

   Public Structure CurvePoint
      Dim Point As Point2d
      Dim Curve As ObjectId
      Dim VertexIndex As Integer
      Dim Status As enPointStatus
      Public Sub New(tPoint As Point3d, tCurveObjID As ObjectId, iVertexIndex As Integer)
         Point = New Point2d(tPoint.X, tPoint.Y)
         Curve = tCurveObjID
         VertexIndex = VertexIndex
      End Sub
      Public Sub New(tPoint As Point2d, tCurveObjID As ObjectId, iCurveIndex As Integer)
         Point = tPoint
         Curve = tCurveObjID
         VertexIndex = iCurveIndex
      End Sub
      Public ReadOnly Property PointKey As Decimal
         Get

            Return DMAcadExt.TplnPointKey.CoordToKey(Point.X, Point.Y)
         End Get
      End Property
      Public Sub Round(iDigits As Integer)
         Dim oCurve As DBObject = AcadTransaction.GetDBObject(Curve, OpenMode.ForWrite)
         TplnSegment.RoundPoint(oCurve, VertexIndex, iDigits)
         Point = TPlnPoint.RoundPointTo2d(Point, iDigits)
      End Sub
      Public Sub SetValue(tPoint As Point2d)
         Dim oCurve As Curve = AcadTransaction.GetCurve(Curve, False, OpenMode.ForWrite)

         Select Case oCurve.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadLineName
               '  Dim oLine As TplnLine

            Case DMAcadExt.AcadConst.AcadArcName

            Case DMAcadExt.AcadConst.AcadPolylineName
               Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
               oPolyline.SetPointAt(VertexIndex, tPoint)



				Case DMAcadExt.AcadConst.Acad2dPolylineName

         End Select

      End Sub
   End Structure
End Class
