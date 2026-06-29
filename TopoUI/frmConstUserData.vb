Option Explicit On
Option Strict On
Imports System.Data
Public Class frmConstUserData
	Private moConstUserDataAdapter As Common.DbDataAdapter
	Private moConstUserDataTable As DataTable
	Private miProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
	Private miDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
	Private Sub zzSetDataBinding()
		Me.txtLT_Name.DataBindings.Clear()
		Me.txtLT_Name.DataBindings.Add("Text", moConstUserDataTable, "lt_Name")
	End Sub
	Private Sub zzLoadData()
		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		If miProjectCode <> 0 Then
			Dim sComText As String = "SELECT * FROM prjUserConstData WHERE (ProjectCode = " & Convert.ToString(miProjectCode) & ") AND (Detail = " & Convert.ToString(miDetailNo) & ")"
         moConstUserDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, CommandType.Text, True, , True, True)
			moConstUserDataTable = New DataTable()
			moConstUserDataAdapter.Fill(moConstUserDataTable)
			Me.bnsUserData.DataSource = moConstUserDataTable
			If moConstUserDataTable.Rows.Count = 0 Then
				bnsUserData.AddNew()
			Else
				''''''''''''''''''moConstUserDataTable.Rows.Item(0).BeginEdit()
			End If
		End If
		
	End Sub
	Private Sub zzUpdateData()
		bnsUserData.EndEdit()
		Dim oRow As DataRow = moConstUserDataTable.Rows.Item(0)
		'oRow.EndEdit()
		'	MessageBox.Show(oRow.Item(0).ToString & vbCrLf & oRow.Item(1).ToString & vbCrLf & oRow.Item(2).ToString & vbCrLf & oRow.RowState.ToString(), "02_350")
		If oRow.RowState = DataRowState.Added Then
			oRow.Item(0) = miProjectCode
			oRow.Item(1) = miDetailNo

		End If
		'oRow.SetModified()
		moConstUserDataAdapter.Update(moConstUserDataTable)
	End Sub

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()
		zzLoadData()
		zzSetDataBinding()
		' Add any initialization after the InitializeComponent() call.

	End Sub

	Private Sub cmdOK_Click(sender As System.Object, e As System.EventArgs) Handles cmdOK.Click
		zzUpdateData()
		Me.Close()
	End Sub

	Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
		Me.Close()
	End Sub

	
End Class