Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map
Public Class frmLotsFromShapes


   Protected ptMapThemeData As DMAcadExt.MapThemeData
   Protected ptDissolveMapThemeData As DMAcadExt.MapThemeData


   Private msCurrentRootName As String
   Private msCurrentGushFileName As String = String.Empty
   Private msCurrentParcelFileName As String = String.Empty
   Private msCurrentLotsFileName As String = String.Empty



   Private msaParcelFolders() As String
   Private moFDO_Manager As FDO.FDO_Manager
   Private mbFolder As Boolean = True
   Private mbParcel As Boolean
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
      '  msCurrentRootName = TopoManager.TPlanGraph.TplnProject.GushimVectorizedRootPath
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
   Public ReadOnly Property CurrentLotsFileName As String
      Get
         Return msCurrentLotsFileName
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
   Public ReadOnly Property Parcel As Boolean
      Get
         Return mbParcel
      End Get
   End Property
   Private Sub zzMyInitializeComponent()
      Me.txtName.Text = msCurrentRootName

   End Sub
   Private Sub zzFolder()
      Dim iDialogResult As DialogResult = Me.fbdParcels.ShowDialog()
      If iDialogResult = Windows.Forms.DialogResult.OK Then
         'Me.clbParcels.CheckOnClick = False
         Dim sSelectedPath As String = Me.fbdParcels.SelectedPath
         If Not String.IsNullOrEmpty(sSelectedPath) Then
            Me.txtName.Text = sSelectedPath
            msCurrentRootName = sSelectedPath

         End If
      End If

   End Sub
   Private Sub zzFile()
      Dim iDialogResult As DialogResult = Me.opdLots.ShowDialog()
      Dim sFileName As String = opdLots.FileName

      Me.txtName.Text = sFileName

   End Sub

   Private Function zzGetClassName(ByVal sFileName As String, ByVal sFileExtension As String) As String
      Return sFileName.Substring(0, sFileName.Length - sFileExtension.Length)
   End Function



   Private Sub frmApp_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
      mbEventsEnabled = True
   End Sub
   Private Sub frmApp_Resize(oSender As System.Object, e As System.EventArgs) Handles Me.Resize

   End Sub
   Private Sub cmdOK_Click(oSender As System.Object, e As System.EventArgs) Handles cmdOK.Click
      Me.Cursor = Cursors.WaitCursor


      'gggggggggggggggggggggggggg()
      If Me.txtName.Text.Length <> 0 Then
         Dim oFileInfo As IO.FileInfo = New IO.FileInfo(Me.txtName.Text)
         Dim sShapeFileName As String, sShapeFolderName As String, sFileName As String
         If oFileInfo.Exists Then
            sShapeFileName = oFileInfo.FullName
            sShapeFolderName = oFileInfo.DirectoryName
            sFileName = oFileInfo.Name
            Dim sClassName As String = zzGetClassName(sFileName, oFileInfo.Extension)




         End If
      End If

      Me.Close()
      Me.tmrDelay.Enabled = True
      '''''''''''''''''''''''Me.tmrDelay.Enabled = False

      Me.Cursor = Cursors.Default
      '	Me.cmdOK.Enabled = False
      '	MessageBox.Show(Me.DialogResult.ToString(), "02_452")
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
            If True Then   'TEMP


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





   Private Sub cmdClear_Click(oSender As System.Object, e As System.EventArgs) Handles cmdClear.Click
      '	Dim o As CheckedListBox.CheckedItemCollection = Me.clbParcels.CheckedItems

   End Sub
   Private Sub frmApp_Shown(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Shown
      '	MessageBox.Show(CStr(Me.Location.X) & ":" & CStr(Me.Location.Y), "01_056")
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
   End Sub

   Private Sub Button1_Click(oSender As System.Object, e As System.EventArgs)


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      moFDO_Manager.CreateBoundingBoxMapLayer(msCurrentParcelFileName)
      moFDO_Manager.ExportToShapeByGush()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub Button2_Click(sender As System.Object, e As System.EventArgs)



      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


      moFDO_Manager.ClipImportAll()



      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()



      '''''''''	moFDO_Manager.ExportToShapeByGushAfter()
   End Sub

   Private Sub cmdSelectFile_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectFile.Click
      Dim iDialogResult As DialogResult = Me.opdLots.ShowDialog()
      Dim sFileName As String = opdLots.FileName

      Me.txtName.Text = sFileName
      msCurrentLotsFileName = sFileName
   End Sub

End Class