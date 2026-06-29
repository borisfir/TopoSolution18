Public Class Form1
	Private Shared msServerDataSource As String

	Public Sub New()
		Const msServerDataBase As String = "ProjectData" ' "ProjectDataTest"
		Const msProjectDatabase As String = "UD_Projects" ' "ProjectDataTest"
		Const sBaseServerName As String = "PLUTO"
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		TPlServerDB.ServerDB.InitCurrentProject()
		msServerDataSource = sBaseServerName
		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, msProjectDatabase, True)
	End Sub
End Class