Imports FDO
Public Class frmExecUtil
   ' Const msRoot As String = "P:\2016\160590\Plan\Compil\BlueLines1\"
   ' Const msOutputDir As String = "P:\2016\160590\Plan\Compil\BlueLines\Paste"
   Const msRoot As String = "C:\BlueLines\"
   '  Const msRoot As String = "P:\2016\160590\Plan\Compil\"

   Private mbDebug As Boolean
   Private msSourceRoot As String = msRoot & "P\" ' "C:\BlueLines\P\"
   Private msOutputDir As String = msRoot & "Paste" '"C:\BlueLines\Paste"
   Private Shared msDataFileFolder As String = msRoot & "Data\" '"C:\BlueLines\Data\"
   Const msDataFileBaseName As String = "AttrData"


   Private miCurrentDirIndexMax As Integer = 2
   Const miStage0Lenth As Integer = 5
   Const miStage1Lenth As Integer = 5

   Private moRootFolder As IO.DirectoryInfo
   Private moaDirInfo() As System.IO.DirectoryInfo
   Private miCurrentDirIndex As Integer = 0
   Private moFDO_Manager As FDO_Manager
   Private msSourceLayer As String = String.Empty
   Private msOverlayLayer As String
   Private msOutputLayer As String
   Private msAttrDataFileName As String
   Private msPgonDataFileName As String



   Private msSourceConnection As String
   Private msOverlayConnection As String
   Private msOutputConnection As String
   Private msOutputShapeFileFullName As String
   Private miTick As Integer
   Private miStage As Integer
   Private miStageLenth As Integer

   Private mbContinue As Boolean
   Private moPgonRefDictionary As PgonRefDictionary
   Private mdicPgonData As IDictionary(Of Integer, System.Data.DataRow)

   Private Sub frmExecUtil_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing

      If mdicPgonData IsNot Nothing Then
         mdicPgonData.Clear()
         mdicPgonData = Nothing
      End If
      If moPgonRefDictionary IsNot Nothing Then
         moPgonRefDictionary.Clear()
         moPgonRefDictionary = Nothing
      End If
      If moaDirInfo IsNot Nothing Then
         Erase moaDirInfo
      End If
      If moFDO_Manager IsNot Nothing Then
         moFDO_Manager.Close()
         moFDO_Manager.Dispose()
         moFDO_Manager = Nothing
      End If
      DMAcadExt.AppMessages.ClearAll()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   Private Sub frmExecUtil_Load(oSender As System.Object, e As EventArgs) Handles MyBase.Load
      Me.txtCurrentIndex.Text = miCurrentDirIndex.ToString()
   End Sub
   Public Shared Sub EnumShapePgons()
      Const sRoot As String = "P:\2016\160590\Plan\Compil\BlueLines\P\"
      Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRoot)
      Dim sShapeFile As String
      Dim sShapeFileFullName As String
      Dim i As Integer
      Dim sFCName As String
      Dim oFDO_Manager As FDO_Manager = New FDO_Manager()
      Dim sShapeConnection As String
      Dim sDirName As String


      For Each oDirectoryInfo As IO.DirectoryInfo In oRootFolder.GetDirectories()
         sDirName = oDirectoryInfo.Name
         sFCName = sDirName.Substring(0, 10)
         sShapeFile = sFCName & ".SHP"
         sShapeFileFullName = oDirectoryInfo.FullName & "\" & sShapeFile
         sShapeConnection = "shp_" & sDirName
         DMAcadExt.AcadDocument.WriteMessage(sShapeFileFullName)
         oFDO_Manager.ConnectToShape(sShapeConnection, sShapeFileFullName)
         oFDO_Manager.AddLayerToMap(sShapeConnection)


         '  
         i += 1
         If i = 2 Then
            Exit For
         End If
      Next
   End Sub
   Private Sub zzInit()
      moRootFolder = New IO.DirectoryInfo(msSourceRoot)
      moaDirInfo = moRootFolder.GetDirectories()
      moFDO_Manager = New FDO_Manager()

      mdicPgonData = New Dictionary(Of Integer, System.Data.DataRow)

      ''''''''''''''  DMCommon.ExcelLog.Open()
      miStage = 0
      miStageLenth = miStage0Lenth
      miTick = 0

   End Sub
   Private Sub zzConnectSourceShape()
      Dim sShapeFile As String
      Dim sShapeFileFullName As String

      Dim sFCName As String

      Dim sShapeConnection As String
      Dim sDirName As String

      Dim oDirectoryInfo As IO.DirectoryInfo = moaDirInfo(miCurrentDirIndex)

      sDirName = oDirectoryInfo.Name
      sFCName = sDirName.Substring(0, 10)
      sShapeFile = sFCName & ".SHP"
      sShapeFileFullName = oDirectoryInfo.FullName & "\" & sShapeFile
      sShapeConnection = "shp_" & sDirName
      '    DMAcadExt.AcadDocument.WriteMessage(sShapeFileFullName)
      moFDO_Manager.ConnectToShape(sShapeConnection, sShapeFileFullName)
      moFDO_Manager.AddLayerToMap(sShapeConnection)
      ''''''''  DMCommon.ExcelLog.SetNextValue(iSheetRow, 0, sDirName)
      If miCurrentDirIndex = 0 Then
         msSourceLayer = sFCName
         msSourceConnection = sShapeConnection
      Else
         If miCurrentDirIndex > 1 Then
            msSourceLayer = msOutputLayer
            msSourceConnection = msOutputConnection

         End If
         msOutputLayer = zzGetOutputLayerName(False)


         msOverlayLayer = sFCName
         msOverlayConnection = sShapeConnection

         zzSetDataFilesName()

      End If
      '   MessageBox.Show(miCurrentDirIndex.ToString() & vbCrLf & msSourceLayer & vbCrLf & msSourceConnection & vbCrLf & msOverlayLayer & vbCrLf & msOverlayConnection & vbCrLf & msOutputLayer, "---All---")
      Dim oDataTable As System.Data.DataTable
      '  DMCommon.ExcelLog.SetValue(iSheetRow, 5, sShapeFileFullName, sFCName)
      oDataTable = moFDO_Manager.GetSourceData(sShapeFileFullName, sFCName)
      For Each oRow As System.Data.DataRow In oDataTable.Rows
         If mdicPgonData.ContainsKey(miCurrentDirIndex + 1) Then
         Else
            mdicPgonData.Add(miCurrentDirIndex + 1, oRow)
         End If

         '''''''  DMCommon.ExcelLog.SetValue(iSheetRow, 2, miCurrentDirIndex, sFCName, oRow.Item(0), oRow.Item(1), oRow.Item(2), oRow.Item(3), oRow.Item(4))

      Next
   End Sub
   Private Sub zzSetDataFilesName()
      msAttrDataFileName = "AttrData" & Format(miCurrentDirIndex, "0000")
      msPgonDataFileName = "PgonData" & Format(miCurrentDirIndex, "0000")
   End Sub
   Private Function zzGetOutputLayerName(bPrevios As Boolean) As String
      Dim iIndex As Integer
      Dim sOutputLayer As String
      If bPrevios Then
         iIndex = miCurrentDirIndex - 1
      Else
         iIndex = miCurrentDirIndex
      End If
      sOutputLayer = "Paste" & Format(iIndex, "0000")
      Me.txtOutputLayer.Text = sOutputLayer
      Return sOutputLayer
   End Function

   Private Sub zzPaste()
      '   MessageBox.Show(miCurrentDirIndex.ToString() & vbCrLf & msOutputLayer, "zzProc0")
      Dim sOutputSHPFileName As String = moFDO_Manager.Paste(msSourceLayer, msOverlayLayer, msOutputLayer, msOutputDir, 1.0, 1.0)
      ' moFDO_Manager.Paste(msSourceLayer, msOverlayLayer, "BBBBB", msOutputDir, 0.1, 1.0)
      If sOutputSHPFileName Is Nothing Then
         Me.tmrMain.Enabled = False
         miTick = 0
         '   MessageBox.Show("Paste ERROR !!!")
         System.Windows.Forms.MessageBox.Show(msSourceLayer & vbCrLf & msOverlayLayer & vbCrLf & msOutputLayer & vbCrLf & msOutputDir, "Failed  1611x")
         mdicPgonData.Remove(miCurrentDirIndex + 1)
         miCurrentDirIndex -= 1
         zzSetDataFilesName()
         zzSaveData()
      End If
   End Sub

   Private Sub zzConnectOutputShape()




      '    Dim oFDO_Manager As FDO_Manager = New FDO_Manager()
      ' Dim sShapeConnection As String



      msOutputShapeFileFullName = msOutputDir & "\" & msOutputLayer & ".SHP"
      msOutputConnection = "shp_" & msOutputLayer
      DMAcadExt.AcadDocument.WriteMessage(msOutputShapeFileFullName)

      moFDO_Manager.ConnectToShape(msOutputConnection, msOutputShapeFileFullName)
      moFDO_Manager.AddLayerToMap(msOutputConnection)

   End Sub
   Private Sub zzRemoveLayer(sShapeConnection As String, sMapLayer As String)
      FDO.FDO_Manager.RemoveConnectionB(sShapeConnection)
      FDO.FDO_Manager.RemoveResource(sShapeConnection, sMapLayer)
   End Sub
   Private Sub cmdInputIndex_Click(oSender As System.Object, e As EventArgs) Handles cmdInputIndex.Click
      zzInitIndex()
   End Sub
   Private Sub zzInsertCentroids(sPolylineLayer As String)
      Const sLayerName As String = "SKANYO"
      Const sBlockName As String = "SKANYO-RG1"
      Dim oODTable As DMAcadExt.ODTable
      Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

      oODTable = New DMAcadExt.ODTable(sPolylineLayer)

      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oMapUtility As Autodesk.Gis.Map.MapUtility = oMapApplication.ActiveProject.MapUtility
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(sPolylineLayer)
      Dim iTest As Integer = 0
      Dim oAcadData As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sBlockName)
      Dim dicAttribValues As Dictionary(Of String, String)
      oAcadData.OpenForRight()
      Try
         oMapUtility.CreateCentroids(colCentroids, colPolylines, sLayerName, sBlockName)
      Catch oMapEx As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(colCentroids.Count) & ":" & CStr(colPolylines.Count) & vbCrLf & sLayerName & ":" & sBlockName & vbCrLf & oMapEx.StackTrace, "01_841t")
         ' DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "FDO_Manager - tsiBuildTopo_Click" & ": ")
      End Try

      Dim iFeatID As Integer
      Dim iPlanID As Integer

      Dim iPlanNum As Integer
      Dim sPlan As String
      Dim sPlanDescripton As String



		Dim oDataRow As System.Data.DataRow = Nothing
      System.Windows.Forms.MessageBox.Show(colCentroids.Count.ToString() & ":" & colPolylines.Count.ToString(), "06_340")

      For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colCentroids
         Try
            oODRec = oODTable.GetODRecord(tAcObjID)

            If oODRec IsNot Nothing Then
               '	oODRec.Init()


               Try
                  iFeatID = oODRec.Item(0).Int32Value
                  If moPgonRefDictionary.TryGetValue(iFeatID, iPlanID) Then
                     If mdicPgonData.TryGetValue(iPlanID, oDataRow) Then
                        dicAttribValues = New Dictionary(Of String, String)
                        If Not oDataRow.IsNull("Taba_Num") Then
                           sPlan = DirectCast(oDataRow.Item("Taba_Num"), String)
                           dicAttribValues.Add("PLAN", sPlan)
                        End If
                        If Not oDataRow.IsNull("Taba_Numer") Then
                           iPlanNum = DirectCast(oDataRow.Item("Taba_Numer"), Integer)
                           dicAttribValues.Add("ID", iPlanNum.ToString())
                        End If
                        If Not oDataRow.IsNull("taba_name") Then
                           sPlanDescripton = DirectCast(oDataRow.Item("taba_name"), String)
                           dicAttribValues.Add("DESCRIPTION", sPlanDescripton)
                        End If
                        ' Beit Shemesh
                        'If Not oDataRow.IsNull("Date") Then
                        '   sDate = DirectCast(oDataRow.Item("Date"), String)
                        '   dicAttribValues.Add("DATE", sDate)
                        'End If


                        oAcadData.UpdateAttribData(tAcObjID, dicAttribValues)
                     End If
                  End If

                  iTest += iFeatID

               Catch oEx As Exception

               End Try



            End If


         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadFDO_Overlay_9")
            System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sPolylineLayer, "04_231yy")
         End Try
      Next

      ''''''''''''   System.Windows.Forms.MessageBox.Show(iTest.ToString(), "iTest")

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   Private Sub zzImport()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()







      Dim miPgonImported As Integer
      Dim sShapeFileName As String = msOutputDir & "\" & msOutputLayer & ".SHP"
      Dim oShapeExpImp As ShapeExpImp = New ShapeExpImp(enExpImp.Import, sShapeFileName)
      oShapeExpImp.ImportPolygonsAsClosedPolylines = True

      'If Not String.IsNullOrEmpty(sDestLayer) Then
      '   oShapeExpImp.SetDestLayer(sDestLayer)
      'End If
      oShapeExpImp.AddImpShapeData()
      oShapeExpImp.Exec()


      miPgonImported = Convert.ToInt32(oShapeExpImp.PolygonCount)
      oShapeExpImp.Dispose()
      oShapeExpImp = Nothing
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
      '  System.Windows.Forms.MessageBox.Show(sShapeFileName & vbCrLf & CStr(miPgonImported), "04_998")
   End Sub
   Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles Button1.Click

      zzProc0()

   End Sub
   Private Sub zzProc0()
      If miCurrentDirIndex > 1 AndAlso Not mbContinue Then
         zzRemoveLayer(msSourceConnection, msSourceLayer)
         zzRemoveLayer(msOverlayConnection, msOverlayLayer)
      End If


      If miCurrentDirIndex > 1 AndAlso Not mbContinue Then
         zzConnectOutputShape()
         zzGetOutputAttrData()
      End If


      If miCurrentDirIndex > miCurrentDirIndexMax Then
         Me.tmrMain.Enabled = False
         zzSaveData()

      Else
         If mbContinue Then
            zzContinueInit()
            mbContinue = False
         End If
         zzConnectSourceShape()
      End If

      '    MessageBox.Show(miCurrentDirIndex.ToString(), "zzProc0")
   End Sub
   Private Sub zzContinueInit()
      msAttrDataFileName = "AttrData" & Format(miCurrentDirIndex - 1, "0000")
      moPgonRefDictionary = New PgonRefDictionary()
      '  MessageBox.Show(msAttrDataFileName, "msAttrDataFileName")
      moPgonRefDictionary.Load(msAttrDataFileName)
      msPgonDataFileName = "PgonData" & Format(miCurrentDirIndex - 1, "0000")
      If mbDebug Then
         MessageBox.Show(msAttrDataFileName & vbCrLf & msPgonDataFileName, "msAttrDataFileName")
      End If

      zzLoadPgonData()
   End Sub
   Private Sub zzProc1()
      If miCurrentDirIndex > 0 Then
         zzPaste()
      End If
      miCurrentDirIndex += 1
      txtCurrentIndex.Text = miCurrentDirIndex.ToString()



      '   MessageBox.Show(miCurrentDirIndex.ToString(), "zzProc1")
   End Sub
   Private Sub zzGetOutputAttrData()
      Dim oDataTable As System.Data.DataTable = moFDO_Manager.GetIdentifyData(msOutputShapeFileFullName, msOutputLayer)
      If moPgonRefDictionary Is Nothing Then
         moPgonRefDictionary = New PgonRefDictionary(oDataTable)
      Else
         moPgonRefDictionary.AddNext(oDataTable, miCurrentDirIndex)
      End If
   End Sub
   Private Sub zzPrintPgonRefDictionary()
      Dim iValue As Integer

		'  DMCommon.ExcelLog.Reset()
		For Each iKey As Integer In moPgonRefDictionary.Keys
         iValue = moPgonRefDictionary.Item(iKey)

		Next


   End Sub
   Private Sub zzSavePgonData()
      Dim sFileName As String = msDataFileFolder & msPgonDataFileName & ".txt"

      Dim saOutput(mdicPgonData.Count - 1) As String
      Dim oDataRow As System.Data.DataRow
      Dim iIndex As Integer = 0
      For Each iKey As Integer In mdicPgonData.Keys
         oDataRow = mdicPgonData.Item(iKey)
         saOutput(iIndex) = iKey.ToString() & vbTab & DMCommon.Functions.CStrN(oDataRow.Item("Taba_Num")) & vbTab & DMCommon.Functions.CIntN(oDataRow.Item("Taba_Numer")) & vbTab & DMCommon.Functions.CStrN(oDataRow.Item("taba_Name")) ''''''''' Beit Shemesh & vbTab & DMCommon.Functions.CStrN(oDataRow.Item("Date"))
         iIndex += 1
      Next
      IO.File.WriteAllLines(sFileName, saOutput)
   End Sub
   Private Sub zzSaveData()
      zzPrintPgonRefDictionary()
      '  MessageBox.Show(msAttrDataFileName, "msDataFileName")
      moPgonRefDictionary.Save(msAttrDataFileName)
      zzSavePgonData()
   End Sub
   Public Sub zzLoadPgonData()
      Dim sFileName As String = msDataFileFolder & msPgonDataFileName & ".txt"
      Dim saInputput() As String

      Dim oNewDataRow As System.Data.DataRow
      Dim iIndex As Integer = 0
      Dim iPlanID As Integer
      Dim iTaba_Numer As Integer
      Dim saLine() As String

      Dim oDataTable As System.Data.DataTable = zzCreateSourceTable()
      saInputput = IO.File.ReadAllLines(sFileName)
      If mbDebug Then
         MessageBox.Show(sFileName & vbCrLf & saInputput.GetUpperBound(0).ToString(), "sFileName")
      End If


      For Each sLine As String In saInputput
         oNewDataRow = oDataTable.NewRow()
         saLine = Split(sLine, vbTab)
         If saLine.GetUpperBound(0) >= 0 Then
            Integer.TryParse(saLine(0), iPlanID)
         End If
         If saLine.GetUpperBound(0) >= 1 Then
            oNewDataRow.Item("Taba_Num") = saLine(1)

         End If
         If saLine.GetUpperBound(0) >= 2 Then
            If Integer.TryParse(saLine(2), iTaba_Numer) Then
               oNewDataRow.Item("Taba_Numer") = iTaba_Numer
            End If
         End If
         If saLine.GetUpperBound(0) >= 3 Then
            oNewDataRow.Item("taba_name") = saLine(3)

         End If
         If saLine.GetUpperBound(0) >= 4 Then
            oNewDataRow.Item("Date") = saLine(4)

         End If


        
         mdicPgonData.Add(iPlanID, oNewDataRow)


      Next
      If mbDebug Then
         MessageBox.Show(mdicPgonData.Count.ToString(), "mdicPgonData")

      End If

     
   End Sub
   Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
      zzProc1()
   End Sub

   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      zzInit()
   End Sub

   Private Sub tmrMain_Tick(oSender As System.Object, e As EventArgs) Handles tmrMain.Tick
      miTick += 1
      If miTick = miStageLenth Then
         If miStage = 0 Then
            miStage = 1
            miStageLenth = miStage1Lenth
            miTick = 0
            zzProc0()

         Else
            miStage = 0
            miStageLenth = miStage0Lenth
            miTick = 0

            zzProc1()


         End If



      End If

   End Sub

   Private Sub cmdStart_Click(oSender As System.Object, e As EventArgs) Handles cmdStart.Click
      zzInitIndex()
      Me.tmrMain.Enabled = True
   End Sub
   Private Sub zzInitIndex()
      If Integer.TryParse(Me.txtCurrentIndex.Text, miCurrentDirIndex) Then
         If miCurrentDirIndex > 0 Then
            mbContinue = True
            msOutputLayer = zzGetOutputLayerName(True)
         End If
      End If

      If Integer.TryParse(Me.txtMaxIndex.Text, miCurrentDirIndexMax) Then
      Else
         miCurrentDirIndexMax = miCurrentDirIndex + 1
      End If
      If miCurrentDirIndexMax > moaDirInfo.GetUpperBound(0) Then
         miCurrentDirIndexMax = moaDirInfo.GetUpperBound(0)
         Me.txtMaxIndex.Text = miCurrentDirIndexMax.ToString()
      End If
   End Sub
   Private Function zzCreateSourceTable() As System.Data.DataTable
      Dim oDataTable As System.Data.DataTable = New System.Data.DataTable("Identifiers")

      '		Else
      '	oOrder = GetType(System.STRING)
      '	End If
      With oDataTable.Columns

         .Add("FeatID", GetType(System.Int32))
         .Add("Taba_Num", GetType(System.String))
         .Add("Taba_Numer", GetType(System.Int32))
         .Add("taba_Name", GetType(System.String))
         .Add("Date", GetType(System.String))


      End With
      Return oDataTable
   End Function

   Private Class PgonRefDictionary
      Inherits Dictionary(Of Integer, Integer)
      Public Sub New()

      End Sub
      Public Sub New(oDataTable As System.Data.DataTable)
         Dim iFeatID As Integer
         Dim iPlanID As Integer
         For Each oDataRow As System.Data.DataRow In oDataTable.Rows
            iFeatID = DirectCast(oDataRow.Item(0), Integer)
            If oDataRow.IsNull(1) AndAlso Not oDataRow.IsNull(2) Then
               iPlanID = 2

            ElseIf Not oDataRow.IsNull(1) AndAlso oDataRow.IsNull(2) Then
               iPlanID = 1
            End If
            MyBase.Add(iFeatID, iPlanID)
         Next
      End Sub

      Public Sub AddNext(oDataTable As System.Data.DataTable, iOverlayID As Integer)
         Dim iaPlanID(oDataTable.Rows.Count - 1) As Integer
         Dim iaFeatID(oDataTable.Rows.Count - 1) As Integer
         Dim iPlanID As Integer
         Dim oDataRow As System.Data.DataRow
         For iIndex As Integer = 0 To oDataTable.Rows.Count - 1
            oDataRow = oDataTable.Rows.Item(iIndex)

            iaFeatID(iIndex) = DirectCast(oDataRow.Item(0), Integer)

            If oDataRow.IsNull(1) AndAlso Not oDataRow.IsNull(2) Then
               iPlanID = iOverlayID

            ElseIf Not oDataRow.IsNull(1) AndAlso oDataRow.IsNull(2) Then
               If MyBase.TryGetValue(DirectCast(oDataRow.Item(1), Integer), iPlanID) Then

               End If

            End If
            iaPlanID(iIndex) = iPlanID

         Next
         MyBase.Clear()
         For iIndex As Integer = 0 To oDataTable.Rows.Count - 1
            MyBase.Add(iaFeatID(iIndex), iaPlanID(iIndex))
         Next
      End Sub
      Public Sub Save(sDataFileName As String)
         Dim sFileName As String = msDataFileFolder & sDataFileName & ".txt"
         Dim saOutput(MyBase.Count - 1) As String
         Dim iValue As Integer
         Dim iIndex As Integer = 0
         For Each iKey As Integer In MyBase.Keys
            iValue = MyBase.Item(iKey)
            saOutput(iIndex) = iKey.ToString() & vbTab & iValue
            iIndex += 1
         Next
         IO.File.WriteAllLines(sFileName, saOutput)
      End Sub

      Public Sub Load(sDataFileName As String)
         Dim sFileName As String = msDataFileFolder & sDataFileName & ".txt"
         Dim saInputput() As String

         Dim iIndex As Integer = 0
         Dim iFeatID As Integer
         Dim iPlanID As Integer
         Dim saLine() As String

         saInputput = IO.File.ReadAllLines(sFileName)

         For Each sLine As String In saInputput
            saLine = Split(sLine, vbTab)
            If saLine.GetUpperBound(0) = 1 Then
               Integer.TryParse(saLine(0), iFeatID)
               Integer.TryParse(saLine(1), iPlanID)
               MyBase.Add(iFeatID, iPlanID)
            End If
         Next
      End Sub


   End Class
   Private Structure PgonData
      Public Taba_Num As String
      Public Taba_Numer As Integer
      Public taba_Name As String
      Public [Date] As String
   End Structure

   Private Sub cmdInsertCentroids_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertCentroids.Click
      zzInitIndex()
      zzContinueInit()

      Dim sPgonsLayer As String = zzGetOutputLayerName(True)
      zzInsertCentroids(sPgonsLayer)
   End Sub
 
   Private Sub cmdImport_Click(oSender As System.Object, e As EventArgs) Handles cmdImport.Click
      zzInitIndex()
      zzImport()

   End Sub

   Private Sub txtCurrentIndex_TextChanged(oSender As System.Object, e As EventArgs) Handles txtCurrentIndex.TextChanged

   End Sub
End Class