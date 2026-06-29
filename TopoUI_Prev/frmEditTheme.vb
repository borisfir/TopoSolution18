Option Explicit On
Option Strict On
Imports System.Data
Public Enum enGraphType
	Undefined
	Topology
	ClosedPolygons
End Enum
Public Class frmEditTheme
	Private Shared mbInitializedDB As Boolean = False
	Private moThemeItem As ThemeItem
	Private miProjectCode As Integer
	Private miDetailID As Integer = 0
	Private mdicThemeItems As Generic.Dictionary(Of Integer, ThemeItem)
	Private mbAddNew As Boolean
	Public Sub New(bAddNew As Boolean)
		' This call is required by the designer.
		InitializeComponent()
		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailID = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		mbAddNew = bAddNew
		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub
	Friend Property ThemeItems As Generic.Dictionary(Of Integer, ThemeItem)
		Get
			Return mdicThemeItems
		End Get
		Set(oValue As Generic.Dictionary(Of Integer, ThemeItem))
			mdicThemeItems = oValue
		End Set
	End Property

	Private Sub zzMyInitializeComponent()

		zzInitializeDB()
		Me.lblCover.Location = Me.tabThemeInfo.Location
		Me.lblCover.Size = New Size(Me.tabThemeInfo.Size.Width, Me.lblCover.Size.Height)

	End Sub
	Private Sub zzInitializeDB()
		'	Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
		Const sDBResourceFile As String = "\\Olympus\Project\AppData\TopoSolution\tblData.mdb"
		'  Const sDBResourceFile As String = "C:\Program Files\TownPlanner\tblData.mdb"
		'	Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
		'	Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
		'	Const sServerName As String = "neptune"
		'	Const sDatabaseName As String = "tblData2009"


		'mbInitializedDB = TPlServerDB.ServerDB.Initialize(iProvider, sDBResourceFile, sSysDBFile)
		If TPlServerDB.ServerDB.CurrentServerDB Is Nothing OrElse TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Open Then
			TPlServerDB.ServerDB.InitCurrentServer()
			TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, String.Empty)
		End If
		''''''''''''''TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName)
		'mbInitializedDB = TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Closed
		If TPlServerDB.ServerDB.CurrentProjectDB Is Nothing OrElse TPlServerDB.ServerDB.CurrentProjectDB.DBConnectionState <> ConnectionState.Open Then

			TPlServerDB.ServerDB.InitCurrentProject()
			MessageBox.Show("", "11_100")
			TPlServerDB.ServerDB.CurrentProjectDB.SetSQL("Pluto", "ProjectData", True)
		End If


	End Sub
	Public Sub FillTheme()
		If mbAddNew Then
			zzFillThemeByServer()
		Else
			zzFillThemeByProject()
		End If
	End Sub
	Private Sub zzFillThemeByServer()
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader("Select ID,UserName,DefaultGraphType From MapThemes")
		While oDataReader.Read
			Me.cmbTheme.Items.Add(New ThemeItem(oDataReader.GetInt32(0), oDataReader.GetString(1), CType(oDataReader.GetInt32(2), enGraphType)))
		End While
		oDataReader.Close()

	End Sub
	Private Sub zzFillThemeByProject()
		Dim iMapThemeID As DMAcadExt.enMapTheme
		Dim iGraphType As enGraphType
		Dim oThemeItem As ThemeItem

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader("SELECT  [MapThemeID],GraphType From PrjMapThemes WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(0) & ")")
		While oDataReader.Read
			iMapThemeID = DMAcadExt.MapThemeData.ToMapTheme(oDataReader.GetInt32(0))
			If [Enum].IsDefined(GetType(enGraphType), oDataReader.GetInt32(1)) Then
				iGraphType = CType(oDataReader.GetInt32(1), enGraphType)
			End If

			If mdicThemeItems.ContainsKey(iMapThemeID) Then
				oThemeItem = mdicThemeItems.Item(iMapThemeID)
				oThemeItem.GraphType = iGraphType
				Me.cmbTheme.Items.Add(oThemeItem)
			End If


		End While
		oDataReader.Close()

	End Sub
	Private Sub frmEditTheme_FormClosing(sender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		TPlServerDB.ServerDB.CurrentServerDB.Close()
	End Sub
	

	Private Sub cmbTheme_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbTheme.SelectedIndexChanged
		Dim oDynObject As System.Object = Me.cmbTheme.SelectedItem
		If oDynObject IsNot Nothing Then
			moThemeItem = DirectCast(oDynObject, ThemeItem)
			Me.GraphType = moThemeItem.GraphType
			zzSetThemeDefs()
		End If
	End Sub
	Private Sub zzSetThemeDefs()
		If Me.rdbTopology.Checked Then
			zzSetTopologyDefs()
		ElseIf Me.rdbClosedPolygons.Checked Then
			zzSetClosedPolygonsDefs()
		End If
	End Sub
	Private Sub zzSetTopologyDefs()
		If moThemeItem IsNot Nothing Then
			Dim iTopoDefID As Integer
			Dim oTopoDef As DMAcadExt.TopoDef

			Select Case moThemeItem.ListIndex
				Case 3
					iTopoDefID = 1
				Case 4
					iTopoDefID = 2
				Case 5
					iTopoDefID = 3
				Case Else
					iTopoDefID = 0
			End Select
			If iTopoDefID <> 0 Then
				oTopoDef = New DMAcadExt.TopoDef(DMAcadExt.enApplications.Taba, iTopoDefID)
				Me.txtLinkLayers.Text = oTopoDef.IncludeLayers
				Me.txtBlockName.Text = oTopoDef.CentroidBlock
				Me.txtBlockLayer.Text = oTopoDef.IncludeLayers
			End If
		End If
	End Sub
	Private Sub zzSetClosedPolygonsDefs()

	End Sub
	Private Property GraphType As enGraphType
		Get
			If Me.rdbTopology.Checked Then
				Return enGraphType.Topology
			ElseIf Me.rdbClosedPolygons.Checked Then
				Return enGraphType.ClosedPolygons
			Else
				Return enGraphType.Undefined
			End If
		End Get
		Set(iValue As enGraphType)
			Select Case iValue
				Case enGraphType.Topology
					Me.rdbTopology.Checked = True
				Case enGraphType.ClosedPolygons
					Me.rdbClosedPolygons.Checked = True
			End Select

		End Set
	End Property
	Private Sub rdbTopology_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbTopology.CheckedChanged
		If rdbTopology.Checked Then
			zzSetTopologyDefs()
		End If

	End Sub

	Private Sub cmdOK_Click(sender As System.Object, e As System.EventArgs) Handles cmdOK.Click
		Dim sComText As String = zzGetInsertSQL()
		Dim oCommandErr As System.Data.Common.DbException = Nothing
		If sComText IsNot Nothing Then
			If TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sComText, CommandType.Text, , oCommandErr) = 1 Then
				Me.DialogResult = Windows.Forms.DialogResult.OK
				Me.Close()
			Else
				'number=156
				Select Case oCommandErr.ErrorCode
					Case &H80131904
						System.Windows.Forms.MessageBox.Show("Record Exists", "dmDBManager - RunCommand")
				End Select

			End If
		End If

	End Sub
	Private Function zzGetInsertSQL() As String
		Dim oSQLBuilder As TPlServerDB.SQLBuilder = New TPlServerDB.SQLBuilder("PrjMapThemes")

		oSQLBuilder.AddField("ProjectCode", miProjectCode)
		oSQLBuilder.AddField("Detail", miDetailID)
		oSQLBuilder.AddField("MapThemeID", moThemeItem.ListIndex)  'MapLayerID
		oSQLBuilder.AddField("GraphType", Me.GraphType)
		oSQLBuilder.AddField("GroupID", 1)
		oSQLBuilder.AddField("LinkLayers", Me.txtLinkLayers.Text)
		oSQLBuilder.AddField("CentroidBlocks", Me.txtBlockName.Text)
		oSQLBuilder.AddField("CentroidLayers", Me.txtBlockLayer.Text)

		Return oSQLBuilder.GetAppendValueComText()
	End Function


	Private Function zzGetUpdateSQL() As String
		Dim oSQLBuilder As TPlServerDB.SQLBuilder = New TPlServerDB.SQLBuilder("PrjMapThemes")

		oSQLBuilder.AddField("ProjectCode", miProjectCode)
		oSQLBuilder.AddField("Detail", TPlServerDB.dmDataType.Int, miDetailID)
		oSQLBuilder.AddField("MapThemeID", TPlServerDB.dmDataType.Int, moThemeItem.ListIndex)	'MapLayerID
		oSQLBuilder.AddField("GraphType", TPlServerDB.dmDataType.Int, Me.GraphType)
		oSQLBuilder.AddField("LinkLayers", TPlServerDB.dmDataType.Text, Me.txtLinkLayers.Text)
		oSQLBuilder.AddField("CentroidBlocks", TPlServerDB.dmDataType.Text, Me.txtBlockName.Text)
		oSQLBuilder.AddField("CentroidLayers", Me.txtBlockLayer.Text)

		Return oSQLBuilder.GetAppendValueComText()
	End Function



	Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
		Me.DialogResult = Windows.Forms.DialogResult.Cancel
		Me.Close()
	End Sub
End Class
