Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports TopoManager.TPlanGraph
Imports TopoManager.TopoScheme


Public Class UD_App
   Const msStageTopoNamePrefix As String = "Stage_"
   Const msFinalTopologyName As String = "FinalStage"

   Public Const UD_NewPointBlockName As String = "SRVS007"
   Public Const UD_NewPointLayer As String = "SRVS007"

   Public Const UD_ForcedPointLayer As String = "UD_Forced_Point"
   Public Const ParcelCentreBlockName As String = "C1603"
   Public Const OldPointBlockName As String = "C1610"

   Public Const NewPointBlockName As String = "C1611"
   Public Const FrontLineBlockName As String = "C1609"
   Public Const OldHorizontalPointBlockName As String = "C1615"
   Public Const OldVerticalPointBlockName As String = "C1616"

   Public Const MarkGushBlockName As String = "C1650_PLT"
   Public Const MarkGushCancelledBlockName As String = "C1651_PLT"
   Public Const MarkNewGushBlockName As String = "C1652_PLT"




	Public Const ReportTableLayer As String = "C1682"
   Public Const FragmentNewNodesLayer As String = "Ud_NewPoints"
  

    
   Public Shared Auxiliary() As String = {"zzPointsOut", "zzUnusedPoints", "zzFLinePoint", "C1609_Err"}
   Public Shared AddLayers() As String = {OldHorizontalPointBlockName, OldVerticalPointBlockName}
   Private Shared miLastStageNo As Integer
   Private Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit" ' "M:\Dm_Work\Blocks\Hanit_1.3" '
   ' Private Shared msBlockPath13 As String = "M:\Dm_Work\Blocks\Hanit_1.3" '
   Private Const msHanit13Folder As String = "Hanit_1.3"

   '
   Private Shared mdicParcels As UD_Parcels
   Private Shared mdicSourceParcels As Dictionary(Of Integer, UD_Parcel)

   Private Shared moParcelsScheme As TopoManager.TopoScheme.tsTopology
   Private Shared moFragmentsToposcheme As TopoManager.TopoScheme.tsTopology


   Private Shared mdicPoints As UD_Points
   Private Shared mdicFLines As UD_FLines
   Private Shared mcolAllNodes As ObjectIdCollection = New ObjectIdCollection()
   Private Shared mcolCanceledLinks As ObjectIdCollection = New ObjectIdCollection()


   Private Shared mdicPrevParcels As Dictionary(Of String, DMAcadExt.BlockRefData)
   Private Shared mdicPrevPoints As Dictionary(Of String, DMAcadExt.BlockRefData)
   Private Shared mdicPrevFLines As Dictionary(Of String, DMAcadExt.BlockRefData)
   Public Shared IsTTG As Boolean

   Private Shared mhsNodes As HashSet(Of Integer) = New HashSet(Of Integer)()
   Private Shared moAttachSurveyPoints As DMAcadExt.AttachEntities
   Private Shared moAttachFLinePoints As DMAcadExt.AttachEntities
   Private Shared miTestCounterA As Integer
   Private Shared miTestCounterAa As Integer
   Private Shared miTestCounterB As Integer
   Private Shared miTestCounterBa As Integer
   Private Shared miTestCounterC As Integer
   Private Shared miTestCounterCa As Integer
   Private Shared miTestCounterD As Integer
   Private Shared miTestCounterDa As Integer
   Private Shared miTestCounterDb As Integer
   Private Shared mdicMarkEntities As ObjectIdCollection

   Private Shared miTestCounterE As Integer
   Private Shared miTestCounterF As Integer
   Private Shared miTestCounterFa As Integer

   Private Shared miTestCounterG As Integer
   Private Shared miTestCounterGa As Integer
   Private Shared miTestCounterGb As Integer
   Private Shared miTestCounterGc As Integer



   Private Shared miTestCounterH As Integer
   Private Shared miTestCounterI As Integer
   Private Shared miTestCounterJ As Integer
   Private Shared miTestCounterK As Integer

   Private Shared moCircleMarkBlock As DMAcadExt.MarkBlock
   Private Shared moTriangleMarkBlock As DMAcadExt.MarkBlock
   Private Shared moSquareMarkBlock As DMAcadExt.MarkBlock
   Private Shared moRhombusMarkBlock As DMAcadExt.MarkBlock
   Private Shared miProblemBlocksCount As Integer = 0
   Private Shared miDrawingIndex As Integer = 0
   Private Shared moNewPointBlock As DMAcadExt.AcadBlock
   Private Shared WithEvents moDrawingSet As Autodesk.Gis.Map.Project.DrawingSet = DMAcadExt.AcadMapApp.GetDrawingSet()
   Private Shared moAttachedDrawing As Autodesk.Gis.Map.Project.AttachedDrawing
   Private Shared mbHasPreviewVersion As Boolean
   Private Delegate Function StageLayer(iStageNo As Integer) As String
   Public Shared Sub InputPrevDB()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

      zzLoadPrevParcelCenters()
      zzLoadPrevSurveyPoints()

      zzLoadPrevFLineBlocks()
      mbHasPreviewVersion = True
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Shared Sub ImportHanitBordersA()
      Dim oDrawing As Autodesk.Gis.Map.Project.AttachedDrawing
      Dim oDrawingSet As Autodesk.Gis.Map.Project.DrawingSet = DMAcadExt.AcadMapApp.GetDrawingSet()

      For iIndex As Integer = 0 To oDrawingSet.DirectDrawingsCount - 1
         oDrawing = oDrawingSet.DirectAttachedDrawings.Item(iIndex)
         System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_340")
         zzImportHanitBorder(oDrawing.ActualPath)
			System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_370i")
			Exit For
      Next

   End Sub

	Public Shared Sub ImportHanitBorders()

		moDrawingSet = DMAcadExt.AcadMapApp.GetDrawingSet()
		' Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
		moAttachedDrawing = moDrawingSet.DirectAttachedDrawings.Item(miDrawingIndex)
		moAttachedDrawing.Activate()


		'  System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_340")
		'   oDrawing.Activate()
		' zzImportHanitBorder(True)
		'  oBulgeVertexArray = zzImportHanitBorder(True)
		'   oDrawing.Deactivate()
		' oDrawingSet.DetachDrawing(iDrawingIndex)
		'  DMAcadExt.AcadDocument.SendRegenAll()


		'  System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_370k")
		'oBulgeVertexArray.CreateDBPolyline(True)



	End Sub
	Public Shared Sub DeleteScriptTable()


		Dim oODTables As Autodesk.Gis.Map.ObjectData.Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
		If oODTables.IsTableDefined(UnidivNet.Ud_ScriptODTable.ODTableName) Then
			oODTables.RemoveTable(UnidivNet.Ud_ScriptODTable.ODTableName)
		End If



	End Sub
	Public Shared Sub ImportHanitBordersLoop()
      Dim oDrawing As Autodesk.Gis.Map.Project.AttachedDrawing
      Dim oDrawingSet As Autodesk.Gis.Map.Project.DrawingSet = DMAcadExt.AcadMapApp.GetDrawingSet()
      ' Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
      For iDrawingIndex As Integer = 2 To oDrawingSet.DirectDrawingsCount - 1
         oDrawing = oDrawingSet.DirectAttachedDrawings.Item(iDrawingIndex)
         System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_340")
         oDrawing.Activate()
         ''''''''''''''''''''''''     zzImportHanitBorder(True, iDrawingIndex
         '  oBulgeVertexArray = zzImportHanitBorder(True)
         oDrawing.Deactivate()
         ' oDrawingSet.DetachDrawing(iDrawingIndex)
         DMAcadExt.AcadDocument.SendRegenAll()


			System.Windows.Forms.MessageBox.Show(oDrawing.ActualPath, "05_370j")
			'oBulgeVertexArray.CreateDBPolyline(True)

		Next

   End Sub
   Public Shared ReadOnly Property Points As UD_Points
      Get
         Return mdicPoints
      End Get
   End Property
   Public Shared ReadOnly Property AllNodes As ObjectIdCollection
      Get
         Return mcolAllNodes
      End Get
   End Property
   Public Shared Sub HanitBorders()
      Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
      oBulgeVertexArray = zzImportHanitBorderA(False)
      oBulgeVertexArray.CreateDBPolyline(True)
   End Sub
   Private Shared Function zzImportHanitBorderA(bCreateObjects As Boolean) As TopoManager.GeoUtilites.BulgeVertexArray
      Dim bRes As Boolean = TopoManager.TopoCreator.TopologyExists("Stage_0")
		'  Dim oCurve As Curve
		'  System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & "", "05_378")
		Dim oTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Stage_0", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True, bCreateObjects)
      If oTopoModel IsNot Nothing Then
         '  System.Windows.Forms.MessageBox.Show(oTopoModel.Name.ToString() & vbCrLf & oTopoModel.Status.ToString(), "05_379")
         '  System.Windows.Forms.MessageBox.Show(oTopoModel.GetPolygons().Count.ToString() & vbCrLf & oTopoModel.Status.ToString(), "05_380")
         Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("OuterBoundary")
         oStageTopoScheme.Load(False, oTopoModel)
         Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray = oStageTopoScheme.GetOuterVertexArray()
         System.Windows.Forms.MessageBox.Show(oBulgeVertexArray.UB.ToString() & vbCrLf & "", "05_369")
         Return oBulgeVertexArray
      Else
         Return Nothing
      End If

   End Function
   Private Shared Sub zzImportHanitBorder(bCreateObjects As Boolean)
      '   Dim sRes As String = TopoManager.TopoCreator.TopoExists("Stage_0")

      '  Dim oDrawing As Autodesk.Gis.Map.Project.AttachedDrawing
      '  System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & "", "05_378")

      '   Dim colLinks As ObjectIdCollection '= oStageTopoScheme.GetOuterBoundary()


      '    oDrawing = moDrawingSet.DirectAttachedDrawings.Item(miDrawingIndex)
      '    oDrawing.Activate()
      Dim oTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Stage_0", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True, bCreateObjects)
      If oTopoModel IsNot Nothing Then
         '  Dim colLinks As ObjectIdCollection '= oStageTopoScheme.GetOuterBoundary()
         '  System.Windows.Forms.MessageBox.Show(oTopoModel.GetPolygons().Count.ToString() & vbCrLf & oTopoModel.Status.ToString(), "05_380")
         If False Then
            Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("OuterBoundary")
            DMAcadExt.AcadDocument.WriteMessage("Before Load: " & CStr(239))
            oStageTopoScheme.Load(False, oTopoModel)

         End If
         'Dim colLinks As ObjectIdCollection '= oStageTopoScheme.GetOuterBoundary()
         '= oStageTopoScheme.GetOuterBoundary()
         ' System.Windows.Forms.MessageBox.Show(colLinks.Count.ToString() & vbCrLf & "", "05_369")
         oTopoModel.Close()
         '''''''''''  moAttachedDrawing.Deactivate()


         '  Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray = New TopoManager.GeoUtilites.BulgeVertexArray(
      Else
         System.Windows.Forms.MessageBox.Show("oTopoModel Is Nothing " & vbCrLf & "", "05_341")
      End If

   End Sub
   Private Shared Sub zzImportHanitBorder(sDWGFullName As String)
      Dim oFileInfo As System.IO.FileInfo
      Dim oHanitDrawing As Autodesk.AutoCAD.ApplicationServices.Document
      Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
      Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("OuterBoundary")
      '  Dim oStage0Topo As TopologyModel
      oFileInfo = New System.IO.FileInfo(sDWGFullName)

      If oFileInfo.Exists Then
         oHanitDrawing = DMAcadExt.AcadDocument.OpenDocument(sDWGFullName, True, True)
         System.Windows.Forms.MessageBox.Show(CStr(oHanitDrawing.Name), "!05_000")
         '  DMAcadExt.AcadDocument.Reset()
         oDocLock = oHanitDrawing.LockDocument()
         ' 
         Dim sRes As String = TopoManager.TopoCreator.TopoExists("Stage_0", oHanitDrawing.Database)

         System.Windows.Forms.MessageBox.Show(sRes & vbCrLf & oHanitDrawing.Database.OriginalFileName, "05_358")
         oStageTopoScheme.Load(False)
         System.Windows.Forms.MessageBox.Show(CStr(oStageTopoScheme.Elements.PolygonIDs.Count), "05_359")

         Try
            oDocLock.Dispose()
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "AcadDocument - Unlock")
         Finally
            oDocLock = Nothing
         End Try
         ''''''''''''''''''''''  oHanitDrawing.CloseAndDiscard()
         'DMAcadExt.AcadDocument.CloseAndDiscardActiveDocument()
         System.Windows.Forms.MessageBox.Show(sDWGFullName, "05_362")
      End If
   End Sub

   Public Shared Function OpenDB() As Boolean
      Dim bRes As Boolean
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      '   DMAcadExt.AcadDocument.OpenLog(False)

      zzLoadJournal()
      '   System.Windows.Forms.MessageBox.Show(mdicParcels.Count.ToString(), "04_009")
      zzLoadParcelList()
      '   System.Windows.Forms.MessageBox.Show(mdicParcels.Count.ToString(), "04_010")
      'zzLoadParcelCentroids()
      zzLoadSnapShot()

      '      System.Windows.Forms.MessageBox.Show("before LoadFragm", "02_060a")
      bRes = zzLoadFragmentCentroids()
      '   System.Windows.Forms.MessageBox.Show("after LoadFragm", "02_061a")
      zzLoadPointsDB()

      If True Then
         zzLoadSurveyPoints("SRVS001", "UD_SRVS001")
         'System.Windows.Forms.MessageBox.Show("SRVS001", "02_066a")
         zzLoadSurveyPoints("SRVS002", "UD_SRVS002")
         '   System.Windows.Forms.MessageBox.Show("SRVS002", "02_066b")
         zzLoadSurveyPoints("SRVS003", "UD_SRVS003")
         '  System.Windows.Forms.MessageBox.Show("SRVS003", "02_066c")
         zzLoadSurveyPoints("SRVS006", "UD_SRVS006")
         'System.Windows.Forms.MessageBox.Show("SRVS006", "02_066k")
         zzLoadSurveyPoints(UD_NewPointBlockName, UD_NewPointLayer)
         'System.Windows.Forms.MessageBox.Show("after zzLoadSurveyPoints", "02_078a")
         zzLoadBlockDim()
      End If

      DMAcadExt.AcadDocument.CloseLog()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      Return bRes
   End Function
   Public Shared Function GetOldPointBlocks() As String()
      Return New String() {OldPointBlockName, OldHorizontalPointBlockName, OldVerticalPointBlockName}
   End Function
   Public Shared Sub DrawPointsOut()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      '  System.Windows.Forms.MessageBox.Show(mdicParcels.Count.ToString(), "04_009")

      '     System.Windows.Forms.MessageBox.Show(mdicParcels.Count.ToString(), "04_010")
      'zzLoadParcelCentroids()


      '   System.Windows.Forms.MessageBox.Show("before LoadFragm", "02_060a")

      '  System.Windows.Forms.MessageBox.Show("after LoadFragm", "02_061a")
      zzDrawPointsOut()

      ' System.Windows.Forms.MessageBox.Show("after zzLoadSurveyPoints", "02_078a")

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Public Shared Sub DeleteMarkBlocks()
      moCircleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
      moCircleMarkBlock.EraseMyReferences()
      moTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
      moTriangleMarkBlock.EraseMyReferences()
      moSquareMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
      moSquareMarkBlock.EraseMyReferences()
   End Sub
   Private Shared Sub zzLoadSurveyPoints(sBlockName As String, Optional sLayerName As String = "")
      '   Dim tSurveyPointBlockRefObjID, tBaseBlockRecObjID As ObjectId
      '    Dim iaBlockAttribIndex(enUD_BaseBlockAttribIndices.UB) As Integer


      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sBlockName)
      Dim tBlockRefData As DMAcadExt.BlockRefData
      Dim saFields() As String = {"Name"}
      '	System.Windows.Forms.MessageBox.Show(CStr(msBaseBlockName), "04_064")
      Dim colObjectIds As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs(oAcadBlock.BlockName)
      '  System.Windows.Forms.MessageBox.Show(oAcadBlock.BlockName & ": " & CStr(colObjectIds.Count), "04_088")
      oAcadBlock.Fields = saFields
      oAcadBlock.OpenForRead()
      If oAcadBlock.DefinitionExists Then
         For Each tAcObjID As ObjectId In colObjectIds
            tBlockRefData = oAcadBlock.GetBlockRefData(tAcObjID)
            If tBlockRefData.Layer = UD_ForcedPointLayer Then
               mdicPoints.BlockingPoint(tBlockRefData.Position)
            Else
               If sLayerName.Length = 0 OrElse tBlockRefData.Layer = sLayerName Then
                  mdicPoints.UpdatePointName(tBlockRefData)
               End If
            End If

         Next
      End If



      '  saAttribText = tBlockRefData.AttribValues

      '	DMCommon.Functions.DispArray(saAttribText, "saAttribText", True)

      '	System.Windows.Forms.MessageBox.Show(CStr(miBaseBlockNo), "04_072")


   End Sub
   Public Shared Sub InsertFlines(iStage As Integer)
      Dim itest As Integer
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)

      For Each oFLine As UD_FLine In mdicFLines.Values

         If oFLine.Stage = iStage Then
            itest += 1
            oFLine.InsertBlock()
         End If

      Next
      DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest, iStage)

   End Sub
   Public Shared Sub CreateFinalTopology()
      Const sFinalTopologyName As String = "FinalStage"
      Dim oPrevPoint As UnidivNet.UD_Point
      Dim oNextPoint As UnidivNet.UD_Point
      Dim oFLineNode As tsNode

      Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim oFinalTopoScheme As tsTopology

      Dim dicFLines As UD_FLines = New UD_FLines()
      ' DMCommon.Debug.MsgBox("08_663", DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(moFragmentsToposcheme.Branches), DMCommon.Debug.ColCount(mcolCanceledLinks))
      '  Dim itest As Integer
      '  Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)
      For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
         If Not mcolCanceledLinks.Contains(oBranch.AcObjID) Then
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
            oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
            oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If

            ' oFLine = New UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
            '  mdicFLines.Add(oBranch.AcObjID, oFLine)
            colLinks.Add(oBranch.AcObjID)
         End If
      Next
      '     DMCommon.Debug.MsgBox("08_664", DMCommon.Debug.ColCount(colLinks), DMCommon.Debug.ColCount(mcolAllNodes))
      If zzCreateFinalTopology(sFinalTopologyName, colLinks) Then
         oFinalTopoScheme = New tsTopology(sFinalTopologyName)
         oFinalTopoScheme.Load(True)
         For Each oNode As tsNode In oFinalTopoScheme.Nodes
            Select Case oNode.BlockName.ToUpper
               Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                  oNode.HasOldPoint = True
            End Select
         Next
         oFinalTopoScheme.RemovePseudoPoints()
      End If

      '  DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest)

   End Sub
   Public Shared Sub InsertFinalFlines()
      Const sFinalTopologyName As String = "FinalStage"
      Dim oPrevPoint As UnidivNet.UD_Point
      Dim oNextPoint As UnidivNet.UD_Point
      Dim oFLineNode As tsNode
      Dim oFLine As UD_FLine
      Dim oFinalTopoScheme As tsTopology
      Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
      Dim dicFLines As UD_FLines = New UD_FLines()
      '  DMCommon.Debug.MsgBox("08_663", moFragmentsToposcheme, DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(moFragmentsToposcheme.Branches), DMCommon.Debug.ColCount(mcolCancelledinks))
      '  Dim itest As Integer

      If TopoManager.TopoCreator.TopologyExists(sFinalTopologyName) Then
         oFinalTopoScheme = New tsTopology(sFinalTopologyName)
         oFinalTopoScheme.Load(True)
         For Each oNode As tsNode In oFinalTopoScheme.Nodes
            Select Case oNode.BlockName.ToUpper
               Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                  oNode.HasOldPoint = True
            End Select
         Next

			oFinalTopoScheme.RemovePseudoPoints()
         colNodeLinks = oFinalTopoScheme.GetNodeLinksCorrected
         '  DMCommon.Debug.MsgBox("09_766W", colNodeLinks.Count, DMCommon.Debug.ColCount(mdicPoints))
         For Each tNodeLink As NodeLink In colNodeLinks

            oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NodeID)

            If oFLineNode IsNot Nothing Then
               oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
               ' DMCommon.Debug.MsgBox("09_779", oPrevPoint, oFLineNode.AcObjID)
               oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NextNodeID)
               If oFLineNode IsNot Nothing Then

                  oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
                  If oPrevPoint IsNot Nothing AndAlso oNextPoint IsNot Nothing Then

                     oFLine = New UD_FLine(oPrevPoint, oNextPoint, tNodeLink.Link)
                     '      DMCommon.Debug.MsgBox("09_791a", oPrevPoint.Name, oPrevPoint.Stage, oNextPoint.Name, oNextPoint.Stage, oFLine.Stage)
                     dicFLines.AddFLine(oFLine)
                  End If
               End If
            End If
         Next
         '   DMCommon.Debug.MsgBox("09_760", dicFLines.Count, dicFLines.Values.Count)
         For Each oFLine In dicFLines.Values
            oFLine.InsertBlock()
         Next
      End If

      '  DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest)

   End Sub
   Public Shared Sub InsertFinalFlines_200717()
      Const sFinalTopologyName As String = "FinalStage"
      Dim oPrevPoint As UnidivNet.UD_Point
      Dim oNextPoint As UnidivNet.UD_Point
      Dim oFLineNode As tsNode
      Dim oFLine As UD_FLine
      Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim oFinalTopoScheme As tsTopology
      Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
      Dim dicFLines As UD_FLines = New UD_FLines()
      '  DMCommon.Debug.MsgBox("08_663", moFragmentsToposcheme, DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(moFragmentsToposcheme.Branches), DMCommon.Debug.ColCount(mcolCancelledinks))
      '  Dim itest As Integer
      '  Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)
      For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
         If Not mcolCanceledLinks.Contains(oBranch.AcObjID) Then
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
            oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
            oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If

            ' oFLine = New UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
            '  mdicFLines.Add(oBranch.AcObjID, oFLine)
            colLinks.Add(oBranch.AcObjID)
         End If
      Next

      If zzCreateFinalTopology(sFinalTopologyName, colLinks) Then
         oFinalTopoScheme = New tsTopology(sFinalTopologyName)
         oFinalTopoScheme.Load(True)

         oFinalTopoScheme.RemovePseudoPoints()
         colNodeLinks = oFinalTopoScheme.GetNodeLinksCorrected
         '  DMCommon.Debug.MsgBox("09_766W", colNodeLinks.Count, DMCommon.Debug.ColCount(mdicPoints))
         For Each tNodeLink As NodeLink In colNodeLinks

            oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NodeID)

            If oFLineNode IsNot Nothing Then
               oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
               ' DMCommon.Debug.MsgBox("09_779", oPrevPoint, oFLineNode.AcObjID)
               oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NextNodeID)
               If oFLineNode IsNot Nothing Then

                  oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
                  If oPrevPoint IsNot Nothing AndAlso oNextPoint IsNot Nothing Then

                     oFLine = New UD_FLine(oPrevPoint, oNextPoint, tNodeLink.Link)
                     '      DMCommon.Debug.MsgBox("09_791a", oPrevPoint.Name, oPrevPoint.Stage, oNextPoint.Name, oNextPoint.Stage, oFLine.Stage)
                     dicFLines.AddFLine(oFLine)
                  End If
               End If
            End If
         Next
         '   DMCommon.Debug.MsgBox("09_760", dicFLines.Count, dicFLines.Values.Count)
         For Each oFLine In dicFLines.Values
            oFLine.InsertBlock()
         Next
      End If

      '  DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest)

   End Sub
   Public Shared Sub AddCancelledLinks(colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      Try
         For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colCancelledinks
            mcolCanceledLinks.Add(tAcObjID)
         Next
      Catch oEx As Exception

      End Try
   End Sub

   Public Shared Sub RoundAll(iDigits As Integer)
		'  Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(New String() {"UD_PCLP001", "UD_PCLP004", "UD_PCLP0011", "UD_PCLP0013"})
		' Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(New String() {"1602", "pCellK", "PCLP001", "PCLP002", "PCLP003", "PCLP004", "PCLP005", "PCLP006", "PCLP011", "PCLP013", "UD_PCLP001", "UD_PCLP002", "UD_PCLP003", "UD_PCLP004", "UD_PCLP005", "UD_PCLP006", "UD_PCLP011", "UD_PCLP013"})
		'	Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(New String() {"C1650", "C1660", "C1662", "pclp013"})
		Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList()


		'  Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(New String() {"SRVS001", "SRVS002", "SRVS003", "SRVS006", "SRVS007"})
		Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(New String() {"C1610", "C1611"})

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      DMAcadExt.AcadTransaction.CleanupRoundInserts(tBlockNames, iDigits)
      If tLinkLayer.Values.GetUpperBound(0) >= 0 Then
         DMAcadExt.AcadTransaction.CleanupRoundLinks(tLinkLayer, iDigits)
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   Private Shared Sub zzLoadJournal()
      Const iBaseStage As Integer = 0
      Dim sComText As String = "SELECT * FROM Journal"
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
      '	Dim iBlock As Short
      Dim oOperDet As UD_OperDetail
      Dim iStage As Integer = iBaseStage
      Dim iActionType As enActionType

      Dim s As String = String.Empty
      '    Dim oParcelComparer As ParcelKeyComparer = New ParcelKeyComparer()

      mdicParcels = New UD_Parcels()  'oParcelComparer

      '  System.Windows.Forms.MessageBox.Show(CStr(111), "04_058")
      mdicParcels.InitHanit()
      '  System.Windows.Forms.MessageBox.Show(CStr(115), "04_070")
      If oDataReader IsNot Nothing Then

         Do While oDataReader.Read
            '
            oOperDet = New UD_OperDetail(oDataReader)
            If oOperDet.ActionType <> enActionType.Update Then
               If iStage = iBaseStage OrElse iActionType <> oOperDet.ActionType Then
                  iActionType = oOperDet.ActionType
                  iStage += 1
               End If
               oOperDet.StageNo = iStage
               mdicParcels.AddOper(oOperDet)
            End If

         Loop
         oDataReader.Close()
         mdicParcels.AddTransferBlock()
         ' System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "01_519a")
      End If
      IsTTG = (mdicParcels.Count = 0)
      mdicParcels.TEST()
   End Sub
   Private Shared Sub zzDrawPointsOut()
      Dim sComText As String = "SELECT XX,YY FROM Points"
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		' Dim oPoint As UD_Point
		Dim tAcadPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim dX, dY As Double
      Dim bCurrentLayerOK As Boolean
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(Auxiliary(0), DMAcadExt.DMApp.AppID, True, True) '"zzPointsOut"
      DMAcadExt.AcadTransaction.ClearLayerList(Auxiliary(0))
      '	System.Windows.Forms.MessageBox.Show(CStr(111), "04_058")

      '	System.Windows.Forms.MessageBox.Show(CStr(115), "04_070")
      If oDataReader IsNot Nothing Then
         moCircleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
         mdicMarkEntities = New ObjectIdCollection()

         Do While oDataReader.Read
            dX = oDataReader.GetDouble(0)
            dY = oDataReader.GetDouble(1)

            tAcadPoint = New Autodesk.AutoCAD.Geometry.Point3d(dX, dY, 0.0)

            zzDrawMarkCircleNew(tAcadPoint, 2S)

         Loop
         oDataReader.Close()
         '   System.Windows.Forms.MessageBox.Show(CStr(mdicPoints.Count) & vbCrLf & moAttachSurveyPoints.GePointsCount(True) & " of " & moAttachSurveyPoints.GePointsCount(False), "01_549a")
      End If

   End Sub

   Private Shared Sub zzLoadPointsDB()
      Dim sComText As String = "SELECT * FROM PointsOut"
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
      Dim oPoint As UD_Point

    




      mdicPoints = New UD_Points()
      moAttachSurveyPoints = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPoint, 0.001)
      '	System.Windows.Forms.MessageBox.Show(CStr(111), "04_058")


      If oDataReader IsNot Nothing Then

         Do While oDataReader.Read

            oPoint = New UD_Point(oDataReader)
            oPoint.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()
            If mdicPoints.ContainsKey(oPoint.PointKey) Then
               ' DMAcadExt.TplnPointKeyLong
               Dim oRndPoint As DMAcadExt.TPlnPoint = DMAcadExt.TplnPointKeyLong.GetRoundedPoint(oPoint.Point.X, oPoint.Point.Y)

               DMAcadExt.AcadDocument.WriteMessage("Double Point: " & CStr(oPoint.Name) & "; " & CStr(oPoint.Coordinates) & ";Rnd " & CStr(oPoint.Coordinates))
               'Err
            Else
               mdicPoints.AddPoint(oPoint)
            End If

            moAttachSurveyPoints.AddPointEntity(oPoint.Point, ObjectId.Null, False)
         Loop
         oDataReader.Close()
         '   System.Windows.Forms.MessageBox.Show(CStr(mdicPoints.Count) & vbCrLf & moAttachSurveyPoints.GePointsCount(True) & " of " & moAttachSurveyPoints.GePointsCount(False), "01_549a")
      End If

      '     System.Windows.Forms.MessageBox.Show(CStr(mdicPoints.Count), "04_070")
   End Sub
	Private Shared Sub zzLoadLinksDB_AAA()
		Dim sComText As String = "SELECT * FROM LinksOut"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		Dim oPoint As UD_Point

		Dim tCenterPoint As Autodesk.AutoCAD.Geometry.Point2d = DMAcadExt.AcadDocument.GetCenterPoint()
		DMAcadExt.TplnPointKey.SetCenter(Math.Round(tCenterPoint.X), Math.Round(tCenterPoint.Y))


		mdicPoints = New UD_Points()
		moAttachSurveyPoints = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPoint, 0.001)
		'	System.Windows.Forms.MessageBox.Show(CStr(111), "04_058")

		'	System.Windows.Forms.MessageBox.Show(CStr(115), "04_070")
		If oDataReader IsNot Nothing Then

			Do While oDataReader.Read

				oPoint = New UD_Point(oDataReader)
				mdicPoints.AddPoint(oPoint)
				moAttachSurveyPoints.AddPointEntity(oPoint.Point, ObjectId.Null, False)
			Loop
			oDataReader.Close()
			'   System.Windows.Forms.MessageBox.Show(CStr(mdicPoints.Count) & vbCrLf & moAttachSurveyPoints.GePointsCount(True) & " of " & moAttachSurveyPoints.GePointsCount(False), "01_549a")
		End If

	End Sub
	Private Shared Sub zzLoadParcelCentroids()
      mdicParcels.LoadUDParcelCentroid()
   End Sub
   Private Shared Function zzLoadFragmentCentroids() As Boolean
      Return mdicParcels.LoadFragmentCentroids()
   End Function
   Private Shared Sub zzLoadParcelList()
      ' Dim sComText As String = "SELECT Block,Parcel,Original,Active,CalcArea,LegalArea,ForcedArea FROM ParcList"
      Dim sComText As String = "SELECT Block,Parcel,ParcelID,Original,Active,CalcArea,LegalArea,ForcedArea,Lot,Plan,Luse FROM ParcList"



      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

      Dim iBlockNo As Integer
      Dim iParcelNo As Integer
      Dim bOrigin As Boolean
      Dim bActive As Boolean



      Dim shBlockNo As Short
      Dim shParcelNo As Short
      Dim dParcelID As Double

      '	Dim oOperDet As UD_OperDetail
      Dim iStage As Integer = 0
      '	Dim iActionType As enActionType
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UD_Parcel = Nothing
      Dim dLegalArea As Double
      Dim dCalcArea As Double
      Dim dForcedArea As Double
      Dim bStage0_Only As Boolean


      If oDataReader IsNot Nothing Then
         '	MessageBox.Show("", "01_518a")
         Dim oTest(5) As System.Object
         Dim s As String = ""
         Dim sTest As String = ""
         bStage0_Only = (mdicParcels.Count = 0)

         '		Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
         Do While oDataReader.Read
            Try


               sTest = "a"
               shBlockNo = oDataReader.GetInt16(0)
               shParcelNo = oDataReader.GetInt16(1)
               'System.Windows.Forms.MessageBox.Show(shBlockNo.ToString() & vbCrLf & shParcelNo.ToString, "04_412b")
               iBlockNo = Convert.ToInt32(shBlockNo)
               iParcelNo = Convert.ToInt32(shParcelNo)


               dParcelID = oDataReader.GetDouble(2)
               bOrigin = oDataReader.GetBoolean(3)
               bActive = oDataReader.GetBoolean(4)
               '		System.Windows.Forms.MessageBox.Show(iBlockNo.ToString() & vbCrLf & iParcelNo.ToString, "04_412c")


               sTest = "b"

               If iBlockNo = 1 Then
                  tParcelKey = New UD_ParcelKey(iParcelNo, bOrigin)
               Else
                  If iBlockNo < 0 Then
                     iBlockNo += UShort.MaxValue + 1
                  End If
                  tParcelKey = New UD_ParcelKey(iBlockNo, iParcelNo, bOrigin)
               End If

               dCalcArea = oDataReader.GetDouble(5)
               sTest = "e "
               '		System.Windows.Forms.MessageBox.Show(dCalcArea.ToString() & vbCrLf & iParcelNo.ToString, "04_412d")
               dLegalArea = oDataReader.GetDouble(6) * 0.001

               dForcedArea = oDataReader.GetDouble(7)



               sTest = "c " & CStr(mdicParcels Is Nothing) & ":" & tParcelKey.ToString()
               'Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
               If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
                  sTest = "d "

                  '   System.Windows.Forms.MessageBox.Show(dForcedArea.ToString() & vbCrLf & iParcelNo.ToString & vbCrLf & dLegalArea.ToString & vbCrLf & dCalcArea.ToString, "04_412f")
                  sTest = "f"

                  '	oParcel.ParcelArea = tParcelArea
                  '	
                  oParcel.CalculateArea(dLegalArea, dCalcArea)
               ElseIf tParcelKey.IsEssential Then    'bStage0_Only AndAlso

                  oParcel = New UD_Parcel(tParcelKey)
                  oParcel.Stage = -1
                  oParcel.TopoID = Convert.ToInt32(dParcelID)


                  mdicParcels.Add(tParcelKey, oParcel)
                  oParcel.IsOriginal = bOrigin
                  '  System.Windows.Forms.MessageBox.Show(dForcedArea.ToString() & vbCrLf & iParcelNo.ToString & vbCrLf & dLegalArea.ToString & vbCrLf & dCalcArea.ToString, "04_417Z")
                  oParcel.CalculateArea(dLegalArea, dCalcArea)
                  sTest = "cd "
                  ' System.Windows.Forms.MessageBox.Show(tParcelKey.ToString() & vbCrLf & iParcelNo.ToString & vbCrLf & bOrigin.ToString & vbCrLf & oParcel.TopoID.ToString(), "04_412e")
               End If
               If Not oDataReader.IsDBNull(8) Then
                  oParcel.Lot = oDataReader.GetString(8)
               End If
               If Not oDataReader.IsDBNull(9) Then
                  oParcel.Plan = oDataReader.GetString(9)
               End If
               If Not oDataReader.IsDBNull(10) Then
                  oParcel.LanduseName = oDataReader.GetString(10)
               End If


               sTest = "e"
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest, "04_413")
            End Try

         Loop
         oDataReader.Close()
         '  System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "01_519x")
      End If
   End Sub

   Public Shared Function LoadParcelTopology() As Boolean

      'DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
      Dim sParcelTopoName As String = "Parcels" 'TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
      Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If oParcelTopology IsNot Nothing Then
         Dim oParcel As UD_Parcel
         Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
         UD_Parcel.Initialize()
         UD_Parcel.CreateMainDataTable()



         mdicParcels = New UD_Parcels()
         mdicSourceParcels = New Dictionary(Of Integer, UD_Parcel)()
         ' System.Windows.Forms.MessageBox.Show(colPolygons.Count.ToString(), "07_501aa")
         If True Then
            For Each oPolygon As Polygon In colPolygons
               oParcel = New UD_Parcel(oPolygon)
               oParcel.IsOriginal = True
               '  DMCommon.Debug.MsgBox("11_505", oParcel.BlockNo, oParcel.Name, oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.ParcelNo)
               oParcel.Calc()

               If oParcel.Correct Then 'TEMP

                  mdicParcels.AddParcel(oParcel)

                  oParcel.AddDataToMainTable()
                  mdicSourceParcels.Add(oPolygon.ID, oParcel)
               End If
               '		DMAcadExt.AcadDocument.WriteDebugMessage("N Count=" & CStr(oParcel.Neighbors.Count))
               oPolygon.Dispose()
               oPolygon = Nothing
               'oParcel.Terminate()
               oParcel = Nothing
            Next
         End If
         moParcelsScheme = New TopoManager.TopoScheme.tsTopology()
         moParcelsScheme.Load(True, oParcelTopology)
         For Each oNode As tsNode In moParcelsScheme.Nodes
            Select Case oNode.BlockName.ToUpper
               Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                  oNode.HasOldPoint = True
            End Select
         Next

         moParcelsScheme.RemovePseudoPoints()

         UnidivNet.UD_App.LoadStageNodes(moParcelsScheme, 0, -1)
         UnidivNet.UD_App.LoadStageBranches(moParcelsScheme, 0)





         Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(0)
         Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
         Dim oParcelPgon As Autodesk.Gis.Map.Topology.Polygon
         DMAcadExt.AcadTransaction.ClearLayerByClassName(sPolylineLayer)
         moParcelsScheme.CreateDBPolylineMPlus(True, False)
         mdicParcels.LoadTopoScheme(oParcelTopology)
         'DMAcadExt.AcadDocument.WriteMessage("###300")
         'colPolygons.Clear()
         '	colPolygons.Dispose()




         For Each oFragmentPgonScheme As tsPolygon In moFragmentsToposcheme.Polygons
            Try
               oParcelPgon = oParcelTopology.FindPolygon(oFragmentPgonScheme.Centroid)
               If oParcelPgon IsNot Nothing Then
                  oParcel = mdicSourceParcels.Item(oParcelPgon.ID)

                  oParcel.AddFragment(oFragmentPgonScheme.ID)
                  mdicParcels.AddFragment(oFragmentPgonScheme.ID, oParcel.ParcelKey)
               End If
            Catch oEx As Exception

            End Try
            oParcel = Nothing
         Next

         oParcelTopology.Close()
         '	oParcelTopology.Dispose()
         oParcelTopology = Nothing

         Return True
      Else
         System.Windows.Forms.MessageBox.Show("Parcel Topology Is Nothing")
         Return False
      End If

   End Function
   ' UnidivNet.UD_App.LoadStageBranches(tsStageParcels, miCurrentStage)
   
   Public Shared Sub LoadFragments()
      Dim sFragmentsTopoName As String = "Fragments"
      Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
      Dim oUD_Point As UnidivNet.UD_Point
      Dim oPrevPoint As UnidivNet.UD_Point
      Dim oNextPoint As UnidivNet.UD_Point
      Dim oFLine As UD_FLine
      Dim oFLineNode As tsNode
      moFragmentsToposcheme = New TopoManager.TopoScheme.tsTopology(sFragmentsTopoName)

      moFragmentsToposcheme.Load(False)

		mdicPoints.OpenMarkBlocks()
		mdicFLines = New UD_FLines()
      DMCommon.Debug.MsgBox("09_109", moFragmentsToposcheme.Polygons.Count)
      For Each oNode As tsNode In moFragmentsToposcheme.Nodes
         If oNode.AcObjID.IsNull Then
            DMCommon.Debug.MsgBox("09_110z", " oNode.AcObjID.IsNull", oNode.Location)
         Else
            mcolAllNodes.Add(oNode.AcObjID)
         End If

         oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(oNode.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

         If oBlockRef IsNot Nothing Then
            oNode.BlockName = oBlockRef.Name
            oUD_Point = New UnidivNet.UD_Point(oBlockRef)
            oUD_Point.FragmentID = oNode.ID
            mdicPoints.AddNode(oUD_Point)
         Else
            DMCommon.Debug.MsgBox("09_111", " oBlockRef Is Nothing", oNode.Location)
         End If
      Next
      For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
         oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
         oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
         oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
         oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
         oFLine = New UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
         mdicFLines.Add(oBranch.AcObjID, oFLine)
      Next
		'  Dim iUndef, i0 As Integer
		'For Each oPoint As UD_Point In mdicPoints.Values
		'       If oPoint.Stage = -1 Then
		'          iUndef = iUndef + 1
		'       ElseIf oPoint.Stage = 0 Then
		'          i0 += 1
		'       End If
		'    Next

		moFragmentsToposcheme.Close()
      '    DMCommon.Debug.MsgBox("09_681", DMCommon.Debug.ColCount(mdicPoints), mcolAllNodes.Count, DMCommon.Debug.ColCount(mdicFLines), iUndef, i0)




   End Sub
   Public Shared Sub WriteAllPointsInfo()
      Dim iIndex As Integer
      For Each oPoint As UD_Point In mdicPoints.Values
         DMAcadExt.AcadDocument.WriteDebugMessage("Pt_" & iIndex.ToString() & "; " & oPoint.Name & "; " & oPoint.Stage.ToString() & "; " & oPoint.Point.Coordinates2d)
         iIndex += 1
      Next
   End Sub
   Public Shared Function LoadStageNodes(tsStageParcels As tsTopology, iStage As Integer, iNewPointNumber As Integer) As Boolean
      Dim oPoint As UnidivNet.UD_Point = Nothing
      Dim iTest As Integer
      Dim sTestName As String = ""
      Dim sPointName As String = ""
      Dim bCurrentLayerOK As Boolean
      Dim sNewPointLayer As String = GetStageUDPointLayer(iStage, iStage <> 0)
      Dim sOldPointLayer As String = GetStageUDPointLayer(iStage, iStage = 0)
      If sNewPointLayer IsNot Nothing Then
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewPointLayer, DMAcadExt.DMApp.AppID, True, True)
      End If
      '    DMCommon.Debug.MsgBox("09_551", tsStageParcels.Name, tsStageParcels.Nodes.Count, mdicPoints.Count)
      For Each oNode As tsNode In tsStageParcels.Nodes
         '  DMAcadExt.AcadDocument.WriteDebugMessage("#aP " & tsStageParcels.Name & "; " & oNode.IsPseudo & "; " & oNode.Location.ToString() & ", " & oNode.AcObjID.ToString())
         If Not oNode.IsPseudo AndAlso mdicPoints.TryGetPoint(oNode.AcObjID, oPoint) Then
            If iStage = 0 Then
               If String.IsNullOrEmpty(oPoint.Name) Then
                  sPointName = "NoName"
               Else
                  sPointName = oPoint.Name
               End If
               '    DMAcadExt.AcadDocument.WriteDebugMessage("#aQ " & sPointName & "; " & oNode.IsPseudo & "; " & oPoint.Coordinates)
            End If
            If oPoint.Stage = -1 Then
               oPoint.Stage = iStage
               If iStage = 0 Then
                  oPoint.CheckBlock(sOldPointLayer)
               Else
                  oPoint.Name = iNewPointNumber.ToString()
                  iNewPointNumber += 1
                  oPoint.UpdateBlock(sNewPointLayer)
               End If

               sTestName &= sTestName & oPoint.Name & ";"
               iTest += 1
            End If
         End If
      Next
      ' DMCommon.Debug.MsgBox("09_682", DMCommon.Debug.ColCount(mdicPoints), oPoint.Name, iTest, iNewPointNumber, iStage)

      '    DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
   End Function
   Public Shared Sub PrintPointsInfo()
      Dim iIndex As Integer = 1
      For Each oPoint As UD_Point In mdicPoints.Values
         DMAcadExt.AcadDocument.WriteDebugMessage("#" & iIndex.ToString() & " " & oPoint.Name & "; St= " & oPoint.Stage.ToString() & "; " & oPoint.Point.Coordinates2d)
         iIndex += 1
      Next
   End Sub
   Public Shared Function LoadStageBranches(tsStageParcels As tsTopology, iStage As Integer) As Boolean
      Dim oFLine As UnidivNet.UD_FLine = Nothing
      '  DMCommon.Debug.MsgBox("09_551", False, tsStageParcels, tsStageParcels.Nodes, tsStageParcels.Nodes.Count, mdicPoints, mdicPoints.Count)
      For Each oBranch As tsBranch In tsStageParcels.Branches
         If mdicFLines.TryGetValue(oBranch.AcObjID, oFLine) Then
            If iStage = 2 Then
               DMAcadExt.AcadDocument.WriteDebugMessage("aX " & oFLine.Stage.ToString() & "; " & oFLine.MaxPointStage & "; " & oFLine.Name)
            End If
            If oFLine.Stage = -1 AndAlso oFLine.MaxPointStage <> -1 AndAlso oFLine.MaxPointStage <= iStage Then
               oFLine.Stage = iStage
               '  DMCommon.Debug.MsgBox("09_574", oFLine.Name, iStage)

               '  oFLine.Name = iNewPointNumber.ToString()
               '  iNewPointNumber += 1
               '  oPoint.UpdateBlock()
            End If
         End If
      Next
      '    DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
   End Function
   Public Shared Sub SetStageLayerOn(iStageNo As Integer, bIsOn As Boolean, Optional bOnlyThese As Boolean = False)
      Dim saLayer(4) As String
      saLayer(0) = GetStagePolineLayer(iStageNo)
      saLayer(1) = GetStageParcelCentroidLayer(iStageNo)
      saLayer(2) = GetStageFrontLineLayer(iStageNo)
      saLayer(3) = GetStageUDPointLayer(iStageNo, True)
      saLayer(4) = GetStageUDPointLayer(iStageNo, False)

      If bOnlyThese Then
         Dim hsLayers As HashSet(Of String) = New HashSet(Of String)(saLayer)
         DMAcadExt.AcadTransaction.SetLayersOnExcept(UnidivNet.UD_App.GetStageLayers(0), False)
      Else
         DMAcadExt.AcadTransaction.SetLayersOn(saLayer, bIsOn)
      End If


   End Sub
   Public Shared Function GetStageLayers(iStageNo As Integer) As HashSet(Of String)
      Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()

      hsHanitLayers.Add(GetStagePolineLayer(iStageNo))
      hsHanitLayers.Add(GetStageParcelCentroidLayer(iStageNo))

      hsHanitLayers.Add(GetStageFrontLineLayer(iStageNo))

      hsHanitLayers.Add(GetStageUDPointLayer(iStageNo, True))
      hsHanitLayers.Add(GetStageUDPointLayer(iStageNo, False))
      Return hsHanitLayers
   End Function
   Private Shared Sub zzLoadSnapShot()
      ' Dim sComText As String = "SELECT Block,Parcel,Original,Active,CalcArea,LegalArea,ForcedArea FROM ParcList"
      '  Dim sComText As String = "SELECT FragmentID,Block,Parcel,Orgn,ParcelID FROM SnapShot WHERE Orgn"
      Dim sComText As String = "SELECT FragmentID,Block,Parcel,Orgn,ParcelID FROM SnapShot"
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

      Dim iBlockNo As Integer
      Dim iParcelNo As Integer
      Dim iFragment As Integer

      Dim bOrigin As Boolean
      ' Dim bActive As Boolean



      Dim shBlockNo As Short
      Dim shParcelNo As Short
      Dim dParcelID As Double
      Dim dFragment As Double


      '	Dim oOperDet As UD_OperDetail

      '	Dim iActionType As enActionType
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UD_Parcel = Nothing
      '     Dim dLegalArea As Double
      '   Dim dCalcArea As Double
      '   Dim dForcedArea As Double
      '   Dim bStage0_Only As Boolean


      If oDataReader IsNot Nothing Then
			'	MessageBox.Show("", "01_518a")
			Dim oTest(5) As System.Object
			Dim s As String = ""
         Dim sTest As String = ""


         '		Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
         Do While oDataReader.Read
            Try


               sTest = "a"
               dFragment = oDataReader.GetDouble(0)
               shBlockNo = oDataReader.GetInt16(1)
               shParcelNo = oDataReader.GetInt16(2)
               'System.Windows.Forms.MessageBox.Show(shBlockNo.ToString() & vbCrLf & shParcelNo.ToString, "04_412b")
               iBlockNo = Convert.ToInt32(shBlockNo)
               iParcelNo = Convert.ToInt32(shParcelNo)

               bOrigin = oDataReader.GetBoolean(3)
               dParcelID = oDataReader.GetDouble(4)


               '		System.Windows.Forms.MessageBox.Show(iBlockNo.ToString() & vbCrLf & iParcelNo.ToString, "04_412c")

               iFragment = Convert.ToInt32(dFragment)
               sTest = "b"

               If iBlockNo = 1 Then
                  tParcelKey = New UD_ParcelKey(iParcelNo, bOrigin)
               Else
                  If iBlockNo < 0 Then
                     iBlockNo += UShort.MaxValue + 1
                  End If
                  tParcelKey = New UD_ParcelKey(iBlockNo, iParcelNo, bOrigin)
               End If



               sTest = "c " & CStr(mdicParcels Is Nothing) & ":" & tParcelKey.ToString()
               'Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
               If mdicParcels.TryGetValue(tParcelKey, oParcel) Then

                  oParcel.TryAddFragment(iFragment)
               Else
                  System.Windows.Forms.MessageBox.Show(tParcelKey.ToString(), "04_467")
               End If
               If oParcel.ParcelKey.ParcelNo > -1 Then
                  ' System.Windows.Forms.MessageBox.Show(tParcelKey.ToString() & vbCrLf & oParcel.ParcelKey.ParcelNo.ToString() & vbCrLf & oParcel.FirstFragment.ToString, "04_499")
               End If

               sTest = "e"
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest, "04_413")
            End Try

         Loop
         oDataReader.Close()
         '  System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "01_519x")
      End If
   End Sub
   Private Shared Sub zzLoadBlockDim()
      Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d
      Dim oPoint As DMAcadExt.TPlnPoint
      moAttachFLinePoints = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPointObject, 0.001)
      Dim colBlockRefIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs("PCLS007")
      Dim oDBObject As DBObject
      For Each tAcObjID As ObjectId In colBlockRefIDs
         oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         If oDBObject.OwnerId = DMAcadExt.AcadTransaction.ModelSpaceObjID Then
            tPoint3d = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(tAcObjID)
            oPoint = New DMAcadExt.TPlnPoint(tPoint3d)
            moAttachFLinePoints.AddPointEntity(oPoint, tAcObjID, False)
         End If

      Next
      'System.Windows.Forms.MessageBox.Show(CStr(moAttachFLinePoints.GetPointsCount(False)), "04_018")
   End Sub

   Public Shared Function GetStagePolineLayer(iStageNo As Integer) As String
      Return "C1602_" & CStr(iStageNo)
   End Function
   Public Shared Function GetStageFrontLineLayer(iStageNo As Integer) As String
      Return "C1609_" & CStr(iStageNo)
   End Function
   Public Shared Function GetStageTopoName(iStageNo As Integer, Optional iAction As Integer = -1) As String
      If iAction = -1 Then
         Return msStageTopoNamePrefix & CStr(iStageNo)
      ElseIf iAction > 0 Then
         Return msStageTopoNamePrefix & CStr(iStageNo) & "_" & iAction.ToString()
      Else
         Return "xxxxx"
      End If


   End Function
   Public Shared ReadOnly Property FinalTopoName() As String
      Get
         Return msFinalTopologyName
      End Get
   End Property



   Public Shared Function IsStageTopoName(sTopoName As String) As Boolean
      Return sTopoName.StartsWith(msStageTopoNamePrefix) OrElse sTopoName = UnidivNet.UD_App.FinalTopoName

   End Function
   Private Shared Sub zzDefLayers(iStageNo As Integer, ByRef saLayer() As String, ByRef hsHanitLayers As HashSet(Of String))

      saLayer(0) = GetStagePolineLayer(iStageNo)
      saLayer(1) = GetStageParcelCentroidLayer(iStageNo)
      saLayer(2) = GetStageFrontLineLayer(iStageNo)
      saLayer(3) = GetStageUDPointLayer(iStageNo, False)
      saLayer(4) = GetStageUDPointLayer(iStageNo, True)
      saLayer(5) = GetStageCentroidLayer(iStageNo)
      hsHanitLayers = New HashSet(Of String)()
      For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
         hsHanitLayers.Add(saLayer(iIndex))
      Next

   End Sub
   Private Shared Sub zzAddStageLayer(ByRef hsHanitLayers As HashSet(Of String), ByVal pStageLayer As StageLayer, bInclStage0 As Boolean, Optional bNew As Boolean = False)
      Dim iStageNo As Integer
      Dim sLayer As String
      If bInclStage0 Then
         iStageNo = 0
      Else
         iStageNo = 1
      End If
      Do
			sLayer = pStageLayer(iStageNo)
			'DMCommon.Debug.MsgBox("13_048c", sLayer, hsHanitLayers.Count)
			If Not DMAcadExt.AcadTransaction.LayerExists(sLayer) Then
            Exit Do
         End If
         hsHanitLayers.Add(sLayer)
         iStageNo += 1
      Loop
   End Sub
   Public Shared Sub NewPointsToInitLayer()
      Dim dlInParcelProc As DMAcadExt.AcadTransaction.Procedure = New DMAcadExt.AcadTransaction.Procedure(AddressOf zzNewPointToInitLayer)
      DMAcadExt.AcadTransaction.EnumModelSpaceObjects(dlInParcelProc, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
   End Sub
   Private Shared Sub zzNewPointToInitLayer(ByVal oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject)
      Dim oBlockRef As BlockReference

      If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then
         oBlockRef = DirectCast(oDBObject, BlockReference)
         If oBlockRef.Name = NewPointBlockName AndAlso zzGetStageNo(oBlockRef.Layer) > 0 Then
            oBlockRef.Layer = FragmentNewNodesLayer
         End If
      End If
   End Sub
   Private Shared Function zzGetStageNo(sName As String) As Integer
      Dim saName() As String = Split(sName, "_")
      Dim iStageNo As Integer
      If saName.GetUpperBound(0) = 1 Then
         If Integer.TryParse(saName(1), iStageNo) Then
            Return iStageNo
         Else
            Return -1
         End If
      Else
         Return -1
      End If
   End Function
	Public Shared Sub DeleteHanitTopos()
		Dim pTopoNameCriteria As TopoManager.TopoCreator.TopoNameCriteria = New TopoManager.TopoCreator.TopoNameCriteria(AddressOf IsStageTopoName)
		Try
			TopoManager.TopoCreator.DeleteTopologies(pTopoNameCriteria)
		Catch oEx As Exception

		End Try
	End Sub
	Public Shared Sub ClearCdBlockMarks()

		Dim colBlockMarks As ObjectIdCollection = New ObjectIdCollection()
		DMAcadExt.AcadTransaction.GetBlockRefsNew(MarkGushBlockName,, colBlockMarks)
		DMAcadExt.AcadTransaction.GetBlockRefsNew(MarkGushCancelledBlockName,, colBlockMarks)
		DMAcadExt.AcadTransaction.GetBlockRefsNew(MarkNewGushBlockName,, colBlockMarks)

		DMAcadExt.AcadTransaction.EraseDBObjects(colBlockMarks)
	End Sub
	Public Shared Sub ComeBackSPoints(bAllStages As Boolean)
		Dim saFields() As String = {"POINT_NAME"}
		'	Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(NewPointBlockName)

		Dim oODTable As Ud_ScriptODTable = New Ud_ScriptODTable()

		Dim tScriptData As Ud_ScriptODTable.ScriptData = New Ud_ScriptODTable.ScriptData()
		'	Dim iStage As Integer
		'	Dim sSourceName As String
		Dim dicOldValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
		Dim bODTableExists As Boolean = oODTable.Exists
		Dim bSourceLayerExists As Boolean
		dicOldValues.Add("POINT_NAME", String.Empty)

		'DMCommon.Debug.MsgBox("13_046", oODTable.Exists)
		Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		''
		If oFragmentTopology IsNot Nothing Then
			Dim colNodeCollection As NodeCollection = oFragmentTopology.GetNodes()
			Dim oBlockRef As BlockReference


			DMAcadExt.AcadDocument.WriteDebugMessageN("#265 ")
			For Each oNode As Node In colNodeCollection
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(oNode.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				If oBlockRef IsNot Nothing Then

					'	DMAcadExt.AcadDocument.WriteDebugMessageN("#266 ", oBlockRef.Handle)
					If oBlockRef.Name = NewPointBlockName AndAlso oBlockRef.Layer <> FragmentNewNodesLayer Then
						If bODTableExists Then
							tScriptData = oODTable.GetData(oBlockRef)
							bSourceLayerExists = tScriptData.SourceLayerExists
							'	DMAcadExt.AcadDocument.WriteDebugMessageN("#267 ", tScriptData.RecordExists)
						Else
							bSourceLayerExists = False
						End If

						If bAllStages OrElse tScriptData.Stage > 0 Then
							If bSourceLayerExists Then
								oBlockRef.Layer = tScriptData.SourceLayer
							Else
								oBlockRef.Layer = FragmentNewNodesLayer
							End If

							If tScriptData.SourceNameIsEmpty Then
								DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, dicOldValues)
							End If
						End If
					End If
				Else
					DMCommon.Debug.MsgBox("09_869a", "BlockReference Is Nothing", oNode.Entity)
				End If

			Next
		End If
		If oFragmentTopology IsNot Nothing Then
			oFragmentTopology.Close()
		End If

	End Sub
	Public Shared Sub ToSourceLayer()
		Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim oODTable As Ud_ScriptODTable = New Ud_ScriptODTable()

		If oFragmentTopology IsNot Nothing AndAlso oODTable.Exists Then
			Dim colFullEdges As FullEdgeCollection = oFragmentTopology.GetFullEdges()
			Dim oEntity As Entity
			Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			Dim tScriptData As Ud_ScriptODTable.ScriptData


			For Each oFullEdge As FullEdge In colFullEdges
				oEntity = DMAcadExt.AcadTransaction.GetEntity(oFullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				tScriptData = oODTable.GetData(oEntity)
				If Not String.IsNullOrEmpty(tScriptData.SourceLayer) AndAlso oEntity.Layer <> tScriptData.SourceLayer Then
					oEntity.Layer = tScriptData.SourceLayer
					oODTable.RemoveODRecord(oEntity)
				End If
			Next

		End If
		If oFragmentTopology IsNot Nothing Then
			oFragmentTopology.Close()
		End If
	End Sub

	Public Shared Sub ClearHanitCentroids()
		Const msNewParcelLayer As String = "Ud_NewParcels"
		Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
		Dim colCentroidsBlocks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(UnidivNet.UD_Parcel.BlockName)
		Dim oODTable As Ud_ScriptODTable = New Ud_ScriptODTable()
		Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
		Dim iStage As Integer
		Dim tScriptData As Ud_ScriptODTable.ScriptData = New Ud_ScriptODTable.ScriptData()
		'	Dim dicAtribValues As Generic.Dictionary(Of String, String) = UnidivNet.UD_Parcel.DefaultAttribValues()
		UnidivNet.UD_Parcel.InitAcadBlock()

		If oODTable.Exists Then
		End If

		For Each tAcObjID As ObjectId In colCentroidsBlocks

				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

				If oBlockRef IsNot Nothing Then
				If oBlockRef.Layer = UnidivNet.UD_App.GetStageParcelCentroidLayer(0) Then
					DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, UnidivNet.UD_Parcel.DefaultSourceAttribValues())

				ElseIf oODTable.Exists Then
					Try
						tScriptData = oODTable.GetData(oBlockRef)
					Catch oEx As Exception

					End Try
					If tScriptData.Exists Then
						iStage = tScriptData.Stage

						If iStage > 0 AndAlso tScriptData.SourceNameIsEmpty Then
							DMAcadExt.AcadTransaction.EraseDBObject(oBlockRef)

						ElseIf oBlockRef.Layer <> msNewParcelLayer Then
							DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, UnidivNet.UD_Parcel.DefaultNewAttribValues())
							oBlockRef.Layer = msNewParcelLayer
						End If
					End If
				End If



				End If


			Next

	End Sub

	Public Shared Function HasHanitTopos() As Boolean
		Dim pTopoNameCriteria As TopoManager.TopoCreator.TopoNameCriteria = New TopoManager.TopoCreator.TopoNameCriteria(AddressOf IsStageTopoName)
		Try
			Return TopoManager.TopoCreator.TopologiesExist(pTopoNameCriteria)
		Catch oEx As Exception
			Return False
		End Try
	End Function
	Public Shared Function GetHanitLayers() As HashSet(Of String)
      Dim saLayer(5) As String
      Dim iStageNo As Integer = 0
      Dim hsHanitLayers As HashSet(Of String) = Nothing

      Do
         zzDefLayers(iStageNo, saLayer, hsHanitLayers)
         If DMAcadExt.AcadTransaction.LayersExist(saLayer, False) Then
            For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
               hsHanitLayers.Add(saLayer(iIndex))
            Next
         Else
            DMCommon.Debug.MsgBox("13_002", iStageNo, hsHanitLayers.Count)
            Exit Do
         End If

         iStageNo += 1
      Loop While iStageNo <20





		hsHanitLayers.UnionWith(UnidivNet.UD_App.Auxiliary)
      hsHanitLayers.UnionWith(UnidivNet.UD_App.AddLayers)

		'	DMCommon.Debug.MsgBox("13_00d1d", hsHanitLayers.Count)
		Return hsHanitLayers
   End Function
   Public Shared Function GetHanitLayersNew(bInclStage0 As Boolean) As HashSet(Of String)
      Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
      zzAddStageLayer(hsHanitLayers, AddressOf GetStagePolineLayer, bInclStage0)
      zzAddStageLayer(hsHanitLayers, AddressOf GetStageParcelCentroidLayer, False)
		zzAddStageLayer(hsHanitLayers, AddressOf GetStageFrontLineLayer, True)
		'DMCommon.Debug.MsgBox("13_001c", DMCommon.Debug.GetListArray(hsHanitLayers.Count - 1, hsHanitLayers))

		Return hsHanitLayers


      '   saLayer(0) = GetStagePolineLayer(iStageNo)
      '   saLayer(1) = GetStageParcelCentroidLayer(iStageNo)
      '  saLayer(2) = GetStageFrontLineLayer(iStageNo)

      '  saLayer(3) = GetStageUDPointLayer(iStageNo, False)
      '  saLayer(4) = GetStageUDPointLayer(iStageNo, True)
      '  saLayer(5) = GetStageCentroidLayer(iStageNo)



   End Function
   Public Shared Function GetStageCentroidLayer(iStageNo As Integer) As String

      Return "UD_StageCenter_" & CStr(iStageNo)
   End Function


   Public Shared Function GetStageParcelCentroidLayer(iStageNo As Integer) As String
      Return "C1603_" & CStr(iStageNo)
   End Function
   Public Shared Function GetStageUDPointLayer(iStageNo As Integer, bNew As Boolean) As String
      If bNew Then
         Return "C1611_" & CStr(iStageNo)
      Else
         Return "C1610_" & CStr(iStageNo)
      End If
   End Function
   Public Shared Function GetStageNewUDPointLayer(iStageNo As Integer) As String
      Return GetStageUDPointLayer(iStageNo, True)
   End Function
   Public Shared Function GetStageOldUDPointLayer(iStageNo As Integer) As String
      Return GetStageUDPointLayer(iStageNo, False)
   End Function
   Public Shared Function DrawHanitEntitiesNew() As System.Collections.ObjectModel.Collection(Of Integer)
      Dim sFrontLineLayerErr As String = "C1609_Err"
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

         Dim iStageNo As Integer = 0
         Dim bCurrentLayerOK As Boolean
         Dim sStageTopoName As String
         Dim sPolylineLayer As String
         Dim sBlockRefLayer As String ''
         Dim sFrontLineLayer As String
         Dim sCentroidLayer As String
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray

         '   oFragmentsTopo = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
         Dim oFragmentsTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("fragments")
         Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
         Dim colParcelCountsByStage As System.Collections.ObjectModel.Collection(Of Integer) = New System.Collections.ObjectModel.Collection(Of Integer)()
         Dim iParcelCount As Integer
         Dim oaStageTopoSchemes() As TopoManager.TopoScheme.tsTopology = Nothing
         If oFragmentsTopoScheme.Load(True) Then
            oFragmentsTopoScheme.CheckHugeArcRadius(100.0, Autodesk.AutoCAD.Colors.Color.FromColor(Drawing.Color.Red))
            '  Dim oPoint As UD_Point
            oFragmentsTopoScheme.MarkIsthmuses()

            Do
               sStageTopoName = GetStageTopoName(iStageNo)
               sBlockRefLayer = GetStageParcelCentroidLayer(iStageNo)
               sCentroidLayer = GetStageCentroidLayer(iStageNo)
               ''''''''''''''''''''''
               ' System.Windows.Forms.MessageBox.Show("Stage:  " & CStr(iStageNo), "!02_004")
               iParcelCount = mdicParcels.SetStageFragments(iStageNo)        '  uuuuuuuuuuuuuuuuu
               If iParcelCount = 0 Or iStageNo = 19 Then
                  miLastStageNo = iStageNo - 1
                  Exit Do
               Else
                  colParcelCountsByStage.Add(iParcelCount)
               End If
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sBlockRefLayer, DMAcadExt.DMApp.AppID, True, True)
               If iStageNo < -2 Then
                  System.Windows.Forms.MessageBox.Show("Stage: " & CStr(iStageNo), "!02_110")
               End If
               Dim colPolygonIDs As System.Collections.ObjectModel.Collection(Of Integer) = mdicParcels.DissolveByStageNew(oFragmentsTopoScheme, sStageTopoName, sCentroidLayer, sBlockRefLayer, iStageNo = 0)
               oStageTopoScheme = New TopoManager.TopoScheme.tsTopology(sStageTopoName)
               oStageTopoScheme.Load(True)
               ReDim Preserve oaStageTopoSchemes(iStageNo)
               oaStageTopoSchemes(iStageNo) = oStageTopoScheme
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sBlockRefLayer, DMAcadExt.DMApp.AppID, True, True)
               zzConnectPointsNodes(oStageTopoScheme)
               oStageTopoScheme.CheckPointCount()
               zzInsertSurveyPoints(oStageTopoScheme, iStageNo, True)  '!!!! 
               iStageNo += 1
            Loop

            For iStageNo = 0 To miLastStageNo

               oStageTopoScheme = oaStageTopoSchemes(iStageNo)
               sPolylineLayer = GetStagePolineLayer(iStageNo)
               sFrontLineLayer = GetStageFrontLineLayer(iStageNo)

               '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo) & vbCrLf & sPolylineLayer, "02_124")
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

               oStageTopoScheme.RemovePseudoPoints()

               '  07/03/2016    oStageTopoScheme.CreateDBPolylineMPlus(True, colPolygonIDs)   '!!!!!!! 
               oStageTopoScheme.CreateDBPolylineMPlus(True, False)  '!!!!!!!!!!! 

               oaResTplnPointArray = oStageTopoScheme.GetMidPointsNew(iStageNo)
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sFrontLineLayer, DMAcadExt.DMApp.AppID, True, True)

               zzInsertFLBlocks(oaResTplnPointArray, sFrontLineLayer, iStageNo)

               Dim oPgon As TopoManager.TopoScheme.tsPolygon
               For Each oParcel As UD_Parcel In mdicParcels.Values
                  If miLastStageNo = 0 OrElse oParcel.Stage = iStageNo Then   'OrElse (oParcel.Stage = -1 AndAlso iStageNo = 0)
                     oPgon = oStageTopoScheme.GetPolygon(oParcel.TopoID)
                     If oPgon IsNot Nothing Then
                        oParcel.PointLinks = oPgon.GetPointLinks()
                        oParcel.CompareArea(oPgon.AreaCorrected)
                        '  DMAcadExt.AcadDocument.WriteMessage("!-AREA: " & 1000 * oParcel.CalcArea.ToString & "; " & oPgon.AreaCorrected.ToString)
                     End If
                  End If
               Next oParcel
               oStageTopoScheme.Close()
               '   TopoM anager.TopoCreator.TopoToClosedPgons(sStageTopoName, True)
            Next iStageNo

            '    zzRemPoints()UD_GEN
            oFragmentsTopoScheme.Close()

            zzInsertRemFLBlocks(sFrontLineLayerErr)

            Dim sMsg As String = "Blocks: 'PCLS007', 'PCLS012' - " & CStr(moAttachFLinePoints.GetPointsCount(False)) & vbCrLf & "Block 'C1609', Layer 'C1609_Err' - " & CStr(moAttachFLinePoints.GetPointsCount(True))
            If miProblemBlocksCount <> 0 Then
               sMsg &= vbCrLf & "Arc/Line problems - " & CStr(miProblemBlocksCount)
            End If
            '   System.Windows.Forms.MessageBox.Show(sMsg, "15_209")
            DMAcadExt.AcadDocument.WriteMessageLog(sMsg)
            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(Auxiliary(1), DMAcadExt.DMApp.AppID, True, True) '"zzUnusedPoints"
            zzMarkUnusedPoints()

         End If
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.CloseLog()
         DMAcadExt.AcadDocument.UpdateScreen()

         Return colParcelCountsByStage
      Else
         Return Nothing
      End If
   End Function



   Public Shared Function DrawHanitEntities() As System.Collections.ObjectModel.Collection(Of Integer)
      'System.Windows.Forms.MessageBox.Show("DrawHanitLines", "02_050")
      Dim sFrontLineLayerErr As String = "C1609_Err"
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

         Dim iStageNo As Integer = 0
         Dim bCurrentLayerOK As Boolean
         Dim sStageTopoName As String
         Dim sPolylineLayer As String
         Dim sBlockRefLayer As String ''
         Dim sFrontLineLayer As String
         Dim sCentroidLayer As String
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray

         '   oFragmentsTopo = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
         Dim oFragmentsTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("fragments")
         Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
         Dim colParcelCountsByStage As System.Collections.ObjectModel.Collection(Of Integer) = New System.Collections.ObjectModel.Collection(Of Integer)()
         Dim iParcelCount As Integer
         Dim oaStageTopoSchemes() As TopoManager.TopoScheme.tsTopology = Nothing
         'System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_054")
         If oFragmentsTopoScheme.Load(True) Then
            oFragmentsTopoScheme.CheckHugeArcRadius(100.0, Autodesk.AutoCAD.Colors.Color.FromColor(Drawing.Color.Red))
            '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_055")
            '  oTopoScheme.PrintInfo()
            Do
               'System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_100")
               sStageTopoName = GetStageTopoName(iStageNo)

               sBlockRefLayer = GetStageParcelCentroidLayer(iStageNo)

               sCentroidLayer = GetStageCentroidLayer(iStageNo)
               ''''''''''''''''''''''
               ' System.Windows.Forms.MessageBox.Show("Stage: " & CStr(iStageNo), "!02_004")
               iParcelCount = mdicParcels.SetStageFragments(iStageNo)        '  uuuuuuuuuuuuuuuuu
               If iParcelCount = 0 Or iStageNo = 19 Then
                  miLastStageNo = iStageNo - 1
                  Exit Do
               Else
                  colParcelCountsByStage.Add(iParcelCount)
               End If


					bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sBlockRefLayer, DMAcadExt.DMApp.AppID, True, True)
               If iStageNo < -2 Then
                  System.Windows.Forms.MessageBox.Show("Stage: " & CStr(iStageNo), "!02_110")
               End If
               Dim colPolygonIDs As System.Collections.ObjectModel.Collection(Of Integer) = mdicParcels.DissolveByStageNew(oFragmentsTopoScheme, sStageTopoName, sCentroidLayer, sBlockRefLayer, iStageNo = 0)

               If iStageNo = 12 Then
                  System.Windows.Forms.MessageBox.Show(CStr(colPolygonIDs.Count), "!02_120")
               End If

					'  bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
					' System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & bCurrentLayerOK.ToString(), "04_418")



					oStageTopoScheme = New TopoManager.TopoScheme.tsTopology(sStageTopoName)
               oStageTopoScheme.Load(True)
               If iStageNo = 0 Then
                  ''''''''  oStageTopoScheme.PrintInfo(40)
               End If
               ReDim Preserve oaStageTopoSchemes(iStageNo)
               oaStageTopoSchemes(iStageNo) = oStageTopoScheme


               '     oStageTopoScheme.PrintInfo(37)

               '    DMAcadExt.AcadDocument.WriteMessage("!0001_01")
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sBlockRefLayer, DMAcadExt.DMApp.AppID, True, True)
               'DMAcadExt.AcadDocument.WriteMessage("!0001_02")
               '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_125")

               '  DMAcadExt.AcadDocument.WriteMessage("!0001_03 " & sPolylineLayer)

               'DMAcadExt.AcadDocument.WriteMessage("!0001_04")
               zzConnectPointsNodes(oStageTopoScheme)
               oStageTopoScheme.CheckPointCount()

               zzInsertSurveyPoints(oStageTopoScheme, iStageNo, True)  '!!!!!!!!! 


               ' System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_124")


               '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo), "02_140")
               iStageNo += 1

            Loop
            Dim colLinks As ObjectIdCollection = oFragmentsTopoScheme.GetOuterBoundary()

            TopoManager.TopoCreator.CreateTopology("OuterBoundary", colLinks, String.Empty, String.Empty, True, False, 0.01)
            For iStageNo = 0 To miLastStageNo

               oStageTopoScheme = oaStageTopoSchemes(iStageNo)
               sPolylineLayer = GetStagePolineLayer(iStageNo)
               sFrontLineLayer = GetStageFrontLineLayer(iStageNo)

               '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo) & vbCrLf & sPolylineLayer, "02_124")
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

               oStageTopoScheme.RemovePseudoPoints()

               '  07/03/2016    oStageTopoScheme.CreateDBPolylineMPlus(True, colPolygonIDs)   '!!!!!!!!!!!! 
               oStageTopoScheme.CreateDBPolylineMPlus(True, False)   '!!!!!!!!!! 


               oaResTplnPointArray = oStageTopoScheme.GetMidPoints()
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sFrontLineLayer, DMAcadExt.DMApp.AppID, True, True)

               '    System.Windows.Forms.MessageBox.Show("Stage: " & CStr(iStageNo) & vbCrLf & CStr(oaResTplnPointArray.Count), "02_052")
               zzInsertFLBlocks(oaResTplnPointArray, sFrontLineLayer, iStageNo)
               Dim oPgon As TopoManager.TopoScheme.tsPolygon
               For Each oParcel As UD_Parcel In mdicParcels.Values

                  If miLastStageNo = 0 OrElse oParcel.Stage = iStageNo Then   'OrElse (oParcel.Stage = -1 AndAlso iStageNo = 0)
                     ' System.Windows.Forms.MessageBox.Show("oParcel.TopoID: " & CStr(oParcel.TopoID), "!02_311")
                     oPgon = oStageTopoScheme.GetPolygon(oParcel.TopoID)
                     If oPgon IsNot Nothing Then
                        oParcel.PointLinks = oPgon.GetPointLinks()
                        oParcel.CompareArea(oPgon.AreaCorrected)
                        '  DMAcadExt.AcadDocument.WriteMessage("!-AREA: " & 1000 * oParcel.CalcArea.ToString & "; " & oPgon.AreaCorrected.ToString)
                     End If
                  End If
               Next

               ' DMAcadExt.AcadDocument.WriteMessage("!0001_05")


               oStageTopoScheme.Close()

               '   TopoManager.TopoCreator.TopoToClosedPgons(sStageTopoName, True)


            Next

            '    zzRemPoints()UD_GEN
            oFragmentsTopoScheme.Close()

            zzInsertRemFLBlocks(sFrontLineLayerErr)

            Dim sMsg As String = "Blocks: 'PCLS007', 'PCLS012' - " & CStr(moAttachFLinePoints.GetPointsCount(False)) & vbCrLf & "Block 'C1609', Layer 'C1609_Err' - " & CStr(moAttachFLinePoints.GetPointsCount(True))
            If miProblemBlocksCount <> 0 Then
               sMsg &= vbCrLf & "Arc/Line problems - " & CStr(miProblemBlocksCount)
            End If
            '   System.Windows.Forms.MessageBox.Show(sMsg, "15_209")
            DMAcadExt.AcadDocument.WriteMessageLog(sMsg)



            '  System.Windows.Forms.MessageBox.Show("Block 'PCLS007': " & moAttachPoints.GePointsCount(True) & " of " & moAttachPoints.GePointsCount(False) & vbCrLf & "Blocks SRVS006/7: " & moAttachSurveyPoints.GePointsCount(True) & " of " & moAttachSurveyPoints.GePointsCount(False) & vbCrLf & CStr(moAttachSurveyPoints.PointDataCount) & vbCrLf & CStr(moAttachSurveyPoints.EntriesCount) & vbCrLf & CStr(moAttachSurveyPoints.FailsCount), "Blocks Err")
            '  System.Windows.Forms.MessageBox.Show(CStr(miTestCounterA) & vbCrLf & "Aa=" & CStr(miTestCounterAa) & vbCrLf & CStr(miTestCounterB) & vbCrLf & "Ba=" & CStr(miTestCounterBa) & vbCrLf & CStr(miTestCounterC) & vbCrLf & "Ca=" & CStr(miTestCounterCa) & vbCrLf & "D=" & CStr(miTestCounterD) & vbCrLf & "Da=" & CStr(miTestCounterDa) & vbCrLf & "Db=" & CStr(miTestCounterDb) & vbCrLf & "E=" & CStr(miTestCounterE) & vbCrLf & "F=" & CStr(miTestCounterF) & vbCrLf & "Fa=" & CStr(miTestCounterFa) & vbCrLf & "G=" & CStr(miTestCounterG) & vbCrLf & "Ga=" & CStr(miTestCounterGa) & vbCrLf & "Gb=" & CStr(miTestCounterGb) & vbCrLf & "Gc=" & CStr(miTestCounterGc) & vbCrLf & "H=" & CStr(miTestCounterH) & vbCrLf & "I=" & CStr(miTestCounterI) & vbCrLf & "J=" & CStr(miTestCounterJ) & vbCrLf & "K=" & CStr(miTestCounterK))
            '	oFragmentsTopo.Dispose()

            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(Auxiliary(1), DMAcadExt.DMApp.AppID, True, True) '"zzUnusedPoints"
            zzMarkUnusedPoints()

         End If
         ' mdicParcels.Clear()
         '    '''''''''''' mdicPoints.Clear()
         'mdicParcels=Nothing
         '''''''''''''''''  mdicPoints = Nothing
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.CloseLog()
         DMAcadExt.AcadDocument.UpdateScreen()

         Return colParcelCountsByStage
      Else
         Return Nothing
      End If
   End Function

   Public Shared Sub GetParcelsString(ByRef sNormal As String, ByRef sTemp As String)
      If mdicParcels IsNot Nothing Then
         mdicParcels.GetString(miLastStageNo = 0, sNormal, sTemp)
      Else
         sNormal = String.Empty
         sTemp = String.Empty

      End If

   End Sub
   Public Shared Function GetBook() As String

      Return mdicParcels.GetBook()
   End Function
   Public Shared Function GetResult() As String
      Dim iOperationNo As Integer = 0

      Dim sResText As String = String.Empty

      ' Dim oParcel As UD_Parcel
      Dim iaBookLineFormat() As Integer = {8, 11, 14, 14, 15}
      Dim iaBookLineTransferFormat() As Integer = {8, 11, 14, 14, 15, 12, 15, 15}
      Dim iaPointLineFormat() As Integer = {8, 15, 15, 15, 15}
      Dim oTextLine As DMCommon.dmTextLine

      Dim sHeader1 As String = " T number   F number   Calc area     area Diff'     Final area"
      Dim sHeader2 As String = " --------   --------   ----------    ----------     ----------"
      Dim sBottomDel As String = " -------------------------------------------------------------"
      Dim sTransferHeader1 As String = " T number   F number   Calc area     area Diff'     Final area       Block         TN number     FN number"
      Dim sTransferHeader2 As String = " --------   --------   ----------    ----------     ----------     ----------     ----------     ----------"
      Dim sTransferBottomDel As String = " ----------------------------------------------------------------------------------------------------------"
      Dim sGroupHeader1 As String = "Point            E              N              Rad            Length"
      Dim sGroupHeader2 As String = "--------------------------------------------------------------------"


      Dim sRepCaption As String
      Dim oFileInfo As IO.FileInfo
      Dim iTableNo As Integer = 0
      'File name:Tr_book.txt     Place:  Gush:  Print Date:12/10/2015
      ' 
      oFileInfo = New IO.FileInfo(DMAcadExt.AcadDocument.GetFileName())
      sRepCaption = "File name:" & oFileInfo.Name & " Print Date:" & FormatDateTime(Date.Today, DateFormat.ShortDate)
      sResText &= sRepCaption


      '  Dim oLink As DMAcadExt.UD_Link
      '   System.Windows.Forms.MessageBox.Show(CStr(mcolTransferBlocks.Count) & vbCrLf & "", "09_003")

      '  System.Windows.Forms.MessageBox.Show(sResText, "09_200")
      '  mdicParcels.Keys
      Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = mdicParcels.GetSorted()
      Dim colPointLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink)
      '  System.Windows.Forms.MessageBox.Show(miLastStageNo.ToString(), "09_210")

      For Each oParcel As UD_Parcel In colRes

         If (miLastStageNo = 0 AndAlso Not oParcel.IsOut) OrElse oParcel.Stage <> -1 Then
            sResText &= vbCrLf & "LOT:  " & oParcel.ParcelKey.ToString() & "   " & oParcel.GetCenter() & "   Area =  " & 1000 * oParcel.CalcArea
            colPointLinks = oParcel.PointLinks
            If colPointLinks IsNot Nothing Then
               ' System.Windows.Forms.MessageBox.Show(CStr(colPointLinks.Count) & vbCrLf & oParcel.Name, "09_220")
               '  sResText &= vbCrLf & CStr(oParcel.PointLinks.Count)
               sResText &= vbCrLf & vbCrLf & sGroupHeader1
               sResText &= vbCrLf & sGroupHeader2
               '  System.Windows.Forms.MessageBox.Show(oParcel.PointLinks.Count.ToString(), "09_215")
               For Each oPointLink As DMAcadExt.PointLink In oParcel.PointLinks
                  If oPointLink.Link IsNot Nothing AndAlso Not String.IsNullOrEmpty(oPointLink.PointName) Then
                     oTextLine = New DMCommon.dmTextLine(iaPointLineFormat)
                     Try
                        oTextLine.FieldValue(0) = oPointLink.PointName
                     Catch oEx As Exception
                        oTextLine.FieldValue(0) = "xx"
                     End Try

                     oTextLine.FieldValue(1) = FormatNumber(oPointLink.Link.StartPoint.X, 3, TriState.True, TriState.False, TriState.False)
                     oTextLine.FieldValue(2) = FormatNumber(oPointLink.Link.StartPoint.Y, 3, TriState.True, TriState.False, TriState.False)
                     oTextLine.FieldValue(3) = FormatNumber(oPointLink.Link.Radius, 3, TriState.True, TriState.False, TriState.False)
                     oTextLine.FieldValue(4) = FormatNumber(oPointLink.Link.Length, 3, TriState.True, TriState.False, TriState.False)
                     sResText &= vbCrLf & oTextLine.LineValue ''''''''''''''' & " " & oPointLink.Link.StartPoint.ToString()
                  End If
               Next
               sResText &= vbCrLf & vbCrLf
            Else
               System.Windows.Forms.MessageBox.Show("colPointLinks IsNot Nothing", "09_220")
            End If
         End If
      Next
      '  " LA=" & oParcel.LegalArea & " C=" & oParcel.CalcArea
      Return sResText
   End Function
   Public Shared ReadOnly Property BlockPath As String
      Get
         Return msBlockPath
      End Get
   End Property
   Public Shared ReadOnly Property BlockPath13 As String
      Get
         Return TopoManager.TPlanGraph.TplnProject.BlockFolder & "\" & msHanit13Folder
      End Get
      
   End Property
   Public Shared ReadOnly Property Parcels As UD_Parcels
      Get
         Return mdicParcels
      End Get
   End Property
   Public Shared ReadOnly Property FragmentsToposcheme As TopoManager.TopoScheme.tsTopology
      Get
         Return moFragmentsToposcheme
      End Get
   End Property
   Public Shared Function ParcelListIsNotEmpty() As Boolean
      Return mdicParcels IsNot Nothing AndAlso mdicParcels.Count > 0
   End Function
   Public Shared Function GetPointsSrv() As String
      Dim sResText As String = String.Empty

      Dim oTextLine As DMCommon.dmTextLine
      Dim oTplnPoint As DMAcadExt.TPlnPoint
      '  System.Windows.Forms.MessageBox.Show("colPoints.Count.ToString()", "09_207b")
      Dim colPoints As System.Collections.ObjectModel.Collection(Of UD_Point) = mdicPoints.GetSortedArray()
      If colPoints.Count > 0 Then
         '  System.Windows.Forms.MessageBox.Show(colPoints.Count.ToString(), "09_207")
         For Each oPoint As UD_Point In colPoints
            oTplnPoint = oPoint.Point
            oTextLine = New DMCommon.dmTextLine(" ")
            oTextLine.AddValue("POINT")
            oTextLine.AddValue(oPoint.Name)
            oTplnPoint = oPoint.Point
            oTextLine.AddValue(oTplnPoint.GetXFormated(3))
            oTextLine.AddValue(oTplnPoint.GetYFormated(3))
            oTextLine.AddAttribute("S", oPoint.SCode)
            oTextLine.AddAttribute("C", oPoint.CCode)
            oTextLine.AddAttribute("M", oPoint.MCode)
            oTextLine.Close(";")
            If sResText.Length <> 0 Then
               sResText &= vbCrLf
            End If
            sResText &= oTextLine.LineValue
         Next
         sResText &= vbCrLf

      End If
      Return sResText
   End Function

   Public Shared Function GetParcelsAreaTable() As System.Data.DataView
      Dim iOperationNo As Integer = 0

      Dim sResText As String = String.Empty

      Dim iTableNo As Integer = 0
      Dim oParcelsTable As System.Data.DataTable
      Dim oDataColumn As System.Data.DataColumn

      '  Dim oLink As DMAcadExt.UD_Link
      '   System.Windows.Forms.MessageBox.Show(CStr(mcolTransferBlocks.Count) & vbCrLf & "", "09_003")


      '  mdicParcels.Keys

      oParcelsTable = New System.Data.DataTable("Lines")




      oDataColumn = New System.Data.DataColumn("ParcelName", GetType(System.String))
      oParcelsTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("LegalArea", GetType(System.Double))
      oParcelsTable.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn("CalcArea", GetType(System.Double))
      oParcelsTable.Columns.Add(oDataColumn)
      ' oDataColumn = New System.Data.DataColumn("Deviation", GetType(System.Double))
      ' oParcelsTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("DeltaArea", GetType(System.Double))
      oParcelsTable.Columns.Add(oDataColumn)


      oDataColumn = New System.Data.DataColumn("IsProper", GetType(System.String))
      oParcelsTable.Columns.Add(oDataColumn)


      Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = mdicParcels.GetSorted()

      If colRes IsNot Nothing Then


         For Each oParcel As UD_Parcel In colRes
            oParcelsTable.NewRow()

            Dim oNewRow As System.Data.DataRow = oParcelsTable.NewRow()
            oNewRow.Item("ParcelName") = oParcel.UD_Name

            oNewRow.Item("LegalArea") = oParcel.LegalArea
            oNewRow.Item("CalcArea") = oParcel.CalcArea

            '   oNewRow.Item("Deviation") = oParcel.Tolerance
            oNewRow.Item("DeltaArea") = oParcel.DeltaArea
            oNewRow.Item("IsProper") = oParcel.ParcelArea.IsProperText
            oParcelsTable.Rows.Add(oNewRow)

         Next
         '  " LA=" & oParcel.LegalArea & " C=" & oParcel.CalcArea
         Return New System.Data.DataView(oParcelsTable)
      Else
         Return Nothing
      End If

   End Function
   Public Shared Sub ErasePoints()
      If mdicPoints IsNot Nothing Then
         For Each oUD_Point As UD_Point In mdicPoints.Values
            If oUD_Point.Stage = -1 Then
               If Not oUD_Point.AcObjID.IsNull Then
                  DMAcadExt.AcadTransaction.EraseDBObject(oUD_Point.AcObjID)
               End If
            End If
         Next
      End If
   End Sub
   Public Shared Sub EraseMarkEntities()

      If mdicMarkEntities IsNot Nothing AndAlso mdicMarkEntities.Count <> 0 Then
         System.Windows.Forms.MessageBox.Show(CStr(mdicMarkEntities.Count), "04_044")
         For Each tAcObjID As ObjectId In mdicMarkEntities
            DMAcadExt.AcadTransaction.EraseDBObject(tAcObjID)
         Next
      End If

   End Sub


   Private Shared Sub zzMarkUnusedPoints()
      Dim sNewLayer As String
      Dim bCurrentLayerOK As Boolean
      moCircleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
      mdicMarkEntities = New ObjectIdCollection()
      For Each oUD_Point As UD_Point In mdicPoints.Values
         If oUD_Point.Stage = -1 Then
            zzDrawMarkCircleNew(oUD_Point.Point.AcGePoint3d, 31S)

         ElseIf oUD_Point.IsScipped Then
            If oUD_Point.IsUserBlocking Then
               sNewLayer = GetStageUDPointLayer(oUD_Point.Stage, True)
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, DMAcadExt.DMApp.AppID, True, True)
               zzInsertSurveyPoint(moNewPointBlock, oUD_Point)
            Else
               zzDrawMarkCircleNew(oUD_Point.Point.AcGePoint3d, 31S)
            End If



         End If
      Next
   End Sub
   Private Shared Sub zzDrawMarkCircleNew(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByVal shColor As Short)
      moCircleMarkBlock.MarkPoint(tPoint, shColor)

      '  Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle = New Circle()
      Dim tMarkEntityObjID As ObjectId

      '  oCircle.Center = tPoint
      '  oCircle.Radius = 1.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor
      Try
         '  tMarkEntityObjID = DMAcadExt.AcadTransaction.AppendEntity(oCircle, False)
         If Not tMarkEntityObjID.IsNull Then
            mdicMarkEntities.Add(tMarkEntityObjID)
         End If

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "UD_App - zzDrawMarkCircle")
      End Try


      '  oAcobjId = oBlockTableRecord.AppendEntity(oCircle)
      '  sTest = "g"
      '  oTransactionManager.AddNewlyCreatedDBObject(oCircle, True)
   End Sub

   Private Shared Sub zzDrawMarkCircle(tPoint As Autodesk.AutoCAD.Geometry.Point3d)
      Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle = New Circle()
      Dim tMarkEntityObjID As ObjectId

      oCircle.Center = tPoint
      oCircle.Radius = 1.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor
      Try
         tMarkEntityObjID = DMAcadExt.AcadTransaction.AppendEntity(oCircle, False)
         If Not tMarkEntityObjID.IsNull Then
            mdicMarkEntities.Add(tMarkEntityObjID)
         End If

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "UD_App - zzDrawMarkCircle")
      End Try


      '  oAcobjId = oBlockTableRecord.AppendEntity(oCircle)
      '  sTest = "g"
      '  oTransactionManager.AddNewlyCreatedDBObject(oCircle, True)
   End Sub
   Private Shared Function zzGetPrevFlineData(sLineName As String, ByRef tPrevBlockRefData As DMAcadExt.BlockRefData) As Boolean
      If mdicPrevFLines IsNot Nothing Then
         If mdicPrevFLines.TryGetValue(sLineName, tPrevBlockRefData) Then
            Return True
         Else
            Dim saVal() As String = Split(sLineName, ",")
            If saVal.GetUpperBound(0) = 1 Then
               sLineName = saVal(1) & "," & saVal(0)
               If mdicPrevFLines.TryGetValue(sLineName, tPrevBlockRefData) Then
                  Return True
               End If
            End If
         End If
      End If

      Return False
   End Function
   Private Shared Sub zzInsertFLBlocks(oaResTplnPointArray As DMAcadExt.TplnPointArray, sFrontLineLayer As String, iStageNo As Integer)


      '    System.Windows.Forms.MessageBox.Show(CStr(oaResTplnPointArray.UpperBound) & vbCrLf & "", "05_601")
      moAttachFLinePoints.SourcePoints = oaResTplnPointArray
      moAttachFLinePoints.Calculate(False)
      Dim oaBlockRefs() As ObjectId = moAttachFLinePoints.OutputAcadObjects
      Dim oArcLineNames() As System.Object = moAttachFLinePoints.OutputObjects
      Dim oArcLineName As Tuple(Of String, Boolean)
      Dim oOutputStatuses() As DMAcadExt.AttachEntities.enStatus = moAttachFLinePoints.OutputStatuses
      '   Dim dicTest As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs("C1609", "C1609_" & CStr(iStageNo))
      '  System.Windows.Forms.MessageBox.Show(CStr(dicTest.Count) & vbCrLf & CStr(oOutputStatuses.GetUpperBound(0)) & vbCrLf & CStr(oArcLineNames.GetUpperBound(0)) & vbCrLf & CStr(oaBlockRefs.GetUpperBound(0)) & vbCrLf & CStr(moAttachFLinePoints.GetPointsCount(True)), "05_612")
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("PCLS007")
      '   moAttachFLinePoints.PointTest()
      Dim tPrevSourceBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim tSourceBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()

      Dim saFields() As String = {"FRONT-LEN"}
      Dim saAttribValues() As String
      Dim sLineName As String
      Dim bIsArc As Boolean
      oAcadBlock.Fields = saFields
      oAcadBlock.OpenForRead()

      Dim oFrontLineBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(FrontLineBlockName, msBlockPath)
      oFrontLineBlock.Fields = {"LINE_NAME", "RADIUS", "LEGAL_LENGTH", "CALC_LENGTH", "TOPO", "COMMENT"}
      oFrontLineBlock.OpenForRight()
      Dim tDestBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim tBlockRefObjID As ObjectId


      DMAcadExt.AcadTransaction.SetCurrentLayer(Auxiliary(2), DMAcadExt.DMApp.AppID, True, True) '"zzFLinePoint"
      Dim iRemCount As Integer = 0
      Dim tPrevBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim bSourceBlockForArc As Boolean
      Dim bPrevExists As Boolean
      Dim dPrevCalcLenValue As Double
      Dim dPrevLegalLenValue As Double
      Dim sPrevLegalLenValue As String = String.Empty


      Dim dCalcLenValue As Double
      '   Dim iRow As Integer

      miProblemBlocksCount = 0
      moRhombusMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Rhombus)
      '  oArcLineName As Tuple(Of String, Boolean)
      '   DMCommon.ExcelLog.SetNextValue(iRow, 0, "---+----+-------+---+---", oaBlockRefs.GetUpperBound(0))
      For iIndex As Integer = 0 To oaBlockRefs.GetUpperBound(0)
         tBlockRefObjID = oaBlockRefs(iIndex)
         ' DMCommon.ExcelLog.SetNextValue(iRow, 0, iRow, tBlockRefObjID.ToString())
         If oArcLineNames(iIndex) IsNot Nothing Then
            oArcLineName = DirectCast(oArcLineNames(iIndex), Tuple(Of String, Boolean))
            sLineName = oArcLineName.Item1
            bIsArc = oArcLineName.Item2
         Else
            sLineName = String.Empty
         End If
         '  System.Windows.Forms.MessageBox.Show(CStr(oaResTplnPointArray.UpperBound) & vbCrLf & sLineName, "05_484")
         '  System.Windows.Forms.MessageBox.Show(CStr(oOutputStatuses(iIndex)) & vbCrLf & sLineName, "05_484X")

         If oOutputStatuses(iIndex) = DMAcadExt.AttachEntities.enStatus.OneEntry Then
            ' System.Windows.Forms.MessageBox.Show(CStr("") & vbCrLf & "sLineName", "05_485")
            bPrevExists = zzGetPrevFlineData(sLineName, tPrevSourceBlockRefData)
            '   DMCommon.ExcelLog.SetNextValue(iRow, 5, sLineName, dPrevLegalLenValue, dPrevCalcLenValue)
            If bPrevExists Then
               dPrevLegalLenValue = zzAttribTextToDouble(tPrevSourceBlockRefData.GetAttribValue("LEGAL_LENGTH"))
               dPrevCalcLenValue = zzAttribTextToDouble(tPrevSourceBlockRefData.GetAttribValue("CALC_LENGTH"))
            Else
               sPrevLegalLenValue = String.Empty
            End If
            '   DMCommon.ExcelLog.SetNextValue(iRow, 6, dPrevLegalLenValue, dPrevCalcLenValue)

            tSourceBlockRefData = oAcadBlock.GetBlockRefData(tBlockRefObjID)
            dCalcLenValue = zzAttribTextToDouble(tSourceBlockRefData.AttribValues(0))

            Select Case UCase(DMCommon.Functions.CStrN(tSourceBlockRefData.Layer, "N"))
               Case "PCLS007"
                  bSourceBlockForArc = False
               Case "PCLS012"
                  bSourceBlockForArc = True
               Case Else
                  System.Windows.Forms.MessageBox.Show(CStr("NOT 'PCLS007', 'PCLS012'") & vbCrLf & "'" & tSourceBlockRefData.Layer & "'", "05_998")

                  ' tDestBlockRefData = New DMAcadExt.BlockRefData()
            End Select
            If bPrevExists Then
               If Math.Abs(dPrevCalcLenValue - dCalcLenValue) >= 0.0001 Then
                  If dPrevLegalLenValue > 0.005 Then
                     sPrevLegalLenValue = FormatNumber(dPrevLegalLenValue, 2)
                  Else
                     sPrevLegalLenValue = String.Empty
                  End If

                  '      DMCommon.ExcelLog.SetNextValue(iRow, 1, dPrevCalcLenValue, dCalcLenValue, Math.Abs(dPrevCalcLenValue - dCalcLenValue))
                  ''''''''''  tSourceBlockRefData.SetAttribValue("LEGAL_LENGTH", tPrevSourceBlockRefData.GetAttribValue(2))
                  bPrevExists = False
                  moRhombusMarkBlock.MarkPoint(tSourceBlockRefData.Position, 2S)
               Else
                  tSourceBlockRefData.Layer = sFrontLineLayer
                  oFrontLineBlock.InsertRefAttrib(tPrevSourceBlockRefData, True, True, False)
               End If
            ElseIf mbHasPreviewVersion Then
               moRhombusMarkBlock.MarkPoint(tSourceBlockRefData.Position, 3S)
            End If
            'If bSourceBlockForArc Or bIsArc Then
            '   DMAcadExt.AcadDocument.WriteMessage("#45!: " & CStr(bSourceBlockForArc) & "|" & CStr(bIsArc) & " " & oOutputStatuses(iIndex).ToString() & " " & tSourceBlockRefData.Layer.ToString() & " " & tSourceBlockRefData.Position.ToString())
            'End If
            If Not bPrevExists AndAlso bSourceBlockForArc = bIsArc Then
               saAttribValues = tSourceBlockRefData.AttribValues
               tDestBlockRefData.Position = tSourceBlockRefData.Position
               tDestBlockRefData.Rotation = tSourceBlockRefData.Rotation
               tDestBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale() 'tSourceBlockRefData.ScaleFactors
               tDestBlockRefData.Layer = sFrontLineLayer
               Select Case UCase(tSourceBlockRefData.Layer)
                  Case "PCLS007"
                     tDestBlockRefData.AttribValues = {sLineName, String.Empty, sPrevLegalLenValue, saAttribValues(0), String.Empty, String.Empty}
                  Case "PCLS012"
                     tDestBlockRefData.AttribValues = {sLineName, saAttribValues(0), sPrevLegalLenValue, String.Empty, String.Empty, String.Empty}
                  Case Else
                     System.Windows.Forms.MessageBox.Show(CStr("NOT 'PCLS007', 'PCLS012'") & vbCrLf & "'" & tSourceBlockRefData.Layer & "'", "05_999")
                     ' tDestBlockRefData = New DMAcadExt.BlockRefData()
               End Select
               oFrontLineBlock.InsertRef(tDestBlockRefData)
            ElseIf bSourceBlockForArc <> bIsArc Then
               moRhombusMarkBlock.MarkPoint(tSourceBlockRefData.Position, 1S)
               miProblemBlocksCount += 1
               ' System.Windows.Forms.MessageBox.Show(CStr("bSourceBlockForArc <> bIsArc") & vbCrLf & "", "05_998")
            End If

            '\\\\\\\\\\\\\\xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
         ElseIf oOutputStatuses(iIndex) = DMAcadExt.AttachEntities.enStatus.NotFound Then
            '  System.Windows.Forms.MessageBox.Show(CStr("____________") & vbCrLf & "sLineName", "05_486")
            iRemCount += 1
            ''''''''''''''''  DMAcadExt.AcadTransaction.InsertPoint(oaResTplnPointArray.ItemObject(iIndex).Point.AcGePoint)
            '  DMAcadExt.AcadDocument.WriteMessage("FL " & CStr(oaResTplnPointArray.Item(iIndex).Coordinates))
         End If
      Next
      '  dicTest = DMAcadExt.AcadTransaction.GetBlockRefs("C1609", "C1609_" & CStr(iStageNo))
      '   System.Windows.Forms.MessageBox.Show(CStr(iRemCount) & vbCrLf & CStr(miProblemBlocksCount) & vbCrLf & CStr(oOutputStatuses.GetUpperBound(0)) & vbCrLf & CStr(oArcLineNames.GetUpperBound(0)) & vbCrLf & CStr(oaBlockRefs.GetUpperBound(0)) & vbCrLf & CStr(moAttachFLinePoints.GetPointsCount(True)), "05_612")
      '    System.Windows.Forms.MessageBox.Show(CStr(moAttachFLinePoints.GetPointsCount(True)) & vbCrLf & CStr(moAttachFLinePoints.GetPointsCount(True)) & vbCrLf & CStr(oaResTplnPointArray.UpperBound) & ":" & CStr(oaBlockRefs.GetUpperBound(0)) & vbCrLf & CStr(iRemCount), "15_199")
   End Sub
   Private Shared Sub zzInsertRemFLBlocks(sFrontLineLayer As String)
      Dim colEntities As ObjectIdCollection = moAttachFLinePoints.GePointsEntityCol(True)
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("PCLS007")
      Dim tSourceBlockRefData As DMAcadExt.BlockRefData
      Dim saFields() As String = {"FRONT-LEN"}
      Dim saAttribValues() As String
      Dim tDestBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      DMAcadExt.AcadTransaction.SetCurrentLayer(sFrontLineLayer, DMAcadExt.DMApp.AppID, True, True)

      '  Dim tBlockRefObjID As ObjectId

      oAcadBlock.Fields = saFields
      oAcadBlock.OpenForRead()

      Dim oFrontLineBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(FrontLineBlockName, msBlockPath)
      oFrontLineBlock.Fields = {"LINE_NAME", "RADIUS", "LEGAL_LENGTH", "CALC_LENGTH", "TOPO", "COMMENT"}
      oFrontLineBlock.OpenForRight()

      For Each tBlockRefObjID As ObjectId In colEntities
         tSourceBlockRefData = oAcadBlock.GetBlockRefData(tBlockRefObjID)
         saAttribValues = tSourceBlockRefData.AttribValues
         tDestBlockRefData.Position = tSourceBlockRefData.Position
         tDestBlockRefData.Rotation = tSourceBlockRefData.Rotation
         tDestBlockRefData.ScaleFactors = tSourceBlockRefData.ScaleFactors

         tDestBlockRefData.Layer = sFrontLineLayer
         Select Case UCase(tSourceBlockRefData.Layer)
            Case "PCLS007"
               tDestBlockRefData.AttribValues = {String.Empty, String.Empty, String.Empty, saAttribValues(0), String.Empty, String.Empty}
            Case "PCLS012"
               tDestBlockRefData.AttribValues = {String.Empty, saAttribValues(0), String.Empty, String.Empty, String.Empty, String.Empty}
            Case Else
               ' tDestBlockRefData = New DMAcadExt.BlockRefData()
         End Select
         oFrontLineBlock.InsertRef(tDestBlockRefData)
      Next
   End Sub
   Private Shared Function zzGetMidPoint(oLine As Line) As DMAcadExt.TPlnPoint
      Return New DMAcadExt.TPlnPoint(oLine.StartPoint, oLine.EndPoint)
   End Function
   Private Shared Function zzGetMidPoint(oArc As Arc) As DMAcadExt.TPlnPoint
      Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc)
      Dim tPoint2d As Autodesk.AutoCAD.Geometry.Point2d = oTplnArc.GetMidPoint()
      Return New DMAcadExt.TPlnPoint(tPoint2d)
   End Function
   Private Shared Function zzCreateFinalTopology(sFinalTopoName As String, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = zzGetAllParcelCentroids()

      Try
         '   DMCommon.Debug.MsgBox("15_125", sFinalTopoName & ":=" & CStr(colTopoLinks.Count), colCentroids.Count)
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         '      DMAcadExt.AcadTransaction.SetLayer(colCentroids, "zzDebug")


         oTopos.Create(sFinalTopoName, colTopoLinks, mcolAllNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "CreateFinalTopology")

         Return False
      End Try
      '   DMCommon.Debug.MsgBox("09_774", sFinalTopoName, oTopos.Exists(sFinalTopoName))
      Return oTopos.Exists(sFinalTopoName)
   End Function
   Private Shared Function zzGetAllParcelCentroids() As ObjectIdCollection
      Dim colAcObjIDs As ObjectIdCollection = New ObjectIdCollection()
      For Each oParcel As UD_Parcel In mdicParcels.Values
         If oParcel.IsResult Then
            If oParcel.CentroidAcObjID.IsNull Then
               DMCommon.Debug.MsgBox("09_214", oParcel.ParcelKey)
            Else
               colAcObjIDs.Add(oParcel.CentroidAcObjID)
            End If

         End If
      Next
      ''''''''''''   mdicParcels.DebugMsg("Final")
      '    DMCommon.Debug.MsgBox("09_222", colAcObjIDs.Count, mdicParcels.Count)
      Return colAcObjIDs
   End Function

   Private Shared Sub zzConnectPointsNodes(oStageTopoScheme As TopoManager.TopoScheme.tsTopology)
      Dim oPoint As UD_Point = Nothing
      For Each oNodeScheme As TopoManager.TopoScheme.tsNode In oStageTopoScheme.Nodes
         If mdicPoints.TryGetPoint(oNodeScheme.Location, oPoint) Then

            If oPoint.IsUserBlocking Then
               oNodeScheme.IsUserBlocking = True
            End If
            If oPoint.IsOld Then
               oNodeScheme.HasOldPoint = True
            End If
            oNodeScheme.Name = oPoint.Name
            oNodeScheme.NodeProperty = oPoint
         End If
      Next
   End Sub
   Private Shared Sub zzInsertSurveyPoints(oStageTopoScheme As TopoManager.TopoScheme.tsTopology, iStageNo As Integer, bWithTolerance As Boolean)
      Dim sOldLayer As String = GetStageUDPointLayer(iStageNo, False) ' "C1610_" & CStr(iStageNo)
      Dim sNewLayer As String = GetStageUDPointLayer(iStageNo, True)       '    "C1611_" & CStr(iStageNo)
      Dim sOldHorizontalLayer As String = "C1615"
      Dim sOldVerticalLayer As String = "C1616"
      Dim sFrontLineLayer As String = GetStageFrontLineLayer(iStageNo)
      Dim bCurrentLayerOK As Boolean
      Dim oOldPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(OldPointBlockName, msBlockPath)
      moNewPointBlock = New DMAcadExt.AcadBlock(NewPointBlockName, msBlockPath)
      Dim oOldHorizontalPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(OldHorizontalPointBlockName, msBlockPath)
      Dim oOldVerticalPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(OldVerticalPointBlockName, msBlockPath)
      Dim saPointFields() As String = {"POINT_NAME", "UID", "MARK", "TOPO", "SOURCE", "CLASS", "HEIGHT"}
      ' Dim oNodeScheme As TopoManager.TopoScheme.tsNode
      '     Dim bHasOldPoint As Boolean
      Dim bIsMustPoint As Boolean

      Dim bPointExists As Boolean
      Dim bPointNotUsed As Boolean
      Dim tBlockRefData As DMAcadExt.BlockRefData
      Dim oPoint As UD_Point = Nothing
      Dim sPointData As String
      moTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
      moSquareMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
      oOldPointBlock.Fields = saPointFields
      oOldPointBlock.OpenForRight()
      oOldHorizontalPointBlock.Fields = saPointFields
      oOldHorizontalPointBlock.OpenForRight()
      oOldVerticalPointBlock.Fields = saPointFields
      oOldVerticalPointBlock.OpenForRight()


      moNewPointBlock.Fields = saPointFields
      moNewPointBlock.OpenForRight()
      Dim oaResTplnPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
      '   System.Windows.Forms.MessageBox.Show(CStr(oStageTopoScheme.Nodes.Count), "15_199")
      If mdicPoints IsNot Nothing Then
         ' System.Windows.Forms.MessageBox.Show(CStr(mdicPoints.Count), "15_211")
      Else
         System.Windows.Forms.MessageBox.Show(CStr("mdicPoints Is Nothing"), "15_231")
      End If
      '    DMAcadExt.AcadDocument.WriteMessageLog("------Insert Points  Topo: " & oStageTopoScheme.Name)

      For Each oNodeScheme As TopoManager.TopoScheme.tsNode In oStageTopoScheme.Nodes
         bIsMustPoint = False
         bPointExists = mdicPoints.TryGetPoint(oNodeScheme.Location, oPoint)
         If bPointExists Then
            bIsMustPoint = oPoint.IsMustPoint
            bPointNotUsed = (oPoint.Stage = -1) OrElse oPoint.IsScipped
         End If
         If bPointExists Then
            sPointData = "; Name: " & oPoint.Name & " XY=" & oPoint.Coordinates & " Stage: " & CStr(oPoint.Stage)
         Else
            sPointData = String.Empty
         End If

         '     DMAcadExt.AcadDocument.WriteMessageLog("Node #" & CStr(oNodeScheme.ID) & " Point Exists:" & CStr(bPointExists) & " Old:" & CStr(oNodeScheme.HasOldPoint) & " Has must:" & CStr(oNodeScheme.HasMustPoint) & " Is PseudoTopo:" & CStr(oNodeScheme.IsPseudoTopo) & " Is PseudoGeo:" & CStr(oNodeScheme.IsPseudoGeo) & sPointData)
         If oNodeScheme.HasOldPoint Then
            If bPointNotUsed Then
               Select Case oPoint.CCode
                  Case 1
                     bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sOldLayer, DMAcadExt.DMApp.AppID, True, True)
                     zzInsertSurveyPoint(oOldPointBlock, oPoint)
                     '   oOldPointBlock.InsertRef(oPoint.GetHanitData())

                  Case 11 '"C1616"
                     bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sOldVerticalLayer, DMAcadExt.DMApp.AppID, True, True)
                     oOldVerticalPointBlock.InsertRef(oPoint.GetHanitData())
                  Case 12, 13 '"C1615"
                     bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sOldHorizontalLayer, DMAcadExt.DMApp.AppID, True, True)
                     oOldHorizontalPointBlock.InsertRef(oPoint.GetHanitData())
                  Case Else
               End Select

               oPoint.Stage = iStageNo
               oNodeScheme.Name = oPoint.Name
               oNodeScheme.HasActiveSurveyPoint = True
            Else
               oNodeScheme.HasUsedPoint = True
            End If

         ElseIf bPointExists Then
            ' DMAcadExt.AcadDocument.WriteMessageLog(oNodeScheme.Name & "; " & oNodeScheme.HasMustPoint.ToString() & "; " & oNodeScheme.IsPseudoTopo.ToString() & "; " & oNodeScheme.IsPseudoGeo.ToString())
            If Not oNodeScheme.IsPseudo Then  'oNodeScheme.HasOldPoint OrElse 210616
               If bPointNotUsed Then
                  bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, DMAcadExt.DMApp.AppID, True, True)
                  tBlockRefData = oPoint.GetHanitData()
                  ' oNewPointBlock.InsertRef(tBlockRefData)
                  zzInsertSurveyPoint(moNewPointBlock, oPoint)
                  oPoint.Stage = iStageNo
                  If oPoint.IsScipped Then
                     oPoint.IsScipped = False
                  End If

                  oNodeScheme.Name = oPoint.Name
                  oNodeScheme.HasActiveSurveyPoint = True
               Else
                  oNodeScheme.HasUsedPoint = True
               End If
            Else
               If bPointNotUsed Then
                  moTriangleMarkBlock.MarkPoint(oNodeScheme.Location, 4S)
                  oPoint.IsScipped = True
                  If oPoint.Stage = -1 Then
                     oPoint.Stage = iStageNo
                  End If
               Else
                  oNodeScheme.HasUsedPoint = True
               End If
            End If

         ElseIf Not oNodeScheme.IsPseudo Then
            moSquareMarkBlock.MarkPoint(oNodeScheme.Location, 2S)
            Dim tPointKey As ULong = DMAcadExt.TplnPointKeyLong.CoordToKey(oNodeScheme.Location.X, oNodeScheme.Location.Y)
            '   Dim oTestPoint As DMAcadExt.TPlnPoint = DMAcadExt.TplnPointKeyLong.PointFromKey(tPointKey)
            '    Dim oRndPoint As DMAcadExt.TPlnPoint = DMAcadExt.TplnPointKeyLong.GetRoundedPoint(oNodeScheme.Location.X, oNodeScheme.Location.Y)

            DMAcadExt.AcadDocument.WriteMessage("Point out of List: " & oNodeScheme.Location.ToString() & "; Key=" & tPointKey.ToString())
            '& "; Rnd=" & oRndPoint.Coordinates()
         End If

      Next

   End Sub
   Private Shared Sub zzInsertSurveyPoint(oPointBlock As DMAcadExt.AcadBlock, oPoint As UD_Point)
      Dim oBlockRefData As DMAcadExt.BlockRefData = oPoint.GetHanitData()
      Dim oPrevBlockRefData As DMAcadExt.BlockRefData = Nothing

      Dim sPointName As String = oPoint.Name '       oBlockRefData.GetAttribValue("POINT_NAME")
      If mdicPrevPoints IsNot Nothing AndAlso mdicPrevPoints.TryGetValue(sPointName, oPrevBlockRefData) Then
         If sPointName = "zzz31" Then
            System.Windows.Forms.MessageBox.Show(CStr(oPrevBlockRefData.AttribData.Count) & ":" & CStr(0) & ":" & oPrevBlockRefData.GetAttribValue("CLASS"), "15_163")

         End If
         oPrevBlockRefData.Position = oBlockRefData.Position
         oPointBlock.InsertRefAttrib(oPrevBlockRefData, False, True, sPointName = "zzz31")
      Else
         oPointBlock.InsertRef(oPoint.GetHanitData())
      End If

   End Sub


   Private Shared Function zzGetPgonID(ByRef oHalfEdge As HalfEdge) As Integer
      Dim oPgon As Polygon = Nothing
      Dim sTestMsg As String = ""

      Try
         oPgon = oHalfEdge.Polygon
      Catch oMapEx As Autodesk.Gis.Map.MapException
         If oMapEx.ErrorCode = 2010 Then
            If oPgon IsNot Nothing Then
               oPgon.Dispose()
               oPgon = Nothing
            End If
            Return 0
         Else
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(oHalfEdge.FullEdge.Entity.ToString()) & " TplnTopoPgon - zzBelongPgon:" & sTestMsg)
         End If
      End Try
      If oPgon Is Nothing Then
         Return 0
      Else
         Dim iID As Integer = oPgon.ID
         oPgon.Dispose()
         oPgon = Nothing
         Return iID
      End If
   End Function
   Private Shared Function zzHasNode(oPrevHalfEdge As HalfEdge, oNextHalfEdge As HalfEdge) As Boolean

      Dim oPrevDBObject As DBObject = DMAcadExt.AcadTransaction.GetDBObject(oPrevHalfEdge.FullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      Dim oNextDBObject As DBObject = DMAcadExt.AcadTransaction.GetDBObject(oNextHalfEdge.FullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      '  DMAcadExt.AcadDocument.WriteMessage("!-ParEdge " & oPrevDBObject.GetRXClass.Name & " <> " & oNextDBObject.GetRXClass.Name)
      If oPrevDBObject.GetRXClass.Name = oNextDBObject.GetRXClass.Name Then
         Select Case oPrevDBObject.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadLineName
               Return zzHasNode(DirectCast(oPrevDBObject, Line), DirectCast(oNextDBObject, Line))
            Case DMAcadExt.AcadConst.AcadArcName
               Return zzHasNode(DirectCast(oPrevDBObject, Arc), DirectCast(oNextDBObject, Arc))
         End Select
      Else
         Return True
      End If

   End Function
   Private Shared Function zzHalfEdgeToLink(oHalfEdge As HalfEdge) As DMAcadExt.IUD_Link
      Dim oDBObject As DBObject = DMAcadExt.AcadTransaction.GetDBObject(oHalfEdge.FullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      Dim oLine As Line
      Dim oArc As Arc

      '  DMAcadExt.AcadDocument.WriteMessage("!-ParEdge " & oPrevDBObject.GetRXClass.Name & " <> " & oNextDBObject.GetRXClass.Name)

      Select Case oDBObject.GetRXClass().Name
         Case DMAcadExt.AcadConst.AcadLineName
            oLine = DirectCast(oDBObject, Line)
            Return New DMAcadExt.TplnLine(oLine)

         Case DMAcadExt.AcadConst.AcadArcName
            oArc = DirectCast(oDBObject, Arc)
            If oArc.Handle.Value = &HC8C6 Then
               Dim taAcObjID(0) As Autodesk.AutoCAD.DatabaseServices.ObjectId
               taAcObjID(0) = oArc.ObjectId
               DMAcadExt.AcadDocument.PrintList(taAcObjID)
            End If


            Return New DMAcadExt.TplnArc(oArc)
         Case Else
            Return Nothing
      End Select
   End Function
   Public Shared ReadOnly Property PrevParcels As Dictionary(Of String, DMAcadExt.BlockRefData)
      Get
         Return mdicPrevParcels
      End Get
   End Property
   Public Shared Function GetPrevParcelData(sParcelName As String, ByRef tParcelData As DMAcadExt.BlockRefData) As Boolean

      If mdicPrevParcels Is Nothing Then
         Return False
      Else
         Return mdicPrevParcels.TryGetValue(sParcelName, tParcelData)
      End If


   End Function
   Public Shared Sub TestPrevParcels()
      DMAcadExt.AcadDocument.WriteMessage("PrevParcels.Count=" & mdicPrevParcels.Count.ToString())
      For Each sKey As String In mdicPrevParcels.Keys
         DMAcadExt.AcadDocument.WriteMessage("PrevParcels.Keys=" & sKey)
      Next
   End Sub
   Private Shared Function zzHasNode(oPrevLine As Line, oNextLine As Line) As Boolean
      Dim tPrevDelta As Autodesk.AutoCAD.Geometry.Vector3d = oPrevLine.Delta
      Dim tNextDelta As Autodesk.AutoCAD.Geometry.Vector3d = oNextLine.Delta
      '  DMAcadExt.AcadDocument.WriteMessage("Paral? " & tPrevDelta.ToString() & " <> " & tNextDelta.ToString())
      Return Not tPrevDelta.IsParallelTo(tNextDelta, New Autodesk.AutoCAD.Geometry.Tolerance(0.001, 0.001))
   End Function
   Private Shared Function zzHasNode(oPrevArc As Arc, oNextArc As Arc) As Boolean
      Return oPrevArc.Center.IsEqualTo(oNextArc.Center)
   End Function
   Private Shared Sub zzLoadPrevParcelCenters()
      '  Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs(ParcelCentreBlockName)
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(ParcelCentreBlockName)
      Dim sParcelName As String = "P_NOTHING"
      mdicPrevParcels = New Dictionary(Of String, DMAcadExt.BlockRefData)
      oAcadBlock.Open()
      oAcadBlock.LoadAllReferences()
      For Each tBlockRefData As DMAcadExt.BlockRefData In oAcadBlock.BlockRefValues

         If tBlockRefData.Layer = "C1603_0" Then
            sParcelName = tBlockRefData.GetAttribValue("PARCEL_NAME")
            If Not String.IsNullOrEmpty(sParcelName) Then

               If mdicPrevParcels.ContainsKey(sParcelName) Then
                  DMAcadExt.AcadDocument.WriteMessage("Parcel name '" & sParcelName & "'already exists; " & tBlockRefData.Position.ToString() & "; " & tBlockRefData.AcObjID.ToString())
               Else
                  mdicPrevParcels.Add(sParcelName, tBlockRefData)
               End If
            Else
               DMAcadExt.AcadDocument.WriteMessage("PARCEL_NAME WAS NOT FOUND " & tBlockRefData.Position.ToString() & ", " & tBlockRefData.AcObjID.ToString())
            End If
         End If

      Next
   End Sub
   Private Shared Sub zzLoadPrevFLineBlocks()
      '  Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs(FrontLineBlockName)
      Dim oAcadBlock As DMAcadExt.AcadBlock
      Dim sLineName As String

      mdicPrevFLines = New Dictionary(Of String, DMAcadExt.BlockRefData)

      oAcadBlock = New DMAcadExt.AcadBlock(FrontLineBlockName)
      oAcadBlock.Open()
      oAcadBlock.LoadAllReferences()
      '  System.Windows.Forms.MessageBox.Show(oAcadBlock.BlockRefsCount.ToString() & vbCrLf & "", "09_003")

      For Each tBlockRefData As DMAcadExt.BlockRefData In oAcadBlock.BlockRefValues
         sLineName = tBlockRefData.GetAttribValue("LINE_NAME")
         If Not String.IsNullOrEmpty(sLineName) Then
            If mdicPrevFLines.ContainsKey(sLineName) Then
               DMAcadExt.AcadDocument.WriteMessage("LINE already exists " & sLineName & "; " & tBlockRefData.Position.ToString() & "; " & tBlockRefData.AcObjID.ToString())
            Else
               mdicPrevFLines.Add(sLineName, tBlockRefData)
            End If

         Else
            DMAcadExt.AcadDocument.WriteMessage("LINE_NAME WAS NOT FOUND " & tBlockRefData.Position.ToString() & "; " & tBlockRefData.AcObjID.ToString())
         End If
      Next
   End Sub
   Private Shared Sub zzLoadPrevSurveyPoints()
      'Dim tBlockRefData As DMAcadExt.BlockRefData
      '    Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs(OldPointBlockName)
      Dim oAcadBlock As DMAcadExt.AcadBlock
      Dim sPointName As String
      '  Dim tBlockRefData As DMAcadExt.BlockRefData
      mdicPrevPoints = New Dictionary(Of String, DMAcadExt.BlockRefData)

      oAcadBlock = New DMAcadExt.AcadBlock(OldPointBlockName)
      oAcadBlock.Open()
      oAcadBlock.LoadAllReferences()
      '  System.Windows.Forms.MessageBox.Show(oAcadBlock.BlockRefsCount.ToString() & vbCrLf & "", "09_003")

      For Each tBlockRefData As DMAcadExt.BlockRefData In oAcadBlock.BlockRefValues
         sPointName = tBlockRefData.GetAttribValue("POINT_NAME")
         If Not String.IsNullOrEmpty(sPointName) Then
            If sPointName = "zz31" Then
               System.Windows.Forms.MessageBox.Show(tBlockRefData.GetAttribValue("CLASS").ToString() & vbCrLf & tBlockRefData.AttribDataDic.Count.ToString(), "04_196Z")
            End If
            If mdicPrevPoints.ContainsKey(sPointName) Then
               System.Windows.Forms.MessageBox.Show("Point '" & sPointName & "' already exists; ", "05_347")
            Else
               mdicPrevPoints.Add(sPointName, tBlockRefData)
            End If

         Else
            DMAcadExt.AcadDocument.WriteMessage("Old POINT_NAME was not found " & tBlockRefData.Position.ToString() & "; " & tBlockRefData.AcObjID.ToString())
         End If

      Next



      '   System.Windows.Forms.MessageBox.Show(colBlockRefs.Count.ToString() & vbCrLf & mdicPrevPoints.Count.ToString() & vbCrLf & mdicPrevPoints.ContainsKey("31"), "09_005")
      '  colBlockRefs = DMAcadExt.AcadTransaction.GetBlockRefs(NewPointBlockName)
      oAcadBlock = New DMAcadExt.AcadBlock(NewPointBlockName)

      oAcadBlock.Open()
      oAcadBlock.LoadAllReferences()
      '  System.Windows.Forms.MessageBox.Show(oAcadBlock.BlockRefsCount.ToString() & vbCrLf & "", "09_003")

      For Each tBlockRefData As DMAcadExt.BlockRefData In oAcadBlock.BlockRefValues
         sPointName = tBlockRefData.GetAttribValue("POINT_NAME")
         If Not String.IsNullOrEmpty(sPointName) Then


            mdicPrevPoints.Add(sPointName, tBlockRefData)
         Else
            DMAcadExt.AcadDocument.WriteMessage("New POINT_NAME  was not found " & tBlockRefData.Position.ToString() & "; " & tBlockRefData.AcObjID.ToString())
         End If

      Next


   End Sub
   Public Sub New()

   End Sub

   Private Shared Sub moDrawingSet_DrawingActivated(oSender As System.Object, e As Autodesk.Gis.Map.Project.DrawingActivatedEventArgs) Handles moDrawingSet.DrawingActivated
      System.Windows.Forms.MessageBox.Show(e.AttachedDrawing.ActualPath.ToString(), "04_008")
      '  zzImportHanitBorder(True)
   End Sub

   Private Shared Sub moDrawingSet_DrawingDeactivated(oSender As System.Object, e As Autodesk.Gis.Map.Project.DrawingDeactivatedEventArgs) Handles moDrawingSet.DrawingDeactivated
      Exit Sub
      System.Windows.Forms.MessageBox.Show(e.AttachedDrawing.ActualPath.ToString(), "04_009")
      miDrawingIndex += 1

      If miDrawingIndex < moDrawingSet.DirectDrawingsCount - 1 Then
         moAttachedDrawing = moDrawingSet.DirectAttachedDrawings.Item(miDrawingIndex)
         moAttachedDrawing.Activate()

      End If
   End Sub

   Private Shared Sub moDrawingSet_DrawingToBeActivated(oSender As System.Object, e As Autodesk.Gis.Map.Project.DrawingToBeActivatedEventArgs) Handles moDrawingSet.DrawingToBeActivated
      System.Windows.Forms.MessageBox.Show(e.AttachedDrawing.ActualPath.ToString(), "04_007")
      zzImportHanitBorder(True)
   End Sub
   Private Shared Function zzAttribTextToDouble(sText As String) As Double
      Try
         If sText IsNot Nothing Then
            Return Convert.ToDouble(sText.Trim)
         Else
            Return 0.0
         End If

      Catch oEx As Exception
         Return 0.0
      End Try
   End Function

End Class
