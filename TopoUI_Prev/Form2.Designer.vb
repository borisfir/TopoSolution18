<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
		Dim ListViewGroup1 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("GroupA", System.Windows.Forms.HorizontalAlignment.Center)
		Dim ListViewGroup2 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("GroupB", System.Windows.Forms.HorizontalAlignment.Left)
		Dim ListViewGroup3 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("GroupC", System.Windows.Forms.HorizontalAlignment.Left)
		Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem(New System.Windows.Forms.ListViewItem.ListViewSubItem() {New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, "Item01", System.Drawing.SystemColors.WindowText, System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer)), New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))), New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, "a01", System.Drawing.Color.Maroon, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(128, Byte), Integer)), New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))), New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, "V", System.Drawing.SystemColors.WindowText, System.Drawing.Color.LawnGreen, New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))), New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, "Weq", System.Drawing.SystemColors.WindowText, System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer)), New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte)))}, -1)
		Dim ListViewItem2 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Item02")
		Dim ListViewItem3 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Item11")
		Dim ListViewItem4 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("item_aa")
		Me.ListView1 = New System.Windows.Forms.ListView()
		Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.PictureBox1 = New System.Windows.Forms.PictureBox()
		Me.ListView2 = New System.Windows.Forms.ListView()
		Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.Button2 = New System.Windows.Forms.Button()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'ListView1
		'
		Me.ListView1.CheckBoxes = True
		Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
		Me.ListView1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.ListView1.FullRowSelect = True
		Me.ListView1.GridLines = True
		ListViewGroup1.Header = "GroupA"
		ListViewGroup1.HeaderAlignment = System.Windows.Forms.HorizontalAlignment.Center
		ListViewGroup1.Name = "ListViewGroup1"
		ListViewGroup2.Header = "GroupB"
		ListViewGroup2.Name = "ListViewGroup2"
		ListViewGroup3.Header = "GroupC"
		ListViewGroup3.Name = "ListViewGroup3"
		Me.ListView1.Groups.AddRange(New System.Windows.Forms.ListViewGroup() {ListViewGroup1, ListViewGroup2, ListViewGroup3})
		ListViewItem1.Checked = True
		ListViewItem1.Group = ListViewGroup1
		ListViewItem1.IndentCount = 80
		ListViewItem1.StateImageIndex = 1
		ListViewItem1.UseItemStyleForSubItems = False
		ListViewItem2.Group = ListViewGroup1
		ListViewItem2.IndentCount = 50
		ListViewItem2.StateImageIndex = 0
		ListViewItem3.Group = ListViewGroup2
		ListViewItem3.StateImageIndex = 0
		Me.ListView1.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1, ListViewItem2, ListViewItem3})
		Me.ListView1.Location = New System.Drawing.Point(428, 4)
		Me.ListView1.Name = "ListView1"
		Me.ListView1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.ListView1.RightToLeftLayout = True
		Me.ListView1.ShowItemToolTips = True
		Me.ListView1.Size = New System.Drawing.Size(424, 598)
		Me.ListView1.TabIndex = 0
		Me.ListView1.TileSize = New System.Drawing.Size(5, 5)
		Me.ListView1.UseCompatibleStateImageBehavior = False
		Me.ListView1.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "ColA"
		Me.ColumnHeader1.Width = 102
		'
		'ColumnHeader2
		'
		Me.ColumnHeader2.Text = "ColB"
		Me.ColumnHeader2.Width = 76
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "ColC"
		'
		'ColumnHeader4
		'
		Me.ColumnHeader4.Width = 82
		'
		'Label1
		'
		Me.Label1.BackColor = System.Drawing.Color.Transparent
		Me.Label1.Location = New System.Drawing.Point(465, 320)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(199, 212)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "A"
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(296, 4)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(87, 24)
		Me.Button1.TabIndex = 2
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'PictureBox1
		'
		Me.PictureBox1.BackColor = System.Drawing.SystemColors.Window
		Me.PictureBox1.Location = New System.Drawing.Point(440, 45)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New System.Drawing.Size(30, 558)
		Me.PictureBox1.TabIndex = 3
		Me.PictureBox1.TabStop = False
		'
		'ListView2
		'
		Me.ListView2.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5})
		Me.ListView2.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem4})
		Me.ListView2.Location = New System.Drawing.Point(864, 153)
		Me.ListView2.Name = "ListView2"
		Me.ListView2.OwnerDraw = True
		Me.ListView2.Size = New System.Drawing.Size(318, 268)
		Me.ListView2.TabIndex = 4
		Me.ListView2.UseCompatibleStateImageBehavior = False
		Me.ListView2.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader5
		'
		Me.ColumnHeader5.Text = "ColMain"
		'
		'Button2
		'
		Me.Button2.Image = Global.TopoUI.My.Resources.Resources.Exit24Tr
		Me.Button2.Location = New System.Drawing.Point(244, 45)
		Me.Button2.Margin = New System.Windows.Forms.Padding(2)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(123, 83)
		Me.Button2.TabIndex = 5
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Form2
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1260, 616)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.ListView2)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.ListView1)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "Form2"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "Form2"
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub
	Private WithEvents ListView1 As System.Windows.Forms.ListView
	Private WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
	Private WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
	Private WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
	Friend WithEvents Label1 As System.Windows.Forms.Label

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		AddHandler Me.ListView1.Paint, AddressOf ListView1_Paint
		AddHandler Me.ListView2.Paint, AddressOf ListView1_Paint

		AddHandler Me.Label1.Paint, AddressOf ListView1_Paint
		InitializeIndentedListViewItems()
	End Sub

	Private Sub Form2_Load(ByVal oSender As System.Object, e As System.EventArgs) Handles Me.Load
		mbEnabled = True

	End Sub
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
	Friend WithEvents ListView2 As System.Windows.Forms.ListView
	Private WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
	Private indentedListView As ListView


	Private Sub InitializeIndentedListViewItems()
		indentedListView = New ListView()
		indentedListView.Width = 200

		' View must be set to Details to use IndentCount.
		indentedListView.View = View.Details
		indentedListView.Columns.Add("Indented Items", 150)

		' Create an image list and add an image. 
		Dim list As New ImageList()
		list.Images.Add(New Bitmap(GetType(Button), "Button.bmp"))

		' SmallImageList must be set when using IndentCount.
		indentedListView.SmallImageList = list

		Dim item1 As New ListViewItem("Click", 0)
		item1.IndentCount = 1
		Dim item2 As New ListViewItem("OK", 0)
		item2.IndentCount = 2
		Dim item3 As New ListViewItem("Cancel", 0)
		item3.IndentCount = 3
		indentedListView.Items.AddRange(New ListViewItem() {item1, item2, item3})

		' Add the controls to the form. 
		Me.Controls.Add(indentedListView)

	End Sub
	Friend WithEvents Button2 As System.Windows.Forms.Button 'InitializeIndentedListViewItems 
End Class
