' SmallImageList must be set when using IndentCount.
Option Explicit On
Option Strict On
Imports System.Data
Public Class frmThemeExplorer
	Private miProjectCode As Integer
	Private miDetailID As Integer = 0

	Private mdicThemeItems As Generic.Dictionary(Of Integer, ThemeItem)
	Private mbInitializedDB As Boolean
	Private lvgSource As System.Windows.Forms.ListViewGroup
	Private lvgOverlay As System.Windows.Forms.ListViewGroup



	Private Sub txtProjecyCode_Leave(oSender As System.Object, e As System.EventArgs) Handles txtProjectCode.Leave
		Dim iProjectCode As Integer
		Dim oDynObj As System.Object
		If Integer.TryParse(txtProjectCode.Text, iProjectCode) Then
			If miProjectCode <> iProjectCode Then
				MessageBox.Show("", "11_300")
				TPlServerDB.ServerDB.CurrentProjectDB.SetSQL("Pluto", "ProjectData", True)
				If TPlServerDB.ServerDB.CurrentProjectDB.DBConnectionState = ConnectionState.Open Then
					oDynObj = TPlServerDB.ServerDB.CurrentProjectDB.GetDataScalar("SELECT PrjName FROM [ProjectList] WHERE PrjCode=" & Convert.ToString(iProjectCode), CommandType.Text)
					If oDynObj IsNot Nothing Then
						Me.txtProjectName.Text = DirectCast(oDynObj, String)
						miProjectCode = iProjectCode ''''''''''''''''''''110337
						TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode = miProjectCode

						zzFillThemeViewList()
					End If
				End If

			End If
		End If

	End Sub
	Private Sub zzFillTheme()
		mdicThemeItems = New Generic.Dictionary(Of Integer, ThemeItem)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader("Select ID,UserName,DefaultGraphType From MapThemes")
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				mdicThemeItems.Add(oDataReader.GetInt32(0), New ThemeItem(oDataReader.GetInt32(0), oDataReader.GetString(1), CType(oDataReader.GetInt32(2), enGraphType)))
			End While
			oDataReader.Close()
		End If
	End Sub

	Private Sub zzFillThemeViewList()
		Dim iThemeID As Integer
		Dim iGraphType As Integer
		Dim iGroupID As Integer

		Dim oThemeItem As ThemeItem
		Dim oThemeListItem As prjThemeListItem

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader("SELECT [MapThemeID],GraphType,GroupID From PrjMapThemes WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(0) & ")")
		Me.lstThemes.Items.Clear()
		Dim iItem As Integer
		While oDataReader.Read
			iThemeID = oDataReader.GetInt32(0)
			iGraphType = oDataReader.GetInt32(1)
			iGroupID = oDataReader.GetInt32(2)

			If mdicThemeItems.ContainsKey(iThemeID) Then
				oThemeItem = mdicThemeItems.Item(iThemeID)
				oThemeListItem = New prjThemeListItem(iThemeID, iGraphType)
				oThemeListItem.Text = oThemeItem.ThemeUserName
				oThemeListItem.Group = zzGetListViewGroup(iGroupID)
				oThemeListItem.ImageIndex = 0
				oThemeListItem.IndentCount = iItem
				iItem += 1
				'	oThemeListItem.ImageKey = "Monitor"
				Me.lstThemes.Items.Add(oThemeListItem)

			End If


		End While
		oDataReader.Close()
	End Sub


	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.

		imlSmall.Images.Add(New Bitmap(GetType(Button), "Button.bmp"))

		Const sDBResourceFile As String = "\\Olympus\Project\AppData\TopoSolution\tblData.mdb"
		TPlServerDB.ServerDB.InitCurrentServer()
		TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, String.Empty)
		''''''''''''''TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName)
		mbInitializedDB = TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Closed

		TPlServerDB.ServerDB.InitCurrentProject()
		zzMyInitializeComponent()
		zzFillTheme()
	End Sub
	Private Sub zzMyInitializeComponent()
		lvgSource = New System.Windows.Forms.ListViewGroup("נושאים מקוריים", System.Windows.Forms.HorizontalAlignment.Left)
		lvgOverlay = New System.Windows.Forms.ListViewGroup("חי", System.Windows.Forms.HorizontalAlignment.Left)

		lvgSource.Header = "נושאים מקוריים"
		lvgSource.Name = "grpSource"
		lvgOverlay.Header = "חי"
		lvgOverlay.Name = "grpOverlay"
		Me.lstThemes.Groups.AddRange(New System.Windows.Forms.ListViewGroup() {lvgSource, lvgOverlay})

		zzFillTheme()

	End Sub
	Private Function zzGetListViewGroup(iGroupID As Integer) As ListViewGroup
		Select Case iGroupID
			Case 1
				Return lvgSource
			Case 2
				Return lvgOverlay
			Case Else
				Return Nothing
		End Select
	End Function

	Private Sub cmdAddTheme_Click(oSender As System.Object, e As System.EventArgs) Handles cmdAddTheme.Click
		If miProjectCode <> 0 Then
			Dim fAdd As frmEditTheme = New frmEditTheme(True)
			fAdd.FillTheme()
			fAdd.ShowDialog()

			If fAdd.DialogResult = Windows.Forms.DialogResult.OK Then
				zzFillThemeViewList()
			End If
		End If

		'If
	End Sub

	Private Sub cmdDelete_Click(oSender As System.Object, e As System.EventArgs) Handles cmdDelete.Click
		If miProjectCode <> 0 Then
			Dim oDin As System.Object
			Dim oThemeListItem As prjThemeListItem
			If Me.lstThemes.SelectedItems.Count <> 0 Then
				oDin = Me.lstThemes.SelectedItems.Item(0)
				If oDin IsNot Nothing Then
					Try
						oThemeListItem = DirectCast(oDin, prjThemeListItem)
						zzDelete(oThemeListItem.ThemeID, oThemeListItem.GraphType)
					Catch oEx As Exception

					End Try
				End If
				Me.lstThemes.Focus()
			End If

		End If

	End Sub
	Private Sub zzDelete(iThemeID As Integer, iGraphType As Integer)
		Dim oSQLBuilder As TPlServerDB.SQLBuilder = New TPlServerDB.SQLBuilder("PrjMapThemes")
		Dim sComText As String
		Dim oCommandErr As System.Data.Common.DbException = Nothing

		oSQLBuilder.AddField("ProjectCode", miProjectCode)
		oSQLBuilder.AddField("Detail", miDetailID)
		oSQLBuilder.AddField("MapThemeID", iThemeID)		'MapLayerID
		oSQLBuilder.AddField("GraphType", TPlServerDB.dmDataType.Int, iGraphType)

		sComText = oSQLBuilder.GetDeleteComText
		If TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sComText, CommandType.Text, , oCommandErr) = 1 Then
			zzFillThemeViewList()
		Else
			Select Case oCommandErr.ErrorCode
				Case &H80131904
					System.Windows.Forms.MessageBox.Show("Record Exists", "dmDBManager - RunCommand")
				Case Else
					System.Windows.Forms.MessageBox.Show(oCommandErr.Message, "frmThemeExplorer - zzDelete")
			End Select

		End If

	End Sub

	Private Sub cmdEditTheme_Click(oSender As System.Object, e As System.EventArgs) Handles cmdEditTheme.Click
		If miProjectCode <> 0 Then
			Dim fEdit As frmEditTheme = New frmEditTheme(False)
			fEdit.ThemeItems = mdicThemeItems
			fEdit.FillTheme()
			fEdit.ShowDialog()
			If fEdit.DialogResult = Windows.Forms.DialogResult.OK Then
				zzFillThemeViewList()
			End If
		End If
	End Sub

	
End Class
Friend Class ThemeItem
	Inherits DMCommon.ItemData
	Private miGraphType As enGraphType
	Public Sub New(iThemeID As Integer, sThemeUserName As String, iGraphType As enGraphType)
		MyBase.New(iThemeID, sThemeUserName)
		miGraphType = iGraphType
	End Sub
	Public Property GraphType As enGraphType
		Get
			Return miGraphType
		End Get
		Set(iValue As enGraphType)
			miGraphType = iValue
		End Set
	End Property
	Public ReadOnly Property ThemeID As Integer
		Get
			Return MyBase.ListIndex
		End Get
	End Property
	Public ReadOnly Property ThemeUserName As String
		Get
			Return MyBase.ListDispData
		End Get
	End Property
End Class
Friend Class prjThemeListItem
	Inherits ListViewItem
	Friend Property ThemeID As Integer
	Friend Property GraphType As Integer
	Public Sub New()

	End Sub
	Public Sub New(iThemeID As Integer, iGraphType As Integer)
		ThemeID = iThemeID
		GraphType = iGraphType
	End Sub
End Class