Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports TopoManager.TPlanGraph
Public Class frmClosedPgons
	Dim miThisStagesUB As Integer
   Const msRootPath As String = "R:\Gushim\Gushim-Vectorized"
	Private mtTopoRes(1) As DMAcadExt.TopoRes
	'	Private mtTopoRes As DMAcadExt.TopoRes
	Private mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private WithEvents mfParcelsFromShapes As frmParcelsFromShapes
   Private WithEvents mfLotsFromShapes As frmLotsFromShapes

   Private WithEvents mfrmPgonSetView As frmPgonSetView = Nothing
	Private msaParcelFolders() As String
	'	Protected ptMapThemeData As DMAcadExt.MapThemeData
	'	Protected ptDissolveMapThemeData As DMAcadExt.MapThemeData

	Private miPoligonCount As Integer
	Private msCurrentRootName As String = msRootPath
   Private moPgonSet As FDO.TplnPolygonSet
   Private mtMapThemeInfo As DMAcadExt.MapThemeInfo
   Private moWorkAreaBound As Polyline
   Private erpWorkArea(1) As System.Windows.Forms.ErrorProvider
#Region "Panel0_1_Declarations"

   Private lblCaption(1) As Label
   Private chkCreateCentroid As System.Windows.Forms.CheckBox
   Private chkHighlightSliver As System.Windows.Forms.CheckBox
   Private lblTolerance As System.Windows.Forms.Label
   Private txtTolerance As System.Windows.Forms.TextBox

   Private grbLinks(1) As System.Windows.Forms.GroupBox
   Private grbWorkArea(1) As System.Windows.Forms.GroupBox


   Private grbCentroids(1) As System.Windows.Forms.GroupBox
   Private lblCentroidBlocks(1) As System.Windows.Forms.Label
   Private lblCentroidLayers(1) As System.Windows.Forms.Label
   '	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
   Private lblTopoErrors As System.Windows.Forms.Label
   Private txtErrorCount As System.Windows.Forms.TextBox
   Private txtErrorIndex As System.Windows.Forms.TextBox
   Private lblTopoName As System.Windows.Forms.Label
   Private lblTopoNameCap As System.Windows.Forms.Label



   Private lblTopoExists As System.Windows.Forms.Label
   '	Private txtPgonCount As System.Windows.Forms.TextBox
   Private txtCentroidCount(1) As System.Windows.Forms.TextBox
   Private txtCentroidBlocks As System.Windows.Forms.TextBox
   Private txtCentroidLayers As System.Windows.Forms.TextBox
   Private lblLinkLayers(1) As System.Windows.Forms.Label
   Private lblClosedPolylineCount(1) As System.Windows.Forms.Label

   Private txtLinkLayers(1) As System.Windows.Forms.TextBox
   Private txtLinkCount(1) As System.Windows.Forms.TextBox
   Private txtClosedPolylineCount(1) As System.Windows.Forms.TextBox

   Private txtWorkAreaLayers(1) As System.Windows.Forms.TextBox
   Private txtWorkAreaLinkCount(1) As System.Windows.Forms.TextBox
   Private txtWorkAreaWarning(1) As System.Windows.Forms.TextBox
   Private lblWorkAreaLinkLayers(1) As System.Windows.Forms.Label

   Private miaPgonCount(1) As Integer
   Private mcolLinks As ObjectIdCollection
   Private moFDO_Manager As FDO.FDO_Manager
