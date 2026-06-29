Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Imports TopoManager
Public Class frmAnaliticClip

	Const miThisStagesUB As Integer = 5
	Private mdicPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)
	Private miResPgonCount As Integer
	Private mbAddOverlayExists As Boolean
	Private miCurrentIndexForDelay As Integer
	Private moCleanupActionsTable As System.Data.DataTable
	Private moCurrentPoints As DMAcadExt.TplnPointArray = Nothing
	Private miCurrentAction As Integer = -1
	Private moaTopoErrors As TopoErrorArray
	Private moPriorView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = Nothing
	Private txtTopoErrors As TextBox
	Private moaErrorPoints() As DMAcadExt.TplnPointArray = Nothing
	Private miStepNum As Integer
	Private lblTopoErrors As System.Windows.Forms.Label
	Private lblCaption As Label
	Private msAddTopoLayer As String
	'	Private mcolClipMPgonsFeatIDs As Autodesk.AutoCAD.Geometry.IntegerCollection
#Region "Panel0_1_2_Declarations"
	'	Private WithEvents PanelX As System.Windows.Forms.Panel
	'	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private lblTopoName(2) As System.Windows.Forms.Label
	Private txtPgonCount(2) As System.Windows.Forms.TextBox
	Private lblTopoExists(2) As System.Windows.Forms.Label
	Private lblTopoNameCap(2) As System.Windows.Forms.Label
	Private lblMapLayerExists(2) As System.Windows.Forms.Label
	Private lblMapLayerCap(2) As System.Windows.Forms.Label
	Private lblMapLayer(2) As System.Windows.Forms.Label
	Private lblPgonCount(2) As System.Windows.Forms.Label
	Private lblThemeCaption(2) As System.Windows.Forms.Label
#End Region
#Region "Panel0_3_Declarations"
	Private dgvActions(1) As DataGridView
	Private nudSteps(1) As System.Windows.Forms.NumericUpDown
	Private nudErrors(1) As System.Windows.Forms.NumericUpDown
	Private cmdEraseCleanupErr(1) As System.Windows.Forms.Button
	Private cmdFix(1) As TabButton
	Private cmdMark(1) As TabButton
	Private lblErrors(1) As System.Windows.Forms.Label
	Private moCurrentView(1) As DataView
#End Region
#Region "Panel3_Declarations"
	Private grbSliverTolerance As System.Windows.Forms.GroupBox
	Private lblToleranceMin As System.Windows.Forms.Label
	Private lblToleranceMax As System.Windows.Forms.Label
	Private txtToleranceMin As System.Windows.Forms.TextBox
	Private txtToleranceMax As System.Windows.Forms.TextBox
	Private WithEvents chkRemoveSlivers As CheckBox


	Private lblResultMapLayerExists As System.Windows.Forms.Label
	Private lblResultMapLayerCap As System.Windows.Forms.Label
	Private lblResultMapLayer As System.Windows.Forms.Label

	Private lblMapPgonCount As System.Windows.Forms.Label
	Private txtMapPgonCount As System.Windows.Forms.TextBox

	Private lblPgonsLayerName As System.Windows.Forms.Label
	Private lblPgonsLayerExists As System.Windows.Forms.Label
	Private lblPgonsLayerNameCap As System.Windows.Forms.Label

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
	Private grbClosedPgonsWA As System.Windows.Forms.GroupBox

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

	Private lblClosedPgonsLayersWA As System.Windows.Forms.Label
	Private txtClosedPgonsLayersWA As System.Windows.Forms.TextBox
	Private txtClosedPgonsCountWA As System.Windows.Forms.TextBox

