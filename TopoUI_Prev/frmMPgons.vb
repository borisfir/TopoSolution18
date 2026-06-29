Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmMPgons

	Const miThisStagesUB As Integer = 1
#Region "Panel0_Declarations"

	Private lblCaption(1) As Label
	Private chkCreateCentroid As System.Windows.Forms.CheckBox
	Private chkHighlightSliver As System.Windows.Forms.CheckBox
	Private lblTolerance As System.Windows.Forms.Label
	Private txtTolerance As System.Windows.Forms.TextBox

	Private grbLinks(1) As System.Windows.Forms.GroupBox

	Private grbCentroids(1) As System.Windows.Forms.GroupBox
	Private lblCentroidBlocks As System.Windows.Forms.Label
	Private lblCentroidLayers As System.Windows.Forms.Label
	'	Private WithEvents tstTopology As System.Windows.Forms.ToolStrip
	Private lblTopoErrors As System.Windows.Forms.Label
	Private txtErrorCount As System.Windows.Forms.TextBox
	Private txtErrorIndex As System.Windows.Forms.TextBox
	Private lblTopoName As System.Windows.Forms.Label
	Private lblTopoNameCap As System.Windows.Forms.Label



	Private lblTopoExists As System.Windows.Forms.Label
	'	Private txtPgonCount As System.Windows.Forms.TextBox
	Private txtCentroidCount As System.Windows.Forms.TextBox
	Private txtCentroidBlocks As System.Windows.Forms.TextBox
	Private txtCentroidLayers As System.Windows.Forms.TextBox
	Private lblLinkLayers(1) As System.Windows.Forms.Label
	Private txtLinkLayers(1) As System.Windows.Forms.TextBox
	Private txtLinkCount(1) As System.Windows.Forms.TextBox

	Private miaPgonCount(1) As Integer

