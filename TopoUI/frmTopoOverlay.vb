Public Class frmTopoOverlay
   Const miThisStagesUB As Integer = 1
   Private mbIsDone As Boolean
#Region "Panel0_Declarations"
   Private lblSrcPgonCount As System.Windows.Forms.Label
   Private lblSrcTopoName As System.Windows.Forms.Label
   Private txtSrcPgonCount As System.Windows.Forms.TextBox
   Private lblSrcTopoExists As System.Windows.Forms.Label
   Private lblSrcTopoNameCap As System.Windows.Forms.Label

   Private lblOvlPgonCount As System.Windows.Forms.Label
   Private lblOvlTopoName As System.Windows.Forms.Label
   Private txtOvlPgonCount As System.Windows.Forms.TextBox
   Private lblOvlTopoExists As System.Windows.Forms.Label
   Private lblOvlTopoNameCap As System.Windows.Forms.Label

   Private lblResPgonCount As System.Windows.Forms.Label
   Private lblResTopoName As System.Windows.Forms.Label
   Private txtResPgonCount As System.Windows.Forms.TextBox
   Private lblResTopoExists As System.Windows.Forms.Label
   Private lblResTopoNameCap As System.Windows.Forms.Label

   Private lblRes1PgonCount As System.Windows.Forms.Label
   Private lblRes1TopoName As System.Windows.Forms.Label
   Private txtRes1PgonCount As System.Windows.Forms.TextBox
   Private lblRes1TopoExists As System.Windows.Forms.Label
   Private lblRes1TopoNameCap As System.Windows.Forms.Label

#End Region
#Region "Panel1_Declarations"

