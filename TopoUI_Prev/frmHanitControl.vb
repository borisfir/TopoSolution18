Option Explicit On
Option Strict On


Public Class frmHanitControl
   Private mbEventsEnabled As Boolean = False
   Private mhsHanitLayers As HashSet(Of String) = New HashSet(Of String)()
   Private mfTextEditor As frmTextEditor
   Private mfUD_General As TopoUI.frmUD_General
   Private mbAutocad As Boolean
   Public Sub GeneralView(bView As Boolean)
      If mfUD_General IsNot Nothing AndAlso Not mfUD_General.IsDisposed Then
         mfUD_General.Visible = bView
      End If
     
   End Sub



   Private Sub zzSetStageLayerOnAAA(iStageNo As Integer, bIsOn As Boolean)
      Dim saLayer(4) As String
      saLayer(0) = UnidivNet.UD_App.GetStagePolineLayer(iStageNo)
      saLayer(1) = UnidivNet.UD_App.GetStageParcelCentroidLayer(iStageNo)
      saLayer(2) = UnidivNet.UD_App.GetStageFrontLineLayer(iStageNo)
      saLayer(3) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, True)
      saLayer(4) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, False)

      DMAcadExt.AcadTransaction.SetLayersOn(saLayer, bIsOn)

   End Sub
   Private Sub cmdCreateHanit_Click(oSender As System.Object, e As EventArgs) Handles cmdCreateHanit.Click
      Dim oGridrow As System.Windows.Forms.DataGridViewRow
      Dim saLayer(5) As String
      mbEventsEnabled = False
      If TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB() Then
         If UnidivNet.UD_App.OpenDB() Then
            '   DMCommon.ExcelLog.Open()

            ' Dim iLastStage As Integer = UnidivNet.UD_App.DrawHanitEntities
            Dim colParcelCountsByStage As System.Collections.ObjectModel.Collection(Of Integer) = UnidivNet.UD_App.DrawHanitEntitiesNew
            '   Dim iLastStage As Integer = UnidivNet.UD_App.DrawHanitEntities()

            '   MessageBox.Show("End of App", "04_090")
            Me.dgvStages.Rows.Clear()
            For iStageNo As Integer = 0 To colParcelCountsByStage.Count - 1
               zzDefLayers(iStageNo, saLayer)
               Me.dgvStages.Rows.Add(1)
               oGridrow = Me.dgvStages.Rows.Item(iStageNo)
               oGridrow.Cells.Item("ctxStageNo").Value = iStageNo
               oGridrow.Cells.Item("ctxParcelCount").Value = colParcelCountsByStage.Item(iStageNo)

               oGridrow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Checked
            Next
            DMAcadExt.AcadDocument.CloseLog()

         End If

      End If
      mbEventsEnabled = True
   End Sub
   Private Sub zzInitLayers()
      Dim saLayer(5) As String
      Dim iStageNo As Integer = 0
      Dim oGridRow As System.Windows.Forms.DataGridViewRow
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         mhsHanitLayers.Clear()
         Do
            zzDefLayers(iStageNo, saLayer)



            If DMAcadExt.AcadTransaction.LayersExist(saLayer, False) Then
               Me.dgvStages.Rows.Add(1)
               oGridRow = Me.dgvStages.Rows.Item(iStageNo)
               oGridRow.Cells.Item("ctxStageNo").Value = iStageNo
               oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Checked
               For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
                  mhsHanitLayers.Add(saLayer(iIndex))
               Next

            Else
               Exit Do
            End If

            iStageNo += 1
         Loop While iStageNo < 20

         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If



   End Sub
   Private Function zzGetHanitLayers() As HashSet(Of String)
      Dim saLayer(5) As String
      Dim iStageNo As Integer = 0
      Dim hsHanitLayers As HashSet(Of String) = New HashSet(Of String)()

      Do
         zzDefLayers(iStageNo, saLayer)
         If DMAcadExt.AcadTransaction.LayersExist(saLayer, False) Then
            For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
               hsHanitLayers.Add(saLayer(iIndex))
            Next
         Else
            Exit Do
         End If

         iStageNo += 1
      Loop While iStageNo < 20





      hsHanitLayers.UnionWith(UnidivNet.UD_App.Auxiliary)
      hsHanitLayers.UnionWith(UnidivNet.UD_App.AddLayers)


      Return hsHanitLayers
   End Function
   Private Sub zzDefLayers(iStageNo As Integer, ByRef saLayer() As String)
      saLayer(0) = UnidivNet.UD_App.GetStagePolineLayer(iStageNo)
      saLayer(1) = UnidivNet.UD_App.GetStageParcelCentroidLayer(iStageNo)
      saLayer(2) = UnidivNet.UD_App.GetStageFrontLineLayer(iStageNo)
      saLayer(3) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, False)
      saLayer(4) = UnidivNet.UD_App.GetStageUDPointLayer(iStageNo, True)
      saLayer(5) = UnidivNet.UD_App.GetStageCentroidLayer(iStageNo)

      For iIndex As Integer = 0 To saLayer.GetUpperBound(0)
         mhsHanitLayers.Add(saLayer(iIndex))
      Next

   End Sub

   Private Sub chkOnlyOne_CheckedChanged(ooSsender As System.Object, e As EventArgs)

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

         zzSetOne()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If
   End Sub
   Private Sub zzSetOne()
      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvStages.CurrentCell

      If oCurrentCell IsNot Nothing Then
         mbEventsEnabled = False
         Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
         Dim oGridRow As DataGridViewRow


         For iRowIndex As Integer = 0 To dgvStages.RowCount - 1
            oGridRow = Me.dgvStages.Rows.Item(iRowIndex)
            If iRowIndex = iCurrentRowIndex Then

               oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Checked
               UnidivNet.UD_App.SetStageLayerOn(iRowIndex, True)
            Else
               oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Unchecked
               UnidivNet.UD_App.SetStageLayerOn(iRowIndex, False)
            End If
         Next
         mbEventsEnabled = True

      End If

   End Sub

   Private Sub zzSetOne(iCurrentRowIndex As Integer)




      Dim oGridRow As DataGridViewRow
      For iRowIndex As Integer = 0 To dgvStages.RowCount - 1
         oGridRow = Me.dgvStages.Rows.Item(iRowIndex)
         If iRowIndex = iCurrentRowIndex Then
            oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Checked
         Else
            oGridRow.Cells.Item("cchLayerOn").Value = System.Windows.Forms.CheckState.Unchecked
         End If
      Next



   End Sub
   Private Sub zzMoveCurrent(iRows As Integer)
      If iRows > 0 Then
         Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvStages.CurrentCell

         If oCurrentCell IsNot Nothing Then
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
            If DMAcadExt.AcadDocument.IsLocked Then


               DMAcadExt.AcadTransaction.Start()

               Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
               Dim iCurrentColumnIndex As Integer = oCurrentCell.ColumnIndex

               iCurrentRowIndex = (iCurrentRowIndex + iRows) Mod (Me.dgvStages.RowCount)
               Dim oGridRow As System.Windows.Forms.DataGridViewRow = Me.dgvStages.Rows.Item(iCurrentRowIndex)
               oCurrentCell = oGridRow.Cells.Item(iCurrentColumnIndex)
               Me.dgvStages.CurrentCell = oCurrentCell
               zzSetOne()
               DMAcadExt.AcadTransaction.Terminate()
               DMAcadExt.AcadDocument.Unlock()
               DMAcadExt.AcadDocument.UpdateScreen()
            End If
         End If
      End If

   End Sub
   Private Sub cmdPreviousStage_Click(oSender As System.Object, e As EventArgs) Handles cmdPreviousStage.Click
      zzMoveCurrent(Me.dgvStages.RowCount - 1)


   End Sub

   Private Sub cmdNextStage_Click(oSender As System.Object, e As EventArgs) Handles cmdNextStage.Click
      zzMoveCurrent(1)
   End Sub


   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.

      zzInitLayers()
      mbEventsEnabled = True
   End Sub
   Private Sub dgvStages_CellValueChanged(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvStages.CellValueChanged
      If mbEventsEnabled AndAlso e.RowIndex >= 0 Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         If DMAcadExt.AcadDocument.IsLocked Then
            DMAcadExt.AcadTransaction.Start()
            '    DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
            Dim oDynObjecct As Object = dgvStages.Rows(e.RowIndex).Cells.Item("cchLayerOn").Value
            Dim iCheckState As System.Windows.Forms.CheckState = CType(oDynObjecct, System.Windows.Forms.CheckState)
            Dim bValue As Boolean = (iCheckState = CheckState.Checked)
            ' MessageBox.Show(CStr(e.RowIndex) & ":" & CStr(bValue), "06_400")
            UnidivNet.UD_App.SetStageLayerOn(e.RowIndex, bValue)

            '    DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            DMAcadExt.AcadDocument.UpdateScreen()
         End If
      End If
   End Sub

   Private Sub frmHanitControl_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub chkAllLayers_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkAllLayers.CheckedChanged
      If mbEventsEnabled Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         If DMAcadExt.AcadDocument.IsLocked Then
            DMAcadExt.AcadTransaction.Start()
            '  Dim oaLayers(mhsHanitLayers.Count - 1) As String
            '  mhsHanitLayers.CopyTo(oaLayers)
            '  DMCommon.Functions.DispArray(oaLayers, "layers", True)
            DMAcadExt.AcadTransaction.SetLayersOnExcept(mhsHanitLayers, Me.chkAllLayers.Checked)

            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            DMAcadExt.AcadDocument.UpdateScreen()
         End If

      End If
   End Sub


   Private Sub cmdOnlyOne_Click(oSender As System.Object, e As EventArgs) Handles cmdOnlyOne.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      '  System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadDocument.IsLocked.ToString() & vbCrLf & "", "21_999 DocLock")
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

         zzSetOne()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If
   End Sub



   Private Sub cmdClearLayers_Click(oSender As System.Object, e As EventArgs) Handles cmdClearLayers.Click

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
         DMAcadExt.AcadDocument.OpenLog(False)
         DMAcadExt.AcadTransaction.ClearLayerSet(zzGetHanitLayers())
         UnidivNet.UD_App.DeleteHanitTopos()
         DMAcadExt.AcadDocument.CloseLog()
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If
   End Sub

   Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
      Me.Close()
   End Sub


   Private Sub cmdEraseUnusedPoints_Click(oSender As System.Object, e As EventArgs) Handles cmdEraseUnusedPoints.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      If DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

         UnidivNet.UD_App.ErasePoints()


         UnidivNet.UD_App.DeleteMarkBlocks()



         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If
   End Sub

   Private Sub cmdOpen_Tr_Book_Click(oSender As System.Object, e As EventArgs) Handles cmdOpen_Tr_Book.Click
      If mfTextEditor Is Nothing OrElse mfTextEditor.IsDisposed Then
         mfTextEditor = New frmTextEditor()
      End If
      Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(DMAcadExt.AcadDocument.GetCurrentDWGName())
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTextEditor, True)
      '  frmHanitControl.vb() : line 305

      Dim sBodyText As String = UnidivNet.UD_App.GetBook()


      mfTextEditor.DefaultFolder = oFileInfo.DirectoryName
      mfTextEditor.DefaultTxtFileName = "Tr_bookH.txt"
      mfTextEditor.DefaultPDFFileName = "Tr_bookH.pdf"

      mfTextEditor.ContentText = sBodyText
      mfTextEditor.Text = "Tr_bookH.txt"
   End Sub

   Private Sub cmdOpen_Tr_Result_Click(oSender As System.Object, e As EventArgs) Handles cmdOpen_Tr_Result.Click
      If mfTextEditor Is Nothing OrElse mfTextEditor.IsDisposed Then
         mfTextEditor = New frmTextEditor()
      End If
      Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(DMAcadExt.AcadDocument.GetCurrentDWGName())
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTextEditor, True)
      '  frmHanitControl.vb() : line 305

      Dim sBodyText As String = UnidivNet.UD_App.GetResult()


      mfTextEditor.DefaultFolder = oFileInfo.DirectoryName
      mfTextEditor.DefaultTxtFileName = "Tr_rsultH.res"
      mfTextEditor.DefaultPDFFileName = "Tr_rsultH.pdf"
      mfTextEditor.ContentText = sBodyText
      mfTextEditor.Text = "Tr_rsultH.res"
   End Sub

   Private Sub cmdOpenGen_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenGen.Click

      Dim sNormalParcelsString As String = Nothing
      Dim sTempParcelsString As String = Nothing

      Dim bIsTTTG As Boolean = UnidivNet.UD_App.IsTTG
      UnidivNet.UD_App.GetParcelsString(sNormalParcelsString, sTempParcelsString)

      '    MessageBox.Show(sNormalParcelsString & vbCrLf & sTempParcelsString, "01_498")
      If mfUD_General Is Nothing OrElse mfUD_General.IsDisposed Then
         mfUD_General = New TopoUI.frmUD_General(True)
      End If
      mfUD_General.GushName = TopoManager.TPlanGraph.UD_ParcelKey.BaseBlockName
      mfUD_General.NormalParcels = sNormalParcelsString
      mfUD_General.TempParcels = sTempParcelsString

      mfUD_General.SetPlanType(bIsTTTG)


      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
      '	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
      mfUD_General.Left = 40
      mfUD_General.Top = 40



   End Sub

   Private Sub cmdInsertRep_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertRep.Click
      Dim oRepApp As AcadReport.Report = New AcadReport.Report
      mbAutocad = True
      zzLaunchReport(oRepApp)

   End Sub
   Private Sub zzCleanupA()
      Dim tCleanupResult As DMAcadExt.dmCleanupResult
      Dim tCleanupOptions As DMAcadExt.dmCleanupOptions = New DMAcadExt.dmCleanupOptions()
      '  System.Windows.Forms.MessageBox.Show(tCleanupOptions.NetPointsLines & vbCrLf & (tCleanupOptions.RoundingSPoints).ToString() & ":" & tCleanupOptions.RoundingTolerance & vbCrLf & CStr(iCleanupIndex) & vbCrLf & tCleanupOptions.Rounding.ToString(), "05_001")
      '  DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      '''''''''''''''''  tCleanupOptions.

      tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupSPoints(False, tCleanupOptions)
   End Sub

   Private Sub cmdExpExcel_Click(oSender As System.Object, e As EventArgs) Handles cmdExpExcel.Click
      mbAutocad = False

      zzLaunchReport(New ExcelReport.Report(False))


   End Sub
   Private Sub zzLaunchReport(ByVal oReport As AcadReport.BaseReport)

      Dim iDataOption As TopoManager.TPlanGraph.enDataOptions = TopoManager.TPlanGraph.enDataOptions.Default
      '     Dim iOverlayMethod As DMAcadExt.enOverlayMethod
      '    Dim iOverlayIndex As DMAcadExt.enOverlayIndex = TopoManager.TPlanGraph.UnionPgonArea.GetOverlayIndex(iOverlayMethod, TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose)
      Dim iOptionIndex As Integer = -1
      Dim bTopoPurpose As Boolean = False
      Dim bAllArea As Boolean = False

      Dim oaOptionValues(1) As System.Object
      '     Dim oaMultiOptionValues()() As System.Object

      Dim sTopoPurposeText As String = Nothing
      If UnidivNet.UD_App.ParcelListIsNotEmpty Then





         Me.Cursor = Cursors.WaitCursor

         '	MessageBox.Show(moSelectedReportItem.ListDispData & vbCrLf & moSelectedReportItem.RepIndex.ToString(), "01_118")






         Dim oDataView As System.Data.DataView = Nothing
         Dim oaDataView() As System.Data.DataView = Nothing
         Dim saModData() As String = Nothing
         Dim iaDataColumns() As Integer = Nothing
         Dim iGroupColumnStart As Integer = -1
         Dim iGroupColumnUB As Integer = 0


         Dim oaCaptions() As System.Object = Nothing
         Dim oaTotals() As System.Object = Nothing

         If oReport.AcadModel Then
            AcadReport.RepApp.InitDWGScaleFactor()
         End If
         '	MessageBox.Show(doSelectedReportItem.RepIndex.ToString() & ":" & doSelectedReportItem.TopoPurpose.ToString() & ":" & iDataOption.ToString(), "01_666")
         '	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & iOverlayMethod.ToString() & vbCrLf & moSelectedReportItem.RepIndex.ToString() & vbCrLf & CStr(CInt(moSelectedReportItem.RepIndex)), "01_399d")


         ''''''''''''''''''   System.Windows.Forms.MessageBox.Show(CStr(999), "05_002")

         ' Case TPlServerDB.enResourceTheme.AcRepLotsK
         '  TopoManager.TPlanGraph.TplnLot.GetMainData(moSelectedReportItem.TopoPurpose, iDataOption, iOverlayMethod, oDataView, iaDataColumns, oaTotals)
         oDataView = UnidivNet.UD_App.GetParcelsAreaTable()
         If oDataView IsNot Nothing Then
            zzInsertReport(oDataView, iaDataColumns, oaCaptions, oaTotals, oaOptionValues, , iGroupColumnStart, iGroupColumnUB)
         End If


         '  System.Windows.Forms.MessageBox.Show(moSelectedReportItem.ReportModification.ToString(), "05_028")



         ' System.Windows.Forms.MessageBox.Show(CStr(999), "05_032")

      End If
      Me.Cursor = Cursors.Default
   End Sub
   Private Sub zzInsertReport(ByVal oDataView As System.Data.DataView, ByVal iaDataColumns() As Integer, ByVal oaCaptions() As System.Object, ByVal oaTotals() As System.Object, ByVal oaOptionValues() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing, Optional ByVal iGroupColumnStart As Integer = -1, Optional ByVal iGroupColumnUB As Integer = 0)
      Dim oRepApp As AcadReport.BaseReport
      Dim bCurrentLayerOK As Boolean = True
      If mbAutocad Then
         oRepApp = New AcadReport.Report
      Else
         oRepApp = New ExcelReport.Report(False)
      End If

      '    System.Windows.Forms.MessageBox.Show(CStr(999), "05_001")
      If oRepApp.Open(TPlServerDB.enResourceTheme.AcRepUD_AreaTable) Then
         oRepApp.MainView = oDataView
         If iaDataColumns IsNot Nothing Then
            oRepApp.DataColumns = iaDataColumns
         End If
         If iaDataColumns IsNot Nothing Then
            oRepApp.DataColumns = iaDataColumns
         End If
         If oaCaptions IsNot Nothing Then
            oRepApp.Captions = oaCaptions
         End If
         If oaTotals IsNot Nothing Then
            oRepApp.Totals = oaTotals
         End If
         If oaOptionValues(0) IsNot Nothing Then
            oRepApp.OptionValues = oaOptionValues
         End If
         If iaEmptyColumns IsNot Nothing Then
            oRepApp.EmptyColumns = iaEmptyColumns
         End If
         '	oRepApp.GroupColumnStart = iGroupColumnStart
         If oRepApp.AcadModel Then
            DMAcadExt.AcadDocument.SaveVarCmdDia(0)
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
            DMAcadExt.AcadTransaction.Start()
            DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
            AcadReport.RepApp.InitDWGScaleFactor()
            AcadReport.RepApp.Init(False)
            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(UnidivNet.UD_App.ReportTableLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)
            
            AcadReport.RepApp.GetStartPoint()
            Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection

            Dim oReportApp As AcadReport.Report
            If bCurrentLayerOK Then
               Me.Hide()
               '    System.Windows.Forms.MessageBox.Show(oRepApp.GetType().ToString, "05_029")
               TopoManager.Common.SetAcadFocus()
               oRepApp.Insert()



               oReportApp = DirectCast(oRepApp, AcadReport.Report)
               If AcadReport.RepApp.AcadTable IsNot Nothing Then
                  ''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
                  '''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
                  '''''''''''''''''	AcadReport.Report.ReDrawTable()
                  colaPoints = oReportApp.GetColorCells()
                 

               End If
               Me.Show()
               DMAcadExt.AcadTransaction.CloseModelSpace()
               DMAcadExt.AcadTransaction.Terminate()
               DMAcadExt.AcadDocument.Unlock()
               DMAcadExt.AcadDocument.RestoreVarCmdDia()
               '''''''''''''''	zzPaintTable()

            End If
         Else

            oRepApp.Insert()
         End If
      End If


   End Sub

   Private Sub cmdOpen_PointsSrv_Click(oSender As System.Object, e As EventArgs) Handles cmdOpen_PointsSrv.Click
      If mfTextEditor Is Nothing OrElse mfTextEditor.IsDisposed Then
         mfTextEditor = New frmTextEditor()
      End If
      Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(DMAcadExt.AcadDocument.GetCurrentDWGName())
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTextEditor, True)
      '  frmHanitControl.vb() : line 305

      Dim sBodyText As String = UnidivNet.UD_App.GetPointsSrv()


      mfTextEditor.DefaultFolder = oFileInfo.DirectoryName
      mfTextEditor.DefaultTxtFileName = "Points.srv"
      mfTextEditor.DefaultPDFFileName = "Points.pdf"

      mfTextEditor.ContentText = sBodyText
      mfTextEditor.Text = "Points.srv"
   End Sub

  
   Private Sub cmdInsertPointBlocking_Click(oSender As System.Object, e As EventArgs) Handles cmdInsertPointBlocking.Click
      zzGetBlockRefFromDrawing()

   End Sub
   Private Sub zzGetBlockRefFromDrawing()
      Const sPromptMsg As String = "Select Block Reference ..."
      Const sBlockName As String = UnidivNet.UD_App.UD_NewPointBlockName
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions(sPromptMsg)
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      '		Dim bExists As Boolean
      Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sBlockName)
      Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference = Nothing
      Dim bCurrentLayerOK As Boolean = True
      Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(UnidivNet.UD_App.UD_ForcedPointLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)
      oAcadBlock.OpenForRight()
      If bCurrentLayerOK AndAlso oAcadBlock.DefinitionExists Then


         Do
            oEditor.WriteMessage(sPromptMsg & vbCrLf)
            '     DMAcadExt.AcadDocument.CommandLine(True)
            ptRes = oEditor.GetEntity(oPromptOpt)
            If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
               Try
                  oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

                  If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then

                     oBlockRef = DirectCast(oEnt, Autodesk.AutoCAD.DatabaseServices.BlockReference)
                     Select Case oBlockRef.Name
                        Case sBlockName
                           tBlockRefData.Position = oBlockRef.Position
                           tBlockRefData.ScaleFactors = oBlockRef.ScaleFactors
                           oAcadBlock.InsertRef(tBlockRefData)
                     End Select



                  End If
               Catch oEx As Exception

               End Try


            Else
               Exit Do
            End If
         Loop
         DMAcadExt.AcadDocument.CommandLine(True)
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub

   
   Private Sub cmdInputPrevVersion_Click(oSender As System.Object, e As EventArgs) Handles cmdInputPrevVersion.Click
      TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
      UnidivNet.UD_App.InputPrevDB()
   End Sub

   Private Sub cmdCleanup_Click(oSender As System.Object, e As EventArgs) Handles cmdCleanup.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
      zzCleanupA()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub chkPointsBlocking_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkPointsBlocking.CheckedChanged
      TopoManager.TopoScheme.tsNode.IsGeoVertex = Me.chkPointsBlocking.Checked
      DMCommon.Debug.MsgBox("12_402", TopoManager.TopoScheme.tsNode.IsGeoVertex)
   End Sub
End Class
