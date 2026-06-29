Namespace Expro
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class frmEntConnected
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
			Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
			Me.cmdDissolve = New System.Windows.Forms.Button()
			Me.cmdUnion = New System.Windows.Forms.Button()
			Me.cmdClip = New System.Windows.Forms.Button()
			Me.cmdParcExMapLayer = New System.Windows.Forms.Button()
			Me.cmdEntAreaMapLayer = New System.Windows.Forms.Button()
			Me.cmdLoadModel = New System.Windows.Forms.Button()
			Me.cmdTopo = New System.Windows.Forms.Button()
			Me.cmdIntersect = New System.Windows.Forms.Button()
			Me.cmdClearMapLayers = New System.Windows.Forms.Button()
			Me.cmdLoadEntPoints = New System.Windows.Forms.Button()
			Me.cmdLoadParcels = New System.Windows.Forms.Button()
			Me.cmdTest = New System.Windows.Forms.Button()
			Me.cmdLoadEntLines = New System.Windows.Forms.Button()
			Me.cmdLoadEntAreas = New System.Windows.Forms.Button()
			Me.dgvMain = New System.Windows.Forms.DataGridView()
			Me.ctxBlockNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxBlockAddNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxParcelNo = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxLayerDescription = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ccbGeometricType = New System.Windows.Forms.DataGridViewComboBoxColumn()
			Me.ctxGeometricType = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxEntValueIn = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxEntCountIn = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxEntValueOut = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.ctxEntCountOut = New System.Windows.Forms.DataGridViewTextBoxColumn()
			Me.cmdIntersectLines = New System.Windows.Forms.Button()
			Me.cmdLoadEnt = New System.Windows.Forms.Button()
			Me.cmdLoadAll = New System.Windows.Forms.Button()
			Me.cmdTopoPlus = New System.Windows.Forms.Button()
			Me.cmdClear = New System.Windows.Forms.Button()
			Me.Button1 = New System.Windows.Forms.Button()
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'cmdDissolve
			'
			Me.cmdDissolve.Enabled = False
			Me.cmdDissolve.Location = New System.Drawing.Point(648, 2)
			Me.cmdDissolve.Name = "cmdDissolve"
			Me.cmdDissolve.Size = New System.Drawing.Size(61, 23)
			Me.cmdDissolve.TabIndex = 3
			Me.cmdDissolve.Text = "Dissolve"
			Me.cmdDissolve.UseVisualStyleBackColor = True
			'
			'cmdUnion
			'
			Me.cmdUnion.ForeColor = System.Drawing.SystemColors.ActiveCaption
			Me.cmdUnion.Location = New System.Drawing.Point(648, 26)
			Me.cmdUnion.Name = "cmdUnion"
			Me.cmdUnion.Size = New System.Drawing.Size(61, 23)
			Me.cmdUnion.TabIndex = 5
			Me.cmdUnion.Text = "Union3"
			Me.cmdUnion.UseVisualStyleBackColor = True
			Me.cmdUnion.Visible = False
			'
			'cmdClip
			'
			Me.cmdClip.Location = New System.Drawing.Point(648, 55)
			Me.cmdClip.Name = "cmdClip"
			Me.cmdClip.Size = New System.Drawing.Size(61, 23)
			Me.cmdClip.TabIndex = 6
			Me.cmdClip.Text = "Clip6"
			Me.cmdClip.UseVisualStyleBackColor = True
			Me.cmdClip.Visible = False
			'
			'cmdParcExMapLayer
			'
			Me.cmdParcExMapLayer.ForeColor = System.Drawing.SystemColors.AppWorkspace
			Me.cmdParcExMapLayer.Location = New System.Drawing.Point(391, -3)
			Me.cmdParcExMapLayer.Name = "cmdParcExMapLayer"
			Me.cmdParcExMapLayer.Size = New System.Drawing.Size(110, 23)
			Me.cmdParcExMapLayer.TabIndex = 7
			Me.cmdParcExMapLayer.Text = "ParcExMapLayer"
			Me.cmdParcExMapLayer.UseVisualStyleBackColor = True
			Me.cmdParcExMapLayer.Visible = False
			'
			'cmdEntAreaMapLayer
			'
			Me.cmdEntAreaMapLayer.ForeColor = System.Drawing.SystemColors.AppWorkspace
			Me.cmdEntAreaMapLayer.Location = New System.Drawing.Point(391, 26)
			Me.cmdEntAreaMapLayer.Name = "cmdEntAreaMapLayer"
			Me.cmdEntAreaMapLayer.Size = New System.Drawing.Size(110, 23)
			Me.cmdEntAreaMapLayer.TabIndex = 8
			Me.cmdEntAreaMapLayer.Text = "EntAreaMapLayer"
			Me.cmdEntAreaMapLayer.UseVisualStyleBackColor = True
			Me.cmdEntAreaMapLayer.Visible = False
			'
			'cmdLoadModel
			'
			Me.cmdLoadModel.ForeColor = System.Drawing.SystemColors.AppWorkspace
			Me.cmdLoadModel.Location = New System.Drawing.Point(550, 2)
			Me.cmdLoadModel.Name = "cmdLoadModel"
			Me.cmdLoadModel.Size = New System.Drawing.Size(70, 23)
			Me.cmdLoadModel.TabIndex = 9
			Me.cmdLoadModel.Text = "LoadEnt"
			Me.cmdLoadModel.UseVisualStyleBackColor = True
			Me.cmdLoadModel.Visible = False
			'
			'cmdTopo
			'
			Me.cmdTopo.ForeColor = System.Drawing.SystemColors.AppWorkspace
			Me.cmdTopo.Location = New System.Drawing.Point(550, 26)
			Me.cmdTopo.Name = "cmdTopo"
			Me.cmdTopo.Size = New System.Drawing.Size(70, 23)
			Me.cmdTopo.TabIndex = 10
			Me.cmdTopo.Text = "Topo"
			Me.cmdTopo.UseVisualStyleBackColor = True
			Me.cmdTopo.Visible = False
			'
			'cmdIntersect
			'
			Me.cmdIntersect.Location = New System.Drawing.Point(408, 50)
			Me.cmdIntersect.Name = "cmdIntersect"
			Me.cmdIntersect.Size = New System.Drawing.Size(93, 23)
			Me.cmdIntersect.TabIndex = 12
			Me.cmdIntersect.Text = "Intersection"
			Me.cmdIntersect.UseVisualStyleBackColor = True
			Me.cmdIntersect.Visible = False
			'
			'cmdClearMapLayers
			'
			Me.cmdClearMapLayers.Location = New System.Drawing.Point(408, 74)
			Me.cmdClearMapLayers.Name = "cmdClearMapLayers"
			Me.cmdClearMapLayers.Size = New System.Drawing.Size(93, 23)
			Me.cmdClearMapLayers.TabIndex = 13
			Me.cmdClearMapLayers.Text = "ClearMapLayers"
			Me.cmdClearMapLayers.UseVisualStyleBackColor = True
			Me.cmdClearMapLayers.Visible = False
			'
			'cmdLoadEntPoints
			'
			Me.cmdLoadEntPoints.AllowDrop = True
			Me.cmdLoadEntPoints.ForeColor = System.Drawing.SystemColors.ActiveCaption
			Me.cmdLoadEntPoints.Location = New System.Drawing.Point(227, 26)
			Me.cmdLoadEntPoints.Name = "cmdLoadEntPoints"
			Me.cmdLoadEntPoints.Size = New System.Drawing.Size(91, 23)
			Me.cmdLoadEntPoints.TabIndex = 14
			Me.cmdLoadEntPoints.Text = "LoadEntPoints"
			Me.cmdLoadEntPoints.UseVisualStyleBackColor = True
			Me.cmdLoadEntPoints.Visible = False
			'
			'cmdLoadParcels
			'
			Me.cmdLoadParcels.Location = New System.Drawing.Point(307, 55)
			Me.cmdLoadParcels.Name = "cmdLoadParcels"
			Me.cmdLoadParcels.Size = New System.Drawing.Size(78, 23)
			Me.cmdLoadParcels.TabIndex = 15
			Me.cmdLoadParcels.Text = "LoadParcels"
			Me.cmdLoadParcels.UseVisualStyleBackColor = True
			Me.cmdLoadParcels.Visible = False
			'
			'cmdTest
			'
			Me.cmdTest.Location = New System.Drawing.Point(176, 2)
			Me.cmdTest.Name = "cmdTest"
			Me.cmdTest.Size = New System.Drawing.Size(45, 23)
			Me.cmdTest.TabIndex = 16
			Me.cmdTest.Text = "Test"
			Me.cmdTest.UseVisualStyleBackColor = True
			'
			'cmdLoadEntLines
			'
			Me.cmdLoadEntLines.AllowDrop = True
			Me.cmdLoadEntLines.Location = New System.Drawing.Point(197, 50)
			Me.cmdLoadEntLines.Name = "cmdLoadEntLines"
			Me.cmdLoadEntLines.Size = New System.Drawing.Size(91, 23)
			Me.cmdLoadEntLines.TabIndex = 17
			Me.cmdLoadEntLines.Text = "LoadEntLines"
			Me.cmdLoadEntLines.UseVisualStyleBackColor = True
			Me.cmdLoadEntLines.Visible = False
			'
			'cmdLoadEntAreas
			'
			Me.cmdLoadEntAreas.AllowDrop = True
			Me.cmdLoadEntAreas.Location = New System.Drawing.Point(197, 74)
			Me.cmdLoadEntAreas.Name = "cmdLoadEntAreas"
			Me.cmdLoadEntAreas.Size = New System.Drawing.Size(91, 23)
			Me.cmdLoadEntAreas.TabIndex = 18
			Me.cmdLoadEntAreas.Text = "LoadEntAreas"
			Me.cmdLoadEntAreas.UseVisualStyleBackColor = True
			Me.cmdLoadEntAreas.Visible = False
			'
			'dgvMain
			'
			Me.dgvMain.AllowUserToAddRows = False
			Me.dgvMain.AllowUserToDeleteRows = False
			DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))
			Me.dgvMain.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
			Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxBlockNo, Me.ctxBlockAddNo, Me.ctxParcelNo, Me.ctxLayerDescription, Me.ccbGeometricType, Me.ctxGeometricType, Me.ctxEntValueIn, Me.ctxEntCountIn, Me.ctxEntValueOut, Me.ctxEntCountOut})
			Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.dgvMain.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
			Me.dgvMain.Location = New System.Drawing.Point(0, 50)
			Me.dgvMain.Name = "dgvMain"
			Me.dgvMain.ReadOnly = True
			Me.dgvMain.RowHeadersWidth = 23
			Me.dgvMain.Size = New System.Drawing.Size(717, 502)
			Me.dgvMain.TabIndex = 19
			'
			'ctxBlockNo
			'
			Me.ctxBlockNo.DataPropertyName = "BlockNo"
			DataGridViewCellStyle2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.ctxBlockNo.DefaultCellStyle = DataGridViewCellStyle2
			Me.ctxBlockNo.HeaderText = "גוש"
			Me.ctxBlockNo.Name = "ctxBlockNo"
			Me.ctxBlockNo.ReadOnly = True
			Me.ctxBlockNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxBlockNo.Width = 60
			'
			'ctxBlockAddNo
			'
			Me.ctxBlockAddNo.DataPropertyName = "BlockAddNo"
			Me.ctxBlockAddNo.HeaderText = "ת"
			Me.ctxBlockAddNo.Name = "ctxBlockAddNo"
			Me.ctxBlockAddNo.ReadOnly = True
			Me.ctxBlockAddNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxBlockAddNo.Visible = False
			Me.ctxBlockAddNo.Width = 5
			'
			'ctxParcelNo
			'
			Me.ctxParcelNo.DataPropertyName = "ParcelNo"
			DataGridViewCellStyle3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.ctxParcelNo.DefaultCellStyle = DataGridViewCellStyle3
			Me.ctxParcelNo.DividerWidth = 2
			Me.ctxParcelNo.HeaderText = "חלקה"
			Me.ctxParcelNo.Name = "ctxParcelNo"
			Me.ctxParcelNo.ReadOnly = True
			Me.ctxParcelNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxParcelNo.Width = 60
			'
			'ctxLayerDescription
			'
			Me.ctxLayerDescription.DataPropertyName = "LayerDescription"
			Me.ctxLayerDescription.DividerWidth = 1
			Me.ctxLayerDescription.HeaderText = "סוג מחוברים"
			Me.ctxLayerDescription.Name = "ctxLayerDescription"
			Me.ctxLayerDescription.ReadOnly = True
			Me.ctxLayerDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			'
			'ccbGeometricType
			'
			Me.ccbGeometricType.DataPropertyName = "GeometricType"
			Me.ccbGeometricType.HeaderText = "סוג גאומ'"
			Me.ccbGeometricType.Name = "ccbGeometricType"
			Me.ccbGeometricType.ReadOnly = True
			Me.ccbGeometricType.Width = 72
			'
			'ctxGeometricType
			'
			Me.ctxGeometricType.DataPropertyName = "GeometricTypeName"
			Me.ctxGeometricType.HeaderText = "סוג גאומ'"
			Me.ctxGeometricType.Name = "ctxGeometricType"
			Me.ctxGeometricType.ReadOnly = True
			'
			'ctxEntValueIn
			'
			Me.ctxEntValueIn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
			Me.ctxEntValueIn.DataPropertyName = "EntValueIn"
			DataGridViewCellStyle4.Format = "N0"
			DataGridViewCellStyle4.NullValue = Nothing
			Me.ctxEntValueIn.DefaultCellStyle = DataGridViewCellStyle4
			Me.ctxEntValueIn.HeaderText = "שטח מחוברים"
			Me.ctxEntValueIn.Name = "ctxEntValueIn"
			Me.ctxEntValueIn.ReadOnly = True
			Me.ctxEntValueIn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxEntValueIn.Width = 64
			'
			'ctxEntCountIn
			'
			Me.ctxEntCountIn.DataPropertyName = "EntCountIn"
			DataGridViewCellStyle5.Format = "N0"
			DataGridViewCellStyle5.NullValue = Nothing
			Me.ctxEntCountIn.DefaultCellStyle = DataGridViewCellStyle5
			Me.ctxEntCountIn.DividerWidth = 1
			Me.ctxEntCountIn.HeaderText = "כמות"
			Me.ctxEntCountIn.Name = "ctxEntCountIn"
			Me.ctxEntCountIn.ReadOnly = True
			Me.ctxEntCountIn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxEntCountIn.Width = 80
			'
			'ctxEntValueOut
			'
			Me.ctxEntValueOut.DataPropertyName = "EntValueOut"
			DataGridViewCellStyle6.Format = "N0"
			DataGridViewCellStyle6.NullValue = Nothing
			Me.ctxEntValueOut.DefaultCellStyle = DataGridViewCellStyle6
			Me.ctxEntValueOut.HeaderText = "שטח מחוברים"
			Me.ctxEntValueOut.Name = "ctxEntValueOut"
			Me.ctxEntValueOut.ReadOnly = True
			Me.ctxEntValueOut.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxEntValueOut.Width = 64
			'
			'ctxEntCountOut
			'
			Me.ctxEntCountOut.DataPropertyName = "EntCountOut"
			DataGridViewCellStyle7.Format = "N0"
			DataGridViewCellStyle7.NullValue = Nothing
			Me.ctxEntCountOut.DefaultCellStyle = DataGridViewCellStyle7
			Me.ctxEntCountOut.HeaderText = "כמות"
			Me.ctxEntCountOut.Name = "ctxEntCountOut"
			Me.ctxEntCountOut.ReadOnly = True
			Me.ctxEntCountOut.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
			Me.ctxEntCountOut.Width = 64
			'
			'cmdIntersectLines
			'
			Me.cmdIntersectLines.Location = New System.Drawing.Point(536, 55)
			Me.cmdIntersectLines.Name = "cmdIntersectLines"
			Me.cmdIntersectLines.Size = New System.Drawing.Size(84, 23)
			Me.cmdIntersectLines.TabIndex = 20
			Me.cmdIntersectLines.Text = "Inters. Lines"
			Me.cmdIntersectLines.UseVisualStyleBackColor = True
			Me.cmdIntersectLines.Visible = False
			'
			'cmdLoadEnt
			'
			Me.cmdLoadEnt.ForeColor = System.Drawing.SystemColors.ActiveCaption
			Me.cmdLoadEnt.Location = New System.Drawing.Point(248, 2)
			Me.cmdLoadEnt.Name = "cmdLoadEnt"
			Me.cmdLoadEnt.Size = New System.Drawing.Size(70, 23)
			Me.cmdLoadEnt.TabIndex = 21
			Me.cmdLoadEnt.Text = "LoadEnt"
			Me.cmdLoadEnt.UseVisualStyleBackColor = True
			Me.cmdLoadEnt.Visible = False
			'
			'cmdLoadAll
			'
			Me.cmdLoadAll.Location = New System.Drawing.Point(87, 24)
			Me.cmdLoadAll.Name = "cmdLoadAll"
			Me.cmdLoadAll.Size = New System.Drawing.Size(70, 23)
			Me.cmdLoadAll.TabIndex = 22
			Me.cmdLoadAll.Text = "Load2"
			Me.cmdLoadAll.UseVisualStyleBackColor = True
			'
			'cmdTopoPlus
			'
			Me.cmdTopoPlus.Location = New System.Drawing.Point(11, 2)
			Me.cmdTopoPlus.Name = "cmdTopoPlus"
			Me.cmdTopoPlus.Size = New System.Drawing.Size(70, 23)
			Me.cmdTopoPlus.TabIndex = 23
			Me.cmdTopoPlus.Text = "Topo"
			Me.cmdTopoPlus.UseVisualStyleBackColor = True
			'
			'cmdClear
			'
			Me.cmdClear.Enabled = False
			Me.cmdClear.Location = New System.Drawing.Point(324, 2)
			Me.cmdClear.Name = "cmdClear"
			Me.cmdClear.Size = New System.Drawing.Size(45, 23)
			Me.cmdClear.TabIndex = 24
			Me.cmdClear.Text = "Clear"
			Me.cmdClear.UseVisualStyleBackColor = True
			'
			'Button1
			'
			Me.Button1.Location = New System.Drawing.Point(87, 0)
			Me.Button1.Name = "Button1"
			Me.Button1.Size = New System.Drawing.Size(70, 23)
			Me.Button1.TabIndex = 25
			Me.Button1.Text = "Load"
			Me.Button1.UseVisualStyleBackColor = True
			'
			'frmEntConnected
			'
			Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.ClientSize = New System.Drawing.Size(717, 552)
			Me.Controls.Add(Me.Button1)
			Me.Controls.Add(Me.cmdClear)
			Me.Controls.Add(Me.cmdTopoPlus)
			Me.Controls.Add(Me.cmdLoadAll)
			Me.Controls.Add(Me.cmdLoadEnt)
			Me.Controls.Add(Me.cmdIntersectLines)
			Me.Controls.Add(Me.dgvMain)
			Me.Controls.Add(Me.cmdLoadEntAreas)
			Me.Controls.Add(Me.cmdLoadEntLines)
			Me.Controls.Add(Me.cmdTest)
			Me.Controls.Add(Me.cmdLoadParcels)
			Me.Controls.Add(Me.cmdLoadEntPoints)
			Me.Controls.Add(Me.cmdClearMapLayers)
			Me.Controls.Add(Me.cmdIntersect)
			Me.Controls.Add(Me.cmdTopo)
			Me.Controls.Add(Me.cmdLoadModel)
			Me.Controls.Add(Me.cmdEntAreaMapLayer)
			Me.Controls.Add(Me.cmdParcExMapLayer)
			Me.Controls.Add(Me.cmdClip)
			Me.Controls.Add(Me.cmdUnion)
			Me.Controls.Add(Me.cmdDissolve)
			Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.Name = "frmEntConnected"
			Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.RightToLeftLayout = True
			Me.Text = "מחוברים"
			CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub
		Friend WithEvents cmdUnion As Button
		Private WithEvents cmdClip As Button
		Private WithEvents cmdEntAreaMapLayer As Button
		Private WithEvents cmdParcExMapLayer As Button
		Private WithEvents cmdIntersect As Button
		Private WithEvents cmdClearMapLayers As Button
		Private WithEvents cmdLoadEntPoints As Button
		Private WithEvents cmdLoadParcels As Button
		Private WithEvents cmdTest As Button
		Private WithEvents cmdLoadEntLines As Button
		Private WithEvents cmdLoadEntAreas As Button
		Friend WithEvents dgvMain As DataGridView
		Private WithEvents cmdTopo As Button
		Private WithEvents cmdIntersectLines As Button
		Private WithEvents cmdLoadEnt As Button
		Private WithEvents cmdLoadModel As Button
		Private WithEvents cmdLoadAll As Button
		Private WithEvents cmdTopoPlus As Button
		Friend WithEvents ctxBlockNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxBlockAddNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxParcelNo As DataGridViewTextBoxColumn
		Friend WithEvents ctxLayerDescription As DataGridViewTextBoxColumn
		Friend WithEvents ccbGeometricType As DataGridViewComboBoxColumn
		Friend WithEvents ctxGeometricType As DataGridViewTextBoxColumn
		Friend WithEvents ctxEntValueIn As DataGridViewTextBoxColumn
		Friend WithEvents ctxEntCountIn As DataGridViewTextBoxColumn
		Friend WithEvents ctxEntValueOut As DataGridViewTextBoxColumn
		Friend WithEvents ctxEntCountOut As DataGridViewTextBoxColumn
		Private WithEvents cmdDissolve As Button
		Private WithEvents cmdClear As Button
		Private WithEvents Button1 As Button
	End Class
End Namespace