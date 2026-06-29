Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme
    Public Enum enRingDirection
        NotDefined = 0
        CounterClockwise = -1
        Clockwise = 1
    End Enum


   Public Class tsRing
      Const miTestPgonID As Integer = 165000
      Const mbTestIsExterior As Boolean = True

      Public Const mdAngleTolerance As Double = 0.002
		Public Const mdDistanceTolerance As Double = 0.001
		Public Const mdAreaTolerance As Double = 0.2

		Private miPolygonID As Integer
		' Private miInnerPolygonID As Integer
		Private mhsInnerPolygons As HashSet(Of Integer) = New HashSet(Of Integer)()
		Private miRingNumber As Integer = 0
      Private miElementsUB As Integer
      Private maNodes() As tsNode
      Private moaBranches() As tsBranch
      Private miRefNewNode As Integer
      Private miRefNewBranch As Integer
      Private mdicElements As tsElements
      Private miDirection As enRingDirection = enRingDirection.NotDefined
      '   Private mbPositiveDirection As Boolean = True
      Private mbIsExterior As Boolean
      Private mhsBranches As HashSet(Of Integer)
      Private mhsNodes As HashSet(Of Integer)
      Private mhsIsthmuses As HashSet(Of Integer)
      '  Private mhsNodeBranches As HashSet(Of Integer)
      Private moBulgeVertexArray As GeoUtilites.BulgeVertexArray
      Private miBranchesUB As Integer
      Private moaLinks() As DMAcadExt.IUD_Link
      Private mcolLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
      Private mcolNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
      Private mcolNodeLinksCorrected As System.Collections.ObjectModel.Collection(Of NodeLink)
      Private mdAreaCorrected As Double
      Private mtBorderObjID As ObjectId
      Private mtBorderHandle As Handle

      Private mbBranchOrder As Boolean
      '	Private mdicNodes As Dictionary(Of Integer, Node)
      '	Private mdicBranches As Dictionary(Of Integer, Branch)
      Public Shared Function IsSameDirection(iDirection1 As enRingDirection, iDirection2 As enRingDirection) As Boolean

         If iDirection1 = enRingDirection.NotDefined OrElse iDirection2 = enRingDirection.NotDefined Then
            Return True
         Else
            Return iDirection1 = iDirection2
         End If

      End Function

      Public Sub New(ByVal iElementsCount As Integer)
         miElementsUB = iElementsCount - 1
         ReDim maNodes(miElementsUB)
         ReDim moaBranches(miElementsUB)

		End Sub
      Public Sub New(iPolygonID As Integer, ByRef dicElements As tsElements)
         miPolygonID = iPolygonID
         mdicElements = dicElements
      End Sub


		Public Sub Load(ByVal oRing As Ring, iInteriorRingIndex As Integer)

			Dim colHalfEdges As HalfEdgeCollection = Nothing
			'   Dim colNodes As NodeCollection = New NodeCollection()
			Dim hsBranches As HashSet(Of Integer)
			'  Dim oFullEdge As FullEdge
			Dim oPreviousHalfEdge As HalfEdge = Nothing
			Dim oPrevBranch As tsBranch = Nothing
			Dim oStartBranch As tsBranch = Nothing
			Dim oBranch As tsBranch
			Dim oElement As tsElement = Nothing
			Dim oNode As Node
			Dim oNodeScheme As tsNode = Nothing
			Dim bIsBranchSameDir As Boolean
			Dim iPrevNodeID As Integer


			'     Dim tPrevNodeLink As NodeLink = Nothing
			Dim iDir As enRingDirection
			Dim bSameDir As Boolean
			Dim bPrevSameDir As Boolean

			'  Dim oLastHalfEdge As HalfEdge


			mhsNodes = New HashSet(Of Integer)
			hsBranches = New HashSet(Of Integer)
			mhsIsthmuses = New HashSet(Of Integer)
			mbIsExterior = oRing.IsExterior

			If mbIsExterior Then
				miRingNumber = 0
			Else
				miRingNumber = iInteriorRingIndex
			End If
			'     DMAcadExt.AcadDocument.WriteMessage("!!@@TopoName: " & mdicElements.TopoName & " Pgon=" & CStr(miPolygonID) & " Ring: " & CStr(miRingNumber))

			colHalfEdges = oRing.GetEdges()



			If colHalfEdges IsNot Nothing Then

				miBranchesUB = colHalfEdges.Count - 1
				'    oPreviousHalfEdge = colHalfEdges.Item(miBranchesUB)

				For Each oHalfEdge As HalfEdge In colHalfEdges

					oBranch = mdicElements.TryGetBranch(oHalfEdge, miRingNumber)
					If oStartBranch Is Nothing Then
						oStartBranch = oBranch
					End If
					'	DMAcadExt.AcadDocument.WriteDebugMessage("Load Ring " & miRingNumber.ToString() & "; " & miPolygonID.ToString & "; " & oBranch.LeftPolygon.ToString & "; " & oBranch.RightPolygon.ToString() & "; " & mhsInnerPolygons.Count)
					If oBranch.LeftPolygon = miPolygonID Then
						iDir = enRingDirection.CounterClockwise
						If Not mbIsExterior AndAlso Not mhsInnerPolygons.Contains(oBranch.RightPolygon) Then
							'miInnerPolygonID = oBranch.RightPolygon
							'	iInnerPolygonID As Integer
							mhsInnerPolygons.Add(oBranch.RightPolygon)
						End If
					ElseIf oBranch.RightPolygon = miPolygonID Then
						iDir = enRingDirection.Clockwise
						If Not mbIsExterior AndAlso Not mhsInnerPolygons.Contains(oBranch.LeftPolygon) Then
							'miInnerPolygonID = oBranch.LeftPolygon
							mhsInnerPolygons.Add(oBranch.LeftPolygon)
						End If
					Else
						DMAcadExt.AcadDocument.WriteMessage("DesignErr_#137: Left: " & CStr(oBranch.LeftPolygon) & " Right: " & oBranch.RightPolygon)
						iDir = enRingDirection.NotDefined
					End If

					bSameDir = True
					If miDirection = enRingDirection.NotDefined Then
						miDirection = iDir
					ElseIf miDirection <> iDir Then
						If iDir = enRingDirection.CounterClockwise Then
							oBranch.LeftDirection = False
						ElseIf iDir = enRingDirection.Clockwise Then
							oBranch.RightDirection = False
						End If

						bSameDir = False
					End If
					If oPreviousHalfEdge IsNot Nothing Then
						' zzSetCheckDirNew(oPreviousHalfEdge, oHalfEdge, bSameDir)
						zzCheckOrder(oPrevBranch, oBranch, bPrevSameDir, bSameDir)
					End If

					hsBranches.Add(oBranch.ID)

					oNode = oHalfEdge.PreviousNode
					oNodeScheme = mdicElements.TryAddNodeScheme(oNode, iInteriorRingIndex)
					mhsNodes.Add(oNode.ID)

					If mbBranchOrder Then
						iPrevNodeID = oBranch.GetPreviousNode(miPolygonID, mbIsExterior)
					Else
						iPrevNodeID = oBranch.GetNextNode(miPolygonID, mbIsExterior)
						bIsBranchSameDir = Not bIsBranchSameDir
					End If

					oPreviousHalfEdge = oHalfEdge
					oPrevBranch = oBranch
					bPrevSameDir = bSameDir

				Next

				zzCheckOrder(oPrevBranch, oStartBranch, bPrevSameDir, True)

				If mbBranchOrder Then
					mhsBranches = hsBranches
				Else
					Dim iaBranches(hsBranches.Count - 1) As Integer
					hsBranches.CopyTo(iaBranches)
					mhsBranches = New HashSet(Of Integer)
					For iIndex As Integer = hsBranches.Count - 1 To 0 Step -1
						mhsBranches.Add(iaBranches(iIndex))
					Next
				End If
				If tsTopology.MapObjectsDispose Then
					colHalfEdges.Dispose()
				End If

			End If
		End Sub
		'  tsRing.vb:line 691
		Public Sub CalcExterior()
			Dim oPreviousHalfEdge As HalfEdge = Nothing
			Dim oPrevBranch As tsBranch = Nothing
         Dim oStartBranch As tsBranch = Nothing
         Dim oBranch As tsBranch
         Dim oElement As tsElement = Nothing

         Dim oNodeScheme As tsNode = Nothing
         Dim bIsBranchSameDir As Boolean
         Dim iPrevNodeID As Integer

         Dim tNodeLink As NodeLink = Nothing
         Dim tPrevNodeLink As NodeLink = Nothing
			Dim oLink As DMAcadExt.IUD_Link = Nothing

			Dim dTotalAngleMax As Double = 6.0
			mcolNodeLinks = New System.Collections.ObjectModel.Collection(Of NodeLink)

			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			If mhsBranches IsNot Nothing Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "mhsBranches", miPolygonID, mhsBranches.Count)
				For Each iBranchID As Integer In mhsBranches
               oBranch = mdicElements.GetBranch(iBranchID)
               bIsBranchSameDir = GetCurveDirection(oBranch)
               oLink = tsBranch.GetLink(oBranch.AcObjID, bIsBranchSameDir)   '   mmmm   mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
					If oLink IsNot Nothing Then
						DMCommon.Debug.ExcelLog.SetNextValue(0, "oLink IsNot Nothing", miPolygonID, mbIsExterior)
						If mbIsExterior Then
							If bIsBranchSameDir Then
								iPrevNodeID = oBranch.PreviousNodeID
							Else
								iPrevNodeID = oBranch.NextNodeID
							End If
						Else
							If bIsBranchSameDir Then
								iPrevNodeID = oBranch.NextNodeID
							Else
								iPrevNodeID = oBranch.PreviousNodeID
							End If
						End If

						tNodeLink = New NodeLink(iPrevNodeID, oBranch.ID, bIsBranchSameDir, oLink)
						'  iNextNodeID = oBranch.GetNextNode(miPolygonID, mbIsExterior)
						mcolNodeLinks.Add(tNodeLink)

						tPrevNodeLink = tNodeLink
					Else
						DMCommon.Debug.ExcelLog.SetNextValue(0, "oLink Is Nothing", miPolygonID)
					End If
            Next
            If mcolNodeLinks.Count > 0 Then
               tNodeLink = mcolNodeLinks.First()
            End If
         End If

		End Sub
      Public Sub CheckExtend()
			'   Const iTestPgonID As Integer = 810000

			Dim oPreviousHalfEdge As HalfEdge = Nothing
         Dim oPrevBranch As tsBranch = Nothing
         Dim oStartBranch As tsBranch = Nothing
         Dim oBranch As tsBranch
         Dim oElement As tsElement = Nothing

			Dim oNodeScheme As tsNode = Nothing
			Dim oNodeSchemeForCheckAngle As tsNode = Nothing

			Dim bIsBranchSameDir As Boolean
			Dim iPrevNodeID As Integer
			Dim iNextNodeID As Integer
			Dim tNodeLink As NodeLink = Nothing
			Dim tPrevNodeLink As NodeLink = Nothing

         Dim oLink As DMAcadExt.IUD_Link = Nothing
			'Dim sX As String
			Dim dTotalAngle As Double
			Dim iSharedNodeID As Integer
			Dim dTotalAngleMax As Double = 6.10865
			Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
			' Dim bCountCond As Boolean = mhsBranches.Count > 2
			mcolNodeLinks = New System.Collections.ObjectModel.Collection(Of NodeLink)
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


			If mhsBranches IsNot Nothing Then
				If miPolygonID = 1 Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!Err03", miPolygonID, miRingNumber, mhsBranches.Count)
				End If

				'	DMCommon.Debug.MsgBox("07_501KB", mhsBranches.Count)

				For Each iBranchID As Integer In mhsBranches
					oBranch = mdicElements.GetBranch(iBranchID)
					bIsBranchSameDir = GetCurveDirection(oBranch)
					oLink = tsBranch.GetLink(oBranch.AcObjID, bIsBranchSameDir)   '   

					If oLink IsNot Nothing Then
						If mbIsExterior Then
							If bIsBranchSameDir Then
								iPrevNodeID = oBranch.PreviousNodeID
								iNextNodeID = oBranch.NextNodeID
							Else
								iPrevNodeID = oBranch.NextNodeID
								iNextNodeID = oBranch.PreviousNodeID
							End If
						Else
							If bIsBranchSameDir Then
								iPrevNodeID = oBranch.NextNodeID
								iNextNodeID = oBranch.PreviousNodeID
							Else
								iPrevNodeID = oBranch.PreviousNodeID
								iNextNodeID = oBranch.NextNodeID
							End If
						End If

						tNodeLink = New NodeLink(iPrevNodeID, oBranch.ID, bIsBranchSameDir, oLink, iNextNodeID)
						mcolNodeLinks.Add(tNodeLink)
						If tPrevNodeLink.Link IsNot Nothing Then

							If tPrevNodeLink.Link.IsExtend(tNodeLink.Link, tTolerance) Then
								iSharedNodeID = tPrevNodeLink.GetSharedID(tNodeLink.NodeID, tNodeLink.NextNodeID)
								If iSharedNodeID <> 0 Then
									oNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
									If tPrevNodeLink.Link.IsArc Then
										If dTotalAngle = 0.0 Then
											dTotalAngle = tPrevNodeLink.Link.TotalAngle
										End If
										dTotalAngle += tNodeLink.Link.TotalAngle
										If dTotalAngle < dTotalAngleMax Then
											oNodeScheme.IsPseudoGeo = True
										Else
											dTotalAngle = 0.0
										End If
									Else
										dTotalAngle = 0.0
										oNodeScheme.IsPseudoGeo = True
									End If
								Else
									DMCommon.Debug.MsgBox("13_136", tPrevNodeLink.NodeID, tPrevNodeLink.NextNodeID, tNodeLink.NodeID, tNodeLink.NextNodeID)
								End If
							End If
						Else

						End If

						tPrevNodeLink = tNodeLink
					Else
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!Err04", oBranch.AcObjID)
					End If
				Next
				If mcolNodeLinks.Count > 0 Then
					tNodeLink = mcolNodeLinks.First()
				End If
				'If miPolygonID = 69 Then
				'	DMCommon.Debug.ExcelLog.SetNextValue(4, "!Err01", tPrevNodeLink.Link Is Nothing, tNodeLink.Link Is Nothing, mcolNodeLinks.Count)
				'End If

				If mcolNodeLinks.Count > 0 Then

					If tPrevNodeLink.Link.IsExtend(tNodeLink.Link, tTolerance) Then
						iSharedNodeID = tPrevNodeLink.GetSharedID(tNodeLink.NodeID, tNodeLink.NextNodeID)
						If iSharedNodeID <> 0 Then
							oNodeScheme = mdicElements.GetNode(iSharedNodeID)
							If tPrevNodeLink.Link.IsArc Then
								If dTotalAngle = 0.0 Then
									dTotalAngle += tPrevNodeLink.Link.TotalAngle
								End If
								dTotalAngle += tNodeLink.Link.TotalAngle
								For iIndex As Integer = 1 To mcolNodeLinks.Count - 1
									tNodeLink = mcolNodeLinks.Item(iIndex)
									oNodeSchemeForCheckAngle = mdicElements.GetNode(tNodeLink.NodeID)
									If oNodeSchemeForCheckAngle.IsPseudoGeo Then
										dTotalAngle += tNodeLink.Link.TotalAngle
									Else
										Exit For
									End If
								Next

								If dTotalAngle < dTotalAngleMax Then
									oNodeScheme.IsPseudoGeo = True
								End If

							Else
								dTotalAngle = 0.0
								oNodeScheme.IsPseudoGeo = True
							End If
						Else
							DMCommon.Debug.MsgBox("13_136d", tPrevNodeLink.NodeID, tPrevNodeLink.NextNodeID, tNodeLink.NodeID, tNodeLink.NextNodeID)
						End If

					End If 'tPrevNodeLink.Link.IsExtend
				End If

			End If

			'   DMAcadExt.AcadDocument.WriteMessage("Inf09 #" & miPolygonID.ToString() & "/" & Str(miRingNumber) & "  Dir= " & miDirection.ToString())
		End Sub
      Public ReadOnly Property Isthmuses As HashSet(Of Integer)
         Get
            Return mhsIsthmuses
         End Get
      End Property
      Public Function ContainsNode(iNodeID As Integer) As Boolean
         Return mhsNodes.Contains(iNodeID)
      End Function
      Public Sub AddIsthmus(iBranchID As Integer)
         mhsIsthmuses.Add(iBranchID)
      End Sub
      Public Function GetCurveDirection(oBranch As tsBranch) As Boolean
         Dim bIsBranchSameDir As Boolean
         If miDirection = enRingDirection.NotDefined Then
            bIsBranchSameDir = True
         ElseIf miDirection = oBranch.GetDirection(miPolygonID, mbIsExterior) Then

            bIsBranchSameDir = True

         Else

            bIsBranchSameDir = False

         End If
         Return bIsBranchSameDir
      End Function
      Public Function GetVectorSet() As DMAcadExt.IUD_Link()
			' Dim colPoints As System.Collections.ObjectModel.Collection(Of Integer)

			Dim oBranch As tsBranch
         Dim bIsBranchSameDir As Boolean
         Dim oLink As DMAcadExt.IUD_Link
         Dim oaLinks(mhsBranches.Count - 1) As DMAcadExt.IUD_Link
         Dim oResBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
         '  MessageBox.Show(CStr(hsBranches.Count), "04_588")
         Dim iIndex As Integer = 0
         For Each iBranchID As Integer In mhsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
            oLink = tsBranch.GetLink(oBranch.AcObjID, bIsBranchSameDir)
            oaLinks(iIndex) = oLink
            iIndex += 1
         Next
         Return oaLinks


      End Function
      Private Function zzGetNodeLinksReverse() As System.Collections.ObjectModel.Collection(Of NodeLink)
			Dim colRes As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()
			Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = zzCurrentNodeLinks()
			Dim iNodeID As Integer = colNodeLinks.First.NodeID
			Dim iUB As Integer = colNodeLinks.Count - 1
			Dim tNodeLink As NodeLink
         Dim tNewNodeLink As NodeLink
         Dim oLink As DMAcadExt.IUD_Link
			For iIndex As Integer = iUB To 0 Step -1
				tNodeLink = colNodeLinks.ElementAt(iIndex)
				oLink = tNodeLink.Link
				'    oLink.Reverse()
				tNewNodeLink = New NodeLink(iNodeID, tNodeLink.BranchID, Not tNodeLink.LinkSameDir, oLink)
				iNodeID = tNodeLink.NodeID
				colRes.Add(tNewNodeLink)
			Next
			Return colRes
		End Function
      Private Function zzGetNodeLinks() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim colRes As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()

			'  Dim tNodeLink As NodeLink
			Dim tNewNodeLink As NodeLink
			Dim oLink As DMAcadExt.IUD_Link
			Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = zzCurrentNodeLinks()

			'zzColNodeLinksToExcel(colNodeLinks, "ComplexI")
			'zzColNodeLinksToExcel(mcolNodeLinks, "ComplexI Src")
			'zzColNodeLinksToExcel(mcolNodeLinksCorrected, "ComplexI Crct")


			For Each tNodeLink As NodeLink In colNodeLinks
				oLink = tNodeLink.Link
				'zzLinkToExcel(oLink, "Inner")


				oLink.Reverse()
				tNewNodeLink = New NodeLink(tNodeLink.NodeID, tNodeLink.BranchID, tNodeLink.LinkSameDir, oLink)

				colRes.Add(tNewNodeLink)
			Next
			'zzColNodeLinksToExcel(colRes, "Complex2")
			Return colRes
      End Function
		Private Sub zzLinkToExcel(oLink As DMAcadExt.IUD_Link, sCaption As String)
		End Sub
		Function GetCurveDirection030915(oBranch As tsBranch) As Boolean
         Dim bIsBranchSameDir As Boolean
         If miDirection = enRingDirection.NotDefined Then
            bIsBranchSameDir = True
         ElseIf miDirection = oBranch.GetDirection(miPolygonID, mbIsExterior) Then
            If mbIsExterior Then
               bIsBranchSameDir = True
            Else
               bIsBranchSameDir = False
            End If
         Else
            If mbIsExterior Then
               bIsBranchSameDir = False
            Else
               bIsBranchSameDir = True
            End If
         End If
         Return bIsBranchSameDir
      End Function



		Private Sub zzAddCurve(ByVal tAcObjID As ObjectId, ByVal bSameDirection As Boolean, bJoin As Boolean)
         Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tAcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         Dim oLine As Line
         Dim oArc As Arc

         Dim oPrevLink As DMAcadExt.IUD_Link = Nothing
         Dim iCount As Integer = mcolLinks.Count
         Dim oTplnLine As DMAcadExt.TplnLine
         Dim oTplnArc As DMAcadExt.TplnArc

         If iCount > 0 Then
            oPrevLink = mcolLinks.Item(iCount - 1)
         End If

         Select Case oCurve.GetRXClass().Name
            Case DMAcadExt.AcadConst.AcadLineName
               oLine = DirectCast(oCurve, Line)
               oTplnLine = New DMAcadExt.TplnLine(oLine, bSameDirection)
               If bJoin Then
                  oPrevLink.Extend(oTplnLine)
               Else
                  mcolLinks.Add(oTplnLine)
               End If



            Case DMAcadExt.AcadConst.AcadArcName
               oArc = DirectCast(oCurve, Arc)
               oTplnArc = New DMAcadExt.TplnArc(oArc, True)
               If bJoin Then
                  oPrevLink.Extend(oTplnArc)
               Else
                  mcolLinks.Add(oTplnArc)
               End If

            Case Else
               DMAcadExt.AcadDocument.WriteMessageLog("!!!Curve Type:" & oCurve.GetType().ToString() & "!")
               Return
               '	MessageBox.Show(oCurve.GetType().ToString(), "12_073")
         End Select
      End Sub
      Private Function zzGetLineName(sStartPointName As String, sEndPointName As String) As String
         If String.IsNullOrEmpty(sStartPointName) Then
            If String.IsNullOrEmpty(sEndPointName) Then
               Return String.Empty
            Else
               Return sEndPointName
            End If
         Else
            If String.IsNullOrEmpty(sEndPointName) Then
               Return sStartPointName
            Else
               Return sStartPointName & "," & sEndPointName
            End If
         End If
      End Function
  

      Public Function GetAllLinkEntities() As ObjectIdCollection
         Dim oBranch As tsBranch
         Dim colResEntities As ObjectIdCollection = New ObjectIdCollection()
         For Each iBranchID As Integer In mhsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
            colResEntities.Add(oBranch.AcObjID)
         Next
         Return colResEntities
      End Function
      Public Sub CheckPointCount()
         Const iMinPointCount As Integer = 3
         Dim bTestCond As Boolean = (miPolygonID = 150) Or ((miPolygonID = 149) And Not mbIsExterior)
         Dim oNodeScheme As tsNode
         Dim iIsNotPseudoCount As Integer = 0
         bTestCond = False
         For Each tNodeLink As NodeLink In mcolNodeLinks
            oNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
            If bTestCond Then
               DMAcadExt.AcadDocument.WriteMessage(" Node!!=" & oNodeScheme.IsPseudo.ToString() & "; " & oNodeScheme.TestPseudoString())
            End If
            If Not oNodeScheme.IsPseudo Then
               iIsNotPseudoCount += 1
            End If
         Next
         If bTestCond Then
            DMAcadExt.AcadDocument.WriteMessage("!!IsNotPseudoCount=" & iIsNotPseudoCount.ToString() & "; " & "")
         End If
         If iIsNotPseudoCount < iMinPointCount Then
            For Each tNodeLink As NodeLink In mcolNodeLinks
               oNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
               If bTestCond Then
                  DMAcadExt.AcadDocument.WriteMessage("+???  !!IsNotPseudoCount=" & iIsNotPseudoCount.ToString() & "; " & oNodeScheme.IsPseudo.ToString())
               End If
               If oNodeScheme.IsPseudo Then
                  oNodeScheme.IsBlockingTri = True
                  iIsNotPseudoCount += 1
                  If bTestCond Then
                     DMAcadExt.AcadDocument.WriteMessage("+1  !!IsNotPseudoCount=" & iIsNotPseudoCount.ToString() & "; " & oNodeScheme.IsPseudo.ToString())
                  End If
                  If iIsNotPseudoCount = iMinPointCount Then
                     Exit For
                  End If
               End If
            Next
         End If
      End Sub
      Public Sub SetCorrect()
         mcolNodeLinksCorrected = mcolNodeLinks
      End Sub

      'Act
      Public Sub RemovePseudoPoints()
         Dim iUB As Integer = mcolNodeLinks.Count - 1
         Dim iEndIndex As Integer = -1
			Dim tPrevLink As NodeLink = Nothing
			Dim oNodeScheme As tsNode = Nothing
			Dim oStartNodeScheme As tsNode = Nothing
			Dim sTest As String
			mcolNodeLinksCorrected = New System.Collections.ObjectModel.Collection(Of NodeLink)
			Try
				For Each tNodeLink As NodeLink In mcolNodeLinks

					If tPrevLink.Link Is Nothing Then
						tPrevLink = tNodeLink.Copy()
						oStartNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
						sTest = "First"
					Else
						oNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
						sTest = oNodeScheme.IsSkipVertex.ToString()
						If oNodeScheme.IsSkipVertex Then
							tPrevLink.Link.Extend(tNodeLink.Link)
						Else
							mcolNodeLinksCorrected.Add(tPrevLink)
							tPrevLink = tNodeLink.Copy()
						End If
					End If
				Next

				'''''''''''''''''''''''
				If oStartNodeScheme.IsSkipVertex Then
					tPrevLink.Link.Extend(mcolNodeLinksCorrected.First.Link)
					mcolNodeLinksCorrected.RemoveAt(0)
				End If
				mcolNodeLinksCorrected.Add(tPrevLink)
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("Err #924", oEx.Message, oEx.GetType())
			End Try

			If miPolygonID = 379 Then
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!PFinalLineCorrect", mdicElements.TopoName, miPolygonID, mcolNodeLinks.Count, mcolNodeLinksCorrected, " 379 379 ")
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Captions", mdicElements.TopoName, miPolygonID, "StartPoint", "EndPoint", "HasUsedPoint", "HasActiveSurveyPoint", "HasOldPoint", "IsSkipVertex", "IsPseudoGeo", "IsPseudoTopo")
				For Each oNodeLink As NodeLink In mcolNodeLinks
					oNodeScheme = mdicElements.GetNode(oNodeLink.NodeID)
					'	DMCommon.Debug.ExcelLog.SetNextValue(2, "!NodeLink", oNodeLink.Link.StartPoint.ToString(), oNodeLink.Link.EndPoint.ToString(), oNodeScheme.HasUsedPoint, oNodeScheme.HasActiveSurveyPoint _
					'		 , oNodeScheme.HasOldPoint, oNodeScheme.IsSkipVertex, oNodeScheme.IsPseudoGeo, oNodeScheme.IsPseudoTopo)

				Next


			End If
		End Sub




      'Act
      Public Function GetNodeLinks(iStartNodeID As Integer, iExteriorDirection As enRingDirection) As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim oNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = zzCurrentNodeLinks()
         Dim tNodeLink As NodeLink
         Dim iUB As Integer = oNodeLinks.Count - 1
         Dim iEndIndex As Integer = -1

         Dim oPrevLink As DMAcadExt.IUD_Link = Nothing

         Dim colResNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()
         For iIndex As Integer = 0 To 2 * iUB
            If IsSameDirection(miDirection, iExteriorDirection) Then
               tNodeLink = oNodeLinks.Item(iIndex Mod (iUB + 1))
            Else
               tNodeLink = oNodeLinks.Item((2 * iUB - iIndex) Mod (iUB + 1))
            End If

            If iEndIndex = -1 AndAlso tNodeLink.NodeID = iStartNodeID Then
               iEndIndex = iIndex + iUB
            End If
            If iEndIndex <> -1 Then
               colResNodeLinks.Add(tNodeLink)
            End If

            If iIndex = iEndIndex Then
               Exit For
            End If
         Next

			Return colResNodeLinks
      End Function

		Public Function GetLinks(iStartNodeID As Integer, iExteriorDirection As enRingDirection) As HashSet(Of DMAcadExt.IUD_Link)
			Dim tNodeLink As NodeLink

			Dim iEndIndex As Integer = -1
			Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
			Dim oPrevLink As DMAcadExt.IUD_Link = Nothing
			Dim oNodeScheme As tsNode
			If IsSameDirection(miDirection, iExteriorDirection) Then
				colNodeLinks = zzGetNodeLinks()

			Else
				colNodeLinks = zzGetNodeLinksReverse()
			End If
			Dim iUB As Integer = colNodeLinks.Count - 1


			Dim hsRes As HashSet(Of DMAcadExt.IUD_Link) = New HashSet(Of DMAcadExt.IUD_Link)()
			For iIndex As Integer = 0 To 2 * iUB
				Try
					tNodeLink = colNodeLinks.Item(iIndex Mod (iUB + 1))
				Catch oEx As Exception
					DMCommon.Debug.ExcelLog.SetNextValue(0, "GetLinks", oEx.Message, iIndex, iIndex Mod (iUB + 1), colNodeLinks.Count, 2 * iUB)
					tNodeLink = New NodeLink()
				End Try
				If tNodeLink.Link IsNot Nothing Then

					If iEndIndex = -1 AndAlso tNodeLink.NodeID = iStartNodeID Then
						iEndIndex = iIndex + iUB
					End If

					If iEndIndex <> -1 Then
						If oPrevLink Is Nothing Then
							oPrevLink = tNodeLink.Link
						Else
							oNodeScheme = mdicElements.GetNode(tNodeLink.NodeID)
							If Not oNodeScheme.IsSkipVertex Then
								hsRes.Add(oPrevLink)
								oPrevLink = tNodeLink.Link
							Else
								oPrevLink.Extend(tNodeLink.Link)
							End If
						End If

					End If

				End If
				If iIndex = iEndIndex Then
					Exit For
				End If
			Next
			If oPrevLink IsNot Nothing Then
				hsRes.Add(oPrevLink)
			End If

			Return hsRes
		End Function



		Public Function GetBulgeVertexArray(iStartNode As Integer, iExteriorDirection As enRingDirection) As GeoUtilites.BulgeVertexArray

         Dim hsBranches As HashSet(Of Integer) = zzShiftBranchSet(iStartNode, iExteriorDirection)
			Dim iNewDir As enRingDirection

			If iExteriorDirection = enRingDirection.Clockwise Then
            iNewDir = enRingDirection.CounterClockwise
         ElseIf iExteriorDirection = enRingDirection.CounterClockwise Then
            iNewDir = enRingDirection.Clockwise
         Else
            iNewDir = enRingDirection.NotDefined
         End If


         Dim oBranch As tsBranch
         Dim bIsBranchSameDir As Boolean
         Dim oResBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			For Each iBranchID As Integer In hsBranches
				oBranch = mdicElements.GetBranch(iBranchID)


				If iNewDir = enRingDirection.NotDefined Then
					bIsBranchSameDir = True
				ElseIf iNewDir = oBranch.GetDirection(miPolygonID, mbIsExterior) Then
					bIsBranchSameDir = True
				Else
					bIsBranchSameDir = False
				End If
				If Not (iExteriorDirection = miDirection) Then
					''''''''''''''  bIsBranchSameDir = Not bIsBranchSameDir
				End If

				oResBulgeVertexArray.AddCurve(oBranch.AcObjID, bIsBranchSameDir)
			Next

			If Not mbIsExterior Then
            '  oResBulgeVertexArray.CloseLoop()
         End If
			Dim tStart As Autodesk.AutoCAD.Geometry.Point2d = oResBulgeVertexArray.StartVertex.Vertex
			Dim tEnd As Autodesk.AutoCAD.Geometry.Point2d = oResBulgeVertexArray.EndVertex.Vertex

			Return oResBulgeVertexArray
      End Function

		Public Function GetBulgeVertexArray() As GeoUtilites.BulgeVertexArray
			Dim oBranch As tsBranch
			Dim oNode As tsNode
			Dim iNode As Integer
			Dim iVertexNum As Integer
			Dim bIsBranchSameDir As Boolean
			Dim oResBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()

			For Each iBranchID As Integer In mhsBranches
				oBranch = mdicElements.GetBranch(iBranchID)
				iNode = oBranch.GetPreviousNode(miPolygonID, mbIsExterior)
				oNode = mdicElements.GetNode(iNode)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetBulgeVert", miPolygonID, miRingNumber, iNode, oNode.BranchCount, oNode.Location)
				If miDirection = enRingDirection.NotDefined Then
					bIsBranchSameDir = True
				ElseIf miDirection = oBranch.GetDirection(miPolygonID, mbIsExterior) Then
					bIsBranchSameDir = True
				Else
					bIsBranchSameDir = False
				End If

				If Not oNode.IsPseudoTopo Then
					iVertexNum = oResBulgeVertexArray.UB
					If iVertexNum = -1 Then
						iVertexNum = 0
					End If
					oResBulgeVertexArray.AddGenuineNodes(iVertexNum)
				End If

				oResBulgeVertexArray.AddCurve(oBranch.AcObjID, bIsBranchSameDir)

			Next
			oResBulgeVertexArray.CloseLoop()
			If Not mbIsExterior Then
				'  oResBulgeVertexArray.CloseLoop()
			End If

			Return oResBulgeVertexArray
		End Function
		Public Function GetMidPointsNew(iStageNo As Integer) As DMAcadExt.TplnPointArray
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
         Dim tMidPoint As Autodesk.AutoCAD.Geometry.Point2d
         Dim oResTplnPoint As DMAcadExt.TPlnPoint = Nothing
         ' Dim oPrevNodeLink As NodeLink
         ' Dim oFirstNodeLink As NodeLink
         Dim sLineName As String = Nothing
         Dim sNodeName As String
         Dim sFirstNodeName As String = Nothing
         Dim sPrevNodeName As String = Nothing

         Dim oArcLineName As Tuple(Of String, Boolean)
         Dim iNodeStageNo As Integer
         Dim oNodeScheme As tsNode
         Dim bLinkIsArcPrev As Boolean
         Dim bLinkIsArc As Boolean
         Dim oPrevLink As DMAcadExt.IUD_Link = Nothing
         Dim oLink As DMAcadExt.IUD_Link = Nothing
         Dim oFirstLink As DMAcadExt.IUD_Link = Nothing


         Dim oFirstPoint As DMAcadExt.TPlnPoint = Nothing
         Dim oPrevPoint As DMAcadExt.TPlnPoint = Nothing
         Dim oCurrentPoint As DMAcadExt.TPlnPoint = Nothing
         Dim oNodeProperty As INodeProperty
         For Each oNodeLink As NodeLink In mcolNodeLinksCorrected

				oNodeScheme = Me.mdicElements.GetNode(oNodeLink.NodeID)
            oNodeProperty = oNodeScheme.NodeProperty
            oLink = oNodeLink.Link

            If oNodeProperty IsNot Nothing Then
               iNodeStageNo = oNodeProperty.Stage
            Else
               iNodeStageNo = 0
               DMAcadExt.AcadDocument.WriteMessage("!???STG= " & oNodeLink.NodeID.ToString())
            End If
            If iNodeStageNo <= iStageNo Then
               oCurrentPoint = New DMAcadExt.TPlnPoint(oNodeScheme.Location)
               bLinkIsArc = oNodeLink.Link.IsArc

               sNodeName = DMCommon.Functions.CStrN(oNodeScheme.Name)

               If oFirstPoint Is Nothing Then
                  oFirstPoint = oCurrentPoint
                  sFirstNodeName = sNodeName
               End If

               If oPrevPoint Is Nothing Then
                  oPrevPoint = oCurrentPoint
                  bLinkIsArcPrev = bLinkIsArc
                  sPrevNodeName = sNodeName
                  oPrevLink = oLink
               Else
                  tMidPoint = oPrevLink.GetMidPoint()
                  oResTplnPoint = New DMAcadExt.TPlnPoint(tMidPoint)


						sLineName = zzGetLineName(sPrevNodeName, sNodeName)
                  If oResTplnPoint IsNot Nothing Then
                     oArcLineName = New Tuple(Of String, Boolean)(sLineName, bLinkIsArcPrev)
                     oaResTplnPointArray.AddObject(oResTplnPoint, oArcLineName)
                     If (miPolygonID = 999647) Then   '647
                        DMAcadExt.AcadDocument.WriteMessage("!!##^" & CStr(miPolygonID) & " " & oResTplnPoint.Coordinates2d & "; " & sLineName & "; " & bLinkIsArc.ToString())
                     End If
                  End If

                  oPrevPoint = oCurrentPoint
                  oPrevLink = oLink
                  bLinkIsArcPrev = bLinkIsArc
                  sPrevNodeName = sNodeName
               End If
            Else
               If oPrevLink IsNot Nothing Then
                  oPrevLink.Extend(oLink)
               ElseIf oFirstLink IsNot Nothing Then
                  oFirstLink.Extend(oLink)
               Else
                  oFirstLink = oLink
               End If

            End If 'iNodeStageNo <= iStageNo
         Next
         If oFirstPoint IsNot Nothing AndAlso oLink IsNot Nothing Then
            If oFirstLink IsNot Nothing Then
               oLink.Extend(oFirstLink)
            End If
            tMidPoint = oLink.GetMidPoint()
            oResTplnPoint = New DMAcadExt.TPlnPoint(tMidPoint)
            sLineName = zzGetLineName(sPrevNodeName, sFirstNodeName)
            If oResTplnPoint IsNot Nothing Then
               oArcLineName = New Tuple(Of String, Boolean)(sLineName, bLinkIsArc)
               oaResTplnPointArray.AddObject(oResTplnPoint, oArcLineName)
               If (miPolygonID = 999647) Then   '647
                  DMAcadExt.AcadDocument.WriteMessage("!+##-" & CStr(miPolygonID) & " " & oResTplnPoint.Coordinates2d & "; " & sLineName & "; " & iNodeStageNo.ToString())
               End If
            End If


         End If


			Return oaResTplnPointArray

		End Function
      Public Function GetMidPoints() As DMAcadExt.TplnPointArray
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
         Dim tMidPoint As Autodesk.AutoCAD.Geometry.Point2d
         Dim oResTplnPoint As DMAcadExt.TPlnPoint = Nothing
			Dim sLineName As String = Nothing
			Dim sNodeName As String
         Dim sFirstNodeName As String = Nothing
         Dim oArcLineName As Tuple(Of String, Boolean)

         Dim oNodeScheme As tsNode
         Dim bLinkIsArc As Boolean
         For Each oNodeLink As NodeLink In mcolNodeLinksCorrected

				oNodeScheme = Me.mdicElements.GetNode(oNodeLink.NodeID)
            If oNodeScheme.Name IsNot Nothing Then
               sNodeName = oNodeScheme.Name
            Else
               sNodeName = String.Empty
            End If
            If sFirstNodeName Is Nothing Then
               sFirstNodeName = sNodeName
            End If
            If sLineName IsNot Nothing Then
               sLineName &= sNodeName
               If oResTplnPoint IsNot Nothing Then
                  oArcLineName = New Tuple(Of String, Boolean)(sLineName, bLinkIsArc)
                  oaResTplnPointArray.AddObject(oResTplnPoint, oArcLineName)
               End If
            End If


            tMidPoint = oNodeLink.Link.GetMidPoint()
            oResTplnPoint = New DMAcadExt.TPlnPoint(tMidPoint)
            bLinkIsArc = oNodeLink.Link.IsArc

            oNodeScheme = Me.mdicElements.GetNode(oNodeLink.NodeID)
            If oNodeScheme.Name IsNot Nothing Then
               sLineName = oNodeScheme.Name
            Else
               sLineName = String.Empty
            End If

            sLineName &= ","
         Next
         If sLineName IsNot Nothing AndAlso sFirstNodeName IsNot Nothing Then
            sLineName &= sFirstNodeName
            If oResTplnPoint IsNot Nothing Then
               oArcLineName = New Tuple(Of String, Boolean)(sLineName, bLinkIsArc) ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Add
               oaResTplnPointArray.AddObject(oResTplnPoint, oArcLineName)


            End If
         End If

         If False Then
            Dim s As String
            If oaResTplnPointArray.ItemObject(0) IsNot Nothing Then
               s = oaResTplnPointArray.ItemObject(0).Point.AcGePoint.ToString() & vbCrLf & oaResTplnPointArray.ItemObject(0).ThisObject.ToString()
            Else
               s = "Nothing"
            End If
            System.Windows.Forms.MessageBox.Show(CStr(oaResTplnPointArray.UpperBound) & vbCrLf & s, "05_413")
         End If
         Return oaResTplnPointArray

      End Function
   
      Public Function CreateDBPolyline(Optional oPolyline As Polyline = Nothing) As ObjectId
         Dim oBranch As tsBranch
         Dim oElement As tsElement = Nothing
         Dim oNodeScheme As tsNode = Nothing
         Dim bIsBranchSameDir As Boolean
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
         Dim iIndex As Integer

         For Each iBranchID As Integer In mhsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
            bIsBranchSameDir = GetCurveDirection(oBranch)
				oBulgeVertexArray.AddCurve(oBranch.AcObjID, bIsBranchSameDir, False)
				iIndex += 1
         Next
			Return oBulgeVertexArray.CreateDBPolyline(True, oPolyline)


		End Function
      Public Sub CopyFrom(oRing As tsRing)
			mhsNodes = tsTopology.CopyIntegerSet(oRing.NodeSet)
			maNodes = DirectCast(oRing.Nodes.Clone, tsNode())

         moaBranches = DirectCast(oRing.Branches.Clone, tsBranch())
         mhsBranches = tsTopology.CopyIntegerSet(oRing.BranchSet)


         mhsIsthmuses = tsTopology.CopyIntegerSet(oRing.Isthmuses)
         mbIsExterior = oRing.IsExterior



      End Sub
      Public Function GetMarkPoints(dDistParam As Double) As List(Of InitInsertData) ' System.Collections.ObjectModel.Collection(Of InitInsertData)
         Dim oBranch As tsBranch
         Dim oElement As tsElement = Nothing
         Dim oNodeScheme As tsNode = Nothing
         Dim bIsBranchSameDir As Boolean
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim iIndex As Integer
			Dim oResList As List(Of InitInsertData) = New List(Of InitInsertData)()
         Dim sPrevLayer As String = Nothing
         For Each iBranchID As Integer In mhsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
				If oBranch.IsFirstPolygon(miPolygonID) AndAlso mdicElements.LayerFilter.Contains(oBranch.Layer) Then
					If (sPrevLayer IsNot Nothing AndAlso sPrevLayer <> oBranch.Layer) Then
						oResList.AddRange(oBulgeVertexArray.GetMarkPoints(dDistParam, sPrevLayer))
						oBulgeVertexArray = New GeoUtilites.BulgeVertexArray()
					End If
					bIsBranchSameDir = GetCurveDirection(oBranch)

					oBulgeVertexArray.AddCurve(oBranch.AcObjID, bIsBranchSameDir, False)
					sPrevLayer = oBranch.Layer
				Else
					oResList.AddRange(oBulgeVertexArray.GetMarkPoints(dDistParam, sPrevLayer))
					oBulgeVertexArray = New GeoUtilites.BulgeVertexArray()
            End If

               iIndex += 1
         Next
			oResList.AddRange(oBulgeVertexArray.GetMarkPoints(dDistParam, sPrevLayer))
			Return oResList
		End Function

		Public Sub CreateDBPolylineCorrected(sLayer As String)
			Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = zzCurrentNodeLinks()
			Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim iCol As Integer
			Dim oNodeScheme As tsNode
			'zzColNodeLinksToExcel(colNodeLinks, "PolylineSimple")
			'zzColNodeLinksToExcel(mcolNodeLinks, "Simple Src")
			'zzColNodeLinksToExcel(mcolNodeLinksCorrected, "Simple Crct")
			If miPolygonID = 69 OrElse miPolygonID = 379 Then
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PLineCorrect", mdicElements.TopoName, miPolygonID, colNodeLinks.Count, "69 69 379 379 ")
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Captions", mdicElements.TopoName, miPolygonID, "StartPoint", "EndPoint", "HasUsedPoint", "HasActiveSurveyPoint", "HasOldPoint", "IsSkipVertex", "IsPseudoGeo", "IsPseudoTopo")
				For Each oNodeLink As NodeLink In colNodeLinks
					oNodeScheme = mdicElements.GetNode(oNodeLink.NodeID)
					'DMCommon.Debug.ExcelLog.SetNextValue(2, "!NodeLink", oNodeLink.Link.StartPoint.ToString(), oNodeLink.Link.EndPoint.ToString(), oNodeScheme.HasUsedPoint, oNodeScheme.HasActiveSurveyPoint _
					'											 , oNodeScheme.HasOldPoint, oNodeScheme.IsSkipVertex, oNodeScheme.IsPseudoGeo, oNodeScheme.IsPseudoTopo)
					'	DMCommon.Debug.ExcelLog.SetValueInRow(iCol, oNodeLink.Link.StartPoint.ToString())

					'	DMCommon.Debug.ExcelLog.SetValueInRow(iCol, oNodeLink.Link.EndPoint.ToString())
					'	DMCommon.Debug.ExcelLog.NextRow()
				Next


			End If

			oBulgeVertexArray.AddDim(colNodeLinks.Count)
			For Each oNodeLink As NodeLink In colNodeLinks
				oBulgeVertexArray.AddBulgeVertex(oNodeLink.Link.StartPoint, oNodeLink.Link.Bulge)
			Next
			mtBorderObjID = oBulgeVertexArray.CreateDBPolyline(True)
			Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(mtBorderObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			If oCurve IsNot Nothing Then
				mdAreaCorrected = oCurve.Area
				mtBorderHandle = oCurve.Handle
				If Not String.IsNullOrEmpty(sLayer) Then
					oCurve.Layer = sLayer
				End If
			End If
		End Sub

		Private Sub zzColNodeLinksToExcel(oNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink), sLabel As String)
			Dim iIndex As Integer
			If oNodeLinks Is Nothing Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, sLabel, miPolygonID, miRingNumber, "Nothing")
			Else
				For Each tNodeLink As NodeLink In oNodeLinks
					iIndex += 1
				Next
			End If



		End Sub


		Public Sub Analysis(oRing As Ring)
         Dim colHalfEdges As HalfEdgeCollection = oRing.GetEdges()
         Dim oStartHalfEdge As HalfEdge = oRing.StartEdge
         Dim oCurrentHalfEdge As HalfEdge = oStartHalfEdge
         DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_00 Start: " & CStr(oStartHalfEdge.FullEdge.ID))
         DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_00 Ring: " & CStr(miRingNumber) & " Pgon: " & CStr(miPolygonID))

         Dim oFullEdge As FullEdge
         Dim hsEdgesLeft As HashSet(Of Integer) = New HashSet(Of Integer)()
         Dim hsEdgesRight As HashSet(Of Integer) = New HashSet(Of Integer)()

         For Each oHalfEdge As HalfEdge In colHalfEdges
            oFullEdge = oHalfEdge.FullEdge
            DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_01: " & CStr(oFullEdge.ID))
         Next
         Do
            Try

               oCurrentHalfEdge = oCurrentHalfEdge.GetNextEdge(True)
               oFullEdge = oCurrentHalfEdge.FullEdge
               DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_02: " & CStr(oFullEdge.ID))
               If hsEdgesLeft.Contains(oFullEdge.ID) Then
                  Exit Do
               Else
                  hsEdgesLeft.Add(oFullEdge.ID)
               End If


            Catch oMapEx As Autodesk.Gis.Map.MapException
               If oMapEx.ErrorCode = 2063 Then
                  oFullEdge = oCurrentHalfEdge.FullEdge
                  DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_020: " & CStr(oFullEdge.ID))
						Exit Do
					Else

                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Analisis_22")
               End If
            End Try
         Loop
         oCurrentHalfEdge = oStartHalfEdge
         Do
            Try

               oCurrentHalfEdge = oCurrentHalfEdge.GetNextEdge(False)
               oFullEdge = oCurrentHalfEdge.FullEdge
               DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_03: " & CStr(oFullEdge.ID))

               If hsEdgesRight.Contains(oFullEdge.ID) Then
                  Exit Do
               Else
                  hsEdgesRight.Add(oFullEdge.ID)
               End If


            Catch oMapEx As Autodesk.Gis.Map.MapException

               If oMapEx.ErrorCode = 2063 Then
                  oFullEdge = oCurrentHalfEdge.FullEdge
                  DMAcadExt.AcadDocument.WriteMessage("@@@@@@ANZ_030: " & CStr(oFullEdge.ID))
						Exit Do
					Else

                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Analisis_32")
               End If
               Exit Do
            End Try
         Loop
      End Sub

      Public Sub AddNode(ByVal oNode As tsNode)
         maNodes(miRefNewNode) = oNode
         miRefNewNode += 1
      End Sub
      Public Sub AddBranch(ByVal oBranch As tsBranch)
         moaBranches(miRefNewBranch) = oBranch
         miRefNewBranch += 1
      End Sub
      Public ReadOnly Property IsExterior() As Boolean
         Get
            Return mbIsExterior
         End Get
      End Property
      Public ReadOnly Property ElementsUB() As Integer
         Get
            Return miElementsUB
         End Get
      End Property
      Public ReadOnly Property RingNumber() As Integer
         Get
            Return miRingNumber
         End Get

      End Property
		Public ReadOnly Property InnerPolygons() As HashSet(Of Integer)
			Get
				Return mhsInnerPolygons

			End Get

		End Property

		Public ReadOnly Property BranchSet As HashSet(Of Integer)
         Get
            Return mhsBranches
         End Get
      End Property
      Public ReadOnly Property NodeSet As HashSet(Of Integer)
         Get
            Return mhsNodes
         End Get
      End Property
      Public ReadOnly Property Nodes() As tsNode()
         Get

            Return maNodes
         End Get
      End Property


      Public ReadOnly Property Branches() As tsBranch()
         Get
            Return moaBranches
         End Get
      End Property
      Public ReadOnly Property NodeLinks() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Get
            Return mcolNodeLinks
         End Get
      End Property
      Public ReadOnly Property NodeLinksCorrected() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Get
            Return mcolNodeLinksCorrected
         End Get
      End Property
      Public ReadOnly Property CurrentNodeLinks() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Get
            Return zzCurrentNodeLinks()
         End Get
      End Property

      Public ReadOnly Property BorderObjID As ObjectId
         Get
            Return mtBorderObjID
         End Get
      End Property
      Public ReadOnly Property BorderHandle As Handle
         Get
            Return mtBorderHandle
         End Get
      End Property
      Public Sub MarkNodes(oMarkBlock As DMAcadExt.MarkBlock)
         Dim oNode As tsNode
         If oMarkBlock IsNot Nothing Then
            For Each iNodeID As Integer In mhsNodes
               oNode = mdicElements.GetNode(iNodeID)
               oMarkBlock.MarkPoint(oNode.Location, 2S)
            Next
         End If

      End Sub
      Public Function GetNodeLinksCorrected() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim iFirstNodeID As Integer
         Dim tNodeLink As NodeLink
         Dim tPrevNodeLink As NodeLink = New NodeLink()
         Dim colNodeLinksCorrected As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()
         For iIndex As Integer = 0 To mcolNodeLinksCorrected.Count - 1

				tNodeLink = mcolNodeLinksCorrected.Item(iIndex)
            If iIndex = 0 Then
               iFirstNodeID = tNodeLink.NodeID
            End If
            If tPrevNodeLink.IsNotEmpty Then
               tPrevNodeLink.NextNodeID = tNodeLink.NodeID
               colNodeLinksCorrected.Add(tPrevNodeLink)
            End If
            tPrevNodeLink = tNodeLink
         Next
         tPrevNodeLink.NextNodeID = iFirstNodeID
         colNodeLinksCorrected.Add(tPrevNodeLink)
         Return colNodeLinksCorrected
      End Function
      Public ReadOnly Property Direction() As enRingDirection
         Get
            Return miDirection
         End Get
      End Property
      Public Property AreaCorrected As Double
         Get
            Return mdAreaCorrected
         End Get
         Set(dValue As Double)
            mdAreaCorrected = dValue
         End Set
      End Property
      Public Overridable Sub UnionWithAAA(oRing As tsRing)

      End Sub
      Public Sub PrintInfo()

         If mbIsExterior Then
            DMAcadExt.AcadDocument.WriteMessage("--------Exterior Ring ")
         Else
            DMAcadExt.AcadDocument.WriteMessage("--------Interior Ring# " & CStr(miRingNumber))
         End If
         DMAcadExt.AcadDocument.WriteMessage("Direction: " & miDirection.ToString() & " BranchOrder: " & mbBranchOrder.ToString())
         DMAcadExt.AcadDocument.WriteMessage("Branches: " & CStr(mhsBranches.Count) & ", Nodes: " & mhsNodes.Count.ToString())
         Dim oBranch As tsBranch
         Dim oPrevNode As tsNode, oNextNode As tsNode
         If False Then


            For Each iBranchID As Integer In mhsBranches
               oBranch = mdicElements.GetBranch(iBranchID)
               DMAcadExt.AcadDocument.WriteMessage("CurveDirection: " & GetCurveDirection(oBranch).ToString())
               GetCurveDirection(oBranch)
               oBranch.PrintInfo()
               oPrevNode = mdicElements.GetNode(oBranch.PreviousNodeID)
               oNextNode = mdicElements.GetNode(oBranch.NextNodeID)
               DMAcadExt.AcadDocument.WriteMessage("Points: " & oPrevNode.Location.ToString() & " ==> " & oNextNode.Location.ToString())
               DMAcadExt.AcadDocument.WriteMessage("Node " & CStr(oPrevNode.ID) & ": PseudoTopo - " & oPrevNode.IsPseudoTopo.ToString() & " & PseudoGeo - " & oPrevNode.IsPseudoGeo.ToString())
               DMAcadExt.AcadDocument.WriteMessage("Node " & CStr(oNextNode.ID) & ": PseudoTopo - " & oNextNode.IsPseudoTopo.ToString() & " & PseudoGeo - " & oNextNode.IsPseudoGeo.ToString())
            Next
         End If

      End Sub
      Private Sub zzSetCheckDirNew(oPreviousHalfEdge As HalfEdge, oCurrentHalfEdge As HalfEdge, bSameDir As Boolean)
         Dim iDir As enRingDirection = enRingDirection.NotDefined

         Dim iCurrentPrevID As Integer
         Dim iCurrentNextID As Integer

         If True Then
            iCurrentPrevID = oCurrentHalfEdge.PreviousNode.ID
            iCurrentNextID = oCurrentHalfEdge.NextNode.ID
         Else
            iCurrentPrevID = oCurrentHalfEdge.NextNode.ID
            iCurrentNextID = oCurrentHalfEdge.PreviousNode.ID
         End If


         If oPreviousHalfEdge.PreviousNode.ID = iCurrentPrevID AndAlso oPreviousHalfEdge.NextNode.ID = iCurrentNextID Then
            '  Return enRingDirection.NotDefined
         ElseIf oPreviousHalfEdge.NextNode.ID = iCurrentPrevID Then

            mbBranchOrder = True
         ElseIf oPreviousHalfEdge.PreviousNode.ID = iCurrentNextID Then
            mbBranchOrder = False
         Else
            If miPolygonID = 43 Then
               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#120: " & bSameDir.ToString() & "," & CStr(miPolygonID) & " Prev: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(oPreviousHalfEdge.NextNode.ID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNode.ID) & "==>" & CStr(oCurrentHalfEdge.NextNode.ID))
               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#120A: " & CStr(miPolygonID) & " Curr: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(iCurrentNextID) & ", " & CStr(oPreviousHalfEdge.NextNode.ID) & "==>" & CStr(iCurrentPrevID) & " ??? " & CStr(oPreviousHalfEdge.PreviousNode.ID = iCurrentNextID))
            End If

         End If
         'Prev: 5==>3, Current: 13==>5
         'oPreviousHalfEdge.PreviousNode.ID  oCurrentHalfEdge.NextNode.ID
      End Sub

      Private Sub zzCheckOrder(oPreviousHalfEdge As tsBranch, oCurrentHalfEdge As tsBranch, bPrevSameDir As Boolean, bSameDir As Boolean)
         Dim iDir As enRingDirection = enRingDirection.NotDefined


         Dim iPrevPrevID As Integer
         Dim iPrevNextID As Integer

         Dim iCurrentPrevID As Integer
         Dim iCurrentNextID As Integer

         If bPrevSameDir Then
            iPrevPrevID = oPreviousHalfEdge.PreviousNodeID
            iPrevNextID = oPreviousHalfEdge.NextNodeID
         Else
            iPrevPrevID = oPreviousHalfEdge.NextNodeID
            iPrevNextID = oPreviousHalfEdge.PreviousNodeID
         End If
         If bSameDir Then
            iCurrentPrevID = oCurrentHalfEdge.PreviousNodeID
            iCurrentNextID = oCurrentHalfEdge.NextNodeID
         Else
            iCurrentPrevID = oCurrentHalfEdge.NextNodeID
            iCurrentNextID = oCurrentHalfEdge.PreviousNodeID
         End If

         If oPreviousHalfEdge.PreviousNodeID = iCurrentPrevID AndAlso oPreviousHalfEdge.NextNodeID = iCurrentNextID Then
            '  Return enRingDirection.NotDefined
         ElseIf iPrevNextID = iCurrentPrevID Then

            mbBranchOrder = True
         ElseIf iPrevPrevID = iCurrentNextID Then
            mbBranchOrder = False
         Else
            If miPolygonID = 43 Then
               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#120: " & bPrevSameDir.ToString() & "/" & bSameDir.ToString() & "," & CStr(miPolygonID) & " Prev: " & CStr(oPreviousHalfEdge.PreviousNodeID) & "==>" & CStr(oPreviousHalfEdge.NextNodeID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNodeID) & "==>" & CStr(oCurrentHalfEdge.NextNodeID))
               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#120A: " & CStr(miPolygonID) & " Curr: " & CStr(oPreviousHalfEdge.PreviousNodeID) & "==>" & CStr(iCurrentNextID) & ", " & CStr(oPreviousHalfEdge.NextNodeID) & "==>" & CStr(iCurrentPrevID) & " ??? " & CStr(oPreviousHalfEdge.PreviousNodeID = iCurrentNextID))
            End If

         End If
         'Prev: 5==>3, Current: 13==>5
         'oPreviousHalfEdge.PreviousNode.ID  oCurrentHalfEdge.NextNode.ID
      End Sub
      Private Sub zzSetCheckDir(oPreviousHalfEdge As HalfEdge, oCurrentHalfEdge As HalfEdge)
         'If miDirection = enRingDirection.NotDefined Then
         '   miDirection = zzGetDir(oPreviousHalfEdge, oCurrentHalfEdge)
         'ElseIf miDirection <> zzGetDir(oPreviousHalfEdge, oCurrentHalfEdge) Then
         '   DMAcadExt.AcadDocument.WriteMessage("DesignErr_#120: " & CStr(miPolygonID) & " RingDir: " & miDirection.ToString() & " Prev: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(oPreviousHalfEdge.NextNode.ID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNode.ID) & "==>" & CStr(oCurrentHalfEdge.NextNode.ID))
         'End If


         Dim iDir As enRingDirection = enRingDirection.NotDefined
         Dim bBranchOrder As Boolean
         If oPreviousHalfEdge.PreviousNode.ID = oCurrentHalfEdge.PreviousNode.ID AndAlso oPreviousHalfEdge.NextNode.ID = oCurrentHalfEdge.NextNode.ID Then
            '  Return enRingDirection.NotDefined
         ElseIf oPreviousHalfEdge.NextNode.ID = oCurrentHalfEdge.PreviousNode.ID Then
            If mbIsExterior Then
               iDir = enRingDirection.CounterClockwise
            Else
               iDir = enRingDirection.Clockwise
            End If
            bBranchOrder = True
         ElseIf oPreviousHalfEdge.PreviousNode.ID = oCurrentHalfEdge.NextNode.ID Then
            If mbIsExterior Then
               iDir = enRingDirection.Clockwise
            Else
               iDir = enRingDirection.CounterClockwise
            End If
            If miDirection = enRingDirection.NotDefined Then
               miDirection = iDir
            End If
            bBranchOrder = False
         Else
				DMAcadExt.AcadDocument.WriteMessage("DesignErr_#113: " & CStr(miPolygonID) & " Prev: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(oPreviousHalfEdge.NextNode.ID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNode.ID) & "==>" & CStr(oCurrentHalfEdge.NextNode.ID))

			End If
         If miDirection = enRingDirection.NotDefined Then
            miDirection = iDir
            mbBranchOrder = bBranchOrder
            If miPolygonID = 43 AndAlso mdicElements.TopoName = "Stage_0" Then
               DMAcadExt.AcadDocument.WriteMessage("!!+ Dir: " & miDirection.ToString() & ", Order: " & mbBranchOrder.ToString())
            End If
         ElseIf miDirection <> iDir Then
            DMAcadExt.AcadDocument.WriteMessage("DesignErr_#112: " & CStr(miPolygonID) & " Prev: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(oPreviousHalfEdge.NextNode.ID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNode.ID) & "==>" & CStr(oCurrentHalfEdge.NextNode.ID))
         ElseIf mbBranchOrder <> bBranchOrder Then
            DMAcadExt.AcadDocument.WriteMessage("DesignErr_#114: " & CStr(miPolygonID) & " Prev: " & CStr(oPreviousHalfEdge.PreviousNode.ID) & "==>" & CStr(oPreviousHalfEdge.NextNode.ID) & ", Current: " & CStr(oCurrentHalfEdge.PreviousNode.ID) & "==>" & CStr(oCurrentHalfEdge.NextNode.ID))
         End If

      End Sub
		Private Function zzCurrentNodeLinks() As System.Collections.ObjectModel.Collection(Of NodeLink)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonID_BranchSet", mcolNodeLinksCorrected Is Nothing, mcolNodeLinks Is Nothing)
			If mcolNodeLinksCorrected Is Nothing Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "mcolNodeLinks.Count", miPolygonID, mcolNodeLinks.Count)
				For Each oNodeLink As NodeLink In mcolNodeLinks

					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "For Each oNodeLink", miPolygonID, oNodeLink.Link Is Nothing)
					If oNodeLink.Link IsNot Nothing Then
						DMAcadExt.AcadDocument.WriteDebugMessageN("oNodeLink.Link IsNot Nothings", miPolygonID, oNodeLink.Link.StartPoint, oNodeLink.Link.EndPoint, oNodeLink.Link.Bulge, oNodeLink.LinkSameDir)

					Else
						DMAcadExt.AcadDocument.WriteDebugMessageN("!!CurrentNodeLinks Is Nothing")
					End If
				Next
				Return mcolNodeLinks

			Else
				Return mcolNodeLinksCorrected
			End If

		End Function
		Private Function zzGetDir(oPrevious As HalfEdge, oCurrent As HalfEdge) As enRingDirection

			If oPrevious.PreviousNode.ID = oCurrent.PreviousNode.ID AndAlso oPrevious.NextNode.ID = oCurrent.NextNode.ID Then
            Return enRingDirection.NotDefined
         ElseIf oPrevious.NextNode.ID = oCurrent.PreviousNode.ID Then
            If mbIsExterior Then
               Return enRingDirection.CounterClockwise
            Else
               Return enRingDirection.Clockwise
            End If

         ElseIf oPrevious.PreviousNode.ID = oCurrent.NextNode.ID Then
            If mbIsExterior Then
               Return enRingDirection.Clockwise
            Else
               Return enRingDirection.CounterClockwise
            End If

         Else
            DMAcadExt.AcadDocument.WriteMessage("DesignErr_#110: " & CStr(miPolygonID) & " Prev: " & CStr(oPrevious.PreviousNode.ID) & "==>" & CStr(oPrevious.NextNode.ID) & ", Current: " & CStr(oCurrent.PreviousNode.ID) & "==>" & CStr(oCurrent.NextNode.ID))
            Return enRingDirection.NotDefined
         End If
      End Function
      Private Function zzShiftBranchSet(iStartNode As Integer, iExteriorDirection As enRingDirection) As HashSet(Of Integer)
         Dim hsPreset As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim hsResult As HashSet(Of Integer) = New HashSet(Of Integer)

         Dim iNodeID As Integer
         Dim oBranch As tsBranch
         Dim bPre As Boolean = True
         Dim bIsBranchSameDir As Boolean
         Dim iaBranches(mhsBranches.Count - 1) As Integer
         '  Dim bDir As Boolean = (miDirection <> iDirection)
         Dim iIndex As Integer = 0
         Dim iNewDir As enRingDirection

         If iExteriorDirection = enRingDirection.Clockwise Then
            iNewDir = enRingDirection.CounterClockwise
         ElseIf iExteriorDirection = enRingDirection.CounterClockwise Then
            iNewDir = enRingDirection.Clockwise
         Else
            iNewDir = enRingDirection.NotDefined
         End If
         For Each iBranchID As Integer In mhsBranches
            If iNewDir = miDirection Then
               iaBranches(iIndex) = iBranchID
               iNewDir = miDirection
            Else
               iaBranches(mhsBranches.Count - 1 - iIndex) = iBranchID
               If miDirection = enRingDirection.Clockwise Then
                  iNewDir = enRingDirection.CounterClockwise
               ElseIf miDirection = enRingDirection.CounterClockwise Then
                  iNewDir = enRingDirection.Clockwise
               End If
            End If
            iIndex += 1
         Next

         '   MessageBox.Show(CStr(bDir) & vbCrLf & miDirection.ToString() & vbCrLf & iDirection.ToString(), "04_611")
         For Each iBranchID As Integer In iaBranches
            '  MessageBox.Show(CStr(iBranchID), "04_613")
            oBranch = mdicElements.GetBranch(iBranchID)

            If iNewDir = enRingDirection.NotDefined Then
               bIsBranchSameDir = True
            ElseIf iNewDir = oBranch.GetDirection(miPolygonID, mbIsExterior) Then
               bIsBranchSameDir = True
            Else
               bIsBranchSameDir = False
            End If
            If bIsBranchSameDir Then  'Not
               iNodeID = oBranch.PreviousNodeID
            Else
               iNodeID = oBranch.NextNodeID
            End If

            '  DMAcadExt.AcadDocument.WriteMessage("!!!+! B=" & CStr(iBranchID) & " NN " & CStr(oBranch.PreviousNodeID) & "<=>" & CStr(oBranch.NextNodeID) & "||" & bIsBranchSameDir.ToString())
            '  DMAcadExt.AcadDocument.WriteMessage("!!!!!! N=" & CStr(iNodeID) & " B=" & CStr(iBranchID))
            If bPre Then
               If iNodeID = iStartNode Then
                  bPre = False
                  hsResult.Add(iBranchID)
               Else
                  hsPreset.Add(iBranchID)
               End If
            Else
               hsResult.Add(iBranchID)
            End If
         Next
         '   MessageBox.Show(CStr(bDir), "04_614")
         If False Then
            For Each i As Integer In hsPreset
               DMAcadExt.AcadDocument.WriteMessage("!!!!Preset:" & CStr(i))
            Next
            For Each i As Integer In hsResult
               DMAcadExt.AcadDocument.WriteMessage("!!!!hsResult:" & CStr(i))
            Next

            hsResult.UnionWith(hsPreset)
            For Each i As Integer In hsResult
               DMAcadExt.AcadDocument.WriteMessage("!!!!Total:" & CStr(i))
            Next
         End If


         Return hsResult
      End Function

      Private Function zzShiftBranchSetA(iStartNode As Integer, iDirection As enRingDirection) As HashSet(Of Integer)
         Dim hsPreset As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim hsResult As HashSet(Of Integer) = New HashSet(Of Integer)

         Dim iNodeID As Integer
         Dim oBranch As tsBranch
         Dim bPre As Boolean = True
         Dim bIsBranchSameDir As Boolean
         Dim iaBranches(mhsBranches.Count - 1) As Integer
         Dim bDir As Boolean = (miDirection <> iDirection)
         Dim iIndex As Integer = 0
         For Each iBranchID As Integer In mhsBranches
            If bDir Then
               iaBranches(iIndex) = iBranchID
            Else
               iaBranches(mhsBranches.Count - 1 - iIndex) = iBranchID
            End If
            iIndex += 1
         Next

         '   MessageBox.Show(CStr(bDir) & vbCrLf & miDirection.ToString() & vbCrLf & iDirection.ToString(), "04_611")
         For Each iBranchID As Integer In iaBranches
            '  MessageBox.Show(CStr(iBranchID), "04_613")
            oBranch = mdicElements.GetBranch(iBranchID)
            If miDirection = enRingDirection.NotDefined Then
               bIsBranchSameDir = True
            ElseIf miDirection = oBranch.GetDirection(miPolygonID, mbIsExterior) Then
               bIsBranchSameDir = True
            Else
               bIsBranchSameDir = False
            End If
            If bIsBranchSameDir Then
               iNodeID = oBranch.PreviousNodeID
            Else
               iNodeID = oBranch.NextNodeID
            End If

            DMAcadExt.AcadDocument.WriteMessage("!!!+! B=" & CStr(iBranchID) & " NN " & CStr(oBranch.PreviousNodeID) & "<=>" & CStr(oBranch.NextNodeID) & "||" & bIsBranchSameDir.ToString())
            DMAcadExt.AcadDocument.WriteMessage("!!!!!! N=" & CStr(iNodeID) & " B=" & CStr(iBranchID))
            If bPre Then
               If iNodeID = iStartNode Then
                  bPre = False
                  hsResult.Add(iBranchID)
               Else
                  hsPreset.Add(iBranchID)
               End If
            Else
               hsResult.Add(iBranchID)
            End If
         Next
         '   MessageBox.Show(CStr(bDir), "04_614")
         If False Then
            For Each i As Integer In hsPreset
               DMAcadExt.AcadDocument.WriteMessage("!!!!Preset:" & CStr(i))
            Next
            For Each i As Integer In hsResult
               DMAcadExt.AcadDocument.WriteMessage("!!!!hsResult:" & CStr(i))
            Next

            hsResult.UnionWith(hsPreset)
            For Each i As Integer In hsResult
               DMAcadExt.AcadDocument.WriteMessage("!!!!Total:" & CStr(i))
            Next
         End If


         Return hsResult
      End Function


      Protected Overrides Sub Finalize()
         MyBase.Finalize()
      End Sub
   End Class
   Public Structure NodeLink
      Dim NodeID As Integer
      Dim BranchID As Integer
      Dim LinkSameDir As Boolean
      Dim Link As DMAcadExt.IUD_Link
      Dim IsNotEmpty As Boolean
      Dim NextNodeID As Integer
      Public Sub New(iNodeID As Integer, oLink As DMAcadExt.IUD_Link)
         NodeID = iNodeID
         Link = oLink
         IsNotEmpty = True
      End Sub
		Public Sub New(iNodeID As Integer, iBranchID As Integer, bLinkSameDir As Boolean, oLink As DMAcadExt.IUD_Link, Optional iNextNodeID As Integer = 0)
			NodeID = iNodeID
			BranchID = iBranchID
			LinkSameDir = bLinkSameDir
			Link = oLink
			NextNodeID = iNextNodeID
			IsNotEmpty = True
		End Sub
		Public Function GetSharedID(iNodeID As Integer, iNextNodeID As Integer) As Integer
			If NodeID = iNodeID OrElse NodeID = iNextNodeID Then
				Return NodeID
			ElseIf (NextNodeID = iNodeID) OrElse (NextNodeID = iNextNodeID) Then
				Return NextNodeID
			Else
				Return 0
			End If
		End Function
		Public Sub PrintInfo(Optional sCaption As String = Nothing)
         Dim sPrefix As String
         If String.IsNullOrEmpty(sCaption) Then
            sPrefix = String.Empty
         Else
            sPrefix = sCaption & " "
         End If
         DMAcadExt.AcadDocument.WriteMessage(sPrefix & "Node: " & CStr(NodeID) & ", BranchID " & BranchID.ToString() & " " & LinkSameDir.ToString() & " Link " & Link.GetInfo(3))
      End Sub
		Public Function Copy() As NodeLink

			Dim oLinkCopy As DMAcadExt.IUD_Link = Link.Copy()

			Return New NodeLink(NodeID, BranchID, LinkSameDir, oLinkCopy)
		End Function
		Public Overrides Function ToString() As String
         Return "N=" & CStr(NodeID) & "; B=" & CStr(BranchID) & "; Link= " & Link.GetInfo(3)
      End Function
   End Structure

   Public Structure TopoEntityData
      Dim TopoID As Integer
      Dim AcObjID As ObjectId
      Dim EntHandle As Handle
    
      Public Sub New(iTopoID As Integer, tAcObjID As ObjectId, tEntHandle As Handle)
         TopoID = iTopoID
         tAcObjID = AcObjID
         EntHandle = tEntHandle
      End Sub
   


      Public Overrides Function ToString() As String
         Return ""
      End Function
   End Structure
End Namespace
 
