Option Explicit On
Option Strict On
Imports System.Collections.Generic
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports TopoManager.TPlanGraph
Namespace TopoScheme

   Public Class tsTopology
      Public Const MapObjectsDispose As Boolean = True
      Private mdicElements As tsElements
      Private msName As String
      '   Private mhsPolygons As HashSet(Of Integer)
      '   Private mhsBranches As HashSet(Of Integer)
      '   Private mhsNodes As HashSet(Of Integer)

      Private mdicTopoIDByStage As Dictionary(Of Integer, UD_ParcelKey)
      Private mdicAttributes As IDictionary(Of Integer, IEqualityComparer)
      Private mdicAttributesA As IDictionary(Of Integer, IEquatable(Of System.Object))
		Private moEqualityComparer As IEqualityComparer(Of tsPolygon)
		Private mdicPolygonBorders As ObjectIdCollection
		Public Shared Function CopyIntegerSet(hsIntegers As HashSet(Of Integer)) As HashSet(Of Integer)
         Dim iaValues() As Integer = Nothing
         hsIntegers.CopyTo(iaValues)
         Return New HashSet(Of Integer)(iaValues)
      End Function

		Public Sub New(Optional sName As String = Nothing, Optional bHasCentroidData As Boolean = False)
			msName = sName
			mdicElements = New tsElements(sName, bHasCentroidData)
		End Sub
		Public Function Load(bCheckExtend As Boolean, Optional ByRef oTopoModel As TopologyModel = Nothing) As Boolean
         Dim bCloseTopo As Boolean = False
         If oTopoModel Is Nothing Then
            oTopoModel = TopoManager.TopoCreator.GetOpenedTopology(msName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
            bCloseTopo = True
         Else
				msName = oTopoModel.Name
				mdicElements.TopoName = msName
			End If

         If oTopoModel IsNot Nothing Then
            If oTopoModel.IsComplete Then
               Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
               Dim colEdges As FullEdgeCollection = oTopoModel.GetFullEdges()
               Dim colNodes As NodeCollection = oTopoModel.GetNodes()
               '  Dim iTest As Integer
               '  Dim colRings As RingCollection

               If colPolygons IsNot Nothing Then

						For Each oPolygon As Polygon In colPolygons
							'  colRings = oPolygon.GetBoundary()
							mdicElements.AddPolygon(oPolygon, bCheckExtend)
							'
							'  DMAcadExt.AcadDocument.WriteMessageLog("After PgonID: " & CStr(oPolygon.ID))
						Next


						'   DMAcadExt.AcadDocument.WriteMessageLog("Full Edges: " & CStr(colEdges.Count))
						''''''''''''''1104
						For Each oFullEdge As FullEdge In colEdges
							mdicElements.TryAddBranch(oFullEdge)
						Next

						If tsTopology.MapObjectsDispose Then
                     colEdges.Dispose()
                  End If

                  colEdges = Nothing
                  If True Then   ''''''''''''''1104
                     For Each oNode As Node In colNodes
                        mdicElements.TryAddNodeScheme(oNode, -1)
                     Next
                  End If
                  If tsTopology.MapObjectsDispose Then
                     colNodes.Dispose()
                  End If
                  colNodes = Nothing
                  If tsTopology.MapObjectsDispose Then
                     colPolygons.Dispose()
                  End If

                  colPolygons = Nothing
                  '  DMCommon.Debug.MsgBox("08_348", mdicElements.Nodes.Count, oTopoModel.GetNodes().Count)
               End If
               If bCloseTopo Then
                  oTopoModel.Close()
               End If

               Return True
            Else
               Dim sMsg As String
               If oTopoModel Is Nothing Then
                  sMsg = "incorrect"
               Else
                  sMsg = "incomplete"
               End If
               MessageBox.Show("Topology '" & msName & "' is " & sMsg, "Datamap")
               If oTopoModel IsNot Nothing Then
                  oTopoModel.Close()
                  oTopoModel = Nothing
               End If
               If bCloseTopo Then
                  oTopoModel.Close()
               End If
               Return False
               'DesignErr
            End If
         Else
            Return False
         End If

      End Function

      Public ReadOnly Property Name As String
         Get
            Return msName
         End Get
      End Property
      Public ReadOnly Property Nodes As System.Collections.ObjectModel.Collection(Of tsNode)

         Get
            Return mdicElements.Nodes
         End Get
      End Property

      Public ReadOnly Property Branches As System.Collections.ObjectModel.Collection(Of tsBranch)

         Get
            Return mdicElements.Branches
         End Get
      End Property
		Public ReadOnly Property Polygons As System.Collections.ObjectModel.Collection(Of tsPolygon)

			Get
				Return mdicElements.Polygons
			End Get
		End Property
		Public ReadOnly Property TotalArea As Double
			Get
				Return mdicElements.TotalArea
			End Get
		End Property

		Public Property LayerFilter As DMCommon.dmList
         Get
            Return mdicElements.LayerFilter
         End Get
         Set(oValue As DMCommon.dmList)
            mdicElements.LayerFilter = oValue
         End Set
      End Property

		'Act

		Public Sub CheckPointCount()
			Dim oPgon As tsPolygon
			For Each iPgonID As Integer In mdicElements.PolygonIDs
				oPgon = GetPolygon(iPgonID)
				For iRingIndex As Integer = 0 To oPgon.RingsUB
					oPgon.Rings(iRingIndex).CheckPointCount()
				Next

			Next
		End Sub
		Public Sub CheckIslands()
			Dim oPgon As tsPolygon
			For Each iPgonID As Integer In mdicElements.PolygonIDs
				oPgon = GetPolygon(iPgonID)

				oPgon.CheckIslands()

			Next
		End Sub
		Public Sub CalcIsthmus()
         Dim oPgon As tsPolygon
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = GetPolygon(iPgonID)

				oPgon.CalcIsthmus()

			Next
      End Sub
      Public Sub CheckHugeArcRadius(dRadiusMax As Double, tMarkColor As Autodesk.AutoCAD.Colors.Color)
         Dim oBranch As tsBranch
         For Each iBranchID As Integer In mdicElements.BrancheIDs
            oBranch = GetBranch(iBranchID)
            oBranch.MarkHugeArcRadius(dRadiusMax, tMarkColor)
         Next
      End Sub
      Public Sub RemovePseudoPoints()
			Dim oPgon As tsPolygon

			For Each iPgonID As Integer In mdicElements.PolygonIDs
				oPgon = GetPolygon(iPgonID)
				For iRingIndex As Integer = 0 To oPgon.RingsUB
					oPgon.Rings(iRingIndex).RemovePseudoPoints()
				Next
				If oPgon.Isthmuses.Count <> 0 Then ''''''''' 2604
					'''''''''''''''''''''  oPgon.CreateComplexNodeLinks()
				End If
			Next

		End Sub
      Public Function GetMidPointsNew(iStageNo As Integer) As DMAcadExt.TplnPointArray
         Dim oPgon As tsPolygon
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = GetPolygon(iPgonID)
            For iRingIndex As Integer = 0 To oPgon.RingsUB
               oaResTplnPointArray.AddObject(oPgon.Rings(iRingIndex).GetMidPointsNew(iStageNo))
            Next
            If False Then
               Dim s As String
               If oaResTplnPointArray.ItemObject(0) IsNot Nothing Then
                  s = oaResTplnPointArray.ItemObject(0).ToString()
               Else
                  s = "Nothing"
               End If
               System.Windows.Forms.MessageBox.Show(CStr(oaResTplnPointArray.UpperBound) & vbCrLf & s, "05_448")
            End If
         Next

         Return oaResTplnPointArray
      End Function
      Public Function GetMidPoints() As DMAcadExt.TplnPointArray
         Dim oPgon As tsPolygon
         Dim oaResTplnPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = GetPolygon(iPgonID)
            For iRingIndex As Integer = 0 To oPgon.RingsUB
               oaResTplnPointArray.AddObject(oPgon.Rings(iRingIndex).GetMidPoints())
            Next
            If False Then
               Dim s As String
               If oaResTplnPointArray.ItemObject(0) IsNot Nothing Then
                  s = oaResTplnPointArray.ItemObject(0).ToString()
               Else
                  s = "Nothing"
               End If
               System.Windows.Forms.MessageBox.Show(CStr(oaResTplnPointArray.UpperBound) & vbCrLf & s, "05_448")
            End If
         Next

         Return oaResTplnPointArray
      End Function
		Public Sub CreateDBPolylines()
			Dim oPgon As tsPolygon

			mdicPolygonBorders = New ObjectIdCollection()
			For Each iPgonID As Integer In mdicElements.PolygonIDs
				If iPgonID <> -1 Then   '= 1721
					oPgon = GetPolygon(iPgonID)
					mdicPolygonBorders.Add(oPgon.CreateDBPolylineWOInners())
				End If
			Next
		End Sub
		Public Sub CalcAreaByCentroidLayer(ByRef saLayers() As String, ByRef daValues() As Double, ByRef dTotal As Double)
			Dim oPgon As tsPolygon

			Dim oSummator As DMCommon.Summator = New DMCommon.Summator()
			For Each iPgonID As Integer In mdicElements.PolygonIDs
				If iPgonID <> -1 Then   '= 1721
					oPgon = GetPolygon(iPgonID)
					oSummator.AddValue(oPgon.Layer, oPgon.Area)
				End If
			Next
			oSummator.GetAllValue(saLayers, daValues, dTotal)
			DMAcadExt.AcadDocument.WriteDebugMessage("Total Polygon Area=" & dTotal.ToString())
		End Sub
		Public Sub CreateDBPolylinesByCentroidLayerForDorit()
         Dim oPgon As tsPolygon
         Dim sLayer As String
         Dim bCurrentLayerOK As Boolean
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            If iPgonID <> -1 Then   '= 1721
               oPgon = GetPolygon(iPgonID)
               sLayer = oPgon.Layer
               DMAcadExt.AcadDocument.WriteMessage("@@167 " & DMCommon.Functions.CStrN(sLayer), "Nothing")
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False, False, False)
               oPgon.CreateDBPolylineWOInners()

            End If
         Next
      End Sub
      Public Function CreatePgonDBPolyline(iPgonID As Integer, Optional ByVal sLayer As String = Nothing) As ObjectId
         Dim oPgon As tsPolygon

         If iPgonID <> -1 Then   '= 1721
            oPgon = GetPolygon(iPgonID)
            Return oPgon.CreateDBPolylineWOInners()
         Else
            Return ObjectId.Null
         End If

      End Function


      Public Function GetMarkPoints(dDistParam As Double) As List(Of InitInsertData) 'System.Collections.ObjectModel.Collection(Of InitInsertData)
         Dim oResList As List(Of InitInsertData) = New List(Of InitInsertData)()
         Dim oPgon As tsPolygon
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = GetPolygon(iPgonID)
            oResList.AddRange(oPgon.GetMarkPoints(dDistParam))
         Next
         Return oResList
      End Function


		Public Sub CreateDBPolylineMPlus(bWithIsthmuses As Boolean, bByCentroidLayer As Boolean, Optional hsDestPgonIDs As HashSet(Of Integer) = Nothing)
			Dim oPgon As tsPolygon
			Dim iCnt As Integer = 0
			'Dim iCntPrev As Integer = zzTestTempAllPlines()

			'DMCommon.ExcelLogLLA.Open()
			'	DMAcadExt.AcadDocument.WriteMessage("OK Topo: PoligonCount=" & CStr(mdicElements.PolygonIDs.Count) & "; Name=" & msName)
			'	DMCommon.ExcelLogAX.SetNextValue(0, "------------")

			For Each iPgonID As Integer In mdicElements.PolygonIDs

				If hsDestPgonIDs Is Nothing OrElse hsDestPgonIDs.Contains(iPgonID) Then
					oPgon = GetPolygon(iPgonID)
					'DMCommon.ExcelLogAX.SetNextValue(1, iPgonID, "oPgon IsNot Nothing", oPgon IsNot Nothing)

					If oPgon IsNot Nothing Then
						If oPgon.RingsUB >= 0 Then  'AndAlso oPgon.RingsUB = oPgon.Isthmuses.Count

							' DMAcadExt.AcadDocument.WriteMessage("OK Before: Poligon #" & CStr(iPgonID))
							'    DMCommon.Debug.MsgBox("08_104", oPgon.AcObjID, oPgon.ID, oPgon.RingsUB, oPgon.Isthmuses.Count)
							'  DMCommon.ExcelLogL.SetNextValue(0, "Before", oPgon.ID, oPgon.RingsUB, bWithIsthmuses, oPgon.Isthmuses.Count)

							oPgon.CreateDBPolylineCorrected(bWithIsthmuses, bByCentroidLayer)

						End If
					Else
						DMAcadExt.AcadDocument.WriteMessage("Error: Poligon #" & CStr(iPgonID) & " was not found")
					End If
				End If
			Next
		End Sub


		Private Function zzTestTempAllPlines() As Integer

			Dim colObjects As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks("BN2106,BN2301,BN2520")
			Return colObjects.Count

		End Function
		Public Function GetNodeLinksCorrected() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim colNodeLinksCorrected As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()
         Dim oPgon As tsPolygon

         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = GetPolygon(iPgonID)
            For Each oNodeLink As NodeLink In oPgon.GetNodeLinksCorrected()
               colNodeLinksCorrected.Add(oNodeLink)
            Next
         Next
         Return colNodeLinksCorrected
      End Function
      '/ Only TopoPgonToPolyline
      Public Sub CreatePgonDBPolyline(bWithIsthmuses As Boolean, iPgonID As Integer)

         Dim oPgon As tsPolygon
         DMAcadExt.AcadDocument.WriteDebugMessage("OK Topo: PoligonCount=" & CStr(mdicElements.PolygonIDs.Count) & "; Name=" & msName)

         oPgon = GetPolygon(iPgonID)
         If oPgon IsNot Nothing Then
            ' DMAcadExt.AcadDocument.WriteMessage("OK Before: Poligon #" & CStr(iPgonID))
            oPgon.CreateDBPolylineCorrected(bWithIsthmuses, False)
         Else
            DMAcadExt.AcadDocument.WriteMessage("Error: Poligon #" & CStr(iPgonID) & " was not found")
         End If



      End Sub
      Public Sub PrintInfo()
         DMAcadExt.AcadDocument.WriteMessage("----Topology: " & msName)
         mdicElements.PrintInfo()
      End Sub
      Public Sub MarkIsthmuses()

         mdicElements.MarkIsthmuses()
      End Sub


      Public Sub PrintInfo(iElementID As Integer)
         DMAcadExt.AcadDocument.WriteMessage("----Topology: " & msName)
         mdicElements.PrintInfo(iElementID)
      End Sub
      Public Sub Close()
         If mdicElements IsNot Nothing Then
            mdicElements.Clear()
            mdicElements.Close()
            mdicElements = Nothing
         End If

      End Sub
      Public Shared Function HashSetItems(hsIntegers As HashSet(Of Integer), bSorted As Boolean) As String
         Dim oEnum As IEnumerable(Of Integer)
         If bSorted Then
            Dim oList As List(Of Integer) = New List(Of Integer)(hsIntegers.Count)
            oList.AddRange(hsIntegers)
            oList.Sort()
            oEnum = oList
         Else

            oEnum = hsIntegers
         End If

         Dim sRes As String = String.Empty
         For Each iValue As Integer In oEnum
            If sRes.Length <> 0 Then
               sRes &= ","
            End If
            sRes &= CStr(iValue)
         Next
         Return sRes
      End Function
      Public Function GetDissolvedLinks(oEqualityComparer As IEqualityComparer(Of tsPolygon)) As ObjectIdCollection
         Dim oLeftPgon, oRightPgon As tsPolygon
         Dim oBranch As tsBranch

         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim iLeftPgonID, iRightPgonID As Integer
         Dim colResObjectIds As ObjectIdCollection = New ObjectIdCollection()
         For Each iBranchID As Integer In mdicElements.BrancheIDs
            oBranch = GetBranch(iBranchID)
            iLeftPgonID = oBranch.LeftPolygon
            oLeftPgon = GetPolygon(iLeftPgonID)
            iRightPgonID = oBranch.RightPolygon
            oRightPgon = GetPolygon(iRightPgonID)
            If iLeftPgonID = 0 OrElse iRightPgonID = 0 OrElse Not oEqualityComparer.Equals(oLeftPgon, oRightPgon) Then
               colResObjectIds.Add(oBranch.AcObjID)
            End If
         Next
         Return colResObjectIds
      End Function
      Public Function GetOuterBoundary() As ObjectIdCollection
         Dim oLeftPgon, oRightPgon As tsPolygon
         Dim oBranch As tsBranch

         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim iLeftPgonID, iRightPgonID As Integer
         Dim colResObjectIds As ObjectIdCollection = New ObjectIdCollection()
         For Each iBranchID As Integer In mdicElements.BrancheIDs
            oBranch = GetBranch(iBranchID)
            iLeftPgonID = oBranch.LeftPolygon
            oLeftPgon = GetPolygon(iLeftPgonID)
            iRightPgonID = oBranch.RightPolygon
            oRightPgon = GetPolygon(iRightPgonID)
            If iLeftPgonID = 0 OrElse iRightPgonID = 0 Then
               colResObjectIds.Add(oBranch.AcObjID)
            End If
         Next
         Return colResObjectIds
      End Function
      Public Function GetOuterVertexArray() As GeoUtilites.BulgeVertexArray
         Dim oLeftPgon, oRightPgon As tsPolygon
         Dim oBranch As tsBranch

         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim iLeftPgonID, iRightPgonID As Integer
         Dim colResObjectIds As ObjectIdCollection = New ObjectIdCollection()
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
         For Each iBranchID As Integer In mdicElements.BrancheIDs
            oBranch = GetBranch(iBranchID)
            iLeftPgonID = oBranch.LeftPolygon
            oLeftPgon = GetPolygon(iLeftPgonID)
            iRightPgonID = oBranch.RightPolygon
            oRightPgon = GetPolygon(iRightPgonID)
            If iLeftPgonID = 0 OrElse iRightPgonID = 0 Then
               colResObjectIds.Add(oBranch.AcObjID)

               oBulgeVertexArray.AddCurve(oBranch.AcObjID, True)
            End If
         Next
         Return oBulgeVertexArray
      End Function
      Public Function GetDissolvedLinks(colPgonIDs As System.Collections.Generic.ICollection(Of Integer)) As ObjectIdCollection
         Dim oPgon As tsPolygon
         Dim oBranch As tsBranch
         Dim iOtherPgon As Integer
         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         For Each iPgonID As Integer In colPgonIDs
            hsPgonIDs.Add(iPgonID)
         Next
         '   For Each iPgonID As Integer In colPgonIDs
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = mdicElements.GetPolygon(iPgonID)
            For Each iBranchID As Integer In oPgon.Branches
               oBranch = mdicElements.GetBranch(iBranchID)
               iOtherPgon = oBranch.GetOtherPolygon(iPgonID)
               If iOtherPgon <> -1 AndAlso Not hsPgonIDs.Contains(iOtherPgon) Then
                  colRes.Add(oBranch.AcObjID)
               End If
            Next
            For Each iBranchID As Integer In oPgon.Isthmuses
               oBranch = mdicElements.GetBranch(iBranchID)
               'MessageBox.Show(CStr(iBranchID), "04_794")
               colRes.Add(oBranch.AcObjID)
            Next
         Next
         '   MessageBox.Show(CStr(colRes.Count), "04_230N")
         Return colRes
      End Function
		Public Function GetBranchesObjIDs(hsBranches As HashSet(Of Integer)) As ObjectIdCollection
			Dim oBranch As tsBranch

			Dim colResObjIDs As ObjectIdCollection = New ObjectIdCollection()
			For Each iBranch As Integer In hsBranches
				oBranch = GetBranch(iBranch)

				colResObjIDs.Add(oBranch.AcObjID)
			Next
			Return colResObjIDs
		End Function
		Public Function GetIsthmusObjIDs(hsBranches As HashSet(Of Integer), hsNewBranches As HashSet(Of Integer)) As ObjectIdCollection
			Dim oBranch As tsBranch

			Dim colResObjIDs As ObjectIdCollection = New ObjectIdCollection()
			For Each iBranch As Integer In hsBranches
				oBranch = GetBranch(iBranch)
				If oBranch.IsIsthmus Then
					colResObjIDs.Add(oBranch.AcObjID)
				End If
			Next
			For Each iBranch As Integer In hsNewBranches
				oBranch = GetBranch(iBranch)
				If oBranch.IsIsthmus Then
					colResObjIDs.Add(oBranch.AcObjID)
				End If
			Next
			Return colResObjIDs
		End Function

		Public Sub RefreshBranches()
         Dim oEntity As Entity
         Dim colResObjIDs As ObjectIdCollection = New ObjectIdCollection()

         For Each oElement As tsElement In mdicElements.Values
            If oElement.ElemType = TopoElemType.Branch Then
               oEntity = DMAcadExt.AcadTransaction.GetEntity(oElement.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) '  oElement.AcObjID
               oElement.Layer = oEntity.Layer
            End If

         Next

      End Sub
      Public Function GetDissolvedLinksBy(hsUnion As HashSet(Of Integer), ByRef colDissolvedLinks As ObjectIdCollection, ByRef colDissolvedNodes As ObjectIdCollection, ByRef colOverlappingLinks As ObjectIdCollection) As Boolean
         Dim oPgon As tsPolygon
         Dim hsDissolve As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim hsOverlapping As HashSet(Of Integer) = New HashSet(Of Integer)

         Dim hsPgonBranches As HashSet(Of Integer)
         Dim hsCurrentOverlapping As HashSet(Of Integer)

         colDissolvedLinks = New ObjectIdCollection()
         colOverlappingLinks = New ObjectIdCollection()


         '  Dim iPgonID As Integer
         For Each iPgonID As Integer In hsUnion
            oPgon = mdicElements.GetPolygon(iPgonID)
            If oPgon Is Nothing Then
               System.Windows.Forms.MessageBox.Show(CStr("oPgon Is Nothing") & vbCrLf & iPgonID.ToString() & vbCrLf & hsUnion.Count.ToString() & vbCrLf & msName, "04_352")
            Else
               '  System.Windows.Forms.MessageBox.Show(iPgonID.ToString() & vbCrLf & hsUnion.Count.ToString() & vbCrLf & msName, "04_363")

            End If
            hsCurrentOverlapping = New HashSet(Of Integer)(hsDissolve)
            hsPgonBranches = oPgon.Branches
            hsDissolve.SymmetricExceptWith(hsPgonBranches)
            hsCurrentOverlapping.IntersectWith(hsPgonBranches)
            hsOverlapping.UnionWith(hsCurrentOverlapping)

         Next
         colDissolvedLinks = zzBranches2ObjectIDCol(hsDissolve)
         colOverlappingLinks = zzBranches2ObjectIDCol(hsOverlapping)
         colDissolvedNodes = zzGetBranchesNodes(hsDissolve)
         Return True
      End Function
      Public Function NewPgonUnion() As tsPgonUnion
         Return New tsPgonUnion(mdicElements)
      End Function
      Private Function zzBranches2ObjectIDCol(hsBranches As HashSet(Of Integer)) As ObjectIdCollection
         Dim oBranch As tsBranch
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         For Each iBranchID As Integer In hsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
            ' MessageBox.Show(CStr(iBranchID), "04_790")
            If Not colRes.Contains(oBranch.AcObjID) Then
               colRes.Add(oBranch.AcObjID)
            End If

         Next
         Return colRes
      End Function
      Private Function zzGetBranchesNodes(hsBranches As HashSet(Of Integer)) As ObjectIdCollection
         Dim oBranch As tsBranch
         Dim hsNodes As HashSet(Of Integer) = New HashSet(Of Integer)()
         Dim iNodeID As Integer
         Dim oNode As tsNode
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         For Each iBranchID As Integer In hsBranches
            For iIndex As Integer = 0 To 1
               oBranch = mdicElements.GetBranch(iBranchID)
               If iIndex = 0 Then
                  iNodeID = oBranch.PreviousNodeID
               Else
                  iNodeID = oBranch.NextNodeID
               End If
               If Not hsNodes.Contains(iNodeID) Then
                  hsNodes.Add(iNodeID)
                  oNode = GetNode(iNodeID)
                  If Not oNode.AcObjID.IsNull Then
                     colRes.Add(oNode.AcObjID)
                  End If
               End If
            Next
         Next
         Return colRes
      End Function
      Public Function GetDissolvedLinksByFromBU(colPgonIDs As System.Collections.Generic.ICollection(Of Integer), ByRef dicInternal As Dictionary(Of UD_ParcelKey, HashSet(Of Integer)), bAllLinks As Boolean) As ObjectIdCollection
         Dim oPgon As tsPolygon
         Dim oBranch As tsBranch
         Dim iOtherPgonID As Integer
         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim colInternal As ObjectIdCollection = New ObjectIdCollection()
         Dim hsCurrentInternal As HashSet(Of Integer) = Nothing
         Dim tThisPgonKey As UD_ParcelKey
         Dim bThisPgonKeyExists As Boolean

         Dim tOtherPgonKey As UD_ParcelKey
			'  Dim oInternalCurve As Entity
			dicInternal = New Dictionary(Of UD_ParcelKey, HashSet(Of Integer))
         ' Dim c As HashSet(Of Integer)
         If mdicTopoIDByStage Is Nothing Then
            '  System.Windows.Forms.MessageBox.Show("mdicTopoIDByStage Is NOTHING", "04_333")
         Else
            '  System.Windows.Forms.MessageBox.Show(mdicTopoIDByStage.Count.ToString() & vbCrLf & colPgonIDs.Count.ToString(), "04_339")
         End If
         '     
         For Each iPgonID As Integer In colPgonIDs
            hsPgonIDs.Add(iPgonID)
         Next

         Dim iTest As Integer = 0
         '    For Each iPgonID As Integer In colPgonIDs
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = mdicElements.GetPolygon(iPgonID)
            If oPgon Is Nothing Then
               System.Windows.Forms.MessageBox.Show(CStr("oPgon Is Nothing") & vbCrLf & iPgonID.ToString() & vbCrLf & msName, "04_352")
            End If
            iTest += 1
            ' tThisPgonKey = mdicTopoIDByStage.Item(iPgonID)
            bThisPgonKeyExists = mdicTopoIDByStage.TryGetValue(iPgonID, tThisPgonKey)
            'System.Windows.Forms.MessageBox.Show(CStr(oPgon.Branches Is Nothing) & vbCrLf & iPgonID.ToString() & vbCrLf & iTest.ToString(), "04_348")
            If bAllLinks OrElse bThisPgonKeyExists Then
               For Each iBranchID As Integer In oPgon.Branches
                  oBranch = mdicElements.GetBranch(iBranchID)
                  iOtherPgonID = oBranch.GetOtherPolygon(iPgonID)
                  If iOtherPgonID = 0 OrElse iPgonID = iOtherPgonID Then ''''''''''''''''''''''''''''''''''''''''TEMP
                     If Not colRes.Contains(oBranch.AcObjID) Then
                        colRes.Add(oBranch.AcObjID)
                     End If
                  ElseIf iOtherPgonID <> -1 Then
                     If bThisPgonKeyExists AndAlso mdicTopoIDByStage.TryGetValue(iOtherPgonID, tOtherPgonKey) Then
                        '   MessageBox.Show(iOtherPgonID.ToString() & ":" & tOtherPgonKey.ToString(), "04_230N")
                        If tThisPgonKey <> tOtherPgonKey Then
                           If Not colRes.Contains(oBranch.AcObjID) Then
                              colRes.Add(oBranch.AcObjID)
                           End If

                        Else
                           If Not dicInternal.TryGetValue(tThisPgonKey, hsCurrentInternal) Then
                              hsCurrentInternal = New HashSet(Of Integer)()
                              dicInternal.Add(tThisPgonKey, hsCurrentInternal)
                           End If
                           hsCurrentInternal.Add(oBranch.ID)
                           colInternal.Add(oBranch.AcObjID)
                           'oInternalCurve = DMAcadExt.AcadTransaction.GetEntity(oBranch.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           ' oInternalCurve.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.GreenYellow)
                        End If
                     Else
                        'MessageBox.Show(CStr(iBranchID), "04_772")
                        If Not colRes.Contains(oBranch.AcObjID) Then
                           colRes.Add(oBranch.AcObjID)
                        End If

                     End If
                  End If
               Next
               '  System.Windows.Forms.MessageBox.Show("", "04_351")
               For Each iBranchID As Integer In oPgon.Isthmuses
                  oBranch = mdicElements.GetBranch(iBranchID)
                  ' MessageBox.Show(CStr(iBranchID), "04_790")
                  If Not colRes.Contains(oBranch.AcObjID) Then
                     colRes.Add(oBranch.AcObjID)
                  End If

               Next
            End If
         Next
         '  MessageBox.Show(CStr(colRes.Count) & ":" & CStr(colInternal.Count), "04_230N")
         Return colRes
      End Function

      Public Function GetDissolvedLinksBy(colPgonIDs As System.Collections.Generic.ICollection(Of Integer), ByRef dicInternal As Dictionary(Of UD_ParcelKey, HashSet(Of Integer)), bAllLinks As Boolean) As ObjectIdCollection
         Dim oPgon As tsPolygon
         Dim oBranch As tsBranch
         Dim iOtherPgonID As Integer
         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim colInternal As ObjectIdCollection = New ObjectIdCollection()
         Dim hsCurrentInternal As HashSet(Of Integer) = Nothing
         Dim tThisPgonKey As UD_ParcelKey
         Dim bThisPgonKeyExists As Boolean

         Dim tOtherPgonKey As UD_ParcelKey
         Dim oInternalCurve As Entity
         dicInternal = New Dictionary(Of UD_ParcelKey, HashSet(Of Integer))
         ' Dim c As HashSet(Of Integer)
         If mdicTopoIDByStage Is Nothing Then
            '  System.Windows.Forms.MessageBox.Show("mdicTopoIDByStage Is NOTHING", "04_333")
         Else
            '  System.Windows.Forms.MessageBox.Show(mdicTopoIDByStage.Count.ToString() & vbCrLf & colPgonIDs.Count.ToString(), "04_339")
         End If
         '     
         For Each iPgonID As Integer In colPgonIDs
            hsPgonIDs.Add(iPgonID)
         Next

         Dim iTest As Integer = 0
         '    For Each iPgonID As Integer In colPgonIDs
         For Each iPgonID As Integer In mdicElements.PolygonIDs
            oPgon = mdicElements.GetPolygon(iPgonID)
            If oPgon Is Nothing Then
               System.Windows.Forms.MessageBox.Show(CStr("oPgon Is Nothing") & vbCrLf & iPgonID.ToString() & vbCrLf & msName, "04_352")
            End If
            iTest += 1
            ' tThisPgonKey = mdicTopoIDByStage.Item(iPgonID)
            bThisPgonKeyExists = mdicTopoIDByStage.TryGetValue(iPgonID, tThisPgonKey)
            'System.Windows.Forms.MessageBox.Show(CStr(oPgon.Branches Is Nothing) & vbCrLf & iPgonID.ToString() & vbCrLf & iTest.ToString(), "04_348")
            If bAllLinks OrElse bThisPgonKeyExists Then
               For Each iBranchID As Integer In oPgon.Branches
                  oBranch = mdicElements.GetBranch(iBranchID)
                  iOtherPgonID = oBranch.GetOtherPolygon(iPgonID)
                  If iOtherPgonID = 0 OrElse iPgonID = iOtherPgonID Then ''''''''''''''''''''''''''''''''''''''''TEMP
                     If Not colRes.Contains(oBranch.AcObjID) Then
                        colRes.Add(oBranch.AcObjID)
                     End If
                  ElseIf iOtherPgonID <> -1 Then
                     If bThisPgonKeyExists AndAlso mdicTopoIDByStage.TryGetValue(iOtherPgonID, tOtherPgonKey) Then
                        '   MessageBox.Show(iOtherPgonID.ToString() & ":" & tOtherPgonKey.ToString(), "04_230N")
                        If tThisPgonKey <> tOtherPgonKey Then
                           If Not colRes.Contains(oBranch.AcObjID) Then
                              colRes.Add(oBranch.AcObjID)
                           End If

                        Else
                           If Not dicInternal.TryGetValue(tThisPgonKey, hsCurrentInternal) Then
                              hsCurrentInternal = New HashSet(Of Integer)()
                              dicInternal.Add(tThisPgonKey, hsCurrentInternal)
                           End If

                           hsCurrentInternal.Add(oBranch.ID)
                           colInternal.Add(oBranch.AcObjID)


                           'oInternalCurve = DMAcadExt.AcadTransaction.GetEntity(oBranch.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           ' oInternalCurve.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.GreenYellow)
                        End If
                     Else
                        'MessageBox.Show(CStr(iBranchID), "04_772")
                        If Not colRes.Contains(oBranch.AcObjID) Then
                           colRes.Add(oBranch.AcObjID)
                        End If

                     End If
                  End If
               Next
               If oPgon.Isthmuses.Count <> 0 Then
                  '  DMCommon.Debug.MsgBox("09_766", msName, oPgon.ID, oPgon.Isthmuses.Count)
               End If
               For Each iBranchID As Integer In oPgon.Isthmuses
                  oBranch = mdicElements.GetBranch(iBranchID)
                  ' MessageBox.Show(CStr(iBranchID), "04_790")
                  If Not colRes.Contains(oBranch.AcObjID) Then
                     colRes.Add(oBranch.AcObjID)
                  End If

               Next
            End If
         Next
         '  MessageBox.Show(CStr(colRes.Count) & ":" & CStr(colInternal.Count), "04_230N")
         Return colRes
      End Function
      Public Function GetDissolvedLinksBy(colPgonIDs As System.Collections.Generic.ICollection(Of Integer)) As ObjectIdCollection
         Dim oPgon As tsPolygon
         Dim oBranch As tsBranch
         Dim iOtherPgonID As Integer
         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         Dim colInternal As ObjectIdCollection = New ObjectIdCollection()

         Dim tPgonKey As UD_ParcelKey
         Dim tOtherPgonKey As UD_ParcelKey
         Dim oInternalCurve As Entity
         For Each iPgonID As Integer In colPgonIDs
            hsPgonIDs.Add(iPgonID)
         Next
         For Each iPgonID As Integer In colPgonIDs
            oPgon = mdicElements.GetPolygon(iPgonID)
            tPgonKey = mdicTopoIDByStage.Item(iPgonID)
            For Each iBranchID As Integer In oPgon.Branches
               oBranch = mdicElements.GetBranch(iBranchID)
               iOtherPgonID = oBranch.GetOtherPolygon(iPgonID)
               If iOtherPgonID = 0 OrElse iPgonID = iOtherPgonID Then
                  colRes.Add(oBranch.AcObjID)
               ElseIf iOtherPgonID <> -1 Then
                  If mdicTopoIDByStage.TryGetValue(iOtherPgonID, tOtherPgonKey) Then
                     If tPgonKey <> tOtherPgonKey Then
                        colRes.Add(oBranch.AcObjID)
                     Else
                        colInternal.Add(oBranch.AcObjID)
                        oInternalCurve = DMAcadExt.AcadTransaction.GetEntity(oBranch.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                        oInternalCurve.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.IndianRed)
                     End If
                  Else
                     'MessageBox.Show(CStr(iBranchID), "04_772")
                     colRes.Add(oBranch.AcObjID)
                  End If
               End If
            Next
            For Each iBranchID As Integer In oPgon.Isthmuses
               oBranch = mdicElements.GetBranch(iBranchID)
               ' MessageBox.Show(CStr(iBranchID), "04_790")
               colRes.Add(oBranch.AcObjID)
            Next
         Next
         MessageBox.Show(CStr(colRes.Count) & ":" & CStr(colInternal.Count), "04_230N")
         Return colRes
      End Function
      Public Function GetDissolvedLinksByDic(colPgonIDs As System.Collections.Generic.ICollection(Of Integer)) As ObjectIdCollection
         Dim oPgon As tsPolygon
         Dim oBranch As tsBranch
         Dim iOtherPgonID As Integer
         Dim hsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)
         Dim colRes As ObjectIdCollection = New ObjectIdCollection()
         '     Dim tPgonKey As UD_ParcelKey
         '   Dim tOtherPgonKey As UD_ParcelKey
         Dim oPgonAttribute As IEqualityComparer
         Dim oOtherPgonAttribute As IEqualityComparer = Nothing


         For Each iPgonID As Integer In colPgonIDs
            hsPgonIDs.Add(iPgonID)
         Next
         For Each iPgonID As Integer In colPgonIDs
            oPgon = mdicElements.GetPolygon(iPgonID)
            oPgonAttribute = mdicAttributes.Item(iPgonID)
            For Each iBranchID As Integer In oPgon.Branches
               oBranch = mdicElements.GetBranch(iBranchID)
               iOtherPgonID = oBranch.GetOtherPolygon(iPgonID)
               If iOtherPgonID = 0 OrElse iPgonID = iOtherPgonID Then
                  colRes.Add(oBranch.AcObjID)
               ElseIf iOtherPgonID <> -1 Then
                  If mdicAttributes.TryGetValue(iOtherPgonID, oOtherPgonAttribute) Then
                     If oPgonAttribute IsNot oOtherPgonAttribute Then
                        colRes.Add(oBranch.AcObjID)
                     End If
                  Else
                     colRes.Add(oBranch.AcObjID)
                  End If
               End If
            Next
            For Each iBranchID As Integer In oPgon.Isthmuses
               oBranch = mdicElements.GetBranch(iBranchID)
               'MessageBox.Show(CStr(iBranchID), "04_790")
               colRes.Add(oBranch.AcObjID)
            Next
         Next
         MessageBox.Show(CStr(colRes.Count), "04_230N")
         Return colRes
      End Function
      Public Function GetNodesByLinks(colLinks As ObjectIdCollection) As HashSet(Of Integer)
         Dim hsNodes As HashSet(Of Integer) = New HashSet(Of Integer)()
         Dim oBranch As tsBranch
         For Each tAcObjID As ObjectId In colLinks
            oBranch = mdicElements.GetBranch(tAcObjID)
            If oBranch IsNot Nothing Then
               If Not hsNodes.Contains(oBranch.PreviousNodeID) Then
                  hsNodes.Add(oBranch.PreviousNodeID)
               End If
               If Not hsNodes.Contains(oBranch.NextNodeID) Then
                  hsNodes.Add(oBranch.NextNodeID)
               End If
            End If
         Next
         Return hsNodes
      End Function
      Public Function BranchesToChains(hsBranches As HashSet(Of Integer)) As System.Collections.ObjectModel.Collection(Of tsBranchChain)
         Dim oBranch As tsBranch
         Dim oChain As tsBranchChain = Nothing
         Dim oNewChain As tsBranchChain
         Dim dicChains As Dictionary(Of Integer, tsBranchChain) = New Dictionary(Of Integer, tsBranchChain)()
         For Each iBranchID As Integer In hsBranches
            oBranch = GetBranch(iBranchID)
            oNewChain = New tsBranchChain(oBranch)
            If dicChains.TryGetValue(oNewChain.PreviousNodeID, oChain) Then
               oChain.UnionWith(oNewChain, oNewChain.PreviousNodeID)
               dicChains.Remove(oNewChain.PreviousNodeID)
               dicChains.Add(oNewChain.NextNodeID, oChain)
            ElseIf dicChains.TryGetValue(oNewChain.NextNodeID, oChain) Then
               oChain.UnionWith(oNewChain, oNewChain.NextNodeID)
               dicChains.Remove(oNewChain.NextNodeID)
               dicChains.Add(oNewChain.PreviousNodeID, oChain)
            Else
               dicChains.Add(oNewChain.PreviousNodeID, oNewChain)
               dicChains.Add(oNewChain.NextNodeID, oNewChain)
            End If
         Next
         Dim colRes As System.Collections.ObjectModel.Collection(Of tsBranchChain) = New ObjectModel.Collection(Of tsBranchChain)()
         For Each iKey As Integer In dicChains.Keys
            oChain = dicChains.Item(iKey)
            If oChain.PreviousNodeID = iKey Then
               colRes.Add(oChain)
            End If
         Next
         Return colRes
      End Function

		Public ReadOnly Property Elements() As tsElements
			Get
				Return mdicElements
			End Get
		End Property
		Public ReadOnly Property PolygonBorders() As ObjectIdCollection
			Get
				Return mdicPolygonBorders
			End Get
		End Property

		Public Property TopoIDByStage() As Dictionary(Of Integer, UD_ParcelKey)
         Get
            Return mdicTopoIDByStage
         End Get
         Set(dicValue As Dictionary(Of Integer, UD_ParcelKey))
            mdicTopoIDByStage = dicValue
         End Set
      End Property
      Public Sub SetDissolveDictionary(dicAttributes As IDictionary(Of Integer, IEquatable(Of Object)))
         mdicAttributesA = dicAttributes
      End Sub
      Public Property SimplexLink As Boolean
         Get
            Return mdicElements.SimplexLink
         End Get
         Set(bValue As Boolean)
            mdicElements.SimplexLink = bValue
         End Set
      End Property

      Public Sub AddPolygon(ByVal oPolygon As tsPolygon)
         Dim oaRings() As tsRing = oPolygon.Rings
         zzAddElement(oPolygon)
         For iIndex As Integer = 0 To oPolygon.RingsUB
            zzAddRing(oaRings(iIndex))
         Next
      End Sub
      Public Function GetElement(ByVal iID As Integer) As tsElement
         Dim oElement As tsElement = Nothing
         If mdicElements.TryGetValue(iID, oElement) Then
            Return oElement
         Else
            Return Nothing
         End If
      End Function
      Public Function GetPolygon(ByVal iID As Integer) As tsPolygon
         Dim oElement As tsElement = Nothing
         If mdicElements.TryGetValue(iID, oElement) Then
            If oElement.ElemType = TopoElemType.Polygon Then
               Return DirectCast(oElement, tsPolygon)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function
      Public Function GetNode(ByVal iID As Integer) As tsNode
         Dim oElement As tsElement = Nothing
         If mdicElements.TryGetValue(iID, oElement) Then
            If oElement.ElemType = TopoElemType.Node Then
               Return DirectCast(oElement, tsNode)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function

      Public Function GetBranch(ByVal tAcObjID As ObjectId) As tsBranch

         Return mdicElements.GetBranch(tAcObjID)

      End Function
      Public Sub SetLayer(colAcadObjIDs As ObjectIdCollection, ByVal sLayer As String)
			Dim oBranch As tsBranch
			Dim sDebug As String
			Dim sDebugException As String

			For Each tAcObjID As ObjectId In colAcadObjIDs
				sDebugException = "None"
				oBranch = GetBranch(tAcObjID)
				sDebug = oBranch.Layer
				Try
					oBranch.Layer = sLayer
				Catch oEx As Exception
					sDebugException = oEx.Message
				End Try

			Next



		End Sub
      Public Function GetBranch(ByVal iID As Integer) As tsBranch
         Dim oElement As tsElement = Nothing
         If mdicElements.TryGetValue(iID, oElement) Then
            If oElement.ElemType = TopoElemType.Branch Then
               Return DirectCast(oElement, tsBranch)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function

      Public Sub UpdateAcadLines()
         Dim oBranch As tsBranch
         Dim oPreviousNode, oNextNode As tsNode
         Dim tAcObjID As ObjectId

         For Each oElement As tsElement In mdicElements.Values
            If oElement.ElemType = TopoElemType.Branch Then
               oBranch = DirectCast(oElement, tsBranch)
               oPreviousNode = Me.GetNode(oBranch.PreviousNodeID)
               oNextNode = Me.GetNode(oBranch.NextNodeID)

               tAcObjID = oElement.AcObjID
            End If
         Next
      End Sub
      Private Sub zzAddRing(ByVal oRing As tsRing)
         Dim oaNodes() As tsNode = oRing.Nodes
         Dim oaBranches() As tsBranch = oRing.Branches

         For iIndex As Integer = 0 To oRing.ElementsUB
            zzAddElement(oaNodes(iIndex))
            zzAddElement(oaBranches(iIndex))
         Next
      End Sub

      Private Sub zzAddElement(ByVal oElement As tsElement)
         If Not mdicElements.ContainsKey(oElement.ID) Then
            mdicElements.Add(oElement.ID, oElement)
         End If
      End Sub
   End Class
End Namespace

