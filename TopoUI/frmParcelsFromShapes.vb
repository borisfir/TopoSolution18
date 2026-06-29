Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map
Public Class frmParcelsFromShapes
	' Private msAllParcelFile As String = "R:\Gushim\Gushim-MMG-ISRAEL\2017\2017.05\GIS\parcel\PARCEL_ALL.shp"
	Private msAllParcelRootFolder As String = "R:\Gushim\Gushim-MMG-ISRAEL"
	Private msAllParcelPathInRoot As String = "\GIS\parcel\PARCEL_ALL.shp"
	Private msAllParcelPath As String = "\GIS\parcel"




	Protected ptMapThemeData As DMAcadExt.MapThemeData
	Protected ptDissolveMapThemeData As DMAcadExt.MapThemeData


   Private msCurrentRootName As String
	Private msCurrentGushFileName As String = String.Empty
	Private msCurrentParcelFileName As String = String.Empty
   Private mdicGushFromParcelData As Dictionary(Of Integer, FDO.GushData) = Nothing

	Private msaParcelFolders() As String
	Private moFDO_Manager As FDO.FDO_Manager
	Private mbFolder As Boolean = True
   Private mbParcel As Boolean
   Private mtFilterList As DMCommon.dmList
	Private miPoligonCount As Integer
	Private msDissolveBlockName As String
	Private msDissolveBlockLayer As String
	Private miStage As Integer = 0
   Private mhsGush As HashSet(Of Integer)
   Private mbEventsEnabled As Boolean = False
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tDissolveMapThemeData As DMAcadExt.MapThemeData)
      ptMapThemeData = tMapThemeData
		ptDissolveMapThemeData = tDissolveMapThemeData
		' This call is required by the designer.
		InitializeComponent()
      msCurrentRootName = TopoManager.TPlanGraph.TplnProject.GushimVectorizedRootPath
		' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()


      Me.DialogResult = Windows.Forms.DialogResult.No
	End Sub
	 
	Public ReadOnly Property CurrentRootName As String
		Get
			Return msCurrentRootName
		End Get
	End Property
	Public ReadOnly Property ParcelFolders As String()
		Get
			Return msaParcelFolders
		End Get
	End Property
	Public ReadOnly Property CurrentParcelFileName As String
		Get
			Return msCurrentParcelFileName
		End Get
	End Property
	Public ReadOnly Property CurrentGushFileName As String
		Get
			Return msCurrentGushFileName
		End Get
	End Property
	Public ReadOnly Property GushSet As HashSet(Of Integer)
		Get
			Return mhsGush
		End Get
   End Property
   Public ReadOnly Property GushFromParcelData As Dictionary(Of Integer, FDO.GushData)
      Get
         Return mdicGushFromParcelData
      End Get
   End Property

	Public ReadOnly Property InnerPolygons As Boolean
		Get
			Return Me.chkInnerPolygons.Checked
		End Get
	End Property
	Public ReadOnly Property PoligonCount As Integer
		Get
			Return miPoligonCount
		End Get
	End Property
	Public ReadOnly Property Folder As Boolean
		Get
			Return mbFolder
		End Get
	End Property
   Public ReadOnly Property MapThemeID As DMAcadExt.enMapTheme
      Get
         If mbParcel Then
            Return DMAcadExt.enMapTheme.Parcels
         Else
            Return DMAcadExt.enMapTheme.Blocks
         End If

      End Get
   End Property
   Public ReadOnly Property FilterList As DMCommon.dmList
      Get
         Return mtFilterList
      End Get
   End Property

   Public ReadOnly Property Parcel As Boolean
      Get
         Return mbParcel
      End Get
   End Property



	Private Sub zzMyInitializeComponent()
      Me.txtName.Text = msCurrentRootName
		zzFillParcelFolderList()
	End Sub
	Private Sub zzFolder(Optional sFolderName As String = Nothing)
		Dim bContinue As Boolean
		Dim sSelectedPath As String = Nothing
		If String.IsNullOrEmpty(sFolderName) Then
			Dim iDialogResult As DialogResult = Me.fbdParcels.ShowDialog()
			If iDialogResult = Windows.Forms.DialogResult.OK Then
				'Me.clbParcels.CheckOnClick = False
				sSelectedPath = Me.fbdParcels.SelectedPath
				bContinue = True
			End If
		Else
			Dim oFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sFolderName)
			bContinue = oFolder.Exists
			sSelectedPath = oFolder.FullName
		End If
		If bContinue AndAlso Not String.IsNullOrEmpty(sSelectedPath) Then
			Me.txtName.Text = sSelectedPath
			msCurrentRootName = sSelectedPath
			Me.clbParcels.Items.Clear()
			bContinue = True
			zzFillParcelFolderList()
		End If

	End Sub
	Private Sub zzFilterList()
		mtFilterList = New DMCommon.dmList(Me.txtFileList.Text)
		mtFilterList.ItemsSplit("/")
	End Sub
	Private Sub zzFilterList_Try()
		Dim iBlockNumCorrect As Integer
		Dim saCorrectNum() As String
		mtFilterList = New DMCommon.dmList(Me.txtFileList.Text)
		If mtFilterList.UpperBound >= 0 Then
			saCorrectNum = mtFilterList.Values
			For iIndex As Integer = 0 To saCorrectNum.GetUpperBound(0)

				If Integer.TryParse(saCorrectNum(iIndex), iBlockNumCorrect) Then
					If iBlockNumCorrect > Short.MaxValue Then
						iBlockNumCorrect = iBlockNumCorrect - UShort.MaxValue - 1
						saCorrectNum(iIndex) = iBlockNumCorrect.ToString
						DMCommon.Debug.MsgBox("13_101", saCorrectNum(iIndex))
					End If
				End If

			Next
			mtFilterList = New DMCommon.dmList(saCorrectNum)
		End If


	End Sub
	Private Sub zzFile(Optional sFileName As String = Nothing)
      Dim bContinue As Boolean
      If String.IsNullOrEmpty(sFileName) Then
         Dim iDialogResult As DialogResult = Me.opdParcels.ShowDialog()
         If iDialogResult = Windows.Forms.DialogResult.OK Then
            sFileName = opdParcels.FileName
            bContinue = True
         End If
      Else

         Dim oFile As IO.FileInfo = New IO.FileInfo(sFileName)
         bContinue = oFile.Exists
      End If
		'DMCommon.Debug.MsgBox("11_260", sFileName, bContinue)
		If bContinue Then
			zzFilterList()
			Me.txtName.Text = sFileName

         If mbParcel Then
            msCurrentParcelFileName = sFileName
            zzFillParcelFileList()
         Else
            msCurrentGushFileName = sFileName
            zzFillGushFileList()
         End If
      End If
   
   End Sub


      
   Private Function zzGetLastFolderPath(sRootPath As String) As String
      Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRootPath)
      Dim oaYearFolders As System.IO.DirectoryInfo()
      Dim oYearFolder As System.IO.DirectoryInfo
      Dim oMaxYearFolder As System.IO.DirectoryInfo = Nothing

      Dim oaMonthFolders As System.IO.DirectoryInfo()
      Dim oMonthFolder As System.IO.DirectoryInfo

      Dim oMaxMonthFolder As System.IO.DirectoryInfo = Nothing
      Dim saMonthDate() As String
      Dim iYear As Integer
      Dim iMaxYear As Integer = 0
      Dim iMonth As Integer
      Dim iMaxMonth As Integer = 0
      Dim sMonthDelim() As Char = {Convert.ToChar(".")}
      If oRootFolder.Exists Then
         oaYearFolders = oRootFolder.GetDirectories()
         For iYearIndex As Integer = 0 To oaYearFolders.GetUpperBound(0)
            oYearFolder = oaYearFolders(iYearIndex)
            If Integer.TryParse(oYearFolder.Name, iYear) Then
               If iYear > iMaxYear Then
                  iMaxYear = iYear
                  oMaxYearFolder = oYearFolder
               End If

            End If
         Next
         If oMaxYearFolder IsNot Nothing Then


            oaMonthFolders = oMaxYearFolder.GetDirectories()
            For iMonthIndex As Integer = 0 To oaMonthFolders.GetUpperBound(0)
               oMonthFolder = oaMonthFolders(iMonthIndex)
               saMonthDate = oMonthFolder.Name.Split(sMonthDelim, StringSplitOptions.RemoveEmptyEntries)
               If saMonthDate.GetUpperBound(0) = 1 AndAlso Integer.TryParse(saMonthDate(1), iMonth) Then
                  If iMonth > iMaxMonth Then
                     iMaxMonth = iMonth
                     oMaxMonthFolder = oMonthFolder
                  End If

               End If
            Next
            If oMaxMonthFolder IsNot Nothing Then
               Return oMaxMonthFolder.FullName
            End If


         End If

      End If
      Return Nothing

   End Function

	Private Sub zzFillGushFileList()
		Dim iItemIndex As Integer
		Me.clbParcels.Items.Clear()

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
            Dim dicRes As Dictionary(Of Integer, FDO.GushData) = FDO.TplnPolygonSet.GetGushData(msCurrentGushFileName, sClassName, mtFilterList)
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
		Dim dicParcelData As Dictionary(Of Integer, FDO.ParcelData) = Nothing

		If msCurrentParcelFileName.Length <> 0 Then
			Dim oFile As IO.FileInfo = New IO.FileInfo(msCurrentParcelFileName)
			Dim sFileName As String = oFile.Name
			Dim sClassName As String = zzGetClassName(sFileName, oFile.Extension)
			Dim iItemIndex As Integer
			Dim bRes As Boolean = FDO.TplnPolygonSet.GetParcelData(msCurrentParcelFileName, sClassName, Nothing, mtFilterList, dicParcelData, mdicGushFromParcelData, False, False)
			If bRes AndAlso dicParcelData IsNot Nothing Then

				Dim dicBlocks As System.Collections.Generic.SortedDictionary(Of Integer, String) = New System.Collections.Generic.SortedDictionary(Of Integer, String)()
				For Each oParcelData As FDO.ParcelData In dicParcelData.Values
					If Not dicBlocks.ContainsKey(oParcelData.BlockKey) Then
						dicBlocks.Add(oParcelData.BlockKey, oParcelData.BlockName)
					End If
				Next

				'   DMCommon.Debug.MsgBox("01_694!!!!!", msCurrentParcelFileName, mtFilterList.UpperBound, dicBlocks.Count, DMCommon.Debug.ColCount(mdicGushFromParcelData))

				For Each sBlockName As String In dicBlocks.Values
					iItemIndex = Me.clbParcels.Items.Add(sBlockName)
					Me.clbParcels.SetItemChecked(iItemIndex, True)
				Next



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
			If oRootFolder IsNot Nothing AndAlso oRootFolder.Exists Then
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
		End If
	End Sub

	Private Sub frmApp_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
			Me.Location = New System.Drawing.Point(0, 0)
      End If
      mbEventsEnabled = True
	End Sub
	Private Sub frmApp_Resize(oSender As System.Object, e As System.EventArgs) Handles Me.Resize
		Me.clbParcels.Height = Me.ClientSize.Height - 76
      Me.Width = 932 '880
	End Sub
	Private Sub cmdOK_Click(oSender As System.Object, e As System.EventArgs) Handles cmdOK.Click
		Me.Cursor = Cursors.WaitCursor
		If mbFolder Then
			Dim colCheckedItems As System.Windows.Forms.CheckedListBox.CheckedItemCollection = Me.clbParcels.CheckedItems
			ReDim msaParcelFolders(colCheckedItems.Count - 1)

			colCheckedItems.CopyTo(msaParcelFolders, 0)
			'	DMCommon.Functions.DispArray(msaParcelFolders, "1msaParcelFolders", True)


			If False Then ' NewsStyleUriParser VERSION
				moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
				moFDO_Manager.imp_Parcels()
				miPoligonCount += moFDO_Manager.PoligonCount
			End If

		Else
			If Me.txtName.Text.Length <> 0 Then
				Dim oFileInfo As IO.FileInfo = New IO.FileInfo(Me.txtName.Text)
				Dim sShapeFileName As String, sShapeFolderName As String, sFileName As String
				If oFileInfo.Exists Then
					sShapeFileName = oFileInfo.FullName
					sShapeFolderName = oFileInfo.DirectoryName
					sFileName = oFileInfo.Name
					Dim sClassName As String = zzGetClassName(sFileName, oFileInfo.Extension)
					'  DMCommon.Debug.MsgBox("11_262!!!!", Me.txtName.Text, sShapeFileName, sShapeFolderName, sFileName, sClassName, DMCommon.Debug.ColCount(mdicGushFromParcelData))

					'  zzCreateGushList300118()
					'  mhsGush.Clear()
					zzCreateGushListNew()

				End If
			End If
		End If
		Me.Close()
		Me.tmrDelay.Enabled = True
		'''''''''''''''''''''''Me.tmrDelay.Enabled = False

		Me.Cursor = Cursors.Default
		'	Me.cmdOK.Enabled = False
		'	MessageBox.Show(Me.DialogResult.ToString(), "02_452")
   End Sub
   Private Sub zzCreateGushList300118()
      If Not String.IsNullOrEmpty(Me.txtFileList.Text) Then
         Dim saVal() As String = Split(Me.txtFileList.Text, ",")
         Dim iGush As Integer
         mhsGush = New HashSet(Of Integer)
         For iIndex As Integer = 0 To saVal.GetUpperBound(0)
            If Integer.TryParse(saVal(iIndex), iGush) Then
               mhsGush.Add(iGush)

            End If
         Next
         'Me.txtFileList.Text,
         DMCommon.Debug.MsgBox("09_927txt", DMCommon.Debug.GetListArray(mhsGush))

      End If
   End Sub
	Private Sub zzCreateGushListNew()
		'If Not String.IsNullOrEmpty(Me.txtFileList.Text) Then
		'   Dim saVal() As String = Split(Me.txtFileList.Text, ",")

		mhsGush = New HashSet(Of Integer)

		For Each tGushData As FDO.GushData In mdicGushFromParcelData.Values
			' If Integer.TryParse(saVal(iGushNo), iGush) Then
			mhsGush.Add(tGushData.Block)

			'  End If
		Next

		'  DMCommon.Debug.MsgBox("08_444c!!!!!", mhsGush.Count, mdicGushFromParcelData.Count)
		'  DMCommon.Debug.MsgBox("09_927ParData", DMCommon.Debug.GetListArray(mhsGush))

		'  End If
	End Sub
	Private Sub cmdSelectFolders_Click(sender As System.Object, e As System.EventArgs) Handles cmdSelectFolders.Click
		If mbFolder Then
			zzFolder()
		Else
         zzFile()
		End If

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
		Return
		Me.tmrDelay.Enabled = False

		If mbFolder Then
			If miStage = 0 Then

				moFDO_Manager.CreateBlocksUnion(ptDissolveMapThemeData.CentroidBlock, ptDissolveMapThemeData.CentroidLayer)
				Me.cmdOK.Enabled = True
				miStage = 1
				Me.tmrDelay.Enabled = True
			ElseIf miStage = 1 Then
				moFDO_Manager.CreateWorkArea()

				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				FDO.FDO_Manager.RemoveAllConnections()
				FDO.Util.ClearAllResources()
				DMAcadExt.AcadDocument.Unlock()
				DMAcadExt.AcadDocument.CommandLine(True)
				DMAcadExt.AcadDocument.UpdateScreen()

			End If
		Else
			'	mtaShapeFOData()
			If miStage = 0 Then
				'	MessageBox.Show("++" & vbCrLf & "", "03_127s")
				If True Then	'TEMP


					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					FDO.FDO_Manager.RemoveAllConnections()
					FDO.Util.ClearAllResources()
					DMAcadExt.AcadDocument.Unlock()
					DMAcadExt.AcadDocument.CommandLine(True)
					DMAcadExt.AcadDocument.UpdateScreen()
				End If
			ElseIf miStage = 1 Then

			End If


		End If

	End Sub
	Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
		Me.Close()
	End Sub
	Private Sub rdbFolder_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbFolder.CheckedChanged
      If mbEventsEnabled AndAlso rdbFolder.Checked Then
         mbFolder = True

         Me.txtName.Text = msCurrentRootName
         If Not String.IsNullOrEmpty(msCurrentRootName) Then
            zzFillParcelFolderList()
         End If
         '		Me.clbParcels.Enabled = True
         Me.clbParcels.SelectionMode = SelectionMode.One
         Me.cmdClear.Enabled = True
      End If

	End Sub
	Private Sub rdbBlockFile_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbBlockFile.CheckedChanged
      If mbEventsEnabled AndAlso rdbBlockFile.Checked Then
         mbFolder = False
         mbParcel = False
         Me.txtName.Text = msCurrentGushFileName
         Me.txtFileList.Text = String.Empty
         '		Me.clbParcels.Enabled = False
         Me.clbParcels.SelectionMode = SelectionMode.None
         Me.cmdOK.Enabled = True
         Me.cmdClear.Enabled = False
         zzFillGushFileList()
      End If
	End Sub
	Private Sub rdbParcelFile_CheckedChanged(oSender As System.Object, e As System.EventArgs) Handles rdbParcelFile.CheckedChanged
      If mbEventsEnabled AndAlso rdbParcelFile.Checked Then
         mbFolder = False
         mbParcel = True


         Me.txtFileList.Text = String.Empty
         Me.txtName.Text = msCurrentParcelFileName
         '		Me.clbParcels.Enabled = False
         Me.clbParcels.SelectionMode = SelectionMode.None
         Me.cmdOK.Enabled = True
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
	Private Sub clbParcels_SelectedIndexChanged(ByVal oSender As System.Object, e As System.EventArgs)	' Handles clbParcels.SelectedIndexChanged
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
	Private Sub frmApp_Shown(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Shown
		'	MessageBox.Show(CStr(Me.Location.X) & ":" & CStr(Me.Location.Y), "01_056")
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
			Me.Location = New System.Drawing.Point(0, 0)
		End If
	End Sub
	Private Sub Button99_Click(sender As System.Object, e As System.EventArgs)
		Me.Close()
	End Sub
	Private Sub Button88_Click(sender As System.Object, e As System.EventArgs)
		MessageBox.Show(Me.DialogResult.ToString(), "02_449")
	End Sub
   Private Sub Button1_Click_1(oSender As System.Object, e As System.EventArgs) 'Handles Button1.Click


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      moFDO_Manager.CreateBoundingBoxMapLayer(msCurrentParcelFileName)
      moFDO_Manager.ExportToShapeByGush()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub Button1_Click(oSender As System.Object, e As System.EventArgs) Handles Button1.Click
      Dim sShapeFile As String = "M:\Tababuild\ProjectsNet\Gushim_Shapes\Gush_7700_7703\A1"
      Dim sShapeFile1 As String = "M:\Tababuild\ProjectsNet\Gushim_Shapes\Gush_7700_7703"
      Dim sShapeFile10 As String = "M:\Tababuild\ProjectsNet\Gushim_Shapes\Gush_7700_7703\Parcel_All_Tmp.shp"

      Dim sShapeFile2 As String = "M:\Tababuild\ProjectsNet\Gushim_Shapes\Gush_7700_7703\Tmp_Shp"



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      mtFilterList = New DMCommon.dmList("7700,7701")
      FDO.TplnPolygonSet.CopyParcelData(sShapeFile10, "Parcel_All_Tmp", mtFilterList, sShapeFile2)
      ' FDO.FDO_Manager.ReadSchema(sShapeFile2)
      '  FDO.FDO_Manager.CreateSchema(sShapeFile)

      ' FDO.FDO_Manager.InsertData(sShapeFile)

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


		moFDO_Manager.ClipImportAll()



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()



		'''''''''	moFDO_Manager.ExportToShapeByGushAfter()
	End Sub


	Private Sub cmdAllParcels_Click(oSsender As System.Object, e As EventArgs) Handles cmdAllParcels.Click
		Me.Cursor = Cursors.WaitCursor
		Dim sResFolder As String = zzGetLastFolderPath(msAllParcelRootFolder)
		If mbFolder Then
			zzFolder(sResFolder & msAllParcelPath)
		Else

			zzFile(sResFolder & msAllParcelPathInRoot)
		End If



		Me.Cursor = Cursors.Default


	End Sub
	Private Sub cmdAllParcelsToDB_Click(oSender As System.Object, e As EventArgs) Handles cmdAllParcelsToDB.Click
		Dim sResFolder As String = zzGetLastFolderPath(msAllParcelRootFolder)
		Dim sFileName As String = sResFolder & msAllParcelPathInRoot
		Dim oFile As IO.FileInfo = New IO.FileInfo(sFileName)
		Dim sClassName As String = zzGetClassName(oFile.Name, oFile.Extension)
		Dim oParcelsAllDataTable As Data.DataTable = New Data.DataTable("ParcelsAll")
		Const sEraseCom As String = "TRUNCATE TABLE dbo.PARCEL_ALL"
		Const sDestTableName As String = "dbo.PARCEL_ALL"

		'Const sComText As String = "Select PARCEL_ID, GUSH_NUM, GUSH_SUFFI, PARCEL, LEGAL_AREA, STATUS, SHAPE_AREA, SHAPE_LEN,dbGeometry FROM PARCEL_ALL_KKK"
		'Const sComText As String = "Select PARCEL_ID, GUSH_NUM, GUSH_SUFFI, PARCEL, LEGAL_AREA, STATUS, SHAPE_AREA, SHAPE_LEN FROM PARCEL_ALL"
		Const sComText As String = "Select PARCEL_ID, GUSH_NUM, GUSH_SUFFI, PARCEL, LEGAL_AREA, STATUS, STATUS_TEX, LOCALITY_I, LOCALITY_N, REG_MUN_ID, REG_MUN_NA, COUNTY_ID, COUNTY_NAM, REGION_ID, REGION_NAM, TALAR_NUMB, TALAR_YEAR, SYS_DATE, SHAPE_AREA, SHAPE_LEN FROM PARCEL_ALL"

		Me.Cursor = Cursors.WaitCursor

		Dim oParcelAllDataAdapter As Data.Common.DbDataAdapter
		Dim oSqlBulkCopy As System.Data.SqlClient.SqlBulkCopy
		'	DMCommon.Debug.MsgBox("04_412c", sFileName)
		oParcelAllDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, Data.CommandType.Text, Nothing, False, String.Empty, True, False)
		oSqlBulkCopy = TPlServerDB.ServerDB.CurrentServerDB.GetSqlBulkCopy(sDestTableName)

		'    moFragmentsDataAdapter.FillLoadOption
		DMCommon.Debug.MsgBox("04_412d", sFileName)
		Try
			oParcelAllDataAdapter.FillSchema(oParcelsAllDataTable, Data.SchemaType.Source)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & sFileName, "AllParcels")
		End Try
		'Mona 054 949 86 85


		If oFile.Exists Then
			Me.txtName.Text = sFileName

			'Dim bRes As Boolean = FDO.TplnPolygonSet.InputParcelData(sFileName, sClassName, False, oParcelsAllDataTable)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "oParcelsAllDataTable", oParcelsAllDataTable.Rows.Count)
			'DMCommon.Debug.MsgBox("04_412e", bRes, oParcelsAllDataTable.Rows.Count)
			TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sEraseCom, Data.CommandType.Text)

			'Dim bRes As Boolean = FDO.TplnPolygonSet.InputParcelDataNew(sFileName, sClassName, False, oParcelAllDataAdapter)
			Dim bRes As Boolean = FDO.TplnPolygonSet.InputParcelDataNew(sFileName, sClassName, False, oParcelsAllDataTable, oSqlBulkCopy)


			If False And bRes Then
				TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sEraseCom, Data.CommandType.Text)
				DMCommon.Debug.MsgBox("04_412t", oParcelsAllDataTable.Rows.Count)
				oParcelAllDataAdapter.Update(oParcelsAllDataTable)
			End If
		Else
			System.Windows.Forms.MessageBox.Show("File '" & sFileName & "' was not found", "cmdAllParcelsToDB_Click-frmParcelsFromShapes")

		End If
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub txtName_Validated(oSender As System.Object, e As EventArgs) Handles txtName.Validated
		If mbFolder Then
			msCurrentRootName = Me.txtName.Text
			Me.clbParcels.Items.Clear()

			zzFillParcelFolderList()
		End If

	End Sub


End Class