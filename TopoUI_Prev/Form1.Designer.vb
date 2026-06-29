<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
      Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
      Me.PanelX = New System.Windows.Forms.Panel()
      Me.cmdEraseErr = New System.Windows.Forms.Button()
      Me.txtPgonCount = New System.Windows.Forms.TextBox()
      Me.lblTopoExists = New System.Windows.Forms.Label()
      Me.cmdNextErr = New System.Windows.Forms.Button()
      Me.cmdPrevErr = New System.Windows.Forms.Button()
      Me.cmdStartErr = New System.Windows.Forms.Button()
      Me.txtErrorCount = New System.Windows.Forms.TextBox()
      Me.txtErrorIndex = New System.Windows.Forms.TextBox()
      Me.lblTopoName = New System.Windows.Forms.Label()
      Me.lblTopoNameCap = New System.Windows.Forms.Label()
      Me.tstTopology = New System.Windows.Forms.ToolStrip()
      Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripButton6 = New System.Windows.Forms.ToolStripButton()
      Me.tsbWorkArea = New System.Windows.Forms.ToolStripButton()
      Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
      Me.FormErrToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
      Me.ToolStripSplitButton1 = New System.Windows.Forms.ToolStripSplitButton()
      Me.FormErrToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
      Me.lblErrors = New System.Windows.Forms.Label()
      Me.grbCentroids = New System.Windows.Forms.GroupBox()
      Me.txtCentroidCount = New System.Windows.Forms.TextBox()
      Me.txtCentroidBlocks = New System.Windows.Forms.TextBox()
      Me.txtCentroidLayers = New System.Windows.Forms.TextBox()
      Me.lblCentroidBlocks = New System.Windows.Forms.Label()
      Me.lblCentroidLayers = New System.Windows.Forms.Label()
      Me.grbLinks = New System.Windows.Forms.GroupBox()
      Me.txtLinkCount = New System.Windows.Forms.TextBox()
      Me.txtLinkLayers = New System.Windows.Forms.TextBox()
      Me.lblLinkLayers = New System.Windows.Forms.Label()
      Me.txtTolerance = New System.Windows.Forms.TextBox()
      Me.lblTolerance = New System.Windows.Forms.Label()
      Me.chkHighlightSliver = New System.Windows.Forms.CheckBox()
      Me.chkCreateCentroid = New System.Windows.Forms.CheckBox()
      Me.PanelX.SuspendLayout()
      Me.tstTopology.SuspendLayout()
      Me.grbCentroids.SuspendLayout()
      Me.grbLinks.SuspendLayout()
      Me.SuspendLayout()
      '
      'PanelX
      '
      Me.PanelX.BackColor = System.Drawing.SystemColors.ControlLight
      Me.PanelX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
      Me.PanelX.Controls.Add(Me.cmdEraseErr)
      Me.PanelX.Controls.Add(Me.txtPgonCount)
      Me.PanelX.Controls.Add(Me.lblTopoExists)
      Me.PanelX.Controls.Add(Me.cmdNextErr)
      Me.PanelX.Controls.Add(Me.cmdPrevErr)
      Me.PanelX.Controls.Add(Me.cmdStartErr)
      Me.PanelX.Controls.Add(Me.txtErrorCount)
      Me.PanelX.Controls.Add(Me.txtErrorIndex)
      Me.PanelX.Controls.Add(Me.lblTopoName)
      Me.PanelX.Controls.Add(Me.lblTopoNameCap)
      Me.PanelX.Controls.Add(Me.tstTopology)
      Me.PanelX.Controls.Add(Me.lblErrors)
      Me.PanelX.Controls.Add(Me.grbCentroids)
      Me.PanelX.Controls.Add(Me.grbLinks)
      Me.PanelX.Controls.Add(Me.txtTolerance)
      Me.PanelX.Controls.Add(Me.lblTolerance)
      Me.PanelX.Controls.Add(Me.chkHighlightSliver)
      Me.PanelX.Controls.Add(Me.chkCreateCentroid)
      Me.PanelX.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.PanelX.Location = New System.Drawing.Point(0, 0)
      Me.PanelX.Margin = New System.Windows.Forms.Padding(4)
      Me.PanelX.Name = "PanelX"
      Me.PanelX.Size = New System.Drawing.Size(449, 337)
      Me.PanelX.TabIndex = 1
      '
      'cmdEraseErr
      '
      Me.cmdEraseErr.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
      Me.cmdEraseErr.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
      Me.cmdEraseErr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
      Me.cmdEraseErr.Location = New System.Drawing.Point(320, 296)
      Me.cmdEraseErr.Margin = New System.Windows.Forms.Padding(4)
      Me.cmdEraseErr.Name = "cmdEraseErr"
      Me.cmdEraseErr.Size = New System.Drawing.Size(25, 25)
      Me.cmdEraseErr.TabIndex = 21
      Me.cmdEraseErr.UseVisualStyleBackColor = True
      '
      'txtPgonCount
      '
      Me.txtPgonCount.Location = New System.Drawing.Point(378, 108)
      Me.txtPgonCount.Margin = New System.Windows.Forms.Padding(4)
      Me.txtPgonCount.Name = "txtPgonCount"
      Me.txtPgonCount.ReadOnly = True
      Me.txtPgonCount.Size = New System.Drawing.Size(58, 22)
      Me.txtPgonCount.TabIndex = 20
      '
      'lblTopoExists
      '
      Me.lblTopoExists.Image = Global.TopoUI.My.Resources.Resources.DoneTr
      Me.lblTopoExists.Location = New System.Drawing.Point(12, 106)
      Me.lblTopoExists.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoExists.Name = "lblTopoExists"
      Me.lblTopoExists.Size = New System.Drawing.Size(30, 25)
      Me.lblTopoExists.TabIndex = 19
      '
      'cmdNextErr
      '
      Me.cmdNextErr.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveNextTr
      Me.cmdNextErr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
      Me.cmdNextErr.Location = New System.Drawing.Point(288, 296)
      Me.cmdNextErr.Margin = New System.Windows.Forms.Padding(4)
      Me.cmdNextErr.Name = "cmdNextErr"
      Me.cmdNextErr.Size = New System.Drawing.Size(24, 30)
      Me.cmdNextErr.TabIndex = 18
      Me.cmdNextErr.UseVisualStyleBackColor = True
      '
      'cmdPrevErr
      '
      Me.cmdPrevErr.BackgroundImage = Global.TopoUI.My.Resources.Resources.MovePrevTr
      Me.cmdPrevErr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
      Me.cmdPrevErr.Location = New System.Drawing.Point(261, 296)
      Me.cmdPrevErr.Margin = New System.Windows.Forms.Padding(4)
      Me.cmdPrevErr.Name = "cmdPrevErr"
      Me.cmdPrevErr.Size = New System.Drawing.Size(24, 30)
      Me.cmdPrevErr.TabIndex = 17
      Me.cmdPrevErr.UseVisualStyleBackColor = True
      '
      'cmdStartErr
      '
      Me.cmdStartErr.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveFirstTr
      Me.cmdStartErr.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
      Me.cmdStartErr.Location = New System.Drawing.Point(228, 296)
      Me.cmdStartErr.Margin = New System.Windows.Forms.Padding(4)
      Me.cmdStartErr.Name = "cmdStartErr"
      Me.cmdStartErr.Size = New System.Drawing.Size(30, 30)
      Me.cmdStartErr.TabIndex = 16
      Me.cmdStartErr.UseVisualStyleBackColor = True
      '
      'txtErrorCount
      '
      Me.txtErrorCount.Location = New System.Drawing.Point(153, 296)
      Me.txtErrorCount.Margin = New System.Windows.Forms.Padding(4)
      Me.txtErrorCount.Name = "txtErrorCount"
      Me.txtErrorCount.ReadOnly = True
      Me.txtErrorCount.Size = New System.Drawing.Size(61, 22)
      Me.txtErrorCount.TabIndex = 15
      '
      'txtErrorIndex
      '
      Me.txtErrorIndex.Location = New System.Drawing.Point(90, 296)
      Me.txtErrorIndex.Margin = New System.Windows.Forms.Padding(4)
      Me.txtErrorIndex.Name = "txtErrorIndex"
      Me.txtErrorIndex.Size = New System.Drawing.Size(58, 22)
      Me.txtErrorIndex.TabIndex = 14
      Me.txtErrorIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
      '
      'lblTopoName
      '
      Me.lblTopoName.Location = New System.Drawing.Point(189, 108)
      Me.lblTopoName.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoName.Name = "lblTopoName"
      Me.lblTopoName.Size = New System.Drawing.Size(180, 25)
      Me.lblTopoName.TabIndex = 13
      '
      'lblTopoNameCap
      '
      Me.lblTopoNameCap.BackColor = System.Drawing.SystemColors.ControlLight
      Me.lblTopoNameCap.ForeColor = System.Drawing.SystemColors.GrayText
      Me.lblTopoNameCap.Location = New System.Drawing.Point(42, 108)
      Me.lblTopoNameCap.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTopoNameCap.Name = "lblTopoNameCap"
      Me.lblTopoNameCap.Size = New System.Drawing.Size(144, 25)
      Me.lblTopoNameCap.TabIndex = 12
      Me.lblTopoNameCap.Text = "Topology name:"
      '
      'tstTopology
      '
      Me.tstTopology.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripButton2, Me.ToolStripButton3, Me.ToolStripButton4, Me.ToolStripButton5, Me.ToolStripButton6, Me.tsbWorkArea, Me.ToolStripDropDownButton1, Me.ToolStripSplitButton1})
      Me.tstTopology.Location = New System.Drawing.Point(0, 0)
      Me.tstTopology.Name = "tstTopology"
      Me.tstTopology.Size = New System.Drawing.Size(447, 25)
      Me.tstTopology.TabIndex = 11
      '
      'ToolStripButton1
      '
      Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton1.Image = Global.TopoUI.My.Resources.Resources.Import16Tr
      Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton1.Name = "ToolStripButton1"
      Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton1.Text = "ToolStripButton1"
      '
      'ToolStripButton2
      '
      Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton2.Image = Global.TopoUI.My.Resources.Resources.Erase1
      Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton2.Name = "ToolStripButton2"
      Me.ToolStripButton2.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton2.Text = "ToolStripButton2"
      '
      'ToolStripButton3
      '
      Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton3.Image = Global.TopoUI.My.Resources.Resources.Table19x17
      Me.ToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton3.Name = "ToolStripButton3"
      Me.ToolStripButton3.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton3.Text = "ToolStripButton3"
      '
      'ToolStripButton4
      '
      Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton4.Image = Global.TopoUI.My.Resources.Resources.ColorPgonsTr
      Me.ToolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton4.Name = "ToolStripButton4"
      Me.ToolStripButton4.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton4.Text = "ToolStripButton4"
      '
      'ToolStripButton5
      '
      Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton5.Image = Global.TopoUI.My.Resources.Resources.GushTr
      Me.ToolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton5.Name = "ToolStripButton5"
      Me.ToolStripButton5.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton5.Text = "ToolStripButton5"
      '
      'ToolStripButton6
      '
      Me.ToolStripButton6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripButton6.Image = Global.TopoUI.My.Resources.Resources.Check16Tr
      Me.ToolStripButton6.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripButton6.Name = "ToolStripButton6"
      Me.ToolStripButton6.Size = New System.Drawing.Size(23, 22)
      Me.ToolStripButton6.Text = "ToolStripButton6"
      '
      'tsbWorkArea
      '
      Me.tsbWorkArea.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.tsbWorkArea.Image = Global.TopoUI.My.Resources.Resources.World14
      Me.tsbWorkArea.ImageTransparentColor = System.Drawing.Color.White
      Me.tsbWorkArea.Name = "tsbWorkArea"
      Me.tsbWorkArea.RightToLeftAutoMirrorImage = True
      Me.tsbWorkArea.Size = New System.Drawing.Size(23, 22)
      '
      'ToolStripDropDownButton1
      '
      Me.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FormErrToolStripMenuItem})
      Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
      Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
      Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(29, 22)
      Me.ToolStripDropDownButton1.Text = "ToolStripDropDownButton1"
      '
      'FormErrToolStripMenuItem
      '
      Me.FormErrToolStripMenuItem.Name = "FormErrToolStripMenuItem"
      Me.FormErrToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
      Me.FormErrToolStripMenuItem.Text = "FormErr"
      '
      'ToolStripSplitButton1
      '
      Me.ToolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
      Me.ToolStripSplitButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FormErrToolStripMenuItem1})
      Me.ToolStripSplitButton1.Image = CType(resources.GetObject("ToolStripSplitButton1.Image"), System.Drawing.Image)
      Me.ToolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta
      Me.ToolStripSplitButton1.Name = "ToolStripSplitButton1"
      Me.ToolStripSplitButton1.Size = New System.Drawing.Size(32, 22)
      Me.ToolStripSplitButton1.Text = "ToolStripSplitButton1"
      '
      'FormErrToolStripMenuItem1
      '
      Me.FormErrToolStripMenuItem1.Name = "FormErrToolStripMenuItem1"
      Me.FormErrToolStripMenuItem1.Size = New System.Drawing.Size(119, 22)
      Me.FormErrToolStripMenuItem1.Text = "Form Err"
      '
      'lblErrors
      '
      Me.lblErrors.Location = New System.Drawing.Point(12, 296)
      Me.lblErrors.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblErrors.Name = "lblErrors"
      Me.lblErrors.Size = New System.Drawing.Size(75, 25)
      Me.lblErrors.TabIndex = 9
      Me.lblErrors.Text = "Errors:"
      '
      'grbCentroids
      '
      Me.grbCentroids.Controls.Add(Me.txtCentroidCount)
      Me.grbCentroids.Controls.Add(Me.txtCentroidBlocks)
      Me.grbCentroids.Controls.Add(Me.txtCentroidLayers)
      Me.grbCentroids.Controls.Add(Me.lblCentroidBlocks)
      Me.grbCentroids.Controls.Add(Me.lblCentroidLayers)
      Me.grbCentroids.Location = New System.Drawing.Point(12, 199)
      Me.grbCentroids.Margin = New System.Windows.Forms.Padding(4)
      Me.grbCentroids.Name = "grbCentroids"
      Me.grbCentroids.Padding = New System.Windows.Forms.Padding(4)
      Me.grbCentroids.Size = New System.Drawing.Size(426, 89)
      Me.grbCentroids.TabIndex = 8
      Me.grbCentroids.TabStop = False
      Me.grbCentroids.Text = "Centroids"
      '
      'txtCentroidCount
      '
      Me.txtCentroidCount.Location = New System.Drawing.Point(357, 20)
      Me.txtCentroidCount.Margin = New System.Windows.Forms.Padding(4)
      Me.txtCentroidCount.Name = "txtCentroidCount"
      Me.txtCentroidCount.ReadOnly = True
      Me.txtCentroidCount.Size = New System.Drawing.Size(58, 22)
      Me.txtCentroidCount.TabIndex = 19
      '
      'txtCentroidBlocks
      '
      Me.txtCentroidBlocks.Location = New System.Drawing.Point(148, 54)
      Me.txtCentroidBlocks.Margin = New System.Windows.Forms.Padding(4)
      Me.txtCentroidBlocks.Name = "txtCentroidBlocks"
      Me.txtCentroidBlocks.ReadOnly = True
      Me.txtCentroidBlocks.Size = New System.Drawing.Size(201, 22)
      Me.txtCentroidBlocks.TabIndex = 18
      '
      'txtCentroidLayers
      '
      Me.txtCentroidLayers.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.txtCentroidLayers.Location = New System.Drawing.Point(99, 20)
      Me.txtCentroidLayers.Margin = New System.Windows.Forms.Padding(4)
      Me.txtCentroidLayers.Name = "txtCentroidLayers"
      Me.txtCentroidLayers.ReadOnly = True
      Me.txtCentroidLayers.Size = New System.Drawing.Size(250, 22)
      Me.txtCentroidLayers.TabIndex = 17
      '
      'lblCentroidBlocks
      '
      Me.lblCentroidBlocks.Location = New System.Drawing.Point(30, 58)
      Me.lblCentroidBlocks.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblCentroidBlocks.Name = "lblCentroidBlocks"
      Me.lblCentroidBlocks.Size = New System.Drawing.Size(117, 25)
      Me.lblCentroidBlocks.TabIndex = 7
      Me.lblCentroidBlocks.Text = "Block names:"
      '
      'lblCentroidLayers
      '
      Me.lblCentroidLayers.Location = New System.Drawing.Point(30, 25)
      Me.lblCentroidLayers.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblCentroidLayers.Name = "lblCentroidLayers"
      Me.lblCentroidLayers.Size = New System.Drawing.Size(68, 25)
      Me.lblCentroidLayers.TabIndex = 6
      Me.lblCentroidLayers.Text = "Layers:"
      '
      'grbLinks
      '
      Me.grbLinks.Controls.Add(Me.txtLinkCount)
      Me.grbLinks.Controls.Add(Me.txtLinkLayers)
      Me.grbLinks.Controls.Add(Me.lblLinkLayers)
      Me.grbLinks.Location = New System.Drawing.Point(12, 133)
      Me.grbLinks.Margin = New System.Windows.Forms.Padding(4)
      Me.grbLinks.Name = "grbLinks"
      Me.grbLinks.Padding = New System.Windows.Forms.Padding(4)
      Me.grbLinks.Size = New System.Drawing.Size(426, 60)
      Me.grbLinks.TabIndex = 7
      Me.grbLinks.TabStop = False
      Me.grbLinks.Text = "Links"
      '
      'txtLinkCount
      '
      Me.txtLinkCount.Location = New System.Drawing.Point(357, 20)
      Me.txtLinkCount.Margin = New System.Windows.Forms.Padding(4)
      Me.txtLinkCount.Name = "txtLinkCount"
      Me.txtLinkCount.ReadOnly = True
      Me.txtLinkCount.Size = New System.Drawing.Size(58, 22)
      Me.txtLinkCount.TabIndex = 17
      '
      'txtLinkLayers
      '
      Me.txtLinkLayers.Location = New System.Drawing.Point(99, 20)
      Me.txtLinkLayers.Margin = New System.Windows.Forms.Padding(4)
      Me.txtLinkLayers.Name = "txtLinkLayers"
      Me.txtLinkLayers.ReadOnly = True
      Me.txtLinkLayers.Size = New System.Drawing.Size(250, 22)
      Me.txtLinkLayers.TabIndex = 16
      '
      'lblLinkLayers
      '
      Me.lblLinkLayers.Location = New System.Drawing.Point(30, 25)
      Me.lblLinkLayers.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblLinkLayers.Name = "lblLinkLayers"
      Me.lblLinkLayers.Size = New System.Drawing.Size(68, 25)
      Me.lblLinkLayers.TabIndex = 6
      Me.lblLinkLayers.Text = "Layers:"
      '
      'txtTolerance
      '
      Me.txtTolerance.Location = New System.Drawing.Point(360, 44)
      Me.txtTolerance.Margin = New System.Windows.Forms.Padding(4)
      Me.txtTolerance.Name = "txtTolerance"
      Me.txtTolerance.Size = New System.Drawing.Size(76, 22)
      Me.txtTolerance.TabIndex = 3
      '
      'lblTolerance
      '
      Me.lblTolerance.Location = New System.Drawing.Point(255, 44)
      Me.lblTolerance.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
      Me.lblTolerance.Name = "lblTolerance"
      Me.lblTolerance.Size = New System.Drawing.Size(102, 25)
      Me.lblTolerance.TabIndex = 2
      Me.lblTolerance.Text = "Tolerance"
      '
      'chkHighlightSliver
      '
      Me.chkHighlightSliver.AutoSize = True
      Me.chkHighlightSliver.Location = New System.Drawing.Point(12, 72)
      Me.chkHighlightSliver.Margin = New System.Windows.Forms.Padding(4)
      Me.chkHighlightSliver.Name = "chkHighlightSliver"
      Me.chkHighlightSliver.Size = New System.Drawing.Size(105, 18)
      Me.chkHighlightSliver.TabIndex = 1
      Me.chkHighlightSliver.Text = "Highlight Sliver"
      Me.chkHighlightSliver.UseVisualStyleBackColor = True
      '
      'chkCreateCentroid
      '
      Me.chkCreateCentroid.AutoSize = True
      Me.chkCreateCentroid.Location = New System.Drawing.Point(12, 44)
      Me.chkCreateCentroid.Margin = New System.Windows.Forms.Padding(4)
      Me.chkCreateCentroid.Name = "chkCreateCentroid"
      Me.chkCreateCentroid.Size = New System.Drawing.Size(108, 18)
      Me.chkCreateCentroid.TabIndex = 0
      Me.chkCreateCentroid.Text = "Insert Centroid"
      Me.chkCreateCentroid.UseVisualStyleBackColor = True
      '
      'Form1
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 18.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.BackColor = System.Drawing.SystemColors.GrayText
      Me.ClientSize = New System.Drawing.Size(1170, 482)
      Me.Controls.Add(Me.PanelX)
      Me.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
      Me.Margin = New System.Windows.Forms.Padding(4)
      Me.Name = "Form1"
      Me.Text = "Form1"
      Me.TopMost = True
      Me.PanelX.ResumeLayout(False)
      Me.PanelX.PerformLayout()
      Me.tstTopology.ResumeLayout(False)
      Me.tstTopology.PerformLayout()
      Me.grbCentroids.ResumeLayout(False)
      Me.grbCentroids.PerformLayout()
      Me.grbLinks.ResumeLayout(False)
      Me.grbLinks.PerformLayout()
      Me.ResumeLayout(False)

   End Sub
	Private WithEvents PanelX As System.Windows.Forms.Panel
	Private WithEvents chkCreateCentroid As System.Windows.Forms.CheckBox
	Private WithEvents chkHighlightSliver As System.Windows.Forms.CheckBox
	Private WithEvents lblTolerance As System.Windows.Forms.Label
	Private WithEvents txtTolerance As System.Windows.Forms.TextBox
	Private WithEvents lblLinkLayers As System.Windows.Forms.Label
	Private WithEvents grbLinks As System.Windows.Forms.GroupBox
	Private WithEvents lblErrors As System.Windows.Forms.Label
	Private WithEvents grbCentroids As System.Windows.Forms.GroupBox
	Private WithEvents lblCentroidBlocks As System.Windows.Forms.Label
	Private WithEvents lblCentroidLayers As System.Windows.Forms.Label
	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private WithEvents txtErrorCount As System.Windows.Forms.TextBox
	Private WithEvents txtErrorIndex As System.Windows.Forms.TextBox
	Private WithEvents lblTopoName As System.Windows.Forms.Label
	Private WithEvents lblTopoNameCap As System.Windows.Forms.Label
	Private WithEvents cmdNextErr As System.Windows.Forms.Button
	Private WithEvents cmdPrevErr As System.Windows.Forms.Button
	Private WithEvents cmdStartErr As System.Windows.Forms.Button
	Private WithEvents lblTopoExists As System.Windows.Forms.Label
	Private WithEvents txtPgonCount As System.Windows.Forms.TextBox
	Private WithEvents txtCentroidCount As System.Windows.Forms.TextBox
	Private WithEvents txtCentroidBlocks As System.Windows.Forms.TextBox
	Private WithEvents txtCentroidLayers As System.Windows.Forms.TextBox
	Private WithEvents txtLinkCount As System.Windows.Forms.TextBox
	Private WithEvents txtLinkLayers As System.Windows.Forms.TextBox
	Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
	Private WithEvents cmdEraseErr As System.Windows.Forms.Button
	Friend WithEvents ToolStripButton2 As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripButton3 As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripButton4 As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripButton5 As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripButton6 As System.Windows.Forms.ToolStripButton
   Private WithEvents tsbWorkArea As System.Windows.Forms.ToolStripButton
   Friend WithEvents ToolStripDropDownButton1 As System.Windows.Forms.ToolStripDropDownButton
   Friend WithEvents FormErrToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
   Friend WithEvents ToolStripSplitButton1 As System.Windows.Forms.ToolStripSplitButton
   Friend WithEvents FormErrToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
End Class
