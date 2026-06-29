Public Class Form2

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Const sDBResourceFile As String = "\\poseidon\Project\AppData\TopoSolution\tblData.mdb"
		'	Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
		'	Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
		Dim mbInitializedDB As Boolean
		If Not mbInitializedDB Then
			'mbInitializedDB = TPlServerDB.ServerDB.Initialize(iProvider, sDBResourceFile, sSysDBFile)
			TPlServerDB.ServerDB.InitCurrentServer()
			TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, "")
			mbInitializedDB = TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Closed
		End If
	End Sub

	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
		TPlServerDB.dmDBManager.TT()
	End Sub

	Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
		'	System.Windows.Forms.MessageBox.Show(iResourceTheme.ToString(), "01_101")
		Dim oRepApp As AcadReport.BaseReport = New AcadReport.Report
		oRepApp.Open(TPlServerDB.enResourceTheme.AcRepParcelLuseK)
	End Sub

	Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
		Dim daSourceArea() As Double = {3.245, 3.245, 3.125, 3.125}
		Dim oBalanceArea As TopoManager.BalanceArea = New TopoManager.BalanceArea(daSourceArea, 1000.0, 13.0, True)
		Dim laRes() As Long = oBalanceArea.Output
		Stop
	End Sub

	Private Sub Button5A_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

		Try

			Me.txtHeb.Text = DMCommon.Hebrew.ToDOS(Me.txtNum.Text)

			Me.txtHebB.Text = BamashNet.modFunctions.ToDOS(Me.txtNum.Text)
			Stop
		Catch ex As Exception

		End Try

	End Sub
	Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

		Try
			Dim iNum As Integer = Convert.ToInt32(Me.txtNum.Text)
			Me.txtHeb.Text = DMCommon.Hebrew.GetHebNumA(iNum)

			Me.txtHebB.Text = DMCommon.Hebrew.GetHebNum(iNum)

		Catch ex As Exception

		End Try

	End Sub
End Class