<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTopoStatus
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
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.cmdSaveDWG = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdCloseSave = New System.Windows.Forms.Button()
		Me.ctxMapThemeName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxTopoName = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchArcs = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchIsCorrect = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.cchIsComplete = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxPolygonCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxMapThemeName, Me.ctxTopoName, Me.cchArcs, Me.cchIsCorrect, Me.cchIsComplete, Me.ctxPolygonCount})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Top
		Me.dgvMain.Location = New System.Drawing.Point(0, 0)
		Me.dgvMain.MultiSelect = False
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersVisible = False
		Me.dgvMain.Size = New System.Drawing.Size(479, 254)
		Me.dgvMain.TabIndex = 0
		'
		'cmdSaveDWG
		'
		Me.cmdSaveDWG.FlatAppearance.BorderSize = 0
		Me.cmdSaveDWG.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.cmdSaveDWG.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
		Me.cmdSaveDWG.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSaveDWG.Location = New System.Drawing.Point(116, 260)
		Me.cmdSaveDWG.Name = "cmdSaveDWG"
		Me.cmdSaveDWG.Size = New System.Drawing.Size(75, 23)
		Me.cmdSaveDWG.TabIndex = 1
		Me.cmdSaveDWG.Text = "Save DWG"
		Me.cmdSaveDWG.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.FlatAppearance.BorderSize = 0
		Me.cmdCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.cmdCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
		Me.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdCancel.Location = New System.Drawing.Point(331, 260)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
		Me.cmdCancel.TabIndex = 2
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdCloseSave
		'
		Me.cmdCloseSave.FlatAppearance.BorderSize = 0
		Me.cmdCloseSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.cmdCloseSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
		Me.cmdCloseSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdCloseSave.Location = New System.Drawing.Point(210, 260)
		Me.cmdCloseSave.Name = "cmdCloseSave"
		Me.cmdCloseSave.Size = New System.Drawing.Size(115, 23)
		Me.cmdCloseSave.TabIndex = 3
		Me.cmdCloseSave.Text = "Save+Close DWG"
		Me.cmdCloseSave.UseVisualStyleBackColor = True
		'
		'ctxMapThemeName
		'
		Me.ctxMapThemeName.HeaderText = "שם נושא"
		Me.ctxMapThemeName.Name = "ctxMapThemeName"
		'
		'ctxTopoName
		'
		Me.ctxTopoName.HeaderText = "שם טופולוגיה"
		Me.ctxTopoName.Name = "ctxTopoName"
		'
		'cchArcs
		'
		Me.cchArcs.HeaderText = "קשתות"
		Me.cchArcs.Name = "cchArcs"
		Me.cchArcs.Width = 60
		'
		'cchIsCorrect
		'
		Me.cchIsCorrect.HeaderText = "Correct"
		Me.cchIsCorrect.Name = "cchIsCorrect"
		Me.cchIsCorrect.Width = 60
		'
		'cchIsComplete
		'
		Me.cchIsComplete.HeaderText = "Complete"
		Me.cchIsComplete.Name = "cchIsComplete"
		Me.cchIsComplete.Width = 60
		'
		'ctxPolygonCount
		'
		Me.ctxPolygonCount.HeaderText = "מס' פוליגונים"
		Me.ctxPolygonCount.Name = "ctxPolygonCount"
		Me.ctxPolygonCount.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ctxPolygonCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPolygonCount.Width = 80
		'
		'frmTopoStatus
		'
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
		Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.ClientSize = New System.Drawing.Size(479, 289)
		Me.Controls.Add(Me.cmdCloseSave)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdSaveDWG)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmTopoStatus"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.ShowInTaskbar = False
		Me.Text = "סטטוס טופולוגיות"
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	Private WithEvents dgvMain As DataGridView
	Private WithEvents cmdSaveDWG As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdCloseSave As Button
	Friend WithEvents ctxMapThemeName As DataGridViewTextBoxColumn
	Friend WithEvents ctxTopoName As DataGridViewTextBoxColumn
	Friend WithEvents cchArcs As DataGridViewCheckBoxColumn
	Friend WithEvents cchIsCorrect As DataGridViewCheckBoxColumn
	Friend WithEvents cchIsComplete As DataGridViewCheckBoxColumn
	Friend WithEvents ctxPolygonCount As DataGridViewTextBoxColumn
End Class
