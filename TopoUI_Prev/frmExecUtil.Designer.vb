<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExecUtil
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
      Me.Button1 = New System.Windows.Forms.Button()
      Me.Button2 = New System.Windows.Forms.Button()
      Me.lblCurrentIndex = New System.Windows.Forms.Label()
      Me.tmrMain = New System.Windows.Forms.Timer(Me.components)
      Me.cmdStart = New System.Windows.Forms.Button()
      Me.txtCurrentIndex = New System.Windows.Forms.TextBox()
      Me.cmdInputIndex = New System.Windows.Forms.Button()
      Me.txtMaxIndex = New System.Windows.Forms.TextBox()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.cmdInsertCentroids = New System.Windows.Forms.Button()
      Me.txtOutputLayer = New System.Windows.Forms.TextBox()
      Me.cmdImport = New System.Windows.Forms.Button()
      Me.SuspendLayout()
      '
      'Button1
      '
      Me.Button1.Location = New System.Drawing.Point(120, 28)
      Me.Button1.Name = "Button1"
      Me.Button1.Size = New System.Drawing.Size(75, 23)
      Me.Button1.TabIndex = 0
      Me.Button1.Text = "Step 1"
      Me.Button1.UseVisualStyleBackColor = True
      '
      'Button2
      '
      Me.Button2.Location = New System.Drawing.Point(120, 52)
      Me.Button2.Name = "Button2"
      Me.Button2.Size = New System.Drawing.Size(75, 23)
      Me.Button2.TabIndex = 1
      Me.Button2.Text = "Step 2"
      Me.Button2.UseVisualStyleBackColor = True
      '
      'lblCurrentIndex
      '
      Me.lblCurrentIndex.AutoSize = True
      Me.lblCurrentIndex.Location = New System.Drawing.Point(12, 62)
      Me.lblCurrentIndex.Name = "lblCurrentIndex"
      Me.lblCurrentIndex.Size = New System.Drawing.Size(36, 13)
      Me.lblCurrentIndex.TabIndex = 2
      Me.lblCurrentIndex.Text = "Index:"
      '
      'tmrMain
      '
      Me.tmrMain.Interval = 600
      '
      'cmdStart
      '
      Me.cmdStart.Location = New System.Drawing.Point(12, 12)
      Me.cmdStart.Name = "cmdStart"
      Me.cmdStart.Size = New System.Drawing.Size(75, 23)
      Me.cmdStart.TabIndex = 3
      Me.cmdStart.Text = "Start"
      Me.cmdStart.UseVisualStyleBackColor = True
      '
      'txtCurrentIndex
      '
      Me.txtCurrentIndex.Location = New System.Drawing.Point(66, 55)
      Me.txtCurrentIndex.Name = "txtCurrentIndex"
      Me.txtCurrentIndex.Size = New System.Drawing.Size(33, 20)
      Me.txtCurrentIndex.TabIndex = 4
      '
      'cmdInputIndex
      '
      Me.cmdInputIndex.Location = New System.Drawing.Point(120, -1)
      Me.cmdInputIndex.Name = "cmdInputIndex"
      Me.cmdInputIndex.Size = New System.Drawing.Size(38, 23)
      Me.cmdInputIndex.TabIndex = 5
      Me.cmdInputIndex.Text = "..."
      Me.cmdInputIndex.UseVisualStyleBackColor = True
      '
      'txtMaxIndex
      '
      Me.txtMaxIndex.Location = New System.Drawing.Point(66, 81)
      Me.txtMaxIndex.Name = "txtMaxIndex"
      Me.txtMaxIndex.Size = New System.Drawing.Size(33, 20)
      Me.txtMaxIndex.TabIndex = 7
      '
      'Label1
      '
      Me.Label1.AutoSize = True
      Me.Label1.Location = New System.Drawing.Point(12, 88)
      Me.Label1.Name = "Label1"
      Me.Label1.Size = New System.Drawing.Size(30, 13)
      Me.Label1.TabIndex = 6
      Me.Label1.Text = "Max:"
      '
      'cmdInsertCentroids
      '
      Me.cmdInsertCentroids.Location = New System.Drawing.Point(120, 121)
      Me.cmdInsertCentroids.Name = "cmdInsertCentroids"
      Me.cmdInsertCentroids.Size = New System.Drawing.Size(97, 23)
      Me.cmdInsertCentroids.TabIndex = 8
      Me.cmdInsertCentroids.Text = "Insert Blocks"
      Me.cmdInsertCentroids.UseVisualStyleBackColor = True
      '
      'txtOutputLayer
      '
      Me.txtOutputLayer.Location = New System.Drawing.Point(12, 123)
      Me.txtOutputLayer.Name = "txtOutputLayer"
      Me.txtOutputLayer.Size = New System.Drawing.Size(87, 20)
      Me.txtOutputLayer.TabIndex = 9
      '
      'cmdImport
      '
      Me.cmdImport.Location = New System.Drawing.Point(120, 92)
      Me.cmdImport.Name = "cmdImport"
      Me.cmdImport.Size = New System.Drawing.Size(97, 23)
      Me.cmdImport.TabIndex = 10
      Me.cmdImport.Text = "Import"
      Me.cmdImport.UseVisualStyleBackColor = True
      '
      'frmExecUtil
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(309, 156)
      Me.Controls.Add(Me.cmdImport)
      Me.Controls.Add(Me.txtOutputLayer)
      Me.Controls.Add(Me.cmdInsertCentroids)
      Me.Controls.Add(Me.txtMaxIndex)
      Me.Controls.Add(Me.Label1)
      Me.Controls.Add(Me.cmdInputIndex)
      Me.Controls.Add(Me.txtCurrentIndex)
      Me.Controls.Add(Me.cmdStart)
      Me.Controls.Add(Me.lblCurrentIndex)
      Me.Controls.Add(Me.Button2)
      Me.Controls.Add(Me.Button1)
      Me.Name = "frmExecUtil"
      Me.Text = "Utilities"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents Button2 As System.Windows.Forms.Button
   Private WithEvents lblCurrentIndex As System.Windows.Forms.Label
   Private WithEvents tmrMain As System.Windows.Forms.Timer
   Private WithEvents Button1 As System.Windows.Forms.Button
   Private WithEvents cmdStart As System.Windows.Forms.Button
   Private WithEvents txtCurrentIndex As System.Windows.Forms.TextBox
   Private WithEvents cmdInputIndex As System.Windows.Forms.Button
   Private WithEvents txtMaxIndex As System.Windows.Forms.TextBox
   Private WithEvents Label1 As System.Windows.Forms.Label
   Private WithEvents cmdInsertCentroids As System.Windows.Forms.Button
   Private WithEvents txtOutputLayer As System.Windows.Forms.TextBox
   Private WithEvents cmdImport As System.Windows.Forms.Button
End Class
