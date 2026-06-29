Option Explicit On
Option Strict On
Public Class frmTopoStatus
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.

	End Sub
	Public Sub AddTopo(tTopoRes As DMAcadExt.TopoRes, sThemeName As String, sTopoName As String, bArcs As Boolean)
		Me.dgvMain.Rows.Add()
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(Me.dgvMain.Rows.Count - 1)
		oGridRow.Cells.Item("ctxMapThemeName").Value = sThemeName
		oGridRow.Cells.Item("ctxTopoName").Value = sTopoName
		oGridRow.Cells.Item("cchArcs").Value = bArcs
		oGridRow.Cells.Item("ctxPolygonCount").Value = tTopoRes.PgonCount
		oGridRow.Cells.Item("cchIsCorrect").Value = tTopoRes.IsCorrect
		oGridRow.Cells.Item("cchIsComplete").Value = tTopoRes.IsComplete
		oGridRow.Cells.Item("ctxPolygonCount").Value = tTopoRes.PgonCount



	End Sub

	Private Sub cmdSaveDWG_Click(oSender As System.Object, e As EventArgs) Handles cmdSaveDWG.Click
		'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
		Me.DialogResult = DialogResult.No
		If DMAcadExt.AcadDocument.SaveCurrentDocument() Then
			Me.Close()
		End If
		'DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub cmdCancel_Click(oSender As System.Object, e As EventArgs) Handles cmdCancel.Click
		Me.DialogResult = DialogResult.No
		Me.Close()
	End Sub

	Private Sub cmdCloseSave_Click(oSender As System.Object, e As EventArgs) Handles cmdCloseSave.Click
		'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
		If DMAcadExt.AcadDocument.CloseAndSaveActiveDocument() Then
			Me.Close()
			Me.DialogResult = DialogResult.Yes
		End If

		'DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub frmTopoStatus_Load(oSender As System.Object, e As EventArgs) Handles MyBase.Load

	End Sub
End Class