Option Explicit On
Option Strict On
Public Class frmDWGBatch

	Private Sub cmdAddFiles_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdAddFiles.Click
		Me.ofdDWG.FileName = "*.DWG"
		Me.ofdDWG.ShowDialog()
		If Me.ofdDWG.FileName <> "" Then
			For iIndex As Integer = 0 To Me.ofdDWG.FileNames.GetUpperBound(0)
				Me.lvwDWGs.Items.Add(Me.ofdDWG.FileNames(iIndex))
			Next
		End If
	End Sub

	Private Sub cmdAddDirectory_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdAddDirectory.Click
		Me.fbdDWGs.ShowDialog()
		If fbdDWGs.SelectedPath.Length <> 0 Then
			Stop
		End If
	End Sub
	Private Sub zzInitDB()
		' 	Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
		Const sDBResourceFile As String = "\\Poseidon\Project\AppData\TopoSolution\tblData.mdb"
		Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
		Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
		Const sServerName As String = "neptune"
		Const sDatabaseName As String = "Ashqelon"
		Const sRootPath As String = "P:\2010\100284\Plan\"
		components = New System.ComponentModel.Container
		Dim oListViewItem As ListViewItem
		Dim oListViewSubItem As ListViewItem.ListViewSubItem
		Dim sDWGName As String
		TPlServerDB.ServerDB.InitCurrentServer()
		TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, "")

		TPlServerDB.ServerDB.InitCurrentProject()
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName)
		Dim oReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader("SELECT ObjectID,PlanNum,DWGName FROM PlanMain")
		If oReader IsNot Nothing Then
			While oReader.Read
				If Not oReader.IsDBNull(2) Then
					sDWGName = sRootPath & oReader.GetString(2)
					oListViewItem = Me.lvwDWGs.Items.Add(sDWGName)
					If Microsoft.VisualBasic.FileIO.FileSystem.FileExists(sDWGName) Then
						oListViewSubItem = oListViewItem.SubItems.Add("Loading ...")
						DMAcadExt.AcadDocument.OpenDocument(sDWGName, True)
						oListViewSubItem.Text = "Success"

						oListViewSubItem = oListViewItem.SubItems.Add("Creating ...")
						'TopoManager.topocreato()
						'		MessageBox.Show("", "")
						oListViewSubItem.Text = "Success"

						oListViewSubItem = oListViewItem.SubItems.Add("Exporting ...")
						oListViewSubItem.Text = "Success"

						oListViewSubItem = oListViewItem.SubItems.Add("Closing ...")
						DMAcadExt.AcadDocument.CloseAndDiscardActiveDocument()
						oListViewSubItem.Text = "Success"
						Me.Focus()
					End If
				End If
			End While
		End If
		oReader.Close()
		TPlServerDB.ServerDB.CurrentServerDB.Close()
		TPlServerDB.ServerDB.CurrentProjectDB.Close()

	End Sub

	Private Sub cmdExec1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdExec1.Click
		zzInitDB()
	End Sub
	Sub zz()

	End Sub
End Class
