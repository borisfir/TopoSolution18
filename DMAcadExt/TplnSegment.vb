Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnSegment
   Private moStartPoint As TPlnPoint
   Private moEndPoint As TPlnPoint
   Private moExtents As TPlnBoundingBox
   Private mdRadius As Double
   Private mtAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
   Private mbIsNotEmpty As Boolean
   Private msHandle As String
   Private msRXClassName As String
   Private moPolyline As Polyline
   Private moLine As Line
   Private moArc As Arc
   Private moaVertices As TplnPointArray
   Public Shared Sub RoundPoint(oDBObject As DBObject, iVertexIndex As Integer, iDigits As Integer)
      Dim sRXClassName As String

      sRXClassName = oDBObject.GetRXClass().Name
      Select Case sRXClassName
         Case AcadConst.AcadPolylineName
            Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
            Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

            tPoint = oPolyline.GetPoint2dAt(iVertexIndex)
            tPoint = TPlnPoint.RoundPointTo2d(tPoint, iDigits)
            oPolyline.SetPointAt(iVertexIndex, tPoint)
            oPolyline.UpgradeOpen()
         Case AcadConst.AcadLineName
            Dim oLine As Line = DirectCast(oDBObject, Line)
            Select Case iVertexIndex
               Case 0
                  oLine.StartPoint = TPlnPoint.RoundPoint(oLine.StartPoint, iDigits)
               Case 1
                  oLine.EndPoint = TPlnPoint.RoundPoint(oLine.EndPoint, iDigits)
            End Select
            oLine.UpgradeOpen()


         Case AcadConst.Acad2dPolylineName
         Case AcadConst.AcadArcName

            Dim oArc As Arc = DirectCast(oDBObject, Arc)

            Dim oTplnArc As TplnArc = New TplnArc(oArc, True)
            oTplnArc.RoundPoint(iVertexIndex, iDigits)
            ' oArc.StartAngle = oTplnArc.RoundedStartAngle(iDigits)
            '  oArc.EndAngle = oTplnArc.RoundedEndAngle(iDigits)
            '   AcadDocument.WriteMessage("-@@0 " & oTplnArc.Center.ToString())
            oArc.Center = TPlnPoint.Point2dTo3d(oTplnArc.Center)
            oArc.Radius = oTplnArc.Radius

            oArc.StartAngle = oTplnArc.StartAngle
            oArc.EndAngle = oTplnArc.EndAngle

      End Select
   End Sub

   Public Shared Function Round(oDBObject As DBObject, bFix As Boolean, iDigits As Integer) As TplnPointArray
      Dim sRXClassName As String
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

      Dim tRoundedPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim tRoundedPoint3d As Autodesk.AutoCAD.Geometry.Point3d

      Dim oaResPoints As TplnPointArray = New TplnPointArray()
      Dim tTolerance As Autodesk.AutoCAD.Geometry.Tolerance = New Autodesk.AutoCAD.Geometry.Tolerance(0.00000001, 0.00000001)
      sRXClassName = oDBObject.GetRXClass().Name
      Select Case sRXClassName
         Case AcadConst.AcadPolylineName
            Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)

            For iVertexIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
               tPoint = oPolyline.GetPoint2dAt(iVertexIndex)
               tRoundedPoint = TPlnPoint.RoundPointTo2d(tPoint, iDigits)
               If Not tPoint.IsEqualTo(tRoundedPoint, tTolerance) Then
                  oaResPoints.Add(tRoundedPoint)
               End If
               If bFix Then
                  oPolyline.SetPointAt(iVertexIndex, tRoundedPoint)
               End If

            Next
         Case AcadConst.AcadLineName
            Dim oLine As Line = DirectCast(oDBObject, Line)
            tPoint3d = oLine.StartPoint
            tRoundedPoint3d = TPlnPoint.RoundPoint(tPoint3d, iDigits)
            If Not tPoint3d.IsEqualTo(tRoundedPoint3d, tTolerance) Then
               oaResPoints.Add(tRoundedPoint)
            End If
            If bFix Then
               oLine.StartPoint = tRoundedPoint3d
            End If


            tPoint3d = oLine.EndPoint
            tRoundedPoint3d = TPlnPoint.RoundPoint(tPoint3d, iDigits)
            If Not tPoint3d.IsEqualTo(tRoundedPoint3d, tTolerance) Then
               oaResPoints.Add(tRoundedPoint3d)
            End If
            If bFix Then
               oLine.EndPoint = tRoundedPoint3d
            End If


         Case AcadConst.Acad2dPolylineName
         Case AcadConst.AcadArcName

            Dim oArc As Arc = DirectCast(oDBObject, Arc)
            Dim oResStartPoint As TPlnPoint = Nothing
            Dim oResEndPoint As TPlnPoint = Nothing
            Dim oTplnArc As TplnArc = New TplnArc(oArc, True)
            oTplnArc.Round(iDigits, tTolerance, oResStartPoint, oResEndPoint)
            ' oArc.StartAngle = oTplnArc.RoundedStartAngle(iDigits)
            '  oArc.EndAngle = oTplnArc.RoundedEndAngle(iDigits)
            '   AcadDocument.WriteMessage("-@@0 " & oTplnArc.Center.ToString())
            '    AcadDocument.WriteMessage("Center=" & TPlnPoint.Point2dTo3d(oTplnArc.Center).ToString() & "; Radius=" & oTplnArc.Radius.ToString())
            '   AcadDocument.WriteMessage("StartAngle=" & oTplnArc.StartAngle.ToString() & "; EndAngle=" & oTplnArc.EndAngle.ToString())
            Try
               If bFix Then
                  oArc.Center = TPlnPoint.Point2dTo3d(oTplnArc.Center)
                  oArc.Radius = oTplnArc.Radius

                  oArc.StartAngle = oTplnArc.StartAngle
                  oArc.EndAngle = oTplnArc.EndAngle
               End If
              
            Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & "Digit=" & iDigits.ToString(), "e1416")
					AcadDocument.WriteMessage("-------------Handle=" & oArc.Handle.ToString())
               AcadDocument.WriteMessage("Center=" & TPlnPoint.Point2dTo3d(oTplnArc.Center).ToString() & "; Radius=" & oTplnArc.Radius.ToString())
               AcadDocument.WriteMessage("StartAngle=" & oTplnArc.StartAngle.ToString() & "; EndAngle=" & oTplnArc.EndAngle.ToString())


            End Try
            If oResStartPoint IsNot Nothing Then
               oaResPoints.Add(oResStartPoint)
            End If

            If oResEndPoint IsNot Nothing Then
               oaResPoints.Add(oResEndPoint)
            End If


            '    AcadDocument.WriteMessage("Center=" & TPlnPoint.Point2dTo3d(oTplnArc.Center).ToString() & "; Radius=" & oTplnArc.Radius.ToString())
            '    AcadDocument.WriteMessage("StartAngle=" & oTplnArc.StartAngle.ToString() & "; EndAngle=" & oTplnArc.EndAngle.ToString())

      End Select
      ' System.Windows.Forms.MessageBox.Show(oaResPoints.UpperBound.ToString(), "05_260")
      Return oaResPoints
   End Function
   Public Sub New(tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
      mtAcObjID = tAcObjID
   End Sub
   Public Sub OpenForWrite()
      Dim oDBObject As DBObject = AcadTransaction.GetDBObject(mtAcObjID, OpenMode.ForWrite)
      msRXClassName = oDBObject.GetRXClass().Name
      Select Case msRXClassName
         Case AcadConst.AcadPolylineName
            moPolyline = DirectCast(oDBObject, Polyline)
            moStartPoint = New TPlnPoint(moPolyline.StartPoint)
            moEndPoint = New TPlnPoint(moPolyline.EndPoint)
            msHandle = moPolyline.Handle.ToString()
            moExtents = New TPlnBoundingBox(moPolyline.GeometricExtents)

            moaVertices = New TplnPointArray(moPolyline)
            mbIsNotEmpty = True
         Case AcadConst.AcadLineName
            moLine = DirectCast(oDBObject, Line)
            moStartPoint = New TPlnPoint(moLine.StartPoint)
            moEndPoint = New TPlnPoint(moLine.EndPoint)
            msHandle = moLine.Handle.ToString()
            moExtents = New TPlnBoundingBox(moLine.GeometricExtents)
            moaVertices = New TplnPointArray(moLine)
            mbIsNotEmpty = True
         Case AcadConst.Acad2dPolylineName
         Case AcadConst.AcadArcName
            moArc = DirectCast(oDBObject, Arc)
            moStartPoint = New TPlnPoint(moArc.StartPoint)
            moEndPoint = New TPlnPoint(moArc.EndPoint)
            msHandle = moArc.Handle.ToString()
            moExtents = New TPlnBoundingBox(moArc.GeometricExtents)
            mdRadius = moArc.Radius
            moaVertices = New TplnPointArray(moArc)
            mbIsNotEmpty = True
      End Select
   End Sub
   Public Sub New(oDBObject As DBObject)
      Dim sRXClassName As String
      mtAcObjID = oDBObject.ObjectId
      sRXClassName = oDBObject.GetRXClass().Name
      Select Case sRXClassName
         Case AcadConst.AcadPolylineName
            Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
            moStartPoint = New TPlnPoint(oPolyline.StartPoint)
            moEndPoint = New TPlnPoint(oPolyline.EndPoint)
            msHandle = oPolyline.Handle.ToString()
            moExtents = New TPlnBoundingBox(oPolyline.GeometricExtents)
            mbIsNotEmpty = True

         Case AcadConst.AcadLineName
            Dim oLine As Line = DirectCast(oDBObject, Line)
            moStartPoint = New TPlnPoint(oLine.StartPoint)
            moEndPoint = New TPlnPoint(oLine.EndPoint)
            msHandle = oLine.Handle.ToString()
            moExtents = New TPlnBoundingBox(oLine.GeometricExtents)
            mbIsNotEmpty = True
         Case AcadConst.Acad2dPolylineName
         Case AcadConst.AcadArcName
            Dim oArc As Arc = DirectCast(oDBObject, Arc)
            moStartPoint = New TPlnPoint(oArc.StartPoint)
            moEndPoint = New TPlnPoint(oArc.EndPoint)
            msHandle = oArc.Handle.ToString()
            moExtents = New TPlnBoundingBox(oArc.GeometricExtents)
            mdRadius = oArc.Radius
            mbIsNotEmpty = True
      End Select
   End Sub
   Public ReadOnly Property Radius As Double
      Get
         Return mdRadius
      End Get
   End Property
   Public Sub SetRadius(dRadius As Double)
      mdRadius = dRadius
   End Sub
   Public Property Vertices As TplnPointArray
      Get
         Return moaVertices
      End Get
      Set(oaValue As TplnPointArray)
         moaVertices = oaValue
         zzUpdateVertices()
      End Set
   End Property
   Public ReadOnly Property AcObjID As ObjectId
      Get
         Return mtAcObjID
      End Get
   End Property
   Public ReadOnly Property UD_Label As String
      Get
         If mdRadius > 0.0 Then
            Return "R=" & FormatNumber(mdRadius, 2)
         Else
            Dim dDist As Double = moStartPoint.Distance(moEndPoint)
            If dDist = 0 Then
               Return moStartPoint.Coordinates & ";" & moEndPoint.Coordinates & " H=" & msHandle
            Else
               Return FormatNumber(dDist, 2)
            End If

         End If

      End Get
   End Property
   Public Property StartPoint As TPlnPoint
      Get
         Return moStartPoint
      End Get
      Set(oValue As TPlnPoint)
         moStartPoint = oValue
      End Set
   End Property

   Public Property EndPoint As TPlnPoint
      Get
         Return moEndPoint
      End Get
      Set(oValue As TPlnPoint)
         moEndPoint = oValue
      End Set
   End Property
   Public Property Extents As TPlnBoundingBox
      Get
         Return moExtents
      End Get
      Set(oValue As TPlnBoundingBox)
         moExtents = oValue
      End Set
   End Property
   Public ReadOnly Property IsNotEmpty As Boolean
      Get
         Return mbIsNotEmpty
      End Get
   End Property
   Private Sub zzUpdateVertices()
      Select Case msRXClassName
         Case AcadConst.AcadLineName
            moLine.StartPoint = moaVertices.Item(0).AcGePoint3d
            moLine.EndPoint = moaVertices.Item(1).AcGePoint3d
         Case AcadConst.AcadArcName
            Dim oTplnArc As TplnArc = New TplnArc(moArc)


            oTplnArc.SetValue(moaVertices.Item(0).AcGePoint, moaVertices.Item(1).AcGePoint, mdRadius)
           
            moArc.Center = TPlnPoint.Point2dTo3d(oTplnArc.Center)
            moArc.Radius = oTplnArc.Radius

            moArc.StartAngle = oTplnArc.StartAngle
            moArc.EndAngle = oTplnArc.EndAngle




         Case AcadConst.AcadPolylineName
            For iIndex As Integer = 0 To moaVertices.UpperBound
               moPolyline.SetPointAt(iIndex, moaVertices.Item(iIndex).AcGePoint)
            Next
      End Select
   End Sub
End Class
