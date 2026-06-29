<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditOwners
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
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditOwners))
		Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.txtBlockNo = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtBlockAddNo = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtParcelNo = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtSumDec = New System.Windows.Forms.TextBox()
		Me.cmdComplete = New System.Windows.Forms.Button()
		Me.txtSumSimple = New System.Windows.Forms.TextBox()
		Me.chkReduction = New System.Windows.Forms.CheckBox()
		Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
		Me.dgvMain = New System.Windows.Forms.DataGridView()
		Me.chkDemolitionOrder = New System.Windows.Forms.CheckBox()
		Me.mskRegulation29IP = New System.Windows.Forms.MaskedTextBox()
		Me.mskRegulation29Page = New System.Windows.Forms.MaskedTextBox()
		Me.mskParagraph123IP = New System.Windows.Forms.MaskedTextBox()
		Me.mskParagraph123Page = New System.Windows.Forms.MaskedTextBox()
		Me.chkParagraph123 = New System.Windows.Forms.CheckBox()
		Me.chkRegulation29 = New System.Windows.Forms.CheckBox()
		Me.chkRoadOrdinance = New System.Windows.Forms.CheckBox()
		Me.chkLeasingAreaExists = New System.Windows.Forms.CheckBox()
		Me.chkParagraph19AreaExists = New System.Windows.Forms.CheckBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.chkNoteEdited = New System.Windows.Forms.CheckBox()
		Me.cmdCalc = New System.Windows.Forms.Button()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.txtNote = New System.Windows.Forms.TextBox()
		Me.mskParagraph5IP = New System.Windows.Forms.MaskedTextBox()
		Me.mskParagraph5Page = New System.Windows.Forms.MaskedTextBox()
		Me.chkAntiqueSite = New System.Windows.Forms.CheckBox()
		Me.chkVerdict = New System.Windows.Forms.CheckBox()
		Me.chkForeclosure = New System.Windows.Forms.CheckBox()
		Me.chkMortgage = New System.Windows.Forms.CheckBox()
		Me.chkParagraph126 = New System.Windows.Forms.CheckBox()
		Me.chkParagraph5 = New System.Windows.Forms.CheckBox()
		Me.chkLeasing = New System.Windows.Forms.CheckBox()
		Me.mskParagraph19IP = New System.Windows.Forms.MaskedTextBox()
		Me.mskParagraph19Page = New System.Windows.Forms.MaskedTextBox()
		Me.chkParagraph19 = New System.Windows.Forms.CheckBox()
		Me.chkSharedHouse = New System.Windows.Forms.CheckBox()
		Me.chkParagraph11a = New System.Windows.Forms.CheckBox()
		Me.CheckBox3 = New System.Windows.Forms.CheckBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.CheckBox4 = New System.Windows.Forms.CheckBox()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.TextBox1 = New System.Windows.Forms.TextBox()
		Me.MaskedTextBox1 = New System.Windows.Forms.MaskedTextBox()
		Me.MaskedTextBox2 = New System.Windows.Forms.MaskedTextBox()
		Me.MaskedTextBox3 = New System.Windows.Forms.MaskedTextBox()
		Me.MaskedTextBox4 = New System.Windows.Forms.MaskedTextBox()
		Me.bnsParcel = New System.Windows.Forms.BindingSource(Me.components)
		Me.ccbOwner = New System.Windows.Forms.DataGridViewComboBoxColumn()
		Me.ctxDividend = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxDivisor = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxPartPct = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.cchSum = New System.Windows.Forms.DataGridViewCheckBoxColumn()
		Me.ctxID = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxOwner = New System.Windows.Forms.DataGridViewTextBoxColumn()
		Me.ctxRowr = New System.Windows.Forms.DataGridViewTextBoxColumn()
		CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SplitContainer1.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel2.SuspendLayout()
		Me.SplitContainer1.SuspendLayout()
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.bnsParcel, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
		Me.cmdExit.Location = New System.Drawing.Point(398, 4)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(26, 26)
		Me.cmdExit.TabIndex = 13
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Image = CType(resources.GetObject("cmdCancel.Image"), System.Drawing.Image)
		Me.cmdCancel.Location = New System.Drawing.Point(368, 4)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(26, 26)
		Me.cmdCancel.TabIndex = 12
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdOK
		'
		Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.cmdOK.Image = CType(resources.GetObject("cmdOK.Image"), System.Drawing.Image)
		Me.cmdOK.Location = New System.Drawing.Point(338, 4)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(26, 26)
		Me.cmdOK.TabIndex = 11
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'txtBlockNo
		'
		Me.txtBlockNo.Location = New System.Drawing.Point(42, 4)
		Me.txtBlockNo.Name = "txtBlockNo"
		Me.txtBlockNo.Size = New System.Drawing.Size(48, 22)
		Me.txtBlockNo.TabIndex = 15
		Me.txtBlockNo.Text = "123456"
		'
		'Label1
		'
		Me.Label1.Location = New System.Drawing.Point(0, 6)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(42, 14)
		Me.Label1.TabIndex = 16
		Me.Label1.Text = "גוש:"
		'
		'txtBlockAddNo
		'
		Me.txtBlockAddNo.Location = New System.Drawing.Point(96, 4)
		Me.txtBlockAddNo.Name = "txtBlockAddNo"
		Me.txtBlockAddNo.Size = New System.Drawing.Size(24, 22)
		Me.txtBlockAddNo.TabIndex = 17
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(0, 34)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(42, 14)
		Me.Label2.TabIndex = 19
		Me.Label2.Text = "חלקה:"
		'
		'txtParcelNo
		'
		Me.txtParcelNo.Location = New System.Drawing.Point(42, 32)
		Me.txtParcelNo.Name = "txtParcelNo"
		Me.txtParcelNo.Size = New System.Drawing.Size(48, 22)
		Me.txtParcelNo.TabIndex = 18
		Me.txtParcelNo.Text = "123456"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(136, 6)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(49, 14)
		Me.Label3.TabIndex = 21
		Me.Label3.Text = "סך הכל"
		'
		'txtSumDec
		'
		Me.txtSumDec.Location = New System.Drawing.Point(188, 4)
		Me.txtSumDec.Name = "txtSumDec"
		Me.txtSumDec.ReadOnly = True
		Me.txtSumDec.Size = New System.Drawing.Size(73, 22)
		Me.txtSumDec.TabIndex = 20
		'
		'cmdComplete
		'
		Me.cmdComplete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdComplete.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.cmdComplete.Location = New System.Drawing.Point(264, 4)
		Me.cmdComplete.Name = "cmdComplete"
		Me.cmdComplete.Size = New System.Drawing.Size(50, 22)
		Me.cmdComplete.TabIndex = 22
		Me.cmdComplete.Text = "100%"
		Me.cmdComplete.UseVisualStyleBackColor = True
		'
		'txtSumSimple
		'
		Me.txtSumSimple.BackColor = System.Drawing.SystemColors.Control
		Me.txtSumSimple.Location = New System.Drawing.Point(188, 32)
		Me.txtSumSimple.Name = "txtSumSimple"
		Me.txtSumSimple.ReadOnly = True
		Me.txtSumSimple.Size = New System.Drawing.Size(109, 22)
		Me.txtSumSimple.TabIndex = 23
		'
		'chkReduction
		'
		Me.chkReduction.AutoSize = True
		Me.chkReduction.Location = New System.Drawing.Point(335, 36)
		Me.chkReduction.Name = "chkReduction"
		Me.chkReduction.Size = New System.Drawing.Size(89, 18)
		Me.chkReduction.TabIndex = 24
		Me.chkReduction.Text = "צמצום שבר"
		Me.chkReduction.UseVisualStyleBackColor = True
		'
		'SplitContainer1
		'
		Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
		Me.SplitContainer1.Location = New System.Drawing.Point(0, 61)
		Me.SplitContainer1.Name = "SplitContainer1"
		Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.dgvMain)
		Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkDemolitionOrder)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskRegulation29IP)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskRegulation29Page)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph123IP)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph123Page)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph123)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkRegulation29)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkRoadOrdinance)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkLeasingAreaExists)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph19AreaExists)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label7)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkNoteEdited)
		Me.SplitContainer1.Panel2.Controls.Add(Me.cmdCalc)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label6)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label5)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label4)
		Me.SplitContainer1.Panel2.Controls.Add(Me.txtNote)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph5IP)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph5Page)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkAntiqueSite)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkVerdict)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkForeclosure)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkMortgage)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph126)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph5)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkLeasing)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph19IP)
		Me.SplitContainer1.Panel2.Controls.Add(Me.mskParagraph19Page)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph19)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkSharedHouse)
		Me.SplitContainer1.Panel2.Controls.Add(Me.chkParagraph11a)
		Me.SplitContainer1.Panel2.Controls.Add(Me.CheckBox3)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label8)
		Me.SplitContainer1.Panel2.Controls.Add(Me.CheckBox4)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Button1)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label9)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label10)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label11)
		Me.SplitContainer1.Panel2.Controls.Add(Me.TextBox1)
		Me.SplitContainer1.Panel2.Controls.Add(Me.MaskedTextBox1)
		Me.SplitContainer1.Panel2.Controls.Add(Me.MaskedTextBox2)
		Me.SplitContainer1.Panel2.Controls.Add(Me.MaskedTextBox3)
		Me.SplitContainer1.Panel2.Controls.Add(Me.MaskedTextBox4)
		Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.SplitContainer1.Size = New System.Drawing.Size(455, 392)
		Me.SplitContainer1.SplitterDistance = 128
		Me.SplitContainer1.TabIndex = 25
		'
		'dgvMain
		'
		Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ccbOwner, Me.ctxDividend, Me.ctxDivisor, Me.ctxPartPct, Me.cchSum, Me.ctxID, Me.ctxOwner, Me.ctxRowr})
		Me.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill
		Me.dgvMain.Location = New System.Drawing.Point(0, 0)
		Me.dgvMain.MultiSelect = False
		Me.dgvMain.Name = "dgvMain"
		Me.dgvMain.RowHeadersWidth = 23
		Me.dgvMain.ScrollBars = System.Windows.Forms.ScrollBars.None
		Me.dgvMain.Size = New System.Drawing.Size(455, 128)
		Me.dgvMain.TabIndex = 1
		'
		'chkDemolitionOrder
		'
		Me.chkDemolitionOrder.AutoSize = True
		Me.chkDemolitionOrder.Font = New System.Drawing.Font("Tahoma", 9.0!)
		Me.chkDemolitionOrder.Location = New System.Drawing.Point(161, 226)
		Me.chkDemolitionOrder.Name = "chkDemolitionOrder"
		Me.chkDemolitionOrder.Size = New System.Drawing.Size(75, 18)
		Me.chkDemolitionOrder.TabIndex = 68
		Me.chkDemolitionOrder.Text = "צו הריסה"
		Me.chkDemolitionOrder.UseVisualStyleBackColor = False
		'
		'mskRegulation29IP
		'
		Me.mskRegulation29IP.Location = New System.Drawing.Point(58, 90)
		Me.mskRegulation29IP.Name = "mskRegulation29IP"
		Me.mskRegulation29IP.Size = New System.Drawing.Size(45, 22)
		Me.mskRegulation29IP.TabIndex = 67
		'
		'mskRegulation29Page
		'
		Me.mskRegulation29Page.Location = New System.Drawing.Point(4, 90)
		Me.mskRegulation29Page.Name = "mskRegulation29Page"
		Me.mskRegulation29Page.Size = New System.Drawing.Size(45, 22)
		Me.mskRegulation29Page.TabIndex = 66
		'
		'mskParagraph123IP
		'
		Me.mskParagraph123IP.Location = New System.Drawing.Point(58, 66)
		Me.mskParagraph123IP.Name = "mskParagraph123IP"
		Me.mskParagraph123IP.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph123IP.TabIndex = 65
		'
		'mskParagraph123Page
		'
		Me.mskParagraph123Page.Location = New System.Drawing.Point(4, 66)
		Me.mskParagraph123Page.Name = "mskParagraph123Page"
		Me.mskParagraph123Page.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph123Page.TabIndex = 64
		'
		'chkParagraph123
		'
		Me.chkParagraph123.AutoSize = True
		Me.chkParagraph123.Font = New System.Drawing.Font("Tahoma", 9.0!)
		Me.chkParagraph123.Location = New System.Drawing.Point(157, 66)
		Me.chkParagraph123.Name = "chkParagraph123"
		Me.chkParagraph123.Size = New System.Drawing.Size(79, 18)
		Me.chkParagraph123.TabIndex = 63
		Me.chkParagraph123.Text = "סעיף 123"
		Me.chkParagraph123.UseVisualStyleBackColor = False
		'
		'chkRegulation29
		'
		Me.chkRegulation29.AutoSize = True
		Me.chkRegulation29.Font = New System.Drawing.Font("Tahoma", 9.0!)
		Me.chkRegulation29.Location = New System.Drawing.Point(161, 90)
		Me.chkRegulation29.Name = "chkRegulation29"
		Me.chkRegulation29.Size = New System.Drawing.Size(75, 18)
		Me.chkRegulation29.TabIndex = 62
		Me.chkRegulation29.Text = "תקנה 29"
		Me.chkRegulation29.UseVisualStyleBackColor = False
		'
		'chkRoadOrdinance
		'
		Me.chkRoadOrdinance.AutoSize = True
		Me.chkRoadOrdinance.Font = New System.Drawing.Font("Tahoma", 9.0!)
		Me.chkRoadOrdinance.Location = New System.Drawing.Point(128, 204)
		Me.chkRoadOrdinance.Name = "chkRoadOrdinance"
		Me.chkRoadOrdinance.Size = New System.Drawing.Size(108, 18)
		Me.chkRoadOrdinance.TabIndex = 61
		Me.chkRoadOrdinance.Text = "פקודת הדרכים"
		Me.chkRoadOrdinance.UseVisualStyleBackColor = False
		'
		'chkLeasingAreaExists
		'
		Me.chkLeasingAreaExists.Appearance = System.Windows.Forms.Appearance.Button
		Me.chkLeasingAreaExists.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.chkLeasingAreaExists.Enabled = False
		Me.chkLeasingAreaExists.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.chkLeasingAreaExists.Location = New System.Drawing.Point(120, 130)
		Me.chkLeasingAreaExists.Name = "chkLeasingAreaExists"
		Me.chkLeasingAreaExists.Size = New System.Drawing.Size(18, 18)
		Me.chkLeasingAreaExists.TabIndex = 60
		Me.chkLeasingAreaExists.Text = "  "
		Me.chkLeasingAreaExists.UseVisualStyleBackColor = True
		'
		'chkParagraph19AreaExists
		'
		Me.chkParagraph19AreaExists.Appearance = System.Windows.Forms.Appearance.Button
		Me.chkParagraph19AreaExists.Enabled = False
		Me.chkParagraph19AreaExists.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.chkParagraph19AreaExists.Location = New System.Drawing.Point(120, 20)
		Me.chkParagraph19AreaExists.Name = "chkParagraph19AreaExists"
		Me.chkParagraph19AreaExists.Size = New System.Drawing.Size(18, 18)
		Me.chkParagraph19AreaExists.TabIndex = 59
		Me.chkParagraph19AreaExists.Text = " "
		Me.chkParagraph19AreaExists.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.chkParagraph19AreaExists.UseVisualStyleBackColor = True
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label7.Location = New System.Drawing.Point(116, 0)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(29, 13)
		Me.Label7.TabIndex = 58
		Me.Label7.Text = "חלק"
		'
		'chkNoteEdited
		'
		Me.chkNoteEdited.AutoSize = True
		Me.chkNoteEdited.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.chkNoteEdited.Location = New System.Drawing.Point(270, 0)
		Me.chkNoteEdited.Name = "chkNoteEdited"
		Me.chkNoteEdited.Size = New System.Drawing.Size(50, 17)
		Me.chkNoteEdited.TabIndex = 57
		Me.chkNoteEdited.Text = "ערוך"
		Me.chkNoteEdited.UseVisualStyleBackColor = True
		'
		'cmdCalc
		'
		Me.cmdCalc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.cmdCalc.Image = Global.TopoUI.My.Resources.Resources.Invert121
		Me.cmdCalc.Location = New System.Drawing.Point(360, 0)
		Me.cmdCalc.Margin = New System.Windows.Forms.Padding(0)
		Me.cmdCalc.Name = "cmdCalc"
		Me.cmdCalc.Size = New System.Drawing.Size(26, 18)
		Me.cmdCalc.TabIndex = 26
		Me.cmdCalc.UseVisualStyleBackColor = True
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label6.Location = New System.Drawing.Point(12, 0)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(24, 13)
		Me.Label6.TabIndex = 56
		Me.Label6.Text = "עמ'"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label5.Location = New System.Drawing.Point(61, 0)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(47, 13)
		Me.Label5.TabIndex = 55
		Me.Label5.Text = "מס' י.פ."
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(410, 0)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(42, 14)
		Me.Label4.TabIndex = 54
		Me.Label4.Text = "הערה:"
		'
		'txtNote
		'
		Me.txtNote.BackColor = System.Drawing.SystemColors.Window
		Me.txtNote.Location = New System.Drawing.Point(247, 21)
		Me.txtNote.Multiline = True
		Me.txtNote.Name = "txtNote"
		Me.txtNote.Size = New System.Drawing.Size(202, 158)
		Me.txtNote.TabIndex = 53
		'
		'mskParagraph5IP
		'
		Me.mskParagraph5IP.Location = New System.Drawing.Point(58, 42)
		Me.mskParagraph5IP.Name = "mskParagraph5IP"
		Me.mskParagraph5IP.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph5IP.TabIndex = 52
		'
		'mskParagraph5Page
		'
		Me.mskParagraph5Page.Location = New System.Drawing.Point(4, 42)
		Me.mskParagraph5Page.Name = "mskParagraph5Page"
		Me.mskParagraph5Page.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph5Page.TabIndex = 51
		'
		'chkAntiqueSite
		'
		Me.chkAntiqueSite.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.chkAntiqueSite.Location = New System.Drawing.Point(112, 172)
		Me.chkAntiqueSite.Name = "chkAntiqueSite"
		Me.chkAntiqueSite.Size = New System.Drawing.Size(124, 32)
		Me.chkAntiqueSite.TabIndex = 49
		Me.chkAntiqueSite.Text = "הערה בדבר אתר עתיקות"
		Me.chkAntiqueSite.TextAlign = System.Drawing.ContentAlignment.TopLeft
		Me.chkAntiqueSite.UseVisualStyleBackColor = True
		'
		'chkVerdict
		'
		Me.chkVerdict.Location = New System.Drawing.Point(34, 202)
		Me.chkVerdict.Name = "chkVerdict"
		Me.chkVerdict.Size = New System.Drawing.Size(74, 18)
		Me.chkVerdict.TabIndex = 48
		Me.chkVerdict.Text = "פסק דין"
		Me.chkVerdict.UseVisualStyleBackColor = True
		'
		'chkForeclosure
		'
		Me.chkForeclosure.Location = New System.Drawing.Point(34, 178)
		Me.chkForeclosure.Name = "chkForeclosure"
		Me.chkForeclosure.Size = New System.Drawing.Size(74, 18)
		Me.chkForeclosure.TabIndex = 47
		Me.chkForeclosure.Text = "צו עיקול"
		Me.chkForeclosure.UseVisualStyleBackColor = True
		'
		'chkMortgage
		'
		Me.chkMortgage.Location = New System.Drawing.Point(34, 154)
		Me.chkMortgage.Name = "chkMortgage"
		Me.chkMortgage.Size = New System.Drawing.Size(74, 18)
		Me.chkMortgage.TabIndex = 46
		Me.chkMortgage.Text = "משכנתא"
		Me.chkMortgage.UseVisualStyleBackColor = True
		'
		'chkParagraph126
		'
		Me.chkParagraph126.Location = New System.Drawing.Point(28, 130)
		Me.chkParagraph126.Name = "chkParagraph126"
		Me.chkParagraph126.Size = New System.Drawing.Size(80, 18)
		Me.chkParagraph126.TabIndex = 45
		Me.chkParagraph126.Text = "סעיף 126"
		Me.chkParagraph126.UseVisualStyleBackColor = True
		'
		'chkParagraph5
		'
		Me.chkParagraph5.Location = New System.Drawing.Point(136, 42)
		Me.chkParagraph5.Name = "chkParagraph5"
		Me.chkParagraph5.Size = New System.Drawing.Size(100, 18)
		Me.chkParagraph5.TabIndex = 44
		Me.chkParagraph5.Text = "סעיפים 5 ו-7"
		Me.chkParagraph5.UseVisualStyleBackColor = True
		'
		'chkLeasing
		'
		Me.chkLeasing.Location = New System.Drawing.Point(136, 130)
		Me.chkLeasing.Name = "chkLeasing"
		Me.chkLeasing.Size = New System.Drawing.Size(100, 18)
		Me.chkLeasing.TabIndex = 43
		Me.chkLeasing.Text = "חכירה"
		Me.chkLeasing.UseVisualStyleBackColor = True
		'
		'mskParagraph19IP
		'
		Me.mskParagraph19IP.Location = New System.Drawing.Point(58, 18)
		Me.mskParagraph19IP.Name = "mskParagraph19IP"
		Me.mskParagraph19IP.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph19IP.TabIndex = 42
		'
		'mskParagraph19Page
		'
		Me.mskParagraph19Page.Location = New System.Drawing.Point(4, 18)
		Me.mskParagraph19Page.Name = "mskParagraph19Page"
		Me.mskParagraph19Page.Size = New System.Drawing.Size(45, 22)
		Me.mskParagraph19Page.TabIndex = 41
		'
		'chkParagraph19
		'
		Me.chkParagraph19.Location = New System.Drawing.Point(136, 18)
		Me.chkParagraph19.Name = "chkParagraph19"
		Me.chkParagraph19.Size = New System.Drawing.Size(100, 18)
		Me.chkParagraph19.TabIndex = 40
		Me.chkParagraph19.Text = "סעיף 19"
		Me.chkParagraph19.UseVisualStyleBackColor = True
		'
		'chkSharedHouse
		'
		Me.chkSharedHouse.Location = New System.Drawing.Point(136, 154)
		Me.chkSharedHouse.Name = "chkSharedHouse"
		Me.chkSharedHouse.Size = New System.Drawing.Size(100, 18)
		Me.chkSharedHouse.TabIndex = 39
		Me.chkSharedHouse.Text = "בית משותף"
		Me.chkSharedHouse.UseVisualStyleBackColor = True
		'
		'chkParagraph11a
		'
		Me.chkParagraph11a.AutoSize = True
		Me.chkParagraph11a.Font = New System.Drawing.Font("Tahoma", 9.0!)
		Me.chkParagraph11a.Location = New System.Drawing.Point(28, 226)
		Me.chkParagraph11a.Name = "chkParagraph11a"
		Me.chkParagraph11a.Size = New System.Drawing.Size(80, 18)
		Me.chkParagraph11a.TabIndex = 60
		Me.chkParagraph11a.Text = "סעיף 11א"
		Me.chkParagraph11a.UseVisualStyleBackColor = False
		'
		'CheckBox3
		'
		Me.CheckBox3.Appearance = System.Windows.Forms.Appearance.Button
		Me.CheckBox3.Enabled = False
		Me.CheckBox3.Font = New System.Drawing.Font("Calibri", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.CheckBox3.Location = New System.Drawing.Point(120, 20)
		Me.CheckBox3.Name = "CheckBox3"
		Me.CheckBox3.Size = New System.Drawing.Size(18, 18)
		Me.CheckBox3.TabIndex = 59
		Me.CheckBox3.Text = " "
		Me.CheckBox3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
		Me.CheckBox3.UseVisualStyleBackColor = True
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label8.Location = New System.Drawing.Point(116, 0)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(29, 13)
		Me.Label8.TabIndex = 58
		Me.Label8.Text = "חלק"
		'
		'CheckBox4
		'
		Me.CheckBox4.AutoSize = True
		Me.CheckBox4.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.CheckBox4.Location = New System.Drawing.Point(270, 0)
		Me.CheckBox4.Name = "CheckBox4"
		Me.CheckBox4.Size = New System.Drawing.Size(50, 17)
		Me.CheckBox4.TabIndex = 57
		Me.CheckBox4.Text = "ערוך"
		Me.CheckBox4.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.Button1.Image = Global.TopoUI.My.Resources.Resources.Invert121
		Me.Button1.Location = New System.Drawing.Point(360, 0)
		Me.Button1.Margin = New System.Windows.Forms.Padding(0)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(26, 18)
		Me.Button1.TabIndex = 26
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label9.Location = New System.Drawing.Point(12, 0)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(24, 13)
		Me.Label9.TabIndex = 56
		Me.Label9.Text = "עמ'"
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label10.Location = New System.Drawing.Point(61, 0)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(47, 13)
		Me.Label10.TabIndex = 55
		Me.Label10.Text = "מס' י.פ."
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(410, 0)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(42, 14)
		Me.Label11.TabIndex = 54
		Me.Label11.Text = "הערה:"
		'
		'TextBox1
		'
		Me.TextBox1.BackColor = System.Drawing.SystemColors.Window
		Me.TextBox1.Location = New System.Drawing.Point(247, 21)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(202, 158)
		Me.TextBox1.TabIndex = 53
		'
		'MaskedTextBox1
		'
		Me.MaskedTextBox1.Location = New System.Drawing.Point(58, 42)
		Me.MaskedTextBox1.Name = "MaskedTextBox1"
		Me.MaskedTextBox1.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox1.TabIndex = 52
		'
		'MaskedTextBox2
		'
		Me.MaskedTextBox2.Location = New System.Drawing.Point(4, 42)
		Me.MaskedTextBox2.Name = "MaskedTextBox2"
		Me.MaskedTextBox2.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox2.TabIndex = 51
		'
		'MaskedTextBox3
		'
		Me.MaskedTextBox3.Location = New System.Drawing.Point(58, 18)
		Me.MaskedTextBox3.Name = "MaskedTextBox3"
		Me.MaskedTextBox3.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox3.TabIndex = 42
		'
		'MaskedTextBox4
		'
		Me.MaskedTextBox4.Location = New System.Drawing.Point(4, 18)
		Me.MaskedTextBox4.Name = "MaskedTextBox4"
		Me.MaskedTextBox4.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox4.TabIndex = 41
		'
		'ccbOwner
		'
		Me.ccbOwner.DataPropertyName = "OwnerDisp"
		Me.ccbOwner.HeaderText = "בעלים"
		Me.ccbOwner.Name = "ccbOwner"
		Me.ccbOwner.Width = 120
		'
		'ctxDividend
		'
		Me.ctxDividend.DataPropertyName = "Dividend"
		DataGridViewCellStyle3.Format = "N0"
		Me.ctxDividend.DefaultCellStyle = DataGridViewCellStyle3
		Me.ctxDividend.HeaderText = "מונה"
		Me.ctxDividend.Name = "ctxDividend"
		Me.ctxDividend.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDividend.Width = 64
		'
		'ctxDivisor
		'
		Me.ctxDivisor.DataPropertyName = "Divisor"
		Me.ctxDivisor.HeaderText = "מכנה"
		Me.ctxDivisor.Name = "ctxDivisor"
		Me.ctxDivisor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxDivisor.Width = 64
		'
		'ctxPartPct
		'
		Me.ctxPartPct.DataPropertyName = "PartPct"
		DataGridViewCellStyle4.Format = "N4"
		Me.ctxPartPct.DefaultCellStyle = DataGridViewCellStyle4
		Me.ctxPartPct.HeaderText = "אחוז"
		Me.ctxPartPct.Name = "ctxPartPct"
		Me.ctxPartPct.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
		Me.ctxPartPct.Width = 80
		'
		'cchSum
		'
		Me.cchSum.DataPropertyName = "Sum"
		Me.cchSum.FalseValue = ""
		Me.cchSum.HeaderText = "סיכום"
		Me.cchSum.Name = "cchSum"
		Me.cchSum.TrueValue = ""
		Me.cchSum.Width = 42
		'
		'ctxID
		'
		Me.ctxID.DataPropertyName = "OwnerID"
		Me.ctxID.HeaderText = "ID"
		Me.ctxID.Name = "ctxID"
		Me.ctxID.ReadOnly = True
		Me.ctxID.Visible = False
		Me.ctxID.Width = 36
		'
		'ctxOwner
		'
		Me.ctxOwner.DataPropertyName = "OwnerIndex"
		Me.ctxOwner.HeaderText = "O"
		Me.ctxOwner.Name = "ctxOwner"
		Me.ctxOwner.ReadOnly = True
		Me.ctxOwner.Visible = False
		Me.ctxOwner.Width = 36
		'
		'ctxRowr
		'
		Me.ctxRowr.DataPropertyName = "RowIndex"
		Me.ctxRowr.HeaderText = "R"
		Me.ctxRowr.Name = "ctxRowr"
		Me.ctxRowr.ReadOnly = True
		Me.ctxRowr.Visible = False
		Me.ctxRowr.Width = 36
		'
		'frmEditOwners
		'
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
		Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.ClientSize = New System.Drawing.Size(455, 453)
		Me.Controls.Add(Me.SplitContainer1)
		Me.Controls.Add(Me.chkReduction)
		Me.Controls.Add(Me.txtSumSimple)
		Me.Controls.Add(Me.cmdComplete)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.txtSumDec)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtParcelNo)
		Me.Controls.Add(Me.txtBlockAddNo)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtBlockNo)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmEditOwners"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "בעלי חלקה"
		Me.SplitContainer1.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel2.ResumeLayout(False)
		Me.SplitContainer1.Panel2.PerformLayout()
		CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.SplitContainer1.ResumeLayout(False)
		CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.bnsParcel, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmdExit As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdOK As Button
	Private WithEvents txtBlockNo As TextBox
	Private WithEvents Label1 As Label
	Private WithEvents txtBlockAddNo As TextBox
	Private WithEvents Label2 As Label
	Private WithEvents txtParcelNo As TextBox
	Private WithEvents Label3 As Label
	Private WithEvents txtSumDec As TextBox
	Private WithEvents cmdComplete As Button
	Private WithEvents txtSumSimple As TextBox
	Private WithEvents chkReduction As CheckBox
	Private WithEvents SplitContainer1 As SplitContainer
	Private WithEvents chkAntiqueSite As CheckBox
	Private WithEvents chkVerdict As CheckBox
	Private WithEvents chkForeclosure As CheckBox
	Private WithEvents chkMortgage As CheckBox
	Private WithEvents chkParagraph126 As CheckBox
	Private WithEvents chkParagraph5 As CheckBox
	Private WithEvents chkLeasing As CheckBox
	Private WithEvents chkParagraph19 As CheckBox
	Private WithEvents chkSharedHouse As CheckBox
	Private WithEvents Label4 As Label
	Private WithEvents txtNote As TextBox
	Private WithEvents bnsParcel As BindingSource
	Private WithEvents Label5 As Label
	Private WithEvents Label6 As Label
	Private WithEvents mskParagraph19IP As MaskedTextBox
	Private WithEvents cmdCalc As Button
	Private WithEvents mskParagraph19Page As MaskedTextBox
	Private WithEvents mskParagraph5IP As MaskedTextBox
	Private WithEvents mskParagraph5Page As MaskedTextBox
	Private WithEvents chkNoteEdited As CheckBox
	Private WithEvents chkLeasingAreaExists As CheckBox
	Private WithEvents chkParagraph19AreaExists As CheckBox
	Private WithEvents Label7 As Label
	Private WithEvents dgvMain As DataGridView
	Private WithEvents chkParagraph11a As CheckBox
	Private WithEvents CheckBox3 As CheckBox
	Private WithEvents Label8 As Label
	Private WithEvents CheckBox4 As CheckBox
	Private WithEvents Button1 As Button
	Private WithEvents Label9 As Label
	Private WithEvents Label10 As Label
	Private WithEvents Label11 As Label
	Private WithEvents TextBox1 As TextBox
	Private WithEvents MaskedTextBox1 As MaskedTextBox
	Private WithEvents MaskedTextBox2 As MaskedTextBox
	Private WithEvents MaskedTextBox3 As MaskedTextBox
	Private WithEvents MaskedTextBox4 As MaskedTextBox
	Private WithEvents chkParagraph123 As CheckBox
	Private WithEvents chkRegulation29 As CheckBox
	Private WithEvents chkRoadOrdinance As CheckBox
	Private WithEvents mskRegulation29IP As MaskedTextBox
	Private WithEvents mskRegulation29Page As MaskedTextBox
	Private WithEvents mskParagraph123IP As MaskedTextBox
	Private WithEvents mskParagraph123Page As MaskedTextBox
	Private WithEvents chkDemolitionOrder As CheckBox
	Friend WithEvents ccbOwner As DataGridViewComboBoxColumn
	Friend WithEvents ctxDividend As DataGridViewTextBoxColumn
	Friend WithEvents ctxDivisor As DataGridViewTextBoxColumn
	Friend WithEvents ctxPartPct As DataGridViewTextBoxColumn
	Friend WithEvents cchSum As DataGridViewCheckBoxColumn
	Friend WithEvents ctxID As DataGridViewTextBoxColumn
	Friend WithEvents ctxOwner As DataGridViewTextBoxColumn
	Friend WithEvents ctxRowr As DataGridViewTextBoxColumn
End Class
