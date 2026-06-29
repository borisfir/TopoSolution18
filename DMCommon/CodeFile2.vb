Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections

Public Class comboBoxSample4
	Inherits Form

	Private ComboBox1 As New ComboBox()
	Private label1 As New Label()
	Private textBox1 As New TextBox()

	<STAThread()> _
	Shared Sub Main()
		Application.Run(New comboBoxSample4())
	End Sub 'Main

	Public Sub New()
		Me.ClientSize = New Size(307, 206)
		Me.Text = "ComboBox Sample3"

		ComboBox1.Location = New Point(54, 16)
		ComboBox1.Name = "ComboBox1"
		ComboBox1.Size = New Size(240, 130)

		label1.Location = New Point(14, 150)
		label1.Name = "label1"
		label1.Size = New Size(40, 24)
		label1.Text = "Value"

		textBox1.Location = New Point(54, 150)
		textBox1.Name = "textBox1"
		textBox1.Size = New Size(240, 24)

		Me.Controls.AddRange(New Control() {ComboBox1, label1, textBox1})

		' Populate the list box using an array as DataSource. 
		Dim USStates As New ArrayList()
		USStates.Add(New USState("Alabama", "AL"))
		USStates.Add(New USState("Washington", "WA"))
		USStates.Add(New USState("West Virginia", "WV"))
		USStates.Add(New USState("Wisconsin", "WI"))
		USStates.Add(New USState("Wyoming", "WY"))
		ComboBox1.DataSource = USStates

		' Set the long name as the property to be displayed and the short
		' name as the value to be returned when a row is selected.  Here
		' these are properties; if we were binding to a database table or
		' query these could be column names.
		ComboBox1.DisplayMember = "LongName"
		ComboBox1.ValueMember = "ShortName"

		' Bind the SelectedValueChanged event to our handler for it.
		AddHandler ComboBox1.SelectedValueChanged, AddressOf ListBox1_SelectedValueChanged

		' Ensure the form opens with no rows selected.

	End Sub 'New

	Private Sub InitializeComponent()
	End Sub 'InitializeComponent

	Private Sub ListBox1_SelectedValueChanged(ByVal sender As Object, ByVal e As EventArgs)
		If ComboBox1.SelectedIndex <> -1 Then
			textBox1.Text = ComboBox1.SelectedValue.ToString()
			' If we also wanted to get the displayed text we could use
			' the SelectedItem item property:
			' Dim s = CType(ListBox1.SelectedItem, USState).LongName
		End If
	End Sub 'ListBox1_SelectedValueChanged
End Class 'ListBoxSample3