#End Region
	Private Sub zzInitPanel0_1(iIndex As Integer)
		Me.lblMapLayerExists(iIndex) = New System.Windows.Forms.Label()
		Me.lblMapLayerCap(iIndex) = New System.Windows.Forms.Label()
		Me.lblMapLayer(iIndex) = New System.Windows.Forms.Label()
		Me.lblPgonCount(iIndex) = New System.Windows.Forms.Label()
		Me.lblThemeCaption(iIndex) = New System.Windows.Forms.Label()
		Me.lblTopoExists(iIndex) = New System.Windows.Forms.Label()
		Me.txtPgonCount(iIndex) = New System.Windows.Forms.TextBox()
		Me.lblTopoNameCap(iIndex) = New System.Windows.Forms.Label()
		Me.lblTopoName(iIndex) = New System.Windows.Forms.Label()
		'Me.tstTopology = New System.Windows.Forms.ToolStrip()
		'MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerExists(iIndex).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerCap(iIndex).RightToLeft.ToString() & vbCrLf & Me.txtPgonCount(iIndex).RightToLeft.ToString(), "02_154a")
		Dim iPanelIndex As Integer = iIndex
		'	Me.tstTopology.SuspendLayout()
		Me.doaPanels(iPanelIndex).SuspendLayout()
		Me.SuspendLayout()
		'
		'doaPanels(iPanelIndex)
		'
		With doaPanels(iPanelIndex)
			'	.BackColor = System.Drawing.SystemColors.ControlLight
         .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			.Controls.Add(Me.lblMapLayerExists(iIndex))
			.Controls.Add(Me.lblMapLayerCap(iIndex))
			.Controls.Add(Me.lblMapLayer(iIndex))
			.Controls.Add(Me.lblPgonCount(iIndex))
			.Controls.Add(Me.lblThemeCaption(iIndex))
			.Controls.Add(Me.lblTopoExists(iIndex))
			.Controls.Add(Me.txtPgonCount(iIndex))
			.Controls.Add(Me.lblTopoNameCap(iIndex))
			.Controls.Add(Me.lblTopoName(iIndex))
			'	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerExists(iIndex).RightToLeft.ToString() & vbCrLf & Me.lblMapLayerCap(iIndex).RightToLeft.ToString() & vbCrLf & Me.txtPgonCount(iIndex).RightToLeft.ToString(), "02_154b")
			'	.Controls.Add(Me.tstTopology)
			.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			.Location = New System.Drawing.Point(0, 0)
			.Margin = New System.Windows.Forms.Padding(4)
			.Size = dtPanelSize
         .Name = "Panel" & CStr(iIndex)

		End With

		'
		'lblTopoExists
		'
		With Me.lblTopoExists(iIndex)
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(260, 80)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblTopoExists"
			.Size = New System.Drawing.Size(30, 25)
			.TabIndex = 21
		End With
		'
		'lblTopoNameCap
		'
		With Me.lblTopoNameCap(iIndex)
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 80)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblTopoNameCap"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 20
			.Text = zzGetMapThemeData(iIndex).GraphTypeName & ":"

			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'lblTopoName
		'
		With Me.lblTopoName(iIndex)
			'	.BorderStyle = BorderStyle.FixedSingle
			.Location = New System.Drawing.Point(34, 80)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblTopoName"
			.Size = New System.Drawing.Size(116, 25)
			.TabIndex = 13
			.Text = zzGetMapThemeData(iIndex).ExportCondition

			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		End With


		'
		'lblMapLayerExists
		'
		With Me.lblMapLayerExists(iIndex)
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(260, 136)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblMapLayerExists"
			.Size = New System.Drawing.Size(30, 25)
			.TabIndex = 27
		End With
		'
		'lblMapLayerCap
		'
		With Me.lblMapLayerCap(iIndex)
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 136)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblMapLayerCap"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 26
			.Text = "שכבת מפה:"
			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'lblMapLayer
		'
		With Me.lblMapLayer(iIndex)
			.Location = New System.Drawing.Point(34, 136)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblMapLayer"
			.Size = New System.Drawing.Size(108, 25)
			.TabIndex = 25
			.Text = zzGetMapThemeData(iIndex).MapLayer
			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		End With
		'
		'lblPgonCount
		'
		With Me.lblPgonCount(iIndex)
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 108)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblPgonCount"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 24
			.Text = "מס' פוליגונים:"
			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'txtPgonCount
		'
		With Me.txtPgonCount(iIndex)
			.Location = New System.Drawing.Point(34, 108)
			.Margin = New System.Windows.Forms.Padding(4)
			.Name = "txtPgonCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(64, 22)
			.TabIndex = 22
		End With

		'
		'lblThemeCaption
		'
		With Me.lblThemeCaption(iIndex)
			'	.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			.Location = New System.Drawing.Point(59, 36)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblThemeCaption"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(180, 25)
			.TabIndex = 23
			.Text = zzGetMapThemeData(iIndex).MapThemeName
		End With




		'
		'me
		'
		Me.Controls.Add(Me.doaPanels(iPanelIndex))

		Me.doaPanels(iPanelIndex).ResumeLayout(False)
		Me.doaPanels(iPanelIndex).PerformLayout()
		Me.ResumeLayout(False)
	End Sub
	Private Sub zzInitPanel0_3(iCleanupIndex As Integer)
		Dim iPanelIndex As Integer
		If iCleanupIndex = 0 Then
			iPanelIndex = 3
		Else
			iPanelIndex = 3
		End If
		'	Me.doaPanels(iPanelIndex) = New System.Windows.Forms.Panel()

		Me.doaPanels(iPanelIndex).SuspendLayout()
		'	Me.SuspendLayout()
		Me.dgvActions(iCleanupIndex) = New System.Windows.Forms.DataGridView
		Me.nudSteps(iCleanupIndex) = New System.Windows.Forms.NumericUpDown
		Me.nudErrors(iCleanupIndex) = New System.Windows.Forms.NumericUpDown
		Me.cmdEraseCleanupErr(iCleanupIndex) = New System.Windows.Forms.Button()
		Me.cmdFix(iCleanupIndex) = New TabButton(doLabelFont, False)
		Me.cmdMark(iCleanupIndex) = New TabButton(doLabelFont, False)
		Me.lblErrors(iCleanupIndex) = New System.Windows.Forms.Label
		AddHandler nudSteps(iCleanupIndex).ValueChanged, AddressOf nudSteps_ValueChanged
		AddHandler dgvActions(iCleanupIndex).RowEnter, AddressOf dgvActions_RowEnter
		AddHandler dgvActions(iCleanupIndex).DataError, AddressOf dgvActions_DataError
		AddHandler Me.cmdFix(iCleanupIndex).Click, AddressOf cmdFix_Click
		AddHandler Me.cmdMark(iCleanupIndex).Click, AddressOf cmdMark_Click
		AddHandler Me.nudErrors(iCleanupIndex).ValueChanged, AddressOf nudErrors_ValueChanged
		AddHandler Me.cmdEraseCleanupErr(iCleanupIndex).Click, AddressOf cmdEraseCleanupErr_Click


		'
		'moaPanels(iPanelIndex)
		'
		With Me.doaPanels(iPanelIndex).Controls
			.Add(Me.nudSteps(iCleanupIndex))
			.Add(Me.nudErrors(iCleanupIndex))
			.Add(Me.cmdEraseCleanupErr(iCleanupIndex))
			.Add(Me.cmdFix(iCleanupIndex))
			.Add(Me.cmdMark(iCleanupIndex))
			.Add(Me.lblErrors(iCleanupIndex))
			.Add(Me.dgvActions(iCleanupIndex))
		End With




		'
		'dgvActions
		'
		With Me.dgvActions(iCleanupIndex)
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(4, 32)
			.Name = "dgvActions" & CStr(iCleanupIndex)
			.Font = doLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 24
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = doBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(294, 224)	'224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False


			


		End With


		'
		'cmdFix
		'
		With Me.cmdFix(iCleanupIndex)
			.Location = New System.Drawing.Point(48, 4)
			.Name = "cmdFix" & CStr(iCleanupIndex)
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 9
			'   .Font = moLabelFont
			.Text = "Fix"
		End With
		'
		'cmdMark
		'
		With Me.cmdMark(iCleanupIndex)
			.Location = New System.Drawing.Point(96, 4)
			.Name = "cmdMark" & CStr(iCleanupIndex)
			'     .Size = New System.Drawing.Size(48, 24)
			.TabIndex = 10
			'     .Font = moLabelFont
			.Text = "Mark"
		End With

		'
		'lblErrors
		'
		With Me.lblErrors(iCleanupIndex)
			.Location = New System.Drawing.Point(176, 6)
			.Name = "lblErrors" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(44, 24)
			.TabIndex = 29
			.Font = doLabelFont
			.Text = "Errors:"
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With



		'
		'nudSteps
		'
		With Me.nudSteps(iCleanupIndex)
			.Location = New System.Drawing.Point(4, 4)
			.Name = "nudSteps" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(32, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.One
			.Maximum = Decimal.One + Decimal.One
			.TabIndex = 19
			.Value = Decimal.One
			.Maximum = miStepNum
		End With
		'
		'nudErrors
		'
		With Me.nudErrors(iCleanupIndex)
			.Location = New System.Drawing.Point(216, 4)	 '232, 4
			.Name = "nudErrors" & CStr(iCleanupIndex)
			.Size = New System.Drawing.Size(42, 24)
			.Cursor = Cursors.Hand
			.Minimum = Decimal.Zero
			.Maximum = Decimal.Zero
			.TabIndex = 20
		End With
		'
		'cmdEraseCleanupErr
		'
		With Me.cmdEraseCleanupErr(iCleanupIndex)
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
			.Location = New System.Drawing.Point(260, 4)
			.Name = "cmdEraseCleanupErr"
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With

		Me.doaPanels(iPanelIndex).ResumeLayout(False)


	End Sub
	Private Sub zzInitPanel2()

		Me.grbSliverTolerance = New System.Windows.Forms.GroupBox()
		Me.txtToleranceMax = New System.Windows.Forms.TextBox()
		Me.txtToleranceMin = New System.Windows.Forms.TextBox()
		Me.lblToleranceMin = New System.Windows.Forms.Label()
		Me.lblToleranceMax = New System.Windows.Forms.Label()
		Me.chkRemoveSlivers = New System.Windows.Forms.CheckBox


		Me.lblResultMapLayerExists = New System.Windows.Forms.Label()
		Me.lblResultMapLayerCap = New System.Windows.Forms.Label()
		Me.lblResultMapLayer = New System.Windows.Forms.Label()

		Me.lblMapPgonCount = New System.Windows.Forms.Label()
		Me.txtMapPgonCount = New System.Windows.Forms.TextBox()

		Me.lblPgonsLayerName = New System.Windows.Forms.Label()
		Me.lblPgonsLayerExists = New System.Windows.Forms.Label()
		Me.lblPgonsLayerNameCap = New System.Windows.Forms.Label()



		'	Me.tstTopology.SuspendLayout()
		'		Me.SuspendLayout()
		'
		'doaPanels(iIndex)
		'
		With doaPanels(2)
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.Location = New System.Drawing.Point(0, 0)
			.Margin = New System.Windows.Forms.Padding(4)
			.Size = dtPanelSize
			''''''''''''''		.Name = "Panel2"
			.Controls.Add(Me.grbSliverTolerance)

			.Controls.Add(Me.lblResultMapLayerExists)
			.Controls.Add(Me.lblResultMapLayerCap)
			.Controls.Add(Me.lblResultMapLayer)

			.Controls.Add(Me.lblMapPgonCount)
			.Controls.Add(Me.txtMapPgonCount)

			.Controls.Add(Me.lblPgonsLayerName)
			.Controls.Add(Me.lblPgonsLayerExists)
			.Controls.Add(Me.lblPgonsLayerNameCap)
		End With

		'
		'grbSliverTolerance
		'
		With Me.grbSliverTolerance
			.Controls.Add(Me.txtToleranceMax)
			.Controls.Add(Me.txtToleranceMin)
			.Controls.Add(Me.lblToleranceMin)
			.Controls.Add(Me.lblToleranceMax)

			.Controls.Add(Me.chkRemoveSlivers)
			.Location = New System.Drawing.Point(8, 32)
			.Name = "grbSliverTolerance"
			.Size = New System.Drawing.Size(284, 72)
			.TabIndex = 7
			.TabStop = False
			.Text = "Sliver tolerance"
			.RightToLeft = Windows.Forms.RightToLeft.No
		End With
		'
		'txtToleranceMax
		'
		With Me.txtToleranceMax
			.Location = New System.Drawing.Point(208, 15)
			.Name = "txtToleranceMax"
			.Size = New System.Drawing.Size(48, 22)
			.TabIndex = 17
			.Text = "WWW"
		End With
		'
		'txtToleranceMin
		'
		With Me.txtToleranceMin
			.Location = New System.Drawing.Point(78, 15)
			.Name = "txtToleranceMin"
			.Size = New System.Drawing.Size(48, 22)
			.Text = ptMapThemeData.LinkLayers
			.TabIndex = 16
		End With
		'
		'lblToleranceMin
		'
		With Me.lblToleranceMin
			.Location = New System.Drawing.Point(16, 18)
			.Name = "lblToleranceMin"
			.Size = New System.Drawing.Size(60, 18)
			.TabIndex = 6
			.Text = "Minimum:123"
		End With
		'
		'lblToleranceMax
		'
		With Me.lblToleranceMax
			.Location = New System.Drawing.Point(144, 18)
			.Name = "lblToleranceMax"
			.Size = New System.Drawing.Size(62, 18)
			.TabIndex = 6
			.Text = "Maximum:123"
		End With
		'
		'chkRemoveSlivers
		'
		With Me.chkRemoveSlivers
			.Location = New System.Drawing.Point(32, 44)
			.Name = "chkRemoveSlivers"
			.Size = New System.Drawing.Size(144, 24)
			.TabIndex = 43
			.Font = doLabelFont
			.Text = "Don't remove slivers"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.RightToLeft = System.Windows.Forms.RightToLeft.No
			.TextAlign = ContentAlignment.MiddleLeft
		End With





		'
		'lblOverlayMapLayerExists
		'
		With Me.lblResultMapLayerExists
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(260, 120)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblOverlayMapLayerExists"
			.Size = New System.Drawing.Size(30, 25)
			.TabIndex = 21
		End With
		'
		'lblOverlayMapLayerCap
		'
		With Me.lblResultMapLayerCap
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 120)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblOverlayMapLayerCap"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 20
			.Text = "שכבת מפה:"
			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'lblOverlayMapLayer
		'
		With Me.lblResultMapLayer
			.Location = New System.Drawing.Point(34, 120)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblOverlayMapLayer"
			.Size = New System.Drawing.Size(108, 25)
			.TabIndex = 13
			.Text = "AA_" & ptMapThemeData.MapLayer
			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		End With

		'
		'lblMapPgonCount
		'
		With Me.lblMapPgonCount
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 148)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblMapPgonCount"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 24
			.Text = "מס' פוליגונים:"
			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'txtMapPgonCount
		'
		With Me.txtMapPgonCount
			.Location = New System.Drawing.Point(34, 148)
			.Margin = New System.Windows.Forms.Padding(4)
			.Name = "txtMapPgonCount"
			.ReadOnly = True
			.Size = New System.Drawing.Size(64, 22)
			.TabIndex = 22
		End With

		'
		'lblPgonsLayerExists
		'
		With Me.lblPgonsLayerExists
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(260, 176)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblPgonsLayerExists"
			.Size = New System.Drawing.Size(30, 25)
			.TabIndex = 27
		End With
		'
		'lblPgonsLayerNameCap
		'
		With Me.lblPgonsLayerNameCap
			'	.BackColor = System.Drawing.SystemColors.ControlLight
			.ForeColor = System.Drawing.SystemColors.ControlText
			.Location = New System.Drawing.Point(150, 176)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblPgonsLayerNameCap"
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.Size = New System.Drawing.Size(100, 25)
			.TabIndex = 26
			.Text = "פוליגונים סגורים:"
			.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		End With
		'
		'lblPgonsLayerName
		'
		With Me.lblPgonsLayerName
			.Location = New System.Drawing.Point(34, 176)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblMapLayer"
			.Size = New System.Drawing.Size(108, 25)
			.TabIndex = 25
			.Text = ptMapThemeData.MapLayer
			.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		End With


		'
		'me
		'

		Me.Controls.Add(Me.doaPanels(3))


	End Sub
	Private Sub zzInitPanel3()

		Me.txtPgonCountWA = New System.Windows.Forms.TextBox()
		Me.lblTopoExistsWA = New System.Windows.Forms.Label()
		Me.cmdStartErrWA = New System.Windows.Forms.Button()
		Me.cmdPrevErrWA = New System.Windows.Forms.Button()
		Me.cmdNextErrWA = New System.Windows.Forms.Button()
		Me.cmdEraseErrWA = New System.Windows.Forms.Button()

		Me.txtErrorCountWA = New System.Windows.Forms.TextBox()
		Me.txtErrorIndexWA = New System.Windows.Forms.TextBox()
		Me.lblTopoNameWA = New System.Windows.Forms.Label()
		Me.lblTopoNameCapWA = New System.Windows.Forms.Label()
		'	Me.tstTopology = New System.Windows.Forms.ToolStrip()
		Me.lblTopoErrorsWA = New System.Windows.Forms.Label()
		Me.grbCentroidsWA = New System.Windows.Forms.GroupBox()
		Me.txtCentroidCountWA = New System.Windows.Forms.TextBox()
		Me.txtCentroidBlocksWA = New System.Windows.Forms.TextBox()
		Me.txtCentroidLayersWA = New System.Windows.Forms.TextBox()
		Me.lblCentroidBlocksWA = New System.Windows.Forms.Label()
		Me.lblCentroidLayersWA = New System.Windows.Forms.Label()
		Me.grbLinksWA = New System.Windows.Forms.GroupBox()

		Me.txtLinkCountWA = New System.Windows.Forms.TextBox()
		Me.txtLinkLayersWA = New System.Windows.Forms.TextBox()
		Me.lblLinkLayersWA = New System.Windows.Forms.Label()

		Me.txtLineLinkCountB = New System.Windows.Forms.TextBox()
		Me.txtLineLinkLayers = New System.Windows.Forms.TextBox()
		Me.lblLineLinkLayers = New System.Windows.Forms.Label()

		Me.grbClosedPgonsWA = New System.Windows.Forms.GroupBox()
		Me.txtClosedPgonsCountWA = New System.Windows.Forms.TextBox()
		Me.txtClosedPgonsLayersWA = New System.Windows.Forms.TextBox()
		Me.lblClosedPgonsLayersWA = New System.Windows.Forms.Label()

		Me.txtToleranceWA = New System.Windows.Forms.TextBox()
		Me.lblToleranceWA = New System.Windows.Forms.Label()
		Me.chkHighlightSliverWA = New System.Windows.Forms.CheckBox()
		Me.chkCreateCentroidWA = New System.Windows.Forms.CheckBox()
		Me.doaPanels(4).SuspendLayout()
		Me.grbCentroidsWA.SuspendLayout()
		Me.grbLinksWA.SuspendLayout()
		Me.SuspendLayout()
		'
		'PanelX1
		'
		With Me.doaPanels(4).Controls
			.Add(Me.txtPgonCountWA)
			.Add(Me.lblTopoExistsWA)
			.Add(Me.cmdStartErrWA)
			.Add(Me.cmdPrevErrWA)
			.Add(Me.cmdNextErrWA)
			.Add(Me.cmdEraseErrWA)

			.Add(Me.txtErrorCountWA)
			.Add(Me.txtErrorIndexWA)
			.Add(Me.lblTopoNameWA)
			.Add(Me.lblTopoNameCapWA)
			.Add(Me.tstTopology)
			.Add(Me.lblTopoErrorsWA)
			.Add(Me.grbLinksWA)
			.Add(Me.grbCentroidsWA)
			.Add(Me.grbClosedPgonsWA)
			.Add(Me.txtToleranceWA)
			.Add(Me.lblToleranceWA)
			.Add(Me.chkHighlightSliverWA)
			.Add(Me.chkCreateCentroidWA)
		End With
		'
		'txtPgonCountWA
		'
		With Me.txtPgonCountWA
			.Location = New System.Drawing.Point(252, 78)
			.Name = "txtPgonCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 20
		End With
		'
		'lblTopoExistsWA
		'
		With Me.lblTopoExistsWA
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(8, 76)
			.Name = "lblTopoExistsWA"
			.Size = New System.Drawing.Size(20, 18)
			.TabIndex = 19
		End With
		'
		'cmdStartErrWA
		'
		With Me.cmdStartErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveFirstTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(152, 238 + 48)
			.Name = "cmdStartErrWA"
			.Size = New System.Drawing.Size(20, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdPrevErrWA
		'
		With Me.cmdPrevErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MovePrevTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(174, 238 + 48)
			.Name = "cmdPrevErrWA"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 17
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdNextErrWA
		'
		With Me.cmdNextErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources.MoveNextTr
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(192, 238 + 48)
			.Name = "cmdNextErrWA"
			.Size = New System.Drawing.Size(16, 22)
			.TabIndex = 18
			.UseVisualStyleBackColor = True
		End With
		'
		'cmdEraseErrWA
		'
		With Me.cmdEraseErrWA
			.BackgroundImage = Global.TopoUI.My.Resources.Resources._Erase
			.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
			.Location = New System.Drawing.Point(216, 238 + 48)
			.Name = "cmdEraseErrWA"
			.Size = New System.Drawing.Size(24, 22)
			.TabIndex = 16
			.UseVisualStyleBackColor = True
		End With
		'
		'txtErrorCountWA
		'
		With Me.txtErrorCountWA
			.Location = New System.Drawing.Point(102, 238 + 48)
			.Name = "txtErrorCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(42, 22)
			.TabIndex = 15
		End With
		'
		'txtErrorIndexWA
		'
		With Me.txtErrorIndexWA
			.Location = New System.Drawing.Point(60, 238 + 48)
			.Name = "txtErrorIndexWA"
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 14
			.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
		End With
		'
		'lblTopoNameWA
		'
		With Me.lblTopoNameWA
			.Location = New System.Drawing.Point(126, 78)
			.Name = "lblTopoNameWA"
			.Size = New System.Drawing.Size(120, 18)
			.Text = ptOverlayMapThemeData.TopoName
			.TabIndex = 13
		End With
		'
		'lblTopoNameCapWA
		'
		With Me.lblTopoNameCapWA
			.Location = New System.Drawing.Point(28, 78)
			.Name = "lblTopoNameCapWA"
			.Size = New System.Drawing.Size(96, 18)
			.TabIndex = 12
			.Text = "Topology name:"
		End With

		'
		'lblTopoErrorsWA
		'
		With Me.lblTopoErrorsWA
			.Location = New System.Drawing.Point(8, 238 + 48 + 24)	 '
			.Name = "lblTopoErrorsWA"
			.Size = New System.Drawing.Size(50, 18)
			.TabIndex = 9
			.Text = "Errors:"
			.BorderStyle = BorderStyle.FixedSingle
		End With
		'
		'grbCentroidsWA
		'
		With Me.grbCentroidsWA
			.Controls.Add(Me.txtCentroidCountWA)
			.Controls.Add(Me.txtCentroidBlocksWA)
			.Controls.Add(Me.txtCentroidLayersWA)
			.Controls.Add(Me.lblCentroidBlocksWA)
			.Controls.Add(Me.lblCentroidLayersWA)
			.Location = New System.Drawing.Point(8, 168)
			.Name = "grbCentroidsWA"
			.Size = New System.Drawing.Size(284, 64)
			.TabIndex = 8
			.TabStop = False
			.Text = "Centroids"
		End With
		'
		'txtCentroidCountWA
		'
		With Me.txtCentroidCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtCentroidCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 19
		End With
		'
		'txtCentroidBlocksWA
		'
		With Me.txtCentroidBlocksWA
			.Location = New System.Drawing.Point(99, 39)
			.Name = "txtCentroidBlocksWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(135, 22)
			.Text = ptOverlayMapThemeData.CentroidBlocks
			.TabIndex = 18
		End With
		'
		'txtCentroidLayersWA
		'
		With Me.txtCentroidLayersWA
			.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtCentroidLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptOverlayMapThemeData.CentroidLayers
			.TabIndex = 17
		End With
		'
		'lblCentroidBlocksWA
		'
		With Me.lblCentroidBlocksWA
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblCentroidBlocksWA"
			.Size = New System.Drawing.Size(78, 18)
			.TabIndex = 7
			.Text = "Block names:"
		End With
		'
		'lblCentroidLayersWA
		'
		With Me.lblCentroidLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblCentroidLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.ForeColor = Color.Brown
		End With
		'
		'grbLinksWA
		'
		With Me.grbLinksWA
			.Controls.Add(Me.txtLinkCountWA)
			.Controls.Add(Me.txtLinkLayersWA)
			.Controls.Add(Me.lblLinkLayersWA)
			.Controls.Add(Me.txtLineLinkCountB)
			.Controls.Add(Me.txtLineLinkLayers)
			.Controls.Add(Me.lblLineLinkLayers)

			.Location = New System.Drawing.Point(8, 96)
			.Name = "grbLinksWA"
			.Size = New System.Drawing.Size(284, 67)
			.TabIndex = 7
			.TabStop = False
			.Text = "Links"
		End With
		'
		'txtLinkCountWA
		'
		With Me.txtLinkCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtLinkCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtLinkLayersWA
		'
		With Me.txtLinkLayersWA
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtLinkLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptOverlayMapThemeData.LinkLayers
			.TabIndex = 16
		End With
		'
		'lblLinkLayersWA
		'
		With Me.lblLinkLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblLinkLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.ForeColor = Color.Blue
		End With

		'
		'txtLineLinkCountB
		'
		With Me.txtLineLinkCountB
			.Location = New System.Drawing.Point(238, 39)
			.Name = "txtLineLinkCountB"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtLineLinkLayers
		'
		With Me.txtLineLinkLayers
			.Location = New System.Drawing.Point(84, 39)
			.Name = "txtLineLinkLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(160, 46)
			.Text = ptOverlayMapThemeData.LineLinkLayers
			.TabIndex = 16
		End With
		'
		'lblLineLinkLayers
		'
		With Me.lblLineLinkLayers
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblLineLinkLayers"
			.Size = New System.Drawing.Size(66, 18)
			.TabIndex = 6
			.Text = "Line Layer:"
		End With
		''''''''''''''''''''''''''''''''''''''''''''''''''
		'
		'grbClosedPgonsWA
		'
		With Me.grbClosedPgonsWA
			.Controls.Add(Me.txtClosedPgonsCountWA)
			.Controls.Add(Me.txtClosedPgonsLayersWA)
			.Controls.Add(Me.lblClosedPgonsLayersWA)
			.Location = New System.Drawing.Point(8, 213 + 24)
			.Name = "grbClosedPgonsWA"
			.Size = New System.Drawing.Size(284, 43)
			.TabIndex = 17
			.TabStop = False
			.Text = "Closed polygons"
		End With
		'
		'txtClosedPgonsCountWA
		'
		With Me.txtClosedPgonsCountWA
			.Location = New System.Drawing.Point(238, 15)
			.Name = "txtClosedPgonsCountWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(40, 22)
			.TabIndex = 17
		End With
		'
		'txtClosedPgonsLayersWA
		'
		With Me.txtClosedPgonsLayersWA
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtClosedPgonsLayersWA"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = ptOverlayMapThemeData.ClosedPgonsLayers
			.TabIndex = 16
		End With
		'
		'lblClosedPgonsLayersWA
		'
		With Me.lblClosedPgonsLayersWA
			.Location = New System.Drawing.Point(20, 18)
			.Name = "lblClosedPgonsLayersWA"
			.Size = New System.Drawing.Size(45, 18)
			.TabIndex = 6
			.Text = "Layers:"
			.ForeColor = Color.Green
		End With

		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

		'
		'txtToleranceWA
		'
		With Me.txtToleranceWA
			.Location = New System.Drawing.Point(240, 32)
			.Name = "txtToleranceWA"
			.Size = New System.Drawing.Size(52, 22)
			.TabIndex = 3
			.Text = "0.01"
		End With
		'
		'lblToleranceWA
		'
		With Me.lblToleranceWA
			.Location = New System.Drawing.Point(170, 32)
			.Name = "lblToleranceWA"
			.Size = New System.Drawing.Size(68, 18)
			.TabIndex = 2
			.Text = "Tolerance"
		End With
		'
		'chkHighlightSliverWA
		'
		With Me.chkHighlightSliverWA
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 52)
			.Name = "chkHighlightSliverWA"
			.Size = New System.Drawing.Size(105, 18)
			.TabIndex = 1
			.Text = "Highlight Sliver"
			.UseVisualStyleBackColor = True
		End With
		'
		'chkCreateCentroidWA
		'

		With Me.chkCreateCentroidWA
			.AutoSize = True
			.Location = New System.Drawing.Point(8, 32)
			.Name = "chkCreateCentroidWA"
			.Size = New System.Drawing.Size(108, 18)
			.TabIndex = 0
			.Text = "Insert Centroid"
			.UseVisualStyleBackColor = True
		End With
		Me.doaPanels(4).ResumeLayout(False)
		Me.doaPanels(4).PerformLayout()
		Me.grbCentroidsWA.ResumeLayout(False)
		Me.grbCentroidsWA.PerformLayout()
		Me.grbLinksWA.ResumeLayout(False)
		Me.grbLinksWA.PerformLayout()
		Me.ResumeLayout(False)
	End Sub
	Private Function zzGetMapThemeData(iIndex As Integer) As DMAcadExt.MapThemeData
		Select Case iIndex
			Case 0
				Return ptSourceMapThemeData
			Case 5
				Return ptOverlayMapThemeData

			Case Else
				Return New DMAcadExt.MapThemeData
		End Select

	End Function
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tSourceMapThemeData As DMAcadExt.MapThemeData, tOverlayMapThemeData As DMAcadExt.MapThemeData, Optional tOverlayMapThemeData_A As DMAcadExt.MapThemeData = Nothing)
		MyBase.New(tMapThemeData)
		'''''''''''''''	ptMapThemeData = tMapThemeData
		'	MessageBox.Show(tMapThemeData.LinkLayer & vbCrLf & "1:" & tSourceMapThemeData.LinkLayer & vbCrLf & tOverlayMapThemeData.LinkLayer, "08_350")
		ptSourceMapThemeData = tSourceMapThemeData
		ptOverlayMapThemeData = tOverlayMapThemeData
		msAddTopoLayer = ptOverlayMapThemeData.LinkLayer.ToLower & "a"
		ptOverlayMapThemeData.AddLinkLayers(msAddTopoLayer)


		mbAddOverlayExists = tOverlayMapThemeData_A.IsNotEmpty
		If mbAddOverlayExists Then
			ptOverlayMapThemeData_A = tOverlayMapThemeData_A
		End If
		MyBase.SetLabelDim(miThisStagesUB)

		' This call is required by the designer.
		InitializeComponent()
		zzCheckRes()
		'	
	End Sub
	Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbCreateTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 4
						zzCreateTopo(0)
				End Select

			Case Me.tsbCheckTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzCheckTopo(LabelCheck.CurrentIndex, True)
				End Select

			Case Me.tsbDeleteTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzDeleteTopo(LabelCheck.CurrentIndex)

						'	zzRemoveMapLayer(LabelCheck.CurrentIndex)
						'	zzCheckMapLayer(LabelCheck.CurrentIndex)
						'	Me.zzDeleteTopo(LabelCheck.CurrentIndex)
					Case 3
						zzClearResLayer()
						zzCheckRes()
						zzDispRes(True)
				End Select

			Case Me.tsbExec.Name
				MessageBox.Show(CStr(LabelCheck.CurrentIndex) & vbCrLf & "", "02_222a")
				Select Case LabelCheck.CurrentIndex
					Case 0
						zzCreateMapLayer(True)
					Case 1
						zzCreateMapLayer(False)
					Case 2
						If miResPgonCount = 0 Then
							zzSaveFDOSliverToleranceSetting()
							zzUnion()
						End If
					Case 3
						zzBreakCreate()
					Case 4, 5
						zzTopoToClosedPgons()
				End Select

			Case Me.tsbClear.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1
						zzRemoveMapLayer(LabelCheck.CurrentIndex)
						zzCheckMapLayer(LabelCheck.CurrentIndex)
						'	Me.zzDeleteTopo(LabelCheck.CurrentIndex)
					Case 2
						zzRemoveMapLayer(0)
						zzRemoveMapLayer(1)
						zzCheckMapLayer(0)
						zzCheckMapLayer(1)
				End Select

		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Private Sub zzAddTopoPgon()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptOverlayMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
		If oTopoModel IsNot Nothing Then
			Dim oCentroidCreationSettings As Autodesk.Gis.Map.Topology.PointCreationSettings = New Autodesk.Gis.Map.Topology.PointCreationSettings(ptOverlayMapThemeData.CentroidLayer, 256, False, ptOverlayMapThemeData.CentroidBlock)
			oTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
			Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
			Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
			Dim colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()


			ptEntRes = oEditor.GetEntity(oPromptOpt)
			If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				colAcObjIDs.Add(ptEntRes.ObjectId)
				Try
					oTopoModel.AddPolygons(colAcObjIDs)
				Catch oEx As Exception

				End Try

			End If
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zzBreakCreate()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sTopoName As String = ptOverlayMapThemeData.TopoName
		Dim sBaseLayers As String = ptOverlayMapThemeData.LinkLayers
		Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		System.Windows.Forms.MessageBox.Show(sTopoName & vbCrLf & sBaseLayers & vbCrLf & ptOverlayMapThemeData.LinkLayers & vbCrLf & ptSourceMapThemeData.CentroidBlocks & vbCrLf & ptSourceMapThemeData.CentroidLayers, "zzClearMapLayer - 01_819")
		TopoManager.TopoCreator.DeleteTopology(sTopoName, False, False, True)
	

		Dim oaActionVar() As TopoManager.ActionVar = {New TopoManager.ActionVar(2, 0.05), New TopoManager.ActionVar(8, 0.05)}
		Dim oaErrorPoints(1) As DMAcadExt.TplnPointArray
		colLinks = DMAcadExt.AcadTransaction.GetLinks(ptOverlayMapThemeData.LinkLayers)
		System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count) & vbCrLf & "", "zzClearMapLayer - 01_821")
		TopoManager.TopoCreator.Cleanup(oaActionVar, sBaseLayers, "", colLinks, True, oaErrorPoints)

      Dim tTopoRes As DMAcadExt.TopoRes = TopoManager.TopoCreator.CreateTopology(sTopoName, ptOverlayMapThemeData.LinkLayers, "", ptSourceMapThemeData.CentroidBlocks, ptSourceMapThemeData.CentroidLayers, False, String.Empty, String.Empty, False, False, 0.05)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer("pclp004", DMAcadExt.DMApp.AppID, True, True)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		Return
	End Sub
	Private Sub zzGetSliverToleranceSetting()
		Dim oMySettings As My.MySettings = New My.MySettings()
		Dim bAllSlivers As Boolean = oMySettings.AllSlivers

		Me.chkRemoveSlivers.Checked = bAllSlivers
		Me.txtToleranceMin.Enabled = Not bAllSlivers
		Me.txtToleranceMax.Enabled = Not bAllSlivers
		Me.txtToleranceMin.Text = CStr(oMySettings.MinFDOSliverTolerance)
		Me.txtToleranceMax.Text = CStr(oMySettings.MaxFDOSliverTolerance)

	End Sub
	Private Sub zzRemoveMapLayer(ByVal iIndex As Integer)
		Dim oMapThemeData As DMAcadExt.MapThemeData = zzGetMapThemeData(iIndex)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		'	System.Windows.Forms.MessageBox.Show(oTopoDef.FDOConnectionName & vbCrLf & oTopoDef.FDOLayerName, "zzClearMapLayer - 01_821")
		FDO.FDO_Manager.RemoveConnectionB(oMapThemeData.ShapeConnection)
		FDO.FDO_Manager.RemoveResource(oMapThemeData.ShapeConnection, oMapThemeData.MapLayer)
		DMAcadExt.AcadDocument.Unlock()


	End Sub
	Private Sub zzSaveFDOSliverToleranceSetting()
		Dim oMySettings As My.MySettings = New My.MySettings()
		Try
			Dim bOK As Boolean
			oMySettings.AllSlivers = Me.chkRemoveSlivers.Checked

			If IsNumeric(Me.txtToleranceMax.Text) Then

				oMySettings.MinFDOSliverTolerance = Convert.ToDouble(Me.txtToleranceMin.Text)
				If IsNumeric(Me.txtToleranceMin.Text) Then
					oMySettings.MaxFDOSliverTolerance = Convert.ToDouble(Me.txtToleranceMax.Text)
				Else
					bOK = False
				End If
				bOK = True
			Else
				bOK = True
			End If
			If bOK Then
				oMySettings.Save()
			Else
				System.Windows.Forms.MessageBox.Show("Input is not numeric", "frmFDO_Overlay - zzSaveFDOSliverToleranceSetting")
			End If

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmFDO_Overlay - zzSaveFDOSliverToleranceSetting")
		End Try

	End Sub

	Private Sub zzCreateTopo(ByVal iIndex As Integer)
		Dim dTolerance As Double
		Dim bCreateCentroids As Boolean = False
		Dim tTopoRes As DMAcadExt.TopoRes

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)


		dTolerance = 0.01

		Try
			MessageBox.Show(ptOverlayMapThemeData.TopoName & vbCrLf & ptOverlayMapThemeData.LinkLayers & vbCrLf & ptOverlayMapThemeData.LineLinkLayers & vbCrLf & zzGetMapThemeData(iIndex).CentroidBlocks & vbCrLf & ptOverlayMapThemeData.CentroidLayers, "09_450")
         tTopoRes = TopoManager.TopoCreator.CreateTopology(ptOverlayMapThemeData.TopoName, ptOverlayMapThemeData.LinkLayers, ptOverlayMapThemeData.LineLinkLayers, ptOverlayMapThemeData.CentroidBlocks, ptOverlayMapThemeData.CentroidLayers, bCreateCentroids, String.Empty, String.Empty, False, True, dTolerance)
			If tTopoRes.TopoExists Then
				Me.txtPgonCountWA.Text = CStr(tTopoRes.PgonCount)


				'	zzDispTopoExists(iIndex, True)

				zzDispTopoExistsWA(True)
			Else
				'	zzDispTopoExists(iIndex, False)
				zzDispTopoExistsWA(False)

			End If

		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmAnaliticClip - zzCreateTopo_01")
		End Try


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default


	End Sub

	Private Sub zzDispTopoOK(ByVal iIndex As Integer, tTopoRes As DMAcadExt.TopoRes)
		Dim bOK As Boolean = tTopoRes.IsOK
		'	Me.doaLabelCheck(iIndex).Checked = bOK
		Me.lblTopoExists(iIndex).Visible = bOK
		Me.txtPgonCount(iIndex).Enabled = bOK
		Me.txtPgonCount(iIndex).Text = Convert.ToString(tTopoRes.PgonCount)
		If bOK AndAlso tTopoRes.PgonCount > 0 Then
			Me.lblTopoName(iIndex).ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoName(iIndex).Font = doLabelBoldFont
		Else
			Me.lblTopoName(iIndex).ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoName(iIndex).Font = doLabelFont
		End If
	End Sub

	Private Sub zzDispTopoExists(ByVal iIndex As Integer, bExists As Boolean)
		Me.lblTopoExists(iIndex).Visible = bExists
		Me.txtPgonCount(iIndex).Enabled = bExists
		If bExists Then
			Me.lblTopoName(iIndex).ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoName(iIndex).Font = doLabelBoldFont
		Else
			Me.lblTopoName(iIndex).ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoName(iIndex).Font = doLabelFont
		End If
	End Sub
	Private Sub zzDispTopoExistsWA(bExists As Boolean)
		Me.lblTopoExistsWA.Visible = bExists
		Me.txtPgonCountWA.Enabled = bExists
		If bExists Then
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblTopoNameWA.Font = doLabelBoldFont
		Else
			Me.lblTopoNameWA.ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblTopoNameWA.Font = doLabelFont
		End If
	End Sub
	Private Sub zzDispMapLayerExists(ByVal iIndex As Integer, bExists As Boolean)
		Me.lblMapLayerExists(iIndex).Visible = bExists
		Me.doaLabelCheck(iIndex).Checked = bExists

		'	Me.txtPgonCount(iIndex).Enabled = bExists
		If Me.doaLabelCheck(iIndex).Checked Then
			Me.lblMapLayer(iIndex).ForeColor = System.Drawing.SystemColors.ControlText
			Me.lblMapLayer(iIndex).Font = doLabelBoldFont
		Else
			Me.lblMapLayer(iIndex).ForeColor = System.Drawing.SystemColors.GrayText
			Me.lblMapLayer(iIndex).Font = doLabelFont
		End If
		'	MessageBox.Show(CStr(bExists) & ":" & CStr(iIndex) & vbCrLf & Me.doaLabelCheck(iIndex).Text & vbCrLf & CStr(Me.doaLabelCheck(iIndex).Checked) & vbCrLf & CStr(Me.doaLabelCheck(iIndex).Enabled), "05_120D")
	End Sub
	Private Sub zzCheckTopo(ByVal iIndex As Integer, ByVal bMsg As Boolean)
		Dim oMapThemeData As DMAcadExt.MapThemeData = zzGetMapThemeData(iIndex)
		Dim tTopoRes As DMAcadExt.TopoRes

		If oMapThemeData.GraphType = DMAcadExt.enGraphType.Topology Then
			tTopoRes = TopoManager.TopoCreator.CheckTopo(oMapThemeData.LineTopoName, True, bMsg)
		ElseIf oMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
			tTopoRes = TopoManager.TopoCreator.CheckMPgons(oMapThemeData.MPgonLayers, True)
		End If

		'	MessageBox.Show("Locked: " & CStr(DMAcadExt.AcadDocument.IsLocked) & vbCrLf & CStr(tTopoRes.IsInstance) & vbCrLf & oMapThemeData.GraphType.ToString(), "04_451")

		'	zzDispTopoExists(iIndex, tTopoRes.TopoExists)
		zzDispTopoOK(iIndex, tTopoRes)
		'
	End Sub

	Private Sub zzCheckMapLayer(ByVal iIndex As Integer)
		Dim bMapLayerExists As Boolean = zzMapLayerExists(iIndex)
		zzDispMapLayerExists(iIndex, bMapLayerExists)
	End Sub
	Private Sub zzDeleteTopo(ByVal iIndex As Integer)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		TopoManager.TopoCreator.DeleteTopology(zzGetMapThemeData(iIndex).LineTopoName, False, False)
		zzCheckTopo(iIndex, True)
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzClearResLayer()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		DMAcadExt.AcadTransaction.ClearLayerList(ptMapThemeData.MapLayer)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzCreateMapLayer(ByVal bSource As Boolean)

		Dim oMapThemeData As DMAcadExt.MapThemeData
		If bSource Then
			oMapThemeData = ptSourceMapThemeData
		Else
			oMapThemeData = ptOverlayMapThemeData
		End If
		Dim sTopoName As String = oMapThemeData.TopoName
		Dim sExpCondition As String
		If oMapThemeData.GraphType = DMAcadExt.enGraphType.Topology Then
			sExpCondition = sTopoName
		ElseIf oMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
			sExpCondition = oMapThemeData.ClosedPgonsLayers
			'	sExpCondition = oMapThemeData.MPgonLayers
			'	sExpCondition = oMapThemeData.LinkLayers
			'sExpCondition = "TplnParcelMPgon"
		Else
			sExpCondition = Nothing
		End If
		Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
		Dim s As String = sExpCondition
		s &= vbCrLf & oMapThemeData.GraphType.ToString()
		'	s &= vbCrLf & sTopoName
		s &= vbCrLf & oMapThemeData.ShapeConnection
		s &= vbCrLf & oMapThemeData.MapLayer
      '	MessageBox.Show("Layer: " & sExpCondition, "01_659z")
		oFDO_Manager.CreateMapLayer(sExpCondition, oMapThemeData.GraphType, oMapThemeData.MapLayer, oMapThemeData.ShapeConnection, "")
		'MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & s, "01_999t")
		Me.tmrDelay.Enabled = True
		miCurrentIndexForDelay = 0
		'	bMapLayerExists = zzMapLayerExists(iIndex)
		'	zzDispMapLayerExists(iIndex, bMapLayerExists)
	End Sub
	Private Function zzMapLayerExists(ByVal iIndex As Integer) As Boolean
		Dim sFDOLayerName As String = zzGetMapThemeData(iIndex).MapLayer
		If sFDOLayerName IsNot Nothing Then
			'MessageBox.Show(sFDOLayerName, "05_430")
			Return FDO.FDO_Manager.LayerExists(sFDOLayerName)
		Else
			''''''MessageBox.Show("sFDOLayerName Is Nothing" & vbCrLf & CStr(iIndex), "05_440")
		End If

	End Function
	Private Sub zzMyInitializeComponent()
		Dim saCaptions() As String = {"בניית שכבת מקור", "בניית שכבת חיתוך'", "חיתוך שכבות", "הכנה לטופולוגיה", "טופולוגיה", "פוליגונים סגורים"}
		MyBase.psaCaptions = saCaptions
		MyBase.piLabelTop = 40
		MyBase.OnNew()
		Me.txtTopoErrors = New TextBox
		Me.lblTopoErrors = New Label
		Me.lblCaption = New Label

		For iIndex As Integer = 0 To 1
			zzInitPanel0_1(iIndex)
			zzCheckTopo(iIndex, False)
			zzCheckMapLayer(iIndex)
		Next
		zzInitToolStrip()

		AfterChangeCurrent(0)

		zzInitPanel0_3(0)
		zzInitPanel2()
		zzInitPanel3()	 'Topo
		zzSetGridColumns(0)
		zzGetSliverToleranceSetting()
	End Sub
	Private Sub zzAddNeigborsOldA(sLayerName As String)
		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(ptOverlayMapThemeData.TopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
		If oTopoModel IsNot Nothing Then
			Dim oXDataParcel As DMAcadExt.TplnXDataParcel
			Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
			Dim colTopoPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oTopoModel.GetPolygons()
			Dim oMPgons As MPgons = New MPgons()
			'	oMPgons.LoadAll("TplnParcelMPgon")
			oMPgons.LoadAll("1602")
			MessageBox.Show(ptOverlayMapThemeData.TopoName & vbCrLf & ptOverlayMapThemeData.LineTopoName & vbCrLf & CStr(oMPgons.Count) & vbCrLf & CStr(colTopoPolygons.Count) & vbCrLf & sLayerName, "03_300")

			For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colTopoPolygons
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(oPolygon.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				oXDataParcel = New DMAcadExt.TplnXDataParcel(oDBObject.XData)
				MessageBox.Show(oDBObject.Handle.ToString() & vbCrLf & oXDataParcel.NeigborList & vbCrLf & CStr(oMPgons.InnerColCount), "03_307")
				oMPgons.AddToInnerCol(oXDataParcel.NeigborList)
			Next
			oMPgons.SetLayer(sLayerName)
		End If
	End Sub
	Private Sub zzUnion021214()
		Dim sSourceLayer As String, sOverlayLayer As String, sResultLayer As String
		Dim sOverlayLayer_A As String = Nothing, sResultLayer_A As String = Nothing
		Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
		Dim oMySettings As My.MySettings = New My.MySettings()
		Dim bAllSlivers As Boolean = oMySettings.AllSlivers
		Dim dMinFDOSliverTolerance, dMaxFDOSliverTolerance As Double
		Dim sSHPFileName As String = ":"

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		If bAllSlivers Then
			dMinFDOSliverTolerance = 0.0
			dMaxFDOSliverTolerance = -1.0
		Else
			'	dMinFDOSliverTolerance = MinFDOSliverTolerance
			dMinFDOSliverTolerance = oMySettings.MinFDOSliverTolerance
			dMaxFDOSliverTolerance = oMySettings.MaxFDOSliverTolerance
		End If

		sSourceLayer = ptSourceMapThemeData.MapLayer
		sOverlayLayer = ptOverlayMapThemeData.MapLayer
		sResultLayer = ptMapThemeData.MapLayer
		If mbAddOverlayExists Then
			sOverlayLayer_A = ptOverlayMapThemeData_A.MapLayer
			sResultLayer_A = sResultLayer_A & "_A"
		End If

		'	DMAcadExt.AcadDocument.WriteMessage("!!!NB_4:" & System.Windows.Forms.Application.LocalUserAppDataPath)
		'	sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
		'	MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer, "08_377")
		Try
			DMAcadExt.AcadTransaction.OpenHandleDictionary()
			'	dicRes = GetOverlayData(sSHPFileName, sClassName, bFromTopologia)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSHPFileName, "04_242")
			'
		End Try

		Dim colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = oFDO_Manager.GetClipAcObjIds(sSourceLayer, sOverlayLayer)
		Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
		Dim tLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.ParcelCP_Erased)
		Dim bLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, False)
		Dim oXDataParcel As DMAcadExt.TplnXDataParcel
		Dim oMPgons As MPgons = New MPgons()
		oMPgons.LoadAll("1602")

		If colAcObjIDs IsNot Nothing Then
			MessageBox.Show(CStr(colAcObjIDs.Count) & vbCrLf & sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & tLayerDef.Name, "08_392")
			For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colAcObjIDs
				oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				'	oEntity.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Magenta)
				oEntity.Layer = tLayerDef.Name
				oXDataParcel = New DMAcadExt.TplnXDataParcel(oEntity.XData)
				'MessageBox.Show(oXDataParcel.NeigborList & vbCrLf & "", "08_399")
				oMPgons.AddToInnerCol(oXDataParcel.NeigborList)
			Next
			oMPgons.SetLayer(tLayerDef.Name)
		Else
			miResPgonCount = 0
			Me.lblResultMapLayerExists.Visible = False
		End If
		'	zzAddNeigborsOld(tLayerDef.Name)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		zzDispRes(False)
	End Sub
	Private Sub zzUnion()
		Dim sSourceLayer As String, sOverlayLayer As String, sResultLayer As String
		Dim sOverlayLayer_A As String = Nothing, sResultLayer_A As String = Nothing
		Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
		Dim oMySettings As My.MySettings = New My.MySettings()
		Dim bAllSlivers As Boolean = oMySettings.AllSlivers
		Dim dMinFDOSliverTolerance, dMaxFDOSliverTolerance As Double
		Dim sSHPFileName As String = ":"

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

		If bAllSlivers Then
			dMinFDOSliverTolerance = 0.0
			dMaxFDOSliverTolerance = -1.0
		Else
			'	dMinFDOSliverTolerance = MinFDOSliverTolerance
			dMinFDOSliverTolerance = oMySettings.MinFDOSliverTolerance
			dMaxFDOSliverTolerance = oMySettings.MaxFDOSliverTolerance
		End If

		sSourceLayer = ptSourceMapThemeData.MapLayer
		sOverlayLayer = ptOverlayMapThemeData.MapLayer
		sResultLayer = ptMapThemeData.MapLayer
		If mbAddOverlayExists Then
			sOverlayLayer_A = ptOverlayMapThemeData_A.MapLayer
			sResultLayer_A = sResultLayer_A & "_A"
		End If

		'	DMAcadExt.AcadDocument.WriteMessage("!!!NB_4:" & System.Windows.Forms.Application.LocalUserAppDataPath)
		'	sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
		'	MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer, "08_377")
		Try
			DMAcadExt.AcadTransaction.OpenHandleDictionary()
			'	dicRes = GetOverlayData(sSHPFileName, sClassName, bFromTopologia)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sSHPFileName, "04_242")
			'
		End Try

		Dim colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = oFDO_Manager.GetClipAcObjIds(sSourceLayer, sOverlayLayer)
        'Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
		Dim tLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.ParcelCP_Erased)
		Dim bLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, False)
        '	Dim oXDataParcel As DMAcadExt.TplnXDataParcel
		'	Dim oMPgons As MPgons = New MPgons()
		'oMPgons.LoadAll("1602")

		If colAcObjIDs IsNot Nothing Then
         Dim oPgonSet As FDO.TplnPolygonSet = New FDO.TplnPolygonSet(DMAcadExt.enMapTheme.Parcels)
			MessageBox.Show(CStr(colAcObjIDs.Count) & vbCrLf & sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & tLayerDef.Name, "08_392")
			Dim colPolylines As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks("1602")
			Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefsNew("1603", "1603")

			MessageBox.Show(CStr(colPolylines.Count) & vbCrLf & CStr(colBlockRefs.Count), "08_394")
			oPgonSet.Load(colPolylines, colBlockRefs)
         oPgonSet.SelectAnalitic(colAcObjIDs, "PrcAnalytic", msAddTopoLayer)
		
		Else
			miResPgonCount = 0
			Me.lblResultMapLayerExists.Visible = False
		End If
		'	zzAddNeigborsOld(tLayerDef.Name)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		zzDispRes(False)
	End Sub
	Private Sub zzDispRes(bResultMapLayer As Boolean)
		If miResPgonCount = 0 Then
			Me.lblPgonsLayerExists.Visible = False
			Me.txtMapPgonCount.Text = String.Empty
			If bResultMapLayer Then
				Me.lblResultMapLayerExists.Visible = False
			End If
			Me.doaLabelCheck(3).Checked = False
		Else
			Me.lblPgonsLayerExists.Visible = True
			Me.txtMapPgonCount.Text = CStr(miResPgonCount)
			If bResultMapLayer Then
				Me.lblResultMapLayerExists.Visible = True
			End If
			Me.doaLabelCheck(3).Checked = True
		End If
	End Sub

	
	Protected Overrides Sub AfterChangeCurrent(iNewIndex As Integer)
		MyBase.OnChangeCurrent(iNewIndex)
		Select Case iNewIndex
			Case 0, 1, 2, 4, 5
				Me.doaPanels(iNewIndex).Controls.Add(Me.tstTopology)
			Case 3
				Me.doaPanels(iNewIndex).Controls.Remove(Me.tstTopology)
		End Select
		If False And iNewIndex = 4 Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer("PCLP004", DMAcadExt.DMApp.AppID, True, True)
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'	zzTestData("AfterChangeCurrent " & CStr(iNewIndex))
	End Sub
	Private Sub zzCheckRes()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		If DMAcadExt.AcadTransaction.LayerExists(ptMapThemeData.MapLayer) Then
			mdicPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(ptMapThemeData.MapLayer, TopoManager.TPlanGraph.TplnProject.XDataAppName, False)
		Else
			mdicPolygons = Nothing
		End If
		If mdicPolygons IsNot Nothing Then
			miResPgonCount = mdicPolygons.Count
		Else
			miResPgonCount = 0
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Public Overrides ReadOnly Property IsDone As Boolean
		Get

			Return (miResPgonCount <> 0)
		End Get
	End Property
	Public Overrides ReadOnly Property IsDoneA As Boolean
		Get
			Return (miResPgonCount <> 0)
		End Get
	End Property
	Private Sub zzParcelPgonsExp(ByVal iIndex As Integer, sFDOConnectionName As String)	 ', sFDOLayerName As String
		'	Dim oTopoDef As DMAcadExt.TopoDef '= zzLoadTopoDef()
		Dim sTopoName As String = zzGetMapThemeData(iIndex).LineTopoName
		Dim oFDO_Manager As FDO.FDO_Manager
		TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.ClosedPolygons
		MessageBox.Show(TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod.ToString(), "10_130")
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()




		'	Dim oExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
		'		Dim tRes As Autodesk.Gis.Map.ImportExport.ExportResults
		'	Dim sTopoName As String = oTopoDef.Name
		Dim sShapeFileName As String = System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sTopoName & "\" & sTopoName & ".shp"
		Dim oDirectoryInfo As IO.DirectoryInfo = New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sTopoName)
		oFDO_Manager = New FDO.FDO_Manager()
		If Not oDirectoryInfo.Exists Then
			oDirectoryInfo.Create()
		End If
		'	sShapeFileName = oDirectoryInfo.FullName & "\" & oTopoDef.Name & ".shp"

		'	sShapeFileName = oTopoDef.GetLocalFileName(".shp")
		sShapeFileName = DMCommon.Functions.GetLocalFileNameInDir(sTopoName, ".shp")
		'	Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection

		System.Windows.Forms.MessageBox.Show(sShapeFileName, "01_802")
		Dim oShapeExpImp As FDO.ShapeExpImp = New FDO.ShapeExpImp(FDO.enExpImp.Export, sShapeFileName)
		oShapeExpImp.FromPoligons("TplnParcelMPgon")
      oShapeExpImp.AddShapeData(1)
		'''''''''	oShapeExpImp.AddLayerFilter("TplnParcelMPgon")
		oShapeExpImp.Exec()

		Try
			oFDO_Manager.ConnectToShape(sFDOConnectionName, sShapeFileName)
			oFDO_Manager.AddLayerToMap(sFDOConnectionName)

		Catch oMapEx As Autodesk.Gis.Map.MapException
			System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace, "01_818")
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - tsiBuildTopo_Click" & ": ")
		End Try

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub zzTopoToClosedPgons()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.SetCurrentLayer(ptOverlayMapThemeData.ClosedPgonsLayers, DMAcadExt.DMApp.AppID, True, True)
		MessageBox.Show(CStr(bCurrentLayerOK) & vbCrLf & ptOverlayMapThemeData.TopoName & vbCrLf & ptOverlayMapThemeData.ClosedPgonsLayers, "02_240")
		If bCurrentLayerOK Then
			TopoManager.TopoCreator.TopoToClosedPgons(ptOverlayMapThemeData.TopoName, True)
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Private Sub tmrDelay_Tick190613(oSender As System.Object, e As System.EventArgs)	'Handles tmrDelay.Tick
		Me.tmrDelay.Enabled = False
		zzCheckMapLayer(0)
		zzCheckMapLayer(1)
		If mbAddOverlayExists Then
			zzCheckMapLayer(2)
		End If


	End Sub
	Private Sub tmrDelay_Tick(oSender As System.Object, e As System.EventArgs) Handles tmrDelay.Tick
		Me.tmrDelay.Enabled = False
		zzCheckMapLayer(miCurrentIndexForDelay)
		'	zzCheckMapLayer(1)



	End Sub

	Private Sub frmFDO_Overlay_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		zzMyInitializeComponent()
		If False Then
			If Not ptOverlayMapThemeData_A.IsNotEmpty Then
				doaLabelCheck(2).Enabled = False
			End If
		End If

		zzDispRes(True)
		''''''''''''''''''''	AfterChangeCurrent(0)
	End Sub

	Private Sub chkRemoveSlivers_CheckedChanged(ByVal oSender As System.Object, e As System.EventArgs) Handles chkRemoveSlivers.CheckedChanged
		Dim bAllSlivers As Boolean
		bAllSlivers = Me.chkRemoveSlivers.Checked
		Me.txtToleranceMin.Enabled = Not bAllSlivers
		Me.txtToleranceMax.Enabled = Not bAllSlivers
	End Sub
	Private Sub zzTestData(sLabel As String)
		If Me.dgvActions(0) IsNot Nothing Then
			Dim oView As DataView = DirectCast(Me.dgvActions(0).DataSource, DataView)
			Dim oCols As System.Windows.Forms.DataGridViewColumnCollection = Me.dgvActions(0).Columns
			System.Windows.Forms.MessageBox.Show(sLabel & vbCrLf & CStr(oView.Count) & vbCrLf & CStr(oCols.Count), "An:Testdata")
		End If
	
	End Sub
