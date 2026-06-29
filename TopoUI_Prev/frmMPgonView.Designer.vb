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
      Me.TextBox1 = New System.Windows.Forms.TextBox()
      Me.txtID = New System.Windows.Forms.TextBox()
      Me.txtBlock = New System.Windows.Forms.TextBox()
      Me.txtBlockAdd = New System.Windows.Forms.TextBox()
      Me.txtParcel = New System.Windows.Forms.TextBox()
      Me.SuspendLayout()
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(93, 37)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(44, 14)
      Me.Label1.TabIndex = 0
      Me.Label1.Text = "ID Imp"
      '
      'Label2
      '
      Me.Label2.AutoSize = True
      Me.Label2.Location = New System.Drawing.Point(93, 75)
      Me.Label2.Name = "Label2"
      Me.Label2.Size = New System.Drawing.Size(19, 14)
      Me.Label2.TabIndex = 1
      Me.Label2.Text = "ID"
      '
      'Label3
      '
      Me.Label3.AutoSize = True
      Me.Label3.Location = New System.Drawing.Point(93, 114)
      Me.Label3.Name = "Label3"
      Me.Label3.Size = New System.Drawing.Size(50, 14)
      Me.Label3.TabIndex = 2
      Me.Label3.Text = "מס' גוש"
      '
      'Label4
      '
      Me.Label4.AutoSize = True
      Me.Label4.Location = New System.Drawing.Point(93, 181)
      Me.Label4.Name = "Label4"
      Me.Label4.Size = New System.Drawing.Size(59, 14)
      Me.Label4.TabIndex = 3
      Me.Label4.Text = "מס חלקה"
      '
      'Label5
      '
      Me.Label5.AutoSize = True
      Me.Label5.Location = New System.Drawing.Point(93, 146)
      Me.Label5.Name = "Label5"
      Me.Label5.Size = New System.Drawing.Size(79, 14)
      Me.Label5.TabIndex = 4
      Me.Label5.Text = "מס' נוסף גוש"
      '
      'txtIDImp
      '
      Me.txtIDImp.Location = New System.Drawing.Point(219, 37)
      Me.txtIDImp.Name = "txtIDImp"
      Me.txtIDImp.ReadOnly = True
      Me.txtIDImp.Size = New System.Drawing.Size(100, 22)
      Me.txtIDImp.TabIndex = 5
      '
      'TextBox1
      '
      Me.TextBox1.Location = New System.Drawing.Point(219, 218)
      Me.TextBox1.Name = "TextBox1"
      Me.TextBox1.ReadOnly = True
      Me.TextBox1.Size = New System.Drawing.Size(100, 22)
      Me.TextBox1.TabIndex = 6
      '
      'txtID
      '
      Me.txtID.Location = New System.Drawing.Point(219, 75)
      Me.txtID.Name = "txtID"
      Me.txtID.ReadOnly = True
      Me.txtID.Size = New System.Drawing.Size(100, 22)
      Me.txtID.TabIndex = 6
      '
      'txtBlock
      '
      Me.txtBlock.Location = New System.Drawing.Point(219, 114)
      Me.txtBlock.Name = "txtBlock"
      Me.txtBlock.ReadOnly = True
      Me.txtBlock.Size = New System.Drawing.Size(100, 22)
      Me.txtBlock.TabIndex = 7
      '
      'txtBlockAdd
      '
      Me.txtBlockAdd.Location = New System.Drawing.Point(219, 142)
      Me.txtBlockAdd.Name = "txtBlockAdd"
      Me.txtBlockAdd.ReadOnly = True
      Me.txtBlockAdd.Size = New System.Drawing.Size(100, 22)
      Me.txtBlockAdd.TabIndex = 8
      '
      'txtParcel
      '
      Me.txtParcel.Location = New System.Drawing.Point(219, 181)
      Me.txtParcel.Name = "txtParcel"
      Me.txtParcel.ReadOnly = True
      Me.txtParcel.Size = New System.Drawing.Size(100, 22)
      Me.txtParcel.TabIndex = 9
      '
      'frmMPgonView
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(331, 323)
      Me.Controls.Add(Me.txtParcel)
      Me.Controls.Add(Me.txtBlockAdd)
      Me.Controls.Add(Me.txtBlock)
      Me.Controls.Add(Me.txtID)
      Me.Controls.Add(Me.TextBox1)
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
      Me.Text = "frmMPgonView"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents Label2 As System.Windows.Forms.Label
	Private WithEvents Label3 As System.Windows.Forms.Label
	Private WithEvents Label4 As System.Windows.Forms.Label
	Private WithEvents Label5 As System.Windows.Forms.Label
	Private WithEvents txtIDImp As System.Windows.Forms.TextBox
	Private WithEvents TextBox1 As System.Windows.Forms.TextBox
	Private WithEvents txtID As System.Windows.Forms.TextBox
	Private WithEvents txtBlock As System.Windows.Forms.TextBox
	Private WithEvents txtBlockAdd As System.Windows.Forms.TextBox
	Private WithEvents txtParcel As System.Windows.Forms.TextBox
End Class
