Imports Autodesk.AutoCAD.DatabaseServices
Imports TopoManager.TPlanGraph

Public Class frmPgonSetView
   'Const msLegalAreaNameFieldName As String = "LegalArea"

   Private Const miDoubleExists As Integer = -1
   Private Const msIntersectionPointsLayer As String = "zzIntersPoints"

   Private mtSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
   Private mtDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
   Private miProjectCode As Integer
   Private miDetailNo As Integer
   Private moParams As TPlServerDB.dmParams
   Private moPgonSet As FDO.TplnPolygonSet
   Private mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private mbIsLot As Boolean = False
   Private mbIsGush As Boolean = False

   Private mbBlocksOnly As Boolean = False
   Private mbIntersections As Boolean = False
   Private mbPolylines As Boolean = False


   Private mdicTopoPolygons As TopoManager.tmPolygons
   Private moPolygonDataTable As System.Data.DataTable
   Private moIntersectionsDataTable As System.Data.DataTable
   Private moVoidsDataTable As System.Data.DataTable


   Private moMainDataView As System.Data.DataView
   Private moBlocksOutsideDataView As System.Data.DataView
   Private mbHasBlockAdd As Boolean
   Private midgvMainLocationY As Integer
   Private msErrLayer As String
   Private moDefaultCellStyle As DataGridViewCellStyle
   Private moAlternatingCellStyle As DataGridViewCellStyle
   Private moErrorDefaultCellStyle As DataGridViewCellStyle
   Private moErrorAlternatingCellStyle As DataGridViewCellStyle
   Private moWarningDefaultCellStyle As DataGridViewCellStyle
   Private moWarningAlternatingCellStyle As DataGridViewCellStyle
   Private mbDissolve As Boolean = False

   Private mbPgonsPainted As Boolean = False
   Private mbEventsEnabled As Boolean = False
   Private mdicLayersOffStatus As Dictionary(Of ObjectId, Boolean)


   Public Sub New(tSourceMapThemeData As DMAcadExt.MapThemeData, tDissolveMapThemeData As DMAcadExt.MapThemeData, bDissolve As Boolean)
      mtSourceMapThemeData = tSourceMapThemeData
      mtDissolveMapThemeData = tDissolveMapThemeData
      mbDissolve = bDissolve
      miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode

      miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

      '		mtMapThemeData = tMapThemeData

      ' This call is required by the designer.
      InitializeComponent()
      '    MessageBox.Show(tSourceMapThemeData.MapThemeID.ToString() & ":" & tDissolveMapThemeData.MapThemeID.ToString(), "04_400")
      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()
      ''''''''''''   TopoManager.TPlanGraph.TplnParcel.Initialize(tSourceMapThemeData)





      If tDissolveMapThemeData.IsNotEmpty AndAlso tDissolveMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Blocks Then
         TopoManager.TPlanGraph.TplnBlock.CreateBlockTable()
      End If

      '	MessageBox.Show(CStr(mtDissolveMapThemeData.IsNotEmpty) & ":" & tDissolveMapThemeData.TopoName, "04_406")

      zzLoadParams()
   End Sub
   Public Property PgonSet As FDO.TplnPolygonSet
      Get
         Return moPgonSet
      End Get
      Set(oValue As FDO.TplnPolygonSet)
         moPgonSet = oValue
      End Set
   End Property
   Private Sub zzSetCellStyles()

      moDefaultCellStyle = Me.dgvPolygons.DefaultCellStyle
      moAlternatingCellStyle = Me.dgvPolygons.AlternatingRowsDefaultCellStyle

      moErrorDefaultCellStyle = moDefaultCellStyle.Clone
      moErrorDefaultCellStyle.BackColor = Color.Red
      moErrorDefaultCellStyle.SelectionBackColor = Color.DarkRed


      moWarningDefaultCellStyle = moDefaultCellStyle.Clone
      moWarningDefaultCellStyle.BackColor = Color.Yellow
      moWarningDefaultCellStyle.SelectionBackColor = Color.Brown
      '  moHideDefaultCellStyle.ForeColor = Color.FromArgb(0, moDefaultCellStyle.BackColor)

      '  moHideAlternatingCellStyle = New DataGridViewCellStyle
      '  moHideAlternatingCellStyle.ForeColor = Color.FromArgb(0, moAlternatingCellStyle.BackColor)

   End Sub


   Private Sub zzMyInitializeComponent()
      Me.dgvPolygons.AutoGenerateColumns = False
      Me.dgvPolygons.RowHeadersWidth = 23
      zzSetCellStyles()
      '  System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString())
      Select Case mtSourceMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.Blocks
            mbIsGush = True
            Me.ctxName.Visible = False
            Me.ctxName_C.Visible = False
         Case DMAcadExt.enMapTheme.Parcels
            Me.ctxName.HeaderText = "חלקה"
            mbIsLot = False
         Case DMAcadExt.enMapTheme.LotApproved
            Me.ctxName.HeaderText = "מגרש"
            Me.ctxAttribA.HeaderText = "מס' יעוד "

            Me.ctxGroupID.Visible = False
            Me.ctxGroupAddID.Visible = False
            mbIsLot = True
         Case DMAcadExt.enMapTheme.UD_Parcels
            '  UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)

         Case DMAcadExt.enMapTheme.UD_Blocks
            mbIsGush = True
            Me.ctxName.Visible = False
            Me.ctxName_C.Visible = False
      End Select
      midgvMainLocationY = Me.dgvPolygons.Location.Y
      ' zzInitToolStrip()
   End Sub
   Private Sub zzLoadParams()


   End Sub
   Private Sub zzSaveParams()

   End Sub
   Private Sub zzCreatePgonSet()
      '   MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370")
      Dim bCurrentLayerOK As Boolean = False

      ' System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString & vbCrLf & mtSourceMapThemeData.TopoPurpose.ToString & vbCrLf & mtSourceMapThemeData.GraphType.ToString() & vbCrLf & mtSourceMapThemeData.TopoName & vbCrLf & mtSourceMapThemeData.ClosedPgonsLayers, "07_001s")
      moPgonSet = New FDO.TplnPolygonSet(mtSourceMapThemeData.MapThemeID, mtSourceMapThemeData.TopoPurpose, mtSourceMapThemeData.TopoName)
      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oEnt As DBObject = Nothing

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      Select Case mtSourceMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.Blocks
            TopoManager.TPlanGraph.TplnBlock.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.Parcels
            TopoManager.TPlanGraph.TplnParcel.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.UD_Parcels
            UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
            TopoManager.TPlanGraph.TplnLot.Initialize(mtSourceMapThemeData)
      End Select
      Select Case mtDissolveMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.Blocks
            TopoManager.TPlanGraph.TplnBlock.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.Parcels
            TopoManager.TPlanGraph.TplnParcel.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.UD_Parcels
            UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)
         Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
            TopoManager.TPlanGraph.TplnLot.Initialize(mtSourceMapThemeData)
      End Select





      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msIntersectionPointsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

      '    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      '   System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.ClosedPgonsLayers & vbCrLf & "", "07_002")
      Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtSourceMapThemeData.ClosedPgonsLayers)
      '
      Dim dicCentroids As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(mtSourceMapThemeData.CentroidBlock, OpenMode.ForWrite)   '"pclp004"
      '
      mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(mtSourceMapThemeData.CentroidBlocks, mtSourceMapThemeData.CentroidLayers)
      DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count) & "; " & mtSourceMapThemeData.CentroidBlocks & "; " & mtSourceMapThemeData.CentroidLayers & "; " & mtSourceMapThemeData.ClosedPgonsLayers)
      moPgonSet.AddPolylineIDs(colPolygonIDs)
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msIntersectionPointsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      '
      'oPgonSet.AddBlocks(dicCentroids)
      moPgonSet.AddCentroids(mcolCentroids)
      '
      'TopoManager.TPlanGraph.TplnBlock.Initialize(mtSourceMapThemeData)
      'TopoManager.TPlanGraph.TplnParcel.Initialize(mtSourceMapThemeData)
      'TopoManager.TPlanGraph.TplnLot.Initialize(mtSourceMapThemeData)
      'UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)


      '

      '   Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)

      '''''''''''''	
      moPgonSet.CalculateNewF(True)
      '  DMCommon.ExcelLog.Open()
      moPgonSet.InfoToExcel()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzGetPgonSetData()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)
      '   System.Windows.Forms.MessageBox.Show("", "04_549")
      moPgonSet.FillIntersectionsTable()
      mbHasBlockAdd = moPgonSet.HasBlockAdd
      moBlocksOutsideDataView = moPgonSet.PointsOutside
      '   System.Windows.Forms.MessageBox.Show("", "04_550")
      moPolygonDataTable = moPgonSet.LinkDataTable
      moIntersectionsDataTable = moPgonSet.IntersectionsTable

      Me.lblBlocksOutsideCount.Text = moBlocksOutsideDataView.Count.ToString()
      If moBlocksOutsideDataView.Count > 0 Then
         Me.lblBlocksOutsideCount.BackColor = Color.Red
      Else
         Me.lblBlocksOutsideCount.BackColor = Color.Empty 'System.Drawing.SystemColors.Control
         '   Me.lblBlocksOutsideCount.BackColor = DefaultBackColor

      End If

      Me.lblIntersectionCount.Text = moIntersectionsDataTable.Rows.Count.ToString()
      If moIntersectionsDataTable.Rows.Count > 0 Then
         Me.lblIntersectionCount.BackColor = Color.Red
      Else
         Me.lblIntersectionCount.BackColor = Color.Empty 'System.Drawing.SystemColors.Control
         '   Me.lblBlocksOutsideCount.BackColor = DefaultBackColor

      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzGetVoidData()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)
      '   System.Windows.Forms.MessageBox.Show("", "04_549")

      zzCreateVoidsTable()
      zzFillVoidsTable()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzFillDataGrid()
      If mbHasBlockAdd Then
         Me.ctxGroupAddID.Visible = True
      End If
      mdicTopoPolygons = moPgonSet.TmPolygons


      '  moMainDataView = New Data.DataView(moPolygonDataTable, String.Empty, moPgonSet.SortByName, Data.DataViewRowState.CurrentRows)
      '   Me.dgvPolygons.DataSource = moMainDataView
   End Sub

   Private Function zzRowToNum(oGridRow As DataGridViewRow) As Integer
      Dim iBlock As Integer = 0
   End Function
   Private Function zzPacelToNum(iGroupID As Integer, iGroupAddID As Integer, lNo As Long) As Long
      ' Dim l As Long = ((9999999& * 1000& + 888&) * 10000& + 7777&) * 1000 + 555
      Return (Convert.ToInt64(iGroupID) * 1000& + Convert.ToInt64(iGroupAddID)) * 10000000& + Convert.ToInt64(lNo)

   End Function
   Private Sub cmdRun_Click(oSender As System.Object, e As EventArgs) Handles cmdRun.Click
      Me.Cursor = Cursors.WaitCursor
      moPgonSet = Nothing
      zzRun()
      zzSetData()
      Me.Cursor = Cursors.Default
   End Sub
   Private Sub zzRun()
      If moPgonSet Is Nothing OrElse moPgonSet.SetMapTheme <> (mtSourceMapThemeData.MapThemeID) Then
         zzCreatePgonSet()
      End If

      '  zzCreatePgonSet()
      zzGetPgonSetData()
      zzGetVoidData()

      zzFillDataGrid()
      If Me.rdbPolygons.Checked Then
         zzFormatDataGrid()
      End If
      '   moBlocksOutsideDataView = moPgonSet.PointsOutside
      ' Me.lblBlocksOutsideCount.Text = moBlocksOutsideDataView.Count.ToString()
   End Sub

   Private Sub cmdZoom_Click(oSender As System.Object, e As EventArgs) Handles cmdZoom.Click
      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvPolygons.CurrentCell

      If oCurrentCell IsNot Nothing Then
         Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
         Dim oGridRow As DataGridViewRow = Me.dgvPolygons.Rows.Item(iCurrentRowIndex)
         Dim dXmin, dXmax, dYmin, dYmax As Double
         Dim oPoint As DMAcadExt.TPlnPoint = Nothing
         Dim dDWGScale As System.Double = CDbl(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.ScaleSysVarName))
         Dim oDataRow As Data.DataRowView = DirectCast(oGridRow.DataBoundItem, Data.DataRowView)
         If oDataRow Is Nothing Then
            oDataRow = moBlocksOutsideDataView.Item(iCurrentRowIndex)
         End If
         Dim tAcObjID As ObjectId = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.AcObjIDFieldName), ObjectId)
         If mbBlocksOnly Then
            oPoint = New DMAcadExt.TPlnPoint(DirectCast(oDataRow.Item(FDO.TplnPolygonSet.XFieldName), Double), DirectCast(oDataRow.Item(FDO.TplnPolygonSet.YFieldName), Double))

         Else
            dXmin = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.XminFieldName), Double)
            dYmin = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.YminFieldName), Double)
            dXmax = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.XmaxFieldName), Double)
            dYmax = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.YmaxFieldName), Double)
         End If

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         If mbBlocksOnly Then
            DMAcadExt.AcadDocument.Zoom(tAcObjID, 2.0)
         Else
            DMAcadExt.AcadDocument.Zoom(dXmin, dYmin, dXmax, dYmax)
         End If

         DMAcadExt.AcadTransaction.Highlight(tAcObjID)
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
         '  FDO.TplnPolygonSet.msAppPrefix





      End If

   End Sub

   Private Function zzGetBox(oDataRow As Data.DataRowView) As DMAcadExt.TPlnBoundingBox

      Dim oMinPoimt As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(DirectCast(oDataRow.Item(FDO.TplnPolygonSet.XminFieldName), Double), DirectCast(oDataRow.Item(FDO.TplnPolygonSet.YminFieldName), Double))
      Dim oMaxPoimt As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(DirectCast(oDataRow.Item(FDO.TplnPolygonSet.XmaxFieldName), Double), DirectCast(oDataRow.Item(FDO.TplnPolygonSet.YmaxFieldName), Double))


      Return New DMAcadExt.TPlnBoundingBox(oMinPoimt, oMaxPoimt)
   End Function
   Private Sub zzFormatDataGrid()
      '    Const iDoubleExists As Integer = -1
      Try


         Dim bIsProper As Boolean
         Dim iCentroidCount As Integer
         Dim dLegalArea As Double
         Dim iGroupID, iGroupAddID As Integer
         Dim lOrder As Long
         Dim oCell As System.Windows.Forms.DataGridViewCell
         Dim dicNames As Dictionary(Of Long, Integer) = New Dictionary(Of Long, Integer)()
         Dim iRowIndex As Integer = 0
         Dim iFirstEntryIndex As Integer
         Dim oFirstEntryGridRow As DataGridViewRow

         Dim iNameExistsRowIndex As Integer = 0
         '    frmPgonSetView.vb:line 259
         Dim lKey As Long
         For Each oGridRow As DataGridViewRow In Me.dgvPolygons.Rows
            ' oDataRow = DirectCast(oGridRow.DataBoundItem, Data.DataRowView)
            ' bIsProper = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.IsProperFieldName), Boolean)
            iGroupID = 0
            iGroupAddID = 0
            lOrder = 0&
            oCell = oGridRow.Cells.Item("ctxIsProper")
            bIsProper = DirectCast(oCell.Value, Boolean)
            If Not bIsProper Then
               oCell.Style = moErrorDefaultCellStyle
            Else
               oCell = oGridRow.Cells.Item("ctxCentroidCount")
               iCentroidCount = DMCommon.Functions.CIntN(oCell.Value)
               If iCentroidCount <> 1 Then
                  oCell.Style = moErrorDefaultCellStyle
                  ' MessageBox.Show(oGridRow.Cells.Item("ctxNo").Value.ToString() & vbCrLf & oGridRow.Cells.Item("ctxNo").Value.GetType().ToString())
               Else
                  oCell = oGridRow.Cells.Item("ctxGroupID")
                  If IsDBNull(oCell.Value) Then
                     oCell.Style = moErrorDefaultCellStyle
                  Else
                     iGroupID = DirectCast(oCell.Value, Integer)
                  End If
                  oCell = oGridRow.Cells.Item("ctxGroupAddID")
                  iGroupAddID = DMCommon.Functions.CIntN(oCell.Value)

                  oCell = oGridRow.Cells.Item("ctxOrder")
                  If (oCell.Value Is Nothing) OrElse IsDBNull(oCell.Value) Then
                     oCell.Style = moErrorDefaultCellStyle
                  Else
                     lOrder = DirectCast(oCell.Value, Long)
                  End If
                  
                  If (mbIsLot OrElse iGroupID <> 0) AndAlso lOrder <> 0& Then
                     lKey = zzPacelToNum(iGroupID, iGroupAddID, lOrder)


                     Try
                        If dicNames.TryGetValue(lKey, iFirstEntryIndex) Then
                           '  DMAcadExt.AcadDocument.WriteMessage("!!iFirstEntryIndex: " & iFirstEntryIndex.ToString() & "  " & "!!miDoubleExists: " & miDoubleExists.ToString())
                           If iFirstEntryIndex <> miDoubleExists Then
                              oFirstEntryGridRow = Me.dgvPolygons.Rows.Item(iFirstEntryIndex)

                              zzMarkDoubleName(oFirstEntryGridRow)
                              dicNames.Remove(lKey)
                              dicNames.Add(lKey, miDoubleExists)
                           End If
                           zzMarkDoubleName(oGridRow)
                        Else
                           dicNames.Add(lKey, iRowIndex)
                        End If
                     Catch oEx As Exception
                        System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_989")
                     End Try

                  End If

                  oCell = oGridRow.Cells.Item("ctxLegalArea")
                  dLegalArea = DMCommon.Functions.CDblN(oCell.Value)
                  If dLegalArea = 0.0 Then
                     oCell.Style = moWarningDefaultCellStyle
                  End If
               End If
            End If


            iRowIndex += 1
         Next

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPgonSetView - zzFormatDataGrid")
      End Try
      'Dim oGridRow As DataGridViewRow = Me.dgvPolygons.Rows.Item(iCurrentRowIndex)
   End Sub
   Private Sub zzMarkDoubleName(oDataRow As Data.DataRowView, oMarkBlock As DMAcadExt.MarkBlock)
      Dim oVal As Object = oDataRow.Item(FDO.TplnPolygonSet.msCentroidAcObjIDFieldName)
      Dim sName As Object = oDataRow.Item(FDO.TplnPolygonSet.msNameFieldName)

      '  MessageBox.Show(oVal.ToString() & vbCrLf & oVal.GetType().ToString() & vbCrLf & oVal.GetType().ToString())
      If Not IsDBNull(oVal) Then
         Dim tAcObjID As ObjectId = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.msCentroidAcObjIDFieldName), ObjectId)

         Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(tAcObjID)
         oMarkBlock.MarkPoint(tPoint, 2S)
      End If
   End Sub
   Private Sub zzMarkDoubleName(oGridRow As DataGridViewRow)
      Dim oCell As System.Windows.Forms.DataGridViewCell

      oCell = oGridRow.Cells.Item("ctxGroupID")
      oCell.Style = moErrorDefaultCellStyle
      oCell = oGridRow.Cells.Item("ctxGroupAddID")
      oCell.Style = moErrorDefaultCellStyle
      oCell = oGridRow.Cells.Item("ctxName")
      oCell.Style = moErrorDefaultCellStyle



   End Sub
   Private Sub frmPgonSetView_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
      zzRun()
      mbEventsEnabled = True
      zzSetData()
      '   Dim o As Object = dgvPolygons.DataSource
      '   System.Windows.Forms.MessageBox.Show(o.ToString(), "04_670")
   End Sub

   Private Sub frmPgonSetView_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
      If midgvMainLocationY <> 0 Then
         Try
            Me.dgvPolygons.Height = Me.ClientSize.Height - midgvMainLocationY
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPgonSetView - Resize")
         End Try
      End If
      '   micmdFindPgonLoopLocationX = Me.cmdFindPgonLoop.Location.X
   End Sub


   Private Sub chkSortByName_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkSortByName.CheckedChanged
      zzSetData()

   End Sub
   Private Sub zzFillDataGridByCentroids()
      Dim tAcObjID As ObjectId
      Dim tParcelData As ParcelData
      Dim tLotData As LotData

      Dim iRowIndex As Integer = 0


      If moBlocksOutsideDataView.Count > 0 Then


         Dim oDataGridRow As System.Windows.Forms.DataGridViewRow
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

         Me.dgvPolygons.Rows.Add(moBlocksOutsideDataView.Count)
         For Each oDataRow As Data.DataRowView In moBlocksOutsideDataView
            tAcObjID = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.AcObjIDFieldName), ObjectId)
            oDataGridRow = dgvPolygons.Rows.Item(iRowIndex)
            If mbIsLot Then
               tLotData = TplnLot.GetLotData(tAcObjID)
               oDataGridRow.Cells.Item("ctxName").Value = tLotData.Name
               '   oDataGridRow.Cells.Item("ctxLegalArea").Value = tLotData.
            Else
               tParcelData = TplnParcel.GetParcelData(tAcObjID)

               oDataGridRow.Cells.Item("ctxGroupID").Value = tParcelData.Block
               If tParcelData.BlockAdd <> 0 Then
                  oDataGridRow.Cells.Item("ctxGroupAddID").Value = tParcelData.BlockAdd
               End If
               oDataGridRow.Cells.Item("ctxName").Value = tParcelData.Name
               oDataGridRow.Cells.Item("ctxLegalArea").Value = tParcelData.LegalArea
            End If
            iRowIndex += 1
         Next
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()

         DMAcadExt.AcadDocument.Unlock()
      End If
   End Sub

   Private Sub chkBlocksOutside_CheckedChanged(oSender As System.Object, e As EventArgs)
      zzSetData()

   End Sub
   Private Sub zzSetData()
      If mbEventsEnabled = True Then
         Dim oCurrentTable As Data.DataTable = Nothing
         If Me.rdbPolygons.Checked Then
           
            oCurrentTable = moPolygonDataTable
            mbPolylines = True
            mbBlocksOnly = False
            mbIntersections = False
            zzSetColumns()
            zzSetColumns(False, True)
            zzBlocksSetColumns(True)
            zzVoidColumns(False)
         ElseIf Me.rdbBlocksOutside.Checked Then
            mbPolylines = False
            mbBlocksOnly = True
            mbIntersections = False
            moMainDataView = moBlocksOutsideDataView
            zzSetColumns()
            zzSetColumns(False, True)
            zzBlocksSetColumns(False)
            ' zzFillDataGridByCentroids()
         ElseIf Me.rdbIntersections.Checked Then
            mbPolylines = False
            mbBlocksOnly = False
            mbIntersections = True
            oCurrentTable = moIntersectionsDataTable
            zzSetColumns()

            zzSetColumns(True, True)
            zzVoidColumns(False)
         ElseIf Me.rdbVoids.Checked Then
            oCurrentTable = moVoidsDataTable
            mbPolylines = True
            mbBlocksOnly = False
            zzSetColumns()
            zzSetColumns(False, False)

            zzBlocksSetColumns(False)
            zzVoidColumns(True)

         Else

         End If

         If oCurrentTable IsNot Nothing Then
            If Me.chkSortByName.Checked Then
               moMainDataView = New Data.DataView(oCurrentTable, String.Empty, moPgonSet.SortByName, Data.DataViewRowState.CurrentRows)
            Else
               moMainDataView = New Data.DataView(oCurrentTable, String.Empty, String.Empty, Data.DataViewRowState.CurrentRows)
            End If


         End If

         Me.dgvPolygons.DataSource = moMainDataView


         If Me.rdbPolygons.Checked Then
            zzFormatDataGrid()
         End If

      End If
   End Sub

   Private Sub zzBlocksSetColumns(bValue As Boolean)
      Me.ctxIsProper.Visible = bValue
      Me.ctxCentroidCount.Visible = bValue

   End Sub
   Private Sub zzVoidColumns(bValue As Boolean)
      Me.ctxGroupID.Visible = Not bValue

   End Sub
   Private Sub zzSetColumns()
      Me.ctxArea.Visible = mbPolylines
      Me.ctxLength.Visible = mbPolylines


   End Sub
   Private Sub zzSetColumns(bValue As Boolean, bNotVoidValue As Boolean)
      If Not bNotVoidValue OrElse Me.ctxGroupID_C.Visible <> bValue Then

         Me.ctxGroupID_C.Visible = bNotVoidValue AndAlso bValue
         If mbHasBlockAdd Then
            Me.ctxGroupAddID_C.Visible = bValue
         End If
         If Not mbIsGush Then
            Me.ctxName_C.Visible = bValue
         End If

         If bNotVoidValue AndAlso bValue Then
            Me.ctxName.DividerWidth = 4
         Else
            Me.ctxName.DividerWidth = 0
         End If
         Me.ctxEntityNo_C.Visible = bNotVoidValue AndAlso bValue
         Me.ctxIntersectCount.Visible = bNotVoidValue AndAlso bValue

         Me.ctxIsProper.Visible = bNotVoidValue AndAlso Not bValue
         Me.ctxCentroidCount.Visible = bNotVoidValue AndAlso Not bValue

         Me.ctxLegalArea.Visible = bNotVoidValue AndAlso Not bValue
         Me.ctxCentroidCount.Visible = bNotVoidValue AndAlso Not bValue


      End If
   End Sub
   Private Sub zzCreateVoidsTable()

      moVoidsDataTable = New System.Data.DataTable("Links")
      With moVoidsDataTable.Columns
         .Add(FDO.TplnPolygonSet.AcObjIDFieldName, GetType(ObjectId))
         .Add(FDO.TplnPolygonSet.EntityNoFieldName, GetType(System.Int32))



         '.Add(msEntityNoFieldName, GetType(System.Int32))
         ' .Add(msIDFieldName, GetType(System.Int32))
         ' .Add(msStatusFieldName, GetType(System.Boolean))
         ' .Add(IsProperFieldName, GetType(System.Boolean))

         .Add(FDO.TplnPolygonSet.XminFieldName, GetType(System.Double))
         .Add(FDO.TplnPolygonSet.XmaxFieldName, GetType(System.Double))
         .Add(FDO.TplnPolygonSet.YminFieldName, GetType(System.Double))
         .Add(FDO.TplnPolygonSet.YmaxFieldName, GetType(System.Double))



         .Add(FDO.TplnPolygonSet.AreaNameFieldName, GetType(System.Double))
         .Add(FDO.TplnPolygonSet.LengthNameFieldName, GetType(System.Double))
         .Add(FDO.TplnPolygonSet.msGroupIDFieldName, GetType(System.Int32))
         .Add(FDO.TplnPolygonSet.msGroupAddIDFieldName, GetType(System.Int32))
         .Add(FDO.TplnPolygonSet.msOrderFieldName, GetType(System.Int32))









      End With

      '  moVoidsDataTable.PrimaryKey = New System.Data.DataColumn() {moLinkTable.Columns.Item(0)}

   End Sub
   Private Sub zzFillVoidsTable()
      Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
      Dim oPolyline As Polyline
      Dim oNewRow As System.Data.DataRow
      Dim oExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d
      Dim iCount As Integer = 1
      For Each tAcObjID As ObjectId In dicModelSpaceObjIDs

         oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, OpenMode.ForRead, True)
         If oPolyline IsNot Nothing AndAlso oPolyline.Layer = DMAcadExt.MapThemeData.WorkAreaBoundaryLayer Then

            oNewRow = moVoidsDataTable.NewRow()
            With oNewRow
               .Item(FDO.TplnPolygonSet.AcObjIDFieldName) = oPolyline.ObjectId
               '	.Item(msAcObjIDFieldName) = oPolyline.ObjectId.OldIdPtr.ToInt64()
               .Item(FDO.TplnPolygonSet.EntityNoFieldName) = iCount
               oExtents = oPolyline.GeometricExtents
               .Item(FDO.TplnPolygonSet.XminFieldName) = oExtents.MinPoint.X
               .Item(FDO.TplnPolygonSet.XmaxFieldName) = oExtents.MaxPoint.X
               .Item(FDO.TplnPolygonSet.YminFieldName) = oExtents.MinPoint.Y
               .Item(FDO.TplnPolygonSet.YmaxFieldName) = oExtents.MaxPoint.Y

               .Item(FDO.TplnPolygonSet.AreaNameFieldName) = Math.Round(oPolyline.Area, 4)
               .Item(FDO.TplnPolygonSet.LengthNameFieldName) = Math.Round(oPolyline.Length, 2)
               .Item(FDO.TplnPolygonSet.msGroupIDFieldName) = 0
               .Item(FDO.TplnPolygonSet.msGroupAddIDFieldName) = 0
               .Item(FDO.TplnPolygonSet.msOrderFieldName) = 0



            End With
            moVoidsDataTable.Rows.Add(oNewRow)
            iCount += 1
         End If
      Next
      If moVoidsDataTable.Rows.Count > 1 Then
         Me.lblVoidCount.BackColor = Color.Red
      Else
         Me.lblVoidCount.BackColor = Color.Empty
      End If


      If moVoidsDataTable.Rows.Count > 0 Then
         Me.lblVoidCount.Text = (moVoidsDataTable.Rows.Count - 1).ToString()
      Else
         Me.lblVoidCount.Text = "-"
      End If





   End Sub
   Private Sub zzPrintTable()
      For Each oRow As Data.DataRow In moIntersectionsDataTable.Rows
         For iIndex As Integer = 0 To moIntersectionsDataTable.Columns.Count - 1
            DMAcadExt.AcadDocument.WriteMessage("!!Item: " & iIndex.ToString() & " " & oRow.Item(iIndex).ToString())
         Next

      Next
   End Sub

   Private Sub zzPrintPolygonTable()
      Dim iRow As Integer = 0
      For Each oRow As Data.DataRow In moPolygonDataTable.Rows
         For iIndex As Integer = 0 To moPolygonDataTable.Columns.Count - 1
            DMAcadExt.AcadDocument.WriteMessage("!!Pgon: " & iIndex.ToString() & " " & oRow.Item(iIndex).ToString())
         Next
         iRow += 1
         If iRow = 7 Then
            Exit For
         End If
      Next
   End Sub
   Private Sub chkIntersections_CheckedChanged(oSender As System.Object, e As EventArgs)
      zzSetData()
   End Sub

   Private Sub rdbIntersections_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbIntersections.CheckedChanged
      zzSetData()
   End Sub

   Private Sub rdbBlocksOutside_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbBlocksOutside.CheckedChanged
      zzSetData()
   End Sub

   Private Sub rdbPolygons_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbPolygons.CheckedChanged
      zzSetData()
   End Sub

   Private Sub cmdMarkErrors_Click(oSender As System.Object, e As EventArgs) Handles cmdMarkErrors.Click
      'For Each oCol As System.Windows.Forms.DataGridViewColumn In dgvPolygons.Columns
      '   oCol.Visible = True
      'Next
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      If mbIntersections Then
         zzMarkIntersections()
      Else
         zzMarkPgons()
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub



   Private Sub cmdEraseErrors_Click(oSender As System.Object, e As EventArgs) Handles cmdEraseErrors.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      '  MessageBox.Show(msErrLayer, "05_430")
      If Not String.IsNullOrEmpty(msErrLayer) Then
         DMAcadExt.AcadTransaction.ClearLayerList(msErrLayer)
      End If
      DMAcadExt.MarkBlock.EraseAllReferences()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub zzMarkIntersections()


      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvPolygons.CurrentCell
      If oCurrentCell IsNot Nothing Then
         Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
         Dim oGridRow As DataGridViewRow = Me.dgvPolygons.Rows.Item(iCurrentRowIndex)

         Dim oPoint As DMAcadExt.TPlnPoint = Nothing
         Dim dDWGScale As System.Double = CDbl(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.ScaleSysVarName))
         Dim oDataRow As Data.DataRowView = DirectCast(oGridRow.DataBoundItem, Data.DataRowView)

         Dim tPLineObjID As ObjectId = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.AcObjIDFieldName), ObjectId)
         Dim tCheckPLineObjID As ObjectId = DirectCast(oDataRow.Item(FDO.TplnPolygonSet.CheckAcObjIDFieldName), ObjectId)


         Dim oPolyline As Polyline = DMAcadExt.AcadTransaction.GetPolyline(tPLineObjID, OpenMode.ForRead)
         Dim oCheckPolyline As Polyline = DMAcadExt.AcadTransaction.GetPolyline(tCheckPLineObjID, OpenMode.ForRead)
         FDO.TplnPolygonSet.PolygonRelation(oPolyline, oCheckPolyline, True)
         FDO.TplnPolygonSet.MarkIntersectionPoints()
      End If




      ' .Add(msIntersectCountFieldName, GetType(System.Int32))





   End Sub
   Private Sub zzMarkPgons()
      Dim bIsProper As Boolean
      Dim iCentroidCount As Integer
      Dim oBox As DMAcadExt.TPlnBoundingBox
      Dim iGroupID, iGroupAddID As Integer
      Dim lNo As Long

      Dim lKey As Long
      Dim dicNames As Dictionary(Of Long, Integer) = New Dictionary(Of Long, Integer)()
      Dim iFirstEntryIndex As Integer
      Dim iRowIndex As Integer = 0
      Dim oFirstEntryGridRow As Data.DataRowView
      Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Square)
      Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
      Dim tLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef(1, DMAcadExt.enLayerFunction.ErrorMarks)
      Dim bCurrentLayerOK As Boolean


      msErrLayer = tLayerDef.Name

      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, False)
      For Each oDataRow As Data.DataRowView In moMainDataView
         If Not bIsProper Then
            oBox = zzGetBox(oDataRow)
            oSquareMarkBlock.MarkBox(oBox, 4S)
         Else

            iCentroidCount = DMCommon.Functions.CIntN(oDataRow.Item(FDO.TplnPolygonSet.msCentroidCountFieldName))
            If iCentroidCount <> 1 Then

               oBox = zzGetBox(oDataRow)
               oSquareMarkBlock.MarkBox(oBox, 5S)
            Else
               iGroupID = DMCommon.Functions.CIntN(oDataRow.Item(FDO.TplnPolygonSet.msGroupIDFieldName))
               iGroupAddID = DMCommon.Functions.CIntN(oDataRow.Item(FDO.TplnPolygonSet.msGroupAddIDFieldName))
               lNo = DMCommon.Functions.CLngN(oDataRow.Item(FDO.TplnPolygonSet.msOrderFieldName))

               ' \frmPgonSetView.vb:line 663
               If (mbIsLot OrElse iGroupID <> 0) AndAlso lNo <> 0 Then
                  lKey = zzPacelToNum(iGroupID, iGroupAddID, lNo)

                  Try
                     If dicNames.TryGetValue(lKey, iFirstEntryIndex) Then
                        If iFirstEntryIndex <> miDoubleExists Then
                           oFirstEntryGridRow = moMainDataView.Item(iFirstEntryIndex)

                           zzMarkDoubleName(oFirstEntryGridRow, oCircleMarkBlock)
                           dicNames.Remove(lKey)
                           dicNames.Add(lKey, miDoubleExists)
                        End If
                        zzMarkDoubleName(oDataRow, oCircleMarkBlock)
                     Else
                        dicNames.Add(lKey, iRowIndex)
                     End If
                  Catch oEx As Exception
                     System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_989")
                  End Try

               End If




            End If
         End If
         iRowIndex += 1
      Next
   End Sub

   Private Sub rdbVoids_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbVoids.CheckedChanged
      zzSetData()
   End Sub

   Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
      Me.Close()
   End Sub

  
   Private Sub cmdPaint_Click(oSender As System.Object, e As EventArgs) Handles cmdPaint.Click
      Dim tAcObjID As ObjectId
      Dim oEnt As Entity
      Dim iColorIndex As Integer = 0

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      '    MessageBox.Show(moMainDataView.Count.ToString(), "05_410")
      For Each oDataRowView As Data.DataRowView In moMainDataView
         tAcObjID = DirectCast(oDataRowView.Item(FDO.TplnPolygonSet.AcObjIDFieldName), ObjectId)
         DMAcadExt.AcadDocument.WriteMessage(tAcObjID.ToString())
         oEnt = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, OpenMode.ForWrite)
         If oEnt IsNot Nothing Then
            If mbPgonsPainted Then
               oEnt.ColorIndex = 256

            Else
               oEnt.ColorIndex = DMAcadExt.AcadUtil.GetAcadColorIndex(iColorIndex)
               iColorIndex += 1
            End If

         End If
      Next
      mbPgonsPainted = Not mbPgonsPainted


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()

      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()





   End Sub

   Private Sub chkLayers_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkLayers.CheckedChanged
      If mbEventsEnabled Then
         '  Dim saBlockLayers() As String = {"1601"}
         '  Dim saParcelLayers() As String = {"C1602_", "C1603_"}
         Dim saLayers() As String
         If mbDissolve Then
            saLayers = New String() {mtDissolveMapThemeData.CentroidLayer, mtDissolveMapThemeData.ClosedPgonsLayer, msIntersectionPointsLayer}
         Else
            saLayers = New String() {mtSourceMapThemeData.CentroidLayer, mtSourceMapThemeData.ClosedPgonsLayer, msIntersectionPointsLayer}
         End If

         '   DMCommon.Functions.DispArray(saLayers, "saLayers", True)


         Dim tLayerList As DMCommon.dmList = New DMCommon.dmList(saLayers)
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()

         If Me.chkLayers.Checked Then
            mdicLayersOffStatus = DMAcadExt.AcadTransaction.GetLayersOffStatus(tLayerList)


         Else
            DMAcadExt.AcadTransaction.SetLayersOffStatus(mdicLayersOffStatus)
         End If

         DMAcadExt.AcadTransaction.Terminate()

         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If



   End Sub

   Private Sub chkWorkAreaBorder_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkWorkAreaBorder.CheckedChanged
      If mbEventsEnabled Then
         Dim saLayers() As String = {DMAcadExt.MapThemeData.WorkAreaBoundaryLayer}

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()

         If Me.chkWorkAreaBorder.Checked Then
            DMAcadExt.AcadTransaction.SetLayersOn(saLayers, True)


         Else
            DMAcadExt.AcadTransaction.SetLayersOn(saLayers, False)
         End If

         DMAcadExt.AcadTransaction.Terminate()

         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.UpdateScreen()
      End If
   End Sub

End Class