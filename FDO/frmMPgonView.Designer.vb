<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMPgonView
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
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.txtIDImp = New System.Windows.Forms.TextBox()
		Me.txtLegalArea = New System.Windows.Forms.TextBox()
		Me.txtID = New System.Windows.Forms.TextBox()
		Me.txtBlock = New System.Windows.Forms.TextBox()
		Me.txtBlockAdd = New System.Windows.Forms.TextBox()
		Me.txtParcel = New System.Windows.Forms.TextBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtAcadArea = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.txtNeigborList = New System.Windows.Forms.TextBox()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.txtExterior = New System.Windows.Forms.TextBox()
		Me.lblIsland = New System.Windows.Forms.Label()
		Me.txtInteriorList = New System.Windows.Forms.TextBox()
		Me.cmdNext = New System.Windows.Forms.Button()
		Me.cmdClose = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(12, 18)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(44, 14)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "ID Imp"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(12, 50)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(19, 14)
		Me.Label2.TabIndex = 1
		Me.Label2.Text = "ID"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(12, 82)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(50, 14)
		Me.Label3.TabIndex = 2
		Me.Label3.Text = "מס' גוש"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(12, 146)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(59, 14)
		Me.Label4.TabIndex = 3
		Me.Label4.Text = "מס חלקה"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(12, 114)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(79, 14)
		Me.Label5.TabIndex = 4
		Me.Label5.Text = "מס' נוסף גוש"
		'
		'txtIDImp
		'
		Me.txtIDImp.Location = New System.Drawing.Point(120, 18)
		Me.txtIDImp.Name = "txtIDImp"
		Me.txtIDImp.ReadOnly = True
		Me.txtIDImp.Size = New System.Drawing.Size(100, 22)
		Me.txtIDImp.TabIndex = 5
		'
		'txtLegalArea
		'
		Me.txtLegalArea.Location = New System.Drawing.Point(120, 178)
		Me.txtLegalArea.Name = "txtLegalArea"
		Me.txtLegalArea.ReadOnly = True
		Me.txtLegalArea.Size = New System.Drawing.Size(100, 22)
		Me.txtLegalArea.TabIndex = 6
		'
		'txtID
		'
		Me.txtID.Location = New System.Drawing.Point(120, 50)
		Me.txtID.Name = "txtID"
		Me.txtID.ReadOnly = True
		Me.txtID.Size = New System.Drawing.Size(100, 22)
		Me.txtID.TabIndex = 6
		'
		'txtBlock
		'
		Me.txtBlock.Location = New System.Drawing.Point(120, 82)
		Me.txtBlock.Name = "txtBlock"
		Me.txtBlock.ReadOnly = True
		Me.txtBlock.Size = New System.Drawing.Size(100, 22)
		Me.txtBlock.TabIndex = 7
		'
		'txtBlockAdd
		'
		Me.txtBlockAdd.Location = New System.Drawing.Point(120, 114)
		Me.txtBlockAdd.Name = "txtBlockAdd"
		Me.txtBlockAdd.ReadOnly = True
		Me.txtBlockAdd.Size = New System.Drawing.Size(100, 22)
		Me.txtBlockAdd.TabIndex = 8
		'
		'txtParcel
		'
		Me.txtParcel.Location = New System.Drawing.Point(120, 146)
		Me.txtParcel.Name = "txtParcel"
		Me.txtParcel.ReadOnly = True
		Me.txtParcel.Size = New System.Drawing.Size(100, 22)
		Me.txtParcel.TabIndex = 9
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(12, 178)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(65, 14)
		Me.Label6.TabIndex = 10
		Me.Label6.Text = "שטח רשום"
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(12, 210)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(37, 14)
		Me.Label7.TabIndex = 12
		Me.Label7.Text = "שטח "
		'
		'txtAcadArea
		'
		Me.txtAcadArea.Location = New System.Drawing.Point(120, 210)
		Me.txtAcadArea.Name = "txtAcadArea"
		Me.txtAcadArea.ReadOnly = True
		Me.txtAcadArea.Size = New System.Drawing.Size(100, 22)
		Me.txtAcadArea.TabIndex = 11
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(12, 242)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(40, 14)
		Me.Label8.TabIndex = 14
		Me.Label8.Text = "שכנים"
		'
		'txtNeigborList
		'
		Me.txtNeigborList.Location = New System.Drawing.Point(120, 242)
		Me.txtNeigborList.Name = "txtNeigborList"
		Me.txtNeigborList.ReadOnly = True
		Me.txtNeigborList.Size = New System.Drawing.Size(134, 22)
		Me.txtNeigborList.TabIndex = 13
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(12, 274)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(36, 14)
		Me.Label9.TabIndex = 16
		Me.Label9.Text = "חיצוני"
		'
		'txtExterior
		'
		Me.txtExterior.Location = New System.Drawing.Point(120, 274)
		Me.txtExterior.Name = "txtExterior"
		Me.txtExterior.ReadOnly = True
		Me.txtExterior.Size = New System.Drawing.Size(100, 22)
		Me.txtExterior.TabIndex = 15
		'
		'lblIsland
		'
		Me.lblIsland.AutoSize = True
		Me.lblIsland.Location = New System.Drawing.Point(12, 306)
		Me.lblIsland.Name = "lblIsland"
		Me.lblIsland.Size = New System.Drawing.Size(29, 14)
		Me.lblIsland.TabIndex = 18
		Me.lblIsland.Text = "איים"
		'
		'txtInteriorList
		'
		Me.txtInteriorList.Location = New System.Drawing.Point(120, 306)
		Me.txtInteriorList.Name = "txtInteriorList"
		Me.txtInteriorList.ReadOnly = True
		Me.txtInteriorList.Size = New System.Drawing.Size(100, 22)
		Me.txtInteriorList.TabIndex = 17
		'
		'cmdNext
		'
		Me.cmdNext.Location = New System.Drawing.Point(173, 338)
		Me.cmdNext.Name = "cmdNext"
		Me.cmdNext.Size = New System.Drawing.Size(45, 24)
		Me.cmdNext.TabIndex = 19
		Me.cmdNext.Text = "Next"
		Me.cmdNext.UseVisualStyleBackColor = True
		'
		'cmdClose
		'
		Me.cmdClose.Location = New System.Drawing.Point(107, 338)
		Me.cmdClose.Name = "cmdClose"
		Me.cmdClose.Size = New System.Drawing.Size(45, 24)
		Me.cmdClose.TabIndex = 20
		Me.cmdClose.Text = "Close"
		Me.cmdClose.UseVisualStyleBackColor = True
		'
		'frmMPgonView
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(266, 366)
		Me.Controls.Add(Me.cmdClose)
		Me.Controls.Add(Me.cmdNext)
		Me.Controls.Add(Me.lblIsland)
		Me.Controls.Add(Me.txtInteriorList)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.txtExterior)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.txtNeigborList)
		Me.Controls.Add(Me.Label7)
		Me.Controls.Add(Me.txtAcadArea)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.txtParcel)
		Me.Controls.Add(Me.txtBlockAdd)
		Me.Controls.Add(Me.txtBlock)
		Me.Controls.Add(Me.txtID)
		Me.Controls.Add(Me.txtLegalArea)
		Me.Controls.Add(Me.txtIDImp)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label1)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "frmMPgonView"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "פוליגון"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents txtIDImp As System.Windows.Forms.TextBox
	Private WithEvents txtLegalArea As System.Windows.Forms.TextBox
	Private WithEvents txtID As System.Windows.Forms.TextBox
	Private WithEvents txtBlock As System.Windows.Forms.TextBox
	Private WithEvents txtBlockAdd As System.Windows.Forms.TextBox
	Private WithEvents txtParcel As System.Windows.Forms.TextBox
	Private WithEvents Label6 As System.Windows.Forms.Label
	Private WithEvents Label7 As System.Windows.Forms.Label
	Private WithEvents txtAcadArea As System.Windows.Forms.TextBox
	Private WithEvents Label8 As System.Windows.Forms.Label
	Private WithEvents txtNeigborList As System.Windows.Forms.TextBox
	Private WithEvents Label9 As System.Windows.Forms.Label
	Private WithEvents txtExterior As System.Windows.Forms.TextBox
	Private WithEvents lblIsland As System.Windows.Forms.Label
	Private WithEvents txtInteriorList As System.Windows.Forms.TextBox
	Private WithEvents cmdNext As System.Windows.Forms.Button
	Private WithEvents cmdClose As System.Windows.Forms.Button
End Class
