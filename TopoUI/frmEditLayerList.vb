Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Imports Autodesk.Gis.Map.Topology
Namespace Expro
	Public Class frmEditLayerList
		Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmExpro
		Private moEntConnectedLayersDataAdapter As Data.Common.DbDataAdapter
		Private WithEvents moEntConnectedLayersTable As Data.DataTable
		Private moEntConnectedLayersInDWG As Data.DataView

		Private moNewRow As DataRow
		Private moDWGLayersTable As Data.DataTable
		Private mbEventsEnabled As Boolean
		Private miID As Integer = 1000
		Public Sub New()

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			zzMyInitializeComponent()
		End Sub
		Private Sub zzMyInitializeComponent()
			zzFillGeometricTypes(Me.ccbDBGeometricType)
			zzFillGeometricTypes(Me.ccbDWGGeometricType)
			dgvDBLayers.AutoGenerateColumns = False
			dgvDWGLayers.AutoGenerateColumns = False
		End Sub
		Private Sub zzSetMaxID()
			Dim sCom As String = "SELECT MAX(ID) FROM dbo.EntConnectedLayers"
			Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sCom, CommandType.Text)
			If oRes IsNot Nothing Then
				miID = DirectCast(oRes, Integer)
			End If

		End Sub
		Private Sub zzLoadGlossary()
			Dim sComText As String = "SELECT * FROM EntConnectedLayers ORDER BY Description"

			moEntConnectedLayersTable = New Data.DataTable("EntConnectedLayers")
			moEntConnectedLayersDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, System.Data.CommandType.Text, True, "", True, True)
			moEntConnectedLayersDataAdapter.Fill(moEntConnectedLayersTable)
			moEntConnectedLayersTable.Columns.Add("Number", GetType(System.Int32))

			Me.dgvDBLayers.DataSource = moEntConnectedLayersTable
			Dim oPrimaryKey() As System.Data.DataColumn = {moEntConnectedLayersTable.Columns.Item("UpLayer")}
			moEntConnectedLayersTable.PrimaryKey = oPrimaryKey
		End Sub
		Private Sub zzFillGeometricTypes(ByRef oComboBoxColumn As DataGridViewComboBoxColumn)

			Dim iaGeometricType() As enGeometricType = DirectCast([Enum].GetValues(GetType(enGeometricType)), enGeometricType())
			Dim oItemData As DMCommon.ItemData

			oComboBoxColumn.ValueMember = DMCommon.ItemData.ValueMember
			oComboBoxColumn.DisplayMember = DMCommon.ItemData.DisplayMember
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "!GeometricType", iaGeometricType)
			For iIndex As Integer = 0 To iaGeometricType.GetUpperBound(0)
				oItemData = New DMCommon.ItemData(iaGeometricType(iIndex), zzGetGeometricTypeName(iaGeometricType(iIndex)))
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!GeometricTypeI", iIndex, iaGeometricType(iIndex), zzGetGeometricTypeName(iaGeometricType(iIndex)))
				oComboBoxColumn.Items.Add(oItemData)
			Next

		End Sub
		Private Sub zzLoadModelSpace()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
			Dim colAllDBObjects As DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
			Dim hsThemeUpLayers As HashSet(Of String) = frmPrjThemes.ThemeUpLayers
			Dim oEntity As Entity
			Dim sLayer As String
			Dim sUpLayer As String = Nothing

			Dim sRXClassName As String
			Dim oResDataRow As DataRow
			Dim oNewDataRow As DataRow
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "!ThemeUpLayer", hsThemeUpLayers)
			moDWGLayersTable = New Data.DataTable("DWGLayers")

			With moDWGLayersTable.Columns

				.Add("Layer", GetType(System.String))
				.Add("UpLayer", GetType(System.String))
				.Add("Description", GetType(System.String))
				.Add("GeometricType", GetType(System.Int32))
				.Add("IsProper", GetType(System.Boolean))
				.Add("Number", GetType(System.Int32))
			End With
			Dim oPrimaryKey() As System.Data.DataColumn = {moDWGLayersTable.Columns.Item("UpLayer")}
			moDWGLayersTable.PrimaryKey = oPrimaryKey


			If colAllDBObjects IsNot Nothing Then
				For Each oDBObject As DBObject In colAllDBObjects
					oEntity = TryCast(oDBObject, Entity)
					sLayer = oEntity.Layer
					sUpLayer = Strings.UCase(sLayer)
					sRXClassName = oDBObject.GetRXClass().Name
					Select Case sRXClassName
						Case DMAcadExt.AcadConst.AcadBlockRefName, DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName, DMAcadExt.AcadConst.AcadArcName
							If Not hsThemeUpLayers.Contains(sUpLayer) Then
								oResDataRow = moEntConnectedLayersTable.Rows.Find(sUpLayer)
								If oResDataRow Is Nothing Then
									oResDataRow = moDWGLayersTable.Rows.Find(sUpLayer)
									If oResDataRow Is Nothing Then
										oNewDataRow = moDWGLayersTable.NewRow
										oNewDataRow.Item("Layer") = sLayer
										oNewDataRow.Item("UpLayer") = sUpLayer

										oNewDataRow.Item("GeometricType") = 0
										oNewDataRow.Item("Number") = 1
										moDWGLayersTable.Rows.Add(oNewDataRow)
									Else
										oResDataRow.Item("Number") = DMCommon.Functions.CIntN(oResDataRow.Item("Number")) + 1
									End If

								Else
									oResDataRow.Item("Number") = DMCommon.Functions.CIntN(oResDataRow.Item("Number")) + 1
								End If
							End If
					End Select
				Next
			End If
			Me.dgvDWGLayers.DataSource = moDWGLayersTable
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub

		Private Sub zzSetFilter()
			moEntConnectedLayersInDWG = New DataView(moEntConnectedLayersTable, "Number > 0", "", DataViewRowState.CurrentRows)
			dgvDBLayers.DataSource = moEntConnectedLayersInDWG
		End Sub
		Private Function zzGetGeometricTypeName(iGeometricType As enGeometricType) As String
			Const iGeometricTypeSectionID As Integer = 4

			Return TPlServerDB.TextResource.GetText(iGeometricType, miResourceTheme, iGeometricTypeSectionID, True)

		End Function
		Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
			Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, True)
		End Function
		Private Function zzGetTextArray(ByVal iItemID As Integer, ByVal iCount As Integer, ByVal iSectionID As Integer) As String()
			Return TPlServerDB.TextResource.GetTextArray(iItemID, iCount, miResourceTheme, iSectionID, True)
		End Function

		Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
			Me.Close()
		End Sub

		Private Sub frmEditLayerList_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
			zzLoadGlossary()
			zzLoadModelSpace()
			zzSetMaxID()
		End Sub

		Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
			Try
				moEntConnectedLayersDataAdapter.Update(moEntConnectedLayersTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditLayerList - Ok")
			End Try

		End Sub

		Private Sub dgvDBLayers_CellEndEdit(sender As System.Object, e As DataGridViewCellEventArgs) Handles dgvDBLayers.CellEndEdit
			Dim oDataRow As DataRow
			If e.RowIndex >= 0 AndAlso e.ColumnIndex = 1 Then
				If e.RowIndex < moEntConnectedLayersTable.Rows.Count Then
					oDataRow = moEntConnectedLayersTable.Rows.Item(e.RowIndex)
				Else
					oDataRow = moNewRow
					miID += 1
					oDataRow.Item("ID") = miID
				End If
				'DMCommon.Debug.MsgBox("!CellEndEdit", DMCommon.Functions.CStrN(oDataRow.Item("Layer")))
				oDataRow.Item("UpLayer") = UCase(DMCommon.Functions.CStrN(oDataRow.Item("Layer")))

			End If

		End Sub

		Private Sub moEntConnectedLayersTable_TableNewRow(sender As System.Object, e As DataTableNewRowEventArgs) Handles moEntConnectedLayersTable.TableNewRow
			moNewRow = e.Row
			'	DMCommon.Debug.MsgBox("!moNewRow")
		End Sub

		Private Sub cmdAddLayers_Click(sender As System.Object, e As EventArgs) Handles cmdAddLayers.Click
			Dim oDBLayerRow As DataRow
			Dim oDataRow As DataRow
			For iIndex As Integer = moDWGLayersTable.Rows.Count - 1 To 0 Step -1
				oDataRow = moDWGLayersTable.Rows.Item(iIndex)
				If DMCommon.Functions.CBoolN(oDataRow.Item("IsProper")) Then
					oDBLayerRow = moEntConnectedLayersTable.NewRow()
					miID += 1
					oDBLayerRow.Item("ID") = miID
					oDBLayerRow.Item("Layer") = oDataRow.Item("Layer")
					oDBLayerRow.Item("UpLayer") = oDataRow.Item("UpLayer")

					oDBLayerRow.Item("GeometricType") = oDataRow.Item("GeometricType")
					oDBLayerRow.Item("Number") = oDataRow.Item("Number")
					oDBLayerRow.Item("Description") = oDataRow.Item("Description")


					moEntConnectedLayersTable.Rows.Add(oDBLayerRow)
					oDataRow.Delete()
				End If
			Next
		End Sub

		Private Sub cmdCancel_Click(sender As System.Object, e As EventArgs) Handles cmdCancel.Click
			Me.Close()
		End Sub

		Private Sub cmdRefresh_Click(sender As System.Object, e As EventArgs) Handles cmdRefresh.Click
			mbEventsEnabled = False
			zzLoadGlossary()
			mbEventsEnabled = True
			zzLoadModelSpace()
			zzSetMaxID()
		End Sub


		Private Sub chkFilter_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkFilter.CheckedChanged
			If mbEventsEnabled Then

				If chkFilter.Checked Then
					zzSetFilter()
				Else
					dgvDBLayers.DataSource = moEntConnectedLayersTable
				End If
			End If
		End Sub

		Private Sub frmEditLayerList_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
			mbEventsEnabled = True
		End Sub
	End Class
End Namespace