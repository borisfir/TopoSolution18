Option Explicit On
Option Strict On
Public Class frmFDO_Overlay
	Const miThisStagesUB As Integer = 3
	Private mdicPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)
	Private miResPgonCount As Integer
	Private mbAddOverlayExists As Boolean
	Private msIntermediateLayer As String = Nothing
	Private msIntermediateSHPFileName As String
	Private msIntermediateConnection As String

	Private miCurrentIndexForDelay As Integer
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


	Private Sub zzInitPanel0_1_2(iIndex As Integer)
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

		'	Me.tstTopology.SuspendLayout()
		Me.doaPanels(1).SuspendLayout()
		Me.SuspendLayout()
		'
		'doaPanels(iIndex)
		'
		With doaPanels(iIndex)
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
		Me.Controls.Add(Me.doaPanels(iIndex))

		Me.doaPanels(1).ResumeLayout(False)
		Me.doaPanels(1).PerformLayout()
		Me.ResumeLayout(False)
	End Sub

	Private Sub zzInitPanel3()

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
		With doaPanels(3)
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
			.Text = ptMapThemeData.MapLayer
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
	Private Function zzGetMapThemeData(iIndex As Integer) As DMAcadExt.MapThemeData
		Select Case iIndex
			Case 0
				Return ptSourceMapThemeData
			Case 1
				Return ptOverlayMapThemeData
			Case 2
				Return ptOverlayMapThemeData_A
			Case Else
				Return New DMAcadExt.MapThemeData
		End Select

	End Function
	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData, tSourceMapThemeData As DMAcadExt.MapThemeData, tOverlayMapThemeData As DMAcadExt.MapThemeData, Optional tOverlayMapThemeData_A As DMAcadExt.MapThemeData = Nothing)
		MyBase.New(tMapThemeData)
		'''''''''''''''	ptMapThemeData = tMapThemeData

		ptSourceMapThemeData = tSourceMapThemeData
		ptOverlayMapThemeData = tOverlayMapThemeData
		mbAddOverlayExists = tOverlayMapThemeData_A.IsNotEmpty
		If mbAddOverlayExists Then
			ptOverlayMapThemeData_A = tOverlayMapThemeData_A
		End If
		MyBase.SetLabelDim(miThisStagesUB)

		' This call is required by the designer.
		InitializeComponent()
		zzCheckRes()

	End Sub

	Private Sub tstTopology_ItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstTopology.ItemClicked
		Me.Cursor = Cursors.WaitCursor
		Select Case e.ClickedItem.Name
			Case Me.tsbCreateTopo.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzCreateTopo(LabelCheck.CurrentIndex)
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
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzCreateMapLayer(LabelCheck.CurrentIndex)
					Case 3
						If miResPgonCount = 0 Then
							zzSaveFDOSliverToleranceSetting()
							zzUnion()
						End If
					
				End Select
			Case Me.tsbExecA.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzCreateMapLayer(LabelCheck.CurrentIndex)
					Case 3


						zzUnionAdd()

					 
				End Select
			Case Me.tsbClear.Name
				Select Case LabelCheck.CurrentIndex
					Case 0, 1, 2
						zzRemoveMapLayer(LabelCheck.CurrentIndex)
						zzCheckMapLayer(LabelCheck.CurrentIndex)
						'	Me.zzDeleteTopo(LabelCheck.CurrentIndex)
					Case 3
						zzRemoveMapLayer(0)
						zzRemoveMapLayer(1)
						If mbAddOverlayExists Then
							zzRemoveMapLayer(2)
							zzRemoveIntermediateMapLayer()
						End If
						zzCheckMapLayer(0)
						zzCheckMapLayer(1)
						If mbAddOverlayExists Then
							zzCheckMapLayer(2)
						End If
				End Select

		End Select
		Me.Cursor = Cursors.Default
		DMAcadExt.AcadDocument.UpdateScreen()
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
	Private Sub zzRemoveIntermediateMapLayer()
		'	Dim oMapThemeData As DMAcadExt.MapThemeData = zzGetMapThemeData(iIndex)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		'	System.Windows.Forms.MessageBox.Show(oTopoDef.FDOConnectionName & vbCrLf & oTopoDef.FDOLayerName, "zzClearMapLayer - 01_821")
		FDO.FDO_Manager.RemoveConnectionB(msIntermediateConnection)
		FDO.FDO_Manager.RemoveResource(msIntermediateConnection, msIntermediateLayer)

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
         tTopoRes = TopoManager.TopoCreator.CreateTopology(zzGetMapThemeData(iIndex).LineTopoName, zzGetMapThemeData(iIndex).LinkLayers, zzGetMapThemeData(iIndex).LineLinkLayers, zzGetMapThemeData(iIndex).CentroidBlocks, zzGetMapThemeData(iIndex).CentroidLayers, bCreateCentroids, String.Empty, String.Empty, False, True, dTolerance)
			If tTopoRes.TopoExists Then
				Me.txtPgonCount(iIndex).Text = CStr(tTopoRes.PgonCount)


				zzDispTopoExists(iIndex, True)


			Else
				zzDispTopoExists(iIndex, False)


			End If

		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmFDO_Overlay - zzCreateTopo_01")
		End Try


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		Me.Cursor = Cursors.Default


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

      If oMapThemeData.GraphType = DMAcadExt.enGraphType.Topology OrElse oMapThemeData.GraphType = DMAcadExt.enGraphType.TopoOverlay Then
         tTopoRes = TopoManager.TopoCreator.CheckTopo(oMapThemeData.LineTopoName, True, bMsg)
      ElseIf oMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
         tTopoRes = TopoManager.TopoCreator.CheckMPgons(oMapThemeData.MPgonLayers, True)
      End If

		'	MessageBox.Show("Locked: " & CStr(DMAcadExt.AcadDocument.IsLocked) & vbCrLf & CStr(tTopoRes.IsInstance) & vbCrLf & oMapThemeData.GraphType.ToString(), "04_451")

		'	zzDispTopoExists(iIndex, tTopoRes.TopoExists)
		zzDispTopoOK(iIndex, tTopoRes)
		'
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
	Private Sub zzCreateMapLayer(ByVal iIndex As Integer)
		Dim oMapThemeData As DMAcadExt.MapThemeData = zzGetMapThemeData(iIndex)
		Dim sTopoName As String = oMapThemeData.LineTopoName
		Dim sExpCondition As String
      If oMapThemeData.GraphType = DMAcadExt.enGraphType.Topology OrElse oMapThemeData.GraphType = DMAcadExt.enGraphType.TopoOverlay Then
         sExpCondition = sTopoName
      ElseIf oMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
         sExpCondition = oMapThemeData.ClosedPgonsLayers
         '	sExpCondition = oMapThemeData.MPgonLayers
      Else
         sExpCondition = Nothing
      End If
		Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
		Dim s As String = sExpCondition
		s &= vbCrLf & oMapThemeData.GraphType.ToString()
		'	s &= vbCrLf & sTopoName
		s &= vbCrLf & oMapThemeData.ShapeConnection
		s &= vbCrLf & oMapThemeData.MapLayer
      '  MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & s, "01_659y")
		oFDO_Manager.CreateMapLayer(sExpCondition, oMapThemeData.GraphType, oMapThemeData.MapLayer, oMapThemeData.ShapeConnection, CStr(iIndex))
		'MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & s, "01_999t")
		Me.tmrDelay.Enabled = True
		miCurrentIndexForDelay = iIndex
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
		Dim saCaptions() As String = {"בניית שכבת מקור", "בניית שכבת חיתוך א'", "בניית שכבת חיתוך ב'", "חיתוך שכבות"}
		MyBase.psaCaptions = saCaptions
		MyBase.OnNew()
		For iIndex As Integer = 0 To 2
			zzInitPanel0_1_2(iIndex)
         zzCheckTopo(iIndex, False)
			zzCheckMapLayer(iIndex)
		Next
		zzInitToolStrip()
		AfterChangeCurrent(0)
		zzInitPanel3()
		zzGetSliverToleranceSetting()
	End Sub
	Private Sub zzUnion()
		Dim sSourceLayer As String, sOverlayLayer As String, sResultLayer As String
		Dim sOverlayLayer_A As String = Nothing, sResultLayer_A As String = Nothing
		Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
		Dim oMySettings As My.MySettings = New My.MySettings()

		Dim bAllSlivers As Boolean = oMySettings.AllSlivers
		Dim dMinFDOSliverTolerance, dMaxFDOSliverTolerance As Double

		Dim sSHPFileName As String

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
			sResultLayer_A = sResultLayer & "_A"
		End If

		If mbAddOverlayExists Then
			msIntermediateLayer = sResultLayer_A
			sSHPFileName = oFDO_Manager.UnionOnly(sSourceLayer, sOverlayLayer, msIntermediateLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			'	sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sOverlayLayer_A, sResultLayer_A, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			Dim saParseVal() As String = Strings.Split(sSHPFileName, ".")
			msIntermediateConnection = "shp" & saParseVal(0)
		Else
			'	DMAcadExt.AcadDocument.WriteMessage("!!!NB_4:" & System.Windows.Forms.Application.LocalUserAppDataPath)
       '  System.Windows.Forms.MessageBox.Show(sSourceLayer & vbCrLf & sOverlayLayer & vbCrLf & sResultLayer_A & vbCrLf & sOverlayLayer_A & vbCrLf & sResultLayer, "zzUN 142ax")
			sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			If sSHPFileName IsNot Nothing Then
				Me.lblResultMapLayerExists.Visible = True
				Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sSHPFileName)
				Dim saParseVal() As String = Strings.Split(oFileInfo.Name, ".")
				Dim sFCName As String = saParseVal(0)
				'	System.Windows.Forms.MessageBox.Show(sSHPFileName & vbCrLf & ptSourceMapThemeData.GraphType.ToString(), "05_217")
            If ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.Topology OrElse ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.TopoOverlay Then
               miResPgonCount = oFDO_Manager.DBF2XDataTopo(sSHPFileName, TopoManager.TPlanGraph.TplnProject.XDataAppName, False, False)
            ElseIf ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
               '	MessageBox.Show(sSHPFileName & vbCrLf & TopoManager.TPlanGraph.TplnProject.XDataAppName, "08_600")
               miResPgonCount = oFDO_Manager.DBF2XDataCP(sSHPFileName, TopoManager.TPlanGraph.TplnProject.XDataAppName, False)
            End If
			Else
				miResPgonCount = 0
				Me.lblResultMapLayerExists.Visible = False
			End If
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		zzDispRes(False)




	End Sub
	Private Sub zzUnionAdd()
		If True Or mbAddOverlayExists Then


			Dim sSourceLayer As String, sOverlayLayer As String, sResultLayer As String
			Dim sOverlayLayer_A As String = Nothing, sResultLayer_A As String = Nothing
			Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
			Dim oMySettings As My.MySettings = New My.MySettings()

			Dim bAllSlivers As Boolean = oMySettings.AllSlivers
			Dim dMinFDOSliverTolerance, dMaxFDOSliverTolerance As Double

			Dim sSHPFileName As String

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
				sResultLayer_A = sResultLayer & "_A"
			End If


			'''''''''''''''''''	sSHPFileName = oFDO_Manager.UnionOnly(sSourceLayer, sOverlayLayer, sResultLayer_A, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			'	sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sOverlayLayer_A, sResultLayer_A, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)


			'	DMAcadExt.AcadDocument.WriteMessage("!!!NB_4:" & System.Windows.Forms.Application.LocalUserAppDataPath)
			'	System.Windows.Forms.MessageBox.Show(msIntermediateLayer & vbCrLf & sOverlayLayer_A & vbCrLf & sResultLayer, "zzUNadd 144cv")
			sSHPFileName = oFDO_Manager.Union(msIntermediateLayer, sOverlayLayer_A, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)


			If (sSHPFileName IsNot Nothing) Then
				'	System.Windows.Forms.MessageBox.Show(sSHPFileName, "04_240")
				Me.lblResultMapLayerExists.Visible = True
				Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sSHPFileName)
				Dim saParseVal() As String = Strings.Split(oFileInfo.Name, ".")
				Dim sFCName As String = saParseVal(0)
				'		System.Windows.Forms.MessageBox.Show(sSHPFileName & vbCrLf & ptSourceMapThemeData.GraphType.ToString(), "05_217A")
            If ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.Topology OrElse ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.TopoOverlay Then

               miResPgonCount = oFDO_Manager.DBF2XDataTopo(sSHPFileName, TopoManager.TPlanGraph.TplnProject.XDataAppName, False, True)
            ElseIf ptSourceMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
               '	MessageBox.Show(sSHPFileName & vbCrLf & TopoManager.TPlanGraph.TplnProject.XDataAppName, "08_600")
               miResPgonCount = oFDO_Manager.DBF2XDataCP(sSHPFileName, TopoManager.TPlanGraph.TplnProject.XDataAppName, False)
            End If
			Else
				miResPgonCount = 0
				Me.lblResultMapLayerExists.Visible = False
				System.Windows.Forms.MessageBox.Show("", "04_241")
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			zzDispRes(False)
		End If
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
			Case 0, 1, 2, 3
				Me.doaPanels(iNewIndex).Controls.Add(Me.tstTopology)
		End Select
	End Sub
	Private Sub zzCheckRes()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		If DMAcadExt.AcadTransaction.LayerExists(ptMapThemeData.MapLayer) Then
			mdicPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(ptMapThemeData.MapLayer, TopoManager.TPlanGraph.TplnProject.XDataAppName, ptOverlayMapThemeData_A.IsNotEmpty)
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
            ' MessageBox.Show(CStr(miResPgonCount), "04_037")
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
		MessageBox.Show(TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod.ToString(), "10_140")
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




		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadTransaction.Terminate()
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
		If Not ptOverlayMapThemeData_A.IsNotEmpty Then
			doaLabelCheck(2).Enabled = False
		End If
		zzDispRes(True)
	End Sub

	Private Sub chkRemoveSlivers_CheckedChanged(ByVal oSender As System.Object, e As System.EventArgs) Handles chkRemoveSlivers.CheckedChanged
		Dim bAllSlivers As Boolean
		bAllSlivers = Me.chkRemoveSlivers.Checked
		Me.txtToleranceMin.Enabled = Not bAllSlivers
		Me.txtToleranceMax.Enabled = Not bAllSlivers
	End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub
End Class