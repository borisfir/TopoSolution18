Option Explicit On
Option Strict On
Imports System.Data
Public Class frmDesign
	Private mtMapThemeData As DMAcadExt.MapThemeData
	Private lblTopoExists As System.Windows.Forms.Label

	Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)

		mtMapThemeData = tMapThemeData

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
		Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
		Me.txtTopoName.Text = sLanduseTopoName
	End Sub
	Private Sub zzFill()
		Const sComText As String = "SELECT ID,Name FROM Scales"
		Me.cmbPaintScale.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Scales")
	End Sub
	Private Sub zzMyInitializeComponent()
		zzFill()
		Me.lblTopoExists = New System.Windows.Forms.Label()
		'
		'lblTopoExists
		'
		With Me.lblTopoExists
			.Image = Global.TopoUI.My.Resources.Resources.DoneTr
			.Location = New System.Drawing.Point(272, 12)
			.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
			.Name = "lblTopoExists"
			.Size = New System.Drawing.Size(30, 25)
			.TabIndex = 21
		End With

		Me.Controls.Add(Me.lblTopoExists)
	End Sub
	Private Function zzCreateLuseTopology() As Boolean
		Dim sAttribExpr As String = mtMapThemeData.DissolveAttribExpr
		Dim sLotTopoName As String = mtMapThemeData.TopoName
		Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName

		If TopoManager.TopoCreator.TopologyExists(sLanduseTopoName) Then
			Return True
		Else
			TopoManager.TopoCreator.DissolveTopo(sLotTopoName, sAttribExpr, sLanduseTopoName)
			If TopoManager.TopoCreator.TopologyExists(sLanduseTopoName) Then
				Return True
			Else
				Return False
			End If
		End If

	End Function
	Private Sub cmdPaintByLanduse_Click(ByVal oSender As System.Object, ByVal oEventArgs As System.EventArgs) Handles cmdPaintByLanduse.Click
		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim dBaseScale As Double = 1000.0
		Dim dScale As Double = zzGetSelectedScale()
		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = zzGetPaintTopoPurpose()
		Dim sPaintLayer As String
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		If iTopoPurpose <> DMAcadExt.enTopoPurpose.Undefined AndAlso dScale > 0.0 Then
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


			'	MessageBox.Show(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CMDDIA").ToString(), "26_201")
			DMAcadExt.AcadDocument.SaveVarCmdDia(0S)
			Dim sLotTopoName As String = mtMapThemeData.TopoName
			Dim sLanduseTopoName As String = mtMapThemeData.DissolveTopoName
			If zzCreateLuseTopology() Then

				TopoManager.TPlanGraph.TplnProject.LoadLusePgons(iTopoPurpose, sLotTopoName, sLanduseTopoName)
            sPaintLayer = TopoManager.TPlanGraph.TplnProject.SetPaintLayers(mtMapThemeData.MapThemeID, iTopoPurpose)
				If sPaintLayer IsNot Nothing Then
               MessageBox.Show(sPaintLayer & vbCrLf & CStr(dScale), "04_255")
					TopoManager.TPlanGraph.TplnLot.PaintByLanduseTopo(iTopoPurpose, dScale, sPaintLayer)
				End If
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
			Me.Cursor = Cursors.Default
		End If


	End Sub
	Private Function zzGetSelectedScale() As Double
		Dim sScale As String
		Dim dBaseScale As Double = 1000.0
		Dim dScale As Double = 0.0
		Dim oDyn As System.Object
		If Me.cmbPaintScale.SelectedIndex >= 0 Then
			oDyn = Me.cmbPaintScale.SelectedItem
			If oDyn IsNot Nothing Then
				sScale = Me.cmbPaintScale.GetItemText(oDyn)

				dScale = DMCommon.Functions.TextToScale(sScale, dBaseScale) / dBaseScale
			End If
		Else
			MessageBox.Show(Me.cmbPaintScale.SelectedIndex.ToString(), "16_711")
		End If
		Return dScale
	End Function


	Private Sub cmbPaintScale_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbPaintScale.SelectedIndexChanged

	End Sub

	Private Sub cmdEditColorSet_Click(sender As System.Object, e As System.EventArgs) Handles cmdEditColorSet.Click
		'	Dim fEditColorSet As frmEditColorSet = New frmEditColorSet(mtMapThemeData, 0)
		'	Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		'	Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, fEditColorSet)
	End Sub
End Class