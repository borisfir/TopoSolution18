Option Explicit On
Option Strict On
Imports System.Data
Public Class frmPaintLanduse
   Private Const msIntersectionPointsLayer As String = "zzIntersPoints"
   Private Const miParamType As Integer = 4
   Private Const mdBaseScale As Double = 1000.0
   Private mtMapThemeData As DMAcadExt.MapThemeData
   Private mtBlueLineMapThemeData As DMAcadExt.MapThemeData
   Private mtTopoSelection As TopoSelection
   'Private lblTopoExists As System.Windows.Forms.Label
   Protected doBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doLabelBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doCaptionBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte)) '9.75
   Private Shared moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
#Region "ToolStrip_LanduseTopo"
   Private tsbLanduseCreateTopo As System.Windows.Forms.ToolStripButton
   Private tsbLanduseCheckTopo As System.Windows.Forms.ToolStripButton
   Private tsbLanduseDeleteTopo As System.Windows.Forms.ToolStripButton
   Private tsbLanduseShowTopo As System.Windows.Forms.ToolStripButton
   Private WithEvents ddbLanduseLayers As System.Windows.Forms.ToolStripDropDownButton
   Private tsbLanduseGetStatistics As System.Windows.Forms.ToolStripButton
   Private tsbLanduseExec As System.Windows.Forms.ToolStripButton
   Private tsiLanduseThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
   Private tsiLanduseThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
   Private tsiLanduseAllVisible As System.Windows.Forms.ToolStripMenuItem
#End Region
#Region "ToolStrip_BlueLineTopo"
   Private tsbBlueLineCreateTopo As System.Windows.Forms.ToolStripButton
   Private tsbBlueLineCheckTopo As System.Windows.Forms.ToolStripButton
   Private tsbBlueLineDeleteTopo As System.Windows.Forms.ToolStripButton
   Private tsbBlueLineShowTopo As System.Windows.Forms.ToolStripButton
   Private WithEvents ddbBlueLineLayers As System.Windows.Forms.ToolStripDropDownButton
   Private tsbBlueLineGetStatistics As System.Windows.Forms.ToolStripButton
   Private tsbBlueLineExec As System.Windows.Forms.ToolStripButton
   Private tsiBlueLineThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
   Private tsiBlueLineThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
   Private tsiBlueLineAllVisible As System.Windows.Forms.ToolStripMenuItem
