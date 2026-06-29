Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices

Namespace TopoScheme
   Public Interface INodeProperty
      Property IsMust As Boolean
      Property Stage As Integer

   End Interface
   Public Class tsNode
      Inherits tsElement
      Public Shared IsGeoVertex As Boolean = False
      Private mbIsPseudoGeo As Boolean
      Private mbIsBlockingTri As Boolean
      Private mbIsUserBlocking As Boolean
      Private mbHasOldPoint As Boolean
      Private miInnerRing As Integer


      Private mtLocation As Point3d
      Private miaBrancheIDs() As Integer
      Private mbSkip As Boolean
      Private mbHasUsedPoint As Boolean

      '   Private mtAcObjID As ObjectId
      Private mbHasEntity As Boolean
      Private mbHasActiveSurveyPoint As Boolean
      Private moNodeProperty As INodeProperty
      Private msBlockName As String
      Private msLayerName As String
      Private mbIsDBPoint As Boolean
      Private msName As String
      Public Sub New(ByVal oNode As Node, iInnerRing As Integer)
			MyBase.New(oNode.ID, TopoElemType.Node)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oNode_A", oNode IsNot Nothing, oNode.ID, oNode.Location)
			Dim colNodeHalfEdges As HalfEdgeCollection
         mtLocation = New Point3d(Math.Round(oNode.Location.X, 6, MidpointRounding.AwayFromZero), Math.Round(oNode.Location.Y, 6, MidpointRounding.AwayFromZero), Math.Round(oNode.Location.Z, 6, MidpointRounding.AwayFromZero))
         miInnerRing = iInnerRing
         Try
            colNodeHalfEdges = oNode.GetEdges()
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "tsNode - New", oNode.Location.ToString())
            DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.Y, "", "Node.GetEdges problem", True)
            colNodeHalfEdges = Nothing
         End Try
			'   DMCommon.Debug.MsgBox("08_331", oNode.ID, iInnerRing, colNodeHalfEdges.Count)
			If colNodeHalfEdges IsNot Nothing Then
				ReDim miaBrancheIDs(colNodeHalfEdges.Count - 1)

				For iIndex As Integer = 0 To colNodeHalfEdges.Count - 1
					miaBrancheIDs(iIndex) = colNodeHalfEdges.Item(iIndex).FullEdge.ID
				Next
				colNodeHalfEdges.Dispose()
			Else
				DMCommon.Debug.MsgBox("08_332s", "colNodeHalfEdges IsNot Nothing")
				'oNode.Location
			End If
         '     tsNode.vb:line 195



         Try
            MyBase.AcObjID = oNode.Entity
            mbHasEntity = True
            DMAcadExt.AcadTransaction.GetNodeInfo(MyBase.AcObjID, msLayerName, msBlockName, mbIsDBPoint)

         Catch oMapEx As Autodesk.Gis.Map.MapException
            If oMapEx.ErrorCode = 2035 Then
               mbHasEntity = False
               '''''  DMAcadExt.AcadDocument.WriteDebugMessage("!#155: " & "" & oNode.ID.ToString() & "; " & oNode.Location.ToString())
            Else
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "New(ByVal oNode)")
            End If
         End Try

      End Sub
      Public Sub New(ByVal iID As Integer, ByVal tLocation As Point2d)
         MyBase.New(iID, TopoElemType.Node)
         miInnerRing = -1
      End Sub
      Public ReadOnly Property Location As Point3d
         Get
            Return mtLocation
         End Get
      End Property
      Public ReadOnly Property InnerRing As Integer
         Get
            Return miInnerRing
         End Get
      End Property
      Public ReadOnly Property IsIsthmusInner As Boolean
         Get
            Return (miInnerRing = -1)
         End Get
      End Property
      Public ReadOnly Property BranchCount As Integer
         Get
            Return miaBrancheIDs.GetUpperBound(0) + 1
         End Get
      End Property

      Public Property NodeProperty As INodeProperty
         Get
            Return moNodeProperty
         End Get
         Set(oValue As INodeProperty)
            moNodeProperty = oValue
         End Set
      End Property
      Public ReadOnly Property IsDBPoint() As Boolean
         Get
            Return mbIsDBPoint
         End Get
      End Property
      Public Property BlockName() As String
         Get
            Return msBlockName
         End Get
         Set(ByVal bValue As String)
            msBlockName = bValue
         End Set
      End Property
      Public Sub SetInnerRing(iInnerRing As Integer)
         If iInnerRing <> -1 AndAlso miInnerRing = 0 OrElse miInnerRing = -1 Then
            miInnerRing = iInnerRing
         End If
      End Sub

#Region "Vertex existence"
      Public ReadOnly Property IsPseudo() As Boolean
         Get
				Return Me.IsIsthmusInner OrElse (Me.IsPseudoTopo AndAlso Me.IsPseudoGeo AndAlso Not Me.HasOldPoint AndAlso Not Me.IsUserBlocking)  'AndAlso Not Me.IsBlockingTri 
			End Get

      End Property

      Public ReadOnly Property IsPseudoTopo() As Boolean
         Get
            
            If miaBrancheIDs Is Nothing Then
               System.Windows.Forms.MessageBox.Show("miaBrancheIDs Is Nothing", "02_874")
               Return False
            Else
               Return (miaBrancheIDs.GetUpperBound(0) = 1)
            End If

         End Get
      End Property

      Public Property IsPseudoGeo() As Boolean
         Get
				Return mbIsPseudoGeo 'AndAlso Not Me.IsUserBlocking AndAlso Not IsBlockingTri
			End Get
			Set(ByVal bValue As Boolean)

				If False AndAlso bValue Then
					Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
					oSquareMarkBlock.MarkPoint(mtLocation, 2S)

				End If
				mbIsPseudoGeo = bValue
			End Set
		End Property

      Public Property HasOldPoint() As Boolean
         Get
            Return mbHasOldPoint
         End Get
         Set(ByVal bValue As Boolean)
            mbHasOldPoint = bValue
         End Set
      End Property
      Public Property IsUserBlocking As Boolean
         Get
            Return mbIsUserBlocking OrElse IsGeoVertex
         End Get
         Set(bValue As Boolean)
            mbIsUserBlocking = bValue
         End Set
      End Property
      Public Property IsBlockingTri() As Boolean
         Get
            Return mbIsBlockingTri
         End Get
         Set(ByVal bValue As Boolean)
            mbIsBlockingTri = bValue
         End Set
      End Property
      'Public ReadOnly Property HasMustPoint As Boolean
      '   Get
      '      '   Return Me.HasOldPoint OrElse Me.IsUserBlocking OrElse IsBlockingTri
      '      Return Me.HasOldPoint
      '   End Get
      'End Property
      Public ReadOnly Property HasMustVertex As Boolean
         Get

            Return (moNodeProperty IsNot Nothing) AndAlso moNodeProperty.IsMust
         End Get
      End Property
      Public Function IsSkipVertex() As Boolean
         Dim oNodeProperty As INodeProperty
         Dim bIsMust As Boolean
         oNodeProperty = Me.NodeProperty
         If oNodeProperty Is Nothing Then
            bIsMust = False
         Else
            bIsMust = Me.NodeProperty.IsMust
         End If
			'''''''''''287
			'	DMCommon.ExcelLogAW5.SetNextValue(3, "IsSkipVertex", Me.IsPseudo, Not Me.HasUsedPoint, Not Me.HasActiveSurveyPoint, Not Me.HasOldPoint, Not Me.HasMustVertex, Not bIsMust)
			Return Me.IsPseudo And Not Me.HasUsedPoint And Not Me.HasActiveSurveyPoint And Not Me.HasOldPoint And Not Me.HasMustVertex And Not bIsMust
      End Function


