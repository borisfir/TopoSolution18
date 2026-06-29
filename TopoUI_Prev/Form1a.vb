Public Class Form1a
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



End Class