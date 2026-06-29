Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices

Public Class EntityCells
	Inherits Generic.SortedDictionary(Of Long, EntityCell)
	'	Inherits Generic.SortedDictionary(Of KeyPoint, EntityCell) 20/05/09
	Private miSourceErrors As Integer
	Private moErrPointLineArray As TplnPointArray
	Private mbCoarse As Boolean
   Private mdLineTolerance As Double
   Private mdArcStep As Double


   Dim miTestCurves, miTestArc, miTestLine, miPolylineCounter, miTestPolyline2d, miTestPoints As Integer
   Public ReadOnly Property SorceErrors() As Integer
      Get
			Return miSourceErrors
		End Get
   End Property
   Public ReadOnly Property ErrPoints() As TplnPointArray
      Get
         Return moErrPointLineArray
      End Get
   End Property
   Public ReadOnly Property ErrPointLines() As TplnPointArray
      Get
         Return moErrPointLineArray
      End Get
   End Property
   Public Property ArcStep As Double
      Get
         Return mdArcStep
      End Get
      Set(dValue As Double)
         mdArcStep = dValue
      End Set
   End Property

   Public Sub PrintSummary()
      AcadDocument.WriteMessage("Points:" & CStr(miTestPoints))
      AcadDocument.WriteMessage("Arcs:" & CStr(miTestArc))
      AcadDocument.WriteMessage("Lines:" & CStr(miTestLine))
      AcadDocument.WriteMessage("Polylines:" & CStr(miPolylineCounter))
      AcadDocument.WriteMessage("2dPolylines:" & CStr(miTestPolyline2d))
      DMAcadExt.AcadDocument.WriteMessage("Cells:" & CStr(Me.Count))
      DMAcadExt.AcadDocument.WriteMessage("PoinLinesErrs:" & CStr(EntityCell.ErrPointLinesCount))

   End Sub
   Public Sub CalculateNew()
      Dim bTest As Boolean = False
      Dim iDir As enCellRelation
      Dim tNeighborKeyPoint As KeyPoint
      Dim oNeighborCell As EntityCell = Nothing
      Dim bNeighborCellExists As Boolean
      Dim oCurrentRoot As EntityCell
      Dim iaRelations() As enCellRelation
      Dim iTest As Integer

		For Each oEntityCell As EntityCell In MyBase.Values
			If oEntityCell.HasCurvePoints Then
				bNeighborCellExists = False
				iaRelations = oEntityCell.GetRelationList()
				oCurrentRoot = oEntityCell.RootCell
				If iaRelations IsNot Nothing Then
					For iIndex As Integer = 0 To iaRelations.GetUpperBound(0)
						iDir = iaRelations(iIndex)
						tNeighborKeyPoint = oEntityCell.GetNeighborPoints(iDir)

						If MyBase.TryGetValue(tNeighborKeyPoint.Code, oNeighborCell) Then
							If oNeighborCell.HasCurvePoints Then
								bNeighborCellExists = True

								'  oEntityCell.AddNeighborCell(oNeighborCell, iDir, (bTest AndAlso iTest = 1289))
								oEntityCell.AddCloseNeighborCell(oNeighborCell, iDir)
							End If
						End If
					Next

				End If
				''""""""""""""""""""""""""""""""""""""
				If oEntityCell.HasSourceError Then   ''''''''''''''''AndAlso oEntityCell.RootCell Is Nothing
					oEntityCell.AddMeToNodes()
				End If

				iTest += 1
				oEntityCell.CalcPointsNearLines()
			End If
		Next

		For Each oEntityCell As EntityCell In MyBase.Values
         If oEntityCell.HasNeighboursNew Then
				oEntityCell.CalculateN()
			End If
      Next
   End Sub

   Public Sub DrawIntersectingLinesCells()

      '   System.Windows.Forms.MessageBox.Show(CStr(MyBase.Values.Count), "08_120 DrawCells")
      Dim shColor As Short
      For Each oEntityCell As EntityCell In MyBase.Values
         If oEntityCell.HasIntersectingLines Then
            shColor = 6S
         Else
            shColor = 3S
         End If

         oEntityCell.DrawCell(shColor)
      Next
   End Sub
   Public Sub DrawCurvePointsCells()
      System.Windows.Forms.MessageBox.Show(CStr(MyBase.Values.Count), "08_122 DrawCells")
      Dim shColor As Short
      For Each oEntityCell As EntityCell In MyBase.Values
         If oEntityCell.HasCurvePoints Then
            shColor = 6S
         Else
            shColor = 3S
         End If

         oEntityCell.DrawCell(shColor)
      Next
   End Sub
   Public Sub TestCells()
      '   System.Windows.Forms.MessageBox.Show(CStr(MyBase.Values.Count), "08_120 DrawCells")
      For Each oEntityCell As EntityCell In MyBase.Values
         If oEntityCell.IsSinglePoint Then
            DMAcadExt.AcadDocument.WriteDebugMessage("$$%% " & oEntityCell.MyKeyPoint.AbsCoordinates)
         End If
         If oEntityCell.AllPoints.Count > 0 Then
            AcadDocument.WriteMessage("!!@@@mcolAllPoints:" & oEntityCell.AllPoints.Count.ToString() & "; mcolSourcePoints:" & oEntityCell.SourcePoints.Count.ToString() & ";Has:" & oEntityCell.HasCurvePoints.ToString() & ";Sng:" & oEntityCell.IsSinglePoint.ToString())
         End If
      Next
   End Sub
   Public Sub TestNeigbours()
      '   System.Windows.Forms.MessageBox.Show(CStr(MyBase.Values.Count), "08_120 DrawCells")
      Dim oNeighbor As EntityCell
      Dim oaNeighborCells() As EntityCell
      For Each oEntityCell As EntityCell In MyBase.Values
         If oEntityCell.NeighborCount > 0 Then
            DMAcadExt.AcadDocument.WriteDebugMessage("Cell_ " & oEntityCell.NeighborCount.ToString() & " " & oEntityCell.MyKeyPoint.AbsCoordinates)
            oaNeighborCells = oEntityCell.NeighborCells
            For iIndex As Integer = 0 To oEntityCell.NeighborCount - 1
               oNeighbor = oaNeighborCells(iIndex)
               DMAcadExt.AcadDocument.WriteDebugMessage("NB_ " & iIndex.ToString() & " " & oNeighbor.MyKeyPoint.AbsCoordinates)
            Next

         End If
         
      Next
   End Sub

   Public Sub AddArc(ByVal oArc As Arc, ByVal bOverlay As Boolean)
      Me.zzAddEntity(zzPoint3dTo2d(oArc.StartPoint), oArc, 0, False, bOverlay)
      Me.zzAddEntity(zzPoint3dTo2d(oArc.EndPoint), oArc, 1, False, bOverlay)

      Dim oTplnArc As TplnArc = New TplnArc(oArc)
      Me.zzAddArcSegment(oTplnArc, oArc.ObjectId)

      miTestArc += 1
   End Sub
   Public Sub AddLine(ByVal oLine As Line, ByVal bOverlay As Boolean)
      Dim tStartPoint As Point2d = zzPoint3dTo2d(oLine.StartPoint)
      Dim tEndPoint As Point2d = zzPoint3dTo2d(oLine.EndPoint)
      Dim oSegment As DMAcadExt.TplnLine = New DMAcadExt.TplnLine(oLine)

		Me.zzAddEntity(tStartPoint, oLine, 0, False, bOverlay)
		Me.zzAddEntity(tEndPoint, oLine, 1, False, bOverlay)
      zzAddLineSegment(oSegment, oLine.ObjectId)
      miTestLine += 1

   End Sub
   Public Sub AddPolyline2d(ByVal oPolyline2d As Polyline2d, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean)
      Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
      Dim tObjID As ObjectId
      Dim oDBObj As DBObject
      Dim oVertex2d As Vertex2d = Nothing
      Dim tPoint As Point2d
      Dim tEndPoint As Point2d = zzPoint3dTo2d(oPolyline2d.EndPoint)
      Dim iPointIndex As Integer = 0
      Dim bInner As Boolean
      Try
         Do While oColEnum.MoveNext()
            tObjID = DirectCast(oColEnum.Current, ObjectId)
            oDBObj = AcadTransaction.GetDBObject(tObjID, OpenMode.ForWrite)
            oVertex2d = DirectCast(oDBObj, Vertex2d)
            tPoint = zzPoint3dTo2d(oVertex2d.Position)
            If iPointIndex = 0 Then
               bInner = False
            ElseIf tPoint.IsEqualTo(tEndPoint) Then
               bInner = False
            Else
               bInner = True
            End If
            Me.zzAddEntity(tPoint, oPolyline2d, iPointIndex, bInner, bOverlay)
            iPointIndex += 1
            '	DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iVert) & ":" & TPlnPoint.DispPoint(oVertex2d.Position) & ";" & CStr(oVertex2d.Bulge) & "-" & oVertex2d.VertexType.ToString())
         Loop
      Catch oEx As Exception
         '		DMAcadExt.AcadDocument.WriteMessage("Reset:" & oEx.Message)
      End Try


      '	Me.zzAddEntity(zzPoint3dTo2d(oPolyline2d.StartPoint), oPolyline2d, 0, oTopoDef, bOverlay)
      '	Me.zzAddEntity(zzPoint3dTo2d(oPolyline2d.EndPoint), oPolyline2d, 1, oTopoDef, bOverlay)
      miTestPolyline2d += 1
   End Sub
   'ubrat=
   Public Sub AddPolylineCounter()
      miPolylineCounter += 1
   End Sub
   Public Sub AddPolyline2dCounter()
      miTestPolyline2d += 1
   End Sub
   Public Sub AddPolyline(ByVal oPolyline As Polyline, ByVal bOverlay As Boolean, ByVal bPoint As Boolean, ByVal bPointLine As Boolean)
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim bInner As Boolean
      Dim tNextPoint, tCurrentPoint As Point2d
      Dim dBulge As Double
      Dim oTplnLine As TplnLine
      Dim oTplnArc As TplnArc
      '  System.Windows.Forms.MessageBox.Show(iVerticesUB.ToString(), "05_511")
      For iIndex As Integer = 0 To iVerticesUB
         If Not oPolyline.Closed AndAlso (iIndex = 0 OrElse iIndex = iVerticesUB) Then
            bInner = False
         Else
            bInner = True
         End If
         tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)


         dBulge = oPolyline.GetBulgeAt(iIndex)
         If bPoint Then
            '   AcadDocument.WriteMessage("!!@Point:" & tCurrentPoint.ToString() & "** " & bInner.ToString() & ".. " & bOverlay.ToString())
            Me.zzAddEntity(tCurrentPoint, oPolyline, iIndex, bInner, bOverlay)
         End If

         If bPointLine AndAlso ((iIndex <> iVerticesUB) OrElse oPolyline.Closed) Then
            tNextPoint = oPolyline.GetPoint2dAt((iIndex + 1) Mod (iVerticesUB + 1))
            If dBulge = 0 Then
               oTplnLine = New TplnLine(tCurrentPoint, tNextPoint)
               Me.zzAddLineSegment(oTplnLine, oPolyline.ObjectId)
            Else
               oTplnArc = New TplnArc(tCurrentPoint, tNextPoint, dBulge)
               Me.zzAddArcSegment(oTplnArc, oPolyline.ObjectId)
            End If

         End If

      Next
      '	Me.zzAddEntity(oPolyline.GetPoint2dAt(0), oPolyline, 0)
      ' 	Me.zzAddEntity(oPolyline.GetPoint2dAt(iVerticesUB), oPolyline, iVerticesUB)
      miPolylineCounter += 1
   End Sub
   Public Sub AddPolylineZ(ByVal oPolyline As Polyline, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean)
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim bInner As Boolean
      Dim tNextPoint, tCurrentPoint As Point2d
      For iIndex As Integer = 0 To iVerticesUB
         If iIndex = 0 AndAlso iIndex = iVerticesUB Then
            bInner = False
         Else
            bInner = True
         End If
         tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
         Me.zzAddEntity(tCurrentPoint, oPolyline, iIndex, bInner, bOverlay)
         If iIndex <> 0 Then
            Me.zzAddLineSegmentZZZ(tCurrentPoint, tNextPoint, oPolyline)
         End If

         If iIndex <> iVerticesUB Then
            Me.zzAddLineSegmentZZZ(tCurrentPoint, tNextPoint, oPolyline)
         End If

      Next
      '	Me.zzAddEntity(oPolyline.GetPoint2dAt(0), oPolyline, 0)
      ' 	Me.zzAddEntity(oPolyline.GetPoint2dAt(iVerticesUB), oPolyline, iVerticesUB)
      miPolylineCounter += 1
   End Sub
   Private Sub zzAddLineSegmentZZZ(ByVal tStartPoint2d As Point2d, ByVal tEndPoint2d As Point2d, ByVal oCurve As Curve)

   End Sub
   Public Sub AddSegment(ByVal oCurveSegment As TplnCurveSegment, ByVal bTempInsertPoint As Boolean)
      Dim lFromPoint As Long = 2L
      Dim lBaseWidth As Long = 2L
      Dim lWidth As Long = Convert.ToInt64(Math.Floor(lBaseWidth / Math.Cos(oCurveSegment.MainAngle)))
      Dim tStartPoint As Point2d = oCurveSegment.StartPoint
      Dim tEndPoint As Point2d = oCurveSegment.EndPoint
      Dim tVector As Vector2d = New Vector2d(tEndPoint.X - tStartPoint.X, tEndPoint.Y - tStartPoint.Y)
      Dim tMinPoint As Point2d = oCurveSegment.MinPoint
      Dim tMaxPoint As Point2d = oCurveSegment.MaxPoint
      Dim tMinKeyPoint As KeyPoint = New KeyPoint(tMinPoint, mbCoarse)
      Dim tMaxKeyPoint As KeyPoint = New KeyPoint(tMaxPoint, mbCoarse)
      If bTempInsertPoint Then
         AcadDocument.WriteMessage("AddSegment:" & tMinKeyPoint.CoarseAbsCoordinates & "-" & CStr(mbCoarse))
      End If

      Dim lX, lY As Long
      Dim lPX, lPY As Long

      Dim lDX, lDY As Long

      Dim tKeyPoint As KeyPoint

      Dim dK As Double
      Dim dX, dY As Double

      '	System.Windows.Forms.MessageBox.Show(CStr(bX), "23_120")
      dK = oCurveSegment.Kx
      '		Dim itestcolor As Integer = 0
      Dim bNearEnd As Boolean

      If oCurveSegment.DirX Then

         For lX = tMinKeyPoint.X + lFromPoint To tMaxKeyPoint.X - lFromPoint  'tMinKeyPoint.X + 4L			 '
            If (lX - tMinKeyPoint.X) <= lWidth OrElse (tMaxKeyPoint.X - lX) <= lWidth Then
               bNearEnd = True
            Else
               bNearEnd = False
            End If
            dX = KeyPoint.GetCoarseAbsX(lX)
            dY = oCurveSegment.GetY(dX)
            'lY = Convert.ToInt64(tMinKeyPoint.Y + dK * (lX - tMinKeyPoint.X))

            '	AcadTransaction.InsertPoint(New Point3d(dX, dY, 0.0), 5)
            '	tKeyPoint = New KeyPoint(lX, lY)
            '	AcadTransaction.InsertPoint(tKeyPoint.GetCenterPoint3d(), 2)
            lY = KeyPoint.GetCoarseY(dY)
            tKeyPoint = New KeyPoint(lX, lY)
            ''''''''''LAST''''''''''AcadTransaction.InsertPoint(tKeyPoint.GetCenterPoint3d(), 1)
            'AcadDocument.WriteMessage("CheckPoint:" & tKeyPoint.AbsCoordinates() & "** " & tKeyPoint.Coordinates() & ":" & CStr(MyBase.Count))

            For lDY = -lWidth To lWidth
               lPY = lY + lDY
               lPX = lX
               '	AcadDocument.WriteMessage("CheckPoint:" & CStr(lPX) & "'" & CStr(lPY))
               zzCheckLinePoint(lPX, lPY, oCurveSegment, mdLineTolerance, 3, bTempInsertPoint)
               If bNearEnd Then
                  lPX = lX - Convert.ToInt64(dK * lDY)
                  zzCheckLinePoint(lPX, lPY, oCurveSegment, mdLineTolerance, 3, bTempInsertPoint)
               End If
            Next
         Next
      Else
         For lY = tMinKeyPoint.Y + lFromPoint To tMaxKeyPoint.Y - lFromPoint
            If (lY - tMinKeyPoint.Y) <= lWidth OrElse (tMaxKeyPoint.Y - lY) <= lWidth Then
               bNearEnd = True
            Else
               bNearEnd = False
            End If
            dY = KeyPoint.GetCoarseAbsY(lY)

            dX = oCurveSegment.GetX(dY)
            'lY = Convert.ToInt64(tMinKeyPoint.Y + dK * (lX - tMinKeyPoint.X))

            '	AcadTransaction.InsertPoint(New Point3d(dX, dY, 0.0), 5)
            '	tKeyPoint = New KeyPoint(lX, lY)
            '	AcadTransaction.InsertPoint(tKeyPoint.GetCenterPoint3d(), 2)
            lX = KeyPoint.GetCoarseX(dX)
            tKeyPoint = New KeyPoint(lX, lY)
            ''''		AcadTransaction.InsertPoint(tKeyPoint.GetCenterPoint3d(), 1)
            'AcadDocument.WriteMessage("CheckPoint:" & tKeyPoint.AbsCoordinates() & "** " & tKeyPoint.Coordinates() & ":" & CStr(MyBase.Count))

            For lDX = -lWidth To lWidth
               lPX = lX + lDX
               lPY = lY
               '	AcadDocument.WriteMessage("CheckPoint:" & CStr(lPX) & "'" & CStr(lPY))
               zzCheckLinePoint(lPX, lPY, oCurveSegment, mdLineTolerance, 3, bTempInsertPoint)
               If bNearEnd Then
                  lPY = lY - Convert.ToInt64(dK * lDX)
                  zzCheckLinePoint(lPX, lPY, oCurveSegment, mdLineTolerance, 3, bTempInsertPoint)
               End If
            Next
         Next
      End If
      '		DMAcadExt.AcadDocument.WriteMessage(CStr(tVector.Angle * 180.0 / Math.PI) & ";" & CStr(oCurveSegment.DirX) & ";" & TPlnPoint.DispPoint(tStartPoint) & ";" & TPlnPoint.DispPoint(tEndPoint))
      '		DMAcadExt.AcadDocument.WriteMessage("-/- " & tMinKeyPoint.AbsCoordinates & ";" & tMaxKeyPoint.AbsCoordinates)
   End Sub
	Private Sub zzCheckLinePoint(ByVal lX As Long, ByVal lY As Long, ByVal oCurveSegment As TplnCurveSegment, ByVal dLineTolerance As Double, ByVal itestcolor As Integer, ByVal bTempInsertPoint As Boolean)
		Dim tKeyPoint As KeyPoint
		tKeyPoint = New KeyPoint(lX, lY)
		If bTempInsertPoint Then
			AcadTransaction.InsertPoint(tKeyPoint.GetCoarseCenterPoint3d(), , itestcolor)
		End If

		If MyBase.ContainsKey(tKeyPoint.Code) Then
			Dim colPoints As Point2dCollection
			'	Dim oCurvePoint As CurvePoint
			Dim dDist As Double
			colPoints = MyBase.Item(tKeyPoint.Code).AllPoints
			For Each tPoint As Point2d In colPoints

				dDist = oCurveSegment.GetDistanceTo(tPoint)
				If dDist <= dLineTolerance Then
					moErrPointLineArray.Add(New TPlnPoint(tPoint))
					miSourceErrors += 1
					Dim oCircleMarkBlock As MarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Circle)
					oCircleMarkBlock.MarkPoint(tPoint, 6)
				End If
			Next

			'		DMAcadExt.AcadDocument.WriteMessage("$$$$ " & CStr(tKeyPoint.Code) & ":" & tKeyPoint.Coordinates & ":" & tKeyPoint.AbsCoordinates)
			DMAcadExt.AcadDocument.WriteDebugMessage("$$$$$ " & CStr(dDist) & "<=" & CStr(dLineTolerance) & ":" & tKeyPoint.AbsCoordinates)

			'	System.Windows.Forms.MessageBox.Show("OK!!!!!!!", "23_128")


		End If
	End Sub
	Public Sub AddBlockRef(ByVal oBlockRef As BlockReference, ByVal iPriority As Integer)
		Dim tCurvePoint As CurvePoint

		Try
			tCurvePoint = New CurvePoint(TPlnPoint.Point3dTo2d(oBlockRef.Position), oBlockRef.ObjectId, -1, False, iPriority)
			Me.zzAddEntity(TPlnPoint.Point3dTo2d(oBlockRef.Position), oBlockRef.ObjectId, -1, False, iPriority)
		Catch oEx As Exception
			Return
		End Try
	End Sub
	Public Sub zzAddEntity(ByVal tPoint2d As Point2d, ByVal iAcObjID As ObjectId, ByVal iVertexIndex As Integer, ByVal bInner As Boolean, ByVal iPriority As Integer)
		Dim tCurvePoint As CurvePoint
		Dim oEntityCell As EntityCell
		Dim tKeyPoint As KeyPoint

		Try
			tCurvePoint = New CurvePoint(tPoint2d, iAcObjID, iVertexIndex, bInner, iPriority)
		Catch oEx As Exception
			Return
		End Try
		tKeyPoint = New KeyPoint(tPoint2d, mbCoarse)
		miTestPoints += 1

		If MyBase.ContainsKey(tKeyPoint.Code) Then
			oEntityCell = MyBase.Item(tKeyPoint.Code)
			'  DMAcadExt.AcadDocument.WriteMessage("^^^^^^^ Pts:" & oEntityCell.HasCurvePoints & ": Lines:" & oEntityCell.HasIntersectingLines & "; " & tPoint2d.ToString())
			'    oEntityCell.AddCurvePoint(tCurvePoint, oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2)
			oEntityCell.AddCurvePoint(tCurvePoint, False)

			'  DMAcadExt.AcadDocument.WriteMessage("After Pts:" & oEntityCell.HasCurvePoints & ": Lines:" & oEntityCell.HasIntersectingLines)


		Else
			'	DMAcadExt.AcadDocument.WriteMessage("-00:" & tKeyPoint.AbsCoordinates)
			oEntityCell = New EntityCell(tCurvePoint, mbCoarse)
			'DMAcadExt.AcadDocument.WriteMessage("!!Point:" & tKeyPoint.AbsCoordinates() & "@@@ " & tKeyPoint.Coordinates())
			MyBase.Add(tKeyPoint.Code, oEntityCell)
			'    DMAcadExt.AcadDocument.WriteMessage("zzAddEntity:" & CStr(tKeyPoint.Code) & "; Cnt=" & MyBase.Count)

		End If

	End Sub
	Public Sub zzAddEntity(ByVal tPoint2d As Point2d, ByVal oCurve As Curve, ByVal iVertexIndex As Integer, ByVal bInner As Boolean, ByVal bOverlay As Boolean)
      Dim tCurvePoint As CurvePoint
      Dim oEntityCell As EntityCell
      Dim tKeyPoint As KeyPoint
      If oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2 Then

         DMAcadExt.AcadDocument.WriteDebugMessage("&&*1 " & "I=" & CStr(iVertexIndex) & "," & oCurve.Handle.ToString() & " - " & TPlnPoint.DispPoint(tPoint2d))
      End If
      Try
         tCurvePoint = New CurvePoint(tPoint2d, oCurve.ObjectId, iVertexIndex, bInner, bOverlay)
      Catch oEx As Exception
         Return
      End Try
      tKeyPoint = New KeyPoint(tPoint2d, mbCoarse)
      miTestPoints += 1
      If oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2 Then
         DMAcadExt.AcadDocument.WriteMessage("&&*0.:" & oCurve.Handle.Value & ":" & CStr(tKeyPoint.Code) & " - " & TPlnPoint.DispPoint(tPoint2d) & "; ?? " & CStr(MyBase.ContainsKey(tKeyPoint.Code)))
      End If
      If MyBase.ContainsKey(tKeyPoint.Code) Then
         oEntityCell = MyBase.Item(tKeyPoint.Code)
         '  DMAcadExt.AcadDocument.WriteMessage("^^^^^^^ Pts:" & oEntityCell.HasCurvePoints & ": Lines:" & oEntityCell.HasIntersectingLines & "; " & tPoint2d.ToString())
         '    oEntityCell.AddCurvePoint(tCurvePoint, oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2)
         oEntityCell.AddCurvePoint(tCurvePoint, False)

         '  DMAcadExt.AcadDocument.WriteMessage("After Pts:" & oEntityCell.HasCurvePoints & ": Lines:" & oEntityCell.HasIntersectingLines)
         If oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2 Then
            DMAcadExt.AcadDocument.WriteMessage("&&+11:" & oCurve.Handle.Value & ":" & tKeyPoint.AbsCoordinates)
         End If

      Else
			'	DMAcadExt.AcadDocument.WriteMessage("-00:" & tKeyPoint.AbsCoordinates)
			oEntityCell = New EntityCell(tCurvePoint, mbCoarse)
			'DMAcadExt.AcadDocument.WriteMessage("!!Point:" & tKeyPoint.AbsCoordinates() & "@@@ " & tKeyPoint.Coordinates())
			MyBase.Add(tKeyPoint.Code, oEntityCell)
			'    DMAcadExt.AcadDocument.WriteMessage("zzAddEntity:" & CStr(tKeyPoint.Code) & "; Cnt=" & MyBase.Count)
			If oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue1 OrElse
				oCurve.Handle.Value = dmLineCleanup.miDebugHandleValue2 Then
				DMAcadExt.AcadDocument.WriteMessage("&&-00:" & oCurve.Handle.Value & ":" & tKeyPoint.AbsCoordinates)
			End If
		End If

	End Sub
   Private Function zzGetEntityCell(tKeyPoint As KeyPoint) As EntityCell
      Dim oEntityCell As EntityCell = Nothing
      ' DMAcadExt.AcadDocument.WriteMessage("-06NEW:" & tKeyPoint.X.ToString() & "," & tKeyPoint.Y.ToString())
      If Not MyBase.TryGetValue(tKeyPoint.Code, oEntityCell) Then

         '  DMAcadExt.AcadDocument.WriteMessage("-07NEW:" & tKeyPoint.X.ToString() & "," & tKeyPoint.Y.ToString())
         oEntityCell = New EntityCell(tKeyPoint, False, False)
         MyBase.Add(tKeyPoint.Code, oEntityCell)
      End If
      Return oEntityCell
   End Function
   Public Sub zzAddArcSegment(ByVal oSegment As DMAcadExt.TplnArc, ByVal tCurveAcObjID As ObjectId)
      Dim tStartKeyPoint As KeyPoint = New KeyPoint(oSegment.StartPoint, mbCoarse)
      Dim tEndKeyPoint As KeyPoint = New KeyPoint(oSegment.EndPoint, mbCoarse)

      Dim lStepX As Long = Math.Sign(tEndKeyPoint.X - tStartKeyPoint.X)
      Dim lStepY As Long = Math.Sign(tEndKeyPoint.Y - tStartKeyPoint.Y)
      
      Dim oaInnerPoints() As Point2d
      '    Dim dAngleStep As Double = mdArcStep / oSegment.Radius
      Dim dTolerance As Double = mdArcStep * 0.5 * 0.01 * 0.5
      Dim oStartKeyPoint As KeyPoint
      Dim oEndKeyPoint As KeyPoint

      Dim oPrevKeyPoint As KeyPoint

      Dim oCurrentKeyPoint As KeyPoint


      oaInnerPoints = oSegment.GetInnerPoints(dTolerance)
      '    System.Windows.Forms.MessageBox.Show(mdArcStep.ToString() & vbCrLf & dTolerance.ToString() & vbCrLf & oaInnerPoints.GetUpperBound(0).ToString(), "05_611")
      oStartKeyPoint = New KeyPoint(oSegment.StartPoint, False)
      oEndKeyPoint = New KeyPoint(oSegment.EndPoint, False)

      oPrevKeyPoint = oStartKeyPoint

      '    DMAcadExt.AcadDocument.WriteDebugMessage("-7Start:" & oSegment.StartPoint.ToString() & "; End" & oSegment.EndPoint.ToString() & "; CW" & oSegment.IsClockWise.ToString())
      For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
         oCurrentKeyPoint = New KeyPoint(oaInnerPoints(iPointIndex), False)
         If iPointIndex = 0 Or iPointIndex = oaInnerPoints.GetUpperBound(0) Then
            '  DMAcadExt.AcadDocument.WriteDebugMessage("-?InnerPts: " & iPointIndex.ToString() & " " & oaInnerPoints(iPointIndex).ToString()) '& "; " & oSegment.EndPoint.ToString()
         End If
         '   DMAcadExt.AcadDocument.WriteMessage("-Inner+: " & oPrevKeyPoint.ToString() & " <=> " & oCurrentKeyPoint.ToString()) '& "; " & oSegment.EndPoint.ToString()
         ''''''''''''''''''''''''''''''''''''''''''  zzAddIntersectingLineToCell(oPrevKeyPoint, oCurrentKeyPoint, tCurveAcObjID, oCurrentKeyPoint = oStartKeyPoint, False)
         zzAddIntersectingLineToCell(oPrevKeyPoint, oCurrentKeyPoint, tCurveAcObjID, oStartKeyPoint, oEndKeyPoint)


         oPrevKeyPoint = oCurrentKeyPoint
      Next
      oCurrentKeyPoint = New KeyPoint(oSegment.EndPoint, False)
      '  zzAddIntersectingLineToCell(oPrevKeyPoint, oCurrentKeyPoint, tCurveAcObjID, FALSE, TRUE)
      zzAddIntersectingLineToCell(oPrevKeyPoint, oCurrentKeyPoint, tCurveAcObjID, oStartKeyPoint, oEndKeyPoint)


      '   DMAcadExt.AcadDocument.WriteMessage("-00:" & oSegment.StartPoint.ToString() & "; " & oSegment.EndPoint.ToString())
      '    DMAcadExt.AcadDocument.WriteMessage("-01:" & tStartKeyPoint.X.ToString() & "; " & tEndKeyPoint.Y.ToString())

   End Sub
   Public Sub zzAddLineSegment(ByVal oSegment As DMAcadExt.TplnLine, ByVal tCurveAcObjID As ObjectId)
      Dim tStartKeyPoint As KeyPoint = New KeyPoint(oSegment.StartPoint, mbCoarse)
      Dim tEndKeyPoint As KeyPoint = New KeyPoint(oSegment.EndPoint, mbCoarse)
      Dim tCurrentKeyPoint As KeyPoint
      Dim lStepX As Long = Math.Sign(tEndKeyPoint.X - tStartKeyPoint.X)
      Dim lStepY As Long = Math.Sign(tEndKeyPoint.Y - tStartKeyPoint.Y)
      Dim lPrevY As Long
      Dim dRealX As Double
      Dim dRealY As Double

      '   DMAcadExt.AcadDocument.WriteMessage("-00:" & oSegment.StartPoint.ToString() & "; " & oSegment.EndPoint.ToString())
      '    DMAcadExt.AcadDocument.WriteMessage("-01:" & tStartKeyPoint.X.ToString() & "; " & tEndKeyPoint.Y.ToString())

      '  Dim lX As Long
      If lStepX = 0 Then

         If lStepY = 1 Then
            For lY As Long = tStartKeyPoint.Y + 1 To tEndKeyPoint.Y - 1 Step lStepY
               '  DMAcadExt.AcadDocument.WriteMessage("-011:" & tStartKeyPoint.X.ToString() & "|" & tStartKeyPoint.X.GetType().ToString() & "; " & lY.ToString() & "|" & lY.GetType().ToString())

               tCurrentKeyPoint = New KeyPoint(tStartKeyPoint.X, lY)

               '  DMAcadExt.AcadDocument.WriteMessage("-02:" & tStartKeyPoint.X.ToString() & "," & lY.ToString())
               '  DMAcadExt.AcadDocument.WriteMessage("-03:" & tCurrentKeyPoint.X.ToString() & "," & tCurrentKeyPoint.Y.ToString())

               zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
            Next
         ElseIf lStepY = -1 Then
            For lY As Long = tEndKeyPoint.Y - 1 To tStartKeyPoint.Y + 1 Step lStepY
               tCurrentKeyPoint = New KeyPoint(tStartKeyPoint.X, lY)
               zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
            Next
         End If
      ElseIf lStepY = 0 Then
         If lStepX = 1 Then


            For lX As Long = tStartKeyPoint.X + 1 To tEndKeyPoint.X - 1 Step lStepX

               tCurrentKeyPoint = New KeyPoint(lX, tStartKeyPoint.Y)

               zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
            Next
         ElseIf lStepX = -1 Then
            For lX As Long = tEndKeyPoint.X - 1 To tStartKeyPoint.X + 1 Step lStepX

               tCurrentKeyPoint = New KeyPoint(lX, tStartKeyPoint.Y)

               zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
            Next
         End If
      ElseIf lStepX = 1 Then



         '  DMAcadExt.AcadDocument.WriteMessage("- lStepX =+ 1 ")
         Dim lY As Long
         lPrevY = tStartKeyPoint.Y
         '   Dim lX_tag As Long
         For lX As Long = tStartKeyPoint.X To tEndKeyPoint.X Step lStepX
            '   lX_tag = lX
            dRealX = KeyPoint.GetRealX(lX + 1)
            If lX = tEndKeyPoint.X Then
               lY = tEndKeyPoint.Y
            Else
               dRealY = oSegment.GetYNew(dRealX)
               lY = KeyPoint.ToKeyPointY(dRealY)
            End If

            '     DMAcadExt.AcadDocument.WriteMessage("-lY:" & lPrevY.ToString() & "," & lY.ToString() & "; lX=" & lX.ToString())
            '     DMAcadExt.AcadDocument.WriteMessage("-Real: " & dRealX.ToString() & "," & dRealY.ToString())


            For lCol As Long = lPrevY To lY Step lStepY
               tCurrentKeyPoint = New KeyPoint(lX, lCol)
               If (lX <> tStartKeyPoint.X OrElse lCol <> tStartKeyPoint.Y) AndAlso (lX <> tEndKeyPoint.X OrElse lCol <> tEndKeyPoint.Y) Then
                  zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
               End If

            Next
            lPrevY = lY
         Next


      Else 'lStepX =- 1 
         ' DMAcadExt.AcadDocument.WriteMessage("-'lStepX =- 1 Start=" & tStartKeyPoint.X.ToString() & "," & tStartKeyPoint.Y.ToString())
         Dim lY As Long
         lPrevY = tStartKeyPoint.Y
         For lX As Long = tStartKeyPoint.X To tEndKeyPoint.X Step lStepX
            dRealX = KeyPoint.GetRealX(lX)
            If lX = tEndKeyPoint.X Then
               lY = tEndKeyPoint.Y
            Else
               dRealY = oSegment.GetYNew(dRealX)
               lY = KeyPoint.ToKeyPointY(dRealY)
            End If

            '   DMAcadExt.AcadDocument.WriteMessage("**lY:" & lPrevY.ToString() & "," & lY.ToString() & "; lX=" & lX.ToString())
            '  DMAcadExt.AcadDocument.WriteMessage("**Real: " & dRealX.ToString() & "," & dRealY.ToString())


            For lCol As Long = lPrevY To lY Step lStepY
               tCurrentKeyPoint = New KeyPoint(lX, lCol)
               If (lX <> tStartKeyPoint.X OrElse lCol <> tStartKeyPoint.Y) AndAlso (lX <> tEndKeyPoint.X OrElse lCol <> tEndKeyPoint.Y) Then
                  zzAddIntersectingLineToCell(tCurrentKeyPoint, tCurveAcObjID)
               End If

            Next
            lPrevY = lY
         Next
      End If


   End Sub
   Public Sub zzAddIntersectingLineToCellOld(tKeyPoint1 As KeyPoint, tKeyPoint2 As KeyPoint, ByVal tCurveAcObjID As ObjectId)
      Dim lXMin, lXMax As Long
      Dim lYMin, lYMax As Long
      If tKeyPoint1.X <= tKeyPoint2.X Then
         lXMin = tKeyPoint1.X
         lXMax = tKeyPoint2.X
      Else
         lXMin = tKeyPoint2.X
         lXMax = tKeyPoint1.X
      End If

      If tKeyPoint1.Y <= tKeyPoint2.Y Then
         lYMin = tKeyPoint1.Y
         lYMax = tKeyPoint2.Y
      Else
         lYMin = tKeyPoint2.Y
         lYMax = tKeyPoint1.Y
      End If
      Dim tKeyPoint As KeyPoint
      For lX As Long = lXMin To lXMax
         For lY As Long = lYMin To lYMax
            tKeyPoint = New KeyPoint(lX, lY)
            DMAcadExt.AcadDocument.WriteMessage("-Cells: " & tKeyPoint.ToString()) '& "; " & oSegment.EndPoint.ToString()
            zzAddIntersectingLineToCell(tKeyPoint, tCurveAcObjID)
         Next
      Next

   End Sub
   Public Sub zzAddIntersectingLineToCell(tKeyPoint1 As KeyPoint, tKeyPoint2 As KeyPoint, ByVal tCurveAcObjID As ObjectId, bFirst As Boolean, bLast As Boolean)
      'Dim lXMin, lXMax As Long
      'Dim lYMin, lYMax As Long
      'If tKeyPoint1.X <= tKeyPoint2.X Then
      '   lXMin = tKeyPoint1.X
      '   lXMax = tKeyPoint2.X
      'Else
      '   lXMin = tKeyPoint2.X
      '   lXMax = tKeyPoint1.X
      'End If

      'If tKeyPoint1.Y <= tKeyPoint2.Y Then
      '   lYMin = tKeyPoint1.Y
      '   lYMax = tKeyPoint2.Y
      'Else
      '   lYMin = tKeyPoint2.Y
      '   lYMax = tKeyPoint1.Y
      'End If
      Dim tKeyPoint As KeyPoint
      For lX As Long = tKeyPoint1.X To tKeyPoint2.X Step zzGetStep(tKeyPoint1.X, tKeyPoint2.X)
         For lY As Long = tKeyPoint1.Y To tKeyPoint2.Y Step zzGetStep(tKeyPoint1.Y, tKeyPoint2.Y)

            If (Not bFirst OrElse lX <> tKeyPoint1.X OrElse lY <> tKeyPoint1.Y) AndAlso (Not bLast OrElse lX <> tKeyPoint2.X OrElse lY <> tKeyPoint2.Y) Then
               tKeyPoint = New KeyPoint(lX, lY)
               ' DMAcadExt.AcadDocument.WriteMessage("+Cells: " & tKeyPoint.ToString() & "; " & bFirst.ToString() & "; " & bLast.ToString()) '& "; " & oSegment.EndPoint.ToString()
               zzAddIntersectingLineToCell(tKeyPoint, tCurveAcObjID)
               'Else
               '   tKeyPoint = New KeyPoint(lX, lY)
               '   DMAcadExt.AcadDocument.WriteMessage("-!#############Cells: " & tKeyPoint.ToString() & "; " & bFirst.ToString() & "; " & bLast.ToString())


            End If
            
         Next
      Next

   End Sub

   Public Sub zzAddIntersectingLineToCell(tKeyPoint1 As KeyPoint, tKeyPoint2 As KeyPoint, ByVal tCurveAcObjID As ObjectId, tFirstPoint As KeyPoint, tLastPoint As KeyPoint)
      'Dim lXMin, lXMax As Long
      'Dim lYMin, lYMax As Long
      'If tKeyPoint1.X <= tKeyPoint2.X Then
      '   lXMin = tKeyPoint1.X
      '   lXMax = tKeyPoint2.X
      'Else
      '   lXMin = tKeyPoint2.X
      '   lXMax = tKeyPoint1.X
      'End If

      'If tKeyPoint1.Y <= tKeyPoint2.Y Then
      '   lYMin = tKeyPoint1.Y
      '   lYMax = tKeyPoint2.Y
      'Else
      '   lYMin = tKeyPoint2.Y
      '   lYMax = tKeyPoint1.Y
      'End If
      Dim tKeyPoint As KeyPoint
      For lX As Long = tKeyPoint1.X To tKeyPoint2.X Step zzGetStep(tKeyPoint1.X, tKeyPoint2.X)
         For lY As Long = tKeyPoint1.Y To tKeyPoint2.Y Step zzGetStep(tKeyPoint1.Y, tKeyPoint2.Y)
            tKeyPoint = New KeyPoint(lX, lY)
            If (tKeyPoint <> tFirstPoint) AndAlso (tKeyPoint <> tLastPoint) Then

               '  DMAcadExt.AcadDocument.WriteMessage("+Cells: " & tKeyPoint.ToString() & "; " & "; ")
               zzAddIntersectingLineToCell(tKeyPoint, tCurveAcObjID)
            Else
               ' tKeyPoint = New KeyPoint(lX, lY)
               '  DMAcadExt.AcadDocument.WriteMessage("-!#############Cells: " & tKeyPoint.ToString() & "; ")


            End If

         Next
      Next

   End Sub
   Public Sub zzAddIntersectingLineToCell(tMinKeyPoint As KeyPoint, ByVal tCurveAcObjID As ObjectId)
      Dim oEntityCell As EntityCell = zzGetEntityCell(tMinKeyPoint)
      Dim oNeigborEntityCell As EntityCell

      oEntityCell.AddIntersectingLine(tCurveAcObjID)
      Return
      Dim oEnum As IEnumerator(Of KeyPoint) = oEntityCell.GetNeighborEnumerator()
      Dim tKeyPoint As KeyPoint
      oEnum.Reset()
      Do While oEnum.MoveNext
         tKeyPoint = oEnum.Current
         oNeigborEntityCell = zzGetEntityCell(tKeyPoint)
         oNeigborEntityCell.AddIntersectingLine(tCurveAcObjID)
      Loop
      
   End Sub
   Private Function zzGetStep(lVal1 As Long, lVal2 As Long) As Long
      If lVal1 <= lVal2 Then
         Return 1L
      Else
         Return -1L
      End If
   End Function
   Private Function zzPoint3dTo2d(ByVal tPoint3d As Autodesk.AutoCAD.Geometry.Point3d) As Autodesk.AutoCAD.Geometry.Point2d
      Return New Autodesk.AutoCAD.Geometry.Point2d(tPoint3d.X, tPoint3d.Y)
   End Function
   Private Class PointComparerAAA
      Implements System.Collections.Generic.IComparer(Of KeyPoint)

      Public Function Compare(ByVal tPointA As KeyPoint, ByVal tPointB As KeyPoint) As Integer Implements System.Collections.Generic.IComparer(Of KeyPoint).Compare
         Const bLess As Integer = -1
         Const bMore As Integer = 1
         Const bEq As Integer = 0
         If tPointA.X = tPointB.X AndAlso tPointA.Y = tPointB.Y Then
            Return bEq
         ElseIf tPointA.Sum < tPointB.Sum Then
            Return bLess
         ElseIf tPointA.Sum > tPointB.Sum Then
            Return bMore
         ElseIf tPointA.X < tPointB.X Then
            Return bLess
         Else
            Return bMore
         End If
      End Function
   End Class

   Public Sub New(ByVal bCoarse As Boolean)
      '	MyBase.New(New PointComparer())
      mbCoarse = bCoarse
      moErrPointLineArray = New TplnPointArray()
      miSourceErrors = 0
   End Sub
   Public Sub New(ByVal bCoarse As Boolean, ByVal dLineTolerance As Double)
      '	MyBase.New(New PointComparer())
      Me.New(bCoarse)
      mdLineTolerance = dLineTolerance
      miSourceErrors = 0
   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub
End Class
 
