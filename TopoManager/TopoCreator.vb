Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map
'Imports Autodesk.AutoCAD.ApplicationServices
'Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Gis.Map.Topology
'Imports Autodesk.AutoCAD.EditorInput
Imports DMAcadExt
Public Enum enTopoErrType
	RefOMark
	RefRMark
	RefSMark
	RefTMark
End Enum
 
Public Structure ActionVar
	Public Sub New(ByVal iActionID As Integer, ByVal dTolerance As Double)
		ActionID = iActionID
		Tolerance = dTolerance
	End Sub
	Dim ActionID As Integer
	Dim Tolerance As Double
End Structure
Public Structure TopoError
	Dim Exists As Boolean
	Dim AcObjID As ObjectId
	Dim Point As TPlnPoint
	Dim Type As enTopoErrType
	Dim Scale As Double
	Public Sub New(tAcObjID As ObjectId, tPoint As Autodesk.AutoCAD.Geometry.Point3d, iType As enTopoErrType, dScale As Double)
		AcObjID = tAcObjID
		Exists = True
		Point = New TPlnPoint(tPoint)
		Type = iType
		Scale = dScale
	End Sub
End Structure
Public NotInheritable Class TopoCreator

	Private Shared mdicLinkLayers As TopoElemDic = New TopoElemDic()
	Private Shared mdicCentroidBlocks As TopoElemDic = New TopoElemDic()
	Private Shared moTopoDefs As TopoDef
	Private Shared moCentroidCol As ObjectIdCollection = New ObjectIdCollection
   '	Private Shared moAcadEditorAAA As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
	Private Shared moaTopoDefs() As TopoDef
	Private Shared miTopoDefIndex As Integer = -1
	Private Shared moCurrentTopoModel As TopologyModel = Nothing
	Private Shared moOverlayTopoModel As TopologyModel = Nothing
	Public Shared TopoErrBlockNames() As String = New String() {AcadConst.TopoErrBlockRefOMark, AcadConst.TopoErrBlockRefRMark, AcadConst.TopoErrBlockRefSMark, AcadConst.TopoErrBlockRefTMark}
	Public Shared CleanupActionNames() As String = New String() {"Erase Short Objects" _
	, "Break Crossing Objects" _
	, "Extend Undershoots" _
	, "Delete Duplicates" _
	, "Snap Clustered Nodes" _
	, "Dissolve Pseudo Nodes" _
	, "Erase Dangling Objects" _
	, "Simplify Objects" _
	, "Zero Length Objects" _
	, "Apparent Intersection" _
	, "Weed Polylines" _
	, "Explode Polylines" _
	, "Convert Arcs" _
	, "Short Lines" _
	, "Rounding Points" _
	, "Merging Points" _
	, "Points Near Line" _
	, "Remove Duplicates" _
	, "Points & Lines" _
	, "Rounding Lines" _
	, "Rounding Points&Vert"}
	Public Shared CleanupActionIDs() As Integer = New Integer() {1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 524288, 524304, 524320, 1048576, 2097152, 4194304, 8388608, 16777216, 33554432, 67108864}

	''   Private Shared m_selEntIdCollection As ObjectIdCollection = New ObjectIdCollection()
	Private Shared moCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
	''' <summary>
	''' Creates a new Topology from users input of the name, description, type
	''' and selected entities in the drawing file.
	''' </summary>
	''' <param name="sName">  Input: name.</param>
	''' <param name="sDesc">  Input: description.</param>
	''' <param name="topologyType">  Input: type.</param>
	''' <param name="polygonCentroidCol">  Input: collection of Polygons Centroids' Object ID.</param>
	''' <param name="linkCol">   Input: collection of Links' Object ID.</param>
	''' <param name="nodeCol">   Input: collection of Nodes' Object ID.</param>
	Public Shared Sub CreateMapTopology(ByVal sName As System.String, ByVal sDesc As System.String, ByVal topologyType As TopologyTypes, _
				  ByVal polygonCentroidCol As ObjectIdCollection, ByVal linkCol As ObjectIdCollection, ByVal nodeCol As ObjectIdCollection)
		Dim topoModel As TopologyModel = Nothing
		Dim app As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim topos As Autodesk.Gis.Map.Topology.Topologies = app.ActiveProject.Topologies
		Try
			topos.Create(sName, linkCol, nodeCol, polygonCentroidCol, topologyType)

			topoModel = topos(sName)
			topoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
			topoModel.Description = sDesc
		Catch err As Autodesk.Gis.Map.MapException
			'' Utility.GetEditor.WriteMessage(String.Format(vbCrLf & "Exception throwed containing the error code: {0}", err.ErrorCode))
		End Try
		'	topoModel.DeletePolygon()
		topoModel.Close()
	End Sub
	Public Shared Sub CreateCurrentTopologyAAA()
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		Dim app As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = app.ActiveProject.Topologies
		Dim sName As System.String = moTopoDefs.Name
		Dim sDescription As System.String = moTopoDefs.Description

		Dim iTopologyType As TopologyTypes = TopologyTypes.Polygon
		Dim colLinks As ObjectIdCollection = zzGetAllLinks()
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = zzGetAllBlockRefs()

		System.Windows.Forms.MessageBox.Show(sName, "410_")
		System.Windows.Forms.MessageBox.Show(colLinks.Count.ToString(), "420")
		System.Windows.Forms.MessageBox.Show(colNodes.Count.ToString(), "430")
		System.Windows.Forms.MessageBox.Show(colCentroids.Count.ToString(), "440")


		Try
			oTopos.Create(sName, colLinks, colNodes, colCentroids, iTopologyType, CreateOptions.HighlightErrors, 1.0)
			System.Windows.Forms.MessageBox.Show("420")
			moCurrentTopoModel = oTopos(sName)
			System.Windows.Forms.MessageBox.Show("430")
			moCurrentTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
			moCurrentTopoModel.Description = sDescription
			'  System.Windows.Forms.MessageBox.Show("440")
		Catch oErr As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadDocument.WriteMessage(String.Format(vbCrLf & "Exception throwed containing the error code: {0}", oErr.ErrorCode))

		End Try
		moCurrentTopoModel.Close()
	End Sub

	Public Shared Sub CreateTopology(ByVal iAppID As Integer, ByVal oTopoDef As TopoDef, ByVal bHighlightSliver As Boolean, ByVal dTolerance As Double, ByVal bCheckTopo As Boolean)

		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim iTopologyType As TopologyTypes = TopologyTypes.Polygon
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim oTopoModel As TopologyModel
		Dim sTopoName As System.String = oTopoDef.Name
		Dim sDescription As System.String = oTopoDef.Description
		Dim bCurrentLayerOK As Boolean = True
		Dim colCentroids As ObjectIdCollection = Nothing
		Dim colCentroidsAdd As ObjectIdCollection = Nothing
		Dim sLayer As String = oTopoDef.CreateTopologyLayer
		Dim sMsg As String

		If oTopos.Exists(sTopoName) Then
			System.Windows.Forms.MessageBox.Show("Topology " & oTopoDef.Name & " already exists", "TopoCreator - CreateTopology", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
			Return
		End If

		If sLayer.Length <> 0 Then
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sLayer, iAppID, DMAcadExt.enLayerFunction.Default, True, True, False)
		ElseIf oTopoDef.CreateCentroid Then
			bCurrentLayerOK = True
		Else
			bCurrentLayerOK = False
		End If

		If bCurrentLayerOK Then
			DMAcadExt.AcadTransaction.ClearLayerByClassName(sLayer, AcadConst.AcadPointName)
			'System.Windows.Forms.MessageBox.Show("Layers: " & oTopoDef.TopoLayers & vbCrLf & "'" & oTopoDef.TopoStraightLayers & "'", "06_211", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
			Dim colLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(oTopoDef.TopoLayers, oTopoDef.TopoStraightLayers)	 ', oTopoDef.TopoStraightLayers
			'MessageBox.Show(oTopoDef.TopoLayers & vbCrLf & oTopoDef.TopoStraightLayers & vbCrLf & colLinks.Count, "03_333")
			Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
			Dim saBlocks() As String
			Try
				If oTopoDef.CentroidBlockExists Then
					saBlocks = oTopoDef.CentroidBlocks
					For iIndex As Integer = 0 To saBlocks.GetUpperBound(0)
						If iIndex = 0 Then
							colCentroids = DMAcadExt.AcadTransaction.GetBlockRefs(saBlocks(0), oTopoDef.CentroidLayers)
						Else
							colCentroidsAdd = DMAcadExt.AcadTransaction.GetBlockRefs(saBlocks(iIndex), oTopoDef.CentroidLayers)
							For Each tAcObjId As ObjectId In colCentroidsAdd
								colCentroids.Add(tAcObjId)
							Next
						End If
					Next
				Else
					colCentroids = New ObjectIdCollection()
				End If
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "C112")
			End Try

			sMsg = "***Topology '" & sTopoName & "' ***"
			'	sMsg &= vbCrLf & oTopoDef.TopoLayers & ":" & oTopoDef.TopoStraightLayers
			sMsg &= vbCrLf & "Links " & oTopoDef.LinkText & ": " & colLinks.Count.ToString()
			sMsg &= vbCrLf & "Nodes: " & colNodes.Count.ToString()
			sMsg &= vbCrLf & "Centroids " & oTopoDef.CentroidText & ": " & colCentroids.Count.ToString() & vbCrLf
			DMAcadExt.AcadDocument.WriteMessage(sMsg)
			Dim iCreateOptions As CreateOptions = CreateOptions.StopAtMultipleCentroid Or CreateOptions.UsePersistentMarkers	'CreateOptions.HighlightErrors Or

			Try
				Dim bTopologyCreated As Boolean = False

				If bHighlightSliver Then
					iCreateOptions = iCreateOptions Or CreateOptions.HighlightSliverPolygons
				End If
				If bCurrentLayerOK AndAlso colLinks.Count <> 0 Then
					'	MessageBox.Show(iCreateOptions.ToString() & ":" & CStr(dTolerance), "12_897")
					oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, iTopologyType, iCreateOptions, dTolerance)
					'	System.Windows.Forms.MessageBox.Show("", "42_841")
					bTopologyCreated = True
				End If

			Catch oEx As System.Exception
				If oEx.GetType().ToString() = "Autodesk.Gis.Map.MapException" Then
					Dim oMapEx As Autodesk.Gis.Map.MapException = DirectCast(oEx, Autodesk.Gis.Map.MapException)
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Create Topology")
				Else
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TopoCreator - CreateTopology_12")
				End If
				oTopoModel = Nothing
			End Try
			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Try
				oTopos = oMapApplication.ActiveProject.Topologies
				If oTopos.Exists(sTopoName) Then
					'	Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(String.Empty, 0, False, String.Empty)
					Dim oPointList As IList(Of TPlnPoint) = Nothing
					Dim colResLinks As ObjectIdCollection
					oTopoModel = oTopos(sTopoName)
					oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
					colResLinks = zzGetTopoLinks(colPolygons, bCheckTopo)
					Dim iPgonCount As Integer = colPolygons.Count
					Dim iLinkCount As Integer = colResLinks.Count
					Dim sPlExt As String
					If iPgonCount = 1 Then
						sPlExt = String.Empty
					Else
						sPlExt = "s"
					End If
					sMsg = "Topology '" & sTopoName & "' contains " & Convert.ToString(iPgonCount) & " polygon" & DMCommon.Functions.GetPlural(iPgonCount) & ", " & Convert.ToString(iLinkCount) & " link" & DMCommon.Functions.GetPlural(iLinkCount)
					'' Dim iPgonCount As Integer = oPoligons.Count
					If iLinkCount <> colLinks.Count Then
						MessageBox.Show(CStr(iLinkCount) & ":" & CStr(colLinks.Count), "09_120")

					End If
					If sLayer.Length <> 0 Then	  'oTopoDef.MissingCentroidLayer.Length <> 0 AndAlso (Not oTopoDef.CreateCentroid)
						Dim iPointCount As Integer = DMAcadExt.AcadTransaction.GetAcadPointsCount(sLayer, True)
						Dim sMsgLayer As String = String.Empty

						If iPointCount <> 0 Then
							If Not oTopoDef.CreateCentroid Then
								sMsgLayer = "; layer " & sLayer
							End If
							If Not (oTopoDef.CreateCentroid) OrElse oTopoDef.CentroidBlockExists Then
								oPointList = DMAcadExt.AcadTransaction.GetAcadPoints(sLayer, True)
								For iIndex As Integer = 0 To oPointList.Count - 1
									DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
								Next
							Else
								iPointCount = -1
							End If
						End If
						''  sMsg = "Topology Poligons: " & CStr(iPgonCount)
						If iPointCount >= 0 Then
							sMsg &= vbCrLf & "Count of Missing Centroids: " & CStr(iPointCount) & sMsgLayer ''& vbCrLf hhh
						End If
						'
					End If
					DMAcadExt.AcadDocument.WriteMessage(sMsg)
					'	oTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
					oTopoModel.Description = sDescription
					Dim tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
					'	Dim oaPolygons() As Polygon = Nothing
					'	colPoligons.CopyTo(oaPolygons, 0)
					Dim colTopoCentroids As ObjectIdCollection = New ObjectIdCollection()
					Dim iHasCentroidCounter As Integer = 0
					Dim oBlockRef As BlockReference
					If oTopoDef.CentroidBlockExists Then
						For Each oPolygon As Polygon In colPolygons
							tCentroidAcObjID = oPolygon.Entity
							colTopoCentroids.Add(tCentroidAcObjID)
						Next
						colPolygons.Dispose()
						colPolygons = Nothing
						oTopoModel.Close()
						'		oTopoModel.Dispose()
						oTopoModel = Nothing
						For Each tAcObjId As ObjectId In colCentroids
							If Not colTopoCentroids.Contains(tAcObjId) Then
								iHasCentroidCounter += 1
								oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
								If oBlockRef IsNot Nothing Then
									DMAcadExt.AppMessages.AddMessage(True, New TPlnPoint(oBlockRef.Position), "", "Centroid out of Topology", False)
								End If
							End If
						Next
						colTopoCentroids.Dispose()
						colTopoCentroids = Nothing

						sMsg = "Count of Centroids out of Topology: " & Convert.ToString(iHasCentroidCounter)
						DMAcadExt.AcadDocument.WriteMessage(sMsg)
						Dim sBlockPath As String = Nothing
						Dim oaAttribDefs() As AttributeDefinition = Nothing
						Dim tBlockAcObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(sBlockPath, oTopoDef.CentroidBlock, oaAttribDefs)
						If Not tBlockAcObjId.IsNull AndAlso oPointList IsNot Nothing AndAlso oPointList.Count > 0 Then

							If oTopoDef.CreateCentroid Then
								Dim colAddCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockAcObjId, oPointList, oaAttribDefs, , oTopoDef.CentroidScale)
								sMsg = "Count of inserted Centroids: " & Convert.ToString(colAddCentroids.Count)
								DMAcadExt.AcadDocument.WriteMessage(sMsg)
								For Each tAcObjID As ObjectId In colAddCentroids
									colCentroids.Add(tAcObjID)
								Next
								If False Then
									oTopos.Delete(sTopoName, False)
									DMAcadExt.AcadTransaction.EraseDBPoints(oPointList)
									oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, iTopologyType, iCreateOptions, dTolerance)
								End If
							
							End If
						End If
					End If
				End If
				oTopoModel = Nothing
				''   zzCommandLine()
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - CreateTopology_10")
				oTopoModel = Nothing
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Layer '" & sLayer & "' was not found", "TopoCreator - CreateTopology_19a")
		End If
   End Sub

   Public Shared Function CreateTopology(ByVal sTopoName As String, iMapThemeID As DMAcadExt.enMapTheme, ByVal sLinkLayers As String, sLineLayers As String, ByVal sCentroidBlocks As String, ByVal sCentroidLayers As String, bCreateCentroids As Boolean, ByVal sNodeBlocks As String, ByVal sNodeLayers As String, bCreateNodes As Boolean, bHighLightSlivers As Boolean, ByVal dTolerance As Double) As DMAcadExt.TopoRes
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim sMsg As String
		Dim tTopoRes As TopoRes
		'	DMCommon.Debug.MsgBox("12_290d", sTopoName, iMapThemeID)
		'Main
		'	DMCommon.Debug.MsgBox("12_690", "CreateTopology 1")
		If oTopos.Exists(sTopoName) Then
         System.Windows.Forms.MessageBox.Show("Topology '" & sTopoName & "' already exists", "TopoCreator - CreateTopology", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
         tTopoRes.TopoExists = True
      Else

         Dim colLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(sLinkLayers, sLineLayers)
         '	MessageBox.Show(sLinkLayers & vbCrLf & sLineLayers & vbCrLf & colLinks.Count, "03_399")
         Dim colNodes As ObjectIdCollection '= New ObjectIdCollection()
         Dim colCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(sCentroidBlocks, sCentroidLayers)
			'  DMCommon.Debug.MsgBox("03_404a", sCentroidBlocks, sCentroidLayers, colCentroids.Count, sNodeBlocks, sNodeLayers)
			'     DMCommon.Debug.MsgBox("03_405", True, sNodeBlocks, sNodeLayers)
			If String.IsNullOrEmpty(sNodeBlocks) Then
				colNodes = New ObjectIdCollection()
			Else
				colNodes = DMAcadExt.AcadTransaction.GetBlockRefsNew(sNodeBlocks, sNodeLayers)
         End If
			'	DMCommon.Debug.MsgBox("03_407", sNodeBlocks, sNodeLayers, colNodes.Count, bCreateNodes)

			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
         Dim iCreateOptions As CreateOptions = CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors
         '	Dim sPrevLayer As String
         If bHighLightSlivers Then
            iCreateOptions = iCreateOptions Or CreateOptions.HighlightSliverPolygons
         End If
         Dim bTopologyCreated As Boolean = False

         '	MessageBox.Show(iCreateOptions.ToString() & ":" & CStr(dTolerance), "12_897")
         If colLinks.Count = 0 Then
            DMAcadExt.AcadDocument.WriteMessageLog("Link was not found")
            tTopoRes = New TopoRes(False)
         Else
            Dim sCurrentLayer As String
            Dim bCurrentLayerOK As Boolean
            Dim oTopoModel As TopologyModel
            Dim bRecreate As Boolean = False
            'sPrevLayer = DMAcadExt.AcadTransaction.GetCurrentLayer()
            If bCreateCentroids Then
               sCurrentLayer = DMCommon.Functions.GetFirstStrFromList(sCentroidLayers)
            Else
               sCurrentLayer = DMAcadExt.MapThemeData.MissingCentroidLayer
            End If
            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sCurrentLayer, DMApp.AppID, True, True)
            If bCurrentLayerOK Then
               If Not bCreateCentroids Then
                  DMAcadExt.AcadTransaction.ClearLayerByClassName(sCurrentLayer, AcadConst.AcadPointName)
               End If
               '  DMCommon.Debug.MsgBox("12_291", sTopoName, colLinks.Count, colCentroids.Count)
               Try
                  oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
               Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Create Topology")
					End Try
               'Dim oPriorView As ViewTableRecord
               'oPriorView = DMAcadExt.AcadDocument.GetCurrentView()
               'DMAcadExt.AcadDocument.WriteMessage("!!997 " & CStr(oPriorView.Width) & ":" & CStr(oPriorView.Height))
               '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
               Try
                  oTopos = oMapApplication.ActiveProject.Topologies
                  If oTopos.Exists(sTopoName) Then
                     tTopoRes = New TopoRes(True)
                     '	Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(String.Empty, 0, False, String.Empty)
                     Dim oPointList As IList(Of TPlnPoint) = Nothing
                     Dim colResLinks As ObjectIdCollection
                     Dim bCentroidBlockExists As Boolean = Not String.IsNullOrEmpty(sCentroidBlocks)
                     Dim bNodeBlockExists As Boolean = Not String.IsNullOrEmpty(sNodeBlocks)

                     oTopoModel = oTopos(sTopoName)
                     oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
                     tTopoRes.IsCorrect = True
                     tTopoRes.IsComplete = oTopoModel.IsComplete
                     Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
                     colResLinks = zzGetTopoLinks(colPolygons, False)
                     Dim iPgonCount As Integer = colPolygons.Count
                     tTopoRes.PgonCount = iPgonCount
                     Dim iLinkCount As Integer = colResLinks.Count
                     tTopoRes.LinkCount = iLinkCount
                     If Not bCreateCentroids Then
                        tTopoRes.CentroidCount = colCentroids.Count
                     End If
							tTopoRes.NodeCount = oTopoModel.GetNodes().Count
							tTopoRes.HasElements = True
							Dim sPlExt As String
                     If iPgonCount = 1 Then
                        sPlExt = String.Empty
                     Else
                        sPlExt = "s"
                     End If
                     sMsg = "Topology '" & sTopoName & "' contains " & Convert.ToString(iPgonCount) & " polygon" & DMCommon.Functions.GetPlural(iPgonCount) & ", " & Convert.ToString(iLinkCount) & " link" & DMCommon.Functions.GetPlural(iLinkCount)
							'' Dim iPgonCount As Integer = oPoligons.Count

							If Not String.IsNullOrEmpty(sCurrentLayer) Then     'oTopoDef.MissingCentroidLayer.Length <> 0 AndAlso (Not oTopoDef.CreateCentroid)
								Dim iPointCount As Integer = DMAcadExt.AcadTransaction.GetAcadPointsCount(sCurrentLayer, True)
								Dim sMsgLayer As String = String.Empty
								DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 1)
								If iPointCount <> 0 Then
									If Not bCreateCentroids Then
										sMsgLayer = "; layer " & sCurrentLayer
									End If

									'   DMCommon.Debug.MsgBox("12_654E", Not (bCreateCentroids AndAlso bCentroidBlockExists), sCentroidBlocks, bCreateCentroids, bCentroidBlockExists)
									oPointList = DMAcadExt.AcadTransaction.GetAcadPoints(sCurrentLayer, True)

									If Not (bCreateCentroids AndAlso bCentroidBlockExists) Then
										tTopoRes.MissingCntrCount = iPointCount
										For iIndex As Integer = 0 To oPointList.Count - 1
											DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
											DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex).X, oPointList.Item(iIndex).Y, "", DMCommon.dmMessages.Message(306), False, iMapThemeID, 1)
										Next

									Else
										iPointCount = -1
									End If
								End If
								''  sMsg = "Topology Poligons: " & CStr(iPgonCount)
								If iPointCount >= 0 Then
									sMsg &= vbCrLf & "Count of Missing Centroids: " & CStr(iPointCount) & sMsgLayer ''& vbCrLf hhh
								End If
								'
							End If
							DMAcadExt.AcadDocument.WriteMessage(sMsg)
                     '	oTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
                     Dim tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
                     '	Dim oaPolygons() As Polygon = Nothing
                     '	colPoligons.CopyTo(oaPolygons, 0)
                     Dim colTopoCentroids As ObjectIdCollection = New ObjectIdCollection()
                     Dim iOutsideCentroidCounter As Integer = 0
                     Dim oBlockRef As BlockReference
                     If bCentroidBlockExists Then
                        DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 2)
                        For Each oPolygon As Polygon In colPolygons
                           tCentroidAcObjID = oPolygon.Entity
                           colTopoCentroids.Add(tCentroidAcObjID)
                        Next
                        '''''''''''''''''''''''''''''  colPolygons.Dispose()
                        '''''''''''''''''  colPolygons = Nothing
                        '  oTopoModel.Close()
                        '		oTopoModel.Dispose()
                        ' oTopoModel = Nothing
                        For Each tAcObjId As ObjectId In colCentroids
                           If Not colTopoCentroids.Contains(tAcObjId) Then
                              iOutsideCentroidCounter += 1
                              oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                              If oBlockRef IsNot Nothing Then
											'  DMCommon.Debug.MsgBox("12_860b", DMAcadExt.AppMessages.RecordCount, iMapThemeID, 2)

											DMAcadExt.AppMessages.AddMessage(True, New TPlnPoint(oBlockRef.Position), "", DMCommon.dmMessages.Message(307), False, iMapThemeID, 2)

                                 '  DMCommon.Debug.MsgBox("12_860a", DMAcadExt.AppMessages.RecordCount)
                              End If
                           End If
                        Next
                        ''''''''''''''''''''''''''''''''''''  colTopoCentroids.Dispose()
                        colTopoCentroids = Nothing
                        sMsg = "Count of Centroids out of Topology: " & Convert.ToString(iOutsideCentroidCounter)
                        DMAcadExt.AcadDocument.WriteMessage(sMsg)
                        tTopoRes.OutsideCntrCount = iOutsideCentroidCounter
                        Dim sBlockPath As String = Nothing
                        Dim sBlockName As String = DMCommon.Functions.GetFirstStrFromList(sCentroidBlocks)
                        Dim oaAttribDefs() As AttributeDefinition = Nothing
                        Dim tBlockAcObjId As ObjectId
                        Dim dScaleFactor As Double
                        If oPointList IsNot Nothing AndAlso oPointList.Count > 0 Then
                           If bCreateCentroids Then
                              tBlockAcObjId = DMAcadExt.AcadTransaction.OpenBlockDB(sBlockPath, sBlockName, oaAttribDefs)
                              bRecreate = True
                              dScaleFactor = DMAcadExt.AcadDocument.GetDWGScaleFactor()
                           Else
                              tBlockAcObjId = zzGetRhombusBlock()
                              dScaleFactor = DMAcadExt.AcadDocument.GetTopoErrBlockScale()
                           End If
                           If Not tBlockAcObjId.IsNull Then
                              Dim colAddCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockAcObjId, oPointList, oaAttribDefs, Nothing, dScaleFactor)  'oaAttribDefs
                              tTopoRes.InsertedCntrCount = colAddCentroids.Count
                              If bCreateCentroids Then
                                 sMsg = "Count of inserted Centroids: " & Convert.ToString(colAddCentroids.Count)
                                 DMAcadExt.AcadDocument.WriteMessage(sMsg)
                              End If

                              For Each tAcObjID As ObjectId In colAddCentroids
                                 colCentroids.Add(tAcObjID)
                              Next
                           End If
                        End If
                     End If 'bCentroidBlockExists
                     If bNodeBlockExists Then
                        Dim oNodeBlock As AcadBlock = New AcadBlock(DMCommon.Functions.GetFirstStrFromList(sNodeBlocks))
                        Dim tBlockRefData As BlockRefData
                        Dim tNodeAcObjId As ObjectId
                        sCurrentLayer = DMCommon.Functions.GetFirstStrFromList(sNodeLayers)

                        oNodeBlock.OpenForRight()
                        '  Dim oNode As Node

                        Dim colTopoNodes As NodeCollection = Nothing
                        Try
                           colTopoNodes = oTopoModel.GetNodes()
                        Catch oMapEx As Autodesk.Gis.Map.MapException
                           DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - CreateTopology_12")
                           colTopoNodes = Nothing
                        End Try
                        '  DMCommon.Debug.MsgBox("08_555", sCurrentLayer, colTopoNodes, colTopoNodes.Count)
                        If colTopoNodes IsNot Nothing Then
                           bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sCurrentLayer, DMApp.AppID, True, True)
                           If bCurrentLayerOK Then
                              For Each oNode As Node In colTopoNodes
											If bCreateNodes AndAlso Not zzNodeHasEntity(oNode) Then
												tBlockRefData = New BlockRefData(oNode.Location)
												tNodeAcObjId = oNodeBlock.InsertRef(tBlockRefData)
												colNodes.Add(tNodeAcObjId)
												bRecreate = True
											End If
										Next
                           End If

                           '  DMCommon.Debug.MsgBox("08_549", colTopoNodes.Count)
                        End If
                     End If
                     If oTopoModel IsNot Nothing Then
                        oTopoModel.Close()

                        oTopoModel = Nothing
                     End If

                     If bRecreate Then
                        oTopos.Delete(sTopoName, False)
                        If oPointList IsNot Nothing Then
                           DMAcadExt.AcadTransaction.EraseDBPoints(oPointList)
                        End If
                        '  DMCommon.Debug.MsgBox("08_571", sTopoName, colLinks.Count, colNodes.Count, colCentroids.Count)
                        Try
                           oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
                           If bCreateCentroids Then
                              tTopoRes.CentroidCount = colCentroids.Count
                           End If
                        Catch oMapEx As Autodesk.Gis.Map.MapException
                           DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - CreateTopology_10")
                           DMCommon.Debug.MsgBox("08_549", sTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
                        End Try

                     End If
                  End If ' Topo Exists

                  ''   zzCommandLine()
               Catch oEx As Exception
                  MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString() & vbCrLf & oEx.StackTrace, "19_001")
                  oTopoModel = Nothing
               End Try
               '	DMAcadExt.AcadTransaction.SetCurrentLayer(sPrevLayer, DMApp.AppID, False, False)
            End If
         End If

         '	System.Windows.Forms.MessageBox.Show("", "42_841")
         bTopologyCreated = True

      End If

      Return tTopoRes
   End Function

	Public Shared Function CreateTopology(ByVal sTopoName As String, colLinks As ObjectIdCollection, ByVal sCentroidBlocks As String, ByVal sCentroidLayers As String, bCreateCentroids As Boolean, bHighLightSlivers As Boolean, ByVal dTolerance As Double) As DMAcadExt.TopoRes
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sMsg As String
		Dim tTopoRes As TopoRes
		'	DMCommon.Debug.MsgBox("12_691", "CreateTopology   2")
		If oTopos.Exists(sTopoName) Then
			System.Windows.Forms.MessageBox.Show("Topology '" & sTopoName & "' already exists", "TopoCreator - CreateTopology", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
			tTopoRes.TopoExists = True
		Else
			'	MessageBox.Show(sLinkLayers & vbCrLf & sLineLayers & vbCrLf & colLinks.Count, "03_399")
			Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
			Dim colCentroids As ObjectIdCollection
			If String.IsNullOrEmpty(sCentroidBlocks) Then
				colCentroids = New ObjectIdCollection()
			Else
				colCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(sCentroidBlocks, sCentroidLayers)
			End If


			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			Dim iCreateOptions As CreateOptions = CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors
			'	Dim sPrevLayer As String
			If bHighLightSlivers Then
				iCreateOptions = iCreateOptions Or CreateOptions.HighlightSliverPolygons
			End If
			Dim bTopologyCreated As Boolean = False

			'	MessageBox.Show(iCreateOptions.ToString() & ":" & CStr(dTolerance), "12_897")
			If colLinks.Count = 0 Then
				DMAcadExt.AcadDocument.WriteMessageLog("Link was not found")
				tTopoRes = New TopoRes(False)
			Else
				Dim sCurrentLayer As String
				Dim bCurrentLayerOK As Boolean
				Dim oTopoModel As TopologyModel
				'sPrevLayer = DMAcadExt.AcadTransaction.GetCurrentLayer()
				If bCreateCentroids Then
					sCurrentLayer = DMCommon.Functions.GetFirstStrFromList(sCentroidLayers)
				Else
					sCurrentLayer = DMAcadExt.MapThemeData.MissingCentroidLayer
				End If
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sCurrentLayer, DMApp.AppID, True, True)
				If bCurrentLayerOK Then
					If Not bCreateCentroids Then
						DMAcadExt.AcadTransaction.ClearLayerByClassName(sCurrentLayer, AcadConst.AcadPointName)
					End If

					Try
						oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Create Topology")
					End Try
					'Dim oPriorView As ViewTableRecord
					'oPriorView = DMAcadExt.AcadDocument.GetCurrentView()
					'DMAcadExt.AcadDocument.WriteMessage("!!997 " & CStr(oPriorView.Width) & ":" & CStr(oPriorView.Height))
					'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
					Try
						oTopos = oMapApplication.ActiveProject.Topologies
						If oTopos.Exists(sTopoName) Then
							tTopoRes = New TopoRes(True)
							'	Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(String.Empty, 0, False, String.Empty)
							Dim oPointList As IList(Of TPlnPoint) = Nothing
							Dim colResLinks As ObjectIdCollection
							Dim bCentroidBlockExists As Boolean = Not String.IsNullOrEmpty(sCentroidBlocks)
							oTopoModel = oTopos(sTopoName)
							oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
							tTopoRes.IsCorrect = True
							tTopoRes.IsComplete = oTopoModel.IsComplete
							Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
							colResLinks = zzGetTopoLinks(colPolygons, False)
							Dim iPgonCount As Integer = colPolygons.Count
							tTopoRes.PgonCount = iPgonCount
							Dim iLinkCount As Integer = colResLinks.Count
							tTopoRes.LinkCount = iLinkCount
							tTopoRes.CentroidCount = colCentroids.Count
							tTopoRes.HasElements = True
							Dim sPlExt As String
							If iPgonCount = 1 Then
								sPlExt = String.Empty
							Else
								sPlExt = "s"
							End If
							sMsg = "Topology '" & sTopoName & "' contains " & Convert.ToString(iPgonCount) & " polygon" & DMCommon.Functions.GetPlural(iPgonCount) & ", " & Convert.ToString(iLinkCount) & " link" & DMCommon.Functions.GetPlural(iLinkCount)
							'' Dim iPgonCount As Integer = oPoligons.Count

							If sCurrentLayer.Length <> 0 Then     'oTopoDef.MissingCentroidLayer.Length <> 0 AndAlso (Not oTopoDef.CreateCentroid)
								Dim iPointCount As Integer = DMAcadExt.AcadTransaction.GetAcadPointsCount(sCurrentLayer, True)
								Dim sMsgLayer As String = String.Empty
								If iPointCount <> 0 Then
									If Not bCreateCentroids Then
										sMsgLayer = "; layer " & sCurrentLayer
									End If
									tTopoRes.MissingCntrCount = iPointCount

									DMCommon.Debug.MsgBox("12_654A", sCentroidBlocks, bCreateCentroids, bCentroidBlockExists)
									If Not (bCreateCentroids) OrElse bCentroidBlockExists Then
										oPointList = DMAcadExt.AcadTransaction.GetAcadPoints(sCurrentLayer, True)
										For iIndex As Integer = 0 To oPointList.Count - 1
											DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
										Next
									Else
										iPointCount = -1
									End If
								End If
								''  sMsg = "Topology Poligons: " & CStr(iPgonCount)
								If iPointCount >= 0 Then
									sMsg &= vbCrLf & "Count of Missing Centroids: " & CStr(iPointCount) & sMsgLayer ''& vbCrLf hhh
								End If
								'
							End If
							DMAcadExt.AcadDocument.WriteMessage(sMsg)
							'	oTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)

							Dim tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
							'	Dim oaPolygons() As Polygon = Nothing
							'	colPoligons.CopyTo(oaPolygons, 0)
							Dim colTopoCentroids As ObjectIdCollection = New ObjectIdCollection()
							Dim iHasCentroidCounter As Integer = 0
							Dim oBlockRef As BlockReference
							If bCentroidBlockExists Then
								For Each oPolygon As Polygon In colPolygons
									tCentroidAcObjID = oPolygon.Entity
									colTopoCentroids.Add(tCentroidAcObjID)
								Next
								colPolygons.Dispose()
								colPolygons = Nothing
								oTopoModel.Close()
								'		oTopoModel.Dispose()
								oTopoModel = Nothing
								For Each tAcObjId As ObjectId In colCentroids
									If Not colTopoCentroids.Contains(tAcObjId) Then
										iHasCentroidCounter += 1
										oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
										If oBlockRef IsNot Nothing Then
											DMAcadExt.AppMessages.AddMessage(True, New TPlnPoint(oBlockRef.Position), "", "Centroid out of Topology", False, 0, 2)
										End If
									End If
								Next
								colTopoCentroids.Dispose()
								colTopoCentroids = Nothing

								sMsg = "Count of Centroids out of Topology: " & Convert.ToString(iHasCentroidCounter)
								DMAcadExt.AcadDocument.WriteMessage(sMsg)
								Dim bRecreate As Boolean = False
								Dim sBlockPath As String = Nothing
								Dim sBlockName As String = DMCommon.Functions.GetFirstStrFromList(sCentroidBlocks)
								Dim oaAttribDefs() As AttributeDefinition = Nothing
								Dim tBlockAcObjId As ObjectId
								Dim dScale As Double
								If oPointList IsNot Nothing AndAlso oPointList.Count > 0 Then

									If bCreateCentroids Then
										tBlockAcObjId = DMAcadExt.AcadTransaction.OpenBlockDB(sBlockPath, sBlockName, oaAttribDefs)
										bRecreate = True
										dScale = 1.0
									Else
										tBlockAcObjId = zzGetRhombusBlock()
										dScale = DMAcadExt.AcadDocument.GetTopoErrBlockScale()
									End If
									If Not tBlockAcObjId.IsNull Then
										Dim colAddCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockAcObjId, oPointList, oaAttribDefs, Nothing, dScale)  'oaAttribDefs
										tTopoRes.InsertedCntrCount = colAddCentroids.Count
										If bCreateCentroids Then
											sMsg = "Count of inserted Centroids: " & Convert.ToString(colAddCentroids.Count)
											DMAcadExt.AcadDocument.WriteMessage(sMsg)
										End If


										For Each tAcObjID As ObjectId In colAddCentroids
											colCentroids.Add(tAcObjID)
										Next
										If bRecreate Then
											oTopos.Delete(sTopoName, False)
											DMAcadExt.AcadTransaction.EraseDBPoints(oPointList)
											oTopos.Create(sTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
										End If
									End If
								End If
							End If
						End If
						oTopoModel = Nothing
						''   zzCommandLine()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - CreateTopology_10")
						oTopoModel = Nothing
					End Try
					'	DMAcadExt.AcadTransaction.SetCurrentLayer(sPrevLayer, DMApp.AppID, False, False)
				End If
			End If

			'	System.Windows.Forms.MessageBox.Show("", "42_841")
			bTopologyCreated = True
		End If
		Return tTopoRes
	End Function
	Private Shared Function zzGetList() As ObjectIdCollection
		Dim iaAcObjId() As Long = {1906419085584, 1906419093680, 1906419093696, 1906419093744, 1906419093776, 1906419093792, 1906419093824, 1906419093856, 1906419093904, 1906419093952, 1906419093984, 1906419094000, 1906419303568, 1906419303744, 1906419303840, 1906419304016, 1906419405776, 1906419405840, 1906419406384, 1906419406592, 1906419406608, 1906419406624, 1906419406640, 1906419406656, 1906419396672, 1906419405584, 1906419405472, 1906419405360, 1906419405248, 1906419405136, 1906419405024, 1906419404864, 1906419394384}
		Dim colRes As ObjectIdCollection = New ObjectIdCollection()
		Dim tIntPtr As IntPtr
		For i As Integer = 0 To iaAcObjId.GetUpperBound(0)
			tIntPtr = New IntPtr(iaAcObjId(i))
			colRes.Add(New ObjectId(tIntPtr))
		Next
		Return colRes
	End Function

	Private Shared Function zzGetHandleList() As ObjectIdCollection
		Dim iaHandleVal() As Long = {38297, 45907, 45908, 45911, 45913, 45914, 45916, 45918, 45921, 45924, 45926, 45927, 72273, 72380, 72394, 72413, 93069, 93089, 93515, 94928, 94929, 94930, 94931, 94932, 94940, 93041, 93034, 93027, 93020, 93013, 93006, 92996, 92989}
		Dim tHandle As Handle
		Dim oEntity As Entity
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		DMAcadExt.AcadTransaction.OpenHandleDictionary()
		Dim colRes As ObjectIdCollection = New ObjectIdCollection()
		For i As Integer = 0 To iaHandleVal.GetUpperBound(0)
			tHandle = New Handle(iaHandleVal(i))
			oEntity = DMAcadExt.AcadTransaction.GetEntity(tHandle, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			colRes.Add(oEntity.ObjectId)
		Next
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		Return colRes
	End Function
	Public Shared Sub CopyTopology(ByVal sSourceName As String, ByVal sDestName As String)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

		Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		Dim colEmpty As ObjectIdCollection = New ObjectIdCollection()



		Dim colFullEdgeCollection As FullEdgeCollection
		Dim colPolygons As PolygonCollection


		If Not String.IsNullOrEmpty(sSourceName) AndAlso oTopos IsNot Nothing AndAlso oTopos.Exists(sSourceName) Then



			''''''''''''''''''''''''''	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			'''''''''''''''''''''''''	DMAcadExt.AcadTransaction.Start()

			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = oTopos.Item(sSourceName)
			oSourceTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

			Dim iTopologyType As TopologyTypes = oSourceTopology.Type
			colFullEdgeCollection = oSourceTopology.GetFullEdges()
			For Each oFullEdge As FullEdge In colFullEdgeCollection
				colLinks.Add(oFullEdge.Entity)
			Next
			colPolygons = oSourceTopology.GetPolygons()
			For Each oPolygon As Polygon In colPolygons
				colCentroids.Add(oPolygon.Entity)
			Next
			oSourceTopology.Close()


			''''''''''''''''''	DMAcadExt.AcadTransaction.Terminate()
			'''''''''''''''''''''''	DMAcadExt.AcadDocument.Unlock()



			''''''''''''''''''	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			''''''''''''''''''''''	DMAcadExt.AcadTransaction.Start()

			'DMAcadExt.AcadTransaction.SetLayer(colEntityIds, "zz1")
			If Not String.IsNullOrEmpty(sDestName) AndAlso Not oTopos.Exists(sDestName) Then
				Try
					oTopos.Create(sDestName, colLinks, colEmpty, colCentroids, TopologyTypes.Polygon)
				Catch oMapEx As Autodesk.Gis.Map.MapException

					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "TopoCreator - CopyTopology")
				End Try
			End If
			'''''''''''''''''''	DMAcadExt.AcadTransaction.Terminate()
			'''''''''''''''''''''	DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Public Shared Sub CopyTopologyA(ByVal sSourceName As String, ByVal sDestName As String)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim colEntityIds As ObjectIdCollection


		If Not String.IsNullOrEmpty(sSourceName) AndAlso oTopos IsNot Nothing AndAlso oTopos.Exists(sSourceName) Then
			If False Then


				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
				DMAcadExt.AcadTransaction.Start()
				Dim oDBObject As DBObject
				Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = oTopos.Item(sSourceName)
				oSourceTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

				Dim iTopologyType As TopologyTypes = oSourceTopology.Type
				colEntityIds = oSourceTopology.GetEntityIds()
				oSourceTopology.Close()
				Dim i As Integer

				DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntityIds", "---", "---", "---", "---", "---", "---", "---")
				For Each tAcObjID As ObjectId In colEntityIds
					oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					DMCommon.Debug.ExcelLog.SetValue(i, "", oDBObject.Handle.Value)
					i += 1
				Next


				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()

				DMCommon.Debug.ExcelLog.MoveRow(1)
				'	DMCommon.Debug.MsgBox("12_299", sDestName, colEntityIds.Count, TopologyTypes.Polygon)
				'	End If
			End If
			colEntityIds = zzGetHandleList()

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			'DMAcadExt.AcadTransaction.SetLayer(colEntityIds, "zz1")
			If Not String.IsNullOrEmpty(sDestName) AndAlso Not oTopos.Exists(sDestName) Then
				Try
					oTopos.Create(sDestName, colEntityIds, TopologyTypes.Polygon)
				Catch oMapEx As Autodesk.Gis.Map.MapException

					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "TopoCreator - CopyTopology")
				End Try
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Public Shared Sub CreateTopology(ByVal sName As String, ByVal sLayers As String, ByVal dTolerance As Double)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		Dim colLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(sLayers)
		Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim iCreateOptions As CreateOptions = CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors
		Dim bTopologyCreated As Boolean = False

		''''MessageBox.Show(iCreateOptions.ToString() & ":" & CStr(dTolerance), "12_897")
		If colLinks.Count = 0 Then
			DMAcadExt.AcadDocument.WriteMessageLog("Link was not found")
		Else
			Try
				oTopos.Create(sName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, iCreateOptions, dTolerance)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Create Topology")
			End Try
		End If
		bTopologyCreated = True
	End Sub

	Public Shared Function GetTopoErrors(ByVal iTopoErrType As enTopoErrType) As TopoErrorArray
		Dim sBlockName As String = zzGetTopoErrBlockName(iTopoErrType)
		Dim colBlockRefObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs(sBlockName, String.Empty)
		Dim iIndex As Integer = 0

		If colBlockRefObjIDs IsNot Nothing Then
			'	MessageBox.Show(sBlockName & vbCrLf & CStr(colBlockRefObjIDs.Count), "02_674")
			Dim oResTopoErrorArray As TopoErrorArray = New TopoErrorArray(colBlockRefObjIDs.Count - 1)
			Dim oBlockRef As BlockReference
			Dim tTopoError As TopoError
			For Each tAcObjID As ObjectId In colBlockRefObjIDs
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				If oBlockRef IsNot Nothing Then
					tTopoError = New TopoError(tAcObjID, oBlockRef.Position, iTopoErrType, oBlockRef.ScaleFactors.X)
					oResTopoErrorArray.Add(tTopoError, iIndex)
				Else
					AcadDocument.WriteMessage("!!!oBlockRef Is Nothing")
				End If
				iIndex += 1
			Next
			Return oResTopoErrorArray
		Else
			Return Nothing
		End If

	End Function
	Public Shared Function GetAllTopoErrors() As TopoErrorArray

		Dim iTopoErrType As enTopoErrType
		Dim oAddTopoErrorArray As TopoErrorArray
		Dim oaTopoErrorArray As TopoErrorArray = New TopoErrorArray()

		For iIndex As Integer = 0 To TopoErrBlockNames.GetUpperBound(0)
			iTopoErrType = CType(iIndex, enTopoErrType)
			oAddTopoErrorArray = GetTopoErrors(iTopoErrType)
			If oAddTopoErrorArray IsNot Nothing Then
				oaTopoErrorArray.Add(oAddTopoErrorArray)
			End If
		Next
		Return oaTopoErrorArray
	End Function
	Public Shared Sub DeleteTopology(ByVal oTopoDef As TopoDef, ByVal bDeleteEntities As Boolean)
		'zzRemoveODTable(oResTopoDef.ODTableName)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application

		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sName As System.String = oTopoDef.Name
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing

		If oTopos.Exists(sName) Then
			'DMAcadExt.AcadTransaction.Start()
			Try
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
				oTopos.Delete(sName, False)
				If oTopoDef.ODTableName.Length <> 0 Then
					zzRemoveODTable(oTopoDef.ODTableName)
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - DeleteTopology!" & ": " & sName)
			Finally
				DMAcadExt.AcadDocument.Unlock()
			End Try
			'DMAcadExt.AcadTransaction.Terminate()
		End If


		''  oDocLock.Dispose()
		' zzCommandLine()
	End Sub
	Public Shared Sub ShowTopology(ByVal oTopoDef As TopoDef)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sName As System.String = oTopoDef.Name
		'	Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
		Try
			Dim oTopoModel As TopologyModel
			If oTopos.Exists(sName) Then
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
				oTopoModel = oTopos.Item(sName)
				'	oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "", "", True)
				oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				Try
					oTopoModel.ShowGeometry(1)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - ShowTopology_1")
				End Try
				DMAcadExt.AcadDocument.Unlock()

				oTopoModel.Close()
				zzCommandLine()
			End If
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - ShowTopology")
		End Try
   End Sub
   Public Shared Sub ShowTopology(ByVal sTopoName As String)
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

		Try
         Dim oTopoModel As TopologyModel
         If oTopos.Exists(sTopoName) Then
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
            oTopoModel = oTopos.Item(sTopoName)
            '	oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "", "", True)
            oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
            Try
               oTopoModel.ShowGeometry(2)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - ShowTopology_1")
            End Try


            oTopoModel.Close()
            DMAcadExt.AcadDocument.Unlock()
            zzCommandLine()
         End If
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - ShowTopology")
      End Try
   End Sub
	Public Shared Function TopologyExists(ByVal oTopoDef As TopoDef) As Boolean

		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sName As System.String = oTopoDef.Name
		Dim bOut As Boolean
		Try
			bOut = oTopos.Exists(sName)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			bOut = False
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - TopologyExists")
		End Try

		Return bOut
	End Function
   Public Shared Function TopologyExists(ByVal sTopoName As String, i As Integer) As Boolean
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim bOut As Boolean
      Try
         bOut = oTopos.Exists(sTopoName)
      Catch oMapEx As Autodesk.Gis.Map.MapException
         bOut = False
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - TopologyExists_1")
      End Try

      Return bOut
   End Function
   Public Shared Function TopologyExists(ByVal sTopoName As String) As Boolean
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim bOut As Boolean
      Try
         bOut = oTopos.Exists(sTopoName)
      Catch oMapEx As Autodesk.Gis.Map.MapException
         bOut = False
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - TopologyExists_1")
      End Try

      Return bOut
   End Function
	Public Shared Sub ShowTopologyStatus(ByVal oTopoDef As TopoDef)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sName As System.String = oTopoDef.Name
		Dim sMsg As String
		Dim oTopoModel As TopologyModel
		Try
			If oTopos.Exists(sName) Then
				oTopoModel = oTopos.Item(sName)
				oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				If oTopoModel.IsComplete Then
					sMsg = "Is Complete"
				Else
					sMsg = "Is Not Complete"
				End If
				oTopoModel.Close()
				sMsg &= vbCrLf & (oTopoModel.Status).ToString()

			Else
				sMsg = "Not Exists"
			End If
			System.Windows.Forms.MessageBox.Show(sMsg, "Topology " & oTopoDef.Name)
		Catch oMapEx As Autodesk.Gis.Map.MapException

			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - TopologyExists")
		End Try

	End Sub
	Public Shared Sub CleanupGet()
		Dim oTopologyClean As TopologyClean = New TopologyClean()

		Dim colLinks As ObjectIdCollection = zzGetLinksByLayerTest("LayerA")
		Dim oVar As Variable = New Variable()
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		System.Windows.Forms.MessageBox.Show(Convert.ToString(colLinks.Count), "colLinks.Count-C105")
		oVar.LoadProfile("C:\NetProjects8\Examples\DotNet\TopologyVB\Dwg\ProfileB1.dpf")
		System.Windows.Forms.MessageBox.Show(CStr(oVar.ActionCount), "oVar.ActionCount-C106")

		Dim oVarList As ResultBuffer = oVar.List()
		zzWriteVar(oVarList, "Ball.txt")


		Dim oVarActList As ResultBuffer
		' oResB = oVar.Get("INTERSECTION_COLOR")
		'  zzWriteVar(oResB, "C11.txt")
		System.Windows.Forms.MessageBox.Show("VSEEE")
		Dim oVarAct As Variable = New Variable()
		Dim iRes As Integer
		For iIndex As Integer = 0 To oVar.ActionCount - 1
			iRes = oVar.GetActionTypeAt(iIndex, oVarAct)
			System.Windows.Forms.MessageBox.Show(CStr(iRes), "!!!!!iRes")
			oVarActList = oVarAct.List()
			zzWriteVar(oVarActList, "Cact" & Convert.ToString(iIndex) & ".txt")
		Next
		Try
			oTopologyClean.Init(oVar, colLinks)

		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadDocument.WriteMessage(String.Format(vbCrLf & "CleanupGet:Exception throwed containing the error code: {0}", oMapEx.ErrorCode))
		End Try


		'  System.Windows.Forms.MessageBox.Show("???")
		Try
			'  oResB()
			'   o = oVar.Get("INTERSECTION_COLOR")
			'For Each oTypedValue As TypedValue In oResB
			'iV += 1
			'shCode = oTypedValue.TypeCode
			'oValue = oTypedValue.Value

			'System.Windows.Forms.MessageBox.Show(CStr(shCode) & ":" & oValue.ToString(), CStr(iV))
			'Next
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C122")
		End Try
		'  System.Windows.Forms.MessageBox.Show("Y R A !!!", "!!!!!iRes")
		Try
			System.Windows.Forms.MessageBox.Show("C131")
			oTopologyClean.Start()
			If Not oTopologyClean.Completed Then
				System.Windows.Forms.MessageBox.Show("C133")
				System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))

				oTopologyClean.GroupNext()

				Dim i As Integer = 0
				Do
					If oTopologyClean.GroupErrorCount > 0 Then
						System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))
					End If



					For iIndex As Integer = 0 To oTopologyClean.GroupErrorCount - 1
						Try
							oTopologyClean.ErrorCur(iIndex)
						Catch ex As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(ex, False, "C251")
						End Try
						Try
							oTopologyClean.ErrorMark()
							' oTopologyClean.ErrorFix()

							'  oTopologyClean.ErrorDraw()
							'  zzMarkError(oPoint)
						Catch ex As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(ex, False, "C253")
						End Try
					Next

					Try
						oTopologyClean.GroupMark()
						' oTopologyClean.GroupFix()
						'  oTopologyClean.GroupDraw()
					Catch oEx As System.Exception
						DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C182a")
					End Try

					oTopologyClean.GroupNext()

				Loop Until oTopologyClean.Completed
			Else
				System.Windows.Forms.MessageBox.Show("??", "C121")
			End If

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "C114")
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C114a")
		End Try



		Try
			oTopologyClean.End()
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C116a")
		End Try



		System.Windows.Forms.MessageBox.Show("OK!", "C200")
		System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.Completed), "CTopologyClean.Completed-201")
		oTopologyClean = Nothing
		oVar = Nothing
	End Sub
	Public Shared Sub CleanupSet()


		Dim oTopologyClean As TopologyClean = New TopologyClean()

		Dim colLinks As ObjectIdCollection = Nothing	' zzGetLinksByLayerTest("LayerA")
		Dim oVar As Variable = New Variable()
		Dim oResBuffer As ResultBuffer = New ResultBuffer()


		Dim oVarList As ResultBuffer = oVar.List()


		Dim oVarAct As Variable

		oVarAct = New Variable()
		Dim oVal As TypedValue = New TypedValue(5001, 0.111)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)

		oVar.InsertActionToList(0, 1, oVarAct)

		oVarAct = New Variable()
		oVal = New TypedValue(5001, 0.222)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)

		oVar.InsertActionToList(1, 2, oVarAct)

		oVarAct = New Variable()
		oVal = New TypedValue(5001, 0.333)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)

		oVar.InsertActionToList(2, 4, oVarAct)


		oVarAct = New Variable()
		oVal = New TypedValue(5001, 0.444)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)

		oVar.InsertActionToList(3, 8, oVarAct)

		oVarAct = New Variable()
		oVal = New TypedValue(5001, 0.555)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)

		oVar.InsertActionToList(4, 16, oVarAct)

		oVarAct = New Variable()
		oVal = New TypedValue(5001, 0.666)

		oResBuffer.Add(oVal)

		oVarAct.Set("CLEAN_TOL", oResBuffer)
		System.Windows.Forms.MessageBox.Show("630")
		oVar.InsertActionToList(5, 32, oVarAct)

		oVal = New TypedValue(5005, "LayerA")
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		oVar.Set("LINK_LAYER", oResBuffer)
		System.Windows.Forms.MessageBox.Show("VSEE")
		Try
			oTopologyClean.Init(oVar, colLinks)

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "C112")
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C112a")
		End Try



		'  System.Windows.Forms.MessageBox.Show("???")
		Try
			'  oResB()
			'   o = oVar.Get("INTERSECTION_COLOR")
			'For Each oTypedValue As TypedValue In oResB
			'iV += 1
			'shCode = oTypedValue.TypeCode
			'oValue = oTypedValue.Value

			'System.Windows.Forms.MessageBox.Show(CStr(shCode) & ":" & oValue.ToString(), CStr(iV))
			'Next
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C122")
		End Try
		'  System.Windows.Forms.MessageBox.Show("Y R A !!!", "!!!!!iRes")
		Try
			System.Windows.Forms.MessageBox.Show("C131")
			oTopologyClean.Start()
			If Not oTopologyClean.Completed Then
				System.Windows.Forms.MessageBox.Show("C133")
				System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))

				oTopologyClean.GroupNext()

				Dim i As Integer = 0
				Do
					If oTopologyClean.GroupErrorCount > 0 Then
						System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))
					End If



					For iIndex As Integer = 0 To oTopologyClean.GroupErrorCount - 1
						Try
							oTopologyClean.ErrorCur(iIndex)
						Catch oEx As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C251")
						End Try
						Try
							oTopologyClean.ErrorMark()
							' oTopologyClean.ErrorFix()

							'  oTopologyClean.ErrorDraw()
							'  zzMarkError(oPoint)
						Catch oEx As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C253")
						End Try
					Next

					Try
						oTopologyClean.GroupMark()
						' oTopologyClean.GroupFix()
						'  oTopologyClean.GroupDraw()
					Catch oEx As System.Exception
						DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C182a")
					End Try

					oTopologyClean.GroupNext()

				Loop Until oTopologyClean.Completed
			Else
				System.Windows.Forms.MessageBox.Show("??", "C121")
			End If

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "C114")
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C114a")
		End Try
		System.Windows.Forms.MessageBox.Show("C139")

		Try
			oTopologyClean.End()
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C116a")
		End Try

		System.Windows.Forms.MessageBox.Show("OK!", "C200")
		System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.Completed), "CTopologyClean.Completed-201")
		oTopologyClean = Nothing
		oVar = Nothing
	End Sub
	Public Shared Function CleanupList(ByVal colLinks As ObjectIdCollection, ByVal sNewLinesLayer As String, ByVal bFix As Boolean, ByRef oaErrorPoints() As TplnPointArray) As Integer()
		Dim iActionUB As Integer
		Dim iaActionID() As Integer = {enCleanupAction.EraseDanglingObjects, enCleanupAction.ExtendUndershoots, enCleanupAction.BreakCrossingObjects, enCleanupAction.SnapClusteredNodes}
		Dim daTolerance() As Double = {0.05, 0.05, 0.05, 0.05}
		iActionUB = iaActionID.GetUpperBound(0)
		If iActionUB >= 0 Then
			Dim oaActionVar(iActionUB) As ActionVar
			ReDim oaErrorPoints(iActionUB)
			Try
				For iIndex As Integer = 0 To iActionUB
					oaActionVar(iIndex) = New ActionVar(iaActionID(iIndex), daTolerance(iIndex))
				Next
			Catch oEx As Exception

			End Try
			Return Cleanup(oaActionVar, String.Empty, sNewLinesLayer, colLinks, bFix, oaErrorPoints)
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function CleanupByThemeData(ByVal oaActionVar() As ActionVar, ByVal oMapThemeData As MapThemeData, ByVal bFix As Boolean, ByRef oaErrorPoints() As TplnPointArray) As Integer()
		Try
			Dim sNewLinesLayer As String = String.Empty
			Dim sBaseLayers As String = String.Empty
			Dim colLinks As ObjectIdCollection = Nothing

			sBaseLayers = oMapThemeData.LineLinkLayers
			Return Cleanup(oaActionVar, sBaseLayers, sNewLinesLayer, colLinks, bFix, oaErrorPoints)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString() & vbCrLf & oEx.StackTrace, "26_996")
			Return Nothing
		End Try

	End Function
	Public Shared Function CleanupByTopoDef(ByVal oaActionVar() As ActionVar, ByVal oTopoDef As TopoDef, ByVal bFix As Boolean, ByRef oaErrorPoints() As TplnPointArray) As Integer()
		Try
			Dim sNewLinesLayer As String = String.Empty
			Dim sBaseLayers As String = String.Empty
			Dim colLinks As ObjectIdCollection = Nothing

			If oTopoDef.LinkLayersExists Then
				sBaseLayers = oTopoDef.LinkLayers
			Else
				sBaseLayers = oTopoDef.IncludeLayers
			End If
			
			'	MessageBox.Show(sBaseLayers & vbCrLf & "'" & sNewLinesLayer & "'", "05_310")
			Return Cleanup(oaActionVar, sBaseLayers, sNewLinesLayer, colLinks, bFix, oaErrorPoints)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "26_993")
			Return Nothing
		End Try

	End Function
	Public Shared Function CleanupByTopoDef_040609(ByVal oaActionVar() As ActionVar, ByVal oTopoDef As TopoDef, ByVal bFix As Boolean, ByRef oaErrorPoints() As TplnPointArray) As Integer()
		Dim sNewLinesLayer As String = String.Empty
		Dim sBaseLayer As String = String.Empty
		Dim colLinks As ObjectIdCollection = Nothing
		If oTopoDef.LinkLayersExists Then
			If DMAcadExt.AcadTransaction.LinkExists(oTopoDef.LinkLayers) Then
				sBaseLayer = oTopoDef.LinkLayers
			Else
				sBaseLayer = oTopoDef.IncludeLayers
				sNewLinesLayer = oTopoDef.LinkLayers
			End If
		Else
			sBaseLayer = oTopoDef.IncludeLayers
		End If
		Return Cleanup(oaActionVar, sBaseLayer, sNewLinesLayer, colLinks, bFix, oaErrorPoints)
	End Function

	Public Shared Function Cleanup(ByVal oaActionVar() As ActionVar, ByVal sBaseLayers As String, ByVal sNewLinesLayer As String, ByVal colLinks As ObjectIdCollection, ByVal bFix As Boolean, ByRef oaErrorPoints() As TplnPointArray) As Integer()
		Dim bCurrentLayerOK As Boolean = True
		Dim oTopologyClean As TopologyClean = New TopologyClean()
		Dim oVar As Variable = New Variable()
		Dim iActionUB As Integer = oaActionVar.GetUpperBound(0)
		Dim iaOut(iActionUB) As Integer
		Dim bNewLinesLayerExists As Boolean = True

		If sNewLinesLayer Is Nothing Then sNewLinesLayer = String.Empty
		Dim iColCount As Integer = -1
		If colLinks IsNot Nothing Then
			iColCount = colLinks.Count
		End If

		If oaActionVar.GetUpperBound(0) = 0 Then
			Dim oActionVar As ActionVar = oaActionVar(0)

			'MessageBox.Show(CStr(oActionVar.ActionID) & vbCrLf & CStr(oActionVar.Tolerance), "05_018r")
		End If
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
		If bCurrentLayerOK Then
			If sNewLinesLayer IsNot Nothing AndAlso sNewLinesLayer.Length <> 0 Then
				bNewLinesLayerExists = DMAcadExt.AcadTransaction.CreateLayer(sNewLinesLayer, DMApp.AppID, DMAcadExt.enLayerFunction.Default, True)
			Else
				sNewLinesLayer = String.Empty
			End If
			If bNewLinesLayerExists Then
				'	sBaseLayer = "" '"1601"
				''''''''''''''''	sBaseLayers = ""
				oVar = zzBuildVar(oaActionVar, sBaseLayers, sNewLinesLayer)
				'''''''''''''''colLinks = New ObjectIdCollection
				''''''''''''''''''colLinks = DMAcadExt.AcadTransaction.GetLinks("1602")

				'DMCommon.Debug.MsgBox("12_251", sBaseLayers, colLinks.Count)
				Try

					oTopologyClean.Init(oVar, colLinks)
					'''''''''''''	oTopologyClean.InitAnchorSet()
					'	MessageBox.Show(sBaseLayers & vbCrLf & "'" & sNewLinesLayer & "'" & vbCrLf & CStr(colLinks.Count), "05_401")
				Catch oMapTopoEx As Autodesk.Gis.Map.MapTopologyException
					DMAcadExt.AcadErrCode.GetMapEx(oMapTopoEx, False, "C564a")
					Return iaOut
				End Try
				Dim iActionIndex As Integer
				Dim iActionID As Integer
				Dim iSubTypeIndex As Integer = 0
				Dim iActionErrCount As Integer
				Dim oPointArray As TplnPointArray
				Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d
				Dim oPoint As DMAcadExt.TPlnPoint
				Dim iPrevPointIndex As Integer
				Dim sTest As String = "a"
				Try
					oTopologyClean.Start()
					sTest = "b"
					If Not oTopologyClean.Completed Then
						Dim i As Integer = 0
						iActionIndex = -1
						sTest = "b1"
						Do
							oTopologyClean.GroupNext()
                     If oTopologyClean.Completed Then Exit Do
                     '   DMCommon.Debug.MsgBox("12_299", iActionUB, iActionIndex, iSubTypeIndex, iActionID)
							Do While iSubTypeIndex = 0
								iActionIndex += 1
                        iActionErrCount = 0
                        ' DMCommon.Debug.MsgBox("12_300", iActionUB, iActionIndex, iSubTypeIndex, iActionID)
                        If iActionIndex > iActionUB Then

                           DMCommon.Debug.MsgBox("Design 287", iActionIndex, iActionUB, iActionID)
                           Return iaOut
                           Exit Function
                        End If
								iActionID = oaActionVar(iActionIndex).ActionID
                        iSubTypeIndex = zzGetSubtypeUB(iActionID)
                        '  DMCommon.Debug.MsgBox("12_301", iActionUB, iActionIndex, iSubTypeIndex, iActionID)
							Loop
							sTest = "b5"
							iSubTypeIndex -= 1
							iaOut(iActionIndex) += oTopologyClean.GroupErrorCount
							sTest = "b5s ind=" & CStr(iActionIndex) & ";" & CStr(iaOut.GetUpperBound(0))
							If oaErrorPoints(iActionIndex) Is Nothing Then
								sTest = "b11"
								oPointArray = New TplnPointArray(oTopologyClean.GroupErrorCount - 1)
								sTest = "b12"
								oaErrorPoints(iActionIndex) = oPointArray
								sTest = "b13"
								iPrevPointIndex = 0
								sTest = "b13"
							Else
								sTest = "b21"
								oPointArray = oaErrorPoints(iActionIndex)
								sTest = "b22"
								iPrevPointIndex = oPointArray.UpperBound + 1
								sTest = "b23"
								oPointArray.AddDim(oTopologyClean.GroupErrorCount - 1)
								sTest = "b24"
							End If
							iActionErrCount += oTopologyClean.GroupErrorCount
							sTest = "b77"
							For iIndex As Integer = 0 To oTopologyClean.GroupErrorCount - 1
								sTest = "k_" & CStr(iActionIndex) & ":" & CStr(iIndex) & " of " & CStr(oTopologyClean.GroupErrorCount - 1)
								Try
									oTopologyClean.ErrorCur(iPrevPointIndex + iIndex)
								Catch oEx As System.Exception
									DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "TopoCreator - Cleanup_3")
								End Try

								Try
									If bFix Then
										oTopologyClean.ErrorFix()
									Else
										oTopologyClean.ErrorMark()
									End If
								Catch oEx As System.Exception
									DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "TopoCreator - Cleanup_4")
								End Try
								Try
									oTopologyClean.ErrorDraw()
								Catch oEx As System.Exception
									If Not bFix Then
										DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "TopoCreator - Cleanup_5a")
									End If
								End Try
								Try
									tPoint3d = oTopologyClean.ErrorPoint
									oPoint = New TPlnPoint(tPoint3d)
									oPointArray.Item(iIndex) = oPoint
									'	zzMarkError(tPoint3d)
								Catch oEx As System.Exception
									If Not bFix Then
										DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "TopoCreator - Cleanup_5b")
									End If
								End Try
							Next
							Try
								If bFix Then
									oTopologyClean.GroupFix()
								Else
									oTopologyClean.GroupMark()
								End If
							Catch oEx As System.Exception
								DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "TopoCreator - Cleanup_6")
                     End Try

						Loop

					Else
						System.Windows.Forms.MessageBox.Show("Topology was not complete", "TopoCreator - Cleanup_11", MessageBoxButtons.OK, MessageBoxIcon.Warning)
					End If

				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest, "TopoCreator - Cleanup_7a")
					DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C114a")
				End Try
				Try
					oTopologyClean.End()
				Catch oMapEx As Autodesk.Gis.Map.MapTopologyException
					DMAcadExt.AcadErrCode.GetMapEx(oMapEx, False, "TopoCreator - Cleanup_8")
				End Try
				'   colCreatedSelSet = oTopologyClean.CreatedSelectionSet
				'   System.Windows.Forms.MessageBox.Show(CStr(colCreatedSelSet.Count), "colCreatedSelSet")
				Try
					Dim oVarList As ResultBuffer = oVar.List()
					'   zzWriteVar(oVarList, "Dall.txt")

					oTopologyClean = Nothing
					oVar = Nothing

					Erase oaActionVar
					oaActionVar = Nothing

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - Cleanup_9")
				End Try
				zzCommandLine()
			End If
		End If
		Return iaOut
	End Function
	Public Shared Sub CleanupBb()
		Dim oTopologyClean As TopologyClean = New TopologyClean()

		Dim colLinks As ObjectIdCollection = zzGetLinksByLayerTest("LayerA")
		Dim oVar As Variable = New Variable()
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count), "colLinks.Count-C105")
		oVar.LoadProfile("C:\NetProjects8\Examples\DotNet\TopologyVB\Dwg\ProfileB.dpf")
		System.Windows.Forms.MessageBox.Show(CStr(oVar.ActionCount), "oVar.ActionCount-C106")
		'  oVar.InsertActionToList(
		Dim oVarList As ResultBuffer = oVar.List()


		Dim iV As Integer = 0
		Dim ssInclude As ObjectIdCollection
		Dim sFileName As String = "C:\NetProjects8\Examples\DotNet\TopologyVB\Dwg\B1.txt"

		Dim oPoint As Autodesk.AutoCAD.Geometry.Point3d

		zzWriteVar(oVarList, "B3.txt")


		' Dim oResB As ResultBuffer
		' oResB = oVar.Get("INTERSECTION_COLOR")
		'  zzWriteVar(oResB, "C11.txt")
		System.Windows.Forms.MessageBox.Show("VSEEE")

		Dim oVarAct As Variable = New Variable()
		Dim oVal As TypedValue = New TypedValue(5001, 0.555)
		System.Windows.Forms.MessageBox.Show("10")
		oResBuffer.Add(oVal)
		System.Windows.Forms.MessageBox.Show("20")
		oVar.Set("CLEAN_TOL", oResBuffer)
		System.Windows.Forms.MessageBox.Show("30")
		oVar.InsertActionToList(0, 1, oVar)
		System.Windows.Forms.MessageBox.Show("40")
		Dim oVarListA As ResultBuffer = oVar.List()
		System.Windows.Forms.MessageBox.Show("50")
		zzWriteVar(oVarListA, "D1.txt")

		oResBuffer = New ResultBuffer()

		oVal = New TypedValue(5003, 5)
		oResBuffer.Add(oVal)

		oVar.Set("INTERSECTION_COLOR", oResBuffer)

		Try
			oTopologyClean.Init(oVar, colLinks)

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "C112")
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C112a")
		End Try
		Dim iRes As Integer = oVar.GetActionTypeAt(2, oVarAct)
		System.Windows.Forms.MessageBox.Show(CStr(iRes), "!!!!!iRes")
		Dim oVarActList As ResultBuffer = oVarAct.List()
		zzWriteVar(oVarActList, "C3.txt")
		Try
			ssInclude = oVar.GetIncludeSelectionSet()
			System.Windows.Forms.MessageBox.Show(CStr(ssInclude.Count), "Include.Count")
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C124")
		End Try

		'  System.Windows.Forms.MessageBox.Show("???")
		Try
			'  oResB()
			'   o = oVar.Get("INTERSECTION_COLOR")
			'For Each oTypedValue As TypedValue In oResB
			'iV += 1
			'shCode = oTypedValue.TypeCode
			'oValue = oTypedValue.Value

			'System.Windows.Forms.MessageBox.Show(CStr(shCode) & ":" & oValue.ToString(), CStr(iV))
			'Next

		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C122")
		End Try
		'  System.Windows.Forms.MessageBox.Show("Y R A !!!", "!!!!!iRes")
		Try
			System.Windows.Forms.MessageBox.Show("C131")
			oTopologyClean.Start()
			If Not oTopologyClean.Completed Then
				System.Windows.Forms.MessageBox.Show("C133")
				System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))

				oTopologyClean.GroupNext()

				Dim i As Integer = 0
				Do
					If oTopologyClean.GroupErrorCount > 0 Then
						System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.GroupErrorCount), "Type=" & CStr(oTopologyClean.GroupType) & "." & CStr(oTopologyClean.GroupSubType))
					End If



					For iIndex As Integer = 0 To oTopologyClean.GroupErrorCount - 1
						Try
							oTopologyClean.ErrorCur(iIndex)
						Catch oEx As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C251")
						End Try
						Try
							oTopologyClean.ErrorMark()
							' oTopologyClean.ErrorFix()
							oPoint = oTopologyClean.ErrorPoint
							'  oTopologyClean.ErrorDraw()
							'  zzMarkError(oPoint)
						Catch oEx As System.Exception
							DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C253")
						End Try
					Next

					Try
						oTopologyClean.GroupMark()
						' oTopologyClean.GroupFix()
						'  oTopologyClean.GroupDraw()
					Catch oEx As System.Exception
						DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C182a")
					End Try

					oTopologyClean.GroupNext()

				Loop Until oTopologyClean.Completed
			Else
				System.Windows.Forms.MessageBox.Show("??", "C121")
			End If

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "C114")
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C114a")
		End Try
		System.Windows.Forms.MessageBox.Show("C139")


		Try
			oTopologyClean.End()
		Catch oEx As System.Exception
			DMAcadExt.AcadErrCode.GetMapEx(oEx, False, "C116a")
		End Try



		System.Windows.Forms.MessageBox.Show("OK!", "C200")
		System.Windows.Forms.MessageBox.Show(CStr(oTopologyClean.Completed), "CTopologyClean.Completed-201")
		oTopologyClean = Nothing
		oVar = Nothing
	End Sub


	'Dim oStreamReader As IO.StreamReader

	Dim sSourceLine, sDestLine As String, sAddDestLine As String
	Dim sPointName As String, sAddPointName As String





	Public Shared Sub CreateMapTopos()
		zzScan()
   End Sub
   Private Shared Function zzBuildVar(ByVal oaActionVar() As ActionVar, ByVal sLayers As String, Optional ByVal sNewLayer As String = "") As Autodesk.Gis.Map.Topology.Variable
      Dim oVar As Autodesk.Gis.Map.Topology.Variable = New Variable()
      Dim oVarAct As Variable

      For iIndex As Integer = 0 To oaActionVar.GetUpperBound(0)
         oVarAct = New Variable()

         Select Case oaActionVar(iIndex).ActionID
            Case 1, 4, 8, 16, 64, 128, 512
               Try
						oVarAct.Set("CLEAN_TOL", DMAcadExt.AcadUtil.GetResBuffer(oaActionVar(iIndex).Tolerance, True))
					Catch oEx As System.Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - zzBuildVar_11")
               End Try
         End Select
         Try
            oVar.InsertActionToList(iIndex, oaActionVar(iIndex).ActionID, oVarAct)
         Catch oMapTopoEx As MapTopologyException
            System.Windows.Forms.MessageBox.Show(oMapTopoEx.Message & vbCrLf & "AdsErrorCode=" & oMapTopoEx.AdsErrorCode & vbCrLf & CStr(iIndex) & "," & CStr(oaActionVar(iIndex).ActionID) & ":" & oVarAct.ToString(), "TopoCreator - zzBuildVar_12")
         End Try
      Next
      '  System.Windows.Forms.MessageBox.Show(sLayers, "W400-INCLUDEOBJS_LAYERS")

      If String.IsNullOrEmpty(sLayers) Then
			oVar.Set("INCLUDEOBJS_AUTOSELECT", DMAcadExt.AcadUtil.GetResBuffer(0, True))
		Else
			oVar.Set("INCLUDEOBJS_LAYERS", DMAcadExt.AcadUtil.GetResBuffer(sLayers, True))
		End If

		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		oVar.Set("CREATE_CNTR", DMAcadExt.AcadUtil.GetResBuffer(0, True))
		''''''''''''''''''''''''''''''''''''''''''''''''''''
		oVar.Set("STOP_AT_MISSING_CNTR", DMAcadExt.AcadUtil.GetResBuffer(1, True))
		''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Try
         If sNewLayer.Length <> 0 Then
				oVar.Set("ENT_PROCESS", DMAcadExt.AcadUtil.GetResBuffer(1, True))
				oVar.Set("LINK_LAYER", DMAcadExt.AcadUtil.GetResBuffer(sNewLayer, True))
				oVar.Set("CNTR_LAYER", DMAcadExt.AcadUtil.GetResBuffer(sNewLayer, True))
			Else
				oVar.Set("ENT_PROCESS", DMAcadExt.AcadUtil.GetResBuffer(0, True))
			End If
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - zzBuildVar")
      End Try

      '   System.Windows.Forms.MessageBox.Show("End  zzBuildVar", "W440- zzBuildVar")
      Return oVar
   End Function


   Private Shared Function zzBuildVar011217(ByVal oaActionVar() As ActionVar, ByVal sLayers As String, Optional ByVal sNewLayer As String = "") As Autodesk.Gis.Map.Topology.Variable
      Dim oVar As Autodesk.Gis.Map.Topology.Variable = New Variable()
      Dim oVarAct As Variable

      For iIndex As Integer = 0 To oaActionVar.GetUpperBound(0)


         Select Case oaActionVar(iIndex).ActionID
            Case 1, 4, 8, 16, 64, 128, 512
               Try
                  oVarAct = New Variable()
						oVarAct.Set("CLEAN_TOL", DMAcadExt.AcadUtil.GetResBuffer(oaActionVar(iIndex).Tolerance, True))
					Catch oEx As System.Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - zzBuildVar_11")
                  oVarAct = Nothing
               End Try
            Case Else
               oVarAct = Nothing
         End Select
         If oVarAct IsNot Nothing Then
            Try
               oVar.InsertActionToList(iIndex, oaActionVar(iIndex).ActionID, oVarAct)
            Catch oMapTopoEx As MapTopologyException
               System.Windows.Forms.MessageBox.Show(oMapTopoEx.Message & vbCrLf & "AdsErrorCode=" & oMapTopoEx.AdsErrorCode & vbCrLf & CStr(iIndex) & "," & CStr(oaActionVar(iIndex).ActionID) & ":" & oVarAct.ToString(), "TopoCreator - zzBuildVar_12")
            End Try
         End If
         If oVarAct IsNot Nothing Then
            Try
               oVar.InsertActionToList(iIndex, oaActionVar(iIndex).ActionID, oVarAct)
            Catch oMapTopoEx As MapTopologyException
               System.Windows.Forms.MessageBox.Show(oMapTopoEx.Message & vbCrLf & "AdsErrorCode=" & oMapTopoEx.AdsErrorCode & vbCrLf & CStr(iIndex) & "," & CStr(oaActionVar(iIndex).ActionID) & ":" & oVarAct.ToString(), "TopoCreator - zzBuildVar_12")
            End Try
         End If


      Next
      '  System.Windows.Forms.MessageBox.Show(sLayers, "W400-INCLUDEOBJS_LAYERS")

      If String.IsNullOrEmpty(sLayers) Then
			oVar.Set("INCLUDEOBJS_AUTOSELECT", DMAcadExt.AcadUtil.GetResBuffer(0, True))
		Else
			oVar.Set("INCLUDEOBJS_LAYERS", DMAcadExt.AcadUtil.GetResBuffer(sLayers, True))
		End If

		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		oVar.Set("CREATE_CNTR", DMAcadExt.AcadUtil.GetResBuffer(0, True))
		''''''''''''''''''''''''''''''''''''''''''''''''''''
		oVar.Set("STOP_AT_MISSING_CNTR", DMAcadExt.AcadUtil.GetResBuffer(1, True))
		''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Try
         If sNewLayer.Length <> 0 Then
				oVar.Set("ENT_PROCESS", DMAcadExt.AcadUtil.GetResBuffer(1, True))
				oVar.Set("LINK_LAYER", DMAcadExt.AcadUtil.GetResBuffer(sNewLayer, True))
				oVar.Set("CNTR_LAYER", DMAcadExt.AcadUtil.GetResBuffer(sNewLayer, True))
			Else
				oVar.Set("ENT_PROCESS", DMAcadExt.AcadUtil.GetResBuffer(0, True))
			End If
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - zzBuildVar")
      End Try

      '   System.Windows.Forms.MessageBox.Show("End  zzBuildVar", "W440- zzBuildVar")
      Return oVar
   End Function
	Private Shared Function zzBuildVar040213(ByVal oaActionVar() As ActionVar, ByVal sLayers As String, Optional ByVal sNewLayer As String = "") As Autodesk.Gis.Map.Topology.Variable
		'	Dim oTopologyClean As TopologyClean = New TopologyClean()
		Dim oVar As Autodesk.Gis.Map.Topology.Variable = New Variable()
		Dim oResBuffer As ResultBuffer '=  New ResultBuffer()
		Dim oVarAct As Variable
		Dim oVal As TypedValue
		'   System.Windows.Forms.MessageBox.Show("W200")
		For iIndex As Integer = 0 To oaActionVar.GetUpperBound(0)
			oVarAct = New Variable()
			oVal = New TypedValue(5001, oaActionVar(iIndex).Tolerance)
			'oVal = New TypedValue(DxfCode.Real, oaActionVar(iIndex).Tolerance)
			oResBuffer = New ResultBuffer()
			Select Case oaActionVar(iIndex).ActionID
				Case 1, 4, 8, 16, 64, 128, 512
					Try
						oResBuffer.Add(oVal)
						oVarAct.Set("CLEAN_TOL", oResBuffer)
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - zzBuildVar_11")
					End Try
			End Select
			Try
				oVar.InsertActionToList(iIndex, oaActionVar(iIndex).ActionID, oVarAct)
			Catch oMapTopoEx As MapTopologyException
				System.Windows.Forms.MessageBox.Show(oMapTopoEx.Message & vbCrLf & "AdsErrorCode=" & oMapTopoEx.AdsErrorCode & vbCrLf & CStr(iIndex) & "," & CStr(oaActionVar(iIndex).ActionID) & ":" & oVarAct.ToString(), "TopoCreator - zzBuildVar_12")
			End Try
		Next
		'  System.Windows.Forms.MessageBox.Show(sLayers, "W400-INCLUDEOBJS_LAYERS")

		oVal = New TypedValue(5005, sLayers)
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		'''''''''''''''	oVar.Set("INCLUDEOBJS_LAYERS", oResBuffer)
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''



		oVal = New TypedValue(5003, 0)
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		oVar.Set("INCLUDEOBJS_AUTOSELECT", oResBuffer)


		oVal = New TypedValue(5003, 0)
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		oVar.Set("CREATE_CNTR", oResBuffer)
		''''''''''''''''''''''''''''''''''''''''''''''''''''
		oVal = New TypedValue(5003, 1)
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		oVar.Set("STOP_AT_MISSING_CNTR", oResBuffer)
		''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Try
			If sNewLayer.Length <> 0 Then
				oVal = New TypedValue(5003, 1)
				oResBuffer = New ResultBuffer()
				oResBuffer.Add(oVal)
				oVar.Set("ENT_PROCESS", oResBuffer)

				oVal = New TypedValue(5005, sNewLayer)
				oResBuffer = New ResultBuffer()
				oResBuffer.Add(oVal)
				oVar.Set("LINK_LAYER", oResBuffer)

				oVal = New TypedValue(5005, sNewLayer)
				oResBuffer = New ResultBuffer()
				oResBuffer.Add(oVal)
				oVar.Set("CNTR_LAYER", oResBuffer)
			Else
				oVal = New TypedValue(5003, 0)
				oResBuffer = New ResultBuffer()
				oResBuffer.Add(oVal)
				oVar.Set("ENT_PROCESS", oResBuffer)
			End If
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - zzBuildVar")

		End Try

		'   System.Windows.Forms.MessageBox.Show("End  zzBuildVar", "W440- zzBuildVar")
		Return oVar
	End Function

	Private Shared Sub zzAddTypedValue(ByVal iTypedCode As Integer, ByVal sVarName As String, ByVal oValue As System.Object, ByRef oVariable As Autodesk.Gis.Map.Topology.Variable)
		Dim oResBuffer As ResultBuffer
		Dim oVal As TypedValue
		oVal = New TypedValue(iTypedCode, oValue)
		oResBuffer = New ResultBuffer()
		oResBuffer.Add(oVal)
		oVariable.Set("CNTR_LAYER", oResBuffer)
	End Sub

	Private Shared Sub zzScan()
		Dim oTransaction As Transaction = Nothing

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity
		Dim oBlockRef As BlockReference
		Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		Dim oaLinks(iToposUB) As ObjectIdCollection
		Dim oaCentroids(iToposUB) As ObjectIdCollection
		Dim sIbjTest As String = ""
		Dim iTest As ObjectId
		Dim oTestBlockTableRecord As BlockTableRecord
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			iTest = moCurrentDatabase.BlockTableId

			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			System.Windows.Forms.MessageBox.Show("210")
			System.Windows.Forms.MessageBox.Show(BlockTableRecord.ModelSpace, "ModelSpace")


			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			System.Windows.Forms.MessageBox.Show("211")
			oTestBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item("Centroid"), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			oTestBlockTableRecord.GetBlockReferenceIds(True, True)
			Dim sRXClassName As String
			Dim sLayerName As String
			Dim iTopologyIndex As Integer
			Dim sTest As String = ""
			For Each objId As ObjectId In oBlockTableRecord
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name
				System.Windows.Forms.MessageBox.Show(sRXClassName)
				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName
						sLayerName = oEntity.Layer
						If mdicLinkLayers.OpenElem(sLayerName) Then
							Do
								iTopologyIndex = mdicLinkLayers.Current
								oaLinks(iTopologyIndex).Add(objId)
							Loop While mdicLinkLayers.MoveNext()
						End If
					Case AcadConst.AcadBlockRefName
						oBlockRef = DirectCast(oEntity, BlockReference)
						sTest = "AnonymousBlockTableRecord=" & oBlockRef.AnonymousBlockTableRecord.ToString()
						sTest &= vbCrLf & "BlockId=" & oBlockRef.BlockId().ToString()
						sTest &= vbCrLf & "BlockTableRecord=" & oBlockRef.BlockTableRecord().ToString()
						sTest &= vbCrLf & "Id=" & oBlockRef.Id.ToString()
						sTest &= vbCrLf & "LayerId=" & oBlockRef.LayerId.ToString()
						sTest &= vbCrLf & "ObjectId=" & oBlockRef.ObjectId().ToString()
						System.Windows.Forms.MessageBox.Show(sTest)

						If mdicLinkLayers.OpenElem(oBlockRef.Layer) Then
							Do
								iTopologyIndex = mdicLinkLayers.Current
								oaLinks(iTopologyIndex).Add(objId)
							Loop While mdicLinkLayers.MoveNext()
						End If

				End Select

			Next

			System.Windows.Forms.MessageBox.Show("212")

			oTransaction.Commit()
			oTransaction = Nothing
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e1200")
		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing

			End If

		End Try
   End Sub
   Private Shared Function zzNodeHasEntity(oNode As Node) As Boolean
      Try
         Return Not oNode.Entity.IsNull
      Catch oMapEx As MapException
         If oMapEx.ErrorCode <> 2035 Then
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "NodeHasEntity")
         End If
         Return False
      End Try
   End Function
	Private Shared Function zzGetAllLinks() As ObjectIdCollection
		Dim oTransaction As Transaction = Nothing
		Dim sLayerDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		sLayerDel = moTopoDefs.LinkLayers
		sLayerDel = TopoDef.AddDelim(sLayerDel)

		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			Dim sRXClassName As String
			Dim sLayerName As String


			For Each objId As ObjectId In oBlockTableRecord
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name

				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName
						sLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayerDel.Contains(sLayerName) Then
							colResIds.Add(objId)
						End If
				End Select

			Next

			System.Windows.Forms.MessageBox.Show("212")

			oTransaction.Commit()
			oTransaction = Nothing

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e1300")

		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing
			End If

		End Try
		Return colResIds
	End Function
	Private Shared Function zzGetLinksAAA(ByVal sLayers As String) As ObjectIdCollection
		Dim oTransaction As Transaction = Nothing
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		'  sLayersDel = moTopoDefs.LinkLayers
		sLayersDel = TopoDef.AddDelim(sLayers)
		Dim sTest As String = "a"
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			sTest = "b"
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			sTest = "c"
			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			sTest = "d"
			Dim sRXClassName As String
			Dim sEntityLayerName As String

			For Each objId As ObjectId In oBlockTableRecord
				sTest = "e"
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sTest = "f"
				sRXClassName = oEntity.GetRXClass().Name
				sTest = "g"
				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName, Common.AcadLineName
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							sTest = "h"
							colResIds.Add(objId)
							sTest = "i"
						End If
				End Select
			Next
			sTest = "j"
			oTransaction.Commit()
			sTest = "k"
			oTransaction = Nothing
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "e1400")
		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing
			End If
		End Try
		Return colResIds
	End Function


	Public Shared Sub EraseLinksAAA(ByVal sLayers As String)
		Dim oTransaction As Transaction = Nothing
		Dim sLayersDel As String

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		'  sLayersDel = moTopoDefs.LinkLayers
		sLayersDel = TopoDef.AddDelim(sLayers)
		Dim sTest As String = "A"
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			sTest = "B"
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
			sTest = "C"
			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			sTest = "D"
			Dim sRXClassName As String
			Dim sEntityLayerName As String


			For Each objId As ObjectId In oBlockTableRecord
				sTest = "Ka"
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
				sTest = "Kb"
				sRXClassName = oEntity.GetRXClass().Name
				sTest = "Kc"
				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName
						sTest = "Kca"
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						sTest = "Kcb"
						If sLayersDel.Contains(sEntityLayerName) Then
							sTest = "Kcc"
							oEntity = Nothing
							sTest = "Kcd"
							oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)

							sTest = "M"
							oEntity.Erase()
							sTest = "N"
							oEntity.Dispose()
							sTest = "P"
							oEntity = Nothing
							sTest = "Pp"
						End If
				End Select

			Next
			sTest = "W"
			oTransaction.Commit()
			oTransaction = Nothing

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - Eraselinks " & sTest)

		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing

			End If

		End Try

	End Sub
	Private Shared Function zzLinkExistsAAA_BBB(ByVal sLayers As String) As Boolean
		Dim oTransaction As Transaction = Nothing
		Dim sLayersDel As String

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity
		Dim bResult As Boolean = False

		sLayersDel = TopoDef.AddDelim(sLayers)
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			Dim sRXClassName As String
			Dim sEntityLayerName As String


			For Each objId As ObjectId In oBlockTableRecord
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name

				Select Case sRXClassName
					Case AcadConst.AcadPolylineName, AcadConst.AcadLineName, AcadConst.AcadArcName, AcadConst.Acad2dPolylineName
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							bResult = True
							Exit For
						End If
					Case Else
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							System.Windows.Forms.MessageBox.Show(sRXClassName, "???sRXClassName???zzLinkExists")
						End If
				End Select

			Next
			oTransaction.Commit()
			oTransaction = Nothing

		Catch e As Exception
			System.Windows.Forms.MessageBox.Show(e.Message, "e1500")

		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing

			End If

		End Try
		Return bResult
	End Function

	Private Shared Function zzGetLinksByLayerTest(ByVal sLayer As String) As ObjectIdCollection
		Dim oTransaction As Transaction = Nothing
		'
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity

		'
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			Dim sRXClassName As String



			For Each objId As ObjectId In oBlockTableRecord
				oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name

				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName
						If sLayer = oEntity.Layer Then
							colResIds.Add(objId)
						End If

				End Select

			Next

			System.Windows.Forms.MessageBox.Show("212")

			oTransaction.Commit()
			oTransaction = Nothing
			Return colResIds
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e1100")
			Return Nothing
		Finally
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing

			End If

		End Try
	End Function

	Private Shared Function zzGetBlockRefs(ByVal sBlockName As String, Optional ByVal sLayers As String = "") As ObjectIdCollection
		Dim oTransaction As Transaction = Nothing
		'    Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oBlockAcObjId As ObjectId
		'   Dim bTest As Boolean
		Dim oOutBlockCol As ObjectIdCollection
		Try
			oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
			oBlockTable = DirectCast(oTransaction.GetObject(moCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e200a")

			oOutBlockCol = New ObjectIdCollection()
		End Try



		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try

				oBlockAcObjId = oBlockTable.Item(sBlockName)
				oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				Dim oAllBlockCol As ObjectIdCollection
				oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
				If sLayers.Length = 0 Then
					oOutBlockCol = oAllBlockCol
				Else
					oOutBlockCol = New ObjectIdCollection()
					Dim oEntity As Entity
					Dim sEntityLayerName As String
					sLayers = TopoDef.AddDelim(sLayers)
					For Each objId As ObjectId In oAllBlockCol
						oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayers.Contains(sEntityLayerName) Then
							oOutBlockCol.Add(objId)
						End If
					Next
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "e200a")

				oOutBlockCol = New ObjectIdCollection()
			Finally
				If Not oTransaction Is Nothing Then
					oTransaction.Abort()
					oTransaction = Nothing
				End If
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Block " & sBlockName & " was not found", "TopoCreator - zzGetBlockRefs")
			oOutBlockCol = New ObjectIdCollection()
			If Not oTransaction Is Nothing Then
				oTransaction.Abort()
				oTransaction = Nothing
			End If
		End If

		Return oOutBlockCol
	End Function
	Private Shared Function zzGetAllBlockRefs() As ObjectIdCollection
		Dim saCentroidBlocks() As String = moTopoDefs.CentroidBlocks
		Dim colResIds As ObjectIdCollection = Nothing
		Dim colBlockRefIds As ObjectIdCollection = New ObjectIdCollection()
		System.Windows.Forms.MessageBox.Show("zzGetAllBlockRefs")
		System.Windows.Forms.MessageBox.Show(saCentroidBlocks.GetUpperBound(0).ToString)
		Try
			For iBlockIndex As Integer = 0 To saCentroidBlocks.GetUpperBound(0)
				If iBlockIndex = 0 Then
					System.Windows.Forms.MessageBox.Show(saCentroidBlocks(iBlockIndex))
					colResIds = zzGetBlockRefs(saCentroidBlocks(iBlockIndex))
				Else
					colBlockRefIds = zzGetBlockRefs(saCentroidBlocks(iBlockIndex))
					For iBlockRefIndex As Integer = 0 To colBlockRefIds.Count - 1
						colResIds.Add(colBlockRefIds.Item(iBlockRefIndex))
					Next

				End If
			Next
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e200")
			Return Nothing
		End Try

		Return colResIds
	End Function

	Public Sub New()

	End Sub
	Public Shared Function CleanupActionItems() As DMCommon.ItemData()
		'	MessageBox.Show(CStr(CleanupActionNames Is Nothing), "10_001")
		Dim iUB As Integer = CleanupActionNames.GetUpperBound(0)

		Dim oaDMObjects(iUB) As DMCommon.ItemData
		For iIndex As Integer = 0 To iUB
			oaDMObjects(iIndex) = New DMCommon.ItemData(CleanupActionIDs(iIndex), CleanupActionNames(iIndex))
		Next
		'	MessageBox.Show("", "10_002")
		Return oaDMObjects
	End Function
	Public Shared Function CleanupActionItemsForMerge() As DMCommon.ItemData()

		Dim oaDMObjects(5) As DMCommon.ItemData

		oaDMObjects(0) = New DMCommon.ItemData(CleanupActionIDs(2), CleanupActionNames(2))
		oaDMObjects(1) = New DMCommon.ItemData(CleanupActionIDs(1), CleanupActionNames(1))
		oaDMObjects(2) = New DMCommon.ItemData(CleanupActionIDs(0), CleanupActionNames(0))

		oaDMObjects(3) = New DMCommon.ItemData(CleanupActionIDs(7), CleanupActionNames(7))
		oaDMObjects(4) = New DMCommon.ItemData(CleanupActionIDs(9), CleanupActionNames(9))
		oaDMObjects(5) = New DMCommon.ItemData(CleanupActionIDs(6), CleanupActionNames(6))
		Return oaDMObjects
	End Function
	Private Shared Sub zzCommandLine()
		Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", True, False, True)
	End Sub

	Private Shared Function zzGetSubtypeUB(ByVal iActionID As Integer) As Integer
		Select Case iActionID
			Case 1
				Return 3
			Case 4, 256, 512
				Return 2
			Case 128, 1024
				Return 0
			Case Else
				Return 1
		End Select
	End Function
	Private Shared Sub zzIdentityTopo(ByVal sResultTopoName As String)
		Dim o As ObjectDataTable = New ObjectDataTable()

		moCurrentTopoModel.Identity(moOverlayTopoModel, sResultTopoName, "")
	End Sub
	Public Shared Sub DissolveTopo(ByVal oResTopoDef As TopoDef)
		Dim oMapApp As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim iResTopDefID As DMAcadExt.TopoDefID = oResTopoDef.ID
		Dim tSrcTopDefID As DMAcadExt.TopoDefID = iResTopDefID.SourceID
		Dim tAltTopDefID As DMAcadExt.TopoDefID = tSrcTopDefID.AdditionalID
		Dim oSrcTopDef As DMAcadExt.TopoDef = New DMAcadExt.TopoDef(DMApp.AppID, tSrcTopDefID)
		Dim oAltTopDef As DMAcadExt.TopoDef = New DMAcadExt.TopoDef(DMApp.AppID, tAltTopDefID)

		Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
		Dim oSrcTopoModel As TopologyModel = Nothing
		Dim sTopologyName As String
		If TopologyExists(oAltTopDef) Then
			sTopologyName = oAltTopDef.Name
		ElseIf TopologyExists(oSrcTopDef) Then
			sTopologyName = oSrcTopDef.Name
		Else
			Return
		End If

		'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		'	AcadTransaction.Start()
		oSrcTopoModel = GetOpenedTopology(sTopologyName, Topology.OpenMode.ForRead, False, True)
		If oSrcTopoModel IsNot Nothing Then
			Dim bCurrentLayerOK As Boolean = True
			Dim oLayerDef As AcadLayerDef = New AcadLayerDef(DMApp.AppID, oResTopoDef.LinkLayers)
			If oResTopoDef.MissingCentroidLayer.Length <> 0 Then
				bCurrentLayerOK = AcadTransaction.SetCurrentLayer(oLayerDef, True, False, True, False)
				'bCurrentLayerOK = True
				'''''''''''''''''''	AcadTransaction.ClearLayerByClassName(oResTopoDef.MissingCentroidLayer, "AcDbPoint")
			End If
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(oResTopoDef.LinkLayers, oResTopoDef.LinkColorIndex)
			Try
				oSrcTopoModel.SetEdgeCreationSettings(oEdgeCreationSettings)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DissolveTopo_3")

			End Try


			Dim sTest As String = "a"
			If bCurrentLayerOK Then
				Try
					Try
						oSrcTopoModel.Dissolve(oResTopoDef.AttribExpession, oResTopoDef.Name)	'@PLAN   '.DWGNAME
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DissolveTopo_23")
					End Try

				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadDocument.WriteMessage(sTest)
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - UnionTopo_1")
				End Try
			End If
		End If
		If oSrcTopoModel IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
			If oSrcTopoModel.Status <> Status.Closed Then
				oSrcTopoModel.Close()
				DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
				oSrcTopoModel = Nothing
			End If
		End If

		DMAcadExt.AcadDocument.CloseMessage()
		'	AcadTransaction.Terminate()
		'	DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public Shared Sub DissolveTopo(ByVal sSourceTopoName As String, sAttributeExpression As String, sDestTopoName As String, Optional sBlockName As String = Nothing, Optional sBlockLayer As String = Nothing)



		Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
		Dim oSrcTopoModel As TopologyModel = Nothing
		Dim sTopologyName As String = Nothing




		'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		'	AcadTransaction.Start()
		oSrcTopoModel = GetOpenedTopology(sSourceTopoName, Topology.OpenMode.ForRead, False, True)
		If oSrcTopoModel IsNot Nothing Then
			Dim bCurrentLayerOK As Boolean = True
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings("", 0)
			'	DMCommon.Debug.MsgBox("13_020", sSourceTopoName)
			Try
				oSrcTopoModel.SetEdgeCreationSettings(oEdgeCreationSettings)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DissolveTopo_3")

			End Try

			If Not String.IsNullOrEmpty(sBlockName) AndAlso Not String.IsNullOrEmpty(sBlockLayer) Then
				Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(sBlockLayer, 256, True, sBlockName)
				oSrcTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
			End If

			Dim sTest As String = "a"
			If bCurrentLayerOK Then
				Try
					Try
						oSrcTopoModel.Dissolve(sAttributeExpression, sDestTopoName)	 '@PLAN   '.DWGNAME
					Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DissolveTopo_22", , sAttributeExpression & vbCrLf & sDestTopoName)
					End Try

				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadDocument.WriteMessage(sTest)
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - UnionTopo_1")
				End Try
			End If
		End If
		If oSrcTopoModel IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
			If oSrcTopoModel.Status <> Status.Closed Then
				oSrcTopoModel.Close()
				DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
				oSrcTopoModel = Nothing
			End If
		End If

		DMAcadExt.AcadDocument.CloseMessage()
		'	AcadTransaction.Terminate()
		'	DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public Shared Sub UnionTopo(ByVal oResTopoDef As TopoDef)
		Dim oMapApp As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApp.ActiveProject.Topologies

		Dim iResTopDefID As DMAcadExt.TopoDefID = oResTopoDef.ID
		Dim stSrcTopDefID As DMAcadExt.TopoDefID = iResTopDefID.SourceID
		Dim stOvlTopDefID As DMAcadExt.TopoDefID = iResTopDefID.OverlayID
		Dim oSrcTopDef As DMAcadExt.TopoDef = New DMAcadExt.TopoDef(DMApp.AppID, stSrcTopDefID)
		Dim oOvlTopDef As DMAcadExt.TopoDef = New TopoDef(DMApp.AppID, stOvlTopDefID)


		Dim tResultODTable As ObjectDataTable = New ObjectDataTable()

		Dim oSrcTopoModel As TopologyModel = Nothing
		Dim oOvlTopoModel As TopologyModel = Nothing

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		AcadTransaction.Start()
		oSrcTopoModel = AcadMapApp.GetTopology(oSrcTopDef.Name)
		If oSrcTopoModel IsNot Nothing Then
			oOvlTopoModel = AcadMapApp.GetTopology(oOvlTopDef.Name)
			If oOvlTopoModel IsNot Nothing Then
				Dim bCurrentLayerOK As Boolean = True
				If oResTopoDef.MissingCentroidLayer.Length <> 0 Then
					' bCurrentLayerOK = AcadTransaction.SetCurrentLayer(oResTopoDef.MissingCentroidLayer, enLayerFunction.Default, True, True, False)
					bCurrentLayerOK = True
					AcadTransaction.ClearLayerByClassName(oResTopoDef.MissingCentroidLayer, AcadConst.AcadPointName)
				End If
				Dim sTest As String = "a"
				If bCurrentLayerOK AndAlso zzRemoveODTable(oResTopoDef.ODTableName) Then
					Try
						tResultODTable.ODTableName = oResTopoDef.ODTableName
						sTest = "b"
						oSrcTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						sTest = "c"
						DMAcadExt.AcadDocument.WriteMessage("NeedsRefresh: " & CStr(oSrcTopoModel.NeedsRefresh))
						Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(oResTopoDef.MissingCentroidLayer, 0, True, "")
						sTest = "d"
						oSrcTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
						sTest = "e"
						Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(oResTopoDef.LinkLayers, 0)
						sTest = "f"
						oSrcTopoModel.SetEdgeCreationSettings(oEdgeCreationSettings)
						sTest = "g"
						Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings(oResTopoDef.MissingCentroidLayer, 0, False, "")
						sTest = "h"
						oSrcTopoModel.SetNodeCreationSettings(oNodeCreationSettings)
						sTest = "i"
						oOvlTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						sTest = "j"
						oSrcTopoModel.Union(oOvlTopoModel, oResTopoDef.Name, oResTopoDef.Description, tResultODTable)
						sTest = "k"
						tResultODTable = Nothing
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadDocument.WriteMessage(sTest)
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - UnionTopo_1")
					End Try
				End If
			End If
		End If
		If oSrcTopoModel IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
			If oSrcTopoModel.Status <> Status.Closed Then
				oSrcTopoModel.Close()
				DMAcadExt.AcadDocument.WriteMessage(oSrcTopoModel.Status.ToString())
				oSrcTopoModel = Nothing
			End If
		End If
		If oOvlTopoModel IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(oOvlTopoModel.Status.ToString())
			If oOvlTopoModel.Status <> Status.Closed Then
				oOvlTopoModel.Close()
				DMAcadExt.AcadDocument.WriteMessage(oOvlTopoModel.Status.ToString())
				oOvlTopoModel = Nothing
			End If
		End If
		DMAcadExt.AcadDocument.CloseMessage()
		AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Shared Sub zzDispArray(ByVal iaVal() As Integer)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & iaVal(iIndex).ToString()
		Next
		DMAcadExt.AcadDocument.WriteMessage(sOut, "Dissolve")
	End Sub
	Public Shared Function GetTopologyName(ByVal sPrefix As String, Optional ByVal sDelimiter As String = "") As String
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sTopoName As String = sPrefix
		Dim iCounter As Integer = 1

		Do
			If Not oTopos.Exists(sTopoName, TopologyScope.CurrentDwg) Then
				If iCounter > 1 Then
					AcadDocument.WriteMessageLog("NewTopoName= " & sTopoName)
				End If
				Return sTopoName
			Else
				sTopoName = sPrefix & sDelimiter & Convert.ToString(iCounter)
				iCounter += 1
			End If
		Loop
		'Return sTopoName
   End Function


	Public Shared Sub Rename(ByVal sOldTopoName As String, ByVal sNewTopoName As String)
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      If sOldTopoName IsNot Nothing AndAlso oTopos.Exists(sOldTopoName) Then
         Try
            oTopos.Rename(sOldTopoName, sNewTopoName)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Rename")

         End Try

      End If
   End Sub
   Public Shared Function TopoExists(sTopoName As String, oDB As Database) As String
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
		If oDB Is Nothing Then
			oProject = oMapApplication.ActiveProject
		Else
			oProject = oMapApplication.GetProjectForDB(oDB)
		End If
		Dim oTopos As Topologies = oProject.Topologies
      If oTopos Is Nothing Then
         Return "Topos Is Nothing"
      Else
         Return "Exists - " & CStr(oTopos.Exists(sTopoName)) & "; CurrentDwg - " & (oTopos.Exists(sTopoName, TopologyScope.CurrentDwg) & "; SourceDwg - " & (oTopos.Exists(sTopoName, TopologyScope.SourceDwg)).ToString())
      End If
   End Function


	Public Shared Function GetOpenedTopology(ByVal sTopoName As String, ByVal iOpenMode As Autodesk.Gis.Map.Topology.OpenMode, ByVal bValidate As Boolean, ByVal bMsgBox As Boolean, Optional ByVal bCreateObjects As Boolean = False) As TopologyModel
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
		'DMCommon.Debug.MsgBox("02_113", sTopoName, iOpenMode, bValidate, bMsgBox, bCreateObjects)
		oProject = oMapApplication.ActiveProject


		Dim oTopos As Topologies = oProject.Topologies
		Dim oTopoModel As TopologyModel
		Dim iStatus As Topology.Status
		''''''''''''''''''''''''''''''''''''  System.Windows.Forms.MessageBox.Show(CStr(oTopos Is Nothing), "05_413")
		'	Dim sMsg As String
		If sTopoName IsNot Nothing AndAlso oTopos IsNot Nothing AndAlso oTopos.Exists(sTopoName) Then
			'System.Windows.Forms.MessageBox.Show(sTopoName, "02_109R")

			oTopoModel = oTopos.Item(sTopoName)
			'System.Windows.Forms.MessageBox.Show(sTopoName, "02_112a")
			If oTopoModel IsNot Nothing Then

				'  System.Windows.Forms.MessageBox.Show(sTopoName, "02_112x")
				iStatus = oTopoModel.Status
				'DMCommon.Debug.MsgBox("02_113", iStatus, iOpenMode, bCreateObjects)
				If iStatus = Status.Closed Then
					Try
						' DMAcadExt.AcadDocument.TestAcadDoc("299a Before OpenTopo")
						If bCreateObjects Then
							oTopoModel.Open(iOpenMode, True, False)
						Else
							oTopoModel.Open(iOpenMode)
						End If


						Return oTopoModel
					Catch oMapEx As MapException
						'  DMAcadExt.AcadDocument.TestAcadDoc("299b  After OpenTopo")
						If bValidate Then
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx, bMsgBox, "Topology '" & sTopoName & "' is damaged", False, "Name=" & sTopoName & ";" & "Status=" & iStatus.ToString() & ";" & "OpenMode=" & iOpenMode.ToString())
						End If
						'  DMAcadExt.AcadDocument.TestAcadDoc("301 After Open Topo  ")
						Return Nothing
					End Try
				ElseIf iStatus = Status.OpenForRead AndAlso iOpenMode = Topology.OpenMode.ForWrite Then
					DMAcadExt.AcadDocument.WriteMessage("Topology '" & sTopoName & "' opened for read #1202")
					Return Nothing
				Else
					Return oTopoModel
				End If
			Else

				Return Nothing
			End If
		Else
			If sTopoName Is Nothing Then
				sTopoName = "Unnamed"
			End If
			If bMsgBox AndAlso oTopos IsNot Nothing Then
				DMAcadExt.AcadDocument.WriteMessageLog("!!!Topology '" & sTopoName & "' was not found")
			End If

			oTopos = Nothing
			Return Nothing
		End If
	End Function
	Public Shared Function GetTopoPolygonCount(ByVal sTopoName As String) As Integer
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim oTopoModel As TopologyModel

		If oTopos IsNot Nothing AndAlso oTopos.Exists(sTopoName) Then
			oTopoModel = oTopos.Item(sTopoName)
			Try
				oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
				Dim iPgonCount As Integer = colPolygons.Count
				colPolygons.Dispose()
				colPolygons = Nothing
				oTopoModel.Close()
				Return iPgonCount
			Catch oMapEx As MapException

				Return 0
			End Try
		Else
			Return 0
		End If





	End Function
	Public Shared Function GetOpenedTopology(ByVal tMapThemeData As DMAcadExt.MapThemeData, ByVal iOpenMode As Autodesk.Gis.Map.Topology.OpenMode) As TopologyModel
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
      Dim sTopoName As String
      oProject = oMapApplication.ActiveProject


      '''''''''''''''''  System.Windows.Forms.MessageBox.Show(CStr(oDB.NumberOfSaves), "05_411")
      '   oProject = oMapApplication.GetProjectForDB(oDB)
      ''''''''''''''''''''''  System.Windows.Forms.MessageBox.Show(CStr(oProject.MapActive), "05_412")

      Dim oTopos As Topologies = oProject.Topologies
      Dim oTopoModel As TopologyModel
      Dim iStatus As Topology.Status
      ''''''''''''''''''''''''''''''''''''  System.Windows.Forms.MessageBox.Show(CStr(oTopos Is Nothing), "05_413")
      '	Dim sMsg As String
      If oTopos IsNot Nothing AndAlso oTopos.Exists(tMapThemeData.TopoName) Then
         sTopoName = tMapThemeData.TopoName
      ElseIf oTopos.Exists(tMapThemeData.LineTopoName) Then
         sTopoName = tMapThemeData.LineTopoName
      Else
         Return Nothing
      End If

      '      System.Windows.Forms.MessageBox.Show(sTopoName, "02_109R")
      If sTopoName IsNot Nothing Then


         oTopoModel = oTopos.Item(sTopoName)
         '   System.Windows.Forms.MessageBox.Show(sTopoName, "02_112a")
         If oTopoModel IsNot Nothing Then

            '     System.Windows.Forms.MessageBox.Show(sTopoName, "02_112x")
            iStatus = oTopoModel.Status
            '  System.Windows.Forms.MessageBox.Show(iStatus.ToString(), "02_113")
            If iStatus = Status.Closed Then
               Try
                  oTopoModel.Open(iOpenMode)
                  Return oTopoModel
               Catch oMapEx As MapException
                  '  DMAcadExt.AcadDocument.TestAcadDoc("299b  After OpenTopo")

                  '  DMAcadExt.AcadDocument.TestAcadDoc("301 After Open Topo  ")
                  Return Nothing
               End Try
            ElseIf iStatus = Status.OpenForRead AndAlso iOpenMode = Topology.OpenMode.ForWrite Then
               DMAcadExt.AcadDocument.WriteMessage("Topology '" & sTopoName & "' opened for read #1202")
               Return Nothing
            Else
               Return oTopoModel
            End If
         Else
            Return Nothing
         End If
      Else
         Return Nothing
      End If
   End Function
   Public Shared Sub DeleteTopology(ByVal sTopoName As String, ByVal bDeleteEntities As Boolean, bLockDoc As Boolean, Optional bMustExists As Boolean = True)
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

      If oTopos.Exists(sTopoName) Then
         Try
            If bLockDoc Then
               DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
            End If

            oTopos.Delete(sTopoName, bDeleteEntities)
            DMAcadExt.AcadDocument.WriteMessage("Topology '" & sTopoName & "' was deleted")
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - DeleteTopology")
         Finally
            If bLockDoc Then
               DMAcadExt.AcadDocument.Unlock()
            End If
            '''''''''''''''	
         End Try
      ElseIf bMustExists Then
         MessageBox.Show("Topology '" & sTopoName & "' does not exist", "")
      End If
      '	MessageBox.Show(CStr(oTopos.Exists(sTopoName)), "OK !!!+" & sTopoName)
   End Sub
   Public Delegate Function TopoNameCriteria(ByVal sName As String) As Boolean
	Public Shared Sub DeleteTopologies(ByVal dlTopoNameCriteria As TopoNameCriteria)
		Const sCentroidTopoNamePrefix As String = "TPMCNTR_"
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
		Dim oODTables As Autodesk.Gis.Map.ObjectData.Tables = oProject.ODTables
		Dim colStrings As System.Collections.Specialized.StringCollection = oODTables.GetTableNames()
		Dim iPrefixLen As Integer = sCentroidTopoNamePrefix.Length
		Dim sTopoName As String
		Dim oTopos As Topologies = oProject.Topologies
		For Each sName As String In colStrings
			'	AcadDocument.WriteMessage("&& " & sName)
			If sName.StartsWith(sCentroidTopoNamePrefix) Then
				sTopoName = sName.Substring(iPrefixLen)
				'	AcadDocument.WriteMessage("** " & sTopoName)
				If oTopos.Exists(sTopoName) AndAlso dlTopoNameCriteria(sTopoName) Then
					'	AcadDocument.WriteMessage("XX " & sTopoName)
					Try
						oTopos.Delete(sTopoName, False)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - DeleteTopologies_2", sTopoName)
					End Try
				End If
			End If
		Next
	End Sub
	Public Shared Function TopologiesExist(ByVal dlTopoNameCriteria As TopoNameCriteria) As Boolean
		Const sCentroidTopoNamePrefix As String = "TPMCNTR_"
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
		Dim oODTables As Autodesk.Gis.Map.ObjectData.Tables = oProject.ODTables
		Dim colStrings As System.Collections.Specialized.StringCollection = oODTables.GetTableNames()
		Dim iPrefixLen As Integer = sCentroidTopoNamePrefix.Length
		Dim sTopoName As String
		Dim oTopos As Topologies = oProject.Topologies
		For Each sName As String In colStrings
			'	AcadDocument.WriteMessage("&& " & sName)
			If sName.StartsWith(sCentroidTopoNamePrefix) Then
				sTopoName = sName.Substring(iPrefixLen)
				'	AcadDocument.WriteMessage("** " & sTopoName)
				If oTopos.Exists(sTopoName) AndAlso dlTopoNameCriteria(sTopoName) Then
					'AcadDocument.WriteMessage("XX " & sTopoName)
					Try
						'	DMCommon.Debug.MsgBox("13_001g", sTopoName)
						Return True
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - DeleteTopologies_2", sTopoName)
					End Try
				End If
			End If
		Next
		Return False
	End Function
	Public Shared Function CheckTopo(ByVal sTopoName As String, bLockDoc As Boolean, bMsgBox As Boolean, iMapThemeID As DMAcadExt.enMapTheme, Optional ByVal colCentroidBlockRefs As ObjectIdCollection = Nothing, Optional ByVal colNodeBlockRefs As ObjectIdCollection = Nothing) As TopoRes  '
		'    Dim colCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(sCentroidBlocks, sCentroidLayers)
		Dim sMsg As String
		Dim iMessageBoxIcon As MessageBoxIcon
		Dim tTopoRes As TopoRes
		Dim bNeedRefresh As Boolean
		Dim tCentroidObjID As ObjectId
		'	DMCommon.Debug.MsgBox("13_001i", sTopoName, bMsgBox, iMapThemeID, colCentroidBlockRefs.Count)
		If sTopoName IsNot Nothing AndAlso TopoCreator.TopologyExists(sTopoName) Then
			tTopoRes = New TopoRes(True)
			sMsg = "Topology '" & sTopoName & "' is "
			If bLockDoc Then
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
			End If

			'     DMAcadExt.AcadDocument.TestAcadDoc("299 Before OpenTopo")
			Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Topo01", oTopoModel IsNot Nothing, tTopoRes.IsOK, tTopoRes.IsTopoOK, sMsg)
			If oTopoModel IsNot Nothing Then
				' MessageBox.Show(sMsg & " Not Nothing" & vbCrLf & oTopoModel.IsComplete & vbCrLf & CStr(oTopoModel.NeedsRefresh) & vbCrLf & oTopoModel.Status.ToString(), "05_455")

				tTopoRes.CanOpen = True
				tTopoRes.HasElements = False
				Try
					tTopoRes.IsCorrect = Not oTopoModel.NeedsRefresh
					Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
					Dim oDBObject As DBObject
					Dim iPointCentroidCount As Integer = 0
					Dim iPointNodeCount As Integer = 0
					bNeedRefresh = oTopoModel.NeedsRefresh
					DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 1)
					'  DMCommon.Debug.MsgBox("12_725p", iMapThemeID, colPolygons.Count)
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						Try
							tCentroidObjID = oPolygon.Entity
							oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							If oDBObject Is Nothing Then

								DMCommon.Debug.MsgBox("13_109", oDBObject Is Nothing)
							ElseIf oDBObject.GetRXClass.Name = DMAcadExt.AcadConst.AcadPointName Then
								DMAcadExt.AppMessages.AddMessage(True, oPolygon.Centroid.X, oPolygon.Centroid.Y, "", DMCommon.dmMessages.Message(306), False, iMapThemeID, 1)
								iPointCentroidCount += 1
							ElseIf oDBObject.GetRXClass.Name = DMAcadExt.AcadConst.AcadBlockRefName Then
								If colCentroidBlockRefs IsNot Nothing AndAlso colCentroidBlockRefs.Contains(oDBObject.ObjectId) Then
									colCentroidBlockRefs.Remove(oDBObject.ObjectId)
								End If
							End If
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - CheckTopo_1", sTopoName, oPolygon.ID)
						End Try

						oPolygon.Dispose()
						oPolygon = Nothing
					Next
					Dim iPgonCount As Integer = colPolygons.Count
					colPolygons.Dispose()
					colPolygons = Nothing
					Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
					DMAcadExt.AppMessages.ClearMessages(iMapThemeID, 2)
					tTopoRes.PgonCount = iPgonCount
					tTopoRes.MissingCntrCount = iPointCentroidCount
					'	DMCommon.Debug.MsgBox("13_108", tTopoRes.MissingCntrCount)
					If colCentroidBlockRefs IsNot Nothing Then
						For Each tAcObjID As ObjectId In colCentroidBlockRefs
							tPoint = AcadTransaction.GetBlockRefInsPoint(tAcObjID)
							DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", DMCommon.dmMessages.Message(307), False, iMapThemeID, 2)
						Next
						tTopoRes.OutsideCntrCount = colCentroidBlockRefs.Count
					End If
					If colNodeBlockRefs IsNot Nothing Then
						Dim colNodes As Autodesk.Gis.Map.Topology.NodeCollection = oTopoModel.GetNodes()
						For Each oNode As Node In colNodes
							oDBObject = DMAcadExt.AcadTransaction.GetDBObject(oNode.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							If oDBObject.GetRXClass.Name = DMAcadExt.AcadConst.AcadPointName Then
								DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.X, "", DMCommon.dmMessages.Message(320), False, iMapThemeID, 1)
								iPointNodeCount += 1
							ElseIf oDBObject.GetRXClass.Name = DMAcadExt.AcadConst.AcadBlockRefName Then
								If colNodeBlockRefs IsNot Nothing AndAlso colNodeBlockRefs.Contains(oDBObject.ObjectId) Then
									colNodeBlockRefs.Remove(oDBObject.ObjectId)
								End If
							End If
						Next
						tTopoRes.NodeCount = iPointNodeCount
					End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Topo02", oTopoModel IsNot Nothing, tTopoRes.IsOK, tTopoRes.IsTopoOK, tTopoRes.PgonCount, sMsg)
					If iPgonCount = 0 Then
						sMsg &= "empty"
					Else
						If oTopoModel.NeedsRefresh Then
							sMsg &= "incorrect"
						Else
							sMsg &= "correct"
						End If

						If oTopoModel.IsComplete Then
							sMsg &= " And complete"
							tTopoRes.IsComplete = True
							If iPgonCount = 1 Then
								sMsg &= "," & vbCrLf & "contains one polygon"
							ElseIf iPgonCount > 1 Then
								sMsg &= "," & vbCrLf & "contains " & CStr(iPgonCount) & " polygons"
							End If
							iMessageBoxIcon = MessageBoxIcon.Information
						Else
							sMsg &= " and incomplete"
							iMessageBoxIcon = MessageBoxIcon.Error
						End If
					End If

					oTopoModel.Close()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Topo03", tTopoRes.IsOK, tTopoRes.IsTopoOK, tTopoRes.PgonCount, sMsg)
				Catch oEx As Exception
					sMsg &= "incorrect"
					iMessageBoxIcon = MessageBoxIcon.Error
					DMCommon.Debug.MsgBox("12_290s", sTopoName, oEx.Message, oEx.StackTrace)
				End Try
				' MessageBox.Show(sTopoName & vbCrLf & oTopoModel.Status.ToString() & vbCrLf & CStr(DMAcadExt.AcadDocument.IsLocked) & vbCrLf & bLockDoc.ToString(), "03_188x")

			Else
				sMsg &= "incorrect!"
				iMessageBoxIcon = MessageBoxIcon.Error
				' MessageBox.Show(sMsg & " nothing", "05_479x")
			End If
			If bLockDoc Then
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
			End If
			sMsg = sMsg & vbCrLf & bNeedRefresh.ToString
			If bMsgBox Then
				MessageBox.Show(sMsg, "Topology creator", MessageBoxButtons.OK, iMessageBoxIcon, MessageBoxDefaultButton.Button1)
			End If
		Else

			tTopoRes = New TopoRes(False)


		End If
		'DMCommon.Debug.MsgBox("13_001k", sTopoName, tTopoRes.IsComplete, tTopoRes.IsCorrect, tTopoRes.IsOK, tTopoRes.IsTopoOK, tTopoRes.PgonCount)
		Return tTopoRes
	End Function
	Public Shared Function CheckMPgons(ByVal sParcelCPLayer As String, bStartTransaction As Boolean) As DMAcadExt.TopoRes
      Dim tTopoRes As DMAcadExt.TopoRes
      If bStartTransaction Then

         DMAcadExt.AcadTransaction.Start()
      End If

      Dim oList As IList(Of Autodesk.AutoCAD.DatabaseServices.Entity) = DMAcadExt.AcadTransaction.GetMPolygons(sParcelCPLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      If oList IsNot Nothing Then
         'MessageBox.Show(":" & sParcelCPLayer & ":" & vbCrLf & CStr(oList.Count), "06_777")
         tTopoRes = New DMAcadExt.TopoRes(True)
         tTopoRes.PgonCount = oList.Count
         tTopoRes.IsCorrect = True
         tTopoRes.IsComplete = True
      Else
         tTopoRes = New DMAcadExt.TopoRes(False)
      End If

      If bStartTransaction Then
         DMAcadExt.AcadTransaction.Terminate()
      End If
      Return tTopoRes
   End Function
   Public Shared Sub DissolveMy(ByVal oSourceTopoModel As TopologyModel, ByVal iaTopoIDs() As Integer, ByVal sDissolveTopoName As String)
      Dim sPgonsTopoName As String = "tmpPgons" & CStr(iaTopoIDs(0))
      Dim oPgon As Polygon
      Dim tCentroid As ObjectId
      Dim colLines As ObjectIdCollection = New ObjectIdCollection()
      Dim colRings As RingCollection
      Dim oFullEdge As FullEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
      Dim oTopoModel As TopologyModel = Nothing
      '	Dim oTestEnt As Entity
      tResultODTable.ODTableName = "AAAA"
      If oSourceTopoModel IsNot Nothing AndAlso oSourceTopoModel.Status <> Status.Closed Then
         DMAcadExt.AcadDocument.WriteMessage("##40 " & sDissolveTopoName & ":" & sPgonsTopoName)
         For iIndex As Integer = 0 To iaTopoIDs.GetUpperBound(0)
            Try
               oPgon = oSourceTopoModel.GetPolygon(iaTopoIDs(iIndex))
               tCentroid = oPgon.Entity
               colCentroids.Add(tCentroid)
               DMAcadExt.AcadDocument.WriteMessage("##43 " & CStr(iaTopoIDs(iIndex)))
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_2")
               oPgon = Nothing
               MessageBox.Show(CStr(iaTopoIDs(iIndex)) & vbNewLine & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), "12_842")
					DMCommon.Functions.DispArray("iaTopoIDs_328", iaTopoIDs)
				End Try
            If oPgon IsNot Nothing Then
               Try
                  colRings = oPgon.GetBoundary()
                  DMAcadExt.AcadDocument.WriteMessage("##45 " & CStr(colRings.Count))
                  For Each oRing As Ring In colRings
                     colHalfEdges = oRing.GetEdges()

                     For Each oHalfEdge As HalfEdge In colHalfEdges
                        oFullEdge = oHalfEdge.FullEdge
                        tAcObjID = oFullEdge.Entity
                        DMAcadExt.AcadDocument.WriteMessage("##49 " & tAcObjID.ToString())
                        If Not colLines.Contains(tAcObjID) Then
                           DMAcadExt.AcadDocument.WriteMessage("##50 " & tAcObjID.ToString())
                           '	oTestEnt = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           '	oTestEnt.ColorIndex = 5
                           colLines.Add(tAcObjID)
                        Else
                           colLines.Remove(tAcObjID)
                           If colCentroids.Contains(tCentroid) Then
                              colCentroids.Remove(tCentroid)
                           End If

                        End If
                     Next
                  Next
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_4")
               End Try
            End If
         Next

         Try
            '	MessageBox.Show(sPgonsTopoName & ":" & CStr(colLines.Count) & ":" & CStr(colNodes.Count) & ":" & CStr(colCentroids.Count), "15_123")
            oTopos.Create(sDissolveTopoName, colLines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_4a")
         End Try
         If oTopos.Exists(sPgonsTopoName) And False Then
            If Not oTopos.Exists(sDissolveTopoName) Then
               Try
                  oTopoModel = oTopos.Item(sPgonsTopoName)
                  oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
                  '	MessageBox.Show(oTopoModel.Status.ToString() & ":" & sDissolveTopoName, "15_129")

                  oTopoModel.Dissolve(".Layer", sDissolveTopoName)
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "TopoCreator - Dissolve_5")
               End Try
               Try
                  If oTopoModel IsNot Nothing AndAlso oTopoModel.Status <> Status.Closed Then
                     oTopoModel.Close()
                  End If
                  If oTopos.Exists(sPgonsTopoName) Then
                     oTopos.Delete(sPgonsTopoName, True)
                  End If
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  AcadErrCode.ShowMapError(oMapEx, True, "")
               End Try
            Else
               DMAcadExt.AcadDocument.WriteMessage("Topology '" & sDissolveTopoName & "' already exists")
            End If
         Else
            DMAcadExt.AcadDocument.WriteMessage("Topology '" & sPgonsTopoName & "' was not found #2")
         End If
      End If
   End Sub
   Public Shared Sub Dissolve(ByVal oSourceTopoModel As TopologyModel, ByVal iaTopoIDs() As Integer, ByVal sDissolveTopoName As String)
      Dim sPgonsTopoName As String = "tmpPgons" & CStr(iaTopoIDs(0))
      Dim oPgon As Polygon
      Dim colLines As ObjectIdCollection = New ObjectIdCollection()
      Dim colRings As RingCollection
      Dim oFullEdge As FullEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
      Dim oTopoModel As TopologyModel = Nothing
      Dim oTestEnt As Entity
      tResultODTable.ODTableName = "AAAA"
      If oSourceTopoModel IsNot Nothing AndAlso oSourceTopoModel.Status <> Status.Closed Then
         DMAcadExt.AcadDocument.WriteMessage("##40 " & sDissolveTopoName & ":" & sPgonsTopoName)
         For iIndex As Integer = 0 To iaTopoIDs.GetUpperBound(0)
            Try
               oPgon = oSourceTopoModel.GetPolygon(iaTopoIDs(iIndex))
               DMAcadExt.AcadDocument.WriteMessage("##43 " & CStr(iaTopoIDs(iIndex)))
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_2")
               oPgon = Nothing
               MessageBox.Show(CStr(iaTopoIDs(iIndex)) & vbNewLine & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), "12_842")
					DMCommon.Functions.DispArray("iaTopoIDs_328", iaTopoIDs)
				End Try
            If oPgon IsNot Nothing Then
               Try
                  colRings = oPgon.GetBoundary()
                  DMAcadExt.AcadDocument.WriteMessage("##45 " & CStr(colRings.Count))
                  For Each oRing As Ring In colRings
                     colHalfEdges = oRing.GetEdges()
                     For Each oHalfEdge As HalfEdge In colHalfEdges
                        oFullEdge = oHalfEdge.FullEdge
                        tAcObjID = oFullEdge.Entity
                        DMAcadExt.AcadDocument.WriteMessage("##49 " & tAcObjID.ToString())
                        If Not colLines.Contains(tAcObjID) Then
                           DMAcadExt.AcadDocument.WriteMessage("##50 " & tAcObjID.ToString())
                           oTestEnt = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           oTestEnt.ColorIndex = 5
                           colLines.Add(tAcObjID)
                        End If
                     Next
                  Next
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_4")
               End Try
            End If
         Next

         Try
            'MessageBox.Show(sPgonsTopoName & ":" & CStr(colLines.Count) & ":" & CStr(colNodes.Count) & ":" & CStr(colCentroids.Count), "15_125")
            oTopos.Create(sPgonsTopoName, colLines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_4a")
         End Try
         If oTopos.Exists(sPgonsTopoName) Then
            If Not oTopos.Exists(sDissolveTopoName) Then
               Try
                  oTopoModel = oTopos.Item(sPgonsTopoName)
                  oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
                  '	MessageBox.Show(oTopoModel.Status.ToString() & ":" & sDissolveTopoName, "15_129")

                  oTopoModel.Dissolve(".Layer", sDissolveTopoName)
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "TopoCreator - Dissolve_5")
               End Try
               Try
                  If oTopoModel IsNot Nothing AndAlso oTopoModel.Status <> Status.Closed Then
                     oTopoModel.Close()
                  End If
                  If oTopos.Exists(sPgonsTopoName) Then
                     oTopos.Delete(sPgonsTopoName, True)
                  End If
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  AcadErrCode.ShowMapError(oMapEx, True, "")
               End Try
            Else
               DMAcadExt.AcadDocument.WriteMessage("Topology '" & sDissolveTopoName & "' already exists")
            End If
         Else
            DMAcadExt.AcadDocument.WriteMessage("Topology '" & sPgonsTopoName & "' was not found #2")
         End If
      End If
   End Sub

   Public Shared Sub DissolveByGroup1(ByVal oSourceTopoModel As TopologyModel, ByVal dicTopoIDGroup As Dictionary(Of Integer, Integer), ByVal sDissolveTopoName As String)
      '	Dim sPgonsTopoName As String = "tmpPgons" & CStr(iaTopoIDs(0))
      Dim oPgon As Polygon
      '	Dim tCentroid As ObjectId
      Dim colLines As ObjectIdCollection = New ObjectIdCollection()
      Dim colRings As RingCollection
      Dim oFullEdge As FullEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
      Dim oTopoModel As TopologyModel = Nothing
      '	Dim oTestEnt As Entity
      tResultODTable.ODTableName = "AAAA"
      Dim iCurrentPgonID As Integer
      Dim iCurrentGroup As Integer
      Dim oLeftHalfEdge As HalfEdge
      '	Dim oRightHalfEdge As HalfEdge
      Dim oOtherHalfEdge As HalfEdge
      Dim iOtherPgonID As Integer
      Dim iOtherGroup As Integer
      Dim oOtherPgon As Polygon

      Dim colKeys As System.Collections.Generic.Dictionary(Of Integer, Integer).KeyCollection = dicTopoIDGroup.Keys
      If oSourceTopoModel IsNot Nothing AndAlso oSourceTopoModel.Status <> Status.Closed Then
         For Each iSourceTopoID As Integer In colKeys
            '	For iIndex As Integer = 0 To iaTopoIDs.GetUpperBound(0)
            Try
               oPgon = oSourceTopoModel.GetPolygon(iSourceTopoID)
               '	tCentroid = oPgon.Entity
               '	colCentroids.Add(tCentroid)
               'DMAcadExt.AcadDocument.WriteMessage("##43 " & CStr(iTopoID))
            Catch oMapEx As Autodesk.Gis.Map.MapException
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_2")
               oPgon = Nothing
               MessageBox.Show(CStr(iSourceTopoID) & vbNewLine & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), "12_842")
               '	DMCommon.Functions.DispArray(iaTopoIDs, "iaTopoIDs_328")
            End Try
            If oPgon IsNot Nothing Then
               Try
                  iCurrentPgonID = oPgon.ID
                  iCurrentGroup = dicTopoIDGroup.Item(iCurrentPgonID)
                  colRings = oPgon.GetBoundary()
                  'DMAcadExt.AcadDocument.WriteMessage("##45 " & CStr(colRings.Count))
                  For Each oRing As Ring In colRings
                     colHalfEdges = oRing.GetEdges()
                     For Each oHalfEdge As HalfEdge In colHalfEdges

                        oFullEdge = oHalfEdge.FullEdge


                        tAcObjID = oFullEdge.Entity
                        'DMAcadExt.AcadDocument.WriteMessage("##49 " & tAcObjID.ToString())

                        If Not colLines.Contains(tAcObjID) Then
                           '	DMAcadExt.AcadDocument.WriteMessage("##50 " & tAcObjID.ToString())
                           '	oTestEnt = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                           '	oTestEnt.ColorIndex = 5
                           oLeftHalfEdge = oFullEdge.GetHalfEdge(True)
                           '	oRightHalfEdge = 
                           If oLeftHalfEdge IsNot Nothing Then
                              If oLeftHalfEdge Is oHalfEdge Then
                                 oOtherHalfEdge = oFullEdge.GetHalfEdge(False)
                              Else
                                 oOtherHalfEdge = oLeftHalfEdge
                              End If
                              If oOtherHalfEdge IsNot Nothing Then
                                 Try
                                    oOtherPgon = oOtherHalfEdge.Polygon
                                    iOtherPgonID = oOtherPgon.ID
                                    If dicTopoIDGroup.TryGetValue(iOtherPgonID, iOtherGroup) Then
                                       If iOtherGroup <> iCurrentGroup Then
                                          colLines.Add(tAcObjID)
                                       End If
                                    Else
                                       colLines.Add(tAcObjID)
                                    End If
                                 Catch oMapEx As Autodesk.Gis.Map.MapException
                                    colLines.Add(tAcObjID)
                                 End Try


                              Else
                                 colLines.Add(tAcObjID)
                              End If

                           Else
                              colLines.Add(tAcObjID)
                           End If




                        End If
                     Next
                  Next
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_14")
               End Try
            End If
         Next

         Try
            MessageBox.Show(CStr(colLines.Count) & ":" & CStr(colNodes.Count) & ":" & CStr(colCentroids.Count), "15_125")
            oTopos.Create(sDissolveTopoName, colLines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_14b")
         End Try

      End If





   End Sub
   Public Shared Function GetTopoLinks(ByVal sTopoName As String) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colLinks As ObjectIdCollection
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim oTopoModel As TopologyModel
      Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection

      If oTopos.Exists(sTopoName) Then
         oTopoModel = oTopos(sTopoName)
         Try
            oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
            colPolygons = oTopoModel.GetPolygons()
            colLinks = zzGetTopoLinks(colPolygons, False)
            colLinks.Dispose()
            oTopoModel.Close()
         Catch oMapEx As Autodesk.Gis.Map.MapException
            AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "GetTopoLinks" & vbCrLf & oMapEx.StackTrace)
            oTopoModel.Close()
            colLinks = New ObjectIdCollection
         End Try
      Else
         colLinks = New ObjectIdCollection
      End If
      Return colLinks
   End Function

   Public Shared Function DeleteTopoLinks(ByVal sTopoName As String) As Integer
      Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = GetTopoLinks(sTopoName)
      Dim iLinkCount As Integer = colLinks.Count
      TopoCreator.DeleteTopology(sTopoName, False, False)
      DMAcadExt.AcadTransaction.EraseDBObjects(colLinks)
      Return iLinkCount
   End Function

   Public Shared Sub TopoToClosedPgons(sTopoName As String, bExteriorRingOnly As Boolean)

      '	Dim oTopoDef As DMAcadExt.TopoDef = zzLoadTopoDef()
      Dim oColorPgon As ColorPolygon

      Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
      If oTopoModel IsNot Nothing Then
         Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
         For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
            oColorPgon = New ColorPolygon(oPolygon)
            oColorPgon.CreateClosedPolygon(bExteriorRingOnly)
            oColorPgon.Terminate()
            oPolygon.Dispose()
            oPolygon = Nothing
         Next
         colPolygons.Dispose()
         colPolygons = Nothing
         oTopoModel.Close()

      End If
   End Sub
   Public Shared Sub TestTrace(ByVal sTopoName As String, ByVal iPgonTopoID As Integer)
      Dim oTopoModel As TopologyModel = GetOpenedTopology(sTopoName, Topology.OpenMode.ForRead, False, True)
      Dim oPolygon As Polygon = Nothing
      Dim colRings As RingCollection
      Dim sTest As String = "a"
      If oTopoModel Is Nothing Then Return
      If iPgonTopoID = 0 Then
         Dim colPolygons As PolygonCollection = oTopoModel.GetPolygons()
         For Each oPolygon In colPolygons
            Exit For
         Next
      Else
         oPolygon = oTopoModel.GetPolygon(iPgonTopoID)
      End If
      If oPolygon IsNot Nothing Then
         sTest = "cb"
         colRings = oPolygon.GetBoundary()
         sTest = "d"


         If colRings IsNot Nothing Then
            sTest = "e"
            ''	DMAcadExt.AcadDocument.WriteMessage("Rings=" & CStr(mcolRings.Count) & " PgonId=" & CStr(diTopoID) & " bPositiveRing=" & CStr(bPositiveRing))
            For Each oRing As Ring In colRings
               zzTestRingB(oRing)
               sTest = "g"
               '	DMAcadExt.AcadDocument.WriteMessage("-- - --ENDOF - --PgonId=" & CStr(diTopoID))
            Next oRing
         End If
      End If
      oTopoModel.Close()
      oTopoModel = Nothing
   End Sub
   Private Shared Function zzGetTopoLinks(ByVal colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection, ByVal bCheckTopo As Boolean) As ObjectIdCollection
      Const sCheckPgonName As String = "TestPgonTopo"
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim colPgonLinks As ObjectIdCollection = New ObjectIdCollection()
      Dim colPgonNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colPgonCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
      Dim colRings As RingCollection
      Dim colHalfEdges As HalfEdgeCollection
      Dim oFullEdge As FullEdge
      Dim tAcObjID As ObjectId
      Dim sTestTopoName As String
      Dim iTopoPgonErrors As Integer = 0

      Dim iPgonIndex As Integer = 0

      Try
         For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
            iPgonIndex += 1

            '	AcadDocument.WriteMessageLog("Pgon: #" & CStr(iPgonIndex) & " " & CStr(bTemp) & " - " & TPlnPoint.DispPoint(oPolygon.Centroid))
            If bCheckTopo Then
               colPgonCentroids.Add(oPolygon.Entity)
            End If
            colRings = oPolygon.GetBoundary()
            For Each oRing As Ring In colRings
               colHalfEdges = oRing.GetEdges()
               For Each oHalfEdge As HalfEdge In colHalfEdges
                  oFullEdge = oHalfEdge.FullEdge
                  tAcObjID = oFullEdge.Entity
                  If bCheckTopo Then
                     colPgonLinks.Add(tAcObjID)
                  End If
                  If Not colLinks.Contains(tAcObjID) Then
                     colLinks.Add(tAcObjID)
                  End If
               Next
            Next
            If bCheckTopo Then
               sTestTopoName = GetTopologyName(sCheckPgonName)
               Try
                  oTopos.Create(sTestTopoName, colPgonLinks, colPgonNodes, colPgonCentroids, TopologyTypes.Polygon, CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors, 0.1)
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  AcadDocument.WriteMessageLog("Error: #" & CStr(iPgonIndex))
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "TopoCreator - zzGetTopoLinks")
               End Try
               If oTopos.Exists(sTestTopoName) Then
                  oTopos.Delete(sTestTopoName, False)
               Else
                  DMAcadExt.AppMessages.AddMessage(True, New TPlnPoint(oPolygon.Centroid), "", "Polygon problems", False)
                  iTopoPgonErrors += 1
               End If
               colPgonLinks.Clear()
               colPgonCentroids.Clear()
            End If
         Next
         colPgonLinks.Dispose()
         colPgonLinks = Nothing
         colPgonCentroids.Dispose()
         colPgonCentroids = Nothing
         If bCheckTopo Then
            If iTopoPgonErrors = 0 Then
               AcadDocument.WriteMessage("Any polygon has no topology problems")
            ElseIf iTopoPgonErrors = 0 Then
               AcadDocument.WriteMessage("A polygon has topology problems")
            Else
               AcadDocument.WriteMessage(CStr(iTopoPgonErrors) & " polygons have topology problems")
            End If
         End If

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TopoCreator - zzGetTopoLinks_9")
      End Try
      Return colLinks
   End Function


   Private Shared Sub zzTestRingA(ByVal oRing As Ring)

      Dim oFullEdge As FullEdge
      Dim oTryFullEdge As FullEdge

      Dim oStartHalfEdge As HalfEdge
      Dim oHalfEdge As HalfEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection

      Dim colLines As ObjectIdCollection = New ObjectIdCollection()
      Dim dicHalfEdges As Dictionary(Of Integer, HalfEdge) = New Dictionary(Of Integer, HalfEdge)
      colHalfEdges = oRing.GetEdges()
      AcadDocument.WriteMessage("colHalfEdges: " & colHalfEdges.Count.ToString())
      For Each oHEdge As HalfEdge In colHalfEdges
         oFullEdge = oHEdge.FullEdge
         tAcObjID = oFullEdge.Entity
         colLines.Add(tAcObjID)
         dicHalfEdges.Add(oHEdge.PreviousNode.ID, oHEdge)
      Next
      AcadDocument.WriteMessage(" colLines: " & colLines.Count.ToString())
      Dim oNode As Node
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim oTryTrueHalfEdge As HalfEdge
      Dim oTryFalseHalfEdge As HalfEdge
      Dim iTrueNext As Integer = 0
      Dim iFalseNext As Integer = 0
      Dim iMidNext As Integer = 0
      Dim iStartID, iEndID As Integer
      Dim sMsg As String
      oHalfEdge = oRing.StartEdge    'START
      oStartHalfEdge = oHalfEdge
      oFullEdge = oHalfEdge.FullEdge

      oNode = oHalfEdge.PreviousNode
      iStartID = oNode.ID
      AcadDocument.WriteMessage("Start=" & CStr(oFullEdge.ID) & ", " & CStr(iStartID) & "-->" & CStr(oHalfEdge.NextNode.ID))
      For iIndex As Integer = 0 To colHalfEdges.Count - 1
         Try
            tAcObjID = oFullEdge.Entity
            tPoint = zzPoint3dTo2d(oNode.Location)

            '	AcadDocument.WriteMessage(tAcObjID.ToString & "-" & TPlnPoint.DispPoint(tPoint))
            oNode = oHalfEdge.NextNode
            oTryTrueHalfEdge = oHalfEdge.GetNextEdge(True)
            oTryFalseHalfEdge = oHalfEdge.GetNextEdge(False)
            If oTryTrueHalfEdge Is Nothing Then
               sMsg = "oTryTrueHalfEdge Is Nothing,"
               'AcadDocument.WriteMessage("oTryTrueHalfEdge Is Nothing")
            Else
               oTryFullEdge = oTryTrueHalfEdge.FullEdge
               sMsg = "oTryTrueHalfEdge=" & CStr(oTryFullEdge.ID) & ","
               'AcadDocument.WriteMessage("oTryTrueHalfEdge=" & CStr(oTryFullEdge.ID))
            End If
            If oTryFalseHalfEdge Is Nothing Then
               sMsg &= "TryFalseHalfEdge Is Nothing"
               'AcadDocument.WriteMessage("TryFalseHalfEdge Is Nothing")
            Else
               oTryFullEdge = oTryFalseHalfEdge.FullEdge
               sMsg &= "TryFalseHalfEdge=" & CStr(oTryFullEdge.ID)
               'AcadDocument.WriteMessage("TryFalseHalfEdge=" & CStr(oTryFullEdge.ID))
            End If
            '' next  HalfEdge
            AcadDocument.WriteMessage(sMsg)
            oHalfEdge = dicHalfEdges.Item(oNode.ID)
            oFullEdge = oHalfEdge.FullEdge

            If oTryTrueHalfEdge.FullEdge.ID = oFullEdge.ID Then
               iTrueNext += 1
            ElseIf oTryFalseHalfEdge.FullEdge.ID = oFullEdge.ID Then
               iFalseNext += 1
            Else
               iMidNext += 1
            End If

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "TopoCreator - zzTestRing")
         End Try
      Next
      iEndID = oNode.ID


      '	AcadDocument.WriteMessage("Start-->End:" & CStr(iStartID) & "-->" & CStr(iEndID))
      If iMidNext <> 0 Then
         AcadDocument.WriteMessage("Dir is not valid(+-) - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      ElseIf iTrueNext <> 0 AndAlso iFalseNext <> 0 Then
         AcadDocument.WriteMessage("Dir is Mix (+-) - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      Else
         AcadDocument.WriteMessage("OK!!! - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      End If

   End Sub
   Private Shared Sub zzTestRingB(ByVal oRing As Ring)

      Dim oFullEdge As FullEdge
      Dim oTryFullEdge As FullEdge

      Dim oStartHalfEdge As HalfEdge
      Dim oHalfEdge As HalfEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection

      Dim colLines As ObjectIdCollection = New ObjectIdCollection()
      Dim dicHalfEdges As Dictionary(Of Integer, HalfEdge) = New Dictionary(Of Integer, HalfEdge)
      colHalfEdges = oRing.GetEdges()
      AcadDocument.WriteMessage("colHalfEdges: " & colHalfEdges.Count.ToString())
      For Each oHEdge As HalfEdge In colHalfEdges
         oFullEdge = oHEdge.FullEdge
         tAcObjID = oFullEdge.Entity
         colLines.Add(tAcObjID)
         dicHalfEdges.Add(oHEdge.PreviousNode.ID, oHEdge)
      Next
      AcadDocument.WriteMessage(" colLines: " & colLines.Count.ToString())
      Dim oNode As Node
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
      Dim oTryTrueHalfEdge As HalfEdge
      Dim oTryFalseHalfEdge As HalfEdge
      Dim iTrueNext As Integer = 0
      Dim iFalseNext As Integer = 0
      Dim iMidNext As Integer = 0
      Dim iStartID, iEndID As Integer
      Dim sMsg As String
      Dim oPgon As Polygon
      Dim oTryTopo As TopologyModel = Nothing


      oHalfEdge = oRing.StartEdge    'START
      oStartHalfEdge = oHalfEdge
      oFullEdge = oHalfEdge.FullEdge

      oNode = oHalfEdge.PreviousNode
      iStartID = oNode.ID
      AcadDocument.WriteMessage("Start=" & CStr(oFullEdge.ID) & ", " & CStr(iStartID) & "-->" & CStr(oHalfEdge.NextNode.ID))
      For iIndex As Integer = 0 To colHalfEdges.Count - 1
         Try
            tAcObjID = oFullEdge.Entity
            oTryTrueHalfEdge = oFullEdge.GetHalfEdge(True)
            oTryFalseHalfEdge = oFullEdge.GetHalfEdge(False)
            If oTryTrueHalfEdge Is Nothing Then
               sMsg = "Full:oTryTrueHalfEdge Is Nothing,"
               'AcadDocument.WriteMessage("oTryTrueHalfEdge Is Nothing")
            Else
               Try
                  oPgon = oTryTrueHalfEdge.Polygon
                  If oPgon IsNot Nothing Then
                     sMsg = "TruePgon=" & CStr(oPgon.ID)
                  Else
                     sMsg = "*TruePgon=" & CStr(0)
                  End If
               Catch oMapEx As MapException
                  '	sMsg = oMapEx.Message
                  sMsg = "TruePgon=" & CStr(0)

               End Try
               AcadDocument.WriteMessage(sMsg)
            End If
            If oTryFalseHalfEdge Is Nothing Then
               sMsg = "Full:oTryFalseHalfEdge Is Nothing,"
               'AcadDocument.WriteMessage("oTryTrueHalfEdge Is Nothing")
            Else

               Try
                  oPgon = oTryFalseHalfEdge.Polygon
                  If oPgon IsNot Nothing Then
                     sMsg = "FalsePgon=" & CStr(oPgon.ID)
                  Else
                     sMsg = "*FalsePgon=" & CStr(0)
                  End If
               Catch oMapEx As MapException
                  sMsg = "FalsePgon=" & CStr(0)

               End Try
            End If

            AcadDocument.WriteMessage(sMsg)
            tPoint = zzPoint3dTo2d(oNode.Location)

            '	AcadDocument.WriteMessage(tAcObjID.ToString & "-" & TPlnPoint.DispPoint(tPoint))
            oNode = oHalfEdge.NextNode
            oTryTrueHalfEdge = oHalfEdge.GetNextEdge(True)
            oTryFalseHalfEdge = oHalfEdge.GetNextEdge(False)
            If oTryTrueHalfEdge Is Nothing Then
               sMsg = "Next:oTryTrueHalfEdge Is Nothing,"
               'AcadDocument.WriteMessage("oTryTrueHalfEdge Is Nothing")
            Else
               oTryFullEdge = oTryTrueHalfEdge.FullEdge
               sMsg = "Next:oTryTrueHalfEdge=" & CStr(oTryFullEdge.ID) & ","
               'AcadDocument.WriteMessage("oTryTrueHalfEdge=" & CStr(oTryFullEdge.ID))
            End If
            If oTryFalseHalfEdge Is Nothing Then
               sMsg &= "TryFalseHalfEdge Is Nothing"
               'AcadDocument.WriteMessage("TryFalseHalfEdge Is Nothing")
            Else
               oTryFullEdge = oTryFalseHalfEdge.FullEdge
               sMsg &= "TryFalseHalfEdge=" & CStr(oTryFullEdge.ID)
               'AcadDocument.WriteMessage("TryFalseHalfEdge=" & CStr(oTryFullEdge.ID))
            End If
            '' next  HalfEdge
            AcadDocument.WriteMessage(sMsg)
            oHalfEdge = dicHalfEdges.Item(oNode.ID)
            oFullEdge = oHalfEdge.FullEdge

            If oTryTrueHalfEdge.FullEdge.ID = oFullEdge.ID Then
               iTrueNext += 1
            ElseIf oTryFalseHalfEdge.FullEdge.ID = oFullEdge.ID Then
               iFalseNext += 1
            Else
               iMidNext += 1
            End If

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "TopoCreator - zzTestRing")
         End Try
      Next
      iEndID = oNode.ID


      '	AcadDocument.WriteMessage("Start-->End:" & CStr(iStartID) & "-->" & CStr(iEndID))
      If iMidNext <> 0 Then
         AcadDocument.WriteMessage("Dir is not valid(+-) - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      ElseIf iTrueNext <> 0 AndAlso iFalseNext <> 0 Then
         AcadDocument.WriteMessage("Dir is Mix (+-) - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      Else
         AcadDocument.WriteMessage("OK!!! - " & ":" & CStr(iTrueNext) & "," & CStr(iFalseNext) & "," & CStr(iMidNext))
      End If

   End Sub
   Private Shared Sub zzWriteVar(ByVal oVarList As ResultBuffer, ByVal sFileName As String)
      Dim oStreamWriter As IO.StreamWriter
      Dim sFolder As String = "C:\NetProjects8\Examples\DotNet\TopologyVB\Dwg\"
      Try
         oStreamWriter = New IO.StreamWriter(sFolder & sFileName)
      Catch oEx As System.Exception
         Exit Sub
      End Try
      Dim sLine As String = ""
      Dim iLine As Integer = 1
      Dim i As Integer = 0
		Dim oValue As System.Object
		Dim shCode As Short
      For Each oTypedValue As TypedValue In oVarList
         i += 1



         oValue = oTypedValue.Value
         Select Case i
            Case 2
               sLine = oValue.ToString()
            Case 3
               shCode = oTypedValue.TypeCode
               sLine = sLine & " = " & oValue.ToString()
         End Select

         If i = 4 Then
            sLine = CStr(iLine) & " " & CStr(shCode) & " " & sLine
            If iLine < 10 Then sLine = " " & sLine
            oStreamWriter.WriteLine(sLine)
            sLine = ""
            i = 0
            iLine += 1
         End If

         '  System.Windows.Forms.MessageBox.Show(CStr(shCode) & ":" & oValue.ToString(), CStr(iV))
      Next


      oStreamWriter.Close()
   End Sub
   Private Shared Function zzRemoveODTable(ByVal sODTableName As String) As Boolean
      Dim oODTables As Autodesk.Gis.Map.ObjectData.Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
      If oODTables.IsTableDefined(sODTableName) Then
         Try
            oODTables.RemoveTable(sODTableName)
            Return True
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoCreator - zzRemoveODTable")
            Return False
         End Try
      Else
         Return True
      End If
   End Function
   Private Shared Sub zzMarkError(ByVal oPoint As Autodesk.AutoCAD.Geometry.Point3d)
      Dim oTransaction As Transaction = Nothing
      Dim oTransactionManager As TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oBlockTableRecord As BlockTableRecord


      '    Dim oTextStyleTableRecord As TextStyleTableRecord = Nothing


      Dim oAcobjId As ObjectId
      Dim sTest As String = "a"
      Try
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), BlockTable)
         oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), BlockTableRecord)



         Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle = New Circle()

         oCircle.Center = oPoint
         oCircle.Radius = 1.0
         oAcobjId = oBlockTableRecord.AppendEntity(oCircle)
         sTest = "g"
         oTransactionManager.AddNewlyCreatedDBObject(oCircle, True)
         oTransaction.Commit()
      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzMarkError")
         oTransaction.Abort()
      Finally
         oTransaction.Dispose()
         oTransaction = Nothing
      End Try

   End Sub
   Private Shared Function zzPoint3dTo2d(ByVal tPoint3d As Autodesk.AutoCAD.Geometry.Point3d) As Autodesk.AutoCAD.Geometry.Point2d
      Return New Autodesk.AutoCAD.Geometry.Point2d(tPoint3d.X, tPoint3d.Y)
   End Function
   '    8 = Delete Duplicates 
   Private Shared Function zzGetTopoErrBlockName(iTopoErrType As enTopoErrType) As String
      Return TopoErrBlockNames(CInt(iTopoErrType))
   End Function
   Private Shared Function zzGetRhombusBlock() As ObjectId
      Dim sBlockName As String = zzGetTopoErrBlockName(enTopoErrType.RefRMark)
      Dim dlBlockDrawing As DMAcadExt.AcadTransaction.BlockDrawing = New DMAcadExt.AcadTransaction.BlockDrawing(AddressOf zzGetRhombusEntity) '
      '	Dim dlBlockDrawing As DMAcadExt.AcadTransaction.BlockDrawing = New DMAcadExt.AcadTransaction.BlockDrawing(AddressOf zzGetTest) ' zzGetTest
      Return DMAcadExt.AcadTransaction.CreateNewBlock(sBlockName, dlBlockDrawing)
   End Function
   Public Shared Function GetOctagonBlock() As ObjectId
      Dim sBlockName As String = zzGetTopoErrBlockName(enTopoErrType.RefOMark)

      Dim dlBlockDrawing As DMAcadExt.AcadTransaction.BlockDrawing = New DMAcadExt.AcadTransaction.BlockDrawing(AddressOf zzGetOctagonEntity)
      '	Dim dlBlockDrawing As DMAcadExt.AcadTransaction.BlockDrawing = New DMAcadExt.AcadTransaction.BlockDrawing(AddressOf zzGetTest) ' zzGetTest
      Return DMAcadExt.AcadTransaction.CreateNewBlock(sBlockName, dlBlockDrawing)
   End Function
   Private Shared Function zzGetRhombusEntity() As Entity()
      Dim taPoints() As Autodesk.AutoCAD.Geometry.Point3d = {New Autodesk.AutoCAD.Geometry.Point3d(-0.25, 0.0, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.5, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(0.25, 0.0, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(0.0, -0.5, 0.0)}
      Dim daBulges() As Double = {0.0, 0.0, 0.0, 0.0}
      Dim colVertices As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection(taPoints)
      Dim colBulges As Autodesk.AutoCAD.Geometry.DoubleCollection = New Autodesk.AutoCAD.Geometry.DoubleCollection(daBulges)
      Dim oPolyline2d As Polyline2d = New Polyline2d(Poly2dType.SimplePoly, colVertices, 0.0, True, 0.0, 0.0, colBulges)

      oPolyline2d.ColorIndex = 3
      Return New Entity() {oPolyline2d}
   End Function
   Private Shared Function zzGetOctagonEntity() As Entity()

      Dim oPolyline2d As Polyline2d = zzPolygonBlock(8)
      Return New Entity() {oPolyline2d}
   End Function
   Private Shared Function zzPolygonBlock(iVertexCount As Integer) As Polyline2d
      Dim daVertexPoints(iVertexCount - 1) As Autodesk.AutoCAD.Geometry.Point3d
      Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection
      Dim colBulges As Autodesk.AutoCAD.Geometry.DoubleCollection = New Autodesk.AutoCAD.Geometry.DoubleCollection()

      Dim dX, dY As Double
      Dim dCenterAngle As Double = 2 * Math.PI / iVertexCount

      For iIndex As Integer = 0 To iVertexCount - 1
         dX = Math.Cos(iIndex * dCenterAngle)
         dY = Math.Sin(iIndex * dCenterAngle)
         daVertexPoints(iIndex) = New Autodesk.AutoCAD.Geometry.Point3d(dX, dY, 0.0)
         colBulges.Add(0.0)
      Next

      colPoints = New Autodesk.AutoCAD.Geometry.Point3dCollection(daVertexPoints)
      'System.Windows.Forms.MessageBox.Show(CStr(colPoints.Count) & ":" & CStr(colBulges.Count), "03_943X")
      Return New Polyline2d(Poly2dType.SimplePoly, colPoints, 0.0, True, 0.0, 0.0, colBulges)
   End Function
   Private Shared Function zzGetTest() As Entity()
      Dim oLine1 As Line = New Line(New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(2.0, 2.0, 0.0))
      Dim oLine2 As Line = New Line(New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(2.0, 2.0, 0.0))

      Return New Entity() {oLine1, oLine2}
   End Function

End Class