#End Region
   Private miProjectCode As Integer
   Private miDetailNo As Integer
   Private moParams As TPlServerDB.dmParams
   Private moPgonSet As FDO.TplnPolygonSet
   Private mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private miPaintMethod As enPaintMethod
   Private miPolygonsCount As Integer
   Private miSourcePolygonsCount As Integer

   Private miDissolvePolygonsCount As Integer

   Private Shared mdicColorSchemes As IDictionary(Of Integer, DMAcadExt.ColorScheme)
   Private WithEvents mfEditColorSet As frmEditColorSet
   Private Structure TopoSelection
      Private mbOpened As Boolean
      Private miCount As Integer
      Private miMinIndex As Integer

      Private miMaxIndex As Integer
      Private mhsExclusions As HashSet(Of Long)
      Public Sub New(iCount As Integer, iMinIndex As Integer, iMaxIndex As Integer)
         If iCount > 0 Then
            If iMinIndex > 1 AndAlso iMinIndex <= iCount Then
               miMinIndex = iMinIndex
               mbOpened = True
            Else
               miMinIndex = 1
            End If
            If iMaxIndex >= iMinIndex AndAlso iMaxIndex < iCount Then
               miMaxIndex = iMaxIndex
               mbOpened = True
            Else
               miMaxIndex = iCount
            End If


         End If
         '   DMCommon.Debug.MsgBox("08_422", True, miMinIndex, miMaxIndex, mbOpened)
      End Sub

      ReadOnly Property MinIndex As Integer
          
         Get
            Return miMinIndex
         End Get
      End Property
      ReadOnly Property MaxIndex As Integer
         
         Get
            Return miMaxIndex
         End Get
      End Property
      ReadOnly Property IndexCount As Integer
         Get
            Return miMaxIndex - miMinIndex + 1
         End Get
      End Property
      Property Count As Integer
         Set(iValue As Integer)
            miCount = iValue
         End Set
         Get
            Return miCount
         End Get
      End Property
      Sub AddExclusionRange(sValueList As String, iNumberStyle As System.Globalization.NumberStyles)
         Dim saValues() As String = Split(sValueList, ",")
         Dim lID As Long
         Dim iHexNumberStyle As System.Globalization.NumberStyles = Globalization.NumberStyles.HexNumber
         '    Dim iTopoID As Integer
         ' Dim iNumberStyle As System.Globalization.NumberStyles = Globalization.NumberStyles.HexNumber
         Dim oCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo("he-IL")

         If mhsExclusions Is Nothing Then
            mhsExclusions = New HashSet(Of Long)()
         End If
         For iIndex As Integer = 0 To saValues.GetUpperBound(0)

            If Long.TryParse(saValues(iIndex), iNumberStyle, oCulture, lID) Then
               mbOpened = True
               mhsExclusions.Add(lID)
            End If
         Next
         ' DMCommon.Debug.MsgBox("08_438", True, mhsExclusions.Count)
      End Sub
      Function Contains(iIndex As Integer, Optional lID As Long = 0L) As Boolean
         Dim bRes As Boolean
        
         If mbOpened Then
            If iIndex >= miMinIndex AndAlso iIndex <= miMaxIndex Then
               bRes = True
            Else
               bRes = False
            End If
         Else
            bRes = True
         End If
         If bRes Then
            If mhsExclusions Is Nothing OrElse lID = 0L Then
               Return True
            Else

               Return Not mhsExclusions.Contains(lID)
            End If
         Else
            Return False
         End If


      End Function
      Function Contains(iIndex As Integer, Optional iID As Integer = 0) As Boolean
         Dim lID As Long = Convert.ToInt64(iID)
         Return Contains(iIndex, lID)
      End Function
      Function ContainsOld(iIndex As Integer, Optional iTopoID As Integer = 0) As Boolean
         If mbOpened AndAlso iIndex >= miMinIndex AndAlso iIndex <= miMaxIndex Then
            If mhsExclusions Is Nothing OrElse iTopoID = 0 Then
               Return True
            Else
               Return Not mhsExclusions.Contains(iTopoID)
            End If
         End If
      End Function
      Public ReadOnly Property Exclusions As HashSet(Of Long)
         Get
            Return mhsExclusions
         End Get
      End Property
   End Structure
 
   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tBlueLineMapThemeData As DMAcadExt.MapThemeData)
      miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode

      miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
      mtMapThemeData = tMapThemeData
      mtBlueLineMapThemeData = tBlueLineMapThemeData
      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()

      Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName

      Me.lblSourceTopoName.Text = mtMapThemeData.LineTopoName
      Me.lblDissolveTopoName.Text = sLanduseTopoName

    

      mdicColorSchemes = New Dictionary(Of Integer, DMAcadExt.ColorScheme)
      zzLoadParams()
      If mtMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
         Me.lblSourcePgonLayers.Text = mtMapThemeData.ClosedPgonsLayers
         Me.rdbClosedPgons.Checked = True
      End If
   End Sub
   Public Property PgonSet As FDO.TplnPolygonSet
      Get
         Return moPgonSet
      End Get
      Set(oValue As FDO.TplnPolygonSet)
         moPgonSet = oValue
      End Set
   End Property

   Private Sub zzLoadParams()

      moParams = New TPlServerDB.dmParams(miProjectCode, miDetailNo, mtMapThemeData.MapThemeID, miParamType)
      Dim iPaintTopoOpt As Integer = moParams.GetIntValue(0)
      If iPaintTopoOpt = 0 Then
         Me.rdbDissolve.Checked = True
      ElseIf iPaintTopoOpt = 1 Then
         Me.rdbSource.Checked = True
      Else
         Me.rdbDissolve.Checked = True
      End If
      zzSetPgonCount()
      Dim dScale As Double = moParams.GetDblValue(1)
      Dim iScale As Integer = CInt(dScale)
      Dim sScale As String = "1:" & CStr(iScale)
      Try
         Me.cmbPaintScale.Text = sScale
      Catch oEx As Exception

      End Try
      Dim dLegendScale As Double = moParams.GetDblValue(2)
      Me.txtLegendPaintFactor.Text = CStr(dLegendScale)
   End Sub
   Private Sub zzSaveParams()
      Try
         Dim iPaintTopoOpt As Integer = -1 '= moParams.GetIntValue(0)
         If Me.rdbDissolve.Checked Then
            iPaintTopoOpt = 0
         ElseIf Me.rdbSource.Checked Then
            iPaintTopoOpt = 1
         End If
         moParams.SetValue(0, iPaintTopoOpt)

         Dim dScale As Double = zzGetSelectedScale()
         moParams.SetValue(1, dScale)
      Catch oEx As Exception

      End Try




      Dim dLegendScale As Double
      Try
         dLegendScale = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
         moParams.SetValue(2, dLegendScale)
      Catch oEx As Exception

      End Try
      Try
         moParams.Update()
      Catch ex As Exception

      End Try



   End Sub
   Private Sub zzFillPaintScale()
      Const sComText As String = "SELECT ID,Name FROM Scales"
      Me.cmbPaintScale.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Scales")
   End Sub

   Private Sub zzCheckSourceTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.LineTopoName, bLockDoc, bMsg, mtMapThemeData.MapThemeID)
      zzDispSourceTopoOK(tTopoRes)
   End Sub
   Private Sub zzCheckDissolveTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.DissolveTopoName, bLockDoc, bMsg, mtMapThemeData.MapThemeID)

      zzDispDissolveTopoOK(tTopoRes)
   End Sub
   Private Sub zzCheckBlueLineTopo(bLockDoc As Boolean, ByVal bMsg As Boolean)
      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtBlueLineMapThemeData.LineTopoName, bLockDoc, bMsg, mtMapThemeData.MapThemeID)

      zzDispBlueLineTopoOK(tTopoRes)
   End Sub
   Private Sub zzDispSourceTopoOK(tTopoRes As DMAcadExt.TopoRes)
      Dim bOK As Boolean = tTopoRes.IsOK
      Me.lblSourceTopoExists.Visible = bOK
      Me.txtSourcePgonCount.Enabled = bOK
      With lblSourceTopoName
         If bOK Then
            .ForeColor = System.Drawing.SystemColors.ControlText
            .Font = doLabelBoldFont
         Else
            .ForeColor = System.Drawing.SystemColors.GrayText
            .Font = doLabelFont
         End If
      End With
      Me.txtSourcePgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
      miSourcePolygonsCount = tTopoRes.PgonCount
      zzSetPgonCount()
   End Sub
   Private Sub zzSetPgonCount()
      If Me.rdbSource.Checked Then
         zzSetPgonCountA(miSourcePolygonsCount)
      ElseIf Me.rdbDissolve.Checked Then
         zzSetPgonCountA(miDissolvePolygonsCount)
      End If
   End Sub
   Private Sub zzSetPgonCountA(iPolygonsCount As Integer)
      miPolygonsCount = iPolygonsCount
      Me.txtFromNumber.Text = "1"
      Me.txtToNumber.Text = iPolygonsCount.ToString()


   End Sub
   Private Sub zzSetCondition()
      Dim iFrom As Integer
      Dim iTo As Integer

      Integer.TryParse(Me.txtFromNumber.Text, iFrom)
       
      Integer.TryParse(Me.txtToNumber.Text, iTo)
      mtTopoSelection = New TopoSelection(miPolygonsCount, iFrom, iTo)
      If Me.txtException.Text.Trim.Length > 0 Then
         mtTopoSelection.AddExclusionRange(Me.txtException.Text, Globalization.NumberStyles.None)
      End If
   End Sub

   Private Sub zzDispSourceTopoExists(bExists As Boolean)
      Me.lblSourceTopoExists.Visible = bExists
      Me.txtSourcePgonCount.Enabled = bExists
      With lblSourceTopoName
         If bExists Then
            .ForeColor = System.Drawing.SystemColors.ControlText
            .Font = doLabelBoldFont
         Else
            .ForeColor = System.Drawing.SystemColors.GrayText
            .Font = doLabelFont
         End If
      End With

   End Sub

   Private Sub zzDispDissolveTopoOK(tTopoRes As DMAcadExt.TopoRes)
      Dim bOK As Boolean = tTopoRes.IsOK
      Me.lblDissolveTopoExists.Visible = bOK
      Me.txtDissolvePgonCount.Enabled = bOK
      With lblDissolveTopoName
         If bOK Then
            .ForeColor = System.Drawing.SystemColors.ControlText
            .Font = doLabelBoldFont
         Else
            .ForeColor = System.Drawing.SystemColors.GrayText
            .Font = doLabelFont
         End If
      End With
      Me.txtDissolvePgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
      miDissolvePolygonsCount = tTopoRes.PgonCount
      zzSetPgonCount()
   End Sub
   Private Sub zzDispBlueLineTopoOK(tTopoRes As DMAcadExt.TopoRes)

   End Sub


   Private Sub zzDispDissolveTopoExists(bExists As Boolean)
      Me.lblDissolveTopoExists.Visible = bExists
      Me.txtDissolvePgonCount.Enabled = bExists
      With lblDissolveTopoName
         If bExists Then
            .ForeColor = System.Drawing.SystemColors.ControlText
            .Font = doLabelBoldFont
         Else
            .ForeColor = System.Drawing.SystemColors.GrayText
            .Font = doLabelFont
         End If
      End With

   End Sub
   Private Sub zzMyInitializeComponent()
      zzFillPaintScale()
      zzInitLanduseToolStrip()
      zzInitBlueLineToolStrip()
   End Sub
   Private Sub zzInitLanduseToolStrip()
      Me.tsbLanduseCreateTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbLanduseCheckTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbLanduseDeleteTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbLanduseShowTopo = New System.Windows.Forms.ToolStripButton
      Me.ddbLanduseLayers = New System.Windows.Forms.ToolStripDropDownButton

      Me.tsbLanduseGetStatistics = New System.Windows.Forms.ToolStripButton
      Me.tsbLanduseExec = New System.Windows.Forms.ToolStripButton

      Me.tsiLanduseThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiLanduseThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiLanduseAllVisible = New System.Windows.Forms.ToolStripMenuItem()

      '
      'tsbLanduseCreateTopo
      '
      With Me.tsbLanduseCreateTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.CreateTopo
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbLanduseCreateTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
         'msCreateTopoText
      End With
      '
      'tsbLanduseCheckTopo
      '
      With Me.tsbLanduseCheckTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Validate1
         .Name = "tsbLanduseCheckTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
      End With
      '
      'tsbLanduseDeleteTopo
      '
      With tsbLanduseDeleteTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Delete
         .Name = "tsbLanduseDeleteTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrDeleteTopo
      End With
      '
      'tsbLanduseShowTopo
      '
      With tsbLanduseShowTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.ShowTopo
         .Name = "tsbLanduseShowTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
      End With

      '
      'tsiLanduseThisTopoOnlyVisible
      '
      With Me.tsiLanduseThisTopoOnlyVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiLanduseThisTopoOnlyVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Only"
      End With
      '
      'tsiLanduseAllVisible
      '
      With Me.tsiLanduseAllVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiLanduseAllVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "All Visible"
      End With
      '
      'tsiLanduseThisTopoVisible
      '
      With Me.tsiLanduseThisTopoVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiLanduseThisTopoVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Visible"
      End With

      '
      'ddbLanduseLayers
      '
      With ddbLanduseLayers
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
         .Name = "ddbLanduseLayers"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoLayers
         .DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiLanduseThisTopoOnlyVisible, Me.tsiLanduseThisTopoVisible, Me.tsiLanduseAllVisible})
      End With
      '
      'tsbLanduseGetStatistics
      '
      With Me.tsbLanduseGetStatistics
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.QuestionMark
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbLanduseGetStatistics"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoStatistics
      End With

      '
      'tstLanduseTopology
      '
      '	Me.tstTopology = New System.Windows.Forms.ToolStrip()
      With Me.tstLanduseTopology
         .Dock = System.Windows.Forms.DockStyle.None

         .Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbLanduseGetStatistics, Me.tsbLanduseExec, Me.ddbLanduseLayers, Me.tsbLanduseShowTopo, Me.tsbLanduseDeleteTopo, Me.tsbLanduseCheckTopo, Me.tsbLanduseCreateTopo})
         ', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
         '.Location = New System.Drawing.Point(300, 2)
         .Name = "tstLanduseTopology"
         '	.Size = New System.Drawing.Size(87, 25)
         .TabIndex = 0
      End With

   End Sub
   Private Sub zzCreateLanduseTopology()
      Dim sAttribExpr As String = mtMapThemeData.DissolveAttribExpr   '"@CODE"
      Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()

      zzCreateDissolveTopology(sLanduseTopoName, sAttribExpr)
      Me.zzCheckDissolveTopo(False, False)
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzCreateBlueLineTopology()
      Dim sAttribExpr As String = TopoManager.TPlanGraph.TplnLot.PlanDissolveAttribExpr    '"@PLAN"
      Dim sLanduseTopoName As String = mtBlueLineMapThemeData.LineTopoName
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(mtBlueLineMapThemeData.LineLinkLayers, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
      If bCurrentLayerOK Then
         zzCreateDissolveTopology(sLanduseTopoName, sAttribExpr)
      End If
      Me.zzCheckBlueLineTopo(False, False)
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzCreateDissolveTopology(sDissolveTopoName As String, sAttribExpr As String)
      Dim sLotTopoName As String = mtMapThemeData.LineTopoName
      '	Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName


      If TopoManager.TopoCreator.TopologyExists(sDissolveTopoName) Then
         System.Windows.Forms.MessageBox.Show("הטופולוגיה כבר קיימת", "21_400")
      Else


         TopoManager.TopoCreator.DissolveTopo(sLotTopoName, sAttribExpr, sDissolveTopoName)


      End If

   End Sub
   Private Sub zzThisOnlyVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
      oEntity.Visible = bCond
   End Sub
   Private Sub zzThisVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
      If bCond Then
         oEntity.Visible = True
      End If
   End Sub
   Private Sub zzLanduseOnlyVisible()
      Dim iLine, iCenter As Integer
      Dim sLotTopoName As String = mtMapThemeData.LineTopoName
      Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
      Dim oLanduseTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLanduseTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim resOBjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = oLanduseTopology.GetEntityIds()
      iLine = resOBjIDs.Count
      Dim oLotTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLotTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim colLotPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oLotTopology.GetPolygons()
      For Each oPgon As Autodesk.Gis.Map.Topology.Polygon In colLotPolygons
         resOBjIDs.Add(oPgon.Entity)
      Next
      iCenter = resOBjIDs.Count - iLine

      Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)

      '	MessageBox.Show(CStr(iLine) & ":" & CStr(iCenter), "01_677")

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      '	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & sLineLinkLayers & vbCrLf & ptMapThemeData.CentroidBlocks & vbCrLf & ptMapThemeData.CentroidLayers, "02_102")
      DMAcadExt.AcadTransaction.ProcByList(dlEntityProc, resOBjIDs)


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()


   End Sub
   Private Sub zzInitBlueLineToolStrip()
      Me.tsbBlueLineCreateTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbBlueLineCheckTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbBlueLineDeleteTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbBlueLineShowTopo = New System.Windows.Forms.ToolStripButton
      Me.ddbBlueLineLayers = New System.Windows.Forms.ToolStripDropDownButton

      Me.tsbBlueLineGetStatistics = New System.Windows.Forms.ToolStripButton
      Me.tsbBlueLineExec = New System.Windows.Forms.ToolStripButton

      Me.tsiBlueLineThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiBlueLineThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiBlueLineAllVisible = New System.Windows.Forms.ToolStripMenuItem()

      '
      'tsbBlueLineCreateTopo
      '
      With Me.tsbBlueLineCreateTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.CreateTopo
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbBlueLineCreateTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
         'msCreateTopoText
      End With
      '
      'tsbBlueLineCheckTopo
      '
      With Me.tsbBlueLineCheckTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Validate1
         .Name = "tsbBlueLineCheckTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrCreateTopo
      End With
      '
      'tsbBlueLineDeleteTopo
      '
      With tsbBlueLineDeleteTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Delete
         .Name = "tsbBlueLineDeleteTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrDeleteTopo
      End With
      '
      'tsbBlueLineShowTopo
      '
      With tsbBlueLineShowTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.ShowTopo
         .Name = "tsbBlueLineShowTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
      End With

      '
      'tsiBlueLineThisTopoOnlyVisible
      '
      With Me.tsiBlueLineThisTopoOnlyVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiBlueLineThisTopoOnlyVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Only"
      End With
      '
      'tsiBlueLineAllVisible
      '
      With Me.tsiBlueLineAllVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiBlueLineAllVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "All Visible"
      End With
      '
      'tsiBlueLineThisTopoVisible
      '
      With Me.tsiBlueLineThisTopoVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiBlueLineThisTopoVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Visible"
      End With

      '
      'ddbBlueLineLayers
      '
      With ddbBlueLineLayers
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
         .Name = "ddbBlueLineLayers"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoLayers
         .DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiBlueLineThisTopoOnlyVisible, Me.tsiBlueLineThisTopoVisible, Me.tsiBlueLineAllVisible})
      End With
      '
      'tsbBlueLineGetStatistics
      '
      With Me.tsbBlueLineGetStatistics
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.QuestionMark
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbBlueLineGetStatistics"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrTopoStatistics
      End With



      '
      'tstBlueLineTopology
      '
      '	Me.tstTopology = New System.Windows.Forms.ToolStrip()


   End Sub
   Private Sub zzPaintByLanduseAAA(sTopoName As String)
      Dim oTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopology.GetPolygons()
      Dim oPgon As TopoManager.TPlanGraph.TplnTopoPgon = Nothing
      For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
         'oPgon = New TopoManager.TPlanGraph.TplnLusePgon(iTopoPurpose, oPolygon)
      Next
   End Sub
   Private Sub cmdPaint_Click(ByVal oSender As System.Object, ByVal oEventArgs As System.EventArgs) Handles cmdPaint.Click
      '	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
      Dim dBaseScale As Double = 1000.0

      '	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
      Dim sPaintLayer As String
      Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
      zzSetCondition()
      '   DMCommon.Debug.MsgBox("08_561", False, mtTopoSelection, mtTopoSelection.Exclusions, mtTopoSelection.Exclusions.Count)

      '   MessageBox.Show(mtMapThemeData.MapThemeID.ToString() & vbCrLf & mtMapThemeData.TopoPurpose.ToString(), "09_560")
      If iTopoPurpose <> DMAcadExt.enTopoPurpose.Undefined Then
         Me.Cursor = Cursors.WaitCursor
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

         '	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_201")
         DMAcadExt.AcadDocument.SaveVarCmdDia(0S)
         ''''''''	Dim sLotTopoName As String = mtMapThemeData.LineTopoName
         Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
         sPaintLayer = TopoManager.TPlanGraph.TplnProject.SetPaintLayers(mtMapThemeData.MapThemeID, iTopoPurpose)
         '    Dim sLayer As String = DMAcadExt.AcadTransaction.GetCurrentLayer()
         Dim bCurrentLayerOK As Boolean = True

         Dim tLayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(mtMapThemeData.MapThemeID)  ', iTopoPurpose
         '   MessageBox.Show(sPaintLayer & ":" & sLayer, "21_320")

         zzFillColorSchemesDic()

         Me.prbPaint.Visible = True
         If sPaintLayer IsNot Nothing Then
            If Not Me.chkLayerByLanduse.Checked Then
               DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
            End If
            If mtMapThemeData.MapThemeID = DMAcadExt.enMapTheme.LotApproved OrElse mtMapThemeData.MapThemeID = DMAcadExt.enMapTheme.LotProposed Then
               If rdbDissolve.Checked Then
                  ''''''''''''''''''''''''''TopoManager.TPlanGraph.TplnProject.LoadLusePgons(iTopoPurpose, sLotTopoName, sLanduseTopoName)
                  '        MessageBox.Show(iTopoPurpose.ToString() & vbCrLf & sPaintLayer, "04_254")
                  zzPaintByLanduseTopo(iTopoPurpose, tLayerDef, Me.chkLayerByLanduse.Checked, Me.chkBypassLine.Checked, mtMapThemeData.DissolveTopoName)

               ElseIf rdbSource.Checked Then

                  'MessageBox.Show(sPaintLayer, "04_254")
                  '	TopoManager.TPlanGraph.TplnLot.PaintAll(mtMapThemeData.MapThemeID, iTopoPurpose, dScale, sPaintLayer)
                  zzPaintAllLots(iTopoPurpose, tLayerDef, Me.chkLayerByLanduse.Checked, Me.chkBypassLine.Checked, mtMapThemeData.LineTopoName)

               ElseIf rdbClosedPgons.Checked Then

                  zzPaintLotPgonset(tLayerDef, Me.chkLayerByLanduse.Checked)
               End If
            ElseIf mtMapThemeData.MapThemeID = DMAcadExt.enMapTheme.Parcels Then
               zzPaintAllParcels(tLayerDef, Me.chkLayerByLanduse.Checked)
            End If
            Me.prbPaint.Hide()
            Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
            If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
               moParams.SetIdData(sFileName)
               moParams.Update()
            End If


         End If

         Me.Cursor = Cursors.Default
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.CloseLog()
         DMAcadExt.AcadDocument.UpdateScreen()
         '  MessageBox.Show("", "04_999")

      End If
   End Sub
   Private Sub zz(sSourceTopoName As String)

      Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = New TopoManager.TopoScheme.tsTopology(sSourceTopoName)
      ' MessageBox.Show(sSourceTopoName, "03_123")
      oTopoScheme.Load(False)
      '  MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "03_124")
      oTopoScheme.CreateDBPolylines()

   End Sub
   Private Function zzGetSelectedScale() As Double
      Dim sScale As String

      Dim dScale As Double = 0.0
      Dim oDyn As System.Object
      If Me.cmbPaintScale.SelectedIndex >= 0 Then
         oDyn = Me.cmbPaintScale.SelectedItem
         If oDyn IsNot Nothing Then
            sScale = Me.cmbPaintScale.GetItemText(oDyn)
            dScale = DMCommon.Functions.TextToScale(sScale, mdBaseScale)
         End If
      Else
         MessageBox.Show(Me.cmbPaintScale.SelectedIndex.ToString(), "16_711")
      End If
      Return dScale
   End Function

   Private Sub zzPaintAllLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal tLayerDef As DMAcadExt.AcadLayerDef, ByVal bLayerByLanduseID As Boolean, ByVal bBypassLine As Boolean, Optional sTopoName As String = Nothing)
      Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
      '	Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
      Dim dicLots As TopoManager.TPlanGraph.TplnLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
      Dim sLanduseNotFoundList As String = String.Empty
      Dim sPaintLayer As String = String.Empty
      Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = Nothing
      Dim bCurrentLayerOK As Boolean
      Dim iPolygonIndex As Integer
      If dicLots Is Nothing Then
         zzLoadGraph()
         dicLots = TopoManager.TPlanGraph.TplnProject.Lots(iTopoPurpose)
      End If
      If bBypassLine Then
         oTopoScheme = New TopoManager.TopoScheme.tsTopology(sTopoName)
         oTopoScheme.Load(False)
      End If
      If dicLots IsNot Nothing Then
         Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
         Me.prbPaint.Maximum = mtTopoSelection.IndexCount
         Me.prbPaint.Step = 1

         For Each oLot As TopoManager.TPlanGraph.TplnLot In dicLots.Values
            iPolygonIndex += 1
            'tColorScheme = mdicColorSchemes.Item(oLot.LanduseID)
            'MessageBox.Show(CStr(mdicColorSchemes.Count) & vbCrLf & CStr(oLot.LanduseID) & ";" & CStr(oLot.Name), "04_466")
            If mdicColorSchemes.TryGetValue(oLot.LanduseID, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
               If mtTopoSelection.Contains(iPolygonIndex, oLot.TopoID) Then
                  If oLot.TopoID = 2018 Then
                     '  DMCommon.Debug.MsgBox("++08_122", True, oLot.TopoID, iPolygonIndex)
                  End If
                  DMAcadExt.AcadDocument.Counter = iPolygonIndex
                  If bLayerByLanduseID Then
                     ' DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True, oLot.LanduseID)
                     bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, False, False, False, oLot.LanduseID)
                     sPaintLayer = tLayerDef.NamePlusExtension(oLot.LanduseID)
                  End If
                  If bCurrentLayerOK AndAlso bBypassLine AndAlso (oTopoScheme IsNot Nothing) Then
                     oTopoScheme.CreatePgonDBPolyline(oLot.TopoID)
                  End If
                  If oLot.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sPaintLayer) <> DMAcadExt.PaintException.OK Then
                     Exit For
                  End If
                  prbPaint.Increment(1)
               End If
            Else
               If sLanduseNotFoundList.Length <> 0 Then
                  sLanduseNotFoundList &= ","
               End If
               sLanduseNotFoundList &= CStr(oLot.LanduseID)
            End If



         Next
         '	MessageBox.Show(iPaintException.ToString(), "01_780")
         If sLanduseNotFoundList.Length <> 0 Then
            MessageBox.Show(sLanduseNotFoundList, "#187: Not Found")
         End If
      End If
   End Sub
   Private Sub zzPaintByLanduseTopo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal tLayerDef As DMAcadExt.AcadLayerDef, ByVal bLayerByLanduseID As Boolean, ByVal bBypassLine As Boolean, Optional sTopoName As String = Nothing)
      Dim dicLusePgons As IDictionary(Of Integer, TopoManager.TPlanGraph.TplnLusePgon) = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
      Dim dicNotFoundLuses As SortedDictionary(Of Integer, String) = New SortedDictionary(Of Integer, String)
      Dim sLanduseNotFoundList As String = String.Empty
      Dim bCurrentLayerOK As Boolean

      Dim oTopoScheme As TopoManager.TopoScheme.tsTopology = Nothing
      Dim sPaintLayer As String = String.Empty
      Dim iPolygonIndex As Integer
      
      If bBypassLine Then
         oTopoScheme = New TopoManager.TopoScheme.tsTopology(sTopoName)
         oTopoScheme.Load(False)
      End If


      '	MessageBox.Show(CStr(dicLusePgons Is Nothing), "01_213a")
      If dicLusePgons Is Nothing Then
         zzLoadGraph()
         dicLusePgons = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
      End If

      If dicLusePgons IsNot Nothing Then
         '	MessageBox.Show(CStr(dicLusePgons.Count) & ":" & CStr(mdicColorSchemes.Count), "04_500")
         Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
         Me.prbPaint.Maximum = mtTopoSelection.IndexCount
         Me.prbPaint.Step = 1
         DMAcadExt.AcadDocument.ResetCounter()
         For Each oLusePgon As TopoManager.TPlanGraph.TplnLusePgon In dicLusePgons.Values
            '	tColorScheme = TopoManager.TPlanGraph.TplnLot.GetColorScheme(iTopoPurpose, oLusePgon.LanduseID, dScale)
            '	tColorScheme = mdicColorSchemes.Item(oLusePgon.LanduseID)
            iPolygonIndex += 1
            If oLusePgon.LanduseID <> 0 Then
               If mdicColorSchemes.TryGetValue(oLusePgon.LanduseID, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
                  If mtTopoSelection.Contains(iPolygonIndex, oLusePgon.TopoID) Then
                     DMAcadExt.AcadDocument.Counter = iPolygonIndex
                     If bLayerByLanduseID Then
                        '  DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True, oLusePgon.LanduseID)
                        bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, False, False, False, oLusePgon.LanduseID)
                        sPaintLayer = tLayerDef.NamePlusExtension(oLusePgon.LanduseID)
                     End If

                     If oLusePgon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sPaintLayer, False) <> DMAcadExt.PaintException.OK Then
                        DMAcadExt.AcadDocument.WriteMessageLog("Temp#09; ")
                        Exit For
                     End If
                     If bCurrentLayerOK AndAlso bBypassLine AndAlso (oTopoScheme IsNot Nothing) Then
                        oTopoScheme.CreatePgonDBPolyline(oLusePgon.TopoID)
                     End If
                     prbPaint.Increment(1)
                  End If
               Else
                  If sLanduseNotFoundList.Length <> 0 Then
                     sLanduseNotFoundList &= ","
                  End If
                  sLanduseNotFoundList &= CStr(oLusePgon.LanduseID)
                  If Not dicNotFoundLuses.ContainsKey(oLusePgon.LanduseID) Then
                     dicNotFoundLuses.Add(oLusePgon.LanduseID, oLusePgon.LanduseID.ToString)
                  End If
               End If
            End If

         Next

         If sLanduseNotFoundList.Length <> 0 Then
            MessageBox.Show(sLanduseNotFoundList, "#188: Not Found")
         End If
         If dicNotFoundLuses.Count > 0 Then
            sLanduseNotFoundList = String.Empty
            For Each dValue As String In dicNotFoundLuses.Values
               If sLanduseNotFoundList.Length <> 0 Then
                  sLanduseNotFoundList &= ","
               End If
               sLanduseNotFoundList &= dValue
            Next
            MessageBox.Show(sLanduseNotFoundList, "#189: Not Found")
         End If

      End If
   End Sub
   Private Sub zzPaintAllParcels(ByVal tLayerDef As DMAcadExt.AcadLayerDef, ByVal bLayerByLanduseID As Boolean)
      Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
      '	Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
      Dim dicParcels As TopoManager.TPlanGraph.TplnParcels = TopoManager.TPlanGraph.TplnProject.Parcels
      Dim sLanduseNotFoundList As String = String.Empty
      Dim sPaintLayer As String = String.Empty
      Dim bCurrentLayerOK As Boolean
      If dicParcels Is Nothing Then
         zzLoadGraph()
         dicParcels = TopoManager.TPlanGraph.TplnProject.Parcels
      End If

      If dicParcels IsNot Nothing Then

         Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
         Me.prbPaint.Maximum = dicParcels.Count
         Me.prbPaint.Step = 1

         For Each oParcel As TopoManager.TPlanGraph.TplnParcel In dicParcels.Values
            'tColorScheme = mdicColorSchemes.Item(oLot.LanduseID)
            'MessageBox.Show(CStr(mdicColorSchemes.Count) & vbCrLf & CStr(oLot.LanduseID) & ";" & CStr(oLot.Name), "04_466")
            If mdicColorSchemes.TryGetValue(oParcel.Owner, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
               If bLayerByLanduseID Then
                  '  DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True, oParcel.Owner)

                  bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, False, False, False, oParcel.Owner)





                  sPaintLayer = tLayerDef.NamePlusExtension(oParcel.Owner)
               End If

               If oParcel.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sPaintLayer) <> DMAcadExt.PaintException.OK Then
                  Exit For
               End If

            Else
               If sLanduseNotFoundList.Length <> 0 Then
                  sLanduseNotFoundList &= ","
               End If
               sLanduseNotFoundList &= CStr(oParcel.Owner)
            End If

            prbPaint.Increment(1)

         Next
         '	MessageBox.Show(iPaintException.ToString(), "01_780")
         If sLanduseNotFoundList.Length <> 0 Then
            MessageBox.Show(sLanduseNotFoundList, "#187: Not Found")
         End If
      End If
   End Sub
   
   Public Sub GetLanduseData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef oDataView As System.Data.DataView, ByVal dColorSchemeScale As Double)
      Const msColorFieldName As String = "Color"
      Const msLanduseOrderFieldName As String = "LanduseOrder"
      Const msNameFieldName As String = "Name"
      Const sSPName As String = "GetColorSchemeSet"
      Const bFirstColorSchemeField As Integer = 3
      Dim oDataTable As System.Data.DataTable = New DataTable("LanduseList")
      Dim iLanduseOrder As Integer
      'Dim dicLusePgons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnLusePgon) = TopoManager.TPlanGraph.TplnProject.GetLusePgons(iTopoPurpose)
      Dim dicLanduses As TopoManager.TPlanGraph.TplnLanduses = TopoManager.TPlanGraph.TplnLot.Landuses

      If dicLanduses Is Nothing Then
         zzLoadGraphA()
         dicLanduses = TopoManager.TPlanGraph.TplnLot.Landuses
      End If
      '	MessageBox.Show(CStr(dicLanduses.Count) & vbCrLf & CStr(dColorSchemeScale) & vbCrLf & iTopoPurpose.ToString(), "01_328h")
      Dim s As String = ""



      With oDataTable.Columns
         .Add(msNameFieldName, GetType(System.String))   'System.Type.GetType("System.Decimal"
         .Add(msColorFieldName, GetType(DMAcadExt.ColorScheme))
         .Add(msLanduseOrderFieldName, GetType(System.Int32))
      End With
      Dim oNewRow As System.Data.DataRow
      Dim tColorScheme As DMAcadExt.ColorScheme



      Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
      Dim oaParams(2) As Common.DbParameter
      '	Dim oErrOut As System.Data.Common.DbException = Nothing
      '	MessageBox.Show(CStr(miProjectCode) & vbCrLf & CStr(miDetailNo) & vbCrLf & CStr(mtMapThemeData.MapThemeID), "01_331q")
      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, mtMapThemeData.MapThemeID)
      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
      Dim oLanduse As TopoManager.TPlanGraph.TplnLanduse = Nothing
      If oDataReader IsNot Nothing Then
         Dim iLanduseID, iColorSchemeID As Integer
         Dim sLanduseName As String
         Dim iSchemeOrder As Integer

         Do While oDataReader.Read
            iLanduseID = oDataReader.GetInt32(0)
            If dicLanduses.TryGetValue(iLanduseID, oLanduse) AndAlso oLanduse.HasLots(iTopoPurpose) Then
               If oDataReader.IsDBNull(bFirstColorSchemeField) Then
                  iColorSchemeID = 0
               Else
                  iColorSchemeID = oDataReader.GetInt32(bFirstColorSchemeField)
               End If
               If Not oDataReader.IsDBNull(2) Then
                  iSchemeOrder = oDataReader.GetInt32(2)
               End If

               If oDataReader.IsDBNull(1) Then
                  sLanduseName = String.Empty
               Else
                  sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
               End If

               If iColorSchemeID <> 0 Then
                  tColorScheme = New DMAcadExt.ColorScheme(oDataReader, bFirstColorSchemeField, dScale)


                  oNewRow = oDataTable.NewRow()
                  oNewRow.Item(msNameFieldName) = tColorScheme.Name
                  oNewRow.Item(msColorFieldName) = tColorScheme
                  oNewRow.Item(msLanduseOrderFieldName) = iLanduseOrder
                  oDataTable.Rows.Add(oNewRow)
               End If
            Else
               ''''''''''''''	MessageBox.Show(CStr(iLanduseID) & ":" & CStr(iColorSchemeID), "01_334x")
            End If
         Loop
         oDataReader.Close()
      Else
         MessageBox.Show("", "01_173g")
      End If

      oDataView = New DataView(oDataTable)
      oDataView.Sort = msLanduseOrderFieldName

   End Sub
   Private Sub cmdEditColorSet_Click(oSender As System.Object, e As System.EventArgs) Handles cmdEditColorSet.Click
      mfEditColorSet = New frmEditColorSet(mtMapThemeData, 0)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditColorSet)
      Me.Visible = False
   End Sub
   Private Sub zzFillColorSchemesDic()
      Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
      Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
      '	MessageBox.Show("", "05_276")

      mdicColorSchemes = TopoManager.TPlanGraph.TplnLanduse.GetColorSchemeDic(iTopoPurpose)
      'DEBUG	MessageBox.Show(CStr(mdicColorSchemes.Count) & vbCrLf & iTopoPurpose.ToString() & vbCrLf & mtMapThemeData.MapThemeID.ToString(), "05_281")
      TopoManager.TPlanGraph.TplnLanduse.FillColorSchemesDic(mtMapThemeData.MapThemeID, mdicColorSchemes, dScale)
   End Sub
   Private Sub cmdClearPaint_Click(sender As System.Object, e As System.EventArgs) Handles cmdClearPaint.Click
      Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
      Dim sPaintLayer As String = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef().Name
      '  MessageBox.Show(sPaintLayer, "04_360")
      Me.Cursor = Cursors.WaitCursor
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      TopoManager.TPlanGraph.TplnProject.ClearPaintNew(mtMapThemeData.MapThemeID)  ', iTopoPurpose
      DMAcadExt.AcadTransaction.ClearLayerList(sPaintLayer)
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
      Me.Cursor = Cursors.Default
   End Sub


   Private Sub cmdClose_Click(sender As System.Object, e As System.EventArgs) Handles cmdClose.Click
      Me.Close()
   End Sub


   Private Sub cmdColorSchemeEditor_Click(sender As System.Object, e As System.EventArgs) Handles cmdColorSchemeEditor.Click
      Dim fColorEditor As frmColorSchemeEditor = New frmColorSchemeEditor(enColorEditorMode.Landuse, False)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, fColorEditor)
   End Sub
   Private Sub tstLanduseTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstLanduseTopology.ItemClicked
      '	MessageBox.Show(e.ClickedItem.Name & vbCrLf & mtMapThemeData.DissolveTopoName, "01_098")


      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbLanduseCreateTopo.Name
            zzCreateLanduseTopology()

         Case Me.tsbLanduseCheckTopo.Name

            zzCheckDissolveTopo(True)

         Case Me.tsbLanduseDeleteTopo.Name
            zzDeleteLanduseTopo(True)
         Case Me.tsbLanduseExec.Name





      End Select
      Me.Cursor = Cursors.Default
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub tstBlueLineTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs)
      '	MessageBox.Show(e.ClickedItem.Name & vbCrLf & mtMapThemeData.DissolveTopoName, "01_098")


      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbBlueLineCreateTopo.Name
            zzCreateBlueLineTopology()

         Case Me.tsbBlueLineCheckTopo.Name
            zzCheckBlueLineTopo(True)


         Case Me.tsbBlueLineDeleteTopo.Name
            zzDeleteBlueLineTopo(True)
         Case Me.tsbBlueLineExec.Name





      End Select
      Me.Cursor = Cursors.Default
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub zzCheckDispTopo()
      zzCheckDissolveTopo(True)
      zzDispTopo()
   End Sub
   Private Sub zzCheckDissolveTopo(ByVal bMsgBox As Boolean)
      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtMapThemeData.DissolveTopoName, True, bMsgBox, mtMapThemeData.MapThemeID)
   End Sub
   Private Sub zzCheckBlueLineTopo(ByVal bMsgBox As Boolean)
      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CheckTopo(mtBlueLineMapThemeData.LineTopoName, True, bMsgBox, mtMapThemeData.MapThemeID)
   End Sub
   Private Sub zzDispTopo()
      '	Me.txtPgonCount.Text = Convert.ToString(mtTopoRes.PgonCount)
      '	zzDispTopoExists(mtTopoRes.TopoExists)
   End Sub
   Private Sub zzDeleteLanduseTopo(ByVal bDeleteEntities As Boolean)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

      TopoManager.TopoCreator.DeleteTopology(mtMapThemeData.DissolveTopoName, bDeleteEntities, False)
      zzCheckDissolveTopo(False, False)

      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzDeleteBlueLineTopo(ByVal bDeleteEntities As Boolean)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)

      TopoManager.TopoCreator.DeleteTopology(mtBlueLineMapThemeData.LineTopoName, bDeleteEntities, False)
      zzCheckBlueLineTopo(False, False)

      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub cmdRefresh_Click(oSender As System.Object, e As System.EventArgs) Handles cmdRefresh.Click
      Me.Cursor = Cursors.WaitCursor
      zzLoadGraphA()
      Me.Cursor = Cursors.Default
   End Sub

   Private Sub zzLoadGraph()

      Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
      TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
      TopoManager.TPlanGraph.TplnProject.LoadLots(iTopoPurpose)
      TopoManager.TPlanGraph.TplnProject.LoadLusePgonsNew(mtMapThemeData)
   End Sub
   Private Sub zzLoadGraphA()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

      zzLoadGraph()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub cmdDrawLegend_Click(oSender As System.Object, e As System.EventArgs) Handles cmdDrawLegend.Click
      Const sTopoApprText As String = "מצב קיים"
      Const sTopoPropText As String = "מצב מוצע"
      Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
      Dim iaDataColumns() As Integer = Nothing
      Dim oaTotals() As System.Object = Nothing
      '	Dim oaOptionValues() As System.Object = Nothing
      Dim oaOptionValues(0) As System.Object

      Dim dLegendPaintFactor As Double
      Dim sTopoPurposeText As String
      Select Case iTopoPurpose
         Case DMAcadExt.enTopoPurpose.Approved
            sTopoPurposeText = sTopoApprText
         Case DMAcadExt.enTopoPurpose.Proposed
            sTopoPurposeText = sTopoPropText
         Case Else
            sTopoPurposeText = String.Empty
      End Select
      oaOptionValues(0) = sTopoPurposeText
      Try
         dLegendPaintFactor = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
      Catch oEx As Exception
         dLegendPaintFactor = 1
      End Try
      ' System.Windows.Forms.MessageBox.Show(CStr(dLegendPaintFactor) & vbCrLf & CStr(AcadReport.RepApp.DrawingScaleFactor), "21_455")
      Dim oDataView As DataView = Nothing

      GetLanduseData(iTopoPurpose, oDataView, (dLegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor))
      '   System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count), "21_456")
      Dim oLaunchReport As LaunchReport = New LaunchReport(True, TPlServerDB.enResourceTheme.AcRepLegendK)
      oLaunchReport.PaintFactor = dLegendPaintFactor

      Me.Hide()
      oLaunchReport.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues)
      Me.Show()
   End Sub

   Private Sub ddbLayers_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbLanduseLayers.DropDownItemClicked
      Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
      Select Case oToolStripItem.Name
         Case Me.tsiLanduseThisTopoOnlyVisible.Name

            zzLanduseOnlyVisible()


         Case Me.tsiLanduseThisTopoVisible.Name

            '	zzSourceVisible(False)

         Case tsiLanduseAllVisible.Name
            zzAllVisible()

      End Select
   End Sub
   Private Sub zzAllVisible()
      Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      DMAcadExt.AcadTransaction.ProcAll(dlEntityProc, True)

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub frmPaintLanduse_FormClosing(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

      zzSaveParams()
   End Sub


   Private Sub mfEditColorSet_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfEditColorSet.FormClosed
      Me.Visible = True
   End Sub


   Private Sub cmdReadLog_Click(sender As System.Object, e As System.EventArgs) Handles cmdReadLog.Click
      Dim sLogName As String = """" & DMAcadExt.AcadDocument.LogName & """"
      '	Dim iID As Integer = Shell("""notepad"" -a -q", , True, 100000)
      Dim iID As Integer = Shell("notepad " & sLogName, AppWinStyle.NormalFocus, False, 100000)
      DMAcadExt.AcadDocument.WriteMessage("notepad " & sLogName)
   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub

   Private Sub frmPaintLanduse_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      Dim bMsgBox As Boolean = False


      zzCheckSourceTopo(False, bMsgBox)

      zzCheckDissolveTopo(False, bMsgBox)
      zzCheckBlueLineTopo(False, bMsgBox)


      DMAcadExt.AcadDocument.Unlock()

      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
      '  MessageBox.Show(mtMapThemeData.MapThemeName, "04_360")
      zzSetVarNames()
   End Sub
   Private Sub zzSetVarNames()
      Const sSpace As String = " "
      Const sLotThemeName As String = "מגרשים"
      Const sParcelThemeName As String = "חלקות"
      Const sPrevTextTopo As String = "טופולוגיית"
      Const sPrevTextLayer As String = "שכבת"
      Const sColon As String = ":"

      Select Case mtMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
            Me.lblSourceTopologyCap.Text = sPrevTextTopo & sSpace & sLotThemeName & sColon
            Me.lblSourceClosedPgonsCap.Text = sPrevTextLayer & sSpace & sLotThemeName & sColon
         Case DMAcadExt.enMapTheme.Parcels
            Me.lblSourceTopologyCap.Text = sPrevTextTopo & sSpace & sParcelThemeName & sColon
            Me.lblSourceClosedPgonsCap.Text = sPrevTextLayer & sSpace & sParcelThemeName & sColon
      End Select
   End Sub

   Private Sub frmPaintLanduse_Shown(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Shown
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
   End Sub
   Private Sub zzCreatePgonSet()
      '   MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370")
      Dim bCurrentLayerOK As Boolean = False
      '  Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing
      ' System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString & vbCrLf & mtSourceMapThemeData.TopoPurpose.ToString & vbCrLf & mtSourceMapThemeData.GraphType.ToString() & vbCrLf & mtSourceMapThemeData.TopoName & vbCrLf & mtSourceMapThemeData.ClosedPgonsLayers, "07_001s")
      moPgonSet = New FDO.TplnPolygonSet(mtMapThemeData.MapThemeID, mtMapThemeData.TopoPurpose, mtMapThemeData.TopoName)
      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      Select Case mtMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.Blocks
            TopoManager.TPlanGraph.TplnBlock.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.Parcels
            TopoManager.TPlanGraph.TplnParcel.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.UD_Parcels
            UnidivNet.UD_Parcel.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
            TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
      End Select






      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msIntersectionPointsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

      '    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      ' System.Windows.Forms.MessageBox.Show(mtMapThemeData.ClosedPgonsLayers & vbCrLf & "", "07_002")
      Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtMapThemeData.ClosedPgonsLayers)
      '
      Dim dicCentroids As System.Collections.Generic.IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Autodesk.AutoCAD.DatabaseServices.BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(mtMapThemeData.CentroidBlock, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)   '"pclp004"
      '
      mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers)
      DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count) & "; " & mtMapThemeData.CentroidBlocks & "; " & mtMapThemeData.CentroidLayers & "; " & mtMapThemeData.ClosedPgonsLayers)
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
      moPgonSet.CalculateNewF(False)
      If moPgonSet.TopoPolygons.Count > 0 Then
         Me.txtSourceClosePgonCount.Text = moPgonSet.TopoPolygons.Count.ToString()
         Dim oFont As Font = New Font(Me.lblSourcePgonLayers.Font.Name, Me.lblSourcePgonLayers.Font.Size, FontStyle.Bold)
         Me.lblSourcePgonLayers.Font = oFont
         Me.lblPgonsetExists.Visible = True
      End If



      '  DMCommon.ExcelLog.Open()
      '  moPgonSet.InfoToExcel()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub zzPaintLotPgonset(ByVal tLayerDef As DMAcadExt.AcadLayerDef, ByVal bLayerByLanduseID As Boolean)
      '   MessageBox.Show(mtMapThemeData.GraphType.ToString() & vbCrLf & CStr(moPgonSet IsNot Nothing), "21_323")

      If mtMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
         Dim dicTopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)

         '  TopoManager.TPlanGraph.TplnLanduse.FillColorSchemesDic(DMAcadExt.enTopoPurpose.Approved, TopoManager.TPlanGraph.TplnProject.ApprMapThemeData.MapThemeID)


         If moPgonSet IsNot Nothing Then

            Me.Cursor = Cursors.WaitCursor
            'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
            'DMAcadExt.AcadTransaction.Start()
            'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

            Dim iPolygonIndex As Integer
            Dim sPaintLayer As String
            Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
            Dim sLanduseNotFoundList As String = String.Empty
            Dim bCurrentLayerOK As Boolean
            sPaintLayer = TopoManager.TPlanGraph.TplnProject.SetPaintLayers(mtMapThemeData.MapThemeID, DMAcadExt.enTopoPurpose.Approved)
            '  zzFillColorSchemesDic()
            dicTopoPolygons = moPgonSet.TopoPolygons
            Me.prbPaint.Maximum = dicTopoPolygons.Count
            Me.prbPaint.Step = 1

            '   System.Windows.Forms.MessageBox.Show(dicTopoPolygons.Count.ToString & vbCrLf & mdicColorSchemes.Count.ToString(), "21_012")
            For Each oLot As TopoManager.TPlanGraph.TplnLot In dicTopoPolygons.Values
               iPolygonIndex += 1
               DMAcadExt.AcadDocument.Counter = iPolygonIndex
               '  System.Windows.Forms.MessageBox.Show(oLot.TopoID.ToString & vbCrLf & oLot.LanduseID.ToString(), "21_013")
               If mdicColorSchemes.TryGetValue(oLot.LanduseID, tColorScheme) AndAlso tColorScheme.ID <> 0 Then
                  If mtTopoSelection.Contains(iPolygonIndex, oLot.ExteriorHandle.Value) Then
                     If bLayerByLanduseID Then
                        '  DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True, oLot.LanduseID)

                        bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, False, False, False, oLot.LanduseID)


                        sPaintLayer = tLayerDef.NamePlusExtension(oLot.LanduseID)
                     End If

                     '   System.Windows.Forms.MessageBox.Show(tColorScheme.ID.ToString & vbCrLf & oLot.LanduseID.ToString(), "21_014")
                     If oLot.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sPaintLayer) <> DMAcadExt.PaintException.OK Then
                        Exit For
                     End If
                  End If
               Else
                  If sLanduseNotFoundList.Length <> 0 Then
                     sLanduseNotFoundList &= ","
                  End If
                  sLanduseNotFoundList &= CStr(oLot.LanduseID)
               End If



               prbPaint.Increment(1)
            Next
            'DMAcadExt.AcadTransaction.CloseModelSpace()
            'DMAcadExt.AcadTransaction.Terminate()
            'DMAcadExt.AcadDocument.Unlock()
            DMAcadExt.AcadDocument.UpdateScreen()
            Me.Cursor = Cursors.Default

         End If
      End If

   End Sub

         


  

   Private Sub cmdPaintPgonset_Click(oSender As System.Object, e As EventArgs) Handles cmdCreatePgonset.Click
      zzCreatePgonSet()
   End Sub

   Private Sub lblSourceTopoName_Click(oSender As System.Object, e As EventArgs) Handles lblSourceTopoName.Click

   End Sub

   Private Sub chkBypassLine_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkBypassLine.CheckedChanged

   End Sub

   Private Sub lblDissolveTopoName_Click(oSender As System.Object, e As EventArgs) Handles lblDissolveTopoName.Click

   End Sub
 

   Private Sub rdbSource_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbSource.CheckedChanged
      zzSetPgonCount()
   End Sub
 

   Private Sub rdbDissolve_CheckedChanged(oSender As System.Object, e As EventArgs) Handles rdbDissolve.CheckedChanged
      zzSetPgonCount()
   End Sub

   Private Sub txtSourcePgonCount_TextChanged(oSender As System.Object, e As EventArgs) Handles txtSourcePgonCount.TextChanged

   End Sub

   Private Sub cmdAddID_Click(oSender As System.Object, e As EventArgs) Handles cmdAddID.Click
      Dim sTopoName As String
      If Me.rdbSource.Checked Then
         sTopoName = mtMapThemeData.LineTopoName
      ElseIf Me.rdbDissolve.Checked Then
         sTopoName = mtMapThemeData.DissolveTopoName
      Else
         sTopoName = String.Empty
      End If
      Dim oTopomodel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim iTopoID As Integer
      Dim iRowIndex As Integer
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
      Dim bResp As Boolean
      Dim bRepeat As Boolean = True



      Do
         bResp = DMAcadExt.AcadDocument.GetPoint("Select Point ", tPoint)
         DMAcadExt.AcadDocument.WriteMessage("Point=" & tPoint.ToString())
         Try
            iTopoID = oTopomodel.FindPolygon(tPoint).ID
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("TopoExp=" & oEx.Message)
         End Try
         DMAcadExt.AcadDocument.WriteMessage("TopoID=" & CStr(iTopoID))
         If iTopoID <> 0 Then
            Try
               If Me.txtException.Text.Length > 0 Then
                  Me.txtException.Text &= ","
               End If
               Me.txtException.Text &= iTopoID.ToString()
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTplnView - zzFindPgon")
            End Try
         End If


      Loop While bRepeat AndAlso bResp

      DMAcadExt.AcadDocument.CloseMessage()
      ''   Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", False, False, False)






   End Sub

End Class