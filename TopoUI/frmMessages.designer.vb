<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMessages
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
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMessages))
		Me.imlMessages = New System.Windows.Forms.ImageList(Me.components)
		Me.cmdShowMsg = New System.Windows.Forms.Button()
		Me.cmbMapThemes = New System.Windows.Forms.ComboBox()
		Me.cmbActions = New System.Windows.Forms.ComboBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.cmdClear = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'imlMessages
		'
		Me.imlMessages.ImageStream = CType(resources.GetObject("imlMessages.ImageStream"), System.Windows.Forms.ImageListStreamer)
		Me.imlMessages.TransparentColor = System.Drawing.Color.White
		Me.imlMessages.Images.SetKeyName(0, "ArrowUp16.png")
		Me.imlMessages.Images.SetKeyName(1, "ArrowDown16.png")
		Me.imlMessages.Images.SetKeyName(2, "2392_ZoomIn_16x16.png")
		Me.imlMessages.Images.SetKeyName(3, "Eraser15Tr.png")
		Me.imlMessages.Images.SetKeyName(4, "Exit24Tr.png")
		'
		'cmdShowMsg
		'
		Me.cmdShowMsg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.cmdShowMsg.Image = Global.TopoUI.My.Resources.Resources.BrushTr
		Me.cmdShowMsg.Location = New System.Drawing.Point(651, 3)
		Me.cmdShowMsg.Name = "cmdShowMsg"
		Me.cmdShowMsg.Size = New System.Drawing.Size(32, 32)
		Me.cmdShowMsg.TabIndex = 0
		Me.cmdShowMsg.UseVisualStyleBackColor = True
		'
		'cmbMapThemes
		'
		Me.cmbMapThemes.FormattingEnabled = True
		Me.cmbMapThemes.Location = New System.Drawing.Point(438, 0)
		Me.cmbMapThemes.Name = "cmbMapThemes"
		Me.cmbMapThemes.Size = New System.Drawing.Size(200, 21)
		Me.cmbMapThemes.TabIndex = 1
		'
		'cmbActions
		'
		Me.cmbActions.FormattingEnabled = True
		Me.cmbActions.Location = New System.Drawing.Point(438, 24)
		Me.cmbActions.Name = "cmbActions"
		Me.cmbActions.Size = New System.Drawing.Size(200, 21)
		Me.cmbActions.TabIndex = 2
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(376, 3)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(31, 13)
		Me.Label1.TabIndex = 3
		Me.Label1.Text = "נושא"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(376, 27)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(61, 13)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "שם בדיקה"
		'
		'cmdClear
		'
		Me.cmdClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
		Me.cmdClear.Image = Global.TopoUI.My.Resources.Resources.Eraser15Tr
		Me.cmdClear.Location = New System.Drawing.Point(689, 3)
		Me.cmdClear.Name = "cmdClear"
		Me.cmdClear.Size = New System.Drawing.Size(32, 32)
		Me.cmdClear.TabIndex = 5
		Me.cmdClear.UseVisualStyleBackColor = True
		'
		'frmMessages
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(788, 669)
		Me.Controls.Add(Me.cmdClear)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.cmbActions)
		Me.Controls.Add(Me.cmbMapThemes)
		Me.Controls.Add(Me.cmdShowMsg)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
		Me.Margin = New System.Windows.Forms.Padding(48, 22, 48, 22)
		Me.Name = "frmMessages"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds
		Me.Text = "הודעות"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents imlMessages As System.Windows.Forms.ImageList
	Private WithEvents Label1 As Label
	Private WithEvents Label2 As Label
	Private WithEvents cmbMapThemes As ComboBox
	Private WithEvents cmbActions As ComboBox
	Private WithEvents cmdShowMsg As Button
	Private WithEvents cmdClear As Button
End Class
