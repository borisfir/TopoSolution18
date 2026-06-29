<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditTheme
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
		Me.cmbTheme = New System.Windows.Forms.ComboBox()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.rdbClosedPolygons = New System.Windows.Forms.RadioButton()
		Me.rdbTopology = New System.Windows.Forms.RadioButton()
		Me.tabThemeInfo = New System.Windows.Forms.TabControl()
		Me.TabPage1 = New System.Windows.Forms.TabPage()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.txtBlockLayer = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtBlockName = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.txtLinkLayers = New System.Windows.Forms.TextBox()
		Me.TabPage2 = New System.Windows.Forms.TabPage()
		Me.lblCover = New System.Windows.Forms.Label()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.ComboBox1 = New System.Windows.Forms.ComboBox()
		Me.GroupBox1.SuspendLayout()
		Me.tabThemeInfo.SuspendLayout()
		Me.TabPage1.SuspendLayout()
		Me.SuspendLayout()
		'
		'cmbTheme
		'
		Me.cmbTheme.FormattingEnabled = True
		Me.cmbTheme.Location = New System.Drawing.Point(254, 22)
		Me.cmbTheme.Name = "cmbTheme"
		Me.cmbTheme.Size = New System.Drawing.Size(223, 22)
		Me.cmbTheme.TabIndex = 0
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.rdbClosedPolygons)
		Me.GroupBox1.Controls.Add(Me.rdbTopology)
		Me.GroupBox1.Location = New System.Drawing.Point(14, 3)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(208, 67)
		Me.GroupBox1.TabIndex = 1
		Me.GroupBox1.TabStop = False
		'
		'rdbClosedPolygons
		'
		Me.rdbClosedPolygons.AutoSize = True
		Me.rdbClosedPolygons.Location = New System.Drawing.Point(59, 44)
		Me.rdbClosedPolygons.Name = "rdbClosedPolygons"
		Me.rdbClosedPolygons.Size = New System.Drawing.Size(111, 18)
		Me.rdbClosedPolygons.TabIndex = 1
		Me.rdbClosedPolygons.TabStop = True
		Me.rdbClosedPolygons.Text = "פוליגונים סגורים"
		Me.rdbClosedPolygons.UseVisualStyleBackColor = True
		'
		'rdbTopology
		'
		Me.rdbTopology.AutoSize = True
		Me.rdbTopology.Location = New System.Drawing.Point(96, 20)
		Me.rdbTopology.Name = "rdbTopology"
		Me.rdbTopology.Size = New System.Drawing.Size(74, 18)
		Me.rdbTopology.TabIndex = 0
		Me.rdbTopology.TabStop = True
		Me.rdbTopology.Text = "טופולוגוה"
		Me.rdbTopology.UseVisualStyleBackColor = True
		'
		'tabThemeInfo
		'
		Me.tabThemeInfo.Appearance = System.Windows.Forms.TabAppearance.FlatButtons
		Me.tabThemeInfo.Controls.Add(Me.TabPage1)
		Me.tabThemeInfo.Controls.Add(Me.TabPage2)
		Me.tabThemeInfo.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.tabThemeInfo.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed
		Me.tabThemeInfo.ItemSize = New System.Drawing.Size(20, 21)
		Me.tabThemeInfo.Location = New System.Drawing.Point(0, 109)
		Me.tabThemeInfo.Name = "tabThemeInfo"
		Me.tabThemeInfo.SelectedIndex = 0
		Me.tabThemeInfo.Size = New System.Drawing.Size(489, 250)
		Me.tabThemeInfo.TabIndex = 2
		'
		'TabPage1
		'
		Me.TabPage1.Controls.Add(Me.Label4)
		Me.TabPage1.Controls.Add(Me.txtBlockLayer)
		Me.TabPage1.Controls.Add(Me.Label3)
		Me.TabPage1.Controls.Add(Me.txtBlockName)
		Me.TabPage1.Controls.Add(Me.Label2)
		Me.TabPage1.Controls.Add(Me.txtLinkLayers)
		Me.TabPage1.Location = New System.Drawing.Point(4, 25)
		Me.TabPage1.Name = "TabPage1"
		Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPage1.Size = New System.Drawing.Size(481, 221)
		Me.TabPage1.TabIndex = 0
		'
		'Label4
		'
		Me.Label4.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label4.Location = New System.Drawing.Point(393, 75)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(80, 14)
		Me.Label4.TabIndex = 5
		Me.Label4.Text = "שכבת בלוק"
		'
		'txtBlockLayer
		'
		Me.txtBlockLayer.Location = New System.Drawing.Point(280, 72)
		Me.txtBlockLayer.Name = "txtBlockLayer"
		Me.txtBlockLayer.Size = New System.Drawing.Size(94, 22)
		Me.txtBlockLayer.TabIndex = 4
		'
		'Label3
		'
		Me.Label3.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label3.Location = New System.Drawing.Point(393, 47)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(80, 14)
		Me.Label3.TabIndex = 3
		Me.Label3.Text = "שם בלוק"
		'
		'txtBlockName
		'
		Me.txtBlockName.Location = New System.Drawing.Point(280, 44)
		Me.txtBlockName.Name = "txtBlockName"
		Me.txtBlockName.Size = New System.Drawing.Size(94, 22)
		Me.txtBlockName.TabIndex = 2
		'
		'Label2
		'
		Me.Label2.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label2.Location = New System.Drawing.Point(393, 16)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(80, 14)
		Me.Label2.TabIndex = 1
		Me.Label2.Text = "שכבות"
		'
		'txtLinkLayers
		'
		Me.txtLinkLayers.Location = New System.Drawing.Point(280, 16)
		Me.txtLinkLayers.Name = "txtLinkLayers"
		Me.txtLinkLayers.Size = New System.Drawing.Size(94, 22)
		Me.txtLinkLayers.TabIndex = 0
		'
		'TabPage2
		'
		Me.TabPage2.Location = New System.Drawing.Point(4, 25)
		Me.TabPage2.Name = "TabPage2"
		Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPage2.Size = New System.Drawing.Size(481, 221)
		Me.TabPage2.TabIndex = 1
		Me.TabPage2.Text = "TabPage2"
		Me.TabPage2.UseVisualStyleBackColor = True
		'
		'lblCover
		'
		Me.lblCover.Location = New System.Drawing.Point(355, 85)
		Me.lblCover.Name = "lblCover"
		Me.lblCover.Size = New System.Drawing.Size(125, 25)
		Me.lblCover.TabIndex = 3
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New System.Drawing.Point(441, 3)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(36, 14)
		Me.Label1.TabIndex = 4
		Me.Label1.Text = "נושא"
		'
		'cmdOK
		'
		Me.cmdOK.Location = New System.Drawing.Point(12, 82)
		Me.cmdOK.Name = "cmdOK"
		Me.cmdOK.Size = New System.Drawing.Size(57, 21)
		Me.cmdOK.TabIndex = 5
		Me.cmdOK.Text = "OK"
		Me.cmdOK.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.Location = New System.Drawing.Point(92, 82)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(57, 21)
		Me.cmdCancel.TabIndex = 6
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label5.Location = New System.Drawing.Point(392, 50)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(83, 14)
		Me.Label5.TabIndex = 8
		Me.Label5.Text = "נושא לחיתוך"
		'
		'ComboBox1
		'
		Me.ComboBox1.FormattingEnabled = True
		Me.ComboBox1.Location = New System.Drawing.Point(254, 69)
		Me.ComboBox1.Name = "ComboBox1"
		Me.ComboBox1.Size = New System.Drawing.Size(223, 22)
		Me.ComboBox1.TabIndex = 7
		'
		'frmEditTheme
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(489, 359)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.ComboBox1)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.lblCover)
		Me.Controls.Add(Me.tabThemeInfo)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.cmbTheme)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Name = "frmEditTheme"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "frmEditTheme"
		Me.TransparencyKey = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.tabThemeInfo.ResumeLayout(False)
		Me.TabPage1.ResumeLayout(False)
		Me.TabPage1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
	Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
	Private WithEvents tabThemeInfo As System.Windows.Forms.TabControl
	Private WithEvents lblCover As System.Windows.Forms.Label
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents cmbTheme As System.Windows.Forms.ComboBox
	Private WithEvents rdbTopology As System.Windows.Forms.RadioButton
	Private WithEvents rdbClosedPolygons As System.Windows.Forms.RadioButton
	Private WithEvents txtLinkLayers As System.Windows.Forms.TextBox
	Private WithEvents txtBlockName As System.Windows.Forms.TextBox
	Private WithEvents txtBlockLayer As System.Windows.Forms.TextBox
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents ComboBox1 As System.Windows.Forms.ComboBox
	Private WithEvents Label1 As System.Windows.Forms.Label
End Class
