Option Explicit On
Option Strict On
Imports System.Data
Public Class frmPrjThemes
	Private Const miParamType As Integer = 0
	Public Shared moBaseFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Public Shared moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
	Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmProjectThemes
	Private moPrjMapThemes As DataTable
   Private mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
   Private mdicDissolves As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.enMapTheme)



	' iMapThemeID As DMAcadExt.enMapTheme
	Private miCurrentRow As Integer
	Private miProjectCode As Integer
   Private miDetailNo As Integer = 0
   Private moDWGProjectData As TopoManager.DWGProjectData
   '	Private WithEvents mfTopoCleanup As frmTopoCleanup
   '	Private WithEvents mfFDO_Overlay As frmFDO_Overlay
   Private WithEvents mfMapThemeBase As frmMapThemeBase = Nothing
   Private WithEvents mfCheckTheme As ICheckTheme = Nothing
   Private WithEvents mfDesign As Form = Nothing
   Private WithEvents mfTopoToClosedPgons As frmTopoToClosedPgons = Nothing
   Private WithEvents mfrmPgonSetView As frmPgonSetView = Nothing

   Private WithEvents mfMessages As frmMessages = Nothing
   Private WithEvents mfProperty As frmMapThemeProperty = Nothing
   Private WithEvents mfTplnView As frmTplnView = Nothing


   Private WithEvents mfReports As frmReports
   Private WithEvents mfUnidiv As frmUnidiv
   Private WithEvents mfUD_General As frmUD_General


   Private moDetailMenuItems() As System.Windows.Forms.ToolStripItem ' DetailMenuItem
   Private miDetailRowsCount As Integer
   Private moParams As TPlServerDB.dmParams
   '	Private moCurrentDataRow As DataRow
   Private moCurrentRowIndex As Integer
   '  Private miSourceMapThemeID As DMAcadExt.enMapTheme
   '  Private miDissolveMapThemeID As DMAcadExt.enMapTheme
   Private miOverlayAddMapThemeID As DMAcadExt.enMapTheme

   Private mbEventsEnabled As Boolean = False
   Private mfMPgonView As frmMPgonView
   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()

   End Sub
   Public Sub ActiveFormView(bVisible As Boolean)
      If mfUnidiv IsNot Nothing Then
         mfUnidiv.ActiveFormView(bVisible)
      End If

   End Sub

   Private Sub zzMyInitializeComponent()

      If TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState = ConnectionState.Open Then
         TPlServerDB.ServerDB.AddInitialize()
      End If

      '''''''''''''''	TopoManager.TPlanGraph.TplnProject.InitializeList()

      Me.dgvPrjThemes.AutoGenerateColumns = False
      Me.dgvPrjThemes.RowHeadersWidth = 23

      zzOpenProjectData()

      DMAcadExt.AcadTransaction.Start()
      Dim iDWGProjectCode As Integer = moDWGProjectData.ProjectCode

      If iDWGProjectCode <> 0 Then
         miProjectCode = iDWGProjectCode
         miDetailNo = moDWGProjectData.DetailNo
      Else
         Dim oMySettings As My.MySettings = New My.MySettings()
         Dim iLastProjectCode As Integer = oMySettings.LastProjectCode
         If iLastProjectCode <> 0 Then
            miProjectCode = iLastProjectCode
            miDetailNo = oMySettings.LastDetailNo

         End If
      End If
      moDWGProjectData.CloseDictionary()
      '  oDWGProjectData.CloseDictionary()
      DMAcadExt.AcadTransaction.Terminate()
      If miProjectCode <> 0 Then
         Me.tstProjectCode.Text = Convert.ToString(miProjectCode)
         zzDispDetailNo()
      End If

      '  System.Windows.Forms.MessageBox.Show(CStr(iLastProjectCode) & ":" & CStr(miProjectCode) & ":" & CStr(miDetailNo), "02_980a")
      '   zzGetProjectList()
      zzGetMyProjectList()
   End Sub
   Private Sub zzOpenProjectData()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()


      moDWGProjectData = New TopoManager.DWGProjectData()

      moDWGProjectData.OpenData(False, True)
      '  System.Windows.Forms.MessageBox.Show(CStr(miProjectCode) & ":" & CStr(miDetailNo) & vbCrLf & moDWGProjectData.ProjectCode.ToString() & ":" & moDWGProjectData.DetailNo.ToString(), "02_980b")
      DMAcadExt.AcadTransaction.Terminate()

      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzLoadParams()
      Try
         moParams = New TPlServerDB.dmParams(miProjectCode, miDetailNo, 0, miParamType)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzLoadParams")
      End Try


   End Sub

   Private Sub zzDispDetailNo()
      If miDetailNo = 0 Then
         Me.tslDetailNo.Text = String.Empty
      Else
         Me.tslDetailNo.Text = "/" & Convert.ToString(miDetailNo)
      End If
   End Sub
   Private Sub zzGetProjectList()

      Dim sComText As String = Nothing '= "SELECT TOP 40000 [ProjectCode] FROM [ProjectData].[dbo].[PrjList]"
      Select Case TPlServerDB.ServerDB.CurrentServerDB.Provider
         Case TPlServerDB.TPlProvider.ProviderSQLServer
            sComText = "SELECT TOP 40000 [ProjectCode] FROM [ProjectData].[dbo].[PrjList]"
         Case TPlServerDB.TPlProvider.ProviderJet
            sComText = "SELECT TOP 40000 [ProjectCode] FROM [PrjList]"
      End Select
      If sComText IsNot Nothing Then
         '	MessageBox.Show(sComText, "05_452")
         Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         Dim i As Integer
         If oDataReader IsNot Nothing Then

            While oDataReader.Read
               Me.tstProjectCode.AutoCompleteCustomSource.Add(Convert.ToString(oDataReader.GetInt32(0)))
               i += 1
            End While

            oDataReader.Close()
         End If
      End If

   End Sub
   Private Sub zzGetMyProjectList()
      Dim sComText As String

      sComText = "SELECT TOP (100) PERCENT PrjString FROM dbo.UserPrjList WHERE (UserName = 'boris') GROUP BY PrjString ORDER BY MAX(RecID) DESC"
      Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
      If oDataReader IsNot Nothing Then

         While oDataReader.Read
            Me.tstProjectCode.AutoCompleteCustomSource.Add(Convert.ToString(oDataReader.GetString(0)))

         End While

         oDataReader.Close()
      End If

      '  Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
   End Sub


   Private Function zzGetInputProjectCode() As Integer
      If IsNumeric(Me.tstProjectCode.Text) Then
         Try
            Return Convert.ToInt32(Me.tstProjectCode.Text)
         Catch oEx As Exception
            Return 0
         End Try
      Else
         Return 0
      End If
   End Function
   Private Sub zzSetInputProject()
      Dim iInputProjectCode As Integer = zzGetInputProjectCode()
      If iInputProjectCode <> 0 Then
         If miProjectCode <> iInputProjectCode Then
            miProjectCode = iInputProjectCode
            DMAcadExt.AppMessages.Clear()
         End If
      End If
   End Sub

   Private Sub zzSetProject()
      zzSetDetails()

      TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode = miProjectCode
      TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo

      zzSetProjectCodeForRead()
      zzLoadParams()
   End Sub
   Private Sub zzSaveLastProjectSetting()
      Dim oMySettings As My.MySettings = New My.MySettings()
      oMySettings.LastProjectCode = miProjectCode
      oMySettings.LastDetailNo = miDetailNo
      oMySettings.Save()
   End Sub
   Private Sub tstProjectCode_Leave(oSender As System.Object, e As System.EventArgs) Handles tstProjectCode.Leave
      zzSetInputProject()
      zzSetProject()
   End Sub
   Private Sub zzSetReadOnly(bValue As Boolean)
      Me.txtTopoName.ReadOnly = bValue
      Me.txtLinkLayers.ReadOnly = bValue
      Me.txtCentroidLayer.ReadOnly = bValue
      Me.txtCentroidBlockName.ReadOnly = bValue
      Me.txtLineTopoName.ReadOnly = bValue
      Me.txtLineLinkLayer.ReadOnly = bValue
      Me.txtClosedPgonsLayer.ReadOnly = bValue


   End Sub

   Private Sub zzSetProjectCodeForRead()
      Dim sComText As String = "SELECT [MapThemeID],[MapThemeName],[GraphTypeID],[GraphTypeName],[GraphTypeShortName],[GroupID],[FileName],[TopoName],[LinkLayers],[CentroidBlocks],[CentroidLayers],[ClosedPgonsLayers], [LineTopoName],[LineLinkLayer],[NodeBlocks],[NodeLayers],[MPgonLayers],[DissolveTopoName],[DissolveAttribExpr],[DissolveClosedPgonsLayers],[CleanupType],[LineCleanupType],[DesignSet],[SourceMapThemeID],[OverlayMapThemeID],[OverlayMapThemeID_A],[TabaPurpose],[DesignElementType],[TopoPriority],[SPointsBlocks],[SPointsLayers] FROM PrjMapThemesExt WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(miDetailNo) & ")"

      '  DMAcadExt.AcadDocument.WriteMessage("!!38 ComText: " & sComText)

      moPrjMapThemes = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sComText, CommandType.Text, "PrjMapThemes")
      Me.dgvPrjThemes.Columns.Clear()

      If moPrjMapThemes IsNot Nothing Then
         miCurrentRow = -1
         Me.dgvPrjThemes.DataSource = moPrjMapThemes

         '	Me.txtLinkLayers.DataBindings.Add("Text", moPrjMapThemes, "LinkLayers")
         '	Me.txtCentroidLayer.DataBindings.Add("Text", moPrjMapThemes, "CentroidLayers")
         zzSetDataBinding()

         zzSetPanelReadOnly(True)

         zzSetGridColumnsForRead()

         Me.dgvPrjThemes.ReadOnly = True ''''''''''''TEMP = False	'
         zzSetReadOnly(True)

         zzCreateMapThemesDictionary()

         zzSaveLastProjectSetting()

      End If
      '	052 461 24 01
      '	052 455 50 81
   End Sub
   Private Function zzGetMapThemeID(oRow As DataRow) As DMAcadExt.enMapTheme
      Const sFieldName As String = "MapThemeID"
      Dim iValue As Integer = DirectCast(oRow.Item(sFieldName), Integer)
      If [Enum].IsDefined(GetType(DMAcadExt.enMapTheme), iValue) Then
         Return CType(iValue, DMAcadExt.enMapTheme)
      Else
         System.Windows.Forms.MessageBox.Show("MapTheme=" & iValue.ToString(), "Err #428")
         Return DMAcadExt.enMapTheme.Undefined
      End If
   End Function

   Private Sub zzCreateMapThemesDictionary()
      Dim oRow As DataRow
      Dim iMapThemeID As DMAcadExt.enMapTheme
      Dim tMapThemeData As DMAcadExt.MapThemeData
      Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim oDataGridRow As System.Windows.Forms.DataGridViewRow

      mdicMapThemes = New Dictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)()
      mdicDissolves = New Dictionary(Of DMAcadExt.enMapTheme, DMAcadExt.enMapTheme)()

      TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.Undefined
      ' System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_543")

      For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
         oRow = moPrjMapThemes.Rows.Item(iIndex)
         iMapThemeID = zzGetMapThemeID(oRow)
         '    MessageBox.Show(CStr(iMapThemeID) & vbCrLf & iMapThemeID.ToString(), "02_467a")
         tMapThemeData = New DMAcadExt.MapThemeData(oRow)
         If tMapThemeData.LineTopoName Is Nothing Then
            MessageBox.Show(CInt(tMapThemeData.MapThemeID).ToString(), "08_495")
         End If
         Try
            mdicMapThemes.Add(iMapThemeID, tMapThemeData)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iMapThemeID), "frmPrjThemes - zzCreateMapThemesDictionary_3")
         End Try
      Next
      For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
         oRow = moPrjMapThemes.Rows.Item(iIndex)
         iMapThemeID = zzGetMapThemeID(oRow)
         '    MessageBox.Show(CStr(iMapThemeID) & vbCrLf & iMapThemeID.ToString(), "02_466")
         '  tMapThemeData = New DMAcadExt.MapThemeData(oRow)
         tMapThemeData = mdicMapThemes.Item(iMapThemeID)
         ' "frmPrjThemes - zzCreateMapThemesDictionary_3")
         If tMapThemeData.LineTopoName Is Nothing Then
            MessageBox.Show(CInt(tMapThemeData.MapThemeID).ToString(), "08_497")
         End If
         oDataGridRow = Me.dgvPrjThemes.Rows.Item(iIndex)
         If tMapThemeData.GraphType = DMAcadExt.enGraphType.MapLayerOverlay Then
            TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
         ElseIf iMapThemeID = DMAcadExt.enMapTheme.Parcels OrElse iMapThemeID = DMAcadExt.enMapTheme.UD_Parcels Then
            If tMapThemeData.GraphType = 1 Then
               TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.Topologia
            ElseIf tMapThemeData.GraphType = 2 Then
               TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.ClosedPolygons
            End If
            '	MessageBox.Show(TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod.ToString(), "10_150")
         ElseIf iMapThemeID = DMAcadExt.enMapTheme.Ownership Then
            DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Ownership
         End If

         Dim s As String = iMapThemeID.ToString & ":" & tMapThemeData.GraphType.ToString
         s &= vbCrLf & TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod.ToString()
         s &= vbCrLf & DMAcadExt.DMApp.AppID.ToString()


         ' System.Windows.Forms.MessageBox.Show(s, "#2164 - zzCreateMapThemesDictionary_12!!!")
         If tMapThemeData.SourceMapThemeID <> 0 AndAlso (tMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons OrElse tMapThemeData.GraphType = DMAcadExt.enGraphType.Topology) Then
            mdicDissolves.Add(tMapThemeData.SourceMapThemeID, iMapThemeID)
            '   miDissolveMapThemeID = iMapThemeID
            '   miSourceMapThemeID = tMapThemeData.SourceMapThemeID
         End If
         If tMapThemeData.OverlayMapThemeID_A <> 0 Then
            'TopoManager.TPlanGraph.TplnProject.OverlayMethod = DMAcadExt.enOverlayMethod.Undefined
            '	MessageBox.Show(tMapThemeData.TopoName & vbCrLf & tMapThemeData.LineTopoName & vbCrLf & CStr(tMapThemeData.OverlayMapThemeID_A), "05_560")
            miOverlayAddMapThemeID = tMapThemeData.OverlayMapThemeID_A
         End If
         Try
            oDataGridRow.Cells.Item(3).Value = zzOpenThemeForm(tMapThemeData, False)

            '	System.Windows.Forms.MessageBox.Show(s, "Base - zzCreateMapThemesDictionary_12!!!")
            '	oRow.Item(3) = zzOpenThemeForm(tMapThemeData, False)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzCreateMapThemesDictionary_2")
         End Try

      Next
      TopoManager.TPlanGraph.TplnProject.MapThemes = mdicMapThemes

      '   System.Windows.Forms.MessageBox.Show(DMAcadExt.DMApp.AppID.ToString(), "01_544")
   End Sub

   Private Sub zzSetDataBinding()
      Me.txtTopoName.DataBindings.Clear()
      Me.txtTopoName.DataBindings.Add("Text", moPrjMapThemes, "TopoName")

      Me.txtLinkLayers.DataBindings.Clear()
      Me.txtLinkLayers.DataBindings.Add("Text", moPrjMapThemes, "LinkLayers")

      Me.txtCentroidLayer.DataBindings.Clear()
      Me.txtCentroidLayer.DataBindings.Add("Text", moPrjMapThemes, "CentroidLayers")

      Me.txtCentroidBlockName.DataBindings.Clear()
      Me.txtCentroidBlockName.DataBindings.Add("Text", moPrjMapThemes, "CentroidBlocks")

      Me.txtNodeLayer.DataBindings.Clear()
      Me.txtNodeLayer.DataBindings.Add("Text", moPrjMapThemes, "NodeLayers")

      Me.txtNodeBlockName.DataBindings.Clear()
      Me.txtNodeBlockName.DataBindings.Add("Text", moPrjMapThemes, "NodeBlocks")

      Me.txtLineTopoName.DataBindings.Clear()
      Me.txtLineTopoName.DataBindings.Add("Text", moPrjMapThemes, "LineTopoName")

      Me.txtLineLinkLayer.DataBindings.Clear()
      Me.txtLineLinkLayer.DataBindings.Add("Text", moPrjMapThemes, "LineLinkLayer")

      Me.txtClosedPgonsLayer.DataBindings.Clear()
      Me.txtClosedPgonsLayer.DataBindings.Add("Text", moPrjMapThemes, "ClosedPgonsLayers")


   End Sub
   Private Sub zzSetPanelReadOnly(bReadOnly As Boolean)
      Me.txtLinkLayers.ReadOnly = bReadOnly
      Me.txtCentroidBlockName.ReadOnly = bReadOnly
      Me.txtCentroidLayer.ReadOnly = bReadOnly

   End Sub
   Private Sub zzSetGridColumnsForRead()
      '	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
      Dim oColumn As DataGridViewTextBoxColumn
      Dim oChkColumn As DataGridViewCheckBoxColumn
      Dim oUI_Settings As TPlServerDB.UI_Settings = New TPlServerDB.UI_Settings(TPlServerDB.enResourceTheme.AcFrmProjectThemes, 1, True, False)
      Dim oItemSetting As TPlServerDB.ItemSetting

      oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      oItemSetting = oUI_Settings.GetItem(0)
      With oColumn
         .Name = ""
         .HeaderText = oItemSetting.Text
         .Width = oItemSetting.Size
         .Name = oItemSetting.ItemName
         .DataPropertyName = oItemSetting.ItemName
         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With

      Try
         Me.dgvPrjThemes.Columns.Add(oColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
      End Try


      oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      oItemSetting = oUI_Settings.GetItem(1)
      With oColumn
         .HeaderText = oItemSetting.Text
         .Width = oItemSetting.Size
         If oItemSetting.ItemName Is Nothing Then
            .Name = "Col" & Convert.ToString(oItemSetting.Size)

         Else
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
         End If

         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With

      Try
         Me.dgvPrjThemes.Columns.Add(oColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
      End Try

      ' Column:

      oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      oItemSetting = oUI_Settings.GetItem(2)
      With oColumn
         .HeaderText = oItemSetting.Text
         .Width = oItemSetting.Size
         If oItemSetting.ItemName Is Nothing Then
            .Name = "Col" & Convert.ToString(oItemSetting.Size)

         Else
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
         End If

         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With

      Try
         Me.dgvPrjThemes.Columns.Add(oColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
      End Try



      oChkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
      oItemSetting = oUI_Settings.GetItem(3)
      With oChkColumn
         .HeaderText = oItemSetting.Text
         .Width = oItemSetting.Size
         If oItemSetting.ItemName Is Nothing Then
            .Name = "Col" & Convert.ToString(oItemSetting.Size)

         Else
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
         End If

         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With

      Try
         Me.dgvPrjThemes.Columns.Add(oChkColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
      End Try
   End Sub

   Private Sub zzSetGridColumnsForEdit()
      '	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
      Dim oTxtColumn As DataGridViewTextBoxColumn

      Dim oCmbColumn As DataGridViewComboBoxColumn
      Dim oChkColumn As DataGridViewCheckBoxColumn
      Dim oUI_Settings As TPlServerDB.UI_Settings = New TPlServerDB.UI_Settings(TPlServerDB.enResourceTheme.AcFrmProjectThemes, 2, True, False)
      Dim oItemSetting As TPlServerDB.ItemSetting
      If oUI_Settings IsNot Nothing Then
         oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
         oItemSetting = oUI_Settings.GetItem(0)
         With oTxtColumn
            .HeaderText = oItemSetting.Text
            .Width = oItemSetting.Size
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
            .SortMode = DataGridViewColumnSortMode.NotSortable
         End With

         Try
            Me.dgvPrjThemes.Columns.Add(oTxtColumn)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
         End Try

         oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
         oItemSetting = oUI_Settings.GetItem(1)
         With oTxtColumn
            .HeaderText = oItemSetting.Text
            .Width = oItemSetting.Size
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
            .SortMode = DataGridViewColumnSortMode.NotSortable
         End With

         Try
            Me.dgvPrjThemes.Columns.Add(oTxtColumn)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
         End Try



         If False Then
            Dim oDataItems As DMCommon.DataItems
            Dim sComText As String = "SELECT [GraphTypeID],[GraphTypeName] FROM [GraphTypes]"
            Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

            oCmbColumn = New System.Windows.Forms.DataGridViewComboBoxColumn
            oItemSetting = oUI_Settings.GetItem(1)
            With oCmbColumn
               .HeaderText = oItemSetting.Text
               .Width = oItemSetting.Size
               If oItemSetting.ItemName Is Nothing Then
                  .Name = "Col" & Convert.ToString(oItemSetting.Size)

               Else
                  .Name = oItemSetting.ItemName
                  .DataPropertyName = oItemSetting.ItemName
               End If
               .FlatStyle = FlatStyle.Flat
               '.Items.Add( New DMCommon.ItemData()

               '	End If
               .MaxDropDownItems = 2
               .ValueMember = DMCommon.ItemData.ValueMember
               .DisplayMember = DMCommon.ItemData.DisplayMember
               If oDataReader IsNot Nothing Then
                  oDataItems = New DMCommon.DataItems(oDataReader)
                  .DataSource = oDataItems.ItemList
               End If


               .SortMode = DataGridViewColumnSortMode.NotSortable
            End With

            Try
               Me.dgvPrjThemes.Columns.Add(oCmbColumn)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
            End Try
         End If


         oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
         oItemSetting = oUI_Settings.GetItem(2)
         With oTxtColumn
            .HeaderText = oItemSetting.Text
            .Width = oItemSetting.Size
            .Name = oItemSetting.ItemName
            .DataPropertyName = oItemSetting.ItemName
            .SortMode = DataGridViewColumnSortMode.NotSortable
         End With

         Try
            Me.dgvPrjThemes.Columns.Add(oTxtColumn)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
         End Try


         oChkColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
         oItemSetting = oUI_Settings.GetItem(3)
         With oChkColumn
            .HeaderText = oItemSetting.Text
            .Width = oItemSetting.Size
            If oItemSetting.ItemName Is Nothing Then
               .Name = "Col" & Convert.ToString(oItemSetting.Size)

            Else
               .Name = oItemSetting.ItemName
               .DataPropertyName = oItemSetting.ItemName
            End If

            .SortMode = DataGridViewColumnSortMode.NotSortable
         End With

         Try
            Me.dgvPrjThemes.Columns.Add(oChkColumn)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzSetGridColumns_2")
         End Try
      End If
      zzSetPanelReadOnly(False)
   End Sub

   Private Sub tstMainTop_AutoSizeChanged(oSender As System.Object, e As EventArgs) Handles tstMainTop.AutoSizeChanged

   End Sub
   Private Sub tstMainTop_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles tstMainTop.ItemClicked
      Dim tMapThemeData As DMAcadExt.MapThemeData
      '   System.Windows.Forms.MessageBox.Show(e.ClickedItem.Name, "tstTopo_ItemClicked")

      Me.Cursor = Cursors.WaitCursor
      Select Case e.ClickedItem.Name
         Case Me.tsbCleanup.Name
            tMapThemeData = zzGetCurrentMapThemeData()
            '  System.Windows.Forms.MessageBox.Show(tMapThemeData.TopoName & vbCrLf & tMapThemeData.GraphType.ToString & vbCrLf & tMapThemeData.GraphTypeName.ToString & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.LineTopoName, "Nothing"), "08_318")
            If tMapThemeData.IsNotEmpty Then
               zzOpenThemeForm(tMapThemeData, True)
            End If


         Case Me.tsbEditProject.Name
            If Me.tsbEditProject.Checked Then
               zzSetProjectCodeForRead()
               Me.tsbUpdate.Enabled = False
            Else
               zzEditProject()
               Me.tsbUpdate.Enabled = True
            End If

            'zzExportToShape()
         Case Me.tsbUpdate.Name
            If Me.tsbEditProject.Checked Then
               If zzUpdate() Then
                  Me.tsbEditProject.Checked = False
                  Me.tsbUpdate.Enabled = False
               End If

            End If

         Case Me.tsbCalculate.Name
            zzSetProjectData()
            Select Case DMAcadExt.DMApp.AppID
               Case DMAcadExt.enApplications.Taba
                  zzCalculateTabaProject(0)
               Case DMAcadExt.enApplications.Ownership
                  zzCalculateOwnerProject()
            End Select

         Case Me.tsbReports.Name
            zzOpenReports()
         Case Me.tsbPaint.Name


            DMCommon.Debug.MsgBox(".ItemClicked()", Me.tsbPaint.Name)
            tMapThemeData = zzGetCurrentMapThemeData()
            If tMapThemeData.IsNotEmpty Then
               zzOpenDesignForm(tMapThemeData)
            End If
         Case Me.ssbUnidiv.Name

            zzOpenUnidiv()
         Case Me.tsbToClosedPgons.Name
            tMapThemeData = zzGetCurrentMapThemeData()

            '    MessageBox.Show(tMapThemeData.TopoName, "02_374")
            If tMapThemeData.IsNotEmpty Then
               zzOpenTopoToClosedPgons(tMapThemeData)
            End If

         Case Me.tsbCheckPgons.Name
            tMapThemeData = zzGetCurrentMapThemeData()

            '  MessageBox.Show(tMapThemeData.TopoName, "02_375")
            If tMapThemeData.IsNotEmpty Then
               zzOpenCheckPgons(tMapThemeData)
            End If
         Case Me.tsbDispTable.Name
            zzDispTpln()
         Case Me.tsbMessages.Name
            zzOpenMsgForm()
         Case Me.tsbProperties.Name
            TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode = miProjectCode
            TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo
            '  MessageBox.Show(CStr(TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode) & ":" & CStr(TPlServerDB.ServerDB.CurrentProjectDB.DetailNo), "02_422")
            tMapThemeData = zzGetCurrentMapThemeData()
            ' zzOpenPropertyForm(tMapThemeData)
            zzOpenPropertyForm()
         Case Me.tsbOpenDWG.Name
            zzOpenDWG()
         Case Me.tsbSaveDWG.Name
            zzSaveDWG()
         Case Me.tsbExit.Name
            Me.Close()
      End Select
      Me.Cursor = Cursors.Default
      '''''''''''''''		DMAcadExt.AcadDocument.UpdateScreen()
   End Sub
   Private Sub zzSetProjectData()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()

      moDWGProjectData.OpenData(True, False)
      moDWGProjectData.UpdateData(miProjectCode, miDetailNo)



      DMAcadExt.AcadTransaction.Terminate()

      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzDispTpln()
      ' mfTplnView = New TPlanGraph.frmTplnView
      '  mfTplnView.ShowDialog()
      '  mfTplnView.Dispose()
      TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
      If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
         If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
            mfTplnView = New frmTplnView()
            mfTplnView.MapThemes = mdicMapThemes
            ' mfTplnView.InitMapThemeData = zzGetCurrentMapThemeData()

         End If

         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
         If mfTplnView IsNot Nothing AndAlso Not mfTplnView.IsDisposed Then

            mfTplnView.InitMapThemeData = zzGetCurrentMapThemeData()

         End If

         Me.Visible = False
      End If
   End Sub
   Private Sub zzOpenDWG()
      Dim sDWGName As String = moParams.GetStrValue(101)
      If Not String.IsNullOrEmpty(sDWGName) Then
         DMAcadExt.AcadDocument.OpenDocument(sDWGName, False)
         zzSetProject()
      End If
   End Sub

   Private Sub zzCalculateTabaProject(iRegion As Integer)
      Dim tPlanMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tApprMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tPropMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

      Dim tParcelMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tBlockMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tMithamMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tMithamProxMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()



      Dim tExproMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tMerhavMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tZoneMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tExproZoneMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
      Dim tFragmentMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

      Dim tUD_ParcelMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()



      'MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345")

      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.PlanApproved, tPlanMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tApprMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotProposed, tPropMapThemeData)

      Dim b1 As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Blocks, tBlockMapThemeData)
      Dim bMitham As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Mitham, tMithamMapThemeData)
      Dim bMithamProx As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.MithamProx, tMithamProxMapThemeData)


      '   MessageBox.Show(b1.ToString() & ":" & b1.ToString() & vbCrLf & "'" & tMithamMapThemeData.CentroidBlock & "'", "09_760")

      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Merhav, tMerhavMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Zone, tZoneMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.ExproZoneOverlay, tExproZoneMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Fragments, tFragmentMapThemeData)
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.UD_Parcels, tUD_ParcelMapThemeData)




      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Exprop, tExproMapThemeData)

      TopoManager.TPlanGraph.TplnProject.ParcelMapThemeData = tParcelMapThemeData
      TopoManager.TPlanGraph.TplnProject.BlockMapThemeData = tBlockMapThemeData
      If bMitham Then
         TopoManager.TPlanGraph.TplnProject.MithamMapThemeData = tMithamMapThemeData
      End If
      If bMithamProx Then
         TopoManager.TPlanGraph.TplnProject.MithamProxMapThemeData = tMithamProxMapThemeData
      End If
      For Each iMapTheme As DMAcadExt.enMapTheme In mdicMapThemes.Keys
         ''''''''''''   DMCommon.Debug.MsgBox("12_225", iMapTheme.ToString)
      Next



      TopoManager.TPlanGraph.TplnProject.PlanMapThemeData = tPlanMapThemeData
      TopoManager.TPlanGraph.TplnProject.ApprMapThemeData = tApprMapThemeData
      TopoManager.TPlanGraph.TplnProject.PropMapThemeData = tPropMapThemeData

      TopoManager.TPlanGraph.TplnProject.ExproMapThemeData = tExproMapThemeData
      TopoManager.TPlanGraph.TplnProject.MerhavMapThemeData = tMerhavMapThemeData
      TopoManager.TPlanGraph.TplnProject.ZoneMapThemeData = tZoneMapThemeData
      TopoManager.TPlanGraph.TplnProject.ExproZoneMapThemeData = tExproZoneMapThemeData

      TopoManager.TPlanGraph.TplnProject.FragmentMapThemeData = tFragmentMapThemeData

      TopoManager.TPlanGraph.TplnProject.UD_ParcelMapThemeData = tUD_ParcelMapThemeData






      Dim bTest As Boolean = mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Merhav, tMerhavMapThemeData)
      '    MessageBox.Show(CStr(mdicMapThemes.Count) & vbCrLf & tMerhavMapThemeData.CentroidBlocks & vbCrLf & CStr(bTest), "02_499")
      If miOverlayAddMapThemeID <> DMAcadExt.enMapTheme.Undefined Then

      End If

      '	MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345after")
      '		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)
      Dim iColorSetID As Integer = 0
      Dim sLanduseList As String = Nothing
      Dim iTopoPurpose As DMAcadExt.enTopoPurpose
      If False Then
         iTopoPurpose = DMAcadExt.enTopoPurpose.Approved
         iColorSetID = 100
         TopoManager.TPlanGraph.TplnLanduse.SetNamedColorSet(iTopoPurpose, iColorSetID)
      End If
      If False Then 'Me.chkProposed.Checked()
         iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
         iColorSetID = 100
         TopoManager.TPlanGraph.TplnLanduse.SetNamedColorSet(iTopoPurpose, iColorSetID)
         '	miDefaultTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
      End If

      '		TopoManager.TPlanGraph.TplnProject.ParcelGeoMethod = TopoManager.TPlanGraph.enGeoMethod.Topologia
      If tApprMapThemeData.IsNotEmpty Then
         TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Approved
      ElseIf tPropMapThemeData.IsNotEmpty Then
         TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
      Else
         TopoManager.TPlanGraph.TplnProject.DefaultTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
      End If
      Dim bPlan As Boolean, bApproved As Boolean, bProposed As Boolean, bParcel As Boolean, bBlock As Boolean, bExpro As Boolean, bMerhav As Boolean 'bMitham As Boolean,
      '     Dim bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean
      bPlan = tPlanMapThemeData.IsNotEmpty
      bApproved = tApprMapThemeData.IsNotEmpty
      bProposed = tPropMapThemeData.IsNotEmpty
      bParcel = tParcelMapThemeData.IsNotEmpty
      bBlock = tBlockMapThemeData.IsNotEmpty
      bMitham = tMithamMapThemeData.IsNotEmpty


      bMerhav = tMerhavMapThemeData.IsNotEmpty

      If tParcelMapThemeData.IsNotEmpty Then
         bExpro = True
      Else
         bExpro = False
      End If
      bExpro = tExproMapThemeData.IsNotEmpty  ''''''''''''''''NB!
      Dim dicTopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)
      Dim oPgonSet As FDO.TplnPolygonSet = Nothing

      If bApproved Then
         '  MessageBox.Show(CStr(bApproved) & ":" & tApprMapThemeData.GraphType.ToString(), "04_779")
         If tApprMapThemeData.GraphType = DMAcadExt.enGraphType.ClosedPolygons Then
            oPgonSet = zzCreatePgonSet(tApprMapThemeData)

         End If
      End If
      If oPgonSet IsNot Nothing Then
         dicTopoPolygons = oPgonSet.TopoPolygons
         TopoManager.TPlanGraph.TplnProject.TopoPolygons = dicTopoPolygons
      End If

      '	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
      DMAcadExt.AcadDocument.WriteDebugMessage("@54  iRegion =" & iRegion.ToString())
      '  DMCommon.Debug.MsgBox("09_322", True, "iRegion =", iRegion.ToString())
      If iRegion > 1 Then
         TopoManager.TPlanGraph.TplnProject.CalculateRegion(iRegion)
      Else
         TopoManager.TPlanGraph.TplnProject.Calculate()

      End If


      '     zzSaveDWG()

   End Sub
   Private Function zzCreatePgonSet(mtMapThemeData As DMAcadExt.MapThemeData) As FDO.TplnPolygonSet
      '   MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370")
      Dim bCurrentLayerOK As Boolean = False
      Dim mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      ' System.Windows.Forms.MessageBox.Show(mtSourceMapThemeData.MapThemeID.ToString & vbCrLf & mtSourceMapThemeData.TopoPurpose.ToString & vbCrLf & mtSourceMapThemeData.GraphType.ToString() & vbCrLf & mtSourceMapThemeData.TopoName & vbCrLf & mtSourceMapThemeData.ClosedPgonsLayers, "07_001s")
      Dim oPgonSet As FDO.TplnPolygonSet = New FDO.TplnPolygonSet(mtMapThemeData.MapThemeID, mtMapThemeData.TopoPurpose, mtMapThemeData.TopoName)
      '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oEnt As Autodesk.AutoCAD.DatabaseServices.DBObject = Nothing

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
      Select Case mtMapThemeData.MapThemeID
         Case DMAcadExt.enMapTheme.Blocks
            TopoManager.TPlanGraph.TplnBlock.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.Parcels
            TopoManager.TPlanGraph.TplnParcel.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.UD_Parcels
            ''''''''''''    UnidivNet.UD_Parcel.Initialize(mtMapThemeData)
         Case DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
            TopoManager.TPlanGraph.TplnLot.Initialize(mtMapThemeData)
      End Select







      bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)

      '    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("pseudo", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      ' System.Windows.Forms.MessageBox.Show(mtMapThemeData.ClosedPgonsLayers & vbCrLf & "", "07_002")
      Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(mtMapThemeData.ClosedPgonsLayers)
      '
      Dim dicCentroids As System.Collections.Generic.IDictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, Autodesk.AutoCAD.DatabaseServices.BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(mtMapThemeData.CentroidBlock, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)   '"pclp004"
      '
      mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew(mtMapThemeData.CentroidBlocks, mtMapThemeData.CentroidLayers)
      DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count) & "; " & mtMapThemeData.CentroidBlocks & "; " & mtMapThemeData.CentroidLayers & "; " & mtMapThemeData.ClosedPgonsLayers)
      oPgonSet.AddPolylineIDs(colPolygonIDs)
      '    bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(msIntersectionPointsLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
      '
      'oPgonSet.AddBlocks(dicCentroids)
      oPgonSet.AddCentroids(mcolCentroids)
      '
      'TopoManager.TPlanGraph.TplnBlock.Initialize(mtSourceMapThemeData)
      'TopoManager.TPlanGraph.TplnParcel.Initialize(mtSourceMapThemeData)
      'TopoManager.TPlanGraph.TplnLot.Initialize(mtSourceMapThemeData)
      'UnidivNet.UD_Parcel.Initialize(mtSourceMapThemeData)


      '

      '   Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)

      '''''''''''''	
      oPgonSet.CalculateNewF(True)




      '  DMCommon.ExcelLog.Open()
      '  moPgonSet.InfoToExcel()


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.CloseMessage()
      DMAcadExt.AcadDocument.Unlock()
      Return oPgonSet

   End Function

   Private Sub zzSaveDWG()
      Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
      If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
         moParams.SetIdData(sFileName)
         moParams.Update()
         DMAcadExt.AcadDocument.WriteMessage("File Name: '" & sFileName & "' saved")
         DMAcadExt.AcadDocument.CloseMessage()
      End If
   End Sub
   Private Sub zzCalculateOwnerProject()
      Dim tOwnerMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)


      'MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345")
      mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Ownership, tOwnerMapThemeData)


      '	MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty), "04_345after")
      '		mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.Parcels, tParcelMapThemeData)


      '	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
      TopoManager.TPlanGraph.TplnOwnerProject.Init(tOwnerMapThemeData)
      TopoManager.TPlanGraph.TplnOwnerProject.Calculate()
      Dim sFileName As String = DMAcadExt.AcadDocument.GetFileName()
      If sFileName IsNot Nothing AndAlso moParams IsNot Nothing Then
         moParams.SetIdData(sFileName)
         moParams.Update()
      End If
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub zzOpenMsgForm()
      '	Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim bIsDone As Boolean = False
      Try
         mfMessages = New frmMessages()
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfMessages)
         Me.Visible = False
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzOpenMsgForm")
      End Try
   End Sub

   Private Function zzOpenThemeForm(tMapThemeData As DMAcadExt.MapThemeData, ByVal bLoad As Boolean) As Boolean
      '	Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim bIsDone As Boolean = False
      '  MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & CStr(tMapThemeData.MapThemeID) & ":" & tMapThemeData.GraphType.ToString(), "05_342a")
      mfMapThemeBase = Nothing
      '& vbCrLf & miSourceMapThemeID.ToString() & ":" & miDissolveMapThemeID.ToString()
      '   System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString(), "07_500")
      Try

         Select Case tMapThemeData.GraphType
            Case DMAcadExt.enGraphType.Topology
               '	mfTopoCleanup = New frmTopoCleanup(tMapThemeData)
               mfMapThemeBase = New frmTopoCleanup(tMapThemeData)

               '	fMapThemeBase = mfTopoCleanup
            Case DMAcadExt.enGraphType.ClosedPolygons
               'MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & tMapThemeData.MapThemeID.ToString(), "05_355c")
               Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
               Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
               If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
                  'MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & CStr(tMapThemeData.MapThemeID), "05_361")
                  mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)      '''''''''''''''''' Prev Version 	
                  '''''''''''''''''' Design  VersionmfMapThemeBase = New frmPgonSet(tSourceMapThemeData) 
               Else
                  mfMapThemeBase = New frmClosedPgons(tMapThemeData)
               End If

            Case DMAcadExt.enGraphType.Undefined
               MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & CStr(tMapThemeData.MapThemeID), "05_979")
               Dim tAddMapThemeData As DMAcadExt.MapThemeData = Nothing
               '''''''''''''''''''''''''''''''''''''''''''''NB 081116
               'If tMapThemeData.MapThemeID = miSourceMapThemeID Then
               '   If mdicMapThemes.TryGetValue(miDissolveMapThemeID, tAddMapThemeData) Then
               '      mfMapThemeBase = New frmClosedPgons(tAddMapThemeData, tMapThemeData)
               '   End If
               'ElseIf tMapThemeData.MapThemeID = miDissolveMapThemeID Then
               '   If mdicMapThemes.TryGetValue(miSourceMapThemeID, tAddMapThemeData) Then
               '      mfMapThemeBase = New frmClosedPgons(tMapThemeData, tAddMapThemeData)
               '   End If
               'End If
               '''''''''''''''''''''''''''''''''''''''''''''NB 081116
            Case DMAcadExt.enGraphType.MapLayerOverlay
               Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               Dim tOverlayMapThemeData_A As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()

               If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
                  If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
                     If tMapThemeData.OverlayMapThemeID_A <> 0 Then
                        mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID_A, tOverlayMapThemeData_A)
                     End If
                     ' System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString(), "07_510")
                     mfMapThemeBase = New frmFDO_Overlay(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData, tOverlayMapThemeData_A)
                  End If
               End If
            Case DMAcadExt.enGraphType.AnalyticClip
               Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               Dim tOverlayMapThemeData_A As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
                  If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
                     If tMapThemeData.OverlayMapThemeID_A <> 0 Then
                        mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID_A, tOverlayMapThemeData_A)
                     End If
                     mfMapThemeBase = New frmAnaliticClip(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData, tOverlayMapThemeData_A)
                  End If
               End If
            Case DMAcadExt.enGraphType.TopoOverlay
               Dim tSourceMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               Dim tOverlayMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
               '  MessageBox.Show(tMapThemeData.GraphType.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString() & vbCrLf & mdicMapThemes.Count, "05_400")
               If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
                  If mdicMapThemes.TryGetValue(tMapThemeData.OverlayMapThemeID, tOverlayMapThemeData) Then
                     '  System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & tMapThemeData.SourceMapThemeID.ToString() & vbCrLf & tMapThemeData.OverlayMapThemeID.ToString() & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.TopoName, "TOPONAME") & vbCrLf & DMCommon.Functions.CStrN(tMapThemeData.LineTopoName, "?LineTopoName"), "07_520")
                     mfMapThemeBase = New frmTopoOverlay(tMapThemeData, tSourceMapThemeData, tOverlayMapThemeData)
                  End If
               End If

         End Select
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenThemeForm")
      End Try


      If mfMapThemeBase IsNot Nothing AndAlso Not mfMapThemeBase.IsDisposed Then
         '  System.Windows.Forms.MessageBox.Show(mfMapThemeBase.Name & vbCrLf & CStr(mfMapThemeBase.IsDone), "04_012")
         bIsDone = mfMapThemeBase.IsDone
      End If

      If bLoad AndAlso (mfMapThemeBase IsNot Nothing) AndAlso (Not mfMapThemeBase.IsDisposed) Then
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfMapThemeBase)
         Me.Visible = False
      End If
      Return bIsDone
   End Function
   Private Function zzGetDissolveMapThemeData(tMapThemeData As DMAcadExt.MapThemeData, ByRef tSourceMapThemeData As DMAcadExt.MapThemeData, ByRef tDissolveMapThemeData As DMAcadExt.MapThemeData) As Boolean
      '	Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
      '	Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing

      ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
      ' Dim iSourceMapThemeID As DMAcadExt.enMapTheme
      Dim iDissolveMapThemeID As DMAcadExt.enMapTheme

      Dim bRes As Boolean = False
      If tMapThemeData.SourceMapThemeID = DMAcadExt.enMapTheme.Undefined Then
         If mdicDissolves.TryGetValue(tMapThemeData.MapThemeID, iDissolveMapThemeID) AndAlso mdicMapThemes.TryGetValue(iDissolveMapThemeID, tDissolveMapThemeData) Then

            '  MessageBox.Show(iDissolveMapThemeID.ToString(), "05_429 Diss")
            tSourceMapThemeData = tMapThemeData
            bRes = True
         End If
      Else
         If mdicMapThemes.TryGetValue(tMapThemeData.SourceMapThemeID, tSourceMapThemeData) Then
            tDissolveMapThemeData = tMapThemeData
            bRes = True
         End If




      End If



      ' MessageBox.Show(tMapThemeData.MapThemeID.ToString() & vbCrLf & tSourceMapThemeData.MapThemeID.ToString() & ":" & tDissolveMapThemeData.MapThemeID.ToString(), "05_444")

      Return bRes
      'If tMapThemeData.MapThemeID = miSourceMapThemeID Then
      '   If mdicMapThemes.TryGetValue(miDissolveMapThemeID, tDissolveMapThemeData) Then
      '      tSourceMapThemeData = tMapThemeData
      '      Return True
      '   End If
      'ElseIf tMapThemeData.MapThemeID = miDissolveMapThemeID Then
      '   If mdicMapThemes.TryGetValue(miSourceMapThemeID, tSourceMapThemeData) Then
      '      tDissolveMapThemeData = tMapThemeData
      '      Return True
      '   End If
      'End If
   End Function


   Private Function zzOpenDesignForm(tMapThemeData As DMAcadExt.MapThemeData) As Boolean
      '	Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim bIsDone As Boolean = False
      Try
         Select Case tMapThemeData.DesignElement
            Case 1
               Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
               Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
               If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
                  'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

               End If
               ' MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tMapThemeData.GraphType.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")

               mfDesign = New frmPaintLanduse(tMapThemeData, tDissolveMapThemeData)
         End Select
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenDesignForm")
      End Try
      If mfDesign IsNot Nothing AndAlso Not mfDesign.IsDisposed Then
         Try
            Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDesign)
            Me.Visible = False
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenDesignForm_1")
         End Try
      End If
      Return bIsDone
   End Function

   Private Function zzOpenPropertyForm() As Boolean
      '	Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim bIsDone As Boolean = False
      Try
         zzOpenProjectData()

         '	MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")
         '  System.Windows.Forms.MessageBox.Show(CStr(moDWGProjectData.ProjectCode), "07_570")
         DMAcadExt.AcadTransaction.Start()
         mfProperty = New frmMapThemeProperty(moDWGProjectData.GetPropertyView())
         DMAcadExt.AcadTransaction.Terminate()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm")
      End Try
      If mfProperty IsNot Nothing Then
         Try
            Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProperty)
            Me.Visible = False
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm_1")
         End Try
      End If
      Return bIsDone
   End Function
   Private Function zzOpenPropertyFormOld(tMapThemeData As DMAcadExt.MapThemeData) As Boolean
      '	Dim fMapThemeBase As frmMapThemeBase = Nothing
      Dim bIsDone As Boolean = False
      Try

         '	MessageBox.Show(tMapThemeData.TopoPurpose.ToString() & vbCrLf & tSourceMapThemeData.TopoPurpose.ToString() & vbCrLf & tDissolveMapThemeData.TopoPurpose.ToString(), "25_439")
         mfProperty = New frmMapThemeProperty(tMapThemeData)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm")
      End Try
      If mfProperty IsNot Nothing Then
         Try
            Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProperty)
            Me.Visible = False
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenPropertyForm_1")
         End Try
      End If
      Return bIsDone
   End Function

   Private Sub zzOpenTopoToClosedPgons(tMapThemeData As DMAcadExt.MapThemeData)
      'Dim bIsDone As Boolean = False
      Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
      Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
      If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
         'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)
         '  MessageBox.Show(tMapThemeData.TopoName, "02_376")
         '   MessageBox.Show(DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.LinkLayer, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.LinkLayer, "Nothing"), "02_364")
      Else
         tSourceMapThemeData = tMapThemeData
      End If
      Try
         '   MessageBox.Show(DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.LinkLayer, "Nothing"), "02_370")
         mfTopoToClosedPgons = New frmTopoToClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)
         Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoToClosedPgons)
         Me.Visible = False
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenTopoToClosedPgons")
      End Try
   End Sub
   Private Sub zzOpenCheckPgons(tMapThemeData As DMAcadExt.MapThemeData)
      'Dim bIsDone As Boolean = False
      Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
      Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
      Dim bDissolve As Boolean
      '  MessageBox.Show(tMapThemeData.TopoName, "02_376")
      If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
         'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

      Else
         tSourceMapThemeData = tMapThemeData
      End If

      mfrmPgonSetView = New frmPgonSetView(tMapThemeData, tDissolveMapThemeData, bDissolve)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfrmPgonSetView)
      Me.Visible = False
   End Sub
   Private Sub zzOpenCheckPgonsOld(tMapThemeData As DMAcadExt.MapThemeData)
      'Dim bIsDone As Boolean = False
      Dim tSourceMapThemeData As DMAcadExt.MapThemeData = Nothing
      Dim tDissolveMapThemeData As DMAcadExt.MapThemeData = Nothing
      MessageBox.Show(tMapThemeData.TopoName, "02_377")
      If zzGetDissolveMapThemeData(tMapThemeData, tSourceMapThemeData, tDissolveMapThemeData) Then
         'mfMapThemeBase = New frmClosedPgons(tSourceMapThemeData, tDissolveMapThemeData)

      Else
         tSourceMapThemeData = tMapThemeData
      End If
      Try
         '   MessageBox.Show(DMCommon.Functions.CStrN(tMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.TopoName, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(tSourceMapThemeData.TopoName, "Nothing") & ":" & DMCommon.Functions.CStrN(tDissolveMapThemeData.LinkLayer, "Nothing"), "02_370")
         Select Case tSourceMapThemeData.MapThemeID
            Case DMAcadExt.enMapTheme.Parcels, DMAcadExt.enMapTheme.LotApproved, DMAcadExt.enMapTheme.LotProposed
               mfrmPgonSetView = New frmPgonSetView(tSourceMapThemeData, tDissolveMapThemeData, False)
               Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
               Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfrmPgonSetView)
               Me.Visible = False

         End Select

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "frmPrjThemes - zzOpenTopoToClosedPgons")
      End Try
   End Sub
   Private Sub zzOpenUnidiv()
      mfUnidiv = New frmUnidiv(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      '  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUnidiv)
      Me.Visible = False

   End Sub
   Private Sub zzOpenUD_General()
      mfUD_General = New frmUD_General(True)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      '  DMCommon.Debug.MsgBox("11_090", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfUD_General)
      Me.Visible = False

   End Sub
   Private Sub zzOpenReports()

      'MessageBox.Show(CStr(oMapThemeData.CleanupType), "05_100")

      mfReports = New frmReports
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfReports)
      Me.Visible = False
   End Sub
   Private Sub zzEditProject()

      Dim sComText As String = "SELECT [MapThemeID],[MapThemeName],[GraphTypeID],[GraphTypeName],[GraphTypeShortName],[GroupID],[FileName],[SelRow],[TopoName],[LinkLayers],[CentroidBlocks],[CentroidLayers],[ClosedPgonsLayers],[LineTopoName],[LineLinkLayer],[LineTolerance],[NodeBlocks],[NodeLayers],[MPgonLayers],[DissolveTopoName],[DissolveAttribExpr],[DissolveClosedPgonsLayers],[CleanupType],[LineCleanupType],[DesignSet],[SourceMapThemeID],[OverlayMapThemeID],[OverlayMapThemeID_A],[SPointsBlocks],[SPointsLayers] FROM PrjMapThemesExt WHERE (ProjectCode=" & CStr(miProjectCode) & ") AND (Detail=" & CStr(miDetailNo) & ")"

      moPrjMapThemes = TPlServerDB.ServerDB.CurrentProjectDB.GetDataTable(sComText, CommandType.Text, "PrjMapThemes")
      '   MessageBox.Show(CStr(moPrjMapThemes.Rows.Count), "05_110")
      Me.dgvPrjThemes.Columns.Clear()
      miCurrentRow = -1
      Me.dgvPrjThemes.DataSource = moPrjMapThemes


      sComText = "AddPrjThemes" '"GetProjectData"	'"SELECT * FROM GetProjectData" '
      Dim oaParams(1) As Common.DbParameter
      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
      Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText, CommandType.StoredProcedure, oaParams)
      Dim i As Integer = -1
      '	Dim oValue As Object
      Dim sFieldName As String
      If oDataReader IsNot Nothing Then
         i = 0
         Dim oDataRow As DataRow
         While oDataReader.Read
            i += 1
            oDataRow = moPrjMapThemes.NewRow()
            For iFieldIndex As Integer = 0 To oDataReader.FieldCount - 1
               If Not oDataReader.IsDBNull(iFieldIndex) Then
                  sFieldName = oDataReader.GetName(iFieldIndex)
                  oDataRow.Item(sFieldName) = oDataReader.GetValue(iFieldIndex)
               End If
            Next

            oDataRow.Item("SelRow") = 0
            moPrjMapThemes.Rows.Add(oDataRow)
         End While
         oDataReader.Close()
         zzSaveLastProjectSetting()
      End If

      ''''''''''''	System.Windows.Forms.MessageBox.Show(CStr(i), "iii")
      Me.dgvPrjThemes.Columns.Clear()
      zzSetGridColumnsForEdit()
      Me.bnsPrjThemes.DataSource = moPrjMapThemes
      zzSetDataBinding()
      Me.dgvPrjThemes.ReadOnly = False
      zzSetReadOnly(False)
   End Sub


   Private Function zzGetCurrentMapThemeData() As DMAcadExt.MapThemeData
      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvPrjThemes.CurrentCell
      If oCurrentCell IsNot Nothing Then
         moCurrentRowIndex = oCurrentCell.RowIndex
         Return zzGetMapThemeData(oCurrentCell.RowIndex)
      Else
         Return New DMAcadExt.MapThemeData()
      End If

   End Function

   Private Function zzGetMapThemeData(iRowIndex As Integer) As DMAcadExt.MapThemeData
      Dim oDataRow As DataRow = moPrjMapThemes.Rows.Item(iRowIndex)
      Dim iMapThemeID As DMAcadExt.enMapTheme = zzGetMapThemeID(oDataRow)
      '	System.Windows.Forms.MessageBox.Show(iMapThemeID.ToString() & vbCrLf & mdicMapThemes.Count.ToString(), "08_304")
      Dim tMapThemeData As DMAcadExt.MapThemeData = Nothing

      mdicMapThemes.TryGetValue(iMapThemeID, tMapThemeData)
      '   System.Windows.Forms.MessageBox.Show(tMapThemeData.MapThemeID.ToString & vbCrLf & tMapThemeData.GraphType.ToString(), "08_305")
      Return tMapThemeData
   End Function

   Private Sub dgvPrjThemes_DefaultCellStyleChanged(oSender As System.Object, e As EventArgs) Handles dgvPrjThemes.DefaultCellStyleChanged

   End Sub
   Private Sub dgvPrjThemes_RowEnter(oSender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvPrjThemes.RowEnter
      If miCurrentRow <> e.RowIndex Then
         miCurrentRow = e.RowIndex

         '''''''''''''''temp		Me.txtLinkLayers.Text = DMCommon.Functions.CStrN(moPrjMapThemes.Rows.Item(miCurrentRow).Item("LinkLayers"))
      End If
   End Sub

   Private Function zzUpdate() As Boolean
      Dim sComText As String = "SELECT * FROM PrjMapThemes"
      Dim oDataAdapter As Common.DbDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sComText, CommandType.Text, , , True, True)
      Dim oDestDataTable As DataTable = New DataTable("Project")
      Dim oSourceRow As DataRow
      Dim oNewRow As DataRow
      Dim bSelRow As Boolean = True
      Dim sFieldName As String
      Dim oValue As Object
      Dim bError As Boolean
      oDataAdapter.FillSchema(oDestDataTable, SchemaType.Source)
      '		DMCommon.Functions.DispDataTableCols(oDestDataTable, "oDestDataTable", True)
      '		DMCommon.Functions.DispDataTableCols(moPrjMapThemes, "moPrjMapThemes", True)

      For iIndex As Integer = 0 To moPrjMapThemes.Rows.Count - 1
         oSourceRow = moPrjMapThemes.Rows.Item(iIndex)
         bSelRow = DirectCast(oSourceRow.Item("SelRow"), Boolean)

         If bSelRow Then
            oNewRow = oDestDataTable.NewRow()
            For iFieldIndex As Integer = 0 To oDestDataTable.Columns.Count - 1
               sFieldName = oDestDataTable.Columns.Item(iFieldIndex).ColumnName
               Select Case sFieldName
                  Case "ProjectCode"
                     oNewRow.Item(sFieldName) = miProjectCode
                  Case "Detail"
                     oNewRow.Item(sFieldName) = miDetailNo
                  Case "ClosedPgonsLayers"
                     oValue = oSourceRow.Item(sFieldName)
                     If Not IsDBNull(oValue) Then
                        oNewRow.Item(sFieldName) = oValue
                     End If
                  Case Else
                     '
                     oValue = oSourceRow.Item(sFieldName)
                     '   DMCommon.Debug.MsgBox("09_864s", sFieldName, oValue)
                     If Not IsDBNull(oValue) Then
                        oNewRow.Item(sFieldName) = oValue
                     End If

               End Select
            Next
            Try
               oDestDataTable.Rows.Add(oNewRow)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes-zzUpdate1")
               bError = True
            End Try

         End If
      Next
      If Not bError AndAlso zzEraseProject() Then
         Try
            oDataAdapter.Update(oDestDataTable)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmPrjThemes - zzUpdate")
            bError = True
         End Try
         If Not bError Then
            zzSetProjectCodeForRead()
         End If

      End If
      Return Not bError
   End Function



   Private Function zzEraseProject() As Boolean
      Dim oCommandErr As System.Data.Common.DbException = Nothing
      Dim sComText As String = "DELETE FROM dbo.PrjMapThemes WHERE (ProjectCode = " & Convert.ToString(miProjectCode) & ") AND (Detail = " & Convert.ToString(miDetailNo) & ")"
      If TPlServerDB.ServerDB.CurrentProjectDB.RunCommand(sComText, CommandType.Text, , oCommandErr) = -1 Then
         Return False
      Else
         Return True
      End If

   End Function



   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub






   Private Sub frmPrjThemes_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      DMAcadExt.AppMessages.Clear()
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      TopoManager.TPlanGraph.TplnProject.Close()
      '	TPlServerDB.ServerDB.CurrentProjectDB.Close()
      '	TPlServerDB.ServerDB.CurrentServerDB.Close()
   End Sub


   Private Sub frmPrjThemes_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
      zzSetProject()
      ' MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "03_117a")

   End Sub
   Private Sub mfMapThemeBase_FormClosed(oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfMapThemeBase.FormClosed
      zzOnFormClose()
   End Sub
   Private Sub tstProjectCode_KeyDown(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tstProjectCode.KeyDown
      '	frmPrjThemes.vb:line 1338
      If e.KeyCode = Keys.Escape Then
         zzEscProjectCode()
      ElseIf e.KeyCode = Keys.Enter Then
         Me.dgvPrjThemes.Focus()
      End If
   End Sub
   Private Sub zzEscProjectCode()
      Dim iInputProjectCode As Integer = zzGetInputProjectCode()
      If miProjectCode = 0 Then
         Me.tstProjectCode.Text = String.Empty
      ElseIf iInputProjectCode <> miProjectCode Then
         Me.tstProjectCode.Text = Convert.ToString(miProjectCode)
      End If
      MessageBox.Show(CStr(iInputProjectCode) & ":" & CStr(miProjectCode) & ":" & Me.tstProjectCode.Text, "01_740")
   End Sub
   Private Sub ddbDetails_Click(sender As System.Object, e As System.EventArgs) Handles ddbDetails.Click
      If miDetailRowsCount = 0 AndAlso moPrjMapThemes.Rows.Count > 0 Then
         zzEditDetails()
      End If

   End Sub
   Private Sub zzEditDetails()
      Dim fEditDetails As frmEditDetails = New frmEditDetails(miProjectCode)
      Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
      Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fEditDetails)
      If fEditDetails.DialogResult = Windows.Forms.DialogResult.OK Then
         zzSetDetails()
      End If
   End Sub

   Private Sub zzSetDetails()
      Dim sComText As String = "SELECT Detail, DetailName FROM PrjDetails WHERE ProjectCode=" & CStr(miProjectCode)
      Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Details")
      Dim oDataRow As DataRow
      Dim iDetailNo As Integer
      Dim sDetailNo As String
      Dim bCheck As Boolean
      Me.ddbDetails.DropDownItems.Clear()
      If oDataTable IsNot Nothing Then
         miDetailRowsCount = oDataTable.Rows.Count
         If miDetailRowsCount > 0 Then
            ReDim moDetailMenuItems(miDetailRowsCount + 2)
            moDetailMenuItems(0) = New DetailMenuItem(0, , (miDetailNo = 0))
            '	zzAddDetailMenuItem(0, "DetailItem_0", " - ", (miDetailNo = 0))
            For iIndex As Integer = 0 To miDetailRowsCount - 1

               oDataRow = oDataTable.Rows.Item(iIndex)
               iDetailNo = DirectCast(oDataRow.Item(0), Integer)
               sDetailNo = Convert.ToString(iDetailNo)
               bCheck = (miDetailNo = iDetailNo)

               '	moDetailMenuItems(iIndex + 1) = New DetailMenuItem(iIndex + 1, oDataRow.Item(1).ToString(), bCheck)
               moDetailMenuItems(iIndex + 1) = New DetailMenuItem(iDetailNo, oDataRow.Item(1).ToString(), bCheck)

               '	MessageBox.Show(sItemName, "05_477")
               'zzAddDetailMenuItem(iIndex + 1, "DetailItem_" & sDetailNo, sDetailNo & " - " & oDataRow.Item(1).ToString(), bCheck)
            Next
            moDetailMenuItems(miDetailRowsCount + 1) = New System.Windows.Forms.ToolStripSeparator
            moDetailMenuItems(miDetailRowsCount + 2) = New DetailMenuItem()
            '	zzAddDetailMenuItem(miDetailRowsCount + 1, "DetailItemEdit", "Add/Edit")
            Me.ddbDetails.DropDownItems.AddRange(moDetailMenuItems)
            zzAddPrjStr(CStr(miProjectCode))

         Else
            miDetailNo = 0
            zzDispDetailNo()
         End If
      End If
   End Sub
   Private Sub zzAddPrjStr(sPrjStr As String)
      Dim sComText As String = "AddUserPrjStr" '"GetProjectData"	'"SELECT * FROM GetProjectData" '
      Dim oaParams(1) As Common.DbParameter
      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prUserName", DbType.String, System.Environment.UserName)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prPrjStr", DbType.String, sPrjStr)
      TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.StoredProcedure, oaParams)

   End Sub
   Private Sub zzAddDetailMenuItem(iIndex As Integer, sName As String, sText As String, Optional bChecked As Boolean = False)
      '	moDetailMenuItems(iIndex) = New System.Windows.Forms.ToolStripMenuItem()
      With moDetailMenuItems(iIndex)
         .Name = sName
         .Size = New System.Drawing.Size(152, 22)
         .Text = sText
         '''''''''''''''''''''''		.Checked = True ' bChecked
      End With
      '	AddHandler moDetailMenuItems(iIndex).Click, AddressOf Details_DropDownItemClicked
   End Sub
   Private Sub ddbDetails_DropDownItemClicked(oSender As System.Object, e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles ddbDetails.DropDownItemClicked
      Dim oToolStripMenuItem As DetailMenuItem = DirectCast(e.ClickedItem, DetailMenuItem)
      Dim sItemName As String = oToolStripMenuItem.Name
      'MessageBox.Show(sItemName, "05_430")
      Select Case sItemName
         Case DetailMenuItem.EditItemName
            zzEditDetails()
         Case Else
            miDetailNo = oToolStripMenuItem.DetailNo
            TPlServerDB.ServerDB.CurrentProjectDB.DetailNo = miDetailNo
            zzDispDetailNo()
            zzSetMenuItemChecked()
            zzSetProjectCodeForRead()
            '	MessageBox.Show(e.ClickedItem.GetType().ToString() & vbCrLf & sItemName & ":" & oToolStripMenuItem.Checked.ToString(), "04_329")
      End Select

   End Sub
   Private Sub zzSetMenuItemChecked()
      Dim oDetailMenuItem As DetailMenuItem
      '	Dim s As String = ""
      '	MessageBox.Show(CStr(moDetailMenuItems.GetUpperBound(0)) & ":" & CStr(miDetailNo), "05_300")
      For iIndex As Integer = 0 To moDetailMenuItems.GetUpperBound(0)
         If moDetailMenuItems(iIndex).GetType().ToString() <> "System.Windows.Forms.ToolStripSeparator" Then
            oDetailMenuItem = DirectCast(moDetailMenuItems(iIndex), DetailMenuItem)
            oDetailMenuItem.SetChecked(miDetailNo)
            '	s &= CStr(oDetailMenuItem.Checked) & vbCrLf
         End If

         '	MessageBox.Show(moDetailMenuItems(iIndex).GetType().ToString(), "03_210")
         '''''''''''''moDetailMenuItems(iIndex).SetChecked(miDetailNo)
      Next
      '	MessageBox.Show(s, "05_301")
   End Sub
   Private Class DetailMenuItem
      Inherits System.Windows.Forms.ToolStripMenuItem
      Public Const EditItemName As String = "DetailItemEdit"
      Const msNamePrefix As String = "DetailItem"
      Const msNameDelim As String = "_"

      Const msDefaultItemText As String = " - "
      Const msEditItemText As String = "Add/Edit"
      Private Shared mtSize As System.Drawing.Size = New System.Drawing.Size(152, 22)
      Private miDetailNo As Integer
      Public Sub New(Optional iIndex As Integer = -1, Optional sText As String = "", Optional bChecked As Boolean = False)
         miDetailNo = iIndex
         Dim sName As String
         Select Case iIndex
            Case -1
               sName = EditItemName
               sText = msEditItemText
            Case 0
               sName = zzGetNumName(iIndex)
               sText = msDefaultItemText
            Case Else
               sName = zzGetNumName(iIndex)
               sText = Convert.ToString(iIndex) & " - " & sText
         End Select
         MyBase.Name = sName
         MyBase.Text = sText
         MyBase.Size = mtSize
         MyBase.Checked = bChecked

      End Sub
      Public ReadOnly Property DetailNo As Integer
         Get
            Return miDetailNo
         End Get
      End Property
      Public Sub SetChecked(iCurrentDetailNo As Integer)
         If miDetailNo = iCurrentDetailNo Then
            Me.Checked = True
         Else
            Me.Checked = False
         End If

      End Sub
      Private Function zzGetNumName(iIndex As Integer) As String
         Return msNamePrefix & msNameDelim & Convert.ToString(iIndex)
      End Function
   End Class





   Private Sub ToolStripMenuItem1_Click(sender As System.Object, e As System.EventArgs)
      'MessageBox.Show("ToolStripMenuItem1.Click", "01_872")
   End Sub

   Private Sub mfDesign_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfDesign.FormClosed
      Me.Visible = True
   End Sub


   Private Sub mfMapThemeBase_ToClose() Handles mfMapThemeBase.OnClose
      zzOnFormClose()
   End Sub
   Private Sub zzOnFormClose()

      Me.Visible = True
      'DirectCast(moCurrentDataRow.Item(3), Boolean)	' oDataGridRow.Cells.Item(3).Value
      Dim bNew As Boolean = mfMapThemeBase.IsDone
      '	MessageBox.Show(CStr(moCurrentRowIndex) & ":" & CStr(bNew), "04_777")

      Dim oDataGridRow As System.Windows.Forms.DataGridViewRow = Me.dgvPrjThemes.Rows.Item(moCurrentRowIndex)
      oDataGridRow.Cells.Item(3).Value = bNew
   End Sub

   Private Sub mfUnidiv_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfUnidiv.FormClosed
      TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL(True)
      Me.Visible = True
   End Sub
   Private Sub mfUD_General_FormClosed(sender As Object, e As FormClosedEventArgs) Handles mfUD_General.FormClosed
      Me.Visible = True
   End Sub

   Private Sub mfReports_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfReports.FormClosed
      Me.Visible = True
   End Sub

   Private Sub mfMessages_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfMessages.FormClosed
      Me.Visible = True
   End Sub
   Private Sub mfTopoToClosedPgons_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfTopoToClosedPgons.FormClosed
      Me.Visible = True
   End Sub



   Private Sub mfProperty_FormClosed(ByVal oSender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles mfProperty.FormClosed
      Me.Visible = True
   End Sub


   Private Sub mfrmPgonSetView_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfrmPgonSetView.FormClosed
      Me.Visible = True
   End Sub


   Private Sub frmPrjThemes_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
      Me.dgvPrjThemes.Height = Me.Panel1.Location.Y - Me.dgvPrjThemes.Location.Y
   End Sub




   Private Sub mfTplnView_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles mfTplnView.FormClosing
      Me.Visible = True
   End Sub



   Private Sub ddbUtilities_DropDownItemClicked(oSender As System.Object, e As ToolStripItemClickedEventArgs) Handles ddbUtilities.DropDownItemClicked
      '    MessageBox.Show(e.ClickedItem.Name, "04_328")
      Dim tLotMapThemeData As DMAcadExt.MapThemeData = Nothing
      Dim tPlanMapThemeData As DMAcadExt.MapThemeData = Nothing

      Select Case e.ClickedItem.Name
         Case Me.tmiPlanName.Name
            If mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.LotApproved, tLotMapThemeData) Then
               '  MessageBox.Show(tLotMapThemeData.TopoName, "04_329")
            End If
            If mdicMapThemes.TryGetValue(DMAcadExt.enMapTheme.PlanApproved, tPlanMapThemeData) Then
               ' MessageBox.Show(tPlanMapThemeData.TopoName, "04_331")
            End If
            TopoManager.TPlanGraph.TplnProject.UpdateLotsByPlans(DMAcadExt.enTopoPurpose.Approved)
         Case Me.tmiRegion.Name

            TopoManager.TPlanGraph.TplnProject.UpdateLotsByRegions(DMAcadExt.enTopoPurpose.Approved)

      End Select
   End Sub



   Private Sub mfReports_Recalculate(iRegion As Integer) Handles mfReports.Recalculate
      zzCalculateTabaProject(iRegion)
   End Sub


  

   Private Sub smiUnidiv_General_Click(sender As Object, e As EventArgs) Handles smiUnidiv_General.Click
      zzOpenUD_General()
   End Sub

 
 
  
End Class