Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.Gis.Map.Utilities
Public Class GeoUtilites
   Public Shared Function PgonTopologyToLoop(ByVal sTopoName As String) As HatchLoop
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim sName As System.String = sTopoName
      Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
      Dim oPolygon As Polygon
      Dim colRings As RingCollection
      Try
         Dim oTopoModel As TopologyModel
         If oTopos.Exists(sName) Then
            oTopoModel = oTopos.Item(sName)
            Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
            oPolygon = colPolygons.Item(0)
            colRings = oPolygon.GetBoundary()
            System.Windows.Forms.MessageBox.Show("", "522")
            oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "", "", True)
            oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
            Try
               oTopoModel.ShowGeometry(1)
            Catch oMapEx1 As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx1.ErrorCode, False, "TopoCreator - ShowTopology_1")
            End Try
            oDocLock.Dispose()
            oTopoModel.Close()
            Return Nothing
            '	zzCommandLine()
         Else
            Return Nothing
         End If
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - ShowTopology")
         Return Nothing
      End Try

   End Function
   Public Shared Function PolylineToCurve(ByVal colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Curve2d()
      Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
      Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      Dim oEntity As Entity
      Dim oPolyline As Polyline
      Dim oLine As Line
      Dim sRXClassName As String
      Dim iCurrentVertexUB As Integer = -1
      Dim oCurve2ds() As Curve2d = Nothing
      Dim iVert As Integer
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      oTransaction = oTransactionManager.StartTransaction()
      System.Windows.Forms.MessageBox.Show(colLinks.Count.ToString(), "colLinks.Count")
      For Each oObjectID As ObjectId In colLinks
         oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)


         oEntity = DirectCast(oDBObject, Entity)
         sRXClassName = oEntity.GetRXClass().Name

         Select Case sRXClassName
            Case DMAcadExt.AcadConst.AcadPolylineName
               oPolyline = DirectCast(oEntity, Polyline)
               iVert = oPolyline.NumberOfVertices

               If Not oPolyline.Closed Then
                  iVert -= 1
               End If
               ReDim oCurve2ds(iCurrentVertexUB + iVert)

               For iIndex As Integer = 0 To iVert - 1
                  oCurve2ds(iCurrentVertexUB + iIndex + 1) = oPolyline.GetLineSegment2dAt(iIndex)
               Next
               '   oPolyline.GetArcSegment2dAt()

               iCurrentVertexUB += iVert

               '  oPolyline.NumberOfVertices
            Case DMAcadExt.AcadConst.AcadPolylineName
               oPolyline = DirectCast(oEntity, Polyline)
               iVert = oPolyline.NumberOfVertices
               System.Windows.Forms.MessageBox.Show(iVert.ToString(), "iVert")
               If Not oPolyline.Closed Then
                  iVert -= 1
               End If
               ReDim oCurve2ds(iCurrentVertexUB + iVert)
               For iIndex As Integer = 0 To iVert - 1
                  oCurve2ds(iCurrentVertexUB + iIndex + 1) = oPolyline.GetLineSegment2dAt(iIndex)
               Next
               '   oPolyline.GetArcSegment2dAt()

               iCurrentVertexUB += iVert
               oPolyline.GetOffsetCurves(1)
               '  oPolyline.NumberOfVertices
            Case Common.AcadLWPolylineName
            Case Common.AcadLineName
               oLine = DirectCast(oEntity, Line)
            Case Else

         End Select

      Next

      Try
         oTransaction.Commit()
         oTransaction = Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - CreateTopology_17")
      Finally
         If Not oTransaction Is Nothing Then
            oTransaction.Abort()
            oTransaction = Nothing
         End If
      End Try
      Return oCurve2ds

   End Function
   Public Shared Function PolylineToVertices(ByVal colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As BulgeVertex()

      Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
      Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      Dim oEntity As Entity
      Dim oPolyline As Polyline
      Dim oLine As Line
      Dim sRXClassName As String
      Dim iCurrentVertexUB As Integer = -1
      Dim oaBulgeVertices() As BulgeVertex = Nothing
      Dim iVert As Integer
      Dim dBulge As Double
      Dim tPoint As Point2d
      Dim oBulgeVertex As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      oTransaction = oTransactionManager.StartTransaction()
      ''  System.Windows.Forms.MessageBox.Show(colLinks.Count.ToString(), "colLinks.Count")
      For Each oObjectID As ObjectId In colLinks
         oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)


         oEntity = DirectCast(oDBObject, Entity)
         sRXClassName = oEntity.GetRXClass().Name

         Select Case sRXClassName
            Case DMAcadExt.AcadConst.AcadPolylineName
               oPolyline = DirectCast(oEntity, Polyline)
               iVert = oPolyline.NumberOfVertices
               ''  System.Windows.Forms.MessageBox.Show(iVert.ToString(), "iVert")

               ReDim oaBulgeVertices(iCurrentVertexUB + iVert)

               For iIndex As Integer = 0 To iVert - 1
                  dBulge = oPolyline.GetBulgeAt(iIndex)

                  tPoint = oPolyline.GetPoint2dAt(iIndex)
                  oBulgeVertex = New BulgeVertex(tPoint, dBulge)
                  oaBulgeVertices(iCurrentVertexUB + iIndex + 1) = oBulgeVertex
               Next
               '   oPolyline.GetArcSegment2dAt()
               '   System.Windows.Forms.MessageBox.Show(oaBulgeVertices.GetUpperBound(0).ToString(), "BulgeVertices.GetUpperBound")
               iCurrentVertexUB += iVert
               oPolyline.GetOffsetCurves(1)
               '  oPolyline.NumberOfVertices

            Case Common.AcadLWPolylineName
            Case Common.AcadLineName
               oLine = DirectCast(oEntity, Line)
            Case Else

         End Select

      Next

      Try
         oTransaction.Commit()
         oTransaction = Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator-CreateTopology,17")
      Finally
         If Not oTransaction Is Nothing Then
            oTransaction.Abort()
            oTransaction = Nothing
         End If
      End Try
      System.Windows.Forms.MessageBox.Show(oaBulgeVertices.GetUpperBound(0).ToString(), "!!!!!BulgeVertices.GetUpperBound")
      Return oaBulgeVertices

   End Function
   Public Shared Sub DrawZebraBox(ByVal oZebra As DMAcadExt.ColorZebra)

   End Sub
   Public Shared Function RingToCurve(ByVal oRing As Ring) As HatchLoop
      Dim iCurrentVertexUB As Integer = -1
      Dim oBulgeVertexArray As BulgeVertexArray
      oBulgeVertexArray = New BulgeVertexArray(oRing)
      Dim iHatchLoopTypes As HatchLoopTypes
      If oRing.IsExterior Then
         iHatchLoopTypes = HatchLoopTypes.External
      Else
         iHatchLoopTypes = HatchLoopTypes.Default
      End If
      Return oBulgeVertexArray.CreateHatchLoop(iHatchLoopTypes)
   End Function
   Public Shared Function RingToLoop(ByVal oRing As Ring) As HatchLoop
      Dim iCurrentVertexUB As Integer = -1
      Dim oBulgeVertexArray As BulgeVertexArray
      oBulgeVertexArray = New BulgeVertexArray(oRing)
      Dim iHatchLoopTypes As HatchLoopTypes
      If oRing.IsExterior Then
         iHatchLoopTypes = HatchLoopTypes.External
      Else
         iHatchLoopTypes = HatchLoopTypes.Default
      End If
      Return oBulgeVertexArray.CreateHatchLoop(iHatchLoopTypes)
   End Function
   Public Shared Function BulgeVertexToString(ByVal oBulgeVertex As BulgeVertex) As String
      If oBulgeVertex Is Nothing Then
         Return " !BulgeVertex Is Nothing "
      Else
         Return DMAcadExt.TPlnPoint.DispPoint(oBulgeVertex.Vertex) & " B=" & CStr(oBulgeVertex.Bulge)
      End If


   End Function
   Public Shared Sub InsertMarkBlock(oResList As List(Of InitInsertData), sMarkBlockName As String, sMarkBlockFolder As String)
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sMarkBlockName, sMarkBlockFolder)
      Dim tBlockRefData As DMAcadExt.BlockRefData
      Dim iParity As Integer = 0
      oAcadBlock.OpenForRight()
      For Each tInitInsertData As InitInsertData In oResList
         '  DMAcadExt.AcadTransaction.InsertPoint(tInitInsertData.Position)
         tBlockRefData = New DMAcadExt.BlockRefData()

         tBlockRefData.Position = tInitInsertData.Position
         'If iParity = 0 Then
         '   tBlockRefData.ColorIndex = 2S
         'Else
         '   tBlockRefData.ColorIndex = 3S
         'End If

         '    tBlockRefData.Layer = msTazarMapBlockLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor)
         tBlockRefData.Rotation = tInitInsertData.Rotation + Convert.ToDouble(iParity) * Math.PI
         oAcadBlock.InsertRefNewNew(tBlockRefData)
         iParity = (iParity + 1) Mod 2

      Next
   End Sub

   Public Shared Sub InsertMarkBlock(oResList As List(Of InitInsertData), dicBlocks As IDictionary(Of String, DMAcadExt.AcadBlock))
      Dim oAcadBlock As DMAcadExt.AcadBlock = Nothing
      Dim tBlockRefData As DMAcadExt.BlockRefData
      Dim iParity As Integer = 0

		For Each tInitInsertData As InitInsertData In oResList
			'  DMAcadExt.AcadTransaction.InsertPoint(tInitInsertData.Position)
			If tInitInsertData.Layer Is Nothing OrElse tInitInsertData.Layer <> "C1650" Then
				DMAcadExt.AcadDocument.WriteDebugMessageN("#275 Layer", tInitInsertData.Layer)
				If tInitInsertData.Layer IsNot Nothing Then
					DMAcadExt.AcadDocument.WriteDebugMessageN("#276 Layer", tInitInsertData.Layer, dicBlocks.ContainsKey(tInitInsertData.Layer))
				End If
			End If

			If tInitInsertData.Layer IsNot Nothing AndAlso dicBlocks.TryGetValue(tInitInsertData.Layer, oAcadBlock) Then
				DMAcadExt.AcadDocument.WriteDebugMessageN("oResList", tInitInsertData.Layer, oAcadBlock.BlockName)
				tBlockRefData = New DMAcadExt.BlockRefData()

				tBlockRefData.Position = tInitInsertData.Position
				tBlockRefData.Layer = tInitInsertData.Layer
				tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor)
				tBlockRefData.Rotation = tInitInsertData.Rotation + Convert.ToDouble(iParity) * Math.PI


				oAcadBlock.InsertRefNewNew(tBlockRefData)
				iParity = (iParity + 1) Mod 2
			End If




		Next
   End Sub

   Public Shared Function DisplayObject(ByVal oLine2d As Line2d) As String
      Dim sStart, sEnd As String
      If oLine2d Is Nothing Then
         sStart = "Line2d Is Nothing"
         sEnd = String.Empty
      Else
         If oLine2d.HasStartPoint() Then
            sStart = DMAcadExt.TPlnPoint.DispPoint(oLine2d.StartPoint)
         Else
            sStart = "Not Start"
         End If
         If oLine2d.HasEndPoint() Then
            sEnd = DMAcadExt.TPlnPoint.DispPoint(oLine2d.EndPoint)
         Else
            sEnd = "Not End"
         End If
      End If


      Return sStart & "; " & sEnd
   End Function

   Public Shared Function CloseCompositeCurve(ByRef oCompositeCurve2d As CompositeCurve2d) As CompositeCurve2d
      Try
         Dim oCurves() As Curve2d = oCompositeCurve2d.GetCurves()
         Dim iUB As Integer = oCurves.GetUpperBound(0)
         Dim LineSegment2d As Curve2d = New LineSegment2d(oCurves(iUB).EndPoint, oCurves(0).StartPoint)
         iUB += 1
         ReDim Preserve oCurves(iUB)
         oCurves(iUB) = LineSegment2d
         Return New CompositeCurve2d(oCurves)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "CloseCompositeCurve 12_760")
         Return Nothing
      End Try
   End Function
   Public Shared Function CurvesToLoop(ByVal oaCurves() As Curve2d) As HatchLoop
      Dim iCurrentVertexUB As Integer = -1
      Dim oBulgeVertexArray As BulgeVertexArray

      If oaCurves Is Nothing Then
         MessageBox.Show("oCurves Is Nothing ", "12_4269")
         Return Nothing
      Else
         oBulgeVertexArray = New BulgeVertexArray(oaCurves)
         If oBulgeVertexArray IsNot Nothing Then
            Dim iHatchLoopTypes As HatchLoopTypes = HatchLoopTypes.Default
            Return oBulgeVertexArray.CreateHatchLoop(iHatchLoopTypes)
         Else
            Return Nothing
         End If
      End If
   End Function
   Public Shared Sub HatchToPolyline(oEnt As DBObject)
      Dim oHatch As Hatch
      Dim oHatchLoop As HatchLoop
      Dim oPolyline As Polyline
      Try

         If oEnt.GetRXClass.Name = DMAcadExt.AcadConst.AcadHatchName Then
            oHatch = DirectCast(oEnt, Hatch)
            If oHatch.NumberOfLoops > 1 Then
               DMAcadExt.AcadDocument.WriteMessage("Loops=" & CStr(oHatch.NumberOfLoops))
            End If
            For iIndex As Integer = 0 To oHatch.NumberOfLoops - 1
               oHatchLoop = oHatch.GetLoopAt(iIndex)
               If oHatchLoop.IsPolyline Then
                  DMAcadExt.AcadDocument.WriteMessage("PL=" & CStr(oHatch.Handle.ToString()))
               End If
               oPolyline = GeoUtilites.HatchLoopToPolyline(oHatchLoop)
               If oPolyline IsNot Nothing Then
                  oPolyline.Layer = oHatch.Layer
                  oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
                  DMAcadExt.AcadTransaction.AppendEntity(oPolyline, False)
                  ''''''''DMAcadExt.AcadDocument.WriteMessage(oPolyline.Handle.ToString())
               Else
                  '''''''''''''''''''MessageBox.Show("Polyline IsNot Nothing ")
               End If
            Next
         End If
      Catch oEx As Exception

      End Try
   End Sub

   Public Shared Function HatchLoopToPolyline(ByVal oHatchLoop As HatchLoop) As Polyline
      Dim iCurrentVertexUB As Integer = -1
      Dim oBulgeVertexArray As BulgeVertexArray = Nothing
      Dim colBulgeVertex As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection
      Dim colCurves As Autodesk.AutoCAD.Geometry.Curve2dCollection
      '	Dim oCurve As Autodesk.AutoCAD.Geometry.Curve2d
      If oHatchLoop.IsPolyline Then
         colBulgeVertex = oHatchLoop.Polyline
         If colBulgeVertex IsNot Nothing Then
            oBulgeVertexArray = New BulgeVertexArray(colBulgeVertex)
         Else
            DMAcadExt.AcadDocument.WriteMessage("HatchLoop.Polyline was not found")
         End If
      Else
         colCurves = oHatchLoop.Curves
         If colCurves IsNot Nothing Then
            Dim oaCurves(colCurves.Count - 1) As Autodesk.AutoCAD.Geometry.Curve2d
            For iIndex As Integer = 0 To colCurves.Count - 1
               oaCurves(iIndex) = colCurves.Item(iIndex)
            Next
            '	DMAcadExt.AcadDocument.WriteMessage(CStr(oaCurves.GetUpperBound(0)))
            oBulgeVertexArray = New BulgeVertexArray(oaCurves)
         Else
            DMAcadExt.AcadDocument.WriteMessage("HatchLoop.Curves was not found")
         End If
      End If

      If oBulgeVertexArray IsNot Nothing AndAlso Not oBulgeVertexArray.HasCircularArc Then
         Dim iHatchLoopTypes As HatchLoopTypes = HatchLoopTypes.Default
         Return oBulgeVertexArray.CreatePolyline()
      Else
         Return Nothing
      End If

   End Function
   Public Shared Function RingToCurves(ByVal oRing As Ring) As Curve2d()
      Dim iCurrentVertexUB As Integer = -1


      Dim oHalfEdge As HalfEdge
      Dim oFullEdge As FullEdge
      Dim oCurveArray As CurveArray
      Dim iStartID As Integer

      oHalfEdge = oRing.StartEdge
      oFullEdge = oHalfEdge.FullEdge
      iStartID = oFullEdge.ID
      System.Windows.Forms.MessageBox.Show(iStartID.ToString(), "601")
      oCurveArray = zzObjectIDToCurveArray(oFullEdge.Entity)
      System.Windows.Forms.MessageBox.Show(oCurveArray.UB.ToString(), "610")
      Do
         Try
            System.Windows.Forms.MessageBox.Show("", "611")
            oHalfEdge = oHalfEdge.GetNextEdge(True)

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "OK:RingToCurves")
            oHalfEdge = Nothing
         End Try


         If oHalfEdge Is Nothing Then
            System.Windows.Forms.MessageBox.Show(oCurveArray.UB.ToString(), "622_&")
            Exit Do
         End If

         oFullEdge = oHalfEdge.FullEdge
         If oFullEdge.ID = iStartID Then
            Exit Do
         End If

         oCurveArray.AddArray(zzObjectIDToCurveArray(oFullEdge.Entity))

      Loop

      Return oCurveArray.GetCurveArray

   End Function
   Public Shared Function RingToComposit(ByVal oRing As Ring) As CompositeCurve2d
      Dim iCurrentVertexUB As Integer = -1
		Dim oHalfEdge As HalfEdge
		Dim oFullEdge As FullEdge
      Dim oCurveArray As CurveArray
      Dim iStartID As Integer

      oHalfEdge = oRing.StartEdge
      oFullEdge = oHalfEdge.FullEdge
      iStartID = oFullEdge.ID
      System.Windows.Forms.MessageBox.Show(iStartID.ToString(), "601")
      oCurveArray = zzObjectIDToCurveArray(oFullEdge.Entity)
      System.Windows.Forms.MessageBox.Show(oCurveArray.UB.ToString(), "610")
      Do
         Try
            System.Windows.Forms.MessageBox.Show("", "611")
            oHalfEdge = oHalfEdge.GetNextEdge(True)

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "OK:RingToComposit")
            oHalfEdge = Nothing
         End Try


         If oHalfEdge Is Nothing Then
            System.Windows.Forms.MessageBox.Show(oCurveArray.UB.ToString(), "622u")
            Exit Do
         End If

         oFullEdge = oHalfEdge.FullEdge
         If oFullEdge.ID = iStartID Then
            Exit Do
         End If

         oCurveArray.AddArray(zzObjectIDToCurveArray(oFullEdge.Entity))

      Loop

      Return New CompositeCurve2d(oCurveArray.GetCurveArray)

   End Function
   Public Shared Function VerticesToCompositeCurve(ByVal oaBulgeVertices() As BulgeVertex) As CompositeCurve2d
      Dim oBulgeVertexArray As BulgeVertexArray = New BulgeVertexArray(oaBulgeVertices)
      Return oBulgeVertexArray.GetCompositeCurve(True)
   End Function
   Public Shared Sub CurvesInfo(ByVal oaCurves() As Curve2d, ByVal sTag As String)
      Dim iUB As Integer = oaCurves.GetUpperBound(0)
      Dim oPointStart, oPointEnd As DMAcadExt.TPlnPoint
      DMAcadExt.AcadDocument.WriteMessage(sTag & ":  Dim=" & CStr(iUB))
      For iIndex As Integer = 0 To iUB
         oPointStart = New DMAcadExt.TPlnPoint(oaCurves(iIndex).StartPoint)
         oPointEnd = New DMAcadExt.TPlnPoint(oaCurves(iIndex).EndPoint)
         DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & ": " & oPointStart.Coordinates & "; " & oPointEnd.Coordinates)
      Next
   End Sub
   Public Shared Function BulgeVertexInfo(ByVal oBulgeVertex As BulgeVertex) As String
      Return DMAcadExt.TPlnPoint.DispPoint(oBulgeVertex.Vertex) & " Bulge=" & CStr(oBulgeVertex.Bulge)
   End Function

   Private Shared Function zzObjectIDToBulgeVertexArray(ByVal oObjectID As ObjectId) As BulgeVertexArray
      Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
      Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      Dim oEntity As Entity
      Dim oPolyline As Polyline
      Dim oLine As Line
      Dim sRXClassName As String
      Dim iCurrentVertexUB As Integer = -1
      Dim oCurve2ds() As Curve2d = Nothing
      Dim iVert As Integer
      Dim sTest As String = String.Empty
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      oTransaction = oTransactionManager.StartTransaction()

      oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)


      oEntity = DirectCast(oDBObject, Entity)
      sRXClassName = oEntity.GetRXClass().Name
      Dim oBulgeVertexArray As BulgeVertexArray
      Dim dBulge As Double = 0
      Dim oPoint As Point2d
      Dim iAddClosed As Integer = 0
      Select Case sRXClassName
         Case DMAcadExt.AcadConst.AcadPolylineName
            oPolyline = DirectCast(oEntity, Polyline)
            iVert = oPolyline.NumberOfVertices
            If oPolyline.Closed Then
               iAddClosed = 1
            End If
            System.Windows.Forms.MessageBox.Show(iVert.ToString(), "!!!++iVert")
            System.Windows.Forms.MessageBox.Show(oPolyline.Closed.ToString(), "Closed???")
            oBulgeVertexArray = New BulgeVertexArray(iVert + iAddClosed)

            For iIndex As Integer = 0 To iVert - 1
               oPoint = oPolyline.GetPoint2dAt(iIndex)
               sTest &= ";" & CStr(oPoint.X) & "," & CStr(oPoint.Y)
               dBulge = oPolyline.GetBulgeAt(iIndex)
               oBulgeVertexArray.AddBulgeVertex(oPoint, dBulge)
            Next
            If iAddClosed = 1 Then
               oPoint = oPolyline.GetPoint2dAt(0)
               sTest &= ";" & CStr(oPoint.X) & "," & CStr(oPoint.Y)
               dBulge = oPolyline.GetBulgeAt(0)
               oBulgeVertexArray.AddBulgeVertex(oPoint, dBulge)
            End If
            '  oPolyline.NumberOfVertices

         Case Common.AcadLWPolylineName
            System.Windows.Forms.MessageBox.Show(oEntity.ToString(), "AcadLWPolylineName 12_329")
            oBulgeVertexArray = Nothing
         Case Common.AcadLineName
            oLine = DirectCast(oEntity, Line)

            oBulgeVertexArray = New BulgeVertexArray(2)
            oBulgeVertexArray.AddBulgeVertex(oLine.StartPoint)
            sTest &= ";" & CStr(oLine.StartPoint.X) & "," & CStr(oLine.StartPoint.Y)
            oBulgeVertexArray.AddBulgeVertex(oLine.EndPoint)
            sTest &= ";" & CStr(oPoint.X) & "," & CStr(oPoint.X)
         Case Else
            System.Windows.Forms.MessageBox.Show(iVert.ToString(), "Type is new 12_341!!!")
            oBulgeVertexArray = Nothing
      End Select

		Try
         oTransaction.Commit()
         oTransaction = Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator-CreateTopology,17")
      Finally
         If Not oTransaction Is Nothing Then
            oTransaction.Abort()
            oTransaction = Nothing
         End If

      End Try
      DMAcadExt.AcadDocument.WriteMessage("Points... " & sTest)
      Return oBulgeVertexArray

   End Function
   Private Shared Function zzObjectIDToCurveArray(ByVal oObjectID As ObjectId) As CurveArray
      Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
      Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      Dim oEntity As Entity
      Dim oPolyline As Polyline
      Dim oLine As Line
      Dim sRXClassName As String
      Dim iCurrentVertexUB As Integer = -1
      Dim oCurve2ds() As Curve2d = Nothing
      Dim iVert As Integer
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      oTransaction = oTransactionManager.StartTransaction()

      oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)


      oEntity = DirectCast(oDBObject, Entity)
      sRXClassName = oEntity.GetRXClass().Name
      Dim oCurveArray As CurveArray
      Dim dBulge As Double = 0
      Dim oPoint As Point2d
      ' Dim iAddClosed As Integer = 0
      Select Case sRXClassName
         Case DMAcadExt.AcadConst.AcadPolylineName
            oPolyline = DirectCast(oEntity, Polyline)

            iVert = oPolyline.NumberOfVertices - 1
            If oPolyline.Closed Then
               iVert += 1
            End If
            System.Windows.Forms.MessageBox.Show(iVert.ToString(), "!!!++iVert")
            System.Windows.Forms.MessageBox.Show(oPolyline.Closed.ToString(), "Closed???")
            oCurveArray = New CurveArray(iVert)


            For iIndex As Integer = 0 To iVert - 1
               oPoint = oPolyline.GetPoint2dAt(iIndex)
               dBulge = oPolyline.GetBulgeAt(iIndex)
               oCurveArray.AddCurve(oPolyline.GetLineSegment2dAt(iIndex))
            Next


            '  oPolyline.NumberOfVertices

         Case Common.AcadLWPolylineName
            oCurveArray = Nothing
         Case Common.AcadLineName
            oLine = DirectCast(oEntity, Line)
            oCurveArray = New CurveArray(0)
            oCurveArray.AddCurve(oLine.StartPoint, oLine.EndPoint)
         Case Else
            oCurveArray = Nothing
      End Select



      Try
         oTransaction.Commit()
         oTransaction = Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator-CreateTopology,17")
      Finally
         If Not oTransaction Is Nothing Then
            oTransaction.Abort()
            oTransaction = Nothing
         End If
      End Try
      Return oCurveArray
   End Function
   Public Class BulgeVertexArray
      Const mdTolerance As Double = 0.01
      Private moaBulgeVertex() As BulgeVertex
      Private miUB As Integer = -1
      Private miCurrentIndex As Integer = 0
      Private mbSourceExterior As Boolean
      Private mbPositiveRotation As Boolean
      Private mbHasBulge As Boolean = False
      Private miSourceID As Integer
      Private msSourceTopoName As String
      Private msObjectList As String = String.Empty
      Private mtaSegments() As BoundarySegment
      Private mdicSegmentIndices As Dictionary(Of Integer, List(Of Integer))
		Private miPrevNeighborID As Integer = -1
		Private mhsGenuineNodes As HashSet(Of Integer) = New HashSet(Of Integer)()
		Private mbHasCircularArc As Boolean
      '   Private mcolLinks As System.Collections.Generic.ICollection(Of DMAcadExt.UD_Link)
      Private mcolLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
      Private mbIsPseudoGeo As Boolean
		Public ReadOnly Property HasCircularArc As Boolean
			Get
				Return mbHasCircularArc
			End Get
		End Property

		Public ReadOnly Property IsPseudoGeo As Boolean
         Get
            Return mbIsPseudoGeo
         End Get
      End Property
      Public Shared Sub DispBulge(sPrefix As String, oBulgeVertex As BulgeVertex)
         DMAcadExt.AcadDocument.WriteMessageLog(sPrefix & " b=" & CStr(oBulgeVertex.Bulge) & ":" & DMAcadExt.TPlnPoint.DispPoint(oBulgeVertex.Vertex))
      End Sub
      Public Sub AddDim(ByVal iVertexNum As Integer)
         If iVertexNum > 0 Then
            miCurrentIndex = miUB + 1
            miUB += iVertexNum
            Try
               ReDim Preserve moaBulgeVertex(miUB)
            Catch oEx As Exception
               MessageBox.Show(oEx.ToString(), "12_288")
            End Try
         End If
      End Sub
      Public Sub DecreaseDim(ByVal iVertexNum As Integer)
         If iVertexNum > 0 Then
            ' miCurrentIndex = miUB + 1
            miUB -= iVertexNum
            Try
               ReDim Preserve moaBulgeVertex(miUB)

				Catch oEx As Exception
               MessageBox.Show(oEx.ToString(), "12_273")
            End Try
         End If
      End Sub

      Public Function GetFirstPoint() As Point2d

         If miUB <> -1 Then
            Return moaBulgeVertex(0).Vertex
         End If
      End Function
      Public Function GetFirstBulge() As Double
         If miUB <> -1 Then
            Return moaBulgeVertex(0).Bulge
         Else
            Return 0.0
         End If
      End Function
      Public Function GetLastPoint() As Point2d
         If miUB <> -1 Then
            Return moaBulgeVertex(miUB).Vertex
         End If
      End Function
      Public Function GetLastReverseBulge() As Double
         If miUB <> -1 Then
            Return moaBulgeVertex(miUB - 1).Bulge
         Else
            Return 0.0
         End If
      End Function
      Public Function GetNeighborBoundary(ByVal iNeighbor As Integer, ByVal iSegmentNo As Integer, ByRef iIndexFrom As Integer, ByRef iIndexTo As Integer) As Polyline
         Dim oSegmentInd As List(Of Integer) = Nothing
         Dim bRes As Boolean = mdicSegmentIndices.TryGetValue(iNeighbor, oSegmentInd)

         If bRes Then
            Dim oResPolylines(oSegmentInd.Count - 1) As Polyline

            For iIndex As Integer = 0 To oSegmentInd.Count - 1
               '	zzGetIndexFromTo(oSegmentInd.Item(iIndex), iIndexFrom, iIndexTo)
               'DMAcadExt.AcadDocument.WriteMessage("N,From->To " & CStr(iIndex) & "," & CStr(iIndexFrom) & "->" & CStr(iIndexTo))
               oResPolylines(iIndex) = Me.CreatePolyline(iIndexFrom, iIndexTo)
            Next
            Return oResPolylines(0)
         Else
            Dim oTest As Dictionary(Of Integer, List(Of Integer)).KeyCollection = mdicSegmentIndices.Keys
            For Each iKey As Integer In oTest
               DMAcadExt.AcadDocument.WriteMessage("Key=" & iKey.ToString())
            Next
            MessageBox.Show(iNeighbor.ToString() & ":" & mdicSegmentIndices.Count.ToString(), "12_285")
            Return Nothing
         End If

      End Function
		Public Function GetNeighborBoundary(ByVal iNeighbor As Integer, ByVal iSegmentNo As Integer, ByRef iIndexFrom As Integer, ByRef iIndexTo As Integer, ByRef colAcObjIds As ObjectIdCollection, ByRef sLayerName As String) As Polyline
			Dim oSegmentInd As List(Of Integer) = Nothing
			Dim bRes As Boolean = mdicSegmentIndices.TryGetValue(iNeighbor, oSegmentInd)

			If bRes Then
				'Dim oResPolylines(oSegmentInd.Count - 1) As Polyline
				If iIndexFrom = miUB Then
					iIndexFrom = 0
				End If
				If iIndexTo = miUB Then
					iIndexTo = 0
				End If
				zzGetIndexFromTo(oSegmentInd.Item(iSegmentNo), iIndexFrom, iIndexTo, colAcObjIds, sLayerName)
				''''''' DMAcadExt.AcadDocument.WriteMessage("N,From->To - " & CStr(iIndexFrom) & "->" & CStr(iIndexTo))
				Return Me.CreatePolyline(iIndexFrom, iIndexTo)
			Else
				Dim oTest As Dictionary(Of Integer, List(Of Integer)).KeyCollection = mdicSegmentIndices.Keys
				For Each iKey As Integer In oTest
					DMAcadExt.AcadDocument.WriteMessage("Key=" & iKey.ToString())
				Next
				DMAcadExt.AcadDocument.WriteMessage(iNeighbor.ToString() & ":" & mdicSegmentIndices.Count.ToString(), "12_286")
				Return Nothing
			End If

		End Function
		Public Sub AddGenuineNodes(iVertexNum As Integer)
			mhsGenuineNodes.Add(iVertexNum)
		End Sub
		Public Property SourceID() As Integer
         Get
            Return miSourceID
         End Get
         Set(ByVal iValue As Integer)
            miSourceID = iValue
         End Set
      End Property
      Public Property SourceTopoName() As String
         Get
            Return msSourceTopoName
         End Get
         Set(ByVal sValue As String)
            msSourceTopoName = sValue
         End Set
      End Property
      Public Property ObjectList() As String
         Get
            Return msObjectList
         End Get
         Set(ByVal sValue As String)
            msObjectList = sValue
         End Set
      End Property

      Public Shared Function GetMidPoint(ByVal oBulgeVertex As BulgeVertex, ByVal tNextPoint As Point2d) As Point2d
         Dim tPointA As Point2d = oBulgeVertex.Vertex
         Dim tMid As Point2d = New Point2d(0.5 * (tPointA.X + tNextPoint.X), 0.5 * (tPointA.Y + tNextPoint.Y))
         Dim tBaseVector As Vector2d = tPointA.GetVectorTo(tNextPoint)
         Dim tPerpen As Vector2d = tBaseVector.GetPerpendicularVector
         Dim tHVector As Vector2d = tPerpen.MultiplyBy(oBulgeVertex.Bulge / 254)
         Return (tMid.Add(tHVector))
      End Function
      Public Sub TestA()
         Dim dDist As Double
         Dim sRes As String = String.Empty
         Dim bErr As Boolean
         Dim iIndexNext As Integer
         '	MessageBox.Show(CStr(oPolyline.NumberOfVertices) & ":" & CStr(VertexNum) & ":" & CStr(moaBulgeVertex.GetUpperBound(0)))
         For iIndex As Integer = 0 To miUB
            Try
               iIndexNext = (iIndex + 1) Mod (miUB + 1)
               dDist = moaBulgeVertex(iIndex).Vertex.GetDistanceTo(moaBulgeVertex(iIndexNext).Vertex)
               If dDist <> 0.0 Then
                  bErr = True
                  sRes &= CStr(iIndex) & ":" & CStr(dDist) & "; "
               End If
               'oPolyline.SetPointAt(iIndex + 1, moaBulgeVertex(iIndex).Vertex)
            Catch oEx As Exception
               MessageBox.Show(CStr(iIndex) & ":" & CStr(miUB), "26_669")
            End Try
         Next
         If bErr Then
            sRes = "!!! " & Me.SourceTopoName & ",ID=" & Me.SourceID.ToString() & " " & sRes
            DMAcadExt.AcadDocument.WriteMessageLog(sRes)
         End If



      End Sub


      Public Sub AddBulgeVertex(ByVal oBulgeVertex As BulgeVertex)
         If miCurrentIndex <= miUB Then
            moaBulgeVertex(miCurrentIndex) = oBulgeVertex
            miCurrentIndex += 1
         End If
      End Sub
      Public Sub AddBulgeVertex(ByVal oPoint As Point2d, ByVal dBulge As Double)
         If miCurrentIndex <= miUB Then
            moaBulgeVertex(miCurrentIndex) = New BulgeVertex(oPoint, dBulge)

            miCurrentIndex += 1
         End If
      End Sub
      Public Sub AddBulgeVertex(ByVal oPoint As Point3d)
         If miCurrentIndex <= miUB Then
            moaBulgeVertex(miCurrentIndex) = New BulgeVertex(New Point2d(oPoint.X, oPoint.Y), 0.0)
            miCurrentIndex += 1
         End If
      End Sub
      Public Sub AddBulgeVertex(ByVal oPoint As Point2d)
         If miCurrentIndex <= miUB Then
            moaBulgeVertex(miCurrentIndex) = New BulgeVertex(oPoint, 0.0)
            miCurrentIndex += 1
         End If
      End Sub
		Public Sub AddBulgeVertex(ByVal oPoint As Point3d, ByVal dBulge As Double)
			If miCurrentIndex <= miUB Then
				moaBulgeVertex(miCurrentIndex) = New BulgeVertex(New Point2d(oPoint.X, oPoint.Y), dBulge)
				miCurrentIndex += 1
			End If
		End Sub
		Public Sub PrintInfo()
			Dim oPoint As DMAcadExt.TPlnPoint
			For iIndex As Integer = 0 To moaBulgeVertex.GetUpperBound(0)
				oPoint = New DMAcadExt.TPlnPoint(moaBulgeVertex(iIndex).Vertex)
				DMCommon.Debug.ExcelLog.SetValueInRow(iIndex, oPoint.Coordinates2d)
			Next
		End Sub
		Public Sub JoinCurve(ByVal tAcObjID As ObjectId, ByVal bSameDirection As Boolean, Optional bDebug As Boolean = False)
         Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tAcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         Dim oLine As Line
         Dim oArc As Arc
         '  Dim oPolyline As Polyline
         '   Dim oPolyline2d As Polyline2d
         Dim oPrevLink As DMAcadExt.IUD_Link = Nothing
         Dim iCount As Integer = mcolLinks.Count

         If iCount > 0 Then
            oPrevLink = mcolLinks.Item(iCount - 1)
         End If
         If oCurve IsNot Nothing Then
            Select Case oCurve.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadLineName
                  oLine = DirectCast(oCurve, Line)
                  Me.zzJoinLine(oLine, bSameDirection)


               Case DMAcadExt.AcadConst.AcadArcName
                  oArc = DirectCast(oCurve, Arc)
                  Me.zzJoinArc(oArc, bSameDirection)

               Case Else
                  DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
                  Return
                  '	MessageBox.Show(oCurve.GetType().ToString(), "12_073")
            End Select
            If oPrevLink IsNot Nothing Then
               zzCheckPseudo(oPrevLink, mcolLinks.Item(mcolLinks.Count - 1))
            End If

         Else
            MessageBox.Show("Line " & tAcObjID.ToString() & " was not found", "13_069")
         End If
      End Sub
      Public Sub AddCurve(ByVal tAcObjID As ObjectId, ByVal bSameDirection As Boolean, Optional bDebug As Boolean = False)
			Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tAcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Dim oLine As Line
         Dim oArc As Arc
         Dim oPolyline As Polyline
         Dim oPolyline2d As Polyline2d
         Dim oPrevLink As DMAcadExt.IUD_Link = Nothing
         Dim iCount As Integer = mcolLinks.Count

         If iCount > 0 Then
            oPrevLink = mcolLinks.Item(iCount - 1)
         End If
         If oCurve IsNot Nothing Then
				Select Case oCurve.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadLineName
						oLine = DirectCast(oCurve, Line)
						Me.zzAddLine(oLine, bSameDirection)
					Case DMAcadExt.AcadConst.AcadPolylineName
						oPolyline = DirectCast(oCurve, Polyline)

						zzAddPolyline(oPolyline, bSameDirection, bDebug)
					Case DMAcadExt.AcadConst.AcadArcName
						oArc = DirectCast(oCurve, Arc)
						Me.zzAddArc(oArc, bSameDirection)
					Case DMAcadExt.AcadConst.Acad2dPolylineName
						oPolyline2d = DirectCast(oCurve, Polyline2d)
						zzAddPolyline2d(oPolyline2d, bSameDirection)
					Case Else
						DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
						Return

				End Select
				If oPrevLink IsNot Nothing Then
               zzCheckPseudo(oPrevLink, mcolLinks.Item(mcolLinks.Count - 1))
            End If

         Else
            MessageBox.Show("Line " & tAcObjID.ToString() & " was not found", "13_069")
         End If
      End Sub
      Public Sub AddCurve(ByVal tAcObjID As ObjectId, ByVal tFirstPoint As Point2d, ByVal iNeighbor As Integer)
         Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tAcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         Dim oLine As Line
         Dim oArc As Arc
         Dim oPolyline As Polyline
         Dim oPolyline2d As Polyline2d
         If msObjectList.Length <> 0 Then
            msObjectList &= ","
         End If
         Dim iSegmentUB As Integer = -1
         If mtaSegments IsNot Nothing Then
            iSegmentUB = mtaSegments.GetUpperBound(0)
         End If

         If miPrevNeighborID = iNeighbor Then
            mtaSegments(iSegmentUB).AddLine(tAcObjID, oCurve.Layer)
         Else
            iSegmentUB += 1
            ReDim Preserve mtaSegments(iSegmentUB)
            mtaSegments(iSegmentUB) = New BoundarySegment(tAcObjID, iNeighbor, oCurve.Layer)
            If mdicSegmentIndices.ContainsKey(iNeighbor) Then
               mdicSegmentIndices.Item(miPrevNeighborID).Add(iSegmentUB)
            Else
               Dim oSegmentInd As List(Of Integer) = New List(Of Integer)()
               oSegmentInd.Add(iSegmentUB)
               mdicSegmentIndices.Add(iNeighbor, oSegmentInd)
            End If
            miPrevNeighborID = iNeighbor
         End If


         msObjectList &= tAcObjID.ToString()
         If oCurve IsNot Nothing Then
            Select Case oCurve.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadLineName
                  oLine = DirectCast(oCurve, Line)
                  Me.zzAddLine(oLine, tFirstPoint)

               Case DMAcadExt.AcadConst.AcadPolylineName
						oPolyline = DirectCast(oCurve, Polyline)
						If oPolyline.NumberOfVertices = 2 AndAlso Not oPolyline.Closed Then
							If oPolyline.GetSegmentType(0) = SegmentType.Arc Then
								zzAddArcSegment(oPolyline, tFirstPoint)

							ElseIf oPolyline.GetSegmentType(0) = SegmentType.Line Then
								zzAddLineSegment(oPolyline, tFirstPoint)
							End If

						Else
							zzAddPolyline(oPolyline, tFirstPoint)
						End If

					Case DMAcadExt.AcadConst.AcadArcName
                  oArc = DirectCast(oCurve, Arc)
                  Me.zzAddArc(oArc, tFirstPoint)
               Case DMAcadExt.AcadConst.Acad2dPolylineName
                  oPolyline2d = DirectCast(oCurve, Polyline2d)
                  ''	DMAcadExt.AcadDocument.WriteMessage("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
                  zzAddPolyline2d(oPolyline2d, tFirstPoint)
               Case Else
                  DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
                  '	MessageBox.Show(oCurve.GetType().ToString(), "12_073")
            End Select
            mtaSegments(iSegmentUB).ArrayIndex = miUB - 1
         Else
            MessageBox.Show("Line " & tAcObjID.ToString() & " was not found", "12_069")
         End If
      End Sub

		Public Sub CloseSegment()
         Dim iNewSegmUB As Integer = 0
         If mtaSegments IsNot Nothing Then
            iNewSegmUB = mtaSegments.GetUpperBound(0) + 1
         End If
         ReDim Preserve mtaSegments(iNewSegmUB)
         mtaSegments(iNewSegmUB) = New BoundarySegment(miUB, miPrevNeighborID)

         If mdicSegmentIndices.ContainsKey(miPrevNeighborID) Then
            mdicSegmentIndices.Item(miPrevNeighborID).Add(iNewSegmUB)
         Else
            Dim oSegmentInd As List(Of Integer) = New List(Of Integer)()
            oSegmentInd.Add(iNewSegmUB)
            mdicSegmentIndices.Add(miPrevNeighborID, oSegmentInd)
         End If
         Dim sTest As String
         sTest = mtaSegments.GetUpperBound(0).ToString()
         For i As Integer = 0 To mtaSegments.GetUpperBound(0)
            sTest &= vbCrLf & i.ToString & ":" & mtaSegments(i).ToString()
         Next
         sTest &= vbCrLf & "-----------------" & vbCrLf
         For Each oList As List(Of Integer) In mdicSegmentIndices.Values
            sTest &= vbCrLf
            For i1 As Integer = 0 To oList.Count - 1
               sTest &= oList.Item(i1).ToString() & ","
            Next
         Next



         ''''''''''''''	DMAcadExt.AcadDocument.WriteMessage(sTest)

      End Sub
      Public Function GetBulgeVertexArrayRev() As BulgeVertex()
         Dim oaBulgeVertex(miUB) As BulgeVertex
         For iIndex As Integer = 0 To miUB
            oaBulgeVertex(miUB - iIndex) = moaBulgeVertex(iIndex)
         Next
         Return oaBulgeVertex
      End Function
      Public Function GetBulgeVertexArrayShifted(iStartIndex As Integer) As BulgeVertexArray
         Dim oaBulgeVertex(miUB) As BulgeVertex
         Dim iPrevIndex As Integer
         For iIndex As Integer = 0 To miUB
            iPrevIndex = (iStartIndex + iIndex) Mod (miUB + 1)
            oaBulgeVertex(iIndex) = moaBulgeVertex(iPrevIndex)
         Next
         Return New BulgeVertexArray(oaBulgeVertex)
      End Function
      Public Sub DispInfo()
         For iIndex As Integer = 0 To moaBulgeVertex.GetUpperBound(0)
            DMAcadExt.AcadDocument.WriteMessage("**" & CStr(iIndex) & " - " & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex) & "" & ";**Bulge= " & CStr(moaBulgeVertex(iIndex).Bulge))
         Next

      End Sub
      Public Function GetBulgeVertexArray() As BulgeVertex()

         Return moaBulgeVertex
      End Function
      Public Function GetBulgeVertexArrayOpened() As BulgeVertex()
         If moaBulgeVertex(0).Vertex.IsEqualTo(moaBulgeVertex(miUB).Vertex) Then
            Dim moaOutBulgeVertex(miUB) As BulgeVertex
            For iIndex As Integer = 0 To miUB - 1
               moaOutBulgeVertex(iIndex) = moaBulgeVertex(iIndex)

            Next
            '  System.Array.Copy(moaBulgeVertex, moaOutBulgeVertex, miUB)
            Dim oSourcePoint As Point2d = moaBulgeVertex(miUB).Vertex
            Dim dBulge As Double = moaBulgeVertex(miUB).Bulge
            moaOutBulgeVertex(miUB) = New BulgeVertex(New Point2d(oSourcePoint.X, oSourcePoint.Y - 0.0001), dBulge)
            Return moaOutBulgeVertex
         Else
            Return moaBulgeVertex
         End If


      End Function
      Public Function GetExtend() As Point2d
         Dim dXmin As Double
         Dim dXmax As Double
         Dim dYmin As Double
         Dim dYmax As Double
         Dim oBulgeVertex As BulgeVertex
         Dim dX, dY As Double
         For iIndex As Integer = 0 To miUB
            oBulgeVertex = moaBulgeVertex(iIndex)
            If oBulgeVertex IsNot Nothing Then
               dX = oBulgeVertex.Vertex.X
               dY = oBulgeVertex.Vertex.Y
               If dXmin > dX Then dXmin = dX
               If dXmax < dX Then dXmin = dX
               If dYmin > dY Then dYmin = dY
               If dYmax < dY Then dYmin = dY
            End If

         Next

      End Function

      Public Function AddArrayTopo(ByVal oBulgeVertexArray As BulgeVertexArray) As Boolean
         If oBulgeVertexArray.UB >= 0 Then
            If miUB = -1 Then
               moaBulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               Return True
            ElseIf BulgeVertexEq(Me.EndVertex, oBulgeVertexArray.StartVertex) Then
               ' MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB)), "08_116")
               ReDim Preserve moaBulgeVertex(miUB + oBulgeVertexArray.UB)
               Dim oaAddBulgeVertex() As BulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               For iIndex As Integer = 0 To oBulgeVertexArray.UB
                  moaBulgeVertex(miUB + iIndex) = oaAddBulgeVertex(iIndex)
               Next
               miUB = miUB + oBulgeVertexArray.UB
               Return True
            Else
               MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "Err 08_145")
               Return False
            End If
         Else
            MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_148")
            DMAcadExt.AcadDocument.WriteMessage("DesignErr: End " & Me.EndVertex.ToString() & " Start " & oBulgeVertexArray.StartVertex.ToString())
            Return True
         End If
      End Function
      Public Function AddArray(ByVal oBulgeVertexArray As BulgeVertexArray) As Boolean
         If oBulgeVertexArray.UB >= 0 Then
            If miUB = -1 Then
               moaBulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               Return True
            ElseIf BulgeVertexEq(Me.EndVertex, oBulgeVertexArray.StartVertex) Then
               ' MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB)), "08_116")
               ReDim Preserve moaBulgeVertex(miUB + oBulgeVertexArray.UB)
               Dim oaAddBulgeVertex() As BulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               For iIndex As Integer = 0 To oBulgeVertexArray.UB
                  moaBulgeVertex(miUB + iIndex) = oaAddBulgeVertex(iIndex)
               Next
               miUB = miUB + oBulgeVertexArray.UB
               Return True
            ElseIf BulgeVertexEq(Me.EndVertex, oBulgeVertexArray.EndVertex) Then
               '  MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_122")
               ReDim Preserve moaBulgeVertex(miUB + oBulgeVertexArray.UB + 1)
               Dim oaAddBulgeVertex() As BulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               For iIndex As Integer = 0 To oBulgeVertexArray.UB
                  moaBulgeVertex(miUB + iIndex) = oaAddBulgeVertex(oBulgeVertexArray.UB - iIndex)
               Next
               miUB = miUB + oBulgeVertexArray.UB + 1
               Return True
            ElseIf BulgeVertexEq(Me.StartVertex, oBulgeVertexArray.EndVertex) Then
               '  MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_129")
               zzInvert()
               ReDim Preserve moaBulgeVertex(miUB + oBulgeVertexArray.UB + 1)
               Dim oaAddBulgeVertex() As BulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               For iIndex As Integer = 0 To oBulgeVertexArray.UB
                  moaBulgeVertex(miUB + iIndex) = oaAddBulgeVertex(oBulgeVertexArray.UB - iIndex)
               Next
               miUB = miUB + oBulgeVertexArray.UB + 1
               Return True
            ElseIf BulgeVertexEq(Me.EndVertex, oBulgeVertexArray.EndVertex) Then
               '  MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_134")
               zzInvert()
               ReDim Preserve moaBulgeVertex(miUB + oBulgeVertexArray.UB + 1)
               Dim oaAddBulgeVertex() As BulgeVertex = oBulgeVertexArray.GetBulgeVertexArray()
               For iIndex As Integer = 0 To oBulgeVertexArray.UB
                  moaBulgeVertex(miUB + iIndex) = oaAddBulgeVertex(iIndex)
               Next
               miUB = miUB + oBulgeVertexArray.UB + 1
               Return True

            Else
               MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_145")
               Return False
            End If
         Else
            MessageBox.Show(CStr(miUB) & vbCrLf & CStr(oBulgeVertexArray.UB) & vbCrLf & CStr((miUB + oBulgeVertexArray.UB + 1)), "08_148")
            Return True
         End If
      End Function
      Public ReadOnly Property UB() As Integer
         Get
            Return miUB
         End Get
      End Property
		Public ReadOnly Property VertexNum() As Integer
			Get
				Return miUB + 1
			End Get
		End Property
		Public ReadOnly Property CurrentIndex() As Integer
			Get
				Return miCurrentIndex
			End Get
		End Property

		Public ReadOnly Property StartVertex() As BulgeVertex
         Get
            If miUB >= 0 Then
               Return moaBulgeVertex(0)
            Else
               Return Nothing
            End If
         End Get
      End Property
      Public ReadOnly Property EndVertex() As BulgeVertex
         Get
            If miUB >= 0 Then
               Return moaBulgeVertex(miUB)
            Else
               Return Nothing
            End If
         End Get
      End Property
      Public ReadOnly Property Item(ByVal iIndex As Integer) As BulgeVertex
         Get
            Return moaBulgeVertex(iIndex)
         End Get
      End Property
      Public ReadOnly Property ReverseItem(ByVal iIndex As Integer) As BulgeVertex
         Get
            Dim dBulge As Double = 0.0
            If iIndex < miUB Then
               dBulge = -moaBulgeVertex(miUB - iIndex - 1).Bulge
            End If
            Return New BulgeVertex(moaBulgeVertex(miUB - iIndex).Vertex, dBulge)
         End Get
      End Property
      Public Shared Function BulgeVertexEq(ByVal oBulgeVertex1 As BulgeVertex, ByVal oBulgeVertex2 As BulgeVertex) As Boolean
         Return oBulgeVertex1.Vertex.IsEqualTo(oBulgeVertex2.Vertex) AndAlso oBulgeVertex1.Bulge = oBulgeVertex2.Bulge
      End Function
      Public Sub New()
         mdicSegmentIndices = New Dictionary(Of Integer, List(Of Integer))()
         mcolLinks = New System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
      End Sub
      Public Sub New(ByVal oRing As Ring, ByVal bPositiveRotation As Boolean)
         Dim iCurrentVertexUB As Integer = -1
         Dim oHalfEdge As HalfEdge
         Dim oFullEdge As FullEdge
         'Dim oBulgeVertexArray As BulgeVertexArray
         Dim iStartID As Integer
         Dim oNode As Node
         mbPositiveRotation = bPositiveRotation
         mbSourceExterior = oRing.IsExterior
         oHalfEdge = oRing.StartEdge

         oNode = oHalfEdge.PreviousNode
         oFullEdge = oHalfEdge.FullEdge
         iStartID = oFullEdge.ID
         zzObjectIDToBulgeVertexArray(oFullEdge.Entity, DMAcadExt.TPlnPoint.Point3dTo2d(oNode.Location))
         If oRing.GetEdges().Count > 1 Then
            Do
               Try
                  oHalfEdge = oHalfEdge.GetNextEdge(Not bPositiveRotation)
                  oNode = oHalfEdge.PreviousNode
                  If oHalfEdge IsNot Nothing Then
                     DMAcadExt.AcadDocument.WriteMessage("HalfEdge.FullEdge.ID=" & CStr(oHalfEdge.FullEdge.ID))
                  End If
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "OK:BulgeVertexArray New")
                  oHalfEdge = Nothing
               End Try

               If oHalfEdge Is Nothing Then
                  'System.Windows.Forms.MessageBox.Show(moaBulgeVertex.GetUpperBound(0).ToString(), "622new")
                  Exit Do
               End If
               oFullEdge = oHalfEdge.FullEdge
               If oFullEdge.ID = iStartID Then
                  Exit Do
               End If
               zzObjectIDToBulgeVertexArray(oFullEdge.Entity, DMAcadExt.TPlnPoint.Point3dTo2d(oNode.Location))
            Loop
         End If
         If oHalfEdge IsNot Nothing Then
            oHalfEdge = Nothing
         End If

      End Sub
      Public Sub New(ByVal oRing As Ring)
         Dim iCurrentVertexUB As Integer = -1
         Dim oHalfEdge As HalfEdge
         Dim oFullEdge As FullEdge
         'Dim oBulgeVertexArray As BulgeVertexArray
         Dim iStartID As Integer
         Dim oNode As Node
         mbSourceExterior = oRing.IsExterior
         oHalfEdge = oRing.StartEdge

         oNode = oHalfEdge.PreviousNode
         oFullEdge = oHalfEdge.FullEdge

         iStartID = oFullEdge.ID

         zzObjectIDToBulgeVertexArray(oFullEdge.Entity, DMAcadExt.TPlnPoint.Point3dTo2d(oNode.Location))

         Do
            Try
               oHalfEdge = oHalfEdge.GetNextEdge(True)
               oNode = oHalfEdge.PreviousNode
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "OK:BulgeVertexArray New")
               oHalfEdge = Nothing
            End Try

            If oHalfEdge Is Nothing Then
               'System.Windows.Forms.MessageBox.Show(moaBulgeVertex.GetUpperBound(0).ToString(), "622new")
               Exit Do
            End If
            oFullEdge = oHalfEdge.FullEdge
            If oFullEdge.ID = iStartID Then
               Exit Do
            End If
            zzObjectIDToBulgeVertexArray(oFullEdge.Entity, DMAcadExt.TPlnPoint.Point3dTo2d(oNode.Location))
         Loop

      End Sub
      Public Sub New(colBulgeVertexCollection As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection)
         miUB = colBulgeVertexCollection.Count - 1
         ReDim moaBulgeVertex(miUB)
         colBulgeVertexCollection.CopyTo(moaBulgeVertex, 0)
         '	For iIndex As Integer = 0 To miUB
         'moaBulgeVertex(iIndex) = colBulgeVertexCollection.Item(iIndex)
         'Next
      End Sub
      Public Sub New(ByVal oaBulgeVertex() As BulgeVertex)
         moaBulgeVertex = oaBulgeVertex
         miUB = moaBulgeVertex.GetUpperBound(0)
      End Sub
      Public Sub New(ByVal iVertexNum As Integer)
         AddDim(iVertexNum)
      End Sub
      Public Sub New(ByVal oPolyline As Polyline)
         miUB = oPolyline.NumberOfVertices - 1

         If oPolyline.Closed Then
            miUB += 1
         End If

         ReDim moaBulgeVertex(miUB)


         Dim oBulgeVertex As BulgeVertex = Nothing

         For iIndex As Integer = 0 To miUB - 1
            oBulgeVertex = New BulgeVertex(oPolyline.GetPoint2dAt(iIndex), oPolyline.GetBulgeAt(iIndex))
            moaBulgeVertex(iIndex) = oBulgeVertex
         Next


         If oPolyline.Closed Then
            moaBulgeVertex(miUB) = moaBulgeVertex(0)
         Else
            oBulgeVertex = New BulgeVertex(oPolyline.GetPoint2dAt(miUB), oPolyline.GetBulgeAt(miUB))
            moaBulgeVertex(miUB) = oBulgeVertex
         End If









      End Sub
      Public Sub New(ByVal oPolyline2d As Polyline2d)
         Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
         Dim iVertexNum As Integer
         Dim iIndex As Integer = 0
         Dim tAcObjID As ObjectId
         Dim oDBObj As DBObject
         Dim oVertex2d As Vertex2d = Nothing
         Do While oColEnum.MoveNext()
            iIndex += 1
         Loop
         iVertexNum = iIndex
         If oPolyline2d.Closed Then
            iVertexNum += 1
         End If
         Me.AddDim(iVertexNum)
         oColEnum = oPolyline2d.GetEnumerator()
         iIndex = 0
         Do While oColEnum.MoveNext()
            tAcObjID = DirectCast(oColEnum.Current, ObjectId)
            oDBObj = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
            oVertex2d = DirectCast(oDBObj, Vertex2d)
            moaBulgeVertex(iIndex) = New BulgeVertex(DMAcadExt.TPlnPoint.Point3dTo2d(oVertex2d.Position), oVertex2d.Bulge)
            iIndex += 1
         Loop
         If oPolyline2d.Closed Then
            moaBulgeVertex(iIndex) = moaBulgeVertex(0)
         End If

         moaBulgeVertex(moaBulgeVertex.GetUpperBound(0)).Bulge = 0.0
      End Sub
      Public Sub New(ByVal oaCurves() As Curve2d)
         Dim oCurve As Curve2d = Nothing
         Dim oCompositeCurve As CompositeCurve2d
         Dim oOffsetCurve2d As OffsetCurve2d
         Dim oCircularArc2d As CircularArc2d
         Dim oaCurveItems() As Curve2d
         '  Dim oBulgeVertex As BulgeVertex
         Dim iOutIndex As Integer = 0

         Try
            miUB = oaCurves.GetUpperBound(0)

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "12_659")
         End Try
         miUB += 1

         ReDim moaBulgeVertex(miUB)


         For iIndex As Integer = 0 To miUB - 1
            oCurve = oaCurves(iIndex)
            If oCurve.GetType().ToString() = "Autodesk.AutoCAD.Geometry.OffsetCurve2d" Then
               oOffsetCurve2d = DirectCast(oCurve, OffsetCurve2d)
               oCurve = oOffsetCurve2d.Curve
            End If

            Select Case oCurve.GetType().ToString()
               Case "Autodesk.AutoCAD.Geometry.LineSegment2d"
                  moaBulgeVertex(iOutIndex) = New BulgeVertex(oCurve.StartPoint, 0.0)
                  iOutIndex += 1
               Case "Autodesk.AutoCAD.Geometry.CompositeCurve2d"
                  oCompositeCurve = DirectCast(oCurve, CompositeCurve2d)
                  oaCurveItems = oCompositeCurve.GetCurves()
                  If oaCurveItems.GetUpperBound(0) > 0 Then
                     miUB += oaCurveItems.GetUpperBound(0)
                     ReDim Preserve moaBulgeVertex(miUB)
                  End If
                  For iIndexItem As Integer = 0 To oaCurveItems.GetUpperBound(0)
                     moaBulgeVertex(iOutIndex) = New BulgeVertex(oaCurveItems(iIndexItem).StartPoint, 0.0)
                     iOutIndex += 1
                  Next
               Case "Autodesk.AutoCAD.Geometry.CircularArc2d"
                  oCircularArc2d = DirectCast(oCurve, CircularArc2d)
                  mbHasCircularArc = True
                  moaBulgeVertex(iOutIndex) = New BulgeVertex(oCircularArc2d.StartPoint, zzGetBulge(oCircularArc2d.StartPoint, oCircularArc2d.EndPoint, oCircularArc2d.Center))
                  iOutIndex += 1
               Case "Autodesk.AutoCAD.Geometry.EllipticalArc2d"
                  System.Windows.Forms.MessageBox.Show("", "Design Error EllipticalArc2d")
               Case "Autodesk.AutoCAD.Geometry.OffsetCurve2d"
                  oOffsetCurve2d = DirectCast(oCurve, OffsetCurve2d)

            End Select
         Next
         If iOutIndex <> miUB Then
            System.Windows.Forms.MessageBox.Show(CStr(iOutIndex) & ":" & CStr(miUB), "Design Error #1")
         ElseIf oCurve IsNot Nothing Then
            moaBulgeVertex(iOutIndex) = New BulgeVertex(oCurve.EndPoint, 0.0)
         End If

      End Sub
      Private Function zzGetBulge(tPointStart As Point2d, tPointEnd As Point2d, tPointCenter As Point2d) As Double
         Dim dRadius As Double = tPointCenter.GetDistanceTo(tPointStart)
         Dim dL As Double = 0.5 * tPointStart.GetDistanceTo(tPointEnd)
         Dim dBulge As Double = (dRadius - Math.Sqrt(dRadius * dRadius - dL * dL)) / dL
         Return dBulge
         '	bulge = (R - sqrt(R * R - L * L)) / L

      End Function
      Public Sub Join(ByVal oBulgeVertexArray As BulgeVertexArray, ByVal bSameDir As Boolean)
         '  DMAcadExt.AcadDocument.WriteMessage("090316b " & CStr(miUB) & "+" & CStr(oBulgeVertexArray.UB) & "; " & CStr(bSameDir))
         If miUB = -1 Then
            Try

               If bSameDir Then
                  moaBulgeVertex = oBulgeVertexArray.GetBulgeVertexArray
                  miUB = moaBulgeVertex.GetUpperBound(0)
                  miCurrentIndex = miUB + 1
               Else
                  Me.AddDim(oBulgeVertexArray.VertexNum)
                  Dim dBulge As Double
                  Dim tPoint As Point2d
                  For iIndex As Integer = 0 To oBulgeVertexArray.UB
                     tPoint = oBulgeVertexArray.Item(miUB - iIndex).Vertex
                     If iIndex <> miUB Then
                        dBulge = -oBulgeVertexArray.Item(miUB - iIndex - 1).Bulge ';;;;;;;;;;;;;;;;;;;temp 090316

                     Else
                        dBulge = 0.0
                     End If
                     moaBulgeVertex(iIndex) = New BulgeVertex(tPoint, dBulge)

                  Next
                  'moaBulgeVertex(miUB).Bulge = oBulgeVertexArray.GetFirstBulge()
               End If

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - Join_1")
            End Try

         ElseIf moaBulgeVertex Is Nothing Then
            System.Windows.Forms.MessageBox.Show("??????????", "BulgeVertexArray - Join_3")
         Else
            If oBulgeVertexArray Is Nothing Then
               System.Windows.Forms.MessageBox.Show("!!--???", "BulgeVertexArray - Join_5")
            End If
            If moaBulgeVertex(miUB) Is Nothing Then
               System.Windows.Forms.MessageBox.Show("*****", "BulgeVertexArray - Join_8")
            End If
            Dim iLastIndexBefore As Integer = miUB
            Try
               Me.AddDim(oBulgeVertexArray.UB)

               If bSameDir Then
                  For iIndex As Integer = 1 To oBulgeVertexArray.UB
                     Me.AddBulgeVertex(oBulgeVertexArray.Item(iIndex))
                  Next
                  moaBulgeVertex(iLastIndexBefore).Bulge = oBulgeVertexArray.GetFirstBulge()
               ElseIf Not bSameDir Then

                  For iIndex As Integer = 1 To oBulgeVertexArray.UB
                     Me.AddBulgeVertex(oBulgeVertexArray.ReverseItem(iIndex))
                  Next
                  moaBulgeVertex(iLastIndexBefore).Bulge = oBulgeVertexArray.GetLastReverseBulge()
                  '  DMAcadExt.AcadDocument.WriteMessage("090316c " & "Bulge=" & CStr(moaBulgeVertex(iLastIndexBefore).Bulge) & "; " & CStr(bSameDir))
               Else
                  'MessageBox.Show(DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_002")
                  ' DMAcadExt.AcadDocument.WriteMessage("#13_710:" & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oBulgeVertexArray.GetFirstPoint()) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oBulgeVertexArray.GetLastPoint()))
                  Return
               End If

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - Join_2")
            End Try
         End If
      End Sub
      Public Sub Join(ByVal oBulgeVertexArray As BulgeVertexArray, ByVal tFirstPoint As Point2d)

         If miUB = -1 Then
            Try

               If tFirstPoint.IsEqualTo(oBulgeVertexArray.GetFirstPoint(), New Tolerance(0.0001, 0.0001)) Then
                  moaBulgeVertex = oBulgeVertexArray.GetBulgeVertexArray
                  miUB = moaBulgeVertex.GetUpperBound(0)
                  miCurrentIndex = miUB + 1
               ElseIf tFirstPoint.IsEqualTo(oBulgeVertexArray.GetLastPoint(), New Tolerance(0.0001, 0.0001)) Then
                  Me.AddDim(oBulgeVertexArray.VertexNum)
                  Dim dBulge As Double
                  Dim tPoint As Point2d
                  For iIndex As Integer = 0 To oBulgeVertexArray.UB
                     tPoint = oBulgeVertexArray.Item(miUB - iIndex).Vertex
                     If iIndex <> miUB Then
                        dBulge = oBulgeVertexArray.Item(miUB - iIndex - 1).Bulge
                     Else
                        dBulge = 0.0
                     End If
                     moaBulgeVertex(iIndex) = New BulgeVertex(tPoint, dBulge)

                  Next
                  'moaBulgeVertex(miUB).Bulge = oBulgeVertexArray.GetFirstBulge()
               End If

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - Join_1")
            End Try

         ElseIf moaBulgeVertex Is Nothing Then
            System.Windows.Forms.MessageBox.Show("??????????", "BulgeVertexArray - Join_3")
         Else
            If oBulgeVertexArray Is Nothing Then
               System.Windows.Forms.MessageBox.Show("!!--???", "BulgeVertexArray - Join_5")
            End If
            If moaBulgeVertex(miUB) Is Nothing Then
               System.Windows.Forms.MessageBox.Show("*****", "BulgeVertexArray - Join_8")
            End If
            Dim iLastIndexBefore As Integer = miUB
            Try
               Me.AddDim(oBulgeVertexArray.UB)

               If tFirstPoint.IsEqualTo(oBulgeVertexArray.GetFirstPoint(), New Tolerance(0.0001, 0.0001)) Then
                  For iIndex As Integer = 1 To oBulgeVertexArray.UB
                     Me.AddBulgeVertex(oBulgeVertexArray.Item(iIndex))
                  Next
                  moaBulgeVertex(iLastIndexBefore).Bulge = oBulgeVertexArray.GetFirstBulge()
               ElseIf tFirstPoint.IsEqualTo(oBulgeVertexArray.GetLastPoint(), New Tolerance(0.0001, 0.0001)) Then

                  For iIndex As Integer = 1 To oBulgeVertexArray.UB
                     Me.AddBulgeVertex(oBulgeVertexArray.ReverseItem(iIndex))
                  Next
                  moaBulgeVertex(iLastIndexBefore).Bulge = oBulgeVertexArray.GetLastReverseBulge()
               Else
                  MessageBox.Show(DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_002")
                  DMAcadExt.AcadDocument.WriteMessage("#13_710:" & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oBulgeVertexArray.GetFirstPoint()) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oBulgeVertexArray.GetLastPoint()))
                  Return
               End If

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - Join_2")
            End Try
         End If
      End Sub

		Public Sub CloseLoop()
			If Me.IsClosed Then
				DecreaseDim(1)

			Else
				'DesignErr

				Dim oStartVertex As BulgeVertex = Me.StartVertex
				Dim oEndVertex As BulgeVertex = Me.EndVertex
				Dim sStartMsg As String
				Dim sEndMsg As String
				If oStartVertex IsNot Nothing Then
					sStartMsg = oStartVertex.Vertex.ToString()
				Else
					sStartMsg = "Nothing"
				End If
				If oEndVertex IsNot Nothing Then
					sEndMsg = oEndVertex.Vertex.ToString()
				Else
					sEndMsg = "Nothing"
				End If
				DMAcadExt.AcadDocument.WriteMessage("DesignErr #143 Start: " & sStartMsg & ", End: " & sEndMsg & ", UB=" & CStr(miUB))
				If oStartVertex.Vertex.IsEqualTo(oEndVertex.Vertex) Then
					DecreaseDim(1)
				End If
			End If


		End Sub

      Public Sub Terminate()
         Erase moaBulgeVertex
      End Sub
      Public ReadOnly Property IsClosed() As Boolean
         Get
            Dim oStartVertex As BulgeVertex = Me.StartVertex
            Dim oEndVertex As BulgeVertex = Me.EndVertex
            If oStartVertex IsNot Nothing AndAlso oEndVertex IsNot Nothing Then
               Return oStartVertex.Vertex.IsEqualTo(oEndVertex.Vertex)
            Else
               Return False
            End If


         End Get
      End Property
      Public ReadOnly Property SourceExterior() As Boolean
         Get
            Return mbSourceExterior
         End Get
      End Property
      Public Property PositiveRotation() As Boolean
         Get
            Return mbPositiveRotation
         End Get
         Set(ByVal bValue As Boolean)
            mbPositiveRotation = bValue
         End Set
      End Property
      Public Function GetCompositeCurve(ByVal bOpened As Boolean) As CompositeCurve2d
         Dim oPointStart, oPointEnd As DMAcadExt.TPlnPoint
         Dim iOutUB As Integer = miUB - 1
         Dim dFactor As Double = 1
         If iOutUB >= 0 Then
            Dim oaCurves(iOutUB) As Curve2d
            '	DMAcadExt.AcadDocument.WriteMessage("!!!!" & moaBulgeVertex.GetUpperBound(0).ToString())
            For iIndex As Integer = 0 To iOutUB
               If iIndex = iOutUB Then dFactor = 0.9994 '0.99 '
               oaCurves(iIndex) = zzCreateCurve(moaBulgeVertex(iIndex), moaBulgeVertex(iIndex + 1), dFactor)
               If iIndex = iOutUB Then
                  '	DMAcadExt.AcadDocument.WriteMessage("Index: " & TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
                  '	DMAcadExt.AcadDocument.WriteMessage("Index + 1: " & TPlnPoint.DispPoint(moaBulgeVertex(iIndex + 1).Vertex))

               End If
               oPointStart = New DMAcadExt.TPlnPoint(oaCurves(iIndex).StartPoint)
               oPointEnd = New DMAcadExt.TPlnPoint(oaCurves(iIndex).EndPoint)
               '	DMAcadExt.AcadDocument.WriteMessage("C: " & oPointStart.Coordinates & "; " & oPointEnd.Coordinates)
            Next
            Return New CompositeCurve2d(oaCurves)
         Else
            Return Nothing
         End If
      End Function
      Public Function Complete() As Boolean
         'DMAcadExt.AcadDocument.WriteMessageLog("Complete 4_76 " & CStr(miUB))
         Dim sNothing As String = ""
         For iIndex As Integer = 0 To miUB
            If moaBulgeVertex(iIndex) Is Nothing Then
               sNothing &= CStr(iIndex) & ","
            End If
         Next

         If sNothing.Length <> 0 Then
				DMAcadExt.AcadDocument.WriteMessageLog("4_32 Nothing:" & msSourceTopoName & "-" & CStr(miSourceID) & " UB=" & CStr(miUB) & "; " & sNothing & vbCrLf & msObjectList)
			End If
         If miUB >= 1 Then
            Dim oFirstBulgeVertex As BulgeVertex = moaBulgeVertex(0)
            Dim oLastBulgeVertex As BulgeVertex = moaBulgeVertex(miUB)

            If oFirstBulgeVertex IsNot Nothing AndAlso oLastBulgeVertex IsNot Nothing Then
               Dim tFirstPoint As Point2d = oFirstBulgeVertex.Vertex
               Dim dDist As Double = tFirstPoint.GetDistanceTo(oLastBulgeVertex.Vertex)
               If dDist <> 0.0 Then
                  'MessageBox.Show(CStr(dDist), Me.SourceTopoName & ":" & CStr(Me.SourceID))
                  ''''''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("4_21 " & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & ": " & DMAcadExt.TPlnPoint.DispPoint(oLastBulgeVertex.Vertex))
                  moaBulgeVertex(miUB).Vertex = tFirstPoint
                  ''''''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("4_22 Complete  TopoName=" & Me.SourceTopoName & "; Id=" & CStr(Me.SourceID) & ":  miUB=" & CStr(miUB) & " Dist=" & CStr(dDist))
                  '& vbCrLf & msObjectList
                  If dDist > 0.01 Then
                     For iIndex As Integer = 0 To miUB
                        If moaBulgeVertex(iIndex) Is Nothing Then
                           DMAcadExt.AcadDocument.WriteMessageLog("4_24  " & CStr(iIndex) & ": Nothing")
                        Else
                           DMAcadExt.AcadDocument.WriteMessageLog("4_23 " & CStr(iIndex) & ": " & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
                        End If
                     Next
                     Return (True)
                  Else
                     Return False
                  End If
               Else
                  Return False
               End If
            Else
               DMAcadExt.AcadDocument.WriteMessageLog("Complete 4_77 " & CStr(oFirstBulgeVertex IsNot Nothing) & " : " & CStr(oLastBulgeVertex IsNot Nothing))
               Return False
            End If
         Else
            Return False
         End If

         '	DMAcadExt.AcadDocument.WriteMessageLog("4_990  TopoName=" & Me.SourceTopoName & "; Id=" & CStr(Me.SourceID) & ":  miUB=" & CStr(miUB) & " Dist=" & CStr(dDist))

      End Function
      Public Function CreatePolyline() As Polyline ' Without Double Start End
         Dim oPolyline As Polyline = New Polyline(VertexNum)
         '	oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255)
         '	MessageBox.Show(CStr(oPolyline.NumberOfVertices) & ":" & CStr(VertexNum) & ":" & CStr(moaBulgeVertex.GetUpperBound(0)))
         For iIndex As Integer = 0 To miUB - 1
            Try
               'DMAcadExt.AcadDocument.WriteMessageLog("4_400 miUB=" & CStr(miUB) & " b=" & CStr(moaBulgeVertex(iIndex).Bulge) & ":" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
               oPolyline.AddVertexAt(iIndex, moaBulgeVertex(iIndex).Vertex, moaBulgeVertex(iIndex).Bulge, 0.0, 0.0)
               'oPolyline.SetPointAt(iIndex + 1, moaBulgeVertex(iIndex).Vertex)
            Catch oEx As Exception
               MessageBox.Show(CStr(iIndex) & ":" & CStr(miUB), "26_777")
            End Try
         Next
         If Not oPolyline.Closed Then
            '	Dim dDist As Double = oPolyline.GetPoint2dAt(0).GetDistanceTo(oPolyline.GetPoint2dAt(miUB-1))
            '	DMAcadExt.AcadDocument.WriteMessageLog("4_121 miUB=" & CStr(miUB) & "! " & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(0)) & ":" & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(miUB)))
            oPolyline.Closed = True
            '	DMAcadExt.AcadDocument.WriteMessageLog("4_122 NumberOfVertices=" & CStr(oPolyline.NumberOfVertices) & "! " & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(0)) & ":" & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(miUB)))

            '	MessageBox.Show(CStr(Me.SourceID) & ":" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(0).Vertex), "4_222")
         End If

         Return oPolyline
      End Function
      Public Function CreatePolyline(ByVal iIndexFrom As Integer, ByVal iIndexTo As Integer) As Polyline
         Dim oPolyline As Polyline = New Polyline(iIndexTo - iIndexFrom + 1)

         '	oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255)
         '	MessageBox.Show(CStr(oPolyline.NumberOfVertices) & ":" & CStr(VertexNum) & ":" & CStr(moaBulgeVertex.GetUpperBound(0)))
         For iIndex As Integer = iIndexFrom To iIndexTo
            Try
               'DMAcadExt.AcadDocument.WriteMessageLog("4_400 miUB=" & CStr(miUB) & " b=" & CStr(moaBulgeVertex(iIndex).Bulge) & ":" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
               oPolyline.AddVertexAt(iIndex - iIndexFrom, moaBulgeVertex(iIndex).Vertex, moaBulgeVertex(iIndex).Bulge, 0.0, 0.0)
               'oPolyline.SetPointAt(iIndex + 1, moaBulgeVertex(iIndex).Vertex)
            Catch oEx As Exception
               MessageBox.Show(CStr(iIndex) & ":" & CStr(miUB), "26_778")
            End Try
         Next

         Dim dDist As Double = oPolyline.GetPoint2dAt(0).GetDistanceTo(oPolyline.GetPoint2dAt(iIndexTo - iIndexFrom))
         '	DMAcadExt.AcadDocument.WriteMessageLog("4_121 miUB=" & CStr(miUB) & "! " & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(0)) & ":" & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(miUB)))
         If dDist < 0.0000001 Then
            oPolyline.Closed = True
         End If
         Return oPolyline
      End Function
      ' Test 21/02/2018
      Public Function CreateDBPolyline(bClosed As Boolean, Optional oPolyline As Polyline = Nothing) As ObjectId
         If oPolyline Is Nothing Then
            oPolyline = New Polyline(VertexNum)
         End If

         Dim oBulgeVertex As BulgeVertex
         For iIndex As Integer = 0 To miUB
            Try
               oBulgeVertex = moaBulgeVertex(iIndex)
               If oBulgeVertex IsNot Nothing Then
                  If iIndex = miUB Then
                     '  DMAcadExt.DMApp.MsgBox("12_800", bClosed, moaBulgeVertex(iIndex).Vertex, oBulgeVertex.Vertex)
                  End If
                  '    DMAcadExt.AcadDocument.WriteDebugMessage("**qq_" & CStr(iIndex) & "; " & bClosed.ToString(), moaBulgeVertex(0).Vertex.IsEqualTo(oBulgeVertex.Vertex, New Tolerance(0.0001, 0.0001)).ToString())
                  If Not (iIndex = miUB AndAlso bClosed AndAlso moaBulgeVertex(0).Vertex.IsEqualTo(oBulgeVertex.Vertex, New Tolerance(0.0001, 0.0001))) Then
                     oPolyline.AddVertexAt(iIndex, moaBulgeVertex(iIndex).Vertex, moaBulgeVertex(iIndex).Bulge, 0.0, 0.0)

                  End If

               Else
                  System.Windows.Forms.MessageBox.Show(CStr(iIndex) & ":" & CStr(miUB), "26_775")
               End If
            Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
               System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & CStr(iIndex) & ":" & CStr(miUB), "26_777")

            End Try
         Next
         '   GeoUtilites.vb() : Line 1765
         oPolyline.Closed = bClosed
         '    DMAcadExt.AcadDocument.WriteMessage("!## PlINES #=" & CStr(oPolyline.NumberOfVertices) & " Closed=" & oPolyline.Closed & " UB=" & miUB.ToString())

         Return DMAcadExt.AcadTransaction.AppendEntity(oPolyline)
      End Function
		Public Function GetMarkPoints(dDistParam As Double, Optional sLayer As String = Nothing) As List(Of InitInsertData) 'System.Collections.ObjectModel.Collection(Of InitInsertData)
			Dim oPolyline As Polyline
			Dim oBulgeVertex As BulgeVertex
			Dim oNextBulgeVertex As BulgeVertex
			Dim dCurrentDistParam As Double = dDistParam * 0.5
			Dim tInitInsertData As InitInsertData
			Dim bIsLast As Boolean
			'  Dim colInitInsertData As System.Collections.ObjectModel.Collection(Of InitInsertData) = New System.Collections.ObjectModel.Collection(Of InitInsertData)()
			Dim oResList As List(Of InitInsertData) = New List(Of InitInsertData)()

			For iIndex As Integer = 0 To miUB - 1
				If iIndex = miUB - 1 Then
					bIsLast = True
				End If
				oBulgeVertex = moaBulgeVertex(iIndex)
				oNextBulgeVertex = moaBulgeVertex(iIndex + 1)

				If oBulgeVertex IsNot Nothing AndAlso oNextBulgeVertex IsNot Nothing Then


					oPolyline = New Polyline(2)
					If sLayer IsNot Nothing Then
						oPolyline.Layer = sLayer
					End If

					oPolyline.AddVertexAt(0, oBulgeVertex.Vertex, oBulgeVertex.Bulge, 0.0, 0.0)
					oPolyline.AddVertexAt(1, oNextBulgeVertex.Vertex, 0.0, 0.0, 0.0)

					Do
						'MessageBox.Show(CStr(iIndex) & vbCrLf & tInitInsertData.Position.ToString(), "08_941")
						If bIsLast Then
							If dCurrentDistParam <= oPolyline.Length - 0.5 * dDistParam Then
								tInitInsertData = New InitInsertData(oPolyline, dCurrentDistParam)
								dCurrentDistParam = dCurrentDistParam + dDistParam
							ElseIf dCurrentDistParam <= oPolyline.Length - 0.25 * dDistParam Then
								If oPolyline.Length - 0.5 * dDistParam <= 0 Then
									'DMCommon.Debug.MsgBox("12_730w", oPolyline.Length, dDistParam)
								End If
								tInitInsertData = New InitInsertData(oPolyline, oPolyline.Length - 0.25 * dDistParam)
								dCurrentDistParam = dCurrentDistParam + dDistParam
							Else
								Exit Do
							End If
						Else
							If dCurrentDistParam < oPolyline.Length Then
								tInitInsertData = New InitInsertData(oPolyline, dCurrentDistParam)
								' DMAcadExt.AcadDocument.WriteDebugMessageN("aFTER nEW=", tInitInsertData.Layer)
								dCurrentDistParam = dCurrentDistParam + dDistParam
							Else
								dCurrentDistParam -= oPolyline.Length
								Exit Do
							End If
						End If
						' DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & " | " & tInitInsertData.Position.ToString() & " | " & dCurrentDistParam)
						'  MessageBox.Show(CStr(iIndex) & vbCrLf & tInitInsertData.Position.ToString(), "08_941")
						oResList.Add(tInitInsertData)
					Loop
					' GeoUtilites.vb:line 1835

				Else
					System.Windows.Forms.MessageBox.Show(CStr(iIndex) & ":" & CStr(miUB), "26_775")
				End If
			Next
			Return oResList
		End Function

		Public Function Create() As Boolean


			For iIndex As Integer = 0 To moaBulgeVertex.GetUpperBound(0)

				Try


				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.ErrorStatus.ToString(), "CreateHatchLoop")
					Exit For
				End Try
			Next
			Return True


		End Function


		Public Function CreateHatchLoop(ByVal iHatchLoopTypes As HatchLoopTypes) As HatchLoop
         Dim oHatchLoop As HatchLoop = New HatchLoop(HatchLoopTypes.Polyline Or iHatchLoopTypes)

			For iIndex As Integer = 0 To moaBulgeVertex.GetUpperBound(0)
            Try
					''''''DMAcadExt.AcadDocument.WriteMessageLog(BulgeVertexToString(moaBulgeVertex(iIndex)))

					oHatchLoop.Polyline.Add(moaBulgeVertex(iIndex))

					'	DMAcadExt.AcadDocument.WriteMessage("!!#Index= " & CStr(iIndex) & "-" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex) & "; " & CStr(moaBulgeVertex(iIndex).Bulge))
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.ErrorStatus.ToString(), "CreateHatchLoop")
					Exit For
				End Try
         Next
         '	DMAcadExt.AcadDocument.WriteMessage("####### E N D ################# " & CStr(moaBulgeVertex.GetUpperBound(0)))
         Return oHatchLoop
         '		Catch oEx As Exception
         'System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "SimplePgon - zzCalcExternalLoop")

      End Function

		Public Function StartPointIsPseudo() As Boolean
			Dim iCount As Integer = mcolLinks.Count
			If iCount > 1 Then
				Dim oPrevLink As DMAcadExt.IUD_Link = mcolLinks.Item(iCount - 1)
				Dim oNextLink As DMAcadExt.IUD_Link = mcolLinks.Item(0)
				Return oPrevLink.IsExtend(oNextLink, New DMAcadExt.dmTolerance(0.01, 0.01, 0.4))
			Else
				Return False
			End If
		End Function
		Public Function GetPseudoVertices(tTolerance As DMAcadExt.dmTolerance) As Point2dCollection
			Dim oLink As DMAcadExt.IUD_Link = Nothing
			Dim oNextLink As DMAcadExt.IUD_Link = Nothing
			Dim colPoints As Point2dCollection = New Point2dCollection()
			'Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
			Dim iNextIndex As Integer
			For iIndex As Integer = 0 To miUB
				oLink = zzGetLink(iIndex)
				oNextLink = zzGetLink(iIndex + 1)
				iNextIndex = (iIndex + 1) Mod (miUB + 1)

				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Link-Next", oLink.Coordinates, oNextLink.Coordinates)
				If oLink.IsExtend(oNextLink, tTolerance) And Not mhsGenuineNodes.Contains(iNextIndex) Then
					colPoints.Add(oLink.EndPoint)
				End If
			Next
			Return colPoints
		End Function

		Public Sub LoopInfo()
			Dim sOut As String = ""
			For iIndex As Integer = 0 To moaBulgeVertex.GetUpperBound(0)
				Try
					'	oHatchLoop.Polyline.Add(moaBulgeVertex(iIndex))
					If sOut.Length <> 0 Then sOut &= vbCrLf
					sOut &= BulgeVertexInfo(moaBulgeVertex(iIndex))

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf, "CreateHatchLoop")
				End Try
			Next
			DMAcadExt.AcadDocument.WriteMessage(sOut)
		End Sub

		Private Function zzGetLink(iIndex As Integer) As DMAcadExt.IUD_Link
			Dim oLink As DMAcadExt.IUD_Link = Nothing
			Dim oStartBulgeVertex As BulgeVertex = moaBulgeVertex(iIndex Mod (miUB + 1))
			Dim iNextIndex As Integer = (iIndex + 1) Mod (miUB + 1)
			Dim oEndBulgeVertex As BulgeVertex = moaBulgeVertex(iNextIndex)

			If oStartBulgeVertex.Bulge = 0 Then
				oLink = New DMAcadExt.TplnLine(oStartBulgeVertex.Vertex, oEndBulgeVertex.Vertex)
			Else
				oLink = New DMAcadExt.TplnArc(oStartBulgeVertex, oEndBulgeVertex.Vertex)
			End If
			Return oLink
		End Function
		Private Function zzCreateCurve(ByVal oBulgeVertex1 As BulgeVertex, ByVal oBulgeVertex2 As BulgeVertex, Optional ByVal dFactor As Double = 1.0) As Curve2d
         Dim tStartPoint As Point2d = oBulgeVertex1.Vertex
         Dim tEndPoint As Point2d = oBulgeVertex2.Vertex
         If dFactor <> 1.0 Then
            tEndPoint = New Point2d(tStartPoint.X * (1 - dFactor) + dFactor * tEndPoint.X, tStartPoint.Y * (1 - dFactor) + dFactor * tEndPoint.Y)
            DMAcadExt.AcadDocument.WriteMessage("EndPoint: " & DMAcadExt.TPlnPoint.DispPoint(tEndPoint))
         End If
         If oBulgeVertex1.Bulge = 0.0 Then
            Return New LineSegment2d(tStartPoint, tEndPoint)
         Else
            Return New CircularArc2d(tStartPoint, tEndPoint, oBulgeVertex1.Bulge, True)
         End If
      End Function
      Private Sub zzGetIndexFromTo(ByVal iSegmentIndex As Integer, ByRef iIndexFrom As Integer, ByRef iIndexTo As Integer, ByRef colAcObjIds As ObjectIdCollection, ByRef sLayerName As String)
         If iSegmentIndex = 0 Then
            iIndexFrom = 0
         Else
            iIndexFrom = mtaSegments(iSegmentIndex - 1).ArrayIndex + 1
         End If
         iIndexTo = mtaSegments(iSegmentIndex).ArrayIndex + 1
         colAcObjIds = mtaSegments(iSegmentIndex).LineObjIDCol
         sLayerName = mtaSegments(iSegmentIndex).LayerName
      End Sub
      Private Sub zzInvert()
         Dim iIndex As Integer = 0
         Do While iIndex < miUB - iIndex
            moaBulgeVertex(iIndex) = moaBulgeVertex(miUB - iIndex)
            iIndex += 1
         Loop

      End Sub
      Private Sub zzMove(ByVal iShift As Integer)
         Dim iIndex As Integer = 0
         Do While iIndex < miUB - iIndex
            moaBulgeVertex(miUB - iIndex) = moaBulgeVertex(miUB - iIndex)
         Loop
      End Sub
      Private Function zzGetBasePoint(ByVal iFrom As Integer, ByVal iTo As Integer) As Point2d
         Dim oStartLine As Line2d = zzGetLine(zzGetPreviousIndex(iFrom), iFrom)
         Dim oEndLine As Line2d = zzGetLine(iTo, zzGetNextIndex(iTo))
         Return oStartLine.IntersectWith(oEndLine)(0)
      End Function
      Private Function zzGetPreviousIndex(ByVal iIndex As Integer) As Integer
         If iIndex = 0 Then
            Return miUB - 1
         Else
            Return iIndex
         End If
      End Function
      Private Function zzGetNextIndex(ByVal iIndex As Integer) As Integer
         If iIndex = miUB Then
            Return 1
         Else
            Return iIndex + 1
         End If
      End Function
      Private Function zzGetLine(ByVal iIndex1 As Integer, ByVal iIndex2 As Integer) As Line2d
         Return New Line2d(Me.Item(iIndex1).Vertex, Me.Item(iIndex2).Vertex)
      End Function

      Private Sub zzAddPolyline(ByVal oPolyline As Polyline, ByVal bSameDirection As Boolean, Optional bDebug As Boolean = False)
         Dim iVertNum As Integer = oPolyline.NumberOfVertices
         Dim iAddClosed As Integer = 0
         Dim iFirst As Integer = 0
         Dim bBulgeOnly As Boolean = False
         Dim iPrevIndex As Integer
         Dim tPoint As Point2d
         Dim dBulge As Double = 0
         Dim iStart As Integer, iEnd As Integer, iStep As Integer
         Dim iBulgeShift As Integer
         '  DMAcadExt.AcadDocument.WriteMessage("#13_860: N=" & oPolyline.NumberOfVertices.ToString())
         Try
            If oPolyline.Closed Then
               iAddClosed = 1
            End If
            If miCurrentIndex = 0 Then
               iFirst = 1
            End If
            If miCurrentIndex <> 0 Then
               bBulgeOnly = True
               iPrevIndex = miCurrentIndex - 1
            End If

            If bSameDirection Then
               iStart = 0
               iEnd = iVertNum - 1
               iStep = 1
               iBulgeShift = 0
            Else
               iStart = iVertNum - 1
               iEnd = 0
               iStep = -1
               iBulgeShift = 1

            End If
            Me.AddDim(iVertNum - 1 + iFirst + iAddClosed)
            Dim iTestCounter As Integer = 0
            Dim iTestExist As Integer = 0
            ' MessageBox.Show(CStr(iStart) & " to " & CStr(iEnd) & vbCrLf & "Step=" & CStr(iStep), "04_800")
            If bDebug Then
               DMAcadExt.AcadDocument.WriteMessageLog("#13_815:" & CStr(iStart) & " to " & CStr(iEnd) & "|||" & "Step=" & CStr(iStep))
            End If

            For iIndex As Integer = iStart To iEnd Step iStep
               tPoint = oPolyline.GetPoint2dAt(iIndex)
               If bDebug Then
                  DMAcadExt.AcadDocument.WriteMessageLog("#13_888:" & tPoint.ToString())
               End If
               If iStep = -1 And iIndex > 0 Then
                  dBulge = -oPolyline.GetBulgeAt(iIndex - 1)
               ElseIf iStep = 1 Then
                  dBulge = oPolyline.GetBulgeAt(iIndex)
               Else
                  dBulge = 0.0
               End If
               If bBulgeOnly AndAlso (iIndex = iStart) Then
                  moaBulgeVertex(iPrevIndex).Bulge = dBulge
               Else
                  Me.AddBulgeVertex(tPoint, dBulge)
                  iTestCounter += 1
               End If
            Next
            If iAddClosed = 1 Then
               tPoint = oPolyline.GetPoint2dAt(iStart)
               dBulge = oPolyline.GetBulgeAt(iStart)
               Me.AddBulgeVertex(tPoint, dBulge)
               iTestCounter += 1
            End If
            iTestExist = iVertNum - 1 + iFirst + iAddClosed
            If iTestCounter <> iTestExist Then
               DMAcadExt.AcadDocument.WriteMessageLog("#13_129:" & CStr(iTestCounter) & " : " & CStr(iTestExist))
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "BulgeVertexArray - zzAddPolyline")
         End Try
      End Sub
      Private Sub zzAddPolyline(ByVal oPolyline As Polyline, ByVal tFirstPoint As Point2d)
         Dim iVertNum As Integer = oPolyline.NumberOfVertices
         Dim iAddClosed As Integer = 0
         Dim iFirst As Integer = 0
         Dim bBulgeOnly As Boolean = False
         Dim iPrevIndex As Integer
         Dim tPoint As Point2d
         Dim dBulge As Double = 0
         Dim iStart As Integer, iEnd As Integer, iStep As Integer
			Dim iBulgeShift As Integer
			oPolyline.GetSegmentType(0)
			Try
				If oPolyline.Closed Then
					iAddClosed = 1
				End If
				If miCurrentIndex = 0 Then
					iFirst = 1
				End If
				If miCurrentIndex <> 0 Then
					bBulgeOnly = True
					iPrevIndex = miCurrentIndex - 1
				End If

				If tFirstPoint.IsEqualTo(oPolyline.GetPoint2dAt(0), New Tolerance(0.0001, 0.0001)) Then
					iStart = 0
					iEnd = iVertNum - 1
					iStep = 1
					iBulgeShift = 0
				ElseIf tFirstPoint.IsEqualTo(oPolyline.GetPoint2dAt(iVertNum - 1), New Tolerance(0.0001, 0.0001)) Then
					iStart = iVertNum - 1
					iEnd = 0
					iStep = -1
					iBulgeShift = 1
				Else
					MessageBox.Show(DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_001")
					DMAcadExt.AcadDocument.WriteMessageLog("#13_700:" & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(0)) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oPolyline.GetPoint2dAt(iVertNum - 1)))
					Return
				End If
				Me.AddDim(iVertNum - 1 + iFirst + iAddClosed)
				Dim iTestCounter As Integer = 0
				Dim iTestExist As Integer = 0

				For iIndex As Integer = iStart To iEnd Step iStep
					tPoint = oPolyline.GetPoint2dAt(iIndex)
					If iStep = -1 And iIndex > 0 Then
						dBulge = -oPolyline.GetBulgeAt(iIndex - 1)
					ElseIf iStep = 1 Then
						dBulge = oPolyline.GetBulgeAt(iIndex)
					Else
						dBulge = 0.0
					End If
					If bBulgeOnly AndAlso (iIndex = iStart) Then
						moaBulgeVertex(iPrevIndex).Bulge = dBulge
					Else
						Me.AddBulgeVertex(tPoint, dBulge)
						iTestCounter += 1
					End If
				Next
				If iAddClosed = 1 Then
					tPoint = oPolyline.GetPoint2dAt(iStart)
					dBulge = oPolyline.GetBulgeAt(iStart)
					Me.AddBulgeVertex(tPoint, dBulge)
					iTestCounter += 1
				End If
				iTestExist = iVertNum - 1 + iFirst + iAddClosed
				If iTestCounter <> iTestExist Then
					DMAcadExt.AcadDocument.WriteMessageLog("#13_129:" & CStr(iTestCounter) & " : " & CStr(iTestExist))
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "BulgeVertexArray - zzAddPolyline")
         End Try
      End Sub
      Private Sub zzAddPolyline2d(ByVal oPolyline2d As Polyline2d, ByVal bSameDir As Boolean)
         ' DMAcadExt.AcadDocument.WriteMessage("090316a " & oPolyline2d.Handle.ToString() & "; " & CStr(bSameDir))
         Dim oAddBulgeVertexArray As BulgeVertexArray = New BulgeVertexArray(oPolyline2d)
         Dim iBefore As Integer = miUB
         Dim iPlus As Integer = oAddBulgeVertexArray.miUB

         If oAddBulgeVertexArray IsNot Nothing Then
            Try
               Me.Join(oAddBulgeVertexArray, bSameDir)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - zzAddPolyline2d")
            End Try
            Dim iAfter As Integer = miUB
            'DMAcadExt.AcadDocument.WriteMessageLog("21_17 " & CStr(iBefore) & " + " & CStr(iPlus) & " = " & CStr(iAfter))
         Else
            System.Windows.Forms.MessageBox.Show("Polyline2d is corrupt", "BulgeVertexArray - zzAddPolyline2d_2")
         End If
      End Sub
      Private Sub zzAddPolyline2d(ByVal oPolyline2d As Polyline2d, ByVal tFirstPoint As Point2d)
         Dim oAddBulgeVertexArray As BulgeVertexArray = New BulgeVertexArray(oPolyline2d)
         Dim iBefore As Integer = miUB
         Dim iPlus As Integer = oAddBulgeVertexArray.miUB

         If oAddBulgeVertexArray IsNot Nothing Then
            Try
               Me.Join(oAddBulgeVertexArray, tFirstPoint)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - zzAddPolyline2d")
            End Try
            Dim iAfter As Integer = miUB
            'DMAcadExt.AcadDocument.WriteMessageLog("21_17 " & CStr(iBefore) & " + " & CStr(iPlus) & " = " & CStr(iAfter))
         Else
            System.Windows.Forms.MessageBox.Show("Polyline2d is corrupt", "BulgeVertexArray - zzAddPolyline2d_2")
         End If
      End Sub
      Private Sub zzAddPolyline2dOld(ByVal oPolyline As Polyline2d, ByVal oFirstPoint As Point2d)
         Dim oSpline As Spline
         oSpline = oPolyline.Spline
         Dim iVert As Integer = oSpline.NumControlPoints
         Dim iAddClosed As Integer = 0
         Dim iFirst As Integer = 0
         Dim bBulgeOnly As Boolean = False
         Dim iPrevIndex As Integer
         Dim oPoint As Point2d
         Dim dBulge As Double = 0
         Dim iStart As Integer, iEnd As Integer, iStep As Integer
         Dim iBulgeShift As Integer
         Try
            If oPolyline.Closed Then
               iAddClosed = 1
            End If
            If miCurrentIndex = 0 Then
               iFirst = 1
            End If
            If miCurrentIndex <> 0 Then
               bBulgeOnly = True
               iPrevIndex = miCurrentIndex - 1
            End If
            Me.AddDim(iVert + iAddClosed)
            If oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oPolyline.StartPoint), New Tolerance(0.0001, 0.0001)) Then
               iStart = 0
               iEnd = iVert - 1
               iStep = 1
               iBulgeShift = 0
            ElseIf oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oPolyline.EndPoint), New Tolerance(0.0001, 0.0001)) Then
               iStart = iVert - 1
               iEnd = 0
               iStep = -1
               iBulgeShift = 1
            Else
               Dim sMsg As String = DMAcadExt.TPlnPoint.DispPoint(oFirstPoint) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oSpline.GetControlPointAt(0)) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oSpline.GetControlPointAt(iVert - 1))
               sMsg &= vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oPolyline.StartPoint) & vbNewLine & DMAcadExt.TPlnPoint.DispPoint(oPolyline.EndPoint) & vbNewLine & CStr(iVert) & ":" & oPolyline.Closed.ToString()

               DMAcadExt.AcadDocument.WriteMessage("^^" & sMsg)
               Return
            End If

            For iIndex As Integer = iStart To iEnd Step iStep
               oPoint = DMAcadExt.TPlnPoint.Point3dTo2d(oSpline.GetControlPointAt(iIndex))
               dBulge = 0.0
               Me.AddBulgeVertex(oPoint, dBulge)
            Next
            If iAddClosed = 1 Then
               oPoint = DMAcadExt.TPlnPoint.Point3dTo2d(oSpline.GetControlPointAt(iStart))
               dBulge = 0.0
               Me.AddBulgeVertex(oPoint, dBulge)
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - zzAddPolyline")
         End Try
      End Sub
      Private Sub zzAddLine(ByVal oLine As Line, ByVal bSameDir As Boolean)
         If miCurrentIndex = 0 Then
            Me.AddDim(2)
         Else
            moaBulgeVertex(miCurrentIndex - 1).Bulge = 0.0
            Me.AddDim(1)

         End If
         If bSameDir Then
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oLine.StartPoint)
            End If
            Me.AddBulgeVertex(oLine.EndPoint)
         Else
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oLine.EndPoint)
            End If
            Me.AddBulgeVertex(oLine.StartPoint)
         End If
         Dim oTplnLine As DMAcadExt.TplnLine = New DMAcadExt.TplnLine(oLine)
      End Sub
      Private Sub zzJoinLine(ByVal oLine As Line, ByVal bSameDir As Boolean)


         If bSameDir Then
            If miCurrentIndex = 0 Then
               ' Me.AddBulgeVertex(oLine.StartPoint)
            End If
            '  Me.AddBulgeVertex(oLine.EndPoint)
            moaBulgeVertex(miCurrentIndex).Vertex = DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint)
         Else
            'If miCurrentIndex = 0 Then
            '    Me.AddBulgeVertex(oLine.EndPoint)
            'End If
            'Me.AddBulgeVertex(oLine.StartPoint)
            moaBulgeVertex(miCurrentIndex).Vertex = DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint)
         End If
         Dim oTplnLine As DMAcadExt.TplnLine = New DMAcadExt.TplnLine(oLine)



      End Sub
		Private Sub zzCheckPseudo(oPrevLink As DMAcadExt.IUD_Link, oNextLink As DMAcadExt.IUD_Link)
			mbIsPseudoGeo = oPrevLink.IsExtend(oNextLink, New DMAcadExt.dmTolerance(0.01, 0.01, 0.4))


		End Sub
		Private Sub zzJoin(oPrevLink As DMAcadExt.IUD_Link, oNextLink As DMAcadExt.IUD_Link)
         oPrevLink.Extend(oNextLink)

      End Sub

		Private Sub zzAddLine(ByVal oLine As Line, ByVal tFirstPoint As Point2d)

			Dim dStartDist, dEndDist As Double
			If miCurrentIndex = 0 Then
				Me.AddDim(2)
			Else
				Me.AddDim(1)
			End If
			dStartDist = tFirstPoint.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint))
			If dStartDist < mdTolerance Then
				If miCurrentIndex = 0 Then
					Me.AddBulgeVertex(oLine.StartPoint)
				End If
				Me.AddBulgeVertex(oLine.EndPoint)
				If dStartDist > 0.00000001 Then
					'	DMAcadExt.AcadDocument.MsgBox(CStr(Me.SourceID) & vbCrLf & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_100")
					DMAcadExt.AcadDocument.WriteMessageLog("26_100a " & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & "!!!" & DMAcadExt.TPlnPoint.DispPoint(oLine.StartPoint) & vbCrLf & "***" & CStr(tFirstPoint.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint))))
				End If

			Else
				dEndDist = tFirstPoint.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint))
				If dEndDist < mdTolerance Then
					If dEndDist > 0.00001 Then
						'	DMAcadExt.AcadDocument.MsgBox(DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_200")
						DMAcadExt.AcadDocument.WriteMessage("26_100b " & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & "!!!" & DMAcadExt.TPlnPoint.DispPoint(oLine.StartPoint) & vbCrLf & DMAcadExt.TPlnPoint.DispPoint(oLine.EndPoint) & " *** " & CStr(tFirstPoint.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint))) & "&& " & CStr(dEndDist))
					End If

					If miCurrentIndex = 0 Then
						Me.AddBulgeVertex(oLine.EndPoint)
					End If
					Me.AddBulgeVertex(oLine.StartPoint)

				Else
					MessageBox.Show(DMAcadExt.TPlnPoint.DispPoint(tFirstPoint), "26_000")

					'DMAcadExt.AcadDocument.WriteMessage("26_000!!! " & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(oLine.StartPoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(oLine.EndPoint))

					DMAcadExt.AcadDocument.WriteMessageLog("26_000!!! " & DMAcadExt.TPlnPoint.DispPoint(tFirstPoint) & "; " & CStr(dStartDist) & "; " & CStr(dEndDist))
				End If
			End If
		End Sub


		Private Sub zzAddArcSegment(ByVal oPolyline As Polyline, ByVal oFirstPoint As Point2d)
			If miCurrentIndex = 0 Then
				Me.AddDim(2)
			Else
				Me.AddDim(1)
			End If
			Dim dBulge As Double = oPolyline.GetBulgeAt(0)
			Dim tStartPoint As Point2d
			Dim tEndPoint As Point2d

			Dim tPointAt0 As Point2d = oPolyline.GetPoint2dAt(0)
			Dim tPointAt1 As Point2d = oPolyline.GetPoint2dAt(1)




			If oFirstPoint.IsEqualTo(tPointAt0) Then
				tStartPoint = tPointAt0
				tEndPoint = tPointAt1
			Else
				tStartPoint = tPointAt1
				tEndPoint = tPointAt0
				dBulge = -dBulge
			End If







			'''''''''''	dBulge = 0
			If miCurrentIndex = 0 Then
				Me.AddBulgeVertex(tStartPoint, dBulge)
			Else
				If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
					moaBulgeVertex(miCurrentIndex - 1).Bulge = dBulge
				Else
					MessageBox.Show(" Ind=" & CStr(miCurrentIndex), " 03_801")
					DMAcadExt.AcadDocument.WriteMessage(" 1??!!" & tStartPoint.ToString())
				End If

			End If
			Me.AddBulgeVertex(tEndPoint)

		End Sub
		Private Sub zzAddLineSegment(ByVal oPolyline As Polyline, ByVal oFirstPoint As Point2d)
			If miCurrentIndex = 0 Then
				Me.AddDim(2)
			Else
				Me.AddDim(1)
			End If



			Dim tStartPoint As Point2d
			Dim tEndPoint As Point2d

			Dim tPointAt0 As Point2d = oPolyline.GetPoint2dAt(0)
			Dim tPointAt1 As Point2d = oPolyline.GetPoint2dAt(1)




			If oFirstPoint.IsEqualTo(tPointAt0) Then


				tStartPoint = tPointAt0
				tEndPoint = tPointAt1
			Else
				tStartPoint = tPointAt1
				tEndPoint = tPointAt0

			End If







			If miCurrentIndex = 0 Then
				Me.AddBulgeVertex(tStartPoint, 0)
			Else
				If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
					moaBulgeVertex(miCurrentIndex - 1).Bulge = 0
				Else
					MessageBox.Show("Ind=" & CStr(miCurrentIndex), "03_801")
					DMAcadExt.AcadDocument.WriteMessage("1??!!" & tStartPoint.ToString())
				End If

			End If
			Me.AddBulgeVertex(tEndPoint)

		End Sub


		Private Sub zzAddArc(ByVal oArc As Arc, ByVal oFirstPoint As Point2d)
         If miCurrentIndex = 0 Then
            Me.AddDim(2)
         Else
            Me.AddDim(1)
         End If
         Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc, True)
         Dim dBulge As Double = oTplnArc.GetBulge()
         'dBulge = 0
         'dBulge = 0.5
         If oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oArc.StartPoint)) Then
            '''''''''''	dBulge = 0
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oArc.StartPoint, dBulge)
            Else
               If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
                  moaBulgeVertex(miCurrentIndex - 1).Bulge = dBulge
               Else
                  MessageBox.Show("Ind=" & CStr(miCurrentIndex), "03_801")
                  DMAcadExt.AcadDocument.WriteMessage("1??!!" & oArc.StartPoint.ToString())
               End If

            End If
            Me.AddBulgeVertex(oArc.EndPoint)
         ElseIf oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oArc.EndPoint)) Then
            dBulge = -dBulge
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oArc.EndPoint, dBulge)
            Else

               If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
                  moaBulgeVertex(miCurrentIndex - 1).Bulge = dBulge
               Else
                  MessageBox.Show("Ind=" & CStr(miCurrentIndex), "03_802")
                  DMAcadExt.AcadDocument.WriteMessage("2??!!" & oArc.EndPoint.ToString())
               End If



            End If
            Me.AddBulgeVertex(oArc.StartPoint)
         End If
      End Sub

      Private Sub zzAddArc(ByVal oArc As Arc, ByVal bSameDir As Boolean)
         If miCurrentIndex = 0 Then
            Me.AddDim(2)
         Else
            Me.AddDim(1)
         End If
         Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc, True)
         Dim dBulge As Double = oTplnArc.GetBulge()
         'dBulge = 0
         'dBulge = 0.5
         If bSameDir Then
            '''''''''''	dBulge = 0
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oArc.StartPoint, dBulge)
            Else
               If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
                  moaBulgeVertex(miCurrentIndex - 1).Bulge = dBulge
               Else
                  MessageBox.Show("Ind=" & CStr(miCurrentIndex), "03_801")
                  DMAcadExt.AcadDocument.WriteMessage("1??!!" & oArc.StartPoint.ToString())
               End If

            End If
            Me.AddBulgeVertex(oArc.EndPoint)
         Else
            dBulge = -dBulge
            If miCurrentIndex = 0 Then
               Me.AddBulgeVertex(oArc.EndPoint, dBulge)
            Else

               If moaBulgeVertex(miCurrentIndex - 1) IsNot Nothing Then
                  moaBulgeVertex(miCurrentIndex - 1).Bulge = dBulge
               Else
                  MessageBox.Show("Ind=" & CStr(miCurrentIndex), "03_802")
                  DMAcadExt.AcadDocument.WriteMessage("2??!!" & oArc.EndPoint.ToString())
               End If
            End If
            Me.AddBulgeVertex(oArc.StartPoint)
            mcolLinks.Add(oTplnArc)

         End If
      End Sub

      Private Sub zzJoinArc(ByVal oArc As Arc, ByVal bSameDir As Boolean)

         Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc, True)
         Dim oPrevTplnArc As DMAcadExt.TplnArc

         Dim dBulge As Double = oTplnArc.GetBulge()
         'dBulge = 0
         'dBulge = 0.5
         If bSameDir Then
            '''''''''''	dBulge = 0
            If miCurrentIndex = 0 Then

            Else
               oPrevTplnArc = New DMAcadExt.TplnArc(moaBulgeVertex(miCurrentIndex - 1), moaBulgeVertex(miCurrentIndex).Vertex)
               oPrevTplnArc.Extend(oTplnArc)
               moaBulgeVertex(miCurrentIndex - 1).Bulge = oPrevTplnArc.GetBulge()

               moaBulgeVertex(miCurrentIndex).Vertex = oPrevTplnArc.EndPoint




            End If

         Else

            If miCurrentIndex = 0 Then

            Else
               oPrevTplnArc = New DMAcadExt.TplnArc(moaBulgeVertex(miCurrentIndex - 1), moaBulgeVertex(miCurrentIndex).Vertex)
               oPrevTplnArc.Extend(oPrevTplnArc)
               moaBulgeVertex(miCurrentIndex - 1).Bulge = oPrevTplnArc.GetBulge()

               moaBulgeVertex(miCurrentIndex).Vertex = oPrevTplnArc.EndPoint

            End If


         End If
      End Sub
      Private Sub zzObjectIDToBulgeVertexArray(ByVal oObjectID As ObjectId, ByVal oFirstPoint As Point2d)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
         Dim oEntity As Entity
         Dim oPolyline As Polyline
         Dim oLine As Line
         Dim sRXClassName As String
         Dim iCurrentVertexUB As Integer = -1
         Dim oCurve2ds() As Curve2d = Nothing
         Dim iVert As Integer
         MessageBox.Show("!!!!!!!!!SSSSTTTTTTOOOOPPPPPPP", "12_920")
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()

         oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)


         oEntity = DirectCast(oDBObject, Entity)
         sRXClassName = oEntity.GetRXClass().Name
         Dim oBulgeVertexArray As BulgeVertexArray
         Dim dBulge As Double = 0
         Dim oPoint As Point2d
         Dim iAddClosed As Integer = 0
         Select Case sRXClassName
            Case DMAcadExt.AcadConst.AcadPolylineName
               oPolyline = DirectCast(oEntity, Polyline)
               iVert = oPolyline.NumberOfVertices
               If oPolyline.Closed Then
                  iAddClosed = 1
               End If
               Me.AddDim(iVert + iAddClosed)
               Dim iStart As Integer, iEnd As Integer, iStep As Integer

               If oFirstPoint.IsEqualTo(oPolyline.GetPoint2dAt(0)) Then
                  iStart = 0
                  iEnd = iVert - 1
                  iStep = 1
               ElseIf oFirstPoint.IsEqualTo(oPolyline.GetPoint2dAt(iVert - 1)) Then
                  iStart = iVert - 1
                  iEnd = 0
                  iStep = -1
               End If
               For iIndex As Integer = iStart To iEnd Step iStep
                  oPoint = oPolyline.GetPoint2dAt(iIndex)
                  dBulge = oPolyline.GetBulgeAt(iIndex)
                  Me.AddBulgeVertex(oPoint, dBulge)
               Next
               If iAddClosed = 1 Then
                  oPoint = oPolyline.GetPoint2dAt(iStart)
                  dBulge = oPolyline.GetBulgeAt(iStart)
                  Me.AddBulgeVertex(oPoint, dBulge)
               End If
            Case Common.AcadLWPolylineName
               oBulgeVertexArray = Nothing
            Case Common.AcadLineName
               oLine = DirectCast(oEntity, Line)
               oBulgeVertexArray = New BulgeVertexArray(2)
               If oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint)) Then
                  oBulgeVertexArray.AddBulgeVertex(oLine.StartPoint)
                  oBulgeVertexArray.AddBulgeVertex(oLine.EndPoint)
               ElseIf oFirstPoint.IsEqualTo(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint)) Then
                  oBulgeVertexArray.AddBulgeVertex(oLine.EndPoint)
                  oBulgeVertexArray.AddBulgeVertex(oLine.StartPoint)
               End If
            Case Else
               oBulgeVertexArray = Nothing
         End Select



         Try
            oTransaction.Commit()
            oTransaction = Nothing
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "BulgeVertexArray - zzObjectIDToBulgeVertexArray_1")
         Finally
            If Not oTransaction Is Nothing Then
               oTransaction.Abort()
               oTransaction = Nothing
            End If
         End Try


      End Sub


   End Class
   Private Class CurveArray
      Private moaCurve() As Curve2d
      Private miUB As Integer = -1
      Private miCurrentIndex As Integer = 0
      Public Sub AddDim(ByVal iCurveNum As Integer)
         If iCurveNum > 0 Then
            miCurrentIndex = miUB + 1
            miUB += iCurveNum
            ReDim Preserve moaCurve(miUB)
         End If
      End Sub
      Public Sub AddCurve(ByVal oCurve As Curve2d)
         If miCurrentIndex <= miUB Then
            moaCurve(miCurrentIndex) = oCurve
            miCurrentIndex += 1
         End If
      End Sub
      Public Sub AddCurve(ByVal oStarPoint As Point3d, ByVal oEndPoint As Point3d)
         If miCurrentIndex <= miUB Then
            moaCurve(miCurrentIndex) = New Line2d(New Point2d(oStarPoint.X, oStarPoint.Y), New Point2d(oEndPoint.X, oEndPoint.Y))

            miCurrentIndex += 1
         End If
      End Sub
      Public Function GetCurveArray() As Curve2d()

         Return moaCurve
      End Function
      Public Function AddArray(ByVal oCurveArray As CurveArray) As Boolean
         If oCurveArray.UB >= 0 Then
            If miUB = -1 Then
               moaCurve = oCurveArray.GetCurveArray
               Return True
            ElseIf Me.EndPoint.IsEqualTo(oCurveArray.StartPoint) Then
               ReDim Preserve moaCurve(miUB + oCurveArray.UB + 1)
               Dim oaAddCurve() As Curve2d = oCurveArray.GetCurveArray()
               For iIndex As Integer = 0 To oCurveArray.UB
                  moaCurve(miUB + iIndex) = oaAddCurve(iIndex)
               Next
               miUB = miUB + oCurveArray.UB + 1
               Return True
            Else
               Return False
            End If
         Else
            Return True
         End If
      End Function
      Public ReadOnly Property UB() As Integer
         Get
            Return miUB
         End Get
      End Property
      Public ReadOnly Property StartPoint() As Point2d
         Get
            If miUB >= 0 Then
               Return moaCurve(0).StartPoint
            Else
               Return Nothing
            End If
         End Get
      End Property
      Public ReadOnly Property EndPoint() As Point2d
         Get
            If miUB >= 0 Then
               Return moaCurve(miUB).EndPoint
            Else
               Return Nothing
            End If
         End Get
      End Property
      Public Shared Function PointEq(ByVal tPoint1 As Point2d, ByVal tPoint2 As Point2d) As Boolean
         Return tPoint1.IsEqualTo(tPoint2)
      End Function
      Public Sub New()

      End Sub

      Public Sub New(ByVal iVertexNum As Integer)
         AddDim(iVertexNum)
      End Sub
      Public Sub New(ByVal oaCurves() As Curve2d)
         Dim oCurve As Curve2d
         miUB = oaCurves.GetUpperBound(0)
         ReDim moaCurve(oaCurves.GetUpperBound(miUB))
         Dim oBulgeVertex As BulgeVertex

         For iIndex As Integer = 0 To oaCurves.GetUpperBound(0)
            oCurve = oaCurves(iIndex)
            Select Case oCurve.GetType().ToString()
               Case "Autodesk.AutoCAD.Geometry.LineSegment2d"
                  oBulgeVertex = New BulgeVertex(oCurve.StartPoint, 0.0)
               Case "Autodesk.AutoCAD.Geometry.EllipticalArc2d"
            End Select
         Next
      End Sub
      Public Function GetCompositeCurve() As CompositeCurve2d

         If miUB > 0 Then
            Dim oaCurves(miUB - 1) As Curve2d
            Return New CompositeCurve2d(oaCurves)
         Else
            Return Nothing
         End If

      End Function
      Private Function zzCreateCurve(ByVal oBulgeVertex1 As BulgeVertex, ByVal oBulgeVertex2 As BulgeVertex) As Curve2d
         If oBulgeVertex1.Bulge = 0.0 Then
            Return New LineSegment2d(oBulgeVertex1.Vertex, oBulgeVertex2.Vertex)
         Else
            Dim oEllipticalArc2d As EllipticalArc2d = New EllipticalArc2d
            Return oEllipticalArc2d
         End If
      End Function
   End Class



   Private Structure BoundarySegment
      Dim ArrayIndex As Integer
      Dim NeighborTopoID As Integer
      Dim LineObjIDCol As ObjectIdCollection
      Private msLayerName As String
      Private mbOneLayer As Boolean


      Public Sub New(ByVal iArrayIndex As Integer, ByVal iNeighborTopoID As Integer)
         ArrayIndex = iArrayIndex
         NeighborTopoID = iNeighborTopoID
      End Sub
      Public Sub New(ByVal tAcObjID As ObjectId, ByVal iNeighborTopoID As Integer, ByVal sLayerName As String)
         LineObjIDCol = New ObjectIdCollection()
         LineObjIDCol.Add(tAcObjID)
         NeighborTopoID = iNeighborTopoID
         msLayerName = sLayerName
      End Sub
      Public Sub AddLine(ByVal tAcObjID As ObjectId, ByVal sLayerName As String)
         If msLayerName Is Nothing OrElse msLayerName.Length = 0 Then
            msLayerName = sLayerName
            mbOneLayer = True
         ElseIf msLayerName <> sLayerName Then
            mbOneLayer = False
         End If
         Me.LineObjIDCol.Add(tAcObjID)
      End Sub
      Public ReadOnly Property LayerName() As String
         Get
            Return msLayerName
         End Get
      End Property

      Public Overrides Function ToString() As String
         Return ArrayIndex.ToString() & "/N=" & NeighborTopoID.ToString()

      End Function
   End Structure

   Public Sub New()
   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub
