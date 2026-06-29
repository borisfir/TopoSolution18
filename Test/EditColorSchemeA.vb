Imports System.Windows.Forms
Imports TopoManager
Imports DMAcadExt
Imports System.Drawing
Public Class EditColorSchemeA

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Dim tZebra As ColorZebra = Nothing
		tZebra.Angle = New LineAngle(45, False)
		tZebra.AddStrip(New DMColor(Drawing.Color.Chocolate), 40)
		tZebra.AddStrip(New DMColor(Drawing.Color.Cyan), 55)
		tZebra.AddStrip(New DMColor(Drawing.Color.DarkBlue), 30)

		Dim oGraph As Graphics = Me.pcbPicture.CreateGraphics

		Dim oRect As Rectangle = New Rectangle(-20, -20, Me.pcbPicture.Width + 40, Me.pcbPicture.Height + 40)
		Dim oPainZebra As PainZebra = New PainZebra(oGraph, oRect)
		oPainZebra.ZebraScheme = tZebra
		oPainZebra.Draw()
	End Sub

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.

	End Sub

	Private Sub pcbPicture_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pcbPicture.Click

	End Sub
End Class
Public Class dmPictureBoxA
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