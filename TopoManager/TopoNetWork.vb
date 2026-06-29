Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TopoNetWork
	Private msTopoName As String
	Private moTopology As Autodesk.Gis.Map.Topology.TopologyModel
	Private mdicFullEdges As Dictionary(Of ObjectId, Integer) = New Dictionary(Of ObjectId, Integer)()
	Public Sub New(sTopoName As String)
		msTopoName = sTopoName
	End Sub
	Public Sub Load(iMode As Autodesk.Gis.Map.Topology.OpenMode)
		moTopology = TopoManager.TopoCreator.GetOpenedTopology(msTopoName, iMode, False, False)
		If moTopology IsNot Nothing Then
			Dim colFullEdges As Autodesk.Gis.Map.Topology.FullEdgeCollection = moTopology.GetFullEdges()
			mdicFullEdges = New Dictionary(Of ObjectId, Integer)()
			For Each oFullEdge As FullEdge In colFullEdges
				mdicFullEdges.Add(oFullEdge.Entity, oFullEdge.ID)
			Next
		End If

	End Sub
	Public Function GetRoute(tLinkObjID As ObjectId) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim iFullEdgeStartID As Integer
		Dim iFullEdgeID As Integer '
		'Dim iFullEdgeIDAlt As Integer
		Dim oFullEdge As Autodesk.Gis.Map.Topology.FullEdge
		Dim colResAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
		Dim oStartEdge As MyEdge
		Dim oMyEdge As MyEdge

		Dim oNextMyEdge As MyEdge
		Dim bGoForward As Boolean
		'	Dim iIndex As Integer = 0
		If mdicFullEdges.TryGetValue(tLinkObjID, iFullEdgeStartID) Then

			colResAcObjIDs.Add(tLinkObjID)
			'iIndex += 1

			iFullEdgeID = iFullEdgeStartID
			oFullEdge = moTopology.GetFullEdge(iFullEdgeID)
			'DMCommon.ExcelLogAW5.SetNextValue(0, "StartEdge", iFullEdgeID)
			oStartEdge = New MyEdge(0, oFullEdge)
			For iDir As Integer = 0 To 1
				bGoForward = Convert.ToBoolean(iDir)
				oMyEdge = oStartEdge
				Do
					oNextMyEdge = oMyEdge.getNext(bGoForward)
					If oNextMyEdge Is Nothing Then
						Exit Do
					Else
						'DMCommon.ExcelLogAW5.SetNextValue(0, "Next++", oNextMyEdge.PreviousFullEdge, oNextMyEdge.MapEdge.ID)
						'	ReDim Preserve taResAcObjIDs(iIndex)
						If colResAcObjIDs.Contains(oNextMyEdge.Entity) Then
							Exit Do
						End If
						colResAcObjIDs.Add(oNextMyEdge.Entity)
						'iIndex += 1
						oMyEdge = oNextMyEdge
					End If
				Loop
			Next

		End If

		Return colResAcObjIDs
		'	DMCommon.Debug.MsgBox("13_044d", oFullEdge.ID, oFullEdge.GetNextEdge(False, False).ID, oFullEdge.GetNextEdge(False, True).ID, oFullEdge.GetNextEdge(True, False).ID, oFullEdge.GetNextEdge(True, True).ID)

	End Function
	Private Class MyEdge
		Private moFullEdge As FullEdge
		Private miPreviousFullEdge As Integer
		Public Sub New(iPreviousFullEdge As Integer, oFullEdge As FullEdge)
			miPreviousFullEdge = iPreviousFullEdge
			moFullEdge = oFullEdge
			'DMCommon.ExcelLogAW5.SetNextValue(0, "NewEdge", miPreviousFullEdge, moFullEdge.ID)
		End Sub
		Public Function getNext(Optional bStartGoForward As Boolean = False) As MyEdge
			Dim oFullEdge As FullEdge = Nothing
			Dim oFullEdgeAlt As FullEdge
			Dim iDebugID As Integer
			Dim iFullEdgeID As Integer

			'DMCommon.ExcelLogAW5.SetNextValue(0, "getNext", bStartGoForward, moFullEdge.ID)

			Try
				oFullEdge = moFullEdge.GetNextEdge(bStartGoForward, False)
				iDebugID = oFullEdge.ID
			Catch oMapEx As Autodesk.Gis.Map.MapException
				If oMapEx.ErrorCode <> 2063 Then
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "GetNextEdge_TopoNetWork", "1:FullEdgeID=" & moFullEdge.ID.ToString())
				End If
				DMCommon.Debug.ExcelLog.SetNextValue(0, "1:nextNothing", bStartGoForward, miPreviousFullEdge, moFullEdge.ID)
			End Try
			'DMCommon.Debug.MsgBox("13_044k", moFullEdge.ID, iDebugID)
			If oFullEdge Is Nothing Then
				Return Nothing
			Else
				iFullEdgeID = oFullEdge.ID
				If iFullEdgeID = miPreviousFullEdge Then
					Try
						oFullEdge = moFullEdge.GetNextEdge(Not bStartGoForward, False)
						oFullEdgeAlt = moFullEdge.GetNextEdge(Not bStartGoForward, True)
						'DMCommon.ExcelLogAW5.SetNextValue(0, "GoBack", bStartGoForward, miPreviousFullEdge, moFullEdge.ID, oFullEdge.ID, oFullEdgeAlt.ID)
						If oFullEdge.ID = oFullEdgeAlt.ID Then
							'DMCommon.ExcelLogAW5.SetNextValue(0, "GoBack+", miPreviousFullEdge, moFullEdge.ID, oFullEdge.ID, oFullEdgeAlt.ID)
							Return New MyEdge(moFullEdge.ID, oFullEdge)
						Else
							Return Nothing
						End If
					Catch oMapEx As Autodesk.Gis.Map.MapException
						If oMapEx.ErrorCode <> 2063 Then
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "GetNextEdge_TopoNetWork", "2:FullEdgeID=" & moFullEdge.ID.ToString())

						End If
						Return Nothing
					End Try
				Else
					oFullEdgeAlt = moFullEdge.GetNextEdge(bStartGoForward, True)
					'	DMCommon.ExcelLogAW5.SetNextValue(0, "GoForward", bStartGoForward, miPreviousFullEdge, moFullEdge.ID, oFullEdge.ID, oFullEdgeAlt.ID)
					If oFullEdgeAlt.ID = iFullEdgeID Then
						Return New MyEdge(moFullEdge.ID, oFullEdge)
					Else
						Return Nothing
					End If
				End If
			End If
		End Function
		Public ReadOnly Property PreviousFullEdge As Integer
			Get
				Return miPreviousFullEdge
			End Get
		End Property
		Public ReadOnly Property MapEdge As FullEdge
			Get
				Return moFullEdge
			End Get
		End Property

		Public ReadOnly Property Entity As ObjectId
			Get
				Return moFullEdge.Entity
			End Get
		End Property
	End Class
End Class