End Class
Public Structure InitInsertData
   Dim Position As Point3d 'DMAcadExt.TPlnPoint
   Dim Direction As Vector3d
   Dim Layer As String

   Public Sub New(oPosition As Point3d, tDirection As Vector3d)
      Position = oPosition
      Direction = tDirection
   End Sub
	Public Sub New(oPolyline As Polyline, dDistParam As Double)
		Dim dParam As Double
		Try
			dParam = oPolyline.GetParameterAtDistance(dDistParam)
			Position = oPolyline.GetPointAtParameter(dParam)
			Direction = oPolyline.GetFirstDerivative(dParam)
			Layer = oPolyline.Layer
			'DMAcadExt.AcadDocument.WriteDebugMessageN("New Layer=", "'" & Layer & "'", "'" & oPolyline.Layer & "'")
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.StackTrace & vbCrLf & "DistParam=" & dDistParam.ToString() & vbCrLf & "dParam=" & dParam.ToString(), "22_110")
		End Try

	End Sub
	Public ReadOnly Property Rotation As Double
      Get
         Dim dAngleX As Double = Direction.GetAngleTo(Vector3d.XAxis)
         Dim dAngleY As Double = Direction.GetAngleTo(Vector3d.YAxis)
         If dAngleY < 0.5 * Math.PI Then
            Return dAngleX
         Else
            Return Math.PI - dAngleX
         End If

      End Get
   End Property
End Structure