Option Explicit On
Option Strict On
Imports System.Windows.Forms
Imports System.ComponentModel
Public Class Form1
	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		Me.ComboBox1 = New System.Windows.Forms.ComboBox

		'
		'ComboBox1
		'
		Me.ComboBox1.FormattingEnabled = True
		Me.ComboBox1.Location = New System.Drawing.Point(22, 26)
		Me.ComboBox1.Name = "ComboBox1"
		Me.ComboBox1.Size = New System.Drawing.Size(183, 21)
		Me.ComboBox1.TabIndex = 0
		Me.Controls.Add(Me.ComboBox1)

		'	Me.ComboBox1.ValueMember = "ListIndex"
		'	Me.ComboBox1.DisplayMember = "ListDispData"


		Dim oItem As ItemData
		oItem = New ItemData(11, "aaaa")
		Me.ComboBox1.Items.Add(oItem)
		oItem = New ItemData(12, "bbbb")
		Me.ComboBox1.Items.Add(oItem)

		oItem = New ItemData(13, "cccc")
		Me.ComboBox1.Items.Add(oItem)

	End Sub


	<LookupBindingProperties( _
  "DataSource", _
  "DisplayMember", _
  "ValueMember", _
  "SelectedValue")> _
  Public Class DemoComboBox
		Inherits System.Windows.Forms.ComboBox
	End Class

	Private Sub ComboBox1_SelectedValueChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedValueChanged
		Stop
	End Sub
End Class