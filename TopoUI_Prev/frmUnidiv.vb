Option Strict On
Imports System.Data
Imports TopoManager.TPlanGraph
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports TopoManager.TopoScheme

Public Class frmUnidiv
   Private Enum enRowStatus
      [Default]
      StartOfAction
      EndOfAction
      EndOfStage
      Transfer
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
   Const msPLineCancelledLayerOld As String = "PCLP005"
   Const msLinkNewLayerOld As String = "PCLP011"

   Const msLinkGushLayer As String = "C1650"
   Const msLinkParcelLayer As String = "C1660"
   Const msLinkCancelledLayer As String = "C1661"
   Const msLinkNewLayer As String = "C1662"
   Const miPlanTypeDflt As Integer = 2



   Const msCentroidBlockName As String = "C1603"
   Const msFragmentsTopoName As String = "Fragments"
   Const msFinalTopologyName As String = "FinalStage"
   Public Const ProjectDataBase As String = "UD_Projects"
   Const msNewParcel As String = "Ud_NewParcels"
   Const msCreateNewNodesLayer As String = "Ud_NewPoints"
   Const miSelectedColorIndex As Integer = 50

   Const miAfterActionDividerHeight As Integer = 2
   Const miAfterStageDividerHeight As Integer = 4
   Const miRoundDigit As Integer = 3
   Const msCaption As String = "איחוד וחלוקה"

   Private moAppWin As Autodesk.AutoCAD.Windows.Window
   '   Private moaParcels() As UnidivNet.UD_Parcel
   '   Private mdicParcelsID As Dictionary(Of Integer, UnidivNet.UD_Parcel)
   Private mdicParcels As UnidivNet.UD_Parcels
   Private Shared mdicPoints As UnidivNet.UD_Points
   Private Shared mdicFLines As UnidivNet.UD_FLines
   Private Shared mcolAllNodes As ObjectIdCollection = New ObjectIdCollection()
   Private mcolCanceledLinks As ObjectIdCollection = New ObjectIdCollection()

   '   Private Shared mdicSourceParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
   Private moFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel
   Private moFragmentsToposcheme As tsTopology
   Private mdicFragments As Fragments
   Private mhsPoints As Ud_Points
   Private mhsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
   Private moDataGridViewCellStyleArea As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
   Private moDataGridViewCellStyleTolerance As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
   Private moDataGridViewCellStyleInput As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()
   Private moDataGridViewCellStyleInputSaved As System.Windows.Forms.DataGridViewCellStyle '= New System.Windows.Forms.DataGridViewCellStyle()
   Private miFromParcelColIndex As Integer = 2
   Private miFromParcelTempColIndex As Integer = 3
   Private miToParcelColIndex As Integer = 4
   Private miToGushColIndex As Integer = 5
   Private miForcedAreaColIndex As Integer = 8

   Private miPlanNameColIndex As Integer = 12
   Private miLotNameColIndex As Integer = 13
   Private miLanduseIDColIndex As Integer = 14
   Private miLanduseNameColIndex As Integer = 15


   Private mcolNewLayerCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private Shared mbHasder As Boolean = True

   '  Private moShrinkPgonJig As UnidivNet.ShrinkPgonJig
   '  Private moShrinkPolygon As UnidivNet.ShrinkPolygon
   Private WithEvents moMainTable As System.Data.DataTable
   '   Private moMainView As DataView
   Private moParcelTable As System.Data.DataTable
   Private moParcelLogTable As System.Data.DataTable
   Private moFragmentTable As System.Data.DataTable
   Private moParcelFragmentTable As System.Data.DataTable

   Private moLastActionRow As DataRow
   Private moJournalDataAdapter As Data.Common.DbDataAdapter
   Private moParcelLogDataAdapter As Data.Common.DbDataAdapter

   Private moFragmentsDataAdapter As Data.Common.DbDataAdapter
   Private moParcelFragmentsDataAdapter As Data.Common.DbDataAdapter
   Private mtEditingCellKey As CellKey
   Private mtEditingParcelKey As UD_ParcelKey


   Private miProjectCode As Integer
   Private miDetailNo As Integer

   Private miPlanType As Integer
   Private mbDatabound As Boolean
   Private mbNewData As Boolean
   Private mcolLine As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private miLastRowIndex As Integer
   Private mhsInputCells As HashSet(Of CellKey) = New HashSet(Of CellKey)(New CellComparer())
   Private mbEventsEnabled As Boolean
   Private miBlockNo As Integer
   Private miBlockAddNo As Integer = 0
   Private miCurrentActionType As UnidivNet.enActionType
   Private miCurrentStage As Integer = 0
   Private miCurrentAction As Integer = 0
   '  Private miCurrentOperation As Integer = 0
   Private miCurrentCentroidStatus As enCentroidStatus = enCentroidStatus.Default
   Private moCurrentStageTopoScheme As TopoManager.TopoScheme.tsTopology

   Private miCurrentActionFirstRowIndex As Integer = -1
   Private miLastActionFirstRowIndex As Integer = -1
   Private mbExtended As Boolean
   '  Private miCurrentNewParcelNo As Integer
   Private mtCurrentParcelKey As UD_ParcelKey
   ' Private miLastParcel As Integer
   Private miLastPoint As Integer
   Private moStageTopologies As StageTopologies = New StageTopologies()
   ' Private mdicPoints As UnidivNet.UD_Points
   '   Private mdicRows As Dictionary(Of Integer, UD_Row)
   Private mhsOperUDLayers As HashSet(Of String) = New HashSet(Of String)(New String() {"aaa", "bbb"})
   Private moOperUDLayersList As DMCommon.dmList = New DMCommon.dmList(New String() {msLinkGushLayer, msLinkParcelLayer, msLinkCancelledLayer, msLinkNewLayer, "C1603_*"})
   Private moUD_ParcelLayerList As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*"})
   Private moUD_ParcelLayerListPlusNew As DMCommon.dmList = New DMCommon.dmList(New String() {"C1602_*", "C1603_*", msLinkNewLayer})

   Private mcolUnvisibleLayers As IEnumerable(Of String) = New System.Collections.ObjectModel.Collection(Of String)()
   Private mcolParcelCancelled As ObjectIdCollection = New ObjectIdCollection()
   Private mcolNotFocused As ObjectIdCollection = New ObjectIdCollection()


   Private mbCheckExtended As Boolean = True

   Private miStageDisplayed As Integer = -1
   Private midgvMainLocationY As Integer
   Private msaPointLayers() As String
   Private WithEvents mfUD_General As TopoUI.frmUD_General
   Private Sub zzFillInitGrid()
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow = Nothing
      Dim iIndex As Integer
      ' mdicRows = New Dictionary(Of Integer, UD_Row)
      '    53 62 12 985 Vova
      Dim b As Boolean = DMCommon.Debug.Debug()
      '   DMCommon.Debug.MsgBox("@mdicParcels.Count", mdicParcels.Count)


      For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
         '  DMCommon.Debug.MsgBox("oParcel.Fragments.Count", oParcel.Fragments.Count)
         zzAddGridRow(enRowStatus.Default)
         '  DMCommon.Debug.MsgBox("12_101gb", mbEventsEnabled)
         If mbDatabound Then
            oDataRow = moMainTable.Rows.Item(iIndex)
         Else
            oGridRow = Me.dgvMain.Rows.Item(iIndex)
         End If


         Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey


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
         zzFillParcelAreaNew(oParcel.ParcelArea, oDataRow, oGridRow)

         'oGridRow.Cells.Item("ctxLegalArea").Value = oParcel.LegalArea
         'oGridRow.Cells.Item("ctxArea").Value = oParcel.AcadArea(True)
         'oGridRow.Cells.Item("ctxTolerance").Value = 0.001 * oParcel.Tolerance
         'oGridRow.Cells.Item("ctxDiff").Value = oParcel.LegalArea - oParcel.AcadArea(True)
         'oGridRow.Cells.Item("ctxDeviation").Value = oParcel.Deviation
         iIndex += 1
      Next
      miLastRowIndex = iIndex - 1
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
         End If
         '  mdicRows.Add(iRowIndex, New UD_Row(0))
         miLastRowIndex = iRowIndex
      Next




      '    DMCommon.Debug.MsgBox("miLastRowIndex =", miLastRowIndex, moMainTable.Rows.Count)

   End Sub
   Private Sub zzInterpretJournal()
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow = Nothing
      Dim iIndex As Integer
      Dim iDBStage As Integer
      Dim tSourceParcelKey As UD_ParcelKey
      Dim oDestParcel As UnidivNet.UD_Parcel

      Dim tDestParcelKey As UD_ParcelKey

      Dim iParcelDbID As Integer
      Dim iRowStatus As enRowStatus
      Dim oListDivided As List(Of UD_ParcelKey) = Nothing
      Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion
      Dim oDestPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing

      Dim hsSourceBoundaryBranches As HashSet(Of Integer) = Nothing ' = New HashSet(Of Integer)()
      Dim hsParcelBoundaryBranches As HashSet(Of Integer)
      Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection ' = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim sPrevParcelName As String = Nothing
      Dim dLegalArea As Double
      Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
      Dim bSourceParcelExists As Boolean
      Dim dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion) = Nothing
      Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel) = Nothing
      oListDivided = New List(Of UD_ParcelKey)
      zzOpenDWG()
      '  mdicRows = New Dictionary(Of Integer, UD_Row)

      For iRowIndex As Integer = miLastRowIndex + 1 To moMainTable.Rows.Count - 1
         oDataRow = moMainTable.Rows.Item(iRowIndex)
         miCurrentStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
         miCurrentAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
         miCurrentActionType = zzToActionType(oDataRow)
         iParcelDbID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelDbID"))
         iRowStatus = zzToRowStatus(oDataRow)

         '''''''''''''' DMCommon.Debug.MsgBox("11_600A", iRowIndex, miCurrentStage, miCurrentActionType, iRowStatus)

         Select Case miCurrentActionType
            Case UnidivNet.enActionType.Default

            Case UnidivNet.enActionType.Divide
               If iRowStatus = enRowStatus.StartOfAction Then
                  tSourceParcelKey = zzParcelKeyFrom(oDataRow)
                  bSourceParcelExists = mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel)

                  dicParcelByFragments = New Dictionary(Of Integer, tsPgonUnion)()
                  hsSourceBoundaryBranches = New HashSet(Of Integer)()
                  '  DMCommon.Debug.MsgBox("11_601", iRowIndex, tSourceParcelKey.UD_ParcelName, bSourceParcelExists)
                  miCurrentActionFirstRowIndex = iRowIndex
               End If
               tDestParcelKey = zzParcelKeyTo(oDataRow)
               oPgonUnion = zzGetParcelPgonUnion(iParcelDbID, tDestParcelKey, dicParcelByFragments)
               '  DMCommon.Debug.MsgBox("11_209C", iParcelDbID, tDestParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo, oPgonUnion.FragmentCount)
               hsParcelBoundaryBranches = oPgonUnion.BoundaryBranches
               hsSourceBoundaryBranches.UnionWith(hsParcelBoundaryBranches)
               If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
                  '   DMCommon.Debug.MsgBox("11_602", iRowIndex, DMCommon.Debug.ColCount(hsSourceBoundaryBranches))
                  colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
                  zzGetTopoElements(hsSourceBoundaryBranches, colTopoLinks, colNodes)
                  If bSourceParcelExists Then
                     zzCreateDivideStageTopologyDB(colTopoLinks, UnidivNet.UD_App.AllNodes, oListDivided, dicDestParcels, oSourceParcel.UD_Name, dicParcelByFragments)
                     zzCreatePgonsPlus()
                     zzUpdateDivideResParcels(dicDestParcels)
                     '''''''''' zzLoadDivideResParcels(oParcel)
                     If oListDivided IsNot Nothing Then
                        zzCalcDivideResParcels(oSourceParcel, oListDivided, True)
                        ' zzFragmentsAllocation(oPgonUnion)
                     End If
                     oSourceParcel.IsCanceled = True
                     oSourceParcel.UpdateBlockAttributes()
                  Else
                     DMCommon.Debug.MsgBox("Datamap", "Parcel '" & tDestParcelKey.UD_ParcelName & "' was not founded")
                  End If
                  ' DMCommon.Debug.MsgBox("11_603", iRowIndex, hsSourceBoundaryBranches.Count, colTopoLinks.Count)
                  If iRowIndex = 6 Then
                     ' Exit For
                  End If

               End If

            Case UnidivNet.enActionType.Union

               tSourceParcelKey = zzParcelKeyFrom(oDataRow)
               bSourceParcelExists = mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel)
               If bSourceParcelExists Then
                  oPgonUnion = oSourceParcel.GetFragmentsPgonUnion(moFragmentsToposcheme)
                  oSourceParcel.IsCanceled = True
               End If

               If iRowStatus = enRowStatus.StartOfAction Then
                  tDestParcelKey = zzParcelKeyTo(oDataRow)
                  oDestPgonUnion = oPgonUnion
                  sPrevParcelName = oSourceParcel.UD_Name
                  dLegalArea = oSourceParcel.LegalArea
                  dicParcelByFragments = New Dictionary(Of Integer, tsPgonUnion)()
                  '  DMCommon.Debug.MsgBox("11_601", iRowIndex, tSourceParcelKey.UD_ParcelName, bSourceParcelExists)
                  miCurrentActionFirstRowIndex = iRowIndex
               Else
                  oDestPgonUnion.AddPolygonUnion(oPgonUnion)
                  sPrevParcelName &= "," & oSourceParcel.UD_Name
                  dLegalArea += oSourceParcel.LegalArea
               End If



               '  DMCommon.Debug.MsgBox("11_209C", iParcelDbID, tDestParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo)

               If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
                  hsParcelBoundaryBranches = oDestPgonUnion.BoundaryBranches
                  '  hsSourceBoundaryBranches.UnionWith(hsParcelBoundaryBranches)

                  colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
                  zzGetTopoElements(hsParcelBoundaryBranches, colTopoLinks, colNodes)
                  '     DMCommon.Debug.MsgBox("11_602", iRowIndex, DMCommon.Debug.ColCount(hsParcelBoundaryBranches), colTopoLinks.Count)
                  If bSourceParcelExists Then
                     oDestParcel = Nothing
                     '   DMCommon.Debug.MsgBox("11_632", colTopoLinks.Count, mcolAllNodes.Count, sPrevParcelName, dLegalArea, oDestPgonUnion.ParcelKey.ToString(), oDestPgonUnion.FragmentCount)
                     zzCreateUnionStageTopologyDB(colTopoLinks, mcolAllNodes, sPrevParcelName, dLegalArea, tDestParcelKey, oDestPgonUnion.Polygons, oDestParcel)
                     zzCreatePgonsPlus()
                     '''''''''' zzLoadDivideResParcels(oParcel)
                     If oDestParcel IsNot Nothing Then
                        zzCalcUnionResParcel(oDestParcel)

                        ' zzFragmentsAllocation(oPgonUnion)
                     End If

                  Else
                     DMCommon.Debug.MsgBox("Datamap", "Parcel '" & tDestParcelKey.UD_ParcelName & "' was not founded")
                  End If
                  ' DMCommon.Debug.MsgBox("11_603", iRowIndex, hsSourceBoundaryBranches.Count, colTopoLinks.Count)
                  
                  Exit For
                  ' DMCommon.Debug.MsgBox("11_217", "after Union")
               End If
            Case UnidivNet.enActionType.Transfer


               ' DMCommon.Debug.MsgBox("11_218z", zzFinalTopologyIsCreated(), "after Union")
               '  zzCreateFinalTopology()
               zzFinalTopo()

               DMCommon.Debug.MsgBox("11_218s", zzFinalTopologyIsCreated(), "after Union")
               tSourceParcelKey = zzParcelKeyFrom(oDataRow)

               zzTransferFromFinalTopology(tSourceParcelKey)
               '  DMCommon.Debug.MsgBox("11_219rs", "zzTransferFromFinalTopology")
               zzInsertTransferCentroid()
               '  DMCommon.Debug.MsgBox("11_2120vv", "zzTransferFromFinalTopology")
         End Select
         '  mdicRows.Add(iRowIndex, New UD_Row(0))
      Next

      miLastRowIndex = iIndex - 1
      zzFinalTopo()
      zzInsertAllFLines()
      zzCloseDWG()
      zzUpdateStagesView()

   End Sub
   Private Function zzGetParcelFragments(iParcelDbID As Integer) As HashSet(Of Integer)
      Dim oDataView As Data.DataView = New DataView(moParcelFragmentTable, "ParcelDbID = " & Convert.ToString(iParcelDbID), String.Empty, DataViewRowState.CurrentRows)
      Dim hsFragments As HashSet(Of Integer) = New HashSet(Of Integer)
      Dim iFragmentID As Integer
      For Each oRow As DataRowView In oDataView
         iFragmentID = DMCommon.Functions.CIntN(oRow.Item("FragmentID"))
         hsFragments.Add(iFragmentID)
      Next
      Return hsFragments




   End Function
   Private Function zzGetParcelPgonUnion(iParcelDbID As Integer, tParcelKey As UD_ParcelKey, ByRef dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion)) As tsPgonUnion
      Dim sFilter As String = "ParcelDbID = " & Convert.ToString(iParcelDbID)
      Dim oDataView As Data.DataView = New DataView(moParcelFragmentTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
      Dim hsFragments As HashSet(Of Integer) = New HashSet(Of Integer)
      Dim iFragmentID As Integer
      Dim oPgonUnion As tsPgonUnion = New tsPgonUnion(moFragmentsToposcheme.Elements)
      Dim oPgonScheme As tsPolygon
      oPgonUnion.ParcelDbID = iParcelDbID
      oPgonUnion.ParcelKey = tParcelKey

      For Each oRow As DataRowView In oDataView
         iFragmentID = DMCommon.Functions.CIntN(oRow.Item("FragmentID"))
         oPgonScheme = moFragmentsToposcheme.GetPolygon(iFragmentID)
         oPgonUnion.AddPolygon(oPgonScheme)

         ' hsFragments.Add(iFragmentID)
      Next
      dicParcelByFragments.Add(iFragmentID, oPgonUnion)

      '  DMCommon.Debug.MsgBox("11_601j", moParcelFragmentTable.Rows.Count, oDataView.Count, iParcelDbID, DMCommon.Debug.ColCount(oPgonUnion.Polygons), DMCommon.Debug.ColCount(oPgonUnion.FragmentPgons))
      Return oPgonUnion
   End Function
   Private Sub zzCloseLastRow()
      If moLastActionRow IsNot Nothing Then
         moLastActionRow.Item("RowStatus") = enRowStatus.EndOfStage
         moLastActionRow = Nothing
      End If
   End Sub

   Private Sub zzAddGridRow(iRowStatus As enRowStatus)
      If mbDatabound Then
         zzAddDataRow(iRowStatus)
      Else
         Me.dgvMain.Rows.Add()
      End If
   End Sub
   Private Sub zzAddDataRow(iRowStatus As enRowStatus, Optional oNewRow As DataRow = Nothing)
      Dim bNewRow As Boolean
      If oNewRow Is Nothing Then
         oNewRow = moMainTable.NewRow
         bNewRow = True
      End If

      oNewRow.Item("ProjectCode") = miProjectCode
      oNewRow.Item("Detail") = miDetailNo
      oNewRow.Item("OriginalBlockNo") = miBlockNo
      oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

      oNewRow.Item("Stage") = miCurrentStage
      oNewRow.Item("Oper") = moMainTable.Rows.Count + 1
      ' DMCommon.Debug.MsgBox("09_843", moMainTable.Rows.Count)
      oNewRow.Item("Action") = miCurrentAction
      oNewRow.Item("ActionType") = miCurrentActionType
      oNewRow.Item("RowStatus") = iRowStatus
      If bNewRow AndAlso oNewRow.RowState = DataRowState.Detached Then
         moMainTable.Rows.Add(oNewRow)
      End If

      ''''''''''''''''  oNewRow.AcceptChanges()
      '     

   End Sub
   Private Sub zzFillLastDataRowAAA()
      Dim oNewRow As DataRow = moMainTable.Rows.Item(moMainTable.Rows.Count - 1)
      Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(Me.dgvMain.Rows.Count - 1)
      oGridRow.DataBoundItem.GetType()
      DMCommon.Debug.MsgBox("09_843b", oGridRow.DataBoundItem.GetType(), moMainTable.Rows.Count, Me.dgvMain.Rows.Count)
      oNewRow.Item("ProjectCode") = miProjectCode
      oNewRow.Item("Detail") = miDetailNo
      oNewRow.Item("OriginalBlockNo") = miBlockNo
      oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

      oNewRow.Item("Stage") = miCurrentStage
      oNewRow.Item("Oper") = moMainTable.Rows.Count + 1

      oNewRow.Item("Action") = miCurrentAction
      oNewRow.Item("ActionType") = miCurrentActionType
      oNewRow.Item("Summarize") = 0

      '  moMainTable.Rows.Add(oNewRow)

      DMCommon.Debug.MsgBox("09_843", oNewRow.RowState, moMainTable.Rows.Count)
      oNewRow.AcceptChanges()
      DMCommon.Debug.MsgBox("09_843a", oNewRow.RowState, moMainTable.Rows.Count)
   End Sub
   Private Sub zzFillParcelArea(tParcelArea As ParcelArea, bInputArea As Boolean, ByRef oDataRow As DataRow, Optional oGridRow As DataGridViewRow = Nothing)

      If bInputArea Then
         oDataRow.Item("LegalArea") = tParcelArea.LegalArea
         '   DMCommon.ExcelLogG.SetNextValue(16, "B16", tParcelArea.LegalArea.ToString, oDataRow.Item("LegalArea"), "Z" & oDataRow.Item("LegalArea").ToString)
         ' DMCommon.ExcelLogG.SetNextValue(16, 1357.999, 1.00001)
         If oGridRow IsNot Nothing Then
            '   DMCommon.ExcelLogG.SetNextValue(20, "C20", oGridRow.Cells.Item("ctxLegalArea").Value, "X" & oGridRow.Cells.Item("ctxLegalArea").Value.ToString())

         End If

         oDataRow.Item("AcadArea") = tParcelArea.AcadAreaD
      End If



      oDataRow.Item("Tolerance") = 0.001 * tParcelArea.Tolerance
      oDataRow.Item("DiffArea") = (tParcelArea.LegalArea - tParcelArea.AcadAreaD)
      oDataRow.Item("Deviation") = tParcelArea.Deviation * 0.001
   End Sub
   Private Sub zzFillParcelAreaNew(tParcelArea As ParcelArea, ByRef oDataRow As DataRow, ByRef oGridRow As DataGridViewRow)
      If mbDatabound Then
         ' Dim oDataRow As DataRow = moMainTable.Rows.Item(oGridRow.Index)
         zzFillParcelAreaDataRow(tParcelArea, oDataRow)
      Else
         zzFillParcelAreaGridRow(tParcelArea, oGridRow)
      End If


   End Sub
   Private Sub zzFillParcelAreaDataRow(tParcelArea As ParcelArea, ByRef oDataRow As DataRow)
      oDataRow.Item("LegalArea") = tParcelArea.LegalArea
      oDataRow.Item("AcadArea") = tParcelArea.AcadAreaD
      oDataRow.Item("Tolerance") = 0.001 * tParcelArea.Tolerance
      oDataRow.Item("DiffArea") = (tParcelArea.LegalArea - tParcelArea.AcadAreaD)
      oDataRow.Item("Deviation") = 0.001 * tParcelArea.Deviation
   End Sub
   Private Sub zzFillParcelAreaGridRow(tParcelArea As ParcelArea, ByRef oGridRow As DataGridViewRow)
      oGridRow.Cells.Item("ctxLegalArea").Value = tParcelArea.LegalArea
      oGridRow.Cells.Item("ctxArea").Value = tParcelArea.AcadAreaD
      oGridRow.Cells.Item("ctxTolerance").Value = 0.001 * tParcelArea.Tolerance
      oGridRow.Cells.Item("ctxDiff").Value = (tParcelArea.LegalArea - tParcelArea.AcadAreaD)
      oGridRow.Cells.Item("ctxDeviation").Value = tParcelArea.Deviation
   End Sub
   Private Sub zzFillParcelArea(oParcel As ParcelArea, ByRef oGridRow As DataGridViewRow)
      If mbDatabound Then
         Dim oDataRow As DataRow = moMainTable.Rows.Item(oGridRow.Index)
         oDataRow.Item("LegalArea") = oParcel.LegalArea
         oDataRow.Item("AcadArea") = oParcel.AcadAreaD
      Else
         oGridRow.Cells.Item("ctxLegalArea").Value = oParcel.LegalArea
         oGridRow.Cells.Item("ctxArea").Value = oParcel.AcadAreaD
      End If

      oGridRow.Cells.Item("ctxTolerance").Value = 0.001 * oParcel.Tolerance
      oGridRow.Cells.Item("ctxDiff").Value = (oParcel.LegalArea - oParcel.AcadAreaD)
      oGridRow.Cells.Item("ctxDeviation").Value = oParcel.Deviation
   End Sub
   Private Sub zzFillFromParcelKeyNew(oParcel As UnidivNet.UD_Parcel, ByRef oDataRow As DataRow, ByRef oGridRow As DataGridViewRow)
      Dim tParcelKey As UD_ParcelKey = oParcel.ParcelKey
      If mbDatabound Then
         ' Dim oDataRow As DataRow = moMainTable.Rows.Item(oGridRow.Index)
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
         Else
            oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
         End If
         oDataRow.Item("ParcelDbID") = oParcel.DbID

         '  zzMarkParcel(tParcelKey)
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
   Private Function zzGetSourceParcelKey(oGridRow As System.Windows.Forms.DataGridViewRow) As UD_ParcelKey
      Dim oDataRowView As DataRowView
      Dim tResParcelKey As UD_ParcelKey
      '   Dim oDataRow As DataRow
      Dim iStageNo As Integer
      Dim iDestBlockNo As Integer

      '  DMCommon.Debug.MsgBox("12_002", "Stage=" & miCurrentStage)
      oDataRowView = TryCast(oGridRow.DataBoundItem, DataRowView)
      If oDataRowView IsNot Nothing Then
         iStageNo = DMCommon.Functions.CIntN(oDataRowView.Item("Stage"))
         If iStageNo = 0 Then
            tResParcelKey = UD_ParcelKey.FromGrid(miBlockNo, oDataRowView.Item("OriginalParcelNo"), oDataRowView.Item("NewParcelNo"))
         Else
            iDestBlockNo = DMCommon.Functions.CIntN(oDataRowView.Item("DestBlockNo"))
            If iDestBlockNo = 0 Then
               iDestBlockNo = miBlockNo
            End If
            tResParcelKey = UD_ParcelKey.FromGrid(iDestBlockNo, DBNull.Value, oDataRowView.Item("DestParcelNo"))
         End If
      End If
      Return tResParcelKey
   End Function
   Private Function zzParcelKeyFrom(oDataRow As DataRow) As UD_ParcelKey
      Return UD_ParcelKey.FromGrid(miBlockNo, oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
   End Function
   Private Function zzParcelKeyFrom(iColumnIndex As Integer, oValue As System.Object) As UD_ParcelKey
      Dim iParcelNo As Integer
      Dim bIsOriginal As Boolean
      If iColumnIndex = miFromParcelColIndex Then
         bIsOriginal = True
      End If
      Dim sValue As String = TryCast(oValue, String)
      If Integer.TryParse(sValue, iParcelNo) Then
         Return New UD_ParcelKey(miBlockNo, iParcelNo, bIsOriginal)
      End If



   End Function
   Private Function zzParcelFrom(oDataRow As DataRow) As UnidivNet.UD_Parcel
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      If mdicParcels.TryGetValue(zzParcelKeyFrom(oDataRow), oParcel) Then
         Return oParcel
      Else
         Return Nothing
      End If

   End Function

   Private Function zzParcelKeyTo(oDataRow As DataRow) As UD_ParcelKey
      Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo"))
      Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("DestBlockNo"))
      If iBlockNo = 0 Then
         iBlockNo = miBlockNo
      End If
      Return New UD_ParcelKey(iBlockNo, iParcelNo, False)
   End Function
   Private Function zzParcelTo(oDataRow As DataRow) As UnidivNet.UD_Parcel
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

      '  DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      ' DMAcadExt.AcadTransaction.Start()
      'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      ' DMAcadExt.AcadBlock.Init()

      UnidivNet.UD_Point.Init()
      UnidivNet.UD_FLine.Init()
      UnidivNet.UD_Parcel.InitAcadBlock()
      AcadReport.RepApp.InitDWGScaleFactor()
      DMAcadExt.AcadDocument.SaveVarCmdDia(0S)
      '  UnidivNet.UD_App.LoadFragmentsTopology()
      '  DMCommon.Debug.MsgBox("09_701", "zzLoadDWG", "bef zzLoadFragments")
      zzLoadFragments()

      If zzLoadParcelTopology() Then
      End If

      mcolNewLayerCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
      If False Then

         ' DMCommon.Debug.MsgBox("09_702", "zzLoadDWG", "bef zzLoadParcelTopology")


         ' mdicParcels = UnidivNet.UD_App.Parcels
         '  moFragmentsToposcheme = UnidivNet.UD_App.FragmentsToposcheme

         moStageTopologies.AddTopoName("Parcels")
      End If
      ''''''''''''''''   mdicParcels.DebugMsg()
      '   DMCommon.Debug.MsgBox("09_887", DMCommon.Debug.ColCount(UnidivNet.UD_App.Points))

   End Sub
   Private Sub zzMyInitializeComponent()
      moDataGridViewCellStyleArea.Format = "N3"
      moDataGridViewCellStyleArea.NullValue = Nothing
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
      '  moDataGridViewCellStyleInput.Font. = Color.White
      Me.dgvMain.Columns.Item("ctxFromParcel").DefaultCellStyle = moDataGridViewCellStyleInputSaved
      Me.dgvMain.Columns.Item("ctxFromParcelTemp").DefaultCellStyle = moDataGridViewCellStyleInputSaved
      Me.dgvMain.Columns.Item("ctxToParcel").DefaultCellStyle = moDataGridViewCellStyleInputSaved
      Me.dgvMain.Columns.Item("ctxToGush").DefaultCellStyle = moDataGridViewCellStyleInputSaved
      Me.dgvMain.AutoGenerateColumns = False
      midgvMainLocationY = Me.dgvMain.Location.Y
      '172584:

      '140731:
   End Sub

   Private Sub frmUnidiv_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
      '    zzOpenDWG()
      If mdicParcels IsNot Nothing Then
         mdicParcels.Clear()
         mdicParcels = Nothing
      End If
      If mdicFragments IsNot Nothing Then
         mdicFragments.Clear()
         mdicFragments = Nothing
      End If
      DMAcadExt.AcadDocument.RestoreVarCmdDia()
      zzCloseDWG()
   End Sub '
   Private Sub frmUnidiv_Load(oSender As System.Object, e As EventArgs) Handles Me.Load

      Dim bInputDWG As Boolean = True
      ''''''''''''''  TopoManager.TopoScheme.tsNode.IsGeoVertex = False
      zzOpenDWG()

      If moFragmentTopology IsNot Nothing Then


         zzLoadDWG()
         ' DMCommon.Debug.MsgBox("12_100a", miBlockNo) +++
         '  DMCommon.Debug.MsgBox("12_100b", "")
         If True Then
            zzOpenDB(False)

            zzCheckFragments()
            '  DMCommon.Debug.MsgBox("12_101a", mdicFragments.DBVersionIsCorrect)
            zzLoadJournalTable(bInputDWG)
            '  DMCommon.Debug.MsgBox("12_101b", "")
            zzCalcJournalAddFields()

            chkDataBound.Checked = mdicFragments.DBVersionIsCorrect
            '  DMCommon.Debug.MsgBox("12_101c", "")
            If Not bInputDWG Then
               zzCalcInitGrid()
               ' DMCommon.Debug.MsgBox("12_101d", "")
               If mdicFragments.DBVersionIsCorrect Then
                  zzInterpretJournal()
               End If
            End If
            ' DMCommon.Debug.MsgBox("12_101e", "")
            If bInputDWG AndAlso mdicParcels IsNot Nothing Then
               '  DMCommon.Debug.MsgBox("12_101f", "")
               zzFillInitGrid()
            End If
            '  DMCommon.Debug.MsgBox("12_101k", "")
            If False Then

               zzSetDB()
            End If

            ' DMAcadExt.AcadTransaction.CloseModelSpace()
            'DMAcadExt.AcadTransaction.Terminate()
            'DMAcadExt.AcadDocument.Unlock()
         End If
      End If
      zzCloseDWG()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.CloseLog()
      '  zzWriteParcelsInfo("After St 0")
      '   UnidivNet.UD_App.PrintPointsInfo()
      mbEventsEnabled = True
   End Sub

   Public Sub New(oAppWin As Autodesk.AutoCAD.Windows.Window)

      ' This call is required by the designer.
      InitializeComponent()
      moAppWin = oAppWin
      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()
      zzInitUD_Project()
      zzLoadProjectDataTable()
      DMAcadExt.AcadDocument.OpenLog(False)
      '   DMCommon.ExcelLogG.Open()
   End Sub
   Private Sub zzInitUD_Project()
      TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
      TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True)
      miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

   End Sub

   Private Sub zzSetActionEnabled(bEnabled As Boolean)
      Me.cmdOpenAction.Enabled = bEnabled
      Me.cmdInsertTable.Enabled = bEnabled
      Me.cmdCancelAction.Enabled = bEnabled
   End Sub
   Private Sub zzOpenAction(bNewStage As Boolean, iActionType As UnidivNet.enActionType)
      Dim oLastPrevRow As DataGridViewRow = Nothing
      Dim iPrevRowDividerHeight As Integer
      Dim iFirstRowStatus As enRowStatus
      mbEventsEnabled = False
      If moLastActionRow IsNot Nothing Then
         If bNewStage Then
            moLastActionRow.Item("RowStatus") = enRowStatus.EndOfStage
         Else
            moLastActionRow.Item("RowStatus") = enRowStatus.EndOfAction
         End If

      End If
      If iActionType = UnidivNet.enActionType.Transfer Then
         iFirstRowStatus = enRowStatus.Transfer
      Else
         iFirstRowStatus = enRowStatus.StartOfAction
      End If
      'If Me.dgvMain.Rows.Count > 0 Then
      '   oLastPrevRow = Me.dgvMain.Rows.Item(Me.dgvMain.Rows.Count - 1)
      'End If
      If bNewStage Then
         miCurrentActionType = iActionType
         miCurrentAction = 1
         miCurrentStage = miCurrentStage + 1
         '  oGridRow.Cells.Item("ctxStage").Value = miCurrentStage
         iPrevRowDividerHeight = miAfterStageDividerHeight
         '  miCurrentOperation = 1
      Else
         miCurrentAction += 1
         iPrevRowDividerHeight = miAfterActionDividerHeight

      End If

      '   DMCommon.Debug.MsgBox("09_873", oLastPrevRow IsNot Nothing, oLastPrevRow.Index, iPrevRowDividerHeight)

      zzAddGridRow(iFirstRowStatus)


      Me.dgvMain.DataSource = moMainTable
      If Me.dgvMain.Rows.Count > 1 Then
         oLastPrevRow = Me.dgvMain.Rows.Item(Me.dgvMain.Rows.Count - 2)
      End If
      If oLastPrevRow IsNot Nothing Then
         oLastPrevRow.DividerHeight = iPrevRowDividerHeight
      End If
      '   DMCommon.Debug.MsgBox("08_944", dgvMain.RowCount, moMainTable.Rows.Count)

      '   Dim iRowIndex As Integer = Math.Min(Me.dgvMain.RowCount, moMainTable.Rows.Count) - 1
      miCurrentActionFirstRowIndex = Math.Min(Me.dgvMain.RowCount, moMainTable.Rows.Count) - 1
      Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
      Dim oDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      Dim sActionName As String
      '  miCurrentActionFirstRowIndex = iRowIndex

      zzSetActionEnabled(True)
      oGridRow.Cells.Item("ctxFromParcel").Style = moDataGridViewCellStyleInput
      oGridRow.Cells.Item("ctxFromParcelTemp").Style = moDataGridViewCellStyleInput
      '   MessageBox.Show(moDataGridViewCellStyleInput.BackColor.ToString(), "09_879")
      zzSetNewParcelNo()
      '   DMCommon.Debug.MsgBox("08_544", bNewStage, miCurrentActionFirstRowIndex, iActionType, oLastPrevRow.Index)


      Select Case miCurrentActionType
         Case UnidivNet.enActionType.Union
            sActionName = "איחוד"
         Case UnidivNet.enActionType.Divide
            sActionName = "חלוקה"
         Case UnidivNet.enActionType.Transfer
            sActionName = "העברה"
         Case Else
            sActionName = ""
      End Select
      If mbDatabound Then
         If bNewStage Then
            oDataRow.Item("StageCaption") = miCurrentStage
         End If

         oDataRow.Item("ActionName") = sActionName
      Else
         If bNewStage Then
            oGridRow.Cells.Item("ctxStage").Value = miCurrentStage
         End If
         oGridRow.Cells.Item("ctxAction").Value = sActionName
      End If
      If miCurrentActionType <> UnidivNet.enActionType.Transfer Then
         '   oGridRow.Cells.Item("ctxToParcel").Value = Convert.ToString(mtCurrentParcelKey.ParcelNo)
         ' DMCommon.Debug.MsgBox("11_390", mtCurrentParcelKey.ParcelNo, mtCurrentParcelKey.UD_ParcelName)
         zzFillToParcelKey(mtCurrentParcelKey, oDataRow)
      End If
      '  zzClearInputCells()
      zzClearInputCellsForStage()
      zzAddInputCellsFirstRow(miCurrentActionFirstRowIndex)

      If miCurrentActionType <> UnidivNet.enActionType.Transfer Then
         Me.dgvMain.AllowUserToAddRows = True
         zzAddInputCellsNextRow(miCurrentActionFirstRowIndex + 1)
      End If
      mbEventsEnabled = True
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

   Private Sub zzSetNewParcelNo()
      Dim iInputParcel As Integer
      Dim iLastParcel As Integer = mdicParcels.GetMaxTempParcelNo()

      If Integer.TryParse(Me.txtLastParcel.Text, iInputParcel) Then
         iLastParcel = Math.Max(iInputParcel, iLastParcel)
      End If
      '    DMCommon.Debug.MsgBox("11_403", iLastParcel, mtCurrentParcelKey.ParcelNo)

      mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, iLastParcel + 1, False)
   End Sub
   Private Sub zzSetNewPointNoOld()
      Dim iInputPoint As Integer
      If Integer.TryParse(Me.txtLastPoint.Text, iInputPoint) Then
         miLastPoint = Math.Max(iInputPoint, miLastPoint)
      Else
         miLastPoint = 0
      End If
      miLastPoint += 1
   End Sub
   Private Sub zzSetNewPointNo()
      Dim iInputPoint As Integer
      If Integer.TryParse(Me.txtLastPoint.Text, iInputPoint) Then
         miLastPoint = Math.Max(iInputPoint, miLastPoint)
      Else
         miLastPoint = 0
      End If
      mdicPoints.PredeterminedNum = miLastPoint

   End Sub
   Private Sub zzCloseAction()
      DMAcadExt.AcadDocument.ClearDrawVectorSet()
      Me.dgvMain.AllowUserToAddRows = False
      Select Case miCurrentActionType
         Case UnidivNet.enActionType.Divide
            Me.rdbUnion.Enabled = True
         Case UnidivNet.enActionType.Union
            Me.rdbDivide.Enabled = True
      End Select
      Me.rdbTransfer.Enabled = True
      miLastActionFirstRowIndex = miCurrentActionFirstRowIndex
      miCurrentActionFirstRowIndex = -1

      ' zzClearInputCells()
      zzClearInputCellsForStage()
      zzAddInputCellsLastAction()
      zzUpdateStagesView()
      '  zzAddInputCellsForcedArea()
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
         oDestParcel = zzParcelTo(oDataRow)
         oaResParcels(iRowIndex - miLastActionFirstRowIndex) = oDestParcel
         '!!!!!!!!!!!!!! dicParcels Update
         dForcedArea = DMCommon.Functions.CDblN(oDataRow.Item("ForcedArea"))
         If True Then
            If dForcedArea = 0 Then
               iFreeAreaIndex += 1
               iaFreeAreaIndecis(iFreeAreaIndex) = iRowIndex


               daSourceArea(iFreeAreaIndex) = oDestParcel.AcadArea


            Else
               dSourceArea -= dForcedArea * 1000.0
               oDestParcel.CalculateArea(dForcedArea)
               zzFillParcelArea(oDestParcel.ParcelArea, True, oDataRow)
            End If
         End If
      Next

      ReDim Preserve daSourceArea(iFreeAreaIndex)
      '  DMCommon.Debug.MsgBox("09_800a", daSourceArea(0), daSourceArea(1), dSourceArea)
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
   Private Sub zzCalcDivideResParcels(oSourceParcel As UnidivNet.UD_Parcel, oList As List(Of UD_ParcelKey), bIsDBSource As Boolean)
      Const dRoundFactor As Double = 1.0
      Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim oBalanceArea As TopoManager.BalanceArea
      Dim iParcelsUB As Integer = oList.Count - 1
      Dim daSourceArea(iParcelsUB) As Double
      Dim oaResParcels(iParcelsUB) As UnidivNet.UD_Parcel
      Dim iIndex As Integer = 0
      Dim oDataRow As DataRow = Nothing
      Dim oGridRow As DataGridViewRow = Nothing
      ' DMCommon.Debug.MsgBox("09_799", oList.Count)
      '  Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      If True OrElse Not bIsDBSource Then
         oList.Sort(oParcelComparer)
      End If



      For Each tParcelKey As UD_ParcelKey In oList
         If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
            '
            daSourceArea(iIndex) = oParcel.AcadArea(False)
            oaResParcels(iIndex) = oParcel



            ' DMCommon.Debug.MsgBox("09_132", True, oParcel.LegalArea, oParcel.AcadArea)
         Else
            DMCommon.Debug.MsgBox("09_137", tParcelKey)
         End If
         iIndex += 1
      Next

      Dim dResArea As Double
      Dim dSourceArea As Double = oSourceParcel.ParcelArea.LegalArea * 1000.0

      Dim oPgonScheme As tsPolygon
      oBalanceArea = New TopoManager.BalanceArea(daSourceArea, dRoundFactor, dSourceArea, True, "Ud")
      For iIndex = 0 To oList.Count - 1
         oParcel = oaResParcels(iIndex)
         '    DMCommon.Debug.MsgBox("09_800", iIndex, oParcel.Name, oParcel.ParcelKey.UD_ParcelName, oParcel.Fragments.Count)
         dResArea = oBalanceArea.OutputItemFloat(iIndex)
         oParcel.CalculateArea(dResArea * 0.001)
         oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)
         oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex + iIndex)
         oDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
         oDataRow.Item("ParcelDbID") = oParcel.DbID

         oDataRow.Item("BorderObjID") = oPgonScheme.BorderObjID

         oParcel.ParcelKey.ToJournalDataRow(oDataRow, False)
         oDataRow.Item("LanduseName") = oParcel.LanduseName
         oDataRow.Item("LotName") = oParcel.Lot
         oDataRow.Item("PlanName") = oParcel.Plan

         '    DMCommon.Debug.MsgBox("11_337", oParcel.TopoID, moCurrentStageTopoScheme.Polygons.Count)

         oGridRow = zzGetRow(miCurrentActionFirstRowIndex + iIndex)
         zzFillParcelArea(oParcel.ParcelArea, True, oDataRow, oGridRow)
         oParcel.UpdateBlockAttributes()
      Next
      oDataRow.Item("RowStatus") = enRowStatus.EndOfAction
      moLastActionRow = oDataRow
      '  zzCloseLastRow()


   End Sub
   Private Sub zzCalcUnionResParcel(oDestParcel As UnidivNet.UD_Parcel)

      Dim oDataRow As DataRow = zzGetDataRow(miCurrentActionFirstRowIndex)
      oDataRow.Item("AcObjID") = oDestParcel.CentroidAcObjID
      oDataRow.Item("ParcelDbID") = oDestParcel.DbID
      Dim oPgonScheme As tsPolygon = moCurrentStageTopoScheme.Polygons.Item(0)
      oDataRow.Item("BorderObjID") = oPgonScheme.BorderObjID
      zzFillParcelArea(oDestParcel.ParcelArea, True, oDataRow)
      oDestParcel.UpdateBlockAttributes()

   End Sub
   Private Sub zzLoadDivideResParcels_AAA(oSingleParcel As UnidivNet.UD_Parcel)

      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      Dim tsStageParcels As tsTopology = New tsTopology(sStageTopoName)
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
      Dim oList As List(Of UD_ParcelKey) = New List(Of UD_ParcelKey)()
      Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex

      '   DMCommon.Debug.MsgBox("09_119", oSingleParcel.ParcelKey.ToString(), oSingleParcel.ParcelArea.LegalArea, oSingleParcel.ParcelArea.AcadArea)




      oAcadBlock.Fields = {"PARCEL_NAME"}
      oAcadBlock.OpenForRead()

      tsStageParcels.Load(mbCheckExtended)

      zzCreatePgonsPlusAAA(sStageTopoName)

      Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow = Nothing

      Dim oBalanceArea As TopoManager.BalanceArea

      Dim iIndex As Integer = 0
      Dim iParcelsUB As Integer = tsStageParcels.Elements.Polygons.Count - 1
      Dim daSourceArea(iParcelsUB) As Double
      Dim laOutput(iParcelsUB) As Long
      Dim dicStageParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel) = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
      Const dRoundFactor As Double = 1.0

      DMCommon.Debug.MsgBox("09_121", mdicParcels.Count)
      For Each oPgon As tsPolygon In tsStageParcels.Polygons

         tBlockRefData = oAcadBlock.GetBlockRefData(oPgon.AcObjID)
         oParcel = New UnidivNet.UD_Parcel(oPgon, tBlockRefData)
         DMCommon.Debug.MsgBox("09_122a", oParcel.Name)
         oParcel.Calc()

         ' dicStageParcels.Add(oPgon.ID, oParcel)
         '  DMCommon.Debug.MsgBox("09_123", True, iIndex, oParcel.ParcelKey, oParcel.AcadArea)
         daSourceArea(iIndex) = oParcel.AcadArea(False)
         '  DMCommon.Debug.MsgBox("09_122", True, daSourceArea(iIndex))
         ' oParcel.ParcelKey()
         DMAcadExt.AcadDocument.WriteDebugMessage(tBlockRefData.AttribValues(0))
         oList.Add(oParcel.ParcelKey)

         Try
            ' mdicParcels.AddParcel(oParcel)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oParcel.ParcelKey.ToString(), "LoadDivideResParcels")
         End Try

         '   mdicParcels.DebugMsg("Div 1")
         iIndex += 1
      Next
      DMCommon.Debug.MsgBox("09_122a", mdicParcels.Count)

      oBalanceArea = New TopoManager.BalanceArea(daSourceArea, dRoundFactor, oSingleParcel.ParcelArea.LegalArea * 1000.0, True, "Ud")

      For iIndex1 As Integer = 0 To iParcelsUB
         ' DMCommon.Debug.MsgBox("09_129", True, oList.Item(iIndex1))
         If mdicParcels.TryGetValue(oList.Item(iIndex1), oParcel) Then
            '  DMCommon.Debug.MsgBox("09_130", True, oBalanceArea.OutputItemFloat(iIndex1))
            oParcel.CalculateArea(oBalanceArea.OutputItemFloat(iIndex1) * 0.001)
            ' DMCommon.Debug.MsgBox("09_132", True, oParcel.LegalArea, oParcel.AcadArea)
         End If
      Next

      oList.Sort(oParcelComparer)
      DMAcadExt.AcadDocument.WriteDebugMessage("oList.count=" & oList.Count.ToString() & " mdicParcels.count=" & mdicParcels.Count.ToString())
      For Each tParcelKey As UD_ParcelKey In oList
         DMAcadExt.AcadDocument.WriteDebugMessage("Key=" & tParcelKey.ToString())
         If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
            ' mdicRows.Add(iRowIndex, New UD_Row(miCurrentStage))
            '    oGridRow = zzGetRow(iRowIndex)
            oDataRow = zzGetDataRow(iRowIndex)
            oParcel.ParcelKey.ToJournalDataRow(oDataRow, False)
            zzFillParcelArea(oParcel.ParcelArea, True, oDataRow)
            iRowIndex += 1
         End If
      Next
      moLastActionRow = oDataRow
      Return
      Dim oStageTopoModel As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sStageTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

      Dim oStagePolygon As Polygon = Nothing
      Dim sParcelName As String = Nothing
      If oStageTopoModel IsNot Nothing Then
         '  DMCommon.Debug.MsgBox("09_934", moFragmentsToposcheme.Elements.Polygons.Count)
         For Each oFragmentPgon As tsPolygon In moFragmentsToposcheme.Elements.Polygons
            oParcel = Nothing
            Try
               oStagePolygon = oStageTopoModel.FindPolygon(oFragmentPgon.Centroid)
            Catch oEx As Exception

            End Try

            If oStagePolygon IsNot Nothing AndAlso dicStageParcels.TryGetValue(oStagePolygon.ID, oParcel) Then
               oParcel.AddFragment(oFragmentPgon.ID)
               '  DMCommon.Debug.MsgBox("09_931", oStagePolygon, oStagePolygon.ID, oParcel)
            End If
            ' DMCommon.Debug.MsgBox("09_937", oStagePolygon, oParcel)
         Next
         oStageTopoModel.Close()
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
   Private Function zzGetCurrentStageTopoNameOld() As String
      Return "Stage" & miCurrentStage.ToString() & "_" & miCurrentAction.ToString()
   End Function
   Private Function zzGetCurrentStageTopoName() As String
      Return zzGetStageTopoName(miCurrentStage, miCurrentAction)
   End Function
   Private Function zzGetStageTopoName(iStage As Integer, iAction As Integer) As String
      Return "Stage" & iStage.ToString() & "_" & iAction.ToString()
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
      ' oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInput

      mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelColIndex, True))
      mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelTempColIndex, True))
      mhsInputCells.Add(New CellKey(iRowIndex, miToParcelColIndex, True))
      '  mhsInputCells.Add(New CellKey(iRowIndex, miToGushColIndex))

      If miCurrentActionType = UnidivNet.enActionType.Transfer Then
         oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInput
         mhsInputCells.Add(New CellKey(iRowIndex, miToGushColIndex, True))
      End If
   End Sub
   Private Sub zzAddInputCellsForcedArea()
      Dim iRowStatus As enRowStatus
      Dim iRowIndex As Integer = 0
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow

      If miCurrentActionType = UnidivNet.enActionType.Divide AndAlso miCurrentActionFirstRowIndex = -1 Then
         iRowIndex = moMainTable.Rows.Count - 1
         Do
            oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
            oDataRow = moMainTable.Rows.Item(iRowIndex)
            iRowStatus = zzToRowStatus(oDataRow)
            oGridRow.Cells.Item(miForcedAreaColIndex).Style = moDataGridViewCellStyleInput
            mhsInputCells.Add(New CellKey(iRowIndex, miForcedAreaColIndex, True))
            iRowIndex = iRowIndex - 1
         Loop Until (iRowStatus = enRowStatus.StartOfAction) OrElse (iRowIndex < 0)
      End If
   End Sub
   Private Sub zzAddInputCellsLotLanduse()
      Dim iRowStatus As enRowStatus
      Dim iRowIndex As Integer = 0
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow

      If miCurrentActionType = UnidivNet.enActionType.Divide AndAlso miCurrentActionFirstRowIndex = -1 Then
         iRowIndex = moMainTable.Rows.Count - 1
         Do
            oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
            oDataRow = moMainTable.Rows.Item(iRowIndex)
            iRowStatus = zzToRowStatus(oDataRow)
            oGridRow.Cells.Item(miForcedAreaColIndex).Style = moDataGridViewCellStyleInput
            mhsInputCells.Add(New CellKey(iRowIndex, miForcedAreaColIndex, False))
            iRowIndex = iRowIndex - 1
         Loop Until (iRowStatus = enRowStatus.StartOfAction) OrElse (iRowIndex < 0)
      End If
   End Sub
   Private Sub zzAddInputCellsLastAction()
      If miLastActionFirstRowIndex >= 0 AndAlso miCurrentActionFirstRowIndex = -1 Then
         Select Case miCurrentActionType
            Case UnidivNet.enActionType.Divide
               zzAddInputCellsLastDivide()
            Case UnidivNet.enActionType.Union
               zzAddInputCellsLastUnion()
         End Select


      End If
   End Sub
   Private Sub zzAddInputCellsLastDivide()
      Dim oGridRow As DataGridViewRow = Nothing
      Dim oDataRow As DataRow

      For iRowIndex As Integer = miLastActionFirstRowIndex To moMainTable.Rows.Count - 1
         oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
         oDataRow = moMainTable.Rows.Item(iRowIndex)

         zzAddInputCell(oGridRow, iRowIndex, miForcedAreaColIndex, True)
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
      Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)
      If miCurrentActionType = UnidivNet.enActionType.Union Then
         mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelColIndex, True))
         mhsInputCells.Add(New CellKey(iRowIndex, miFromParcelTempColIndex, True))
         oGridRow.Cells.Item(miFromParcelColIndex).Style = moDataGridViewCellStyleInput
         oGridRow.Cells.Item(miFromParcelTempColIndex).Style = moDataGridViewCellStyleInput
      ElseIf miCurrentActionType = UnidivNet.enActionType.Divide Then
         mhsInputCells.Add(New CellKey(iRowIndex, miToParcelColIndex, True))
         ' mhsInputCells.Add(New CellKey(iRowIndex, miToGushColIndex))
         oGridRow.Cells.Item(miToParcelColIndex).Style = moDataGridViewCellStyleInput
         ' oGridRow.Cells.Item(miToGushColIndex).Style = moDataGridViewCellStyleInput

         ' oGridRow.Cells.Item(miForcedAreaColIndex).Style = moDataGridViewCellStyleInput
         'mhsInputCells.Add(New CellKey(iRowIndex, miForcedAreaColIndex))


      End If
   End Sub

   Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
      If mbEventsEnabled Then


         Dim tCellKey As CellKey = New CellKey(e.RowIndex, e.ColumnIndex, True)
         'If miCurrentActionFirstRowIndex = -1 OrElse Not mhsInputCells.Contains(tCellKey) Then
         If mhsInputCells.Contains(tCellKey) Then
            mtEditingCellKey = tCellKey
            Select Case e.ColumnIndex
               Case 2, 3
                  ' DMCommon.Debug.MsgBox("09_877", e.RowIndex.ToString() & ":" & e.ColumnIndex.ToString(), moMainTable.Rows.Count)
                  Dim oDataRow As DataRow = Nothing
                  If e.RowIndex < moMainTable.Rows.Count Then
                     oDataRow = zzGetDataRow(e.RowIndex)
                     mtEditingParcelKey = zzParcelKeyFrom(oDataRow)
                  Else
                     mtEditingParcelKey = New UD_ParcelKey()
                  End If
            End Select
         Else
            e.Cancel = True
         End If
      End If '
   End Sub
   Private o As System.Collections.Generic.IEqualityComparer(Of CellKey)
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
      Public Function FindPolygon(tPoint As Autodesk.AutoCAD.Geometry.Point3d) As String
         Dim oPolygon As Polygon = Nothing
         Dim sParcelName As String = Nothing
         For iIndex As Integer = 0 To moaOpenedTopos.GetUpperBound(0)
            Try
               oPolygon = moaOpenedTopos(iIndex).FindPolygon(tPoint)
               If oPolygon IsNot Nothing Then
                  Exit For
               End If
            Catch oEx As Exception

            End Try

         Next
         If oPolygon IsNot Nothing Then
            Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)

            Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
            oAcadBlock.Fields = {"PARCEL_NAME"}
            oAcadBlock.OpenForRead()

            '  DMCommon.Debug.MsgBox("09_991a", oPolygon.Entity)


            tBlockRefData = oAcadBlock.GetBlockRefData(oPolygon.Entity)
            '    DMCommon.Debug.MsgBox("09_991b")
            sParcelName = tBlockRefData.AttribValues(0)
         End If


         Return sParcelName


      End Function
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

      '  Private miParcelNo As Integer
      Private mtParcelKey As UD_ParcelKey
      Private mtSourceParcelKey As UD_ParcelKey




      Private mlHandleVal As Long
      Private mdCentroidX As Double
      Private mdCentroidY As Double
      Private mdArea As Double





      Public Sub New(oDataRow As DataRow)
         Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ParcelNo"))
         Dim iOriginalBlockNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("OriginalBlockNo"))
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
            mtParcelKey = tValue
         End Set
      End Property
      Public Property SourceParcelKey As UD_ParcelKey
         Get
            Return mtSourceParcelKey
         End Get
         Set(tValue As UD_ParcelKey)
            mtSourceParcelKey = tValue
         End Set
      End Property
      Public ReadOnly Property SourceParcelNo As Integer
         Get
            Return mtSourceParcelKey.ParcelNo
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
      Private mbDBVersionIsCorrect As Boolean = True
      Private mbTopoIDChanged As Boolean = False
      Private mdicDWGIds As Dictionary(Of Integer, Integer)
      '  Private moaFragmentsTemp() As Fragment
      ' Private moaFragmentPgonScheme() As tsPolygon
      Private moaFragments() As Fragment

      Private miaParcelNo() As Integer
      Private miCheckedCounter As Integer
      Public Function TryGetParcel(iFragmentID As Integer, ByRef tParcelKey As UD_ParcelKey) As Boolean
         Dim oFragment As Fragment = Nothing
         If mdicByTopoID.TryGetValue(iFragmentID, oFragment) Then
            ' DMCommon.Debug.MsgBox("12_054", iFragmentID, oFragment.DWGTopoID, oFragment.ParcelKey)
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
            '  DMCommon.Debug.MsgBox("12_231", "Fragment #" & iFragmentID.ToString(), tParcelKey)
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
      Public Sub AddFragment(oFragment As Fragment)
         '  DMCommon.Debug.MsgBox("12_229", "Fragment #" & oFragment.HandleVal, "FragmentDB #" & oFragment.DBTopoID, "FragmentDWG #" & oFragment.DWGTopoID)
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
      'Public Sub CheckByScheme(oFragmentPgonScheme As tsPolygon, iParcelNo As Integer)
      '   Dim oDBObject As DBObject
      '   Dim lHandleVal As Long
      '   Dim oFragment As Fragment = Nothing
      '   If mbDBVersionIsCorrect Then
      '      oDBObject = DMAcadExt.AcadTransaction.GetDBObject(oFragmentPgonScheme.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      '      If oDBObject IsNot Nothing Then
      '         lHandleVal = oDBObject.Handle.Value
      '         If Me.TryGetValue(lHandleVal, oFragment) AndAlso oFragment.ParcelNo = iParcelNo Then
      '            oFragment.DWGTopoID = oFragmentPgonScheme.ID
      '            moaFragments(miCheckedCounter) = New Fragment(oFragmentPgonScheme, iParcelNo)
      '            miaParcelNo(miCheckedCounter) = iParcelNo
      '            miCheckedCounter += 1
      '         Else
      '            mbDBVersionIsCorrect = False
      '         End If
      '      End If
      '   Else
      '      oFragment = New Fragment(oFragmentPgonScheme, iParcelNo)
      '   End If
      '   Me.SetTopoIDChanged(oFragment.TopoIDChanged)
      '   mbDBVersionIsCorrect = False
      '   If oFragment.ParcelNo = iParcelNo Then
      '      oFragment.DWGTopoID = oFragmentPgonScheme.ID
      '      Me.SetTopoIDChanged(oFragment.TopoIDChanged)
      '   Else
      '      Me.DBVersionIsCorrect = False
      '   End If
      'End Sub
      Public Sub CheckByDB(oDataRow As DataRow)


         Dim oFragment As Fragment = Nothing
         Dim oFragmentDB As Fragment = Nothing

         If mbDBVersionIsCorrect Then
            oFragmentDB = New Fragment(oDataRow)
            '   DMCommon.Debug.MsgBox("12_101p", DBVersionIsCorrect, oFragmentDB.HandleVal, oFragmentDB.ParcelNo)
            If Me.TryGetValue(oFragmentDB.HandleVal, oFragment) Then   'AndAlso oFragment.ParcelNo = oFragmentDB.ParcelNo
               '  DMCommon.Debug.MsgBox("12_101q", DBVersionIsCorrect, oFragmentDB.HandleVal, oFragment.ParcelNo, oFragmentDB.ParcelNo)
               If oFragment.ParcelNo = oFragmentDB.ParcelNo Then
                  '   DMCommon.Debug.MsgBox("12_101r", DBVersionIsCorrect, oFragmentDB.HandleVal, oFragment.ParcelNo, oFragmentDB.ParcelNo)
                  oFragment.DBTopoID = oFragmentDB.DBTopoID
                  SetTopoIDChanged(oFragment.TopoIDChanged)

                  mdicDWGIds.Add(oFragment.DBTopoID, oFragment.DWGTopoID)
               Else
                  mbDBVersionIsCorrect = False

               End If

            Else
               mbDBVersionIsCorrect = False
            End If

         End If







      End Sub

      Public Sub zzZZ(ByRef oFragmentTable As System.Data.DataTable)

      End Sub
      Private Sub zz()


      End Sub
      Public Function GetDWGTopoID(iDBTopoID As Integer) As Integer
         If mbTopoIDChanged AndAlso mdicDWGIds IsNot Nothing Then
            Dim iDWGTopoID As Integer
            If mdicDWGIds.TryGetValue(iDBTopoID, iDWGTopoID) Then
               Return iDWGTopoID
            Else
               Return 0
            End If

         ElseIf Not mbTopoIDChanged Then
            Return iDBTopoID
         Else
            Return 0
         End If
      End Function

      Public Sub CreateDWGDic()
         If mbTopoIDChanged Then
            mdicDWGIds = New Dictionary(Of Integer, Integer)()
            For Each oFragment As Fragment In MyBase.Values
               mdicDWGIds.Add(oFragment.DBTopoID, oFragment.DWGTopoID)
            Next
         End If
      End Sub

      Public Sub New()
         mdicByTopoID = New Dictionary(Of Integer, Fragment)()
      End Sub
   End Class
   Private Sub dgvMain_RowsAdded(oSender As System.Object, e As DataGridViewRowsAddedEventArgs) Handles dgvMain.RowsAdded
      If mbEventsEnabled Then
         ' MessageBox.Show(e.RowIndex.ToString() & ":" & e.RowCount.ToString(), "09_878")
         zzAddInputCellsNextRow(e.RowIndex)
      End If

   End Sub

   Private Sub rdbUnion_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbUnion.CheckedChanged
      If Me.rdbUnion.Checked Then
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
            If mcolNewLayerCentroidsBlocks IsNot Nothing AndAlso mcolNewLayerCentroidsBlocks.Count > 0 Then
               Me.rdbHanit.Checked = True
            End If
         End If
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
            zzFinalTopo()
            zzCloseDWG()
            '  zzCreateFinalTopology()

            'DMAcadExt.AcadTransaction.CloseModelSpace()
            'DMAcadExt.AcadTransaction.Terminate()
            'DMAcadExt.AcadDocument.Unlock()
         End If

      End If

   End Sub
   Private Sub cmdAllFragments_Click_NoDB(oSender As System.Object, e As EventArgs) 'Handles cmdAllFragments.Click

      Dim oFirstActionRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)

      zzSetNewPointNo()
      If miCurrentActionType = UnidivNet.enActionType.Divide Then

         Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockNo, oFirstActionRow.Cells.Item(2).Value, oFirstActionRow.Cells.Item(3).Value)
         DMCommon.Debug.MsgBox("08_547x", tParcelKey)

         zzBeforeDivide(True)
         mdicParcels.DebugMsg()
      ElseIf miCurrentActionType = UnidivNet.enActionType.Union Then

         zzBeforeUnion(mdicParcels.Values)
         mdicParcels.DebugMsg()
      End If
      '   UnidivNet.UD_App.PrintPointsInfo()

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

      zzSetNewPointNo()
      If miCurrentActionType = UnidivNet.enActionType.Divide Then

         '  Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(oFirstActionRow.Item("OriginalParcelNo"), oFirstActionRow.Item("NewParcelNo"))
         '  Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)


         '  DMCommon.Debug.MsgBox("08_547", tParcelKey)

         zzBeforeDivide(True)
         '''''''''''''  zzDivideParcel(True)
         ''''''''''  mdicParcels.DebugMsg()
      ElseIf miCurrentActionType = UnidivNet.enActionType.Union Then
         Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = zzGridToParcels()
         zzOpenDWG()
         If colParcels.Count = 0 Then
            zzBeforeUnion(mdicParcels.Values)
         Else
            zzBeforeUnion(colParcels)
         End If
         zzCloseDWG()
         ''''''''''   mdicParcels.DebugMsg()
      ElseIf miCurrentActionType = UnidivNet.enActionType.Transfer Then
         ' DMCommon.Debug.MsgBox("08_577bef", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)

         zzOpenDWG()
         zzBeforeTransfer()

         DMAcadExt.AcadDocument.Regen()
         zzCloseDWG()
         zzCloseAction()

         ' DMCommon.Debug.MsgBox("08_577a", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)
      End If
      '   UnidivNet.UD_App.PrintPointsInfo()
   End Sub
   Private Sub zzInsertTransferCentroid()
      Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      Dim tSourceParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
      Dim oDestParcel As UnidivNet.UD_Parcel
      Dim tDestParcelKey As UD_ParcelKey
      '  Dim sParcelNo As String = DirectCast(oFirstActionRow.Item("ctxToParcel"), String)
      Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
      Dim iGushNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockNo"))
      Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
      Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim sLayer As String = zzGetCentroidLayer()
      Dim bCurrentLayerOK As Boolean
      '  Integer.TryParse(sParcelNo, iParcelNo)
      '   MessageBox.Show(sParcelNo, "09_866")
      mtCurrentParcelKey = New UD_ParcelKey(iGushNo, iParcelNo, False)
      '  mtCurrentParcelKey.DebugMsg("BeforeDivide")

      Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
      If mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then

         tDestParcelKey = New UD_ParcelKey(iGushNo, iParcelNo, False)
         oDestParcel = New UnidivNet.UD_Parcel(oSourceParcel, tDestParcelKey)
         oDestParcel.Stage = miCurrentStage

         Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
         oAcadBlock.OpenForRight()
         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()

         oAcadBlock.Fields = {"PARCEL_NAME", "GUSH", "PARCEL_PREVIOUS", "GUSH_PREVIOUS", "LEGAL_AREA", "CALC_AREA"}



         tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(oSourceParcel.CenterPosition.AcGePoint3d.X + 0.5, oSourceParcel.CenterPosition.AcGePoint3d.Y + 0.5, 0.0) ' + 2.0  + 2.0
         tBlockRefData.AttribValues = {oDestParcel.UD_Name, oDestParcel.BlockFull, tParcelKey.UD_ParcelName, tParcelKey.BlockName, oSourceParcel.LegalArea.ToString(), FormatNumber(oSourceParcel.ParcelArea.AcadAreaD, 3)}



         bCurrentLayerOK = DMAcadExt.AcadTransaction.CreateLayer(sLayer)
         If bCurrentLayerOK Then
            tBlockRefData.Layer = sLayer
         End If

         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)

         tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)

         oFirstActionRow.Item("AcObjID") = tCentroidBlockAcObjID
         oFirstActionRow.Item("ParcelDbID") = oDestParcel.DbID
         oDestParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)

         mdicParcels.AddParcel(oDestParcel)



         '     DMCommon.Debug.MsgBox("11_140", oSourceParcel.UD_Name)
      End If
   End Sub

   Private Sub zzBeforeUnionNoDB(colParcels As ICollection(Of UnidivNet.UD_Parcel))
      Dim oGridRow As DataGridViewRow = Nothing
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      '    Dim hsUnion As HashSet(Of Integer) = New HashSet(Of Integer)
      Dim hsFragmentsUnion As HashSet(Of Integer) = New HashSet(Of Integer)
      Dim iParcelNo As Integer
      Dim oFirstActionRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
      Dim sParcelNo As String = DirectCast(oFirstActionRow.Cells.Item("ctxToParcel").Value, String)

      Integer.TryParse(sParcelNo, iParcelNo)
      '   MessageBox.Show(sParcelNo, "09_866")
      mtCurrentParcelKey = New UD_ParcelKey(iParcelNo, False)
      mtCurrentParcelKey.DebugMsg("BeforeUnion!!!")

      Dim tResParcelArea As ParcelArea
      Dim oResParcel As UnidivNet.UD_Parcel
      oGridRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
      Dim colParcelArea As System.Collections.ObjectModel.Collection(Of ParcelArea) = New System.Collections.ObjectModel.Collection(Of ParcelArea)()

      For Each oParcel As UnidivNet.UD_Parcel In colParcels
         '   MessageBox.Show(oParcel.ParcelKey.ToString(), "09_677")
         '   DMCommon.Debug.MsgBox("09_769b", oParcel.ParcelKey, mdicParcels.Count, colParcels.Count, oParcel.Fragments.Count)
         oParcel.IsCanceled = True
         '  hsUnion.UnionWith(oParcel.Fragments)
         'hsUnion.Add(oParcel.TopoID)
         ' mdicParcels.AddParcel(oParcel)
         '   DMCommon.Debug.MsgBox("09_769a", oParcel.ParcelKey, mdicParcels.Count)
         hsFragmentsUnion.UnionWith(oParcel.Fragments)
         DMCommon.Debug.MsgBox("09_753")
         '        DMCommon.Debug.MsgBox("09_662", oParcel.Fragments.Count, hsFragmentsUnion.Count)
         DMAcadExt.AcadDocument.WriteDebugMessage("Area= " & oParcel.ParcelArea.LegalArea & "; " & oParcel.ParcelArea.AcadArea & "; " & oParcel.ParcelArea.CalcArea)
         colParcelArea.Add(oParcel.ParcelArea)
         oGridRow = zzGetRow(iRowIndex)
         DMCommon.Debug.MsgBox("09_754")
         '  mdicRows.Add(iRowIndex, New UD_Row(miCurrentStage))
         oParcel.ParcelKey.ToGrid(oGridRow, True)
         iRowIndex += 1
      Next
      DMCommon.Debug.MsgBox("09_760", colParcels.Count, hsFragmentsUnion.Count)


      'Dim oResParcel As UnidivNet.UD_Parcel = New UnidivNet.UD_Parcel()

      tResParcelArea.CalculateArea(colParcelArea, miRoundDigit)
      '  DMAcadExt.AcadDocument.WriteDebugMessage("ResArea= " & tResParcelArea.LegalArea & "; " & tResParcelArea.AcadArea & "; " & tResParcelArea.CalcArea)
      ''''''''''''''''''''''''''''''''''''''''''    oResParcel = New UnidivNet.UD_Parcel(mtCurrentParcelKey, hsFragmentsUnion, tResParcelArea)
      '''''''''''''''''''''''''''''''''''''''''''''''    mdicParcels.AddParcel(oResParcel)
      '    DMCommon.Debug.MsgBox("09_883", oResParcel.Name, mtCurrentParcelKey.ParcelNo, mtCurrentParcelKey.ParcelNo, mtCurrentParcelKey.Origin)
      zzFillParcelArea(tResParcelArea, oFirstActionRow)

      '  oFirstActionRow()
      MessageBox.Show("", "09_686a")
      zzUnion(hsFragmentsUnion, tResParcelArea, "")
      mtCurrentParcelKey.NextParcel()
      '   oGridRow.DividerHeight = 14
      zzCloseAction()
      zzWriteParcelsInfo("After Union")

   End Sub
   Private Sub zzParcelsToGrid(colParcels As ICollection(Of UnidivNet.UD_Parcel))
      Dim oDataRow As DataRow = Nothing
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      '    DMCommon.Debug.MsgBox("11_164", colParcels.Count)
      For Each oSourceParcel As UnidivNet.UD_Parcel In colParcels
         oDataRow = zzGetDataRow(iRowIndex)
         zzFillFromParcelKeyNew(oSourceParcel, oDataRow, Nothing)
         ' DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))

         iRowIndex += 1
      Next
   End Sub
   Private Function zzGridToParcels() As ObjectModel.Collection(Of UnidivNet.UD_Parcel)
      Dim oDataRow As DataRow = Nothing
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

      For iRowIndex = miCurrentActionFirstRowIndex To moMainTable.Rows.Count - 1
         oDataRow = zzGetDataRow(iRowIndex)
         tParcelKey = zzParcelKeyFrom(oDataRow)
         'DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))
         If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
            colParcels.Add(oParcel)
         End If

      Next
      '  DMCommon.Debug.MsgBox("11_167s", colParcels.Count)
      Return colParcels
   End Function
   Private Sub zzBeforeUnion(colParcels As ICollection(Of UnidivNet.UD_Parcel))
      '  DMCommon.Debug.MsgBox("11_607a", mdicParcels.Count)
      Dim oDataRow As DataRow = Nothing

      '   Dim oGridRow As DataGridViewRow = Nothing

      Dim oActionFirstRow As DataRow = Nothing

      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      '    Dim hsUnion As HashSet(Of Integer) = New HashSet(Of Integer)
      Dim hsFragmentsUnion As HashSet(Of Integer) = New HashSet(Of Integer)

      Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      '    Dim sParcelNo As String = DirectCast(oFirstActionRow.Cells.Item("ctxToParcel").Value, String)
      Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
      '   Integer.TryParse(sParcelNo, iParcelNo)
      '   MessageBox.Show(sParcelNo, "09_866")
      mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, iParcelNo, False)
      '   mtCurrentParcelKey.DebugMsg("BeforeUnion!!!")

      Dim tResParcelArea As ParcelArea
      Dim sPrevParcelName As String = Nothing
      '   Dim oResParcel As UnidivNet.UD_Parcel
      oActionFirstRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      Dim colParcelArea As System.Collections.ObjectModel.Collection(Of ParcelArea) = New System.Collections.ObjectModel.Collection(Of ParcelArea)()

      'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      'DMAcadExt.AcadTransaction.Start()
      'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


      '  DMCommon.Debug.MsgBox("09_676", colParcels.Count)
      For Each oSourceParcel As UnidivNet.UD_Parcel In colParcels
         ' DMCommon.Debug.MsgBox("09_677", oParcel.ParcelKey.ToString(), oParcel.CentroidAcObjID)
         '   DMCommon.Debug.MsgBox("09_769b", oParcel.ParcelKey, mdicParcels.Count, colParcels.Count, oParcel.Fragments.Count)
         oSourceParcel.IsCanceled = True

         oSourceParcel.UpdateBlockAttributes()

         If sPrevParcelName Is Nothing Then
            sPrevParcelName = oSourceParcel.UD_Name
         Else
            sPrevParcelName &= "," & oSourceParcel.UD_Name
         End If

         '  hsUnion.UnionWith(oParcel.Fragments)
         'hsUnion.Add(oParcel.TopoID)
         ' mdicParcels.AddParcel(oParcel)
         '  DMCommon.Debug.MsgBox("09_769a!!", oSourceParcel.ParcelKey, mdicParcels.Count, oSourceParcel.Fragments.Count)
         hsFragmentsUnion.UnionWith(oSourceParcel.Fragments)
         '        DMCommon.Debug.MsgBox("09_662", oParcel.Fragments.Count, hsFragmentsUnion.Count)
         ' DMCommon.Debug.MsgBox("11_275", "Area= " & oSourceParcel.ParcelArea.LegalArea & "; " & oSourceParcel.ParcelArea.AcadArea & "; " & oSourceParcel.ParcelArea.CalcArea)
         colParcelArea.Add(oSourceParcel.ParcelArea)
         '     oGridRow = zzGetRow(iRowIndex)
         oDataRow = zzGetDataRow(iRowIndex)
         '  mdicRows.Add(iRowIndex, New UD_Row(miCurrentStage))
         oSourceParcel.ParcelKey.ToJournalDataRow(oDataRow, True)
         ' DMCommon.Debug.MsgBox("11_165", oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"), oDataRow.Item("DestParcelNo"))

         iRowIndex += 1
      Next
      moLastActionRow = oDataRow
      oDataRow.Item("RowStatus") = enRowStatus.EndOfAction

      '   zzCloseLastRow()
      '   DMCommon.Debug.MsgBox("09_760", colParcels.Count, hsFragmentsUnion.Count)


      'Dim oResParcel As UnidivNet.UD_Parcel = New UnidivNet.UD_Parcel()

      tResParcelArea.CalculateArea(colParcelArea, miRoundDigit)

      '  DMCommon.Debug.MsgBox("11_188", "ResArea= " & tResParcelArea.LegalArea & "; " & tResParcelArea.AcadArea & "; " & tResParcelArea.CalcArea)
      ''''''''''''''''''''''''''''''''''''''''''    oResParcel = New UnidivNet.UD_Parcel(mtCurrentParcelKey, hsFragmentsUnion, tResParcelArea)
      '''''''''''''''''''''''''''''''''''''''''''''''    mdicParcels.AddParcel(oResParcel)
      '    DMCommon.Debug.MsgBox("09_883", oResParcel.Name, mtCurrentParcelKey.ParcelNo, mtCurrentParcelKey.ParcelNo, mtCurrentParcelKey.Origin)
      zzFillParcelArea(tResParcelArea, True, oFirstActionRow)
      '  DMCommon.Debug.MsgBox("11_607b", mdicParcels.Count)

      '  DMCommon.Debug.MsgBox("09_686b", hsFragmentsUnion.Count.ToString(), tResParcelArea.AcadArea)

      zzUnion(hsFragmentsUnion, tResParcelArea, sPrevParcelName)

      mtCurrentParcelKey.NextParcel()
      '   oGridRow.DividerHeight = 14
      zzCloseAction()
      '''''''  zzWriteParcelsInfo("After Union")

      'DMAcadExt.AcadTransaction.CloseModelSpace()

      'DMAcadExt.AcadTransaction.Terminate()
      'DMAcadExt.AcadDocument.Unlock()
      'DMAcadExt.AcadDocument.UpdateScreen()

   End Sub
   Private Sub zzLoadJournalTable(bSchemaOnly As Boolean)
      '  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      '  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

      Const sSPName As String = "GetJournalData"

      moMainTable = New System.Data.DataTable("Journal")

      moJournalDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)
      If bSchemaOnly Then
         ' moJournalDataAdapter.FillSchema(moMainTable, SchemaType.Source)
         moJournalDataAdapter.FillSchema(moMainTable, SchemaType.Source)

      Else
         moJournalDataAdapter.Fill(moMainTable)

      End If
      mbNewData = bSchemaOnly
      zzAddCalcFields()
      Me.dgvMain.DataSource = moMainTable
      mbDatabound = True
      mbEventsEnabled = False
      Me.chkDataBound.Checked = True
      '    mbEventsEnabled = True
      '    DMCommon.Debug.MsgBox("09_949", moMainTable.Rows.Count, mbNewData, miProjectCode, miDetailNo, miBlockNo, miBlockAddNo)
      '  moMainTable = ShopData.ShopDataManager.GetDataTable(sComText, System.Data.CommandType.Text, "Main")
      '    moDataAdapter.Fill(moMainTable)


   End Sub
   Private Sub zzLoadParcelLogTable(bSchemaOnly As Boolean)
      '  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      '  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

      Const sSPName As String = "GetParcelLogData"
      Dim oaParams(3) As System.Data.Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing

      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, miBlockNo)
      oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)


      moParcelLogTable = New System.Data.DataTable("ParcelLog")

      moParcelLogDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, oaParams, True, String.Empty, True, True)
      If bSchemaOnly Then
         ' moParcelLogDataAdapter.FillSchema(moParcelLogTable, SchemaType.Source)
         moParcelLogDataAdapter.FillSchema(moParcelLogTable, SchemaType.Source)

      Else
         moParcelLogDataAdapter.Fill(moParcelLogTable)

      End If
      moParcelLogTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))

   End Sub
   Private Sub zzLoadFragmentsTable(bSchemaOnly As Boolean)
      '  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      '  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
      Const sSPName As String = "GetFragments"

      moFragmentTable = New System.Data.DataTable("Fragments")
      moFragmentsDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)

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

      moParcelFragmentTable = New System.Data.DataTable("ParcelFragments")
      moParcelFragmentsDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, "", True, True)

      If bSchemaOnly Then
         moParcelFragmentsDataAdapter.FillSchema(moParcelFragmentTable, SchemaType.Source)
      Else
         moParcelFragmentsDataAdapter.Fill(moParcelFragmentTable)
      End If
      ' moFragmentTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))

   End Sub
   Private Sub zzLoadProjectDataTable()
      '  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      '  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
      Dim sComText As String = "Select PlanType FROM dbo.Ud_ProjectData WHERE (ProjectCode = " & miProjectCode & ") AND (Detail = " & miDetailNo & ")"
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

      Dim s As String = String.Empty

      If oDataReader IsNot Nothing Then
         If oDataReader.HasRows Then

            Do While oDataReader.Read

               If oDataReader.IsDBNull(0) Then
                  miPlanType = miPlanTypeDflt

               Else
                  miPlanType = oDataReader.GetInt32(0)

               End If
            Loop
            oDataReader.Close()
         Else
            miPlanType = miPlanTypeDflt
         End If
      Else
         miPlanType = miPlanTypeDflt
      End If


      Me.Label4.Text = CStr(miPlanType)
      Me.Text = msCaption & " - " & frmUD_General.GetPlanTypeName(miPlanType)

   End Sub

   Private Sub zzCheckFragments()
      '  Dim oFragment As Fragment
      If mdicFragments IsNot Nothing AndAlso moFragmentTable IsNot Nothing Then
         '  DMCommon.Debug.MsgBox("11_341", mdicFragments.Count, moFragmentTable.Rows.Count)
         mdicFragments.CheckByDBPolygonCount(moFragmentTable.Rows.Count)
         '   DMCommon.Debug.MsgBox("12_101k", mdicFragments.DBVersionIsCorrect)
         For Each oDataRow As DataRow In moFragmentTable.Rows
            mdicFragments.CheckByDB(oDataRow)
         Next
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
      moFragmentsToposcheme = New TopoManager.TopoScheme.tsTopology(msFragmentsTopoName)

      If moFragmentsToposcheme.Load(False, moFragmentTopology) Then
         mdicPoints = New UnidivNet.UD_Points()
         mdicFLines = New UnidivNet.UD_FLines()
         '    DMCommon.Debug.MsgBox("09_109A", "zzLoadFragments", moFragmentsToposcheme.Polygons.Count)
         For Each oNode As tsNode In moFragmentsToposcheme.Nodes
            If oNode.AcObjID.IsNull Then
               DMCommon.Debug.MsgBox("09_110", " oNode.AcObjID.IsNull", oNode.Location)
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
            Else
               DMCommon.Debug.MsgBox("09_111", " oBlockRef Is Nothing", oNode.Location)
            End If
         Next
         For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
            oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
            oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            oFLine = New UnidivNet.UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
            mdicFLines.Add(oBranch.AcObjID, oFLine)
         Next
         Dim iUndef, i0 As Integer
         For Each oPoint As UnidivNet.UD_Point In mdicPoints.Values
            If oPoint.Stage = -1 Then
               iUndef = iUndef + 1
            ElseIf oPoint.Stage = 0 Then
               i0 += 1
            End If
         Next
         '  DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD()
         'Dim oNewRow As DataRow = moMainTable.NewRow
         'For Each oPgon As tsPolygon In moFragmentsToposcheme.Polygons
         '   moFragmentTable.NewRow()


         '   oNewRow.Item("ProjectCode") = miProjectCode
         '   oNewRow.Item("Detail") = miDetailNo
         '   oNewRow.Item("OriginalBlockNo") = miBlockNo
         '   oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo
         '   oNewRow.Item("FragmentID") = oPgon.ID



         '   moFragmentTable.Rows.Add(oNewRow)
         'Next


         Dim iFragmentID As Integer
         Dim iFragmentHandle As Long

         Dim tFragmentHandle As Handle
         Dim tFragmentObjID As ObjectId

         DMAcadExt.AcadTransaction.OpenHandleDictionary()
         'Do While oDataReader.Read
         '   iFragmentID = oDataReader.GetInt32(0)
         '   iFragmentHandle = oDataReader.GetInt64(5)
         '   tFragmentHandle = New Handle(iFragmentHandle)
         '   tFragmentObjID = DMAcadExt.AcadTransaction.GetObjectID(tFragmentHandle)
         'Loop

         moFragmentsToposcheme.Close()
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
      '  DMCommon.Debug.MsgBox("09_682s", DMCommon.Debug.ColCount(mdicPoints), oPoint.Name, iTest, iNewPointNumber, iStage)
      ' DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
   End Function
   Public Function zzLoadStageNodes(tsStageParcels As tsTopology, iNewPointNumber As Integer) As Boolean
      Dim oPoint As UnidivNet.UD_Point = Nothing
      Dim iTest As Integer
      '  Dim sTestName As String = ""
      Dim sPointName As String = ""
      Dim bCurrentLayerOK As Boolean
      Dim sNewPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(miCurrentStage, miCurrentStage <> 0)
      Dim sOldPointLayer As String = UnidivNet.UD_App.GetStageUDPointLayer(miCurrentStage, miCurrentStage = 0)
      If sNewPointLayer IsNot Nothing Then
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewPointLayer, DMAcadExt.DMApp.AppID, True, True)
      End If
      '   DMCommon.Debug.MsgBox("09_551", tsStageParcels.Name, tsStageParcels.Nodes.Count, mdicPoints.Count)
      For Each oNode As tsNode In tsStageParcels.Nodes
         '  DMAcadExt.AcadDocument.WriteDebugMessage("#aP " & tsStageParcels.Name & "; " & oNode.IsPseudo & "; " & oNode.Location.ToString() & ", " & oNode.AcObjID.ToString())
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
                  '    DMAcadExt.AcadDocument.WriteDebugMessage("#aQ " & sPointName & "; " & oNode.IsPseudo & "; " & oPoint.Coordinates)
               End If
               If oPoint.Stage = -1 Then

                  oPoint.Stage = miCurrentStage
                  oPoint.Action = miCurrentAction

                  '  DMCommon.Debug.MsgBox("09_551M", iStage, oPoint.Name, mdicPoints.Count)
                  If miCurrentStage = 0 And miPlanType <> 1 Then
                     oPoint.CheckBlock(sOldPointLayer)

                  ElseIf String.IsNullOrEmpty(oPoint.Name) Then
                     ''''''''''''''''''''''''''    oPoint.Name = iNewPointNumber.ToString()
                     mdicPoints.SetNewPointNumber(True, oPoint)
                     iNewPointNumber += 1

                  End If
                  oPoint.UpdateBlock(sNewPointLayer)
                  '    sTestName &= sTestName & oPoint.Name & ";"
                  iTest += 1
               End If
            End If
         End If
      Next
      '   DMCommon.Debug.MsgBox("09_682", DMCommon.Debug.ColCount(mdicPoints), oPoint.Name, iTest, iNewPointNumber, iStage)
      ' DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
   End Function
   Public Shared Function zzLoadStageBranches(tsStageParcels As tsTopology, iStage As Integer) As Boolean
      Dim oFLine As UnidivNet.UD_FLine = Nothing
      '  DMCommon.Debug.MsgBox("09_551", False, tsStageParcels, tsStageParcels.Nodes, tsStageParcels.Nodes.Count, mdicPoints, mdicPoints.Count)
      For Each oBranch As tsBranch In tsStageParcels.Branches
         If mdicFLines.TryGetValue(oBranch.AcObjID, oFLine) Then
            If iStage = 2 Then
               DMAcadExt.AcadDocument.WriteDebugMessage("aX " & oFLine.Stage.ToString() & "; " & oFLine.MaxPointStage & "; " & oFLine.Name)
            End If
            If oFLine.Stage = -1 AndAlso oFLine.MaxPointStage <> -1 AndAlso oFLine.MaxPointStage <= iStage Then
               oFLine.Stage = iStage
               '  DMCommon.Debug.MsgBox("09_574", oFLine.Name, iStage)

               '  oFLine.Name = iNewPointNumber.ToString()
               '  iNewPointNumber += 1
               '  oPoint.UpdateBlock()
            End If
         End If
      Next
      '    DMCommon.Debug.MsgBox("09_552", False, tsStageParcels, tsStageParcels.Nodes)
   End Function
   Private Function zzGetParameters() As System.Data.Common.DbParameter()
      Dim oaParams(3) As System.Data.Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing

      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockNo", DbType.Int32, miBlockNo)
      oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)
      ' DMCommon.Debug.MsgBox("11_572", oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
      Return oaParams
   End Function
   Private Sub zzCalcJournalAddFields()
      Dim iStage As Integer
      Dim iAction As Integer
      Dim iActionType As UnidivNet.enActionType
      Dim iRowStatus As enRowStatus
      Dim iRowIndex As Integer = 0
      Dim oGridRow As DataGridViewRow = Nothing
      Dim tParcelArea As ParcelArea
      Dim dLegalAreaD As Double
      Dim dAcadAreaM As Double
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UnidivNet.UD_Parcel
      Dim hsFragments As HashSet(Of Integer) = New HashSet(Of Integer)()
      '   mdicParcels = New UnidivNet.UD_Parcels()
      For Each oDataRow As DataRow In moMainTable.Rows
         iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))

         iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
         iActionType = zzToActionType(oDataRow)
         iRowStatus = zzToRowStatus(oDataRow)
         dLegalAreaD = DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))
         dAcadAreaM = 1000.0 * DMCommon.Functions.CDblN(oDataRow.Item("AcadArea"))
         If dLegalAreaD > System.Double.Epsilon Then
            tParcelArea.CalculateArea(dLegalAreaD, dAcadAreaM, miRoundDigit)
            zzFillParcelArea(tParcelArea, False, oDataRow)
         End If
         tParcelKey = zzGetRowParcelKey(iActionType, oDataRow)
         If False AndAlso tParcelKey.Exists Then
            oParcel = New UnidivNet.UD_Parcel(tParcelKey, hsFragments, tParcelArea)
            oParcel.Stage = iStage
            Try
               If iStage = 0 Then
                  ''''''compare  DWG & DB
               Else
                  mdicParcels.AddParcel(oParcel)
               End If


            Catch oEx As Exception
               DMCommon.Debug.MsgBox("08_123", tParcelKey, oParcel.ParcelKey)
            End Try
         End If

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
               oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
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
   Public Function zzLoadParcelTopology() As Boolean

      'DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
      Dim sParcelTopoName As String = "Parcels" 'TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
      Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim dicSourceParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
      Dim oParcelsScheme As TopoManager.TopoScheme.tsTopology
      Dim bRes As Boolean
      UnidivNet.UD_Parcel.Initialize()
      mdicParcels = New UnidivNet.UD_Parcels()
      dicSourceParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()

      If oParcelTopology IsNot Nothing AndAlso oParcelTopology.IsComplete Then  '#12  ' False And 
         Dim oParcel As UnidivNet.UD_Parcel
         Dim colParcelPgons As Autodesk.Gis.Map.Topology.PolygonCollection = oParcelTopology.GetPolygons()
         Dim iParcelUB As Integer = colParcelPgons.Count - 1
         '    Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
         Dim iPgonIndex As Integer = 0

         Dim oaParcels(iParcelUB) As UnidivNet.UD_Parcel

         '   UnidivNet.UD_Parcel.CreateMainDataTable()
         If UnidivNet.UD_Parcel.AcadBlock.AttributeIndices IsNot Nothing Then




            If True Then '#11
               '   DMCommon.Debug.MsgBox("07_501cdX", colPolygons.Count.ToString())
               For Each oPolygon As Polygon In colParcelPgons
                  oParcel = New UnidivNet.UD_Parcel(oPolygon)
                  oParcel.Stage = 0
                  ' oParcel.IsOrigin = True
                  '  DMCommon.Debug.MsgBox("11_505A", miBlockNo, oParcel.IsOriginal, oParcel.IsCanceled, oParcel.ParcelKey.UD_ParcelName, oParcel.BlockNo, oParcel.Name, oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.ParcelNo)
                  If miBlockNo = 0 Then
                     miBlockNo = oParcel.BlockNo
                     miBlockAddNo = oParcel.BlockAdd
                  ElseIf (miBlockNo <> oParcel.BlockNo) OrElse (miBlockAddNo <> oParcel.BlockAdd) Then
                     If oParcel.BlockNo <> 0 Then
                        DMCommon.Debug.MsgBox("Error #367", miBlockNo, miBlockAddNo, oParcel.BlockNo, oParcel.BlockAdd)
                     End If


                     ' DMCommon.Debug.MsgBox("11_588", miBlockNo, oParcel.BlockNo, oParcel.ParcelKey.ParcelNo)
                  End If
                  oParcel.Calc()
                  '     DMCommon.Debug.MsgBox("11_507", "zzLoadParcelTopology", oParcel.Correct)
                  If oParcel.Correct Then 'TEMP

                     '  mdicParcels.AddParcel(oParcel)
                     oaParcels(iPgonIndex) = oParcel
                     '  oParcel.AddDataToMainTable()
                     dicSourceParcels.Add(oPolygon.ID, oParcel)
                  End If
                  iPgonIndex += 1
                  '		DMAcadExt.AcadDocument.WriteDebugMessage("N Count=" & CStr(oParcel.Neighbors.Count))
                  oPolygon.Dispose()
                  oPolygon = Nothing
                  'oParcel.Terminate()
                  ' oParcel = Nothing
               Next
            End If  '#11
            If True Then '#14
               For Each oSourceParcel As UnidivNet.UD_Parcel In oaParcels
                  oSourceParcel.BlockNo = miBlockNo
                  oSourceParcel.BlockAdd = miBlockAddNo
                  '   DMCommon.Debug.MsgBox("11_591A", oSourceParcel.BlockNo, oSourceParcel.ParcelKey.ParcelNo, oSourceParcel.ParcelKey.UD_ParcelName, oSourceParcel.Name, oSourceParcel.IsOriginal, oSourceParcel.IsCanceled)
                  oSourceParcel.UpdateBlockAttributes()
                  oSourceParcel.Stage = 0
                  mdicParcels.AddParcel(oSourceParcel)
               Next
            End If '#14

            If True Then  '#13
               oParcelsScheme = New TopoManager.TopoScheme.tsTopology()
               If oParcelsScheme.Load(mbCheckExtended, oParcelTopology) Then
                  For Each oNode As tsNode In oParcelsScheme.Nodes
                     Select Case oNode.BlockName.ToUpper
                        Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                           oNode.HasOldPoint = True
                     End Select
                  Next
                  If mbCheckExtended Then
                     oParcelsScheme.RemovePseudoPoints()
                  End If
                  '     DMCommon.Debug.MsgBox("11_532", "zzLoadStageNodes(moParcelsScheme, 0, -1)")
                  If True Then
                     zzLoadStageNodes(oParcelsScheme, -1)
                     '     DMCommon.Debug.MsgBox("11_532after", "zzLoadStageNodes(moParcelsScheme, 0, -1)")
                     zzLoadStageBranches(oParcelsScheme, 0)
                  End If
                  Me.txtGushNo.Text = Convert.ToString(miBlockNo)
                  Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(0)
                  Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)
                  Dim oParcelPgon As Autodesk.Gis.Map.Topology.Polygon
                  ' Dim oDestParcel As UnidivNet.UD_Parcel
                  DMAcadExt.AcadTransaction.ClearLayerByClassName(sPolylineLayer)
                  oParcelsScheme.CreateDBPolylineMPlus(True)
                  If True Then
                     For Each oPolygonScheme As tsPolygon In oParcelsScheme.Polygons
                        If Not oPolygonScheme.BorderObjID.IsNull AndAlso dicSourceParcels.TryGetValue(oPolygonScheme.ID, oParcel) Then
                           oParcel.BorderObjID = oPolygonScheme.BorderObjID
                           oParcel.PolygonScheme = oPolygonScheme
                        End If
                     Next
                  End If
                  If True Then '#16
                     For Each oParcel In mdicParcels.Values
                        oParcel.PolygonScheme = oParcelsScheme.GetPolygon(oParcel.TopoID)
                     Next
                  End If '#16
                  '''''''''''''''''''''''''''''  mdicParcels.LoadTopoScheme(oParcelTopology)
                  '  Dim oNewRow As DataRow
                  mdicFragments = New Fragments()
                  Dim oFragment As Fragment = Nothing
                  ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''     mdicFragments.CheckByCount(moFragmentsToposcheme.Polygons.Count)
                  If True Then '#15
                     For Each oFragmentPgonScheme As tsPolygon In moFragmentsToposcheme.Polygons
                        Try
                           oParcelPgon = oParcelTopology.FindPolygon(oFragmentPgonScheme.Centroid) ''''
                           If oParcelPgon IsNot Nothing Then
                              oParcel = dicSourceParcels.Item(oParcelPgon.ID)
                              oParcel.AddFragment(oFragmentPgonScheme.ID)
                              ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''  mdicFragments.CheckByScheme(oFragmentPgonScheme, oParcel.ParcelKey.ParcelNo)
                              ''''''''''''''    mdicParcels.AddFragment(oFragmentPgonScheme.ID, oParcel.ParcelKey)
                              oFragment = New Fragment(oFragmentPgonScheme, oParcel.ParcelKey)
                              mdicFragments.AddFragment(oFragment)
                           End If
                        Catch oEx As Exception

                        End Try
                        oParcel = Nothing
                     Next
                  End If  '#15
                  '  DMCommon.Debug.MsgBox("!!mdicParcels.Count", mdicParcels.Count, iParcelUB)
                  '	oParcelTopology.Dispose()
                  dicSourceParcels.Clear()
                  dicSourceParcels = Nothing
                  bRes = True
               Else
                  bRes = False
               End If
            End If  '#13
         Else
            System.Windows.Forms.MessageBox.Show("Parcel Block is wrong")
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
      Return bRes
   End Function

   Private Sub zzBeforeDivideNoDB(bAll As Boolean, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      Dim oGridRow As DataGridViewRow = Nothing
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      Dim oFirstActionRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
      Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockNo, oFirstActionRow.Cells.Item(2).Value, oFirstActionRow.Cells.Item(3).Value)
      Dim sParcelNo As String = DirectCast(oFirstActionRow.Cells.Item("ctxToParcel").Value, String)
      Dim iParcelNo As Integer

      Integer.TryParse(sParcelNo, iParcelNo)
      '   MessageBox.Show(sParcelNo, "09_866")
      mtCurrentParcelKey = New UD_ParcelKey(iParcelNo, False)
      '  mtCurrentParcelKey.DebugMsg("BeforeDivide")
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
         '   MessageBox.Show(oParcel.Fragments.Count.ToString(), "09_675")
         oGridRow = zzGetRow(iRowIndex)
         oParcel.ParcelKey.ToGrid(oGridRow, True)
         ' DMCommon.Debug.MsgBox("08_549", tParcelKey, oParcel.ParcelKey)
         ''''''''''''''''''   zzDivideAllBranches(oParcel, bAll, colTopoLinks)
      Else
         DMCommon.Debug.MsgBox("08_551", mdicParcels.Count)
      End If
   End Sub
   Private Sub zzBeforeDivide(bAll As Boolean)
      Const sMsg1 As String = "החלקה לחלוקה לא נמצאת"
      Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
      Dim oGridRow As DataGridViewRow = Nothing
      Dim iRowIndex As Integer = miCurrentActionFirstRowIndex
      Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      ' Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(oFirstActionRow.Item("OriginalParcelNo"), oFirstActionRow.Item("NewParcelNo"))
      Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)


      '  Dim sParcelNo As String = DirectCast(oFirstActionRow.Item("ctxToParcel"), String)
      Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
      '  Integer.TryParse(sParcelNo, iParcelNo)
      '   MessageBox.Show(sParcelNo, "09_866")
      mtCurrentParcelKey = New UD_ParcelKey(miBlockNo, iParcelNo, False)
      '  mtCurrentParcelKey.DebugMsg("BeforeDivide")


      If mdicParcels.TryGetValue(tParcelKey, oSourceParcel) Then

         oGridRow = zzGetRow(iRowIndex)
         oSourceParcel.ParcelKey.ToGrid(oGridRow, True)
         '
         zzOpenDWG(True, False)
         DMAcadExt.AcadTransaction.MoveToTopOrder(oSourceParcel.BorderObjID)
         zzActiveParcelsView(moUD_ParcelLayerListPlusNew, oSourceParcel.DbID)
         zzCloseDWG()
         '  DMCommon.Debug.MsgBox("08_549", bAll, tParcelKey, oSourceParcel.ParcelKey.UD_ParcelName)
         zzDivideBranches(oSourceParcel, bAll)
      Else
         DMCommon.Debug.MsgBox("08_552", sMsg1, mdicParcels.Count, tParcelKey.BlockNo, tParcelKey.UD_ParcelName)
         ' For Each tKey As UD_ParcelKey In mdicParcels.Keys
         ' DMCommon.Debug.MsgBox("08_55a2", mdicParcels.Count, tKey.BlockNo, tKey.UD_ParcelName, tKey.CompareKey)
         ' Next
      End If
   End Sub
   Private Sub zzBeforeTransfer()

      Dim oParcel As UnidivNet.UD_Parcel = Nothing


      Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
      Dim tSourceParcelKey As UD_ParcelKey = zzParcelKeyFrom(oFirstActionRow)
      Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing
      Dim oDestParcel As UnidivNet.UD_Parcel

      Dim tDestParcelKey As UD_ParcelKey = zzParcelKeyTo(oFirstActionRow)

      Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestParcelNo"))
      Dim iGushNo As Integer = DMCommon.Functions.CIntN(oFirstActionRow.Item("DestBlockNo"))
      If tSourceParcelKey.Exists AndAlso tDestParcelKey.Exists Then
         If mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then
            If Not oSourceParcel.IsCanceled Then
               zzTransferFromFinalTopology(tSourceParcelKey)
            End If
         End If

         zzInsertTransferCentroid()

      End If





   End Sub

   Private Sub zzAllBranchesOld(oParcel As UnidivNet.UD_Parcel)
      Dim sFragmentsTopoName As String = "fragments"
      Dim oPgonUnion As tsPgonUnion
      Dim oPgon As tsPolygon
      Dim oBranch As tsBranch
      Dim oNode As tsNode

      Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

      Dim oFragmentsToposcheme As tsTopology = New tsTopology(sFragmentsTopoName)
      oFragmentsToposcheme.Load(False)

      DMAcadExt.AcadTransaction.CloseModelSpace()

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


      '  DMCommon.Debug.MsgBox("09_566", True, oFragmentsToposcheme.Elements.Count)
      oPgonUnion = oFragmentsToposcheme.NewPgonUnion()
      For Each iFragmentId As Integer In oParcel.Fragments
         oPgon = oFragmentsToposcheme.GetPolygon(iFragmentId)
         oPgonUnion.AddPolygon(oPgon)
      Next
      Dim hsInnerBranches As HashSet(Of Integer) = oPgonUnion.InnerBranches
      Dim hsBoundaryBranches As HashSet(Of Integer) = oPgonUnion.BoundaryBranches

      DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; FrCnt: " & oParcel.Fragments.Count.ToString() & "; " & hsInnerBranches.Count.ToString() & "; " & hsBoundaryBranches.Count.ToString())
      For Each iBranchID As Integer In hsInnerBranches
         oBranch = oFragmentsToposcheme.GetBranch(iBranchID)
         If oBranch IsNot Nothing Then

            DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; Inner: " & iBranchID.ToString() & "; L=" & oBranch.Layer & "; ID=" & oBranch.AcObjID.ToString())
            If oBranch.Layer.ToUpper() = "PCLP011" Then
               colTopoLinks.Add(oBranch.AcObjID)
               oNode = oFragmentsToposcheme.GetNode(oBranch.PreviousNodeID)
               If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
                  colNodes.Add(oNode.AcObjID)
               End If
               oNode = oFragmentsToposcheme.GetNode(oBranch.NextNodeID)
               If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
                  colNodes.Add(oNode.AcObjID)
               End If
            End If
         Else
            DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; Inner: " & iBranchID.ToString() & "; Link was not found")
         End If
      Next

      For Each iBranchID As Integer In hsBoundaryBranches
         DMAcadExt.AcadDocument.WriteDebugMessage(oParcel.ParcelKey.ToString() & "; Bound: " & iBranchID.ToString() & "; L=" & oFragmentsToposcheme.GetBranch(iBranchID).Layer & "; ID=" & oFragmentsToposcheme.GetBranch(iBranchID).AcObjID.ToString())
      Next

   End Sub

   Private Sub zzSelectSubSet(colAllInnerTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)

   End Sub
   Private Sub zzDivideBranches(oSourceParcel As UnidivNet.UD_Parcel, bAll As Boolean)
      Dim oPgonUnion As tsPgonUnion
      Dim oPgon As tsPolygon
      Dim colAllTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
      Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim oDestParcel As UnidivNet.UD_Parcel = Nothing
      Dim bOKContunue As Boolean
      '   DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      '   DMAcadExt.AcadTransaction.Start()
      '   DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      '  Dim oFragmentsToposcheme As tsTopology = New tsTopology(sFragmentsTopoName)

      ' DMCommon.Debug.MsgBox("09_566", True, oSourceParcel.ParcelKey, oSourceParcel.Fragments.Count)
      oPgonUnion = moFragmentsToposcheme.NewPgonUnion()
      For Each iFragmentId As Integer In oSourceParcel.Fragments
         DMAcadExt.AcadDocument.WriteDebugMessage(oSourceParcel.ParcelKey.ToString() & "; **FrID: " & iFragmentId.ToString() & "; ")
         oPgon = moFragmentsToposcheme.GetPolygon(iFragmentId)
         oPgonUnion.AddPolygon(oPgon)
      Next
      Dim hsInnerBranches As HashSet(Of Integer) = oPgonUnion.InnerBranches
      Dim hsBoundaryBranches As HashSet(Of Integer) = oPgonUnion.BoundaryBranches
      Dim oListDivided As List(Of UD_ParcelKey) = Nothing
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      Dim hsDestPgonIDs As HashSet(Of Integer) = Nothing


      Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel) = Nothing
      DMAcadExt.AcadDocument.WriteDebugMessage(oSourceParcel.ParcelKey.ToString() & "; FrCnt: " & oSourceParcel.Fragments.Count.ToString() & "; " & hsInnerBranches.Count.ToString() & "; " & hsBoundaryBranches.Count.ToString())
      If bAll Then
         DMAcadExt.AcadDocument.ClearDrawVectorSet()
         colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()


         zzGetTopoElements(moFragmentsToposcheme, hsInnerBranches, colTopoLinks, colNodes, msLinkNewLayer)

         zzOpenDWG(True, True)
         ' DMCommon.Debug.MsgBox("09_566M", "Inner", hsInnerBranches.Count, colTopoLinks.Count, msLinkNewLayer)
         bOKContunue = True
      Else

         colAllTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()


         zzGetTopoElements(moFragmentsToposcheme, hsInnerBranches, colAllTopoLinks, colNodes, msLinkNewLayer)
         '  ddddddddd
         zzOpenDWG(True, False)
         Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colAllTopoLinks, miSelectedColorIndex)
         ' zzCloseDWG()
         '  zzOpenDWG(True, False)
         DMAcadExt.AcadTransaction.SetVisible(msLinkNewLayer, colAllTopoLinks, False)
         '
         zzCloseDWG()
         '    DMCommon.Debug.MsgBox("09_567P", "Inner", hsInnerBranches.Count, colAllTopoLinks.Count)
         zzOpenDWG(True, True)
         colTopoLinks = zzGetLinksSet(colAllTopoLinks)
         bOKContunue = colTopoLinks.Count <> 0
      End If
      DMAcadExt.AcadDocument.WriteDebugMessage("!!54*; LinkCnt: " & colTopoLinks.Count.ToString())
      If bOKContunue Then
         oSourceParcel.IsCanceled = True
         oSourceParcel.UpdateBlockAttributes()
         zzGetTopoElements(moFragmentsToposcheme, hsBoundaryBranches, colTopoLinks, colNodes)
         DMAcadExt.AcadDocument.WriteDebugMessage("!!55*; LinkCnt2: " & colTopoLinks.Count.ToString())

         If Me.rdbHanit.Checked Then
            miCurrentCentroidStatus = enCentroidStatus.FromNewLayer
         ElseIf Me.rdbTaba.Checked Then
            miCurrentCentroidStatus = enCentroidStatus.FromTaba
         Else
            miCurrentCentroidStatus = enCentroidStatus.New
         End If

         '   DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug") Ud_Centroids
         If zzCreateDivideStageTopology(oSourceParcel, colTopoLinks, mcolAllNodes, oListDivided, hsDestPgonIDs, dicDestParcels, oSourceParcel.UD_Name) Then
            '  DMCommon.Debug.MsgBox("12_277")

            '  zzFragmentsAllocationNew()
            zzCreatePgonsPlus(hsDestPgonIDs)
            zzUpdateDivideResParcels(dicDestParcels)
            If False Then
               For Each oPolygonScheme As tsPolygon In moCurrentStageTopoScheme.Polygons

                  If Not oPolygonScheme.BorderObjID.IsNull AndAlso dicDestParcels.TryGetValue(oPolygonScheme.ID, oDestParcel) Then
                     oDestParcel.BorderObjID = oPolygonScheme.BorderObjID
                     oDestParcel.PolygonScheme = oPolygonScheme
                     DMAcadExt.AcadDocument.WriteDebugMessage("!PS " & oDestParcel.UD_Name & "; " & oPolygonScheme.BorderHandle.ToString() & "; " & oPolygonScheme.ID & "; " & oDestParcel.TopoID.ToString() & "; " & oPolygonScheme.Centroid.ToString())
                  End If
               Next
            End If
          

            '''''''''' zzLoadDivideResParcels(oParcel)
            If oListDivided IsNot Nothing Then
               '  DMCommon.Debug.MsgBox("12_080", oListDivided.Count)
               zzFragmentsAllocationNew(oPgonUnion, oListDivided)
               'zzFragmentsAllocation(oPgonUnion)
               zzCalcDivideResParcels(oSourceParcel, oListDivided, False)
            End If

            zzCloseAction()
         End If

      Else
         ' DMCommon.Debug.MsgBox("12_085", "miCurrentActionFirstRowIndex", miCurrentActionFirstRowIndex)
      End If
      zzRestoreView()


      '   UnidivNet.UD_App.SetStageLayerOn(miCurrentStage - 1, False)
      '    UnidivNet.UD_App.SetStageLayerOn(miCurrentStage, True)

      DMAcadExt.AcadTransaction.CloseModelSpace()

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()

      DMAcadExt.AcadDocument.Regen()
      '  DMAcadExt.AcadDocument.RestoreVarCmdDia()
   End Sub
   Private Sub zzUpdateDivideResParcels(dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel))
      Dim oDestParcel As UnidivNet.UD_Parcel = Nothing
      For Each oPolygonScheme As tsPolygon In moCurrentStageTopoScheme.Polygons

         If Not oPolygonScheme.BorderObjID.IsNull AndAlso dicDestParcels.TryGetValue(oPolygonScheme.ID, oDestParcel) Then
            oDestParcel.BorderObjID = oPolygonScheme.BorderObjID
            oDestParcel.PolygonScheme = oPolygonScheme
            DMAcadExt.AcadDocument.WriteDebugMessage("!PS " & oDestParcel.UD_Name & "; " & oPolygonScheme.BorderHandle.ToString() & "; " & oPolygonScheme.ID & "; " & oDestParcel.TopoID.ToString() & "; " & oPolygonScheme.Centroid.ToString())
         End If
      Next
   End Sub
   Private Sub zzFragmentsAllocationNew(oPgonUnion As tsPgonUnion, ByRef oListDivided As List(Of UD_ParcelKey))
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      Dim oTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sStageTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
      Dim oPolygon As Polygon
      Dim sParcelName As String

      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim tParcelKey As UD_ParcelKey

      ''''''''''''''''''''''''''''''''''''''''''     oListDivided = New List(Of UD_ParcelKey)
      For Each oPgon As Polygon In oTopology.GetPolygons()
         '   DMAcadExt.AcadDocument.WriteDebugMessage("997K " & sStageTopoName & "; " & oPgon.ID.ToString() & "; " & oPgon.Entity.ToString())
         '   DMAcadExt.AcadTransaction.DBObjectInfo(oPgon.Entity)
      Next
      For Each oParcel1 As UnidivNet.UD_Parcel In mdicParcels.Values

         ' DMAcadExt.AcadDocument.WriteDebugMessage("997za: " & oParcel1.CentroidAcObjID.ToString() & "; " & oParcel1.UD_Name)
      Next
      DMAcadExt.AcadDocument.WriteDebugMessage("37------------------------------------------ ")
      '     DMCommon.Debug.MsgBox("09_954c", oPgonUnion.FragmentPgons.Count)
      '  mdicParcels.DebugMsg("Before")
      For Each oFragmentPgonScheme As tsPolygon In oPgonUnion.FragmentPgons
         Try
            oPolygon = oTopology.FindPolygon(oFragmentPgonScheme.Centroid)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FragmentsAllocationNew-frmUnidiv", sStageTopoName & "; " & oFragmentPgonScheme.Centroid.ToString())
            oPolygon = Nothing
         End Try


         If oPolygon IsNot Nothing Then
            '  DMCommon.Debug.MsgBox("09_997zM", sStageTopoName, oPolygon.ID, oPolygon.Entity)


            If False Then
               Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
               Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
               oAcadBlock.Fields = {"PARCEL_NAME"}
               oAcadBlock.OpenForRead()
               '  DMCommon.Debug.MsgBox("09_991a", oPolygon.Entity)
               tBlockRefData = oAcadBlock.GetBlockRefData(oPolygon.Entity)
               '    DMCommon.Debug.MsgBox("09_991b")
               sParcelName = tBlockRefData.AttribValues(0)
               tParcelKey = New UD_ParcelKey(miBlockNo, sParcelName)
            End If



            If mdicParcels.TryGetValueByObjID(oPolygon.Entity, oParcel) Then
               ''''''''' DMCommon.Debug.MsgBox("09_998zz", oPolygon.Entity.ToString())
               oParcel.AddFragment(oFragmentPgonScheme.ID)
               mdicFragments.SetParcel(oFragmentPgonScheme.ID, oParcel.ParcelKey)
               ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   oListDivided.Add(oParcel.ParcelKey)
            Else
               DMCommon.Debug.MsgBox("09_991qq", mdicParcels.Count, tParcelKey.BlockNo, oPolygon.Entity, tParcelKey.BlockAdd, tParcelKey.ParcelNo, tParcelKey.UD_ParcelName, tParcelKey.CompareKey, oFragmentPgonScheme.ID)
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
   Private Sub zzGetTopoElements(ByRef oFragmentsToposcheme As tsTopology, hsBranches As HashSet(Of Integer), ByRef colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, Optional sLinkLayer As String = Nothing)
      Dim oBranch As tsBranch
      Dim oNode As tsNode
      If Not String.IsNullOrEmpty(sLinkLayer) Then
         sLinkLayer = sLinkLayer.ToUpper
      End If

      For Each iBranchID As Integer In hsBranches
         oBranch = oFragmentsToposcheme.GetBranch(iBranchID)
         If oBranch IsNot Nothing Then
            If String.IsNullOrEmpty(sLinkLayer) OrElse oBranch.Layer.ToUpper() = sLinkLayer Then
               colTopoLinks.Add(oBranch.AcObjID)
               oNode = oFragmentsToposcheme.GetNode(oBranch.PreviousNodeID)
               If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
                  colNodes.Add(oNode.AcObjID)
               End If
               oNode = oFragmentsToposcheme.GetNode(oBranch.NextNodeID)
               If oNode IsNot Nothing AndAlso Not colNodes.Contains(oNode.AcObjID) Then
                  colNodes.Add(oNode.AcObjID)
               End If
            End If
         Else
            DMAcadExt.AcadDocument.WriteDebugMessage("BranchID: " & iBranchID.ToString() & "; Link was not found")
         End If
      Next
      '     DMCommon.Debug.MsgBox("12_070", hsBranches.Count, colTopoLinks.Count, colNodes.Count)
   End Sub
   Private Sub zzUnion(hsFragmentUnion As HashSet(Of Integer), tResParcelArea As ParcelArea, sPrevParcelName As String)
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      '  Dim oFragmentTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("fragments")
      If True Then ' oFragmentTopoScheme.Load(False) Then
         Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing '= oFragmentsTopoScheme.GetDissolvedLinksBy(hsUnion)
         Dim colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
         Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
         Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing ' = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         Dim sStageTopoName As String = zzGetCurrentStageTopoName()
         moFragmentsToposcheme.GetDissolvedLinksBy(hsFragmentUnion, colTopoLinks, colNodes, colCancelledinks)
         zzAddCancelledLinks(colCancelledinks)
         Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLinkCancelledLayer, DMAcadExt.DMApp.AppID, True, False)
         DMAcadExt.AcadTransaction.SetLayer(colCancelledinks, msLinkCancelledLayer)
         ' Me.dgvMain.InvalidateRow(iRowIndex)

         '  DMCommon.Debug.MsgBox("11_230", hsFragmentUnion.Count, colTopoLinks.Count, colCancelledinks.Count, sPrevParcelName)
         If colTopoLinks.Count = 0 Then Return
         zzCreateStageTopology(colTopoLinks, mcolAllNodes, Nothing, sPrevParcelName)
         '      DMCommon.Debug.MsgBox("11_607d", mdicParcels.Count)
         zzCreatePgonsPlus()


         '   Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
         Dim oFirstActionRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)

         '    moCurrentStageTopoScheme.Load(False)
         Dim oPgonScheme As tsPolygon
         Dim oDestParcel As UnidivNet.UD_Parcel = Nothing '= New UnidivNet.UD_Parcel(moCurrentStageTopoScheme.Polygons.Item(0), mtCurrentParcelKey)
         If mdicParcels.TryGetValue(mtCurrentParcelKey, oDestParcel) Then
            oDestParcel.Fragments = hsFragmentUnion
            '  DMCommon.Debug.MsgBox("11_540", oDestParcel.AcadArea, oDestParcel.ParcelArea.LegalArea, oDestParcel.ParcelArea.AcadArea)

            oDestParcel.CalculateArea(tResParcelArea.LegalArea, tResParcelArea.AcadArea)
            oDestParcel.PrevParcelName = sPrevParcelName
            ' DMCommon.Debug.MsgBox("11_540b", oDestParcel.AcadArea, oDestParcel.ParcelArea.LegalArea, oDestParcel.ParcelArea.AcadArea)
            oDestParcel.UpdateBlockAttributes()
            mdicFragments.SetParcel(hsFragmentUnion, oDestParcel.ParcelKey)
            ' DMCommon.Debug.MsgBox("11_540a")
         Else
            oDestParcel = New UnidivNet.UD_Parcel(moCurrentStageTopoScheme.Polygons.Item(0), mtCurrentParcelKey)

         End If


         '  mdicParcels.AddParcel(oDestParcel)

         oFirstActionRow.Item("AcObjID") = oDestParcel.CentroidAcObjID
         oFirstActionRow.Item("ParcelDbID") = oDestParcel.DbID

         oPgonScheme = moCurrentStageTopoScheme.GetPolygon(oDestParcel.TopoID)

         oFirstActionRow.Item("BorderObjID") = oPgonScheme.BorderObjID
         oDestParcel.BorderObjID = oPgonScheme.BorderObjID


         '  DMCommon.Debug.MsgBox("11_437", oDestParcel.TopoID, moCurrentStageTopoScheme.Polygons.Count)

         '''''''''''''''''''''''    Dim oResParcel As UnidivNet.UD_Parcel = New UnidivNet.UD_Parcel()
         DMAcadExt.AcadTransaction.SetLayer(colCancelledinks, msLinkCancelledLayer)

         ' UnidivNet.UD_App.SetStageLayerOn(miCurrentStage - 1, False) 
         ' UnidivNet.UD_App.SetStageLayerOn(miCurrentStage, True)
      End If

      ' mcolLine As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   End Sub
   Private Sub zzAddCancelledLinks(colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      Try
         For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colCancelledinks
            mcolCanceledLinks.Add(tAcObjID)
         Next
      Catch oEx As Exception

      End Try
   End Sub
   Private Sub zzFinalTopo()
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
                  oDestPgonUnion.AddPolygonUnion(oPgonUnion)
               End If
               colCentroids.Add(oParcel.CentroidAcObjID)
               iCnt += 1
            End If
         Next
         hsParcelBoundaryBranches = oDestPgonUnion.BoundaryBranches
         '  hsSourceBoundaryBranches.UnionWith(hsParcelBoundaryBranches)

         colTopoLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
         '  zzGetTopoElements(hsParcelBoundaryBranches, colTopoLinks, colNodes)
         zzGetTopoElements(hsAllBoundaryBranches, colTopoLinks, colNodes)

         ' DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         Dim bRes As Boolean = zzCreateTopology(msFinalTopologyName, colTopoLinks, colCentroids)

         '   DMCommon.Debug.MsgBox("15_889", bRes, iCnt, mdicParcels.Count, colCentroids.Count, hsAllBoundaryBranches.Count, hsParcelBoundaryBranches.Count, colTopoLinks.Count, colNodes.Count, mcolAllNodes.Count)

      Else
         'DMCommon.Debug.MsgBox("15_889", "Final OK!")
      End If
   End Sub

   Private Sub zzUnion_180717(hsParcelUnion As HashSet(Of Integer))
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      miCurrentAction += 1
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Dim oParcelTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("Parcels")
      oParcelTopoScheme.Load(False)
      Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing '= oFragmentsTopoScheme.GetDissolvedLinksBy(hsUnion)
      Dim colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing ' = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      oParcelTopoScheme.GetDissolvedLinksBy(hsParcelUnion, colTopoLinks, colNodes, colCancelledinks)
      UnidivNet.UD_App.AddCancelledLinks(colCancelledinks)
      DMAcadExt.AcadTransaction.SetLayer(colCancelledinks, "pclp005")
      ' Me.dgvMain.InvalidateRow(iRowIndex)
      zzCreateStageTopology(colTopoLinks, mcolAllNodes, Nothing, "")


      zzCreatePgonsPlusAAA(sStageTopoName)

      '''''''''''''''''''''''    Dim oResParcel As UnidivNet.UD_Parcel = New UnidivNet.UD_Parcel()
      DMAcadExt.AcadTransaction.SetLayer(colCancelledinks, msLinkCancelledLayer)

      UnidivNet.UD_App.SetStageLayerOn(miCurrentStage - 1, False)
      UnidivNet.UD_App.SetStageLayerOn(miCurrentStage, True)
      DMAcadExt.AcadTransaction.CloseModelSpace()

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
      ' mcolLine As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   End Sub
   Private Sub zzUnion_210617(hsUnion As HashSet(Of Integer))
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      miCurrentAction += 1
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Dim oFragmentsTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology("fragments")
      oFragmentsTopoScheme.Load(False)
      Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing '= oFragmentsTopoScheme.GetDissolvedLinksBy(hsUnion)
      Dim colCancelledinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      oFragmentsTopoScheme.GetDissolvedLinksBy(hsUnion, colTopoLinks, colNodes, colCancelledinks)
      ' Me.dgvMain.InvalidateRow(iRowIndex)
      Try
         '  System.Windows.Forms.MessageBox.Show(sDissolveTopoName & ":=" & CStr(colLinks.Count), "15_125")

         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, Autodesk.Gis.Map.Topology.CreateOptions.IgnoreIncompleteArea, 0.001)
         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_14a")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "Topo")
      End Try
      zzCreatePgonsAAA(sStageTopoName)

      DMAcadExt.AcadTransaction.SetLayer(colCancelledinks, msLinkCancelledLayer)
      DMAcadExt.AcadTransaction.CloseModelSpace()

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      ' mcolLine As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   End Sub
   Private Sub zzStageTopologyFragments()

   End Sub
   Private Function zzCreateDivideStageTopologyDB(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UD_ParcelKey), ByRef dicDestParcelsP As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String, dicParcelByFragments As Dictionary(Of Integer, tsPgonUnion)) As Boolean

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
      '  Dim oPolygon As Polygon = Nothing
      Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colDestParcels As System.Collections.ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
      Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Select Case miCurrentCentroidStatus
         Case enCentroidStatus.FromNewLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
            DMCommon.Debug.MsgBox("15_126a", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         Case enCentroidStatus.FromStageLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
         Case Else
            colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      End Select
      Try
         '   DMCommon.Debug.MsgBox("15_125b", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count, colNodes.Count, mcolAllNodes.Count)

         oTopos.Create(sStageTopoName, colTopoLinks, mcolAllNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateStageTopology")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         Return False
      End Try
      DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
      If oTopos.Exists(sStageTopoName) Then

         '  Dim oPointList As IList(Of TPlnPoint) = Nothing
         Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

         oAcadBlock.OpenForRight()
         oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
         If miCurrentActionType = UnidivNet.enActionType.Divide Then
            oListDivided = New List(Of UD_ParcelKey)
         End If

         Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

         If oFragmentTopology IsNot Nothing Then
            Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion = Nothing


            '  DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count)
            '    Dim sX As String = "X"
            '   DMCommon.Debug.MsgBox("11_607c", mdicParcels.Count, oTopoModel.GetPolygons.Count)
            For Each oPolygonM As Polygon In oTopoModel.GetPolygons
               Try
                  oFragmentPgon = oFragmentTopology.FindPolygon(oPolygonM.Centroid)
               Catch oEx As Exception
                  oFragmentPgon = Nothing
               End Try
               If oFragmentPgon IsNot Nothing AndAlso dicParcelByFragments.TryGetValue(oFragmentPgon.ID, oPgonUnion) Then


                  oParcel = New UnidivNet.UD_Parcel(oPolygonM, oPgonUnion.ParcelKey)
                  '  DMCommon.Debug.MsgBox("11_210", oParcel.Name, oParcel.ParcelKey.ParcelNo, oPgonUnion.ParcelKey.ParcelNo)
                  oParcel.IsOriginal = False
                  oParcel.PrevParcelName = sPrevParcelName
                  oParcel.Fragments = oPgonUnion.Polygons
                  oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygonM.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                  Select Case oCentroidDBObject.GetRXClass.Name
                     Case DMAcadExt.AcadConst.AcadBlockRefName
                        oParcel.ReadNewBlockAttributes()
                     Case DMAcadExt.AcadConst.AcadPointName
                        oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
                        colCentroidPoints.Add(oPolygonM.Entity)
                        tBlockRefData.Position = oCentroidPoint.Position
                  End Select

                  '   oCentroidPoint = DMAcadExt.AcadTransaction.GetAcadPoint(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

                  tBlockRefData.Layer = sStageCentroidLayer


                  oParcel.InsertBlock(oPolygonM.Centroid)
                  '   mtCurrentParcelKey
                  '  tBlockRefData.ScaleFactors = oBlockRef.ScaleFactors


                  If miCurrentActionType = UnidivNet.enActionType.Divide Then
                     ''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())
                     oParcel.Calc()
                     '  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}


                     '   DMCommon.Debug.MsgBox("09_955", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
                     oListDivided.Add(oParcel.ParcelKey)
                     colDestParcels.Add(oParcel)
                     '  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
                     '  mtCurrentParcelKey.NextParcel()
                  End If
                  If False Then
                     tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
                     oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
                  End If



                  colCentroidsBlocks.Add(oParcel.CentroidAcObjID) 'tCentroidBlockAcObjID
                  mdicParcels.AddParcel(oParcel)
               End If
               ' miLastParcel += 1
            Next
         End If

         ' 

         oTopoModel.Close()
         oFragmentTopology.Close()

         oTopos.Delete(sStageTopoName, False)
         DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
         '  zzDicpCol("colCentroidPoints", colCentroidPoints)
         '  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
         '   DMCommon.Debug.MsgBox("11_260", sStageTopoName, mcolAllNodes.Count, colCentroidsBlocks.Count, colNodes.Count)
         oTopos.Create(sStageTopoName, colTopoLinks, mcolAllNodes, colCentroidsBlocks, TopologyTypes.Polygon)
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
         If oTopoModel IsNot Nothing Then
            Dim oPolygon As Polygon
            moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
            moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
            '    DMCommon.Debug.MsgBox("12_301")
            dicDestParcelsP = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
            '  DMCommon.Debug.MsgBox("12_301a", colDestParcels.Count)
            For Each oParcel In colDestParcels
               '   DMCommon.Debug.MsgBox("12_301b", oParcel Is Nothing)
               oPolygon = oTopoModel.FindPolygon(oParcel.CentroidPoint3d)
               '  DMCommon.Debug.MsgBox("12_301c", oParcel Is Nothing)
               If oPolygon IsNot Nothing Then
                  DMAcadExt.AcadDocument.WriteDebugMessage("!#pgon: " & oParcel.TopoID & "; " & oPolygon.ID & "; " & "; " & oParcel.Name & "; " & oParcel.CentroidPoint3d.ToString())
                  oParcel.TopoID = oPolygon.ID
                  dicDestParcelsP.Add(oParcel.TopoID, oParcel)
               End If
            Next
         End If

         ''''''''''''  moStageTopologies.AddTopoName(sStageTopoName)
         DMAcadExt.AcadDocument.Regen()
      End If
      '   DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
   End Function
   Private Function zzCreateUnionStageTopologyDB(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sPrevParcelName As String, dSumOfLegalArea As Double, tDestParcelKey As UD_ParcelKey, hsDestFragments As HashSet(Of Integer), ByRef oParcel As UnidivNet.UD_Parcel) As Boolean
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      ' Dim 
      Dim sStageCentroidLayer As String = zzGetCentroidLayer()
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
      Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim oCentroidPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
      Dim oTopoModel As TopologyModel
      Dim oCentroidDBObject As DBObject
      Dim oFragmentPgon As Polygon
      Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Select Case miCurrentCentroidStatus
         Case enCentroidStatus.FromNewLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
            '  DMCommon.Debug.MsgBox("15_126a", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         Case enCentroidStatus.FromStageLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
         Case Else
            colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      End Select
      Try
         '  DMCommon.Debug.MsgBox("15_125b", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)

         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateStageTopology")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         Return False
      End Try
      DMAcadExt.AcadDocument.SaveVarCmdDia(0S)

      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
      If oTopos.Exists(sStageTopoName) Then

         '  Dim oPointList As IList(Of TPlnPoint) = Nothing
         Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

         oAcadBlock.OpenForRight()
         oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)


         '     Dim oFragmentTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

         '   If oFragmentTopology IsNot Nothing Then
         '  


         '  DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count)
         '    Dim sX As String = "X"
         '   DMCommon.Debug.MsgBox("11_607L", mdicParcels.Count, oTopoModel.GetPolygons.Count)
         For Each oPolygon As Polygon In oTopoModel.GetPolygons

            Try
               '  oFragmentPgon = oFragmentTopology.FindPolygon(oPolygon.Centroid)
            Catch oEx As Exception

            End Try

            '  DMCommon.Debug.MsgBox("11_607M", mdicParcels.Count, oTopoModel.GetPolygons.Count)

            oParcel = New UnidivNet.UD_Parcel(oPolygon, tDestParcelKey)
            '   DMCommon.Debug.MsgBox("11_210", oParcel.Name, oParcel.ParcelKey.ParcelNo, tDestParcelKey.ParcelNo)
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
            End Select

            '   oCentroidPoint = DMAcadExt.AcadTransaction.GetAcadPoint(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

            tBlockRefData.Layer = sStageCentroidLayer


            oParcel.InsertBlock(oPolygon.Centroid)
            '   mtCurrentParcelKey
            '  tBlockRefData.ScaleFactors = oBlockRef.ScaleFactors


            If miCurrentActionType = UnidivNet.enActionType.Divide Then
               ''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())

               oParcel.Calc() '?????????
               '  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}


               '   DMCommon.Debug.MsgBox("09_955", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)

               '  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
               '  mtCurrentParcelKey.NextParcel()
            End If
            If False Then
               tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
               oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
            End If
           
            ' 
            colCentroidsBlocks.Add(oParcel.CentroidAcObjID)  'tCentroidBlockAcObjID

            mdicParcels.AddParcel(oParcel)
            ' miLastParcel += 1
         Next
         'End If

         moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
         moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
         ' 

         oTopoModel.Close()
         'oFragmentTopology.Close()

         oTopos.Delete(sStageTopoName, False)
         DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
         '  zzDicpCol("colCentroidPoints", colCentroidPoints)
         '  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
         '   DMCommon.Debug.MsgBox("11_260", sStageTopoName, colCentroidsBlocks.Count)
         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)

         moStageTopologies.AddTopoName(sStageTopoName)
         DMAcadExt.AcadDocument.Regen()
      End If
      '   DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
   End Function
   Private Function zzCreateDivideStageTopology(oSourceParcel As UnidivNet.UD_Parcel, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UD_ParcelKey), ByRef hsDestPgonIDs As HashSet(Of Integer), ByRef dicDestParcelsP As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String) As Boolean
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
      Dim bInsertByPick As Boolean = Me.rdbInsertByPick.Checked
      Dim iInsertBlockSouce As Boolean = Me.rdbInsertByPick.Checked
      Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim bParcelSuccess As Boolean
      Dim bCancel As Boolean = False
      Dim hsAcadPointsPgonIDs As HashSet(Of Integer) = New HashSet(Of Integer)()
      Dim dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel)
      DMAcadExt.AcadDocument.SavePointFormat(64S, 2.0)
      DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, False)
      Select Case miCurrentCentroidStatus
         Case enCentroidStatus.FromNewLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
            '  DMCommon.Debug.MsgBox("15_126q", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         Case enCentroidStatus.FromStageLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
         Case Else
            colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      End Select
      DMAcadExt.AcadTransaction.SetCurrentLayer(zzGetCentroidLayer(), 1, True, False)

      Try
         DMCommon.Debug.MsgBox("15_125b", sStageTopoName, colTopoLinks.Count, colCentroidsBlocks.Count, miCurrentCentroidStatus)
         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)
         '   DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateDivideStageTopology")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         DMAcadExt.AcadDocument.RestorePointFormat()
         DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, True)

         Return False
      End Try

      ' Dim oTestDbobject As DBObject
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
      If oTopos.Exists(sStageTopoName) Then

         '  Dim oPointList As IList(Of TPlnPoint) = Nothing
         ' Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         Dim iFragmentID As Integer

         oAcadBlock.OpenForRight()
         oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)

         DMCommon.Debug.MsgBox("15_125AQ", sStageTopoName, oTopoModel.GetPolygons.Count, oTopoModel.GetFullEdges.Count, colTopoLinks.Count, miCurrentCentroidStatus, colCentroidsBlocks.Count)
         If miCurrentActionType = UnidivNet.enActionType.Divide Then
            oListDivided = New List(Of UD_ParcelKey)
            hsDestPgonIDs = New HashSet(Of Integer)()
            dicDestParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
         End If
         '''''''' zzTopoInfo(oTopoModel, "!!Points ")
         Dim oTestEntity As Autodesk.AutoCAD.DatabaseServices.Entity
         Dim oPolygon As Polygon = Nothing
         Dim iPolygonID As Integer
         Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
         Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d


         moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
         '     DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count, moFragmentTopology.GetPolygons.Count)

         For iIndex As Integer = 0 To oTopoModel.GetPolygons.Count - 1

            oPolygon = oTopoModel.GetPolygons.Item(iIndex)


            '    DMAcadExt.AcadDocument.WriteDebugMessage("999pgon : " & oPolygon.Entity.ToString() & "; " & iIndex)

            iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
            '  DMCommon.Debug.MsgBox("11_662d", oTestEntity.Layer, oTestEntity.GetRXClass.Name, iFragmentID)
            If oSourceParcel.HasFragment(iFragmentID) Then
               ' DMCommon.Debug.MsgBox("09_201bef", oTopoModel.GetPolygons.Count, moFragmentTopology.GetPolygons.Count)
               oParcel = New UnidivNet.UD_Parcel(oPolygon)
               '  DMAcadExt.AcadDocument.WriteDebugMessage("999pgon  Point? " & oParcel.AcadPoint.ToString() & "; " & oParcel.CentroidAcObjID.ToString() & "; " & oParcel.UD_Name & "; " & oParcel.Landuse)
               '  DMCommon.Debug.MsgBox("09_201af ", oTopoModel.GetPolygons.Count, moFragmentTopology.GetPolygons.Count)
               If oParcel.AcadPoint Then
                  If bInsertByPick Then
                     hsAcadPointsPgonIDs.Add(oPolygon.ID)
                  Else
                     oParcel.SetParcelKey(mtCurrentParcelKey)

                     oListDivided.Add(oParcel.ParcelKey)
                  End If
               Else
                  ' DMCommon.Debug.MsgBox("09_231tr ", oParcel.UD_Name)
                  mdicParcels.AddParcel(oParcel)
                  oListDivided.Add(oParcel.ParcelKey)
               End If


               dicDestParcels.Add(oParcel.TopoID, oParcel)
               hsDestPgonIDs.Add(oParcel.TopoID)
               oParcel.PrevParcelName = sPrevParcelName
               oParcel.Stage = miCurrentStage
               oParcel.Calc()


            Else
               DMCommon.Debug.MsgBox("09_207x ", colCentroidsBlocks.Count)
               '   colCentroidsBlocks.Add(oPolygon.Entity)
            End If


            tBlockRefData.Layer = sStageCentroidLayer
            mtCurrentParcelKey.NextParcel()
         Next



         '  DMCommon.Debug.MsgBox("12_260 ", hsAcadPointsPgonIDs.Count, mdicParcels.Count, dicDestParcels.Count)
         ''''''''''''''''''''''''  By Pick
         If hsAcadPointsPgonIDs.Count > 0 Then
            Me.Visible = False
            AppActivate(moAppWin.Text)
            Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

            Do
               bParcelSuccess = False
               ' DMCommon.Debug.MsgBox("12_241K", DMCommon.Debug.GetList(hsAcadPointsPgonIDs))
               If hsAcadPointsPgonIDs.Count = 1 Then
                  iPolygonID = hsAcadPointsPgonIDs.ElementAt(0)
                  '  DMCommon.Debug.MsgBox("12_241w", iPolygonID)
               Else
                  iSelectStatus = zzSelectPoint(tPoint, True)
                  If iSelectStatus = PromptStatus.OK AndAlso oTopoModel IsNot Nothing Then
                     Try
                        oPolygon = oTopoModel.FindPolygon(tPoint)
                        iPolygonID = oPolygon.ID
                        '  DMCommon.Debug.MsgBox("12_242", iPolygonID)
                     Catch oEx As Exception
                        DMCommon.Debug.MsgBox("12_241", oEx.Message, oEx.StackTrace)
                     End Try
                  ElseIf iSelectStatus = PromptStatus.Cancel Then
                     bCancel = True
                     Exit Do
                  End If
               End If

               If dicDestParcels.TryGetValue(iPolygonID, oParcel) Then

                  If oParcel.ParcelKey.ParcelNo = 0 Then
                     oParcel.SetParcelKey(mtCurrentParcelKey)
                     mtCurrentParcelKey.NextParcel()
                     '  DMCommon.Debug.MsgBox("12_244", oParcel.UD_Name)
                     zzSetNameToParcels(oParcel, colCentroidsBlocks)
                     zzCloseDWG()
                     zzOpenDWG()
                     '  oParcel.SetParcelKey(mtCurrentParcelKey)
                     '  DMCommon.Debug.MsgBox("12_241YY", DMCommon.Debug.GetList(hsAcadPointsPgonIDs))
                     '  DMCommon.Debug.MsgBox("12_241ZZ", "_________", oPolygon.ID)
                     hsAcadPointsPgonIDs.Remove(iPolygonID)
                     '   DMCommon.Debug.MsgBox("12_241++", hsAcadPointsPgonIDs.Count)
                     oListDivided.Add(oParcel.ParcelKey)
                  End If
               Else
                  DMCommon.Debug.MsgBox("12_245", iPolygonID)
               End If
               '  DMCommon.Debug.MsgBox("12_247", hsAcadPointsPgonIDs.Count, DMCommon.Debug.ColCount(hsAcadPointsPgonIDs))
               '''''''''''''''''''''''''  Loop Until bEsc OrElse bParcelSuccess
            Loop While hsAcadPointsPgonIDs.Count <> 0
         End If
         ''''''''''''''''''''''''End of by Pick


         ''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())

         '''''''  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}
         '   DMCommon.Debug.MsgBox("09_955", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
         ' oListDivided.Add(oParcel.ParcelKey)
         '  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
         '    mtCurrentParcelKey.NextParcel()
         '    DMCommon.Debug.MsgBox("09_908a", oParcel.AcadPoint, tBlockRefData.Layer, tBlockRefData.Position, oParcel.CentroidAcObjID)
         If Not bCancel Then
            For Each oParcel In dicDestParcels.Values
               If oParcel.AcadPoint Then
                  zzSetNameToParcels(oParcel, colCentroidsBlocks)
                  '    DMCommon.Debug.MsgBox("09_908t", tCentroidBlockAcObjID, oParcel.CentroidAcObjID)
               Else
                  '   DMCommon.Debug.MsgBox("09_908m", sStageCentroidLayer, oParcel.CentroidAcObjID)
                  DMAcadExt.AcadTransaction.SetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer)
               End If

               ' DMCommon.Debug.MsgBox("12_244")
               oParcel.UpdateBlockAttributes()  '??????????????????
            Next
         End If



         ' miLastParcel += 1
         ' zzCloseDWG()
         DMAcadExt.AcadDocument.UpdateScreen()
         ' DMAcadExt.AcadDocument.Regen()
         '  DMCommon.Debug.MsgBox("12_245")
         '   zzOpenDWG()
         '  DMCommon.Debug.MsgBox("12_246")
         oTopoModel.Close()
         If Not bCancel Then
            oTopos.Delete(sStageTopoName, False)
            DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
            '  zzDicpCol("colCentroidPoints", colCentroidPoints)
            '  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
            '   DMCommon.Debug.MsgBox("11_260", sStageTopoName, colCentroidsBlocks.Count)
            For Each tAcObjID As ObjectId In colCentroidsBlocks
               '   DMAcadExt.AcadTransaction.DBObjectInfo(tAcObjID)
            Next
            '      DMCommon.Debug.MsgBox("15_125K", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
            oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)


            oTopoModel = oTopos(sStageTopoName)
            oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
            moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
            moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
            ''''''''''''   zzTopoInfo(oTopoModel, "!!Blocks ")
            dicDestParcelsP = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()
            For Each oParcel In dicDestParcels.Values
               oPolygon = oTopoModel.FindPolygon(oParcel.CentroidPoint3d)
               oParcel.TopoID = oPolygon.ID
               dicDestParcelsP.Add(oParcel.TopoID, oParcel)
            Next


            moStageTopologies.AddTopoName(sStageTopoName)
            DMAcadExt.AcadDocument.Regen()
         End If

         Me.Visible = True
         DMAcadExt.AcadDocument.RestorePointFormat()
         DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, True)
         Return Not bCancel
      Else
         DMAcadExt.AcadDocument.RestorePointFormat()
         DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, True)
         Return False
      End If
      '   DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
   End Function
   Private Sub zzSetNameToParcels(ByRef oParcel As UnidivNet.UD_Parcel, ByRef colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
      Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName)
      Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId

      oAcadBlock.OpenForRight()
      oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
      tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), "sPrevParcelName"}
      tBlockRefData.Position = oParcel.CentroidPoint3d
      tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
      If Not tCentroidBlockAcObjID.IsNull Then
         DMAcadExt.AcadTransaction.EraseDBObject(oParcel.CentroidAcObjID)
         '  DMCommon.Debug.MsgBox("12_265", mtCurrentParcelKey.UD_ParcelName(), tCentroidBlockAcObjID, oParcel.UD_Name)
         oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
         colCentroidsBlocks.Add(tCentroidBlockAcObjID)
      End If

      '   DMAcadExt.AcadDocument.WriteDebugMessage("!After point: " & tCentroidBlockAcObjID.ToString() & "; " & oParcel.UD_Name & "; " & tBlockRefData.Position.ToString())
      '  DMAcadExt.AcadTransaction.DBObjectInfo(tCentroidBlockAcObjID, "ToPoint: ")
      mdicParcels.AddParcel(oParcel)
   End Sub
   Private Sub zzTopoInfo(oTopoModel As TopologyModel, sLabel As String)
      For Each oPgon As Polygon In oTopoModel.GetPolygons
         DMAcadExt.AcadDocument.WriteDebugMessage(sLabel & oPgon.ID & "; " & oPgon.Centroid.ToString())
      Next

   End Sub
   Private Function zzCreateDivideStageTopologyOld(oSourceParcel As UnidivNet.UD_Parcel, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UD_ParcelKey), ByRef hsDestPgonIDs As HashSet(Of Integer), ByRef dicDestParcels As Dictionary(Of Integer, UnidivNet.UD_Parcel), sPrevParcelName As String) As Boolean
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
      Dim bInsertByPick As Boolean = Me.rdbInsertByPick.Checked
      Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim bParcelSuccess As Boolean
      Dim bEsc As Boolean

      DMAcadExt.AcadDocument.SavePointFormat(64S, 2.0)
      DMAcadExt.AcadTransaction.SetLayersOnExcept(moOperUDLayersList, False)
      Select Case miCurrentCentroidStatus
         Case enCentroidStatus.FromNewLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
            '  DMCommon.Debug.MsgBox("15_126q", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         Case enCentroidStatus.FromStageLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
         Case Else
            colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      End Select
      DMAcadExt.AcadTransaction.SetCurrentLayer("Ud_Centroids", 1, True, False)
      Try
         '  DMCommon.Debug.MsgBox("15_125b", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)
         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateStageTopology")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         Return False
      End Try

      Dim oTestDbobject As DBObject
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
      If oTopos.Exists(sStageTopoName) Then

         '  Dim oPointList As IList(Of TPlnPoint) = Nothing
         Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         Dim iFragmentID As Integer
         oAcadBlock.OpenForRight()
         oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
         If miCurrentActionType = UnidivNet.enActionType.Divide Then
            oListDivided = New List(Of UD_ParcelKey)
            hsDestPgonIDs = New HashSet(Of Integer)()
            dicDestParcels = New Dictionary(Of Integer, UnidivNet.UD_Parcel)()

         End If
         Dim oTestEntity As Autodesk.AutoCAD.DatabaseServices.Entity
         Dim oPolygon As Polygon = Nothing
         Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
         Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
         '  DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count)
         '    Dim sX As String = "X"
         '     DMCommon.Debug.MsgBox("11_607c", mdicParcels.Count, oTopoModel.GetPolygons.Count)
         moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
         For iIndex As Integer = 0 To oTopoModel.GetPolygons.Count - 1
            oPolygon = oTopoModel.GetPolygons.Item(iIndex)

            If oParcel.AcadPoint AndAlso bInsertByPick Then
               oParcel = New UnidivNet.UD_Parcel(oPolygon)
               dicDestParcels.Add(oParcel.TopoID, oParcel)
            Else
               oParcel = New UnidivNet.UD_Parcel(oPolygon, mtCurrentParcelKey)
            End If

            iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
            '  DMCommon.Debug.MsgBox("11_662d", oTestEntity.Layer, oTestEntity.GetRXClass.Name, iFragmentID)
            If oSourceParcel.HasFragment(iFragmentID) Then
            Else
               colCentroidsBlocks.Add(oPolygon.Entity)
            End If
         Next


         Me.Visible = False
         AppActivate(moAppWin.Text)
         Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

         For iIndex As Integer = 0 To oTopoModel.GetPolygons.Count - 1
            ''''''''''''''''''''''''''''''''''''''''''''   
            bParcelSuccess = False
            Do

               If bInsertByPick Then
                  iSelectStatus = zzSelectPoint(tPoint, True)
                  If iSelectStatus = PromptStatus.OK AndAlso oTopoModel IsNot Nothing Then
                     Try
                        oPolygon = oTopoModel.FindPolygon(tPoint)
                     Catch oEx As Exception
                        DMCommon.Debug.MsgBox("12_241", oEx.Message, oEx.StackTrace)
                     End Try
                  ElseIf iSelectStatus = PromptStatus.Cancel Then
                     bEsc = True
                     Exit Do
                  End If
               Else
                  oPolygon = oTopoModel.GetPolygons.Item(iIndex)
                  bParcelSuccess = True
               End If


               oParcel = New UnidivNet.UD_Parcel(oPolygon, mtCurrentParcelKey)


               DMCommon.Debug.MsgBox("12_240", iSelectStatus, oPolygon, bEsc)
               If oPolygon IsNot Nothing And bEsc = False Then
                  oTestEntity = DMAcadExt.AcadTransaction.GetEntity(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                  iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
                  '  DMCommon.Debug.MsgBox("11_662d", oTestEntity.Layer, oTestEntity.GetRXClass.Name, iFragmentID)
                  If oSourceParcel.HasFragment(iFragmentID) Then
                     If Not hsDestPgonIDs.Contains(oPolygon.ID) Then
                        hsDestPgonIDs.Add(oPolygon.ID)
                        If Not dicDestParcels.ContainsKey(oPolygon.ID) Then
                           dicDestParcels.Add(oPolygon.ID, oParcel)
                           DMCommon.Debug.MsgBox("12_254", oParcel.UD_Name)
                           bParcelSuccess = True
                        End If
                     End If
                     oTestDbobject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
                     '   DMCommon.Debug.MsgBox("11_703!!!", oTestDbobject.GetRXClass.Name, mdicParcels.Count, oParcel.BlockNo, oParcel.Name, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName)

                  Else
                     colCentroidsBlocks.Add(oPolygon.Entity)
                  End If
               End If
            Loop Until bEsc OrElse bParcelSuccess
            If bEsc Then
               Exit For
            End If
            DMCommon.Debug.MsgBox("12_255", bParcelSuccess, oParcel)
            If bParcelSuccess Then
               oParcel.PrevParcelName = sPrevParcelName
               oParcel.Stage = miCurrentStage
            End If

            If oParcel.AcadPoint Then
               oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
               oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
               colCentroidPoints.Add(oPolygon.Entity)
               tBlockRefData.Position = oCentroidPoint.Position
            End If
            tBlockRefData.Layer = sStageCentroidLayer
            ''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())
            oParcel.Calc()
            '''''''  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}
            '   DMCommon.Debug.MsgBox("09_955", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
            oListDivided.Add(oParcel.ParcelKey)
            '  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
            mtCurrentParcelKey.NextParcel()
            '    DMCommon.Debug.MsgBox("09_908a", oParcel.AcadPoint, tBlockRefData.Layer, tBlockRefData.Position, oParcel.CentroidAcObjID)
            If oParcel.AcadPoint Then
               '   DMCommon.Debug.MsgBox("12_250", mtCurrentParcelKey.UD_ParcelName())
               tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}
               tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
               oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
               '    DMCommon.Debug.MsgBox("09_908t", tCentroidBlockAcObjID, oParcel.CentroidAcObjID)
            Else
               '   DMCommon.Debug.MsgBox("09_908m", sStageCentroidLayer, oParcel.CentroidAcObjID)
               DMAcadExt.AcadTransaction.SetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer)
            End If

            ''''''''''''''''''''''''''''''''  oParcel.UpdateBlockAttributes()
            colCentroidsBlocks.Add(tCentroidBlockAcObjID)

            mdicParcels.AddParcel(oParcel)

            ' DMCommon.Debug.MsgBox("12_244")
            oParcel.UpdateBlockAttributes()

            'mdicParcels.AddParcel(oParcel)
            ' miLastParcel += 1
            ' zzCloseDWG()
            DMAcadExt.AcadDocument.UpdateScreen()
            DMAcadExt.AcadDocument.Regen()
            '  DMCommon.Debug.MsgBox("12_245")
            '   zzOpenDWG()
            '  DMCommon.Debug.MsgBox("12_246")


         Next

         moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
         moCurrentStageTopoScheme.Load(True, oTopoModel)
         ' 

         oTopoModel.Close()
         If Not bEsc Then


            oTopos.Delete(sStageTopoName, False)
            DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
            '  zzDicpCol("colCentroidPoints", colCentroidPoints)
            '  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
            '   DMCommon.Debug.MsgBox("11_260", sStageTopoName, colCentroidsBlocks.Count)
            oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)

            moStageTopologies.AddTopoName(sStageTopoName)
            DMAcadExt.AcadDocument.Regen()
         End If

         Me.Visible = True
         Return True
      Else
         Return False
      End If
      '   DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
   End Function
   Private Function zzCreateStageTopology(colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByRef oListDivided As List(Of UD_ParcelKey), sPrevParcelName As String) As Boolean
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
      Dim tCentroidBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim colCentroidsBlocks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim colCentroidPoints As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Select Case miCurrentCentroidStatus
         Case enCentroidStatus.FromNewLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, msNewParcel)
            '  DMCommon.Debug.MsgBox("15_126q", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)
         Case enCentroidStatus.FromStageLayer
            colCentroidsBlocks = DMAcadExt.AcadTransaction.GetBlockRefsNew(msCentroidBlockName, UnidivNet.UD_App.GetStageParcelCentroidLayer(miCurrentStage))
         Case Else
            colCentroidsBlocks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      End Select
      Try
         '  DMCommon.Debug.MsgBox("15_125b", sStageTopoName, miCurrentCentroidStatus, colTopoLinks.Count, colCentroidsBlocks.Count)

         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateStageTopology")
         DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         Return False
      End Try

      Dim oTestDbobject As DBObject
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sStageCentroidLayer, DMAcadExt.DMApp.AppID, True, False)
      If oTopos.Exists(sStageTopoName) Then

         '  Dim oPointList As IList(Of TPlnPoint) = Nothing
         Dim colResLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         Dim iFragmentID As Integer
         oAcadBlock.OpenForRight()
         oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "PARCEL_PREVIOUS"}
         oTopoModel = oTopos(sStageTopoName)
         oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
         If miCurrentActionType = UnidivNet.enActionType.Divide Then
            oListDivided = New List(Of UD_ParcelKey)
         End If


         '  DMCommon.Debug.MsgBox("09_201", oTopoModel.GetPolygons.Count)
         '    Dim sX As String = "X"
         '     DMCommon.Debug.MsgBox("11_607c", mdicParcels.Count, oTopoModel.GetPolygons.Count)
         moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
         For Each oPolygon As Polygon In oTopoModel.GetPolygons
            iFragmentID = zzGetFragmentByPoint(oPolygon.Centroid)
            oParcel = New UnidivNet.UD_Parcel(oPolygon, mtCurrentParcelKey)
            oTestDbobject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
            '    DMCommon.Debug.MsgBox("11_703!!!", oTestDbobject.GetRXClass.Name, mdicParcels.Count, oParcel.BlockNo, oParcel.Name, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName)
            oParcel.IsOriginal = False
            oParcel.PrevParcelName = sPrevParcelName
            oParcel.Stage = miCurrentStage

            If oParcel.AcadPoint Then
               oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
               oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
               colCentroidPoints.Add(oPolygon.Entity)
               tBlockRefData.Position = oCentroidPoint.Position
            End If
            'oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            'Select Case oCentroidDBObject.GetRXClass.Name
            '   Case DMAcadExt.AcadConst.AcadBlockRefName
            '      oParcel.ReadNewBlockAttributes()
            '   Case DMAcadExt.AcadConst.AcadPointName
            '      oCentroidPoint = DirectCast(oCentroidDBObject, DBPoint)
            '      colCentroidPoints.Add(oPolygon.Entity)
            '      tBlockRefData.Position = oCentroidPoint.Position
            'End Select

            '   oCentroidPoint = DMAcadExt.AcadTransaction.GetAcadPoint(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)




            tBlockRefData.Layer = sStageCentroidLayer

            '   mtCurrentParcelKey
            '  tBlockRefData.ScaleFactors = oBlockRef.ScaleFactors


            If miCurrentActionType = UnidivNet.enActionType.Divide Then
               ''''''''''''''  DMCommon.Debug.MsgBox("09_201y", oPolygon.Entity, mtCurrentParcelKey.BlockNo, mtCurrentParcelKey.UD_ParcelName())


               oParcel.Calc()

               '  tBlockRefData.AttribValues = {mtCurrentParcelKey.UD_ParcelName(), oParcel.LegalArea.ToString(), oParcel.CalcArea.ToString(), sPrevParcelName}


               '   DMCommon.Debug.MsgBox("09_955", oParcel.ParcelKey.BlockNo, oParcel.ParcelKey.UD_ParcelName)
               oListDivided.Add(oParcel.ParcelKey)
               '  DMCommon.Debug.MsgBox("09_957", oParcel.Name, oParcel.ParcelKey.Exists, oParcel.ParcelKey.ToString, oListDivided.Count)
               mtCurrentParcelKey.NextParcel()
            End If
            '    DMCommon.Debug.MsgBox("09_908a", oParcel.AcadPoint, tBlockRefData.Layer, tBlockRefData.Position, oParcel.CentroidAcObjID)

            If oParcel.AcadPoint Then
               tCentroidBlockAcObjID = oAcadBlock.InsertRef(tBlockRefData)
               oParcel.UpdateCentroidAcObjId(tCentroidBlockAcObjID, False)
               '    DMCommon.Debug.MsgBox("09_908t", tCentroidBlockAcObjID, oParcel.CentroidAcObjID)
            Else
               '   DMCommon.Debug.MsgBox("09_908m", sStageCentroidLayer, oParcel.CentroidAcObjID)
               DMAcadExt.AcadTransaction.SetLayer(oParcel.CentroidAcObjID, sStageCentroidLayer)
            End If

            ''''''''''''''''''''''''''''''''  oParcel.UpdateBlockAttributes()



            colCentroidsBlocks.Add(tCentroidBlockAcObjID)

            mdicParcels.AddParcel(oParcel)
            ' miLastParcel += 1
         Next
         moCurrentStageTopoScheme = New tsTopology(sStageTopoName)
         moCurrentStageTopoScheme.Load(mbCheckExtended, oTopoModel)
         ' 

         oTopoModel.Close()
         oParcel.PolygonScheme = moCurrentStageTopoScheme.GetPolygon(oParcel.TopoID)
         oTopos.Delete(sStageTopoName, False)
         DMAcadExt.AcadTransaction.EraseDBObjects(colCentroidPoints)
         '  zzDicpCol("colCentroidPoints", colCentroidPoints)
         '  zzDicpCol("colCentroidsBlocks", colCentroidsBlocks)
         '   DMCommon.Debug.MsgBox("11_260", sStageTopoName, colCentroidsBlocks.Count)
         oTopos.Create(sStageTopoName, colTopoLinks, colNodes, colCentroidsBlocks, TopologyTypes.Polygon)

         moStageTopologies.AddTopoName(sStageTopoName)
         DMAcadExt.AcadDocument.Regen()
      End If
      '   DMCommon.Debug.MsgBox("15_125after", sStageTopoName, moCurrentStageTopoScheme Is Nothing, moStageTopologies Is Nothing)
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
   Private Sub zzCreateFinalTopology()
      '  Const sFinalTopologyName As String = "FinalStage"
      Dim oPrevPoint As UnidivNet.UD_Point
      Dim oNextPoint As UnidivNet.UD_Point
      Dim oFLineNode As tsNode

      Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()

      Dim oFinalTopoScheme As tsTopology

      Dim dicFLines As UnidivNet.UD_FLines = New UnidivNet.UD_FLines()
      '  DMCommon.Debug.MsgBox("08_663", DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(moFragmentsToposcheme.Branches), DMCommon.Debug.ColCount(mcolCanceledLinks))
      '  Dim itest As Integer
      '  Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)
      For Each oBranch As tsBranch In moFragmentsToposcheme.Branches
         If Not mcolCanceledLinks.Contains(oBranch.AcObjID) Then
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.PreviousNodeID())
            oPrevPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If
            oFLineNode = moFragmentsToposcheme.GetNode(oBranch.NextNodeID())
            oNextPoint = mdicPoints.GetPoint(oFLineNode.AcObjID)
            If Not colNodes.Contains(oFLineNode.AcObjID) Then
               colNodes.Add(oFLineNode.AcObjID)
            End If

            ' oFLine = New UD_FLine(oPrevPoint, oNextPoint, oBranch.AcObjID)
            '  mdicFLines.Add(oBranch.AcObjID, oFLine)
            colLinks.Add(oBranch.AcObjID)
         End If
      Next
      '    DMCommon.Debug.MsgBox("08_664", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(colLinks), DMCommon.Debug.ColCount(mcolAllNodes), DMCommon.Debug.ColCount(mcolCanceledLinks))
      If zzCreateTopology(msFinalTopologyName, colLinks, colCentroids) Then

         oFinalTopoScheme = New tsTopology(msFinalTopologyName)
         oFinalTopoScheme.Load(mbCheckExtended)
         For Each oNode As tsNode In oFinalTopoScheme.Nodes
            Select Case oNode.BlockName.ToUpper
               Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                  oNode.HasOldPoint = True
            End Select
         Next
         If mbCheckExtended Then
            oFinalTopoScheme.RemovePseudoPoints()
         End If

         ''''''''''''''''''''''' oFinalTopoScheme.CreateDBPolylineMPlus(True)
      End If

      ' DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count)

   End Sub
   Private Function zzCreateTopology(sTopoName As String, colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
      ' zzGetAllParcelCentroids()
      If colCentroids Is Nothing Then
         colCentroids = New ObjectIdCollection()
      End If
      Try
         '   DMCommon.Debug.MsgBox("15_125", sFinalTopoName & ":=" & CStr(colTopoLinks.Count), colCentroids.Count)
         '  DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "zzDebug")
         '      DMAcadExt.AcadTransaction.SetLayer(colCentroids, "zzDebug")


         oTopos.Create(sTopoName, colTopoLinks, mcolAllNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateTopology")

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


         Return oTopos.Exists(msFinalTopologyName)

         '   iStageLinksCount = colLinks.Count
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "CreateTopology")

         Return False
      End Try
      '   DMCommon.Debug.MsgBox("09_774", sFinalTopoName, oTopos.Exists(sFinalTopoName))

   End Function
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
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
      oPromptOpt.AllowNone = Not bOneOnly
      Dim tVector As Autodesk.AutoCAD.Geometry.Vector2d
      '   Dim bRes As Boolean

      ptRes = oEditor.GetPoint(oPromptOpt)
      If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
         Try
            tPoint = ptRes.Value
            tVector = New Autodesk.AutoCAD.Geometry.Vector2d(tPoint.X, tPoint.Y)
            '  bRes = True
            oEditor.WriteMessage("OK!" & vbCrLf)

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
         End Try
      End If
      Return ptRes.Status
   End Function

   Private Function zzGetBlockRefsSet() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim mcolSelectionBlockRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

      Try


         Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
         Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
         'Dim oPolyline As Polyline = New Polyline
         Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
         '	Dim colLines As ObjectIdCollection = New ObjectIdCollection
         Dim iTotal As Integer = 0
         Dim iFound As Integer = 0
         mcolSelectionBlockRefs.Clear()

         Try
            oPromptOpt.AllowDuplicates = False
            oPromptOpt.SingleOnly = False
            oPromptOpt.MessageForAdding = "Add ***"
            oPromptOpt.MessageForRemoval = "Remove ***"

         Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
            System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
         End Try
         Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
         Dim taObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId

         Dim oCurve As Autodesk.AutoCAD.DatabaseServices.Curve
         ptRes = oEditor.GetSelection(oPromptOpt)
         ' System.Windows.Forms.MessageBox.Show(ptRes.Status.ToString(), "05_377")
         If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
            oSelSet = ptRes.Value()
            '	zzTestSet(oSelSet, True)
            taObjIDs = oSelSet.GetObjectIds()


            iFound = mcolSelectionBlockRefs.Count - iTotal
            iTotal = mcolSelectionBlockRefs.Count
            DMAcadExt.AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
            For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)

               oCurve = DMAcadExt.AcadTransaction.GetCurve(taObjIDs(iIndex), True, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

               If oCurve IsNot Nothing Then
                  mcolSelectionBlockRefs.Add(taObjIDs(iIndex))
               End If

            Next
            Return mcolSelectionBlockRefs

         ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
            mcolSelectionBlockRefs.Clear()
            'Exit Do
         Else
            DMAcadExt.AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
            'Exit Do
         End If
         mbEventsEnabled = True
         '	Loop

      Catch oEx As Exception
         mbEventsEnabled = True
      End Try
   End Function

   Private Sub zzCreatePgonsAAA(sStageTopoName As String)
      Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
      Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(miCurrentStage)
      oStageTopoScheme = New TopoManager.TopoScheme.tsTopology(sStageTopoName)
      oStageTopoScheme.Load(mbCheckExtended)
      For Each oNode As tsNode In oStageTopoScheme.Nodes
         Select Case oNode.BlockName.ToUpper
            Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
               oNode.HasOldPoint = True
         End Select
      Next
      oStageTopoScheme.RemovePseudoPoints()
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

      oStageTopoScheme.CreateDBPolylineMPlus(True)



      '  System.Windows.Forms.MessageBox.Show(CStr(iStageNo) & vbCrLf & sPolylineLayer, "02_124")


   End Sub
   Private Sub zzCreatePgonsPlus(Optional hsDestPgonIDs As HashSet(Of Integer) = Nothing)
      '  Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
      Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(miCurrentStage)
      Dim colPolylineObjID As ObjectIdCollection = New ObjectIdCollection()

      'DMCommon.Debug.MsgBox("15_126con", moCurrentStageTopoScheme Is Nothing, moCurrentStageTopoScheme.Nodes Is Nothing, moStageTopologies Is Nothing)
      'For Each oNode As tsNode In moCurrentStageTopoScheme.Nodes
      '   DMCommon.Debug.MsgBox("15_126con", oNode Is Nothing, oNode.BlockName Is Nothing)
      '   Select Case oNode.BlockName.ToUpper
      '      Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
      '         oNode.HasOldPoint = True
      '   End Select
      'Next
      '  DMCommon.Debug.MsgBox("15_126After", "zzCreatePgonsPlus")
      zzLoadStageNodes(moCurrentStageTopoScheme, miLastPoint)
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

      moCurrentStageTopoScheme.RemovePseudoPoints()

      moCurrentStageTopoScheme.CreateDBPolylineMPlus(True, hsDestPgonIDs)
      ''''''''''''''''    zzFillParcelLogTable(colPolylineObjID)

      '   UnidivNet.UD_App.WriteAllPointsInfo()
      '   UnidivNet.UD_App.LoadStageBranches(oStageTopoScheme, miCurrentStage)
      '    UnidivNet.UD_App.InsertFlines(miCurrentStage)





   End Sub
   Private Sub zzCreatePgonsPlusAAA(sStageTopoName As String)
      Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
      Dim sPolylineLayer As String = UnidivNet.UD_App.GetStagePolineLayer(miCurrentStage)
      Dim colPolylineObjID As ObjectIdCollection = New ObjectIdCollection()
      oStageTopoScheme = New tsTopology(sStageTopoName)

      oStageTopoScheme.Load(True)
      For Each oNode As tsNode In oStageTopoScheme.Nodes
         Select Case oNode.BlockName.ToUpper
            Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
               oNode.HasOldPoint = True
         End Select
      Next
      oStageTopoScheme.RemovePseudoPoints()
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(sPolylineLayer, DMAcadExt.DMApp.AppID, True, True)

      oStageTopoScheme.CreateDBPolylineMPlus(True)
      zzFillParcelLogTable(colPolylineObjID)
      UnidivNet.UD_App.LoadStageNodes(oStageTopoScheme, miCurrentStage, miLastPoint)
      '   UnidivNet.UD_App.WriteAllPointsInfo()
      '   UnidivNet.UD_App.LoadStageBranches(oStageTopoScheme, miCurrentStage)
      '    UnidivNet.UD_App.InsertFlines(miCurrentStage)





   End Sub
   Private Sub zzFillParcelLogTable(colPolylineObjID As ObjectIdCollection)
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
         zzAddGridRow(enRowStatus.Default)
      End If
      oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
      Return oGridRow
   End Function
   Private Function zzGetDataRow(iRowIndex As Integer) As DataRow
      Dim oDataRow As DataRow = Nothing

      If iRowIndex > moMainTable.Rows.Count - 1 Then
         zzAddGridRow(enRowStatus.Default)
      End If
      oDataRow = moMainTable.Rows.Item(iRowIndex)
      Return oDataRow
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

               Dim oParcel As UnidivNet.UD_Parcel

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
      If mfUD_General.Visible <> bVisible Then
         mfUD_General.Visible = bVisible
      End If
   End Sub

   Private Sub Button1_Click(oSender As System.Object, e As EventArgs)
      DMCommon.Debug.MsgBox("09_788", miCurrentStage, miCurrentAction)
   End Sub

   Private Sub zzMarkParcel(tParcelKey As UD_ParcelKey)
      'Dim eui As EditorUserInteraction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.StartUserInteraction(Me)


      '   DMCommon.Debug.MsgBox("09_875a_MARK", tParcelKey.ToString())
      'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      'DMAcadExt.AcadTransaction.Start()
      'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      '  moShrinkPgonJig.GetEntity().AddToDatabase()






      Dim oaLinks As DMAcadExt.IUD_Link() = mdicParcels.GetParcelLinks(tParcelKey)
      Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(oaLinks, oaLinks.GetUpperBound(0) + 1, 1, False)
      '  DMAcadExt.AcadDocument.SetDrawVectorSet(New DMAcadExt.DrawVectorSet(tVectorSet))
      DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, -1)



      'DMAcadExt.AcadTransaction.CloseModelSpace()

      'DMAcadExt.AcadTransaction.Terminate()
      'DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub zzMarkParcel(oParcel As UnidivNet.UD_Parcel, iIndex As Integer, bDWGOpen As Boolean)
      '  DMCommon.Debug.MsgBox("09_588", oParcel.BorderObjID)
      If bDWGOpen Then
         zzOpenDWG()
      End If
      Dim oaLinks As DMAcadExt.IUD_Link() = oParcel.GetLinks() ' mdicParcels.GetParcelLinks(oParcel)
      If oaLinks IsNot Nothing Then
         Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(oaLinks, oaLinks.GetUpperBound(0), 1, False)
         '  DMAcadExt.AcadDocument.SetDrawVectorSet(New DMAcadExt.DrawVectorSet(tVectorSet))
         DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, iIndex)
      End If


      If bDWGOpen Then
         zzCloseDWG()
      End If


   End Sub
   Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
      'Dim eui As EditorUserInteraction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.StartUserInteraction(Me)
      Dim oCurrentRow As DataGridViewRow = Me.dgvMain.CurrentRow
      Dim tParcelKey As UD_ParcelKey = UD_ParcelKey.FromGrid(miBlockNo, oCurrentRow.Cells.Item(2).Value, oCurrentRow.Cells.Item(3).Value)
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

   Private Sub zzOpenDWG(Optional bModelSpace As Boolean = True, Optional bFragmentTopology As Boolean = True)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      If bModelSpace Then
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
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

   Private Sub cmdInsertFLine_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertFLine.Click
      zzCloseLastRow()
      zzOpenDWG()
      zzInsertAllFLines()
      zzCloseDWG()
      zzAllUnenabled()
   End Sub

   Private Sub zzInsertFlines(iStage As Integer)
      Dim itest As Integer
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(UnidivNet.UD_FLine.GetStageFLineLayer(iStage), DMAcadExt.DMApp.AppID, True, False)

      For Each oFLine As UnidivNet.UD_FLine In mdicFLines.Values

         If oFLine.Stage = iStage Then
            itest += 1
            oFLine.InsertBlock()
         End If

      Next
      '     DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest, iStage)

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
            Select Case oNode.BlockName.ToUpper
               Case UnidivNet.UD_App.OldPointBlockName, UnidivNet.UD_App.OldHorizontalPointBlockName, UnidivNet.UD_App.OldVerticalPointBlockName
                  oNode.HasOldPoint = True
            End Select
         Next

         oFinalTopoScheme.RemovePseudoPoints()
         colNodeLinks = oFinalTopoScheme.GetNodeLinksCorrected
         '  DMCommon.Debug.MsgBox("09_766W", colNodeLinks.Count, DMCommon.Debug.ColCount(mdicPoints))
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
         '   DMCommon.Debug.MsgBox("09_760", dicFLines.Count, dicFLines.Values.Count)
         For Each oFLine In dicFLines.Values
            oFLine.InsertBlock()
         Next
      End If

      '  DMCommon.Debug.MsgBox("09_549", mdicFLines.Values.Count, itest)

   End Sub
   Private Sub zzInsertAllFLines()
      Dim sFrontLineLayer As String
      Dim bCurrentLayerOK As Boolean
      If miCurrentStage = 0 Then
         zzInsertFlines(miCurrentStage)
      Else
         zzFinalTopo()
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
   Private Sub zzAllUnenabled()
      Me.rdbTransfer.Enabled = False
      Me.rdbUnion.Enabled = False
      Me.rdbDivide.Enabled = False
      Me.cmdContinue.Enabled = False

   End Sub
   Private Sub zzSetStageViewA(iStageView As Integer)
      UnidivNet.UD_App.SetStageLayerOn(iStageView, True, True)
   End Sub
   Private Sub zzSetStageView()
      If chkHanitView.Checked Then
         Dim iStage As Integer = Convert.ToInt32(Me.nudStagesView.Value)
         zzOpenDWG(False, False)
         zzSetStageViewA(iStage)
         zzCloseDWG()
      End If
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
   Private Sub dgvMain_RowEnter(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.RowEnter
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
      Dim colAttribRefs As ObjectIdCollection



      mcolUnvisibleLayers = DMAcadExt.AcadTransaction.SetLayersOnExcept(tLayerList, False)
      mcolParcelCancelled.Clear()
      mcolNotFocused.Clear()
      For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
         ' DMCommon.Debug.MsgBox("12_288b", oParcel.UD_Name, oParcel.DbID, iFocusedParcelDbID)
         If oParcel.IsCanceled Then
            ' DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.BorderObjID)
            ' DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.CentroidAcObjID)

            If oParcel.BorderObjID.IsNull Then
               DMCommon.Debug.MsgBox("12_282B", "BorderObjID.IsNull", oParcel.UD_Name)
            Else
               mcolParcelCancelled.Add(oParcel.BorderObjID)

            End If
            If oParcel.CentroidAcObjID.IsNull Then
               DMCommon.Debug.MsgBox("12_283C", oParcel.UD_Name)
            Else
               mcolParcelCancelled.Add(oParcel.CentroidAcObjID)


               If False Then
                  colAttribRefs = DMAcadExt.AcadTransaction.GetVisibleAttributes(oParcel.CentroidAcObjID)
                  For Each tAttribRefID As ObjectId In colAttribRefs
                     mcolParcelCancelled.Add(tAttribRefID)
                  Next
               End If

            End If


         End If
         If iFocusedParcelDbID <> 0 AndAlso oParcel.DbID <> iFocusedParcelDbID Then
            mcolNotFocused.Add(oParcel.BorderObjID)
            mcolNotFocused.Add(oParcel.CentroidAcObjID)
            DMAcadExt.AcadTransaction.SetAttributesByBlock(oParcel.CentroidAcObjID)
         End If

      Next
      DMAcadExt.AcadTransaction.SetVisible(mcolParcelCancelled, False)
      DMAcadExt.AcadTransaction.SetColor(mcolNotFocused, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, 251S))


      ' DMCommon.Debug.MsgBox("12_280B", mcolUnvisibleLayers.Count, tLayerList.List, mcolParcelCancelled.Count, mcolNotFocused.Count)
   End Sub
   Private Sub zzRestoreView()

      DMAcadExt.AcadTransaction.SetLayersOffStatus(mcolUnvisibleLayers, False)
      mcolUnvisibleLayers = New ObjectModel.ObservableCollection(Of String)()
      DMAcadExt.AcadTransaction.SetVisible(mcolParcelCancelled, True)
      DMAcadExt.AcadTransaction.SetColor(mcolNotFocused, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256S))
      '   DMCommon.Debug.MsgBox("12_280A", mcolUnvisibleLayers.Count, mcolParcelCancelled.Count, mcolNotFocused.Count)
   End Sub
   Private Sub cmdSelectPgon_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectPgon.Click
      If miCurrentActionFirstRowIndex <> -1 Then
         zzOpenDWG(False, False)
         zzActiveParcelsView(moUD_ParcelLayerList)
         zzCloseDWG()
         zzOpenDWG()
         Me.Visible = False
         AppActivate(moAppWin.Text)
         Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

         Select Case miCurrentActionType
            Case UnidivNet.enActionType.Divide
               Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
               Dim tSourceParcelKey As UD_ParcelKey
               Dim oSourceParcel As UnidivNet.UD_Parcel = Nothing

               ' "C1603_*"
               If zzSelectPoint(tPoint, True) = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, tSourceParcelKey) Then
                  ' DMCommon.Debug.MsgBox("12_060", tPoint, tSourceParcelKey)
                  If tSourceParcelKey.Exists AndAlso mdicParcels.TryGetValue(tSourceParcelKey, oSourceParcel) Then
                     '  DMCommon.Debug.MsgBox("09_991d_Div", oSourceParcel.AcadArea, oSourceParcel.LegalArea, oSourceParcel.BorderObjID)
                     zzSetSourceParcel(oSourceParcel, False)
                     '  DMAcadExt.AcadDocument.ClearDrawVectorSet()
                     zzMarkParcel(oSourceParcel, 0, False)
                     '  DMCommon.Debug.MsgBox("09_912t", Me.dgvMain.AllowUserToAddRows)
                  End If
               End If
               ' Me.Visible = True
               'zzRestoreView()
               '  zzSelectFromStagesTopologies(False)
            Case UnidivNet.enActionType.Union
               Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = Nothing
               ' zzActiveParcelsView(False)
               '   DMAcadExt.AcadTransaction.ReStart()

               If zzSelectParcelColByFragment(colParcels) Then
                  '   DMCommon.Debug.MsgBox("11_163", colParcels.Count)
                  zzParcelsToGrid(colParcels)
                  '''''''''''''''''''''''''''''''''''''''''''    zzBeforeUnion(colParcels)
               End If


               ''''''''''''''''''''''''''''''''''''''''''''''''''' zzRestoreView()
            Case UnidivNet.enActionType.Transfer
               Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
               Dim tSourceParcelKey As UD_ParcelKey
               ' zzActiveParcelsView(False)

               If zzSelectPoint(tPoint, True) = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, tSourceParcelKey) Then
                  ''''''''  DMCommon.Debug.MsgBox("12_055", tPoint, tSourceParcelKey)
                  zzTransferFromFinalTopology(tSourceParcelKey)
               End If


         End Select
         Me.Visible = True
         zzRestoreView()
         zzCloseDWG()

      End If

   End Sub
   Private Sub zzInitLayers()
      Dim saLayer(5) As String
      Dim iStageNo As Integer = 0
      Dim oGridRow As System.Windows.Forms.DataGridViewRow
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
   Private Sub zzTransferFromFinalTopology(tParcelKey As UD_ParcelKey)
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim sParcelName As String

      Dim oParcel As UnidivNet.UD_Parcel = Nothing

      Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      '  Dim oPolygon As Polygon = Nothing
      Dim oFinalTopoScheme As tsTopology
      Dim oPgon As tsPolygon

      Dim iPgonID As Integer
      Dim colPolylineObjID As ObjectIdCollection = New ObjectIdCollection()
      '   DMCommon.Debug.MsgBox("12_040", tParcelKey)
      Dim oFinalTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msFinalTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If Not tParcelKey.Exists Then
         tPoint = oParcel.CenterPosition.AcGePoint3d
         DMCommon.Debug.MsgBox("11_801s", tPoint)
         zzSelectParcelFromFinalTopo(tPoint, iPgonID, tParcelKey)

      End If


      If tParcelKey.Exists AndAlso mdicParcels.TryGetValue(tParcelKey, oParcel) Then
         ' DMCommon.Debug.MsgBox("09_991d_Tr", oParcel.AcadArea, oParcel.LegalArea, oParcel.ParcelArea.LegalArea, oParcel.BorderObjID)
         oParcel.IsCanceled = True
         oParcel.IsMoved = True
         oParcel.UpdateBlockAttributes()
         Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
         Dim oFirstActionDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
         zzFillFromParcelKey(oParcel, oFirstActionGridRow)
         zzFillParcelAreaNew(oParcel.ParcelArea, oFirstActionDataRow, oFirstActionGridRow)
         oFirstActionDataRow.Item("AcObjID") = oParcel.CentroidAcObjID
      End If
      '   DMCommon.Debug.MsgBox("11_350", msFinalTopologyName)
      oFinalTopoScheme = New tsTopology(msFinalTopologyName)

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
      If oParcel IsNot Nothing AndAlso Not oParcel.BorderObjID.IsNull Then
         Dim oPLine As Polyline = DMAcadExt.AcadTransaction.GetPolyline(oParcel.BorderObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
         Dim oNewPline As Polyline = New Polyline()

         oNewPline.CopyFrom(oPLine)
         oNewPline.Layer = sPolylineLayer
         '     DMCommon.Debug.MsgBox("12_080", sPolylineLayer)
         DMAcadExt.AcadTransaction.AppendEntity(oNewPline)



      End If

   End Sub
   Private Function zzSelectParcelByFragment(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef tParcelKey As UD_ParcelKey) As Boolean
      '   DMCommon.Debug.MsgBox("12_050", tPoint)
      Dim bFragmentTopologyClose As Boolean
      Dim bRes As Boolean = False
      '  If moFragmentTopology Is Nothing OrElse moFragmentTopology.Status = Status.Closed Then
      'moFragmentTopology = TopoManager.TopoCreator.GetOpenedTopology("Fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
      ' bFragmentTopologyClose = True
      ' End If

      If moFragmentTopology IsNot Nothing AndAlso moFragmentTopology.Status <> Status.Closed Then
         Dim oPolygon As Polygon
         Dim iPolygonID As Integer
         Dim oFragment As Fragment
         '   DMCommon.Debug.MsgBox("12_051", "A")
         Try
            oPolygon = moFragmentTopology.FindPolygon(tPoint)
            iPolygonID = oPolygon.ID
         Catch oEx As Exception
            DMCommon.Debug.MsgBox("12_052 Err", tPoint, moFragmentTopology.GetPolygons().Count)
         End Try
         '  DMCommon.Debug.MsgBox("12_052", iPolygonID)
         If iPolygonID <> 0 AndAlso mdicFragments.TryGetParcel(iPolygonID, tParcelKey) Then
            'DMCommon.Debug.MsgBox("12_053", iPolygonID, tParcelKey)
            bRes = True
         End If
      End If
      '   If bFragmentTopologyClose Then
      'moFragmentTopology.Close()
      'moFragmentTopology = Nothing
      'End If
      Return bRes
   End Function
   Private Function zzSelectParcelFromFinalTopo(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef iPolygonID As Integer, ByRef tParcelKey As UD_ParcelKey) As Boolean
      Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      Dim oPolygon As Polygon = Nothing
      Dim sParcelName As String
      Dim oFinalTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msFinalTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
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

            tParcelKey = New UD_ParcelKey(miBlockNo, sParcelName)
            DMCommon.Debug.MsgBox("11_221V", "tParcelKey=" & tParcelKey.ToString())
            Return True
         End If
      End If
      Return False
   End Function
   Private Sub zzSelectFromStagesTopologies(bOneOnly As Boolean)


      Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
      Dim oParcel As UnidivNet.UD_Parcel = Nothing

      Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus


      zzOpenDWG()



      '  DMCommon.Debug.MsgBox("09_224q", sParcelName, iSelectStatus, miCurrentActionType, oParcel.ParcelKey.ParcelNo)
      If iSelectStatus = PromptStatus.None OrElse iSelectStatus = PromptStatus.OK Then
         If miCurrentActionType = UnidivNet.enActionType.Divide Then
            Dim oFirstActionRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
            '  oFirstActionRow.Cells.Item("ctxFromParcel").Value = oParcel.Name
            '  zzFillParcelArea(oParcel.ParcelArea, oFirstActionRow)
            zzFillFromParcelKey(oParcel, oFirstActionRow)
         ElseIf miCurrentActionType = UnidivNet.enActionType.Union Then
            zzBeforeUnion(colParcels)
            ''''''''''''''''''''' mdicParcels.DebugMsg()
         End If
      End If
      zzCloseDWG()
   End Sub

   Private Sub cmdSelectLinks_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectLinks.Click
      If miCurrentActionFirstRowIndex <> -1 AndAlso miCurrentActionType = UnidivNet.enActionType.Divide Then
         zzBeforeDivide(False)
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

   End Sub
   Private Function zzSelectParcelColByFragment(ByRef colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel)) As Boolean
      Dim bOneOnly As Boolean = False
      Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim sParcelName As String = Nothing
      colParcels = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
      '  moStageTopologies.OpenTopos()
      '    Me.Visible = False
      '   AppActivate(moAppWin.Text)
      '   Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()



      Do
         iSelectStatus = zzSelectPoint(tPoint, bOneOnly)
         If iSelectStatus = PromptStatus.OK AndAlso zzSelectParcelByFragment(tPoint, tParcelKey) Then
            If True Then
               'sParcelName = moStageTopologies.FindPolygon(tPoint)

               '  tParcelKey = New UD_ParcelKey(miBlockNo, sParcelName)
               '   DMCommon.Debug.MsgBox("11_240", tParcelKey.BlockNo, tParcelKey.ParcelNo, tParcelKey.Original, tParcelKey.CompareKey)
               If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso oParcel.IsResult Then
                  colParcels.Add(oParcel)
                  If miCurrentActionType = UnidivNet.enActionType.Union Then
                     ' DMCommon.Debug.MsgBox("12_202", tParcelKey, oParcel.ParcelKey)


                  End If


                  zzMarkParcel(oParcel, -1, False)
                  If miCurrentActionType = UnidivNet.enActionType.Divide Then
                     DMCommon.Debug.MsgBox("09_207", sParcelName, oParcel)
                     Exit Do
                  End If
               End If
            End If
            DMAcadExt.AcadDocument.WriteMessage(" גוש " & tParcelKey.BlockNo & "; חלקה " & tParcelKey.UD_ParcelName & vbCrLf)
         Else
            Exit Do
         End If
      Loop

      '   DMCommon.Debug.MsgBox("11_162", colParcels.Count)
      '  Me.Visible = True
      '  moStageTopologies.CloseTopos()
      Return colParcels.Count > 0
   End Function
   Private Function zzSelectParcelColAAA(ByRef colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel)) As Boolean
      Dim bOneOnly As Boolean = False
      Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim sParcelName As String = Nothing
      colParcels = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
      moStageTopologies.OpenTopos()
      Me.Visible = False
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

      Do
         iSelectStatus = zzSelectPoint(tPoint, bOneOnly)
         If iSelectStatus = PromptStatus.OK Then

            sParcelName = moStageTopologies.FindPolygon(tPoint)

            tParcelKey = New UD_ParcelKey(miBlockNo, sParcelName)
            DMCommon.Debug.MsgBox("11_240", tParcelKey.BlockNo, tParcelKey.ParcelNo, sParcelName, tParcelKey.Original, tParcelKey.CompareKey)
            If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso oParcel.IsResult Then
               colParcels.Add(oParcel)
               zzMarkParcel(oParcel, -1, False)
               If miCurrentActionType = UnidivNet.enActionType.Divide Then
                  DMCommon.Debug.MsgBox("09_207", sParcelName, oParcel)
                  Exit Do
               End If
            End If
            DMAcadExt.AcadDocument.WriteMessage(" גוש " & tParcelKey.BlockNo & "; חלקה " & tParcelKey.UD_ParcelName & vbCrLf)
         Else
            Exit Do
         End If
      Loop

      DMCommon.Debug.MsgBox("11_162", colParcels.Count)
      Me.Visible = True
      moStageTopologies.CloseTopos()
      Return colParcels.Count > 0
   End Function
   Private Function zzGetParcelColFromGrid(ByRef colParcels As ICollection(Of UnidivNet.UD_Parcel)) As Boolean
      Dim bOneOnly As Boolean = False

      Dim tParcelKey As UD_ParcelKey
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      Dim sParcelName As String = Nothing
      ' Dim oDataViewRow As DataView


      colParcels = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

      For Each oGridRow As System.Windows.Forms.DataGridViewRow In dgvMain.SelectedRows()

         tParcelKey = zzGetSourceParcelKey(oGridRow)

         If mdicParcels.TryGetValue(tParcelKey, oParcel) AndAlso oParcel.IsResult Then
            colParcels.Add(oParcel)
         End If
      Next

      Return colParcels.Count > 1
   End Function
   Private Sub chkHanitView_CheckedChangedOld(oSender As System.Object, e As EventArgs) 'Handles chkHanitView.CheckedChanged
      Dim bIsOn As Boolean = Me.chkHanitView.Checked
      If True OrElse Me.chkHanitView.Checked Then
         Dim oRow As UD_Row
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
      If Integer.TryParse(Me.txtGushNo.Text, miBlockNo) Then

         zzLoadJournalTable(False)
         zzCalcJournalAddFields()
         zzLoadParcelLogTable(False)

         If mdicFragments.DBVersionIsCorrect Then
            zzInterpretJournal()
         End If

         ' DMCommon.Debug.MsgBox("09_655x", moMainTable.Rows.Count, moMainTable.Columns.Count, mdicParcels.Count)
         '''''''''''  zzLoadDWG()
         '''''''''''''' zzFillGrid()
      End If
   End Sub


   Private Sub cmdSaveDB_Click(oSender As System.Object, e As EventArgs) Handles cmdSaveDB.Click
      '  
      ' DMCommon.ExcelLogG.Open()
      '  DMCommon.Debug.MsgBox("09_650x", moMainTable.Rows.Count, moMainTable.Columns.Count)
      '  DMCommon.ExcelLogG.SetDataTable(moMainTable, 0)
      'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
      'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
      'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
      'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
      'moMainTable.Columns.RemoveAt(moMainTable.Columns.Count - 1)
      'DMCommon.ExcelLogG.SetDataTable(moMainTable, 5)
      Me.Cursor = Cursors.WaitCursor
      If mbNewData Then

      End If
      zzClearGushData()
      '   DMCommon.Debug.MsgBox("Wait")
      moJournalDataAdapter.Update(moMainTable)
      '   moParcelLogDataAdapter.Update(moParcelLogTable)
      '   moFragmentsDataAdapter.Update(moFragmentTable)
      zzUpdateFragmentTable()
      zzUpdateParcelFragmentTable()
      Me.Cursor = Cursors.Default
   End Sub
   Private Sub zzClearGushData()

      Const sSPName As String = "ClearBlockData"

      Dim iRes As Integer = TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())
   End Sub
   Private Sub zzUpdateStagesView()
      Me.nudStagesView.Maximum = miCurrentStage
   End Sub
   Private Sub zzUpdateFragmentTable()
      Dim oNewRow As DataRow

      ' Dim oFragmentA As Fragment
      zzLoadFragmentsTable(True)
      For Each oFragment As Fragment In mdicFragments.Values
         Try

            ' oFragment = mdicFragmentsA.FragmentArray(iIndex)
            oNewRow = moFragmentTable.NewRow()


            oNewRow.Item("ProjectCode") = miProjectCode
            oNewRow.Item("Detail") = miDetailNo
            oNewRow.Item("OriginalBlockNo") = miBlockNo
            oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

            oNewRow.Item("FragmentID") = oFragment.DWGTopoID
            oNewRow.Item("CentroidHandle") = oFragment.HandleVal
            oNewRow.Item("ParcelNo") = oFragment.SourceParcelNo
            oNewRow.Item("ParcelIsOriginal") = oFragment.SourceParcelKey.Original
            oNewRow.Item("CentroidX") = oFragment.CentroidX
            oNewRow.Item("CentroidY") = oFragment.CentroidX
            moFragmentTable.Rows.Add(oNewRow)

         Catch oEx As Exception

         End Try
         '   DMCommon.Debug.MsgBox("bef moFragmentTable.Rows", moFragmentTable.Rows.Count)

         '  DMCommon.Debug.MsgBox("aft moFragmentTable.Rows", moFragmentTable.Rows.Count)

      Next
      moFragmentsDataAdapter.Update(moFragmentTable)
   End Sub
   Private Sub zzUpdateParcelFragmentTable()
      Dim oNewRow As DataRow

      ' Dim oFragmentA As Fragment
      zzLoadParcelFragmentsTable(True)
      '    DMCommon.Debug.MsgBox("08_577x", mdicParcels.Count, UnidivNet.UD_Parcel.DbIDCounter)
      For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
         For Each iFragmentID As Integer In oParcel.Fragments
            Try
               ' oFragment = mdicFragmentsA.FragmentArray(iIndex)
               oNewRow = moParcelFragmentTable.NewRow()


               oNewRow.Item("ProjectCode") = miProjectCode
               oNewRow.Item("Detail") = miDetailNo
               oNewRow.Item("OriginalBlockNo") = miBlockNo
               oNewRow.Item("OriginalBlockAddNo") = miBlockAddNo

               oNewRow.Item("ParcelDbID") = oParcel.DbID
               oNewRow.Item("ParcelNo") = oParcel.ParcelKey.ParcelNo
               oNewRow.Item("ParcelIsOriginal") = oParcel.ParcelKey.Original




               oNewRow.Item("FragmentID") = iFragmentID


               moParcelFragmentTable.Rows.Add(oNewRow)

            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oParcel.ParcelKey.ToString() & vbCrLf & iFragmentID.ToString(), "UpdateParcelFragmentTable")
            End Try
         Next

         '  DMCommon.Debug.MsgBox("bef Update", moParcelFragmentTable.Rows.Count)

         '  DMCommon.Debug.MsgBox("aft moFragmentTable.Rows", moFragmentTable.Rows.Count)

      Next
      moParcelFragmentsDataAdapter.Update(moParcelFragmentTable)
   End Sub
   Private Sub chkDataBound_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkDataBound.CheckedChanged
      If mbEventsEnabled AndAlso Me.chkDataBound.Checked Then
         zzSetDB()
      End If

   End Sub
   Private Sub zzOpenDB(bSchemaOnly As Boolean)

      zzLoadFragmentsTable(bSchemaOnly)
      zzLoadParcelFragmentsTable(bSchemaOnly)
      '  zzFillInitGrid()
   End Sub
   Private Sub zzSetDB()


      If Integer.TryParse(Me.txtGushNo.Text, miBlockNo) Then
         zzLoadJournalTable(True)
         zzLoadParcelLogTable(True)

         DMCommon.Debug.MsgBox("09_650", moMainTable.Rows.Count, moMainTable.Columns.Count)
         '''''''''''''''''  zzLoadDWG()
         zzFillInitGrid()
      End If

   End Sub
   Private Structure PlanExt
      Dim Plan As String
      Dim LanduseName As String
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


      Private mdAreaSum As Double


      Public Sub New(iActionType As UnidivNet.enActionType, iTableNumber As Integer)
         miActionType = iActionType
         miTableNumber = iTableNumber
         zzOpenAcadBlocks()
         mtHeaderStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdHeaderHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)
         mtTableWidth = New Autodesk.AutoCAD.Geometry.Vector3d(-mdTableWidth * AcadReport.RepApp.DrawingScaleFactor, 0.0, 0.0)

         mtRowStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdRowHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)
         mtAfterTableStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdAfterTableDistance * AcadReport.RepApp.DrawingScaleFactor, 0.0)


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


         Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLayer, DMAcadExt.DMApp.AppID, True, True)
         zzInsertHeader()

         mtCurrentPoint = mtBaseBoint.Add(mtHeaderStep)
      End Sub
      Public Sub InsertRow(tParcelKey As UD_ParcelKey, tPlanExt As PlanExt, dArea As Double, bTransfer As Boolean, tTransferToParcelKey As UD_ParcelKey)
         zzInsertRow(tParcelKey, tPlanExt, dArea, bTransfer, tTransferToParcelKey)
         mtCurrentPoint = mtCurrentPoint.Add(mtRowStep)
         mdAreaSum += dArea
      End Sub
      Public Sub InsertSumRow(tParcelKey As UD_ParcelKey)
         Dim tPlanExt As PlanExt = New PlanExt()
         zzInsertLine()
         zzInsertRow(tParcelKey, tPlanExt, mdAreaSum, False, New UD_ParcelKey())
         mtCurrentPoint = mtCurrentPoint.Add(mtRowStep)
         zzInsertLine()

         '''''''''''''''     zzInsertLine()
         mdAreaSum = 0.0
      End Sub

      Private Sub zzOpenAcadBlocks()
         Dim msHeaderBlockName As String
         Dim msRowBlockName As String
         Dim saRowTags() As String
         Dim msUniDivRowTags() As String = {"TABA_MIGRASH", "TEMP_NAME", "FINAL_NAME", "LEGAL_AREA", "ROW_NUM", "TBL_NUM", "SUMMARIZE_ROW", "TABA_YEUD_DESC"}
         Dim msTransferRowTags() As String = {"TEMP_NAME", "FINAL_NAME", "LEGAL_AREA", "TO_GUSH", "TO_TEMP_NAME", "TO_FINAL_NAME", "ROW_NUM", "TBL_NUM"}

         '    TBL_NUM	ROW_NUM	TO_FINAL_NAME	TO_TEMP_NAME	TO_GUSH	LEGAL_AREA	FINAL_NAME	TEMP_NAME
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
         '    DMCommon.Debug.MsgBox("12_280", oNewLine.StartPoint, oNewLine.EndPoint)

         DMAcadExt.AcadTransaction.AppendEntity(oNewLine)
      End Sub

      Private Sub zzInsertHeader()
         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
         tBlockRefData.Position = mtBaseBoint
         tBlockRefData.AttribValues = {miTableNumber.ToString()}
         tBlockRefData.Layer = msLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
         moHeaderAcadBlock.InsertRef(tBlockRefData)
      End Sub
      Private Sub zzInsertRow(tParcelKey As UD_ParcelKey, tPlanExt As PlanExt, dArea As Double, bTransfer As Boolean, tTransferToParcelKey As UD_ParcelKey)
         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
         tBlockRefData.Position = mtCurrentPoint
         If bTransfer Then
            tBlockRefData.AttribValues = {tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), tTransferToParcelKey.BlockName, tTransferToParcelKey.TempName, tTransferToParcelKey.FinalName, miRowNumber.ToString(), miTableNumber.ToString()}
         Else
            tBlockRefData.AttribValues = {tPlanExt.LotName, tParcelKey.TempName, tParcelKey.FinalName, FormatNumber(dArea, 3), miRowNumber.ToString(), miTableNumber.ToString(), "0", DMCommon.Hebrew.WordToDOS(tPlanExt.LanduseName, False)}  '"Yeud"
         End If

         tBlockRefData.Layer = msLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
         moRowAcadBlock.InsertRef(tBlockRefData)
         miRowNumber += 1
      End Sub
   End Class
   Private Class AreaTable
      Const msFDBTableLayer As String = "FDB_TABLE"
      Const msLRTableLayer As String = "LR_TABLE"

      Const msFDBHeaderBlockName As String = "FDB_HEADER" ' "LR_HEADER - Copy" '
      Const msLRHeaderBlockName As String = "LR_HEADER"
      Const msFDBRowBlockName As String = "FDB_TABLE"  '"LR_TABLE - Copy"  '
      Const msLRRowBlockName As String = "LR_TABLE"

      Private mdHeaderHeight As Double = 10.0
      Const mdRowHeight As Double = 5.0
      Const dAfterTableDistance As Double = 20.0
      Private moHeaderAcadBlock As DMAcadExt.AcadBlock

      Private moRowAcadBlock As DMAcadExt.AcadBlock
      Private mtBasePoint As Autodesk.AutoCAD.Geometry.Point3d
      Private mtCurrentPoint As Autodesk.AutoCAD.Geometry.Point3d
      Private mtHeaderStep As Autodesk.AutoCAD.Geometry.Vector3d
      Private mtRowStep As Autodesk.AutoCAD.Geometry.Vector3d
      Private miRowNumber As Integer = 1
      Private miAreaTable As enAreaTable
      Private msLayer As String
      Private msHeaderBlockName As String
      Private msaHeaderFields() As String
      Private msRowBlockName As String
      Private msaRowFields() As String

      Public Sub InsertHeader()
         Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(msLayer, DMAcadExt.DMApp.AppID, True, True)
         zzInsertHeader()
         DMAcadExt.AcadDocument.WriteDebugMessage("Pt01" & mtBasePoint.ToString() & "; " & mtHeaderStep.ToString() & "; " & "!!")
         mtCurrentPoint = mtBasePoint.Add(mtHeaderStep)
         DMAcadExt.AcadDocument.WriteDebugMessage("Pt02" & mtCurrentPoint.ToString() & "; " & mtHeaderStep.ToString() & "; " & "!!")
      End Sub
      Public Sub InsertRow(tParcelKey As UD_ParcelKey, tParcelArea As ParcelArea)
         zzInsertRow(tParcelKey, tParcelArea)
         mtCurrentPoint = mtCurrentPoint.Add(mtRowStep)
         DMAcadExt.AcadDocument.WriteDebugMessage("Pt03" & mtCurrentPoint.ToString() & "; " & mtRowStep.ToString() & "; " & "!!")
      End Sub

      Private Sub zzInsertHeader()
         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
         tBlockRefData.Position = mtBasePoint

         tBlockRefData.Layer = msLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
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
               mdHeaderHeight = 6.0
            Case enAreaTable.LR
               msLayer = msLRTableLayer
               msHeaderBlockName = msLRHeaderBlockName
               msaHeaderFields = {"COMMENT"}
               msRowBlockName = msLRRowBlockName
               msaRowFields = {"ROW_NUM", "PARCEL_NAME", "CALC_AREA"}
         End Select

         mtHeaderStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdHeaderHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)
         mtRowStep = New Autodesk.AutoCAD.Geometry.Vector3d(0.0, -mdRowHeight * AcadReport.RepApp.DrawingScaleFactor, 0.0)
         moHeaderAcadBlock = New DMAcadExt.AcadBlock(msHeaderBlockName, UnidivNet.UD_App.BlockPath13)
         moHeaderAcadBlock.OpenForRight(False)
         If msaHeaderFields IsNot Nothing Then
            moHeaderAcadBlock.Fields = msaHeaderFields
         End If

         moRowAcadBlock = New DMAcadExt.AcadBlock(msRowBlockName, UnidivNet.UD_App.BlockPath13)
         moRowAcadBlock.OpenForRight(False)
         moRowAcadBlock.Fields = msaRowFields
      End Sub
      Public Sub SetBasePoint(ByRef tBaseBoint As Autodesk.AutoCAD.Geometry.Point3d)
         mtBasePoint = tBaseBoint

         mtCurrentPoint = mtBasePoint
      End Sub
      Private Sub zzInsertRow(tParcelKey As UD_ParcelKey, tParcelArea As ParcelArea)
         Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
         tBlockRefData.Position = mtCurrentPoint
         Select Case miAreaTable
            Case enAreaTable.FDB
               tBlockRefData.AttribValues = {miRowNumber.ToString(), tParcelKey.FinalName, FormatNumber(tParcelArea.LegalArea, 3), FormatNumber(tParcelArea.CalcArea, 3), FormatNumber(tParcelArea.DeltaArea, 3), DMCommon.Hebrew.WordToDOS(tParcelArea.IsProperText, False)}
            Case enAreaTable.LR
               tBlockRefData.AttribValues = {miRowNumber.ToString(), tParcelKey.UD_ParcelName, FormatNumber(tParcelArea.CalcArea, 3)}
         End Select


         tBlockRefData.Layer = msLayer
         tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor)
         moRowAcadBlock.InsertRef(tBlockRefData)
         miRowNumber += 1
      End Sub
   End Class

   Private Sub cmdInsertTable_ClickBackup(oSender As System.Object, e As EventArgs) ' Handles cmdInsertTable.Click

      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Dim oJournalTable As JournalTable = Nothing '= New JournalTable(UnidivNet.enActionType.Divide, 1)
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d


      Dim iStage As Integer
      Dim iAction As Integer
      Dim iActionType As UnidivNet.enActionType
      Dim iRowStatus As enRowStatus
      Dim tSumParcelKey As UD_ParcelKey
      Dim tRowParcelKey As UD_ParcelKey
      Dim tTransferToRowParcelKey As UD_ParcelKey


      Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
      Dim dLegalAreaD As Double
      Dim tSum As UD_ParcelKey
      Dim tPlanExt As PlanExt
      '  Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      If zzSelectPoint(tPoint, True) = PromptStatus.OK Then


         For Each oDataRow As DataRow In moMainTable.Rows
            tPlanExt = New PlanExt()
            iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
            If iStage <> 0 Then
               iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
               iActionType = zzToActionType(oDataRow)
               iRowStatus = zzToRowStatus(oDataRow)
               dLegalAreaD = DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))

               If iRowStatus = enRowStatus.StartOfAction OrElse iRowStatus = enRowStatus.Transfer Then
                  tSumParcelKey = zzGetSumParcelKey(iActionType, oDataRow)
                  'If iActionType = UnidivNet.enActionType.Divide Then
                  '   'tSumParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
                  '   tSumParcelKey = zzParcelKeyFrom(oDataRow)
                  'Else
                  '   'tSumParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
                  '   tSumParcelKey = zzParcelKeyTo(oDataRow)
                  'End If
                  If iAction = 1 Then
                     If oJournalTable IsNot Nothing Then
                        tPoint = oJournalTable.GetNewBasePoint()
                     End If
                     oJournalTable = New JournalTable(iActionType, iStage)
                     oJournalTable.SetBasePoint(tPoint)
                     oJournalTable.InsertHeader()
                  End If
               End If

               If iActionType = UnidivNet.enActionType.Divide Then
                  ' tRowParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
                  tRowParcelKey = zzParcelKeyTo(oDataRow)
                  tSumParcelKey.FinalNameInBrackets = True
                  tPlanExt = New PlanExt(oDataRow)
               Else
                  '  tRowParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
                  tRowParcelKey = zzParcelKeyFrom(oDataRow)
               End If
               If iActionType = UnidivNet.enActionType.Transfer Then
                  tTransferToRowParcelKey = zzParcelKeyTo(oDataRow)
               End If

               If mdicParcels.TryGetValue(tRowParcelKey, oRowParcel) Then
                  oJournalTable.InsertRow(tRowParcelKey, tPlanExt, oRowParcel.ParcelArea.LegalArea, iActionType = UnidivNet.enActionType.Transfer, tTransferToRowParcelKey)
                  'iActionType
               Else
                  DMCommon.Debug.MsgBox("09_771", tRowParcelKey, mdicParcels.Count)
               End If



               'End If
               If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
                  oJournalTable.InsertSumRow(tSumParcelKey)
               End If
            End If
         Next
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Private Sub cmdInsertTable_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertTable.Click
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
      Me.Visible = False
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
      zzOpenDWG()




      If zzSelectPoint(tPoint, True) = PromptStatus.OK Then
         If miCurrentStage = 0 Then
            zzInsertTatagTable(tPoint)
         Else
            zzInsertTazarTable(tPoint)
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
      Dim tTransferToRowParcelKey As UD_ParcelKey
      Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
      Dim dLegalAreaD As Double
      Dim tSum As UD_ParcelKey
      Dim tPlanExt As PlanExt
      '  Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus



      For Each oDataRow As DataRow In moMainTable.Rows
         tPlanExt = New PlanExt()
         iStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
         If iStage <> 0 Then
            iAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))
            iActionType = zzToActionType(oDataRow)
            iRowStatus = zzToRowStatus(oDataRow)
            dLegalAreaD = DMCommon.Functions.CDblN(oDataRow.Item("LegalArea"))

            If iRowStatus = enRowStatus.StartOfAction OrElse iRowStatus = enRowStatus.Transfer Then
               tSumParcelKey = zzGetSumParcelKey(iActionType, oDataRow)
               'If iActionType = UnidivNet.enActionType.Divide Then
               '   'tSumParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
               '   tSumParcelKey = zzParcelKeyFrom(oDataRow)
               'Else
               '   'tSumParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
               '   tSumParcelKey = zzParcelKeyTo(oDataRow)
               'End If
               If iAction = 1 Then
                  If oJournalTable IsNot Nothing Then
                     tPoint = oJournalTable.GetNewBasePoint()
                  End If
                  oJournalTable = New JournalTable(iActionType, iStage)
                  oJournalTable.SetBasePoint(tPoint)
                  oJournalTable.InsertHeader()
               End If
            End If

            If iActionType = UnidivNet.enActionType.Divide Then
               ' tRowParcelKey = New UD_ParcelKey(DMCommon.Functions.CIntN(oDataRow.Item("DestParcelNo")), False)
               tRowParcelKey = zzParcelKeyTo(oDataRow)
               tSumParcelKey.FinalNameInBrackets = True
               tPlanExt = New PlanExt(oDataRow)
            Else
               '  tRowParcelKey = New UD_ParcelKey(oDataRow.Item("OriginalParcelNo"), oDataRow.Item("NewParcelNo"))
               tRowParcelKey = zzParcelKeyFrom(oDataRow)
            End If
            If iActionType = UnidivNet.enActionType.Transfer Then
               tTransferToRowParcelKey = zzParcelKeyTo(oDataRow)
            End If

            If mdicParcels.TryGetValue(tRowParcelKey, oRowParcel) Then
               oJournalTable.InsertRow(tRowParcelKey, tPlanExt, oRowParcel.ParcelArea.LegalArea, iActionType = UnidivNet.enActionType.Transfer, tTransferToRowParcelKey)
               'iActionType
            Else
               DMCommon.Debug.MsgBox("09_771", tRowParcelKey, mdicParcels.Count)
            End If



            'End If
            If iRowStatus = enRowStatus.EndOfAction OrElse iRowStatus = enRowStatus.EndOfStage Then
               oJournalTable.InsertSumRow(tSumParcelKey)
            End If
         End If
      Next



   End Sub
   Private Sub zzInsertTatagTable(tPoint As Autodesk.AutoCAD.Geometry.Point3d)
      Dim oAreaTable As AreaTable = New AreaTable(enAreaTable.FDB)
      Dim oList As List(Of UD_ParcelKey) = New List(Of UD_ParcelKey)(mdicParcels.Keys)
      Dim oParcelComparer As UnidivNet.UD_Parcels.ParcelKeyComparer = New UnidivNet.UD_Parcels.ParcelKeyComparer()
      Dim oParcel As UnidivNet.UD_Parcel = Nothing
      oAreaTable.SetBasePoint(tPoint)
      Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
      oAreaTable.InsertHeader()
      oList.Sort(oParcelComparer)
      For Each tParcelKey As UD_ParcelKey In oList
         If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
            oAreaTable.InsertRow(oParcel.ParcelKey, oParcel.ParcelArea)
         End If

      Next
 







   End Sub
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
   Private Function zzGetLinksSet(colAllowable As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      ' Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
      Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iIndex As Integer = -1
      Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkNewLayer), New TypedValue(DxfCode.Color, miSelectedColorIndex)}
      Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colAllowable, miSelectedColorIndex)
      Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter = New SelectionFilter(oaValues) '
      DMAcadExt.AcadDocument.Regen()
      oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      oPromptOpt.AllowDuplicates = False




      ' oPromptOpt.SingleOnly = True
      '  oPromptOpt.SinglePickInSpace = True
      '   oPromptOpt.AllowSubSelections = True
      Me.Visible = False
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

      ptRes = oEditor.GetSelection(oPromptOpt, oFilter)



      ' oEditor.WriteMessage("D: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count))

      If ptRes.Value IsNot Nothing Then
         For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
            tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
            colSelected.Add(tAcObjID)
         Next
      End If

      '    DMCommon.Debug.MsgBox("09_763", colSelected.Count, ptRes.Status)
      '  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

      '  oEditor.SetImpliedSelection(oSelSet)
      '  oEditor.SetImpliedSelection(taAcObjIDs)

      ' oPromptOpt.ForceSubSelections = True


      Me.Visible = True


      oEntitySet.RestoreColor()
      Return colSelected
   End Function

   Private Function zzGetLinksSetOld(colAllowable As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      ' Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
      Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iIndex As Integer = -1
      Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId


      oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      oPromptOpt.SingleOnly = True
      oPromptOpt.SinglePickInSpace = True
      oPromptOpt.Keywords.Add("AA", "BB")


      ' oPromptOpt.SingleOnly = True
      '  oPromptOpt.SinglePickInSpace = True
      '   oPromptOpt.AllowSubSelections = True
      Me.Visible = False
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
      Do
         ptRes = oEditor.GetSelection(oPromptOpt)
         If ptRes.Value Is Nothing Then
            DMCommon.Debug.MsgBox("09_756a", -1, ptRes.Status)
         Else
            DMCommon.Debug.MsgBox("09_755", ptRes.Value.Count, ptRes.Status)
         End If


         If ptRes.Status <> PromptStatus.OK Then
            Exit Do
         Else
            iIndex += 1
         End If


         ' oEditor.WriteMessage("D: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count))


         For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
            tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
            If Not colSelected.Contains(tAcObjID) AndAlso colAllowable.Contains(tAcObjID) Then
               colSelected.Add(tAcObjID)

               DMAcadExt.AcadTransaction.Highlight(tAcObjID)


            End If
         Next
         '''''''''''''''''''''''''''''''''''''    DMCommon.Debug.MsgBox("09_763", colSelected.Count, ptRes.Status)
         '  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

         '  oEditor.SetImpliedSelection(oSelSet)
         '  oEditor.SetImpliedSelection(taAcObjIDs)

         ' oPromptOpt.ForceSubSelections = True''

      Loop
      Me.Visible = True

      If colSelected.Count > 0 AndAlso ptRes.Status <> PromptStatus.Cancel Then
         ReDim Preserve taAcObjIDs(colSelected.Count - 1)
         colSelected.CopyTo(taAcObjIDs, 0)

         DMCommon.Debug.MsgBox("09_764", taAcObjIDs.GetUpperBound(0))
         If taAcObjIDs.GetUpperBound(0) >= 0 Then
            ''''''''''''''''''''''''''oEditor.SetImpliedSelection(taAcObjIDs)
         End If
         For Each tAcObjID In colSelected
            DMAcadExt.AcadTransaction.Unhighlight(tAcObjID)
         Next
      End If

      Return colSelected
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

   Private Function zzToActionType(oDataRow As DataRow) As UnidivNet.enActionType
      Dim oValue As System.Object = oDataRow.Item("ActionType")
      Dim iActionType As Integer = DMCommon.Functions.CIntN(oValue)
      '  DMCommon.Debug.MsgBox("11_605", IsDBNull(oValue), iActionType, [Enum].IsDefined(GetType(UnidivNet.enActionType), iActionType), CType(iActionType, UnidivNet.enActionType))
      If [Enum].IsDefined(GetType(UnidivNet.enActionType), iActionType) Then
         Return CType(iActionType, UnidivNet.enActionType)
      Else
         Return UnidivNet.enActionType.Default
      End If
   End Function
   Private Function zzToRowStatus(oDataRow As DataRow) As enRowStatus
      Dim oValue As System.Object = oDataRow.Item("RowStatus")
      Dim iRowStatus As Integer = DMCommon.Functions.CIntN(oValue)
      If [Enum].IsDefined(GetType(enRowStatus), iRowStatus) Then
         Return CType(iRowStatus, enRowStatus)
      Else
         Return enRowStatus.Default
      End If
   End Function
   Private Sub zzCreateJournaltable()
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



   Private Sub cmdCancelAction_Click(oSender As System.Object, e As EventArgs) Handles cmdCancelAction.Click
      If miCurrentActionType <> UnidivNet.enActionType.Registered Then

         '  DMCommon.Debug.MsgBox("11_206a", miCurrentStage, miCurrentAction)
         Dim iIndex As Integer = moMainTable.Rows.Count - 1
         Dim oDataRow As DataRow
         Dim tPlineObjId As ObjectId

         Dim iPrevActionType As UnidivNet.enActionType = UnidivNet.enActionType.Default
         Dim iPrevStage As Integer
         Dim iPrevAction As Integer
         Dim oSourceParcel As UnidivNet.UD_Parcel
         Dim oDestParcel As UnidivNet.UD_Parcel

         Dim iRowStatus As enRowStatus

         ' Dim iPrevActionType As UnidivNet.enActionType
         '  Dim iPrevActionTypeInt As Integer

         '  DMCommon.Debug.MsgBox("11_206b", miCurrentStage, miCurrentActionType, miCurrentAction)
         mbEventsEnabled = False
         zzOpenDWG()
         zzDeleteCurrentTopology()
         ' zzClearLayers()

         Do
            oSourceParcel = Nothing
            oDestParcel = Nothing

            oDataRow = moMainTable.Rows.Item(iIndex)
            If DMCommon.Functions.CIntN(oDataRow.Item("Stage")) = miCurrentStage AndAlso DMCommon.Functions.CIntN(oDataRow.Item("Action")) = miCurrentAction Then
               iRowStatus = zzToRowStatus(oDataRow)
               Select Case miCurrentActionType
                  Case UnidivNet.enActionType.Union
                     oSourceParcel = zzParcelFrom(oDataRow)
                     If iRowStatus = enRowStatus.StartOfAction Then
                        oDestParcel = zzParcelTo(oDataRow)
                     End If
                  Case UnidivNet.enActionType.Divide
                     oDestParcel = zzParcelTo(oDataRow)
                     If iRowStatus = enRowStatus.StartOfAction Then
                        oSourceParcel = zzParcelFrom(oDataRow)
                     End If
                  Case UnidivNet.enActionType.Transfer
                     oSourceParcel = zzParcelFrom(oDataRow)
                     oDestParcel = zzParcelTo(oDataRow)

               End Select

               If oSourceParcel IsNot Nothing Then
                  oSourceParcel.Restore()
               End If
               If oDestParcel IsNot Nothing Then
                  oDestParcel.Delete()
                  mdicParcels.RemoveParcel(oDestParcel)

               End If

               If iRowStatus = enRowStatus.StartOfAction AndAlso miCurrentAction = 1 Then
                  zzClearCurrentStageLayers()
               End If
               '
               oDataRow.Delete()
            Else
               iPrevActionType = zzToActionType(oDataRow)
               iPrevStage = DMCommon.Functions.CIntN(oDataRow.Item("Stage"))
               iPrevAction = DMCommon.Functions.CIntN(oDataRow.Item("Action"))




               '   DMCommon.Debug.MsgBox("11_207", miCurrentStage, oDataRow.Item("BorderObjID"), miCurrentActionType, miCurrentAction)
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

         '  DMCommon.Debug.MsgBox("08_581a", miCurrentActionFirstRowIndex, miCurrentActionType)
         zzSetActionTypeFrame()


         If Me.dgvMain.AllowUserToAddRows Then
            Me.dgvMain.AllowUserToAddRows = False
         End If
         DMAcadExt.AcadDocument.ClearDrawVectorSet()
         If Me.chkActiveParcels.Checked Then
            zzRestoreView()
            zzActiveParcelsView(moUD_ParcelLayerList)
         End If
         zzUpdateStagesView()
         zzCloseDWG()
         mbEventsEnabled = True



         '   DMCommon.Debug.MsgBox("08_581b", miCurrentActionFirstRowIndex, miCurrentActionType)

      End If


   End Sub
   Private Sub zzCancelPoints()
      If 1 = miCurrentStage AndAlso 2 = miCurrentAction Then

      End If
      Dim colOriginalPoints As ObjectIdCollection = New ObjectIdCollection()
      For Each oPoint As UnidivNet.UD_Point In mdicPoints.Values
         If oPoint.Stage = miCurrentStage AndAlso oPoint.Action = miCurrentAction Then
            If Not oPoint.OriginalName Then
               mdicPoints.RemoveName(oPoint.Name)
               oPoint.Name = Nothing
               oPoint.UpdateBlock(msCreateNewNodesLayer)
            Else
               colOriginalPoints.Add(oPoint.AcObjID)
            End If
         End If
      Next
      mdicPoints.Recalc()
      DMAcadExt.AcadTransaction.SetLayer(colOriginalPoints, msCreateNewNodesLayer)
   End Sub
   Private Sub zzDeleteCurrentTopology()
      Dim sStageTopoName As String = zzGetCurrentStageTopoName()
      TopoManager.TopoCreator.DeleteTopology(sStageTopoName, False, False, False)

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
      Next
   End Sub
   Private Sub zzClearJournalTable()
      Dim sRowFilter As String = "(Stage <> 0)"
      Dim oActionDataView As DataView = New DataView(moMainTable, sRowFilter, String.Empty, DataViewRowState.CurrentRows)

      For Each oDataRowView As DataRowView In oActionDataView
         oDataRowView.Delete()
      Next
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

      hsHanitLayers.Add(msFinalTopologyName)
      '   DMCommon.Debug.MsgBox("12_130", miCurrentStage, hsHanitLayers.Count)
      DMAcadExt.AcadTransaction.ClearLayerSet(hsHanitLayers)

   End Sub
   Private Sub zzChangeNewPointStageLayers()
      Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
      Dim sNewPointLayer As String


      For iStage As Integer = 1 To miCurrentStage
         sNewPointLayer = UnidivNet.UD_App.GetStageUDPointLayer(iStage, True)
         hsHanitLayers.Add(sNewPointLayer)

      Next
      DMAcadExt.AcadTransaction.ChangeLayerSet(hsHanitLayers, msCreateNewNodesLayer)

   End Sub

   Private Sub zzGetParcelsString(ByRef sOrigin As String, ByRef sTemp As String)
      If mdicParcels IsNot Nothing Then
         mdicParcels.GetGushString(miBlockNo, miBlockAddNo, sOrigin, sTemp)
         mdicParcels.GetString(False, sOrigin, sTemp)

      Else
         sOrigin = String.Empty
         sTemp = String.Empty

      End If

   End Sub

   Private Sub frmUnidiv_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
      If midgvMainLocationY <> 0 Then
         Try
            Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmUnidiv - Resize")
         End Try
      End If
   End Sub


   Private Sub cmdInsertAreaTable_Click(sender As Object, e As EventArgs) Handles cmdInsertAreaTable.Click
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      Dim oAreaTable As AreaTable = New AreaTable(enAreaTable.FDB)
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d




      Dim oRowParcel As UnidivNet.UD_Parcel = Nothing
      Dim dLegalAreaD As Double
      Dim tSum As UD_ParcelKey
      Dim tPlanExt As PlanExt
      '  Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
      If zzSelectPoint(tPoint, True) = PromptStatus.OK Then
         oAreaTable.InsertHeader()
         For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values

         Next


      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub

   Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
      '   DMCommon.Debug.MsgBox("12_001", "DMAcadExt.AcadDocument.IsLocked", DMAcadExt.AcadDocument.IsLocked())
      '  DMCommon.Debug.MsgBox("12_001", "Stage=" & miCurrentStage, miCurrentActionFirstRowIndex)
      '   For Each oFragment As Fragment In mdicFragments.Values
      'DMAcadExt.AcadDocument.WriteDebugMessage("Fr" & oFragment.DWGTopoID & "; " & oFragment.DBTopoID & "; " & oFragment.ParcelKey.UD_ParcelName)
      'Next
      zzOpenDWG()
      For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values

         DMAcadExt.AcadDocument.WriteDebugMessage("PP " & oParcel.UD_Name & "; " & oParcel.DbID & "; " & oParcel.IsCanceled.ToString() & "; ")
         DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.BorderObjID)
         DMAcadExt.AcadTransaction.DBObjectInfo(oParcel.CentroidAcObjID)

      Next
      zzCloseDWG()
      ' DMCommon.Debug.MsgBox("12_009", DMAcadExt.AcadDocument.GetDrawVectorSetActive, DMAcadExt.AcadDocument.GetDrawVectorSetUB())
      ' DMAcadExt.AcadDocument.DrawVectorSetInfo()
   End Sub



   Private Sub cmdSelectRow_Click(sender As Object, e As EventArgs) Handles cmdSelectRow.Click
      '   DMCommon.Debug.MsgBox("12_019", miCurrentActionFirstRowIndex, miCurrentActionType)
      If miCurrentActionFirstRowIndex <> -1 Then
         '  Dim tParcelKey As UD_ParcelKey
         Select Case miCurrentActionType
            Case UnidivNet.enActionType.Divide
               zzParcelFromCurrentRow()
            Case UnidivNet.enActionType.Union
               Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = Nothing
               If zzGetParcelColFromGrid(colParcels) Then
                  '  DMCommon.Debug.MsgBox("12_021", colParcels.Count)
                  zzOpenDWG()
                  zzBeforeUnion(colParcels)
                  zzCloseDWG()
               End If
            Case UnidivNet.enActionType.Transfer
               zzParcelFromCurrentRow()
         End Select
      End If
   End Sub

   Private Sub zzParcelFromCurrentRow()
      Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
      Dim oDataRow As DataRow
      Dim tParcelKey As UD_ParcelKey
      If oCurrentGridRow.Index < miCurrentActionFirstRowIndex Then
         tParcelKey = zzGetSourceParcelKey(oCurrentGridRow)
         '  DMCommon.Debug.MsgBox("12_020", tParcelKey.ToString())
         Dim oParcel As UnidivNet.UD_Parcel = Nothing
         If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
            oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex)
            If tParcelKey.Original Then
               oDataRow.Item("OriginalParcelNo") = tParcelKey.ParcelNo
            Else
               oDataRow.Item("NewParcelNo") = tParcelKey.ParcelNo
            End If

            Me.dgvMain.DataSource = moMainTable

            ' DMAcadExt.AcadDocument.ClearDrawVectorSet()
            zzMarkParcel(oParcel, 0, True)
         End If

      End If

   End Sub
   Private Sub zzParcelFromCurrentCell()
      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
      Dim oCurrentGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(oCurrentCell.RowIndex)
      Dim oDataRow As DataRow
      Dim tParcelKey As UD_ParcelKey

      If oCurrentGridRow.Index < miCurrentActionFirstRowIndex Then
         tParcelKey = zzGetSourceParcelKey(oCurrentGridRow)
         '  DMCommon.Debug.MsgBox("12_020", tParcelKey.ToString())
         oDataRow = zzGetDataRow(miCurrentActionFirstRowIndex)
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



   Private Sub cmdGeneral_Click(sender As Object, e As EventArgs) Handles cmdGeneral.Click
      Dim sNormalParcelsString As String = Nothing
      Dim sTempParcelsString As String = Nothing

      Dim bIsTTTG As Boolean = UnidivNet.UD_App.IsTTG

      zzGetParcelsString(sNormalParcelsString, sTempParcelsString)
      Me.Visible = False
      '    MessageBox.Show(sNormalParcelsString & vbCrLf & sTempParcelsString, "01_498")
      If mfUD_General Is Nothing OrElse mfUD_General.IsDisposed Then
         mfUD_General = New TopoUI.frmUD_General(True)
      End If
      mfUD_General.GushName = miBlockNo.ToString()
      mfUD_General.NormalParcels = sNormalParcelsString
      mfUD_General.TempParcels = sTempParcelsString

      mfUD_General.SetPlanType(bIsTTTG)


      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
      mfUD_General.Left = 40
      mfUD_General.Top = 40
   End Sub

   Private Sub mfUD_General_FormClosed(sender As Object, e As FormClosedEventArgs) Handles mfUD_General.FormClosed
      Me.Visible = True
   End Sub


   Private Sub dgvMain_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dgvMain.CellValidating
      If mbEventsEnabled AndAlso mtEditingCellKey.Exists AndAlso miCurrentActionFirstRowIndex <> -1 AndAlso e.RowIndex >= miCurrentActionFirstRowIndex Then
         Select Case e.ColumnIndex
            Case 2, 3
               If e.FormattedValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(e.FormattedValue.ToString()) Then
                  Dim bRes As Boolean
                  Dim oParcel As UnidivNet.UD_Parcel = Nothing
                  Dim tParcelKey As UD_ParcelKey = zzParcelKeyFrom(e.ColumnIndex, e.FormattedValue)

                  If mdicParcels.TryGetValue(tParcelKey, oParcel) Then
                     '  DMCommon.Debug.MsgBox("12_211A", tParcelKey, miCurrentActionType, e.RowIndex - miCurrentActionFirstRowIndex)
                     '''''''''''''   DMAcadExt.AcadDocument.DrawVectorSetInfo()
                     zzMarkParcel(oParcel, e.RowIndex - miCurrentActionFirstRowIndex, True)
                  Else
                     e.Cancel = True
                  End If
                  'bRes = mdicParcels.ContainsKey(tParcelKey)
                  'If Not bRes Then
                  '  
                  'End If
                  '  DMCommon.Debug.MsgBox("12_110Z", bRes, tParcelKey.UD_ParcelName, e.RowIndex, e.ColumnIndex, e.FormattedValue.ToString(), e.FormattedValue.GetType().ToString())
               End If
         End Select
      End If
   End Sub

   Private Sub dgvMain_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
      If mbEventsEnabled Then


         mtEditingCellKey.Erase()
         Select Case e.ColumnIndex
            Case 22, 23
               Dim tParcelKey As UD_ParcelKey
               Dim oDataRow As DataRow = Nothing
               Dim oParcel As UnidivNet.UD_Parcel = Nothing
               If e.RowIndex < moMainTable.Rows.Count Then
                  oDataRow = zzGetDataRow(e.RowIndex)
                  tParcelKey = zzParcelKeyFrom(oDataRow)
               Else

               End If

               If tParcelKey.Exists AndAlso mdicParcels.TryGetValue(tParcelKey, oParcel) Then

                  If miCurrentActionType = UnidivNet.enActionType.Union Then
                     DMCommon.Debug.MsgBox("12_207", tParcelKey, oParcel.ParcelKey)

                     zzMarkParcel(oParcel, e.RowIndex - miCurrentActionFirstRowIndex, True)
                  End If


               End If

         End Select
      End If
   End Sub


   Private Sub moMainTable_TableNewRow(sender As Object, e As DataTableNewRowEventArgs) Handles moMainTable.TableNewRow

      If mbEventsEnabled Then
         ' DMCommon.Debug.MsgBox("12_120", e.Row.RowState)
         Dim oNewRow As DataRow = e.Row
         zzAddDataRow(enRowStatus.Default, e.Row)
      End If


   End Sub

   Private Sub cmdOpenAction_Click(sender As Object, e As EventArgs) Handles cmdOpenAction.Click
      DMCommon.Debug.MsgBox("12_121", mbEventsEnabled, moMainTable.Rows.Count)
   End Sub

   Private Sub cmdLoadSchema_Click(sender As Object, e As EventArgs)

   End Sub

   Private Sub cmdRestoreCancelLink_Click(sender As Object, e As EventArgs) Handles cmdRestoreCancelLink.Click
      zzOpenDWG()
      Dim colLinks As ObjectIdCollection = zzGetCancelLinks()
      If colLinks.Count > 0 Then
         DMAcadExt.AcadTransaction.SetLayer(colLinks, msLinkNewLayer)
      End If
      zzCloseDWG()
   End Sub

   Private Function zzGetCancelLinks() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Const iSelectedColorIndex As Integer = 10
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
      Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      ' Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
      Dim colSelected As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim iIndex As Integer = -1
      Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim oaValues() As TypedValue = {New TypedValue(DxfCode.LayerName, msLinkCancelledLayer)}
      ' Dim oEntitySet As DMAcadExt.dmEntitySet = New DMAcadExt.dmEntitySet(colAllowable, iSelectedColorIndex)
      Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter = New SelectionFilter(oaValues)
      DMAcadExt.AcadDocument.Regen()
      oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
      oPromptOpt.AllowDuplicates = False

      ' oPromptOpt.SingleOnly = True
      '  oPromptOpt.SinglePickInSpace = True
      '   oPromptOpt.AllowSubSelections = True
      Me.Visible = False
      AppActivate(moAppWin.Text)
      Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()

      ptRes = oEditor.GetSelection(oPromptOpt, oFilter)



      ' oEditor.WriteMessage("D: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count))

      If ptRes.Value IsNot Nothing Then
         For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
            tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
            colSelected.Add(tAcObjID)
         Next
      End If

      DMCommon.Debug.MsgBox("09_763", colSelected.Count, ptRes.Status)
      '  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

      '  oEditor.SetImpliedSelection(oSelSet)
      '  oEditor.SetImpliedSelection(taAcObjIDs)

      ' oPromptOpt.ForceSubSelections = True


      Me.Visible = True



      Return colSelected
   End Function

   Private Sub dgvMain_DoubleClick(sender As Object, e As EventArgs) Handles dgvMain.DoubleClick
      If mbEventsEnabled Then


         '  DMCommon.Debug.MsgBox("12_133", miCurrentActionFirstRowIndex, miCurrentActionType)
         If miCurrentActionFirstRowIndex >= 0 Then
            '  Dim tParcelKey As UD_ParcelKey
            Select Case miCurrentActionType
               Case UnidivNet.enActionType.Divide
                  '   zzParcelFromCurrentCell()
                  zzParcelFromCurrentRow()

               Case UnidivNet.enActionType.Union
                  Dim colParcels As ICollection(Of UnidivNet.UD_Parcel) = Nothing
                  If zzGetParcelColFromGrid(colParcels) Then
                     '  DMCommon.Debug.MsgBox("12_021", colParcels.Count)
                     zzOpenDWG()
                     zzBeforeUnion(colParcels)
                     zzCloseDWG()
                  End If
               Case UnidivNet.enActionType.Transfer
                  zzParcelFromCurrentRow()
            End Select
         End If

      End If
   End Sub
   Private Sub zzSetSourceParcel(oSourceParcel As UnidivNet.UD_Parcel, bArea As Boolean)
      If miCurrentActionFirstRowIndex >= 0 Then
         Dim oFirstActionGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(miCurrentActionFirstRowIndex)
         Dim oFirstActionDataRow As DataRow = moMainTable.Rows.Item(miCurrentActionFirstRowIndex)
         zzFillFromParcelKey(oSourceParcel, oFirstActionGridRow)
         If bArea Then
            zzFillParcelAreaNew(oSourceParcel.ParcelArea, oFirstActionDataRow, oFirstActionGridRow)
            oFirstActionDataRow.Item("AcObjID") = oSourceParcel.CentroidAcObjID
         End If
      End If

   End Sub
   Private Sub cmdSelectCentroids_Click(sender As Object, e As EventArgs) Handles cmdSelectCentroids.Click
      If miCurrentActionFirstRowIndex <> -1 Then
         zzOpenDWG()
         zzActiveParcelsView(moUD_ParcelLayerList)
         Dim oParcel As UnidivNet.UD_Parcel = Nothing
         Select Case miCurrentActionType
            Case UnidivNet.enActionType.Divide
               Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(True, "C1603_*", zzGetActiveParcelCentroids(), 10)
               ' Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()

               If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
                  '   DMCommon.Debug.MsgBox("12_122", colCentroidIds.Count)
                  If mdicParcels.TryGetValueByObjID(colCentroidIds.Item(0), oParcel) Then
                     zzSetSourceParcel(oParcel, False)
                  End If


                  ' zzParcelsToGrid(colParcels)
               End If


            Case UnidivNet.enActionType.Union
               Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(False, "C1603_*", zzGetActiveParcelCentroids(), 10)

               Dim colParcels As ObjectModel.Collection(Of UnidivNet.UD_Parcel) = New ObjectModel.Collection(Of UnidivNet.UD_Parcel)()
               If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
                  ' DMCommon.Debug.MsgBox("12_122", colCentroidIds.Count)
                  For Each tCentroidID As ObjectId In colCentroidIds
                     If mdicParcels.TryGetValueByObjID(tCentroidID, oParcel) Then
                        zzMarkParcel(oParcel, -1, False)
                        colParcels.Add(oParcel)
                     End If
                  Next

                  zzParcelsToGrid(colParcels)
               End If

            Case UnidivNet.enActionType.Transfer
               Dim colCentroidIds As ObjectIdCollection = zzGetEntitySet(True, "C1603_*", zzGetActiveParcelCentroids(), 10)
               If colCentroidIds IsNot Nothing AndAlso colCentroidIds.Count > 0 Then
                  '   DMCommon.Debug.MsgBox("12_122", colCentroidIds.Count)
                  If mdicParcels.TryGetValueByObjID(colCentroidIds.Item(0), oParcel) Then
                     zzSetSourceParcel(oParcel, False)
                     zzTransferFromFinalTopology(oParcel.ParcelKey)
                  End If


                  ' zzParcelsToGrid(colParcels)
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
      '   DMCommon.Debug.MsgBox("12_120", colLayers.Count)
      Return colLayers
   End Function
   Private Function zzGetActiveParcelCentroids() As ObjectIdCollection
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      For Each oParcel As UnidivNet.UD_Parcel In mdicParcels.Values
         If Not oParcel.IsCanceled Then
            colCentroids.Add(oParcel.CentroidAcObjID)
         End If
      Next
      '  DMCommon.Debug.MsgBox("12_121", colCentroids.Count)
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
         End If
      Next
   End Sub
   Private Sub zzResetVariables()
      miCurrentAction = 0
      miCurrentActionFirstRowIndex = -1
      miCurrentActionType = UnidivNet.enActionType.Registered
      miCurrentCentroidStatus = enCentroidStatus.Default
      miCurrentStage = 0

      Me.rdbInsertByPick.Checked = True
      Me.dgvMain.AllowUserToAddRows = False

   End Sub

   Private Sub cmdEraseAllStages_Click(oSender As System.Object, e As EventArgs) Handles cmdEraseAllStages.Click
      mbEventsEnabled = False
      zzOpenDWG()
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

   Private Sub cmdRecalc_Click(sender As Object, e As EventArgs) Handles cmdRecalc.Click
      zzRecalcDivideResParcels()
   End Sub


   Private Sub dgvMain_UserDeletingRow(sender As Object, e As DataGridViewRowCancelEventArgs) Handles dgvMain.UserDeletingRow
      If miCurrentActionFirstRowIndex = -1 OrElse e.Row.Index <= miCurrentActionFirstRowIndex Then
         e.Cancel = True
      ElseIf miCurrentActionType <> UnidivNet.enActionType.Union Then
         e.Cancel = True
      End If

   End Sub

   Private Sub rdbCentroidExists_CheckedChanged(sender As Object, e As EventArgs) Handles rdbInsertAuto.CheckedChanged

   End Sub

   Private Sub rdbHanit_CheckedChanged(sender As Object, e As EventArgs) Handles rdbHanit.CheckedChanged

   End Sub

   Private Sub chkActiveParcels_CheckedChanged(sender As Object, e As EventArgs) Handles chkActiveParcels.CheckedChanged

      zzOpenDWG()
      If chkActiveParcels.Checked Then
         zzActiveParcelsView(moUD_ParcelLayerList)
      Else
         zzRestoreView()
      End If
      zzCloseDWG()
   End Sub

   Private Sub chkPointsBlocking_CheckedChanged(sender As Object, e As EventArgs) Handles chkPointsBlocking.CheckedChanged
      TopoManager.TopoScheme.tsNode.IsGeoVertex = Me.chkPointsBlocking.Checked
      DMCommon.Debug.MsgBox("12_401", TopoManager.TopoScheme.tsNode.IsGeoVertex)
   End Sub

   Private Sub txtLastPoint_TextChanged(sender As Object, e As EventArgs) Handles txtLastPoint.TextChanged

   End Sub

   Private Sub cmdExit_Click(oSender As System.Object, e As System.EventArgs) Handles cmdExit.Click
      Me.Close()
   End Sub

   Private Sub chkDataBound_Validating(oSender As System.Object, e As System.ComponentModel.CancelEventArgs) Handles chkDataBound.Validating
      DMCommon.Debug.MsgBox("12_127", mbEventsEnabled, oSender)
      If mbEventsEnabled Then
         e.Cancel = True
      End If

   End Sub

   Private Sub chkHanitView_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkHanitView.CheckedChanged
      zzSetStageView()
      'If chkHanitView.Checked Then
      '   Dim iStage As Integer = Convert.ToInt32(Me.nudStagesView.Value)
      '   zzOpenDWG(False, False)
      '   zzSetStageViewA(iStage)
      '   zzCloseDWG()
      'End If

   End Sub

   Private Sub nudStagesView_ValueChanged(oSender As System.Object, e As EventArgs) Handles nudStagesView.ValueChanged
      zzSetStageView()
   End Sub
End Class