#Region "CleanupProcedures"
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

	Private Sub nudSteps_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles nudSteps.ValueChanged
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()

		Me.zzLoadData(iCleanupIndex)

	End Sub
	Private Sub dgvActions_RowEnter(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)	'Handles dgvActions.RowEnter
		'	If miCurrentAction <> -1 Then
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		miCurrentAction = e.RowIndex
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction), "21_459")
		zzSetCleanupErrPoints(iCleanupIndex)
		'	End If
	End Sub
	Private Sub dgvActions_DataError(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.DataGridViewDataErrorEventArgs)	'Handles dgvActions.DataError
		Dim sMsg As String = "dvgActionsDataErr:" & CStr(e.RowIndex) & "," & CStr(e.ColumnIndex) & "-" & e.Exception.Message
		e.ThrowException = False
		DMAcadExt.AcadDocument.WriteMessage(sMsg)
	End Sub
	Private Sub cmdFix_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()	'' 0 or 1
      zzCleanupAnalitic(iCleanupIndex, True)
   End Sub
   Private Sub cmdMark_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
      Dim iCleanupIndex As Integer = zzGetCleanupIndex()
      zzCleanupAnalitic(iCleanupIndex, False)
   End Sub
   Private Sub nudErrors_ValueChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs)    'Handles nudErrors.ValueChanged
      Dim iCleanupIndex As Integer = zzGetCleanupIndex()
      Dim iErrIndex As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Value)
      Dim iMax As Integer = Convert.ToInt32(Me.nudErrors(iCleanupIndex).Maximum)
      If iErrIndex = 0 OrElse (moCurrentPoints Is Nothing) Then
         If moPriorView Is Nothing Then
            TPlanGraph.TplnProject.SetInitView()
         Else
            DMAcadExt.AcadDocument.SetCurrentView(moPriorView)
         End If
      Else
         Try
            If moCurrentPoints Is Nothing Then
               DMAcadExt.AcadDocument.WriteMessage("_17 moCurrentPoints Is Nothing")
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
   Private Sub cmdEraseCleanupErr_Click(oSender As System.Object, e As System.EventArgs)
      '	MessageBox.Show("", "02_261")
      zzEraseCleanupErrors()
   End Sub
   Protected Sub zzCleanupAnalitic(ByVal iCleanupIndex As Integer, ByVal bFix As Boolean)

      Dim oDataRowView As DataRowView
      Dim iActionID As Integer
      Dim iAcadActionUB As Integer = -1
      Dim tCleanupOptions As DMAcadExt.dmCleanupOptions = Nothing
      Dim dTolerance As Double
      Dim bDmCleanupFirst As Boolean
      Dim tCleanupResult As DMAcadExt.dmCleanupResult = Nothing
      Dim oDataGridViewRow As DataGridViewRow
      Dim dicSelectedIndices As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
      If Me.dgvActions(iCleanupIndex).SelectedRows.Count > 0 Then
         For iSelectedIndex As Integer = 0 To Me.dgvActions(iCleanupIndex).SelectedRows.Count - 1
            oDataGridViewRow = Me.dgvActions(iCleanupIndex).SelectedRows.Item(iSelectedIndex)
            dicSelectedIndices.Add(oDataGridViewRow.Index, 0)
         Next
      End If
      For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
         If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
            oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
            iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
            dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))

            If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
               If iIndex = 0 Then
                  bDmCleanupFirst = False
               End If
               iAcadActionUB += 1
            Else
               If iIndex = 0 Then
                  bDmCleanupFirst = True
               End If
               tCleanupOptions.AddAction(iActionID, dTolerance, iIndex)
            End If
         End If
      Next
      '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      Dim oDataRow As DataRow
      Dim bCurrentLayerOK As Boolean
      MessageBox.Show(ptOverlayMapThemeData.LinkLayers & vbCrLf & CStr(iAcadActionUB) & vbCrLf & CStr(tCleanupOptions.HasAction), "05_400")
      If iAcadActionUB >= 0 OrElse tCleanupOptions.HasAction Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

         If tCleanupOptions.HasAction Then
            MessageBox.Show(ptOverlayMapThemeData.LinkLayers, "05_453")
            tCleanupOptions.MapThemeData = ptOverlayMapThemeData
         End If
         DMAcadExt.AcadDocument.WriteMessage("Layers:" & tCleanupOptions.SourceLayers & ";" & tCleanupOptions.DestLayers & "!")
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.CleanupErrMarks, True, True, True)
         DMAcadExt.AcadDocument.WriteMessage("&&&Layer:" & bCurrentLayerOK.ToString())
         If bCurrentLayerOK Then
            Me.Cursor = Cursors.WaitCursor

            If bDmCleanupFirst Then
               tCleanupResult = zzDMCleanupAnalitic(bFix, tCleanupOptions, iCleanupIndex)
            End If
            If iAcadActionUB >= 0 Then
               Dim oaActionVar(iAcadActionUB) As ActionVar
               Dim iaAcadCleanupRowIndex(iAcadActionUB) As Integer
               ReDim moaErrorPoints(iAcadActionUB)
               Dim iaErrors() As Integer
               Try
                  Dim iVarIndex As Integer = 0
                  For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
                     If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
                        oDataRowView = moCurrentView(iCleanupIndex).Item(iIndex)
                        iActionID = DMCommon.Functions.CIntN(oDataRowView.Item(msActionIDFldName))
                        If iActionID < DMAcadExt.enCleanupAction.First_dmAction Then
                           dTolerance = DMCommon.Functions.CDblN(oDataRowView.Item(msToleranceFldName))
                           oaActionVar(iVarIndex) = New ActionVar(iActionID, dTolerance)
                           iaAcadCleanupRowIndex(iVarIndex) = iIndex
                           iVarIndex += 1
                        End If
                     End If
                  Next
                  If ptOverlayMapThemeData.IsNotEmpty Then
                     Try
                        Dim sBaseLayers As String
                        Dim colLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
                        If iCleanupIndex = 0 Then
                           sBaseLayers = ptOverlayMapThemeData.LinkLayers
                           colLinks = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
                           MessageBox.Show(CStr(iCleanupIndex) & vbCrLf & sBaseLayers & vbCrLf & CStr(colLinks.Count), "05_013")
                        ElseIf iCleanupIndex = 1 Then
                           sBaseLayers = String.Empty
                           '	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers, "05_014")
                           colLinks = DMAcadExt.AcadTransaction.GetLinksNew(ptOverlayMapThemeData.LinkLayers, ptOverlayMapThemeData.LineLinkLayers)
                           MessageBox.Show(ptOverlayMapThemeData.LinkLayers & vbCrLf & ptOverlayMapThemeData.LineLinkLayers & vbCrLf & CStr(colLinks.Count), "05_014")
                        Else
                           Return
                        End If
                        '	MessageBox.Show(ptMapThemeData.LinkLayers & vbCrLf & ptMapThemeData.LineLinkLayers & vbCrLf & sBaseLayers & vbCrLf & CStr(colLinks.Count), "05_015")
                        iaErrors = TopoManager.TopoCreator.Cleanup(oaActionVar, sBaseLayers, "", colLinks, bFix, moaErrorPoints)
                        '	MessageBox.Show(CStr(colLinks.Count) & ":" & CStr(iaErrors.GetUpperBound(0)), "05_016")
                     Catch oEx As Exception
                        MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString(), "26_997")
                        Common.GetMapTopoEx(oEx, "C919aMM_")
                        DMAcadExt.AcadTransaction.Terminate()
                        DMAcadExt.AcadDocument.CloseMessage()
                        DMAcadExt.AcadDocument.Unlock()
                        Return
                     End Try
                     Dim iResIndex As Integer = 0
                     For iIndex As Integer = 0 To moCurrentView(iCleanupIndex).Count - 1
                        'oDataRow = oDataTable.Rows(iaAcadCleanupRowIndex(iIndex))
                        If dicSelectedIndices.Count = 0 OrElse dicSelectedIndices.ContainsKey(iIndex) Then
                           oDataRow = moCurrentView(iCleanupIndex).Item(iaAcadCleanupRowIndex(iResIndex)).Row
                           '	MessageBox.Show(CStr(iIndex) & ":" & CStr(iaErrors(iResIndex)), "21_472")
                           With oDataRow
                              If .RowState <> DataRowState.Deleted Then
                                 .BeginEdit()
                                 .Item(msErrorsFldName) = iaErrors(iResIndex)
                                 If moaErrorPoints(iResIndex) IsNot Nothing Then
                                    .Item(msPointsFldName) = moaErrorPoints(iResIndex)
                                 End If
                                 .EndEdit()
                              Else
                                 '' System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
                              End If
                           End With
                           iResIndex += 1
                        End If
                     Next
                  End If

               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - zzCleanup")
               End Try
               If Not bDmCleanupFirst AndAlso tCleanupOptions.HasAction Then
                  tCleanupResult = zzDMCleanupAnalitic(bFix, tCleanupOptions, iCleanupIndex)
               End If
            End If  'iAcadActionUB >= 0

            Do While tCleanupResult.NextAction
               '	oDataRow = oDataTable.Rows(tCleanupResult.RowIndex)
               oDataRow = moCurrentView(iCleanupIndex).Item(tCleanupResult.RowIndex).Row
               With oDataRow
                  '	MessageBox.Show(.RowState.ToString() & ":" & CStr(tCleanupResult.RowIndex), "21_458")
                  If .RowState <> DataRowState.Deleted Then
                     .BeginEdit()
                     .Item(msErrorsFldName) = tCleanupResult.ErrNums
                     If tCleanupResult.ErrorPoints IsNot Nothing Then
                        '	MessageBox.Show(CStr(tCleanupResult.ErrorPoints.UpperBound), "21_652bb")
                        .Item(msPointsFldName) = tCleanupResult.ErrorPoints
                     End If
                     .EndEdit()
                     '.AcceptChanges()
                  Else
                     System.Windows.Forms.MessageBox.Show("RowDeleted", "Design_m")
                  End If
               End With
            Loop
            Me.Cursor = Cursors.Default
         End If 'If bCurrentLayerOK Then
         Try
            miCurrentAction = Me.dgvActions(iCleanupIndex).CurrentRow.Index
            zzSetCleanupErrPoints(iCleanupIndex)
         Catch oEx As Exception
            miCurrentAction = -1
         End Try
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()
      End If
   End Sub
   Private Function zzDMCleanupAnalitic(ByVal bFix As Boolean, ByVal tCleanupOptions As DMAcadExt.dmCleanupOptions, ByVal iCleanupIndex As Integer) As DMAcadExt.dmCleanupResult
      Dim tCleanupResult As DMAcadExt.dmCleanupResult
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      If iCleanupIndex = 0 Then
         tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanup(bFix, tCleanupOptions)

      Else
         tCleanupResult = DMAcadExt.dmLineCleanup.dmCleanupNew(bFix, tCleanupOptions)
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      Return tCleanupResult
   End Function
	Private Sub zzEraseCleanupErrors()
		Dim dlEntityProc As DMAcadExt.AcadTransaction.EntityProc = New DMAcadExt.AcadTransaction.EntityProc(AddressOf zzDeleteBlockRefs)
		Dim sErrBlockNameList As String = DMAcadExt.MarkBlock.GetMarkBlockList()
		Dim iCleanupIndex As Integer = zzGetCleanupIndex()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.ProcByFilter(dlEntityProc, , , sErrBlockNameList)
		'	Me.nudErrors(iCleanupIndex).Minimum = Decimal.Zero
		Me.nudErrors(iCleanupIndex).Value = Decimal.Zero
		Me.nudErrors(iCleanupIndex).Maximum = Decimal.Zero


		If False Then	'Temp !Add
			If moaTopoErrors IsNot Nothing Then
				moaTopoErrors.Clear()
			End If
		End If

		'	zzDispTopoErrors()
		zzRegen()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
