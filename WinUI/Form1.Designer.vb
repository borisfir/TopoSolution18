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

		Me.txtPlanName = New System.Windows.Forms.TextBox
		Me.txtObjectID = New System.Windows.Forms.TextBox
		Me.Label1 = New System.Windows.Forms.Label
		Me.txtPlanNum = New System.Windows.Forms.TextBox
		Me.Label2 = New System.Windows.Forms.Label
		Me.Label3 = New System.Windows.Forms.Label
		Me.txtPlanAreaLegal = New System.Windows.Forms.TextBox
		Me.Label4 = New System.Windows.Forms.Label
		Me.Label5 = New System.Windows.Forms.Label
		Me.txtEditionNo = New System.Windows.Forms.TextBox
		Me.Label6 = New System.Windows.Forms.Label
		Me.cmbPlanPhase = New System.Windows.Forms.ComboBox
		Me.Button1 = New System.Windows.Forms.Button
		Me.mskEditionDate = New System.Windows.Forms.MaskedTextBox
		Me.Label7 = New System.Windows.Forms.Label
		Me.Label8 = New System.Windows.Forms.Label
		Me.chkContDetailInstr = New System.Windows.Forms.CheckBox
		Me.cmbPlanTypeID = New System.Windows.Forms.ComboBox
		Me.Label9 = New System.Windows.Forms.Label
		Me.ComboBox1 = New System.Windows.Forms.ComboBox
		Me.Label10 = New System.Windows.Forms.Label
		Me.TextBox1 = New System.Windows.Forms.TextBox
		Me.Label11 = New System.Windows.Forms.Label
		Me.ComboBox2 = New System.Windows.Forms.ComboBox
		Me.Label12 = New System.Windows.Forms.Label
		Me.ComboBox3 = New System.Windows.Forms.ComboBox
		Me.CheckBox1 = New System.Windows.Forms.CheckBox
		Me.Label13 = New System.Windows.Forms.Label
		Me.Label14 = New System.Windows.Forms.Label
		Me.Label15 = New System.Windows.Forms.Label
		Me.Label16 = New System.Windows.Forms.Label
		Me.Label17 = New System.Windows.Forms.Label
		Me.Label18 = New System.Windows.Forms.Label
		Me.Label19 = New System.Windows.Forms.Label
		Me.Label20 = New System.Windows.Forms.Label
		Me.Button2 = New System.Windows.Forms.Button
		Me.ccbLocality = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.ccbMunicipialStatus = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.ccbCommittee = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.ccbDistrict = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.ccbSubdistrict = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.ccbAuthorityEntire = New System.Windows.Forms.DataGridViewComboBoxColumn
		Me.TabControl1 = New System.Windows.Forms.TabControl
		Me.tbpMainData = New System.Windows.Forms.TabPage
		Me.TabPage2 = New System.Windows.Forms.TabPage
		Me.tbpPurpose = New System.Windows.Forms.TabPage
		Me.DataGridView1 = New System.Windows.Forms.DataGridView
		dgvPlaceDetailsA = New System.Windows.Forms.DataGridView
		Me.TabControl1.SuspendLayout()
		Me.tbpMainData.SuspendLayout()
		Me.TabPage2.SuspendLayout()
		CType(dgvPlaceDetailsA, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'txtPlanName
		'
		Me.txtPlanName.Location = New System.Drawing.Point(26, 26)
		Me.txtPlanName.Multiline = True
		Me.txtPlanName.Name = "txtPlanName"
		Me.txtPlanName.Size = New System.Drawing.Size(100, 32)
		Me.txtPlanName.TabIndex = 0
		'
		'txtObjectID
		'
		Me.txtObjectID.Location = New System.Drawing.Point(1203, 12)
		Me.txtObjectID.Name = "txtObjectID"
		Me.txtObjectID.Size = New System.Drawing.Size(99, 21)
		Me.txtObjectID.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(146, 29)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(70, 13)
		Me.Label1.TabIndex = 3
		Me.Label1.Text = "שם התוכנית"
		'
		'txtPlanNum
		'
		Me.txtPlanNum.Location = New System.Drawing.Point(222, 53)
		Me.txtPlanNum.Name = "txtPlanNum"
		Me.txtPlanNum.Size = New System.Drawing.Size(96, 21)
		Me.txtPlanNum.TabIndex = 4
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(351, 44)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(72, 13)
		Me.Label2.TabIndex = 5
		Me.Label2.Text = "מס' התוכנית"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(345, 70)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(78, 13)
		Me.Label3.TabIndex = 6
		Me.Label3.Text = "שטח התוכנית"
		'
		'txtPlanAreaLegal
		'
		Me.txtPlanAreaLegal.Location = New System.Drawing.Point(226, 79)
		Me.txtPlanAreaLegal.Name = "txtPlanAreaLegal"
		Me.txtPlanAreaLegal.Size = New System.Drawing.Size(96, 21)
		Me.txtPlanAreaLegal.TabIndex = 7
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(349, 100)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(74, 13)
		Me.Label4.TabIndex = 8
		Me.Label4.Text = "שלב התכנית"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(340, 138)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(83, 13)
		Me.Label5.TabIndex = 10
		Me.Label5.Text = "מספר מהדורה"
		'
		'txtEditionNo
		'
		Me.txtEditionNo.Location = New System.Drawing.Point(206, 131)
		Me.txtEditionNo.Name = "txtEditionNo"
		Me.txtEditionNo.Size = New System.Drawing.Size(96, 21)
		Me.txtEditionNo.TabIndex = 11
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(297, 160)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(126, 13)
		Me.Label6.TabIndex = 12
		Me.Label6.Text = "תאריך עדכון המהדורה"
		'
		'cmbPlanPhase
		'
		Me.cmbPlanPhase.FormattingEnabled = True
		Me.cmbPlanPhase.Location = New System.Drawing.Point(211, 105)
		Me.cmbPlanPhase.Name = "cmbPlanPhase"
		Me.cmbPlanPhase.Size = New System.Drawing.Size(89, 21)
		Me.cmbPlanPhase.TabIndex = 14
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(1022, 8)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(146, 20)
		Me.Button1.TabIndex = 15
		Me.Button1.Text = "Button1"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'mskEditionDate
		'
		Me.mskEditionDate.Location = New System.Drawing.Point(211, 157)
		Me.mskEditionDate.Mask = "00/00/00"
		Me.mskEditionDate.Name = "mskEditionDate"
		Me.mskEditionDate.Size = New System.Drawing.Size(89, 21)
		Me.mskEditionDate.SkipLiterals = False
		Me.mskEditionDate.TabIndex = 16
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(363, 187)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(60, 13)
		Me.Label7.TabIndex = 17
		Me.Label7.Text = "סוג תכנית"
		'
		'Label8
		'
		Me.Label8.Location = New System.Drawing.Point(298, 217)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(125, 30)
		Me.Label8.TabIndex = 18
		Me.Label8.Text = "האם מכילה הוראות של תכנית מפורטת"
		'
		'chkContDetailInstr
		'
		Me.chkContDetailInstr.AutoSize = True
		Me.chkContDetailInstr.Location = New System.Drawing.Point(282, 217)
		Me.chkContDetailInstr.Name = "chkContDetailInstr"
		Me.chkContDetailInstr.Size = New System.Drawing.Size(15, 14)
		Me.chkContDetailInstr.TabIndex = 19
		Me.chkContDetailInstr.UseVisualStyleBackColor = True
		'
		'cmbPlanTypeID
		'
		Me.cmbPlanTypeID.FormattingEnabled = True
		Me.cmbPlanTypeID.Location = New System.Drawing.Point(211, 184)
		Me.cmbPlanTypeID.Name = "cmbPlanTypeID"
		Me.cmbPlanTypeID.Size = New System.Drawing.Size(86, 21)
		Me.cmbPlanTypeID.TabIndex = 20
		'
		'Label9
		'
		Me.Label9.Location = New System.Drawing.Point(298, 247)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(125, 27)
		Me.Label9.TabIndex = 21
		Me.Label9.Text = "מוסד התכנון המוסמך  להפקיד את התוכנית"
		Me.Label9.UseMnemonic = False
		'
		'ComboBox1
		'
		Me.ComboBox1.FormattingEnabled = True
		Me.ComboBox1.Location = New System.Drawing.Point(216, 247)
		Me.ComboBox1.Name = "ComboBox1"
		Me.ComboBox1.Size = New System.Drawing.Size(86, 21)
		Me.ComboBox1.TabIndex = 22
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(341, 277)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(82, 13)
		Me.Label10.TabIndex = 23
		Me.Label10.Text = "לפי סעיף בחוק"
		'
		'TextBox1
		'
		Me.TextBox1.Location = New System.Drawing.Point(213, 274)
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(96, 21)
		Me.TextBox1.TabIndex = 24
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(317, 301)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(106, 13)
		Me.Label11.TabIndex = 25
		Me.Label11.Text = "היתרים או הרשאות"
		'
		'ComboBox2
		'
		Me.ComboBox2.FormattingEnabled = True
		Me.ComboBox2.Location = New System.Drawing.Point(211, 301)
		Me.ComboBox2.Name = "ComboBox2"
		Me.ComboBox2.Size = New System.Drawing.Size(86, 21)
		Me.ComboBox2.TabIndex = 26
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(326, 331)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(97, 13)
		Me.Label12.TabIndex = 27
		Me.Label12.Text = "סוג איחוד וחלוקה"
		'
		'ComboBox3
		'
		Me.ComboBox3.FormattingEnabled = True
		Me.ComboBox3.Location = New System.Drawing.Point(214, 328)
		Me.ComboBox3.Name = "ComboBox3"
		Me.ComboBox3.Size = New System.Drawing.Size(86, 21)
		Me.ComboBox3.TabIndex = 28
		'
		'CheckBox1
		'
		Me.CheckBox1.AutoSize = True
		Me.CheckBox1.Location = New System.Drawing.Point(282, 364)
		Me.CheckBox1.Name = "CheckBox1"
		Me.CheckBox1.Size = New System.Drawing.Size(15, 14)
		Me.CheckBox1.TabIndex = 30
		Me.CheckBox1.UseVisualStyleBackColor = True
		'
		'Label13
		'
		Me.Label13.Location = New System.Drawing.Point(302, 364)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(121, 28)
		Me.Label13.TabIndex = 29
		Me.Label13.Text = "האם כוללת הוראות לענין תכנון תלת מימד"
		'
		'Label14
		'
		Me.Label14.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.Label14.Location = New System.Drawing.Point(1312, 12)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(92, 30)
		Me.Label14.TabIndex = 31
		Me.Label14.Text = "מס' המזהה את המהדורה "
		'
		'Label15
		'
		Me.Label15.AutoSize = True
		Me.Label15.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label15.Location = New System.Drawing.Point(1200, 54)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(141, 14)
		Me.Label15.TabIndex = 32
		Me.Label15.Text = "1. זיהוי וסיווג התוכנית"
		'
		'Label16
		'
		Me.Label16.AutoSize = True
		Me.Label16.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label16.Location = New System.Drawing.Point(1141, 125)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New System.Drawing.Size(217, 14)
		Me.Label16.TabIndex = 33
		Me.Label16.Text = "2. מטרת התוכנית ועיקרי הוראותיה"
		'
		'Label17
		'
		Me.Label17.AutoSize = True
		Me.Label17.Location = New System.Drawing.Point(286, 20)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New System.Drawing.Size(177, 13)
		Me.Label17.TabIndex = 34
		Me.Label17.Text = "נקודת ציון של מרכז(קו רוחב צפון)"
		'
		'Label18
		'
		Me.Label18.AutoSize = True
		Me.Label18.Location = New System.Drawing.Point(286, 48)
		Me.Label18.Name = "Label18"
		Me.Label18.Size = New System.Drawing.Size(182, 13)
		Me.Label18.TabIndex = 35
		Me.Label18.Text = "נקודת ציון של מרכז(קו אורך מזרח)"
		'
		'Label19
		'
		Me.Label19.AutoSize = True
		Me.Label19.Location = New System.Drawing.Point(304, 75)
		Me.Label19.Name = "Label19"
		Me.Label19.Size = New System.Drawing.Size(159, 13)
		Me.Label19.TabIndex = 36
		Me.Label19.Text = "תאור מילולי של מקום התכנית"
		'
		'Label20
		'
		Me.Label20.AutoSize = True
		Me.Label20.Location = New System.Drawing.Point(102, 20)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New System.Drawing.Size(61, 13)
		Me.Label20.TabIndex = 37
		Me.Label20.Text = "פרטי מקום"
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(1003, 54)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(50, 26)
		Me.Button2.TabIndex = 39
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'ccbLocality
		'
		Me.ccbLocality.DataPropertyName = "LocalityID"
		Me.ccbLocality.HeaderText = "יישוב"
		Me.ccbLocality.Name = "ccbLocality"
		'
		'ccbMunicipialStatus
		'
		Me.ccbMunicipialStatus.DataPropertyName = "MunicipialStatus"
		Me.ccbMunicipialStatus.HeaderText = "רשות מקומית"
		Me.ccbMunicipialStatus.Name = "ccbMunicipialStatus"
		Me.ccbMunicipialStatus.ReadOnly = True
		'
		'ccbCommittee
		'
		Me.ccbCommittee.DataPropertyName = "CommitteeID"
		Me.ccbCommittee.HeaderText = "מרחב תכנון"
		Me.ccbCommittee.Name = "ccbCommittee"
		Me.ccbCommittee.ReadOnly = True
		Me.ccbCommittee.ToolTipText = "מרחב תכנון מקומי"
		'
		'ccbDistrict
		'
		Me.ccbDistrict.DataPropertyName = "DistrictID"
		Me.ccbDistrict.HeaderText = "מחוז"
		Me.ccbDistrict.Name = "ccbDistrict"
		Me.ccbDistrict.ReadOnly = True
		'
		'ccbSubdistrict
		'
		Me.ccbSubdistrict.DataPropertyName = "SubdistrictID"
		Me.ccbSubdistrict.HeaderText = "נפה"
		Me.ccbSubdistrict.Name = "ccbSubdistrict"
		Me.ccbSubdistrict.ReadOnly = True
		'
		'ccbAuthorityEntire
		'
		Me.ccbAuthorityEntire.DataPropertyName = "AuthorityEntire"
		Me.ccbAuthorityEntire.HeaderText = "התייחסות לתחום הרשות"
		Me.ccbAuthorityEntire.Name = "ccbAuthorityEntire"
		'
		'TabControl1
		'
		Me.TabControl1.Controls.Add(Me.tbpMainData)
		Me.TabControl1.Controls.Add(Me.TabPage2)
		Me.TabControl1.Controls.Add(Me.tbpPurpose)
		Me.TabControl1.Location = New System.Drawing.Point(12, 12)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New System.Drawing.Size(891, 430)
		Me.TabControl1.TabIndex = 41
		'
		'tbpMainData
		'
		Me.tbpMainData.Controls.Add(Me.txtPlanName)
		Me.tbpMainData.Controls.Add(Me.Label1)
		Me.tbpMainData.Controls.Add(Me.Label9)
		Me.tbpMainData.Controls.Add(Me.txtPlanNum)
		Me.tbpMainData.Controls.Add(Me.Label2)
		Me.tbpMainData.Controls.Add(Me.Label3)
		Me.tbpMainData.Controls.Add(Me.txtPlanAreaLegal)
		Me.tbpMainData.Controls.Add(Me.Label4)
		Me.tbpMainData.Controls.Add(Me.Label5)
		Me.tbpMainData.Controls.Add(Me.txtEditionNo)
		Me.tbpMainData.Controls.Add(Me.CheckBox1)
		Me.tbpMainData.Controls.Add(Me.Label6)
		Me.tbpMainData.Controls.Add(Me.Label13)
		Me.tbpMainData.Controls.Add(Me.cmbPlanPhase)
		Me.tbpMainData.Controls.Add(Me.ComboBox3)
		Me.tbpMainData.Controls.Add(Me.mskEditionDate)
		Me.tbpMainData.Controls.Add(Me.Label12)
		Me.tbpMainData.Controls.Add(Me.Label7)
		Me.tbpMainData.Controls.Add(Me.ComboBox2)
		Me.tbpMainData.Controls.Add(Me.Label8)
		Me.tbpMainData.Controls.Add(Me.Label11)
		Me.tbpMainData.Controls.Add(Me.chkContDetailInstr)
		Me.tbpMainData.Controls.Add(Me.TextBox1)
		Me.tbpMainData.Controls.Add(Me.cmbPlanTypeID)
		Me.tbpMainData.Controls.Add(Me.Label10)
		Me.tbpMainData.Controls.Add(Me.ComboBox1)
		Me.tbpMainData.Location = New System.Drawing.Point(4, 22)
		Me.tbpMainData.Name = "tbpMainData"
		Me.tbpMainData.Padding = New System.Windows.Forms.Padding(3)
		Me.tbpMainData.Size = New System.Drawing.Size(883, 404)
		Me.tbpMainData.TabIndex = 0
		Me.tbpMainData.Text = "זיהוי וסיווג התוכנית"
		Me.tbpMainData.UseVisualStyleBackColor = True
		'
		'TabPage2
		'
		Me.TabPage2.Controls.Add(Me.DataGridView1)
		Me.TabPage2.Controls.Add(Me.Label19)
		Me.TabPage2.Controls.Add(Me.Label17)
		Me.TabPage2.Controls.Add(dgvPlaceDetailsA)
		Me.TabPage2.Controls.Add(Me.Label18)
		Me.TabPage2.Controls.Add(Me.Label20)
		Me.TabPage2.Location = New System.Drawing.Point(4, 22)
		Me.TabPage2.Name = "TabPage2"
		Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPage2.Size = New System.Drawing.Size(883, 404)
		Me.TabPage2.TabIndex = 1
		Me.TabPage2.Text = "מקום התוכנית"
		Me.TabPage2.UseVisualStyleBackColor = True
		'
		'dgvPlaceDetailsA
		'
		dgvPlaceDetailsA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		dgvPlaceDetailsA.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ccbLocality, Me.ccbMunicipialStatus, Me.ccbCommittee, Me.ccbDistrict, Me.ccbSubdistrict, Me.ccbAuthorityEntire})
		dgvPlaceDetailsA.Location = New System.Drawing.Point(3, 101)
		dgvPlaceDetailsA.Name = "dgvPlaceDetailsA"
		dgvPlaceDetailsA.RowHeadersWidth = 24
		dgvPlaceDetailsA.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing
		dgvPlaceDetailsA.Size = New System.Drawing.Size(660, 91)
		dgvPlaceDetailsA.TabIndex = 38
		AddHandler dgvPlaceDetailsA.CellValueChanged, AddressOf Me.dgvPlaceDetailsA_CellValueChanged
		'
		'tbpPurpose
		'
		Me.tbpPurpose.Location = New System.Drawing.Point(4, 22)
		Me.tbpPurpose.Name = "tbpPurpose"
		Me.tbpPurpose.Size = New System.Drawing.Size(883, 404)
		Me.tbpPurpose.TabIndex = 2
		Me.tbpPurpose.Text = "מטרת התוכנית ועיקרי הוראותיה"
		Me.tbpPurpose.UseVisualStyleBackColor = True
		'
		'DataGridView1
		'
		Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.DataGridView1.Location = New System.Drawing.Point(7, 213)
		Me.DataGridView1.Name = "DataGridView1"
		Me.DataGridView1.Size = New System.Drawing.Size(655, 52)
		Me.DataGridView1.TabIndex = 39
		'
		'Form1
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1432, 867)
		Me.Controls.Add(Me.TabControl1)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Label16)
		Me.Controls.Add(Me.Label15)
		Me.Controls.Add(Me.Label14)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.txtObjectID)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Name = "Form1"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "Form1"
		Me.TabControl1.ResumeLayout(False)
		Me.tbpMainData.ResumeLayout(False)
		Me.tbpMainData.PerformLayout()
		Me.TabPage2.ResumeLayout(False)
		Me.TabPage2.PerformLayout()
		CType(dgvPlaceDetailsA, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents txtPlanName As System.Windows.Forms.TextBox
	Private WithEvents txtObjectID As System.Windows.Forms.TextBox
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents Label6 As System.Windows.Forms.Label
	Private WithEvents txtPlanNum As System.Windows.Forms.TextBox
	Private WithEvents txtPlanAreaLegal As System.Windows.Forms.TextBox
	Private WithEvents txtEditionNo As System.Windows.Forms.TextBox
	Private WithEvents cmbPlanPhase As System.Windows.Forms.ComboBox
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Private WithEvents mskEditionDate As System.Windows.Forms.MaskedTextBox
	Private WithEvents Label7 As System.Windows.Forms.Label
	Private WithEvents chkContDetailInstr As System.Windows.Forms.CheckBox
	Private WithEvents cmbPlanTypeID As System.Windows.Forms.ComboBox
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Private WithEvents ComboBox1 As System.Windows.Forms.ComboBox
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Private WithEvents TextBox1 As System.Windows.Forms.TextBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Private WithEvents ComboBox2 As System.Windows.Forms.ComboBox
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Private WithEvents ComboBox3 As System.Windows.Forms.ComboBox
	Private WithEvents CheckBox1 As System.Windows.Forms.CheckBox
	Private WithEvents Label14 As System.Windows.Forms.Label
	Friend WithEvents Label16 As System.Windows.Forms.Label
	Private WithEvents Label15 As System.Windows.Forms.Label
	Private WithEvents Label8 As System.Windows.Forms.Label
	Private WithEvents Label13 As System.Windows.Forms.Label
	Private WithEvents Label17 As System.Windows.Forms.Label
	Private WithEvents Label18 As System.Windows.Forms.Label
	Private WithEvents Label19 As System.Windows.Forms.Label
	Private WithEvents Label20 As System.Windows.Forms.Label
	Friend WithEvents Button2 As System.Windows.Forms.Button

	Private Sub ccbLocality_Disposed(ByVal sender As Object, ByVal e As System.EventArgs)

	End Sub
	Friend dgvPlaceDetailsA As System.Windows.Forms.DataGridView
	Friend WithEvents ccbLocality As System.Windows.Forms.DataGridViewComboBoxColumn
	Friend WithEvents ccbMunicipialStatus As System.Windows.Forms.DataGridViewComboBoxColumn
	Friend WithEvents ccbCommittee As System.Windows.Forms.DataGridViewComboBoxColumn
	Friend WithEvents ccbDistrict As System.Windows.Forms.DataGridViewComboBoxColumn
	Friend WithEvents ccbSubdistrict As System.Windows.Forms.DataGridViewComboBoxColumn
	Friend WithEvents ccbAuthorityEntire As System.Windows.Forms.DataGridViewComboBoxColumn
	Private WithEvents TabControl1 As System.Windows.Forms.TabControl
	Private WithEvents tbpMainData As System.Windows.Forms.TabPage
	Private WithEvents TabPage2 As System.Windows.Forms.TabPage
	Private WithEvents tbpPurpose As System.Windows.Forms.TabPage
	Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
End Class
'Friend WithEvents dgvPlaceDetailsA As System.Windows.Forms.DataGridView
'dgvPlaceDetailsA = new System.Windows.Forms.DataGridView