#End Region

   Protected Overrides Sub AfterChangeCurrent(iNewIndex As Integer)
      MyBase.OnChangeCurrent(iNewIndex)
      Select Case iNewIndex
         Case 0, 1
            Try
               Me.doaPanels(iNewIndex).Controls.Add(Me.tstClosedPgons)
               '	
               If Me.tstTopology IsNot Nothing Then
                  Me.doaPanels(iNewIndex).Controls.Add(Me.tstTopology)
                  'MessageBox.Show(CStr(Me.tstTopology.Items.Count) & ":" & CStr(iNewIndex), "01_262")
               End If
               '	Me.doaPanels(iNewIndex).Controls.Add(Me.tstClosedPgons)
            Catch oEx As Exception
               MessageBox.Show(oEx.Message & ":" & CStr(iNewIndex) & ":" & CStr(Me.doaPanels.GetUpperBound(0)), "01_199")
            End Try

      End Select
   End Sub

   Public Overrides ReadOnly Property IsDone As Boolean
      Get
         Return mtTopoRes(0).IsOK
      End Get
   End Property

	Public Overrides ReadOnly Property IsDoneA As Boolean
		Get

		End Get
	End Property
	Public Overrides Sub ExecDefaultAction()

	End Sub

	Private Sub zzMyInitializeComponent()
      Dim saCaptions(miThisStagesUB) As String '= {"חלקות", "גושים"}
      saCaptions(0) = ptMapThemeData.MapThemeName()
      If miThisStagesUB = 1 Then
         saCaptions(1) = ptDissolveMapThemeData.MapThemeName()
      End If

      MyBase.psaCaptions = saCaptions
      MyBase.OnNew()
      zzInitToolStrip()
      zzInitToolStripA()
      zzInitPanel0_1(0, 50)
      If miThisStagesUB = 1 Then
         zzInitPanel0_1(1, 50)
      End If


      '	zzCheckTopo(iIndex, False)
      '	zzCheckMapLayer(iIndex)


      AfterChangeCurrent(0)
      '
   End Sub
	Private Sub zzOpenCheckPgons(tMapThemeData As DMAcadExt.MapThemeData, bDissolve As Boolean)

		'Dim bIsDone As Boolean = False
		Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
		'  Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
		'   MessageBox.Show(tMapThemeData.TopoName, "02_376")


		Me.Cursor = Cursors.WaitCursor

		mfrmPgonSetView = New frmPgonSetView(tMapThemeData, ptDissolveMapThemeData, bDissolve)
		'  MessageBox.Show(CStr(moPgonSet Is Nothing), "02_377")
		If moPgonSet IsNot Nothing Then
			mfrmPgonSetView.PgonSet = moPgonSet
		End If

		'  MessageBox.Show(CStr(moPgonSet Is Nothing), "02_378")
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfrmPgonSetView)
		Me.Visible = False
		Me.Cursor = Cursors.Default
		'   MessageBox.Show(CStr(moPgonSet Is Nothing), "02_379")
	End Sub

	Private Sub zzGetStatistics(bParcel As Boolean, bWorkArea As Boolean)

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
      If bParcel Then
         zzGetStatisticsA(0, ptMapThemeData)
      End If
      If bWorkArea Then
         zzGetWorkAreaLinks(0)
      End If


      If miThisStagesUB = 1 Then
         zzGetStatisticsA(1, ptDissolveMapThemeData)
         '   zzGetWorkAreaLinks(1)
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzGetStatisticsA(iIndex As Integer, tMapThemeData As DMAcadExt.MapThemeData)
      '  MessageBox.Show(CStr(mtMapThemeInfo.PgonSetCount) & ":" & CStr(mtMapThemeInfo.LinkCount) & ":" & CStr(mtMapThemeInfo.LineLinksOK), "01_988")
      mtMapThemeInfo = DMAcadExt.AcadTransaction.GetStatistics(tMapThemeData)
      '	Me.txtArcCount.Text = CStr(mtMapThemeInfo.ArcCount)
      Me.txtCentroidCount(iIndex).Text = CStr(mtMapThemeInfo.CentroidBlocksCount)

      Me.txtLinkCount(iIndex).Text = CStr(mtMapThemeInfo.LinkCount)
      Me.txtClosedPolylineCount(iIndex).Text = CStr(mtMapThemeInfo.PgonSetCount)
      Me.txtWorkAreaLinkCount(iIndex).Text = CStr(mtMapThemeInfo.WorkAreaRingCount)
      '   Me.txtWorkAreaLinkCount(iIndex).Invalidate()
      'Me.txtLineLiC.Text = CStr(mtMapThemeInfo.LineLinkCount)
      'Me.txtLineLinkCoun.Text = CStr(mtMapThemeInfo.LineLinkCount)
      'Me.txtLineLinkCountB.Text = CStr(mtMapThemeInfo.LineLinkCount)

      'Me.txtLinkWithArcCount.Text = CStr(mtMapThemeInfo.LinkWithArcCount)

      'Me.txtLinkCountWA.Text = CStr(mtMapThemeInfo.LinkCount - mtMapThemeInfo.LineLinkCount)

      '	MessageBox.Show(CStr(mtMapThemeInfo.PgonSetCount) & ":" & CStr(mtMapThemeInfo.LinkCount) & ":" & CStr(mtMapThemeInfo.LineLinksOK), "01_988")

      'zzDispLineLinksExist(mtMapThemeInfo.LineLinksOK)

   End Sub
   Private Sub zzGetWorkAreaLinks(iIndex As Integer)
      mcolLinks = DMAcadExt.AcadTransaction.GetLinksNew(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)
      Me.txtWorkAreaLinkCount(iIndex).Text = Convert.ToString(mcolLinks.Count)
      If mcolLinks.Count > 1 Then
         ' MessageBox.Show(CStr(iIndex) & vbCrLf & CStr(mcolLinks.Count), "04_400")
         '   Me.erpWorkArea(iIndex).SetError(Me.txtWorkAreaLinkCount(iIndex), "aaa" & CStr(iIndex))
         Me.erpWorkArea(0).SetError(Me.txtWorkAreaWarning(0), "a" & CStr(iIndex))
         Me.erpWorkArea(1).SetError(Me.txtWorkAreaWarning(1), "a" & CStr(iIndex))



      End If
   End Sub


   Private Sub frmClosedPgons_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
      MyBase.SetLabelDim(miThisStagesUB)
      zzMyInitializeComponent()
      zzGetStatistics(True, True)
      '	zzDispTopoOK(0)
      zzDispCentroids()
      '	zzCheckMPolygonSet()
   End Sub

   Private Sub zzInitPanel0_1(iIndex As Integer, iTopA As Integer)
      Dim tMapThemeData As DMAcadExt.MapThemeData = Nothing
      zzGetMapThemeData(iIndex, tMapThemeData)
      Me.lblTopoExists = New System.Windows.Forms.Label()
      Me.lblCaption(iIndex) = New System.Windows.Forms.Label()

      Me.txtErrorCount = New System.Windows.Forms.TextBox()
      Me.txtErrorIndex = New System.Windows.Forms.TextBox()
      Me.lblTopoName = New System.Windows.Forms.Label()
      Me.lblTopoNameCap = New System.Windows.Forms.Label()
      '''''''''''''''''''''''	Me.tstTopology = New System.Windows.Forms.ToolStrip()
      Me.lblTopoErrors = New System.Windows.Forms.Label()
      Me.grbCentroids(iIndex) = New System.Windows.Forms.GroupBox()
      Me.txtCentroidCount(iIndex) = New System.Windows.Forms.TextBox()
      Me.txtCentroidBlocks = New System.Windows.Forms.TextBox()
      Me.txtCentroidLayers = New System.Windows.Forms.TextBox()
      Me.lblCentroidBlocks(iIndex) = New System.Windows.Forms.Label()
      Me.lblCentroidLayers(iIndex) = New System.Windows.Forms.Label()

      Me.txtLinkCount(iIndex) = New System.Windows.Forms.TextBox()
      Me.txtClosedPolylineCount(iIndex) = New System.Windows.Forms.TextBox()
      Me.txtLinkLayers(iIndex) = New System.Windows.Forms.TextBox()
      Me.lblLinkLayers(iIndex) = New System.Windows.Forms.Label()
      Me.lblClosedPolylineCount(iIndex) = New System.Windows.Forms.Label()
      Me.grbWorkArea(iIndex) = New System.Windows.Forms.GroupBox()
      Me.grbLinks(iIndex) = New System.Windows.Forms.GroupBox()

      Me.txtWorkAreaLayers(iIndex) = New System.Windows.Forms.TextBox()
      Me.txtWorkAreaLinkCount(iIndex) = New System.Windows.Forms.TextBox()
      Me.txtWorkAreaWarning(iIndex) = New System.Windows.Forms.TextBox()



      Me.lblWorkAreaLinkLayers(iIndex) = New System.Windows.Forms.Label()


      Me.txtTolerance = New System.Windows.Forms.TextBox()
      Me.lblTolerance = New System.Windows.Forms.Label()
      Me.chkHighlightSliver = New System.Windows.Forms.CheckBox()
      Me.chkCreateCentroid = New System.Windows.Forms.CheckBox()
      Me.erpWorkArea(iIndex) = New System.Windows.Forms.ErrorProvider(Me.components)
      Me.erpWorkArea(iIndex).ContainerControl = Me
      '	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154a")
      If False Then
         Me.doaPanels(1).SuspendLayout()
         Me.grbCentroids(iIndex).SuspendLayout()
         Me.grbLinks(iIndex).SuspendLayout()
         Me.SuspendLayout()
      End If

      '
      'PanelX1
      '
      With Me.doaPanels(iIndex)
         '	.RightToLeft = Windows.Forms.RightToLeft.No
         '	.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
         .AutoSize = True '!!!???
         With .Controls

            .Add(Me.lblTopoExists)
            .Add(Me.lblCaption(iIndex))

            .Add(Me.txtErrorCount)
            .Add(Me.txtErrorIndex)
            .Add(Me.lblTopoName)
            .Add(Me.lblTopoNameCap)
            '	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154b")
            .Add(Me.tstTopology)
            .Add(Me.lblTopoErrors)
            .Add(Me.grbCentroids(iIndex))
            .Add(Me.grbLinks(iIndex))
            .Add(Me.grbWorkArea(iIndex))
            .Add(Me.txtTolerance)
            .Add(Me.lblTolerance)
            .Add(Me.chkHighlightSliver)
            .Add(Me.chkCreateCentroid)
         End With

      End With
      '
      'lblCaption
      '
      With Me.lblCaption(iIndex)
         .Location = New System.Drawing.Point(8, 36)  '36
         .Name = "lblCaption"
         .Size = New System.Drawing.Size(288, 24)
         .TabIndex = 29
         .Font = doCaptionBoldFont
         .Text = tMapThemeData.MapThemeName
         '	.BorderStyle = BorderStyle.FixedSingle
         .TextAlign = ContentAlignment.MiddleCenter
         '	.BorderStyle = BorderStyle.FixedSingle
      End With
      '
      'lblTopoExists
      '
      With Me.lblTopoExists
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .Location = New System.Drawing.Point(8, 76)
         .Name = "lblTopoExists"
         .Size = New System.Drawing.Size(20, 18)
         .TabIndex = 19
         .RightToLeft = Windows.Forms.RightToLeft.No
         '.Visible = False
      End With


      '
      'txtErrorCount
      '
      With Me.txtErrorCount
         .Location = New System.Drawing.Point(102, 214)
         .Name = "txtErrorCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(42, 22)
         .TabIndex = 15
         .RightToLeft = Windows.Forms.RightToLeft.No
         .Visible = False

      End With
      '
      'txtErrorIndex
      '
      With Me.txtErrorIndex
         .Location = New System.Drawing.Point(60, 214)
         .Name = "txtErrorIndex"
         .Size = New System.Drawing.Size(40, 22)
         .TabIndex = 14
         .TextAlign = System.Windows.Forms.HorizontalAlignment.Right
         .Visible = False
      End With
      '
      'lblTopoName
      '
      With Me.lblTopoName
         .Location = New System.Drawing.Point(126, 78)
         .Name = "lblTopoName"
         .Size = New System.Drawing.Size(120, 18)
         .Text = tMapThemeData.TopoName
         .TabIndex = 13
         .Visible = False
      End With
      '
      'lblTopoNameCap
      '
      With Me.lblTopoNameCap
         .Location = New System.Drawing.Point(28, 78)
         .Name = "lblTopoNameCap"
         .Size = New System.Drawing.Size(96, 18)
         .TabIndex = 12
         .Text = "MPolygon Set:"
         .RightToLeft = Windows.Forms.RightToLeft.No
         .Visible = False
      End With
      If False Then
         '
         'tstTopology
         '
         With Me.tstTopology
            .Location = New System.Drawing.Point(0, 0)
            .Name = "tstTopology"
            .Size = New System.Drawing.Size(300, 25)
            .TabIndex = 11
         End With
      End If

      '
      'lblTopoErrors
      '
      With Me.lblTopoErrors
         .Location = New System.Drawing.Point(8, 214)
         .Name = "lblTopoErrors"
         .Size = New System.Drawing.Size(50, 18)
         .TabIndex = 9
         .Text = "Errors:"
         .Visible = False
      End With
      '
      'grbCentroids
      '
      With Me.grbCentroids(iIndex)
         .Controls.Add(Me.txtCentroidCount(iIndex))
         .Controls.Add(Me.txtCentroidBlocks)
         .Controls.Add(Me.txtCentroidLayers)
         .Controls.Add(Me.lblCentroidBlocks(iIndex))
         .Controls.Add(Me.lblCentroidLayers(iIndex))

         '.Location = New System.Drawing.Point(8, 36 + 22)
         '.Name = "grbLinks"
         '.Size = New System.Drawing.Size(284, 43 + 24)



         .Location = New System.Drawing.Point(8, 36 + 68 + 24)

         .Name = "grbCentroids"
         .Size = New System.Drawing.Size(284, 64)
         .TabIndex = 8
         .TabStop = False
         .Text = "Centroids"
      End With
      '
      'txtCentroidCount
      '
      With Me.txtCentroidCount(iIndex)
         .Location = New System.Drawing.Point(238, 15)
         .Name = "txtCentroidCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(40, 22)
         .TabIndex = 19
      End With
      '
      'txtCentroidBlocks
      '
      With Me.txtCentroidBlocks
         .Location = New System.Drawing.Point(99, 39)
         .Name = "txtCentroidBlocks"
         .ReadOnly = True
         .Size = New System.Drawing.Size(135, 22)
         .Text = tMapThemeData.CentroidBlocks
         .TabIndex = 18
      End With
      '
      'txtCentroidLayers
      '
      With Me.txtCentroidLayers
         .Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
         .Location = New System.Drawing.Point(66, 15)
         .Name = "txtCentroidLayers"
         .ReadOnly = True
         .Size = New System.Drawing.Size(168, 22)
         .Text = tMapThemeData.CentroidLayers
         .TabIndex = 17
      End With
      '
      'lblCentroidBlocks
      '
      With Me.lblCentroidBlocks(iIndex)
         .Location = New System.Drawing.Point(20, 42)
         .Name = "lblCentroidBlocks"
         .Size = New System.Drawing.Size(78, 18)
         .TabIndex = 7
         .Text = "Block names:"
      End With
      '
      'lblCentroidLayers
      '
      With Me.lblCentroidLayers(iIndex)
         .Location = New System.Drawing.Point(20, 18)
         .Name = "lblCentroidLayers"
         .Size = New System.Drawing.Size(45, 18)
         .TabIndex = 6
         .Text = "Layers:"
      End With
      '
      'grbLinks
      '
      With Me.grbLinks(iIndex)
         .Controls.Add(Me.txtLinkCount(iIndex))
         .Controls.Add(Me.txtLinkLayers(iIndex))
         .Controls.Add(Me.lblLinkLayers(iIndex))
         .Controls.Add(Me.lblClosedPolylineCount(iIndex))
         .Controls.Add(Me.txtClosedPolylineCount(iIndex))

         .Location = New System.Drawing.Point(8, 36 + 22)
         .Name = "grbLinks"
         .Size = New System.Drawing.Size(284, 43 + 24)
         .TabIndex = 7
         .TabStop = False
         .Text = "Polygons"
      End With
      '
      'txtLinkCount
      '
      With Me.txtLinkCount(iIndex)
         .Location = New System.Drawing.Point(238, 15)
         .Name = "txtLinkCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(40, 22)
         .TabIndex = 17
      End With
      '
      'txtClosedPolylineCount
      '
      With Me.txtClosedPolylineCount(iIndex)
         .Location = New System.Drawing.Point(238, 15 + 24)
         .Name = "txtClosedPolylisCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(40, 22)
         .TabIndex = 17
      End With

      '
      'txtLinkLayers
      '
      With Me.txtLinkLayers(iIndex)
         .Location = New System.Drawing.Point(66, 15)
         .Name = "txtLinkLayers"
         .ReadOnly = True
         .Size = New System.Drawing.Size(168, 22)
         'MessageBox.Show(tMapThemeData.LinkLayers, "04_347")
         .Text = tMapThemeData.ClosedPgonsLayer
         .TabIndex = 16
      End With
      '+*LoopInd.Count
      'lblLinkLayers
      '
      With Me.lblLinkLayers(iIndex)
         .Location = New System.Drawing.Point(20, 18)
         .Name = "lblLinkLayers"
         .Size = New System.Drawing.Size(45, 18)
         .TabIndex = 6
         .Text = "Layers:"
      End With
      ' 
      'lblClosedPolylineCount
      '
      With Me.lblClosedPolylineCount(iIndex)
         .Location = New System.Drawing.Point(20, 18 + 24)
         .Name = "lblClosedPolylineCount"
         .Size = New System.Drawing.Size(92, 18)
         .TabIndex = 6
         .Text = "Polygon count:"
      End With

      ''''''''''''''''''''''''''''''''''''''''' QQQ
      '
      'grbWorkArea
      '
      With Me.grbWorkArea(iIndex)
         .Controls.Add(Me.txtWorkAreaLinkCount(iIndex))
         .Controls.Add(Me.txtWorkAreaLayers(iIndex))
         .Controls.Add(Me.txtWorkAreaWarning(iIndex))
         .Controls.Add(Me.lblWorkAreaLinkLayers(iIndex))

         '.Controls.Add(Me.lblClosedPolylineCount(iIndex))
         '.Controls.Add(Me.txtClosedPolylineCount(iIndex))


         '.Location = New System.Drawing.Point(8, 36 + 22)
         '.Name = "grbLinks"
         '.Size = New System.Drawing.Size(284, 43 + 24)

         '.Location = New System.Drawing.Point(8, 36 + 68 + 24)
         '.Name = "grbCentroids"
         '.Size = New System.Drawing.Size(284, 64)

         .Location = New System.Drawing.Point(8, 104 + 64 + 24)
         .Name = "grbWorkArea"
         .Size = New System.Drawing.Size(284, 47)
         .TabIndex = 7
         .TabStop = False
         .Text = "Work Area"
      End With

      '
      'txtWorkAreaLayers
      '
      With Me.txtWorkAreaLayers(iIndex)
         .Location = New System.Drawing.Point(66, 15)
         .Name = "txtWorkAreaLayers"
         .ReadOnly = True
         .Size = New System.Drawing.Size(168, 22)
         'MessageBox.Show(tMapThemeData.LinkLayers, "04_347")
         .Text = DMAcadExt.MapThemeData.WorkAreaBoundaryLayer

         .TabIndex = 24
      End With
      '
      'txtWorkAreaLinkCount
      '
      With Me.txtWorkAreaLinkCount(iIndex)
         .Location = New System.Drawing.Point(238, 15)

         .Name = "txtWorkAreaLinkCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(40, 22)
         ' .BackColor = Color.LightCyan
         'MessageBox.Show(tMapThemeData.LinkLayers, "04_347")
         .Text = DMAcadExt.MapThemeData.WorkAreaBoundaryLayer

         .TabIndex = 24
      End With
      '
      'txtWorkAreaWarning
      '
      With Me.txtWorkAreaWarning(iIndex)
         .Location = New System.Drawing.Point(2, 15)

         .Name = "txtWorkAreaWarning"
         .ReadOnly = True
         .Size = New System.Drawing.Size(0, 22)
         .BackColor = Color.Red
         'MessageBox.Show(tMapThemeData.LinkLayers, "04_347")


         .TabIndex = 24
      End With



      ' 
      'lblWorkAreaLinkLayers
      '
      With Me.lblWorkAreaLinkLayers(iIndex)
         .Location = New System.Drawing.Point(20, 18)
         .Name = "lblWorkAreaLinkLayers"
         .Size = New System.Drawing.Size(45, 18)
         .TabIndex = 6
         .Text = "Layers:"
      End With


      '
      'txtTolerance
      '
      With Me.txtTolerance
         .Location = New System.Drawing.Point(240, 32)
         .Name = "txtTolerance"
         .Size = New System.Drawing.Size(52, 22)
         .TabIndex = 3
         .Text = "0.01"
         .Visible = False
      End With
      '
      'lblTolerance
      '
      With Me.lblTolerance
         .Location = New System.Drawing.Point(170, 32)
         .Name = "lblTolerance"
         .Size = New System.Drawing.Size(68, 18)
         .TabIndex = 2
         .Text = "Tolerance"
         .Visible = False
      End With
      '
      'chkHighlightSliver
      '
      With Me.chkHighlightSliver
         .AutoSize = True
         .Location = New System.Drawing.Point(8, 52)
         .Name = "chkHighlightSliver"
         .Size = New System.Drawing.Size(105, 18)
         .TabIndex = 1
         .Text = "Highlight Sliver"
         .UseVisualStyleBackColor = True
         .RightToLeft = Windows.Forms.RightToLeft.No
         .Visible = False
      End With
      '
      'chkCreateCentroid
      '

      With Me.chkCreateCentroid
         .AutoSize = True
         .Location = New System.Drawing.Point(8, 32)
         .Name = "chkCreateCentroid"
         .Size = New System.Drawing.Size(108, 18)
         .TabIndex = 0
         .Text = "Insert Centroid"
         .UseVisualStyleBackColor = True
         .RightToLeft = Windows.Forms.RightToLeft.No
         .Visible = False
      End With

   End Sub

   'tTopoRes = TopoManager.TopoCreator.CheckMPgons(oMapThemeData.MPgonLayers)


   Private Sub zzDispTopoOK(iIndex As Integer)
      Dim bOK As Boolean = mtTopoRes(iIndex).IsOK
      Me.doaLabelCheck(0).Checked = bOK
      Me.lblTopoExists.Visible = bOK

      If bOK Then
         Me.lblTopoName.ForeColor = System.Drawing.SystemColors.ControlText
         Me.lblTopoName.Font = doLabelBoldFont
         '  054 810 8770 YOEL BOROVSKI  054 2204750  ILAN
         '  054 586 4250
      Else
         Me.lblTopoName.ForeColor = System.Drawing.SystemColors.GrayText
         Me.lblTopoName.Font = doLabelFont
      End If
      miaPgonCount(iIndex) = mtTopoRes(iIndex).PgonCount

      Me.txtLinkCount(iIndex).Text = Convert.ToString(miaPgonCount(iIndex))
      '		MessageBox.Show(Me.txtLinkCount(iIndex).Text & bOK & ":" & CStr(miPgonCount), "3_322")
      If mtTopoRes(iIndex).HasElements Then
         Me.txtLinkCount(iIndex).Text = CStr(mtTopoRes(iIndex).LinkCount)
         Me.txtCentroidCount(iIndex).Text = CStr(mtTopoRes(iIndex).CentroidCount)
      End If

   End Sub
   Private Sub zzCheckCentroidsAAA()
      '	ptMapThemeData()
      mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptMapThemeData.CentroidBlocks, ptMapThemeData.CentroidLayers)


   End Sub
   Private Sub zzDispCentroids()
      '	ptMapThemeData()

      If mcolCentroids IsNot Nothing Then
         'MessageBox.Show(ptMapThemeData.CentroidBlocks & ":" & ptMapThemeData.CentroidLayers & vbCrLf & CStr(mcolCentroids.Count), "02_980")
         Me.txtCentroidCount(0).Text = Convert.ToString(mcolCentroids.Count + 1000)
      Else
         '	MessageBox.Show(ptMapThemeData.CentroidBlocks & ":" & ptMapThemeData.CentroidLayers & vbCrLf & CStr("NNN"), "02_982")
      End If

   End Sub
   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tDissolveMapThemeData As DMAcadExt.MapThemeData)
      MyBase.New(tMapThemeData)
      miThisStagesUB = 1
      '    MessageBox.Show(tMapThemeData.TopoName & ":" & tDissolveMapThemeData.TopoName, "04_100")
      ptDissolveMapThemeData = tDissolveMapThemeData
      ' This call is required by the designer.
      InitializeComponent()
      '	MessageBox.Show(ptMapThemeData.CentroidBlocks & ":" & ptMapThemeData.CentroidLayers & ":" & ptMapThemeData.MapThemeID.ToString() & vbCrLf & tDissolveMapThemeData.CentroidBlocks & ":" & tDissolveMapThemeData.CentroidLayers & ":" & tDissolveMapThemeData.MapThemeID.ToString(), "02_943d")
      ' Add any initialization after the InitializeComponent() call.
      '	zzCheckMPolygonSet()

   End Sub
   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
      MyBase.New(tMapThemeData)
      miThisStagesUB = 0
		DMCommon.Debug.MsgBox("04_102", tMapThemeData.TopoName, tMapThemeData.GraphType)

		' This call is required by the designer.
		InitializeComponent()
      '	MessageBox.Show(ptMapThemeData.CentroidBlocks & ":" & ptMapThemeData.CentroidLayers & ":" & ptMapThemeData.MapThemeID.ToString() & vbCrLf & tDissolveMapThemeData.CentroidBlocks & ":" & tDissolveMapThemeData.CentroidLayers & ":" & tDissolveMapThemeData.MapThemeID.ToString(), "02_943d")
      ' Add any initialization after the InitializeComponent() call.
      '	zzCheckMPolygonSet()

   End Sub

   Private Sub tstClosedPgons_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstClosedPgons.ItemClicked

      Dim s As String = ""
      Dim ss As String = ""
      If tstClosedPgons IsNot Nothing Then
         s = tstClosedPgons.Location.ToString() & vbCrLf
         For i As Integer = 0 To tstClosedPgons.Items.Count - 1
            s &= tstClosedPgons.Items(i).Name & ","
         Next
      Else
         s = "NotClosedPgons"
      End If
      If tstTopology IsNot Nothing Then
         ss = tstTopology.Location.ToString() & vbCrLf
         For i As Integer = 0 To tstTopology.Items.Count - 1
            ss &= tstTopology.Items(i).Name & ","
         Next
      Else
         ss = "NotTopology"
      End If


      '  

      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbImportShapes.Name
            '   MessageBox.Show(e.ClickedItem.Name & vbCrLf & ptMapThemeData.MapThemeID.ToString(), "02_459")
            Select Case ptMapThemeData.MapThemeID
               Case DMAcadExt.enMapTheme.Parcels
                  zzOpenParcelsForm()
               Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
                  zzOpenLotsForm()
               Case DMAcadExt.enMapTheme.PlanApproved
                  zzOpenLotsForm()
            End Select




         Case tsbDissolveShapes.Name
            zzDissolve()
            zzGetStatistics(True, False)
         Case tsbDissolveAllArea.Name

            zzCreateWorkArea()
            zzGetStatistics(False, True)
         Case Me.tsbClearMap.Name
            zzClearMap()

         Case Me.tsbA.Name
            ' zzCreateBlockPgonSet()
            '	zzCreatePgonSet_290614()
            Select Case LabelCheck.CurrentIndex
               Case 0
                  MessageBox.Show(ptMapThemeData.MapThemeID.ToString())
                  zzCreatePgonSet(ptMapThemeData)
                  zzOpenCheckPgons(ptMapThemeData, False)



               Case 1
                  MessageBox.Show(ptDissolveMapThemeData.MapThemeID.ToString())
                  '  zzCreateBlockPgonSet()
                  zzCreatePgonSet(ptDissolveMapThemeData)
                  zzOpenCheckPgons(ptDissolveMapThemeData, True)
            End Select
            'zzOpenCheckPgons(
         Case Me.ssbCreatePgonset.Name
            ' zzCreateBlockPgonSet()
            '	zzCreatePgonSet_290614()
            '   MessageBox.Show(ptMapThemeData.MapThemeID.ToString() & vbCrLf & ptDissolveMapThemeData.MapThemeID.ToString(), "10_300")
            Select Case LabelCheck.CurrentIndex
               Case 0
                  zzCreatePgonSet(ptMapThemeData)
                  zzOpenCheckPgons(ptMapThemeData, False)
               Case 1
                  ' zzCreateBlockPgonSet()
                  zzCreatePgonSet(ptDissolveMapThemeData)
                  zzOpenCheckPgons(ptDissolveMapThemeData, True)
            End Select

         Case Me.tsbB.Name
            zzCreateTopoLinks()
         Case Me.tsbExportShapes.Name
            zzGroupDissolve()
         Case Me.tsbPgonView.Name

            FDO.TplnPolygonSet.PgonView()

         Case Me.tsbExec.Name

         Case Me.tsbGetStatistics.Name
            FDO.TplnPolygonSet.PgonView()
         Case Me.tsbExecA.Name


         Case Me.tsbClear.Name


      End Select
      Me.Cursor = Cursors.Default
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub ssbCreatePgonset_DropDownItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ssbCreatePgonset.DropDownItemClicked
      'MessageBox.Show(e.ClickedItem.Name, "07_341")
      Select Case e.ClickedItem.Name
         Case Me.tsiFormErr.Name
            Select Case LabelCheck.CurrentIndex
               Case 0
                  zzOpenCheckPgons(ptMapThemeData, False)
               Case 1
                  zzOpenCheckPgons(ptDissolveMapThemeData, True)
            End Select
         Case Me.tsiPgonSetReset.Name
            Select Case LabelCheck.CurrentIndex
               Case 0
                  moPgonSet = Nothing
               Case 1

            End Select



      End Select
   End Sub

   Private Sub zzDissolve()
		'	moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)

		'DMCommon.Debug.MsgBox("05_341aa", ptDissolveMapThemeData.CentroidBlock, ptDissolveMapThemeData.CentroidLayer, ptDissolveMapThemeData.LinkLayer, ptDissolveMapThemeData.ClosedPgonsLayer)
		If moFDO_Manager IsNot Nothing Then
         moFDO_Manager.CreateBlocksUnion(ptDissolveMapThemeData.CentroidBlock, ptDissolveMapThemeData.CentroidLayer)
      Else
         MessageBox.Show("FDO_Manager was not found", "01_277")
      End If

   End Sub
   Private Sub zzCreateWorkArea()
      '	moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
      'MessageBox.Show(ptDissolveMapThemeData.CentroidBlock & vbCrLf & ptDissolveMapThemeData.CentroidLayer, "05_341")
      If moFDO_Manager IsNot Nothing Then
         moFDO_Manager.CreateWorkArea()
      End If

   End Sub


   Private Sub zzClearMap()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		FDO.FDO_Manager.RemoveAllConnections()
      FDO.Util.ClearAllResources()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.CommandLine(True)
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
      '	MessageBox.Show(e.ClickedItem.Name & vbCrLf, "02_466d")
      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbCreateTopo.Name

            '	
            zzCreateTopo(LabelCheck.CurrentIndex)
         Case Me.tsbCheckTopo.Name
            '	MessageBox.Show(e.ClickedItem.Name, "02_341a")
            zzCheckTopo(True, True)
         Case Me.tsbDeleteTopo.Name

            zzDeleteTopo(LabelCheck.CurrentIndex, False)
         Case Me.tsbExec.Name
            MessageBox.Show(e.ClickedItem.Name & vbCrLf & ptMapThemeData.MapThemeID.ToString(), "02_711")
            zzOpenParcelsForm()
         Case Me.tsbGetStatistics.Name
            'MessageBox.Show("Case Me.tsbGetStatistics.Name", "02_340")
            FDO.TplnPolygonSet.PgonView()
         Case Me.tsbExecA.Name


         Case Me.tsbClear.Name


      End Select
      Me.Cursor = Cursors.Default
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub zzCreateTopo(ByVal iIndex As Integer)
      Dim dTolerance As Double
      Dim bCreateCentroids As Boolean = False


      Me.Cursor = Cursors.WaitCursor
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)


      dTolerance = 0.01
      System.Windows.Forms.MessageBox.Show(ptMapThemeData.TopoName & vbCrLf & ptMapThemeData.LinkLayers, "frmClosedPgons - zzCreateTopo_01")
      Try
         TopoManager.TopoCreator.CreateTopology(ptMapThemeData.TopoName, ptMapThemeData.MapThemeID, ptMapThemeData.LinkLayers, "", ptMapThemeData.CentroidBlock, ptMapThemeData.CentroidLayer, True, String.Empty, String.Empty, False, False, dTolerance)


      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmClosedPgons - zzCreateTopo_01")
      End Try


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
      Me.Cursor = Cursors.Default


   End Sub
   Private Sub zzDeleteTopo(ByVal iIndex As Integer, ByVal bDeleteEntities As Boolean)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      TopoManager.TopoCreator.DeleteTopology(ptMapThemeData.TopoName, bDeleteEntities, False)
      zzCheckTopo(False, False)
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzCheckTopo(bLockDoc As Boolean, ByVal bMsgBox As Boolean)
      mtTopoRes(0) = TopoManager.TopoCreator.CheckTopo(ptMapThemeData.TopoName, bLockDoc, bMsgBox, ptMapThemeData.MapThemeID)
   End Sub

   Private Sub zzGroupDissolve()
      If moPgonSet IsNot Nothing Then
         '  MessageBox.Show(Me.Name & vbCrLf & "zzGroupDissolve")
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

         moFDO_Manager = New FDO.FDO_Manager()
         Dim saShapeFileNames() As FDO.FDO_Manager.ShapeFOData = moPgonSet.ExportByGroup
         'moPgonSet.PrintLinkTable()	'

         moFDO_Manager.FOData = saShapeFileNames
         moFDO_Manager.CreateBoundingBoxesMapLayer()
         moFDO_Manager.ConnectAddToMap()
         '	CreateDissolveMapLayer(mcolDisPlines, "WorkArea", "shpWorkArea")
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()
      Else

         MessageBox.Show("Polygon Set was not found", "DMT-Planner")
      End If

   End Sub
   Private Sub zzCreateBlockPgonSet()
      '  MessageBox.Show(Me.Name & vbCrLf & "zzCreateBlockPgonSet" & vbCrLf & ptDissolveMapThemeData.MapThemeID.ToString() & vbCrLf & ptDissolveMapThemeData.TopoPurpose & vbCrLf & ptDissolveMapThemeData.MapThemeName & vbCrLf & ptDissolveMapThemeData.TopoName & vbCrLf & ptDissolveMapThemeData.ClosedPgonsLayers & vbCrLf & ptDissolveMapThemeData.LinkLayer, "05_360")
      Dim bCurrentLayerOK As Boolean = False
      moPgonSet = New FDO.TplnPolygonSet(ptDissolveMapThemeData.MapThemeID, ptDissolveMapThemeData.TopoPurpose, ptDissolveMapThemeData.TopoName)
      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oEnt As DBObject = Nothing

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
      System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.DMApp.AppID), "pseudo")
      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      'System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadDocument.IsLocked & vbCrLf & "", "07_001")
      Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(ptDissolveMapThemeData.ClosedPgonsLayers)

      Dim dicCentroids As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(ptMapThemeData.CentroidBlock, OpenMode.ForWrite)   '"pclp004"

      mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(ptDissolveMapThemeData.CentroidBlocks, ptDissolveMapThemeData.CentroidLayers)
      DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count))
      moPgonSet.AddPolylineIDs(colPolygonIDs)
      '?	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("TplnParcelMPgon", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      '
      'oPgonSet.AddBlocks(dicCentroids)
      ' System.Windows.Forms.MessageBox.Show(ptDissolveMapThemeData.CentroidBlocks & ":" & ptDissolveMapThemeData.CentroidLayers & vbCrLf & CStr(mcolCentroids.Count), "04_673")
      moPgonSet.AddCentroids(mcolCentroids)
      '
      TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
      TopoManager.TPlanGraph.TplnLot.Initialize(ptMapThemeData)
      '
      '''''''''''''''''''''''''moPgonSet()

      Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(ptDissolveMapThemeData.ClosedPgonsLayers)
      '   System.Windows.Forms.MessageBox.Show(ptDissolveMapThemeData.ClosedPgonsLayers & vbCrLf & CStr(colPolylines.Count), "07_016")
      If colPolylines IsNot Nothing AndAlso colPolylines.Count > 0 Then
         moWorkAreaBound = DMAcadExt.AcadTransaction.GetPolyline(colPolylines.Item(0), OpenMode.ForRead)

      End If
      moPgonSet.WorkAreaBound = moWorkAreaBound


      '''''''''''''	
      moPgonSet.CalculateNewF(True)
      '    MessageBox.Show(moPgonSet.C
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzCreatePgonSet(tMapThemeData As DMAcadExt.MapThemeData)
      If False AndAlso mfrmPgonSetView IsNot Nothing AndAlso Not mfrmPgonSetView.IsDisposed Then
         moPgonSet = mfrmPgonSetView.PgonSet
      End If

      If moPgonSet Is Nothing OrElse moPgonSet.SetMapTheme <> tMapThemeData.MapThemeID Then
			' MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370a")
			Dim bCurrentLayerOK As Boolean = False
			'   System.Windows.Forms.MessageBox.Show(ptMapThemeData.MapThemeID.ToString & vbCrLf & ptMapThemeData.TopoPurpose.ToString & vbCrLf & ptMapThemeData.TopoName & vbCrLf & ptMapThemeData.ClosedPgonsLayers, "07_001w")
			moPgonSet = New FDO.TplnPolygonSet(tMapThemeData.MapThemeID, tMapThemeData.TopoPurpose, tMapThemeData.TopoName)
         '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
         Dim oEnt As DBObject = Nothing

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
         ' System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.DMApp.AppID), "pseudo")
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
         'System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadDocument.IsLocked & vbCrLf & "", "07_001")
         Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(tMapThemeData.ClosedPgonsLayers)
         '
         Dim dicCentroids As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(tMapThemeData.CentroidBlock, OpenMode.ForWrite)   '"pclp004"
         '
         mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(tMapThemeData.CentroidBlocks, tMapThemeData.CentroidLayers)
         DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count))
         moPgonSet.AddPolylineIDs(colPolygonIDs)
         '?	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("TplnParcelMPgon", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
         '
         'oPgonSet.AddBlocks(dicCentroids)
         moPgonSet.AddCentroids(mcolCentroids)
         '   System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString(), "10_120")
         Select Case tMapThemeData.MapThemeID
            Case DMAcadExt.enMapTheme.Blocks, DMAcadExt.enMapTheme.UD_Blocks
               ' System.Windows.Forms.MessageBox.Show(tMapThemeData.CentroidBlock, "10_121")
               TopoManager.TPlanGraph.TplnBlock.Initialize(tMapThemeData)
               ' System.Windows.Forms.MessageBox.Show(tMapThemeData.CentroidBlock.ToString(), "10_122")
            Case DMAcadExt.enMapTheme.Parcels
               TopoManager.TPlanGraph.TplnParcel.Initialize(tMapThemeData)
            Case DMAcadExt.enMapTheme.UD_Parcels
               UnidivNet.UD_Parcel.Initialize(tMapThemeData)
            Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
               TopoManager.TPlanGraph.TplnLot.Initialize(tMapThemeData)
         End Select

         '
         'TplnBlock.Initialize(tMapThemeData)
         'TopoManager.TPlanGraph.TplnParcel.Initialize(tMapThemeData)
         'TopoManager.TPlanGraph.TplnLot.Initialize(tMapThemeData)
         'UnidivNet.UD_Parcel.Initialize(tMapThemeData)


         '
         '''''''''''''''''''''''''moPgonSet()
         Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)
         If colPolylines IsNot Nothing AndAlso colPolylines.Count > 0 Then
            moWorkAreaBound = DMAcadExt.AcadTransaction.GetPolyline(colPolylines.Item(0), OpenMode.ForRead)

         End If
         moPgonSet.WorkAreaBound = moWorkAreaBound


         '''''''''''''	
         moPgonSet.CalculateNewF(True)
         '    DMCommon.ExcelLog.Open()
         moPgonSet.InfoToExcel()
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()

      End If
   End Sub

   Private Sub zzCreateTopoLinks()
      If moPgonSet IsNot Nothing Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
         Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer("pclp004", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
         If bCurrentLayerOK Then
            'moPgonSet.CreateTopoLinks()
            moPgonSet.DrawEdges()
            '	frmClosedPgons.vb:line 842
         End If
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If
   End Sub



   Private Sub mfParcelsFromShapes_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfParcelsFromShapes.FormClosed
		DMCommon.Debug.MsgBox("12_901K", mfParcelsFromShapes.DialogResult, mfParcelsFromShapes.Folder)

		If mfParcelsFromShapes.DialogResult = Windows.Forms.DialogResult.OK AndAlso mfParcelsFromShapes.PoligonCount <> -1 Then


			''''''	DMCommon.Debug.MsgBox("03_220", mfParcelsFromShapes.DialogResult, mfParcelsFromShapes.PoligonCount, mfParcelsFromShapes.Folder, mfParcelsFromShapes.Parcel)
			If mfParcelsFromShapes.Folder Then
            msaParcelFolders = mfParcelsFromShapes.ParcelFolders
            msCurrentRootName = mfParcelsFromShapes.CurrentRootName

            '  DMCommon.Functions.DispArray(msaParcelFolders, "msaParcelFolders", True)
            moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
            moFDO_Manager.imp_Parcels() '!!!!!


            miPoligonCount += moFDO_Manager.PoligonCount
            '  DMCommon.Debug.MsgBox("03_780", (moFDO_Manager Is Nothing), miPoligonCount)
            ' add check
            '	zzCheckMPolygonSet()
            '	zzDispTopoOK(0)
            zzDispCentroids()
         Else
            Dim oFileInfo As IO.FileInfo
            If mfParcelsFromShapes.Parcel Then
               oFileInfo = New IO.FileInfo(mfParcelsFromShapes.CurrentParcelFileName)
            Else
               oFileInfo = New IO.FileInfo(mfParcelsFromShapes.CurrentGushFileName)
            End If


            Dim sShapeFileName As String, sShapeFolderName As String, sFileName As String
            If oFileInfo.Exists Then
               sShapeFileName = oFileInfo.FullName
               sShapeFolderName = oFileInfo.DirectoryName
               sFileName = oFileInfo.Name
               Dim sClassName As String = zzGetClassName(sFileName, oFileInfo.Extension)

               moFDO_Manager = New FDO.FDO_Manager()
					'	DMCommon.Debug.MsgBox("07_030D!!! OK", sShapeFolderName, sShapeFileName, DMCommon.Debug.ColCount(mfParcelsFromShapes.GushSet), DMCommon.Debug.ColCount(mfParcelsFromShapes.GushFromParcelData), sClassName, mfParcelsFromShapes.MapThemeID, mfParcelsFromShapes.FilterList.List)
					'	Dim oaTest(mfParcelsFromShapes.GushFromParcelData.Count - 1) As System.Object


					DMCommon.Debug.MsgBox("12_901b", mfParcelsFromShapes.GushSet.Count)
					moFDO_Manager.imp_Set(sShapeFileName, sShapeFolderName, sClassName, mfParcelsFromShapes.GushSet, mfParcelsFromShapes.GushFromParcelData, mfParcelsFromShapes.MapThemeID, mfParcelsFromShapes.FilterList, mfParcelsFromShapes.InnerPolygons)
					DMCommon.Debug.MsgBox("12_901a")
				End If
         End If
      End If
      zzGetStatistics(True, False)
      Me.Visible = True
   End Sub
   Private Sub mfLotsFromShapes_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfLotsFromShapes.FormClosed
      If mfLotsFromShapes.DialogResult = Windows.Forms.DialogResult.OK AndAlso mfLotsFromShapes.PoligonCount <> -1 Then
         Dim sShapeFileName As String, sShapeFolderName As String, sFileName As String
         Dim oFileInfo As IO.FileInfo
         oFileInfo = New IO.FileInfo(mfLotsFromShapes.CurrentLotsFileName)
         If oFileInfo.Exists Then
            sShapeFileName = oFileInfo.FullName
            sShapeFolderName = oFileInfo.DirectoryName
            sFileName = oFileInfo.Name
            Dim sClassName As String = zzGetClassName(sFileName, oFileInfo.Extension)
            moFDO_Manager = New FDO.FDO_Manager()
				'	MessageBox.Show(CStr(mbParcel) & vbCrLf & sShapeFolderName & vbCrLf & sShapeFileName & vbCrLf & sClassName & vbCrLf & CStr(setGush.Count), "07_030")
				moFDO_Manager.imp_Set(sShapeFileName, sShapeFolderName, sClassName, Nothing, Nothing, ptMapThemeData.MapThemeID, New DMCommon.dmList(), True) 'DMAcadExt.enMapTheme.LotApproved,

			End If
      End If

      zzGetStatistics(True, False)
      Me.Visible = True

   End Sub

   Private Function zzGetClassName(ByVal sFileName As String, ByVal sFileExtension As String) As String
      Return sFileName.Substring(0, sFileName.Length - sFileExtension.Length)
   End Function

   Private Sub zzOpenParcelsForm()
      mfParcelsFromShapes = New frmParcelsFromShapes(ptMapThemeData, ptDissolveMapThemeData)
      Try
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfParcelsFromShapes)
         '	MessageBox.Show(CStr(moFDO_Manager Is Nothing) & vbCrLf & CStr(miPoligonCount), "03_782")
         Me.Visible = False
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmClosedPgons - zzOpenParcelForm")
      End Try
   End Sub

   Private Sub zzOpenLotsForm()
      System.Windows.Forms.MessageBox.Show(ptMapThemeData.MapThemeID.ToString & vbCrLf & ptDissolveMapThemeData.MapThemeID.ToString, "12_frmClosedPgons - zzOpenParcelForm")
      mfLotsFromShapes = New frmLotsFromShapes(ptMapThemeData, ptDissolveMapThemeData)
      Try
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfLotsFromShapes)
         '	MessageBox.Show(CStr(moFDO_Manager Is Nothing) & vbCrLf & CStr(miPoligonCount), "03_782")
         Me.Visible = False
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmClosedPgons - zzOpenLotsForm")
      End Try
   End Sub
   Private Sub zzGetMapThemeData(iIndex As Integer, ByRef tMapThemeData As DMAcadExt.MapThemeData)
      Select Case iIndex
         Case 0
            tMapThemeData = ptMapThemeData
         Case 1
            tMapThemeData = ptDissolveMapThemeData
      End Select
   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub

   Private Sub mfrmPgonSetView_FormClosed(ByVal oSender As System.Object, e As FormClosedEventArgs) Handles mfrmPgonSetView.FormClosed
      Me.Visible = True
   End Sub
 
  
End Class