Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports DMAcadExt
Imports Autodesk.Gis.Map.Topology
Public Class frmLotPoints
	Const msBlockName As String = "SRVS006"
	Const msBlockLayer As String = "SRVS006"
	Const msBlockNewName As String = "SRVS007"
	Const msBlockNewLayer As String = "SRVS007"

	Const msTopoName As String = "LotsK"
	Const msTopoLineName As String = "LotsKLine"

	Const msObjectIDFieldName As String = "ObjectID"

	Private Const miParamType As Integer = 1
	Private moMainTable As System.Data.DataTable
	Private moMainDataView As System.Data.DataView
	Private msaAttribFields() As String
	Private moCurrentAcadBlock As DMAcadExt.AcadBlock
	Private miFirstAttribColIndex As Integer
	Private miLastAttribColIndex As Integer
	Private miFirstAttribFieldIndex As Integer
	Private miLastAttribFieldIndex As Integer

	Private mdicBlockRefs As Dictionary(Of ObjectId, Integer) = New Dictionary(Of ObjectId, Integer)()

	Private midgvMainLocationY As Integer

	Private mtMapThemeData As DMAcadExt.MapThemeData
	Private mdicBlockPointCells As DMAcadExt.TplnPointCells = New DMAcadExt.TplnPointCells()
	Private mdicNodePointCells As DMAcadExt.TplnPointCells = New DMAcadExt.TplnPointCells()
	Private mdicMarkPointCells As DMAcadExt.TplnPointCells = New DMAcadExt.TplnPointCells()
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)

		' This call is required by the designer.
		InitializeComponent()
		mtMapThemeData = tMapThemeData
		' Add any initialization after the InitializeComponent() call.

		zzMyInitializeComponent()
	End Sub
	Private Sub zzMyInitializeComponent()
		Me.dgvMain.AutoGenerateColumns = False
		midgvMainLocationY = Me.dgvMain.Location.Y
		zzCreateMainTable()
		'zzInitBlocks()
		TplnPointKeyULong.SetOrigin(AcadDocument.GetExtMinPoint())
		TplnPointKeyULong.Tolerance = 0.001

		zzLoadBlocks()

		zzLoadNodes()

	End Sub

	Private Sub zzInitBlocks()
		Dim oAcadBlock As AcadBlock = Nothing
		Dim oAttribDef As AttributeDefinition
		miFirstAttribColIndex = Me.dgvMain.ColumnCount

		oAcadBlock = New AcadBlock(msBlockName)
		moCurrentAcadBlock = oAcadBlock
		'DMCommon.Debug.MsgBox("11_090d1", oAcadBlock.BlockName)
		oAcadBlock.Open(True)
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!DefNumber", oAcadBlock.AttributeDefs.GetUpperBound(0))
		'	DMCommon.Debug.MsgBox("11_090e", oAcadBlock.BlockName, msBlockName, oAcadBlock.HasAttributeDefinitions)
		If oAcadBlock.HasAttributeDefinitions Then
			For iIndex As Integer = 0 To oAcadBlock.AttributeDefs.GetUpperBound(0)
				oAttribDef = oAcadBlock.AttributeDefs(iIndex)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!DefName", oAttribDef.Tag, oAttribDef.Prompt)
				zzAddGridColumn(oAttribDef)
				zzAddAttribField(oAttribDef.Tag)
			Next
			' = Me.dgvMain.ColumnCount - 1
			miLastAttribColIndex = Me.dgvMain.ColumnCount - 1
			miLastAttribFieldIndex = moMainTable.Columns.Count - 1
		End If

	End Sub
	Private Sub zzFillGrid()

		moCurrentAcadBlock.LoadAllReferences()


		Dim mcolCurrentBlockRefIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(msBlockName & "," & msBlockNewName)

		Dim oBlockRef As BlockReference
		Dim oAddBlockRef As BlockReference = Nothing

		Dim colAttributes As AttributeCollection
		Dim colAddAttributes As AttributeCollection = Nothing


		'  Dim oGridRow As DataGridViewRow = Nothing
		Dim iRowIndex As Integer
		'   Dim lObjID As Long
		Dim iAttribRefUB As Integer = -1
		Dim iAddAttribRefUB As Integer = -1

		Dim oAttribRef As AttributeReference = Nothing
		Dim iColIndex As Integer
		Dim iFieldIndex As Integer

		Dim bErr As Boolean
		Dim oNewRow As System.Data.DataRow
		Dim hsLayers As HashSet(Of String) = New HashSet(Of String)()


		Dim tAddBlockRefObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing

		Dim iName As Integer
		Dim iNameMax As Integer = 0
		moMainTable.Rows.Clear()
		mdicBlockRefs.Clear()
		'	DMCommon.Debug.MsgBox("EdBlRefs003", "After zzFillFirstRow", mcolCurrentBlockRefIDs.Count)
		For Each tBlockRefObjID As ObjectId In mcolCurrentBlockRefIDs
			bErr = False
			oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				iAttribRefUB = colAttributes.Count - 1
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("AcadTransaction - GetAttribText_97:" & oEx.Message)
				iAttribRefUB = -1
				colAttributes = Nothing
			End Try

			oNewRow = moMainTable.NewRow()
			With oNewRow
				.Item("BlockName") = oBlockRef.Name
				.Item("Layer") = oBlockRef.Layer
				If Not hsLayers.Contains(oBlockRef.Layer) Then
					hsLayers.Add(oBlockRef.Layer)
				End If

				.Item(msObjectIDFieldName) = oBlockRef.ObjectId
				.Item("PositionX") = oBlockRef.Position.X
				.Item("PositionY") = oBlockRef.Position.Y
				.Item("Scale") = oBlockRef.ScaleFactors.X
				.Item("Changed") = False

				'DMCommon.Debug.MsgBox("EdBlRefs005", iAttribRefUB, colAttributes.Count)
				For iAttribIndex As Integer = 0 To iAttribRefUB
					Try
						oAttribRef = DMAcadExt.AcadTransaction.GetAttribRef(colAttributes.Item(iAttribIndex), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					Catch oEx As Exception
						DMCommon.Debug.MsgBox("EdBlRefs105", oEx.Message, iAttribRefUB, colAttributes.Count)

					End Try

					iColIndex = miFirstAttribColIndex + iAttribIndex
					iFieldIndex = miFirstAttribFieldIndex + iAttribIndex
					If oAttribRef IsNot Nothing AndAlso Me.dgvMain.Columns.Item(iColIndex) IsNot Nothing Then

						If moMainTable.Columns.Item(iFieldIndex).ColumnName = oAttribRef.Tag Then
							.Item(iFieldIndex) = oAttribRef.TextString

						Else
							.Item(iFieldIndex) = " ***" & oAttribRef.Tag
							bErr = True
						End If
					Else
						bErr = True
					End If
				Next

			End With

			iName = zzGetNameInt(oNewRow)
			If iName > iNameMax Then
				iNameMax = iName
			End If
			'	msLayerSelected = String.Empty

			moMainTable.Rows.Add(oNewRow)
			'	DMCommon.Debug.MsgBox("13_047GD")
			'   lObjID = oBlockRef.ObjectId.OldIdPtr.ToInt64()
			iRowIndex += 1
			mdicBlockRefs.Add(oBlockRef.ObjectId, iRowIndex)



		Next
		'DMCommon.Debug.MsgBox("13_047GC")
		Me.txtNumberMax.Text = CStr(iNameMax)
		Me.txtNumberFrom.Text = CStr(iNameMax + 1)
		moMainDataView = New System.Data.DataView(moMainTable, String.Empty, msObjectIDFieldName, Data.DataViewRowState.CurrentRows)

		dgvMain.DataSource = moMainDataView
		'	miRecordCount = mcolCurrentBlockRefIDs.Count
		Me.txtRowNumber.Text = CStr(moMainDataView.Count)
		'	mbEventsEnabled = True

	End Sub
	Private Sub zzUpdateDWG()
		Dim bChanged As Boolean
		Dim tAcObjID As ObjectId
		For Each oDataRow As Data.DataRow In moMainTable.Rows
			bChanged = DMCommon.Functions.CBoolN(oDataRow.Item("Changed"))
			If bChanged Then
				tAcObjID = DirectCast(oDataRow.Item("ObjectID"), ObjectId)
				'DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID,)
			End If
		Next
	End Sub
	Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles Button1.Click
		Dim sSourceTopoName As String = "LotsK"
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
		Dim oRing As TopoManager.TopoScheme.tsRing
		Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
		Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology(sSourceTopoName)
		Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
		Dim colPoints As Autodesk.AutoCAD.Geometry.Point2dCollection = New Autodesk.AutoCAD.Geometry.Point2dCollection()
		Dim tPoint As Point2d
		Dim tPoint3d As Point3d
		Dim iRes As Integer
		'Dim iName As Integer
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		oAcadBlock.OpenForRight()
		'DMCommon.Debug.MsgBox("13_128r", oAcadBlock.BlockDefObjID)
		oTopoScheme.Load(True)

		For Each oPolygon As TopoManager.TopoScheme.tsPolygon In oTopoScheme.Polygons
			For iIndex As Integer = 0 To oPolygon.RingsUB
				oRing = oPolygon.Rings(iIndex)
				oBulgeVertexArray = oRing.GetBulgeVertexArray()

				For iVertexIndex As Integer = 0 To oBulgeVertexArray.UB
					tPoint = oBulgeVertexArray.Item(iVertexIndex).Vertex
					iRes = mdicBlockPointCells.AnalysisPoint(tPoint)
					DMCommon.Debug.ExcelLog.SetNextValue(2, "!Analisis", mdicBlockPointCells.Count, oPolygon.Centroid, iIndex, iVertexIndex, iRes)
					If iRes = -1 Then
						tPoint3d = DMAcadExt.TPlnPoint.Point2dTo3d(tPoint)
						tBlockRefData = New BlockRefData(tPoint3d)
						oAcadBlock.InsertRef(tBlockRefData)
					End If
				Next

				oBulgeVertexArray.PrintInfo()
				DMCommon.Debug.ExcelLog.NextRow()
				colPoints = oBulgeVertexArray.GetPseudoVertices(tTolerance)
				For Each tPointA As Autodesk.AutoCAD.Geometry.Point2d In colPoints
					oSquareMarkBlock.MarkPoint(tPointA, 2S)
				Next
			Next
		Next
		oTopoScheme.Close()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Function zzGetNameInt(oDataRow As System.Data.DataRow) As Integer
		Dim sName As String = DMCommon.Functions.CStrN(oDataRow.Item("Name"))
		Return zzToInt(sName)
	End Function
	Private Function zzToInt(sName As String) As Integer
		Dim iName As Integer
		If Not Integer.TryParse(sName, iName) Then
			iName = 0
		End If
		Return iName
	End Function
	Private Function zzGetNameInt(oDataRow As System.Data.DataRowView) As Integer
		Dim sName As String = DMCommon.Functions.CStrN(oDataRow.Item("Name"))
		Return zzToInt(sName)
	End Function

	Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
		Dim sSourceTopoName As String = "LotsK"
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology(sSourceTopoName)

		oTopoScheme.Load(True)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	Private Sub zzLoadBlocks()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim oBlockRef As BlockReference

		zzInitBlocks()

		zzFillGrid()


		Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(msBlockName, msBlockLayer)
		For Each tBlockRefObjID As ObjectId In colBlockRefs
			oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
			mdicBlockPointCells.AddBlockRef(oBlockRef, 0)
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzLoadNodes()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		'	Dim oBlockRef As BlockReference

		Dim oTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)

		If oTopology IsNot Nothing Then
			Dim colNodes As NodeCollection = oTopology.GetNodes()

			For Each oNode As Node In colNodes
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Nodes", oNode.GetEdges().Count, TPlnPoint.Point3dTo2d(oNode.Location))
				If oNode.GetEdges().Count > 2 Then
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!1Nodes")
					mdicNodePointCells.AddGeoPoint(TPlnPoint.Point3dTo2d(oNode.Location))
				End If
			Next
		End If
		'DMCommon.Debug.MsgBox("11_090S")
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub cmdMarkPseudoVertices_Click(oSender As System.Object, e As EventArgs) Handles cmdMarkPseudoVertices.Click

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
		Dim oRing As TopoManager.TopoScheme.tsRing
		Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
		Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology(msTopoName)
		Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
		Dim colPoints As Autodesk.AutoCAD.Geometry.Point2dCollection = New Autodesk.AutoCAD.Geometry.Point2dCollection()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim iRes As Integer
		oAcadBlock.OpenForRight()
		'	For Each oPointCell As TplnPointCell In mdicPointCells.Values
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!AllPoint", oPointCell.PointCount, oPointCell.PrimaryPoint.Point.Coordinates2d, oPointCell.Key.Point.Coordinates2d, oPointCell.Key.Code, oPointCell.Key.X, oPointCell.Key.y)
		'	Next
		mdicMarkPointCells.Clear()
		oTopoScheme.Load(True)

		For Each oPolygon As TopoManager.TopoScheme.tsPolygon In oTopoScheme.Polygons
			For iIndex As Integer = 0 To oPolygon.RingsUB
				oRing = oPolygon.Rings(iIndex)
				oBulgeVertexArray = oRing.GetBulgeVertexArray()
				colPoints = oBulgeVertexArray.GetPseudoVertices(tTolerance)
				For Each tPointA As Autodesk.AutoCAD.Geometry.Point2d In colPoints
					iRes = mdicBlockPointCells.AnalysisPoint(tPointA)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Analisis1", mdicPointCells.Count, tPointA, oPolygon.ID, iIndex, iRes)
					If iRes <= 0 Then
						iRes = mdicNodePointCells.AnalysisPoint(tPointA)
						If iRes <= 0 Then
							iRes = mdicMarkPointCells.AnalysisPoint(tPointA)
							If iRes <= 0 Then
								oSquareMarkBlock.MarkPoint(tPointA, 2S)
								mdicMarkPointCells.AddGeoPoint(tPointA)
							End If

						End If

					End If
				Next
			Next
		Next
		oTopoScheme.Close()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdDeleteTopo_Click(oSender As System.Object, e As EventArgs) Handles cmdDeleteTopo.Click
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Try
			oTopos.Delete(msTopoName, False)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Delete Topology 1")
		End Try

		Try
			oTopos.Delete(msTopoLineName, False)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Delete Topology 2")
		End Try



		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	Private Sub CreateTopo_Click(oSender As System.Object, e As EventArgs) Handles CreateTopo.Click
		Dim mtTopoRes As DMAcadExt.TopoRes
		Dim moParams As TPlServerDB.dmParams
		Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		moParams = New TPlServerDB.dmParams(iProjectCode, iDetailNo, mtMapThemeData.MapThemeID, miParamType)
		Dim dTolerance As Double = moParams.GetDblValue(0)


		mtTopoRes = TopoManager.TopoCreator.CreateTopology(mtMapThemeData.TopoName, mtMapThemeData.MapThemeID, mtMapThemeData.LinkLayers, "", mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers, False, mtMapThemeData.NodeBlocks, mtMapThemeData.NodeLayers, False, False, dTolerance)
		mtTopoRes = TopoManager.TopoCreator.CreateTopology(mtMapThemeData.LineTopoName, mtMapThemeData.MapThemeID, mtMapThemeData.LinkLayers, mtMapThemeData.LineLinkLayers, mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers, False, mtMapThemeData.NodeBlocks, mtMapThemeData.CentroidLayers, False, False, dTolerance)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub cmdInsertBlocks_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertBlocks.Click
		'	Dim sSourceTopoName As String = "LotsK"

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
		'	Dim oRing As TopoManager.TopoScheme.tsRing
		'Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray
		Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology(msTopoName)
		Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
		Dim colPoints As Autodesk.AutoCAD.Geometry.Point2dCollection = New Autodesk.AutoCAD.Geometry.Point2dCollection()
		Dim tPoint As Point2d
		Dim tPoint3d As Point3d
		Dim iRes As Integer
		Dim oaInnerPoints As DMAcadExt.TplnPointArray
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msBlockName)
		Dim oAcadBlockNew As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msBlockNewName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim tBlockRefObjID As ObjectId
		Dim oBlockRef As BlockReference
		oAcadBlockNew.OpenForRight()
		oTopoScheme.Load(True)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msBlockNewLayer, DMApp.AppID, True, True)
		'mdicPointCells.PrintSummary()
		For Each oNode As TopoManager.TopoScheme.tsNode In oTopoScheme.Nodes
			tPoint3d = oNode.Location
			tPoint = TPlnPoint.Point3dTo2d(tPoint3d)
			iRes = mdicBlockPointCells.AnalysisPoint(tPoint)
			If iRes <= 0 Then
				tPoint3d = DMAcadExt.TPlnPoint.Point2dTo3d(tPoint)
				tBlockRefData = New BlockRefData(tPoint3d)
				tBlockRefObjID = oAcadBlockNew.InsertRef(tBlockRefData)
				oBlockRef = AcadTransaction.GetBlockRef(tBlockRefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				mdicBlockPointCells.AddBlockRef(oBlockRef, 0)
			End If
		Next
		For Each oBranch As TopoManager.TopoScheme.tsBranch In oTopoScheme.Branches
			oaInnerPoints = oBranch.GetInnerPoints()
			If oaInnerPoints IsNot Nothing Then
				For iIndex As Integer = 0 To oaInnerPoints.UpperBound()
					tPoint = oaInnerPoints.Item(iIndex).AcGePoint
					iRes = mdicBlockPointCells.AnalysisPoint(tPoint)
					If iRes <= 0 Then
						tPoint3d = DMAcadExt.TPlnPoint.Point2dTo3d(tPoint)
						tBlockRefData = New BlockRefData(tPoint3d)
						tBlockRefObjID = oAcadBlockNew.InsertRef(tBlockRefData)
						oBlockRef = AcadTransaction.GetBlockRef(tBlockRefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
						mdicBlockPointCells.AddBlockRef(oBlockRef, 0)
					End If
				Next
			End If

		Next

		oTopoScheme.Close()
		zzFillGrid()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzAddAttribField(sFieldName As String)
		Dim oDataColumn As System.Data.DataColumn = New System.Data.DataColumn(sFieldName, GetType(System.String))
		moMainTable.Columns.Add(oDataColumn)

	End Sub
	Private Sub zzCreateMainTable()
		Dim oDataColumn As System.Data.DataColumn
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim oDataType As System.Type = tAcObjID.GetType()
		msaAttribFields = {"Name", "Kind", "Height"}
		'	Dim tLines As System.Data.DataTable
		moMainTable = New System.Data.DataTable("MainTable")
		oDataColumn = New System.Data.DataColumn(msObjectIDFieldName, oDataType)
		moMainTable.Columns.Add(oDataColumn)



		oDataColumn = New System.Data.DataColumn("PositionX", GetType(System.Double))
		moMainTable.Columns.Add(oDataColumn)
		oDataColumn = New System.Data.DataColumn("PositionY", GetType(System.Double))
		moMainTable.Columns.Add(oDataColumn)
		oDataColumn = New System.Data.DataColumn("Scale", GetType(System.Double))
		moMainTable.Columns.Add(oDataColumn)
		oDataColumn = New System.Data.DataColumn("Selected", GetType(System.Boolean))
		oDataColumn.DefaultValue = False
		moMainTable.Columns.Add(oDataColumn)
		oDataColumn = New System.Data.DataColumn("Modified", GetType(System.Boolean))
		oDataColumn.DefaultValue = False
		moMainTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("Changed", GetType(System.Boolean))
		oDataColumn.DefaultValue = False
		moMainTable.Columns.Add(oDataColumn)
		oDataColumn = New System.Data.DataColumn("Layer", GetType(System.String))
		moMainTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("BlockName", GetType(System.String))
		moMainTable.Columns.Add(oDataColumn)

		oDataColumn = New System.Data.DataColumn("ObjectId", GetType(ObjectId))
		moMainTable.Columns.Add(oDataColumn)

		'For iIndex As Integer = 0 To msaAttribFields.GetUpperBound(0)
		'zzAddAttribField(msaAttribFields(iIndex))
		'Next
		'	oDataColumn = New System.Data.DataColumn(msEntityFldName, System.Type.GetType("Autodesk.AutoCAD.DatabaseServices.ObjectId"))
		miFirstAttribFieldIndex = moMainTable.Columns.Count


	End Sub
	Private Sub cmdUpdateCurrent_Click(sender As System.Object, e As System.EventArgs) Handles cmdUpdateCurrent.Click
		Dim saValues(miLastAttribFieldIndex - miFirstAttribFieldIndex) As String
		'Dim saAddValues() As String = Nothing


		Dim tAcObjID As ObjectId


		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		Dim sLayer As String

		'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		'	DMAcadExt.AcadTransaction.Start()
		'	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		If oCurrentCell IsNot Nothing Then
			Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
			Dim oDataRow As System.Data.DataRowView = DirectCast(oGridRow.DataBoundItem, System.Data.DataRowView)



			If zzRowIsChanged(oDataRow) Then
				tAcObjID = zzGetRowObjectID(oDataRow)
				For iIndex As Integer = miFirstAttribFieldIndex To miLastAttribFieldIndex
					saValues(iIndex - miFirstAttribFieldIndex) = DMCommon.Functions.CStrN(oDataRow.Item(iIndex))
				Next


				sLayer = zzGetLayer(oDataRow)
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

				Dim bRes As Boolean = DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID, saValues, sLayer)
				'	Dim bAddRes As Boolean = True



				If bRes Then
					zzSetRowChanged(oDataRow, False)
				End If

				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				DMAcadExt.AcadDocument.UpdateScreen()
			End If
		End If

		'DMAcadExt.AcadTransaction.CloseModelSpace()
		'DMAcadExt.AcadTransaction.Terminate()
		'DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Function zzGetLayer(oDataRow As System.Data.DataRowView) As String
		Return DMCommon.Functions.CStrN(oDataRow.Item("Layer"))
	End Function
	Private Function zzRowIsChanged(oDataRow As System.Data.DataRowView) As Boolean
		Return DMCommon.Functions.CBoolN(oDataRow.Item("Changed"))
	End Function
	Private Sub zzSetRowChanged(oDataRow As System.Data.DataRowView, bVaue As Boolean)
		oDataRow.Item("Changed") = bVaue
	End Sub

	Private Sub cmdZoom_Click(oSender As System.Object, e As System.EventArgs) Handles cmdZoom.Click
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell

		If oCurrentCell IsNot Nothing Then
			Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
			Dim dX, dY, dScale As Double
			Dim dDWGScale As System.Double = DMAcadExt.AcadDocument.GetDWGScaleFactor()
			Dim oDataRowView As Data.DataRowView = DirectCast(oGridRow.DataBoundItem, Data.DataRowView)

			'CDbl(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.ScaleSysVarName))
			Dim tAcObjID As ObjectId = zzGetRowObjectID(oDataRowView)
			dX = Convert.ToDouble(oDataRowView.Item("PositionX"))
			dY = Convert.ToDouble(oDataRowView.Item("PositionY"))
			dScale = Convert.ToDouble(oDataRowView.Item("Scale"))

			Dim oPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(dX, dY)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadDocument.Zoom(oPoint, dScale * 10.0)  'dDWGScale *

			DMAcadExt.AcadTransaction.Highlight(tAcObjID)
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()

		End If





	End Sub

	Private Function zzGetRowObjectID(oDataRowView As Data.DataRowView) As ObjectId
		Return DirectCast(oDataRowView.Item("ObjectID"), ObjectId)
	End Function

	Private Sub zzAddGridColumn(oAttribDef As AttributeDefinition)
		Const sColNamePrefix As String = "ctx"
		'Dim iColIndex As Integer = Me.dgvMain.ColumnCount

		Dim oTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		'
		'Column1A
		'

		'	DMAcadExt.AcadDocument.WriteMessage("TAG: " & mcolTagPrompts.Item(mcolTagPrompts.Count - 1).Tag)
		With oTextBoxColumn

			.HeaderText = oAttribDef.Prompt
			'	DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.Spell(oAttribDef.Prompt))

			.ReadOnly = False
			.Name = sColNamePrefix & oAttribDef.Tag
			.DataPropertyName = oAttribDef.Tag
			.Width = 64
		End With
		Me.dgvMain.Columns.Add(oTextBoxColumn)
	End Sub

	Private Sub dgvMain_CellContentClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellContentClick

	End Sub

	Private Sub dgvMain_CellValueChanged(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellValueChanged
		If e.RowIndex >= 0 Then
			Dim oDataRowView As System.Data.DataRowView = moMainDataView.Item(e.RowIndex)
			oDataRowView.Item("Changed") = True
		End If

	End Sub

	Private Sub frmLotPoints_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
		If midgvMainLocationY <> 0 Then
			Try
				Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditBlockRef_Resize")
			End Try
		End If
	End Sub

	Private Sub cmdNumber_Click(oSender As System.Object, e As EventArgs) Handles cmdNumber.Click
		Dim sName As String
		Dim sCurrentVal As String
		Dim iCurrentVal As Integer
		Dim tAcObjID As ObjectId
		Dim dicNameValue As Dictionary(Of String, String) = New Dictionary(Of String, String)()
		Dim iName As Integer
		Dim iNameMax As Integer
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		If Integer.TryParse(Me.txtNumberFrom.Text, iCurrentVal) Then
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!StartNum", iCurrentVal, moMainDataView.Count, msBlockName)
			For Each oDataRow As Data.DataRowView In moMainDataView
				sName = DMCommon.Functions.CStrN(oDataRow.Item("Name"))
				If String.IsNullOrEmpty(sName) OrElse (Me.chkReplace.Checked AndAlso IsNumeric(sName) AndAlso (DMCommon.Functions.CStrN(oDataRow.Item("BlockName")) <> msBlockName)) Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!iCurrentVal", iCurrentVal, moMainDataView.Count, DMCommon.Functions.CStrN(oDataRow.Item("BlockName")))
					sCurrentVal = iCurrentVal.ToString()
					oDataRow.Item("Name") = sCurrentVal
					dicNameValue.Clear()
					dicNameValue.Add("NAME", sCurrentVal)
					tAcObjID = DirectCast(oDataRow.Item("ObjectID"), ObjectId)
					DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID, dicNameValue)
					iCurrentVal += 1
				End If
				iName = zzGetNameInt(oDataRow)
				If iName > iNameMax Then
					iNameMax = iName
				End If
			Next

			Me.txtNumberMax.Text = CStr(iNameMax)
			Me.txtNumberFrom.Text = CStr(iNameMax + 1)
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub



	Private Sub cmdUpdateAll_Click(oSender As System.Object, e As EventArgs) Handles cmdUpdateAll.Click
		Dim saValues(miLastAttribFieldIndex - miFirstAttribFieldIndex) As String

		Dim tAcObjID As ObjectId


		'	Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		Dim sLayer As String

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		'	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)




		'Dim oDataRow As System.Data.DataRowView = DirectCast(oGridRow.DataBoundItem, System.Data.DataRowView)

		For Each oDataRow As Data.DataRowView In moMainDataView
			If zzRowIsChanged(oDataRow) Then
				tAcObjID = zzGetRowObjectID(oDataRow)
				For iIndex As Integer = miFirstAttribFieldIndex To miLastAttribFieldIndex
					saValues(iIndex - miFirstAttribFieldIndex) = DMCommon.Functions.CStrN(oDataRow.Item(iIndex))
				Next


				sLayer = zzGetLayer(oDataRow)

				Dim bRes As Boolean = DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID, saValues, sLayer)
				'	Dim bAddRes As Boolean = True



				If bRes Then
					zzSetRowChanged(oDataRow, False)
				End If
			End If
		Next

		'	DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()




	End Sub

	Private Sub cmdRefresh_Click(oSender As System.Object, e As EventArgs) Handles cmdRefresh.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
		zzFillGrid()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

End Class