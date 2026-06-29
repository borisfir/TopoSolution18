Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map
Public Class frmApp
	Const msRootPath As String = "R:\Gushim\Gushim-Vectorized"
	Private msCurrentRootName As String = msRootPath
	Private msCurrentGushFileName As String = String.Empty
	Private msCurrentParcelFileName As String = String.Empty


	Private msaParcelFolders() As String
	Private moFDO_Manager As FDO.FDO_Manager
	Private mbFolder As Boolean = True
	Private mbParcel As Boolean


	Private Sub cmdSelectFolders_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectFolders.Click
		If mbFolder Then
			zzFolder()
		Else
			zzFile()
		End If

	End Sub

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()

	End Sub
	Public ReadOnly Property RootPath As String
		Get
			Return msRootPath
		End Get
	End Property
	Public ReadOnly Property ParcelFolders As String()
		Get
			Return msaParcelFolders
		End Get
	End Property
	Private Sub zzMyInitializeComponent()
		Me.txtName.Text = msRootPath
		zzFillParcelFolderList()
	End Sub
	Private Sub zzFolder()
		Dim iDialogResult As DialogResult = Me.fbdParcels.ShowDialog()
		If iDialogResult = Windows.Forms.DialogResult.OK Then
			'Me.clbParcels.CheckOnClick = False
			Dim sSelectedPath As String = Me.fbdParcels.SelectedPath
			If Not String.IsNullOrEmpty(sSelectedPath) Then
				Me.txtName.Text = sSelectedPath
				msCurrentRootName = sSelectedPath
				Me.clbParcels.Items.Clear()

				zzFillParcelFolderList()
			End If
		End If

	End Sub
	Private Sub zzFile()
		Dim iDialogResult As DialogResult = Me.opdParcels.ShowDialog()
		Dim sFileName As String = opdParcels.FileName

		Me.txtName.Text = sFileName

		If mbParcel Then
			msCurrentParcelFileName = sFileName
			zzFillParcelFileList()
		Else
			msCurrentGushFileName = sFileName
			zzFillGushFileList()
		End If
	End Sub
	Private Sub zzFillGushFileList()
		Dim iItemIndex As Integer
		Me.clbParcels.Items.Clear()

		'\\Titan\DM_APP\Tababuild\ProjectsNet\Shape\Road9
		If msCurrentGushFileName.Length <> 0 Then
			Dim oFile As IO.FileInfo = Nothing
			Try
				oFile = New IO.FileInfo(msCurrentGushFileName)
			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & msCurrentGushFileName, "File Name")
			End Try
			If oFile IsNot Nothing Then
				Dim sFileName As String = oFile.Name
				Dim sClassName As String = zzGetClassName(sFileName, oFile.Extension)  ' sFileName.Substring(0, sFileName.Length - oFile.Extension.Length)


            Dim dicRes As Dictionary(Of Integer, FDO.GushData) = FDO.TplnPolygonSet.GetGushData(msCurrentGushFileName, sClassName, New DMCommon.dmList())
            If dicRes IsNot Nothing Then
               Dim dicBlocks As System.Collections.Generic.SortedDictionary(Of Integer, Integer) = New System.Collections.Generic.SortedDictionary(Of Integer, Integer)()
               For Each oGushData As FDO.GushData In dicRes.Values
                  If Not dicBlocks.ContainsKey(oGushData.Block) Then
                     dicBlocks.Add(oGushData.Block, oGushData.Block)
                  End If
               Next

               For Each iBlockNum As Integer In dicBlocks.Values
                  iItemIndex = Me.clbParcels.Items.Add(iBlockNum)
                  Me.clbParcels.SetItemChecked(iItemIndex, True)
               Next
            End If

         End If
      End If

   End Sub

   Private Function zzGetClassName(ByVal sFileName As String, ByVal sFileExtension As String) As String
      Return sFileName.Substring(0, sFileName.Length - sFileExtension.Length)
   End Function
   Private Sub zzFillParcelFileList()
      Me.clbParcels.Items.Clear()
      Me.clbParcels.CheckOnClick = False
      '\\Titan\DM_APP\Tababuild\ProjectsNet\Shape\Road9
      If msCurrentParcelFileName.Length <> 0 Then
         Dim oFile As IO.FileInfo = New IO.FileInfo(msCurrentParcelFileName)
         Dim sFileName As String = oFile.Name
         Dim sClassName As String = zzGetClassName(sFileName, oFile.Extension)
         Dim iItemIndex As Integer
         Dim dicParcelData As Dictionary(Of Integer, FDO.ParcelData) = Nothing
         Dim dicGushFromParcelData As Dictionary(Of Integer, FDO.GushData) = Nothing

         Dim bRes As Boolean '''''''''''''''''''''''''''''''''''''''TEMP = FDO.TplnPolygonSet.GetParcelData(msCurrentParcelFileName, sClassName, New DMCommon.dmList(), dicParcelData, dicGushFromParcelData)
         If bRes AndAlso dicParcelData IsNot Nothing Then

            Dim dicBlocks As System.Collections.Generic.SortedDictionary(Of Integer, Integer) = New System.Collections.Generic.SortedDictionary(Of Integer, Integer)()
            For Each oParcelData As FDO.ParcelData In dicParcelData.Values
               If Not dicBlocks.ContainsKey(oParcelData.Block) Then
                  dicBlocks.Add(oParcelData.Block, oParcelData.Block)
               End If
            Next

            '	MessageBox.Show(CStr(dicBlocks.Count), "01_694")
            For Each iBlockNum As Integer In dicBlocks.Values
               iItemIndex = Me.clbParcels.Items.Add(iBlockNum)
               Me.clbParcels.SetItemChecked(iItemIndex, True)
            Next
            '	Me.clbParcels.ReferenceEqua = False
         End If
      End If
   End Sub
	Private Sub zzFillParcelFolderList()
		Me.clbParcels.Items.Clear()
		If msCurrentRootName IsNot Nothing Then
			Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(msCurrentRootName)
			Dim oaParcelFolders() As IO.DirectoryInfo
			Dim oList As List(Of TopoManager.NumerationPair.ComplexNum) = New List(Of TopoManager.NumerationPair.ComplexNum)
			Dim oComparer As MyComparer = New MyComparer

			oaParcelFolders = oRootFolder.GetDirectories()
			If oaParcelFolders IsNot Nothing Then
				For iIndex As Integer = 0 To oaParcelFolders.GetUpperBound(0)
					'	Me.clbParcels.Items.Add(oParcelFolders(iIndex).Name)
					oList.Add(New TopoManager.NumerationPair.ComplexNum(oaParcelFolders(iIndex).Name))
				Next
				oList.Sort(oComparer)
				For iIndex As Integer = 0 To oList.Count - 1
					Me.clbParcels.Items.Add(oList.Item(iIndex).Source())
				Next
			End If

		End If
	End Sub

	Private Sub frmApp_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
			Me.Location = New System.Drawing.Point(0, 0)
		End If
	End Sub


	Private Sub frmApp_Resize(oSender As System.Object, e As System.EventArgs) Handles Me.Resize
		Me.clbParcels.Height = Me.ClientSize.Height - 80
	End Sub

	Private Sub cmdOK_Click(sender As System.Object, e As System.EventArgs) Handles cmdOK.Click
		Me.Cursor = Cursors.WaitCursor
		If mbFolder Then
			Dim colCheckedItems As System.Windows.Forms.CheckedListBox.CheckedItemCollection = Me.clbParcels.CheckedItems
			ReDim msaParcelFolders(colCheckedItems.Count - 1)
			MessageBox.Show(CStr(colCheckedItems.Count), "07_216")
			colCheckedItems.CopyTo(msaParcelFolders, 0)
			moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
			moFDO_Manager.imp_Parcels()
			MessageBox.Show(CStr(colCheckedItems.Count), "07_290")
		Else
			MessageBox.Show("", "07_180")
			If Me.txtName.Text.Length <> 0 Then
				Dim oFileInfo As IO.FileInfo = New IO.FileInfo(Me.txtName.Text)
            Dim sShapeFileName As String, sShapeFolderName As String, sFileName As String
            Dim iMapThemeID As DMAcadExt.enMapTheme
				If oFileInfo.Exists Then
					sShapeFileName = oFileInfo.FullName
					sShapeFolderName = oFileInfo.DirectoryName
					sFileName = oFileInfo.Name
					Dim sClassName As String = zzGetClassName(sFileName, oFileInfo.Extension)
					System.Windows.Forms.MessageBox.Show(sClassName, "03_190")
               moFDO_Manager = New FDO.FDO_Manager()
               If mbParcel Then
                  iMapThemeID = DMAcadExt.enMapTheme.Parcels
               Else
                  iMapThemeID = DMAcadExt.enMapTheme.Blocks
               End If
					moFDO_Manager.imp_Set(sShapeFileName, sShapeFolderName, sClassName, Nothing, Nothing, iMapThemeID, New DMCommon.dmList(), True)
				End If
			End If
		End If
		'''''''''''''''''''''''''''''''''''''''''''		Me.tmrDelay.Enabled = True
		Me.Cursor = Cursors.Default
		Me.cmdOK.Enabled = False
	End Sub
	Private Sub zzSetDelayInterval(dFileCount As Integer)
		Dim iInterval As Integer = CInt(Math.Ceiling(dFileCount * 0.66) * 1000.0)
		If iInterval < 2000 Then
			iInterval = 2000
		End If
		Me.tmrDelay.Interval = iInterval
	End Sub

	Private Class MyComparer
		Implements IComparer(Of TopoManager.NumerationPair.ComplexNum)

		Public Function Compare(x As TopoManager.NumerationPair.ComplexNum, y As TopoManager.NumerationPair.ComplexNum) As Integer Implements System.Collections.Generic.IComparer(Of TopoManager.NumerationPair.ComplexNum).Compare
			If x.Order > y.Order Then
				Return 1
			ElseIf x.Order = y.Order Then
				Return 0
			Else
				Return -1
			End If
		End Function
	End Class

	Private Sub tmrDelay_Tick(oSender As System.Object, e As System.EventArgs) Handles tmrDelay.Tick
		Me.tmrDelay.Enabled = False

		If mbFolder Then
			moFDO_Manager.CreateBlocks("1601", "1601")
			Me.cmdOK.Enabled = True
		End If

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		FDO.FDO_Manager.RemoveAllConnections()
		FDO.Util.ClearAllResources()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CommandLine(True)
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub

	Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
		Me.Close()
	End Sub

	Private Sub rdbFolder_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbFolder.CheckedChanged
		If rdbFolder.Checked Then
			mbFolder = True
			Me.txtName.Text = msCurrentRootName
			If msCurrentRootName.Length <> 0 Then
				zzFillParcelFolderList()
			End If
			'		Me.clbParcels.Enabled = True
			Me.clbParcels.SelectionMode = SelectionMode.One
			Me.cmdClear.Enabled = True
		End If
	End Sub

	Private Sub rdbBlockFile_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbBlockFile.CheckedChanged
		If rdbBlockFile.Checked Then
			mbFolder = False
			mbParcel = False
			Me.txtName.Text = msCurrentGushFileName
			Me.txtFileList.Text = String.Empty
			'		Me.clbParcels.Enabled = False
			Me.clbParcels.SelectionMode = SelectionMode.None
			Me.cmdClear.Enabled = False
			zzFillGushFileList()
		End If
	End Sub

	Private Sub rdbParcelFile_CheckedChanged(oSender As System.Object, e As System.EventArgs) Handles rdbParcelFile.CheckedChanged
		If rdbParcelFile.Checked Then
			mbFolder = False
			mbParcel = True
			Me.txtFileList.Text = String.Empty
			Me.txtName.Text = msCurrentParcelFileName
			'		Me.clbParcels.Enabled = False
			Me.clbParcels.SelectionMode = SelectionMode.None
			Me.cmdClear.Enabled = False
			zzFillParcelFileList()
		End If
	End Sub


	Private Sub clbParcels_ItemCheck(oSender As System.Object, e As System.Windows.Forms.ItemCheckEventArgs) Handles clbParcels.ItemCheck
		'x As TopoManager.NumerationPair.ComplexNum
		If rdbFolder.Checked Then
			Dim iCheck As System.Windows.Forms.CheckState
			Dim bPlus As Boolean = (e.NewValue = CheckState.Checked)
			Dim bMinus As Boolean = (e.NewValue = CheckState.Unchecked)
			'	MessageBox.Show(CStr(bPlus) & ":" & CStr(bMinus), "07_999")

			Dim oValue As System.Object = Me.clbParcels.Items(e.Index)
			Dim sValue As String = Convert.ToString(oValue)
			Dim iGushNo As TopoManager.NumerationPair.ComplexNum
			Dim iCurrent As TopoManager.NumerationPair.ComplexNum
			Dim colCheckedItems As System.Windows.Forms.CheckedListBox.CheckedItemCollection = Me.clbParcels.CheckedItems
			Dim iCount As Integer = colCheckedItems.Count

			iGushNo = New TopoManager.NumerationPair.ComplexNum(sValue)

			'	MessageBox.Show(o.ToString() & vbCrLf & o.GetType().ToString, "03_987")

			If colCheckedItems.Count >= 0 Then
				ReDim msaParcelFolders(colCheckedItems.Count - 1)
				Dim sList As String = String.Empty
				Dim bStop As Boolean

				colCheckedItems.CopyTo(msaParcelFolders, 0)
				For iIndex As Integer = 0 To msaParcelFolders.GetUpperBound(0)
					bStop = False
					If bPlus Then

						iCurrent = New TopoManager.NumerationPair.ComplexNum(msaParcelFolders(iIndex))
						If iCurrent.Order > iGushNo.Order Then
							If sList.Length <> 0 Then
								sList &= ","
							End If
							sList &= iGushNo.Source()
							iCount += 1
							bPlus = False
						End If

					End If
			If bMinus Then

						iCurrent = New TopoManager.NumerationPair.ComplexNum(msaParcelFolders(iIndex))
						If iCurrent.Order = iGushNo.Order Then
							bStop = True
							iCount -= 1
							bMinus = False
						End If

					End If
					If Not bStop Then
						If sList.Length <> 0 Then
							sList &= ","
						End If
						sList &= msaParcelFolders(iIndex)
					End If
				Next
				If bPlus Then
					If sList.Length <> 0 Then
						sList &= ","
					End If
					sList &= iGushNo.Source()
					iCount += 1
					bPlus = False
				End If
				If iCount > 0 Then
					Me.txtFileList.Text = CStr(iCount) & ": " & sList
				Else
					Me.txtFileList.Text = String.Empty
				End If

				If Not mbFolder Then
					DMAcadExt.AcadDocument.WriteMessage((e.NewValue).ToString() & ":" & CStr(e.Index))
					If e.NewValue = CheckState.Unchecked Then
						Me.clbParcels.SetItemChecked(e.Index, True)
						Me.clbParcels.SetItemCheckState(e.Index, CheckState.Checked)
						iCheck = Me.clbParcels.GetItemCheckState(e.Index)
						DMAcadExt.AcadDocument.WriteMessage("After " & iCheck.ToString())

					End If
				End If
			Else

			End If
		Else

			Dim bPlus As Boolean = (e.NewValue = CheckState.Checked)
			Dim bMinus As Boolean = (e.NewValue = CheckState.Unchecked)
			'	MessageBox.Show(CStr(bPlus) & ":" & CStr(bMinus), "07_999")

			Dim oValue As System.Object = Me.clbParcels.Items(e.Index)
			Dim sValue As String = Convert.ToString(oValue)

		End If



	End Sub

	 

	Private Sub clbParcels_SelectedIndexChanged(oSender As System.Object, e As System.EventArgs)	' Handles clbParcels.SelectedIndexChanged
		DMAcadExt.AcadDocument.WriteMessage("Index  " & oSender.ToString())
		DMAcadExt.AcadDocument.WriteMessage("SelectedIndex  " & clbParcels.SelectedIndex.ToString())
		Dim iCheck As System.Windows.Forms.CheckState
		Dim iIndex As Integer = clbParcels.SelectedIndex
		iCheck = Me.clbParcels.GetItemCheckState(iIndex)
		If iCheck = CheckState.Unchecked Then
			Me.clbParcels.SetItemCheckState(iIndex, CheckState.Checked)
		End If

	End Sub

	Private Sub cmdClear_Click(oSender As System.Object, e As System.EventArgs) Handles cmdClear.Click
		'	Dim o As CheckedListBox.CheckedItemCollection = Me.clbParcels.CheckedItems
		Dim colCheckedIndecis As CheckedListBox.CheckedIndexCollection = Me.clbParcels.CheckedIndices
		For Each iIndex As Integer In colCheckedIndecis
			Me.clbParcels.SetItemChecked(iIndex, False)
		Next
		 
		Me.txtFileList.Text = String.Empty
	End Sub

	Private Sub frmApp_Shown(oSender As System.Object, e As System.EventArgs) Handles Me.Shown
		'	MessageBox.Show(CStr(Me.Location.X) & ":" & CStr(Me.Location.Y), "01_056")
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(0, 0)
		End If
	End Sub

	Private Sub cmdInsertBlocks_Click(sender As System.Object, e As System.EventArgs) Handles cmdInsertBlocks.Click
		moFDO_Manager.CreateBlocksUnion("1601", "1601")
		Me.cmdOK.Enabled = True
	End Sub
End Class