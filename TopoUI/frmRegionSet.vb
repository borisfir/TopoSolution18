Option Strict On
Option Explicit On

Imports TopoManager
Public Class frmRegionSet
	Private mbUpdated As Boolean
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()

		zzFillListRegionSet()
		zzFillRegions()

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public ReadOnly Property Updated As Boolean
		Get
			Return mbUpdated
		End Get
	End Property
	Private Sub zzFillListRegionSet()
		Const sNewItem As String = "(הוסף חדש)"

		Dim oaRegionSet() As TopoManager.TPlanGraph.TplnRegionSet = TopoManager.TPlanGraph.TplnRegionSet.GetArray()
		TopoManager.TPlanGraph.TplnRegionSet.HasPrefix = False
		If oaRegionSet IsNot Nothing AndAlso oaRegionSet.GetUpperBound(0) >= 0 Then
			For iIndex As Integer = 0 To oaRegionSet.GetUpperBound(0)
				If oaRegionSet(iIndex) IsNot Nothing Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!FillListRegionSet", iIndex, oaRegionSet(iIndex).Name, oaRegionSet(iIndex).Index)
					Me.lstRegionSets.Items.Add(oaRegionSet(iIndex))
				End If
			Next

		End If
		Dim oItem As TopoManager.TPlanGraph.TplnRegionSet = New TopoManager.TPlanGraph.TplnRegionSet(0, sNewItem, String.Empty)
		Me.lstRegionSets.Items.Add(oItem)
	End Sub
	Private Sub zzFillRegions()
		For Each oRegion As TPlanGraph.TplnRegion In TopoManager.TPlanGraph.TplnProject.Regions.Values
			Me.clbRegions.Items.Add(oRegion)
		Next

	End Sub

	Private Sub cmdSave_Click(oSender As System.Object, e As EventArgs) Handles cmdSave.Click
		Dim oRegionSet As TopoManager.TPlanGraph.TplnRegionSet
		Dim sRegionList As String = Nothing
		Dim iListCount As Integer
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		TopoManager.TPlanGraph.TplnRegionSet.RefreshDBDictionary()
		Dim iCheckedCount As Integer = zzGetCheckedList(sRegionList)
		oRegionSet = DirectCast(Me.lstRegionSets.SelectedItem, TPlanGraph.TplnRegionSet)
		If oRegionSet.No = 0 Then
			If Not String.IsNullOrEmpty(Me.txtRecordSetName.Text) Then

				If Not String.IsNullOrEmpty(sRegionList) AndAlso (iCheckedCount > 1) Then
					mbUpdated = True
					oRegionSet = TopoManager.TPlanGraph.TplnRegionSet.Add(Me.txtRecordSetName.Text, sRegionList)
					iListCount = Me.lstRegionSets.Items.Count
					Me.lstRegionSets.Items.Insert(iListCount - 1, oRegionSet)
					Me.txtRecordSetName.Text = Nothing
					zzSetCheckedList(String.Empty)
					'Me.DialogResult = DialogResult.OK
				End If
			End If
		Else
			Dim iIndex As Integer = oRegionSet.Index
			Dim sName As String = Me.txtRecordSetName.Text
			If Not String.IsNullOrEmpty(sName) Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Update", oRegionSet.No, oRegionSet.Index, iIndex, sName, sRegionList)
				Me.lstRegionSets.Items.Remove(oRegionSet)

				oRegionSet = TopoManager.TPlanGraph.TplnRegionSet.Replace(oRegionSet.No, sName, sRegionList)
				Me.lstRegionSets.Items.Insert(iIndex, oRegionSet)

				mbUpdated = True
				'Me.DialogResult = DialogResult.Yes
			End If
		End If


		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Function zzGetCheckedListOLD(ByRef sList As String) As Integer
		Dim oItem As TPlanGraph.TplnRegion
		'Dim sRes As String = Nothing
		Dim iRes As Integer = 0
		DMCommon.Debug.MsgBox("!SelectedItems.Count", Me.clbRegions.SelectedItems.Count)
		For Each oObject As System.Object In Me.clbRegions.SelectedItems
			oItem = DirectCast(oObject, TPlanGraph.TplnRegion)
			iRes += 1
			If String.IsNullOrEmpty(sList) Then
				sList = oItem.RegionNo.ToString()
			Else
				sList = "," & oItem.RegionNo.ToString()
			End If
		Next
		Return iRes
	End Function


	Private Sub cmdDelete_Click(oSender As System.Object, e As EventArgs) Handles cmdDelete.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		TopoManager.TPlanGraph.TplnRegionSet.RefreshDBDictionary()
		Dim oRegionSet As TopoManager.TPlanGraph.TplnRegionSet
		oRegionSet = DirectCast(Me.lstRegionSets.SelectedItem, TPlanGraph.TplnRegionSet)
		If oRegionSet.No <> 0 Then
			TopoManager.TPlanGraph.TplnRegionSet.Delete(oRegionSet.No)
			Me.lstRegionSets.Items.Remove(oRegionSet)
			mbUpdated = True
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub lstRegionSets_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles lstRegionSets.SelectedIndexChanged
		Dim oRegionSet As TopoManager.TPlanGraph.TplnRegionSet
		If Me.lstRegionSets.SelectedItem Is Nothing Then
			Me.txtRecordSetName.Text = Nothing
			zzSetCheckedList(String.Empty)

		Else
			oRegionSet = DirectCast(Me.lstRegionSets.SelectedItem, TPlanGraph.TplnRegionSet)
			If oRegionSet.No = 0 Then
				Me.txtRecordSetName.Text = Nothing
				zzSetCheckedList(String.Empty)
			Else
				Me.txtRecordSetName.Text = oRegionSet.Name
				zzSetCheckedList(oRegionSet.RegionList)
			End If
		End If

	End Sub
	Private Sub zzSetCheckedList(sRegionList As String)
		sRegionList = "," & sRegionList & ","
		Dim tList As DMCommon.dmList = New DMCommon.dmList(sRegionList, ",")
		Dim oObject As System.Object
		Dim oRegion As TPlanGraph.TplnRegion
		Dim bChecked As Boolean
		For iIndex As Integer = 0 To Me.clbRegions.Items.Count - 1
			oObject = Me.clbRegions.Items.Item(iIndex)
			oRegion = DirectCast(oObject, TPlanGraph.TplnRegion)
			bChecked = tList.Contains(oRegion.RegionNo.ToString())
			Me.clbRegions.SetItemChecked(iIndex, bChecked)
		Next

		'SetItemChecked
	End Sub
	Private Function zzGetCheckedList(ByRef sRegionList As String) As Integer
		Dim oObject As System.Object
		Dim oRegion As TPlanGraph.TplnRegion
		Dim bChecked As Boolean
		Dim iCheckedCount As Integer = 0
		For iIndex As Integer = 0 To Me.clbRegions.Items.Count - 1
			bChecked = Me.clbRegions.GetItemChecked(iIndex)
			If bChecked Then
				iCheckedCount += 1
				oObject = Me.clbRegions.Items.Item(iIndex)
				oRegion = DirectCast(oObject, TPlanGraph.TplnRegion)
				If String.IsNullOrEmpty(sRegionList) Then
					sRegionList = oRegion.RegionNo.ToString()
				Else
					sRegionList &= "," & oRegion.RegionNo.ToString()
				End If
			End If
		Next
		Return iCheckedCount

		'SetItemChecked
	End Function

	Private Sub frmRegionSet_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		Me.lstRegionSets.Focus()
	End Sub

	Private Sub frmRegionSet_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		Me.lstRegionSets.Focus()
	End Sub

	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

End Class