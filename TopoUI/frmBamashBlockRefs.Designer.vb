<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmBamashBlockRefs
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
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBamashBlockRefs))
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.ctxUserID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPropID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxSubParcelNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxCaption = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ccbPropType = New System.Windows.Forms.DataGridViewComboBoxColumn()
		Me.ctxBldNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBldPart = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBldEntr = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBldFloor = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxBldFloorDesc = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ccbAprtDescNum = New System.Windows.Forms.DataGridViewComboBoxColumn()
		Me.ctxAprtDesc = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxAprtDesc2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchMain = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ccbPolygonColor = New System.Windows.Forms.DataGridViewComboBoxColumn()
		Me.ctxPurchaser = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchChanged = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxPropKey = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPgonKey = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdZoom = New System.Windows.Forms.Button()
		Me.pcbImage = New System.Windows.Forms.PictureBox()
		Me.cmdSelectByPick = New System.Windows.Forms.Button()
		Me.chkAllBlocks = New System.Windows.Forms.CheckBox()
		Me.cmdSetValue = New System.Windows.Forms.Button()
		Me.txtValue = New System.Windows.Forms.TextBox()
		Me.cmbAprtDesc = New System.Windows.Forms.ComboBox()
		Me.cmdSetApartDesc = New System.Windows.Forms.Button()
		Me.cmdPlusValue = New System.Windows.Forms.Button()
		Me.txtAddendum = New System.Windows.Forms.TextBox()
		Me.cmdUpdateKeys = New System.Windows.Forms.Button()
		Me.cmdSerial = New System.Windows.Forms.Button()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'dgvMain
		'
		Me.dgvMain.AllowUserToAddRows = False
		Me.dgvMain.AllowUserToDeleteRows = False
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxUserID, Me.ctxPropID, Me.ctxSubParcelNo, Me.ctxCaption, Me.ccbPropType, Me.ctxBldNo, Me.ctxBldPart, Me.ctxBldEntr, Me.ctxBldFloor, Me.ctxBldFloorDesc, Me.ccbAprtDescNum, Me.ctxAprtDesc, Me.ctxAprtDesc2, Me.cchMain, Me.ccbPolygonColor, Me.ctxPurchaser, Me.ctxArea, Me.cchChanged, Me.ctxPropKey, Me.ctxPgonKey})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.dgvMain.Location = New System.Drawing.Point(0, 32)
		Me.dgvMain.Margin = New System.Windows.Forms.Padding(2)
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.RowTemplate.Height = 24
		Me.dgvMain.Size = New System.Drawing.Size(1185, 269)
		Me.dgvMain.TabIndex = 0
		'
		'ctxUserID
		'
		Me.ctxUserID.DataPropertyName = "UserID"
		Me.ctxUserID.HeaderText = "UserID"
		Me.ctxUserID.Name = "ctxUserID"
		Me.ctxUserID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxUserID.Visible = False
		Me.ctxUserID.Width = 64
		'
		'ctxPropID
		'
		Me.ctxPropID.DataPropertyName = "PropID"
		Me.ctxPropID.HeaderText = "מס' יחידת משנה"
		Me.ctxPropID.Name = "ctxPropID"
		Me.ctxPropID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPropID.Width = 56
		'
		'ctxSubParcelNo
		'
		Me.ctxSubParcelNo.DataPropertyName = "SubParcelNo"
		Me.ctxSubParcelNo.HeaderText = "תת חלקה"
		Me.ctxSubParcelNo.Name = "ctxSubParcelNo"
		Me.ctxSubParcelNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxSubParcelNo.Width = 64
		'
		'ctxCaption
		'
		Me.ctxCaption.DataPropertyName = "Caption"
		Me.ctxCaption.HeaderText = "כותרת"
		Me.ctxCaption.Name = "ctxCaption"
		Me.ctxCaption.ReadOnly = True
		Me.ctxCaption.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxCaption.Width = 48
		'
		'ccbPropType
		'
		Me.ccbPropType.DataPropertyName = "PropType"
		Me.ccbPropType.DisplayStyleForCurrentCellOnly = True
		Me.ccbPropType.HeaderText = "סוג נכס"
		Me.ccbPropType.Name = "ccbPropType"
		Me.ccbPropType.Width = 112
		'
		'ctxBldNo
		'
		Me.ctxBldNo.DataPropertyName = "BldNo"
		Me.ctxBldNo.HeaderText = "מס' בית"
		Me.ctxBldNo.Name = "ctxBldNo"
		Me.ctxBldNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBldNo.ToolTipText = "מס' בית"
		Me.ctxBldNo.Width = 56
		'
		'ctxBldPart
		'
		Me.ctxBldPart.DataPropertyName = "BldPart"
		Me.ctxBldPart.HeaderText = "אגף"
		Me.ctxBldPart.Name = "ctxBldPart"
		Me.ctxBldPart.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBldPart.Width = 36
		'
		'ctxBldEntr
		'
		Me.ctxBldEntr.DataPropertyName = "BldEntr"
		Me.ctxBldEntr.HeaderText = "כניסה"
		Me.ctxBldEntr.Name = "ctxBldEntr"
		Me.ctxBldEntr.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBldEntr.Width = 44
		'
		'ctxBldFloor
		'
		Me.ctxBldFloor.DataPropertyName = "BldFloor"
		Me.ctxBldFloor.HeaderText = "קומה"
		Me.ctxBldFloor.Name = "ctxBldFloor"
		Me.ctxBldFloor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBldFloor.Width = 40
		'
		'ctxBldFloorDesc
		'
		Me.ctxBldFloorDesc.DataPropertyName = "BldFloorDesc"
		Me.ctxBldFloorDesc.HeaderText = "תיאור קומה"
		Me.ctxBldFloorDesc.Name = "ctxBldFloorDesc"
		Me.ctxBldFloorDesc.ReadOnly = True
		Me.ctxBldFloorDesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxBldFloorDesc.Width = 104
		'
		'ccbAprtDescNum
		'
		Me.ccbAprtDescNum.DataPropertyName = "AprtDescNum"
		Me.ccbAprtDescNum.DisplayStyleForCurrentCellOnly = True
		Me.ccbAprtDescNum.HeaderText = "תיאור נכס"
		Me.ccbAprtDescNum.Name = "ccbAprtDescNum"
		Me.ccbAprtDescNum.Width = 112
		'
		'ctxAprtDesc
		'
		Me.ctxAprtDesc.DataPropertyName = "AprtDesc"
		Me.ctxAprtDesc.HeaderText = "תיאור"
		Me.ctxAprtDesc.Name = "ctxAprtDesc"
		Me.ctxAprtDesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxAprtDesc.Visible = False
		Me.ctxAprtDesc.Width = 112
		'
		'ctxAprtDesc2
		'
		Me.ctxAprtDesc2.DataPropertyName = "AprtDesc2"
		Me.ctxAprtDesc2.HeaderText = "תיאור א'"
		Me.ctxAprtDesc2.Name = "ctxAprtDesc2"
		Me.ctxAprtDesc2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		'
		'cchMain
		'
		Me.cchMain.DataPropertyName = "Main"
		Me.cchMain.HeaderText = "ראשי"
		Me.cchMain.Name = "cchMain"
		Me.cchMain.Width = 40
		'
		'ccbPolygonColor
		'
		Me.ccbPolygonColor.DataPropertyName = "PolygonColor"
		Me.ccbPolygonColor.DisplayStyleForCurrentCellOnly = True
		Me.ccbPolygonColor.HeaderText = "צבע"
		Me.ccbPolygonColor.Name = "ccbPolygonColor"
		Me.ccbPolygonColor.Width = 64
		'
		'ctxPurchaser
		'
		Me.ctxPurchaser.DataPropertyName = "Purchaser"
		Me.ctxPurchaser.DividerWidth = 2
		Me.ctxPurchaser.HeaderText = "שם הרוכש"
		Me.ctxPurchaser.Name = "ctxPurchaser"
		Me.ctxPurchaser.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPurchaser.Width = 72
		'
		'ctxArea
		'
		Me.ctxArea.DataPropertyName = "Area"
		DataGridViewCellStyle2.Format = "N2"
		DataGridViewCellStyle2.NullValue = Nothing
		Me.ctxArea.DefaultCellStyle = DataGridViewCellStyle2
		Me.ctxArea.HeaderText = "שטח"
		Me.ctxArea.Name = "ctxArea"
		Me.ctxArea.ReadOnly = True
		Me.ctxArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxArea.Width = 48
		'
		'cchChanged
		'
		Me.cchChanged.DataPropertyName = "Changed"
		Me.cchChanged.HeaderText = "שונה"
		Me.cchChanged.Name = "cchChanged"
		Me.cchChanged.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
		Me.cchChanged.Width = 36
		'
		'ctxPropKey
		'
		Me.ctxPropKey.DataPropertyName = "PropKey"
		Me.ctxPropKey.HeaderText = "PropKey"
		Me.ctxPropKey.Name = "ctxPropKey"
		Me.ctxPropKey.Visible = False
		Me.ctxPropKey.Width = 64
		'
		'ctxPgonKey
		'
		Me.ctxPgonKey.DataPropertyName = "PgonKey"
		Me.ctxPgonKey.HeaderText = "PgonKey"
		Me.ctxPgonKey.Name = "ctxPgonKey"
		Me.ctxPgonKey.Visible = False
		Me.ctxPgonKey.Width = 64
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
		Me.cmdExit.Location = New System.Drawing.Point(174, 0)
		Me.cmdExit.Margin = New System.Windows.Forms.Padding(2)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(19, 24)
		Me.cmdExit.TabIndex = 13
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Image = CType(resources.GetObject("cmdCancel.Image"), System.Drawing.Image)
		Me.cmdCancel.Location = New System.Drawing.Point(147, 0)
		Me.cmdCancel.Margin = New System.Windows.Forms.Padding(2)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(24, 24)
		Me.cmdCancel.TabIndex = 12
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Image = CType(resources.GetObject("cmdOK.Image"), System.Drawing.Image)
		Me.cmdOK.Location = New System.Drawing.Point(118, 0)
		Me.cmdOK.Margin = New System.Windows.Forms.Padding(2)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(25, 24)
		Me.cmdOK.TabIndex = 11
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'cmdZoom
		'
		Me.cmdZoom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdZoom.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdZoom.Image = CType(resources.GetObject("cmdZoom.Image"), System.Drawing.Image)
		Me.cmdZoom.Location = New System.Drawing.Point(54, -1)
		Me.cmdZoom.Margin = New System.Windows.Forms.Padding(2)
		Me.cmdZoom.Name = "cmdZoom"
		Me.cmdZoom.Size = New System.Drawing.Size(24, 24)
		Me.cmdZoom.TabIndex = 30
		Me.cmdZoom.UseVisualStyleBackColor = True
		'
		'pcbImage
		'
		Me.pcbImage.BackColor = System.Drawing.Color.Transparent
		Me.pcbImage.Location = New System.Drawing.Point(1149, 0)
		Me.pcbImage.Margin = New System.Windows.Forms.Padding(2)
		Me.pcbImage.Name = "pcbImage"
		Me.pcbImage.Size = New System.Drawing.Size(36, 20)
		Me.pcbImage.TabIndex = 195
		Me.pcbImage.TabStop = False
		Me.pcbImage.Visible = False
		'
		'cmdSelectByPick
		'
		Me.cmdSelectByPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSelectByPick.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdSelectByPick.Image = CType(resources.GetObject("cmdSelectByPick.Image"), System.Drawing.Image)
		Me.cmdSelectByPick.Location = New System.Drawing.Point(25, -1)
		Me.cmdSelectByPick.Name = "cmdSelectByPick"
		Me.cmdSelectByPick.Size = New System.Drawing.Size(24, 24)
		Me.cmdSelectByPick.TabIndex = 196
		Me.cmdSelectByPick.UseVisualStyleBackColor = True
		'
		'chkAllBlocks
		'
		Me.chkAllBlocks.AutoSize = True
		Me.chkAllBlocks.Location = New System.Drawing.Point(207, 2)
		Me.chkAllBlocks.Name = "chkAllBlocks"
		Me.chkAllBlocks.Size = New System.Drawing.Size(93, 17)
		Me.chkAllBlocks.TabIndex = 197
		Me.chkAllBlocks.Text = "בלוקים ריקים"
		Me.chkAllBlocks.UseVisualStyleBackColor = True
		'
		'cmdSetValue
		'
		Me.cmdSetValue.Location = New System.Drawing.Point(452, 4)
		Me.cmdSetValue.Name = "cmdSetValue"
		Me.cmdSetValue.Size = New System.Drawing.Size(30, 20)
		Me.cmdSetValue.TabIndex = 199
		Me.cmdSetValue.Text = "OK"
		Me.cmdSetValue.UseVisualStyleBackColor = True
		'
		'txtValue
		'
		Me.txtValue.Location = New System.Drawing.Point(386, 2)
		Me.txtValue.Name = "txtValue"
		Me.txtValue.Size = New System.Drawing.Size(64, 21)
		Me.txtValue.TabIndex = 198
		'
		'cmbAprtDesc
		'
		Me.cmbAprtDesc.FormattingEnabled = True
		Me.cmbAprtDesc.Location = New System.Drawing.Point(490, 2)
		Me.cmbAprtDesc.Name = "cmbAprtDesc"
		Me.cmbAprtDesc.Size = New System.Drawing.Size(96, 21)
		Me.cmbAprtDesc.TabIndex = 200
		'
		'cmdSetApartDesc
		'
		Me.cmdSetApartDesc.Location = New System.Drawing.Point(588, 4)
		Me.cmdSetApartDesc.Name = "cmdSetApartDesc"
		Me.cmdSetApartDesc.Size = New System.Drawing.Size(30, 20)
		Me.cmdSetApartDesc.TabIndex = 201
		Me.cmdSetApartDesc.Text = "OK"
		Me.cmdSetApartDesc.UseVisualStyleBackColor = True
		'
		'cmdPlusValue
		'
		Me.cmdPlusValue.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdPlusValue.Location = New System.Drawing.Point(364, 4)
		Me.cmdPlusValue.Name = "cmdPlusValue"
		Me.cmdPlusValue.Size = New System.Drawing.Size(14, 20)
		Me.cmdPlusValue.TabIndex = 203
		Me.cmdPlusValue.Text = "+"
		Me.cmdPlusValue.UseVisualStyleBackColor = True
		'
		'txtAddendum
		'
		Me.txtAddendum.Location = New System.Drawing.Point(326, 3)
		Me.txtAddendum.Name = "txtAddendum"
		Me.txtAddendum.Size = New System.Drawing.Size(36, 21)
		Me.txtAddendum.TabIndex = 202
		'
		'cmdUpdateKeys
		'
		Me.cmdUpdateKeys.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdUpdateKeys.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdUpdateKeys.Image = Global.TopoUI.My.Resources.Resources.Invert12
		Me.cmdUpdateKeys.Location = New System.Drawing.Point(89, -1)
		Me.cmdUpdateKeys.Margin = New System.Windows.Forms.Padding(2)
		Me.cmdUpdateKeys.Name = "cmdUpdateKeys"
		Me.cmdUpdateKeys.Size = New System.Drawing.Size(25, 24)
		Me.cmdUpdateKeys.TabIndex = 204
		Me.cmdUpdateKeys.UseVisualStyleBackColor = True
		'
		'cmdSerial
		'
		Me.cmdSerial.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdSerial.Image = Global.TopoUI.My.Resources.Resources.ArrowDown16Tr
		Me.cmdSerial.Location = New System.Drawing.Point(306, 1)
		Me.cmdSerial.Name = "cmdSerial"
		Me.cmdSerial.Size = New System.Drawing.Size(20, 24)
		Me.cmdSerial.TabIndex = 205
		Me.cmdSerial.UseVisualStyleBackColor = True
		Me.cmdSerial.Visible = False
		'
		'frmBamashBlockRefs
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.CausesValidation = False
		Me.ClientSize = New System.Drawing.Size(1185, 301)
		Me.Controls.Add(Me.cmdSerial)
		Me.Controls.Add(Me.cmdUpdateKeys)
		Me.Controls.Add(Me.cmdPlusValue)
		Me.Controls.Add(Me.txtAddendum)
		Me.Controls.Add(Me.cmdSetApartDesc)
		Me.Controls.Add(Me.cmbAprtDesc)
		Me.Controls.Add(Me.cmdSetValue)
		Me.Controls.Add(Me.txtValue)
		Me.Controls.Add(Me.chkAllBlocks)
		Me.Controls.Add(Me.cmdSelectByPick)
		Me.Controls.Add(Me.pcbImage)
		Me.Controls.Add(Me.cmdZoom)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.dgvMain)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Margin = New System.Windows.Forms.Padding(2)
		Me.MinimizeBox = False
		Me.Name = "frmBamashBlockRefs"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.Text = "נכסים"
		Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.pcbImage, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents dgvMain As DataGridView
	Private WithEvents cmdExit As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdOK As Button
	Private WithEvents cmdZoom As Button
	Private WithEvents pcbImage As PictureBox
	Private WithEvents cmdSelectByPick As Button
	Private WithEvents chkAllBlocks As CheckBox
	Private WithEvents cmdSetValue As Button
	Private WithEvents txtValue As TextBox
	Private WithEvents cmbAprtDesc As ComboBox
	Private WithEvents cmdSetApartDesc As Button
	Private WithEvents cmdPlusValue As Button
	Private WithEvents txtAddendum As TextBox
	Private WithEvents cmdUpdateKeys As Button
	Friend WithEvents ctxUserID As DataGridViewTextBoxColumn
	Friend WithEvents ctxPropID As DataGridViewTextBoxColumn
	Friend WithEvents ctxSubParcelNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxCaption As DataGridViewTextBoxColumn
	Friend WithEvents ccbPropType As DataGridViewComboBoxColumn
	Friend WithEvents ctxBldNo As DataGridViewTextBoxColumn
	Friend WithEvents ctxBldPart As DataGridViewTextBoxColumn
	Friend WithEvents ctxBldEntr As DataGridViewTextBoxColumn
	Friend WithEvents ctxBldFloor As DataGridViewTextBoxColumn
	Friend WithEvents ctxBldFloorDesc As DataGridViewTextBoxColumn
	Friend WithEvents ccbAprtDescNum As DataGridViewComboBoxColumn
	Friend WithEvents ctxAprtDesc As DataGridViewTextBoxColumn
	Friend WithEvents ctxAprtDesc2 As DataGridViewTextBoxColumn
	Friend WithEvents cchMain As DataGridViewCheckBoxColumn
	Friend WithEvents ccbPolygonColor As DataGridViewComboBoxColumn
	Friend WithEvents ctxPurchaser As DataGridViewTextBoxColumn
	Friend WithEvents ctxArea As DataGridViewTextBoxColumn
	Friend WithEvents cchChanged As DataGridViewCheckBoxColumn
	Friend WithEvents ctxPropKey As DataGridViewTextBoxColumn
	Friend WithEvents ctxPgonKey As DataGridViewTextBoxColumn
	Private WithEvents cmdSerial As Button
End Class
