Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Public Enum enCellRelation
	Undefined = -1
	North
	NorthEast
	East
	SouthEast
	South
	SouthWest
	West
	NorthWest
   Outer
   NeigborUB = NorthWest
End Enum
Public Class EntityCell
	'  Const DistEqTolerance As Double = 0.000000001
	Const DistEqTolerance As Double = 0.000000000001
	Const DistEqTolerance1 As Double = 1.7976931348623157
   Private Shared miaCellRelation() As enCellRelation
   Public Shared Digits As Integer
   Public Shared PointLineTolerance As Double
   Public Shared RoundingError As Integer
   Private Shared moaErrorPoints As TplnPointArray
   Private Shared miSourceErrors As Integer
   '	Private Shared mdicNodes As System.Collections.Generic.Dictionary(Of KeyPoint, EntityCell)  20.05.09
   Private Shared mdicNodes As System.Collections.Generic.Dictionary(Of Long, EntityCell)
   Private Shared mhsErrPointLines As CurvePointLines

   Private Shared moCellMarkBlock As DMAcadExt.MarkBlock
   'Public Shared SourceTopoDef As TopoDef
   Public Shared OverlayTopoDef As TopoDef
   Private Shared mdMaxUpdateDist As Double
   Private Shared mdMaxErrDist As Double
   Private Shared mdUpdateTolerance As Double
   Private mtMinKeyPoint As KeyPoint
   '	Private mtSourcePoint As Point2d
   Private mbSourceError As Boolean = False
   Private miPointIndex As Integer
   Private moaAcadEntity() As CurvePoint
   Private miNotOverlayCount As Integer = 0
   'Private moErrPointArray As TplnPointArray
   Private moRootCell As EntityCell
   Private miRelationToRoot As enCellRelation = enCellRelation.Undefined
   Private mbHasFarNeighborCell As Boolean
   Private mdMaxDistNeighborPoints As Double = 0.0
   Private moaNeighborCells() As EntityCell
   Private mhsNeighborCells As HashSet(Of EntityCell)

   Private mcolAllPoints As Point2dCollection = New Point2dCollection()
   Private mcolSourcePoints As Point2dCollection = New Point2dCollection()
   Private mcolOverlayPoints As Point2dCollection = New Point2dCollection()
   Private mdicSegments As System.Collections.Generic.Dictionary(Of LineSegmentID, CurvePoint)
   Private mdicIntersectingLines As ObjectIdCollection
   'Private mbNearLine As Boolean
   Private mbSinglePoint As Boolean
   Private mdUpdateDist As Double
   Private mdErrDist As Double
   Private moNeigborEnum As NeigborEnum

   Private Shared CircleMarkBlock As MarkBlock
   Private Shared moTriangleMarkBlock As DMAcadExt.MarkBlock

	Public Sub New(ByVal tCurvePoint As CurvePoint, ByVal bCoarse As Boolean)
		mtMinKeyPoint = New KeyPoint(tCurvePoint.AcadPoint, bCoarse)

		Me.AddCurvePoint(tCurvePoint)


		mdicIntersectingLines = New ObjectIdCollection()

	End Sub
	Public Sub New(ByVal tKeyPoint As KeyPoint, ByVal bOverlay As Boolean, ByVal bCoarse As Boolean)

      mtMinKeyPoint = tKeyPoint
      mdicIntersectingLines = New ObjectIdCollection()

   End Sub
   Public Function GetNeighborEnumerator() As System.Collections.Generic.IEnumerator(Of KeyPoint)
      Dim iNeigborIndex As enCellRelation
      Dim oaNeigborPoints(enCellRelation.NeigborUB) As KeyPoint

      For iIndex As Integer = 0 To enCellRelation.NeigborUB
         iNeigborIndex = miaCellRelation(iIndex)
         oaNeigborPoints(iIndex) = GetNeighborPoints(iNeigborIndex)
      Next
      Return New NeigborEnum(oaNeigborPoints)
   End Function
   Public Function GetNeighborPoints(ByVal iDir As enCellRelation) As KeyPoint
      Return New KeyPoint(mtMinKeyPoint, iDir)
   End Function
   Public ReadOnly Property MyKeyPoint() As KeyPoint
      Get
         Return mtMinKeyPoint
      End Get
   End Property
   Public ReadOnly Property RelationToRoot() As enCellRelation
      Get
         Return miRelationToRoot
      End Get
   End Property
   Public ReadOnly Property HasOuterCell() As Boolean
      Get
         Dim bHasOuter As Boolean = False
         If moaNeighborCells IsNot Nothing Then
            For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
               If moaNeighborCells(iIndex).RelationToRoot = enCellRelation.Outer Then
                  bHasOuter = True
                  Exit For
               End If
            Next
         End If
         Return bHasOuter
      End Get
   End Property
   Public Shared Function GetErrPointLines() As TplnPointArray
      Dim oaRes As TplnPointArray = New TplnPointArray(mhsErrPointLines.Count - 1)
      Dim iIndex As Integer
      For Each oCurvePointLine As CurvePointLine In mhsErrPointLines
         oaRes.Item(iIndex) = New TPlnPoint(oCurvePointLine.CurvePoint.AcadPoint)
         iIndex += 1
      Next

		Return oaRes

   End Function
   Public Shared Sub MarkErrPointLines()
      '   Dim oaRes As TplnPointArray = New TplnPointArray(mhsErrPointLines.Count - 1)
      ' Dim iIndex As Integer
      Dim tPosition As Point3d
      For Each oCurvePointLine As CurvePointLine In mhsErrPointLines
         tPosition = oCurvePointLine.CurvePoint.AcadPoint3d
         moTriangleMarkBlock.MarkPoint(tPosition, 4S)
         '  DMAcadExt.AcadDocument.WriteMessage("+++ ++++tPosition= " & tPosition.ToString())
         '
         '  iIndex += 1
      Next



   End Sub
   Public ReadOnly Property NeighborCells() As EntityCell()
      Get
         Return moaNeighborCells
      End Get
   End Property
   Public ReadOnly Property NeighborCount() As Integer
      Get
         If moaNeighborCells Is Nothing Then
            Return 0
         Else
            Return moaNeighborCells.GetUpperBound(0) + 1
         End If
      End Get
   End Property
   Public ReadOnly Property Vertex(ByVal iIndex As Integer) As KeyPoint
      Get
         Select Case iIndex
            Case 0
               Return mtMinKeyPoint
            Case 1
               Return New KeyPoint(mtMinKeyPoint, enCellRelation.North)
            Case 2
               Return New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
            Case 3
               Return New KeyPoint(mtMinKeyPoint, enCellRelation.East)
         End Select
      End Get
   End Property
   Public Shared ReadOnly Property UpdateDist As Double
      Get
         Return mdMaxUpdateDist
      End Get
   End Property
   Public Sub AddIntersectingLine(tLineObjID As ObjectId)
      If Not mdicIntersectingLines.Contains(tLineObjID) Then
         mdicIntersectingLines.Add(tLineObjID)
      End If

   End Sub
   Public ReadOnly Property HasIntersectingLines As Boolean
      Get
         Return mdicIntersectingLines.Count > 0
      End Get
   End Property
   Public ReadOnly Property HasCurvePoints As Boolean
      Get
         Return (moaAcadEntity IsNot Nothing) AndAlso (moaAcadEntity.GetUpperBound(0) >= 0)
      End Get
   End Property
   Public ReadOnly Property HasNeighbours As Boolean
      Get
         Return moaNeighborCells IsNot Nothing AndAlso moaNeighborCells.GetUpperBound(0) >= 0
      End Get
   End Property
   Public ReadOnly Property HasNeighboursNew As Boolean
      Get
         Return mhsNeighborCells IsNot Nothing AndAlso mhsNeighborCells.Count > 0
      End Get
   End Property
   Public ReadOnly Property Neighbours As HashSet(Of EntityCell)
      Get
         Return mhsNeighborCells
      End Get
   End Property

   Public Sub AddCurvePoint(ByVal tCurvePoint As CurvePoint, Optional bDebug As Boolean = False)
      bDebug = False
      Dim iUB As Integer
      Dim tAcadPoint As Point2d = tCurvePoint.AcadPoint
      Dim bPointExists As Boolean = False
      Dim dErrDist As Double

      If mcolAllPoints.Count = 0 Then
         mcolAllPoints.Add(tAcadPoint)
         If bDebug Then
            DMAcadExt.AcadDocument.WriteMessage("&&* A ")
         End If
      ElseIf False AndAlso Not mcolAllPoints.Contains(tAcadPoint) Then
         mcolAllPoints.Add(tAcadPoint)
         mbSourceError = True
         If bDebug Then
            DMAcadExt.AcadDocument.WriteMessage("&&* B ")
         End If
      Else
         For Each tPoint As Point2d In mcolAllPoints

            '	If TPlnPoint.IsEqualPoint(tAcadPoint, tPoint) Then
            dErrDist = tPoint.GetDistanceTo(tAcadPoint)
            '  DMAcadExt.AcadDocument.WriteMessage("&&* BK " & CStr(dErrDist) & " |==> " & tPoint.ToString())
            If dErrDist < DistEqTolerance Then

               bPointExists = True

               If bDebug Then
                  DMAcadExt.AcadDocument.WriteMessage("&&* C " & CStr(mcolAllPoints.Count) & ":" & TPlnPoint.DispPoint(tAcadPoint) & "<->" & TPlnPoint.DispPoint(tPoint))
               End If
               If dErrDist > 0.0 Then
                  DMAcadExt.AcadDocument.WriteMessage("&&  Avtive Toler " & ":" & TPlnPoint.DispPoint(tAcadPoint) & "<==>" & TPlnPoint.DispPoint(tPoint))
               End If
               Exit For
            Else
               If mdErrDist < dErrDist Then
                  mdErrDist = dErrDist
               End If
            End If
         Next
         If bPointExists Then

            If bDebug Then
               DMAcadExt.AcadDocument.WriteMessage("&&* D ")
            End If
         Else
            mcolAllPoints.Add(tAcadPoint)
            mbSourceError = True
            If bDebug Then
               DMAcadExt.AcadDocument.WriteDebugMessage("&&&&&* F " & CStr(mcolAllPoints.Count) & "; " & TPlnPoint.DispPoint(tAcadPoint))
            End If
         End If
      End If
      If tCurvePoint.Overlay Then
         If Not mcolOverlayPoints.Contains(tAcadPoint) Then
            mcolOverlayPoints.Add(tAcadPoint)
         End If
      Else
         If Not mcolSourcePoints.Contains(tAcadPoint) Then
            mcolSourcePoints.Add(tAcadPoint)
         End If
      End If
      If moaAcadEntity Is Nothing Then
         iUB = -1
         mbSinglePoint = Not tCurvePoint.Inner
      Else
         iUB = moaAcadEntity.GetUpperBound(0)
         mbSinglePoint = False
      End If
      ReDim Preserve moaAcadEntity(iUB + 1)
      moaAcadEntity(iUB + 1) = tCurvePoint
      If Not tCurvePoint.Overlay Then
         miNotOverlayCount += 1
      End If
      '  AcadDocument.WriteMessage("PointCell: " & CStr(Me.mtMinKeyPoint.Code) & ":" & Me.mtMinKeyPoint.AbsCoordinates & "||" & moaAcadEntity.GetUpperBound(0))
      If moaAcadEntity IsNot Nothing Then
         '    AcadDocument.WriteMessage("moaAcadEntity!!--???****** " & CStr(moaAcadEntity.GetUpperBound(0)))
      End If

   End Sub
   Public Sub SetInRoot()
      mdicNodes.Add(Me.MyKeyPoint.Code, Me)
   End Sub
   Private Sub zzRoundingAlone()
      Dim dMin As Double
      Dim dCurrent As Double
      Dim iVertexMin As Integer = 0
      Dim dDist As Double
      Dim oVertex As KeyPoint
      Dim tCurvePoint As CurvePoint
      For iVertex As Integer = 0 To 3
         dCurrent = 0.0
         'dCurrentOverlay = 0.0
         oVertex = Me.Vertex(iVertex)
         For iIndex As Integer = 0 To moaAcadEntity.GetUpperBound(0)
            tCurvePoint = moaAcadEntity(iIndex)
            dDist = oVertex.GetDistanceTo(tCurvePoint.AcadPoint)
            If Me.NotOverlayExists Xor tCurvePoint.Overlay Then
               dCurrent += dDist * dDist
            End If
         Next
         dCurrent = GetTotalDist(oVertex.GetAbsPoint2d())
         If iVertex = 0 Then
            dMin = dCurrent
         ElseIf dCurrent < dMin Then
            dMin = dCurrent
            iVertexMin = iVertex
         End If
      Next
      oVertex = Me.Vertex(iVertexMin)
      UpdatePoint(oVertex.GetAbsPoint2d())
   End Sub
   Public Sub UpdatePoint(ByVal tNewPoint As Point2d)
      Dim dUpdateDist As Double = 0.0
      Dim dCurrentUpdateDist As Double
      '   AcadDocument.WriteDebugMessage("!!Update: " & CStr(moaAcadEntity.GetUpperBound(0)) & "," & tNewPoint.ToString() & "," & miSourceErrors.ToString())

      For iIndex As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         moaAcadEntity(iIndex).UpdatePoint(tNewPoint)

         dCurrentUpdateDist = tNewPoint.GetDistanceTo(moaAcadEntity(iIndex).AcadPoint)
         If dUpdateDist < dCurrentUpdateDist Then
            dUpdateDist = dCurrentUpdateDist
         End If
         miSourceErrors += 1

      Next

      '   AcadDocument.WriteDebugMessage("!!After Update: " & tNewPoint.ToString() & "," & miSourceErrors.ToString())
      If mdUpdateDist < dUpdateDist Then
         mdUpdateDist = dUpdateDist
      End If
      If mdMaxUpdateDist < mdUpdateDist Then
         mdMaxUpdateDist = mdUpdateDist
      End If
   End Sub
   Public ReadOnly Property NotOverlayCount() As Integer
      Get
         Return miNotOverlayCount
      End Get
   End Property
   Public ReadOnly Property NotOverlayExists() As Boolean
      Get
         Return (miNotOverlayCount <> 0)
      End Get
   End Property
   Private Sub zzMarkInnerCellPoints(bKoah As Boolean)
      Dim tPoint As Point2d
      ' AcadDocument.WriteMessage("333+mcolAllPoints:" & "; " & mcolAllPoints.Count.ToString() & "; mcolSourcePoints:" & mcolSourcePoints.Count.ToString() & "; Cnd:" & ":" & mbSourceError.ToString() & ":" & mbSinglePoint.ToString() & ":" & bKoah.ToString())
      If bKoah OrElse Me.HasSourceError() OrElse Me.HasNeighboursNew() Then '   moaAcadEntity.GetUpperBound(0) = 0
         '	AcadDocument.WriteMessage("Source:" & CStr(SourceTopoDef.ID.ID) & "," & CStr(SourceTopoDef.MarkColor))
         '	AcadDocument.WriteMessage("Overlay:" & CStr(OverlayTopoDef.ID.ID) & "," & CStr(OverlayTopoDef.MarkColor))


         For Each tPoint In mcolSourcePoints
            '  AcadDocument.WriteDebugMessage("T.Source:" & TPlnPoint.DispPoint(tPoint))
            'CircleMarkBlock.MarkPoint(tPoint, SourceTopoDef.MarkColor)
         Next
         For Each tPoint In mcolOverlayPoints
            '   AcadDocument.WriteDebugMessage("T.Overlay:" & TPlnPoint.DispPoint(tPoint))
            'CircleMarkBlock.MarkPoint(tPoint, OverlayTopoDef.MarkColor)
         Next

         For Each tPoint In mcolAllPoints
            '	AcadDocument.WriteMessage("T.Overlay:" & TPlnPoint.DispPoint(tPoint))
            moaErrorPoints.Add(tPoint)
            miSourceErrors += 1
            Try
               CircleMarkBlock.MarkPoint(tPoint, 1S) ' OverlayTopoDef.MarkColor
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "EntityCell - zzMarkPoints")
            End Try

         Next

         If mdMaxErrDist < mdErrDist Then
            mdMaxErrDist = mdErrDist
         End If
      End If
   End Sub
   Private Sub zzUpdateNeighborsCells()

      Dim tNewPoint As Point2d



      tNewPoint = zzGetAllPointsCenter(True)
      Me.UpdatePoint(tNewPoint)
      For Each oNeighborCell As EntityCell In mhsNeighborCells
         oNeighborCell.UpdatePoint(tNewPoint)

      Next
   End Sub


   Private Sub zzUpdateNeighborsCellsOld()
      Dim dMinX As Double = Me.MyKeyPoint.GetAbsX
      Dim dMaxX As Double = Me.MyKeyPoint.GetAbsX

      Dim dMinY As Double = Me.MyKeyPoint.GetAbsY
      Dim dMaxY As Double = Me.MyKeyPoint.GetAbsY
      Dim dNeighborX As Double
      Dim dNeighborY As Double
      Dim tNewPoint As Point2d


      For Each oNeighborCell As EntityCell In mhsNeighborCells
         dNeighborX = oNeighborCell.MyKeyPoint.GetAbsX()
         dNeighborY = oNeighborCell.MyKeyPoint.GetAbsY()
         If dMinX > dNeighborX Then
            dMinX = dNeighborX
         End If
         If dMaxX < dNeighborX Then
            dMaxX = dNeighborX
         End If
         If dMinY > dNeighborY Then
            dMinY = dNeighborY
         End If
         If dMaxY < dNeighborY Then
            dMaxY = dNeighborY
         End If
      Next
      tNewPoint = New Point2d(0.5 * (dMinX + dMaxX), 0.5 * (dMinY + dMaxY))
      Me.UpdatePoint(tNewPoint)
      For Each oNeighborCell As EntityCell In mhsNeighborCells
         oNeighborCell.UpdatePoint(tNewPoint)
      Next
   End Sub
   Private Sub zzUpdateMyself()
      Dim tCenterPoint As Point2d


      If mbSinglePoint Then
         zzMarkInnerCellPoints(False)
      Else
         tCenterPoint = zzGetCellCenterPoint()
         Me.UpdatePoint(tCenterPoint)

      End If



   End Sub

   Private Sub zzMarkNeighboursPoints()
      ' Dim tPoint As Point2d
      '  AcadDocument.WriteMessage("+mcolAllPoints:" & "; " & mcolAllPoints.Count.ToString() & "; mcolSourcePoints:" & mcolSourcePoints.Count.ToString() & "; Cnd:" & bAll.ToString() & ":" & mbSourceError.ToString() & ":" & mbSinglePoint.ToString())

      '	AcadDocument.WriteMessage("Source:" & CStr(SourceTopoDef.ID.ID) & "," & CStr(SourceTopoDef.MarkColor))
      '	AcadDocument.WriteMessage("Overlay:" & CStr(OverlayTopoDef.ID.ID) & "," & CStr(OverlayTopoDef.MarkColor))
      '  AcadDocument.WriteMessage("+mhsNeighborCells:" & (mhsNeighborCells Is Nothing).ToString())
      If mhsNeighborCells IsNot Nothing Then
         '  AcadDocument.WriteMessage("+++++mhsNeighborCells:" & (mhsNeighborCells.Count).ToString())
         For Each oNeighborCell As EntityCell In mhsNeighborCells
            oNeighborCell.zzMarkInnerCellPoints(True)
         Next
      End If
      If mdMaxErrDist < mdErrDist Then
         mdMaxErrDist = mdErrDist
      End If

   End Sub
   Private Sub zzMarkPoints(ByVal bAll As Boolean)
      Dim tPoint As Point2d
      AcadDocument.WriteMessage("+mcolAllPoints:" & "; " & mcolAllPoints.Count.ToString() & "; mcolSourcePoints:" & mcolSourcePoints.Count.ToString() & "; Cnd:" & bAll.ToString() & ":" & mbSourceError.ToString() & ":" & mbSinglePoint.ToString())
      If bAll OrElse mbSourceError OrElse mbSinglePoint OrElse Me.HasNeighboursNew Then '   moaAcadEntity.GetUpperBound(0) = 0
         '	AcadDocument.WriteMessage("Source:" & CStr(SourceTopoDef.ID.ID) & "," & CStr(SourceTopoDef.MarkColor))
         '	AcadDocument.WriteMessage("Overlay:" & CStr(OverlayTopoDef.ID.ID) & "," & CStr(OverlayTopoDef.MarkColor))


         For Each tPoint In mcolSourcePoints
            AcadDocument.WriteDebugMessage("T.Source:" & TPlnPoint.DispPoint(tPoint))
            'CircleMarkBlock.MarkPoint(tPoint, SourceTopoDef.MarkColor)
         Next
         For Each tPoint In mcolOverlayPoints
            AcadDocument.WriteDebugMessage("T.Overlay:" & TPlnPoint.DispPoint(tPoint))
            'CircleMarkBlock.MarkPoint(tPoint, OverlayTopoDef.MarkColor)
         Next

         For Each tPoint In mcolAllPoints
            '	AcadDocument.WriteMessage("T.Overlay:" & TPlnPoint.DispPoint(tPoint))
            moaErrorPoints.Add(tPoint)
            miSourceErrors += 1
            Try
               CircleMarkBlock.MarkPoint(tPoint, 1S) ' OverlayTopoDef.MarkColor
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "EntityCell - zzMarkPoints")
            End Try

         Next
         If moaNeighborCells IsNot Nothing Then
            For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
               moaNeighborCells(iIndex).zzMarkPoints(True)
            Next
         End If
         If mdMaxErrDist < mdErrDist Then
            mdMaxErrDist = mdErrDist
         End If
      End If
   End Sub
   Public ReadOnly Property IsSinglePoint() As Boolean
      Get
         Return mbSinglePoint
      End Get
   End Property
   Public Function HasSourceError() As Boolean
      If Not mbSourceError AndAlso Not mbSinglePoint Then
         Return False
      Else
         Return True
      End If
   End Function
   Private Sub zzRoundingWithThree()
      Dim tKeyPoint As KeyPoint
      Dim bTwoPoint As Boolean = False
      Dim tNewPoint As Point2d

      Dim oNeighbourCellI As EntityCell = Me.NeighborCells(0)
      Dim oNeighbourCellII As EntityCell = Me.NeighborCells(1)
      Dim oNeighbourCellIII As EntityCell = Me.NeighborCells(2)

      Dim bErr As Boolean = False
      If oNeighbourCellI.RelationToRoot = enCellRelation.North AndAlso oNeighbourCellII.RelationToRoot = enCellRelation.NorthEast AndAlso oNeighbourCellII.RelationToRoot = enCellRelation.East Then
         tKeyPoint = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
      Else
         bErr = True
      End If
      If bErr Then
         moaErrorPoints.Add(mtMinKeyPoint.GetAbsPoint2d())
      Else
         tNewPoint = tKeyPoint.GetAbsPoint2d()
         Me.UpdatePoint(tNewPoint)
         oNeighbourCellI.UpdatePoint(tNewPoint)
         oNeighbourCellII.UpdatePoint(tNewPoint)
         oNeighbourCellIII.UpdatePoint(tNewPoint)
      End If


   End Sub
   Private Sub zzRoundingWithTwo()
      Dim tKeyPoint As KeyPoint
      Dim bTwoPoint As Boolean = False
      Dim tNewPoint As Point2d

      Dim oNeighbourCellI As EntityCell = Me.NeighborCells(0)
      Dim oNeighbourCellII As EntityCell = Me.NeighborCells(1)
      Dim bErr As Boolean = False
      If oNeighbourCellI.RelationToRoot = enCellRelation.North Then
         If oNeighbourCellII.RelationToRoot = enCellRelation.NorthEast OrElse oNeighbourCellII.RelationToRoot = enCellRelation.East Then
            tKeyPoint = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
         Else
            bErr = True
         End If
      ElseIf oNeighbourCellI.RelationToRoot = enCellRelation.NorthEast Then
         If oNeighbourCellII.RelationToRoot = enCellRelation.East Then
            tKeyPoint = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
         Else
            bErr = True
         End If
      ElseIf oNeighbourCellI.RelationToRoot = enCellRelation.East AndAlso oNeighbourCellII.RelationToRoot = enCellRelation.SouthEast Then
         tKeyPoint = New KeyPoint(mtMinKeyPoint, enCellRelation.SouthEast)
      Else
         bErr = True
      End If

      If bErr Then
         moaErrorPoints.Add(mtMinKeyPoint.GetAbsPoint2d())
      Else
         tNewPoint = tKeyPoint.GetAbsPoint2d()
         Me.UpdatePoint(tNewPoint)
         oNeighbourCellI.UpdatePoint(tNewPoint)
         oNeighbourCellII.UpdatePoint(tNewPoint)
      End If

   End Sub
   Private Sub zzRoundingWithOne()
      Dim tKeyPointI, tKeyPointII As KeyPoint
      Dim bTwoPoint As Boolean = False
      Dim tNewPoint, tPointI, tPointII As Point2d
      Dim dTotalDistI, dTotalDistII As Double
      Dim oNeighbourCell As EntityCell = Me.NeighborCells(0)

      Select Case oNeighbourCell.RelationToRoot
         Case enCellRelation.North
            tKeyPointI = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthWest)
            tKeyPointII = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
            bTwoPoint = True
         Case enCellRelation.NorthEast
            tKeyPointI = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
         Case enCellRelation.East
            tKeyPointI = New KeyPoint(mtMinKeyPoint, enCellRelation.NorthEast)
            tKeyPointII = New KeyPoint(mtMinKeyPoint, enCellRelation.SouthEast)
            bTwoPoint = True
         Case enCellRelation.SouthEast
            tKeyPointI = New KeyPoint(mtMinKeyPoint, enCellRelation.SouthEast)
      End Select
      tPointI = tKeyPointI.GetAbsPoint2d()
      If bTwoPoint Then
         tPointII = tKeyPointII.GetAbsPoint2d()
         dTotalDistI = Me.GetTotalDist(tPointI) + oNeighbourCell.GetTotalDist(tPointI)
         dTotalDistII = Me.GetTotalDist(tPointII) + oNeighbourCell.GetTotalDist(tPointII)
         If dTotalDistI <= dTotalDistII Then
            tNewPoint = tPointI
         Else
            tNewPoint = tPointII
         End If
      Else
         tNewPoint = tPointI
      End If
      Me.UpdatePoint(tNewPoint)
      oNeighbourCell.UpdatePoint(tNewPoint)
   End Sub
   Public Function GetTotalDist(ByVal tPoint2d As Point2d) As Double
      Dim tCurvePoint As CurvePoint
      Dim dDist As Double
      Dim dTotalDist As Double
      For iIndex As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         tCurvePoint = moaAcadEntity(iIndex)
         dDist = tCurvePoint.AcadPoint.GetDistanceTo(tPoint2d)
         dTotalDist += dDist * dDist
      Next
      Return dTotalDist
   End Function
   Public Sub SetRoot(ByVal oEntityCell As EntityCell, ByVal iRelation As enCellRelation)
      moRootCell = oEntityCell
      miRelationToRoot = iRelation
   End Sub
   Public Function SamoObman() As Long
      If moRootCell IsNot Nothing Then
         If moRootCell.MyKeyPoint.Code = Me.MyKeyPoint.Code Then
            Return Me.MyKeyPoint.Code
         End If
      End If
      Return 0L
   End Function
   Public Shared Operator <(oCellA As EntityCell, oCellB As EntityCell) As Boolean
      If oCellA Is Nothing Then
         Return False
      ElseIf oCellB Is Nothing Then
         Return True
      Else
         Return oCellA.MyKeyPoint.Code < oCellB.MyKeyPoint.Code
      End If
   End Operator

   Public Shared Operator >(oCellA As EntityCell, oCellB As EntityCell) As Boolean
      If oCellA Is Nothing Then
         Return True
      ElseIf oCellB Is Nothing Then
         Return False
      Else
         Return oCellA.MyKeyPoint.Code > oCellB.MyKeyPoint.Code
      End If
   End Operator
   Public Sub CalculateN()
      Dim bRootUpdate As Boolean
      Dim bIsRoot As Boolean
      If moRootCell Is Nothing Then
         moRootCell = Me
         bIsRoot = True
      End If
      For Each oNeighborCell As EntityCell In mhsNeighborCells
         If oNeighborCell.RootCell < moRootCell Then
            moRootCell = oNeighborCell.RootCell
            bRootUpdate = True
         End If
      Next
      For Each oNeighborCell As EntityCell In mhsNeighborCells
         oNeighborCell.RootCell = moRootCell
      Next
      If bRootUpdate Then
         moRootCell.AddFarNeighborCell(Me, enCellRelation.Outer)
         moRootCell.AddMeToNodes()
      ElseIf bIsRoot Then
         AddMeToNodes()
      End If
      For Each oNeighborCell As EntityCell In mhsNeighborCells
         oNeighborCell.RootCell = moRootCell
         moRootCell.AddNeighborCell(oNeighborCell, enCellRelation.Outer)
      Next
   End Sub
   Public Sub AddCloseNeighborCell(ByRef oNeighborCell As EntityCell, ByVal iRelation As enCellRelation)
      Dim dDist As Double
      If mhsNeighborCells Is Nothing Then
         mhsNeighborCells = New HashSet(Of EntityCell)()
      End If
      mhsNeighborCells.Add(oNeighborCell)

      For Each tMyPoint As Point2d In mcolAllPoints
         For Each tNeighborPoint As Point2d In oNeighborCell.AllPoints
            dDist = tMyPoint.GetDistanceTo(tNeighborPoint)
            If mdMaxDistNeighborPoints < dDist Then
               mdMaxDistNeighborPoints = dDist
            End If
         Next
      Next

   End Sub
   ReadOnly Property Updatable As Boolean
      Get
         Return (Not mbHasFarNeighborCell) AndAlso mdMaxDistNeighborPoints < mdUpdateTolerance
      End Get
   End Property

   Public Sub AddNeighborCellNew(ByRef oNeighborCell As EntityCell, ByVal iRelation As enCellRelation)
      Dim oNeighborRoot As EntityCell = oNeighborCell.RootCell

      Dim bTest As Boolean
      If oNeighborRoot Is Nothing Then
         If moRootCell Is Nothing OrElse (moRootCell.MyKeyPoint.Code = Me.MyKeyPoint.Code) Then
            Dim iPriorUB As Integer
            If moaNeighborCells IsNot Nothing Then
               iPriorUB = moaNeighborCells.GetUpperBound(0)
            Else
               iPriorUB = -1
               AddMeToNodes()
            End If
            Dim iUB As Integer = -1
            Dim oaNeighborCells() As EntityCell = oNeighborCell.NeighborCells
            If oaNeighborCells IsNot Nothing Then
               iUB = oaNeighborCells.GetUpperBound(0)
            End If
            ReDim Preserve moaNeighborCells(iPriorUB + iUB + 2)
            moaNeighborCells(iPriorUB + 1) = oNeighborCell
            For iIndex As Integer = 0 To iUB
               moaNeighborCells(iPriorUB + 2 + iIndex) = oaNeighborCells(iIndex)
            Next

            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            oNeighborCell.SetRoot(Me, iRelation)
            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
         Else 'i.e. moRootCell IsNot Nothing 

            moRootCell.AddNeighborCell(oNeighborCell, enCellRelation.Outer, bTest)
         End If
      Else 'i.e.   oPriorRoot IsNot Nothing  
         '  DMAcadExt.AcadDocument.WriteMessage(CStr(moRootCell Is Nothing) & "^^^^^^^^^^^^^^^^^^^^^^^^^")
         If moRootCell Is Nothing Then
            oNeighborRoot.AddNeighborCell(Me, enCellRelation.Outer, bTest)
            If bTest Then
               System.Windows.Forms.MessageBox.Show("SETROOT 2-!!!" & vbCrLf & "", "07_055")
            End If

            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            moRootCell = oNeighborRoot
            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            AddMeToNodes()
         Else 'i.e. moRootCell IsNot Nothing
            If oNeighborRoot.MyKeyPoint.Code < moRootCell.MyKeyPoint.Code Then
               oNeighborRoot.AddNeighborCell(moRootCell, enCellRelation.Outer, bTest)
            ElseIf oNeighborRoot.MyKeyPoint.Code > moRootCell.MyKeyPoint.Code Then
               moRootCell.AddNeighborCell(oNeighborRoot, enCellRelation.Outer, bTest)
            End If
         End If
      End If
   End Sub
   Public Sub AddFarNeighborCell(ByRef oNeighborCell As EntityCell, ByVal iRelation As enCellRelation)
      If Not mhsNeighborCells.Contains(oNeighborCell) Then
         mhsNeighborCells.Add(oNeighborCell)
         mbHasFarNeighborCell = True
      End If


   End Sub

   Public Sub AddNeighborCell(ByRef oNeighborCell As EntityCell, ByVal iRelation As enCellRelation, Optional bTest As Boolean = False)
      Dim oPriorRoot As EntityCell = oNeighborCell.RootCell
      Dim sTest As String
      If moRootCell Is Nothing Then
         sTest = "Root=" & "Nothing"
      Else
         sTest = "Root=" & CStr(moRootCell.MyKeyPoint.Code) & ":" & CStr(moRootCell.RootCode)
      End If
      If bTest Then
         System.Windows.Forms.MessageBox.Show(sTest & vbCrLf & CStr(Me.SamoObman) & vbCrLf & CStr(Me.MyKeyPoint.Code) & vbCrLf & CStr(oNeighborCell.MyKeyPoint.Code) & vbCrLf & CStr(oPriorRoot IsNot Nothing) & ":" & CStr(moRootCell IsNot Nothing) & ":" & CStr(moaNeighborCells IsNot Nothing), "07_012")

      End If
      If oPriorRoot Is Nothing Then
         If moRootCell Is Nothing OrElse (moRootCell.MyKeyPoint.Code = Me.MyKeyPoint.Code) Then
            Dim iPriorUB As Integer
            If moaNeighborCells IsNot Nothing Then
               iPriorUB = moaNeighborCells.GetUpperBound(0)
            Else
               iPriorUB = -1
               AddMeToNodes()
            End If
            Dim iUB As Integer = -1
            Dim oaNeighborCells() As EntityCell = oNeighborCell.NeighborCells
            If oaNeighborCells IsNot Nothing Then
               iUB = oaNeighborCells.GetUpperBound(0)
            End If
            ReDim Preserve moaNeighborCells(iPriorUB + iUB + 2)
            moaNeighborCells(iPriorUB + 1) = oNeighborCell
            For iIndex As Integer = 0 To iUB
               moaNeighborCells(iPriorUB + 2 + iIndex) = oaNeighborCells(iIndex)
            Next
            If bTest Then
               System.Windows.Forms.MessageBox.Show("SetRoot 1!!!" & vbCrLf & iRelation.ToString(), "07_044")

            End If
            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            oNeighborCell.SetRoot(Me, iRelation)
            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
         Else 'i.e. moRootCell IsNot Nothing 
            If bTest Then
               System.Windows.Forms.MessageBox.Show("Root:" & CStr(moRootCell.MyKeyPoint.Code) & vbCrLf & "Me:" & CStr(Me.MyKeyPoint.Code) & vbCrLf & "NB:" & CStr(oNeighborCell.MyKeyPoint.Code) & vbCrLf & vbCrLf & CStr(oPriorRoot IsNot Nothing) & ":" & CStr(moRootCell IsNot Nothing) & ":" & CStr(moaNeighborCells IsNot Nothing), "07_015")

            End If
            moRootCell.AddNeighborCell(oNeighborCell, enCellRelation.Outer, bTest)
         End If
      Else 'i.e.   oPriorRoot IsNot Nothing  
         '  DMAcadExt.AcadDocument.WriteMessage(CStr(moRootCell Is Nothing) & "^^^^^^^^^^^^^^^^^^^^^^^^^")
         If moRootCell Is Nothing Then
            oPriorRoot.AddNeighborCell(Me, enCellRelation.Outer, bTest)
            If bTest Then
               System.Windows.Forms.MessageBox.Show("SETROOT 2-!!!" & vbCrLf & "", "07_055")
            End If

            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            moRootCell = oPriorRoot
            '''''@@@@@@@@@@@@@@@@@@@@@@@@@
            AddMeToNodes()
         Else 'i.e. moRootCell IsNot Nothing
            If oPriorRoot.MyKeyPoint.Code < moRootCell.MyKeyPoint.Code Then
               oPriorRoot.AddNeighborCell(moRootCell, enCellRelation.Outer, bTest)
            ElseIf oPriorRoot.MyKeyPoint.Code > moRootCell.MyKeyPoint.Code Then
               moRootCell.AddNeighborCell(oPriorRoot, enCellRelation.Outer, bTest)
            End If
         End If
      End If
   End Sub

   Public Sub TestRes()
      System.Windows.Forms.MessageBox.Show(CStr(mdicNodes.Count), "21_500")
   End Sub
   Public Shared Sub Init()
      mdicNodes = New System.Collections.Generic.Dictionary(Of Long, EntityCell)
      moaErrorPoints = New TplnPointArray()
      miSourceErrors = 0
      mdMaxUpdateDist = 0.0
      mdMaxErrDist = 0.0
      mdUpdateTolerance = 1.5 * KeyPoint.Tolerance
      moCellMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Cell, KeyPoint.Tolerance)
      mhsErrPointLines = New CurvePointLines()
      CurvePoint.Init()
      ReDim miaCellRelation(enCellRelation.NeigborUB)
      For iIndex As Integer = 0 To enCellRelation.NeigborUB
         If [Enum].IsDefined(GetType(enCellRelation), iIndex) Then
            miaCellRelation(iIndex) = CType(iIndex, enCellRelation)
         End If
      Next
      moTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
      CircleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
   End Sub
   Public Sub MergingNew(ByVal bFix As Boolean)
      '  System.Windows.Forms.MessageBox.Show("start Merging" & vbCrLf & "Fix=" & bFix.ToString(), "21_211")
      If bFix AndAlso Updatable Then
         Dim iAllNotOverlayCount As Integer = miNotOverlayCount
         Dim iAllCount As Integer = Me.AcadEntities.GetUpperBound(0) + 1


         If Me.HasNeighboursNew Then
            zzUpdateNeighborsCells()
         Else
            zzUpdateMyself()
         End If

      Else 'i.e  Not bFix

         '   System.Windows.Forms.MessageBox.Show(" before zzMarkPoints", "05_989b")
         '''''''''' Me.zzMarkPoints(False)
         Me.zzMarkInnerCellPoints(False)
         Me.zzMarkNeighboursPoints()

         '  System.Windows.Forms.MessageBox.Show(" After zzMarkPoints", "05_989a")
      End If
   End Sub
   Public Sub Merging(ByVal bFix As Boolean)
      '  System.Windows.Forms.MessageBox.Show("start Merging" & vbCrLf & "Fix=" & bFix.ToString(), "21_211")
      If bFix Then
         Dim iAllNotOverlayCount As Integer = miNotOverlayCount
         Dim iAllCount As Integer = Me.AcadEntities.GetUpperBound(0) + 1
         Dim oEntityCell As EntityCell
         Dim tCenterPoint As Point2d

         If moaNeighborCells IsNot Nothing Then
            For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
               oEntityCell = moaNeighborCells(iIndex)
               iAllNotOverlayCount += oEntityCell.NotOverlayCount
               iAllCount += oEntityCell.AcadEntities.GetUpperBound(0) + 1
            Next
         End If
         '	If iAllCount <> 1 OrElse iAllNotOverlayCount <> 1 Then
         AcadDocument.WriteMessage("All=" & CStr(iAllCount) & ";  NotOverlay=" & CStr(iAllNotOverlayCount))
         'End If

         If iAllCount = 1 Then
            Me.zzMarkPoints(True)
         Else
            If iAllNotOverlayCount <> 0 Then
               tCenterPoint = zzGetCenterPoint(True)
            Else
               tCenterPoint = zzGetCenterPoint(False)
            End If


            Me.UpdatePoint(tCenterPoint)
            If moaNeighborCells IsNot Nothing Then
               For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
                  oEntityCell = moaNeighborCells(iIndex)
                  oEntityCell.UpdatePoint(tCenterPoint)
               Next
            End If
         End If
      Else 'i.e  Not bFix

         '   System.Windows.Forms.MessageBox.Show(" before zzMarkPoints", "05_989b")
         '''''''''' Me.zzMarkPoints(False)
         Me.zzMarkInnerCellPoints(False)
         Me.zzMarkNeighboursPoints()

         '  System.Windows.Forms.MessageBox.Show(" After zzMarkPoints", "05_989a")
      End If
   End Sub
   Public Sub AddMeToNodes()
      If Not mdicNodes.ContainsKey(Me.MyKeyPoint.Code) Then
         mdicNodes.Add(Me.MyKeyPoint.Code, Me)
      End If
   End Sub

   Private Function zzGetAllPoints(ByVal iPointsUB As Integer, ByVal bNotOverlayOnly As Boolean) As Point2d()
      Dim oaCurvePoint() As CurvePoint
      Dim oaAllPoint(iPointsUB) As Point2d
      Dim iAllIndex As Integer = 0

      For iIndexInner As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         If Not bNotOverlayOnly OrElse Not moaAcadEntity(iIndexInner).Overlay Then
            oaAllPoint(iAllIndex) = moaAcadEntity(iIndexInner).AcadPoint
            iAllIndex += 1
         End If
      Next

      For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
         oaCurvePoint = moaNeighborCells(iIndex).AcadEntities
         For iIndexInner As Integer = 0 To moaNeighborCells.GetUpperBound(0)
            If Not bNotOverlayOnly OrElse Not oaCurvePoint(iIndexInner).Overlay Then
               oaAllPoint(iAllIndex) = oaCurvePoint(iIndexInner).AcadPoint
               iAllIndex += 1
            End If
         Next
      Next
      Return oaAllPoint
   End Function
   Private Function zzGetCellCenterPoint() As Point2d

      Dim tAcadPoint As Point2d
      Dim dCenterX, dCenterY As Double
      Dim iCounter As Integer = 0
      Dim iAllIndex As Integer = 0
      Dim tCurvePoint As CurvePoint
      For iIndexInner As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         Try
            tCurvePoint = moaAcadEntity(iIndexInner)

            tAcadPoint = tCurvePoint.AcadPoint
            dCenterX += tAcadPoint.X
            dCenterY += tAcadPoint.Y
            iCounter += 1

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1790")
         End Try
      Next

      Return New Point2d(dCenterX / iCounter, dCenterY / iCounter)
   End Function
   Private Function zzGetAllPointsCenter(bNeighbours As Boolean) As Point2d
      Dim oaCurvePoint() As CurvePoint
      Dim tAcadPoint As Point2d
      Dim dCenterX, dCenterY As Double
      Dim iCounter As Integer = 0
      Dim iAllIndex As Integer = 0
      Dim tCurvePoint As CurvePoint
      For iIndexInner As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         Try
            tCurvePoint = moaAcadEntity(iIndexInner)

            tAcadPoint = tCurvePoint.AcadPoint
            dCenterX += tAcadPoint.X
            dCenterY += tAcadPoint.Y
            iCounter += 1

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1790")
         End Try
      Next
      If bNeighbours AndAlso mhsNeighborCells IsNot Nothing Then
         For Each oNeighborCell As EntityCell In mhsNeighborCells
            oaCurvePoint = oNeighborCell.AcadEntities
            For iIndexInner As Integer = 0 To oaCurvePoint.GetUpperBound(0)

               tAcadPoint = oaCurvePoint(iIndexInner).AcadPoint
               dCenterX += tAcadPoint.X
               dCenterY += tAcadPoint.Y
               iCounter += 1

            Next
         Next
      End If
      Return New Point2d(dCenterX / iCounter, dCenterY / iCounter)
   End Function
   Private Function zzGetCenterPoint(ByVal bNotOverlayOnly As Boolean) As Point2d
      Dim oaCurvePoint() As CurvePoint
      Dim tAcadPoint As Point2d
      Dim dCenterX, dCenterY As Double
      Dim iCounter As Integer = 0
      Dim iAllIndex As Integer = 0
      Dim tCurvePoint As CurvePoint
      For iIndexInner As Integer = 0 To moaAcadEntity.GetUpperBound(0)
         Try
            tCurvePoint = moaAcadEntity(iIndexInner)
            If Not bNotOverlayOnly OrElse Not tCurvePoint.Overlay Then
               tAcadPoint = tCurvePoint.AcadPoint
               dCenterX += tAcadPoint.X
               dCenterY += tAcadPoint.Y
               iCounter += 1
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1790")
         End Try
      Next
      If moaNeighborCells IsNot Nothing Then
         For iIndex As Integer = 0 To moaNeighborCells.GetUpperBound(0)
            oaCurvePoint = moaNeighborCells(iIndex).AcadEntities
            For iIndexInner As Integer = 0 To oaCurvePoint.GetUpperBound(0)
               If Not bNotOverlayOnly OrElse Not oaCurvePoint(iIndexInner).Overlay Then
                  tAcadPoint = oaCurvePoint(iIndexInner).AcadPoint
                  dCenterX += tAcadPoint.X
                  dCenterY += tAcadPoint.Y
                  iCounter += 1
               End If
            Next
         Next
      End If
      Return New Point2d(dCenterX / iCounter, dCenterY / iCounter)
   End Function
   Public Sub Rounding(ByVal bFix As Boolean)
      Dim bErr As Boolean = False
      moaErrorPoints = New TplnPointArray()
      miSourceErrors = 0
      If Not bFix Then
         Me.zzMarkPoints(False)
      ElseIf Me.HasOuterCell Then
         bErr = True
      Else
         Select Case Me.NeighborCount
            Case 0
               Me.zzRoundingAlone()
            Case 1
               Me.zzRoundingWithOne()
            Case 2
               Me.zzRoundingWithTwo()
            Case 3
               Me.zzRoundingWithThree()
            Case Else
               bErr = True
         End Select
      End If
      If bErr Then
         moaErrorPoints.Add(mtMinKeyPoint.GetAbsPoint2d())
      End If
   End Sub
   Public Sub CalcPointsNearLines()
      '  AcadDocument.WriteMessage("!!!!!!!!!!!CalcPointsNearLines!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!")
      If moaAcadEntity IsNot Nothing Then
         '  AcadDocument.WriteMessage("moaAcadEntity!!--?????????????????? " & CStr(Me.HasIntersectingLines) & " AcadEntity=" & CStr(moaAcadEntity.GetUpperBound(0)))
      End If

      If Me.HasIntersectingLines Then
         For Each tLineObjID As ObjectId In mdicIntersectingLines
            '  AcadDocument.WriteMessage("!!++++" & tLineObjID.ToString())
            zzCalcPointsNearLine(tLineObjID)
         Next
      End If
   End Sub
   Private Sub zzCalcPointsNearLine(tLineObjID As ObjectId)
      If moaAcadEntity IsNot Nothing Then
         Dim oCurve As Curve '= AcadTransaction.GetCurve(tLineObjID, False, OpenMode.ForRead)
         Dim tClosestPointOnCurve As Point3d
         Dim bRes As Boolean
         '  AcadDocument.WriteMessage("!!--------------------------------------------------------")
         For Each tCurvePoint As CurvePoint In moaAcadEntity
            '  AcadDocument.WriteMessage("!!++++" & tCurvePoint.CurveObjID.ToString() & ":" & tLineObjID.ToString())
            If tCurvePoint.CurveObjID <> tLineObjID Then

               oCurve = AcadTransaction.GetCurve(tLineObjID, False, OpenMode.ForRead)
               tClosestPointOnCurve = oCurve.GetClosestPointTo(tCurvePoint.AcadPoint3d, False)
               If tClosestPointOnCurve.DistanceTo(tCurvePoint.AcadPoint3d) <= PointLineTolerance Then
                  AcadDocument.WriteDebugMessage("!!--Dist=" & tClosestPointOnCurve.DistanceTo(tCurvePoint.AcadPoint3d).ToString() & "; Tol=" & PointLineTolerance.ToString())
                  bRes = mhsErrPointLines.AddItem(tCurvePoint, tLineObjID)
                  If bRes Then
                     AcadDocument.WriteDebugMessage("!!!!--Dist=" & tClosestPointOnCurve.ToString & ";!@@=" & tCurvePoint.AcadPoint3d.ToString() & "; Tol=" & PointLineTolerance.ToString())
                     '   AcadDocument.WriteMessage("!!!!--Dist=" & tClosestPointOnCurve.ToString & ";" & tCurvePoint.AcadPoint3d.ToString() & "; Tol=" & PointLineTolerance.ToString())
                  End If
               End If
            End If
         Next
      End If
   End Sub

   Public Shared Sub DrawNodes()
      Dim shColor As Short
      AcadDocument.WriteMessage("3--Node:" & mdicNodes.Count.ToString())
      For Each oEntityCell As EntityCell In mdicNodes.Values
         If oEntityCell.HasIntersectingLines Then
            shColor = 1S
         Else
            shColor = 2S
         End If

         oEntityCell.DrawCell(shColor)
      Next
   End Sub
   Public Sub DrawCell(shColor As Short)


      '   AcadDocument.WriteDebugMessage("!!!!--MarkPoint=" & mtMinKeyPoint.GetAbsPoint2d().ToString() & "; " & moCellMarkBlock.BlockName)
      moCellMarkBlock.MarkPoint(mtMinKeyPoint.GetAbsPoint2d(), shColor)

   End Sub
   Public Shared ReadOnly Property ErrPoints() As TplnPointArray
      Get
         Return moaErrorPoints
      End Get
   End Property
   Public Shared ReadOnly Property SourceErrors() As Integer
      Get
         Return miSourceErrors
      End Get
   End Property
   Public Shared ReadOnly Property ErrPointLinesCount() As Integer
      Get
			Return mhsErrPointLines.Count
		End Get
   End Property
   Public Shared Sub ExecAll(ByVal bFix As Boolean, ByVal bRounding As Boolean)
      '  System.Windows.Forms.MessageBox.Show(bFix.ToString() & ":" & bRounding.ToString() & vbCrLf & EntityCell.CircleMarkBlock.BlockName, "05_484")



      '    System.Windows.Forms.MessageBox.Show(mdicNodes.Count.ToString(), "05_489")
      For Each oEntityCell As EntityCell In mdicNodes.Values
         '  DMAcadExt.AcadDocument.WriteMessage("+4 NeighborCount:" & CStr(mdicNodes.Count) & "!" & CStr(oEntityCell.NeighborCount) & "," & CStr(oEntityCell.AcadEntities.GetUpperBound(0)))
         If bRounding Then
            oEntityCell.Rounding(bFix)
         Else
            oEntityCell.MergingNew(bFix)
         End If
      Next

   End Sub
   Public Property RootCell() As EntityCell
      Get
         Return moRootCell
      End Get
      Set(ByVal oValue As EntityCell)
         moRootCell = oValue
      End Set
   End Property
   Public ReadOnly Property RootCode() As Long
      Get
         If moRootCell IsNot Nothing Then
            Return moRootCell.MyKeyPoint.Code
         Else
            Return 0L
         End If

      End Get

   End Property
   Public Shared ReadOnly Property NodesCount() As Integer
      Get
         If mdicNodes IsNot Nothing Then
            Return mdicNodes.Count
         Else
            Return 0
         End If

      End Get
   End Property
   Public Shared ReadOnly Property MaxUpdateDist() As Double
      Get
         Return mdMaxUpdateDist
      End Get
   End Property
   Public Shared ReadOnly Property MaxErrDist() As Double
      Get
         Return mdMaxErrDist
      End Get
   End Property
   Public ReadOnly Property AllPoints() As Point2dCollection
      Get
         Return mcolAllPoints
      End Get
   End Property
   Public ReadOnly Property SourcePoints() As Point2dCollection
      Get
         Return mcolSourcePoints
      End Get
   End Property

   Public ReadOnly Property AcadEntities() As CurvePoint()
      Get
         Return moaAcadEntity
      End Get
   End Property

   Public Shared Function GetCoefficient() As Integer
      Dim iRes As Integer = 1
      For iIndex As Integer = 1 To Digits
         iRes *= 10
      Next
      Return iRes
   End Function
   Public Function GetRelationList() As enCellRelation()
      Dim iaRes() As enCellRelation = Nothing
      Select Case miRelationToRoot
         Case enCellRelation.Undefined, enCellRelation.NorthEast
            ReDim iaRes(3)
            iaRes(0) = enCellRelation.North
            iaRes(1) = enCellRelation.NorthEast
            iaRes(2) = enCellRelation.East
            iaRes(3) = enCellRelation.SouthEast
         Case enCellRelation.North
            ReDim iaRes(1)
            iaRes(0) = enCellRelation.North
            iaRes(1) = enCellRelation.NorthEast
         Case enCellRelation.East, enCellRelation.SouthEast
            ReDim iaRes(2)
            iaRes(0) = enCellRelation.NorthEast
            iaRes(1) = enCellRelation.East
            iaRes(2) = enCellRelation.SouthEast
         Case Else
      End Select
      Return iaRes
   End Function
   Private Class NeigborEnum
      Implements System.Collections.Generic.IEnumerator(Of KeyPoint)
      Private moaNeigborPoints() As KeyPoint
      Private miEnumIndex As Integer = -1
      Public Sub New(oaNeigborPoints() As KeyPoint)
         moaNeigborPoints = oaNeigborPoints
      End Sub


      Public ReadOnly Property NeighborCell As KeyPoint Implements IEnumerator(Of KeyPoint).Current
         Get
            If miEnumIndex >= 0 AndAlso miEnumIndex <= moaNeigborPoints.GetUpperBound(0) Then
               Return moaNeigborPoints(miEnumIndex)
            Else
               Return Nothing
            End If

         End Get
      End Property

		Public ReadOnly Property Current As System.Object Implements IEnumerator.Current
			Get
				Return moaNeigborPoints(miEnumIndex)
			End Get
		End Property

		Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
         If miEnumIndex >= -1 Then
            miEnumIndex += 1
            Return miEnumIndex <= moaNeigborPoints.GetUpperBound(0)
         Else
            Return False
         End If

      End Function

      Public Sub Reset() Implements IEnumerator.Reset
         miEnumIndex = -1
      End Sub

#Region "IDisposable Support"
      Private disposedValue As Boolean ' To detect redundant calls

      ' IDisposable
      Protected Overridable Sub Dispose(disposing As Boolean)
         If Not Me.disposedValue Then
            If disposing Then
               ' TODO: dispose managed state (managed objects).
               Erase moaNeigborPoints
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
         End If
         Me.disposedValue = True
      End Sub

      ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
      'Protected Overrides Sub Finalize()
      '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      '    Dispose(False)
      '    MyBase.Finalize()
      'End Sub

      ' This code added by Visual Basic to correctly implement the disposable pattern.
      Public Sub Dispose() Implements IDisposable.Dispose
         ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
         Dispose(True)
         GC.SuppressFinalize(Me)
      End Sub
#End Region

   End Class
End Class

Public Structure CurvePoint
   ' Private moCurve As Curve
   Private mtCurveObjID As ObjectId

	'	Private moTopoDef As TopoDef
	Private miPointIndex As Integer
	Private mbInner As Boolean
	Private mtPoint2d As Point2d
	Private mbOverlay As Boolean
	Private miPriority As Integer
	Private Shared mcolZeroLenPolylines As System.Collections.ObjectModel.Collection(Of Polyline)
	'	Private mbOriginExists As Boolean
	Public Sub New(ByVal tPoint2d As Point2d, ByVal tCurveObjID As ObjectId, ByVal iPointIndex As Integer, ByVal bInner As Boolean, ByVal bOverlay As Boolean)
		'   moCurve = oCurve
		mtCurveObjID = tCurveObjID
		'	moTopoDef = oTopoDef
		miPointIndex = iPointIndex
		mbInner = bInner
		mbOverlay = bOverlay
		mtPoint2d = tPoint2d
	End Sub
	Public Sub New(ByVal tPoint2d As Point2d, ByVal tCurveObjID As ObjectId, ByVal iPointIndex As Integer, ByVal bInner As Boolean, ByVal iPriority As Integer)
		'   moCurve = oCurve
		mtCurveObjID = tCurveObjID
		'	moTopoDef = oTopoDef
		miPointIndex = iPointIndex
		mbInner = bInner

		mtPoint2d = tPoint2d
		miPriority = iPriority
	End Sub
	Public Shared Operator =(tCurvePoint1 As CurvePoint, tCurvePoint2 As CurvePoint) As Boolean
      Return tCurvePoint1.CurveObjID = tCurvePoint2.CurveObjID AndAlso tCurvePoint1.PointIndex = tCurvePoint2.PointIndex
   End Operator
   Public Shared Sub Init()

      mcolZeroLenPolylines = New ObjectModel.Collection(Of Polyline)()

   End Sub
   Public Shared Operator <>(tCurvePoint1 As CurvePoint, tCurvePoint2 As CurvePoint) As Boolean
      Return Not (tCurvePoint1 = tCurvePoint2)
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
   Public ReadOnly Property AcadPoint3d() As Point3d
      Get
         Return TPlnPoint.Point2dTo3d(mtPoint2d)
      End Get
   End Property
	Public ReadOnly Property AcadPointCoordinates() As String
		Get
			Return TPlnPoint.DispPoint(mtPoint2d)
		End Get
	End Property
	Public ReadOnly Property Overlay() As Boolean
		Get
			Return mbOverlay
		End Get
	End Property
	Public ReadOnly Property Inner() As Boolean
		Get
			Return mbInner
		End Get
   End Property
   Public ReadOnly Property CurveObjID() As ObjectId
      Get
         Return mtCurveObjID
      End Get
   End Property
   Public ReadOnly Property PointIndex As Integer
      Get
         Return miPointIndex
      End Get
   End Property

	Public Function GetDistanceTo(ByVal tPoint2d As Point2d) As Double
		Return tPoint2d.GetDistanceTo(tPoint2d)
	End Function
	
	Public Sub UpdatePoint(ByVal tNewPoint As Point2d)
		'	DMAcadExt.AcadDocument.WriteMessage(CStr(mtPoint2d.X) & "," & CStr(mtPoint2d.Y) & " UpdatePoint to:" & CStr(tNewPoint.X) & "," & CStr(tNewPoint.Y))
		Dim oDBObject As DBObject = AcadTransaction.GetDBObject(mtCurveObjID, OpenMode.ForWrite)

		Select Case oDBObject.GetRXClass().Name
			Case AcadConst.AcadPolylineName
				Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
				zzUpdatePolylinePoint(oPolyline, tNewPoint)
			Case AcadConst.Acad2dPolylineName
				Dim oPolyline2d As Polyline2d = DirectCast(oDBObject, Polyline2d)
				zzUpdatePolyline2dPoint(oPolyline2d, tNewPoint)
			Case AcadConst.AcadLineName
				Dim oLine As Line = DirectCast(oDBObject, Line)
				zzUpdatLinePoint(oLine, tNewPoint)
			Case AcadConst.AcadArcName
				Dim oArc As Arc = DirectCast(oDBObject, Arc)
				zzUpdateArcPoint(oArc, tNewPoint)
			Case AcadConst.AcadBlockRefName
				Dim oBlockRef As BlockReference = DirectCast(oDBObject, BlockReference)
				zzUpdatBlockRefPoint(oBlockRef, tNewPoint)
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
            '	DMAcadExt.AcadDocument.WriteMessage("Angle before:" & CStr(oArc.StartAngle) & "," & CStr(oArc.Radius))
            '	DMAcadExt.AcadDocument.WriteMessage("Angle After:" & CStr(oArc.StartAngle) & "," & CStr(oArc.Radius))
            '	DMAcadExt.AcadDocument.WriteMessage("A Center:" & CStr(tCenter.X) & "," & CStr(tCenter.Y))
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
	Private Sub zzUpdatBlockRefPoint(oBlockRef As BlockReference, ByVal tNewPoint As Point2d)
		oBlockRef.Position = TPlnPoint.Point2dTo3d(tNewPoint)
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
Public Structure LineSegmentID
	Public AcadObjID As ObjectId
	Public SegmentIndex As Integer
End Structure
Public Structure LineSegment
	Dim StartPoint As Point2d
	Dim EndPoint As Point2d
	Public AcadObjID As ObjectId
	Public SegmentIndex As Integer
	Public Sub New(ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d)
		StartPoint = tStartPoint
		EndPoint = tEndPoint
	End Sub
	
End Structure
Public Structure KeyPoint
	Public Shared CoarseRoundScale As Double = 1.0
	Public Shared ByCenter As Boolean = False
	Private Shared mdOriginX As Double
	Private Shared mdOriginY As Double
	Private Shared mtPointTolerance As Tolerance
   Private Shared mdRoundScale As Double = 1.0
   Private Shared mdRoundScaleInv As Double = 1.0



	Public X As Long
	Public Y As Long
	Public Sum As Long
	Public Code As Long
   'Private mtPoint As System.Drawing.Point
   Public Shared Function ToKeyPointX(dX As Double) As Long
      Return zzToMinInt(mdRoundScale * (dX - mdOriginX))
   End Function
   Public Shared Function ToKeyPointY(dY As Double) As Long
      Return zzToMinInt(mdRoundScale * (dY - mdOriginY))
   End Function
   Public Shared Function GetPointByOrigin(tPoint As Point2d) As Point2d
      Return New Point2d(tPoint.X - mdOriginX, tPoint.Y - mdOriginY)
   End Function

	Public Sub New(ByVal tPoint As Point2d, ByVal bCoarse As Boolean)
      Me.New(tPoint.X, tPoint.Y, bCoarse)

	End Sub

   Public Sub New(ByVal dX As Double, ByVal dY As Double, ByVal bCoarse As Boolean)

      Dim dScale As Double = Me.zzGetRoundScale(bCoarse)

      Dim dXInt As Double = Math.Round((dX - mdOriginX) * dScale, 8)
      Dim dYInt As Double = Math.Round((dY - mdOriginY) * dScale, 8)




		'   AcadDocument.WriteMessage("dScale=" & CStr(dScale) & "; XDelta=" & CStr(dXDelta) & ";  Xint=" & CStr(dXInt) & ";  yint=" & CStr(dYInt))
		If ByCenter Then
			zzNew(zzToRoundInt(dXInt), zzToRoundInt(dYInt))
		Else
			zzNew(zzToMinInt(dXInt), zzToMinInt(dYInt))
		End If

	End Sub

   Public Sub New(ByVal lX As Long, ByVal lY As Long)

      zzNew(lX, lY)

   End Sub
	Public Sub New(ByVal tKeyPoint As KeyPoint, ByVal iDir As enCellRelation)
      Const lOne As Long = 1L
      Select Case iDir
			Case enCellRelation.North
				zzNew(tKeyPoint.X, tKeyPoint.Y + lOne)
			Case enCellRelation.NorthEast
				zzNew(tKeyPoint.X + lOne, tKeyPoint.Y + lOne)
			Case enCellRelation.East
				zzNew(tKeyPoint.X + lOne, tKeyPoint.Y)
			Case enCellRelation.SouthEast
				zzNew(tKeyPoint.X + lOne, tKeyPoint.Y - lOne)
			Case enCellRelation.South
				zzNew(tKeyPoint.X, tKeyPoint.Y - lOne)
			Case enCellRelation.SouthWest
				zzNew(tKeyPoint.X - lOne, tKeyPoint.Y - lOne)
			Case enCellRelation.West
				zzNew(tKeyPoint.X - lOne, tKeyPoint.Y)
			Case enCellRelation.NorthWest
				zzNew(tKeyPoint.X - lOne, tKeyPoint.Y + lOne)
		End Select
   End Sub
   Public Shared Operator =(tKeyPoint1 As KeyPoint, tKeyPoint2 As KeyPoint) As Boolean
      Return (tKeyPoint1.X = tKeyPoint2.X) AndAlso (tKeyPoint1.Y = tKeyPoint2.Y)
   End Operator
   Public Shared Operator <>(tKeyPoint1 As KeyPoint, tKeyPoint2 As KeyPoint) As Boolean
      Return Not (tKeyPoint1.X = tKeyPoint2.X)
   End Operator
   Public Function GetDistanceTo(ByVal tPoint2d As Point2d) As Double
      Return tPoint2d.GetDistanceTo(GetAbsPoint2d())
   End Function
   Public Shared Function GetRealX(lX As Long) As Double
      Return mdOriginX + lX * mdRoundScaleInv
   End Function
   Public Function GetAbsX() As Double
      Return mdOriginX + X / mdRoundScale
   End Function
   Public Function GetAbsY() As Double
      Return mdOriginY + Y / mdRoundScale
   End Function
   Public Function GetAbsPoint2d() As Point2d
      Return New Point2d(mdOriginX + X / mdRoundScale, mdOriginY + Y / mdRoundScale)
   End Function
	 
	Public Function GetAbsPoint3d() As Point3d
		Return New Point3d(mdOriginX + X / mdRoundScale, mdOriginY + Y / mdRoundScale, 0.0)
	End Function
	Public Function GetCoarseCenterPoint3d() As Point3d
		Return New Point3d(mdOriginX + (X + 0.5) / CoarseRoundScale, mdOriginY + (Y + 0.5) / CoarseRoundScale, 0.0)
	End Function
	Public Function GetCoarsePoint2d() As Point2d
		Return New Point2d(mdOriginX + X / CoarseRoundScale, mdOriginY + Y / CoarseRoundScale)
	End Function
	Public Function GetTPlnPoint() As TPlnPoint
		Return New TPlnPoint(mdOriginX + X / mdRoundScale, mdOriginY + Y / mdRoundScale)
	End Function
	Public Function GetCoarseTPlnPoint() As TPlnPoint
		Return New TPlnPoint(mdOriginX + X / CoarseRoundScale, mdOriginY + Y / CoarseRoundScale)
   End Function
   Public Overrides Function ToString() As String
      Return X.ToString & "," & Y.ToString()
   End Function
	Public Shared Sub SetOrigin(ByVal tPoint As Point2d)
		mdOriginX = Math.Floor(tPoint.X)
		mdOriginY = Math.Floor(tPoint.Y)
		AcadDocument.WriteMessage("Origin=" & CStr(mdOriginX) & "," & CStr(mdOriginY))
	End Sub
	Public Shared Property RoundScale() As Double
		Get
			Return mdRoundScale
		End Get
		Set(ByVal dValue As Double)
			mdRoundScale = dValue
		End Set
	End Property
	Public Shared Property Tolerance() As Double
		Get
			Return 1.0 / mdRoundScale
		End Get
      Set(ByVal dValue As Double)
         mdRoundScaleInv = dValue
			mdRoundScale = Math.Round(1.0 / dValue, MidpointRounding.AwayFromZero)
			mtPointTolerance = New Tolerance(0.0, dValue)
         AcadDocument.WriteMessage("RoundScale=" & CStr(mdRoundScale) & ";  RoundScaleInv=" & CStr(mdRoundScaleInv))
      End Set
	End Property
	Public Shared ReadOnly Property PointTolerance() As Tolerance
		Get
			Return mtPointTolerance
		End Get
	End Property
	Public ReadOnly Property AbsCoordinates() As String
		Get
			Return Convert.ToString(mdOriginX + X / mdRoundScale) & "," & Convert.ToString(mdOriginY + Y / mdRoundScale)
		End Get
	End Property
	Public ReadOnly Property CoarseAbsCoordinates() As String
		Get
			Return Convert.ToString(mdOriginX + X / CoarseRoundScale) & "," & Convert.ToString(mdOriginY + Y / CoarseRoundScale)
		End Get
	End Property
	Public ReadOnly Property Coordinates() As String
		Get
			Return Convert.ToString(X) & "," & Convert.ToString(Y)
		End Get
	End Property
	Public Shared Function GetCoarseAbsX(ByVal lX As Long) As Double
		Return mdOriginX + lX / CoarseRoundScale
	End Function
	Public Shared Function GetCoarseAbsY(ByVal lY As Long) As Double
		Return mdOriginY + lY / CoarseRoundScale
	End Function
	Public Shared Function GetCoarseX(ByVal dX As Double) As Long
		Return zzToMinInt(CoarseRoundScale * (dX - mdOriginX))
	End Function
	Public Shared Function GetCoarseY(ByVal dY As Double) As Long
		Return zzToMinInt(CoarseRoundScale * (dY - mdOriginY))
	End Function

	Private Sub zzNew(ByVal lX As Long, ByVal lY As Long)
		Dim lR As Long
		X = lX
      Y = lY
      '  DMAcadExt.AcadDocument.WriteMessage("-New_KeyPnt:" & X.ToString() & "," & Y.ToString())
		Sum = X + Y
		Try
         '29/06/16   Code = Math.DivRem(Sum * (Sum + 1), 2L, lR) + lX
         Code = Math.DivRem((Sum + 1), 2L, lR)
         Code = Code * (Sum + lR) + lX
		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & Sum.ToString(), "KeyPoint - Code")
         AcadDocument.WriteMessage("XY=" & CStr(lX) & "," & CStr(lY))
		End Try
      '   Dim iii As Long = -10652570521
      '   Dim ii2 As Long = -200000020000000

	End Sub
	Private Shared Function zzToMinInt(ByVal dValue As Double) As Long
		Dim lRes As Long
		Try
			lRes = Convert.ToInt64(Math.Floor(dValue))
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & dValue.ToString(), "KeyPoint - zzToMinInt")
			lRes = 100L
		End Try
		Return lRes
	End Function
	Private Shared Function zzToRoundInt(ByVal dValue As Double) As Long
		Dim lRes As Long
		Try
			lRes = Convert.ToInt64(Math.Round(dValue, MidpointRounding.AwayFromZero))
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & dValue.ToString(), "KeyPoint - zzToMinInt")
			lRes = 100L
		End Try
		Return lRes
	End Function

	Private Function zzGetRoundScale(ByVal bCoarse As Boolean) As Double
		If bCoarse Then
			Return CoarseRoundScale
		Else
			Return mdRoundScale
		End If
	End Function
End Structure