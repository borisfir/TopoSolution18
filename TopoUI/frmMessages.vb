Option Explicit On
Option Strict On
Imports System.Data
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmMessages
   Private Const msToleranceFldName As String = "Tolerance"
   Private Const miCommandsTop As Integer = 8
   Private Const miCommandsDeltaWidth As Integer = 2


	Private WithEvents dgvMessages As DataGridView
	Private txtMessageText As TextBox

   Private miGridTop As Integer = -1
   Private miClientHeight As Integer = -1
   Private miClientHeightDflt As Integer = -1
	Private miMapThemeID As Integer
	Private miActionsUB As Integer
	Private moCurrentDataView As DataView
	Private mcolActions As ObjectModel.Collection(Of DMAcadExt.AppMessages.Action) = New ObjectModel.Collection(Of DMAcadExt.AppMessages.Action)
	Private mdicActions As IDictionary(Of Integer, DMAcadExt.AppMessages.Action)
	Private WithEvents chkWrapText As System.Windows.Forms.CheckBox = New System.Windows.Forms.CheckBox()
	Private WithEvents cmdTableMsg As TabButton
	Private WithEvents cmdZoomMsg As TabButton
	Private WithEvents cmdZoomDown As TabButton
	Private WithEvents cmdZoomUp As TabButton


	Private WithEvents cmdClearMsg As TabButton
	Private WithEvents cmdExit As TabButton
	Private WithEvents ctxText As System.Windows.Forms.DataGridViewTextBoxColumn
	Private WithEvents ctxMapThemeName As System.Windows.Forms.DataGridViewTextBoxColumn
	Private WithEvents ctxActionName As System.Windows.Forms.DataGridViewTextBoxColumn



	Private moWrapCellStyle As System.Windows.Forms.DataGridViewCellStyle
	Private mcolMarkedEntities As ObjectIdCollection = New ObjectIdCollection()
	Public Event DisplayBasePgon(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer)


	Public Sub New(Optional iThemeID As Integer = 0, Optional iActionsUB As Integer = -1)
		miMapThemeID = iThemeID
		miActionsUB = iActionsUB
		' This call is required by the designer.
		InitializeComponent()
		' DMCommon.Debug.MsgBox("12_482", iThemeID, iActionsUB)
		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub
	Private Sub zzMyInitializeComponent()
		zzInitTabMessages()
		moWrapCellStyle = Me.dgvMessages.DefaultCellStyle.Clone
		moWrapCellStyle.WrapMode = DataGridViewTriState.True
		mcolActions = DMAcadExt.AppMessages.Actions
		mdicActions = DMAcadExt.AppMessages.ActionDictionary

		'	DMCommon.Debug.MsgBox("101120_2", DMCommon.Debug.ColCount(mcolActions), DMAcadExt.AppMessages.Actions.Count)
		Me.cmbMapThemes.Items.Add(New DMCommon.ItemData(0, "כל הנושאים"))
		For Each oMapThemeData As DMAcadExt.MapThemeData In DMAcadExt.AppMessages.MapThemes.Values
			Me.cmbMapThemes.Items.Add(New DMCommon.ItemData(oMapThemeData.MapThemeID, oMapThemeData.MapThemeName))
		Next
		Me.cmbMapThemes.SelectedIndex = 0
		Me.cmbActions.ValueMember = DMCommon.ItemData.ValueMember
		Me.cmbActions.DisplayMember = DMCommon.ItemData.DisplayMember

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
		'   Dim oTestRow As DataRowView

		'   DMCommon.Debug.MsgBox("12_284", miThemeID)
		If miMapThemeID <> 0 Then
			sFilter = "MapThemeID=" & Convert.ToString(miMapThemeID)
			sSort = "ActionID"

			moCurrentDataView = New DataView(DMAcadExt.AppMessages.MsgTable, sFilter, sSort, DataViewRowState.CurrentRows)




			Me.dgvMessages.DataSource = moCurrentDataView

		Else
			moCurrentDataView = DMAcadExt.AppMessages.MsgTable.DefaultView
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
			.BorderStyle = BorderStyle.FixedSingle
		End With
		Me.Controls.Add(Me.txtMessageText)

		Me.cmdTableMsg = New TabButton(frmPrjThemes.moLabelFont, False)
		'
		'cmdTableMsg
		'
		With Me.cmdTableMsg
			.AutoSize = True
			.Location = New System.Drawing.Point(12, miCommandsTop)
			.Image = Global.TopoUI.My.Resources.Resources.Table19x17
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
			.Name = "cmdTableMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12
			.Text = "Table"

		End With
		Me.Controls.Add(Me.cmdTableMsg)
		'   DMCommon.Debug.MsgBox("12_480", Me.cmdTableMsg.Location, Me.cmdTableMsg.Left, Me.cmdTableMsg.Right, Me.cmdTableMsg.Width, Me.cmdTableMsg.RightToLeft)
		Me.cmdZoomMsg = New TabButton(frmPrjThemes.moLabelFont, False)
		'
		'cmdZoomMsg
		'
		With Me.cmdZoomMsg
			.AutoSize = True
			'.Image = CType(Resources.GetObject("tbbZoomPgon.Image"), System.Drawing.Image)
			'.Image = CType(Global.TopoUI.My.Resources.Resources.tbbZoomPgon, System.Drawing.Image)
			.ImageIndex = 2
			.ImageList = Me.imlMessages

			.Location = New System.Drawing.Point(Me.cmdTableMsg.Right + miCommandsDeltaWidth, miCommandsTop)
			'  .Image = Global.TopoUI.My.Resources.Resources.2392_ZoomIn_16x16
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
			.Name = "cmdZoomMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12
			.Text = "Zoom"


		End With
		Me.Controls.Add(Me.cmdZoomMsg)
		'    DMCommon.Debug.MsgBox("12_481", Me.cmdZoomMsg.Location, Me.cmdZoomMsg.Left, Me.cmdZoomMsg.Right, Me.cmdZoomMsg.Width, Me.cmdZoomMsg.RightToLeft)
		Me.cmdZoomDown = New TabButton(frmPrjThemes.moLabelFont, False)

		'
		'cmdZoomDown
		'
		With cmdZoomDown
			.AutoSize = True
			.ImageIndex = 1
			.ImageList = Me.imlMessages
			.Location = New System.Drawing.Point(Me.cmdZoomMsg.Right + miCommandsDeltaWidth, miCommandsTop)
			.Name = "cmdZoomDown"
			.Size = New System.Drawing.Size(24, 24)
			.TabIndex = 0
			.UseMnemonic = False
			.UseVisualStyleBackColor = False
		End With


		Me.Controls.Add(Me.cmdZoomDown)


		Me.cmdZoomUp = New TabButton(frmPrjThemes.moLabelFont, False)

		'
		'cmdZoomUp
		'
		With cmdZoomUp
			.AutoSize = True
			.ImageIndex = 0
			.ImageList = Me.imlMessages
			.Location = New System.Drawing.Point(Me.cmdZoomDown.Right + miCommandsDeltaWidth, miCommandsTop)
			.Name = "cmdZoomUp"
			.Size = New System.Drawing.Size(24, 24)
			.TabIndex = 0
			.UseMnemonic = False
			.UseVisualStyleBackColor = False
		End With


		Me.Controls.Add(Me.cmdZoomUp)

		Me.cmdClearMsg = New TabButton(frmPrjThemes.moLabelFont, False)
		'
		'cmdClearMsg
		'
		With Me.cmdClearMsg
			.AutoSize = True
			.ImageIndex = 3
			.ImageList = Me.imlMessages
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
			.Location = New System.Drawing.Point(Me.cmdZoomUp.Right + miCommandsDeltaWidth, miCommandsTop)
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
			.AutoSize = True
			.Location = New System.Drawing.Point(Me.cmdClearMsg.Right + miCommandsDeltaWidth, miCommandsTop)
			.ImageIndex = 4
			.ImageList = Me.imlMessages
			.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText

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
			.Location = New System.Drawing.Point(Me.cmdExit.Right + miCommandsDeltaWidth, miCommandsTop)
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
		Dim sComText As String = "SELECT TOP (100) PERCENT ActionID,ActionName FROM dbo.CheckActionsExt WHERE (CheckType = 1) AND (MapThemeID = " & Convert.ToString(miMapThemeID) & ") ORDER BY ActionOrder"
		'      sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)

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
	Private Sub zzSetActionList()
		If Me.cmbMapThemes.SelectedItem IsNot Nothing Then
			Dim oSelectedItem As DMCommon.ItemData = DirectCast(Me.cmbMapThemes.SelectedItem, DMCommon.ItemData)
			miMapThemeID = oSelectedItem.ListIndex
			Dim tActionGrouped As DMAcadExt.AppMessages.Action = New DMAcadExt.AppMessages.Action()
			Dim oaActions As IEnumerable(Of DMAcadExt.AppMessages.Action) = From tAction As DMAcadExt.AppMessages.Action In mcolActions
																								 Where (miMapThemeID = 0) OrElse (tAction.MapThemeID = miMapThemeID)
																								 Select tAction
			Dim oGrouped As IEnumerable(Of IGrouping(Of Integer, DMAcadExt.AppMessages.Action)) = oaActions.GroupBy(Function(fb) fb.ActionID)
			'''''''''''''''''''''''''''	Dim iCount As Integer = grouped.Count
			'	DMCommon.Debug.MsgBox("101120_1", DMCommon.Debug.ColCount(mcolActions), oaActions.Count, miMapThemeID)
			Me.cmbActions.Items.Clear()
			Me.cmbActions.Items.Add(New DMCommon.ItemData(0, "כל הבדיקות"))
			For Each oGrouping As IGrouping(Of Integer, DMAcadExt.AppMessages.Action) In oGrouped
				If mdicActions.TryGetValue(oGrouping.Key, tActionGrouped) Then
					Me.cmbActions.Items.Add(New DMCommon.ItemData(tActionGrouped.ActionID, tActionGrouped.ActionDescription))
				End If

			Next
			Me.cmbActions.SelectedIndex = 0
		End If

	End Sub
	Private Sub zzApplyFilter()

		Dim iActionID As Integer
		Dim sFilter As String = Nothing
		Dim sSort As String


		If Me.cmbActions.SelectedItem IsNot Nothing Then
			Dim oSelectedItem As DMCommon.ItemData = DirectCast(Me.cmbActions.SelectedItem, DMCommon.ItemData)
			iActionID = oSelectedItem.ListIndex
		End If
		sSort = "ActionID"
		If miMapThemeID <> 0 Then
			sFilter = "(MapThemeID=" & Convert.ToString(miMapThemeID) & ")"
		End If
		If iActionID <> 0 Then
			If Not String.IsNullOrEmpty(sFilter) Then
				sFilter &= " AND "
			End If
			sFilter = "(ActionID=" & Convert.ToString(iActionID) & ")"
		End If


		moCurrentDataView = New DataView(DMAcadExt.AppMessages.MsgTable, sFilter, sSort, DataViewRowState.CurrentRows)




		Me.dgvMessages.DataSource = moCurrentDataView


		'moCurrentDataView = DMAcadExt.AppMessages.MsgTable.DefaultView
		'Me.dgvMessages.DataSource = DMAcadExt.AppMessages.MsgTable



	End Sub
	Private Sub zzSetMessagesGridColumns() '''''As System.Windows.Forms.DataGridViewComboBoxColumn
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oTxtColumn As DataGridViewTextBoxColumn
		Dim oChkColumn As DataGridViewCheckBoxColumn

		Me.ctxMapThemeName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With Me.ctxMapThemeName
			.HeaderText = "נושא"
			.Width = 140
			.Name = "ctxThemeName"
			.ReadOnly = True
			.DataPropertyName = "MapThemeName"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvMessages.Columns.Add(Me.ctxMapThemeName)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_1")
		End Try
		Me.ctxActionName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With Me.ctxActionName
			.HeaderText = "שם בדיקה"
			.Width = 200
			.Name = "ctxActionName"
			.ReadOnly = True
			.DataPropertyName = "ActionName"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvMessages.Columns.Add(Me.ctxActionName)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_2")
		End Try
		If miMapThemeID >= 0 Then
			If False Then
				Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
				Try
					With oCmbColumn
						.Name = "ccbAction"
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
						.MaxDropDownItems = Math.Max(8, .Items.Count)
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
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_3")
				End Try
			End If
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
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_4")
		End Try
		oChkColumn = New DataGridViewCheckBoxColumn(False)
		With oChkColumn
			.HeaderText = "שג'?"
			.Width = 38
			.Name = "cchErr"
			.DataPropertyName = "Err"
			.SortMode = DataGridViewColumnSortMode.NotSortable
			.ReadOnly = True
			.Visible = True
		End With
		Try
			Me.dgvMessages.Columns.Add(oChkColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_7")
		End Try
		oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = "ctxX"
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
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_5")
		End Try

      oTxtColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
      With oTxtColumn
         .Name = "ctxY"
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
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_6")
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
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmMessages - zzSetGridColumns_8")
		End Try



	End Sub

   Private Sub dgvMessages_DataError(oSender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvMessages.DataError
		DMCommon.Debug.MsgBoxLoop("dgvMessages_DataError", e.ColumnIndex, e.RowIndex, e.Exception.Message)
		e.ThrowException = False
      e.Cancel = True
   End Sub
	Private Sub dgvMessages_RowEnter(ByVal oSender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMessages.RowEnter
      If e.RowIndex >= 0 AndAlso moCurrentDataView IsNot Nothing Then
         '  Dim oDatarow As DataRow = DMAcadExt.AppMessages.MsgTable.Rows.Item(e.RowIndex)
         Dim oDatarow As DataRow = moCurrentDataView(e.RowIndex).Row

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

   Private Sub frmMessages_Load(oSender As System.Object, e As EventArgs) Handles Me.Load

      If DMCommon.dmScreen.MyResolution <> DMCommon.enScreenResolutions.res1920x1080 Then
         Me.AutoScroll = True
      End If
      Me.ctxText.DefaultCellStyle = moWrapCellStyle
      '   zzResizeRows()
     

   End Sub




	Private Sub frmMessages_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
      If miClientHeightDflt = -1 Then
         miClientHeightDflt = Me.ClientSize.Height
      End If

      If Me.dgvMessages IsNot Nothing Then
         If miGridTop = -1 Then
            miGridTop = Me.dgvMessages.Top
         End If
         If Me.AutoScroll Then
				Me.dgvMessages.Height = Me.ClientSize.Height - Me.txtMessageText.Height - miGridTop - 42 '+ Me.Top
				Me.dgvMessages.Top = miGridTop
         Else
				Me.dgvMessages.Height = Me.ClientSize.Height - Me.txtMessageText.Height - miGridTop - 42
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

   Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
      Me.Close()
   End Sub

   Private Sub chkWrapText_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkWrapText.CheckedChanged
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


	Private Sub cmdZoomDown_Click(oSender As System.Object, e As EventArgs) Handles cmdZoomDown.Click
      cmdZoomNext(1)
      If False Then


         Dim oGridRow As DataGridViewRow = Me.dgvMessages.CurrentRow
         Dim oViewRow As DataRowView
         Dim iNextRowIndex As Integer
         If oGridRow IsNot Nothing Then
            iNextRowIndex = oGridRow.Index + 1
            If iNextRowIndex < Me.dgvMessages.RowCount Then
               oGridRow = Me.dgvMessages.Rows.Item(iNextRowIndex)
               Me.dgvMessages.CurrentCell = oGridRow.Cells(0)
               oViewRow = moCurrentDataView.Item(iNextRowIndex)
               DMAcadExt.AppMessages.Zoom(oViewRow.Row)
            End If
         End If
      End If

   End Sub
   Private Sub cmdZoomNext(iStep As Integer)
      Dim oGridRow As DataGridViewRow = Me.dgvMessages.CurrentRow
      Dim oViewRow As DataRowView
      Dim iNextRowIndex As Integer
      If oGridRow IsNot Nothing Then
         iNextRowIndex = oGridRow.Index + iStep
         If iNextRowIndex < Me.dgvMessages.RowCount AndAlso iNextRowIndex >= 0 Then
            oGridRow = Me.dgvMessages.Rows.Item(iNextRowIndex)
            Me.dgvMessages.CurrentCell = oGridRow.Cells(0)
            oViewRow = moCurrentDataView.Item(iNextRowIndex)
            DMAcadExt.AppMessages.Zoom(oViewRow.Row)
         End If
      End If

	End Sub

   Private Sub cmdZoomUp_Click(oSender As System.Object, e As EventArgs) Handles cmdZoomUp.Click
      cmdZoomNext(-1)
   End Sub


	Private Sub cmbMapThemes_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbMapThemes.SelectedIndexChanged
		zzSetActionList()
		zzApplyFilter()
	End Sub

	Private Sub cmbActions_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbActions.SelectedIndexChanged
		zzApplyFilter()
	End Sub

	Private Sub cmdShowMsg_Click(oSender As System.Object, e As EventArgs) Handles cmdShowMsg.Click
		Dim oDataRowView As DataRowView
		Dim oDataRow As DataRow
		Try
			Dim oViewRow As DataGridViewRow = Me.dgvMessages.CurrentRow
			If oViewRow IsNot Nothing Then
				oDataRowView = DirectCast(oViewRow.DataBoundItem, DataRowView)
				oDataRow = oDataRowView.Row
				zzShowMsg(26, oDataRow)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdZoomMsg_Click")
		End Try
	End Sub
	Private Sub zzShowMsg(iActionID As Integer, ByVal oDataRow As DataRow)


		Select Case iActionID
			Case 26
				'DMCommon.Debug.MsgBox("221120_1", DMCommon.Functions.CStrN(oDataRow.Item("SysID")))
				Dim sSysID As String = DMCommon.Functions.CStrN(oDataRow.Item("SysID"))
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex
				Dim iParcelID As Integer
				Dim iOverlayID As Integer

				If DMAcadExt.AppMessages.ParseSysID(sSysID, iOverlayIndex, iParcelID, iOverlayID) Then
					'DMCommon.Debug.MsgBox("221120_2", sSysID, iOverlayIndex, iParcelID, iOverlayID)

					TopoManager.TPlanGraph.TplnProject.ShowOverlayPgons(iOverlayIndex, iParcelID, iOverlayID, mcolMarkedEntities)


				Else
					DMCommon.Debug.MsgBox("221120_7", sSysID)
				End If
		End Select


	End Sub

	Private Sub cmdClear_Click(oSender As System.Object, e As EventArgs) Handles cmdClear.Click
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()


		'DMCommon.Debug.MsgBox("221120_4", iParcelID, oParcel.Name, oParcel.Lines.Count)

		DMAcadExt.AcadTransaction.SetColor(mcolMarkedEntities, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 0S), TriState.False)






		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
End Class