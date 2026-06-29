Public Class Form6
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
	End Sub


End Class