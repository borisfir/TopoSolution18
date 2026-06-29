Option Explicit On
Option Strict On
Public Enum enColorSetType
	All = 1
	Standard
	Local
	Named
End Enum
Public Class frmSelectLanduse
	Private Enum enMarkType
		CheckBox
		Selection
	End Enum
	Private Enum enItemStatus
		Outside
		Unchanged
		Added
		Deleted
		Modified
	End Enum
	Private mtColorScheme As DMAcadExt.ColorScheme
	Private moCurrentViewMenuItem As System.Windows.Forms.ToolStripMenuItem
	Private miColorSetType As enColorSetType
	Private mdicLanduseIDs As Dictionary(Of Integer, Integer)
	Private miColorSetID As Integer
	Private miColorSetIndex As Integer
	Private moSelectedItem As DMCommon.ItemData	'System.Object	'
	Private mbPermitLanduseView As Boolean = True
	Private miListViewTop As Integer
	Private miTopoPurpose As DMAcadExt.enTopoPurpose
	Private msLanduseList As String


	Public Event SetColorSet(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iColorSetID As Integer, ByVal sLanduseList As String)
	Private Sub zzMyInitializeComponent()
		Me.cmbColorSets.ComboBox.AutoCompleteMode = AutoCompleteMode.Suggest
		Dim iTest As Integer = 0
		mdicLanduseIDs = New Dictionary(Of Integer, Integer)


		zzSetView(Me.tsiLargeIcons, View.LargeIcon)
		Me.tsiCheckBox.Checked = True
		Me.tsiLargeIcons.Checked = True
		Const sStandard As String = "מבא""ת"
		Const sApprText As String = "מצב קיים"
		Const sPropText As String = "מצב מוצע"

		 
		Me.tsiStandardOnly.Text = sStandard & " בלבד"
		Me.tsiAppr.Text = sApprText
		Me.tsiProp.Text = sPropText
		If miTopoPurpose = TPlanGraph.enTopoPurpose.Approved Then
			Me.tsiAppr.Checked = True
		ElseIf miTopoPurpose = TPlanGraph.enTopoPurpose.Proposed Then
			Me.tsiProp.Checked = True
		End If

		'	msLanduseList = sLanduseList

		TPlanGraph.TplnProject.FillColorSets(Me.cmbColorSets.ComboBox, miColorSetID)
		If miColorSetID = 0 Then
			Try
				Me.cmbColorSets.SelectedIndex = 1
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmSelect - zzMyInitializeComponent_1")
			End Try
		End If
		miListViewTop = Me.lvwLanduses.Top

	End Sub
	 
	Private Sub zzFillColorSetsAAA()
		Const sAllText As String = "כל היעודים"
		Const sStandard As String = "מבא""ת"
		Const sLocal As String = "לוקלי"
		Const sComText As String = "SELECT ID,Name FROM ColorSetList ORDER BY ID"
		Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
		Dim oItemData As DMCommon.ItemData
		With Me.cmbColorSets.ComboBox
			.ValueMember = DMCommon.ItemData.ValueMember
			.DisplayMember = DMCommon.ItemData.DisplayMember
			.Items.Add(New DMCommon.ItemData(CType(enColorSetType.All, Integer), sAllText))
			.Items.Add(New DMCommon.ItemData(CType(enColorSetType.Standard, Integer), sStandard))
			.Items.Add(New DMCommon.ItemData(CType(enColorSetType.Local, Integer), sLocal))
			If oDataReader IsNot Nothing Then
				While oDataReader.Read
					oItemData = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
					.Items.Add(oItemData)
				End While
				oDataReader.Close()
			End If
		End With
	End Sub
	Private Sub zzPaint(ByVal oGraphics As Graphics)
		If oGraphics Is Nothing Then
			oGraphics = Me.pcbImage.CreateGraphics
		End If
		Dim oPaint As PaintBox = New PaintBox(oGraphics, Me.pcbImage.ClientSize)
		oPaint.ColorScheme = mtColorScheme
		oPaint.Draw()
	End Sub
	Private Sub zzFillLanduses(ByVal iColorSetType As enColorSetType)
		'	TPlanGraph.TplnProject.FillLandusesLV(Me.lvwLanduses, DMAcadExt.ColorScheme.NameDelim, True)
		Dim sLanduseTableName As String = "Landuses_" & CStr(TPlanGraph.TplnProject.LandusesFormatID) & "F"
		Dim sStandardCondition As String = " WHERE (" & sLanduseTableName & ".ID<=" & Convert.ToString(DMAcadExt.ColorScheme.MaxStandardID) & ")"

		Dim sWhere As String = String.Empty
		Dim sOrderBy As String = " ORDER BY ColorSchemes.ID"
		Dim sComText As String
		Dim bPartial As Boolean
		Dim bColorSetNamed As Boolean
		Select Case iColorSetType
			Case enColorSetType.All
				sWhere = String.Empty
				sComText = zzGetComText(sLanduseTableName) & sOrderBy
				bPartial = False
				bColorSetNamed = False
			Case enColorSetType.Standard
				sWhere = " WHERE (" & sLanduseTableName & ".ID<=" & Convert.ToString(DMAcadExt.ColorScheme.MaxStandardID) & ")"
				sComText = zzGetComText(sLanduseTableName) & sWhere & sOrderBy
				bPartial = False
				bColorSetNamed = False
			Case enColorSetType.Local
				sWhere = " WHERE (" & sLanduseTableName & ".ID IN (" & msLanduseList & "))"
				sComText = zzGetComText(sLanduseTableName) & sWhere & sOrderBy
				bPartial = True
				bColorSetNamed = False
			Case enColorSetType.Named
				sComText = "SELECT ColorSets.LanduseID,'', ColorSchemes.* FROM ColorSets INNER JOIN ColorSchemes ON ColorSets.ColorSchemeID = ColorSchemes.ID WHERE (ColorSets.ID) =" & Convert.ToString(miColorSetID) & sOrderBy
				bPartial = True
				bColorSetNamed = True
			Case Else
				sComText = String.Empty
		End Select
		DMAcadExt.AcadDocument.WriteMessage("ComText=" & bPartial.ToString() & ":" & bColorSetNamed & ":" & sComText)
		'	Dim sComText As String = "SELECT " & sLanduseTableName & ".ID, " & sLanduseTableName & ".Name, ColorSchemes.* FROM " & sLanduseTableName & " LEFT JOIN ColorSchemes ON " & sLanduseTableName & ".ID = ColorSchemes.ID" & sWhere
		zzFillLanduseView(sComText, bPartial, bColorSetNamed)
	End Sub
	Private Function zzGetComText(ByVal sLanduseTableName As String) As String
		Return "SELECT " & sLanduseTableName & ".ID, " & sLanduseTableName & ".Name, ColorSchemes.* FROM " & sLanduseTableName & " LEFT JOIN ColorSchemes ON " & sLanduseTableName & ".ID = ColorSchemes.ID"
	End Function

	Private Sub zzFillByColorSetNamedAAA()
		Dim sComText As String = "SELECT ColorSets.LanduseID,'', ColorSchemes.* FROM ColorSets INNER JOIN ColorSchemes ON ColorSets.ColorSchemeID = ColorSchemes.ID WHERE (ColorSets.ID) =" & Convert.ToString(miColorSetID)
		Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
		'	zzFillLanduseView(sComText, True)
	End Sub
	Private Sub zzFillLanduseView(ByVal sComText As String, ByVal bPartial As Boolean, ByVal bColorSetNamed As Boolean)
		Dim sComText1 As String = "SELECT ColorSets.LanduseID, ColorSchemes.* FROM ColorSets INNER JOIN ColorSchemes ON ColorSets.ColorSchemeID = ColorSchemes.ID WHERE (ColorSets.ID) =" & Convert.ToString(99)
		Dim sComText2 As String = "SELECT * FROM LanduseColors ORDER BY ID"

		Dim olviList As ListViewItemExt
		Dim iLanduseID, iColorSchemeID As Integer
		Dim sLanduseName As String
		If bPartial Then
			mdicLanduseIDs.Clear()
		End If

		Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
		'	Dim tColorScheme As DMAcadExt.ColorScheme
		If oDataReader IsNot Nothing Then
			Me.lvwLanduses.BeginUpdate()
			Me.imlLarge.Images.Clear()
			Do While oDataReader.Read
				iLanduseID = oDataReader.GetInt32(0)
				If bPartial Then
					mdicLanduseIDs.Add(iLanduseID, 0)
				End If
				If bPartial OrElse Not mdicLanduseIDs.ContainsKey(iLanduseID) Then
					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
					Else
						sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
					End If
					If oDataReader.IsDBNull(2) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(2)
					End If
					olviList = New ListViewItemExt()
					If iColorSchemeID <> 0 Then
						mtColorScheme = New DMAcadExt.ColorScheme(oDataReader, 2, 4.0)
						mtColorScheme.Mirror()

						olviList.ImageKey = zzAddImage(iColorSchemeID)
					End If
					olviList.LanduseID = iLanduseID
					olviList.ColorSchemeID = iColorSchemeID
					If bColorSetNamed Then
						olviList.Text = mtColorScheme.Name
					Else
						olviList.Text = sLanduseName
					End If
					'	DMAcadExt.AcadDocument.WriteMessage(CStr(bColorSetNamed) & ":" & CStr(iLanduseID) & ":" & mtColorScheme.ID_Name)
					Me.lvwLanduses.Items.Add(olviList)
				End If
			Loop
			oDataReader.Close()
			Me.lvwLanduses.EndUpdate()
			''''''''''''''''''''''''	mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
		End If
	End Sub
	Private Sub zzClearAddition()
		Me.lvwLanduses.BeginUpdate()
		For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
			If Not mdicLanduseIDs.ContainsKey(oListViewItem.LanduseID) Then
				oListViewItem.Remove()
			End If
		Next
		Me.lvwLanduses.EndUpdate()

	End Sub

	Private Sub zzRemoveImage(ByVal iColorSchemeID As Integer)
		Dim sImageKey As String = Convert.ToString(iColorSchemeID)
		If Me.imlLarge.Images.ContainsKey(sImageKey) Then
			Me.imlLarge.Images.RemoveByKey(sImageKey)
			Me.imlSmall.Images.RemoveByKey(sImageKey)
		End If
	End Sub
	Private Function zzAddImage(ByVal iColorSchemeID As Integer) As String
		Dim oBM As Bitmap
		Dim sImageKey As String
		oBM = New Bitmap(Me.pcbImage.Width, Me.pcbImage.Height)

		Me.pcbImage.DrawToBitmap(oBM, New Rectangle(0, 0, Me.pcbImage.Width, Me.pcbImage.Height))
		sImageKey = CStr(iColorSchemeID)
		If Not Me.imlLarge.Images.ContainsKey(sImageKey) Then
			Me.imlLarge.Images.Add(sImageKey, oBM)
			Me.imlSmall.Images.Add(sImageKey, oBM)
		End If
		Return sImageKey
	End Function

	Public Sub New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iColorSet As Integer, Optional ByVal sLanduseList As String = "")

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		miTopoPurpose = iTopoPurpose
		miColorSetID = iColorSet
		msLanduseList = sLanduseList
		zzSetColorSetType()

		zzMyInitializeComponent()
	End Sub

	Private Sub pcbImage_Paint(ByVal oSender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pcbImage.Paint
		zzPaint(e.Graphics)
		'	DMAcadExt.AcadDocument.WriteMessage(mtColorScheme.ID.ToString())
	End Sub

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		If Me.lvwLanduses.View = View.Tile Then
			Me.lvwLanduses.View = View.LargeIcon
		ElseIf Me.lvwLanduses.View = View.LargeIcon Then
			Me.lvwLanduses.View = View.SmallIcon
		ElseIf Me.lvwLanduses.View = View.SmallIcon Then
			Me.lvwLanduses.View = View.List
		ElseIf Me.lvwLanduses.View = View.List Then
			Me.lvwLanduses.View = View.Details
		ElseIf Me.lvwLanduses.View = View.Details Then
			Me.lvwLanduses.View = View.Tile
		End If
	End Sub
	Private Sub zzSelect()
		If Me.lvwLanduses.SelectedItems.Count = 1 Then
			Dim oListViewItem As ListViewItem = Me.lvwLanduses.SelectedItems.Item(0)
			Dim sBlockName As String, sLayerName As String
			Dim oTopoDef As DMAcadExt.TopoDef
			If oListViewItem IsNot Nothing Then
				Dim tTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(miTopoPurpose)

				oTopoDef = TopoDefs.Item(tTopoDefID)
				'		MessageBox.Show(tTopoDefID.BaseID.ToString() & ":" & miTopoPurpose.ToString(), "18_889")
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()

				'	oTopoDef = TopoDefs.moaTopoDefs(1)
				If oTopoDef Is Nothing Then
					MessageBox.Show("Definition was not found", "18_991")
				Else
					'oTopoDef = TopoDefs.moaTopoDefs(1)
					sBlockName = oTopoDef.CentroidBlocks(0)
					sLayerName = oTopoDef.CentroidLayers
					'	MessageBox.Show(sBlockName & ":" & sLayerName, "18_992")
					Dim iaAttribIndices() As Integer = {TopoManager.TPlanGraph.TplnLot.LanduseCodeAttribIndex(TPlanGraph.enTopoPurpose.Approved)}
					'	MessageBox.Show(CStr(iaAttribIndices(0)), "18_971")
					Dim saAttribText() As String = {oListViewItem.ImageKey}
					'	MessageBox.Show(sBlockName & ":" & sLayerName, "18_973")
					DMAcadExt.AcadUtil.UpdateAttribText(iaAttribIndices, saAttribText, sBlockName, sLayerName)
				End If
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
			End If
		End If
	End Sub
	Private Sub tsbViews_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbViews.Click

	End Sub
	Private Sub zzSetView(ByVal oViewMenuItem As ToolStripMenuItem, ByVal iView As System.Windows.Forms.View)
		If moCurrentViewMenuItem IsNot Nothing Then
			moCurrentViewMenuItem.Checked = False
		End If
		moCurrentViewMenuItem = oViewMenuItem
		Me.lvwLanduses.View = iView
	End Sub
	Private Sub tlbTop_ItemClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem

		Select Case oToolStripItem.Name
			Case Me.tsbUpdateSet.Name
				zzUpdateSet()
			Case Me.tsbSelect.Name
				zzSelect()
			Case Me.tsbAddColorSet.Name
				zzAddNewColorSet()

			Case Me.tsbOpenNewSet.Name
				If moSelectedItem IsNot Nothing Then 'AndAlso (Me.cmbColorSets.SelectedItem Is moSelectedItem) Then 'AndAlso Me.cmbColorSets.SelectedItem Is Nothing
					'	Me.cmbColorSets.Text = moSelectedItem.ListDispData
					MessageBox.Show(CStr(Me.cmbColorSets.SelectedIndex) & ":" & CStr(Me.cmbColorSets.Text) & ":" & CStr(moSelectedItem.ListDispData), "18_397x")
				End If
			Case Me.tsbEditColorScheme.Name
				zzEditColorScheme()
			Case Me.tsbClose.Name
				zzClose()
		End Select
	End Sub
	Private Sub zzClose()
		Me.Close()
	End Sub
	Private Function zzUpdateColorSet(ByVal iLanduseID As Integer, ByVal iColorSchemeID As Integer) As Boolean
		Dim sCom As String = "UPDATE ColorSets SET ColorSchemeID = " & Convert.ToString(iColorSchemeID) & " WHERE((ID = " & Convert.ToString(miColorSetID) & ") And ((ColorSets.LanduseID) = " & Convert.ToString(iLanduseID) & "))"
		DMAcadExt.AcadDocument.WriteMessage(sCom)

		Return (TPlServerDB.ServerDB.RunCommand(sCom) = 1)
	End Function
	Private Sub zzEditColorScheme()
		If Me.lvwLanduses.SelectedItems.Count = 1 Then
			Dim oListViewItem As ListViewItemExt = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
			Dim iColorSchemeID As Integer = oListViewItem.ColorSchemeID
			Dim iMode As enColorEditorMode
			If miColorSetType = enColorSetType.Named Then
				If oListViewItem.IsColorSchemeSource Then
					iMode = enColorEditorMode.ColorSchemeSource
				Else
					iMode = enColorEditorMode.ColorScheme
				End If
			Else
				iMode = enColorEditorMode.Landuse
			End If
			Dim fColorEditor As frmColorEditor = New frmColorEditor(iMode, iColorSchemeID)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fColorEditor)
			Try
				If fColorEditor.DialogResult = Windows.Forms.DialogResult.OK Then
					mtColorScheme = New DMAcadExt.ColorScheme(iColorSchemeID, 4.0)
					mtColorScheme.Mirror()
					If iMode = enColorEditorMode.ColorScheme Then
						zzRemoveImage(iColorSchemeID)
						zzAddImage(iColorSchemeID)
						oListViewItem.Text = mtColorScheme.Name
					ElseIf iColorSchemeID <> fColorEditor.NewID Then
						iColorSchemeID = fColorEditor.NewID
						If iColorSchemeID <> 0 Then
							zzUpdateColorSet(oListViewItem.LanduseID, iColorSchemeID)
							oListViewItem.ImageKey = zzAddImage(iColorSchemeID)
							oListViewItem.Text = mtColorScheme.Name
						End If
					End If
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmSelectLanduse")
			End Try
		End If
	End Sub
	Private Sub zzAddNewColorSet()
		Dim sName As String = Me.cmbColorSets.ComboBox.Text
		If sName.Length <> 0 Then
			If Me.cmbColorSets.ComboBox.FindString(sName) = -1 Then
				Dim iColorSetID As Integer = TPlanGraph.TplnProject.AddNewColorSet(sName)
				If iColorSetID > 0 Then
					mbPermitLanduseView = False
					Me.cmbColorSets.Items.Clear()
					TPlanGraph.TplnProject.FillColorSets(Me.cmbColorSets.ComboBox, iColorSetID)
					'Me.cmbColorSets.ComboBox.SelectedItem = New DMCommon.ItemData(iColorSetID, sName)
					mbPermitLanduseView = True
					MessageBox.Show(miTopoPurpose.ToString() & ":" & CStr(iColorSetID), "18_219")
					RaiseEvent SetColorSet(miTopoPurpose, iColorSetID, String.Empty)
				End If

			Else
				'	mbPermitLanduseView = True
				'	Me.lvwLanduses.Focus()

				'	Me.cmbColorSets.ComboBox.ResetText()
				'	Me.cmbColorSets.ComboBox.Invalidate()
				'	MessageBox.Show(Me.cmbColorSets.SelectedIndex & ":" & Me.cmbColorSets.ComboBox.Text, "18_241  ")
			End If
		End If
		mbPermitLanduseView = True
		Me.lvwLanduses.Focus()
		'	miColorSetID = iColorSetID
		'	miColorSetType = enColorSetType.Named
		'	Me.lvwLanduses.Items.Clear()
		'	MessageBox.Show(CStr(Me.cmbColorSets.Items.Contains(sName)) & ":" & CStr(Me.cmbColorSets.SelectedIndex) & ":" & CStr(Me.cmbColorSets.ComboBox.FindString(sName)), "18_380")

		'	MessageBox.Show(Me.cmbColorSets.SelectedIndex & ":" & Me.cmbColorSets.ComboBox.Text, "18_045 BEFORE")



	End Sub
	Private Sub zzSaveAsNamed()

	End Sub
	Private Sub zzAddition()
		'MessageBox.Show(miColorSetType.ToString(), "18_380")
		Dim iAddColorSetType As enColorSetType
		If miColorSetType = enColorSetType.Named OrElse miColorSetType = enColorSetType.Local Then
			For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
				oListViewItem.SelectedOrChecked = True
				oListViewItem.Marked = True
			Next
			If Me.tsiStandardOnly.Checked Then
				iAddColorSetType = enColorSetType.Standard
			Else
				iAddColorSetType = enColorSetType.All
			End If
			Me.zzFillLanduses(iAddColorSetType)
		Else
			Try
				miColorSetIndex = Me.cmbColorSets.SelectedIndex
				moSelectedItem = DirectCast(Me.cmbColorSets.SelectedItem, DMCommon.ItemData)
				'MessageBox.Show(moSelectedItem.ToString(), "18_393")
			Catch oEx As Exception
			End Try
			Me.cmbColorSets.Text = String.Empty
		End If
	End Sub
	Private Sub zzUpdateSet()
		Select Case miColorSetType
			Case enColorSetType.All, enColorSetType.Standard
			Case enColorSetType.Local
				zzUpdateLocalSet()
			Case enColorSetType.Named
				zzUpdateNamedSet()
		End Select
	End Sub
	Private Sub zzUpdateSetOld()
		mbPermitLanduseView = False
		If miColorSetType = enColorSetType.Named Then
			Me.zzAddNewColorSet()
		End If
		Dim oOleDbDataAdapter As System.Data.OleDb.OleDbDataAdapter
		Dim sSelectComText As String = "SELECT ID,LanduseID,ColorSchemeID FROM ColorSets WHERE ID=" & Convert.ToString(miColorSetID)
		Dim oDataTable As DataTable = New DataTable
		oOleDbDataAdapter = TPlServerDB.ServerDB.GetDataAdapter(sSelectComText, True)
		oOleDbDataAdapter.Fill(oDataTable)
		Dim oPrimaryKey() As DataColumn = {oDataTable.Columns.Item("LanduseID")}
		oDataTable.PrimaryKey() = oPrimaryKey
		Dim oDataRow As DataRow
		Dim iTest As Integer = 0
		Me.lvwLanduses.BeginUpdate()

		For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
			iTest += 1

			Select Case oListViewItem.Status
				Case enItemStatus.Added
					oDataRow = oDataTable.NewRow()
					oDataRow.Item("ID") = miColorSetID
					oDataRow.Item("LanduseID") = oListViewItem.LanduseID
					oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
					Try
						oDataTable.Rows.Add(oDataRow)
					Catch ex As Exception
						MessageBox.Show(ex.Message & vbCrLf & ex.StackTrace, "18_200")
					End Try


				Case enItemStatus.Modified
					oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
					oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
				Case enItemStatus.Deleted
					oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
					oDataRow.Delete()
					oListViewItem.Remove()
				Case enItemStatus.Outside
					oListViewItem.Remove()
				Case enItemStatus.Unchanged
			End Select
		Next
		oOleDbDataAdapter.Update(oDataTable)
		Me.lvwLanduses.CheckBoxes = False
		Me.lvwLanduses.EndUpdate()
		mbPermitLanduseView = True
		tsbSet.Checked = False
	End Sub

	Private Sub zzUpdateNamedSet()
		mbPermitLanduseView = False
		Dim oOleDbDataAdapter As System.Data.OleDb.OleDbDataAdapter
		Dim sSelectComText As String = "SELECT ID,LanduseID,ColorSchemeID FROM ColorSets WHERE ID=" & Convert.ToString(miColorSetID)
		Dim oDataTable As DataTable = New DataTable
		oOleDbDataAdapter = TPlServerDB.ServerDB.GetDataAdapter(sSelectComText, True)
		oOleDbDataAdapter.Fill(oDataTable)
		Dim oPrimaryKey() As DataColumn = {oDataTable.Columns.Item("LanduseID")}
		oDataTable.PrimaryKey() = oPrimaryKey
		Dim oDataRow As DataRow
		Dim iTest As Integer = 0
		Me.lvwLanduses.BeginUpdate()
		'	MessageBox.Show(CStr(miColorSetID & ":" & CStr(oDataTable.Rows.Count)), "18_490")
		For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
			iTest += 1

			Select Case oListViewItem.Status
				Case enItemStatus.Added
					'MessageBox.Show(CStr(oListViewItem.LanduseID) & ":" & CStr(oDataTable.Rows.Count), "18_497")
					oDataRow = oDataTable.NewRow()
					oDataRow.Item("ID") = miColorSetID
					oDataRow.Item("LanduseID") = oListViewItem.LanduseID
					oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
					Try
						oDataTable.Rows.Add(oDataRow)
					Catch ex As Exception
						MessageBox.Show(ex.Message & vbCrLf & ex.StackTrace, "18_200")
					End Try

					'	MessageBox.Show(CStr(oListViewItem.LanduseID), "18_498")
				Case enItemStatus.Modified
					oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
					oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
				Case enItemStatus.Deleted
					oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
					oDataRow.Delete()
					oListViewItem.Remove()
				Case enItemStatus.Outside
					oListViewItem.Remove()
				Case enItemStatus.Unchanged
			End Select
		Next
		oOleDbDataAdapter.Update(oDataTable)
		Me.lvwLanduses.CheckBoxes = False
		Me.lvwLanduses.EndUpdate()
		mbPermitLanduseView = True
		tsbSet.Checked = False
	End Sub

	Private Sub zzUpdateLocalSet()
		Dim sLanduseList As String = String.Empty
		mbPermitLanduseView = False
		For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
			Select Case oListViewItem.Status
				Case enItemStatus.Added, enItemStatus.Modified, enItemStatus.Unchanged
					If sLanduseList.Length <> 0 Then
						sLanduseList &= ","
					End If
					sLanduseList &= Convert.ToString(oListViewItem.LanduseID)
				Case enItemStatus.Unchanged, enItemStatus.Outside
					oListViewItem.Remove()
			End Select
		Next

		tsbSet.Checked = False
		mbPermitLanduseView = True
		RaiseEvent SetColorSet(miTopoPurpose, CType(enColorSetType.Local, Integer), sLanduseList)
	End Sub
	Private Sub tsbViews_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tsbViews.DropDownItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tsiLargeIcons.Name
				zzSetView(Me.tsiLargeIcons, View.LargeIcon)
			Case Me.tsiSmallIcons.Name
				zzSetView(Me.tsiSmallIcons, View.SmallIcon)
			Case Me.tsiTile.Name

				zzSetView(Me.tsiTile, View.Tile)
			Case Me.tsiList.Name
				zzSetView(Me.tsiList, View.List)
			Case Me.tsiCheckBox.Name
				''	Me.tsiSelection.Checked = Me.tsiCheckBox.Checked
			Case Me.tsiSelection.Name
				'Me.tsiCheckBox.Checked = Me.tsiSelection.Checked
		End Select
	End Sub

	Private Sub tsbSet_CheckedChanged(ByVal oSender As Object, ByVal e As System.EventArgs) Handles tsbSet.Click
		If Me.tsbSet.Checked Then
			Me.lvwLanduses.CheckBoxes = True
			zzAddition()
			Me.tsiTile.Enabled = False
		Else
			Me.lvwLanduses.CheckBoxes = False
			If miColorSetType = enColorSetType.Named OrElse miColorSetType = enColorSetType.Local Then
				zzClearAddition()
			Else
				Dim sTest As String

				Me.tsiTile.Enabled = True
				If Me.cmbColorSets.SelectedItem Is Nothing Then
					sTest = "SelectedItem Is Nothing"
				Else
					sTest = Me.cmbColorSets.SelectedItem.ToString()
				End If


				If moSelectedItem IsNot Nothing Then 'AndAlso (Me.cmbColorSets.SelectedItem Is moSelectedItem) Then 'AndAlso Me.cmbColorSets.SelectedItem Is Nothing
					'Me.cmbColorSets.Text = moSelectedItem.ListDispData
					Me.cmbColorSets.Focus()
					mbPermitLanduseView = False
					Me.cmbColorSets.ComboBox.ResetText()
					Me.cmbColorSets.ComboBox.SelectedText = moSelectedItem.ListDispData
					Me.cmbColorSets.ComboBox.SelectedIndex = miColorSetIndex
					'	MessageBox.Show(CStr(Me.cmbColorSets.Text) & ":" & CStr(moSelectedItem.ListDispData), "18_394b")
					'	Me.cmbColorSets.ComboBox.ResetText()
					mbPermitLanduseView = True
					'	miColorSetType = enColorSetType.Named
				End If
			End If

			'	oTest = Me.cmbColorSets.SelectedItem
			'	MessageBox.Show(oTest.ToString(), "18_387")

			'	MessageBox.Show(oItemData.ListIndexStr & ":" & oItemData.ListDispData, "18_381")
		End If
	End Sub

	Private Sub cmbColorSets_SelectedIndexChanged(ByVal oSender As Object, ByVal e As System.EventArgs) Handles cmbColorSets.SelectedIndexChanged
		'MessageBox.Show(CStr(mbPermitLanduseView), "18_048")
		If mbPermitLanduseView Then
			Dim oItemData As DMCommon.ItemData
			'	Dim oTest As Object = [Enum].ToObject(GetType(enColorSetType), 1)
			Try
				oItemData = DirectCast(Me.cmbColorSets.SelectedItem, DMCommon.ItemData)
				miColorSetID = oItemData.ListIndex
				zzSetColorSetType()
				'	MessageBox.Show(oItemData.ListIndexStr & ":" & oItemData.ListDispData, "18_381")
				If Me.tsbSet.Checked Then tsbSet.Checked = False
				Me.lvwLanduses.Items.Clear()
				zzFillLanduses(miColorSetType)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "cmbColorSets_SelectedIndexChanged")
			End Try
		End If


	End Sub
	Sub zzSetColorSetType()
		Select Case miColorSetID
			Case CType(enColorSetType.All, Integer)
				miColorSetType = enColorSetType.All
			Case CType(enColorSetType.Standard, Integer)
				miColorSetType = enColorSetType.Standard
			Case CType(enColorSetType.Local, Integer)
				miColorSetType = enColorSetType.Local
			Case Else
				miColorSetType = enColorSetType.Named
		End Select
	End Sub
	Private Sub tsiCheckBox_CheckedChanged(ByVal oSender As Object, ByVal e As System.EventArgs) Handles tsiCheckBox.CheckedChanged
		Me.tsiSelection.Checked = Not Me.tsiCheckBox.Checked
	End Sub

	Private Sub tsiSelection_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsiSelection.CheckedChanged
		Me.tsiCheckBox.Checked = Not Me.tsiSelection.Checked
	End Sub

	Private Sub tsiAppr_CheckedChanged(ByVal oSender As Object, ByVal e As System.EventArgs) Handles tsiAppr.CheckedChanged
		Me.tsiProp.Checked = Not Me.tsiAppr.Checked
		zzSetTopoPurpose()
	End Sub

	Private Sub tsiProp_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tsiProp.CheckedChanged
		Me.tsiAppr.Checked = Not Me.tsiProp.Checked
		zzSetTopoPurpose()
	End Sub
	Private Sub zzSetTopoPurpose()
		If Me.tsiAppr.Checked And Not Me.tsiProp.Checked Then
			miTopoPurpose = DMAcadExt.enTopoPurpose.Approved
		ElseIf Me.tsiProp.Checked And Not Me.tsiAppr.Checked Then
			miTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
		End If
	End Sub

	Private Class ListViewItemExt
		Inherits ListViewItem
		Private mbMarked As Boolean
		Private miLanduseID As Integer
		Private miColorSchemeID As Integer
		Private Shared miMarkType As enMarkType
		Public Property Marked() As Boolean
			Get
				Return mbMarked
			End Get
			Set(ByVal bValue As Boolean)
				mbMarked = bValue
			End Set
		End Property
		Public Property LanduseID() As Integer
			Get
				Return miLanduseID
			End Get
			Set(ByVal iValue As Integer)
				miLanduseID = iValue
			End Set
		End Property
		Public Property ColorSchemeID() As Integer
			Get
				Return miColorSchemeID
			End Get
			Set(ByVal iValue As Integer)
				miColorSchemeID = iValue
			End Set
		End Property

		Public ReadOnly Property IsNewAAA() As Boolean
			Get
				Return Me.SelectedOrChecked AndAlso Not mbMarked
			End Get
		End Property
		Public ReadOnly Property IsColorSchemeSource() As Boolean
			Get
				Return miLanduseID = miColorSchemeID
			End Get
		End Property
		Public ReadOnly Property Status() As enItemStatus
			Get
				If Not Me.SelectedOrChecked AndAlso Not mbMarked Then
					Return enItemStatus.Outside

				ElseIf Me.SelectedOrChecked AndAlso mbMarked Then
					If miLanduseID = miColorSchemeID Then
						Return enItemStatus.Unchanged
					Else
						Return enItemStatus.Modified
					End If
				ElseIf Me.SelectedOrChecked AndAlso Not mbMarked Then
					Return enItemStatus.Added
				ElseIf Not Me.SelectedOrChecked AndAlso mbMarked Then
					Return enItemStatus.Deleted
				End If
			End Get
		End Property
		Public ReadOnly Property IsDeletedAAA() As Boolean
			Get
				Return Not Me.SelectedOrChecked AndAlso mbMarked
			End Get
		End Property
		Public Property SelectedOrChecked() As Boolean
			Get
				If miMarkType = enMarkType.CheckBox Then
					Return MyBase.Checked
				ElseIf miMarkType = enMarkType.Selection Then
					Return MyBase.Selected
				End If
			End Get
			Set(ByVal bValue As Boolean)
				If miMarkType = enMarkType.CheckBox Then
					MyBase.Checked = bValue
				ElseIf miMarkType = enMarkType.Selection Then
					MyBase.Selected = bValue
				End If
			End Set
		End Property


		Public Sub New()

		End Sub
	End Class

	Private Sub lvwLanduses_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvwLanduses.DoubleClick
		zzEditColorScheme()
	End Sub

	Private Sub frmSelect_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

	End Sub

	Private Sub frmSelect_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
		Me.lvwLanduses.Height = Me.ClientSize.Height - miListViewTop
	End Sub
End Class