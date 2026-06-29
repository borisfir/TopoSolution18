Option Explicit On
Option Strict On
Public Class frmMPgonView
	Const msAltLabelText As String = "מס' איים "
	Private moXDataParcel As DMAcadExt.TplnXDataParcel
	Public Sub New(oXDataParcel As DMAcadExt.TplnXDataParcel)

		' This call is required by the designer.
		InitializeComponent()
		moXDataParcel = oXDataParcel
		zzMyInitializeComponent()
	End Sub
	Private Sub zzMyInitializeComponent()
		Me.txtIDImp.Text = Convert.ToString(moXDataParcel.ID)
		Me.txtID.Text = Convert.ToString(moXDataParcel.DataID)
		Me.txtBlock.Text = Convert.ToString(moXDataParcel.Block)
		Me.txtBlockAdd.Text = Convert.ToString(moXDataParcel.BlockAdd)
		Me.txtParcel.Text = Convert.ToString(moXDataParcel.Name)
		Me.txtLegalArea.Text = Convert.ToString(moXDataParcel.LegalArea)
		Me.txtAcadArea.Text = FormatNumber(moXDataParcel.AcadArea, 3, TriState.True, , TriState.True)
		Me.txtNeigborList.Text = moXDataParcel.NeigborList
		If moXDataParcel.Exterior <> 0 Then
			Me.txtExterior.Text = Convert.ToString(moXDataParcel.Exterior)
		End If
		If String.IsNullOrEmpty(moXDataParcel.InteriorList) Then
			Me.txtInteriorList.Text = Convert.ToString(moXDataParcel.InteriorCount)
			Me.lblIsland.Text = msAltLabelText
		Else
			Me.txtInteriorList.Text = moXDataParcel.InteriorList

		End If

	End Sub

	Private Sub cmdClose_Click(sender As System.Object, e As System.EventArgs) Handles cmdClose.Click
		Me.Close()
	End Sub

	Private Sub cmdNext_Click(oSender As System.Object, e As System.EventArgs) Handles cmdNext.Click
		Me.Close()
      FDO.TplnPolygonSet.PgonView()
	End Sub

	Private Sub txtBlock_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtBlock.TextChanged

	End Sub
End Class