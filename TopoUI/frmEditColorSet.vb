Option Explicit On
Option Strict On
Imports System.Data
Public Enum enColorSetType
	All = 1
	Standard
	Local
	Named
	ByProject
End Enum
Public Class frmEditColorSet
	' SP -  "CopyColorSchemeToNew"

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
		Foreign
		OrderChanged
	End Enum
	Private Const miParamType As Integer = 5
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private miMapThemeID As DMAcadExt.enMapTheme
	Private mtMapThemeData As DMAcadExt.MapThemeData
	'	Private mbMarked As Boolean = True
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
	Private msLanduseList As String = ""
	Private mbItemMoved As Boolean
	Private mbDirty As Boolean
	Private miMaxSchemeOrder As Integer
	Private mbPictureBoxLarge As Boolean
   Private mdColorScale As Double = 2.0
   Private mbViewIsDetails As Boolean
   Private moParams As TPlServerDB.dmParams
   Private miDetailSortType As enDetailSortType = enDetailSortType.ByOrder
   Private mbDescending As Boolean
	Private WithEvents mfColorEditor As frmColorSchemeEditor
	Public Event SetColorSet(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iColorSetID As Integer, ByVal sLanduseList As String)
	Public Event ChangeColorSet()

	Private Sub zzMyInitializeComponent()
		'	Me.cmbColorSets.ComboBox.AutoCompleteMode = AutoCompleteMode.Suggest
		'	MessageBox.Show(CStr(miListViewTop), "03_502")
		mdicLanduseIDs = New Dictionary(Of Integer, Integer)

		DMAcadExt.DMPatterns.Init()
		zzSetView(Me.tsiLargeIcons, View.LargeIcon)
		zzSetMovedButtonsEnabled(True)
		Me.tsiCheckBox.Checked = True
      Me.tsiLargeIcons.Checked = True

		'	Const sStandard As String = "מבא""ת"
		'	Const sApprText As String = "מצב קיים"
		'	Const sPropText As String = "מצב מוצע"

		ListViewItemExt.MarkType = enMarkType.CheckBox
      '
      'lvcLanduseCode
      '
      Me.lvcLanduseCode.Name = "lvcLanduseCode"
      
      '
      'lvcLanduseName
      '
      Me.lvcLanduseName.Name = "lvcLanduseName"
      
      '
      'lvcLanduseIndex
      '
      Me.lvcLanduseIndex.Name = "lvcLanduseIndex"
      Me.lvcColorSchemeID.Name = "lvcColorSchemeID"
      Me.lvcColorSchemeName.Name = "lvcColorSchemeName"

     
      Me.lvwLanduses.Sorting = SortOrder.None
		'	msLanduseList = sLanduseList

		'
		'
		miListViewTop = Me.lvwLanduses.Top
		'		MessageBox.Show(CStr(miListViewTop), "03_551")
		'	Me.tsbViews.Image = TopoManager.My.Resources.Views
		Me.tsbSelect.Image = Global.TopoUI.My.Resources.Resources.FilterBySelect
		'	Me.tsbAddColorSet.Image = TopoManager.My.Resources.AddTable
		zzSetEditing(False)

	End Sub
	Public ReadOnly Property Dirty() As Boolean
		Get
			Return mbDirty
		End Get
	End Property

	Private Sub zzPaint(ByVal oGraphics As Graphics, bLarge As Boolean)
		If oGraphics Is Nothing Then
			''''''''''''''''''''''''oGraphics = Me.pcbImage.CreateGraphics
		End If
		Dim oPaint As TopoManager.PaintBox = New TopoManager.PaintBox(oGraphics, zzGetPictureSize(bLarge))
		oPaint.ColorScheme = mtColorScheme
		oPaint.Draw()
	End Sub

	Private Function zzGetBitmap(bLarge As Boolean) As Bitmap
		Dim tPictureSize As Size = zzGetPictureSize(bLarge)
		Return New Bitmap(tPictureSize.Width, tPictureSize.Height)
	End Function


	Private Sub zzDrawToBitmap(ByRef oBM As Bitmap, bLarge As Boolean)
		Dim tPictureSize As Size = zzGetPictureSize(bLarge)

		zzGetPictureBox(bLarge).DrawToBitmap(oBM, New Rectangle(0, 0, tPictureSize.Width, tPictureSize.Height))
	End Sub

	Private Function zzGetPictureBox(bLarge As Boolean) As PictureBox
		If bLarge Then
			Return Me.pcbLargeImage
		Else
			Return Me.pcbImage
		End If
	End Function


	Private Function zzGetPictureSize(bLarge As Boolean) As Size
		Dim oPictureBox As PictureBox = zzGetPictureBox(bLarge)
		Return oPictureBox.ClientSize
	End Function

	Private Function zzGetPictureSizeOLD(bLarge As Boolean) As Size

		If bLarge Then
			Return Me.pcbLargeImage.ClientSize
		Else
			Return Me.pcbImage.ClientSize
		End If
	End Function

	Private Sub zzAddLandusesStandard(bChecked As Boolean)
		Dim sComText As String = "SELECT * FROM dbo.LanduseColorSchemes WHERE (ID <= " & Convert.ToString(DMAcadExt.ColorScheme.MaxStandardID) & ")"

		zzAddLanduseView(sComText, True, bChecked)
		Me.tsbSet.Checked = True
		zzSetEditing(True)
	End Sub
	Private Sub zzAddLandusesAll(bChecked As Boolean)
		Dim sComText As String = "SELECT * FROM dbo.LanduseColorSchemes WHERE (ID <= " & Convert.ToString(DMAcadExt.ColorScheme.MaxLanduseID) & ")"

		zzAddLanduseView(sComText, True, bChecked)
		Me.tsbSet.Checked = True
		zzSetEditing(True)
	End Sub
	Private Sub zzAddLanduseOne(iLanduseID As Integer, bAdded As Boolean, bChecked As Boolean)
		Dim sComText As String = "SELECT * FROM dbo.LanduseColorSchemes WHERE (ID = " & Convert.ToString(iLanduseID) & ")"
      DMAcadExt.AcadDocument.WriteDebugMessage("LanduseOne=" & CStr(iLanduseID))
		zzAddLanduseView(sComText, bAdded, bChecked)
		Me.tsbSet.Checked = True
		zzSetEditing(True)
	End Sub

	Private Sub zzAddLanduseView(ByVal sComText As String, bAdded As Boolean, bChecked As Boolean)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		zzFillLanduseViewByReader(oDataReader, 2, bAdded, bChecked)
		mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
	End Sub
	Private Sub zzFillLanduseView()
		Const sSPNameLuse As String = "GetColorSchemeSet"
		Const sExproSPName As String = "GetExproColorSchemeSet"
		Dim sSPName As String
		Dim oaParams(2) As Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing
		Select Case miMapThemeID
			Case DMAcadExt.enMapTheme.Expropriation
				sSPName = sExproSPName
			Case Else
				sSPName = sSPNameLuse
		End Select
		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, miMapThemeID)
		DMCommon.Debug.MsgBox("13_200f", miMapThemeID, miProjectCode, miDetailNo, sSPName)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
		zzFillLanduseViewByReader(oDataReader, 3, False, True)
	End Sub
	Private Sub zzClear()
		mdicLanduseIDs.Clear()
		Me.lvwLanduses.Items.Clear()
		Me.imlLarge.Images.Clear()
		Me.imlSmall.Images.Clear()
		Me.imlExtraLarge.Images.Clear()

	End Sub
	Private Sub zzFillLanduseViewByProject(ByVal iProjectCode As Integer, ByVal iDetailNo As Integer, iMapThemeID As DMAcadExt.enMapTheme)
		'	Dim bColorSetNamed As Boolean = True

		Const sSPName As String = "GetColorSchemeSet"
		Dim oaParams(2) As System.Data.Common.DbParameter

		Dim oErrOut As System.Data.Common.DbException = Nothing
		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, iProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, iDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, CType(iMapThemeID, Integer))

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
		zzFillLanduseViewByReader(oDataReader, 3, True, False)

	End Sub
	Private Sub zzFillLanduseViewByReader(oDataReader As System.Data.Common.DbDataReader, bFirstColorSchemeField As Integer, bAdded As Boolean, bChecked As Boolean)
		If oDataReader IsNot Nothing Then
			'miMaxSchemeOrder
			Dim olviList As ListViewItemExt
			Dim iLanduseID, iColorSchemeID As Integer
         Dim sLanduseName As String
         Dim sLanduseNameExt As String

			Dim iSchemeOrder As Integer
         Dim iSchemeOrderFieldNo As Integer = -1
         Dim itest As Integer = 0
			Me.lvwLanduses.BeginUpdate()
			If bFirstColorSchemeField = 3 Then
				iSchemeOrderFieldNo = 2

			End If


			Do While oDataReader.Read
				iLanduseID = oDataReader.GetInt32(0)
				DMAcadExt.AcadDocument.WriteMessage(CStr(iLanduseID) & ":" & CStr(iLanduseID))
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "iLanduseID", mdicLanduseIDs.Count, iLanduseID)
				If Not mdicLanduseIDs.ContainsKey(iLanduseID) Then
					mdicLanduseIDs.Add(iLanduseID, 0)
					If oDataReader.IsDBNull(bFirstColorSchemeField) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(bFirstColorSchemeField)
					End If

					If iSchemeOrderFieldNo <> -1 AndAlso Not oDataReader.IsDBNull(iSchemeOrderFieldNo) Then

						iSchemeOrder = oDataReader.GetInt32(iSchemeOrderFieldNo)
						If miMaxSchemeOrder < iSchemeOrder Then
							miMaxSchemeOrder = iSchemeOrder
						End If

					End If

					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
						sLanduseNameExt = String.Empty
					Else
						sLanduseName = oDataReader.GetString(1)
						sLanduseNameExt = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & sLanduseName
					End If

					olviList = New ListViewItemExt()
					olviList.SelectedOrChecked = bChecked
					olviList.Added = bAdded
					olviList.OrderNumber = iSchemeOrder
					If iColorSchemeID <> 0 Then
						mtColorScheme = New DMAcadExt.ColorScheme(oDataReader, bFirstColorSchemeField, mdColorScale)
						mtColorScheme.Mirror()
						olviList.ImageKey = zzAddImage(iColorSchemeID)
						olviList.ColorSchemeName = mtColorScheme.Name
					End If

					olviList.LanduseID = iLanduseID
					olviList.LanduseName = sLanduseName

					olviList.ColorSchemeID = iColorSchemeID
					olviList.SubItems.Add(sLanduseName)
					olviList.SubItems.Add(String.Format("{0,5}", iSchemeOrder))
					olviList.SubItems.Add(iColorSchemeID.ToString())
					olviList.SubItems.Add(mtColorScheme.Name)


					DMAcadExt.AcadDocument.WriteDebugMessage("*" & CStr(iLanduseID) & ":" & CStr(iLanduseID))

					'''''''''''''	olviList.Status = enItemStatus.Added
					Dim sPrefix As String = String.Empty
					olviList.Refresh()

					'	DMAcadExt.AcadDocument.WriteMessage(CStr(bColorSetNamed) & ":" & CStr(iLanduseID) & ":" & mtColorScheme.ID_Name)
					Me.lvwLanduses.Items.Add(olviList)
				End If
			Loop
			oDataReader.Close()
            Me.lvwLanduses.EndUpdate()

			mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
		Else
			MessageBox.Show("", "01_159b")
		End If
	End Sub

	Private Sub zzFillLanduseViewByReaderOld(oDataReader As System.Data.Common.DbDataReader, ByVal bAdditional As Boolean, ByVal bPartial As Boolean, bColorSetNamed As Boolean)
		If oDataReader IsNot Nothing Then
			Dim olviList As ListViewItemExt
			Dim iLanduseID, iColorSchemeID As Integer
			Dim sLanduseName As String
			Me.lvwLanduses.BeginUpdate()
			MessageBox.Show(CStr(oDataReader.HasRows), "01_245")
			Do While oDataReader.Read
				iLanduseID = oDataReader.GetInt32(0)
				If bPartial Then
					mdicLanduseIDs.Add(iLanduseID, 0)
				End If
				If Not bAdditional OrElse Not mdicLanduseIDs.ContainsKey(iLanduseID) Then
					If oDataReader.IsDBNull(2) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(2)
					End If

					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
					Else
						sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
					End If

					olviList = New ListViewItemExt()
					olviList.SelectedOrChecked = True
					If iColorSchemeID <> 0 Then
						mtColorScheme = New DMAcadExt.ColorScheme(oDataReader, 2, mdColorScale)
						mtColorScheme.Mirror()

						olviList.ImageKey = zzAddImage(iColorSchemeID)
					End If
					olviList.LanduseID = iLanduseID
					olviList.ColorSchemeID = iColorSchemeID
					If bColorSetNamed Then
						Dim sPrefix As String = String.Empty
						If iLanduseID <> iColorSchemeID Then
							sPrefix = Convert.ToString(iColorSchemeID) & "/" '& Convert.ToString(iLanduseID)

						End If
						olviList.Text = sPrefix & Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & mtColorScheme.Name	' mtColorScheme.ID_Name
					Else
						olviList.Text = sLanduseName
					End If
					'	DMAcadExt.AcadDocument.WriteMessage(CStr(bColorSetNamed) & ":" & CStr(iLanduseID) & ":" & mtColorScheme.ID_Name)
					Me.lvwLanduses.Items.Add(olviList)
				End If
			Loop
			oDataReader.Close()
			Me.lvwLanduses.EndUpdate()
			mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
		Else
			MessageBox.Show("", "01_154")
		End If
	End Sub

	Private Sub zzRepaint()
		Dim colImages As ImageList.ImageCollection = Me.imlLarge.Images
		Dim dicKeys As System.Collections.Specialized.StringCollection = colImages.Keys
		Dim iColorSchemeID As Integer
		Dim iImageIndex As Integer
		For Each sKey As String In dicKeys

			iColorSchemeID = Convert.ToInt32(sKey)
			If iColorSchemeID <> 0 Then
				mtColorScheme = New DMAcadExt.ColorScheme(iColorSchemeID, mdColorScale)
				mtColorScheme.Mirror()
			End If
			iImageIndex = colImages.IndexOfKey(sKey)
			'		MessageBox.Show(sKey & vbCrLf & CStr(iImageIndex), "01_032")
			'		Me.pcbLargeImage.Invalidate()
			'	Me.pcbImage.Invalidate()

			zzRepaintImage(iColorSchemeID, iImageIndex)
		Next
		Me.lvwLanduses.Refresh()
	End Sub
	Private Sub zzRemoveImage(ByVal iColorSchemeID As Integer)
		Dim sImageKey As String = Convert.ToString(iColorSchemeID)
		If Me.imlLarge.Images.ContainsKey(sImageKey) Then
			Me.imlLarge.Images.RemoveByKey(sImageKey)
			Me.imlSmall.Images.RemoveByKey(sImageKey)
		End If
	End Sub
	Private Function zzAddImage(ByVal iColorSchemeID As Integer) As String
		Dim oBMLarge, oBM As Bitmap
		Dim sImageKey As String
		'oBM = New Bitmap(Me.pcbImage.Width, Me.pcbImage.Height)
		oBMLarge = zzGetBitmap(True)
		oBM = zzGetBitmap(False)

		'	MessageBox.Show(CStr(oBMLarge.Width) & ":" & CStr(oBMLarge.Height) & vbCrLf & oBM.Width & ":" & CStr(oBM.Height), "01_500")
		'	Me.pcbImage.DrawToBitmap(oBM, New Rectangle(0, 0, Me.pcbImage.Width, Me.pcbImage.Height))
		zzDrawToBitmap(oBMLarge, True)
		zzDrawToBitmap(oBM, False)

		sImageKey = CStr(iColorSchemeID)
		If Not Me.imlLarge.Images.ContainsKey(sImageKey) Then
			Me.imlExtraLarge.Images.Add(sImageKey, oBMLarge)
         Me.imlLarge.Images.Add(sImageKey, oBM)
			Me.imlSmall.Images.Add(sImageKey, oBM)
		End If
		Return sImageKey
	End Function
	Private Function zzRepaintImage(ByVal iColorSchemeID As Integer, iImageIndex As Integer) As String
		Dim oBMLarge, oBM As Bitmap
		Dim sImageKey As String
		'oBM = New Bitmap(Me.pcbImage.Width, Me.pcbImage.Height)
		oBMLarge = zzGetBitmap(True)
		oBM = zzGetBitmap(False)

		'	MessageBox.Show(CStr(oBMLarge.Width) & ":" & CStr(oBMLarge.Height) & vbCrLf & oBM.Width & ":" & CStr(oBM.Height), "01_500")
		'	Me.pcbImage.DrawToBitmap(oBM, New Rectangle(0, 0, Me.pcbImage.Width, Me.pcbImage.Height))
		zzDrawToBitmap(oBMLarge, True)
		zzDrawToBitmap(oBM, False)

		sImageKey = CStr(iColorSchemeID)
		If Me.imlLarge.Images.ContainsKey(sImageKey) Then
			Me.imlExtraLarge.Images.Item(iImageIndex) = oBMLarge
			Me.imlLarge.Images.Item(iImageIndex) = oBM
			Me.imlSmall.Images.Item(iImageIndex) = oBM
		End If
		Return sImageKey
	End Function
	Public Sub New(ByVal tMapThemeData As DMAcadExt.MapThemeData, ByVal iColorSet As Integer)
		'MessageBox.Show("TopoUI" & ":" & Me.Name)
		' This call is required by the Windows Form Designer.
		InitializeComponent()
		'	MessageBox.Show(CStr(miListViewTop), "03_501")
		' Add any initialization after the InitializeComponent() call.
		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		mtMapThemeData = tMapThemeData
		miMapThemeID = mtMapThemeData.MapThemeID
		miColorSetID = iColorSet
		DMCommon.Debug.MsgBox("13_200e", miMapThemeID, miColorSetID, mtMapThemeData.TopoName)
		'MessageBox.Show(CStr(miProjectCode) & ":" & CStr(miDetailNo) & vbCrLf & miMapThemeID.ToString() & vbCrLf & iColorSet.ToString(), "04_431")
		zzSetColorSetType()

        zzMyInitializeComponent()
        Dim tA As SizeF = Me.AutoScaleDimensions()
        '  MessageBox.Show(CStr(Me.Size.Width) & ":" & CStr(Me.Size.Height) & vbCrLf & Me.AutoScaleBaseSize.Width.ToString() & vbCrLf & Me.AutoScaleBaseSize.Height.ToString() & vbCrLf & tA.Width.ToString() & vbCrLf & tA.Height.ToString(), "04_499")

        moParams = New TPlServerDB.dmParams(miProjectCode, miDetailNo, miMapThemeID, miParamType)
		zzInitColorScale()
	End Sub
	Private Sub zzRefresh()
		zzClear()
		zzFillLanduseView()
	End Sub
	Private Sub pcbImage_Paint(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pcbImage.Paint
		zzPaint(e.Graphics, False)
		'	DMAcadExt.AcadDocument.WriteMessage(mtColorScheme.ID.ToString())
	End Sub

	Private Sub pcbLargeImage_Paint(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pcbLargeImage.Paint
		zzPaint(e.Graphics, True)
		'	DMAcadExt.AcadDocument.WriteMessage(mtColorScheme.ID.ToString())
	End Sub
	Private Sub zzSelect()
		If Me.lvwLanduses.SelectedItems.Count = 1 Then
			Dim oListViewItem As ListViewItemExt = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
			Dim sBlockName As String, sLayerName As String

			If oListViewItem IsNot Nothing Then
				'	Dim tTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(miTopoPurpose)


				'		MessageBox.Show(tTopoDefID.BaseID.ToString() & ":" & miTopoPurpose.ToString(), "18_889")

				'	oTopoDef = TopoDefs.moaTopoDefs(1)
				If mtMapThemeData.IsNotEmpty Then
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()




					'oTopoDef = TopoDefs.moaTopoDefs(1)
					sBlockName = mtMapThemeData.CentroidBlocks  'oTopoDef.CentroidBlocks(0)
					sLayerName = mtMapThemeData.CentroidLayers
					'	MessageBox.Show(sBlockName & ":" & sLayerName, "18_992")
					Dim iaAttribIndices() As Integer = {TopoManager.TPlanGraph.TplnLot.LanduseCodeAttribIndexNew(TopoManager.TPlanGraph.enTopoPurpose.Approved)}

					Dim saAttribText() As String = {Convert.ToString(oListViewItem.LanduseID)}

					DMAcadExt.AcadUtil.UpdateAttribText(iaAttribIndices, saAttribText, sBlockName, sLayerName)

				Else
					MessageBox.Show("Definition was not found", "18_991")
				End If

			End If
		End If
	End Sub
	Private Sub zzClearColorScaleCheced()
		Me.tsiColorScale_1.Checked = False
		Me.tsiColorScale_2.Checked = False
		Me.tsiColorScale_4.Checked = False
		Me.tsiColorScale_6.Checked = False
		Me.tsiColorScale_8.Checked = False

	End Sub
	Private Sub zzSetView(ByVal oViewMenuItem As ToolStripMenuItem, ByVal iView As System.Windows.Forms.View)
		If moCurrentViewMenuItem IsNot Nothing Then
			moCurrentViewMenuItem.Checked = False
		End If
		moCurrentViewMenuItem = oViewMenuItem
		Me.lvwLanduses.View = iView
	End Sub
	Private Sub tlbTop_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Dim oListViewItem As ListViewItemExt
		Me.Cursor = Cursors.WaitCursor
		Select Case oToolStripItem.Name
			Case Me.tsbUpdateSet.Name
				zzUpdatePrjSet()
			Case Me.tsbSelect.Name
				If Me.lvwLanduses.SelectedItems.Count > 0 Then
					Try
						oListViewItem = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditColorSet - tlbTop_ItemClicked")
					End Try
				End If

				'	MessageBox.Show("Added=" & CStr(oListViewItem.Added) & vbCrLf & "Modified=" & CStr(oListViewItem.Modified) & vbCrLf & "LanduseID=" & CStr(oListViewItem.LanduseID) & vbCrLf & "ColorSchemeID=" & CStr(oListViewItem.ColorSchemeID) & vbCrLf & "ColorSchemeName=" & CStr(oListViewItem.ColorSchemeName), "01_100")
				zzSelect()

			Case Me.tsbSet.Name
				zzSetEditing(Not Me.tsbSet.Checked)
			Case Me.tsbRefresh.Name
				zzRefresh()
			Case Me.tsbLeft.Name
				zzMoveItem(SearchDirectionHint.Right)
			Case Me.tsbRight.Name
				zzMoveItem(SearchDirectionHint.Left)
			Case Me.tsbUp.Name
				zzMoveItem(SearchDirectionHint.Up)
			Case Me.tsbDown.Name
				zzMoveItem(SearchDirectionHint.Down)
			Case Me.tsbOpenNewSet.Name
				zzEraseAdded()
			Case Me.tsbEditColorScheme.Name
				zzEditColorScheme()
			Case Me.tsbCheckAll.Name
				zzSelectAll(True)
			Case Me.tsbUncheckAll.Name
				zzSelectAll(False)
			Case Me.tsbClose.Name
				'	MessageBox.Show(CStr(Me.Size.Width) & ":" & CStr(Me.Size.Height) & vbCrLf & CStr(Me.lvwLanduses.Location.X) & ":" & CStr(Me.lvwLanduses.Location.Y))
				zzClose()
		End Select
		'''''''''''	lvwLanduses.CheckBoxes = True
		Me.Cursor = Cursors.Default
	End Sub
	Private Sub zzEraseAdded()
		For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
			If oListViewItem.Added Then
				mdicLanduseIDs.Remove(oListViewItem.LanduseID)
				oListViewItem.Remove()
			End If
		Next
   End Sub
   Private Sub zzSetItemTextByViewAAA()
      If mbViewIsDetails Then
         zzSetDetailCode()
      Else
         zzRefreshItems()
      End If
   End Sub

   Private Sub zzRefreshItems()
      If mbViewIsDetails Then
         For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
            oListViewItem.Refresh()
         Next
      End If
      mbViewIsDetails = False
   End Sub
   Private Sub zzSetDetailCode()
      If Not mbViewIsDetails Then
         For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
            oListViewItem.SetDetailCode()
         Next
         mbViewIsDetails = True
      End If

   End Sub
   Private Sub zzSelectAll(bChecked As Boolean)
      For Each oListViewItem As ListViewItemExt In Me.lvwLanduses.Items
         oListViewItem.SelectedOrChecked = bChecked
      Next
   End Sub
   Private Sub zzMoveItem(ByVal iDir As System.Windows.Forms.SearchDirectionHint)
      Dim oListViewItem As ListViewItemExt
      Dim oListViewItemNext As ListViewItemExt

      If Me.lvwLanduses.SelectedItems.Count > 0 Then
         mbItemMoved = True
         Try
            oListViewItem = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
            '	MessageBox.Show(oListViewItem.FindNearestItem(iDir).GetType().ToString(), "26_022")
            '	oListViewItemNext = Me.ListView1.Items(iIndex + 1)
            Select Case Me.lvwLanduses.View
               Case View.LargeIcon, View.SmallIcon
                  oListViewItemNext = DirectCast(oListViewItem.FindNearestItem(iDir), ListViewItemExt)

                  If oListViewItemNext IsNot Nothing Then
                     '		MessageBox.Show(CStr(oListViewItem.Index) & "->" & CStr(oListViewItemNext.Index), "01_106")
                     Me.lvwLanduses.Items.Remove(oListViewItem)
                     Me.lvwLanduses.Items.Remove(oListViewItemNext)
                     Select Case iDir
                        Case Windows.Forms.SearchDirectionHint.Down, Windows.Forms.SearchDirectionHint.Right
                           Me.lvwLanduses.Items.Add(oListViewItemNext)
                           Me.lvwLanduses.Items.Add(oListViewItem)
                        Case Else
                           Me.lvwLanduses.Items.Add(oListViewItem)
                           Me.lvwLanduses.Items.Add(oListViewItemNext)
                     End Select
                  End If
               Case Else
                  Dim iNextIndex As Integer = zzGetNextIndex(oListViewItem, iDir)
                  If iNextIndex >= 0 Then
                     Me.lvwLanduses.Items.Remove(oListViewItem)
                     Me.lvwLanduses.Items.Insert(iNextIndex, oListViewItem)
                  End If
            End Select

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmSelectLandus - zzMoveItem")
         End Try



      End If
   End Sub


   Private Sub zzAddProjectSet()
      Dim fSelectColorSet As frmSelectPrjColorSet = New frmSelectPrjColorSet()

      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fSelectColorSet)

      If fSelectColorSet.DialogResult = Windows.Forms.DialogResult.OK Then
         zzFillLanduseViewByProject(fSelectColorSet.ProjectCode, fSelectColorSet.DetailNo, fSelectColorSet.MapThemeID)
         Me.tsbSet.Checked = True
         zzSetEditing(True)
      End If

   End Sub
   Private Function zzGetNextItem(oListViewItem As ListViewItemExt, ByVal iDir As System.Windows.Forms.SearchDirectionHint) As ListViewItemExt
      Dim iIndex As Integer = oListViewItem.Index
      Dim iNextIndex As Integer

      Select Case iDir
         Case SearchDirectionHint.Down
            If iIndex > 0 Then
               iNextIndex = iIndex - 1
            Else
               Return Nothing
            End If
         Case SearchDirectionHint.Up
            If iIndex < Me.lvwLanduses.Items.Count - 1 Then
               iNextIndex = iIndex + 1
            Else
               Return Nothing
            End If
      End Select
      Return DirectCast(Me.lvwLanduses.Items(iNextIndex), ListViewItemExt)
   End Function
   Private Function zzGetNextIndex(oListViewItem As ListViewItemExt, ByVal iDir As System.Windows.Forms.SearchDirectionHint) As Integer
      Dim iIndex As Integer = oListViewItem.Index
      Dim iNextIndex As Integer

      Select Case iDir
         Case SearchDirectionHint.Up
            If iIndex > 0 Then
               Return iIndex - 1
            Else
               Return -1
            End If
         Case SearchDirectionHint.Down
            If iIndex < Me.lvwLanduses.Items.Count - 1 Then
               iNextIndex = iIndex + 1
            Else
               Return -1
            End If
         Case SearchDirectionHint.Left
            If iIndex > 0 Then
               Return 0
            Else
               Return -1
            End If
         Case SearchDirectionHint.Right
            If iIndex < Me.lvwLanduses.Items.Count - 1 Then
               Return Me.lvwLanduses.Items.Count - 1
            Else
               Return -1
            End If

      End Select
      '	MessageBox.Show(CStr(iIndex) & "->" & CStr(iNextIndex), "01_105")
      Return iNextIndex
   End Function
   Private Sub zzClose()
      Me.Close()
   End Sub

   Private Function zzAddColorSet(ByVal iLanduseID As Integer, ByVal iColorSchemeID As Integer) As Boolean ', ByVal iSchemeOrder As Integer
      Const sSPName As String = "AddPrjColorSet"
      Dim oaParams(5) As Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing
      miMaxSchemeOrder = miMaxSchemeOrder + 1
      '	MessageBox.Show(CStr(iLanduseID) & ":" & CStr(iColorSchemeID) & ":" & CStr(miMaxSchemeOrder), "01_442")
      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, miMapThemeID)
      oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapElementID", DbType.Int32, iLanduseID)
      oaParams(4) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prColorSchemeID", DbType.Int32, iColorSchemeID)
      oaParams(5) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prSchemeOrder", DbType.Int32, miMaxSchemeOrder)

      Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, CommandType.StoredProcedure, oaParams)
      Return (iRes = 1)
   End Function

   Private Function zzUpdateColorSet(ByVal iLanduseID As Integer, ByVal iColorSchemeID As Integer) As Boolean
      Const sSPName As String = "UpdatePrjColorSet"
      Dim oaParams(4) As Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing

      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, miMapThemeID)
      oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapElementID", DbType.Int32, iLanduseID)
      oaParams(4) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prColorSchemeID", DbType.Int32, iColorSchemeID)

      Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, CommandType.StoredProcedure, oaParams)
      Return (iRes = 1)
   End Function

   Private Sub zzEditColorScheme()
      If Me.lvwLanduses.SelectedItems.Count = 1 Then
         Dim oListViewItem As ListViewItemExt = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
         Dim iLanduseID As Integer = oListViewItem.LanduseID
         Dim iColorSchemeID As Integer = oListViewItem.ColorSchemeID



         Dim bReadOnly As Boolean = Me.lvwLanduses.CheckBoxes
         If Me.lvwLanduses.CheckBoxes Then
            oListViewItem.SelectedOrChecked = True
         End If
         Dim iMode As enColorEditorMode
         '		If miColorSetType = enColorSetType.Named Then
         If oListViewItem.IsColorSchemeSource Then
            iMode = enColorEditorMode.ColorSchemeSource
         Else
            iMode = enColorEditorMode.ColorScheme
         End If
			'			Else
			'				iMode = TopoManager.enColorEditorMode.Landuse
			'			End If
			'		MessageBox.Show(CStr(iLanduseID) & ":" & CStr(iColorSchemeID), "26_877")
			Dim fColorEditor As frmColorSchemeEditor = New frmColorSchemeEditor(miMapThemeID, iMode, bReadOnly, iLanduseID, iColorSchemeID)


			fColorEditor.Owner = Me
			fColorEditor.ShowDialog()

			If False Then
				Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
				Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fColorEditor)
			End If

			Try
            If fColorEditor.DialogResult = Windows.Forms.DialogResult.OK Then
               mbDirty = True
               '	oListViewItem.Modified = True
               RaiseEvent ChangeColorSet()
               If fColorEditor.NewID <> 0 AndAlso iColorSchemeID <> fColorEditor.NewID Then
                  iColorSchemeID = fColorEditor.NewID
               End If
               oListViewItem.ColorSchemeID = iColorSchemeID
               mtColorScheme = New DMAcadExt.ColorScheme(iColorSchemeID, mdColorScale)
               mtColorScheme.Mirror()
               '	MessageBox.Show(CStr(iColorSchemeID) & ":" & iMode.ToString() & ":" & mtColorScheme.Name, "26_800")
               If iMode = TopoManager.enColorEditorMode.ColorScheme Then
                  zzRemoveImage(iColorSchemeID)
                  zzAddImage(iColorSchemeID)
                  oListViewItem.Text = DMAcadExt.ColorScheme.GetFullName(iLanduseID, iColorSchemeID, mtColorScheme.Name)
               ElseIf iMode = TopoManager.enColorEditorMode.ColorSchemeSource Then
                  If iColorSchemeID <> 0 Then
                     If oListViewItem.Added Then
                        zzAddColorSet(oListViewItem.LanduseID, iColorSchemeID)
                        oListViewItem.Added = False
                     Else
                        zzUpdateColorSet(oListViewItem.LanduseID, iColorSchemeID)
                     End If

                     'MessageBox.Show(oListViewItem.ImageKey & ":" & oListViewItem.Text, "26_801 bef")
                     oListViewItem.ImageKey = zzAddImage(iColorSchemeID)
                     oListViewItem.Text = DMAcadExt.ColorScheme.GetFullName(iLanduseID, iColorSchemeID, mtColorScheme.Name)
                     'MessageBox.Show(oListViewItem.ImageKey & ":" & oListViewItem.Text & ":" & CStr(Me.imlLarge.Images.ContainsKey(oListViewItem.ImageKey)), "26_802 after")
                  End If
               End If
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmSelectLanduse")
         End Try
      End If
   End Sub
   Private Sub zzEditColorScheme290513()
      If Me.lvwLanduses.SelectedItems.Count = 1 Then
         Dim oListViewItem As ListViewItemExt = DirectCast(Me.lvwLanduses.SelectedItems.Item(0), ListViewItemExt)
         Dim iLanduseID As Integer = oListViewItem.LanduseID
         Dim iColorSchemeID As Integer = oListViewItem.ColorSchemeID
         MessageBox.Show(CStr(iLanduseID) & ":" & CStr(iColorSchemeID), "26_876")
         Dim bReadOnly As Boolean = Me.lvwLanduses.CheckBoxes
         If Me.lvwLanduses.CheckBoxes Then
            oListViewItem.SelectedOrChecked = True
         End If
         Dim iMode As enColorEditorMode
         '		If miColorSetType = enColorSetType.Named Then
         If oListViewItem.IsColorSchemeSource Then
            iMode = enColorEditorMode.ColorSchemeSource
         Else
            iMode = enColorEditorMode.ColorScheme
         End If
			'			Else
			'				iMode = TopoManager.enColorEditorMode.Landuse
			'			End If
			Dim fColorEditor As frmColorSchemeEditor = New frmColorSchemeEditor(miMapThemeID, iMode, bReadOnly, iLanduseID, iColorSchemeID)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fColorEditor)
         Try
            If fColorEditor.DialogResult = Windows.Forms.DialogResult.OK Then
               mbDirty = True
               oListViewItem.Modified = True
               RaiseEvent ChangeColorSet()
               If fColorEditor.NewID <> 0 AndAlso iColorSchemeID <> fColorEditor.NewID Then
                  iColorSchemeID = fColorEditor.NewID
               End If
               mtColorScheme = New DMAcadExt.ColorScheme(iColorSchemeID, 2.0)
               mtColorScheme.Mirror()
               '	MessageBox.Show(CStr(iColorSchemeID) & ":" & iMode.ToString() & ":" & mtColorScheme.Name, "26_800")
               If iMode = TopoManager.enColorEditorMode.ColorScheme Then
                  zzRemoveImage(iColorSchemeID)
                  zzAddImage(iColorSchemeID)
                  oListViewItem.Text = DMAcadExt.ColorScheme.GetFullName(iLanduseID, iColorSchemeID, mtColorScheme.Name)
               ElseIf iMode = TopoManager.enColorEditorMode.ColorSchemeSource Then
                  If iColorSchemeID <> 0 Then
                     zzUpdateColorSet(oListViewItem.LanduseID, iColorSchemeID)
                     'MessageBox.Show(oListViewItem.ImageKey & ":" & oListViewItem.Text, "26_801 bef")
                     oListViewItem.ImageKey = zzAddImage(iColorSchemeID)
                     oListViewItem.Text = DMAcadExt.ColorScheme.GetFullName(iLanduseID, iColorSchemeID, mtColorScheme.Name)
                     'MessageBox.Show(oListViewItem.ImageKey & ":" & oListViewItem.Text & ":" & CStr(Me.imlLarge.Images.ContainsKey(oListViewItem.ImageKey)), "26_802 after")
                  End If
               End If
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmEditColorSet")
         End Try
      End If
   End Sub

   Private Sub zzUpdatePrjSet()
      Dim oDbDataAdapter As System.Data.Common.DbDataAdapter
      Dim sSelectComText As String = "SELECT ProjectCode,Detail,MapThemeID,MapElementID, ColorSchemeID, SchemeOrder, ElementOwned FROM dbo.ColorSchemeSets WHERE (ProjectCode = " & CStr(miProjectCode) & ") AND (Detail = " & CStr(miDetailNo) & ") AND (MapThemeID = " & CStr(miMapThemeID) & ")"
      Dim oDataTable As DataTable = New DataTable
      mbPermitLanduseView = False
      oDbDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSelectComText, CommandType.Text, True)
      oDbDataAdapter.Fill(oDataTable)
      Dim oPrimaryKey() As DataColumn = {oDataTable.Columns.Item("MapElementID")}
      oDataTable.PrimaryKey() = oPrimaryKey
      Dim oDataRow As DataRow
      'Dim iTest As Integer = 0
      Dim iOrderIndex As Integer = 0
      Dim oListSort As List(Of ListViewItemExt) = New List(Of ListViewItemExt)
      Dim oListDelete As List(Of ListViewItemExt) = New List(Of ListViewItemExt)
		DMCommon.Debug.MsgBox("13_200j", miMapThemeID, miProjectCode, miDetailNo, sSelectComText)
		Dim oItemComparer As ItemComparerByLocation = New ItemComparerByLocation(Me.lvwLanduses.View)
      Dim iNewColorSchemeID As Integer
      For Each oItem As ListViewItemExt In Me.lvwLanduses.Items
         If oItem.SelectedOrChecked Then
            oListSort.Add(oItem)
         Else
            oListDelete.Add(oItem)
            Try
               mdicLanduseIDs.Remove(oItem.LanduseID)
            Catch oEx As Exception
               MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "LanduseID=" & CStr(oItem.LanduseID), "frmEditColorSet - zzUpdatePrjSet")
            End Try
         End If

      Next

      oListSort.Sort(oItemComparer)

      Me.lvwLanduses.BeginUpdate()
      Me.lvwLanduses.Items.Clear()
      '	MessageBox.Show(CStr(oListSort.Count) & ":" & CStr(oListDelete.Count) & ":" & CStr(oDataTable.Rows.Count), "18_490")
      For Each oListViewItem As ListViewItemExt In oListSort
         If oListViewItem.Added Then
            oDataRow = oDataTable.NewRow()
            oDataRow.Item("ProjectCode") = miProjectCode
            oDataRow.Item("Detail") = miDetailNo
				oDataRow.Item("MapThemeID") = CType(miMapThemeID, Integer)
				oDataRow.Item("MapElementID") = oListViewItem.LanduseID
            oDataRow.Item("SchemeOrder") = iOrderIndex

            If Not oListViewItem.Modified Then
               iNewColorSchemeID = zzCopyColorSchemeToNew(oListViewItem.ColorSchemeID)
					If iNewColorSchemeID > 0 Then
						DMCommon.Debug.MsgBox("13_200L", iNewColorSchemeID, oListViewItem.ColorSchemeID)
						oDataRow.Item("ColorSchemeID") = iNewColorSchemeID
						oListViewItem.ColorSchemeID = iNewColorSchemeID
						oListViewItem.Refresh()
					Else
						Return
               End If
            Else
               oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
               oListViewItem.Modified = False
            End If
				iOrderIndex += 1
				DMCommon.Debug.ExcelLog.SetEnumerable(0, "oDataRow", oDataRow.ItemArray)
				DMCommon.Debug.ExcelLog.SetNextValue(2, "miMapThemeID", miMapThemeID)


				Try
					oDataTable.Rows.Add(oDataRow)
					oListViewItem.Added = False
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(oListViewItem.LanduseID), "18_200d")
				End Try
         Else
            oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
            If oListViewItem.Modified Then
               oDataRow.Item("ColorSchemeID") = oListViewItem.ColorSchemeID
               oListViewItem.Modified = False
            End If
            oDataRow.Item("SchemeOrder") = iOrderIndex

         End If
         miMaxSchemeOrder = iOrderIndex
         iOrderIndex += 1
         Me.lvwLanduses.Items.Add(oListViewItem)
         'DMAcadExt.AcadDocument.WriteMessage("Stat=" & oListViewItem.Status.ToString())

      Next
      For Each oListViewItem As ListViewItemExt In oListDelete
         oDataRow = oDataTable.Rows.Find(oListViewItem.LanduseID)
         If oDataRow IsNot Nothing Then
            oDataRow.Delete()
            '	oListViewItem.Remove()
         End If

      Next
      Dim iRes As Integer = oDbDataAdapter.Update(oDataTable)
      '	MessageBox.Show(CStr(iRes), "18_800")
      If Me.lvwLanduses.CheckBoxes Then
         Me.lvwLanduses.CheckBoxes = False
         tsbSet.Checked = False
      End If
      Me.lvwLanduses.EndUpdate()

      mbPermitLanduseView = True
   End Sub


   Private Function zzCopyColorSchemeToNew(iSourceID As Integer) As Integer
		Const sSPNameLanduse As String = "CopyColorSchemeToNew"
		Const sSPNameExpro As String = "CopyColorSchemeExproToNew"
		Dim sSPName As String
		Dim oaParams(3) As Common.DbParameter
		Dim iDestID As Integer

		Select Case miMapThemeID
			Case DMAcadExt.enMapTheme.Expropriation
				sSPName = sSPNameExpro
			Case Else
				sSPName = sSPNameLanduse
		End Select

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prID_Source", DbType.Int32, iSourceID)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prID_Dest", DbType.Int32, 0, ParameterDirection.InputOutput)
		If miMapThemeID = DMAcadExt.enMapTheme.LotApproved OrElse miMapThemeID = DMAcadExt.enMapTheme.LotProposed Then
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMinColorSchemeID", DbType.Int32, TopoManager.TPlanGraph.TplnProject.ColorScheme_ID_Control.MinID)
			oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMaxColorSchemeID", DbType.Int32, TopoManager.TPlanGraph.TplnProject.ColorScheme_ID_Control.MaxID)
		ElseIf miMapThemeID = DMAcadExt.enMapTheme.Expropriation Then

			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMinColorSchemeID", DbType.Int32, TopoManager.TPlanGraph.TplnProject.ColorSchemeExpro_ID_Control.MinID)
			oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMaxColorSchemeID", DbType.Int32, TopoManager.TPlanGraph.TplnProject.ColorSchemeExpro_ID_Control.MaxID)
		End If



		'		@prID_Sorce int,@prID_Dest int output,@prMinColorSchemeID int,@prMaxColorSchemeID int
		Dim oCommandErr As System.Data.Common.DbException = Nothing
		Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, CommandType.StoredProcedure, oaParams, oCommandErr)

		MessageBox.Show(sSPName & vbCrLf & CStr(126) & vbCrLf & oaParams(2).Value.ToString() & vbCrLf & oaParams(3).Value.ToString() & vbCrLf & iRes.ToString(), "bef 211a")

		'   MessageBox.Show(sSPName & vbCrLf & CStr(iRes), "aft 212")
		If iRes <> 0 Then
         Try
            iDestID = DirectCast(oaParams(1).Value, Integer)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditColorSet - zzCopyColorSchemeToNew")
         End Try


         Dim sDestID As String = iDestID.ToString()
         '	MessageBox.Show(sDestID & vbCrLf & CStr(iRes), "26_733")
         Return iDestID
      Else
         Return -1
      End If

   End Function

   Private Sub zzSetEditing(bEditing As Boolean)

      If ListViewItemExt.MarkType = enMarkType.CheckBox Then
         If bEditing AndAlso Me.lvwLanduses.View <> View.Tile Then
            Me.lvwLanduses.View = View.LargeIcon
         End If
         Me.lvwLanduses.CheckBoxes = bEditing
      End If
      Me.tsbUncheckAll.Enabled = bEditing
      Me.tsbCheckAll.Enabled = bEditing

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
         Case Me.tsiExtraLarge.Name
            Me.tsbSet.Enabled = True
            zzSetLargeImages(True)
            zzRefreshItems()
            zzSetView(Me.tsiExtraLarge, View.LargeIcon)

            zzSetMovedButtonsEnabled(True)
         Case Me.tsiLargeIcons.Name
            Me.tsbSet.Enabled = True
            zzSetLargeImages(False)
            zzRefreshItems()
            zzSetView(Me.tsiLargeIcons, View.LargeIcon)
            zzSetMovedButtonsEnabled(True)
         Case Me.tsiSmallIcons.Name
            Me.tsbSet.Enabled = True
            zzRefreshItems()
            zzSetView(Me.tsiSmallIcons, View.SmallIcon)
            zzSetMovedButtonsEnabled(True)
         Case Me.tsiTile.Name
            Me.tsbSet.Enabled = False
            zzRefreshItems()
            zzSetView(Me.tsiTile, View.Tile)
            zzSetMovedButtonsEnabled(True)
         Case Me.tsiList.Name
            Me.tsbSet.Enabled = True
            zzSetView(Me.tsiList, View.List)
            zzRefreshItems()
            zzSetMovedButtonsEnabled(True)
         Case Me.tsiDetails.Name
            zzSetDetailCode()
            zzSetView(Me.tsiDetails, View.Details)
         Case tsiColorScale_1.Name
            MessageBox.Show("", "01_121")
            zzClearColorScaleCheced()
         Case Me.tsiCheckBox.Name
            ''	Me.tsiSelection.Checked = Me.tsiCheckBox.Checked
         Case Me.tsiSelection.Name
            'Me.tsiCheckBox.Checked = Me.tsiSelection.Checked
      End Select
   End Sub
   Private Sub zzSetMovedButtonsEnabled(ByVal bEnabled As Boolean)
      If Me.tsbDown.Enabled <> bEnabled Then
         Me.tsbDown.Enabled = bEnabled
         Me.tsbUp.Enabled = bEnabled
         Me.tsbLeft.Enabled = bEnabled
         Me.tsbRight.Enabled = bEnabled
      End If
   End Sub


   Private Sub tsbSet_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsbSet.Click
      Return
      MessageBox.Show("", "_01_282")
      If Me.tsbSet.Checked Then
         If Me.lvwLanduses.View <> View.Tile Then
            Me.lvwLanduses.CheckBoxes = True
            Me.tsbUpdateSet.Enabled = True
            ''''	zzAddition()
            Me.tsiTile.Enabled = False
         End If
         zzSetMovedButtonsEnabled(False)
      Else
         Me.lvwLanduses.CheckBoxes = False
         If miColorSetType = enColorSetType.Named Then
            Me.tsbUpdateSet.Enabled = True
         Else
            Me.tsbUpdateSet.Enabled = False
         End If
         Me.tsiTile.Enabled = True
         zzSetMovedButtonsEnabled(Me.lvwLanduses.View = View.LargeIcon)

      End If
   End Sub




   Sub zzSetColorSetType()
      Select Case miColorSetID
         Case CType(enColorSetType.All, Integer)
            miColorSetType = enColorSetType.All
            Me.tsbUpdateSet.Enabled = False
         Case CType(enColorSetType.Standard, Integer)
            miColorSetType = enColorSetType.Standard
            Me.tsbUpdateSet.Enabled = False
         Case CType(enColorSetType.Local, Integer)
            miColorSetType = enColorSetType.Local
            Me.tsbUpdateSet.Enabled = False
         Case Else
            miColorSetType = enColorSetType.Named
            Me.tsbUpdateSet.Enabled = True
      End Select
   End Sub
   Private Sub tsiCheckBox_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsiCheckBox.CheckedChanged
      Me.tsiSelection.Checked = Not Me.tsiCheckBox.Checked
   End Sub

   Private Sub tsiSelection_CheckedChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsiSelection.CheckedChanged
      Me.tsiCheckBox.Checked = Not Me.tsiSelection.Checked
   End Sub

   Private Class ListViewItemExt
      Inherits ListViewItem
      Private mbAdded As Boolean
      Private mbModified As Boolean
      Private mbMarked As Boolean
      Private mbMoved As Boolean

      Private miOrderNumber As Integer
      Private miLanduseID As Integer
      Private msLanduseName As String
      Private miColorSchemeID As Integer
      Private msColorSchemeName As String
      Private Shared miMarkType As enMarkType
      Public Shared Property MarkType As enMarkType
         Get
            Return miMarkType
         End Get
         Set(iValue As enMarkType)
            miMarkType = iValue
         End Set
      End Property
      Public ReadOnly Property ForeignAAA() As Boolean
         Get
            Return miLanduseID <> miColorSchemeID
         End Get

      End Property
      Public Property Added() As Boolean
         Get
            Return mbAdded
         End Get
         Set(ByVal bValue As Boolean)
            mbAdded = bValue
         End Set
      End Property
      Public Property Marked() As Boolean
         Get
            Return mbMarked
         End Get
         Set(ByVal bValue As Boolean)
            mbMarked = bValue
         End Set
      End Property

      Public Property Moved() As Boolean
         Get
            Return mbMoved
         End Get
         Set(ByVal bValue As Boolean)
            mbMoved = bValue
         End Set
      End Property
      Public Property Modified() As Boolean
         Get
            Return mbModified
         End Get
         Set(ByVal bValue As Boolean)
            mbModified = bValue
         End Set
      End Property
      Public Property OrderNumber() As Integer
         Get
            Return miOrderNumber
         End Get
         Set(ByVal iValue As Integer)
            miOrderNumber = iValue
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
      Public Property LanduseName() As String
         Get
            Return msLanduseName
         End Get
         Set(ByVal sValue As String)
            msLanduseName = sValue
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
      Public Property ColorSchemeName() As String
         Get
            Return msColorSchemeName
         End Get
         Set(ByVal sValue As String)
            msColorSchemeName = sValue
         End Set
      End Property

      Public Function OwnColorScheme() As Boolean
         If mbAdded AndAlso (Not mbModified) Then
            Return False
         ElseIf IsColorSchemeSource Then
            Return False
         Else
            Return True
         End If
      End Function
      Public ReadOnly Property IsColorSchemeSource() As Boolean
         Get
            Return miLanduseID = miColorSchemeID
         End Get
      End Property

      Public ReadOnly Property Status() As enItemStatus
         Get
            If Me.Moved Then
               Return enItemStatus.OrderChanged
            ElseIf Me.Added Then
               Return enItemStatus.Added
            ElseIf Not Me.SelectedOrChecked AndAlso Not mbMarked Then
               Return enItemStatus.Outside

            ElseIf Me.SelectedOrChecked AndAlso mbMarked Then
               If miLanduseID = miColorSchemeID Then
                  Return enItemStatus.Unchanged
               Else
                  Return enItemStatus.Modified
               End If

            ElseIf Not Me.SelectedOrChecked AndAlso mbMarked Then
               Return enItemStatus.Deleted
            Else
               Return enItemStatus.Unchanged 'Unenabled
            End If
         End Get
      End Property
      Public Sub Refresh()
         Dim sPrefix As String = String.Empty
         If miLanduseID <> miColorSchemeID Then
            sPrefix = Convert.ToString(miColorSchemeID) & "/"
         End If
         MyBase.Text = sPrefix & Convert.ToString(miLanduseID) & DMAcadExt.ColorScheme.NameDelim & msColorSchemeName ' mtColorScheme.ID_Name
      End Sub
      Public Sub SetDetailCode()
         '
         MyBase.Text = String.Format("{0,5}", miLanduseID)
         ' Convert.ToString(miLanduseID)
      End Sub
      Public Property SelectedOrChecked() As Boolean
         Get
            If miMarkType = enMarkType.CheckBox Then
               Return MyBase.Checked
            ElseIf miMarkType = enMarkType.Selection Then
               Return MyBase.Selected
            Else
               Return Nothing
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

   Private Sub lvwLanduses_ColumnClick(oSender As System.Object, e As ColumnClickEventArgs) Handles lvwLanduses.ColumnClick
      Dim iDetailSortType As enDetailSortType
      Dim bDescending As Boolean
      Dim oColumnHeader As System.Windows.Forms.ColumnHeader = Me.lvwLanduses.Columns.Item(e.Column)

      Select Case oColumnHeader.Name
         Case Me.lvcLanduseCode.Name
            iDetailSortType = enDetailSortType.ByLanduseCode
         Case Me.lvcLanduseName.Name
            iDetailSortType = enDetailSortType.ByLanduseName
         Case Me.lvcLanduseIndex.Name
            iDetailSortType = enDetailSortType.ByOrder
         Case Me.lvcColorSchemeID.Name
            iDetailSortType = enDetailSortType.ByColorSchemeID
         Case Me.lvcColorSchemeName.Name
            iDetailSortType = enDetailSortType.ByColorSchemeName

      End Select
      If iDetailSortType = miDetailSortType Then
         bDescending = Not mbDescending
      End If


      Me.lvwLanduses.ListViewItemSorter = New ItemComparerDetails(iDetailSortType, bDescending)
      '   Me.lvwLanduses.Sorting = SortOrder.Ascending
      Me.lvwLanduses.Sort()
      miDetailSortType = iDetailSortType
      mbDescending = bDescending
      '    MessageBox.Show(e.Column.ToString() & vbCrLf & DirectCast(oColumnHeader.Tag, String) & vbCrLf & oColumnHeader.Text & vbCrLf & iDetailSortType.ToString(), "07_309")

   End Sub

   Private Sub lvwLanduses_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles lvwLanduses.DoubleClick
      zzEditColorScheme()
   End Sub

   Private Sub frmEditColorSet_FormClosing(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      Try
         moParams.SetValue(0, mdColorScale)
         moParams.Update()
      Catch oEx As Exception
      End Try
   End Sub



   Private Sub frmSelect_Resize(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Resize
      '  MessageBox.Show(CStr(Me.Size.Width) & ":" & CStr(Me.Size.Height) & vbCrLf & Me.AutoScaleBaseSize.Width.ToString() & vbCrLf & Me.AutoScaleBaseSize.Height.ToString(), "04_455")
      Return
      If Me.Size.Width <> 942 OrElse Me.Size.Height <> 480 Then

         Me.Size = New Size(942, 480)
      End If

      Me.lvwLanduses.Height = Me.ClientSize.Height - miListViewTop
      MessageBox.Show(CStr(Me.lvwLanduses.Height) & ":" & Me.lvwLanduses.Top & vbCrLf & Me.ClientSize.Height & ":" & CStr(miListViewTop), "03_549")
   End Sub


   Private Class ItemComparerByLocation
      Implements System.Collections.Generic.IComparer(Of ListViewItemExt)
      Private miViewType As System.Windows.Forms.View
      Public Sub New(ByVal iViewType As System.Windows.Forms.View)
         miViewType = iViewType
      End Sub
      Public Function Compare(ByVal oItemA As ListViewItemExt, ByVal oItemB As ListViewItemExt) As Integer Implements System.Collections.Generic.IComparer(Of ListViewItemExt).Compare
         Dim tPointA As Drawing.Point = oItemA.Position
         Dim tPointB As Drawing.Point = oItemB.Position
         Select Case miViewType
            Case Windows.Forms.View.LargeIcon
               If tPointA.Y < tPointB.Y Then
                  Return -1
               ElseIf tPointA.Y > tPointB.Y Then
                  Return 1
               ElseIf tPointA.X < tPointB.X Then
                  Return -1
               ElseIf tPointA.X > tPointB.X Then
                  Return 1
               Else
                  Return 0
               End If
            Case View.List
               If tPointA.X < tPointB.X Then
                  Return -1
               ElseIf tPointA.X > tPointB.X Then
                  Return 1
               ElseIf tPointA.Y < tPointB.Y Then
                  Return -1
               ElseIf tPointA.Y > tPointB.Y Then
                  Return 1
               Else
                  Return 0
               End If
            Case Else

               Return 0
         End Select
      End Function
   End Class
   Private Enum enDetailSortType
      ByOrder
      ByLanduseCode
      ByLanduseName
      ByColorSchemeID
      ByColorSchemeName
   End Enum
   Private Class ItemComparerDetails
      Implements IComparer


      Private miDetailSortType As enDetailSortType
      Private mbDescending As Boolean
      Public Sub NewAAA()
         miDetailSortType = enDetailSortType.ByOrder
      End Sub

      Public Sub New(ByVal iDetailSortType As enDetailSortType, bDescending As Boolean)
         miDetailSortType = iDetailSortType
         mbDescending = bDescending
      End Sub


      Public Function Compare(oItemA As System.Object, oItemB As System.Object) As Integer Implements IComparer.Compare
         Dim oListViewItemA As ListViewItemExt = DirectCast(oItemA, ListViewItemExt)
         Dim oListViewItemB As ListViewItemExt = DirectCast(oItemB, ListViewItemExt)
         Dim iCompare As Integer
         Select Case miDetailSortType
            Case enDetailSortType.ByLanduseCode
               iCompare = oListViewItemA.LanduseID.CompareTo(oListViewItemB.LanduseID)
            Case enDetailSortType.byLanduseName
               iCompare = oListViewItemA.LanduseName.CompareTo(oListViewItemB.LanduseName)
            Case enDetailSortType.ByOrder
               iCompare = oListViewItemA.OrderNumber.CompareTo(oListViewItemB.OrderNumber)
            Case enDetailSortType.ByColorSchemeID
               iCompare = oListViewItemA.ColorSchemeID.CompareTo(oListViewItemB.ColorSchemeID)
            Case enDetailSortType.ByColorSchemeName
               iCompare = oListViewItemA.ColorSchemeName.CompareTo(oListViewItemB.ColorSchemeName)
         End Select
         If mbDescending Then
            Return -iCompare
         Else
            Return iCompare
         End If


      End Function
   End Class
   Private Sub lvwLanduses_MouseDown(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) 'Handles lvwLanduses.MouseDown
      Dim oGraphics As Graphics = Nothing
      Dim r As Rectangle = New Rectangle(Me.pcbImage.Location, Me.pcbImage.Size)
      '''''''''''''		zzPaint(oGraphics, False)
      Me.Cursor.Draw(oGraphics, r)

   End Sub



   Private Sub tsiAddStandard_Click(oSender As System.Object, e As System.EventArgs) Handles tsiAddStandard.Click
      zzAddLandusesStandard(False)
   End Sub

   Private Sub tsiAddAll_Click(oSender As System.Object, e As System.EventArgs) Handles tsiAddAll.Click
      zzAddLandusesAll(False)
   End Sub

   Private Sub tsiAddProject_Click(oSender As System.Object, e As System.EventArgs) Handles tsiAddProject.Click
      zzAddProjectSet()
   End Sub




   Private Sub frmEditColorSet_Shown(oSender As System.Object, e As System.EventArgs) Handles Me.Shown
      '  MessageBox.Show(CStr(Me.Size.Width) & ":" & CStr(Me.Size.Height) & vbCrLf & Me.AutoScaleBaseSize.Width.ToString() & vbCrLf & Me.AutoScaleBaseSize.Height.ToString(), "04_486")
      zzFillLanduseView()
      '  MessageBox.Show(CStr(Me.Size.Width) & ":" & CStr(Me.Size.Height) & vbCrLf & Me.AutoScaleBaseSize.Width.ToString() & vbCrLf & Me.AutoScaleBaseSize.Height.ToString(), "04_488")
      '	Me.Location = New System.Drawing.Point(0, 0)
      '	Me.Size = New Size(942, 480)
   End Sub

   Private Sub tsiPrjLanduses_Click(oSender As System.Object, e As System.EventArgs) Handles tsiPrjLanduses.Click
      zzAddPrjFeatures()
   End Sub

   Private Sub zzAddPrjFeatures()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
      TopoManager.TPlanGraph.TplnParcel.Initialize(mtMapThemeData)

      Dim iTest As Integer = 0
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew(mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers)
      '  MessageBox.Show(miMapThemeID.ToString() & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(mdicLanduseIDs.Count), "01_768c")
      '     DMCommon.ExcelLog.Open()
      DMAcadExt.AcadTransaction.OpenHandleDictionary()
      If colCentroids IsNot Nothing AndAlso colCentroids.Count <> 0 Then
         Dim tLotData As TopoManager.TPlanGraph.LotData
         Dim tParcelData As TopoManager.TPlanGraph.ParcelData
         Dim iFeatureID As Integer
         For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colCentroids
            If miMapThemeID = DMAcadExt.enMapTheme.LotApproved OrElse miMapThemeID = DMAcadExt.enMapTheme.LotProposed Then
               tLotData = TopoManager.TPlanGraph.TplnLot.GetLotData(mtMapThemeData.TopoPurpose, tAcObjID)
               iFeatureID = tLotData.LanduseID
            ElseIf miMapThemeID = DMAcadExt.enMapTheme.Parcels Then
               tParcelData = TopoManager.TPlanGraph.TplnParcel.GetParcelData(tAcObjID)
               iFeatureID = tParcelData.Owner
               '  MessageBox.Show(iFeatureID.ToString() & vbCrLf & tAcObjID.ToString() & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(mdicLanduseIDs.Count), "01_777c")
            End If

            DMAcadExt.AcadDocument.WriteDebugMessage("AddPrj_" & CStr(tLotData.LanduseID))
            If Not mdicLanduseIDs.ContainsKey(tLotData.LanduseID) Then
               zzAddLanduseOne(iFeatureID, True, True)
               iTest += 1
            End If

         Next
      Else
         MessageBox.Show(mtMapThemeData.CentroidBlocks & ":" & mtMapThemeData.CentroidLayers, "01_769")
      End If
      ' MessageBox.Show(CStr(colCentroids.Count) & ":" & CStr(mdicLanduseIDs.Count) & ":" & CStr(itest), "01_768z")
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()

      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Private Sub zzSetLargeImages(bYes As Boolean)
      If bYes AndAlso Not mbPictureBoxLarge Then
         Me.lvwLanduses.LargeImageList = Me.imlExtraLarge
         mbPictureBoxLarge = True
      ElseIf Not bYes AndAlso mbPictureBoxLarge Then
         Me.lvwLanduses.LargeImageList = Me.imlLarge
         mbPictureBoxLarge = False

      End If
   End Sub

   Private Sub zzAddLanduse()
		mfColorEditor = New frmColorSchemeEditor(miMapThemeID, enColorEditorMode.Landuse, True)
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfColorEditor)
      mfColorEditor.AllowAddToSet = True
   End Sub
   Private Sub tsiAddLanduse_Click(oSender As System.Object, e As System.EventArgs) Handles tsiAddLanduse.Click
      zzAddLanduse()
   End Sub
   Private Sub mfColorEditor_AddLanduse(iLanduseID As Integer, iColorSchemeID As Integer) Handles mfColorEditor.AddLanduse
      If Not mdicLanduseIDs.ContainsKey(iLanduseID) Then
         zzAddColorSet(iLanduseID, iColorSchemeID)
         zzAddLanduseOne(iLanduseID, False, True)
      End If
   End Sub

   Private Sub tsiColorScale_DropDownItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tsiColorScale.DropDownItemClicked
      Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
      zzClearColorScaleCheced()
      zzSetColorScale(oToolStripItem.Name)

   End Sub
   Private Sub zzSetColorScale(sItemName As String)
      Dim saVal() As String = Split(sItemName, "_")
      If saVal.GetUpperBound(0) = 1 AndAlso IsNumeric(saVal(1)) Then
         mdColorScale = Convert.ToDouble(saVal(1))
         zzRepaint()
      Else
         MessageBox.Show(sItemName, "01_991")
      End If
   End Sub
   Private Sub zzInitColorScale()

      mdColorScale = moParams.GetDblValue(0)
      Dim iColorScale As Integer = CInt(mdColorScale)
      Select Case iColorScale
         Case 1
            Me.tsiColorScale_1.Checked = True
         Case 2
            Me.tsiColorScale_2.Checked = True
         Case 4
            Me.tsiColorScale_4.Checked = True
         Case 6
            Me.tsiColorScale_6.Checked = True
         Case 8
            Me.tsiColorScale_8.Checked = True
      End Select
   End Sub


   Private Sub tsiLargeIcons_Click(oSender As System.Object, e As EventArgs) Handles tsiLargeIcons.Click

   End Sub

   Private Sub tsbDown_Click(oSender As System.Object, e As EventArgs) Handles tsbDown.Click

   End Sub

	Private Sub tsiAddExpro_Click(oSender As System.Object, e As EventArgs) Handles tsiAddExpro.Click
		zzAddExpro(False)
	End Sub
	Private Sub zzAddExpro(bChecked As Boolean)
		Dim sComText As String = "SELECT * FROM dbo.ExproColorSchemes"

		zzAddLanduseView(sComText, True, bChecked)
		Me.tsbSet.Checked = True
		zzSetEditing(True)
	End Sub

	Private Sub tsbUpdateSet_Click(oSender As System.Object, e As EventArgs) Handles tsbUpdateSet.Click

	End Sub

	Private Sub tsbViews_Click(sender As Object, e As EventArgs) Handles tsbViews.Click

	End Sub
End Class