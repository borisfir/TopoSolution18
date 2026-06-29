Public Class Form1
	Const msRootPath As String = "R:\Gushim\גושים ממוחשבים"
	Private msCurrentRootName As String = msRootPath

	Private WithEvents Panel0 As System.Windows.Forms.Panel
	Private WithEvents Panel1 As System.Windows.Forms.Panel
	Private WithEvents Panel2 As System.Windows.Forms.Panel
	Private WithEvents Panel3 As System.Windows.Forms.Panel
	Private chkCreateCentroidAAA As CheckBox
	Private chkHighlightSliverAAA As CheckBox
	Private txtToleranceAAA As TextBox
	Private lblToleranceAAA As Label
	Protected Shared moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		Dim daSourceArea() As Double = {606.312061814591, 4.01483931241091, 0.491793727967888, 0.0235748291015625}
      Dim oBalanceArea As TopoManager.BalanceArea = New TopoManager.BalanceArea(daSourceArea, 1.0, 611, False, "Form1")

	End Sub


	
	
	Private Class MyComparer
		Implements IComparer(Of TopoManager.NumerationPair.ComplexNum)

		Public Function Compare(x As TopoManager.NumerationPair.ComplexNum, y As TopoManager.NumerationPair.ComplexNum) As Integer Implements System.Collections.Generic.IComparer(Of TopoManager.NumerationPair.ComplexNum).Compare
			If x.Order > y.Order Then
				Return 1
			ElseIf x.Order = y.Order Then
				Return 0
			Else
				Return -1
			End If
		End Function
	End Class

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()
		'zzInitPanel1()
		' Add any initialization after the InitializeComponent() call.

	End Sub
	Private Sub zzInitPanel1()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.chkCreateCentroid = New CheckBox
		Me.chkHighlightSliver = New CheckBox
		Me.txtTolerance = New TextBox
		Me.lblTolerance = New Label

		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		Me.Panel1.Controls.Add(Me.chkCreateCentroid)
		Me.Panel1.Controls.Add(Me.chkHighlightSliver)
		Me.Panel1.Controls.Add(Me.txtTolerance)
		Me.Panel1.Controls.Add(Me.lblTolerance)

		'
		'chkCreateCentroid
		'
		With Me.chkCreateCentroid
			' .Location = New System.Drawing.Point(302, 60)
			.Location = New System.Drawing.Point(10, 10)
			.Name = "chkCreateCentroid"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 16
			.Font = moLabelFont
			.Text = "Insert Centroid"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
		End With
		'
		'chkHighlight Sliver
		'
		With Me.chkHighlightSliver
			.Location = New System.Drawing.Point(10, 42)
			.Name = "chkHighlightSliver"
			.Size = New System.Drawing.Size(120, 24)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "Highlight Sliver"
			.FlatStyle = FlatStyle.Standard
			.ThreeState = False
			.Checked = True
		End With


		'
		'txtTolerance
		'
		With Me.txtTolerance
			'  .Location = New System.Drawing.Point(302, 84)
			.Location = New System.Drawing.Point(100, 74)
			.Name = "txtTolerance"
			.Size = New System.Drawing.Size(36, 16)
			.TabIndex = 17
			.Font = moLabelFont
			.Text = "0.01"
			.TextAlign = HorizontalAlignment.Left
		End With

		'
		'lblTolerance
		'
		With Me.lblTolerance
			'.Location = New System.Drawing.Point(342, 86)
			.Location = New System.Drawing.Point(10, 74)
			.Name = "lblTolerance"
			.Size = New System.Drawing.Size(80, 24)
			.TabIndex = 18
			.Font = moLabelFont
			.Text = "Tolerance"

		End With

		Me.Panel1.Location = New System.Drawing.Point(0, 0)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(300, 244)
		Me.Panel1.TabIndex = 4
		Me.Panel1.BackColor = Color.Aqua
		Me.Panel1.RightToLeft = Windows.Forms.RightToLeft.No

		Me.Panel1.ResumeLayout(False)
		Me.Controls.Add(Me.Panel1)

	End Sub

	
End Class