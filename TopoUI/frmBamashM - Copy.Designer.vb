<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBamashM
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBamashM))
		Me.cmbAreaFormat = New System.Windows.Forms.ComboBox()
		Me.cmdCalculate = New System.Windows.Forms.Button()
		Me.cmdBamashPaint = New System.Windows.Forms.Button()
		Me.lstReports = New System.Windows.Forms.ListBox()
		Me.cmdInsertRep = New System.Windows.Forms.Button()
		Me.cmdExpExcel = New System.Windows.Forms.Button()
		Me.txtZebraWidthDrawing = New System.Windows.Forms.TextBox()
		Me.txtZebraWidthTable = New System.Windows.Forms.TextBox()
		Me.txtBufferOffset = New System.Windows.Forms.TextBox()
		Me.txtZoomRadius = New System.Windows.Forms.TextBox()
		Me.txtRepBamashSharedScale = New System.Windows.Forms.TextBox()
		Me.lblZebraWidthDrawing = New System.Windows.Forms.Label()
		Me.lblZebraWidthTable = New System.Windows.Forms.Label()
		Me.lblBufferOffset = New System.Windows.Forms.Label()
		Me.lblZoomRadius = New System.Windows.Forms.Label()
		Me.lblRepBamashSharedScale = New System.Windows.Forms.Label()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtPropID = New System.Windows.Forms.TextBox()
		Me.cmbChoiceColor = New System.Windows.Forms.ComboBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.cmbPropertyType = New System.Windows.Forms.ComboBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.cmbAprtDesc = New System.Windows.Forms.ComboBox()
		Me.txtBldFloor = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.txtBldEntr = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.txtBldPart = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtBldNo = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.cmdOpenBlockRefTable = New System.Windows.Forms.Button()
		Me.cmdSelectByPick = New System.Windows.Forms.Button()
		Me.chkBldFloor = New System.Windows.Forms.CheckBox()
		Me.txtSubpropNum = New System.Windows.Forms.TextBox()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.chkBldPart = New System.Windows.Forms.CheckBox()
		Me.chkBldEntr = New System.Windows.Forms.CheckBox()
		Me.chkPropID = New System.Windows.Forms.CheckBox()
		Me.cmbNumerationPropID = New System.Windows.Forms.ComboBox()
		Me.chkBldNo = New System.Windows.Forms.CheckBox()
		Me.chkColor = New System.Windows.Forms.CheckBox()
		Me.chkSubproperty = New System.Windows.Forms.CheckBox()
		Me.chkPropertyType = New System.Windows.Forms.CheckBox()
		Me.chkAprtDesc = New System.Windows.Forms.CheckBox()
		Me.txtMaxPropertyID = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.txtMinPropertyID = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.cmdSetDataBySelSet = New System.Windows.Forms.Button()
		Me.cmdSetDataByPick = New System.Windows.Forms.Button()
		Me.txtMaxSubpropNum = New System.Windows.Forms.TextBox()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.txtMinSubpropNum = New System.Windows.Forms.TextBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.lblAreaFormat = New System.Windows.Forms.Label()
		Me.txtParcelNo = New System.Windows.Forms.TextBox()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.tabMain = New System.Windows.Forms.TabControl()
		Me.tpgEditBloks = New System.Windows.Forms.TabPage()
		Me.cmdSelectBlocks = New System.Windows.Forms.Button()
		Me.Label15 = New System.Windows.Forms.Label()
		Me.tpgPaint = New System.Windows.Forms.TabPage()
		Me.cmdClearPaint = New System.Windows.Forms.Button()
		Me.tpgReport = New System.Windows.Forms.TabPage()
		Me.cmdRecomputeTable = New System.Windows.Forms.Button()
		Me.cmdGetDistSpacing = New System.Windows.Forms.Button()
		Me.cmdGetDistBreakHeight = New System.Windows.Forms.Button()
		Me.txtSpacing = New System.Windows.Forms.TextBox()
		Me.Label17 = New System.Windows.Forms.Label()
		Me.txtBreakHeight = New System.Windows.Forms.TextBox()
		Me.Label16 = New System.Windows.Forms.Label()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.tabMain.SuspendLayout()
		Me.tpgEditBloks.SuspendLayout()
		Me.tpgPaint.SuspendLayout()
		Me.tpgReport.SuspendLayout()
		Me.SuspendLayout()
		'
		'cmbAreaFormat
		'
		Me.cmbAreaFormat.FormattingEnabled = True
		Me.cmbAreaFormat.Location = New System.Drawing.Point(202, 200)
		Me.cmbAreaFormat.Name = "cmbAreaFormat"
		Me.cmbAreaFormat.Size = New System.Drawing.Size(72, 22)
		Me.cmbAreaFormat.TabIndex = 8
		'
		'cmdCalculate
		'
		Me.cmdCalculate.AutoSize = True
		Me.cmdCalculate.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdCalculate.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
		Me.cmdCalculate.FlatAppearance.BorderSize = 0
		Me.cmdCalculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdCalculate.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdCalculate.Location = New System.Drawing.Point(141, 8)
		Me.cmdCalculate.Name = "cmdCalculate"
		Me.cmdCalculate.Size = New System.Drawing.Size(61, 24)
		Me.cmdCalculate.TabIndex = 1
		Me.cmdCalculate.Text = "חישוב"
		Me.cmdCalculate.UseVisualStyleBackColor = True
		'
		'cmdBamashPaint
		'
		Me.cmdBamashPaint.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdBamashPaint.FlatAppearance.BorderSize = 0
		Me.cmdBamashPaint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdBamashPaint.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdBamashPaint.Location = New System.Drawing.Point(192, 20)
		Me.cmdBamashPaint.Name = "cmdBamashPaint"
		Me.cmdBamashPaint.Size = New System.Drawing.Size(93, 26)
		Me.cmdBamashPaint.TabIndex = 4
		Me.cmdBamashPaint.Text = "צביעת במ""ש"
		Me.cmdBamashPaint.UseVisualStyleBackColor = True
		'
		'lstReports
		'
		Me.lstReports.FormattingEnabled = True
		Me.lstReports.ItemHeight = 14
		Me.lstReports.Location = New System.Drawing.Point(216, 17)
		Me.lstReports.Name = "lstReports"
		Me.lstReports.Size = New System.Drawing.Size(137, 214)
		Me.lstReports.TabIndex = 5
		'
		'cmdInsertRep
		'
		Me.cmdInsertRep.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdInsertRep.FlatAppearance.BorderSize = 0
		Me.cmdInsertRep.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdInsertRep.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdInsertRep.Location = New System.Drawing.Point(294, 248)
		Me.cmdInsertRep.Name = "cmdInsertRep"
		Me.cmdInsertRep.Size = New System.Drawing.Size(59, 26)
		Me.cmdInsertRep.TabIndex = 6
		Me.cmdInsertRep.Text = "Insert"
		Me.cmdInsertRep.UseVisualStyleBackColor = True
		'
		'cmdExpExcel
		'
		Me.cmdExpExcel.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdExpExcel.FlatAppearance.BorderSize = 0
		Me.cmdExpExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExpExcel.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdExpExcel.Location = New System.Drawing.Point(216, 246)
		Me.cmdExpExcel.Name = "cmdExpExcel"
		Me.cmdExpExcel.Size = New System.Drawing.Size(59, 28)
		Me.cmdExpExcel.TabIndex = 7
		Me.cmdExpExcel.Text = "Excel"
		Me.cmdExpExcel.UseVisualStyleBackColor = True
		'
		'txtZebraWidthDrawing
		'
		Me.txtZebraWidthDrawing.Location = New System.Drawing.Point(160, 72)
		Me.txtZebraWidthDrawing.Name = "txtZebraWidthDrawing"
		Me.txtZebraWidthDrawing.Size = New System.Drawing.Size(114, 22)
		Me.txtZebraWidthDrawing.TabIndex = 1
		'
		'txtZebraWidthTable
		'
		Me.txtZebraWidthTable.Location = New System.Drawing.Point(160, 104)
		Me.txtZebraWidthTable.Name = "txtZebraWidthTable"
		Me.txtZebraWidthTable.Size = New System.Drawing.Size(114, 22)
		Me.txtZebraWidthTable.TabIndex = 11
		'
		'txtBufferOffset
		'
		Me.txtBufferOffset.Location = New System.Drawing.Point(160, 136)
		Me.txtBufferOffset.Name = "txtBufferOffset"
		Me.txtBufferOffset.Size = New System.Drawing.Size(114, 22)
		Me.txtBufferOffset.TabIndex = 12
		'
		'txtZoomRadius
		'
		Me.txtZoomRadius.Location = New System.Drawing.Point(63, 380)
		Me.txtZoomRadius.Name = "txtZoomRadius"
		Me.txtZoomRadius.Size = New System.Drawing.Size(76, 22)
		Me.txtZoomRadius.TabIndex = 13
		Me.txtZoomRadius.Visible = False
		'
		'txtRepBamashSharedScale
		'
		Me.txtRepBamashSharedScale.Location = New System.Drawing.Point(160, 168)
		Me.txtRepBamashSharedScale.Name = "txtRepBamashSharedScale"
		Me.txtRepBamashSharedScale.Size = New System.Drawing.Size(114, 22)
		Me.txtRepBamashSharedScale.TabIndex = 14
		'
		'lblZebraWidthDrawing
		'
		Me.lblZebraWidthDrawing.Location = New System.Drawing.Point(280, 72)
		Me.lblZebraWidthDrawing.Name = "lblZebraWidthDrawing"
		Me.lblZebraWidthDrawing.Size = New System.Drawing.Size(60, 14)
		Me.lblZebraWidthDrawing.TabIndex = 15
		Me.lblZebraWidthDrawing.Text = "Label1"
		'
		'lblZebraWidthTable
		'
		Me.lblZebraWidthTable.Location = New System.Drawing.Point(280, 104)
		Me.lblZebraWidthTable.Name = "lblZebraWidthTable"
		Me.lblZebraWidthTable.Size = New System.Drawing.Size(60, 14)
		Me.lblZebraWidthTable.TabIndex = 16
		Me.lblZebraWidthTable.Text = "Label1"
		'
		'lblBufferOffset
		'
		Me.lblBufferOffset.Location = New System.Drawing.Point(280, 136)
		Me.lblBufferOffset.Name = "lblBufferOffset"
		Me.lblBufferOffset.Size = New System.Drawing.Size(60, 14)
		Me.lblBufferOffset.TabIndex = 17
		Me.lblBufferOffset.Text = "Label1"
		'
		'lblZoomRadius
		'
		Me.lblZoomRadius.AutoSize = True
		Me.lblZoomRadius.Location = New System.Drawing.Point(15, 384)
		Me.lblZoomRadius.Name = "lblZoomRadius"
		Me.lblZoomRadius.Size = New System.Drawing.Size(42, 14)
		Me.lblZoomRadius.TabIndex = 18
		Me.lblZoomRadius.Text = "Label1"
		Me.lblZoomRadius.Visible = False
		'
		'lblRepBamashSharedScale
		'
		Me.lblRepBamashSharedScale.Location = New System.Drawing.Point(280, 168)
		Me.lblRepBamashSharedScale.Name = "lblRepBamashSharedScale"
		Me.lblRepBamashSharedScale.Size = New System.Drawing.Size(60, 14)
		Me.lblRepBamashSharedScale.TabIndex = 19
		Me.lblRepBamashSharedScale.Text = "Label1"
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(278, 8)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(78, 28)
		Me.Label1.TabIndex = 20
		Me.Label1.Text = "מס' יחידת משנה"
		'
		'txtPropID
		'
		Me.txtPropID.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtPropID.Location = New System.Drawing.Point(230, 12)
		Me.txtPropID.Name = "txtPropID"
		Me.txtPropID.Size = New System.Drawing.Size(48, 21)
		Me.txtPropID.TabIndex = 21
		'
		'cmbChoiceColor
		'
		Me.cmbChoiceColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable
		Me.cmbChoiceColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cmbChoiceColor.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmbChoiceColor.FormattingEnabled = True
		Me.cmbChoiceColor.Location = New System.Drawing.Point(181, 66)
		Me.cmbChoiceColor.Name = "cmbChoiceColor"
		Me.cmbChoiceColor.Size = New System.Drawing.Size(97, 22)
		Me.cmbChoiceColor.TabIndex = 22
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(327, 66)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(31, 14)
		Me.Label2.TabIndex = 23
		Me.Label2.Text = "צבע"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(308, 122)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(50, 14)
		Me.Label3.TabIndex = 25
		Me.Label3.Text = "סוג נכס"
		'
		'cmbPropertyType
		'
		Me.cmbPropertyType.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmbPropertyType.FormattingEnabled = True
		Me.cmbPropertyType.Location = New System.Drawing.Point(108, 122)
		Me.cmbPropertyType.Name = "cmbPropertyType"
		Me.cmbPropertyType.Size = New System.Drawing.Size(170, 21)
		Me.cmbPropertyType.TabIndex = 24
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(290, 150)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(68, 14)
		Me.Label4.TabIndex = 27
		Me.Label4.Text = "תיאור דירה"
		'
		'cmbAprtDesc
		'
		Me.cmbAprtDesc.FormattingEnabled = True
		Me.cmbAprtDesc.Location = New System.Drawing.Point(108, 150)
		Me.cmbAprtDesc.Name = "cmbAprtDesc"
		Me.cmbAprtDesc.Size = New System.Drawing.Size(170, 22)
		Me.cmbAprtDesc.TabIndex = 26
		'
		'txtBldFloor
		'
		Me.txtBldFloor.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldFloor.Location = New System.Drawing.Point(230, 178)
		Me.txtBldFloor.Name = "txtBldFloor"
		Me.txtBldFloor.Size = New System.Drawing.Size(48, 21)
		Me.txtBldFloor.TabIndex = 29
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(324, 178)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(34, 14)
		Me.Label5.TabIndex = 28
		Me.Label5.Text = "קומה"
		'
		'txtBldEntr
		'
		Me.txtBldEntr.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldEntr.Location = New System.Drawing.Point(230, 206)
		Me.txtBldEntr.Name = "txtBldEntr"
		Me.txtBldEntr.Size = New System.Drawing.Size(48, 21)
		Me.txtBldEntr.TabIndex = 31
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(319, 206)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(39, 14)
		Me.Label6.TabIndex = 30
		Me.Label6.Text = "כניסה"
		'
		'txtBldPart
		'
		Me.txtBldPart.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldPart.Location = New System.Drawing.Point(230, 234)
		Me.txtBldPart.Name = "txtBldPart"
		Me.txtBldPart.Size = New System.Drawing.Size(48, 21)
		Me.txtBldPart.TabIndex = 33
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(305, 234)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(53, 14)
		Me.Label7.TabIndex = 32
		Me.Label7.Text = "מס' אגף"
		'
		'txtBldNo
		'
		Me.txtBldNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldNo.Location = New System.Drawing.Point(230, 262)
		Me.txtBldNo.Name = "txtBldNo"
		Me.txtBldNo.Size = New System.Drawing.Size(48, 21)
		Me.txtBldNo.TabIndex = 35
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(305, 262)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(53, 14)
		Me.Label8.TabIndex = 34
		Me.Label8.Text = "מס' בית"
		'
		'cmdOpenBlockRefTable
		'
		Me.cmdOpenBlockRefTable.BackColor = System.Drawing.SystemColors.Control
		Me.cmdOpenBlockRefTable.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdOpenBlockRefTable.FlatAppearance.BorderSize = 0
		Me.cmdOpenBlockRefTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdOpenBlockRefTable.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdOpenBlockRefTable.Location = New System.Drawing.Point(214, 8)
		Me.cmdOpenBlockRefTable.Name = "cmdOpenBlockRefTable"
		Me.cmdOpenBlockRefTable.Size = New System.Drawing.Size(61, 24)
		Me.cmdOpenBlockRefTable.TabIndex = 36
		Me.cmdOpenBlockRefTable.Text = "טבלה"
		Me.cmdOpenBlockRefTable.UseVisualStyleBackColor = False
		'
		'cmdSelectByPick
		'
		Me.cmdSelectByPick.AutoSize = True
		Me.cmdSelectByPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSelectByPick.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdSelectByPick.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdSelectByPick.FlatAppearance.BorderSize = 0
		Me.cmdSelectByPick.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSelectByPick.Image = CType(resources.GetObject("cmdSelectByPick.Image"), System.Drawing.Image)
		Me.cmdSelectByPick.Location = New System.Drawing.Point(8, 8)
		Me.cmdSelectByPick.Name = "cmdSelectByPick"
		Me.cmdSelectByPick.Size = New System.Drawing.Size(21, 21)
		Me.cmdSelectByPick.TabIndex = 37
		Me.cmdSelectByPick.UseVisualStyleBackColor = True
		'
		'chkBldFloor
		'
		Me.chkBldFloor.AutoSize = True
		Me.chkBldFloor.Location = New System.Drawing.Point(208, 178)
		Me.chkBldFloor.Name = "chkBldFloor"
		Me.chkBldFloor.Size = New System.Drawing.Size(15, 14)
		Me.chkBldFloor.TabIndex = 38
		Me.chkBldFloor.UseVisualStyleBackColor = True
		'
		'txtSubpropNum
		'
		Me.txtSubpropNum.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtSubpropNum.Location = New System.Drawing.Point(230, 94)
		Me.txtSubpropNum.Name = "txtSubpropNum"
		Me.txtSubpropNum.Size = New System.Drawing.Size(48, 21)
		Me.txtSubpropNum.TabIndex = 40
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(288, 94)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(70, 14)
		Me.Label9.TabIndex = 39
		Me.Label9.Text = "מס' צמידות"
		'
		'chkBldPart
		'
		Me.chkBldPart.AutoSize = True
		Me.chkBldPart.Location = New System.Drawing.Point(208, 234)
		Me.chkBldPart.Name = "chkBldPart"
		Me.chkBldPart.Size = New System.Drawing.Size(15, 14)
		Me.chkBldPart.TabIndex = 41
		Me.chkBldPart.UseVisualStyleBackColor = True
		'
		'chkBldEntr
		'
		Me.chkBldEntr.AutoSize = True
		Me.chkBldEntr.Location = New System.Drawing.Point(208, 206)
		Me.chkBldEntr.Name = "chkBldEntr"
		Me.chkBldEntr.Size = New System.Drawing.Size(15, 14)
		Me.chkBldEntr.TabIndex = 42
		Me.chkBldEntr.UseVisualStyleBackColor = True
		'
		'chkPropID
		'
		Me.chkPropID.AutoSize = True
		Me.chkPropID.Location = New System.Drawing.Point(208, 12)
		Me.chkPropID.Name = "chkPropID"
		Me.chkPropID.Size = New System.Drawing.Size(15, 14)
		Me.chkPropID.TabIndex = 43
		Me.chkPropID.UseVisualStyleBackColor = True
		'
		'cmbNumerationPropID
		'
		Me.cmbNumerationPropID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cmbNumerationPropID.Font = New System.Drawing.Font("Tahoma", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmbNumerationPropID.FormattingEnabled = True
		Me.cmbNumerationPropID.Location = New System.Drawing.Point(32, 12)
		Me.cmbNumerationPropID.Name = "cmbNumerationPropID"
		Me.cmbNumerationPropID.Size = New System.Drawing.Size(148, 20)
		Me.cmbNumerationPropID.TabIndex = 44
		'
		'chkBldNo
		'
		Me.chkBldNo.AutoSize = True
		Me.chkBldNo.Location = New System.Drawing.Point(208, 262)
		Me.chkBldNo.Name = "chkBldNo"
		Me.chkBldNo.Size = New System.Drawing.Size(15, 14)
		Me.chkBldNo.TabIndex = 46
		Me.chkBldNo.UseVisualStyleBackColor = True
		'
		'chkColor
		'
		Me.chkColor.AutoSize = True
		Me.chkColor.Location = New System.Drawing.Point(160, 66)
		Me.chkColor.Name = "chkColor"
		Me.chkColor.Size = New System.Drawing.Size(15, 14)
		Me.chkColor.TabIndex = 47
		Me.chkColor.UseVisualStyleBackColor = True
		'
		'chkSubproperty
		'
		Me.chkSubproperty.AutoSize = True
		Me.chkSubproperty.Location = New System.Drawing.Point(208, 94)
		Me.chkSubproperty.Name = "chkSubproperty"
		Me.chkSubproperty.Size = New System.Drawing.Size(15, 14)
		Me.chkSubproperty.TabIndex = 48
		Me.chkSubproperty.UseVisualStyleBackColor = True
		'
		'chkPropertyType
		'
		Me.chkPropertyType.AutoSize = True
		Me.chkPropertyType.Location = New System.Drawing.Point(86, 122)
		Me.chkPropertyType.Name = "chkPropertyType"
		Me.chkPropertyType.Size = New System.Drawing.Size(15, 14)
		Me.chkPropertyType.TabIndex = 49
		Me.chkPropertyType.UseVisualStyleBackColor = True
		'
		'chkAprtDesc
		'
		Me.chkAprtDesc.AutoSize = True
		Me.chkAprtDesc.Location = New System.Drawing.Point(84, 150)
		Me.chkAprtDesc.Name = "chkAprtDesc"
		Me.chkAprtDesc.Size = New System.Drawing.Size(15, 14)
		Me.chkAprtDesc.TabIndex = 50
		Me.chkAprtDesc.UseVisualStyleBackColor = True
		'
		'txtMaxPropertyID
		'
		Me.txtMaxPropertyID.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMaxPropertyID.Location = New System.Drawing.Point(137, 64)
		Me.txtMaxPropertyID.Name = "txtMaxPropertyID"
		Me.txtMaxPropertyID.ReadOnly = True
		Me.txtMaxPropertyID.Size = New System.Drawing.Size(30, 15)
		Me.txtMaxPropertyID.TabIndex = 54
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(4, 64)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(133, 14)
		Me.Label10.TabIndex = 53
		Me.Label10.Text = "מס' יחידת משנה מרבי:"
		'
		'txtMinPropertyID
		'
		Me.txtMinPropertyID.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMinPropertyID.Location = New System.Drawing.Point(141, 40)
		Me.txtMinPropertyID.Name = "txtMinPropertyID"
		Me.txtMinPropertyID.ReadOnly = True
		Me.txtMinPropertyID.Size = New System.Drawing.Size(30, 15)
		Me.txtMinPropertyID.TabIndex = 56
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(4, 40)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(137, 14)
		Me.Label11.TabIndex = 55
		Me.Label11.Text = "מס' יחידת משנה מזערי:"
		'
		'cmdSetDataBySelSet
		'
		Me.cmdSetDataBySelSet.AutoSize = True
		Me.cmdSetDataBySelSet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSetDataBySelSet.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdSetDataBySelSet.FlatAppearance.BorderSize = 0
		Me.cmdSetDataBySelSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSetDataBySelSet.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdSetDataBySelSet.Location = New System.Drawing.Point(2, 86)
		Me.cmdSetDataBySelSet.Name = "cmdSetDataBySelSet"
		Me.cmdSetDataBySelSet.Size = New System.Drawing.Size(90, 24)
		Me.cmdSetDataBySelSet.TabIndex = 57
		Me.cmdSetDataBySelSet.Text = "By selection"
		Me.cmdSetDataBySelSet.UseVisualStyleBackColor = True
		'
		'cmdSetDataByPick
		'
		Me.cmdSetDataByPick.AutoSize = True
		Me.cmdSetDataByPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSetDataByPick.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdSetDataByPick.FlatAppearance.BorderSize = 0
		Me.cmdSetDataByPick.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSetDataByPick.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdSetDataByPick.Location = New System.Drawing.Point(2, 54)
		Me.cmdSetDataByPick.Name = "cmdSetDataByPick"
		Me.cmdSetDataByPick.Size = New System.Drawing.Size(60, 24)
		Me.cmdSetDataByPick.TabIndex = 58
		Me.cmdSetDataByPick.Text = "By pick"
		Me.cmdSetDataByPick.UseVisualStyleBackColor = True
		'
		'txtMaxSubpropNum
		'
		Me.txtMaxSubpropNum.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMaxSubpropNum.Location = New System.Drawing.Point(295, 64)
		Me.txtMaxSubpropNum.Name = "txtMaxSubpropNum"
		Me.txtMaxSubpropNum.ReadOnly = True
		Me.txtMaxSubpropNum.Size = New System.Drawing.Size(30, 15)
		Me.txtMaxSubpropNum.TabIndex = 63
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(186, 40)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(109, 14)
		Me.Label12.TabIndex = 62
		Me.Label12.Text = "מס' צמידות מזערי:"
		'
		'txtMinSubpropNum
		'
		Me.txtMinSubpropNum.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMinSubpropNum.Location = New System.Drawing.Point(291, 40)
		Me.txtMinSubpropNum.Name = "txtMinSubpropNum"
		Me.txtMinSubpropNum.ReadOnly = True
		Me.txtMinSubpropNum.Size = New System.Drawing.Size(30, 15)
		Me.txtMinSubpropNum.TabIndex = 61
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(186, 64)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(105, 14)
		Me.Label13.TabIndex = 60
		Me.Label13.Text = "מס' צמידות מרבי:"
		'
		'lblAreaFormat
		'
		Me.lblAreaFormat.Location = New System.Drawing.Point(280, 200)
		Me.lblAreaFormat.Name = "lblAreaFormat"
		Me.lblAreaFormat.Size = New System.Drawing.Size(60, 14)
		Me.lblAreaFormat.TabIndex = 64
		Me.lblAreaFormat.Text = "Label1"
		'
		'txtParcelNo
		'
		Me.txtParcelNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtParcelNo.Location = New System.Drawing.Point(72, 12)
		Me.txtParcelNo.Name = "txtParcelNo"
		Me.txtParcelNo.Size = New System.Drawing.Size(48, 21)
		Me.txtParcelNo.TabIndex = 66
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New System.Drawing.Point(4, 12)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(62, 14)
		Me.Label14.TabIndex = 65
		Me.Label14.Text = "מס' חלקה"
		'
		'tabMain
		'
		Me.tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
		Me.tabMain.Controls.Add(Me.tpgEditBloks)
		Me.tabMain.Controls.Add(Me.tpgPaint)
		Me.tabMain.Controls.Add(Me.tpgReport)
		Me.tabMain.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.tabMain.Location = New System.Drawing.Point(0, 105)
		Me.tabMain.Name = "tabMain"
		Me.tabMain.RightToLeftLayout = True
		Me.tabMain.SelectedIndex = 0
		Me.tabMain.Size = New System.Drawing.Size(372, 348)
		Me.tabMain.TabIndex = 67
		'
		'tpgEditBloks
		'
		Me.tpgEditBloks.Controls.Add(Me.cmdSelectBlocks)
		Me.tpgEditBloks.Controls.Add(Me.Label15)
		Me.tpgEditBloks.Controls.Add(Me.cmbAprtDesc)
		Me.tpgEditBloks.Controls.Add(Me.cmbPropertyType)
		Me.tpgEditBloks.Controls.Add(Me.Label3)
		Me.tpgEditBloks.Controls.Add(Me.Label4)
		Me.tpgEditBloks.Controls.Add(Me.Label5)
		Me.tpgEditBloks.Controls.Add(Me.txtBldFloor)
		Me.tpgEditBloks.Controls.Add(Me.Label6)
		Me.tpgEditBloks.Controls.Add(Me.cmdSetDataByPick)
		Me.tpgEditBloks.Controls.Add(Me.txtBldEntr)
		Me.tpgEditBloks.Controls.Add(Me.Label7)
		Me.tpgEditBloks.Controls.Add(Me.cmdSetDataBySelSet)
		Me.tpgEditBloks.Controls.Add(Me.txtBldPart)
		Me.tpgEditBloks.Controls.Add(Me.Label8)
		Me.tpgEditBloks.Controls.Add(Me.txtBldNo)
		Me.tpgEditBloks.Controls.Add(Me.chkBldFloor)
		Me.tpgEditBloks.Controls.Add(Me.chkBldPart)
		Me.tpgEditBloks.Controls.Add(Me.chkSubproperty)
		Me.tpgEditBloks.Controls.Add(Me.chkAprtDesc)
		Me.tpgEditBloks.Controls.Add(Me.chkColor)
		Me.tpgEditBloks.Controls.Add(Me.chkBldEntr)
		Me.tpgEditBloks.Controls.Add(Me.cmbNumerationPropID)
		Me.tpgEditBloks.Controls.Add(Me.chkPropertyType)
		Me.tpgEditBloks.Controls.Add(Me.chkPropID)
		Me.tpgEditBloks.Controls.Add(Me.chkBldNo)
		Me.tpgEditBloks.Controls.Add(Me.cmdSelectByPick)
		Me.tpgEditBloks.Controls.Add(Me.cmbChoiceColor)
		Me.tpgEditBloks.Controls.Add(Me.txtSubpropNum)
		Me.tpgEditBloks.Controls.Add(Me.Label1)
		Me.tpgEditBloks.Controls.Add(Me.Label9)
		Me.tpgEditBloks.Controls.Add(Me.txtPropID)
		Me.tpgEditBloks.Controls.Add(Me.Label2)
		Me.tpgEditBloks.Location = New System.Drawing.Point(4, 26)
		Me.tpgEditBloks.Name = "tpgEditBloks"
		Me.tpgEditBloks.Padding = New System.Windows.Forms.Padding(3)
		Me.tpgEditBloks.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.tpgEditBloks.Size = New System.Drawing.Size(364, 318)
		Me.tpgEditBloks.TabIndex = 0
		Me.tpgEditBloks.Text = "עריכת בלוקים"
		'
		'cmdSelectBlocks
		'
		Me.cmdSelectBlocks.FlatAppearance.BorderSize = 0
		Me.cmdSelectBlocks.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdSelectBlocks.Image = Global.TopoUI.My.Resources.Resources.ArrowRightDown12Tr
		Me.cmdSelectBlocks.Location = New System.Drawing.Point(92, 86)
		Me.cmdSelectBlocks.Name = "cmdSelectBlocks"
		Me.cmdSelectBlocks.Size = New System.Drawing.Size(24, 23)
		Me.cmdSelectBlocks.TabIndex = 60
		Me.cmdSelectBlocks.UseVisualStyleBackColor = True
		'
		'Label15
		'
		Me.Label15.BackColor = System.Drawing.SystemColors.ButtonHighlight
		Me.Label15.Location = New System.Drawing.Point(12, 49)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(340, 3)
		Me.Label15.TabIndex = 59
		'
		'tpgPaint
		'
		Me.tpgPaint.Controls.Add(Me.cmdClearPaint)
		Me.tpgPaint.Controls.Add(Me.cmdBamashPaint)
		Me.tpgPaint.Controls.Add(Me.txtZebraWidthDrawing)
		Me.tpgPaint.Controls.Add(Me.lblAreaFormat)
		Me.tpgPaint.Controls.Add(Me.txtZebraWidthTable)
		Me.tpgPaint.Controls.Add(Me.txtBufferOffset)
		Me.tpgPaint.Controls.Add(Me.txtRepBamashSharedScale)
		Me.tpgPaint.Controls.Add(Me.lblZebraWidthDrawing)
		Me.tpgPaint.Controls.Add(Me.cmbAreaFormat)
		Me.tpgPaint.Controls.Add(Me.lblZebraWidthTable)
		Me.tpgPaint.Controls.Add(Me.lblBufferOffset)
		Me.tpgPaint.Controls.Add(Me.lblRepBamashSharedScale)
		Me.tpgPaint.Controls.Add(Me.txtZoomRadius)
		Me.tpgPaint.Controls.Add(Me.lblZoomRadius)
		Me.tpgPaint.Location = New System.Drawing.Point(4, 26)
		Me.tpgPaint.Name = "tpgPaint"
		Me.tpgPaint.Padding = New System.Windows.Forms.Padding(3)
		Me.tpgPaint.Size = New System.Drawing.Size(364, 318)
		Me.tpgPaint.TabIndex = 1
		Me.tpgPaint.Text = "צביעה"
		'
		'cmdClearPaint
		'
		Me.cmdClearPaint.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdClearPaint.FlatAppearance.BorderSize = 0
		Me.cmdClearPaint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdClearPaint.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdClearPaint.Location = New System.Drawing.Point(66, 20)
		Me.cmdClearPaint.Name = "cmdClearPaint"
		Me.cmdClearPaint.Size = New System.Drawing.Size(93, 26)
		Me.cmdClearPaint.TabIndex = 65
		Me.cmdClearPaint.Text = "מחיקת צביעה"
		Me.cmdClearPaint.UseVisualStyleBackColor = True
		'
		'tpgReport
		'
		Me.tpgReport.Controls.Add(Me.cmdRecomputeTable)
		Me.tpgReport.Controls.Add(Me.cmdGetDistSpacing)
		Me.tpgReport.Controls.Add(Me.cmdGetDistBreakHeight)
		Me.tpgReport.Controls.Add(Me.txtSpacing)
		Me.tpgReport.Controls.Add(Me.Label17)
		Me.tpgReport.Controls.Add(Me.txtBreakHeight)
		Me.tpgReport.Controls.Add(Me.Label16)
		Me.tpgReport.Controls.Add(Me.lstReports)
		Me.tpgReport.Controls.Add(Me.cmdInsertRep)
		Me.tpgReport.Controls.Add(Me.cmdExpExcel)
		Me.tpgReport.Location = New System.Drawing.Point(4, 26)
		Me.tpgReport.Name = "tpgReport"
		Me.tpgReport.Padding = New System.Windows.Forms.Padding(3)
		Me.tpgReport.Size = New System.Drawing.Size(364, 318)
		Me.tpgReport.TabIndex = 2
		Me.tpgReport.Text = "דוחות"
		'
		'cmdRecomputeTable
		'
		Me.cmdRecomputeTable.AutoSize = True
		Me.cmdRecomputeTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdRecomputeTable.FlatAppearance.BorderSize = 0
		Me.cmdRecomputeTable.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdRecomputeTable.Image = Global.TopoUI.My.Resources.Resources.Refresh16Tr
		Me.cmdRecomputeTable.Location = New System.Drawing.Point(51, 109)
		Me.cmdRecomputeTable.Name = "cmdRecomputeTable"
		Me.cmdRecomputeTable.Size = New System.Drawing.Size(22, 22)
		Me.cmdRecomputeTable.TabIndex = 14
		Me.cmdRecomputeTable.UseVisualStyleBackColor = True
		'
		'cmdGetDistSpacing
		'
		Me.cmdGetDistSpacing.FlatAppearance.BorderSize = 0
		Me.cmdGetDistSpacing.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdGetDistSpacing.Image = Global.TopoUI.My.Resources.Resources.ArrowRightDown12Tr
		Me.cmdGetDistSpacing.Location = New System.Drawing.Point(6, 66)
		Me.cmdGetDistSpacing.Name = "cmdGetDistSpacing"
		Me.cmdGetDistSpacing.Size = New System.Drawing.Size(24, 23)
		Me.cmdGetDistSpacing.TabIndex = 13
		Me.cmdGetDistSpacing.UseVisualStyleBackColor = True
		'
		'cmdGetDistBreakHeight
		'
		Me.cmdGetDistBreakHeight.FlatAppearance.BorderSize = 0
		Me.cmdGetDistBreakHeight.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdGetDistBreakHeight.Image = Global.TopoUI.My.Resources.Resources.ArrowRightDown12Tr
		Me.cmdGetDistBreakHeight.Location = New System.Drawing.Point(6, 24)
		Me.cmdGetDistBreakHeight.Name = "cmdGetDistBreakHeight"
		Me.cmdGetDistBreakHeight.Size = New System.Drawing.Size(24, 23)
		Me.cmdGetDistBreakHeight.TabIndex = 12
		Me.cmdGetDistBreakHeight.UseVisualStyleBackColor = True
		'
		'txtSpacing
		'
		Me.txtSpacing.Location = New System.Drawing.Point(36, 66)
		Me.txtSpacing.Name = "txtSpacing"
		Me.txtSpacing.Size = New System.Drawing.Size(59, 22)
		Me.txtSpacing.TabIndex = 11
		'
		'Label17
		'
		Me.Label17.AutoSize = True
		Me.Label17.Location = New System.Drawing.Point(100, 69)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New System.Drawing.Size(106, 14)
		Me.Label17.TabIndex = 10
		Me.Label17.Text = "מרווח בין טבלאות"
		'
		'txtBreakHeight
		'
		Me.txtBreakHeight.Location = New System.Drawing.Point(36, 24)
		Me.txtBreakHeight.Name = "txtBreakHeight"
		Me.txtBreakHeight.Size = New System.Drawing.Size(59, 22)
		Me.txtBreakHeight.TabIndex = 9
		'
		'Label16
		'
		Me.Label16.AutoSize = True
		Me.Label16.Location = New System.Drawing.Point(100, 27)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New System.Drawing.Size(77, 14)
		Me.Label16.TabIndex = 8
		Me.Label16.Text = "גובה הטבלה"
		'
		'cmdExit
		'
		Me.cmdExit.BackColor = System.Drawing.SystemColors.Control
		Me.cmdExit.Cursor = System.Windows.Forms.Cursors.Hand
		Me.cmdExit.FlatAppearance.BorderSize = 0
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Font = New System.Drawing.Font("Tahoma", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Underline), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdExit.Location = New System.Drawing.Point(306, 8)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(61, 24)
		Me.cmdExit.TabIndex = 68
		Me.cmdExit.Text = "יציאה"
		Me.cmdExit.UseVisualStyleBackColor = False
		'
		'frmBamashM
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.BackColor = System.Drawing.SystemColors.Control
		Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.ClientSize = New System.Drawing.Size(372, 453)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.tabMain)
		Me.Controls.Add(Me.txtParcelNo)
		Me.Controls.Add(Me.Label14)
		Me.Controls.Add(Me.txtMaxSubpropNum)
		Me.Controls.Add(Me.Label12)
		Me.Controls.Add(Me.txtMinSubpropNum)
		Me.Controls.Add(Me.Label13)
		Me.Controls.Add(Me.txtMinPropertyID)
		Me.Controls.Add(Me.Label11)
		Me.Controls.Add(Me.txtMaxPropertyID)
		Me.Controls.Add(Me.Label10)
		Me.Controls.Add(Me.cmdOpenBlockRefTable)
		Me.Controls.Add(Me.cmdCalculate)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmBamashM"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.Text = "במ""ש"
		Me.tabMain.ResumeLayout(False)
		Me.tpgEditBloks.ResumeLayout(False)
		Me.tpgEditBloks.PerformLayout()
		Me.tpgPaint.ResumeLayout(False)
		Me.tpgPaint.PerformLayout()
		Me.tpgReport.ResumeLayout(False)
		Me.tpgReport.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmdCalculate As Button
	Friend WithEvents lstReports As ListBox
	Private WithEvents cmdInsertRep As Button
	Private WithEvents cmdExpExcel As Button
	Friend WithEvents txtZebraWidthDrawing As TextBox
	Friend WithEvents txtZebraWidthTable As TextBox
	Private WithEvents txtBufferOffset As TextBox
	Friend WithEvents lblBufferOffset As Label
	Friend WithEvents lblRepBamashSharedScale As Label
	Private WithEvents lblZebraWidthTable As Label
	Private WithEvents lblZoomRadius As Label
	Private WithEvents cmdBamashPaint As Button
	Private WithEvents Label1 As Label
	Private WithEvents cmbChoiceColor As ComboBox
	Private WithEvents Label2 As Label
	Private WithEvents Label3 As Label
	Private WithEvents cmbPropertyType As ComboBox
	Private WithEvents Label4 As Label
	Private WithEvents cmbAprtDesc As ComboBox
	Friend WithEvents txtBldFloor As TextBox
	Private WithEvents Label5 As Label
	Friend WithEvents txtBldEntr As TextBox
	Private WithEvents Label6 As Label
	Private WithEvents Label7 As Label
	Private WithEvents Label8 As Label
	Private WithEvents cmdOpenBlockRefTable As Button
	Private WithEvents cmdSelectByPick As Button
	Private WithEvents Label9 As Label
	Private WithEvents chkBldPart As CheckBox
	Private WithEvents chkBldEntr As CheckBox
	Private WithEvents chkPropID As CheckBox
	Private WithEvents txtPropID As TextBox
	Private WithEvents chkBldFloor As CheckBox
	Private WithEvents txtSubpropNum As TextBox
	Private WithEvents txtBldPart As TextBox
	Private WithEvents txtBldNo As TextBox
	Private WithEvents chkBldNo As CheckBox
	Private WithEvents cmbNumerationPropID As ComboBox
	Private WithEvents chkColor As CheckBox
	Private WithEvents chkSubproperty As CheckBox
	Private WithEvents chkPropertyType As CheckBox
	Private WithEvents chkAprtDesc As CheckBox
	Private WithEvents txtMaxPropertyID As TextBox
	Private WithEvents Label10 As Label
	Private WithEvents txtMinPropertyID As TextBox
	Private WithEvents Label11 As Label
	Private WithEvents txtMaxSubpropNum As TextBox
	Private WithEvents Label12 As Label
	Private WithEvents txtMinSubpropNum As TextBox
	Private WithEvents Label13 As Label
	Private WithEvents cmdSetDataByPick As Button
	Private WithEvents txtZoomRadius As TextBox
	Private WithEvents lblAreaFormat As Label
	Private WithEvents txtParcelNo As TextBox
	Private WithEvents Label14 As Label
	Private WithEvents tabMain As TabControl
	Private WithEvents tpgEditBloks As TabPage
	Private WithEvents tpgPaint As TabPage
	Private WithEvents tpgReport As TabPage
	Private WithEvents Label15 As Label
	Private WithEvents cmdSetDataBySelSet As Button
	Private WithEvents cmdClearPaint As Button
	Private WithEvents cmbAreaFormat As ComboBox
	Private WithEvents txtRepBamashSharedScale As TextBox
	Private WithEvents lblZebraWidthDrawing As Label
	Private WithEvents cmdExit As Button
	Friend WithEvents Label16 As Label
	Private WithEvents txtBreakHeight As TextBox
	Private WithEvents txtSpacing As TextBox
	Private WithEvents cmdGetDistBreakHeight As Button
	Private WithEvents Label17 As Label
	Private WithEvents cmdGetDistSpacing As Button
	Private WithEvents cmdRecomputeTable As Button
	Private WithEvents cmdSelectBlocks As Button
	'	Me.rdbSubNumerationAll = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationEmptyFirst = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationEmptyMax = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationNone = New System.Windows.Forms.RadioButton
End Class
