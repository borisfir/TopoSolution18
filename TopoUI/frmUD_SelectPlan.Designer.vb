<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmUD_SelectPlan
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmUD_SelectPlan))
		Me.cmdUp = New System.Windows.Forms.Button()
		Me.imlMain = New System.Windows.Forms.ImageList(Me.components)
		Me.cmdDown = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.ctxPlan = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxIndexNum = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBlockNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBlockAddNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxProcessNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'cmdUp
		'
		Me.cmdUp.AutoSize = True
		Me.cmdUp.ImageKey = "ArrowUp16Tr.png"
		Me.cmdUp.ImageList = Me.imlMain
		Me.cmdUp.Location = New System.Drawing.Point(2, 109)
		Me.cmdUp.Name = "cmdUp"
		Me.cmdUp.Size = New System.Drawing.Size(22, 28)
		Me.cmdUp.TabIndex = 1
		Me.cmdUp.UseVisualStyleBackColor = True
		'
		'imlMain
		'
		Me.imlMain.ImageStream = CType(resources.GetObject("imlMain.ImageStream"), System.Windows.Forms.ImageListStreamer)
		Me.imlMain.TransparentColor = System.Drawing.Color.Transparent
		Me.imlMain.Images.SetKeyName(0, "ArrowUp16Tr.png")
		Me.imlMain.Images.SetKeyName(1, "ArrowDown16Tr.png")
		Me.imlMain.Images.SetKeyName(2, "Check16Tr.png")
		Me.imlMain.Images.SetKeyName(3, "Cancel16Tr.png")
		'
		'cmdDown
		'
		Me.cmdDown.AutoSize = True
		Me.cmdDown.ImageKey = "ArrowDown16Tr.png"
		Me.cmdDown.ImageList = Me.imlMain
		Me.cmdDown.Location = New System.Drawing.Point(2, 143)
		Me.cmdDown.Name = "cmdDown"
		Me.cmdDown.Size = New System.Drawing.Size(22, 28)
		Me.cmdDown.TabIndex = 2
		Me.cmdDown.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSize = True
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.ImageKey = "Cancel16Tr.png"
		Me.cmdCancel.ImageList = Me.imlMain
		Me.cmdCancel.Location = New System.Drawing.Point(2, 60)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(22, 22)
		Me.cmdCancel.TabIndex = 3
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.AllowUserToResizeRows = False
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxPlan, Me.ctxIndexNum, Me.ctxBlockNo, Me.ctxBlockAddNo, Me.ctxProcessNo})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Right
		Me.dgvMain.Location = New System.Drawing.Point(30, 0)
		Me.dgvMain.MultiSelect = False
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.ReadOnly = True
		Me.dgvMain.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None
		Me.dgvMain.RowHeadersVisible = False
		Me.dgvMain.RowHeadersWidth = 4
		Me.dgvMain.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
		Me.dgvMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.dgvMain.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
		Me.dgvMain.ShowEditingIcon = False
		Me.dgvMain.Size = New System.Drawing.Size(218, 260)
		Me.dgvMain.TabIndex = 4
		'
		'cmdOK
		'
		Me.cmdOK.AutoSize = True
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.ImageKey = "Check16Tr.png"
		Me.cmdOK.ImageList = Me.imlMain
		Me.cmdOK.Location = New System.Drawing.Point(2, 26)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(22, 22)
		Me.cmdOK.TabIndex = 5
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'ctxPlan
		'
		Me.ctxPlan.DataPropertyName = "PlanID"
		Me.ctxPlan.DividerWidth = 1
		Me.ctxPlan.FillWeight = 20.0!
		Me.ctxPlan.HeaderText = "מס'"
		Me.ctxPlan.Name = "ctxPlan"
		Me.ctxPlan.ReadOnly = True
		Me.ctxPlan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPlan.Width = 36
		'
		'ctxIndexNum
		'
		Me.ctxIndexNum.DataPropertyName = "IndexNum"
		Me.ctxIndexNum.HeaderText = "ג.ב"
		Me.ctxIndexNum.Name = "ctxIndexNum"
		Me.ctxIndexNum.ReadOnly = True
		Me.ctxIndexNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxIndexNum.Width = 36
		'
		'ctxBlockNo
		'
		Me.ctxBlockNo.DataPropertyName = "OriginalBlockNo"
		Me.ctxBlockNo.HeaderText = "גוש"
		Me.ctxBlockNo.Name = "ctxBlockNo"
		Me.ctxBlockNo.ReadOnly = True
		Me.ctxBlockNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic
		Me.ctxBlockNo.Width = 52
		'
		'ctxBlockAddNo
		'
		Me.ctxBlockAddNo.DataPropertyName = "OriginalBlockAddNo"
		Me.ctxBlockAddNo.DividerWidth = 1
		Me.ctxBlockAddNo.HeaderText = "\..."
		Me.ctxBlockAddNo.Name = "ctxBlockAddNo"
		Me.ctxBlockAddNo.ReadOnly = True
		Me.ctxBlockAddNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBlockAddNo.Width = 24
		'
		'ctxProcessNo
		'
		Me.ctxProcessNo.DataPropertyName = "ProcessNo"
		Me.ctxProcessNo.DividerWidth = 1
		Me.ctxProcessNo.FillWeight = 40.0!
		Me.ctxProcessNo.HeaderText = "מס' אל'"
		Me.ctxProcessNo.Name = "ctxProcessNo"
		Me.ctxProcessNo.ReadOnly = True
		Me.ctxProcessNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxProcessNo.Width = 52
		'
		'frmUD_SelectPlan
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.cmdCancel
		Me.ClientSize = New System.Drawing.Size(248, 260)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.dgvMain)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdDown)
		Me.Controls.Add(Me.cmdUp)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
		Me.Name = "frmUD_SelectPlan"
		Me.Opacity = 0.8R
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.ShowInTaskbar = False
		Me.Text = "frmSelectPlan"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmdUp As Button
	Private WithEvents imlMain As ImageList
	Private WithEvents cmdDown As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents dgvMain As DataGridView
	Private WithEvents cmdOK As Button
	Friend WithEvents ctxPlan As DataGridViewTextBoxColumn
	Friend WithEvents ctxIndexNum As DataGridViewTextBoxColumn
	Friend WithEvents ctxBlockNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxBlockAddNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxProcessNo As DataGridViewTextBoxColumn
End Class
