Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme



   Public Class tsPolygon
      Inherits tsElement
      Protected mdicElements As tsElements
      Protected miRingsUB As Integer = -1
      Protected moaRings() As tsRing
      Protected mhsBranches As HashSet(Of Integer)
      Private mhsIsthmuses As HashSet(Of Integer)
      '   Private mcolIsthmusChains As System.Collections.ObjectModel.Collection(Of tsBranchChain)
      Private mdicIsthmusChains As Dictionary(Of Integer, tsBranchChain)
      Private mhsInnerPolygons As HashSet(Of Integer) = New HashSet(Of Integer)()

      Private mhsConnectionPoints As HashSet(Of Integer)
      Private mcolComplexNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
      Private mdAreaCorrected As Double
      Private moCentroidData As DMAcadExt.BlockRefData
      Private mdArea As Double
      Private mdPerimeter As Double
      Private mtCentroid As Autodesk.AutoCAD.Geometry.Point3d
      Private msCentroidLayer As String
      Private mtBorderObjID As ObjectId
      Private mtBorderHandle As Handle
		Private mbIsInner As Boolean


		Public Sub New(iPolygonID As Integer, ByRef dicElements As tsElements)
         MyBase.New(iPolygonID, TopoElemType.Polygon)
         mdicElements = dicElements
         mhsIsthmuses = New HashSet(Of Integer)()
         mhsBranches = New HashSet(Of Integer)()
      End Sub
      Public Sub Load(ByVal oPgon As Polygon, bCheckExtend As Boolean)
         Dim colRings As RingCollection '= oPgon.GetBoundary()
         Dim tCenterAcObjID As ObjectId
         Dim oRing As Ring
         Dim hsRingBranches As HashSet(Of Integer)
         Try
            mdArea = oPgon.Area
            mdPerimeter = oPgon.Perimeter
            mtCentroid = oPgon.Centroid
            colRings = oPgon.GetBoundary()
            miRingsUB = colRings.Count - 1
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, oMapEx.StackTrace, oMapEx.StackTrace & vbCrLf & "1:tsPolygon-Load")
            Return
         End Try

			Try
				tCenterAcObjID = oPgon.Entity
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!IsActive", DMAcadExt.AcadTransaction.IsActive, tCenterAcObjID)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Layer", msCentroidLayer)
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "3:tsPolygon-Load")
				Return
			End Try
			If tCenterAcObjID.IsValid Then
				msCentroidLayer = DMAcadExt.AcadTransaction.GetLayer(tCenterAcObjID)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Layer", msCentroidLayer)
				MyBase.AcObjID = tCenterAcObjID


				moCentroidData = mdicElements.LoadCentroidData(MyBase.AcObjID)
				ReDim moaRings(miRingsUB)
				For iRingIndex As Integer = 0 To miRingsUB

					moaRings(iRingIndex) = New tsRing(diID, mdicElements)
					oRing = colRings.Item(iRingIndex)
					'SUDA


					moaRings(iRingIndex).Load(oRing, iRingIndex)


					If moaRings(iRingIndex).InnerPolygons.Count <> 0 Then
						mhsInnerPolygons.UnionWith(moaRings(iRingIndex).InnerPolygons)
					End If
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "Pgon Load", diID, bCheckExtend, mdicElements.SimplexLink)
					If bCheckExtend Then
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Err02", diID, Centroid.X & "," & Centroid.Y, miRingsUB)
						moaRings(iRingIndex).CheckExtend()
					Else

						If mdicElements.SimplexLink Then
							moaRings(iRingIndex).CalcExterior()
						End If

					End If

					hsRingBranches = moaRings(iRingIndex).BranchSet
					If hsRingBranches IsNot Nothing Then
						mhsBranches.UnionWith(hsRingBranches)
					End If

					If mdicElements.CentroidAcadBlock IsNot Nothing Then
						moCentroidData = mdicElements.CentroidAcadBlock.GetBlockRefData(AcObjID)
					End If

				Next
			End If
			If tsTopology.MapObjectsDispose Then
            colRings.Dispose()
         End If

         colRings = Nothing
         '   DMAcadExt.AcadDocument.WriteDebugMessage("before Isthmuses= " & mhsBranches.Count.ToString())
         '     For iIsthmusIndex As Integer = 0 To mhsIsthmuses.Count - 1
         'mhsBranches.Add(iIsthmusIndex)
         '   Next

         ' oPgon.Entity
      End Sub
      Public ReadOnly Property Rings() As tsRing()
         Get
            Return moaRings
         End Get
      End Property
      Public ReadOnly Property ExteriorRing() As tsRing
         Get
            Return moaRings(0)
         End Get
      End Property
      Public ReadOnly Property TopoName() As String
         Get
            Return mdicElements.TopoName
         End Get
      End Property
      Public ReadOnly Property RingsUB() As Integer
         Get
            Return miRingsUB
         End Get
      End Property
		Public Property IsInner() As Boolean
			Get
				Return mbIsInner
			End Get
			Set(iValue As Boolean)
				mbIsInner = iValue
			End Set
		End Property
		Public Function GetVectorSet() As DMAcadExt.IUD_Link()
			'Dim colPoints As System.Collections.ObjectModel.Collection(Of Integer)

			Dim oBranch As tsBranch
			'Dim bIsBranchSameDir As Boolean
			Dim oLink As DMAcadExt.IUD_Link
         Dim oaLinks(mhsBranches.Count - 1) As DMAcadExt.IUD_Link
         Dim oResBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
         '  MessageBox.Show(CStr(hsBranches.Count), "04_588")
         Dim iIndex As Integer = 0
         For Each iBranchID As Integer In mhsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
				'    System.Windows.Forms.MessageBox.Show(oBranch.AcObjID.ToString(), "09_878k")
				oLink = tsBranch.GetLink(oBranch.AcObjID, True) ' 17/06 'bIsBranchSameDir

				'	DMAcadExt.AcadDocument.WriteDebugMessage("T/S " & oLink.GetInfo(4))
				oaLinks(iIndex) = oLink
				iIndex += 1
         Next
         Return oaLinks


      End Function
      Public Function CopyRings() As tsRing()
         Dim oaRings(moaRings.GetUpperBound(0)) As tsRing
         For iIndex As Integer = 0 To moaRings.GetUpperBound(0)
            oaRings(iIndex).CopyFrom(moaRings(iIndex))
         Next
         Return oaRings
      End Function
      Public ReadOnly Property Branches As HashSet(Of Integer)
         Get
            Return mhsBranches
         End Get
      End Property
		Public ReadOnly Property Isthmuses As HashSet(Of Integer)
			Get
				Return mhsIsthmuses
			End Get
		End Property
		Public ReadOnly Property BoundaryBranches As HashSet(Of Integer)
			Get
				Dim hsBranchesCopy As HashSet(Of Integer) = New HashSet(Of Integer)(mhsBranches)
				hsBranchesCopy.ExceptWith(mhsIsthmuses)
				Return hsBranchesCopy
			End Get
		End Property
		Public ReadOnly Property CentroidData As DMAcadExt.BlockRefData
         Get
            Return moCentroidData
         End Get
      End Property
      Public Sub AddIsthmus(oBranch As tsBranch)

         mhsIsthmuses.Add(oBranch.ID)
         mhsBranches.Add(oBranch.ID)

      End Sub
      Public Sub CalcIsthmus()
         '   Me.moaRings(0)
         Dim oBranch As tsBranch
         Dim oChain As tsBranchChain
         Dim iThisNodeID As Integer
         Dim iOtherNodeID As Integer
			' Dim iNextNodeID As Integer
			Dim bDir As Boolean

         Dim iNextBranchID As Integer
         Dim oNextBranch As tsBranch
         Dim oOtherNode As tsNode
         Dim oThisNode As tsNode

         Dim iNewChainIndex As Integer = 0
         mdicIsthmusChains = New Dictionary(Of Integer, tsBranchChain)()
			If mhsIsthmuses IsNot Nothing AndAlso mhsIsthmuses.Count <> 0 Then

				'  DMCommon.Debug.MsgBox("08_202", mhsIsthmuses.Count)
				'DMCommon.ExcelLogAW5.SetNextValue(0, "Pgon", ID, mtCentroid, mhsIsthmuses.Count)
				For Each iBranchID As Integer In mhsIsthmuses
					'   DMCommon.Debug.MsgBox("08_180", Me.ID, iBranchID, mhsIsthmuses.Count, DMCommon.Debug.ColCount(Me.moaRings(0).NodeSet))
					'   DMAcadExt.AcadDocument.WriteDebugMessage("08_180 " & Me.ID & "; " & iBranchID & "; " & mhsIsthmuses.Count)
					oChain = Nothing
					oBranch = Me.mdicElements.GetBranch(iBranchID)
					'	DMCommon.ExcelLogAW5.SetNextValue(2, "Branch", ID, oBranch.ID, oBranch.ChainIndex)
					'  DMCommon.Debug.MsgBox("08_180k", oBranch, oBranch.ChainIndex)
					If oBranch.ChainIndex = -1 Then

						If Me.moaRings(0).NodeSet.Contains(oBranch.PreviousNodeID) Then

							iThisNodeID = oBranch.PreviousNodeID
							oChain = New tsBranchChain(iNewChainIndex, iThisNodeID, oBranch, True)
							iOtherNodeID = oBranch.NextNodeID
						ElseIf Me.moaRings(0).NodeSet.Contains(oBranch.NextNodeID) Then

							iThisNodeID = oBranch.NextNodeID
							oChain = New tsBranchChain(iNewChainIndex, iThisNodeID, oBranch, False)
							iOtherNodeID = oBranch.PreviousNodeID
						Else

						End If

						If oChain IsNot Nothing Then
							' DMCommon.Debug.MsgBox("08_181", oChain IsNot Nothing, iNewChainIndex, iBranchID, iThisNodeID, iOtherNodeID)
							oThisNode = mdicElements.GetNode(iThisNodeID)
							oChain.ExteriorRingPoint = oThisNode.Location
							'  DMAcadExt.AcadDocument.WriteDebugMessage("08_181b " & iThisNodeID & "; " & oThisNode.InnerRing & "; ")
							'  DMAcadExt.AcadDocument.WriteDebugMessage("08_181 " & Me.moaRings(0).RingNumber & "; " & iBranchID & "; " & oBranch.PreviousNodeID & "; " & oBranch.NextNodeID & "; " & iOtherNodeID & "; iNewChainIndex= " & iNewChainIndex)
							oBranch.ChainIndex = iNewChainIndex

							Do
								oOtherNode = mdicElements.GetNode(iOtherNodeID)



								'  DMAcadExt.AcadDocument.WriteDebugMessage("08_182 " & iBranchID & "; iOtherNodeID=" & iOtherNodeID & "; iOtherRing=" & oOtherNode.InnerRing & "; oOtherNode.Branches.Count= " & oOtherNode.Branches.Count)
								If oOtherNode.BranchCount = 2 Then

									iNextBranchID = oOtherNode.GetOtherBranch(iBranchID)

									oNextBranch = Me.mdicElements.GetBranch(iNextBranchID)
									oNextBranch.ChainIndex = iNewChainIndex
									If oNextBranch.PreviousNodeID = iOtherNodeID Then
										iOtherNodeID = oNextBranch.NextNodeID
										bDir = True
									ElseIf oNextBranch.NextNodeID = iOtherNodeID Then
										iOtherNodeID = oNextBranch.PreviousNodeID
										bDir = False
									End If
									'   iOtherNodeID = oNextBranch.GetOtherNode(iOtherNodeID)

									oChain.AddBranch(oNextBranch, bDir)
									iBranchID = iNextBranchID

									' DMAcadExt.AcadDocument.WriteDebugMessage("08_183 " & iBranchID & "; " & oOtherNode.Branches.Count)
								Else
									' DMAcadExt.AcadDocument.WriteDebugMessage("08_183K " & iBranchID & "; " & oOtherNode.Branches.Count & "; " & oOtherNode.InnerRing)
									oChain.InnerNodeID = iOtherNodeID
									oChain.InnerRingIndex = oOtherNode.InnerRing
									mdicIsthmusChains.Add(oChain.ExteriorNodeID, oChain)
									oOtherNode = mdicElements.GetNode(iOtherNodeID)
									oChain.InnerRingPoint = oOtherNode.Location

									'  DMAcadExt.AcadDocument.WriteDebugMessage("08_183L " & mdicIsthmusChains.Count)
									iNewChainIndex += 1
									Exit Do
								End If
							Loop
							' DMCommon.Debug.MsgBox("08_183a EndOfChain", oChain.Index, oChain.Count)
						End If

					End If
				Next
				'  DMCommon.Debug.MsgBox("08_120", Me.ID, mhsIsthmuses.Count, mdicIsthmusChains.Count)
				'   DMAcadExt.AcadDocument.WriteDebugMessage("08_184 " & Me.ID & "; " & mhsIsthmuses.Count & ";  mdicIsthmusChains.Count= " & mdicIsthmusChains.Count)
				For Each oResChain As tsBranchChain In mdicIsthmusChains.Values
					'   DMCommon.Debug.MsgBox("08_121", oResChain.ExteriorNodeID, oResChain.InnerRingIndex, oResChain.InnerNodeID)
					'   DMAcadExt.AcadDocument.WriteDebugMessage("08_185 " & oResChain.ExteriorNodeID & "; InnerRingIndex=" & oResChain.InnerRingIndex & "; oResChain.InnerNodeID= " & oResChain.InnerNodeID)
				Next
			End If
		End Sub
		Public Function CheckIslands(Optional iMapThemeID As DMAcadExt.enMapTheme = 0, Optional oMarkBlock As DMAcadExt.MarkBlock = Nothing) As Integer
			DMAcadExt.AcadDocument.WriteDebugMessage("miRingsUB=" & miRingsUB.ToString() & "; " & (mdicIsthmusChains IsNot Nothing))
			If miRingsUB > 0 Then
				Dim oInnerPgon As tsPolygon
				Dim baCheck(miRingsUB) As Boolean
				Dim iIslandsCount As Integer = 0
				Dim hsExteriorNodeSet As HashSet(Of Integer) = moaRings(0).NodeSet
				Dim hsInnerNodeSet As HashSet(Of Integer)




				If mdicIsthmusChains IsNot Nothing Then
					For Each oChain As tsBranchChain In mdicIsthmusChains.Values
						If oChain.InnerRingIndex > 0 AndAlso oChain.InnerRingIndex <= miRingsUB Then
							baCheck(oChain.InnerRingIndex) = True
						End If
					Next
				End If
				For iRingIndex As Integer = 1 To miRingsUB
					If Not baCheck(iRingIndex) Then
						hsInnerNodeSet = moaRings(iRingIndex).NodeSet
						DMAcadExt.AcadDocument.WriteDebugMessage("NodeSets " & "; " & iRingIndex & "; " & hsExteriorNodeSet.Count.ToString() & "; " & hsInnerNodeSet.Count.ToString() & "; " & moaRings(iRingIndex).InnerPolygons.Count)
						If Not hsExteriorNodeSet.Overlaps(hsInnerNodeSet) Then
							iIslandsCount += 1
							If oMarkBlock IsNot Nothing Then
								moaRings(iRingIndex).MarkNodes(oMarkBlock)
							End If

							For Each iPgonID As Integer In moaRings(iRingIndex).InnerPolygons
								'DMCommon.Debug.MsgBox("In InnerPolygons", iPgonID, mdicElements.Count)
								oInnerPgon = mdicElements.GetPolygon(iPgonID)
								oInnerPgon.IsInner = True
								DMAcadExt.AcadDocument.WriteDebugMessage("!!mbIsInner " & "; " & oInnerPgon.IsInner.ToString() & "; " & oInnerPgon.IsInner.ToString())
								If oMarkBlock IsNot Nothing Then
									DMAcadExt.AppMessages.AddMessage(True, oInnerPgon.Centroid.X, oInnerPgon.Centroid.Y, "", DMCommon.dmMessages.Message(318), False, iMapThemeID, 34)
								End If
							Next

						End If
					End If
				Next
				Return iIslandsCount
			Else
				Return 0
			End If




		End Function
		Public ReadOnly Property Area As Double
         Get
            Return mdArea
         End Get
      End Property

      Public ReadOnly Property Perimeter As Double
         Get
            Return mdPerimeter
         End Get
      End Property
      Public ReadOnly Property Centroid As Autodesk.AutoCAD.Geometry.Point3d
         Get
            Return mtCentroid
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
      Public ReadOnly Property CentroidLayer As String
         Get
            Return msCentroidLayer
         End Get
      End Property


      Public Sub PrintInfo()

      End Sub
   
      'Act
      Public Sub CreateComplexNodeLinks()
         DMCommon.Debug.MsgBox("2:CreateComplexNodeLinks")
         Dim oBranch As tsBranch
         '  Dim enumerator3 As IEnumerator(Of NodeLink)
         Dim oBranchSet As HashSet(Of Integer) = Me.moaRings(0).BranchSet
			Dim hsExteriorNodeSet As HashSet(Of Integer) = Me.moaRings(0).NodeSet
			Dim iRingDirection As enRingDirection = Me.moaRings(0).Direction
         Dim colExteriorNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(0).NodeLinksCorrected
         Dim hsIntegers As New HashSet(Of Integer)
         Dim dicInnerNodes As New Dictionary(Of Integer, Integer)
         '     Dim set5 As New HashSet(Of Integer)
         '      Dim set3 As New HashSet(Of Integer)
         Me.mhsConnectionPoints = New HashSet(Of Integer)

         Dim miRingsUB As Integer = Me.miRingsUB

         '!! dicInnerNodes
         For iRingIndex As Integer = 1 To miRingsUB
            hsIntegers = Me.moaRings(iRingIndex).NodeSet
            'DMAcadExt.AcadDocument.WriteMessage("hsInteriorN=" & tsTopology.HashSetItems(hsIntegers, False))
            For Each iNode As Integer In hsIntegers
               dicInnerNodes.Add(iNode, iRingIndex)
               '  DMAcadExt.AcadDocument.WriteMessage("+!!=" & iNode.ToString())
            Next
         Next

         ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''End og Part I



         'Dim numArray As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}
         '   Dim a(Me.mhsIsthmuses.Count - 1) As Integer

         '   Dim numArray3 As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}
         '   Dim numArray2 As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}

         Dim dicIsthmuses As New Dictionary(Of Integer, Isthmus)

         For Each iIsthmusID As Integer In Me.mhsIsthmuses
            Dim iInnerRingIndex As Integer
            Dim tIsthmus As Isthmus
            oBranch = Me.mdicElements.GetBranch(iIsthmusID)
				If hsExteriorNodeSet.Contains(oBranch.PreviousNodeID) Then
					If dicInnerNodes.TryGetValue(oBranch.NextNodeID, iInnerRingIndex) Then
						tIsthmus = New Isthmus(oBranch.PreviousNodeID, iInnerRingIndex, oBranch.NextNodeID, oBranch.ID, oBranch.AcObjID, True)
						dicIsthmuses.Add(oBranch.PreviousNodeID, tIsthmus)
					Else
						MessageBox.Show("DesignErr", "04_660")
					End If
				ElseIf hsExteriorNodeSet.Contains(oBranch.NextNodeID) Then
					Me.mhsConnectionPoints.Add(oBranch.NextNodeID)
					If dicInnerNodes.TryGetValue(oBranch.PreviousNodeID, iInnerRingIndex) Then
						tIsthmus = New Isthmus(oBranch.NextNodeID, iInnerRingIndex, oBranch.PreviousNodeID, oBranch.ID, oBranch.AcObjID, False)
						dicIsthmuses.Add(oBranch.NextNodeID, tIsthmus)
					Else
						MessageBox.Show("DesignErr", "04_662a")
					End If
				Else
					MessageBox.Show("DesignErr" & vbCrLf & Me.mhsIsthmuses.Count.ToString() & vbCrLf & oBranch.PreviousNodeID.ToString() & "<==>" & oBranch.NextNodeID.ToString() & vbCrLf & oBranch.EntityHandle.ToString(), "04_670")
            End If
         Next


         ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''End og Part II

         Dim iExteriorDirection As enRingDirection = Me.moaRings(0).Direction

         Dim oLink As DMAcadExt.IUD_Link = Nothing

         '  Dim tNodeLink As New NodeLink
         Me.mcolComplexNodeLinks = New System.Collections.ObjectModel.Collection(Of NodeLink)

         Try
            For Each tNodeLink As NodeLink In colExteriorNodeLinks
               Dim tIsthmus As Isthmus = New Isthmus()

               oBranch = Me.mdicElements.GetBranch(tNodeLink.BranchID)
               Dim curveDirection As Boolean = Me.moaRings(0).GetCurveDirection(oBranch)
               Dim iNodeID As Integer = tNodeLink.NodeID
               Dim oNode As tsNode = Me.mdicElements.GetNode(iNodeID)

               If dicIsthmuses.TryGetValue(iNodeID, tIsthmus) Then
                  oLink = tsBranch.GetLink(tIsthmus.PolylineAcObjID, tIsthmus.Direction)
                  tNodeLink = New NodeLink(tIsthmus.ExteriorNodeID, tIsthmus.BranchID, tIsthmus.Direction, oLink)
                  Me.mcolComplexNodeLinks.Add(tNodeLink)
                  Dim colInnerNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(tIsthmus.InteriorRingIndex).GetNodeLinks(tIsthmus.InteriorNodeID, iExteriorDirection)
                  Enumerable.Union(Of NodeLink)(Me.mcolComplexNodeLinks, colInnerNodeLinks)

                  For Each tInnerNodeLink As NodeLink In colInnerNodeLinks
                     Me.mcolComplexNodeLinks.Add(tInnerNodeLink)
                     DMAcadExt.AcadDocument.WriteMessage("+%!=" & tInnerNodeLink.NodeID & "; " & tInnerNodeLink.BranchID)
                  Next

                  oLink = tsBranch.GetLink(tIsthmus.PolylineAcObjID, Not tIsthmus.Direction)
                  tNodeLink = New NodeLink(tIsthmus.InteriorNodeID, tIsthmus.BranchID, Not tIsthmus.Direction, oLink)
                  Me.mcolComplexNodeLinks.Add(tNodeLink)

               End If
               oNode = Me.mdicElements.GetNode(tNodeLink.NodeID)
               Me.mcolComplexNodeLinks.Add(tNodeLink)
            Next
         Catch
            MessageBox.Show("DesignErr", "04_680")
         End Try
      End Sub
    
      Public Sub CreateDBPolylineCorrected(ByVal bWithIsthmuses As Boolean, bByCentroidLayer As Boolean)
         If (bWithIsthmuses AndAlso (Me.mhsIsthmuses.Count > 0)) Then
				'17/09/2017  Me.zzCreateComplexPolyline()
				' ''''''''''''''''''''''  zzCreateComplexPolyline()
				'DMCommon.ExcelLogAW5.SetNextValue(0, "CreateDBPolyline", ID, Me.mhsIsthmuses.Count)

				zzCreateComplexPolyline(True, bByCentroidLayer)


				' zzCreateComplexPolylineShort()
			Else
            Dim miRingsUB As Integer = Me.miRingsUB
            Dim iRingIndex As Integer = 0
				Dim sLayer As String = Nothing
				Dim iRingMax As Integer
				If bWithIsthmuses Then
					iRingMax = 0
				Else
					iRingMax = miRingsUB
				End If
				If bByCentroidLayer Then
               sLayer = msCentroidLayer
            End If
				Do While (iRingIndex <= iRingMax)

					' Me.moaRings(iRingIndex).CreateDBPolylineNewLast()
					'  DMAcadExt.AcadDocument.WriteMessage("OK Ring: Poligon #" & CStr(ID))

					Me.moaRings(iRingIndex).CreateDBPolylineCorrected(sLayer)

					If iRingIndex = 0 Then
						mtBorderObjID = Me.moaRings(iRingIndex).BorderObjID
						mtBorderHandle = Me.moaRings(iRingIndex).BorderHandle
					End If
					mdAreaCorrected += Me.moaRings(iRingIndex).AreaCorrected

					iRingIndex += 1
				Loop
			End If
      End Sub
      Public Function GetNodeLinksCorrected() As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim colNodeLinksCorrected As System.Collections.ObjectModel.Collection(Of NodeLink) = New System.Collections.ObjectModel.Collection(Of NodeLink)()
         Dim miRingsUB As Integer = Me.miRingsUB
         Dim iRingIndex As Integer = 0

         Do While (iRingIndex <= miRingsUB)
            For Each oNodeLink As NodeLink In Me.moaRings(iRingIndex).GetNodeLinksCorrected()
               colNodeLinksCorrected.Add(oNodeLink)
            Next
            iRingIndex += 1
         Loop
         Return colNodeLinksCorrected
      End Function
      Public Sub CreateDBPolyline(Optional oPolyline As Polyline = Nothing)
         Dim miRingsUB As Integer = Me.miRingsUB
         Dim iRingIndex As Integer = 0
         Do While (iRingIndex <= miRingsUB)
            ' Me.moaRings(iRingIndex).CreateDBPolylineNewLast()
            Me.moaRings(iRingIndex).CreateDBPolyline(oPolyline)
            iRingIndex += 1
         Loop

      End Sub
      Public Function CreateDBPolylineWOInners() As ObjectId
         Dim iRingIndex As Integer = 0

         ' Me.moaRings(iRingIndex).CreateDBPolylineNewLast()
         Return Me.moaRings(iRingIndex).CreateDBPolyline()
      End Function
      Public Function GetMarkPoints(dDistParam As Double) As List(Of InitInsertData) ' System.Collections.ObjectModel.Collection(Of InitInsertData)
         Dim miRingsUB As Integer = Me.miRingsUB
         Dim iRingIndex As Integer = 0
         Dim oResList As List(Of InitInsertData) = New List(Of InitInsertData)()
         '  DMAcadExt.AcadDocument.WriteDebugMessage("miRingsUB= " & CStr(miRingsUB))
         Do While (iRingIndex <= miRingsUB)

            oResList.AddRange(Me.moaRings(iRingIndex).GetMarkPoints(dDistParam))
            '  MessageBox.Show(CStr(oResList.Count), "08_900")
            ' DMAcadExt.AcadDocument.WriteMessage("PGon! " & CStr(Me.ID) & " PCnt=" & oResList.Count.ToString())
            iRingIndex += 1
         Loop
         Return oResList
      End Function

      Public Property AreaCorrected As Double
         Get
            Return mdAreaCorrected
         End Get
         Set(dValue As Double)
            mdAreaCorrected = dValue
         End Set
      End Property
      Public Function GetPointLinks() As System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink)
         Dim mcolComplexNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
         Dim collection2 As New System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink)
         If (Not Me.mcolComplexNodeLinks Is Nothing) Then
            mcolComplexNodeLinks = Me.mcolComplexNodeLinks
         Else
            mcolComplexNodeLinks = Me.moaRings(0).NodeLinksCorrected
         End If
         If (Not mcolComplexNodeLinks Is Nothing) Then
            Dim link As NodeLink
            For Each link In mcolComplexNodeLinks
               Dim name As String
               Dim node As tsNode = Me.mdicElements.GetNode(link.NodeID)
               If (Not node Is Nothing) Then
                  name = node.Name
               Else
                  name = String.Empty
               End If
               If (Not link.Link Is Nothing) Then
                  Try
                     Dim item As New DMAcadExt.PointLink(name, link.Link)
                     collection2.Add(item)
                  Catch exception1 As Exception

                  End Try
               End If
            Next
         End If
         Return collection2
      End Function
		Private Sub zzCreateComplexPolyline(bOneLine As Boolean, bByCentroidLayer As Boolean)
			' qqq12() 02/01/19
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "PgonID_BranchSet", diID, mtCentroid.ToString())
			'''''''''''''''''''''''	DMCommon.Debug.MsgBox("11:zzCreateComplexPolylinePlus")
			'DMCommon.ExcelLogAW5.SetNextValue(0, "ComplexPolyline", ID, moaRings.GetUpperBound(0))
			Dim oBranch As tsBranch
			Dim branchSet As HashSet(Of Integer) = Me.moaRings(0).BranchSet
			DMCommon.Debug.ExcelLog.SetNextValue(0, "NodeSet", diID, mtCentroid.ToString())
			Dim hsExteriorNodes As HashSet(Of Integer) = Me.moaRings(0).NodeSet
			DMCommon.Debug.ExcelLog.SetNextValue(0, "Direction", diID, mtCentroid.ToString())
			Dim direction2 As enRingDirection = Me.moaRings(0).Direction
			DMCommon.Debug.ExcelLog.SetNextValue(0, "CurrentNodeLinks", diID, mtCentroid.ToString())
			' Dim nodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(0).NodeLinks
			Dim colExteriorNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(0).CurrentNodeLinks
			Dim hsIntegers As New HashSet(Of Integer)
			Dim dicInnerNodes As New Dictionary(Of Integer, Integer)
			'   Dim set5 As New HashSet(Of Integer)
			'    Dim set3 As New HashSet(Of Integer)
			Me.mhsConnectionPoints = New HashSet(Of Integer)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End", diID)
			Dim miRingsUB As Integer = Me.miRingsUB
			Dim oIsthmus As Isthmus

			'    DMAcadExt.AcadDocument.WriteMessage(mdicElements.TopoName & "; " & CStr(Me.ID) & "; " & CStr(miRingsUB))
			'	DMCommon.Debug.MsgBox("08_131")
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End", diID, miRingsUB)
			For iRingIndex As Integer = 1 To miRingsUB
				' Dim oHashsetEnum As System.Collections.Generic.HashSet(Of Integer).Enumerator = Nothing
				hsIntegers = Me.moaRings(iRingIndex).NodeSet
				DMCommon.Debug.ExcelLog.SetNextValue(0, "hsIntegers", diID, hsIntegers Is Nothing)
				'  DMAcadExt.AcadDocument.WriteMessage(("hsInteriorN=" & tsTopology.HashSetItems(hsIntegers, False)), New Object(0 - 1) {})
				Try
					'   oHashsetEnum = hsIntegers.GetEnumerator()
					' Do While oHashsetEnum.MoveNext
					'Dim current As Integer = oHashsetEnum.Current
					'dicInnerNodes.Add(current, iRingIndex)
					' Loop
					If hsIntegers IsNot Nothing Then
						For Each iNode As Integer In hsIntegers
							dicInnerNodes.Add(iNode, iRingIndex)
							'  DMAcadExt.AcadDocument.WriteMessage("+!!=" & iNode.ToString())
						Next
					End If


				Catch
					'
				End Try
			Next


			'  DMCommon.Debug.MsgBox("13_016a", DMCommon.Debug.ColCount(Me.mhsIsthmuses))
			If False Then
				Dim dicIsthmuses As New Dictionary(Of Integer, Isthmus)
				For Each iBranchID As Integer In Me.mhsIsthmuses
					Dim iRingID As Integer

					oBranch = Me.mdicElements.GetBranch(iBranchID)

					If hsExteriorNodes.Contains(oBranch.PreviousNodeID) Then
						If dicInnerNodes.TryGetValue(oBranch.NextNodeID, iRingID) Then
							oIsthmus = New Isthmus(oBranch.PreviousNodeID, iRingID, oBranch.NextNodeID, oBranch.ID, oBranch.AcObjID, True)
							dicIsthmuses.Add(oBranch.PreviousNodeID, oIsthmus)
						Else
							MessageBox.Show("DesignErr", "04_660")
						End If
					ElseIf hsExteriorNodes.Contains(oBranch.NextNodeID) Then
						Me.mhsConnectionPoints.Add(oBranch.NextNodeID)
						If dicInnerNodes.TryGetValue(oBranch.PreviousNodeID, iRingID) Then
							oIsthmus = New Isthmus(oBranch.NextNodeID, iRingID, oBranch.PreviousNodeID, oBranch.ID, oBranch.AcObjID, False)
							dicIsthmuses.Add(oBranch.NextNodeID, oIsthmus)
						Else
							MessageBox.Show("DesignErr", "04_662b")
						End If
					Else
						MessageBox.Show("DesignErr", "04_668")
					End If

				Next
			End If


			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''End og Part II
			Dim iExteriorDirection As enRingDirection = Me.moaRings(0).Direction
			Dim oBulgeVertexArray As New GeoUtilites.BulgeVertexArray
			Dim collection2 As New System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
			Dim hsResLinks As New HashSet(Of DMAcadExt.IUD_Link)
			Dim item As DMAcadExt.IUD_Link = Nothing
			Dim oNode As tsNode = Nothing
			Dim oLink As DMAcadExt.IUD_Link = Nothing
			Dim oDebugPrevLink As DMAcadExt.IUD_Link = Nothing
			Dim tDebugPrevLinkStartPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim tDebugPrevLinkEndPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim dDebugPrevLinkBulge As Double
			Dim oChain As tsBranchChain = Nothing
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End2", diID, colExteriorNodeLinks Is Nothing)
			For Each tNodeLink As NodeLink In colExteriorNodeLinks

				oBranch = Me.mdicElements.GetBranch(tNodeLink.BranchID)

				Dim curveDirection As Boolean = Me.moaRings(0).GetCurveDirection(oBranch)
				'   DMAcadExt.AcadDocument.WriteDebugMessage("xx!1 " & oBranch.ToString() & "; " & tNodeLink.BranchID.ToString() & "; Dir=" & curveDirection)
				Dim iNodeID As Integer = tNodeLink.NodeID
				Dim node As tsNode = Me.mdicElements.GetNode(iNodeID)
				Dim flag As Boolean = False
				If mdicIsthmusChains IsNot Nothing AndAlso mdicIsthmusChains.TryGetValue(iNodeID, oChain) Then
					'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!2 Chain= " & oChain.Info)
					'   DMAcadExt.AcadDocument.WriteDebugMessage("xx!3 " & nodeID.ToString() & "; Ex=" & oChain.ExteriorNodeID.ToString() & "; In=" & oChain.InnerNodeID.ToString())
					If (item IsNot Nothing) Then
						hsResLinks.Add(item)
						''''''''''''''''''
						If oDebugPrevLink Is Nothing Then
							tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							dDebugPrevLinkBulge = -999
						Else
							tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
							tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
							dDebugPrevLinkBulge = oDebugPrevLink.Bulge
						End If
						'DMCommon.ExcelLogAW5.SetNextValue(0, "AddItemStart", ID, hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, item.StartPoint, item.EndPoint)
						oDebugPrevLink = item
						' DMAcadExt.AcadDocument.WriteDebugMessage("+**= " & item.StartPoint.ToString() & "; " & item.EndPoint.ToString())
					End If
					'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!4' " & hsResLinks.Count.ToString())

					'  link = tsRing.GetLink(oIsthmus.PolylineAcObjID, oIsthmus.Direction)
					'   Dim colInnerNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(tIsthmus.InteriorRingIndex).GetNodeLinks(tIsthmus.InteriorNodeID, iExteriorDirection)
					If bOneLine Then
						oLink = oChain.GetSegment()
						hsResLinks.Add(oLink)
						'zzLinkToExcel(oLink, "BridgeOne")
						''''''''''''''''''''''''''''''''''''''
						If oDebugPrevLink Is Nothing Then
							tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							dDebugPrevLinkBulge = -999
						Else
							tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
							tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
							dDebugPrevLinkBulge = oDebugPrevLink.Bulge
						End If
						'DMCommon.ExcelLogAW5.SetNextValue(0, "AddBridge1", ID, hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, oLink.StartPoint, oLink.EndPoint, oLink.Bulge, mtCentroid)
						oDebugPrevLink = oLink
						''''''''''''''''''''''''''''''''''''''

					Else
						For iIndex As Integer = 0 To oChain.Count - 1
							oLink = oChain.GetLink(iIndex)
							hsResLinks.Add(oLink)

							''''''''''''''''''''''
							If oDebugPrevLink Is Nothing Then
								tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								dDebugPrevLinkBulge = -999
							Else
								tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
								tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
								dDebugPrevLinkBulge = oDebugPrevLink.Bulge
							End If
							'DMCommon.ExcelLogAW5.SetNextValue(0, "AddBridge2", ID, hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, oLink.StartPoint, oLink.EndPoint, ID, mtCentroid)
							oDebugPrevLink = oLink
							'''''''''''''''''''''''''''''''''''''''''
							'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!4 " & hsResLinks.Count.ToString())
						Next
					End If

					'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!5 " & oChain.InnerRingIndex & "; Ex=" & hsResLinks.Count.ToString() & "; Ex=" & oChain.InnerNodeID)

					hsResLinks.UnionWith(Me.moaRings(oChain.InnerRingIndex).GetLinks(oChain.InnerNodeID, iExteriorDirection))
					'	DMCommon.ExcelLogAW5.SetNextValue(0, "AddRing", ID, hsResLinks.Count)
					'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!5a " & hsResLinks.Count.ToString())
					'Dim oTest As HashSet(Of DMAcadExt.IUD_Link) = Me.moaRings(oChain.InnerRingIndex).GetLinksNewLast(oChain.InnerNodeID, iExteriorDirection)
					If bOneLine Then

						oLink = oChain.GetReverseSegment()
						hsResLinks.Add(oLink)


						''''''''''''''''''''''
						If oDebugPrevLink Is Nothing Then
							tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
							dDebugPrevLinkBulge = -999
						Else
							tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
							tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
							dDebugPrevLinkBulge = oDebugPrevLink.Bulge
						End If
						'DMCommon.ExcelLogAW5.SetNextValue(0, "Oneline", ID, hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, oLink.StartPoint, oLink.EndPoint, ID, mtCentroid)
						oDebugPrevLink = oLink
						'''''''''''''''''''''''''''''''''''''''''''''

					Else

						For iIndex As Integer = oChain.Count - 1 To 0 Step -1
							oLink = oChain.GetReverseLink(iIndex)
							hsResLinks.Add(oLink)

							''''''''''''''''''''''
							If oDebugPrevLink Is Nothing Then
								tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								dDebugPrevLinkBulge = -999
							Else
								tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
								tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
								dDebugPrevLinkBulge = oDebugPrevLink.Bulge
							End If
							'DMCommon.ExcelLogAW5.SetNextValue(0, "manyline", hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, oLink.StartPoint, oLink.EndPoint, ID, mtCentroid)
							oDebugPrevLink = oLink
							'''''''''''''''''''''''''''''''''''''''''''''
							'  DMAcadExt.AcadDocument.WriteDebugMessage("xx!6 " & hsResLinks.Count.ToString())
						Next

					End If




					' link = tsRing.GetLink(oIsthmus.PolylineAcObjID, Not oIsthmus.Direction)

					flag = True
				End If
				If (item Is Nothing) Then
					item = tNodeLink.Link
					oNode = Me.mdicElements.GetNode(tNodeLink.NodeID)
				Else
					node = Me.mdicElements.GetNode(tNodeLink.NodeID)
					'If   oNodeScheme.IsPseudo And Not oNodeScheme.HasUsedPoint And Not oNodeScheme.HasActiveSurveyPoint And Not oNodeScheme.HasMustPoint And Not oNodeScheme.HasMustVertex And Not bIsMust Then
					If (node.IsSkipVertex And Not flag) Then

						item.Extend(tNodeLink.Link)
					Else
						If Not flag Then
							hsResLinks.Add(item)

							''''''''''''''''''''''
							If oDebugPrevLink Is Nothing Then
								tDebugPrevLinkStartPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								tDebugPrevLinkEndPoint = New Autodesk.AutoCAD.Geometry.Point2d()
								dDebugPrevLinkBulge = -999
							Else
								tDebugPrevLinkStartPoint = oDebugPrevLink.StartPoint
								tDebugPrevLinkEndPoint = oDebugPrevLink.EndPoint
								dDebugPrevLinkBulge = oDebugPrevLink.Bulge
							End If
							'	DMCommon.ExcelLogAW5.SetNextValue(0, "After2", ID, hsResLinks.Count, tDebugPrevLinkStartPoint, tDebugPrevLinkEndPoint, dDebugPrevLinkBulge, item.StartPoint, item.EndPoint)
							oDebugPrevLink = item
							'''''''''''''''''''''''''''''''''''''''''''''





						End If
						item = tNodeLink.Link
					End If
				End If
			Next
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End3", diID)
			If item IsNot Nothing Then


				If oNode IsNot Nothing AndAlso oNode.IsSkipVertex Then
					item.Extend(Enumerable.First(Of DMAcadExt.IUD_Link)(hsResLinks))
					hsResLinks.Remove(Enumerable.First(Of DMAcadExt.IUD_Link)(hsResLinks))
				End If
				hsResLinks.Add(item)
			End If

			DMCommon.Debug.ExcelLog.SetNextValue(0, "End4", diID)
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			oBulgeVertexArray = New GeoUtilites.BulgeVertexArray
			oBulgeVertexArray.AddDim(hsResLinks.Count)
			Dim num As Integer = 0
			'DMCommon.Debug.MsgBox("3:zzCreateComplexPolyline", hsResLinks.Count)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End5", diID, hsResLinks Is Nothing)
			If hsResLinks IsNot Nothing Then

				For Each oLink In hsResLinks


					If oLink IsNot Nothing Then
						oBulgeVertexArray.AddBulgeVertex(oLink.StartPoint, oLink.Bulge)
					End If
					'DMAcadExt.AcadDocument.WriteDebugMessage("+%!=" & link4.StartPoint.ToString() & "; " & num.ToString())
					num += 1
				Next
			End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End6", diID, hsResLinks Is Nothing)
			mtBorderObjID = oBulgeVertexArray.CreateDBPolyline(True)
			Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(mtBorderObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "End7", diID, oCurve Is Nothing)

			If bByCentroidLayer AndAlso oCurve Is Nothing Then
				oCurve.Layer = msCentroidLayer
			End If


			mtBorderHandle = oCurve.Handle
			mdAreaCorrected = oCurve.Area

		End Sub
		Private Sub zzLinkToExcel(oLink As DMAcadExt.IUD_Link, sCaption As String)
			'DMCommon.ExcelLogAW5.SetNextValue(0, sCaption, oLink.StartPoint, oLink.EndPoint, ID, mtCentroid)
		End Sub
		'Act

		Private Sub zzCreateComplexPolylineShort()
         DMCommon.Debug.MsgBox("41:zzCreateComplexPolylineShort")
         'mcolComplexNodeLinks
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray
         oBulgeVertexArray.AddDim(mcolComplexNodeLinks.Count)
         For Each tNodeLink As NodeLink In mcolComplexNodeLinks

            If tNodeLink.Link IsNot Nothing Then
               oBulgeVertexArray.AddBulgeVertex(tNodeLink.Link.StartPoint, tNodeLink.Link.Bulge)
            End If


         Next

         Dim tPlineObjID As ObjectId = oBulgeVertexArray.CreateDBPolyline(True)
         Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tPlineObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         mdAreaCorrected = oCurve.Area
      End Sub
      Private Function zzBranchesToChains(hsBranches As HashSet(Of Integer)) As System.Collections.ObjectModel.Collection(Of tsBranchChain)
         Dim oBranch As tsBranch
         Dim oChain As tsBranchChain = Nothing
         Dim oNewChain As tsBranchChain
         Dim dicChains As Dictionary(Of Integer, tsBranchChain) = New Dictionary(Of Integer, tsBranchChain)()
         For Each iBranchID As Integer In hsBranches
            oBranch = mdicElements.GetBranch(iBranchID)
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



      'Private Sub zzCreateComplexPolylineNewLast_DOUBLE()
      '   Dim branch As tsBranch
      '   Dim branchSet As HashSet(Of Integer) = Me.moaRings(0).BranchSet
      '   Dim nodeSet As HashSet(Of Integer) = Me.moaRings(0).NodeSet
      '   Dim direction2 As enRingDirection = Me.moaRings(0).Direction
      '   Dim nodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink) = Me.moaRings(0).NodeLinks
      '   Dim hsIntegers As New HashSet(Of Integer)
      '   Dim dictionary As New Dictionary(Of Integer, Integer)
      '   Dim set5 As New HashSet(Of Integer)
      '   Dim set3 As New HashSet(Of Integer)
      '   Me.mhsConnectionPoints = New HashSet(Of Integer)
      '   Dim miRingsUB As Integer = Me.miRingsUB
      '   Dim i As Integer = 1
      '   Do While (i <= miRingsUB)
      '      Dim enumerator As System.Collections.Generic.HashSet(Of Integer).Enumerator
      '      hsIntegers = Me.moaRings(i).NodeSet
      '      DMAcadExt.AcadDocument.WriteMessage(("hsInteriorN=" & tsTopology.HashSetItems(hsIntegers, False)), New Object(0 - 1) {})
      '      Try
      '         enumerator = hsIntegers.GetEnumerator
      '         Do While enumerator.MoveNext
      '            Dim current As Integer = enumerator.Current
      '            dictionary.Add(current, i)
      '         Loop
      '      Finally
      '         enumerator.Dispose()
      '      End Try
      '      i += 1
      '   Loop
      '   Dim numArray As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}
      '   Dim numArray3 As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}
      '   Dim numArray2 As Integer() = New Integer(((Me.mhsIsthmuses.Count - 1) + 1) - 1) {}
      '   Dim num4 As Integer = 0
      '   Dim dictionary2 As New Dictionary(Of Integer, Isthmus)
      '   Dim num7 As Integer
      '   For Each num7 In Me.mhsIsthmuses
      '      Dim num2 As Integer
      '      Dim isthmus2 As Isthmus
      '      branch = Me.mdicElements.GetBranch(num7)
      '      If nodeSet.Contains(branch.PreviousNodeID) Then
      '         If dictionary.TryGetValue(branch.NextNodeID, num2) Then
      '            isthmus2 = New Isthmus(branch.PreviousNodeID, num2, branch.NextNodeID, branch.ID, branch.AcObjID, True)
      '            dictionary2.Add(branch.PreviousNodeID, isthmus2)
      '         Else
      '            MessageBox.Show("DesignErr", "04_660")
      '         End If
      '      ElseIf nodeSet.Contains(branch.NextNodeID) Then
      '         Me.mhsConnectionPoints.Add(branch.NextNodeID)
      '         If dictionary.TryGetValue(branch.PreviousNodeID, num2) Then
      '            isthmus2 = New Isthmus(branch.PreviousNodeID, num2, branch.PreviousNodeID, branch.ID, branch.AcObjID, False)
      '            dictionary2.Add(branch.NextNodeID, isthmus2)
      '         Else
      '            MessageBox.Show("DesignErr", "04_662")
      '         End If
      '      End If
      '      num4 += 1
      '   Next
      '   Dim iExteriorDirection As enRingDirection = Me.moaRings(0).Direction
      '   Dim array As New GeoUtilites.BulgeVertexArray
      '   Dim collection2 As New System.Collections.ObjectModel.Collection(Of DMAcadExt.UD_Link)
      '   Dim source As New HashSet(Of DMAcadExt.UD_Link)
      '   Dim item As DMAcadExt.UD_Link = Nothing
      '   Dim node2 As tsNode = Nothing
      '   Dim link As DMAcadExt.UD_Link = Nothing
      '   Dim link3 As NodeLink
      '   For Each link3 In nodeLinks
      '      Dim isthmus As Isthmus
      '      branch = Me.mdicElements.GetBranch(link3.BranchID)
      '      Dim curveDirection As Boolean = Me.moaRings(0).GetCurveDirection(branch)
      '      Dim nodeID As Integer = link3.NodeID
      '      Dim node As tsNode = Me.mdicElements.GetNode(nodeID)
      '      Dim flag As Boolean = False
      '      If dictionary2.TryGetValue(nodeID, isthmus) Then
      '         If (Not item Is Nothing) Then
      '            source.Add(item)
      '         End If
      '         link = tsRing.GetLink(isthmus.PolylineAcObjID, isthmus.Direction)
      '         source.Add(link)
      '         source.UnionWith(Me.moaRings(isthmus.InteriorRingIndex).GetLinksNewLast(isthmus.InteriorNodeID, iExteriorDirection))
      '         link = tsRing.GetLink(isthmus.PolylineAcObjID, Not isthmus.Direction)
      '         source.Add(link)
      '         flag = True
      '      End If
      '      If (item Is Nothing) Then
      '         item = link3.Link
      '         node2 = Me.mdicElements.GetNode(link3.NodeID)
      '      Else
      '         node = Me.mdicElements.GetNode(link3.NodeID)
      '         If (((node.IsPseudoTopo AndAlso node.IsPseudoGeo) And Not node.HasActiveSurveyPoint) And Not flag) Then
      '            item.Extend(link3.Link)
      '         Else
      '            If Not flag Then
      '               source.Add(item)
      '            End If
      '            item = link3.Link
      '         End If
      '      End If
      '   Next
      '   If ((node2.IsPseudoTopo AndAlso node2.IsPseudoGeo) And Not node2.HasActiveSurveyPoint) Then
      '      item.Extend(Enumerable.First(Of DMAcadExt.UD_Link)(source))
      '      source.Remove(Enumerable.First(Of DMAcadExt.UD_Link)(source))
      '   End If
      '   source.Add(item)
      '   array = New GeoUtilites.BulgeVertexArray
      '   array.AddDim(source.Count)
      '   Dim num As Integer = 0
      '   Dim link4 As DMAcadExt.UD_Link
      '   For Each link4 In source
      '      If (MyBase.diID = &H9858) Then
      '         DMAcadExt.AcadDocument.WriteMessage(String.Concat(New String() {"!!Handle= ", link4.EntityHandle.ToString, " Pt=", link4.StartPoint.ToString, " B=", link4.Bulge.ToString}), New Object(0 - 1) {})
      '      End If
      '      array.AddBulgeVertex(link4.StartPoint, link4.Bulge)
      '      num += 1
      '   Next
      '   array.CreateDBPolyline(True)
      'End Sub











      Private Structure Isthmus
         Public ExteriorNodeID As Integer
         Public InteriorRingIndex As Integer
         Public InteriorNodeID As Integer
         Public BranchID As Integer
         Public PolylineAcObjID As ObjectId
         Public Direction As Boolean
         '  Public Branches As tsBranch()
         Public Sub New(ByVal iExteriorNodeID As Integer, ByVal iInteriorRingIndex As Integer, ByVal iInteriorNodeID As Integer, ByVal iBranchID As Integer, ByVal tPolylineAcObjID As ObjectId, ByVal bDirection As Boolean)

            Me.ExteriorNodeID = iExteriorNodeID
            Me.InteriorRingIndex = iInteriorRingIndex
            Me.InteriorNodeID = iInteriorNodeID
            Me.BranchID = iBranchID
            Me.PolylineAcObjID = tPolylineAcObjID
            Me.PolylineAcObjID = tPolylineAcObjID
            Me.Direction = bDirection
         End Sub
      End Structure


   End Class
End Namespace