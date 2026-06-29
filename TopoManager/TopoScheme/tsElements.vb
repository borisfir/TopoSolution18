Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme

   Public Class tsElements
      Inherits Dictionary(Of Integer, tsElement)
      Private mhsPolygons As HashSet(Of Integer)
      Private mhsBranches As HashSet(Of Integer)
      Private mhsNodes As HashSet(Of Integer)
      Private mdicEntities As Dictionary(Of ObjectId, tsElement)
      Private msTopoName As String
      Private moCentroidAcadBlock As DMAcadExt.AcadBlock
      Private mbHasCentroidData As Boolean
      Private miMaxID As Integer
      Private moLayerFilter As DMCommon.dmList
      Private mbSimplexLink As Boolean
		Private mdTotalArea As Double = 0.0
		Private mbMapObjectsDispose As Boolean = True
      Public Sub New(sTopoName As String, Optional bHasCentroidData As Boolean = False)
         msTopoName = sTopoName

         mhsPolygons = New HashSet(Of Integer)
         mhsBranches = New HashSet(Of Integer)
         mhsNodes = New HashSet(Of Integer)
         mbHasCentroidData = bHasCentroidData
         mdicEntities = New Dictionary(Of ObjectId, tsElement)()
      End Sub
      Public ReadOnly Property PolygonIDs As HashSet(Of Integer)
         Get
            Return mhsPolygons
         End Get
      End Property
      Public ReadOnly Property Polygons As System.Collections.ObjectModel.Collection(Of tsPolygon)
         Get
            Dim colPolygons As System.Collections.ObjectModel.Collection(Of tsPolygon) = New System.Collections.ObjectModel.Collection(Of tsPolygon)()
            Dim oPgon As tsPolygon
            For Each iPgonID As Integer In mhsPolygons
               oPgon = GetPolygon(iPgonID)
               colPolygons.Add(oPgon)
            Next
            Return colPolygons
         End Get
      End Property
      Public ReadOnly Property BrancheIDs As HashSet(Of Integer)
         Get
            Return mhsBranches
         End Get
      End Property
      Public ReadOnly Property NodeIDs As HashSet(Of Integer)
         Get
            Return mhsNodes
         End Get
      End Property
      Public ReadOnly Property Nodes As System.Collections.ObjectModel.Collection(Of tsNode)
         Get
            Dim colNodes As System.Collections.ObjectModel.Collection(Of tsNode) = New System.Collections.ObjectModel.Collection(Of tsNode)()
            Dim oNode As tsNode
            For Each iNodeID As Integer In mhsNodes
               oNode = GetNode(iNodeID)
               colNodes.Add(oNode)
            Next

            Return colNodes
         End Get
      End Property
      Public ReadOnly Property Branches As System.Collections.ObjectModel.Collection(Of tsBranch)
         Get
            Dim colBranches As System.Collections.ObjectModel.Collection(Of tsBranch) = New System.Collections.ObjectModel.Collection(Of tsBranch)()
            Dim oBranch As tsBranch

            For Each iBranchID As Integer In mhsBranches
               oBranch = GetBranch(iBranchID)
               colBranches.Add(oBranch)
            Next

            Return colBranches
         End Get
      End Property
		Public Property TopoName As String
			Get
				Return msTopoName
			End Get
			Set(sValue As String)
				msTopoName = sValue
			End Set
		End Property
		Public ReadOnly Property CentroidAcadBlock As DMAcadExt.AcadBlock
			Get
				Return moCentroidAcadBlock
			End Get
		End Property
		Public ReadOnly Property TotalArea As Double
			Get
				Return mdTotalArea
			End Get
		End Property
		Public Property LayerFilter As DMCommon.dmList
         Get
            Return moLayerFilter
         End Get
         Set(oValue As DMCommon.dmList)
            moLayerFilter = oValue
         End Set
      End Property
      Public Property SimplexLink As Boolean
         Get
            Return mbSimplexLink
         End Get
         Set(bValue As Boolean)
            mbSimplexLink = bValue
         End Set
      End Property
      Public Property MapObjectsDispose As Boolean
         Get
            Return mbMapObjectsDispose
         End Get
         Set(bValue As Boolean)
            mbMapObjectsDispose = bValue
         End Set
      End Property



      Public Sub TryAddBranch(oFullEdge As FullEdge)
         Dim oBranch As tsBranch = Nothing
         Dim oPgonScheme As tsPolygon
			If Not mhsBranches.Contains(oFullEdge.ID) Then
				'	DMCommon.ExcelLogAW5.SetNextValue(0, "NotPgonBranch", oFullEdge.ID, oFullEdge.GetNextNode(False).Location, oFullEdge.GetNextNode(True).Location)
				oBranch = New tsBranch(oFullEdge)
				zzAddID(oBranch.ID)
				If oBranch.IsIsthmus Then
					MyBase.Add(oBranch.ID, oBranch)
					mhsBranches.Add(oBranch.ID)
					oPgonScheme = GetPolygon(oBranch.LeftPolygon)
					' DMCommon.Debug.MsgBox("13_013", oBranch.ID, oBranch.LeftPolygon, oBranch.RightPolygon, oPgonScheme.ID)
					oPgonScheme.AddIsthmus(oBranch)

				Else
					DMAcadExt.AcadDocument.WriteMessage("DesignErr_#080: LeftPgon" & CStr(oBranch.LeftPolygon) & " RightPgon " & CStr(oBranch.RightPolygon) & " Br_Cnt= " & CStr(mhsBranches.Count))
				End If

			End If
		End Sub
      Public Function GetCentroids() As ObjectIdCollection
         Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
         Dim oPgon As tsPolygon
         For Each iPgonID As Integer In mhsPolygons
            oPgon = GetPolygon(iPgonID)
            colCentroids.Add(oPgon.AcObjID)
         Next
         Return colCentroids
      End Function
      Public Function TryGetBranch(oHalfEdge As HalfEdge, iRingNumber As Integer) As tsBranch
         Dim oElement As tsElement = Nothing
         Dim oBranch As tsBranch = Nothing
         Dim oFullEdge As FullEdge = oHalfEdge.FullEdge
         If MyBase.TryGetValue(oFullEdge.ID, oElement) Then


            If oElement.ElemType = TopoElemType.Branch Then
               oBranch = DirectCast(oElement, tsBranch)
               oBranch.SetRingNumber(iRingNumber)
            Else
               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#055: FullEdge.ID=" & CStr(oHalfEdge.FullEdge.ID))
            End If
         Else


            oBranch = New tsBranch(oFullEdge)
            oBranch.SetRingNumber(iRingNumber)
            MyBase.Add(oBranch.ID, oBranch)
            mhsBranches.Add(oBranch.ID)
            mdicEntities.Add(oBranch.AcObjID, oBranch)
         End If
         Return oBranch
      End Function
      Public Function TryAddNodeScheme(oNode As Node, iRing As Integer) As tsNode
         Dim oElement As tsElement = Nothing
         Dim oNodeScheme As tsNode = Nothing

         If MyBase.TryGetValue(oNode.ID, oElement) Then
            If oElement.ElemType = TopoElemType.Node Then
               oNodeScheme = DirectCast(oElement, tsNode)
               oNodeScheme.SetInnerRing(iRing)
               '  DMAcadExt.AcadDocument.WriteDebugMessageN("NodeExists", oNodeScheme.ID, oNodeScheme.InnerRing, iRing)
            Else

               DMAcadExt.AcadDocument.WriteMessage("DesignErr_#064: Node.ID=" & CStr(oNode.ID))
            End If
         Else

				'   DMAcadExt.AcadDocument.WriteMessage("------" & CStr(oNode.ID) & "; " & oNode.Location.ToString())
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oNode", oNode IsNot Nothing)
				oNodeScheme = New tsNode(oNode, iRing)
            ' DMAcadExt.AcadDocument.WriteDebugMessageN("NodeNew", oNodeScheme.ID, oNodeScheme.InnerRing)

            MyBase.Add(oNodeScheme.ID, oNodeScheme)
            zzAddID(oNodeScheme.ID)
            mhsNodes.Add(oNodeScheme.ID)
            If oNodeScheme.HasEntity Then
               mdicEntities.Add(oNodeScheme.AcObjID, oNodeScheme)
            End If
            '   oNode.GetEdges()
         End If
         Return oNodeScheme

      End Function

      Public Function AddPolygon(ByVal oPolygon As Polygon, bCheckExtend As Boolean) As tsPolygon
			Dim oPgonScheme As tsPolygon = New tsPolygon(oPolygon.ID, Me)
			mdTotalArea += oPgonScheme.Area
			Try
            MyBase.Add(oPgonScheme.ID, oPgonScheme)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oPgonScheme.ID.ToString(), "AddPolygon_23")
            Return oPgonScheme
         End Try
         zzAddID(oPgonScheme.ID)

			oPgonScheme.Load(oPolygon, bCheckExtend)


			Try
            mdicEntities.Add(oPgonScheme.AcObjID, oPgonScheme)
         Catch oEx As Exception
            DMAcadExt.DMApp.MsgBox("07_810", oPgonScheme Is Nothing, oEx.Message, mdicEntities.Count, oPgonScheme.ID, oPgonScheme.AcObjID, oPgonScheme.Centroid)

         End Try

         Try
            mhsPolygons.Add(oPgonScheme.ID)
         Catch oEx As Exception
            DMAcadExt.DMApp.MsgBox("07_811", mhsPolygons.Count, oPgonScheme.ID, oPgonScheme.AcObjID, oPgonScheme.Centroid)
         End Try


         Return oPgonScheme
      End Function

      Public Function GetPolygon(ByVal iID As Integer) As tsPolygon
         Dim oElement As tsElement = Nothing
         If MyBase.TryGetValue(iID, oElement) Then
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
         If MyBase.TryGetValue(iID, oElement) Then
            If oElement.ElemType = TopoElemType.Node Then
               Return DirectCast(oElement, tsNode)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function
      Public Function GetBranch(ByVal iID As Integer) As tsBranch
         Dim oElement As tsElement = Nothing
         If MyBase.TryGetValue(iID, oElement) Then
            If oElement.ElemType = TopoElemType.Branch Then
               Return DirectCast(oElement, tsBranch)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function
      Public Function GetBranch(ByVal tAcObjID As ObjectId) As tsBranch
         Dim oElement As tsElement = Nothing
         If mdicEntities.TryGetValue(tAcObjID, oElement) Then
            If oElement.ElemType = TopoElemType.Branch Then
               Return DirectCast(oElement, tsBranch)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function
      Public Function GetPolygon(ByVal tAcObjID As ObjectId) As tsPolygon
         Dim oElement As tsElement = Nothing
         If mdicEntities.TryGetValue(tAcObjID, oElement) Then
            If oElement.ElemType = TopoElemType.Polygon Then
               Return DirectCast(oElement, tsPolygon)
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function

      Public Function LoadCentroidData(ByVal tAcObjID As ObjectId) As DMAcadExt.BlockRefData
         If mbHasCentroidData Then
            If moCentroidAcadBlock Is Nothing Then
               moCentroidAcadBlock = New DMAcadExt.AcadBlock()
               moCentroidAcadBlock.Open(tAcObjID)
            End If
            Return moCentroidAcadBlock.GetBlockRefData(tAcObjID)
         Else
            Return Nothing
         End If

      End Function
      Public Sub MarkIsthmuses()
         Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
         Dim oBranch As tsBranch
         Dim oNode As tsNode

         For Each iBranchID As Integer In mhsBranches

            oBranch = GetBranch(iBranchID)
            If oBranch.IsIsthmus Then
               oNode = GetNode(oBranch.PreviousNodeID)
               oCircleMarkBlock.MarkPoint(oNode.Location, 3S)
               oNode = GetNode(oBranch.NextNodeID)
               oCircleMarkBlock.MarkPoint(oNode.Location, 3S)
            End If

            '    oBranch.PrintInfo()
         Next
      End Sub
      Public Sub Close()

         If mhsPolygons IsNot Nothing Then
            mhsPolygons.Clear()
            mhsPolygons = Nothing
         End If
         If mhsBranches IsNot Nothing Then
            mhsBranches.Clear()
            mhsBranches = Nothing
         End If
         If mhsNodes IsNot Nothing Then
            mhsNodes.Clear()
            mhsNodes = Nothing
         End If
         If mdicEntities IsNot Nothing Then
            mdicEntities.Clear()
            mdicEntities = Nothing
         End If
         MyBase.Clear()

      End Sub
      Public Sub PrintInfo()

         DMAcadExt.AcadDocument.WriteMessage("---------Elements: " & CStr(MyBase.Count))
         DMAcadExt.AcadDocument.WriteMessage("Polygons: " & CStr(mhsPolygons.Count))
         DMAcadExt.AcadDocument.WriteMessage("Links: " & CStr(mhsBranches.Count))
         DMAcadExt.AcadDocument.WriteMessage("Nodes: " & CStr(mhsNodes.Count))

         Dim oPgon As tsPolygon
         For Each iPgonID As Integer In mhsPolygons
            '   If iPgonID = 58 Or iPgonID = 61 Then
            oPgon = GetPolygon(iPgonID)
            oPgon.PrintInfo()
            '  End If
         Next

         'Dim oBranch As tsBranch
         'For Each iBranchID As Integer In mhsBranches
         '    oBranch = DirectCast(MyBase.Item(iBranchID), tsBranch)
         '    oBranch = GetBranch(iBranchID)

         '    oBranch.PrintInfo()
         'Next

      End Sub
      Public Sub PrintInfo(iPolygonID As Integer)

         DMAcadExt.AcadDocument.WriteMessage("---------Elements!!!: " & CStr(MyBase.Count))
         DMAcadExt.AcadDocument.WriteMessage("Polygons: " & CStr(mhsPolygons.Count))
         DMAcadExt.AcadDocument.WriteMessage("Links: " & CStr(mhsBranches.Count))
         DMAcadExt.AcadDocument.WriteMessage("Nodes: " & CStr(mhsNodes.Count))

         Dim oPgon As tsPolygon
         For Each iPgonID As Integer In mhsPolygons
            If iPgonID = iPolygonID Then
               oPgon = GetPolygon(iPgonID)
               oPgon.PrintInfo()
            End If
         Next

         'Dim oBranch As tsBranch
         'For Each iBranchID As Integer In mhsBranches
         '    oBranch = DirectCast(MyBase.Item(iBranchID), tsBranch)
         '    oBranch = GetBranch(iBranchID)

         '    oBranch.PrintInfo()
         'Next

      End Sub
      Private Sub zzAddID(ByVal iID As Integer)
         If miMaxID < iID Then
            miMaxID = iID
         End If
      End Sub

      Protected Overrides Sub Finalize()
         Me.Close()
         MyBase.Finalize()
      End Sub
   End Class

End Namespace