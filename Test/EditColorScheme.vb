Option Explicit On
Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports TopoManager
Public Class EditColorScheme
	Private miColorUB As Integer = 11
	Private moFWColor(miColorUB) As TextBox
	Private moAcadColor(miColorUB) As TextBox
	Private moWidth(miColorUB) As TextBox
	Private mlblColor(miColorUB) As Label
	Private mtDMColors(miColorUB) As DMAcadExt.DMColor
	Private mlblCaption(5) As Label
	Private mlblAngle As Label
	Private mlblHatchName As Label

	Private mcmbAngle As ComboBox
	Private mcmbHatchAngle As ComboBox
	Private mcmbHatchLine As ComboBox

	Private mtxtHatchScale As TextBox
	Private mlblHatchScale As Label
	Private mlblHatchAngle As Label
	Private mlblHatchLine As Label
	Private WithEvents lblMark As System.Windows.Forms.Label

	Private mcmbHatchName As ComboBox
	Private mrdbAngle(1) As RadioButton
	Private miCurrentColorIndex As Integer
	Private Sub zzMyInitializeComponent()

		Dim oFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Dim sColorText As String
		'		Dim tColorlabelSize As System.Drawing.Size
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
			moFWColor(iIndex) = New TextBox
			moAcadColor(iIndex) = New TextBox
			moWidth(iIndex) = New TextBox
			mlblColor(iIndex) = New Label
			AddHandler moFWColor(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler moAcadColor(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler moWidth(iIndex).GotFocus, AddressOf Color_GotFocus
			AddHandler mlblColor(iIndex).Click, AddressOf Color_GotFocus
			Select Case iIndex
				Case 0
					Me.grbFill.Controls.Add(Me.moFWColor(iIndex))
					Me.grbFill.Controls.Add(Me.moAcadColor(iIndex))
					Me.grbFill.Controls.Add(Me.mlblColor(iIndex))
					sColorText = "RGB/Acad"
				Case 1 To 5
					Me.grbBorder.Controls.Add(Me.moFWColor(iIndex))
					Me.grbBorder.Controls.Add(Me.moAcadColor(iIndex))
					Me.grbBorder.Controls.Add(Me.moWidth(iIndex))
					Me.grbBorder.Controls.Add(Me.mlblColor(iIndex))
					sColorText = Chr(223 + iIndex) & "'"
				Case 6 To 10
					Me.grbZebra.Controls.Add(Me.moFWColor(iIndex))
					Me.grbZebra.Controls.Add(Me.moAcadColor(iIndex))
					Me.grbZebra.Controls.Add(Me.moWidth(iIndex))
					Me.grbZebra.Controls.Add(Me.mlblColor(iIndex))
					sColorText = Chr(218 + iIndex) & "'"
				Case 11
					Me.grbHatch.Controls.Add(Me.moFWColor(iIndex))
					Me.grbHatch.Controls.Add(Me.moAcadColor(iIndex))
					Me.grbHatch.Controls.Add(Me.mlblColor(iIndex))
					sColorText = "RGB/Acad"
				Case Else
					sColorText = String.Empty
			End Select

			'
			'moFWColor
			'
			With Me.moFWColor(iIndex)
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
			With Me.moAcadColor(iIndex)
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
			With Me.moWidth(iIndex)
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
			mrdbAngle(iIndex) = New RadioButton
			mrdbAngle(iIndex).Size = New System.Drawing.Size(32, 16)
			mrdbAngle(iIndex).Top = 18

			If iIndex = 0 Then
				mrdbAngle(iIndex).Left = 2
				mrdbAngle(iIndex).Text = "/"
				mrdbAngle(iIndex).Checked = True
			Else
				mrdbAngle(iIndex).Left = 40
				mrdbAngle(iIndex).Text = "\"
			End If
			Me.grbZebra.Controls.Add(mrdbAngle(iIndex))
		Next
		mlblAngle = New Label
		With mlblAngle
			.Size = New System.Drawing.Size(40, 16)
			.Text = "ז ו ו י ת"
			.Font = oFont
			.Location = New System.Drawing.Point(140, 18)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbZebra.Controls.Add(mlblAngle)
		mcmbAngle = New ComboBox
		With mcmbAngle
			.AutoSize = False
			.Size = New System.Drawing.Size(48, 16)
			.Location = New System.Drawing.Point(88, 18)
			.Font = oFont
			.Items.Add(0)
			.Items.Add(15)
			.Items.Add(30)
			.Items.Add(45)
			.Items.Add(60)
			.Items.Add(75)
			.Items.Add(90)
		End With
		Me.grbZebra.Controls.Add(mcmbAngle)
		mcmbHatchName = New ComboBox
		With mcmbHatchName
			.AutoSize = False
			.Size = New System.Drawing.Size(64, 16)
			.Location = New System.Drawing.Point(80, 16)
			.Font = oFont
			.Items.Add("LINE")
			.Items.Add("NET")
		End With
		Me.grbHatch.Controls.Add(mcmbHatchName)
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

		Me.mcmbHatchAngle = New ComboBox
		With Me.mcmbHatchAngle
			.AutoSize = False
			.Size = New System.Drawing.Size(40, 16)
			.Location = New System.Drawing.Point(4, 72)
			.Font = oFont
			.Items.Add(0)
			.Items.Add(15)
			.Items.Add(30)
			.Items.Add(45)
			.Items.Add(60)
			.Items.Add(75)
			.Items.Add(90)

			Me.grbHatch.Controls.Add(mcmbHatchAngle)
		End With

		Me.mtxtHatchScale = New TextBox
		With Me.mtxtHatchScale
			.Location = New System.Drawing.Point(5, 16)
			.Name = "mtxtHatchScale"
			.Size = New System.Drawing.Size(36, 16)
			.Font = oFont
			.TextAlign = HorizontalAlignment.Left
		End With
		Me.grbHatch.Controls.Add(mtxtHatchScale)
		Me.mlblHatchScale = New Label
		With Me.mlblHatchScale
			.Size = New System.Drawing.Size(32, 16)
			.Text = "קנ""מ"
			.Font = oFont
			.Location = New System.Drawing.Point(40, 16)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.mlblHatchScale)
		Me.mlblHatchAngle = New Label
		With Me.mlblHatchAngle
			.Size = New System.Drawing.Size(32, 16)
			.Text = "זווית"
			.Font = oFont
			.Location = New System.Drawing.Point(40, 72)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.mlblHatchAngle)
		Me.mcmbHatchLine = New ComboBox
		Dim iaLines() As Object = {12, 15, 30}
		With Me.mcmbHatchLine
			.Font = oFont
			mcmbHatchLine.Items.AddRange(iaLines)
			mcmbHatchLine.Size = New System.Drawing.Size(46, 16)
			mcmbHatchLine.Location = New System.Drawing.Point(80, 72)
		End With
		Me.grbHatch.Controls.Add(Me.mcmbHatchLine)
		Me.mlblHatchLine = New Label
		With Me.mlblHatchLine
			.Size = New System.Drawing.Size(50, 16)
			.Text = "רוחב קו"
			.Font = oFont
			.Location = New System.Drawing.Point(124, 72)
			.BorderStyle = BorderStyle.None
		End With
		Me.grbHatch.Controls.Add(Me.mlblHatchLine)

		Me.lblMark.Location = New System.Drawing.Point(174, 0)


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
	Private Sub Color_GotFocus(ByVal oSender As Object, ByVal e As System.EventArgs)
		Dim oControl As Control
		Try
			oControl = DirectCast(oSender, Control)
			zzSetCurrentColorIndex(oControl.Name)
		Catch ex As Exception
		End Try

	End Sub
	Private Sub zzSetCurrentColorIndex(ByVal sControlName As String)
		Dim saPart() As String = Strings.Split(sControlName, "_")
		If saPart.GetUpperBound(0) = 1 Then
			Try
				miCurrentColorIndex = CInt(saPart(1))
			Catch ex As Exception
			End Try
		End If
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


	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub

	Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

	End Sub

	Private Sub tcbWinColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tcbWinColor.Click
		Dim oRes As DialogResult = Me.cdlWindow.ShowDialog()
		Dim oColor As Color = Me.cdlWindow.Color
		moFWColor(miCurrentColorIndex).BackColor = oColor
		' = CStr(oColor.R) & "," & CStr(oColor.G) & "," & CStr(oColor.B)
		mtDMColors(miColorUB) = New DMAcadExt.DMColor(oColor)
		moFWColor(miCurrentColorIndex).Text = mtDMColors(miColorUB).RGBString
		Dim d As Double = oColor.GetBrightness
		Stop
	End Sub

	Private Sub zzPaint()
		Dim oRect As Rectangle = New Rectangle(Me.pcbPicture.Location, Me.pcbPicture.ClientSize)
		Dim oPaint As PainZebra = New PainZebra(Me.pcbPicture.CreateGraphics, oRect)
		Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
		Dim dWidth As Double
		For iBorderIndex As Integer = 1 To 6
			If moWidth(iBorderIndex).Text.Length = 0 Then
				Exit For
			Else
				dWidth = Convert.ToDouble(moWidth(iBorderIndex).Text)
				tColorScheme.AddBorderStrip(mtDMColors(iBorderIndex), dWidth)
			End If
		Next
		oPaint.ZebraScheme = tColorScheme.Zebra
		oPaint.Draw()
	End Sub

	Private Sub tlbTop_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tlbTop.ItemClicked
		Dim oToolStripItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
		Select Case oToolStripItem.Name
			Case Me.tcbPaint.Name
				zzPaint()
		End Select


	End Sub

	Private Sub EditColorScheme_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Dim oColor As Color = Color.FromArgb(255, 255, 0, 0)
		Dim snB As Single = oColor.GetBrightness()
		oColor = Color.FromArgb(255, 255, 255, 0)
		snB = oColor.GetBrightness()
		oColor = Color.FromArgb(255, 255, 255, 255)
		snB = oColor.GetBrightness()
		oColor = Color.FromArgb(255, 120, 120, 120)
		snB = oColor.GetBrightness()
	End Sub
End Class

Public Class dmPictureBox
	Inherits PictureBox
	Property ScaleLeft() As Single
		Get

		End Get
		Set(ByVal snValue As Single)

		End Set
	End Property
	Property ScaleTop() As Single
		Get

		End Get
		Set(ByVal snValue As Single)

		End Set
	End Property
End Class