#End Region

	Private Function zzGetCleanupIndex() As Integer
		Select Case LabelCheck.CurrentIndex
			Case 0
				'Return 0
			Case 3
				'Return 1
		End Select
		Return 0
	End Function
	Private Sub zzRegen()
		'	Dim oAcadEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Try
			DMAcadExt.AcadDocument.Regen()
			'	Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzRegen")
		End Try
	End Sub
	Private Sub zzDeleteBlockRefs(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity, bCond As Boolean)
		If bCond Then
			oEntity.Erase()
		End If

	End Sub
	Private Sub zzSetCleanupErrPoints(ByVal iCleanupIndex As Integer)
		Dim oDataRow As DataRow = moCurrentView(iCleanupIndex).Item(miCurrentAction).Row
		Dim iErrorCount As Integer
		If oDataRow.IsNull(msPointsFldName) Then
			moCurrentPoints = Nothing
			iErrorCount = 0
		Else
			Dim oDin As Object = oDataRow.Item(msPointsFldName)
			If False Then


				If (oDin Is Nothing) Then
					MessageBox.Show(CStr(oDin Is Nothing), "21_659")
				Else
					MessageBox.Show(oDin.ToString() & vbCrLf & oDin.GetType().ToString(), "21_669")
				End If
			End If
			moCurrentPoints = DirectCast(oDataRow.Item(msPointsFldName), DMAcadExt.TplnPointArray)
			'	MessageBox.Show(CStr(moCurrentPoints.UpperBound), "21_654f")
			iErrorCount = moCurrentPoints.UpperBound + 1
		End If
		'	System.Windows.Forms.MessageBox.Show(CStr(miCurrentAction) & ":" & CStr(iErrorCount), "21_461")
		Try

			moPriorView = DMAcadExt.AcadDocument.GetCurrentView()
			With Me.nudErrors(iCleanupIndex)
				.Maximum = Convert.ToDecimal(iErrorCount)
				.Value = Decimal.Zero
			End With
			Me.txtTopoErrors.Text = " - "
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmTopoActionsBase - zzSetCleanupErrPoints")
		End Try
	End Sub
	Private Sub zzLoadData(iCleanupIndex As Integer)
		Const iCleanupType As Integer = 4
		Dim iStepNo As Integer = Convert.ToInt32(Me.nudSteps(iCleanupIndex).Value)
		Dim sStepNo As String = Convert.ToString(Me.nudSteps(iCleanupIndex).Value)
		Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
		Dim sSelectComText As String
		sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(iCleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"
		moCleanupActionsTable = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sSelectComText, CommandType.Text, "CleanupActions")
		moCleanupActionsTable.Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
		moCleanupActionsTable.Columns.Add(msPointsFldName, oPointArray.GetType())

		Dim iRowCount As Integer = moCleanupActionsTable.Rows.Count
		'	MessageBox.Show(sSelectComText & vbCrLf & CStr(iRowCount), "05_340")
		If iRowCount > 0 Then
			Dim oRow As DataRow = moCleanupActionsTable.Rows.Item(iRowCount - 1)
			miStepNum = DirectCast(oRow.Item("Step"), Integer)
		End If
		'	MessageBox.Show(CStr(moMapThemeData.CleanupType) & ":" & CStr(iRowCount), "05_200")
		Dim oColumn As System.Data.DataColumn = moCleanupActionsTable.Columns("Step")
		oColumn.DefaultValue = iStepNo

		moCurrentView(iCleanupIndex) = New DataView(moCleanupActionsTable, "Step=" & sStepNo, "", DataViewRowState.CurrentRows)

		Me.dgvActions(iCleanupIndex).DataSource = moCurrentView(iCleanupIndex)
		'	MessageBox.Show("Step=" & sStepNo & vbCrLf & CStr(moCurrentView(iCleanupIndex).Count) & vbCrLf & CStr(iCleanupIndex), "05_342")
	End Sub
	Private Class MPgons
		Inherits Dictionary(Of Integer, Entity)
		Private mcolInner As Autodesk.AutoCAD.Geometry.IntegerCollection = New Autodesk.AutoCAD.Geometry.IntegerCollection()
		Public Sub LoadAllMPgons(sMPgonLayer As String)
			Dim oXDataParcel As DMAcadExt.TplnXDataParcel
			Dim oList As IList(Of Autodesk.AutoCAD.DatabaseServices.Entity) = DMAcadExt.AcadTransaction.GetMPolygons(sMPgonLayer, OpenMode.ForWrite)
			For Each oMPgon As Entity In oList
				oXDataParcel = New DMAcadExt.TplnXDataParcel(oMPgon.XData)
				If MyBase.ContainsKey(oXDataParcel.ID) Then
					DMAcadExt.AcadDocument.WriteMessage("!?!: " & CStr(oXDataParcel.ID) & "," & CStr(oXDataParcel.DataID) & "," & CStr(oXDataParcel.Name))
				Else
					MyBase.Add(oXDataParcel.ID, oMPgon)
				End If

			Next
		End Sub
		Public Sub LoadAll(sPgonLayer As String)
			Dim oXDataParcel As DMAcadExt.TplnXDataParcel
			Dim colObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(sPgonLayer)
			Dim oEntity As Entity
			For Each tAcObjID As ObjectId In colObjIDs
				oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, OpenMode.ForWrite)
				oXDataParcel = New DMAcadExt.TplnXDataParcel(oEntity.XData)
				If MyBase.ContainsKey(oXDataParcel.ID) Then
					DMAcadExt.AcadDocument.WriteMessage("!?!: " & CStr(oXDataParcel.ID) & "," & CStr(oXDataParcel.DataID) & "," & CStr(oXDataParcel.Name))
				Else
					MyBase.Add(oXDataParcel.ID, oEntity)
				End If

			Next
		End Sub
		Public ReadOnly Property InnerColCount As Integer
			Get
				Return mcolInner.Count
			End Get
		End Property
		Public Sub AddToInnerCol(sList As String)
			Dim tList As DMCommon.dmList = New DMCommon.dmList(sList)
			Dim iMgonID As Integer
			If Not String.IsNullOrEmpty(sList) Then
				For iIndex As Integer = 0 To tList.UpperBound
					If Integer.TryParse(tList.Item(iIndex), iMgonID) Then
						If Not mcolInner.Contains(iMgonID) Then
							mcolInner.Add(iMgonID)
						End If

					End If
				Next
			End If
		End Sub
		Public Sub SetLayer(sLayer As String)
			Dim oMPgon As Entity
			Dim iTest As Integer

			For Each iID As Integer In mcolInner
				DMAcadExt.AcadDocument.WriteMessage("##: " & CStr(iID))
				If MyBase.ContainsKey(iID) Then
					oMPgon = MyBase.Item(iID)
					If oMPgon.Layer <> sLayer Then
						oMPgon.Layer = sLayer
						iTest += 1
					End If
				End If
			Next
			'	MessageBox.Show(CStr(iTest) & vbCrLf & CStr(MyBase.Count) & vbCrLf & CStr(mcolInner.Count), "03_328a")
		End Sub


	End Class


End Class