#End Region
   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tSourceMapThemeData As DMAcadExt.MapThemeData, tOverlayMapThemeData As DMAcadExt.MapThemeData)
      MyBase.New(tMapThemeData)
      ptSourceMapThemeData = tSourceMapThemeData
      ptOverlayMapThemeData = tOverlayMapThemeData
      ' This call is required by the designer.

      MyBase.SetLabelDim(miThisStagesUB)
      ' Add any initialization after the InitializeComponent() call.
      InitializeComponent()
      zzCheckTopo(2, True, False, False)
      Me.Text = "Datamap!!!"
   End Sub
   Private Sub zzMyInitialize()
      For iIndex As Integer = 0 To 2

         zzCheckTopo(iIndex, True, True, False)

      Next
      AddHandler Me.doaPanels(0).Paint, AddressOf zzPaintPanel
      '   MessageBox.Show(Me.lblOvlTopoName.Font.ToString())
   End Sub
   Private Sub zzPaintPanel(oSender As System.Object, e As System.Windows.Forms.PaintEventArgs)

      Dim oGraphics As Graphics = e.Graphics
      Dim oPen As Pen = New Pen(Color.BlueViolet, 2.0!)
      Dim tPoint1 As Point = New Point(20, 175)
      Dim tPoint2 As Point = New Point(280, 175)

      oGraphics.DrawLine(oPen, tPoint1, tPoint2)
   End Sub
   Private Sub zzInitPanel0()
      Dim iYTop As Integer = 52
      Dim iRowHeight As Integer = 24
      Dim iRowYDist As Integer = 64

      Dim iYTop1 As Integer = iYTop + iRowYDist
      Dim iYTop2 As Integer = iYTop1 + iRowYDist

      Dim iBorderStyle As BorderStyle = BorderStyle.None

      Me.lblSrcPgonCount = New System.Windows.Forms.Label()
      Me.lblSrcTopoExists = New System.Windows.Forms.Label()
      Me.txtSrcPgonCount = New System.Windows.Forms.TextBox()
      Me.lblSrcTopoNameCap = New System.Windows.Forms.Label()
      Me.lblSrcTopoName = New System.Windows.Forms.Label()

      Me.lblOvlPgonCount = New System.Windows.Forms.Label()
      Me.lblOvlTopoExists = New System.Windows.Forms.Label()
      Me.txtOvlPgonCount = New System.Windows.Forms.TextBox()
      Me.lblOvlTopoNameCap = New System.Windows.Forms.Label()
      Me.lblOvlTopoName = New System.Windows.Forms.Label()

      Me.lblResPgonCount = New System.Windows.Forms.Label()
      Me.lblResTopoExists = New System.Windows.Forms.Label()
      Me.txtResPgonCount = New System.Windows.Forms.TextBox()
      Me.lblResTopoNameCap = New System.Windows.Forms.Label()
      Me.lblResTopoName = New System.Windows.Forms.Label()





      '
      'lblSrcTopoExists
      '
      With Me.lblSrcTopoExists
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .Location = New System.Drawing.Point(260, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblSrcTopoExists"
         .Size = New System.Drawing.Size(30, 22)
         .TabIndex = 21
         .ImageAlign = ContentAlignment.MiddleCenter
      End With

      '
      'lblSrcTopoNameCap
      '
      With Me.lblSrcTopoNameCap
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblSrcTopoNameCap"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 20

         .Text = ptSourceMapThemeData.GraphTypeName & ":"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With

      '
      'lblSrcTopoName
      '
      With Me.lblSrcTopoName
         '	.BorderStyle = iBorderStyle
         .Location = New System.Drawing.Point(34, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblSrcTopoName"
         .Size = New System.Drawing.Size(116, 22)
         .TabIndex = 13
         .Text = ptSourceMapThemeData.LineTopoName

         .TextAlign = System.Drawing.ContentAlignment.MiddleRight
         .BorderStyle = iBorderStyle
      End With
      '
      'lblSrcPgonCount
      '
      With Me.lblSrcPgonCount
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblSrcPgonCount"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 24
         .Text = "מס' פוליגונים:"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With


      '
      'txtSrcPgonCount
      '
      With Me.txtSrcPgonCount

         .Location = New System.Drawing.Point(114, iYTop + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4)
         .Name = "txtSrcPgonCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(36, 22)
         .TabIndex = 22
         .BorderStyle = BorderStyle.Fixed3D ' iBorderStyle
      End With

      '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

      '
      'lblOvlTopoExists
      '
      With Me.lblOvlTopoExists
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .Location = New System.Drawing.Point(260, iYTop1)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblOvlTopoExists"
         .Size = New System.Drawing.Size(30, 22)
         .TabIndex = 21
      End With

      '
      'lblOvlTopoNameCap
      '
      With Me.lblOvlTopoNameCap
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop1)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblOvlTopoNameCap"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 20

         .Text = ptOverlayMapThemeData.GraphTypeName & ":"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With

      '
      'lblOvlTopoName
      '
      With Me.lblOvlTopoName
         '	.BorderStyle = iBorderStyle
         .Location = New System.Drawing.Point(34, iYTop1)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblOvlTopoName"
         .Size = New System.Drawing.Size(116, 22)
         .TabIndex = 13
         .Text = ptOverlayMapThemeData.LineTopoName

         .TextAlign = System.Drawing.ContentAlignment.MiddleRight
         .BorderStyle = iBorderStyle
      End With
      '
      'lblOvlPgonCount
      '
      With Me.lblOvlPgonCount
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop1 + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblOvlPgonCount"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 24
         .Text = "מס' פוליגונים:"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With


      '
      'txtOvlPgonCount
      '
      With Me.txtOvlPgonCount

         .Location = New System.Drawing.Point(114, iYTop1 + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4)
         .Name = "txtOvlPgonCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(36, 22)
         .TabIndex = 22
         .BorderStyle = BorderStyle.Fixed3D
      End With


      '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

      '
      'lblResTopoExists
      '
      With Me.lblResTopoExists
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .Location = New System.Drawing.Point(260, iYTop2)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblResTopoExists"
         .Size = New System.Drawing.Size(30, 22)
         .TabIndex = 21
      End With

      '
      'lblResTopoNameCap
      '
      With Me.lblResTopoNameCap
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop2)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblResTopoNameCap"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 20

         .Text = ptMapThemeData.GraphTypeName & ":"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With

      '
      'lblResTopoName
      '
      With Me.lblResTopoName
         '	.BorderStyle = iBorderStyle
         .Location = New System.Drawing.Point(34, iYTop2)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblResTopoName"
         .Size = New System.Drawing.Size(116, 22)
         .TabIndex = 13
         .Text = ptMapThemeData.LineTopoName

         .TextAlign = System.Drawing.ContentAlignment.MiddleRight
         .BorderStyle = iBorderStyle
      End With
      '
      'lblResPgonCount
      '
      With Me.lblResPgonCount
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop2 + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblResPgonCount"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 24
         .Text = "מס' פוליגונים:"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With


      '
      'txtResPgonCount
      '
      With Me.txtResPgonCount

         .Location = New System.Drawing.Point(114, iYTop2 + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4)
         .Name = "txtResPgonCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(36, 22)
         .TabIndex = 22
         .BorderStyle = BorderStyle.Fixed3D
      End With


      '
      'doaPanels(iIndex)
      '
      With doaPanels(0)
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle

         '.Controls.Add(Me.lblMapLayerExists(iIndex))
         '.Controls.Add(Me.lblMapLayerCap(iIndex))
         '.Controls.Add(Me.lblMapLayer(iIndex))
         '.Controls.Add(Me.lblPgonCount(iIndex))
         '.Controls.Add(Me.lblThemeCaption(iIndex))
         .Controls.Add(Me.lblSrcTopoExists)
         .Controls.Add(Me.txtSrcPgonCount)
         .Controls.Add(Me.lblSrcTopoNameCap)
         .Controls.Add(Me.lblSrcTopoName)
         .Controls.Add(Me.lblSrcPgonCount)

         .Controls.Add(Me.lblOvlTopoExists)
         .Controls.Add(Me.txtOvlPgonCount)
         .Controls.Add(Me.lblOvlTopoNameCap)
         .Controls.Add(Me.lblOvlTopoName)
         .Controls.Add(Me.lblOvlPgonCount)


         .Controls.Add(Me.lblResTopoExists)
         .Controls.Add(Me.txtResPgonCount)
         .Controls.Add(Me.lblResTopoNameCap)
         .Controls.Add(Me.lblResTopoName)
         .Controls.Add(Me.lblResPgonCount)

         '	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerExists(iIndex).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerCap(iIndex).RightToLeft.ToString() & vbCrLf & Me.txtPgonCount(iIndex).RightToLeft.ToString(), "02_154b")
         '	.Controls.Add(Me.tstTopology)
         .Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
         .Location = New System.Drawing.Point(0, 0)
         .Margin = New System.Windows.Forms.Padding(4)
         .Size = dtPanelSize

         .Name = "Panel0"
      End With



      Me.Controls.Add(Me.doaPanels(0))
   End Sub
   Private Sub zzInitPanel1()
      Dim iYTop As Integer = 52
      Dim iRowHeight As Integer = 24
      Dim iRowYDist As Integer = 64

      Dim iYTop1 As Integer = iYTop + iRowYDist
      Dim iYTop2 As Integer = iYTop1 + iRowYDist

      Dim iBorderStyle As BorderStyle = BorderStyle.None


      Me.lblRes1PgonCount = New System.Windows.Forms.Label()
      Me.lblRes1TopoExists = New System.Windows.Forms.Label()
      Me.txtRes1PgonCount = New System.Windows.Forms.TextBox()
      Me.lblRes1TopoNameCap = New System.Windows.Forms.Label()
      Me.lblRes1TopoName = New System.Windows.Forms.Label()

      '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

      '
      'lblRes1TopoExists
      '
      With Me.lblRes1TopoExists
         .Image = Global.TopoUI.My.Resources.Resources.DoneTr
         .Location = New System.Drawing.Point(260, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblRes1TopoExists"
         .Size = New System.Drawing.Size(30, 22)
         .TabIndex = 21
      End With

 



      '
      'lblRes1TopoNameCap
      '
      With Me.lblRes1TopoNameCap
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblRes1TopoNameCap"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 20

         .Text = ptMapThemeData.GraphTypeName & ":"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With

      '
      'lblRes1TopoName
      '
      With Me.lblRes1TopoName
         '	.BorderStyle = iBorderStyle
         .Location = New System.Drawing.Point(34, iYTop)
         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblRes1TopoName"
         .Size = New System.Drawing.Size(116, 22)
         .TabIndex = 13
         .Text = ptMapThemeData.LineTopoName

         .TextAlign = System.Drawing.ContentAlignment.MiddleRight
         .BorderStyle = iBorderStyle
      End With
      '
      'lblRes1PgonCount
      '
      With Me.lblRes1PgonCount
         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .ForeColor = System.Drawing.SystemColors.ControlText
         .Location = New System.Drawing.Point(150, iYTop + iRowHeight + 4)

         .Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
         .Name = "lblRes1PgonCount"
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .Size = New System.Drawing.Size(110, 22)
         .TabIndex = 24
         .Text = "מס' פוליגונים:"
         .TextAlign = System.Drawing.ContentAlignment.MiddleLeft
         .BorderStyle = iBorderStyle
      End With


      '
      'txtRes1PgonCount
      '
      With Me.txtRes1PgonCount

         .Location =  New System.Drawing.Point(114, iYTop + iRowHeight + 4)
         .Margin = New System.Windows.Forms.Padding(4)
         .Name = "txtRes1PgonCount"
         .ReadOnly = True
         .Size = New System.Drawing.Size(36, 22)
         .TabIndex = 22
         .BorderStyle = BorderStyle.Fixed3D
      End With





    
 
    

 


      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      '
      With doaPanels(1)


         '	.BackColor = System.Drawing.SystemColors.ControlLight
         .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle

         '.Controls.Add(Me.lblMapLayerExists(iIndex))
         '.Controls.Add(Me.lblMapLayerCap(iIndex))
         '.Controls.Add(Me.lblMapLayer(iIndex))
         '.Controls.Add(Me.lblPgonCount(iIndex))
         '.Controls.Add(Me.lblThemeCaption(iIndex))
         .Controls.Add(Me.lblRes1TopoExists)
         .Controls.Add(Me.txtRes1PgonCount)
         .Controls.Add(Me.lblRes1TopoNameCap)
         .Controls.Add(Me.lblRes1TopoName)
         .Controls.Add(Me.lblRes1PgonCount)
      End With
      Me.Controls.Add(Me.doaPanels(1))

   End Sub
   Private Sub zzMyInitializeComponent()
      Dim saCaptions() As String = {"חיתוך טופולוגיות", "בלוקים"}
      MyBase.psaCaptions = saCaptions
      MyBase.OnNew()

      zzInitPanel0()
      zzInitPanel1()
      '    zzCheckTopo(iIndex, False)
      '   zzCheckMapLayer(iIndex)

      zzInitToolStrip()
      AfterChangeCurrent(0)
      ' MessageBox.Show(Me.doaPanels.GetUpperBound(0).ToString, "07_460")

   End Sub
   Private Function zzGetMapThemeData(iIndex As Integer) As DMAcadExt.MapThemeData
      Select Case iIndex
         Case 0
            Return ptSourceMapThemeData
         Case 1
            Return ptOverlayMapThemeData
         Case 2
            Return ptMapThemeData
         Case Else
            Return New DMAcadExt.MapThemeData()
      End Select
   End Function
   Private Sub zzCheckTopo(ByVal iIndex As Integer, ByVal bLockDoc As Boolean, ByVal bDispRes As Boolean, ByVal bMsg As Boolean)
      Dim oMapThemeData As DMAcadExt.MapThemeData = zzGetMapThemeData(iIndex)
      Dim tTopoRes As DMAcadExt.TopoRes

      If oMapThemeData.LineTopoName IsNot Nothing Then
         tTopoRes = TopoManager.TopoCreator.CheckTopo(oMapThemeData.LineTopoName, bLockDoc, bMsg, ptMapThemeData.MapThemeID)
         If bDispRes Then
            zzDispTopoOK(iIndex, tTopoRes)
         End If
         mbIsDone = tTopoRes.IsOK

      End If

      '	MessageBox.Show("Locked: " & CStr(DMAcadExt.AcadDocument.IsLocked) & vbCrLf & CStr(tTopoRes.IsInstance) & vbCrLf & oMapThemeData.GraphType.ToString(), "04_451")

      '	zzDispTopoExists(iIndex, tTopoRes.TopoExists)

      '
   End Sub
   Private Sub zzDispTopoOK(ByVal iIndex As Integer, tTopoRes As DMAcadExt.TopoRes)
      Dim bOK As Boolean = tTopoRes.IsOK

      Select Case iIndex
         Case 0
            Me.lblSrcTopoExists.Visible = bOK
            Me.txtSrcPgonCount.Enabled = bOK
            Me.txtSrcPgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
            Me.lblSrcTopoName.ForeColor = ColorByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            Me.lblSrcTopoName.Font = FontByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            '    MessageBox.Show(iIndex.ToString() & vbCrLf & bOK.ToString() & vbCrLf & tTopoRes.PgonCount.ToString() & vbCrLf & Me.lblSrcTopoName.Font.ToString())

         Case 1
            Me.lblOvlTopoExists.Visible = bOK
            Me.txtOvlPgonCount.Enabled = bOK
            Me.txtOvlPgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
            Me.lblOvlTopoName.ForeColor = ColorByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            Me.lblOvlTopoName.Font = FontByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            '  MessageBox.Show(iIndex.ToString() & vbCrLf & bOK.ToString() & vbCrLf & tTopoRes.PgonCount.ToString() & vbCrLf & Me.lblOvlTopoName.Font.ToString())
         Case 2
            Me.lblResTopoExists.Visible = bOK
            Me.txtResPgonCount.Enabled = bOK
            Me.txtResPgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
            Me.lblResTopoName.ForeColor = ColorByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            Me.lblResTopoName.Font = FontByExists(bOK AndAlso tTopoRes.PgonCount > 0)

            Me.lblRes1TopoExists.Visible = bOK
            Me.txtRes1PgonCount.Enabled = bOK
            Me.txtRes1PgonCount.Text = Convert.ToString(tTopoRes.PgonCount)
            Me.lblRes1TopoName.ForeColor = ColorByExists(bOK AndAlso tTopoRes.PgonCount > 0)
            Me.lblRes1TopoName.Font = FontByExists(bOK AndAlso tTopoRes.PgonCount > 0)
      End Select
     
     
   End Sub
   Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbCreateTopo.Name
            Select Case LabelCheck.CurrentIndex
               Case 0
                  zzCreateTopo()
            End Select

         Case Me.tsbCheckTopo.Name
            Select Case LabelCheck.CurrentIndex
               Case 0, 1, 2
                  ''''''''''  zzCheckTopo(LabelCheck.CurrentIndex, True, True)
            End Select

         Case Me.tsbDeleteTopo.Name
            Select Case LabelCheck.CurrentIndex
               Case 0
                  zzDeleteTopo()

                  '	zzRemoveMapLayer(LabelCheck.CurrentIndex)
                  '	zzCheckMapLayer(LabelCheck.CurrentIndex)
                  '	Me.zzDeleteTopo(LabelCheck.CurrentIndex)
               Case 3
              
                  zzDispRes(True)
            End Select

         Case Me.tsbExec.Name
            Select Case LabelCheck.CurrentIndex
               Case 1
                  zzFillCentroids()
               Case 3


            End Select
         Case Me.tsbExecA.Name
            Select Case LabelCheck.CurrentIndex
               Case 0, 1, 2

               Case 3





            End Select
         Case Me.tsbClear.Name
            Select Case LabelCheck.CurrentIndex
               Case 0, 1, 2
                 
                  '	Me.zzDeleteTopo(LabelCheck.CurrentIndex)
               Case 3
                  
            End Select

      End Select
      Me.Cursor = Cursors.Default
      DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub zzCreateTopo()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()

      Dim oSrcTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptSourceMapThemeData.LineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If oSrcTopology IsNot Nothing Then

         Dim oOvlTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptOverlayMapThemeData.LineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
         If oOvlTopology IsNot Nothing Then
            Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer("pCellM", DMAcadExt.DMApp.AppID, True, False)
            '

            Dim oEdgeCreationSettings As Autodesk.Gis.Map.Topology.EntityCreationSettings = New Autodesk.Gis.Map.Topology.EntityCreationSettings(ptMapThemeData.LinkLayer, 0)
            Dim oCentroidCreationSettings As Autodesk.Gis.Map.Topology.PointCreationSettings = New Autodesk.Gis.Map.Topology.PointCreationSettings(ptMapThemeData.CentroidLayer, 0, True, ptMapThemeData.CentroidBlock)
            Dim oNodeCreationSettings As Autodesk.Gis.Map.Topology.PointCreationSettings = New Autodesk.Gis.Map.Topology.PointCreationSettings("0", 0, False, String.Empty)
            oSrcTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
            '  MessageBox.Show(oSrcTopology.Name & vbCrLf & oOvlTopology.Name & vbCrLf & ptMapThemeData.CentroidLayer & vbCrLf & ptMapThemeData.CentroidBlock, "07_300aa")
            oSrcTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
            '  MessageBox.Show(oSrcTopology.Name & vbCrLf & oOvlTopology.Name & vbCrLf & ptMapThemeData.LineTopoName & vbCrLf & ptSourceMapThemeData.CentroidBlock & vbCrLf & ptMapThemeData.CentroidBlock & vbCrLf & ptMapThemeData.CentroidLayer & vbCrLf & ptMapThemeData.LinkLayer, "07_300c")


            Try

               oSrcTopology.SetNodeCreationSettings(oNodeCreationSettings)

            Catch oMapEx As Autodesk.Gis.Map.MapException
               '  System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_848")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "SetNode - ")

            End Try

            '     MessageBox.Show(oSrcTopology.Name & vbCrLf & oOvlTopology.Name & vbCrLf & ptMapThemeData.LineTopoName & vbCrLf & ptSourceMapThemeData.CentroidBlock & vbCrLf & ptMapThemeData.CentroidBlock & vbCrLf & ptMapThemeData.CentroidLayer & vbCrLf & ptMapThemeData.LinkLayer, "07_301")

            Try
               oSrcTopology.Clip(oOvlTopology, ptMapThemeData.LineTopoName)
            Catch oMapEx As Autodesk.Gis.Map.MapException
               '  System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_848")
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "CreateTopo - ")
            End Try


            '  MessageBox.Show(ptMapThemeData.LineTopoName, "07_302")
            zzCheckTopo(2, False, True, False)
            '   MessageBox.Show(ptMapThemeData.LineTopoName, "07_303")
         End If
      End If
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzDeleteTopo()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      TopoManager.TopoCreator.DeleteTopology(zzGetMapThemeData(2).LineTopoName, False, False)

      zzCheckTopo(2, False, True, True)
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   Private Sub zzFillCentroids()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


      Dim oResTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptMapThemeData.LineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      Dim oSrcTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptSourceMapThemeData.LineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If oResTopology IsNot Nothing AndAlso oSrcTopology IsNot Nothing Then
         ' MessageBox.Show(ptMapThemeData.CentroidBlock & vbCrLf & "", "07_750")
         ' DMCommon.ExcelLog.Open()
         Dim oSrcBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(ptMapThemeData.CentroidBlock)
         Dim oResBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(ptMapThemeData.CentroidBlock)
         Dim saFields() As String = {"CELLNO", "CODE", "PLAN"}
         Dim tCentroidData As DMAcadExt.BlockRefData
         Dim sCellNo As String
         Dim sCode As String
         Dim sPlan As String



         oSrcBlock.Fields = saFields
         oSrcBlock.OpenForRead()
         oResBlock.Fields = saFields
         oResBlock.OpenForRead()
         Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oResTopology.GetPolygons()
         Dim oSrcPolygon As Autodesk.Gis.Map.Topology.Polygon
         Dim saValues(saFields.GetUpperBound(0)) As String


         For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
            oSrcPolygon = oSrcTopology.FindPolygon(oPolygon.Centroid)
            tCentroidData = oSrcBlock.GetBlockRefData(oSrcPolygon.Entity)
            sCellNo = tCentroidData.GetAttribValue("CELLNO")
            sCode = tCentroidData.GetAttribValue("CODE")
            sPlan = tCentroidData.GetAttribValue("PLAN")


            '
            '   MessageBox.Show(sCellNo & vbCrLf & sCode, "07_760")
            saValues(0) = sCellNo
            saValues(1) = sCode
            saValues(2) = sPlan


            oResBlock.UpdateAttribData(oPolygon.Entity, saValues, String.Empty)

            '    oPolygon.Entity
            oPolygon = Nothing
         Next
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()

   End Sub
   Private Sub zzDispRes(bResultMapLayer As Boolean)
      
   End Sub
   
   Private Sub frmTopoOverlay_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
      zzMyInitializeComponent()

      zzMyInitialize()

      '   zzDispRes(True)
   End Sub
End Class