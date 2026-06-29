Public Class Form2
	Private mbEnabled As Boolean = False
	Private Sub ListView1_ColumnWidthChanged(ByVal oSender As System.Object, e As System.Windows.Forms.ColumnWidthChangedEventArgs) Handles ListView1.ColumnWidthChanged
	End Sub

	Private Sub ListView1_ColumnWidthChanging(ByVal oSender As System.Object, e As System.Windows.Forms.ColumnWidthChangingEventArgs) Handles ListView1.ColumnWidthChanging

	End Sub

	Private Sub ListView1_DrawColumnHeader(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewColumnHeaderEventArgs) Handles ListView1.DrawColumnHeader
		e.DrawBackground()
		e.DrawText()
	End Sub

	Private Sub ListView1_DrawItem(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewItemEventArgs) Handles ListView1.DrawItem
		'	e.DrawBackground()
		'	e.DrawText()
		e.DrawDefault = True
	End Sub

	Private Sub ListView1_DrawSubItem(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewSubItemEventArgs) Handles ListView1.DrawSubItem
		'	e.DrawBackground()
		'e.DrawText()
		e.DrawDefault = True
	End Sub



	Private Sub ListView1_Paint(ByVal oSender As System.Object, e As System.Windows.Forms.PaintEventArgs)
		Dim g As System.Drawing.Graphics = e.Graphics
		Dim oPen As System.Drawing.Pen
		Dim tResRect As System.Drawing.Rectangle
		Dim tDrawLocation As System.Drawing.Point
		Dim tDrawSize As System.Drawing.Size

		tDrawLocation = New Point(12, 12)
		oPen = New System.Drawing.Pen(Color.DarkMagenta, 4)

		tDrawSize = New System.Drawing.Size(40, 140)
		'	tResRect = New System.Drawing.Rectangle(maControlExt(iIndex).Location, maControlExt(iIndex).Size)
		tResRect = New System.Drawing.Rectangle(tDrawLocation, tDrawSize)
		g.DrawRectangle(oPen, tResRect)

	End Sub



	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Me.ListView1.Invalidate()
		Me.ListView2.Invalidate()

		Me.Label1.Invalidate()

	End Sub

	Private Sub ListView2_DrawColumnHeader(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewColumnHeaderEventArgs) Handles ListView2.DrawColumnHeader
		e.DrawBackground()
		e.DrawText()
	End Sub

	Private Sub ListView2_DrawItem(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewItemEventArgs) Handles ListView2.DrawItem
		e.DrawBackground()
		e.DrawText()
		'Stop
	End Sub

	Private Sub ListView2_DrawSubItem(ByVal oSender As System.Object, e As System.Windows.Forms.DrawListViewSubItemEventArgs) Handles ListView2.DrawSubItem
		'Stop
	End Sub


	Private Sub PictureBox1_Paint(ByVal oSender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
		Dim g As System.Drawing.Graphics = e.Graphics
		Dim oPen As System.Drawing.Pen
		Dim tResRect As System.Drawing.Rectangle
		Dim tDrawLocation As System.Drawing.Point
		Dim tDrawSize As System.Drawing.Size
		Dim tPointTop As Point = New Point(1, 1)
		Dim tPointBottom As Point = New Point(1, 81)
		tDrawLocation = New Point(12, 12)
		oPen = New System.Drawing.Pen(Color.DarkOrange, 4)

		tDrawSize = New System.Drawing.Size(40, 140)
		'	tResRect = New System.Drawing.Rectangle(maControlExt(iIndex).Location, maControlExt(iIndex).Size)
		tResRect = New System.Drawing.Rectangle(tDrawLocation, tDrawSize)
		g.DrawLine(oPen, tPointTop, tPointBottom)
	End Sub

	Private Sub ListView1_ItemCheck(ByVal oSender As System.Object, e As System.Windows.Forms.ItemCheckEventArgs) Handles ListView1.ItemCheck

		If mbEnabled Then
			e.NewValue = e.CurrentValue
		End If

	End Sub

	Private Sub ListView1_ItemChecked(ByVal oSender As System.Object, e As System.Windows.Forms.ItemCheckedEventArgs) Handles ListView1.ItemChecked
		'	Stop
	End Sub

	Private Sub ListView1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ListView1.SelectedIndexChanged

	End Sub

	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
		zzInitDB()
		Dim f As TopoManager.frmColorEditor = New TopoManager.frmColorEditor(TopoManager.enColorEditorMode.Landuse, False)
	End Sub
	Private Sub zzInitDB()
		Const sServerName As String = "Pluto"
		Const sDatabaseName As String = "ProjectData"

		'		TPlServerDB.ServerDB.InitCurrentProject()
		TPlServerDB.ServerDB.InitCurrentServer()
		'		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName, True)
		TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName, True)



	End Sub
End Class