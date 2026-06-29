Option Explicit On
Option Strict On
Public Class frmMPgonView
	Private moXDataParcel As DMAcadExt.TplnXDataParcel
	Public Sub New(oXDataParcel As DMAcadExt.TplnXDataParcel)

		' This call is required by the designer.
		InitializeComponent()
		moXDataParcel = oXDataParcel
		zzMyInitializeComponent()
	End Sub
	Private Sub zzMyInitializeComponent()
		Me.txtBlock.Text = Convert.ToString(moXDataParcel.Block)
		Me.txtBlockAdd.Text = Convert.ToString(moXDataParcel.BlockAdd)
		Me.txtParcel.Text = Convert.ToString(moXDataParcel.Name)

	End Sub
End Class