Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class tmRing
	Private Shared moNodes As TopoManager.tmNodes
	Private Shared mtPointTolerance As Autodesk.AutoCAD.Geometry.Tolerance = New Autodesk.AutoCAD.Geometry.Tolerance(0.005, 0.005)
	Private miID As Integer
	Private mbIsExterior As TriState = TriState.UseDefault
	Private moPolyline As Polyline
	Private moVertices As tmVertices
	'Private mcolEdges As ICollection(Of tmEdge)
	Private moaNodes() As tmNode
	Private mhsAllAdjRings As HashSet(Of Integer)
	Private mdicLoops As Dictionary(Of Integer, tmLoop)
	Private mssChains As tmChainSet
	Private mcolEdges As System.Collections.ObjectModel.Collection(Of tmEdge)
	Private moEdges As tmEdges

	Private Enum enDirection
		NotDefined = 0
		CounterClockwise = -1
		Clockwise = 1
	End Enum
	'tmRing.vb:line 950
   Public Sub New(iID As Integer, oPolyline As Polyline)

      miID = iID
      moPolyline = oPolyline

   
      zzCalcNew()




   End Sub
   Private Sub zzCalcNew()
      Dim iVerticesUB As Integer = moPolyline.NumberOfVertices - 1
      Dim bIsLast As Boolean


      moVertices = New tmVertices(moPolyline.GeometricExtents, iVerticesUB, moPolyline.Handle.ToString())
      mhsAllAdjRings = New HashSet(Of Integer)


      '	DMAcadExt.AcadDocument.WriteMessage(CStr(miID) & ":" & moPolyline.Handle.ToString() & "," & "Ring:" & DMAcadExt.TPlnPoint.DispPoint(moPolyline.GetPoint2dAt(0)) & "|" & DMAcadExt.TPlnPoint.DispPoint(moPolyline.GetPoint2dAt(moPolyline.NumberOfVertices - 1)))
      For iIndex As Integer = 0 To iVerticesUB
         bIsLast = (iIndex = iVerticesUB)

         moVertices.AddVertex(miID, iIndex, moPolyline.GetPoint3dAt(iIndex), bIsLast)
      Next
      moVertices.CheckPseudo(miID)
      moEdges = New tmEdges(miID, Vertices.Direction)
      If moVertices.LastIsFirst Then
         DMAcadExt.AcadDocument.WriteDebugMessage("LastIsFirst!!!!!!!!!:")
         moPolyline.RemoveVertexAt(iVerticesUB)
      End If
   End Sub
   Private Sub zzCalcNewNew()
      Dim iVerticesUB As Integer = moPolyline.NumberOfVertices - 1
      Dim bIsLast As Boolean


      moVertices = New tmVertices(moPolyline.GeometricExtents, iVerticesUB, moPolyline.Handle.ToString())
      mhsAllAdjRings = New HashSet(Of Integer)


      '	DMAcadExt.AcadDocument.WriteMessage(CStr(miID) & ":" & moPolyline.Handle.ToString() & "," & "Ring:" & DMAcadExt.TPlnPoint.DispPoint(moPolyline.GetPoint2dAt(0)) & "|" & DMAcadExt.TPlnPoint.DispPoint(moPolyline.GetPoint2dAt(moPolyline.NumberOfVertices - 1)))
      Dim iIndex As Integer = 0
      Do
         '  For iIndex As Integer = 0 To iVerticesUB
         bIsLast = (iIndex = iVerticesUB)

         If moVertices.AddVertex(miID, iIndex, moPolyline.GetPoint3dAt(iIndex), bIsLast) Then
            iIndex += 1
         Else
            moPolyline.RemoveVertexAt(iIndex)
            iVerticesUB = moPolyline.NumberOfVertices - 1
         End If
      Loop
      '    Next
      moVertices.CheckPseudo(miID)
      moEdges = New tmEdges(miID, Vertices.Direction)
      If moVertices.LastIsFirst Then
         DMAcadExt.AcadDocument.WriteMessage("LastIsFirst!!!!!!!!!:")
         moPolyline.RemoveVertexAt(iVerticesUB)
      End If
   End Sub

	Public Shared Property Nodes As tmNodes
		Get
			Return moNodes
		End Get
		Set(oValue As tmNodes)
			moNodes = oValue
		End Set
	End Property
	Public ReadOnly Property ID As Integer
		Get
			Return miID
		End Get
	End Property
	Public Property IsExterior As TriState
		Get
			Return mbIsExterior
		End Get
		Set(bValue As TriState)
			mbIsExterior = bValue
		End Set
	End Property
	Public ReadOnly Property ChainCount As Integer
		Get
			If mssChains Is Nothing Then
				Return -1
			Else
				Return mssChains.Count
			End If

		End Get
   End Property
   Public ReadOnly Property AcObjID As ObjectId
      Get
         Return moPolyline.ObjectId
      End Get
   End Property
	Public ReadOnly Property Handle As Handle
		Get
			Return moPolyline.Handle
		End Get
	End Property
	Public Function GetDir(iStartIndex As Integer, iMidIndex As Integer, iEndIndex As Integer) As Boolean
      Return moVertices.GetDir(iStartIndex, iMidIndex, iEndIndex)
   End Function
	Public Function AddNodes160215(oaNodes() As tmNode) As tmVertex()
		Dim oNode As tmNode
		'	Dim iFirstIndex As Integer
		'	Dim iLastIndex As Integer
		Dim iPrevIndex As Integer
		Dim iVertexIndex As Integer
		Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
			oNode = oaNodes(iIndex)
			If miID = 99999 Then
				oNode.DebugWrite()
			End If
			If oNode Is Nothing Then
				MessageBox.Show("SOS!!" & vbCrLf & CStr(oaNodes.GetUpperBound(0)) & vbCrLf & CStr(oaNodes.GetUpperBound(0)), "06_340")
			End If
			iVertexIndex = moVertices.ConnectNode(oNode, "")
			If iVertexIndex >= 0 Then
				If iIndex = 0 Then
					iPrevIndex = iVertexIndex
				Else
					If iDirection = enDirection.NotDefined Then
						If zzRealVertexiExists(iPrevIndex, iVertexIndex) Then
							iDirection = enDirection.CounterClockwise
						Else
							iDirection = enDirection.Clockwise
						End If
					End If
					If miID = 0 Then
						DMAcadExt.AcadDocument.WriteMessage("$$%" & CStr(iPrevIndex) & "<>" & CStr(iPrevIndex) & "; Dir=" & iDirection.ToString())
					End If
					moVertices.GetPseudoVertices(iPrevIndex, iVertexIndex, iDirection, oaMissedVertices, iMissedVertexIndex, bErr)
					iPrevIndex = iVertexIndex
					If bErr Then
						DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(oaNodes.GetUpperBound(0)) & ";" & CStr(iIndex) & "||" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
						Exit For
					End If
				End If
				Try
					mhsAllAdjRings.UnionWith(oNode.Rings)
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
				End Try
			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next
		If iMissedVertexIndex > 0 Then
			ReDim Preserve oaMissedVertices(iMissedVertexIndex - 1)
			Return oaMissedVertices
		Else
			Return Nothing
		End If

	End Function
	Public Sub ConnectNodes(oaNodes() As tmNode, sDebugNote As String)
		Dim oNode As tmNode

		Dim iVertexIndex As Integer
		Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0

		For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
			oNode = oaNodes(iIndex)

			If oNode Is Nothing Then
				MessageBox.Show("SOS!!" & vbCrLf & CStr(oaNodes.GetUpperBound(0)) & vbCrLf & CStr(oaNodes.GetUpperBound(0)), "06_340")
			End If
			iVertexIndex = moVertices.ConnectNode(oNode, sDebugNote)
			If iVertexIndex >= 0 Then
				Try
					mhsAllAdjRings.UnionWith(oNode.Rings)
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
				End Try
			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next


	End Sub
	Public Function AddNodesPrev(oaNodes() As tmNode) As tmVertex()
		Dim oNode As tmNode
		'	Dim iFirstIndex As Integer
		'	Dim iLastIndex As Integer
		Dim iFirstIndex As Integer
		Dim iPrevIndex As Integer

		Dim iVertexIndex As Integer
		Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
			oNode = oaNodes(iIndex)
			If miID = 99999 Then
				oNode.DebugWrite()
			End If
			If oNode Is Nothing Then
				MessageBox.Show("SOS!!" & vbCrLf & CStr(oaNodes.GetUpperBound(0)) & vbCrLf & CStr(oaNodes.GetUpperBound(0)), "06_340")
			End If
			iVertexIndex = moVertices.ConnectNode(oNode, "")
			If iVertexIndex >= 0 Then
				If miID = -4 Then
					DMAcadExt.AcadDocument.WriteMessage("$$!! iVertexIndex=" & CStr(iVertexIndex))
				End If

				If iIndex = 0 Then
					iFirstIndex = iVertexIndex
					iPrevIndex = iVertexIndex
				Else

					If Not moVertices.RealVertexiExists(iPrevIndex, iVertexIndex) Then

						moVertices.GetPseudoVertices(iPrevIndex, iVertexIndex, oaMissedVertices, iMissedVertexIndex, bErr)
					End If

					If miID = 0 Then
						DMAcadExt.AcadDocument.WriteMessage("$$%" & CStr(iPrevIndex) & "<>" & CStr(iIndex) & "; Dir=" & iDirection.ToString())
					End If

					iPrevIndex = iVertexIndex
					If bErr Then
						DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(oaNodes.GetUpperBound(0)) & ";" & CStr(iIndex) & "||" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
						Exit For
					End If
				End If
				Try
					mhsAllAdjRings.UnionWith(oNode.Rings)
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
				End Try
			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next
		If Not zzRealVertexiExists(iPrevIndex, iFirstIndex) Then
			moVertices.GetPseudoVertices(iPrevIndex, iFirstIndex, oaMissedVertices, iMissedVertexIndex, bErr)
		End If
		If iMissedVertexIndex > 0 Then
			ReDim Preserve oaMissedVertices(iMissedVertexIndex - 1)
			Return oaMissedVertices
		Else
			Return Nothing
		End If

	End Function
	Public ReadOnly Property Vertices As tmVertices
		Get
			Return moVertices
		End Get
	End Property
	Public ReadOnly Property Polyline As Polyline

		Get
			Return moPolyline
		End Get
	End Property
	Public Function AddIntersectPoints050315(colIntersectPoints As Autodesk.AutoCAD.Geometry.Point3dCollection, oNeighbourRing As tmRing) As System.Collections.ObjectModel.Collection(Of tmEdge)
		Dim oNode As tmNode = Nothing
		Dim oNeighbourVertices As tmVertices = oNeighbourRing.Vertices
		Dim iFirstIndex As Integer = -1
		Dim iPrevIndex As Integer
		Dim iVertexIndex As Integer
		Dim iNBVertexIndex As Integer
		Dim tPrevVertexPair As tmVertexPair
		'	Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		Dim bPseudoVertexExists, bNBPseudoVertexExists As enStatusVertexSegment
		Dim bOK, bNBOk As Boolean

		Dim oVertexSet As tmVertexSet = New tmVertexSet(miID, 99)
		Dim oVertexList As tmVertexPairList = New tmVertexPairList()
		Dim bNBForwardDirection As Boolean
		If moNodes Is Nothing Then
			moNodes = New tmNodes()
		End If
      DMAcadExt.AcadDocument.WriteDebugMessage("VertUB=" & CStr(moVertices.UB) & "; NB VertUB=" & CStr(oNeighbourVertices.UB))
		For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colIntersectPoints
			oNode = moNodes.AddPoint(miID, oNeighbourRing.ID, tPoint)
			iVertexIndex = moVertices.ConnectNode(oNode, "")
			iNBVertexIndex = oNeighbourVertices.ConnectNode(oNode, "")
			DMAcadExt.AcadDocument.WriteMessage("VPairIndex: " & CStr(iVertexIndex) & "<=>" & CStr(iNBVertexIndex) & "; P=" & tPoint.ToString() & "; N=" & oNode.Point2d.ToString())
			If iVertexIndex >= 0 AndAlso iNBVertexIndex >= 0 Then
				oVertexList.AddVertexPair(iVertexIndex, iNBVertexIndex)
				If False Then
					Try
						mhsAllAdjRings.UnionWith(oNode.Rings)
					Catch oEx As Exception
						DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
					End Try
				End If

			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next
		oVertexList.SortBy()

		Dim bBreak As Boolean
		'	Dim bStartEdge As tmVertexPair
		Dim tVertexPair As tmVertexPair
		Dim oEdge As tmEdge
		oVertexList.DebugWrite()
		Dim colEdges As System.Collections.ObjectModel.Collection(Of tmEdge) = New System.Collections.ObjectModel.Collection(Of tmEdge)
		Dim iIndex As Integer = 0
		tPrevVertexPair = oVertexList.Last
		'	tmRing.vb:line 299
		oEdge = New tmEdge(tPrevVertexPair, miID, oNeighbourRing.ID, moVertices.Direction)


		If moVertices.Direction = oNeighbourVertices.Direction Then
			bNBForwardDirection = False
		Else
			bNBForwardDirection = True
		End If
		Do
			bBreak = False
			tVertexPair = oVertexList.Item(iIndex)
			tPrevVertexPair.DebugWrite("Prev:")
			tVertexPair.DebugWrite("Current")


			bOK = moVertices.IsNextVertex(tPrevVertexPair.VertexIndex, tVertexPair.VertexIndex, True)
			bNBOk = oNeighbourVertices.IsNextVertex(tPrevVertexPair.NBVertexIndex, tVertexPair.NBVertexIndex, bNBForwardDirection)
			'DMAcadExt.AcadDocument.WriteMessage("Res:" & CStr(bOK) & "<>" & CStr(bNBOk))
			If Not bOK AndAlso Not bNBOk Then
				bPseudoVertexExists = moVertices.GetPseudoVertices(tPrevVertexPair.VertexIndex, tVertexPair.VertexIndex, oaMissedVertices, True)
				bNBPseudoVertexExists = oNeighbourVertices.GetPseudoVertices(tPrevVertexPair.NBVertexIndex, tVertexPair.NBVertexIndex, oaMissedVertices, bNBForwardDirection)
				If bPseudoVertexExists = enStatusVertexSegment.PseudoPoints AndAlso bNBPseudoVertexExists = enStatusVertexSegment.PseudoPoints Then
				Else
					bBreak = True
				End If
			ElseIf bOK AndAlso bNBOk Then
			Else
				bBreak = True
			End If

			If bBreak Then
            DMAcadExt.AcadDocument.WriteLog("Break:" & "||" & CStr(tPrevVertexPair.VertexIndex) & "<>" & CStr(tVertexPair.VertexIndex))

				If oEdge IsNot Nothing Then
					If oEdge.TryAdd(tPrevVertexPair) Then
						'oEdge.DebugWrite("E61", miID, moVertices.Direction)
						'oEdge.DebugWrite("E81", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)
						oEdge.DebugWrite("E61", miID)
						oEdge.DebugWrite("E81", oNeighbourRing.ID)
						colEdges.Add(oEdge)
					End If

				End If
				oEdge = New tmEdge(tVertexPair, miID, oNeighbourRing.ID, moVertices.Direction)



			End If
			tPrevVertexPair = tVertexPair
			iIndex += 1
			If iIndex >= oVertexList.Count Then
				Exit Do
			End If
			If bErr Then
				DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(888) & ";" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
				Exit Do
			End If

		Loop
		DMAcadExt.AcadDocument.WriteMessage("oEdge Exists" & CStr(oEdge IsNot Nothing) & ";  colEdges.Count=" & CStr(colEdges.Count))
		If oEdge IsNot Nothing Then
			If oEdge.TryAdd(tPrevVertexPair) Then
				'x	oEdge.DebugWrite("E62", miID, moVertices.Direction)
				'x	oEdge.DebugWrite("E82", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)
				'	oEdge.DebugWrite("E62", miID)
				'	oEdge.DebugWrite("E82", oNeighbourRing.ID)
				colEdges.Add(oEdge)
			End If
		End If



		If oEdge IsNot Nothing AndAlso colEdges.Count > 0 Then

			colEdges.First.TryJoin(oEdge)
			oEdge = colEdges.First
			'x oEdge.DebugWrite("E64", miID, moVertices.Direction)
			'x oEdge.DebugWrite("E84", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)
			'oEdge.DebugWrite("E64", miID)
			'oEdge.DebugWrite("E84", oNeighbourRing.ID)
		ElseIf oEdge IsNot Nothing Then
			'x	oEdge.DebugWrite("E13")
			'x	oEdge.DebugWrite("E63", miID, moVertices.Direction)
			'x	oEdge.DebugWrite("E83", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)
			colEdges.Add(oEdge)
			'	oEdge.DebugWrite("E63", miID)
			'	oEdge.DebugWrite("E83", oNeighbourRing.ID)
		End If


		'	If Not zzRealVertexiExists(iPrevIndex, iFirstIndex) Then
		'moVertices.GetPseudoVertices(iPrevIndex, iFirstIndex, oaMissedVertices, iMissedVertexIndex, bErr)
		'	End If
		DMAcadExt.AcadDocument.WriteMessage("!!!!!  colEdges.Count=" & CStr(colEdges.Count))
		moEdges.AddEdges(colEdges)
		oNeighbourRing.AddEdges(colEdges)
		DMAcadExt.AcadDocument.WriteMessage("Es1--" & "R=" & CStr(miID) & "; Dir=" & CStr(Vertices.Direction) & "R=" & CStr(moEdges.RingID) & "; Dir=" & CStr(moEdges.Direction))
		moEdges.DebugWrite("Es1")
		Return colEdges
		'	tmRing.vb:line 362
	End Function
	Public Function AddIntersectPoints(colIntersectPoints As Autodesk.AutoCAD.Geometry.Point3dCollection, oNeighbourRing As tmRing) As System.Collections.ObjectModel.Collection(Of tmEdge)
		Dim oNode As tmNode = Nothing
		Dim oNeighbourVertices As tmVertices = oNeighbourRing.Vertices

		Dim iPrevIndex As Integer
		Dim iVertexIndex As Integer
		Dim iNBVertexIndex As Integer
		Dim tPrevVertexPair As tmVertexPair
		'	Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		Dim oaNBMissedVertices() As tmVertex

		ReDim oaMissedVertices(moVertices.UB)
		ReDim oaNBMissedVertices(moVertices.UB)

		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		Dim bPseudoVertexExists, bNBPseudoVertexExists As enStatusVertexSegment
		Dim bOK, bNBOk As Boolean

		Dim oVertexSet As tmVertexSet = New tmVertexSet(miID, 99)
		Dim oVertexList As tmVertexPairList = New tmVertexPairList()
		Dim bNBForwardDirection As Boolean
		If moNodes Is Nothing Then
			moNodes = New tmNodes()
		End If
      '     DMAcadExt.AcadDocument.WriteDebugMessage("VertUB=" & CStr(moVertices.UB) & "; NB VertUB=" & CStr(oNeighbourVertices.UB))
		For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colIntersectPoints
			oNode = moNodes.AddPoint(miID, oNeighbourRing.ID, tPoint)
         DMAcadExt.AcadDocument.WriteLog("--IntPoint " & DMAcadExt.TPlnPoint.DispPoint(tPoint))
			iVertexIndex = moVertices.ConnectNode(oNode, "My-Inters")
			iNBVertexIndex = oNeighbourVertices.ConnectNode(oNode, "NB-Inters")
			'DMAcadExt.AcadDocument.WriteMessage("VPairIndex: " & CStr(iVertexIndex) & "<=>" & CStr(iNBVertexIndex) & "; P=" & tPoint.ToString() & "; N=" & oNode.Point2d.ToString())
			If iVertexIndex >= 0 AndAlso iNBVertexIndex >= 0 Then
				oVertexList.AddVertexPair(iVertexIndex, iNBVertexIndex)
				If False Then
					Try
						mhsAllAdjRings.UnionWith(oNode.Rings)
					Catch oEx As Exception
						DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
					End Try
				End If

			Else
            DMAcadExt.AcadDocument.WriteDebugMessage("??????Prob ID=" & CStr(miID) & "; " & oNode.Point2d.ToString())
            DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", "Error #167", False)
            '   DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", "Double Points", False)
            ''''''''''''''''''moVertices.PrintVertices()
				oNode.DebugWrite()
			End If
      Next
      Dim colEdges As System.Collections.ObjectModel.Collection(Of tmEdge) = New System.Collections.ObjectModel.Collection(Of tmEdge)
      If oVertexList.Count > 0 Then


         oVertexList.SortBy()

         Dim bBreak As Boolean
         '	Dim bStartEdge As tmVertexPair
         Dim tVertexPair As tmVertexPair
         Dim oEdge As tmEdge
         '	oVertexList.DebugWrite()

         Dim iIndex As Integer = 0
         tPrevVertexPair = oVertexList.Last
         '	tmRing.vb:line 299


         '  \tmRing.vb:line 468
         If moVertices Is Nothing Or oNeighbourVertices Is Nothing Then

         Else
            DMAcadExt.AcadDocument.WriteLog("1834:" & CStr(moVertices.Direction) & "<>" & CStr(oNeighbourVertices.Direction))
         End If

         If moVertices.Direction = oNeighbourVertices.Direction Then
            bNBForwardDirection = False
         Else
            bNBForwardDirection = True
         End If
         oEdge = New tmEdge(tPrevVertexPair, miID, oNeighbourRing.ID, Not bNBForwardDirection)
         Do
            bBreak = False
            tVertexPair = oVertexList.Item(iIndex)
            'tPrevVertexPair.DebugWrite("Prev:")
            'tVertexPair.DebugWrite("Current")


            bOK = moVertices.IsNextVertex(tPrevVertexPair.VertexIndex, tVertexPair.VertexIndex, True)
            bNBOk = oNeighbourVertices.IsNextVertex(tPrevVertexPair.NBVertexIndex, tVertexPair.NBVertexIndex, bNBForwardDirection)
            '	DMAcadExt.AcadDocument.WriteMessage("Res:" & CStr(bOK) & "<>" & CStr(bNBOk))
            If Not bOK AndAlso Not bNBOk Then
               bPseudoVertexExists = moVertices.GetPseudoVertices(tPrevVertexPair.VertexIndex, tVertexPair.VertexIndex, oaMissedVertices, True)
               bNBPseudoVertexExists = oNeighbourVertices.GetPseudoVertices(tPrevVertexPair.NBVertexIndex, tVertexPair.NBVertexIndex, oaNBMissedVertices, bNBForwardDirection)
               '	DMAcadExt.AcadDocument.WriteMessage("Pseudo Res:" & CStr(bPseudoVertexExists) & "<>" & CStr(bNBPseudoVertexExists))

               If bPseudoVertexExists = enStatusVertexSegment.DoubtPoints AndAlso bNBPseudoVertexExists = enStatusVertexSegment.DoubtPoints Then
                  If Not zzCompareVertexArray(oaMissedVertices, oaNBMissedVertices) Then
                     bBreak = True
                  End If
               ElseIf bPseudoVertexExists = enStatusVertexSegment.PseudoPoints AndAlso bNBPseudoVertexExists = enStatusVertexSegment.PseudoPoints Then
                  'OK
               Else
                  bBreak = True
               End If
            ElseIf bOK AndAlso bNBOk Then
            Else
               bBreak = True
            End If

            If bBreak Then
               DMAcadExt.AcadDocument.WriteLog("Break:" & "||" & CStr(tPrevVertexPair.VertexIndex) & "<>" & CStr(tVertexPair.VertexIndex))

               If oEdge IsNot Nothing Then
                  If oEdge.TryAdd(tPrevVertexPair) Then
                     '	oEdge.DebugWrite("E61", miID, moVertices.Direction)
                     '	oEdge.DebugWrite("E81", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)
                     oEdge.DebugWrite("E61", miID)
                     oEdge.DebugWrite("E81", oNeighbourRing.ID)
                     If oEdge.IsRight Then
                        colEdges.Add(oEdge)
                     End If

                  End If

               End If
               oEdge = New tmEdge(tVertexPair, miID, oNeighbourRing.ID, Not bNBForwardDirection)



            End If
            tPrevVertexPair = tVertexPair
            iIndex += 1
            If iIndex >= oVertexList.Count Then
               Exit Do
            End If
            If bErr Then
					DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(888) & ";" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
					Exit Do
            End If

         Loop
         DMAcadExt.AcadDocument.WriteLog("oEdge Exists" & CStr(oEdge IsNot Nothing) & ";  colEdges.Count=" & CStr(colEdges.Count))
         If oEdge IsNot Nothing Then
            If oEdge.TryAdd(tPrevVertexPair) Then
               'x oEdge.DebugWrite("E62", miID, moVertices.Direction)
               'x oEdge.DebugWrite("E82", oNeighbourRing.ID, oNeighbourRing.Vertices.Direction)

               'oEdge.DebugWrite("E62", miID)
               'oEdge.DebugWrite("E82", oNeighbourRing.ID)

               'x colEdges.Add(oEdge)
            End If
         End If

         zzDebugWrite(colEdges, "AfterLoop")
         oEdge.DebugWrite("Last", miID)
         If oEdge IsNot Nothing Then
            If colEdges.Count > 0 Then
               If Not colEdges.First.TryJoin(oEdge) Then
                  'oEdge.DebugWrite("E64", miID)
                  'oEdge.DebugWrite("E84", oNeighbourRing.ID)
                  If oEdge.IsRight Then
                     colEdges.Add(oEdge)
                  End If
               End If
            Else
               'oEdge.DebugWrite("E63", miID)
               'oEdge.DebugWrite("E83", oNeighbourRing.ID)
               If oEdge.IsRight Then
                  colEdges.Add(oEdge)
               End If
            End If

         End If

			' zzDebugCheck(colEdges, "END !")

			'	If Not zzRealVertexiExists(iPrevIndex, iFirstIndex) Then
			'moVertices.GetPseudoVertices(iPrevIndex, iFirstIndex, oaMissedVertices, iMissedVertexIndex, bErr)
			'	End If
			DMAcadExt.AcadDocument.WriteLog("!!!!!  colEdges.Count=" & CStr(colEdges.Count))
         moEdges.AddEdges(colEdges)
         oNeighbourRing.AddEdges(colEdges)
         DMAcadExt.AcadDocument.WriteLog("Es1--" & "R=" & CStr(miID) & "; Dir=" & CStr(Vertices.Direction) & "R=" & CStr(moEdges.RingID) & "; Dir=" & CStr(moEdges.Direction))
         moEdges.CheckVertices("?????Es1")
      End If

      Return colEdges
      '	tmRing.vb:line 362
   End Function

   Public Sub AddEdges(colEdges As System.Collections.ObjectModel.Collection(Of tmEdge))
      moEdges.AddEdges(colEdges)
      'DMAcadExt.AcadDocument.WriteMessage("Es2--" & "R=" & CStr(miID) & "; Dir=" & CStr(Vertices.Direction))
      'moEdges.DebugWrite("Es2")
   End Sub
   Public Sub Complete(iExteriorRingID As Integer)
      Dim bErr As Boolean = False
      DMAcadExt.AcadDocument.WriteMessage("!!Complete R=" & CStr(miID) & "; Cnt=" & CStr(moEdges.Count) & "; Dir=" & CStr(Vertices.Direction))
      If miID Mod 10 = 0 Then
         'MessageBox.Show(CStr(miID) & ":" & CStr(moEdges.Count), "06_900")
      End If
      If moEdges.Count > 0 Then
         Dim oPrevEdge As tmEdge = moEdges.Last.Value
         Dim iPrevEndVertex As Integer
         Dim iStartVertex As Integer
         Dim oAddEdge As tmEdge
         Dim colExterEdges As ObjectModel.Collection(Of tmEdge) = New ObjectModel.Collection(Of tmEdge)()
         moEdges.CheckVertices("Cmpl")
         For Each oEdge As tmEdge In moEdges.Values
            'oPrevEdge.DebugWrite("PE3", miID, Vertices.Direction)
            'oEdge.DebugWrite("E4", miID, Vertices.Direction)
            If False Then
               oPrevEdge.DebugWrite("PE3", miID)
               oEdge.DebugWrite("E4", miID)
            End If

            'iPrevEndVertex = oPrevEdge.EndVertex(miID, Vertices.Direction)
            iPrevEndVertex = oPrevEdge.EndVertex(miID)

            '	iStartVertex = oEdge.StartVertex(miID, Vertices.Direction)
            iStartVertex = oEdge.StartVertex(miID)

            DMAcadExt.AcadDocument.WriteMessage("Complete R=" & CStr(miID) & "; Cnt=" & CStr(moEdges.Count) & "; Vert:" & CStr(iPrevEndVertex) & "<=>" & CStr(iStartVertex) & " Dir=" & CStr(Vertices.Direction))
            If iPrevEndVertex <> iStartVertex Then
               oAddEdge = New tmEdge(iPrevEndVertex, iStartVertex, miID, iExteriorRingID, moVertices.Direction)
               'oAddEdge.DebugWrite("E7", miID, Vertices.Direction
               oAddEdge.DebugWrite("E7", miID)
               colExterEdges.Add(oAddEdge)
            End If
            oPrevEdge = oEdge
         Next
         DMAcadExt.AcadDocument.WriteMessage("Complete R=" & CStr(miID) & "; Cnt=" & CStr(colExterEdges.Count))
         For Each oEdge As tmEdge In colExterEdges
            Try
               moEdges.AddEdge(oEdge)
            Catch oEx As Exception
               bErr = True
               DMAcadExt.AcadDocument.WriteMessage("???ERR#37 " & oEx.Message)
               oEdge.DebugWrite(miID, "Err#8")
            End Try

         Next
      Else
         Dim oEdge As tmEdge = New tmEdge(0, Vertices.UB, miID, iExteriorRingID, moVertices.Direction)
         oEdge.Closed = True
         moEdges.AddEdge(oEdge)
      End If
      If bErr Then
         moEdges.DebugWrite("Es9")
      End If


   End Sub
	Public Function AddNodes(oaNodes() As tmNode) As tmVertex()
		'	Const sH As String = "259FB"
		Dim oNode As tmNode = Nothing
		'	Dim iFirstIndex As Integer
		'	Dim iLastIndex As Integer
		Dim iFirstIndex As Integer = -1
		Dim iPrevIndex As Integer

		Dim iVertexIndex As Integer
		'	Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		Dim oVertexSet As tmVertexSet = New tmVertexSet(miID, 99)
		For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
			oNode = oaNodes(iIndex)
			iVertexIndex = moVertices.ConnectNode(oNode, "")
			If iVertexIndex >= 0 Then
				oVertexSet.Add(iVertexIndex)
				Try
					mhsAllAdjRings.UnionWith(oNode.Rings)
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
				End Try
			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next
		'	DMAcadExt.AcadDocument.WriteMessage("oVertexSet.Count= " & CStr(oVertexSet.Count) & " Ring=" & CStr(miID))
		For Each iVertexIndex In oVertexSet
			If iFirstIndex = -1 Then
				iFirstIndex = iVertexIndex
				iPrevIndex = iVertexIndex
			Else
				If Not moVertices.RealVertexiExists(iPrevIndex, iVertexIndex) Then
					moVertices.GetPseudoVertices(iPrevIndex, iVertexIndex, oaMissedVertices, iMissedVertexIndex, bErr)
				End If

				If miID = 3 And iVertexIndex > 700 And iVertexIndex < 720 Then
					DMAcadExt.AcadDocument.WriteMessage("$$%  " & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
				End If

				iPrevIndex = iVertexIndex
				If bErr Then
					DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(oaNodes.GetUpperBound(0)) & ";" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
					Exit For
				End If
			End If
			Try
				mhsAllAdjRings.UnionWith(oNode.Rings)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
			End Try
		Next
		If Not zzRealVertexiExists(iPrevIndex, iFirstIndex) Then
			moVertices.GetPseudoVertices(iPrevIndex, iFirstIndex, oaMissedVertices, iMissedVertexIndex, bErr)
		End If
		If iMissedVertexIndex > 0 Then
			ReDim Preserve oaMissedVertices(iMissedVertexIndex - 1)
			Return oaMissedVertices
		Else
			Return Nothing
		End If

	End Function

	Public Function AddNodes250215(oaNodes() As tmNode) As tmVertex()
		'	Const sH As String = "259FB"
		Dim oNode As tmNode = Nothing
		'	Dim iFirstIndex As Integer
		'	Dim iLastIndex As Integer
		Dim iFirstIndex As Integer = -1
		Dim iPrevIndex As Integer

		Dim iVertexIndex As Integer
		'	Dim iDirection As enDirection = enDirection.NotDefined
		Dim oaMissedVertices() As tmVertex
		ReDim oaMissedVertices(moVertices.UB)
		Dim iMissedVertexIndex As Integer = 0
		Dim bErr As Boolean
		Dim oVertexSet As SortedSet(Of Integer) = New SortedSet(Of Integer)
		For iIndex As Integer = 0 To oaNodes.GetUpperBound(0)
			oNode = oaNodes(iIndex)
			iVertexIndex = moVertices.ConnectNode(oNode, "")
			If iVertexIndex >= 0 Then
				oVertexSet.Add(iVertexIndex)
				Try
					mhsAllAdjRings.UnionWith(oNode.Rings)
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
				End Try
			Else
				DMAcadExt.AcadDocument.WriteMessage("???Prob ")
				oNode.DebugWrite()
			End If
		Next
		'	DMAcadExt.AcadDocument.WriteMessage("oVertexSet.Count= " & CStr(oVertexSet.Count) & " Ring=" & CStr(miID))
		For Each iVertexIndex In oVertexSet
			If iFirstIndex = -1 Then
				iFirstIndex = iVertexIndex
				iPrevIndex = iVertexIndex
			Else
				If Not moVertices.RealVertexiExists(iPrevIndex, iVertexIndex) Then
					moVertices.GetPseudoVertices(iPrevIndex, iVertexIndex, oaMissedVertices, iMissedVertexIndex, bErr)
				End If

				If miID = 3 And iVertexIndex > 700 And iVertexIndex < 720 Then
					DMAcadExt.AcadDocument.WriteMessage("$$%  " & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
				End If

				iPrevIndex = iVertexIndex
				If bErr Then
					DMAcadExt.AcadDocument.WriteMessage("???! H=" & Me.Handle.ToString() & "; UB=" & CStr(oaNodes.GetUpperBound(0)) & ";" & CStr(iPrevIndex) & "<>" & CStr(iVertexIndex))
					Exit For
				End If
			End If
			Try
				mhsAllAdjRings.UnionWith(oNode.Rings)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("Err AddNodes " & CStr(miID) & ":" & oEx.Message)
			End Try
		Next
		If Not zzRealVertexiExists(iPrevIndex, iFirstIndex) Then
			moVertices.GetPseudoVertices(iPrevIndex, iFirstIndex, oaMissedVertices, iMissedVertexIndex, bErr)
		End If
		If iMissedVertexIndex > 0 Then
			ReDim Preserve oaMissedVertices(iMissedVertexIndex - 1)
			Return oaMissedVertices
		Else
			Return Nothing
		End If

	End Function
	Public Sub TmpDisplayVertices()
		Dim oVertex As tmVertex
		For iIndex As Integer = 0 To moVertices.UB
			oVertex = moVertices.Vertex(iIndex)
			oVertex.TmpDisplay()
		Next
	End Sub
	Public Sub Build(bHasBoundary As Boolean)
		zzAnalysis()
		'	MessageBox.Show("AFTERANALYSIS")
		If tmPolygon.PgonTestA = 0 Or miID = tmPolygon.PgonTestA Then
			DebugWrite("AfterAnal ", True)
		End If
		'	DebugWrite("AfterAnal ")
		If mssChains.Count > 1 Then
			zzAddInclPgons()
		End If

		If tmPolygon.PgonTestA = 0 Or miID = tmPolygon.PgonTestA Then
			DebugWrite("AfterInters", True)
		End If
		If Not bHasBoundary Then


			zzAddExterPgons()
			If tmPolygon.PgonTestA = 0 Or miID = tmPolygon.PgonTestA Then
				DebugWrite("AfterExter", True)
			End If
		End If
	End Sub
   Public Function RemoveVertices(iStartIndex As Integer, iEndIndex As Integer, bIncreaseIndex As Boolean) As Integer
      If bIncreaseIndex Then
         zzRemoveVertices(iStartIndex, iEndIndex)
         If iEndIndex > iStartIndex Then
            Return iStartIndex
         Else
            Return iStartIndex - iEndIndex
         End If

      Else
         zzRemoveVertices(iEndIndex, iStartIndex)
         If iEndIndex > iStartIndex Then
            Return 0
         Else
            Return iEndIndex + 1
         End If
      End If
      zzCalcNew()

   End Function
   Public Function zzRemoveVertices(iStartIndex As Integer, iEndIndex As Integer) As Integer
      If iEndIndex > iStartIndex Then
         For iIndex As Integer = iStartIndex + 1 To iEndIndex - 1
            moPolyline.RemoveVertexAt(iStartIndex + 1)
         Next
         Return iStartIndex
      Else
         Dim iVerticesUB As Integer = moPolyline.NumberOfVertices - 1
         For iIndex As Integer = iStartIndex + 1 To iVerticesUB
            moPolyline.RemoveVertexAt(iStartIndex + 1)
         Next
         For iIndex As Integer = 0 To iEndIndex - 1
            moPolyline.RemoveVertexAt(0)
         Next
         Return 0 'iStartIndex - iEndIndex - 1
      End If

   End Function
	Private Sub zzAddInclPgons()
		Dim oPrev As tmChain = Nothing
		Dim oFirst As tmChain = Nothing
		Dim bRes As Boolean
		'DMAcadExt.AcadDocument.WriteMessage("M000 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))
		Dim colInclChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim oAddChain As tmChain = Nothing

		For Each oChain As tmChain In mssChains.Values
			If miID = 4 Then
				DMAcadExt.AcadDocument.WriteMessage("!!! " & CStr(oChain.FirstNo) & "<=>" & CStr(oChain.LastNo) & "; Vcnt" & oChain.VertexCount & "; Cls-" & oChain.IsClosed)
			End If
			If oChain Is Nothing Then
				DMAcadExt.AcadDocument.WriteMessage("Err2026 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))
			End If
			If Not oChain.IsSingular Then
				If oPrev Is Nothing Then
					oPrev = oChain
					oFirst = oChain
				Else
					'DMAcadExt.AcadDocument.WriteMessage("M100 " & CStr(miID))
					Try
						bRes = oPrev.TryInsert(oChain, oAddChain)
						If oPrev.IsChanged Then
							colInclChains.Add(oPrev)
						End If
						If bRes Then
							oPrev = oChain
						End If
						If oAddChain IsNot Nothing Then
							colInclChains.Add(oAddChain)
						End If

						'	DMAcadExt.AcadDocument.WriteMessage("M200 " & CStr(oPrev.LastNo) & "," & CStr(oChain.FirstNo) & "||" & CStr(miID))

						'	DMAcadExt.AcadDocument.WriteMessage("M300 " & "||" & CStr(miID))

						'	DMAcadExt.AcadDocument.WriteMessage("M400 " & "||" & CStr(miID))

					Catch oEx As Exception
						DMAcadExt.AcadDocument.WriteMessage("ERR_03 " & oEx.Message & ";" & oEx.StackTrace & " Ring N" & CStr(miID))
					End Try
				End If
			End If
		Next
		oPrev.TryInsert(oFirst, oAddChain)
		If oPrev.IsChanged Then
			colInclChains.Add(oPrev)
		End If
		If oAddChain IsNot Nothing Then
			colInclChains.Add(oAddChain)
		End If

		'DMAcadExt.AcadDocument.WriteMessage("Zero chains ... " & CStr(colExterChains.Count))
		For Each oChain As tmChain In colInclChains
			If oChain.IsChanged Then
				If miID = 1 Then
					DMAcadExt.AcadDocument.WriteMessage("OldKey=" & CStr(oChain.OldKey) & "  " & CStr(oChain.Key))
				End If
				mssChains.Remove(oChain.OldKey)
			End If
			'	oChain.DebugWrite("Zero Chain ")
			mssChains.AddChain(oChain)
		Next

	End Sub
	Private Sub zzDebugWrite(colEdges As System.Collections.ObjectModel.Collection(Of tmEdge), sCaption As String)
		For Each oEdge As tmEdge In colEdges
			oEdge.DebugWrite(sCaption, miID)
		Next
	End Sub
	Private Sub zzDebugCheck(colEdges As System.Collections.ObjectModel.Collection(Of tmEdge), sCaption As String)
		For Each oEdge As tmEdge In colEdges
			If oEdge.Check(miID) Then
				oEdge.DebugWrite("?????" & sCaption, miID)
			End If

		Next
	End Sub


	Private Sub zzAddExterPgons080215()
		Dim oPrevChain As tmChain = Nothing
		Dim oFirstChain As tmChain = Nothing
		DMAcadExt.AcadDocument.WriteMessage("M000 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))
		Dim colExterChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim colRemoveChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim bNextChain As Boolean
		Dim oExterChain As tmChain
		Dim iCurrentAdjRingID As Integer = -1
		oPrevChain = mssChains.GetLastChain()
		If oPrevChain Is Nothing OrElse oPrevChain.IsClosed Then
			Return
		End If
		oPrevChain.DebugWrite("PrevInit")
		For Each oChain As tmChain In mssChains.Values
			If oChain Is Nothing Then
				DMAcadExt.AcadDocument.WriteMessage("Err2023 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))
			End If
			If miID = 81 Then
				DMAcadExt.AcadDocument.WriteMessage("++++Key " & CStr(oChain.Key))
			End If
			bNextChain = True
			If oChain.AdjRingID = iCurrentAdjRingID OrElse Not oChain.IsSingular Then
				' Not oChain.IsSingular And
				DMAcadExt.AcadDocument.WriteMessage("M100 " & CStr(miID))

				Try
					If oPrevChain.NotIncludesStrictRight(oChain.FirstNo) Then
						If miID = 81 Then
							If oPrevChain.LastNo > -1 And oPrevChain.LastNo < 530 Then
								DMAcadExt.AcadDocument.WriteMessage("M41 " & CStr(oPrevChain.FirstNo) & "<->" & CStr(oPrevChain.LastNo) & " , " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & "||" & CStr(oPrevChain.AdjRingID) & "-" & CStr(oChain.AdjRingID))
							End If
						End If
						If oPrevChain.AdjRingID <> oChain.AdjRingID OrElse zzRealVertexiExists(oPrevChain.LastNo, oChain.FirstNo) Then
							If miID = 2 Then
								If oPrevChain.LastNo > -1 And oPrevChain.LastNo < 530 Then
									DMAcadExt.AcadDocument.WriteMessage("M200 " & CStr(oPrevChain.LastNo) & "-,-" & CStr(oChain.FirstNo) & "||" & CStr(oPrevChain.AdjRingID) & "-" & CStr(oChain.AdjRingID))
								End If

							End If

							oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, 0)
							DMAcadExt.AcadDocument.WriteMessage("M300 " & "||" & CStr(miID))
							If oExterChain.TryAddLast(oChain.FirstNo) Then
								colExterChains.Add(oExterChain)
							End If
							If oPrevChain.IsChanged Then
								''''''''''colExterChains.Add(oPrevChain)
							End If
							DMAcadExt.AcadDocument.WriteMessage("M400 " & "||" & CStr(miID))
						Else
							If miID = 2 Then
								If oPrevChain.LastNo > -1 And oPrevChain.LastNo < 530 Then
									DMAcadExt.AcadDocument.WriteMessage("M33 " & CStr(oPrevChain.FirstNo) & "<->" & CStr(oPrevChain.LastNo) & " , " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & "||" & CStr(oPrevChain.AdjRingID) & "-" & CStr(oChain.AdjRingID))
								End If
							End If

							If oPrevChain.TryUnion(oChain) Then
								bNextChain = False

								colRemoveChains.Add(oChain)
							End If
						End If
					ElseIf oPrevChain.IsChanged Then
						'colExterChains.Add(oPrevChain)

					End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("ERR_03 " & oEx.Message & ";" & oEx.StackTrace & " Ring N" & CStr(miID))
				End Try
				Try
					If bNextChain Then
						If oPrevChain.IsChanged Then
							colExterChains.Add(oPrevChain)
						End If
						oPrevChain = oChain
					End If
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("ERR_02 " & oEx.Message & " Ring N" & CStr(miID))
				End Try
			End If
			Try
				iCurrentAdjRingID = oPrevChain.AdjRingID
			Catch ex As Exception
			End Try
		Next oChain
		If oPrevChain.IsChanged Then
			colExterChains.Add(oPrevChain)


		End If
		If oFirstChain IsNot Nothing Then
			If miID = 81 Then
				oFirstChain.DebugWrite("FirstChain")
			End If
			If miID = 81 Then
				oPrevChain.DebugWrite("PrevChain")
			End If
			If oPrevChain.NotIncludesStrictRight(oFirstChain.FirstNo) Then
				DMAcadExt.AcadDocument.WriteMessage("04_166 " & " oFirstChain.FirstNo = " & CStr(oFirstChain.FirstNo))
				If oPrevChain.AdjRingID <> oFirstChain.AdjRingID OrElse zzRealVertexiExists(oPrevChain.LastNo, oFirstChain.FirstNo) Then
					oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, 0)
					If oExterChain.TryAddLast(oFirstChain.FirstNo) Then
						colExterChains.Add(oExterChain)
					End If
				Else
					If oPrevChain.TryUnion(oFirstChain) Then
						bNextChain = False
						If oPrevChain.IsChanged Then
							colExterChains.Add(oPrevChain)
						End If
						colRemoveChains.Add(oFirstChain)
					End If
				End If
			End If
		End If

		For Each oChain As tmChain In colRemoveChains
			'DMAcadExt.AcadDocument.WriteMessage("M50 " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & " , " & CStr(oChain.OldKey) & "||" & CStr(oChain.AdjRingID))
			mssChains.Remove(oChain.Key)
		Next oChain
		'DMAcadExt.AcadDocument.WriteMessage("Zero chains ... " & CStr(colExterChains.Count))
		For Each oChain As tmChain In colExterChains
			'	oChain.DebugWrite("Zero Chain ")
			DMAcadExt.AcadDocument.WriteMessage("M52 " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & " , " & CStr(oChain.OldKey) & "||" & CStr(oChain.AdjRingID) & ", Ch=" & CStr(oChain.IsChanged))
			If oChain.IsChanged Then
				mssChains.DebugWriteKeys()
				DMAcadExt.AcadDocument.WriteMessage("M55 OldKey=" & CStr(oChain.OldKey) & "; " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & " , " & CStr(mssChains.Count))
				mssChains.Remove(oChain.OldKey)
				DMAcadExt.AcadDocument.WriteMessage("M56 " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & " , " & CStr(mssChains.Count))
			End If
			mssChains.AddChain(oChain)
		Next oChain

	End Sub
	Private Sub zzAddExterPgonsOLD()
		Dim oPrevChain As tmChain = Nothing
		Dim oFirstChain As tmChain = Nothing
		'DMAcadExt.AcadDocument.WriteMessage("M000 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))
		Dim colExterChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim colRemoveChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim b As Boolean
		Dim oExterChain As tmChain
		Dim iCurrentAdjRingID As Integer = -1
		For Each oChain As tmChain In mssChains.Values
			If oChain Is Nothing Then
				DMAcadExt.AcadDocument.WriteMessage("Err2023 " & "||" & CStr(mssChains.Count) & "||" & CStr(miID))

			End If
			If miID = 81 Then
				DMAcadExt.AcadDocument.WriteMessage("++++Key " & CStr(oChain.Key))
			End If
			If oChain.AdjRingID = iCurrentAdjRingID OrElse Not oChain.IsSingular Then
				If Not oChain.IsClosed Then ' Not oChain.IsSingular And
					If oPrevChain Is Nothing Then
						oPrevChain = oChain
						oFirstChain = oChain

					Else
						'DMAcadExt.AcadDocument.WriteMessage("M100 " & CStr(miID))
						b = True
						Try
							If oPrevChain.NotIncludesStrictRight(oChain.FirstNo) Then
								If oPrevChain.AdjRingID <> oChain.AdjRingID OrElse zzRealVertexiExists(oPrevChain.LastNo, oChain.FirstNo) Then
									If miID = 81 Then
										If oPrevChain.LastNo > -1 And oPrevChain.LastNo < 530 Then
											DMAcadExt.AcadDocument.WriteMessage("M200 " & CStr(oPrevChain.LastNo) & "-,-" & CStr(oChain.FirstNo) & "||" & CStr(oPrevChain.AdjRingID) & "-" & CStr(oChain.AdjRingID))
										End If

									End If

									oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, 0)
									'	DMAcadExt.AcadDocument.WriteMessage("M300 " & "||" & CStr(miID))
									If oExterChain.TryAddLast(oChain.FirstNo) Then
										colExterChains.Add(oExterChain)
									End If
									If oPrevChain.IsChanged Then
										colExterChains.Add(oPrevChain)
									End If
									'	DMAcadExt.AcadDocument.WriteMessage("M400 " & "||" & CStr(miID))
								Else
									If miID = 81 Then
										If oPrevChain.LastNo > -1 And oPrevChain.LastNo < 530 Then
											DMAcadExt.AcadDocument.WriteMessage("M33 " & CStr(oPrevChain.FirstNo) & "<->" & CStr(oPrevChain.LastNo) & " , " & CStr(oChain.FirstNo) & "<->" & CStr(oChain.LastNo) & "||" & CStr(oPrevChain.AdjRingID) & "-" & CStr(oChain.AdjRingID))
										End If
									End If
									If True Then ' TEMP
										If oPrevChain.TryUnion(oChain) Then
											b = False

											colRemoveChains.Add(oChain)
										End If
									End If

								End If
							ElseIf oPrevChain.IsChanged Then
								colExterChains.Add(oPrevChain)
							End If
						Catch oEx As Exception
							DMAcadExt.AcadDocument.WriteMessage("ERR_03 " & oEx.Message & ";" & oEx.StackTrace & " Ring N" & CStr(miID))
						End Try
						Try
							If b Then
								oPrevChain = oChain
							End If

						Catch oEx As Exception
							DMAcadExt.AcadDocument.WriteMessage("ERR_02 " & oEx.Message & " Ring N" & CStr(miID))
						End Try
					End If
				End If
			End If
			Try
				iCurrentAdjRingID = oPrevChain.AdjRingID
			Catch ex As Exception

			End Try

		Next oChain
		If oFirstChain IsNot Nothing Then
			If miID = 81 Then
				oFirstChain.DebugWrite("FirstChain")
			End If
			If miID = 81 Then
				oPrevChain.DebugWrite("PrevChain")
			End If
			If oPrevChain.NotIncludesStrictRight(oFirstChain.FirstNo) Then
				DMAcadExt.AcadDocument.WriteMessage("04_166 " & " oFirstChain.FirstNo = " & CStr(oFirstChain.FirstNo))
				If oPrevChain.AdjRingID <> oFirstChain.AdjRingID OrElse zzRealVertexiExists(oPrevChain.LastNo, oFirstChain.FirstNo) Then
					oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, 0)
					If oExterChain.TryAddLast(oFirstChain.FirstNo) Then
						colExterChains.Add(oExterChain)
					End If
				Else
					If oPrevChain.TryUnion(oFirstChain) Then
						b = False
						If oPrevChain.IsChanged Then
							colExterChains.Add(oPrevChain)
						End If
						colRemoveChains.Add(oFirstChain)
					End If
				End If
			End If
		End If

		For Each oChain As tmChain In colRemoveChains
			mssChains.Remove(oChain.Key)
		Next oChain
		'DMAcadExt.AcadDocument.WriteMessage("Zero chains ... " & CStr(colExterChains.Count))
		For Each oChain As tmChain In colExterChains
			'	oChain.DebugWrite("Zero Chain ")

			mssChains.AddChain(oChain)
		Next oChain
	End Sub

	Private Sub zzAddExterPgons()
		Dim oPrevChain As tmChain = Nothing
		Dim oFirstChain As tmChain = Nothing
		Dim oExterChain As tmChain
		Dim colExterChains As ObjectModel.Collection(Of tmChain) = New ObjectModel.Collection(Of tmChain)()
		Dim bIsNotClosed As Boolean = False
		DMAcadExt.AcadDocument.WriteMessage("ChainsCount=" & CStr(mssChains.Count))
		If mssChains.Count = 0 Then
			oExterChain = New tmChain(0, moVertices.UB)
			mssChains.AddChain(oExterChain)
		Else
			For Each oChain As tmChain In mssChains.Values
				If Not oChain.IsSingular Then

					If miID < -10 Then
						oChain.DebugWrite("**Current")
					End If
					If Not oChain.IsClosed Then
						If oPrevChain Is Nothing Then
							oPrevChain = oChain
							oFirstChain = oChain
							bIsNotClosed = True
						Else
							If miID = -1 Then
								oPrevChain.DebugWrite("Prev")
								DMAcadExt.AcadDocument.WriteMessage("oChain.FirstNo= " & CStr(oChain.FirstNo))
							End If
							If oPrevChain.NotIncludesStrictRight(oChain.FirstNo) Then
								If miID = -1 Then
									DMAcadExt.AcadDocument.WriteMessage("II oChain.FirstNo= " & CStr(oChain.FirstNo))
								End If
								If True Or oPrevChain.AdjRingID <> oChain.AdjRingID Then
									oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, oChain.FirstNo, 0)
									colExterChains.Add(oExterChain)
									If miID = -1 Then
										DMAcadExt.AcadDocument.WriteMessage("colExterChains.Count= " & CStr(colExterChains.Count))
									End If
								End If
							End If
						End If
						oPrevChain = oChain
					Else
						oChain.DebugWrite("Closed")
					End If
				End If
			Next
			If bIsNotClosed Then
				If miID = -1 Then
					oFirstChain.DebugWrite("**CurrentFirst")
				End If
				If oPrevChain.NotIncludesStrictRight(oFirstChain.FirstNo) Then
					If True Or oPrevChain.AdjRingID <> oFirstChain.AdjRingID Then
						oExterChain = New tmChain(0, moVertices.UB, oPrevChain.LastNo, oFirstChain.FirstNo, 0)
						colExterChains.Add(oExterChain)
					End If

				End If

				For Each oChain As tmChain In colExterChains
					'	oChain.DebugWrite("Zero Chain ")

					mssChains.AddChain(oChain)
				Next oChain
			End If

		End If
	End Sub
	Private Function zzCompareVertexArray(oaVertices() As tmVertex, oaNBVertices() As tmVertex) As Boolean
		Dim iUB As Integer = oaVertices.GetUpperBound(0)
		Dim tPoint, tNBPoint As Autodesk.AutoCAD.Geometry.Point2d
		If oaNBVertices.GetUpperBound(0) = iUB Then
			For iIndex As Integer = 0 To iUB
				tPoint = oaVertices(iIndex).Point2d
				tNBPoint = oaNBVertices(iIndex).Point2d
				If Not tPoint.IsEqualTo(tNBPoint, mtPointTolerance) Then
					Return False
				End If
			Next
			Return True
		Else
			Return False
		End If
	End Function
	Private Function zzRealVertexiExists(iVertexANo As Integer, iVertexBNo As Integer) As Boolean
		Dim iIndex As Integer = iVertexANo
		Do
			iIndex = (iIndex + 1) Mod (moVertices.UB + 1)


			If iIndex = iVertexBNo Then
				Return False
			End If
		Loop While zzVertexIsPseudo(iIndex)
		'	DMAcadExt.AcadDocument.WriteMessage("ISNOT Pseudo! " & CStr(iIndex))
		Return True
	End Function

	Private Function zzVertexIsPseudo(iVertexNo As Integer) As Boolean
		Dim oVertex As tmVertex = moVertices.Vertex(iVertexNo)
		If oVertex.IsPseudo AndAlso oVertex.Node Is Nothing Then
			Return True
		Else
			Return False
		End If
	End Function
	Private Sub zzAnalysis()
		Dim iAdjacentRingsCount As Integer
		Dim oLoop As tmLoop
		Dim oNode As tmNode
		Dim oVertex As tmVertex

		mssChains = New tmChainSet()
		moaNodes = moVertices.GetAllNodes()
		DMAcadExt.AcadDocument.WriteMessage("Nodes.UB = " & moaNodes.GetUpperBound(0).ToString() & "; mhsAllRings.Count=" & CStr(mhsAllAdjRings.Count))
		mhsAllAdjRings.Remove(miID)
		iAdjacentRingsCount = mhsAllAdjRings.Count
		mdicLoops = New Dictionary(Of Integer, tmLoop)
		For Each iAdjRingID As Integer In mhsAllAdjRings
			If iAdjRingID <> miID Then
				oLoop = New tmLoop(iAdjRingID, moVertices.UB)
				mdicLoops.Add(iAdjRingID, oLoop)
			End If
		Next
		oLoop = Nothing
		DMAcadExt.AcadDocument.WriteMessage("mdicLoops.Count = " & mdicLoops.Count.ToString() & " VertUB=" & CStr(moVertices.UB))


		If False And (mbIsExterior = TriState.False) Then
			Dim oChain As tmChain = New tmChain(0, moVertices.UB)
			mssChains.AddChain(oChain)
			'	MessageBox.Show(CStr(mssChains.Count), "03_540")
		Else
			For iVertIndex As Integer = 0 To moVertices.UB
				oVertex = moVertices.Vertex(iVertIndex)
				oNode = oVertex.Node

				If oNode IsNot Nothing Then
					For Each iAdjRingID As Integer In oNode.Rings
						If iAdjRingID <> miID Then
							If mdicLoops.TryGetValue(iAdjRingID, oLoop) Then
                        oLoop.AddVertex(iVertIndex, oNode.ID)
                     Else
                        If miID = 3 And iVertIndex > 700 And iVertIndex < 800 Then
                           DMAcadExt.AcadDocument.WriteMessage("mdicLoops Not Found" & " VertNo=" & CStr(iVertIndex) & " Ring N" & CStr(miID) & " pt=" & DMAcadExt.TPlnPoint.DispPoint(oVertex.Point2d))
                        End If
							End If
						End If
					Next

					If oNode.Rings.Count = 0 Then
						If miID = 3 And iVertIndex > 700 And iVertIndex < 800 Then
							DMAcadExt.AcadDocument.WriteMessage("Rings.Count = 0" & " VertNo=" & CStr(iVertIndex) & " Ring N" & CStr(miID) & " pt=" & DMAcadExt.TPlnPoint.DispPoint(oVertex.Point2d))
						End If
					Else
						If iVertIndex = 703 Then
							DMAcadExt.AcadDocument.WriteMessage("oNode.DebugWrite " & " VertNo=" & CStr(iVertIndex))
							oNode.DebugWrite()

						End If
					End If
				Else
					If miID = 3 And iVertIndex > 700 And iVertIndex < 800 Then
						DMAcadExt.AcadDocument.WriteMessage("Node Is Nothing" & " VertNo=" & CStr(iVertIndex) & " Ring N" & CStr(miID) & " pt=" & DMAcadExt.TPlnPoint.DispPoint(oVertex.Point2d))
					End If

				End If
			Next
			For Each oLoop In mdicLoops.Values

				oLoop.Calculate()
				'DMAcadExt.AcadDocument.WriteMessage("Loops RingID,Count = " & oLoop.AdjRingID.ToString() & "," & oLoop.Count.ToString())
				For iIndex As Integer = 0 To oLoop.Count - 1
					Try
						If miID = 4 Then
							oLoop.Item(iIndex).DebugWrite("AnA")
						End If

						mssChains.AddChain(oLoop.Item(iIndex))
						If miID = 4 Then
							oLoop.Item(iIndex).DebugWrite("R4 Ch ")
							DMAcadExt.AcadDocument.WriteMessage("mssChains.Count = " & mssChains.Count.ToString())

						End If

					Catch oEx As Exception
						DMAcadExt.AcadDocument.WriteMessage("ERR_01 " & oEx.Message & " Ring N=" & CStr(miID))
					End Try

				Next
			Next

		End If

		If miID = 999999 Then
			Me.TmpDisplayVertices()
		End If
	End Sub
	Public Sub DebugWrite(sCaption As String, bAll As Boolean)
		If bAll Then
			DMAcadExt.AcadDocument.WriteMessage(sCaption & " Chain Count " & CStr(mssChains.Count) & " Ring N=" & CStr(miID))
		End If

		For Each oChain As tmChain In mssChains.Values
			If bAll Then
				oChain.DebugWrite(sCaption)
			End If

		Next
	End Sub
	Public Sub CreatePolylines(Optional bDebug As Boolean = False)
		Dim iAdjacentRingID As Integer
		Dim iNodeID As Integer
		Dim sTest As String = "NOTH"
		If mssChains IsNot Nothing Then
			sTest = CStr(mssChains.Count)
		End If
		If bDebug Then
			DMAcadExt.AcadDocument.WriteMessage("MyRingID=" & CStr(miID) & "||" & "AdjacentRingID=" & CStr(iAdjacentRingID) & "||" & "IsExterior=" & mbIsExterior.ToString() & "||" & sTest, "09_998A")
		End If
		If mssChains IsNot Nothing Then

			For Each oChain As tmChain In mssChains.Values
				If bDebug Then
					DMAcadExt.AcadDocument.WriteMessage("Singul=" & oChain.IsSingular.ToString() & "||" & "CLOSED=" & oChain.IsClosed.ToString() & "||" & "IsExterior=" & mbIsExterior.ToString() & "||" & sTest)
				End If
				If bDebug Then
					oChain.DebugWrite("Plines")
				End If
				If Not oChain.IsSingular Then
					iAdjacentRingID = oChain.AdjRingID
					If bDebug Then
						DMAcadExt.AcadDocument.WriteMessage("AdjacentRingID=" & CStr(iAdjacentRingID) & "||" & "IsExterior=" & mbIsExterior.ToString())
					End If
					iNodeID = oChain.FirstNodeID
					If iAdjacentRingID = 0 OrElse iAdjacentRingID > miID Then
						'	oChain.DebugWrite("CrPline!!")
						zzCreateDBPolyline(oChain)
					End If
				End If
			Next
		Else
			MessageBox.Show("RingID=" & CStr(miID) & vbCrLf & "IsExterior=" & mbIsExterior.ToString())
		End If
	End Sub
	Public Sub DrawEdges(Optional bDebug As Boolean = False)
		For Each oEdge As tmEdge In moEdges.Values
			If oEdge.RingID = miID Then
				oEdge.DebugWrite(miID, "DRAW")
				CreateDBPolyline(oEdge)
			End If

		Next
	End Sub
	Private Function zzCreateDBPolyline(oChain As tmChain) As ObjectId

		If mbIsExterior = TriState.False Then
			'DMAcadExt.AcadDocument.WriteMessage("Inter!!! RingID=" & CStr(miID) & " VertexCount=" & " " & CStr(oChain.VertexCount) & " VerUB=" & CStr(oChain.VerticesUB) & vbCrLf & CStr(oChain.FirstNo) & "<-->" & CStr(oChain.LastNo), "09_980c")
		Else
			'DMAcadExt.AcadDocument.WriteMessage("RingID=" & CStr(miID) & " VertexCount=" & " " & CStr(oChain.VertexCount) & " VerUB=" & CStr(oChain.VerticesUB))
		End If
		Dim oPolyline As Polyline = New Polyline(oChain.VertexCount)
		Dim oVertex As tmVertex
		Dim oNode As tmNode
		Dim oPoint As Autodesk.AutoCAD.Geometry.Point2d

		For iIndex As Integer = 0 To oChain.VertexCount - 1
			Try
				oVertex = moVertices.Vertex(oChain.Item(iIndex))
				oNode = oVertex.Node
				If oNode Is Nothing Then
					oPoint = oVertex.Point2d
				Else
					oPoint = oNode.Point2d
				End If
				If mbIsExterior = TriState.False Then
					''''''''''''''''''''''	DMAcadExt.AcadDocument.WriteMessage("@Index=" & CStr(iIndex) & " " & DMAcadExt.TPlnPoint.DispPoint(oPoint))
				End If

				oPolyline.AddVertexAt(iIndex, oPoint, 0.0, 0.0, 0.0)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("Err  IND=" & CStr(iIndex) & "|" & oEx.Message)
				'	MessageBox.Show(CStr(iIndex) & ":" & CStr(oChain.VertexCount), "26_132")
			End Try
		Next
		If oChain.IsClosed Then
			oPolyline.Closed = True
		End If
		'oPolyline.ColorIndex = miID
		Return DMAcadExt.AcadTransaction.AppendEntity(oPolyline)
	End Function
	Public Function CreateDBPolyline(oEdge As tmEdge) As ObjectId

		If mbIsExterior = TriState.False Then
			'DMAcadExt.AcadDocument.WriteMessage("Inter!!! RingID=" & CStr(miID) & " VertexCount=" & " " & CStr(oChain.VertexCount) & " VerUB=" & CStr(oChain.VerticesUB) & vbCrLf & CStr(oChain.FirstNo) & "<-->" & CStr(oChain.LastNo), "09_980c")
		Else
			'DMAcadExt.AcadDocument.WriteMessage("RingID=" & CStr(miID) & " VertexCount=" & " " & CStr(oChain.VertexCount) & " VerUB=" & CStr(oChain.VerticesUB))
		End If
		'oEdge.DebugWrite("S6", miID, Vertices.Direction)
		oEdge.DebugWrite("S6", miID)
		'		Dim iStartVertex As Integer = oEdge.StartVertex(miID, Vertices.Direction)
		'		Dim iEndVertex As Integer = oEdge.EndVertex(miID, Vertices.Direction)

		Dim iStartVertex As Integer = oEdge.StartVertex(miID)
		Dim iEndVertex As Integer = oEdge.EndVertex(miID)

		DMAcadExt.AcadDocument.WriteMessage("!!@@ " & CStr(miID) & "; " & CStr(Vertices.Direction) & "; " & CStr(iStartVertex) & "; " & CStr(iEndVertex))

		Dim oPolyline As Polyline = New Polyline(moVertices.VertexCount(iStartVertex, iEndVertex))
		Dim oVertex As tmVertex
		Dim oNode As tmNode
		Dim oPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim iIndex As Integer = 0
		If False Then
			If oEdge.Direction = Vertices.Direction Then
				iStartVertex = oEdge.Start.VertexIndex
				iEndVertex = oEdge.End.VertexIndex
			Else
				iStartVertex = oEdge.End.VertexIndex
				iEndVertex = oEdge.Start.VertexIndex
			End If
			iStartVertex = oEdge.Start.VertexIndex
			iEndVertex = oEdge.End.VertexIndex
		End If


		oVertex = moVertices.Vertex(iStartVertex)
		Do
			Try

				oNode = oVertex.Node
				If oNode Is Nothing Then
					oPoint = oVertex.Point2d
				Else
					oPoint = oNode.Point2d
				End If
				If mbIsExterior = TriState.False Then
					''''''''''''''''''''''	DMAcadExt.AcadDocument.WriteMessage("@Index=" & CStr(iIndex) & " " & DMAcadExt.TPlnPoint.DispPoint(oPoint))
				End If

				oPolyline.AddVertexAt(iIndex, oPoint, 0.0, 0.0, 0.0)
				iIndex += 1
				If oVertex.Index = iEndVertex Then
					Exit Do
				End If
				oVertex = moVertices.GetNextVertex(oVertex.Index)

			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("Err  IND=" & CStr(iIndex) & "|" & oEx.Message)
				'	MessageBox.Show(CStr(iIndex) & ":" & CStr(oChain.VertexCount), "26_132")
			End Try
			If iIndex > Vertices.UB Then
            MessageBox.Show(CStr(iIndex) & ":" & CStr(miID) & vbCrLf & CStr(Vertices.UB) & vbCrLf & moPolyline.Handle.ToString(), "26_177")
				oEdge.DebugWrite("???????" & moPolyline.Handle.ToString() & "|", miID)
				Exit Do
			End If
		Loop
		If oEdge.Closed Then
			oPolyline.Closed = True
		End If

		'oPolyline.ColorIndex = miID
		Return DMAcadExt.AcadTransaction.AppendEntity(oPolyline)
	End Function
	Private Class tmLoop
		Inherits System.Collections.ObjectModel.Collection(Of tmChain)
		Private miAdjRingID As Integer
		Private miVerticesUB As Integer
		Private iaVertices() As Integer
		'Private iCurrentVertexNo As Integer = -1
		Private moCurrentChain As tmChain = Nothing
		Private mdicChaines As Dictionary(Of Integer, tmChain)
		Public Sub New(iAdjRingID As Integer, iVerticesUB As Integer)
			ReDim iaVertices(iVerticesUB)
			miVerticesUB = iVerticesUB
			miAdjRingID = iAdjRingID
			mdicChaines = New Dictionary(Of Integer, tmChain)
		End Sub
		Public Sub AddVertex(iVertexNo As Integer, iNodeID As Integer)
			If moCurrentChain Is Nothing Then
				moCurrentChain = New tmChain(miAdjRingID, miVerticesUB, iVertexNo, iNodeID)
				Me.Add(moCurrentChain)
				mdicChaines.Add(iNodeID, moCurrentChain)
			ElseIf Not moCurrentChain.TryAddNext(iVertexNo) Then
				moCurrentChain = New tmChain(miAdjRingID, miVerticesUB, iVertexNo, iNodeID)
				Me.Add(moCurrentChain)
				mdicChaines.Add(iNodeID, moCurrentChain)
			End If
		End Sub
		Public Sub AddVertexProba(iVertexNo As Integer, iNodeID As Integer)
			Dim iVertex As Integer
			If moCurrentChain Is Nothing Then
				moCurrentChain = New tmChain(miAdjRingID, miVerticesUB, iVertexNo, iNodeID)
				Me.Add(moCurrentChain)
				mdicChaines.Add(iNodeID, moCurrentChain)
			Else
				iVertex = moCurrentChain.LastNo
				iVertex = (iVertex + 1) Mod (miVerticesUB + 1)

				moCurrentChain = New tmChain(miAdjRingID, miVerticesUB, iVertexNo, iNodeID)
				Me.Add(moCurrentChain)
				mdicChaines.Add(iNodeID, moCurrentChain)
			End If
		End Sub
		Public ReadOnly Property AdjRingID As Integer
			Get
				Return miAdjRingID
			End Get
		End Property

		Public Sub Calculate()
			If Me.Count > 0 Then


				Dim oFirstChain As tmChain = Me.First
				Dim oLastChain As tmChain = Me.Last
				'DMAcadExt.AcadDocument.WriteMessage("CalcLOOP " &  & "|" & oEx.Message)

				'DMAcadExt.AcadDocument.WriteMessage("miVerticesUB= " & CStr(miVerticesUB) & "||||" & CStr(Me.Count))
				If Me.Count = 1 Then
					Dim oChain As tmChain = Me.Item(0)
					oChain.TryClose()

				ElseIf Me.Count > 1 Then
					If oLastChain.UnionWith(oFirstChain) Then
						Me.RemoveAt(0)
					End If

				End If
				'	Me.Item(0).DebugWrite("AfterCalc")
			Else
				DMAcadExt.AcadDocument.WriteMessage("** LOOP Is EMPTY miAdjRingID=" & CStr(miAdjRingID))
			End If
		End Sub
		Public Sub DebugWrite()
			For iIndex As Integer = 0 To Me.Count - 1
				Me.Item(iIndex).DebugWrite("In Loop")
			Next

		End Sub
	End Class
	Private Class tmChain
		Const ulZero As ULong = 0UL
		Const mulShift As ULong = 1000000UL
		Private miAdjRingID As Integer
		Private miVerticesUB As Integer
		Private miFirstNodeID As Integer
		Private miFirstNo As Integer
		Private miLastNo As Integer
		Private mbClosed As Boolean
		Private mulOldKey As ULong = ulZero
		Public Sub New(iAdjRingID As Integer, iVerticesUB As Integer, iFirstVertexNo As Integer, iFirstNodeID As Integer)

			miAdjRingID = iAdjRingID
			miVerticesUB = iVerticesUB
			miFirstNodeID = iFirstNodeID
			miFirstNo = iFirstVertexNo
			miLastNo = iFirstVertexNo
		End Sub
		Public Sub New(iAdjRingID As Integer, iVerticesUB As Integer, iFirstVertexNo As Integer, iLastVertexNo As Integer, iFirstNodeID As Integer)

			miAdjRingID = iAdjRingID
			miVerticesUB = iVerticesUB
			miFirstNodeID = iFirstNodeID

			miFirstNo = iFirstVertexNo
			miLastNo = iLastVertexNo
			DMAcadExt.AcadDocument.WriteMessage("CHAIN NEW= " & CStr(miFirstNo) & "<-->" & CStr(miLastNo))
		End Sub
		Public Sub New(iAdjRingID As Integer, iVerticesUB As Integer)
			miAdjRingID = iAdjRingID
			miVerticesUB = iVerticesUB
			miFirstNo = 0
			miLastNo = 0
			mbClosed = True


		End Sub
		Public ReadOnly Property Key As ULong
			Get
				Return Convert.ToUInt64(miFirstNo + 1) * mulShift - Convert.ToUInt64(Me.VertexCount)
			End Get
		End Property
		
		Public ReadOnly Property OldKey As ULong
			Get
				Return mulOldKey
			End Get
		End Property
		Public ReadOnly Property IsChanged As Boolean
			Get
				Return (mulOldKey <> ulZero)
			End Get
		End Property
		Public ReadOnly Property IsSingular As Boolean
			Get
				Return (miFirstNo = miLastNo) AndAlso Not mbClosed
			End Get
		End Property
		Public ReadOnly Property IsClosed As Boolean
			Get
				Return (miFirstNo = miLastNo) AndAlso mbClosed
			End Get
		End Property
		Public ReadOnly Property FirstNo As Integer
			Get
				Return miFirstNo
			End Get
		End Property
		Public ReadOnly Property LastNo As Integer
			Get
				Return miLastNo
			End Get
		End Property

		Public ReadOnly Property AdjRingID As Integer
			Get
				Return miAdjRingID
			End Get
		End Property
		Public ReadOnly Property VerticesUB As Integer
			Get
				Return miVerticesUB
			End Get
		End Property
		Public ReadOnly Property FirstNodeID As Integer
			Get
				Return miFirstNodeID
			End Get
		End Property
		Public ReadOnly Property Item(iIndex As Integer) As Integer
			Get
				Return (miFirstNo + iIndex) Mod (miVerticesUB + 1)
			End Get
		End Property
		Public ReadOnly Property VertexCount As Integer
			Get
				If Me.IsClosed Then
					Return (miVerticesUB + 1)
				Else
					Return (miLastNo - miFirstNo + miVerticesUB + 1) Mod (miVerticesUB + 1) + 1
				End If


				'	=MOD(B2-A2,C2+1)+1
			End Get
		End Property
		Public ReadOnly Property HasZero As Boolean
			Get
				Return (miLastNo < miFirstNo)
			End Get
		End Property
		Public Function TryAddNext(iVertexNo As Integer) As Boolean
			If iVertexNo = (miLastNo + 1) Mod (miVerticesUB + 1) Then
				miLastNo = iVertexNo
				Return True
			Else
				Return False
			End If
			DMAcadExt.AcadDocument.WriteMessage("TryAddNext " & CStr(miVerticesUB) & "|||" & CStr(iVertexNo) & " : " & CStr(miFirstNo) & "/" & CStr(miLastNo))
		End Function
		Public Function IncludesStrict(iVertexNo As Integer) As Boolean
			If mbClosed Then
				Return True

			ElseIf Me.HasZero Then
				If iVertexNo < miLastNo OrElse iVertexNo > miFirstNo Then
					Return True
				Else
					Return False
				End If
			Else
				If iVertexNo < miLastNo AndAlso iVertexNo > miFirstNo Then
					Return True
				Else
					Return False
				End If
			End If

		End Function

		Public Function NotIncludesStrict(iVertexNo As Integer) As Boolean
			If mbClosed Then
				Return False

			ElseIf Me.HasZero Then
				If iVertexNo > miLastNo AndAlso iVertexNo < miFirstNo Then
					Return True
				Else
					Return False
				End If
			Else
				If iVertexNo > miLastNo OrElse iVertexNo < miFirstNo Then
					Return True
				Else
					Return False
				End If
			End If

		End Function
		Public Function NotIncludesStrictRight(iVertexNo As Integer) As Boolean
			If mbClosed Then
				Return False

			ElseIf Me.HasZero Then
				If iVertexNo > miLastNo AndAlso iVertexNo <= miFirstNo Then
					Return True
				Else
					Return False
				End If
			Else
				If iVertexNo > miLastNo OrElse iVertexNo <= miFirstNo Then
					Return True
				Else
					Return False
				End If
			End If

		End Function
		Public Function TryClose() As Boolean
			If miFirstNo = (miLastNo + 1) Mod (miVerticesUB + 1) Then
				miLastNo = miFirstNo
				DMAcadExt.AcadDocument.WriteMessage("TryClose= " & CStr(miFirstNo) & "<-->" & CStr(miLastNo))
				mbClosed = True
				Return True
			Else
				Return False
			End If

		End Function
		Public Function TryUnion(oChain As tmChain) As Boolean
			If Me.NotIncludesStrict(oChain.FirstNo) AndAlso Me.NotIncludesStrict(oChain.LastNo) AndAlso oChain.NotIncludesStrict(Me.FirstNo) Then
				zzSaveKey()
				miLastNo = oChain.LastNo
				''''''''DMAcadExt.AcadDocument.WriteMessage("!!!+++ " & CStr(miFirstNo) & "<-->" & CStr(miLastNo))
				Return True
			Else
				DMAcadExt.AcadDocument.WriteMessage("????? " & CStr(miLastNo) & "<-->" & CStr(oChain.FirstNo))
				Return False
			End If
		End Function
		Public Function TryInsert(oChain As tmChain, ByRef oAddChain As tmChain) As Boolean
			'	DMAcadExt.AcadDocument.WriteMessage("TryInsert= " & CStr(oChain.FirstNo) & "<-->" & CStr(oChain.LastNo) & " IN " & CStr(Me.FirstNo) & "<-->" & CStr(Me.LastNo))
			If Me.miFirstNo = oChain.FirstNo AndAlso Me.IncludesStrict(oChain.LastNo) Then
				zzSaveKey()
				miFirstNo = oChain.LastNo
				oAddChain = Nothing
				If mbClosed Then
					mbClosed = False
				End If
				'	DMAcadExt.AcadDocument.WriteMessage("Insert res=  Case 1 " & CStr(Me.FirstNo) & "<-->" & CStr(Me.LastNo))
				Return False
			ElseIf Me.IncludesStrict(oChain.FirstNo) AndAlso Me.miLastNo = oChain.LastNo Then
				zzSaveKey()
				miLastNo = oChain.FirstNo
				oAddChain = Nothing
				'	DMAcadExt.AcadDocument.WriteMessage("Insert res=  Case 2 " & CStr(Me.FirstNo) & "<-->" & CStr(Me.LastNo))
				Return True
			ElseIf Me.IncludesStrict(oChain.FirstNo) AndAlso Me.IncludesStrict(oChain.LastNo) Then
				zzSaveKey()
				oAddChain = New tmChain(miAdjRingID, miVerticesUB, Me.FirstNo, oChain.FirstNo, Me.FirstNodeID)
				'	oAddChain.DebugWrite("**AddChain")
				miFirstNo = oChain.LastNo
				If mbClosed Then
					mbClosed = False
				End If

				'	DMAcadExt.AcadDocument.WriteMessage("Insert res=  Case 3 " & CStr(Me.FirstNo) & "<-->" & CStr(Me.LastNo) & "Add=" & CStr(oAddChain.FirstNo) & "<-->" & CStr(oAddChain.LastNo))
				Return False
			Else
				oAddChain = Nothing
				' OK impossible DMAcadExt.AcadDocument.WriteMessage("MyERR TryInsert " & " Ring N=" & CStr(miAdjRingID))
				'	DMAcadExt.AcadDocument.WriteMessage("Insert res=  Case 4 NO " & CStr(Me.FirstNo) & "<-->" & CStr(Me.LastNo))
				Return True
			End If
		End Function
		Public Function TryInsertLast(iVertexNo As Integer) As Boolean
			If Me.HasZero Then
				If iVertexNo < miLastNo OrElse iVertexNo > miFirstNo Then
					miLastNo = iVertexNo
					Return True
				Else
					Return False
				End If
			Else
				If iVertexNo < miLastNo AndAlso iVertexNo > miFirstNo Then
					miLastNo = iVertexNo
					Return True
				ElseIf iVertexNo < miFirstNo Then
					miLastNo = iVertexNo
					Return True
				Else
					Return False
				End If
			End If

		End Function
		Public Function TryAddLast(iVertexNo As Integer) As Boolean
			If Me.HasZero Then
				If iVertexNo > miLastNo AndAlso iVertexNo < miFirstNo Then
					miLastNo = iVertexNo
					Return True
				Else
					Return False
				End If
			Else
				If iVertexNo > miLastNo AndAlso iVertexNo <= miVerticesUB Then
					miLastNo = iVertexNo
					Return True
				ElseIf iVertexNo < miFirstNo Then
					miLastNo = iVertexNo
					Return True
				Else
					Return False
				End If
			End If

		End Function
		Public Function UnionWith(oChain As tmChain) As Boolean
			If oChain.FirstNo = (miLastNo + 1) Mod (miVerticesUB + 1) Then
				'DMAcadExt.AcadDocument.WriteMessage("Last+1= " & CStr(miLastNo + 1) & " FirstNo " & CStr(oChain.FirstNo))
				miLastNo = oChain.LastNo
				If miFirstNo = miLastNo Then
					mbClosed = True
				End If
				Return True
			Else
				Return False
			End If
		End Function
		Public Function Contain(oChain As tmChain) As Boolean
			If mbClosed Then
				Return True
			Else
				Return (miFirstNo < oChain.FirstNo) AndAlso (miFirstNo < oChain.LastNo)
			End If

		End Function
		Public Sub Regularization()

		End Sub
		Public Sub DebugWrite(sCaption As String)
			Dim sClosed As String
			If Me.IsClosed Then

				If Me.IsSingular Then
					sClosed = " Cl,S"
				Else
					sClosed = " Cl"
				End If
			Else
				If Me.IsSingular Then
					sClosed = " ,S"
				Else
					sClosed = String.Empty
				End If


			End If
			'" Key=" & CStr(Me.Key) &
			DMAcadExt.AcadDocument.WriteMessage(sCaption & " R=" & CStr(miAdjRingID) & " : " & CStr(miFirstNo) & "/" & CStr(miLastNo) & sClosed & "; VertCnt=" & CStr(Me.VertexCount))
		End Sub
		Private Function zzGetRealNext() As Integer
			Dim iIndex As Integer = miLastNo
			Do
				iIndex = (iIndex + 1) Mod (miVerticesUB + 1)
				If IncludesStrict(iIndex) Then
					Return -1
				Else

				End If
			Loop
		End Function
		Private Sub zzSaveKey()
			If mulOldKey = ulZero Then
				mulOldKey = Me.Key
			End If

		End Sub
	End Class
	Private Class tmChainSet
		Inherits SortedList(Of ULong, tmChain)
		Public Sub AddChain(oChain As tmChain)
			Try

				MyBase.Add(oChain.Key, oChain)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("ERR_07 " & oEx.Message & " Key=" & CStr(oChain.FirstNo) & "|" & CStr(oChain.VertexCount))
				oChain.DebugWrite("Err Insert")
			End Try


		End Sub
		Public Function GetLastChain() As tmChain
			Dim iUB As Integer = MyBase.Count - 1
			If iUB >= 0 Then
				Return MyBase.Values(iUB)
			Else
				Return Nothing
			End If
		End Function
		Public Sub DebugWriteKeys()
			For Each oChainKey As ULong In MyBase.Keys
				DMAcadExt.AcadDocument.WriteMessage("^30 Key=" & CStr(oChainKey) & "|")
			Next

			For Each oChain As tmChain In MyBase.Values
				DMAcadExt.AcadDocument.WriteMessage("^32 Key=" & CStr(oChain.Key) & "|" & CStr(oChain.FirstNo) & "<>" & CStr(oChain.LastNo))
			Next

		End Sub
		Public Sub New()
			MyBase.New()
		End Sub
	End Class
	Private Class tmVertexPairList
		Inherits List(Of tmVertexPair)


		Public Sub SortBy()
			Dim oComparer As VertexPairComparer = New VertexPairComparer()
			MyBase.Sort(oComparer)
		End Sub
		Public Sub AddVertexPair(iVertexIndex As Integer, iNBVertexIndex As Integer)
			MyBase.Add(New tmVertexPair(iVertexIndex, iNBVertexIndex))
		End Sub
		Public ReadOnly Property LastItem As tmVertexPair
			Get
				Return MyBase.Item(MyBase.Count - 1)
			End Get
		End Property
		Public Sub DebugWrite()
			Dim tVertexPair As tmVertexPair
			For i As Integer = 0 To MyBase.Count - 1
				tVertexPair = MyBase.Item(i)
				tVertexPair.DebugWrite("AA1 ")
			Next


		End Sub
	End Class

	Private Class VertexPairComparer
		Implements IComparer(Of tmVertexPair)

		Public Function Compare(VertexPairA As tmVertexPair, VertexPairB As tmVertexPair) As Integer Implements System.Collections.Generic.IComparer(Of tmVertexPair).Compare
			Return VertexPairA.VertexIndex.CompareTo(VertexPairB.VertexIndex)
		End Function
	End Class


	Private Class tmPreedge
		Private miMyRingID As Integer

		Private miAdjRingID As Integer
		Private miVerticesUB As Integer
		Private oNodeSet As SortedSet(Of Integer) = New SortedSet(Of Integer)
	End Class


End Class
Public Structure tmVertexPair
	Public VertexIndex As Integer
	Public NBVertexIndex As Integer
	Private mbIsInstance As Boolean
	Public Sub New(iVertexIndex As Integer, iNBVertexIndex As Integer)
		VertexIndex = iVertexIndex
		NBVertexIndex = iNBVertexIndex
		mbIsInstance = True
	End Sub
	Public Sub New(iVertexIndex As Integer)
		VertexIndex = iVertexIndex
		NBVertexIndex = -9
		mbIsInstance = True
	End Sub
	Public ReadOnly Property IsInstance As Boolean
		Get
			Return mbIsInstance
		End Get
	End Property
	Public Sub DebugWrite(sCaption As String)
		If mbIsInstance Then
			DMAcadExt.AcadDocument.WriteMessage(sCaption & CStr(VertexIndex) & "<=>" & CStr(NBVertexIndex))
		Else
			DMAcadExt.AcadDocument.WriteMessage(sCaption & " Is Null ")
		End If

	End Sub
	Public Shared Operator =(tVertexPairA As tmVertexPair, tVertexPairB As tmVertexPair) As Boolean
		Return tVertexPairA.VertexIndex = tVertexPairB.VertexIndex
	End Operator
	Public Shared Operator <>(tVertexPairA As tmVertexPair, tVertexPairB As tmVertexPair) As Boolean
		Return Not tVertexPairA = tVertexPairB
	End Operator
End Structure
