Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Namespace TPlanGraph
   Public MustInherit Class TplnTopoPgon
      Const msTopoName As String = "Pgon"
      Const msOffsetTopoName As String = "PgonOff"
      Protected diTopoID As Integer
      Protected ddAcadArea As Double
      Protected ddPerimeter As Double
      Protected ddCentroidX As Double
      Protected ddCentroidY As Double
		Protected dsTopologyName As String
      Protected diCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      'Protected dtExtents As Autodesk.AutoCAD.DatabaseServices.Extents2d
      Protected doBoundingBox As TPlnBoundingBox
      Protected dcolBoundaryObj As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      '''''''''''''''  Protected doTopology As TplnTopology
      Private mdAreaScale As Double = 0.001
		Private moPolygon As Autodesk.Gis.Map.Topology.Polygon
		Private mcolRings As RingCollection
		Private mbExteriorLeftAAA As Boolean
		Protected moaBulgeVertexArray() As GeoUtilites.BulgeVertexArray
      Public Sub New(ByVal iTopoID As Integer)
         diTopoID = iTopoID
      End Sub
		Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			Dim sTest As String = "a"
			Try
				moPolygon = oPolygon
				sTest = "b"
				dsTopologyName = oPolygon.Topology.Name
				sTest = "c"
				diTopoID = oPolygon.ID
				sTest = "d"
				ddAcadArea = oPolygon.Area * mdAreaScale
				sTest = "e"
				ddPerimeter = Math.Round(oPolygon.Perimeter, 0)
				sTest = "f"
				ddCentroidX = oPolygon.Centroid.X
				sTest = "g"
				ddCentroidY = oPolygon.Centroid.Y
				sTest = "h"
				diCentroidAcObjID = oPolygon.Entity
				sTest = "k"
				zzCalcExtents()

			Catch oMapEx As Autodesk.Gis.Map.MapException
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, sTest & " Polygon Id = " & CStr(diTopoID))
			End Try

		End Sub
		Public Sub LoadTopo(ByVal oTopology As TopologyModel)
			If dsTopologyName = oTopology.Name Then
				moPolygon = oTopology.GetPolygon(diTopoID)
			End If

		End Sub
      Public Property TopoID() As Integer
         Get
            Return diTopoID
         End Get
         Set(ByVal iValue As Integer)
            diTopoID = iValue
         End Set
      End Property
      Public ReadOnly Property CentroidAcObjID() As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Get
            Return diCentroidAcObjID
         End Get
      End Property
      Public ReadOnly Property AcadArea() As Double
         Get
            Return ddAcadArea
         End Get
      End Property
      Public ReadOnly Property CentroidX() As Double
         Get
            Return ddCentroidX
         End Get
      End Property
      Public ReadOnly Property CentroidY() As Double
         Get
            Return ddCentroidY
         End Get
      End Property
      Public ReadOnly Property BoundingBox() As TPlnBoundingBox
         Get
            Return doBoundingBox
         End Get
      End Property
      Public Sub Highlight()
         AcadTransaction.Highlight(dcolBoundaryObj)
      End Sub


      Public Function GetBoundBoxAAAAA() As Autodesk.AutoCAD.DatabaseServices.Extents2d
         Dim colRings As RingCollection
         Dim colHalfEdges As HalfEdgeCollection
         Dim oFullEdge As FullEdge
         Dim tObjectID As Autodesk.AutoCAD.DatabaseServices.ObjectId
         '  Dim tExtents2d As Autodesk.AutoCAD.DatabaseServices.Extents2d
         Dim tCurrentExtents2d As Autodesk.AutoCAD.DatabaseServices.Extents2d
         Dim dMinX As Double = 999999.0, dMinY As Double = 999999.0
         Dim dMaxX As Double = 0.0, dMaxY As Double = 0.0

         colRings = moPolygon.GetBoundary()
         For Each oRing As Ring In colRings
            colHalfEdges = oRing.GetEdges()
            For Each oHalfEdge As HalfEdge In colHalfEdges
               oFullEdge = oHalfEdge.FullEdge
               tObjectID = oFullEdge.Entity
               tCurrentExtents2d = AcadTransaction.GetExtents2d(tObjectID)
               If dMinX > tCurrentExtents2d.MinPoint.X Then
                  dMinX = tCurrentExtents2d.MinPoint.X
               End If
               If dMinY > tCurrentExtents2d.MinPoint.Y Then
                  dMinY = tCurrentExtents2d.MinPoint.Y
               End If
               If dMaxX < tCurrentExtents2d.MaxPoint.X Then
                  dMaxX = tCurrentExtents2d.MaxPoint.X
               End If
               If dMaxY < tCurrentExtents2d.MaxPoint.Y Then
                  dMaxY = tCurrentExtents2d.MaxPoint.Y
               End If
            Next
         Next
         Return New Autodesk.AutoCAD.DatabaseServices.Extents2d(dMinX, dMinY, dMaxX, dMaxY)
      End Function
      Private Function zzCalcExtents() As Autodesk.AutoCAD.DatabaseServices.Extents2d

         Dim colHalfEdges As HalfEdgeCollection
         Dim oFullEdge As FullEdge
         Dim tObjectID As Autodesk.AutoCAD.DatabaseServices.ObjectId
         doBoundingBox = New TPlnBoundingBox()
         Dim dMinX As Double = 999999.0, dMinY As Double = 999999.0
			Dim dMaxX As Double = 0.0, dMaxY As Double = 0.0
			Dim sTest As String = "a"
			Dim iInteriorRingIndex As Integer = 0
			Dim bPositiveRing As Boolean
			Dim bFirstEdge As Boolean
         Try
				dcolBoundaryObj = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
				sTest = "b"
				If moPolygon IsNot Nothing Then
					sTest = "c"
					sTest = "ca"
					sTest = "cb"
					mcolRings = moPolygon.GetBoundary()
					sTest = "d"
					ReDim moaBulgeVertexArray(mcolRings.Count - 1)

					If mcolRings IsNot Nothing Then
						sTest = "e"
						'	MessageBox.Show(CStr(diTopoID) & ":" & CStr(mcolRings.Count), "12_227")
						For Each oRing As Ring In mcolRings
							If mcolRings.Count > 1 Then
								AcadDocument.WriteMessage("Rings=" & CStr(mcolRings.Count) & " PgonId=" & CStr(diTopoID))
							End If
							sTest = "f"
							colHalfEdges = oRing.GetEdges()

							sTest = "g"
							If colHalfEdges IsNot Nothing Then
								sTest = "h"
								bFirstEdge = True
								For Each oHalfEdge As HalfEdge In colHalfEdges
									sTest = "i"
									Try
										oFullEdge = oHalfEdge.FullEdge
									Catch oEx As Exception
										System.Windows.Forms.MessageBox.Show(oEx.Message, "oFullEdge = oHalfEdge.FullEdge " & CStr(diTopoID))
										oFullEdge = Nothing
									End Try
									sTest = "ik"
									If oFullEdge IsNot Nothing Then
										sTest = "ip"
										If bFirstEdge Then
											bPositiveRing = zzGetExteriorLeft(oFullEdge)
											bFirstEdge = False
										End If

										'	mbExteriorLeft = zzDispEdge(oFullEdge)
										sTest = "j"
										tObjectID = oFullEdge.Entity
										'mbExteriorLeft = (oFullEdge.GetHalfEdge(True).Polygon.ID = diTopoID)
										'	zzDispHalfEdge(oFullEdge.GetHalfEdge(True), "Left")
										'	zzDispHalfEdge(oFullEdge.GetHalfEdge(False), "RIGHT")
										sTest = "k"
										dcolBoundaryObj.Add(tObjectID)
										sTest = "L"
										If oRing.IsExterior Then
											sTest = "m"
											doBoundingBox.Union(AcadTransaction.GetBoundingBox(tObjectID))
											sTest = "n"
										End If
										sTest = "o"
									End If
								Next
							End If
							If oRing.IsExterior Then
								sTest = "mw"
								moaBulgeVertexArray(0) = New GeoUtilites.BulgeVertexArray(oRing, bPositiveRing)
								sTest = "nw"


							Else
								moaBulgeVertexArray(iInteriorRingIndex) = New GeoUtilites.BulgeVertexArray(oRing, bPositiveRing)

								iInteriorRingIndex += 1
							End If

							colHalfEdges.Dispose()
						Next oRing
					End If
					'	colRings.Dispose()
					'	colRings = Nothing
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalcExtents 12_666" & "_" & CStr(diTopoID) & ";" & sTest)
			End Try
		End Function
		Private Sub zzDispHalfEdge(ByVal oHalfEdge As HalfEdge, ByVal sCaption As String)
			Dim sMsg As String
			If oHalfEdge Is Nothing Then
				sMsg = "Nothing"
			Else
				sMsg = CStr(oHalfEdge.Polygon.ID)
			End If
			MessageBox.Show(sMsg & ":" & CStr(diTopoID), "  12_249  " & sCaption)
		End Sub
		Private Function zzDispEdge(ByVal oFullEdge As FullEdge) As Boolean
			Dim sMsg As String = String.Empty
			Dim oHalfEdge As HalfEdge
			Dim iLeftPgonID, iRightPgonID As Integer
			Dim bOut As Boolean
			System.Windows.Forms.MessageBox.Show("zzDispEdge Before", CStr(diTopoID))
			Try
				Try
					oHalfEdge = oFullEdge.GetHalfEdge(True)
					System.Windows.Forms.MessageBox.Show(CStr(oHalfEdge Is Nothing), "Left " & CStr(diTopoID))
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Left" & CStr(diTopoID))
					oHalfEdge = Nothing
					iLeftPgonID = 0
				End Try

				If oHalfEdge IsNot Nothing Then
					Try
						iLeftPgonID = oHalfEdge.Polygon.ID
					Catch oMapEx As Autodesk.Gis.Map.MapException
						AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " LeftPgonID = oHalfEdge.Polygon.ID")
						System.Windows.Forms.MessageBox.Show(oHalfEdge.ToString() & ":" & CStr(oHalfEdge.IsDisposed), "IsDisposed " & CStr(diTopoID))

					End Try
				Else
					System.Windows.Forms.MessageBox.Show("oHalfEdge Is Nothing", CStr(diTopoID))
				End If

				Try
					oHalfEdge = oFullEdge.GetHalfEdge(False)
					System.Windows.Forms.MessageBox.Show(CStr(oHalfEdge Is Nothing), "Right " & CStr(diTopoID))

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Right" & CStr(diTopoID))
					oHalfEdge = Nothing
					iRightPgonID = 0
				End Try

				If oHalfEdge IsNot Nothing Then
					Try
						iRightPgonID = oHalfEdge.Polygon.ID
					Catch oMapEx As Autodesk.Gis.Map.MapException
						AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " RightPgonID = oHalfEdge.Polygon.ID")
						System.Windows.Forms.MessageBox.Show(oHalfEdge.ToString() & ":" & CStr(oHalfEdge.IsDisposed), "IsDisposed " & CStr(diTopoID))
					End Try

				End If
				If iLeftPgonID = diTopoID Then
					bOut = True
				ElseIf iRightPgonID = diTopoID Then
					bOut = False
				Else
					sMsg = "  Error!!!"
				End If
				MessageBox.Show(CStr(iLeftPgonID) & ":" & CStr(iRightPgonID) & sMsg, "12_249  -" & CStr(diTopoID))

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Global After" & CStr(diTopoID))
			End Try
			Return bOut
		End Function
		Private Function zzGetExteriorLeft(ByVal oFullEdge As FullEdge) As Boolean
			Dim sMsg As String = String.Empty
			Dim oHalfEdge As HalfEdge
			Dim iLeftPgonID, iRightPgonID As Integer

			Try
				oHalfEdge = oFullEdge.GetHalfEdge(True)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Left" & CStr(diTopoID))
				oHalfEdge = Nothing
				iLeftPgonID = 0
			End Try

			If oHalfEdge IsNot Nothing Then
				Try
					iLeftPgonID = oHalfEdge.Polygon.ID
				Catch oMapEx As Autodesk.Gis.Map.MapException
					iLeftPgonID = 0
				End Try
			End If
			If iLeftPgonID = diTopoID Then
				Return True
			Else
				Try
					oHalfEdge = oFullEdge.GetHalfEdge(False)
				Catch oEx As Exception
					oHalfEdge = Nothing
					iRightPgonID = 0
				End Try
				If oHalfEdge IsNot Nothing Then
					Try
						iRightPgonID = oHalfEdge.Polygon.ID
					Catch oMapEx As Autodesk.Gis.Map.MapException
						iRightPgonID = 0
					End Try
				End If
				If iRightPgonID = diTopoID Then
					Return False
				Else
					sMsg = "Error 1298: Link " & CStr(oFullEdge.ID)
					sMsg &= "; Left Polygon & CSTR(iLeftPgonID)"
					sMsg &= "; Right Polygon & CSTR(iRightPgonID)"
					AcadDocument.WriteMessage(sMsg)
				End If
			End If
		End Function
		Public Sub Paint(ByVal iPaintMethod As PaintMethod, ByVal tColorScheme As ColorScheme)
			Select Case iPaintMethod
				Case PaintMethod.FillPgon
					zzAddHatch(tColorScheme.BackColor)
				Case PaintMethod.BorderByTrim
					zzAddHatchBorder(tColorScheme.Border)
				Case PaintMethod.BorderByTopoBuffer
					zzCreateTopology("Pgon")
				Case Else
					zzPaintBorder(tColorScheme.Border)
			End Select

		End Sub
 
      Public MustOverride Sub Terminate()
		Private Sub zzFill(ByVal tColor As System.Drawing.Color)

		End Sub
		Private Sub zzPaintBorder(ByVal tBorder As Border)

		End Sub
		Private Sub zzPaintZebra(ByVal tZebra As Zebra)

		End Sub

		Private Sub zzPaintHatch(ByVal tHatch As TplnHatch)

		End Sub
      Private Sub zzGetAcadEntity(ByVal oObjectID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()
         oDBObject = oTransactionManager.GetObject(oObjectID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False, False)

         System.Windows.Forms.MessageBox.Show(oDBObject.GetType().ToString(), "DBObject")
         Try
            oTransaction.Commit()
            oTransaction = Nothing
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzGetAcadEntity")
         Finally
            If Not oTransaction Is Nothing Then
               oTransaction.Abort()
               oTransaction = Nothing
            End If
         End Try
      End Sub
      Private Sub zzCreateMainTopologyAAA()
         Const iOptions As Integer = 1
         Dim colRings As RingCollection
         Dim colHalfEdges As HalfEdgeCollection
         Dim oFullEdge As FullEdge
         Dim oObjectID As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

         Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim colInnerLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
         colRings = moPolygon.GetBoundary()
         For Each oRing As Ring In colRings
            colHalfEdges = oRing.GetEdges()
            For Each oHalfEdge As HalfEdge In colHalfEdges
               oFullEdge = oHalfEdge.FullEdge
               oObjectID = oFullEdge.Entity
               If Not colLinks.Contains(oObjectID) Then
                  colLinks.Add(oObjectID)
               End If
               ' zzGetAcadEntity(oObjectID)
            Next
         Next
         If iOptions = 1 Then


         End If

         Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
         Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

         Dim oTopoModel As TopologyModel
         Dim sName As System.String = msTopoName & diTopoID.ToString()
         Dim sOffsetName As System.String = msOffsetTopoName & diTopoID.ToString()


         Dim iTopologyType As TopologyTypes = TopologyTypes.Polygon
         Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
         If oTopos.Exists(sName) Then
            System.Windows.Forms.MessageBox.Show("Topology " & sName & " already exists", "TopoCreator - CreateTopology")
         Else
            Try
               oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "", "", True)
               oTopos.Create(sName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.StopAtMultipleCentroid, 0.1)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, sName & ": " & CStr(colLinks.Count) & ":" & "zzCreateMainTopology")
            Finally
               oDocLock.Dispose()

            End Try


         End If

         If oTopos.Exists(sName) Then

            Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

            oTopoModel = oTopos(sName)
            Try
               '''''''''''''''''''''''''''''''''''oTopoModel.Open(OpenMode.ForRead)

               ''''''''''''''''''''''''''''  oTopoModel.Buffer(CStr(-40), sOffsetName)

               ''''''''''''''''''''''''''''''''''  colInnerLinks = zzGetTopoLinks(sOffsetName)
               ''''''''''''''''''''   System.Windows.Forms.MessageBox.Show(CStr(colInnerLinks.Count), "300")
               zzAddHatchP(oHatch, colLinks, colInnerLinks)
               System.Windows.Forms.MessageBox.Show("", "600")
               '       oHatch.Associative = True
               '      oHatch.ColorIndex = 3
               '     System.Windows.Forms.MessageBox.Show("", "700")
               '    oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline, colLinks)
               '   System.Windows.Forms.MessageBox.Show("", "800")
               '  oHatch.EvaluateHatch(True)
               ' System.Windows.Forms.MessageBox.Show("", "900")

            Catch oMapEx As Autodesk.Gis.Map.MapException
               AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCreateMainTopology-4")
            Finally
               If oTopoModel.Status <> Status.Closed Then
                  oTopoModel.Close()
               End If
            End Try
         End If
         oTopos = Nothing
      End Sub
      Private Sub zzAddHatchAAA(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch, ByVal colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal colInnerLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
         oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External)
         '   oHatchLoop.Curves.Add(
         Try
            System.Windows.Forms.MessageBox.Show("", "501")
            oHatch.Associative = False

            System.Windows.Forms.MessageBox.Show("", "502")
            System.Windows.Forms.MessageBox.Show("", "560")
            ''\'  oHatch.Associative = False
            System.Windows.Forms.MessageBox.Show("", "565")
            oHatch.ColorIndex = 5
            System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count), "570")

            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal

            System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count), "574")
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()
            System.Windows.Forms.MessageBox.Show("", "510")
            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
            System.Windows.Forms.MessageBox.Show("", "520")
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)
            System.Windows.Forms.MessageBox.Show("", "530")

            Try
               sTest = "ga"
               oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External, colLinks)
               sTest = "gb"
               System.Windows.Forms.MessageBox.Show("", "579")
               oHatch.AppendLoop(oHatchLoop)
               oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default, colInnerLinks)
               sTest = "gx"
               System.Windows.Forms.MessageBox.Show("", "579a")
               oHatch.EvaluateHatch(True)
               System.Windows.Forms.MessageBox.Show("", "590")
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
            End Try

            oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


      End Sub
      Private Sub zzAddHatchC(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch, ByVal colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal colInnerLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

         oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default)
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing) & ":" & CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Curves OR Polylines Is Nothing DefaultPolyline-1")

         oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing) & ":" & CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Curves OR Polylines Is Nothing Polyline-2")

         Dim oaCurve2ds() As Autodesk.AutoCAD.Geometry.Curve2d
         oaCurve2ds = GeoUtilites.PolylineToCurve(colLinks)
         Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d = New Autodesk.AutoCAD.Geometry.CompositeCurve2d(oaCurve2ds)

			Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
			oaBulgeVertices = GeoUtilites.PolylineToVertices(colLinks)
         System.Windows.Forms.MessageBox.Show(oaBulgeVertices.GetUpperBound(0).ToString(), "??????BulgeVertices.GetUpperBound")


         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing Before")



         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing After")
         For iIndex As Integer = 0 To oaCurve2ds.GetUpperBound(0)
            Try
               zzDispPoint(oaCurve2ds(iIndex).StartPoint, "StartPoint " & CStr(iIndex))
               zzDispPoint(oaCurve2ds(iIndex).EndPoint, "EndPoint " & CStr(iIndex))
               oHatchLoop.Curves.Add(oaCurve2ds(iIndex))
            Catch oEx As Autodesk.AutoCAD.Runtime.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & sTest, "zzAddHatch=15")
            End Try
         Next

         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing After")
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Polyline Is Nothing Before")


         '  oHatchLoop.Curves.Add(oCompositeCurve)
         Dim colBulgeVertices As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection

         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.IsPolyline), "HatchLoop.IsPolyline")
         colBulgeVertices = oHatchLoop.Polyline
         If colBulgeVertices Is Nothing Then
            colBulgeVertices = New Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection()
            System.Windows.Forms.MessageBox.Show("!!!!!!!", "OK")
         End If
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Polyline Is Nothing Before")



         Try
            System.Windows.Forms.MessageBox.Show("", "501")
            oHatch.Associative = False


            ''\'  oHatch.Associative = False

            oHatch.ColorIndex = 5

            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal

            System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count), "574")
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()
            System.Windows.Forms.MessageBox.Show("", "510")
            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
            System.Windows.Forms.MessageBox.Show("", "520")
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)


            Try
               sTest = "ga"
               ''  oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External, colLinks)
               sTest = "gb"



               System.Windows.Forms.MessageBox.Show(oHatchLoop.Curves.Count.ToString(), "530")
               System.Windows.Forms.MessageBox.Show(oHatchLoop.IsPolyline.ToString(), "531")
               System.Windows.Forms.MessageBox.Show(oHatchLoop.LoopType.ToString(), "532")
               If oHatchLoop.Polyline IsNot Nothing Then
                  sTest = "gba"
                  System.Windows.Forms.MessageBox.Show(oHatchLoop.Polyline.Count.ToString(), "533")
               End If

               System.Windows.Forms.MessageBox.Show("Before AppendLoop", "579")
               sTest = "gbb"
               oHatch.AppendLoop(oHatchLoop)
               sTest = "gc"
               System.Windows.Forms.MessageBox.Show("After AppendLoop", "579a")

               '''''' ''''''''   oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default, colInnerLinks)
               sTest = "gx"
               System.Windows.Forms.MessageBox.Show("", "579a")
               oHatch.EvaluateHatch(True)
               System.Windows.Forms.MessageBox.Show("", "590")
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
            End Try

            oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            '  oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


      End Sub
      Private Sub zzAddHatchP(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch, ByVal colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal colInnerLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

         oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing) & ":" & CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Curves OR Polylines Is Nothing Polyline-2")



         Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
         oaBulgeVertices = GeoUtilites.PolylineToVertices(colLinks)
         System.Windows.Forms.MessageBox.Show(oaBulgeVertices.GetUpperBound(0).ToString(), "!!!!!!!!BulgeVertices.GetUpperBound")


         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing Before")



         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing After")
         For iIndex As Integer = 0 To oaBulgeVertices.GetUpperBound(0)
            Try
               zzDispPoint(oaBulgeVertices(iIndex).Vertex, "StartPoint " & CStr(iIndex))

					oHatchLoop.Polyline.Add(oaBulgeVertices(iIndex))
            Catch oEx As Autodesk.AutoCAD.Runtime.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & sTest, "zzAddHatch=15")
            End Try
         Next

         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing), "oHatchLoop.Curves Is Nothing After")
         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Polyline Is Nothing Before")


         '  oHatchLoop.Curves.Add(oCompositeCurve)
         Dim colBulgeVertices As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection

         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.IsPolyline), "HatchLoop.IsPolyline")

         System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline Is Nothing), "oHatchLoop.Polyline Is Nothing Before")



         Try
            System.Windows.Forms.MessageBox.Show("", "501")
            oHatch.Associative = False


            ''\'  oHatch.Associative = False

            oHatch.ColorIndex = 5

            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal

            System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count), "574")
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()
            System.Windows.Forms.MessageBox.Show("", "510")
            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
            System.Windows.Forms.MessageBox.Show("", "520")
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)


            Try
               sTest = "ga"
               ''  oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External, colLinks)
               sTest = "gb"


               If oHatchLoop.Polyline IsNot Nothing Then
                  System.Windows.Forms.MessageBox.Show(oHatchLoop.Polyline.Count.ToString(), "530")
               End If

               System.Windows.Forms.MessageBox.Show(oHatchLoop.IsPolyline.ToString(), "531")
               System.Windows.Forms.MessageBox.Show(oHatchLoop.LoopType.ToString(), "532")
               If oHatchLoop.Polyline IsNot Nothing Then
                  sTest = "gba"
                  System.Windows.Forms.MessageBox.Show(oHatchLoop.Polyline.Count.ToString(), "533")
               End If

               System.Windows.Forms.MessageBox.Show("Before AppendLoop", "579")
               sTest = "gbb"
               oHatch.AppendLoop(oHatchLoop)
               sTest = "gc"
               System.Windows.Forms.MessageBox.Show("After AppendLoop", "579a")

               '''''' ''''''''   oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default, colInnerLinks)
               sTest = "gx"
               System.Windows.Forms.MessageBox.Show("", "579a")
               oHatch.EvaluateHatch(True)
               System.Windows.Forms.MessageBox.Show("", "590")
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
            End Try

            oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            '  oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


		End Sub
		Private Sub zzCreateTopology(ByVal sTopoName As String)
			Dim colRings As RingCollection
			Dim bExteriorRing As Boolean = False
			Dim oHalfEdge As HalfEdge
			Dim oFullEdge As FullEdge
			Dim iStartID As Integer

			Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
			Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

			Try
				colRings = moPolygon.GetBoundary()
				For Each oRing As Ring In colRings
					If oRing.IsExterior AndAlso Not bExteriorRing Then
						oHalfEdge = oRing.StartEdge
						oFullEdge = oHalfEdge.FullEdge
						iStartID = oFullEdge.ID
						bExteriorRing = True

						Do
							Try
								oHalfEdge = oHalfEdge.GetNextEdge(True)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "OK:zzCreateTopology")
								oHalfEdge = Nothing
							End Try

							oFullEdge = oHalfEdge.FullEdge
							If oFullEdge.ID = iStartID Then
								Exit Do
							End If
							colLinks.Add(oFullEdge.Entity)
						Loop
					End If
				Next
				oTopos.Create(sTopoName & CStr(diTopoID), colLinks, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzAddHatchNew-327")
				'	oTransaction.Commit()
				AcadDocument.Unlock()
				Return
			End Try
		End Sub
		Private Sub zzAddHatchNew(ByVal tColor As DMColor)
			Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
			Dim colRings As RingCollection
			Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try

				oHatch.Associative = False
				MessageBox.Show(CStr(tColor.AcadColorIndex), "tColor.AcadColorIndex 12_127")
				oHatch.ColorIndex = tColor.AcadColorIndex

				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
				oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
				oTransaction = oTransactionManager.StartTransaction()

				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

				sTest = "b"
				Dim oTopology As TopologyModel
				Try
					oTopology = Common.GetTopology(dsTopologyName)
					oTopology.Open(OpenMode.ForWrite)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest & vbCrLf & dsTopologyName, "zzAddBorderHatch=129")
					Return
				End Try
				Try
					colRings = moPolygon.GetBoundary()
				Catch oMapEx As Autodesk.Gis.Map.MapException
					AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzAddHatchNew-327")
					oTransaction.Commit()
					AcadDocument.Unlock()
					Return
				End Try

				System.Windows.Forms.MessageBox.Show(CStr(colRings.Count), "colRings.Count 1222")

				For Each oRing As Ring In colRings
					'oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
					System.Windows.Forms.MessageBox.Show(oRing.IsExterior.ToString(), "IsExterior? 523")
					sTest = "ba"
					oHatchLoop = GeoUtilites.RingToLoop(oRing)
					sTest = "bb"

					Try
						sTest = "gbb"
						oHatch.AppendLoop(oHatchLoop)
						sTest = "gc"
						oHatch.EvaluateHatch(True)
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
					End Try
					Exit For
				Next
				oTopology.Close()
				oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
				sTest = "g"
				oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
				Try
					oHatch.UpgradeOpen()

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
				End Try

				'  oHatch.Associative = True
				oTransaction.Commit()
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
			Finally
				oTransaction.Dispose()
			End Try


		End Sub
		Private Sub zzAddHatch(ByVal tColor As DMColor)
			'		Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			'		Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			'		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			'		Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			'	Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			'	Dim oDrawOrderTable As Autodesk.AutoCAD.DatabaseServices.DrawOrderTable
			'	Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId

			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop


			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try

				oHatch.Associative = False
				oHatch.ColorIndex = tColor.AcadColorIndex + 1

				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal

				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				'			oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

				'	oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

				sTest = "b"
				'	tDrawOrderTableID = oBlockTableRecord.DrawOrderTableId
				'	oDrawOrderTable = DirectCast(oTransactionManager.GetObject(tDrawOrderTableID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.DrawOrderTable)

				oHatchLoop = moaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
				Try
					sTest = "gbb"
					oHatch.AppendLoop(oHatchLoop)
					sTest = "gc"
					oHatch.EvaluateHatch(True)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=249")
				End Try

				sTest = "be"
				oAcobjId = AcadTransaction.AppendEntity(oHatch, True)
				Try
					oHatch.UpgradeOpen()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
				End Try

				'  oHatch.Associative = True
				'
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
				'
			End Try


		End Sub
		Private Sub zzAddHatchBorder(ByVal tBorder As Border)
			'	Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			'	Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			'	Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			'	Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			'	Dim oDrawOrderTable As Autodesk.AutoCAD.DatabaseServices.DrawOrderTable
			'	Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId

			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

			Dim oHatchLoopBorder As Autodesk.AutoCAD.DatabaseServices.HatchLoop
			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try
				sTest = "axb"
				oHatch.Associative = False
				sTest = "axc"
				oHatch.ColorIndex = tBorder.Strips(0).Color.AcadColorIndex
				sTest = "axd"
				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
				sTest = "axe"
				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)


				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCompositeInner As Autodesk.AutoCAD.Geometry.CompositeCurve2d


				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				Dim dOffsetDist As Double

				'	oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
				oHatchLoop = moaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)

				sTest = "baa"
				oComposite = moaBulgeVertexArray(0).GetCompositeCurve(True)
				sTest = "baa1"
				If moaBulgeVertexArray(0).PositiveRotation Then
					dOffsetDist = 0.6
				Else
					dOffsetDist = -0.6
				End If
				System.Windows.Forms.MessageBox.Show(CStr(moaBulgeVertexArray(0).PositiveRotation) & ":" & CStr(Me.diTopoID), "!!!!!+++  12_2162  ")
				sTest = "baa1a"
				oCurves = oComposite.GetTrimmedOffset(dOffsetDist, Geometry.OffsetCurveExtensionType.Extend, New Geometry.Tolerance(0.1, 0.1))
				sTest = "baa2"
				Try
					oCompositeInner = DirectCast(oCurves(0), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					sTest = "bba1"
					If Not oCompositeInner.IsClosed Then
						sTest = "bba2"
						oCurves(0) = GeoUtilites.CloseCompositeCurve(oCompositeInner)
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), "oComposite.IsClosed 12_4423 zzAddHatchBorderNew")
					Else
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), " 12_3164 zzAddHatchBorderNew" & CStr(Me.diTopoID))
					End If
					sTest = "bba3"
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
				End Try

				Try
					sTest = "gbb"

					oHatch.AppendLoop(oHatchLoop)

					sTest = "gc"
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=232bNew")
				End Try

				If oCurves Is Nothing OrElse oCurves.GetUpperBound(0) = -1 Then


				Else

					oHatchLoopBorder = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)

					oHatch.AppendLoop(oHatchLoopBorder)
				End If
				oHatch.EvaluateHatch(True)
				sTest = "baa3"
				sTest = "bb"

				oAcobjId = AcadTransaction.AppendEntity(oHatch, True)




				sTest = "g"

				Try
					'	oHatch.UpgradeOpen()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11_bnew")
				End Try

				'  oHatch.Associative = True
				'	oTransaction.Commit()
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=moaBulgeVertex.GetUpperBound(0).ToString()!!!")
			End Try


		End Sub
		Private Sub zzAddHatchBorderOld(ByVal tColor As DMColor)
			Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

			Dim oHatchLoopBorder As Autodesk.AutoCAD.DatabaseServices.HatchLoop
			Dim colRings As RingCollection
			Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try

				oHatch.Associative = False
				MessageBox.Show(CStr(tColor.AcadColorIndex), "tColor.AcadColorIndex 12_234")
				oHatch.ColorIndex = tColor.AcadColorIndex

				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
				oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
				oTransaction = oTransactionManager.StartTransaction()

				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

				sTest = "b"
				Dim oTopology As TopologyModel
				Try
					oTopology = Common.GetTopology(dsTopologyName)
					oTopology.Open(OpenMode.ForWrite)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzAddHatchNew-344")
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & dsTopologyName & vbCrLf & sTest, "zzAddHatchNew=129a")
					Return
				End Try
				Try
					colRings = moPolygon.GetBoundary()
				Catch oMapEx As Autodesk.Gis.Map.MapException
					AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzAddHatchNew-327")
					oTransaction.Commit()
					AcadDocument.Unlock()
					Return
				End Try
				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				System.Windows.Forms.MessageBox.Show(CStr(colRings.Count), "colRings.Count 1222")
				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				For Each oRing As Ring In colRings
					'oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
					System.Windows.Forms.MessageBox.Show(oRing.IsExterior.ToString(), "IsExterior? 523")
					sTest = "baa"
					oComposite = GeoUtilites.RingToComposit(oRing)
					sTest = "baa1"

					System.Windows.Forms.MessageBox.Show(CStr(oComposite.IsClosed()), "oComposite.IsClosed 12_2366")
					oCurves = oComposite.GetTrimmedOffset(2, Geometry.OffsetCurveExtensionType.Extend, New Geometry.Tolerance(1, 1))
					sTest = "baa2"
					oHatchLoop = GeoUtilites.RingToLoop(oRing)
					sTest = "baa3"
					oHatchLoopBorder = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)
					sTest = "bb"

					Try
						sTest = "gbb"
						MessageBox.Show(CStr(oHatchLoop.Polyline.Count) & ":" & CStr(oHatchLoopBorder.Polyline.Count), "12_742")
						oHatch.AppendLoop(oHatchLoop)
						oHatch.AppendLoop(oHatchLoopBorder)
						sTest = "gc"
						oHatch.EvaluateHatch(True)
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
					End Try
					Exit For
				Next
				oTopology.Close()
				oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
				sTest = "g"
				oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
				Try
					oHatch.UpgradeOpen()

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
				End Try

				'  oHatch.Associative = True
				oTransaction.Commit()
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
			Finally
				oTransaction.Dispose()
			End Try


		End Sub
		Private Sub zzAddHatchBorderNew(ByVal tColor As DMColor)
			Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			Dim oDrawOrderTable As Autodesk.AutoCAD.DatabaseServices.DrawOrderTable
			Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId

			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

			Dim oHatchLoopBorder As Autodesk.AutoCAD.DatabaseServices.HatchLoop
			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try
				oHatch.Associative = False
				MessageBox.Show(CStr(tColor.AcadColorIndex), "tColor.AcadColorIndex 12_034zz!! zzAddHatchBorderNew")
				oHatch.ColorIndex = tColor.AcadColorIndex + 1

				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
				oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
				oTransaction = oTransactionManager.StartTransaction()

				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

				sTest = "b"
				tDrawOrderTableID = oBlockTableRecord.DrawOrderTableId
				oDrawOrderTable = DirectCast(oTransactionManager.GetObject(tDrawOrderTableID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.DrawOrderTable)

				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCompositeInner As Autodesk.AutoCAD.Geometry.CompositeCurve2d


				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				Dim dOffsetDist As Double
				If True Then
					dOffsetDist = 0.6
				Else
					dOffsetDist = -0.6
				End If
				'	oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
				oHatchLoop = moaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)

				sTest = "baa"
				oComposite = moaBulgeVertexArray(0).GetCompositeCurve(True)
				sTest = "baa1"

				System.Windows.Forms.MessageBox.Show(CStr(oComposite.IsClosed()), "!!!!!oComposite.IsClosed 12_2160c zzAddHatchBorderNew")
				sTest = "baa1a"
				oCurves = oComposite.GetTrimmedOffset(dOffsetDist, Geometry.OffsetCurveExtensionType.Extend, New Geometry.Tolerance(0.1, 0.1))
				sTest = "baa2"
				Try
					oCompositeInner = DirectCast(oCurves(0), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					sTest = "bba1"
					If Not oCompositeInner.IsClosed Then
						sTest = "bba2"
						oCurves(0) = GeoUtilites.CloseCompositeCurve(oCompositeInner)
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), "oComposite.IsClosed 12_4423 zzAddHatchBorderNew")
					Else
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), " 12_3167 zzAddHatchBorderNew")
					End If
					sTest = "bba3"
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
				End Try

				Try
					sTest = "gbb"
					oHatch.AppendLoop(oHatchLoop)
					sTest = "gc"
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=232bNew")
				End Try

				If oCurves Is Nothing OrElse oCurves.GetUpperBound(0) = -1 Then
					If oCurves Is Nothing Then
						MessageBox.Show("oCurves Is Nothing ", "12_870")
					Else
						MessageBox.Show(CStr(oCurves.GetUpperBound(0)), "12_873")
					End If

				Else

					oHatchLoopBorder = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)
					MessageBox.Show(CStr(oHatchLoop.Polyline.Count) & ":" & CStr(oHatchLoopBorder.Polyline.Count), "12_744z")

					oHatch.AppendLoop(oHatchLoopBorder)
				End If
				oHatch.EvaluateHatch(True)
				sTest = "baa3"
				sTest = "bb"
				oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
				sTest = "g"
				oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
				Try
					oHatch.UpgradeOpen()

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11_bnew")
				End Try

				'  oHatch.Associative = True
				oTransaction.Commit()
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=moaBulgeVertex.GetUpperBound(0).ToString()!!!")
			Finally
				oTransaction.Dispose()
			End Try


		End Sub
		Private Sub zzAddHatchBorderNewOf(ByVal tColor As DMColor)
			Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
			Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
			Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
			Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
			Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop

			Dim oHatchLoopBorder As Autodesk.AutoCAD.DatabaseServices.HatchLoop


			Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()

			Try
				oHatch.Associative = False
				MessageBox.Show(CStr(tColor.AcadColorIndex), "tColor.AcadColorIndex 12_034zz!! zzAddHatchBorderNew")
				oHatch.ColorIndex = tColor.AcadColorIndex + 1

				oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
				oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
				oTransaction = oTransactionManager.StartTransaction()
				'	AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)
				sTest = "b"

				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCompositeInner As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				Dim oOffsetCurve2d As Autodesk.AutoCAD.Geometry.OffsetCurve2d
				ReDim oCurves(0)

				'	oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline)
				oHatchLoop = moaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)

				sTest = "baa"
				oComposite = moaBulgeVertexArray(0).GetCompositeCurve(True)
				sTest = "baa1"

				System.Windows.Forms.MessageBox.Show(CStr(oComposite.IsClosed()), "oComposite.IsClosed 12_2143zzAddHatchBorderNew")
				sTest = "baa1a"
				'oCurves = oComposite.GetTrimmedOffset(0.6, Geometry.OffsetCurveExtensionType.Chamfer, New Geometry.Tolerance(0.1, 0.1))
				sTest = "baa2"
				GeoUtilites.CurvesInfo(oComposite.GetCurves(), "Before")
				Try
					oOffsetCurve2d = New Autodesk.AutoCAD.Geometry.OffsetCurve2d(oComposite, 1)
					sTest = "bba0"
					oCompositeInner = DirectCast(oOffsetCurve2d.Curve, Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					oCurves(0) = oCompositeInner
					'oCompositeInner = DirectCast(oCurves(0), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					sTest = "bba1"
					System.Windows.Forms.MessageBox.Show(oOffsetCurve2d.Curve.GetType().ToString(), "12_953")
					GeoUtilites.CurvesInfo(oCompositeInner.GetCurves(), "After")
					'	If Not oCompositeInner.IsClosed() Then
					sTest = "bba2"
					oCurves(0) = GeoUtilites.CloseCompositeCurve(oCompositeInner)
					sTest = "bba2a"
					System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), "oComposite.IsClosed 12_4421 zzAddHatchBorderNew")
					'	Else
					'	System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), " 12_3274 zzAddHatchBorderNew")
					'	End If
					sTest = "bba3"
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
				End Try


				Try
					sTest = "gbb"

					oHatch.AppendLoop(oHatchLoop)

					sTest = "gc"

				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=232bNew")
				End Try

				If oCurves Is Nothing OrElse oCurves.GetUpperBound(0) = -1 Then
					If oCurves Is Nothing Then
						MessageBox.Show("oCurves Is Nothing ", "12_870")
					Else
						MessageBox.Show(CStr(oCurves.GetUpperBound(0)), "12_873")
					End If

				Else

					oHatchLoopBorder = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)
					sTest = "bab4"
					MessageBox.Show(CStr(oHatchLoop.Polyline.Count) & ":" & CStr(oHatchLoopBorder.Polyline.Count), "12_743z")

					oHatch.AppendLoop(oHatchLoopBorder)
				End If
				oHatch.EvaluateHatch(True)
				sTest = "baa3"
				sTest = "bb"
				oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
				sTest = "g"
				oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
				Try
					oHatch.UpgradeOpen()

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11_bnew")
				End Try

				'  oHatch.Associative = True
				oTransaction.Commit()
				''''	AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=moaBulgeVertex.GetUpperBound(0).ToString()!!!")
			Finally
				oTransaction.Dispose()
			End Try


		End Sub
      Private Sub zzAddHatchRCurveAAA(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
         Dim colRings As RingCollection
         '   Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
         Dim oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d

         Try
            System.Windows.Forms.MessageBox.Show("", "501")
            oHatch.Associative = False

            oHatch.ColorIndex = 5

            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()
            System.Windows.Forms.MessageBox.Show("", "510")
            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
            System.Windows.Forms.MessageBox.Show("", "520")
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

            sTest = "b"
            colRings = moPolygon.GetBoundary()
            Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d
            System.Windows.Forms.MessageBox.Show("", "522")
            For Each oRing As Ring In colRings
               oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default)
               System.Windows.Forms.MessageBox.Show(oRing.IsExterior.ToString(), "523")
               sTest = "ba"
               oaCurves = GeoUtilites.RingToCurves(oRing)
               sTest = "bb"
               System.Windows.Forms.MessageBox.Show("", "523a")
               If oaCurves Is Nothing Then
                  System.Windows.Forms.MessageBox.Show("STOP", "STOP523b")
               Else
                  sTest = "bc"
                  System.Windows.Forms.MessageBox.Show(CStr(oaCurves.GetUpperBound(0)), "523b")

               End If
               sTest = "bd"
               If oRing.IsExterior Then
                  sTest = "be"
                  oCompositeCurve = New Autodesk.AutoCAD.Geometry.CompositeCurve2d(oaCurves)
                  sTest = "bf"
                  Dim oaInnerCurves() As Autodesk.AutoCAD.Geometry.Curve2d = oCompositeCurve.GetTrimmedOffset(10, Autodesk.AutoCAD.Geometry.OffsetCurveExtensionType.Extend)
                  sTest = "bg"
                  System.Windows.Forms.MessageBox.Show(oaInnerCurves.GetUpperBound(0).ToString(), "!!!!599")

                  zzDispCurves(oaInnerCurves, "TrimmedOffset")
               End If
               System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Curves Is Nothing) & ":" & CStr(oHatchLoop.Polyline Is Nothing), "!!!!oHatchLoop.Curves OR Polylines Is Nothing Default-1")
               System.Windows.Forms.MessageBox.Show(CStr(oaCurves.GetUpperBound(0)), "524")
               zzDispCurves(oaCurves, "!!!Main")
               For iIndex As Integer = 0 To oaCurves.GetUpperBound(0) - 1 '''''!!!!!!!!!!!!!  
                  Try
                     oHatchLoop.Curves.Add(oaCurves(iIndex))

                  Catch oEx As Autodesk.AutoCAD.Runtime.Exception
                     System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & sTest, "zzAddHatch=15")
                  End Try
               Next
               Try

                  sTest = "gbb"
                  oHatch.AppendLoop(oHatchLoop)
                  sTest = "gc"
                  System.Windows.Forms.MessageBox.Show("", "579a")
                  oHatch.EvaluateHatch(True)
                  System.Windows.Forms.MessageBox.Show("", "590")
               Catch oEx As System.Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
               End Try
            Next

            oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            '  oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


      End Sub
      Private Sub zzAddHatchRVert(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
         Dim colRings As RingCollection
         Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex


         Try
            System.Windows.Forms.MessageBox.Show("", "501")
            oHatch.Associative = False

            oHatch.ColorIndex = 5

            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()
            System.Windows.Forms.MessageBox.Show("", "510")
            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
            System.Windows.Forms.MessageBox.Show("", "520")
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

            sTest = "b"
            colRings = moPolygon.GetBoundary()
            Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d
            System.Windows.Forms.MessageBox.Show("", "522")
            For Each oRing As Ring In colRings
               oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Polyline Or Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External)
               System.Windows.Forms.MessageBox.Show(oRing.IsExterior.ToString(), "523")
               sTest = "ba"
               oHatchLoop = GeoUtilites.RingToLoop(oRing)
               sTest = "bb"
               System.Windows.Forms.MessageBox.Show("", "523a")

               sTest = "bd"
               If oRing.IsExterior And False Then
                  sTest = "be"
                  oCompositeCurve = GeoUtilites.VerticesToCompositeCurve(oaBulgeVertices)
                  sTest = "bf"
                  Dim oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d = oCompositeCurve.GetTrimmedOffset(10, Autodesk.AutoCAD.Geometry.OffsetCurveExtensionType.Extend)

                  sTest = "bg"
                  System.Windows.Forms.MessageBox.Show(oaCurves.GetUpperBound(0).ToString(), "!!!!599")

                  zzDispCurves(oaCurves, "GetTrimmedOffset")
               End If


               Try

                  sTest = "gbb"
                  oHatch.AppendLoop(oHatchLoop)
                  sTest = "gc"
                  System.Windows.Forms.MessageBox.Show("", "579a")
                  oHatch.EvaluateHatch(True)
                  System.Windows.Forms.MessageBox.Show("", "590")
               Catch oEx As System.Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
               End Try
            Next

            oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            '  oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


      End Sub
      Private Sub zzAddHatchN(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch)
         Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
         Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
         Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
         Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
         Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
         Dim oAcObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
         Dim sTest As String = "a"
         Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
         Dim oInnerHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop = Nothing

         Dim colRings As RingCollection
         Dim oaBulgeVertices() As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray
         Dim oInnerBulgeVertexArray As GeoUtilites.BulgeVertexArray


         Try
            oHatch.Associative = False
            oHatch.ColorIndex = 5
            oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
            oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal
            oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            oTransaction = oTransactionManager.StartTransaction()

            oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)

            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)

            sTest = "b"
            colRings = moPolygon.GetBoundary()
            Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d

            For Each oRing As Ring In colRings
               oBulgeVertexArray = New GeoUtilites.BulgeVertexArray(oRing)
               oHatchLoop = oBulgeVertexArray.CreateHatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.External)
               System.Windows.Forms.MessageBox.Show(oRing.IsExterior.ToString(), "523")
               sTest = "ba"
               '      oHatchLoop = GeoUtilites.RingToLoop(oRing)
               sTest = "bb"
               System.Windows.Forms.MessageBox.Show("", "523a")

               If oRing.IsExterior Then
                  sTest = "be"
                  oCompositeCurve = GeoUtilites.VerticesToCompositeCurve(oBulgeVertexArray.GetBulgeVertexArrayOpened())
                  sTest = "bf"
                  Dim oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d = oCompositeCurve.GetTrimmedOffset(-60, Autodesk.AutoCAD.Geometry.OffsetCurveExtensionType.Extend)
                  sTest = "bg"
                  oInnerBulgeVertexArray = New GeoUtilites.BulgeVertexArray(oaCurves)
                  sTest = "bi"
                  oInnerHatchLoop = oInnerBulgeVertexArray.CreateHatchLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default)

                  sTest = "bk"
                  System.Windows.Forms.MessageBox.Show(oaCurves.GetUpperBound(0).ToString(), "!!!!599N")

                  zzDispCurves(oaCurves, "GetTrimmedOffset")
               End If


               Try

                  sTest = "gbb"
                  System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline.Count) & ":" & CStr(oHatchLoop.LoopType), "579a")
                  '''''''''''  oHatch.AppendLoop(oHatchLoop)
                  sTest = "gc"
                  If oInnerHatchLoop IsNot Nothing Then
                     System.Windows.Forms.MessageBox.Show(CStr(oInnerHatchLoop.Polyline.Count) & ":" & CStr(oInnerHatchLoop.LoopType), "579b")

                     sTest = "gca"
                     oHatch.AppendLoop(oHatchLoop)
                     oHatch.AppendLoop(oInnerHatchLoop)

                  End If
                  sTest = "gd"
                  oHatch.EvaluateHatch(True)
                  System.Windows.Forms.MessageBox.Show("", "590")
               Catch oEx As System.Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
               End Try
            Next

				oAcObjId = New Autodesk.AutoCAD.DatabaseServices.ObjectId(New System.IntPtr(0))	 ', oBlockTableRecord.AppendEntity(oHatch)
            System.Windows.Forms.MessageBox.Show("", "540")
            sTest = "g"
            oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
            System.Windows.Forms.MessageBox.Show("", "550")
            Try
               oHatch.UpgradeOpen()

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
            End Try
            System.Windows.Forms.MessageBox.Show("", "979")
            '  oHatch.Associative = True
            System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
            oTransaction.Commit()
            System.Windows.Forms.MessageBox.Show("", "592")
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
         Finally
            oTransaction.Dispose()
         End Try


      End Sub
      Private Function zzGetTopoLinks(ByVal sTopoName As String) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
         Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

         Dim oTopoModel As TopologyModel

         Dim iTopologyType As TopologyTypes = TopologyTypes.Polygon
         Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
         Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
         Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection
         Dim colRings As RingCollection
         Dim colHalfEdges As HalfEdgeCollection
         Dim oFullEdge As FullEdge
         Dim oObjectID As Autodesk.AutoCAD.DatabaseServices.ObjectId
         If oTopos.Exists(sTopoName) Then
            oTopoModel = oTopos(sTopoName)
            Try
               oTopoModel.Open(OpenMode.ForRead)
               colPolygons = oTopoModel.GetPolygons()
               For Each oPolygon In colPolygons
                  colRings = oPolygon.GetBoundary()
                  For Each oRing As Ring In colRings
                     colHalfEdges = oRing.GetEdges()
                     For Each oHalfEdge As HalfEdge In colHalfEdges
                        oFullEdge = oHalfEdge.FullEdge
                        oObjectID = oFullEdge.Entity
                        If Not colLinks.Contains(oObjectID) Then
                           colLinks.Add(oObjectID)
                        End If
                     Next
                  Next
               Next
               oTopoModel.Close()
               Return colLinks
            Catch oMapEx As Autodesk.Gis.Map.MapException
               AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzGetTopoLinks")
               oTopoModel.Close()
               Return Nothing
            End Try

         Else
            Return Nothing
         End If
      End Function

      Private Sub zzDispPoint(ByVal oPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal sName As String)
         Dim sText As String = sName & ": " & CStr(oPoint.X) & "," & CStr(oPoint.Y) & vbCrLf
         AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
      End Sub
      Private Sub zzDispCurves(ByVal oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d, ByVal sTitle As String)
         Dim sText As String
         Dim sType As String
         Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d
         AcadReport.AcadUtil.GetEditor().WriteMessage(sTitle & vbCrLf)
         sText = "Curves " & oaCurves.GetUpperBound(0).ToString() & vbCrLf
         AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
         For iIndex As Integer = 0 To oaCurves.GetUpperBound(0)
            zzDispPoint(oaCurves(iIndex).StartPoint, "StartPoint " & iIndex.ToString())
            zzDispPoint(oaCurves(iIndex).EndPoint, "EndPoint " & iIndex.ToString())
            sType = oaCurves(iIndex).GetType().ToString()
            sText = sType & vbCrLf
            AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
            If sType = "Autodesk.AutoCAD.Geometry.CompositeCurve2d" Then
               oCompositeCurve = DirectCast(oaCurves(iIndex), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
               AcadReport.AcadUtil.GetEditor().WriteMessage("Composite No " & CStr(iIndex) & vbCrLf)
               zzDispCurves(oCompositeCurve.GetCurves(), sTitle & CStr(iIndex))

            End If
         Next
         AcadReport.AcadUtil.GetEditor().WriteMessage("-----------------" & vbCrLf)
      End Sub
		Protected Sub OnTerminate()
			If mcolRings IsNot Nothing Then
				mcolRings.Dispose()
				mcolRings = Nothing
			End If

			If moPolygon IsNot Nothing Then
				''''  moPolygon.Dispose()
				'''''   GC.SuppressFinalize(moPolygon)
				''''   moPolygon = Nothing

			End If

			If dcolBoundaryObj IsNot Nothing Then
				Try
					dcolBoundaryObj.Clear()
					dcolBoundaryObj.Dispose()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - OnTerminate")
				End Try

				dcolBoundaryObj = Nothing
			End If
			If doBoundingBox IsNot Nothing Then
				doBoundingBox = Nothing
			End If
		End Sub
      Protected Overrides Sub Finalize()
         MyBase.Finalize()
      End Sub
   End Class
   
End Namespace
