Option Explicit On
Option Strict On
Public Class frmTopoActions
   Private Const msActionIDFldName As String = "ActionID"
   Private Const msToleranceFldName As String = "Tolerance"
   Private Const msErrorsFldName As String = "Errors"

   Private miActionDflt As Integer = 0
   Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmActions
   Private Const miToposUB As Integer = 4
   Private moaLabels(miToposUB) As LabelInd
   Private miaTopoIDs(miToposUB) As Integer
   Private moaTopoDefs(miToposUB) As TopoDef
   Private miCurrentTopoID As Integer
   Private moCurrentLabel As LabelInd = Nothing
   Private moLabelColor As System.Drawing.Color = Color.DimGray
   Private moLabelSelectColor As System.Drawing.Color = Color.DarkBlue

   Private moLabelFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Private moLabelSelectFont As System.Drawing.Font = New System.Drawing.Font("Tahoma", 11.25!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
   Private moDataTable(miToposUB) As System.Data.DataTable
   Private moOleDbDataAdapter(miToposUB) As System.Data.OleDb.OleDbDataAdapter
   'new System.Data.DataTable(msTableName);
   Private dgvMain As DataGridView
   Private tbpReports As System.Windows.Forms.TabPage
   Private tabMain As TabControl
   Private WithEvents tbpTopo As System.Windows.Forms.TabPage
   Private WithEvents cmdFix As TabButton
   Private WithEvents cmdMark As TabButton
   Private WithEvents cmdBuild As TabButton
   Private WithEvents cmdKill As TabButton




   Public Sub New()

      ' This call is required by the Windows Form Designer.
      InitializeComponent()
      ' Add any initialization after the InitializeComponent() call.
      miaTopoIDs(0) = TPlanGraph.TopoPurpose.Parcel
      miaTopoIDs(1) = TPlanGraph.TopoPurpose.Approved
      miaTopoIDs(2) = TPlanGraph.TopoPurpose.Proposed
      miaTopoIDs(3) = TPlanGraph.TplnProject.GetUnionID(TPlanGraph.TopoPurpose.Parcel, TPlanGraph.TopoPurpose.Approved)
      miaTopoIDs(4) = TPlanGraph.TplnProject.GetUnionID(TPlanGraph.TopoPurpose.Parcel, TPlanGraph.TopoPurpose.Proposed)
      zzMyInitializeComponent()
   End Sub

   Private Sub lblTopo_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
      '  System.Windows.Forms.MessageBox.Show(oSender.ToString())
      Try
         Dim oLabelInd As LabelInd
         oLabelInd = DirectCast(oSender, LabelInd)
         If moCurrentLabel IsNot Nothing AndAlso moCurrentLabel IsNot oLabelInd Then
            moCurrentLabel.Font = moLabelFont
            moCurrentLabel.ForeColor = moLabelColor
         End If
         moCurrentLabel = oLabelInd
         oLabelInd.Font = moLabelSelectFont
         oLabelInd.ForeColor = moLabelSelectColor
         miCurrentTopoID = miaTopoIDs(oLabelInd.Index)
         Me.dgvMain.Columns.Clear()
         zzLoadData(oLabelInd.Index)
         zzSetGridColumns()
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "lblTopo_Click")
      End Try

   End Sub
   Private Sub TopoActions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

   End Sub

   Private Sub dgvMain_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)

   End Sub
   Private Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
      Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID)

   End Function
   Private Sub zzMyInitializeComponent()
      Me.tbpTopo = New System.Windows.Forms.TabPage
      Me.tbpReports = New System.Windows.Forms.TabPage

      Me.cmdFix = New TabButton(moLabelFont)
      Me.cmdMark = New TabButton(moLabelFont)
      Me.cmdBuild = New TabButton(moLabelFont)
      Me.cmdKill = New TabButton(moLabelFont)
      Me.tabMain = New System.Windows.Forms.TabControl

      Me.dgvMain = New System.Windows.Forms.DataGridView
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      Me.tabMain.Controls.Add(Me.tbpTopo)
      Me.tabMain.Controls.Add(Me.tbpReports)
      '
      'tabMain
      '
      Me.tabMain.Location = New System.Drawing.Point(0, 0)
      Me.tabMain.Name = "tabMain"
      Me.tabMain.SelectedIndex = 0
      Me.tabMain.Size = New System.Drawing.Size(549, 300)
      Me.tabMain.TabIndex = 7

      '
      'dgvMain
      '
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Location = New System.Drawing.Point(4, 4)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.dgvMain.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
      Me.dgvMain.RowHeadersWidth = 24
      Me.dgvMain.RowHeadersVisible = True
      Me.dgvMain.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing

      Me.dgvMain.Size = New System.Drawing.Size(280, 264)
      Me.dgvMain.TabIndex = 5
      Me.dgvMain.AllowUserToOrderColumns = False
      Me.dgvMain.AutoGenerateColumns = False

      Me.tbpTopo.Controls.Add(Me.dgvMain)
      '
      'tbpTopo
      '
      Me.tbpTopo.Location = New System.Drawing.Point(4, 22)
      Me.tbpTopo.Name = "tbpTopo"
      Me.tbpTopo.Padding = New System.Windows.Forms.Padding(3)
      Me.tbpTopo.Size = New System.Drawing.Size(576, 304)
      Me.tbpTopo.TabIndex = 8
      Me.tbpTopo.Text = "Topology"
      Me.tbpTopo.UseVisualStyleBackColor = True
      '
      'tbpReports
      '
      Me.tbpReports.Location = New System.Drawing.Point(4, 22)
      Me.tbpReports.Name = "tbpReports"
      Me.tbpReports.Padding = New System.Windows.Forms.Padding(3)
      Me.tbpReports.Size = New System.Drawing.Size(576, 304)
      Me.tbpReports.TabIndex = 6
      Me.tbpReports.Text = "Reports"
      Me.tbpReports.UseVisualStyleBackColor = True

      '
      'cmdFix
      '
      With Me.cmdFix
         .Location = New System.Drawing.Point(288, 4)
         .Name = "cmdFix"
         '     .Size = New System.Drawing.Size(48, 24)
         .TabIndex = 9
         '   .Font = moLabelFont
         .Text = "Fix"
      End With

      '
      'cmdMark
      '
      With Me.cmdMark
         .Location = New System.Drawing.Point(356, 4)
         .Name = "cmdMark"
         '     .Size = New System.Drawing.Size(48, 24)
         .TabIndex = 10
         '     .Font = moLabelFont
         .Text = "Mark"

      End With
      '
      'cmdBuild
      '
      With Me.cmdBuild
         .Location = New System.Drawing.Point(412, 4)
         .Name = "cmdBuild"
         '   .Size = New System.Drawing.Size(48, 24)
         .TabIndex = 11
         '   .Font = moLabelFont
         .Text = "Build"
      End With

      '
      'cmdKill
      '
      With Me.cmdKill
         .Location = New System.Drawing.Point(468, 4)
         .Name = "cmdKill"
         '   .Size = New System.Drawing.Size(48, 24)
         .TabIndex = 12
         '    .Font = moLabelFont
         .Text = "Kill"
      End With
      Me.Controls.Add(Me.tabMain)
      Me.tbpTopo.Controls.Add(Me.cmdFix)
      Me.tbpTopo.Controls.Add(Me.cmdMark)
      Me.tbpTopo.Controls.Add(Me.cmdBuild)
      Me.tbpTopo.Controls.Add(Me.cmdKill)

      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)

      For iIndex As Integer = 0 To miToposUB
         Me.moaLabels(iIndex) = New LabelInd(iIndex)
         Me.moaLabels(iIndex).AutoSize = False
         Me.moaLabels(iIndex).Cursor = System.Windows.Forms.Cursors.Hand
         If iIndex = miActionDflt Then
            Me.moaLabels(iIndex).Font = moLabelSelectFont
            Me.moaLabels(iIndex).ForeColor = moLabelSelectColor
         Else
            Me.moaLabels(iIndex).Font = moLabelFont
            Me.moaLabels(iIndex).ForeColor = moLabelColor
         End If
         Me.moaLabels(iIndex).Location = New System.Drawing.Point(288, 80 + 24 * iIndex)
         Me.moaLabels(iIndex).RightToLeft = Windows.Forms.RightToLeft.Yes
         Me.moaLabels(iIndex).Name = "lblTopo" & CStr(iIndex)
         Me.moaLabels(iIndex).Size = New System.Drawing.Size(200, 18)
         Me.moaLabels(iIndex).TabIndex = iIndex
         Me.moaLabels(iIndex).Text = zzGetText(iIndex, 1)
         Me.moaLabels(iIndex).FlatStyle = FlatStyle.Popup
         Me.moaLabels(iIndex).TextAlign = ContentAlignment.MiddleLeft
         Me.moaLabels(iIndex).BorderStyle = BorderStyle.None
         Me.tbpTopo.Controls.Add(Me.moaLabels(iIndex))
         AddHandler moaLabels(iIndex).Click, AddressOf lblTopo_Click
      Next
      miCurrentTopoID = miaTopoIDs(miActionDflt)
      zzLoadData(miActionDflt)
      zzSetGridColumns()
      moCurrentLabel = Me.moaLabels(miActionDflt)
   End Sub


   Private Function zzSetGridColumns() As Windows.Forms.DataGridViewComboBoxColumn
      Dim oCmbColumn As New Windows.Forms.DataGridViewComboBoxColumn()
      Dim oColumn As DataGridViewTextBoxColumn

      Try
         With oCmbColumn
            .Name = msActionIDFldName
            '   System.Windows.Forms.MessageBox.Show("30", "!!!!!1")
            .DataPropertyName = "ActionID"
            '   System.Windows.Forms.MessageBox.Show("40", "!!!!!1")
            .HeaderText = "Actions"
            '    System.Windows.Forms.MessageBox.Show("50", "!!!!!1")
            .Width = 140
            '     System.Windows.Forms.MessageBox.Show("60", "!!!!!1")
            .Items.Clear()
            '   System.Windows.Forms.MessageBox.Show("70", "!!!!!1")
            .FlatStyle = FlatStyle.Standard
            '   System.Windows.Forms.MessageBox.Show(CStr(.Items.Count), "!!!!!BEFORE")
            If TPlanGraph.TplnProject.TopoIsUnion(miCurrentTopoID) Then
               .Items.AddRange(TopoCreator.CleanupActionItemsForUnion())
               ' .Items.AddRange(TopoCreator.CleanupActionItems())
            Else
               ' .Items.AddRange(TopoCreator.CleanupActionItemsForUnion())
               .Items.AddRange(TopoCreator.CleanupActionItems())
            End If
            '   System.Windows.Forms.MessageBox.Show(CStr(.Items.Count), "!!!!!")
            .MaxDropDownItems = .Items.Count
            .ValueMember = DMObjects.ItemData.ValueMember
            .DisplayMember = DMObjects.ItemData.DisplayMember
            '.ReadOnly = TPlanGraph.TplnProject.TopoIsUnion(miCurrentTopoID)
         End With
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "zzSetGridColumns-12")
      End Try

      Try
         Me.dgvMain.Columns.Add(oCmbColumn)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "zzSetGridColumns-10")
      End Try

      oColumn = New Windows.Forms.DataGridViewTextBoxColumn()
      With oColumn
         .Name = msToleranceFldName
         .HeaderText = "Tolerance"
         .Width = 58
         .Name = "Tolerance"
         .DataPropertyName = "Tolerance"
      End With

      Try
         Me.dgvMain.Columns.Add(oColumn)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "zzSetGridColumns-2")
      End Try

      oColumn = New Windows.Forms.DataGridViewTextBoxColumn()
      With oColumn
         .HeaderText = "Errors"
         .Width = 38
         .Name = msErrorsFldName
         .ReadOnly = True
         .DataPropertyName = msErrorsFldName
      End With
      Try
         Me.dgvMain.Columns.Add(oColumn)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "zzSetGridColumns")
      End Try
      Return oCmbColumn
   End Function
   Private Sub zzLoadData(ByVal iTopoTypeIndex As Integer)

      If moDataTable(iTopoTypeIndex) Is Nothing Then
         moDataTable(iTopoTypeIndex) = New DataTable("TopoType" & CStr(miCurrentTopoID))
         Dim sSelectComText As String = "SELECT TopologyType,ActionNo,ActionID,Tolerance FROM(CleanupActions) WHERE(CleanupActions.TopologyType=" & CStr(miCurrentTopoID) & ") ORDER BY CleanupActions.ActionNo"
         Dim sUpdateComText As String = "UPDATE Localities SET Actual = ? WHERE ID = ?"
         moOleDbDataAdapter(iTopoTypeIndex) = TPlServerDB.ServerDB.GetDataAdapter(sSelectComText, True)
         moOleDbDataAdapter(iTopoTypeIndex).Fill(moDataTable(iTopoTypeIndex))
         moDataTable(iTopoTypeIndex).Columns.Add(msErrorsFldName, System.Type.GetType("System.Int32"))
      End If

      Me.dgvMain.DataSource = moDataTable(iTopoTypeIndex)



   End Sub



   Private Class LabelInd
      Inherits System.Windows.Forms.Label
      Private miIndex As Integer
      Public Sub New(ByVal iIndex As Integer)
         miIndex = iIndex
      End Sub
      Public Property Index() As Integer
         Get
            Return miIndex
         End Get
         Set(ByVal iValue As Integer)
            miIndex = iValue
         End Set
      End Property
   End Class


   Private Sub zzCleanup(ByVal bFix As Boolean)
      Dim iActionUB As Integer = Me.dgvMain.RowCount - 2
      Dim iActionID As Integer
      Dim dTolerance As Double
      Dim oaActionVar(iActionUB) As ActionVar
      Dim iaErrors() As Integer
      Dim sTest As String = "a"
      Try
         System.Windows.Forms.MessageBox.Show(CStr(iActionUB), "BeforeCleanup")
         For iIndex As Integer = 0 To iActionUB
            If Not Me.dgvMain.Rows.Item(iIndex).IsNewRow Then
               iActionID = DirectCast(Me.dgvMain.Rows.Item(iIndex).Cells(msActionIDFldName).Value, Integer)
               sTest = "b"
               dTolerance = CDblN(Me.dgvMain.Rows.Item(iIndex).Cells(msToleranceFldName).Value)
               sTest = "c"
               oaActionVar(iIndex) = New ActionVar(iActionID, dTolerance)
               sTest = "d"
            End If
         Next
         sTest = "e"
         If iActionUB >= 0 Then
            sTest = "ea"
            Dim oTopoDef As TopoDef = zzLoadTopoDef()
            sTest = "eb"
            If oTopoDef IsNot Nothing Then
               sTest = "ec"
               iaErrors = TopoCreator.Cleanup(oaActionVar, oTopoDef, bFix)
               sTest = "ed"
               Dim oDataTable As DataTable = moDataTable(moCurrentLabel.Index)
               sTest = "ee"
               Dim oDataRow As DataRow
               System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count - 1), "AfterCleanup")
               For iIndex As Integer = 0 To iaErrors.GetUpperBound(0)
                  sTest = "ek" & CStr(iIndex)
                  oDataRow = oDataTable.Rows(iIndex)
                  sTest = "em" & CStr(iIndex)
                  If oDataRow.RowState <> DataRowState.Deleted Then
                     sTest = "en" & CStr(iIndex)
                     oDataRow.BeginEdit()
                     sTest = "ep" & CStr(iIndex)
                     oDataRow.Item(msErrorsFldName) = iaErrors(iIndex)
                     sTest = "eq" & CStr(iIndex)
                     oDataRow.EndEdit()
                     sTest = "er" & CStr(iIndex)
                  Else
                     System.Windows.Forms.MessageBox.Show("RowDeleted")
                  End If
                  sTest = "es" & CStr(iIndex)
               Next
            End If
            sTest = "ff"

            sTest = "g"
            ''  zzDispArray(iaErrors)
            'T   Me.dgvMain.BeginEdit(True)
            'T    For iIndex As Integer = 0 To iActionUB
            'T If Not Me.dgvMain.Rows.Item(iIndex).IsNewRow Then
            'T Me.dgvMain.Rows.Item(iIndex).Cells(msErrorsFldName).Value = iaErrors(iIndex)
            'T  Me.dgvMain.UpdateCellValue(2, iIndex)
            'T  End If
            'T      Next
            'T   Me.dgvMain.EndEdit()

         End If

      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "zzCleanup")
      End Try
   End Sub
   Private Sub zzDispArray(ByVal iaVal() As Integer)
      Dim sOut As String = String.Empty
      For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
         sOut += ":" & iaVal(iIndex).ToString()
      Next
      System.Windows.Forms.MessageBox.Show(sOut, "Errors")
   End Sub
   Private Sub zzCreateTopo()

      Dim oTopoDef As TopoDef = zzLoadTopoDef()
      If oTopoDef IsNot Nothing Then
         TopoCreator.CreateTopology(oTopoDef)
      End If


   End Sub

   Private Function zzLoadTopoDef() As TopoDef
      Dim iTopoTypeIndex As Integer
      iTopoTypeIndex = moCurrentLabel.Index
      If moaTopoDefs(iTopoTypeIndex) Is Nothing Then
         moaTopoDefs(iTopoTypeIndex) = New TopoDef(CType(miaTopoIDs(iTopoTypeIndex), TPlanGraph.TopoPurpose))
      End If
      Return moaTopoDefs(iTopoTypeIndex)
   End Function
   Private Sub zzDeleteTopo()
      Dim iTopoTypeIndex As Integer
      iTopoTypeIndex = moCurrentLabel.Index
      If moaTopoDefs(iTopoTypeIndex) Is Nothing Then
         moaTopoDefs(iTopoTypeIndex) = New TopoDef(CType(miaTopoIDs(iTopoTypeIndex), TPlanGraph.TopoPurpose))
         TopoCreator.DeleteTopology(moaTopoDefs(iTopoTypeIndex))
      End If
   End Sub
   Private Sub zzActualize()
      For iIndex As Integer = 0 To miToposUB
         If moaTopoDefs(iIndex).LinkLayersExists Then
            DWGInfo.AddLayer(iIndex, moaTopoDefs(iIndex).LinkLayers)
         End If
      Next
      DWGInfo.Open()
      Dim iLayerIndex() As Integer = Nothing, bHasEntity() As Boolean = Nothing
      DWGInfo.LayerStatus(iLayerIndex, bHasEntity)
      For iIndex As Integer = 0 To iLayerIndex.GetUpperBound(0)
         moaTopoDefs(iLayerIndex(iIndex)).LinkLayerEmpty = Not bHasEntity(iIndex)
      Next
   End Sub
   Private Sub cmdFix_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdFix.Click
      zzCleanup(True)
   End Sub

   Private Sub cmdMark_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdMark.Click
      zzCleanup(False)
   End Sub
   Private Class TabButton
      Inherits Button
      Public Sub New(ByVal oFont As Font)
         MyBase.Size = New System.Drawing.Size(44, 24)
         MyBase.Font = oFont
         MyBase.Cursor = Cursors.Hand
         MyBase.FlatStyle = Windows.Forms.FlatStyle.Standard
         MyBase.UseVisualStyleBackColor = True
      End Sub
   End Class

   Private Sub cmdBuild_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdBuild.Click
      zzCreateTopo()
   End Sub

   Private Sub cmdKill_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdKill.Click
      zzDeleteTopo()
   End Sub

 
End Class