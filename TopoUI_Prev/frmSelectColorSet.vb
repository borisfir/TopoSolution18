Option Explicit On
Option Strict On
Imports System.Data
Public Class frmSelectColorSet
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private miMapThemeID As DMAcadExt.enMapTheme
	Public ReadOnly Property ColorSetType As enColorSetType
		Get
			If Me.rdbStandard.Checked Then
				Return enColorSetType.Standard
			ElseIf Me.rdbAllLanduses.Checked Then
				Return enColorSetType.All
			Else
				Return enColorSetType.ByProject
			End If
		End Get
	End Property
	Public ReadOnly Property ProjectCode As Integer
		Get
			If Me.ColorSetType = enColorSetType.ByProject Then
				Return miProjectCode
			Else
				Return 0
			End If

		End Get
	End Property
	Public ReadOnly Property DetailNo As Integer
		Get
			If Me.ColorSetType = enColorSetType.ByProject Then
				Return miDetailNo
			Else
				Return 0
			End If

		End Get
	End Property


	Public ReadOnly Property MapThemeID As Integer
		Get
			If Me.ColorSetType = enColorSetType.ByProject Then
				Return miMapThemeID
			Else
				Return 0
			End If

		End Get
	End Property

	Private Sub rdbProject_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbProject.CheckedChanged
		Me.txtProjectCode.Enabled = Me.rdbProject.Checked
		Me.cmbDetail.Enabled = Me.rdbProject.Checked
		Me.cmbTheme.Enabled = Me.rdbProject.Checked

	End Sub
	Private Sub zzFillProject()

		Dim sComText As String = "SELECT DISTINCT ProjectCode FROM dbo.ColorSchemeSets"
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				Me.txtProjectCode.AutoCompleteCustomSource.Add(Convert.ToString(oDataReader.GetInt32(0)))
			End While
			oDataReader.Close()
		End If
	End Sub

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.

	End Sub

	Private Sub frmSelectColorSet_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
		zzFillProject()
	End Sub

	Private Sub txtProjectCodeAfterUpdate()
		Dim sProjectCode As String = Me.txtProjectCode.Text
		sProjectCode.Trim()
		Dim sComText As String = "SELECT DISTINCT Detail, ISNULL(DetailName, N' - ') AS DetailName FROM dbo.PrjColorSchemeSets WHERE (ProjectCode = " & sProjectCode & ")"
		Me.cmbDetail.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
		'	Me.cmbDetail.SelectedValue = 0
	End Sub
	Private Sub zzFillMapTheme()
		Dim sProjectCode As String = Me.txtProjectCode.Text
		Dim sDetail As String = CStr(Me.cmbDetail.SelectedValue)
		If IsNumeric(sDetail) Then
			sProjectCode.Trim()
			Dim sComText As String = "SELECT DISTINCT dbo.ColorSchemeSets.MapThemeID, dbo.MapThemes.MapThemeName FROM dbo.ColorSchemeSets INNER JOIN dbo.MapThemes ON dbo.ColorSchemeSets.MapThemeID = dbo.MapThemes.MapThemeID WHERE (dbo.ColorSchemeSets.ProjectCode = " & sProjectCode & ") AND (dbo.ColorSchemeSets.Detail = " & sDetail & ")"
			Me.cmbTheme.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
		End If
	End Sub

	Private Sub txtProjectCode_LostFocus(oSender As System.Object, e As System.EventArgs) Handles txtProjectCode.LostFocus
		txtProjectCodeAfterUpdate()
		zzFillMapTheme()
	End Sub



	Private Sub cmbDetail_LostFocus(ByVal oSender As System.Object, e As System.EventArgs) Handles cmbDetail.LostFocus
		zzFillMapTheme()
	End Sub





	Private Sub cmdOK_Click(sender As System.Object, e As System.EventArgs) Handles cmdOK.Click

	End Sub
End Class