#End Region

      Public ReadOnly Property TestPseudoString() As String
         Get
            Return (MyBase.ID.ToString() & "===" & Me.IsPseudoTopo.ToString() & "//" & Me.IsPseudoGeo.ToString() & "//" & Me.HasUsedPoint.ToString() & "//" & Me.HasActiveSurveyPoint.ToString() & "//" & Me.IsBlockingTri.ToString())

         End Get

      End Property







      Public Property Name() As String
         Get
            Return msName
         End Get
         Set(ByVal bValue As String)
            msName = bValue
         End Set
      End Property
#Region "Points existence"
      Public Property HasActiveSurveyPoint() As Boolean
         Get
            Return mbHasActiveSurveyPoint
         End Get
         Set(ByVal bValue As Boolean)
            mbHasActiveSurveyPoint = bValue
         End Set
      End Property

      Public Property HasUsedPoint() As Boolean
         Get
            Return mbHasUsedPoint
         End Get
         Set(ByVal bValue As Boolean)
            mbHasUsedPoint = bValue
         End Set
      End Property
#End Region

      Public Property Skip() As Boolean
         Get
            Return mbSkip
         End Get
         Set(ByVal bValue As Boolean)
            mbSkip = bValue
         End Set
      End Property
      Public ReadOnly Property Branches As Integer()
         Get
            Return miaBrancheIDs ''''''''''''''''''''''''''
         End Get
      End Property ''
      Public ReadOnly Property HasEntity As Boolean
         Get
            Return mbHasEntity
         End Get
      End Property
      Public Function GetOtherBranch(iBranchID As Integer) As Integer
         If miaBrancheIDs.GetUpperBound(0) = 1 Then
            If iBranchID = miaBrancheIDs(0) AndAlso iBranchID <> miaBrancheIDs(1) Then
               Return miaBrancheIDs(1)
            ElseIf iBranchID = miaBrancheIDs(1) AndAlso iBranchID <> miaBrancheIDs(0) Then
               Return miaBrancheIDs(0)
            Else
               Return -1
            End If
         Else
            Return -2

         End If

      End Function

 

   End Class
End Namespace