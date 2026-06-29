
Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports FDO
Imports Autodesk.AutoCAD.EditorInput
'Imports AcDbSymbolUtilities
'Imports Autodesk.Gis.Map

'Imports Autodesk.Gis.Map.Utilities
'Imports System.Runtime.InteropServices
Imports Autodesk.AutoCAD.Geometry
Imports TopoManager

Public NotInheritable Class TmstNetApplication
   Private Shared miIndex As Integer
   Private mfTopoView As frmTopoView
   Private WithEvents mfTplnView As frmTplnView
   Private WithEvents mfTopoActions As frmTopoActionsBase
   Private WithEvents mfDWGBatch As TopoUI.frmDWGBatch
   Private WithEvents mfApp As TopoUI.frmApp
   Private WithEvents mfPrjThemes As TopoUI.frmPrjThemes
   Private WithEvents mfUD_General As TopoUI.frmUD_General
   Private WithEvents mfEditBlockRefs As TopoUI.frmEditBlockRefs
   Private WithEvents mfHanitControl As TopoUI.frmHanitControl
   Private WithEvents mfProjectFiles As TopoUI.frmProjectFiles
   Private WithEvents mfExecUtil As TopoUI.frmExecUtil



   Private WithEvents moPgonRelation As FDO.PgonRelation


   Private moFDO_Manager As FDO.FDO_Manager
   '	Private WithEvents mfTopoActionsB As frmTopoActionsBase
   Private Shared mdTolerance As Double = 0.01
   Private Shared moMPolygonA As MPolygon
   Private WithEvents moAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document
   Private Shared WithEvents moEditor As Autodesk.AutoCAD.EditorInput.Editor
   <Autodesk.AutoCAD.Runtime.CommandMethodAttribute("CmdList")> _
   Public Shared Sub CmdList()
      Common.GetEditor.WriteMessage(vbCrLf & " TownPlanner Commands : " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : TplnCalc " & vbCrLf)

      Common.GetEditor.WriteMessage("** Cmd : RepContent  - 301" & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : RepPlanParcels  - 302 " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : RepLotsK  - 303 " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : RepLotsM  - 304 " & vbCrLf)

      Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseK  - 305 " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseM  - 306 " & vbCrLf)

      Common.GetEditor.WriteMessage("** Cmd : RepSumLuse  - 307 " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : RepParcels  - 311 " & vbCrLf)


      Common.GetEditor.WriteMessage("** Cmd : DispTpln " & vbCrLf)
      Common.GetEditor.WriteMessage("** Cmd : TplnClose " & vbCrLf)

   End Sub


   <CommandMethod("TestMPa")> _
   Public Sub TestMPolygonA()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      '	Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity A")
      '	Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
      Dim iParent As Integer
      '	Dim oPlineA, oPlineB As Polyline
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      Dim lstPolygons As System.Collections.Generic.IList(Of Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylines("1602", OpenMode.ForRead)  '"pclp004"
      Dim oMPolygonA As MPolygon = New MPolygon()
      Dim oMPgonLoopB As MPolygonLoop
      For iIndex As Integer = 0 To lstPolygons.Count - 1
         oPolygon = lstPolygons.Item(iIndex)

         Try
            oMPolygonA.AppendLoopFromBoundary(oPolygon, False, 0.001)
         Catch oEx As Exception
            oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try

      Next

      oMPolygonA.BalanceTree()



      oEditor.WriteMessage("NumMPgonLoops After: " & CStr(oMPolygonA.NumMPolygonLoops) & vbCrLf)
      Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection = oMPolygonA.GetLoopDirection(1)
      Dim bCrossed As Boolean
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      For iIndex As Integer = 0 To oMPolygonA.NumMPolygonLoops - 1
         iDir = oMPolygonA.GetLoopDirection(iIndex)
         iParent = 0
         oMPgonLoopB = oMPolygonA.GetMPolygonLoopAt(iIndex)
         bCrossed = oMPolygonA.LoopCrossesMPolygon(oMPgonLoopB, 0.001)
         If bCrossed Or iDir = LoopDirection.Interior Then

            If iDir = LoopDirection.Interior Then
               iParent = oMPolygonA.GetParentLoop(iIndex)
               zzDispMPgonLoop(oMPgonLoopB, Color.Red)
               If iParent >= 0 AndAlso iParent < oMPolygonA.NumMPolygonLoops Then
                  oMPgonLoopB = oMPolygonA.GetMPolygonLoopAt(iParent)
                  zzDispMPgonLoop(oMPgonLoopB, Color.Yellow)

               End If
            Else
               zzDispMPgonLoop(oMPgonLoopB, Color.Empty)

            End If
            oEditor.WriteMessage("Dir of Loop # " & CStr(iIndex) & " " & iDir.ToString() & " : " & bCrossed.ToString() & " : " & CStr(iParent) & vbCrLf)
         End If
      Next

      '	DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)

      Dim oaBlockRefPoints As DMAcadExt.TplnPointArray = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint("1603")
      Dim oPoint As DMAcadExt.TPlnPoint
      Dim iLoop As Integer
      For iIndex As Integer = 0 To oaBlockRefPoints.UpperBound
         oPoint = oaBlockRefPoints.Item(iIndex)
         iLoop = oMPolygonA.GetClosestLoopTo(oPoint.AcGePoint3d)
      Next
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub

   <CommandMethod("INTERS_")> _
   Public Sub Intersection()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity = Nothing
      Dim oEntB As Entity = Nothing
      Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            Return
         End Try
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
            Try
               oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
            Catch oEx As Exception
               Return
            End Try
         End If
         oEntA.IntersectWith(oEntB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))

         If colPoints IsNot Nothing Then
            Dim oSaltireMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)
            For Each tPoint As Point3d In colPoints
               ' DMAcadExt.AcadTransaction.InsertPoint(tPoint)
               oSaltireMarkBlock.MarkPoint(tPoint, 4S)

            Next
            DMAcadExt.AcadDocument.WriteMessage("Number of Intersect points: " & CStr(colPoints.Count))
         Else
            DMAcadExt.AcadDocument.WriteMessage("Intersect points not exist")
         End If

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TestMPb")> _
   Public Sub TestMPolygonB()
      Const dToler As Double = 0.001
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

      Dim oEnt As DBObject = Nothing
      Dim oPlineA As Polyline
      Dim oPlineB As Polyline


      Dim iParent As Integer
      '	Dim oPlineA, oPlineB As Polyline
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

      Dim lstPolygons As System.Collections.Generic.IList(Of Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylines("LA", OpenMode.ForRead)   '"pclp004"
      oPlineA = lstPolygons.Item(0)
      lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines("LB", OpenMode.ForRead)      '"pclp004"
      'lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines("LC", OpenMode.ForRead)	  '"pclp004"

      oPlineB = lstPolygons.Item(0)

      Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

      Dim oMPolygon As MPolygon = New MPolygon()
      Dim oMPgonLoop As MPolygonLoop
      Dim oMPgonLoopA As MPolygonLoop




      Try
         oMPolygon.AppendLoopFromBoundary(oPlineA, False, 0.001)
      Catch oEx As Exception
         oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         Return
      End Try

      Try
         oMPolygon.AppendLoopFromBoundary(oPlineB, False, 0.001)
      Catch oEx As Exception
         oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         Return
      End Try

      Try
         oMPolygon.BalanceTree()
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         oEditor.WriteMessage(" BP Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
      End Try


      Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection
      Dim bCrossed As Boolean

      Dim bCrossedItself As Boolean
      Dim bCrossedItselfA As Boolean

      Dim bTouch As Boolean
      '	Dim bLoopCrossing0 As Boolean
      '	Dim bLoopCrossing1 As Boolean
      '	Dim oMPgonLoop0 As MPolygonLoop
      '	Dim oMPgonLoop1 As MPolygonLoop

      bTouch = oMPolygon.IncludesTouchingLoops(0.001)


      '	oMPgonLoop0 = oMPolygonA.GetMPolygonLoopAt(0)

      '	bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
      '	bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)
      oPlineA.IntersectWith(oPlineB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))

      oEditor.WriteMessage("!!Number of Loop : " & CStr(oMPolygon.NumMPolygonLoops) & vbCrLf)
      oEditor.WriteMessage("IncludesTouchingLoops: " & CStr(bTouch) & vbCrLf)
      If colPoints IsNot Nothing Then
         oEditor.WriteMessage("Intersect Points: " & CStr(colPoints.Count) & vbCrLf)
      End If

      For iIndex As Integer = 0 To oMPolygon.NumMPolygonLoops - 1
         iDir = oMPolygon.GetLoopDirection(iIndex)
         iParent = 0
         oMPgonLoop = oMPolygon.GetMPolygonLoopAt(iIndex)
         oMPgonLoopA = oMPolygon.GetMPolygonLoopAt((iIndex + 1) Mod oMPolygon.NumMPolygonLoops)
         bCrossed = oMPolygon.LoopCrossesMPolygon(oMPgonLoop, dToler)
         bCrossedItself = oMPgonLoop.LoopCrossesItself(oMPgonLoop, dToler)
         bCrossedItselfA = oMPgonLoop.LoopCrossesItself(oMPgonLoopA, dToler)

         oEditor.WriteMessage("Dir of Loop # " & CStr(iIndex) & " = " & iDir.ToString() & " : " & bCrossed.ToString() & " : itself=" & CStr(bCrossedItself) & ":" & CStr(bCrossedItselfA) & vbCrLf)
      Next

      '	DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)
      moMPolygonA = oMPolygon

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   <CommandMethod("TestMPP")> _
   Public Sub TestMPPoint()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult

      Dim iaLoopInd As IntegerCollection
      oPromptOpt.UseBasePoint = False
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      '	DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)

      ptRes = oEditor.GetPoint(oPromptOpt)
      Dim tPoint As Point3d
      If ptRes.Status = PromptStatus.OK Then
         tPoint = ptRes.Value
         oEditor.WriteMessage("Point= " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
         iaLoopInd = moMPolygonA.IsPointInsideMPolygon(tPoint, 0.001)
         If iaLoopInd Is Nothing Then
            oEditor.WriteMessage("Point is Nothing ")
         Else
            For iIndex As Integer = 0 To iaLoopInd.Count - 1
               oEditor.WriteMessage("##" & CStr(iIndex) & ": " & CStr(iaLoopInd.Item(iIndex)))
            Next
         End If

      End If


      '	DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub


   Private Sub zzDispMPgonLoop(oMPgonLoop As MPolygonLoop, tColor As Color)
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

      Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle
      Dim tNormal As Autodesk.AutoCAD.Geometry.Vector3d = New Autodesk.AutoCAD.Geometry.Vector3d(0, 0, 1)
      For iIndex As Integer = 0 To oMPgonLoop.Count - 1
         tPoint = oMPgonLoop.Item(iIndex).Vertex
         tPoint3d = New Point3d(tPoint.X, tPoint.Y, 0.0)
         oCircle = New Autodesk.AutoCAD.DatabaseServices.Circle(tPoint3d, tNormal, 1.0)
         If tColor <> Color.Empty Then
            oCircle.Color = Autodesk.AutoCAD.Colors.Color.FromColor(tColor)
         End If
         DMAcadExt.AcadTransaction.AppendEntity(oCircle)
      Next

   End Sub
   <CommandMethod("PgonR")> _
   Public Sub PgonRelation()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      '  Dim bExists As Boolean
      Dim oPolyline As Polyline = Nothing
      Dim oPolylineA As Polyline = Nothing
      Dim oBlockRef As BlockReference = Nothing


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      DMAcadExt.AcadDocument.OpenLog(False)
      Do
         oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
            Catch oEx As Exception
               Return
            End Try
            If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               If oPolyline Is Nothing Then
                  oPolyline = DirectCast(oEnt, Polyline)
                  If oBlockRef IsNot Nothing Then
                     Exit Do
                  End If
               Else
                  oPolylineA = DirectCast(oEnt, Polyline)
                  Exit Do
               End If
            ElseIf oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then
               If oBlockRef Is Nothing Then
                  oBlockRef = DirectCast(oEnt, BlockReference)
               End If
               If oPolyline IsNot Nothing Then
                  Exit Do
               End If
            End If
         Else
            Exit Do
         End If

      Loop
      If oPolyline IsNot Nothing AndAlso oPolylineA IsNot Nothing Then
         moPgonRelation = TplnPolygonSet.PolygonRelation(oPolyline, oPolylineA, True)

         If moPgonRelation.ErrorPointsCount > 0 Then
            '''''''''''''''''  moPgonRelation.MarkIntersectionPoints(False)
         End If

      ElseIf oPolyline IsNot Nothing AndAlso oBlockRef IsNot Nothing Then
         TplnPolygonSet.PolygonRelation(oPolyline, oBlockRef)
      End If

      DMAcadExt.AcadDocument.CloseLog()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("MarkP")> _
   Public Sub MarkIntersectionPoints()
      If moPgonRelation IsNot Nothing Then

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
         DMAcadExt.AcadDocument.OpenLog(False)


         moPgonRelation.MarkIntersectionPoints(True)


         DMAcadExt.AcadDocument.CloseLog()
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If


   End Sub
   <CommandMethod("PgonI")> _
   Public Sub PgonIntersection()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      '   Dim bExists As Boolean
      Dim oPolyline As Polyline = Nothing
      Dim oPolylineA As Polyline = Nothing
      Dim oBlockRef As BlockReference = Nothing


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Do
         oEditor.WriteMessage("Select Polyline ...: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
            Catch oEx As Exception
               Return
            End Try
            If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               If oPolyline Is Nothing Then
                  oPolyline = DirectCast(oEnt, Polyline)
                  If oBlockRef IsNot Nothing Then
                     Exit Do
                  End If
               Else
                  oPolylineA = DirectCast(oEnt, Polyline)
                  Exit Do
               End If
            ElseIf oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then
               If oBlockRef Is Nothing Then
                  oBlockRef = DirectCast(oEnt, BlockReference)
               End If
               If oPolyline IsNot Nothing Then
                  Exit Do
               End If
            End If
         Else
            Exit Do
         End If

      Loop
      If oPolyline IsNot Nothing AndAlso oPolylineA IsNot Nothing Then
         TopoManager.tmVertices.SetTolerance(0.000001)
         Dim oRing As tmRing = New tmRing(1, oPolyline)
         Dim oRingA As tmRing = New tmRing(2, oPolylineA)
         Dim oPgonIntersect As tmPgonIntersect = New tmPgonIntersect(oRing, oRingA)


      End If


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("PgonC")> _
   Public Sub PgonCorrection()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
      '    Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      '  Dim bExists As Boolean
      Dim oPolyline As Polyline = Nothing
      Dim oPolylineA As Polyline = Nothing
      Dim oBlockRef As BlockReference = Nothing


    
      Dim tStartPoint As Point3d
      Dim tEndPoint As Point3d

      If zzSelectPoint(tStartPoint, True) Then
         '  System.Windows.Forms.MessageBox.Show("!!!!" & CStr("???"), "05_112")
         If zzSelectPoint(tEndPoint, True) Then
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
            DMAcadExt.AcadTransaction.Start()
            DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
            moPgonRelation.OpenLinesForWrite()
            moPgonRelation.CorrectBetween(tStartPoint, tEndPoint)

            DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
         End If
      End If


   End Sub


  
   <CommandMethod("ToMP")> _
   Public Sub ToMPolygon()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      Dim bExists As Boolean

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      '	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      Dim oMPolygonA As MPolygon = New MPolygon()
      Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

      Do
         oEditor.WriteMessage("Start: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try

               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
               oEditor.WriteMessage("oEnt: " & vbCrLf)
            Catch oEx As Exception
               oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
               DMAcadExt.AcadTransaction.Abort()
               DMAcadExt.AcadDocument.Unlock()
               Return
            End Try
            Select Case oEnt.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadPolylineName
                  Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     oMPolygonA.AppendLoopFromBoundary(oPolyline, True, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))
                     DMAcadExt.AcadTransaction.Abort()
                     DMAcadExt.AcadDocument.Unlock()
                     Return
                  End Try
               Case DMAcadExt.AcadConst.Acad2dPolylineName
                  Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     oMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))
                     DMAcadExt.AcadTransaction.Abort()
                     DMAcadExt.AcadDocument.Unlock()
                     Return
                  End Try

            End Select
         Else
            Exit Do
         End If
      Loop
      If bExists Then
         Try
            oMPolygonA.BalanceTree()
            For Each oDBObject As DBObject In colDBObject
               oDBObject.Erase()
            Next

            DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
            DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)
            DMAcadExt.AcadTransaction.CloseModelSpace()


         Catch oEx As Exception
            oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))
            DMAcadExt.AcadTransaction.Abort()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try
      End If

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("TestFC")> _
   Public Sub TestFindCenter()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      Dim oMPgon As MPolygon = zzGetMPolygon(False)
      If oMPgon IsNot Nothing Then
         Dim oFC As FDO.TplnPolygonSet.FindCenter = New FDO.TplnPolygonSet.FindCenter(oMPgon)
         Dim oPoint As DMAcadExt.TPlnPoint = oFC.GetInnerPoint()
         If oPoint IsNot Nothing Then

            DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
            DMAcadExt.AcadTransaction.AppendEntity(New DBPoint(oPoint.GetPoint3d()))
            DMAcadExt.AcadTransaction.CloseModelSpace()

         Else

         End If
      Else
         DMAcadExt.AcadDocument.WriteMessage("MPolygon Is Nothing")
      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("ListRX")> _
   Public Sub ListRX()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)

      DMAcadExt.AcadTransaction.TestModelSpaceCount()
      DMAcadExt.AcadTransaction.ListDBObjects()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()



   End Sub


   <CommandMethod("ClearXData")> _
   Public Sub ClearXData()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


      DMAcadExt.AcadTransaction.ClearXData()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   <CommandMethod("PgonV", CommandFlags.Session)> _
   Public Sub PgonView()
      FDO.TplnPolygonSet.PgonView()
   End Sub
   <CommandMethod("PgonL", CommandFlags.Session)> _
   Public Sub PgonList()
      FDO.TplnPolygonSet.PgonList()
   End Sub
   <CommandMethod("TmstOpen", CommandFlags.Session)> _
   Public Sub TmstOpen()
      zzOpenApp(DMAcadExt.enApplications.TopoMaster)
   End Sub
   <CommandMethod("UDOpen", CommandFlags.Session)> _
   Public Sub UDOpen()
      zzOpenApp(DMAcadExt.enApplications.Unidiv)
   End Sub
   <CommandMethod("Uni_Start", CommandFlags.Session)> _
   Public Sub Start()
      UnidivNet.Unidiv.Start()
   End Sub
   <CommandMethod("Exut", CommandFlags.Session)> _
   Public Sub ExecUtil()
      ' FDO.Util.SeparateShapePgons()
      ' DMAcadExt.AcadDocument.InitDebug()
      '  FDO.Util.SetScheme()
      FDO.Util.EnumShapePgons()
   End Sub
  
   <CommandMethod("Nat", CommandFlags.Session)> _
   Public Sub Nat()
      UnidivNet.Unidiv.NatA()
   End Sub
   <CommandMethod("Dorit", CommandFlags.Session)> _
   Public Sub PolylinesByCentroidLayer()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      '   MessageBox.Show(tMapThemeData.ClosedPgonsLayers, "03_120")


      ' MessageBox.Show(sSourceTopoName, "03_122")

      Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("BN")
      ' MessageBox.Show(sSourceTopoName, "03_123")
      oTopoScheme.Load(False)
      MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "03_124")
      oTopoScheme.CreateDBPolylinesByCentroidLayer()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("TmstClose", CommandFlags.Session)> _
   Public Sub TmstClose()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.Close()
         mfTopoActions.Dispose()
         mfTopoActions = Nothing
      End If

   End Sub
   <CommandMethod("dmList", CommandFlags.Session)> _
   Public Sub dmList()

   End Sub
   <CommandMethod("Tst", CommandFlags.Session)> _
   Public Sub TestGeo()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Arc")
      '		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim dBulge As Double = 0.0
      Dim oArc2d As CircularArc2d = Nothing
      Dim tPriorPoint, tCurrentPoint2d As Point2d

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      oArc2d = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, False)

      Try
         Dim oArc As Arc = New Arc(New Point3d(0, 0, 0), 2, 3.0, 2.0)
         oArc.Layer = "Red"

         DMAcadExt.AcadTransaction.AppendEntity(oArc)
         DMAcadExt.AcadDocument.WriteMessage("Total =" & CStr(oArc.TotalAngle) & ":" & oArc.Normal.Z)

         Dim oArc1 As Arc = New Arc(New Point3d(0, 0, 0), 2, 5.0, 4.0)
         oArc1.Layer = "Green"
         DMAcadExt.AcadTransaction.AppendEntity(oArc1)
         DMAcadExt.AcadDocument.WriteMessage("Total1 =" & CStr(oArc1.TotalAngle) & ":" & oArc1.Normal.Z)


         Dim oArc2 As Arc = New Arc(New Point3d(0, 0, 0), 2, 6.0, -1.0)
         oArc2.Layer = "Blue"
         DMAcadExt.AcadTransaction.AppendEntity(oArc2)
         DMAcadExt.AcadDocument.WriteMessage("Total2 =" & CStr(oArc2.TotalAngle) & ":" & oArc2.Normal.Z)
         '		DMAcadExt.AcadDocument.WriteDebugMessage("Center=" & CStr(oArc.Center.X) & "," & CStr(oArc.Center.Y))



      Catch ex As Exception

      End Try



      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   <CommandMethod("TplnNatA", CommandFlags.Session)> _
   Public Sub TplnNatA()
      Const sInfoBlockName As String = "PCLS014"
      Const sPointBlockName As String = "SRVS007"

      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim colBlockRefs As ICollection(Of DMAcadExt.AttachPair)
      Dim colAttachPairs As ICollection(Of DMAcadExt.AttachPair)

      Dim colSegments As ICollection(Of DMAcadExt.TplnSegment)

      Dim oAttach As DMAcadExt.AttachEntities
      Dim oaAttachPairs() As DMAcadExt.AttachPair

      Dim oaBlockRefsA() As ObjectId
      Dim oaBlockRefsB() As ObjectId

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      '   Dim oaPointsA As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray(iMainUB)
      '  Dim oaPointsB As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray(iMainUB)

      Dim oaLength() As Double
      Dim saLabels() As String

      Dim oaPointsA As DMAcadExt.TplnPointArray
      Dim oaPointsB As DMAcadExt.TplnPointArray
      Try

         Dim iMainUB As Integer
         colBlockRefs = DMAcadExt.AcadTransaction.GetAllBlockRefsForAttach(sInfoBlockName)
         colSegments = DMAcadExt.AcadTransaction.GetSegments()
         iMainUB = colBlockRefs.Count - 1
         ' MessageBox.Show(CStr(colBlockRefs.Count) & ":" & CStr(colSegments.Count), "03_240")
         oAttach = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToEntity2D, colBlockRefs) '"PCLS014"
         oAttach.AddSegments(colSegments)
         oAttach.Calculate(False)
         System.Windows.Forms.MessageBox.Show("!!!!" & CStr("???"), "03_520b")
         colAttachPairs = oAttach.GetAttachedPairs()   'Line +  "PCLS014"
         oaAttachPairs = oAttach.Output ''Line +  "PCLS014"
         Dim oAttachedSegment As DMAcadExt.AttachedSegment
         '	MessageBox.Show(CStr(colAttachPairs.Count) & ":" & CStr(oaAttachPairs.GetUpperBound(0)) & ":" & CStr(oAttach.AttachedCount), "03_252")
         '	MessageBox.Show(oaAttachPairs(0).ToString, "03_253")


         '	DMAcadExt.AcadDocument.WriteMessage("Total1 =" & CStr(oArc1.TotalAngle) & ":" & oArc1.Normal.Z)
         oaPointsA = New DMAcadExt.TplnPointArray(iMainUB)
         oaPointsB = New DMAcadExt.TplnPointArray(iMainUB)
         ReDim oaLength(iMainUB)
         ReDim saLabels(iMainUB)


         Dim oSegment As DMAcadExt.TplnSegment

         For iIndex As Integer = 0 To oaAttachPairs.GetUpperBound(0)
            If oaAttachPairs(iIndex) IsNot Nothing Then
               Try
                  oAttachedSegment = DirectCast(oaAttachPairs(iIndex), DMAcadExt.AttachedSegment)
                  oSegment = oAttachedSegment.Segment
                  If oSegment IsNot Nothing Then
                     oaPointsA.Item(iIndex) = oSegment.StartPoint
                     oaPointsB.Item(iIndex) = oSegment.EndPoint
                     oaLength(iIndex) = oaPointsA.Item(iIndex).Distance(oaPointsB.Item(iIndex))
                     saLabels(iIndex) = oSegment.UD_Label
                  End If
               Catch ex As Exception
                  MessageBox.Show(ex.Message & vbCrLf & ex.StackTrace, "03_255")
               End Try


            End If

         Next

         oAttach = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPoint)

         Dim oPointArray As DMAcadExt.TplnPointArray = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint(sPointBlockName) 'SRV007
         '	MessageBox.Show(CStr(oPointArray.UpperBound), "03_256b")
         oAttach.AddPointArray(oPointArray, False)
         '	MessageBox.Show(CStr(oaPointsA.UpperBound), "03_256c")
         oAttach.SourcePoints = oaPointsA

         oAttach.Calculate(False)
         oaBlockRefsA = oAttach.OutputAcadObjects
         'MessageBox.Show(CStr(oaBlockRefsA.GetUpperBound(0)), "03_260")

         oAttach.SourcePoints = oaPointsB
         oAttach.Calculate(False)
         oaBlockRefsB = oAttach.OutputAcadObjects
         'MessageBox.Show(CStr(oaBlockRefsB.GetUpperBound(0)), "03_290")


      Catch oEx As Exception
         Return
      End Try


      Dim sFileName As String = DMAcadExt.AcadDocument.GetCurrentDWGName()
      Dim oFileInfo As IO.FileInfo

      Dim saFile() As String
      Dim sBlock As String
      If sFileName IsNot Nothing Then
         oFileInfo = New IO.FileInfo(sFileName)

         saFile = Split(oFileInfo.Name, ".")
         sBlock = saFile(0)
      Else
         sBlock = ""
      End If


      Dim bPoint As Boolean
      Dim iaAttribIndex() As Integer = {0, 1}
      Dim sValuePointA As String
      Dim sValuePointB As String

      Dim saValue() As String
      Dim oRep As ExcelReport.Report = New ExcelReport.Report(False)
      Dim dRef As Double
      oRep.Open()
      oRep.SetValue(0, 0, "גוש מספר")
      oRep.SetValue(0, 1, "ממס' נקודה")
      oRep.SetValue(0, 2, "למס' נקודה")
      oRep.SetValue(0, 3, "מרחק מדוד")
      oRep.SetValue(0, 4, "מרחק מחושב")
      oRep.SetValue(0, 5, "הפרש בין מדוד למחושב")
      oRep.SetHeaderFormat(0, 5)
      Dim sNumFormat As String = oRep.GetWinNumFormat(2, TriState.True)
      For iIndex As Integer = 0 To oaAttachPairs.GetUpperBound(0)
         oRep.SetValue(iIndex + 1, 0, sBlock)
         ' If Not oaBlockRefsA(iIndex).IsNull Then
         sValuePointA = DMAcadExt.AcadTransaction.GetAttribText(oaBlockRefsA(iIndex), True, bPoint, 0)
         oRep.SetValue(iIndex + 1, 1, sValuePointA)
         '  End If
         If Not oaBlockRefsB(iIndex).IsNull Then
            sValuePointB = DMAcadExt.AcadTransaction.GetAttribText(oaBlockRefsB(iIndex), True, bPoint, 0)
            oRep.SetValue(iIndex + 1, 2, sValuePointB)
         End If




         If Not oaAttachPairs(iIndex).SourceObjID.IsNull Then
            saValue = DMAcadExt.AcadTransaction.GetAttribText(oaAttachPairs(iIndex).SourceObjID, True, bPoint, iaAttribIndex)
            oRep.SetValue(iIndex + 1, 3, saValue(1), sNumFormat)
            '   oRep.SetValue(iIndex + 1, 4, saValue(0), sNumFormat)
            oRep.SetValue(iIndex + 1, 4, saLabels(iIndex))
            '  oRep.SetValue(iIndex + 1, 7, oaPointsA.Item(iIndex).Coordinates)
            '   oRep.SetValue(iIndex + 1, 8, oaPointsB.Item(iIndex).Coordinates)


            If (saValue(0) & saValue(1)).Length <> 0 Then
               dRef = zzAttribTextToDouble(saValue(1)) - zzAttribTextToDouble(saValue(0))
               oRep.SetValue(iIndex + 1, 5, dRef, sNumFormat)
            End If
            '  oRep.SetValue(iIndex + 1, 6, FormatNumber(oaLength(iIndex), 3), sNumFormat)
            '  oRep.SetValue(iIndex + 1, 6, saLabels(iIndex))


            '	MessageBox.Show(CStr(saValue(0) & ":" & saValue(1)), "03_292")
         End If


      Next

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Private Function zzAttribTextToDouble(sValue As String) As Double
      Dim dRes As Double
      If String.IsNullOrEmpty(sValue) Then
         dRes = 0.0
      Else
         Dim saVal() As String = Split(sValue, "=")
         If saVal.GetUpperBound(0) = 0 Then
            Double.TryParse(saVal(0), dRes)
         Else
            Double.TryParse(saVal(1), dRes)
         End If

      End If
      Return dRes
   End Function
   <CommandMethod("TestOleDB")> _
   Public Sub TestOleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB()
      TPlServerDB.dmDBManager.CheckProviderFactory()
   End Sub
   <CommandMethod("TplnV1")> _
   Public Sub TplnV1()
      DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      Dim iTest As Integer = TopoDefs.giToposUB
      TopoManager.TPlanGraph.TplnProject.InitAppName()
      Parameters.Init()
      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()   'TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB


      If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
         frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
         TPlServerDB.ServerDB.AddInitialize()
         TopoManager.TPlanGraph.TplnProject.InitializeList()
         If mfTopoActions Is Nothing OrElse mfTopoActions.IsDisposed Then
            TopoDefs.InitTaba()
            mfTopoActions = New frmTopoActionsT()
         End If
         TopoManager.TPlanGraph.TplnProject.InitializeView()
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActions)
      End If
   End Sub
   <CommandMethod("TplnBatch")> _
   Public Sub TplnBatch()
      '		TopoManager.TPlanGraph.TplnProject.InitAppName()
      '		TopoManager.TPlanGraph.TplnProject.InitializeDB()

      DMAcadExt.AcadDocument.SetLogName()
      TopoManager.TPlanGraph.TplnProject.InitializeList()
      If mfDWGBatch Is Nothing OrElse mfDWGBatch.IsDisposed Then
         mfDWGBatch = New TopoUI.frmDWGBatch()
      End If

      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDWGBatch)

   End Sub

   <CommandMethod("Excv")> _
   Public Sub ExecUtilForm()



      DMAcadExt.AcadDocument.SetLogName()

      If mfExecUtil Is Nothing OrElse mfExecUtil.IsDisposed Then
         mfExecUtil = New TopoUI.frmExecUtil
      End If

      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfExecUtil)

   End Sub


   <CommandMethod("TplnPrc")> _
   Public Sub TplnParcels()
      '		TopoManager.TPlanGraph.TplnProject.InitAppName()
      '		TopoManager.TPlanGraph.TplnProject.InitializeDB()


      '	TopoManager.TPlanGraph.TplnProject.InitializeList()

      DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      If mfApp Is Nothing OrElse mfApp.IsDisposed Then
         mfApp = New TopoUI.frmApp()
      End If

      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfApp)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")

   End Sub
   <CommandMethod("WWW")> _
   Public Sub WWW()
      Dim iLT As UInt32 = DMAcadExt.AcadDocument.GetLayerTransparency
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

      oEditor.WriteMessage("LT= " & CStr(iLT))
   End Sub
   '   <CommandMethod("QQQ")> _
   Public Sub Test1()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
            oEntA.Visible = False
         Catch oEx As Exception
            Return
         End Try
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("Test2")> _
   Public Sub Test2()
      Dim oPolylineJig As UnidivNet.PolylineJig
      Dim oPromptResult As PromptResult
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity
      Dim oCurve As Curve
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEditor.WriteMessage(vbCrLf)
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
            If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               oCurve = DirectCast(oEntA, Curve)
               If oCurve IsNot Nothing Then
                  Dim oaCurves() As Curve = {oCurve}



                  oPolylineJig = New UnidivNet.PolylineJig(oaCurves)
                  oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

                  If oPromptResult.Status = PromptStatus.OK Then
                     oPolylineJig.CreateEntity()

                  End If

               End If


            End If
         Catch oEx As Exception
            Return
         End Try
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()







   End Sub
   '<CommandMethod("QQQ")> _
   Public Sub Test3()
      Dim oPolylineJig As UnidivNet.ShowPolylineJig
      Dim oPromptResult As PromptResult
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity
      Dim oCurve As Polyline
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEditor.WriteMessage(vbCrLf)
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
            If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               oCurve = DirectCast(oEntA, Polyline)
               If oCurve IsNot Nothing Then




                  oPolylineJig = New UnidivNet.ShowPolylineJig(oCurve)
                  oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

                  If oPromptResult.Status = PromptStatus.OK Then


                  End If

               End If


            End If
         Catch oEx As Exception
            Return
         End Try
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      ' TplnPointKeyLong.vb() : Line 50






   End Sub
   <CommandMethod("Test04")> _
   Public Sub Test04()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      '   Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      '  Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

      Dim ptPoint1Res, ptPoint2Res As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptPoint1Res = oEditor.GetPoint("Point1..")
      If ptPoint1Res.Status = PromptStatus.OK Then
         ptPoint2Res = oEditor.GetPoint("Point2..")
         If ptPoint2Res.Status = PromptStatus.OK Then
            oEditor.DrawVector(ptPoint1Res.Value, ptPoint2Res.Value, 1, False)

         End If

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("Test04a")> _
   Public Sub Test04a()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      '   Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      '  Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

      Dim ptPoint1Res, ptPoint2Res As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oEnt As DBObject = Nothing
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
      Dim tTransform As Matrix3d = New Matrix3d()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptPoint1Res = oEditor.GetPoint("Point1..")
      If ptPoint1Res.Status = PromptStatus.OK Then
         ptPoint2Res = oEditor.GetPoint("Point2..")
         If ptPoint2Res.Status = PromptStatus.OK Then
            ' Dim oPline As Polyline = New Polyline(2)
            Dim tVector1, tVector2 As Vector3d
            '  oPline.AddVertexAt(0, DMAcadExt.TPlnPoint.Point3dTo2d(ptPoint1Res.Value), 0, 0, 0)
            '  oPline.AddVertexAt(1, DMAcadExt.TPlnPoint.Point3dTo2d(ptPoint2Res.Value), 0, 0, 0)
            ' DMAcadExt.AcadTransaction.AppendEntity(oPline, False)
            tVector1 = New Vector3d(ptPoint1Res.Value.X, ptPoint1Res.Value.Y, 0.0)
            tVector2 = New Vector3d(ptPoint2Res.Value.X, ptPoint1Res.Value.Y, 0.0)

            oResBuffer.Add(tVector1)
            oResBuffer.Add(tVector2)

            tTransform = Autodesk.AutoCAD.Geometry.Matrix3d.Scaling(2.0, ptPoint1Res.Value)
            oEditor.DrawVectors(oResBuffer, tTransform)

         End If

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("Test05")> _
   Public Sub Test05()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      'Dim oPolyline As Polyline = New Polyline
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim ptResA As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet



      Try
         oPromptOpt.AllowDuplicates = False
         oPromptOpt.SingleOnly = False
         oPromptOpt.MessageForAdding = "Add ***"
         oPromptOpt.MessageForRemoval = "Remove ***"

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
      End Try

      ptRes = oEditor.GetSelection(oPromptOpt)




      Dim col As ICollection(Of DMAcadExt.TplnLine) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.TplnLine)
      Dim oLine As Line
      Dim oUdLine As DMAcadExt.TplnLine

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oSelSet = ptRes.Value()
         For i As Integer = 0 To oSelSet.Count - 1
            oLine = DMAcadExt.AcadTransaction.GetLine(oSelSet.Item(i).ObjectId, OpenMode.ForRead, True)
            If oLine IsNot Nothing Then
               oUdLine = New DMAcadExt.TplnLine(oLine)
               col.Add(oUdLine)
            End If
         Next

      End If

      Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(col, oSelSet.Count, 1, False)

      DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, -1)

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub



   <CommandMethod("QQQ")> _
   Public Sub TestBlocks()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim colNodes As ObjectIdCollection
      Dim sNodeBlocks As String = "C1610,C1611,C1615,C1616"
      Dim sNodeLayers As String = "UD_NewPoints,1610_0"


      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      colNodes = DMAcadExt.AcadTransaction.GetBlockRefsNew(sNodeBlocks, sNodeLayers)
      oEditor.WriteMessage("A: " & colNodes.Count.ToString())
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestPJ")> _
   Public Sub TestPgonJig()


      Dim oPromptResult As PromptResult
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity
      Dim oCurve As Polyline = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try

            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)

            If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               oCurve = DirectCast(oEntA, Polyline)


               Dim oPolylineJig As TopoManager.TopoScheme.TopoPgonJig

               oPolylineJig = New TopoManager.TopoScheme.TopoPgonJig("fragments", oCurve)

               oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

               If oPromptResult.Status = PromptStatus.OK Then




               End If
            End If

         Catch ex As Exception
         End Try

      End If


   End Sub
   <CommandMethod("TplnOpen")> _
   Public Sub TplnOpen()
      '  If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
      DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      '    End If


      TopoManager.TPlanGraph.TplnProject.InitAppName()
      '   DMAcadExt.AcadDocument.InitDebug()
      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
      'TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      If mfPrjThemes Is Nothing OrElse mfPrjThemes.IsDisposed Then
         mfPrjThemes = New TopoUI.frmPrjThemes()
      End If
      'MessageBox.Show(CStr(
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfPrjThemes)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
      mfPrjThemes.Left = 40
      mfPrjThemes.Top = 40
   End Sub
   <CommandMethod("UD_Gen")> _
   Public Sub UD_Gen()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Unidiv
      End If
      '   frmHanitControl.vb() : Line 25

      TopoManager.TPlanGraph.TplnProject.InitAppName()
      '	MessageBox.Show("", "07_117")
      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      '	MessageBox.Show("", "07_300")
      '	TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      If mfUD_General Is Nothing OrElse mfUD_General.IsDisposed Then
         mfUD_General = New TopoUI.frmUD_General(False)
      End If
      'MessageBox.Show(CStr(
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
      mfUD_General.Left = 40
      mfUD_General.Top = 40
   End Sub
   <CommandMethod("BAEd")> _
   Public Sub BAEd()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If


      TopoManager.TPlanGraph.TplnProject.InitAppName()

      ''''''''''''''''''''''''''''		TopoManager.TPlanGraph.TplnProject.InitializeDB_SQL()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      If mfEditBlockRefs Is Nothing OrElse mfEditBlockRefs.IsDisposed Then
         mfEditBlockRefs = New TopoUI.frmEditBlockRefs
      End If
      'MessageBox.Show(CStr(
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditBlockRefs)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
      mfEditBlockRefs.Left = 40
      mfEditBlockRefs.Top = 40
   End Sub
   <CommandMethod("AddB")> _
   Public Sub AdditionalBlock()
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("1603")
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      oAcadBlock.LoadAllReferences()
      oAcadBlock.OpenAdditionalBlock("1603@a", "M:\Dm_Work\Blocks\Additional")
      oAcadBlock.InsertAddBlockRefs()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub


   <CommandMethod("TTMP")> _
   Public Sub TempBatch()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      DMAcadExt.AcadTransaction.DrawNet(101, 101, 0.1, 0.1)


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("VPNet")> _
   Public Sub DrawViewportNet()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      DMAcadExt.AcadTransaction.DrawNet(DMAcadExt.KeyPoint.Tolerance, DMAcadExt.KeyPoint.Tolerance)


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("LTS")> _
   Public Sub LoadTopoScheme()
      '  Dim sToponame As String = "Parcels"
      '  Dim sToponame As String = "LotsK"
      '  Dim sToponame As String = "TopoOne"
      Dim sToponame As String = "fragments"




      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      DMAcadExt.AcadDocument.SetLogName()
      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
      '  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
      Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sToponame)

      oTopoScheme.Load(False)
      oTopoScheme.Close()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TTS")> _
   Public Sub TestTopoScheme()
      '  Dim sToponame As String = "Parcels"
      '  Dim sToponame As String = "LotsK"
      '  Dim sToponame As String = "TopoOne"
      Dim sToponame As String = "TopoMany"




      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
      '  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
      Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sToponame)



      DMAcadExt.AcadDocument.SetLogName()
      oTopoScheme.Load(True)

      oTopoScheme.CalcIsthmus()
      DMCommon.Debug.MsgBox("08_100", oTopoScheme.Elements.Polygons.Count)
      oTopoScheme.RemovePseudoPoints()
      DMCommon.Debug.MsgBox("08_101", oTopoScheme.Elements.Polygons.Count)
      ' oTopoScheme.CreateBulgeVertexArray(False)
      ''''''''''''''''''''''''  oTopoScheme.RemovePseudoPoints()
      '  oTopoScheme.CreateDBPolylines()
      'Dim oElem As TopoScheme.tsElement = oTopoScheme.GetElement(1721)
      'If oElem Is Nothing Then
      '   '  MessageBox.Show("oElem Is Nothing", "08_200")
      'Else
      '   ' MessageBox.Show(oElem.ElemType.ToString(), "08_200")
      'End If
      oTopoScheme.CreateDBPolylineMPlus(True)
      ' MessageBox.Show("", "08_200")
      '''''''''   oTopoScheme.PrintInfo()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TMP")> _
   Public Sub TestMarkPoints()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
      '  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
      '   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
      Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Blocks")



      DMAcadExt.AcadDocument.SetLogName()
      '   Dim bCurrentLayerOK As Boolean






      ' bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("1620", DMAcadExt.DMApp.AppID, True, True)
      oTopoScheme.Load(False)

      MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "08_100")
      Dim oResList As List(Of InitInsertData) = oTopoScheme.GetMarkPoints(14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor)

      MessageBox.Show(CStr(oResList.Count), "08_110")

      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("1620", "M:\Dm_Work\Blocks\Mavat-2010")
      Dim tBlockRefData As DMAcadExt.BlockRefData
      Dim iParity As Integer = 0
      oAcadBlock.OpenForRight()
      For Each tInitInsertData As InitInsertData In oResList
         '  DMAcadExt.AcadTransaction.InsertPoint(tInitInsertData.Position)
         tBlockRefData = New DMAcadExt.BlockRefData()

         tBlockRefData.Position = tInitInsertData.Position
         If iParity = 0 Then
            tBlockRefData.ColorIndex = 2S
         Else
            tBlockRefData.ColorIndex = 3S
         End If

         '    tBlockRefData.Layer = msTazarMapBlockLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor)
         tBlockRefData.Rotation = tInitInsertData.Rotation + Convert.ToDouble(iParity) * Math.PI
         oAcadBlock.InsertRefNewNew(tBlockRefData)
         iParity = (iParity + 1) Mod 2

      Next


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TopoToPl")> _
   Public Sub TopoPgonToPolyline()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()

      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      Dim sTopoName As String = "fragments"
      Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
      Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sTopoName)
      MessageBox.Show(oTopoModel.Name, "03_123")

      Dim tSelectedPoint As Point3d
      Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon = Nothing
      oTopoScheme.Load(True, oTopoModel)
      If zzSelectPoint(tSelectedPoint, False) Then
         Try
            oPolygon = oTopoModel.FindPolygon(tSelectedPoint)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            If oMapEx.ErrorCode <> 3 Then
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & sTopoName & vbCrLf & tSelectedPoint.ToString(), "01_87f")
               DMAcadExt.AcadDocument.WriteDebugMessage(oMapEx.ErrorCode & "; " & sTopoName & "; " & tSelectedPoint.ToString())
            End If

         End Try

         If oPolygon IsNot Nothing Then

            oTopoScheme.CreatePgonDBPolyline(True, oPolygon.ID)
         End If

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("IsExt")> _
   Public Sub IsExtend()
      Extend(False)
   End Sub
   <CommandMethod("UnL")> _
   Public Sub UnionLinks()
      Extend(True)
   End Sub
   Public Sub Extend(ByVal bUnion As Boolean)
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity = Nothing
      Dim oEntB As Entity = Nothing
      Dim oLinkA As DMAcadExt.IUD_Link = Nothing
      Dim oLinkB As DMAcadExt.IUD_Link = Nothing
      Dim bIsExt As Boolean
      Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            Return
         End Try

         Select Case oEntA.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadArcName
               Dim oArc As Arc = DirectCast(oEntA, Arc)
               zzArcInfo(oArc)
               oLinkA = New DMAcadExt.TplnArc(oArc)
            Case DMAcadExt.AcadConst.AcadLineName
               Dim oLine As Line = DirectCast(oEntA, Line)
               oLinkA = New DMAcadExt.TplnLine(oLine)
         End Select
         oEditor.WriteMessage(vbCrLf)
         oPromptOpt.Message = "Select Next Entity"
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
            oEditor.WriteMessage(vbCrLf)
            Try
               oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
            Catch oEx As Exception
               Return
            End Try
            Select Case oEntB.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadArcName
                  Dim oArc As Arc = DirectCast(oEntB, Arc)
                  zzArcInfo(oArc)
                  oLinkB = New DMAcadExt.TplnArc(oArc)
               Case DMAcadExt.AcadConst.AcadLineName
                  Dim oLine As Line = DirectCast(oEntB, Line)
                  oLinkB = New DMAcadExt.TplnLine(oLinkA.EndPoint, oLine)
                  If oLinkB.Length = 0.0 Then
                     oLinkA.Reverse()
                     oLinkB = New DMAcadExt.TplnLine(oLinkA.EndPoint, oLine)
                  End If
            End Select
            bIsExt = oLinkA.IsExtend(oLinkB, TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance, True)
            DMAcadExt.AcadDocument.WriteMessage("IsExtend: " & CStr(bIsExt))
            If bUnion AndAlso bIsExt Then
               Dim tPlineStart As Point2d = oLinkA.StartPoint
               Dim tPlineEnd As Point2d = oLinkB.EndPoint

               oLinkA.Extend(oLinkB)
               Dim oNewLink As Entity = oLinkA.GetNewEntity()
               DMAcadExt.AcadTransaction.AppendEntity(oNewLink, False)
               Dim oNewPline As Polyline = New Polyline(2)
               oNewPline.AddVertexAt(0, oLinkA.StartPoint, oLinkA.Bulge, 0.0, 0.0)
               oNewPline.AddVertexAt(1, oLinkA.EndPoint, 0.0, 0.0, 0.0)
               oNewPline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
               DMAcadExt.AcadTransaction.AppendEntity(oNewPline, False)

               Dim oNewPlineA As Polyline = New Polyline(2)
               oLinkA.Reverse()
               DMAcadExt.AcadDocument.WriteMessage("IsExtend: " & oLinkA.StartPoint.ToString() & "; " & oLinkA.EndPoint.ToString() & "; B=" & oLinkA.Bulge.ToString())
               oNewPlineA.AddVertexAt(0, oLinkA.StartPoint, oLinkA.Bulge, 0.0, 0.0)
               oNewPlineA.AddVertexAt(1, oLinkA.EndPoint, 0.0, 0.0, 0.0)
               oNewPlineA.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Yellow)
               DMAcadExt.AcadTransaction.AppendEntity(oNewPlineA, False)

            End If
         End If

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("HL")> _
   Public Sub HideLayer()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As Entity = Nothing
      '   Dim bExists As Boolean



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Do
         oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
            Catch oEx As Exception
               Return
            End Try
            DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
         Else
            Exit Do
         End If

      Loop

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("HE")> _
   Public Sub HideEntity()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As Entity = Nothing
      '   Dim bExists As Boolean



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      ' DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Do
         oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
            Catch oEx As Exception
               Return
            End Try
            oEnt.Visible = False
            DMAcadExt.AcadDocument.UpdateScreen()
            ' DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
         Else
            Exit Do
         End If
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadTransaction.Start()
      Loop

      '  DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("ColE")> _
   Public Sub ColorEntity()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As Entity = Nothing
      '   Dim bExists As Boolean



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      ' DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Do
         oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
            Catch oEx As Exception
               Return
            End Try
            oEnt.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
            DMAcadExt.AcadDocument.UpdateScreen()
            ' DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
         Else
            Exit Do
         End If
         '   DMAcadExt.AcadTransaction.Terminate()
         '   DMAcadExt.AcadTransaction.Start()
      Loop

      '  DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("UD_Hanit")> _
   Public Sub UD_Hanit()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If

      '    DMAcadExt.AcadDocument.InitDebug()
      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      If TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState = System.Data.ConnectionState.Open Then
         TPlServerDB.ServerDB.AddInitialize()
      End If




      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      If mfHanitControl Is Nothing OrElse mfHanitControl.IsDisposed Then
         mfHanitControl = New TopoUI.frmHanitControl
      End If
      'MessageBox.Show(CStr(
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfHanitControl)

      '

   End Sub
   <CommandMethod("FormView")> _
   Public Sub ActiveFormView()
      If mfHanitControl IsNot Nothing Then
         mfHanitControl.GeneralView(True)
      End If
      If mfPrjThemes IsNot Nothing Then
         mfPrjThemes.ActiveFormView(True)
      End If

   End Sub
   <CommandMethod("TestPrjF")> _
   Public Sub TestPrjF()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If


      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()

      If mfProjectFiles Is Nothing OrElse mfHanitControl.IsDisposed Then
         mfProjectFiles = New TopoUI.frmProjectFiles
      End If
      'MessageBox.Show(CStr(
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProjectFiles)
   End Sub

   <CommandMethod("UD_Rnd0")> _
   Public Sub UD_Rnd0()


      UnidivNet.UD_App.RoundAll(0)




   End Sub

   <CommandMethod("UD_Rnd2")> _
   Public Sub UD_Rnd2()


      UnidivNet.UD_App.RoundAll(2)




   End Sub

   <CommandMethod("UD_Rnd3")> _
   Public Sub UD_Rnd3()
      UnidivNet.UD_App.RoundAll(3)
   End Sub
   <CommandMethod("UD_Rnd6")> _
   Public Sub UD_Rnd6()


      UnidivNet.UD_App.RoundAll(6)




   End Sub
   <CommandMethod("UD_RndE")> _
   Public Sub UD_RndEnt()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity = Nothing
      Dim iDigits As Integer = 3
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
         Catch oEx As Exception
            Return
         End Try

         Select Case oEntA.GetRXClass().Name

            Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadLineName
               DMAcadExt.TplnSegment.Round(oEntA, True, iDigits)
         End Select
         oEditor.WriteMessage(vbCrLf)


      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

      ' UnidivNet.UD_App.RoundAll(3)




   End Sub
   <CommandMethod("UD_HL")> _
   Public Sub UD_HL()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If


      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      UnidivNet.UD_App.OpenDB()

      UnidivNet.UD_App.DrawHanitEntities()
      '	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")
      TPlServerDB.ServerDB.CurrentServerDB.Close()
      If TPlServerDB.ServerDB.CurrentProjectDB IsNot Nothing Then
         TPlServerDB.ServerDB.CurrentProjectDB.Close()

      End If
      '

   End Sub
   <CommandMethod("UD_Br")> _
   Public Sub UD_Border()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If

      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      '  TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      ' UnidivNet.UD_App.OpenDB()



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


      UnidivNet.UD_App.HanitBorders()
      '	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")




      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("ReMarks")> _
   Public Sub Reset_MarkBlocks()




      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      DMAcadExt.MarkBlock.ResetAllRefs()





      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TopoEx")> _
   Public Sub TopoExists()
      Dim bRes As Boolean = TopoManager.TopoCreator.TopologyExists("Stage_0")

      System.Windows.Forms.MessageBox.Show(bRes.ToString() & vbCrLf & "", "05_375")
   End Sub
   <CommandMethod("UD_Un")> _
   Public Sub UD_Union()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If

      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      '  TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
      ' UnidivNet.UD_App.OpenDB()



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


      UnidivNet.UD_App.ImportHanitBorders()
      '	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")




      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("UD_Points")> _
   Public Sub UD_PointsOut()
      If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
         DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
      End If


      TopoManager.TPlanGraph.TplnProject.InitAppName()

      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      '	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


      '	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control


      UnidivNet.UD_App.DrawPointsOut()
      '	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")
      TPlServerDB.ServerDB.CurrentServerDB.Close()
      If TPlServerDB.ServerDB.CurrentProjectDB IsNot Nothing Then
         TPlServerDB.ServerDB.CurrentProjectDB.Close()

      End If
      '

   End Sub
   <CommandMethod("IsLocked")> _
   Public Sub IsLocked()
      Dim bLocked As Boolean = DMAcadExt.AcadDocument.IsLocked()

      DMAcadExt.AcadDocument.WriteMessage("Is Locked? " & CStr(bLocked) & "; " & Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode.ToString())
   End Sub
   <CommandMethod("Unlock")> _
   Public Sub Unlock()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TplnClearMap")> _
   Public Sub ClearAllMap()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      FDO_Manager.RemoveAllConnections()
      FDO_Manager.RemoveAllResources()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TplnDelR")> _
   Public Sub TplnPlatfTest()
      Dim oGeoSpatPl As FDO.GeoSpatPl = New FDO.GeoSpatPl()

      FDO.GeoSpatPl.DeleteRecource()
   End Sub
   <CommandMethod("TplnPlt")> _
   Public Sub TplnDeleteRes()
      Dim oGeoSpatPl As FDO.GeoSpatPl = New FDO.GeoSpatPl()

      oGeoSpatPl.Ex_DefiningVectorfeatureSource()
   End Sub
   Public Sub TplnCloseA()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadTransaction.Terminate()
   End Sub


   <CommandMethod("TplnLTest")> _
   Public Sub TplnLayerTest()
      FDO.FDO_Manager.NewTestB()
   End Sub

   <CommandMethod("ResetLinks")> _
   Public Sub ResetLinks()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      Dim sLayers As String = "pclp001,pclp004,pCellK"
      Dim sBlockName As String = "1603"
      Dim sBlockNameA As String = "Cellno"

      DMAcadExt.dmLineCleanup.CopyLinks(sLayers)
      DMAcadExt.dmLineCleanup.CopyBlockRefs(sBlockName)
      DMAcadExt.dmLineCleanup.CopyBlockRefs(sBlockNameA)




      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub




   <CommandMethod("TplnGazAddOD")> _
   Public Sub TplnGazAddOD()
      prjPrograms.GazAddOD()
   End Sub
   Private Sub zzOpenApp(ByVal iApp As DMAcadExt.enApplications)
      DMAcadExt.DMApp.AppID = iApp

      TopoManager.TPlanGraph.TplnProject.InitAppName()
      TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()
      If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then

         TopoManager.TPlanGraph.TplnProject.InitializeList()
         System.Windows.Forms.MessageBox.Show(iApp.ToString(), "07_018")
         If mfTopoActions Is Nothing OrElse mfTopoActions.IsDisposed Then
            Select Case iApp
               Case DMAcadExt.enApplications.Taba
                  TopoDefs.InitTaba()
                  mfTopoActions = New frmTopoActionsT
               Case DMAcadExt.enApplications.TopoMaster
                  DMAcadExt.TopoDef.Format = 1
                  TopoDefs.InitTopoMaster()
                  mfTopoActions = New frmTopoActionsM
               Case DMAcadExt.enApplications.Unidiv
                  DMAcadExt.TopoDef.Format = 1
                  TopoDefs.InitUnidiv()
                  System.Windows.Forms.MessageBox.Show("zzOpenApp", "")
                  mfTopoActions = New frmTopoActionsUD
            End Select
         End If
         TopoManager.TPlanGraph.TplnProject.InitializeView()
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActions, True)
      End If
   End Sub

   <CommandMethod("TplnCleanup")> _
   Public Sub TplnCleanup()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.Mark()
      End If
   End Sub

   Public Shared Sub TestLayer()
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.TestSetLayer("1602", "TestLayer")
      DMAcadExt.AcadTransaction.Terminate()
   End Sub
   <CommandMethod("TestMPgon")> _
   Public Shared Sub TestMPgon()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      Dim oList As IList(Of Entity) = DMAcadExt.AcadTransaction.GetMPolygons("TplnParcelMPgon", OpenMode.ForRead)
      DMAcadExt.AcadDocument.WriteMessage("EntityCnt: " & CStr(oList.Count))



      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestBlock")> _
   Public Shared Sub TestBlock()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      DMAcadExt.AcadTransaction.OpenNewBlockTest("*U")
      Dim tGeoPoint As Point3d = New Point3d(0.0, 0.0, 0.0)
      Dim oPoint As DBPoint = New DBPoint(tGeoPoint)
      '	DMAcadExt.AcadTransaction.AddToNewBlock(oPoint)
      '	DMAcadExt.AcadTransaction.InsertNewBlock(False)
      '	DMAcadExt.AcadTransaction.GetAllBlocks()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestDoc")> _
   Public Shared Sub TestDocLock()
      'DMAcadExt.AcadDocument.SetActiveDoc(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      MessageBox.Show("TestDoc", "01_184a")

      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()




      ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(oPromptOpt)

      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ptRes.Status.ToString())
      Dim oLine As Line
      oLine = New Line(New Point3d(194100, 654000, 0), New Point3d(194200, 6541000, 0))
      DMAcadExt.AcadTransaction.AppendEntity(oLine)
   End Sub
   <CommandMethod("TestUnlock")> _
   Public Shared Sub TestUnlock()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      MessageBox.Show("TestUnlock", "01_184b")
   End Sub
   <CommandMethod("TestL")> _
   Public Sub GetLinksSet()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()

      Dim colSelectionLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      'Dim oPolyline As Polyline = New Polyline
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim ptResA As Autodesk.AutoCAD.EditorInput.PromptSelectionResult

      '	Dim colLines As ObjectIdCollection = New ObjectIdCollection
      Dim iTotal As Integer = 0
      Dim iFound As Integer = 0
      colSelectionLinks.Clear()

      Try
         oPromptOpt.AllowDuplicates = False
         oPromptOpt.SingleOnly = False
         oPromptOpt.MessageForAdding = "Add ***"
         oPromptOpt.MessageForRemoval = "Remove ***"

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
      End Try

      Dim oReslSet As Autodesk.AutoCAD.EditorInput.SelectionSet '= New Autodesk.AutoCAD.EditorInput.SelectionSetDelayMarshalled()
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet

      Dim taObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId

      Dim oSelEntity As Autodesk.AutoCAD.DatabaseServices.Entity

      Dim bAcadPoint As Boolean

      ptRes = oEditor.GetSelection(oPromptOpt)

      DMAcadExt.AcadDocument.WriteSingleMessage(ptRes.Status.ToString & "; Cnt=" & ptRes.Value().Count.ToString & "; ")
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oSelSet = ptRes.Value()
         ' DMAcadExt.AcadDocument.WriteSingleMessage(oSelSet.ToString & "; Cnt=" & oSelSet.GetType().ToString & "; ")
         '	zzTestSet(oSelSet, True)
         taObjIDs = oSelSet.GetObjectIds()
         colSelectionLinks.Add(taObjIDs(0))

         iFound = colSelectionLinks.Count - iTotal
         iTotal = colSelectionLinks.Count
         DMAcadExt.AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
         '    ptResA = oEditor.SelectImplied()
         ' oEditor.SetImpliedSelection(oReslSet)
         '    DMAcadExt.AcadDocument.WriteSingleMessage("++ " & ptRes.Status.ToString & "; Cnt=" & ptRes.Value().Count.ToString & "; ")
      ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
         colSelectionLinks.Clear()
         'Exit Do
      Else
         DMAcadExt.AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
         'Exit Do
      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("TestSS")> _
   Public Sub TestSS()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim iIndex As Integer
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      '  oPromptOpt.
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception

            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try
         ReDim Preserve taAcObjIDs(iIndex)
         taAcObjIDs(iIndex) = ptRes.ObjectId
         oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

         oEditor.SetImpliedSelection(oSelSet)

      End If

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   Private Shared Function CallB(tPoint As Point3d, ByRef tTransform As Matrix3d) As Autodesk.AutoCAD.EditorInput.SamplerStatus
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("CallBack")
      tTransform = Autodesk.AutoCAD.Geometry.Matrix3d.Scaling(2.0, tPoint)
   End Function
   <CommandMethod("TestDrag")> _
   Public Shared Sub TestDrag()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptSelOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptSelRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptDragOptions
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim oDragCallback As DragCallback = New DragCallback(AddressOf CallB)
      Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      ptSelRes = oEditor.GetSelection(oPromptSelOpt)

      If ptSelRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oSelSet = ptSelRes.Value
         oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptDragOptions(oSelSet, "DRAG", oDragCallback)
         ptPointRes = oEditor.Drag(oPromptOpt)
         oEditor.WriteMessage(ptPointRes.Status.ToString() & ":" & ptPointRes.StringResult)
      End If

   End Sub
   <CommandMethod("TestTr")> _
   Public Shared Sub TestTr()
      Dim sTopoName As String = Nothing
      Try
         sTopoName = DMAcadExt.AcadUtil.GetName("Topology Name ...")
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
         sTopoName = Nothing
      End Try
      If sTopoName IsNot Nothing Then
         TopoCreator.TestTrace(sTopoName, 0)
      End If

   End Sub
   <CommandMethod("TestDicL")> _
   Public Shared Sub TestDictionaryList()

      Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary
      Dim iNamedMode As OpenMode
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()


      dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(OpenMode.ForRead)
      '   MessageBox.Show(CStr(dicNamed.Count) & ":" & dsDictionaryName, "01_492")
      If dicNamed IsNot Nothing Then
         Try
            For Each oEntry As DBDictionaryEntry In dicNamed

               DMAcadExt.AcadDocument.WriteMessage("#199" & oEntry.m_key & "//" & oEntry.m_value.ToString())
            Next
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "ProjectData - TestDictionaryList")
         End Try

      Else
         MessageBox.Show("NamedDictionary is nothing !!!" & iNamedMode.ToString())
      End If

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TestSSet")> Public Shared Sub TestSSet()
      Dim moEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As DBObject = Nothing
      Dim oDxfCode As Object = DxfCode.Color

      Dim oaValues() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Operator), "<or") _
                                     , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5F))) _
                                     , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5E))) _
                                     , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5D))) _
                                     , New TypedValue(Convert.ToInt32(DxfCode.Operator), "or>")}
      Dim oaValues1() As TypedValue = {New TypedValue(DxfCode.LayerName, "pclp011"), New TypedValue(DxfCode.Color, 254)}
      '   Dim oaValues2() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5F)).ToString())}
      Dim oaValues2() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Handle), "19295")}


      Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter
      Dim oFilter1 As Autodesk.AutoCAD.EditorInput.SelectionFilter
      Dim oFilter2 As Autodesk.AutoCAD.EditorInput.SelectionFilter

      '     (or(And objType="*POLYLINE,CIRCLE"                 ))
      moEditor.WriteMessage(oDxfCode.GetType().ToString() & vbCrLf)
      moEditor.WriteMessage(New Handle(Convert.ToInt64(&H4B5F)).ToString() & vbCrLf)


      oPromptOpt.MessageForRemoval = "Remove ..."

      oPromptOpt.MessageForAdding = "Add ..."
      '  oPromptOpt.SingleOnly = False
      '  oPromptOpt.SinglePickInSpace = True
      oPromptOpt.AllowDuplicates = False

      'oaValues(0) =
      'oaValues(1) =
      'oaValues(2) =
      oFilter = New SelectionFilter(oaValues) '
      oFilter1 = New SelectionFilter(oaValues1)
      oFilter2 = New SelectionFilter(oaValues2)



      ptRes = moEditor.GetSelection(oPromptOpt, oFilter1)
      'ptRes = moEditor.GetSelection(oPromptOpt, oFilter1)



      '	oEditor.WriteMessage(ptRes.StringResult)
      'ptRes.Value
      '	Dim tAcObjID As ObjectId = ptRes.ObjectId
      If ptRes.Value IsNot Nothing Then
         '  moEditor.WriteMessage(CStr(ptRes.Value.Count))
      End If
      ' moEditor.WriteMessage(CStr(ptRes.Value.Count), ptRes.Status.ToString())
      moEditor.WriteMessage(ptRes.Status.ToString() & " !!!!!!!!!!!!!!!-!!!!!!!!!!!!!!-!!!!!!!!!-!!!!!!!!!!-!!!!!!!!!!!!!-!!!!!!!!!!-!!!!!")
   End Sub
   <CommandMethod("Test11")> _
   Public Sub SelectionTest()
      Dim ed As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim pso As PromptSelectionOptions = New PromptSelectionOptions()
      pso.SingleOnly = True
      pso.SinglePickInSpace = True
      Dim psr As PromptSelectionResult
      Dim ids As ObjectIdCollection = New ObjectIdCollection()
      Do
         psr = ed.GetSelection(pso)
         If (psr.Status <> PromptStatus.OK) Then
            Exit Do
         End If
         ids.Add(psr.Value.Item(0).ObjectId)
         ed.WriteMessage("\n{0} selected objects", ids.Count)
      Loop
   End Sub

   <CommandMethod("TestSSetA")> Public Shared Sub TestSSetA()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As DBObject = Nothing
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
      Dim colSelected As ObjectIdCollection = New ObjectIdCollection()
      Dim iIndex As Integer = -1
      Dim tAcObjID As ObjectId
      Dim oaValues(2) As TypedValue
      Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      '    oPromptOpt.SingleOnly = True


      oaValues(0) = New TypedValue(DxfCode.Handle, New Handle(&H4B5F))
      oaValues(1) = New TypedValue(DxfCode.Handle, New Handle(&H4B5E))
      oaValues(2) = New TypedValue(DxfCode.Handle, New Handle(&H4B5D))
      oFilter = New SelectionFilter(oaValues)
      oPromptOpt.SingleOnly = True
      oPromptOpt.SinglePickInSpace = False

      '  oPromptOpt.AllowSubSelections = True

      Do
         iIndex += 1
         oEditor.WriteMessage("8ii= " & iIndex.ToString() & "; " & oPromptOpt.SingleOnly.ToString & vbCrLf)


         ptRes = oEditor.GetSelection(oPromptOpt, oFilter)

         If ptRes.Status <> PromptStatus.OK Then
            Exit Do
         Else
            oEditor.WriteMessage("AS: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count) & "; " & ptRes.Status.ToString() & vbCrLf)
         End If





         For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
            tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
            If Not colSelected.Contains(tAcObjID) Then
               colSelected.Add(tAcObjID)

               DMAcadExt.AcadTransaction.Highlight(tAcObjID)


            End If
         Next
         '  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

         '  oEditor.SetImpliedSelection(oSelSet)
         '  oEditor.SetImpliedSelection(taAcObjIDs)

         ' oPromptOpt.ForceSubSelections = True
         oEditor.WriteMessage("TT: " & iIndex.ToString() & "; " & CStr(colSelected.Count) & vbCrLf)
      Loop
      DMCommon.Debug.MsgBox("09_763a", colSelected.Count, ptRes.Status)
      If False Then

         ReDim Preserve taAcObjIDs(colSelected.Count - 1)
         colSelected.CopyTo(taAcObjIDs, 0)

         DMCommon.Debug.MsgBox("09_764a", taAcObjIDs.GetUpperBound(0))
      End If

      'If taAcObjIDs.GetUpperBound(0) >= 0 Then
      '   oEditor.SetImpliedSelection(taAcObjIDs)
      'End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   <CommandMethod("TestSub")> Public Shared Sub TestSubentity()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptNestedEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptNestedEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptNestedEntityResult
      Dim oEnt As DBObject = Nothing
      ptRes = oEditor.GetNestedEntity(oPromptOpt)
      oEditor.WriteMessage(ptRes.StringResult)
      Dim tAcObjID As ObjectId = ptRes.ObjectId
   End Sub
   <CommandMethod("VPort")> Public Shared Sub VPort()
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadDocument.GetMinPoint()
      DMAcadExt.AcadTransaction.Terminate()
   End Sub
   <CommandMethod("TestSel")>
   Public Shared Sub TestSel()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point Or 1")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      Dim tVector As Vector2d
      oPromptOpt.AllowNone = True
      ptRes = oEditor.GetPoint(oPromptOpt)
      tVector = New Vector2d(ptRes.Value.X, ptRes.Value.Y)

      DMCommon.Debug.MsgBox("Status!", ptRes.Status, ptRes.Value, tVector.Length, tVector.Angle, 180 * tVector.Angle / Math.PI, ptRes.StringResult)
   End Sub

   <CommandMethod("TestDB")> _
   Public Shared Sub TestDB()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      DMAcadExt.AcadTransaction.OpenNewBlockDB()
      Dim tGeoPoint As Point3d = New Point3d(0.0, 0.0, 0.0)
      Dim oPoint As DBPoint = New DBPoint(tGeoPoint)

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestPrj")> _
   Public Shared Sub TestPrj()
      Dim oDB As Database = Nothing
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
      If oDB Is Nothing Then
         oProject = oMapApplication.ActiveProject
      Else

         System.Windows.Forms.MessageBox.Show(CStr(oDB.NumberOfSaves), "05_411")
         oProject = oMapApplication.GetProjectForDB(oDB)
         System.Windows.Forms.MessageBox.Show(CStr(oProject.MapActive), "05_412")
      End If
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oProject.Topologies

      System.Windows.Forms.MessageBox.Show(CStr(oTopos Is Nothing), "05_413")
   End Sub
   <CommandMethod("TplnBuild")> _
   Public Sub TplnBuild()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.CreateTopo()
      End If
   End Sub
   <CommandMethod("TplnKill")> _
   Public Sub TplnKill()

      If False Then
         If mfTopoActions IsNot Nothing Then
            mfTopoActions.DeleteTopo()
         End If
      End If
   End Sub
   <CommandMethod("ExecUnion")> _
   Public Sub ExecUnion()
      TopoOverlay.TopoSourceName = "TopoA"
      TopoOverlay.TopoOverlayName = "TopoB"
      TopoOverlay.Exec()
   End Sub
   <CommandMethod("UnionTplnM")> _
   Public Sub UnionTplnM()
      TopoOverlay.TopoSourceName = TopoManager.TPlanGraph.TplnLot.TopoName(DMAcadExt.enTopoPurpose.Proposed)
      TopoOverlay.TopoOverlayName = TopoManager.TPlanGraph.TplnParcel.TopoName
      TopoOverlay.TopoResultName = TopoManager.TPlanGraph.TplnLot.UnionTopoNameB(TopoManager.TPlanGraph.enTopoPurpose.Proposed)
      TopoOverlay.ExecTplan()
   End Sub
   <CommandMethod("UnionTplnK")> _
   Public Sub UnionTplnK()
      TopoOverlay.TopoSourceName = TopoManager.TPlanGraph.TplnLot.TopoName(DMAcadExt.enTopoPurpose.Approved)
      TopoOverlay.TopoOverlayName = TopoManager.TPlanGraph.TplnParcel.TopoName
      TopoOverlay.TopoResultName = TopoManager.TPlanGraph.TplnLot.UnionTopoNameB(TopoManager.TPlanGraph.enTopoPurpose.Approved)
      TopoOverlay.ExecTplan()
   End Sub
   <CommandMethod("H2Pl")> _
   Public Sub HatchToPolyline()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      Dim oHatch As Hatch
      Dim oHatchLoop As HatchLoop
      'Dim iHatchLoopType As HatchLoopTypes
      'Dim colCurves As Autodesk.AutoCAD.Geometry.Curve2dCollection
      Dim oPolyline As Polyline

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
            If oEnt.GetRXClass.Name = DMAcadExt.AcadConst.AcadHatchName Then
               oHatch = DirectCast(oEnt, Hatch)
               For iIndex As Integer = 0 To oHatch.NumberOfLoops - 1
                  oHatchLoop = oHatch.GetLoopAt(iIndex)
                  oPolyline = GeoUtilites.HatchLoopToPolyline(oHatchLoop)
                  If oPolyline IsNot Nothing Then
                     oPolyline.Layer = oHatch.Layer
                     oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
                     DMAcadExt.AcadTransaction.AppendEntity(oPolyline, False)
                  Else
                     MessageBox.Show("Polyline IsNot Nothing ")
                  End If
               Next
            End If
         Catch oEx As Exception
            Return
         End Try
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("AH2Pl")> _
   Public Sub AllHatchToPolyline()
      '	Dim dHatchToPolyline As DMAcadExt.AcadTransaction.Procedure = New DMAcadExt.AcadTransaction.Procedure(AddressOf GeoUtilites.HatchToPolyline)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      ''''''''''DMAcadExt.AcadTransaction.EnumModelSpaceObjectsBuffer(dHatchToPolyline, OpenMode.ForRead)

      Dim colDBObjects As Autodesk.AutoCAD.DatabaseServices.DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
      For Each oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject In colDBObjects
         '	DMAcadExt.AcadDocument.WriteMessage("Entity: " & oDBObject.GetRXClass().Name & ":" & oDBObject.Handle.ToString())
         GeoUtilites.HatchToPolyline(oDBObject)


      Next

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("Dst")> _
   Public Sub Dist()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select 1st Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEntA As Entity = Nothing
      Dim oEntB As Entity = Nothing
      Dim oCurve As Curve = Nothing
      Dim oDBPoint As DBPoint
      Dim oBlockRef As BlockReference
      Dim tPointA As Point3d
      Dim tPointB As Point3d
      Dim iOption As Integer = 0
      Dim dDist As Double
      Dim bContinue As Boolean = True
      Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
            DMAcadExt.AcadDocument.WriteMessage("A ID: " & oEntA.ObjectId.ToString() & vbCrLf)
         Catch oEx As Exception
            bContinue = False
         End Try
         If bContinue Then

            oPromptOpt.Message = "Select 2nd Entity"
            ptRes = oEditor.GetEntity(oPromptOpt)
            If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
               Try
                  oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
                  DMAcadExt.AcadDocument.WriteMessage("B ID: " & oEntB.ObjectId.ToString() & vbCrLf)
               Catch oEx As Exception
                  bContinue = False
               End Try
            End If
            If bContinue Then


               Select Case oEntA.GetRXClass.Name
                  Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName
                     oCurve = DirectCast(oEntA, Curve)
                     iOption = 1
                  Case DMAcadExt.AcadConst.AcadPointName
                     oDBPoint = DirectCast(oEntA, DBPoint)
                     tPointA = oDBPoint.Position
                     iOption = 2
                  Case DMAcadExt.AcadConst.AcadBlockRefName
                     oBlockRef = DirectCast(oEntA, BlockReference)
                     tPointA = oBlockRef.Position
                     iOption = 2
                  Case Else
                     DMAcadExt.AcadDocument.WriteMessage("EntityA: " & oEntA.GetRXClass().Name & ":" & oEntA.GetType().ToString())
                     bContinue = False
               End Select
               If bContinue And iOption <> 0 Then


                  Select Case oEntB.GetRXClass.Name
                     Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName
                        oCurve = DirectCast(oEntB, Curve)
                        If iOption = 1 Then
                           bContinue = False
                        Else
                           iOption = 3
                        End If
                     Case DMAcadExt.AcadConst.AcadPointName
                        oDBPoint = DirectCast(oEntB, DBPoint)
                        If iOption = 1 Then
                           tPointA = oDBPoint.Position
                           iOption = 3
                        ElseIf iOption = 2 Then
                           tPointB = oDBPoint.Position
                           iOption = 4
                        End If

                     Case DMAcadExt.AcadConst.AcadBlockRefName
                        oBlockRef = DirectCast(oEntB, BlockReference)
                        If iOption = 1 Then
                           tPointA = oBlockRef.Position
                           iOption = 3
                        ElseIf iOption = 2 Then
                           tPointB = oBlockRef.Position
                           iOption = 4
                        End If

                     Case Else
                        DMAcadExt.AcadDocument.WriteMessage("EntityB: " & oEntA.GetRXClass().Name & ":" & oEntA.GetType().ToString() & vbCrLf)
                        bContinue = False
                  End Select
                  If bContinue Then
                     DMAcadExt.AcadDocument.WriteMessage("Option: " & iOption.ToString() & vbCrLf)
                     Select Case iOption
                        Case 3
                           Dim tClosestPointOnCurve As Point3d
                           Try
                              tClosestPointOnCurve = oCurve.GetClosestPointTo(tPointA, False)
                              dDist = tPointA.DistanceTo(tClosestPointOnCurve)
                           Catch oEx As Exception
                              System.Windows.Forms.MessageBox.Show(oEx.Message, "21_211")
                           End Try

                        Case 4
                           dDist = tPointA.DistanceTo(tPointB)
                     End Select
                  End If
               End If
               DMAcadExt.AcadDocument.WriteMessage("Dist: " & dDist.ToString() & vbCrLf)
            End If
         End If
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("Test03")> _
   Public Sub Test03()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try
      End If
      If oEnt IsNot Nothing Then
         Dim tPickedPoint As Point3d = ptRes.PickedPoint
         Select Case oEnt.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadArcName
               Dim oArc As Arc = DirectCast(oEnt, Arc)

            Case DMAcadExt.AcadConst.AcadPolylineName
               Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)

            Case DMAcadExt.AcadConst.Acad2dPolylineName
               Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)

            Case DMAcadExt.AcadConst.AcadLineName
               Dim dVal As Double
               Dim oLine As Line = DirectCast(oEnt, Line)
               Dim oCurveSegment As DMAcadExt.TplnCurveSegment = New DMAcadExt.TplnCurveSegment(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint), DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint))
               ptPointRes = oEditor.GetPoint("Point..")
               If ptPointRes.Status = PromptStatus.OK Then
                  oEditor.WriteMessage("P " & CStr(ptPointRes.Value.X) & "," & CStr(ptPointRes.Value.Y) & vbCrLf)
                  oEditor.WriteMessage("Dist= " & CStr(oCurveSegment.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(ptPointRes.Value))) & " ")
                  dVal = oCurveSegment.IsOn(DMAcadExt.TPlnPoint.Point3dTo2d(ptPointRes.Value))
                  '  oEditor.WriteMessage("; IsOn= " & CStr(dVal))
               End If

            Case DMAcadExt.AcadConst.AcadPointName

            Case DMAcadExt.AcadConst.AcadHatchName

            Case DMAcadExt.AcadConst.AcadBlockRefName

            Case Else
               DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
         End Select
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("LnInfo")> _
   Public Sub LnInfo()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try
      End If
      If oEnt IsNot Nothing Then
         Dim tPickedPoint As Point3d = ptRes.PickedPoint
         Select Case oEnt.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadArcName
               Dim oArc As Arc = DirectCast(oEnt, Arc)
               zzArcInfo(oArc)
            Case DMAcadExt.AcadConst.AcadPolylineName
               Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
               zzSegmentInfo(oPolyline, tPickedPoint)
            Case DMAcadExt.AcadConst.Acad2dPolylineName
               Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
               zzPolyline2dInfo(oPolyline)
            Case DMAcadExt.AcadConst.AcadLineName
               Dim oLine As Line = DirectCast(oEnt, Line)
               zzLineInfo(oLine)
            Case DMAcadExt.AcadConst.AcadPointName

            Case DMAcadExt.AcadConst.AcadHatchName

            Case DMAcadExt.AcadConst.AcadBlockRefName

            Case Else
               DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
         End Select
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("EntInfo")> _
   Public Sub EntInfo()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            Return
         End Try
      End If
      If oEnt IsNot Nothing Then
         DMAcadExt.AcadDocument.WriteMessage(" Handle:" & oEnt.Handle.ToString() & "; ID:" & oEnt.ObjectId.ToString())
         Select Case oEnt.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadArcName
               Dim oArc As Arc = DirectCast(oEnt, Arc)
               zzArcInfo(oArc)
            Case DMAcadExt.AcadConst.AcadPolylineName
               Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
               zzPolylineInfo(oPolyline, True)
            Case DMAcadExt.AcadConst.Acad2dPolylineName
               Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
               zzPolyline2dInfo(oPolyline)
            Case DMAcadExt.AcadConst.AcadLineName
               Dim oLine As Line = DirectCast(oEnt, Line)
               zzLineInfo(oLine)
            Case DMAcadExt.AcadConst.AcadPointName
               Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
               zzPointInfo(oDBPoint)
            Case DMAcadExt.AcadConst.AcadPointName
               Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
               zzPointInfo(oDBPoint)
            Case DMAcadExt.AcadConst.AcadHatchName
               Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = DirectCast(oEnt, Hatch)
               zzHatchInfo(oHatch)
            Case DMAcadExt.AcadConst.AcadBlockRefName
               Dim oBlockReference As BlockReference = DirectCast(oEnt, BlockReference)
               zzBlockRefInfo(oBlockReference)
            Case Else
               DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
         End Select
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("NodeList")> _
   Public Sub NodeList()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Link")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
         Catch oEx As Exception
            Return
         End Try
      End If
      If oEnt IsNot Nothing Then
         Select Case oEnt.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadArcName
               Dim oArc As Arc = DirectCast(oEnt, Arc)
               zzArcInfo(oArc)
            Case DMAcadExt.AcadConst.AcadPolylineName
               Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
               zzPolylineInfo(oPolyline)
            Case DMAcadExt.AcadConst.Acad2dPolylineName
               Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
               zzPolyline2dInfo(oPolyline)
            Case DMAcadExt.AcadConst.AcadLineName
               Dim oLine As Line = DirectCast(oEnt, Line)
               zzLineInfo(oLine)
            Case DMAcadExt.AcadConst.AcadPointName
               Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
               zzPointInfo(oDBPoint)
            Case Else
               DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
         End Select
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TopoList")> _
   Public Sub TopoList()

      Dim saAllTopoNames() As String = Nothing
      Dim saUnionTopoNames() As String = Nothing


      ODEditor.GetTopoNames(saAllTopoNames, saUnionTopoNames)
      Dim sAllOut As String = Join(saAllTopoNames, vbCrLf)
      Dim sUnionOut As String = Join(saAllTopoNames, vbCrLf)

      System.Windows.Forms.MessageBox.Show(sAllOut & vbCrLf & sUnionOut)
   End Sub
   <CommandMethod("DispTopo")> _
   Public Sub DispTopo()
      mfTopoView = New frmTopoView
      mfTopoView.ShowDialog()
      mfTopoView.Dispose()
   End Sub
   <CommandMethod("DispTpln")> _
   Public Sub DispTpln()
      ' mfTplnView = New TPlanGraph.frmTplnView
      '  mfTplnView.ShowDialog()
      '  mfTplnView.Dispose()
      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
         If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
            mfTplnView = New frmTplnView
         End If

         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
      End If
   End Sub




   <CommandMethod("TplnCalc")> Public Sub TplnCalc()
      UnionTplnM()
      UnionTplnK()


   End Sub
   <CommandMethod("TestColor", CommandFlags.Session)> Public Shared Sub TestColor()
      'Autodesk.AutoCAD.Windows
      Dim oColorDialog As Autodesk.AutoCAD.Windows.ColorDialog
      oColorDialog = New Autodesk.AutoCAD.Windows.ColorDialog()
      oColorDialog.ShowDialog()
      Dim oColor As Autodesk.AutoCAD.Colors.Color = oColorDialog.Color

      MessageBox.Show(oColor.ColorValue.Name, "17_998")
      MessageBox.Show(oColor.ColorNameForDisplay, "17_999")
   End Sub
   <CommandMethod("ToMPoly", CommandFlags.Session)> Public Shared Sub GetToMPolygon()
      Const sPolylineLayer As String = "TplnParcelsClosed"
      Const sMPolygonLayer As String = "TplnParcelMpgons"

      Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      Dim oMPoly As MPolygon
      Dim tAcObjID As ObjectId

      For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolylines
         oMPoly = New MPolygon()
         oMPoly.AppendLoopFromBoundary(oPolygon, False, 0.001)
         oMPoly.Layer = sMPolygonLayer
         tAcObjID = DMAcadExt.AcadTransaction.AppendEntity(oMPoly)
      Next

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TestMPoly", CommandFlags.Session)> Public Shared Sub TestMPoly()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ptEntRes = oEditor.GetEntity(oPromptOpt)
      If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try

            Dim oDBObject As DBObject
            oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

            If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


               Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)

               Dim oMPoly As MPolygon = New MPolygon()

               oMPoly.AppendLoopFromBoundary(oPolyline, False, 0.001)
               DMAcadExt.AcadDocument.WriteMessage("Before: " & oMPoly.GetType().ToString & ";" & oMPoly.GetRXClass().Name)
               Dim tAcObjID As ObjectId = DMAcadExt.AcadTransaction.AppendEntity(oMPoly)
               DMAcadExt.AcadDocument.WriteMessage("after: " & oMPoly.GetType().ToString & ";" & oMPoly.GetRXClass().Name)
               DMAcadExt.AcadDocument.WriteMessage("MPolyId: " & tAcObjID.ToString() & ";" & oMPoly.ObjectId.ToString())
               Dim oMPDBObject As DBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               DMAcadExt.AcadDocument.WriteMessage("Type: " & oMPDBObject.GetType().ToString & ";" & oMPDBObject.GetRXClass().Name)
               Dim colObj As Autodesk.AutoCAD.DatabaseServices.DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()

               For i As Integer = 0 To colObj.Count - 1
                  oDBObject = colObj.Item(i)
                  DMAcadExt.AcadDocument.WriteMessage("All: " & oDBObject.GetType().ToString & ";" & oDBObject.GetRXClass().Name & ";" & oDBObject.ObjectId.ToString())
               Next
            End If

            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
         End Try

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub


   <CommandMethod("TestPMP", CommandFlags.Session)> Public Shared Sub TestPointInMPoly()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      '		Dim oMPoly As MPolygon
      Dim colLoops As IntegerCollection
      Dim bExists As Boolean
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


      moMPolygonA = New MPolygon()
      Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

      Do
         oEditor.WriteMessage("Start: " & vbCrLf)
         ptEntRes = oEditor.GetEntity(oPromptOpt)
         If ptEntRes.Status = PromptStatus.OK Then
            Try

               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)
               oEditor.WriteMessage("oEnt: " & vbCrLf)
            Catch oEx As Exception
               oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
               DMAcadExt.AcadTransaction.Abort()
               DMAcadExt.AcadDocument.Unlock()
               Return
            End Try
            Select Case oEnt.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadPolylineName
                  Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     moMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))
                     DMAcadExt.AcadTransaction.Abort()
                     DMAcadExt.AcadDocument.Unlock()
                     Return
                  End Try
               Case DMAcadExt.AcadConst.Acad2dPolylineName
                  Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     moMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))
                     DMAcadExt.AcadTransaction.Abort()
                     DMAcadExt.AcadDocument.Unlock()
                     Return
                  End Try

            End Select
         Else
            Exit Do
         End If
      Loop
      If bExists Then
         Try
            moMPolygonA.BalanceTree()
            For Each oDBObject As DBObject In colDBObject
               '	oDBObject.Erase()
            Next

            DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
            moMPolygonA.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
            DMAcadExt.AcadTransaction.AppendEntity(moMPolygonA)
            DMAcadExt.AcadTransaction.CloseModelSpace()


         Catch oEx As Exception
            oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))
            DMAcadExt.AcadTransaction.Abort()
            DMAcadExt.AcadDocument.Unlock()
            Return
         End Try
      Else
         System.Windows.Forms.MessageBox.Show("???", "21_223")
      End If

      Try
         Dim iLoopInd As Integer
         Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection '= oMPolygonA.GetLoopDirection(1)
         If moMPolygonA IsNot Nothing Then
            ptPointRes = oEditor.GetPoint("Point..")
            If ptPointRes.Status = PromptStatus.OK Then
               oEditor.WriteMessage("P " & CStr(ptPointRes.Value.X) & "," & CStr(ptPointRes.Value.Y) & vbCrLf)
               colLoops = moMPolygonA.IsPointInsideMPolygon(ptPointRes.Value, 0.001)
               oEditor.WriteMessage("LoopCount " & CStr(colLoops.Count) & vbCrLf)
               For iIndex As Integer = 0 To colLoops.Count - 1
                  iLoopInd = colLoops.Item(iIndex)
                  iDir = moMPolygonA.GetLoopDirection(iLoopInd)

                  oEditor.WriteMessage("&& " & CStr(iIndex) & ":" & "N=" & CStr(moMPolygonA.NumMPolygonLoops) & ";" & iDir.ToString() & "||" & CStr(colLoops.Count) & vbCrLf)
               Next
            End If
         End If
         '&& 0:1||1
         DMAcadExt.AcadDocument.CloseMessage()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
      End Try




      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub



   <CommandMethod("TestPLine", CommandFlags.Session)> Public Shared Sub TestPLine()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim ptDoubleRes As Autodesk.AutoCAD.EditorInput.PromptDoubleResult

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ptEntRes = oEditor.GetEntity(oPromptOpt)
      If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim tStartPoint As Point2d
            Dim tNextPoint As Point2d
            Dim tPoint As Point2d
            Dim oDBObject As DBObject
            oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

            If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


               Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
               oPolyline.Reset(True, 2)

               If False Then
                  ptDoubleRes = oEditor.GetDouble("Enter new value for tolerance <" & CStr(mdTolerance) & ">:")
                  If ptDoubleRes.Status = PromptStatus.OK Then
                     mdTolerance = ptDoubleRes.Value
                  End If
                  If ptDoubleRes.Status <> PromptStatus.Cancel Then
                     DMAcadExt.dmLineCleanup.StraightenPolylineTest(oPolyline, mdTolerance)
                  End If
               End If


               If False Then
                  For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
                     tPoint = oPolyline.GetPoint2dAt(iIndex)
                     DMAcadExt.AcadDocument.WriteMessage("Before=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
                     If iIndex = 0 Then
                        tStartPoint = oPolyline.GetPoint2dAt(iIndex)
                        DMAcadExt.AcadDocument.WriteMessage("Start=" & CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y))
                     ElseIf iIndex = 1 Then
                        tNextPoint = oPolyline.GetPoint2dAt(iIndex)
                        oPolyline.AddVertexAt(1, New Point2d(tStartPoint.X, tNextPoint.Y), 0.0, 0, 0)
                     End If
                     tPoint = oPolyline.GetPoint2dAt(iIndex)
                     DMAcadExt.AcadDocument.WriteMessage("After=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
                  Next
               End If
            End If

            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
         End Try

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("PLineList", CommandFlags.Session)> Public Shared Sub PLineList()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ptEntRes = oEditor.GetEntity(oPromptOpt)
      If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try

            Dim tPoint As Point2d
            Dim iSegmentType As SegmentType
            Dim oDBObject As DBObject
            oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

            If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
               Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
               For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
                  tPoint = oPolyline.GetPoint2dAt(iIndex)
                  iSegmentType = oPolyline.GetSegmentType(iIndex)
                  DMAcadExt.AcadDocument.WriteMessage("iIndex=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y) & " Type=" & iSegmentType.ToString())

               Next
            End If


            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
         End Try

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestPLineA", CommandFlags.Session)> Public Shared Sub TestPLineA()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim ptDoubleRes As Autodesk.AutoCAD.EditorInput.PromptDoubleResult
      Dim oTopoDef As DMAcadExt.TopoDef = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ptEntRes = oEditor.GetEntity(oPromptOpt)
      If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim tStartPoint As Point2d
            Dim tNextPoint As Point2d
            Dim tPoint As Point2d
            Dim oDBObject As DBObject

            oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

            If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


               Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)

               ptDoubleRes = oEditor.GetDouble("Enter new value for tolerance <" & CStr(mdTolerance) & ">:")
               If ptDoubleRes.Status = PromptStatus.OK Then
                  mdTolerance = ptDoubleRes.Value
               End If
               If ptDoubleRes.Status <> PromptStatus.Cancel Then
                  DMAcadExt.dmLineCleanup.StraightenPolylineZ(oPolyline, True, mdTolerance, oTopoDef, False)
               End If

               If False Then
                  For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
                     tPoint = oPolyline.GetPoint2dAt(iIndex)
                     DMAcadExt.AcadDocument.WriteMessage("Before=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
                     If iIndex = 0 Then
                        tStartPoint = oPolyline.GetPoint2dAt(iIndex)
                        DMAcadExt.AcadDocument.WriteMessage("Start=" & CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y))
                     ElseIf iIndex = 1 Then
                        tNextPoint = oPolyline.GetPoint2dAt(iIndex)
                        oPolyline.AddVertexAt(1, New Point2d(tStartPoint.X, tNextPoint.Y), 0.0, 0, 0)
                     End If
                     tPoint = oPolyline.GetPoint2dAt(iIndex)
                     DMAcadExt.AcadDocument.WriteMessage("After=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
                  Next
               End If
            End If

            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
         End Try

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("EraseOD", CommandFlags.Session)> Public Shared Sub EraseAllODTables()

      ' Dim oLinkTable As DMAcadExt.ODTable
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim sTableName As String = "LotKParcel"
      Dim sPolylineLayer As String = "LotKParcel"
      '    Dim iTestCounter As Integer
      Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()

      DMAcadExt.ODTable.EraseAllTables()

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   <CommandMethod("TestOD", CommandFlags.Session)> Public Shared Sub TestOD()
      Dim oLinkTable As DMAcadExt.ODTable
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim sTableName As String = "LotKParcel"
      Dim sPolylineLayer As String = "LotKParcel"
      Dim iTestCounter As Integer
      Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing

      Dim dValue As Double
      Dim sHandle As String
      Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()

      oLinkTable = New DMAcadExt.ODTable(sTableName)
      If oLinkTable.Exists Then
         iTestCounter = 0

         lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, OpenMode.ForRead)
         For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolylines

            iTestCounter += 1
            sHandle = oPolygon.Handle.ToString()
            Try
               oODRec = oLinkTable.GetODRecord(oPolygon.ObjectId)

            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadFDO_Overlay_9")
               System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sTableName, "04_200yy")
            End Try
            If oODRec IsNot Nothing Then
               '	oODRec.Init()
               Dim iFeatureID As Integer
               Dim iParcelID As Integer
               Dim iLotID As Integer

               Try
                  dValue = oODRec.Item(0).DoubleValue
                  iFeatureID = Convert.ToInt32(dValue)
                  dValue = oODRec.Item(1).DoubleValue
                  iParcelID = Convert.ToInt32(dValue)
                  dValue = oODRec.Item(3).DoubleValue
                  iLotID = Convert.ToInt32(dValue)
                  If iTestCounter < 24 Then
                     oEditor.WriteMessage("27-- " & ":" & CStr(iFeatureID) & ":" & CStr(iParcelID) & ":" & CStr(iLotID) & vbCrLf)
                  End If

               Catch oEx As Exception

                  System.Windows.Forms.MessageBox.Show(oEx.Message, "04_208")
               End Try

            End If

         Next
      Else
         oEditor.WriteMessage("25-- Not exists" & ":" & CStr(sTableName) & vbCrLf)
      End If



      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("TestPrjXData", CommandFlags.Session)> Public Shared Sub TestPrjXData()

      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taObjIDs() As ObjectId
      Dim oXDataPrjFile As DMAcadExt.TplnXDataPrjFile
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataPrjFile.XDataAppName)
      '   ptRes = oEditor.GetEntity(oPromptOpt)
      Dim oPromptSelOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptSelRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As DBObject = Nothing
      ptSelRes = oEditor.GetSelection(oPromptSelOpt)

      If ptSelRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         oSelSet = ptSelRes.Value
         taObjIDs = oSelSet.GetObjectIds()
         For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)

            oEnt = DMAcadExt.AcadTransaction.GetDBObject(taObjIDs(iIndex), OpenMode.ForWrite)
            oXDataPrjFile = New DMAcadExt.TplnXDataPrjFile(2)
            oXDataPrjFile.ID = 3
            oXDataPrjFile.Priority = 300
            oEnt.XData = oXDataPrjFile.GetResBuffer()
         Next
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   <CommandMethod("TestXData", CommandFlags.Session)> Public Shared Sub TestXData()
      Const sAppName As String = "SourceID"
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(sAppName)
            If bRes Then
               Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
               oVal = New TypedValue(1001, sAppName)
               oResBuffer.Add(oVal)
               oVal = New TypedValue(1071, 32768)
               oResBuffer.Add(oVal)
               oVal = New TypedValue(1005, oEnt.Handle)
               oResBuffer.Add(oVal)

               oEnt.XData = oResBuffer
            End If
            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
         End Try

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("GetXData", CommandFlags.Session)> Public Shared Sub GetXData()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oResBuffer As ResultBuffer
      Dim shCode As Short
      Dim oValue As Object
      Dim sValue As String
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
            oResBuffer = oEnt.XData
            Dim taTypedValues() As TypedValue = oResBuffer.AsArray()
            For iIndex As Integer = 0 To taTypedValues.GetUpperBound(0)
               shCode = taTypedValues(iIndex).TypeCode
               oValue = taTypedValues(iIndex).Value
               If shCode = 1004S Then
                  Dim baVal() As Byte = DirectCast(oValue, Byte())
                  For iIndexAr As Integer = 0 To baVal.GetUpperBound(0)
                     DMAcadExt.AcadDocument.WriteMessage("Index=" & CStr(iIndexAr) & ", Value=" & baVal(iIndexAr).ToString() & ", Type=" & baVal(iIndexAr).GetType().ToString())
                     '	DMAcadExt.AcadDocument.WriteMessage("Type=" & oValue.GetType().ToString())

                  Next
               Else
                  sValue = Replace(oValue.ToString(), """", """""")
                  DMAcadExt.AcadDocument.WriteMessage("Code=" & CStr(shCode) & ", Value=" & sValue & ", Type=" & oValue.GetType().ToString())

               End If
               '	DMAcadExt.AcadDocument.WriteMessage("Type=" & oValue.GetType().ToString())

            Next

            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception

         End Try

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("ClearOD", CommandFlags.Session)> Public Shared Sub ClearOD()

      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
            DMAcadExt.AcadMapApp.ClearOD(oEnt)
            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
         End Try

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TestArc", CommandFlags.Session)> Public Shared Sub TestArc()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Arc")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      '     DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      ptRes = oEditor.GetEntity(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            Dim oArc As Arc = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead), Arc)
            DMAcadExt.AcadDocument.WriteMessage("Center=" & CStr(oArc.Center.X) & "," & CStr(oArc.Center.Y))
            DMAcadExt.AcadDocument.WriteMessage("Start,End-Total=" & CStr(oArc.StartAngle) & "," & CStr(oArc.EndAngle) & "<>" & CStr(oArc.TotalAngle))
            DMAcadExt.AcadDocument.WriteMessage("Radius,Length=" & CStr(oArc.Radius) & "," & CStr(oArc.Length))
            Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc)

            DMAcadExt.AcadDocument.WriteMessage("MidPoint=" & CStr(oTplnArc.GetMidPoint().ToString()))


            ''''''''''''''''''''  DMAcadExt.dmLineCleanup.StraightenArcTest(oArc, 0.4, False, Nothing, False)
            DMAcadExt.AcadDocument.CloseMessage()
         Catch oEx As Exception

         End Try

      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   <CommandMethod("DocIsLock")> Public Sub DocIsLock()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("CreateTopo", CommandFlags.Session)> Public Shared Sub CreateTopo()
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim colLines As ObjectIdCollection
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim tResultODTable As Autodesk.Gis.Map.Topology.ObjectDataTable = New Autodesk.Gis.Map.Topology.ObjectDataTable()
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      Dim sTopoName As String = Nothing
      Try
         sTopoName = DMAcadExt.AcadUtil.GetName("Topology Name ...")
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
         sTopoName = Nothing
      End Try
      If sTopoName IsNot Nothing Then
         colLines = DMAcadExt.AcadUtil.GetLinesA()
         If colLines IsNot Nothing Then
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
            DMAcadExt.AcadTransaction.Start()
            DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
            Try
               oTopos.Create(sTopoName, colLines, colNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, Autodesk.Gis.Map.Topology.CreateOptions.IgnoreIncompleteArea, 1.1)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "")
            End Try

            DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
         End If
      End If
   End Sub


   <CommandMethod("ScanLayer")> Public Sub ScanLayer()
      Dim sLayerName As String = Nothing
      Try
         sLayerName = DMAcadExt.AcadUtil.GetName("Enter Layer Name ...")

      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
         sLayerName = Nothing
      End Try
      If sLayerName IsNot Nothing Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)
         DMAcadExt.AcadTransaction.ScanLayer(sLayerName)
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
      End If

   End Sub
   <CommandMethod("TplnClose")> Public Sub TplnClose()

      If True Then
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadTransaction.Terminate()

         If mfTopoActions IsNot Nothing Then
            If Not mfTopoActions.IsDisposed Then
               mfTopoActions.Close()
               mfTopoActions.Dispose()
            End If
            mfTopoActions = Nothing
         End If
         If mfTplnView IsNot Nothing Then
            If Not mfTplnView.IsDisposed Then
               mfTplnView.Close()
               mfTplnView.Dispose()
            End If
            mfTplnView = Nothing
         End If
         TopoManager.TPlanGraph.TplnProject.Close()
         BamashNet.bmBamash.Close()

      End If

   End Sub
   <CommandMethod("Strc")> _
   Public Sub StraightenCurve()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ptRes = oEditor.GetEntity(oPromptOpt)

      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         DMAcadExt.dmLineCleanup.StraightenCurve(ptRes.ObjectId, 0.001, True, "")


      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   <CommandMethod("TestLockA")> _
   Public Sub TestLockA()
      DMAcadExt.AcadDocument.TestAcadDoc("TestLockA")
   End Sub

   <CommandMethod("TestLockB")> _
   Public Sub TestLockB()
      DMAcadExt.AcadDocument.TestAcadDoc("Bef:TestLockA")
      DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Read, True)
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.TestAcadDoc("Aft:TestLockA")

   End Sub
   Public Sub TestLockC()
      DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Read, True)
   End Sub

   Private Sub mfTopoActions_AppExit() Handles mfTopoActions.AppExit
      If mfTplnView IsNot Nothing Then
         mfTplnView.Close()
         mfTplnView.Dispose()
         mfTplnView = Nothing
      End If

      If mfTopoActions IsNot Nothing Then

         If mfTopoActions.Visible Then
            mfTopoActions.Close()
         End If
         mfTopoActions.Dispose()
         mfTopoActions = Nothing
      End If

   End Sub
   Private Sub mfTopoActions_Calculate() Handles mfTopoActions.Calculate
      If mfTplnView IsNot Nothing Then
         mfTplnView.Reset()
      End If
   End Sub

   Private Sub mfTplnView_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mfTplnView.Deactivate
      If mfTplnView.DialogResult = DialogResult.Yes Then
         mfTplnView.DialogResult = DialogResult.No
         mfTplnView.Dispose()
         mfTplnView = Nothing
      End If
   End Sub


   Private Sub moAcadDocument_CommandEnded(ByVal oSender As System.Object, ByVal e As Autodesk.AutoCAD.ApplicationServices.CommandEventArgs) Handles moAcadDocument.CommandEnded
      System.Windows.Forms.MessageBox.Show(e.GlobalCommandName, "29_767")
      Select Case e.GlobalCommandName
         Case "LAYER"
            mfTopoActions.RefreshLayers()
         Case "MAPTOPOCREATE", "MAPTOPODEL", "MAPTOPOREN"
            mfTopoActions.RefreshTopo()
         Case Else
            ' System.Windows.Forms.MessageBox.Show(e.GlobalCommandName, "29_769")
      End Select

   End Sub

   Private Sub mfTopoActions_DisplayBasePgon(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer) Handles mfTopoActions.DisplayBasePgon
      If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
         mfTplnView = New frmTplnView
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
      End If



      If mfTplnView IsNot Nothing Then
         mfTplnView.Visible = True
         mfTplnView.SetPgonFilter(iOverlayIndex, bParcel, iTopoID)
      End If
   End Sub

   Private Sub mfTopoActions_FormatChanged() Handles mfTopoActions.FormatChanged
      If mfTplnView IsNot Nothing Then
         mfTplnView.RefreshFormat()
      End If
   End Sub
   Private Sub mfTopoActions_PaintScaleChanged() Handles mfTopoActions.PaintScaleChanged
      If mfTplnView IsNot Nothing Then
         mfTplnView.RefreshPaintScale()
      End If
   End Sub


   Private Sub mfTopoActions_Deactivate(ByVal oSender As System.Object, ByVal e As System.EventArgs)   '''''''''''''''' Handles mfTopoActions.Deactivate
      If mfTopoActions IsNot Nothing AndAlso mfTopoActions.DialogResult = DialogResult.Yes Then
         mfTopoActions.DialogResult = DialogResult.No
         mfTopoActions.Dispose()
         mfTopoActions = Nothing
         Try
            BamashNet.bmBamash.Close()
         Catch oEx As Exception
         End Try
      End If
   End Sub
   Private Shared Function zzGetMPolygon(bEraseSource As Boolean) As MPolygon
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
      Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

      Dim bExists As Boolean
      Dim oEnt As DBObject = Nothing



      Dim oMPolygon As MPolygon = New MPolygon()
      Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

      Do
         oEditor.WriteMessage("Start: " & vbCrLf)
         ptEntRes = oEditor.GetEntity(oPromptOpt)
         If ptEntRes.Status = PromptStatus.OK Then
            Try

               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)
               oEditor.WriteMessage("oEnt: " & vbCrLf)
            Catch oEx As Exception
               oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
               DMAcadExt.AcadTransaction.Abort()
               DMAcadExt.AcadDocument.Unlock()
               Return Nothing
            End Try
            Select Case oEnt.GetRXClass().Name
               Case DMAcadExt.AcadConst.AcadPolylineName
                  Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))

                     Return Nothing
                  End Try
               Case DMAcadExt.AcadConst.Acad2dPolylineName
                  Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
                  If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
                     oPolyline.Closed = True
                  End If

                  Try
                     oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
                     bExists = True
                     colDBObject.Add(oEnt)
                  Catch oEx As Exception
                     oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))

                     Return Nothing
                  End Try

            End Select
         Else
            Exit Do
         End If
      Loop
      If bExists Then
         Try
            oMPolygon.BalanceTree()
            If bEraseSource Then
               For Each oDBObject As DBObject In colDBObject
                  oDBObject.Erase()
               Next
            End If


            DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
            oMPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
            DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
            DMAcadExt.AcadTransaction.CloseModelSpace()


         Catch oEx As Exception
            oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))

            Return Nothing
         End Try
      Else
         System.Windows.Forms.MessageBox.Show("???", "21_223")
      End If
      Return oMPolygon
   End Function

   Private Sub zzArcInfo(ByVal oArc As Arc)
      Dim tCurrentPoint3d As Point3d
      Dim tCurrentPoint As Point2d

      Dim sHandle As String = CStr(oArc.Handle.Value)
      tCurrentPoint3d = oArc.StartPoint

      DMAcadExt.AcadDocument.WriteMessage("A R C   Handle: " & sHandle)
      DMAcadExt.AcadDocument.WriteMessage("Start Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint3d))
      tCurrentPoint3d = oArc.EndPoint
      DMAcadExt.AcadDocument.WriteMessage("End Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint3d))
      DMAcadExt.AcadDocument.WriteMessage("Radius:" & CStr(oArc.Radius))

      Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc)
      tCurrentPoint = oTplnArc.GetMidPoint()
      DMAcadExt.AcadDocument.WriteMessage("Middle Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))

      '   DMAcadExt.AcadDocument.WriteMessage("Start Angle:" & oTplnArc.StartAngle)
      '   DMAcadExt.AcadDocument.WriteMessage("End Angle:" & oTplnArc.EndAngle)
      oTplnArc.PrintInfo()

   End Sub
   Private Shared Sub zzPolylineInfo(ByVal oPolyline As Polyline, Optional bOrigin As Boolean = False)

      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim tCurrentPoint As Point2d
      If bOrigin Then
         DMAcadExt.KeyPoint.SetOrigin(DMAcadExt.AcadDocument.GetExtMinPoint())
      End If
      DMAcadExt.AcadDocument.WriteMessage("Polyline Points(" & CStr(iVerticesUB) & "):")
      For iIndex As Integer = 0 To iVerticesUB
         tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
         DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & ":" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint) & "; B=" & oPolyline.GetBulgeAt(iIndex))
         If bOrigin Then
            DMAcadExt.AcadDocument.WriteMessage("       " & DMAcadExt.TPlnPoint.DispPoint(DMAcadExt.KeyPoint.GetPointByOrigin(tCurrentPoint)))
         End If
      Next
   End Sub
   Private Shared Sub zzSegmentInfo(ByVal oPolyline As Polyline, tPoint As Point3d)
      Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
      Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
      Dim iIndex As Integer = Convert.ToInt32(Math.Floor(dPointParameter))
      Dim iNextIndex As Integer = (iIndex + 1) Mod oPolyline.NumberOfVertices
      '   DMAcadExt.AcadDocument.WriteMessage(vbCrLf & CStr(999) & ":" & dPointParameter.ToString() & "; " & DMAcadExt.TPlnPoint.DispPoint(tClosestPointOnCurve))
      Dim tNextVertex, tVertex As Point3d
      Dim dNextBulge, dBulge As Double
      Dim sNextBulge, sBulge As String
      ' Return
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim iSegmentType As SegmentType
      iSegmentType = oPolyline.GetSegmentType(iIndex)
      tVertex = oPolyline.GetPoint3dAt(iIndex)
      dBulge = oPolyline.GetBulgeAt(iIndex)
      If dBulge <> 0.0 Then
         sBulge = ";B=" & FormatNumber(dBulge, 4)
      Else
         sBulge = String.Empty
      End If
      tNextVertex = oPolyline.GetPoint3dAt(iNextIndex)
      dNextBulge = oPolyline.GetBulgeAt(iNextIndex)

      If dNextBulge <> 0.0 Then
         sNextBulge = ";B=" & FormatNumber(dNextBulge, 4)
      Else
         sNextBulge = String.Empty
      End If
      Dim dAngleX As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.XAxis)
      Dim dAngleY As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.YAxis)

      '  DMAcadExt.AcadDocument.WriteMessage("Polyline Points: " & tClosestPointOnCurve.ToString() & "; " & dPointParameter.ToString())
      DMAcadExt.AcadDocument.WriteMessage(vbCrLf & ":" & iSegmentType.ToString() & "; " & zzGetDir(tVertex, tNextVertex) & ", Angle X: " & (180.0 * dAngleX / Math.PI).ToString() & " Angle Y: " & (180.0 * dAngleY / Math.PI).ToString() & vbCrLf & CStr(iIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tVertex) & sBulge & vbCrLf & CStr(iNextIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tNextVertex) & sNextBulge)
   End Sub
   Private Shared Function zzGetDir(dFirst As Double, dLast As Double) As String
      Select Case Math.Sign(dLast - dFirst)
         Case 1
            Return "+"
         Case 0
            Return "="
         Case -1
            Return "-"
         Case Else
            Return String.Empty
      End Select
   End Function
   Private Shared Function zzGetDir(tFirstPoint As Point3d, tLastPoint As Point3d) As String
      Return zzGetDir(tFirstPoint.X, tLastPoint.X) & zzGetDir(tFirstPoint.Y, tLastPoint.Y)
   End Function
   Private Shared Sub zzSegmentInfoold(ByVal oPolyline As Polyline, tPoint As Point3d)
      Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
      Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
      Dim tPrevVertex, tVertex As Point3d
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim iSegmentType As SegmentType
      Dim dParam As Double
      DMAcadExt.AcadDocument.WriteMessage("Polyline Points: " & tClosestPointOnCurve.ToString() & "; " & dPointParameter.ToString())
      For iIndex As Integer = 1 To iVerticesUB
         tVertex = oPolyline.GetPoint3dAt(iIndex)
         dParam = oPolyline.GetParameterAtPoint(tVertex)
         If dPointParameter < dParam Then
            tPrevVertex = oPolyline.GetPoint3dAt(iIndex - 1)
            iSegmentType = oPolyline.GetSegmentType(iIndex - 1)
            DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex - 1) & ":" & iSegmentType.ToString() & "; " & DMAcadExt.TPlnPoint.DispPoint(tPrevVertex) & "<=>" & DMAcadExt.TPlnPoint.DispPoint(tVertex) & "; " & dParam.ToString())
            Exit For
         End If

      Next
   End Sub
   Private Shared Sub zzPolyline2dInfo(ByVal oPolyline As Polyline2d)
      Dim iVerticesUB As Integer = -1
      Dim iIndex As Integer = 0
      Dim tCurrentPoint As Point3d
      Dim tAcObjID As ObjectId
      Dim oDBObj As DBObject
      Dim oVertex2d As Vertex2d = Nothing
      Dim oColEnum As Collections.IEnumerator = oPolyline.GetEnumerator()
      DMAcadExt.AcadDocument.WriteMessage("Polyline2d Points:")
      Do While oColEnum.MoveNext()
         tAcObjID = DirectCast(oColEnum.Current, ObjectId)
         oDBObj = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForWrite)
         oVertex2d = DirectCast(oDBObj, Vertex2d)
         tCurrentPoint = oVertex2d.Position
         DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iIndex) & ":" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint) & "; Bulge= " & CStr(oVertex2d.Bulge))
         iIndex += 1
      Loop
   End Sub
   Private Shared Sub zzLineInfo(ByVal oLine As Line)
      Dim tCurrentPoint As Point3d
      Dim tDelta As Autodesk.AutoCAD.Geometry.Vector3d
      tCurrentPoint = oLine.StartPoint
      DMAcadExt.AcadDocument.WriteMessage("Line")
      DMAcadExt.AcadDocument.WriteMessage("StartPoint:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
      tCurrentPoint = oLine.EndPoint
      DMAcadExt.AcadDocument.WriteMessage("EndPoint:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
      tDelta = oLine.Delta
      DMAcadExt.AcadDocument.WriteMessage("Delta:" & tDelta.ToString() & " Tg=" & CStr(tDelta.Y / tDelta.X))

   End Sub
   Private Shared Sub zzPointInfo(ByVal oDBPoint As DBPoint)
      Dim tCurrentPoint As Point3d = oDBPoint.Position
      DMAcadExt.AcadDocument.WriteMessage("Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
   End Sub
   Private Shared Sub zzBlockRefInfo(ByVal oBlockRef As BlockReference)
      Dim tCurrentPoint As Point3d = oBlockRef.Position
      Dim oAttribRef As AttributeReference
      DMAcadExt.AcadDocument.WriteMessage("Insert Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
      For Each tAttrRefObjID As ObjectId In oBlockRef.AttributeCollection
         oAttribRef = DMAcadExt.AcadTransaction.GetAttribRef(tAttrRefObjID, OpenMode.ForRead)
         DMAcadExt.AcadDocument.WriteMessage(oAttribRef.Tag & ": " & oAttribRef.Position.ToString & ", " & oAttribRef.AlignmentPoint.ToString())
      Next
   End Sub
   Private Shared Sub zzHatchInfo(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch)
      Dim oHatchLoop As HatchLoop
      Dim iHatchLoopType As HatchLoopTypes
      Dim colCurves As Autodesk.AutoCAD.Geometry.Curve2dCollection
      Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection
      Dim iCurvesCount As Integer
      Dim iBulgeVertexCount As Integer
      Dim oCurve As Autodesk.AutoCAD.Geometry.Curve2d
      Dim oLineSegment2d As Autodesk.AutoCAD.Geometry.LineSegment2d

      For iIndex As Integer = 0 To oHatch.NumberOfLoops - 1
         iCurvesCount = -1
         iBulgeVertexCount = -1
         oHatchLoop = oHatch.GetLoopAt(iIndex)
         iHatchLoopType = oHatchLoop.LoopType
         DMAcadExt.AcadDocument.WriteMessage("HatchLoop:" & iHatchLoopType.ToString() & "; IsPline:" & CStr(oHatchLoop.IsPolyline))
         colCurves = oHatchLoop.Curves
         If colCurves IsNot Nothing Then
            iCurvesCount = colCurves.Count
         End If
         oPolyline = oHatchLoop.Polyline
         If oPolyline IsNot Nothing Then
            iBulgeVertexCount = oPolyline.Count
         End If
         DMAcadExt.AcadDocument.WriteMessage("Curves:" & iCurvesCount.ToString() & "; Vertices:" & CStr(iBulgeVertexCount))
         If iCurvesCount > 0 Then
            For iCurveIndex As Integer = 0 To iCurvesCount - 1
               oCurve = colCurves.Item(iCurveIndex)

               DMAcadExt.AcadDocument.WriteMessage("Curve#" & iCurveIndex.ToString() & ": Type:" & oCurve.GetType().ToString())
            Next
         End If
      Next
      ' GeoUtilites.BulgeVertexArray(oMPolygonLoop)
   End Sub
   Private Shared Function zzSelectPoint(ByRef tPoint As Point3d, bLockDoc As Boolean) As Boolean
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      Dim oResBuffer As ResultBuffer = New ResultBuffer()
      Dim bRes As Boolean
      If bLockDoc Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
      End If


      ptRes = oEditor.GetPoint(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            tPoint = ptRes.Value
            bRes = True
            oEditor.WriteMessage("OK!" & vbCrLf)

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
         End Try
      Else
         bRes = False
      End If
      If bLockDoc Then
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If

      Return bRes
   End Function

   Private Shared Sub moEditor_PromptedForSelection(oSsender As System.Object, e As PromptSelectionResultEventArgs) Handles moEditor.PromptedForSelection
      Dim sL As String = "P/Selection: "
      If e.Result.Value Is Nothing Then
         moEditor.WriteMessage(sL & e.Result.Status.ToString() & vbCrLf)
      Else
         moEditor.WriteMessage(sL & e.Result.Status.ToString() & "; " & e.Result.Value.Count.ToString() & vbCrLf)
      End If
   End Sub


   Private Shared Sub moEditor_PromptForEntityEnding(oSsender As System.Object, e As PromptForEntityEndingEventArgs) Handles moEditor.PromptForEntityEnding
      Dim sL As String = "P/EntityEnd: "
      moEditor.WriteMessage(sL & e.Result.Status.ToString() & "; " & e.Result.ObjectId.ToString() & "; " & e.Result.PickedPoint.ToString() & vbCrLf)
   End Sub

   Private Shared Sub moEditor_PromptForSelectionEnding(oSsender As System.Object, e As PromptForSelectionEndingEventArgs) Handles moEditor.PromptForSelectionEnding
      Dim sL As String = "P/SelectionEnd: "
      moEditor.WriteMessage(sL & e.Selection.Count.ToString() & "; " & e.Flags.ToString() & "; " & vbCrLf)
   End Sub
   Private Shared Sub zzPrintRes(sL As String, oRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult)

   End Sub


   Private Shared Sub moEditor_PromptingForSelection(oSsender As System.Object, e As PromptSelectionOptionsEventArgs) Handles moEditor.PromptingForSelection
      Dim sL As String = "PromptingSel: "
      Dim oSelFilter As SelectionFilter = e.Filter
      moEditor.WriteMessage(sL & vbCrLf)
      If oSelFilter IsNot Nothing Then
         Dim oaValues() As TypedValue = oSelFilter.GetFilter()

         If oaValues IsNot Nothing Then
            moEditor.WriteMessage(sL & oaValues.GetUpperBound(0) & "; " & vbCrLf)
         End If
      End If

   End Sub
End Class


