Option Explicit On
Option Strict On
Imports TopoManager
Imports FDO
Imports System.Data
Public Class frmMapThemeBase
   Implements ICheckTheme

   Const msSwitchCleanupText As String = "  Cleanup"
   Const msCreateTopoText As String = "יצירת טופולוגיה"
   Const msCheckTopoText As String = "בדיקת טופולוגיה"

   Const msDeleteTopoText As String = "מחיקת טופולוגיה"
   '	Const msShowGeometryText As String = "הצגת גיאומטריה"
   Const msEraseTopoGeoText As String = "מחיקת טופולוגיה וגיאומטריה"
   Const msTopoPropertiesText As String = "מאפייני טופולוגיה"
   Const msTopoLayersText As String = "שכבות"
   Const msStraightenText As String = "יצירת קווים במקום קשתות"
   Const msGetStatisticsText As String = "סטטיסטיקה"
   Protected Const msExecText As String = "לבצע"
   Protected Const msEraseText As String = "מחיקה"

   Protected Const msToClosedPgonsText As String = "פוליגונים סגורים"
   Protected Const msExteriorRingsToClosedPgonsText As String = "פוליגונים סגורים בלי איים"
   Protected Const msEraseClosedPgonsText As String = "מחיקת פוליגונים סגורים"

   Protected piProjectCode As Integer
   Protected piDetailNo As Integer


   Const miTopoErrIndexNotErr As Integer = -1
   Protected piStagesUB As Integer = -1
   Protected psaCaptions() As String
   Protected psaCurrentCaptions() As String
   Protected piLabelTop As Integer = 72
   '	Private WithEvents cmdPrepare As TabButton
   Private moaTopoErrors As TopoErrorArray
   Private moaTopoErrorsWA As TopoErrorArray

   Private miCurrentTopoErrIndex As Integer = miTopoErrIndexNotErr
   Protected dtPanelSize As System.Drawing.Size = New System.Drawing.Size(300, 320)
   Protected miVertLineX As Integer = dtPanelSize.Width + 1

   '	Protected WithEvents dgvMessages As DataGridView
   '	Protected tbpProjectData As System.Windows.Forms.TabPage
   '	Protected tbpMessages As System.Windows.Forms.TabPage

   Protected miToposUB As Integer = TopoDefs.giToposUB


   Protected doaPanels() As System.Windows.Forms.Panel '= {New System.Windows.Forms.Panel(), New System.Windows.Forms.Panel(), New System.Windows.Forms.Panel(), New System.Windows.Forms.Panel(), New System.Windows.Forms.Panel()}
   'Private WithEvents cmdEraseTopoGeometria As TabButton

   'Private WithEvents tsbSwitchCleanup As System.Windows.Forms.ToolStripButton
   '	Private WithEvents tsbEraseTopoGeometria As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbCopyFromOverlay As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbToClosedPolygons As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbExportToShape As System.Windows.Forms.ToolStripButton


   Private WithEvents cmsMain As System.Windows.Forms.ContextMenuStrip
   Private WithEvents tsiCreateTopo As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiDeleteTopo As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiShowGeometry As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiEraseTopoGeo As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiTopoProperties As System.Windows.Forms.ToolStripMenuItem
   Private WithEvents tsiCloseForm As System.Windows.Forms.ToolStripMenuItem

   '	Private WithEvents tsiMapClearAll As System.Windows.Forms.ToolStripMenuItem
   '	Private WithEvents tsiMapClearLayer As System.Windows.Forms.ToolStripMenuItem
   '	Private WithEvents tsiParcelPgonsExp As System.Windows.Forms.ToolStripMenuItem



   Private tsiDel1 As System.Windows.Forms.ToolStripMenuItem



   '	Private WithEvents tsbMapPlatf As System.Windows.Forms.ToolStripDropDownButton


   '	Protected WithEvents chkCheckTopo As CheckBox
   '''''''''''''	Private WithEvents tmrInactive As System.Windows.Forms.Timer
   Private miActionDflt As Integer = 0
   Protected Const msActionIDFldName As String = "ActionID"
   Protected Const msToleranceFldName As String = "Tolerance"
   Protected Const msErrorsFldName As String = "Errors"
   Protected Const msPointsFldName As String = "Points"
   Protected Const msCalculateSettingKey As String = "Calculate"
   Protected Const msLotNameNumSettingKey As String = "LotNameNum"
   Protected Const msCheckTopoSettingKey As String = "CheckTopo"
   Protected Const msBlueLineSettingKey As String = "BlueLine"
   Protected Const msPaintStraightenSettingKey As String = "PaintStraighten"




   'Protected pbDone As Boolean


   Private Shared miBaseResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmMapThemeBase
   Protected Shared diResourceTheme As TPlServerDB.enResourceTheme


   Private moPriorView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = Nothing



   Private lblCaption As Label

   Private txtTopologyName As TextBox
   Private lblTopologyName As Label

   Private txtLinkLayersAAA As TextBox
   Private lblLinkLayersAAA As Label

   Private WithEvents chkSourceTopologia As CheckBox
   Private chkStraightenArcs As CheckBox
   Private chkLineTopologia As CheckBox

   'Private lblSourceTopologiaCap As Label
   '	Private lblSourceTopologiaV As Label



   Protected doaLabelCheck() As LabelCheck

   Private moaErrorPoints() As DMAcadExt.TplnPointArray = Nothing
   Protected miCurrentTopoDefID As DMAcadExt.TopoDefID
   Private miCurrentAction As Integer = -1
   Private miStepNum As Integer
   '	Private moDataTable(miToposUB) As System.Data.DataTable
   Private moCleanupActionsTable As System.Data.DataTable





   '	Protected moaChecks(miToposUB) As TopoCheck
   '	Protected doProjectData As ProjectData	'''''''''''''''''''''''''''''

   Protected WithEvents txtLegendPaintFactor As TextBox



   Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
   Private moTopoErrPoints As DMAcadExt.TplnPointArray
   Private moCurrentTopoErrIndex As Integer = -1

   Private WithEvents chkTopoLayersOn As CheckBox
   Private mbCodeExecuting As Boolean = False
   Private mbInactive As Boolean
   Private mdInactiveTiks As Double = 0.0

   Private moLabelColor As System.Drawing.Color = Color.DimGray
   Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue
   '														  New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected doBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doLabelBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Protected Shared doCaptionBoldFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte)) '9.75
   Private Shared moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))


   '	Private moFDO_Manager As FDO_Manager

   Private txtProjectData() As TextBox
   Private cmbProjectData() As ComboBox
   Private lblProjectData() As Label

   Private iTestIndex As Integer
   Private WithEvents cmdSaveProjectData As TabButton
   Private WithEvents cmdSaveParameters As TabButton
   Protected WithEvents tmrDelay As System.Windows.Forms.Timer
#Region "Main_Declarations"
   Protected WithEvents cmdFirst As System.Windows.Forms.Button
   Protected WithEvents cmdPrev As System.Windows.Forms.Button
   Protected WithEvents cmdNext As System.Windows.Forms.Button
   Protected WithEvents cmdLast As System.Windows.Forms.Button
   Protected WithEvents cmdExit As System.Windows.Forms.Button
   Protected WithEvents prbPaint As System.Windows.Forms.ProgressBar
#End Region
#Region "Panel0_3_Declarations"
   Private dgvActions(1) As DataGridView
   Private nudSteps(1) As System.Windows.Forms.NumericUpDown
   '	Private nudErrors(1) As System.Windows.Forms.NumericUpDown
   Private cmdFix(1) As TabButton
   Private cmdMark(1) As TabButton
   Private lblErrors(1) As System.Windows.Forms.Label
   Private moCurrentView(1) As DataView
#End Region
#Region "Panel1_Declarations"

   Private chkCreateCentroid As System.Windows.Forms.CheckBox
   Private chkHighlightSliver As System.Windows.Forms.CheckBox
   Private lblTolerance As System.Windows.Forms.Label
   Private txtTolerance As System.Windows.Forms.TextBox

   Private grbLinks As System.Windows.Forms.GroupBox

   Private grbCentroids As System.Windows.Forms.GroupBox
   Private lblCentroidBlocks As System.Windows.Forms.Label
   Private lblCentroidLayers As System.Windows.Forms.Label
   Protected WithEvents tstTopology As System.Windows.Forms.ToolStrip
   Protected WithEvents tstClosedPgons As System.Windows.Forms.ToolStrip


   Private txtErrorCount As System.Windows.Forms.TextBox
   Private txtErrorIndex As System.Windows.Forms.TextBox
   Private lblTopoName As System.Windows.Forms.Label
   Private lblTopoNameCap As System.Windows.Forms.Label

   '	Private WithEvents cmdStartErr As System.Windows.Forms.Button
   '	Private WithEvents cmdPrevErr As System.Windows.Forms.Button
   '	Private WithEvents cmdNextErr As System.Windows.Forms.Button
   '	Private WithEvents cmdEraseErr As System.Windows.Forms.Button

   Private lblTopoExists As System.Windows.Forms.Label
   Private txtPgonCount As System.Windows.Forms.TextBox
   Private txtCentroidCount As System.Windows.Forms.TextBox
   Private txtCentroidBlocks As System.Windows.Forms.TextBox
   Private txtCentroidLayers As System.Windows.Forms.TextBox
   Private lblLinkLayers As System.Windows.Forms.Label
   Private txtLinkLayers As System.Windows.Forms.TextBox
   Private txtLinkCount As System.Windows.Forms.TextBox

#End Region
#Region "ToolStrip_Topo"
   Protected tsbCreateTopo As System.Windows.Forms.ToolStripButton
   Protected tsbCheckTopo As System.Windows.Forms.ToolStripButton
   Protected tsbDeleteTopo As System.Windows.Forms.ToolStripButton
   Protected tsbShowTopo As System.Windows.Forms.ToolStripButton
   Protected WithEvents ddbLayers As System.Windows.Forms.ToolStripDropDownButton
   Protected tsbGetStatistics As System.Windows.Forms.ToolStripButton

   Protected tsbExec As System.Windows.Forms.ToolStripButton
   Protected tsbExecA As System.Windows.Forms.ToolStripButton

   Protected tsbClear As System.Windows.Forms.ToolStripButton

   Protected tsiThisTopoOnlyVisible As System.Windows.Forms.ToolStripMenuItem
   Protected tsiThisTopoVisible As System.Windows.Forms.ToolStripMenuItem
   Protected tsiAllVisible As System.Windows.Forms.ToolStripMenuItem

   '	Global.TopoUI.My.Resources.Resources.Layers16Tr
#End Region
#Region "ToolStrip_ClosedPgons"
   Protected tsbImportShapes As System.Windows.Forms.ToolStripButton
   Protected tsbDissolveShapes As System.Windows.Forms.ToolStripButton
   Protected tsbDissolveAllArea As System.Windows.Forms.ToolStripButton
   Protected tsbClearMap As System.Windows.Forms.ToolStripButton
   Protected tsbA As System.Windows.Forms.ToolStripButton
   Protected WithEvents ssbCreatePgonset As System.Windows.Forms.ToolStripSplitButton

   Protected WithEvents tsiFormErr As System.Windows.Forms.ToolStripMenuItem
   Protected WithEvents tsiPgonSetReset As System.Windows.Forms.ToolStripMenuItem

   Protected tsbB As System.Windows.Forms.ToolStripButton

   Protected tsbExportShapes As System.Windows.Forms.ToolStripButton
   Protected tsbPgonView As System.Windows.Forms.ToolStripButton
   '  Protected tsbTempA As System.Windows.Forms.ToolStripButton



   '	Global.TopoUI.My.Resources.Resources.Layers16Tr
#End Region

#Region "Panel2_Declarations"
   Private WithEvents grbSource As System.Windows.Forms.GroupBox
   Private WithEvents grbWithoutArcs As System.Windows.Forms.GroupBox

   Private WithEvents lblLinkLayersP2 As System.Windows.Forms.Label
   Private WithEvents txtLinkLayersP2 As System.Windows.Forms.TextBox
   Private WithEvents txtLinkCountP2 As System.Windows.Forms.TextBox

   Private WithEvents lblLinkWithArcCount As System.Windows.Forms.Label
   Private WithEvents txtLinkWithArcCount As System.Windows.Forms.TextBox
   Private WithEvents lblArcCount As System.Windows.Forms.Label
   Private WithEvents txtArcCount As System.Windows.Forms.TextBox

   Private WithEvents lblLineLinkLayer As System.Windows.Forms.Label
   Private WithEvents txtLineLinkLayer As System.Windows.Forms.TextBox
   Private WithEvents txtLineLinkCount As System.Windows.Forms.TextBox
   Private WithEvents txtStraightenTolerance As System.Windows.Forms.TextBox



#End Region
#Region "Panel4_Declarations"

   Private chkCreateCentroidWA As System.Windows.Forms.CheckBox
   Private chkHighlightSliverWA As System.Windows.Forms.CheckBox
   Private lblToleranceWA As System.Windows.Forms.Label
   Private txtToleranceWA As System.Windows.Forms.TextBox

   Private grbLinksWA As System.Windows.Forms.GroupBox

   Private grbCentroidsWA As System.Windows.Forms.GroupBox
   Private lblCentroidBlocksWA As System.Windows.Forms.Label
   Private lblCentroidLayersWA As System.Windows.Forms.Label

   Private lblTopoErrorsWA As System.Windows.Forms.Label
   Private txtErrorCountWA As System.Windows.Forms.TextBox
   Private txtErrorIndexWA As System.Windows.Forms.TextBox
   Private lblTopoNameWA As System.Windows.Forms.Label
   Private lblTopoNameCapWA As System.Windows.Forms.Label
   Private WithEvents cmdNextErrWA As System.Windows.Forms.Button
   Private WithEvents cmdPrevErrWA As System.Windows.Forms.Button
   Private WithEvents cmdStartErrWA As System.Windows.Forms.Button
   Private WithEvents cmdEraseErrWA As System.Windows.Forms.Button


   Private lblTopoExistsWA As System.Windows.Forms.Label
   Private txtPgonCountWA As System.Windows.Forms.TextBox
   Private txtCentroidCountWA As System.Windows.Forms.TextBox
   Private txtCentroidBlocksWA As System.Windows.Forms.TextBox
   Private txtCentroidLayersWA As System.Windows.Forms.TextBox

   Private lblLinkLayersWA As System.Windows.Forms.Label
   Private txtLinkLayersWA As System.Windows.Forms.TextBox
   Private txtLinkCountWA As System.Windows.Forms.TextBox

   Private lblLineLinkLayers As System.Windows.Forms.Label
   Private txtLineLinkLayers As System.Windows.Forms.TextBox
   Private txtLineLinkCountB As System.Windows.Forms.TextBox

#End Region
   Public Event FormatChanged()
   Public Event PaintScaleChanged()
   Public Event Calculate()
   Public Event AppExit()
   Public Event OnClose()
   Protected ptMapThemeData As DMAcadExt.MapThemeData
   Protected ptDissolveMapThemeData As DMAcadExt.MapThemeData
   Protected ptSourceMapThemeData As DMAcadExt.MapThemeData
   Protected ptOverlayMapThemeData As DMAcadExt.MapThemeData
   Protected ptOverlayMapThemeData_A As DMAcadExt.MapThemeData

   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
      '   MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.TopoName, "?nothing") & ":" & DMCommon.Functions.CStrN(tMapThemeData.LineTopoName, "NOT"), "07_422")
      ptMapThemeData = tMapThemeData
      ' This call is required by the Windows Form Designer.
      InitializeComponent()
      '
      ' Add any initialization after the InitializeComponent() call.
      piProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
      piDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
   End Sub
   Public MustOverride ReadOnly Property IsDone As Boolean

   Protected Sub OnNew()
      zzMyInitializeComponent()

      doaLabelCheck(0).SetCurrent()

      ''''''''''	zzAfterChangeCurrent(0)
      DMAcadExt.AcadDocument.SetLogName()
      Me.DialogResult = System.Windows.Forms.DialogResult.No


   End Sub
   Protected Sub OnCalculate()
      RaiseEvent Calculate()
   End Sub
   Protected Sub OnFormatChanged()
      RaiseEvent FormatChanged()
   End Sub
   Protected Sub OnPaintScaleChanged()
      RaiseEvent PaintScaleChanged()
   End Sub
   Protected Sub SetLabelDim(ByVal iStagesUB As Integer)

      piStagesUB = iStagesUB
      ReDim doaPanels(piStagesUB)
      ReDim doaLabelCheck(piStagesUB)
      '	MessageBox.Show(CStr(iStagesUB) & ":" & CStr(doaPanels.GetUpperBound(0)), "07_020")
   End Sub
   Protected Function ColorByExists(bExists As Boolean) As System.Drawing.Color
      If bExists Then
         Return System.Drawing.SystemColors.ControlText
      Else
         Return System.Drawing.SystemColors.GrayText()
      End If
   End Function

   Protected Function FontByExists(bExists As Boolean) As System.Drawing.Font
      If bExists Then
         Return doLabelBoldFont
      Else
         Return doLabelFont
      End If
   End Function
   Private Sub zzMyInitializeComponent()
      Me.moComponents = New System.ComponentModel.Container
      '	Me.ClientSize = New System.Drawing.Size(540, 320)	';(540, 300)
      '	Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
      Me.Location = New System.Drawing.Point(200, 200)



      Me.tmrDelay = New System.Windows.Forms.Timer(Me.moComponents)


      '	Me.cmdEraseTopoGeometria = New TabButton(moLabelFont, True)
      '	Me.cmdCopyFromOverlay = New TabButton(moLabelFont, True)



      Me.cmsMain = New System.Windows.Forms.ContextMenuStrip(Me.moComponents)



      '	Me.Controls.Add(Me.cmdPrepare)
      '	Me.Controls.Add(Me.tstTopo)


      '	Me.Controls.Add(Me.chkTopoLayersOn)
      ''''''''''''''		Me.Controls.Add(Me.chkSourceTopologia)
      '''''''''''''''''Me.Controls.Add(Me.lblSourceTopologiaCap)
      '''''''''''''Me.Controls.Add(Me.lblSourceTopologiaV)


      '
      'tmrDelay
      '
      Me.tmrDelay.Interval = 1000
      '

      '	DMCommon.Functions.DispArray(moaPanels, "moaPanels", True)
      zzInitPanels()
      '	zzInitToolStrip()
      zzInitMoveButtons()
      zzInitLabelChecks()



      ''''''''''''''''''''''	Me.Controls.Add(Me.txtTopoErrors)
      Me.Controls.Add(Me.lblCaption)




      ''''''''Me.tsbMapPlatf.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiMapClearAll, Me.tsiMapClearLayer, Me.tsiParcelPgonsExp})


      '





      'הצגת גיאומטריה


      '
      'tmrInactive
      '
      '


      '		miCurrentTopoDefID = TopoDefs.miaTopoIDs(miActionDflt)


      ''''''	zzLoadData(0)
      ''''''''''	zzLoadData(1)





      '''''''''	zzSetGridColumns(0)
      ''''''''''''	zzSetGridColumns(1)


   End Sub


   Private Sub zzInitMoveButtons()
      Const iButtonsY As Integer = 248
      Me.cmdFirst = New System.Windows.Forms.Button()
      Me.cmdPrev = New System.Windows.Forms.Button()
      Me.cmdNext = New System.Windows.Forms.Button()
      Me.cmdLast = New System.Windows.Forms.Button()
      Me.cmdExit = New System.Windows.Forms.Button()
      Me.prbPaint = New System.Windows.Forms.ProgressBar()

      '
      'cmdFirst
      '
      With Me.cmdFirst
         .BackgroundImage = Global.TopoUI.My.Resources.Resources.Start25Tr                                                          '.TopoUI.My.Resources.Resources.MoveFirstTr
         .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None  '''''''''''''''		.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter			  '					  'System.Windows.Forms.ImageLayout.None
         .Location = New System.Drawing.Point(320, iButtonsY)  '400
         .Name = "cmdFirst"
         .Size = New System.Drawing.Size(28, 34)
         .TabIndex = 0
         .UseVisualStyleBackColor = True
      End With
      '
      'cmdPrev
      '
      With Me.cmdPrev
         .BackgroundImage = Global.TopoUI.My.Resources.Resources.Left25Tr   'Global.TopoUI.My.Resources.Resources.MovePrevTr
         .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
         .Location = New System.Drawing.Point(364, iButtonsY)
         .Name = "cmdPrev"
         .Size = New System.Drawing.Size(32, 34)
         .TabIndex = 1
         .UseVisualStyleBackColor = True
      End With
      '
      'cmdNext
      '
      With Me.cmdNext
         .BackgroundImage = Global.TopoUI.My.Resources.Resources.Right25Tr
         .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
         .Location = New System.Drawing.Point(400, iButtonsY)
         .Name = "cmdNext"
         .Size = New System.Drawing.Size(32, 34)
         .TabIndex = 2
         .UseVisualStyleBackColor = True
      End With

      '
      'cmdLast
      '
      With Me.cmdLast
         .BackgroundImage = Global.TopoUI.My.Resources.Resources.Finish25Tr
         .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
         .Location = New System.Drawing.Point(452, iButtonsY)
         .Name = "cmdLast"
         .Size = New System.Drawing.Size(28, 34)
         .TabIndex = 3
         .UseVisualStyleBackColor = True
      End With
      '
      'cmdExit
      '
      With Me.cmdExit
         .BackgroundImage = Global.TopoUI.My.Resources.Resources.Exit24Tr
         .BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
         .Location = New System.Drawing.Point(500, iButtonsY)
         .Name = "cmdExit"
         .Size = New System.Drawing.Size(28, 34)
         .TabIndex = 3
         .UseVisualStyleBackColor = True
      End With

      '
      'prbPaint
      '
      With Me.prbPaint
         ' .Dock = System.Windows.Forms.DockStyle.Bottom

         'Me.ClientSize = New System.Drawing.Size(574, 314)
         .Name = "prbPaint"
         .Size = New System.Drawing.Size(Me.ClientSize.Width - dtPanelSize.Width, 23)
         .Location = New System.Drawing.Point(miVertLineX + 2, Me.ClientSize.Height - Me.prbPaint.Size.Height)
         .Step = 1
         .TabIndex = 24
         .Maximum = 100
         '  .Increment(75)
         .Visible = False
         ' .RightToLeftLayout = True
      End With




      Me.Controls.Add(Me.cmdLast)
      Me.Controls.Add(Me.cmdNext)
      Me.Controls.Add(Me.cmdPrev)
      Me.Controls.Add(Me.cmdFirst)
      Me.Controls.Add(Me.cmdExit)
      Me.Controls.Add(Me.prbPaint)


   End Sub
   Private Function zzGetCleanupIndex() As Integer
      Select Case LabelCheck.CurrentIndex
         Case 0
            Return 0
         Case 3
            Return 1
      End Select
   End Function




   Protected Sub zzInitToolStrip()
      Me.tsbCreateTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbCheckTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbDeleteTopo = New System.Windows.Forms.ToolStripButton
      Me.tsbShowTopo = New System.Windows.Forms.ToolStripButton
      Me.ddbLayers = New System.Windows.Forms.ToolStripDropDownButton

      Me.tsbGetStatistics = New System.Windows.Forms.ToolStripButton
      Me.tsbExec = New System.Windows.Forms.ToolStripButton
      Me.tsbExecA = New System.Windows.Forms.ToolStripButton

      Me.tsbClear = New System.Windows.Forms.ToolStripButton


      Me.tsiThisTopoOnlyVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiThisTopoVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiAllVisible = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsbToClosedPolygons = New System.Windows.Forms.ToolStripButton
      '
      'tsbCreateTopo
      '
      With Me.tsbCreateTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.CreateTopo
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbCreateTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msCreateTopoText
      End With
      '
      'tsbCheckTopo
      '
      With Me.tsbCheckTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Validate1
         .Name = "tsbCheckTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msCheckTopoText
      End With
      '
      'tsbDeleteTopo
      '
      With tsbDeleteTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Delete
         .Name = "tsbDeleteTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msDeleteTopoText
      End With
      '
      'tsbShowTopo
      '
      With tsbShowTopo
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.ShowTopo
         .Name = "tsbShowTopo"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = Global.TopoUI.My.Resources.Resources.StrShowTopoGeometry
      End With

      '
      'tsiThisTopoOnlyVisible
      '
      With Me.tsiThisTopoOnlyVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiThisTopoOnlyVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Only"
      End With
      '
      'tsiAllVisible
      '
      With Me.tsiAllVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiAllVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "All Visible"
      End With
      '
      'tsiThisTopoVisible
      '
      With Me.tsiThisTopoVisible
         .CheckOnClick = False
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
         .Name = "tsiThisTopoVisible"
         .Size = New System.Drawing.Size(134, 22)
         .Text = "Visible"
      End With

      '
      'ddbLayers
      '
      With ddbLayers
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Layers16Tr
         .Name = "ddbLayers"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msTopoLayersText
         .DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiThisTopoOnlyVisible, Me.tsiThisTopoVisible, Me.tsiAllVisible})
      End With
      '
      'tsbGetState
      '
      With Me.tsbGetStatistics
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.QuestionMark
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbGetStatistics"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msGetStatisticsText
      End With
      '
      'tsbToClosedPolygons
      '
      With tsbToClosedPolygons
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Run15Tr ' Global.TopoUI.My.Resources.Resources.ToClosedPgons
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Name = "tsbToClosedPolygons"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = "Create closed polylines"
      End With

      '
      'tsbExec
      '
      With Me.tsbExec
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Run15Tr
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbExec"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msExecText
      End With
      '
      'tsbExecA
      '
      With Me.tsbExecA
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Run15Tr
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbExecA"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msExecText
      End With
      '
      'tsbClear
      '
      With Me.tsbClear
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbClear"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = msExecText
      End With


      '
      'tstTopology
      '
      Me.tstTopology = New System.Windows.Forms.ToolStrip()
      With Me.tstTopology
         .Dock = System.Windows.Forms.DockStyle.None

         .Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbCreateTopo, Me.tsbCheckTopo, Me.tsbDeleteTopo, Me.tsbShowTopo, Me.ddbLayers, Me.tsbGetStatistics, Me.tsbExec, Me.tsbExecA, Me.tsbClear})
         ', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
         '.Location = New System.Drawing.Point(300, 2)
         .Name = "tstTopology"
         '	.Size = New System.Drawing.Size(87, 25)
         .TabIndex = 0
         .CanOverflow = True
      End With

      Dim s As String = ""
      Dim ss As String = ""
      If tstClosedPgons IsNot Nothing Then
         For i As Integer = 0 To tstClosedPgons.Items.Count - 1
            s &= tstClosedPgons.Items(i).Name & ","
         Next
      Else
         s = "NotClosedPgons"
      End If
      If tstTopology IsNot Nothing Then
         For i As Integer = 0 To tstTopology.Items.Count - 1
            ss &= tstTopology.Items(i).Name & ","
         Next
      Else
         ss = "NotTopology"
      End If

      '	MessageBox.Show(s & vbCrLf & ss, "02_455z")
   End Sub

   Protected Sub zzInitToolStripA()
      Me.tsbImportShapes = New System.Windows.Forms.ToolStripButton
      Me.tsbDissolveShapes = New System.Windows.Forms.ToolStripButton
      Me.tsbDissolveAllArea = New System.Windows.Forms.ToolStripButton


      Me.tsbClearMap = New System.Windows.Forms.ToolStripButton
      Me.tsbA = New System.Windows.Forms.ToolStripButton
      Me.tsbB = New System.Windows.Forms.ToolStripButton
      Me.ssbCreatePgonset = New System.Windows.Forms.ToolStripSplitButton()
      Me.tsiFormErr = New System.Windows.Forms.ToolStripMenuItem()
      Me.tsiPgonSetReset = New System.Windows.Forms.ToolStripMenuItem()


      Me.tsbExportShapes = New System.Windows.Forms.ToolStripButton
      Me.tsbPgonView = New System.Windows.Forms.ToolStripButton

      '  Me.tsbTempA = New System.Windows.Forms.ToolStripButton



      '
      'tsbImportShapes
      '
      With Me.tsbImportShapes
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.Import16Tr
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbImportShapes"
         .Size = New System.Drawing.Size(23, 22)
      End With
      '
      'tsbDissolveShapes
      '
      With Me.tsbDissolveShapes
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.GushTr
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbDissolveShapes"
         .Size = New System.Drawing.Size(23, 22)
      End With

      '
      'tsbDissolveAllArea
      '
      With Me.tsbDissolveAllArea
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.World14
         .ImageTransparentColor = System.Drawing.Color.White
         .Name = "tsbDissolveAllArea"
         .Size = New System.Drawing.Size(23, 22)
      End With



      '
      'tsbClearMap
      '
      With Me.tsbClearMap
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.Erase1
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbClearMap"
         .Size = New System.Drawing.Size(23, 22)
      End With

      '
      'ssbCreatePgonset
      '
      Me.ssbCreatePgonset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      ''''''''''''''''''''''''''''''''''''   Me.ssbCreatePgonset.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsiFormErr, Me.tsiPgonSetReset})
      Me.ssbCreatePgonset.Image = Global.TopoUI.My.Resources.Resources.Check16Tr
      Me.ssbCreatePgonset.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ssbCreatePgonset.Name = "ssbCreatePgonset"
      Me.ssbCreatePgonset.Size = New System.Drawing.Size(29, 22)
      Me.ssbCreatePgonset.Text = ""
      '
      'tsiFormErr
      '
      Me.tsiFormErr.Name = "tsiFormErr"
      Me.tsiFormErr.Size = New System.Drawing.Size(152, 22)
      Me.tsiFormErr.Text = "Check Form"

      '
      'tsbA
      '
      With Me.tsbA
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.Check16Tr
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbA"
         .Size = New System.Drawing.Size(23, 22)
      End With
      '
      'tsbB
      '
      With Me.tsbB
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbB"
         .Size = New System.Drawing.Size(23, 22)
      End With

      '
      'tsbExportShapes
      '
      With Me.tsbExportShapes
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.ColorPgonsTr
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbExportShapes"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = " "
      End With

      '
      'tsbPgonView
      '
      With Me.tsbPgonView
         .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
         .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
         .Image = Global.TopoUI.My.Resources.Resources.Table19x17
         .ImageTransparentColor = System.Drawing.Color.Magenta
         .Name = "tsbPgonView"
         .Size = New System.Drawing.Size(23, 22)
         .ToolTipText = " "
      End With
      '
      'tsbTempA
      '
      'With Me.tsbTempA
      '   .DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      '   .ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None
      '   .Image = Global.TopoUI.My.Resources.Resources.Undo
      '   .ImageTransparentColor = System.Drawing.Color.Magenta
      '   .Name = "tsbTempA"
      '   .Size = New System.Drawing.Size(23, 22)
      '   .ToolTipText = " "
      'End With

      '
      'tstClosedPgons
      '
      Me.tstClosedPgons = New System.Windows.Forms.ToolStrip()
      With Me.tstClosedPgons
         .Dock = System.Windows.Forms.DockStyle.None
         .AutoSize = True
         '   .Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbImportShapes, Me.tsbDissolveShapes, Me.tsbDissolveAllArea, Me.tsbClearMap, Me.tsbA, Me.tsbB, Me.tsbExportShapes, Me.tsbPgonView})
         .Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbImportShapes, Me.tsbDissolveShapes, Me.tsbDissolveAllArea, Me.tsbClearMap, Me.ssbCreatePgonset, Me.tsbB, Me.tsbExportShapes, Me.tsbPgonView})



         ', Me.tsbEraseTopoGeometria, Me.tsbCopyFromOverlay, Me.tsbToClosedPolygons, Me.tsbExportToShape, Me.tsbMapPlatf
         '.Location = New System.Drawing.Point(300, 2)
         .Name = "tstClosedPgons"
         '	.Size = New System.Drawing.Size(87, 25)
         .TabIndex = 0
         .CanOverflow = True
         .Location = New Point(tstTopology.Size.Width, 0)
      End With
      Dim s As String = ""
      Dim ss As String = ""
      If tstClosedPgons IsNot Nothing Then
         For i As Integer = 0 To tstClosedPgons.Items.Count - 1
            s &= tstClosedPgons.Items(i).Name & ","
         Next
      Else
         s = "NotClosedPgons"
      End If
      If tstTopology IsNot Nothing Then
         For i As Integer = 0 To tstTopology.Items.Count - 1
            ss &= tstTopology.Items(i).Name & ","
         Next
      Else
         ss = "NotTopology"
      End If

      '	MessageBox.Show(s & vbCrLf & ss, "02_443")
   End Sub

   Private Sub dgvActions_DataError(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs) 'Handles dgvActions.DataError
      Dim sMsg As String = "dvgActionsDataErr:" & CStr(e.RowIndex) & "," & CStr(e.ColumnIndex) & "-" & e.Exception.Message
      e.ThrowException = False
      DMAcadExt.AcadDocument.WriteMessage(sMsg)
   End Sub

















   Private Sub zzRefreshAcadDataAAA()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      Dim bTopoSourceExists As Boolean = TopoManager.TopoCreator.TopologyExists(ptMapThemeData.TopoName)
      zzDispTopoExists(bTopoSourceExists)

      bTopoSourceExists = TopoManager.TopoCreator.TopologyExists(ptMapThemeData.LineTopoName)
      zzDispTopoExistsWA(bTopoSourceExists)
      Me.doaLabelCheck(2).Checked = Not DMAcadExt.AcadTransaction.LayersIsEmpty(ptMapThemeData.LineLinkLayers)

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   Private Sub zzRefreshAcadDataOld()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      Me.chkSourceTopologia.Checked = TopoManager.TopoCreator.TopologyExists(ptMapThemeData.TopoName)
      Me.chkLineTopologia.Checked = TopoManager.TopoCreator.TopologyExists(ptMapThemeData.LineTopoName)
      Me.chkStraightenArcs.Checked = Not DMAcadExt.AcadTransaction.LayersIsEmpty(ptMapThemeData.LineLinkLayers)
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub



   Private Sub UpdateTopoByMerge()
      ''''''''''	UpdateTopoByMerge()
   End Sub
   Private Sub zzSetTopoErr()
      Dim tTopoError As TopoError
      If miCurrentTopoErrIndex >= 0 Then
         tTopoError = moaTopoErrors.Item(miCurrentTopoErrIndex)
      End If


   End Sub
   Private Sub zzShowTopoError()

      If miCurrentTopoErrIndex = miTopoErrIndexNotErr OrElse (moaTopoErrors Is Nothing) Then
         If moPriorView Is Nothing Then
            TPlanGraph.TplnProject.SetInitView()
         Else
            DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
         End If
      Else
         Try
            If moaTopoErrors Is Nothing Then
               DMAcadExt.AcadDocument.WriteMessage("_17 moaTopoErrors Is Nothing")
            Else
               '	DMAcadExt.AcadDocument.WriteMessage("moCurrentPoints.UpperBound=" & CStr(moCurrentPoints.UpperBound) & "," & CStr(iErrIndex - 1))
               Dim tTopoError As TopoError = moaTopoErrors.Item(miCurrentTopoErrIndex)
               If tTopoError.Exists Then
                  DMAcadExt.AcadDocument.WriteMessage("TPlnPoint(" & CStr(miCurrentTopoErrIndex + 1) & ")=" & tTopoError.Point.Coordinates)
                  DMAcadExt.AcadDocument.SetView(tTopoError.Point.AcGePoint, 2.0 * tTopoError.Scale, 2.0 * tTopoError.Scale)  '10.0
               Else
                  DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
               End If
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - nudErrors_ValueChanged+")
         End Try
      End If
   End Sub

   Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles nudSteps.ValueChanged
      Dim iCleanupIndex As Integer = zzGetCleanupIndex()
      Me.zzLoadData(iCleanupIndex)

   End Sub
   Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)    'Handles nudErrors.ValueChanged
      Dim iCleanupIndex As Integer = zzGetCleanupIndex()
      Dim iErrIndex As Integer = 0 'Convert.ToInt32(Me.nudErrors(iCleanupIndex).Value)
      If iErrIndex = 0 OrElse (moCurrentPoints Is Nothing) Then
         If moPriorView Is Nothing Then
            TPlanGraph.TplnProject.SetInitView()
         Else
            DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
         End If
      Else
         Try
            If moCurrentPoints Is Nothing Then
               DMAcadExt.AcadDocument.WriteMessage("_11 moCurrentPoints Is Nothing")
            Else
               '	DMAcadExt.AcadDocument.WriteMessage("moCurrentPoints.UpperBound=" & CStr(moCurrentPoints.UpperBound) & "," & CStr(iErrIndex - 1))
               Dim oPoint As DMAcadExt.TPlnPoint = moCurrentPoints.Item(iErrIndex - 1)
               If oPoint IsNot Nothing Then
                  DMAcadExt.AcadDocument.WriteMessage("TPlnPoint(" & CStr(iErrIndex) & ")=" & oPoint.Coordinates)
                  DMAcadExt.AcadDocument.SetView(oPoint.AcGePoint, 100.0, 100.0)
               Else
                  DMAcadExt.AcadDocument.WriteMessage("oPoint Is Nothing")
               End If
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - nudErrors_ValueChanged+")
         End Try
      End If
   End Sub
   Private Sub zzZoomPoint(ByVal oPoint As DMAcadExt.TPlnPoint)
      Try
         Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
         oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
         oViewTableRecord.CenterPoint = oPoint.AcGePoint
         oViewTableRecord.Width = 10.0
         oViewTableRecord.Height = 10.0
         Common.GetEditor().SetCurrentView(oViewTableRecord)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzZoomPoint")
      End Try
   End Sub



































   Private Sub zzRegen()
      '	Dim oAcadEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Try
         DMAcadExt.AcadDocument.Regen()
         '	Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzRegen")
      End Try
   End Sub





   Protected Sub OnInitTabParameters()
      Me.cmdSaveParameters = New TabButton(doLabelFont, False)
      'cmdSaveParameters
      'cmdSaveProjectData
      '
      With Me.cmdSaveParameters
         .Location = New System.Drawing.Point(100, 244)
         .Name = "cmdSaveParameters"
         '	.Size = New System.Drawing.Size(88, 24)
         .TabIndex = 32
         .Text = "Save"
      End With

   End Sub

   Protected Function zzGetDoubleSetting(ByVal sSettingName As String, Optional ByVal dDefaultValue As Double = 0.0) As Double
      Try
         Dim oDynamic As System.Object = My.Settings.Item(sSettingName)
         If oDynamic IsNot Nothing Then
            Return DirectCast(oDynamic, Double)
         Else
            Return dDefaultValue
         End If
      Catch oEx As Exception

         Return 0.0
      End Try
   End Function
   Protected Sub FillFormatRow(ByRef oFormatCombo As ComboBox, ByVal iSectionID As Integer)
      Dim iItemIndex As Integer = 0
      Dim sItemText As String
      Do
         sItemText = zzGetText(iItemIndex, iSectionID, True)
         If sItemText.Length = 0 Then Exit Do
         oFormatCombo.Items.Add(New DMCommon.ItemData(iItemIndex, sItemText))
         iItemIndex += 1
      Loop
   End Sub

   Private Function zzSetGridColumns(iIndex As Integer) As System.Windows.Forms.DataGridViewComboBoxColumn
      Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
      Dim oTxtColumn As DataGridViewTextBoxColumn
      Try
         With oCmbColumn
            .Name = msActionIDFldName
            .DataPropertyName = msActionIDFldName '"ActionID"
            .HeaderText = "Actions"
            .Width = 152
            .Items.Clear()
            .FlatStyle = FlatStyle.Standard
            '	If miCurrentTopoDefID.TopoIsMerge Then
            '.Items.AddRange(TopoManager.TopoCreator.CleanupActionItemsForMerge())
            'Else
            .Items.AddRange(TopoManager.TopoCreator.CleanupActionItems())
            '	End If
            .MaxDropDownItems = .Items.Count
            .ValueMember = DMCommon.ItemData.ValueMember
            .DisplayMember = DMCommon.ItemData.DisplayMember
            .SortMode = DataGridViewColumnSortMode.NotSortable
            '.ReadOnly = TPlanGraph.TplnProject.TopoIsUnion(miCurrentTopoID)
         End With
      Catch oEx As Exception
         DMCommon.Functions.ShowEx(oEx, Me.Name)
      End Try

      Try
         Me.dgvActions(iIndex).Columns.Add(oCmbColumn)
      Catch oEx As Exception
         DMCommon.Functions.ShowEx(oEx, Me.Name)
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = msToleranceFldName
         .HeaderText = "Tolerance"
         .Width = 58
         .Name = "Tolerance"
         .DataPropertyName = "Tolerance"
         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With

      Try
         Me.dgvActions(iIndex).Columns.Add(oTxtColumn)
      Catch oEx As Exception
         DMCommon.Functions.ShowEx(oEx, Me.Name)
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .HeaderText = "Errors"
         .Width = 40
         .Name = msErrorsFldName
         .ReadOnly = True
         .DataPropertyName = msErrorsFldName
         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With
      Try
         Me.dgvActions(iIndex).Columns.Add(oTxtColumn)
      Catch oEx As Exception
         DMCommon.Functions.ShowEx(oEx, Me.Name)
      End Try
      Return oCmbColumn
   End Function
   Private Sub zzLoadData(iCleanupIndex As Integer)
      Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps(iCleanupIndex).Value)
      Dim sStepNo As String = Convert.ToString(Me.nudSteps(iCleanupIndex).Value)
      Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
      Dim sSelectComText As String
      sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"
      moCleanupActionsTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sSelectComText, CommandType.Text, "CleanupActions")
      moCleanupActionsTable.Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
      moCleanupActionsTable.Columns.Add(msPointsFldName, oPointArray.GetType())
      Dim iRowCount As Integer = moCleanupActionsTable.Rows.Count
      If iRowCount > 0 Then
         Dim oRow As DataRow = moCleanupActionsTable.Rows.Item(iRowCount - 1)
         miStepNum = DirectCast(oRow.Item("Step"), Integer)
      End If
      '	MessageBox.Show(CStr(moMapThemeData.CleanupType) & ":" & CStr(iRowCount), "05_200")
      Dim oColumn As DataColumn = moCleanupActionsTable.Columns("Step")
      oColumn.DefaultValue = iStepNo

      moCurrentView(iCleanupIndex) = New DataView(moCleanupActionsTable, "Step=" & sStepNo & "", "", DataViewRowState.CurrentRows)
      Me.dgvActions(iCleanupIndex).DataSource = moCurrentView(iCleanupIndex)
   End Sub




   Protected Shared Function zzGetBaseText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
      Try
         Return TPlServerDB.TextResource.GetText(iItemID, miBaseResourceTheme, iSectionID, bReturnEmpty)
      Catch oEx As Exception
         Return String.Empty
      End Try

   End Function
   Protected Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
      Try
         Return TPlServerDB.TextResource.GetText(iItemID, diResourceTheme, iSectionID, bReturnEmpty)
      Catch oEx As Exception
         Return String.Empty
      End Try

   End Function
   Private Structure GroupLocation
      Dim GroupXShift As Integer
      Dim GroupYShiftIndex As Integer
      Dim LabelRightX As Integer
      Dim LabelLength As Integer
   End Structure
   Private Structure TopoLocation
      Dim BaseLocation As GroupLocation
      Dim YPosIndex As Integer
      Public ReadOnly Property GroupXShift() As Integer
         Get
            Return BaseLocation.GroupXShift
         End Get
      End Property
      Public Function GetYPosIndex() As Integer
         Return BaseLocation.GroupYShiftIndex + YPosIndex
      End Function
      Public ReadOnly Property LabelRightX() As Integer
         Get
            Return BaseLocation.LabelRightX
         End Get
      End Property
      Public ReadOnly Property LabelLength() As Integer
         Get
            Return BaseLocation.LabelLength
         End Get
      End Property
   End Structure
   Private Sub frmTopoActionsBase_Activated(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Activated
      '	mbInactive = False
      '	Me.Opacity = 1.0
      ''''''''''''''''''	Me.tmrInactive.Stop()
   End Sub

   Private Sub frmTopoActionsBase_Deactivate(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Deactivate
      '		mbInactive = True
      '		mdInactiveTiks = 0.0
      '		Me.tmrInactive.Start()
   End Sub


   Private Sub frmTopoActionsBase_FormClosing(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      Dim iCloseReason As CloseReason = e.CloseReason
      DMAcadExt.AcadDocument.RestoreVarCmdDia()

      '	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_202")

      If Me.DialogResult = Windows.Forms.DialogResult.None AndAlso iCloseReason = CloseReason.UserClosing Then
         Me.Hide()
         e.Cancel = True

      End If
      Common.SetAcadFocus()
      '	System.Windows.Forms.MessageBox.Show(e.CloseReason.ToString() & ":" & Me.DialogResult.ToString(), "12_888 Base frmTopoActions_FormClosing")
   End Sub





   Private Sub tmrInactive_Tick(ByVal oSender As System.Object, ByVal e As System.EventArgs) '''''''''''''''''''''''' Handles tmrInactive.Tick
      Const dMinTiks As Double = 2.0
      'DMAcadExt.AcadDocument.WriteMessage(CStr(mdInactiveTiks))
      If mbInactive Then
         mdInactiveTiks += 1.0
         If mdInactiveTiks <= dMinTiks Then
            Me.Opacity = 1.0
         ElseIf mdInactiveTiks < 10.0 Then
            Me.Opacity = 1.2 - 0.1 * mdInactiveTiks
         Else
            Me.Opacity = 0.2
         End If
      End If
   End Sub

   Private Sub tsbExportToShape_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles tsbExportToShape.Click
      '	System.Windows.Forms.MessageBox.Show("", "tsbExportToShape_Click")
      '	zzCreateFDOLayer()
      '	zzExportToShape()

   End Sub

   Private Sub txtStraightenTolerance_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles txtStraightenTolerance.Leave
      If IsNumeric(txtStraightenTolerance) Then
         Parameters.LegendPaintFactor = Convert.ToDouble(Me.txtStraightenTolerance.Text)
      Else
         Me.txtStraightenTolerance.Text = Convert.ToString(Parameters.StraightenTolerance)
      End If
   End Sub
   Private Sub txtLegendPaintFactor_Leave(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles txtLegendPaintFactor.Leave
      If IsNumeric(Me.txtLegendPaintFactor.Text) Then
         Parameters.LegendPaintFactor = Convert.ToDouble(Me.txtLegendPaintFactor.Text)
      Else
         Me.txtLegendPaintFactor.Text = Convert.ToString(Parameters.LegendPaintFactor)
      End If
   End Sub
   Protected MustOverride Sub AfterChangeCurrent(iNewIndex As Integer)


   Protected Sub OnChangeCurrent(iNewIndex As Integer)
      Dim iPrevIndex As Integer = LabelCheck.CurrentIndex
      '	MessageBox.Show(CStr(doaPanels.GetUpperBound(0)) & ":" & CStr(iNewIndex) & ":" & CStr(iPrevIndex), "04_532")
      If iNewIndex <> iPrevIndex Then
         If iPrevIndex <> -1 AndAlso iPrevIndex <= doaLabelCheck.GetUpperBound(0) Then
            doaLabelCheck(iPrevIndex).SetNotCurrent()
         End If
         '	MessageBox.Show(CStr(doaPanels.GetUpperBound(0)) & ":" & CStr(iPrevIndex) & ":" & CStr(iNewIndex), "04_545") ''071214
         For iIndex As Integer = 0 To piStagesUB
            If doaPanels.GetUpperBound(0) <> piStagesUB Then
               MessageBox.Show(CStr(doaPanels.GetUpperBound(0)) & ":" & CStr(piStagesUB), "04_540")
            Else
               If Me.doaPanels(iIndex) IsNot Nothing AndAlso Not Me.doaPanels(iIndex).IsDisposed Then

                  Me.doaPanels(iIndex).Visible = (iNewIndex = iIndex)
               End If
            End If

         Next
      End If

      '	MessageBox.Show(CStr(doaLabelCheck.GetUpperBound(0)) & ":" & CStr(doaPanels.GetUpperBound(0)) & ":" & CStr(iNewIndex), "04_895a")

   End Sub
   Protected Sub TestPanel()
      Dim s As String = CStr(piStagesUB)
      For iIndex As Integer = 0 To piStagesUB

         If Me.doaPanels(iIndex) IsNot Nothing AndAlso Not Me.doaPanels(iIndex).IsDisposed Then
            s &= vbCrLf & CStr(iIndex) & ":" & CStr(Me.doaPanels(iIndex).Visible)

         End If

      Next
      MessageBox.Show(s, "01_555")
   End Sub
   Protected Class LabelCheck
      Inherits System.Windows.Forms.Label
      Private WithEvents moCheck As Label
      Private WithEvents moCurrent As Label
      Private miIndex As Integer
      Private mbIsCurrent As Boolean = False
      Private mbCheckExists As Boolean = True
      Private mbChecked As Boolean = False
      Private msText As String
      Private msCurrentText As String

      Public Shared Event ChangeCurrent(iNewIndex As Integer)
      Public Shared CurrentIndex As Integer = -1
      Public Sub New(iIndex As Integer)
         miIndex = iIndex
         MyBase.Name = "lblLabelCheck" & CStr(iIndex)
         MyBase.Size = New System.Drawing.Size(184, 16)
         MyBase.TextAlign = ContentAlignment.BottomLeft
         moCheck = New Label
         moCurrent = New Label
         '	moCheck.Image = Global.TopoUI.My.Resources.Resources.DoneTr
         moCheck.Name = "lblOK" & CStr(iIndex)
         moCheck.Size = New System.Drawing.Size(20, 16)
         '	moCheck.Visible = False
         zzSetChecked(False)
         ''''''''''''''		moCurrent.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
         moCurrent.Name = "lblCurrent" & CStr(iIndex)
         moCurrent.Size = New System.Drawing.Size(30, 16)
         moCurrent.ImageAlign = ContentAlignment.MiddleLeft

         '''''''''''''		moCurrent.Visible = False
         MyBase.Font = doLabelFont

         '	moLabelBoldFont(A)
      End Sub
      Public Property CurrentText As String
         Get
            Return msCurrentText
         End Get
         Set(sValue As String)
            msCurrentText = sValue
         End Set
      End Property
      Private Sub zzCurrentVisible(bVisible As Boolean)
         If bVisible Then
            moCurrent.Image = Global.TopoUI.My.Resources.Resources.ArrowLeft16Tr
         Else
            moCurrent.Image = Nothing
         End If

      End Sub
      Private Sub zzSetChecked(bVisible As Boolean)
         If bVisible Then
            moCheck.Image = Global.TopoUI.My.Resources.Resources.DoneTr
         Else
            moCheck.Image = Nothing
         End If

      End Sub




      Public Overloads Property Location As System.Drawing.Point
         Get
            Return MyBase.Location
         End Get
         Set(tValue As System.Drawing.Point)
            MyBase.Location = tValue
            moCheck.Location = New System.Drawing.Point(tValue.X + MyBase.Size.Width, tValue.Y)
            moCurrent.Location = New System.Drawing.Point(moCheck.Location.X + moCheck.Size.Width, tValue.Y)
         End Set
      End Property
      Public Property CheckLabel As Label
         Get
            Return moCheck
         End Get
         Set(oValue As Label)
            moCheck = oValue
         End Set
      End Property
      Public Property CurrentLabel As Label
         Get
            Return moCurrent
         End Get
         Set(oValue As Label)
            moCheck = oValue
         End Set
      End Property

      Public Property CheckExists As Boolean
         Get
            Return mbCheckExists
         End Get
         Set(bValue As Boolean)
            mbCheckExists = bValue
            '	moCheck.Visible = bValue
            zzSetChecked(bValue)
            If Not mbCheckExists Then
               mbChecked = False
            End If
         End Set
      End Property
      Public Property Checked As Boolean
         Get
            Return mbChecked
         End Get
         Set(bValue As Boolean)
            If mbCheckExists Then
               mbChecked = bValue
               '	moCheck.Visible = bValue
               zzSetChecked(bValue)
            End If
         End Set
      End Property
      Public Property IsCurrent As Boolean
         Get
            Return mbIsCurrent
         End Get
         Set(bValue As Boolean)
            mbIsCurrent = bValue
         End Set
      End Property
      Public Sub AddToForm(ByRef colControls As System.Windows.Forms.Control.ControlCollection)
         colControls.Add(Me)
         colControls.Add(moCheck)
         colControls.Add(moCurrent)
      End Sub
      Public Sub SetNotCurrent()
         '	moCurrent.Visible = False
         If mbIsCurrent Then
            zzCurrentVisible(False)
            MyBase.Font = doLabelFont
            mbIsCurrent = False
            If Not String.IsNullOrEmpty(msText) Then
               Me.Text = msText
            End If
         End If
      End Sub
      Public Sub SetCurrent()
         '071214
         '	MessageBox.Show(CStr(mbIsCurrent) & ":" & CurrentIndex & ":" & miIndex, "05_290")
         If Not mbIsCurrent Then
            zzCurrentVisible(True)
            MyBase.Font = moLabelSelectFont
            RaiseEvent ChangeCurrent(miIndex)
            CurrentIndex = miIndex
            mbIsCurrent = True
            If Not String.IsNullOrEmpty(msCurrentText) Then
               msText = Me.Text
               Me.Text = msCurrentText
            End If
         End If

      End Sub
      Private Sub moCheck_Click(oSender As System.Object, e As System.EventArgs) Handles moCheck.Click
         If Me.Enabled Then
            SetCurrent()
         End If

      End Sub

      Private Sub moCurrent_Click(oSender As System.Object, e As System.EventArgs) Handles moCurrent.Click
         If Me.Enabled Then
            SetCurrent()
         End If
      End Sub

      Private Sub LabelCheck_Click(oSender As System.Object, e As System.EventArgs) Handles Me.Click
         SetCurrent()
      End Sub


   End Class




   Private Sub cmdFirst_Click(oSender As System.Object, e As System.EventArgs) Handles cmdFirst.Click
      doaLabelCheck(0).SetCurrent()
      '	TestPanel()
   End Sub

   Private Sub cmdLast_Click(oSender As System.Object, e As System.EventArgs) Handles cmdLast.Click
      doaLabelCheck(piStagesUB).SetCurrent()
      '	TestPanel()
   End Sub
   Private Sub cmdExit_Click(oSender As System.Object, e As System.EventArgs) Handles cmdExit.Click
      Me.Close()
      RaiseEvent OnClose()
   End Sub
   Private Sub cmdPrev_Click(oSender As System.Object, e As System.EventArgs) Handles cmdPrev.Click
      Dim iNewIndex As Integer = LabelCheck.CurrentIndex - 1
      Do While iNewIndex >= 0
         If doaLabelCheck(iNewIndex).Enabled Then
            doaLabelCheck(iNewIndex).SetCurrent()
            Exit Do
         Else
            iNewIndex -= 1
         End If
      Loop
      '	TestPanel()
   End Sub
   Private Sub cmdPrev_Click_1(oSender As System.Object, e As System.EventArgs)
      Dim iCurrentIndex As Integer = LabelCheck.CurrentIndex
      If iCurrentIndex > 0 Then
         doaLabelCheck(iCurrentIndex - 1).SetCurrent()
      End If
   End Sub

   Private Sub cmdNext_Click(oSender As System.Object, e As System.EventArgs) Handles cmdNext.Click
      Dim iNewIndex As Integer = LabelCheck.CurrentIndex + 1
      Do While iNewIndex <= piStagesUB
         If doaLabelCheck(iNewIndex).Enabled Then
            doaLabelCheck(iNewIndex).SetCurrent()
            Exit Do
         Else
            iNewIndex += 1
         End If
      Loop
      '	TestPanel()
   End Sub




   Private Sub ddbLayers_DropDownItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbLayers.DropDownItemClicked
      Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
      Select Case oToolStripItem.Name
         Case Me.tsiThisTopoOnlyVisible.Name
            Select Case LabelCheck.CurrentIndex
               Case 1
                  zzSourceOnlyVisible(False)
               Case 4
                  zzSourceOnlyVisible(True)
            End Select
         Case Me.tsiThisTopoVisible.Name
            MessageBox.Show(CStr(LabelCheck.CurrentIndex), "01_871 This")
            Select Case LabelCheck.CurrentIndex
               Case 1
                  zzSourceVisible(False)
               Case 4
                  zzSourceVisible(True)
            End Select
         Case tsiAllVisible.Name
            MessageBox.Show("", "01_878 BASE")
            zzAllVisible()

      End Select
   End Sub
   Private Sub zzSourceOnlyVisible(bLineLinks As Boolean)
      Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisOnlyVisible)
      Dim sLineLinkLayers As String
      If bLineLinks Then
         sLineLinkLayers = ptMapThemeData.LineLinkLayers
      Else
         sLineLinkLayers = String.Empty
      End If

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & sLineLinkLayers & vbCrLf & ptMapThemeData.CentroidBlocks & vbCrLf & ptMapThemeData.CentroidLayers, "02_117")

      DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, ptMapThemeData.LinkLayers, sLineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub

   Private Sub zzSourceVisible(bLineLinks As Boolean)
      Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzThisVisible)
      Dim sLineLinkLayers As String
      If bLineLinks Then
         sLineLinkLayers = ptMapThemeData.LineLinkLayers
      Else
         sLineLinkLayers = String.Empty
      End If

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
      MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & sLineLinkLayers & vbCrLf & ptMapThemeData.CentroidBlocks & vbCrLf & ptMapThemeData.CentroidLayers, "02_103")
      DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, ptMapThemeData.LinkLayers, sLineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub

   Private Sub zzThisOnlyVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
      oEntity.Visible = bCond
   End Sub
   Private Sub zzThisVisible(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
      If bCond Then
         oEntity.Visible = True
      End If
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

   Private Sub zzCreateTopoAAA()
      Dim dTolerance As Double
      Dim bCreateCentroids As Boolean
      Dim tTopoRes As DMAcadExt.TopoRes

      Me.Cursor = Cursors.WaitCursor
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

      If Information.IsNumeric(Me.txtTolerance.Text) Then
         dTolerance = Convert.ToDouble(Me.txtTolerance.Text)
         bCreateCentroids = Me.chkCreateCentroid.Checked
         Try
            tTopoRes = TopoCreator.CreateTopology(ptMapThemeData.TopoName, ptMapThemeData.LinkLayers, ptMapThemeData.LineLinkLayers, ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers, bCreateCentroids, String.Empty, String.Empty, False, Me.chkHighlightSliver.Checked, dTolerance)
            If tTopoRes.TopoExists Then
               Me.txtPgonCount.Text = CStr(tTopoRes.PgonCount)
               Me.txtLinkCount.Text = CStr(tTopoRes.LinkCount)
               Me.txtCentroidCount.Text = CStr(tTopoRes.CentroidCount)
               zzDispTopoExists(True)
               moaTopoErrors = TopoCreator.GetTopoErrors(enTopoErrType.RefRMark)
               If moaTopoErrors IsNot Nothing Then
                  System.Windows.Forms.MessageBox.Show(CStr(moaTopoErrors.UpperBound) & ":" & CStr(tTopoRes.MissingCntrCount), "02_540")
               End If

            Else
               zzDispTopoExists(False)
               moaTopoErrors = TopoCreator.GetAllTopoErrors()

            End If
            If moaTopoErrors IsNot Nothing Then
               Me.txtErrorCount.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
               miCurrentTopoErrIndex = miTopoErrIndexNotErr
               zzDispErrIndex()
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmMapThemeBase - zzCreateTopo_01")
         End Try
      Else
         Beep()
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
      Me.Cursor = Cursors.Default


   End Sub
   Private Sub zzInitPanels()
      'System.Windows.Forms.MessageBox.Show(CStr(piStagesUB) & vbCrLf & "", "frmTopoCleanup - zzInitPanels")
      For iIndex As Integer = 0 To piStagesUB
         doaPanels(iIndex) = New System.Windows.Forms.Panel()
         With doaPanels(iIndex)
            .RightToLeft = Windows.Forms.RightToLeft.No
            .Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
            .Location = New System.Drawing.Point(0, 0)
            .Name = "Panel" & Convert.ToString(iIndex)
            .Size = dtPanelSize ' New System.Drawing.Size(300, 244)
            .TabIndex = iIndex
            Me.Controls.Add(doaPanels(iIndex))
         End With

      Next


   End Sub
   Private Sub zzInitLabelChecks()
      '	MessageBox.Show(CStr(piStagesUB) & ":" & CStr(doaLabelCheck.GetUpperBound(0)), "04_980")
      For iIndex As Integer = 0 To piStagesUB

         doaLabelCheck(iIndex) = New LabelCheck(iIndex)
         doaLabelCheck(iIndex).Location = New System.Drawing.Point(302, piLabelTop + iIndex * 32)
         doaLabelCheck(iIndex).Text = psaCaptions(iIndex)
         If psaCurrentCaptions IsNot Nothing Then
            doaLabelCheck(iIndex).CurrentText = psaCurrentCaptions(iIndex)
         End If


         doaLabelCheck(iIndex).AddToForm(Me.Controls)
         If iIndex = -100 Then
            doaLabelCheck(iIndex).CheckExists = False
         End If
      Next
      AddHandler LabelCheck.ChangeCurrent, AddressOf AfterChangeCurrent

   End Sub

   Private Sub zzDispTopoErrors()
      If moaTopoErrors IsNot Nothing Then
         Me.txtErrorCount.Text = Convert.ToString(moaTopoErrors.UpperBound + 1)
      Else
         Me.txtErrorCount.Text = "0"
      End If
      miCurrentTopoErrIndex = miTopoErrIndexNotErr
      zzDispErrIndex()
   End Sub


   Private Sub zzDispTopoExists(bExists As Boolean)
      Me.doaLabelCheck(1).Checked = bExists
      Me.lblTopoExists.Visible = bExists
      Me.txtPgonCount.Enabled = bExists
      Me.lblTopoName.ForeColor = ColorByExists(bExists)
      Me.lblTopoName.Font = FontByExists(bExists)
     


   End Sub
   Private Sub zzDispTopoExistsWA(bExists As Boolean)
      Me.doaLabelCheck(4).Checked = bExists
      Me.lblTopoExistsWA.Visible = bExists
      Me.txtPgonCountWA.Enabled = bExists
      Me.lblTopoNameWA.ForeColor = ColorByExists(bExists)
      Me.lblTopoNameWA.Font = FontByExists(bExists)

    


   End Sub





   Private Sub zzGetStatistics()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

      zzGetStatisticsA()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()





   End Sub
   Private Sub zzGetStatisticsA()


      Dim tMapThemeInfo As DMAcadExt.MapThemeInfo = DMAcadExt.AcadTransaction.GetStatistics(ptMapThemeData)
      Me.txtArcCount.Text = CStr(tMapThemeInfo.ArcCount)
      Me.txtCentroidCount.Text = CStr(tMapThemeInfo.CentroidBlocksCount)
      Me.txtCentroidCountWA.Text = CStr(tMapThemeInfo.CentroidBlocksCount)
      Me.txtLinkCount.Text = CStr(tMapThemeInfo.LinkCount)
      Me.txtLinkCountP2.Text = CStr(tMapThemeInfo.LinkCount)
      Me.txtLineLinkCount.Text = CStr(tMapThemeInfo.LineLinkCount)
      Me.txtLineLinkCountB.Text = CStr(tMapThemeInfo.LineLinkCount)

      Me.txtLinkWithArcCount.Text = CStr(tMapThemeInfo.LinkWithArcCount)

      Me.txtLinkCountWA.Text = CStr(tMapThemeInfo.LinkCount - tMapThemeInfo.LineLinkCount)




   End Sub

   Protected Class TabButton
      Inherits Button
      Private Shared moMyCursor As Cursor = Cursors.Hand
      Public Sub New(ByVal oFont As Font, ByVal bImage As Boolean)
         Dim iWidth As Integer
         If bImage Then
            iWidth = 24
         Else
            iWidth = 48
         End If
         MyBase.Size = New System.Drawing.Size(iWidth, 24)
         MyBase.Font = oFont
         MyBase.Cursor = moMyCursor
         MyBase.FlatStyle = System.Windows.Forms.FlatStyle.Standard
         MyBase.UseVisualStyleBackColor = True
      End Sub
      Public Overrides Sub ResetCursor()
         MyBase.Cursor = moMyCursor
      End Sub
   End Class

   Private Sub zzEraseErrors()
      Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzDeleteBlockRefs)

      Dim sErrBlockNameList As String = Join(TopoCreator.TopoErrBlockNames, ",")

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, , , sErrBlockNameList)
      moaTopoErrors.Clear()
      zzDispTopoErrors()
      zzRegen()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub

   Private Sub zzDeleteBlockRefs(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
      If bCond Then
         oEntity.Erase()
      End If

   End Sub


   Private Sub zzDispErrIndex()
      Try
         Me.txtErrorIndex.Text = Convert.ToString(miCurrentTopoErrIndex + 1)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoCleanup - zzDispErrIndex")
      End Try
   End Sub

   Private Sub frmTopoCleanup_Paint(oSender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
      zzPaintForm(e.Graphics)
   End Sub
   Private Sub zzPaintForm(oGraphics As Graphics)
      Dim oPen As Pen = New Pen(Color.Black, 2.0!)
      Dim tPoint1 As Point = New Point(miVertLineX, 0)
      Dim tPoint2 As Point = New Point(miVertLineX, Me.ClientSize.Height)

      oGraphics.DrawLine(oPen, tPoint1, tPoint2)
   End Sub





   Public MustOverride ReadOnly Property IsDoneA As Boolean Implements ICheckTheme.IsDone


End Class