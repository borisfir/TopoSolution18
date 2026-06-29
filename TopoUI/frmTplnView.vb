Option Explicit On
Option Strict On

Imports Autodesk.Gis.Map.Topology
Imports TopoManager
Imports TopoManager.TPlanGraph
Imports System.Data
Public Enum enPaintMethod
	Landuse = 1
	BlockParcel = 2
	ParcelExcept = 3
End Enum


Public Class frmTplnView
	Private Enum enZoomType
		Unable
		Polygon
		Boundary
   End Enum
   Private Enum enTabaThemeIndex

      Zones = 25
   End Enum
   Private Structure PaintParam
      Dim LanduseByLayer As Boolean
      Dim LayerDef As DMAcadExt.AcadLayerDef
      Dim ByPassedLine As Boolean
      Dim Scale As Double


   End Structure
	Private moTopoModel As TopologyModel
	Private mbTopologyExists As Boolean
   Private msTopologyName As String
   Private moTopoScheme As TopoManager.TopoScheme.tsTopology
   Private miCurrentTopoDefID As DMAcadExt.TopoDefID


   Private miMapTheme As DMAcadExt.enMapTheme

   Private miColumnSetIndex As Integer
	Private mbPolygons As Boolean
	Private msaUnionTopoNames() As String = Nothing
	Private moOverlayODRecords() As OverlayODRecordSet
	Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmView
	Private moResource As TPlServerDB.TPlResource
	'  Private mbTopologyData As Boolean
	Private moCurrentDataView As DataView
	Private msCurrentCaption As String
	Private msBaseFilter As String
	Private msInPlanFilter As String
	Private moHiddenColumns As Dictionary(Of Integer, Integer)
	Private msTaskFilter As String
	Private msSelectFilter As String

	Private msBaseSort As String

	Private msAreaFieldName As String
   Private miZoomType As enZoomType = enZoomType.Unable
	Private mdicRows As Generic.Dictionary(Of Integer, Integer)
	Private midgvMainLocationY As Integer
	Private micmdFindPgonLoopLocationX As Integer
	Private moScaleMenuItemChecked As System.Windows.Forms.ToolStripMenuItem
	Private miTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
   Private miPaintMethod As enPaintMethod

   Private miCurrentLanduseID As Integer
   Private mtCurrentColorScheme As DMAcadExt.ColorScheme

	Private mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
	Private mtInitMapThemeData As DMAcadExt.MapThemeData
   Private moMarkedCellStyle As System.Windows.Forms.DataGridViewCellStyle
   Private mbMarked As Boolean
	Public Sub New(tInitMapThemeData As DMAcadExt.MapThemeData)
		mtInitMapThemeData = tInitMapThemeData
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		mdicMapThemes = frmPrjThemes.MapThemes



		Try
			zzMyInitializeComponent()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - New")
		End Try
		Me.DialogResult = System.Windows.Forms.DialogResult.No

		' Add any initialization after the InitializeComponent() call.
		TopoManager.TPlanGraph.TplnProject.InitializeView()
	End Sub
	Public Property MapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
      Get
         Return mdicMapThemes
      End Get
      Set(dicValue As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData))
         mdicMapThemes = dicValue
      End Set
   End Property
   Public Property InitMapThemeData As DMAcadExt.MapThemeData
      Get
         Return mtInitMapThemeData
      End Get
      Set(tValue As DMAcadExt.MapThemeData)
         mtInitMapThemeData = tValue
         '  System.Windows.Forms.MessageBox.Show(mtInitMapThemeData.MapThemeID.ToString(), "01_911")
         '  DMCommon.Debug.MsgBox("12_360", mtInitMapThemeData.MapThemeID)
         zzSetInitTable()
         '  DMCommon.Debug.MsgBox("12_361", mtInitMapThemeData.MapThemeName)
      End Set
   End Property

   Public Sub Reset()
      zzSetData()
      Me.dgvMain.DataSource = moCurrentDataView
      'Me.tcbData.SelectedItem = Nothing
      'Me.tcbFilter.SelectedItem = Nothing
   End Sub
   Public Sub RefreshFormat()
      Me.zzSetDataGridColumns()
   End Sub
   Public Sub RefreshPaintScale()
      zzSetPaintScale()
   End Sub
   Public Sub SetPgonFilter(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer)
      Dim sFilterFieldName As String

      Me.tcbData.SelectedIndex = 9 + CInt(iOverlayIndex)
      If bParcel Then
         sFilterFieldName = TopoReader.msParcelTopoIDFldName
      Else
         sFilterFieldName = TopoReader.msLotTopoIDFldName
      End If

      If sFilterFieldName IsNot Nothing Then

         msSelectFilter = "(" & sFilterFieldName & "=" & CStr(iTopoID) & ")"
         zzApplyFilter()
      End If
   End Sub
   Private Sub zzMyInitializeComponent()
      Dim iStartIndex, iEndIndex As Integer

      '	System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_519")
      Select Case DMAcadExt.DMApp.AppID
         Case DMAcadExt.enApplications.Taba
            iStartIndex = 0
            iEndIndex = 25
         Case DMAcadExt.enApplications.TopoMaster
            iStartIndex = 41
            iEndIndex = 41
         Case DMAcadExt.enApplications.Unidiv
            iStartIndex = 51
            iEndIndex = 52
         Case DMAcadExt.enApplications.Ownership
            iStartIndex = 61
				iEndIndex = 62
			Case DMAcadExt.enApplications.BN
				iStartIndex = 63
				iEndIndex = 63
		End Select
      Try
         moResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(miResourceTheme)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzMyInitializeComponent")
      End Try
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      For iIndex As Integer = iStartIndex To iEndIndex
         Me.tcbData.Items.Add(New DMCommon.ItemData(iIndex, zzGetText(iIndex, 1)))
      Next

      If Me.tcbData.Items.Count = 1 Then
         '	Me.tcbData.SelectedItem = New DMCommon.ItemData(iStartIndex, zzGetText(iStartIndex, 1))
         Me.tcbData.SelectedIndex = 0
      End If
      Me.ddbPaintScale.ToolTipText = "קנ""מ צביעה"
      midgvMainLocationY = Me.dgvMain.Location.Y
      Dim oScaleList As System.Collections.Generic.List(Of String) = TplnProject.GetScales()
      Dim iListUB As Integer = oScaleList.Count - 1
      ReDim tmiPaintScale(iListUB)
      For iIndex As Integer = 0 To iListUB
         tmiPaintScale(iIndex) = New System.Windows.Forms.ToolStripMenuItem
         With tmiPaintScale(iIndex)
            .CheckOnClick = True
            .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            .AutoSize = True
            .Text = oScaleList(iIndex)
            .Name = Me.ddbPaintScale.Name & CStr(iIndex)
            Me.ddbPaintScale.DropDownItems.Add(tmiPaintScale(iIndex))
            AddHandler tmiPaintScale(iIndex).Click, AddressOf PaintScale_Click
            If oScaleList(iIndex) = "1:500" Then
               moScaleMenuItemChecked = tmiPaintScale(iIndex)
               .Checked = True
            End If
         End With
      Next
      moMarkedCellStyle = Me.dgvMain.DefaultCellStyle.Clone
      moMarkedCellStyle.BackColor = Color.Red
   End Sub
	Private Sub zzSetPaintScale()
		Dim oProjectData As TopoManager.ProjectData = TopoManager.TPlanGraph.TplnProject.ProjectData
		Dim oTplanProjectData As TplanProjectData
		Dim sPaintScale As String

		If oProjectData IsNot Nothing AndAlso oProjectData.Opened Then
			oTplanProjectData = DirectCast(oProjectData, TplanProjectData)
			sPaintScale = oTplanProjectData.PaintScale(miTopoPurpose)
			If sPaintScale IsNot Nothing Then
				zzSetMenuScale(sPaintScale)
			End If
		End If


		Select Case miTopoPurpose
			Case DMAcadExt.enTopoPurpose.Approved
			Case DMAcadExt.enTopoPurpose.Proposed
		End Select

	End Sub
	Private Sub zzSetMenuScale(ByVal sMenuText As String)
		For iIndex As Integer = 0 To tmiPaintScale.GetUpperBound(0)
			With tmiPaintScale(iIndex)
				If .Text = sMenuText Then
					moScaleMenuItemChecked = tmiPaintScale(iIndex)
					.Checked = True
				Else
					.Checked = False
				End If
			End With
		Next
	End Sub
	Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
		Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, True)
	End Function
	Private Sub zzSetFilterItems(ByVal sItemsList As String)
		Dim saItems() As String
		Dim iListIndex As Integer
		Me.tcbFilter.Items.Clear()
		If Not String.IsNullOrEmpty(sItemsList) Then
			saItems = Strings.Split(sItemsList, ",")
			For iIndex As Integer = 0 To saItems.GetUpperBound(0)
				iListIndex = CInt(saItems(iIndex))
				Me.tcbFilter.Items.Add(New DMCommon.ItemData(iListIndex, zzGetText(iListIndex, 2)))
			Next
		End If
	End Sub
	Private Sub tlbTop_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		'	MessageBox.Show(oToolStripItem.Name, "01_477")
		Select Case oToolStripItem.Name
			Case Me.tbbSaveSelected.Name
				If Me.dgvMain.SelectedRows.Count = 0 Then
					zzUpdateCurrentPolygon()
				Else
					zzUpdateSelectedPolygons()
				End If
			Case Me.tbbSaveAll.Name

			Case Me.tbbZoomPgon.Name

				If miZoomType = enZoomType.Boundary Then
					zzZoomCurrentBoundary()
				ElseIf Me.dgvMain.SelectedRows.Count = 0 Then
					zzZoomCurrentPolygon()
				Else
					zzZoomSelectedPolygons()
				End If
			Case Me.tbbBaseSort.Name
				If moCurrentDataView IsNot Nothing Then
					moCurrentDataView.Sort = msBaseSort
				End If
			Case Me.tsbFilterBySelect.Name
				zzFilterBySelect()

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
			Case Me.tsbRemoveFilter.Name
				zzRemoveFilter()
			Case Me.tbbClose.Name
				Me.Hide()
			Case Me.tbbColorScheme.Name
				zzColorSchemeShow()
			Case Me.tbbFill.Name
				'	zzPaintCurrentPolygon()
				zzPaintSelectedPolygons()
			Case Me.tbbPaintAll.Name
				zzPaintAll()
			Case Me.tbbErasePaint.Name
				zzEraseParcelExeptPaint()
				Dim oPaintLayer As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef

			Case Me.tbbExcel.Name
				zzExportToExcel()
			Case Me.tbbInPlan.Name
				'   --> tbbInPlan.CheckedChanged
		End Select
	End Sub
	Private Sub zzEraseParcelExeptPaint()
		Dim oPaintLayer As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef

		'	MessageBox.Show(sPaintLayer, "04_360")
		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		DMAcadExt.AcadTransaction.ClearLayerList(oPaintLayer.Name)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub zzExportToExcel()

		If moCurrentDataView IsNot Nothing Then
			Dim oRepApp As ExcelReport.Report = New ExcelReport.Report(True)

			If oRepApp.Open() Then
				oRepApp.MainView = moCurrentDataView
				'	oRepApp.DataTableCell=
				If msCurrentCaption IsNot Nothing AndAlso msCurrentCaption.Length <> 0 Then
					AcadReport.BaseReport.Caption = msCurrentCaption
				End If

				Dim saColCaptions(Me.dgvMain.Columns.Count - 1) As String
				Dim daColWidths(Me.dgvMain.Columns.Count - 1) As Double
				Dim iUB As Integer = Me.dgvMain.Columns.Count - 1
				Dim iColIndex As Integer = iUB

				For Each oColumn As DataGridViewColumn In Me.dgvMain.Columns
					'DMAcadExt.AcadDocument.WriteMessage("HeaderText=" & oColumn.HeaderText)
					saColCaptions(iColIndex) = oColumn.HeaderText
					If oColumn.Visible Then
						daColWidths(iUB - iColIndex) = oColumn.Width
					Else
						daColWidths(iUB - iColIndex) = 0
					End If
					iColIndex -= 1
				Next

				AcadReport.BaseReport.ColumnResUB = Me.dgvMain.Columns.Count - 1

				'	AcadReport.BaseReport.NumColumns = AcadReport.BaseReport.ColumnUB + 1

				AcadReport.BaseReport.ColumnCaptions = saColCaptions
				AcadReport.BaseReport.ColWidths = daColWidths
				oRepApp.Insert()
			End If
		End If
	End Sub
	Private Sub zzPaintPolygon(ByVal iTopoID As Integer, ByVal tColorScheme As DMAcadExt.ColorScheme)
		If iTopoID <> 0 Then
			Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
			Dim dScale As Double = Me.zzGetScale()
			Dim oLayer As DMAcadExt.AcadLayerDef
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

			oPolygon = zzGetTopoPolygon(iTopoID)
			If oPolygon IsNot Nothing Then
            Dim bLayerOK As Boolean = True


            oLayer = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(miMapTheme)  ', miTopoPurpose
				bLayerOK = DMAcadExt.AcadTransaction.CreateLayer(oLayer, True)
				If bLayerOK Then
					bLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef(), True, False, True, True)
				End If

				If bLayerOK Then
					oPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme Or DMAcadExt.PaintMethod.ZebraByTopo Or DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True, oLayer.Name)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Polygon was not found", "frmTplnView - zzPaintPolygon", MessageBoxButtons.OK, MessageBoxIcon.Warning)
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			'	zzUpdateBlockShare()

			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End If
	End Sub

	Private Sub zzPaintPolygonAAA(ByVal iTopoID As Integer, ByVal iColorSchemeID As Integer)
		If iTopoID <> 0 Then
			Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
			Dim dScale As Double = Me.zzGetScale()
			Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(iColorSchemeID, dScale)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

			oPolygon = zzGetTopoPolygon(iTopoID)
			If oPolygon IsNot Nothing Then
				Dim bCurrentLayerOK As Boolean = True

            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(TopoManager.TPlanGraph.TplnProject.PaintLayerDef(miMapTheme), True, False, True, True)  ', miTopoPurpose

				If bCurrentLayerOK Then
					oPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme Or DMAcadExt.PaintMethod.ZebraByTopo Or DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("??  Polygon was not found", "frmTplnView - zzZoomPolygon", MessageBoxButtons.OK, MessageBoxIcon.Warning)
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			'	zzUpdateBlockShare()

			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End If
	End Sub
	Private Function zzGetScale() As Double
		Dim sScale As String
		Dim dBaseScale As Double = 1000.0
		If moScaleMenuItemChecked IsNot Nothing Then
			sScale = moScaleMenuItemChecked.Text
			Return DMCommon.Functions.TextToScale(sScale, dBaseScale) / dBaseScale
		Else
			Return 1.0
		End If
	End Function

	Private Sub zzFilterBySelect()
		Dim oGridRow As DataGridViewRow
		Dim oGridColumn As DataGridViewColumn
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		If oCurrentCell IsNot Nothing Then
			Dim sFieldName As String
			Dim sFilterFieldName As String = Nothing

			oGridRow = oCurrentCell.OwningRow
			oGridColumn = oCurrentCell.OwningColumn
			sFieldName = oGridColumn.DataPropertyName
			Select Case sFieldName
				Case TplnParcel.BlockFieldName
					sFilterFieldName = TplnParcel.BlockFieldName
				Case TplnParcel.NameFieldName, TopoReader.msParcelTopoIDFldName
					sFilterFieldName = TopoReader.msParcelTopoIDFldName
				Case TplnParcel.msLotNameFieldName, TopoReader.msLotTopoIDFldName
					sFilterFieldName = TopoReader.msLotTopoIDFldName
				Case TplnParcel.LanduseIDFieldName, TplnParcel.LanduseNameFieldName
					sFilterFieldName = TplnParcel.LanduseIDFieldName
			End Select
			If sFilterFieldName IsNot Nothing Then
				Dim iTopoID As Integer = Me.zzGetRowTopoID(oViewRow, sFilterFieldName)
				msSelectFilter = "(" & sFilterFieldName & "=" & CStr(iTopoID) & ")"
				zzApplyFilter()
			End If
		End If
	End Sub
	Private Sub zzRemoveFilter()
		msSelectFilter = String.Empty
		zzApplyFilter()
	End Sub
	Private Sub zzEraseAll()

	End Sub
	Private Sub zzPaintAll()
		Dim dScale As Double = Me.zzGetScale()

		Select Case DMAcadExt.DMApp.AppID
			Case DMAcadExt.enApplications.Taba
				Select Case miPaintMethod
					Case enPaintMethod.Landuse
						If miTopoPurpose <> TPlanGraph.enTopoPurpose.Undefined AndAlso dScale > 0.0 Then
							TplnParcel.FillColorSchemesDic()
							DMAcadExt.AcadDocument.ResetCounter()
							TPlanGraph.TplnLot.PaintByLanduse(miTopoPurpose, dScale, True, True)
						End If
					Case enPaintMethod.ParcelExcept
						zzPaintExceptParcel()
				End Select

			Case DMAcadExt.enApplications.TopoMaster

			Case DMAcadExt.enApplications.Unidiv

				UnidivNet.UD_Parcel.PaintAllByArea()

		End Select

	End Sub

	Private Sub zzPaintExceptParcel()
		Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(200001, 1.0)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


		TplnParcel.PaintExceptions(tColorScheme, 1.0)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()


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
				System.Windows.Forms.MessageBox.Show("+Polygon was not found" & vbCrLf & CStr(iTopoID) & ":" & miMapTheme.ToString(), "frmTplnView - zzZoomPolygon_", MessageBoxButtons.OK, MessageBoxIcon.Warning)
			End If
		End If
	End Sub
	Private Sub zzZoomBoard(ByVal iTopoID As Integer, ByVal iNeighborTopoID As Integer, ByVal bHighlight As Boolean)
		'msTopoIDNeighborFldName
		If iTopoID <> 0 Then
			Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
			Dim colBoundary As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			oPolygon = zzGetTopoPolygon(iTopoID)
			If oPolygon IsNot Nothing Then
				colBoundary = oPolygon.GetBoundary(iNeighborTopoID)
				If colBoundary IsNot Nothing Then
					DMAcadExt.AcadTransaction.Zoom(colBoundary, bHighlight)
				Else
					System.Windows.Forms.MessageBox.Show("Boundary was not found" & vbCrLf & CStr(iTopoID) & ":" & CStr(miCurrentTopoDefID.ID), "frmTplnView - zzZoomBoard", MessageBoxButtons.OK, MessageBoxIcon.Warning)

				End If

			Else
				System.Windows.Forms.MessageBox.Show("Polygon was not found" & vbCrLf & CStr(iTopoID), "frmTplnView - zzZoomBoard", MessageBoxButtons.OK, MessageBoxIcon.Warning)
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

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
		If mbTopologyExists Then
			iCurrentTopoID = zzGetCurrentTopoID()
			'	System.Windows.Forms.MessageBox.Show(CStr(iCurrentTopoID), "38_490")
			zzZoomPolygon(iCurrentTopoID, True)
		End If
	End Sub
	Private Sub zzZoomCurrentBoundary()
		Dim iTopoID, iNeighborTopoID As Integer
		If miCurrentTopoDefID.ID <> 0 Then
			zzGetCurrentPairTopoID(iTopoID, iNeighborTopoID)
			zzZoomBoard(iTopoID, iNeighborTopoID, True)
		End If
	End Sub

	Private Sub zzColorSchemeShow()
		Dim iCurrentTopoID As Integer
		Dim iCurrentLanduseID As Integer

		If miCurrentTopoDefID.ID <> 0 Then
			iCurrentTopoID = zzGetCurrentTopoID()
			iCurrentLanduseID = zzGetCurrentLanduseID()
			Dim tColorScheme As DMAcadExt.ColorScheme = TplnLot.GetColorScheme(miTopoPurpose, iCurrentLanduseID, 1.0)
			'	MessageBox.Show(CStr(iCurrentLanduseID) & ":" & CStr(tColorScheme.ID), "01_978")
			Dim fEditColorScheme As frmColorEditor = New frmColorEditor(DMAcadExt.enMapTheme.LotApproved, TopoManager.enColorEditorMode.ColorScheme, True, iCurrentLanduseID, tColorScheme.ID)
			'	New frmColorEditor(iMode, bReadOnly, iLanduseID, iColorSchemeID)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, fEditColorScheme)
		End If
	End Sub
	Private Sub zzPaintCurrentPolygonOld()
		Dim iCurrentTopoID As Integer
		Dim iCurrentLanduseID As Integer
		Dim dScale As Double = Me.zzGetScale()
		If miCurrentTopoDefID.ID <> 0 Then
			iCurrentTopoID = zzGetCurrentTopoID()
			iCurrentLanduseID = zzGetCurrentLanduseID()
			Dim tColorScheme As DMAcadExt.ColorScheme = TplnLot.GetColorScheme(miTopoPurpose, iCurrentLanduseID, dScale)

			'	System.Windows.Forms.MessageBox.Show(tColorScheme.ID_Name, "01_456")
			zzPaintPolygon(iCurrentTopoID, tColorScheme)
		End If
	End Sub


	Private Sub zzPaintSelectedPolygons_220217()
		'	MessageBox.Show(miPaintMethod.ToString(), "01_003")
		Dim iTopoID As Integer

		Dim iLanduseIDPrev As Integer = 0

		Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
		Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
		Dim dScale As Double = Me.zzGetScale()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		Dim sLayerName As String = zzSetPaintLayer()





		If sLayerName IsNot Nothing Then
			'  MessageBox.Show(sLayerName & vbCrLf & CStr(Me.dgvMain.SelectedRows.Count) & vbCrLf & CStr(dScale), "01_548a")
			If miPaintMethod = enPaintMethod.BlockParcel Then
				TplnParcel.FillColorSchemesDic()
			End If
			For Each oViewRow As DataGridViewRow In Me.dgvMain.SelectedRows
				iTopoID = zzGetRowTopoID(oViewRow)
				tColorScheme = zzGetColorScheme(oViewRow, dScale)
				'	MessageBox.Show(tColorScheme.ID_Name & ":" & CStr(iTopoID) & ":" & CStr(tColorScheme.ID), "01_515")
				Try
					If iTopoID <> 0 Then
						oPolygon = zzGetTopoPolygon(iTopoID)
						If oPolygon IsNot Nothing Then
							oPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayerName)
						Else
							System.Windows.Forms.MessageBox.Show("Polygon was not found", "27_442")
						End If
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzPaintSelectedPolygons")
				End Try
			Next
		End If


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub

	Private Sub zzPaintSelectedPolygons()
		'	MessageBox.Show(miPaintMethod.ToString(), "01_003")
		'    Dim iTopoID As Integer

		'     Dim iLanduseIDPrev As Integer = 0

		'   Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon
		'   Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
		'   Dim dScale As Double = Me.zzGetScale()
		miCurrentLanduseID = 0
		Dim tPaintParam As PaintParam = zzGetPaintParam()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		'    Dim sLayerName As String = zzSetPaintLayer()

		Dim tLayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(miMapTheme)  ', iTopoPurpose


		Dim bLayerOK As Boolean
		If tPaintParam.LanduseByLayer Then
			bLayerOK = True
			tPaintParam.LayerDef = tLayerDef
		Else
			bLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, False, True, True)
		End If

		If bLayerOK Then
			'   MessageBox.Show(tLayerDef.Name & vbCrLf & CStr(Me.dgvMain.SelectedRows.Count) & vbCrLf & CStr(tPaintParam.Scale) & vbCrLf & CStr(tPaintParam.ByPassedLine) & vbCrLf & CStr(tPaintParam.LanduseByLayer), "01_548b")
			If miPaintMethod = enPaintMethod.BlockParcel Then
				TplnParcel.FillColorSchemesDic()
			End If
			If Me.dgvMain.SelectedRows.Count > 0 Then
				For Each oViewRow As DataGridViewRow In Me.dgvMain.SelectedRows
					zzPaintPolygon(oViewRow, tPaintParam)
				Next
			Else
				Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
				If oViewRow IsNot Nothing Then
					zzPaintPolygon(oViewRow, tPaintParam)
				End If
			End If

		Else
			MessageBox.Show(tLayerDef.Name & ":" & CStr(146) & ":" & CStr(147), "01_383")

		End If


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub
	Private Function zzPaintPolygon(oViewRow As DataGridViewRow, tPaintParam As PaintParam) As DMAcadExt.PaintException
		Dim iTopoID As Integer
		'  Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
		Dim oPolygon As TopoManager.TPlanGraph.TplnTopoPgon

		iTopoID = zzGetRowTopoID(oViewRow)
		If iTopoID <> 0 Then
			zzGetColorScheme(oViewRow, tPaintParam.Scale)
			If tPaintParam.LanduseByLayer Then

				Dim bLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(tPaintParam.LayerDef, True, False, False, False, miCurrentLanduseID)
				'  MessageBox.Show(CStr(bLayerOK) & ":" & CStr(tPaintParam.LayerDef.Name), "05_115")

			End If
			If tPaintParam.ByPassedLine Then
				zzOpenTopoScheme()
				If moTopoScheme IsNot Nothing Then
					moTopoScheme.CreatePgonDBPolyline(iTopoID)
				Else
					MessageBox.Show("moTopoScheme Is Nothing", "03_211")

				End If

			End If

			'	MessageBox.Show(tColorScheme.ID_Name & ":" & CStr(iTopoID) & ":" & CStr(tColorScheme.ID), "01_515")
			Try
				oPolygon = zzGetTopoPolygon(iTopoID)
				If oPolygon IsNot Nothing Then
					Return oPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme, mtCurrentColorScheme, True)
				Else
					System.Windows.Forms.MessageBox.Show("Polygon was not found", "27_442")
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzPaintSelectedPolygons")
			End Try
		End If

		Return DMAcadExt.PaintException.BadInput
	End Function
	Private Function zzGetPaintParam() As PaintParam
		Dim tPaintParam As PaintParam = New PaintParam()
		tPaintParam.ByPassedLine = Me.tsmiBypassLine.Checked
		tPaintParam.LanduseByLayer = Me.tsmiLayerByLanduse.Checked
		tPaintParam.Scale = zzGetScale()
		Return tPaintParam
	End Function
	Private Function zzSetPaintLayer() As String
		Dim tLayerDef As DMAcadExt.AcadLayerDef

		Dim bLayerOK As Boolean = True
		Select Case miPaintMethod
			Case enPaintMethod.Landuse
				tLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(miMapTheme)  ', miTopoPurpose

			Case enPaintMethod.BlockParcel
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchPgons)
			Case Else
				Return Nothing
		End Select

		bLayerOK = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
		If bLayerOK Then
			bLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef(), True, False, True, True)
		End If
		If bLayerOK Then
			Return tLayerDef.Name
		Else
			Return Nothing
		End If

	End Function
	Private Sub zzSetColorSchemeByLanduse(oViewRow As DataGridViewRow, dScale As Double)
		Dim iLanduseID As Integer = zzGetRowLanduseID(oViewRow)
		'     System.Windows.Forms.MessageBox.Show(iLanduseID.ToString(), "04_018")
		If iLanduseID <> miCurrentLanduseID Then
			miCurrentLanduseID = iLanduseID
			mtCurrentColorScheme = TplnLot.GetColorScheme(miTopoPurpose, iLanduseID, dScale)

		End If





	End Sub
	Private Function zzGetColorSchemeByParcelName(oViewRow As DataGridViewRow, dScale As Double) As DMAcadExt.ColorScheme
		Dim sParcelNameKey As String = zzGetRowParcelNameKey(oViewRow)

		If sParcelNameKey IsNot Nothing Then

			Return TplnParcel.GetColorScheme(sParcelNameKey, dScale)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzGetColorScheme(oViewRow As DataGridViewRow, dScale As Double) As DMAcadExt.ColorScheme
		Select Case miPaintMethod
			Case enPaintMethod.Landuse
				zzSetColorSchemeByLanduse(oViewRow, dScale)
				Return Nothing
			Case enPaintMethod.BlockParcel
				Return zzGetColorSchemeByParcelName(oViewRow, dScale)
			Case Else
				Return New DMAcadExt.ColorScheme()
		End Select


	End Function
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
							If oAddBoundingBox IsNot Nothing AndAlso (Not oAddBoundingBox.IsEmpty) Then
								oBoundingBox.Union(oAddBoundingBox)
							Else
								System.Windows.Forms.MessageBox.Show("Bounding Box was not found", "27_443")
							End If
						Else
							System.Windows.Forms.MessageBox.Show("Polygon was not found" & vbCrLf & CStr(iTopoID) & vbCrLf & CStr(miCurrentTopoDefID.ID), "27_441")
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

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			For Each oGridViewRow As DataGridViewRow In Me.dgvMain.SelectedRows
				oDataRowView = DirectCast(oGridViewRow.DataBoundItem, DataRowView)
				oDataRow = oDataRowView.Row
				iTopoID = zzGetRowTopoID(oGridViewRow)
				Try
					If iTopoID <> 0 Then
						oBamashPolygon = BamashNet.bmBamash.GetBamashPgon(iTopoID)

						If oBamashPolygon IsNot Nothing Then
							oBamashPolygon.GetDataFromTable(oDataRow)
							oBamashPolygon.UpdateAllData()
						Else
							System.Windows.Forms.MessageBox.Show("Polygon was not found", "27_442")
						End If
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzUpdateSelectedPolygons")
				End Try
			Next
			DMAcadExt.AcadDocument.UpdateScreen()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.Zoom(oBoundingBox)
			DMAcadExt.AcadDocument.UpdateScreen()
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

	Private Sub zzGetCurrentPairTopoID(ByRef iTopoID As Integer, ByRef iNeighborTopoID As Integer)
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		iTopoID = zzGetRowTopoID(oViewRow)
		iNeighborTopoID = zzGetNeighborRowTopoID(oViewRow)
	End Sub

	Private Function zzGetCurrentTopoID() As Integer
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Return zzGetRowTopoID(oViewRow)
	End Function
	Private Function zzGetCurrentLanduseID() As Integer
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Return zzGetRowLanduseID(oViewRow)
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
					Dim iNeighborTopoID As Integer
					If miZoomType = enZoomType.Boundary Then
						iNeighborTopoID = zzGetNeighborRowTopoID(oNewViewRow)
						zzZoomBoard(iTopoID, iNeighborTopoID, False)
					Else
						zzZoomPolygon(iTopoID, False)
					End If

				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzMovePolygon")
			End Try

		End If
	End Sub
	Private Function zzGetRowTopoID(ByVal oViewRow As DataGridViewRow, sTopoIDFieldName As String) As Integer
		Dim oValue As System.Object
		Try
			oValue = oViewRow.Cells.Item(sTopoIDFieldName).Value
			If oValue IsNot Nothing AndAlso Not IsDBNull(oValue) Then
				'System.Windows.Forms.MessageBox.Show(oValue.ToString() & vbCrLf & sTopoIDFieldName, "01_837")
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

	Private Function zzGetRowTopoIDOld(ByVal oViewRow As DataGridViewRow) As Integer
		Dim oValue As System.Object
		Try
			oValue = oViewRow.Cells.Item(TopoReader.msTopoIDFldName).Value
			If oValue IsNot Nothing Then
				'System.Windows.Forms.MessageBox.Show(oValue.ToString(), "01_837")
				Return DirectCast(oValue, Integer)
			Else
				Return 0
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzGetRowTopoID")
			Return 0
		End Try
	End Function
	Private Function zzGetNeighborRowTopoID(ByVal oViewRow As DataGridViewRow) As Integer
		Dim oValue As System.Object
		Try
			oValue = oViewRow.Cells.Item(UnidivNet.UD_Parcel.msTopoIDNeighborFldName).Value
			If oValue IsNot Nothing Then
				'System.Windows.Forms.MessageBox.Show(oValue.ToString(), "01_837")
				Return DirectCast(oValue, Integer)
			Else
				Return 0
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzGetRowTopoID")
			Return 0
		End Try
	End Function
	Private Function zzGetRowParcelNameKey(ByVal oViewRow As DataGridViewRow) As String
		Dim oValue As System.Object
		Dim iBlock As Integer
		Dim sParcelName As String
		Try
			oValue = oViewRow.Cells.Item(TplnParcel.BlockFieldName).Value
			If oValue IsNot Nothing Then
            iBlock = DirectCast(oValue, Integer)
            oValue = oViewRow.Cells.Item(TplnParcel.NameFieldName).Value
            If oValue IsNot Nothing Then
               sParcelName = DirectCast(oValue, String)
               Return TplnParcel.GetNameKey(iBlock, sParcelName)
            End If
         End If
         Return Nothing
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzGetRowTopoID")
         Return Nothing
      End Try
   End Function
   Private Function zzGetRowLanduseID(ByVal oViewRow As DataGridViewRow) As Integer
      Dim oValue As System.Object
      Try
			oValue = oViewRow.Cells.Item(TplnParcel.LanduseIDFieldName).Value
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
		Select Case DMAcadExt.DMApp.AppID
			Case DMAcadExt.enApplications.Taba
				If miMapTheme = DMAcadExt.enMapTheme.Undefined Then
					Return TopoManager.TPlanGraph.TplnProject.GetTopoPolygon(iTopoID, miCurrentTopoDefID)
				Else
					Return TopoManager.TPlanGraph.TplnProject.GetTopoPolygon(iTopoID, miMapTheme)
				End If

			Case DMAcadExt.enApplications.TopoMaster
				Return BamashNet.bmBamash.GetBamashPgon(iTopoID)
			Case DMAcadExt.enApplications.Unidiv
				Dim oParcel As UnidivNet.UD_Parcel = Nothing
				If UnidivNet.Unidiv.TryGetParcel(iTopoID, oParcel) Then
					Return oParcel
				Else
					Return Nothing
				End If
			Case DMAcadExt.enApplications.Ownership
				Return TplnOwnerProject.GetOwnerPgon(iTopoID)
			Case DMAcadExt.enApplications.BN
				Return TplnBNProject.GetBNPgon(iTopoID)
			Case Else
				Return Nothing
		End Select
	End Function
	Private Function zzGetCurrentTopoNameAAA() As String
		If msTopologyName IsNot Nothing Then
			Return msTopologyName
		Else
			Return Nothing

		End If
		'	frmTplnView.vb:line 918
		'\frmTplnView.vb:line 923
	End Function

	Private Function zzGetCurrentTopoName310315() As String
      If msTopologyName IsNot Nothing Then
         Return msTopologyName
      ElseIf miCurrentTopoDefID.ID <> 0 Then
         Dim oTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(miCurrentTopoDefID)
         If oTopoDef IsNot Nothing Then
            Return oTopoDef.Name
         Else
            Return Nothing
         End If
      Else
         Return Nothing
      End If
      '	frmTplnView.vb:line 918
      '\frmTplnView.vb:line 923
   End Function
   Private Function zzGetPolygon(ByVal iTopoID As Integer) As Autodesk.Gis.Map.Topology.Polygon
      If moTopoModel Is Nothing Then
			moTopoModel = DMAcadExt.AcadMapApp.GetTopology(msTopologyName)
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
                  oDocLock = Nothing
               End If
               Return Nothing
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
   Private Sub zzOpenTopoScheme()
      If moTopoScheme Is Nothing AndAlso moTopoModel IsNot Nothing Then
         moTopoScheme = New TopoManager.TopoScheme.tsTopology(moTopoModel.Name)
         moTopoScheme.Load(False, moTopoModel)
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
      If moTopoScheme IsNot Nothing Then
         moTopoScheme = Nothing
      End If
   End Sub
   Private Sub zzFindPgonNew(ByVal bHideForm As Boolean, ByVal bRepeat As Boolean)
      If mbTopologyExists Then


         Dim iTopoID As Integer
         Dim iRowIndex As Integer
         Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim bResp As Boolean
			Dim oPgon As Polygon
			If bHideForm Then
            Me.Hide()
         End If
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			Do
            bResp = DMAcadExt.AcadDocument.GetPoint("Select Point ", tPoint)
            DMAcadExt.AcadDocument.WriteMessage("Point=" & tPoint.ToString())
				Try
					oPgon = moTopoModel.FindPolygon(tPoint)
					If oPgon IsNot Nothing Then
						iTopoID = oPgon.ID
					End If

				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("TopoExp=" & oEx.Message)
            End Try
            DMAcadExt.AcadDocument.WriteMessage("TopoID=" & CStr(iTopoID))
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


         Loop While bRepeat AndAlso bResp
         If bHideForm Then Me.Show()
         DMAcadExt.AcadDocument.CloseMessage()
         ''   Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", False, False, False)

      End If
   End Sub
   Private Sub zzFindPgon(ByVal bHideForm As Boolean, ByVal bRepeat As Boolean)
      If mbTopologyExists Then
			'	Dim sTopoName As String = msTopologyName
			If msTopologyName IsNot Nothing Then
				Dim iTopoID As Integer
				Dim iRowIndex As Integer
				If bHideForm Then
					Me.Hide()
				End If
				MessageBox.Show(msTopologyName, "01_601")
				Do
					iTopoID = TopoManager.TPlanGraph.TplnProject.FindPgonByPoint(msTopologyName)
					DMAcadExt.AcadDocument.WriteMessage("TopoID=" & CStr(iTopoID))
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
      Dim oCoordinateCellStyle As DataGridViewCellStyle = New DataGridViewCellStyle()
      Dim iColWidth As Integer
      oAreaCellStyle.Format = TopoManager.TPlanGraph.TplnProject.AreaFormat
		oCoordinateCellStyle.Format = TopoManager.TPlanGraph.TplnProject.CoordinateFormat
		Try
         oDataResource = moResource.GetChild(miColumnSetIndex)
         If oDataResource IsNot Nothing Then
				For Each oColumn As DataGridViewColumn In Me.dgvMain.Columns
					Try
						oColumn.HeaderText = zzGetText(oColumn.Index, 10 + miColumnSetIndex)
						oColumn.ToolTipText = zzGetText(oColumn.Index + 50, 10 + miColumnSetIndex)

						iColWidth = oDataResource.GetIntItem(oColumn.Index)(0)
						If iColWidth = 0.0 OrElse (moHiddenColumns IsNot Nothing AndAlso moHiddenColumns.ContainsKey(oColumn.Index)) Then
							oColumn.Visible = False
						Else
							oColumn.Width = iColWidth
						End If

					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oColumn.Name & vbCrLf & oColumn.Index.ToString(), "frmTplnView - zzSetDatagridColumns_1")
					End Try
					Try
						If oColumn.Width = 0 Then
							oColumn.Visible = False
						Else
							Select Case oColumn.Name
								Case TopoReader.msAreaFldName, TopoReader.msRoundedAreaFldName, TopoReader.msCalcArea2FldName, TopoReader.msSumPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msSumApprPgonAreaMergeFldName, TopoManager.TPlanGraph.TplnParcel.msSumPropPgonAreaMergeFldName, TopoManager.TPlanGraph.TplnParcel.PgonAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaPropFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaApprFieldName, TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaMergeFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaUnionFieldName, TopoManager.TPlanGraph.TplnParcel.msCalcAreaFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaApprUnFieldName, TopoManager.TPlanGraph.TplnParcel.msInPlanCalcAreaPropUnFieldName, TopoManager.TPlanGraph.TplnParcel.msSumApprUnPgonAreaFldName, TopoManager.TPlanGraph.TplnParcel.msSumPropUnPgonAreaFldName, TopoManager.TPlanGraph.TplnLot.msCalcAreaFieldName, TopoManager.TPlanGraph.TplnLot.msCalcAreaUnionFieldName, TopoReader.msSumPgonAreaUnionFldName, TopoReader.msSumPgonAreaFDO_OverlayFldName, TplnParcel.msInPlanAreaApprMergeFieldName, TplnParcel.msInPlanAreaPropMergeFieldName, TplnParcel.msInPlanCalcAreaApprMergeFieldName, TplnParcel.msInPlanCalcAreaPropMergeFieldName, TplnParcel.msInPlanCalcArea2ApprMergeFieldName, TplnParcel.msInPlanCalcArea2PropMergeFieldName, TplnParcel.msInPlanRoundedAreaApprMergeFieldName, TplnParcel.msInPlanRoundedAreaPropMergeFieldName, TplnParcel.msInPlanCalcArea2MergeFieldName, TplnParcel.msInPlanRoundedAreaMergeFieldName, TplnParcel.msInPlanAreaMergeFieldName, TplnParcel.msLotAreaFieldName

									oTextBoxColumn = DirectCast(oColumn, DataGridViewTextBoxColumn)
									oTextBoxColumn.DefaultCellStyle = oAreaCellStyle
								Case TopoReader.msCentroidXFldName, TopoReader.msCentroidYFldName
									oTextBoxColumn = DirectCast(oColumn, DataGridViewTextBoxColumn)
									oTextBoxColumn.DefaultCellStyle = oCoordinateCellStyle
									'		System.Windows.Forms.MessageBox.Show(oCoordinateCellStyle.Format, "01_341")
							End Select
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetDatagridColumns_2")
					End Try
				Next
			Else
				System.Windows.Forms.MessageBox.Show("Resource Is Nothing " & CStr(miColumnSetIndex) & vbCrLf & "Count = " & CStr(moResource.ChildCount), "frmTplnView - zzSetDatagridColumns_4")
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetDatagridColumns_3")
		End Try
	End Sub
	Private Sub zzDispUD_Parcels()
		moCurrentDataView = UnidivNet.UD_Parcel.MainView
		If moCurrentDataView IsNot Nothing Then
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)
			mbTopologyExists = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 9
			miZoomType = enZoomType.Polygon

		End If
	End Sub

	Private Sub zzDispOwnerPgons()
		moCurrentDataView = TopoManager.TPlanGraph.TplnOwnerPgon.MainView
		If moCurrentDataView IsNot Nothing Then
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)
			mbTopologyExists = True
			msTopologyName = TopoManager.TPlanGraph.TplnOwnerProject.TopologyName
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 12
			miZoomType = enZoomType.Polygon
			SetCmdFindPgon(-80)
		End If

	End Sub

	Private Sub zzDispBNPgons()
		moCurrentDataView = TopoManager.TPlanGraph.TplnBNPgon.MainView
		Dim tMapThemeData As DMAcadExt.MapThemeData = Nothing
		If moCurrentDataView IsNot Nothing Then
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)
			miMapTheme = DMAcadExt.enMapTheme.BN
			mbTopologyExists = True
			msTopologyName = TopoManager.TPlanGraph.TplnBNProject.TopologyName
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 17
			miZoomType = enZoomType.Polygon
			SetCmdFindPgon(-80)
			If mdicMapThemes.TryGetValue(miMapTheme, tMapThemeData) Then
				moTopoModel = TopoManager.TopoCreator.GetOpenedTopology(tMapThemeData, OpenMode.ForRead)
				zzSetFilterItems("0")

			End If
		End If


	End Sub
	Private Sub SetCmdFindPgon(iShiftX As Integer)
		Dim iY As Integer = Me.cmdFindPgonLoop.Location.Y
		Me.cmdFindPgonLoop.Location = New System.Drawing.Point(micmdFindPgonLoopLocationX + iShiftX, iY)
		Me.cmdFindPgon.Location = New System.Drawing.Point(micmdFindPgonLoopLocationX + iShiftX + Me.cmdFindPgonLoop.Width, iY)

	End Sub
	Private Sub zzDispOwners()
		moCurrentDataView = TopoManager.TPlanGraph.TplnOwner.GetView()
		If moCurrentDataView IsNot Nothing Then
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)

			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 13
			miZoomType = enZoomType.Polygon
		End If
	End Sub
	Private Sub zzDispUD_AdjoiningParcels()
		moCurrentDataView = UnidivNet.UD_Parcel.AdjoiningParcelsView
		If moCurrentDataView IsNot Nothing Then
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			mbTopologyExists = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 10
			miZoomType = enZoomType.Boundary
		End If
	End Sub
	Private Sub zzDispParcels()
		Dim tMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.MainView
		miMapTheme = DMAcadExt.enMapTheme.Parcels
		' DMCommon.Debug.MsgBox("12_363P", miMapTheme, "zzDispParcels")
		If moCurrentDataView IsNot Nothing Then
			If mdicMapThemes IsNot Nothing AndAlso mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tMapThemeData) Then
			Else
				tMapThemeData = mtInitMapThemeData
			End If

			moTopoModel = TopoManager.TopoCreator.GetOpenedTopology(tMapThemeData, OpenMode.ForRead)
			If moTopoModel IsNot Nothing Then

				mbTopologyExists = True
				msBaseFilter = moCurrentDataView.RowFilter
				msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
				moHiddenColumns = TopoManager.TPlanGraph.TplnParcel.MainHiddenColumns

				miColumnSetIndex = 0
				miZoomType = enZoomType.Polygon
				msAreaFieldName = TopoReader.msAreaFldName
			End If



		End If
	End Sub
	Private Sub zzDispExpros()
		miColumnSetIndex = 8
		moCurrentDataView = TopoManager.TPlanGraph.TplnExpro.MainView
		If moHiddenColumns IsNot Nothing Then
			moHiddenColumns.Clear()
		End If

		If moCurrentDataView IsNot Nothing Then
			miMapTheme = DMAcadExt.enMapTheme.Expropriation
			msTopologyName = TopoManager.TPlanGraph.TplnProject.GetTopologyName(miMapTheme, False)
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Undefined)
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parce)
			'	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.Columns.Count) & vbCrLf & CStr(miColumnSetIndex) & vbCrLf & CStr(moCurrentDataView.Count), "01_430")
			mbTopologyExists = True

			mbPolygons = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnExpro.InPlanFilter
			moHiddenColumns = TopoManager.TPlanGraph.TplnExpro.MainHiddenColumns


			miZoomType = enZoomType.Polygon
			msAreaFieldName = TopoReader.msAreaFldName
		Else
			System.Windows.Forms.MessageBox.Show("moCurrentDataView Is  Nothing" & vbCrLf & CStr(1952), "01_505")
		End If
	End Sub
	Private Sub zzDispZones()

		miColumnSetIndex = 16
		moCurrentDataView = TopoManager.TPlanGraph.TplnZone.MainView
		If moHiddenColumns IsNot Nothing Then
			moHiddenColumns.Clear()
		End If

		If moCurrentDataView IsNot Nothing Then
			miMapTheme = DMAcadExt.enMapTheme.Zone
			msTopologyName = TopoManager.TPlanGraph.TplnProject.GetTopologyName(miMapTheme, False)
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Undefined)
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parce)
			'	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.Columns.Count) & vbCrLf & CStr(miColumnSetIndex) & vbCrLf & CStr(moCurrentDataView.Count), "01_430")
			mbTopologyExists = True

			mbPolygons = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = "" 'TopoManager.TPlanGraph.TplnExpro.InPlanFilter
			moHiddenColumns = TopoManager.TPlanGraph.TplnExpro.MainHiddenColumns


			miZoomType = enZoomType.Polygon
			msAreaFieldName = TopoReader.msAreaFldName
		Else
			System.Windows.Forms.MessageBox.Show("moCurrentDataView Is  Nothing" & vbCrLf & CStr(1952), "01_505")
		End If
	End Sub
	Private Sub zzDispMerhavs()
		miColumnSetIndex = 11
		moCurrentDataView = TopoManager.TPlanGraph.TplnMerhav.MainView
		If moCurrentDataView IsNot Nothing Then
			miMapTheme = DMAcadExt.enMapTheme.Expropriation
			msTopologyName = TopoManager.TPlanGraph.TplnProject.GetTopologyName(miMapTheme, False)
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Undefined)
			'	miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parce)
			'	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.Columns.Count) & vbCrLf & CStr(miColumnSetIndex) & vbCrLf & CStr(moCurrentDataView.Count), "01_430")
			mbTopologyExists = True

			mbPolygons = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			moHiddenColumns = TopoManager.TPlanGraph.TplnParcel.MainHiddenColumns


			miZoomType = enZoomType.Polygon
			msAreaFieldName = TopoReader.msAreaFldName
		Else
			System.Windows.Forms.MessageBox.Show("moCurrentDataView Is  Nothing" & vbCrLf & CStr(1959), "01_506")
		End If
	End Sub
	Private Sub zzDispPlanParcels()
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.InPlanView
		miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
		mbTopologyExists = True
		miColumnSetIndex = 0
	End Sub
	Private Sub zzDispBlocksA()
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.MainView
		If moCurrentDataView IsNot Nothing Then
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			mbTopologyExists = True
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.InPlanFilter
			miColumnSetIndex = 0
		End If
	End Sub
	Private Sub zzSetLotMapTheme(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
		Select Case iTopoPurpose
			Case DMAcadExt.enTopoPurpose.Approved
				miMapTheme = DMAcadExt.enMapTheme.LotApproved
			Case DMAcadExt.enTopoPurpose.Proposed
				miMapTheme = DMAcadExt.enMapTheme.LotProposed
		End Select

	End Sub
	Private Sub zzDispLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
		Dim tMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		moCurrentDataView = TopoManager.TPlanGraph.TplnLot.MainView(iTopoPurpose, False)

		zzSetLotMapTheme(iTopoPurpose)
		' DMCommon.Debug.MsgBox("12_363L", miMapTheme, "zzDispLots", DMCommon.Debug.ColCount(moCurrentDataView))

		If moCurrentDataView IsNot Nothing AndAlso mdicMapThemes IsNot Nothing AndAlso mdicMapThemes.TryGetValue(miMapTheme, tMapThemeData) Then

			moTopoModel = TopoManager.TopoCreator.GetOpenedTopology(tMapThemeData, OpenMode.ForRead)
			If moTopoModel IsNot Nothing Then
				mbTopologyExists = True
				miZoomType = enZoomType.Polygon
				msBaseFilter = moCurrentDataView.RowFilter
				msInPlanFilter = TopoManager.TPlanGraph.TplnLot.InPlanFilter
				moHiddenColumns = TopoManager.TPlanGraph.TplnLot.MainHiddenColumns(iTopoPurpose)
				miColumnSetIndex = 1
				msAreaFieldName = TopoReader.msAreaFldName
			End If
		End If
	End Sub
	Private Sub zzDispBlocks()
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.BlockView(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
		If moCurrentDataView Is Nothing Then
			'System.Windows.Forms.MessageBox.Show("moCurrentDataView Is Nothing", "01_579b")
		Else
			'System.Windows.Forms.MessageBox.Show(CStr(moCurrentDataView.Count), "01_584e")
		End If
		miCurrentTopoDefID = TPlanGraph.TplnProject.GetDissolveID(DMAcadExt.enTopoPurpose.Parcel)
		mbTopologyExists = False
		miColumnSetIndex = 2
	End Sub
	Private Sub zzDispLotsContent()
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.LotContentView
		miCurrentTopoDefID.ID = 0
		miColumnSetIndex = 5
	End Sub
	Private Sub zzDispLanduse(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.LanduseView(iTopoPurpose)
		miCurrentTopoDefID.ID = 0
		miColumnSetIndex = 3
	End Sub
	Private Sub zzDispLandusePgons(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

		''''''''''''''''''''''''''''''''''''''''''''''''''
		Dim tMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
		zzSetLotMapTheme(iTopoPurpose)

		moCurrentDataView = TopoManager.TPlanGraph.TplnLusePgon.GetLusePgonDataTable(iTopoPurpose)
		''''''''''''  DMCommon.ExcelLogD.SetDataTable(moCurrentDataView, 0)
		miColumnSetIndex = 6
		'  miMapTheme = DMAcadExt.enMapTheme.LanduseApproved
		If moCurrentDataView IsNot Nothing AndAlso mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tMapThemeData) Then
			moTopoModel = TopoManager.TopoCreator.GetOpenedTopology(tMapThemeData.DissolveTopoName, OpenMode.ForRead, True, True)
			moHiddenColumns = TopoManager.TPlanGraph.TplnLot.MainHiddenColumns(iTopoPurpose)
			mbTopologyExists = True
			miZoomType = enZoomType.Polygon
			msAreaFieldName = TopoReader.msAreaFldName
		End If
		miMapTheme = DMAcadExt.enMapTheme.LanduseApproved

	End Sub
	Private Sub zzDispBamashPgons()
		moCurrentDataView = BamashNet.BamashPolygon.MainView
		miCurrentTopoDefID = New DMAcadExt.TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Bamash)
		mbTopologyExists = True
		miColumnSetIndex = 21
	End Sub
	Private Sub zzDispPolygons(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = TopoManager.TPlanGraph.UnionPgonArea.GetTopoPurpose(iOverlayIndex)
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod = TopoManager.TPlanGraph.UnionPgonArea.GetOverlayMethod(iOverlayIndex)
		miCurrentTopoDefID = New DMAcadExt.TopoDefID(iOverlayIndex)
		mbTopologyExists = True
		If iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay Then
			miMapTheme = DMAcadExt.enMapTheme.ParcelsXApproved
		End If


		moCurrentDataView = TopoManager.TPlanGraph.TplnParcel.PolygonView(iOverlayIndex)
		If moCurrentDataView IsNot Nothing Then
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, iTopoPurpose, iOverlayMethod)
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(iOverlayIndex)
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.PolygonInPlanFilter
			miColumnSetIndex = 4
		Else
			System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "frmTplnView - zzDispPolygons")
		End If
		msAreaFieldName = TplnParcel.PgonAreaFieldName
	End Sub
	Private Sub zzDispOverlayGroup(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = TopoManager.TPlanGraph.UnionPgonArea.GetTopoPurpose(iOverlayIndex)
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod = TopoManager.TPlanGraph.UnionPgonArea.GetOverlayMethod(iOverlayIndex)
		'		MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & iTopoPurpose.ToString() & vbCrLf & iOverlayMethod.ToString(), "05_410")
		moCurrentDataView = TopoManager.TPlanGraph.TplnProject.OverlayGroupView(iOverlayIndex)
		If iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay And iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
			miMapTheme = DMAcadExt.enMapTheme.ParcelsXApproved
		End If

		If moCurrentDataView IsNot Nothing Then
			miCurrentTopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, iTopoPurpose, iOverlayMethod)
			msBaseFilter = moCurrentDataView.RowFilter
			msInPlanFilter = TopoManager.TPlanGraph.TplnParcel.PolygonInPlanFilter
			miColumnSetIndex = 7
		Else
			MessageBox.Show("moCurrentDataView Is Nothing", "01_432")
		End If
	End Sub
	Private Sub zzDifArea(ByVal iStatus As enTopoPurpose)
		Dim sTolerance As String = Me.zzGetTolerance()
		If sTolerance.Length <> 0 Then
			Dim sPgonAreaFldName As String
			Select Case iStatus
				Case TopoManager.TPlanGraph.enTopoPurpose.Approved
					sPgonAreaFldName = TopoManager.TPlanGraph.TplnParcel.msSumApprPgonAreaMergeFldName
				Case TopoManager.TPlanGraph.enTopoPurpose.Proposed
					sPgonAreaFldName = TopoManager.TPlanGraph.TplnParcel.msSumPropPgonAreaMergeFldName
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
		Dim sResult As String
		Try
			If msBaseFilter.Length <> 0 Then
				sResult = "(" & msBaseFilter & ")"
			Else
				sResult = String.Empty
			End If

			If Me.tbbInPlan.Checked Then
				If sResult.Length <> 0 Then
					sResult &= sAnd
				End If
				sResult &= "(" & msInPlanFilter & ")"
			End If
			If msTaskFilter.Length <> 0 Then
				If sResult.Length <> 0 Then
					sResult &= sAnd
				End If
				sResult &= "(" & msTaskFilter & ")"
			End If
			If msSelectFilter.Length <> 0 Then
				If sResult.Length <> 0 Then
					sResult &= sAnd
				End If
				sResult &= "(" & msSelectFilter & ")"
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzBuildFilter")
			sResult = String.Empty
		End Try
		Return sResult
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
		If sTolerance.Length <> 0 AndAlso msAreaFieldName.Length <> 0 Then
			msTaskFilter = "(" & msAreaFieldName & "<=" & sTolerance & ")"
		Else
			msTaskFilter = String.Empty
		End If

	End Sub
	Private Sub zzDoubleNames()
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = miCurrentTopoDefID.BaseID
		Dim sFilter As String
		If iTopoPurpose <> DMAcadExt.enTopoPurpose.Undefined Then
			sFilter = TplnProject.GetDoubleNamesCriteria(iTopoPurpose)
			If sFilter.Length = 0 Then
				moCurrentDataView.RowStateFilter = DataViewRowState.None
			Else
				msTaskFilter = sFilter
				MessageBox.Show(msTaskFilter & vbCrLf & miCurrentTopoDefID.BaseID.ToString())
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
		'   System.Windows.Forms.MessageBox.Show(miMapTheme.ToString() & vbCrLf & CStr(moCurrentDataView IsNot Nothing), "06_400")
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
							MessageBox.Show(miMapTheme.ToString() & vbCrLf & miCurrentTopoDefID.BaseID.ToString())
							Me.zzDoubleNames()
					End Select
					zzApplyFilter()
					'  System.Windows.Forms.MessageBox.Show("" & vbCrLf & zzBuildFilter(), "08_320")
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & zzBuildFilter(), "frmTplnView - zzSetFilter_2")
				End Try
			End If
			zzClearDicRows()

		End If
	End Sub
	Private Sub zzApplyFilter()
		moCurrentDataView.RowFilter = zzBuildFilter()
	End Sub
	Private Sub tcbData_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tcbData.SelectedIndexChanged
		'	Dim oItemData As DMCommon.ItemData
		'	Dim oSelectedItem As System.Object
		'$$% 1777
		msBaseFilter = String.Empty
		msInPlanFilter = String.Empty
		msTaskFilter = String.Empty
		msSelectFilter = String.Empty
		Try
			'	DMCommon.Debug.MsgBox("12_364a", mtInitMapThemeData.MapThemeName)
			zzCloseCurrentTopology()

			zzSetData()
			' DMCommon.Debug.MsgBox("12_364c", mtInitMapThemeData.MapThemeName)
			If moCurrentDataView Is Nothing Then
				msBaseSort = String.Empty
			Else
				msBaseSort = moCurrentDataView.Sort
			End If
			'    DMCommon.Debug.MsgBox("12_364d", mtInitMapThemeData.MapThemeName)
			Me.tbbInPlan.Enabled = msInPlanFilter.Length <> 0
			zzClearDicRows()
			Me.dgvMain.Columns.Clear()
			'	For iIndex = 0 To moCurrentDataView.
			Me.dgvMain.DataSource = moCurrentDataView
			' DMCommon.Debug.MsgBox("01_431a", CStr(Me.dgvMain.Columns.Count), CStr(miColumnSetIndex), CStr(moCurrentDataView.Count))
			zzSetDataGridColumns()

			If Me.tcbFilter.Items.Count = 0 Then
				Me.tcbFilter.Text = String.Empty
			Else
				Me.tcbFilter.SelectedIndex = 0
			End If
			zzMarkRows()
			zzSetEnable()
			'  DMCommon.Debug.MsgBox("12_364k", mtInitMapThemeData.MapThemeName)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTplnView - tcbData_SelectedIndexChanged")
		End Try
	End Sub
	Private Sub zzSetData()
		Dim oItemData As DMCommon.ItemData
		Dim oSelectedItem As System.Object
		oSelectedItem = Me.tcbData.SelectedItem

		If oSelectedItem Is Nothing Then
			moCurrentDataView = Nothing
			zzSetFilterItems(String.Empty)
		Else
			oItemData = DirectCast(oSelectedItem, DMCommon.ItemData)
			mbPolygons = False

			Select Case DMAcadExt.DMApp.AppID
				Case DMAcadExt.enApplications.Taba
					zzExecTaba(oItemData.ListIndex)
				Case DMAcadExt.enApplications.TopoMaster
					zzExecTopoMaster(oItemData.ListIndex)
				Case DMAcadExt.enApplications.Unidiv
					'System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_522????")
					zzExecUD(oItemData.ListIndex)
				Case DMAcadExt.enApplications.Ownership
					'	System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_523!!!")
					zzExecOwnership(oItemData.ListIndex)
				Case DMAcadExt.enApplications.BN
					'	System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_523!!!")
					zzDispBNPgons()
			End Select
			msCurrentCaption = oItemData.ListDispData

		End If
	End Sub
	Private Sub zzExecTaba(ByVal iIndex As Integer)
		msAreaFieldName = String.Empty
		miMapTheme = DMAcadExt.enMapTheme.Undefined
		mbMarked = False

		Select Case iIndex
			Case 0
				miTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
				zzDispParcels()
				zzSetFilterItems("0,1,3,4,5")
				miPaintMethod = enPaintMethod.ParcelExcept
				mbMarked = True
			Case 1
				' DMCommon.Debug.MsgBox("12_377c")
				miTopoPurpose = DMAcadExt.enTopoPurpose.Approved
				zzDispLots(DMAcadExt.enTopoPurpose.Approved)
				zzSetFilterItems("0,1,2,5")
				miPaintMethod = enPaintMethod.Landuse
			Case 2
				miTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				zzDispLots(DMAcadExt.enTopoPurpose.Proposed)
				zzSetFilterItems("0,1,2,5")
				miPaintMethod = enPaintMethod.Landuse
			Case 3
				miTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
				zzDispBlocks()
				zzSetFilterItems(String.Empty)
			Case 4
				miTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
				zzDispLotsContent()
				zzSetFilterItems(String.Empty)

			Case 5
				miTopoPurpose = DMAcadExt.enTopoPurpose.Approved
				zzDispLanduse(DMAcadExt.enTopoPurpose.Approved)
				zzSetFilterItems(String.Empty)
			Case 6
				miTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				zzDispLanduse(DMAcadExt.enTopoPurpose.Proposed)
				zzSetFilterItems(String.Empty)

			Case 7
				miTopoPurpose = DMAcadExt.enTopoPurpose.Approved
				zzDispLandusePgons(miTopoPurpose)
				zzSetFilterItems(String.Empty)
			Case 8
				miTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				zzDispLandusePgons(miTopoPurpose)
				zzSetFilterItems(String.Empty)
			Case 9 To 14
				miTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
				mbPolygons = True
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = CType(iIndex - 9, DMAcadExt.enOverlayIndex)
				miTopoPurpose = TopoManager.TPlanGraph.UnionPgonArea.GetTopoPurpose(iOverlayIndex)
				miPaintMethod = enPaintMethod.BlockParcel
				zzDispPolygons(iOverlayIndex)
				zzSetFilterItems("0,1")
			Case 15 To 20
				'	MessageBox.Show(CStr(iIndex), "05_408")
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = CType(iIndex - 15, DMAcadExt.enOverlayIndex)
				zzDispOverlayGroup(iOverlayIndex)
				zzSetFilterItems("0,1")
			Case 21
				zzDispExpros()
				zzSetFilterItems("0,1,2,5")

			Case 22
				zzDispExproOverlay()
				zzSetFilterItems("0,1")

			Case 23
				zzDispMerhavs()
				zzSetFilterItems("0,1,2,5")

			Case 24
				zzDispMerhavOverlay()
				zzSetFilterItems("0,1")
			Case 25
				zzDispZones()
		End Select
		zzSetPaintScale()
	End Sub
	Private Sub zzExecTopoMaster(ByVal iIndex As Integer)
		Select Case iIndex
			Case 41
				zzDispBamashPgons()
		End Select
	End Sub
	Private Sub zzExecUD(ByVal iIndex As Integer)
		Select Case iIndex
			Case 51
				zzDispUD_Parcels()
			Case 52
				zzDispUD_AdjoiningParcels()
		End Select
	End Sub
	Private Sub zzExecOwnership(ByVal iIndex As Integer)
		'	System.Windows.Forms.MessageBox.Show(iIndex.ToString(), "01_525")
		Select Case iIndex
			Case 61
				zzDispOwnerPgons()
			Case 62
				zzDispOwners()
		End Select
	End Sub
	Private Sub zzSetEnable()
		Try
			Dim bValue As Boolean = mbTopologyExists
			'		If miCurrentTopoDefID.ID = 0 Then
			'bValue = False
			'		Else
			'		bValue = True
			'		End If
			Me.cmdFindPgon.Enabled = bValue
			Me.cmdFindPgonLoop.Enabled = bValue
			'	System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetEnable")
			Me.tbbZoomPgon.Enabled = bValue

			Me.tbbPolygonNext.Enabled = bValue
			Me.tbbPolygonPrevious.Enabled = bValue
			Me.tsbFilterBySelect.Visible = mbPolygons
			Me.tsbRemoveFilter.Visible = mbPolygons

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzSetEnable")
		End Try
	End Sub
	Private Sub zzDispExproOverlay()
		moCurrentDataView = TopoManager.TPlanGraph.TplnOverlayPgon.MainView
		miMapTheme = DMAcadExt.enMapTheme.ParcelsXExproXLots
		miColumnSetIndex = 14
		msInPlanFilter = "Isnull(" & TplnOverlayPgon.msExproTypeIDFieldName & ",0) <>0"
		'"Isnull(Col1,'Null Column') = 'Null Column'"
		mbPolygons = True
		mbTopologyExists = True
		miZoomType = enZoomType.Polygon
	End Sub
	Private Sub zzDispMerhavOverlay()
		moCurrentDataView = TopoManager.TPlanGraph.TplnMerhavOverlayPgon.MainView
		miMapTheme = DMAcadExt.enMapTheme.MerhavOverlay
		miColumnSetIndex = 15
		msInPlanFilter = ""
		'"Isnull(Col1,'Null Column') = 'Null Column'"
		mbPolygons = True
		mbTopologyExists = True
		miZoomType = enZoomType.Polygon
	End Sub
	Private Sub tcbFilter_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tcbFilter.SelectedIndexChanged
		Me.zzSetFilter()

	End Sub

	Private Sub frmTplnView_FormClosing(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		Dim iCloseReason As CloseReason = e.CloseReason

		Select Case iCloseReason
			Case CloseReason.UserClosing
				e.Cancel = True
				Me.Hide()
		End Select
	End Sub

	Private Sub tbbPolygonAppr_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Me.Focus()
		Me.cmdFindPgon.Focus()
		Common.MouseClick()
		'  Me.zzDispBlocks()
		'  Me.zzTestInsert()

	End Sub

	Private Sub cmdFindPgon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFindPgon.Click
		Me.zzFindPgonNew(True, False)

	End Sub

	Private Sub zzSetInitTable()
		Dim iInitIndex As Integer = -1

		Select Case mtInitMapThemeData.MapThemeID
			Case DMAcadExt.enMapTheme.Zone
				iInitIndex = 25
			Case DMAcadExt.enMapTheme.Parcels
				iInitIndex = 0
			Case DMAcadExt.enMapTheme.LotApproved
				iInitIndex = 1
			Case DMAcadExt.enMapTheme.Expropriation
				iInitIndex = 21
			Case DMAcadExt.enMapTheme.ParcelsXApproved
				iInitIndex = 13
			Case DMAcadExt.enMapTheme.BN
				iInitIndex = 0

		End Select
		If iInitIndex <> -1 Then
			Me.tcbData.SelectedIndex = iInitIndex
		End If

	End Sub

	Private Sub frmTplnView_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		'  DMCommon.ExcelLogD.Open()
	End Sub

	Private Sub frmTplnView_Resize(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Resize
		If midgvMainLocationY <> 0 Then
			Try
				Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - frmTplnView_Resize")
			End Try
		End If
		micmdFindPgonLoopLocationX = Me.cmdFindPgonLoop.Location.X
	End Sub


	Private Sub tbbInPlan_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tbbInPlan.CheckedChanged
		Me.zzSetFilter()
	End Sub

	Private Sub cmdFindPgonLoop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdFindPgonLoop.Click
		Me.zzFindPgonNew(False, True)
	End Sub



	Private Sub PaintScale_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oMenuItem As System.Windows.Forms.ToolStripMenuItem
		oMenuItem = DirectCast(oSender, System.Windows.Forms.ToolStripMenuItem)
		If moScaleMenuItemChecked IsNot Nothing Then
			moScaleMenuItemChecked.Checked = False
			moScaleMenuItemChecked = oMenuItem
		End If
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
	Private Sub zzZoomParcel(ByVal bHighlight As Boolean)
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oBoundingBox As DMAcadExt.TPlnBoundingBox

		Dim iParcelTopoID As Integer
		Try
			iParcelTopoID = Me.zzGetRowTopoID(oViewRow, TopoReader.msParcelTopoIDFldName)
			If iParcelTopoID <> 0 Then
				Dim oParcel As TplnParcel = TplnProject.GetParcel(iParcelTopoID)
				If oParcel IsNot Nothing Then
					If bHighlight Then
						DMAcadExt.AcadTransaction.Start()
						oParcel.Highlight()
						DMAcadExt.AcadTransaction.Terminate()
					End If
					oBoundingBox = oParcel.BoundingBox
					DMAcadExt.AcadDocument.Zoom(oBoundingBox)
				End If
			End If
		Catch oEx As Exception

		End Try









	End Sub
	Private Sub zzZoomLot(ByVal bHighlight As Boolean)
		Dim oViewRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oBoundingBox As DMAcadExt.TPlnBoundingBox

		Dim iLotTopoID As Integer
		Try
			iLotTopoID = Me.zzGetRowTopoID(oViewRow, TopoReader.msLotTopoIDFldName)
			If iLotTopoID <> 0 Then
				Dim oLot As TplnLot = TplnProject.GetLot(miTopoPurpose, iLotTopoID)
				If oLot IsNot Nothing Then
					If bHighlight Then
						DMAcadExt.AcadTransaction.Start()
						oLot.Highlight()
						DMAcadExt.AcadTransaction.Terminate()
					End If
					oBoundingBox = oLot.BoundingBox
					DMAcadExt.AcadDocument.Zoom(oBoundingBox)
				End If
			End If

		Catch oEx As Exception

		End Try

	End Sub


	Private Sub tsmiParcel_Click(oSender As System.Object, e As System.EventArgs) Handles tsmiParcel.Click

		Select Case miMapTheme
			Case DMAcadExt.enMapTheme.ParcelsXApproved
				zzZoomParcel(True)
		End Select

	End Sub

	Private Sub tsmiLot_Click(oSender As System.Object, e As System.EventArgs) Handles tsmiLot.Click
		Select Case miMapTheme
			Case DMAcadExt.enMapTheme.ParcelsXApproved
				zzZoomLot(True)
		End Select

	End Sub


	Private Sub frmTplnView_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		zzMarkRows()
	End Sub
	Private Sub zzMarkRows()
		Dim oDataRowView As DataRowView
		Dim oGridRow As DataGridViewRow
      If mbMarked AndAlso moCurrentDataView IsNot Nothing Then
         For iRowIndex As Integer = 0 To moCurrentDataView.Count - 1
            oDataRowView = moCurrentDataView.Item(iRowIndex)
            If Not IsDBNull(oDataRowView.Item(TplnParcel.msDeviationFieldName)) Then
               oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
               oGridRow.DefaultCellStyle = moMarkedCellStyle
            End If
         Next
      End If
   End Sub

	Private Sub tcbFilter_Click(oSender As System.Object, e As EventArgs) Handles tcbFilter.Click

	End Sub

	Private Sub tcbData_Click(oSender As System.Object, e As EventArgs) Handles tcbData.Click

	End Sub

	Private Sub tbbExcel_Click(oSender As System.Object, e As EventArgs) Handles tbbExcel.Click

	End Sub

	Private Sub tbbZoomPgon_Click(oSender As System.Object, e As EventArgs) Handles tbbZoomPgon.Click

	End Sub

	Private Sub dgvMain_AllowUserToAddRowsChanged(sender As Object, e As EventArgs) Handles dgvMain.AllowUserToAddRowsChanged

	End Sub

	Private Sub tbbInPlan_Click(sender As Object, e As EventArgs) Handles tbbInPlan.Click

	End Sub
	' bHasNoteType0 = False
	''''''''''''''''''''''''''''''''''''''

End Class
