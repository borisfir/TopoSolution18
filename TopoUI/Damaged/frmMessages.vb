Option Explicit On
Option Strict On
Imports System.Data
Public Class frmMessages
	Private Const msToleranceFldName As String = "Tolerance"
	Private WithEvents dgvMessages As DataGridView
	Private txtMessageText As TextBox

	Private WithEvents cmdTableMsg As TabButton
	Private WithEvents cmdZoomMsg As TabButton
	Private WithEvents cmdClearMsg As TabButton
	Public Event DisplayBasePgon(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer)
	

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub
	Private Sub zzMyInitializeComponent()
		zzInitTabMessages()
		Me.dgvMessages.DataSource = DMAcadExt.AppMessages.MsgTable
	End Sub
	Protected Overridable Sub zzInitTabMessages()
		Me.dgvMessages = New System.Windows.Forms.DataGridView
		Me.txtMessageText = New System.Windows.Forms.TextBox



		'
		'txtMessageText
		'
		With txtMessageText
			.Location = New System.Drawing.Point(2, 186)
			.Name = "txtMessageText"
			.Size = New System.Drawing.Size(530, 48)
			.TabIndex = 45
			.Multiline = True
			.RightToLeft = Windows.Forms.RightToLeft.Yes
		End With
		Me.Controls.Add(Me.txtMessageText)

		'
		'dgvMessages
		'
		With Me.dgvMessages
			.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			.Location = New System.Drawing.Point(2, 2)
			.Name = "dgvMessages"
			.Font = frmPrjThemes.moLabelFont
			.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
			.RowHeadersWidth = 23
			.RowHeadersVisible = True
			.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
			.ColumnHeadersDefaultCellStyle.Font = frmPrjThemes.moBaseFont
			'  .Size = New System.Drawing.Size(316, 236)
			.Size = New System.Drawing.Size(530, 180)	 '224
			.TabIndex = 5
			.AllowUserToOrderColumns = False
			.AutoGenerateColumns = False
			.AllowUserToAddRows = False
			.AllowUserToDeleteRows = True
		End With
		Me.Controls.Add(Me.dgvMessages)
		zzSetMessagesGridColumns()

		Me.dgvMessages.DataSource = DMAcadExt.AppMessages.MsgTable
		Me.cmdTableMsg = New TabButton(frmPrjThemes.moLabelFont, False)
		'
		'cmdTableMsg
		'
		With Me.cmdTableMsg
			.Location = New System.Drawing.Point(40, 244)
			.Name = "cmdTableMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12
			.Text = "Table"
		End With
		Me.Controls.Add(Me.cmdTableMsg)

		Me.cmdZoomMsg = New TabButton(frmPrjThemes.moLabelFont, False)
		'
		'cmdZoomMsg
		'
		With Me.cmdZoomMsg
			.Location = New System.Drawing.Point(100, 244)
			.Name = "cmdZoomMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 12
			.Text = "Zoom"
		End With
		Me.Controls.Add(Me.cmdZoomMsg)

		Me.cmdClearMsg = New TabButton(frmPrjThemes.moLabelFont, False)

		'
		'cmdClearMsg
		'
		With Me.cmdClearMsg
			.Location = New System.Drawing.Point(160, 244)
			.Name = "cmdClearMsg"
			'	.Size = New System.Drawing.Size(88, 24)
			.TabIndex = 13
			.Text = "Clear"
		End With
		Me.Controls.Add(Me.cmdClearMsg)
	End Sub
	Private Sub zzSetMessagesGridColumns()	'''''As System.Windows.Forms.DataGridViewComboBoxColumn
		'	Dim oCmbColumn As New System.Windows.Forms.DataGridViewComboBoxColumn()
		Dim oColumn As DataGridViewTextBoxColumn

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.HeaderText = "Text"
			.Width = 346
			.Name = "Text"
			.ReadOnly = True
			.DataPropertyName = "Text"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With
		Try
			Me.dgvMessages.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_1")
		End Try

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.Name = msToleranceFldName
			.HeaderText = "X"
			.Width = 70
			.Name = "X"
			.DataPropertyName = "X"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvMessages.Columns.Add(oColumn)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - zzSetGridColumns_2")
		End Try

		oColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
		With oColumn
			.Name = msToleranceFldName
			.HeaderText = "Y"
			.Width = 70
			.Name = "Y"
			.DataPropertyName = "Y"
			.SortMode = DataGridViewColumnSortMode.NotSortable
		End With

		Try
			Me.dgvMessages.Columns.Add(oColumn)
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
			DMAcadExt.AppMessages.Clear()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - cmdClearMsg_Click")
		End Try
	End Sub

   Private Sub frmMessages_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
      '	MessageBox.Show(CStr(Me.Location.X) & ":" & CStr(Me.Location.Y), "01_056")
      If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 Then
         Me.Location = New System.Drawing.Point(0, 0)
      End If
   End Sub
End Class