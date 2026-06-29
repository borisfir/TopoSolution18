Option Explicit On
Option Strict On
Imports System.Windows.Forms
Public Class EditColorSchemeOld
	Private miColorUB As Integer = 12
	Private moFWColor(miColorUB) As TextBox
	Private moAcadColor(miColorUB) As TextBox
	Private moWidth(miColorUB) As TextBox
	Private Sub zzMyInitializeComponent()
		Dim iFirstTop As Integer
		For iIndex As Integer = 0 To miColorUB
			Select Case iIndex
				Case 0
				Case 1 To 5
					iFirstTop = -20
					Me.grbBorder.Controls.Add(Me.moFWColor(iIndex))
				Case 6 To 10
					iFirstTop = -120
					Me.grbZebra.Controls.Add(Me.moFWColor(iIndex))
				Case 11

			End Select
			moFWColor(iIndex) = New TextBox
			'
			'moFWColor
			'
			With Me.moFWColor(iIndex)
				'  .Location = New System.Drawing.Point(302, 84)
				.Location = New System.Drawing.Point(2, iFirstTop + 22 * iIndex)
				.Name = "txtFWColor_" & CStr(iIndex)
				.Size = New System.Drawing.Size(36, 16)
				.TextAlign = HorizontalAlignment.Left
			End With
			'	Me.Controls.Add(Me.moFWColor(iIndex))
		Next
	End Sub
	Private Sub EditColorScheme_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Stop
	End Sub
	Private Class ColorWidth
		Inherits System.Windows.Forms.Control
		Private WithEvents mtxtFrameColor As TextBox
		Private WithEvents mtxtAcadColor As TextBox
		Private WithEvents mtxtWidth As TextBox
		Public Sub New()
			mtxtFrameColor = New TextBox
			mtxtAcadColor = New TextBox
			mtxtWidth = New TextBox
		End Sub




		Private Sub ColorWidth_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click

		End Sub

		Private Sub mtxtAcadColor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mtxtAcadColor.Click
			'	RaiseEvent Click()
		End Sub
	End Class

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
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