#End Region
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)
		MyBase.New(tMapThemeData)
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.

	End Sub
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tDissolveMapThemeData As DMAcadExt.MapThemeData)
		MyBase.New(tMapThemeData)
		'	MessageBox.Show(tMapThemeData.TopoName & ":" & tDissolveMapThemeData.TopoName, "04_100")
		ptDissolveMapThemeData = tDissolveMapThemeData
		' This call is required by the designer.
		InitializeComponent()
		'	MessageBox.Show(ptMapThemeData.CentroidBlocks & ":" & ptMapThemeData.CentroidLayers & ":" & ptMapThemeData.MapThemeID.ToString() & vbCrLf & tDissolveMapThemeData.CentroidBlocks & ":" & tDissolveMapThemeData.CentroidLayers & ":" & tDissolveMapThemeData.MapThemeID.ToString(), "02_943d")
		' Add any initialization after the InitializeComponent() call.
	End Sub
	Private Sub zzMyInitializeComponent()
		'ptMapThemeData.TopoName
		Dim saCaptions() As String = {ptMapThemeData.MapThemeName, ptDissolveMapThemeData.MapThemeName}
		MyBase.psaCaptions = saCaptions
		MyBase.OnNew()
		zzInitToolStrip()
		zzInitPanel0(0)
		zzInitPanel0(1)

		'	zzCheckTopo(iIndex, False)
		'	zzCheckMapLayer(iIndex)


		AfterChangeCurrent(0)
		'
	End Sub

	Private Sub zzInitPanel0(iIndex As Integer)
		Dim iTop As Integer = 20
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
		Me.txtCentroidCount = New System.Windows.Forms.TextBox()
		Me.txtCentroidBlocks = New System.Windows.Forms.TextBox()
		Me.txtCentroidLayers = New System.Windows.Forms.TextBox()
		Me.lblCentroidBlocks = New System.Windows.Forms.Label()
		Me.lblCentroidLayers = New System.Windows.Forms.Label()
		Me.grbLinks(iIndex) = New System.Windows.Forms.GroupBox()
		Me.txtLinkCount(iIndex) = New System.Windows.Forms.TextBox()
		Me.txtLinkLayers(iIndex) = New System.Windows.Forms.TextBox()
		Me.lblLinkLayers(iIndex) = New System.Windows.Forms.Label()
		Me.txtTolerance = New System.Windows.Forms.TextBox()
		Me.lblTolerance = New System.Windows.Forms.Label()
		Me.chkHighlightSliver = New System.Windows.Forms.CheckBox()
		Me.chkCreateCentroid = New System.Windows.Forms.CheckBox()
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
		With Me.doaPanels(0)
			'	.RightToLeft = Windows.Forms.RightToLeft.No
			'	.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			With .Controls

				.Add(Me.lblTopoExists)
				'''''''''''	.Add(Me.lblCaption(iIndex))

				.Add(Me.txtErrorCount)
				.Add(Me.txtErrorIndex)
				.Add(Me.lblTopoName)
				.Add(Me.lblTopoNameCap)
				'	MessageBox.Show(Me.RightToLeft.ToString() & vbCrLf & Me.doaPanels(1).RightToLeft.ToString() & vbCrLf & lblTopoNameCap.RightToLeft.ToString() & vbCrLf & Me.txtPgonCount.RightToLeft.ToString() & vbCrLf & Me.txtErrorCount.RightToLeft.ToString(), "02_154b")
				.Add(Me.tstTopology)
				.Add(Me.lblTopoErrors)
				.Add(Me.grbCentroids(iIndex))
				.Add(Me.grbLinks(iIndex))
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
			.Location = New System.Drawing.Point(8, iTop)	'36
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
		'
		'tstTopology
		'
		With Me.tstTopology
			.Location = New System.Drawing.Point(0, 0)
			.Name = "tstTopology"
			.Size = New System.Drawing.Size(300, 25)
			.TabIndex = 11
		End With
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
			.Controls.Add(Me.txtCentroidCount)
			.Controls.Add(Me.txtCentroidBlocks)
			.Controls.Add(Me.txtCentroidLayers)
			.Controls.Add(Me.lblCentroidBlocks)
			.Controls.Add(Me.lblCentroidLayers)
			.Location = New System.Drawing.Point(8, iTop + 68)
			.Name = "grbCentroids"
			.Size = New System.Drawing.Size(284, 64)
			.TabIndex = 8
			.TabStop = False
			.Text = "Centroids"
		End With
		'
		'txtCentroidCount
		'
		With Me.txtCentroidCount
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
		With Me.lblCentroidBlocks
			.Location = New System.Drawing.Point(20, 42)
			.Name = "lblCentroidBlocks"
			.Size = New System.Drawing.Size(78, 18)
			.TabIndex = 7
			.Text = "Block names:"
		End With
		'
		'lblCentroidLayers
		'
		With Me.lblCentroidLayers
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
			.Location = New System.Drawing.Point(8, iTop + 22)
			.Name = "grbLinks"
			.Size = New System.Drawing.Size(284, 43)
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
		'txtLinkLayers
		'
		With Me.txtLinkLayers(iIndex)
			.Location = New System.Drawing.Point(66, 15)
			.Name = "txtLinkLayers"
			.ReadOnly = True
			.Size = New System.Drawing.Size(168, 22)
			.Text = tMapThemeData.LineLinkLayers
			.TabIndex = 16
		End With
		'
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
	Private Sub zzGetMapThemeData(iIndex As Integer, ByRef tMapThemeData As DMAcadExt.MapThemeData)
		Select Case iIndex
			Case 0
				tMapThemeData = ptMapThemeData
			Case 1
				tMapThemeData = ptDissolveMapThemeData
		End Select
	End Sub
	Private Sub frmMPgons_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load
		MessageBox.Show(CStr(miThisStagesUB), "07_120")
		MyBase.SetLabelDim(miThisStagesUB)
		zzMyInitializeComponent()
		'	zzDispTopoOK(0)
		'	zzDispCentroids()
	End Sub
End Class