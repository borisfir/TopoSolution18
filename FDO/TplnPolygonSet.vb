Option Explicit On
Option Strict On
Imports OSGeo.FDO
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Enum enPolygonSetSource
	NotDefined
	Shapes
	Polylines
End Enum
Public Enum enRelationType
   NotExists
   Exterior
   Interior
   Neigbour

End Enum

Public Class TplnPolygonSet

	''''''''''''PgonSetCREATOR
   Const msAppPrefix As String = "CPTopo_"
   Public Const AcObjIDFieldName As String = "AcObjID"
   Public Const CheckAcObjIDFieldName As String = "AcObjID_C"

   Public Const EntityNoFieldName As String = "EntityNo"
   Const msCheckEntityNoFieldName As String = "EntityNo_C"

   Const msIDFieldName As String = "ID"
   Const msEntityTypeFieldName As String = "Type"
   Const msStatusFieldName As String = "Status"
   Public Const IsProperFieldName As String = "IsProper"

   Public Const msGroupIDFieldName As String = "GroupID"
   Public Const msGroupAddIDFieldName As String = "GroupAddID"

   Const msCheckGroupIDFieldName As String = "GroupID_C"
   Const msCheckGroupAddIDFieldName As String = "GroupAddID_C"



   Const msAttribInt1FieldName As String = "AttribInt1"

   Public Const XminFieldName As String = "Xmin"
   Public Const XmaxFieldName As String = "Xmax"
   Public Const YminFieldName As String = "Ymin"
   Public Const YmaxFieldName As String = "Ymax"
   Public Const XFieldName As String = "X"
   Public Const YFieldName As String = "Y"
   Const msHandleFieldName As String = "Handle"

   Public Const msCentroidCountFieldName As String = "CentroidCount"
   Public Const msCentroidAcObjIDFieldName As String = "CentroidAcObjID"


   Public Const AreaNameFieldName As String = "Area"
   Public Const LengthNameFieldName As String = "Length"



   Const msIntersectCountFieldName As String = "IntersectCount"

   Public Const msNameFieldName As String = "Name"

   Const msCheckNameFieldName As String = "Name_C"
   Public Const msOrderFieldName As String = "Order"

   Const msParentNameFieldName As String = "ParentName"

   '  Const msParentNoFieldName As String = "ParentNo"
   ' Const msParentNoAddFieldName As String = "ParentNoAdd"

   Const msLegalAreaNameFieldName As String = "LegalArea"






   Const msNeigborListFieldName As String = "NeigborList"
   Const msExteriorFieldName As String = "Exterior"
   Const msInteriorListFieldName As String = "InteriorList"
   Const msInteriorLinesObjIDFieldName As String = "InteriorLinesObjID"

   Const mdToler As Double = 0.001
   Const mbExcludeCrossing As Boolean = False
   Private Shared mcolLastActIntersectPoints As Point3dCollection
   Private msName As String
   Private miMapTheme As DMAcadExt.enMapTheme
   Private miTopoPurpose As DMAcadExt.enTopoPurpose
   Private moLinkTable As System.Data.DataTable
   Private moCentroidTable As System.Data.DataTable
   Private moIntersectionsTable As System.Data.DataTable
   '   Private moCentroidDataTable As CentroidTable
   Private mdicCentroids As IDictionary(Of GroupKey, ObjectId)
   Private mdicCentroidsOld As IDictionary(Of Integer, ObjectId)

   Private mcolPolylineIDs As ObjectIdCollection
   Private mdicPolylines As IDictionary(Of ObjectId, Polyline)
   Private mdicPolygons As IDictionary(Of Integer, ObjectId)
   Private mdicSetCentroids As IDictionary(Of Integer, ObjectId)

   Private mdicMPolygons As IDictionary(Of ObjectId, MPolygon)
   Private mdicBlocks As IDictionary(Of ObjectId, BlockReference)
   ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

   ''' '''''''''NEW 
   Private miFeatureIDMin As Integer
   Private miFeatureIDMax As Integer
   Private miaFeaturesID() As Integer
   Private mcolOuterPolylineIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private miaCentroidIndecis() As Integer
   Private mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private mdicParcelData As Dictionary(Of Integer, ParcelData)
   Private mdicImpMPgons As IDictionary(Of Integer, MPolygon)
   Private mdSumMPgonArea As Double
   '''''''''''''''''''''''''''''''
   Private Shared moNodes As TopoManager.tmNodes
   '	Private moaTopoPolygons() As TopoManager.tmPolygon
   Private mdicTmPolygons As TopoManager.tmPolygons
   Private mdicTopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)
   Private mdicGushFromParcelData As Dictionary(Of Integer, GushData) = Nothing
   Private miTopoIDCounter As Integer = 0
   Private mbParcel As Boolean
   Private miPgonImported As Integer
   Private macolMPgonObjIDsAAA() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private moMPgonsByGush As MPgonsByGush
   '	Dim mdicGushObjIDCollections As IDictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
   Private miSource As enPolygonSetSource = enPolygonSetSource.NotDefined
   Private moLinkDataTable As LinkTable

   Private mdicMPgons As IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, MPolygonExt)
   Private moaBaseMPgons() As Autodesk.AutoCAD.DatabaseServices.ObjectId
   Private miBaseMPgonsUB As Integer
   Private mbHasBlockAdd As Boolean
   Private mtShapeFOData As FDO_Manager.ShapeFOData
   Private Shared mfMPgonView As frmMPgonView
   Private Shared miaBlockAttribIndex() As Integer
   Private moWorkAreaBound As Polyline
   Public Shared Sub PgonView()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polygon ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntity As DBObject = Nothing

      Dim oResBuffer As ResultBuffer
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      '	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)



      TopoManager.Common.SetAcadFocus()
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oEntity = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         If oEntity IsNot Nothing Then
            oResBuffer = oEntity.XData
            oXDataParcel = New DMAcadExt.TplnXDataParcel(oResBuffer)
         End If

      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      If oXDataParcel IsNot Nothing Then
         mfMPgonView = New frmMPgonView(oXDataParcel)
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, mfMPgonView)
      End If

   End Sub
   Public Shared Sub PgonList()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polygon ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing

      Dim oResBuffer As ResultBuffer
      Dim oXDataBasePgon As DMAcadExt.TplnXDataBasePgon = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      '	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)




      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         If oEnt IsNot Nothing Then
            oResBuffer = oEnt.XData
            oXDataBasePgon = New DMAcadExt.TplnXDataParcel(oResBuffer)
            If oXDataBasePgon.HasData Then
               oXDataBasePgon.PrintList()
            End If
            oXDataBasePgon = New DMAcadExt.TplnXDataLot(oResBuffer, "") ' ????????????
            If oXDataBasePgon.HasData Then
               oXDataBasePgon.PrintList()
            End If

         End If

      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Public Shared Function PolygonRelation(oPolyline As Polyline, oPolylineA As Polyline, bCheckIntersection As Boolean) As PgonRelation
      Dim oMPolygon As MPolygon = New MPolygon
      Dim oMPolygonA As MPolygon = New MPolygon
      Dim sMsg As String
      Try
         oMPolygon.AppendLoopFromBoundary(oPolyline, mbExcludeCrossing, mdToler)
         oMPolygon.BalanceTree()
      Catch oEx As Exception
      End Try
      If oMPolygon.IsBalanced Then
         Try
            oMPolygonA.AppendLoopFromBoundary(oPolylineA, mbExcludeCrossing, mdToler)
            oMPolygonA.BalanceTree()
         Catch oEx As Exception
         End Try
         If oMPolygonA.IsBalanced Then
            Dim oPgonRelation As PgonRelation = New PgonRelation(oPolyline, oPolylineA)
            oPgonRelation.Calculate(bCheckIntersection, True)
            mcolLastActIntersectPoints = oPgonRelation.IntersectPoints
            ''''''''''   oPgonRelation.DrawErrorPoints()
            PgonRelation.RegApp()
            If Not oPgonRelation.IsTouch Then
               oPgonRelation.PrintIntersectPoints(1, 2)
               oPgonRelation.MarkIntersectionPoints(False)
            End If

            sMsg = oPgonRelation.ErrorStatus
            If oPgonRelation.ErrorPointsCount > 0 Then
               sMsg &= CStr(oPgonRelation.ErrorPointsCount) & " Intersection Point(s)"
               DMAcadExt.AcadDocument.WriteMessage(sMsg)
               For iIndex As Integer = 0 To oPgonRelation.ErrorPointsCount - 1
                  '''''''DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & ": " & oPgonRelation.ErrorPoint(iIndex).Coordinates2d)
               Next
            End If
            DMAcadExt.AcadDocument.WriteMessage("Relation - " & oPgonRelation.RelationType.ToString())
            Return oPgonRelation
         Else
            DMAcadExt.AcadDocument.WriteMessage("2nd Polygon Is incorrect")
         End If
      Else
         DMAcadExt.AcadDocument.WriteMessage("1st Polygon Is incorrect")
      End If
      Return Nothing
   End Function
   Public Shared Sub PolygonRelation(oPolyline As Polyline, oBlockRef As BlockReference)
      Dim oMPolygon As MPolygon = New MPolygon()
      Dim iExteriorIndex As Integer
      Try
         oMPolygon.AppendLoopFromBoundary(oPolyline, False, mdToler)
         oMPolygon.BalanceTree()
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         DMAcadExt.AcadDocument.WriteMessage(oPolyline.Handle.ToString() & " Err#08=" & oAcadEx.ErrorStatus.ToString())

      End Try
      If oMPolygon.IsBalanced Then
         For iIndex As Integer = 0 To oMPolygon.NumMPolygonLoops - 1
            If oMPolygon.GetLoopDirection(iIndex) = LoopDirection.Exterior Then
               iExteriorIndex = iIndex
               Exit For
            End If

         Next
         Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d = oBlockRef.Position
         Dim iaLoopInd As IntegerCollection = oMPolygon.IsPointInsideMPolygon(tPoint, mdToler)
         Dim sMsg As String
         If iaLoopInd IsNot Nothing AndAlso iaLoopInd.Count > 0 Then
            If iaLoopInd.Item(0) = iExteriorIndex Then
               sMsg = "Inside"
            Else
               sMsg = "Outside"
            End If
         Else
            sMsg = "Outside"
         End If
         DMAcadExt.AcadDocument.WriteMessage("Relation - " & sMsg)
      Else
         DMAcadExt.AcadDocument.WriteMessage(oPolyline.Handle.ToString() & " Err#12 - Polygon is incorrect")
      End If

   End Sub
   Public Shared Sub MarkIntersectionPoints()
      Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
      Dim iIndex As Integer = 0
      Dim iLastIndex As Integer = mcolLastActIntersectPoints.Count - 1
      Dim shColorIndex As Short

      For Each tPoint As Point3d In mcolLastActIntersectPoints
         If iIndex = 0 OrElse iIndex = iLastIndex Then
            shColorIndex = 4S
         Else
            shColorIndex = 4S
         End If

         oCircleMarkBlock.MarkPoint(tPoint, shColorIndex)
         iIndex += 1
      Next

   End Sub
	Public Sub SetDataInsertBlock(hsGush As HashSet(Of Integer), dicGushFromParcelData As Dictionary(Of Integer, GushData), ByRef tShapeFOData As FDO_Manager.ShapeFOData, tFilterList As DMCommon.dmList, bInnerPolygons As Boolean)
		If miMapTheme = DMAcadExt.enMapTheme.Parcels Then

			SetDataInsertBlock_Parcel(hsGush, dicGushFromParcelData, tShapeFOData, tFilterList, bInnerPolygons)
		ElseIf miMapTheme = DMAcadExt.enMapTheme.Blocks Then
			TopoManager.TPlanGraph.TplnBlock.Initialize("1601")
			SetDataInsertBlock_Gush(tShapeFOData, "1601", "1601", tFilterList)
		ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved Then
			SetDataInsertBlock_Lot(tShapeFOData)
		ElseIf miMapTheme = DMAcadExt.enMapTheme.PlanApproved Then
			SetDataInsertBlock_Plan(tShapeFOData)
		End If
	End Sub
	Public Sub Load(colPolylineIDs As ObjectIdCollection, colCentroidsIDs As ObjectIdCollection)
      mcolPolylineIDs = colPolylineIDs
      mcolCentroids = colCentroidsIDs
      'Dim oEntity As Entity
      Dim oDBObject As DBObject
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      mdicPolygons = New Dictionary(Of Integer, ObjectId)
      mdicSetCentroids = New Dictionary(Of Integer, ObjectId)

      For Each tAcObjID As ObjectId In mcolPolylineIDs
         oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
         oXDataParcel = New DMAcadExt.TplnXDataParcel(oDBObject.XData)
         mdicPolygons.Add(oXDataParcel.ID, tAcObjID)
      Next
      For Each tAcObjID As ObjectId In mcolCentroids
         oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
         oXDataParcel = New DMAcadExt.TplnXDataParcel(oDBObject.XData)
         mdicSetCentroids.Add(oXDataParcel.ID, tAcObjID)
      Next
      System.Windows.Forms.MessageBox.Show(CStr(mdicPolygons.Count) & vbCrLf & CStr(mdicSetCentroids.Count), "08_398")
   End Sub
   Public Sub AddPolylineIDs(colPolylineIDs As ObjectIdCollection)
      mcolPolylineIDs = colPolylineIDs
      ' System.Windows.Forms.MessageBox.Show(CStr(mcolPolylineIDs.Count) & vbCrLf & CStr(colPolylineIDs.Count), "08_345")
      zzFillLinkTable()
   End Sub
   Public ReadOnly Property SumMPgonArea As Double
      Get
         Return mdSumMPgonArea
      End Get
   End Property
   Public ReadOnly Property ShapeData As FDO_Manager.ShapeFOData
      Get
         Return mtShapeFOData
      End Get
   End Property
   Public ReadOnly Property TmPolygons As TopoManager.tmPolygons
      Get
         Return mdicTmPolygons
      End Get
   End Property

   Public ReadOnly Property SetMapTheme As DMAcadExt.enMapTheme
      Get
         Return miMapTheme
      End Get
   End Property
   Public ReadOnly Property PointsOutside As DataView
      Get
         If moCentroidTable IsNot Nothing Then
            Dim sFilter As String = msStatusFieldName & " = False"
            Dim tAcObjID As ObjectId
            Dim oPointsOutside As DataView = New Data.DataView(moCentroidTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
            Dim oParcel As TopoManager.TPlanGraph.TplnParcel
            Dim oLot As TopoManager.TPlanGraph.TplnLot
            Dim oBlock As TopoManager.TPlanGraph.TplnBlock

            For Each oRow As DataRowView In oPointsOutside
               tAcObjID = DirectCast(oRow.Item(AcObjIDFieldName), ObjectId)
               With oRow
                  If miMapTheme = DMAcadExt.enMapTheme.Parcels Then
                     oParcel = New TopoManager.TPlanGraph.TplnParcel(Nothing, 0, tAcObjID)
                     .Item(msGroupIDFieldName) = oParcel.BlockNo
                     If oParcel.BlockAdd <> 0 Then
                        .Item(msGroupAddIDFieldName) = oParcel.BlockAdd
                        mbHasBlockAdd = True
                     End If

                     .Item(msNameFieldName) = oParcel.Name
                     .Item(msOrderFieldName) = oParcel.Order
                     .Item(msLegalAreaNameFieldName) = oParcel.LegalArea(False)
                  ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved Then
                     oLot = New TopoManager.TPlanGraph.TplnLot(miTopoPurpose, Nothing, 0, tAcObjID)
                     If oLot.Name IsNot Nothing Then
                        .Item(msNameFieldName) = oLot.Name
                     End If
                     .Item(msAttribInt1FieldName) = oLot.LanduseID
                     .Item(msOrderFieldName) = oLot.Order
                  ElseIf miMapTheme = DMAcadExt.enMapTheme.Blocks OrElse miMapTheme = DMAcadExt.enMapTheme.UD_Blocks Then
                     oBlock = New TopoManager.TPlanGraph.TplnBlock(tAcObjID)

                  End If

               End With


               '  System.Windows.Forms.MessageBox.Show(CStr(oParcel.BlockNo) & vbCrLf & oParcel.Name, "04_211")
            Next
            Return oPointsOutside
         Else
            System.Windows.Forms.MessageBox.Show("Centroid Table was not found", "04_124")
            Return Nothing
         End If

      End Get



   End Property

   Public Sub SetFeatureIDMin()
      Dim oResBuffer As ResultBuffer
      Dim oValue As System.Object
      Dim iPgonID As Integer
      Dim iIndex As Integer = 0
      Dim iCentroidIndex As Integer = 0

      Dim iRingType As Integer
      Dim oPolyline As DBObject
      mcolOuterPolylineIDs = New ObjectIdCollection()

      '	System.Windows.Forms.MessageBox.Show(CStr(mcolPolylineIDs.Count) & ":" & CStr(mdicParcelData.Count), "04_194")

      ReDim miaFeaturesID(mcolPolylineIDs.Count - 1)
      ReDim miaCentroidIndecis(mcolPolylineIDs.Count - 1)
      If mcolPolylineIDs Is Nothing Then
         System.Windows.Forms.MessageBox.Show("mcolPolylineIDs Is Nothing", "04_190")
      Else

         For Each oPolylineObjID As ObjectId In mcolPolylineIDs
            oPolyline = DMAcadExt.AcadTransaction.GetDBObject(oPolylineObjID, OpenMode.ForRead)
            oResBuffer = oPolyline.XData
            If oResBuffer IsNot Nothing Then
               Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()

               oValue = taTypedValues(2).Value
               iPgonID = CInt(oValue)
               oValue = taTypedValues(3).Value
               iRingType = CInt(oValue)
               DMAcadExt.AcadDocument.WriteMessage("RingType" & ": " & CStr(iRingType) & " ID=" & CStr(iPgonID))
               If miFeatureIDMin = 0 Then
                  miFeatureIDMin = iPgonID
                  miFeatureIDMax = iPgonID
               Else
                  miFeatureIDMin = Math.Min(miFeatureIDMin, iPgonID)
                  miFeatureIDMax = Math.Max(miFeatureIDMax, iPgonID)
               End If
               'End If
               miaFeaturesID(iIndex) = iPgonID
               '	dicPgonsID.Add(oPolygon.ObjectId, iPgonID)
               If iRingType = 1 Then
                  mcolOuterPolylineIDs.Add(oPolyline.ObjectId)
                  miaCentroidIndecis(iIndex) = iCentroidIndex
                  iCentroidIndex += 1
               Else
                  miaCentroidIndecis(iIndex) = -1
                  oPolyline.Erase()
               End If
            End If
            iIndex += 1
         Next
      End If
   End Sub
   Public Sub SetDataInsertBlock_Gush(ByRef tShapeFOData As FDO_Manager.ShapeFOData, sBlockName As String, sLayerName As String, tFilterList As DMCommon.dmList)
      sLayerName = "1601"
      sBlockName = "1601"
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iTestCounter As Integer
      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim sClassName As String = tShapeFOData.FeatureClass
      Dim dicRes As Dictionary(Of Integer, GushData)
      Dim tGushData As GushData = Nothing
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
      Dim oValue As System.Object
      Dim iPgonID, iPgonIDNew As Integer
      Dim iRingType As Integer
      Dim iFeatureIDMin, iFeatureIDMax As Integer
      Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()

      Dim dicPgonsID As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer) = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer)()
      Dim dicMPgons As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon)()
      Try
         dicRes = GetGushData(tShapeFOData.ShapeFileName, sClassName, tFilterList)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tShapeFOData.ShapeFileName & vbCrLf & sClassName, "04_246")
         dicRes = Nothing
      End Try
      '	Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
      Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = Nothing
      DMAcadExt.AcadTransaction.CreateLayer("TplnGushMPgon")
      If dicRes IsNot Nothing Then
         If bRes Then

            iTestCounter = 0
            lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sClassName, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            Dim iaPgonID(lstPolygons.Count - 1) As Integer
            Dim iaCentroidIndecis(lstPolygons.Count - 1) As Integer
            Dim iCentroidIndex As Integer = 0

            colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               oPolygon = lstPolygons.Item(iIndex)
               iTestCounter += 1
               oResBuffer = oPolygon.XData
               Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()

               oValue = taTypedValues(2).Value
               iPgonID = CInt(oValue)
               oValue = taTypedValues(3).Value
               iRingType = CInt(oValue)

               If iFeatureIDMin = 0 Then
                  iFeatureIDMin = iPgonID
                  iFeatureIDMax = iPgonID
               Else
                  iFeatureIDMin = Math.Min(iFeatureIDMin, iPgonID)
                  iFeatureIDMax = Math.Max(iFeatureIDMax, iPgonID)
               End If
               'End If
               iaPgonID(iIndex) = iPgonID
               dicPgonsID.Add(oPolygon.ObjectId, iPgonID)
               If iRingType = 1 Then
                  colPolylines.Add(oPolygon.ObjectId)
                  iaCentroidIndecis(iIndex) = iCentroidIndex
                  iCentroidIndex += 1
               Else
                  iaCentroidIndecis(iIndex) = -1
               End If
               If Not dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                  oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()

                  oMPolygon.Layer = "TplnGushMPgon"
                  DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
                  '	oEditor.WriteMessage("39@  " & CStr(oMPolygon.Layer) & vbCrLf)
                  ' oMPolygon.Layer = Tpln_LotsKMisCntr
                  dicMPgons.Add(iPgonID, oMPolygon)
               End If
               oMPolygon.AppendLoopFromBoundary(oPolygon, False, 0.001)
            Next



            iTestCounter = 0

            Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
            Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
            '''''''''''''''''''''''	TopoManager.TPlanGraph.TplnBlock.Initialize(mtDissolveMapThemeData)
            '	Dim ia1603AttribIndices() As Integer = {0, 1, 2, 3}
            '	Dim sa1603AttribText(ia1603AttribIndices.GetUpperBound(0)) As String
            '		System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count) & vbCrLf & CStr(dicMPgons.Count), "01_690")
            System.Windows.Forms.MessageBox.Show("SetDataInsertBlock_Gush" & vbCrLf & sLayerName & vbCrLf & sBlockName, "02_331Gush")
            Try
               oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_841q")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - tsiBuildTopo_Click" & ": ")
            End Try
            moMPgonsByGush = New MPgonsByGush()
            Dim oXDataParcel As DMAcadExt.TplnXDataParcel
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               iTestCounter += 1
               tPgonAcObjID = colPolylines.Item(iIndex)
               If iaCentroidIndecis(iIndex) = -1 Then
                  iRingType = -1
               Else
                  iRingType = 1
                  tCentroidAcObjID = colCentroids.Item(iaCentroidIndecis(iIndex))
               End If

               oPolygon = lstPolygons.Item(iIndex)
               'oResBuffer = oPolygon.XData
               oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
               oXDataParcel = New DMAcadExt.TplnXDataParcel()
               '	System.Windows.Forms.MessageBox.Show(CStr(oResBuffer Is Nothing), "05_110 B")
               If oResBuffer IsNot Nothing Then
                  iPgonID = iaPgonID(iIndex)
                  iPgonIDNew = iPgonID - iFeatureIDMin + 1

                  If dicRes.TryGetValue(iPgonIDNew, tGushData) Then
                     miTopoIDCounter += 1
                     oXDataParcel.ID = miTopoIDCounter
                     oXDataParcel.DataID = iPgonIDNew

                     oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels
                     oXDataParcel.Block = tGushData.Block
                     oXDataParcel.BlockAdd = tGushData.BlockAdd

                     DMAcadExt.AcadTransaction.CreateLayer("1602")
                     Try
                        oPolygon.Layer = "1602"
                     Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                        System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "FDO_Manager - zzSetDataInsertBlock")

                     End Try

                     If iRingType = 1 Then
                        '	tGushData.DebugWrite("UpdAttr")
                        TopoManager.TPlanGraph.TplnBlock.Initialize()
                        DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, TopoManager.TPlanGraph.TplnBlock.GetAttribIndex, tGushData.GetAttribText())
                        oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

                        If dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                           oXDataParcel.AcadArea = Math.Abs(oMPolygon.Area)
                           oXDataParcel.Perimeter = oMPolygon.Perimeter
                           oMPolygon.XData = oXDataParcel.GetResBuffer()
                           oCentroidDBObject.XData = oMPolygon.XData
                        End If
                     End If
                  Else
                     If iTestCounter < 5 Then
                        System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iPgonID), "04_271e")
                     End If
                  End If
               Else
                  System.Windows.Forms.MessageBox.Show("XData was not found" & vbCrLf & tPgonAcObjID.ToString() & vbCrLf & oPolygon.GetRXClass().Name, "04_156")
               End If
            Next

            '	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
         Else
            System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & tShapeFOData.GetXDataAppName, "04_272")
         End If
      Else
         System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & tShapeFOData.ShapeFileName, "04_273G")
         oEditor.WriteMessage("76-- Not exists" & ":" & CStr(sClassName) & vbCrLf)
      End If
   End Sub
   Public ReadOnly Property AppName As String
      Get
         Return GetAppName(msName)
      End Get
   End Property
   Public ReadOnly Property SortByName As String
      Get
         Return msGroupIDFieldName & "," & msGroupAddIDFieldName & "," & msOrderFieldName
      End Get
   End Property
   Public Sub CreateCentroids()
      mcolCentroids = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim sLayerName As String = "1603"
      Dim sBlockName As String = "1603"
      '	Dim tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      '	Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      '	Dim ia1603AttribIndices() As Integer = {0, 1, 2, 3}
      '	Dim sa1603AttribText(ia1603AttribIndices.GetUpperBound(0)) As String
      '	System.Windows.Forms.MessageBox.Show(CStr(mcolOuterPolylineIDs.Count) & vbCrLf & CStr(mdicImpMPgons.Count), "01_690")

      If mcolOuterPolylineIDs.Count > 0 Then
         'System.Windows.Forms.MessageBox.Show("CreateCentroids" & ":" & CStr(mcolOuterPolylineIDs.Count) & vbCrLf & sLayerName & vbCrLf & sBlockName, "02_338p")
         'חלקות
         Try
            oMapUtility.CreateCentroids(mcolCentroids, mcolOuterPolylineIDs, sLayerName, sBlockName)
            ' System.Windows.Forms.MessageBox.Show(mcolCentroids.Count.ToString() & ":" & mcolOuterPolylineIDs.Count.ToString() & ":" & mdicImpMPgons.Count.ToString() & vbCrLf & sLayerName & vbCrLf & sBlockName, "04_452")
         Catch oMapEx As Autodesk.Gis.Map.MapException
            System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & CStr(mcolCentroids.Count) & ":" & CStr(mcolOuterPolylineIDs.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_841w")
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "FDO_Manager - tsiBuildTopo_Click" & ": ", False)
         End Try
      Else
         System.Windows.Forms.MessageBox.Show("Polylines=0" & vbCrLf & CStr(mcolOuterPolylineIDs.Count), "02_347B")
      End If
      '	System.Windows.Forms.MessageBox.Show("After InsertBlock" & vbCrLf & CStr(mcolCentroids.Count) & ":" & CStr(mcolOuterPolylineIDs.Count), "02_339A")
      DMAcadExt.AcadTransaction.CreateLayer("1602")

   End Sub

   Public Sub InfoToExcel()
      mdicTmPolygons.InfoToExcel()

   End Sub
   Public Sub CreateMPolygons()

      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
      Dim iRingType As Integer
      Dim iIndex As Integer = 0
      Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim tParcelData As ParcelData = Nothing
      Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = Nothing
      Dim oMPgonDbObj As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing

      Dim oMPgonCentroid As MPgonCentroid = Nothing


      Dim iPgonID, iPgonIDNew As Integer
      '	Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim oCentroidBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
      Dim dSumMPgonArea As Double = 0.0
      Dim iaLoopInd As IntegerCollection
      Dim iLoopInd As Integer
      Dim iLoopDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      '	Dim dicMPgons As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon)()
      Dim dicMPgons As Dictionary(Of Integer, MPgonCentroid) = New Dictionary(Of Integer, MPgonCentroid)()
      Dim oPolylineDBObj As DBObject
      Dim oPolyline As Polyline
      Dim bTest As Boolean

      moMPgonsByGush = New MPgonsByGush()
      DMAcadExt.AcadTransaction.CreateLayer("1602")
      '	Dim iTest As Integer = -1
      moMPgonsByGush = New MPgonsByGush()
      'System.Windows.Forms.MessageBox.Show("START" & vbCrLf & mcolPolylineIDs.Count.ToString(), "04_320A")

      For Each oPolylineObjID As ObjectId In mcolPolylineIDs
         bTest = False
         oPolylineDBObj = DMAcadExt.AcadTransaction.GetDBObject(oPolylineObjID, OpenMode.ForWrite)
         If oPolylineDBObj IsNot Nothing Then
            oPolyline = DirectCast(oPolylineDBObj, Polyline)

            'iTest += 1
            tPgonAcObjID = mcolOuterPolylineIDs.Item(iIndex)

            If miaCentroidIndecis(iIndex) = -1 Then
               iRingType = -1
            Else
               iRingType = 1
               tCentroidAcObjID = mcolCentroids.Item(miaCentroidIndecis(iIndex))
            End If


            oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
            oXDataParcel = New DMAcadExt.TplnXDataParcel()
            '	System.Windows.Forms.MessageBox.Show(CStr(oResBuffer Is Nothing), "05_100 B")
            '	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_706")
            If oResBuffer IsNot Nothing Then
               iPgonID = miaFeaturesID(iIndex)
               iPgonIDNew = iPgonID - miFeatureIDMin + 1

               If mdicParcelData.TryGetValue(iPgonIDNew, tParcelData) Then
                  If iIndex = 0 Then
                     'System.Windows.Forms.MessageBox.Show(CStr(tParcelData.Block) & vbCrLf & CStr(tParcelData.BlockAdd), "01_144")
                     '	tShapeFOData.Block = tParcelData.Block
                     '	tShapeFOData.BlockAdd = tParcelData.BlockAdd
                     mtShapeFOData.Gush = New GushData(0, tParcelData.Block, tParcelData.BlockAdd, tParcelData.Status, 0, tParcelData.LegalArea)
                  End If
                  miTopoIDCounter += 1
                  oXDataParcel.ID = miTopoIDCounter
                  oXDataParcel.DataID = iPgonIDNew
                  oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels
                  oXDataParcel.Block = tParcelData.Block
                  oXDataParcel.BlockAdd = tParcelData.BlockAdd
                  oXDataParcel.Name = tParcelData.ParcelName
                  oXDataParcel.LegalArea = tParcelData.LegalArea


                  oXDataParcel.Status = tParcelData.Status
                  oXDataParcel.NeigborList = String.Empty
                  oXDataParcel.Exterior = 0
                  oXDataParcel.InteriorList = String.Empty
                  oXDataParcel.InteriorCount = 0
                  'System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_707A")
                  Try
                     oPolyline.Layer = "1602"
                  Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                     System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "FDO_Manager - zzSetDataInsertBlock")
                  End Try
                  '	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_710")
                  If dicMPgons.TryGetValue(iPgonIDNew, oMPgonCentroid) Then
                     'System.Windows.Forms.MessageBox.Show(CStr(oMPgonCentroid Is Nothing) & vbCrLf & "", "04_456")
                     oMPolygon = oMPgonCentroid.MPgon
                     oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bTest = True
                  Else

                     oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()
                     oMPolygon.Layer = "TplnParcelMPgon"
                     oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     '	DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
                     '''''''''''''''''''''	oMPolygon.UpgradeOpen()
                     oMPgonCentroid = New MPgonCentroid(oMPolygon)
                     dicMPgons.Add(iPgonIDNew, oMPgonCentroid)
                  End If
                  oMPolygon.BalanceTree()
                  If bTest Then
                     System.Windows.Forms.MessageBox.Show(CStr(oMPolygon.Area), "04_720d")
                  End If

                  If iRingType = 1 Then
                     DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, ParcelData.mia1603AttribIndices, tParcelData.GetAttribText())
                     oCentroidBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                     If dicMPgons.TryGetValue(iPgonIDNew, oMPgonCentroid) Then
                        oMPgonCentroid.Centroid = oCentroidBlockRef
                        'oXDataParcel.AcadArea = Math.Abs(oMPolygon.Area)
                        dSumMPgonArea += Math.Abs(oMPolygon.Area)
                        oXDataParcel.AcadArea = oMPolygon.Area
                        oXDataParcel.Perimeter = oMPolygon.Perimeter
                        '	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_730")
                        '	oMPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Yellow)
                        oPolyline.XData = oXDataParcel.GetResBuffer()

                        '	System.Windows.Forms.MessageBox.Show(CStr(iIndex) & vbCrLf & tCentroidAcObjID.ToString(), "04_740C")
                        'oMPgonDbObj = DMAcadExt.AcadTransaction.GetDBObject(oMPolygon.ObjectId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                        '	oMPgonDbObj.XData = oXDataParcel.GetResBuffer()
                        oCentroidBlockRef.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Blue)
                        '	System.Windows.Forms.MessageBox.Show(CStr(iIndex) & vbCrLf & tCentroidAcObjID.ToString(), "04_741")
                        oCentroidBlockRef.XData = oXDataParcel.GetResBuffer()
                        '	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_749")
                        moMPgonsByGush.Add(tParcelData.Block, oMPolygon.ObjectId)
                        '	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_750")
                     Else
                        '	System.Windows.Forms.MessageBox.Show("#" & CStr(iPgonIDNew), "01_892Z")
                        oPolyline.Erase()
                     End If
                  Else
                     DMAcadExt.AcadDocument.WriteMessage("!RingType" & ": #" & CStr(iPgonIDNew) & " Y=" & CStr(iRingType))
                  End If
                  oCentroidBlockRef = oMPgonCentroid.Centroid
                  If oCentroidBlockRef IsNot Nothing Then
                     iaLoopInd = oMPolygon.IsPointInsideMPolygon(oCentroidBlockRef.Position, mdToler)
                     If iaLoopInd IsNot Nothing AndAlso iaLoopInd.Count > 0 Then   ' OrElse 
                        If oMPolygon.NumMPolygonLoops > 1 Then
                           iLoopInd = iaLoopInd.Item(0)
                           iLoopDir = oMPolygon.GetLoopDirection(iLoopInd)
                           If iLoopDir = LoopDirection.Interior Then
                              oCentroidBlockRef.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                              DMAcadExt.AcadDocument.WriteMessage("Inner" & ": #" & CStr(iPgonIDNew) & " N=" & CStr(iaLoopInd.Count) & "/" & CStr(oMPolygon.NumMPolygonLoops) & "," & iLoopInd.ToString() & "|" & iLoopDir.ToString() & "  " & CStr(oCentroidBlockRef.Position.X) & "," & CStr(oCentroidBlockRef.Position.Y) & vbCrLf)
                           Else
                              DMAcadExt.AcadDocument.WriteMessage("|****" & ": #" & CStr(iPgonIDNew) & "|" & iLoopDir.ToString())
                           End If
                        End If
                     Else
                        oCentroidBlockRef.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        DMAcadExt.AcadDocument.WriteMessage("Point NOT" & ": #" & CStr(iPgonIDNew) & "X,Y=" & CStr(oCentroidBlockRef.Position.X) & "," & CStr(oCentroidBlockRef.Position.Y))
                     End If
                  End If

               Else
                  DMAcadExt.AcadDocument.WriteMessage("ParcelDataNotExists=" & ":" & CStr(iPgonIDNew) & vbCrLf)
               End If
            Else
               System.Windows.Forms.MessageBox.Show("XData was not found" & vbCrLf & tPgonAcObjID.ToString(), "04_156")
            End If
         End If
         iIndex += 1

      Next
      mtShapeFOData.SumMPgonArea = dSumMPgonArea

   End Sub
	'054 8 103 599 leonid Rotfarb
	Public Sub SetDataInsertBlock_Parcel(setGush As HashSet(Of Integer), dicGushFromParcelData As Dictionary(Of Integer, GushData), ByRef tShapeFOData As FDO_Manager.ShapeFOData, tFilterList As DMCommon.dmList, bInnerPolygons As Boolean)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
		Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim iTestCounter As Integer
		Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
		Dim sClassName As String = tShapeFOData.FeatureClass
		Dim dicRes As Dictionary(Of Integer, ParcelData) = Nothing
		Dim tParcelData As ParcelData = Nothing
		Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
		Dim bResRegApp As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
		'?????	oXDataParcel = New DMAcadExt.TplnXDataParcel()
		Dim oValue As System.Object
		Dim iPgonID, iPgonIDNew As Integer
		Dim iRingType As Integer
		Dim sPlinesLayer As String
		Dim iFeatureIDMin, iFeatureIDMax As Integer
		Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()
		Dim bFilterNotExists As Boolean = (setGush Is Nothing) OrElse (setGush.Count = 0)
		Dim dicParcelData As Dictionary(Of Integer, ParcelData) = Nothing
		Dim bRes As Boolean
		'	Dim dicPgonsID As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer) = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer)()
		Dim dicMPgons As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon)()

		Try
			'DMCommon.Debug.MsgBox("09_171!!!", tShapeFOData.ShapeFileName, sClassName, DMCommon.Debug.ColCount(setGush), DMCommon.Debug.ColCount(mdicGushFromParcelData), DMCommon.Debug.ColCount(dicGushFromParcelData), tFilterList.List, tFilterList.Exists)
			bRes = GetParcelData(tShapeFOData.ShapeFileName, sClassName, setGush, tFilterList, dicParcelData, dicGushFromParcelData, True, bInnerPolygons)
			mdicGushFromParcelData = dicGushFromParcelData
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tShapeFOData.ShapeFileName & vbCrLf & sClassName, "04_997Pr")
			dicRes = Nothing
		End Try
		If tFilterList.Exists Then
			Return
		End If
		Return

		'	Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
		Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
		Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = Nothing
		Dim dSumMPgonArea As Double = 0.0
		If tFilterList.Exists Then
			sPlinesLayer = "1602"
		Else
			sPlinesLayer = sClassName
		End If
		DMAcadExt.AcadTransaction.CreateLayer("TplnParcelMPgon")
		If dicRes IsNot Nothing Then
			If bResRegApp Then
				iTestCounter = 0

				lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sPlinesLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)  'XXXXXXXXXXXXXXXXXXXX
				' OUT Dim dicPolygons As System.Collections.Generic.IDictionary(Of ObjectId, Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylinesDic("1602", OpenMode.ForWrite)	'"pclp004"
				DMCommon.Debug.MsgBox("09_870", sClassName, CStr(lstPolygons.Count - 1), CStr(dicRes.Count))

				Dim iaPgonID(lstPolygons.Count - 1) As Integer
				Dim iaRingType(lstPolygons.Count - 1) As Integer
				Dim iaCentroidIndecis(lstPolygons.Count - 1) As Integer
				Dim iCentroidIndex As Integer = 0
				Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
				colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

				For iIndex As Integer = 0 To lstPolygons.Count - 1
					oPolygon = lstPolygons.Item(iIndex)
					oResBuffer = oPolygon.XData
					taTypedValues = oResBuffer.AsArray()
					oValue = taTypedValues(2).Value
					iPgonID = CInt(oValue)
					oValue = taTypedValues(3).Value
					iRingType = CInt(oValue)
					If iFeatureIDMin = 0 Then
						iFeatureIDMin = iPgonID
						iFeatureIDMax = iPgonID
					Else
						iFeatureIDMin = Math.Min(iFeatureIDMin, iPgonID)
						iFeatureIDMax = Math.Max(iFeatureIDMax, iPgonID)
					End If

					iaPgonID(iIndex) = iPgonID
					iaRingType(iIndex) = iRingType
				Next
				System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & vbCrLf & CStr(iFeatureIDMax), "08_510")
				For iIndex As Integer = 0 To lstPolygons.Count - 1
					oPolygon = lstPolygons.Item(iIndex)
					iTestCounter += 1
					iPgonID = iaPgonID(iIndex)
					iPgonIDNew = iPgonID - iFeatureIDMin + 1
					If dicRes.TryGetValue(iPgonIDNew, tParcelData) Then
						If bFilterNotExists OrElse setGush.Contains(tParcelData.Block) Then
							If iaRingType(iIndex) = 1 Then
								colPolylines.Add(oPolygon.ObjectId)
								iaCentroidIndecis(iIndex) = iCentroidIndex
								iCentroidIndex += 1
								If Not dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
									oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()

									oMPolygon.Layer = "TplnParcelMPgon"
									DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
									dicMPgons.Add(iPgonID, oMPolygon)

								End If
								oMPolygon.AppendLoopFromBoundary(oPolygon, False, 0.001)

							Else
								iaCentroidIndecis(iIndex) = -1

							End If
						Else
							oPolygon.Erase()
						End If
					End If
					'		dicPgonsID.Add(oPolygon.ObjectId, iPgonID)


				Next
				'    System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count), "08_600")
				'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
				iTestCounter = 0
				Dim sLayerName As String = "1603"
				Dim sBlockName As String = "1603"
				Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
				Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
				'	Dim ia1603AttribIndices() As Integer = {0, 1, 2, 3}
				'	Dim sa1603AttribText(ia1603AttribIndices.GetUpperBound(0)) As String
				'		System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count) & vbCrLf & CStr(dicMPgons.Count), "01_690")
				'	System.Windows.Forms.MessageBox.Show("SetDataInsertBlock" & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & vbCrLf & sBlockName, "!02_331parcel")
				Try
					oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_847u")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - tsiBuildTopo_Click" & ": ")
				End Try
				DMAcadExt.AcadTransaction.CreateLayer("1602")
				Dim oXDataParcel As DMAcadExt.TplnXDataParcel
				moMPgonsByGush = New MPgonsByGush()
				For iIndex As Integer = 0 To lstPolygons.Count - 1
					iTestCounter += 1
					tPgonAcObjID = colPolylines.Item(iIndex)
					If iaCentroidIndecis(iIndex) = -1 Then
						iRingType = -1
					Else
						iRingType = 1
						tCentroidAcObjID = colCentroids.Item(iaCentroidIndecis(iIndex))
					End If

					oPolygon = lstPolygons.Item(iIndex)
					'oResBuffer = oPolygon.XData
					If Not oPolygon.IsErased Then
						oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
						oXDataParcel = New DMAcadExt.TplnXDataParcel()
						'	System.Windows.Forms.MessageBox.Show(CStr(oResBuffer Is Nothing), "05_100 B")
						If oResBuffer IsNot Nothing Then
							iPgonID = iaPgonID(iIndex)
							iPgonIDNew = iPgonID - iFeatureIDMin + 1

							If dicRes.TryGetValue(iPgonIDNew, tParcelData) Then
								If iIndex = 0 Then
									'System.Windows.Forms.MessageBox.Show(CStr(tParcelData.Block) & vbCrLf & CStr(tParcelData.BlockAdd), "01_144")
									'	tShapeFOData.Block = tParcelData.Block
									'	tShapeFOData.BlockAdd = tParcelData.BlockAdd
									tShapeFOData.Gush = New GushData(0, tParcelData.Block, tParcelData.BlockAdd, tParcelData.Status, 0, tParcelData.LegalArea)
								End If
								miTopoIDCounter += 1
								oXDataParcel.ID = miTopoIDCounter
								oXDataParcel.DataID = iPgonIDNew

								oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels
								oXDataParcel.Block = tParcelData.Block
								oXDataParcel.BlockAdd = tParcelData.BlockAdd
								oXDataParcel.Name = tParcelData.ParcelName
								oXDataParcel.LegalArea = tParcelData.LegalArea
								oXDataParcel.Status = tParcelData.Status

								Try
									oPolygon.Layer = "1602"
								Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
									System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "SetDataInsertBlock_Parcel")
								End Try

								If iRingType = 1 Then
									DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, ParcelData.mia1603AttribIndices, tParcelData.GetAttribText())
									oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
									If dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
										oXDataParcel.AcadArea = Math.Abs(oMPolygon.Area)
										dSumMPgonArea += Math.Abs(oMPolygon.Area)
										oXDataParcel.Perimeter = oMPolygon.Perimeter
										oMPolygon.XData = oXDataParcel.GetResBuffer()
										oCentroidDBObject.XData = oMPolygon.XData
										moMPgonsByGush.Add(tParcelData.Block, oMPolygon.ObjectId)
									End If
								Else
									'System.Windows.Forms.MessageBox.Show("#" & CStr(iPgonIDNew) & vbCrLf & iRingType.ToString(), "01_892y")
									oPolygon.Erase()
								End If

							Else
								If iTestCounter < 3 Then
									System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iPgonID), "04_271e")
								End If
							End If
						Else
							System.Windows.Forms.MessageBox.Show("XData was not found" & vbCrLf & tPgonAcObjID.ToString() & vbCrLf & oPolygon.GetRXClass().Name, "04_156")
						End If
					End If
				Next
				tShapeFOData.SumMPgonArea = dSumMPgonArea
				'	System.Windows.Forms.MessageBox.Show(CStr(moMPgonsByGush.Count) & vbCrLf & CStr(999), "01_873q")
				'	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
			Else
				System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & tShapeFOData.GetXDataAppName, "04_272")
			End If
		Else
			System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & tShapeFOData.ShapeFileName, "04_273Pr")
			oEditor.WriteMessage("76-- Not exists" & ":" & CStr(sClassName) & vbCrLf)
		End If
	End Sub
	Public ReadOnly Property GushFromParcelData As Dictionary(Of Integer, GushData)
      Get
         Return mdicGushFromParcelData
      End Get
   End Property
   Public Sub SetDataInsertBlock_Lot(ByRef tShapeFOData As FDO_Manager.ShapeFOData)
      System.Windows.Forms.MessageBox.Show(CStr("setGush Is Nothing"), "08_400")
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iTestCounter As Integer
      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim sClassName As String = tShapeFOData.FeatureClass
      Dim dicRes As Dictionary(Of Integer, LotData_Complot)
      Dim tLotData As LotData_Complot = Nothing
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp("CPTopo_Lots")

      '?????	oXDataParcel = New DMAcadExt.TplnXDataParcel()
      Dim oValue As System.Object
      Dim iPgonID, iPgonIDNew As Integer
      Dim iRingType As Integer
      Dim sPlinesLayer As String
      Dim iFeatureIDMin, iFeatureIDMax As Integer
      Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()
      Dim bFilterNotExists As Boolean = True
      '	Dim dicPgonsID As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer) = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer)()
      Dim dicMPgons As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon)()
      Try
         dicRes = GetLotData_Complot(tShapeFOData.ShapeFileName, sClassName)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tShapeFOData.ShapeFileName & vbCrLf & sClassName, "04_998L")
         dicRes = Nothing
      End Try
      '	Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
      Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = Nothing
      Dim dSumMPgonArea As Double = 0.0
      DMAcadExt.AcadTransaction.CreateLayer("TplnParcelMPgon")
      If dicRes IsNot Nothing Then
         If bRes Then
            iTestCounter = 0
            lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sClassName, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)  'XXXXXXXXXXXXXXXXXXXX
            ' OUT Dim dicPolygons As System.Collections.Generic.IDictionary(Of ObjectId, Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylinesDic("1602", OpenMode.ForWrite)	'"pclp004"

            '     System.Windows.Forms.MessageBox.Show(CStr(lstPolygons.Count - 1) & vbCrLf & CStr(dicRes.Count), "04_247y")
            Dim iaPgonID(lstPolygons.Count - 1) As Integer
            Dim iaRingType(lstPolygons.Count - 1) As Integer
            Dim iaCentroidIndecis(lstPolygons.Count - 1) As Integer
            Dim iCentroidIndex As Integer = 0
            Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
            colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

            For iIndex As Integer = 0 To lstPolygons.Count - 1
               oPolygon = lstPolygons.Item(iIndex)
               oResBuffer = oPolygon.XData
               taTypedValues = oResBuffer.AsArray()
               oValue = taTypedValues(2).Value
               iPgonID = CInt(oValue)
               oValue = taTypedValues(3).Value
               iRingType = CInt(oValue)
               If iFeatureIDMin = 0 Then
                  iFeatureIDMin = iPgonID
                  iFeatureIDMax = iPgonID
               Else
                  iFeatureIDMin = Math.Min(iFeatureIDMin, iPgonID)
                  iFeatureIDMax = Math.Max(iFeatureIDMax, iPgonID)
               End If

               iaPgonID(iIndex) = iPgonID
               iaRingType(iIndex) = iRingType
            Next
            '     System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & vbCrLf & CStr(iFeatureIDMax), "08_510")
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               oPolygon = lstPolygons.Item(iIndex)
               iTestCounter += 1
               iPgonID = iaPgonID(iIndex)
               iPgonIDNew = iPgonID - iFeatureIDMin + 1
               If dicRes.TryGetValue(iPgonIDNew, tLotData) Then
                  If bFilterNotExists Then
                     If iaRingType(iIndex) = 1 Then
                        colPolylines.Add(oPolygon.ObjectId)
                        iaCentroidIndecis(iIndex) = iCentroidIndex
                        iCentroidIndex += 1
                        If Not dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                           oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()

                           oMPolygon.Layer = "TplnParcelMPgon"
                           DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
                           dicMPgons.Add(iPgonID, oMPolygon)

                        End If
                        oMPolygon.AppendLoopFromBoundary(oPolygon, False, 0.001)

                     Else
                        iaCentroidIndecis(iIndex) = -1

                     End If
                  Else
                     oPolygon.Erase()
                  End If
               End If
               '		dicPgonsID.Add(oPolygon.ObjectId, iPgonID)


            Next
            '   System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count), "08_600")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            iTestCounter = 0
            Dim sLayerName As String = "SCellNoK"
            Dim sBlockName As String = "Cellno"

            Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
            Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
            '	Dim ia1603AttribIndices() As Integer = {0, 1, 2, 3}
            '	Dim sa1603AttribText(ia1603AttribIndices.GetUpperBound(0)) As String
            '		System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count) & vbCrLf & CStr(dicMPgons.Count), "01_690")
            '	System.Windows.Forms.MessageBox.Show("SetDataInsertBlock" & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & vbCrLf & sBlockName, "!02_331parcel")
            Try
               oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_841u")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - tsiBuildTopo_Click" & ": ")
            End Try
            DMAcadExt.AcadTransaction.CreateLayer("1602")
            Dim oXDataLot As DMAcadExt.TplnXDataLot
            moMPgonsByGush = New MPgonsByGush()
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               iTestCounter += 1
               tPgonAcObjID = colPolylines.Item(iIndex)
               If iaCentroidIndecis(iIndex) = -1 Then
                  iRingType = -1
               Else
                  iRingType = 1
                  tCentroidAcObjID = colCentroids.Item(iaCentroidIndecis(iIndex))
               End If

               oPolygon = lstPolygons.Item(iIndex)
               'oResBuffer = oPolygon.XData
               If Not oPolygon.IsErased Then
                  oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
                  oXDataLot = New DMAcadExt.TplnXDataLot("CPTopo_Lots")
                  '	System.Windows.Forms.MessageBox.Show(CStr(oResBuffer Is Nothing), "05_100 B")
                  If oResBuffer IsNot Nothing Then
                     iPgonID = iaPgonID(iIndex)
                     iPgonIDNew = iPgonID - iFeatureIDMin + 1

                     If dicRes.TryGetValue(iPgonIDNew, tLotData) Then
                        If iIndex = 0 Then
                           'System.Windows.Forms.MessageBox.Show(CStr(tParcelData.Block) & vbCrLf & CStr(tParcelData.BlockAdd), "01_144")
                           '	tShapeFOData.Block = tParcelData.Block
                           '	tShapeFOData.BlockAdd = tParcelData.BlockAdd

                        End If
                        miTopoIDCounter += 1
                        oXDataLot.ID = miTopoIDCounter
                        oXDataLot.DataID = iPgonIDNew

                        oXDataLot.MapTheme = DMAcadExt.enMapTheme.LotApproved
                        oXDataLot.Landuse = tLotData.Ykd2


                        Try
                           oPolygon.Layer = "pcell"
                        Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                           System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "pcell", "SetDataInsertBlock_Lot")

                        End Try

                        If iRingType = 1 Then
                           DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, LotData_Complot.miaCellnoAttribIndices, tLotData.GetAttribText())
                           oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           If dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                              oXDataLot.AcadArea = Math.Abs(oMPolygon.Area)
                              dSumMPgonArea += Math.Abs(oMPolygon.Area)
                              oXDataLot.Perimeter = oMPolygon.Perimeter
                              oMPolygon.XData = oXDataLot.GetResBuffer()
                              oCentroidDBObject.XData = oMPolygon.XData

                           End If
                        Else
                           'System.Windows.Forms.MessageBox.Show("#" & CStr(iPgonIDNew) & vbCrLf & iRingType.ToString(), "01_892y")
                           oPolygon.Erase()
                        End If

                     Else
                        If iTestCounter < 3 Then
                           System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iPgonID), "04_271e")
                        End If
                     End If
                  Else
                     System.Windows.Forms.MessageBox.Show("XData was not found" & vbCrLf & tPgonAcObjID.ToString() & vbCrLf & oPolygon.GetRXClass().Name, "04_156")
                  End If
               End If
            Next
            tShapeFOData.SumMPgonArea = dSumMPgonArea
            '	System.Windows.Forms.MessageBox.Show(CStr(moMPgonsByGush.Count) & vbCrLf & CStr(999), "01_873q")
            '	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
         Else
            System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & tShapeFOData.GetXDataAppName, "04_272")
         End If
      Else
         System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & tShapeFOData.ShapeFileName, "04_273L")
         oEditor.WriteMessage("76-- Not exists" & ":" & CStr(sClassName) & vbCrLf)
      End If
   End Sub

   Public Sub SetDataInsertBlock_Plan(ByRef tShapeFOData As FDO_Manager.ShapeFOData)
      Const sMPgonLayer As String = "TplnPlanMPgon"
      System.Windows.Forms.MessageBox.Show(CStr("SetDataInsertBlock_Plan"), "08_410")
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iTestCounter As Integer
      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim lstPolygons As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      Dim sClassName As String = tShapeFOData.FeatureClass
      Dim dicRes As Dictionary(Of Integer, LotData_Complot)
      Dim tLotData As LotData_Complot = Nothing
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer '= New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp("CPTopo_Lots")

      '?????	oXDataParcel = New DMAcadExt.TplnXDataParcel()
      Dim oValue As System.Object
      Dim iPgonID, iPgonIDNew As Integer
      Dim iRingType As Integer
      Dim iFeatureIDMin, iFeatureIDMax As Integer
      Dim dicTest As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectId)()
      Dim bFilterNotExists As Boolean = True
      '	Dim dicPgonsID As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer) = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Integer)()
      Dim dicMPgons As Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon) = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.MPolygon)()
      Try
         dicRes = GetLotData_Complot(tShapeFOData.ShapeFileName, sClassName)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tShapeFOData.ShapeFileName & vbCrLf & sClassName, "04_999P")
         dicRes = Nothing
      End Try
      '	Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
      Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim oMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon = Nothing
      Dim dSumMPgonArea As Double = 0.0
      DMAcadExt.AcadTransaction.CreateLayer(sMPgonLayer)
      If dicRes IsNot Nothing Then
         If bRes Then
            iTestCounter = 0
            lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines(sClassName, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)  'XXXXXXXXXXXXXXXXXXXX
            ' OUT Dim dicPolygons As System.Collections.Generic.IDictionary(Of ObjectId, Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylinesDic("1602", OpenMode.ForWrite)	'"pclp004"

            '     System.Windows.Forms.MessageBox.Show(CStr(lstPolygons.Count - 1) & vbCrLf & CStr(dicRes.Count), "04_247y")
            Dim iaPgonID(lstPolygons.Count - 1) As Integer
            Dim iaRingType(lstPolygons.Count - 1) As Integer
            Dim iaCentroidIndecis(lstPolygons.Count - 1) As Integer
            Dim iCentroidIndex As Integer = 0
            Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
            colPolylines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

            For iIndex As Integer = 0 To lstPolygons.Count - 1
               oPolygon = lstPolygons.Item(iIndex)
               oResBuffer = oPolygon.XData
               taTypedValues = oResBuffer.AsArray()
               oValue = taTypedValues(2).Value
               iPgonID = CInt(oValue)
               oValue = taTypedValues(3).Value
               iRingType = CInt(oValue)
               If iFeatureIDMin = 0 Then
                  iFeatureIDMin = iPgonID
                  iFeatureIDMax = iPgonID
               Else
                  iFeatureIDMin = Math.Min(iFeatureIDMin, iPgonID)
                  iFeatureIDMax = Math.Max(iFeatureIDMax, iPgonID)
               End If

               iaPgonID(iIndex) = iPgonID
               iaRingType(iIndex) = iRingType
            Next
            '     System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & vbCrLf & CStr(iFeatureIDMax), "08_510")
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               oPolygon = lstPolygons.Item(iIndex)
               iTestCounter += 1
               iPgonID = iaPgonID(iIndex)
               iPgonIDNew = iPgonID - iFeatureIDMin + 1
               If dicRes.TryGetValue(iPgonIDNew, tLotData) Then
                  If bFilterNotExists Then
                     If iaRingType(iIndex) = 1 Then
                        colPolylines.Add(oPolygon.ObjectId)
                        iaCentroidIndecis(iIndex) = iCentroidIndex
                        iCentroidIndex += 1
                        If Not dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                           oMPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()

                           oMPolygon.Layer = sMPgonLayer
                           DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
                           dicMPgons.Add(iPgonID, oMPolygon)

                        End If
                        oMPolygon.AppendLoopFromBoundary(oPolygon, False, 0.001)

                     Else
                        iaCentroidIndecis(iIndex) = -1

                     End If
                  Else
                     oPolygon.Erase()
                  End If
               End If
               '		dicPgonsID.Add(oPolygon.ObjectId, iPgonID)


            Next
            '   System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count), "08_600")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            iTestCounter = 0
            ' Dim sLayerName As String = "SCellNoK"
            '  Dim sBlockName As String = "Cellno"
            Dim sLayerName As String = "skayno"
            Dim sBlockName As String = "SKANYO-RG1"

            Dim tPgonAcObjID, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
            Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
            '	Dim ia1603AttribIndices() As Integer = {0, 1, 2, 3}
            '	Dim sa1603AttribText(ia1603AttribIndices.GetUpperBound(0)) As String
            '		System.Windows.Forms.MessageBox.Show(CStr(colPolylines.Count) & vbCrLf & CStr(dicMPgons.Count), "01_690")
            '	System.Windows.Forms.MessageBox.Show("SetDataInsertBlock" & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & vbCrLf & sBlockName, "!02_331parcel")
            Try
               oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_841u")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - tsiBuildTopo_Click" & ": ")
            End Try
            DMAcadExt.AcadTransaction.CreateLayer("skayno")
            Dim oXDataLot As DMAcadExt.TplnXDataLot
            moMPgonsByGush = New MPgonsByGush()
            For iIndex As Integer = 0 To lstPolygons.Count - 1
               iTestCounter += 1
               tPgonAcObjID = colPolylines.Item(iIndex)
               If iaCentroidIndecis(iIndex) = -1 Then
                  iRingType = -1
               Else
                  iRingType = 1
                  tCentroidAcObjID = colCentroids.Item(iaCentroidIndecis(iIndex))
               End If

               oPolygon = lstPolygons.Item(iIndex)
               'oResBuffer = oPolygon.XData
               If Not oPolygon.IsErased Then
                  oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
                  oXDataLot = New DMAcadExt.TplnXDataLot("CPTopo_Lots")
                  '	System.Windows.Forms.MessageBox.Show(CStr(oResBuffer Is Nothing), "05_100 B")
                  If oResBuffer IsNot Nothing Then
                     iPgonID = iaPgonID(iIndex)
                     iPgonIDNew = iPgonID - iFeatureIDMin + 1

                     If dicRes.TryGetValue(iPgonIDNew, tLotData) Then
                        If iIndex = 0 Then
                           'System.Windows.Forms.MessageBox.Show(CStr(tParcelData.Block) & vbCrLf & CStr(tParcelData.BlockAdd), "01_144")
                           '	tShapeFOData.Block = tParcelData.Block
                           '	tShapeFOData.BlockAdd = tParcelData.BlockAdd

                        End If
                        miTopoIDCounter += 1
                        oXDataLot.ID = miTopoIDCounter
                        oXDataLot.DataID = iPgonIDNew

                        oXDataLot.MapTheme = DMAcadExt.enMapTheme.LotApproved
                        oXDataLot.Landuse = tLotData.Ykd2


                        Try
                           oPolygon.Layer = "Pgvul1"
                        Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                           System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "Pgvul1", "SetDataI...k_Lot")

                        End Try

                        If iRingType = 1 Then
                           DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, LotData_Complot.miaCellnoAttribIndices, tLotData.GetAttribText())
                           oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           If dicMPgons.TryGetValue(iPgonID, oMPolygon) Then
                              oXDataLot.AcadArea = Math.Abs(oMPolygon.Area)
                              dSumMPgonArea += Math.Abs(oMPolygon.Area)
                              oXDataLot.Perimeter = oMPolygon.Perimeter
                              oMPolygon.XData = oXDataLot.GetResBuffer()
                              oCentroidDBObject.XData = oMPolygon.XData

                           End If
                        Else
                           'System.Windows.Forms.MessageBox.Show("#" & CStr(iPgonIDNew) & vbCrLf & iRingType.ToString(), "01_892y")
                           oPolygon.Erase()
                        End If

                     Else
                        If iTestCounter < 3 Then
                           System.Windows.Forms.MessageBox.Show("Shape FeatureId was not found" & vbCrLf & CStr(iPgonID), "04_271e")
                        End If
                     End If
                  Else
                     System.Windows.Forms.MessageBox.Show("XData was not found" & vbCrLf & tPgonAcObjID.ToString() & vbCrLf & oPolygon.GetRXClass().Name, "04_156")
                  End If
               End If
            Next
            tShapeFOData.SumMPgonArea = dSumMPgonArea
            '	System.Windows.Forms.MessageBox.Show(CStr(moMPgonsByGush.Count) & vbCrLf & CStr(999), "01_873q")
            '	System.Windows.Forms.MessageBox.Show(CStr(iFeatureIDMin) & ":" & CStr(iFeatureIDMax), "04_002 Feature Min:Max")
         Else
            System.Windows.Forms.MessageBox.Show("Registration was not found" & vbCrLf & tShapeFOData.GetXDataAppName, "04_272")
         End If
      Else
         System.Windows.Forms.MessageBox.Show("OverlayData was not found" & vbCrLf & tShapeFOData.ShapeFileName, "04_273Pl")
         oEditor.WriteMessage("76-- Not exists" & ":" & CStr(sClassName) & vbCrLf)
      End If
   End Sub
   Public Sub Load()

   End Sub
   Public Sub LoadParcelData()
      '	mdicParcelData = GetParcelData(mtShapeFOData.ShapeFileName, mtShapeFOData.FeatureClass)

      mdicParcelData = New Dictionary(Of Integer, ParcelData)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(mtShapeFOData.ShapeFileName, mtShapeFOData.FeatureClass, New DMCommon.dmList)
      If oFeatureReader IsNot Nothing Then
         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim iBlock As Integer
         Dim iBlockAdd As Integer
         Dim iParcelName As Integer
         Dim sParcelName As String
         Dim dLegalArea As Double
         Dim iStatus As Integer
         Dim sPropName As String = ""
         Dim tParcelData As ParcelData = Nothing
         Dim iFeatIDIndex As Integer
         Dim iaPropIndices() = {-1, -1, -1, -1, -1, -1}
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)
            sPropName = oPropDef.Name
            '	System.Windows.Forms.MessageBox.Show(oPropDef.Name, "01_420")
            Select Case sPropName
               Case "GUSH_NO", "GUSH_NUM"
                  iaPropIndices(enParcelFields.BlockNo) = iIndex
               Case "GUSH_SUFFI"
                  iaPropIndices(enParcelFields.BlockAdd) = iIndex
               Case "PARCEL"
                  iaPropIndices(enParcelFields.ParcelName) = iIndex
               Case "PARCEL_ID"
                  iaPropIndices(enParcelFields.ParcelID) = iIndex
               Case "LEGAL_AREA"
                  iaPropIndices(enParcelFields.LegalArea) = iIndex
               Case "STATUS_ID", "STATUS"
                  If iaPropIndices(enParcelFields.Status) = -1 Then
                     iaPropIndices(enParcelFields.Status) = iIndex
                  End If
               Case "FeatId"
                  iFeatIDIndex = iIndex
               Case Else
                  '  DMCommon.Debug.MsgBox("07_280", sPropName, iIndex)
            End Select
         Next


         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enParcelFields.BlockNo) <> -1 Then
                  iBlock = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockNo))
               Else
                  iBlock = 0
               End If
               If iaPropIndices(enParcelFields.BlockAdd) <> -1 Then
                  iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockAdd))
               Else
                  iBlockAdd = 0
               End If
               If iaPropIndices(enParcelFields.ParcelName) <> -1 Then
                  iParcelName = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelName))
               Else
                  iParcelName = 0
               End If

               If iaPropIndices(enParcelFields.LegalArea) <> -1 Then
                  dLegalArea = oFeatureReader.GetDouble(iaPropIndices(enParcelFields.LegalArea))
               Else
                  dLegalArea = 0.0
               End If
               If iaPropIndices(enParcelFields.Status) <> -1 Then
                  iStatus = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.Status))
               Else
                  iStatus = 0
               End If
               sParcelName = Convert.ToString(iParcelName)
               tParcelData = New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)
               If iPgonID = 1 Then
                  mtShapeFOData.Gush = New GushData(0, tParcelData.Block, tParcelData.BlockAdd, tParcelData.Status, 0, tParcelData.LegalArea)
               End If
               DMAcadExt.AcadDocument.WriteMessage(sText & " FeatID: " & oFeatureReader.GetInt32(iFeatIDIndex))
               mdicParcelData.Add(iPgonID, tParcelData)
            Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("142err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf & oEx.StackTrace)
				End Try
         End While
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & mtShapeFOData.FeatureClass, "04_409")

      End If

   End Sub
   Public ReadOnly Property TopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)
      Get
         Return mdicTopoPolygons
      End Get
   End Property

   Public Sub Paint()
      Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(200001, 1.0)

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)




      For Each oPgon As TopoManager.TPlanGraph.TplnTopoPgon In mdicTopoPolygons.Values
         oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
      Next

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Sub PaintA()
      Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(200001, 1.0)

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)




      For Each oPgon As TopoManager.tmPolygon In mdicTmPolygons.Values
         oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
      Next

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Sub ImportParcels(colPolylineIDs As ObjectIdCollection)
      zzLoadImport(colPolylineIDs)
      CreateCentroids()
      zzSetData()
   End Sub
   Private Sub zzLoadImport(colPolylineIDs As ObjectIdCollection)
      Const dToler As Double = 0.001
      Dim bExcludeCrossing As Boolean = False
      Dim oResBuffer As ResultBuffer
      Dim oValue As System.Object
      Dim iPgonImpID As Integer
      Dim iIndex As Integer = 0
      Dim iCentroidIndex As Integer = 0

      Dim iRingType As Integer
      Dim oPolylineDBObj As DBObject
      Dim oPolyline As Polyline
      Dim iNLoopTest As Integer
      Dim oMPolygon As MPolygon = Nothing

      mcolOuterPolylineIDs = New ObjectIdCollection()

      '   System.Windows.Forms.MessageBox.Show(CStr(colPolylineIDs.Count) & ":" & CStr(mdicParcelData.Count), "04_194")

      mdicImpMPgons = New Dictionary(Of Integer, MPolygon)

      ReDim miaFeaturesID(mdicParcelData.Count - 1)
      ReDim miaCentroidIndecis(mdicParcelData.Count - 1)
      '   System.Windows.Forms.MessageBox.Show(colPolylineIDs.Count.ToString() & ":" & mdicParcelData.Count.ToString(), "04_807")

      If colPolylineIDs Is Nothing Then
         System.Windows.Forms.MessageBox.Show("colPolylineIDs Is Nothing", "04_190")
      Else

         For Each tPolylineObjID As ObjectId In colPolylineIDs
            iNLoopTest = 0
            oPolylineDBObj = DMAcadExt.AcadTransaction.GetDBObject(tPolylineObjID, OpenMode.ForWrite)
            oPolyline = DirectCast(oPolylineDBObj, Polyline)
            oResBuffer = oPolylineDBObj.XData
            If oResBuffer IsNot Nothing Then
               Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue = oResBuffer.AsArray()
               If taTypedValues.GetUpperBound(0) >= 3 Then


                  oValue = taTypedValues(2).Value
                  iPgonImpID = CInt(oValue)
                  oValue = taTypedValues(3).Value
                  iRingType = CInt(oValue)

                  If miFeatureIDMin = 0 Then
                     miFeatureIDMin = iPgonImpID
                     miFeatureIDMax = iPgonImpID
                  Else
                     miFeatureIDMin = Math.Min(miFeatureIDMin, iPgonImpID)
                     miFeatureIDMax = Math.Max(miFeatureIDMax, iPgonImpID)
                  End If
                  'End If

                  '	dicPgonsID.Add(oPolygon.ObjectId, iPgonID)
                  If Not mdicImpMPgons.TryGetValue(iPgonImpID, oMPolygon) Then
                     oMPolygon = New MPolygon()
                     mdicImpMPgons.Add(iPgonImpID, oMPolygon)
                  End If
                  Try
                     iNLoopTest = oMPolygon.NumMPolygonLoops
                     oMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
                  Catch oEx As Exception
                     System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & iNLoopTest & vbCrLf & CStr(iPgonImpID), "04_178")
                     oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.SkyBlue)
                  End Try
                  'System.Windows.Forms.MessageBox.Show(CStr(iPgonImpID) & ":" & CStr(mdicImpMPgons.Count), "04_166")
                  If iRingType = 1 Then
                     mcolOuterPolylineIDs.Add(oPolylineDBObj.ObjectId)
                     miaFeaturesID(iIndex) = iPgonImpID
                     iIndex += 1
                  Else
                     oPolylineDBObj.Erase()
                  End If


               End If
            End If

         Next
         '
         '    System.Windows.Forms.MessageBox.Show(CStr(miaFeaturesID.Count) & ":" & CStr(mcolOuterPolylineIDs.Count) & ":" & CStr(mdicImpMPgons.Count), "04_199")
      End If
   End Sub
   Private Sub zzSetData()
      Dim oPolylineDBObj As DBObject
      Dim oBlockRefDBObj As DBObject = Nothing

      Dim oPolyline As Polyline
      Dim tBlockRefObjID As ObjectId
      Dim oBlockRef As BlockReference = Nothing
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
      Dim tParcelData As ParcelData = Nothing
      Dim oMPolygon As MPolygon = Nothing
      '	Dim iPgonIndex As Integer = 0
      Dim tPolylineObjID As ObjectId
      Dim iID As Integer
      Dim oFindCenter As FindCenter
      ' System.Windows.Forms.MessageBox.Show(CStr(mcolOuterPolylineIDs.Count) & vbCrLf & mcolCentroids.Count.ToString() & vbCrLf & mdicImpMPgons.Count.ToString(), "04_539q")
      For iPgonIndex As Integer = 0 To mcolOuterPolylineIDs.Count - 1
         tPolylineObjID = mcolOuterPolylineIDs.Item(iPgonIndex)
         oPolyline = Nothing
         oBlockRef = Nothing
         'mcolCentroids, mcolOuterPolylineIDs
         If mcolCentroids IsNot Nothing AndAlso mcolCentroids.Count > iPgonIndex Then
            tBlockRefObjID = mcolCentroids.Item(iPgonIndex)
            oBlockRefDBObj = DMAcadExt.AcadTransaction.GetDBObject(tBlockRefObjID, OpenMode.ForWrite)
            oBlockRef = DirectCast(oBlockRefDBObj, BlockReference)
         End If

         oPolylineDBObj = DMAcadExt.AcadTransaction.GetDBObject(tPolylineObjID, OpenMode.ForWrite)

         If oPolylineDBObj IsNot Nothing Then


            oPolyline = DirectCast(oPolylineDBObj, Polyline)


            oResBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
            oXDataParcel = New DMAcadExt.TplnXDataParcel()
            iID = miaFeaturesID(iPgonIndex) - miFeatureIDMin + 1
            If mdicParcelData.TryGetValue(iID, tParcelData) AndAlso mdicImpMPgons.TryGetValue(miaFeaturesID(iPgonIndex), oMPolygon) Then
               If iPgonIndex = 0 Then
                  'System.Windows.Forms.MessageBox.Show(CStr(tParcelData.Block) & vbCrLf & CStr(tParcelData.BlockAdd), "01_144")
                  '	tShapeFOData.Block = tParcelData.Block
                  '	tShapeFOData.BlockAdd = tParcelData.BlockAdd
                  mtShapeFOData.Gush = New GushData(0, tParcelData.Block, tParcelData.BlockAdd, tParcelData.Status, 0, tParcelData.LegalArea)
               End If
               miTopoIDCounter += 1
               oXDataParcel.ID = miaFeaturesID(iPgonIndex)
               oXDataParcel.DataID = iID
               '	DMAcadExt.AcadDocument.WriteMessage("-H- " & CStr(iPgonIndex) & " " & oPolylineDBObj.Handle.ToString() & ";" & oBlockRefDBObj.Handle.ToString() & "__" & CStr(oXDataParcel.ImpID) & ":" & CStr(oXDataParcel.ID))
               oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels
               oXDataParcel.Block = tParcelData.Block
               oXDataParcel.BlockAdd = tParcelData.BlockAdd
               oXDataParcel.Name = tParcelData.ParcelName
               oXDataParcel.LegalArea = tParcelData.LegalArea
               oMPolygon.BalanceTree()
               oXDataParcel.AcadArea = oMPolygon.Area
               oXDataParcel.Status = tParcelData.Status
               oXDataParcel.NeigborList = String.Empty
               oXDataParcel.Exterior = 0
               oXDataParcel.InteriorList = String.Empty
               oXDataParcel.InteriorCount = oMPolygon.NumMPolygonLoops - 1
               mdSumMPgonArea += oMPolygon.Area
               'miaFeaturesID(iPgonIndex)
               'System.Windows.Forms.MessageBox.Show(CStr(iIndex), "04_707A")
               Try
                  ''''''''''''''''''''''''''   oPolyline.XData = oXDataParcel.GetResBuffer()
                  oPolyline.Layer = "1602"
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "FDO_Manager - zzSetDataInsertBlock")

               End Try
               If oBlockRefDBObj IsNot Nothing Then
                  '''''''''''''''   oBlockRef.XData = oXDataParcel.GetResBuffer()
                  DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, ParcelData.mia1603AttribIndices, tParcelData.GetAttribText())
                  oFindCenter = New FindCenter(oMPolygon)
                  oFindCenter.MoveBlockRef(oBlockRef, oPolyline.Handle.ToString() & ":" & tParcelData.ParcelName)
               End If
            Else
               System.Windows.Forms.MessageBox.Show("Data Was not found" & vbCrLf & CStr(iPgonIndex) & ":" & iID & vbCrLf & CStr(mdicParcelData.ContainsKey(iID)) & ":" & CStr(mdicImpMPgons.ContainsKey(miaFeaturesID(iPgonIndex))), "02_118")
            End If

         End If
         'iPgonIndex += 1
      Next

   End Sub

   Private Sub zzCreateLinkTable()
      moLinkTable = New System.Data.DataTable("Links")
      With moLinkTable.Columns
         .Add(AcObjIDFieldName, GetType(ObjectId))
         '		.Add(msAcObjIDFieldName, GetType(System.Int64))
         .Add(EntityNoFieldName, GetType(System.Int32))
         .Add(msIDFieldName, GetType(System.Int32))
         .Add(msStatusFieldName, GetType(System.Boolean))
         .Add(IsProperFieldName, GetType(System.Boolean))

         .Add(XminFieldName, GetType(System.Double))
         .Add(XmaxFieldName, GetType(System.Double))
         .Add(YminFieldName, GetType(System.Double))
         .Add(YmaxFieldName, GetType(System.Double))
         .Add(msNeigborListFieldName, GetType(System.String))
         .Add(msExteriorFieldName, GetType(System.Int32))
         .Add(msInteriorListFieldName, GetType(System.String))
         .Add(msInteriorLinesObjIDFieldName, GetType(System.String))

         .Add(msGroupIDFieldName, GetType(System.Int32))
         .Add(msGroupAddIDFieldName, GetType(System.Int32))
         .Add(msNameFieldName, GetType(System.String))

         .Add(msCentroidCountFieldName, GetType(System.Int32))
         .Add(msCentroidAcObjIDFieldName, GetType(ObjectId))
         .Add(AreaNameFieldName, GetType(System.Double))
         .Add(LengthNameFieldName, GetType(System.Double))


         .Add(msOrderFieldName, GetType(System.Int64))
         ' .Add(msParentNoFieldName, GetType(System.Int32))
         ' .Add(msParentNoAddFieldName, GetType(System.Int32))
         .Add(msLegalAreaNameFieldName, GetType(System.Double))

         .Add(msAttribInt1FieldName, GetType(System.Int32))




      End With
      moLinkTable.PrimaryKey = New System.Data.DataColumn() {moLinkTable.Columns.Item(0)}

   End Sub
   Private Sub zzFillLinkTable()
      Dim oNewRow As DataRow
      Dim oExtents3d As Extents3d
      Dim iNo As Integer = 0
      Dim iID As Integer = 0
      Dim GroupID As Integer = 0
      Dim GroupIDAdd As Integer = 0

      Dim oPolyline As Polyline
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      DMAcadExt.AcadDocument.WriteMessage("-- " & "Polylines: " & CStr(mcolPolylineIDs.Count))
      miSource = enPolygonSetSource.Polylines
      For Each tAcObjID As ObjectId In mcolPolylineIDs
         iNo += 1
         oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForRead)
         oExtents3d = oPolyline.GeometricExtents
         'DMAcadExt.AcadDocument.WriteMessage("&^& " & miSource.ToString())
         If miSource = enPolygonSetSource.Shapes OrElse miSource = enPolygonSetSource.NotDefined Then
            oXDataParcel = New DMAcadExt.TplnXDataParcel(oPolyline.GetXDataForApplication(DMAcadExt.TplnXDataParcel.XDataAppName), False)
            'DMAcadExt.AcadDocument.WriteMessage("&--& " & miSource.ToString() & ":" & CStr(oXDataParcel.HasData))
            If oXDataParcel IsNot Nothing AndAlso oXDataParcel.HasData Then
               iID = oXDataParcel.DataID
               GroupID = oXDataParcel.Block
               GroupIDAdd = oXDataParcel.BlockAdd
               miSource = enPolygonSetSource.Shapes
            Else
               miSource = enPolygonSetSource.Polylines
            End If

         End If

         oNewRow = moLinkTable.NewRow()
         With oNewRow
            .Item(AcObjIDFieldName) = oPolyline.ObjectId
            '	.Item(msAcObjIDFieldName) = oPolyline.ObjectId.OldIdPtr.ToInt64()

            .Item(EntityNoFieldName) = iNo
            If miSource = enPolygonSetSource.Shapes Then
               .Item(msIDFieldName) = iID
               .Item(msGroupIDFieldName) = GroupID
               .Item(msGroupAddIDFieldName) = GroupIDAdd

            End If



            .Item(msStatusFieldName) = False
            .Item(IsProperFieldName) = True


            .Item(XminFieldName) = oExtents3d.MinPoint.X
            .Item(XmaxFieldName) = oExtents3d.MaxPoint.X
            .Item(YminFieldName) = oExtents3d.MinPoint.Y
            .Item(YmaxFieldName) = oExtents3d.MaxPoint.Y
            .Item(AreaNameFieldName) = Math.Round(oPolyline.Area, 2)
            .Item(LengthNameFieldName) = Math.Round(oPolyline.Length, 2)

         End With
         moLinkTable.Rows.Add(oNewRow)

      Next
      DMAcadExt.AcadDocument.WriteMessage("-- Set type: " & miSource.ToString())

   End Sub
   Private Sub zzFillMPgonTable()
      Dim oNewRow As DataRow
      Dim oExtents3d As Extents3d
      Dim iRowNo As Integer = 0

      For Each oMPgon As MPolygon In mdicMPolygons.Values
         iRowNo += 1
         oExtents3d = oMPgon.GeometricExtents
         oNewRow = moLinkTable.NewRow()
         With oNewRow
            .Item(AcObjIDFieldName) = oMPgon.ObjectId
            '	.Item(msAcObjIDFieldName) = oMPgon.ObjectId.OldIdPtr.ToInt64()

            .Item(EntityNoFieldName) = iRowNo

            .Item(msStatusFieldName) = False
            .Item(XminFieldName) = oExtents3d.MinPoint.X
            .Item(XmaxFieldName) = oExtents3d.MaxPoint.X
            .Item(YminFieldName) = oExtents3d.MinPoint.Y
            .Item(YmaxFieldName) = oExtents3d.MaxPoint.Y

            .Item(AreaNameFieldName) = oMPgon.Area
            .Item(LengthNameFieldName) = oMPgon.Perimeter
         End With
         moLinkTable.Rows.Add(oNewRow)
      Next
   End Sub
   Private Sub zzCreateCentroidTable()
      moCentroidTable = New System.Data.DataTable("Centroids")
      '	System.Windows.Forms.MessageBox.Show(CStr(moCentroids Is Nothing), "03_400X")
      With moCentroidTable.Columns
         '  .Add(msAcObjIDFieldName, GetType(System.Int64))
         .Add(AcObjIDFieldName, GetType(ObjectId))

         .Add(msStatusFieldName, GetType(System.Boolean))
         .Add(XFieldName, GetType(System.Double))
         .Add(YFieldName, GetType(System.Double))
         .Add(msHandleFieldName, GetType(System.String))
         .Add(msGroupIDFieldName, GetType(System.Int32))
         .Add(msGroupAddIDFieldName, GetType(System.Int32))
         .Add(msNameFieldName, GetType(System.String))
         .Add(msOrderFieldName, GetType(System.Int64))
         .Add(msLegalAreaNameFieldName, GetType(System.Double))
      End With
   End Sub
   Private Sub zzCreateIntersectTable()
      moIntersectionsTable = New System.Data.DataTable("Intersections")
      '	System.Windows.Forms.MessageBox.Show(CStr(moCentroids Is Nothing), "03_400X")
      With moIntersectionsTable.Columns
         '  .Add(msAcObjIDFieldName, GetType(System.Int64))
         .Add(AcObjIDFieldName, GetType(ObjectId))
         .Add(EntityNoFieldName, GetType(System.Int32))

         .Add(msGroupIDFieldName, GetType(System.Int32))
         .Add(msGroupAddIDFieldName, GetType(System.Int32))
         .Add(msNameFieldName, GetType(System.String))
         .Add(msOrderFieldName, GetType(System.Int64))
         .Add(CheckAcObjIDFieldName, GetType(ObjectId))
         .Add(msCheckEntityNoFieldName, GetType(System.Int32))

         .Add(msCheckGroupIDFieldName, GetType(System.Int32))
         .Add(msCheckGroupAddIDFieldName, GetType(System.Int32))
         .Add(msCheckNameFieldName, GetType(System.String))


         .Add(msIntersectCountFieldName, GetType(System.Int32))


         .Add(XminFieldName, GetType(System.Double))
         .Add(XmaxFieldName, GetType(System.Double))
         .Add(YminFieldName, GetType(System.Double))
         .Add(YmaxFieldName, GetType(System.Double))




         '.Add(msStatusFieldName, GetType(System.Boolean))
         '.Add(XFieldName, GetType(System.Double))
         '.Add(YFieldName, GetType(System.Double))
         '.Add(msHandleFieldName, GetType(System.String))

      End With
   End Sub

   Private Sub zzFillCentroidDict()

      Dim oBlockRef As BlockReference
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim tGroupKey As GroupKey
      Dim iTest As Integer
      mdicCentroids = New Dictionary(Of GroupKey, ObjectId)
      For Each tAcObjID As ObjectId In mcolCentroids
         iTest += 1
         oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead)

         If oBlockRef IsNot Nothing Then
            oXDataParcel = New DMAcadExt.TplnXDataParcel(oBlockRef.XData)
            If oXDataParcel.ID = 0 Then
               miSource = enPolygonSetSource.Polylines
               Return
            End If
            tGroupKey = New GroupKey(oXDataParcel.Block, oXDataParcel.BlockAdd, oXDataParcel.DataID)
            Try
               mdicCentroids.Add(tGroupKey, oBlockRef.ObjectId)

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(oXDataParcel.DataID), "03_273a")
            End Try

         End If
      Next
   End Sub
   Private Sub zzFillCentroidTable()
      Dim oNewRow As DataRow
      Dim oBlockRef As BlockReference
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d

      '	System.Windows.Forms.MessageBox.Show(CStr(mdicBlocks.Count), "03_405")
      For Each tAcObjID As ObjectId In mcolCentroids
         oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead)

         If oBlockRef IsNot Nothing Then
            Try
               tPoint = oBlockRef.Position
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf, "04_122")
            End Try
            oNewRow = moCentroidTable.NewRow()



            With oNewRow
               .Item(AcObjIDFieldName) = oBlockRef.ObjectId
               '	.Item(msAcObjIDFieldName) = tAcObjID.OldIdPtr.ToInt64()
               .Item(msStatusFieldName) = False
               .Item(XFieldName) = tPoint.X
               .Item(YFieldName) = tPoint.Y

            End With
            moCentroidTable.Rows.Add(oNewRow)
         End If
      Next






      'System.Windows.Forms.MessageBox.Show("Centroids: " & vbCrLf & CStr(mcolCentroids.Count) & ":" & CStr(moCentroidTable.Rows.Count), "03_
   End Sub
   Public ReadOnly Property LinkDataTable As System.Data.DataTable

      Get
         Return moLinkTable
      End Get
   End Property
   Public ReadOnly Property IntersectionsTable As System.Data.DataTable

      Get
         Return moIntersectionsTable
      End Get
   End Property
   Public Sub FillIntersectionsTable()

      Dim iEntityNo As Integer
      Dim oLinkRow As DataRow
      Dim tAcObjID As ObjectId
      For Each oRow As DataRow In moIntersectionsTable.Rows
         iEntityNo = DirectCast(oRow.Item(EntityNoFieldName), System.Int32)
         tAcObjID = DirectCast(oRow.Item(AcObjIDFieldName), ObjectId)
         oLinkRow = moLinkTable.Rows.Find(tAcObjID)
         If oLinkRow IsNot Nothing Then
            With oRow
               .Item(msGroupIDFieldName) = oLinkRow.Item(msGroupIDFieldName)
               .Item(msGroupAddIDFieldName) = oLinkRow.Item(msGroupAddIDFieldName)
               .Item(msNameFieldName) = oLinkRow.Item(msNameFieldName)
            End With
         Else
            DMAcadExt.AcadDocument.WriteMessage(" oLinkRow IsNothing =" & iEntityNo.ToString())
         End If
         tAcObjID = DirectCast(oRow.Item(CheckAcObjIDFieldName), ObjectId)
         oLinkRow = moLinkTable.Rows.Find(tAcObjID)
         If oLinkRow IsNot Nothing Then
            With oRow
               .Item(msCheckGroupIDFieldName) = oLinkRow.Item(msGroupIDFieldName)
               .Item(msCheckGroupAddIDFieldName) = oLinkRow.Item(msGroupAddIDFieldName)
               .Item(msCheckNameFieldName) = oLinkRow.Item(msNameFieldName)
            End With
         Else
            DMAcadExt.AcadDocument.WriteMessage(" oCheckLinkRow Is Nothing =" & iEntityNo.ToString())
         End If
      Next
   End Sub
   Public ReadOnly Property CentroidCount As Integer
      Get
         If miSource = enPolygonSetSource.Shapes Then
            Return mdicCentroids.Count
         End If
         If miSource = enPolygonSetSource.Polylines Then
            Return moCentroidTable.Rows.Count
         End If
         Return 0
      End Get
   End Property
   Public Sub CalculateNewA()
      Const dToler As Double = 0.001
      Dim bExcludeCrossing As Boolean = False
      '	Dim iTest As Integer = 0
      Dim tExtents3d As Extents3d
      Dim oLinkDataView As DataView = New DataView(moLinkTable)
      Dim oCentroidDataView As DataView = New DataView(moCentroidTable)
      Dim sRowFilter As String
      Dim oDataRow As DataRow
      Dim oRelationDataRow As DataRow
      Dim oDataViewRow As DataRowView
      Dim oBaseMPolygon As MPolygon
      Dim oResMPolygon As MPolygon
      Dim iRowIndex As Integer = 0
      Dim iPlineNo As Integer = 0

      '	Dim oCheckMPolygon As MPolygon
      Dim oCheckPolyline As Polyline
      Dim tCheckAcObjID As ObjectId
      Dim iCheckNo As Integer

      Dim iNumMPolygonLoops As Integer
      Dim iDir0, iDir1 As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      Dim iParent As Integer
      Dim oMPgonLoop0 As MPolygonLoop
      Dim oMPgonLoop1 As MPolygonLoop
      Dim bBalanceSuccess As Boolean
      Dim bCrossed0 As Boolean
      Dim bCrossed1 As Boolean
      Dim bLoopCrossing0 As Boolean
      Dim bLoopCrossing1 As Boolean
      Dim dArea0, dAreaEnd As Double
      Dim dPLineArea0, dPLineArea1 As Double
      Dim dPLineLen0 As Double
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection
      Dim oParcel As TopoManager.TPlanGraph.TplnParcel
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim tAcObjIDKey(0) As Int64
      Dim bMpgon As Boolean
      Dim lAcObjID As Long
      Dim tIntPtr As System.IntPtr
      Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim iRelationType As enRelationType
      Dim oPolyline As Polyline
      For Each tAcObjID As ObjectId In mcolPolylineIDs
         oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForWrite)
         oDataRow = moLinkTable.Rows.Item(iRowIndex)
         iPlineNo = DirectCast(oDataRow.Item(EntityNoFieldName), Integer)
         oDataRow.Item(msStatusFieldName) = True
         tExtents3d = oPolyline.GeometricExtents
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XmaxFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XminFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YmaxFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YminFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         oLinkDataView.RowFilter = sRowFilter
         ''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged

         oResMPolygon = New MPolygon()

         Try
            oResMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
         Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

            DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & "Res1 Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
         End Try



         If iRowIndex < 10000 Then
            '''''''''INNER loop
            For iIndex As Integer = 0 To oLinkDataView.Count - 1
               bMpgon = False
               bBalanceSuccess = False
               iRelationType = enRelationType.NotExists
               dPLineArea0 = oPolyline.Area
               dPLineLen0 = oPolyline.Length
               oDataViewRow = oLinkDataView.Item(iIndex)
               tCheckAcObjID = DirectCast(oDataViewRow.Item(AcObjIDFieldName), ObjectId)
               '		tCheckAcObjID = zzToAcObjID(oDataViewRow.Item(msAcObjIDFieldName))
               iCheckNo = DirectCast(oDataViewRow.Item(EntityNoFieldName), Integer)
               oCheckPolyline = DMAcadExt.AcadTransaction.GetPolyline(tCheckAcObjID, OpenMode.ForWrite)

               dPLineArea1 = oCheckPolyline.Area
               dPLineLen0 = oCheckPolyline.Length

               oBaseMPolygon = New MPolygon()

               Try
                  oBaseMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
                  oBaseMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                  dArea0 = oBaseMPolygon.Area

                  oBaseMPolygon.BalanceTree()
                  bBalanceSuccess = True
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  bBalanceSuccess = False
                  DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " BP Err=" & oAcadEx.ErrorStatus.ToString())
                  DMAcadExt.AcadDocument.WriteMessage(oPolyline.Handle.ToString() & ":" & oCheckPolyline.Handle.ToString())
               End Try
               If bBalanceSuccess AndAlso oBaseMPolygon.IsBalanced Then
                  dAreaEnd = oBaseMPolygon.Area
                  iNumMPolygonLoops = oBaseMPolygon.NumMPolygonLoops()
                  If iNumMPolygonLoops = 2 Then  '''''''''''Minimal Conditions

                     oMPgonLoop0 = oBaseMPolygon.GetMPolygonLoopAt(0)
                     bCrossed0 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop0, dToler)
                     If bCrossed0 Then
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        'zzDispMPgonLoop(oMPgonLoop0, Drawing.Color.Magenta)
                     End If

                     oMPgonLoop1 = oBaseMPolygon.GetMPolygonLoopAt(1)
                     bCrossed1 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop1, dToler)
                     If bCrossed1 Then
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        'zzDispMPgonLoop(oMPgonLoop0, Drawing.Color.Magenta)
                     End If

                     If bCrossed0 OrElse bCrossed1 Then
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " Crossed = " & bCrossed0.ToString() & " : " & bCrossed1.ToString() & " : " & oPolyline.Handle.ToString() & " : " & oCheckPolyline.Handle.ToString() & vbCrLf)
                        zzDispMPgonLoop(oMPgonLoop0, Drawing.Color.Magenta)
                        zzDispMPgonLoop(oMPgonLoop1, Drawing.Color.Magenta)
                     End If

                     bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
                     bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)


                     ''''''''''''''''Search of islands
                     iDir0 = oBaseMPolygon.GetLoopDirection(0)
                     iDir1 = oBaseMPolygon.GetLoopDirection(1)


                     If iDir0 = LoopDirection.Interior Then
                        iRelationType = enRelationType.Interior
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        iParent = oBaseMPolygon.GetParentLoop(0)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " A = " & iDir0.ToString() & " : " & FormatNumber(oCheckPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        '	zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(1), Drawing.Color.Cyan)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir1 = LoopDirection.Interior Then
                        iRelationType = enRelationType.Exterior
                        '	oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Magenta)
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        '	iParent = oBaseMPolygon.GetParentLoop(1)
                        zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(0), Drawing.Color.DeepPink)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " B = " & iDir1.ToString() & " : " & FormatNumber(oPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir0 = LoopDirection.Interior OrElse iDir1 = LoopDirection.Interior Then
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " MP: " & FormatNumber(dAreaEnd, 3) & vbCrLf)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & " Perim = " & FormatNumber(oBaseMPolygon.Perimeter, 3) & ":" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)) & vbCrLf) ' & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1
                     End If
                     '	DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & "//" & CStr(iCheckNo) & "__" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)))

                     ''''Check neigbors
                     If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                        iRelationType = enRelationType.Neigbour
                     End If
                     If iRelationType <> enRelationType.NotExists Then
                        zzAddRelation(iCheckNo, ObjectId.Null, iRelationType, False, oDataRow)
                        tIntPtr = oCheckPolyline.ObjectId.OldIdPtr
                        lAcObjID = tIntPtr.ToInt64()
                        tAcObjIDKey(0) = lAcObjID
                        oRelationDataRow = moLinkTable.Rows.Find(lAcObjID)

                        If oRelationDataRow IsNot Nothing Then
                           zzAddRelation(iPlineNo, ObjectId.Null, iRelationType, True, oRelationDataRow)
                        End If
                     End If
                     colPoints.Clear()
                     oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
                  End If
               End If
            Next

         End If
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         '	System.Windows.Forms.MessageBox.Show(CStr(sRowFilter), "03_167")
         oCentroidDataView.RowFilter = sRowFilter
         '	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
         '	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
         tCheckAcObjID = ObjectId.Null
         For iIndex As Integer = 0 To oCentroidDataView.Count - 1
            oDataViewRow = oCentroidDataView.Item(iIndex)
            tPoint = New Point3d(DirectCast(oDataViewRow.Item(XFieldName), System.Double), DirectCast(oDataViewRow.Item(YFieldName), System.Double), 0.0)
            'tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId) mm
            iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, dToler)
            If iaLoopInd Is Nothing Then
            ElseIf (iaLoopInd.Count = 1) Then '(iaLoopInd.Count = 1 AndAlso iaLoopInd.Item(0) <> 0) OrElse iaLoopInd.Count > 1 Then
               '	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " LoopInd.Count=" & iaLoopInd.Count & " #" & CStr(iaLoopInd.Item(0)) & vbCrLf)


               tCheckAcObjID = zzToAcObjID(oDataViewRow.Item(AcObjIDFieldName))
               Exit For

            End If
         Next

         ''''''''''''''	DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)
         If tCheckAcObjID.IsNull Then
            Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
            DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " Point was not founded" & oTPlnBoundingBox.Coordinates & vbCrLf)
         End If
         '''''''''	Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaBlockAttribIndex)
         'System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & oResMPolygon.GetType().ToString() & ":" & tCheckAcObjID.ToString(), "03_210")

         oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, 0, tCheckAcObjID)
         '	System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & tCheckAcObjID.ToString(), "03_550")
         oXDataParcel = New DMAcadExt.TplnXDataParcel()
         oXDataParcel.ID = iPlineNo
         oXDataParcel.DataID = iPlineNo

         oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels

         oXDataParcel.Block = oParcel.BlockNo
         oXDataParcel.BlockAdd = 0
         If oParcel.Name IsNot Nothing Then
            oXDataParcel.Name = oParcel.Name
         End If

         oXDataParcel.LegalArea = oParcel.LegalArea(False)
         oXDataParcel.Status = 1 ' tParcelData.Status

         oXDataParcel.AcadArea = Math.Abs(oResMPolygon.Area)
         oXDataParcel.Perimeter = oResMPolygon.Perimeter
         If Not IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
            oXDataParcel.NeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String)
         Else
            oXDataParcel.NeigborList = String.Empty
         End If
         If Not IsDBNull(oDataRow.Item(msExteriorFieldName)) Then
            oXDataParcel.Exterior = DirectCast(oDataRow.Item(msExteriorFieldName), Integer)
         Else
            oXDataParcel.Exterior = 0
         End If
         If Not IsDBNull(oDataRow.Item(msInteriorListFieldName)) Then
            oXDataParcel.InteriorList = DirectCast(oDataRow.Item(msInteriorListFieldName), String)
         Else
            oXDataParcel.InteriorList = ""
         End If

         '		oResMPolygon.XData = oXDataParcel.GetResBuffer()
         oPolyline.XData = oXDataParcel.GetResBuffer()

         If Not tCheckAcObjID.IsNull Then
            oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCheckAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            oCentroidDBObject.XData = oXDataParcel.GetResBuffer()
         Else
            DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " ??LoopInd.Count=" & oCentroidDataView.Count & vbCrLf)
         End If
         iRowIndex += 1

      Next
   End Sub
   
   Public Sub CalculateNewE()
      Const dDelta As Double = 0.001
      Dim tExtents3d As Extents3d
      Dim oLinkDataView As DataView = New DataView(moLinkTable)
      Dim oCentroidDataView As DataView = New DataView(moCentroidTable)
      Dim sRowFilter As String
      ''''''''''''''''	Dim oLinkDataRow As DataRow
      Dim oCheckDataRowView As DataRowView
      Dim oaCheckDataRowView() As DataRowView
      '		Dim oBaseMPolygon As MPolygon

      '	Dim iRowIndex As Integer = 0
      Dim iPLineID As Integer
      Dim iGroupID As Integer
      Dim iCheckPLineID As Integer
      Dim iPLineIndex As Integer
      Dim oResMPolygon As MPolygon
      Dim oRing As TopoManager.tmRing
      Dim oCheckRing As TopoManager.tmRing
      Dim oTopoPolygon As TopoManager.tmPolygon = Nothing
      Dim oCheckMPolygon As MPolygon
      Dim oCheckPolyline As Polyline
      Dim tCheckAcObjID As ObjectId

      Dim tCentroidAcObjID As ObjectId

      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection

      Dim oXDataBasePgon As DMAcadExt.TplnXDataBasePgon = Nothing
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel = Nothing
      Dim oXDataBlock As DMAcadExt.TplnXDataBlock = Nothing

      Dim oXDataLot As DMAcadExt.TplnXDataLot = Nothing

      'Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(Me.AppName)
      System.Windows.Forms.MessageBox.Show(miMapTheme.ToString() & vbCrLf & Me.AppName, "10_125A")
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim tAcObjIDKey(0) As Int64
      'Dim bMpgon As Boolean
      Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim iRelationType As enRelationType
      Dim bIsCorrect As Boolean
      Dim oaPseudoVertices() As TopoManager.tmVertex
      Dim oPolyline As Polyline
      Dim oBlockRef As BlockReference = Nothing
      Dim oFindCenter As FindCenter
      Dim oPgonCenter As DMAcadExt.TPlnPoint
      Dim oPgonRelation As PgonRelation
      Dim tAcObjID As ObjectId
      Dim bStatus As Boolean
      Dim sMsg As String
      DMAcadExt.AcadDocument.OpenLog(False)
      Dim tGroupKey As GroupKey
      PgonRelation.RegApp()
      '	System.Windows.Forms.MessageBox.Show(miSource.ToString & vbCrLf & CStr(moLinkTable.Rows.Count) & vbCrLf & CStr(Me.CentroidCount), "03_177C")
      ' enPolygonSetSource.Polylines
      Dim colCentroidsObjID As ObjectIdCollection
      Dim colIntersectPoints As Point3dCollection
      Dim oaNodes() As TopoManager.tmNode
      Dim oaAddNodes() As TopoManager.tmNode

      Dim oaRings(moLinkTable.Rows.Count - 1) As TopoManager.tmRing
      If moWorkAreaBound Is Nothing Then Exit Sub

      'TopoManager.tmVertices.SetRoundDec(5)
      TopoManager.tmVertices.SetTolerance(0.0001)

      miSource = enPolygonSetSource.Polylines
      moNodes = New TopoManager.tmNodes
      '	ReDim moaTopoPolygons(moLinkTable.Rows.Count - 1)
      mdicTmPolygons = New TopoManager.tmPolygons
      For Each oLinkDataRow As DataRow In moLinkTable.Rows
         bStatus = DirectCast(oLinkDataRow.Item(msStatusFieldName), Boolean)

         If True Or Not bStatus Then
            tAcObjID = zzToAcObjID(oLinkDataRow.Item(AcObjIDFieldName))
            oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForWrite)
            If miSource = enPolygonSetSource.Shapes Then
               iPLineID = DMCommon.Functions.CIntN(oLinkDataRow.Item(msIDFieldName))
               iGroupID = DMCommon.Functions.CIntN(oLinkDataRow.Item(msGroupIDFieldName))
            ElseIf miSource = enPolygonSetSource.Polylines Then
               iPLineID = DirectCast(oLinkDataRow.Item(EntityNoFieldName), Integer)
            End If

            If miMapTheme = DMAcadExt.enMapTheme.Parcels Then
               oXDataParcel = New DMAcadExt.TplnXDataParcel()
               oXDataBasePgon = oXDataParcel
            ElseIf miMapTheme = DMAcadExt.enMapTheme.UD_Blocks OrElse miMapTheme = DMAcadExt.enMapTheme.Blocks Then
               oXDataBlock = New DMAcadExt.TplnXDataBlock()
               oXDataBasePgon = oXDataBlock
            ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved Then
               oXDataLot = New DMAcadExt.TplnXDataLot(Me.AppName)
               oXDataBasePgon = oXDataLot
            End If

            'DMAcadExt.AcadDocument.WriteMessage(CStr(iPLineID) & ":" & oPolyline.Handle.ToString() & "=" & iRowIndex.ToString())
            oLinkDataRow.Item(msStatusFieldName) = True
            tExtents3d = oPolyline.GeometricExtents
            sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XmaxFieldName & " >" & (tExtents3d.MinPoint.X - dDelta).ToString & ") AND (" & XminFieldName & " <" & (tExtents3d.MaxPoint.X + dDelta).ToString & ") AND (" & YmaxFieldName & ">" & (tExtents3d.MinPoint.Y - dDelta).ToString & ") AND (" & YminFieldName & "<" & (tExtents3d.MaxPoint.Y + dDelta).ToString & ")"
            oLinkDataView.RowFilter = sRowFilter
            'DMAcadExt.AcadDocument.WriteMessage("sRowFilter" & "=" & CStr(sRowFilter))
            ''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged
            oResMPolygon = zzGetMPolygon(oLinkDataRow)
            Try
               If Not oPolyline.Closed Then
                  oPolyline.Closed = True
               End If
               oResMPolygon.AppendLoopFromBoundary(oPolyline, mbExcludeCrossing, mdToler)
               oResMPolygon.BalanceTree()
            Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
               DMAcadExt.AcadDocument.WriteMessage(CStr(iPLineID) & ":" & oPolyline.Handle.ToString() & " Err #02=" & oAcadEx.ErrorStatus.ToString())
            End Try

            If oResMPolygon.IsBalanced Then
               'DMAcadExt.AcadDocument.WriteMessage("****PLineID" & "=" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " of " & CStr(moLinkTable.Rows.Count))
               iPLineIndex = iPLineID - 1
               If oaRings(iPLineIndex) Is Nothing Then
                  oaRings(iPLineIndex) = New TopoManager.tmRing(iPLineID, oPolyline)
               End If
               oRing = oaRings(iPLineIndex)
               oPolyline.IntersectWith(moWorkAreaBound, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))

               If colPoints IsNot Nothing Then
                  For Each tExtPoint As Point3d In colPoints
                     DMAcadExt.AcadTransaction.InsertPoint(tExtPoint)
                  Next
               End If

               oaNodes = moNodes.AddNodes(iPLineID, 0, colPoints)
               oaPseudoVertices = oRing.AddNodes(oaNodes)
               If oaPseudoVertices IsNot Nothing AndAlso oaPseudoVertices.GetUpperBound(0) >= 0 Then
                  oaAddNodes = moNodes.AddNodes(iPLineID, 0, oaPseudoVertices)

               End If

               '	DMAcadExt.AcadDocument.WriteMessage("--EndOf PLineID" & "=" & CStr(iPLineID))

               If oLinkDataView.Count > 0 Then
                  '''''''''''''''''''''''''''''INNER Lines loop 
                  ReDim oaCheckDataRowView(oLinkDataView.Count - 1)
                  For iIndex As Integer = 0 To oLinkDataView.Count - 1
                     oaCheckDataRowView(iIndex) = oLinkDataView.Item(iIndex)
                  Next

                  For iCheckIndex As Integer = 0 To oaCheckDataRowView.GetUpperBound(0)
                     Try
                        oCheckDataRowView = oaCheckDataRowView(iCheckIndex)
                     Catch oEx As Exception
                        System.Windows.Forms.MessageBox.Show(oEx.Message, "04_711")
                        Exit For
                     End Try

                     tCheckAcObjID = zzToAcObjID(oCheckDataRowView.Item(AcObjIDFieldName))
                     iCheckPLineID = DirectCast(oCheckDataRowView.Item(EntityNoFieldName), Integer)

                     oCheckPolyline = DMAcadExt.AcadTransaction.GetPolyline(tCheckAcObjID, OpenMode.ForWrite)
                     If Not oCheckPolyline.Closed Then
                        oCheckPolyline.Closed = True
                     End If
                     '	DMAcadExt.AcadDocument.WriteMessage("CheckPLineID=" & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " of " & CStr(oLinkDataView.Count))
                     oCheckMPolygon = New MPolygon()
                     '	oCheckRing = New TopoManager.tmRing(iCheckPLineID, oCheckPolyline)
                     iPLineIndex = iCheckPLineID - 1
                     If oaRings(iPLineIndex) Is Nothing Then
                        oaRings(iPLineIndex) = New TopoManager.tmRing(iCheckPLineID, oCheckPolyline)
                     End If
                     oCheckRing = oaRings(iPLineIndex)
                     Try
                        oCheckMPolygon.AppendLoopFromBoundary(oCheckPolyline, mbExcludeCrossing, mdToler)
                        oCheckMPolygon.BalanceTree()
                     Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Err#04=" & oAcadEx.ErrorStatus.ToString())
                     End Try

                     If oCheckMPolygon.IsBalanced Then
                        sMsg = String.Empty
                        ''''''''''''''''''''''	nnnnnnnnnnn()
                        oPgonRelation = New PgonRelation(oPolyline, oCheckPolyline)
                        oPgonRelation.Calculate(True, False)



                        sMsg = oPgonRelation.ErrorStatus
                        If oPgonRelation.ErrorPointsCount > 0 Then
                           sMsg &= "; " & CStr(oPgonRelation.ErrorPointsCount) & " intersection point(s)"
                           DMAcadExt.AcadDocument.WriteMessageLog("Crossing Points:" & CStr(oPgonRelation.ErrorPointsCount) & " - " & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString())

                        End If
                        If sMsg.Length <> 0 Then
                           DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " " & sMsg)
                        End If


                        iRelationType = oPgonRelation.RelationType
                        bIsCorrect = oPgonRelation.IsCorrect

                        '	DMAcadExt.AcadDocument.WriteMessage(iRelationType.ToString() & "*I*" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString())
                        ''''Check neigbors

                        If iRelationType <> enRelationType.NotExists Then
                           zzAddRelation(iCheckPLineID, oCheckPolyline.ObjectId, iRelationType, False, oLinkDataRow)
                           zzAddRelation(iPLineID, oPolyline.ObjectId, iRelationType, True, oCheckDataRowView.Row)
                        End If

                        If oResMPolygon IsNot Nothing AndAlso iRelationType = enRelationType.Exterior AndAlso bIsCorrect Then
                           ''///oRing - Exterior;  oCheckRing - Interior

                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, mbExcludeCrossing, mdToler)
                           mdicTmPolygons.AddRingPair(oRing, oCheckRing)
                        End If

                        If oResMPolygon IsNot Nothing AndAlso iRelationType = enRelationType.Interior AndAlso bIsCorrect Then
                           ''///oCheckRing - Exterior;  oRing - Interior
                           If False And Not mdicTmPolygons.TryGetValue(iCheckPLineID, oTopoPolygon) Then
                              oTopoPolygon = New TopoManager.tmPolygon(oRing)
                              mdicTmPolygons.Add(iCheckPLineID, New TopoManager.tmPolygon(oRing))
                           End If

                           mdicTmPolygons.AddRingPair(oCheckRing, oRing)

                        End If


                        If iRelationType = enRelationType.Neigbour AndAlso bIsCorrect Then
                           'DMAcadExt.AcadDocument.WriteMessageLog("!@!Touch Points:" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Count=" & CStr(oPgonRelation.IntersectPointCount))
                           '''''oPgonRelation.DrawIntersectPoints()
                           colIntersectPoints = oPgonRelation.IntersectPoints

                           If colIntersectPoints IsNot Nothing AndAlso colIntersectPoints.Count <> 0 Then
                              If (iPLineID = -4) Or (oCheckRing.ID = -4) Then
                                 ''''''''DMAcadExt.AcadDocument.WriteMessageLog("Before 1x4" & CStr(moNodes.Count))
                                 oPgonRelation.PrintIntersectPoints(iPLineID, iCheckPLineID)
                              End If
                              oaNodes = moNodes.AddNodes(iPLineID, iCheckPLineID, colIntersectPoints)
                              If (iPLineID = 1) And (oCheckRing.ID = 2) Then
                                 For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
                                    oaNodes(iIndex).DebugWrite()
                                 Next
                              End If

                              Dim s1 As String = "Pseudo - NO"
                              oaPseudoVertices = oRing.AddNodes(oaNodes)
                              If oaPseudoVertices IsNot Nothing Then
                                 s1 = CStr(oaPseudoVertices.GetUpperBound(0))
                                 If oaPseudoVertices.GetUpperBound(0) >= 0 Then
                                    s1 &= "@@" & oaPseudoVertices(0).Point3d.ToString()
                                 End If
                              End If

                              If (iPLineID = 1) Or (oCheckRing.ID = 2) Then
                                 '	DMAcadExt.AcadDocument.WriteMessage(oRing.ID & "; H=" & oRing.Handle & "; Ps-" & s1)
                              End If

                              oCheckRing.ConnectNodes(oaNodes, "")
                              If oaPseudoVertices IsNot Nothing AndAlso oaPseudoVertices.GetUpperBound(0) >= 0 Then
                                 oaAddNodes = moNodes.AddNodes(iPLineID, iCheckPLineID, oaPseudoVertices)
                                 oCheckRing.ConnectNodes(oaAddNodes, "")
                              End If

                              If oRing.ID = -2 AndAlso oCheckRing.ID = -3 Then
                                 DMAcadExt.AcadDocument.WriteMessageLog("After  1x4" & CStr(moNodes.Count))

                              End If
                              '	oRing.TmpDisplayVertices()
                              '	oCheckRing.TmpDisplayVertices()
                           End If

                        End If
                        ''''''''''	colPoints.Clear()
                        '''''''''''''''	oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
                     Else
                        oCheckDataRowView.Item(msStatusFieldName) = True
                        oCheckPolyline.XData = oXDataBasePgon.GetNullResBuffer()
                        DMAcadExt.AcadDocument.WriteMessageLog(CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Err#22 Polygon is incorrect")
                     End If
                  Next iCheckIndex  'i  in oLinkDataView End of INNER Integer loop
               End If
               If oRing.IsExterior = TriState.UseDefault Then

                  mdicTmPolygons.AddIfNotExists(oRing)
                  'System.Windows.Forms.MessageBox.Show("RingID " & oRing.ID.ToString() & vbCrLf & CStr(mdicTopoPolygons.Count), "15_633")
               Else
                  'System.Windows.Forms.MessageBox.Show("RingID = " & oRing.ID.ToString() & vbCrLf & oRing.IsExterior.ToString() & vbCrLf & CStr(mdicTopoPolygons.Count), "15_698")
               End If

               ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
               If oResMPolygon IsNot Nothing Then
                  oResMPolygon.BalanceTree()
               End If

               '	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
               '	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
               If miSource = enPolygonSetSource.Shapes Then
                  tGroupKey = New GroupKey(iGroupID, iPLineID)
                  If oResMPolygon IsNot Nothing AndAlso mdicCentroids.TryGetValue(tGroupKey, tCentroidAcObjID) Then
                     oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tCentroidAcObjID, OpenMode.ForWrite)
                     If oBlockRef IsNot Nothing Then
                        tPoint = oBlockRef.Position
                        If oResMPolygon.NumMPolygonLoops > 1 Then
                           'System.Windows.Forms.MessageBox.Show(CStr(iGroupID) & "/" & CStr(iPLineID) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y), "03_165M")
                        End If
                        iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, mdToler)
                        If iaLoopInd IsNot Nothing AndAlso (iaLoopInd.Count = 1) Then
                           Dim iLoopNo As Integer = iaLoopInd.Item(0)
                           If oResMPolygon.NumMPolygonLoops > 1 Then
                              'System.Windows.Forms.MessageBox.Show(CStr(iPLineID) & ":" & CStr(iLoopNo) & "," & oResMPolygon.GetLoopDirection(iLoopNo).ToString(), "03_168R")
                           End If
                           If oResMPolygon.GetLoopDirection(iLoopNo) <> LoopDirection.Exterior Then
                              oFindCenter = New FindCenter(oResMPolygon)
                              oPgonCenter = oFindCenter.GetInnerPoint()
                              If oPgonCenter IsNot Nothing Then
                                 DMAcadExt.AcadTransaction.MoveBlockRef(oBlockRef, oPgonCenter.GetPoint3d())
                              End If
                              DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " CentroidNotFound In pgon#" & CStr(iPLineID))
                           End If
                        Else
                           'Err
                        End If
                     End If
                  Else
                     'ERRR
                     DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " CentroidNotFound In Dict #" & CStr(iPLineID))
                     oBlockRef = Nothing
                  End If
                  oXDataParcel = New DMAcadExt.TplnXDataParcel(oPolyline.XData)
               Else ' miSource = enPolygonSetSource.Polylines  
                  If oResMPolygon.NumMPolygonLoops > 1 Then
                     DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " has " & CStr(oResMPolygon.NumMPolygonLoops - 1) & " island(s)")
                  End If
                  colCentroidsObjID = zzGetCentroidsInMPgon(oResMPolygon, oPolyline.Handle.ToString())


                  oXDataBasePgon.ID = iPLineID
                  oXDataBasePgon.DataID = iPLineID
                  '	DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " oXDataParcel.ID = " & CStr(oXDataParcel.ID) & "  " & oParcel.CentroidX & "," & oParcel.CentroidY & vbCrLf)
                  oXDataBasePgon.MapTheme = miMapTheme
                  ''''''''''''''	DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)
                  If colCentroidsObjID.Count = 1 Then
                     tCentroidAcObjID = colCentroidsObjID.Item(0)
                     'oEditor.WriteMessage(CStr(iRowIndex) & " CentroidID=" & colCentroidsObjID.Item(0).ToString() & vbCrLf)

                     If miMapTheme = DMAcadExt.enMapTheme.Parcels Then

                        Dim oParcel As TopoManager.TPlanGraph.TplnParcel

                        oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, iPLineID, tCentroidAcObjID)
                        oXDataParcel.Block = oParcel.BlockNo
                        oXDataParcel.BlockAdd = 0
                        If oParcel.Name IsNot Nothing Then
                           oXDataBasePgon.Name = oParcel.Name
                        End If

                        oXDataParcel.LegalArea = oParcel.LegalArea(False)
                        oXDataParcel.Status = 1 ' tParcelData.Status
                        oLinkDataRow.Item(msGroupIDFieldName) = oParcel.BlockNo
                     ElseIf miMapTheme = DMAcadExt.enMapTheme.Blocks OrElse miMapTheme = DMAcadExt.enMapTheme.UD_Blocks Then
                        Dim oBlock As TopoManager.TPlanGraph.TplnBlock

                        oBlock = New TopoManager.TPlanGraph.TplnBlock(oResMPolygon, tCentroidAcObjID)

                        oXDataBlock.Block = oBlock.BlockNo
                        oXDataBlock.BlockAdd = oBlock.BlockAddNo

                        DMAcadExt.AcadDocument.WriteMessage("**!!!" & oXDataBlock.Block.ToString() & ", " & oBlock.BlockNo.ToString())

                     ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved Then
                        Dim oLot As TopoManager.TPlanGraph.TplnLot
                        oLot = New TopoManager.TPlanGraph.TplnLot(miTopoPurpose, oResMPolygon, iPLineID, tCentroidAcObjID)
                        If oLot.Name IsNot Nothing Then
                           oXDataBasePgon.Name = oLot.Name
                        End If
                     End If


                  ElseIf colCentroidsObjID.Count = 0 Then
                     Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
                     DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " Point was not founded!" & oTPlnBoundingBox.Coordinates)
                  Else
                     Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
                     DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & ":" & CStr(colCentroidsObjID.Count) & " points were founded" & oTPlnBoundingBox.Coordinates)
                  End If
               End If  'miSource = enPolygonSetSource.Shapes

               If oXDataBasePgon IsNot Nothing Then
                  oXDataBasePgon.AcadArea = Math.Abs(oResMPolygon.Area)
                  oXDataBasePgon.Perimeter = oResMPolygon.Perimeter

                  If Not IsDBNull(oLinkDataRow.Item(msNeigborListFieldName)) Then
                     oXDataBasePgon.NeigborList = DirectCast(oLinkDataRow.Item(msNeigborListFieldName), String)
                  Else
                     oXDataBasePgon.NeigborList = ""
                  End If

                  If Not IsDBNull(oLinkDataRow.Item(msExteriorFieldName)) Then
                     'System.Windows.Forms.MessageBox.Show(miMapTheme.ToString() & vbCrLf & CStr(oXDataBasePgon IsNot Nothing), "03_134$")
                     'DMAcadExt.AcadDocument.WriteMessage(CStr(oXDataParcel.ImpID) & " Has Exterior: " & CStr(DirectCast(oLinkDataRow.Item(msExteriorFieldName), Integer)))
                     oXDataBasePgon.Exterior = DirectCast(oLinkDataRow.Item(msExteriorFieldName), Integer)
                  Else
                     oXDataBasePgon.Exterior = 0
                  End If
                  If Not IsDBNull(oLinkDataRow.Item(msInteriorListFieldName)) Then
                     'DMAcadExt.AcadDocument.WriteMessage(CStr(oXDataParcel.ImpID) & "|" & CStr(iPLineID) & " Has InteriorList: " & DirectCast(oLinkDataRow.Item(msInteriorListFieldName), String))
                     oXDataBasePgon.InteriorList = DirectCast(oLinkDataRow.Item(msInteriorListFieldName), String)

                  Else
                     oXDataBasePgon.InteriorList = String.Empty
                     oXDataBasePgon.InteriorCount = 0
                  End If

                  '		oResMPolygon.XData = oXDataParcel.GetResBuffer()
                  oPolyline.XData = oXDataBasePgon.GetResBuffer()
                  If oBlockRef IsNot Nothing Then

                  End If
                  If Not tCentroidAcObjID.IsNull Then
                     oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                     oCentroidDBObject.XData = oXDataBasePgon.GetResBuffer()
                  Else
                     DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " ??LoopInd.Count=" & oCentroidDataView.Count)
                  End If
               Else
                  DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " XData Is Not Exist")
               End If 'oXDataBasePgon IsNot Nothing
            Else 'Not oResMPolygon.IsBalanced
               oPolyline.XData = oXDataBasePgon.GetNullResBuffer()
               DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " Loops=" & oResMPolygon.NumMPolygonLoops & " Err#21 Polygon is incorrect")
            End If
         Else
            System.Windows.Forms.MessageBox.Show("DESIGN ERROR " & miMapTheme.ToString(), "15_450")
         End If

         If iPLineID > 100000 Then
            Exit For
         End If

         'Next 'mcolPolylineIDs
      Next
      '''''''''''''''	moNodes.DebugWriteAll()
      '	System.Windows.Forms.MessageBox.Show("LastID " & iPLineID.ToString() & vbCrLf & CStr(oLinkDataView.Count) & vbCrLf & CStr(mdicTopoPolygons.Count), "15_670")
      For Each oPgon As TopoManager.tmPolygon In mdicTmPolygons.Values
         If oPgon.IslandCount <> 0 Then
            'System.Windows.Forms.MessageBox.Show("Island   " & CStr(oPgon.IslandCount) & vbCrLf & oPgon.ID.ToString(), "15_466")
         End If

      Next

      DMAcadExt.AcadDocument.WriteMessage("Source = " & miSource.ToString())


   End Sub

   Public Sub CalculateNewF(bCheckIntersection As Boolean)
      Const dDelta As Double = 0.001
      Dim oTestDB As DBObject
      Dim tExtents3d As Extents3d
      Dim oLinkDataView As DataView = New DataView(moLinkTable)
      Dim oCentroidDataView As DataView = New DataView(moCentroidTable)
      Dim sRowFilter As String
      ''''''''''''''''	Dim oLinkDataRow As DataRow
      Dim oCheckDataRowView As DataRowView
      Dim oaCheckDataRowView() As DataRowView
      '		Dim oBaseMPolygon As MPolygon

      '	Dim iRowIndex As Integer = 0
      Dim iPLineID As Integer
      Dim iGroupID As Integer
      Dim iGroupAddID As Integer

      Dim iCheckPLineID As Integer
      Dim iPLineIndex As Integer
      Dim oResMPolygon As MPolygon
      Dim oRing As TopoManager.tmRing
      Dim oCheckRing As TopoManager.tmRing
      Dim oTopoPolygon As TopoManager.tmPolygon = Nothing
      Dim oCheckMPolygon As MPolygon
      Dim oCheckPolyline As Polyline
      Dim tCheckAcObjID As ObjectId

      Dim tCentroidAcObjID As ObjectId

      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection

      Dim oXDataBasePgon As DMAcadExt.TplnXDataBasePgon = Nothing
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel = Nothing
      Dim oXDataLot As DMAcadExt.TplnXDataLot = Nothing
      Dim oXDataBlock As DMAcadExt.TplnXDataBlock = Nothing
      Dim moCentroidAcadBlok As DMAcadExt.AcadBlock = Nothing
      Dim tBlockRefData As DMAcadExt.BlockRefData


		'Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(Me.AppName)
      '  System.Windows.Forms.MessageBox.Show(miMapTheme.ToString() & vbCrLf & miSource.ToString() & vbCrLf & Me.AppName & vbCrLf & bRes.ToString(), "03_125C")
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim tAcObjIDKey(0) As Int64
      'Dim bMpgon As Boolean
      Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim iRelationType As enRelationType
      Dim bIsCorrect As Boolean
      Dim oaPseudoVertices() As TopoManager.tmVertex
      Dim oPolyline As Polyline
      Dim oBlockRef As BlockReference = Nothing
      Dim oFindCenter As FindCenter
      Dim oPgonCenter As DMAcadExt.TPlnPoint
      Dim oPgonRelation As PgonRelation
      Dim tAcObjID As ObjectId
      Dim bStatus As Boolean
      Dim sMsg As String
      '  Dim sAtributeTextStr As String

      DMAcadExt.AcadDocument.OpenLog(False)
      Dim tGroupKey As GroupKey
      PgonRelation.RegApp()
      '    System.Windows.Forms.MessageBox.Show(miSource.ToString & vbCrLf & CStr(moLinkTable.Rows.Count) & vbCrLf & CStr(Me.CentroidCount) & vbCrLf & miMapTheme.ToString(), "03_177C")
      ' enPolygonSetSource.Polylines
      Dim colCentroidsObjID As ObjectIdCollection
      Dim colIntersectPoints As Point3dCollection
      Dim oaNodes() As TopoManager.tmNode
      Dim oaAddNodes() As TopoManager.tmNode

      Dim oaRings(moLinkTable.Rows.Count - 1) As TopoManager.tmRing
		Dim iDebugIndex As Integer = 0
		Dim oIntersectBox As DMAcadExt.TPlnBoundingBox
      '	If moWorkAreaBound Is Nothing Then Exit Sub

      'TopoManager.tmVertices.SetRoundDec(5)
      TopoManager.tmVertices.SetTolerance(0.0001)

      miSource = enPolygonSetSource.Polylines
      moNodes = New TopoManager.tmNodes
      '	ReDim moaTopoPolygons(moLinkTable.Rows.Count - 1)
      mdicTmPolygons = New TopoManager.tmPolygons
		mdicTopoPolygons = New Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)()


		'@@@@@@@@@'oLinkDataRow

		For Each oLinkDataRow As DataRow In moLinkTable.Rows
			bStatus = DirectCast(oLinkDataRow.Item(msStatusFieldName), Boolean)
			iDebugIndex += 1
			If True Or Not bStatus Then
				tAcObjID = zzToAcObjID(oLinkDataRow.Item(AcObjIDFieldName))
				oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForWrite)
				If miSource = enPolygonSetSource.Shapes Then
					iPLineID = DMCommon.Functions.CIntN(oLinkDataRow.Item(msIDFieldName))
					iGroupID = DMCommon.Functions.CIntN(oLinkDataRow.Item(msGroupIDFieldName))
					iGroupAddID = DMCommon.Functions.CIntN(oLinkDataRow.Item(msGroupAddIDFieldName))
				ElseIf miSource = enPolygonSetSource.Polylines Then
					iPLineID = DirectCast(oLinkDataRow.Item(EntityNoFieldName), Integer)
				End If

				If miMapTheme = DMAcadExt.enMapTheme.Parcels Then
					oXDataParcel = New DMAcadExt.TplnXDataParcel()
					oXDataBasePgon = oXDataParcel
				ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved OrElse miMapTheme = DMAcadExt.enMapTheme.LotProposed Then
					oXDataLot = New DMAcadExt.TplnXDataLot(Me.AppName)
					oXDataBasePgon = oXDataLot
				ElseIf miMapTheme = DMAcadExt.enMapTheme.Blocks Then
					oXDataBlock = New DMAcadExt.TplnXDataBlock
					oXDataBasePgon = oXDataBlock
				ElseIf miMapTheme = DMAcadExt.enMapTheme.UD_Parcels Then
					oXDataParcel = New DMAcadExt.TplnXDataParcel()
					oXDataBasePgon = oXDataParcel
				ElseIf miMapTheme = DMAcadExt.enMapTheme.UD_Blocks Then
					oXDataBlock = New DMAcadExt.TplnXDataBlock()
					oXDataBasePgon = oXDataBlock
				End If

				'DMAcadExt.AcadDocument.WriteMessage(CStr(iPLineID) & ":" & oPolyline.Handle.ToString() & "=" & iRowIndex.ToString())
				oLinkDataRow.Item(msStatusFieldName) = True
				tExtents3d = oPolyline.GeometricExtents
				sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XmaxFieldName & " >" & (tExtents3d.MinPoint.X - dDelta).ToString & ") AND (" & XminFieldName & " <" & (tExtents3d.MaxPoint.X + dDelta).ToString & ") AND (" & YmaxFieldName & ">" & (tExtents3d.MinPoint.Y - dDelta).ToString & ") AND (" & YminFieldName & "<" & (tExtents3d.MaxPoint.Y + dDelta).ToString & ")"
				oLinkDataView.RowFilter = sRowFilter
				'DMAcadExt.AcadDocument.WriteMessage("sRowFilter" & "=" & CStr(sRowFilter))
				''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged
				oResMPolygon = zzGetMPolygon(oLinkDataRow)
				Try
					If Not oPolyline.Closed Then
						oPolyline.Closed = True
					End If
					oResMPolygon.AppendLoopFromBoundary(oPolyline, mbExcludeCrossing, mdToler)
					oResMPolygon.BalanceTree()
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					DMAcadExt.AcadDocument.WriteMessage(CStr(iPLineID) & ":" & oPolyline.Handle.ToString() & " Err#01=" & oAcadEx.ErrorStatus.ToString())
				End Try

				If oResMPolygon.IsBalanced Then
					'DMAcadExt.AcadDocument.WriteMessage("****PLineID" & "=" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " of " & CStr(moLinkTable.Rows.Count))
					iPLineIndex = iPLineID - 1
					If oaRings(iPLineIndex) Is Nothing Then
						oaRings(iPLineIndex) = New TopoManager.tmRing(iPLineID, oPolyline)
					End If
					oRing = oaRings(iPLineIndex)


					'	DMAcadExt.AcadDocument.WriteMessage("--EndOf PLineID" & "=" & CStr(iPLineID))

					If oLinkDataView.Count > 0 Then
						'''''''''''''''''''''''''''''INNER Lines loop 
						ReDim oaCheckDataRowView(oLinkDataView.Count - 1)
						For iIndex As Integer = 0 To oLinkDataView.Count - 1
							oaCheckDataRowView(iIndex) = oLinkDataView.Item(iIndex)
						Next
31:               '@@@@@@@@@'iCheckIndex
						For iCheckIndex As Integer = 0 To oaCheckDataRowView.GetUpperBound(0)
							Try
								oCheckDataRowView = oaCheckDataRowView(iCheckIndex)
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message, "04_711")
								Exit For
							End Try

							tCheckAcObjID = zzToAcObjID(oCheckDataRowView.Item(AcObjIDFieldName))
							iCheckPLineID = DirectCast(oCheckDataRowView.Item(EntityNoFieldName), Integer)

							oCheckPolyline = DMAcadExt.AcadTransaction.GetPolyline(tCheckAcObjID, OpenMode.ForWrite)
							If Not oCheckPolyline.Closed Then
								oCheckPolyline.Closed = True
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("CheckPLineID=" & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " of " & CStr(oLinkDataView.Count))
							oCheckMPolygon = New MPolygon()
							'	oCheckRing = New TopoManager.tmRing(iCheckPLineID, oCheckPolyline)
							iPLineIndex = iCheckPLineID - 1
							If oaRings(iPLineIndex) Is Nothing Then
								oaRings(iPLineIndex) = New TopoManager.tmRing(iCheckPLineID, oCheckPolyline)
							End If
							oCheckRing = oaRings(iPLineIndex)
							Try
								oCheckMPolygon.AppendLoopFromBoundary(oCheckPolyline, mbExcludeCrossing, mdToler)
								oCheckMPolygon.BalanceTree()
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								DMAcadExt.AcadDocument.WriteMessage(CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Err#04=" & oAcadEx.ErrorStatus.ToString())
							End Try

							If oCheckMPolygon.IsBalanced Then
								sMsg = String.Empty

								oPgonRelation = New PgonRelation(oRing, oCheckRing)
								oPgonRelation.Calculate(bCheckIntersection, False)

								If oPgonRelation.IntersectPointCount > 0 AndAlso Not oPgonRelation.IsTouch Then
									oIntersectBox = oPgonRelation.GetIntersectBoundingBox()
									zzAddIntersection(oRing, oLinkDataRow, oCheckRing, oPgonRelation.IntersectPointCount, oIntersectBox)
									oPgonRelation.MarkIntersectionPoints(False)
								End If

								sMsg = oPgonRelation.ErrorStatus
								If oPgonRelation.ErrorPointsCount > 0 Then
									sMsg &= "; " & CStr(oPgonRelation.ErrorPointsCount) & " intersection point(s)"
									DMAcadExt.AcadDocument.WriteMessageLog("Crossing Points:" & CStr(oPgonRelation.ErrorPointsCount) & " - " & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString())

								End If
								If sMsg.Length <> 0 Then
									DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " " & sMsg)
								End If


								iRelationType = oPgonRelation.RelationType
								bIsCorrect = oPgonRelation.IsCorrect

								'	DMAcadExt.AcadDocument.WriteMessage(iRelationType.ToString() & "*I*" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString())
								''''Check neigbors

								If iRelationType <> enRelationType.NotExists Then

									zzAddRelation(iCheckPLineID, oCheckPolyline.ObjectId, iRelationType, False, oLinkDataRow)
									zzAddRelation(iPLineID, oPolyline.ObjectId, iRelationType, True, oCheckDataRowView.Row)
								End If
								If oResMPolygon IsNot Nothing AndAlso iRelationType = enRelationType.Exterior AndAlso bIsCorrect Then
									''///oRing - Exterior;  oCheckRing - Interior
									oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, mbExcludeCrossing, mdToler)
									oTopoPolygon = mdicTmPolygons.AddRingPair(oRing, oCheckRing)
									'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  
								End If
								If oResMPolygon IsNot Nothing AndAlso iRelationType = enRelationType.Interior AndAlso bIsCorrect Then
									''///oCheckRing - Exterior;  oRing - Interior
									If False And Not mdicTmPolygons.TryGetValue(iCheckPLineID, oTopoPolygon) Then
										oTopoPolygon = New TopoManager.tmPolygon(oRing)
										mdicTmPolygons.Add(iCheckPLineID, New TopoManager.tmPolygon(oRing))
									End If
									oTopoPolygon = mdicTmPolygons.AddRingPair(oCheckRing, oRing)
									'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! ++ +++

								End If

								''''' Neigbour 
								If iRelationType = enRelationType.Neigbour AndAlso bIsCorrect Then
									oPgonRelation.DoIntersection()
									'DMAcadExt.AcadDocument.WriteMessageLog("!@!Touch Points:" & CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " : " & CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Count=" & CStr(oPgonRelation.IntersectPointCount))
									'''''oPgonRelation.DrawIntersectPoints()
									colIntersectPoints = oPgonRelation.IntersectPoints
									If colIntersectPoints IsNot Nothing AndAlso colIntersectPoints.Count <> 0 Then
										If (iPLineID = 1) Or (oCheckRing.ID = 2) Then
											''''''''DMAcadExt.AcadDocument.WriteMessageLog("Before 1x4" & CStr(moNodes.Count))
											''''''''''''''''''''''''''''' oPgonRelation.PrintIntersectPoints(iPLineID, iCheckPLineID)
										End If
										If False Then
											oaNodes = moNodes.AddNodes(iPLineID, iCheckPLineID, colIntersectPoints)
											If (iPLineID = -1) And (oCheckRing.ID = -2) Then
												For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
													oaNodes(iIndex).DebugWrite()
												Next
											End If
											Dim s1 As String = "Pseudo - NO"
											oaPseudoVertices = oRing.AddNodes(oaNodes)
											If oaPseudoVertices IsNot Nothing Then
												s1 = CStr(oaPseudoVertices.GetUpperBound(0))
												If oaPseudoVertices.GetUpperBound(0) >= 0 Then
													s1 &= "@@" & oaPseudoVertices(0).Point3d.ToString()
												End If
											End If
											If (iPLineID = 1) Or (oCheckRing.ID = 2) Then
												'	DMAcadExt.AcadDocument.WriteMessage(oRing.ID & "; H=" & oRing.Handle & "; Ps-" & s1)
											End If
											oCheckRing.ConnectNodes(oaNodes, "")
											If oaPseudoVertices IsNot Nothing AndAlso oaPseudoVertices.GetUpperBound(0) >= 0 Then
												oaAddNodes = moNodes.AddNodes(iPLineID, iCheckPLineID, oaPseudoVertices)
												oCheckRing.ConnectNodes(oaAddNodes, "")
											End If
											If oRing.ID = -2 AndAlso oCheckRing.ID = -3 Then
												DMAcadExt.AcadDocument.WriteMessageLog("After  1x4" & CStr(moNodes.Count))
											End If
										End If
										'	oRing.TmpDisplayVertices()
										'	oCheckRing.TmpDisplayVertices()
									End If
								End If
								''''''''''	colPoints.Clear()
								'''''''''''''''	oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
							Else
								oCheckDataRowView.Item(msStatusFieldName) = True
								oCheckPolyline.XData = oXDataBasePgon.GetNullResBuffer()
								DMAcadExt.AcadDocument.WriteMessageLog(CStr(iCheckPLineID) & "/" & oCheckPolyline.Handle.ToString() & " Err#22 Polygon is incorrect")
							End If
						Next iCheckIndex  'i  in oLinkDataView End of INNER Integer loop
					End If


32:            '@@@@@@@@@'iCheckIndex
					If oRing.IsExterior = TriState.UseDefault Then
						oTopoPolygon = mdicTmPolygons.AddIfNotExists(oRing)
						'System.Windows.Forms.MessageBox.Show("RingID " & oRing.ID.ToString() & vbCrLf & CStr(mdicTopoPolygons.Count), "15_633")
					Else
						'System.Windows.Forms.MessageBox.Show("RingID = " & oRing.ID.ToString() & vbCrLf & oRing.IsExterior.ToString() & vbCrLf & CStr(mdicTopoPolygons.Count), "15_698")
					End If

					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
					If oResMPolygon IsNot Nothing Then
						oResMPolygon.BalanceTree()
					End If

					'	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
					'	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
					If miSource = enPolygonSetSource.Shapes Then
						tGroupKey = New GroupKey(iGroupID, iPLineID)
						If oResMPolygon IsNot Nothing AndAlso mdicCentroids.TryGetValue(tGroupKey, tCentroidAcObjID) Then
							oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tCentroidAcObjID, OpenMode.ForWrite)
							If oBlockRef IsNot Nothing Then
								tPoint = oBlockRef.Position
								If oResMPolygon.NumMPolygonLoops > 1 Then
									'System.Windows.Forms.MessageBox.Show(CStr(iGroupID) & "/" & CStr(iPLineID) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y), "03_165M")
								End If
								iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, mdToler)
								If iaLoopInd IsNot Nothing AndAlso (iaLoopInd.Count = 1) Then
									Dim iLoopNo As Integer = iaLoopInd.Item(0)
									If oResMPolygon.NumMPolygonLoops > 1 Then
										DMAcadExt.AcadDocument.WriteLog("1841:" & CStr(oResMPolygon.NumMPolygonLoops) & "$$$ " & tPoint.ToString())
										'System.Windows.Forms.MessageBox.Show(CStr(iPLineID) & ":" & CStr(iLoopNo) & "," & oResMPolygon.GetLoopDirection(iLoopNo).ToString(), "03_168R")
									End If
									If oResMPolygon.GetLoopDirection(iLoopNo) <> LoopDirection.Exterior Then
										oFindCenter = New FindCenter(oResMPolygon)
										oPgonCenter = oFindCenter.GetInnerPoint()
										If oPgonCenter IsNot Nothing Then
											DMAcadExt.AcadTransaction.MoveBlockRef(oBlockRef, oPgonCenter.GetPoint3d())
										End If
										DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " CentroidNotFound In pgon#" & CStr(iPLineID))
									End If
								Else
									'Err
								End If
							End If
						Else
							'ERRR
							DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " CentroidNotFound In Dict #" & CStr(iPLineID))
							oBlockRef = Nothing
						End If
						oXDataParcel = New DMAcadExt.TplnXDataParcel(oPolyline.XData)
					Else ' miSource = enPolygonSetSource.Polylines  

						If oResMPolygon.NumMPolygonLoops > 1 Then
							DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " has " & CStr(oResMPolygon.NumMPolygonLoops - 1) & " island(s)")
						End If
						colCentroidsObjID = zzGetCentroidsInMPgon(oResMPolygon, oPolyline.Handle.ToString())
						If oTopoPolygon IsNot Nothing Then
							oTopoPolygon.Centroids = colCentroidsObjID
						End If


						'  TplnPolygonSet.vb:line 3010 TplnPolygonSet.vb:line 3047
						oXDataBasePgon.ID = iPLineID
						oXDataBasePgon.DataID = iPLineID
						'	DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " oXDataParcel.ID = " & CStr(oXDataParcel.ID) & "  " & oParcel.CentroidX & "," & oParcel.CentroidY & vbCrLf)
						oXDataBasePgon.MapTheme = miMapTheme
						''''''''''''''	DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)
						oLinkDataRow.Item(msCentroidCountFieldName) = colCentroidsObjID.Count
						If colCentroidsObjID.Count = 1 Then
							tCentroidAcObjID = colCentroidsObjID.Item(0)
							oLinkDataRow.Item(msCentroidAcObjIDFieldName) = tCentroidAcObjID

							If moCentroidAcadBlok Is Nothing Then
								moCentroidAcadBlok = New DMAcadExt.AcadBlock()
								moCentroidAcadBlok.Open(tCentroidAcObjID)
							End If

							tBlockRefData = moCentroidAcadBlok.GetBlockRefData(tCentroidAcObjID)
							'oEditor.WriteMessage(CStr(iRowIndex) & " CentroidID=" & colCentroidsObjID.Item(0).ToString() & vbCrLf)




							If miMapTheme = DMAcadExt.enMapTheme.Blocks OrElse miMapTheme = DMAcadExt.enMapTheme.UD_Blocks Then

								Dim oBlock As TopoManager.TPlanGraph.TplnBlock
								oBlock = New TopoManager.TPlanGraph.TplnBlock(oResMPolygon, tCentroidAcObjID)
								oXDataBlock.Block = oBlock.BlockNo
								oXDataBlock.BlockAdd = oBlock.BlockAddNo
								' DMAcadExt.AcadDocument.WriteMessage("!!## " & oBlock.BlockNo.ToString())
								oLinkDataRow.Item(msGroupIDFieldName) = oBlock.BlockNo
								If oBlock.BlockAddNo <> 0 Then
									oLinkDataRow.Item(msGroupAddIDFieldName) = oBlock.BlockAddNo
									mbHasBlockAdd = True
								End If
								' oLinkDataRow.Item(msOrderFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("PARCEL_NUM"))
								'   oLinkDataRow.Item(msNameFieldName) = oParcel.Name
								oLinkDataRow.Item(msOrderFieldName) = oBlock.Order
								oLinkDataRow.Item(msLegalAreaNameFieldName) = oBlock.LegalArea
							ElseIf miMapTheme = DMAcadExt.enMapTheme.Parcels Then
								Dim oParcel As TopoManager.TPlanGraph.TplnParcel
								oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, iPLineID, tCentroidAcObjID)
								oXDataParcel.Block = oParcel.BlockNo
								oXDataParcel.BlockAdd = oParcel.BlockAdd
								If oParcel.Name IsNot Nothing Then
									oXDataBasePgon.Name = oParcel.Name
								End If
								oXDataParcel.LegalArea = oParcel.LegalArea(False)
								oXDataParcel.Status = 1 ' tParcelData.Status
								oLinkDataRow.Item(msGroupIDFieldName) = oParcel.BlockNo
								If oParcel.BlockAdd <> 0 Then
									oLinkDataRow.Item(msGroupAddIDFieldName) = oParcel.BlockAdd
									mbHasBlockAdd = True
								End If
								' oLinkDataRow.Item(msOrderFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("PARCEL_NUM"))
								oLinkDataRow.Item(msNameFieldName) = oParcel.Name
								oLinkDataRow.Item(msOrderFieldName) = oParcel.Order
								oLinkDataRow.Item(msLegalAreaNameFieldName) = oParcel.LegalArea(False)
								'  oLinkDataRow.Item(msParentNoFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("LOT_NUM"))
								'  oLinkDataRow.Item(msParentNoAddFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("GUSH_SUFFI"))
								'  oLinkDataRow.Item(msLegalAreaNameFieldName) = zzStringToDouble(tBlockRefData.GetAttribValue("LEGAL_AREA"))
							ElseIf miMapTheme = DMAcadExt.enMapTheme.LotApproved OrElse miMapTheme = DMAcadExt.enMapTheme.LotProposed Then
								Dim oLot As TopoManager.TPlanGraph.TplnLot


								oLot = New TopoManager.TPlanGraph.TplnLot(miTopoPurpose, oResMPolygon, iPLineID, tCentroidAcObjID)
								oLot.ExteriorHandle = oPolyline.Handle
								''''''''''''''''''''''''''''	oLot.Lines = colCentroidsObjID
								oLot.Lines = mcolPolylineIDs

								If oLot.Name IsNot Nothing Then

									oXDataBasePgon.Name = oLot.Name
								End If
2222222222222:
								'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
								Try

2222222222223:
									''''''''''''''''''''''''
									mdicTopoPolygons.Add(oLot.TopoID, oLot)
								Catch oEx As Exception
									System.Windows.Forms.MessageBox.Show(oLot.TopoID.ToString() & vbCrLf & CStr(iPLineID), "03_134$")
								End Try

								oLinkDataRow.Item(msNameFieldName) = oLot.Name
								oLinkDataRow.Item(msAttribInt1FieldName) = oLot.LanduseID
								oLinkDataRow.Item(msOrderFieldName) = oLot.Order
								'  oLinkDataRow.Item(msParentNoFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("LOT_NUM"))

								'  oLinkDataRow.Item(msParentNoAddFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("GUSH_SUFFI"))

								oLinkDataRow.Item(msLegalAreaNameFieldName) = zzStringToDouble(tBlockRefData.GetAttribValue("AREA"))
							ElseIf miMapTheme = DMAcadExt.enMapTheme.UD_Parcels Then

								Dim oParcel As UnidivNet.UD_Parcel


								oParcel = New UnidivNet.UD_Parcel(oResMPolygon, tCentroidAcObjID)
								oParcel.Calc()
								' System.Windows.Forms.MessageBox.Show(CStr(oParcel.LegalArea), "08_279")
								oXDataParcel.Block = oParcel.BlockNo
								oXDataParcel.BlockAdd = oParcel.BlockAdd
								If oParcel.Name IsNot Nothing Then
									oXDataBasePgon.Name = oParcel.Name
								Else
									DMAcadExt.AcadDocument.WriteMessage("!!!! oParcel.Name Is  Nothing " & tCentroidAcObjID.ToString())
								End If

								oXDataParcel.LegalArea = oParcel.LegalArea
								oXDataParcel.Status = 1 ' tParcelData.Status
								DMAcadExt.AcadDocument.WriteMessage("!!!! " & tCentroidAcObjID.ToString() & ", " & oParcel.BlockNo.ToString() & ", " & oParcel.BlockAdd.ToString() & ", " & oParcel.LegalArea.ToString())
								oLinkDataRow.Item(msGroupIDFieldName) = oParcel.BlockNo
								If oParcel.BlockAdd <> 0 Then
									oLinkDataRow.Item(msGroupAddIDFieldName) = oParcel.BlockAdd
									mbHasBlockAdd = True
								End If

								' oLinkDataRow.Item(msOrderFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("PARCEL_NUM"))
								oLinkDataRow.Item(msNameFieldName) = oParcel.Name
								oLinkDataRow.Item(msOrderFieldName) = oParcel.Order
								oLinkDataRow.Item(msLegalAreaNameFieldName) = oParcel.LegalArea
							ElseIf miMapTheme = DMAcadExt.enMapTheme.UD_Blocks Then
								Dim oBlock As TopoManager.TPlanGraph.TplnBlock
								oTestDB = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
								If oTestDB.GetRXClass().Name <> DMAcadExt.AcadConst.AcadBlockRefName Then
									System.Windows.Forms.MessageBox.Show(oTestDB.GetRXClass().Name)
								End If
								oBlock = New TopoManager.TPlanGraph.TplnBlock(oResMPolygon, tCentroidAcObjID)
								oLinkDataRow.Item(msGroupIDFieldName) = oBlock.BlockNo
								If oBlock.BlockAddNo <> 0 Then
									oLinkDataRow.Item(msGroupAddIDFieldName) = oBlock.BlockAddNo
									mbHasBlockAdd = True
								End If

								' oLinkDataRow.Item(msOrderFieldName) = zzStringToInt(tBlockRefData.GetAttribValue("PARCEL_NUM"))
								'   oLinkDataRow.Item(msNameFieldName) = oParcel.Name
								oLinkDataRow.Item(msOrderFieldName) = oBlock.Order
								oLinkDataRow.Item(msLegalAreaNameFieldName) = oBlock.LegalArea
							End If



						ElseIf colCentroidsObjID.Count = 0 Then


							Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
							DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " Point was not founded!" & oTPlnBoundingBox.Coordinates)
						Else

							Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
							DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & ":" & CStr(colCentroidsObjID.Count) & " points were founded" & oTPlnBoundingBox.Coordinates)
						End If
					End If  'miSource = enPolygonSetSource.Shapes

					If oXDataBasePgon IsNot Nothing Then
						oXDataBasePgon.AcadArea = Math.Abs(oResMPolygon.Area)
						oXDataBasePgon.Perimeter = oResMPolygon.Perimeter

						If Not IsDBNull(oLinkDataRow.Item(msNeigborListFieldName)) Then
							oXDataBasePgon.NeigborList = DirectCast(oLinkDataRow.Item(msNeigborListFieldName), String)
						Else
							oXDataBasePgon.NeigborList = ""
						End If

						If Not IsDBNull(oLinkDataRow.Item(msExteriorFieldName)) Then
							'System.Windows.Forms.MessageBox.Show(miMapTheme.ToString() & vbCrLf & CStr(oXDataBasePgon IsNot Nothing), "03_134$")
							'DMAcadExt.AcadDocument.WriteMessage(CStr(oXDataParcel.ImpID) & " Has Exterior: " & CStr(DirectCast(oLinkDataRow.Item(msExteriorFieldName), Integer)))
							oXDataBasePgon.Exterior = DirectCast(oLinkDataRow.Item(msExteriorFieldName), Integer)
						Else
							oXDataBasePgon.Exterior = 0
						End If
						If Not IsDBNull(oLinkDataRow.Item(msInteriorListFieldName)) Then
							'DMAcadExt.AcadDocument.WriteMessage(CStr(oXDataParcel.ImpID) & "|" & CStr(iPLineID) & " Has InteriorList: " & DirectCast(oLinkDataRow.Item(msInteriorListFieldName), String))
							oXDataBasePgon.InteriorList = DirectCast(oLinkDataRow.Item(msInteriorListFieldName), String)

						Else
							oXDataBasePgon.InteriorList = String.Empty
							oXDataBasePgon.InteriorCount = 0
						End If
						'TplnPolygonSet.vb:line 3087
						'		oResMPolygon.XData = oXDataParcel.GetResBuffer()
						oPolyline.XData = oXDataBasePgon.GetResBuffer()
						If oBlockRef IsNot Nothing Then
							'    TplnPolygonSet.vb() : Line 3087
						End If
						If Not tCentroidAcObjID.IsNull Then
							oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
							oCentroidDBObject.XData = oXDataBasePgon.GetResBuffer()
						Else
							DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " ??LoopInd.Count=" & oCentroidDataView.Count)
						End If
					Else
						DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & " XData Is Not Exist")
					End If 'oXDataBasePgon IsNot Nothing
				Else 'Not oResMPolygon.IsBalanced
					oLinkDataRow.Item(IsProperFieldName) = False
					oPolyline.XData = oXDataBasePgon.GetNullResBuffer()
					DMAcadExt.AcadDocument.WriteMessageLog(CStr(iPLineID) & "/" & oPolyline.Handle.ToString() & " Loops=" & oResMPolygon.NumMPolygonLoops & " Err#21 Polygon is incorrect")
				End If
			Else
				System.Windows.Forms.MessageBox.Show("DESIGN ERROR " & miMapTheme.ToString(), "15_450")
			End If

			If iPLineID > 100000 Then
				Exit For
			End If
			'	oRing.Vertices.DrawBreakPoints()
			'Next 'mcolPolylineIDs
		Next

2:    '@@@@@@@@@'oLinkDataRow
      '''''''''''''''	moNodes.DebugWriteAll()
      '	System.Windows.Forms.MessageBox.Show("LastID " & iPLineID.ToString() & vbCrLf & CStr(oLinkDataView.Count) & vbCrLf & CStr(mdicTopoPolygons.Count), "15_670")
      For Each oPgon As TopoManager.tmPolygon In mdicTmPolygons.Values
         If oPgon.IslandCount <> 0 Then
            'System.Windows.Forms.MessageBox.Show("Island   " & CStr(oPgon.IslandCount) & vbCrLf & oPgon.ID.ToString(), "15_466")
         End If

      Next

      DMAcadExt.AcadDocument.WriteMessage("Source = " & miSource.ToString())


   End Sub

   Public Sub CreateTopoLinks()
      Dim bHasBoundary As Boolean = moWorkAreaBound IsNot Nothing
      mdicTmPolygons.CreateTopoLinks(bHasBoundary)
   End Sub
   Public Sub DrawEdges()
      If mdicTmPolygons IsNot Nothing Then
         mdicTmPolygons.DrawEdges()
      Else
         System.Windows.Forms.MessageBox.Show("mdicTopoPolygons Is Nothing", "03_401")
      End If

   End Sub
   Public Sub PrintLinkTable()
      Dim oDataRowView As DataRowView

      Dim iGroupID As Integer
      DMAcadExt.AcadDocument.OpenLog(True)
      Dim oLinkViewByGroup As DataView = New DataView(moLinkTable, String.Empty, msGroupIDFieldName, DataViewRowState.CurrentRows)
      For iRowIndex As Integer = 0 To oLinkViewByGroup.Count - 1
         oDataRowView = oLinkViewByGroup.Item(iRowIndex)
         iGroupID = DMCommon.Functions.CIntN(oDataRowView.Item(msGroupIDFieldName))
         DMAcadExt.AcadDocument.WriteMessageLog("-" & CStr(iRowIndex) & ":" & CStr(iGroupID) & "E= " & CStr(DMCommon.Functions.CIntN(oDataRowView.Item(EntityNoFieldName)) & "N= " & DMCommon.Functions.CStrN(oDataRowView.Item(msNeigborListFieldName))))
      Next
      DMAcadExt.AcadDocument.CloseLog()




   End Sub
   Public Function ExportByGroup() As FDO_Manager.ShapeFOData()
      Dim colDisPlines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim oLinkViewByGroup As DataView = New DataView(moLinkTable, String.Empty, msGroupIDFieldName, DataViewRowState.CurrentRows)
      Dim colLinkObjIDs As ObjectIdCollection = New ObjectIdCollection()
      Dim oDataRowView As DataRowView
      Dim oDataRow As DataRow
      Dim oMPgon As MPolygon
      Dim iGroupID As Integer
      Dim iCurrentGroupID As Integer = -1
      Dim tAcObjID As ObjectId
      Dim sSHPFileName As String
      Dim sFeatureClass As String
      Dim oFDO_Manager As FDO.FDO_Manager = New FDO_Manager()
      Dim taShapeFileNames() As FDO_Manager.ShapeFOData
      Dim iShapeFileNameUB = -1
      Dim oFileInfo As IO.FileInfo
      Dim oPolyline As Polyline
      Dim dSumArea As Double = 0.0
      Dim colMPgonIDs As ObjectIdCollection = New ObjectIdCollection()

      For iRowIndex As Integer = 0 To oLinkViewByGroup.Count - 1
         oDataRowView = oLinkViewByGroup.Item(iRowIndex)
         iGroupID = DMCommon.Functions.CIntN(oDataRowView.Item(msGroupIDFieldName))
         If iCurrentGroupID = -1 Then
            iCurrentGroupID = iGroupID
         End If

         If iGroupID <> iCurrentGroupID Then
            sFeatureClass = "p" & Convert.ToString(iCurrentGroupID)
            sSHPFileName = DMCommon.Functions.GetLocalFileNameInDir(sFeatureClass, ".shp")
            'System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & sFeatureClass & vbCrLf & CStr(iCurrentGroupID), "01_155a")
            zzExportGroup(colLinkObjIDs, sSHPFileName)
            iShapeFileNameUB += 1
            ReDim Preserve taShapeFileNames(iShapeFileNameUB)

            oFileInfo = New IO.FileInfo(sSHPFileName)
            taShapeFileNames(iShapeFileNameUB) = New FDO_Manager.ShapeFOData(oFileInfo.Directory.FullName, sFeatureClass)
            taShapeFileNames(iShapeFileNameUB).Gush = New GushData(0, iCurrentGroupID, 0, 0, 0, 0.0)
            taShapeFileNames(iShapeFileNameUB).SumMPgonArea = dSumArea
            colLinkObjIDs.Clear()
            dSumArea = 0
            iCurrentGroupID = iGroupID
         End If
         tAcObjID = zzToAcObjID(oDataRowView.Item(AcObjIDFieldName))
         oDataRow = oDataRowView.Row
         oMPgon = zzGetMPolygon(oDataRow)
         oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForRead)
         If oMPgon.NumMPolygonLoops > 0 Then
            oMPgon.AppendLoopFromBoundary(oPolyline, mbExcludeCrossing, mdToler)
            tAcObjID = DMAcadExt.AcadTransaction.AppendEntity(oMPgon)
            oMPgon.BalanceTree()
            dSumArea += Math.Abs(oMPgon.Area)
            colMPgonIDs.Add(tAcObjID)
            '	System.Windows.Forms.MessageBox.Show(CStr(oMPgon.Area) & vbCrLf & CStr(oPolyline.Area), "01_714")
         Else
            dSumArea += oPolyline.Area
         End If
         'DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & oPolyline.Area.ToString() & " Mpoly=" & Math.Abs(oMPgon.Area).ToString())
         colLinkObjIDs.Add(tAcObjID)
      Next
      sFeatureClass = "p" & Convert.ToString(iCurrentGroupID)
      sSHPFileName = DMCommon.Functions.GetLocalFileNameInDir(sFeatureClass, ".shp")
      'System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & sFeatureClass & vbCrLf & CStr(iCurrentGroupID), "01_156b")
      zzExportGroup(colLinkObjIDs, sSHPFileName)
      iShapeFileNameUB += 1
      ReDim Preserve taShapeFileNames(iShapeFileNameUB)
      oFileInfo = New IO.FileInfo(sSHPFileName)
      'System.Windows.Forms.MessageBox.Show(oFileInfo.Directory.FullName & vbCrLf & oFileInfo.Name, "01_158b")
      taShapeFileNames(iShapeFileNameUB) = New FDO_Manager.ShapeFOData(oFileInfo.Directory.FullName, sFeatureClass)
      taShapeFileNames(iShapeFileNameUB).Gush = New GushData(0, iCurrentGroupID, 0, 0, 0, 0)
      taShapeFileNames(iShapeFileNameUB).SumMPgonArea = dSumArea

      DMAcadExt.AcadTransaction.EraseDBObjects(colMPgonIDs)

      '	saShapeFileNames(iShapeFileNameUB) = sShapeFileName
      '''''''''''''''''''''''	oFDO_Manager.CreateBoundingBoxesMapLayer(saShapeFileNames)
      Return taShapeFileNames
   End Function
   Private Function zzGetMPolygon(oDataRow As DataRow) As MPolygon
      Dim saAcObjID() As String
      Dim tIslandAcObjID As ObjectId
      Dim oIslandPLine As Polyline
      Dim oMPolygon As MPolygon = New MPolygon()
      If Not IsDBNull(oDataRow.Item(msInteriorLinesObjIDFieldName)) Then
         saAcObjID = Strings.Split(DirectCast(oDataRow.Item(msInteriorLinesObjIDFieldName), String), ",")
         For iIndex As Integer = 0 To saAcObjID.GetUpperBound(0)
            tIslandAcObjID = zzStrToAcObjID(saAcObjID(iIndex))
            oIslandPLine = DMAcadExt.AcadTransaction.GetPolyline(tIslandAcObjID, OpenMode.ForRead)
            oMPolygon.AppendLoopFromBoundary(oIslandPLine, mbExcludeCrossing, mdToler)
         Next
      End If
      Return oMPolygon
   End Function
   Private Sub zzExportGroup(colLinkObjIDs As ObjectIdCollection, sShapeFileName As String)
      Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Export, sShapeFileName)
      oShapeExpImp.FromPoligons(colLinkObjIDs)

      Try
         oShapeExpImp.Exec()
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			'Dim o  As System.Object = oMapImpExpEx.GetObjectData(
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sShapeFileName, "01_941d")
      End Try
   End Sub
   Public Sub CalculateNew()
      Const dToler As Double = 0.001
      Dim bExcludeCrossing As Boolean = True
      '	Dim iTest As Integer = 0
      Dim tExtents3d As Extents3d
      Dim oLinkDataView As DataView = New DataView(moLinkTable)
      Dim oCentroidDataView As DataView = New DataView(moCentroidTable)
      Dim sRowFilter As String
      Dim oDataRow As DataRow
      Dim oRelationDataRow As DataRow
      Dim oDataViewRow As DataRowView
      Dim oBaseMPolygon As MPolygon
      Dim oResMPolygon As MPolygon
      Dim iRowIndex As Integer = 0
      Dim iPlineNo As Integer = 0

      '	Dim oCheckMPolygon As MPolygon
      Dim oCheckPolyline As Polyline
      Dim tCheckAcObjID As ObjectId
      Dim iCheckNo As Integer

      Dim iNumMPolygonLoops As Integer
      Dim iDir0, iDir1 As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      Dim iParent As Integer
      Dim oMPgonLoop0 As MPolygonLoop
      Dim oMPgonLoop1 As MPolygonLoop
      Dim bBalanceSuccess As Boolean
      Dim bCrossed0 As Boolean
      Dim bCrossed1 As Boolean
      Dim bLoopCrossing0 As Boolean
      Dim bLoopCrossing1 As Boolean
      Dim dArea0, dAreaEnd As Double
      Dim dPLineArea0, dPLineArea1 As Double
      Dim dPLineLen0 As Double
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection
      Dim oParcel As TopoManager.TPlanGraph.TplnParcel
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim tAcObjIDKey(0) As Int64
      Dim bMpgon As Boolean
      Dim lAcObjID As Long
      Dim tIntPtr As System.IntPtr
      Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim iRelationType As enRelationType
      Dim oPolyline As Polyline
      For Each tAcObjID As ObjectId In mcolPolylineIDs
         oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForWrite)
         oDataRow = moLinkTable.Rows.Item(iRowIndex)
         iPlineNo = DirectCast(oDataRow.Item(EntityNoFieldName), Integer)
         oDataRow.Item(msStatusFieldName) = True
         tExtents3d = oPolyline.GeometricExtents
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XmaxFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XminFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YmaxFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YminFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         oLinkDataView.RowFilter = sRowFilter
         ''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged

         oResMPolygon = New MPolygon()

         Try
            oResMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
         Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

            DMAcadExt.AcadDocument.WriteMessage(CStr(iPlineNo) & ":" & oPolyline.Handle.ToString() & " Err #01=" & oAcadEx.ErrorStatus.ToString())

         End Try



         If iRowIndex < 10000 Then
            For iIndex As Integer = 0 To oLinkDataView.Count - 1
               bMpgon = False
               bBalanceSuccess = False
               iRelationType = enRelationType.NotExists
               dPLineArea0 = oPolyline.Area
               dPLineLen0 = oPolyline.Length
               oDataViewRow = oLinkDataView.Item(iIndex)
               ''''''''''''	tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId)
               lAcObjID = DirectCast(oDataViewRow.Item(AcObjIDFieldName), Long)
               tCheckAcObjID = New ObjectId(New System.IntPtr(lAcObjID))


               iCheckNo = DirectCast(oDataViewRow.Item(EntityNoFieldName), Integer)
               oCheckPolyline = DMAcadExt.AcadTransaction.GetPolyline(tCheckAcObjID, OpenMode.ForWrite)

               dPLineArea1 = oCheckPolyline.Area
               dPLineLen0 = oCheckPolyline.Length

               oBaseMPolygon = New MPolygon()

               Try
                  oBaseMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
                  oBaseMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                  dArea0 = oBaseMPolygon.Area

                  oBaseMPolygon.BalanceTree()
                  bBalanceSuccess = True
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  bBalanceSuccess = False
                  DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " BP Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
                  DMAcadExt.AcadDocument.WriteMessage(oPolyline.Handle.ToString() & ":" & oCheckPolyline.Handle.ToString() & vbCrLf)
               End Try
               If bBalanceSuccess AndAlso oBaseMPolygon.IsBalanced Then
                  dAreaEnd = oBaseMPolygon.Area
                  iNumMPolygonLoops = oBaseMPolygon.NumMPolygonLoops()
                  If iNumMPolygonLoops = 2 Then

                     oMPgonLoop0 = oBaseMPolygon.GetMPolygonLoopAt(0)
                     bCrossed0 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop0, dToler)
                     If bCrossed0 Then
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                     End If

                     oMPgonLoop1 = oBaseMPolygon.GetMPolygonLoopAt(1)
                     bCrossed1 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop1, dToler)
                     If bCrossed0 Then
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                     End If
                     If bCrossed0 OrElse bCrossed1 Then
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " Crossed = " & bCrossed0.ToString() & " : " & bCrossed1.ToString() & " : " & oPolyline.Handle.ToString() & " : " & oCheckPolyline.Handle.ToString() & vbCrLf)
                     End If
                     bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
                     bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)


                     iDir0 = oBaseMPolygon.GetLoopDirection(0)
                     iDir1 = oBaseMPolygon.GetLoopDirection(1)


                     If iDir0 = LoopDirection.Interior Then
                        iRelationType = enRelationType.Interior
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        iParent = oBaseMPolygon.GetParentLoop(0)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " A = " & iDir0.ToString() & " : " & FormatNumber(oCheckPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(1), Drawing.Color.Cyan)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir1 = LoopDirection.Interior Then
                        iRelationType = enRelationType.Exterior
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Magenta)
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        iParent = oBaseMPolygon.GetParentLoop(1)
                        zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(0), Drawing.Color.DeepPink)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " B = " & iDir1.ToString() & " : " & FormatNumber(oPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir0 = LoopDirection.Interior OrElse iDir1 = LoopDirection.Interior Then
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " MP: " & FormatNumber(dAreaEnd, 3) & vbCrLf)
                        DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & " Perim = " & FormatNumber(oBaseMPolygon.Perimeter, 3) & ":" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)) & vbCrLf) ' & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1
                     End If
                     '	DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & "//" & CStr(iCheckNo) & "__" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)))
                     If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                        iRelationType = enRelationType.Neigbour
                     End If
                     If iRelationType = enRelationType.Neigbour Then
                        zzAddRelation(iCheckNo, ObjectId.Null, iRelationType, False, oDataRow)
                        tIntPtr = oCheckPolyline.ObjectId.OldIdPtr
                        lAcObjID = tIntPtr.ToInt64()
                        tAcObjIDKey(0) = lAcObjID
                        oRelationDataRow = moLinkTable.Rows.Find(lAcObjID)

                        If oRelationDataRow IsNot Nothing Then
                           zzAddRelation(iPlineNo, ObjectId.Null, iRelationType, True, oRelationDataRow)
                        End If
                     End If

                     If False Then
                        colPoints.Clear()
                        oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
                        If colPoints IsNot Nothing Then
                           Dim oPoint As DMAcadExt.TPlnPoint
                           DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & CStr(colPoints.Count) & vbCrLf)
                           For iPointIndex As Integer = 0 To colPoints.Count - 1
                              oPoint = New DMAcadExt.TPlnPoint(colPoints.Item(iPointIndex))
                              DMAcadExt.AcadDocument.WriteMessage("P= " & oPoint.Coordinates)
                           Next
                        End If
                     End If
                  End If
               End If
            Next

         End If
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         '	System.Windows.Forms.MessageBox.Show(CStr(sRowFilter), "03_167")
         oCentroidDataView.RowFilter = sRowFilter
         '	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
         '	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
         tCheckAcObjID = ObjectId.Null
         For iIndex As Integer = 0 To oCentroidDataView.Count - 1
            oDataViewRow = oCentroidDataView.Item(iIndex)
            tPoint = New Point3d(DirectCast(oDataViewRow.Item(XFieldName), System.Double), DirectCast(oDataViewRow.Item(YFieldName), System.Double), 0.0)
            'tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId) mm
            iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, dToler)
            If iaLoopInd Is Nothing Then
            ElseIf (iaLoopInd.Count = 1) Then '(iaLoopInd.Count = 1 AndAlso iaLoopInd.Item(0) <> 0) OrElse iaLoopInd.Count > 1 Then
               '	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " LoopInd.Count=" & iaLoopInd.Count & " #" & CStr(iaLoopInd.Item(0)) & vbCrLf)

               tCheckAcObjID = zzToAcObjID(oDataViewRow.Item(AcObjIDFieldName))
               Exit For

            End If
         Next

         DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)
         If tCheckAcObjID.IsNull Then
            Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
            DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " Point was not founded" & oTPlnBoundingBox.Coordinates & vbCrLf)
         End If
         '''''''''	Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaBlockAttribIndex)
         'System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & oResMPolygon.GetType().ToString() & ":" & tCheckAcObjID.ToString(), "03_210")

         oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, 0, tCheckAcObjID)
         '	System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & tCheckAcObjID.ToString(), "03_550")
         oXDataParcel = New DMAcadExt.TplnXDataParcel()
         oXDataParcel.ID = iPlineNo
         oXDataParcel.DataID = iPlineNo

         oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels

         oXDataParcel.Block = oParcel.BlockNo
         oXDataParcel.BlockAdd = 0
         If oParcel.Name IsNot Nothing Then
            oXDataParcel.Name = oParcel.Name
         End If

         oXDataParcel.LegalArea = oParcel.LegalArea(False)
         oXDataParcel.Status = 1 ' tParcelData.Status

         oXDataParcel.AcadArea = Math.Abs(oResMPolygon.Area)
         oXDataParcel.Perimeter = oResMPolygon.Perimeter
         If Not IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
            oXDataParcel.NeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String)
         End If
         If Not IsDBNull(oDataRow.Item(msExteriorFieldName)) Then
            oXDataParcel.Exterior = DirectCast(oDataRow.Item(msExteriorFieldName), Integer)
         End If
         If Not IsDBNull(oDataRow.Item(msInteriorListFieldName)) Then
            oXDataParcel.InteriorList = DirectCast(oDataRow.Item(msInteriorListFieldName), String)
         End If

         oResMPolygon.XData = oXDataParcel.GetResBuffer()
         oPolyline.XData = oXDataParcel.GetResBuffer()

         If Not tCheckAcObjID.IsNull Then
            oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCheckAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            oCentroidDBObject.XData = oXDataParcel.GetResBuffer()
         Else
            DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " ??LoopInd.Count=" & oCentroidDataView.Count & vbCrLf)
         End If
         iRowIndex += 1

      Next
   End Sub
   Public Function FindPgonByPoint(tPoint As Point2d) As Integer
      Return 0
   End Function
   Public Sub Calculate()
      Const dToler As Double = 0.001
      Dim bExcludeCrossing As Boolean = True
      Dim iTest As Integer = 0
      Dim tExtents3d As Extents3d
      Dim oLinkDataView As DataView = New DataView(moLinkTable)
      Dim oCentroidDataView As DataView = New DataView(moCentroidTable)
      Dim sRowFilter As String
      Dim oDataRow As DataRow
      Dim oNeigborDataRow As DataRow
      Dim oDataViewRow As DataRowView
      Dim oBaseMPolygon As MPolygon
      Dim oResMPolygon As MPolygon
      Dim iRowIndex As Integer = 0
      Dim iPlineNo As Integer = 0

      '	Dim oCheckMPolygon As MPolygon
      Dim oCheckPolyline As Polyline
      Dim tCheckAcObjID As ObjectId
      Dim tCheckNo As Integer

      Dim iNumMPolygonLoops As Integer
      Dim iDir0, iDir1 As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      Dim iParent As Integer
      Dim oMPgonLoop0 As MPolygonLoop
      Dim oMPgonLoop1 As MPolygonLoop
      Dim bBalanceSuccess As Boolean
      Dim bCrossed0 As Boolean
      Dim bCrossed1 As Boolean
      Dim bLoopCrossing0 As Boolean
      Dim bLoopCrossing1 As Boolean
      Dim dArea0, dAreaEnd As Double
      Dim dPLineArea0, dPLineArea1 As Double
      Dim dPLineLen0 As Double
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection
      Dim oParcel As TopoManager.TPlanGraph.TplnParcel
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataParcel.XDataAppName)
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim tAcObjIDKey(0) As Int64
      Dim bMpgon As Boolean
      Dim lAcObjID As Long
      Dim tIntPtr As System.IntPtr
      Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      For Each oPolyline As Polyline In mdicPolylines.Values
         oDataRow = moLinkTable.Rows.Item(iRowIndex)
         iPlineNo = DirectCast(oDataRow.Item(EntityNoFieldName), Integer)
         oDataRow.Item(msStatusFieldName) = True
         tExtents3d = oPolyline.GeometricExtents
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XmaxFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XminFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YmaxFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YminFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         oLinkDataView.RowFilter = sRowFilter
         ''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged

         oResMPolygon = New MPolygon()
         '	oEditor.WriteMessage(CStr(iRowIndex) & " !" & CStr(oLinkDataView.Count) & vbCrLf)
         Try
            oResMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
         Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

            oEditor.WriteMessage(CStr(iRowIndex) & "Res1 Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
         End Try



         If iRowIndex < 1000 Then
            For iIndex As Integer = 0 To oLinkDataView.Count - 1
               bMpgon = False
               bBalanceSuccess = False
               dPLineArea0 = oPolyline.Area
               dPLineLen0 = oPolyline.Length
               oDataViewRow = oLinkDataView.Item(iIndex)
               ''''''''''''	tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId)
               lAcObjID = DirectCast(oDataViewRow.Item(AcObjIDFieldName), Long)
               tCheckAcObjID = New ObjectId(New System.IntPtr(lAcObjID))


               tCheckNo = DirectCast(oDataViewRow.Item(EntityNoFieldName), Integer)
               oCheckPolyline = mdicPolylines.Item(tCheckAcObjID)
               dPLineArea1 = oCheckPolyline.Area
               dPLineLen0 = oCheckPolyline.Length

               oBaseMPolygon = New MPolygon()

               Try
                  oBaseMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
                  oBaseMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                  dArea0 = oBaseMPolygon.Area

                  oBaseMPolygon.BalanceTree()
                  bBalanceSuccess = True
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  bBalanceSuccess = False
                  oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " BP Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
                  oEditor.WriteMessage(oPolyline.Handle.ToString() & ":" & oCheckPolyline.Handle.ToString() & vbCrLf)
               End Try
               If bBalanceSuccess AndAlso oBaseMPolygon.IsBalanced Then
                  dAreaEnd = oBaseMPolygon.Area
                  iNumMPolygonLoops = oBaseMPolygon.NumMPolygonLoops()
                  If iNumMPolygonLoops = 2 Then

                     oMPgonLoop0 = oBaseMPolygon.GetMPolygonLoopAt(0)
                     bCrossed0 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop0, dToler)
                     If bCrossed0 Then
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                     End If

                     oMPgonLoop1 = oBaseMPolygon.GetMPolygonLoopAt(1)
                     bCrossed1 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop1, dToler)
                     If bCrossed0 Then
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                     End If
                     If bCrossed0 OrElse bCrossed1 Then
                        oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " Crossed = " & bCrossed0.ToString() & " : " & bCrossed1.ToString() & " : " & oPolyline.Handle.ToString() & " : " & oCheckPolyline.Handle.ToString() & vbCrLf)
                     End If
                     bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
                     bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)


                     iDir0 = oBaseMPolygon.GetLoopDirection(0)
                     iDir1 = oBaseMPolygon.GetLoopDirection(1)


                     If iDir0 = LoopDirection.Interior Then

                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        iParent = oBaseMPolygon.GetParentLoop(0)
                        oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " A = " & iDir0.ToString() & " : " & FormatNumber(oCheckPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(1), Drawing.Color.Cyan)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir1 = LoopDirection.Interior Then
                        oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Magenta)
                        oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
                        iParent = oBaseMPolygon.GetParentLoop(1)
                        zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(0), Drawing.Color.DeepPink)
                        oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " B = " & iDir1.ToString() & " : " & FormatNumber(oPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
                           oResMPolygon.BalanceTree()
                        End If
                     End If
                     If iDir0 = LoopDirection.Interior OrElse iDir1 = LoopDirection.Interior Then
                        '	oEditor.WriteMessage(CStr(iTest) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " : " & FormatNumber(dArea0, 3) & " : " & FormatNumber(dArea1, 3) & " : " & FormatNumber(dAreaEnd, 3) & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1 & vbCrLf)
                        oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " MP: " & FormatNumber(dAreaEnd, 3) & vbCrLf)
                        oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & " Perim = " & FormatNumber(oBaseMPolygon.Perimeter, 3) & ":" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)) & vbCrLf) ' & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1
                     Else
                        If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
                           zzAddNeigbor(tCheckNo, oDataRow)
                           tIntPtr = oCheckPolyline.ObjectId.OldIdPtr
                           lAcObjID = tIntPtr.ToInt64()
                           tAcObjIDKey(0) = lAcObjID
                           oNeigborDataRow = moLinkTable.Rows.Find(lAcObjID)
                           'oEditor.WriteMessage(CStr(iRowIndex) & "**" & tAcObjIDKey(0).ToString() & CStr(oNeigborDataRow Is Nothing))
                           If oNeigborDataRow IsNot Nothing Then
                              zzAddNeigbor(iPlineNo, oNeigborDataRow)
                           End If

                        End If
                     End If
                  End If
                  '	oDataViewRow.Row.SetModified()
                  'jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj
               Else
                  oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
                  If colPoints IsNot Nothing Then
                     Dim oPoint As DMAcadExt.TPlnPoint
                     oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & CStr(colPoints.Count) & vbCrLf)
                     For iPointIndex As Integer = 0 To colPoints.Count - 1
                        oPoint = New DMAcadExt.TPlnPoint(colPoints.Item(iPointIndex))
                        oEditor.WriteMessage("P= " & oPoint.Coordinates)
                     Next
                  End If
               End If
            Next
         End If
         sRowFilter = "(" & msStatusFieldName & " = False) AND (" & XFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         '	System.Windows.Forms.MessageBox.Show(CStr(sRowFilter), "03_167")
         oCentroidDataView.RowFilter = sRowFilter
         '	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
         '	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
         tCheckAcObjID = ObjectId.Null
         For iIndex As Integer = 0 To oCentroidDataView.Count - 1
            oDataViewRow = oCentroidDataView.Item(iIndex)
            tPoint = New Point3d(DirectCast(oDataViewRow.Item(XFieldName), System.Double), DirectCast(oDataViewRow.Item(YFieldName), System.Double), 0.0)



            'tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId) mm
            iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, dToler)
            If iaLoopInd Is Nothing Then
            ElseIf (iaLoopInd.Count = 1) Then '(iaLoopInd.Count = 1 AndAlso iaLoopInd.Item(0) <> 0) OrElse iaLoopInd.Count > 1 Then
               '	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " LoopInd.Count=" & iaLoopInd.Count & " #" & CStr(iaLoopInd.Item(0)) & vbCrLf)

               lAcObjID = DirectCast(oDataViewRow.Item(AcObjIDFieldName), Long)
               tCheckAcObjID = New ObjectId(New System.IntPtr(lAcObjID))
               Exit For

            End If
         Next

         DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)
         If tCheckAcObjID.IsNull Then
            Dim oTPlnBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
            oEditor.WriteMessage(CStr(iRowIndex) & " Point was not founded" & oTPlnBoundingBox.Coordinates & vbCrLf)
         End If
         '''''''''	Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaBlockAttribIndex)
         'System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & oResMPolygon.GetType().ToString() & ":" & tCheckAcObjID.ToString(), "03_210")

         oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, 0, tCheckAcObjID)
         '	System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & tCheckAcObjID.ToString(), "03_550")
         oXDataParcel = New DMAcadExt.TplnXDataParcel()
         oXDataParcel.ID = iPlineNo
         oXDataParcel.DataID = iPlineNo

         oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels

         oXDataParcel.Block = oParcel.BlockNo
         oXDataParcel.BlockAdd = 0
         If oParcel.Name IsNot Nothing Then
            oXDataParcel.Name = oParcel.Name
         End If

         oXDataParcel.LegalArea = oParcel.LegalArea(False)
         oXDataParcel.Status = 1 ' tParcelData.Status

         oXDataParcel.AcadArea = Math.Abs(oResMPolygon.Area)
         oXDataParcel.Perimeter = oResMPolygon.Perimeter
         If Not IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
            oXDataParcel.NeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String)

            oResMPolygon.XData = oXDataParcel.GetResBuffer()
         End If
         If Not tCheckAcObjID.IsNull Then
            oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCheckAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            oCentroidDBObject.XData = oXDataParcel.GetResBuffer()
         Else
            oEditor.WriteMessage(CStr(iRowIndex) & " ??LoopInd.Count=" & oCentroidDataView.Count & vbCrLf)
         End If
         iRowIndex += 1

      Next
   End Sub
   Private Sub zzAddIntersection(oRing As TopoManager.tmRing, oLinkDataRow As DataRow, oCheckRing As TopoManager.tmRing, iPointsCount As Integer, oIntersectBox As DMAcadExt.TPlnBoundingBox)
		' Dim oVal  As System.Object
		'   Dim s As String
		'For iIndex As Integer = 0 To moLinkTable.Columns.Count - 1
		'   oVal = oLinkDataRow.Item(iIndex)
		'   If oVal Is Nothing Then
		'      s = " NOTHING "
		'   Else
		'      s = oVal.ToString()
		'   End If
		'   DMAcadExt.AcadDocument.WriteMessage("!!AddIn: " & iIndex.ToString() & " " & s)
		'Next


		Dim oNewRow As DataRow = moIntersectionsTable.NewRow()
      With oNewRow
         .Item(AcObjIDFieldName) = oLinkDataRow.Item(AcObjIDFieldName)                   'oRing.AcObjID
         .Item(EntityNoFieldName) = oLinkDataRow.Item(EntityNoFieldName)  ''''''''''''''''''''' oRing.ID

         '		.Add(msAcObjIDFieldName, GetType(System.Int64))



         '.Item(msGroupIDFieldName) = oLinkDataRow.Item(msGroupIDFieldName)
         '.Item(msGroupAddIDFieldName) = oLinkDataRow.Item(msGroupAddIDFieldName)
         '.Item(msNameFieldName) = "aaaa" 'oLinkDataRow.Item(msNameFieldName)




         .Item(CheckAcObjIDFieldName) = oCheckRing.AcObjID
         .Item(msCheckEntityNoFieldName) = oCheckRing.ID
         .Item(msIntersectCountFieldName) = iPointsCount

         .Item(XminFieldName) = oIntersectBox.MinPoint.X
         .Item(XmaxFieldName) = oIntersectBox.MaxPoint.X
         .Item(YminFieldName) = oIntersectBox.MinPoint.Y
         .Item(YmaxFieldName) = oIntersectBox.MaxPoint.Y

         '	.Item(msAcObjIDFieldName) = tAcObjID.OldIdPtr.ToInt64()
         ' .Item(msStatusFieldName) = False

      End With
      moIntersectionsTable.Rows.Add(oNewRow)
   End Sub
   Private Sub zzAddRelation(iID As Integer, tAcObjID As ObjectId, iRelationType As enRelationType, bInverse As Boolean, ByRef oDataRow As DataRow)
      Dim sList As String
      Dim sPLineObjIDList As String
      If iRelationType = enRelationType.Neigbour Then
         If IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
            sList = String.Empty
         Else
            sList = DirectCast(oDataRow.Item(msNeigborListFieldName), String) & ","
         End If
         oDataRow.Item(msNeigborListFieldName) = sList & iID.ToString()
         '	DMAcadExt.AcadDocument.WriteMessage("NB= " & CStr(oDataRow.Item(msNeigborListFieldName)))
      ElseIf ((iRelationType = enRelationType.Exterior) AndAlso Not bInverse) OrElse ((iRelationType = enRelationType.Interior) AndAlso bInverse) Then
         If IsDBNull(oDataRow.Item(msInteriorListFieldName)) Then
            sList = String.Empty
            sPLineObjIDList = String.Empty
         Else
            sList = DirectCast(oDataRow.Item(msInteriorListFieldName), String) & ","
            sPLineObjIDList = DirectCast(oDataRow.Item(msInteriorLinesObjIDFieldName), String) & ","

         End If
         '	DMAcadExt.AcadDocument.WriteMessage("RL_1 " & CStr(iID) & ":" & iRelationType.ToString() & ":" & CStr(bInverse) & "-" & oDataRow.Item(msEntityNoFieldName).ToString)
         oDataRow.Item(msInteriorListFieldName) = sList & iID.ToString()
         oDataRow.Item(msInteriorLinesObjIDFieldName) = sPLineObjIDList & tAcObjID.OldIdPtr.ToInt64().ToString()


      ElseIf ((iRelationType = enRelationType.Interior) AndAlso Not bInverse) OrElse ((iRelationType = enRelationType.Exterior) AndAlso bInverse) Then
         If Not IsDBNull(oDataRow.Item(msInteriorListFieldName)) Then
            ''''''ERROR
            DMAcadExt.AcadDocument.WriteMessage("RL_Error " & CStr(iID) & ":" & iRelationType.ToString() & ":" & CStr(bInverse))
         Else
            'DMAcadExt.AcadDocument.WriteMessage("RL_2 " & CStr(iID) & ":" & iRelationType.ToString() & ":" & CStr(bInverse))
            oDataRow.Item(msExteriorFieldName) = iID
         End If

      End If
   End Sub
   Private Sub zzAddNeigbor(iID As Integer, ByRef oDataRow As DataRow)
      Dim sNeigborList As String
      If IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
         sNeigborList = String.Empty
      Else
         sNeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String) & ","

      End If
      oDataRow.Item(msNeigborListFieldName) = sNeigborList & iID.ToString()
   End Sub
   Private Function zzGetCentroidsInMPgon(oMPolygon As MPolygon, sExteriorLineHandle As String) As ObjectIdCollection
      Dim tExtents3d As Extents3d
      Dim sRowFilter As String
      Dim oCentroidDataView As DataView = Nothing
      Dim oDataViewRow As DataRowView
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim iaLoopInd As IntegerCollection
      Dim tAcObjID As ObjectId
      Dim oBox As DMAcadExt.TPlnBoundingBox
      Dim colRes As ObjectIdCollection = New ObjectIdCollection()
      Dim iExteriorIndex As Integer = -1
      Dim sMsg As String = String.Empty

      Try
         For iIndex As Integer = 0 To oMPolygon.NumMPolygonLoops - 1
            If oMPolygon.GetLoopDirection(iIndex) = LoopDirection.Exterior Then
               iExteriorIndex = iIndex
               Exit For
            End If
         Next
         If sExteriorLineHandle = "1AFF2" Then
            '	DMAcadExt.AcadDocument.WriteMessageLog("AA+1:" & CStr(oMPolygon.NumMPolygonLoops) & "," & CStr(iExteriorIndex))
         End If

         sMsg = CStr(oMPolygon.NumMPolygonLoops) & "," & CStr(iExteriorIndex)
         tExtents3d = oMPolygon.GeometricExtents
         oBox = New DMAcadExt.TPlnBoundingBox(tExtents3d)
         '	sRowFilter = "(" & msStatusFieldName & " = False) AND (" & msXFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & msXFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & msYFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & msYFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
         sRowFilter = "(" & XFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & XFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & YFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & YFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"

         '	System.Windows.Forms.MessageBox.Show(CStr(sRowFilter), "03_167")
         oCentroidDataView = New DataView(moCentroidTable, sRowFilter, "", DataViewRowState.CurrentRows)
         'oCentroidDataView.RowFilter = sRowFilter
         'System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count) & vbCrLf & sRowFilter, "03_168")


         For iIndex As Integer = 0 To oCentroidDataView.Count - 1
            sMsg &= vbCrLf & CStr(iIndex)
            oDataViewRow = oCentroidDataView.Item(iIndex)
            tPoint = New Point3d(DirectCast(oDataViewRow.Item(XFieldName), System.Double), DirectCast(oDataViewRow.Item(YFieldName), System.Double), 0.0)
            'tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId) mm
            iaLoopInd = oMPolygon.IsPointInsideMPolygon(tPoint, mdToler)
            If iaLoopInd Is Nothing Then
               sMsg &= "," & "LoopInd Is Nothing"
            ElseIf (iaLoopInd.Count = 1) Then '(iaLoopInd.Count = 1 AndAlso iaLoopInd.Item(0) <> 0) OrElse iaLoopInd.Count > 1 Then
               '	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " LoopInd.Count=" & iaLoopInd.Count & " #" & CStr(iaLoopInd.Item(0)) & vbCrLf)
               If iaLoopInd.Item(0) = iExteriorIndex Then

                  tAcObjID = zzToAcObjID(oDataViewRow.Item(AcObjIDFieldName))
                  sMsg &= "," & tAcObjID.ToString()
                  If DirectCast(oDataViewRow.Item(msStatusFieldName), Boolean) Then
                     DMAcadExt.AcadDocument.WriteMessageLog("Double Point: " & CStr(tPoint.X) & "," & CStr(tPoint.Y) & " " & sExteriorLineHandle & "," & DMCommon.Functions.CStrN(oDataViewRow.Item(msHandleFieldName)))
                     DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", "Double Points", False)
                     ''''''''''DMAcadExt.AcadDocument.WriteMessage("2nd Box " & oBox.Coordinates & ":")
                  End If
                  oDataViewRow.Item(msStatusFieldName) = True
                  oDataViewRow.Item(msHandleFieldName) = sExteriorLineHandle

                  '	1AFF2
                  colRes.Add(tAcObjID)
               Else
                  sMsg &= ";*" & iaLoopInd.Item(0).ToString() & "; ExtIndex=" & iExteriorIndex.ToString() & "; " & CStr(tPoint.X) & "," & CStr(tPoint.Y)
               End If

            ElseIf (iaLoopInd.Count > 1) Then
               sMsg &= "," & "LoopInd = " & CStr(iaLoopInd.Count)
            End If
         Next

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage("Err#1264 = " & oEx.Message)
      End Try
      If colRes.Count <> 1 Then
         DMAcadExt.AcadDocument.WriteMessage("Points in Ext = " & CStr(colRes.Count) & vbCrLf & sMsg)
      End If
      Return colRes
   End Function
	Private Shared Function zzFDO_RingToPline(oRing As OSGeo.FDO.Geometry.ILinearRing, sPlineLayer As String) As Polyline

		If oRing.Positions IsNot Nothing AndAlso oRing.Count > 1 Then


			Dim oPline As Polyline = New Polyline(oRing.Count)
			Dim tPoint As Point2d
			Dim iVertexIndex As Integer = 0
			For Each oPosition As OSGeo.FDO.Geometry.IDirectPosition In oRing.Positions
				tPoint = New Point2d(oPosition.X, oPosition.Y)
				oPline.AddVertexAt(iVertexIndex, tPoint, 0.0, 0.0, 0.0)
			Next

			oPline.Closed = True
			oPline.Layer = sPlineLayer
			DMAcadExt.AcadTransaction.AppendEntity(oPline)

			Return oPline
		Else
			Return Nothing
		End If
	End Function
	Private Shared Sub zzFDO_Polygon(oPoligon As OSGeo.FDO.Geometry.IPolygon, tParcelData As ParcelData)
      Dim oRing As OSGeo.FDO.Geometry.ILinearRing
		Dim oPline As Polyline = Nothing
		Dim oPlineInner As Polyline

      Dim oMPolygon As MPolygon = New Autodesk.AutoCAD.DatabaseServices.MPolygon()

      Dim iVertexIndex As Integer = 0
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel = New DMAcadExt.TplnXDataParcel()
      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
      Dim sLayerName As String = "1603"
      Dim sBlockName As String = "1603"
      oMPolygon.Layer = "TplnParcelMPgon"
		oRing = oPoligon.ExteriorRing
		If oRing IsNot Nothing Then
			oPline = zzFDO_RingToPline(oRing, "1602")
			If oPline IsNot Nothing Then
				colPolylines.Add(oPline.ObjectId)
				oMPolygon.AppendLoopFromBoundary(oPline, False, 0.001)
			End If
		End If


		For iInnerRingIndex As Integer = 0 To oPoligon.InteriorRingCount - 1
			oRing = oPoligon.GetInteriorRing(iInnerRingIndex)
			If oRing IsNot Nothing Then
				oPlineInner = zzFDO_RingToPline(oRing, "1602Inner")
				If oPlineInner IsNot Nothing Then
					oMPolygon.AppendLoopFromBoundary(oPlineInner, False, 0.001)
				End If
			End If


		Next

      oXDataParcel.ID = tParcelData.PgonID
      oXDataParcel.DataID = tParcelData.PgonID

      oXDataParcel.MapTheme = DMAcadExt.enMapTheme.Parcels
      oXDataParcel.Block = tParcelData.Block
      oXDataParcel.BlockAdd = tParcelData.BlockAdd
      oXDataParcel.Name = tParcelData.ParcelName
      oXDataParcel.LegalArea = tParcelData.LegalArea
      oXDataParcel.Status = tParcelData.Status

      '  DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, ParcelData.mia1603AttribIndices, tParcelData.GetAttribText())
      '  oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

      oXDataParcel.AcadArea = Math.Abs(oMPolygon.Area)
		'  dSumMPgonArea += Math.Abs(oMPolygon.Area)
		oXDataParcel.Perimeter = oMPolygon.Perimeter
		If oPline IsNot Nothing Then
			oPline.XData = oXDataParcel.GetResBuffer()
		End If

		'  oCentroidDBObject.XData = oMPolygon.XData

		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      '  mtShapeFOData.SumMPgonArea = dSumMPgonArea
      oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
      tCentroidAcObjID = colCentroids.Item(0)

      DMAcadExt.AcadTransaction.UpdateAttribText(tCentroidAcObjID, True, ParcelData.mia1603AttribIndices, tParcelData.GetAttribText())
      oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      oCentroidDBObject.XData = oMPolygon.XData


   End Sub
     
   Private Sub zzDispMPgonLoop(oMPgonLoop As MPolygonLoop, tColor As System.Drawing.Color)


      Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

      Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle
      Dim tNormal As Autodesk.AutoCAD.Geometry.Vector3d = New Autodesk.AutoCAD.Geometry.Vector3d(0, 0, 1)
      For iIndex As Integer = 0 To oMPgonLoop.Count - 1
         tPoint = oMPgonLoop.Item(iIndex).Vertex
         tPoint3d = New Point3d(tPoint.X, tPoint.Y, 0.0)
         oCircle = New Autodesk.AutoCAD.DatabaseServices.Circle(tPoint3d, tNormal, 1.0)
         If tColor <> System.Drawing.Color.Empty Then
            oCircle.Color = Autodesk.AutoCAD.Colors.Color.FromColor(tColor)
         End If
         DMAcadExt.AcadTransaction.AppendEntity(oCircle)
      Next

   End Sub
   Private Enum enGushFields
      BlockNo
      BlockAdd
      Status
      IsAnality
   End Enum
   Private Enum enParcelFields
      ParcelID
      BlockNo
      BlockAdd
      ParcelName
      LegalArea
      Status
   End Enum
   Private Enum enLot_ComplotFields
      Layer
      Gush
      Helka
      Ykd2
      YStr
   End Enum

   Public Sub Import(sShapeFileName As String, Optional sDestLayer As String = Nothing)
      Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Import, sShapeFileName)
      oShapeExpImp.ImportPolygonsAsClosedPolylines = True

      If Not String.IsNullOrEmpty(sDestLayer) Then
         oShapeExpImp.SetDestLayer(sDestLayer)
      End If

      oShapeExpImp.Exec()


      miPgonImported = Convert.ToInt32(oShapeExpImp.PolygonCount)

      'System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & CStr(miPgonImported), "04_998")
      'oShapeExpImp.Dispose()
   End Sub
   Public Function GetMPgonColByGush() As IDictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      If moMPgonsByGush IsNot Nothing Then
         Return moMPgonsByGush.GetMPgonColByGush()
      Else

         Return Nothing
      End If

   End Function
   Private Shared Function zzGetConnection(sSHPFileName As String) As Connections.IConnectionImp
      '  Dim className As Expression.Identifier = New Expression.Identifier("AFeatureClass")
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oConnManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(oConnManager.CreateConnection(FDO_Manager.SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim iConnState As Connections.ConnectionState
      oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

      Try
         iConnState = oConnection.Open()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259con")
      End Try
      Return oConnection
   End Function
	Private Shared Function zzDestSHPFileName(sSHPFileName As String, sBlockNo As String) As String
		'  Dim className As Expression.Identifier = New Expression.Identifier("AFeatureClass")
		Const sTemplateDir As String = "M:\Dm_Work\FileSupport\SHPFiles\Parcels"
		Dim oFileInfo As IO.FileInfo
		Dim oDestFileInfo As IO.FileInfo
		Dim oDirInfo As IO.DirectoryInfo
		Dim oSubDirInfo As IO.DirectoryInfo
		Dim oTemplateSubdirInfo As IO.DirectoryInfo = New IO.DirectoryInfo(sTemplateDir)

		oFileInfo = New IO.FileInfo(sSHPFileName)
		oDirInfo = oFileInfo.Directory
		oSubDirInfo = oDirInfo.CreateSubdirectory(sBlockNo)
		oDestFileInfo = New IO.FileInfo(oSubDirInfo.FullName & "\p" & sBlockNo & ".SHP")
		If True Or Not oDestFileInfo.Exists Then
			For Each oFile As IO.FileInfo In oTemplateSubdirInfo.GetFiles()
				oFile.CopyTo(oSubDirInfo.FullName & "\p" & sBlockNo & oFile.Extension, True)
			Next
		End If
		Return oSubDirInfo.FullName & "\p" & sBlockNo & ".SHP"
	End Function
	Private Shared Function zzGetReaderSrc(sSHPFileName As String, sClassName As String, tFilterList As DMCommon.dmList) As OSGeo.FDO.Commands.Feature.IFeatureReader
		Dim sSHPPropFileName As String = "DefaultFileLocation"
		Dim oConnManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
		Dim oConnection As Connections.IConnectionImp = CType(oConnManager.CreateConnection(FDO_Manager.SHPProviderName), Connections.IConnectionImp)
		Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
		Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
		Dim iConnState As Connections.ConnectionState
		Dim sConnectionString As String = oConnection.ConnectionString
		Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		Dim oPropertyExpression As OSGeo.FDO.Expression.Expression = OSGeo.FDO.Expression.Expression.Parse("GUSH_NUM")
		Dim oValueExpression As OSGeo.FDO.Expression.Expression '= OSGeo.FDO.Expression.Expression.Parse("7703")
		Dim oFilter As OSGeo.FDO.Filter.Filter = Nothing

		Dim oComparisonCondition As OSGeo.FDO.Filter.ComparisonCondition = Nothing ' New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression)

		'    DMCommon.Debug.MsgBox("04_241", tFilterList.Exists, tFilterList.First, tFilterList.Item(0))
		If tFilterList.Exists Then
			oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.First)
			oFilter = New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression)
			For iIndex = 1 To tFilterList.UpperBound
				oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.Item(iIndex))
				oFilter = oFilter.Combine(oFilter, Filter.BinaryLogicalOperations.BinaryLogicalOperations_Or, New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression))
			Next

		End If
		oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
		Try
			iConnState = oConnection.Open()
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("04_259r", oEx.Message, sSHPPropFileName, sSHPFileName, sClassName)
			Return Nothing
		End Try
		'   DMCommon.Debug.MsgBox("04_400")
		Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)
		'  DMCommon.Debug.MsgBox("04_400A")
		Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()
		'   DMCommon.Debug.MsgBox("04_400B")
		Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
		Dim oClasses As Schema.ClassCollection = oSchema.Classes
		'  DMCommon.Debug.MsgBox("04_400C")
		Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)
		'  DMCommon.Debug.MsgBox("04_400D")
		Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)
		'   DMCommon.Debug.MsgBox("04_400E", sClassName)
		Dim oClassName As Expression.Identifier = New Expression.Identifier(sClassName)
		'   DMCommon.Debug.MsgBox("04_400F", sClassName)
		oSelect.FeatureClassName = oClassName
		'   DMCommon.Debug.MsgBox("04_400G", sClassName)
		If oFilter IsNot Nothing Then
			' DMCommon.Debug.MsgBox("04_401")
			oSelect.Filter = oFilter
		ElseIf oComparisonCondition IsNot Nothing Then
			DMCommon.Debug.MsgBox("04_402")
			oSelect.Filter = oComparisonCondition
		End If
		'   DMCommon.Debug.MsgBox("04_400H", sClassName)
		Dim oRes As OSGeo.FDO.Commands.Feature.IFeatureReader
		Try
			oRes = oSelect.Execute()
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("04_403r", oEx.Message, oEx.StackTrace)
			oRes = Nothing
		End Try
		'  DMCommon.Debug.MsgBox("04_400I", sClassName, oRes Is Nothing)
		Return oRes
	End Function
	Private Shared Function zzGetReader(sSHPFileName As String, sClassName As String, tFilterList As DMCommon.dmList) As OSGeo.FDO.Commands.Feature.IFeatureReader
      Dim sSHPPropFileName As String = "DefaultFileLocation"
      Dim oConnManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
      Dim oConnection As Connections.IConnectionImp = CType(oConnManager.CreateConnection(FDO_Manager.SHPProviderName), Connections.IConnectionImp)
      Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
      Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
      Dim iConnState As Connections.ConnectionState
      Dim sConnectionString As String = oConnection.ConnectionString
      Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		Dim oPropertyExpression As OSGeo.FDO.Expression.Expression = OSGeo.FDO.Expression.Expression.Parse("GUSH_NUM")
		Dim oPropertyAddExpression As OSGeo.FDO.Expression.Expression = OSGeo.FDO.Expression.Expression.Parse("GUSH_SUFFI")

		Dim oValueExpression As OSGeo.FDO.Expression.Expression '= OSGeo.FDO.Expression.Expression.Parse("7703")
		Dim oFilter As OSGeo.FDO.Filter.Filter = Nothing
		Dim oPartFilter As OSGeo.FDO.Filter.Filter = Nothing


		Dim oComparisonCondition As OSGeo.FDO.Filter.ComparisonCondition = Nothing ' New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression)

		'DMCommon.Debug.MsgBox("04_241", tFilterList.Exists, tFilterList.First, tFilterList.Item(0))

		'	DMCommon.Debug.MsgBox("04_241x", tFilterList.Exists)
		If tFilterList.Exists Then
         oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.First)
			oFilter = New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression)
			If Not String.IsNullOrEmpty(tFilterList.FirstExt) Then
				oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.FirstExt)
				oFilter = oFilter.Combine(oFilter, Filter.BinaryLogicalOperations.BinaryLogicalOperations_And, New OSGeo.FDO.Filter.ComparisonCondition(oPropertyAddExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression))
			End If
			For iIndex = 1 To tFilterList.UpperBound
				oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.Item(iIndex))
				oPartFilter = New OSGeo.FDO.Filter.ComparisonCondition(oPropertyExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression)



				If Not String.IsNullOrEmpty(tFilterList.ItemExt(iIndex)) Then
					'DMCommon.Debug.MsgBox("04_241c", tFilterList.Exists, tFilterList.First, tFilterList.FirstExt, tFilterList.Item(0), tFilterList.ItemExt(0), tFilterList.Item(iIndex), tFilterList.ItemExt(iIndex))
					oValueExpression = OSGeo.FDO.Expression.Expression.Parse(tFilterList.ItemExt(iIndex))
					oPartFilter = oFilter.Combine(oPartFilter, Filter.BinaryLogicalOperations.BinaryLogicalOperations_And, New OSGeo.FDO.Filter.ComparisonCondition(oPropertyAddExpression, OSGeo.FDO.Filter.ComparisonOperations.ComparisonOperations_EqualTo, oValueExpression))
				End If
				oFilter = oFilter.Combine(oFilter, Filter.BinaryLogicalOperations.BinaryLogicalOperations_Or, oPartFilter)

			Next
			'	DMCommon.Debug.MsgBox("04_241K", oFilter.ToString())
		End If
		oProperties.SetProperty(sSHPPropFileName, sSHPFileName)
		'DMCommon.Debug.MsgBox("04_241S", sSHPPropFileName, sSHPFileName)
		Try
         iConnState = oConnection.Open()
      Catch oEx As Exception
         DMCommon.Debug.MsgBox("04_259r", oEx.Message, sSHPPropFileName, sSHPFileName, sClassName)
         Return Nothing
      End Try
		'DMCommon.Debug.MsgBox("04_400")
		Dim oDescSchema As Commands.Schema.IDescribeSchema = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_DescribeSchema), Commands.Schema.IDescribeSchema)

		Dim oSchemas As Schema.FeatureSchemaCollection = oDescSchema.Execute()

		Dim oSchema As Schema.FeatureSchema = oSchemas.Item(0)
      Dim oClasses As Schema.ClassCollection = oSchema.Classes

		Dim oFeatureClass As Schema.ClassDefinition = DirectCast(oClasses.Item(0), Schema.ClassDefinition)

		Dim oSelect As Commands.Feature.ISelect = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Select), Commands.Feature.ISelect)

		Dim oClassName As Expression.Identifier = New Expression.Identifier(sClassName)

		oSelect.FeatureClassName = oClassName

		If oFilter IsNot Nothing Then
			' DMCommon.Debug.MsgBox("04_401")
			oSelect.Filter = oFilter
		ElseIf oComparisonCondition IsNot Nothing Then ''

			oSelect.Filter = oComparisonCondition
      End If
		'	DMCommon.Debug.MsgBox("04_400H", sClassName)
		Dim oRes As OSGeo.FDO.Commands.Feature.IFeatureReader

		Try
         oRes = oSelect.Execute()
      Catch oEx As Exception
         DMCommon.Debug.MsgBox("04_403r", oEx.Message, oEx.StackTrace)
         oRes = Nothing
      End Try

		Return oRes
   End Function
   Public Shared Function GetClipF_ID(sSHPFileName As String, sClassName As String) As Dictionary(Of Integer, ParcelData)
      Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList)
      If oFeatureReader IsNot Nothing Then
         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim iBlock As Integer
         Dim iBlockAdd As Integer
         Dim iParcelName As Integer
         Dim sParcelName As String
         Dim dLegalArea As Double
         Dim iStatus As Integer
         Dim sPropName As String = ""
         Dim iaPropIndices() = {-1, -1, -1, -1, -1}
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)
            sPropName = oPropDef.Name
            '	System.Windows.Forms.MessageBox.Show(oPropDef.Name, "01_420")
            Select Case sPropName
               Case "GUSH_NO", "GUSH_NUM"
                  iaPropIndices(enParcelFields.BlockNo) = iIndex
               Case "GUSH_SUFFI"
                  iaPropIndices(enParcelFields.BlockAdd) = iIndex
               Case "PARCEL"
                  iaPropIndices(enParcelFields.ParcelName) = iIndex
               Case "LEGAL_AREA"
                  iaPropIndices(enParcelFields.LegalArea) = iIndex
               Case "STATUS_ID", "STATUS"
                  If iaPropIndices(enParcelFields.Status) = -1 Then
                     iaPropIndices(enParcelFields.Status) = iIndex
                  End If
            End Select
         Next


         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enParcelFields.BlockNo) <> -1 Then
                  iBlock = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockNo))
               Else
                  iBlock = 0
               End If
               If iaPropIndices(enParcelFields.BlockAdd) <> -1 Then
                  iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockAdd))
               Else
                  iBlockAdd = 0
               End If
               If iaPropIndices(enParcelFields.ParcelName) <> -1 Then
                  iParcelName = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelName))
               Else
                  iParcelName = 0
               End If

               If iaPropIndices(enParcelFields.LegalArea) <> -1 Then
                  dLegalArea = oFeatureReader.GetDouble(iaPropIndices(enParcelFields.LegalArea))
               Else
                  dLegalArea = 0.0
               End If
               If iaPropIndices(enParcelFields.Status) <> -1 Then
                  iStatus = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.Status))
               Else
                  iStatus = 0
               End If
               sParcelName = Convert.ToString(iParcelName)
               dicRes.Add(iPgonID, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
            End Try
         End While
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")

      End If
      Return dicRes
   End Function

	Public Shared Sub InsertParcelData(sSHPFileName As String)
		Dim className As Expression.Identifier = New Expression.Identifier("FeatureClass")
		Dim sSHPPropFileName As String = "DefaultFileLocation"
		Dim oConnManager As OSGeo.FDO.IConnectionManager = OSGeo.FDO.ClientServices.FeatureAccessManager.GetConnectionManager()
		Dim oConnection As Connections.IConnectionImp = CType(oConnManager.CreateConnection(FDO_Manager.SHPProviderName), Connections.IConnectionImp)
		Dim oConnInfo As Connections.IConnectionInfo = oConnection.ConnectionInfo
		Dim oProperties As Connections.IConnectionPropertyDictionary = oConnInfo.ConnectionProperties
		Dim iConnState As Connections.ConnectionState
		oProperties.SetProperty(sSHPPropFileName, sSHPFileName)

		DMCommon.Debug.MsgBox("09_341a", sSHPPropFileName, sSHPFileName)
		Try
			iConnState = oConnection.Open()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "04_259")
		End Try
		DMCommon.Debug.MsgBox("09_341z", iConnState.ToString())

		If False Then


			Dim createDS As OSGeo.FDO.Commands.DataStore.ICreateDataStore = DirectCast(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_CreateDataStore), OSGeo.FDO.Commands.DataStore.ICreateDataStore)
			DMCommon.Debug.MsgBox("09_342", "01")
			Dim properties As OSGeo.FDO.Commands.DataStore.IDataStorePropertyDictionary = createDS.DataStoreProperties
			DMCommon.Debug.MsgBox("09_343", "02a")
			Try
				createDS.Execute()
			Catch ex As Exception

			End Try

		End If

		Dim insert As Commands.Feature.IInsert = CType(oConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
		insert.FeatureClassName = className
		Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
		' add the Int32 value to the insert command
		Dim iInt32Value As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(5)
		Dim int32PropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Int32Prop", iInt32Value)
		values.Add(int32PropVal)
		' add the feature geometry to the insert command
		Dim geomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New OSGeo.FDO.Geometry.FgfGeometryFactory()
		Dim position1 As OSGeo.FDO.Geometry.DirectPositionImpl = New OSGeo.FDO.Geometry.DirectPositionImpl(1.0, 1.0)
		Dim point As OSGeo.FDO.Geometry.IPoint = geomFactory.CreatePoint(position1)
		Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(geomFactory.GetFgf(point))
		Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("FeatGeomProp", geomVal)
		values.Add(geomPropVal)
		' insert the feature
		Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
		reader.Close()
	End Sub
	Public Shared Function InputParcelData(sSHPFileName As String, sClassName As String, bInclGeometry As Boolean, ByRef oParcelAllDataAdapter As Data.Common.DbDataAdapter) As Boolean
		' Dim dicParcelData As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		'   Dim dicGushFromParcelData As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()

		Dim oFeatureClass As Schema.ClassDefinition
		Dim oToConnection As Connections.IConnectionImp = Nothing
		Dim sToClassName As String
		Dim bRes As Boolean = True
		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList())
		If oFeatureReader IsNot Nothing Then
			Dim iGeo As Integer = 0
			Dim sText As String
			Dim iPgonID As Integer
			Dim iBlock As Integer
			Dim iBlockAdd As Integer
			Dim iParcelName As Integer

			Dim iParcelID As Integer
			Dim dLegalArea As Double
			Dim iStatus As Integer
			Dim sPropName As String = ""

			Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte()
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
			Dim oPoligon As OSGeo.FDO.Geometry.IPolygon

			Dim tPrevGushData As GushData = New GushData()
			Dim tGushData As GushData
			Dim oNewRow As DataRow
			'	DMCommon.Debug.MsgBox("04_411a!!!!!", "GetParcelData", sSHPFileName, bInclGeometria, DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData))
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
			Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
			Dim oParcelsAllDataTable As Data.DataTable = New Data.DataTable("ParcelsAll")

			iPgonID = 0
			oFeatureClass = oFeatureReader.GetClassDefinition
			'DMCommon.Debug.MsgBox("04_411b")
			Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
			' DMCommon.Debug.MsgBox("04_411c")

			'  DMCommon.Debug.MsgBox("04_411d !#!#!", bInclGeometria, tFilterList.UpperBound, DMCommon.Debug.ColCount(hsGush))
			If bInclGeometry Then

				'  DMCommon.ExcelLogG.Open()






				' DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)
			End If

			' DMCommon.Debug.MsgBox("04_410B", "GeometryIndex", iGeometryIndex)
			'Dim iA, iB, iC As Integer


			While oFeatureReader.ReadNext()


				oNewRow = oParcelsAllDataTable.NewRow
				Try
					iPgonID += 1
					'  DMCommon.ExcelLogX.SetNextValue(1, iPgonID)
					sText = CStr(iPgonID) & ":"
					oNewRow.Item("PARCEL_ID") = oFeatureReader.GetInt32("PARCEL_ID")
					oNewRow.Item("GUSH_NUM") = oFeatureReader.GetInt32("GUSH_NUM")
					oNewRow.Item("GUSH_SUFFI") = oFeatureReader.GetInt32("GUSH_SUFFI")
					oNewRow.Item("PARCEL") = oFeatureReader.GetInt32("PARCEL")

					oNewRow.Item("LEGAL_AREA") = oFeatureReader.GetDouble("LEGAL_AREA")
					oNewRow.Item("PARCEL") = oFeatureReader.GetInt16("PARCEL")
					oNewRow.Item("SHAPE_AREA") = oFeatureReader.GetDouble("SHAPE_AREA")
					oNewRow.Item("SHAPE_LEN") = oFeatureReader.GetDouble("SHAPE_LEN")

					oParcelsAllDataTable.Rows.Add(oNewRow)

					If bInclGeometry AndAlso iGeometryIndex <> -1 Then
						oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
						If oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) >= 0 Then

							oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
							If oGeometry IsNot Nothing Then


								oPoligon = FromGeometry(oGeometry)
								If oPoligon IsNot Nothing Then


									If dicGushConnections.TryGetValue(tGushData.Folder, oToConnection) Then
										sToClassName = "p" & tGushData.Folder
										Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
										Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

										insert.FeatureClassName = oToClassName
										Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
										' add the Int32 value to the insert command

										Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
										Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
										values.Add(oGushPropVal)

										Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
										Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
										values.Add(oGushAddPropVal)

										Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
										Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
										values.Add(oParcelIDPropVal)

										Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
										Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
										values.Add(oParcelPropVal)

										Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
										Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
										values.Add(oLegalAreaPropVal)

										Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
										Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
										values.Add(oStatusPropVal)

										' add the feature geometry to the insert command
										Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
										Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
										values.Add(geomPropVal)
										' insert the feature

										Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
										reader.Close()

										' DMCommon.ExcelLogG.SetNextValue(6, "+++ +++ +++", iBlock, oToConnection.ConnectionString)
									ElseIf dicGushConnections.Count <> 0 Then
										DMCommon.Debug.MsgBox("09_921", iBlock, dicGushConnections.Count, iBlock.ToString())
										bRes = False
									End If
								End If
							End If
						End If
					End If
					'       DMCommon.Debug.MsgBox("12_601", iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)

					bRes = True
					'If oGeometry IsNot Nothing Then
					'   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
					'End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & "|" & oEx.StackTrace & "|" & oEx.GetType().ToString() & ":" & vbCrLf)
					bRes = False
				End Try
				'  DMCommon.ExcelLogX.SetValue(1, iA, iB, iC, iPgonID, iBlock, sParcelName, dLegalArea)
			End While

			'  DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, bInclGeometria, dicParcelData.Count, dicGushFromParcelData.Count, iPgonID)
			'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
		Else
			System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")
			bRes = False
		End If
		Return bRes
	End Function
	Public Shared Function InputParcelDataNew(sSHPFileName As String, sClassName As String, bInclGeometry As Boolean, ByRef oParcelAllDataAdapter As Data.Common.DbDataAdapter) As Boolean
		' Dim dicParcelData As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		'   Dim dicGushFromParcelData As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()

		Dim oFeatureClass As Schema.ClassDefinition
		Dim oToConnection As Connections.IConnectionImp = Nothing
		Dim sToClassName As String
		Dim bRes As Boolean = True
		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList())
		If oFeatureReader IsNot Nothing Then
			Dim iGeo As Integer = 0
			Dim sText As String
			Dim iPgonID As Integer
			Dim iBulkRecordNo As Integer

			Dim iBlock As Integer
			Dim iBlockAdd As Integer
			Dim iParcelName As Integer

			Dim iParcelID As Integer
			Dim dLegalArea As Double
			Dim iStatus As Integer
			Dim sPropName As String = ""

			Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte()
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
			Dim oPoligon As OSGeo.FDO.Geometry.IPolygon

			Dim tPrevGushData As GushData = New GushData()
			Dim tGushData As GushData = New GushData()
			Dim oNewRow As DataRow
			'	DMCommon.Debug.MsgBox("04_411a!!!!!", "GetParcelData", sSHPFileName, bInclGeometria, DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData))
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
			Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
			Dim oParcelsAllDataTable As Data.DataTable = Nothing '= New Data.DataTable("ParcelsAll")

			iPgonID = 0
			oFeatureClass = oFeatureReader.GetClassDefinition
			'DMCommon.Debug.MsgBox("04_411b")
			Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
			' DMCommon.Debug.MsgBox("04_411c")

			'  DMCommon.Debug.MsgBox("04_411d !#!#!", bInclGeometria, tFilterList.UpperBound, DMCommon.Debug.ColCount(hsGush))
			If bInclGeometry Then

				'  DMCommon.ExcelLogG.Open()

				' DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)
			End If

			' DMCommon.Debug.MsgBox("04_410B", "GeometryIndex", iGeometryIndex)
			'	Dim iA, iB, iC As Integer

			While oFeatureReader.ReadNext()
				If iBulkRecordNo = 0 Then
					Try
						oParcelsAllDataTable = New Data.DataTable("ParcelsAll")
						oParcelAllDataAdapter.FillSchema(oParcelsAllDataTable, Data.SchemaType.Source)
						oParcelAllDataAdapter.UpdateBatchSize = 0
					Catch oEx As Exception
						DMCommon.Debug.MsgBox("AllParcels", oEx.Message)
					End Try
				End If



				oNewRow = oParcelsAllDataTable.NewRow
				Try
					iPgonID += 1
					iBulkRecordNo += 1
					'  DMCommon.ExcelLogX.SetNextValue(1, iPgonID)
					sText = CStr(iPgonID) & ":"
					oNewRow.Item("PARCEL_ID") = oFeatureReader.GetInt32("PARCEL_ID")
					oNewRow.Item("GUSH_NUM") = oFeatureReader.GetInt32("GUSH_NUM")
					oNewRow.Item("GUSH_SUFFI") = oFeatureReader.GetInt32("GUSH_SUFFI")
					oNewRow.Item("PARCEL") = oFeatureReader.GetInt32("PARCEL")
					oNewRow.Item("PARCEL") = oFeatureReader.GetInt16("PARCEL")

					oNewRow.Item("LEGAL_AREA") = oFeatureReader.GetDouble("LEGAL_AREA")

					oNewRow.Item("STATUS") = oFeatureReader.GetInt16("STATUS")
					oNewRow.Item("STATUS_TEX") = oFeatureReader.GetString("STATUS_TEX")


					oNewRow.Item("LOCALITY_I") = oFeatureReader.GetInt32("LOCALITY_I")
					oNewRow.Item("LOCALITY_N") = oFeatureReader.GetString("LOCALITY_N")



					oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")
					oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")

					oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")
					oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")

					oNewRow.Item("COUNTY_ID") = oFeatureReader.GetInt16("COUNTY_ID")
					oNewRow.Item("COUNTY_NAM") = oFeatureReader.GetString("COUNTY_NAM")

					oNewRow.Item("REGION_ID") = oFeatureReader.GetInt16("REGION_ID")
					oNewRow.Item("REGION_NAM") = oFeatureReader.GetString("REGION_NAM")

					oNewRow.Item("TALAR_NUMB") = oFeatureReader.GetInt16("TALAR_NUMB")
					oNewRow.Item("TALAR_YEAR") = oFeatureReader.GetInt16("TALAR_YEAR")

					oNewRow.Item("SYS_DATE") = oFeatureReader.GetString("SYS_DATE")

					'	STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE


					oNewRow.Item("SHAPE_AREA") = oFeatureReader.GetDouble("SHAPE_AREA")
					oNewRow.Item("SHAPE_LEN") = oFeatureReader.GetDouble("SHAPE_LEN")
					'	DMCommon.Debug.MsgBox("04_412i", bInclGeometry, iPgonID)
					oByteArrayGeometry = oFeatureReader.GetGeometry("Geometry")
					If bInclGeometry AndAlso oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) > 0 Then
						oNewRow.Item("Geometry") = oByteArrayGeometry
					End If

					'	PARCEL_ID, GUSH_NUM, GUSH_SUFFI, Parcel, LEGAL_AREA, STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE, SHAPE_AREA, SHAPE_LEN

					oParcelsAllDataTable.Rows.Add(oNewRow)

					'	DMCommon.Debug.MsgBox("04_412j", oDataTable.Rows.Count)
					If bInclGeometry AndAlso iGeometryIndex <> -1 Then
						oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
						If oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) >= 0 Then

							oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
							If oGeometry IsNot Nothing Then


								oPoligon = FromGeometry(oGeometry)
								If oPoligon IsNot Nothing Then


									If dicGushConnections.TryGetValue(tGushData.Folder, oToConnection) Then
										sToClassName = "p" & tGushData.Folder
										Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
										Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

										insert.FeatureClassName = oToClassName
										Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
										' add the Int32 value to the insert command

										Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
										Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
										values.Add(oGushPropVal)

										Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
										Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
										values.Add(oGushAddPropVal)

										Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
										Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
										values.Add(oParcelIDPropVal)

										Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
										Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
										values.Add(oParcelPropVal)

										Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
										Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
										values.Add(oLegalAreaPropVal)

										Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
										Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
										values.Add(oStatusPropVal)

										' add the feature geometry to the insert command
										Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
										Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
										values.Add(geomPropVal)
										' insert the feature

										Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
										reader.Close()

										' DMCommon.ExcelLogG.SetNextValue(6, "+++ +++ +++", iBlock, oToConnection.ConnectionString)
									ElseIf dicGushConnections.Count <> 0 Then
										DMCommon.Debug.MsgBox("09_921", iBlock, dicGushConnections.Count, iBlock.ToString())
										bRes = False
									End If
								End If
							End If
						End If
					End If
					'       DMCommon.Debug.MsgBox("12_601", iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)

					bRes = True
					'If oGeometry IsNot Nothing Then
					'   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
					'End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & "|" & oEx.StackTrace & "|" & oEx.GetType().ToString() & ":" & vbCrLf)
					bRes = False
				End Try
				'If iPgonID Mod 2000 = 0 Then
				'DMCommon.Debug.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))
				'End If
				If iBulkRecordNo = 2000 Then   '&HFFF
					'oParcelsAllDataTable.AcceptChanges()


					DMCommon.Debug.MsgBox("04_412U", oParcelsAllDataTable.Rows.Count, iPgonID, iBulkRecordNo, oParcelAllDataAdapter.UpdateBatchSize)
					oParcelAllDataAdapter.Update(oParcelsAllDataTable)

					iBulkRecordNo = 0
				End If

				If iPgonID = 16000 Then
					Exit While
				End If
			End While

			'	DMCommon.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))
			DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, bInclGeometry, iPgonID)
			'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
		Else
			System.Windows.Forms.MessageBox.Show("oFeatureReader Is Nothing" & vbCrLf & sClassName, "04_409")
			bRes = False
		End If
		Return bRes
	End Function
	Public Shared Function InputParcelDataNew(sSHPFileName As String, sClassName As String, bInclGeometry As Boolean, ByRef oDataTable As Data.DataTable, ByRef oSqlBulkCopy As System.Data.SqlClient.SqlBulkCopy) As Boolean
		' Dim dicParcelData As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		'   Dim dicGushFromParcelData As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()

		Dim oFeatureClass As Schema.ClassDefinition
		Dim oToConnection As Connections.IConnectionImp = Nothing
		Dim sToClassName As String
		Dim bRes As Boolean = True
		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList())
		If oFeatureReader IsNot Nothing Then
			Dim iGeo As Integer = 0
			Dim sText As String
			Dim iPgonID As Integer
			Dim iBlock As Integer
			Dim iBlockAdd As Integer
			Dim iParcelName As Integer

			Dim iParcelID As Integer
			Dim dLegalArea As Double
			Dim iStatus As Integer
			Dim sPropName As String = ""

			Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte()
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
			Dim oPoligon As OSGeo.FDO.Geometry.IPolygon

			Dim tPrevGushData As GushData = New GushData()
			Dim tGushData As GushData = New GushData()
			Dim oNewRow As DataRow
			'	DMCommon.Debug.MsgBox("04_411a!!!!!", "GetParcelData", sSHPFileName, bInclGeometria, DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData))
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
			Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
			Dim oBulkDataTable As Data.DataTable
			Dim sDate As String
			Dim tDate As DateTime
			Dim tLastDate As DateTime

			iPgonID = 0
			oFeatureClass = oFeatureReader.GetClassDefinition
			'DMCommon.Debug.MsgBox("04_411b")
			Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
			' DMCommon.Debug.MsgBox("04_411c")

			'  DMCommon.Debug.MsgBox("04_411d !#!#!", bInclGeometria, tFilterList.UpperBound, DMCommon.Debug.ColCount(hsGush))
			If bInclGeometry Then

				'  DMCommon.ExcelLogG.Open()

				' DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)
			End If

			' DMCommon.Debug.MsgBox("04_410B", "GeometryIndex", iGeometryIndex)
			'	Dim iA, iB, iC As Integer
			oBulkDataTable = oDataTable.Clone()
			'DMCommon.Debug.MsgBox("04_412P1", oDataTable.Columns.Count, oBulkDataTable.Columns.Count)
			'oBulkDataTable = oDataTable.Copy()
			DMCommon.Debug.MsgBox("04_412P2", oDataTable.Columns.Count, oBulkDataTable.Columns.Count)
			While oFeatureReader.ReadNext()
				oNewRow = oBulkDataTable.NewRow
				Try
					iPgonID += 1
					sText = CStr(iPgonID) & ":"
					oNewRow.Item("PARCEL_ID") = oFeatureReader.GetInt32("PARCEL_ID")
					oNewRow.Item("GUSH_NUM") = oFeatureReader.GetInt32("GUSH_NUM")
					oNewRow.Item("GUSH_SUFFI") = oFeatureReader.GetInt32("GUSH_SUFFI")
					oNewRow.Item("PARCEL") = oFeatureReader.GetInt32("PARCEL")
					'	oNewRow.Item("PARCEL") =oFeatureReader.GetInt16("PARCEL")
					oNewRow.Item("LEGAL_AREA") = oFeatureReader.GetDouble("LEGAL_AREA")
					oNewRow.Item("STATUS") = oFeatureReader.GetInt16("STATUS")
					oNewRow.Item("STATUS_TEX") = oFeatureReader.GetString("STATUS_TEX")
					oNewRow.Item("LOCALITY_I") = oFeatureReader.GetInt32("LOCALITY_I")
					If Not oFeatureReader.IsNull("LOCALITY_N") Then
						oNewRow.Item("LOCALITY_N") = oFeatureReader.GetString("LOCALITY_N")
					End If
					oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")
					If Not oFeatureReader.IsNull("REG_MUN_NA") Then
						oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")
					End If
					'oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")
					'oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")
					oNewRow.Item("COUNTY_ID") = oFeatureReader.GetInt16("COUNTY_ID")
					If Not oFeatureReader.IsNull("COUNTY_NAM") Then
						oNewRow.Item("COUNTY_NAM") = oFeatureReader.GetString("COUNTY_NAM")
					End If
					oNewRow.Item("REGION_ID") = oFeatureReader.GetInt16("REGION_ID")
					If Not oFeatureReader.IsNull("REGION_NAM") Then
						oNewRow.Item("REGION_NAM") = oFeatureReader.GetString("REGION_NAM")
					End If
					oNewRow.Item("TALAR_NUMB") = oFeatureReader.GetInt16("TALAR_NUMB")
					oNewRow.Item("TALAR_YEAR") = oFeatureReader.GetInt16("TALAR_YEAR")
					If Not oFeatureReader.IsNull("SYS_DATE") Then
						sDate = oFeatureReader.GetString("SYS_DATE")
						If DateTime.TryParse(sDate, tDate) Then
							oNewRow.Item("SYS_DATE") = tDate
							If tLastDate < tDate Then
								tLastDate = tDate
							End If
						End If

					End If


					'	STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE


					oNewRow.Item("SHAPE_AREA") = oFeatureReader.GetDouble("SHAPE_AREA")
					oNewRow.Item("SHAPE_LEN") = oFeatureReader.GetDouble("SHAPE_LEN")
					'	DMCommon.Debug.MsgBox("04_412i", bInclGeometry, iPgonID)
					oByteArrayGeometry = oFeatureReader.GetGeometry("Geometry")
					If bInclGeometry AndAlso oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) > 0 Then
						oNewRow.Item("Geometry") = oByteArrayGeometry
					End If

					'	PARCEL_ID, GUSH_NUM, GUSH_SUFFI, Parcel, LEGAL_AREA, STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE, SHAPE_AREA, SHAPE_LEN
					'DMCommon.Debug.MsgBox("04_412z", oNewRow.IsNull("PARCEL_ID"))
					oBulkDataTable.Rows.Add(oNewRow)
					'	oDataTable.AcceptChanges()
					'DMCommon.Debug.MsgBox("04_412j", oBulkDataTable.Rows.Count)
					If bInclGeometry AndAlso iGeometryIndex <> -1 Then
						oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
						If oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) >= 0 Then

							oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
							If oGeometry IsNot Nothing Then


								oPoligon = FromGeometry(oGeometry)
								If oPoligon IsNot Nothing Then


									If dicGushConnections.TryGetValue(tGushData.Folder, oToConnection) Then
										sToClassName = "p" & tGushData.Folder
										Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
										Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

										insert.FeatureClassName = oToClassName
										Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
										' add the Int32 value to the insert command

										Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
										Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
										values.Add(oGushPropVal)

										Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
										Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
										values.Add(oGushAddPropVal)

										Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
										Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
										values.Add(oParcelIDPropVal)

										Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
										Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
										values.Add(oParcelPropVal)

										Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
										Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
										values.Add(oLegalAreaPropVal)

										Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
										Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
										values.Add(oStatusPropVal)

										' add the feature geometry to the insert command
										Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
										Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
										values.Add(geomPropVal)
										' insert the feature

										Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
										reader.Close()

										' DMCommon.ExcelLogG.SetNextValue(6, "+++ +++ +++", iBlock, oToConnection.ConnectionString)
									ElseIf dicGushConnections.Count <> 0 Then
										DMCommon.Debug.MsgBox("09_921", iBlock, dicGushConnections.Count, iBlock.ToString())
										bRes = False
									End If
								End If
							End If
						End If
					End If
					'       DMCommon.Debug.MsgBox("12_601", iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)

					bRes = True
					'If oGeometry IsNot Nothing Then
					'   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
					'End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("47err 1!! " & sPropName & "|" & oEx.Message & "|" & oEx.StackTrace & "|" & oEx.GetType().ToString() & ":" & vbCrLf)
					bRes = False
				End Try
				'If iPgonID Mod 2000 = 0 Then
				'DMCommon.Debug.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))
				'End If
				'	
				'oBulkDataTable

				If iPgonID = 100000 Then
					'''''''''''Exit While
				End If
			End While
			DMCommon.Debug.MsgBox("08_560CCZ", sSHPFileName, sClassName, bInclGeometry, oBulkDataTable.Rows.Count, iPgonID)
			Try
				oSqlBulkCopy.WriteToServer(oBulkDataTable)
				DMCommon.Debug.UserMsg("הכנסת חלקות", "File: ", sSHPFileName, "מס' פוליגונים: " & iPgonID.ToString(), " מס' חלקות: " & oBulkDataTable.Rows.Count, "תאריך אחרון: " & tLastDate.ToString())
			Catch ex As Exception

			End Try

			'	DMCommon.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))

			'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
		Else
			System.Windows.Forms.MessageBox.Show("oFeatureReader Is Nothing" & vbCrLf & sClassName, "04_409")
			bRes = False
		End If
		Return bRes
	End Function

	Public Shared Function InputParcelData(sSHPFileName As String, sClassName As String, bInclGeometry As Boolean, ByRef oDataTable As Data.DataTable) As Boolean
		' Dim dicParcelData As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		'   Dim dicGushFromParcelData As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()

		Dim oFeatureClass As Schema.ClassDefinition
		Dim oToConnection As Connections.IConnectionImp = Nothing
		Dim sToClassName As String
		Dim bRes As Boolean = True
		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList())
		If oFeatureReader IsNot Nothing Then
			Dim iGeo As Integer = 0
			Dim sText As String
			Dim iPgonID As Integer
			Dim iBlock As Integer
			Dim iBlockAdd As Integer
			Dim iParcelName As Integer

			Dim iParcelID As Integer
			Dim dLegalArea As Double
			Dim iStatus As Integer
			Dim sPropName As String = ""

			Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte()
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
			Dim oPoligon As OSGeo.FDO.Geometry.IPolygon

			Dim tPrevGushData As GushData = New GushData()
			Dim tGushData As GushData = New GushData()
			Dim oNewRow As DataRow
			'	DMCommon.Debug.MsgBox("04_411a!!!!!", "GetParcelData", sSHPFileName, bInclGeometria, DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData))
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
			Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()


			iPgonID = 0
			oFeatureClass = oFeatureReader.GetClassDefinition
			'DMCommon.Debug.MsgBox("04_411b")
			Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
			' DMCommon.Debug.MsgBox("04_411c")

			'  DMCommon.Debug.MsgBox("04_411d !#!#!", bInclGeometria, tFilterList.UpperBound, DMCommon.Debug.ColCount(hsGush))
			If bInclGeometry Then

				'  DMCommon.ExcelLogG.Open()

				' DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)
			End If

			' DMCommon.Debug.MsgBox("04_410B", "GeometryIndex", iGeometryIndex)
			'	Dim iA, iB, iC As Integer

			While oFeatureReader.ReadNext()

				oNewRow = oDataTable.NewRow
				Try
					iPgonID += 1
					'  DMCommon.ExcelLogX.SetNextValue(1, iPgonID)
					sText = CStr(iPgonID) & ":"



					oNewRow.Item("PARCEL_ID") = oFeatureReader.GetInt32("PARCEL_ID")
					oNewRow.Item("GUSH_NUM") = oFeatureReader.GetInt32("GUSH_NUM")
					oNewRow.Item("GUSH_SUFFI") = oFeatureReader.GetInt32("GUSH_SUFFI")
					'oNewRow.Item("PARCEL") = oFeatureReader.GetInt32("PARCEL")
					oNewRow.Item("PARCEL") = 1111 'oFeatureReader.GetInt16("PARCEL")

					oNewRow.Item("LEGAL_AREA") = oFeatureReader.GetDouble("LEGAL_AREA")

					oNewRow.Item("STATUS") = oFeatureReader.GetInt16("STATUS")
					oNewRow.Item("STATUS_TEX") = oFeatureReader.GetString("STATUS_TEX")


					oNewRow.Item("LOCALITY_I") = oFeatureReader.GetInt32("LOCALITY_I")

					If Not oFeatureReader.IsNull("LOCALITY_N") Then
						oNewRow.Item("LOCALITY_N") = oFeatureReader.GetString("LOCALITY_N")
					End If


					oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")

					If Not oFeatureReader.IsNull("REG_MUN_NA") Then
						oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")
					End If


					'oNewRow.Item("REG_MUN_ID") = oFeatureReader.GetInt16("REG_MUN_ID")
					'oNewRow.Item("REG_MUN_NA") = oFeatureReader.GetString("REG_MUN_NA")

					oNewRow.Item("COUNTY_ID") = oFeatureReader.GetInt16("COUNTY_ID")

					If Not oFeatureReader.IsNull("COUNTY_NAM") Then
						oNewRow.Item("COUNTY_NAM") = oFeatureReader.GetString("COUNTY_NAM")
					End If


					oNewRow.Item("REGION_ID") = oFeatureReader.GetInt16("REGION_ID")

					If Not oFeatureReader.IsNull("REGION_NAM") Then
						oNewRow.Item("REGION_NAM") = oFeatureReader.GetString("REGION_NAM")
					End If


					oNewRow.Item("TALAR_NUMB") = oFeatureReader.GetInt16("TALAR_NUMB")
					oNewRow.Item("TALAR_YEAR") = oFeatureReader.GetInt16("TALAR_YEAR")
					If Not oFeatureReader.IsNull("SYS_DATE") Then
						oNewRow.Item("SYS_DATE") = oFeatureReader.GetString("SYS_DATE")
					End If

					'	STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE


					oNewRow.Item("SHAPE_AREA") = oFeatureReader.GetDouble("SHAPE_AREA")
					oNewRow.Item("SHAPE_LEN") = oFeatureReader.GetDouble("SHAPE_LEN")
					'	DMCommon.Debug.MsgBox("04_412i", bInclGeometry, iPgonID)
					oByteArrayGeometry = oFeatureReader.GetGeometry("Geometry")
					If bInclGeometry AndAlso oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) > 0 Then
						oNewRow.Item("Geometry") = oByteArrayGeometry
					End If

					'	PARCEL_ID, GUSH_NUM, GUSH_SUFFI, Parcel, LEGAL_AREA, STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE, SHAPE_AREA, SHAPE_LEN

					oDataTable.Rows.Add(oNewRow)
					'oDataTable.AcceptChanges()
					'	DMCommon.Debug.MsgBox("04_412j", oDataTable.Rows.Count)
					If bInclGeometry AndAlso iGeometryIndex <> -1 Then
						oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
						If oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) >= 0 Then

							oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
							If oGeometry IsNot Nothing Then


								oPoligon = FromGeometry(oGeometry)
								If oPoligon IsNot Nothing Then


									If dicGushConnections.TryGetValue(tGushData.Folder, oToConnection) Then
										sToClassName = "p" & tGushData.Folder
										Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
										Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

										insert.FeatureClassName = oToClassName
										Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
										' add the Int32 value to the insert command

										Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
										Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
										values.Add(oGushPropVal)

										Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
										Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
										values.Add(oGushAddPropVal)

										Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
										Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
										values.Add(oParcelIDPropVal)

										Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
										Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
										values.Add(oParcelPropVal)

										Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
										Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
										values.Add(oLegalAreaPropVal)

										Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
										Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
										values.Add(oStatusPropVal)

										' add the feature geometry to the insert command
										Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
										Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
										values.Add(geomPropVal)
										' insert the feature

										Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
										reader.Close()

										' DMCommon.ExcelLogG.SetNextValue(6, "+++ +++ +++", iBlock, oToConnection.ConnectionString)
									ElseIf dicGushConnections.Count <> 0 Then
										DMCommon.Debug.MsgBox("09_921", iBlock, dicGushConnections.Count, iBlock.ToString())
										bRes = False
									End If
								End If
							End If
						End If
					End If
					'       DMCommon.Debug.MsgBox("12_601", iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)

					bRes = True
					'If oGeometry IsNot Nothing Then
					'   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
					'End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & "|" & oEx.StackTrace & "|" & oEx.GetType().ToString() & ":" & vbCrLf)
					bRes = False
				End Try
				'If iPgonID Mod 2000 = 0 Then
				'DMCommon.Debug.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))
				'End If
				If iPgonID = 10000 Then
					Exit While
				End If
			End While

			'	DMCommon.ExcelLog.SetValue(0, "", iPgonID, oFeatureReader.GetInt32("PARCEL_ID"))
			DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, bInclGeometry, iPgonID)
			'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
		Else
			System.Windows.Forms.MessageBox.Show("oFeatureReader Is Nothing" & vbCrLf & sClassName, "04_409")
			bRes = False
		End If
		Return bRes
	End Function
	Public Shared Function GetParcelData(sSHPFileName As String, sClassName As String, hsGush As HashSet(Of Integer), tFilterList As DMCommon.dmList, ByRef dicParcelData As Dictionary(Of Integer, ParcelData), ByRef dicGushFromParcelData As Dictionary(Of Integer, GushData), bInclGeometria As Boolean, bInnerPolygons As Boolean) As Boolean
		' Dim dicParcelData As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
		'   Dim dicGushFromParcelData As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()

		Dim oFeatureClass As Schema.ClassDefinition
		Dim oToConnection As Connections.IConnectionImp = Nothing
		Dim sToClassName As String
		Dim bRes As Boolean = True
		Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, tFilterList)
		If oFeatureReader IsNot Nothing Then
			Dim iGeo As Integer = 0
			Dim sText As String
			Dim iPgonID As Integer
			Dim iBlock As Integer
			Dim iBlockAdd As Integer
			Dim iParcelName As Integer
			Dim sParcelName As String
			Dim iParcelID As Integer
			Dim dLegalArea As Double
			Dim iStatus As Integer
			Dim sPropName As String = ""
			Dim iaPropIndices() = {-1, -1, -1, -1, -1, -1}
			Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte()
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
			Dim oPoligon As OSGeo.FDO.Geometry.IPolygon
			Dim sDestSHPFile As String
			Dim tPrevGushData As GushData = New GushData()
			Dim tGushData As GushData

			'	DMCommon.Debug.MsgBox("04_411a!!!!!", "GetParcelData", sSHPFileName, bInclGeometria, DMCommon.Debug.ColCount(hsGush), DMCommon.Debug.ColCount(dicGushFromParcelData))
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
			Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
			iPgonID = 0
			oFeatureClass = oFeatureReader.GetClassDefinition
			'DMCommon.Debug.MsgBox("04_411b")
			Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
			' DMCommon.Debug.MsgBox("04_411c")
			Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
			dicParcelData = New Dictionary(Of Integer, ParcelData)()
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   dicGushFromParcelData = New Dictionary(Of Integer, GushData)()
			If dicGushFromParcelData Is Nothing Then
				dicGushFromParcelData = New Dictionary(Of Integer, GushData)()
			End If
			'  DMCommon.Debug.MsgBox("04_411d !#!#!", bInclGeometria, tFilterList.UpperBound, DMCommon.Debug.ColCount(hsGush))
			If bInclGeometria Then
				DMAcadExt.AcadTransaction.CreateLayer("1602")
				DMAcadExt.AcadTransaction.CreateLayer("1602Inner")
				'  DMCommon.ExcelLogG.Open()




				For Each tGushData In dicGushFromParcelData.Values

					sDestSHPFile = zzDestSHPFileName(sSHPFileName, tGushData.Folder)
					'	DMCommon.Debug.MsgBox("09_662", sSHPFileName, sDestSHPFile, tGushData.Folder)
					oToConnection = zzGetConnection(sDestSHPFile)
					dicGushConnections.Add(tGushData.Folder, oToConnection)
				Next


				' DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)
			End If
			'   DMCommon.Debug.MsgBox("04_410A")
			For iIndex As Integer = 0 To colProperties.Count - 1
				oPropDef = colProperties.Item(iIndex)

				sPropName = oPropDef.Name
				'    DMCommon.Debug.MsgBox("01_420", oPropDef.Name, iIndex)
				Select Case sPropName
					Case "PARCEL_ID"
						iaPropIndices(enParcelFields.ParcelID) = iIndex
					Case "GUSH_NO", "GUSH_NUM"
						iaPropIndices(enParcelFields.BlockNo) = iIndex
					Case "GUSH_SUFFI"
						iaPropIndices(enParcelFields.BlockAdd) = iIndex
					Case "PARCEL"
						iaPropIndices(enParcelFields.ParcelName) = iIndex
					Case "LEGAL_AREA"
						iaPropIndices(enParcelFields.LegalArea) = iIndex
					Case "STATUS_ID", "STATUS"
						If iaPropIndices(enParcelFields.Status) = -1 Then
							iaPropIndices(enParcelFields.Status) = iIndex
						End If
					Case "Geometry"
						iGeometryIndex = iIndex
				End Select
			Next
			' DMCommon.Debug.MsgBox("04_410B", "GeometryIndex", iGeometryIndex)
			Dim iA, iB, iC As Integer


			While oFeatureReader.ReadNext()
				iA = 0 : iB = 0 : iC = 0
				Try
					iPgonID += 1
					'  DMCommon.ExcelLogX.SetNextValue(1, iPgonID)
					sText = CStr(iPgonID) & ":"
					If iaPropIndices(enParcelFields.BlockNo) <> -1 Then
						iBlock = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockNo))
					Else
						iBlock = 0
					End If
					If iaPropIndices(enParcelFields.BlockAdd) <> -1 Then
						iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockAdd))
					Else
						iBlockAdd = 0
					End If
					If iaPropIndices(enParcelFields.ParcelName) <> -1 Then
						iParcelName = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelName))
					Else
						iParcelName = 0
					End If
					If iaPropIndices(enParcelFields.ParcelID) <> -1 Then
						iParcelID = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelID))
					Else
						iParcelID = 0
					End If
					If iaPropIndices(enParcelFields.LegalArea) <> -1 Then
						dLegalArea = oFeatureReader.GetDouble(iaPropIndices(enParcelFields.LegalArea))
					Else
						dLegalArea = 0.0
					End If
					If iaPropIndices(enParcelFields.Status) <> -1 Then
						iStatus = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.Status))
					Else
						iStatus = 0
					End If
					tGushData = New GushData(0, iBlock, iBlockAdd, iStatus, 0, dLegalArea)

					sParcelName = Convert.ToString(iParcelName)
					iA = 1
					If bInclGeometria AndAlso iGeometryIndex <> -1 Then
						oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
						If oByteArrayGeometry IsNot Nothing AndAlso oByteArrayGeometry.GetUpperBound(0) >= 0 Then

							oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
							If oGeometry IsNot Nothing Then


								oPoligon = FromGeometry(oGeometry)
								If oPoligon IsNot Nothing Then
									If bInnerPolygons Then
										zzFDO_Polygon(oPoligon, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
									End If

									If dicGushConnections.TryGetValue(tGushData.Folder, oToConnection) Then
										sToClassName = "p" & tGushData.Folder
										Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
										Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

										insert.FeatureClassName = oToClassName
										Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
										' add the Int32 value to the insert command

										Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
										Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
										values.Add(oGushPropVal)

										Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
										Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
										values.Add(oGushAddPropVal)

										Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
										Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
										values.Add(oParcelIDPropVal)

										Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
										Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
										values.Add(oParcelPropVal)

										Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
										Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
										values.Add(oLegalAreaPropVal)

										Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
										Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
										values.Add(oStatusPropVal)

										' add the feature geometry to the insert command
										Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
										Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
										values.Add(geomPropVal)
										' insert the feature
										iB = 1
										Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
										reader.Close()
										iC = 1
										' DMCommon.ExcelLogG.SetNextValue(6, "+++ +++ +++", iBlock, oToConnection.ConnectionString)
									ElseIf dicGushConnections.Count <> 0 Then
										DMCommon.Debug.MsgBox("09_921", iBlock, dicGushConnections.Count, iBlock.ToString())
										bRes = False
									End If
								End If
							End If
						End If
					End If
					'       DMCommon.Debug.MsgBox("12_601", iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus)
					dicParcelData.Add(iPgonID, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
					If dicGushFromParcelData.TryGetValue(tGushData.Key, tPrevGushData) Then
						tGushData += tPrevGushData
						dicGushFromParcelData.Remove(tGushData.Key)
					End If
					'   DMCommon.Debug.MsgBox("12_607a", tGushData.Key, tGushData.Block)
					dicGushFromParcelData.Add(tGushData.Key, tGushData)

					'If oGeometry IsNot Nothing Then
					'   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
					'End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & "|" & oEx.StackTrace & "|" & oEx.GetType().ToString() & ":" & vbCrLf)
					bRes = False
				End Try
				'  DMCommon.ExcelLogX.SetValue(1, iA, iB, iC, iPgonID, iBlock, sParcelName, dLegalArea)
			End While

			'  DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, bInclGeometria, dicParcelData.Count, dicGushFromParcelData.Count, iPgonID)
			'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
		Else
			System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")
		End If
		Return bRes
	End Function
	Public Shared Function GetParcelData_220118(sSHPFileName As String, sClassName As String, tFilterList As DMCommon.dmList, Optional bInclGeometria As Boolean = False) As Dictionary(Of Integer, ParcelData)
      Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oToConnection As Connections.IConnectionImp = Nothing
      Dim sToClassName As String
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, tFilterList)
      If oFeatureReader IsNot Nothing Then
         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim iBlock As Integer
         Dim iBlockAdd As Integer
         Dim iParcelName As Integer
         Dim sParcelName As String
         Dim iParcelID As Integer
         Dim dLegalArea As Double
         Dim iStatus As Integer
         Dim sPropName As String = ""
         Dim iaPropIndices() = {-1, -1, -1, -1, -1, -1}
         Dim iGeometryIndex As Integer = -1
         Dim oByteArrayGeometry As Byte()
         Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
         Dim oPoligon As OSGeo.FDO.Geometry.IPolygon
         Dim sDestSHPFile As String
         '    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition
         Dim dicGushConnections As IDictionary(Of String, Connections.IConnectionImp) = New Dictionary(Of String, Connections.IConnectionImp)()
         Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         If bInclGeometria Then
            DMAcadExt.AcadTransaction.CreateLayer("1602")
            DMAcadExt.AcadTransaction.CreateLayer("1602Inner")
            '  DMCommon.ExcelLogG.Open()
            For iIndex = 0 To tFilterList.UpperBound

               sDestSHPFile = zzDestSHPFileName(sSHPFileName, tFilterList.Item(iIndex))
               '  DMCommon.Debug.MsgBox("09_662", sSHPFileName, sDestSHPFile)
               oToConnection = zzGetConnection(sDestSHPFile)
               dicGushConnections.Add(tFilterList.Item(iIndex), oToConnection)
            Next

            '   DMCommon.Debug.MsgBox("09_940", dicGushConnections.Count, colProperties.Count)


         End If

         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)

            sPropName = oPropDef.Name
            '    DMCommon.Debug.MsgBox("01_420", oPropDef.Name, iIndex)
            Select Case sPropName
               Case "PARCEL_ID"
                  iaPropIndices(enParcelFields.ParcelID) = iIndex
               Case "GUSH_NO", "GUSH_NUM"
                  iaPropIndices(enParcelFields.BlockNo) = iIndex
               Case "GUSH_SUFFI"
                  iaPropIndices(enParcelFields.BlockAdd) = iIndex
               Case "PARCEL"
                  iaPropIndices(enParcelFields.ParcelName) = iIndex
               Case "LEGAL_AREA"
                  iaPropIndices(enParcelFields.LegalArea) = iIndex
               Case "STATUS_ID", "STATUS"
                  If iaPropIndices(enParcelFields.Status) = -1 Then
                     iaPropIndices(enParcelFields.Status) = iIndex
                  End If
               Case "Geometry"
                  iGeometryIndex = iIndex
            End Select
         Next

         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enParcelFields.BlockNo) <> -1 Then
                  iBlock = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockNo))
               Else
                  iBlock = 0
               End If
               If iaPropIndices(enParcelFields.BlockAdd) <> -1 Then
                  iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockAdd))
               Else
                  iBlockAdd = 0
               End If
               If iaPropIndices(enParcelFields.ParcelName) <> -1 Then
                  iParcelName = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelName))
               Else
                  iParcelName = 0
               End If
               If iaPropIndices(enParcelFields.ParcelID) <> -1 Then
                  iParcelID = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelID))
               Else
                  iParcelID = 0
               End If
               If iaPropIndices(enParcelFields.LegalArea) <> -1 Then
                  dLegalArea = oFeatureReader.GetDouble(iaPropIndices(enParcelFields.LegalArea))
               Else
                  dLegalArea = 0.0
               End If
               If iaPropIndices(enParcelFields.Status) <> -1 Then
                  iStatus = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.Status))
               Else
                  iStatus = 0
               End If
               sParcelName = Convert.ToString(iParcelName)
               If bInclGeometria AndAlso iGeometryIndex <> -1 Then
                  oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
                  oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
                  oPoligon = FromGeometry(oGeometry)
                  zzFDO_Polygon(oPoligon, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
                  If dicGushConnections.TryGetValue(iBlock.ToString(), oToConnection) Then
                     sToClassName = "p" & iBlock.ToString()
                     Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
                     Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)

                     insert.FeatureClassName = oToClassName
                     Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
                     ' add the Int32 value to the insert command

                     Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
                     Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
                     values.Add(oGushPropVal)

                     Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
                     Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
                     values.Add(oGushAddPropVal)

                     Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
                     Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
                     values.Add(oParcelIDPropVal)

                     Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
                     Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
                     values.Add(oParcelPropVal)

                     Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
                     Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
                     values.Add(oLegalAreaPropVal)

                     Dim iStatusValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iStatus)
                     Dim oStatusPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("STATUS", iStatusValue)
                     values.Add(oStatusPropVal)

                     ' add the feature geometry to the insert command
                     Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
                     Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
                     values.Add(geomPropVal)
                     ' insert the feature
                     Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
                     reader.Close()
                     ' DMCommon.ExcelLogG.SetNextValue(6, "+++ ++++ ++", iBlock, oToConnection.ConnectionString)
                  Else
                     DMCommon.Debug.MsgBox("09_921", dicGushConnections.Count, iBlock.ToString())
                  End If
               End If
               dicRes.Add(iPgonID, New ParcelData(iPgonID, iBlock, iBlockAdd, sParcelName, dLegalArea, iStatus))
               'If oGeometry IsNot Nothing Then
               '   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
               'End If
            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("40err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
            End Try
         End While
         '   DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, bInclGeometria, dicRes.Count, iPgonID)
         'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")
      End If
      Return dicRes
   End Function
   Public Shared Function CopyParcelData(sSHPFileName As String, sClassName As String, tFilterList As DMCommon.dmList, sToSHPFileName As String) As Dictionary(Of Integer, ParcelData)
      Dim dicRes As Dictionary(Of Integer, ParcelData) = New Dictionary(Of Integer, ParcelData)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oToConnection As Connections.IConnectionImp
      Dim sToClassName As String = "tmp"
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, tFilterList)
      If oFeatureReader IsNot Nothing Then

         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim iBlock As Integer
         Dim iBlockAdd As Integer
         Dim iParcelID As Integer
         Dim iParcelName As Integer

         Dim sParcelName As String
         Dim dLegalArea As Double
         Dim iStatus As Integer
         Dim sPropName As String = ""
         Dim iaPropIndices() = {-1, -1, -1, -1, -1, -1}
         Dim iGeometryIndex As Integer = -1
			Dim oByteArrayGeometry As Byte() = Nothing
			Dim oGeometry As OSGeo.FDO.Geometry.IGeometry = Nothing
         Dim oPoligon As OSGeo.FDO.Geometry.IPolygon
			'Dim oRing As OSGeo.FDO.Geometry.ILinearRing
			'    Dim oPosition As OSGeo.FDO.Geometry.IDirectPosition

			Dim oGeomFactory As OSGeo.FDO.Geometry.FgfGeometryFactory = New Geometry.FgfGeometryFactory()
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         oToConnection = zzGetConnection(sToSHPFileName)
         '      DMCommon.ExcelLogG.Open()
         ''''''''''''''   Dim tShapeFOData As ShapeFOData
         '''''''''''''''''  tShapeFOData = New ShapeFOData(sShapeFolderName, sFileName)

         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)

            sPropName = oPropDef.Name
            '    DMCommon.Debug.MsgBox("01_420", oPropDef.Name, iIndex)
            Select Case sPropName
               Case "PARCEL_ID"
                  iaPropIndices(enParcelFields.ParcelID) = iIndex
               Case "GUSH_NO", "GUSH_NUM"
                  iaPropIndices(enParcelFields.BlockNo) = iIndex
               Case "GUSH_SUFFI"
                  iaPropIndices(enParcelFields.BlockAdd) = iIndex
               Case "PARCEL"
                  iaPropIndices(enParcelFields.ParcelName) = iIndex
               Case "LEGAL_AREA"
                  iaPropIndices(enParcelFields.LegalArea) = iIndex
               Case "STATUS_ID", "STATUS"
                  If iaPropIndices(enParcelFields.Status) = -1 Then
                     iaPropIndices(enParcelFields.Status) = iIndex
                  End If
               Case "Geometry"
                  iGeometryIndex = iIndex
            End Select
         Next


         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enParcelFields.BlockNo) <> -1 Then
                  iBlock = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockNo))
               Else
                  iBlock = 0
               End If
               If iaPropIndices(enParcelFields.BlockAdd) <> -1 Then
                  iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.BlockAdd))
               Else
                  iBlockAdd = 0
               End If
               If iaPropIndices(enParcelFields.ParcelID) <> -1 Then
                  iParcelID = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelID))
               Else
                  iParcelID = 0
               End If
               If iaPropIndices(enParcelFields.ParcelName) <> -1 Then
                  iParcelName = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.ParcelName))
               Else
                  iParcelName = 0
               End If


               If iaPropIndices(enParcelFields.LegalArea) <> -1 Then
                  dLegalArea = oFeatureReader.GetDouble(iaPropIndices(enParcelFields.LegalArea))
               Else
                  dLegalArea = 0.0
               End If
               If iaPropIndices(enParcelFields.Status) <> -1 Then
                  iStatus = oFeatureReader.GetInt32(iaPropIndices(enParcelFields.Status))
               Else
                  iStatus = 0
               End If
               sParcelName = Convert.ToString(iParcelName)
               If iGeometryIndex <> -1 Then
                  oByteArrayGeometry = oFeatureReader.GetGeometry(iGeometryIndex)
                  oGeometry = oGeomFactory.CreateGeometryFromFgf(oByteArrayGeometry)
                  oPoligon = FromGeometry(oGeometry)


               End If




               If True Then


                  Dim insert As Commands.Feature.IInsert = CType(oToConnection.CreateCommand(OSGeo.FDO.Commands.CommandType.CommandType_Insert), Commands.Feature.IInsert)
                  Dim oToClassName As Expression.Identifier = New Expression.Identifier(sToClassName)


                  insert.FeatureClassName = oToClassName
                  Dim values As OSGeo.FDO.Commands.PropertyValueCollection = insert.PropertyValues
                  ' add the Int32 value to the insert command


                  Dim iGushValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlock)
                  Dim oGushPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_NUM", iGushValue)
                  values.Add(oGushPropVal)

                  Dim iGushAddValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iBlockAdd)
                  Dim oGushAddPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("GUSH_SUFFI", iGushAddValue)
                  values.Add(oGushAddPropVal)

                  Dim iParcelIDValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelID)
                  Dim oParcelIDPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL_ID", iParcelIDValue)
                  values.Add(oParcelIDPropVal)

                  Dim iParcelValue As OSGeo.FDO.Expression.Int32Value = New OSGeo.FDO.Expression.Int32Value(iParcelName)
                  Dim oParcelPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("PARCEL", iParcelValue)
                  values.Add(oParcelPropVal)

                  Dim dLegalAreaValue As OSGeo.FDO.Expression.DoubleValue = New OSGeo.FDO.Expression.DoubleValue(dLegalArea)
                  Dim oLegalAreaPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("LEGAL_AREA", dLegalAreaValue)
                  values.Add(oLegalAreaPropVal)



                  ' add the feature geometry to the insert command

                  Dim geomVal As OSGeo.FDO.Expression.GeometryValue = New OSGeo.FDO.Expression.GeometryValue(oByteArrayGeometry)
                  Dim geomPropVal As OSGeo.FDO.Commands.PropertyValue = New OSGeo.FDO.Commands.PropertyValue("Geometry", geomVal)
                  values.Add(geomPropVal)
                  ' insert the feature
                  Dim reader As OSGeo.FDO.Commands.Feature.IFeatureReader = insert.Execute()
                  reader.Close()

               End If







               'If oGeometry IsNot Nothing Then
               '   DMCommon.ExcelLogG.SetNextValue(i, 0, sParcelName, oGeometry.DerivedType.ToString(), oGeometry.Dimensionality, oGeometry.Envelope.MaxX.ToString() & "," & oGeometry.Envelope.MinX.ToString(), oGeometry.Text)
               'End If

            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("47err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
            End Try

         End While
         DMCommon.Debug.MsgBox("08_560", sSHPFileName, sClassName, dicRes.Count, iPgonID)
         'DMCommon.Debug.MsgBox("01_424", oGeometry.Dimensionality, oGeometry.Text)
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")

      End If

      Return dicRes
   End Function
   Public Shared Function FromGeometry(oIn As OSGeo.FDO.Geometry.IGeometry) As OSGeo.FDO.Geometry.IPolygon
      Dim oPolygon As OSGeo.FDO.Geometry.IPolygon
      Select Case oIn.DerivedType
         Case Common.GeometryType.GeometryType_Polygon
            oPolygon = CType(oIn, OSGeo.FDO.Geometry.IPolygon)
			Case Else
				DMCommon.Debug.MsgBox("13_109c", oIn.DerivedType.ToString())
				oPolygon = Nothing
      End Select
      Return oPolygon
   End Function

   Public Shared Function GetGushData(sSHPFileName As String, sClassName As String, tFilterList As DMCommon.dmList) As Dictionary(Of Integer, GushData)

      Dim dicRes As Dictionary(Of Integer, GushData) = New Dictionary(Of Integer, GushData)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, tFilterList)
      If oFeatureReader IsNot Nothing Then

         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim iBlock As Integer
         Dim iBlockAdd As Integer
         Dim iStatus As Integer
         Dim iIsAnality As Integer
         Dim dLegalArea As Double 'For future

         Dim sPropName As String = ""
         Dim iaPropIndices() = {-1, -1, -1, -1}
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)
            '	System.Windows.Forms.MessageBox.Show(oPropDef.Name, "01_420")
            Select Case oPropDef.Name
               Case "GUSH_NO", "GUSH_NUM"
                  iaPropIndices(enGushFields.BlockNo) = iIndex
               Case "GUSH_SUFFI"
                  iaPropIndices(enGushFields.BlockAdd) = iIndex
               Case "STATUS_ID", "STATUS"
                  If iaPropIndices(enGushFields.Status) = -1 Then
                     iaPropIndices(enGushFields.Status) = iIndex
                  End If
               Case "IS_ANALITY"
                  iaPropIndices(enGushFields.IsAnality) = iIndex
            End Select
         Next
         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enGushFields.BlockNo) <> -1 Then
                  Try
                     iBlock = oFeatureReader.GetInt32(iaPropIndices(enGushFields.BlockNo))
                  Catch oEx As Exception
                     DMAcadExt.AcadDocument.WriteMessage("44err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
                  End Try
               Else
                  iBlock = 0
               End If
               If iaPropIndices(enGushFields.BlockAdd) <> -1 Then
                  Try
                     iBlockAdd = oFeatureReader.GetInt32(iaPropIndices(enGushFields.BlockAdd))
                  Catch oEx As Exception
                     DMAcadExt.AcadDocument.WriteMessage("45err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
                  End Try
               Else
                  iBlockAdd = 0
               End If

               If iaPropIndices(enGushFields.Status) <> -1 Then
                  Try
                     iStatus = oFeatureReader.GetInt32(iaPropIndices(enGushFields.Status))
                  Catch oEx As Exception
                     DMAcadExt.AcadDocument.WriteMessage("46err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)

                  End Try

               Else
                  iStatus = 0
               End If
               If iaPropIndices(enGushFields.IsAnality) <> -1 Then
                  Try
                     iIsAnality = oFeatureReader.GetInt32(iaPropIndices(enGushFields.IsAnality))
                  Catch oEx As Exception
                     DMAcadExt.AcadDocument.WriteMessage("47err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
                  End Try
               Else
                  iBlockAdd = 0
               End If
               dicRes.Add(iPgonID, New GushData(iPgonID, iBlock, iBlockAdd, iStatus, iIsAnality, dLegalArea))
            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("49err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
            End Try
         End While
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_403")
      End If
      Return dicRes
   End Function


   Public Shared Function GetLotData_Complot(sSHPFileName As String, sClassName As String) As Dictionary(Of Integer, LotData_Complot)
      Dim dicRes As Dictionary(Of Integer, LotData_Complot) = New Dictionary(Of Integer, LotData_Complot)()
      Dim oFeatureClass As Schema.ClassDefinition
      Dim oFeatureReader As OSGeo.FDO.Commands.Feature.IFeatureReader = zzGetReader(sSHPFileName, sClassName, New DMCommon.dmList())
      If oFeatureReader IsNot Nothing Then
         Dim iGeo As Integer = 0
         Dim sText As String
         Dim iPgonID As Integer
         Dim sLayer As String

         Dim iGush As Integer
         Dim iHelka As Integer

         Dim iYkd2 As Integer
         Dim sYstr As String

         'Dim iParcelName As Integer
         'Dim sParcelName As String
         'Dim dLegalArea As Double
         'Dim iStatus As Integer
         Dim sPropName As String = ""
         Dim iaPropIndices() = {-1, -1, -1, -1, -1}
         iPgonID = 0
         oFeatureClass = oFeatureReader.GetClassDefinition
         Dim colProperties As OSGeo.FDO.Schema.PropertyDefinitionCollection = oFeatureClass.Properties
         Dim oPropDef As OSGeo.FDO.Schema.PropertyDefinition
         For iIndex As Integer = 0 To colProperties.Count - 1
            oPropDef = colProperties.Item(iIndex)
            sPropName = oPropDef.Name
            '	System.Windows.Forms.MessageBox.Show(oPropDef.Name, "01_420")
            Select Case sPropName
               Case "LAYER"
                  iaPropIndices(enLot_ComplotFields.Layer) = iIndex
               Case "Gush"
                  iaPropIndices(enLot_ComplotFields.Gush) = iIndex
               Case "Helka"
                  iaPropIndices(enLot_ComplotFields.Helka) = iIndex
               Case "Ykd2"
                  iaPropIndices(enLot_ComplotFields.Ykd2) = iIndex
               Case "Ystr"
                  iaPropIndices(enLot_ComplotFields.YStr) = iIndex
            End Select
         Next



         While oFeatureReader.ReadNext()
            Try
               iPgonID += 1
               sText = CStr(iPgonID) & ":"
               If iaPropIndices(enLot_ComplotFields.Layer) <> -1 Then
                  sLayer = oFeatureReader.GetString(iaPropIndices(enLot_ComplotFields.Layer))
               Else
                  sLayer = String.Empty
               End If
               If iaPropIndices(enLot_ComplotFields.Gush) <> -1 Then
                  iGush = oFeatureReader.GetInt32(iaPropIndices(enLot_ComplotFields.Gush))
               Else
                  iGush = 0
               End If
               If iaPropIndices(enLot_ComplotFields.Helka) <> -1 Then
                  iHelka = oFeatureReader.GetInt32(iaPropIndices(enLot_ComplotFields.Helka))
               Else
                  iHelka = 0
               End If

               If iaPropIndices(enLot_ComplotFields.Ykd2) <> -1 Then
                  iYkd2 = oFeatureReader.GetInt32(iaPropIndices(enLot_ComplotFields.Ykd2))
               Else
                  iYkd2 = 0
               End If
               If iaPropIndices(enLot_ComplotFields.YStr) <> -1 Then
                  sYstr = oFeatureReader.GetString(iaPropIndices(enLot_ComplotFields.YStr))
               Else
                  sYstr = String.Empty
               End If


               dicRes.Add(iPgonID, New LotData_Complot(iPgonID, sLayer, iGush, iHelka, iYkd2, sYstr))
            Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("41err 1!! " & sPropName & "|" & oEx.Message & ":" & vbCrLf)
				End Try
         End While
      Else
         System.Windows.Forms.MessageBox.Show("oFeatureReader Is NOTHING" & vbCrLf & sClassName, "04_409")

      End If
      Return dicRes
   End Function

   Public ReadOnly Property PgonImported As Integer
      Get
         Return miPgonImported
      End Get
   End Property
   Public Property WorkAreaBound As Polyline
      Get
         Return moWorkAreaBound
      End Get
      Set(oValue As Polyline)
         moWorkAreaBound = oValue
      End Set

   End Property
   Public ReadOnly Property HasBlockAdd As Boolean
      Get
         Return mbHasBlockAdd
      End Get
   End Property


   Public Shared Function GetAppName(sTopoName As String) As String
      Return msAppPrefix & sTopoName
   End Function
   Public Sub New(iMapTheme As DMAcadExt.enMapTheme, iTopoPurpose As DMAcadExt.enTopoPurpose, sName As String)
      miMapTheme = iMapTheme
      msName = sName
      miTopoPurpose = iTopoPurpose
      zzCreateLinkTable()
      zzCreateCentroidTable()
		zzCreateIntersectTable()
		miSource = enPolygonSetSource.Polylines
		'DMCommon.Debug.MsgBox("03_122a", iMapTheme.ToString(), iTopoPurpose.ToString, sName, miSource.ToString)

	End Sub
   Public Sub New(ByVal iMapTheme As DMAcadExt.enMapTheme)
      miMapTheme = iMapTheme
      '   mbParcel = bParcel
      miSource = enPolygonSetSource.Shapes
		DMCommon.Debug.MsgBox("03_120", miSource.ToString)
	End Sub
   Public Sub New(ByVal tShapeFOData As FDO_Manager.ShapeFOData, ByVal bParcel As Boolean)
      mbParcel = bParcel
      miSource = enPolygonSetSource.Shapes
      mtShapeFOData = tShapeFOData
      zzCreateLinkTable()
      zzCreateCentroidTable()
		DMCommon.Debug.MsgBox("03_121", bParcel, miSource.ToString)
	End Sub

   Public Sub AddCentroids(colCentroids As ObjectIdCollection)
      ' mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)
      mcolCentroids = colCentroids
      If miSource = enPolygonSetSource.Shapes Then
         zzFillCentroidDict()
      End If
      If miSource = enPolygonSetSource.Polylines Then
         zzFillCentroidTable()
      End If




   End Sub

   Public Sub SelectAnalitic(colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sTopoName As String, sAddLayerName As String)
      Dim oEntity As Entity
      Dim oXDataParcel As DMAcadExt.TplnXDataParcel
      Dim tCentroidObjID As ObjectId
      Dim tPolylineObjID As ObjectId

      Dim tPoint3d As Point3d
      Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Topology.OpenMode.ForRead, False, False)
      Dim oTopoPgon As Autodesk.Gis.Map.Topology.Polygon = Nothing

      Dim mcolNeighbors As Autodesk.AutoCAD.Geometry.IntegerCollection = New Autodesk.AutoCAD.Geometry.IntegerCollection()
      Dim tList As DMCommon.dmList
      Dim iPgonID As Integer
      DMAcadExt.AcadTransaction.CreateLayer(sAddLayerName, 1, DMAcadExt.enLayerFunction.Default, True)
      For Each tAcObjID As ObjectId In colAcObjIDs
         oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, OpenMode.ForWrite)
         oXDataParcel = New DMAcadExt.TplnXDataParcel(oEntity.XData)
         tList = New DMCommon.dmList(oXDataParcel.NeigborList)

         If tList.Exists Then
            For iIndex As Integer = 0 To tList.UpperBound
               If Integer.TryParse(tList.Item(iIndex), iPgonID) Then
                  If Not mcolNeighbors.Contains(iPgonID) Then
                     mcolNeighbors.Add(iPgonID)
                  End If
               End If
            Next
         End If


         If mdicSetCentroids.TryGetValue(oXDataParcel.ID, tCentroidObjID) Then
            DMAcadExt.AcadDocument.WriteMessage("+IDParcel: " & CStr(oXDataParcel.ID) & "," & tCentroidObjID.ToString())
            tPoint3d = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(tCentroidObjID)
            Try
               oTopoPgon = oTopoModel.FindPolygon(tPoint3d)
            Catch oMapEx As MapException
               If oMapEx.ErrorCode <> 3 Then
                  System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & oTopoModel.Name & vbCrLf & tPoint3d.ToString(), "01_876w")
               End If

            End Try

            If oTopoPgon Is Nothing Then
               oEntity.Layer = sAddLayerName
            Else
               oEntity.Layer = "TplnParcelErased"
            End If
         Else
            DMAcadExt.AcadDocument.WriteMessage("-IDParcel: " & CStr(oXDataParcel.ID))
         End If
      Next
      oTopoModel.Close()
      For Each iID As Integer In mcolNeighbors
         If mdicPolygons.TryGetValue(iID, tPolylineObjID) Then
            oEntity = DMAcadExt.AcadTransaction.GetEntity(tPolylineObjID, OpenMode.ForWrite)
            oEntity.Layer = sAddLayerName
         End If
      Next
   End Sub
   Private Function zzLongToAcObjID(oValue As System.Object) As ObjectId
      Dim lAcObjID As Long = DirectCast(oValue, Long)
      Return New ObjectId(New System.IntPtr(lAcObjID))
   End Function
   Private Function zzToAcObjID(oValue As System.Object) As ObjectId
      Return DirectCast(oValue, ObjectId)
   End Function
   Private Function zzStrToAcObjID(sValue As String) As ObjectId
      Dim lAcObjID As Long = Convert.ToInt64(sValue)
      Return New ObjectId(New System.IntPtr(lAcObjID))
   End Function
   Private Function zzStringToInt(sAtributeTextStr As String) As System.Object

      If Not String.IsNullOrEmpty(sAtributeTextStr) Then
         Dim iResult As Integer
         If Integer.TryParse(sAtributeTextStr, iResult) Then
            Return iResult
         Else
            Return -1
         End If
      Else
         Return DBNull.Value
      End If
   End Function
   Private Function zzStringToDouble(sAtributeTextStr As String) As System.Object

      If Not String.IsNullOrEmpty(sAtributeTextStr) Then
         Dim dResult As Double
         If Double.TryParse(sAtributeTextStr, dResult) Then
            Return dResult
         Else
            Return -1.0
         End If
      Else
         Return DBNull.Value
      End If
   End Function


   Private Structure TopoHalfEdge
      Dim StartVertex As Integer
      Dim StartPoint As Point3d
      Dim EndVertex As Integer
      Dim EndPoint As Point3d
   End Structure
   Private Structure TopoHalfEdges
      Dim AdjacentID As Integer
      Dim Edges() As TopoHalfEdge
   End Structure
   Private Class MPgonsByGush
      Private mdicGushObjIDCollections As IDictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      Private macolMPgonObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Public Sub New()
         mdicGushObjIDCollections = New Dictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)()
      End Sub
      Public Sub Add(iGushID As Integer, tMPgonAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         Dim colGush As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         If mdicGushObjIDCollections.ContainsKey(iGushID) Then
            colGush = mdicGushObjIDCollections.Item(iGushID)
         Else
            colGush = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
            mdicGushObjIDCollections.Add(iGushID, colGush)
         End If
         colGush.Add(tMPgonAcObjID)
      End Sub
      Public ReadOnly Property Count As Integer
         Get
            If mdicGushObjIDCollections IsNot Nothing Then
               Return mdicGushObjIDCollections.Count
            Else
               Return -1
            End If
         End Get
      End Property
      Public Function GetMPgonColByGush() As IDictionary(Of Integer, Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)

         Return mdicGushObjIDCollections
      End Function
   End Class
   Private Class LinkTable
      Inherits System.Data.DataTable
      Const msAcObjIDFieldName As String = "AcObjID"
      Const msEntityTypeFieldName As String = "Type"
      Const msStatusFieldName As String = "Status"
      Const msXminFieldName As String = "Xmin"
      Const msXmaxFieldName As String = "Xmax"
      Const msYminFieldName As String = "Ymin"
      Const msYmaxFieldName As String = "Xmax"
      Public Sub New()
         MyBase.New("Links")
      End Sub
      Private Sub zzCreateLinkTable()

         With MyBase.Columns
            .Add(msAcObjIDFieldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
            .Add(msStatusFieldName, GetType(System.Boolean))
            .Add(msXminFieldName, GetType(System.Double))
            .Add(msXmaxFieldName, GetType(System.Double))
            .Add(msYminFieldName, GetType(System.Double))
            .Add(msYmaxFieldName, GetType(System.Double))

         End With
      End Sub
      Public Sub AddLink(oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline)
         Dim oNewRow As DataRow
         Dim oExtents3d As Autodesk.AutoCAD.DatabaseServices.Extents3d
         oExtents3d = oPolyline.GeometricExtents
         oNewRow = MyBase.NewRow()
         With oNewRow
            .Item(msAcObjIDFieldName) = oPolyline.ObjectId
            .Item(msStatusFieldName) = False

            .Item(msXminFieldName) = oExtents3d.MinPoint.X
            .Item(msXmaxFieldName) = oExtents3d.MaxPoint.X
            .Item(msYminFieldName) = oExtents3d.MinPoint.Y
            .Item(msYmaxFieldName) = oExtents3d.MaxPoint.Y


         End With
         MyBase.Rows.Add(oNewRow)

      End Sub
      Public Sub GetCrossesBoundingBox(ByRef tFirstAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, ByRef taObjectIds() As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         Dim oDataRow As DataRow = MyBase.Rows.Item(0)
         Dim sXmin As String = oDataRow.Item(msXminFieldName).ToString()
         Dim sXmax As String = oDataRow.Item(msXmaxFieldName).ToString()
         Dim sYmin As String = oDataRow.Item(msYminFieldName).ToString()
         Dim sYmax As String = oDataRow.Item(msYmaxFieldName).ToString()
         tFirstAcObjID = DirectCast(oDataRow.Item(msYmaxFieldName), Autodesk.AutoCAD.DatabaseServices.ObjectId)
         MyBase.Rows.RemoveAt(0)
         Dim sRowFilter As String = "(" & msXmaxFieldName & " >" & sXmin & ") AND (" & msXminFieldName & " <" & sXmax & ") AND (" & msYmaxFieldName & ">" & sYmin & ") AND (" & msYminFieldName & "<" & sYmax & ")"
         Dim oDataView As DataView = New DataView(Me)
         oDataView.RowFilter = sRowFilter
         Dim iResUB As Integer = oDataView.Count - 1
         If iResUB >= 0 Then
            ReDim taObjectIds(iResUB)
            For iIndex As Integer = 0 To iResUB
               taObjectIds(iIndex) = DirectCast(oDataView.Item(iIndex).Item(msAcObjIDFieldName), Autodesk.AutoCAD.DatabaseServices.ObjectId)
            Next
         End If
      End Sub
   End Class
   Private Class CentroidTable
      Inherits System.Data.DataTable
      Const msAcObjIDFieldName As String = "AcObjID"
      Const msStatusFieldName As String = "Status"
      Const msXFieldName As String = "X"
      Const msYFieldName As String = "Y"
      Public Sub New()
         MyBase.New("Links")
      End Sub

      Public Function GetCrossesBoundingBox(ByRef tExtents3d As Autodesk.AutoCAD.DatabaseServices.Extents3d) As Autodesk.AutoCAD.DatabaseServices.ObjectId()
         Dim sXmin As String = tExtents3d.MinPoint.X.ToString()
         Dim sXmax As String = tExtents3d.MaxPoint.X.ToString()
         Dim sYmin As String = tExtents3d.MinPoint.Y.ToString()
         Dim sYmax As String = tExtents3d.MaxPoint.Y.ToString()

         Dim sRowFilter As String = "(" & msXFieldName & " >" & sXmin & ") AND (" & msXFieldName & " <" & sXmax & ") AND (" & msYFieldName & ">" & sYmin & ") AND (" & msYFieldName & "<" & sYmax & ")"
         Dim oDataView As DataView = New DataView(Me)
         oDataView.RowFilter = sRowFilter
         Dim iResUB As Integer = oDataView.Count - 1
         If iResUB >= 0 Then
            Dim taObjectIds(iResUB) As Autodesk.AutoCAD.DatabaseServices.ObjectId
            For iIndex As Integer = 0 To iResUB
               taObjectIds(iIndex) = DirectCast(oDataView.Item(iIndex).Item(msAcObjIDFieldName), Autodesk.AutoCAD.DatabaseServices.ObjectId)
            Next
            Return taObjectIds
         Else
            Return Nothing
         End If

      End Function
      Public Sub AddCentroid(oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference)
         Dim oNewRow As DataRow
         Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = oBlockRef.Position
         oNewRow = MyBase.NewRow()
         With oNewRow
            .Item(msAcObjIDFieldName) = oBlockRef.ObjectId
            .Item(msStatusFieldName) = False

            .Item(msXFieldName) = tInsertPoint.X
            .Item(msYFieldName) = tInsertPoint.Y



         End With
         MyBase.Rows.Add(oNewRow)

      End Sub

      Private Sub zzCreateCentroidTable()
         Dim moCentroids As System.Data.DataTable = New System.Data.DataTable("Centroids")
         With moCentroids.Columns
            .Add(msAcObjIDFieldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
            .Add(msStatusFieldName, GetType(System.Boolean))
            .Add(msXFieldName, GetType(System.Double))
            .Add(msYFieldName, GetType(System.Double))

         End With
      End Sub
   End Class
   Private Class MPgonCentroid
      Public MPgon As MPolygon
      Public Centroid As BlockReference
      Public Sub New(oMPgon As MPolygon)
         MPgon = oMPgon
      End Sub
   End Class
   Public Class FindCenter
      Const mdToler = 0.001
      Const miMaxIteration As Integer = 2
      Private moMPgon As MPolygon
      Private moExtents As DMAcadExt.TPlnBoundingBox
      Private miExteriorIndex As Integer
      Public Sub New(oMPgon As MPolygon)
         Dim oExtents3d As Extents3d
         moMPgon = oMPgon
         oExtents3d = moMPgon.GeometricExtents
         moExtents = New DMAcadExt.TPlnBoundingBox(oExtents3d)
         For iIndex As Integer = 0 To moMPgon.NumMPolygonLoops - 1
            If moMPgon.GetLoopDirection(iIndex) = LoopDirection.Exterior Then
               miExteriorIndex = iIndex
               Exit Sub
            End If
         Next
      End Sub
      Public Sub MoveBlockRef(ByRef oBlockRef As BlockReference, sOkLabel As String)
         Dim oPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBlockRef.Position)
         Dim oPgonCenter As DMAcadExt.TPlnPoint
         Dim tVector As Vector3d
         Dim tMatrix As Matrix3d
         '  DMAcadExt.AcadDocument.WriteDebugMessage("!--!!!!!!!!!!!!!!!!!!!!!!!!!: " & oPoint.Coordinates.ToString())
         If Not zzCheckPoint(oPoint) Then
            DMAcadExt.AcadDocument.WriteDebugMessage("!--!Out: " & oPoint.Coordinates.ToString())
            oPgonCenter = Me.GetInnerPoint()
            If oPgonCenter IsNot Nothing Then
               DMAcadExt.AcadDocument.WriteDebugMessage("!--!New PgonCenter: " & oPgonCenter.Coordinates.ToString())
               tVector = oPoint.GetSubstract(oPgonCenter)
               tMatrix = Matrix3d.Displacement(tVector)
               ' oBlockRef.Position = oPgonCenter.GetPoint3d()
               oBlockRef.TransformBy(tMatrix)
               DMAcadExt.AcadDocument.WriteDebugMessage("!--!After: " & oBlockRef.Position.ToString())
            Else
               DMAcadExt.AcadDocument.WriteDebugMessage("!--!Failed : ")
            End If
         ElseIf Not String.IsNullOrEmpty(sOkLabel) Then

            DMAcadExt.AcadDocument.WriteDebugMessage("!--!OK: " & sOkLabel)
         End If
      End Sub

      Public Function GetInnerPoint() As DMAcadExt.TPlnPoint
         Dim oCenterPoint As DMAcadExt.TPlnPoint = moExtents.GetCenterPoint()
         Dim oaSegments() As Segment
         Dim oaSegmentsNext() As Segment

         Dim miSegmentsCount = 4
         DMAcadExt.AcadDocument.WriteDebugMessage("ExtenCntrPoint:" & oCenterPoint.Coordinates)
         If zzCheckPoint(oCenterPoint) Then
            Return oCenterPoint
         Else
            ReDim oaSegments(miSegmentsCount - 1)
            oaSegments(0) = New Segment(oCenterPoint, New DMAcadExt.TPlnPoint(moExtents.Right, oCenterPoint.Y))
            oaSegments(1) = New Segment(oCenterPoint, New DMAcadExt.TPlnPoint(oCenterPoint.X, moExtents.Top))
            oaSegments(2) = New Segment(oCenterPoint, New DMAcadExt.TPlnPoint(moExtents.Left, oCenterPoint.Y))
            oaSegments(3) = New Segment(oCenterPoint, New DMAcadExt.TPlnPoint(oCenterPoint.X, moExtents.Bottom))
            For iIteration = 0 To miMaxIteration
               For iIndex As Integer = 0 To miSegmentsCount - 1
                  If zzCheckPoint(oaSegments(iIndex).Center) Then
                     Return oaSegments(iIndex).Center
                  End If
               Next
               ReDim oaSegmentsNext(2 * miSegmentsCount)
               For iIndex As Integer = 0 To miSegmentsCount - 1
                  oaSegmentsNext(iIndex) = oaSegments(iIndex).GetSegmentA
               Next
               For iIndex As Integer = 0 To miSegmentsCount - 1
                  oaSegmentsNext(miSegmentsCount + iIndex) = oaSegments(iIndex).GetSegmentB
               Next
               miSegmentsCount += miSegmentsCount
               oaSegments = oaSegmentsNext
            Next
            DMAcadExt.AcadDocument.WriteDebugMessage("Not Success:" & miMaxIteration.ToString())
            Return Nothing
         End If
      End Function
      Private Function zzCheckPoint(oPoint As DMAcadExt.TPlnPoint) As Boolean
         Dim iaLoopInd As IntegerCollection
         iaLoopInd = moMPgon.IsPointInsideMPolygon(oPoint.GetPoint3d(), 0.00001)  'mdToler
         If iaLoopInd IsNot Nothing AndAlso iaLoopInd.Count > 0 Then
            If iaLoopInd.Item(0) = miExteriorIndex Then
               Return True
            End If
         End If
         Return False
      End Function
      Private Structure Segment
         Dim PointA As DMAcadExt.TPlnPoint
         Dim PointB As DMAcadExt.TPlnPoint
         Public Sub New(oPointA As DMAcadExt.TPlnPoint, oPointB As DMAcadExt.TPlnPoint)
            PointA = oPointA
            PointB = oPointB
         End Sub
         Public ReadOnly Property Center As DMAcadExt.TPlnPoint
            Get
               Return New DMAcadExt.TPlnPoint(PointA, PointB)
            End Get
         End Property
         Public Function GetSegmentA() As Segment
            Return New Segment(PointA, Me.Center)
         End Function
         Public Function GetSegmentB() As Segment
            Return New Segment(Me.Center, PointB)
         End Function
      End Structure
   End Class
   Private Structure GroupKey
      Dim GroupID As Integer
      Dim GroupAddID As Integer
      Dim ID As Integer
      Public Sub New(iGroupID As Integer, iID As Integer)
         GroupID = iGroupID
         ID = iID
      End Sub
      Public Sub New(iGroupID As Integer, iGroupAddID As Integer, iID As Integer)
         GroupID = iGroupID
         GroupAddID = iGroupAddID
         ID = iID
      End Sub
      Public Overrides Function ToString() As String
         If GroupAddID = 0 Then
            Return Convert.ToString(GroupID) & "," & Convert.ToString(ID)
         Else
            Return Convert.ToString(GroupID) & "\" & Convert.ToString(GroupAddID) & "," & Convert.ToString(ID)
         End If

      End Function
      Public Shared Operator =(tGroupKeyA As GroupKey, tGroupKeyB As GroupKey) As Boolean
         Return (tGroupKeyA.GroupID = tGroupKeyA.GroupID) AndAlso (tGroupKeyA.GroupAddID = tGroupKeyA.GroupAddID) AndAlso (tGroupKeyA.ID = tGroupKeyA.ID)
      End Operator
      Public Shared Operator <>(tGroupKeyA As GroupKey, tGroupKeyB As GroupKey) As Boolean
         Return Not (tGroupKeyA = tGroupKeyB)
      End Operator
   End Structure
End Class

Public Class PgonRelation
   Const mdToler As Double = 0.00001
   Const mbExcludeCrossing As Boolean = False
   Private Const msXDataApp As String = "IntersectPoint"

   Private moPolylineA As Polyline
   Private moPolylineB As Polyline
   Private moRingA As TopoManager.tmRing
   Private moRingB As TopoManager.tmRing
   Private moPointList As IList(Of DMAcadExt.TPlnPoint) = New List(Of DMAcadExt.TPlnPoint)
   Private mcolErrorPoints As Point3dCollection
   Private miRelationType As enRelationType = enRelationType.NotExists
   Private mbIsBalanced As Boolean
   Private mbIsCorrect As Boolean
   Private mbIsTouch As Boolean

   Private msErrorStatus As String
   '	Dim mcolPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
   Private colErrBlockRefs As ObjectIdCollection
   Private mcolIntersectPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
   Private mtStartPoint As Point3d
   Private mtEndPoint As Point3d

   Private Shared mtOctagonAcObjId As ObjectId

   Public Shared Sub RegApp()
      Dim bRegAppOK As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataApp)
   End Sub
   Public Shared Function IntersectPolylines(oPolylineA As Polyline, oPolylineB As Polyline, dTolerance As Double) As Point3dCollection
      Dim colIntersectPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      Dim colResPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      Dim dDist As Double
      Dim tClosestPointOnCurve As Point3d

      oPolylineA.IntersectWith(oPolylineB, Intersect.OnBothOperands, colIntersectPoints, New IntPtr(0), New IntPtr(0))
      DMAcadExt.AcadDocument.WriteDebugMessage("!@@!Inter: " & colIntersectPoints.Count.ToString())
      For Each tPoint As Point3d In colIntersectPoints
         tClosestPointOnCurve = oPolylineA.GetClosestPointTo(tPoint, False)
         dDist = tPoint.DistanceTo(tClosestPointOnCurve)

         If dDist < dTolerance Then
            tClosestPointOnCurve = oPolylineB.GetClosestPointTo(tPoint, False)
            dDist = tPoint.DistanceTo(tClosestPointOnCurve)
            If dDist < dTolerance Then
               colResPoints.Add(tPoint)
            End If
         End If
      Next
      Return colResPoints
   End Function
   Public Sub New(oPolylineA As Polyline, oPolylineB As Polyline)
      moPolylineA = oPolylineA
      moPolylineB = oPolylineB

   End Sub
   Public Sub New(oRingA As TopoManager.tmRing, oRingB As TopoManager.tmRing)
      moPolylineA = oRingA.Polyline
      moPolylineB = oRingB.Polyline
      moRingA = oRingA
      moRingB = oRingB

   End Sub

   Public Sub Calculate(bCheckIntersection As Boolean, bPrintMessages As Boolean)
      Dim oMPolygon As MPolygon
      '    Dim oMPolygonTest As MPolygon

      Dim oMPgonLoop0 As MPolygonLoop
      Dim oMPgonLoop1 As MPolygonLoop

      Dim bHasInterior As Boolean
      Dim bCrossed1, bCrossed2 As Boolean
      '   Dim bTouch As Boolean
      Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      Dim iDir0, iDir1 As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      Dim bReverse As Boolean = False
      Dim iLoopNumber As Integer
      colPoints = New Autodesk.AutoCAD.Geometry.Point3dCollection()


      If moPolylineA.Closed AndAlso moPolylineB.Closed Then
         oMPolygon = zzGetBalancedMPgon(bReverse)
         '   oMPolygonTest = zzGetBalancedMPgon(Not bReverse)
         If oMPolygon Is Nothing Then
            bReverse = True
            oMPolygon = zzGetBalancedMPgon(bReverse)
         End If


         If oMPolygon IsNot Nothing Then

            iDir0 = oMPolygon.GetLoopDirection(0)
            iDir1 = oMPolygon.GetLoopDirection(1)

            If bPrintMessages Then

               DMAcadExt.AcadDocument.WriteMessage("Loops: " & iDir0.ToString() & "," & iDir1.ToString() & " Rev=" & CStr(bReverse))
               '  DMAcadExt.AcadDocument.WriteMessage("LoopsT: " & oMPolygonTest.GetLoopDirection(0).ToString() & "," & oMPolygonTest.GetLoopDirection(1).ToString() & " Rev=" & CStr(Not bReverse))


               '	Dim oLoop As MPolygonLoop = oMPolygon.GetMPolygonLoopAt(0)
               '	zzDrawLoop(oLoop)
            End If

            '   System.Windows.Forms.MessageBox.Show(CStr(113), "03_925")

            bHasInterior = (iDir0 = LoopDirection.Interior) OrElse (iDir1 = LoopDirection.Interior)
            oMPgonLoop0 = oMPolygon.GetMPolygonLoopAt(0)

            bCrossed1 = oMPolygon.LoopCrossesMPolygon(oMPgonLoop0, mdToler)
            oMPgonLoop1 = oMPolygon.GetMPolygonLoopAt(1)
            bCrossed2 = oMPolygon.LoopCrossesMPolygon(oMPgonLoop1, mdToler)
            mbIsTouch = oMPolygon.IncludesTouchingLoops(mdToler)

            If bHasInterior Then
               iLoopNumber = zzGetLoopVertNumber(oMPgonLoop0)
               If iLoopNumber = -1 Then
                  iLoopNumber = 1 - zzGetLoopVertNumber(oMPgonLoop1)
               End If
               If iLoopNumber = 0 Then
                  If iDir0 = LoopDirection.Interior Then
                     miRelationType = enRelationType.Interior
                  ElseIf iDir1 = LoopDirection.Interior Then
                     miRelationType = enRelationType.Exterior
                  End If
               ElseIf iLoopNumber = 1 Then
                  If iDir0 = LoopDirection.Interior Then
                     miRelationType = enRelationType.Exterior
                  ElseIf iDir1 = LoopDirection.Interior Then
                     miRelationType = enRelationType.Interior
                  End If
               Else
                  DMAcadExt.AcadDocument.WriteMessageLog("System Err #3427; " & CStr(iLoopNumber) & "; OK-" & CStr(oMPolygon.IsBalanced) & "; " & moPolylineA.Handle.ToString & ":" & moPolylineB.Handle.ToString)
               End If
            End If
            If bPrintMessages Then
               DMAcadExt.AcadDocument.WriteMessage("Crossed: " & bCrossed1.ToString() & "," & bCrossed2.ToString() & "; Touch: " & mbIsTouch.ToString() & "; Interior: " & bHasInterior.ToString())
            End If
         End If

         If bCheckIntersection AndAlso (bCrossed1 OrElse bCrossed2 OrElse Not mbIsBalanced) Then
            'Err
            '   moPolylineA.IntersectWith(moPolylineB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
            colPoints = IntersectPolylines(moPolylineA, moPolylineB, 0.00001)
            If colPoints.Count <> 0 Then
               miRelationType = enRelationType.Neigbour
               mbIsCorrect = True
               mcolIntersectPoints = colPoints
               If bPrintMessages Then
                  DMAcadExt.AcadDocument.WriteMessage("IntersectWith " & CStr(colPoints.Count))
               End If

            ElseIf mbIsBalanced Then
               mbIsCorrect = True
            Else
               mbIsCorrect = False

            End If

            '	DMAcadExt.AcadDocument.WriteMessage("bCrossed OrElse Not mbIsBalanced " & CStr(bCrossed1) & "," & CStr(bCrossed2))
         ElseIf bHasInterior Then
            ''
            moPolylineA.IntersectWith(moPolylineB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
            '   DMAcadExt.AcadDocument.WriteMessage("Int IntW points: " & CStr(colPoints.Count) & " Dir0=" & iDir0.ToString() & " Dir1=" & iDir1.ToString())
            If colPoints.Count = 0 Then
               'Island
               mbIsCorrect = True

            Else
               'Err
               '  moPolylineA.
               mcolIntersectPoints = colPoints
               msErrorStatus = "-Island is incorrect "
               mbIsCorrect = False
               miRelationType = enRelationType.Neigbour
            End If
         ElseIf bCheckIntersection Then
            mbIsCorrect = True

            mbIsTouch = oMPolygon.IncludesTouchingLoops(mdToler)
            If mbIsTouch Then
               miRelationType = enRelationType.Neigbour
               'Neigb

               moPolylineA.IntersectWith(moPolylineB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
               colPoints = IntersectPolylines(moPolylineA, moPolylineB, 0.00001)
               '  DMAcadExt.AcadDocument.WriteMessage("Out IntW points: " & CStr(colPoints.Count))
               If colPoints.Count > 0 Then
                  mcolIntersectPoints = colPoints
               End If
            Else
               'CLUM
               miRelationType = enRelationType.NotExists
            End If
         Else
            'CLUM
            miRelationType = enRelationType.NotExists
         End If

      Else
         DMAcadExt.AcadDocument.WriteMessageLog("Polygon was not found")
      End If
   End Sub
   Public Sub OpenLinesForWrite()

      moPolylineA = DMAcadExt.AcadTransaction.GetPolyline(moPolylineA.ObjectId, OpenMode.ForWrite)
      moPolylineB = DMAcadExt.AcadTransaction.GetPolyline(moPolylineB.ObjectId, OpenMode.ForWrite)

   End Sub
   Public Sub MarkIntersectionPoints(bDBPoint As Boolean)
      Dim iIndex As Integer = 0
      Dim iLastIndex As Integer = mcolIntersectPoints.Count - 1
      Dim shColorIndex As Short
      Dim oSaltireMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)

      ' System.Windows.Forms.MessageBox.Show("!!!!" & CStr(mcolIntersectPoints.Count), "05_117")
      For Each tPoint As Point3d In mcolIntersectPoints
         If iIndex = 0 OrElse iIndex = iLastIndex Then
            shColorIndex = 4S
         Else
            shColorIndex = 4S
         End If
         If bDBPoint Then
            DMAcadExt.AcadTransaction.InsertPoint(tPoint, , 2)
         Else
            oSaltireMarkBlock.MarkPoint(tPoint, shColorIndex)
         End If

         iIndex += 1
      Next

   End Sub
   Public Function CorrectBetween(tStartPoint As Point3d, tEndPoint As Point3d) As Integer
      TopoManager.tmVertices.SetTolerance(0.000001)
      If mcolIntersectPoints.Contains(tStartPoint) Then
         mtStartPoint = tStartPoint
      Else
         DMAcadExt.AcadDocument.WriteMessage("Start Select failed: " & tStartPoint.ToString())
         ' Return 1
      End If
      If mcolIntersectPoints.Contains(tEndPoint) Then
         mtEndPoint = tEndPoint
      Else
         DMAcadExt.AcadDocument.WriteMessage("End Select failed: " & tEndPoint.ToString())
         ' Return 2
      End If
      'Dim bVertexAdded As Boolean
      'Dim iStartAIndex As Integer = zzSelectVertex(moPolylineA, tStartPoint, bVertexAdded)
      'Dim iEndAIndex As Integer = zzSelectVertex(moPolylineA, tEndPoint, bVertexAdded)
      'If bVertexAdded AndAlso (iEndAIndex <= iStartAIndex) Then
      '   iStartAIndex += 1
      'End If
      Dim iStartAIndex As Integer
      Dim iEndAIndex As Integer
      Dim iStartBIndex As Integer ' = zzSelectVertex(moPolylineB, tStartPoint)
      Dim iEndBIndex As Integer ' = zzSelectVertex(moPolylineB, tEndPoint)
      zzSelectVertexPair(moPolylineA, tStartPoint, tEndPoint, iStartAIndex, iEndAIndex)
      zzSelectVertexPair(moPolylineB, tStartPoint, tEndPoint, iStartBIndex, iEndBIndex)


      moRingA = New TopoManager.tmRing(0, moPolylineA)
      moRingB = New TopoManager.tmRing(0, moPolylineB)

      Dim dMaxDist As Double = 0.0
      Dim iMaxDistIndex As Integer
      Dim dDist As Double
      Dim tVertexA As Point3d
      Dim tPointOnPline As Point3d

      For iIndex As Integer = 0 To moPolylineA.NumberOfVertices - 1
         tVertexA = moPolylineA.GetPoint3dAt(iIndex)
         tPointOnPline = moPolylineB.GetClosestPointTo(tVertexA, False)
         dDist = tVertexA.DistanceTo(tPointOnPline)
         If dDist > dMaxDist Then
            dMaxDist = dDist
            iMaxDistIndex = iIndex
         End If
      Next
      Dim bRingADir As Boolean = moRingA.Vertices.Direction
      Dim bRingBDir As Boolean = moRingB.Vertices.Direction
      '  TplnPolygonSet.vb() : Line 5224
      Dim bDirA As Boolean = Not moRingA.Vertices.GetDir(iStartAIndex, iMaxDistIndex, iEndAIndex)
      Dim bDirB As Boolean = (Not (bRingADir Xor bRingBDir) Xor bDirA)
      DMAcadExt.AcadDocument.WriteMessage("bRingADir=" & bRingADir.ToString() & vbCrLf & "bRingBDir=" & bRingBDir.ToString() & vbCrLf & vbCrLf & "bDirA=" & bDirA.ToString() & vbCrLf & vbCrLf & "bDirB=" & bDirB.ToString())
      ' System.Windows.Forms.MessageBox.Show("StartAIndex=" & CStr(iStartAIndex) & vbCrLf & "MaxDistIndex=" & CStr(iMaxDistIndex) & vbCrLf & "EndAIndex=" & CStr(iEndAIndex) & vbCrLf & CStr(bDirA) & vbCrLf & CStr(bDirB), "05_117")
      DMAcadExt.AcadDocument.WriteMessage("StartAIndex=" & CStr(iStartAIndex) & vbCrLf & "MaxDistIndex=" & CStr(iMaxDistIndex) & vbCrLf & "EndAIndex=" & CStr(iEndAIndex) & vbCrLf & "UB=" & moRingA.Vertices.UB.ToString() & vbCrLf & CStr(bDirA) & vbCrLf & CStr(bDirB))
      DMAcadExt.AcadDocument.WriteMessage("StartBIndex=" & CStr(iStartBIndex) & vbCrLf & vbCrLf & "EndBIndex=" & CStr(iEndBIndex))

      moPolylineA.SetPointAt(iStartAIndex, moRingB.Vertices.Vertex(iStartBIndex).Point2d)
      moPolylineA.SetPointAt(iEndAIndex, moRingB.Vertices.Vertex(iEndBIndex).Point2d)

      iStartAIndex = moRingA.RemoveVertices(iStartAIndex, iEndAIndex, bDirA)
      DMAcadExt.AcadDocument.WriteMessage("After Remove StartAIndex=" & CStr(iStartAIndex) & "; " & moRingA.Vertices.Vertex(iStartAIndex).ToString())

      Dim iRingAIndex As Integer = iStartAIndex 'moRingA.Vertices.GetNextIndex(iStartAIndex, bDirA)

      Dim iRingBIndex As Integer = iStartBIndex
		'  Dim oVertex As TopoManager.tmVertex
		DMAcadExt.AcadDocument.WriteMessage("After After iRingAIndex=" & CStr(iRingAIndex))
      '  Return 0
      Dim i As Integer = 0
      Do
         iRingAIndex = moRingA.Vertices.GetNextIndexForAdd(iRingAIndex, bDirA)
         iRingBIndex = moRingB.Vertices.GetNextIndex(iRingBIndex, bDirB)

         If i < 8 Or i > 90 Then
            DMAcadExt.AcadDocument.WriteMessage("iRingAIndex=" & CStr(iRingAIndex) & "; iRingBIndex=" & CStr(iRingBIndex))
         End If
         If iRingBIndex = iEndBIndex Then
            DMAcadExt.AcadDocument.WriteMessage("Exit: iRingAIndex=" & CStr(iRingAIndex) & "; iRingBIndex=" & CStr(iRingBIndex))
            Exit Do
         End If
         '   System.Windows.Forms.MessageBox.Show("iRingAIndex=" & CStr(iRingAIndex) & vbCrLf & "iRingBIndex=" & CStr(iRingBIndex), "05_769")
         moPolylineA.AddVertexAt(iRingAIndex, moRingB.Vertices.Vertex(iRingBIndex).Point2d, 0.0, 0.0, 0.0)

         ' iRingAIndex = moRingA.Vertices.GetNextIndex(iRingAIndex, bDirA)
         If bDirA Then
            '  iRingAIndex += 1
         End If
         i += 1
      Loop Until i = 1000
      ' System.Windows.Forms.MessageBox.Show("i=" & CStr(i), "05_119")
      Return 0

   End Function
   Private Sub zzSelectVertexPair(oPolyline As Polyline, tStartPoint As Point3d, tEndPoint As Point3d, ByRef iStartIndex As Integer, ByRef iEndIndex As Integer)
      Dim bVertexAdded As Boolean
      iStartIndex = zzSelectVertex(oPolyline, tStartPoint, bVertexAdded)
      iEndIndex = zzSelectVertex(oPolyline, tEndPoint, bVertexAdded)
      If bVertexAdded AndAlso (iEndIndex <= iStartIndex) Then
         iStartIndex += 1
      End If
   End Sub
   Private Function zzSelectVertex(oPolyline As Polyline, tPoint As Point3d, ByRef bVertexAdded As Boolean) As Integer
      Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
      'Debug  DMAcadExt.AcadTransaction.InsertPoint(tClosestPointOnCurve, , 5)
      Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
      Dim iIndex As Integer = Convert.ToInt32(Math.Floor(dPointParameter))
      Dim iNextIndex As Integer = (iIndex + 1) Mod oPolyline.NumberOfVertices
      Dim tVertex As Point3d = oPolyline.GetPoint3dAt(iIndex)
      If tVertex.DistanceTo(tClosestPointOnCurve) > 0.0005 Then
         DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & ": " & tClosestPointOnCurve.ToString() & "; " & tVertex.ToString())
         oPolyline.AddVertexAt(iIndex + 1, DMAcadExt.TPlnPoint.Point3dTo2d(tClosestPointOnCurve), 0.0, 0.0, 0.0)
         bVertexAdded = True
         Return iIndex + 1
      Else
         bVertexAdded = False
         DMAcadExt.AcadDocument.WriteMessage("!!!" & CStr(iIndex) & ": " & tClosestPointOnCurve.ToString() & "; " & tVertex.ToString())
         Return iIndex
      End If

   End Function
   Public Sub DoIntersection()
		'DMCommon.Debug.MsgBox("!DoIntersection", mcolIntersectPoints, moRingA, moRingB)
		'     DMAcadExt.AcadDocument.WriteLog("Inter. Points:" & CStr(mcolIntersectPoints.Count) & "; Rings- " & CStr(moRingA.ID) & "," & CStr(moRingB.ID))
		If mcolIntersectPoints.Count > 1 Then
         Dim colEdges As System.Collections.ObjectModel.Collection(Of TopoManager.tmEdge) = moRingA.AddIntersectPoints(mcolIntersectPoints, moRingB)
         If False Then
            For Each oEdge As TopoManager.tmEdge In colEdges
               oEdge.DebugWrite(1, "")
               moRingA.CreateDBPolyline(oEdge)
            Next
         End If
      End If
   End Sub
   Private Function zzGetVertexUB(oPolyline As Polyline) As Integer
      Dim iVertUB = oPolyline.NumberOfVertices - 1
      If oPolyline.GetPoint2dAt(0).IsEqualTo(oPolyline.GetPoint2dAt(iVertUB)) Then
         Return iVertUB
      Else
         Return iVertUB + 1
      End If
   End Function
   Private Function zzPolylineToArray(oPolyline As Polyline) As Point2d()
      Dim iVertUB As Integer = oPolyline.NumberOfVertices - 1
      Dim iVertLoopUB As Integer
      Dim dMin As Double = 999999.0
      Dim dDist As Double = 0

      Dim dPrevPoint As Point2d
      Dim bLoopExists As Boolean
      If oPolyline.GetPoint2dAt(0).IsEqualTo(oPolyline.GetPoint2dAt(iVertUB)) Then
         bLoopExists = True
         iVertLoopUB = iVertUB
      Else
         bLoopExists = False
         iVertLoopUB = iVertUB + 1
         dPrevPoint = oPolyline.GetPoint2dAt(iVertUB)
         dMin = oPolyline.GetPoint2dAt(0).GetDistanceTo(oPolyline.GetPoint2dAt(iVertUB))
      End If
      Dim iaResPoints(iVertLoopUB) As Point2d
      For iIndex As Integer = 0 To iVertLoopUB - 1
         iaResPoints(iIndex) = oPolyline.GetPoint2dAt(iIndex)
         dDist = dPrevPoint.GetDistanceTo(iaResPoints(iIndex))
         If dDist < dMin Then
            dMin = dDist
         End If
         dPrevPoint = iaResPoints(iIndex)
      Next
      iaResPoints(iVertLoopUB) = oPolyline.GetPoint2dAt(0)
      DMAcadExt.AcadDocument.WriteDebugMessage("Dist Min= " & CStr(dMin))
      Return iaResPoints
   End Function
   Private Function zzGetLoopVertNumber(oMPolygonLoop As MPolygonLoop) As Integer
      '  Dim iLoopCount As Integer = oMPolygonLoop.Count
      Dim iLoopUB As Integer = oMPolygonLoop.Count - 1
      Dim iaResPointsA() As Point2d = zzPolylineToArray(moPolylineA)
      Dim iaResPointsB() As Point2d = zzPolylineToArray(moPolylineB)

      Dim iVertA_UB As Integer = iaResPointsA.GetUpperBound(0)
      Dim iVertB_UB As Integer = iaResPointsB.GetUpperBound(0)
      '  System.Windows.Forms.MessageBox.Show(CStr(117), "03_925")

      Dim iRes As Integer
      ' DMAcadExt.AcadDocument.WriteMessage("Loop= " & CStr(iLoopUB) & ";" & "; A<=>B " & CStr(iVertA_UB) & "<=>" & CStr(iVertB_UB))
      If iVertA_UB = iVertB_UB Then
         If iLoopUB = iVertA_UB Then
            For iIndex As Integer = 0 To iLoopUB - 1
               iRes = 0
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(iaResPointsA(iIndex)) OrElse oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(iaResPointsA(iLoopUB - iIndex)) Then
                  iRes += 1
               End If
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(iaResPointsB(iIndex)) OrElse oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(iaResPointsB(iLoopUB - iIndex)) Then
                  iRes += 2
               End If
               If iRes = 1 Then
                  Return 0
               End If
               If iRes = 2 Then
                  Return 1
               End If
            Next
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3416; " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
            For iIndex As Integer = 0 To iLoopUB - 1
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!a " & oMPolygonLoop.Item(iIndex).Vertex.ToString())
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!b " & moPolylineA.GetPoint2dAt(iIndex).ToString() & "; " & moPolylineA.GetPoint2dAt(iLoopUB - iIndex - 1).ToString())
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!c " & moPolylineB.GetPoint2dAt(iIndex).ToString() & "; " & moPolylineB.GetPoint2dAt(iLoopUB - iIndex - 1).ToString())
            Next

            Return -1
         Else
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3418; " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
            Return -1
         End If
      ElseIf iLoopUB = iVertA_UB Then
         Return 0
      ElseIf iLoopUB = iVertB_UB Then
         Return 1
      Else
         DMAcadExt.AcadDocument.WriteMessageLog("System Err #3422A; " & moPolylineA.Handle.ToString() & "," & moPolylineB.Handle.ToString & " " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
         '   moPolylineA.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 1S)
         '  moPolylineB.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 2S)
         ' moPolylineA.Layer = "0"


         Return -1
      End If
   End Function

   Private Function zzGetLoopVertNumber030316(oMPolygonLoop As MPolygonLoop) As Integer
      '  Dim iLoopCount As Integer = oMPolygonLoop.Count
      Dim iLoopUB As Integer = oMPolygonLoop.Count - 1
      Dim iVertA_UB As Integer = zzGetVertexUB(moPolylineA)
      Dim iVertB_UB As Integer = zzGetVertexUB(moPolylineB)


      Dim iRes As Integer
      DMAcadExt.AcadDocument.WriteMessageLog("Loop= " & CStr(iLoopUB) & ";" & "; A<=>B " & CStr(iVertA_UB) & "<=>" & CStr(iVertB_UB))
      If iVertA_UB = iVertB_UB Then
         If iLoopUB = iVertA_UB Then
            For iIndex As Integer = 0 To iLoopUB - 1
               iRes = 0
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(moPolylineA.GetPoint2dAt(iIndex)) OrElse oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(moPolylineA.GetPoint2dAt(iIndex)) Then
                  iRes += 1
               End If
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(moPolylineB.GetPoint2dAt(iIndex)) Then
                  iRes += 2
               End If
               If iRes = 1 Then
                  Return 0
               End If
               If iRes = 2 Then
                  Return 1
               End If
            Next
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3416; " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
            For iIndex As Integer = 0 To iLoopUB - 1
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!a " & oMPolygonLoop.Item(iIndex).Vertex.ToString())
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!b " & moPolylineA.GetPoint2dAt(iIndex).ToString())
               DMAcadExt.AcadDocument.WriteMessageLog(iIndex.ToString() & "!!c " & moPolylineB.GetPoint2dAt(iIndex).ToString())

            Next

            Return -1
         Else
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3418; " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
            Return -1
         End If
      ElseIf iLoopUB = iVertA_UB Then
         Return 0
      ElseIf iLoopUB = iVertB_UB Then
         Return 1
      Else
         DMAcadExt.AcadDocument.WriteMessageLog("System Err #3422; " & CStr(iLoopUB) & "; <> " & CStr(iVertA_UB) & "; <> " & CStr(iVertB_UB))
         Return -1
      End If
   End Function
   Private Function zzGetLoopVertNumberOld(oMPolygonLoop As MPolygonLoop) As Integer
      Dim iLoopCount As Integer = oMPolygonLoop.Count
      Dim iLoopUB As Integer = oMPolygonLoop.Count - 1

      Dim iRes As Integer
      DMAcadExt.AcadDocument.WriteMessageLog("Loop= " & CStr(iLoopCount) & ";" & "; A<=>B " & CStr(moPolylineA.NumberOfVertices) & "<=>" & CStr(moPolylineB.NumberOfVertices))
      If moPolylineA.NumberOfVertices = moPolylineB.NumberOfVertices Then
         If iLoopCount = moPolylineA.NumberOfVertices Then
            For iIndex As Integer = 0 To iLoopCount - 1
               iRes = 0
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(moPolylineA.GetPoint2dAt(iIndex)) Then
                  iRes += 1
               End If
               If oMPolygonLoop.Item(iIndex).Vertex.IsEqualTo(moPolylineB.GetPoint2dAt(iIndex)) Then
                  iRes += 2
               End If
               If iRes = 1 Then
                  Return 0
               End If
               If iRes = 2 Then
                  Return 1
               End If
            Next
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3416; " & CStr(iLoopCount) & "; <> " & CStr(moPolylineA.NumberOfVertices))
            Return -1
         Else
            DMAcadExt.AcadDocument.WriteMessageLog("System Err #3418; " & CStr(iLoopCount) & "; <> " & CStr(moPolylineA.NumberOfVertices))
            Return -1
         End If
      ElseIf iLoopCount = moPolylineA.NumberOfVertices Then
         Return 0
      ElseIf iLoopCount = moPolylineB.NumberOfVertices Then
         Return 1
      Else
         DMAcadExt.AcadDocument.WriteMessageLog("System Err #3422; " & CStr(iLoopCount) & "; <> " & CStr(moPolylineA.NumberOfVertices) & "; <> " & CStr(moPolylineB.NumberOfVertices))
         Return -1
      End If
   End Function


   Private Function zzGetBalancedMPgon(bReverse As Boolean) As MPolygon
      Dim bBalanceSuccess As Boolean
      Dim oMPolygon As MPolygon = New MPolygon()

      Try
         If bReverse Then
            oMPolygon.AppendLoopFromBoundary(moPolylineB, mbExcludeCrossing, mdToler)
            oMPolygon.AppendLoopFromBoundary(moPolylineA, mbExcludeCrossing, mdToler)
         Else
            oMPolygon.AppendLoopFromBoundary(moPolylineA, mbExcludeCrossing, mdToler)
            oMPolygon.AppendLoopFromBoundary(moPolylineB, mbExcludeCrossing, mdToler)
         End If

         oMPolygon.BalanceTree()
         If oMPolygon.IsBalanced AndAlso oMPolygon.NumMPolygonLoops = 2 Then
            bBalanceSuccess = True
         Else
            bBalanceSuccess = False
         End If

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         bBalanceSuccess = False
         DMAcadExt.AcadDocument.WriteMessage("MPgob Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
         '	DMAcadExt.AcadDocument.WriteMessage(moPolyline.Handle.ToString() & ":" & moPolylineA.Handle.ToString() & " MPgon Err: " & oAcadEx.ErrorStatus.ToString())
         msErrorStatus = oAcadEx.ErrorStatus.ToString()
         '	moPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Yellow)
         '	moPolylineA.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Yellow)
      End Try
      If bBalanceSuccess Then
         mbIsBalanced = True
         Return oMPolygon
      Else
         Return Nothing
      End If

   End Function
   Public ReadOnly Property RelationType As enRelationType
      Get
         Return miRelationType
      End Get
   End Property
   Public ReadOnly Property HasIsland As Boolean
      Get
         Return (miRelationType = enRelationType.Exterior) OrElse (miRelationType = enRelationType.Interior)
      End Get
   End Property
   Public ReadOnly Property IsCorrect As Boolean
      Get
         Return mbIsCorrect
      End Get
   End Property
   Public ReadOnly Property IsBalanced As Boolean
      Get
         Return mbIsBalanced
      End Get
   End Property

   Public ReadOnly Property ErrorPointsCount As Integer
      Get
         Return moPointList.Count
      End Get
   End Property
   Public ReadOnly Property ErrorPoint(iIndex As Integer) As DMAcadExt.TPlnPoint
      Get
         Return moPointList.Item(iIndex)
      End Get
   End Property
   Public ReadOnly Property ErrorStatus() As String
      Get
         If msErrorStatus Is Nothing Then
            Return String.Empty
         Else
            Return msErrorStatus
         End If

      End Get
   End Property
   Public ReadOnly Property IntersectPoints As Point3dCollection
      Get
         Return mcolIntersectPoints
      End Get
   End Property
   Public ReadOnly Property IntersectPointCount() As Integer
      Get
         If mcolIntersectPoints Is Nothing Then
            Return 0
         Else
            Return mcolIntersectPoints.Count
         End If

      End Get
   End Property
   Public Function GetIntersectBoundingBox() As DMAcadExt.TPlnBoundingBox
      If mcolIntersectPoints IsNot Nothing AndAlso mcolIntersectPoints.Count > 0 Then
         Dim oBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox()
         For Each tPoint As Point3d In mcolIntersectPoints
            oBox.Union(New DMAcadExt.TPlnPoint(tPoint))
         Next
         Return oBox
      Else
         Return Nothing
      End If
   End Function
   Public Sub MarkIntersectPoints()
      If mcolIntersectPoints IsNot Nothing AndAlso mcolIntersectPoints.Count > 0 Then
         Dim oBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox()
         Dim oSaltireMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)
         For Each tPoint As Point3d In mcolIntersectPoints
            oSaltireMarkBlock.MarkPoint(tPoint, 3S)
         Next

      End If
   End Sub

   Public ReadOnly Property IsTouch() As Boolean
      Get
         Return mbIsTouch
      End Get
   End Property

   Public Sub PrintIntersectPoints(iLineID As Integer, iCheckLine As Integer)
      Const sIntersect As String = " Number of Intersect points: "
      Const sTouch As String = " Number of Touch points: "
      Dim iIndex As Integer = 0
      Dim sMsg As String
      If mcolIntersectPoints IsNot Nothing Then
         If mbIsTouch Then
            sMsg = sTouch
         Else
            sMsg = sIntersect
         End If
         Dim iMinIndex, iMaxIndex As Integer
         If mcolIntersectPoints.Count <= 12 Then
            iMinIndex = mcolIntersectPoints.Count - 1
         Else
            iMinIndex = 7
            iMaxIndex = mcolIntersectPoints.Count - 4
         End If
         For Each tPoint As Point3d In mcolIntersectPoints
            DMAcadExt.AcadTransaction.InsertPoint(tPoint, zzGetNumResBuffer(iIndex, iLineID, iCheckLine))

            If iIndex <= iMinIndex OrElse iIndex >= iMaxIndex Then
               DMAcadExt.AcadDocument.WriteMessage(CStr(iLineID) & "\" & CStr(iCheckLine) & " pt #" & CStr(iIndex) & " " & DMAcadExt.TPlnPoint.DispPoint(tPoint))
            End If


            iIndex += 1
         Next
         DMAcadExt.AcadDocument.WriteMessage(CStr(iLineID) & "\" & CStr(iCheckLine) & sMsg & CStr(mcolIntersectPoints.Count))
      Else
         DMAcadExt.AcadDocument.WriteMessage("Intersect points not exist")
      End If
   End Sub
   Private Sub zzDrawLoop(oMPgonLoop As MPolygonLoop)
      Dim tPoint As Point2d
      For iIndex As Integer = 0 To oMPgonLoop.Count - 1
         tPoint = oMPgonLoop.Item(iIndex).Vertex
         DMAcadExt.AcadTransaction.InsertPoint(tPoint)
      Next
   End Sub

   Private Function zzGetNumResBuffer(iNumber As Integer, Optional iLine As Integer = 0, Optional iCheckLine As Integer = 0) As ResultBuffer
      Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1001, msXDataApp)
      oResBuffer.Add(oVal)
      oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "{")
      oResBuffer.Add(oVal)
      oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iNumber)
      oResBuffer.Add(oVal)
      If iLine <> 0 Then
         oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iLine)
         oResBuffer.Add(oVal)
      End If
      If iCheckLine <> 0 Then
         oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iCheckLine)
         oResBuffer.Add(oVal)
      End If
      oVal = New Autodesk.AutoCAD.DatabaseServices.TypedValue(1002, "}")
      oResBuffer.Add(oVal)
      Return oResBuffer
   End Function
   Public Sub DrawErrorPoints()
      If moPointList IsNot Nothing AndAlso moPointList.Count > 0 Then
         System.Windows.Forms.MessageBox.Show(CStr(moPointList.Count), "03_811")
         Dim dScale As Double = DMAcadExt.AcadDocument.GetTopoErrBlockScale()


         Dim oaAttribDefs() As AttributeDefinition = Nothing

         '	System.Windows.Forms.MessageBox.Show(CStr(dScale), "03_944")
         If mtOctagonAcObjId.IsNull Then
            mtOctagonAcObjId = TopoManager.TopoCreator.GetOctagonBlock()
         End If
         'Return
         colErrBlockRefs = DMAcadExt.AcadTransaction.InsertBlockRef(mtOctagonAcObjId, moPointList, oaAttribDefs, Nothing, dScale, )
      End If

   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub
End Class