Imports BamashNet
Imports TopoManager
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmBamashBlockRefs
	Private moMainDataView As System.Data.DataView
	Private moColorCellStyle(5) As DataGridViewCellStyle
	Private mbaVarColumnExists() As Boolean
	Private midgvMainLocationY As Integer
	Private moCurrentImageCell As DataGridViewCell
	Private msRowFilter As String
	Private mdicGridRows As Dictionary(Of Integer, Integer)
	Public Event SelectBamahPgon(ByRef oBamashPgon As BamashPolygon, ByRef oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel)
	Public Sub New(oMainDataView As System.Data.DataView)

		' This call is required by the designer.
		InitializeComponent()
		zzMyInitializeComponent()
		If oMainDataView IsNot Nothing Then
			moMainDataView = oMainDataView
			msRowFilter = moMainDataView.RowFilter
			' Add any initialization after the InitializeComponent() call.
			Me.dgvMain.DataSource = moMainDataView
		End If

	End Sub
	Private Sub zzMyInitializeComponent()
		mbaVarColumnExists = BamashPolygon.VarColumnExists

		If mbaVarColumnExists(enVarColumn.UserID) Then
			Me.ctxUserID.Visible = True
		End If

		Dim oItem As DMCommon.ItemData
		Dim tColor As DMAcadExt.DMColor



		Me.ccbPolygonColor.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbPolygonColor.DisplayMember = DMCommon.ItemData.DisplayMember
		For shIndex As Short = 1S To 6S
			tColor = New DMAcadExt.DMColor(shIndex)
			oItem = New DMCommon.ItemData(shIndex, tColor.HebColorName)
			Me.ccbPolygonColor.Items.Add(oItem)
		Next

		'Me.ccbPropType

		Dim iaPropertyTypes() As enPropertyTypes = DirectCast([Enum].GetValues(GetType(enPropertyTypes)), enPropertyTypes())
		Dim oItemData As DMCommon.ItemData
		Dim iPropertyType As enPropertyTypes
		Dim oaItems As New List(Of DMCommon.ItemData)
		'DMCommon.Debug.ExcelLog.SetEnumerable(0, "PropertyTypes()", iaPropertyTypes)
		Me.ccbPropType.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbPropType.DisplayMember = DMCommon.ItemData.DisplayMember
		For iIndex As Integer = 0 To iaPropertyTypes.GetUpperBound(0)
			iPropertyType = iaPropertyTypes(iIndex)
			Select Case iPropertyType
				Case enPropertyTypes.Default
				Case Else
					oItemData = New DMCommon.ItemData(iPropertyType, bmBamash.GetPropTypesName(iPropertyType))
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PropertyTypes", iaPropertyTypes(iIndex), bmBamash.GetPropTypesName(iaPropertyTypes(iIndex)))
					Me.ccbPropType.Items.Add(oItemData)
			End Select

		Next
		Me.cmbAprtDesc.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbAprtDesc.DisplayMember = DMCommon.ItemData.DisplayMember
		For iIndex As Integer = 1 To 14
			oItem = New DMCommon.ItemData(iIndex, iIndex.ToString() & "-" & GetApartDescr(iIndex))

			oaItems.Add(oItem)

		Next
		Me.cmbAprtDesc.DataSource = oaItems
		If False Then
			For iIndex As Integer = 0 To 13
				Me.ccbAprtDescNum.Items.Add(iIndex.ToString() & "-" & GetApartDescr(iIndex))
			Next
		End If

		Me.ccbAprtDescNum.ValueMember = DMCommon.ItemData.ValueMember
		Me.ccbAprtDescNum.DisplayMember = DMCommon.ItemData.DisplayMember

		oItem = New DMCommon.ItemData(99, GetApartDescr(99))
		'	Me.ccbAprtDescNum.Items.Add(oItem)
		oaItems.Add(oItem)
		Me.ccbAprtDescNum.DataSource = oaItems
		Dim oDefaultCellStyle As DataGridViewCellStyle
		Dim oaColors() As System.Drawing.Color = {System.Drawing.Color.Red, System.Drawing.Color.Yellow, System.Drawing.Color.Green, System.Drawing.Color.Cyan, System.Drawing.Color.Blue, System.Drawing.Color.Magenta}

		oDefaultCellStyle = Me.dgvMain.DefaultCellStyle
		For iIndex As Integer = 0 To 5
			moColorCellStyle(iIndex) = Me.dgvMain.DefaultCellStyle.Clone
			moColorCellStyle(iIndex).BackColor = oaColors(iIndex)
		Next

		Me.dgvMain.AutoGenerateColumns = False
		Me.dgvMain.RowHeadersWidth = 23
	End Sub
	Private Sub zzFillRowDictionary()
		Dim oDataRowView As DataRowView
		Dim iTopoID As Integer
		If mdicGridRows Is Nothing AndAlso moMainDataView IsNot Nothing Then
			mdicGridRows = New Dictionary(Of Integer, Integer)()
			For iRowIndex As Integer = 0 To moMainDataView.Count - 1
				oDataRowView = moMainDataView.Item(iRowIndex)
				iTopoID = DMCommon.Functions.CIntN(oDataRowView.Item(TopoReader.msTopoIDFldName))
				If iTopoID <> 0 Then
					mdicGridRows.Add(iTopoID, iRowIndex)
				End If

			Next
		End If

	End Sub
	Private Sub zzGridFormatting()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRowView As Data.DataRowView
		Dim iPrevPropKey As Integer = 0
		Dim iPropKey As Integer
		Dim iColor As Integer
		Dim bMain As Boolean
		Dim iPropType As enPropertyTypes
		Dim oGridCell As DataGridViewCell
		For iRowIndex As Integer = 0 To moMainDataView.Count - 1
			oDataRowView = moMainDataView.Item(iRowIndex)
			iPropKey = DMCommon.Functions.CIntN(oDataRowView.Item(BamashPolygon.PropKeyFieldName))
			iPropType = DMCommon.Functions.CEnumN(Of enPropertyTypes)(oDataRowView.Item(BamashPolygon.PropTypeFieldName), enPropertyTypes.ZeroType)
			bMain = DMCommon.Functions.CBoolN(oDataRowView.Item(BamashPolygon.MainDataFieldName))
			If bMain Then
				iColor = DMCommon.Functions.CIntN(oDataRowView.Item(BamashPolygon.PolygonColorFieldName))
			End If

			If iPropKey <> iPrevPropKey Then
				'oGridRow = Me.dgvMain.Rows.Item(iRowIndex - 1)
				If iRowIndex <> 0 Then
					oGridRow.DividerHeight = 3
				End If

				iPrevPropKey = iPropKey
			End If
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
			'"PolygonColor"
			If iPropType = enPropertyTypes.SubShareType Then
				'iColor = DMCommon.Functions.CIntN()
				If iRowIndex = moMainDataView.Count - 1 Then
					'moCurrentImageCell = oGridRow.Cells.Item("ccbPolygonColor")
					moCurrentImageCell = oGridRow.Cells.Item("ctxPurchaser")


					Dim oBM As Bitmap = New Bitmap(moCurrentImageCell.Size.Width, moCurrentImageCell.Size.Height)



					'  moCurrentImageCell

					DMCommon.Debug.ExcelLog.SetNextValue(0, "CurrentImageCell", moCurrentImageCell.Size.Width, moCurrentImageCell.Size.Height)

					Me.pcbImage.DrawToBitmap(oBM, New Rectangle(0, 0, moCurrentImageCell.Size.Width, moCurrentImageCell.Size.Height))
					moCurrentImageCell.Value = oBM
				Else
					oGridCell = oGridRow.Cells.Item("ccbPolygonColor")

					oGridCell.Style = moColorCellStyle(0)
				End If
			Else
				If iColor <> 0 Then
					oGridCell = oGridRow.Cells.Item("ccbPolygonColor")
					'oGridCell = oGridRow.Cells.Item("ctxAprtDesc2")
					oGridCell.Style = moColorCellStyle(iColor - 1)
				End If
			End If
		Next
	End Sub
	Private Sub zzUpdateKeys()
		Dim oGridRow As DataGridViewRow = Nothing
		Dim oDataRowView As Data.DataRowView
		Dim iPropKey As Integer
		Dim iSubPropNum As Integer

		Dim iPropType As enPropertyTypes
		Dim sPropertyID As String
		Dim sSubParcelNo As String

		Dim bMainData As Boolean
		Dim oTable As System.Data.DataTable = moMainDataView.Table
		oTable.AcceptChanges()

		For iRowIndex As Integer = 0 To Me.dgvMain.RowCount - 1
			oDataRowView = moMainDataView.Item(iRowIndex)
			sPropertyID = DMCommon.Functions.CStrN(oDataRowView.Item(BamashPolygon.PropIDFieldName))
			iPropKey = DMCommon.Functions.CIntN(oDataRowView.Item(BamashPolygon.PropKeyFieldName))
			iPropType = DMCommon.Functions.CEnumN(Of enPropertyTypes)(oDataRowView.Item(BamashPolygon.PropTypeFieldName), enPropertyTypes.ZeroType)
			bMainData = DMCommon.Functions.CBoolN(oDataRowView.Item(BamashPolygon.MainDataFieldName))
			sSubParcelNo = DMCommon.Functions.CStrN(oDataRowView.Item(BamashPolygon.SubParcelNoFieldName))
			iSubPropNum = bmBamash.Heb2Num(sSubParcelNo)
			oDataRowView.BeginEdit()
			oDataRowView.Item(BamashPolygon.PropKeyFieldName) = BamashPolygon.GetPropKey(sPropertyID, iPropType)
			oDataRowView.Item(BamashPolygon.PgonKeyFieldName) = BamashPolygon.GetPgonKey(bMainData, iPropType, iSubPropNum)
		Next
		moMainDataView.Table.AcceptChanges()
	End Sub

	Private Sub dgvMain_DataError(oSender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
		e.Cancel = True
	End Sub


	Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
		Dim oGridColumn As DataGridViewColumn = Me.dgvMain.Columns.Item(e.ColumnIndex)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(e.RowIndex)
		Dim oDataRowView As DataRowView = moMainDataView.Item(e.RowIndex)
		Dim oGridCell As DataGridViewCell
		Dim iColor As Integer
		Dim bIsBlockAttribute As Boolean = True
		Dim sMainPropID As String
		Select Case oGridColumn.DataPropertyName
			Case "PolygonColor"
				oGridCell = oGridRow.Cells.Item(e.ColumnIndex)
				iColor = DMCommon.Functions.CIntN(oGridCell.Value)
				oGridCell.Style = moColorCellStyle(iColor - 1)
				sMainPropID = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPropID").Value)
				'''''''''''''''''''''''''''''
				Dim iRowIndex As Integer = e.RowIndex
				Dim iPropType As enPropertyTypes
				Dim sPropID As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPropID").Value)
				Do
					iRowIndex += 1
					If iRowIndex >= Me.dgvMain.Rows.Count Then
						Exit Do
					Else
						oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
						oDataRowView = moMainDataView.Item(iRowIndex)
						sPropID = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPropID").Value)
						iPropType = DMCommon.Functions.CEnumN(Of enPropertyTypes)(oDataRowView.Item(BamashPolygon.PropTypeFieldName), enPropertyTypes.ZeroType)
						If sPropID = sMainPropID Then
							If iPropType <> enPropertyTypes.SubShareType Then
								If iColor <> 0 Then
									oDataRowView.Item("PolygonColor") = iColor
									oGridCell = oGridRow.Cells.Item("ccbPolygonColor")
									oGridCell.Style = moColorCellStyle(iColor - 1)
								End If
							End If
						Else
							Exit Do
						End If

					End If
				Loop


''''''''''''''''''''''''''''''''''''''''
			Case "BldFloor"
				Dim sBldFloor As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxBldFloor").Value)
				oGridCell = oGridRow.Cells.Item("ctxBldFloorDesc")
				If String.IsNullOrEmpty(sBldFloor) Then
					oGridCell.Value = String.Empty
				Else
					oGridCell.Value = GetFloorDescr2015(0, "BldfloorDesc", sBldFloor)
				End If

			'	oDataRowView.Item("BldFloorDesc") = GetFloorDescr2015(0, "BldfloorDesc", sBldFloor)

			'	DMCommon.Debug.MsgBox("oGridCell.Value", sBldFloor, oGridCell.Value, oDataRowView.Item("BldFloorDesc"), GetFloorDescr2015(0, "BldfloorDesc", sBldFloor))
			Case "PropType", "PropID", "SubParcelNo"
				'Dim iPropType1 As Integer = DMCommon.Functions.CIntN(oGridRow.Cells.Item("ccbPropType").Value)
				Dim iPropType As enPropertyTypes = DMCommon.Functions.CEnumN(Of enPropertyTypes)(oGridRow.Cells.Item("ccbPropType").Value, enPropertyTypes.ZeroType)
				Dim sPropID As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPropID").Value)
				Dim sSubParcelNo As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxSubParcelNo").Value)
				Dim iSubPropNum As Integer
				If String.IsNullOrEmpty(sSubParcelNo) Then
					iSubPropNum = 0
				ElseIf Integer.TryParse(sSubParcelNo, iSubPropNum) Then
					oGridCell = oGridRow.Cells.Item("ctxSubParcelNo")
					oGridCell.Value = DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
				Else
					iSubPropNum = bmBamash.Heb2Num(sSubParcelNo)
				End If
				'DMCommon.Debug.MsgBox("PropType,PropIDSubParcelNo", iPropType, sPropID, iSubPropNum)
				oGridCell = oGridRow.Cells.Item("ctxCaption")
				oGridCell.Value = BamashPolygon.GetCaption(iPropType, sPropID, iSubPropNum)

			Case "Changed"
				bIsBlockAttribute = False

		End Select
		If bIsBlockAttribute Then
			oGridRow.Cells.Item("cchChanged").Value = True
			dgvMain.CausesValidation = True
		End If

	End Sub

	Private Sub zzAfterCellUpdate(iRowIndex As Integer, iColumnIndex As Integer)
		Dim oGridColumn As DataGridViewColumn = Me.dgvMain.Columns.Item(iColumnIndex)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)
		Dim oDataRowView As DataRowView = moMainDataView.Item(iRowIndex)
		Dim oGridCell As DataGridViewCell
		Dim iColor As Integer
		Dim bIsBlockAttribute As Boolean = True
		Select Case oGridColumn.DataPropertyName
			Case "PolygonColor"
				oGridCell = oGridRow.Cells.Item(iColumnIndex)
				iColor = DMCommon.Functions.CIntN(oGridCell.Value)
				'oGridCell = oGridRow.Cells.Item(e.ColumnIndex) 'temp
				oGridCell.Style = moColorCellStyle(iColor - 1)

			Case "BldFloor"
				Dim sBldFloor As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxBldFloor").Value)
				oGridCell = oGridRow.Cells.Item("ctxBldFloorDesc")
				If String.IsNullOrEmpty(sBldFloor) Then
					oGridCell.Value = String.Empty
				Else
					oGridCell.Value = GetFloorDescr2015(0, "BldfloorDesc", sBldFloor)
				End If

			'	oDataRowView.Item("BldFloorDesc") = GetFloorDescr2015(0, "BldfloorDesc", sBldFloor)

			'	DMCommon.Debug.MsgBox("oGridCell.Value", sBldFloor, oGridCell.Value, oDataRowView.Item("BldFloorDesc"), GetFloorDescr2015(0, "BldfloorDesc", sBldFloor))
			Case "PropType", "PropID", "SubParcelNo"
				'Dim iPropType1 As Integer = DMCommon.Functions.CIntN(oGridRow.Cells.Item("ccbPropType").Value)
				Dim iPropType As enPropertyTypes = DMCommon.Functions.CEnumN(Of enPropertyTypes)(oGridRow.Cells.Item("ccbPropType").Value, enPropertyTypes.ZeroType)
				Dim sPropID As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxPropID").Value)
				Dim sSubParcelNo As String = DMCommon.Functions.CStrN(oGridRow.Cells.Item("ctxSubParcelNo").Value)
				Dim iSubPropNum As Integer
				If String.IsNullOrEmpty(sSubParcelNo) Then
					iSubPropNum = 0
				ElseIf Integer.TryParse(sSubParcelNo, iSubPropNum) Then
					oGridCell = oGridRow.Cells.Item("ctxSubParcelNo")
					oGridCell.Value = DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
				Else
					iSubPropNum = bmBamash.Heb2Num(sSubParcelNo)
				End If
				'DMCommon.Debug.MsgBox("PropType,PropIDSubParcelNo", iPropType, sPropID, iSubPropNum)
				oGridCell = oGridRow.Cells.Item("ctxCaption")
				oGridCell.Value = BamashPolygon.GetCaption(iPropType, sPropID, iSubPropNum)

			Case "Changed"
				bIsBlockAttribute = False

		End Select
		If bIsBlockAttribute Then
			oGridRow.Cells.Item("cchChanged").Value = True
			dgvMain.CausesValidation = True
		End If

	End Sub
	Private Sub zzPaintPictureBox(ByVal oGraphics As Graphics)
		Const iBottom As Integer = 3
		Const iHeight As Integer = 16
		Const iTextWidth As Integer = 20

		If moCurrentImageCell IsNot Nothing Then  ' mbEventsEnabled AndAlso
			Dim iDividerWidth As Integer = dgvMain.Columns.Item(moCurrentImageCell.ColumnIndex).DividerWidth
			Dim iCellClientWidth As Integer = moCurrentImageCell.Size.Width - iDividerWidth

			Dim oBrush As Brush
			Dim oBrushText As Brush = New SolidBrush(Color.Black)
			Dim oFont As System.Drawing.Font = Me.Font
			'	Dim sCellText As String = FormatNumber(mtCurrentGanttDay.Duration, 1)
			Dim iStart As Integer '= Convert.ToInt32(mtCurrentGanttDay.Start / mtCurrentGanttDay.DayLenth * iCellClientWidth)
			Dim iLenth As Integer '= Convert.ToInt32(mtCurrentGanttDay.Duration / mtCurrentGanttDay.DayLenth * iCellClientWidth)
			Dim iStartRect As Integer = iCellClientWidth - iStart - iLenth
			Dim iStartText As Integer
			Dim iLeftMargin As Integer

			If True Then
				oBrush = New SolidBrush(Color.Red)
			Else
				oBrush = New SolidBrush(Color.LightGreen)
			End If
			If iDividerWidth > 1 Then
				iLeftMargin = (iDividerWidth - 1) \ 2
			Else
				iLeftMargin = 0
			End If
			If iStartRect < iLeftMargin Then
				iStartRect = iLeftMargin
			End If
			iLenth = 12
			DMCommon.Debug.ExcelLog.SetNextValue(0, "PaintPictB", iStartRect, moCurrentImageCell.Size.Height - iBottom - iHeight, iLenth, iHeight)
			oGraphics.FillRectangle(oBrush, iStartRect, moCurrentImageCell.Size.Height - iBottom - iHeight, iLenth, iHeight)
			'If mtCurrentGanttDay.Duration < mtCurrentGanttDay.DayLenth Then

			If iStartRect >= iCellClientWidth - iTextWidth Then
				iStartText = iCellClientWidth - iTextWidth
			Else
				iStartText = iStartRect + iLenth - iTextWidth

			End If
			If iStartText < iLeftMargin Then
				iStartText = iLeftMargin
			End If

			'oGraphics.DrawString(sCellText, oFont, oBrushText, iStartText, moCurrentImageCell.Size.Height - iBottom - iHeight)
			'End If
		End If
		'  mtCurrentGanttDay()
		'  moCurrentImageCell
	End Sub
	Private Sub pcbImage_Paint(oSender As System.Object, e As PaintEventArgs) Handles pcbImage.Paint
		zzPaintPictureBox(e.Graphics)
	End Sub


	Private Sub frmBamashBlockRefs_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
		If midgvMainLocationY <> 0 Then
			Try
				Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditBlockRef_Resize")
			End Try
		Else
			midgvMainLocationY = Me.dgvMain.Location.Y
		End If
	End Sub

	Private Sub frmBamashBlockRefs_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			Me.Size = New System.Drawing.Size(1200, 400)
			Me.WindowState = FormWindowState.Normal
		End If
		zzGridFormatting()
	End Sub


	Private Sub cmdZoom_Click(oSender As System.Object, e As EventArgs) Handles cmdZoom.Click
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim iGridRowIndex As Integer = oGridRow.Index
		Dim oDataRowView As DataRowView
		Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
		If oGridRow IsNot Nothing Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			Dim oBamashPolygon As BamashPolygon = zzGetBamashPolygon(oDataRowView)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			oBamashPolygon.Highlight()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			oBoundingBox = oBamashPolygon.BoundingBox
			DMAcadExt.AcadDocument.Zoom(oBoundingBox)
		End If
	End Sub
	Private Function zzGetBamashPolygon(oDataRowView As DataRowView) As BamashPolygon
		Dim iTopoID As Integer = DMCommon.Functions.CIntN(oDataRowView.Item(TopoReader.msTopoIDFldName))
		Return BamashNet.bmBamash.GetBamashPgon(iTopoID)
	End Function

	Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
		Dim oGridRow As DataGridViewRow
		Dim oDataRowView As DataRowView
		Me.Cursor = Cursors.WaitCursor
		'Dim saValues(miLastAttribColIndex - miFirstAttribColIndex) As String
		Dim saAddValues() As String = Nothing



		Dim oBamashPolygon As BamashPolygon

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		For iRowIndex As Integer = 0 To Me.dgvMain.RowCount - 1
			Try
				oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
				oDataRowView = moMainDataView.Item(iRowIndex)
				If True Or DMCommon.Functions.CBoolN(oDataRowView.Item("Changed")) Then
					oBamashPolygon = zzGetBamashPolygon(oDataRowView)
					oBamashPolygon.GetDataFromTable(oDataRowView.Row)
					'lObjID = DirectCast(oGridRow.Cells.Item("ctxObjectID").Value, Int64)
					'tAcObjID = New ObjectId(New System.IntPtr(lObjID))
					'tAcObjID = DirectCast(oGridRow.Cells.Item("ctxObjectID").Value, ObjectId)

					oBamashPolygon.UpdateAllData()

					oDataRowView.Item("Changed") = False
					Dim bAddRes As Boolean = True




				End If
			Catch oEx As Exception

			End Try
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub cmdSelectByPick_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectByPick.Click
		'	Dim iSelectPointStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oBamashPgon As BamashPolygon = Nothing
		Dim iRowIndex As Integer = 0
		Dim oGridRow As DataGridViewRow
		Dim oBamashTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Bamash", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oBamashTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			zzFillRowDictionary()
			RaiseEvent SelectBamahPgon(oBamashPgon, oBamashTopology)
			If oBamashPgon IsNot Nothing AndAlso mdicGridRows.TryGetValue(oBamashPgon.TopoID, iRowIndex) Then
				oGridRow = Me.dgvMain.Rows.Item(iRowIndex)

				Me.dgvMain.CurrentCell = oGridRow.Cells.Item(1)

			End If
			Me.Visible = True
		End If

	End Sub

	Private Sub chkAllBlocks_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkAllBlocks.CheckedChanged
		Dim sRowFilter As String = moMainDataView.RowFilter
		If Me.chkAllBlocks.Checked AndAlso Not String.IsNullOrEmpty(sRowFilter) Then
			moMainDataView.RowFilter = String.Empty
			zzGridFormatting()
		ElseIf Not Me.chkAllBlocks.Checked AndAlso String.IsNullOrEmpty(sRowFilter) Then
			moMainDataView.RowFilter = msRowFilter
			zzGridFormatting()
		End If

	End Sub

	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Sub cmdSetValue_Click(oSender As System.Object, e As EventArgs) Handles cmdSetValue.Click
		zzInputNewValue()
	End Sub
	Private Sub cmdPlusValue_Click(oSender As System.Object, e As EventArgs) Handles cmdPlusValue.Click
		zzAdditionValue()
	End Sub
	Private Sub zzInputNewValue()
		Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
		For Each oCell As DataGridViewCell In oSelCells
			oCell.Value = Me.txtValue.Text
			zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
		Next
	End Sub
	Private Sub zzAdditionValue()
		Dim iAddendum As Integer
		Dim sCellValue As String
		Dim iCellValue As Integer
		Dim oGridColumn As DataGridViewColumn
		If Integer.TryParse(Me.txtAddendum.Text, iAddendum) Then
			Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
			For Each oCell As DataGridViewCell In oSelCells
				oGridColumn = Me.dgvMain.Columns.Item(oCell.ColumnIndex)
				Select Case oGridColumn.DataPropertyName
					Case "PropID" ', "SubParcelNo"
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzAdditionValue", oCell.Value, oCell.Value.GetType())
						sCellValue = DMCommon.Functions.CStrN(oCell.Value)
						If Integer.TryParse(sCellValue, iCellValue) Then
							oCell.Value = iCellValue + iAddendum
							zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
						End If
					Case "SubParcelNo"
						sCellValue = DMCommon.Functions.CStrN(oCell.Value)
						If Not String.IsNullOrEmpty(sCellValue) Then
							iCellValue = bmBamash.Heb2Num(sCellValue)
							If iCellValue <> 0 Then
								oCell.Value = iCellValue + iAddendum
								zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
							End If
						End If
				End Select
			Next
			'	
		End If
	End Sub

	Private Sub zzSerialValue()
		Dim iCurrentValue As Integer?
		Dim sCellValue As String
		Dim iCellValue As Integer

		Dim oGridColumn As DataGridViewColumn
		Dim iValue As Integer
		If Integer.TryParse(Me.txtAddendum.Text, iValue) Then
			iCurrentValue = New Integer?(iValue)
		End If

		Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
		For Each oCell As DataGridViewCell In oSelCells
			oGridColumn = Me.dgvMain.Columns.Item(oCell.ColumnIndex)

			Select Case oGridColumn.DataPropertyName
				Case "PropID" ', "SubParcelNo"
					If iCurrentValue.HasValue Then
						oCell.Value = iCurrentValue
						iCurrentValue += 1
						zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
					Else
						sCellValue = DMCommon.Functions.CStrN(oCell.Value)

						If Integer.TryParse(sCellValue, iCellValue) Then

							iCurrentValue = New Integer?(iCellValue)
						End If

					End If

				Case "SubParcelNo"


					If iCurrentValue.HasValue Then
						oCell.Value = iCurrentValue
						iCurrentValue += 1
						zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
					Else
						sCellValue = DMCommon.Functions.CStrN(oCell.Value)
						If Not String.IsNullOrEmpty(sCellValue) Then
							iCellValue = bmBamash.Heb2Num(sCellValue)
							iCurrentValue = New Integer?(iCellValue)
						End If

					End If
			End Select
		Next
	End Sub
	Private Sub cmdSetValue_Click1(oSender As System.Object, e As EventArgs) 'Handles cmdSetValue.Click
		Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
		Dim oGridRow As DataGridViewRow
		Dim oDataRowView As System.Data.DataRowView
		Dim oCheckCell As DataGridViewCheckBoxCell
		'  Dim iValue As System.Windows.Forms.CheckState
		Dim bValue As Boolean
		Dim bChanged As Boolean

		For Each oCell As DataGridViewCell In oSelCells
			oGridRow = Me.dgvMain.Rows.Item(oCell.RowIndex)
			oDataRowView = moMainDataView.Item(oCell.RowIndex)
			bChanged = DirectCast(oDataRowView.Item("Changed"), Boolean)
			oCheckCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)

			'   MessageBox.Show(oCheckCell.Value.GetType().ToString() & vbCrLf & oCheckCell.Value.ToString(), "04_465")
			If Not bChanged Then
				oDataRowView.Item("Changed") = True
			End If
			bValue = DirectCast(oCheckCell.Value, Boolean)
			If Not bValue Then
				oCheckCell.Value = CheckState.Checked
			End If
			oCell.Value = Me.txtValue.Text

		Next

	End Sub

	Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
		Dim oGridColumn As DataGridViewColumn = Me.dgvMain.Columns.Item(e.ColumnIndex)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(e.RowIndex)
		Dim oDataRowView As DataRowView = moMainDataView.Item(e.RowIndex)

		Dim bMain As Boolean
		Dim bIsBlockAttribute As Boolean = True
		Select Case oGridColumn.DataPropertyName
			Case "PolygonColor"
				'oGridCell = oGridRow.Cells.Item(e.ColumnIndex)
				'	iColor = DMCommon.Functions.CIntN(oGridCell.Value)
				bMain = DMCommon.Functions.CBoolN(oDataRowView.Item(BamashPolygon.MainDataFieldName))
				If Not bMain Then
					e.Cancel = True
				End If

				'
		End Select

	End Sub


	Private Sub cmdSetApartDesc_Click(oSender As System.Object, e As EventArgs) Handles cmdSetApartDesc.Click
		Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
		Dim oComboCell As DataGridViewComboBoxCell
		If Me.cmbAprtDesc.SelectedValue IsNot Nothing Then
			Dim iSelectedValue As Integer = DirectCast(Me.cmbAprtDesc.SelectedValue, Integer)
			For Each oCell As DataGridViewCell In oSelCells
				oComboCell = TryCast(oCell, DataGridViewComboBoxCell)
				If Me.dgvMain.Columns.Item(oCell.ColumnIndex).DataPropertyName = "AprtDescNum" Then
					oCell.Value = iSelectedValue
					zzAfterCellUpdate(oCell.RowIndex, oCell.ColumnIndex)
				End If

				'	If oComboCell IsNot Nothing Then
				'oComboCell.Value = iSelectedValue
				'End If

			Next
		End If


	End Sub

	Private Sub cmbAprtDesc_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbAprtDesc.SelectedIndexChanged

	End Sub

	Private Sub Button1_Click(oSender As System.Object, e As EventArgs)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim iGridRowIndex As Integer = oGridRow.Index
		Dim oDataRowView As DataRowView = moMainDataView.Item(iGridRowIndex)
		oDataRowView.Item("AprtDescNum") = 5
	End Sub

	Private Sub cmdUpdateKeys_Click(oSender As System.Object, e As EventArgs) Handles cmdUpdateKeys.Click
		zzUpdateKeys()
		zzGridFormatting()
	End Sub

End Class