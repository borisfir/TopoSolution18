Public Class frmMsgBox

	Private Sub frmMsgBox_Load(oSender As System.Object, e As EventArgs) Handles MyBase.Load

	End Sub

	Public Sub New(sText As String)

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      Me.txtMain.Text = sText
   End Sub

	Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles cndOkCancel.Click
		Me.Close()
	End Sub
End Class