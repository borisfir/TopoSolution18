<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
		Me.bnnPlans = New System.Windows.Forms.BindingNavigator(Me.components)
		Me.BindingNavigatorAddNewItem = New System.Windows.Forms.ToolStripButton()
		Me.BindingNavigatorCountItem = New System.Windows.Forms.ToolStripLabel()
		Me.BindingNavigatorDeleteItem = New System.Windows.Forms.ToolStripButton()
		Me.BindingNavigatorMoveFirstItem = New System.Windows.Forms.ToolStripButton()
		Me.BindingNavigatorMovePreviousItem = New System.Windows.Forms.ToolStripButton()
		Me.BindingNavigatorSeparator = New System.Windows.Forms.ToolStripSeparator()
		Me.BindingNavigatorPositionItem = New System.Windows.Forms.ToolStripTextBox()
		Me.BindingNavigatorSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.BindingNavigatorMoveNextItem = New System.Windows.Forms.ToolStripButton()
		Me.BindingNavigatorSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.BindingNavigatorMoveLastItem = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
		Me.bnsPlans = New System.Windows.Forms.BindingSource(Me.components)
		Me.mskPlanID = New System.Windows.Forms.MaskedTextBox()
		Me.txtNormalParcels = New System.Windows.Forms.TextBox()
		Me.txtGush = New System.Windows.Forms.TextBox()
		Me.cmdSave = New System.Windows.Forms.Button()
		Me.mskProjectCode = New System.Windows.Forms.MaskedTextBox()
		Me.mskDetail = New System.Windows.Forms.MaskedTextBox()
		CType(Me.bnnPlans, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.bnnPlans.SuspendLayout()
		CType(Me.bnsPlans, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'bnnPlans
		'
		Me.bnnPlans.AddNewItem = Me.BindingNavigatorAddNewItem
		Me.bnnPlans.CountItem = Me.BindingNavigatorCountItem
		Me.bnnPlans.DeleteItem = Me.BindingNavigatorDeleteItem
		Me.bnnPlans.Dock = System.Windows.Forms.DockStyle.None
		Me.bnnPlans.ImageScalingSize = New System.Drawing.Size(20, 20)
		Me.bnnPlans.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BindingNavigatorMoveFirstItem, Me.BindingNavigatorMovePreviousItem, Me.BindingNavigatorSeparator, Me.BindingNavigatorPositionItem, Me.BindingNavigatorCountItem, Me.BindingNavigatorSeparator1, Me.BindingNavigatorMoveNextItem, Me.BindingNavigatorSeparator2, Me.BindingNavigatorMoveLastItem, Me.BindingNavigatorAddNewItem, Me.BindingNavigatorDeleteItem, Me.ToolStripButton1})
		Me.bnnPlans.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow
		Me.bnnPlans.Location = New System.Drawing.Point(398, 9)
		Me.bnnPlans.MoveFirstItem = Me.BindingNavigatorMoveFirstItem
		Me.bnnPlans.MoveLastItem = Me.BindingNavigatorMoveLastItem
		Me.bnnPlans.MoveNextItem = Me.BindingNavigatorMoveNextItem
		Me.bnnPlans.MovePreviousItem = Me.BindingNavigatorMovePreviousItem
		Me.bnnPlans.Name = "bnnPlans"
		Me.bnnPlans.PositionItem = Me.BindingNavigatorPositionItem
		Me.bnnPlans.RightToLeft = System.Windows.Forms.RightToLeft.No
		Me.bnnPlans.Size = New System.Drawing.Size(285, 27)
		Me.bnnPlans.TabIndex = 224
		'
		'BindingNavigatorAddNewItem
		'
		Me.BindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorAddNewItem.Image = CType(resources.GetObject("BindingNavigatorAddNewItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorAddNewItem.Name = "BindingNavigatorAddNewItem"
		Me.BindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorAddNewItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorAddNewItem.Text = "Add new"
		'
		'BindingNavigatorCountItem
		'
		Me.BindingNavigatorCountItem.Name = "BindingNavigatorCountItem"
		Me.BindingNavigatorCountItem.Size = New System.Drawing.Size(35, 24)
		Me.BindingNavigatorCountItem.Text = "of {0}"
		Me.BindingNavigatorCountItem.ToolTipText = "Total number of items"
		'
		'BindingNavigatorDeleteItem
		'
		Me.BindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorDeleteItem.Image = CType(resources.GetObject("BindingNavigatorDeleteItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorDeleteItem.Name = "BindingNavigatorDeleteItem"
		Me.BindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorDeleteItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorDeleteItem.Text = "Delete"
		'
		'BindingNavigatorMoveFirstItem
		'
		Me.BindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorMoveFirstItem.Image = CType(resources.GetObject("BindingNavigatorMoveFirstItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorMoveFirstItem.Name = "BindingNavigatorMoveFirstItem"
		Me.BindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorMoveFirstItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorMoveFirstItem.Text = "Move first"
		'
		'BindingNavigatorMovePreviousItem
		'
		Me.BindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorMovePreviousItem.Image = CType(resources.GetObject("BindingNavigatorMovePreviousItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorMovePreviousItem.Name = "BindingNavigatorMovePreviousItem"
		Me.BindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorMovePreviousItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorMovePreviousItem.Text = "Move previous"
		'
		'BindingNavigatorSeparator
		'
		Me.BindingNavigatorSeparator.Name = "BindingNavigatorSeparator"
		Me.BindingNavigatorSeparator.Size = New System.Drawing.Size(6, 27)
		'
		'BindingNavigatorPositionItem
		'
		Me.BindingNavigatorPositionItem.AccessibleName = "Position"
		Me.BindingNavigatorPositionItem.AutoSize = False
		Me.BindingNavigatorPositionItem.Name = "BindingNavigatorPositionItem"
		Me.BindingNavigatorPositionItem.Size = New System.Drawing.Size(50, 23)
		Me.BindingNavigatorPositionItem.Text = "0"
		Me.BindingNavigatorPositionItem.ToolTipText = "Current position"
		'
		'BindingNavigatorSeparator1
		'
		Me.BindingNavigatorSeparator1.Name = "BindingNavigatorSeparator1"
		Me.BindingNavigatorSeparator1.Size = New System.Drawing.Size(6, 27)
		'
		'BindingNavigatorMoveNextItem
		'
		Me.BindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorMoveNextItem.Image = CType(resources.GetObject("BindingNavigatorMoveNextItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorMoveNextItem.Name = "BindingNavigatorMoveNextItem"
		Me.BindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorMoveNextItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorMoveNextItem.Text = "Move next"
		'
		'BindingNavigatorSeparator2
		'
		Me.BindingNavigatorSeparator2.Name = "BindingNavigatorSeparator2"
		Me.BindingNavigatorSeparator2.Size = New System.Drawing.Size(6, 27)
		'
		'BindingNavigatorMoveLastItem
		'
		Me.BindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.BindingNavigatorMoveLastItem.Image = CType(resources.GetObject("BindingNavigatorMoveLastItem.Image"), System.Drawing.Image)
		Me.BindingNavigatorMoveLastItem.Name = "BindingNavigatorMoveLastItem"
		Me.BindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = True
		Me.BindingNavigatorMoveLastItem.Size = New System.Drawing.Size(24, 24)
		Me.BindingNavigatorMoveLastItem.Text = "Move last"
		'
		'ToolStripButton1
		'
		Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
		Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripButton1.Name = "ToolStripButton1"
		Me.ToolStripButton1.Size = New System.Drawing.Size(24, 24)
		Me.ToolStripButton1.Text = "ToolStripButton1"
		'
		'bnsPlans
		'
		'
		'mskPlanID
		'
		Me.mskPlanID.Location = New System.Drawing.Point(321, 123)
		Me.mskPlanID.Mask = "000"
		Me.mskPlanID.Name = "mskPlanID"
		Me.mskPlanID.Size = New System.Drawing.Size(68, 20)
		Me.mskPlanID.TabIndex = 229
		'
		'txtNormalParcels
		'
		Me.txtNormalParcels.Location = New System.Drawing.Point(289, 181)
		Me.txtNormalParcels.Name = "txtNormalParcels"
		Me.txtNormalParcels.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.txtNormalParcels.Size = New System.Drawing.Size(144, 20)
		Me.txtNormalParcels.TabIndex = 228
		'
		'txtGush
		'
		Me.txtGush.Location = New System.Drawing.Point(289, 149)
		Me.txtGush.Name = "txtGush"
		Me.txtGush.Size = New System.Drawing.Size(100, 20)
		Me.txtGush.TabIndex = 227
		'
		'cmdSave
		'
		Me.cmdSave.Location = New System.Drawing.Point(52, 39)
		Me.cmdSave.Name = "cmdSave"
		Me.cmdSave.Size = New System.Drawing.Size(75, 23)
		Me.cmdSave.TabIndex = 230
		Me.cmdSave.Text = "Button1"
		Me.cmdSave.UseVisualStyleBackColor = True
		'
		'mskProjectCode
		'
		Me.mskProjectCode.Location = New System.Drawing.Point(321, 64)
		Me.mskProjectCode.Mask = "000000"
		Me.mskProjectCode.Name = "mskProjectCode"
		Me.mskProjectCode.Size = New System.Drawing.Size(68, 20)
		Me.mskProjectCode.TabIndex = 231
		'
		'mskDetail
		'
		Me.mskDetail.Location = New System.Drawing.Point(321, 97)
		Me.mskDetail.Mask = "000"
		Me.mskDetail.Name = "mskDetail"
		Me.mskDetail.Size = New System.Drawing.Size(80, 20)
		Me.mskDetail.TabIndex = 232
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(723, 261)
		Me.Controls.Add(Me.mskDetail)
		Me.Controls.Add(Me.mskProjectCode)
		Me.Controls.Add(Me.cmdSave)
		Me.Controls.Add(Me.mskPlanID)
		Me.Controls.Add(Me.txtNormalParcels)
		Me.Controls.Add(Me.txtGush)
		Me.Controls.Add(Me.bnnPlans)
		Me.Name = "Form1"
		Me.Text = "Form1"
		CType(Me.bnnPlans, System.ComponentModel.ISupportInitialize).EndInit()
		Me.bnnPlans.ResumeLayout(False)
		Me.bnnPlans.PerformLayout()
		CType(Me.bnsPlans, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents bnnPlans As Windows.Forms.BindingNavigator
	Friend WithEvents BindingNavigatorAddNewItem As Windows.Forms.ToolStripButton
	Friend WithEvents BindingNavigatorCountItem As Windows.Forms.ToolStripLabel
	Friend WithEvents BindingNavigatorDeleteItem As Windows.Forms.ToolStripButton
	Friend WithEvents BindingNavigatorMoveFirstItem As Windows.Forms.ToolStripButton
	Friend WithEvents BindingNavigatorMovePreviousItem As Windows.Forms.ToolStripButton
	Friend WithEvents BindingNavigatorSeparator As Windows.Forms.ToolStripSeparator
	Friend WithEvents BindingNavigatorPositionItem As Windows.Forms.ToolStripTextBox
	Friend WithEvents BindingNavigatorSeparator1 As Windows.Forms.ToolStripSeparator
	Friend WithEvents BindingNavigatorMoveNextItem As Windows.Forms.ToolStripButton
	Friend WithEvents BindingNavigatorSeparator2 As Windows.Forms.ToolStripSeparator
	Friend WithEvents BindingNavigatorMoveLastItem As Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripButton1 As Windows.Forms.ToolStripButton
	Private WithEvents bnsPlans As Windows.Forms.BindingSource
	Private WithEvents mskPlanID As Windows.Forms.MaskedTextBox
	Private WithEvents txtNormalParcels As Windows.Forms.TextBox
	Private WithEvents txtGush As Windows.Forms.TextBox
	Friend WithEvents cmdSave As Windows.Forms.Button
	Private WithEvents mskProjectCode As Windows.Forms.MaskedTextBox
	Private WithEvents mskDetail As Windows.Forms.MaskedTextBox
End Class
