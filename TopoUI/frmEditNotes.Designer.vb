<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmEditNotes
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditNotes))
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.txtBlockNo = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtBlockAddNo = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtParcelNo = New System.Windows.Forms.TextBox()
		Me.txtNote = New System.Windows.Forms.TextBox()
		Me.chkSharedHouse = New System.Windows.Forms.CheckBox()
		Me.bnsParcel = New System.Windows.Forms.BindingSource(Me.components)
		Me.chkParagraph19 = New System.Windows.Forms.CheckBox()
		Me.MaskedTextBox1 = New System.Windows.Forms.MaskedTextBox()
		Me.MaskedTextBox2 = New System.Windows.Forms.MaskedTextBox()
		Me.chkLeasing = New System.Windows.Forms.CheckBox()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.chkParagraph5 = New System.Windows.Forms.CheckBox()
		Me.chkParagraph126 = New System.Windows.Forms.CheckBox()
		Me.chkMortgage = New System.Windows.Forms.CheckBox()
		Me.chkForeclosure = New System.Windows.Forms.CheckBox()
		Me.chkVerdict = New System.Windows.Forms.CheckBox()
		Me.chkTreasurerNote = New System.Windows.Forms.CheckBox()
		Me.MaskedTextBox3 = New System.Windows.Forms.MaskedTextBox()
		Me.MaskedTextBox4 = New System.Windows.Forms.MaskedTextBox()
		Me.CheckBox1 = New System.Windows.Forms.CheckBox()
		CType(Me.bnsParcel, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'cmdExit
		'
		Me.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
		Me.cmdExit.Image = CType(resources.GetObject("cmdExit.Image"), System.Drawing.Image)
		Me.cmdExit.Location = New System.Drawing.Point(269, 12)
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
		Me.cmdCancel.Location = New System.Drawing.Point(239, 12)
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
		Me.cmdOK.Location = New System.Drawing.Point(209, 12)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(26, 26)
		Me.cmdOK.TabIndex = 11
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
		Me.Button1.Location = New System.Drawing.Point(434, 2)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(28, 30)
		Me.Button1.TabIndex = 14
		Me.Button1.UseVisualStyleBackColor = True
		Me.Button1.Visible = False
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
		Me.txtBlockAddNo.ReadOnly = True
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
		'txtNote
		'
		Me.txtNote.BackColor = System.Drawing.SystemColors.Window
		Me.txtNote.Location = New System.Drawing.Point(12, 83)
		Me.txtNote.Multiline = True
		Me.txtNote.Name = "txtNote"
		Me.txtNote.Size = New System.Drawing.Size(180, 181)
		Me.txtNote.TabIndex = 23
		'
		'chkSharedHouse
		'
		Me.chkSharedHouse.AutoSize = True
		Me.chkSharedHouse.Location = New System.Drawing.Point(12, 246)
		Me.chkSharedHouse.Name = "chkSharedHouse"
		Me.chkSharedHouse.Size = New System.Drawing.Size(91, 18)
		Me.chkSharedHouse.TabIndex = 24
		Me.chkSharedHouse.Text = "בית משותף"
		Me.chkSharedHouse.UseVisualStyleBackColor = True
		'
		'bnsParcel
		'
		'
		'chkParagraph19
		'
		Me.chkParagraph19.AutoSize = True
		Me.chkParagraph19.Location = New System.Drawing.Point(12, 157)
		Me.chkParagraph19.Name = "chkParagraph19"
		Me.chkParagraph19.Size = New System.Drawing.Size(72, 18)
		Me.chkParagraph19.TabIndex = 25
		Me.chkParagraph19.Text = "סעיף 19"
		Me.chkParagraph19.UseVisualStyleBackColor = True
		'
		'MaskedTextBox1
		'
		Me.MaskedTextBox1.Location = New System.Drawing.Point(96, 157)
		Me.MaskedTextBox1.Name = "MaskedTextBox1"
		Me.MaskedTextBox1.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox1.TabIndex = 26
		'
		'MaskedTextBox2
		'
		Me.MaskedTextBox2.Location = New System.Drawing.Point(159, 157)
		Me.MaskedTextBox2.Name = "MaskedTextBox2"
		Me.MaskedTextBox2.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox2.TabIndex = 27
		'
		'chkLeasing
		'
		Me.chkLeasing.AutoSize = True
		Me.chkLeasing.Location = New System.Drawing.Point(239, 161)
		Me.chkLeasing.Name = "chkLeasing"
		Me.chkLeasing.Size = New System.Drawing.Size(59, 18)
		Me.chkLeasing.TabIndex = 28
		Me.chkLeasing.Text = "חכירה"
		Me.chkLeasing.UseVisualStyleBackColor = True
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(12, 66)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(42, 14)
		Me.Label4.TabIndex = 29
		Me.Label4.Text = "הערה:"
		'
		'chkParagraph5
		'
		Me.chkParagraph5.AutoSize = True
		Me.chkParagraph5.Location = New System.Drawing.Point(239, 185)
		Me.chkParagraph5.Name = "chkParagraph5"
		Me.chkParagraph5.Size = New System.Drawing.Size(94, 18)
		Me.chkParagraph5.TabIndex = 30
		Me.chkParagraph5.Text = "סעיפים 5 ו-7"
		Me.chkParagraph5.UseVisualStyleBackColor = True
		'
		'chkParagraph126
		'
		Me.chkParagraph126.AutoSize = True
		Me.chkParagraph126.Location = New System.Drawing.Point(12, 210)
		Me.chkParagraph126.Name = "chkParagraph126"
		Me.chkParagraph126.Size = New System.Drawing.Size(79, 18)
		Me.chkParagraph126.TabIndex = 31
		Me.chkParagraph126.Text = "סעיף 126"
		Me.chkParagraph126.UseVisualStyleBackColor = True
		'
		'chkMortgage
		'
		Me.chkMortgage.AutoSize = True
		Me.chkMortgage.Location = New System.Drawing.Point(239, 210)
		Me.chkMortgage.Name = "chkMortgage"
		Me.chkMortgage.Size = New System.Drawing.Size(74, 18)
		Me.chkMortgage.TabIndex = 32
		Me.chkMortgage.Text = "משכנתא"
		Me.chkMortgage.UseVisualStyleBackColor = True
		'
		'chkForeclosure
		'
		Me.chkForeclosure.AutoSize = True
		Me.chkForeclosure.Location = New System.Drawing.Point(239, 234)
		Me.chkForeclosure.Name = "chkForeclosure"
		Me.chkForeclosure.Size = New System.Drawing.Size(69, 18)
		Me.chkForeclosure.TabIndex = 33
		Me.chkForeclosure.Text = "צו עיקול"
		Me.chkForeclosure.UseVisualStyleBackColor = True
		'
		'chkVerdict
		'
		Me.chkVerdict.AutoSize = True
		Me.chkVerdict.Location = New System.Drawing.Point(237, 258)
		Me.chkVerdict.Name = "chkVerdict"
		Me.chkVerdict.Size = New System.Drawing.Size(71, 18)
		Me.chkVerdict.TabIndex = 34
		Me.chkVerdict.Text = "פסק דין"
		Me.chkVerdict.UseVisualStyleBackColor = True
		'
		'chkTreasurerNote
		'
		Me.chkTreasurerNote.AutoSize = True
		Me.chkTreasurerNote.Location = New System.Drawing.Point(12, 270)
		Me.chkTreasurerNote.Name = "chkTreasurerNote"
		Me.chkTreasurerNote.Size = New System.Drawing.Size(165, 18)
		Me.chkTreasurerNote.TabIndex = 35
		Me.chkTreasurerNote.Text = "הערת גזבר אתר עתיקות"
		Me.chkTreasurerNote.UseVisualStyleBackColor = True
		'
		'MaskedTextBox3
		'
		Me.MaskedTextBox3.Location = New System.Drawing.Point(159, 185)
		Me.MaskedTextBox3.Name = "MaskedTextBox3"
		Me.MaskedTextBox3.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox3.TabIndex = 38
		'
		'MaskedTextBox4
		'
		Me.MaskedTextBox4.Location = New System.Drawing.Point(96, 185)
		Me.MaskedTextBox4.Name = "MaskedTextBox4"
		Me.MaskedTextBox4.Size = New System.Drawing.Size(45, 22)
		Me.MaskedTextBox4.TabIndex = 37
		'
		'CheckBox1
		'
		Me.CheckBox1.AutoSize = True
		Me.CheckBox1.Location = New System.Drawing.Point(12, 185)
		Me.CheckBox1.Name = "CheckBox1"
		Me.CheckBox1.Size = New System.Drawing.Size(72, 18)
		Me.CheckBox1.TabIndex = 36
		Me.CheckBox1.Text = "סעיף 19"
		Me.CheckBox1.UseVisualStyleBackColor = True
		'
		'frmEditNotes
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(340, 407)
		Me.Controls.Add(Me.MaskedTextBox3)
		Me.Controls.Add(Me.MaskedTextBox4)
		Me.Controls.Add(Me.CheckBox1)
		Me.Controls.Add(Me.chkTreasurerNote)
		Me.Controls.Add(Me.chkVerdict)
		Me.Controls.Add(Me.chkForeclosure)
		Me.Controls.Add(Me.chkMortgage)
		Me.Controls.Add(Me.chkParagraph126)
		Me.Controls.Add(Me.chkParagraph5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.chkLeasing)
		Me.Controls.Add(Me.MaskedTextBox2)
		Me.Controls.Add(Me.MaskedTextBox1)
		Me.Controls.Add(Me.chkParagraph19)
		Me.Controls.Add(Me.chkSharedHouse)
		Me.Controls.Add(Me.txtNote)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.txtParcelNo)
		Me.Controls.Add(Me.txtBlockAddNo)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtBlockNo)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "frmEditNotes"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "בעלי חלקה"
		CType(Me.bnsParcel, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents cmdExit As Button
	Private WithEvents cmdCancel As Button
	Private WithEvents cmdOK As Button
	Private WithEvents Button1 As Button
	Private WithEvents txtBlockNo As TextBox
	Private WithEvents Label1 As Label
	Private WithEvents txtBlockAddNo As TextBox
	Private WithEvents Label2 As Label
	Private WithEvents txtParcelNo As TextBox
	Private WithEvents txtNote As TextBox
	Private WithEvents chkSharedHouse As CheckBox
	Private WithEvents bnsParcel As BindingSource
	Private WithEvents chkParagraph19 As CheckBox
	Friend WithEvents MaskedTextBox1 As MaskedTextBox
	Friend WithEvents MaskedTextBox2 As MaskedTextBox
	Private WithEvents chkLeasing As CheckBox
	Private WithEvents Label4 As Label
	Private WithEvents chkParagraph5 As CheckBox
	Private WithEvents chkParagraph126 As CheckBox
	Private WithEvents chkMortgage As CheckBox
	Private WithEvents chkForeclosure As CheckBox
	Private WithEvents chkVerdict As CheckBox
	Private WithEvents chkTreasurerNote As CheckBox
	Friend WithEvents MaskedTextBox3 As MaskedTextBox
	Friend WithEvents MaskedTextBox4 As MaskedTextBox
	Private WithEvents CheckBox1 As CheckBox
End Class
