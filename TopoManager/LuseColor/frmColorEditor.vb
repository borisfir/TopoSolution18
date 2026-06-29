Option Explicit On
Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports TopoManager
Imports System.Data
Public Enum enColorEditorMode
	Landuse
	ColorSchemeSource
	ColorScheme
End Enum
Public Class frmColorEditor
	Private miColorUB As Integer = 11
	Private txtFWColor(miColorUB) As TextBox
	Private txtAcadColor(miColorUB) As TextBox
	Private txtWidth(miColorUB) As TextBox
	Private mlblColor(miColorUB) As Label
	Private mtaDMColors(miColorUB) As DMAcadExt.DMColor
	Private mlblCaption(5) As Label
	Private lblAngle As Label
	Private mlblHatchName As Label

	Private cmbZebraAngle As ComboBox
	Private cmbHatchAngle As ComboBox
	Private cmbHatchLine As ComboBox

	Private txtHatchScale As TextBox
	Private lblHatchScale As Label
	Private lblHatchAngle As Label
	Private lblHatchLine As Label
	Private WithEvents lblMark As System.Windows.Forms.Label

	Private cmbHatchName As ComboBox
	Private rdbAngle(1) As RadioButton
	Private miCurrentColorIndex As Integer
	Private mbReadOnly As Boolean
	Private miLanduseID As Integer
	Private miColorSchemeID As Integer

	Private miMapThemeID As DMAcadExt.enMapTheme
	'	Private miColorSchemeID As Integer
	Private mtColorSchemeDB As DMAcadExt.ColorScheme
	Private mtColorScheme As DMAcadExt.ColorScheme
	Private miFormMode As enColorEditorMode
	Private miNewID As Integer
	Private mbPermitColorSchemeList As Boolean = True
	Private WithEvents txtFind As System.Windows.Forms.TextBox
	Private WithEvents lstFind As System.Windows.Forms.ListBox
	Private mbEventsEnabled As Boolean = False
	Private Sub zzMyInitializeComponent()

		Dim oFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Dim sColorText As String
		'		Dim tColorlabelSize As System.Drawing.Size
		'	Me.cmbColorSchemes.ValueMember = "ID"
		'	Me.cmbColorSchemes.DisplayMember = "Name"

		If miFormMode = enColorEditorMode.Landuse Then
			TPlanGraph.TplnProject.FillLanduses(Me.cmbColorSchemes, DMAcadExt.ColorScheme.NameDelim, miLanduseID)
			Me.cmbColorSchemes.DataSource = LanduseData.GetMainTable()

		End If

		Me.lblMark = New System.Windows.Forms.Label
		'
		'lblMark
		'
		'		Me.lblMark.AutoSize = True
		Me.lblMark.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.lblMark.Font = New System.Drawing.Font("Wingdings 3", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
		Me.lblMark.Location = New System.Drawing.Point(349, 375)
		Me.lblMark.Margin = New System.Windows.Forms.Padding(3, 6, 3, 0)
		Me.lblMark.Name = "lblMark"
		Me.lblMark.Size = New System.Drawing.Size(14, 15)
		Me.lblMark.TabIndex = 10
		Me.lblMark.Text = "|"

		For iIndex As Integer = 0 To miColorUB
			txtFWColor(iIndex) = New TextBox
			txtAcadColor(iIndex) = New TextBox
			txtWidth(iIndex) = New TextBox
			mlblColor(iIndex) = New Label
			AddHandler txtFWColor(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler txtAcadColor(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler txtWidth(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler txtWidth(iIndex).DoubleClick, AddressOf txtWidth_DoubleClick

			AddHandler mlblColor(iIndex).Click, AddressOf Color_GotFocus
			AddHandler txtFWColor(iIndex).LostFocus, AddressOf txtFWColor_LostFocus
			AddHandler txtAcadColor(iIndex).LostFocus, AddressOf txtAcadColor_LostFocus
			Select Case iIndex
				Case 0
					With Me.grbFill.Controls
						.Add(Me.txtFWColor(iIndex))
						.Add(Me.txtAcadColor(iIndex))
						.Add(Me.mlblColor(iIndex))
					End With
					sColorText = "RGB/Acad"
				Case 1 To 5
					With Me.grbBorder.Controls
						.Add(Me.txtFWColor(iIndex))
						.Add(Me.txtAcadColor(iIndex))
						.Add(Me.txtWidth(iIndex))
						.Add(Me.mlblColor(iIndex))
					End With
					sColorText = ChrW(1487 + iIndex) & "'"	''''''= 223  
				Case 6 To 10
					With Me.grbZebra.Controls
						.Add(Me.txtFWColor(iIndex))
						.Add(Me.txtAcadColor(iIndex))
						.Add(Me.txtWidth(iIndex))
						.Add(Me.mlblColor(iIndex))
					End With
					sColorText = ChrW(1482 + iIndex) & "'"
				Case 11
					With Me.grbHatch.Controls
						.Add(Me.txtFWColor(iIndex))
						.Add(Me.txtAcadColor(iIndex))
						.Add(Me.mlblColor(iIndex))
					End With
					sColorText = "RGB/Acad"
				Case Else
					sColorText = String.Empty
			End Select

			'
			'moFWColor
			'
			With Me.txtFWColor(iIndex)
				'  .Location = New System.Drawing.Point(302, 84)
				.Location = New System.Drawing.Point(5, zzGetColorTop(iIndex))
				.Name = "txtFWColor_" & CStr(iIndex)
				.Size = New System.Drawing.Size(72, 16)
				.Font = oFont
				.TextAlign = HorizontalAlignment.Left
			End With
			'
			'moAcadColor
			'
			With Me.txtAcadColor(iIndex)
				'  .Location = New System.Drawing.Point(302, 84)
				.Location = New System.Drawing.Point(80, zzGetColorTop(iIndex))
				.Name = "txtAcadColor_" & CStr(iIndex)
				.Size = New System.Drawing.Size(36, 16)
				.Font = oFont
				.TextAlign = HorizontalAlignment.Left
			End With
			'
			'moWidth
			'
			With Me.txtWidth(iIndex)
				'  .Location = New System.Drawing.Point(302, 84)
				.Location = New System.Drawing.Point(119, zzGetColorTop(iIndex))
				.Name = "txtAWidth_" & CStr(iIndex)
				.Size = New System.Drawing.Size(36, 16)
				.Font = oFont
				.TextAlign = HorizontalAlignment.Left
			End With
			'
			'mlblColor
			'
			With Me.mlblColor(iIndex)
				.Location = New System.Drawing.Point(158, zzGetColorTop(iIndex))
				.Name = "lblColor_" & CStr(iIndex)
				.Size = New System.Drawing.Size(16, 16)
				.Text = sColorText
				.Font = oFont
			End With
		Next
		For iIndex As Integer = 0 To 5
			mlblCaption(iIndex) = New Label
			If iIndex < 3 Then
				Me.grbBorder.Controls.Add(mlblCaption(iIndex))
				mlblCaption(iIndex).Top = 16
			Else
				Me.grbZebra.Controls.Add(mlblCaption(iIndex))
				mlblCaption(iIndex).Top = 48
			End If

			mlblCaption(iIndex).Size = New System.Drawing.Size(40, 16)
			Select Case iIndex Mod 3
				Case 0
					mlblCaption(iIndex).Left = 18
					mlblCaption(iIndex).Text = "RGB"
				Case 1
					mlblCaption(iIndex).Left = 76
					mlblCaption(iIndex).Text = "Acad"
				Case 2
					mlblCaption(iIndex).Left = 116
					mlblCaption(iIndex).Text = "רוחב"
			End Select

		Next
		For iIndex As Integer = 0 To 1
			rdbAngle(iIndex) = New RadioButton
			rdbAngle(iIndex).Size = New System.Drawing.Size(32, 16)
			rdbAngle(iIndex).Top = 18

			If iIndex = 0 Then
				rdbAngle(iIndex).Left = 2
				rdbAngle(iIndex).Text = "/"
				rdbAngle(iIndex).Checked = True
			Else
				rdbAngle(iIndex).Left = 40
				rdbAngle(iIndex).Text = "\"
			End If
			Me.grbZebra.Controls.Add(rdbAngle(iIndex))
		Next
		lblAngle = New Label
		With lblAngle
			.Size = New System.Drawing.Size(40, 16)
			.Text = "ז ו ו י ת"
			.Font = oFont
			.Location = New System.Drawing.Point(140, 18)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbZebra.Controls.Add(lblAngle)
		cmbZebraAngle = New ComboBox()
		With cmbZebraAngle
			.AutoSize = False
			.Size = New System.Drawing.Size(48, 16)
			.Location = New System.Drawing.Point(88, 18)
			.Font = oFont
			For iAngle As Integer = 0 To 90 Step 15
				.Items.Add(iAngle)
			Next

		End With
		Me.grbZebra.Controls.Add(cmbZebraAngle)

		cmbHatchName = New ComboBox
		With cmbHatchName
			.AutoSize = False
			.Size = New System.Drawing.Size(72, 16)
			.Location = New System.Drawing.Point(78, 16)
			.Font = oFont
			''''''''''''''''.Items.Add(DMAcadExt.DMHatch.LineHatchName)
			''''''''''''.Items.Add(DMAcadExt.DMHatch.NetHatchName)
			.Items.AddRange(DMAcadExt.DMPatterns.GetAllNames())
		End With
		Me.grbHatch.Controls.Add(cmbHatchName)
		Me.mlblHatchName = New Label
		With mlblHatchName
			.Size = New System.Drawing.Size(44, 16)
			.Text = "תבנית"
			.Font = oFont
			.Location = New System.Drawing.Point(140, 18)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(mlblHatchName)
		With Me.mlblColor(11)
			.Width = 56
			.Left = 115
		End With
		With Me.mlblColor(0)
			.Text = "RGB/Acad" '"צבע"
			.Width = 56
			.Left = 115
		End With

		Me.cmbHatchAngle = New ComboBox
		With Me.cmbHatchAngle
			.AutoSize = False
			.Size = New System.Drawing.Size(40, 16)
			.Location = New System.Drawing.Point(4, 72)
			.Font = oFont
			For iAngle As Integer = 0 To 179 Step 15
				.Items.Add(iAngle)
			Next
			Me.grbHatch.Controls.Add(cmbHatchAngle)
		End With

		Me.txtHatchScale = New TextBox
		With Me.txtHatchScale
			.Location = New System.Drawing.Point(5, 16)
			.Name = "txtHatchScale"
			.Size = New System.Drawing.Size(36, 16)
			.Font = oFont
			.TextAlign = HorizontalAlignment.Left
		End With
		Me.grbHatch.Controls.Add(txtHatchScale)
		Me.lblHatchScale = New Label
		With Me.lblHatchScale
			.Size = New System.Drawing.Size(32, 16)
			.Text = "קנ""מ"
			.Font = oFont
			.Location = New System.Drawing.Point(40, 16)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.lblHatchScale)
		Me.lblHatchAngle = New Label
		With Me.lblHatchAngle
			.Size = New System.Drawing.Size(32, 16)
			.Text = "זווית"
			.Font = oFont
			.Location = New System.Drawing.Point(40, 72)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.lblHatchAngle)
		Me.cmbHatchLine = New ComboBox
		TPlanGraph.TplnProject.FillLineWeights(Me.cmbHatchLine)
		With Me.cmbHatchLine
			.Font = oFont
			.Size = New System.Drawing.Size(46, 16)
			.Location = New System.Drawing.Point(80, 72)
		End With
		Me.grbHatch.Controls.Add(Me.cmbHatchLine)
		Me.lblHatchLine = New Label
		With Me.lblHatchLine
			.Size = New System.Drawing.Size(50, 16)
			.Text = "רוחב קו"
			.Font = oFont
			.Location = New System.Drawing.Point(124, 72)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.lblHatchLine)
		Me.lblMark.Location = New System.Drawing.Point(174, 0)
		If miFormMode = enColorEditorMode.Landuse Then
			Me.Text = "צביעת " & "יעוד " 
			Me.Label2.Text = "קוד יעוד"
			Me.Label3.Text = "שם יעוד"
			Me.Controls.Add(Me.cmbColorSchemes)
			Me.Controls.Add(Me.chkStandard)
			Me.tsbAddNew.Enabled = True
		ElseIf miFormMode = enColorEditorMode.ColorSchemeSource OrElse miFormMode = enColorEditorMode.ColorScheme Then
			Me.Text = "סכימת צביעה"
			Me.Label2.Text = "קוד יעוד"
			Me.Label3.Text = "שם סכמה"
			Me.tsbAddNew.Enabled = False
			Me.tsbFind.Enabled = False

		End If
		Me.chkStandard.Text = DMAcadExt.ColorScheme.StandardName

		If miLanduseID <> 0 Then
			Me.txtColorSchemeID.Text = Convert.ToString(miLanduseID)
		End If
		Me.txtColorSchemeID.Enabled = False
		If mtColorSchemeDB.ID <> 0 Then
			zzDisplayColorScheme()
			pcbPicture.Invalidate()
			'	zzPaint(Nothing)
		End If

	End Sub
	
	Private Function zzGetColorTop(ByVal iIndex As Integer) As Integer
		Dim iFirstTop As Integer
		Select Case iIndex
			Case 0
				iFirstTop = 20
			Case 1 To 5
				iFirstTop = 12
			Case 6 To 10
				iFirstTop = 44 - 5 * 26
			Case 11
				iFirstTop = -242
			Case Else
				iFirstTop = 0
		End Select
		Return iFirstTop + 26 * iIndex
	End Function
	Private Sub Color_GotFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oControl As Control
		Try
			oControl = DirectCast(oSender, Control)
			zzSetCurrentColorIndex(oControl.Name)
			zzHideFindByEvent()
		Catch oEx As Exception
		End Try

	End Sub

	Private Function zzGetIndex(ByVal sControlName As String) As Integer
		Dim saPart() As String = Strings.Split(sControlName, "_")
		If saPart.GetUpperBound(0) = 1 Then
			Try
				Return CInt(saPart(1))
			Catch oEx As Exception
				Return -1
			End Try
		Else
			Return -1
		End If
	End Function
	Private Sub zzSetCurrentColorIndex(ByVal sControlName As String)
		miCurrentColorIndex = zzGetIndex(sControlName)
		Select Case miCurrentColorIndex
			Case 0
				Me.grbFill.Controls.Add(Me.lblMark)
			Case 1 To 5
				Me.grbBorder.Controls.Add(Me.lblMark)
			Case 6 To 10
				Me.grbZebra.Controls.Add(Me.lblMark)
			Case 11
				Me.grbHatch.Controls.Add(Me.lblMark)
		End Select
		Me.lblMark.Top = Me.zzGetColorTop(miCurrentColorIndex)
	End Sub

	Private Sub zzInitFind()
		Me.txtColorSchemeName.Visible = False
		If Me.txtFind Is Nothing Then
			Me.txtFind = New System.Windows.Forms.TextBox()

			'
			'txtFind
			'
			With txtFind
				.Location = Me.txtColorSchemeName.Location
				.Name = "txtFind"
				.Size = Me.txtColorSchemeName.Size
				.TabIndex = 3
			End With
			Me.Controls.Add(Me.txtFind)
			Me.lstFind = New System.Windows.Forms.ListBox()
			'
			'lstFind
			'
			Me.lstFind.DisplayMember = "Name"
			Me.lstFind.FormattingEnabled = True
			Me.lstFind.Location = New System.Drawing.Point(txtFind.Location.X, txtFind.Location.Y + txtFind.Size.Height + 1)
			Me.lstFind.Name = "lstFind"
			Me.lstFind.Size = New System.Drawing.Size(txtFind.Size.Width, 0)
			Me.lstFind.TabIndex = 15
			Me.lstFind.ValueMember = "ID"
			Me.Controls.Add(Me.lstFind)
			zzSetFindDataSource(String.Empty)
			Me.lstFind.BringToFront()
		Else
			Me.txtFind.Visible = True
			Me.lstFind.Visible = True
		End If
		txtFind.Focus()
		DMCommon.BiLang.SetHebrew()
	End Sub
	Private Function zzGetListHeight(ByVal iRows As Integer) As Integer
		Const iMaxRows As Integer = 24
		If iRows > iMaxRows Then
			iRows = iMaxRows
		End If
		Return 4 + 13 * iRows
	End Function
	Private Sub zzHideFind(oSender As System.Object, e As System.EventArgs)
		Me.txtColorSchemeName.Visible = True
		Me.txtFind.Visible = False
		Me.lstFind.Visible = False
		If tsbFind.Checked Then
			tsbFind.Checked = False
		End If
	End Sub
	Private Sub zzHideFind()
		If Me.txtFind IsNot Nothing AndAlso Me.txtFind.Visible Then
			Me.txtColorSchemeName.Visible = True
			Me.txtFind.Visible = False
			Me.lstFind.Visible = False
			If tsbFind.Checked Then
				tsbFind.Checked = False
			End If
		End If

	End Sub
	Private Sub zzHideFindByEvent()
		If mbEventsEnabled Then
			zzHideFind()
		End If
	End Sub

	Public Sub New(iMapThemeID As DMAcadExt.enMapTheme, ByVal iMode As enColorEditorMode, ByVal bReadOnly As Boolean, Optional ByVal iLanduseID As Integer = 0, Optional ByVal iColorSchemeID As Integer = 0)
		MessageBox.Show("TopoManager" & ":" & Me.Name)
		' This call is required by the Windows Form Designer.
		mbReadOnly = bReadOnly
		miMapThemeID = iMapThemeID

		InitializeComponent()
		miFormMode = iMode

		miLanduseID = iLanduseID
		miColorSchemeID = iColorSchemeID



		If iColorSchemeID <> 0 Then
			mtColorSchemeDB = New DMAcadExt.ColorScheme(iColorSchemeID, 2.0)
		End If

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()


	End Sub



	Private Sub zzSetWinColor()
		Dim iRes As DialogResult = Me.cdlWindow.ShowDialog()
		If iRes = Windows.Forms.DialogResult.OK Then
			Dim oWinColor As Color = Me.cdlWindow.Color
			mtaDMColors(miCurrentColorIndex) = New DMAcadExt.DMColor(oWinColor)
			zzSetColor(txtFWColor(miCurrentColorIndex), mtaDMColors(miCurrentColorIndex))
			zzClearColor(txtAcadColor(miCurrentColorIndex))
		End If
	End Sub
	Private Sub zzSetAcadColor()
		Dim oAcadColor As Autodesk.AutoCAD.Colors.Color = DMAcadExt.AcadDocument.GetColor()
		If oAcadColor IsNot Nothing Then
			mtaDMColors(miCurrentColorIndex) = New DMAcadExt.DMColor(oAcadColor)
			zzSetColor(txtAcadColor(miCurrentColorIndex), mtaDMColors(miCurrentColorIndex))
			zzClearColor(txtFWColor(miCurrentColorIndex))
		End If
	End Sub
	Private Sub zzAddNewToDB(ByVal bColorSchemeOnly As Boolean)

	End Sub
	Private Function zzGetNewColorSchemeID(ByVal bLanduse As Boolean) As Integer
		Dim sCom As String = "SELECT Max(ID) FROM ColorSchemes WHERE"
		Dim sMin, sMax As String
		If bLanduse Then
			sMin = Convert.ToString(DMAcadExt.ColorScheme.MaxStandardID)
			sMax = Convert.ToString(DMAcadExt.ColorScheme.MaxLanduseID)
		Else
			sMin = Convert.ToString(DMAcadExt.ColorScheme.MaxLanduseID)
			sMax = String.Empty
		End If
		sCom &= "(ID > " & sMin & ")"
		If sMax.Length <> 0 Then
			sCom &= " AND (ID <= " & sMax & ")"
		End If
		Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sCom, CommandType.Text)
		If oRes Is Nothing Then
			Return 2001
		ElseIf IsDBNull(oRes) Then
			If bLanduse Then
				System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.ColorScheme.MaxStandardID + 1), "2_0100")
				Return DMAcadExt.ColorScheme.MaxStandardID + 1
			Else
				System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.ColorScheme.MaxStandardID + 1), "2_0140")
				Return DMAcadExt.ColorScheme.MaxLanduseID + 1
			End If
		Else
			Try
				'	System.Windows.Forms.MessageBox.Show(CStr(DirectCast(oRes, Integer) + 1), "2_0150")
				Return DirectCast(oRes, Integer) + 1
			Catch oEx As Exception
				Return -1
			End Try
		End If
	End Function
	Private Sub zzAddNewToDB()
		Dim sNewID_Name As String = Nothing
		Dim bRes As Boolean = False
		'	System.Windows.Forms.MessageBox.Show(CStr(miLanduseID) & ":" & mtColorScheme.Name & ":" & CStr(mtColorScheme.HasName), "miLanduseID 18_871")
		miNewID = 0

		If mtColorScheme.HasName Then
			If miLanduseID = 0 And miFormMode = enColorEditorMode.Landuse Then
				Dim sID As String
				Try
					sID = Me.txtColorSchemeID.Text
					If sID.Length = 0 Then
						If Not Me.chkStandard.Checked Then
							miNewID = Me.zzGetNewColorSchemeID(True)
						End If
					Else
						miNewID = Convert.ToInt32(sID)
					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmEditColorScheme")
				End Try

				'Me.chkStandard.Ch=ecked
			ElseIf miFormMode = enColorEditorMode.ColorSchemeSource Then
				miNewID = Me.zzGetNewColorSchemeID(False)
				Me.txtColorSchemeID.Text = Convert.ToString(miNewID)
				'	MessageBox.Show(Convert.ToString(miNewID) & ":" & mtColorScheme.Name, "26_176")
			End If

			If miNewID <> 0 And miFormMode = enColorEditorMode.Landuse Then
				'	System.Windows.Forms.MessageBox.Show(CStr(miNewID) & ":" & mtColorScheme.Name, "18_873")
				bRes = TPlanGraph.TplnProject.AddNewLanduse(miNewID, mtColorScheme.Name)
			End If
			Try
				'	System.Windows.Forms.MessageBox.Show(CStr(miNewID) & ":" & mtColorScheme.Name & ":" & CStr(bRes), "18_874")
				If bRes OrElse miNewID <> 0 Then
					bRes = mtColorScheme.AddNewToDB(miNewID)
					If bRes AndAlso miFormMode = enColorEditorMode.Landuse Then
						mbPermitColorSchemeList = False
						''''''''''''''	Me.cmbColorSchemes.Items.Clear()
						'''''''''''''''''''	TPlanGraph.TplnProject.FillLanduses(Me.cmbColorSchemes, DMAcadExt.ColorScheme.NameDelim)
						Me.cmbColorSchemes.DataSource = LanduseData.GetMainTable()
						mbPermitColorSchemeList = True
						If Me.cmbColorSchemes.Items.Count > 0 Then
							'''''''''	sNewID_Name = Convert.ToString(miNewID) & DMAcadExt.ColorScheme.NameDelim & mtColorScheme.Name
							'''''''''''''	Me.cmbColorSchemes.Text = sNewID_Name
							Me.cmbColorSchemes.SelectedValue = miNewID

						End If
					End If
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmEditColorScheme")
			End Try
		End If
	End Sub
	
	Private Sub zzUpdate()
		Dim sNewID_Name As String
		Dim bRes As Boolean
		Dim bPaint As Boolean = False

		If (miFormMode = enColorEditorMode.Landuse AndAlso miLanduseID <> 0) OrElse (miFormMode = enColorEditorMode.ColorScheme AndAlso miLanduseID <> 0) Then
			'MessageBox.Show(CStr(mtColorSchemeDB.ID) & ":" & miFormMode.ToString(), "18_055")
			bRes = mtColorScheme.UpdateDB(mtColorSchemeDB.ID)
			bPaint = True
		End If
		If miFormMode = enColorEditorMode.Landuse AndAlso miLanduseID <> 0 Then
			If mtColorSchemeDB.Name <> mtColorScheme.Name Then
				sNewID_Name = Convert.ToString(miLanduseID) & DMAcadExt.ColorScheme.NameDelim & mtColorScheme.Name
				bRes = TPlanGraph.TplnProject.UpdateLanduse(miLanduseID, mtColorScheme.Name)
				mbPermitColorSchemeList = False
				Me.cmbColorSchemes.Items.Clear()
				TPlanGraph.TplnProject.FillLanduses(Me.cmbColorSchemes, DMAcadExt.ColorScheme.NameDelim, miLanduseID)
				mbPermitColorSchemeList = True
				If Me.cmbColorSchemes.Items.Count > 0 Then
					Me.cmbColorSchemes.Text = sNewID_Name
					bPaint = False
				End If
			End If
		End If
		If bPaint Then
			pcbPicture.Invalidate()
			'	zzPaint(Nothing)
		End If
	End Sub

	Private Sub zzClearGraphics()
		Dim oGraphics As Graphics = Me.pcbPicture.CreateGraphics
		oGraphics.Clear(SystemColors.Window)
	End Sub
	Private Sub zzPaint(ByVal oGraphics As Graphics)
		If oGraphics Is Nothing Then
			oGraphics = Me.pcbPicture.CreateGraphics
		End If
		Dim oPaint As PaintBox = New PaintBox(oGraphics, Me.pcbPicture.ClientSize)
		If mtColorScheme.IsInstance Then
			oPaint.ColorScheme = mtColorScheme
		Else
			oPaint.ColorScheme = mtColorSchemeDB
		End If

		oPaint.Draw()
	End Sub
	Private Sub zzBuildColorScheme()
		mtColorScheme = New DMAcadExt.ColorScheme(2.0)
		mtColorScheme.Name = Me.txtColorSchemeName.Text
		Dim dWidth As Double
		For iBorderIndex As Integer = 1 To 5
			If Me.txtWidth(iBorderIndex).Text.Length = 0 Then
				Exit For
			Else
				dWidth = Convert.ToDouble(Me.txtWidth(iBorderIndex).Text)
				mtColorScheme.AddBorderStrip(mtaDMColors(iBorderIndex), dWidth)
			End If
		Next
		Dim dAngle As Double
		Dim iQuadrant As Integer
		Try
			dAngle = Convert.ToDouble(Me.cmbZebraAngle.Text)
			If Me.rdbAngle(0).Checked Then
				iQuadrant = 1
			Else
				iQuadrant = 2
			End If
			mtColorScheme.ZebraAngle = New DMAcadExt.LineAngle(dAngle, iQuadrant, False)
		Catch oEx As Exception

		End Try
		Try
			For iZebraIndex As Integer = 6 To 10
				If txtWidth(iZebraIndex).Text.Length = 0 Then
					Exit For
				Else
					dWidth = Convert.ToDouble(txtWidth(iZebraIndex).Text)
					mtColorScheme.AddZebraStrip(mtaDMColors(iZebraIndex), dWidth)
				End If
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmColorEditor")
		End Try

		Try
			Dim sPatternName As String = Me.cmbHatchName.Text
			Dim tHatch As DMAcadExt.DMHatch
			Dim tPattern As DMAcadExt.DMPattern = Nothing
			If DMAcadExt.DMPatterns.PatternsExist AndAlso DMAcadExt.DMPatterns.Item(sPatternName, tPattern) Then
				tHatch = New DMAcadExt.DMHatch(tPattern)
			Else
				tHatch = New DMAcadExt.DMHatch(sPatternName)
			End If

			With tHatch
				.Scale = 1
				.BackColor = mtaDMColors(0)
				If (sPatternName IsNot Nothing) AndAlso sPatternName.Length <> 0 Then
					If Me.cmbHatchAngle.Text.Length = 0 Then
						Me.cmbHatchAngle.Text = "0"
					End If
					dAngle = Convert.ToDouble(Me.cmbHatchAngle.Text)
					.Angle = New DMAcadExt.LineAngle(dAngle, False)
					Dim iValue As Integer
					If Me.cmbHatchLine.Text.Length = 0 Then
						Me.cmbHatchLine.SelectedIndex = 1
					End If
					Try
						iValue = Convert.ToInt32(Me.cmbHatchLine.Text)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditColorScheme")
					End Try
					If [Enum].IsDefined(GetType(Autodesk.AutoCAD.DatabaseServices.LineWeight), iValue) Then
						.LineWeight = CType(iValue, Autodesk.AutoCAD.DatabaseServices.LineWeight)
					Else
						.LineWeight = Autodesk.AutoCAD.DatabaseServices.LineWeight.LineWeight000
					End If
					If Me.txtHatchScale.Text.Length = 0 Then
						Me.txtHatchScale.Text = "1"
					End If
					Try
						.PatternScale = Convert.ToDouble(Me.txtHatchScale.Text)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmEditColorScheme")
					End Try
					.PatternColor = mtaDMColors(11)
				End If
			End With
			mtColorScheme.Hatch = tHatch
			DMAcadExt.AcadDocument.WriteDebugMessage("HasHatch: " & CStr(mtColorScheme.HasHatch()) & ":" & CStr(mtColorScheme.Hatch.SolidOnly))
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmEditColorScheme")
		End Try

	End Sub

	Private Sub tlbTop_GotFocus(sender As System.Object, e As System.EventArgs) Handles tlbTop.GotFocus
		zzHideFindByEvent()
	End Sub
	Private Sub tlbTop_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tsbWinColor.Name
				zzSetWinColor()
			Case Me.tsbAcadColor.Name
				zzSetAcadColor()
			Case Me.tsbPaint.Name
				zzBuildColorScheme()
				pcbPicture.Invalidate()
				'	zzPaint(Nothing)
			Case Me.tsbSave.Name
				zzBuildColorScheme()
				'	System.Windows.Forms.MessageBox.Show(mtColorScheme.Name, "18_130")
				zzUpdate()
				zzAddNewToDB()
				If miFormMode <> enColorEditorMode.Landuse Then
					Me.DialogResult = Windows.Forms.DialogResult.OK
				End If
			Case Me.tsbUndo.Name
				zzDisplayColorScheme()
			Case Me.tsbDelete.Name
				zzDeleteStrip()
			Case Me.tsbClear.Name
				zzClearScheme()
				zzClearGraphics()
			Case Me.tsbAddNew.Name
				zzAddNew()
			Case Me.tsbFind.Name

				Me.tsbFind.Checked = Not Me.tsbFind.Checked
				If Me.tsbFind.Checked Then
					zzInitFind()
				Else
					zzHideFind()
				End If
			Case Me.tsbExit.Name
				zzClose()
		End Select
	End Sub
	Public ReadOnly Property NewID() As Integer
		Get
			Return miNewID
		End Get
	End Property
	
	Private Sub zzAddNew()
		zzClearScheme()
		zzClearGraphics()
		miLanduseID = 0
		mtColorSchemeDB = DMAcadExt.ColorScheme.GetEmpty()
		Me.txtColorSchemeID.Text = String.Empty
		Me.txtColorSchemeID.Enabled = True
		Me.txtColorSchemeID.Focus()
		Me.txtColorSchemeName.Text = String.Empty
		Me.cmbColorSchemes.Text = String.Empty
		Me.chkStandard.Checked = False
	End Sub
	Private Sub zzClose()
		Me.Close()
	End Sub
	Private Sub zzClearScheme()
		Me.cmbZebraAngle.Text = "0"
		Me.cmbHatchName.Text = String.Empty
		Me.cmbHatchAngle.Text = String.Empty
		Me.cmbHatchLine.Text = String.Empty
		Me.txtHatchScale.Text = String.Empty

		For iColorIndex As Integer = 0 To 11
			mtaDMColors(iColorIndex) = DMAcadExt.DMColor.GetEmpty()
			zzClearStrip(iColorIndex)
		Next
	End Sub
	Private Sub zzDeleteStrip()
		mtaDMColors(miCurrentColorIndex) = DMAcadExt.DMColor.GetEmpty()
		zzClearStrip(miCurrentColorIndex)
	End Sub

	Private Sub zzDisplayColorScheme()
		If miFormMode <> enColorEditorMode.Landuse Then
			Me.txtColorSchemeName.Text = mtColorSchemeDB.Name
		End If
		zzDisplayZebra(mtColorSchemeDB.Zebra)
		zzDisplayHatch(mtColorSchemeDB.Hatch)
		zzDisplayBorder(mtColorSchemeDB.Border)
	End Sub
	Private Sub zzDisplayZebra(ByVal tZebra As DMAcadExt.ColorZebra)

		Dim tStrip As DMAcadExt.ColorStrip
		Dim iFormIndex As Integer
		If tZebra.IsEmpty Then
			Me.cmbZebraAngle.Text = String.Empty
		ElseIf tZebra.Angle.Quadrant = DMAcadExt.enQuadrants.QuadrantII Then
			Me.rdbAngle(0).Checked = False
			Me.rdbAngle(1).Checked = True
		ElseIf tZebra.Angle.Quadrant = DMAcadExt.enQuadrants.QuadrantI Then
			Me.rdbAngle(0).Checked = True
			Me.rdbAngle(1).Checked = False
		End If
		Me.cmbZebraAngle.Text = Convert.ToString(tZebra.Angle.BaseAngle(False))
		For iIndex As Integer = 0 To tZebra.StripUB
			tStrip = tZebra.Strip(iIndex)
			iFormIndex = iIndex + 6
			zzDisplayStrip(tStrip, iFormIndex)
		Next
		For iIndex As Integer = tZebra.StripUB + 1 To 4
			iFormIndex = iIndex + 6
			zzClearStrip(iFormIndex)
		Next

	End Sub
	Private Sub zzDisplayStrip(ByVal tStrip As DMAcadExt.ColorStrip, ByVal iFormIndex As Integer)
		Dim tDMColor As DMAcadExt.DMColor = tStrip.Color
		txtWidth(iFormIndex).Text = CStr(tStrip.Width)
		mtaDMColors(iFormIndex) = tDMColor
		zzDisplayColor(tDMColor, iFormIndex)
	End Sub
	Private Sub zzClearStrip(ByVal iFormIndex As Integer)
		Dim tDMColor As DMAcadExt.DMColor = Nothing
		zzClearColor(txtFWColor(iFormIndex))
		zzClearColor(txtAcadColor(iFormIndex))
		mtaDMColors(iFormIndex) = tDMColor
		txtWidth(iFormIndex).Text = String.Empty
	End Sub
	Private Sub zzDisplayColor(ByVal tDMColor As DMAcadExt.DMColor, ByVal iFormIndex As Integer)
		If tDMColor.Source = DMAcadExt.DMColor.enSource.Acad Then
			zzSetColor(txtAcadColor(iFormIndex), tDMColor)
			zzClearColor(txtFWColor(iFormIndex))
		ElseIf tDMColor.Source = DMAcadExt.DMColor.enSource.Framework Then
			zzSetColor(txtFWColor(iFormIndex), tDMColor)
			zzClearColor(txtAcadColor(iFormIndex))
		Else
			zzClearColor(txtFWColor(iFormIndex))
			zzClearColor(txtAcadColor(iFormIndex))
		End If
	End Sub
	Private Sub zzDisplayBorder(ByVal tBorder As DMAcadExt.ColorBorder)
		Dim tStrip As DMAcadExt.ColorStrip
		Dim iFormIndex As Integer

		For iIndex As Integer = 0 To tBorder.StripUB

			tStrip = tBorder.Strip(iIndex)
			iFormIndex = iIndex + 1

			zzDisplayStrip(tStrip, iFormIndex)
		Next
		For iIndex As Integer = tBorder.StripUB + 1 To 4
			iFormIndex = iIndex + 1
			zzClearStrip(iFormIndex)
		Next
	End Sub
	Private Sub zzDisplayHatch(ByVal tHatch As DMAcadExt.DMHatch)
		Dim tDMColor As DMAcadExt.DMColor = tHatch.BackColor
		zzDisplayColor(tDMColor, 0)
		mtaDMColors(0) = tDMColor

		tDMColor = tHatch.PatternColor
		zzDisplayColor(tDMColor, 11)
		mtaDMColors(11) = tDMColor

		If tHatch.SolidOnly Then
			Me.cmbHatchAngle.Text = String.Empty
			Me.cmbHatchLine.Text = String.Empty
			Me.txtHatchScale.Text = String.Empty
			Me.cmbHatchName.Text = String.Empty
		Else

			Try
				Me.cmbHatchAngle.Text = CStr(tHatch.Angle.AngleDegree)
			Catch oEx As Exception
			End Try
			Try
				Me.cmbHatchLine.Text = Convert.ToString(tHatch.LineWeightInt)
			Catch oEx As Exception
			End Try
			Me.txtHatchScale.Text = Convert.ToString(tHatch.PatternScale)
			Try
				Me.cmbHatchName.Text = tHatch.PatternName
			Catch oEx As Exception
			End Try
		End If
	End Sub

	Private Sub cmbColorSchemes_GotFocus(sender As System.Object, e As System.EventArgs) Handles cmbColorSchemes.GotFocus
		zzHideFindByEvent()
	End Sub
	Private Sub cmbColorSchemes_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbColorSchemes.SelectedIndexChanged
		'	MessageBox.Show(CStr(miLanduseID) & ":" & CStr(cmbColorSchemes.SelectedIndex) & ":" & CStr(cmbColorSchemes.SelectedValue Is Nothing), "02_850")
		If mbPermitColorSchemeList AndAlso cmbColorSchemes.SelectedIndex <> -1 AndAlso Me.cmbColorSchemes.SelectedValue IsNot Nothing Then
			zzSetBySelect()
			
		End If
	End Sub
	Private Sub zzSetBySelect()
		miLanduseID = DirectCast(Me.cmbColorSchemes.SelectedValue, Integer)


		Try

			mtColorSchemeDB = New DMAcadExt.ColorScheme(miLanduseID, 2.0)
			mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
			'MessageBox.Show(CStr(mtColorScheme.HasBorder) & ":" & CStr(mtColorScheme.HasZebra) & ":" & CStr(mtColorScheme.HasHatch), "18_250")
			Me.txtColorSchemeID.Text = miLanduseID.ToString()
			Me.txtColorSchemeID.Enabled = False
			Me.txtColorSchemeName.Text = mtColorSchemeDB.Name
			Me.chkStandard.Checked = mtColorSchemeDB.IsStandard
			'	MessageBox.Show(CStr(mtColorSchemeDB.ID) & ":" & CStr(mtColorSchemeDB.IsStandard), "18_922")
			'Me.txtColorSchemeName.Tag = saValue(1)

			zzDisplayColorScheme()
			pcbPicture.Invalidate()
			'	zzPaint(Nothing)
		Catch oEx As Exception
		End Try

	End Sub
	Private Sub cmbColorSchemes_SelectedIndexChanged140413(ByVal oSender As System.Object, ByVal e As System.EventArgs) 'Handles cmbColorSchemes.SelectedIndexChanged
		If mbPermitColorSchemeList Then
			Dim sValue As String = Me.cmbColorSchemes.SelectedItem.ToString()
			Dim saValue() As String = Strings.Split(sValue, " - ")
			miLanduseID = 0
			MessageBox.Show(CStr(miLanduseID), "02_851")
			If saValue.GetUpperBound(0) = 1 Then
				Try
					miLanduseID = Convert.ToInt32(saValue(0))
					mtColorSchemeDB = New DMAcadExt.ColorScheme(miLanduseID, 2.0)
					mtColorScheme = DMAcadExt.ColorScheme.GetEmpty()
					'MessageBox.Show(CStr(mtColorScheme.HasBorder) & ":" & CStr(mtColorScheme.HasZebra) & ":" & CStr(mtColorScheme.HasHatch), "18_250")
					Me.txtColorSchemeID.Text = saValue(0)
					Me.txtColorSchemeID.Enabled = False
					Me.txtColorSchemeName.Text = mtColorSchemeDB.Name
					Me.chkStandard.Checked = mtColorSchemeDB.IsStandard
					'	MessageBox.Show(CStr(mtColorSchemeDB.ID) & ":" & CStr(mtColorSchemeDB.IsStandard), "18_922")
					'Me.txtColorSchemeName.Tag = saValue(1)

					zzDisplayColorScheme()
					pcbPicture.Invalidate()
					'	zzPaint(Nothing)
				Catch oEx As Exception
				End Try
			End If
		End If
	End Sub

	Private Sub txtFWColor_LostFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oTextBox As TextBox
		Dim iColorIndex As Integer
		Dim tDMColor As DMAcadExt.DMColor
		Try
			oTextBox = DirectCast(oSender, TextBox)
			iColorIndex = zzGetIndex(oTextBox.Name)
			tDMColor = mtaDMColors(iColorIndex)
			If (oTextBox.Text.Length <> 0) AndAlso (tDMColor.Source <> DMAcadExt.DMColor.enSource.Framework OrElse tDMColor.RGBString <> oTextBox.Text) Then
				tDMColor = New DMAcadExt.DMColor(oTextBox.Text)
				mtaDMColors(iColorIndex) = tDMColor
				zzDisplayColor(tDMColor, iColorIndex)
			ElseIf (oTextBox.Text.Length = 0) AndAlso (tDMColor.Source = DMAcadExt.DMColor.enSource.Framework) Then
				mtaDMColors(iColorIndex) = DMAcadExt.DMColor.GetEmpty()
				zzClearColor(oTextBox)
			End If

		Catch oEx As Exception
		End Try
	End Sub
	Private Sub txtAcadColor_LostFocus(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oTextBox As TextBox
		Dim iColorIndex As Integer
		Dim tDMColor As DMAcadExt.DMColor
		Dim shAcadColorIndex As Short = -1S
		Try
			oTextBox = DirectCast(oSender, TextBox)
			iColorIndex = zzGetIndex(oTextBox.Name)
			Try
				shAcadColorIndex = Convert.ToInt16(oTextBox.Text)
			Catch oEx As Exception
			End Try
			tDMColor = mtaDMColors(iColorIndex)
			If shAcadColorIndex <> -1S AndAlso (tDMColor.Source <> DMAcadExt.DMColor.enSource.Acad OrElse tDMColor.AcadColorIndex <> shAcadColorIndex) Then
				tDMColor = New DMAcadExt.DMColor(shAcadColorIndex)
				mtaDMColors(iColorIndex) = tDMColor
				zzDisplayColor(tDMColor, iColorIndex)
			ElseIf (oTextBox.Text.Length = 0) AndAlso (tDMColor.Source = DMAcadExt.DMColor.enSource.Acad) Then
				mtaDMColors(iColorIndex) = DMAcadExt.DMColor.GetEmpty()
				zzClearColor(oTextBox)
			End If
		Catch oEx As Exception
		End Try
	End Sub
	Private Sub zzClearColor(ByVal oTextBox As TextBox)
		With oTextBox
			.BackColor = SystemColors.Window
			.Text = String.Empty
			.ForeColor = Color.Black
		End With
	End Sub
	Private Sub zzSetColor(ByRef oTextBox As TextBox, ByVal tDMColor As DMAcadExt.DMColor)	', ByVal bAnyway As Boolean
		With oTextBox
			'If bAnyway OrElse .Text <> tDMColor.CommonString Then
			.BackColor = tDMColor.FrameworkColor
			.Text = tDMColor.CommonString
			If tDMColor.FrameworkColor.GetBrightness > 0.5 Then
				'.ForeColor = Color.Black
			Else
				'.ForeColor = Color.White
			End If
			.ForeColor = tDMColor.SelectColor
			'	End If
		End With
	End Sub
	Private Sub txtWidth_DoubleClick(ByVal oSender As System.Object, ByVal e As System.EventArgs)
		Dim oTextBox As TextBox
		Dim iColorIndex As Integer
		Dim tDMColor As DMAcadExt.DMColor
		Try
			oTextBox = DirectCast(oSender, TextBox)
			iColorIndex = zzGetIndex(oTextBox.Name)
			tDMColor = mtaDMColors(iColorIndex)
		Catch oEx As Exception

		End Try

	End Sub

	Private Sub pcbPicture_GotFocus(oSender As System.Object, e As System.EventArgs) Handles pcbPicture.GotFocus
		zzHideFindByEvent()
	End Sub
	Private Sub pcbPicture_Paint(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles pcbPicture.Paint
		zzPaint(e.Graphics)
	End Sub

	Private Sub pcbPicture_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles pcbPicture.Click
		zzBuildColorScheme()
		pcbPicture.Invalidate()
		'	zzPaint(Nothing)
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	
	
	Private Function zzGetCriteria() As String
		Const sAnd As String = " AND "
		Dim sWhereStr As String = String.Empty
		Dim sConnectStr As String = String.Empty
		Dim sFindText As String
		sFindText = Me.txtFind.Text
		If Not String.IsNullOrEmpty(sFindText) Then

			sFindText = Replace(sFindText, "'", "''", 1, -1, vbBinaryCompare)
			sFindText = Replace(sFindText, """", """""", 1, -1, vbBinaryCompare)



			If sFindText.Length <> 0 Then

				sWhereStr = "InStr([Name] ,'" & sFindText & "') <> 0"
				sConnectStr = sAnd
			End If
		End If

		If sWhereStr.Length = 0 Then
			Return String.Empty
		Else
			Return " WHERE " & sWhereStr
		End If


	End Function
	Private Function zzGetCriteriaSQL() As String
		Const gsAnd As String = " AND "
		Dim sWhereStr As String = String.Empty
		Dim sConnectStr As String = String.Empty
		Dim sFindText As String

		sFindText = Me.txtFind.Text
		sFindText = Replace(sFindText, "'", "''", 1, -1, vbBinaryCompare)


		If sFindText.Length <> 0 Then

			sWhereStr = "CHARINDEX( '" & sFindText & "',[Name]) <> 0"
			sConnectStr = gsAnd
		End If


		If sWhereStr.Length = 0 Then
			Return String.Empty
		Else
			Return " WHERE " & sWhereStr
		End If


	End Function
	Private Sub txtFind_TextChanged(oSender As System.Object, e As System.EventArgs) Handles txtFind.TextChanged
		Dim sCriteria As String = zzGetCriteria()
		zzSetFindDataSource(sCriteria)
	End Sub
	Private Sub zzSetFindDataSource(sCriteria As String)
		Dim oDataTable As DataTable = LanduseData.GetFindTable(sCriteria)
		Dim iItemsCount As Integer = oDataTable.Rows.Count
		If iItemsCount = 0 Then
			Me.lstFind.Visible = False
		Else
			mbEventsEnabled = False
			Me.lstFind.Visible = True
			Me.lstFind.DataSource = oDataTable
			If Me.lstFind.SelectedIndex <> -1 Then
				Me.lstFind.SelectedIndex = -1
			End If

			Me.lstFind.Size = New Size(Me.lstFind.Width, zzGetListHeight(iItemsCount))
			mbEventsEnabled = True
		End If


	End Sub

	 

	Private Sub lstFind_SelectedIndexChanged(oSender As System.Object, e As System.EventArgs) Handles lstFind.SelectedIndexChanged

		If mbEventsEnabled AndAlso Me.lstFind.SelectedIndex <> -1 Then
			miLanduseID = DirectCast(Me.lstFind.SelectedValue, Integer)
			Me.cmbColorSchemes.SelectedValue = miLanduseID
			tsbFind.Checked = False
			'	tsbFind.validate()
			'	tlbTop.Invalidate()
			'	Me.Invalidate()
			zzHideFind()
		End If
	End Sub
	Private Class LanduseData
		'	Dim sComText As String = "SELECT ID,Name FROM Landuses_" & CStr(miLandusesFormatID) & "F" & sWhere & " ORDER BY ID"
		Private miLandusesFormatID As Integer = 1
		Private msTableName As String = "Landuses_" & CStr(miLandusesFormatID) & "F"
		Public Shared Function GetFindTable(ByVal sCriteria As String, Optional ByVal bShortName As Boolean = False, Optional ByVal iTop As Integer = -1) As DataTable
			Dim sComText As String = zzGetFindTableComText(sCriteria, iTop)

			Return TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, System.Data.CommandType.Text, "List")
		End Function
		Public Shared Function GetMainTable() As DataTable
			Dim sComText As String = zzGetMainTableComText()

			Return TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, System.Data.CommandType.Text, "List")
		End Function
		Private Shared Function zzGetMainTableComText() As String
			Dim sName As String = "Name"
			Select Case TPlServerDB.ServerDB.CurrentServerDB.Provider
				Case TPlServerDB.TPlProvider.ProviderJet
					Return "SELECT ID,ID & ' - ' & Name AS Name FROM Landuses_1F ORDER BY ID"
				Case TPlServerDB.TPlProvider.ProviderSQLServer
					Return "SELECT ID,ID_Name AS Name FROM Landuses_1F ORDER BY ID"
				Case Else
					Return String.Empty
			End Select

		End Function
		Private Shared Function zzGetFindTableComText(ByVal sCriteria As String, Optional ByVal iTop As Integer = -1) As String
			Dim sTopText As String
			Dim sName As String = "Name"
			If iTop = -1 Then
				sTopText = String.Empty
			Else
				sTopText = " TOP " & CStr(iTop)
			End If
			Return "SELECT " & sTopText & "ID," & sName & " FROM Landuses_1F " & sCriteria & " ORDER BY Name"
		End Function
	End Class
	
	Private Sub frmColorEditor_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		mbEventsEnabled = True
		'	MessageBox.Show(miFormMode.ToString(), "02_856")

		If miFormMode = enColorEditorMode.Landuse Then
			If Me.cmbColorSchemes.Items.Count > 0 Then
				If miColorSchemeID = 0 Then
					Me.cmbColorSchemes.SelectedIndex = 0
				Else
					Me.cmbColorSchemes.SelectedValue = miColorSchemeID
				End If

				zzSetBySelect()
			End If
		End If

		'	

	End Sub
End Class

