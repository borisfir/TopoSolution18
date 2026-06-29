<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmUD_ProjectData
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
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
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.dgvPlanData = New System.Windows.Forms.DataGridView()
		Me.ctxPlanID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBlockNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBlockAddNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxOriginalParcelList = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxNewParcelList = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdSave = New System.Windows.Forms.Button()
		CType(Me.dgvPlanData, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvPlanData
		'
		Me.dgvPlanData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvPlanData.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxPlanID, Me.ctxBlockNo, Me.ctxBlockAddNo, Me.ctxOriginalParcelList, Me.ctxNewParcelList})
		Me.dgvPlanData.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvPlanData.Location = New System.Drawing.Point(0, 173)
		Me.dgvPlanData.Name = "dgvPlanData"
		Me.dgvPlanData.Size = New System.Drawing.Size(1402, 368)
		Me.dgvPlanData.TabIndex = 0
		'
		'ctxPlanID
		'
		Me.ctxPlanID.DataPropertyName = "PlanID"
		Me.ctxPlanID.HeaderText = "# תוכנית"
		Me.ctxPlanID.Name = "ctxPlanID"
		'
		'ctxBlockNo
		'
		Me.ctxBlockNo.DataPropertyName = "OriginalBlockNo"
		Me.ctxBlockNo.HeaderText = "גוש"
		Me.ctxBlockNo.Name = "ctxBlockNo"
		'
		'ctxBlockAddNo
		'
		Me.ctxBlockAddNo.DataPropertyName = "OriginalBlockAddNo"
		Me.ctxBlockAddNo.HeaderText = "תוספת"
		Me.ctxBlockAddNo.Name = "ctxBlockAddNo"
		'
		'ctxOriginalParcelList
		'
		Me.ctxOriginalParcelList.DataPropertyName = "OriginalParcelList"
		Me.ctxOriginalParcelList.HeaderText = "חלקות"
		Me.ctxOriginalParcelList.Name = "ctxOriginalParcelList"
		'
		'ctxNewParcelList
		'
		Me.ctxNewParcelList.DataPropertyName = "NewParcelList"
		Me.ctxNewParcelList.HeaderText = "חלקות"
		Me.ctxNewParcelList.Name = "ctxNewParcelList"
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
		Me.cmdExit.Location = New System.Drawing.Point(1366, 12)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(24, 25)
		Me.cmdExit.TabIndex = 10
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdSave
		'
		Me.cmdSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSave.Image = Global.TopoUI.My.Resources.Resources.Save
		Me.cmdSave.Location = New System.Drawing.Point(1336, 12)
		Me.cmdSave.Name = "cmdSave"
		Me.cmdSave.Size = New System.Drawing.Size(24, 25)
		Me.cmdSave.TabIndex = 11
		Me.cmdSave.UseVisualStyleBackColor = True
		'
		'frmUD_ProjectData
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1402, 541)
		Me.Controls.Add(Me.cmdSave)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.dgvPlanData)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmUD_ProjectData"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "נתוני פרויקט"
		Me.TopMost = True
		CType(Me.dgvPlanData, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	Private WithEvents dgvPlanData As DataGridView
	Friend WithEvents ctxPlanID As DataGridViewTextBoxColumn
	Friend WithEvents ctxBlockNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxBlockAddNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxOriginalParcelList As DataGridViewTextBoxColumn
	Friend WithEvents ctxNewParcelList As DataGridViewTextBoxColumn
	Private WithEvents cmdExit As Button
	Private WithEvents cmdSave As Button
End Class
