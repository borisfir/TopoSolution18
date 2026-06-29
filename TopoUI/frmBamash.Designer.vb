<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmBamash
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBamash))
		Me.grbCalcOption = New System.Windows.Forms.GroupBox()
		Me.rdbSubNumerationNone = New System.Windows.Forms.RadioButton()
		Me.rdbSubNumerationEmptyMax = New System.Windows.Forms.RadioButton()
		Me.rdbSubNumerationEmptyFirst = New System.Windows.Forms.RadioButton()
		Me.rdbSubNumerationAll = New System.Windows.Forms.RadioButton()
		Me.cmbAreaFormat = New System.Windows.Forms.ComboBox()
		Me.cmdCalculate = New System.Windows.Forms.Button()
		Me.cmdClearPaint = New System.Windows.Forms.Button()
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
		Me.cmbNumerationSubprop = New System.Windows.Forms.ComboBox()
		Me.txtMaxPropertyID = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.txtMinPropertyID = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.cmdSetDataBySelSet = New System.Windows.Forms.Button()
		Me.cmdSetDataByPick = New System.Windows.Forms.Button()
		Me.cmdRefreshMinMaxProprtyID = New System.Windows.Forms.Button()
		Me.txtMaxSubpropNum = New System.Windows.Forms.TextBox()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.txtMinSubpropNum = New System.Windows.Forms.TextBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.lblAreaFormat = New System.Windows.Forms.Label()
		Me.cmdPaintTable = New System.Windows.Forms.Button()
		Me.txtParcelNo = New System.Windows.Forms.TextBox()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.grbCalcOption.SuspendLayout()
		Me.SuspendLayout()
		'
		'grbCalcOption
		'
		Me.grbCalcOption.AutoSize = True
		Me.grbCalcOption.Controls.Add(Me.rdbSubNumerationNone)
		Me.grbCalcOption.Controls.Add(Me.rdbSubNumerationEmptyMax)
		Me.grbCalcOption.Controls.Add(Me.rdbSubNumerationEmptyFirst)
		Me.grbCalcOption.Controls.Add(Me.rdbSubNumerationAll)
		Me.grbCalcOption.Location = New System.Drawing.Point(599, 516)
		Me.grbCalcOption.Name = "grbCalcOption"
		Me.grbCalcOption.Size = New System.Drawing.Size(237, 162)
		Me.grbCalcOption.TabIndex = 0
		Me.grbCalcOption.TabStop = False
		Me.grbCalcOption.Visible = False
		'
		'rdbSubNumerationNone
		'
		Me.rdbSubNumerationNone.AutoSize = True
		Me.rdbSubNumerationNone.Location = New System.Drawing.Point(72, 115)
		Me.rdbSubNumerationNone.Name = "rdbSubNumerationNone"
		Me.rdbSubNumerationNone.Size = New System.Drawing.Size(99, 18)
		Me.rdbSubNumerationNone.TabIndex = 3
		Me.rdbSubNumerationNone.TabStop = True
		Me.rdbSubNumerationNone.Text = "RadioButton4"
		Me.rdbSubNumerationNone.UseVisualStyleBackColor = True
		'
		'rdbSubNumerationEmptyMax
		'
		Me.rdbSubNumerationEmptyMax.AutoSize = True
		Me.rdbSubNumerationEmptyMax.Location = New System.Drawing.Point(72, 84)
		Me.rdbSubNumerationEmptyMax.Name = "rdbSubNumerationEmptyMax"
		Me.rdbSubNumerationEmptyMax.Size = New System.Drawing.Size(99, 18)
		Me.rdbSubNumerationEmptyMax.TabIndex = 2
		Me.rdbSubNumerationEmptyMax.TabStop = True
		Me.rdbSubNumerationEmptyMax.Text = "RadioButton3"
		Me.rdbSubNumerationEmptyMax.UseVisualStyleBackColor = True
		'
		'rdbSubNumerationEmptyFirst
		'
		Me.rdbSubNumerationEmptyFirst.AutoSize = True
		Me.rdbSubNumerationEmptyFirst.Location = New System.Drawing.Point(72, 54)
		Me.rdbSubNumerationEmptyFirst.Name = "rdbSubNumerationEmptyFirst"
		Me.rdbSubNumerationEmptyFirst.Size = New System.Drawing.Size(99, 18)
		Me.rdbSubNumerationEmptyFirst.TabIndex = 1
		Me.rdbSubNumerationEmptyFirst.TabStop = True
		Me.rdbSubNumerationEmptyFirst.Text = "RadioButton2"
		Me.rdbSubNumerationEmptyFirst.UseVisualStyleBackColor = True
		'
		'rdbSubNumerationAll
		'
		Me.rdbSubNumerationAll.AutoSize = True
		Me.rdbSubNumerationAll.Location = New System.Drawing.Point(96, 25)
		Me.rdbSubNumerationAll.Name = "rdbSubNumerationAll"
		Me.rdbSubNumerationAll.Size = New System.Drawing.Size(99, 18)
		Me.rdbSubNumerationAll.TabIndex = 1
		Me.rdbSubNumerationAll.TabStop = True
		Me.rdbSubNumerationAll.Text = "RadioButton1"
		Me.rdbSubNumerationAll.UseVisualStyleBackColor = True
		'
		'cmbAreaFormat
		'
		Me.cmbAreaFormat.FormattingEnabled = True
		Me.cmbAreaFormat.Location = New System.Drawing.Point(520, 473)
		Me.cmbAreaFormat.Name = "cmbAreaFormat"
		Me.cmbAreaFormat.Size = New System.Drawing.Size(69, 22)
		Me.cmbAreaFormat.TabIndex = 8
		'
		'cmdCalculate
		'
		Me.cmdCalculate.Location = New System.Drawing.Point(369, 2)
		Me.cmdCalculate.Name = "cmdCalculate"
		Me.cmdCalculate.Size = New System.Drawing.Size(61, 24)
		Me.cmdCalculate.TabIndex = 1
		Me.cmdCalculate.Text = "חישוב"
		Me.cmdCalculate.UseVisualStyleBackColor = True
		'
		'cmdClearPaint
		'
		Me.cmdClearPaint.Location = New System.Drawing.Point(507, 311)
		Me.cmdClearPaint.Name = "cmdClearPaint"
		Me.cmdClearPaint.Size = New System.Drawing.Size(102, 26)
		Me.cmdClearPaint.TabIndex = 3
		Me.cmdClearPaint.Text = "מחיקת צביעה"
		Me.cmdClearPaint.UseVisualStyleBackColor = True
		'
		'cmdBamashPaint
		'
		Me.cmdBamashPaint.Location = New System.Drawing.Point(408, 311)
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
		Me.lstReports.Location = New System.Drawing.Point(484, 43)
		Me.lstReports.Name = "lstReports"
		Me.lstReports.Size = New System.Drawing.Size(137, 214)
		Me.lstReports.TabIndex = 5
		'
		'cmdInsertRep
		'
		Me.cmdInsertRep.Location = New System.Drawing.Point(483, 270)
		Me.cmdInsertRep.Name = "cmdInsertRep"
		Me.cmdInsertRep.Size = New System.Drawing.Size(59, 26)
		Me.cmdInsertRep.TabIndex = 6
		Me.cmdInsertRep.Text = "Insert"
		Me.cmdInsertRep.UseVisualStyleBackColor = True
		'
		'cmdExpExcel
		'
		Me.cmdExpExcel.Location = New System.Drawing.Point(548, 269)
		Me.cmdExpExcel.Name = "cmdExpExcel"
		Me.cmdExpExcel.Size = New System.Drawing.Size(59, 28)
		Me.cmdExpExcel.TabIndex = 7
		Me.cmdExpExcel.Text = "Excel"
		Me.cmdExpExcel.UseVisualStyleBackColor = True
		'
		'txtZebraWidthDrawing
		'
		Me.txtZebraWidthDrawing.Location = New System.Drawing.Point(520, 348)
		Me.txtZebraWidthDrawing.Name = "txtZebraWidthDrawing"
		Me.txtZebraWidthDrawing.Size = New System.Drawing.Size(114, 22)
		Me.txtZebraWidthDrawing.TabIndex = 10
		'
		'txtZebraWidthTable
		'
		Me.txtZebraWidthTable.Location = New System.Drawing.Point(520, 382)
		Me.txtZebraWidthTable.Name = "txtZebraWidthTable"
		Me.txtZebraWidthTable.Size = New System.Drawing.Size(114, 22)
		Me.txtZebraWidthTable.TabIndex = 11
		'
		'txtBufferOffset
		'
		Me.txtBufferOffset.Location = New System.Drawing.Point(520, 415)
		Me.txtBufferOffset.Name = "txtBufferOffset"
		Me.txtBufferOffset.Size = New System.Drawing.Size(114, 22)
		Me.txtBufferOffset.TabIndex = 12
		'
		'txtZoomRadius
		'
		Me.txtZoomRadius.Location = New System.Drawing.Point(201, 581)
		Me.txtZoomRadius.Name = "txtZoomRadius"
		Me.txtZoomRadius.Size = New System.Drawing.Size(114, 22)
		Me.txtZoomRadius.TabIndex = 13
		Me.txtZoomRadius.Visible = False
		'
		'txtRepBamashSharedScale
		'
		Me.txtRepBamashSharedScale.Location = New System.Drawing.Point(520, 445)
		Me.txtRepBamashSharedScale.Name = "txtRepBamashSharedScale"
		Me.txtRepBamashSharedScale.Size = New System.Drawing.Size(114, 22)
		Me.txtRepBamashSharedScale.TabIndex = 14
		'
		'lblZebraWidthDrawing
		'
		Me.lblZebraWidthDrawing.AutoSize = True
		Me.lblZebraWidthDrawing.Location = New System.Drawing.Point(366, 352)
		Me.lblZebraWidthDrawing.Name = "lblZebraWidthDrawing"
		Me.lblZebraWidthDrawing.Size = New System.Drawing.Size(42, 14)
		Me.lblZebraWidthDrawing.TabIndex = 15
		Me.lblZebraWidthDrawing.Text = "Label1"
		'
		'lblZebraWidthTable
		'
		Me.lblZebraWidthTable.AutoSize = True
		Me.lblZebraWidthTable.Location = New System.Drawing.Point(366, 385)
		Me.lblZebraWidthTable.Name = "lblZebraWidthTable"
		Me.lblZebraWidthTable.Size = New System.Drawing.Size(42, 14)
		Me.lblZebraWidthTable.TabIndex = 16
		Me.lblZebraWidthTable.Text = "Label1"
		'
		'lblBufferOffset
		'
		Me.lblBufferOffset.AutoSize = True
		Me.lblBufferOffset.Location = New System.Drawing.Point(366, 415)
		Me.lblBufferOffset.Name = "lblBufferOffset"
		Me.lblBufferOffset.Size = New System.Drawing.Size(42, 14)
		Me.lblBufferOffset.TabIndex = 17
		Me.lblBufferOffset.Text = "Label1"
		'
		'lblZoomRadius
		'
		Me.lblZoomRadius.AutoSize = True
		Me.lblZoomRadius.Location = New System.Drawing.Point(149, 587)
		Me.lblZoomRadius.Name = "lblZoomRadius"
		Me.lblZoomRadius.Size = New System.Drawing.Size(42, 14)
		Me.lblZoomRadius.TabIndex = 18
		Me.lblZoomRadius.Text = "Label1"
		Me.lblZoomRadius.Visible = False
		'
		'lblRepBamashSharedScale
		'
		Me.lblRepBamashSharedScale.AutoSize = True
		Me.lblRepBamashSharedScale.Location = New System.Drawing.Point(370, 448)
		Me.lblRepBamashSharedScale.Name = "lblRepBamashSharedScale"
		Me.lblRepBamashSharedScale.Size = New System.Drawing.Size(42, 14)
		Me.lblRepBamashSharedScale.TabIndex = 19
		Me.lblRepBamashSharedScale.Text = "Label1"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(8, 12)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(52, 14)
		Me.Label1.TabIndex = 20
		Me.Label1.Text = "מס' נכס"
		'
		'txtPropID
		'
		Me.txtPropID.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtPropID.Location = New System.Drawing.Point(98, 10)
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
		Me.cmbChoiceColor.Location = New System.Drawing.Point(98, 94)
		Me.cmbChoiceColor.Name = "cmbChoiceColor"
		Me.cmbChoiceColor.Size = New System.Drawing.Size(97, 22)
		Me.cmbChoiceColor.TabIndex = 22
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(8, 96)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(31, 14)
		Me.Label2.TabIndex = 23
		Me.Label2.Text = "צבע"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(8, 226)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(50, 14)
		Me.Label3.TabIndex = 25
		Me.Label3.Text = "סוג נכס"
		'
		'cmbPropertyType
		'
		Me.cmbPropertyType.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmbPropertyType.FormattingEnabled = True
		Me.cmbPropertyType.Location = New System.Drawing.Point(98, 224)
		Me.cmbPropertyType.Name = "cmbPropertyType"
		Me.cmbPropertyType.Size = New System.Drawing.Size(170, 21)
		Me.cmbPropertyType.TabIndex = 24
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(8, 256)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(68, 14)
		Me.Label4.TabIndex = 27
		Me.Label4.Text = "תיאור דירה"
		'
		'cmbAprtDesc
		'
		Me.cmbAprtDesc.FormattingEnabled = True
		Me.cmbAprtDesc.Location = New System.Drawing.Point(98, 254)
		Me.cmbAprtDesc.Name = "cmbAprtDesc"
		Me.cmbAprtDesc.Size = New System.Drawing.Size(170, 22)
		Me.cmbAprtDesc.TabIndex = 26
		'
		'txtBldFloor
		'
		Me.txtBldFloor.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldFloor.Location = New System.Drawing.Point(98, 284)
		Me.txtBldFloor.Name = "txtBldFloor"
		Me.txtBldFloor.Size = New System.Drawing.Size(48, 21)
		Me.txtBldFloor.TabIndex = 29
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(8, 286)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(34, 14)
		Me.Label5.TabIndex = 28
		Me.Label5.Text = "קומה"
		'
		'txtBldEntr
		'
		Me.txtBldEntr.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldEntr.Location = New System.Drawing.Point(98, 314)
		Me.txtBldEntr.Name = "txtBldEntr"
		Me.txtBldEntr.Size = New System.Drawing.Size(48, 21)
		Me.txtBldEntr.TabIndex = 31
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(8, 316)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(39, 14)
		Me.Label6.TabIndex = 30
		Me.Label6.Text = "כניסה"
		'
		'txtBldPart
		'
		Me.txtBldPart.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldPart.Location = New System.Drawing.Point(98, 344)
		Me.txtBldPart.Name = "txtBldPart"
		Me.txtBldPart.Size = New System.Drawing.Size(48, 21)
		Me.txtBldPart.TabIndex = 33
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(8, 346)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(53, 14)
		Me.Label7.TabIndex = 32
		Me.Label7.Text = "מס' אגף"
		'
		'txtBldNo
		'
		Me.txtBldNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtBldNo.Location = New System.Drawing.Point(98, 373)
		Me.txtBldNo.Name = "txtBldNo"
		Me.txtBldNo.Size = New System.Drawing.Size(48, 21)
		Me.txtBldNo.TabIndex = 35
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(8, 376)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(53, 14)
		Me.Label8.TabIndex = 34
		Me.Label8.Text = "מס' בית"
		'
		'cmdOpenBlockRefTable
		'
		Me.cmdOpenBlockRefTable.Location = New System.Drawing.Point(369, 31)
		Me.cmdOpenBlockRefTable.Name = "cmdOpenBlockRefTable"
		Me.cmdOpenBlockRefTable.Size = New System.Drawing.Size(61, 24)
		Me.cmdOpenBlockRefTable.TabIndex = 36
		Me.cmdOpenBlockRefTable.Text = "טבלה"
		Me.cmdOpenBlockRefTable.UseVisualStyleBackColor = True
		'
		'cmdSelectByPick
		'
		Me.cmdSelectByPick.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdSelectByPick.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdSelectByPick.Image = CType(resources.GetObject("cmdSelectByPick.Image"), System.Drawing.Image)
		Me.cmdSelectByPick.Location = New System.Drawing.Point(259, 36)
		Me.cmdSelectByPick.Name = "cmdSelectByPick"
		Me.cmdSelectByPick.Size = New System.Drawing.Size(30, 30)
		Me.cmdSelectByPick.TabIndex = 37
		Me.cmdSelectByPick.UseVisualStyleBackColor = True
		'
		'chkBldFloor
		'
		Me.chkBldFloor.AutoSize = True
		Me.chkBldFloor.Location = New System.Drawing.Point(152, 288)
		Me.chkBldFloor.Name = "chkBldFloor"
		Me.chkBldFloor.Size = New System.Drawing.Size(15, 14)
		Me.chkBldFloor.TabIndex = 38
		Me.chkBldFloor.UseVisualStyleBackColor = True
		'
		'txtSubpropNum
		'
		Me.txtSubpropNum.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtSubpropNum.Location = New System.Drawing.Point(98, 127)
		Me.txtSubpropNum.Name = "txtSubpropNum"
		Me.txtSubpropNum.Size = New System.Drawing.Size(48, 21)
		Me.txtSubpropNum.TabIndex = 40
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(8, 129)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(70, 14)
		Me.Label9.TabIndex = 39
		Me.Label9.Text = "מס' צמידות"
		'
		'chkBldPart
		'
		Me.chkBldPart.AutoSize = True
		Me.chkBldPart.Location = New System.Drawing.Point(152, 348)
		Me.chkBldPart.Name = "chkBldPart"
		Me.chkBldPart.Size = New System.Drawing.Size(15, 14)
		Me.chkBldPart.TabIndex = 41
		Me.chkBldPart.UseVisualStyleBackColor = True
		'
		'chkBldEntr
		'
		Me.chkBldEntr.AutoSize = True
		Me.chkBldEntr.Location = New System.Drawing.Point(152, 318)
		Me.chkBldEntr.Name = "chkBldEntr"
		Me.chkBldEntr.Size = New System.Drawing.Size(15, 14)
		Me.chkBldEntr.TabIndex = 42
		Me.chkBldEntr.UseVisualStyleBackColor = True
		'
		'chkPropID
		'
		Me.chkPropID.AutoSize = True
		Me.chkPropID.Location = New System.Drawing.Point(152, 14)
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
		Me.cmbNumerationPropID.Location = New System.Drawing.Point(214, 7)
		Me.cmbNumerationPropID.Name = "cmbNumerationPropID"
		Me.cmbNumerationPropID.Size = New System.Drawing.Size(136, 20)
		Me.cmbNumerationPropID.TabIndex = 44
		'
		'chkBldNo
		'
		Me.chkBldNo.AutoSize = True
		Me.chkBldNo.Location = New System.Drawing.Point(152, 378)
		Me.chkBldNo.Name = "chkBldNo"
		Me.chkBldNo.Size = New System.Drawing.Size(15, 14)
		Me.chkBldNo.TabIndex = 46
		Me.chkBldNo.UseVisualStyleBackColor = True
		'
		'chkColor
		'
		Me.chkColor.AutoSize = True
		Me.chkColor.Location = New System.Drawing.Point(201, 97)
		Me.chkColor.Name = "chkColor"
		Me.chkColor.Size = New System.Drawing.Size(15, 14)
		Me.chkColor.TabIndex = 47
		Me.chkColor.UseVisualStyleBackColor = True
		'
		'chkSubproperty
		'
		Me.chkSubproperty.AutoSize = True
		Me.chkSubproperty.Location = New System.Drawing.Point(152, 132)
		Me.chkSubproperty.Name = "chkSubproperty"
		Me.chkSubproperty.Size = New System.Drawing.Size(15, 14)
		Me.chkSubproperty.TabIndex = 48
		Me.chkSubproperty.UseVisualStyleBackColor = True
		'
		'chkPropertyType
		'
		Me.chkPropertyType.AutoSize = True
		Me.chkPropertyType.Location = New System.Drawing.Point(274, 229)
		Me.chkPropertyType.Name = "chkPropertyType"
		Me.chkPropertyType.Size = New System.Drawing.Size(15, 14)
		Me.chkPropertyType.TabIndex = 49
		Me.chkPropertyType.UseVisualStyleBackColor = True
		'
		'chkAprtDesc
		'
		Me.chkAprtDesc.AutoSize = True
		Me.chkAprtDesc.Location = New System.Drawing.Point(274, 259)
		Me.chkAprtDesc.Name = "chkAprtDesc"
		Me.chkAprtDesc.Size = New System.Drawing.Size(15, 14)
		Me.chkAprtDesc.TabIndex = 50
		Me.chkAprtDesc.UseVisualStyleBackColor = True
		'
		'cmbNumerationSubprop
		'
		Me.cmbNumerationSubprop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cmbNumerationSubprop.Font = New System.Drawing.Font("Tahoma", 7.8!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmbNumerationSubprop.FormattingEnabled = True
		Me.cmbNumerationSubprop.Location = New System.Drawing.Point(227, 127)
		Me.cmbNumerationSubprop.Name = "cmbNumerationSubprop"
		Me.cmbNumerationSubprop.Size = New System.Drawing.Size(168, 20)
		Me.cmbNumerationSubprop.TabIndex = 52
		Me.cmbNumerationSubprop.Visible = False
		'
		'txtMaxPropertyID
		'
		Me.txtMaxPropertyID.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMaxPropertyID.Location = New System.Drawing.Point(100, 68)
		Me.txtMaxPropertyID.Name = "txtMaxPropertyID"
		Me.txtMaxPropertyID.ReadOnly = True
		Me.txtMaxPropertyID.Size = New System.Drawing.Size(24, 15)
		Me.txtMaxPropertyID.TabIndex = 54
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(8, 68)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(83, 14)
		Me.Label10.TabIndex = 53
		Me.Label10.Text = "מס' נכס מרבי"
		'
		'txtMinPropertyID
		'
		Me.txtMinPropertyID.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMinPropertyID.Location = New System.Drawing.Point(100, 41)
		Me.txtMinPropertyID.Name = "txtMinPropertyID"
		Me.txtMinPropertyID.ReadOnly = True
		Me.txtMinPropertyID.Size = New System.Drawing.Size(24, 15)
		Me.txtMinPropertyID.TabIndex = 56
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(8, 41)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(87, 14)
		Me.Label11.TabIndex = 55
		Me.Label11.Text = "מס' נכס מזערי"
		'
		'cmdSetDataBySelSet
		'
		Me.cmdSetDataBySelSet.AutoSize = True
		Me.cmdSetDataBySelSet.Location = New System.Drawing.Point(303, 85)
		Me.cmdSetDataBySelSet.Name = "cmdSetDataBySelSet"
		Me.cmdSetDataBySelSet.Size = New System.Drawing.Size(103, 25)
		Me.cmdSetDataBySelSet.TabIndex = 57
		Me.cmdSetDataBySelSet.Text = "By selection set"
		Me.cmdSetDataBySelSet.UseVisualStyleBackColor = True
		'
		'cmdSetDataByPick
		'
		Me.cmdSetDataByPick.AutoSize = True
		Me.cmdSetDataByPick.Location = New System.Drawing.Point(244, 84)
		Me.cmdSetDataByPick.Name = "cmdSetDataByPick"
		Me.cmdSetDataByPick.Size = New System.Drawing.Size(55, 26)
		Me.cmdSetDataByPick.TabIndex = 58
		Me.cmdSetDataByPick.Text = "By pick"
		Me.cmdSetDataByPick.UseVisualStyleBackColor = True
		'
		'cmdRefreshMinMaxProprtyID
		'
		Me.cmdRefreshMinMaxProprtyID.Image = CType(resources.GetObject("cmdRefreshMinMaxProprtyID.Image"), System.Drawing.Image)
		Me.cmdRefreshMinMaxProprtyID.Location = New System.Drawing.Point(214, 43)
		Me.cmdRefreshMinMaxProprtyID.Name = "cmdRefreshMinMaxProprtyID"
		Me.cmdRefreshMinMaxProprtyID.Size = New System.Drawing.Size(29, 23)
		Me.cmdRefreshMinMaxProprtyID.TabIndex = 59
		Me.cmdRefreshMinMaxProprtyID.UseVisualStyleBackColor = True
		Me.cmdRefreshMinMaxProprtyID.Visible = False
		'
		'txtMaxSubpropNum
		'
		Me.txtMaxSubpropNum.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMaxSubpropNum.Location = New System.Drawing.Point(119, 191)
		Me.txtMaxSubpropNum.Name = "txtMaxSubpropNum"
		Me.txtMaxSubpropNum.ReadOnly = True
		Me.txtMaxSubpropNum.Size = New System.Drawing.Size(48, 15)
		Me.txtMaxSubpropNum.TabIndex = 63
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(8, 161)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(105, 14)
		Me.Label12.TabIndex = 62
		Me.Label12.Text = "מס' צמידות מזערי"
		'
		'txtMinSubpropNum
		'
		Me.txtMinSubpropNum.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtMinSubpropNum.Location = New System.Drawing.Point(119, 162)
		Me.txtMinSubpropNum.Name = "txtMinSubpropNum"
		Me.txtMinSubpropNum.ReadOnly = True
		Me.txtMinSubpropNum.Size = New System.Drawing.Size(48, 15)
		Me.txtMinSubpropNum.TabIndex = 61
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(8, 192)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(101, 14)
		Me.Label13.TabIndex = 60
		Me.Label13.Text = "מס' צמידות מרבי"
		'
		'lblAreaFormat
		'
		Me.lblAreaFormat.AutoSize = True
		Me.lblAreaFormat.Location = New System.Drawing.Point(370, 481)
		Me.lblAreaFormat.Name = "lblAreaFormat"
		Me.lblAreaFormat.Size = New System.Drawing.Size(42, 14)
		Me.lblAreaFormat.TabIndex = 64
		Me.lblAreaFormat.Text = "Label1"
		'
		'cmdPaintTable
		'
		Me.cmdPaintTable.Location = New System.Drawing.Point(503, 501)
		Me.cmdPaintTable.Name = "cmdPaintTable"
		Me.cmdPaintTable.Size = New System.Drawing.Size(86, 26)
		Me.cmdPaintTable.TabIndex = 9
		Me.cmdPaintTable.Text = "Button1"
		Me.cmdPaintTable.UseVisualStyleBackColor = True
		Me.cmdPaintTable.Visible = False
		'
		'txtParcelNo
		'
		Me.txtParcelNo.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.txtParcelNo.Location = New System.Drawing.Point(573, 6)
		Me.txtParcelNo.Name = "txtParcelNo"
		Me.txtParcelNo.Size = New System.Drawing.Size(48, 21)
		Me.txtParcelNo.TabIndex = 66
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New System.Drawing.Point(505, 9)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(62, 14)
		Me.Label14.TabIndex = 65
		Me.Label14.Text = "מס' חלקה"
		'
		'frmBamash
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.AutoSize = True
		Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.ClientSize = New System.Drawing.Size(638, 623)
		Me.Controls.Add(Me.txtParcelNo)
		Me.Controls.Add(Me.Label14)
		Me.Controls.Add(Me.lblAreaFormat)
		Me.Controls.Add(Me.txtMaxSubpropNum)
		Me.Controls.Add(Me.Label12)
		Me.Controls.Add(Me.txtMinSubpropNum)
		Me.Controls.Add(Me.cmbAreaFormat)
		Me.Controls.Add(Me.Label13)
		Me.Controls.Add(Me.cmdRefreshMinMaxProprtyID)
		Me.Controls.Add(Me.cmdSetDataByPick)
		Me.Controls.Add(Me.cmdSetDataBySelSet)
		Me.Controls.Add(Me.txtMinPropertyID)
		Me.Controls.Add(Me.Label11)
		Me.Controls.Add(Me.txtMaxPropertyID)
		Me.Controls.Add(Me.Label10)
		Me.Controls.Add(Me.cmbNumerationSubprop)
		Me.Controls.Add(Me.chkAprtDesc)
		Me.Controls.Add(Me.chkPropertyType)
		Me.Controls.Add(Me.chkSubproperty)
		Me.Controls.Add(Me.chkColor)
		Me.Controls.Add(Me.chkBldNo)
		Me.Controls.Add(Me.cmbNumerationPropID)
		Me.Controls.Add(Me.chkPropID)
		Me.Controls.Add(Me.cmdSelectByPick)
		Me.Controls.Add(Me.chkBldEntr)
		Me.Controls.Add(Me.chkBldPart)
		Me.Controls.Add(Me.txtSubpropNum)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.chkBldFloor)
		Me.Controls.Add(Me.cmdOpenBlockRefTable)
		Me.Controls.Add(Me.txtBldNo)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.txtBldPart)
		Me.Controls.Add(Me.Label7)
		Me.Controls.Add(Me.txtBldEntr)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.txtBldFloor)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.cmbAprtDesc)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.cmbPropertyType)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.cmbChoiceColor)
		Me.Controls.Add(Me.txtPropID)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.lblRepBamashSharedScale)
		Me.Controls.Add(Me.lblZoomRadius)
		Me.Controls.Add(Me.lblBufferOffset)
		Me.Controls.Add(Me.lblZebraWidthTable)
		Me.Controls.Add(Me.lblZebraWidthDrawing)
		Me.Controls.Add(Me.txtRepBamashSharedScale)
		Me.Controls.Add(Me.txtZoomRadius)
		Me.Controls.Add(Me.txtBufferOffset)
		Me.Controls.Add(Me.txtZebraWidthTable)
		Me.Controls.Add(Me.txtZebraWidthDrawing)
		Me.Controls.Add(Me.cmdPaintTable)
		Me.Controls.Add(Me.cmdExpExcel)
		Me.Controls.Add(Me.cmdInsertRep)
		Me.Controls.Add(Me.lstReports)
		Me.Controls.Add(Me.cmdBamashPaint)
		Me.Controls.Add(Me.cmdClearPaint)
		Me.Controls.Add(Me.cmdCalculate)
		Me.Controls.Add(Me.grbCalcOption)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Name = "frmBamash"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
		Me.Text = "במ""ש"
		Me.grbCalcOption.ResumeLayout(False)
		Me.grbCalcOption.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Private WithEvents grbCalcOption As GroupBox
	Private WithEvents rdbSubNumerationNone As RadioButton
	Private WithEvents rdbSubNumerationEmptyMax As RadioButton
	Private WithEvents rdbSubNumerationEmptyFirst As RadioButton
	Private WithEvents rdbSubNumerationAll As RadioButton
	Private WithEvents cmdCalculate As Button
	Private WithEvents cmdClearPaint As Button
	Friend WithEvents lstReports As ListBox
	Private WithEvents cmdInsertRep As Button
	Private WithEvents cmdExpExcel As Button
	Friend WithEvents cmbAreaFormat As ComboBox
	Friend WithEvents txtZebraWidthDrawing As TextBox
	Friend WithEvents txtZebraWidthTable As TextBox
	Private WithEvents txtBufferOffset As TextBox
	Friend WithEvents txtRepBamashSharedScale As TextBox
	Friend WithEvents lblZebraWidthDrawing As Label
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
	Private WithEvents cmbNumerationSubprop As ComboBox
	Private WithEvents txtMaxPropertyID As TextBox
	Private WithEvents Label10 As Label
	Private WithEvents txtMinPropertyID As TextBox
	Private WithEvents Label11 As Label
	Friend WithEvents cmdSetDataBySelSet As Button
	Private WithEvents cmdRefreshMinMaxProprtyID As Button
	Private WithEvents txtMaxSubpropNum As TextBox
	Private WithEvents Label12 As Label
	Private WithEvents txtMinSubpropNum As TextBox
	Private WithEvents Label13 As Label
	Private WithEvents cmdSetDataByPick As Button
	Private WithEvents txtZoomRadius As TextBox
	Private WithEvents lblAreaFormat As Label
	Friend WithEvents cmdPaintTable As Button
	Private WithEvents txtParcelNo As TextBox
	Private WithEvents Label14 As Label
	'	Me.rdbSubNumerationAll = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationEmptyFirst = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationEmptyMax = New System.Windows.Forms.RadioButton
	'	Me.rdbSubNumerationNone = New System.Windows.Forms.RadioButton
End Class
