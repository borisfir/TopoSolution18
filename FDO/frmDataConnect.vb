Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.DataConnect.UI
Imports OverlayMgd
Imports OSGeo.FDO
Imports OSGeo.FDO.ClientServices
Imports OSGeo.FDO.Connections
Public Class frmDataConnect
	Dim msParcelShapePath As String = "D:\aWork\ArcGIS\D\Parcel\topoParcels.shp"
	Dim msLotShapePath As String = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
	Dim sUnionPath As String = "D:\aWork\ArcGIS\E_Acad\topoParcels_UnionK.shp" ''''"D:\aWork\ArcGIS\E_Acad\topoLots_UnionC.sdf"	 'topoLots_UnionDa.sdf
	'topoParcels_UnionK.shp
	Public Shared msSHPProviderName As String = "OSGeo.SHP.4.2" ' "OSGeo.SHP.3.6"
	Public Const SHPProviderName As String = "OSGeo.SHP.4.2" '"OSGeo.SHP.3.6"
	Dim sSDFProviderName As String = "OSGeo.SDF.3.6"
	Dim msSHPPropFileName As String = "DefaultFileLocation"
	Dim sSDFPropFileName As String = "File"
	Private maControlExt() As System.Windows.Forms.Control
	Private mtaExtColor() As System.Drawing.Color
	Private moEditor As Autodesk.AutoCAD.EditorInput.Editor
	Private WithEvents moDataConnectUI As dmDataConnectUI
	Private WithEvents moSHPConnectionControl As dmSHPConnectionControl

	Private WithEvents moConnectionConfigureSourceControl As dmConnectionConfigureSourceControl
	Private WithEvents moGenericConfigureLayerControl As dmGenericConfigureLayerControl
	Private WithEvents moGenericBrowseSchemaControl As dmGenericBrowseSchemaControl
	'Private WithEvents moGenericBrowseSchemaControlSrc As GenericBrowseSchemaControl




	Private moConnection As IConnectionImp
	Private moSHPNode As System.Windows.Forms.TreeNode
	Private moConnectionNode As System.Windows.Forms.TreeNode
	Private moFeatureNode As System.Windows.Forms.TreeNode

	Private moSHPConnectionPlugin As SHPConnectionPlugin
	Private WithEvents moTreeView As System.Windows.Forms.TreeView
	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles btn1.Click
		'moDataConnectUI.A1(moSHPNode)
		'''''''''''	moDataConnectUI.Init()
		Dim oPanel As System.Windows.Forms.Panel
		Dim oControlCollection As System.Windows.Forms.Control.ControlCollection
		moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)
		'	Return
		moConnectionConfigureSourceControl.Init(moDataConnectUI, "topoLots")
		moConnectionConfigureSourceControl.SetParent(moDataConnectUI)
		oPanel = moConnectionConfigureSourceControl.panelPluginControl
		oPanel.BackColor = Drawing.Color.Aquamarine
		oControlCollection = oPanel.Controls
		System.Windows.Forms.MessageBox.Show(CStr(oControlCollection.Count), "-384")

		moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)
		moDataConnectUI.AddFeatureSourceNode("topoPLots")

	End Sub
	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles btn2.Click
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim oControl As System.Windows.Forms.Control
		Dim sMsg As String = "!"
		Dim sFeatureSource As String = msLotShapePath
		Dim sLayerSource As String = "topoLots"
		Dim iConnectionState As OSGeo.FDO.Connections.ConnectionState
		Dim oNode As System.Windows.Forms.TreeNode
		Dim oSHPConnectionPlugin As SHPConnectionPlugin
		'	oDataConnectPlugin = moSHPConnectionControl.SetPluginA()
		'	If oDataConnectPlugin IsNot Nothing Then
		'		sMsg = oDataConnectPlugin.ToString()
		'	End If
		System.Windows.Forms.MessageBox.Show(sMsg, "310")
		'moDataConnectUI.SetPlugin(oDataConnectPlugin)
		moDataConnectUI.SelectFeatureSource(msSHPProviderName)
		moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)
		oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		moDataConnectUI.OnProviderNodeSelected(oNode)
		System.Windows.Forms.MessageBox.Show(msLotShapePath, "320")
		oDataConnectPlugin = moDataConnectUI.GetPluginConnection(msSHPProviderName)
		System.Windows.Forms.MessageBox.Show(CStr(oDataConnectPlugin Is Nothing), "324")
		oSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
		oSHPConnectionPlugin.SourceFileName = msLotShapePath
		iConnectionState = oDataConnectPlugin.ConnectWithName("SHP_X")
		System.Windows.Forms.MessageBox.Show(oDataConnectPlugin.ToString() & ":" & oDataConnectPlugin.FeatureSourceName & ":" & oDataConnectPlugin.LayerSourceName, "324AA")
		System.Windows.Forms.MessageBox.Show(iConnectionState.ToString, "325")
		'Return
		'''''''''''''''''	oDataConnectPlugin.Attach(moDataConnectUI, msSHPProviderName, sFeatureSource, sLayerSource)
		System.Windows.Forms.MessageBox.Show("", "330")
		''''''''''''	oControl = oDataConnectPlugin.PluginControl
		'	System.Windows.Forms.MessageBox.Show(oControl.ToString(), "321x")
		moSHPConnectionControl.SetPluginB(oDataConnectPlugin)
		System.Windows.Forms.MessageBox.Show("", "340")
		moSHPConnectionControl.Test()
		If False Then

			moSHPConnectionControl.Attach(moDataConnectUI)

			oDataConnectPlugin = moDataConnectUI.GetPluginConfigureSource()
			oControl = oDataConnectPlugin.PluginControl
			'	System.Windows.Forms.MessageBox.Show(oControl.ToString(), "322x")
			moGenericConfigureLayerControl.SetPluginB(oDataConnectPlugin)


			'	modmGenericConfigureLayerControl.Test()
			'	modmGenericConfigureLayerControl.Attach(moDataConnectUI)
			Return

			moDataConnectUI.LayerSourceAdded("topoLots")
			moDataConnectUI.PostAddLayerSourceNode("topoLots")
			Return
			zzResetSHPProvider()
			Return
			moDataConnectUI.RefreshSHRProvider(moSHPNode)
			Return
			SetConnection()
			Return

		End If
	
	



	End Sub
	Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles btn3.Click
		'''''''''''''	moGenericConfigureLayerControl.Test()
		Dim oNode As System.Windows.Forms.TreeNode = Nothing
		If True Then
			If False Then
				oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
				moDataConnectUI.RefreshProvider(oNode)
				moDataConnectUI.Refresh()
				moDataConnectUI.RefreshFeatureSource("SHP_X")
				moDataConnectUI.PostRefreshFeatureSource("SHP_X")

				moDataConnectUI.SelectFeatureSource("SHP_X")
				moDataConnectUI.LayerSourceAdded("topoLots")
				moDataConnectUI.PostAddLayerSourceNode("topoLots")
			End If
			
			''''''''''''''''		oNode = moDataConnectUI.GetFeatureSourceNode("SHP_X")
			If oNode Is Nothing Then
				System.Windows.Forms.MessageBox.Show("oNode Is Nothing", "-248BBBB")
			Else
				System.Windows.Forms.MessageBox.Show(oNode.Text, "-248aAAAA")
			End If
			moDataConnectUI.SelectFeatureSource("SHP_X")
			''''''''''	moDataConnectUI.OnFeatureSourceNodeSelected(oNode)

			'	moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)
		End If
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin


		oDataConnectPlugin = moDataConnectUI.GetPluginBrowseSchema()

		'	moGenericBrowseSchemaControlSrc = DirectCast(oControl, GenericBrowseSchemaControl)
		moGenericBrowseSchemaControl.SetPluginB(oDataConnectPlugin)
		moGenericBrowseSchemaControl.Attach(moDataConnectUI)
		moGenericBrowseSchemaControl.AddToMap()
		Return
		zzSetFeatureNode()

		Return

		moDataConnectUI.ConnectedToFeatureSource(msParcelShapePath)
		moDataConnectUI.RefreshFeatureSource(msParcelShapePath)
		moDataConnectUI.AddProviderNode(sSDFProviderName)
		oNode = moDataConnectUI.AddFeatureSourceNode("topoParcels")
		If oNode Is Nothing Then
			System.Windows.Forms.MessageBox.Show("oNode Is Nothing", "-244")
		Else
			System.Windows.Forms.MessageBox.Show(oNode.Text, "-251")
		End If
	End Sub

	Private Sub btn4_Click(sender As System.Object, e As System.EventArgs) Handles btn4.Click
		Dim oNode As System.Windows.Forms.TreeNode
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim oSHPConnectionPlugin As SHPConnectionPlugin

	 
		oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		moDataConnectUI.OnProviderNodeSelected(oNode)

		oDataConnectPlugin = moDataConnectUI.GetPluginConnection(msSHPProviderName)
		'
		oSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
	
		oSHPConnectionPlugin.Attach(msSHPProviderName, "topoLots")
		Return
		moGenericConfigureLayerControl.AddToMap()
	End Sub
	Private Sub btn5_Click(oSender As System.Object, e As System.EventArgs) Handles btn5.Click
		'	Dim oNode As System.Windows.Forms.TreeNode
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim oSHPConnectionPlugin As SHPConnectionPlugin
		Dim iConnectionState As OSGeo.FDO.Connections.ConnectionState
		'Dim moDataConnectUI As dmDataConnectUI = New dmDataConnectUI()
		'''''moDataConnectUI.Visible = False
		''''''''''''''''''''	Dim oAcMapDataConnect As dmDataConnectUI = New dmDataConnectUI()
		'''''''''	moDataConnectUI.SelectFeatureSource(msSHPProviderName)

		'''''''''	moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)  OK!


		''	oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		''	moDataConnectUI.OnProviderNodeSelected(oNode)
		moDataConnectUI.SelectProvider(msSHPProviderName)

		oDataConnectPlugin = moDataConnectUI.GetPluginConnection(msSHPProviderName)
		'
		oSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
		oSHPConnectionPlugin.SourceFileName = msLotShapePath
		iConnectionState = oSHPConnectionPlugin.ConnectWithName("SHP_X")

		System.Windows.Forms.MessageBox.Show(iConnectionState.ToString(), "324_!!!")
		'''''''''''		oSHPConnectionPlugin.AfterConnect(True)
		''''''''''''	moDataConnectUI.FeatureSourceAdded("SHP_X")

		'''''''''''		moDataConnectUI.RefreshProvider(oNode)
		''''''''''''''		moDataConnectUI.RefreshSHRProvider(oNode)

		'''''''''''''	System.Windows.Forms.MessageBox.Show(oSHPConnectionPlugin.FeatureSourceName, "324a")
		moDataConnectUI.RemoveProviderNode(msSHPProviderName)
		moDataConnectUI.AddProviderNode(msSHPProviderName)
		'	oSHPConnectionPlugin.Attach(msSHPProviderName, "topoLots")
		Return
		oSHPConnectionPlugin.SourceFileName = msParcelShapePath
		iConnectionState = oSHPConnectionPlugin.ConnectWithName("SHP_Y")
		Return
		'	Dim oSHPConnectionControl As SHPConnectionControl = New SHPConnectionControl


		'	oSHPConnectionControl.ConnectionName = "SHP_2"
		'	oSHPConnectionControl.SourceFileName = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"



		
	End Sub
	Private Sub zzSetControlAAA()
		Me.moDataConnectUI = New dmDataConnectUI()

		'
		'oTest
		'
		Me.moDataConnectUI.Location = New System.Drawing.Point(0, 0)
		Me.moDataConnectUI.Name = "oTest"
		Me.moDataConnectUI.Size = New System.Drawing.Size(600, 420)
		Me.moDataConnectUI.TabIndex = 0
		Me.Controls.Add(Me.moDataConnectUI)


	End Sub
	Public Sub Ad()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		moDataConnectUI.Select()
		Dim oSHPNode As System.Windows.Forms.TreeNode


		'oAcMapDataConnectUI.AcadDocumentActivatedHelper()
		moDataConnectUI.AddProviderNode(sSDFProviderName)
		oSHPNode = moDataConnectUI.AddProviderNode(msSHPProviderName)

		moDataConnectUI.SelectProvider(msSHPProviderName)
		moDataConnectUI.OnProviderNodeSelected(oSHPNode)

		moDataConnectUI.ConnectedToFeatureSource(msParcelShapePath)
		moDataConnectUI.AddFeatureSourceNode("topoParcels")
		moDataConnectUI.PostAddFeatureSourceNode("topoParcels")


		moDataConnectUI.FeatureSourceAdded("topoParcels")
		oEditor.WriteMessage("102 KKK !! " & vbCrLf)
		moDataConnectUI.LayerSourceAdded("topoParcels")
		oEditor.WriteMessage("120 !! " & vbCrLf)
		moDataConnectUI.SelectFeatureSource("topoParcels")
		oEditor.WriteMessage("121 !! " & vbCrLf)
		moDataConnectUI.ConnectedToFeatureSource(msLotShapePath)
		moDataConnectUI.FeatureSourceAdded("topoLots")
		oEditor.WriteMessage("110 !! " & vbCrLf)
		moDataConnectUI.LayerSourceAdded("topoLots")
		oEditor.WriteMessage("125 !! " & vbCrLf)
		moDataConnectUI.SelectFeatureSource("topoLots")
		oEditor.WriteMessage("140 !! " & vbCrLf)

		oEditor.WriteMessage("150 !! " & vbCrLf)


	End Sub
	Private Sub Ba()
		moDataConnectUI.RefreshProvider(msSHPProviderName)

		moDataConnectUI.RefreshFeatureSource(msParcelShapePath)
		System.Windows.Forms.MessageBox.Show("Ba-Refresh", "-232")
	End Sub
	
	Private Sub Bb()

	End Sub
	Public Sub Overlay()
		Try
			FeatureEditor.Overlay("topoLots", "topoParcels", "Union", "D:\aWork\ArcGIS\E_Acad\topoLots_UnionMa.sdf", "topoLots_Union_Ma", 6, 0.01, 0.5, 0.5)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "FeatureEditor 2329")
		End Try
	End Sub
	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		zzMyInitializeComponent()

		moEditor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		' Add any initialization after the InitializeComponent() call.
		zzLoadProviders()
		DMCommon.Debug.MsgBox("13_040f")
		'	moDataConnectUI.PopulateProvidersTree()
		zzDrawExtent(moDataConnectUI, Drawing.Color.Aqua)
		zzDrawExtent(moSHPConnectionControl, Drawing.Color.Red)
		zzDrawExtent(moConnectionConfigureSourceControl, Drawing.Color.Blue)
		DMCommon.Debug.MsgBox("13_040ע")
		zzDrawExtent(moGenericConfigureLayerControl, Drawing.Color.Purple)
		zzDrawExtent(moGenericBrowseSchemaControl, Drawing.Color.Green)
		'	zzDrawExtent(moSHPConnectionControl, Drawing.Color.Red)

		DMCommon.Debug.MsgBox("13_040י")


	End Sub
	Private Sub zzMyInitializeComponent()
		Me.moDataConnectUI = New dmDataConnectUI()

		'
		'oTest
		'
		Me.moDataConnectUI.Location = New System.Drawing.Point(1, 1)
		Me.moDataConnectUI.Name = "oTest"
		Me.moDataConnectUI.Size = New System.Drawing.Size(600, 420)
		Me.moDataConnectUI.TabIndex = 0
		Me.Controls.Add(Me.moDataConnectUI)

		Me.moSHPConnectionControl = New dmSHPConnectionControl
		'
		'
		'
		Me.moSHPConnectionControl.Location = New System.Drawing.Point(1, 440)
		Me.moSHPConnectionControl.Name = "SHPConnectionControl"
		Me.moSHPConnectionControl.Size = New System.Drawing.Size(600, 120)
		Me.moSHPConnectionControl.TabIndex = 1
		Me.Controls.Add(Me.moSHPConnectionControl)


		Me.moConnectionConfigureSourceControl = New dmConnectionConfigureSourceControl
		'
		'
		'
		Me.moConnectionConfigureSourceControl.Location = New System.Drawing.Point(1, 580)
		Me.moConnectionConfigureSourceControl.Name = "ConnectionConfigureSourceControl"
		Me.moConnectionConfigureSourceControl.Size = New System.Drawing.Size(600, 320)
		Me.moConnectionConfigureSourceControl.TabIndex = 2
		Me.Controls.Add(Me.moConnectionConfigureSourceControl)

		Me.moGenericConfigureLayerControl = New dmGenericConfigureLayerControl()
		'
		'
		'
		Me.moGenericConfigureLayerControl.Location = New System.Drawing.Point(1, 900)
		Me.moGenericConfigureLayerControl.Name = "moGenericConfigureLayerControl"
		Me.moGenericConfigureLayerControl.Size = New System.Drawing.Size(600, 280)
		Me.moGenericConfigureLayerControl.TabIndex = 2
		Me.Controls.Add(Me.moGenericConfigureLayerControl)

		Me.moGenericBrowseSchemaControl = New dmGenericBrowseSchemaControl()
		'
		'
		'
		Me.moGenericBrowseSchemaControl.Location = New System.Drawing.Point(1, 1200)
		Me.moGenericBrowseSchemaControl.Name = "moGenericConfigureLayerControl"
		Me.moGenericBrowseSchemaControl.Size = New System.Drawing.Size(600, 240)
		Me.moGenericBrowseSchemaControl.TabIndex = 2
		Me.Controls.Add(Me.moGenericBrowseSchemaControl)

	End Sub
	Private Sub zz()

	End Sub
	Private Sub zzResetSHPProvider()
		moDataConnectUI.RemoveProviderNode(msSHPProviderName)
		DMCommon.Debug.MsgBox("13_040fc")
		moSHPNode = moDataConnectUI.AddProviderNode(msSHPProviderName)
		DMCommon.Debug.MsgBox("13_040fd", moSHPNode.Nodes.Count)
		If moSHPNode.Nodes.Count > 0 Then
			moConnectionNode = moSHPNode.FirstNode
			System.Windows.Forms.MessageBox.Show(moConnectionNode.ToString(), "First Connection Node")
		End If
		moDataConnectUI.PostAddFeatureSourceNode(msSHPProviderName)
		DMCommon.Debug.MsgBox("13_040fe", moSHPNode.Nodes.Count)


	End Sub
	Private Sub zzSetFeatureNode()
		If moConnectionNode IsNot Nothing Then
			If moConnectionNode.Nodes.Count > 0 Then
				moFeatureNode = moConnectionNode.FirstNode
				System.Windows.Forms.MessageBox.Show(moFeatureNode.ToString(), "First Feature Node")
			End If
		End If
		If moFeatureNode IsNot Nothing Then
			moDataConnectUI.OnFeatureSourceNodeSelected(moFeatureNode)
		End If
	End Sub
	Private Sub zzLoadProviders()
		Dim oRegistry As IProviderRegistry = FeatureAccessManager.GetProviderRegistry()
		Dim colProviders As ProviderCollection = oRegistry.GetProviders()
		Dim oNode As System.Windows.Forms.TreeNode
		DMCommon.Debug.MsgBox("13_040ec", colProviders, DMCommon.Debug.ColCount(colProviders))
		For Each oProv As Provider In colProviders
			moEditor.WriteMessage("4!! " & oProv.Name & ":" & oProv.LibraryPath & ":" & CStr(oProv.IsManaged) & vbCrLf)
			'DMCommon.Debug.MsgBox("13_040eck", oProv.Name, oProv.LibraryPath, oProv.IsManaged)
			Try
				''''''''''''''moDataConnectUI.PopulateProvidersTree()
				oNode = moDataConnectUI.AddProviderNode(oProv.Name)
				If oProv.Name = msSHPProviderName Then
					moSHPNode = oNode
				End If
			Catch oEx As Exception
				moEditor.WriteMessage("22!! " & oEx.Message & vbCrLf)
				DMCommon.Debug.MsgBox("13_040ecm", oEx.Message, oEx.StackTrace)
			End Try
			'DMCommon.Debug.MsgBox("13_040ecn")
		Next
		DMCommon.Debug.MsgBox("13_040ed", colProviders, DMCommon.Debug.ColCount(colProviders))
		moDataConnectUI.PopulateProvidersTree()
		DMCommon.Debug.MsgBox("13_040ee", colProviders, DMCommon.Debug.ColCount(colProviders))
		Try
			moTreeView = moSHPNode.TreeView
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("13_040eem", oEx.Message, oEx.GetType())
		End Try

		DMCommon.Debug.MsgBox("13_040ef", colProviders, DMCommon.Debug.ColCount(colProviders))
		'	moDataConnectUI.DispSelectedNode()
	End Sub
	Private Sub SetConnection()
		Dim connMgr As IConnectionManager = FeatureAccessManager.GetConnectionManager()

		moConnection = CType(connMgr.CreateConnection(msSHPProviderName), IConnectionImp)

		Dim connInfo As IConnectionInfo = moConnection.ConnectionInfo

		Dim properties As IConnectionPropertyDictionary = connInfo.ConnectionProperties

		properties.SetProperty(msSHPPropFileName, msParcelShapePath)	'sUnionPath

		Dim connStateA As ConnectionState = moConnection.Open()
		System.Windows.Forms.MessageBox.Show(connStateA.ToString(), "FeatureEditor 129")
	End Sub
	Private Sub btnFind_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
		'	Overlay()
		zzResetSHPProvider()
		Dim oSenderA As Object = moDataConnectUI
		'	moDataConnectUI.Delete(oSenderA)
		moDataConnectUI.DispSelectedNode()


	End Sub
	
	Private Class dmDataConnectUI
		Inherits AcMapDataConnectUI
		Private moConnectionAddControl As Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl
		Public Sub New()
			'AddHandler DoDelete, AddressOf MyBase.ConnectionMenu_Delete_Click
		End Sub
		Public Function GetSHPPluginConnection() As Autodesk.Gis.Map.DataConnect.UI.SHPConnectionPlugin
			Return DirectCast(MyBase.LookupConnectionPlugin(SHPProviderName), SHPConnectionPlugin)
		End Function

		Public Sub Init()
			moConnectionAddControl = MyBase._pConnectionAddControl
			Try
				Dim odmConnectionAddControl As dmConnectionAddControl = CType(moConnectionAddControl, dmConnectionAddControl)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "FdmConnectionAddControl")
			End Try

		End Sub
		Public Sub DispSelectedNode()
			Dim oTreeNode As System.Windows.Forms.TreeNode
			Dim oDinVal As Object = MyBase._pSelectedTreeNode
			If oDinVal Is Nothing Then
				DMCommon.Debug.MsgBox("06_006d", "MyBase._pSelectedTreeNode is nothing")

			Else

				oTreeNode = MyBase._pSelectedTreeNode

				DMCommon.Debug.MsgBox("06_006c", oTreeNode.Name)
				oTreeNode.Remove()
			End If



		End Sub
		Public Sub TestDelete(oNode As System.Windows.Forms.TreeNode)
			RaiseEvent DoDelete(oNode, New System.EventArgs)
		End Sub
		Public Sub Delete()
         'Dim oSender As System.Object = moDataConnectUI
			Dim e As System.EventArgs = New System.EventArgs()
			Try
				MyBase.ConnectionMenu_Delete_Click(Me, e)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Delete")
			End Try


		End Sub
		Public Sub DeleteA(oSHPNode As System.Windows.Forms.TreeNode, e As System.EventArgs)
         'Dim oSender As System.Object = moDataConnectUI
			'	Dim e As System.EventArgs = System.EventArgs.Empty

			Try
				If oSHPNode IsNot Nothing Then
					System.Windows.Forms.MessageBox.Show(oSHPNode.ToString(), "oSHPNode.ToString")
					MyBase.OnFeatureSourceNodeSelected(oSHPNode)
					MyBase.ConnectionMenu_Delete_Click(oSHPNode, e)
				Else
					System.Windows.Forms.MessageBox.Show("oSHPNode Is Nothing", "DeleteA_1")
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DeleteA")
			End Try


		End Sub
		Public Sub DeleteB(oSHPNode As System.Windows.Forms.TreeNode, e As System.EventArgs)
         'Dim oSender As System.Object = moDataConnectUI
			'	Dim e As System.EventArgs = System.EventArgs.Empty
			Dim eCancel As System.ComponentModel.CancelEventArgs = New System.ComponentModel.CancelEventArgs(False)
			Try
				If oSHPNode IsNot Nothing Then
					System.Windows.Forms.MessageBox.Show(oSHPNode.ToString(), "oSHPNode.ToString_1")
					MyBase.OnFeatureSourceNodeSelected(oSHPNode)
					System.Windows.Forms.MessageBox.Show(oSHPNode.TreeView, "oSHPNode.ToString_2")

					MyBase.ConnectionMenu_Opening(Me, eCancel)
					System.Windows.Forms.MessageBox.Show("B", "oSHPNode.ToString_3")
					If Not eCancel.Cancel Then
						'''''''	MyBase.ConnectionMenu_Delete_Click(oSHPNode, e)
					End If

				Else
					System.Windows.Forms.MessageBox.Show("oSHPNode Is Nothing", "DeleteB_1")
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "DeleteB")
			End Try


		End Sub
		'DMUI_DELETEFEATURESOURCEUICMD

		Public Sub DeleteC()

			Dim oSHPNode As System.Windows.Forms.TreeNode

			oSHPNode = MyBase._pSelectedTreeNode
			 
			Dim eCancel As System.ComponentModel.CancelEventArgs = New System.ComponentModel.CancelEventArgs(False)
			Dim e As System.EventArgs = New System.EventArgs()

			Try
				If oSHPNode IsNot Nothing Then
					System.Windows.Forms.MessageBox.Show(oSHPNode.ToString(), "oSHPNode.ToString_1")
					'''''''''''	MyBase.OnFeatureSourceNodeSelected(oSHPNode)
					System.Windows.Forms.MessageBox.Show(oSHPNode.TreeView, "oSHPNode.ToString_2")
					MyBase.DisconnectedFromFeatureSource("BoundingBox")
					System.Windows.Forms.MessageBox.Show(oSHPNode.TreeView, "oSHPNode.ToString_3")
					oSHPNode.Tag = "ABC"
					MyBase.Tag = "XYZ"
					MyBase.ConnectionMenu_Opening(oSHPNode, eCancel)
					System.Windows.Forms.MessageBox.Show("C", "oSHPNode.ToString_4")
					If Not eCancel.Cancel Then
						'''''	MyBase.ConnectionMenu_Delete_Click(oSHPNode, e)
					End If

				Else
					System.Windows.Forms.MessageBox.Show("oSHPNode Is Nothing", "DeleteC_1")
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "DeleteC")
			End Try


		End Sub
		Public Sub DeleteD()
			Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
			DMAcadExt.AcadDocument.SendExec("DMUI_DELETEFEATURESOURCEUICMD", True)
		End Sub
		Public Sub DisconnectA()
			'	Dim i As Integer = DataSources.ConnectedDataSourcesCount()
			'	o.DisconnectAllDataSources()
		End Sub
		Public Sub AddDeleteEventHandler()
			AddHandler DoDelete, AddressOf MyBase.ConnectionMenu_Delete_Click
		End Sub
		Public Function GetPluginConnection(ByVal sSHPProviderName As String) As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
			Return MyBase.LookupConnectionPlugin(sSHPProviderName)
		End Function
		Public Function GetPluginConfigureSource() As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
			Return MyBase.LookupConfigureSourcePlugin(msSHPProviderName)
		End Function
		Public Function GetPluginBrowseSchema() As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
			Return MyBase.LookupBrowseSchemaPlugin(msSHPProviderName)
		End Function
		Public Sub Delete_Click(ByVal oSender As System.Object)

			Dim e As System.EventArgs
			e = New System.EventArgs()
			MyBase.ConnectionMenu_Delete_Click(oSender, e)
		End Sub

		Public Sub SetPlugin(ByRef oPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
			Dim bRes As Boolean

			Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin = New SHPConnectionPlugin()
			System.Windows.Forms.MessageBox.Show(CStr(bRes), "319 SetPlugin ")
			Try
				bRes = MyBase.AttachPlugin(oDataConnectPlugin)
				System.Windows.Forms.MessageBox.Show(CStr(bRes), "322 SetPlugin ")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "323 SetPlugin ")
			End Try
			oPlugin = oDataConnectPlugin
		End Sub
		Public Sub A1(oNode As System.Windows.Forms.TreeNode)
			MyBase.OnProviderNodeSelected(oNode)
		End Sub

		Public Sub RefreshSHRProvider(ByVal oNode As System.Windows.Forms.TreeNode)
			If oNode IsNot Nothing Then
				MyBase.RefreshProvider(oNode)
			End If
		End Sub
		Public Event DoDelete(ByVal oSender As System.Object, e As EventArgs)


	End Class

	Private Class dmConnectionAddControl
		Inherits Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl
		Public Sub New(oControl As Autodesk.Gis.Map.DataConnect.UI.ConnectionAddControl)

		End Sub
	End Class
	Private Class dmSHPConnectionControl
		Inherits SHPConnectionControl
		Private moSHPConnectionPlugin As SHPConnectionPlugin
		Public Function SetPluginA() As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
			Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin = New SHPConnectionPlugin()
			MyBase.SetPlugin(oDataConnectPlugin)
			Return oDataConnectPlugin
		End Function
		Public Sub SetPluginB(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
			Try
				MyBase.SetPlugin(oDataConnectPlugin)
				moSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "331 SetPlugin ")
			End Try


		End Sub
		Public Sub Test()
			Dim sMsg As String = "Nothing"
			Dim plugin As Object = MyBase._plugin
			If plugin IsNot Nothing Then
				sMsg = plugin.ToString()
			End If
			System.Windows.Forms.MessageBox.Show(sMsg, "341 Test ")
			Dim oMsg As System.Windows.Forms.Message
			MyBase.ConnectionName = "SHP_2"
			System.Windows.Forms.MessageBox.Show(sMsg, "341 AA Test ")
			MyBase.SourceFileName = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
			System.Windows.Forms.MessageBox.Show(sMsg, "341 BB Test ")
			'''''''	moSHPConnectionPlugin.ConnectionName = "SHP_2"
			moSHPConnectionPlugin.SourceFileName = "D:\aWork\ArcGIS\D\Lot\topoLots.shp"
			System.Windows.Forms.MessageBox.Show("", "342 Test ")
			MyBase.ProcessCmdKey(oMsg, Windows.Forms.Keys.Enter)
			System.Windows.Forms.MessageBox.Show("", "343 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.LayerSourceName, "351 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.FeatureSourceName, "352 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ProviderName, "353 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.Configuration, "354 Test ")

			'moSHPConnectionPlugin.Attach(
			'''''''''''''	moSHPConnectionPlugin.Attach(msSHPProviderName, "SHP_2")


		End Sub
		Public Sub Attach(oDataConnectUI As AcMapDataConnectUI)
			Dim sFeatureSource As String = moSHPConnectionPlugin.FeatureSourceName
			Dim sLayerSource As String = "topoLots"
			moSHPConnectionPlugin.Attach(oDataConnectUI, msSHPProviderName, sFeatureSource, sLayerSource)

			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.LayerSourceName, "351 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.FeatureSourceName, "352 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ProviderName, "353 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.Configuration, "354 Test ")
			'			System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.PluginControl.ToString(), "355 Test ")


		End Sub
	End Class
	Private Class dmConnectionConfigureSourceControl
		Inherits ConnectionConfigureSourceControl
	
		Public Sub SetParent(oDataConnectUI As dmDataConnectUI)
			MyBase._parent = oDataConnectUI
		End Sub
	End Class
	Private Class dmGenericConfigureLayerControl
		Inherits GenericConfigureLayerControl
		Private moGenericConfigureLayerPlugin As GenericConfigureLayerPlugin

		Public Sub SetPluginB(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
			Dim sMsg As String = "Nothing"
			Try
				MyBase._className = "SHP_2"
				MyBase._schemaName = "topLots"
				MyBase.SetPlugin(oDataConnectPlugin)
				moGenericConfigureLayerPlugin = DirectCast(oDataConnectPlugin, GenericConfigureLayerPlugin)
				System.Windows.Forms.MessageBox.Show(moGenericConfigureLayerPlugin.ToString(), "377 SetPluginB ")
				MyBase._plugin = oDataConnectPlugin

				System.Windows.Forms.MessageBox.Show(moGenericConfigureLayerPlugin.ProviderName, "378D SetPluginB ")
				'	MyBase.CoordinateSystemColumnVisible = True
				Dim plugin As Object = MyBase._plugin
				If plugin IsNot Nothing Then
					sMsg = plugin.ToString()
				End If
				System.Windows.Forms.MessageBox.Show(sMsg, "379A SetPluginB ")

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "377 SetPlugin ")
			End Try
		End Sub

		Public Sub Test()
			Dim sMsg As String = "Nothing"
			MyBase._className = "topLots"
			MyBase._schemaName = "Default"
			Dim plugin As Object = MyBase._plugin
			If plugin IsNot Nothing Then
				sMsg = plugin.ToString()
			Else
				MyBase._plugin = moGenericConfigureLayerPlugin
			End If
			MyBase.CreateAndAddListViewSchema()
			MyBase.toolStripAddToMap.BackColor = Drawing.Color.Red
			Dim oItem As SchemaClassTreeViewItem = New SchemaClassTreeViewItem("Default", SchemaClassTreeViewItem.ItemType.Schema)
			'	oItem.DefaultGeometry
			Try
				MyBase._listViewSchema.Items.Add(oItem)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "387 Test ")
			End Try

         '	MyBase.SchemaClassTreeView() 
			System.Windows.Forms.MessageBox.Show(sMsg, "371 Test ")

			''''	MyBase.panelCustom.BackColor = Drawing.Color.LightPink
		End Sub
		Public Function AddToMap() As Boolean
			Return MyBase.AddClassesToMap(False)
		End Function
	End Class
	Private Class dmGenericBrowseSchemaControl
		Inherits GenericBrowseSchemaControl
		Private moGenericBrowseSchemaPlugin As GenericBrowseSchemaPlugin
		Private moSchemaClassTreeView As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeView
		Public Sub SetPluginB(oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin)
			Dim sMsg As String = "Nothing"
			Try

				'	MyBase._strConnectionName = "SHP_2"	'"topLots"
				MyBase.SetPlugin(oDataConnectPlugin)
				moGenericBrowseSchemaPlugin = DirectCast(oDataConnectPlugin, GenericBrowseSchemaPlugin)
				'	System.Windows.Forms.MessageBox.Show(moGenericConfigureLayerPlugin.ToString(), "377 SetPluginB ")
				MyBase._plugin = oDataConnectPlugin
				moGenericBrowseSchemaPlugin.Connect()

				'	MyBase.CoordinateSystemColumnVisible = True
				Dim plugin As Object = MyBase._plugin
				If plugin IsNot Nothing Then
					sMsg = plugin.ToString()
				End If
				System.Windows.Forms.MessageBox.Show(sMsg, "379A SetPluginB ")

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "355 SetPlugin ")
			End Try
		End Sub
		Public Sub Attach(oDataConnectUI As AcMapDataConnectUI)
			Dim sFeatureSource As String = moGenericBrowseSchemaPlugin.FeatureSourceName
			Dim sLayerSource As String = "topoParcels"
			moGenericBrowseSchemaPlugin.Attach(oDataConnectUI, msSHPProviderName, sFeatureSource, sLayerSource)
			'	MyBase._strMapCoordSystem
			 

		End Sub

		Public Sub ABX()
			Dim oSchemaTreeViewItem As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem


			Dim oFeatureClassTreeViewItem As Autodesk.Gis.Map.DataConnect.UI.SchemaClassTreeViewItem

			moSchemaClassTreeView = MyBase._schemaClassTreeView
			oSchemaTreeViewItem = New SchemaClassTreeViewItem("Default", SchemaClassTreeViewItem.ItemType.Schema)
			moSchemaClassTreeView.AddSchema(oSchemaTreeViewItem)

			oFeatureClassTreeViewItem = New SchemaClassTreeViewItem("topoParcels", SchemaClassTreeViewItem.ItemType.Class)
			moSchemaClassTreeView.AddFeatureClass(oSchemaTreeViewItem, oFeatureClassTreeViewItem)
		End Sub
		Public Sub SetView()
			MyBase.CreateAndAddListViewSchema()
			Dim oListViewEx As Autodesk.Gis.Map.DataConnect.UI.ListViewEx
			Dim oItem As SchemaClassTreeViewItem = New SchemaClassTreeViewItem("Default", SchemaClassTreeViewItem.ItemType.Schema)
			Dim oItemA As System.Windows.Forms.ListViewItem
			Dim oSubItemA As System.Windows.Forms.ListViewItem

			MyBase._listViewSchema.CheckBoxes = True
			'	oItem.DefaultGeometry
			Try
				oListViewEx = MyBase._listViewSchema
				oItemA = oListViewEx.Items.Add(oItem)
				oSubItemA = New System.Windows.Forms.ListViewItem("topoLots")
				MyBase.AddSubItems(oSubItemA)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "387 Test ")
			End Try

		End Sub
		Public Sub AddToMap()
			MyBase.AddClassesToMap(False)
		End Sub
	End Class
	'DMUI_ADDLAYERSUICMD
	'DMUI_MAPREGISTERCMDSCMD


	
	Private Sub Refresh_Click(sender As System.Object, e As System.EventArgs) Handles btnRefresh.Click
		moDataConnectUI.Invalidate()
		zzResetSHPProvider()
	End Sub
	Private Sub zzDrawExtent(oControl As System.Windows.Forms.Control, tColor As System.Drawing.Color)
		'	oControl.Size
		Dim iUB As Integer
		If maControlExt Is Nothing Then
			iUB = -1
		Else
			iUB = maControlExt.GetUpperBound(0)

		End If
		iUB += 1
		ReDim Preserve maControlExt(iUB)
		ReDim Preserve mtaExtColor(iUB)
		maControlExt(iUB) = oControl
		mtaExtColor(iUB) = tColor

		 
		Me.Invalidate()
	End Sub

	Private Sub frmDataConnect_Paint(ByVal oSender As System.Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
		Dim g As System.Drawing.Graphics = e.Graphics
		Dim oPen As System.Drawing.Pen
		Dim tResRect As System.Drawing.Rectangle
		Dim tDrawLocation As System.Drawing.Point
		Dim tDrawSize As System.Drawing.Size

		For iIndex As Integer = 0 To maControlExt.GetUpperBound(0)
			oPen = New System.Drawing.Pen(mtaExtColor(iIndex), 1)
			tDrawLocation = New System.Drawing.Point(maControlExt(iIndex).Location.X - 1, maControlExt(iIndex).Location.Y - 1)
			tDrawSize = New System.Drawing.Size(maControlExt(iIndex).Size.Width + 1, maControlExt(iIndex).Size.Height + 1)
			'	tResRect = New System.Drawing.Rectangle(maControlExt(iIndex).Location, maControlExt(iIndex).Size)
			tResRect = New System.Drawing.Rectangle(tDrawLocation, tDrawSize)
			g.DrawRectangle(oPen, tResRect)
		Next

		'	oPen = New System.Drawing.Pen(Drawing.Brushes.Brown, 1)
		'tResRect = New System.Drawing.Rectangle(New System.Drawing.Point(720, 40), New System.Drawing.Size(20, 20))
		'	g.DrawRectangle(oPen, tResRect)
	End Sub

	Private Sub btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles btnDelete.Click
		'	moDataConnectUI.AddDeleteEventHandler()

		'	moDataConnectUI.TestDelete(moConnectionNode)
		Me.moDataConnectUI.Focus()

		'moDataConnectUI.DeleteB(moConnectionNode, e)
		moDataConnectUI.DeleteC()

	End Sub

	Private Sub Button1_Click_1(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim oSHPConnectionPlugin As SHPConnectionPlugin

		oDataConnectPlugin = moDataConnectUI.GetPluginConnection(msSHPProviderName)
		oSHPConnectionPlugin = DirectCast(oDataConnectPlugin, SHPConnectionPlugin)
		moSHPConnectionControl.SetPluginB(oDataConnectPlugin)
	End Sub

	Private Sub moDataConnectUI_Click(ByVal oSender As System.Object, e As System.EventArgs) Handles moDataConnectUI.Click

	End Sub

	Private Sub moDataConnectUI_DoubleClick(ByVal oSender As System.Object, e As System.EventArgs) Handles moDataConnectUI.DoubleClick

	End Sub

	Public Sub ConnectToShape(sConnectionName As String, sShapeFileName As String)
		'	Dim oNode As System.Windows.Forms.TreeNode
		'	Dim oDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		Dim iConnectionState As OSGeo.FDO.Connections.ConnectionState
		'''''''''''		Dim moDataConnectPlugin As Autodesk.Gis.Map.DataConnect.UI.IDataConnectPlugin
		''''''''''		Dim moSHPConnectionPlugin As SHPConnectionPlugin

		'	System.Windows.Forms.MessageBox.Show(sConnectionName & vbCrLf & sShapeFileName, "ConnectToShape 01_899_5")
		'	oNode = moDataConnectUI.GetProviderNode(msSHPProviderName)
		'	moDataConnectUI.OnProviderNodeSelected(oNode)
		'	If moSHPConnectionPlugin Is Nothing Then
		moDataConnectUI = New dmDataConnectUI()
		moDataConnectUI.ForceRereshOnFeatureSourceNodeSelected = True
		moDataConnectUI.AddProviderNode(SHPProviderName)     'new
		'''''''''''''''''''''''''''	moSHPConnectionControl = New dmSHPConnectionControl()	' new
		'DMCommon.Debug.MsgBox("13_030B", SHPProviderName, sConnectionName, sShapeFileName)

		moDataConnectUI.SelectProvider(SHPProviderName)



		moSHPConnectionPlugin = moDataConnectUI.GetSHPPluginConnection()

		'	moSHPConnectionPlugin = DirectCast(moDataConnectPlugin, SHPConnectionPlugin)
		moSHPConnectionPlugin.SourceFileName = sShapeFileName
		'''''''''''		moSHPConnectionPlugin.ConnectionName = sConnectionName

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName & vbCrLf & sConnectionName & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString(), "05_800")
		'	End If

		iConnectionState = moSHPConnectionPlugin.ConnectWithName(sConnectionName)
		'DMCommon.Debug.MsgBox("13_030D", sConnectionName, CInt(iConnectionState), iConnectionState.ToString())
		moDataConnectUI.SelectFeatureSource(sConnectionName)
		'System.Windows.Forms.MessageBox.Show(moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_801")
		'

		moDataConnectUI.RemoveProviderNode(SHPProviderName)

		moDataConnectUI.AddProviderNode(SHPProviderName)

		'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & moSHPConnectionPlugin.ConnectionName.ToString & vbCrLf & moSHPConnectionPlugin.ConnectionState.ToString & vbCrLf & iConnectionState.ToString(), "05_807")
	End Sub

	Private Sub Button2_Click_1(oSender As System.Object, e As EventArgs) Handles Button2.Click
		'ConnectToShape()
	End Sub
End Class