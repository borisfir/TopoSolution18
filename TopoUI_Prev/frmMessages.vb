Option Explicit On
Option Strict On
Imports System.Data
Public Class frmMessages
   Private Const msToleranceFldName As String = "Tolerance"
   Private Const miCommandsTop As Integer = 8

	Private WithEvents dgvMessages As DataGridView
	Private txtMessageText As TextBox

   Private miGridTop As Integer = -1
   Private miClientHeight As Integer = -1
   Private miClientHeightDflt As Integer = -1
   Private miThemeID As Integer
   Private miActionsUB As Integer

   Private WithEvents chkWrapText As System.Windows.Forms.CheckBox = New System.Windows.Forms.CheckBox()
   Private WithEvents cmdTableMsg As TabButton
   Private WithEvents cmdZoomMsg As TabButton
   Private WithEvents cmdZoomDown As TabButton

   Private WithEvents cmdClearMsg As TabButton
   Private WithEvents cmdExit As TabButton
   Private WithEvents ctxText As System.Windows.Forms.DataGridViewTextBoxColumn

   Private moWrapCellStyle As System.Windows.Forms.DataGridViewCellStyle

   Public Event DisplayBasePgon(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer)


   Public Sub New(Optional iThemeID As Integer = 0, Optional iActionsUB As Integer = -1)
      miThemeID = iThemeID
      miActionsUB = iActionsUB
      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()
   End Sub
   Private Sub zzMyInitializeComponent()
      zzInitTabMessages()
      moWrapCellStyle = Me.dgvMessages.DefaultCellStyle.Clone
      moWrapCellStyle.WrapMode = DataGridViewTriState.True
   End Sub
   Protected Overridable Sub zzInitTabMessages()
      Me.dgvMessages = New System.Windows.Forms.DataGridView
      Me.txtMessageText = New System.Windows.Forms.TextBox
      '
      'dgvMessages
      '
      With Me.dgvMessages
         .ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
         Me.dgvMessages.Dock = System.Windows.Forms.DockStyle.Bottom
         .Location = New System.Drawing.Point(2, 2)
         .Name = "dgvMessages"
         .Font = frmPrjThemes.moLabelFont
         .RightToLeft = System.Windows.Forms.RightToLeft.Yes
         .RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
         .RowHeadersWidth = 23
         .RowHeadersVisible = True
         .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
         .RowTemplate.Height = 20
         .ColumnHeadersDefaultCellStyle.Font = frmPrjThemes.moBaseFont
         '  .Size = New System.Drawing.Size(316, 236)
         .Size = New System.Drawing.Size(530, 664)  '224
         .TabIndex = 5
         .AllowUserToOrderColumns = False
         .AutoGenerateColumns = False
         .AllowUserToAddRows = False
         .AllowUserToDeleteRows = True
      End With
      Me.Controls.Add(Me.dgvMessages)
      zzSetMessagesGridColumns()
      Dim sFilter As String
      Dim sSort As String
      If miThemeID <> 0 Then
         sFilter = "MapThemeID=" & Convert.ToString(miThemeID)
         sSort = "ActionID"
         Me.dgvMessages.DataSource = New DataView(DMAcadExt.AppMessages.MsgTable, sFilter, sSort, DataViewRowState.CurrentRows)
      Else
         Me.dgvMessages.DataSource = DMAcadExt.AppMessages.MsgTable
      End If

      Me.dgvMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both


      '
      'txtMessageText
      '
      With txtMessageText
         Me.txtMessageText.Dock = System.Windows.Forms.DockStyle.Bottom
         .Location = New System.Drawing.Point(2, 186)
         .Name = "txtMessageText"
         .Size = New System.Drawing.Size(530, 48)
         .TabIndex = 45
         .Multiline = True
         .RightToLeft = Windows.Forms.RightToLeft.Yes
      End With
      Me.Controls.Add(Me.txtMessageText)

      Me.cmdTableMsg = New TabButton(frmPrjThemes.moLabelFont, False)
      '
      'cmdTableMsg
      '
      With Me.cmdTableMsg
         .Location = New System.Drawing.Point(40, miCommandsTop)
         .Image = Global.TopoUI.My.Resources.Resources.Table19x17
         .TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
         .Name = "cmdTableMsg"
         '	.Size = New System.Drawing.Size(88, 24)
         .TabIndex = 12
         .Text = "Table"
         .AutoSize = True
      End With
      Me.Controls.Add(Me.cmdTableMsg)

      Me.cmdZoomMsg = New TabButton(frmPrjThemes.moLabelFont, False)
      '
      'cmdZoomMsg
      '
      With Me.cmdZoomMsg
         '  .Image = CType(Resources.GetObject("tbbZoomPgon.Image"), System.Drawing.Image)
         '  .Image = CType(Global.TopoUI.My.Resources.Resources.tbbZoomPgon, System.Drawing.Image)
         .Location = New System.Drawing.Point(100, miCommandsTop)
         '  .Image = Global.TopoUI.My.Resources.Resources.2392_ZoomIn_16x16
         .TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage
         .Name = "cmdZoomMsg"
         '	.Size = New System.Drawing.Size(88, 24)
         .TabIndex = 12
         .Text = "Zoom"
         .AutoSize = True

      End With
      Me.Controls.Add(Me.cmdZoomMsg)

      Me.cmdZoomDown = New TabButton(frmPrjThemes.moLabelFont, False)

      '
      'cmdZoomDown
      '
      With cmdZoomDown
         .AutoSize = True
         .Image = Global.TopoUI.My.Resources.Resources.DownTr
         .Location = New System.Drawing.Point(160, miCommandsTop)
         .Name = "Button1"
         .Size = New System.Drawing.Size(24, 38)
         .TabIndex = 0
         .UseMnemonic = False
         .UseVisualStyleBackColor = False
      End With
      Me.Controls.Add(Me.cmdZoomDown)

      Me.cmdClearMsg = New TabButton(frmPrjThemes.moLabelFont, False)
      '
      'cmdClearMsg
      '
      With Me.cmdClearMsg
         .Location = New System.Drawing.Point(160, miCommandsTop)
         .Name = "cmdClearMsg"
         '	.Size = New System.Drawing.Size(88, 24)
         .TabIndex = 13
         .Text = "Clear"
      End With
      Me.Controls.Add(Me.cmdClearMsg)

      Me.cmdExit = New TabButton(frmPrjThemes.moLabelFont, False)

      '
      'cmdExit
      '
      With Me.cmdExit
         .Location = New System.Drawing.Point(220, miCommandsTop)
         .Name = "cmdExit"
         '	.Size = New System.Drawing.Size(88, 24)
         .TabIndex = 13
         .Text = "Exit"
      End With
      Me.Controls.Add(Me.cmdExit)

      '
      'chkWrapText
      '
      With Me.chkWrapText
         .AutoSize = True
         .FlatStyle = System.Windows.Forms.FlatStyle.System
         .Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
         .ImeMode = System.Windows.Forms.ImeMode.NoControl
         .Location = New System.Drawing.Point(260, miCommandsTop)
         .Name = "chkWrapText"
         .Size = New System.Drawing.Size(62, 18)
         .TabIndex = 238
         .Text = "Wrap"
         .UseVisualStyleBackColor = True

      End With
      Me.Controls.Add(Me.chkWrapText)
   End Sub
   Private Function zzGetActionItems() As DMCommon.ItemData()
      Dim oPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray()
      Dim sComText As String = "SELECT TOP (100) PERCENT ActionID,ActionName FROM dbo.CheckActionsExt WHERE (CheckType = 1) AND (MapThemeID = " & Convert.ToString(miThemeID) & ") ORDER BY ActionOrder"
      '      sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)

      Dim oaDMObjects(miActionsUB) As DMCommon.ItemData
      Dim iIndex As Integer
      If oDataReader IsNot Nothing Then
         Do While oDataReader.Read
            oaDMObjects(iIndex) = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
            iIndex += 1
         Loop
         oDataReader.Close()

         '	MessageBox.Show("", "10_002")



         oDataReader.Close()
      End If

      Return oaDMObjects

   End Function
	Private Sub zzSetMessagesGridColumns()	'''''As System.Windows.Forms.DataGridViewComboBoxColumn
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
      Dim oTxtColumn As DataGridViewTextBoxColumn
      If miThemeID <> 0 Then
         Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
         Try
            With oCmbColumn
               .Name = "Action"
               .DataPropertyName = "ActionID"
               .HeaderText = "Actions"
               .Width = 180
               .Items.Clear()
               .FlatStyle = FlatStyle.Standard
               '	If miCurrentTopoDefID.TopoIsMerge Then
               '.Items.AddRange(TopoManager.TopoCreator.CleanupActionItemsForMerge())
               'Else
               .Items.AddRange(zzGetActionItems())
               '	End If
               .MaxDropDownItems = .Items.Count
               .ValueMember = DMCommon.ItemData.ValueMember
               .DisplayMember = DMCommon.ItemData.DisplayMember
               .SortMode = DataGridViewColumnSortMode.NotSortable
               .ReadOnly = True
            End With
         Catch oEx As Exception
            DMCommon.Functions.ShowEx(oEx, Me.Name)
         End Try

         Try
            Me.dgvMessages.Columns.Add(oCmbColumn)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
         End Try
      End If
      Me.ctxText = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With Me.ctxText
         .HeaderText = "Text"
         .Width = 420
         .Name = "ctxText"
         .ReadOnly = True
         .DataPropertyName = "Text"
         .SortMode = DataGridViewColumnSortMode.NotSortable
      End With
      Try
         Me.dgvMessages.Columns.Add(Me.ctxText)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_1")
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = msToleranceFldName
         .HeaderText = "X"
         .Width = 70
         .Name = "X"
         .DataPropertyName = "X"
         .SortMode = DataGridViewColumnSortMode.NotSortable
         .Visible = False
      End With

      Try
         Me.dgvMessages.Columns.Add(oTxtColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = msToleranceFldName
         .HeaderText = "Y"
         .Width = 70
         .Name = "Y"
         .DataPropertyName = "Y"
         .SortMode = DataGridViewColumnSortMode.NotSortable
         .Visible = False
      End With

      Try
         Me.dgvMessages.Columns.Add(oTxtColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = "ThemeID"
         .HeaderText = "Theme"
         .Width = 70

         .DataPropertyName = "MapThemeID"
         .SortMode = DataGridViewColumnSortMode.NotSortable
         .Visible = False
      End With

      Try
         Me.dgvMessages.Columns.Add(oTxtColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
      End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = "ActionID"
         .HeaderText = "Action"
         .Width = 70

         .DataPropertyName = "ActionID"
         .SortMode = DataGridViewColumnSortMode.NotSortable
         .Visible = False
      End With

      Try
         Me.dgvMessages.Columns.Add(oTxtColumn)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
      End Try
     
	End Sub
	Private Sub dgvMessages_RowEnter(ByVal oSender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMessages.RowEnter
		If e.RowIndex >= 0 Then
			Dim oDatarow As DataRow = DMAcadExt.AppMessages.MsgTable.Rows.Item(e.RowIndex)
			Me.txtMessageText.Text = DMCommon.Functions.CStrN(oDatarow.Item("Text"))
		Else
			Me.txtMessageText.Text = String.Empty
		End If


	End Sub
	Private Sub cmdTableMsg_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdTableMsg.Click
		Dim oDataRowView As DataRowView
		Dim oDataRow As DataRow
		Try

			Dim oViewRow As DataGridViewRow = Me.dgvMessages.CurrentRow
			Dim sSysID As String
			If oViewRow IsNot Nothing Then
				oDataRowView = DirectCast(oViewRow.DataBoundItem, DataRowView)
				oDataRow = oDataRowView.Row
				sSysID = DMCommon.Functions.CStrN(oDataRow.Item("SysID"))
				Dim saSysID() As String = Split(sSysID, ",")
				Dim iIndex As Integer

				Dim iOverlayIndex As DMAcadExt.enOverlayIndex
				Dim iTopoID As Integer
				Dim bParcel As Boolean
				Select Case saSysID(0)
					Case "DVA"
						'	System.Windows.Forms.MessageBox.Show(sSysID & vbCrLf & CStr(saSysID.GetUpperBound(0)), "04_567")
						If saSysID.GetUpperBound(0) >= 3 Then
							If IsNumeric(saSysID(1)) Then
								iIndex = Convert.ToInt32(saSysID(1))
								If IsNumeric(saSysID(2)) Then
									bParcel = (saSysID(2) = "1")
									If IsNumeric(saSysID(3)) Then
										iTopoID = Convert.ToInt32(saSysID(3))
										If [Enum].IsDefined(GetType(DMAcadExt.enOverlayIndex), iIndex) Then
											iOverlayIndex = CType(iIndex, DMAcadExt.enOverlayIndex)
											'			System.Windows.Forms.MessageBox.Show(CStr(iOverlayIndex) & vbCrLf & iOverlayIndex.ToString() & vbCrLf & CStr(bParcel) & vbCrLf & CStr(iTopoID), "04_569")
											RaiseEvent DisplayBasePgon(iOverlayIndex, bParcel, iTopoID)
										End If
									End If
								End If

							End If
						End If

					Case Else

				End Select
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdTableMsg_Click")
		End Try
	End Sub

	Private Sub cmdZoomMsg_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdZoomMsg.Click
		Dim oDataRowView As DataRowView
		Dim oDataRow As DataRow
		Try
			Dim oViewRow As DataGridViewRow = Me.dgvMessages.CurrentRow
			If oViewRow IsNot Nothing Then
				oDataRowView = DirectCast(oViewRow.DataBoundItem, DataRowView)
				oDataRow = oDataRowView.Row
				DMAcadExt.AppMessages.Zoom(oDataRow)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdZoomMsg_Click")
		End Try
	End Sub
	Private Sub cmdClearMsg_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdClearMsg.Click
		Try
         DMAcadExt.AppMessages.ClearAll()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdClearMsg_Click")
		End Try
   End Sub

   Private Sub frmMessages_Load(sender As Object, e As EventArgs) Handles Me.Load

      If DMCommon.dmScreen.MyResolution <> DMCommon.enScreenResolutions.res1920x1080 Then
         Me.AutoScroll = True
      End If
      Me.ctxText.DefaultCellStyle = moWrapCellStyle
      '   zzResizeRows()
     

   End Sub

   Private Sub frmMessages_QueryContinueDrag(sender As Object, e As QueryContinueDragEventArgs) Handles Me.QueryContinueDrag

   End Sub
 
 

   Private Sub frmMessages_Resize(sender As Object, e As EventArgs) Handles Me.Resize
      If miClientHeightDflt = -1 Then
         miClientHeightDflt = Me.ClientSize.Height
      End If

      If Me.dgvMessages IsNot Nothing Then
         If miGridTop = -1 Then
            miGridTop = Me.dgvMessages.Top
         End If
         If Me.AutoScroll Then
            Me.dgvMessages.Height = Me.ClientSize.Height - Me.txtMessageText.Height - miGridTop '+ Me.Top
            Me.dgvMessages.Top = miGridTop
         Else
            Me.dgvMessages.Height = Me.ClientSize.Height - Me.txtMessageText.Height - miGridTop
         End If
         If Not Me.AutoScroll AndAlso Me.WindowState = FormWindowState.Maximized Then
            miClientHeight = Me.ClientSize.Height
         End If

      End If

   End Sub

   Private Sub frmMessages_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown

      '	MessageBox.Show(CStr(Me.Location.X) & ":" & CStr(Me.Location.Y), "01_056")
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
      Me.ClientSize = New Size(Me.ClientSize.Width, miClientHeightDflt)
   End Sub

   Private Sub cmdExit_Click(sender As Object, e As EventArgs) Handles cmdExit.Click
      Me.Close()
   End Sub

   Private Sub chkWrapText_CheckedChanged(sender As Object, e As EventArgs) Handles chkWrapText.CheckedChanged
      If Me.chkWrapText.Checked Then
         Me.ctxText.DefaultCellStyle = moWrapCellStyle
      Else
         Me.ctxText.DefaultCellStyle = Nothing
      End If
      zzResizeRows()
   End Sub
   Private Sub zzResizeRows()
      Try
         dgvMessages.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders)
      Catch oEx As Exception

      End Try
   End Sub

   Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

   End Sub
End Class