Option Strict On
Option Explicit On
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Imports TopoManager.TopoScheme
Imports TopoManager.TPlanGraph

Public Class frmUnidiv
	Private Enum enRowStatus
		[Default]
		StartOfAction
		EndOfAction
		EndOfStage
		Transfer
	End Enum
	Private Enum enAddDataType
		AllData
		StageData
		ActionData
	End Enum



	Private Enum enCentroidStatus
		[Default]
		FromNewLayer
		FromStageLayer
		FromTaba
		' ByPick
		[New]
	End Enum
	Private Enum enAreaTable
		LR
		FDB
	End Enum
	Private Enum enBlockEdgeType
		[Default]
		NewBlockBorder
		CancelledBlockBorder
		BlockedBlockBorder
	End Enum
	Private Enum enSelectLinkType
		[Default]
		SelectionSet
		SelectLinks
		SelectLinksByFragments

		Script

	End Enum

	Const msParcelTopoName As String = "Parcels"
	Const msPLineCancelledLayerOld As String = "PCLP005"
	Const msLinkNewLayerOld As String = "PCLP011"

	Const msLinkGushLayer As String = "C1650"
	Const msLinkGushCancelledLayer As String = "C1651"

	Const msLinkGushBlockedLayer As String = "C1650Bl"

	Const msLinkNewGushLayer As String = "C1652"

	Const msLinkParcelLayer As String = "C1660"
	Const msLinkCancelledLayer As String = "C1661"
	Const msLinkNewLayer As String = "C1662"
	'Const msLinkBridgeLayer As String = "PCLP010"
	'Const msLinkNewBridgeLayer As String = "PCLP013"

	Const msLinkBridgeLayer As String = "pclp010"
	Const msLinkNewBridgeLayer As String = "pclp013"


	Const miPlanTypeDflt As frmUD_General.enPlanType = frmUD_General.enPlanType.PlanType1
	Const miStartPointNumDflt As Integer = 10
	Const mdMinAreaDeviation As Double = 0.0005

	Const msUnionActionName As String = "איחוד"
	Const msDivideActionName As String = "חלוקה"
	Const msTransferActionName As String = "העברה"



	Dim msTabaCentroidBlockName As String = "CellNoDM"

	Const msFragmentsTopoName As String = "Fragments"
	'  Const msFinalTopologyName As String = "FinalStage"
	Public Const ProjectDataBase As String = "UD_Projects"
	Const msNewParcelLayer As String = "Ud_NewParcels"


	Const miSelectedColorIndex As Integer = 50

	Const miAfterActionDividerHeight As Integer = 2
	Const miAfterStageDividerHeight As Integer = 4
	Const miRoundDigit As Integer = 3
	Const msCaption As String = "איחוד וחלוקה"

	Private moAppWin As Autodesk.AutoCAD.Windows.Window
	Private mdicParcels As UnidivNet.UD_Parcels
	Private mlstInitParcels As List(Of UD_ParcelKey)
	Private mdicJournalRows As Dictionary(Of UD_ParcelKey, Integer) = New Dictionary(Of UD_ParcelKey, Integer)()

	Private mhsSelectedParcels As HashSet(Of UnidivNet.UD_Parcel) = New HashSet(Of UnidivNet.UD_Parcel)()
	Private mlstSelectedParcelKeys As List(Of UD_ParcelKey) = New List(Of UD_ParcelKey)()
	Private mlstSelectedParcels As List(Of UnidivNet.UD_Parcel) = New List(Of UnidivNet.UD_Parcel)()

	Private Shared msCentroidBlockName As String
	Private Shared mdicPoints As UnidivNet.UD_Points
	Private Shared mdicFLines As UnidivNet.UD_FLines
	Private Shared mcolAllNodes As ObjectIdCollection = New ObjectIdCollection()
	Private mcolCanceledLinks As ObjectIdCollection = New ObjectIdCollection()
	Private mcolBlockedLinks As ObjectIdCollection = New ObjectIdCollection()


	Private moFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel
	Private moFragmentsToposcheme As tsTopology
	Private mdicFragments As Fragments
	Private mdicBaseParcels1 As Dictionary(Of Integer, UnidivNet.UD_Parcels)

	Private mhsPoints As Ud_Points
	Private mhsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
#Region "dgvCellStyle"
	Private moDataGridViewCellStyleArea As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
	Private moDataGridViewCellStyleAreaExcept As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
	Private moDataGridViewCellStyleAreaCanceled As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()

	Private moDataGridViewCellStyleTolerance As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
	Private moDataGridViewCellStyleInput As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()
	Private moDataGridViewCellStyleInputGush As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()

	Private moDataGridViewCellStyleInputSaved As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()
	Private moDataGridViewCellStyleInputSavedGush As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()


	Private moDataGridViewCellStyleGush As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()


#End Region

	Private miFromParcelColIndex As Integer = 2
	Private miFromParcelTempColIndex As Integer = 3
	Private miToParcelColIndex As Integer = 4
	Private miToGushColIndex As Integer = 5
	Private miToGushAddColIndex As Integer = 6

	Private miForcedAreaColIndex As Integer = 9

	Private miPlanNameColIndex As Integer = 13
	Private miLotNameColIndex As Integer = 14
	Private miLanduseIDColIndex As Integer = 15
	Private miLanduseNameColIndex As Integer = 16
	Private mdicLayersOffStatus As Dictionary(Of ObjectId, Boolean)

	Private mcolTabaCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private mcolNewLayerTabaCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private mcolStageLayerCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()

	Private mtScale3d As Autodesk.AutoCAD.Geometry.Scale3d
	Private Shared mbHasder As Boolean = True

	'  Private moShrinkPgonJig As UnidivNet.ShrinkPgonJig
	'  Private moShrinkPolygon As UnidivNet.ShrinkPolygon
	Private moJournalDBTable As System.Data.DataTable
	Private WithEvents moMainTable As System.Data.DataTable
	Private moNewDataRow As DataRow
	'Private mbNewDataRowUsed As Boolean
	'   Private moMainView As DataView
	Private moParcelTable As System.Data.DataTable
	Private moParcelLogTable As System.Data.DataTable
	Private moFragmentTable As System.Data.DataTable
	Private moParcelFragmentsTable As System.Data.DataTable
	Private moProjectPlanTable As System.Data.DataTable


	Private moLastActionRow As DataRow
	Private moJournalDataAdapter As Data.Common.DbDataAdapter
	Private moParcelLogDataAdapter As Data.Common.DbDataAdapter
	Private mbUpdateOpened As Boolean = False
	Private moFragmentsDataAdapter As Data.Common.DbDataAdapter
	Private moParcelFragmentsDataAdapter As Data.Common.DbDataAdapter
	Private moProjectPlanDataAdapter As Data.Common.DbDataAdapter

	Private mtEditingCellKey As CellKey
	Private mtEditingParcelKey As UD_ParcelKey


	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private miPlanID As Integer
	Private miPlanType As frmUD_General.enPlanType
	Private mbDatabound As Boolean
	Private mbDirty As Boolean
	Private mcolLine As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private miLastRowIndex As Integer
	Private miLastRowStatusIsNotDefined As Boolean
	Private mhsInputCells As HashSet(Of CellKey) = New HashSet(Of CellKey)(New CellComparer())
	Private mbEventsEnabled As Boolean
	Private miBlockNo As Integer
	Private miBlockAddNo As Integer = 0
	Private miBlockKey As Integer
	Private mtCentroidScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d

	Private miThisPlanMinDBNum As Integer = Integer.MaxValue
	Private miMaxPointDBNum As Integer = 0

	Private miCurrentActionType As UnidivNet.enActionType
	Private miCurrentStage As Integer = 0
	Private miCurrentAction As Integer = 0
	'  Private miCurrentOperation As Integer = 0
	Private miCurrentCentroidStatus As enCentroidStatus = enCentroidStatus.Default
	Private moCurrentStageTopoScheme As TopoManager.TopoScheme.tsTopology

	Private miCurrentActionFirstRowIndex As Integer = -1
	Private miLastActionFirstRowIndex As Integer = -1

	Private miActionBatchFirstRowIndex As Integer = -1
	Private miBatchFirstAction As Integer = -1


	Private mtCurrentScriptData As ScriptData
	' Private mbExtended As Boolean
	'  Private miCurrentNewParcelNo As Integer
	Private mtCurrentParcelKey As UD_ParcelKey
	Private moStageTopologies As StageTopologies = New StageTopologies()
	Private mhsOperUDLayers As HashSet(Of String) = New HashSet(Of String)(New String() {"aaa", "bbb"})

	Private moOperUDLayersList As DMCommon.dmList = New DMCommon.dmList(New String() {msLinkGushLayer, msLinkParcelLayer, msLinkCancelledLayer, msLinkNewLayer, msLinkNewBridgeLayer, "C1603_*"})

	'Private moByPickLayersList As DMCommon.dmList = New DMCommon.dmList(New String() {msLinkCancelledLayer, msLinkNewLayer, msLinkNewBridgeLayer})
	'C1650,C1660,C1603
	Private moUD_ParcelLayerList As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*"})
	Private moUD_PLineLayerList As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_0", "C1602_1", "C1602_2", "C1602_3", "C1602_4", "C1602_5", "C1602_6"})



	Private moODTable As UnidivNet.Ud_ScriptODTable '  DMAcadExt.ODTable

	' 230322 Private moUD_ParcelLayerListPlusNew As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*", msLinkNewLayer, msLinkNewBridgeLayer})
	''''''''''''''	Private moUD_ParcelLayerListPlusNew As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*", msLinkGushLayer, msLinkParcelLayer, msLinkNewLayer, msLinkNewBridgeLayer}) '070724
	Private moUD_ParcelLayerListPlusNew As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*", msLinkGushLayer, msLinkParcelLayer, msLinkNewLayer, msLinkNewBridgeLayer, msLinkBridgeLayer})




	'C1650,C1660,C1603
	Private mdicBaseParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
	Private mdicDWGBaseParcelsByFragments As Dictionary(Of Integer, UnidivNet.UD_Parcel)

	Private mcolUnvisibleLayers As IEnumerable(Of String) = New System.Collections.ObjectModel.Collection(Of String)()
	Private mcolParcelCancelled As ObjectIdCollection = New ObjectIdCollection()
	Private mcolNotFocused As ObjectIdCollection = New ObjectIdCollection()
	Private mcolNewLinksOutOfSourceParcel As ObjectIdCollection

	Private mbCheckExtended As Boolean = True

	Private miStageDisplayed As Integer = -1
	Private midgvMainLocationY As Integer
	Private msaPointLayers() As String
	Private WithEvents mfUD_General As TopoUI.frmUD_General
	Private Sub zzFillInitGrid()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow = Nothing
		Dim iIndex As Integer
		Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		' mdicRows = New Dictionary(Of Integer, UD_Row)
		'    53 62 12 985 Vova   0536212985
		Dim b As Boolean = DMCommon.Debug.Debug()
		'DMCommon.Debug.MsgBox("@mdicParcels.Count", mdicParcels.Count)
		mlstInitParcels.Sort(oParcelComparer)
		For Each tParcelKey As UD_ParcelKey In mlstInitParcels
			' DMCommon.Debug.MsgBox("12_908", tParcelKey.ToString())
			'  Next
			If mdicParcels.TryGetValue(tParcelKey, oParcel) Then

				'For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
				'  DMCommon.Debug.MsgBox("oParcel.Fragments.Count", oParcel.Fragments.Count)
				zzAddGridRow(enRowStatus.Default, False, oParcel.DbID)
				'  DMCommon.Debug.MsgBox("12_101gb", mbEventsEnabled)
				If mbDatabound Then
					oDataRow = moMainTable.Rows.Item(iIndex)
				End If
				oGridRow = Me.dgvMain.Rows.Item(iIndex)

				'   Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey


				' DMCommon.Debug.MsgBox("12_101fa", "")
				'  mdicRows.Add(iIndex, New UD_Row(0))
				If iIndex = 0 Then
					If mbDatabound Then
						' DMCommon.Debug.MsgBox("12_101fb", mbEventsEnabled)
						oDataRow.Item("StageCaption") = miCurrentStage
						' DMCommon.Debug.MsgBox("12_101fc", mbEventsEnabled)
						oDataRow.Item("ActionName") = zzGetActionName(miCurrentActionType)
						'  oDataRow.Item("OriginalParcelNo") = oParcel.Name
						oDataRow.Item("RowStatus") = enRowStatus.StartOfAction
						' oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
					Else
						oGridRow.Cells.Item("ctxStage").Value = miCurrentStage
						oGridRow.Cells.Item("ctxAction").Value = zzGetActionName(miCurrentActionType)
						'  zzFillFromParcelKey(oParcel, oGridRow)

						' oGridRow.Cells.Item("ctxFromParcel").Value = oParcel.Name
					End If
					'  DMCommon.Debug.MsgBox("12_101fk", mbEventsEnabled)
				ElseIf iIndex = mdicParcels.Count - 1 Then
					If mbDatabound Then
						oDataRow.Item("RowStatus") = enRowStatus.EndOfStage
					End If
				End If

				If mbDatabound Then
					'   oDataRow.Item("OriginalParcelNo") = oParcel.ParcelKey.ParcelNo
					oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
				End If
				'   DMCommon.Debug.MsgBox("12_101fs", "")
				'  zzFillFromParcelKey(oParcel, oGridRow)
				zzFillFromParcelKeyNew(oParcel, oDataRow, oGridRow)
				zzFillParcelArea(oParcel.ParcelArea, oDataRow, oGridRow)

				'oGridRow.Cells.Item("ctxLegalArea").Value = oParcel.LegalArea
				'oGridRow.Cells.Item("ctxArea").Value = oParcel.AcadArea(True)
				'oGridRow.Cells.Item("ctxTolerance").Value = 0.001 * oParcel.Tolerance
				'oGridRow.Cells.Item("ctxDiff").Value = oParcel.LegalArea - oParcel.AcadArea(True)
				'oGridRow.Cells.Item("ctxDeviation").Value = oParcel.Deviation
				iIndex += 1
			End If

		Next
		miLastRowIndex = iIndex - 1
		zzSetParcelCount(mlstInitParcels.Count)
	End Sub
	Private Sub zzSetStageLayers()
		ReDim msaPointLayers(miCurrentStage)
		For iIndex As Integer = 0 To miCurrentStage
			msaPointLayers(iIndex) = UnidivNet.UD_App.GetStagePolineLayer(iIndex)

		Next
	End Sub
	Private Sub zzSetUD_ParcelList()
		Dim oaLayers(2 * miCurrentStage + 1) As String
		For iIndex As Integer = 0 To miCurrentStage
			' oaLayers(2 * iIndex + 1) =
			' oaLayers(2 * iIndex)
		Next

	End Sub
	Private Sub zzCalcInitGrid()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow = Nothing
		'   Dim iIndex As Integer
		Dim iDBstage As Integer
		Dim tSourceParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		'    mdicRows = New Dictionary(Of Integer, UD_Row)
		For iRowIndex As Integer = 0 To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			iDBstage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
			If iDBstage <> 0 Then
				Exit For
			End If
			tSourceParcelKey = zzParcelKeyFrom(oDataRow)
			If mdicParcels.TryGetValue(tSourceParcelKey, oParcel) Then
				oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
				zzFillParcelAreaDataRow(oParcel.ParcelArea, oDataRow)
				If oParcel.ParcelArea.Deviation > mdMinAreaDeviation Then
					oGridRow = zzGetRow(iRowIndex)
					oGridRow.Cells.Item("ctxDeviation").Style = moDataGridViewCellStyleAreaExcept
				End If
			End If
			'  mdicRows.Add(iRowIndex, New UD_Row(0))
			miLastRowIndex = iRowIndex
		Next


	End Sub
	Private Sub zzSetParcelCount(iCount As Integer)
		Const sManyParcelsText As String = " חלקות  "
		Const sOneParcelsText As String = "חלקה אחת"
		If iCount = 1 Then
			Me.txtParcelCount.Text = sOneParcelsText
		ElseIf iCount > 1 Then
			Me.txtParcelCount.Text = iCount.ToString & sManyParcelsText
		Else
			Me.txtParcelCount.Text = "___"
		End If

	End Sub
	Private Sub zzUpdateLastRowStatus()
		Dim oGridRow As DataGridViewRow = zzGetRow(miLastRowIndex)
		Dim oDataRow As DataRow = moMainTable.Rows.Item(miLastRowIndex)
		Dim iActionType As UnidivNet.enActionType = zzToActionType(oDataRow)
		Dim iRowStatus As enRowStatus = zzGetRowStatus(oDataRow)
		If iActionType = UnidivNet.enActionType.Divide AndAlso miCurrentActionType <> UnidivNet.enActionType.Divide OrElse iActionType = UnidivNet.enActionType.Union AndAlso miCurrentActionType <> UnidivNet.enActionType.Union Then
			If iRowStatus <> enRowStatus.EndOfStage Then
				oDataRow.Item("RowStatus") = enRowStatus.EndOfStage
			End If
			oGridRow.DividerHeight = miAfterStageDividerHeight
		ElseIf iActionType = UnidivNet.enActionType.Divide AndAlso miCurrentActionType = UnidivNet.enActionType.Divide OrElse iActionType = UnidivNet.enActionType.Union AndAlso miCurrentActionType = UnidivNet.enActionType.Union Then
			oGridRow.DividerHeight = miAfterActionDividerHeight
		End If
	End Sub
	'Amnon
	'054-6474015 
	Private Sub zzInterpretJournal()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow = Nothing
		Dim tSourceParcelKey As UD_ParcelKey
		Dim iParcelDbID As Integer
		Dim oDestParcel As UnidivNet.UD_Parcel = Nothing
		Dim tDestParcelKey As UD_ParcelKey
		Dim iRowStatus As enRowStatus
		Dim oListDivided As List(Of UnidivNet.UD_Parcel) = Nothing
		Dim oParcelListDivided As List(Of UnidivNet.UD_Parcel) = Nothing
		Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing
		Dim oDestPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing
		Dim hsSourceBoundaryBranches As HashSet(Of Integer) = Nothing
		Dim hsParcelBoundaryBranches As HashSet(Of Integer)
		Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim colCancelledLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim colIsLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

		Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
		Dim sPrevParcelName As String = Nothing
		Dim dLegalArea As Double
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim bSourceParcelExists As Boolean
		Dim oPgonScheme As tsPolygon
		Dim oLastPrevRow As DataGridViewRow = Nothing
		Dim dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion) = Nothing
		Dim colForcedArea As System.Collections.ObjectModel.Collection(Of Double) = Nothing
		Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel) = Nothing
		Dim dForcedArea As Double
		Dim bHasForcedArea As Boolean

		oListDivided = New List(Of UnidivNet.UD_Parcel)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!StartInter", miLastRowIndex + 1, moMainTable.Rows.Count - 1)
		If miLastRowIndex = mlstInitParcels.Count - 1 Then
			'	DMCommon.Debug.MsgBox("11_212f", miLastRowIndex, Me.dgvMain.Rows.Count)
			If Me.dgvMain.Rows.Count > 1 Then
				oLastPrevRow = Me.dgvMain.Rows.Item(miLastRowIndex)
			End If
			If oLastPrevRow IsNot Nothing Then
				oLastPrevRow.DividerHeight = miAfterStageDividerHeight
			End If
		End If

		For iRowIndex As Integer = miLastRowIndex + 1 To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			miCurrentStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
			miCurrentAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
			miCurrentActionType = zzToActionType(oDataRow)
			iRowStatus = zzGetRowStatus(oDataRow)

			If miLastRowStatusIsNotDefined Then
				zzUpdateLastRowStatus()
				miLastRowStatusIsNotDefined = False
			End If
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Interpret", iRowIndex, miCurrentStage, miCurrentAction, miCurrentActionType, DMCommon.Functions.CIntN(oDataRow.Item("Oper")))
			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Default

				Case UnidivNet.enActionType.Divide
					iParcelDbID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID"))
					If iRowStatus = enRowStatus.StartOfAction Then
						tSourceParcelKey = zzParcelKeyFrom(oDataRow)
						oSourceParcel = zzGetParcelByDbID(oDataRow, True)
						bSourceParcelExists = oSourceParcel IsNot Nothing
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!DivA", iRowIndex, tSourceParcelKey.ParcelNo, tSourceParcelKey.Original, oSourceParcel.Name, tSourceParcelKey.Original)
						If Not bSourceParcelExists Then
							bSourceParcelExists = mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel)
						End If
						If tSourceParcelKey <> oSourceParcel.ParcelKey Then
							zzFillSourceParcel(oSourceParcel.ParcelKey, 0, oDataRow)
						End If

						dicParcelByFragments = New Dictionary(Of Integer, tsPgonUnion)()
						hsSourceBoundaryBranches = New HashSet(Of Integer)()
						colForcedArea = New ObjectModel.Collection(Of Double)()
						miCurrentActionFirstRowIndex = iRowIndex

					End If
					''''''''''''''''''''''''''''''''''''''''''DIVIDE BODY
					tDestParcelKey = zzParcelKeyTo(oDataRow)

					dForcedArea = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("ForcedArea"))
					If Not bHasForcedArea AndAlso dForcedArea > 0.0 Then
						bHasForcedArea = True
					End If
					colForcedArea.Add(dForcedArea)
					oPgonUnion = zzGetParcelPgonUnion(iParcelDbID, dicParcelByFragments)

					oPgonUnion.ParcelDbID = iParcelDbID
					oPgonUnion.ParcelKey = tDestParcelKey
					oPgonUnion.ForcedArea = dForcedArea
					mdicFragments.SetParcel(oPgonUnion.Polygons, tDestParcelKey)

					hsParcelBoundaryBranches = oPgonUnion.BoundaryBranches
					hsSourceBoundaryBranches.UnionWith(hsParcelBoundaryBranches)
					If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then

						colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

						zzGetTopoElements(hsSourceBoundaryBranches, colTopoLinks, colNodes)

						If bSourceParcelExists Then

							zzCreateDivideStageTopologyDB(colTopoLinks, mcolAllNodes, oListDivided, dicDestParcels, oSourceParcel.UD_Name, dicParcelByFragments)
							zzCreatePgonsPlus()
							For Each oPolygonScheme As tsPolygon In moCurrentStageTopoScheme.Polygons
								If Not oPolygonScheme.BorderObjID.IsNull AndAlso dicDestParcels.TryGetValue(oPolygonScheme.ID, oDestParcel) Then
									oDestParcel.BorderAcObjID = oPolygonScheme.BorderObjID
									oDestParcel.PolygonScheme = oPolygonScheme
								End If
							Next

							zzCalcDivideResParcelsNew(oSourceParcel, oListDivided, True)
							oSourceParcel.IsCanceled = True
							oSourceParcel.UpdateBlockAttributes()
						Else
							DMCommon.Debug.MsgBox("Datamap", "Parcel '" & tDestParcelKey.UD_ParcelName & "' was not founded")
						End If
						zzAddInputCellsLastDivide(miCurrentActionFirstRowIndex, iRowIndex)

					End If
				Case UnidivNet.enActionType.Union
					Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLinkCancelledLayer, DMAcadExt.DMApp.AppID, True, False)
					tSourceParcelKey = zzParcelKeyFrom(oDataRow)
					oSourceParcel = zzGetParcelByDbID(oDataRow, True)
					bSourceParcelExists = oSourceParcel IsNot Nothing
					If Not bSourceParcelExists Then
						bSourceParcelExists = mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel)
					End If
					If tSourceParcelKey <> oSourceParcel.ParcelKey Then
						zzFillSourceParcel(oSourceParcel.ParcelKey, 0, oDataRow)
					End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!DBUnion", oSourceParcel, tSourceParcelKey, bSourceParcelExists, oDataRow.Item("ParcelSourceDbID"))

					'	mdicParcels.DebugMsg()
					If bSourceParcelExists Then
						oPgonUnion = oSourceParcel.GetFragmentsPgonUnion(moFragmentsToposcheme)
						oSourceParcel.IsCanceled = True
						oSourceParcel.UpdateBlockAttributes()

						zzFillSourceParcel(oSourceParcel, oDataRow)
						If iRowStatus = enRowStatus.StartOfAction Then
							iParcelDbID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID"))
							tDestParcelKey = zzParcelKeyTo(oDataRow)
							oDestPgonUnion = oPgonUnion
							sPrevParcelName = oSourceParcel.UD_Name
							dLegalArea = oSourceParcel.LegalArea
							dicParcelByFragments = New Dictionary(Of Integer, tsPgonUnion)()
							miCurrentActionFirstRowIndex = iRowIndex
							oDataRow.Item("ActionName") = zzGetActionName(miCurrentActionType)
						Else
							oDestPgonUnion.AddPgonUnion(oPgonUnion)
							sPrevParcelName &= "," & oSourceParcel.UD_Name
							dLegalArea += oSourceParcel.LegalArea
						End If
					Else
						DMCommon.Debug.MsgBox("11_601ZZ", tSourceParcelKey.ToString())
						Exit Sub
					End If

					If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then

						hsParcelBoundaryBranches = oDestPgonUnion.BoundaryBranches

						colTopoLinks = moFragmentsToposcheme.GetBranchesObjIDs(oDestPgonUnion.BoundaryBranches)
						colCancelledLinks = moFragmentsToposcheme.GetBranchesObjIDs(oDestPgonUnion.NewInnerBranches)



						zzSetLayer(colCancelledLinks, msLinkCancelledLayer)
						zzGetTopoElements(oDestPgonUnion.NewInnerBranches, colCancelledLinks, colNodes)
						zzGetTopoElements(hsParcelBoundaryBranches, colTopoLinks, colNodes)

						If bSourceParcelExists Then
							oDestParcel = Nothing
							zzCreateUnionStageTopologyDB(colTopoLinks, mcolAllNodes, sPrevParcelName, dLegalArea, iParcelDbID, tDestParcelKey, oDestPgonUnion.Polygons, oDestParcel)
							zzCreatePgonsPlus()

							'''''''''' zzLoadDivideResParcels(oParcel)
							If oDestParcel IsNot Nothing Then
								zzCalcUnionResParcel(oDestParcel)
								oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oDestParcel.TopoID)

								oDestParcel.BorderAcObjID = oPgonScheme.BorderObjID
								' zzFragmentsAllocation(oPgonUnion)
							End If

						Else
							DMCommon.Debug.MsgBox("Unidiv", "Parcel '" & tDestParcelKey.UD_ParcelName & "' was not founded")
						End If
						mdicFragments.SetParcel(oDestPgonUnion.Polygons, oDestParcel.ParcelKey)
					End If

				Case UnidivNet.enActionType.Transfer
					iParcelDbID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID"))
					miCurrentActionFirstRowIndex = iRowIndex
					oGridRow = zzGetRow(iRowIndex)

					oDataRow.Item("ActionName") = zzGetActionName(miCurrentActionType)

					tSourceParcelKey = zzParcelKeyFrom(oDataRow)
					zzTransferFromFinalTopology(tSourceParcelKey, oDataRow, oGridRow, iRowIndex)
					zzInsertTransferCentroid(oDataRow)
			End Select

			If iRowStatus = enRowStatus.StartOfAction Then
				oDataRow.Item("StageCaption") = miCurrentStage
				oDataRow.Item("ActionName") = zzGetActionName(miCurrentActionType)

			ElseIf iRowStatus = enRowStatus.EndOfAction Then
				oGridRow = zzGetRow(iRowIndex)
				oGridRow.DividerHeight = miAfterActionDividerHeight
			ElseIf iRowStatus = enRowStatus.EndOfStage Then
				oGridRow = zzGetRow(iRowIndex)
				oGridRow.DividerHeight = miAfterStageDividerHeight
			End If

		Next
		miCurrentActionFirstRowIndex = -1
		miLastRowIndex = moMainTable.Rows.Count - 1 '''''''''''- 1
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		mbEventsEnabled = False
		Select Case miCurrentActionType
			Case UnidivNet.enActionType.Divide
				Me.rdbDivide.Checked = True
			Case UnidivNet.enActionType.Union
				Me.rdbUnion.Checked = True
			Case UnidivNet.enActionType.Transfer
				Me.rdbTransfer.Checked = True
		End Select

		mbEventsEnabled = bEventsEnabled
	End Sub

	Private Sub zzSetActionType()
		mbEventsEnabled = False
		Select Case miCurrentActionType
			Case UnidivNet.enActionType.Union
			Case UnidivNet.enActionType.Divide
			Case UnidivNet.enActionType.Transfer
		End Select

		mbEventsEnabled = True
	End Sub
	Private Function zzGetParcelPgonUnion(ByVal iParcelDbID As Integer, ByRef dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion)) As tsPgonUnion
		Dim sFilter As String = "ParcelDbID = " & Convert.ToString(iParcelDbID)
		Dim oDataView As Data.DataView = New DataView(moParcelFragmentsTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
		Dim hsFragments As HashSet(Of Integer) = New HashSet(Of Integer)
		Dim iFragmentDB_ID As Integer
		Dim iFragmentID As Integer
		Dim oPgonUnion As tsPgonUnion = New tsPgonUnion(moFragmentsToposcheme.Elements)
		Dim oPgonScheme As tsPolygon

		For Each oRow As DataRowView In oDataView
			iFragmentDB_ID = DMCommon.Functions.CIntN(oRow.Item("FragmentID"))
			iFragmentID = mdicFragments.GetDWGTopoID(iFragmentDB_ID)
			oPgonScheme = moFragmentsToposcheme.GetPolygon(iFragmentID)
			If oPgonScheme IsNot Nothing Then
				oPgonUnion.AddPolygon(oPgonScheme)
				If dicParcelByFragments.ContainsKey(iFragmentID) Then

					DMCommon.Functions.DispArray("Frgm=" & CStr(iFragmentID) & "," & CStr(iParcelDbID), dicParcelByFragments.Keys.ToArray())
				Else
					dicParcelByFragments.Add(iFragmentID, oPgonUnion)
				End If

			Else
				MessageBox.Show("Fragment " & iFragmentID.ToString() & " was not found", "", MessageBoxButtons.OK, MessageBoxIcon.Stop)
			End If
		Next

		Return oPgonUnion
	End Function
	Private Sub zzCloseLastRow()
		If moLastActionRow IsNot Nothing Then
			moLastActionRow.Item("RowStatus") = enRowStatus.EndOfStage
			moLastActionRow = Nothing
		End If
	End Sub

	Private Sub zzAddGridRow(iRowStatus As enRowStatus, bParcelIsSource As Boolean, Optional iParcelDbID As Integer = 0)
		If mbDatabound Then
			zzAddDataRow(iRowStatus, iParcelDbID, bParcelIsSource)
		Else
			Me.dgvMain.Rows.Add()
		End If
	End Sub
	Private Sub zzAddDataRowByB(iRowStatus As enRowStatus, iParcelDbID As Integer, Optional oNewRow As DataRow = Nothing)
		Dim bNewRow As Boolean = False
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		If oNewRow Is Nothing Then
			mbEventsEnabled = False
			oNewRow = moMainTable.NewRow
			bNewRow = True
		End If

		oNewRow.Item("ProjectCode") = miProjectCode
		oNewRow.Item("Detail") = miDetailNo
		oNewRow.Item("OriginalBlockNo") = miBlockNo
		oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo
		oNewRow.Item("ParcelDbID") = iParcelDbID


		oNewRow.Item("Stage") = miCurrentStage
		oNewRow.Item("Oper") = moMainTable.Rows.Count + 1
		' DMCommon.Debug.MsgBox("09_843", moMainTable.Rows.Count)
		oNewRow.Item("Action") = miCurrentAction
		oNewRow.Item("ActionType") = miCurrentActionType
		oNewRow.Item("RowStatus") = iRowStatus
		If bNewRow AndAlso oNewRow.RowState = DataRowState.Detached Then
			moMainTable.Rows.Add(oNewRow)
		End If
		mbEventsEnabled = bEventsEnabled
		''''''''''''''''  oNewRow.AcceptChanges()
	End Sub
	Private Sub zzAddDataRow(iRowStatus As enRowStatus, Optional oNewRow As DataRow = Nothing)
		Dim bNewRow As Boolean = False
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		If oNewRow Is Nothing Then
			mbEventsEnabled = False
			oNewRow = moMainTable.NewRow
			bNewRow = True
		End If

		oNewRow.Item("ProjectCode") = miProjectCode
		oNewRow.Item("Detail") = miDetailNo
		oNewRow.Item("PlanID") = miPlanID
		oNewRow.Item("Stage") = miCurrentStage
		oNewRow.Item("Oper") = moMainTable.Rows.Count + 1

		If miActionBatchFirstRowIndex >= 0 Then
			miCurrentAction = miBatchFirstAction + moMainTable.Rows.Count - miActionBatchFirstRowIndex
		End If
		oNewRow.Item("Action") = miCurrentAction
		oNewRow.Item("ActionType") = miCurrentActionType
		oNewRow.Item("RowStatus") = iRowStatus
		oNewRow.Item("ParcelDbID") = 0
		If bNewRow AndAlso oNewRow.RowState = DataRowState.Detached Then
			moMainTable.Rows.Add(oNewRow)
		End If
		mbEventsEnabled = bEventsEnabled
		''''''''''''''''  oNewRow.AcceptChanges()

	End Sub
	Private Sub zzAddDataRow(iRowStatus As enRowStatus, iParcelDbID As Integer, bParcelIsSource As Boolean, Optional oNewRow As DataRow = Nothing)
		Dim bNewRow As Boolean = False
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		If oNewRow Is Nothing Then
			mbEventsEnabled = False
			oNewRow = moMainTable.NewRow
			bNewRow = True
		End If

		oNewRow.Item("ProjectCode") = miProjectCode
		oNewRow.Item("Detail") = miDetailNo

		oNewRow.Item("PlanID") = miPlanID
		If bParcelIsSource Then
			oNewRow.Item("ParcelSourceDbID") = iParcelDbID
			oNewRow.Item("ParcelDbID") = 0
		Else
			oNewRow.Item("ParcelDbID") = iParcelDbID
		End If

		oNewRow.Item("Stage") = miCurrentStage
		oNewRow.Item("Oper") = moMainTable.Rows.Count + 1
		If miActionBatchFirstRowIndex >= 0 Then
			miCurrentAction = miBatchFirstAction + moMainTable.Rows.Count - miActionBatchFirstRowIndex
		Else
		End If
		oNewRow.Item("Action") = miCurrentAction
		oNewRow.Item("ActionType") = miCurrentActionType
		oNewRow.Item("RowStatus") = iRowStatus
		If bNewRow AndAlso oNewRow.RowState = DataRowState.Detached Then
			Try
				moMainTable.Rows.Add(oNewRow)
			Catch oEx As Exception
				DMCommon.Debug.UserMsg("Err #281", oEx.Message, iParcelDbID, bParcelIsSource)
			End Try

		End If
		mbEventsEnabled = bEventsEnabled

	End Sub
	Private Sub zzFillParcelDestRow(oParcel As UnidivNet.UD_Parcel, ByRef oDataRow As DataRow, Optional oGridRow As DataGridViewRow = Nothing) ' 
		zzFillParcelArea(oParcel.ParcelArea, True, oDataRow, oGridRow)
		oDataRow.Item("ParcelDbID") = oParcel.DbID
		oDataRow.Item("DestParcelNo") = oParcel.ParcelKey.ParcelNo
		oDataRow.Item("LanduseName") = oParcel.LanduseName
		oDataRow.Item("LotName") = oParcel.Lot
		oDataRow.Item("PlanName") = oParcel.Plan
		oDataRow.Item("BorderObjID") = oParcel.BorderAcObjID
		oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
	End Sub
	Private Sub zzFillParcelArea(tParcelArea As ParcelArea, bInputArea As Boolean, ByRef oDataRow As DataRow, Optional oGridRow As DataGridViewRow = Nothing) '  

		If bInputArea Then
			'	DMCommon.Debug.MsgBox("13_146K", oDataRow.Item("LegalArea"), tParcelArea.LegalAreaD, tParcelArea.LegalAreaM, tParcelArea.AcadAreaD, tParcelArea.ConditionalArea, tParcelArea.ForcedArea)
			oDataRow.Item("LegalArea") = tParcelArea.LegalAreaD
			oDataRow.Item("AcadArea") = tParcelArea.AcadAreaD
		End If

		oDataRow.Item("Tolerance") = tParcelArea.ToleranceD
		oDataRow.Item("DiffArea") = (tParcelArea.LegalAreaD - tParcelArea.AcadAreaD)
		oDataRow.Item("Deviation") = tParcelArea.DeviationD
		If tParcelArea.ForcedArea = 0.0 Then
			oDataRow.Item("ForcedArea") = DBNull.Value
			If oGridRow IsNot Nothing AndAlso oGridRow.Cells.Item("ctxLegalArea").Style IsNot Nothing Then
				oGridRow.Cells.Item("ctxLegalArea").Style = Nothing
			End If
		Else
			oDataRow.Item("ForcedArea") = tParcelArea.ForcedArea * 0.001
			oDataRow.Item("LegalArea") = tParcelArea.ConditionalArea * 0.001
			oGridRow.Cells.Item("ctxLegalArea").Style = moDataGridViewCellStyleAreaCanceled
		End If
		If tParcelArea.Deviation >= mdMinAreaDeviation AndAlso oGridRow IsNot Nothing Then
			oGridRow.Cells.Item("ctxDeviation").Style = moDataGridViewCellStyleAreaExcept
		ElseIf oGridRow IsNot Nothing AndAlso oGridRow.Cells.Item("ctxDeviation").Style IsNot Nothing Then
			oGridRow.Cells.Item("ctxDeviation").Style = Nothing
		End If
	End Sub
	Private Sub zzFillParcelArea(tParcelArea As ParcelArea, ByRef oDataRow As DataRow, ByRef oGridRow As DataGridViewRow)
		If mbDatabound Then
			zzFillParcelAreaDataRow(tParcelArea, oDataRow)
			If tParcelArea.Deviation > mdMinAreaDeviation Then
				oGridRow.Cells.Item("ctxDeviation").Style = moDataGridViewCellStyleAreaExcept
			ElseIf oGridRow.Cells.Item("ctxDeviation").Style IsNot Nothing Then
				oGridRow.Cells.Item("ctxDeviation").Style = Nothing
			End If
		Else
			zzFillParcelAreaGridRow(tParcelArea, oGridRow)
		End If
	End Sub
	Private Sub zzFillParcelAreaDataRow(tParcelArea As ParcelArea, ByRef oDataRow As DataRow)
		oDataRow.Item("LegalArea") = tParcelArea.LegalAreaD
		oDataRow.Item("AcadArea") = tParcelArea.AcadAreaD
		oDataRow.Item("Tolerance") = tParcelArea.ToleranceD
		oDataRow.Item("DiffArea") = (tParcelArea.LegalAreaD - tParcelArea.AcadAreaD)
		oDataRow.Item("Deviation") = tParcelArea.DeviationD
	End Sub
	Private Sub zzFillParcelAreaGridRow(tParcelArea As ParcelArea, ByRef oGridRow As DataGridViewRow)
		oGridRow.Cells.Item("ctxLegalArea").Value = tParcelArea.LegalAreaD
		oGridRow.Cells.Item("ctxArea").Value = tParcelArea.AcadAreaD
		oGridRow.Cells.Item("ctxTolerance").Value = tParcelArea.ToleranceD
		oGridRow.Cells.Item("ctxDiff").Value = (tParcelArea.LegalAreaD - tParcelArea.AcadAreaD)
		oGridRow.Cells.Item("ctxDeviation").Value = tParcelArea.DeviationD
	End Sub

	Private Sub zzFillSourceParcel(oParcel As UnidivNet.UD_Parcel, ByRef oDataRow As DataRow)
		Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey
		If tParcelKey.Original Then
			oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
			oDataRow.Item("NewParcelNo") = DBNull.Value
		Else
			oDataRow.Item("OriginalParcelNo") = DBNull.Value
			oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
		End If
		oDataRow.Item("ParcelSourceDbID") = oParcel.DbID
	End Sub

	Private Sub zzFillSourceParcel(tParcelKey As UD_ParcelKey, iDbID As Integer, ByRef oDataRow As DataRow)
		If tParcelKey.Original Then
			oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
			oDataRow.Item("NewParcelNo") = DBNull.Value
		Else
			oDataRow.Item("OriginalParcelNo") = DBNull.Value
			oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
		End If
		If iDbID > 0 Then
			oDataRow.Item("ParcelDbID") = iDbID
		End If

	End Sub

	Private Sub zzFillFromParcelKeyNew(oParcel As UnidivNet.UD_Parcel, ByRef oDataRow As DataRow, ByRef oGridRow As DataGridViewRow)
		Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey
		If mbDatabound Then
			If tParcelKey.Original Then
				oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
			Else
				oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
			End If
			oDataRow.Item("ParcelDbID") = oParcel.DbID
		Else
			If tParcelKey.Original Then
				oGridRow.Cells.Item("ctxFromParcel").Value = tParcelKey.ParcelNoStr
			Else
				oGridRow.Cells.Item("ctxFromParcelTemp").Value = tParcelKey.ParcelNoStr
			End If
		End If
	End Sub
	Private Sub zzFillFromParcelKey(oParcel As UnidivNet.UD_Parcel, ByRef oGridRow As DataGridViewRow)
		Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey

		If mbDatabound Then
			Dim oDataRow As DataRow = moMainTable.Rows.Item(oGridRow.Index)
			If tParcelKey.Original Then
				oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
				oDataRow.Item("NewParcelNo") = DBNull.Value
			Else
				oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
				oDataRow.Item("OriginalParcelNo") = DBNull.Value
			End If
			oDataRow.Item("ParcelSourceDbID") = oParcel.DbID
		Else
			If tParcelKey.Original Then
				oGridRow.Cells.Item("ctxFromParcel").Value = tParcelKey.ParcelNoStr
			Else
				oGridRow.Cells.Item("ctxFromParcelTemp").Value = tParcelKey.ParcelNoStr
			End If
		End If

	End Sub
	Private Sub zzFillToParcelKey(tParcelKey As UD_ParcelKey, ByRef oGridRow As DataGridViewRow)
		If mbDatabound Then
			Dim oDataRow As DataRow = moMainTable.Rows.Item(oGridRow.Index)

			oDataRow.Item("DestParcelNo") = tParcelKey.ParcelNo

		Else
			oGridRow.Cells.Item("ctxToParcel").Value = tParcelKey.ParcelNoStr
		End If
	End Sub
	Private Sub zzFillToParcelKey(tParcelKey As UD_ParcelKey, ByRef oDataRow As DataRow)
		If mbDatabound Then
			oDataRow.Item("DestParcelNo") = tParcelKey.ParcelNo
		End If
	End Sub
	Private Function zzGetSourceParcelDbID(oGridRow As System.Windows.Forms.DataGridViewRow) As Integer
		Dim oDataRowView As DataRowView
		Dim iDestBlockAddNo As Integer = 0
		Dim iResParcelDbID As Integer = 0
		oDataRowView = TryCast(oGridRow.DataBoundItem, DataRowView)
		If oDataRowView IsNot Nothing Then
			iResParcelDbID = DMCommon.Functions.CIntN(oDataRowView.Item("ParcelDbID"))
		End If
		Return iResParcelDbID
	End Function

	Private Function zzGetSourceParcelKey(oGridRow As System.Windows.Forms.DataGridViewRow) As UD_ParcelKey
		Dim oDataRowView As DataRowView
		Dim tResParcelKey As UD_ParcelKey
		'   Dim oDataRow As DataRow
		Dim iStageNo As Integer
		Dim iDestBlockNo As Integer
		Dim iDestBlockAddNo As Integer = 0
		oDataRowView = TryCast(oGridRow.DataBoundItem, DataRowView)
		If oDataRowView IsNot Nothing Then
			iStageNo = DMCommon.Functions.CIntN(oDataRowView.Item("Stage"))
			If iStageNo = 0 Then
				tResParcelKey = UD_ParcelKey.FromGrid(miBlockNo, miBlockAddNo, oDataRowView.Item("OriginalParcelNo"), oDataRowView.Item("NewParcelNo"))
			Else
				iDestBlockNo = DMCommon.Functions.CIntN(oDataRowView.Item("DestBlockNo"))
				iDestBlockAddNo = DMCommon.Functions.CIntN(oDataRowView.Item("DestBlockAddNo"))

				If iDestBlockNo = 0 Then
					iDestBlockNo = miBlockNo
					iDestBlockAddNo = miBlockAddNo
				End If
				tResParcelKey = UD_ParcelKey.FromGrid(iDestBlockNo, iDestBlockAddNo, DBNull.Value, oDataRowView.Item("DestParcelNo"))
			End If
		End If
		Return tResParcelKey
	End Function
	Private Function zzParcelDbID(oDataRow As DataRow, bParcelIsSource As Boolean) As Integer
		Dim iStage As Integer = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
		If iStage = 0 OrElse Not bParcelIsSource Then
			Return DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID"))
		Else
			Return DMCommon.Functions.CIntN(oDataRow.Item("ParcelSourceDbID"))
		End If
	End Function
	Private Function zzGetParcelByDbID(oDataRow As DataRow, bParcelIsSource As Boolean) As UnidivNet.UD_Parcel
		Dim iDbID As Integer = zzParcelDbID(oDataRow, bParcelIsSource)
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		If iDbID <> 0 Then
			mdicParcels.TryGetValueByDbID(iDbID, oParcel)
			If oParcel Is Nothing Then
				DMCommon.Debug.MsgBox("Err #1525", mdicParcels.Count, bParcelIsSource, oDataRow.Item("ParcelDbID"), oDataRow.Item("ParcelSourceDbID"))
				For Each oParcel In mdicParcels.Values
					DMCommon.Debug.ExcelLog.SetNextValue(0, "Parcels", oParcel.UD_Name, oParcel.IsCanceled, oParcel.DbID, oParcel.PreviousParcelsStr)
				Next
			End If
		Else
			DMCommon.Debug.MsgBox("Err #1526", mdicParcels.Count, bParcelIsSource, oDataRow.Item("Stage"), oDataRow.Item("ParcelDbID"), oDataRow.Item("ParcelSourceDbID"))
		End If

		Return oParcel
	End Function
	Private Function zzGetParcelByDbIDOrKey(oDataRow As DataRow, bParcelIsSource As Boolean) As UnidivNet.UD_Parcel
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim iDbID As Integer = zzParcelDbID(oDataRow, bParcelIsSource)

		If iDbID = 0 Then
			Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockNo, miBlockAddNo, oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
			If tParcelKey.Exists AndAlso mdicParcels.TryGetValue(tParcelKey, oParcel) Then
				oDataRow.Item("ParcelSourceDbID") = oParcel.DbID
			End If
			Return oParcel
		Else
			mdicParcels.TryGetValueByDbID(iDbID, oParcel)
		End If
		Return oParcel
	End Function
	Private Function zzParcelKeyFrom(oDataRow As DataRow) As UD_ParcelKey
		Return UD_ParcelKey.FromGrid(miBlockNo, miBlockAddNo, oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
	End Function
	Private Function zzParcelKeyFrom(iColumnIndex As Integer, oValue As System.Object) As UD_ParcelKey
		Dim iParcelNo As Integer
		Dim bIsOriginal As Boolean
		If iColumnIndex = miFromParcelColIndex Then
			bIsOriginal = True
		End If
		Dim sValue As String = TryCast(oValue, String)
		If Integer.TryParse(sValue, iParcelNo) Then
			Return New UD_ParcelKey(miBlockNo, miBlockAddNo, iParcelNo, bIsOriginal)
		End If
	End Function

	Private Function zzParcelKeyTo(oDataRow As DataRow) As UD_ParcelKey
		Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo"))
		Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("DestBlockNo"))
		Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("DestBlockAddNo"))

		If iBlockNo = 0 Then
			iBlockNo = miBlockNo
			iBlockAddNo = miBlockAddNo
		End If
		Return New UD_ParcelKey(iBlockNo, iBlockAddNo, iParcelNo, False)
	End Function

	Private Function zzParcelFrom(oDataRow As DataRow) As UnidivNet.UD_Parcel
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(zzParcelKeyFrom(oDataRow), oParcel) Then
			Return oParcel
		Else
			Return Nothing
		End If

	End Function

	Private Function zzGetParcelTo(oDataRow As DataRow) As UnidivNet.UD_Parcel
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(zzParcelKeyTo(oDataRow), oParcel) Then
			Return oParcel
		Else
			Return Nothing
		End If
	End Function


	Private Sub zzLoadDWG()
		miCurrentAction = 1
		miCurrentActionType = UnidivNet.enActionType.Registered

		DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

		UnidivNet.Ud_ScriptODTable.EraseTable(UnidivNet.Ud_ScriptODTable.ODTableName)

		zzCreateScriptODTableNew()
		zzBeforeLoadParcels()
		If miBlockNo <> 0 Then
			zzGetPlanIDs()
		Else
			DMCommon.Debug.UserMsg("Err #274", DMCommon.dmMessages.Message(324))
		End If


	End Sub
	Private Function zzGetNewParcelBlocks() As ObjectIdCollection
		Dim sBlockLayers As String = msCentroidBlockName & "," & zzGetCentroidLayer()
		Return DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, sBlockLayers)
	End Function

	Private Sub zzMyInitializeComponent()
		moDataGridViewCellStyleArea.Format = "N3"
		moDataGridViewCellStyleArea.NullValue = "" ' Nothing
		Me.ctxArea.DefaultCellStyle = moDataGridViewCellStyleArea
		Me.ctxForcedArea.DefaultCellStyle = moDataGridViewCellStyleArea
		Me.ctxDiff.DefaultCellStyle = moDataGridViewCellStyleArea
		moDataGridViewCellStyleTolerance.Format = "N4"
		moDataGridViewCellStyleTolerance.NullValue = Nothing
		Me.ctxTolerance.DefaultCellStyle = moDataGridViewCellStyleTolerance

		moDataGridViewCellStyleInput = Me.dgvMain.DefaultCellStyle.Clone
		moDataGridViewCellStyleInput.BackColor = Color.White
		moDataGridViewCellStyleInput.ForeColor = Color.DarkBlue
		moDataGridViewCellStyleInputSaved = Me.dgvMain.DefaultCellStyle.Clone
		moDataGridViewCellStyleInputSaved.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))

		moDataGridViewCellStyleInputSavedGush = Me.dgvMain.DefaultCellStyle.Clone
		moDataGridViewCellStyleInputSavedGush.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))



		moDataGridViewCellStyleAreaExcept = moDataGridViewCellStyleArea.Clone
		moDataGridViewCellStyleAreaExcept.BackColor = System.Drawing.Color.FromArgb(CType(CType(255 - 32, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
		moDataGridViewCellStyleAreaExcept.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer))
		moDataGridViewCellStyleAreaCanceled = moDataGridViewCellStyleArea.Clone
		moDataGridViewCellStyleAreaCanceled.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer))

		moDataGridViewCellStyleGush = Me.dgvMain.DefaultCellStyle.Clone
		moDataGridViewCellStyleGush.Font = New System.Drawing.Font("Tahoma", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))


		Me.dgvMain.Columns.Item("ctxFromParcel").DefaultCellStyle = moDataGridViewCellStyleInputSaved
		Me.dgvMain.Columns.Item("ctxFromParcelTemp").DefaultCellStyle = moDataGridViewCellStyleInputSaved
		Me.dgvMain.Columns.Item("ctxToParcel").DefaultCellStyle = moDataGridViewCellStyleInputSaved
		Me.dgvMain.Columns.Item("ctxToGush").DefaultCellStyle = moDataGridViewCellStyleGush
		Me.dgvMain.Columns.Item("ctxToGushAdd").DefaultCellStyle = moDataGridViewCellStyleGush


		Me.dgvMain.AutoGenerateColumns = False
		midgvMainLocationY = Me.dgvMain.Location.Y

		zzUpdateCurrentScale()

	End Sub

	Private Sub frmUnidiv_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles Me.FormClosed
		If mdicParcels IsNot Nothing Then
			mdicFragments = Nothing
		End If
		If moFragmentTable IsNot Nothing Then
			moFragmentTable.Rows.Clear()
			moFragmentTable.Dispose()
			moFragmentTable = Nothing
		End If
		If moFragmentsDataAdapter IsNot Nothing Then
			moFragmentsDataAdapter.Dispose()
			moFragmentsDataAdapter = Nothing
		End If

		DMAcadExt.AcadDocument.RestoreVarCmdDia()
		DMAcadExt.AcadDocument.ClearDrawVectorSet()
		zzCloseDWG()
	End Sub

	Private Sub frmUnidiv_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If mbDirty Then
			Dim sMsg As String = DMCommon.dmMessages.Message(314)

			If System.Windows.Forms.MessageBox.Show(sMsg, "Datamap", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, MessageBoxOptions.RightAlign) = Windows.Forms.DialogResult.No Then

				e.Cancel = True
			End If
		End If

	End Sub '
	Private Sub zzInit()
		msCentroidBlockName = UnidivNet.UD_Parcel.BlockName
		msTabaCentroidBlockName = UnidivNet.UD_Parcel.LotBlockName

		AcadReport.RepApp.InitDWGScaleFactor()
		TopoManager.TopoScheme.tsNode.IsGeoVertex = False

	End Sub

	Private Sub zzInitBlocks()
		UnidivNet.UD_Point.Init()
		UnidivNet.UD_FLine.Init()
		UnidivNet.UD_Parcel.InitAcadBlock()
	End Sub
	Private Sub zzMainLoad()
		Dim bFragmentsExist As Boolean
		If miPlanType <> frmUD_General.enPlanType.PlanType1 Then
			bFragmentsExist = True
		End If
		zzOpenDWG(, bFragmentsExist)
		zzInitBlocks()
		zzLoadDWG()

		zzCheckHanitObjects()

		If miPlanID <> 0 Then
			zzLoadByPlanID()
		Else
			zzCloseDWG()
		End If

		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
		If DMCommon.Debug.Debug Then
			Me.Button3.Visible = True
			Me.dgvMain.Columns.Item("ctxActionNo").Visible = True
			Me.dgvMain.Columns.Item("ctxActionType").Visible = True
			Me.dgvMain.Columns.Item("ctxRowStatus").Visible = True
			Me.dgvMain.Columns.Item("ctxStageNo").Visible = True
			Me.dgvMain.Columns.Item("ctxParcelDbID").Visible = True
			Me.dgvMain.Columns.Item("ctxParcelSourceDbID").Visible = True
			Me.dgvMain.Columns.Item("ctxOper").Visible = True

		End If
		mbEventsEnabled = True
	End Sub

	Private Sub zzLoadByPlanID()
		Dim bInputDWG As Boolean = True
		Dim bDocIsNotLocked As Boolean = Not DMAcadExt.AcadDocument.IsLocked
		If bDocIsNotLocked Then
			zzOpenDWG()
		End If
		zzOpenDB(False)

		zzLoadDBPoints()
		zzLoadProjectPlanDataTable()
		zzLoadFragments()
		zzCalcStartPointNum()

		zzLoadParcelTopology()

		zzCheckFragments()
		zzSetDbIDs()

		Me.cmdLoadDBData.Enabled = mdicFragments.DBVersionIsCorrect
		Me.cmdLoadByAction.Enabled = mdicFragments.DBVersionIsCorrect
		Me.cmdLoadByStage.Enabled = mdicFragments.DBVersionIsCorrect

		zzOpenJournalTableSchema()
		zzCalcJournalAddFields(True)

		If Not bInputDWG Then
			zzCalcInitGrid()

			If mdicFragments.DBVersionIsCorrect Then
				zzInterpretJournal()
			Else
				DMCommon.Debug.MsgBox("09_655k", "DBVersionIsNotCorrect", moMainTable.Rows.Count, moMainTable.Columns.Count, mdicParcels.Count)

			End If
		End If

		If bInputDWG AndAlso mdicParcels IsNot Nothing Then

			zzFillInitGrid()
		End If

		zzCloseDWG()

		mbEventsEnabled = True
	End Sub
	Private Sub frmUnidiv_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		zzMainLoad()

	End Sub


	Private Sub zzGetPlanIDs()
		Const sSPName As String = "GetPlanID"

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sSPName, CommandType.StoredProcedure, zzGetParametersByBlock())

		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read
					miPlanID = oDataReader.GetInt32(0)
					Me.cmbPlanID.Items.Add(miPlanID)
				Loop
			End If
			oDataReader.Close()
		End If
		If Me.cmbPlanID.Items.Count = 1 Then
			Me.cmbPlanID.SelectedItem = miPlanID
			Me.cmdSaveDB.Enabled = True
		Else
			Me.cmdSaveDB.Enabled = False
			miPlanID = 0
		End If

	End Sub
	Public Sub New(oAppWin As Autodesk.AutoCAD.Windows.Window)

		InitializeComponent()
		moAppWin = oAppWin


		zzMyInitializeComponent()
		zzInitUD_Project()
		zzLoadProjectDataTable()
		zzInit()
	End Sub
	Private Sub zzInitUD_Project()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True, False)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub

	Private Sub zzSetActionEnabled(bEnabled As Boolean)

		Me.cmdInsertTable.Enabled = bEnabled
		Me.cmdCancelAction.Enabled = bEnabled
	End Sub
	Private Sub zzOpenAction(bNewStage As Boolean, iActionType As UnidivNet.enActionType)
		Dim oLastPrevRow As DataGridViewRow = Nothing
		Dim iLastRowStatus As enRowStatus
		Dim iPrevRowDividerHeight As Integer
		Dim iFirstRowStatus As enRowStatus
		Dim bEventsEnabled As Boolean = mbEventsEnabled
		mbEventsEnabled = False
		If moLastActionRow IsNot Nothing Then
			iLastRowStatus = zzGetRowStatus(moLastActionRow)
			If iLastRowStatus <> enRowStatus.Transfer Then
				If bNewStage Then
					moLastActionRow.Item("RowStatus") = enRowStatus.EndOfStage
				Else
					moLastActionRow.Item("RowStatus") = enRowStatus.EndOfAction
				End If
			End If

		End If
		If iActionType = UnidivNet.enActionType.Transfer Then
			iFirstRowStatus = enRowStatus.Transfer
		Else
			iFirstRowStatus = enRowStatus.StartOfAction
		End If
		If bNewStage Then
			miCurrentActionType = iActionType
			miCurrentAction = 1
			miCurrentStage += 1
			iPrevRowDividerHeight = miAfterStageDividerHeight
		Else
			miCurrentAction += 1
			iPrevRowDividerHeight = miAfterActionDividerHeight

		End If



		zzAddGridRow(iFirstRowStatus, False)
		zzSetScriptData(False)
		Me.dgvMain.DataSource = moMainTable
		If Me.dgvMain.Rows.Count > 1 Then
			oLastPrevRow = Me.dgvMain.Rows.Item(Me.dgvMain.Rows.Count - 2)
		End If
		If oLastPrevRow IsNot Nothing Then
			oLastPrevRow.DividerHeight = iPrevRowDividerHeight
		End If

		miCurrentActionFirstRowIndex = Math.Min(Me.dgvMain.RowCount, moMainTable.Rows.Count) - 1
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
		Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim sActionName As String
		oGridRow.Cells.Item("ctxFromParcel").Style = moDataGridViewCellStyleInput
		oGridRow.Cells.Item("ctxFromParcelTemp").Style = moDataGridViewCellStyleInput
		zzSetNewParcelNo()

		Select Case miCurrentActionType
			Case UnidivNet.enActionType.Union
				sActionName = msUnionActionName
			Case UnidivNet.enActionType.Divide
				sActionName = msDivideActionName
			Case UnidivNet.enActionType.Transfer
				sActionName = msTransferActionName
			Case Else
				sActionName = String.Empty
		End Select

		If mbDatabound Then
			If bNewStage Then
				oFirstActionRow.Item("StageCaption") = miCurrentStage
			End If
			oFirstActionRow.Item("ActionName") = sActionName
		Else
			If bNewStage Then
				oGridRow.Cells.Item("ctxStage").Value = miCurrentStage
			End If
			oGridRow.Cells.Item("ctxAction").Value = sActionName
		End If
		If miCurrentActionType <> UnidivNet.enActionType.Transfer Then
			zzFillToParcelKey(mtCurrentParcelKey, oFirstActionRow)
		End If
		zzClearInputCellsForStage()
		zzAddInputCellsFirstRow(miCurrentActionFirstRowIndex)

		If miCurrentActionType = UnidivNet.enActionType.Union Then 'OrElse miCurrentActionType = UnidivNet.enActionType.Transfer
			zzAllowUserToAddDelete(True)
			zzAddInputCellsNextRow(miCurrentActionFirstRowIndex + 1)
		End If

		mbEventsEnabled = bEventsEnabled
	End Sub
	Private Function zzGetActionName(iActionType As UnidivNet.enActionType) As String
		Select Case iActionType
			Case UnidivNet.enActionType.Registered
				Return "מ' רשום"
		Case UnidivNet.enActionType.Union
				Return "איחוד"
			Case UnidivNet.enActionType.Divide
				Return "חלוקה"
			Case UnidivNet.enActionType.Transfer
				Return "העברה"
			Case Else
				Return String.Empty
		End Select
	End Function
	Private Sub zzOpenActionBatch()

		miActionBatchFirstRowIndex = moMainTable.Rows.Count - 1
		miBatchFirstAction = miCurrentAction
	End Sub
	Private Sub zzCloseActionBatch()
		miActionBatchFirstRowIndex = -1
		miBatchFirstAction = -1
	End Sub
	Private Sub zzSetNewParcelNo()
		Dim iInputParcel As Integer
		Dim iLastParcel As Integer = mdicParcels.GetMaxTempParcelNo()

		If Integer.TryParse(Me.txtLastParcel.Text, iInputParcel) Then
			iLastParcel = Math.Max(iInputParcel, iLastParcel)
		End If
		mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, miBlockAddNo, iLastParcel + 1, False)

	End Sub
	Private Sub zzUpdateCurrentParcelKey(tNewParcelKey As UD_ParcelKey)

		If tNewParcelKey.ParcelNo >= mtCurrentParcelKey.ParcelNo Then
			mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, miBlockAddNo, tNewParcelKey.ParcelNo + 1, False)
		End If
	End Sub

	Private Sub zzSetNewPointNoOld()
		Dim iInputPoint As Integer
		If Integer.TryParse(Me.txtStartPoint.Text, iInputPoint) Then

			mdicPoints.StartNewNum = iInputPoint
		End If
	End Sub
	Private Sub zzSetNewPointNo()
		Dim iInputPoint As Integer
		If Integer.TryParse(Me.txtStartPoint.Text, iInputPoint) Then

			mdicPoints.StartNewNum = iInputPoint
		End If

	End Sub

	Private Sub zzCloseAction()
		DMAcadExt.AcadDocument.ClearDrawVectorSet()
		mtCurrentScriptData.Close()
		'  Me.dgvMain.AllowUserToAddRows = False
		zzAllowUserToAddDelete(False)
		Select Case miCurrentActionType
			Case UnidivNet.enActionType.Divide
				Me.rdbUnion.Enabled = True
			Case UnidivNet.enActionType.Union
				Me.rdbDivide.Enabled = True
		End Select
		Me.rdbTransfer.Enabled = True
		miLastActionFirstRowIndex = miCurrentActionFirstRowIndex
		miCurrentActionFirstRowIndex = -1
		mhsSelectedParcels.Clear()
		mlstSelectedParcelKeys.Clear()
		mlstSelectedParcels.Clear()
		zzClearInputCellsForStage()
		zzAddInputCellsLastAction()
		mtCurrentParcelKey = New UD_ParcelKey()
		Me.cmdLoadScript.Enabled = False
		mbDirty = True

	End Sub
	Private Sub zzRecalcDivideAllResParcels()
		Const dRoundFactor As Double = 1.0
		Dim oDataRow As DataRow
		Dim iCurrentActionType As UnidivNet.enActionType
		Dim iCurrentStage As Integer = 0
		Dim iCurrentAction As Integer = 0
		Dim iParcelNo As Integer
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim oResParcel As UnidivNet.UD_Parcel = Nothing
		Dim oBalanceArea As TopoManager.BalanceArea
		Dim oBalanceConditionalArea As TopoManager.BalanceArea
		Dim daSourceArea() As Double
		Dim daSourceConditionalArea() As Double
		Dim colSourceArea As System.Collections.ObjectModel.Collection(Of Double) = New ObjectModel.Collection(Of Double)()
		Dim colSourceConditionalArea As System.Collections.ObjectModel.Collection(Of Double) = New ObjectModel.Collection(Of Double)()
		Dim lstResParcels As List(Of UnidivNet.UD_Parcel) = New List(Of UnidivNet.UD_Parcel)()

		Dim colFreeAreaIndecis As System.Collections.ObjectModel.Collection(Of Integer) = New ObjectModel.Collection(Of Integer)()
		Dim colForcedAreaIndecis As System.Collections.ObjectModel.Collection(Of Integer) = New ObjectModel.Collection(Of Integer)()

		Dim oGridRow As DataGridViewRow = Nothing

		Dim iFreeAreaIndex As Integer = -1
		Dim dSourceArea As Double
		Dim dSourceConditionalArea As Double

		Dim dForcedArea As Double
		Dim dResArea As Double
		Dim iRowStatus As enRowStatus
		Dim iActionFirstRowIndex As Integer
		Dim tSourceParcelKey As UD_ParcelKey
		Dim iSourceDbID As Integer
		Dim tPlanExt As PlanExt

		Dim dicRenamedParcels As Dictionary(Of UD_ParcelKey, Integer) = New Dictionary(Of UD_ParcelKey, Integer)()
		Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelComparer = New UnidivNet.UD_Parcels.ParcelComparer()
		zzOpenDWG()
		For iRowIndex As Integer = 0 To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			oGridRow = zzGetRow(iRowIndex)
			iCurrentStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
			iCurrentAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
			iCurrentActionType = zzToActionType(oDataRow)
			iRowStatus = zzGetRowStatus(oDataRow)
			If iCurrentActionType = UnidivNet.enActionType.Divide Then
				If iRowStatus = enRowStatus.StartOfAction Then
					iActionFirstRowIndex = iRowIndex
					oSourceParcel = zzParcelFrom(oDataRow)
					dSourceArea = oSourceParcel.ParcelArea.LegalArea
					dSourceConditionalArea = dSourceArea
					colSourceArea.Clear()
					colSourceConditionalArea.Clear()
					lstResParcels.Clear()
					colFreeAreaIndecis.Clear()
					iFreeAreaIndex = -1
				End If

				oResParcel = zzGetParcelByDbID(oDataRow, False)
				dForcedArea = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("ForcedArea"))
				oResParcel.SetForcedArea(dForcedArea)
				tPlanExt = New PlanExt(oDataRow)
				oResParcel.SetPlanData(tPlanExt.LotName, tPlanExt.LanduseName, tPlanExt.Plan)
				'@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
				iParcelNo = DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo"))
				If oResParcel.ParcelKey.ParcelNo <> iParcelNo Then
					mdicParcels.RemoveParcel(oResParcel)
					dicRenamedParcels.Add(oResParcel.ParcelKey, oResParcel.DbID)
					oResParcel.Rename(iParcelNo)

				End If

				zzUpdateCurrentParcelKey(oResParcel.ParcelKey)
				lstResParcels.Add(oResParcel)

				colSourceConditionalArea.Add(oResParcel.AcadArea)
				If dForcedArea = 0.0 Then
					iFreeAreaIndex += 1

					colSourceArea.Add(oResParcel.AcadArea)

					colFreeAreaIndecis.Add(iRowIndex)
				Else
					dSourceArea -= dForcedArea
					oResParcel.CalculateArea(dForcedArea)
					colForcedAreaIndecis.Add(iRowIndex)
					'''''''''''''''''''''07/03	zzFillParcelArea(oResParcel.ParcelArea, True, oDataRow, oGridRow)
				End If

				If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
					If colSourceArea.Count > 0 Then
						daSourceArea = colSourceArea.ToArray()
						oBalanceArea = New TopoManager.BalanceArea(daSourceArea, dRoundFactor, dSourceArea, True, "Ud")
						daSourceConditionalArea = colSourceConditionalArea.ToArray()

						oBalanceConditionalArea = New TopoManager.BalanceArea(daSourceConditionalArea, dRoundFactor, dSourceConditionalArea, True, "Ud")
						For iIndex As Integer = 0 To iFreeAreaIndex
							''''''''''''''''''oDataRow = moMainTable.Rows.Item(iaFreeAreaIndecis(iIndex))
							oDataRow = moMainTable.Rows.Item(colFreeAreaIndecis.Item(iIndex))

							oGridRow = zzGetRow(colFreeAreaIndecis.Item(iIndex))
							'DMCommon.Debug.MsgBox("09_800K")
							dResArea = oBalanceArea.OutputItemFloat(iIndex)
							oResParcel = lstResParcels.Item(colFreeAreaIndecis.Item(iIndex) - iActionFirstRowIndex)
							oResParcel.CalculateArea(dResArea)

							zzFillParcelArea(oResParcel.ParcelArea, True, oDataRow, oGridRow)
						Next

						For iIndex As Integer = 0 To lstResParcels.Count - 1
							''''''''''''''''''oDataRow = moMainTable.Rows.Item(iaFreeAreaIndecis(iIndex))
							oResParcel = lstResParcels.Item(iIndex)
							oDataRow = moMainTable.Rows.Item(iActionFirstRowIndex + iIndex)

							If oResParcel._ParcelArea.ForcedArea <> 0 Then
								dResArea = oBalanceConditionalArea.OutputItemFloat(iIndex)
								oResParcel.SetConditionalArea(dResArea)
							End If


							oGridRow = zzGetRow(iActionFirstRowIndex + iIndex)


							zzFillParcelArea(oResParcel.ParcelArea, True, oDataRow, oGridRow)
						Next


						lstResParcels.Sort(oParcelComparer)
						For iIndex As Integer = 0 To lstResParcels.Count - 1
							''''''''''''''''''oaResParcels(iIndex).UpdateBlockAttributes()
							If Not mdicParcels.ContainsKey(lstResParcels.Item(iIndex).ParcelKey) Then
								mdicParcels.AddParcel(lstResParcels.Item(iIndex))
							End If
							oGridRow = Me.dgvMain.Rows.Item(iActionFirstRowIndex + iIndex)
							zzFillParcelDestRow(lstResParcels.Item(iIndex), moMainTable.Rows.Item(iActionFirstRowIndex + iIndex), oGridRow)
							'''''''''''''''''''''''colResParcels.Item(iIndex).UpdateBlockAttributes()
							lstResParcels.Item(iIndex).UpdateBlockAttributes()

						Next

					End If
					oSourceParcel = Nothing
				End If
			ElseIf iCurrentActionType = UnidivNet.enActionType.Union Then

				If iRowStatus = enRowStatus.StartOfAction Then
					dResArea = 0.0
					'oResParcel = zzGetParcelTo(oDataRow)
					oResParcel = zzGetParcelByDbID(oDataRow, False)
					iActionFirstRowIndex = iRowIndex
					lstResParcels.Clear()
				End If
				tSourceParcelKey = zzParcelKeyFrom(oDataRow)
				If dicRenamedParcels.TryGetValue(tSourceParcelKey, iSourceDbID) Then
					If mdicParcels.TryGetValueByDbID(iSourceDbID, oSourceParcel) Then
						zzFillSourceParcel(oSourceParcel.ParcelKey, 0, oDataRow)
					End If
				Else
					oSourceParcel = zzParcelFrom(oDataRow)
				End If

				dResArea += oSourceParcel.LegalArea
				lstResParcels.Add(oSourceParcel)
				If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage AndAlso oResParcel IsNot Nothing Then
					oResParcel.CalculateArea(dResArea)

					oGridRow = Me.dgvMain.Rows.Item(iActionFirstRowIndex)
					zzFillParcelArea(oResParcel.ParcelArea, True, moMainTable.Rows.Item(iActionFirstRowIndex), oGridRow)
					lstResParcels.Sort(oParcelComparer)
					For iIndex As Integer = 0 To lstResParcels.Count - 1
						zzFillSourceParcel(oSourceParcel.ParcelKey, 0, oDataRow)
						lstResParcels.Item(iIndex).UpdateBlockAttributes()

					Next
				End If
			ElseIf iCurrentActionType = UnidivNet.enActionType.Transfer Then

				tSourceParcelKey = zzParcelKeyFrom(oDataRow)
				If dicRenamedParcels.TryGetValue(tSourceParcelKey, iSourceDbID) Then
					If mdicParcels.TryGetValueByDbID(iSourceDbID, oSourceParcel) Then
						zzFillSourceParcel(oSourceParcel.ParcelKey, 0, oDataRow)
					End If
				Else
					oSourceParcel = zzParcelFrom(oDataRow)
				End If


			End If
		Next
		zzCloseDWG()

	End Sub
	Private Sub zzRecalcDivideResParcels()
		Const dRoundFactor As Double = 1.0
		Dim oDataRow As DataRow
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim oDestParcel As UnidivNet.UD_Parcel
		Dim oBalanceArea As TopoManager.BalanceArea
		Dim iParcelsUB As Integer = moMainTable.Rows.Count - miLastActionFirstRowIndex - 1
		Dim daSourceArea(iParcelsUB) As Double
		Dim oaResParcels(iParcelsUB) As UnidivNet.UD_Parcel
		Dim iaFreeAreaIndecis(iParcelsUB) As Integer
		Dim iFreeAreaIndex As Integer = -1
		Dim dSourceArea As Double
		Dim dForcedArea As Double
		Dim dResArea As Double

		For iRowIndex As Integer = miLastActionFirstRowIndex To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			If oSourceParcel Is Nothing Then
				oSourceParcel = zzParcelFrom(oDataRow)
				dSourceArea = oSourceParcel.ParcelArea.LegalArea * 1000.0
			End If
			oDestParcel = zzGetParcelTo(oDataRow)
			oaResParcels(iRowIndex - miLastActionFirstRowIndex) = oDestParcel
			dForcedArea = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("ForcedArea"))

			If dForcedArea = 0.0 Then
				iFreeAreaIndex += 1
				iaFreeAreaIndecis(iFreeAreaIndex) = iRowIndex
				daSourceArea(iFreeAreaIndex) = oDestParcel.AcadArea

			Else
				dSourceArea -= dForcedArea
				oDestParcel.CalculateArea(dForcedArea)
				zzFillParcelArea(oDestParcel.ParcelArea, True, oDataRow)
			End If

		Next

		ReDim Preserve daSourceArea(iFreeAreaIndex)
		oBalanceArea = New TopoManager.BalanceArea(daSourceArea, dRoundFactor, dSourceArea, True, "Ud")
		For iIndex As Integer = 0 To iFreeAreaIndex
			oDataRow = moMainTable.Rows.Item(iaFreeAreaIndecis(iIndex))
			dResArea = oBalanceArea.OutputItemFloat(iIndex)
			oaResParcels(iaFreeAreaIndecis(iIndex) - miLastActionFirstRowIndex).CalculateArea(dResArea * 0.001)
			zzFillParcelArea(oaResParcels(iaFreeAreaIndecis(iIndex) - miLastActionFirstRowIndex).ParcelArea, True, oDataRow)
		Next
		zzOpenDWG()
		For iIndex As Integer = 0 To oaResParcels.GetUpperBound(0)
			oaResParcels(iIndex).UpdateBlockAttributes()
		Next
		zzCloseDWG()
	End Sub
	Private Sub zzCalcDivideResParcelsNew(oSourceParcel As UnidivNet.UD_Parcel, oParcelList As List(Of UnidivNet.UD_Parcel), bIsDBSource As Boolean)
		Const dRoundFactor As Double = 1.0
		Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelComparer = New UnidivNet.UD_Parcels.ParcelComparer()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oBalanceArea As TopoManager.BalanceArea
		Dim oBalanceConditionalArea As TopoManager.BalanceArea

		Dim iParcelsUB As Integer = oParcelList.Count - 1
		Dim daResultArea(iParcelsUB) As Double
		Dim daResultAreaProp(iParcelsUB) As Double

		Dim daRangeMin(iParcelsUB) As Double
		Dim daRangeMax(iParcelsUB) As Double
		Dim colFreeAreaIndecis As System.Collections.ObjectModel.Collection(Of Integer) = New ObjectModel.Collection(Of Integer)()

		Dim oaResParcels(iParcelsUB) As UnidivNet.UD_Parcel
		Dim iIndex As Integer = 0
		Dim oDataRow As DataRow = Nothing
		Dim oGridRow As DataGridViewRow = Nothing
		Dim dResArea As Double
		Dim iFreeIndex As Integer
		Dim dSourceArea As Double = oSourceParcel.ParcelArea.LegalArea
		Dim dSourceAreaProp As Double = dSourceArea
		Dim oPgonScheme As tsPolygon

		If True OrElse Not bIsDBSource Then
			oParcelList.Sort(oParcelComparer)
		End If


		For Each oParcel In oParcelList

			oaResParcels(iIndex) = oParcel
			daResultAreaProp(iIndex) = oParcel.AcadArea

			If oParcel.ParcelArea.ForcedArea = 0.0 Then
				daResultArea(iFreeIndex) = oParcel.AcadArea
				iFreeIndex += 1
				oParcel._ParcelArea.CalculateRange(daRangeMin(iIndex), daRangeMax(iIndex))
			Else
				dSourceArea -= oParcel.ParcelArea.ForcedArea
				oParcel.SetForcedArea(oParcel.ParcelArea.ForcedArea)
				oParcel.CalculateArea(oParcel.ParcelArea.ForcedArea)
			End If

			iIndex += 1
		Next
		If iFreeIndex < iParcelsUB + 1 Then
			ReDim Preserve daResultArea(iFreeIndex - 1)
		End If

		If Me.chkCalcByRange.Checked Then

			oBalanceArea = New TopoManager.BalanceArea(daResultArea, daRangeMin, daRangeMax, dRoundFactor, dSourceArea, True, "Ud")
		Else

			oBalanceArea = New TopoManager.BalanceArea(daResultArea, dRoundFactor, dSourceArea, True, "Ud")

		End If
		oBalanceConditionalArea = New TopoManager.BalanceArea(daResultAreaProp, dRoundFactor, dSourceAreaProp, True, "Ud")

		iFreeIndex = 0
		For iIndex = 0 To oParcelList.Count - 1
			oParcel = oaResParcels(iIndex)

			If oParcel.ParcelArea.ForcedArea = 0 Then
				dResArea = oBalanceArea.OutputItemFloat(iFreeIndex)
				oParcel.CalculateArea(dResArea)

				'	
				iFreeIndex += 1
			Else
				oParcel.SetConditionalArea(oBalanceConditionalArea.OutputItemFloat(iIndex))

			End If

			oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)
			oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex + iIndex, False, oParcel.DbID)
			oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID

			oDataRow.Item("ParcelDbID") = oParcel.DbID
			If oPgonScheme IsNot Nothing Then
				oDataRow.Item("BorderObjID") = oPgonScheme.BorderObjID
				oParcel.BorderAcObjID = oPgonScheme.BorderObjID
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("------Source Parcel: " & oSourceParcel.Name & "; " & oParcelList.Count.ToString & "-----------------Parcel count= " & mdicParcels.Count.ToString())
			End If
			oParcel.ParcelKey.ToJournalDataRow(oDataRow, False)
			If Not String.IsNullOrEmpty(oParcel.LanduseName) Then
				oDataRow.Item("LanduseName") = oParcel.LanduseName
			End If
			If Not String.IsNullOrEmpty(oParcel.Lot) Then
				oDataRow.Item("LotName") = oParcel.Lot
			End If
			If Not String.IsNullOrEmpty(oParcel.Plan) Then
				oDataRow.Item("PlanName") = oParcel.Plan
			End If

			oDataRow.EndEdit()

			oGridRow = zzGetRow(miCurrentActionFirstRowIndex + iIndex)
			zzFillParcelArea(oParcel.ParcelArea, True, oDataRow, oGridRow)
			oParcel.UpdateBlockAttributes()
		Next
		oDataRow.Item("RowStatus") = enRowStatus.EndOfAction
		moLastActionRow = oDataRow
		zzSetParcelCount(oParcelList.Count)

	End Sub
	Private Sub zzCalcDivideResParcels(oSourceParcel As UnidivNet.UD_Parcel, oList As List(Of UD_ParcelKey), bIsDBSource As Boolean)
		Const dRoundFactor As Double = 1.0
		Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oBalanceArea As TopoManager.BalanceArea = Nothing
		Dim oBalanceConditionalArea As TopoManager.BalanceArea

		Dim iParcelsUB As Integer = oList.Count - 1
		Dim daResultArea(iParcelsUB) As Double
		Dim daResultAreaProp(iParcelsUB) As Double

		Dim daRangeMin(iParcelsUB) As Double
		Dim daRangeMax(iParcelsUB) As Double
		Dim colFreeAreaIndecis As System.Collections.ObjectModel.Collection(Of Integer) = New ObjectModel.Collection(Of Integer)()
		'Dim iFreeParcelsUB As Integer
		Dim oaResParcels(iParcelsUB) As UnidivNet.UD_Parcel
		Dim iIndex As Integer = 0
		Dim oDataRow As DataRow = Nothing
		Dim oGridRow As DataGridViewRow = Nothing
		Dim dResArea As Double
		Dim iFreeIndex As Integer
		Dim dSourceArea As Double = oSourceParcel.ParcelArea.LegalAreaM
		Dim dSourceAreaProp As Double = dSourceArea
		Dim oPgonScheme As tsPolygon

		'	DMCommon.Debug.MsgBox("09_799", oList.Count)
		'  Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
		If True OrElse Not bIsDBSource Then
			oList.Sort(oParcelComparer)
		End If

		'Dim dTestArea As Double = 0.0
		'	Dim iTestArea As Integer = 0

		iFreeIndex = 0
		For Each tParcelKey As UD_ParcelKey In oList
			If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "ParArea3", tParcelKey, oParcel.UD_Name, oParcel.LegalArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.AcadArea(False))
				oParcel.LegalToForcedArea()
				'DMCommon.ExcelLog.SetNextValue(0, "ParArea4", tParcelKey, oParcel.UD_Name, oParcel.LegalArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.AcadArea(False))
				oaResParcels(iIndex) = oParcel
				daResultAreaProp(iIndex) = oParcel.AcadArea

				DMCommon.Debug.ExcelLog.SetNextValue(4, msDivideActionName, iIndex + 1, oParcel.Name)


				If oParcel.ParcelArea.ForcedArea = 0.0 Then
					daResultArea(iFreeIndex) = oParcel.AcadArea
					iFreeIndex += 1
					'	colFreeAreaIndecis.Add(iRowIndex)
					oParcel._ParcelArea.CalculateRange(daRangeMin(iIndex), daRangeMax(iIndex))
				Else
					dSourceArea -= oParcel.ParcelArea.ForcedArea
					oParcel.SetForcedArea(oParcel.ParcelArea.ForcedArea)
					oParcel.CalculateArea(oParcel.ParcelArea.ForcedArea)

					'	zzFillParcelArea(oParcel.ParcelArea, True, oDataRow, oGridRow)

				End If

				'DMCommon.ExcelLog.SetNextValue(0, "ParArea5", tParcelKey, oParcel.UD_Name, oParcel.LegalArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.AcadArea(False))
				'
				'	DMCommon.ExcelLog.SetNextValue(0, "Res", daResultAreaProp(iIndex), daResultArea(iIndex))
				'dTestArea += oParcel.AcadArea(False)

				' DMCommon.Debug.MsgBox("09_132", True, oParcel.LegalArea, oParcel.AcadArea)
			Else
				DMCommon.Debug.MsgBox("09_137", tParcelKey)
			End If
			iIndex += 1
		Next
		'	DMCommon.Debug.MsgBox("09_137Sum", oList.Count, dSourceArea, iFreeIndex, iParcelsUB)
		If iFreeIndex - 1 < iParcelsUB AndAlso iFreeIndex >= 1 Then
			ReDim Preserve daResultArea(iFreeIndex - 1)
		End If



		'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetNextValue(0, "Source", dSourceAreaProp, dSourceArea, iFreeIndex)

		If Me.chkCalcByRange.Checked Then
			'DMCommon.Functions.DispArray(daRangeMin, "03_884a")
			'DMCommon.Functions.DispArray(daRangeMax, "03_884b")

			oBalanceArea = New TopoManager.BalanceArea(daResultArea, daRangeMin, daRangeMax, dRoundFactor, dSourceArea, True, "Ud")
		ElseIf dSourceArea >= 0 AndAlso iFreeIndex > 0 Then

			'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetArray(0, "ResultArea", True, daResultArea)
			'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetNextValue(0, "SourceArea", dSourceArea)
			oBalanceArea = New TopoManager.BalanceArea(daResultArea, dRoundFactor, dSourceArea, True, "Ud")

		End If
		oBalanceConditionalArea = New TopoManager.BalanceArea(daResultAreaProp, dRoundFactor, dSourceAreaProp, True, "Ud")
		If oBalanceArea IsNot Nothing Then
			'''''''''''''''''''''10/11/19  DMCommon.Debug.ExcelLog.SetArray(0, "Output", True, oBalanceArea.Output)
		End If

		iFreeIndex = 0
		For iIndex = 0 To oList.Count - 1
			oParcel = oaResParcels(iIndex)
			If oParcel IsNot Nothing Then


				'DMCommon.Debug.MsgBox("09_800h", iIndex, oParcel.Name, oParcel.ParcelKey.ToString(), oParcel.Fragments.Count, oBalanceConditionalArea.OutputItemFloat(iIndex))
				oParcel.SetConditionalArea(oBalanceConditionalArea.OutputItemFloat(iIndex))
				If oParcel.ParcelArea.ForcedArea = 0 AndAlso oBalanceArea IsNot Nothing Then
					dResArea = oBalanceArea.OutputItemFloat(iFreeIndex)
					oParcel.CalculateArea(dResArea)
					iFreeIndex += 1
					'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetNextValue(0, "NotForced", oParcel.Name, dResArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.LegalArea, oParcel.CalcArea)
				Else
					oParcel.LegalFromForcedArea()
					'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetNextValue(0, "Forced", oParcel.Name, dResArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.LegalArea, oParcel.CalcArea)

				End If

				oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)
				oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex + iIndex, False, oParcel.DbID)
				oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID

				oDataRow.Item("ParcelDbID") = oParcel.DbID
				'  DMCommon.Debug.MsgBox("09_138", miCurrentActionFirstRowIndex + iIndex, oParcel.ParcelKey, oParcel.DbID, oDataRow.Item("ParcelDbID"))
				'  zzTestDataRow(oDataRow, "ParcelDbID")
				If oPgonScheme IsNot Nothing Then
					oDataRow.Item("BorderObjID") = oPgonScheme.BorderObjID
					oParcel.BorderAcObjID = oPgonScheme.BorderObjID
				Else
					DMAcadExt.AcadDocument.WriteMessageLog("------Source Parcel: " & oSourceParcel.Name & "; " & oList.Count.ToString & "-----------------Parcel count= " & mdicParcels.Count.ToString())
				End If




				oParcel.ParcelKey.ToJournalDataRow(oDataRow, False)
				If Not String.IsNullOrEmpty(oParcel.LanduseName) Then
					oDataRow.Item("LanduseName") = oParcel.LanduseName
				End If
				If Not String.IsNullOrEmpty(oParcel.Lot) Then
					oDataRow.Item("LotName") = oParcel.Lot
				End If
				If Not String.IsNullOrEmpty(oParcel.Plan) Then
					oDataRow.Item("PlanName") = oParcel.Plan
				End If

				oDataRow.EndEdit()
				'DMCommon.Debug.MsgBox("11_337", oParcel.TopoID, moCurrentStageTopoScheme.Polygons.Count)

				oGridRow = zzGetRow(miCurrentActionFirstRowIndex + iIndex)
				'''''''''''''''''''''10/11/19 DMCommon.Debug.ExcelLog.SetNextValue(0, "End", oSourceParcel.Name, oParcel.Name, dResArea, oParcel.ParcelArea.ForcedArea, oParcel.AcadArea, oParcel.LegalArea, oParcel.CalcArea)
				zzFillParcelArea(oParcel.ParcelArea, True, oDataRow, oGridRow)
				'	DMCommon.Debug.MsgBox("11_337f", oParcel.TopoID, oParcel.CentroidAcObjID)
				oParcel.UpdateBlockAttributes()
			End If
		Next
		oDataRow.Item("RowStatus") = enRowStatus.EndOfAction
		'DMCommon.ExcelLogT.SetDataTable(moMainTable, 0)
		moLastActionRow = oDataRow
		'  zzCloseLastRow()
		zzSetParcelCount(oList.Count)

	End Sub

	Private Sub zzCalcUnionResParcel(oDestParcel As UnidivNet.UD_Parcel)

		Dim oDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)

		oDataRow.Item("AcObjID") = oDestParcel.CentroidAcObjID
		oDataRow.Item("ParcelDbID") = oDestParcel.DbID
		Dim oPgonScheme As tsPolygon = moCurrentStageTopoScheme.Polygons.Item(0)
		oDataRow.Item("BorderObjID") = oPgonScheme.BorderObjID
		zzFillParcelArea(oDestParcel.ParcelArea, True, oDataRow)
		oDestParcel.UpdateBlockAttributes()

	End Sub
	Private Sub zzCheckHanitObjects()

		If DMAcadExt.AcadDocument.IsLocked Then

			'    DMAcadExt.AcadDocument.OpenLog(False)
			If Not (DMAcadExt.AcadTransaction.HasEntity(UnidivNet.UD_App.GetHanitLayersNew(False)) AndAlso UnidivNet.UD_App.HasHanitTopos()) Then
				'	DMCommon.Debug.MsgBox("13_001h")
				Me.cmdClearLayers.Enabled = False
				Me.cmdClearLayers.BackColor = Me.BackColor
			End If


		End If
	End Sub

	Private Sub cmdClearLayers_Click(oSender As System.Object, e As EventArgs) Handles cmdClearLayers.Click

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		If DMAcadExt.AcadDocument.IsLocked Then
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
			'    DMAcadExt.AcadDocument.OpenLog(False)
			DMAcadExt.AcadTransaction.ClearLayerSet(UnidivNet.UD_App.GetHanitLayersNew(False))
			UnidivNet.UD_App.DeleteHanitTopos()
			UnidivNet.UD_App.ComeBackSPoints(False)
			DMCommon.Debug.MsgBox("13_132b")
			UnidivNet.UD_App.ToSourceLayer()
			DMCommon.Debug.MsgBox("13_132c")
			UnidivNet.UD_App.ClearCdBlockMarks()
			DMCommon.Debug.MsgBox("13_132d")
			zzCheckHanitObjects()
			DMAcadExt.AcadDocument.CloseLog()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End If
	End Sub


	Private Sub zzWriteParcelsInfo(sLabel As String)
		DMAcadExt.AcadDocument.WriteDebugMessage("------" & sLabel & "-----------------Parcel count= " & mdicParcels.Count.ToString())
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; !Fr: " & oParcel.Fragments.Count.ToString() & "; " & oParcel.AcadArea.ToString() & "/" & oParcel.LegalArea.ToString())

			For Each iId As Integer In oParcel.Fragments
				DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; Fr: " & iId.ToString())
			Next

		Next
	End Sub

	Private Function zzGetCurrentStageTopoName() As String
		Return zzGetStageTopoName(miCurrentStage, miCurrentAction)
	End Function
	Private Function zzGetStageTopoName(iStage As Integer, iAction As Integer) As String
		Return UnidivNet.UD_App.GetStageTopoName(iStage, iAction)
		'  Return "Stage" & iStage.ToString() & "_" & iAction.ToString()
	End Function
	Private Function zzGetCentroidLayer() As String
		Return "C1603_" & miCurrentStage.ToString()
	End Function
	Private Sub zzClearInputCells()
		Dim oGridRow As DataGridViewRow
		For Each tCellKey As CellKey In mhsInputCells
			If tCellKey.Row < Me.dgvMain.Rows.Count Then
				oGridRow = Me.dgvMain.Rows.Item(tCellKey.Row)
				oGridRow.Cells.Item(tCellKey.Column).Style = Nothing
			End If

		Next
		mhsInputCells.Clear()
	End Sub
	Private Sub zzClearInputCellsForStage()
		Dim oGridRow As DataGridViewRow
		Dim colForRemove As ObjectModel.ObservableCollection(Of CellKey) = New ObjectModel.ObservableCollection(Of CellKey)()

		For Each tCellKey As CellKey In mhsInputCells
			If tCellKey.ForStage AndAlso tCellKey.Row < Me.dgvMain.Rows.Count Then
				oGridRow = Me.dgvMain.Rows.Item(tCellKey.Row)
				oGridRow.Cells.Item(tCellKey.Column).Style = Nothing
				colForRemove.Add(tCellKey)
			End If

		Next
		mhsInputCells.ExceptWith(colForRemove)
	End Sub
	Private Sub zzAddInputCellsFirstRow(iRowIndex As Integer)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)
		oGridRow.Cells.Item(miFromParcelColIndex).Style = moDataGridViewCellStyleInput
		oGridRow.Cells.Item(miFromParcelTempColIndex).Style = moDataGridViewCellStyleInput
		oGridRow.Cells.Item(miToParcelColIndex).Style = moDataGridViewCellStyleInput
		oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInputGush
		oGridRow.Cells.Item(miToGushAddColIndex).Style = moDataGridViewCellStyleInputGush


		mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelColIndex, True))
		mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelTempColIndex, True))
		mhsInputCells.Add(New CellKey(iRowIndex, miToParcelColIndex, True))
		'  mhsInputCells.Add(New CellKey(iRowIndex, miToGushColIndex))

		If miCurrentActionType = UnidivNet.enActionType.Transfer Then
			oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInput
			mhsInputCells.Add(New CellKey(iRowIndex, miToGushColIndex, True))
			mhsInputCells.Add(New CellKey(iRowIndex, miToGushAddColIndex, True))

		End If
	End Sub


	Private Sub zzAddInputCellsLastAction()
		If miLastActionFirstRowIndex >= 0 AndAlso miCurrentActionFirstRowIndex = -1 Then
			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Divide
					zzAddInputCellsLastDivide(miLastActionFirstRowIndex, moMainTable.Rows.Count - 1)
				Case UnidivNet.enActionType.Union
					zzAddInputCellsLastUnion()
			End Select


		End If
	End Sub
	Private Sub zzAddInputCellsLastDivide(ByVal iFirstRowIndex As Integer, ByVal iLastRowIndex As Integer)
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow

		For iRowIndex As Integer = iFirstRowIndex To iLastRowIndex
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
			oDataRow = moMainTable.Rows.Item(iRowIndex)

			zzAddInputCell(oGridRow, iRowIndex, miForcedAreaColIndex, False)
			zzAddInputCell(oGridRow, iRowIndex, miToParcelColIndex, False) '27/01/19

			'zzAddInputCell(oGridRow, iRowIndex, miForcedAreaColIndex, True)

			zzAddInputCell(oGridRow, iRowIndex, miPlanNameColIndex, False)
			zzAddInputCell(oGridRow, iRowIndex, miLotNameColIndex, False)
			zzAddInputCell(oGridRow, iRowIndex, miLanduseIDColIndex, False)
			zzAddInputCell(oGridRow, iRowIndex, miLanduseNameColIndex, False)

		Next
	End Sub
	Private Sub zzAddInputCell(ByRef oGridRow As DataGridViewRow, iRowIndex As Integer, iColumnIndex As Integer, bForStage As Boolean)
		oGridRow.Cells.Item(iColumnIndex).Style = moDataGridViewCellStyleInput
		mhsInputCells.Add(New CellKey(iRowIndex, iColumnIndex, bForStage))
	End Sub
	Private Sub zzAddInputCellsLastUnion()
		Dim oGridRow As DataGridViewRow = Nothing
		If miLastActionFirstRowIndex >= 0 Then
			oGridRow = Me.dgvMain.Rows.Item(miLastActionFirstRowIndex)
			'DMCommon.Debug.MsgBox("12_224", miLastActionFirstRowIndex, miCurrentActionFirstRowIndex)
			'If False Then
			zzAddInputCell(oGridRow, miLastActionFirstRowIndex, miPlanNameColIndex, False)
			zzAddInputCell(oGridRow, miLastActionFirstRowIndex, miLotNameColIndex, False)

			zzAddInputCell(oGridRow, miLastActionFirstRowIndex, miLanduseIDColIndex, False)
			zzAddInputCell(oGridRow, miLastActionFirstRowIndex, miLanduseNameColIndex, False)

		End If




	End Sub
	Private Sub zzClearInputStyle()
		For Each tCellKey As CellKey In mhsInputCells
			If tCellKey.Row < Me.dgvMain.Rows.Count Then
				Me.dgvMain.Rows.Item(tCellKey.Row).Cells.Item(tCellKey.Column).Style = Nothing
			End If

		Next
		mhsInputCells.Clear()
	End Sub
	Private Sub zzAddInputCellsNextRow(iRowIndex As Integer)
		Try
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)

			If miCurrentActionType = UnidivNet.enActionType.Union Then
				mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelColIndex, True))
				mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelTempColIndex, True))
				oGridRow.Cells.Item(miFromParcelColIndex).Style = moDataGridViewCellStyleInput
				oGridRow.Cells.Item(miFromParcelTempColIndex).Style = moDataGridViewCellStyleInput
			ElseIf miCurrentActionType = UnidivNet.enActionType.Divide Then
				mhsInputCells.Add(New CellKey(iRowIndex, miToParcelColIndex, True))
				oGridRow.Cells.Item(miToParcelColIndex).Style = moDataGridViewCellStyleInput
				oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInputSavedGush
				oGridRow.Cells.Item(miToGushAddColIndex).Style = moDataGridViewCellStyleInputSavedGush




			End If
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("13_022", oEx.Message, oEx.StackTrace, iRowIndex, miFromParcelColIndex, miFromParcelTempColIndex)
		End Try


	End Sub

	Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
		If mbEventsEnabled Then


			Dim tCellKey As CellKey = New CellKey(e.RowIndex, e.ColumnIndex, True)
			'If miCurrentActionFirstRowIndex = -1 OrElse Not mhsInputCells.Contains(tCellKey) Then
			If mhsInputCells.Contains(tCellKey) Then
				mtEditingCellKey = tCellKey
				Select Case e.ColumnIndex
					Case 2, 3
						'  DMCommon.Debug.MsgBox("09_877", e.RowIndex.ToString() & ":" & e.ColumnIndex.ToString(), moMainTable.Rows.Count)
						Dim oDataRow As DataRow = Nothing
						If e.RowIndex < moMainTable.Rows.Count Then
							oDataRow = moMainTable.Rows.Item(e.RowIndex)
							mtEditingParcelKey = zzParcelKeyFrom(oDataRow)
						Else
							mtEditingParcelKey = New UD_ParcelKey()
						End If
				End Select
			Else
				e.Cancel = True
			End If
		End If '
		'  DMCommon.Debug.MsgBox("09_877A", e.RowIndex.ToString() & ":" & e.ColumnIndex.ToString(), moMainTable.Rows.Count)
	End Sub

	Private Function zzCalcStage() As Boolean
		Return True
	End Function
	Private Class CellComparer
		Implements System.Collections.Generic.IEqualityComparer(Of CellKey)

		Public Function CellsEqual(tCellKey1 As CellKey, tCellKey2 As CellKey) As Boolean Implements IEqualityComparer(Of CellKey).Equals
			Return tCellKey1.Row = tCellKey2.Row AndAlso tCellKey1.Column = tCellKey2.Column
		End Function

		Public Function GetHashCode1(tCellKey As CellKey) As Integer Implements IEqualityComparer(Of CellKey).GetHashCode
			Return tCellKey.CellKey
		End Function
	End Class

	Private Structure CellKey
		Const RowLevel As Integer = 1000
		Dim Row As Integer
		Dim Column As Integer
		Dim ForStage As Boolean
		Private mbExists As Boolean
		Public Sub New(iRow As Integer, iColumn As Integer, bForStage As Boolean)
			Row = iRow
			Column = iColumn
			ForStage = bForStage
			mbExists = True
		End Sub
		Public Sub New(iCellKey As Integer)
			Row = iCellKey \ RowLevel
			Column = iCellKey - Row * RowLevel
		End Sub
		ReadOnly Property CellKey As Integer
			Get
				Return Row * RowLevel + Column
			End Get
		End Property
		ReadOnly Property Exists As Boolean
			Get
				Return mbExists
			End Get
		End Property
		Sub [Erase]()
			mbExists = False
		End Sub

	End Structure
	Private Structure UD_Row
		Dim Stage As Integer
		Public Sub New(iStage As Integer)
			Stage = iStage
		End Sub
	End Structure
	Private Structure ProcessInfo

		Dim StageNo As Integer
		Dim ActionType As UnidivNet.enActionType
		Dim Oper As Integer
		Public Sub New(iStageNo As Integer, iActionType As UnidivNet.enActionType, iOper As Integer)
			StageNo = iStageNo
			ActionType = iActionType
			Oper = iOper
		End Sub


	End Structure
	Private Enum enScriptType
		Empty
		ParcelLinkList
		LinkList
		ParcelList


	End Enum
	Private Structure ScriptData
		Private mbAdditionalAction As Boolean
		Private miStage As Integer
		Private miAction As Integer
		Private miActionType As UnidivNet.enActionType
		Private msFileName As String
		'Input Script Data
		Private miScriptType As enScriptType
		Private miParcelDbID As Integer
		Private msParcelName As String
		Private mcolLinkObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Private mcolParcelsDbIDs As ObjectModel.Collection(Of Integer)
		'Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		Public Sub New(bAdditionalAction As Boolean, bActionClosed As Boolean, iCurrentActionType As UnidivNet.enActionType, iCurrentStage As Integer, iCurrentAction As Integer)
			Const sDelim As String = "_"
			If bAdditionalAction Then
				miActionType = UnidivNet.enActionType.RestoreLayer
				If bActionClosed Then
					If iCurrentActionType = UnidivNet.enActionType.Union Then
						miAction = 1
						miStage = iCurrentStage + 1
					ElseIf iCurrentActionType = UnidivNet.enActionType.Divide Then
						miAction = iCurrentAction + 1
						miStage = iCurrentStage
					End If
				Else
					miAction = iCurrentAction
					miStage = iCurrentStage
				End If
			Else
				miAction = iCurrentAction
				miActionType = iCurrentActionType
				miStage = iCurrentStage
			End If




			msFileName = DMAcadExt.AcadDocument.GetCurrentDirectory & "\" & miActionType.ToString() & sDelim & miStage.ToString() & sDelim & miAction.ToString() & ".xml"
		End Sub
		'Private miStage As Integer
		'Private miAction As Integer
		'Private miActionType As UnidivNet.enActionType

		Public ReadOnly Property Stage As Integer
			Get
				Return miStage
			End Get
		End Property
		Public ReadOnly Property Action As Integer
			Get
				Return miAction
			End Get
		End Property
		Public ReadOnly Property ActionType As UnidivNet.enActionType
			Get
				Return miActionType
			End Get
		End Property
		Public Sub Load(iScriptType As enScriptType)
			miScriptType = iScriptType
			Select Case miScriptType
				Case enScriptType.ParcelLinkList
					zzLoadLinks(True)
				Case enScriptType.LinkList
					zzLoadLinks(False)
				Case enScriptType.ParcelList
					zzLoadParcelList()
			End Select
		End Sub
		Public Sub Close()
			miScriptType = enScriptType.Empty
		End Sub
		Public ReadOnly Property FileName As String
			Get
				Return msFileName
			End Get
		End Property


		Public Function FileExists() As Boolean
			Dim oFileInfo As IO.FileInfo = New IO.FileInfo(msFileName)
			'DMCommon.Debug.MsgBox("13_146s", sFileName, oFileInfo.Exists)
			Return oFileInfo.Exists
		End Function
		Public ReadOnly Property ScriptType As enScriptType
			Get
				Return miScriptType
			End Get
		End Property
		Public ReadOnly Property ParcelDbID As Integer
			Get
				Return miParcelDbID
			End Get
		End Property
		Public ReadOnly Property LinkObjIDs As ObjectIdCollection
			Get
				Return mcolLinkObjIDs
			End Get
		End Property
		Public ReadOnly Property ParcelName As String
			Get
				Return msParcelName
			End Get
		End Property
		Private Function zzLoadLinks(bParcelExists As Boolean) As Boolean

			Dim iChildIndex As Integer = 0
			'	Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			'Dim iDbID As Integer
			Dim oXMLDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()
			Dim oParcelNameNode As Xml.XmlNode
			Dim oLinkNode As Xml.XmlNode


			'	Dim sParcelName As String = Nothing
			oXMLDocument.Load(msFileName)
			Dim oDocNode As Xml.XmlNode = oXMLDocument.DocumentElement
			Dim oActionNode As Xml.XmlNode = oDocNode.FirstChild
			If bParcelExists Then
				Dim oSourceParcelNode As Xml.XmlNode = oActionNode.ChildNodes(iChildIndex)
				Dim oParcelDbIDNode As Xml.XmlNode = oSourceParcelNode.ChildNodes(0)
				iChildIndex += 1

				oParcelNameNode = oSourceParcelNode.ChildNodes(1)
				Integer.TryParse(oParcelDbIDNode.InnerText, miParcelDbID)
				msParcelName = oParcelNameNode.InnerText
			End If


			Dim oLinksNode As Xml.XmlNode = oActionNode.ChildNodes(iChildIndex)



			'Dim oLinksNode As Xml.XmlNode = oNode.FirstChild
			Dim oLinkObjIDNode As Xml.XmlNode


			Dim sAcObjIDString As String
			Dim sHandleString As String
			Dim tAcObjID As ObjectId
			mcolLinkObjIDs = New ObjectIdCollection()
			'	DMCommon.Debug.MsgBox("13_146c!", DMAcadExt.AcadTransaction.ModelSpaceObjID, oSourceParcel Is Nothing)



			'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			DMAcadExt.AcadTransaction.OpenHandleDictionary()

			For Each oNodeLink As Xml.XmlNode In oLinksNode.ChildNodes

				oLinkNode = oNodeLink.ChildNodes(1)
				sHandleString = oLinkNode.InnerText
				tAcObjID = DMAcadExt.AcadTransaction.GetObjectID(sHandleString)

				If Not tAcObjID.IsNull Then
					mcolLinkObjIDs.Add(tAcObjID)
				End If
				oLinkObjIDNode = oNodeLink.ChildNodes(0)
				sAcObjIDString = oLinkObjIDNode.InnerText

			Next
			'DMAcadExt.AcadTransaction.CloseModelSpace()


			'
			'	DMCommon.Debug.MsgBox("11_165n", mcolLinkObjIDs.Count, miParcelDbID, msParcelName)

		End Function
		Private Sub zzLoadParcelList() 'As ICollection(Of UnidivNet.UD_Parcel)  'sFileName As String
			'Dim sFileName As String = zzGetScriptFileName()
			'	Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
			'	Dim iOutputActionType As UnidivNet.enActionType
			'Dim iOutputStage As Integer
			'Dim iOutputAction As Integer


			'Dim sFileName As String = zzGetScriptInitData(False, iOutputActionType, iOutputStage, iOutputAction)

			Dim oParcel As UnidivNet.UD_Parcel = Nothing

			Dim oXMLDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()

			oXMLDocument.Load(msFileName)
			Dim oNode As Xml.XmlNode = oXMLDocument.DocumentElement
			Dim oParcelsNode As Xml.XmlNode = oNode.FirstChild
			Dim oParcelDbIDNode As Xml.XmlNode
			Dim oParcelNameNode As Xml.XmlNode
			Dim iDbID As Integer
			Dim sParcelName As String
			mcolParcelsDbIDs = New ObjectModel.Collection(Of Integer)()
			For Each oNodeParcel As Xml.XmlNode In oParcelsNode.ChildNodes
				iDbID = 0
				oParcelNameNode = oNodeParcel.ChildNodes(1)
				sParcelName = oParcelNameNode.InnerText

				oParcelDbIDNode = oNodeParcel.ChildNodes(0)
				If Integer.TryParse(oParcelDbIDNode.InnerText, iDbID) Then
					mcolParcelsDbIDs.Add(iDbID)
				End If

			Next
			'DMCommon.Debug.MsgBox("11_165", mlstSelectedParcels.Count)


			'+ToGrid





			'Return colParcels

		End Sub

	End Structure
	Private Class StageTopologies
		Private mcolTopoNames As System.Collections.ObjectModel.Collection(Of String) = New System.Collections.ObjectModel.Collection(Of String)()

		Private moaOpenedTopos() As TopologyModel
		Public Sub CloseTopos()

			For Each oTopologyModel As TopologyModel In moaOpenedTopos
				oTopologyModel.Close()
			Next
		End Sub
		Public Sub OpenTopos()
			Dim iIndex As Integer = mcolTopoNames.Count - 1
			ReDim moaOpenedTopos(iIndex)

			For Each sTopoName As String In mcolTopoNames
				moaOpenedTopos(iIndex) = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
				iIndex -= 1
			Next
		End Sub
		Public Sub AddTopoName(sTopoName As String)
			mcolTopoNames.Add(sTopoName)
		End Sub
	End Class
	Private Class Ud_Points
		Inherits HashSet(Of String)
		Private miPointExistsMaxNum As Integer
		Private miPredeterminedNum As Integer
		Private moTriangleMarkBlock As DMAcadExt.MarkBlock
		Public Sub New()
			moTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
		End Sub
		ReadOnly Property PointExistsMaxNum As Integer
			Get
				Return miPointExistsMaxNum
			End Get
		End Property
		Property PredeterminedNum As Integer
			Get
				Return miPredeterminedNum
			End Get
			Set(iValue As Integer)
				miPredeterminedNum = iValue
			End Set
		End Property
		Public Sub AddPointExists(tPoint As Autodesk.AutoCAD.Geometry.Point3d, sName As String)
			If Me.Contains(sName) Then
				moTriangleMarkBlock.MarkPoint(tPoint, 2S)
			Else
				Me.Add(sName)
			End If

		End Sub
		Public Function GetNewPointNumber(bMaxNumber As Boolean) As String
			Dim iNumStart As Integer
			Dim sNumber As String
			If bMaxNumber Then
				iNumStart = Math.Max(miPointExistsMaxNum, miPredeterminedNum)
			Else
				iNumStart = miPredeterminedNum
			End If
			Do
				iNumStart += 1
				sNumber = Convert.ToString(iNumStart)
			Loop While Me.Contains(sNumber)
			miPointExistsMaxNum = iNumStart
			Return sNumber
		End Function
	End Class
	Private Class Fragment
		Private miDWGTopoID As Integer
		Private miDBTopoID As Integer
		Private mtSourceParcelKey As UD_ParcelKey
		Private mtParcelKey As UD_ParcelKey
		Private mstCancelledParcelKeys As Stack(Of UD_ParcelKey) = New Stack(Of UD_ParcelKey)()
		Private mlHandleVal As Long
		Private mdCentroidX As Double
		Private mdCentroidY As Double
		Private mdArea As Double


		Public Sub New(oDataRow As DataRow, iOriginalBlockNo As Integer, iOriginalAddBlockNo As Integer)
			Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ParcelNo"))
			Dim bParcelIsOriginal As Boolean = DirectCast(oDataRow.Item("ParcelIsOriginal"), Boolean)

			miDBTopoID = DMCommon.Functions.CIntN(oDataRow.Item("FragmentID"))
			mlHandleVal = DMCommon.Functions.CLngN(oDataRow.Item("CentroidHandle"))
			mdCentroidX = DMCommon.Functions.CDblN(oDataRow.Item("CentroidX"))
			mdCentroidY = DMCommon.Functions.CDblN(oDataRow.Item("CentroidY"))
			mtParcelKey = New UD_ParcelKey(iOriginalBlockNo, iParcelNo, bParcelIsOriginal)
		End Sub
		Public Sub New(oFragmentPgonScheme As tsPolygon, tParcelKey As UD_ParcelKey)
			Dim oDBObject As DBObject

			miDWGTopoID = oFragmentPgonScheme.ID
			mtParcelKey = tParcelKey
			mtSourceParcelKey = tParcelKey
			oDBObject = DMAcadExt.AcadTransaction.GetDBObject(oFragmentPgonScheme.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			If oDBObject IsNot Nothing Then
				mlHandleVal = oDBObject.Handle.Value
			End If
			mdCentroidX = oFragmentPgonScheme.Centroid.X
			mdCentroidY = oFragmentPgonScheme.Centroid.Y

		End Sub

		Public ReadOnly Property HandleVal As Long
			Get
				Return mlHandleVal
			End Get
		End Property

		Public Property ParcelKey As UD_ParcelKey
			Get
				Return mtParcelKey
			End Get
			Set(tValue As UD_ParcelKey)
				If mtParcelKey.Exists Then
					mstCancelledParcelKeys.Push(mtParcelKey)
				End If
				mtParcelKey = tValue
			End Set
		End Property
		Public ReadOnly Property CancelledParcelKeys As Stack(Of UD_ParcelKey)
			Get
				Return mstCancelledParcelKeys
			End Get
		End Property
		Public Sub DeleteParcel()
			If mstCancelledParcelKeys.Count > 0 Then
				mtParcelKey = mstCancelledParcelKeys.Pop
			End If

		End Sub

		Public ReadOnly Property SourceParcelKey As UD_ParcelKey
			Get
				Return mtSourceParcelKey
			End Get

		End Property
		Public ReadOnly Property ParcelNo As Integer
			Get
				Return mtParcelKey.ParcelNo
			End Get
		End Property
		Public Property DBTopoID As Integer
			Get
				Return miDBTopoID
			End Get
			Set(iValue As Integer)
				miDBTopoID = iValue
			End Set
		End Property
		Public Property DWGTopoID As Integer
			Get
				Return miDWGTopoID
			End Get
			Set(iValue As Integer)
				miDWGTopoID = iValue
			End Set
		End Property
		Public ReadOnly Property CentroidX As Double
			Get
				Return mdCentroidX
			End Get
		End Property
		Public ReadOnly Property CentroidY As Double
			Get
				Return mdCentroidY
			End Get
		End Property
		Public ReadOnly Property TopoIDChanged As Boolean
			Get
				Return (miDBTopoID <> miDWGTopoID)
			End Get
		End Property

	End Class

	Private Class Fragments
		Inherits Dictionary(Of Long, Fragment)
		Private mdicByTopoID As Dictionary(Of Integer, Fragment)
		Private mhsCenterDWGTopoIDs As HashSet(Of Integer) = New HashSet(Of Integer)()
		Private mdicDBToDwg_TopoIDs_ByHandle As Dictionary(Of Integer, Integer)
		Private mdicDBToDwg_TopoIDs_ByCenter As Dictionary(Of Integer, Integer)

		Private miTopoIDMethodStatus As enMethodStatus = enMethodStatus.Identical
		Private miHandleIDMethodStatus As enMethodStatus = enMethodStatus.Identical
		Private miCenterIDMethodStatus As enMethodStatus = enMethodStatus.Identical

		Private mbDBVersionIsCorrect As Boolean = True
		Private mbTopoIDChanged As Boolean = False
		Private mdicDWGIds As Dictionary(Of Integer, Integer)
		Private moaFragments() As Fragment

		Private miaParcelNo() As Integer
		Private miCheckedCounter As Integer
		Private miCurrentMethod As enMethod = enMethod.NotExists

		Private Enum enMethod
			NotExists
			Identical
			ByHandle
			ByCenter
		End Enum
		Private Enum enMethodStatus
			Identical
			Correct
			Invalid
		End Enum
		Public Sub New()
			mdicByTopoID = New Dictionary(Of Integer, Fragment)()
		End Sub
		Public Sub Test_dicByTopoID()

			For Each oFragment As Fragment In mdicByTopoID.Values
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Fragm", oFragment.DWGTopoID, oFragment.DBTopoID, oFragment.ParcelKey, oFragment.ParcelNo)
				For Each tParcelKey As UD_ParcelKey In oFragment.CancelledParcelKeys
					DMCommon.Debug.ExcelLog.SetNextValue(1, "!ParcKey", tParcelKey.ParcelNo)

				Next
			Next
			DMCommon.Debug.ExcelLog.SetNextValue(1, "!Fr", "------", "------", "------", "------")
		End Sub
		Public Function TryGetParcel(iFragmentID As Integer, ByRef tParcelKey As UD_ParcelKey) As Boolean
			Dim oFragment As Fragment = Nothing
			If mdicByTopoID.TryGetValue(iFragmentID, oFragment) Then
				tParcelKey = oFragment.ParcelKey
				Return True
			Else
				Return False
			End If
		End Function
		Public Sub SetParcel(iFragmentID As Integer, tParcelKey As UD_ParcelKey)
			Dim oFragment As Fragment = Nothing
			If mdicByTopoID.TryGetValue(iFragmentID, oFragment) Then
				oFragment.ParcelKey = tParcelKey
			Else
				DMCommon.Debug.MsgBox("12_230", "Fragment #" & iFragmentID.ToString() & " was not found")
			End If
		End Sub
		Public Sub SetParcel(colFragmentIDs As ICollection(Of Integer), tParcelKey As UD_ParcelKey)
			Dim oFragment As Fragment = Nothing
			Dim iFragmentID As Integer
			For Each iFragmentID In colFragmentIDs
				If mdicByTopoID.TryGetValue(iFragmentID, oFragment) Then
					oFragment.ParcelKey = tParcelKey
				End If
			Next

		End Sub
		Public Sub DeleteParcel(oParcel As UnidivNet.UD_Parcel)
			Dim oFragment As Fragment = Nothing
			Dim iFragmentID As Integer
			Dim hsFragments As HashSet(Of Integer) = oParcel.Fragments

			For Each iFragmentID In hsFragments
				If mdicByTopoID.TryGetValue(iFragmentID, oFragment) Then
					oFragment.DeleteParcel()
				End If
			Next

		End Sub

		Public Sub AddFragment(oFragment As Fragment)
			MyBase.Add(oFragment.HandleVal, oFragment)
			mdicByTopoID.Add(oFragment.DWGTopoID, oFragment)

		End Sub
		Public ReadOnly Property TopoIDChanged As Boolean
			Get
				Return mbTopoIDChanged
			End Get
		End Property
		Property DBVersionIsCorrect As Boolean
			Get
				Return mbDBVersionIsCorrect
			End Get
			Set(bValue As Boolean)
				mbDBVersionIsCorrect = bValue
			End Set
		End Property
		Public ReadOnly Property FragmentArray As Fragment()
			Get
				Return moaFragments
			End Get
		End Property
		Public Sub SetTopoIDChanged(bTopoIDChanged As Boolean)
			If Not mbTopoIDChanged AndAlso bTopoIDChanged Then
				mbTopoIDChanged = True
			End If
		End Sub
		Public Sub CheckByDBPolygonCount(iDBPgonCount As Integer)
			If MyBase.Count = iDBPgonCount Then
				mdicDWGIds = New Dictionary(Of Integer, Integer)()
			Else
				mbDBVersionIsCorrect = False
			End If
		End Sub
		Public Sub CheckByDWGPgonCount(iFragmentPolygonCount As Integer)
			If MyBase.Count = iFragmentPolygonCount Then

				ReDim moaFragments(iFragmentPolygonCount - 1)
				ReDim miaParcelNo(iFragmentPolygonCount - 1)
				ReDim moaFragments(iFragmentPolygonCount - 1)
				miCheckedCounter = 0

			Else
				mbDBVersionIsCorrect = False
			End If
		End Sub
		Public Sub Calculate(oFragmentTable As System.Data.DataTable)
			Dim hsDBFragmentTopoIDs As HashSet(Of Integer) = New HashSet(Of Integer)()
			Dim hsDBFragmentHandles As HashSet(Of Long) = New HashSet(Of Long)()
			Dim iDBFragmentTopoID As Integer
			Dim lDBFragmentHandle As Long
			Dim oFragmentPgon As Polygon
			Dim oFragment As Fragment = Nothing
			Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim dCentroidX, dCentroidY As Double
			Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

			mdicDBToDwg_TopoIDs_ByHandle = New Dictionary(Of Integer, Integer)()
			mdicDBToDwg_TopoIDs_ByCenter = New Dictionary(Of Integer, Integer)()

			For Each oRow As DataRow In oFragmentTable.Rows
				iDBFragmentTopoID = DMCommon.Functions.CIntN(oRow.Item("FragmentID"))
				If miTopoIDMethodStatus = enMethodStatus.Identical AndAlso Not mdicByTopoID.ContainsKey(iDBFragmentTopoID) Then
					miTopoIDMethodStatus = enMethodStatus.Invalid
				End If

				If miHandleIDMethodStatus <> enMethodStatus.Invalid Then
					lDBFragmentHandle = DMCommon.Functions.CLngN(oRow.Item("CentroidHandle"))
					If MyBase.TryGetValue(lDBFragmentHandle, oFragment) Then
						mdicDBToDwg_TopoIDs_ByHandle.Add(iDBFragmentTopoID, oFragment.DWGTopoID)
						If miHandleIDMethodStatus = enMethodStatus.Identical AndAlso iDBFragmentTopoID <> oFragment.DWGTopoID Then
							miHandleIDMethodStatus = enMethodStatus.Correct
						End If
					Else
						miHandleIDMethodStatus = enMethodStatus.Invalid
					End If
				End If

				If miCenterIDMethodStatus <> enMethodStatus.Invalid Then
					dCentroidX = DMCommon.Functions.CDblN(oRow.Item("CentroidX"))
					dCentroidY = DMCommon.Functions.CDblN(oRow.Item("CentroidY"))
					tPoint3d = New Autodesk.AutoCAD.Geometry.Point3d(dCentroidX, dCentroidY, 0.0)
					Try
						oFragmentPgon = oFragmentTopology.FindPolygon(tPoint3d)

					Catch oEx As Exception
						oFragmentPgon = Nothing
					End Try

					If oFragmentPgon Is Nothing OrElse mhsCenterDWGTopoIDs.Contains(oFragmentPgon.ID) Then
						miCenterIDMethodStatus = enMethodStatus.Invalid
					Else
						mdicDBToDwg_TopoIDs_ByCenter.Add(iDBFragmentTopoID, oFragmentPgon.ID)
						mhsCenterDWGTopoIDs.Add(oFragmentPgon.ID)

						If miCenterIDMethodStatus = enMethodStatus.Identical AndAlso iDBFragmentTopoID <> oFragmentPgon.ID Then
							miCenterIDMethodStatus = enMethodStatus.Correct
						End If
					End If
				End If
			Next
			If miTopoIDMethodStatus = enMethodStatus.Identical Then
				miCurrentMethod = enMethod.Identical

			ElseIf miHandleIDMethodStatus <> enMethodStatus.Invalid Then

				miCurrentMethod = enMethod.ByHandle
			ElseIf miCenterIDMethodStatus <> enMethodStatus.Invalid Then
				miCurrentMethod = enMethod.ByCenter
			Else
				miCurrentMethod = enMethod.NotExists
			End If
			DMCommon.Debug.MsgBox("13_203P", miCurrentMethod)

		End Sub

		Public Sub CheckByDB(oDataRow As DataRow, iOriginalBlockNo As Integer, iOriginalAddBlockNo As Integer)
			Dim iTestParcelNo As Integer

			Dim oFragment As Fragment = Nothing
			Dim oFragmentDB As Fragment = Nothing

			If mbDBVersionIsCorrect Then
				oFragmentDB = New Fragment(oDataRow, iOriginalBlockNo, iOriginalAddBlockNo)

				If Me.TryGetValue(oFragmentDB.HandleVal, oFragment) AndAlso oFragment.ParcelNo = oFragmentDB.ParcelNo Then

					oFragment.DBTopoID = oFragmentDB.DBTopoID
					SetTopoIDChanged(oFragment.TopoIDChanged)

					mdicDWGIds.Add(oFragment.DBTopoID, oFragment.DWGTopoID)
				Else
					mbDBVersionIsCorrect = False
				End If

			End If
			If oFragment IsNot Nothing Then
				iTestParcelNo = oFragment.ParcelNo
			End If

			'     DMCommon.Debug.MsgBox("12_822", mbDBVersionIsCorrect, Me.Count, oFragmentDB.HandleVal, Me.TryGetValue(oFragmentDB.HandleVal, oFragment), oFragmentDB.ParcelNo, iTestParcelNo)




		End Sub
		Public Function GetDWGTopoID(iDBTopoID As Integer) As Integer
			Dim iResDWGTopoID As Integer
			Select Case miCurrentMethod
				Case enMethod.Identical
					Return iDBTopoID
				Case enMethod.ByHandle
					If mdicDBToDwg_TopoIDs_ByHandle.TryGetValue(iDBTopoID, iResDWGTopoID) Then
						Return iResDWGTopoID
					End If
				Case enMethod.ByCenter
					If mdicDBToDwg_TopoIDs_ByCenter.TryGetValue(iDBTopoID, iResDWGTopoID) Then
						Return iResDWGTopoID
					End If
				Case Else
					Return 0
			End Select

		End Function



		Public Sub CreateDWGDic()
			If mbTopoIDChanged Then
				mdicDWGIds = New Dictionary(Of Integer, Integer)()
				For Each oFragment As Fragment In MyBase.Values
					mdicDWGIds.Add(oFragment.DBTopoID, oFragment.DWGTopoID)
				Next
			End If
		End Sub


	End Class
	Private Sub dgvMain_RowsAdded(oSender As System.Object, e As DataGridViewRowsAddedEventArgs) Handles dgvMain.RowsAdded
		If mbEventsEnabled Then

			'DMCommon.Debug.MsgBox("09_878", e.RowIndex.ToString(), e.RowCount.ToString())
			zzAddInputCellsNextRow(e.RowIndex)
		End If

	End Sub

	Private Sub rdbUnion_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbUnion.CheckedChanged
		If Me.rdbUnion.Checked Then
			' 	DMCommon.Debug.MsgBox("13_220", mbEventsEnabled)
			If mbEventsEnabled Then
				zzOpenAction(True, UnidivNet.enActionType.Union)
				Me.rdbDivide.Enabled = False
				Me.rdbTransfer.Enabled = False
				'  Me.rdbInsertBlock.Checked = True
				miCurrentCentroidStatus = enCentroidStatus.Default
			End If
		End If
	End Sub
	Private Sub rdbDivide_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbDivide.CheckedChanged
		If Me.rdbDivide.Checked Then

			If mbEventsEnabled Then
				zzOpenAction(True, UnidivNet.enActionType.Divide)
				Me.rdbUnion.Enabled = False
				Me.rdbTransfer.Enabled = False
				zzOpenDWG(True, False)
				If zzDivideDestBlockExists() Then
					Me.rdbHanit.Checked = True
				Else
					Me.rdbNew.Checked = True
				End If
				zzCloseDWG()
				'If mcolNewLayerCentroidsBlocks IsNot Nothing AndAlso mcolNewLayerCentroidsBlocks.Count > 0 Then
				'	Me.rdbHanit.Checked = True
				'ElseIf mcolTabaCentroidsBlocks IsNot Nothing AndAlso mcolTabaCentroidsBlocks.Count > 0 Then
				'	Me.rdbTaba.Checked = True
				'Else
				'	Me.rdbNew.Checked = True
				'End If
				Me.chkAddToSelect.Enabled = True
			End If
		Else
			Me.chkAddToSelect.Enabled = False
		End If
	End Sub
	Private Sub rdbTransfer_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbTransfer.CheckedChanged
		If Me.rdbTransfer.Checked Then
			If mbEventsEnabled Then
				zzOpenAction(True, UnidivNet.enActionType.Transfer)


				Me.rdbUnion.Enabled = False
				Me.rdbDivide.Enabled = False

				'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				'DMAcadExt.AcadTransaction.Start()
				'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
				zzOpenDWG()
				zzCreateFinalTopo()
				zzCloseDWG()
				'  zzCreateFinalTopology()

				'DMAcadExt.AcadTransaction.CloseModelSpace()
				'DMAcadExt.AcadTransaction.Terminate()
				'DMAcadExt.AcadDocument.Unlock()
			End If

		End If

	End Sub


	Private Sub cmdAllFragments_Click(oSender As System.Object, e As EventArgs) Handles cmdAllFragments.Click
		zzExec()

	End Sub
	Private Sub zzSetActionTypeFrame()
		If miCurrentActionType = UnidivNet.enActionType.Registered Then
			Me.rdbUnion.Checked = False
			Me.rdbDivide.Checked = False
			Me.rdbTransfer.Checked = False
			Me.rdbUnion.Enabled = True
			Me.rdbDivide.Enabled = True
			Me.rdbTransfer.Enabled = True
		ElseIf miCurrentActionFirstRowIndex = -1 Then
			Me.rdbUnion.Enabled = True
			Me.rdbDivide.Enabled = True
			Me.rdbTransfer.Enabled = True

			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Union
					Me.rdbUnion.Checked = True
				Case UnidivNet.enActionType.Divide
					Me.rdbDivide.Checked = True
				Case UnidivNet.enActionType.Transfer
					Me.rdbTransfer.Checked = True
			End Select
		Else
			zzSetActionOpenedFrame(Me.rdbUnion)
			zzSetActionOpenedFrame(Me.rdbDivide)
			zzSetActionOpenedFrame(Me.rdbTransfer)

		End If

		'   DMCommon.Debug.MsgBox("08_581c", miCurrentActionType, miCurrentActionFirstRowIndex, Me.rdbUnion.Enabled, Me.rdbDivide.Enabled, Me.rdbTransfer.Enabled)

	End Sub

	Private Sub zzSetActionOpenedFrame(oActionTypeRadioButton As RadioButton)
		oActionTypeRadioButton.Enabled = oActionTypeRadioButton.Checked
	End Sub


	Private Sub zzExec()
		Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		If miCurrentActionType = UnidivNet.enActionType.Divide Then
			zzBeforeDivide(True, enSelectLinkType.Script)
		ElseIf miCurrentActionType = UnidivNet.enActionType.Union Then
			Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = zzGridToParcels()
			Dim colActiveParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

			If colParcels.Count = 0 AndAlso Not zzGetParcelColFromGrid(colParcels) Then
				colParcels = mdicParcels.GetSorted()
			End If
			For Each oParcel As UnidivNet.UD_Parcel In colParcels
				If Not oParcel.IsCanceled Then
					colActiveParcels.Add(oParcel)
				End If
			Next
			DMCommon.Debug.ExcelLog.SetNextValue(1, "S1_Parcels", colParcels.Count, mlstSelectedParcels.Count, mhsSelectedParcels.Count, colActiveParcels.Count)
			If colActiveParcels.Count > 1 Then
				'DMCommon.Debug.MsgBox("13_011k", colParcels.Count)

				zzOpenDWG()
				'	
				zzWriteParcelListScript(colActiveParcels)
				zzBeforeUnion(colActiveParcels)
				zzCloseDWG()
				zzSetParcelCount(colActiveParcels.Count)
				miLastRowStatusIsNotDefined = True
				''''''''''   mdicParcels.DebugMsg()
			End If
		ElseIf miCurrentActionType = UnidivNet.enActionType.Transfer Then
			' DMCommon.Debug.MsgBox("08_577bef", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)

			zzOpenDWG()
			zzTransfer()
			miLastRowStatusIsNotDefined = False
			DMAcadExt.AcadDocument.Regen()
			zzCloseDWG()
			zzCloseAction()
			zzCloseActionBatch()
			' DMCommon.Debug.MsgBox("08_577a", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)
		End If

		'   UnidivNet.UD_App.PrintPointsInfo()
	End Sub
	Private Sub zzWriteParcelListScript(colParcels As ICollection(Of UnidivNet.UD_Parcel))
		'Dim iOutputActionType As UnidivNet.enActionType
		'Dim iOutputStage As Integer
		'Dim iOutputAction As Integer


		Dim sFileName As String = zzGetScriptInitData(False, mtCurrentScriptData.ActionType, mtCurrentScriptData.Stage, mtCurrentScriptData.Action)
		sFileName = mtCurrentScriptData.FileName
		Dim oWriterSettings As Xml.XmlWriterSettings = New Xml.XmlWriterSettings()
		oWriterSettings.Indent = True

		Using oXMLWriter As Xml.XmlWriter = Xml.XmlWriter.Create(sFileName, oWriterSettings)
			' Begin writing.
			oXMLWriter.WriteStartDocument()
			oXMLWriter.WriteStartElement("Script") ' Root.
			oXMLWriter.WriteStartElement(mtCurrentScriptData.ActionType.ToString())
			oXMLWriter.WriteAttributeString("Stage", mtCurrentScriptData.Stage.ToString())
			oXMLWriter.WriteAttributeString("Action", mtCurrentScriptData.Action.ToString())



			' Loop over employees in array.

			For Each oParcel As UnidivNet.UD_Parcel In colParcels
				oXMLWriter.WriteStartElement("Parcel")
				oXMLWriter.WriteElementString("DbID", oParcel.DbID.ToString)
				oXMLWriter.WriteElementString("Name", oParcel.UD_Name)
				oXMLWriter.WriteEndElement()
			Next

			' End document.
			oXMLWriter.WriteEndElement()
			oXMLWriter.WriteEndElement()
			oXMLWriter.WriteEndDocument()
		End Using
	End Sub
	Private Sub zzLoadParcelListScript() 'As ICollection(Of UnidivNet.UD_Parcel)  'sFileName As String
		'Dim sFileName As String = zzGetScriptFileName()
		'	Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		'	Dim iOutputActionType As UnidivNet.enActionType
		'Dim iOutputStage As Integer
		'Dim iOutputAction As Integer


		'Dim sFileName As String = zzGetScriptInitData(False, iOutputActionType, iOutputStage, iOutputAction)
		Dim sFileName As String = mtCurrentScriptData.FileName
		Dim oParcel As UnidivNet.UD_Parcel = Nothing

		Dim oXMLDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()

		oXMLDocument.Load(sFileName)
		Dim oNode As Xml.XmlNode = oXMLDocument.DocumentElement
		Dim oParcelsNode As Xml.XmlNode = oNode.FirstChild
		Dim oParcelDbIDNode As Xml.XmlNode
		Dim oParcelNameNode As Xml.XmlNode
		Dim iDbID As Integer
		Dim sParcelName As String
		For Each oNodeParcel As Xml.XmlNode In oParcelsNode.ChildNodes
			iDbID = 0
			oParcelNameNode = oNodeParcel.ChildNodes(1)
			sParcelName = oParcelNameNode.InnerText

			oParcelDbIDNode = oNodeParcel.ChildNodes(0)
			If Integer.TryParse(oParcelDbIDNode.InnerText, iDbID) Then
				If mdicParcels.TryGetValueByDbID(iDbID, oParcel) Then
					'colParcels.Add(oParcel)
					If Not mhsSelectedParcels.Contains(oParcel) Then
						mhsSelectedParcels.Add(oParcel)
						mlstSelectedParcels.Add(oParcel)
					End If

					'	zzMarkParcel(oParcel, -1, False)
				End If
			End If

		Next
		'DMCommon.Debug.MsgBox("11_165", mlstSelectedParcels.Count)
		zzSelectedParcelsToGrid(True, False)

		'+ToGrid





		'Return colParcels

	End Sub
	Private Sub zzWriteLinkListScript(bAdditionalAction As Boolean, colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, Optional oSourceParcel As UnidivNet.UD_Parcel = Nothing)
		'	Dim sFileName As String = zzGetScriptFileName(sActionLabel)
		'	Dim iOutputActionType As UnidivNet.enActionType
		'	Dim iOutputStage As Integer
		'Dim iOutputAction As Integer


		'Dim sFileName As String = zzGetScriptInitData(bAdditionalAction, iOutputActionType, iOutputStage, iOutputAction)
		Dim sFileName As String = zzGetScriptInitData(False, mtCurrentScriptData.ActionType, mtCurrentScriptData.Stage, mtCurrentScriptData.Action)
		sFileName = mtCurrentScriptData.FileName
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oWriterSettings As Xml.XmlWriterSettings = New Xml.XmlWriterSettings()
		oWriterSettings.Indent = True
		'	DMCommon.Debug.MsgBox("13_146Write", sFileName, colLinks.Count, miCurrentStage, miCurrentAction)
		Using oXMLWriter As Xml.XmlWriter = Xml.XmlWriter.Create(sFileName, oWriterSettings)
			' Begin writing.
			oXMLWriter.WriteStartDocument()
			oXMLWriter.WriteStartElement("Script") ' Root.
			oXMLWriter.WriteStartElement(mtCurrentScriptData.ActionType.ToString())
			oXMLWriter.WriteAttributeString("Stage", mtCurrentScriptData.Stage.ToString())
			oXMLWriter.WriteAttributeString("Action", mtCurrentScriptData.Action.ToString())
			If oSourceParcel IsNot Nothing Then
				oXMLWriter.WriteStartElement("Parcel")
				oXMLWriter.WriteElementString("DbID", oSourceParcel.DbID.ToString)
				oXMLWriter.WriteElementString("Name", oSourceParcel.UD_Name)
				oXMLWriter.WriteEndElement()
			End If
			oXMLWriter.WriteStartElement("Links")
			For Each tLinkAcObjID As ObjectId In colLinks
				oXMLWriter.WriteStartElement("Link")
				oXMLWriter.WriteElementString("AcObjID", tLinkAcObjID.ToString)
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tLinkAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				If oDBObject IsNot Nothing Then
					oXMLWriter.WriteElementString("Handle", oDBObject.Handle.ToString())
				End If

				oXMLWriter.WriteEndElement()
			Next
			oXMLWriter.WriteEndElement()
			oXMLWriter.WriteEndElement()
			oXMLWriter.WriteEndElement()
			oXMLWriter.WriteEndDocument()
		End Using
	End Sub
	Private Function zzGetScriptInitData(bAdditionalAction As Boolean, ByRef iOutputActionType As UnidivNet.enActionType, ByRef iOutputStage As Integer, ByRef iOutputAction As Integer) As String
		Const sDelim As String = "_"
		If bAdditionalAction Then
			iOutputActionType = UnidivNet.enActionType.RestoreLayer
			If miCurrentActionFirstRowIndex = -1 Then
				If miCurrentActionType = UnidivNet.enActionType.Union Then
					iOutputAction = 1
					iOutputStage = miCurrentStage + 1
				ElseIf miCurrentActionType = UnidivNet.enActionType.Divide Then
					iOutputAction = miCurrentAction + 1
					iOutputStage = miCurrentStage
				End If
			Else
				iOutputAction = miCurrentAction
				iOutputStage = miCurrentStage
			End If
		Else
			iOutputAction = miCurrentAction
			iOutputActionType = miCurrentActionType
			iOutputStage = miCurrentStage
		End If




		Return DMAcadExt.AcadDocument.GetCurrentDirectory & "\" & iOutputActionType.ToString() & sDelim & miCurrentStage.ToString() & sDelim & miCurrentAction.ToString() & ".xml"
	End Function
	Private Function zzGetScriptFileName(Optional sActionLabel As String = Nothing) As String
		Const sDelim As String = "_"
		If sActionLabel Is Nothing Then
			sActionLabel = miCurrentActionType.ToString()
		End If

		Return DMAcadExt.AcadDocument.GetCurrentDirectory & "\" & sActionLabel & sDelim & miCurrentStage.ToString() & sDelim & miCurrentAction.ToString() & ".xml"
	End Function
	Private Function zzIsScriptFound(bAdditionalAction As Boolean) As Boolean
		'Dim sFileName As String = zzGetScriptFileName()
		Dim iOutputActionType As UnidivNet.enActionType
		Dim iOutputStage As Integer
		Dim iOutputAction As Integer


		Dim sFileName As String = zzGetScriptInitData(bAdditionalAction, iOutputActionType, iOutputStage, iOutputAction)

		Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sFileName)
		'	DMCommon.Debug.MsgBox("13_146s", sFileName, oFileInfo.Exists)
		Return oFileInfo.Exists
	End Function
	Private Sub zzInsertTransferCentroid(oActionRow As DataRow)
		'	Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim tSourceParcelKey As UD_ParcelKey = zzParcelKeyFrom(oActionRow)
		Dim oDestParcel As UnidivNet.UD_Parcel
		Dim tDestParcelKey As UD_ParcelKey
		'  Dim sParcelNo As String = DirectCast(oFirstActionRow.Item("ctxToParcel"), String)
		Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oActionRow.Item("DestParcelNo"))
		Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oActionRow.Item("DestBlockNo"))
		Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(oActionRow.Item("DestBlockAddNo"))
		'	mtCurrentParcelKey = New UD_ParcelKey(iBlockNo, iBlockAddNo, iParcelNo, False)
		Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(oActionRow)
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim sLayer As String = zzGetCentroidLayer()
		Dim bCurrentLayerOK As Boolean
		'  Integer.TryParse(sParcelNo, iParcelNo)


		'  mtCurrentParcelKey.DebugMsg("BeforeDivide")

		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then

			tDestParcelKey = New UD_ParcelKey(iBlockNo, iBlockAddNo, iParcelNo, False)
			oDestParcel = New UnidivNet.UD_Parcel(oSourceParcel, tDestParcelKey)
			oDestParcel.Stage = miCurrentStage
			oSourceParcel.FinalBlockKey = tDestParcelKey.BlockKey
			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			oAcadBlock.OpenForRight()
			'	DMCommon.Debug.MsgBox("13_017", mdicParcels.Count, oDestParcel.ParcelKey, oDestParcel.IsMoved)

			oAcadBlock.Fields = {"PARCEL_NAME", "GUSH", "PARCEL_PREVIOUS", "GUSH_PREVIOUS", "LEGAL_AREA", "CALC_AREA"}


			'   oooooo   sss()
			tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(oSourceParcel.CenterPosition.AcGePoint3d.X, oSourceParcel.CenterPosition.AcGePoint3d.Y, 0.0) ' + 2.0  + 2.0
			tBlockRefData.AttribValues = {oDestParcel.UD_Name, oDestParcel.BlockFull, tParcelKey.UD_ParcelName, tParcelKey.BlockName, zzAreaToString(oSourceParcel.LegalArea, True), zzAreaToString(oSourceParcel.AcadArea, True)}

			'dddd???

			oDestParcel.HasNewBlock = True
			bCurrentLayerOK = DMAcadExt.AcadTransaction.CreateLayer(sLayer)
			If bCurrentLayerOK Then
				tBlockRefData.Layer = sLayer
			End If

			tBlockRefData.ScaleFactors = mtScale3d

			tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
			'moODTable.SetIntValue(0, miCurrentStage, tCentroidBlockAcObjID)
			moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, String.Empty, 0.0))

			oActionRow.Item("AcObjID") = tCentroidBlockAcObjID
			oActionRow.Item("ParcelDbID") = oDestParcel.DbID
			oDestParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
			'  DMCommon.Debug.MsgBox("11_140b", miCurrentActionFirstRowIndex, oDestParcel.ParcelKey, oSourceParcel.UD_Name)
			oDestParcel.IsMoved = True
			mdicParcels.AddParcel(oDestParcel)

		Else
			DMCommon.Debug.MsgBox("11_140a", miCurrentActionFirstRowIndex, tSourceParcelKey)
		End If
	End Sub
	Private Sub zzSelectedParcelsToGrid(bMarkParcel As Boolean, bDWGOpen As Boolean)
		If miCurrentActionFirstRowIndex <> -1 Then
			Dim oDataRow As DataRow = Nothing
			Dim iRowIndex As Integer
			Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelComparer = New UnidivNet.UD_Parcels.ParcelComparer()
			Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			Dim oFirstDestParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
			Dim iDbID As Integer
			Dim sFirsParcelName As String = "First"
			Dim sLasParcelName As String = "Last"

			DMCommon.Debug.MsgBox("230921k", mlstSelectedParcels.Count)
			mlstSelectedParcels.Sort(oParcelComparer)
			iRowIndex = miCurrentActionFirstRowIndex

			If mlstSelectedParcels.Count > 0 Then
				sFirsParcelName = mlstSelectedParcels.Item(0).UD_Name
				sLasParcelName = mlstSelectedParcels.Item(mlstSelectedParcels.Count - 1).UD_Name

			End If
			'	DMCommon.Debug.MsgBox("12_207b", mlstSelectedParcels.Count, moMainTable.Rows.Count, Me.dgvMain.Rows.Count, sFirsParcelName, sLasParcelName)
			'  DMCommon.Debug.MsgBox("12_880", oFirstDestParcelKey, oFirstDestParcelKey.Exists, mlstSelectedParcels.Count, iRowIndex, miCurrentActionFirstRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount)
			For Each oParcel As UnidivNet.UD_Parcel In mlstSelectedParcels

				iDbID = oParcel.DbID
				If bMarkParcel Then
					zzMarkParcel(oParcel, -1, bDWGOpen)
				End If


				'     DMCommon.Debug.MsgBox("12_881", iRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount, mbDatabound)
				oDataRow = zzGetDataRow(iRowIndex)
				zzFillSourceParcel(oParcel, oDataRow)
				zzParcelBatch(oDataRow)
				iRowIndex += 1
			Next
			If iRowIndex < Me.dgvMain.RowCount Then
				zzAddInputCellsNextRow(iRowIndex)
			End If
		End If
	End Sub


	Private Sub zzSelectedParcelKeysToGrid(bMarkParcel As Boolean, bDWGOpen As Boolean)
		If miCurrentActionFirstRowIndex <> -1 Then
			Dim oDataRow As DataRow = Nothing
			Dim iRowIndex As Integer
			Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
			Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			Dim oFirstDestParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Dim iDbID As Integer
			mlstSelectedParcelKeys.Sort(oParcelComparer)
			iRowIndex = miCurrentActionFirstRowIndex
			'DMCommon.Debug.MsgBox("12_207b", mlstSelectedParcels.Count, iRowIndex, moMainTable.Rows.Count, Me.dgvMain.Rows.Count)
			'  DMCommon.Debug.MsgBox("12_880", oFirstDestParcelKey, oFirstDestParcelKey.Exists, mlstSelectedParcels.Count, iRowIndex, miCurrentActionFirstRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount)
			For Each tParcelKey As UD_ParcelKey In mlstSelectedParcelKeys
				If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
					iDbID = oParcel.DbID
					If bMarkParcel Then
						zzMarkParcel(oParcel, -1, bDWGOpen)
					End If
				Else
					iDbID = 0
				End If
				If False Then   'bNewAction

					miCurrentAction += 1
				End If

				'     DMCommon.Debug.MsgBox("12_881", iRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount, mbDatabound)
				oDataRow = zzGetDataRow(iRowIndex, True, oParcel.DbID)

				zzFillSourceParcel(tParcelKey, iDbID, oDataRow)
				' DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
				zzParcelBatch(oDataRow)
				iRowIndex += 1
			Next
			'	DMCommon.Debug.MsgBox("12_207af", "zzSelectedParcelsToGrid")
			If iRowIndex < Me.dgvMain.RowCount Then
				zzAddInputCellsNextRow(iRowIndex)
			End If
		End If
	End Sub
	Private Sub zzFillJournalRows()
		Dim oDataRow As DataRow = Nothing

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		'	Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		mdicJournalRows.Clear()

		For iRowIndex As Integer = moMainTable.Rows.Count - 1 To 0 Step -1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			tParcelKey = zzParcelKeyFrom(oDataRow)
			'  DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
			If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso Not mdicJournalRows.ContainsKey(tParcelKey) Then

				mdicJournalRows.Add(tParcelKey, iRowIndex)
			End If

		Next

	End Sub
	Private Function zzGridToParcels() As ObjectModel.Collection(Of UnidivNet.UD_Parcel)
		Dim oDataRow As DataRow = Nothing

		'Dim tParcelKey As UD_ParcelKey
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

		For iRowIndex As Integer = miCurrentActionFirstRowIndex To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			'zzGetParcelByDbIDOrKey(oDataRow, True)
			oSourceParcel = zzGetParcelByDbIDOrKey(oDataRow, True)
			'tParcelKey = zzParcelKeyFrom(oDataRow)
			'DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
			If oSourceParcel IsNot Nothing Then
				colParcels.Add(oSourceParcel)
			Else
				DMCommon.Debug.MsgBox("Err #232", "oSourceParcel Is Nothing", zzParcelDbID(oDataRow, False), zzParcelDbID(oDataRow, True))
				DMCommon.Debug.MsgBox("11_165", miCurrentActionFirstRowIndex, moMainTable.Rows.Count - 1, oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"), DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID")), DMCommon.Functions.CIntN(oDataRow.Item("ParcelSourceDbID")))

			End If


		Next
		Return colParcels
	End Function
	Private Function zzGridToParcels_240319() As ObjectModel.Collection(Of UnidivNet.UD_Parcel)
		Dim oDataRow As DataRow = Nothing

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

		For iRowIndex As Integer = miCurrentActionFirstRowIndex To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			tParcelKey = zzParcelKeyFrom(oDataRow)
			'DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
			If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
				colParcels.Add(oParcel)

			End If

		Next
		'DMCommon.Debug.MsgBox("11_167s", colParcels.Count)
		Return colParcels
	End Function
	Private Sub zzGridToSelectedParcelsAAA()
		Dim oDataRow As DataRow = Nothing

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		'	Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

		For iRowIndex As Integer = miCurrentActionFirstRowIndex To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			tParcelKey = zzParcelKeyFrom(oDataRow)
			'DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
			If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
				mlstSelectedParcelKeys.Add(tParcelKey)
				'colParcels.Add(oParcel)

			End If

		Next
		'  DMCommon.Debug.MsgBox("11_167s", colParcels.Count)

	End Sub

	Private Sub zzBeforeUnion(colParcels As ICollection(Of UnidivNet.UD_Parcel))
		'  DMCommon.Debug.MsgBox("11_607a", mdicParcels.Count, colParcels.Count)
		Dim oDataRow As DataRow = Nothing
		Dim iRowIndex As Integer = miCurrentActionFirstRowIndex

		'  Dim hsUnion As HashSet(Of Integer) = New HashSet(Of Integer)
		'  Dim hsFragmentsUnion As HashSet(Of Integer) = New HashSet(Of Integer)

		Dim oFirstActionDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)

		'    Dim sParcelNo As String = DirectCast(oFirstActionRow.Cells.Item("ctxToParcel").Value, String)
		Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionDataRow.Item("DestParcelNo"))

		Dim oDestPgonUnion As tsPgonUnion = Nothing
		Dim oCurrentPgonUnion As tsPgonUnion
		'  Dim oDestPgonUnion As tsPgonUnion


		mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, miBlockAddNo, iParcelNo, False)
		'   mtCurrentParcelKey.DebugMsg("BeforeUnion!!!")

		Dim tResParcelArea As ParcelArea
		Dim sPrevParcelName As String = Nothing
		'   Dim oResParcel As UnidivNet.UD_Parcel
		'  oActionFirstRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim colParcelArea As System.Collections.ObjectModel.Collection(Of ParcelArea) = New System.Collections.ObjectModel.Collection(Of ParcelArea)()

		' DMCommon.Debug.MsgBox("09_676", colParcels.Count, mdicParcels.Count)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!!BeforeUnion", colParcels.Count)
		For Each oSourceParcel As UnidivNet.UD_Parcel In colParcels
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!!SourceParcel", oSourceParcel.Name)
			oSourceParcel.IsCanceled = True

			oSourceParcel.UpdateBlockAttributes()

			If sPrevParcelName Is Nothing Then
				sPrevParcelName = oSourceParcel.UD_Name
			Else
				sPrevParcelName &= "," & oSourceParcel.UD_Name
			End If

			If oDestPgonUnion Is Nothing Then
				oDestPgonUnion = oSourceParcel.GetFragmentsPgonUnion(moFragmentsToposcheme)
				oDestPgonUnion.DebugExcel("!GetFragments")
				oDestPgonUnion.Reset()
			Else
				oCurrentPgonUnion = oSourceParcel.GetFragmentsPgonUnion(moFragmentsToposcheme)
				oDestPgonUnion.AddPgonUnion(oCurrentPgonUnion)
				oCurrentPgonUnion.DebugExcel("!CurrentPgonUnion")
				oDestPgonUnion.DebugExcel("!SumPgonUnion")
			End If
			colParcelArea.Add(oSourceParcel.ParcelArea)
			oDataRow = zzGetDataRow(iRowIndex)
			oDataRow.Item("ParcelSourceDbID") = oSourceParcel.DbID
			oSourceParcel.ParcelKey.ToJournalDataRow(oDataRow, True)
			iRowIndex += 1
		Next
		'Давид Шарп
		moLastActionRow = oDataRow
		oDataRow.Item("RowStatus") = enRowStatus.EndOfAction

		tResParcelArea.CalculateArea(colParcelArea, miRoundDigit)
		zzFillParcelArea(tResParcelArea, True, oFirstActionDataRow, oFirstActionGridRow)


		zzUnion(oDestPgonUnion, tResParcelArea, sPrevParcelName)

		mtCurrentParcelKey.NextParcel()
		zzCloseAction()
		''	zzWriteParcelsInfo("After Union")
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterUnion")


	End Sub


	Private Sub zzOpenJournalTableSchema()
		mbDatabound = True
		Me.chkDataBound.Checked = True
		moMainTable = New System.Data.DataTable("Journal")

		zzOpenJournalDataAdapter()
		moJournalDataAdapter.FillSchema(moMainTable, SchemaType.Source)

		zzAddCalcFields()
		Me.dgvMain.DataSource = moMainTable

	End Sub
	Private Sub zzOpenJournalDataAdapter()
		Const sSPName As String = "GetJournalData"
		moJournalDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)
	End Sub

	Private Sub zzOpenJournalTable()
		If moJournalDBTable Is Nothing Then
			moJournalDBTable = New Data.DataTable("JournalDB")
			zzOpenJournalDataAdapter()
			moJournalDataAdapter.Fill(moJournalDBTable)
		End If

	End Sub

	Private Sub zzAddDBJournalData(iAddDataType As enAddDataType)
		Dim iCurrentOper As Integer = moMainTable.Rows.Count

		If iCurrentOper < moJournalDBTable.Rows.Count Then
			Dim sFilter As String
			Dim oDataRow As DataRow
			Dim oTempView As System.Data.DataView
			If iAddDataType = enAddDataType.AllData Then
				sFilter = "Oper > " & iCurrentOper.ToString()
			Else
				oDataRow = moJournalDBTable.Rows.Item(iCurrentOper)
				miCurrentStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
				If iAddDataType = enAddDataType.StageData Then
					sFilter = "Stage = " & miCurrentStage.ToString()

				ElseIf iAddDataType = enAddDataType.ActionData Then
					miCurrentAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
					sFilter = "Stage = " & miCurrentStage.ToString() & " AND Action = " & miCurrentAction.ToString()
				Else
					sFilter = String.Empty
				End If
			End If

			oTempView = New DataView(moJournalDBTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
			zzMyMerge(oTempView.ToTable())
		Else
			DMCommon.Debug.MsgBox("12_877F", iAddDataType, miCurrentStage, miCurrentAction, iCurrentOper, moJournalDBTable.Rows.Count)
		End If
	End Sub
	Private Sub zzMyMerge(oDataTable As Data.DataTable)
		Dim oSourceRow As DataRow
		For Each oRow As DataRow In oDataTable.Rows
			oSourceRow = moMainTable.NewRow
			For iColIndex As Integer = 0 To oDataTable.Columns.Count - 1
				oSourceRow.Item(iColIndex) = oRow.Item(iColIndex)
			Next
			Try
				moMainTable.Rows.Add(oSourceRow)
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("Err #282", oEx.Message, oEx.GetType())
			End Try

		Next
	End Sub

	Private Sub zzLoadDBPoints()
		Const sSPName As String = "GetPoints"
		Dim iPlanID As Integer
		Dim sPointName As String
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sSPName, CommandType.StoredProcedure, zzGetParameters(0))

		Dim iPointNumber As Integer





		mdicPoints = New UnidivNet.UD_Points()
		'	DMCommon.ExcelLogAWW.Open()
		'DMCommon.Debug.MsgBox("13_103Y")
		'DMCommon.ExcelLogAWW.SetNextValue(2, oDataReader IsNot Nothing, miLastPointIdDflt)
		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read
					iPlanID = oDataReader.GetInt32(2)
					sPointName = oDataReader.GetString(3)
					If Int32.TryParse(sPointName, iPointNumber) Then
						If iPlanID = miPlanID Then
							If iPointNumber < miThisPlanMinDBNum Then
								miThisPlanMinDBNum = iPointNumber
							End If
						Else
							If iPointNumber > miMaxPointDBNum Then
								miMaxPointDBNum = iPointNumber
							End If

						End If
					End If
					If iPlanID = miPlanID Then



					Else
						mdicPoints.AddForeignName(sPointName)
					End If

					'	DMCommon.ExcelLogAWW.SetNextValue(0, miPlanID, iPlanID, sPointName, iThisPlanPointID, iMinDBNum)

				Loop


			End If
			oDataReader.Close()
		End If




	End Sub
	Private Sub zzCalcStartPointNum()
		Dim iStartPointNum As Integer
		Dim s As String = String.Empty


		iStartPointNum = miStartPointNumDflt
		If miThisPlanMinDBNum <> Integer.MaxValue AndAlso miThisPlanMinDBNum > miStartPointNumDflt Then
			iStartPointNum = miThisPlanMinDBNum
			Me.chkLastPoint.Checked = False
		Else
			iStartPointNum = miMaxPointDBNum + 1
			miThisPlanMinDBNum = miStartPointNumDflt
			Me.chkLastPoint.Checked = True
		End If

		Me.txtLastPoint.Text = Convert.ToString(miMaxPointDBNum)
		Me.txtStartPoint.Text = Convert.ToString(iStartPointNum)
		zzSetNewPointNo()
	End Sub
	Private Sub zzUpdatePointsTable()


		Const sTableName As String = "Select * FROM Ud_Points"
		Dim oPointsTable As System.Data.DataTable = New System.Data.DataTable("Points")
		Dim oPointsDataAdapter As Data.Common.DbDataAdapter
		Dim oRow As DataRow
		oPointsDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sTableName, CommandType.Text,, False, String.Empty, True, False)
		'   moFragmentsDataAdapter.FillLoadOption
		Try
			oPointsDataAdapter.FillSchema(oPointsTable, SchemaType.Source)

			For Each oUD_Point As UnidivNet.UD_Point In mdicPoints.Values
				If Not oUD_Point.IsRepeating AndAlso oUD_Point.HasName Then
					oRow = oPointsTable.NewRow
					oRow.Item("ProjectCode") = miProjectCode
					oRow.Item("Detail") = miDetailNo
					oRow.Item("PlanID") = miPlanID
					oRow.Item("Name") = oUD_Point.Name
					oRow.Item("IsOriginalName") = oUD_Point.IsOriginalName
					oRow.Item("X") = oUD_Point.Point.X
					oRow.Item("Y") = oUD_Point.Point.Y
					oPointsTable.Rows.Add(oRow)
				End If
			Next
			oPointsDataAdapter.Update(oPointsTable)
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("Err #217", oEx.Message & vbCrLf & oEx.StackTrace)
		End Try



	End Sub

	Private Sub zzLoadFragmentsTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetFragments"

		moFragmentTable = New System.Data.DataTable("Fragments")
		moFragmentsDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)
		'   moFragmentsDataAdapter.FillLoadOption
		If bSchemaOnly Then
			moFragmentsDataAdapter.FillSchema(moFragmentTable, SchemaType.Source)
		Else
			moFragmentsDataAdapter.Fill(moFragmentTable)
		End If
		'   moFragmentTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))


	End Sub
	Private Sub zzLoadParcelFragmentsTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetParcelFragments"

		moParcelFragmentsTable = New System.Data.DataTable("ParcelFragments")
		moParcelFragmentsDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)

		If bSchemaOnly Then
			moParcelFragmentsDataAdapter.FillSchema(moParcelFragmentsTable, SchemaType.Source)
		Else
			moParcelFragmentsDataAdapter.Fill(moParcelFragmentsTable)
		End If
		' moFragmentTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
		If Not bSchemaOnly Then
			'	DMCommon.Debug.MsgBox("1:ParcelFragmentsTable", moParcelFragmentsTable.Rows.Count, bSchemaOnly)
		End If

	End Sub
	Private Sub zzSetDbIDs()
		Dim iDBFragmentID As Integer
		Dim iDWGFragmentID As Integer
		Dim iParcelDbID As Integer
		Dim iParcelNo As Integer

		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oParcelChanged As UnidivNet.UD_Parcel = Nothing

		Dim sFilter As String = "ParcelIsBase = 1"
		Dim oBaseParcelFragmentsDataView As Data.DataView = New DataView(moParcelFragmentsTable, sFilter, String.Empty, DataViewRowState.CurrentRows)

		If oBaseParcelFragmentsDataView.Count = 0 OrElse Not mdicFragments.DBVersionIsCorrect Then
			iParcelDbID = 1

			For Each oParcel In mdicParcels.Values
				'DMCommon.Debug.ExcelLog.SetNextValue(2, "DbIDOldNew", oParcel.DbID, iParcelDbID, oParcel.UD_Name)
				mdicParcels.RemoveByDbID(oParcel.DbID)
				oParcel.DbID = iParcelDbID
				mdicParcels.AddByDbID(oParcel)
				iParcelDbID += 1
			Next
		Else
			mdicParcels.ClearByDbID()

			For Each oRow As DataRowView In oBaseParcelFragmentsDataView
				iDBFragmentID = DirectCast(oRow.Item("FragmentID"), Integer)
				iParcelDbID = DirectCast(oRow.Item("ParcelDbID"), Integer)
				iDWGFragmentID = mdicFragments.GetDWGTopoID(iDBFragmentID)
				iParcelNo = DirectCast(oRow.Item("ParcelNo"), Integer)
				If mdicDWGBaseParcelsByFragments.TryGetValue(iDWGFragmentID, oParcel) Then
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "SetDBParcels", iDBFragmentID, iDWGFragmentID, oParcel.DbID, iParcelDbID, oParcel.UD_Name, oParcel.TmpDbID, oParcel.LegalArea)
					If oParcel.TmpDbID = 0 Then
						If mdicParcels.TryGetValueByDbID(iParcelDbID, oParcelChanged) Then
							mdicFragments.DBVersionIsCorrect = False

							DMCommon.Debug.UserMsg("Msg #211", DMCommon.dmMessages.Message(321, 12, 34), iParcelNo, oParcel.Name, oParcelChanged.Name)
							Exit For
						Else

							oParcel.TmpDbID = iParcelDbID
							oParcel.TmpParcelNo = iParcelNo
							mdicParcels.AddParcelDbID(oParcel)
							'''''''''''''oParcel.IDFromDB = True
						End If

					ElseIf oParcel.TmpDbID <> iParcelDbID Then
						If mdicFragments.DBVersionIsCorrect Then
							DMCommon.Debug.MsgBox("13_191F", "Continuity Failed", moParcelFragmentsTable.Rows.Count, mdicDWGBaseParcelsByFragments.Count)
						End If

						mdicParcels.TryGetValueByDbID(iParcelDbID, oParcelChanged)
						'	DMCommon.Debug.UserMsg("Msg #221", DMCommon.dmMessages.Message(321, 12, 34), iParcelNo, oParcel.Name, oParcel.TmpParcelNo, oParcelChanged.Name, oParcel.TmpDbID, iParcelDbID, iDBFragmentID)
						DMCommon.Debug.UserMsg("Msg #221", DMCommon.dmMessages.Message(321, iParcelNo, oParcel.TmpParcelNo))

						mdicFragments.DBVersionIsCorrect = False
						Exit For
					End If
				Else
					DMCommon.Debug.ExcelLog.SetNextValue(0, "?!ParceFragm", iDWGFragmentID)
				End If
			Next
		End If
		If mdicFragments.DBVersionIsCorrect Then
			mdicParcels.ClearByDbID()
			For Each oPrc As UnidivNet.UD_Parcel In mdicParcels.Values

				oPrc.DbID = oPrc.TmpDbID
				mdicParcels.AddParcelDbID(oPrc)
			Next
		Else
			DMCommon.Debug.MsgBox("09_655l", "DBVersionIsNotCorrect", DMCommon.Debug.ColCount(mdicParcels))
		End If

		'	oNewRow.Item("ParcelNo") = oParcel.ParcelKey.ParcelNo
		'	oNewRow.Item("ParcelIsOriginal") = oParcel.ParcelKey.Original

	End Sub


	Private Sub zzLoadProjectPlanDataTable()

		Dim sComText As String = "SELECT LastParcel FROM dbo.Ud_ProjectPlanData WHERE (ProjectCode = " & miProjectCode & ") And (Detail = " & miDetailNo & ") And (PlanID = " & miPlanID & ")"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)


		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				If Not oDataReader.IsDBNull(0) Then
					Me.txtLastParcel.Text = Convert.ToString(oDataReader.GetInt32(0))
				End If
			Loop
			oDataReader.Close()

		End If

	End Sub


	Private Sub zzLoadProjectDataTable()
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		Dim sComText As String = "Select PlanType FROM dbo.Ud_ProjectData WHERE (ProjectCode = " & miProjectCode & ") And (Detail = " & miDetailNo & ")"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

		Dim s As String = String.Empty
		If oDataReader IsNot Nothing Then
			Do While oDataReader.Read
				If oDataReader.IsDBNull(0) Then
					miPlanType = miPlanTypeDflt
				Else
					'miPlanType = oDataReader.GetInt32(0)

					If [Enum].IsDefined(GetType(frmUD_General.enPlanType), oDataReader.GetInt32(0)) Then
						miPlanType = CType(oDataReader.GetInt32(0), frmUD_General.enPlanType)
					Else
						miPlanType = miPlanTypeDflt
					End If


				End If
			Loop
			oDataReader.Close()
		Else
			miPlanType = miPlanTypeDflt
		End If

		Me.Label4.Text = CStr(miPlanType)
		Me.Text = msCaption & " - " & frmUD_General.GetPlanTypeName(miPlanType)

	End Sub
	Private Sub zzLoadProjectPlanTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetProjectPlanData"

		moProjectPlanTable = New System.Data.DataTable("ProjectPlan")
		moProjectPlanDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)

		If bSchemaOnly Then
			moProjectPlanDataAdapter.FillSchema(moProjectPlanTable, SchemaType.Source)
		Else
			moProjectPlanDataAdapter.Fill(moProjectPlanTable)
		End If



	End Sub

	Private Sub zzCheckFragments()
		If mdicFragments IsNot Nothing AndAlso moFragmentTable IsNot Nothing Then
			mdicFragments.CheckByDBPolygonCount(moFragmentTable.Rows.Count)
			mdicFragments.Calculate(moFragmentTable)
		End If
		' 
	End Sub


	Private Sub zzLoadFragments()

		Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
		Dim oUD_Point As UnidivNet.UD_Point
		Dim oPrevPoint As UnidivNet.UD_Point
		Dim oNextPoint As UnidivNet.UD_Point
		Dim oFLine As UnidivNet.UD_FLine
		Dim oFLineNode As tsNode
		Dim iPointNumber As Integer
		UnidivNet.UD_Point.OldHanit = False
		mdicFLines = New UnidivNet.UD_FLines()
		moFragmentsToposcheme = New TopoManager.TopoScheme.tsTopology(msFragmentsTopoName)
		mdicPoints.OpenMarkBlocks()
		If moFragmentsToposcheme.Load(False, moFragmentTopology) Then


			'DMCommon.Debug.MsgBox("09_109B", "zzLoadFragments-Nodes", moFragmentsToposcheme.Polygons.Count)
			For Each oNode As tsNode In moFragmentsToposcheme.Nodes
				If oNode.AcObjID.IsNull Then
					'DMCommon.Debug.MsgBox("09_110", " oNode.AcObjID.IsNull", oNode.Location)
					'	DMAcadExt.DMApp.MsgBox("09_111", " oNode.AcObjID.IsNull", oNode.Location)
					oBlockRef = Nothing
				Else
					mcolAllNodes.Add(oNode.AcObjID)
					oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(oNode.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				End If

				If oBlockRef IsNot Nothing Then
					oNode.BlockName = oBlockRef.Name
					oUD_Point = New UnidivNet.UD_Point(oBlockRef)
					oUD_Point.FragmentID = oNode.ID
					mdicPoints.AddNode(oUD_Point)
					If oUD_Point.HasName AndAlso Integer.TryParse(oUD_Point.Name, iPointNumber) Then
						If iPointNumber > miMaxPointDBNum Then
							miMaxPointDBNum = iPointNumber
						End If
					End If
				Else
					DMCommon.Debug.MsgBox("09_113", " oBlockRef Is Nothing", oNode.Location)
				End If

			Next
			'	DMCommon.Debug.MsgBox("09_109a", "zzLoadFragments", moFragmentsToposcheme.Polygons.Count)
			'	DMCommon.Debug.MsgBox("09_112", "Points Count = " & mdicPoints.Count.ToString())
			For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
				oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
				oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
				oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
				oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
				oFLine = New UnidivNet.UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
				mdicFLines.Add(oBranch.AcObjID, oFLine)
			Next
			'DMCommon.Debug.MsgBox("09_109MM", "zzLoadFragments after Branches", moFragmentsToposcheme.Branches.Count, mdicFLines.Count, mdicPoints.Count)
			Dim iUndef, i0 As Integer
			For Each oPoint As UnidivNet.UD_Point In mdicPoints.Values
				If oPoint.Stage = -1 Then
					iUndef = iUndef + 1
				ElseIf oPoint.Stage = 0 Then
					i0 += 1
				End If
			Next




			'	DMCommon.Debug.MsgBox("09_109Q", "zzLoadFragments bef OpenHandleDictionary")
			DMAcadExt.AcadTransaction.OpenHandleDictionary()
			'	DMCommon.Debug.MsgBox("09_109R", "zzLoadFragments after OpenHandleDictionary")
			'Do While oDataReader.Read
			'   iFragmentID = oDataReader.GetInt32(0)
			'   iFragmentHandle = oDataReader.GetInt64(5)
			'   tFragmentHandle = New Handle(iFragmentHandle)
			'   tFragmentObjID = DMAcadExt.AcadTransaction.GetObjectID(tFragmentHandle)
			'Loop

			'  moFragmentsToposcheme.Close()
			'  DMCommon.Debug.MsgBox("09_681", DMCommon.Debug.ColCount(mdicPoints), mcolAllNodes.Count, DMCommon.Debug.ColCount(mdicFLines), iUndef, i0)

		End If

	End Sub
	Public Shared Function zzLoadStageNodesSrc(tsStageParcels As tsTopology, iStage As Integer, iNewPointNumber As Integer) As Boolean
		Dim oPoint As UnidivNet.UD_Point = Nothing
		Dim iTest As Integer
		'  Dim sTestName As String = ""
		Dim sPointName As String = ""
		Dim bCurrentLayerOK As Boolean
		Dim sNewPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(iStage, iStage <> 0)
		Dim sOldPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(iStage, iStage = 0)
		If sNewPointLayer IsNot Nothing Then
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewPointLayer, DMAcadExt.DMApp.AppID, True, True)
		End If
		'  DMCommon.Debug.MsgBox("09_551", tsStageParcels.Name, tsStageParcels.Nodes.Count, mdicPoints.Count)
		For Each oNode As tsNode In tsStageParcels.Nodes
			'  DMAcadExt.AcadDocument.WriteDebugMessage("#aP " & tsStageParcels.Name & "; " & oNode.IsPseudo & "; " & oNode.Location.ToString() & ", " & oNode.AcObjID.ToString())
			If mdicPoints.TryGetPoint(oNode.AcObjID, oPoint) Then
				If oPoint.IsOld Then
					oNode.HasOldPoint = True
				End If
				If Not oNode.IsPseudo Then
					If iStage = 0 Then
						If String.IsNullOrEmpty(oPoint.Name) Then
							sPointName = "NoName"
						Else
							sPointName = oPoint.Name
						End If
						'    DMAcadExt.AcadDocument.WriteDebugMessage("#aQ " & sPointName & "; " & oNode.IsPseudo & "; " & oPoint.Coordinates)
					End If
					If oPoint.Stage = -1 Then
						oPoint.Stage = iStage
						'  DMCommon.Debug.MsgBox("09_551M", iStage, oPoint.Name, mdicPoints.Count)
						If iStage = 0 Then
							oPoint.CheckBlock(sOldPointLayer)

						Else
							oPoint.Name = iNewPointNumber.ToString()
							iNewPointNumber += 1
							oPoint.UpdateBlock(sNewPointLayer)
						End If

						'    sTestName &= sTestName & oPoint.Name & ";"
						iTest += 1
					End If
				End If
			End If
		Next
		'  DMCommon.Debug.MsgBox("09_682S", DMCommon.Debug.ColCount(mdicPoints), oPoint.Name, iTest, iNewPointNumber, iStage)
		' DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
	End Function
	Public Function zzLoadStageNodes(tsStageParcels As tsTopology) As Boolean
		Dim oPoint As UnidivNet.UD_Point = Nothing
		Dim iTest As Integer
		'  Dim sTestName As String = ""
		Dim sPointName As String = String.Empty
		Dim bCurrentLayerOK As Boolean
		Dim sNewPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(miCurrentStage, miCurrentStage <> 0)
		Dim sOldPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(miCurrentStage, miCurrentStage = 0)
		If sNewPointLayer IsNot Nothing Then
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewPointLayer, DMAcadExt.DMApp.AppID, True, True)
		End If

		'DMCommon.Debug.MsgBox("09_551+++", tsStageParcels.Name, tsStageParcels.Nodes.Count, mdicPoints.Count)

		For Each oNode As tsNode In tsStageParcels.Nodes
			If mdicPoints.TryGetPoint(oNode.AcObjID, oPoint) Then
				If oPoint.IsOld Then
					oNode.HasOldPoint = True
				End If
				If Not oNode.IsPseudo Then
					If miCurrentStage = 0 Then
						If String.IsNullOrEmpty(oPoint.Name) Then
							sPointName = "NoName"
						Else
							sPointName = oPoint.Name
						End If

					End If
					If oPoint.Stage = -1 Then

						If True Then '????????????????
							moODTable.SetData(oNode.AcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, oPoint.Name, oNode.Layer, 0.0), False)
						End If


						oPoint.Stage = miCurrentStage
						oPoint.Action = miCurrentAction
						If miCurrentStage = 0 And miPlanType <> 1 Then
							oPoint.CheckBlock(sOldPointLayer)

						ElseIf String.IsNullOrEmpty(oPoint.Name) Then
							''''''''''''''''''''''''''    oPoint.Name = iNewPointNumber.ToString()


						End If
						'27/06
						If String.IsNullOrEmpty(oPoint.Name) Then
							mdicPoints.SetNewPointNumber(True, oPoint)

						End If

						oPoint.UpdateBlock(sNewPointLayer)

						iTest += 1
					Else
						oNode.HasUsedPoint = True
					End If
				Else
					If oPoint.Stage <> -1 Then
						oNode.HasUsedPoint = True
					End If

				End If
			End If


		Next

	End Function
	Public Shared Function zzLoadStageBranches(tsStageParcels As tsTopology, iStage As Integer) As Boolean
		Dim oFLine As UnidivNet.UD_FLine = Nothing
		'  DMCommon.Debug.MsgBox("09_551", False, tsStageParcels, tsStageParcels.Nodes, tsStageParcels.Nodes.Count, mdicPoints, mdicPoints.Count)
		If tsStageParcels IsNot Nothing Then
			For Each oBranch As tsBranch In tsStageParcels.Branches
				If mdicFLines.TryGetValue(oBranch.AcObjID, oFLine) Then

					If oFLine.Stage = -1 AndAlso oFLine.MaxPointStage <> -1 AndAlso oFLine.MaxPointStage <= iStage Then
						oFLine.Stage = iStage
						'  DMCommon.Debug.MsgBox("09_574", oFLine.Name, iStage)

						'  oFLine.Name = iNewPointNumber.ToString()
						'  iNewPointNumber += 1
						'  oPoint.UpdateBlock()
					End If
				End If
			Next
		End If

		'    DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
	End Function
	Private Function zzGetParametersByBlock() As System.Data.Common.DbParameter()
		Dim oaParams(3) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, miBlockNo)
		oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)
		'DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function

	Private Function zzGetParameters(Optional iPlanID As Integer = -1) As System.Data.Common.DbParameter()
		Dim oaParams(2) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing
		Dim iParamPlanID As Integer
		If iPlanID = -1 Then
			iParamPlanID = miPlanID
		Else
			iParamPlanID = iPlanID
		End If
		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prPlanID", DbType.Int32, iParamPlanID)
		'oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, miBlockNo)

		'oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)
		'DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
		Return oaParams
	End Function
	Private Sub zzCalcJournalAddFields(bAllStages As Boolean)
		Dim iStage As Integer
		Dim iAction As Integer
		Dim iActionType As UnidivNet.enActionType
		Dim iRowStatus As enRowStatus
		Dim iRowIndex As Integer = 0
		Dim oGridRow As DataGridViewRow = Nothing
		Dim tParcelArea As ParcelArea
		Dim dLegalAreaM As Double
		Dim dAcadAreaM As Double
		Dim tParcelKey As UD_ParcelKey
		'	Dim oParcel As UnidivNet.UD_Parcel
		Dim hsFragments As HashSet(Of Integer) = New HashSet(Of Integer)()
		'   mdicParcels = New UnidivNet.UD_Parcels()
		For Each oDataRow As DataRow In moMainTable.Rows
			iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))

			iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
			iActionType = zzToActionType(oDataRow)
			iRowStatus = zzGetRowStatus(oDataRow)
			dLegalAreaM = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))
			dAcadAreaM = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("AcadArea"))
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
			If dLegalAreaM > System.Double.Epsilon Then
				tParcelArea.CalculateArea(dLegalAreaM, dAcadAreaM, miRoundDigit)
				zzFillParcelArea(tParcelArea, False, oDataRow, oGridRow)

			End If
			tParcelKey = zzGetRowParcelKey(iActionType, oDataRow)


			Select Case iRowStatus
				Case enRowStatus.StartOfAction, enRowStatus.Transfer
					If iAction = 1 Then
						oDataRow.Item("StageCaption") = iStage
					End If
					oDataRow.Item("ActionName") = zzGetActionName(iActionType)
				Case enRowStatus.EndOfAction
					oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
					oGridRow.DividerHeight = miAfterActionDividerHeight
				Case enRowStatus.EndOfStage

					oGridRow.DividerHeight = miAfterStageDividerHeight

			End Select
			iRowIndex += 1
		Next



	End Sub
	Private Sub zzAddCalcFields()


		moMainTable.Columns.Add("StageCaption", GetType(System.Int32))
		moMainTable.Columns.Add("ActionName", GetType(System.String))

		moMainTable.Columns.Add("Tolerance", GetType(System.Double))
		moMainTable.Columns.Add("DiffArea", GetType(System.Double))
		moMainTable.Columns.Add("Deviation", GetType(System.Double))
		moMainTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
		moMainTable.Columns.Add("BorderObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))


		'     moMainTable.Columns.Add("Deviation", GetType(System.Double))


	End Sub
	Private Sub zzCalcFragmentsAddFields()
		Dim lFragmentHandle As Long
		Dim tFragmentHandle As Handle
		Dim tFragmentObjID As ObjectId

		DMAcadExt.AcadTransaction.OpenHandleDictionary()
		For Each oDataRow As DataRow In moFragmentTable.Rows
			If Not IsDBNull(oDataRow.Item("CentroidHandle")) Then
				lFragmentHandle = DirectCast(oDataRow.Item("CentroidHandle"), Long)

				tFragmentHandle = New Handle(lFragmentHandle)
				tFragmentObjID = DMAcadExt.AcadTransaction.GetObjectID(tFragmentHandle)
			End If
		Next

	End Sub
	Private Sub zzBeforeLoadParcels()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim iPgonIndex As Integer = 0
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		UnidivNet.UD_Parcel.Initialize()
		mdicParcels = New UnidivNet.UD_Parcels()
		mdicBaseParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
		mlstInitParcels = New List(Of UD_ParcelKey)()

		If oParcelTopology IsNot Nothing AndAlso oParcelTopology.IsComplete Then  '#12  ' False And 
			'DMCommon.Debug.UserMsg("!Before1", "___")
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oParcelTopology.GetPolygons()
			Dim iParcelUB As Integer = colParcelPgons.Count - 1
			'DMCommon.Debug.UserMsg("!Before2", iParcelUB)

			Dim oaParcels(iParcelUB) As UnidivNet.UD_Parcel
			For Each oPolygon As Polygon In colParcelPgons
				oParcel = New UnidivNet.UD_Parcel(oPolygon)
				' 	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oParcel", oParcel.BlockNo, oParcel.Name, oParcel.TopoID, oParcel.CentroidPoint2d.ToString)
				oParcel.Stage = 0
				If miBlockNo = 0 Then
					miBlockNo = oParcel.BlockNo
					miBlockAddNo = oParcel.BlockAdd
					mtCentroidScaleFactors = oParcel.CentroidScaleFactors
				ElseIf (miBlockNo <> oParcel.BlockNo) OrElse (miBlockAddNo <> oParcel.BlockAdd) Then
					If oParcel.BlockNo <> 0 Then
						DMCommon.Debug.UserMsg("Error #367", miBlockNo, miBlockAddNo, oParcel.BlockNo, oParcel.BlockAdd)
					End If

				End If
				oParcel.Calc()
				If oParcel.Correct Then 'TEMP
					oaParcels(iPgonIndex) = oParcel
					mdicBaseParcels.Add(oPolygon.ID, oParcel)
				Else
					DMCommon.Debug.UserMsg("11_588", "Parcel Is Not correct", miBlockNo, oParcel.BlockNo, oParcel.ParcelKey.ParcelNo)
				End If
				iPgonIndex += 1
				oPolygon.Dispose()
				oPolygon = Nothing
			Next
			'oParcel.Terminate()
			' oParcel = Nothing
			If miBlockNo <> 0 Then
				For Each oSourceParcel As UnidivNet.UD_Parcel In oaParcels
					oSourceParcel.BlockNo = miBlockNo
					oSourceParcel.BlockAdd = miBlockAddNo
					oSourceParcel.UpdateBlockAttributes()
					oSourceParcel.Stage = 0
					mdicParcels.AddParcel(oSourceParcel)
					mlstInitParcels.Add(oSourceParcel.ParcelKey)
				Next
			End If
		Else
			DMCommon.Debug.UserMsg("Err #3016", DMCommon.dmMessages.Message(329, msParcelTopoName))
		End If
	End Sub
	Private Function zzLoadParcelTopology() As Boolean
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim oParcelsScheme As TopoManager.TopoScheme.tsTopology
		Dim bRes As Boolean
		'	DMCommon.Debug.MsgBox("zzLoadParcelTopology_1", oParcelTopology IsNot Nothing, oParcelTopology.IsComplete, UnidivNet.UD_Parcel.AcadBlockDef.AttributeIndices IsNot Nothing, moFragmentsToposcheme IsNot Nothing)
		If oParcelTopology IsNot Nothing AndAlso oParcelTopology.IsComplete Then  '#12  ' False And 
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oParcelTopology.GetPolygons()
			Dim iParcelUB As Integer = colParcelPgons.Count - 1
			Dim iPgonIndex As Integer = 0
			Dim oaParcels(iParcelUB) As UnidivNet.UD_Parcel

			If UnidivNet.UD_Parcel.AcadBlockDef.AttributeIndices IsNot Nothing Then

				oParcelsScheme = New TopoManager.TopoScheme.tsTopology()

				If oParcelsScheme.Load(mbCheckExtended, oParcelTopology) Then
					For Each oNode As tsNode In oParcelsScheme.Nodes
						If oNode.BlockName IsNot Nothing Then
							Select Case oNode.BlockName.ToUpper
								Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
									oNode.HasOldPoint = True
							End Select
						Else
							DMAcadExt.DMApp.MsgBox("Error", oNode.Location.ToString())
						End If

					Next
					'+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

					If mbCheckExtended Then
						oParcelsScheme.RemovePseudoPoints()
					End If


					zzLoadStageNodes(oParcelsScheme)
					zzLoadStageBranches(oParcelsScheme, 0)
					Me.txtGushNo.Text = Convert.ToString(miBlockNo)
					Me.txtGushAddNo.Text = Convert.ToString(miBlockAddNo)


					Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(0)
					Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
					Dim oParcelPgon As Autodesk.Gis.Map.Topology.Polygon
					Dim oFragment As Fragment = Nothing
					DMAcadExt.AcadTransaction.ClearLayerByClassName(sPolylineLayer)

					oParcelsScheme.CalcIsthmus()

					oParcelsScheme.CreateDBPolylineMPlus(True, False)


					For Each oPolygonScheme As tsPolygon In oParcelsScheme.Polygons
						If Not oPolygonScheme.BorderObjID.IsNull AndAlso mdicBaseParcels.TryGetValue(oPolygonScheme.ID, oParcel) Then

							oParcel.BorderAcObjID = oPolygonScheme.BorderObjID
							oParcel.PolygonScheme = oPolygonScheme
							oParcel = Nothing

						End If
					Next

					For Each oParcel In mdicParcels.Values
						oParcel.PolygonScheme = oParcelsScheme.GetPolygon(oParcel.TopoID)
					Next
					If miPlanType = frmUD_General.enPlanType.PlanType1 Then

						For Each oParcel In mdicParcels.Values
							DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcArea", oParcel.LegalArea, oParcel.AcadArea)
							If oParcel.LegalArea = 0 Then
								oParcel.LegalArea = oParcel.AcadArea
							End If

							oParcel.UpdateBlockAttributes()
						Next
					End If
					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''     mdicFragments.CheckByCount(moFragmentsToposcheme.Polygons.Count)

					mdicFragments = New Fragments()
						mdicDWGBaseParcelsByFragments = New Dictionary(Of Integer, UnidivNet.UD_Parcel)


						For Each oFragmentPgonScheme As tsPolygon In moFragmentsToposcheme.Polygons

							Try
								oParcelPgon = oParcelTopology.FindPolygon(oFragmentPgonScheme.Centroid) ''''
								If oParcelPgon IsNot Nothing Then
									oParcel = mdicBaseParcels.Item(oParcelPgon.ID)
									oParcel.AddFragment(oFragmentPgonScheme.ID)
									mdicDWGBaseParcelsByFragments.Add(oFragmentPgonScheme.ID, oParcel)
									''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''  mdicFragments.CheckByScheme(oFragmentPgonScheme, oParcel.ParcelKey.ParcelNo)
									''''''''''''''    mdicParcels.AddFragment(oFragmentPgonScheme.ID, oParcel.ParcelKey)
									oFragment = New Fragment(oFragmentPgonScheme, oParcel.ParcelKey)
									mdicFragments.AddFragment(oFragment)
								End If
							Catch oEx As Exception

							End Try
							oParcel = Nothing
						Next
						mdicBaseParcels.Clear()
						mdicBaseParcels = Nothing


						bRes = True
					Else
						bRes = False
				End If

			Else
				System.Windows.Forms.MessageBox.Show("Parcel Block Is wrong")
				bRes = False
			End If
			colParcelPgons = Nothing
			oParcelTopology.Close()
			oParcelTopology = Nothing

		Else
			If oParcelTopology IsNot Nothing Then
				oParcelTopology.Close()
				oParcelTopology = Nothing
			End If
			System.Windows.Forms.MessageBox.Show("Parcel Topology Is Nothing")
			bRes = False
		End If

		zzSetBlockKey()

		Return bRes
	End Function
	Private Sub zzSetBlockKey()
		miBlockKey = TopoManager.TPlanGraph.BlockData.GetBlockKey(miBlockNo, miBlockAddNo)
	End Sub

	Private Sub zzBeforeDivideNoDB(bAll As Boolean, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		Dim oGridRow As DataGridViewRow = Nothing
		Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
		Dim oFirstActionRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
		Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockNo, 0, oFirstActionRow.Cells.Item(2).Value, oFirstActionRow.Cells.Item(3).Value)
		Dim sParcelNo As String = DirectCast(oFirstActionRow.Cells.Item("ctxToParcel").Value, String)
		Dim iParcelNo As Integer

		Integer.TryParse(sParcelNo, iParcelNo)

		mtCurrentParcelKey = New UD_ParcelKey(iParcelNo, False)
		'  mtCurrentParcelKey.DebugMsg("BeforeDivide")
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(tParcelKey, oParcel) Then

			oGridRow = zzGetRow(iRowIndex)
			oParcel.ParcelKey.ToGrid(oGridRow, True)
			' DMCommon.Debug.MsgBox("08_549", tParcelKey, oParcel.ParcelKey)
			''''''''''''''''''   zzDivideAllBranches(oParcel, bAll, colTopoLinks)
		Else
			DMCommon.Debug.MsgBox("08_551", mdicParcels.Count)
		End If
	End Sub

	Private Sub zzBeforeDivide(bAllLinks As Boolean, iSelectLinkType As enSelectLinkType)
		Dim bRes As Boolean
		Dim oGridRow As DataGridViewRow = Nothing
		Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
		Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim tSourceParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
		Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
		Dim bParcelBatch As Boolean
		Dim oScriptSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim oSourceParcel As UnidivNet.UD_Parcel = zzGetParcelByDbIDOrKey(oFirstActionRow, True)
		'	DMCommon.Debug.MsgBox("250123_1", mdicParcels.Count, iSelectLinkType, mtCurrentScriptData.ParcelDbID)
		If iSelectLinkType = enSelectLinkType.Script AndAlso mtCurrentScriptData.ScriptType = enScriptType.ParcelLinkList AndAlso mdicParcels.TryGetValueByDbID(mtCurrentScriptData.ParcelDbID, oScriptSourceParcel) Then
			If oSourceParcel IsNot Nothing Then

				If oSourceParcel.DbID <> oScriptSourceParcel.DbID Then
					DMCommon.Debug.UserMsg("???1", oSourceParcel.UD_Name, oScriptSourceParcel.UD_Name)
					Return

				End If
			Else
				tSourceParcelKey = oScriptSourceParcel.ParcelKey

				zzFillSourceParcel(tSourceParcelKey, 0, oFirstActionRow)
				zzFillSourceParcel(oSourceParcel, oFirstActionRow)
			End If
		End If
		mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, miBlockAddNo, iParcelNo, False)

		If oSourceParcel IsNot Nothing Then
			bParcelBatch = False
			bRes = zzDivide(oSourceParcel, bAllLinks, iSelectLinkType, bParcelBatch)
		Else
			Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = Nothing
			Dim lstSelectedParcels As List(Of UnidivNet.UD_Parcel) = Nothing


			If zzGetParcelColFromGrid(lstSelectedParcels) Then
				Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelComparer = New UnidivNet.UD_Parcels.ParcelComparer()
				bParcelBatch = True
				lstSelectedParcels.Sort(oParcelComparer)

				For Each oListSourceParcel As UnidivNet.UD_Parcel In lstSelectedParcels
					If miCurrentActionFirstRowIndex = -1 Then
						zzOpenAction(False, UnidivNet.enActionType.Divide)
					End If
					DMCommon.Debug.ExcelLog.SetNextValue(0, msDivideActionName, oListSourceParcel.Name, "FragmentsNum=" & DMCommon.Debug.ColCount(oListSourceParcel.Fragments))
					bRes = zzDivide(oListSourceParcel, bAllLinks, iSelectLinkType, bParcelBatch)
				Next

			End If

		End If

		If Not bRes AndAlso bParcelBatch AndAlso miCurrentActionFirstRowIndex >= 0 Then
			Me.dgvMain.Rows.RemoveAt(miCurrentActionFirstRowIndex)
			miCurrentActionFirstRowIndex = -1
		End If
		miLastRowStatusIsNotDefined = True
	End Sub
	Private Function zzDivide(oSourceParcel As UnidivNet.UD_Parcel, bAllLinks As Boolean, iSelectLinkType As enSelectLinkType, bParcelBatch As Boolean) As Boolean

		Dim sMsg2 As String = DMCommon.dmMessages.GetText(326)
		Dim oDataRow As DataRow = Nothing

		Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
		Dim bDone As Boolean

		If oSourceParcel.Fragments.Count > 1 Then
			oDataRow = zzGetDataRow(iRowIndex)
			zzFillSourceParcel(oSourceParcel, oDataRow)

			zzOpenDWG(True, False, True)
			DMAcadExt.AcadTransaction.MoveToTopOrder(oSourceParcel.BorderAcObjID)
			zzActiveParcelsView(moUD_ParcelLayerListPlusNew, oSourceParcel.DbID)

			zzCloseDWG()

			bDone = zzDivideBranches(oSourceParcel, bAllLinks, iSelectLinkType)

		ElseIf Not bParcelBatch Then
			DMCommon.Debug.UserMsg("08_553", sMsg2, DMCommon.dmMessages.Message(305, oSourceParcel.UD_Name, oSourceParcel.BlockNo))
		End If


		Return bDone
	End Function

	Private Function zzDivide(tParcelKey As UD_ParcelKey, bAllLinks As Boolean, iSelectLinkType As enSelectLinkType, bParcelBatch As Boolean) As Boolean
		Dim sMsg1 As String = DMCommon.dmMessages.GetText(327)
		Dim sMsg2 As String = DMCommon.dmMessages.GetText(328)


		Dim oGridRow As DataGridViewRow = Nothing
		Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
		Dim bDone As Boolean
		'Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		' Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(oFirstActionRow.Item("OriginalParcelNo"), oFirstActionRow.Item("NewParcelNo"))
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(tParcelKey, oSourceParcel) Then
			DMCommon.Debug.MsgBox("13_118", oSourceParcel.Fragments.Count)
			If oSourceParcel.Fragments.Count > 1 Then

				oGridRow = zzGetRow(iRowIndex)
				oSourceParcel.ParcelKey.ToGrid(oGridRow, True)
				'
				zzOpenDWG(True, False, True)
				'   Dim bIsOff As Boolean, iColor As Autodesk.AutoCAD.Colors.Color
				DMAcadExt.AcadTransaction.MoveToTopOrder(oSourceParcel.BorderAcObjID)

				'  DMAcadExt.AcadTransaction.GetLayerInfo(msLinkNewLayer, bIsOff, iColor)
				DMCommon.Debug.MsgBox("12_282---", "OperView #5")
				zzActiveParcelsView(moUD_ParcelLayerListPlusNew, oSourceParcel.DbID)
				'  zzActiveParcelsView(moUD_ParcelLayerList, oSourceParcel.DbID)

				'  DMAcadExt.AcadTransaction.GetLayerInfo(msLinkNewLayer, bIsOff, iColor)
				DMCommon.Debug.MsgBox("12_282RRR", "OperView #5")

				zzCloseDWG()

				'DMCommon.Debug.MsgBox("08_549", bAllLinks, iSelectLinkType, tParcelKey, oSourceParcel.ParcelKey.UD_ParcelName)

				bDone = zzDivideBranches(oSourceParcel, bAllLinks, iSelectLinkType)
				'DebugTopoExists("14_001", zzGetCurrentStageTopoName())
				'DMCommon.Debug.MsgBox("13_035F3", oSourceParcel.ParcelKey, bDone)

			ElseIf Not bParcelBatch Then
				DMCommon.Debug.UserMsg("08_553", sMsg2, DMCommon.dmMessages.Message(305, tParcelKey.UD_ParcelName, tParcelKey.BlockNo))
			End If

		Else
			DMCommon.Debug.UserMsg("08_552L", sMsg1, tParcelKey.BlockNo, tParcelKey.BlockAdd, tParcelKey.UD_ParcelName, miBlockAddNo)
			' For Each tKey As UD_ParcelKey In mdicParcels.Keys
			' DMCommon.Debug.MsgBox("08_55a2", mdicParcels.Count, tKey.BlockNo, tKey.UD_ParcelName, tKey.CompareKey)
			' Next
		End If
		Return bDone
	End Function
	Private Sub zzTransfer()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oActionRow As DataRow
		Dim oGridRow As DataGridViewRow
		Dim tSourceParcelKey As UD_ParcelKey
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		Dim tDestParcelKey As UD_ParcelKey
		Dim iParcelNo As Integer
		Dim iBlockNo As Integer
		Dim iBlockAddNo As Integer
		For iRowIndex As Integer = miCurrentActionFirstRowIndex To moMainTable.Rows.Count - 1

			oActionRow = moMainTable.Rows.Item(iRowIndex)
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
			tSourceParcelKey = zzParcelKeyFrom(oActionRow)
			tDestParcelKey = zzParcelKeyTo(oActionRow)
			iParcelNo = DMCommon.Functions.CIntN(oActionRow.Item("DestParcelNo"))
			iBlockNo = DMCommon.Functions.CIntN(oActionRow.Item("DestBlockNo"))
			iBlockAddNo = DMCommon.Functions.CIntN(oActionRow.Item("DestBlockAddNo"))


			If tSourceParcelKey.Exists AndAlso tDestParcelKey.Exists Then
				If mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then
					If Not oSourceParcel.IsCanceled Then
						zzTransferFromFinalTopology(tSourceParcelKey, oActionRow, oGridRow, iRowIndex)
					End If
				End If
				zzInsertTransferCentroid(oActionRow)
			End If
			mtCurrentParcelKey = New UD_ParcelKey()
		Next
	End Sub

	Private Sub zzTransferSingle()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
		Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)

		Dim tSourceParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
		Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
		'Dim oDestParcel As UnidivNet.UD_Parcel
		Dim tDestParcelKey As UD_ParcelKey = zzParcelKeyTo(oFirstActionRow)
		Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
		Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockNo"))
		Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockAddNo"))


		If tSourceParcelKey.Exists AndAlso tDestParcelKey.Exists Then
			If mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then
				If Not oSourceParcel.IsCanceled Then
					zzTransferFromFinalTopology(tSourceParcelKey, oFirstActionRow, oFirstActionGridRow, miCurrentActionFirstRowIndex)
				End If
			End If
			zzInsertTransferCentroid(oFirstActionRow)
		End If
	End Sub

	Private Function zzFragmentSet2ObjectIdCol(hsTopoElements As HashSet(Of Integer)) As ObjectIdCollection
		Dim oElement As tsElement
		Dim colResObjectIDs As ObjectIdCollection = New ObjectIdCollection()
		For Each iTopoID As Integer In hsTopoElements
			oElement = moFragmentsToposcheme.Elements.Item(iTopoID)
			If oElement.AcObjID.IsNull Then
				colResObjectIDs.Add(oElement.AcObjID)
			End If
		Next
		Return colResObjectIDs
	End Function
	Private Function zzGetNewWithoutFocused(hsNewFocusedBranches As HashSet(Of Integer)) As ObjectIdCollection
		Dim colResObjectIDs As ObjectIdCollection = New ObjectIdCollection()
		For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
			If Not hsNewFocusedBranches.Contains(oBranch.ID) AndAlso (oBranch.Layer = msLinkNewLayer OrElse oBranch.Layer.ToUpper = msLinkNewBridgeLayer) Then
				colResObjectIDs.Add(oBranch.AcObjID)
			End If
		Next
		Return colResObjectIDs
	End Function
	Private Function zzDivideBranches(oSourceParcel As UnidivNet.UD_Parcel, bAllLinks As Boolean, iSelectLinkType As enSelectLinkType) As Boolean
		Dim oPgonUnion As tsPgonUnion
		Dim oPgon As tsPolygon
		Dim colAllTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
		Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim oDestParcel As UnidivNet.UD_Parcel = Nothing
		Dim bOKContunue As Boolean = False

		oPgonUnion = moFragmentsToposcheme.NewPgonUnion()
		'''''15/01
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		moFragmentsToposcheme.RefreshBranches()

		For Each iFragmentId As Integer In oSourceParcel.Fragments
			oPgon = moFragmentsToposcheme.GetPolygon(iFragmentId)
			oPgonUnion.AddPolygon(oPgon)
		Next

		'''''15/01
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		Dim hsInnerBranches As HashSet(Of Integer) = oPgonUnion.InnerBranches
		Dim hsBoundaryBranches As HashSet(Of Integer) = oPgonUnion.BoundaryBranches
		Dim oListDivided As List(Of UD_ParcelKey) = Nothing
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim hsDestPgonIDs As HashSet(Of Integer) = Nothing
		Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel) = Nothing
		Dim hsLinkLayers As HashSet(Of String) = New HashSet(Of String)(New String() {msLinkNewLayer, msLinkBridgeLayer.ToUpper(), msLinkNewBridgeLayer.ToUpper()})
		If bAllLinks Then
			DMAcadExt.AcadDocument.ClearDrawVectorSet()
			colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

			zzGetTopoElements(moFragmentsToposcheme, hsInnerBranches, colTopoLinks, colNodes, hsLinkLayers, mcolBlockedLinks)
			zzOpenDWG(True, True)
			If colTopoLinks.Count > 0 Then

				mcolNewLinksOutOfSourceParcel = zzGetNewWithoutFocused(hsInnerBranches)
				DMAcadExt.AcadTransaction.SetVisible(mcolNewLinksOutOfSourceParcel, False)  ''''''''''''''''''''''''''''''040724
				bOKContunue = True
			End If
		Else

			colAllTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			zzGetTopoElements(moFragmentsToposcheme, hsInnerBranches, colAllTopoLinks, colNodes, hsLinkLayers)
			zzOpenDWG(True, True)
			mcolNewLinksOutOfSourceParcel = zzGetNewWithoutFocused(hsInnerBranches)
			DMAcadExt.AcadTransaction.SetVisible(mcolNewLinksOutOfSourceParcel, False)  ''''''''''''''''''''''''''''''040724
			colTopoLinks = zzGetLinkSet(colAllTopoLinks, iSelectLinkType, oSourceParcel)
			bOKContunue = colTopoLinks.Count <> 0
		End If

		If bOKContunue Then
			zzMarkParcel(oSourceParcel, -1, False)
			oSourceParcel.IsCanceled = True
			oSourceParcel.UpdateBlockAttributes()
			zzWriteLinkListScript(False, colTopoLinks, oSourceParcel)
			zzGetTopoElements(moFragmentsToposcheme, hsBoundaryBranches, colTopoLinks, colNodes)

			If Me.rdbHanit.Checked Then
				miCurrentCentroidStatus = enCentroidStatus.FromNewLayer
			ElseIf Me.rdbTaba.Checked Then
				miCurrentCentroidStatus = enCentroidStatus.FromTaba
			Else
				miCurrentCentroidStatus = enCentroidStatus.New
			End If

			''''''''''''''''''''DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			If zzCreateDivideStageTopology(oSourceParcel, colTopoLinks, mcolAllNodes, oListDivided, hsDestPgonIDs, dicDestParcels, oSourceParcel.UD_Name) Then
				If miCurrentStage <333 Then
					zzCreatePgonsPlus(hsDestPgonIDs)
				End If

				For Each oPolygonScheme As tsPolygon In moCurrentStageTopoScheme.Polygons
					If Not oPolygonScheme.BorderObjID.IsNull AndAlso dicDestParcels.TryGetValue(oPolygonScheme.ID, oDestParcel) Then
						oDestParcel.BorderAcObjID = oPolygonScheme.BorderObjID
						oDestParcel.PolygonScheme = oPolygonScheme
					End If
				Next

				If oListDivided IsNot Nothing Then
					zzFragmentsAllocationNew(oPgonUnion, oListDivided)
					zzCalcDivideResParcels(oSourceParcel, oListDivided, False)
				End If

				zzCloseAction()
			Else
				bOKContunue = False
				DMCommon.Debug.MsgBox("13_035F", bOKContunue)
			End If

		Else
			' DMCommon.Debug.MsgBox("12_085", "miCurrentActionFirstRowIndex", miCurrentActionFirstRowIndex)
		End If
		zzRestoreView()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		DMAcadExt.AcadDocument.Regen()
		Return bOKContunue
	End Function
	Private Function zzDivideDestBlockExists() As Boolean
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim colLayerCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

		colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
		If colCentroidsBlocks.Count > 0 Then
			Return True
		Else
			colLayerCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
			For Each tBlockRefObjID As ObjectId In colLayerCentroidsBlocks
				If Not zzIsStageTopoElement(tBlockRefObjID) Then
					Return True
				End If
			Next
		End If
		Return False

	End Function
	Private Function zzGetDivideDestBlocks() As ObjectIdCollection
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim colLayerCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		If Me.rdbHanit.Checked Then
			colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
			colLayerCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
			For Each tBlockRefObjID As ObjectId In colLayerCentroidsBlocks
				If Not zzIsStageTopoElement(tBlockRefObjID) Then
					colCentroidsBlocks.Add(tBlockRefObjID)
				End If
			Next

		ElseIf Me.rdbTaba.Checked Then
			colCentroidsBlocks = New ObjectIdCollection()
		Else
			colCentroidsBlocks = New ObjectIdCollection()
		End If

		Return colCentroidsBlocks



	End Function

	Private Sub zzFragmentsAllocationNew(oPgonUnion As tsPgonUnion, Optional ByRef oListDividedAAA As List(Of UD_ParcelKey) = Nothing)
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim oTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sStageTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim oPolygon As Polygon
		Dim sParcelName As String = Nothing

		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		'	Dim tParcelKey As UD_ParcelKey
		Dim tFragmentCentroid As Autodesk.AutoCAD.Geometry.Point3d
		Dim oTestDB As DBObject
		Dim sTestRxClassName As String
		''''''''''''''''''''''''''''''''''''''''''     oListDivided = New List(Of UD_ParcelKey)


		For Each oFragmentPgonScheme As tsPolygon In oPgonUnion.FragmentPgons
			Try
				tFragmentCentroid = oFragmentPgonScheme.Centroid
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "1:FragmentsAllocation-frmUnidiv")
			End Try

			Try
				oPolygon = oTopology.FindPolygon(tFragmentCentroid)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				'	DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "FragmentsAllocation-frmUnidiv", DMCommon.dmMessages.Message(311, sStageTopoName))
				DMAcadExt.DMApp.MsgBox("13_018", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), tFragmentCentroid)

				oPolygon = Nothing
			End Try


			If oPolygon IsNot Nothing Then
				oTestDB = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				If oTestDB Is Nothing Then
					sTestRxClassName = "2NOTHING"
				Else
					sTestRxClassName = oTestDB.GetRXClass.Name
				End If



				'	DMCommon.Debug.MsgBox("09_997zM", sStageTopoName, oPolygon.ID, oPolygon.Entity, sTestRxClassName, oPolygon.Topology.Name)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "09_997zM", sStageTopoName, oPolygon.ID, oPolygon.Entity, sTestRxClassName, oPolygon.Topology.Name)
				If mdicParcels.TryGetValueByObjID(oPolygon.Entity, oParcel) Then
					'DMCommon.Debug.MsgBox("09_998zz", oPolygon.Entity.ToString(), oParcel.DbID, oParcel.Name, oFragmentPgonScheme.ID)
					oParcel.AddFragment(oFragmentPgonScheme.ID)
					mdicFragments.SetParcel(oFragmentPgonScheme.ID, oParcel.ParcelKey)
					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   oListDivided.Add(oParcel.ParcelKey)
				Else
					DMCommon.Debug.MsgBox("09_991qq", mdicParcels.Count, oPolygon.Entity, miBlockNo, sParcelName, oFragmentPgonScheme.ID)

					Dim array(mdicParcels.ParcelObjectIDs.Keys.Count - 1) As ObjectId
					mdicParcels.ParcelObjectIDs.Keys.CopyTo(array, 0)
					Dim colObjIDs As ObjectIdCollection = New ObjectIdCollection(array)

					'	DMAcadExt.DMApp.MsgBox("09_991qq", mdicParcels.Count, oPolygon.Entity, miBlockNo, oFragmentPgonScheme.ID)
					'DMAcadExt.DMApp.MsgBox("09_991YY", DMAcadExt.DMApp.GetListArray(colObjIDs))


					DMAcadExt.AcadDocument.WriteDebugMessage("997qq: " & mdicParcels.Count & "; " & oPolygon.Entity.ToString())
				End If
				'   DMCommon.Debug.MsgBox("11_370", miBlockNo, tParcelKey.UD_ParcelName, tParcelKey.BlockNo, tParcelKey.UD_ParcelName, tParcelKey.CompareKey)
				'For Each tKey As UD_ParcelKey In mdicParcels.Keys
				'   DMCommon.Debug.MsgBox("11_400", miBlockNo, tKey.UD_ParcelName, tKey.BlockNo, tKey.UD_ParcelName, tKey.CompareKey)
				'Next

			End If
		Next
		oTopology.Close()
	End Sub

	Private Sub zzGetTopoElements(hsBranches As HashSet(Of Integer), ByRef colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, Optional sLinkLayer As String = Nothing)
		Dim oBranch As tsBranch
		Dim oNode As tsNode
		colNodes = New ObjectIdCollection()
		If Not String.IsNullOrEmpty(sLinkLayer) Then
			sLinkLayer = sLinkLayer.ToUpper
		End If
		'  mUnidiv.vb() : Line 2548
		For Each iBranchID As Integer In hsBranches
			oBranch = moFragmentsToposcheme.GetBranch(iBranchID)
			If oBranch IsNot Nothing Then
				If String.IsNullOrEmpty(sLinkLayer) OrElse oBranch.Layer.ToUpper() = sLinkLayer Then
					colTopoLinks.Add(oBranch.AcObjID)
					oNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
					oNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
				End If
			Else
				DMAcadExt.AcadDocument.WriteDebugMessage("BranchID: " & iBranchID.ToString() & "; Link was not found")
			End If
		Next
	End Sub
	Private Sub zzGetTopoElements(ByRef oFragmentsToposcheme As tsTopology, hsBranches As HashSet(Of Integer), ByRef colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection,
		ByRef colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, Optional hsLinkLayers As HashSet(Of String) = Nothing, Optional colBlockedLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing)
		Dim oBranch As tsBranch
		Dim oNode As tsNode
		Dim iDebug1 As Integer
		Dim sDebug2 As String
		Dim oDebugEntity As Entity
		Dim bDebugClose As Boolean


		If Not DMAcadExt.AcadTransaction.IsActive Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			bDebugClose = True
		End If

		For Each iBranchID As Integer In hsBranches
			oBranch = oFragmentsToposcheme.GetBranch(iBranchID)
			If oBranch IsNot Nothing Then
				If (hsLinkLayers Is Nothing OrElse hsLinkLayers.Contains(oBranch.Layer.ToUpper())) And (colBlockedLinks Is Nothing OrElse Not colBlockedLinks.Contains(oBranch.AcObjID)) Then
					colTopoLinks.Add(oBranch.AcObjID)
					oNode = oFragmentsToposcheme.GetNode(oBranch.PreviousNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
					oNode = oFragmentsToposcheme.GetNode(oBranch.NextNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
				Else
					iDebug1 += 1
					oDebugEntity = DMAcadExt.AcadTransaction.GetEntity(oBranch.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					If oDebugEntity Is Nothing Then
						sDebug2 = "Br-Nothing"
					Else
						sDebug2 = oDebugEntity.Layer
					End If
				End If
			Else
				DMAcadExt.AcadDocument.WriteDebugMessage("BranchID: " & iBranchID.ToString() & "; Link was not found")
			End If
		Next

		If bDebugClose Then
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'     DMCommon.Debug.MsgBox("12_070", hsBranches.Count, colTopoLinks.Count, colNodes.Count)
	End Sub
	Private Sub zzGetTopoElements1(ByRef oFragmentsToposcheme As tsTopology, hsBranches As HashSet(Of Integer), ByRef colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, Optional sLinkLayer As String = Nothing, Optional sLinkLayerAdd As String = Nothing)
		Dim oBranch As tsBranch
		Dim oNode As tsNode
		Dim iDebug1 As Integer
		Dim sDebug2 As String
		Dim oDebugEntity As Entity
		Dim bDebugClose As Boolean
		If Not String.IsNullOrEmpty(sLinkLayer) Then
			sLinkLayer = sLinkLayer.ToUpper
		End If

		If Not DMAcadExt.AcadTransaction.IsActive Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			bDebugClose = True
		End If

		For Each iBranchID As Integer In hsBranches
			oBranch = oFragmentsToposcheme.GetBranch(iBranchID)
			If oBranch IsNot Nothing Then
				If String.IsNullOrEmpty(sLinkLayer) OrElse oBranch.Layer.ToUpper() = sLinkLayer OrElse (sLinkLayerAdd IsNot Nothing AndAlso oBranch.Layer.ToUpper() = sLinkLayerAdd) Then
					colTopoLinks.Add(oBranch.AcObjID)
					oNode = oFragmentsToposcheme.GetNode(oBranch.PreviousNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
					oNode = oFragmentsToposcheme.GetNode(oBranch.NextNodeID)
					If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
						colNodes.Add(oNode.AcObjID)
					End If
				Else
					iDebug1 += 1
					oDebugEntity = DMAcadExt.AcadTransaction.GetEntity(oBranch.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					If oDebugEntity Is Nothing Then
						sDebug2 = "Br-Nothing"
					Else
						sDebug2 = oDebugEntity.Layer
					End If
					'DMCommon.ExcelLogAW5.SetNextValue(0, "Layer out", iDebug1, iBranchID, sDebug2, oBranch.Layer, hsBranches.Count, oBranch.AcObjID, colTopoLinks.Count)
				End If
			Else
				DMAcadExt.AcadDocument.WriteDebugMessage("BranchID: " & iBranchID.ToString() & "; Link was not found")
			End If
		Next

		If bDebugClose Then
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'     DMCommon.Debug.MsgBox("12_070", hsBranches.Count, colTopoLinks.Count, colNodes.Count)
	End Sub

	Private Sub zzUnion(oDestPgonUnion As tsPgonUnion, tResParcelArea As ParcelArea, sPrevParcelName As String)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing '= oFragmentsTopoScheme.GetDissolvedLinksBy(hsUnion)
		Dim colCancelledLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
		Dim colIsthmusLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing


		Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing ' = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()

		colTopoLinks = moFragmentsToposcheme.GetBranchesObjIDs(oDestPgonUnion.BoundaryBranches)
		DMAcadExt.AcadUtil.AddObjectIDCollection(colTopoLinks, mcolBlockedLinks)

		colCancelledLinks = moFragmentsToposcheme.GetBranchesObjIDs(oDestPgonUnion.NewInnerBranches)
		colIsthmusLinks = moFragmentsToposcheme.GetIsthmusObjIDs(oDestPgonUnion.InnerBranches, oDestPgonUnion.NewInnerBranches)
		zzAddCancelledLinks(colCancelledLinks)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLinkCancelledLayer, DMAcadExt.DMApp.AppID, True, False)
		zzSetLayer(colCancelledLinks, msLinkCancelledLayer)
		If colTopoLinks.Count <> 0 Then
			DMCommon.Debug.MsgBox("06_152", colIsthmusLinks.Count)
			zzCreateUnionStageTopology(colTopoLinks, colIsthmusLinks, mcolAllNodes, sPrevParcelName)
			'''''''!!!!!'''''''
			zzCreatePgonsPlus()

			Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			Dim oPgonScheme As tsPolygon
			Dim oDestParcel As UnidivNet.UD_Parcel = Nothing
			If mdicParcels.TryGetValue(mtCurrentParcelKey, oDestParcel) Then
				oDestParcel.Fragments = oDestPgonUnion.Polygons
				oDestParcel.CalculateArea(tResParcelArea.LegalArea, tResParcelArea.AcadArea)
				oDestParcel.PrevParcelName = sPrevParcelName
				oDestParcel.UpdateBlockAttributes()
				mdicFragments.SetParcel(oDestPgonUnion.Polygons, oDestParcel.ParcelKey)
			Else
				oDestParcel = New UnidivNet.UD_Parcel(moCurrentStageTopoScheme.Polygons.Item(0), mtCurrentParcelKey)
			End If

			oFirstActionRow.Item("AcObjID") = oDestParcel.CentroidAcObjID
			oFirstActionRow.Item("ParcelDbID") = oDestParcel.DbID

			oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oDestParcel.TopoID)

			oFirstActionRow.Item("BorderObjID") = oPgonScheme.BorderObjID
			oDestParcel.BorderAcObjID = oPgonScheme.BorderObjID
		End If

	End Sub
	Private Sub zzSetLayer(colAcadObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sLayer As String)
		Dim oEntity As Entity
		Dim sLabel As String = "Start"
		For Each tAcObjID As ObjectId In colAcadObjIDs
			oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			Try
				oEntity.Layer = sLayer
				sLabel = "SetLayer"
				moODTable.SetData(oEntity, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, oEntity.Layer, 0.0))

			Catch oEx As Exception
				DMCommon.Debug.MsgBox("Err #931", oEx.Message, sLabel, oEx.GetType(), oEntity.Layer, sLayer)
			End Try

		Next
	End Sub


	Private Sub zzSetLayer(tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, sLayer As String, bUpdateScriptTable As Boolean)
		Dim oEntity As Entity

		oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		Try
			If bUpdateScriptTable Then
				moODTable.SetData(oEntity, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, oEntity.Layer, 0.0))
			End If

			oEntity.Layer = sLayer
		Catch oEx As Exception
			DMCommon.Debug.UserMsg("zzSetLayer", oEx.Message, oEx.StackTrace, oEx.GetType().ToString(), sLayer)
		End Try


	End Sub


	Private Sub zzAddCancelledLinksOld(colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		Try
			For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colCancelledinks
				mcolCanceledLinks.Add(tAcObjID)
			Next
		Catch oEx As Exception

		End Try
	End Sub
	Private Sub zzAddCancelledLinks(colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		DMAcadExt.AcadUtil.AddObjectIDCollection(mcolCanceledLinks, colCancelledinks)
	End Sub
	Private Sub zzCreateFinalTopo()
		If Not zzFinalTopologyIsCreated() Then
			Dim iCnt As Integer
			Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion
			Dim oDestPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing
			Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
			Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			Dim hsParcelBoundaryBranches As HashSet(Of Integer)
			Dim hsAllBoundaryBranches As HashSet(Of Integer) = New HashSet(Of Integer)()

			Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
			For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
				'   
				If Not oParcel.IsCanceled AndAlso oParcel.Fragments.Count > 0 Then
					oPgonUnion = oParcel.GetFragmentsPgonUnion(moFragmentsToposcheme)
					'   DMCommon.Debug.MsgBox("15_888", iCnt, mdicParcels.Count, oParcel.IsCanceled, oParcel.UD_Name, oPgonUnion.Polygons.Count)
					hsAllBoundaryBranches.UnionWith(oPgonUnion.BoundaryBranches)
					If oDestPgonUnion Is Nothing Then
						oDestPgonUnion = oPgonUnion
					Else
						oDestPgonUnion.AddPgonUnion(oPgonUnion)
					End If

					colCentroids.Add(oParcel.CentroidAcObjID)
					iCnt += 1
				End If
			Next
			hsParcelBoundaryBranches = oDestPgonUnion.BoundaryBranches

			colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			zzGetTopoElements(hsAllBoundaryBranches, colTopoLinks, colNodes)


			Dim bRes As Boolean = zzCreateTopology(UnidivNet.UD_App.FinalTopoName, colTopoLinks, colCentroids)

			If Not bRes Then
				DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			End If

		Else
			'DMCommon.Debug.MsgBox("15_889", "Final OK!")
		End If
	End Sub
	Private Sub zzDebugTopoExists(sLabel As String, sName As String)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		DMCommon.Debug.MsgBox(sLabel, sName, oTopos.Exists(sName))
	End Sub


	Private Function zzCreateDivideStageTopologyDB(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UnidivNet.UD_Parcel), ByRef dicDestParcelsP As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String, dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion)) As Boolean


		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim oParcel As UnidivNet.UD_Parcel
		Dim sStageCentroidLayer As String = zzGetCentroidLayer()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oCentroidPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
		Dim oTopoModel As TopologyModel
		Dim oCentroidDBObject As DBObject
		Dim oFragmentPgon As Polygon
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection '= New ObjectIdCollection()
		Dim colCentroidAcadPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()

		Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim oPolygonP As Polygon = Nothing
		'	Dim colParcelBlocks As ObjectIdCollection = zzGetNewParcelBlocks()
		Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
		dicDestParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()

		colCentroidsBlocks = zzGetDivideDestBlocks()


		If oTopos.Exists(sStageTopoName) Then
			oTopos.Delete(sStageTopoName, False)
		End If

		Try
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, CreateOptions.HighlightErrors)

		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateDivStageTopoDB", sPrevParcelName & vbCrLf & sStageTopoName & vbCrLf & DMCommon.Debug.ColCount(colTopoLinks))
			DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			Return False
		End Try
		DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
		If oTopos.Exists(sStageTopoName) Then


			oAcadBlock.OpenForRight()
			oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
			oTopoModel = oTopos(sStageTopoName)
			oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

			oListDivided = New List(Of UnidivNet.UD_Parcel)


			Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

			If oFragmentTopology IsNot Nothing Then
				Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing

				Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()


				For Each oPolygon As Polygon In colParcelPgons
					Try
						oFragmentPgon = oFragmentTopology.FindPolygon(oPolygon.Centroid)
					Catch oEx As Exception
						oFragmentPgon = Nothing
					End Try
					If oFragmentPgon IsNot Nothing AndAlso dicParcelByFragments.TryGetValue(oFragmentPgon.ID, oPgonUnion) Then

						oParcel = New UnidivNet.UD_Parcel(oPolygon, oPgonUnion.ParcelDbID, oPgonUnion.ParcelKey)
						'	DMCommon.Debug.ExcelLog.SetValue(0, "!NewDivPrc", oPgonUnion.ParcelDbID, oPgonUnion.ParcelKey, oParcel.DbID, oParcel.UD_Name)
						oParcel.Stage = miCurrentStage
						oParcel.IsOriginal = False
						oParcel.PrevParcelName = sPrevParcelName
						oParcel.Fragments = oPgonUnion.Polygons
						oParcel.SetForcedArea(oPgonUnion.ForcedArea)

						oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
							Select Case oCentroidDBObject.GetRXClass.Name
								Case DMAcadExt.AcadConst.AcadBlockRefName
									oParcel.ReadNewBlockAttributes()
								Case DMAcadExt.AcadConst.AcadPointName
									oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
									colCentroidPoints.Add(oPolygon.Entity)
									tBlockRefData.Position = oCentroidPoint.Position
							End Select
							oParcel.SetGush(miBlockNo, miBlockAddNo)
							oParcel.IsOriginal = False

							tBlockRefData.Layer = sStageCentroidLayer

							If oParcel.AcadPoint Then
								colCentroidAcadPoints.Add(oParcel.CentroidAcObjID)
								tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
								moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, UnidivNet.Ud_ScriptODTable.ScriptData.NewObject, String.Empty, 0.0))

								oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
							End If
							colCentroidsBlocks.Add(oParcel.CentroidAcObjID)
							oParcel.Calc()

							oListDivided.Add(oParcel)
							dicDestParcels.Add(oParcel.TopoID, oParcel)
							mdicParcels.AddParcel(oParcel)

						Else
							DMAcadExt.AcadDocument.WriteDebugMessage("!!oFragmentPgon.ID= " & oFragmentPgon.ID & "; " & dicParcelByFragments.Count.ToString)
					End If
				Next
				colParcelPgons = Nothing
			End If

			moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
			moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
			' 

			dicDestParcelsP = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
			For Each oParcel In dicDestParcels.Values
				oPolygonP = oTopoModel.FindPolygon(oParcel.CentroidPoint3d)
				oParcel.TopoID = oPolygonP.ID

				dicDestParcelsP.Add(oParcel.TopoID, oParcel)
				'NB 190218
				If colCentroidsBlocks.Contains(oParcel.CentroidAcObjID) Then
					colCentroidsBlocks.Remove(oParcel.CentroidAcObjID)
				End If
			Next


			oTopoModel.Close()
			oFragmentTopology.Close()



			If colCentroidPoints.Count > 0 Then
				oTopos.Delete(sStageTopoName, False)
				DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
				oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)
			End If


			DMAcadExt.AcadDocument.Regen()
		End If
	End Function
	Private Function zzCreateDivideStageTopologyDB_240319(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UD_ParcelKey), ByRef dicDestParcelsP As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String, dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion)) As Boolean


		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim oParcel As UnidivNet.UD_Parcel
		Dim sStageCentroidLayer As String = zzGetCentroidLayer()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oCentroidPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
		Dim oTopoModel As TopologyModel
		Dim oCentroidDBObject As DBObject
		Dim oFragmentPgon As Polygon
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection '= New ObjectIdCollection()
		Dim colCentroidAcadPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()

		Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim oPolygonP As Polygon = Nothing
		'	Dim colParcelBlocks As ObjectIdCollection = zzGetNewParcelBlocks()
		Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
		dicDestParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
		'    DMCommon.Debug.MsgBox("11_619d", colTopoLinks.Count, dicParcelByFragments.Count)

		For Each iFragmentID As Integer In dicParcelByFragments.Keys
			DMAcadExt.AcadDocument.WriteDebugMessage("-*oFragmID: " & iFragmentID)
		Next
		'	If Not zzIsStageTopoElement(tBlockRefObjID) Then
		'colCentroidsBlocks.Add(tBlockRefObjID)
		'End If
		colCentroidsBlocks = zzGetDivideDestBlocks()

		Try
			'DMCommon.Debug.MsgBox("15_125b", sStageTopoName, miCurrentCentroidStatus, DMCommon.Debug.ColCount(colTopoLinks), DMCommon.Debug.ColCount(colCentroidsBlocks))
		Catch ex As Exception

		End Try

		If oTopos.Exists(sStageTopoName) Then
			oTopos.Delete(sStageTopoName, False)
		End If
		Try
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, CreateOptions.HighlightErrors)

		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateDivStageTopoDB", sPrevParcelName & vbCrLf & sStageTopoName & vbCrLf & DMCommon.Debug.ColCount(colTopoLinks))
			'DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
			DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			Return False
		End Try
		DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
		If oTopos.Exists(sStageTopoName) Then

			'  Dim oPointList As IList(Of TPlnPoint) = Nothing
			'Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

			oAcadBlock.OpenForRight()
			oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
			oTopoModel = oTopos(sStageTopoName)
			oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
			If miCurrentActionType = UnidivNet.enActionType.Divide Then
				oListDivided = New List(Of UD_ParcelKey)
			End If

			Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

			If oFragmentTopology IsNot Nothing Then
				Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing

				Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
				'  DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count)
				'    Dim sX As String = "X"  
				'  DMCommon.Debug.MsgBox("11_607c", mdicParcels.Count, colParcelPgons.Count)

				For Each oPolygon As Polygon In colParcelPgons
					Try
						oFragmentPgon = oFragmentTopology.FindPolygon(oPolygon.Centroid)
					Catch oEx As Exception
						oFragmentPgon = Nothing
					End Try
					If oFragmentPgon IsNot Nothing AndAlso dicParcelByFragments.TryGetValue(oFragmentPgon.ID, oPgonUnion) Then

						oParcel = New UnidivNet.UD_Parcel(oPolygon, 0, oPgonUnion.ParcelKey)
						'DMCommon.Debug.MsgBox("11_210T!", oPgonUnion.ParcelKey, oPgonUnion.ParcelKey.Original, oParcel.ParcelKey, oParcel.ParcelKey.Original, oParcel.IsOriginal)

						oParcel.Stage = miCurrentStage
						oParcel.IsOriginal = False
						oParcel.PrevParcelName = sPrevParcelName
						oParcel.Fragments = oPgonUnion.Polygons
						oParcel.SetForcedArea(oPgonUnion.ForcedArea)
						'DMCommon.Debug.MsgBox("11_210U", oParcel.ParcelKey, oParcel.ParcelKey.Original, oParcel.CentroidAcObjID, oParcel.AcadPoint, oParcel.Name, oParcel.UD_Name, miBlockNo, oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.ParcelNo, oPgonUnion.ParcelKey.Original)

						oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
						'  DMCommon.Debug.MsgBox("11_210k", oParcel.CentroidAcObjID, oParcel.AcadPoint, oParcel.Name, miBlockNo, oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo)
						Select Case oCentroidDBObject.GetRXClass.Name
							Case DMAcadExt.AcadConst.AcadBlockRefName
								oParcel.ReadNewBlockAttributes()
							Case DMAcadExt.AcadConst.AcadPointName
								oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
								colCentroidPoints.Add(oPolygon.Entity)
								tBlockRefData.Position = oCentroidPoint.Position
						End Select
						oParcel.SetGush(miBlockNo, miBlockAddNo)
						oParcel.IsOriginal = False
						'   oCentroidPoint = DMAcadExt.AcadTransaction.GetAcadPoint(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

						tBlockRefData.Layer = sStageCentroidLayer

						If oParcel.AcadPoint Then
							colCentroidAcadPoints.Add(oParcel.CentroidAcObjID)
							''''>????????????   ;;;;;;;; oParcel.InsertBlock(oPolygon.Centroid)
							tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
							'	moODTable.SetIntValue(0, miCurrentStage, tCentroidBlockAcObjID)
							moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, UnidivNet.Ud_ScriptODTable.ScriptData.NewObject, String.Empty, 0.0))

							oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
						End If
						colCentroidsBlocks.Add(oParcel.CentroidAcObjID)
						'   mtCurrentParcelKey
						'  tBlockRefData.ScaleFactors = oBlockRef.ScaleFactors
						'    DMCommon.Debug.MsgBox("11_210N", oParcel.CentroidAcObjID, oParcel.AcadPoint, oParcel.Name, miBlockNo, oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo)


						''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())
						'
						'  DMCommon.Debug.MsgBox("09_955A", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
						oParcel.Calc()

						'  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}


						'   DMCommon.Debug.MsgBox("09_955B", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
						oListDivided.Add(oParcel.ParcelKey)
						dicDestParcels.Add(oParcel.TopoID, oParcel)

						'  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
						'  mtCurrentParcelKey.NextParcel()




						' 
						'  colCentroidsBlocks.Add(tCentroidBlockAcObjID)
						'	DMCommon.Debug.MsgBox("11_210b!", miLastRowIndex, miCurrentAction, miCurrentStage, mdicParcels.Count, oParcel.CentroidAcObjID, oParcel.Name, oParcel.UD_Name, oParcel.ParcelKey, oParcel.ParcelKey.ParcelNo, oParcel.ParcelKey.Original, oPgonUnion.ParcelKey.Original)
						mdicParcels.AddParcel(oParcel)

					Else
						DMAcadExt.AcadDocument.WriteDebugMessage("!!oFragmentPgon.ID= " & oFragmentPgon.ID & "; " & dicParcelByFragments.Count.ToString)
					End If
					' miLastParcel += 1
				Next
				colParcelPgons = Nothing
			End If

			moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
			moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
			' 

			dicDestParcelsP = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
			For Each oParcel In dicDestParcels.Values
				oPolygonP = oTopoModel.FindPolygon(oParcel.CentroidPoint3d)
				oParcel.TopoID = oPolygonP.ID

				dicDestParcelsP.Add(oParcel.TopoID, oParcel)
				'NB 190218
				If colCentroidsBlocks.Contains(oParcel.CentroidAcObjID) Then
					colCentroidsBlocks.Remove(oParcel.CentroidAcObjID)
				End If
			Next


			oTopoModel.Close()
			oFragmentTopology.Close()


			'  zzDicpCol("colCentroidPoints", colCentroidPoints)
			'  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
			'   DMCommon.Debug.MsgBox("11_260", sStageTopoName, colCentroidsBlocks.Count)
			If colCentroidPoints.Count > 0 Then
				oTopos.Delete(sStageTopoName, False)
				DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
				oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)
			End If


			' moStageTopologies.AddTopoName(sStageTopoName)
			DMAcadExt.AcadDocument.Regen()
		End If

	End Function
	Private Function zzCreateUnionStageTopologyDB(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sPrevParcelName As String, dSumOfLegalArea As Double, iDestParcelDbID As Integer, tDestParcelKey As UD_ParcelKey, hsDestFragments As HashSet(Of Integer), ByRef oParcel As UnidivNet.UD_Parcel) As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim sStageCentroidLayer As String = zzGetCentroidLayer()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oCentroidPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
		Dim oTopoModel As TopologyModel
		Dim oCentroidDBObject As DBObject
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Select Case miCurrentCentroidStatus
			Case enCentroidStatus.FromNewLayer
				colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
			Case enCentroidStatus.FromStageLayer
				colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
			Case Else
				colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		End Select
		Try
			If oTopos.Exists(sStageTopoName) Then
				oTopos.Delete(sStageTopoName, False)
			End If
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateStageTopologyDB")
			DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			Return False
		End Try
		DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
		If oTopos.Exists(sStageTopoName) Then

			oAcadBlock.OpenForRight()
			oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
			oTopoModel = oTopos(sStageTopoName)
			oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
			If oTopoModel.GetPolygons().Count > 1 Then
				DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			End If
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
			For Each oPolygon As Polygon In oTopoModel.GetPolygons

				If colParcelPgons.Count = 1 OrElse colParcelPgons.Count = oPolygon.GetBoundary().Count Then

					oParcel = New UnidivNet.UD_Parcel(oPolygon, iDestParcelDbID, tDestParcelKey)
					oParcel.IsOriginal = False
					oParcel.Stage = miCurrentStage
					oParcel.PrevParcelName = sPrevParcelName
					oParcel.Fragments = hsDestFragments 'oPgonUnion.Polygons
					oParcel.CalculateArea(dSumOfLegalArea)

					oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					Select Case oCentroidDBObject.GetRXClass.Name
						Case DMAcadExt.AcadConst.AcadBlockRefName
							oParcel.ReadNewBlockAttributes()
						Case DMAcadExt.AcadConst.AcadPointName
							oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
							colCentroidPoints.Add(oPolygon.Entity)
							tBlockRefData.Position = oCentroidPoint.Position
							tBlockRefData.ScaleFactors = mtCentroidScaleFactors
					End Select

					tBlockRefData.Layer = sStageCentroidLayer

					If miCurrentActionType = UnidivNet.enActionType.Divide Then

						oParcel.Calc() '?????????
					End If

					tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
					moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, String.Empty, 0.0))
					oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
					' 
					colCentroidsBlocks.Add(tCentroidBlockAcObjID)
					mdicParcels.AddParcel(oParcel)
				Else
					DMCommon.Debug.MsgBox("09_164", sStageTopoName, colParcelPgons.Count, oPolygon.GetBoundary().Count)

				End If

			Next
			'End If

			moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
			moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
			' 

			oTopoModel.Close()
			If oParcel IsNot Nothing Then
				oParcel.PolygonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)
			End If
			oTopos.Delete(sStageTopoName, False)
			DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)

			moStageTopologies.AddTopoName(sStageTopoName)
			DMAcadExt.AcadDocument.Regen()
		End If
	End Function


	Private Function zzCreateDivideStageTopology(oSourceParcel As UnidivNet.UD_Parcel, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection _
																, ByRef oListDivided As List(Of UD_ParcelKey), ByRef hsDestPgonIDs As HashSet(Of Integer), ByRef dicDestParcelsP As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String) As Boolean

		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sStageCentroidLayer As String = zzGetCentroidLayer()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oTopoModel As TopologyModel
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim bInsertByPick As Boolean = Me.rdbInsertByPick.Checked
		Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim bParcelSuccess As Boolean
		Dim bCancel As Boolean = False
		Dim dicAcadPointsPgonIDs As Dictionary(Of Integer, Autodesk.AutoCAD.Geometry.Point3d) = New Dictionary(Of Integer, Autodesk.AutoCAD.Geometry.Point3d)()
		Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
		Dim iAllBlocksForInsert As Integer
		DMCommon.Debug.ExcelLog.SetNextValue(5, "!before SetLayersOnExcept", "-----------", "-----------", "-----------", "-----------", False)
		DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, False)
		DMCommon.Debug.ExcelLog.SetNextValue(5, "!After SetLayersOnExcept", "-----------", "-----------", "-----------", "-----------", False)
		Select Case miCurrentCentroidStatus
			Case enCentroidStatus.FromNewLayer
				' colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
				' colCentroidsBlocks = zzCopyObjIDCol(mcolNewLayerCentroidsBlocks)
				'colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
				'Dim taAcObjIDs(mcolNewLayerCentroidsBlocks.Count + mcolTabaCentroidsBlocks.Count - 1) As ObjectId
				'	mcolNewLayerCentroidsBlocks.CopyTo(taAcObjIDs, 0)
				'mcolNewLayerTabaCentroidsBlocks = New ObjectIdCollection(taAcObjIDs)

			Case enCentroidStatus.FromStageLayer
				'colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
			Case enCentroidStatus.FromTaba
				'colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msTabaCentroidBlockName)

			Case Else
				'colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		End Select
		DMAcadExt.AcadTransaction.SetCurrentLayer(zzGetCentroidLayer(), 1, True, False)

		colCentroidsBlocks = zzGetDivideDestBlocks()

		Try
			If oTopos.Exists(sStageTopoName) Then
				oTopos.Delete(sStageTopoName, False)
			End If
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, CreateOptions.HighlightErrors)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateDivideStageTopology", oSourceParcel.Name & " חלקה  " & vbCrLf & "Links - " & CStr(colTopoLinks.Count) & vbCrLf & "Blocks - " & CStr(colCentroidsBlocks.Count))
			DMCommon.Debug.MsgBox("13_131PP", colCentroidsBlocks.Count, colTopoLinks.Count)
			DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			DMCommon.Debug.MsgBox("13_035Ga", DMAcadExt.AcadDocument.PointFormatSaved, DMAcadExt.AcadDocument.PDMode, DMAcadExt.AcadDocument.PDSize)
			DMAcadExt.AcadDocument.RestorePointFormat()
			Return False
		End Try
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)

		If oTopos.Exists(sStageTopoName) Then
			Dim iFragmentID As Integer
			oTopoModel = oTopos(sStageTopoName)
			oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
			If oTopoModel.GetFullEdges.Count = colTopoLinks.Count Then
				oAcadBlock.OpenForRight()
				oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
				oListDivided = New List(Of UD_ParcelKey)
				hsDestPgonIDs = New HashSet(Of Integer)()
				dicDestParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
				Dim oPolygon As Polygon = Nothing
				Dim iPolygonID As Integer
				Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
				Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
				Dim bUpdateScriptTable As Boolean
				Dim colDestParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection
				Dim bCheck As Boolean = oSourceParcel.RingCount > 1
				Dim iParcelWithAcadPoint As Integer = 0
				moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
				colDestParcelPgons = oTopoModel.GetPolygons()
				For iIndex As Integer = 0 To colDestParcelPgons.Count - 1
					oPolygon = colDestParcelPgons.Item(iIndex)
					If bCheck Then
						iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
					End If
					If Not bCheck OrElse oSourceParcel.HasFragment(iFragmentID) Then
						If bCheck Then
							DMAcadExt.AcadDocument.WriteUserMessage("New polygon #" & iIndex.ToString(), iFragmentID, oPolygon.Entity, oPolygon.Area)
						End If

						oParcel = New UnidivNet.UD_Parcel(oPolygon)
						If oParcel.AcadPoint Then
							colCentroidPoints.Add(oParcel.CentroidAcObjID)
							If bInsertByPick Then
								dicAcadPointsPgonIDs.Add(oPolygon.ID, oPolygon.Centroid)
								oParcel.ForcedNumber = True
							Else
								oParcel.SetParcelKey(mtCurrentParcelKey)
								oParcel.HasNewBlock = True
								mtCurrentParcelKey.NextParcel()
								oListDivided.Add(oParcel.ParcelKey)
							End If
							iParcelWithAcadPoint += 1
						Else 'Exist blocks
							If oParcel.ParcelKey.ParcelNo = 0 Then
								oParcel.SetParcelKey(mtCurrentParcelKey)
								mtCurrentParcelKey.NextParcel()
							End If
							oParcel.SetGush(miBlockNo, miBlockAddNo)
							oParcel.IsOriginal = False

							If oParcel.AltBlock Then

							Else

								mdicParcels.AddParcel(oParcel)
							End If
							' 

							oListDivided.Add(oParcel.ParcelKey)
							zzUpdateCurrentParcelKey(oParcel.ParcelKey)
						End If


						dicDestParcels.Add(oParcel.TopoID, oParcel)

						oParcel.PrevParcelName = sPrevParcelName
						oParcel.Stage = miCurrentStage
						oParcel.Calc()

					Else
					End If

					tBlockRefData.Layer = sStageCentroidLayer

				Next
				Dim oTestDB As DBObject
				Dim sTestRxClassName As String
				''''''''''''''''''''''''  By Pick
				If dicAcadPointsPgonIDs.Count > 0 Then ''byPick

					iAllBlocksForInsert = dicAcadPointsPgonIDs.Count
					Me.Visible = False
					AppActivate(moAppWin.Text)
					Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
					DMCommon.Debug.MsgBox("12_120", sStageTopoName)
					Do
						bParcelSuccess = False
						If dicAcadPointsPgonIDs.Count = 1 Then
							iPolygonID = dicAcadPointsPgonIDs.Keys.ElementAt(0)
						Else
							iPolygonID = 0

							DMAcadExt.AcadDocument.WriteMessage(DMCommon.dmMessages.Message(316, dicAcadPointsPgonIDs.Count, iAllBlocksForInsert))
							'
							If dicAcadPointsPgonIDs.Count <= 4 Then
								iSelectStatus = zzSelectPoint(tPoint, dicAcadPointsPgonIDs.Values.ToArray())
							Else
								iSelectStatus = zzSelectPoint(tPoint, True)
							End If

							If iSelectStatus = PromptStatus.OK AndAlso oTopoModel IsNot Nothing Then
								Try
									oPolygon = oTopoModel.FindPolygon(tPoint)
									If oPolygon IsNot Nothing Then
										iPolygonID = oPolygon.ID
									Else
										iPolygonID = 0
									End If

								Catch oMapEx As Autodesk.Gis.Map.MapException
									If oMapEx.ErrorCode = 3 Then
										DMAcadExt.AcadDocument.WriteMessage(DMCommon.dmMessages.Message(317))
									Else

										DMCommon.Debug.MsgBox("12_241E", oMapEx.ErrorCode, oMapEx.Message, oMapEx.StackTrace)
										DMAcadExt.DMApp.MsgBox("12_241E", tPoint, oMapEx.ErrorCode, oMapEx.Message, oMapEx.StackTrace)
									End If

								End Try
							ElseIf iSelectStatus = PromptStatus.Cancel Then
								bCancel = True
								Exit Do
							End If
						End If

						If iPolygonID <> 0 AndAlso dicDestParcels.TryGetValue(iPolygonID, oParcel) Then

							If oParcel.ParcelKey.ParcelNo = 0 Then

								oParcel.SetParcelKey(mtCurrentParcelKey)

								mtCurrentParcelKey.NextParcel()
								DMCommon.Debug.MsgBox("12_244", oParcel.UD_Name)
								zzSetNameToParcels(oParcel, colCentroidsBlocks)
								DMCommon.Debug.MsgBox("12_244a", oParcel.CentroidAcObjID)

								oTestDB = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
								If oTestDB Is Nothing Then
									sTestRxClassName = "!NOTHING"
								Else
									sTestRxClassName = oTestDB.GetRXClass.Name
								End If
								mdicParcels.AddParcel(oParcel)
								zzCloseDWG()
								zzOpenDWG()
								dicAcadPointsPgonIDs.Remove(iPolygonID)
								oListDivided.Add(oParcel.ParcelKey)
							End If
						Else
							DMAcadExt.AcadDocument.WriteMessage(DMCommon.dmMessages.Message(317))
						End If
					Loop While dicAcadPointsPgonIDs.Count <> 0
				End If 'If dicAcadPointsPgonIDs.Count > 0
				''''''''''''''''''''''''End of by Pick


				'Auto New BLOCKS
				If Not bCancel AndAlso Not oParcel.ForcedNumber Then
					For Each oParcel In dicDestParcels.Values
						If oParcel.AcadPoint OrElse oParcel.AltBlock Then
							zzSetNameToParcels(oParcel, colCentroidsBlocks)
							If oParcel.ForcedNumber Then
								mdicParcels.AddCentroid(oParcel.CentroidAcObjID, oParcel)
							Else
								mdicParcels.AddParcel(oParcel)
							End If
							iParcelWithAcadPoint += 1
						Else
							bUpdateScriptTable = Not oParcel.HasNewBlock
							zzSetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer, bUpdateScriptTable)
						End If

					Next
				End If

				oTopoModel.Close()
				If Not bCancel Then
					If iParcelWithAcadPoint <> 0 Then
						oTopos.Delete(sStageTopoName, False)
						DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
						Try
							'--	ReCreate 
							oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateDivStageTopo2", oSourceParcel.Name & " חלקה  " & vbCrLf & "Links - " & CStr(colTopoLinks.Count) & vbCrLf & "Blocks - " & CStr(colCentroidsBlocks.Count))
							DMCommon.Debug.MsgBox("13_131QQ", colCentroidsBlocks.Count, colTopoLinks.Count)
							DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
							Me.Visible = True
							DMAcadExt.AcadDocument.RestorePointFormat()

							Return False

						End Try
					End If
					If oTopos.Exists(sStageTopoName) Then
						oTopoModel = oTopos(sStageTopoName)
						oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
						moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
						''''''''''''   zzTopoInfo(oTopoModel, "!!Blocks ")
						dicDestParcelsP = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
						For Each oParcel In dicDestParcels.Values
							oPolygon = oTopoModel.FindPolygon(oParcel.CentroidPoint3d)
							oParcel.TopoID = oPolygon.ID
							oParcel.CentroidAcObjID = oPolygon.Entity
							hsDestPgonIDs.Add(oParcel.TopoID)
							dicDestParcelsP.Add(oParcel.TopoID, oParcel)
							'NB 190218
							If colCentroidsBlocks.Contains(oParcel.CentroidAcObjID) Then
								colCentroidsBlocks.Remove(oParcel.CentroidAcObjID)
							End If
						Next
						oTopoModel.Close()
					Else
						DMCommon.Debug.UserMsg("13_131T", "Topology '" & sStageTopoName & "' was not found")
					End If

					DMAcadExt.AcadDocument.Regen()
				End If
				moFragmentTopology.Close()
				Me.Visible = True
				DMAcadExt.AcadDocument.RestorePointFormat()
				DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, True)

				Return Not bCancel
			Else
				DMCommon.Debug.MsgBox("13_047c", oTopoModel.GetFullEdges.Count, colTopoLinks.Count)

			End If
			DMAcadExt.AcadDocument.RestorePointFormat()
			DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, True)
			Return True
		Else

			Return False
		End If
		DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
	End Function
	Private Sub zzCreateScriptODTableNew()
		moODTable = New UnidivNet.Ud_ScriptODTable()
		moODTable.CreateScriptODTable()



	End Sub

	Private Sub zzCreateScriptODTable()
		moODTable = New UnidivNet.Ud_ScriptODTable()
		Dim oaFieldDef(1) As Autodesk.Gis.Map.ObjectData.FieldDefinition
		If Not moODTable.Exists Then
			oaFieldDef(0) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("Stage", "Stage Number", 0)
			oaFieldDef(1) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("SourceName", "Source Name", "")

			'	oaFieldDef(1) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("LuseName", "", "")
			'	oaFieldDef(2) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("Area", "", 0.0)
			'	oaFieldDef(3) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("PlanName", "", "")
			moODTable.CreateTable(oaFieldDef)
		End If
	End Sub
	Private Sub zzSetNameToParcels(ByRef oParcel As UnidivNet.UD_Parcel, ByRef colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId

		oAcadBlock.OpenForRight()
		oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
		tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), zzAreaToString(oParcel.LegalArea, True), zzAreaToString(oParcel.CalcArea, True), "sPrevParcelName"}
		tBlockRefData.Position = oParcel.CentroidPoint3d
		tBlockRefData.ScaleFactors = mtCentroidScaleFactors
		tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
		If Not tCentroidBlockAcObjID.IsNull Then

			oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
			'	DMCommon.Debug.MsgBox("12_265a", oParcel.CentroidAcObjID, oParcel.UD_Name)
			colCentroidsBlocks.Add(tCentroidBlockAcObjID)
			'moODTable.SetIntValue(0, miCurrentStage, tCentroidBlockAcObjID)
			moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, String.Empty, 0.0))
		End If


	End Sub
	Private Sub zzTopoInfo(oTopoModel As TopologyModel, sLabel As String)
		For Each oPgon As Polygon In oTopoModel.GetPolygons
			DMAcadExt.AcadDocument.WriteDebugMessage(sLabel & oPgon.ID & "; " & oPgon.Centroid.ToString())
		Next

	End Sub

	Private Function zzCreateUnionStageTopology(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colIsthmusLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sPrevParcelName As String) As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sStageCentroidLayer As String = zzGetCentroidLayer()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		Dim oCentroidPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
		Dim oTopoModel As TopologyModel
		Dim oCentroidDBObject As DBObject
		Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Select Case miCurrentCentroidStatus
			Case enCentroidStatus.FromNewLayer
				colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcelLayer)
			Case enCentroidStatus.FromStageLayer
				colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
			Case Else
				colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		End Select
		Try
			If oTopos.Exists(sStageTopoName) Then
				oTopos.Delete(sStageTopoName, False)
			End If
			oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea)
			DMCommon.Debug.MsgBox("15_125k", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count, colNodes.Count)

			'   iStageLinksCount = colLinks.Count
		Catch oMapEx As Autodesk.Gis.Map.MapException
			Dim sAddInfo As String = "Links: " & colTopoLinks.Count.ToString() & vbCrLf
			sAddInfo &= "Centroids: " & colCentroidsBlocks.Count.ToString() & vbCrLf
			sAddInfo &= "Nodes: " & colNodes.Count.ToString()
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "zzCreateUnionStageTopology", sAddInfo)

			DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
			Return False
		End Try


		Dim oTestDbobject As DBObject
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
		If oTopos.Exists(sStageTopoName) Then

			'  Dim oPointList As IList(Of TPlnPoint) = Nothing
			'    Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			Dim iFragmentID As Integer
			oAcadBlock.OpenForRight()
			oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
			oTopoModel = oTopos(sStageTopoName)
			oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

			''''''''''''''''''''''''	bb moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
			Dim colFullEdges As FullEdgeCollection = oTopoModel.GetFullEdges()
			Dim colResNodes As NodeCollection = oTopoModel.GetNodes()
			Dim iConnectedDomains As Integer = colParcelPgons.Count + colResNodes.Count - colFullEdges.Count
			DMCommon.Debug.MsgBox("15_125D", sStageTopoName, colTopoLinks.Count, colIsthmusLinks.Count, iConnectedDomains)
			If iConnectedDomains = 2 Then
				DMAcadExt.AcadTransaction.SetExceptionLayer(colTopoLinks)
				If True Then
					oTopoModel.Close()
					If oTopos.Exists(sStageTopoName) Then
						oTopos.Delete(sStageTopoName, False)
					End If
					DMAcadExt.AcadUtil.AddObjectIDCollection(colTopoLinks, colIsthmusLinks)
					oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea)
					oTopoModel = oTopos(sStageTopoName)
					oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					colFullEdges = oTopoModel.GetFullEdges()
					colParcelPgons = oTopoModel.GetPolygons()
				End If

			End If
			Dim colEdgesIDs As ObjectIdCollection = New ObjectIdCollection()
			For Each oFullEdge As FullEdge In colFullEdges
				colEdgesIDs.Add(oFullEdge.Entity)
			Next

			DMCommon.Debug.MsgBox("15_125a", sStageTopoName, colParcelPgons.Count, colFullEdges.Count, colTopoLinks.Count, colCentroidsBlocks.Count, iConnectedDomains)
			For Each oPolygon As Polygon In colParcelPgons
				If colParcelPgons.Count = 1 OrElse colParcelPgons.Count = oPolygon.GetBoundary().Count Then
					iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
					oParcel = New UnidivNet.UD_Parcel(oPolygon, 0, mtCurrentParcelKey)
					oTestDbobject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					oParcel.IsOriginal = False
					oParcel.PrevParcelName = sPrevParcelName
					oParcel.Stage = miCurrentStage

					If oParcel.AcadPoint Then
						oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
						oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
						colCentroidPoints.Add(oPolygon.Entity)
						tBlockRefData.Position = oCentroidPoint.Position
						tBlockRefData.ScaleFactors = mtCentroidScaleFactors
					End If

					tBlockRefData.Layer = sStageCentroidLayer

					If oParcel.AcadPoint Then
						tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
						oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
						moODTable.SetData(tCentroidBlockAcObjID, New UnidivNet.Ud_ScriptODTable.ScriptData(miCurrentStage, String.Empty, String.Empty, 0.0))
					Else
						DMAcadExt.AcadTransaction.SetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer)
						zzSetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer, False)
					End If

					''''''''''''''''''''''''''''''''  oParcel.UpdateBlockAttributes()

					colCentroidsBlocks.Add(tCentroidBlockAcObjID)

					mdicParcels.AddParcel(oParcel)
				End If

			Next
			moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
			moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)

			oTopoModel.Close()
			If oParcel IsNot Nothing Then
				oParcel.PolygonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)


				oTopos.Delete(sStageTopoName, False)
				DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
				Try
					oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateTopology: " & sStageTopoName)

				End Try


				DMAcadExt.AcadDocument.Regen()
			End If
		End If
	End Function
	Private Function zzGetFragmentByPoint(tPoint As Autodesk.AutoCAD.Geometry.Point3d) As Integer
		Dim iFragmentID As Integer = 0
		If moFragmentTopology IsNot Nothing AndAlso moFragmentTopology.Status <> Status.Closed Then
			Dim oPolygon As Polygon
			Try
				oPolygon = moFragmentTopology.FindPolygon(tPoint)
				If oPolygon IsNot Nothing Then
					iFragmentID = oPolygon.ID
				End If
			Catch oEx As Exception

			End Try

		End If
		Return iFragmentID
	End Function

	Private Function zzCreateTopology(sTopoName As String, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		' zzGetAllParcelCentroids()
		If colCentroids Is Nothing Then
			colCentroids = New ObjectIdCollection()
		End If
		Try

			oTopos.Create(sTopoName, colTopoLinks, mcolAllNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

			'   iStageLinksCount = colLinks.Count
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateTopology: " & sTopoName)

			Return False
		End Try
		'   DMCommon.Debug.MsgBox("09_774", sFinalTopoName, oTopos.Exists(sFinalTopoName))
		Return oTopos.Exists(sTopoName)
	End Function
	Private Function zzFinalTopologyIsCreated() As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies


		Try
			'   


			Return oTopos.Exists(UnidivNet.UD_App.FinalTopoName)

			'   iStageLinksCount = colLinks.Count
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "CreateTopology")

			Return False
		End Try
		'   DMCommon.Debug.MsgBox("09_774", sFinalTopoName, oTopos.Exists(sFinalTopoName))

	End Function
	Private Function zzCreateTopology(sTopoName As String, bHasPolygons As Boolean, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		' zzGetAllParcelCentroids()

		If oTopos.Exists(sTopoName) Then
			oTopos.Delete(sTopoName, False)
		End If
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()

		Dim iTopoType As Autodesk.Gis.Map.Topology.TopologyTypes
		If bHasPolygons Then
			iTopoType = Autodesk.Gis.Map.Topology.TopologyTypes.Polygon
		Else
			iTopoType = Autodesk.Gis.Map.Topology.TopologyTypes.Linear
		End If
		'DMCommon.Debug.MsgBox("13_022L", sTopoName, colTopoLinks.Count, collNodes.Count, colCentroids.Count)


		Try

			'oTopos.Create(sTopoName, colTopoLinks, mcolAllNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Linear)
			oTopos.Create(sTopoName, colTopoLinks, colNodes, colCentroids, iTopoType)
			'   iStageLinksCount = colLinks.Count
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "3CreateLinearTopology: " & sTopoName)

			'Return False
		End Try

		'   DMCommon.Debug.MsgBox("09_774", sFinalTopoName, oTopos.Exists(sFinalTopoName))
		Return oTopos.Exists(sTopoName)
	End Function
	Private Sub zzDeleteTopology(sTopoName As String)
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		' zzGetAllParcelCentroids()

		If oTopos.Exists(sTopoName) Then
			oTopos.Delete(sTopoName, False)
		End If




	End Sub
	Private Function zzGetAllParcelCentroids() As ObjectIdCollection
		Dim colAcObjIDs As ObjectIdCollection = New ObjectIdCollection()
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			If oParcel.IsResult Then
				If oParcel.CentroidAcObjID.IsNull Then
					DMCommon.Debug.MsgBox("09_214", oParcel.ParcelKey)
				Else
					colAcObjIDs.Add(oParcel.CentroidAcObjID)
				End If

			End If
		Next
		''''''''''''   mdicParcels.DebugMsg("Final")
		'    DMCommon.Debug.MsgBox("09_222", colAcObjIDs.Count, mdicParcels.Count)
		Return colAcObjIDs
	End Function

	Private Shared Function zzSelectPoint(ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d, bOneOnly As Boolean) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOptDebug As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("")
		'	Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(oPromptOptDebug.AllowNone.ToString() & "," & bOneOnly.ToString() & " Select Point")
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(" Select Point")

		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		oPromptOpt.AllowNone = Not bOneOnly
		''''''''''''''''''''''''''''''''''''''oPromptOpt.AllowArbitraryInput = False

		'    Dim tVector As Autodesk.AutoCAD.Geometry.Vector2d
		'   Dim bRes As Boolean
		'  DMAcadExt.AcadDocument.WriteMessage("Start select Ss10b " & oPromptOpt.AllowNone & "; " & oPromptOpt.AllowArbitraryInput & vbCrLf)
		ptRes = oEditor.GetPoint(oPromptOpt)
		'   DMAcadExt.AcadDocument.WriteMessage("after select Ss10a" & vbCrLf)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = ptRes.Value
				'     tVector = New Autodesk.AutoCAD.Geometry.Vector2d(tPoint.X, tPoint.Y)
				'  bRes = True
				' oEditor.WriteMessage("OK! " & tPoint.ToString() & vbCrLf)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "1:Unidiv-SelectPoint")
			End Try
		End If
		Return ptRes.Status
	End Function
	Private Shared Function zzSelectPoint(ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d, taPoints() As Autodesk.AutoCAD.Geometry.Point3d) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
		'	Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult

		Dim oJig As DMAcadExt.PointerPlineJig = New DMAcadExt.PointerPlineJig(taPoints)


		Dim oPromptResult As PromptResult = oEditor.Drag(oJig)


		'   DMAcadExt.AcadDocument.WriteMessage("after select Ss10a" & vbCrLf)
		If oPromptResult.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = oJig.GetPoint()


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Unidiv - SelectPoint")
			End Try
		End If
		Return oPromptResult.Status
	End Function

	Private Sub zzCreatePgonsPlus(Optional hsDestPgonIDs As HashSet(Of Integer) = Nothing)   '
		'  Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
		Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(miCurrentStage)
		Dim colPolylineObjID As ObjectIdCollection = New ObjectIdCollection()

		'	DMCommon.Debug.MsgBox("15_126con", moCurrentStageTopoScheme Is Nothing, moCurrentStageTopoScheme.Nodes Is Nothing, moStageTopologies Is Nothing, moCurrentStageTopoScheme.Name, sPolylineLayer, miCurrentStage)

		zzLoadStageNodes(moCurrentStageTopoScheme)

		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
		If True Then
			moCurrentStageTopoScheme.RemovePseudoPoints()
		End If

		'	DMCommon.Debug.MsgBox("15_975c", DMCommon.Debug.ColCount(hsDestPgonIDs), sPolylineLayer)
		moCurrentStageTopoScheme.CalcIsthmus()
		'	DMCommon.Debug.MsgBox("15_975d")

		'DMCommon.ExcelLogAW4.SetEnumerable(0, "+hsDestPgonIDs", hsDestPgonIDs)
		moCurrentStageTopoScheme.CreateDBPolylineMPlus(True, False, hsDestPgonIDs)


		''''''''''''''''    zzFillParcelLogTable(colPolylineObjID)

		'   UnidivNet.UD_App.WriteAllPointsInfo()
		'   UnidivNet.UD_App.LoadStageBranches(oStageTopoScheme, miCurrentStage)
		'    UnidivNet.UD_App.InsertFlines(miCurrentStage)





	End Sub
	Private Function zzIsStageTopoElement(tAcObjID As ObjectId) As Boolean
		Dim oTables As Autodesk.Gis.Map.ObjectData.Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
		Dim colRecords As Autodesk.Gis.Map.ObjectData.Records
		Dim sTableName As String
		Try
			colRecords = oTables.GetObjectRecords(0UI, tAcObjID, Autodesk.Gis.Map.Constants.OpenMode.OpenForRead, False)
			If colRecords IsNot Nothing Then
				For Each oRecord As Autodesk.Gis.Map.ObjectData.Record In colRecords
					sTableName = oRecord.TableName
					If sTableName.StartsWith("TPMCNTR_Stage") Then
						Return True
					End If
				Next
			End If
		Catch oMapEx As Autodesk.Gis.Map.MapException
		End Try

		Return False
	End Function
	Private Sub zzFillParcelLogTableAAA(colPolylineObjID As ObjectIdCollection)
		Dim oNewRow As DataRow
		For Each tAcObjID As ObjectId In colPolylineObjID
			oNewRow = moParcelLogTable.NewRow()
			oNewRow.Item("ProjectCode") = miProjectCode
			oNewRow.Item("Detail") = miDetailNo

			oNewRow.Item("OriginalBlockNo") = miBlockNo
			oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

			oNewRow.Item("Stage") = miCurrentStage



			oNewRow.Item("Action") = miCurrentAction
			oNewRow.Item("TopoID") = moParcelLogTable.Rows.Count + 1
			oNewRow.Item("AcObjID") = tAcObjID
			'  oNewRow.Item("BorderHandle")

			moParcelLogTable.Rows.Add(oNewRow)

		Next


	End Sub
	Private Function zzGetRow(iRowIndex As Integer) As DataGridViewRow
		Dim oGridRow As DataGridViewRow = Nothing
		oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
		If oGridRow.IsNewRow Then
			zzAddGridRow(enRowStatus.Default, False)
			''''''''''''''''''''12/03/19 oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
		End If

		Return oGridRow
	End Function
	Private Function zzGetDataRow(iRowIndex As Integer) As DataRow
		Dim oDataRow As DataRow = Nothing
		'DMCommon.Debug.MsgBox("13_016", iRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount, zzTest)
		If iRowIndex = moMainTable.Rows.Count Then
			If zzUseDetachRow() Then  'AndAlso mbNewDataRowUsed = False 
				Return moNewDataRow
			Else
				zzAddDataRow(enRowStatus.Default)
			End If

		End If
		oDataRow = moMainTable.Rows.Item(iRowIndex)
		Return oDataRow
	End Function
	Private Function zzGetDataRow(iRowIndex As Integer, bParcelIsSource As Boolean, iParcelDbID As Integer) As DataRow
		Dim oDataRow As DataRow = Nothing
		'DMCommon.Debug.MsgBox("13_016", iRowIndex, moMainTable.Rows.Count, Me.dgvMain.RowCount, zzTest)
		If iRowIndex = moMainTable.Rows.Count Then
			If zzUseDetachRow() Then  'AndAlso mbNewDataRowUsed = False 
				Return moNewDataRow
			Else
				zzAddGridRow(enRowStatus.Default, bParcelIsSource, iParcelDbID)
			End If

		End If
		oDataRow = moMainTable.Rows.Item(iRowIndex)
		Return oDataRow
	End Function
	Private Function zzUseDetachRow() As Boolean
		Return moNewDataRow IsNot Nothing AndAlso moNewDataRow.RowState = DataRowState.Detached AndAlso Me.dgvMain.RowCount - moMainTable.Rows.Count = 2

	End Function
	Public Sub Start()
		Dim oPromptResult As PromptResult
		Dim oPgonJig As UnidivNet.PgonJig
		' Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
		Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		''
		If oFragmentTopology IsNot Nothing Then
			' moShrinkPgonJig = New UnidivNet.ShrinkPgonJig(oParcelTopology)
			oPgonJig = New UnidivNet.PgonJig(oFragmentTopology)
			'  oPgonJig.JigStatus = UnidivNet.enPgonJigStatus.SelectNeighborPgon

			oPromptResult = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)
			If oPromptResult.Status = PromptStatus.OK Then

				'  oPgonJig.Parcel = moShrinkPgonJig.Parcel
				'   oPromptResult = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)

				If oPromptResult.Status = PromptStatus.OK Then

					'  moShrinkPgonJig.JigStatus = UnidivNet.enPgonJigStatus.GetShrinkValue
					'   moShrinkPgonJig.SetSegmentIndecis(oPgonJig.IndexFrom, oPgonJig.IndexTo, oPgonJig.LayerName)
					mcolLine = oPgonJig.LineCollection
					'   DMAcadExt.AcadDocument.WriteMessage("Area0=" & UnidivNet.ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))

					'Dim oParcel As UnidivNet.UD_Parcel

					'  moShrinkPolygon = moShrinkPgonJig.GetEntity()
					'  oParcel = moShrinkPgonJig.Parcel
					'  moShrinkPolygon.ShrinkTo(oParcel.LegalArea + oParcel.Tolerance)

					'   DMAcadExt.AcadDocument.WriteMessage("DestArea=" & UnidivNet.ShrinkPolygon.DispArea(oParcel.LegalArea + oParcel.Tolerance))
					'  DMAcadExt.AcadDocument.WriteMessage("Area1=" & UnidivNet.ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))
					'		oShrinkPgonJig.Shrink(15.0)
					'	DMAcadExt.AcadDocument.WriteMessage("Area2=" & CStr(oShrinkPgonJig.Area))
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
					'  moShrinkPgonJig.GetEntity().AddToDatabase()

					DMAcadExt.AcadTransaction.CloseModelSpace()

					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()


					'	oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)
					'	DMAcadExt.AcadDocument.WriteMessage("!Out: " & oPromptResult.Status.ToString() & ":" & oShrinkPgonJig.PromptResult.StringResult & "!")

					'''''''''''''oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)


				End If
				oPgonJig.Terminate()
				oPgonJig = Nothing
			End If

			oFragmentTopology.Close()
			'	moShrinkPgonJig.Terminate()
			System.Windows.Forms.MessageBox.Show(oFragmentTopology.Status.ToString(), "01_549")
			oFragmentTopology = Nothing
		End If
	End Sub
	Public Sub ActiveFormView(bVisible As Boolean)
		If mfUD_General IsNot Nothing AndAlso mfUD_General.Visible <> bVisible Then
			mfUD_General.Visible = bVisible
		End If
	End Sub

	Private Sub Button1_Click(oSender As System.Object, e As EventArgs)
		DMCommon.Debug.MsgBox("09_788", miCurrentStage, miCurrentAction)
	End Sub


	Private Sub zzMarkParcel(oParcel As UnidivNet.UD_Parcel, iIndex As Integer, bDWGOpen As Boolean)
		DMAcadExt.AcadDocument.WriteDebugMessage("Start Mark " & vbCrLf)
		If bDWGOpen Then
			zzOpenDWG()
		End If

		Dim oaLinks As DMAcadExt.IUD_Link() = oParcel.GetLinks() ' mdicParcels.GetParcelLinks(oParcel)
		Dim tBorderObjID As ObjectId = oParcel.BorderAcObjID ' mdicParcels.GetParcelLinks(oParcel)
		Dim tCentroidObjID As ObjectId = oParcel.CentroidAcObjID ' mdicParcels.GetParcelLinks(oParcel)
		Dim oCenterPoint As DMAcadExt.TPlnPoint = oParcel.CenterPosition

		If oaLinks IsNot Nothing Then
			Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(oaLinks, oaLinks.GetUpperBound(0), 1, False)
			'  DMAcadExt.AcadDocument.SetDrawVectorSet(New DMAcadExt.DrawVectorSet(tVectorSet))

			DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, iIndex)
		End If
		If Not tCentroidObjID.IsNull Then
			DMAcadExt.AcadDocument.SetDrawPoint(oCenterPoint)
		End If
		'  DMCommon.Debug.MsgBox("09_588", bDWGOpen, oParcel.BorderAcObjID)
		''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   DMAcadExt.AcadTransaction.SetColor(tBorderObjID, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, 251S))

		If bDWGOpen Then
			zzCloseDWG()
		End If

		DMAcadExt.AcadDocument.WriteDebugMessage("End Mark " & vbCrLf)
	End Sub
	Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
		'Dim eui As EditorUserInteraction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.StartUserInteraction(Me)
		Dim oCurrentRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockAddNo, miBlockNo, oCurrentRow.Cells.Item(2).Value, oCurrentRow.Cells.Item(3).Value)
		MessageBox.Show(tParcelKey.ToString(), "09_875a")
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'  moShrinkPgonJig.GetEntity().AddToDatabase()

		Dim oaLinks As DMAcadExt.IUD_Link() = mdicParcels.GetParcelLinks(tParcelKey)
		Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(oaLinks, oaLinks.Count, 1, False)
		DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, -1)


		DMAcadExt.AcadTransaction.CloseModelSpace()

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub zzOpenDWG(Optional bModelSpace As Boolean = True, Optional bFragmentTopology As Boolean = True, Optional ByVal bOpenDrawOrderTable As Boolean = False)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		If bModelSpace Then
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, bOpenDrawOrderTable)
		End If
		If bFragmentTopology Then
			moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology(msFragmentsTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		End If


	End Sub
	Private Sub zzCloseDWG()
		If moFragmentTopology IsNot Nothing Then
			moFragmentTopology.Close()
			moFragmentTopology = Nothing
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		'   DMAcadExt.AcadDocument.RestoreVarCmdDia()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()


	End Sub

	Private Sub cmdInsertFLine_Click(oSsender As System.Object, e As EventArgs) Handles cmdInsertFLine.Click
		Me.Cursor = Cursors.WaitCursor
		zzCloseLastRow()
		zzOpenDWG()
		zzInsertAllFLines()
		'zzCreateFinalTopo()
		zzCloseDWG()
		zzAllEnabled(False)
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub zzInsertFlines(iStage As Integer)
		Dim iTest As Integer
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UnidivNet.UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)

		For Each oFLine As UnidivNet.UD_FLine In mdicFLines.Values

			If oFLine.Stage = iStage Then
				itest += 1
				oFLine.InsertBlock()
			End If

		Next
		DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest, iStage)

	End Sub
	Private Sub zzInsertFinalFlines()
		Const sFinalTopologyName As String = "FinalStage"
		Dim oPrevPoint As UnidivNet.UD_Point
		Dim oNextPoint As UnidivNet.UD_Point
		Dim oFLineNode As tsNode
		Dim oFLine As UnidivNet.UD_FLine
		Dim oFinalTopoScheme As tsTopology
		Dim colNodeLinks As System.Collections.ObjectModel.Collection(Of NodeLink)
		Dim dicFLines As UnidivNet.UD_FLines = New UnidivNet.UD_FLines()
		'  DMCommon.Debug.MsgBox("08_663", moFragmentsToposcheme, DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(moFragmentsToposcheme.Branches), DMCommon.Debug.ColCount(mcolCancelledinks))
		'  Dim itest As Integer

		If TopoManager.TopoCreator.TopologyExists(sFinalTopologyName) Then
			oFinalTopoScheme = New tsTopology(sFinalTopologyName)
			oFinalTopoScheme.Load(mbCheckExtended)
			For Each oNode As tsNode In oFinalTopoScheme.Nodes
				' DMAcadExt.AcadDocument.WriteDebugMessage("#144 " & oNode.BlockName.ToUpper)
				Select Case oNode.BlockName.ToUpper
					Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
						oNode.HasOldPoint = True
				End Select
			Next
			zzLoadStageNodes(oFinalTopoScheme)
			oFinalTopoScheme.RemovePseudoPoints()
			colNodeLinks = oFinalTopoScheme.GetNodeLinksCorrected
			DMCommon.Debug.MsgBox("09_766W", colNodeLinks.Count, DMCommon.Debug.ColCount(mdicPoints))
			For Each tNodeLink As NodeLink In colNodeLinks

				oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NodeID)

				If oFLineNode IsNot Nothing Then
					oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
					' DMCommon.Debug.MsgBox("09_779", oPrevPoint, oFLineNode.AcObjID)
					oFLineNode = oFinalTopoScheme.GetNode(tNodeLink.NextNodeID)
					If oFLineNode IsNot Nothing Then

						oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
						If oPrevPoint IsNot Nothing AndAlso oNextPoint IsNot Nothing Then

							oFLine = New UnidivNet.UD_FLine(oPrevPoint, oNextPoint, tNodeLink.Link)
							'      DMCommon.Debug.MsgBox("09_791a", oPrevPoint.Name, oPrevPoint.Stage, oNextPoint.Name, oNextPoint.Stage, oFLine.Stage)
							dicFLines.AddFLine(oFLine)
						End If
					End If
				End If
			Next
			DMCommon.Debug.MsgBox("09_760", dicFLines.Count, dicFLines.Values.Count)
			For Each oFLine In dicFLines.Values
				oFLine.InsertBlock()
			Next
		End If

		'  DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest)

	End Sub
	Private Sub zzInsertAllFLines()
		Dim sFrontLineLayer As String
		Dim bCurrentLayerOK As Boolean
		DMCommon.Debug.MsgBox("13_105", miCurrentStage)
		If miCurrentStage = 0 Then
			zzInsertFlines(miCurrentStage)
		Else
			zzCreateFinalTopo()
			For iStageNo As Integer = 0 To miCurrentStage
				sFrontLineLayer = UnidivNet.UD_App.GetStageFrontLineLayer(iStageNo)
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sFrontLineLayer, DMAcadExt.DMApp.AppID, True, False)
				If iStageNo = miCurrentStage Then
					UnidivNet.UD_App.SetStageLayerOn(miCurrentStage, True)
				Else
					UnidivNet.UD_App.SetStageLayerOn(iStageNo, True)
				End If

			Next

			zzInsertFinalFlines()
		End If
	End Sub
	Private Sub zzAllEnabled(bEnabled As Boolean)
		Me.rdbTransfer.Enabled = bEnabled
		Me.rdbUnion.Enabled = bEnabled
		Me.rdbDivide.Enabled = bEnabled
		Me.cmdContinue.Enabled = bEnabled

	End Sub
	Private Sub cmdStageView_Click(oSender As System.Object, e As EventArgs) Handles cmdStageView.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		UnidivNet.UD_App.SetStageLayerOn(0, True)
		DMAcadExt.AcadTransaction.SetLayersOnExcept(UnidivNet.UD_App.GetStageLayers(0), False)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub dgvMain_RowEnter(oSsender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.RowEnter
		If mbEventsEnabled Then
			If chkHanitView.Checked Then
				Dim oRow As UD_Row
				' If mdicRows.TryGetValue(e.RowIndex, oRow) Then
				If oRow.Stage <> miStageDisplayed Then
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
					DMAcadExt.AcadTransaction.Start()

					If miStageDisplayed <> -1 Then
						UnidivNet.UD_App.SetStageLayerOn(miStageDisplayed, False)
					End If
					miStageDisplayed = oRow.Stage
					UnidivNet.UD_App.SetStageLayerOn(miStageDisplayed, True)
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
					DMAcadExt.AcadDocument.UpdateScreen()
				End If
				'End If
			End If
		End If
	End Sub
	Private Sub zzActiveParcelsView(tLayerList As DMCommon.dmList, Optional iFocusedParcelDbID As Integer = 0)
		tLayerList.Add(DMAcadExt.AcadTransaction.FreezeLayer)
		'	DMCommon.Debug.MsgBox("12_287C", tLayerList.List)

		mcolUnvisibleLayers = DMAcadExt.AcadTransaction.SetLayersOnExcept(tLayerList, False)
		mcolParcelCancelled.Clear()
		mcolNotFocused.Clear()
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			If oParcel.IsCanceled Then
				' DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.BorderObjID)
				' DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.CentroidAcObjID)
				'  DMCommon.Debug.MsgBox("12_287", oParcel.UD_Name)
				If oParcel.BorderAcObjID.IsNull Then
					DMCommon.Debug.MsgBox("12_282B", oParcel.UD_Name)
				Else
					mcolParcelCancelled.Add(oParcel.BorderAcObjID)
				End If
				If oParcel.CentroidAcObjID.IsNull Then
					DMCommon.Debug.MsgBox("12_283C", oParcel.UD_Name)
				Else
					mcolParcelCancelled.Add(oParcel.CentroidAcObjID)
				End If
			End If
			If iFocusedParcelDbID <> 0 AndAlso oParcel.DbID <> iFocusedParcelDbID Then
				mcolNotFocused.Add(oParcel.BorderAcObjID)
				mcolNotFocused.Add(oParcel.CentroidAcObjID)
				DMAcadExt.AcadTransaction.SetAttributesByBlock(oParcel.CentroidAcObjID)
			End If
		Next
		'DMCommon.Debug.MsgBox("12_282ZZ", "OperView #3", mcolParcelCancelled.Count, mcolNotFocused.Count)
		DMAcadExt.AcadTransaction.SetVisible(mcolParcelCancelled, False)   ''''''''''''''''''''''''''''''040724
		DMAcadExt.AcadTransaction.SetColor(mcolNotFocused, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, 251S))  ''''''''''''''''''''''''''''''040724


		' DMCommon.Debug.MsgBox("12_280B", mcolUnvisibleLayers.Count, tLayerList.List, mcolParcelCancelled.Count, mcolNotFocused.Count)
	End Sub
	Private Sub zzRestoreView()

		DMAcadExt.AcadTransaction.SetLayersOffStatus(mcolUnvisibleLayers, False)
		mcolUnvisibleLayers = New ObjectModel.ObservableCollection(Of String)()
		DMAcadExt.AcadTransaction.SetVisible(mcolParcelCancelled, True)
		DMAcadExt.AcadTransaction.SetVisible(mcolNewLinksOutOfSourceParcel, True)
		DMAcadExt.AcadTransaction.SetColor(mcolNotFocused, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256S))
		'   DMCommon.Debug.MsgBox("12_280A", mcolUnvisibleLayers.Count, mcolParcelCancelled.Count, mcolNotFocused.Count)
	End Sub
	Private Sub cmdSelectPgon_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectPgon.Click
		If miCurrentActionFirstRowIndex <> -1 Then
			zzOpenDWG(True, True)
			zzActiveParcelsView(moUD_ParcelLayerList)
			'  zzCloseDWG()
			'   zzOpenDWG()
			Me.Visible = False
			'''''''''''''  AppActivate(moAppWin.Text)
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Divide
					If Me.chkAddToSelect.Checked Then
						zzSelectParcelsByPgon(False)

					Else
						Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
						Dim tSourceParcelKey As UD_ParcelKey
						Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing



						If zzSelectPoint(tPoint, True) = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, tSourceParcelKey) Then
							'  DMCommon.Debug.MsgBox("12_060", tPoint, tSourceParcelKey)
							If tSourceParcelKey.Exists AndAlso mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then
								'  DMCommon.Debug.MsgBox("09_991d_Div", oSourceParcel.AcadArea, oSourceParcel.LegalArea, oSourceParcel.BorderObjID)
								zzSetSourceParcel(oSourceParcel, False)
								'  DMAcadExt.AcadDocument.ClearDrawVectorSet()
								DMAcadExt.AcadDocument.ClearDrawVectorSet()
								zzMarkParcel(oSourceParcel, 0, False)
								'  DMCommon.Debug.MsgBox("09_912t", Me.dgvMain.AllowUserToAddRows)
							End If
						End If


					End If
					' Me.Visible = True
					'zzRestoreView()
					'  zzSelectFromStagesTopologies(False)
				Case UnidivNet.enActionType.Union
					zzSelectParcelsByPgon(True)
				Case UnidivNet.enActionType.Default
					'  Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = Nothing
					' zzActiveParcelsView(False)
					'   DMAcadExt.AcadTransaction.ReStart()

					Dim oSelectedEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(zzGetSelected(), 51)
					Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(zzGetActiveParcelCentroids(), 10)
					zzCloseDWG()
					zzOpenDWG()

					If zzSelectParcelColByFragment() Then

						'   zzParcelsToGrid(colParcels)
						DMAcadExt.AcadDocument.ClearDrawVectorSet()
						'	zzSelectedParcelKeysToGrid(True, False)
						zzSelectedParcelsToGrid(True, False)
						zzSetParcelCount(mhsSelectedParcels.Count)

						'''''''''''''''''''''''''''''''''''''''''''    zzBeforeUnion(colParcels)
					End If
					oSelectedEntitySet.RestoreColor()
					oEntitySet.RestoreColor()
				'	zzDispSelectedParcels("13_020")
					''''''''''''''''''''''''''''''''''''''''''''''''''' zzRestoreView()
				Case UnidivNet.enActionType.Transfer
					Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
					Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
					Dim bSingleOnly As Boolean = Not dgvMain.AllowUserToAddRows
					If bSingleOnly Then
						If zzSelectPoint(tPoint, True) = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, oSourceParcel) Then
							'DMCommon.Debug.MsgBox("12_055", tPoint, oSourceParcel, bSingleOnly)
							zzSetSourceParcel(oSourceParcel, False)
							'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''zzTransferFromFinalTopology(tSourceParcelKey)
						End If
					Else
						zzSelectParcelsByPgon(True)
					End If
					' zzActiveParcelsView(False)




			End Select
			Me.Visible = True
			zzRestoreView()
			zzCloseDWG()

		End If
		'	DMCommon.Debug.MsgBox("12_059c", mlstSelectedParcels.Count, mhsSelectedParcels.Count)
	End Sub
	Private Sub zzSelectParcelsByTopo(bUnion As Boolean)
		Const sSelectTopoName As String = "UnionBorder"
		Dim sPolylineLayer As String = "UD_StageCenter"
		Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim oaValues() As TypedValue = {New TypedValue(DxfCode.Start, "LWPOLYLINE,POLYLINE,LINE,ARC")}
		zzGetLinksBySelection(oaValues, colLinks)
		zzOpenDWG(False, False)
		DMCommon.Debug.MsgBox("13_188c", colLinks.Count)
		If colLinks.Count > 0 Then
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
			DMCommon.Debug.MsgBox("13_188d", colLinks.Count, bCurrentLayerOK, sSelectTopoName, sPolylineLayer)
			If bCurrentLayerOK Then

				Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
				Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
				Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
				If zzCreateTopology(sSelectTopoName, True, colLinks) Then
					Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
					Dim oPgonScheme As tsPolygon

					Dim oTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sSelectTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
					Dim oTopoScheme As tsTopology = New tsTopology()
					DMCommon.Debug.MsgBox("13_188e", colLinks.Count, bCurrentLayerOK, sSelectTopoName, oTopology IsNot Nothing)
					If oTopology IsNot Nothing Then
						oTopoScheme.Load(False, oTopology)
						oTopoScheme.CheckIslands()

						For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
							If Not oParcel.IsCanceled Then
								Try
									oPolygon = oTopology.FindPolygon(oParcel.CentroidPoint3d)
									If oPolygon IsNot Nothing Then
										oPgonScheme = oTopoScheme.GetPolygon(oPolygon.ID)
										If Not oPgonScheme.IsInner Then

											'If oPgonScheme.CheckIslands Then
											zzMarkParcel(oParcel, -1, False)

											If Not mhsSelectedParcels.Contains(oParcel) Then
												mhsSelectedParcels.Add(oParcel)
												mlstSelectedParcelKeys.Add(oParcel.ParcelKey)
												mlstSelectedParcels.Add(oParcel)
												'colParcels.Add(oParcel)
											End If
										End If

									End If
								Catch oEx As Exception

								End Try

							End If
						Next

						zzSelectedParcelKeysToGrid(True, False)


						Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopology.GetPolygons()

						For Each oPolygon In colParcelPgons
							colCentroids.Add(oPolygon.Entity)
						Next
						oTopology.Close()
						DMCommon.Debug.MsgBox("13_188K", colCentroids.Count, sSelectTopoName)
						zzDeleteTopology(sSelectTopoName)
						DMCommon.Debug.MsgBox("13_188L", sSelectTopoName)
						DMAcadExt.AcadTransaction.EraseDBObjects(colCentroids)
						DMCommon.Debug.MsgBox("13_188M")
					End If
				End If



			End If
		End If
		zzCloseDWG()
	End Sub
	Private Sub zzSelectParcelsByPgon(bUnion As Boolean)
		Dim oSelectedEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(zzGetSelected(), 51)
		Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(zzGetActiveParcelCentroids(), 10)
		Dim iRowIndex As Integer

		zzCloseDWG()
		zzOpenDWG()

		If zzSelectParcelColByFragment() Then
			DMAcadExt.AcadDocument.ClearDrawVectorSet()
			If bUnion Then
				zzSelectedParcelsToGrid(True, False)
			Else
				zzFillJournalRows()
				For Each tParcelKey As UD_ParcelKey In mlstSelectedParcelKeys
					If mdicJournalRows.TryGetValue(tParcelKey, iRowIndex) Then
						Me.dgvMain.Rows.Item(iRowIndex).Selected = True
					End If
				Next
			End If

			zzSetParcelCount(mhsSelectedParcels.Count)

			'''''''''''''''''''''''''''''''''''''''''''    zzBeforeUnion(colParcels)
		End If
		oSelectedEntitySet.RestoreColor()
		oEntitySet.RestoreColor()
		'	zzDispSelectedParcels("13_020")
	End Sub
	Private Sub zzSelectParcelsByCentroid(bUnion As Boolean)
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim colSelectedParcels As ObjectIdCollection = zzGetSelected()

		Dim oSelectedEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colSelectedParcels, 51)

		Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(False, "C1603_*", zzGetActiveParcelCentroids(), 10)

		'	Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		Dim iRowIndex As Integer

		If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
			' DMCommon.Debug.MsgBox("12_122", colCentroidIds.Count)
			For Each tCentroidID As ObjectId In colCentroidIds
				If mdicParcels.TryGetValueByObjID(tCentroidID, oParcel) Then
					zzMarkParcel(oParcel, -1, False)

					If Not mhsSelectedParcels.Contains(oParcel) Then
						mhsSelectedParcels.Add(oParcel)
						mlstSelectedParcelKeys.Add(oParcel.ParcelKey)

						mlstSelectedParcels.Add(oParcel)

						'colParcels.Add(oParcel)
					End If
				End If
			Next

			If bUnion Then
				'zzSelectedParcelKeysToGrid(True, False)
				zzSelectedParcelsToGrid(True, False)


			Else
				zzFillJournalRows()
				For Each tParcelKey As UD_ParcelKey In mlstSelectedParcelKeys
					If mdicJournalRows.TryGetValue(tParcelKey, iRowIndex) Then
						Me.dgvMain.Rows.Item(iRowIndex).Selected = True
					End If
				Next
			End If
			'  zzParcelsToGrid(colParcels)

			zzSetParcelCount(mhsSelectedParcels.Count)
		End If
		oSelectedEntitySet.RestoreColor()
	End Sub
	Private Sub zzInitLayersAAA()
		Dim saLayer(5) As String
		Dim iStageNo As Integer = 0
		Dim oGridRow As System.Windows.Forms.DataGridViewRow = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		If DMAcadExt.AcadDocument.IsLocked Then
			DMAcadExt.AcadTransaction.Start()
			mhsHanitLayers.Clear()
			Do
				zzDefLayers(iStageNo, saLayer)

				If DMAcadExt.AcadTransaction.LayersExist(saLayer, False) Then
					oGridRow.Cells.Item("ctxStageNo").Value = iStageNo
					oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Checked
					For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
						mhsHanitLayers.Add(saLayer(iIndex))
					Next

				Else
					Exit Do
				End If

				iStageNo += 1
			Loop While iStageNo < 20

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If

	End Sub
	Private Sub zzDefLayers(iStageNo As Integer, ByRef saLayer() As String)
		saLayer(0) = UnidivNet.UD_App.GetStagePolineLayer(iStageNo)
		saLayer(1) = UnidivNet.UD_App.GetStageParcelCentroidLayer(iStageNo)
		saLayer(2) = UnidivNet.UD_App.GetStageFrontLineLayer(iStageNo)
		saLayer(3) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, False)
		saLayer(4) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, True)
		saLayer(5) = UnidivNet.UD_App.GetStageCentroidLayer(iStageNo)

		For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
			mhsHanitLayers.Add(saLayer(iIndex))
		Next

	End Sub

	'  zzSelectParcelByFragment
	Private Sub zzTransferParcel(tParcelKey As UD_ParcelKey)
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
			oParcel.GetBoundary()
		End If
	End Sub
	Private Sub zzTransferFromFinalTopology(tSourceParcelKey As UD_ParcelKey, oActionRow As DataRow, oGridRow As DataGridViewRow, iRowIndex As Integer)
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oParcel As UnidivNet.UD_Parcel = Nothing


		'  Dim oPolygon As Polygon = Nothing
		Dim oFinalTopoScheme As tsTopology
		Dim oPgon As tsPolygon = Nothing

		Dim iPgonID As Integer
		Dim colPolylineObjID As ObjectIdCollection = New ObjectIdCollection()
		'  DMCommon.Debug.MsgBox("12_040", tSourceParcelKey)
		'    Dim oFinalTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(UnidivNet.UD_App.FinalTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If Not tSourceParcelKey.Exists Then
			tPoint = oParcel.CenterPosition.AcGePoint3d
			DMCommon.Debug.MsgBox("11_801s", tPoint)
			zzSelectParcelFromFinalTopo(tPoint, iPgonID, tSourceParcelKey)

		End If


		If tSourceParcelKey.Exists AndAlso mdicParcels.TryGetValue(tSourceParcelKey, oParcel) Then
			' DMCommon.Debug.MsgBox("09_991d_Tr", tSourceParcelKey, oParcel.AcadArea, oParcel.LegalArea, oParcel.ParcelArea.LegalArea, oParcel.BorderAcObjID)
			oParcel.IsCanceled = True
			oParcel.IsMoved = True
			' DMCommon.Debug.MsgBox("09_991f_Tr!!", tSourceParcelKey, oParcel.ParcelKey, oParcel.IsCanceled, oParcel.IsMoved, oParcel.BorderAcObjID)
			oParcel.UpdateBlockAttributes()
			'	Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
			'	Dim oFirstActionDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			zzFillFromParcelKey(oParcel, oGridRow)
			zzFillParcelArea(oParcel.ParcelArea, oActionRow, oGridRow)
			oActionRow.Item("AcObjID") = oParcel.CentroidAcObjID
			oActionRow.Item("RowStatus") = enRowStatus.Transfer
			zzAddInputCell(oGridRow, iRowIndex, miToParcelColIndex, True)
			zzAddInputCell(oGridRow, iRowIndex, miToGushColIndex, True)
			zzAddInputCell(oGridRow, iRowIndex, miToGushAddColIndex, True)





		End If
		'   DMCommon.Debug.MsgBox("11_350", msFinalTopologyName)
		oFinalTopoScheme = New tsTopology(UnidivNet.UD_App.FinalTopoName)

		oFinalTopoScheme.Load(mbCheckExtended)
		'   DMCommon.Debug.MsgBox("11_351", DMCommon.Debug.ColCount(oFinalTopoScheme.Polygons))
		oFinalTopoScheme.RemovePseudoPoints()
		'   DMCommon.Debug.MsgBox("11_353")

		'''''''''''''''''''''''''''''''''''''''''''  oPgon = oFinalTopoScheme.GetPolygon(iPgonID)

		Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(miCurrentStage)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

		'   DMCommon.Debug.MsgBox("12_055", bCurrentLayerOK, sPolylineLayer, oPgon)
		If oPgon IsNot Nothing Then
			' DMAcadExt.AcadDocument.WriteMessage("OK Before: Poligon #" & CStr(iPgonID))
			''''''''''''''''''''''   oPgon.CreateDBPolylineCorrected(True)
		Else
			'''''''''''''''''''''''''' DMAcadExt.AcadDocument.WriteMessage("Error 543: Poligon #" & CStr(iPgonID) & " was not found")
		End If
		If oParcel IsNot Nothing AndAlso Not oParcel.BorderAcObjID.IsNull Then
			Dim oPLine As Polyline = DMAcadExt.AcadTransaction.GetPolyline(oParcel.BorderAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Dim oNewPline As Polyline = New Polyline()

			oNewPline.CopyFrom(oPLine)
			oNewPline.Layer = sPolylineLayer
			'     DMCommon.Debug.MsgBox("12_080", sPolylineLayer)
			DMAcadExt.AcadTransaction.AppendEntity(oNewPline)
		End If
	End Sub



	Private Function zzSelectParcelByFragment(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef oParcel As UnidivNet.UD_Parcel) As Boolean
		'	DMCommon.Debug.MsgBox("12_050", tPoint)
		'  Dim bFragmentTopologyClose As Boolean
		Dim bRes As Boolean = False
		'  If moFragmentTopology Is Nothing OrElse moFragmentTopology.Status = Status.Closed Then
		'moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		' bFragmentTopologyClose = True
		' End If

		If moFragmentTopology IsNot Nothing AndAlso moFragmentTopology.Status <> Status.Closed Then
			Dim oPolygon As Polygon
			Dim iPolygonID As Integer
			Dim tParcelKey As UD_ParcelKey
			'  Dim oFragment As Fragment

			Try
				oPolygon = moFragmentTopology.FindPolygon(tPoint)
				iPolygonID = oPolygon.ID
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("12_052 Err", tPoint, moFragmentTopology.GetPolygons().Count)
			End Try

			If iPolygonID <> 0 AndAlso mdicFragments.TryGetParcel(iPolygonID, tParcelKey) Then
				If tParcelKey.Exists AndAlso mdicParcels.TryGetValue(tParcelKey, oParcel) Then
					'DMCommon.Debug.MsgBox("12_053", iPolygonID, tParcelKey)
					bRes = True
				End If


			Else
				DMCommon.Debug.MsgBox("12_058 Err", iPolygonID)
			End If
		Else
			DMCommon.Debug.MsgBox("12_057", "Topology 'Fragments' was not found")
		End If
		'   If bFragmentTopologyClose Then
		'moFragmentTopology.Close()
		'moFragmentTopology = Nothing
		'End If
		Return bRes
	End Function

	Private Function zzSelectParcelByFragment(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef tParcelKey As UD_ParcelKey) As Boolean
		Dim bRes As Boolean = False

		If moFragmentTopology IsNot Nothing AndAlso moFragmentTopology.Status <> Status.Closed Then
			Dim oPolygon As Polygon
			Dim iPolygonID As Integer

			Try
				oPolygon = moFragmentTopology.FindPolygon(tPoint)
				iPolygonID = oPolygon.ID
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("12_052 Err", tPoint, moFragmentTopology.GetPolygons().Count)
			End Try
			''''''''''''''''''''''mdicFragments.Test_dicByTopoID()
			If iPolygonID <> 0 AndAlso mdicFragments.TryGetParcel(iPolygonID, tParcelKey) Then
				bRes = True
			Else
				DMCommon.Debug.MsgBox("12_058 Err", iPolygonID)
			End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!FrRes", iPolygonID, tParcelKey.ParcelNo)
		Else
			DMCommon.Debug.MsgBox("12_057", "Topology 'Fragments' was not found")
		End If
		Return bRes
	End Function
	Private Function zzSelectParcelFromFinalTopo(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef iPolygonID As Integer, ByRef tParcelKey As UD_ParcelKey) As Boolean
		Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oPolygon As Polygon = Nothing
		Dim sParcelName As String
		Dim oFinalTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(UnidivNet.UD_App.FinalTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oFinalTopology IsNot Nothing Then
			If Not tPoint.IsEqualTo(Autodesk.AutoCAD.Geometry.Point3d.Origin) Then
				' DMCommon.Debug.MsgBox("11_220c", "in zzSelectParcelFromFinalTopo")
				Try
					oPolygon = oFinalTopology.FindPolygon(tPoint)
					iPolygonID = oPolygon.ID
				Catch oEx As Exception

				End Try
				' DMCommon.Debug.MsgBox("11_220D", "iPolygonID=" & iPolygonID)
			Else
				' DMCommon.Debug.MsgBox("11_221P", "in FF zzSelectParcelFromFinalTopo")
				Me.Visible = False
				AppActivate(moAppWin.Text)
				Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

				iSelectStatus = zzSelectPoint(tPoint, True)
				If iSelectStatus = PromptStatus.OK AndAlso oFinalTopology IsNot Nothing Then
					Try
						oPolygon = oFinalTopology.FindPolygon(tPoint)
						iPolygonID = oPolygon.ID
					Catch oEx As Exception

					End Try
				End If
				Me.Visible = True
			End If
			If oPolygon IsNot Nothing Then
				Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
				Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()

				oAcadBlock.Fields = {"PARCEL_NAME"}
				oAcadBlock.OpenForRead()

				tBlockRefData = oAcadBlock.GetBlockRefData(oPolygon.Entity)

				sParcelName = tBlockRefData.AttribValues(0)

				tParcelKey = New UD_ParcelKey(miBlockNo, miBlockAddNo, sParcelName)
				DMCommon.Debug.MsgBox("11_221V", "tParcelKey=" & tParcelKey.ToString())
				Return True
			End If
		End If
		Return False
	End Function


	Private Sub cmdSelectLinks_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectLinks.Click
		If miCurrentActionFirstRowIndex <> -1 AndAlso miCurrentActionType = UnidivNet.enActionType.Divide Then
			zzBeforeDivide(False, enSelectLinkType.SelectLinks)
		ElseIf Me.chkRestoreCancelLink.Checked Then
			'	zzOpenDWG(True, False)
			'	Dim colAllCancelledLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(msLinkCancelledLayer)

			'	Dim colSelectedLinks As ObjectIdCollection = zzGetLinkSet(colAllCancelledLinks, enSelectLinkType.SelectLinks)

			zzRestoreCancelLinks(enSelectLinkType.SelectLinksByFragments)
			Me.chkRestoreCancelLink.Checked = False
			'zzCloseDWG()
		End If
	End Sub
	Private Sub cmdContinue_Click(oSender As System.Object, e As EventArgs) Handles cmdContinue.Click

		If miCurrentActionFirstRowIndex = -1 Then
			If miCurrentActionType <> UnidivNet.enActionType.Registered Then
				zzOpenAction(False, UnidivNet.enActionType.Default)
			End If
		Else
			zzExec()
		End If
		'	DMCommon.Debug.MsgBox("13_022c")
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("13_022c")
	End Sub


	Private Function zzSelectParcelColByFragment() As Boolean
		Dim bOneOnly As Boolean = False
		Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sParcelName As String = Nothing

		Do
			iSelectStatus = zzSelectPoint(tPoint, bOneOnly)
			If iSelectStatus = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, tParcelKey) Then

				DMCommon.Debug.MsgBox("11_240", tParcelKey.BlockNo, tParcelKey.ParcelNo, tParcelKey.Original, tParcelKey.CompareKey)
				If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso oParcel.IsResult Then

					If Not mhsSelectedParcels.Contains(oParcel) Then
						'	colParcels.Add(oParcel)
						mhsSelectedParcels.Add(oParcel)
						mlstSelectedParcelKeys.Add(tParcelKey)
						mlstSelectedParcels.Add(oParcel)

						'+ToGrid
						DMAcadExt.AcadDocument.WriteMessage(" גוש " & tParcelKey.BlockNo & "; חלקה " & tParcelKey.UD_ParcelName & vbCrLf)

						DMAcadExt.AcadDocument.WriteMessage("Selected: " & "; " & "Total: " & mhsSelectedParcels.Count.ToString() & vbCrLf)
						'  DMAcadExt.AcadDocument.CloseMessage()


						zzMarkParcel(oParcel, -1, False)


					End If

				End If


			Else
				Exit Do
			End If
		Loop

		DMCommon.Debug.MsgBox("13_301c", mlstSelectedParcelKeys.Count, mhsSelectedParcels.Count)
		'  Me.Visible = True
		'  moStageTopologies.CloseTopos()
		Return mlstSelectedParcelKeys.Count > 0
	End Function

	Private Function zzGetParcelColFromGrid(ByRef colParcels As ICollection(Of UnidivNet.UD_Parcel)) As Boolean
		Dim bOneOnly As Boolean = False

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sParcelName As String = Nothing
		Dim oGridRow As System.Windows.Forms.DataGridViewRow
		Dim oDataRow As DataRow
		Dim oParcelKeyComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
		Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelComparer = New UnidivNet.UD_Parcels.ParcelComparer()

		' Dim oDataViewRow As DataView

		Dim oParcelKeyList As List(Of UD_ParcelKey) = New List(Of UD_ParcelKey)()
		Dim oParcelList As List(Of UnidivNet.UD_Parcel) = New List(Of UnidivNet.UD_Parcel)()

		colParcels = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		' DMCommon.Debug.MsgBox("11_249", dgvMain.SelectedRows().Count)
		' For Each oGridRow As System.Windows.Forms.DataGridViewRow In dgvMain.SelectedRows()
		For iIndex As Integer = 0 To dgvMain.SelectedRows().Count - 1
			oGridRow = dgvMain.SelectedRows().Item(iIndex)
			oDataRow = moMainTable.Rows.Item(oGridRow.Index)

			oParcel = zzGetParcelByDbID(oDataRow, False)
			tParcelKey = zzGetSourceParcelKey(oGridRow)
			'DMCommon.Debug.MsgBox("12_826y", tParcelKey, oParcel.ParcelKey, oParcel.DbID, oGridRow.Index)

			oParcelList.Add(oParcel)


		Next
		oParcelList.Sort(oParcelComparer)
		For Each oParcel In oParcelList
			colParcels.Add(oParcel)
		Next

		Return colParcels.Count > 1
	End Function
	Private Function zzGetParcelColFromGrid(ByRef lstSelectedParcels As List(Of UnidivNet.UD_Parcel)) As Boolean
		Dim bOneOnly As Boolean = False

		Dim tParcelKey As UD_ParcelKey
		Dim iParcelDbID As Integer

		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sParcelName As String = Nothing
		Dim oGridRow As System.Windows.Forms.DataGridViewRow
		' Dim oDataViewRow As DataView
		'    DMCommon.Debug.MsgBox("12_827a", "zzGetParcelColFromGrid")

		lstSelectedParcels = New List(Of UnidivNet.UD_Parcel)()

		' DMCommon.Debug.MsgBox("11_249", dgvMain.SelectedRows().Count)
		' For Each oGridRow As System.Windows.Forms.DataGridViewRow In dgvMain.SelectedRows()
		For iIndex As Integer = 0 To dgvMain.SelectedRows().Count - 1
			oGridRow = dgvMain.SelectedRows().Item(iIndex)

			tParcelKey = zzGetSourceParcelKey(oGridRow)
			iParcelDbID = zzGetSourceParcelDbID(oGridRow)

			If mdicParcels.TryGetValueByDbID(iParcelDbID, oParcel) AndAlso oParcel.IsResult Then

				lstSelectedParcels.Add(oParcel)
			Else
				DMCommon.Debug.MsgBox("12_826U", tParcelKey, iParcelDbID, oGridRow.Index, mdicParcels.Count, lstSelectedParcels.Count, oParcel Is Nothing)
			End If
			'	DMCommon.Debug.MsgBox("12_826w", tParcelKey, iParcelDbID, oGridRow.Index, lstSelectedParcels.Count)
		Next

		Return lstSelectedParcels.Count > 1
	End Function
	Private Function zzGetParcelColFromGrid(ByRef lstSelectedParcels As List(Of UD_ParcelKey)) As Boolean
		Dim bOneOnly As Boolean = False

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim sParcelName As String = Nothing
		Dim oGridRow As System.Windows.Forms.DataGridViewRow
		' Dim oDataViewRow As DataView
		'    DMCommon.Debug.MsgBox("12_827a", "zzGetParcelColFromGrid")

		lstSelectedParcels = New List(Of UD_ParcelKey)()

		' DMCommon.Debug.MsgBox("11_249", dgvMain.SelectedRows().Count)
		' For Each oGridRow As System.Windows.Forms.DataGridViewRow In dgvMain.SelectedRows()
		For iIndex As Integer = 0 To dgvMain.SelectedRows().Count - 1
			oGridRow = dgvMain.SelectedRows().Item(iIndex)

			tParcelKey = zzGetSourceParcelKey(oGridRow)
			'	DMCommon.Debug.MsgBox("12_826w", tParcelKey, oGridRow.Index)
			If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso oParcel.IsResult Then

				lstSelectedParcels.Add(tParcelKey)
			End If
		Next

		Return lstSelectedParcels.Count > 1
	End Function


	Private Sub chkHanitView_CheckedChanged_1(oSender As System.Object, e As EventArgs)
		Dim bIsOn As Boolean = Me.chkHanitView.Checked
		If Me.chkHanitView.Checked Then
			'	Dim oRow As UD_Row
			' If mdicRows.TryGetValue(Me.dgvMain.CurrentRow.Index, oRow) Then
			'  miStageDisplayed = oRow.Stage
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			If False Then
				For iIndex As Integer = 0 To miCurrentStage
					If iIndex = miStageDisplayed Then
						UnidivNet.UD_App.SetStageLayerOn(miStageDisplayed, True)
					Else
						UnidivNet.UD_App.SetStageLayerOn(miStageDisplayed, False)
					End If
				Next
			End If
			Dim saLayer(miCurrentStage) As String
			For iIndex As Integer = 0 To miCurrentStage
				saLayer(iIndex) = UnidivNet.UD_App.GetStagePolineLayer(iIndex)

			Next
			DMAcadExt.AcadTransaction.SetLayersOn(saLayer, bIsOn)
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()

			' End If
		End If

	End Sub
	Private Sub cmdLoadDBData_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadDBData.Click

		Me.Cursor = Cursors.WaitCursor
		zzOpenJournalTable()

		If mdicFragments.DBVersionIsCorrect Then
			zzAddDBJournalData(enAddDataType.AllData)
			zzOpenDWG(True, True)
			zzInterpretJournal()
			zzCloseDWG()
		Else
			DMCommon.Debug.MsgBox("09_655x", "DBVersionIsCorrect", moMainTable.Rows.Count, moMainTable.Columns.Count, mdicParcels.Count)
		End If
		Me.Cursor = Cursors.Default

		'''''''''''  zzLoadDWG()
		'''''''''''''' zzFillGrid()
		'	End If
	End Sub




	Private Sub cmdSaveDB_Click(oSsender As System.Object, e As EventArgs) Handles cmdSaveDB.Click
		'  

		'	DMCommon.Debug.MsgBox("09_650x", moMainTable.Rows.Count, moMainTable.Columns.Count, miCurrentActionFirstRowIndex)
		'  DMCommon.ExcelLogG.SetDataTable(moMainTable, 0)
		'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
		'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
		'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
		'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
		'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
		'DMCommon.ExcelLogG.SetDataTable(moMainTable, 5)
		If miCurrentActionFirstRowIndex = -1 Then
			Me.Cursor = Cursors.WaitCursor
			'	DMCommon.Debug.ExcelLog.SetDataTable(0, "MainT!!", moMainTable)

			'    DMCommon.Debug.MsgBox("Wait", moMainTable.Rows.Count)
			'  DMCommon.ExcelLogY.OpenA()





			Try
				If Not mbUpdateOpened Then
					zzClearJournalData()
					zzOpenJournalDataAdapter()
					mbUpdateOpened = True
				End If

				moJournalDataAdapter.Update(moMainTable)



			Catch oEx As Exception
				DMCommon.Debug.UserMsg("Update Journal Data", oEx.Message, "Records=" & moMainTable.Rows.Count)
			End Try

			If True Then
				'   moParcelLogDataAdapter.Update(moParcelLogTable)
				'   moFragmentsDataAdapter.Update(moFragmentTable)
				zzClearPlanData()
				zzUpdateFragmentTable()
				zzUpdateParcelFragmentTable()
				DMCommon.Debug.UserMsg(" Data", "Journal Records = " & moMainTable.Rows.Count, "Fragment Records = " & moFragmentTable.Rows.Count, "ParcelFragment Records = " & moParcelFragmentsTable.Rows.Count)
				zzUpdateProjectPlanTable()
				zzUpdatePointsTable()


			End If
			Me.Cursor = Cursors.Default
			mbDirty = False
		Else
			DMCommon.Debug.MsgBox("Save DB", "Action is opened", miCurrentActionFirstRowIndex)
		End If

	End Sub
	Private Sub zzClearJournalData()

		Const sSPName As String = "ClearJournalData"
		Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())

	End Sub
	Private Sub zzClearPlanData()

		'	Const sSPName As String = "ClearBlockData"
		Const sSPName As String = "ClearPlanData"


		Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())
		'   DMCommon.Debug.MsgBox("zzClearGushData", iRes)
	End Sub

	Private Sub zzUpdateFragmentTable()
		Dim oNewRow As DataRow
		Dim sMessage As String = Nothing

		Dim sStackTrace As String = Nothing
		' Dim oFragmentA As Fragment
		zzLoadFragmentsTable(True)
		For Each oFragment As Fragment In mdicFragments.Values
			Try

				oNewRow = moFragmentTable.NewRow()
				oNewRow.Item("ProjectCode") = miProjectCode
				oNewRow.Item("Detail") = miDetailNo
				oNewRow.Item("PlanID") = miPlanID
				oNewRow.Item("FragmentID") = oFragment.DWGTopoID
				oNewRow.Item("CentroidHandle") = oFragment.HandleVal
				oNewRow.Item("ParcelNo") = oFragment.SourceParcelKey.ParcelNo
				oNewRow.Item("ParcelIsOriginal") = True
				oNewRow.Item("CentroidX") = oFragment.CentroidX
				oNewRow.Item("CentroidY") = oFragment.CentroidY
				moFragmentTable.Rows.Add(oNewRow)

			Catch oEx As Exception
				If String.IsNullOrEmpty(sMessage) Then
					sMessage = oEx.Message
					sStackTrace = oEx.StackTrace
				End If


			End Try


		Next
		If Not String.IsNullOrEmpty(sMessage) Then
			DMCommon.Debug.UserMsg("Fill Fragment table", sMessage, sStackTrace, "Fragments count =" & mdicFragments.Count, "Records=" & moFragmentTable.Rows.Count)
		End If

		Try
			moFragmentsDataAdapter.Update(moFragmentTable)
		Catch oEx As Exception
			DMCommon.Debug.UserMsg("Update Fragment table", oEx.Message, "Fragments count =" & mdicFragments.Count, "Records=" & moFragmentTable.Rows.Count)
		End Try

	End Sub
	Private Sub zzUpdateParcelFragmentTable()
		Dim sMessage As String = Nothing
		Dim sStackTrace As String = Nothing
		Dim tParcelKey As UD_ParcelKey
		Dim oNewRow As DataRow

		' Dim oFragmentA As Fragment
		zzLoadParcelFragmentsTable(True)
		'    DMCommon.Debug.MsgBox("08_577x", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			For Each iFragmentID As Integer In oParcel.Fragments
				Try
					oNewRow = moParcelFragmentsTable.NewRow()
					oNewRow.Item("ProjectCode") = miProjectCode
					oNewRow.Item("Detail") = miDetailNo
					oNewRow.Item("PlanID") = miPlanID
					oNewRow.Item("ParcelDbID") = oParcel.DbID
					oNewRow.Item("ParcelNo") = oParcel.ParcelKey.ParcelNo
					oNewRow.Item("ParcelIsOriginal") = oParcel.ParcelKey.Original
					oNewRow.Item("ParcelIsBase") = (oParcel.Stage = 0)
					oNewRow.Item("FragmentID") = iFragmentID

					moParcelFragmentsTable.Rows.Add(oNewRow)

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oParcel.ParcelKey.ToString() & vbCrLf & iFragmentID.ToString(), "UpdateParcelFragmentTable")

					If String.IsNullOrEmpty(sMessage) Then
						sMessage = oEx.Message
						sStackTrace = oEx.StackTrace
						tParcelKey = oParcel.ParcelKey
					End If

				End Try
			Next

		Next


		If Not String.IsNullOrEmpty(sMessage) Then
			DMCommon.Debug.UserMsg("Fill ParcelFragment table", sMessage, sStackTrace, "Parcels count =" & mdicParcels.Count, "P_arcel: " & tParcelKey.ToString(), "Records=" & moParcelFragmentsTable.Rows.Count)
		End If



		Try
			moParcelFragmentsDataAdapter.Update(moParcelFragmentsTable)
		Catch oEx As Exception
			DMCommon.Debug.UserMsg("Update ParcelFragment table", oEx.Message, "Parcels count =" & mdicParcels.Count, "Records=" & moParcelFragmentsTable.Rows.Count)
		End Try


	End Sub
	Private Sub zzUpdateParcelFragmentTableByB()

		Dim oNewRow As DataRow


		zzLoadParcelFragmentsTable(True)

		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			For Each iFragmentID As Integer In oParcel.Fragments
				Try

					oNewRow = moParcelFragmentsTable.NewRow()


					oNewRow.Item("ProjectCode") = miProjectCode
					oNewRow.Item("Detail") = miDetailNo
					oNewRow.Item("OriginalBlockNo") = miBlockNo
					oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

					oNewRow.Item("ParcelDbID") = oParcel.DbID
					oNewRow.Item("ParcelNo") = oParcel.ParcelKey.ParcelNo
					oNewRow.Item("ParcelIsOriginal") = oParcel.ParcelKey.Original




					oNewRow.Item("FragmentID") = iFragmentID


					moParcelFragmentsTable.Rows.Add(oNewRow)

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oParcel.ParcelKey.ToString() & vbCrLf & iFragmentID.ToString(), "UpdateParcelFragmentTable")
				End Try
			Next

			'  DMCommon.Debug.MsgBox("bef Update", moParcelFragmentTable.Rows.Count)

			'  DMCommon.Debug.MsgBox("aft moFragmentTable.Rows", moFragmentTable.Rows.Count)

		Next
		moParcelFragmentsDataAdapter.Update(moParcelFragmentsTable)
	End Sub
	Private Sub zzUpdateProjectPlanTableByB()
		Dim oNewRow As DataRow
		Dim sOriginalParcelList As String = Nothing
		Dim sNewParcelList As String = Nothing
		Dim iIncomingParcelsCount As Integer
		Dim iOutgoingParcelsCount As Integer
		' Dim oFragmentA As Fragment
		zzLoadProjectPlanTable(True)

		mdicParcels.GetInitParcelString(UnidivNet.UD_App.IsTTG, sOriginalParcelList, sNewParcelList, iIncomingParcelsCount, iOutgoingParcelsCount)
		'    DMCommon.Debug.MsgBox("08_577x", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)





		oNewRow = moProjectPlanTable.NewRow()


		oNewRow.Item("ProjectCode") = miProjectCode
		oNewRow.Item("Detail") = miDetailNo
		oNewRow.Item("OriginalBlockNo") = miBlockNo
		oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo
		If sOriginalParcelList IsNot Nothing Then
			oNewRow.Item("OriginalParcelList") = sOriginalParcelList
		End If
		If sNewParcelList IsNot Nothing Then
			oNewRow.Item("NewParcelList") = sNewParcelList
		End If





		moProjectPlanDataAdapter.Update(moProjectPlanTable)

	End Sub

	Private Sub zzUpdateProjectPlanTable()
		Dim oRow As DataRow = Nothing
		Dim sOriginalParcelList As String = Nothing
		Dim sNewParcelList As String = Nothing
		Dim iIncomingParcelsCount As Integer
		Dim iOutgoingParcelsCount As Integer

		zzLoadProjectPlanTable(False)

		mdicParcels.GetInitParcelString(miPlanType = 12, sOriginalParcelList, sNewParcelList, iIncomingParcelsCount, iOutgoingParcelsCount)

		If moProjectPlanTable.Rows.Count = 1 Then
			oRow = moProjectPlanTable.Rows.Item(0)
		ElseIf moProjectPlanTable.Rows.Count = 0 Then
			oRow = moProjectPlanTable.NewRow()
		End If




		If oRow IsNot Nothing Then
			oRow.Item("ProjectCode") = miProjectCode
			oRow.Item("Detail") = miDetailNo
			oRow.Item("PlanID") = miPlanID
			If IsDBNull(oRow.Item("EmplID")) Then
				oRow.Item("EmplID") = TPlServerDB.ServerDB.CurrentProjectDB.EmployeeID
			Else
				oRow.Item("EmplID1") = TPlServerDB.ServerDB.CurrentProjectDB.EmployeeID
			End If
			If sOriginalParcelList IsNot Nothing Then
					oRow.Item("OriginalParcelList") = sOriginalParcelList
				End If
				If sNewParcelList IsNot Nothing Then
					oRow.Item("NewParcelList") = sNewParcelList
				End If

				oRow.Item("IncomingParcelsCount") = iIncomingParcelsCount
				oRow.Item("OutgoingParcelsCount") = iOutgoingParcelsCount

				oRow.Item("OriginalBlockNo") = miBlockNo
				oRow.Item("OriginalBlockAddNo") = miBlockAddNo

				Try
					oRow.Item("UserName") = System.Environment.UserName
					oRow.Item("CreateDate") = Date.Now
				Catch ex As Exception

				End Try
				If oRow.RowState = DataRowState.Detached Then
					moProjectPlanTable.Rows.Add(oRow)
				End If
				moProjectPlanDataAdapter.Update(moProjectPlanTable)
			End If

	End Sub

	Private Sub zzOpenDB(bSchemaOnly As Boolean)
		zzLoadFragmentsTable(bSchemaOnly)
		zzLoadParcelFragmentsTable(bSchemaOnly)
	End Sub

	Private Structure PlanExt
		Dim Plan As String
		Dim LanduseName As String
		Dim LanduseNameDos As String

		Dim LotName As String

		Public Sub New(oDataRow As DataRow)
			LanduseName = DMCommon.Functions.CStrN(oDataRow.Item("LanduseName"))
			LotName = DMCommon.Functions.CStrN(oDataRow.Item("LotName"))
		End Sub
	End Structure

	Private Class JournalTable
		Const mdHeaderHeight As Double = 10.0
		Const mdRowHeight As Double = 5.0
		Const mdTableWidth As Double = 70.0
		Const mdAfterTableDistance As Double = 20.0

		Private mtBaseBoint As Autodesk.AutoCAD.Geometry.Point3d
		Private mtCurrentPoint As Autodesk.AutoCAD.Geometry.Point3d

		Private miActionType As UnidivNet.enActionType
		Private miTableNumber As Integer
		Private miRowNumber As Integer = 1

		Private moHeaderAcadBlock As DMAcadExt.AcadBlock
		Private moRowAcadBlock As DMAcadExt.AcadBlock
		Private msLayer As String
		Private mtHeaderStep As Autodesk.AutoCAD.Geometry.Vector3d
		Private mtRowStep As Autodesk.AutoCAD.Geometry.Vector3d
		Private mtAfterTableStep As Autodesk.AutoCAD.Geometry.Vector3d
		Private mtTableWidth As Autodesk.AutoCAD.Geometry.Vector3d
		Private mtScale3d As Autodesk.AutoCAD.Geometry.Scale3d

		Private mdAreaSumD As Double


		Public Sub New(iActionType As UnidivNet.enActionType, iTableNumber As Integer)
			miActionType = iActionType
			miTableNumber = iTableNumber
			zzOpenAcadBlocks()
			mtScale3d = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor())
			mtHeaderStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdHeaderHeight * mtScale3d.Y, 10.0)
			mtTableWidth = New Autodesk.AutoCAD.Geometry.Vector3d(-mdTableWidth * mtScale3d.X, 0.0, 10.0)

			mtRowStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdRowHeight * mtScale3d.Y, 0.0)
			mtAfterTableStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdAfterTableDistance * mtScale3d.Y, 0.0)

		End Sub
		Public Sub SetBasePoint(ByRef tBaseBoint As Autodesk.AutoCAD.Geometry.Point3d)
			mtBaseBoint = tBaseBoint

			mtCurrentPoint = mtBaseBoint
		End Sub
		Public Function GetNewBasePoint() As Autodesk.AutoCAD.Geometry.Point3d

			mtCurrentPoint = mtCurrentPoint.Add(mtAfterTableStep)
			Return mtCurrentPoint
		End Function
		Public Sub InsertHeader()

			Dim tTestPoint As Autodesk.AutoCAD.Geometry.Point3d = mtCurrentPoint

			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLayer, DMAcadExt.DMApp.AppID, True, True)
			zzInsertHeader()

			mtCurrentPoint = mtBaseBoint.Add(mtHeaderStep)
			'DMAcadExt.AcadDocument.WriteMessage("Prev=" & tTestPoint.ToString() & "; Step=" & mtHeaderStep.ToString() & "; Step=" & mtHeaderStep.ToString())
		End Sub
		Public Sub InsertRow(tParcelKey As UD_ParcelKey, tPlanExt As PlanExt, dAreaD As Double, bTransfer As Boolean, tTransferToParcelKey As UD_ParcelKey)
			zzInsertRow(tParcelKey, tPlanExt, dAreaD, False, bTransfer, tTransferToParcelKey)
			mtCurrentPoint = mtCurrentPoint.Add(mtRowStep)
			mdAreaSumD += dAreaD
		End Sub
		Public Sub InsertSumRow(tParcelKey As UD_ParcelKey, tPlanExt As PlanExt)

			zzInsertLine()
			zzInsertRow(tParcelKey, tPlanExt, mdAreaSumD, True, False, New UD_ParcelKey())
			mtCurrentPoint = mtCurrentPoint.Add(mtRowStep)
			zzInsertLine()

			'''''''''''''''     zzInsertLine()
			mdAreaSumD = 0.0
		End Sub

		Private Sub zzOpenAcadBlocks()
			Dim msHeaderBlockName As String
			Dim msRowBlockName As String
			Dim saRowTags() As String
			Dim msUniDivRowTags() As String = {"TABA_MIGRASH", "TEMP_NAME", "FINAL_NAME", "LEGAL_AREA", "ROW_NUM", "TBL_NUM", "SUMMARIZE_ROW", "TABA_YEUD_DESC"}
			Dim msTransferRowTags() As String = {"TEMP_NAME", "FINAL_NAME", "LEGAL_AREA", "TO_GUSH", "TO_TEMP_NAME", "TO_FINAL_NAME", "ROW_NUM", "TBL_NUM"}

			Select Case miActionType
				Case UnidivNet.enActionType.Divide
					msHeaderBlockName = "DIVIDE_HEADER"
					msRowBlockName = "DIVIDE_TABLE"
					saRowTags = msUniDivRowTags
					msLayer = "DIVIDE"
				Case UnidivNet.enActionType.Union
					msHeaderBlockName = "UNION_HEADER"
					msRowBlockName = "UNION_TABLE"
					saRowTags = msUniDivRowTags
					msLayer = "UNION"
				Case UnidivNet.enActionType.Transfer
					msHeaderBlockName = "TRANSFER_HEADER"
					msRowBlockName = "TRANSFER_TABLE"
					saRowTags = msTransferRowTags
					msLayer = "TRANSFER"
				Case Else
					msHeaderBlockName = Nothing
					msRowBlockName = Nothing
					saRowTags = Nothing
			End Select
			moHeaderAcadBlock = New DMAcadExt.AcadBlock(msHeaderBlockName, UnidivNet.UD_App.BlockPath13)
			moHeaderAcadBlock.OpenForRight(False)
			moHeaderAcadBlock.Fields = {"TBL_NUM"}
			moRowAcadBlock = New DMAcadExt.AcadBlock(msRowBlockName, UnidivNet.UD_App.BlockPath13)
			moRowAcadBlock.OpenForRight(False)
			moRowAcadBlock.Fields = saRowTags

		End Sub
		Private Sub zzInsertLine()
			Dim oNewLine As Line = New Line(mtCurrentPoint, mtCurrentPoint.Add(mtTableWidth))


			DMAcadExt.AcadTransaction.AppendEntity(oNewLine)
		End Sub

		Private Sub zzInsertHeader()
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			tBlockRefData.Position = mtBaseBoint
			tBlockRefData.AttribValues = {miTableNumber.ToString()}
			tBlockRefData.Layer = msLayer
			tBlockRefData.ScaleFactors = mtScale3d
			moHeaderAcadBlock.InsertRef(tBlockRefData)
		End Sub
		Private Sub zzInsertRow(tParcelKey As UD_ParcelKey, tPlanExt As PlanExt, dAreaD As Double, bSummarize As Boolean, bTransfer As Boolean, tTransferToParcelKey As UD_ParcelKey)
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			tBlockRefData.Position = mtCurrentPoint
			Dim sSummarize As String

			If bSummarize Then
				sSummarize = "1"
			Else
				sSummarize = "0"
			End If


			If bTransfer Then
				tBlockRefData.AttribValues = {tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dAreaD, 3), tTransferToParcelKey.BlockName, tTransferToParcelKey.TempName, tTransferToParcelKey.FinalName, miRowNumber.ToString(), miTableNumber.ToString()}
			Else
				'tBlockRefData.AttribValues = {DMCommon.Hebrew.WordToDOS(tPlanExt.LotName, False), tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, DMCommon.Hebrew.WordToDOS(tPlanExt.LanduseName, False)}  '"Yeud"
				'tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, DMCommon.Hebrew.WordToDOS(tPlanExt.LanduseName, False)}  '"Yeud"
				'tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, tPlanExt.LanduseName}  '"Yeud"
				'tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, DMCommon.Hebrew.FromAcadA(tPlanExt.LanduseName)}  '"Yeud"
				tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dAreaD, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, DMCommon.Hebrew.Invert(tPlanExt.LanduseName)}  '"Yeud"
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!FinalName", tParcelKey.ParcelNo, tParcelKey.FinalName, tParcelKey.FinalNameInBrackets)
				'	tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), sSummarize, tPlanExt.LanduseName}  '"Yeud"





			End If

			tBlockRefData.Layer = msLayer
			tBlockRefData.ScaleFactors = mtScale3d
			moRowAcadBlock.InsertRef(tBlockRefData)
			miRowNumber += 1
		End Sub
	End Class
	Private Class AreaTable
		Const msFDBTableLayer As String = "FDB_TABLE"
		Const msLRTableLayer As String = "LR_TABLE"

		Const msFDBHeaderBlockName As String = "FDB_HEADER" ' "LR_HEADER - Copy" '
		Const msLRHeaderBlockName As String = "LR_HEADER"
		Const msFDBRowBlockName As String = "FDB_TABLE" '"LR_TABLE - Copy"  '"FDB_TABLE"
		Const msLRRowBlockName As String = "LR_TABLE"

		Const mdHeaderHeight As Double = 6.0
		Const mdRowHeight As Double = 6.0
		Const dAfterTableDistance As Double = 20.0
		Private moHeaderAcadBlock As DMAcadExt.AcadBlock

		Private moRowAcadBlock As DMAcadExt.AcadBlock
		Private mtBaseBoint As Autodesk.AutoCAD.Geometry.Point3d
		Private mtCurrentBoint As Autodesk.AutoCAD.Geometry.Point3d
		Private mtHeaderStep As Autodesk.AutoCAD.Geometry.Vector3d
		Private mtRowStep As Autodesk.AutoCAD.Geometry.Vector3d
		Private miRowNumber As Integer = 1
		Private miAreaTable As enAreaTable
		Private msLayer As String
		Private msHeaderBlockName As String
		Private msaHeaderFields() As String
		Private msRowBlockName As String
		Private msaRowFields() As String
		Private mtScale3d As Autodesk.AutoCAD.Geometry.Scale3d

		Public Sub InsertHeader()
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLayer, DMAcadExt.DMApp.AppID, True, True)
			zzInsertHeader()
			mtCurrentBoint = mtBaseBoint.Add(mtHeaderStep)
		End Sub
		Public Sub InsertRow(tParcelKey As UD_ParcelKey, tParcelArea As ParcelArea)
			zzInsertRow(tParcelKey, tParcelArea)
			mtCurrentBoint = mtCurrentBoint.Add(mtRowStep)

		End Sub

		Private Sub zzInsertHeader()
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			tBlockRefData.Position = mtBaseBoint

			tBlockRefData.Layer = msLayer
			tBlockRefData.ScaleFactors = mtScale3d
			moHeaderAcadBlock.InsertRef(tBlockRefData)
		End Sub

		Public Sub New(iAreaTable As enAreaTable)
			miAreaTable = iAreaTable
			Select Case miAreaTable
				Case enAreaTable.FDB
					msLayer = msFDBTableLayer
					msHeaderBlockName = msFDBHeaderBlockName
					msaHeaderFields = Nothing
					msRowBlockName = msFDBRowBlockName
					msaRowFields = {"ROW_NUM", "FINAL_NAME", "LEGAL_AREA", "CALC_AREA", "DIFF_AREA", "DIFF_CHECK"}
				Case enAreaTable.LR
					msLayer = msLRTableLayer
					msHeaderBlockName = msLRHeaderBlockName
					msaHeaderFields = {"COMMENT"}
					msRowBlockName = msLRRowBlockName
					msaRowFields = {"ROW_NUM", "PARCEL_NAME", "CALC_AREA"}
			End Select
			mtScale3d = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor())
			mtHeaderStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdHeaderHeight * mtScale3d.Y, 0.0)
			mtRowStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdRowHeight * mtScale3d.Y, 0.0)
			moHeaderAcadBlock = New DMAcadExt.AcadBlock(msHeaderBlockName, UnidivNet.UD_App.BlockPath13)
			moHeaderAcadBlock.OpenForRight(False)
			If msaHeaderFields IsNot Nothing Then
				moHeaderAcadBlock.Fields = msaHeaderFields
			End If
			DMCommon.Debug.MsgBox("13_043K", msRowBlockName, UnidivNet.UD_App.BlockPath13)
			moRowAcadBlock = New DMAcadExt.AcadBlock(msRowBlockName, UnidivNet.UD_App.BlockPath13)
			DMCommon.Debug.MsgBox("13_043L", msRowBlockName, UnidivNet.UD_App.BlockPath13)
			moRowAcadBlock.OpenForRight(False)
			DMCommon.Debug.MsgBox("13_043M", msRowBlockName, UnidivNet.UD_App.BlockPath13)
			moRowAcadBlock.Fields = msaRowFields


		End Sub
		Public Sub SetBasePoint(ByRef tBaseBoint As Autodesk.AutoCAD.Geometry.Point3d)
			mtBaseBoint = tBaseBoint

			mtCurrentBoint = mtBaseBoint
		End Sub
		Private Sub zzInsertRow(tParcelKey As UD_ParcelKey, tParcelArea As ParcelArea)
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			tBlockRefData.Position = mtCurrentBoint
			Select Case miAreaTable
				Case enAreaTable.FDB
					tBlockRefData.AttribValues = {miRowNumber.ToString(), tParcelKey.FinalName, zzAreaToString(tParcelArea.LegalArea, True), zzAreaToString(tParcelArea.CalcArea, True), zzAreaToString(tParcelArea.DeltaAreaM, True), tParcelArea.IsProperText}
				Case enAreaTable.LR
					tBlockRefData.AttribValues = {miRowNumber.ToString(), tParcelKey.UD_ParcelName, zzAreaToString(tParcelArea.CalcArea, True)}
			End Select
			'	DMCommon.Debug.MsgBox("13_043E", miAreaTable)

			tBlockRefData.Layer = msLayer
			tBlockRefData.ScaleFactors = mtScale3d
			moRowAcadBlock.InsertRef(tBlockRefData)
			miRowNumber += 1
		End Sub
	End Class
	Private Shared Function zzAreaToString(dArea As Double, bAreaInMeters As Boolean) As String
		If bAreaInMeters Then
			dArea = 0.001 * dArea
		End If
		Return FormatNumber(dArea, 3)
	End Function


	Private Sub cmdInsertTable_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertTable.Click
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
		Me.Visible = False
		zzUpdateCurrentScale()




		AppActivate(moAppWin.Text)
		Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
		zzOpenDWG()




		If zzSelectPoint(tPoint, True) = PromptStatus.OK Then

			Select Case miPlanType
				Case frmUD_General.enPlanType.PlanType1
					zzInsertTatagTable(tPoint, enAreaTable.LR) 'seder
				Case frmUD_General.enPlanType.PlanType2
					zzInsertTazarTable(tPoint)
				Case frmUD_General.enPlanType.PlanType9
					' msPlanType9
				Case frmUD_General.enPlanType.PlanType12
					zzInsertTatagTable(tPoint, enAreaTable.FDB)
				Case Else

			End Select

			If miCurrentStage = 0 Then
				'zzInsertTatagTable(tPoint)
			Else
				'zzInsertTazarTable(tPoint)
			End If
		End If
		zzCloseDWG()
		Me.Visible = True
	End Sub
	Private Sub zzInsertTazarTable(tPoint As Autodesk.AutoCAD.Geometry.Point3d)
		Dim oJournalTable As JournalTable = Nothing '= New JournalTable(UnidivNet.enActionType.Divide, 1)
		Dim iStage As Integer
		Dim iAction As Integer
		Dim iActionType As UnidivNet.enActionType
		Dim iRowStatus As enRowStatus
		Dim tSumParcelKey As UD_ParcelKey
		Dim tRowParcelKey As UD_ParcelKey
		Dim iParcelDbID As Integer
		Dim tTransferToRowParcelKey As UD_ParcelKey
		Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
		Dim oSumParcel As UnidivNet.UD_Parcel = Nothing
		Dim dLegalAreaD As Double

		Dim tRowPlanExt As PlanExt = New PlanExt()
		Dim tSumPlanExt As PlanExt = New PlanExt()

		'  Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus



		For Each oDataRow As DataRow In moMainTable.Rows

			iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
			'  DMCommon.Debug.MsgBox("12_850", iStage, moMainTable.Rows.Count)
			If iStage <> 0 Then
				iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
				iActionType = zzToActionType(oDataRow)
				iRowStatus = zzGetRowStatus(oDataRow)
				dLegalAreaD = DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))
				'   DMCommon.Debug.MsgBox("12_851", iStage, iActionType, iAction, iRowStatus)
				If iRowStatus = enRowStatus.StartOfAction OrElse iRowStatus = enRowStatus.Transfer Then
					tSumParcelKey = zzGetSumParcelKey(iActionType, oDataRow)
					oSumParcel = zzGetSumParcel(iActionType, oDataRow)


					If iAction = 1 Then
						If oJournalTable IsNot Nothing Then
							tPoint = oJournalTable.GetNewBasePoint()
						End If
						oJournalTable = New JournalTable(iActionType, iStage)
						oJournalTable.SetBasePoint(tPoint)
						oJournalTable.InsertHeader()
					End If
					If iActionType = UnidivNet.enActionType.Union OrElse iActionType = UnidivNet.enActionType.Transfer Then
						tSumPlanExt = New PlanExt(oDataRow)
						'	DMCommon.Debug.MsgBox("12_857a", tSumPlanExt.Plan, tSumPlanExt.LanduseName, tSumPlanExt.LotName)
					Else
						tSumPlanExt = New PlanExt()
					End If

				End If

				If iActionType = UnidivNet.enActionType.Divide Then
					' tRowParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
					tRowParcelKey = zzParcelKeyTo(oDataRow)
					iParcelDbID = zzParcelDbID(oDataRow, False)

					tSumParcelKey.FinalNameInBrackets = True
					tRowPlanExt = New PlanExt(oDataRow)
				Else
					tRowPlanExt = New PlanExt()
					'  tRowParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
					tRowParcelKey = zzParcelKeyFrom(oDataRow)
					iParcelDbID = zzParcelDbID(oDataRow, True)
				End If
				If iActionType = UnidivNet.enActionType.Transfer Then
					tTransferToRowParcelKey = zzParcelKeyTo(oDataRow)
				End If

				If mdicParcels.TryGetValueByDbID(iParcelDbID, oRowParcel) Then
					oJournalTable.InsertRow(tRowParcelKey, tRowPlanExt, oRowParcel.ParcelArea.LegalAreaD, iActionType = UnidivNet.enActionType.Transfer, tTransferToRowParcelKey)
					'iActionType
				Else
					DMCommon.Debug.MsgBox("09_772!!", iActionType, mdicParcels.Count, tRowParcelKey, iParcelDbID)
					mdicParcels.DebugMsg()
				End If

				'End If
				If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
					'DMCommon.Debug.MsgBox("12_857end", tSumPlanExt.Plan, tSumPlanExt.LanduseName, tSumPlanExt.LotName)
					'oJournalTable.InsertSumRow(oSumParcel.ParcelKey, tSumPlanExt)
					oJournalTable.InsertSumRow(tSumParcelKey, tSumPlanExt)

				End If
			End If
		Next



	End Sub
	Private Sub zzInsertTazarTable_240319(tPoint As Autodesk.AutoCAD.Geometry.Point3d)
		Dim oJournalTable As JournalTable = Nothing '= New JournalTable(UnidivNet.enActionType.Divide, 1)
		Dim iStage As Integer
		Dim iAction As Integer
		Dim iActionType As UnidivNet.enActionType
		Dim iRowStatus As enRowStatus
		Dim tSumParcelKey As UD_ParcelKey
		Dim tRowParcelKey As UD_ParcelKey
		Dim tTransferToRowParcelKey As UD_ParcelKey
		Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
		Dim dLegalAreaD As Double

		Dim tRowPlanExt As PlanExt = New PlanExt()
		Dim tSumPlanExt As PlanExt = New PlanExt()

		'  Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus



		For Each oDataRow As DataRow In moMainTable.Rows

			iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
			'  DMCommon.Debug.MsgBox("12_850", iStage, moMainTable.Rows.Count)
			If iStage <> 0 Then
				iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
				iActionType = zzToActionType(oDataRow)
				iRowStatus = zzGetRowStatus(oDataRow)
				dLegalAreaD = DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))
				'   DMCommon.Debug.MsgBox("12_851", iStage, iActionType, iAction, iRowStatus)
				If iRowStatus = enRowStatus.StartOfAction OrElse iRowStatus = enRowStatus.Transfer Then
					tSumParcelKey = zzGetSumParcelKey(iActionType, oDataRow)
					'If iActionType = UnidivNet.enActionType.Divide Then
					'   'tSumParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
					'   tSumParcelKey = zzParcelKeyFrom(oDataRow)
					'Else
					'   'tSumParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
					'   tSumParcelKey = zzParcelKeyTo(oDataRow)
					'End If
					'  DMCommon.Debug.MsgBox("12_852", miBlockNo, iStage, iActionType, iAction, iRowStatus, tSumParcelKey)
					If iAction = 1 Then
						If oJournalTable IsNot Nothing Then
							tPoint = oJournalTable.GetNewBasePoint()
						End If
						oJournalTable = New JournalTable(iActionType, iStage)
						oJournalTable.SetBasePoint(tPoint)
						oJournalTable.InsertHeader()
					End If
					If iActionType = UnidivNet.enActionType.Union OrElse iActionType = UnidivNet.enActionType.Transfer Then
						tSumPlanExt = New PlanExt(oDataRow)
						'   DMCommon.Debug.MsgBox("12_857a", tSumPlanExt.Plan, tSumPlanExt.LanduseName, tSumPlanExt.LotName)
					Else
						tSumPlanExt = New PlanExt()
					End If

				End If

				If iActionType = UnidivNet.enActionType.Divide Then
					' tRowParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
					tRowParcelKey = zzParcelKeyTo(oDataRow)
					tSumParcelKey.FinalNameInBrackets = True
					tRowPlanExt = New PlanExt(oDataRow)
				Else
					tRowPlanExt = New PlanExt()
					'  tRowParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
					tRowParcelKey = zzParcelKeyFrom(oDataRow)
				End If
				If iActionType = UnidivNet.enActionType.Transfer Then
					tTransferToRowParcelKey = zzParcelKeyTo(oDataRow)
				End If

				If mdicParcels.TryGetValue(tRowParcelKey, oRowParcel) Then
					oJournalTable.InsertRow(tRowParcelKey, tRowPlanExt, oRowParcel.ParcelArea.LegalArea, iActionType = UnidivNet.enActionType.Transfer, tTransferToRowParcelKey)
					'iActionType
				Else
					DMCommon.Debug.MsgBox("09_772!!", mdicParcels.Count, tRowParcelKey)
					mdicParcels.DebugMsg()
				End If



				'End If
				If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
					' DMCommon.Debug.MsgBox("12_857end", tSumPlanExt.Plan, tSumPlanExt.LanduseName, tSumPlanExt.LotName)
					oJournalTable.InsertSumRow(tSumParcelKey, tSumPlanExt)
				End If
			End If
		Next



	End Sub
	Private Sub zzInsertTatagTable(tPoint As Autodesk.AutoCAD.Geometry.Point3d, iAreaTable As enAreaTable)
		Dim oAreaTable As AreaTable = New AreaTable(iAreaTable)
		oAreaTable.SetBasePoint(tPoint)
		Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
		Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = mdicParcels.GetSorted()
		oAreaTable.InsertHeader()

		If False Then
			For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
				oAreaTable.InsertRow(oParcel.ParcelKey, oParcel.ParcelArea)

			Next
		End If
		For Each oParcel As UnidivNet.UD_Parcel In colParcels
			oAreaTable.InsertRow(oParcel.ParcelKey, oParcel.ParcelArea)
		Next
	End Sub
	Private Function zzGetSelected() As ObjectIdCollection
		Dim colRes As ObjectIdCollection = New ObjectIdCollection()
		For Each oParcel As UnidivNet.UD_Parcel In mhsSelectedParcels
			colRes.Add(oParcel.CentroidAcObjID)
		Next
		'  DMCommon.Debug.MsgBox("12_894", colRes.Count)
		Return colRes
	End Function
	Private Function zzGetEntitySet(bSingleOnly As Boolean, sLayers As String, colAllowable As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, iSelectedColorIndex As Integer) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
		' Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim iIndex As Integer = -1
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		'   Dim oaValues(colLayers.Count) As TypedValue
		'   For Each sLayer As String In colLayers
		'iIndex += 1
		'  oaValues(iIndex) = New TypedValue(DxfCode.LayerName, sLayer)
		'  Next
		Dim bSuccess As Boolean

		Dim oaValues(1) As TypedValue
		oaValues(0) = New TypedValue(DxfCode.Color, iSelectedColorIndex)
		oaValues(1) = New TypedValue(DxfCode.LayerName, sLayers)



		' Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer), New TypedValue(DxfCode.Color, iSelectedColorIndex)}
		'    DMCommon.Debug.MsgBox("12_280", colAllowable.Count)
		Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colAllowable, iSelectedColorIndex)
		'  DMCommon.Debug.MsgBox("09_771", oEntitySet.Count)
		Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter = New SelectionFilter(oaValues)
		'  zzCloseDWG()

		''''''''''''  zzOpenDWG()
		DMAcadExt.AcadDocument.Regen()
		oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		oPromptOpt.AllowDuplicates = False



		' oPromptOpt.MessageForAdding = "Add ***"
		' oPromptOpt.MessageForRemoval = "Remove ***"


		oPromptOpt.SingleOnly = bSingleOnly
		oPromptOpt.SinglePickInSpace = False ' bSingleOnly '
		oPromptOpt.AllowSubSelections = True
		Me.Visible = False
		AppActivate(moAppWin.Text)
		Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
		Do
			bSuccess = False

			ptRes = oEditor.GetSelection(oPromptOpt, oFilter)


			If ptRes.Value Is Nothing Then
				oEditor.WriteMessage("Val: " & iIndex.ToString() & "; Nothing SingleOnly=" & oPromptOpt.SingleOnly.ToString & "; SinglePickInSpace=" & oPromptOpt.SinglePickInSpace.ToString)
			Else
				oEditor.WriteMessage("D: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count))

			End If

			If ptRes.Value IsNot Nothing Then
				If bSingleOnly Then
					bSuccess = (ptRes.Value.Count = 1)
				Else
					bSuccess = (ptRes.Value.Count > 0)
				End If
				'  DMCommon.Debug.MsgBox("12_285", ptRes.Value.Count, ptRes.Status, oPromptOpt.SingleOnly, oPromptOpt.SinglePickInSpace)
				If bSuccess Then
					For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
						tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
						colSelected.Add(tAcObjID)
					Next
				End If

			Else
				'   DMCommon.Debug.MsgBox("12_286", ptRes.Value IsNot Nothing, ptRes.Status, oPromptOpt.SingleOnly, oPromptOpt.SinglePickInSpace)
			End If
		Loop Until ptRes.Status = PromptStatus.Cancel OrElse bSuccess
		'    DMCommon.Debug.MsgBox("09_769", colSelected.Count, ptRes.Status)
		'  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

		'  oEditor.SetImpliedSelection(oSelSet)
		'  oEditor.SetImpliedSelection(taAcObjIDs)

		' oPromptOpt.ForceSubSelections = True


		Me.Visible = True


		oEntitySet.RestoreColor()
		Return colSelected
	End Function

	Private Function zzGetLinkSet(colAllowable As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, iSelectLinkType As enSelectLinkType, Optional oSourceParcel As UnidivNet.UD_Parcel = Nothing) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Const sTopoName As String = "Links"

		Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
		' Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim iIndex As Integer = -1


		DMAcadExt.AcadDocument.Regen()


		' oPromptOpt.SingleOnly = True
		'  oPromptOpt.SinglePickInSpace = True
		'   oPromptOpt.AllowSubSelections = True
		If iSelectLinkType <> enSelectLinkType.Script Then
			Me.WindowState = FormWindowState.Minimized
			AppActivate(moAppWin.Text)
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			DMAcadExt.AcadDocument.Regen()
			DMAcadExt.AcadDocument.SetDrawVectorSetActive(True)
		End If


		'DMCommon.Debug.MsgBox("13_043c", iSelectLinkType, sTopoName, colAllowable.Count)

		Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colAllowable, miSelectedColorIndex)

		If iSelectLinkType = enSelectLinkType.SelectLinks Then
			'DMCommon.Debug.MsgBox("13_022k", sTopoName)
			If zzCreateTopology(sTopoName, False, colAllowable) Then
				zzGetLinksBy(sTopoName, Nothing, colSelected)
				zzDeleteTopology(sTopoName)
			End If
		ElseIf iSelectLinkType = enSelectLinkType.SelectLinksByFragments Then
			zzGetLinksBy(msFragmentsTopoName, colAllowable, colSelected)
		ElseIf iSelectLinkType = enSelectLinkType.SelectionSet Then
			'	Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer), New TypedValue(DxfCode.LayerName, msLinkNewBridgeLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)}
			'	Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer & "," & msLinkNewBridgeLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)}   '12/08/19
			Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer & "," & msLinkBridgeLayer & "," & msLinkNewBridgeLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)} ''  070724
			'''''	Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer & "," & msLinkNewBridgeLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)} '  070724



			zzGetLinksBySelection(oaValues, colSelected)
		ElseIf iSelectLinkType = enSelectLinkType.Script Then
			'	DMCommon.Debug.MsgBox("13_146c", oSourceParcel Is Nothing)
			zzGetLinksByScript(oSourceParcel, colSelected)
			If mtCurrentScriptData.ScriptType = enScriptType.LinkList Then
				colSelected = mtCurrentScriptData.LinkObjIDs
			End If
			mtCurrentScriptData.Close()
			Me.chkRestoreCancelLink.Checked = False

		End If



		'    DMCommon.Debug.MsgBox("09_763", colSelected.Count, ptRes.Status)
		'  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

		'  oEditor.SetImpliedSelection(oSelSet)
		'  oEditor.SetImpliedSelection(taAcObjIDs)

		' oPromptOpt.ForceSubSelections = True
		If iSelectLinkType <> enSelectLinkType.Script Then
			Me.WindowState = FormWindowState.Normal
		End If




		oEntitySet.RestoreColor()
		Return colSelected
	End Function


	Private Function zzGetLinksByScript(oSourceParcel As UnidivNet.UD_Parcel, ByRef colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
		Dim sFileName As String = mtCurrentScriptData.FileName
		Dim iChildIndex As Integer = 0
		'	Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim iDbID As Integer
		Dim oXMLDocument As System.Xml.XmlDocument = New System.Xml.XmlDocument()
		Dim oParcelNameNode As Xml.XmlNode
		'	DMCommon.Debug.MsgBox("13_146K", oSourceParcel Is Nothing)
		Dim sParcelName As String = Nothing
		oXMLDocument.Load(sFileName)
		Dim oDocNode As Xml.XmlNode = oXMLDocument.DocumentElement
		Dim oActionNode As Xml.XmlNode = oDocNode.FirstChild
		If oSourceParcel IsNot Nothing Then
			Dim oSourceParcelNode As Xml.XmlNode = oActionNode.ChildNodes(iChildIndex)
			Dim oParcelDbIDNode As Xml.XmlNode = oSourceParcelNode.ChildNodes(0)
			iChildIndex += 1

			oParcelNameNode = oSourceParcelNode.ChildNodes(1)
			Integer.TryParse(oParcelDbIDNode.InnerText, iDbID)
			sParcelName = oParcelNameNode.InnerText
		End If


		Dim oLinksNode As Xml.XmlNode = oActionNode.ChildNodes(iChildIndex)



		'Dim oLinksNode As Xml.XmlNode = oNode.FirstChild
		Dim oLinkObjIDNode As Xml.XmlNode


		Dim sAcObjIDString As String
		Dim sHandelString As String
		Dim tAcObjID As ObjectId
		'	DMCommon.Debug.MsgBox("13_146c!", DMAcadExt.AcadTransaction.ModelSpaceObjID, oSourceParcel Is Nothing)
		If oSourceParcel Is Nothing OrElse oSourceParcel.DbID = iDbID Then


			'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			DMAcadExt.AcadTransaction.OpenHandleDictionary()

			For Each oNodeLink As Xml.XmlNode In oLinksNode.ChildNodes
				iDbID = 0
				oParcelNameNode = oNodeLink.ChildNodes(1)
				sHandelString = oParcelNameNode.InnerText
				tAcObjID = DMAcadExt.AcadTransaction.GetObjectID(sHandelString)

				If Not tAcObjID.IsNull Then
					colSelected.Add(tAcObjID)
				End If
				oLinkObjIDNode = oNodeLink.ChildNodes(0)
				sAcObjIDString = oLinkObjIDNode.InnerText
				'DMCommon.Debug.MsgBox("13_022s", sHandelString, tAcObjID, sAcObjIDString)

			Next
			'DMAcadExt.AcadTransaction.CloseModelSpace()
		ElseIf sParcelName IsNot Nothing Then
			DMCommon.Debug.UserMsg("Msg #523", oSourceParcel.UD_Name, sParcelName, oSourceParcel.DbID, iDbID)
		End If

		'
		'DMCommon.Debug.MsgBox("11_165n", colSelected.Count)

	End Function
	Private Function zzGetLinksBySelection(oaValues() As TypedValue, ByRef colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		oPromptOpt.AllowDuplicates = False
		For i As Integer = 0 To oaValues.GetUpperBound(0)
			DMCommon.Debug.MsgBox("11_165x", oaValues(i).TypeCode, oaValues(i).Value)
		Next
		'Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer), New TypedValue(DxfCode.LayerName, msLinkNewBridgeLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)}
		Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter = New SelectionFilter(oaValues) '

		Me.Visible = False
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		ptRes = oEditor.GetSelection(oPromptOpt, oFilter)
		If ptRes.Value IsNot Nothing Then
			For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
				tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
				colSelected.Add(tAcObjID)
			Next
		End If
		Me.Visible = True
	End Function
	Private Function zzGetLinksBy(sTopoName As String, colAllowable As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean



		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions(vbCrLf & "Select Link")
		'oPromptOpt.SetMessageAndKeywords("Finish? [Y]:", "Y")
		oPromptOpt.AllowNone = True
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'	Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
		'Dim colSumAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()

		Dim iIndex As Integer = -1
		'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		'DMAcadExt.AcadTransaction.Start()
		'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		'  oPromptOpt.
		Dim oLinkTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		'	DMCommon.Debug.MsgBox("13_049d", oLinkTopology IsNot Nothing, sTopoName)
		If oLinkTopology IsNot Nothing Then


			Dim colFullEdges As Autodesk.Gis.Map.Topology.FullEdgeCollection = oLinkTopology.GetFullEdges()
			'	Dim dicFullEdges As Dictionary(Of ObjectId, Integer) = New Dictionary(Of ObjectId, Integer)()

			'Dim oFullEdge As Autodesk.Gis.Map.Topology.FullEdge
			Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
			Dim taNullAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
			'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			'DMAcadExt.AcadTransaction.Start()

			'	For Each oFullEdge In colFullEdges
			'dicFullEdges.Add(oFullEdge.Entity, oFullEdge.ID)
			'	Next
			Dim oTopoNetWork As TopoManager.TopoNetWork = New TopoManager.TopoNetWork(sTopoName)
			oTopoNetWork.Load(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

			Do
				ptRes = oEditor.GetEntity(oPromptOpt)
				'	DMCommon.Debug.MsgBox("13_045H", ptRes.Status.ToString(), ptRes.StringResult, ptRes.ObjectId, colAllowable IsNot Nothing)

				If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK AndAlso (colAllowable Is Nothing OrElse colAllowable.Contains(ptRes.ObjectId)) Then

					colAcObjIDs = oTopoNetWork.GetRoute(ptRes.ObjectId)

					For Each tAcObjID As ObjectId In colAcObjIDs
						If Not colSelected.Contains(tAcObjID) Then
							colSelected.Add(tAcObjID)
							DMAcadExt.AcadTransaction.Highlight(tAcObjID)
						End If
					Next

					'DMCommon.Debug.MsgBox("13_045I", DMAcadExt.AcadDocument.IsLocked, DMCommon.Debug.ColCount(colAcObjIDs), DMCommon.Debug.ColCount(taAcObjIDs), ptRes.Status.ToString(), ptRes.StringResult)
				ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colSelected
						DMAcadExt.AcadTransaction.Unhighlight(tAcObjID)
					Next

					Exit Do
				ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then

					Exit Do
				ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Keyword Then

					'DMAcadExt.AcadTransaction.Terminate()
					'	DMAcadExt.AcadDocument.Unlock()
					Exit Do
				Else
					DMCommon.Debug.MsgBox("13_045W")
					Exit Do
				End If

			Loop


			'DMAcadExt.AcadTransaction.Terminate()
			'DMAcadExt.AcadDocument.Unlock()
			'DMAcadExt.AcadDocument.UpdateScreen()


			oLinkTopology.Close()
		End If
	End Function
	Private Function zzGetRowParcelKey(iActionType As UnidivNet.enActionType, oDataRow As DataRow) As UD_ParcelKey
		If iActionType = UnidivNet.enActionType.Registered Then
			' Return New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
			Return zzParcelKeyFrom(oDataRow)

		Else
			' Return New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
			Return zzParcelKeyTo(oDataRow)
		End If
	End Function
	Private Function zzGetSumParcelKey(iActionType As UnidivNet.enActionType, oDataRow As DataRow) As UD_ParcelKey
		If iActionType = UnidivNet.enActionType.Divide Then
			' Return New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
			Return zzParcelKeyFrom(oDataRow)
		ElseIf iActionType = UnidivNet.enActionType.Union Then
			' Return New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
			Return zzParcelKeyTo(oDataRow)
		End If
	End Function
	Private Function zzGetSumParcel(iActionType As UnidivNet.enActionType, oDataRow As DataRow) As UnidivNet.UD_Parcel
		If iActionType = UnidivNet.enActionType.Divide Then
			Return zzGetParcelByDbID(oDataRow, True)
		ElseIf iActionType = UnidivNet.enActionType.Union Then
			Return zzGetParcelByDbID(oDataRow, False)

		Else
			'DMCommon.Debug.MsgBox("Err #152", iActionType, oDataRow.Item("ParcelDbID"), oDataRow.Item("ParcelSourceDbID"))
			Return Nothing
		End If
	End Function

	Private Function zzToActionType(oDataRow As DataRow) As UnidivNet.enActionType
		Dim oValue As System.Object = oDataRow.Item("ActionType")
		Dim iActionType As Integer = DMCommon.Functions.CIntN(oValue)
		If [Enum].IsDefined(GetType(UnidivNet.enActionType), iActionType) Then
			Return CType(iActionType, UnidivNet.enActionType)
		Else
			Return UnidivNet.enActionType.Default
		End If
	End Function
	Private Function zzGetRowStatus(oDataRow As DataRow) As enRowStatus
		Dim oValue As System.Object
		Try
			oValue = oDataRow.Item("RowStatus")
		Catch oEx As Exception
			oValue = Nothing
		End Try
		If oValue IsNot Nothing Then
			Dim iRowStatus As Integer = DMCommon.Functions.CIntN(oValue)
			If [Enum].IsDefined(GetType(enRowStatus), iRowStatus) Then
				Return CType(iRowStatus, enRowStatus)
			Else
				Return enRowStatus.Default
			End If
		Else
			Return enRowStatus.Default
		End If


	End Function
	Private Sub zzCreateJournalTable()
		Dim iStage As Integer
		For Each oRow As DataRow In moMainTable.Rows
			iStage = DMCommon.Functions.CIntN(oRow.Item("[Stage]"))
		Next
	End Sub


	Private Sub chkShowHideCol_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkShowHideCol.CheckedChanged
		Me.ctxAcObjID.Visible = Me.chkShowHideCol.Checked
		Me.ctxParcelDbID.Visible = Me.chkShowHideCol.Checked
		Me.ctxOper.Visible = Me.chkShowHideCol.Checked

	End Sub
	Private Function zzCountBlockRefs(sBlockName As String) As Integer
		Dim colCentroids As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs(sBlockName)
		Return colCentroids.Count
	End Function


	Private Sub cmdCancelAction_Click(oSender As System.Object, e As EventArgs) Handles cmdCancelAction.Click
		If miCurrentActionType <> UnidivNet.enActionType.Registered Then

			Dim iIndex As Integer = moMainTable.Rows.Count - 1
			Dim oDataRow As DataRow
			Dim iPrevActionType As UnidivNet.enActionType = UnidivNet.enActionType.Default
			Dim iPrevStage As Integer
			Dim iPrevAction As Integer
			Dim oSourceParcel As UnidivNet.UD_Parcel
			Dim oDestParcel As UnidivNet.UD_Parcel

			Dim iRowStatus As enRowStatus
			Dim iRowState As System.Data.DataRowState
			mbEventsEnabled = False
			zzOpenDWG()
			zzClearAllFrontLines()
			zzDeleteFinalTopology()
			zzAllEnabled(True)
			zzDeleteCurrentTopology()
			Do
				oSourceParcel = Nothing
				oDestParcel = Nothing

				oDataRow = moMainTable.Rows.Item(iIndex)
				If DMCommon.Functions.CIntN(oDataRow.Item("Stage")) = miCurrentStage AndAlso DMCommon.Functions.CIntN(oDataRow.Item("Action")) = miCurrentAction Then
					iRowStatus = zzGetRowStatus(oDataRow)
					Select Case miCurrentActionType
						Case UnidivNet.enActionType.Union
							oSourceParcel = zzParcelFrom(oDataRow)
							If iRowStatus = enRowStatus.StartOfAction Then
								oDestParcel = zzGetParcelTo(oDataRow)
							End If
						Case UnidivNet.enActionType.Divide
							oDestParcel = zzGetParcelTo(oDataRow)
							If iRowStatus = enRowStatus.StartOfAction Then
								oSourceParcel = zzParcelFrom(oDataRow)
							End If
						Case UnidivNet.enActionType.Transfer
							oSourceParcel = zzParcelFrom(oDataRow)
							oDestParcel = zzGetParcelTo(oDataRow)

					End Select

					If oSourceParcel IsNot Nothing Then
						oSourceParcel.Restore()
					End If
					If oDestParcel IsNot Nothing Then
						oDestParcel.Undo(msNewParcelLayer)




						mdicParcels.RemoveParcel(oDestParcel)
						mdicFragments.DeleteParcel(oDestParcel)
					End If

					If iRowStatus = enRowStatus.StartOfAction AndAlso miCurrentAction = 1 Then
						'''''''''''''''''''''''''''''''''''''''''??????   zzClearCurrentStageLayers()
					End If
					oDataRow.Delete()
					Try
						iRowState = oDataRow.RowState()
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show("AcceptChanges" & vbCrLf & oEx.Message & vbCrLf & oEx.GetType().ToString())
					End Try
					If iRowState = DataRowState.Deleted Then
						Try
							oDataRow.AcceptChanges()
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show("iRowState" & vbCrLf & oEx.Message & vbCrLf & oEx.GetType().ToString())
						End Try
					End If

				Else
					iPrevActionType = zzToActionType(oDataRow)
					iPrevStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
					iPrevAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))

					Exit Do
				End If
				iIndex -= 1

			Loop
			zzCancelPoints()

			If iPrevActionType <> UnidivNet.enActionType.Default AndAlso iPrevAction > 0 Then
				miCurrentActionType = iPrevActionType
				miCurrentStage = iPrevStage
				miCurrentAction = iPrevAction
			End If
			miCurrentActionFirstRowIndex = -1
			mhsSelectedParcels.Clear()
			mlstSelectedParcelKeys.Clear()
			mlstSelectedParcels.Clear()

			zzSetActionTypeFrame()
			zzAllowUserToAddDelete(False)
			miLastRowIndex = moMainTable.Rows.Count - 1

			DMAcadExt.AcadDocument.ClearDrawVectorSet()
			If Me.chkActiveParcels.Checked Then
				zzRestoreView()
				zzActiveParcelsView(moUD_ParcelLayerList)
			End If
			zzCloseDWG()
			mbEventsEnabled = True

		End If


	End Sub
	Private Sub zzClearAllFrontLines()
		Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
		Dim sFrontLineLayer As String
		For iStage As Integer = 1 To miCurrentStage
			sFrontLineLayer = UnidivNet.UD_App.GetStageFrontLineLayer(iStage)
			hsHanitLayers.Add(sFrontLineLayer)
		Next

		hsHanitLayers.Add(UnidivNet.UD_App.FinalTopoName)
		DMAcadExt.AcadTransaction.ClearLayerSet(hsHanitLayers)
	End Sub
	Private Sub zzCancelPoints221018()
		Dim colOriginalPoints As ObjectIdCollection = New ObjectIdCollection()
		For Each oPoint As UnidivNet.UD_Point In mdicPoints.Values
			If oPoint.Stage = miCurrentStage AndAlso oPoint.Action = miCurrentAction Then
				If Not oPoint.IsOriginalName Then
					mdicPoints.RemoveName(oPoint.Name)
					oPoint.Name = Nothing
					oPoint.Stage = -1
					oPoint.UpdateBlock(UnidivNet.UD_App.FragmentNewNodesLayer)
				Else
					colOriginalPoints.Add(oPoint.AcObjID)
				End If
			End If
		Next

		DMAcadExt.AcadTransaction.SetLayer(colOriginalPoints, UnidivNet.UD_App.FragmentNewNodesLayer)
	End Sub
	Private Sub zzCancelPoints()
		Dim colOriginalPoints As ObjectIdCollection = New ObjectIdCollection()
		For Each oPoint As UnidivNet.UD_Point In mdicPoints.Values
			If oPoint.Stage = miCurrentStage AndAlso oPoint.Action = miCurrentAction Then
				mdicPoints.ResetPoint(oPoint)
				If Not oPoint.IsOriginalName Then
					mdicPoints.RemoveName(oPoint.Name)
					oPoint.Name = Nothing
					oPoint.Stage = -1
					oPoint.UpdateBlock(UnidivNet.UD_App.FragmentNewNodesLayer)
				Else
					colOriginalPoints.Add(oPoint.AcObjID)
				End If
				zzSetNewPointNo()
			End If
		Next

		DMAcadExt.AcadTransaction.SetLayer(colOriginalPoints, UnidivNet.UD_App.FragmentNewNodesLayer)
	End Sub

	Private Sub zzUpdateCurrentScale()
		mtScale3d = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor())

	End Sub
	Private Sub zzDeleteCurrentTopology()
		Dim sStageTopoName As String = zzGetCurrentStageTopoName()
		TopoManager.TopoCreator.DeleteTopology(sStageTopoName, False, False, False)

	End Sub
	Private Sub zzDeleteFinalTopology()
		TopoManager.TopoCreator.DeleteTopology(UnidivNet.UD_App.FinalTopoName, False, False, False)
	End Sub
	Private Sub zzDeleteAllStageTopos()
		Dim sRowFilter As String = "(RowStatus = 1) And (Stage <> 0)"
		Dim oActionDataView As DataView = New DataView(moMainTable, sRowFilter, String.Empty, DataViewRowState.CurrentRows)
		Dim iStage As Integer
		Dim sStageTopoName As String
		Dim iAction As Integer

		For Each oDataRowView As DataRowView In oActionDataView
			iStage = DMCommon.Functions.CIntN(oDataRowView.Item("Stage"))
			iAction = DMCommon.Functions.CIntN(oDataRowView.Item("Action"))
			sStageTopoName = zzGetStageTopoName(iStage, iAction)
			TopoManager.TopoCreator.DeleteTopology(sStageTopoName, False, False, False)
			DMCommon.Debug.MsgBox("14_300", sStageTopoName)
		Next
	End Sub
	Private Sub zzClearJournalTable()
		Dim sRowFilter As String = "(Stage <> 0)"
		Dim oActionDataView As DataView = New DataView(moMainTable, sRowFilter, String.Empty, DataViewRowState.CurrentRows)

		For Each oDataRowView As DataRowView In oActionDataView
			oDataRowView.Delete()
		Next
		moMainTable.AcceptChanges()
	End Sub
	Private Sub zzClearCurrentStageLayers()
		zzClearStageLayers(miCurrentStage)
	End Sub
	Private Sub zzClearStageLayers(iStage As Integer)
		Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
		Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(iStage)
		hsHanitLayers.Add(sPolylineLayer)

		Dim sParcelCentroidLayer As String = UnidivNet.UD_App.GetStageParcelCentroidLayer(iStage)
		hsHanitLayers.Add(sParcelCentroidLayer)
		DMAcadExt.AcadTransaction.ClearLayerSet(hsHanitLayers)

	End Sub
	Private Sub zzClearAllStageLayers()
		Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
		Dim sPolylineLayer As String
		Dim sParcelCentroidLayer As String
		Dim sFrontLineLayer As String

		For iStage As Integer = 1 To miCurrentStage
			sPolylineLayer = UnidivNet.UD_App.GetStagePolineLayer(iStage)
			hsHanitLayers.Add(sPolylineLayer)
			sParcelCentroidLayer = UnidivNet.UD_App.GetStageParcelCentroidLayer(iStage)
			hsHanitLayers.Add(sParcelCentroidLayer)
			sFrontLineLayer = UnidivNet.UD_App.GetStageFrontLineLayer(iStage)
			hsHanitLayers.Add(sFrontLineLayer)
		Next

		hsHanitLayers.Add(UnidivNet.UD_App.FinalTopoName)
		DMAcadExt.AcadTransaction.ClearLayerSet(hsHanitLayers)

	End Sub
	Private Sub zzChangeNewPointStageLayers()
		Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
		Dim sNewPointLayer As String


		For iStage As Integer = 1 To miCurrentStage
			sNewPointLayer = UnidivNet.UD_App.GetStageUDPointLayer(iStage, True)
			hsHanitLayers.Add(sNewPointLayer)

		Next
		DMAcadExt.AcadTransaction.ChangeLayerSet(hsHanitLayers, UnidivNet.UD_App.FragmentNewNodesLayer)

	End Sub

	Private Sub zzGetParcelsString(ByRef sOrigin As String, ByRef sTemp As String)
		Dim iIncomingParcelsCount As Integer
		Dim iOutgoingParcelsCount As Integer

		If mdicParcels IsNot Nothing Then
			mdicParcels.GetGushString(miBlockNo, miBlockAddNo, sOrigin, sTemp)
			mdicParcels.GetInitParcelString(UnidivNet.UD_App.IsTTG, sOrigin, sTemp, iIncomingParcelsCount, iOutgoingParcelsCount)

		Else
			sOrigin = String.Empty
			sTemp = String.Empty

		End If

	End Sub

	Private Sub frmUnidiv_Resize(oSsender As System.Object, e As EventArgs) Handles Me.Resize
		If midgvMainLocationY <> 0 Then
			Try
				Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmUnidiv - Resize")
			End Try
		End If
	End Sub


	Private Sub cmdInsertAreaTable_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertAreaTable.Click
		AppActivate(moAppWin.Text)
		Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'A1
		Dim oAreaTable As AreaTable = New AreaTable(enAreaTable.FDB)
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d


		Dim oRowParcel As UnidivNet.UD_Parcel = Nothing

		If zzSelectPoint(tPoint, True) = PromptStatus.OK Then
			oAreaTable.InsertHeader()
			For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values

			Next


		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub




	Private Sub cmdSelectRow_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectRow.Click
		If miCurrentActionFirstRowIndex <> -1 Then
			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Divide
					zzParcelFromCurrentRow(False, True, False)
				Case UnidivNet.enActionType.Union
					Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = Nothing
					If zzGetParcelColFromGrid(colParcels) Then
						zzOpenDWG()
						zzBeforeUnion(colParcels)
						zzCloseDWG()
					Else
						zzParcelFromCurrentRow(True, False, False)
					End If
				Case UnidivNet.enActionType.Transfer
					zzParcelFromCurrentRow(False, True, True)
			End Select
		End If
	End Sub
	Private Function zzSourceIsEmpty(oDataRow As DataRow) As Boolean
		Return IsDBNull(oDataRow.Item("OriginalParcelNo")) AndAlso IsDBNull(oDataRow.Item("NewParcelNo"))
	End Function
	Private Sub zzParcelFromCurrentRow(bNewRow As Boolean, bReplace As Boolean, bNewAction As Boolean)
		Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oDataRow As DataRow
		Dim oGridRow As System.Windows.Forms.DataGridViewRow
		Dim oDestCell As System.Windows.Forms.DataGridViewCell
		Dim tParcelKey As UD_ParcelKey
		Dim iParcelDbID As Integer
		Dim bUnique As Boolean
		Dim iNewRowIndex As Integer = moMainTable.Rows.Count
		Dim oParcel As UnidivNet.UD_Parcel = Nothing

		If oCurrentGridRow.Index < miCurrentActionFirstRowIndex Then
			tParcelKey = zzGetSourceParcelKey(oCurrentGridRow)
			iParcelDbID = zzGetSourceParcelDbID(oCurrentGridRow)

			If mdicParcels.TryGetValueByDbID(iParcelDbID, oParcel) Then
				If bNewRow Then
					bUnique = Not mhsSelectedParcels.Contains(oParcel)
				Else
					bUnique = True
				End If
				If bUnique Then

					oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex)
					If bNewRow AndAlso Not zzSourceIsEmpty(oDataRow) Then
						If bNewAction Then
							DMCommon.Debug.MsgBox("13_103S", miCurrentAction, iNewRowIndex, miCurrentActionFirstRowIndex)
							miCurrentAction += 1
						End If

						oDataRow = zzGetDataRow(iNewRowIndex, True, oParcel.DbID)
						oGridRow = Me.dgvMain.Rows.Item(iNewRowIndex)
						If False Then
							If miCurrentActionType = UnidivNet.enActionType.Transfer Then
								mtCurrentParcelKey.NextParcel()
								oDataRow.Item("DestParcelNo") = mtCurrentParcelKey.ParcelNo
								oDataRow.Item("DestBlockNo") = mtCurrentParcelKey.BlockNo
								If mtCurrentParcelKey.BlockAdd <> 0 Then
									oDataRow.Item("DestBlockAddNo") = mtCurrentParcelKey.BlockAdd
								End If

								oDataRow.Item("RowStatus") = enRowStatus.Transfer
							End If
						Else
							zzParcelBatch(oDataRow)
						End If

					Else
						oGridRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
					End If





					If tParcelKey.Original Then
						oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
						oDataRow.Item("NewParcelNo") = DBNull.Value
						oDestCell = oGridRow.Cells.Item("ctxFromParcel")

					Else
						oDataRow.Item("OriginalParcelNo") = DBNull.Value
						oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
						oDestCell = oGridRow.Cells.Item("ctxFromParcelTemp")
					End If
					oDataRow.Item("ParcelSourceDbID") = iParcelDbID

					''''''''''''''''''''''''''''''''''	Me.dgvMain.DataSource = moMainTable
					If miCurrentActionType = UnidivNet.enActionType.Union Then
						mhsSelectedParcels.Add(oParcel)
						mlstSelectedParcelKeys.Add(tParcelKey)
						mlstSelectedParcels.Add(oParcel)

						'	zzSelectedParcelKeysToGrid(True, True)
						zzSelectedParcelsToGrid(True, True)

						zzSetParcelCount(mhsSelectedParcels.Count)
					End If

					Try
						Me.dgvMain.CurrentCell = oDestCell
					Catch oEx As Exception

					End Try

					If bReplace Then

						DMAcadExt.AcadDocument.ClearDrawVectorSet()
					End If

					zzMarkParcel(oParcel, 0, True)
				End If
			Else
				DMCommon.Debug.ExcelLog.SetNextValue(0, "jj")

				DMCommon.Debug.ExcelLog.SetNextValue(0, "jj")



			End If

		End If

	End Sub
	Private Sub zzParcelBatch(oDataRow As DataRow)
		If miCurrentActionType = UnidivNet.enActionType.Transfer Then

			oDataRow.Item("DestParcelNo") = mtCurrentParcelKey.ParcelNo
			oDataRow.Item("DestBlockNo") = mtCurrentParcelKey.BlockNo
			If mtCurrentParcelKey.BlockAdd <> 0 Then
				oDataRow.Item("DestBlockAddNo") = mtCurrentParcelKey.BlockAdd
			End If

			oDataRow.Item("RowStatus") = enRowStatus.Transfer
			mtCurrentParcelKey.NextParcel()
		End If
	End Sub
	Private Sub zzDispSelectedParcels(sCaption As String)
		Dim sRes(mhsSelectedParcels.Count - 1) As System.Object
		Dim i As Integer
		For Each oParcel As UnidivNet.UD_Parcel In mhsSelectedParcels
			sRes(i) = oParcel.ParcelKey.ToString()
			i += 1
		Next
		DMCommon.Debug.MsgBox(sCaption, sRes)
	End Sub
	Private Sub zzParcelFromCurrentCell()
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(oCurrentCell.RowIndex)
		Dim oDataRow As DataRow
		Dim tParcelKey As UD_ParcelKey

		If oCurrentGridRow.Index < miCurrentActionFirstRowIndex Then
			tParcelKey = zzGetSourceParcelKey(oCurrentGridRow)
			'  DMCommon.Debug.MsgBox("12_020", tParcelKey.ToString())
			oDataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			If tParcelKey.Original Then
				oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
			Else
				oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
			End If

			Me.dgvMain.DataSource = moMainTable

		Else
			'  DMCommon.Debug.MsgBox("12_132", miCurrentActionFirstRowIndex, miCurrentActionType)
		End If

	End Sub



	Private Sub cmdGeneral_Click(oSender As System.Object, e As EventArgs) Handles cmdGeneral.Click
		Dim sNormalParcelsString As String = Nothing
		Dim sTempParcelsString As String = Nothing

		Dim bIsTTTG As Boolean = UnidivNet.UD_App.IsTTG

		zzGetParcelsString(sNormalParcelsString, sTempParcelsString)
		Me.Visible = False
		If mfUD_General Is Nothing OrElse mfUD_General.IsDisposed Then
			mfUD_General = New TopoUI.frmUD_General(True, miPlanID)
		End If

		mfUD_General.SetPlanType(bIsTTTG)
		'	DMCommon.Debug.MsgBox("08_932", miPlanID, sNormalParcelsString, sTempParcelsString)


		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
		'	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
		mfUD_General.Left = 40
		mfUD_General.Top = 40
	End Sub

	Private Sub mfUD_General_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfUD_General.FormClosed
		Me.Visible = True
	End Sub


	Private Sub dgvMain_CellValidating(oSender As System.Object, e As DataGridViewCellValidatingEventArgs) Handles dgvMain.CellValidating
		Dim oFirstActionRow As DataRow
		Dim oGridRow As DataGridViewRow
		Dim iParcelNo As Integer
		Dim iBlockNo As Integer
		Dim iBlockAddNo As Integer

		If mbEventsEnabled AndAlso mtEditingCellKey.Exists AndAlso miCurrentActionFirstRowIndex <> -1 AndAlso e.RowIndex >= miCurrentActionFirstRowIndex Then
			Select Case e.ColumnIndex
				Case miFromParcelColIndex, miFromParcelTempColIndex
					If e.FormattedValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(e.FormattedValue.ToString()) Then
						'Dim bRes As Boolean
						Dim oParcel As UnidivNet.UD_Parcel = Nothing
						Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(e.ColumnIndex, e.FormattedValue)

						If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
							DMCommon.Debug.MsgBox("12_211A", tParcelKey, miCurrentActionType, e.RowIndex - miCurrentActionFirstRowIndex)
							'''''''''''''   DMAcadExt.AcadDocument.DrawVectorSetInfo()
							If miCurrentActionType = UnidivNet.enActionType.Union Then
								If mhsSelectedParcels.Contains(oParcel) Then
									e.Cancel = True
								Else
									mhsSelectedParcels.Add(oParcel)
									mlstSelectedParcelKeys.Add(tParcelKey)
								End If
							End If

							oGridRow = Me.dgvMain.Rows.Item(e.RowIndex)
							mbEventsEnabled = False

							mbEventsEnabled = True

							zzMarkParcel(oParcel, e.RowIndex - miCurrentActionFirstRowIndex, True)
						End If
						DMCommon.Debug.MsgBox("12_110Z", tParcelKey.ToString(), e.RowIndex, e.ColumnIndex, e.Cancel, mdicParcels.ContainsKey(tParcelKey), e.FormattedValue.ToString(), e.FormattedValue.GetType().ToString())
					Else
						e.Cancel = True
					End If


				Case miToParcelColIndex
					oFirstActionRow = moMainTable.Rows.Item(e.RowIndex)
					iBlockNo = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockNo"))
					iBlockAddNo = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockAddNo"))
					iParcelNo = Convert.ToInt32(e.FormattedValue)
				Case miToGushColIndex
					oFirstActionRow = moMainTable.Rows.Item(e.RowIndex)
					iParcelNo = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
					iBlockNo = Convert.ToInt32(e.FormattedValue)
					iBlockAddNo = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockAddNo"))
			End Select
			If iParcelNo <> 0 AndAlso iBlockNo <> 0 Then

				Me.dgvMain.AllowUserToAddRows = True
				mtCurrentParcelKey = New UD_ParcelKey(iBlockNo, iBlockAddNo, iParcelNo, False)
				zzOpenActionBatch()
			End If


		End If
	End Sub
	Private Function zzTest() As String
		If moNewDataRow Is Nothing Then
			Return "Nothing"
		Else
			Return moNewDataRow.RowState.ToString()
		End If
	End Function
	Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
		If mbEventsEnabled Then

			mtEditingCellKey.Erase()
			Select Case e.ColumnIndex
				Case 22, 23
					Dim tParcelKey As UD_ParcelKey
					Dim oDataRow As DataRow = Nothing
					Dim oParcel As UnidivNet.UD_Parcel = Nothing
					If e.RowIndex < moMainTable.Rows.Count Then
						oDataRow = moMainTable.Rows.Item(e.RowIndex)
						tParcelKey = zzParcelKeyFrom(oDataRow)
					Else

					End If

					If tParcelKey.Exists AndAlso mdicParcels.TryGetValue(tParcelKey, oParcel) Then

						If miCurrentActionType = UnidivNet.enActionType.Union Then

							zzMarkParcel(oParcel, e.RowIndex - miCurrentActionFirstRowIndex, True)
							mhsSelectedParcels.Add(oParcel)
							mlstSelectedParcelKeys.Add(tParcelKey)

						End If


					End If

			End Select
			If miCurrentActionType = UnidivNet.enActionType.Union Then
				zzSelectedParcelsToGrid(True, True)
				zzSetParcelCount(mhsSelectedParcels.Count)
			End If

		End If

	End Sub


	Private Sub moMainTable_TableNewRow(oSender As System.Object, e As DataTableNewRowEventArgs) Handles moMainTable.TableNewRow

		If mbEventsEnabled Then
			moNewDataRow = e.Row
			zzAddDataRow(enRowStatus.Default, 7777, False, e.Row)
		End If

	End Sub



	Private Sub cmdRestoreCancelLink_Click(oSender As System.Object, e As EventArgs) Handles cmdRestoreCancelLink.Click
		zzRestoreCancelLinks(enSelectLinkType.Script)
	End Sub

	Private Sub zzRestoreCancelLinks(iSelectLinkType As enSelectLinkType)
		Dim colSelectedLinks As ObjectIdCollection = Nothing
		zzOpenDWG(True)
		mbEventsEnabled = False
		DMAcadExt.AcadTransaction.SetLayersOn(moUD_PLineLayerList, False)


		Me.chkHanitView.Checked = False
		mbEventsEnabled = True
		If iSelectLinkType = enSelectLinkType.SelectionSet Then
			colSelectedLinks = zzGetCancelLinksBySelSet()
		ElseIf iSelectLinkType = enSelectLinkType.SelectLinksByFragments Then
			Dim colAllCancelledLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(msLinkCancelledLayer)
			colSelectedLinks = zzGetLinkSet(colAllCancelledLinks, iSelectLinkType)
		ElseIf iSelectLinkType = enSelectLinkType.Script Then
			Dim colAllCancelledLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(msLinkCancelledLayer)
			colSelectedLinks = zzGetLinkSet(colAllCancelledLinks, enSelectLinkType.Script)

		End If
		If colSelectedLinks IsNot Nothing AndAlso colSelectedLinks.Count > 0 Then
			zzChangeLayerToNew(colSelectedLinks)
			zzWriteLinkListScript(True, colSelectedLinks)
			DMAcadExt.AcadDocument.WriteMessage("Total Links: " & colSelectedLinks.Count.ToString())
		End If


		zzCloseDWG()
	End Sub

	Private Sub zzSelectUserLinks(iSelectLinkType As enSelectLinkType)
		Dim colSelectedLinks As ObjectIdCollection = Nothing
		zzOpenDWG(True)
		mbEventsEnabled = False
		DMAcadExt.AcadTransaction.SetLayersOn(moUD_PLineLayerList, False)

		Me.chkHanitView.Checked = False
		mbEventsEnabled = True
		If iSelectLinkType = enSelectLinkType.SelectionSet Then
			colSelectedLinks = zzGetUnionLinksBySelSet()
		ElseIf iSelectLinkType = enSelectLinkType.SelectLinksByFragments Then
			Dim colAllCancelledLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(msLinkCancelledLayer)
			colSelectedLinks = zzGetLinkSet(colAllCancelledLinks, iSelectLinkType)
		ElseIf iSelectLinkType = enSelectLinkType.Script Then
			Dim colAllCancelledLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(msLinkCancelledLayer)
			colSelectedLinks = zzGetLinkSet(colAllCancelledLinks, enSelectLinkType.Script)

		End If
		If colSelectedLinks IsNot Nothing AndAlso colSelectedLinks.Count > 0 Then
			mcolBlockedLinks = colSelectedLinks
			DMAcadExt.AcadDocument.WriteMessage("Total Frozen Links: " & colSelectedLinks.Count.ToString())
			Me.chkSelectUser.Checked = False
		End If


		zzCloseDWG()
	End Sub

	Private Sub zzChangeLayerToNew(colLinks As ObjectIdCollection)
		DMAcadExt.AcadTransaction.SetLayer(colLinks, msLinkNewLayer, True)
		moFragmentsToposcheme.SetLayer(colLinks, msLinkNewLayer)
	End Sub
	Private Function zzGetLinksBySelSet(sFilterList As String) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim iIndex As Integer = -1
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, sFilterList)} ' msLinkCancelledLayer
		Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter = New SelectionFilter(oaValues)
		DMAcadExt.AcadDocument.Regen()
		oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		oPromptOpt.AllowDuplicates = False
		Me.Visible = False
		AppActivate(moAppWin.Text)
		Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

		ptRes = oEditor.GetSelection(oPromptOpt, oFilter)

		If ptRes.Value IsNot Nothing Then
			For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
				tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId

				colSelected.Add(tAcObjID)
			Next
		End If
		Dim sException As String = String.Empty

		If ptRes.Status = PromptStatus.Error Then
			Dim taTypedValue() As TypedValue = oFilter.GetFilter()
			For iValueIndex As Integer = 0 To taTypedValue.GetUpperBound(0)
				sException &= taTypedValue(iValueIndex).Value.ToString()
			Next

		End If
		Me.Visible = True
		Return colSelected
	End Function
	Private Function zzGetCancelLinksBySelSet() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Return zzGetLinksBySelSet(msLinkCancelledLayer)
	End Function
	Private Function zzGetUnionLinksBySelSet() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
		Return zzGetLinksBySelSet(msLinkGushLayer & "," & msLinkParcelLayer & "," & msLinkNewLayer & "," & msLinkBridgeLayer & "," & msLinkNewBridgeLayer)

	End Function

	Private Sub dgvMain_DoubleClick(oSender As System.Object, e As EventArgs) Handles dgvMain.DoubleClick
		If mbEventsEnabled Then
			If miCurrentActionFirstRowIndex >= 0 Then
				Select Case miCurrentActionType
					Case UnidivNet.enActionType.Divide
						zzParcelFromCurrentRow(False, True, False)

					Case UnidivNet.enActionType.Union
						zzParcelFromCurrentRow(True, False, False)

						Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = Nothing

					Case UnidivNet.enActionType.Transfer
						If dgvMain.AllowUserToAddRows Then
							zzParcelFromCurrentRow(True, False, True)
						Else
							zzParcelFromCurrentRow(False, True, False)
						End If

				End Select
			End If

		End If
	End Sub
	Private Sub zzSetSourceParcel(oSourceParcel As UnidivNet.UD_Parcel, bArea As Boolean)
		If miCurrentActionFirstRowIndex >= 0 Then
			Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
			Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
			zzFillFromParcelKey(oSourceParcel, oFirstActionGridRow)
			If bArea Then
				zzFillParcelArea(oSourceParcel.ParcelArea, oFirstActionRow, oFirstActionGridRow)
				oFirstActionRow.Item("AcObjID") = oSourceParcel.CentroidAcObjID
			End If
		End If

	End Sub
	Private Sub cmdSelectCentroids_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectCentroids.Click
		If miCurrentActionFirstRowIndex <> -1 Then
			zzOpenDWG()

			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Select Case miCurrentActionType
				Case UnidivNet.enActionType.Divide
					zzActiveParcelsView(moUD_ParcelLayerListPlusNew)
					If Me.chkAddToSelect.Checked Then
						zzSelectParcelsByCentroid(False)
					Else
						Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(True, "C1603_*", zzGetActiveParcelCentroids(), 10)
						If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
							If mdicParcels.TryGetValueByObjID(colCentroidIds.Item(0), oParcel) Then
								zzSetSourceParcel(oParcel, False)
								DMAcadExt.AcadDocument.ClearDrawVectorSet()

								zzMarkParcel(oParcel, 0, False)
							End If
						End If
					End If

				Case UnidivNet.enActionType.Union
					zzActiveParcelsView(moUD_ParcelLayerList)
					zzSelectParcelsByCentroid(True)
				Case UnidivNet.enActionType.Union
					Dim oSelectedEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(zzGetSelected(), 51)
					Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(False, "C1603_*", zzGetActiveParcelCentroids(), 10)
					Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

					If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
						For Each tCentroidID As ObjectId In colCentroidIds
							If mdicParcels.TryGetValueByObjID(tCentroidID, oParcel) Then
								zzMarkParcel(oParcel, -1, False)

								If Not mhsSelectedParcels.Contains(oParcel) Then
									mhsSelectedParcels.Add(oParcel)
									mlstSelectedParcelKeys.Add(oParcel.ParcelKey)

									colParcels.Add(oParcel)
								End If
							End If
						Next
						zzSelectedParcelsToGrid(True, False)
						zzSetParcelCount(mhsSelectedParcels.Count)
					End If
					oSelectedEntitySet.RestoreColor()
				Case UnidivNet.enActionType.Transfer
					Dim bSingleOnly As Boolean = Not dgvMain.AllowUserToAddRows
					If bSingleOnly Then
						Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(True, "C1603_*", zzGetActiveParcelCentroids(), 10)
						If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
							If mdicParcels.TryGetValueByObjID(colCentroidIds.Item(0), oParcel) Then
								zzSetSourceParcel(oParcel, False)
								'''''''''''''''''''''''''''''''''''''''''''''''''''''''	zzTransferFromFinalTopology(oParcel.ParcelKey)
							End If
						End If
					Else
						zzActiveParcelsView(moUD_ParcelLayerList)
						zzSelectParcelsByCentroid(True)
					End If

			End Select
			zzRestoreView()
			zzCloseDWG()

		End If
	End Sub
	Private Function zzGetCentroidLayerList() As ICollection(Of String)
		Dim colLayers As ICollection(Of String) = New System.Collections.ObjectModel.Collection(Of String)
		For iStageIndex As Integer = 0 To miCurrentStage
			colLayers.Add(UnidivNet.UD_App.GetStageParcelCentroidLayer(iStageIndex))
		Next
		Return colLayers
	End Function
	Private Function zzGetActiveParcelCentroids() As ObjectIdCollection
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
			If Not oParcel.IsCanceled AndAlso Not mhsSelectedParcels.Contains(oParcel) Then
				colCentroids.Add(oParcel.CentroidAcObjID)
			End If
		Next
		Return colCentroids
	End Function

	Private Sub zzClearParcelList()
		Dim oaParcels(mdicParcels.Count - 1) As UnidivNet.UD_Parcel
		mdicParcels.Values.CopyTo(oaParcels, 0)
		For Each oParcel As UnidivNet.UD_Parcel In oaParcels
			If oParcel.Stage = 0 Then
				oParcel.IsCanceled = False
				oParcel.UpdateBlockAttributes()
			Else
				mdicParcels.RemoveParcel(oParcel)
				mdicFragments.DeleteParcel(oParcel)
			End If
		Next
	End Sub
	Private Sub zzResetVariables()
		miCurrentAction = 0
		miCurrentActionFirstRowIndex = -1
		miCurrentActionType = UnidivNet.enActionType.Registered
		miCurrentCentroidStatus = enCentroidStatus.Default
		miCurrentStage = 0
		mhsSelectedParcels.Clear()
		mlstSelectedParcelKeys.Clear()
		zzAllowUserToAddDelete(False)
	End Sub
	Private Sub zzAllowUserToAddDelete(bValue As Boolean)
		If Me.dgvMain.AllowUserToAddRows <> bValue Then
			Me.dgvMain.AllowUserToAddRows = bValue
			Me.dgvMain.AllowUserToDeleteRows = bValue

		End If
	End Sub
	Private Sub cmdEraseAllStages_Click(oSender As System.Object, e As EventArgs) Handles cmdEraseAllStages.Click
		mbEventsEnabled = False
		zzOpenDWG()
		zzDeleteFinalTopology()
		zzDeleteAllStageTopos()
		zzClearAllStageLayers()
		zzChangeNewPointStageLayers()
		zzClearParcelList()
		zzClearJournalTable()
		zzResetVariables()
		zzSetActionTypeFrame()
		zzCloseDWG()
		mbEventsEnabled = True
	End Sub

	Private Function mdicRows() As System.Object
		Throw New NotImplementedException
	End Function

	Private Sub cmdRecalc_Click(oSender As System.Object, e As EventArgs) Handles cmdRecalc.Click
		'	zzRecalcDivideResParcels()
		If miCurrentActionFirstRowIndex = -1 Then
			zzRecalcDivideAllResParcels()
		End If

	End Sub


	Private Sub dgvMain_UserDeletingRow(oSender As System.Object, e As DataGridViewRowCancelEventArgs) Handles dgvMain.UserDeletingRow

		If miCurrentActionFirstRowIndex = -1 OrElse e.Row.Index <= miCurrentActionFirstRowIndex Then
			e.Cancel = True
		ElseIf miCurrentActionType <> UnidivNet.enActionType.Union Then
			e.Cancel = True
		Else
			Dim oDeletingDataRow As DataRow = moMainTable.Rows.Item(e.Row.Index)
			Dim oDataRow As DataRow
			Dim iDeletingOperNumber As Integer = DMCommon.Functions.CIntN(oDeletingDataRow.Item("Oper"), 0)

			oDeletingDataRow.Item("Oper") = 9999
			For iRowIndex As Integer = e.Row.Index + 1 To moMainTable.Rows.Count - 1
				oDataRow = moMainTable.Rows.Item(iRowIndex)
				Try
					oDataRow.Item("Oper") = iDeletingOperNumber + iRowIndex - (e.Row.Index + 1)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("Err #311", oEx.Message, iRowIndex, iDeletingOperNumber + iRowIndex - (e.Row.Index + 1))
				End Try

			Next
			Dim oDeletingParcel As UnidivNet.UD_Parcel = zzParcelFrom(oDeletingDataRow)

			If mhsSelectedParcels.Contains(oDeletingParcel) Then
				mhsSelectedParcels.Remove(oDeletingParcel)
				mlstSelectedParcels.Remove(oDeletingParcel)
				zzSetParcelCount(mhsSelectedParcels.Count)
			End If
			DMCommon.Debug.MsgBox("110619_1", e.Row.Index, iDeletingOperNumber, mhsSelectedParcels.Count, mlstSelectedParcels.Count)
		End If

	End Sub
	Private Sub chkActiveParcels_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkActiveParcels.CheckedChanged

		zzOpenDWG()
		If chkActiveParcels.Checked Then
			zzActiveParcelsView(moUD_ParcelLayerList)
		Else
			zzRestoreView()
		End If
		zzCloseDWG()
	End Sub

	Private Sub chkPointsBlocking_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkPointsBlocking.CheckedChanged
		TopoManager.TopoScheme.tsNode.IsGeoVertex = Me.chkPointsBlocking.Checked
	End Sub

	Private Sub cmdExit_Click(oSender As System.Object, e As System.EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Sub cmdBlockBorder_Click_140218(oSender As System.Object, e As EventArgs)
		zzOpenDWG()
		Dim oFinalTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(UnidivNet.UD_App.FinalTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oFinalTopoModel IsNot Nothing Then
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oFinalTopoModel.GetPolygons()
			Dim dicPolygons As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Dim colNewBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colCancelledBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colBlockedBlockBorder As ObjectIdCollection = New ObjectIdCollection()


			For Each oPolygon As Polygon In colParcelPgons
				If mdicParcels.TryGetValueByObjID(oPolygon.Entity, oParcel) Then
					dicPolygons.Add(oPolygon.ID, oParcel.FinalBlockKey)
				End If
			Next

			Dim colEdges As FullEdgeCollection = oFinalTopoModel.GetFullEdges()
			Dim iPgonLeftID, iPgonRightID, iPgonInnerID As Integer
			Dim iBlockKeyLeft, iBlockKeyRight, iBlockKeyInner As Integer
			Dim bIsBlockBorder As Boolean
			Dim bIsBlockedBlockBorder As Boolean

			For Each oFullEdge As FullEdge In colEdges
				iPgonLeftID = zzGetPolygon(oFullEdge, True)
				iPgonRightID = zzGetPolygon(oFullEdge, False)

				If iPgonLeftID = 0 OrElse iPgonRightID = 0 Then
					If iPgonLeftID = 0 Then
						iPgonInnerID = iPgonRightID
					Else
						iPgonInnerID = iPgonLeftID
					End If
					If zzIsSourceBlockBorder(oFullEdge.Entity, bIsBlockBorder, bIsBlockedBlockBorder) AndAlso dicPolygons.TryGetValue(iPgonInnerID, iBlockKeyInner) Then
						If bIsBlockBorder Then
							If dicPolygons.TryGetValue(iPgonInnerID, iBlockKeyInner) Then
								If iBlockKeyInner <> miBlockKey Then
									If bIsBlockedBlockBorder Then
										colBlockedBlockBorder.Add(oFullEdge.Entity)
									Else
										colCancelledBlockBorder.Add(oFullEdge.Entity)
									End If
								End If
							End If
						Else
							If iBlockKeyInner <> miBlockKey Then
								colNewBlockBorder.Add(oFullEdge.Entity)
							End If
						End If
					End If
				Else
					If dicPolygons.TryGetValue(iPgonLeftID, iBlockKeyLeft) AndAlso dicPolygons.TryGetValue(iPgonRightID, iBlockKeyRight) Then
						If iBlockKeyLeft <> iBlockKeyRight Then
							colNewBlockBorder.Add(oFullEdge.Entity)
						End If
					End If
				End If

			Next
			zzMarkNewBlockBorder(colNewBlockBorder)
			zzMarkCancelledBlockBorder(colCancelledBlockBorder)
			zzMarkBlockingBlockBorder(colBlockedBlockBorder)

			oFinalTopoModel.Close()
			DMCommon.Debug.MsgBox("12_730", colNewBlockBorder.Count, colCancelledBlockBorder.Count, colBlockedBlockBorder.Count)

		End If


		zzCloseDWG()
	End Sub
	Private Sub cmdBlockBorder_Click_Ver1(oSender As System.Object, e As EventArgs)
		zzOpenDWG()
		Dim oFinalTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(UnidivNet.UD_App.FinalTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oFinalTopoModel IsNot Nothing Then
			Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oFinalTopoModel.GetPolygons()
			Dim dicPolygons As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim colRings As RingCollection = Nothing
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray = New TopoManager.GeoUtilites.BulgeVertexArray()

			Dim colNewBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colCancelledBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colBlockedBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			' Dim colMovedParcels As System.Collections.ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
			Dim colMovedParcels As System.Collections.ObjectModel.Collection(Of Polygon) = New ObjectModel.Collection(Of Polygon)()
			Dim colHalfEdges As HalfEdgeCollection
			Dim oFullEdge As FullEdge = Nothing
			Dim iMyPolygonID As Integer, iMyBlockKey As Integer
			Dim tAcObjID As ObjectId
			Dim iPrevBlockEdgeType As enBlockEdgeType = enBlockEdgeType.Default
			Dim iBlockEdgeType As enBlockEdgeType
			Dim oNode As Node
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim dDistParam As Double = 14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor
			Dim oResList As List(Of TopoManager.InitInsertData) = New List(Of TopoManager.InitInsertData)()
			For Each oPolygon As Polygon In colParcelPgons
				If mdicParcels.TryGetValueByObjID(oPolygon.Entity, oParcel) Then
					dicPolygons.Add(oPolygon.ID, oParcel.FinalBlockKey)
					If oParcel.IsMoved Then
						colMovedParcels.Add(oPolygon)
					End If
				End If
			Next

			For Each oPolygon As Polygon In colMovedParcels
				iMyPolygonID = oPolygon.ID
				If dicPolygons.TryGetValue(iMyPolygonID, iMyBlockKey) Then
					colRings = oPolygon.GetBoundary()
					For Each oRing As Ring In colRings
						colHalfEdges = oRing.GetEdges()
						oNode = Nothing
						For Each oHalfEdge As HalfEdge In colHalfEdges
							If oNode Is Nothing Then
								oNode = oHalfEdge.PreviousNode
								tPoint = DMAcadExt.TPlnPoint.Point3dTo2d(oNode.Location)
							End If
							oFullEdge = oHalfEdge.FullEdge
							tAcObjID = oFullEdge.Entity
							iBlockEdgeType = zzGetFullEdgeType(iMyPolygonID, iMyBlockKey, oFullEdge, dicPolygons)
							Select Case iBlockEdgeType
								Case enBlockEdgeType.NewBlockBorder
									colNewBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.CancelledBlockBorder
									colCancelledBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.BlockedBlockBorder
									colBlockedBlockBorder.Add(tAcObjID)
							End Select
							If iPrevBlockEdgeType <> iBlockEdgeType Then
								If oBulgeVertexArray IsNot Nothing Then
									oResList.AddRange(oBulgeVertexArray.GetMarkPoints(dDistParam))
								End If
								oBulgeVertexArray = New TopoManager.GeoUtilites.BulgeVertexArray()
							Else

							End If

							oBulgeVertexArray.AddCurve(tAcObjID, tPoint, 0)
						Next
						If oBulgeVertexArray IsNot Nothing Then
							oResList.AddRange(oBulgeVertexArray.GetMarkPoints(dDistParam))
						End If
					Next
				End If
			Next
			zzMarkNewBlockBorder(colNewBlockBorder)
			zzMarkCancelledBlockBorder(colCancelledBlockBorder)
			zzMarkBlockingBlockBorder(colBlockedBlockBorder)

			oFinalTopoModel.Close()
			DMCommon.Debug.MsgBox("12_730", colNewBlockBorder.Count, colCancelledBlockBorder.Count, colBlockedBlockBorder.Count)

		End If

		zzCloseDWG()
	End Sub
	Private Sub cmdBlockBorder_Click_020518(oSender As System.Object, e As EventArgs) 'Handles cmdBlockBorder.Click
		zzOpenDWG()
		Dim oFinalTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(UnidivNet.UD_App.FinalTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oFinalTopoModel IsNot Nothing Then
			Dim colFinalParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oFinalTopoModel.GetPolygons()
			Dim dicPolygons As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim colRings As RingCollection = Nothing
			Dim oParcel As UnidivNet.UD_Parcel = Nothing
			Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray = New TopoManager.GeoUtilites.BulgeVertexArray()

			Dim colNewBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colCancelledBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colBlockedBlockBorder As ObjectIdCollection = New ObjectIdCollection()
			Dim colMovedParcels As System.Collections.ObjectModel.Collection(Of Polygon) = New ObjectModel.Collection(Of Polygon)()
			Dim colHalfEdges As HalfEdgeCollection
			Dim oFullEdge As FullEdge = Nothing
			Dim iThisPolygonID As Integer, iThisBlockKey As Integer
			Dim tAcObjID As ObjectId
			Dim iPrevBlockEdgeType As enBlockEdgeType = enBlockEdgeType.Default
			Dim iBlockEdgeType As enBlockEdgeType
			Dim dDistParam As Double = 14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor
			Dim oResList As List(Of TopoManager.InitInsertData) = New List(Of TopoManager.InitInsertData)()
			For Each oPolygon As Polygon In colFinalParcelPgons
				If mdicParcels.TryGetValueByObjID(oPolygon.Entity, oParcel) Then
					dicPolygons.Add(oPolygon.ID, oParcel.FinalBlockKey)
					If oParcel.IsMoved Then
						colMovedParcels.Add(oPolygon)
					End If
				Else
					DMCommon.Debug.MsgBox("Err #1204o", mdicParcels.Count, mdicParcels.AcObjIDsCount, colFinalParcelPgons.Count, oPolygon.Entity)
				End If
			Next
			For Each oPolygon As Polygon In colMovedParcels
				iThisPolygonID = oPolygon.ID
				If dicPolygons.TryGetValue(iThisPolygonID, iThisBlockKey) Then
					colRings = oPolygon.GetBoundary()
					For Each oRing As Ring In colRings
						colHalfEdges = oRing.GetEdges()

						For Each oHalfEdge As HalfEdge In colHalfEdges

							oFullEdge = oHalfEdge.FullEdge
							tAcObjID = oFullEdge.Entity
							iBlockEdgeType = zzGetFullEdgeType(iThisPolygonID, iThisBlockKey, oFullEdge, dicPolygons)
							Select Case iBlockEdgeType
								Case enBlockEdgeType.NewBlockBorder
									colNewBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.CancelledBlockBorder
									colCancelledBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.BlockedBlockBorder
									colBlockedBlockBorder.Add(tAcObjID)
							End Select
						Next
					Next
				End If

			Next
			zzMarkNewBlockBorder(colNewBlockBorder)
			zzMarkCancelledBlockBorder(colCancelledBlockBorder)
			zzMarkBlockingBlockBorder(colBlockedBlockBorder)
			oFinalTopoModel.Close()
		End If

		zzCloseDWG()
	End Sub
	Private Sub cmdBlockBorder_Click(oSender As System.Object, e As EventArgs) Handles cmdBlockBorder.Click

		Me.Cursor = Cursors.WaitCursor
		zzOpenDWG()
		Dim sTopoName As String
		If miPlanType = 12 Then
			sTopoName = msParcelTopoName
		Else
			sTopoName = UnidivNet.UD_App.FinalTopoName
		End If
		Dim oFinalTopoScheme As tsTopology = New tsTopology(sTopoName)
		oFinalTopoScheme.Load(False)


		Dim dicPolygons As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
		Dim colRings As RingCollection = Nothing
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim oBulgeVertexArray As TopoManager.GeoUtilites.BulgeVertexArray = New TopoManager.GeoUtilites.BulgeVertexArray()

		Dim colNewBlockBorder As ObjectIdCollection = New ObjectIdCollection()
		Dim colCancelledBlockBorder As ObjectIdCollection = New ObjectIdCollection()
		Dim colBlockedBlockBorder As ObjectIdCollection = New ObjectIdCollection()
		Dim colMovedParcels As System.Collections.ObjectModel.Collection(Of tsPolygon) = New ObjectModel.Collection(Of tsPolygon)()

		Dim oFullEdge As FullEdge = Nothing
		Dim iThisPolygonID As Integer, iThisBlockKey As Integer
		Dim tAcObjID As ObjectId
		Dim iPrevBlockEdgeType As enBlockEdgeType = enBlockEdgeType.Default
		Dim iBlockEdgeType As enBlockEdgeType
		Dim oRing As tsRing
		Dim oBranch As tsBranch
		Dim dDistParam As Double = 14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor
		Dim oResList As List(Of TopoManager.InitInsertData) = New List(Of TopoManager.InitInsertData)()
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!BlockBorderA", sTopoName, oFinalTopoScheme.Polygons.Count, mdicParcels.Count)
		For Each oPolygonScheme As tsPolygon In oFinalTopoScheme.Polygons
			If mdicParcels.TryGetValueByObjID(oPolygonScheme.AcObjID, oParcel) Then
				dicPolygons.Add(oPolygonScheme.ID, oParcel.FinalBlockKey)
				If oParcel.IsMoved Then
					colMovedParcels.Add(oPolygonScheme)
				End If
			Else
				DMCommon.Debug.MsgBox("Err #1204", mdicParcels.Count, mdicParcels.AcObjIDsCount, oPolygonScheme.AcObjID)
			End If
		Next
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!BlockBorderB", colMovedParcels.Count)
		For Each oPolygon As tsPolygon In colMovedParcels
			iThisPolygonID = oPolygon.ID
			If dicPolygons.TryGetValue(iThisPolygonID, iThisBlockKey) Then

				For iRingIndex As Integer = 0 To oPolygon.RingsUB
					oRing = oPolygon.Rings(iRingIndex)

					For Each iBranchID As Integer In oRing.BranchSet
						oBranch = oFinalTopoScheme.GetBranch(iBranchID)
						If oBranch IsNot Nothing Then
							tAcObjID = oBranch.AcObjID

							iBlockEdgeType = zzGetFullEdgeType(iThisPolygonID, iThisBlockKey, oBranch, dicPolygons)
							Select Case iBlockEdgeType
								Case enBlockEdgeType.NewBlockBorder
									colNewBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.CancelledBlockBorder
									colCancelledBlockBorder.Add(tAcObjID)
								Case enBlockEdgeType.BlockedBlockBorder
									colBlockedBlockBorder.Add(tAcObjID)
							End Select

						End If
					Next
				Next
			End If
		Next
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!BlockBorderC", colNewBlockBorder.Count, colCancelledBlockBorder.Count, colBlockedBlockBorder.Count)
		zzMarkNewBlockBorder(colNewBlockBorder)
		zzMarkCancelledBlockBorder(colCancelledBlockBorder)
		zzMarkBlockingBlockBorder(colBlockedBlockBorder)

		oFinalTopoScheme.RefreshBranches()


		Dim dicBlocks As IDictionary(Of String, DMAcadExt.AcadBlock) = New Dictionary(Of String, DMAcadExt.AcadBlock)
		Dim oAcadBlock As DMAcadExt.AcadBlock

		oAcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.MarkGushBlockName, UnidivNet.UD_App.BlockPath13)
		oAcadBlock.OpenForRight()
		dicBlocks.Add(msLinkGushLayer, oAcadBlock)

		oAcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.MarkGushCancelledBlockName, UnidivNet.UD_App.BlockPath13)
		oAcadBlock.OpenForRight()
		dicBlocks.Add(msLinkGushCancelledLayer, oAcadBlock)

		oAcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.MarkNewGushBlockName, UnidivNet.UD_App.BlockPath13)
		oAcadBlock.OpenForRight()
		dicBlocks.Add(msLinkNewGushLayer, oAcadBlock)



		oFinalTopoScheme.LayerFilter = New DMCommon.dmList(New String() {msLinkGushLayer, msLinkGushCancelledLayer, msLinkNewGushLayer})
		oResList = oFinalTopoScheme.GetMarkPoints(10.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor) '14.0
		DMAcadExt.AcadDocument.WriteDebugMessageN("oResList", oResList.Count, dicBlocks.Count)

		'  TopoManager.GeoUtilites.InsertMarkBlock(oResList, sMarkBlockName, sMarkBlockFolder)
		TopoManager.GeoUtilites.InsertMarkBlock(oResList, dicBlocks)
		DMAcadExt.AcadTransaction.SetLayer(colBlockedBlockBorder, msLinkGushLayer)

		zzCloseDWG()
		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzGetFullEdgeType(iThisPolygonID As Integer, iThisBlockKey As Integer, oBranch As tsBranch, ByRef dicPolygons As Dictionary(Of Integer, Integer)) As enBlockEdgeType
		Dim iOtherPgonID As Integer
		Dim iOtherBlockKey As Integer
		Dim bIsBlockBorder As Boolean
		Dim bIsBlockedBlockBorder As Boolean

		iOtherPgonID = oBranch.GetOtherPolygon(iThisPolygonID)
		If iOtherPgonID = 0 Then
			If zzIsSourceBlockBorder(oBranch.AcObjID, bIsBlockBorder, bIsBlockedBlockBorder) Then
				If bIsBlockBorder Then
					If bIsBlockedBlockBorder Then
						Return enBlockEdgeType.BlockedBlockBorder
					Else
						Return enBlockEdgeType.CancelledBlockBorder
					End If
				Else
					Return enBlockEdgeType.NewBlockBorder
				End If
			End If
		ElseIf dicPolygons.TryGetValue(iOtherPgonID, iOtherBlockKey) Then
			If iThisBlockKey = iOtherBlockKey Then
				Return enBlockEdgeType.Default
			Else
				Return enBlockEdgeType.NewBlockBorder
			End If
		End If
	End Function


	Private Function zzGetFullEdgeType(iThisPolygonID As Integer, iThisBlockKey As Integer, oFullEdge As FullEdge, ByRef dicPolygons As Dictionary(Of Integer, Integer)) As enBlockEdgeType
		Dim iOtherPgonID As Integer
		Dim iOtherBlockKey As Integer
		Dim bIsBlockBorder As Boolean
		Dim bIsBlockedBlockBorder As Boolean

		iOtherPgonID = zzGetOtherPolygon(oFullEdge, iThisPolygonID)
		If iOtherPgonID = 0 Then
			If zzIsSourceBlockBorder(oFullEdge.Entity, bIsBlockBorder, bIsBlockedBlockBorder) Then
				If bIsBlockBorder Then
					If bIsBlockedBlockBorder Then
						Return enBlockEdgeType.BlockedBlockBorder
					Else
						Return enBlockEdgeType.CancelledBlockBorder
					End If
				Else
					Return enBlockEdgeType.NewBlockBorder
				End If
			End If
		ElseIf dicPolygons.TryGetValue(iOtherPgonID, iOtherBlockKey) Then
			If iThisBlockKey = iOtherBlockKey Then
				Return enBlockEdgeType.Default
			Else
				Return enBlockEdgeType.NewBlockBorder
			End If
		End If

	End Function
	Private Function zzGetPolygon(oFullEdge As FullEdge, bOnLeft As Boolean) As Integer
		Dim oPgon As Polygon
		Try
			oPgon = oFullEdge.GetPolygon(bOnLeft)
			Return oPgon.ID
		Catch oMapEx As Autodesk.Gis.Map.MapException
			Return 0
		End Try

	End Function
	Private Function zzGetOtherPolygon(oFullEdge As FullEdge, iMyPolygon As Integer) As Integer
		Dim iPgonLeftID, iPgonRightID As Integer

		iPgonLeftID = zzGetPolygon(oFullEdge, True)
		iPgonRightID = zzGetPolygon(oFullEdge, False)
		If iMyPolygon = iPgonLeftID Then
			Return iPgonRightID
		ElseIf iMyPolygon = iPgonRightID Then
			Return iPgonLeftID
		Else
			Return -1
		End If
	End Function

	Private Function zzIsSourceBlockBorder(tLinkAcObjID As ObjectId, ByRef bIsBlockBorder As Boolean, ByRef bIsBlockedBlockBorder As Boolean) As Boolean
		Dim oEntity As Entity
		oEntity = DMAcadExt.AcadTransaction.GetEntity(tLinkAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
		Select Case oEntity.Layer
			Case msLinkGushLayer
				bIsBlockBorder = True
				bIsBlockedBlockBorder = False
				Return True
			Case msLinkGushBlockedLayer
				bIsBlockBorder = True
				bIsBlockedBlockBorder = True
				Return True

			Case msLinkParcelLayer, msLinkNewLayer, msLinkNewBridgeLayer
				bIsBlockBorder = False
				Return True
			Case Else
				DMCommon.Debug.MsgBox("Err #1271", oEntity.Layer, oEntity.Handle)
				Return False
		End Select
	End Function
	Private Sub zzMarkNewBlockBorder(colNewBlockBorder As ObjectIdCollection)
		zzMarkObjIDCollection(colNewBlockBorder, msLinkNewGushLayer)
	End Sub
	Private Sub zzMarkCancelledBlockBorder(colCancelledBlockBorder As ObjectIdCollection)
		'   DMCommon.Debug.MsgBox("12_803", colCancelledBlockBorder.Count, msLinkGushCancelledLayer)
		zzMarkObjIDCollection(colCancelledBlockBorder, msLinkGushCancelledLayer)
	End Sub
	Private Sub zzMarkBlockingBlockBorder(colBlockingBlockBorder As ObjectIdCollection)
		zzMarkObjIDCollection(colBlockingBlockBorder, msLinkGushLayer)
	End Sub
	Private Sub zzMarkObjIDCollection(colObjectIDs As ObjectIdCollection, sNewLayer As String)
		If colObjectIDs.Count <> 0 AndAlso DMAcadExt.AcadTransaction.CreateLayer(sNewLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, False) Then
			zzSetLayer(colObjectIDs, sNewLayer)
		End If
	End Sub

	Private Sub chkHanitView_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkHanitView.CheckedChanged
		If mbEventsEnabled Then
			zzOpenDWG()
			DMAcadExt.AcadTransaction.SetLayersOn(moUD_PLineLayerList, chkHanitView.Checked)
			zzCloseDWG()
		End If

	End Sub

	Private Sub chkDataBound_Validating(oSender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles chkDataBound.Validating
		e.Cancel = True
	End Sub



	Private Sub cmdZoom_Click(oSender As System.Object, e As EventArgs) Handles cmdZoom.Click

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.CurrentRow

		Dim tParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		tParcelKey = zzGetSourceParcelKey(oCurrentGridRow)

		If mdicParcels.TryGetValue(tParcelKey, oParcel) Then

			If Not oParcel.BorderAcObjID.IsNull Then
				DMAcadExt.AcadDocument.Regen()
				DMAcadExt.AcadDocument.Zoom(oParcel.BorderAcObjID, 2.0)
				DMAcadExt.AcadTransaction.Highlight(oParcel.BorderAcObjID)
				DMAcadExt.AcadDocument.DrawShape(DMAcadExt.MarkBlock.enMarkBlockType.Square, 4.0 * mtScale3d.X, oParcel.CenterPosition, 1, True)
			End If
		End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub


	Private Sub zzShowAllRows()

		For Each oGridRow As DataGridViewRow In Me.dgvMain.Rows
			oGridRow.Visible = True
		Next
	End Sub
	Private Sub zzViewAllRows()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow = Nothing
		Dim oParcel As UnidivNet.UD_Parcel = Nothing

		For iRowIndex As Integer = 0 To moMainTable.Rows.Count - 1
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
		Next
	End Sub
	Private Sub zzHideCanceledRows()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRow As DataRow = Nothing
		Dim tSourceParcelKey As UD_ParcelKey
		Dim oParcel As UnidivNet.UD_Parcel = Nothing
		Dim iActionType As UnidivNet.enActionType



		For iRowIndex As Integer = 0 To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
			iActionType = zzToActionType(oDataRow)

			tSourceParcelKey = zzGetRowParcelKey(iActionType, oDataRow)
			If mdicParcels.TryGetValue(tSourceParcelKey, oParcel) Then
				oGridRow.Visible = Not oParcel.IsCanceled
			End If
			miLastRowIndex = iRowIndex
		Next
	End Sub


	Private Sub chkHideCanceledRows_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkHideCanceledRows.CheckedChanged
		If mbEventsEnabled Then
			If Me.chkHideCanceledRows.Checked Then
				zzHideCanceledRows()
			Else
				zzShowAllRows()
			End If
		End If

	End Sub

	Private Sub dgvMain_DataError(oSender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
		e.Cancel = True
	End Sub




	Private Sub chkAddToSelect_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkAddToSelect.CheckedChanged
		If Me.chkAddToSelect.Checked Then
			Me.chkAddToSelect.Text = "+"
		Else
			Me.chkAddToSelect.Text = "1"
		End If
	End Sub



	Private Sub chkLastPoint_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkLastPoint.CheckedChanged
		If mbEventsEnabled Then
			Dim iStartPointNum As Integer
			If Me.chkLastPoint.Checked Then
				iStartPointNum = miMaxPointDBNum + 1
			Else
				iStartPointNum = miThisPlanMinDBNum
			End If
			Me.txtStartPoint.Text = Convert.ToString(iStartPointNum)
			zzSetNewPointNo()
		End If

	End Sub

	Private Sub txtStartPoint_Validated(oSender As System.Object, e As EventArgs) Handles txtStartPoint.Validated
		If mbEventsEnabled Then
			zzSetNewPointNo()
		End If

	End Sub


	Private Sub cmbPlanID_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbPlanID.SelectedIndexChanged
		If mbEventsEnabled Then
			mbEventsEnabled = False
			If Me.cmbPlanID.SelectedIndex >= 0 AndAlso Me.cmbPlanID.SelectedItem IsNot Nothing Then
				miPlanID = DirectCast(Me.cmbPlanID.SelectedItem, Integer)
				zzLoadByPlanID()
				Me.cmdSaveDB.Enabled = True
				Me.cmbPlanID.Enabled = False
			Else
				Me.cmdSaveDB.Enabled = False

			End If
			mbEventsEnabled = True
		End If

	End Sub

	Private Sub cmdSelectionSet_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectionSet.Click
		If miCurrentActionFirstRowIndex <> -1 AndAlso miCurrentActionType = UnidivNet.enActionType.Divide Then
			If Me.chkSelectUser.Checked Then
				zzSelectUserLinks(enSelectLinkType.SelectionSet)
			Else
				zzBeforeDivide(False, enSelectLinkType.SelectionSet)
			End If

		ElseIf miCurrentActionFirstRowIndex <> -1 AndAlso miCurrentActionType = UnidivNet.enActionType.Union Then
			If Me.chkSelectUser.Checked Then
				zzSelectUserLinks(enSelectLinkType.SelectionSet)
			Else
				zzSelectParcelsByTopo(False)
			End If

		ElseIf miCurrentActionFirstRowIndex <> -1 AndAlso miCurrentActionType = UnidivNet.enActionType.Transfer AndAlso dgvMain.AllowUserToAddRows Then
			zzSelectParcelsByTopo(False)

		ElseIf Me.chkRestoreCancelLink.Checked Then
			zzRestoreCancelLinks(enSelectLinkType.SelectionSet)
			Me.chkRestoreCancelLink.Checked = False
		End If
	End Sub

	Private Sub cmdLoadScript_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadScript.Click

		Select Case mtCurrentScriptData.ActionType
			Case UnidivNet.enActionType.Divide
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
				mtCurrentScriptData.Load(enScriptType.ParcelLinkList)
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				zzBeforeDivide(False, enSelectLinkType.Script)
			Case UnidivNet.enActionType.Union
				mtCurrentScriptData.Load(enScriptType.ParcelList)
				zzLoadParcelListScript()

			Case UnidivNet.enActionType.RestoreLayer
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
				mtCurrentScriptData.Load(enScriptType.LinkList)
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()

				Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
				zzRestoreCancelLinks(enSelectLinkType.Script)
		End Select
	End Sub

	Private Sub chkRestoreCancelLink_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkRestoreCancelLink.CheckedChanged
		If Me.chkRestoreCancelLink.Checked Then
			zzOpenDWG(False, False)
			Me.chkRestoreCancelLink.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16InvTr
			mbEventsEnabled = False
			chkHanitView.Checked = False
			mbEventsEnabled = True

			DMAcadExt.AcadTransaction.SetLayersOn(moUD_PLineLayerList, False)
			zzCloseDWG()
			zzSetScriptData(True)
		Else
			Me.chkRestoreCancelLink.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
		End If
	End Sub
	Private Sub zzSetScriptData(bAdditionalAction As Boolean)
		Dim bActionClosed As Boolean = miCurrentActionFirstRowIndex = -1
		mtCurrentScriptData = New ScriptData(bAdditionalAction, bActionClosed, miCurrentActionType, miCurrentStage, miCurrentAction)
		If mtCurrentScriptData.FileExists Then
			Me.cmdLoadScript.Enabled = True
		End If
	End Sub

	Private Sub cmdLoadByAction_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadByAction.Click
		Me.Cursor = Cursors.WaitCursor
		zzOpenJournalTable()


		If mdicFragments.DBVersionIsCorrect Then
			zzAddDBJournalData(enAddDataType.ActionData)
			zzOpenDWG(True, True)
			zzInterpretJournal()
			zzCloseDWG()
		Else

			DMCommon.Debug.MsgBox("09_655n", "DBVersionIsNotCorrect", moMainTable.Rows.Count, moMainTable.Columns.Count, mdicParcels.Count)
		End If
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdLoadByStage_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadByStage.Click
		Me.Cursor = Cursors.WaitCursor
		zzOpenJournalTable()
		If mdicFragments.DBVersionIsCorrect Then
			miLastRowIndex = moMainTable.Rows.Count - 1
			zzAddDBJournalData(enAddDataType.StageData)
			zzOpenDWG(True, True)
			zzInterpretJournal()
			zzCloseDWG()
		Else
			DMCommon.Debug.MsgBox("09_655p", "DBVersionIsNotCorrect", moMainTable.Rows.Count, moMainTable.Columns.Count, mdicParcels.Count)
		End If
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdClearDB_Click(oSender As System.Object, e As EventArgs) Handles cmdClearDB.Click
		zzClearJournalData()
		zzClearPlanData()
	End Sub


	Private Sub chkFreezeUnion_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkSelectUser.CheckedChanged
		If chkSelectUser.Checked Then
			mcolBlockedLinks.Clear()
		End If
	End Sub

	Private Sub chkExcelLog_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkExcelLog.CheckedChanged
		If chkExcelLog.Checked AndAlso Not DMCommon.Debug.ExcelLogIsOpened Then

			DMCommon.Debug.ExcelLog.Open()

		ElseIf Not chkExcelLog.Checked Then
			DMCommon.Debug.ExcelLog.Close()
		End If

	End Sub


	Private Sub cmdExcelJournal_Click(oSender As System.Object, e As EventArgs) Handles cmdExcelJournal.Click
		Const sSheetName As String = "אאאאאא"
		Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
		Dim oGridColumn As DataGridViewColumn
		Dim iExcelRow As Integer = 0
		Dim iExcelColumn As Integer = 0
		Dim oDataRow As DataRow
		Dim colFieldNames As System.Collections.ObjectModel.Collection(Of String) = New ObjectModel.Collection(Of String)()
		Dim colFormats As System.Collections.ObjectModel.Collection(Of String) = New ObjectModel.Collection(Of String)()
		Dim sColFormat As String
		Dim sFieldName As String
		Dim oNumCellValue As System.Object
		Dim iRowsCount As Integer = moMainTable.Rows.Count
		Dim dValue As Double
		Dim tRect As Rectangle
		Dim iRowStatus As Integer
		Me.Cursor = Cursors.WaitCursor
		oExcelAppExt.Open()
		oExcelAppExt.SetSheetName(sSheetName)


		For iColIndex As Integer = 0 To dgvMain.ColumnCount - 1
			oGridColumn = dgvMain.Columns.Item(iColIndex)
			If oGridColumn.Visible Then
				If oGridColumn.DividerWidth > 0 Then
					tRect = New Rectangle(iExcelColumn, 0, 0, iRowsCount)
					oExcelAppExt.SetBorders(tRect, zzDividerToBorderWeight(oGridColumn.DividerWidth))
				End If
				oExcelAppExt.SetValueInHeaderRow(iExcelRow, iExcelColumn, True, oGridColumn.HeaderText)

				colFieldNames.Add(oGridColumn.DataPropertyName)
				colFormats.Add(oGridColumn.DefaultCellStyle.Format)
				iExcelColumn += 1
			End If

		Next
		tRect = New Rectangle(iExcelColumn, 0, 0, iRowsCount)
		oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

		For iRowIndex As Integer = 0 To moMainTable.Rows.Count - 1
			oDataRow = moMainTable.Rows.Item(iRowIndex)
			For iFieldIndex As Integer = 0 To colFieldNames.Count - 1
				sColFormat = colFormats.Item(iFieldIndex)
				sFieldName = colFieldNames.Item(iFieldIndex)
				If String.IsNullOrEmpty(sColFormat) Then
					oNumCellValue = oDataRow.Item(sFieldName)
				Else
					oNumCellValue = zzGridFormat(oDataRow.Item(sFieldName), sColFormat)
				End If
				oExcelAppExt.SetValueInRow(iRowIndex + 1, iFieldIndex, oNumCellValue)

			Next

			iRowStatus = DMCommon.Functions.CIntN(oDataRow.Item("RowStatus"))
			If iRowStatus > 1 Then
				tRect = New Rectangle(0, iRowIndex + 1, iExcelColumn - 1, 0)
				oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

			End If

		Next

		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzDividerToBorderWeight(iDivider As Integer) As DMCommon.ExcelAppExt.enBorderWeight
		Select Case iDivider
			Case 0
				Return DMCommon.ExcelAppExt.enBorderWeight.Missing
			Case 1
				Return DMCommon.ExcelAppExt.enBorderWeight.Hairline
			Case 2
				Return DMCommon.ExcelAppExt.enBorderWeight.Thin
			Case 3, 4
				Return DMCommon.ExcelAppExt.enBorderWeight.Medium
			Case Else
				Return DMCommon.ExcelAppExt.enBorderWeight.Thick
		End Select
	End Function
	Private Function zzGridFormat(oValue As System.Object, sFormat As String) As System.Object
		Dim iDigit As Integer
		If Not String.IsNullOrEmpty(sFormat) AndAlso sFormat.StartsWith("N") Then
			'	iDigit = Convert.ToInt32(sFormat.Substring(1))
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzGridFormat", oValue, sFormat)
			If sFormat.Length > 1 AndAlso Integer.TryParse(sFormat.Substring(1), iDigit) Then

				Dim dValue As Double = DMCommon.Functions.CDblN(oValue)
				Return Math.Round(dValue, iDigit)
			Else
				DMCommon.Debug.MsgBox("!zzGridFormat", oValue, sFormat)
				Return oValue
			End If
		Else
			Return oValue
		End If
	End Function


	Private Sub Button3_Click(sender As System.Object, e As EventArgs) Handles Button3.Click
		Dim iIncomingParcelsCount As Integer
		Dim iOutgoingParcelsCount As Integer

		Dim sOriginalParcelList As String = ""
		Dim sNewParcelList As String = ""
		mdicParcels.GetInitParcelString(False, sOriginalParcelList, sNewParcelList, iIncomingParcelsCount, iOutgoingParcelsCount)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!AllPARC_RES", sOriginalParcelList, sNewParcelList)
	End Sub

	Private Sub frmUnidiv_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			'	Me.Size = New System.Drawing.Size(934, 654)
			Me.WindowState = FormWindowState.Normal
		End If
		mbEventsEnabled = True
	End Sub
End Class