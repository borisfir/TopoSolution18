Option Explicit On
Option Strict On
Imports TopoManager
Imports TopoManager.TPlanGraph
Imports System.Data
Public Class frmCalcAreas
	Private miCurrentMapTheme As DMAcadExt.enMapTheme
	Private mbEventsEnabled As Boolean = False
	Private miCurrentTopoDefID As DMAcadExt.TopoDefID
	Private moMarkedCellStyle As System.Windows.Forms.DataGridViewCellStyle
	Private moMarkedCellStyleCentre As System.Windows.Forms.DataGridViewCellStyle
	Private moDataView As DataView = Nothing



	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		miCurrentMapTheme = DMAcadExt.enMapTheme.Parcels
		moMarkedCellStyle = Me.dgvMain.DefaultCellStyle.Clone
		moMarkedCellStyle.BackColor = Color.Red
		moMarkedCellStyleCentre = moMarkedCellStyle.Clone
		moMarkedCellStyleCentre.Alignment = DataGridViewContentAlignment.MiddleCenter
		Me.dgvMain.AutoGenerateColumns = False

	End Sub



	Private Sub zzCalc()
		Const msPgonsNum As String = "מספר פוליגונים: "


		Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.AcadArea
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
		Dim oDataTable1 As DataTable = Nothing




		Dim iaDataColumns() As Integer = Nothing
		Dim oaTotals() As System.Object = Nothing
		Dim iCurrentRegionNo As Integer = 0
		Dim hsCurrentRegions As HashSet(Of Integer) = Nothing





		Dim bEntirety As Boolean
		iaDataColumns = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}

		'TPlanGraph.TplnProject.GetInPlanDataNew(DMAcadExt.enTopoPurpose.Approved, bEntirety, True, iDataOption, iOverlayMethod, iCurrentRegionNo, hsCurrentRegions, oDataView, iaDataColumns, oaTotals)
		'TPlanGraph.TplnProject.GetInPlanDataII(DMAcadExt.enTopoPurpose.Approved, True, True, iDataOption, iOverlayMethod, iCurrentRegionNo, Nothing, True, oDataView, iaDataColumns, oaTotals)
		Select Case miCurrentMapTheme
			Case DMAcadExt.enMapTheme.Parcels
				moDataView = TopoManager.TPlanGraph.TplnParcel.MainView
				'oDataTable = TopoManager.TPlanGraph.TplnParcel.MainDataTable
				miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			Case DMAcadExt.enMapTheme.LotApproved
				moDataView = TopoManager.TPlanGraph.TplnLot.MainView(DMAcadExt.enTopoPurpose.Approved, True)
				miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Approved)
				'oDataTable = TopoManager.TPlanGraph.TplnLot.MainDataTable(DMAcadExt.enTopoPurpose.Approved)
			Case DMAcadExt.enMapTheme.Mitham
				moDataView = TopoManager.TPlanGraph.TplnRegion.MainDataTable.DefaultView
				miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Mitham)
				'oDataTable = moDataView
		End Select


		'DMCommon.Debug.MsgBox("171124", moDataView.Count, miCurrentMapTheme, miCurrentTopoDefID.ID)
		Me.dgvMain.DataSource = moDataView
		Me.lblRecCount.Text = msPgonsNum & CStr(moDataView.Count)
		zzFormat()
	End Sub
	Private Sub zzFormat()
		Const msNotProperPgonsNum As String = "פוליגונים לא תקינים: "
		Dim oDataRowView As DataRowView
		Dim oGridRow As DataGridViewRow
		Dim oGridCell As DataGridViewCell
		Dim iNotProperRecCount As Integer = 0
		For iIndex As Integer = 0 To moDataView.Count - 1
			oDataRowView = moDataView.Item(iIndex)
			oGridRow = Me.dgvMain.Rows.Item(iIndex)
			oGridCell = oGridRow.Cells.Item("cchProper")

			If DMCommon.Functions.CDblN(oDataRowView.Item("Deviation")) > 0.0 Then
				oGridCell.Value = False
				iNotProperRecCount += 1
				oGridRow.DefaultCellStyle = moMarkedCellStyle
				oGridCell.Style = moMarkedCellStyleCentre
			Else
				oGridCell.Value = True
			End If
		Next
		Me.lblNotProperRecCount.Text = msNotProperPgonsNum & CStr(iNotProperRecCount)
	End Sub

	Private Sub zzColumnVisible()
		Dim oGridColumn As DataGridViewColumn

		Try
			oGridColumn = Me.dgvMain.Columns.Item("ctxBlockFull")
			If oGridColumn IsNot Nothing Then
				oGridColumn.Visible = Me.rdbParcels.Checked
			Else
				System.Windows.Forms.MessageBox.Show("oGridColumn Is Nothing", "frmCalcAreas - ctxBlockFull1")
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxBlockFull")
		End Try

		Try
			Me.dgvMain.Columns.Item("ctxParcelNo").Visible = Me.rdbParcels.Checked
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxParcelNo")
		End Try

		Try
			Me.dgvMain.Columns.Item("ctxRegionName").Visible = Me.rdbLots.Checked Or Me.rdbRegions.Checked
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxRegionName")
		End Try

		Try
			Me.dgvMain.Columns.Item("ctxLot").Visible = Me.rdbLots.Checked
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxLot")
		End Try

		Try
			Me.dgvMain.Columns.Item("ctxLegalArea").Visible = Me.rdbParcels.Checked
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxLegalArea")
		End Try

		Try
			Me.dgvMain.Columns.Item("ctxCalcAreaFO").Visible = Me.rdbLots.Checked OrElse Me.rdbRegions.Checked
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmCalcAreas - ctxCalcAreaFO")
		End Try
		Try
			Me.dgvMain.Focus()
		Catch oEx As Exception


		End Try

	End Sub

	Private Sub zzZoomPolygon(ByVal iTopoID As Integer, ByVal bHighlight As Boolean)
		If iTopoID <> 0 Then
			Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox

			oPolygon = zzGetTopoPolygon(iTopoID)
			If oPolygon IsNot Nothing Then
				If bHighlight Then
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
					DMAcadExt.AcadTransaction.Start()
					oPolygon.Highlight()
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
				End If
				oBoundingBox = oPolygon.BoundingBox
				DMAcadExt.AcadDocument.Zoom(oBoundingBox)
			Else
				System.Windows.Forms.MessageBox.Show("!+Polygon was not found" & vbCrLf & CStr(iTopoID) & ":" & miCurrentTopoDefID.ID.ToString(), "frmCalcArea - zzZoomPolygon", MessageBoxButtons.OK, MessageBoxIcon.Warning)
			End If
		End If
	End Sub

	Private Function zzGetTopoPolygon(ByVal iTopoID As Integer) As TopoManager.TPlanGraph.TplnTopoPgon
		Return TopoManager.TPlanGraph.TplnProject.GetTopoPolygon(iTopoID, miCurrentTopoDefID)
	End Function

	Private Sub zzCalc1()
		Dim iDataOption As TPlanGraph.enDataOptions = TPlanGraph.enDataOptions.AcadArea
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
		Dim oDataView As System.Data.DataView = Nothing
		Dim iaDataColumns() As Integer = Nothing
		Dim oaTotals() As System.Object = Nothing
		Dim iCurrentRegionNo As Integer = 0
		Dim hsCurrentRegions As HashSet(Of Integer) = Nothing
		Dim bEntirety As Boolean
		iaDataColumns = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12}
		'TPlanGraph.TplnProject.GetInPlanDataNew(DMAcadExt.enTopoPurpose.Approved, bEntirety, True, iDataOption, iOverlayMethod, iCurrentRegionNo, hsCurrentRegions, oDataView, iaDataColumns, oaTotals)
		'	TPlanGraph.TplnProject.GetInPlanDataII(DMAcadExt.enTopoPurpose.Approved, True, True, iDataOption, iOverlayMethod, iCurrentRegionNo, Nothing, True, oDataView, iaDataColumns, oaTotals)
		'	DMCommon.Debug.MsgBox("171124", oDataView.Count)
		Me.dgvMain.DataSource = oDataView
	End Sub

	Private Sub Button1_Click(sender As System.Object, e As EventArgs) Handles Button1.Click
		Me.Cursor = Cursors.WaitCursor
		zzCalc()
		zzColumnVisible()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub rdbParcels_CheckedChanged(sender As Object, e As EventArgs) Handles rdbParcels.CheckedChanged
		If mbEventsEnabled AndAlso Me.rdbParcels.Checked Then
			miCurrentMapTheme = DMAcadExt.enMapTheme.Parcels
			zzCalc()
			zzColumnVisible()
		End If
	End Sub

	Private Sub rdbLots_CheckedChanged(sender As Object, e As EventArgs) Handles rdbLots.CheckedChanged
		If mbEventsEnabled AndAlso Me.rdbLots.Checked Then
			miCurrentMapTheme = DMAcadExt.enMapTheme.LotApproved
			zzCalc()
			zzColumnVisible()
		End If
	End Sub

	Private Sub rdbRegions_CheckedChanged(sender As Object, e As EventArgs) Handles rdbRegions.CheckedChanged
		If mbEventsEnabled AndAlso Me.rdbRegions.Checked Then
			miCurrentMapTheme = DMAcadExt.enMapTheme.Mitham
			zzCalc()
			zzColumnVisible()
		End If
	End Sub

	Private Sub frmCalcAreas_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		mbEventsEnabled = True
		Me.rdbParcels.Checked = True
		Me.dgvMain.Focus()
	End Sub
	Private Function zzGetCurrentTopoID() As Integer
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		If oViewRow IsNot Nothing Then
			Return zzGetRowTopoID(oViewRow)
		Else
			Return 0
		End If

	End Function
	Private Function zzGetRowTopoID(ByVal oViewRow As DataGridViewRow, sTopoIDFieldName As String) As Integer
		Dim oValue As System.Object
		Dim oDataRowView As DataRowView = DirectCast(oViewRow.DataBoundItem, DataRowView)
		Try
			oValue = oDataRowView.Item(sTopoIDFieldName)
			If oValue IsNot Nothing AndAlso Not IsDBNull(oValue) Then
				Return DirectCast(oValue, Integer)
			Else
				System.Windows.Forms.MessageBox.Show("Nothing" & vbCrLf & sTopoIDFieldName, "01_838")
				Return 0
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzGetRowTopoID")
			Return 0
		End Try
	End Function
	Private Function zzGetRowTopoID(ByVal oViewRow As DataGridViewRow) As Integer
		Return zzGetRowTopoID(oViewRow, TopoReader.msTopoIDFldName)

	End Function

	Private Sub cmdZoom_Click(sender As Object, e As EventArgs) Handles cmdZoom.Click
		Dim iTopoID As Integer
		iTopoID = zzGetCurrentTopoID()
		If iTopoID > 0 Then
			zzZoomPolygon(iTopoID, True)
		End If


	End Sub

	Private Sub cmdExit_Click(sender As Object, e As EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Sub cmdExcelReport_Click(sender As Object, e As EventArgs) Handles cmdExcelReport.Click

	End Sub

	Private Sub lblNotProperRecCount_Click(sender As Object, e As EventArgs) Handles lblNotProperRecCount.Click

	End Sub

	Private Sub dgvMain_Sorted(sender As Object, e As EventArgs) Handles dgvMain.Sorted
		'zzCalc()
		zzFormat()
	End Sub
End Class