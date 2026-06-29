Option Explicit On
Option Strict On

Imports Autodesk.Gis.Map.Topology
Imports TopoManager
Imports TopoManager.TPlanGraph
Namespace TPlanGraph
	Public Class frmTplnViewTemp
		Private moTopoModel As TopologyModel
		Private miCurrentTopoDefID As TopoDefID
		Private miColumnSetIndex As Integer
		Private msaUnionTopoNames() As String = Nothing
		Private moOverlayODRecords() As OverlayODRecordSet
		Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmView
		Private moResource As TPlServerDB.TPlResource
		'  Private mbTopologyData As Boolean
		Private moCurrentDataView As DataView
		Private msBaseFilter As String
		Private msInPlanFilter As String
		Private msTaskFilter As String
		Private msBaseSort As String

		Private mdicRows As Generic.Dictionary(Of Integer, Integer)
		Private midgvMainLocationY As Integer

		Public Sub New()
			' This call is required by the Windows Form Designer.
			InitializeComponent()
			Try
				zzMyInitializeComponent()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - New")
			End Try
			Me.DialogResult = System.Windows.Forms.DialogResult.No
			' Add any initialization after the InitializeComponent() call.

		End Sub
		Public Sub Reset()
			Me.tcbData.SelectedItem = Nothing
			Me.tcbFilter.SelectedItem = Nothing
		End Sub
		Public Sub RefreshFormat()
			Me.zzSetDataGridColumns()
		End Sub
		Private Sub zzMyInitializeComponent()
			Dim iStartIndex, iEndIndex As Integer
			Select Case Common.AppID
				Case enApplications.Taba
					iStartIndex = 0
					iEndIndex = 10
				Case enApplications.TopoMaster
					iStartIndex = 21
					iEndIndex = 21
			End Select
			Try
				moResource = TPlServerDB.ServerDB.GetResource(miResourceTheme)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzMyInitializeComponent")
			End Try
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
			For iIndex As Integer = iStartIndex To iEndIndex
				Me.tcbData.Items.Add(New DMCommon.ItemData(iIndex, zzGetText(iIndex, 1)))
			Next
			midgvMainLocationY = Me.dgvMain.Location.Y
		End Sub
		Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
			Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID)
		End Function
		Private Sub zzSetFilterItems(ByVal sItemsList As String)
			Dim saItems() As String
			Dim iListIndex As Integer
			Me.tcbFilter.Items.Clear()
			If sItemsList.Length > 0 Then
				saItems = Strings.Split(sItemsList, ",")
				For iIndex As Integer = 0 To saItems.GetUpperBound(0)
					iListIndex = CInt(saItems(iIndex))
					Me.tcbFilter.Items.Add(New DMCommon.ItemData(iListIndex, zzGetText(iListIndex, 2)))
				Next
			End If
		End Sub
		Private Sub tlbTop_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
			Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
			Select Case oToolStripItem.Name
				Case Me.tbbSaveSelected.Name
					If Me.dgvMain.SelectedRows.Count = 0 Then
						zzUpdateCurrentPolygon()
					Else
						zzUpdateSelectedPolygons()
					End If
				Case Me.tbbSaveAll.Name

				Case Me.tbbZoomPgon.Name
					If Me.dgvMain.SelectedRows.Count = 0 Then
						zzZoomCurrentPolygon()
					Else
						zzZoomSelectedPolygons()
					End If
				Case Me.tbbBaseSort.Name
					If moCurrentDataView IsNot Nothing Then
						moCurrentDataView.Sort = msBaseSort
					End If
				Case Me.tbbPolygonNext.Name
					If moCurrentDataView IsNot Nothing Then
						Me.zzMovePolygon(1)
					End If
				Case Me.tbbPolygonPrevious.Name
					If moCurrentDataView IsNot Nothing Then
						Me.zzMovePolygon(-1)
					End If
				Case Me.tbbSetInitView.Name
					TopoManager.TPlanGraph.TplnProject.SetInitView()
				Case Me.tbbSetInitView.Name
					zzFindPgon(False, True)
				Case Me.tbbApplyFilter.Name
					zzSetFilter()
				Case Me.tbbClose.Name
					Me.Hide()
				Case Me.tbbFill.Name
					zzFillCurrentPolygon()
				Case Me.tbbInPlan.Name
					'   --> tbbInPlan.CheckedChanged
			End Select
		End Sub


		Private Sub zzFillPolygon(ByVal iTopoID As Integer)
			If iTopoID <> 0 Then
				Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
				Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme()
				oPolygon = zzGetTopoPolygon(iTopoID)
				tColorScheme.BackColor = New DMAcadExt.DMColor(Color.Azure)
				tColorScheme.Border.AddStrip(New DMAcadExt.DMColor(Convert.ToInt16(3)), 0.6)
				If oPolygon IsNot Nothing Then
					oPolygon.Paint(DMAcadExt.PaintMethod.FillPgon, tColorScheme)

				Else
					System.Windows.Forms.MessageBox.Show("Polygon was not found", "frmTplnView - zzZoomPolygon", MessageBoxButtons.OK, MessageBoxIcon.Warning)
				End If
			End If
		End Sub
		Private Sub zzZoomPolygon(ByVal iTopoID As Integer, ByVal bHighlight As Boolean)
			If iTopoID <> 0 Then
				Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
				Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
				oPolygon = zzGetTopoPolygon(iTopoID)
				If oPolygon IsNot Nothing Then
					If bHighlight Then
						DMAcadExt.AcadTransaction.Start()
						oPolygon.Highlight()
						DMAcadExt.AcadTransaction.Terminate()
					End If
					oBoundingBox = oPolygon.BoundingBox
					DMAcadExt.AcadDocument.Zoom(oBoundingBox)
				Else
					System.Windows.Forms.MessageBox.Show("Polygon was not found", "frmTplnView - zzZoomPolygon", MessageBoxButtons.OK, MessageBoxIcon.Warning)
				End If
			End If
		End Sub
		Private Sub zzUpdateCurrentPolygon()
			Dim iCurrentTopoID As Integer
			If miCurrentTopoDefID.ID <> 0 Then
				iCurrentTopoID = zzGetCurrentTopoID()

				'zzZoomPolygon(iCurrentTopoID, True)
			End If
		End Sub
		Private Sub zzZoomCurrentPolygon()
			Dim iCurrentTopoID As Integer
			If miCurrentTopoDefID.ID <> 0 Then
				iCurrentTopoID = zzGetCurrentTopoID()
				System.Windows.Forms.MessageBox.Show(CStr(iCurrentTopoID), "38_490")
				zzZoomPolygon(iCurrentTopoID, True)
			End If
		End Sub
		Private Sub zzFillCurrentPolygon()
			Dim iCurrentTopoID As Integer
			If miCurrentTopoDefID.ID <> 0 Then
				iCurrentTopoID = zzGetCurrentTopoID()
				zzFillPolygon(iCurrentTopoID)
			End If
		End Sub
		Private Sub zzZoomSelectedPolygons()
			Dim iTopoID As Integer
			Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox
			Dim oAddBoundingBox As DMAcadExt.TPlnBoundingBox = Nothing
			If miCurrentTopoDefID.ID <> 0 Then

				DMAcadExt.AcadTransaction.Start()
				For Each oViewRow As DataGridViewRow In Me.dgvMain.SelectedRows
					iTopoID = zzGetRowTopoID(oViewRow)
					Try
						If iTopoID <> 0 Then
							oPolygon = zzGetTopoPolygon(iTopoID)
							If oPolygon IsNot Nothing Then
								oPolygon.Highlight()
								oAddBoundingBox = oPolygon.BoundingBox
								If oAddBoundingBox IsNot Nothing AndAlso (Not oAddBoundingBox.Empty) Then
									oBoundingBox.Union(oAddBoundingBox)
								Else
									System.Windows.Forms.MessageBox.Show("Bounding Box was not found", "27_443")
								End If
							Else
								System.Windows.Forms.MessageBox.Show("Polygon was not found", "27_441")
							End If
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzZoomSelectedPolygons")
					End Try
				Next
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Zoom(oBoundingBox)
			End If
		End Sub
		Private Sub zzUpdateSelectedPolygons()
			Dim iTopoID As Integer
			Dim oBamashPolygon As BamashNet.BamashPolygon
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox
			Dim oDataRowView As DataRowView
			Dim oDataRow As DataRow

			If miCurrentTopoDefID.ID <> 0 Then
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				For Each oGridViewRow As DataGridViewRow In Me.dgvMain.SelectedRows
					oDataRowView = DirectCast(oGridViewRow.DataBoundItem, DataRowView)
					oDataRow = oDataRowView.Row
					iTopoID = zzGetRowTopoID(oGridViewRow)
					Try
						If iTopoID <> 0 Then
							oBamashPolygon = BamashNet.bmBamash.GetBamashPgon(iTopoID)

							If oBamashPolygon IsNot Nothing Then
								System.Windows.Forms.MessageBox.Show("", "27_439")
								oBamashPolygon.GetDataFromTable(oDataRow)
								oBamashPolygon.UpdateAllData()
							Else
								System.Windows.Forms.MessageBox.Show("Polygon was not found", "27_441")
							End If
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzUpdateSelectedPolygons")
					End Try
				Next
				DMAcadExt.AcadDocument.Unlock()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Zoom(oBoundingBox)
			End If
		End Sub
		Public Sub zzBoundingBoxZoomAAA(ByVal oBoundingBox As DMAcadExt.TPlnBoundingBox)
			Dim sTest As String = "a"
			Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
			Dim oPoint As DMAcadExt.TPlnPoint
			Try
				sTest = "b"
				oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
				sTest = "c"
				oPoint = oBoundingBox.GetCenterPoint()
				sTest = "d"
				If oPoint IsNot Nothing Then
					sTest = "e"
					oViewTableRecord.CenterPoint = oPoint.AcGePoint
					sTest = "f"
					oViewTableRecord.Width = oBoundingBox.Width
					sTest = "g"
					oViewTableRecord.Height = oBoundingBox.Height
					sTest = "h"
					Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.SetCurrentView(oViewTableRecord)
					sTest = "i"
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "frmTplnView - zzBoundingBoxZoom")
			End Try
		End Sub



		Private Function zzGetCurrentTopoID() As Integer
			Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
			Return zzGetRowTopoID(oViewRow)
		End Function
		Private Sub zzMovePolygon(ByVal iStep As Integer)
			If iStep <> 0 Then
				Try
					Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
					Dim iRowIndex As Integer = oViewRow.Index
					Dim iRowNewIndex As Integer = iRowIndex + iStep
					Dim bSuccess As Boolean
					bSuccess = zzSetCurrentRow(iRowNewIndex)
					If bSuccess Then
						Dim oNewViewRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowNewIndex)
						Dim iTopoID As Integer = zzGetRowTopoID(oNewViewRow)
						zzZoomPolygon(iTopoID, False)
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzMovePolygon")
				End Try

			End If
		End Sub
		Private Function zzGetRowTopoID(ByVal oViewRow As DataGridViewRow) As Integer
			Dim oValue As System.Object
			Try
				oValue = oViewRow.Cells.Item(TopoReader.msTopoIDFldName).Value
				If oValue IsNot Nothing Then
					Return DirectCast(oValue, Integer)
				Else
					Return 0
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzGetRowTopoID")
				Return 0
			End Try
		End Function
		Private Function zzGetTopoPolygon(ByVal iTopoID As Integer) As TopoManager.TPlanGraph.TplnTopoPgon
			Select Case Common.AppID
				Case enApplications.Taba
					Return TopoManager.TPlanGraph.TplnProject.GetTopoPolygon(iTopoID, miCurrentTopoDefID)
				Case enApplications.TopoMaster
					Return BamashNet.bmBamash.GetBamashPgon(iTopoID)
				Case Else
					Return Nothing
			End Select

		End Function
		Private Function zzGetCurrentTopoName() As String

			Dim oTopoDef As TopoDef = TopoDefs.Item(miCurrentTopoDefID)
			If oTopoDef IsNot Nothing Then
				Return oTopoDef.Name
			Else
				Return Nothing
			End If
		End Function
		Private Function zzGetPolygon(ByVal iTopoID As Integer) As Autodesk.Gis.Map.Topology.Polygon
			If moTopoModel Is Nothing Then
				moTopoModel = Common.GetTopology(zzGetCurrentTopoName())
			End If
			If moTopoModel IsNot Nothing Then
				If moTopoModel.Status = Status.Closed Then
					Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
					Try
						oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, False)
						moTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "frmTplnView - zzGetPolygon")
						If oDocLock IsNot Nothing Then
							oDocLock.Dispose()
						End If
						Return Nothing
						Exit Function
					End Try
				End If
				If moTopoModel.Status = Status.OpenForRead Then
					Return moTopoModel.GetPolygon(iTopoID)
				Else
					Return Nothing
				End If
			Else
				Return Nothing
			End If
		End Function
		Private Sub zzIndexing()
			If mdicRows Is Nothing Then
				mdicRows = New Generic.Dictionary(Of Integer, Integer)
				Dim oViewRow As DataGridViewRow
				Dim iTopoID As Integer
				For iIndex As Integer = 0 To Me.dgvMain.Rows.Count - 1
					oViewRow = Me.dgvMain.Rows.Item(iIndex)
					iTopoID = zzGetRowTopoID(oViewRow)
					mdicRows.Add(iTopoID, iIndex)
				Next
			End If
		End Sub
		Private Sub zzCloseCurrentTopology()
			If moTopoModel IsNot Nothing Then
				If moTopoModel.Status <> Status.Closed Then
					Try
						moTopoModel.Close()
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzCloseCurrentTopology")
					End Try
				End If
				moTopoModel = Nothing
			End If
		End Sub

		Private Sub zzFindPgon(ByVal bHideForm As Boolean, ByVal bRepeat As Boolean)
			If miCurrentTopoDefID.ID <> 0 Then
				Dim sTopoName As String = zzGetCurrentTopoName()
				Dim iTopoID As Integer
				Dim iRowIndex As Integer
				If bHideForm Then
					Me.Hide()
				End If
				Common.SetAcadFocus()
				Do
					iTopoID = TopoManager.TPlanGraph.TplnProject.FindPgonByPoint(sTopoName)
					DMAcadExt.AcadDocument.WriteMessage("TopoID=" & CStr(iTopoID) & vbCrLf)
					If iTopoID <> 0 Then
						Try
							zzIndexing()
							If mdicRows.ContainsKey(iTopoID) Then
								iRowIndex = mdicRows.Item(iTopoID)
								zzSetCurrentRow(iRowIndex)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzFindPgon")
						End Try
					End If
				Loop While bRepeat AndAlso iTopoID <> 0
				If bHideForm Then Me.Show()
				DMAcadExt.AcadDocument.CloseMessage()
				''   Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", False, False, False)
			End If
		End Sub
		Private Function zzSetCurrentRow(ByVal iRowIndex As Integer) As Boolean
			Dim bSuccess As Boolean
			If iRowIndex >= 0 AndAlso iRowIndex < Me.dgvMain.Rows.Count Then
				Try
					Dim oRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)
					Dim oCell As DataGridViewCell = oRow.Cells.Item(0)
					Me.dgvMain.CurrentCell = oCell
					bSuccess = True
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetCurrentRow")
					bSuccess = False
				End Try
			Else
				bSuccess = False
			End If
			Return bSuccess
		End Function
		Private Sub zzSetDataGridColumns()
			Dim oDataResource As TPlServerDB.TPlResource
			Dim oTextBoxColumn As DataGridViewTextBoxColumn
			Dim oAreaCellStyle As DataGridViewCellStyle = New DataGridViewCellStyle()
			Dim iColWidth As Integer
			oAreaCellStyle.Format = TopoManager.TPlanGraph.TplnProject.AreaFormat
			Dim oCoordinateCellStyle As DataGridViewCellStyle = New DataGridViewCellStyle()
			oCoordinateCellStyle.Format = TopoManager.TPlanGraph.TplnProject.CoordinateFormat
			Try
				oDataResource = moResource.GetChild(miColumnSetIndex)
				If oDataResource IsNot Nothing Then
					For Each oColumn As DataGridViewColumn In Me.dgvMain.Columns
						Try
							MessageBox.Show(CStr(oColumn.Index) & ":" & 10 + miColumnSetIndex, "12_646")
							oColumn.HeaderText = zzGetText(oColumn.Index, 10 + miColumnSetIndex)
							iColWidth = oDataResource.GetIntItem(oColumn.Index)(0)
							If iColWidth = 0 Then
								oColumn.Visible = False
							End If
							oColumn.Width = iColWidth
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetDatagridColumns_1")
						End Try
						Try
							If oColumn.Width = 0 Then
								oColumn.Visible = False
							Else
								Select Case oColumn.Name
									Case TopoReader.msAreaFldName, TopoReader.msSumPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msSumApprPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msSumPropPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msPgonAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaPropFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaApprFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaApprFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaPropFieldName, TopoManager.TPlanGraph.TplnParcel.msLegalAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaMergeFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaUnionFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaApprUnFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaPropUnFieldName, TopoManager.TPlanGraph.TplnParcel.msSumApprUnPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msSumPropUnPgonAreaFldName, TopoManager.TPlanGraph.TplnLot.msCalcAreaFieldName, TopoManager.TPlanGraph.TplnLot.msCalcAreaUnFieldName, TopoReader.msSumPgonAreaUnFldName
										oTextBoxColumn = DirectCast(oColumn, DataGridViewTextBoxColumn)
										oTextBoxColumn.DefaultCellStyle = oAreaCellStyle
									Case TopoReader.msCentroidXFldName, TopoReader.msCentroidYFldName
										oTextBoxColumn = DirectCast(oColumn, DataGridViewTextBoxColumn)
										oTextBoxColumn.DefaultCellStyle = oCoordinateCellStyle
								End Select
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetDatagridColumns_2")
						End Try
					Next
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetDatagridColumns_3")
			End Try
		End Sub
		Private Sub zzDispParcels()
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.MainView
			If moCurrentDataView IsNot Nothing Then
				miCurrentTopoDefID = New TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Parcel)
				msBaseFilter = moCurrentDataView.RowFilter
				msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
				miColumnSetIndex = 0

			End If
		End Sub
		Private Sub zzDispPlanParcels()
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.InPlanView
			miCurrentTopoDefID = New TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Parcel)
			miColumnSetIndex = 0
		End Sub
		Private Sub zzDispLots(ByVal iStatus As TopoManager.TPlanGraph.enTopoPurpose)
			moCurrentDataView = TopoManager.TPlanGraph.TplnLot.MainView(iStatus, False)
			If moCurrentDataView IsNot Nothing Then
				miCurrentTopoDefID = New TopoDefID(iStatus)
				msBaseFilter = moCurrentDataView.RowFilter
				msInPlanFilter = TopoManager.TPlanGraph.TplnLot.InPlanFilter
				miColumnSetIndex = 1
			End If
		End Sub
		Private Sub zzDispBlocks()
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.BlockView
			miCurrentTopoDefID.ID = 0
			miColumnSetIndex = 2
		End Sub
		Private Sub zzDispLotsContent()
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.LotContentView
			miCurrentTopoDefID.ID = 0
			miColumnSetIndex = 5
		End Sub
		Private Sub zzDispLanduse(ByVal iStatus As TopoManager.TPlanGraph.enTopoPurpose)
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.LanduseView(iStatus)
			miCurrentTopoDefID.ID = 0
			miColumnSetIndex = 3
		End Sub
		Private Sub zzDispBamashPgons()
			moCurrentDataView = BamashNet.BamashPolygon.MainView
			miCurrentTopoDefID = New TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Bamash)
			miColumnSetIndex = 21
		End Sub
		Private Sub zzDispPolygon(ByVal iOverlayIndex As TopoManager.TPlanGraph.enOverlayIndex)
			Dim iTopoPurpose As TopoManager.TPlanGraph.enTopoPurpose = TopoManager.TPlanGraph.UnionPgonArea.GetTopoPurpose(iOverlayIndex)
			Dim iOverlayMethod As TopoManager.TPlanGraph.enOverlayMethod = TopoManager.TPlanGraph.UnionPgonArea.GetOverlayMethod(iOverlayIndex)
			moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.PolygonView(iOverlayIndex)
			If moCurrentDataView IsNot Nothing Then
				miCurrentTopoDefID = New TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Parcel, iTopoPurpose, iOverlayMethod)
				msBaseFilter = moCurrentDataView.RowFilter
				msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.PolygonInPlanFilter
				miColumnSetIndex = 4
			End If
		End Sub
		Private Sub zzDifArea(ByVal iStatus As enTopoPurpose)
			Dim sTolerance As String = Me.zzGetTolerance()
			If sTolerance.Length <> 0 Then
				Dim sPgonAreaFldName As String
				Select Case iStatus
					Case TopoManager.TPlanGraph.enTopoPurpose.Approved
						sPgonAreaFldName = TopoManager.TPlanGraph.TplnParcel.msSumApprPgonAreaFldName
					Case TopoManager.TPlanGraph.enTopoPurpose.Proposed
						sPgonAreaFldName = TopoManager.TPlanGraph.TplnParcel.msSumPropPgonAreaFldName
					Case TopoManager.TPlanGraph.enTopoPurpose.Parcel
						sPgonAreaFldName = TopoReader.msSumPgonAreaFldName
					Case Else
						sPgonAreaFldName = String.Empty
				End Select
				msTaskFilter = "(Abs(" & TopoReader.msAreaFldName & "-" & sPgonAreaFldName & ")>" & sTolerance & ")"
				' sFilter = zzGetBaseFilter(True) & "(Abs(" & TopoReader.msAreaFldName & "-" & sPgonAreaFldName & ")>" & sTolerance & ")"
				If moCurrentDataView IsNot Nothing Then
					Try
						'   moCurrentDataView.RowFilter = sFilter
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzDifArea")
					End Try
				End If
			End If
		End Sub
		Private Function zzBuildFilter() As String
			Const sAnd As String = " AND "
			Dim sReturn As String
			Try
				If msBaseFilter.Length <> 0 Then
					sReturn = "(" & msBaseFilter & ")"
				Else
					sReturn = String.Empty
				End If

				If Me.tbbInPlan.Checked Then
					If sReturn.Length <> 0 Then
						sReturn &= sAnd
					End If
					sReturn &= "(" & msInPlanFilter & ")"
				End If
				If msTaskFilter.Length <> 0 Then
					If sReturn.Length <> 0 Then
						sReturn &= sAnd
					End If
					sReturn &= "(" & msTaskFilter & ")"
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzBuildFilter")
				sReturn = String.Empty
			End Try
			Return sReturn
		End Function
		Private Function zzGetBaseFilter(ByVal bAddAnd As Boolean) As String
			Dim sInPlanFilter As String
			Dim sAnd As String
			If bAddAnd Then
				sAnd = " AND "
			Else
				sAnd = String.Empty
			End If
			If Me.tbbInPlan.Checked Then
				sInPlanFilter = "(" & msInPlanFilter & ")" & sAnd
			Else
				sInPlanFilter = String.Empty
			End If
			If msBaseFilter.Length = 0 Then
				Return sInPlanFilter
			Else
				Return sInPlanFilter & "(" & msBaseFilter & ")" & sAnd
			End If
		End Function
		Private Sub zzClearRowStateFilter()
			If moCurrentDataView.RowStateFilter <> DataViewRowState.CurrentRows Then
				moCurrentDataView.RowStateFilter = DataViewRowState.CurrentRows
			End If
		End Sub
		Private Sub zzAll()
			msTaskFilter = String.Empty
		End Sub
		Private Sub zzSmallArea()

			Dim sTolerance As String = Me.zzGetTolerance()
			If sTolerance.Length <> 0 Then
				msTaskFilter = "(" & TopoReader.msAreaFldName & "<=" & sTolerance & ")"
				'  sFilter = zzGetBaseFilter(True) & "(" & TopoReader.msAreaFldName & "<=" & sTolerance & ")"
				'  zzz()
				Try
					'  moCurrentDataView.RowFilter = sFilter
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSmallArea")
				End Try
			End If

		End Sub
		Private Sub zzDoubleNames()
			Dim iStatus As enTopoPurpose = miCurrentTopoDefID.BaseID
			Dim sFilter As String
			If iStatus <> enTopoPurpose.Undefined Then
				sFilter = TplnProject.GetDoubleNamesCriteria(iStatus)
				If sFilter.Length = 0 Then
					moCurrentDataView.RowStateFilter = DataViewRowState.None
				Else
					msTaskFilter = sFilter
				End If
			End If
		End Sub
		Private Sub zzClearDicRows()
			If mdicRows IsNot Nothing Then
				mdicRows = Nothing
			End If

		End Sub
		Private Function zzGetTolerance() As String
			Dim dTolerance As Double
			If IsNumeric(Me.txtTolerance.Text) Then
				dTolerance = CDbl(Me.txtTolerance.Text)
				Return CStr(dTolerance)
			Else
				Return String.Empty
			End If

		End Function

		Private Sub zzSetFilter()
			If moCurrentDataView IsNot Nothing Then
				Dim oItemData As DMCommon.ItemData
				Try
					oItemData = DirectCast(Me.tcbFilter.SelectedItem, DMCommon.ItemData)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetFilter_1")
					Exit Sub
				End Try
				If oItemData IsNot Nothing Then
					Try
						zzClearRowStateFilter()
						Select Case oItemData.ListIndex
							Case 0
								zzAll()
							Case 1
								zzSmallArea()
							Case 2
								zzDifArea(enTopoPurpose.Parcel)
							Case 3
								zzDifArea(enTopoPurpose.Approved)
							Case 4
								zzDifArea(enTopoPurpose.Proposed)
							Case 5
								Me.zzDoubleNames()
						End Select
						moCurrentDataView.RowFilter = zzBuildFilter()
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetFilter_2")
					End Try
				End If
				zzClearDicRows()

			End If
		End Sub

		Private Sub tcbData_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tcbData.SelectedIndexChanged
			Dim oItemData As DMCommon.ItemData
			Dim oSelectedItem As System.Object
			Dim sTest As String = "0"
			msBaseFilter = String.Empty
			msInPlanFilter = String.Empty
			msTaskFilter = String.Empty
			Try
				zzCloseCurrentTopology()
				sTest = "1"
				oSelectedItem = tcbData.SelectedItem
				sTest = "2"
				If oSelectedItem Is Nothing Then
					sTest = "3"
					moCurrentDataView = Nothing
					zzSetFilterItems(String.Empty)
				Else
					sTest = "4"
					oItemData = DirectCast(oSelectedItem, DMCommon.ItemData)
					sTest = "5"
					Select Case Common.AppID
						Case enApplications.Taba
							zzExecTaba(oItemData.ListIndex)
						Case enApplications.TopoMaster
							zzExecTopoMaster(oItemData.ListIndex)
					End Select

				End If
				sTest = "a"
				If moCurrentDataView Is Nothing Then
					msBaseSort = String.Empty
				Else
					msBaseSort = moCurrentDataView.Sort
				End If

				Me.tbbInPlan.Enabled = msInPlanFilter.Length <> 0
				zzClearDicRows()
				sTest = "b"
				'  Me.dgvMain.DataSource = String.Empty
				sTest = "ba"
				Me.dgvMain.Columns.Clear()
				sTest = "bc"
				Me.dgvMain.DataSource = moCurrentDataView

				sTest = "c"
				zzSetDataGridColumns()
				sTest = "d"

				If Me.tcbFilter.Items.Count = 0 Then
					Me.tcbFilter.Text = String.Empty
				Else
					Me.tcbFilter.SelectedIndex = 0
				End If
				zzSetEnable()

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "frmTplnView - tcbData_SelectedIndexChanged")
			End Try
		End Sub
		Private Sub zzExecTaba(ByVal iIndex As Integer)
			Select Case iIndex
				Case 0
					zzDispParcels()
					zzSetFilterItems("0,1,3,4,5")
				Case 1
					zzDispLots(enTopoPurpose.Approved)
					zzSetFilterItems("0,1,2,5")
				Case 2
					zzDispLots(enTopoPurpose.Proposed)
					zzSetFilterItems("0,1,2,5")
				Case 3
					zzDispBlocks()
					zzSetFilterItems(String.Empty)
				Case 4
					zzDispLotsContent()
					zzSetFilterItems(String.Empty)

				Case 5
					zzDispLanduse(enTopoPurpose.Approved)
					zzSetFilterItems(String.Empty)
				Case 6
					zzDispLanduse(TopoManager.TPlanGraph.enTopoPurpose.Proposed)
					zzSetFilterItems(String.Empty)
				Case 7 To 20

					Dim iOverlayIndex As TopoManager.TPlanGraph.enOverlayIndex = CType(iIndex - 7, enOverlayIndex)

					zzDispPolygon(iOverlayIndex)
					zzSetFilterItems("0,1")
				Case 21
					zzDispBamashPgons()
			End Select

		End Sub
		Private Sub zzExecTopoMaster(ByVal iIndex As Integer)
			Select Case iIndex
				Case 21
					zzDispBamashPgons()
			End Select
		End Sub
		Private Sub zzSetEnable()
			Try
				Dim bValue As Boolean
				If miCurrentTopoDefID.ID = 0 Then
					bValue = False
				Else
					bValue = True
				End If
				Me.cmdFindPgon.Enabled = bValue
				Me.cmdFindPgonLoop.Enabled = bValue
				Me.tbbZoomPgon.Enabled = bValue
				Me.tbbPolygonNext.Enabled = bValue
				Me.tbbPolygonPrevious.Enabled = bValue
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetEnable")
			End Try
		End Sub
		Private Sub tcbFilter_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tcbFilter.SelectedIndexChanged
			Me.zzSetFilter()

		End Sub

		Private Sub frmTplnView_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
			Dim iCloseReason As CloseReason = e.CloseReason

			Select Case iCloseReason
				Case CloseReason.UserClosing
					e.Cancel = True
					Me.Hide()
			End Select
		End Sub

		Private Sub tbbPolygonAppr_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
			Me.Focus()
			Me.cmdFindPgon.Focus()
			Common.MouseClick()
			'  Me.zzDispBlocks()
			'  Me.zzTestInsert()

		End Sub

		Private Sub cmdFindPgon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFindPgon.Click
			Me.zzFindPgon(True, False)
		End Sub

		Private Sub frmTplnView_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
			If midgvMainLocationY <> 0 Then
				Try
					Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - frmTplnView_Resize")
				End Try
			End If
		End Sub


		Private Sub tbbInPlan_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tbbInPlan.CheckedChanged
			Me.zzSetFilter()
		End Sub

		Private Sub cmdFindPgonLoop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFindPgonLoop.Click
			DMAcadExt.AcadDocument.WriteMessage("Before zzFindPgon" & vbCrLf)
			Me.zzFindPgon(False, True)
			DMAcadExt.AcadDocument.WriteMessage("After zzFindPgon" & vbCrLf)
		End Sub
	End Class
End Namespace