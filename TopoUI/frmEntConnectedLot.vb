Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Imports Autodesk.Gis.Map.Topology
Namespace Expro

	Public Class frmEntConnectedLot
		Private Class ExproLot
			Private miTopoID As Integer
			Private miLotID As Integer
			Private miExproType As Integer
			Private mdArea As Double
			Private mlstAreaEnt As List(Of AreaEnt)
			Public Sub New(iTopoID As Integer, iExproType As Integer, iLotID As Integer, dArea As Double)
				miTopoID = iTopoID
				miLotID = iLotID

				miExproType = iExproType
				mdArea = dArea
				mlstAreaEnt = New List(Of AreaEnt)()
			End Sub
			Public Sub AreaEnt(oAreaEnt As AreaEnt)
				mlstAreaEnt.Add(oAreaEnt)
			End Sub
			Public ReadOnly Property LotID As Integer
				Get
					Return miLotID
				End Get
			End Property
			Public ReadOnly Property ExproType As Integer
				Get
					Return miExproType
				End Get
			End Property
			Public ReadOnly Property Area As Double
				Get
					Return mdArea
				End Get
			End Property
		End Class
		Private Class AreaEnt
			Private miTopoID As Integer
			Private msTopoName As String
			Private msDescription As String

			Private miEntLayerID As Integer
			Private miExproLotID As Integer

			Private miLotID As Integer

			'Private miExproType As Integer
			Private mdAreaIn As Double
			Private mdArea As Double

			Public Sub New(iTopoID As Integer, sTopoName As String, sDescription As String, iEntLayerID As Integer, iExproLotID As Integer, dArea As Double)
				miTopoID = iTopoID
				msTopoName = sTopoName
				msDescription = sDescription
				miEntLayerID = iEntLayerID
				miExproLotID = iExproLotID
				mdArea = dArea
			End Sub
			Public ReadOnly Property TopoName As String
				Get
					Return msTopoName
				End Get
			End Property
			Public ReadOnly Property Description As String
				Get
					Return msDescription
				End Get
			End Property
			Public ReadOnly Property EntLayerID As Integer
				Get
					Return miEntLayerID
				End Get
			End Property
			Public ReadOnly Property ExproLotID As Integer
				Get
					Return miExproLotID
				End Get
			End Property

			Public Sub Add(oAreaEnt As AreaEnt)
				If msTopoName = oAreaEnt.TopoName AndAlso miEntLayerID = oAreaEnt.EntLayerID Then
					mdArea += oAreaEnt.Area
				Else
					DMCommon.Debug.ExcelLog.SetNextValue(11, "!Add(oAreaEnt", msTopoName, oAreaEnt.TopoName, miEntLayerID, oAreaEnt.EntLayerID, miExproLotID, oAreaEnt.ExproLotID, oAreaEnt.Area)
				End If
			End Sub
			Public ReadOnly Property Area As Double
				Get
					Return mdArea
				End Get
			End Property
		End Class
		Private Class LotExt
			Private miLotID As Integer
			Private msName As String
			Private miNumber As Integer
			Private miLanduseID As Integer
			Private msLanduseName As String
			Private msPlanName As String

			Private mdArea As Double
			Private mdInArea As Double
			Private mdOutArea As Double
			'	Private mcolAreaEnts As ObjectModel.Collection(Of AreaEnt)
			Private mdicAreaEnts As Dictionary(Of Integer, AreaEnt)

			Public Sub New(oLot As TopoManager.TPlanGraph.TplnLot)
				miLotID = oLot.TopoID
				msName = oLot.Name
				If Integer.TryParse(msName, miNumber) Then

				End If
				miLanduseID = oLot.LanduseID
				msLanduseName = oLot.LanduseName
				msPlanName = oLot.PlanName

				mdArea = oLot.AcadArea(False)
				'	mcolAreaEnts = New ObjectModel.Collection(Of AreaEnt)()
				mdicAreaEnts = New Dictionary(Of Integer, AreaEnt)()
			End Sub
			Public Sub Add(oExproLot As ExproLot)
				If oExproLot.ExproType = 0 Then
					mdOutArea += oExproLot.Area
				Else
					mdInArea += oExproLot.Area
				End If
			End Sub
			Public Sub AddAreaEnt(oAreaEnt As AreaEnt)
				Dim oSrcAreaEnt As AreaEnt = Nothing
				Dim dTest As Double
				'mcolAreaEnts.Add(oAreaEnt)
				If mdicAreaEnts.TryGetValue(oAreaEnt.EntLayerID, oSrcAreaEnt) Then
					oSrcAreaEnt.Add(oAreaEnt)

				Else
					oSrcAreaEnt = oAreaEnt
					mdicAreaEnts.Add(oAreaEnt.EntLayerID, oSrcAreaEnt)
				End If
				If miLotID = 1681 Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!AAEnt", mdicAreaEnts.Count, oAreaEnt.EntLayerID, oSrcAreaEnt.Area)
				End If
			End Sub

			Public ReadOnly Property LotID As Integer
				Get
					Return miLotID
				End Get
			End Property
			Public ReadOnly Property Name As String
				Get
					Return msName
				End Get
			End Property
			Public ReadOnly Property Number As Integer
				Get
					Return miNumber
				End Get
			End Property
			Public ReadOnly Property LanduseID As Integer
				Get
					Return miLanduseID
				End Get
			End Property
			Public ReadOnly Property LanduseName As String
				Get
					Return msLanduseName
				End Get
			End Property
			Public ReadOnly Property PlanName As String
				Get
					Return msPlanName
				End Get
			End Property
			Public ReadOnly Property Area As Double
				Get
					Return mdArea
				End Get
			End Property
			Public ReadOnly Property InArea As Double
				Get
					Return mdInArea
				End Get
			End Property
			Public ReadOnly Property OutArea As Double
				Get
					Return mdOutArea
				End Get
			End Property
			'Public ReadOnly Property AreaEnts As ObjectModel.Collection(Of AreaEnt)
			Public ReadOnly Property AreaEnts As Dictionary(Of Integer, AreaEnt).ValueCollection

				Get
					'Return mcolAreaEnts
					Return mdicAreaEnts.Values
				End Get

			End Property
		End Class

		Private Class LotComparer
			Implements System.Collections.Generic.IComparer(Of LotExt)

			Public Function Compare(oLotExtA As LotExt, oLotExtB As LotExt) As Integer Implements IComparer(Of LotExt).Compare
				Return oLotExtA.Number.CompareTo(oLotExtB.Number)
			End Function
		End Class
		Private Class EntLayer
			Private msName As String
			Private mdicEntLayerExproPgon As Dictionary(Of Integer, EntLayerExproPgon)
			Private mdicEntLayerPgons As Dictionary(Of Integer, EntLayerPgon)

			Public Sub New(sName As String)
				msName = sName
				mdicEntLayerExproPgon = New Dictionary(Of Integer, EntLayerExproPgon)()
				mdicEntLayerPgons = New Dictionary(Of Integer, EntLayerPgon)()
			End Sub
			Public Sub AddEntLayerPgon(oEntLayerPgon As EntLayerPgon)
				mdicEntLayerPgons.Add(oEntLayerPgon.TopoID, oEntLayerPgon)
			End Sub
			Public Sub AddInArea(iEntLayerPgon As Integer, dInArea As Double)
				Dim oEntLayerPgon As EntLayerPgon = Nothing
				If mdicEntLayerPgons.TryGetValue(iEntLayerPgon, oEntLayerPgon) Then
					oEntLayerPgon.AddInArea(dInArea)
				End If
			End Sub
			Public ReadOnly Property EntLayerPgons As Dictionary(Of Integer, EntLayerPgon)
				Get
					Return mdicEntLayerPgons
				End Get
			End Property

			Public Function GetOutsideArea(iEntLayerPgonID As Integer) As Double
				Dim oEntLayerPgon As EntLayerPgon = Nothing
				If mdicEntLayerPgons.TryGetValue(iEntLayerPgonID, oEntLayerPgon) Then
					Return oEntLayerPgon.GetOutsideArea()
				End If
			End Function
		End Class
		Private Class EntLayerExproPgon
			Private msEntLayerName As String
			Private miTopoID As Integer
			Private mdArea As Double
			Public Sub New(sEntLayerName As String, iTopoID As Integer, dArea As Double)
				msEntLayerName = sEntLayerName
				miTopoID = iTopoID
				mdArea = dArea
			End Sub

			Public Sub AddArea(dArea As Double)

			End Sub
		End Class
		Private Class EntLayerPgon
			Private msEntLayerName As String
			Private miTopoID As Integer
			Private mdArea As Double
			Private mdInArea As Double

			Public Sub New(sEntLayerName As String, iTopoID As Integer, dArea As Double)
				msEntLayerName = sEntLayerName
				miTopoID = iTopoID
				mdArea = dArea
			End Sub
			Public ReadOnly Property TopoID As Integer
				Get
					Return miTopoID
				End Get
			End Property
			Public Sub AddInArea(dInArea As Double)
				mdInArea += dInArea
			End Sub
			Public Function GetOutsideArea() As Double
				DMCommon.Debug.ExcelLog.SetNextValue(8, "!GetOutA", mdArea, mdInArea)
				Return mdArea - mdInArea
			End Function
		End Class
		Const msLayerPrefix As String = "dm-"
		Private mhsEntLayers As HashSet(Of String)
		Private mdicLots As TopoManager.TPlanGraph.TplnLots
		Private mdicExproLots As Dictionary(Of Integer, ExproLot)
		Private mdicLotExts As Dictionary(Of Integer, LotExt)
		Private mdicEntLayers As Dictionary(Of String, EntLayer)
		Private miTestTopoID As Integer
		Private mlstLotExts As List(Of LotExt)
		Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmExproLotConn

		Public Sub New()

			' This call is required by the designer.
			InitializeComponent()
			mdicLotExts = New Dictionary(Of Integer, LotExt)()
			zzInit()
		End Sub

		Private Sub frmEntConnectedLot_FormClosed(osender As System.Object, e As FormClosedEventArgs) Handles Me.FormClosed

		End Sub
		Private Sub zzInit()
			Const sCentroidTopoNamePrefix As String = "TPMCNTR_"

			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
			Dim oODTables As Autodesk.Gis.Map.ObjectData.Tables = oProject.ODTables
			Dim colStrings As System.Collections.Specialized.StringCollection = oODTables.GetTableNames()
			Dim iPrefixLen As Integer = sCentroidTopoNamePrefix.Length
			Dim sTopoName As String
			Dim oTopos As Topologies = oProject.Topologies
			Dim iPgonsCount As Integer
			Dim oRow As ListViewItem
			mhsEntLayers = New HashSet(Of String)()
			For Each sName As String In colStrings
				'	AcadDocument.WriteMessage("&& " & sName)
				If sName.StartsWith(sCentroidTopoNamePrefix) Then
					sTopoName = sName.Substring(iPrefixLen)

					If oTopos.Exists(sTopoName) AndAlso sTopoName.StartsWith(msLayerPrefix) AndAlso Not sTopoName.EndsWith("_L") AndAlso Not sTopoName.EndsWith("_L_C") Then
						Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Topo01", oTopoModel IsNot Nothing, tTopoRes.IsOK, tTopoRes.IsTopoOK, sMsg)
						If oTopoModel IsNot Nothing Then
							iPgonsCount = oTopoModel.GetPolygons().Count
							oRow = New ListViewItem(sTopoName, 0)
							oRow.SubItems.Add(iPgonsCount.ToString())
							'oRow.SubItems.Add("2")

							Me.lvwEntLayers.Items.Add(oRow)
							mhsEntLayers.Add(sTopoName)
							oTopoModel.Close()
						End If
					End If
				End If

			Next


		End Sub

		Private Function zzGetMapName(sSourceTopoName As String, iMaxLenth As Integer) As String
			If sSourceTopoName.Length > iMaxLenth Then
				Return sSourceTopoName.Substring(0, iMaxLenth - 1)
			Else
				Return sSourceTopoName
			End If
		End Function
		Private Sub zzLoadExproLots()

			Dim oExproLotTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("ExproLotK", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			Dim colPolygons As PolygonCollection
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			Dim oExproLot As ExproLot
			Dim oLot As TopoManager.TPlanGraph.TplnLot = Nothing
			Dim oLotExt As LotExt = Nothing

			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet("ExproLine", "LotsKLine", "OD_ExproLotK")


			If oExproLotTopo IsNot Nothing Then
				mdicExproLots = New Dictionary(Of Integer, ExproLot)()


				colPolygons = oExproLotTopo.GetPolygons()
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!dicLotsB", DMCommon.Debug.ColCount(mdicLots), DMCommon.Debug.ColCount(mdicLotExts), DMCommon.Debug.ColCount(colPolygons))
				For Each oExproLotPgon As Autodesk.Gis.Map.Topology.Polygon In colPolygons

					oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oExproLotPgon.Entity)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayODRecord", oOverlayODRecord.NewID, oOverlayODRecord.SourceID, oOverlayODRecord.OverlayID, oExproLotPgon.Area)
					If oOverlayODRecord.NewID <> 0 Then
						oExproLot = New ExproLot(oOverlayODRecord.NewID, oOverlayODRecord.SourceID, oOverlayODRecord.OverlayID, oExproLotPgon.Area)

						mdicExproLots.Add(oOverlayODRecord.NewID, oExproLot)
						If oExproLot IsNot Nothing AndAlso mdicLotExts IsNot Nothing AndAlso Not mdicLotExts.TryGetValue(oExproLot.LotID, oLotExt) Then
							If mdicLots.TryGetValue(oExproLot.LotID, oLot) Then
								oLotExt = New LotExt(oLot)
								mdicLotExts.Add(oLotExt.LotID, oLotExt)
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!!mdicLotExts.Add", oLotExt.LotID, oLotExt.Area, oLotExt.InArea, oLotExt.OutArea)
							Else
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!Not mdicLotExts.Try", oExproLot.LotID)
							End If

						End If
						If oLotExt IsNot Nothing Then

							oLotExt.Add(oExproLot)
							DMCommon.Debug.ExcelLog.SetNextValue(0, "!!oLotExt", oLotExt.InArea, oLotExt.OutArea, oExproLot.ExproType, oExproLot.Area)
						End If


					End If



					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oExproLotPgon", oExproLotPgon.ID, oOverlayODRecord.SourceAreaPcnt)
					'If mdicLotExts.SourceAreaPcnt = 100.0 Then
					'hExternalPolygons.Add(oUnionPgon.ID)
					'End If
				Next
			End If

			mlstLotExts = New List(Of LotExt)(mdicLotExts.Values)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadExproLots", mdicExproLots.Count, mdicLotExts.Count)



		End Sub
		Private Function zzGetDescription(sTopoName As String) As String
			Dim sComText As String = "SELECT Description FROM  dbo.EntConnectedLayers WHERE  (UpLayer = '" & UCase(sTopoName) & "')"
						Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, System.Data.CommandType.Text)
			If oRes IsNot Nothing Then
				Return DirectCast(oRes, String)
			Else
				Return sTopoName
			End If

		End Function
		Private Sub zzLoadEntLayerLot(sTopoName As String)
			Dim oEntLayerIdenTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(zzGetIdentityTopoName(sTopoName, "_L"), Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			Dim colPolygons As PolygonCollection
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			Dim oExproLot As ExproLot = Nothing
			Dim oAreaEnt As AreaEnt
			Dim iExproLotID As Integer
			Dim oLotExt As LotExt = Nothing
			Dim oEntLayer As EntLayer = Nothing
			Dim sDescription As String
			Dim bRes As Boolean
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			bRes = mdicEntLayers.TryGetValue(sTopoName, oEntLayer)
			DMCommon.Debug.ExcelLog.SetNextValue(6, "!zzLoadEntLayerLot", sTopoName, bRes)
			If bRes Then
				DMCommon.Debug.ExcelLog.SetNextValue(6, "!EntLayer", oEntLayer.EntLayerPgons.Count)
			End If

			sDescription = zzGetDescription(sTopoName)
			'Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(sTopoName, "ExproLotK", zzGetODTableName(zzGetIdentityTopoName(sTopoName, "_L")))
			' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''New TopoManager.OverlayODRecordSet("ExproLine", "LotsKLine", "OD_ExproLotK")
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!dicLots1", sTopoName)
			If oEntLayerIdenTopo IsNot Nothing Then
				colPolygons = oEntLayerIdenTopo.GetPolygons()
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayerLot!!", sTopoName, colPolygons.Count)
				For Each oEntLayerPgon As Autodesk.Gis.Map.Topology.Polygon In colPolygons

					oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oEntLayerPgon.Entity)
					oAreaEnt = New AreaEnt(oOverlayODRecord.NewID, sTopoName, sDescription, oOverlayODRecord.SourceID, oOverlayODRecord.OverlayID, oEntLayerPgon.Area)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!@EntLayerPgon!", sTopoName, oOverlayODRecord.SourceID, oOverlayODRecord.OverlayID)

					If oOverlayODRecord.NewID <> 0 Then
						iExproLotID = oOverlayODRecord.OverlayID
						If mdicExproLots.TryGetValue(iExproLotID, oExproLot) Then
							If oExproLot.ExproType <> 0 AndAlso mdicLotExts.TryGetValue(oExproLot.LotID, oLotExt) Then
								oLotExt.AddAreaEnt(oAreaEnt)

								DMCommon.Debug.ExcelLog.SetNextValue(24, "!AddAreaEnt!", sTopoName, oLotExt.Name, oAreaEnt.Area)

								If oEntLayer IsNot Nothing Then
									oEntLayer.AddInArea(oOverlayODRecord.SourceID, oAreaEnt.Area)

									DMCommon.Debug.ExcelLog.SetNextValue(18, "!@EntLayerPgon!", sTopoName, oLotExt.Name, oAreaEnt.Area, oEntLayer.GetOutsideArea(miTestTopoID))
								End If
								'oExproLot.ExproType
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!zz!!", oExproLot.LotID, mdicLotExts.Count, oLotExt.AreaEnts.Count)
							Else
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer1", oExproLot.LotID, mdicLotExts.Count)
							End If
						Else
							DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer2", iExproLotID, mdicExproLots.Count)
						End If
					Else
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer3", oOverlayODRecord.NewID)
					End If
				Next

			End If
		End Sub
		Private Sub zzCalcAreaOutsidePgon(sTopoName As String)
			Dim oEntLayer As EntLayer = Nothing
			If mdicEntLayers.TryGetValue(sTopoName, oEntLayer) Then

			End If
		End Sub

		Private Sub zzLoadEntLayerExpro(sTopoName As String)
			Dim oEntLayerIdenTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(zzGetIdentityTopoName(sTopoName, "_E"), Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			Dim oEntLayerTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(zzGetIdentityTopoName(sTopoName, "_E"), Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)

			Dim colPolygons As PolygonCollection
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			Dim oExproLot As ExproLot = Nothing
			Dim oAreaEnt As AreaEnt
			Dim iExproLotID As Integer
			Dim oLotExt As LotExt = Nothing
			Dim oEntLayer As EntLayer
			Dim oEntLayerPgon As EntLayerExproPgon

			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			'Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(sTopoName, "ExproLine", zzGetODTableName(zzGetIdentityTopoName(sTopoName, "_E")))
			' ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''New TopoManager.OverlayODRecordSet("ExproLine", "LotsKLine", "OD_ExproLotK")
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayerExpro", sTopoName)
			If oEntLayerIdenTopo IsNot Nothing Then
				oEntLayer = New EntLayer(sTopoName)
				colPolygons = oEntLayerIdenTopo.GetPolygons()
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntLayerExpro!!", sTopoName, colPolygons.Count)
				For Each oEntLayerPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons

					oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oEntLayerPolygon.Entity)

					''''''''''''''	oAreaEnt = New AreaEnt(oOverlayODRecord.NewID, sTopoName, oOverlayODRecord.OverlayID, oEntLayerPolygon.Area)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!OverlayODRecord!!", oOverlayODRecord.NewID, oOverlayODRecord.SourceID, oOverlayODRecord.OverlayID)
					If oOverlayODRecord.NewID <> 0 Then
						'	iExproLotID = oOverlayODRecord.OverlayID
						'	If mdicExproLots.TryGetValue(iExproLotID, oExproLot) Then
						'		If oExproLot.ExproType <> 0 AndAlso mdicLotExts.TryGetValue(oExproLot.LotID, oLotExt) Then
						'			oLotExt.AddAreaEnt(oAreaEnt)

						'		Else
						'			DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer1", oExproLot.LotID, mdicLotExts.Count)
						'		End If
						'	Else
						'		DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer2", iExproLotID, mdicExproLots.Count)
						'	End If
						'Else
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadEntLayer3", oOverlayODRecord.NewID)
					End If
				Next

			End If
		End Sub

		Private Sub zzLoadEntLayer(sTopoName As String)
			Dim oEntLayerTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)

			Dim colPolygons As PolygonCollection


			Dim iTest As Integer = 0
			Dim oEntLayer As EntLayer = New EntLayer(sTopoName)

			Dim oEntLayerPgon As EntLayerPgon

			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!dicLots2", DMCommon.Debug.ColCount(dicLots))
			If oEntLayerTopo IsNot Nothing Then

				colPolygons = oEntLayerTopo.GetPolygons()
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntLayerExpro!!", sTopoName, colPolygons.Count)
				For Each oEntLayerPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					miTestTopoID = oEntLayerPolygon.ID
					oEntLayerPgon = New EntLayerPgon(sTopoName, oEntLayerPolygon.ID, oEntLayerPolygon.Area)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadEntLayer!", sTopoName, colPolygons.Count, iTest, oEntLayerPolygon.Area)
					oEntLayer.AddEntLayerPgon(oEntLayerPgon)
				Next
				oEntLayerTopo.Close()
				DMCommon.Debug.ExcelLog.SetNextValue(8, "!LoadEntLayer2!", sTopoName, oEntLayer.GetOutsideArea(miTestTopoID), iTest)
				mdicEntLayers.Add(sTopoName, oEntLayer)
			End If
		End Sub
		Private Function zzGetIdentityTopoName(sSourceTopoName As String, sSuffix As String) As String
			Return zzGetMapName(sSourceTopoName, 17 - sSuffix.Length) & sSuffix

		End Function
		Private Function zzGetODTableName(sTopoName As String) As String
			Return "OD_" & sTopoName

		End Function
		Private Function zzGetCentroidLayerName(sTopoName As String, sSuffix As String) As String
			Dim sIdenTopoName As String = zzGetIdentityTopoName(sTopoName, sSuffix)

			Return sIdenTopoName & "_C"

		End Function


		Private Sub cmdTopo_Overlay_Click(oSender As System.Object, e As EventArgs) Handles cmdTopo_Overlay.Click
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			Dim oExproLotTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("ExproLotK", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
			Dim oExproTopo As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("ExproLine", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)

			Dim oEntLayerTopo As TopologyModel
			Dim tResultODTable As ObjectDataTable
			Dim oCentroidCreationSettings As PointCreationSettings ' = New PointCreationSettings("MRK", 256, True, "MRK")
			Dim oEdgeCreationSettings As EntityCreationSettings '= New EntityCreationSettings("ExproParcels", 0)
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 0, False, "")
			Dim sCentroidLayer As String
			Dim sNewTopoName As String
			If oExproLotTopo IsNot Nothing Then
				For Each sTopoName As String In mhsEntLayers
					oEntLayerTopo = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
					If oEntLayerTopo IsNot Nothing Then
						tResultODTable = New ObjectDataTable()
						sNewTopoName = zzGetIdentityTopoName(sTopoName, "_L")
						tResultODTable.ODTableName = zzGetODTableName(sNewTopoName)
						sCentroidLayer = zzGetCentroidLayerName(sTopoName, "_L")
						oCentroidCreationSettings = New PointCreationSettings(sCentroidLayer, 256, True, "MRK")
						oEdgeCreationSettings = New EntityCreationSettings(sNewTopoName, 0)

						oEntLayerTopo.SetCentroidCreationSettings(oCentroidCreationSettings)
						oEntLayerTopo.SetNodeCreationSettings(oNodeCreationSettings)
						oEntLayerTopo.SetEdgeCreationSettings(oEdgeCreationSettings)
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!PrevData7", sNewTopoName, tResultODTable.ODTableName, sCentroidLayer, zzGetMapName(sNewTopoName, 17))
						'	DMCommon.Debug.MsgBox("!Identity-1a", tResultODTable.ODTableName, oEntLayerTopo.Name, oExproLotTopo.Name)
						Try
							oEntLayerTopo.Identity(oExproLotTopo, zzGetMapName(sNewTopoName, 17), "", tResultODTable)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "xx7", False)
							DMCommon.Debug.MsgBox("!Identity1", tResultODTable.ODTableName)
						End Try
						'190524

						If oExproTopo IsNot Nothing Then

							tResultODTable = New ObjectDataTable()
							sNewTopoName = zzGetIdentityTopoName(sTopoName, "_E")
							tResultODTable.ODTableName = zzGetODTableName(sNewTopoName)
							sCentroidLayer = zzGetCentroidLayerName(sTopoName, "_E")
							oCentroidCreationSettings = New PointCreationSettings(sCentroidLayer, 256, True, "MRK")
							oEdgeCreationSettings = New EntityCreationSettings(sNewTopoName, 0)

							oEntLayerTopo.SetCentroidCreationSettings(oCentroidCreationSettings)
							oEntLayerTopo.SetNodeCreationSettings(oNodeCreationSettings)
							oEntLayerTopo.SetEdgeCreationSettings(oEdgeCreationSettings)

							DMCommon.Debug.ExcelLog.SetNextValue(0, "!PrevData8", sNewTopoName, sCentroidLayer, tResultODTable.ODTableName, sCentroidLayer)
							Try
								oEntLayerTopo.Identity(oExproTopo, sNewTopoName, "", tResultODTable)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "xx8", False)
								DMCommon.Debug.MsgBox("!Identity2", tResultODTable.ODTableName)
							End Try
						End If
						oEntLayerTopo.Close()
					End If

				Next
				oExproLotTopo.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			Me.Cursor = Cursors.Default
		End Sub
		Private Sub zzPrint()
			For Each oLotExt As LotExt In mdicLotExts.Values
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!LotExts", oLotExt.Name, oLotExt.Number, oLotExt.LanduseID, oLotExt.LanduseName, oLotExt.PlanName,oLotExt.Area, oLotExt.InArea, oLotExt.OutArea)
				If oLotExt.AreaEnts.Count > 0 Then
					DMCommon.Debug.ExcelLog.MoveRow(-1)
				End If
				For Each oAreaEnt As AreaEnt In oLotExt.AreaEnts
					DMCommon.Debug.ExcelLog.SetNextValue(12, oAreaEnt.TopoName, oAreaEnt.Area)
				Next
			Next
		End Sub
		Private Sub zzPrint1()
			'Lot Num=389  TopoId=1681
			Const sSheetName As String = "מחוברים"
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim oComparer As LotComparer = New LotComparer()
			Dim oEntLayer As EntLayer = Nothing
			Dim oHebText As DMCommon.HebrewTrans
			Dim dOutsideArea As Double
			Dim iRow As Integer
			mlstLotExts.Sort(oComparer)
			If oExcelAppExt.Open() Then


				oExcelAppExt.SetSheetName(sSheetName)


				''''''''''''''''''''''''''''''''''''''''''''''''''''''

				oExcelAppExt.Open()
				oExcelAppExt.Activate()

				'zzSetTitlePart(oExcelAppExt)
				'zzSetTitlePartII(oExcelAppExt)

				'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				Dim dlProc As DMCommon.ExcelAppExt.HeaderValue = New DMCommon.ExcelAppExt.HeaderValue(AddressOf zzHeaderText)

				'For iCol As Integer = 0 To 9
				oExcelAppExt.SetValueInHeaderRow(0, 0, 0, 9, True, dlProc)
				'Next

				'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
				oExcelAppExt.MoveRow(1)
				'oExcelAppExt.SetNextValue(0, "!!LotExts", "------------------------", mlstLotExts.Count, "--------------------")
				For Each oLotExt As LotExt In mlstLotExts
					'oExcelAppExt.SetNextValue(0, "!!Lot+", oLotExt.LotID, oLotExt.Number, oLotExt.Name, oLotExt.InArea)
					If oLotExt.InArea > 0.0 Then


						oHebText = New DMCommon.HebrewTrans(oLotExt.PlanName, False)
						oExcelAppExt.SetNextValue(0, "", oLotExt.Number, oLotExt.LanduseID, oLotExt.LanduseName, oHebText.GetWinDest(False),
																		  FormatNumber(oLotExt.InArea, 0,,, TriState.True), FormatNumber(oLotExt.OutArea, 0,,, TriState.True))
						If oLotExt.AreaEnts.Count > 0 Then
							oExcelAppExt.MoveRow(-1)
						End If
						For Each oAreaEnt As AreaEnt In oLotExt.AreaEnts
							If mdicEntLayers.TryGetValue(oAreaEnt.TopoName, oEntLayer) Then
								dOutsideArea = oEntLayer.GetOutsideArea(oAreaEnt.EntLayerID)
							Else
								dOutsideArea = 0.0
							End If
							oExcelAppExt.SetNextValue(6, oAreaEnt.Description, "מ""ר", FormatNumber(oAreaEnt.Area, 0,   ,, TriState.True), FormatNumber(dOutsideArea, 0,   ,, TriState.True))
						Next
					End If
				Next
			End If
		End Sub
		Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
			Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, True)
		End Function
		Private Function zzHeaderText(ByVal iHeaderRow As Integer, ByVal iColumnHeader As Integer) As String
			Return TPlServerDB.TextResource.GetText(iColumnHeader, miResourceTheme, iHeaderRow, True)
		End Function


		Private Sub zzTest()
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
			'	For Each oLot As TopoManager.TPlanGraph.TplnLot In dicLots.Values
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!dicLots", DMCommon.Debug.ColCount(dicLots), oLot.Name, oLot.TopoID, oLot.LanduseID, oLot.LanduseName, oLot.AcadArea(False))
			'Next
			For Each oLotExt As LotExt In mdicLotExts.Values
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicLotExts", DMCommon.Debug.ColCount(mdicLotExts), oLotExt.Name, oLotExt.Number, oLotExt.LanduseID, oLotExt.LanduseName, oLotExt.PlanName,
																 oLotExt.Area, oLotExt.InArea, oLotExt.OutArea, oLotExt.AreaEnts.Count)
			Next
		End Sub
		Private Sub cmdCalculate_Click(oSender As System.Object, e As EventArgs) Handles cmdCalculate.Click
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Approved
			Me.Cursor = Cursors.WaitCursor
			mdicLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
			If mdicLots IsNot Nothing Then

				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				zzLoadExproLots()
				mdicEntLayers = New Dictionary(Of String, EntLayer)()
				'zzTest()
				For Each sTopoName As String In mhsEntLayers
					'zzLoadEntLayerExpro(sTopoName)
					zzLoadEntLayer(sTopoName)
					zzLoadEntLayerLot(sTopoName)

					'zzCalcAreaOutsidePgon(sTopoName)
				Next
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
			End If

			Me.Cursor = Cursors.Default
		End Sub



		Private Sub cmdExcel_Click(osender As System.Object, e As EventArgs) Handles cmdExcel.Click
			Me.Cursor = Cursors.WaitCursor
			zzPrint1()
			Me.Cursor = Cursors.Default
		End Sub

		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub
	End Class
End Namespace