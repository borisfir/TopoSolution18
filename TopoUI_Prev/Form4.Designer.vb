<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form4
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
      Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
      Me.Button1 = New System.Windows.Forms.Button()
      Me.Button2 = New System.Windows.Forms.Button()
      Me.txtLtoR = New System.Windows.Forms.TextBox()
      Me.txtRtoL = New System.Windows.Forms.TextBox()
      Me.Button3 = New System.Windows.Forms.Button()
      Me.txtMline = New System.Windows.Forms.TextBox()
      Me.Button4 = New System.Windows.Forms.Button()
      Me.Button5 = New System.Windows.Forms.Button()
      Me.Button6 = New System.Windows.Forms.Button()
      Me.dgvMain = New System.Windows.Forms.DataGridView()
      Me.ctxStage = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxAction = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxFromParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxFromParcelTemp = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxToParcel = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxToGush = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxLegalArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxForcedArea = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxTolerance = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxDiff = New System.Windows.Forms.DataGridViewTextBoxColumn()
      Me.ctxDeviation = New System.Windows.Forms.DataGridViewTextBoxColumn()
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).BeginInit()
      Me.SuspendLayout()
      '
      'Button1
      '
      Me.Button1.Location = New System.Drawing.Point(30, 393)
      Me.Button1.Name = "Button1"
      Me.Button1.Size = New System.Drawing.Size(75, 23)
      Me.Button1.TabIndex = 0
      Me.Button1.Text = "Button1"
      Me.Button1.UseVisualStyleBackColor = True
      '
      'Button2
      '
      Me.Button2.Location = New System.Drawing.Point(30, 422)
      Me.Button2.Name = "Button2"
      Me.Button2.Size = New System.Drawing.Size(75, 23)
      Me.Button2.TabIndex = 1
      Me.Button2.Text = "Button2"
      Me.Button2.UseVisualStyleBackColor = True
      '
      'txtLtoR
      '
      Me.txtLtoR.Location = New System.Drawing.Point(226, 396)
      Me.txtLtoR.Name = "txtLtoR"
      Me.txtLtoR.Size = New System.Drawing.Size(211, 20)
      Me.txtLtoR.TabIndex = 2
      '
      'txtRtoL
      '
      Me.txtRtoL.Location = New System.Drawing.Point(226, 422)
      Me.txtRtoL.Name = "txtRtoL"
      Me.txtRtoL.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtRtoL.Size = New System.Drawing.Size(211, 20)
      Me.txtRtoL.TabIndex = 3
      '
      'Button3
      '
      Me.Button3.Location = New System.Drawing.Point(453, 420)
      Me.Button3.Name = "Button3"
      Me.Button3.Size = New System.Drawing.Size(36, 23)
      Me.Button3.TabIndex = 4
      Me.Button3.Text = "Button3"
      Me.Button3.UseVisualStyleBackColor = True
      '
      'txtMline
      '
      Me.txtMline.Location = New System.Drawing.Point(180, 397)
      Me.txtMline.Multiline = True
      Me.txtMline.Name = "txtMline"
      Me.txtMline.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.txtMline.Size = New System.Drawing.Size(40, 46)
      Me.txtMline.TabIndex = 5
      '
      'Button4
      '
      Me.Button4.Location = New System.Drawing.Point(30, 451)
      Me.Button4.Name = "Button4"
      Me.Button4.Size = New System.Drawing.Size(75, 23)
      Me.Button4.TabIndex = 6
      Me.Button4.Text = "Button4"
      Me.Button4.UseVisualStyleBackColor = True
      '
      'Button5
      '
      Me.Button5.Location = New System.Drawing.Point(30, 480)
      Me.Button5.Name = "Button5"
      Me.Button5.Size = New System.Drawing.Size(75, 23)
      Me.Button5.TabIndex = 7
      Me.Button5.Text = "Button5"
      Me.Button5.UseVisualStyleBackColor = True
      '
      'Button6
      '
      Me.Button6.Location = New System.Drawing.Point(30, 509)
      Me.Button6.Name = "Button6"
      Me.Button6.Size = New System.Drawing.Size(75, 23)
      Me.Button6.TabIndex = 8
      Me.Button6.Text = "Button6"
      Me.Button6.UseVisualStyleBackColor = True
      '
      'dgvMain
      '
      Me.dgvMain.AllowUserToAddRows = False
      Me.dgvMain.AllowUserToDeleteRows = False
      Me.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
      Me.dgvMain.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ctxStage, Me.ctxAction, Me.ctxFromParcel, Me.ctxFromParcelTemp, Me.ctxToParcel, Me.ctxToGush, Me.ctxLegalArea, Me.ctxArea, Me.ctxForcedArea, Me.ctxTolerance, Me.ctxDiff, Me.ctxDeviation})
      Me.dgvMain.Location = New System.Drawing.Point(1, 12)
      Me.dgvMain.Name = "dgvMain"
      Me.dgvMain.RowHeadersWidth = 24
      Me.dgvMain.Size = New System.Drawing.Size(678, 361)
      Me.dgvMain.TabIndex = 9
      '
      'ctxStage
      '
      Me.ctxStage.HeaderText = "שלב"
      Me.ctxStage.Name = "ctxStage"
      Me.ctxStage.Width = 32
      '
      'ctxAction
      '
      Me.ctxAction.HeaderText = "פעולה"
      Me.ctxAction.Name = "ctxAction"
      Me.ctxAction.Width = 56
      '
      'ctxFromParcel
      '
      DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
      DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
      Me.ctxFromParcel.DefaultCellStyle = DataGridViewCellStyle1
      Me.ctxFromParcel.HeaderText = "מחלקה רשומה"
      Me.ctxFromParcel.Name = "ctxFromParcel"
      Me.ctxFromParcel.Width = 56
      '
      'ctxFromParcelTemp
      '
      Me.ctxFromParcelTemp.HeaderText = "מחלקה ארעית"
      Me.ctxFromParcelTemp.Name = "ctxFromParcelTemp"
      Me.ctxFromParcelTemp.Width = 56
      '
      'ctxToParcel
      '
      Me.ctxToParcel.HeaderText = "לחלקה"
      Me.ctxToParcel.Name = "ctxToParcel"
      Me.ctxToParcel.Width = 48
      '
      'ctxToGush
      '
      Me.ctxToGush.HeaderText = "לגוש"
      Me.ctxToGush.Name = "ctxToGush"
      Me.ctxToGush.Width = 48
      '
      'ctxLegalArea
      '
      Me.ctxLegalArea.HeaderText = "שטח רשום"
      Me.ctxLegalArea.Name = "ctxLegalArea"
      Me.ctxLegalArea.Width = 56
      '
      'ctxArea
      '
      Me.ctxArea.HeaderText = "שטח"
      Me.ctxArea.Name = "ctxArea"
      Me.ctxArea.Width = 56
      '
      'ctxForcedArea
      '
      Me.ctxForcedArea.HeaderText = "שטח כפוי"
      Me.ctxForcedArea.Name = "ctxForcedArea"
      Me.ctxForcedArea.Width = 60
      '
      'ctxTolerance
      '
      Me.ctxTolerance.HeaderText = "טולרנס"
      Me.ctxTolerance.Name = "ctxTolerance"
      Me.ctxTolerance.Width = 56
      '
      'ctxDiff
      '
      Me.ctxDiff.HeaderText = "רשום - מחושב"
      Me.ctxDiff.Name = "ctxDiff"
      Me.ctxDiff.Width = 56
      '
      'ctxDeviation
      '
      Me.ctxDeviation.HeaderText = "סטיה מטולרנס"
      Me.ctxDeviation.Name = "ctxDeviation"
      Me.ctxDeviation.Width = 56
      '
      'Form4
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      Me.ClientSize = New System.Drawing.Size(679, 485)
      Me.Controls.Add(Me.dgvMain)
      Me.Controls.Add(Me.Button6)
      Me.Controls.Add(Me.Button5)
      Me.Controls.Add(Me.Button4)
      Me.Controls.Add(Me.txtMline)
      Me.Controls.Add(Me.Button3)
      Me.Controls.Add(Me.txtRtoL)
      Me.Controls.Add(Me.txtLtoR)
      Me.Controls.Add(Me.Button2)
      Me.Controls.Add(Me.Button1)
      Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
      Me.Name = "Form4"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.RightToLeftLayout = True
      Me.Text = "איחוד וחלוקה"
      CType(Me.dgvMain, System.ComponentModel.ISupportInitialize).EndInit()
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Friend WithEvents Button1 As System.Windows.Forms.Button
   Friend WithEvents Button2 As System.Windows.Forms.Button
   Private WithEvents txtLtoR As System.Windows.Forms.TextBox
   Private WithEvents txtRtoL As System.Windows.Forms.TextBox
   Friend WithEvents Button3 As System.Windows.Forms.Button
   Private WithEvents txtMline As System.Windows.Forms.TextBox
   Friend WithEvents Button4 As System.Windows.Forms.Button
   Friend WithEvents Button5 As System.Windows.Forms.Button
   Friend WithEvents Button6 As System.Windows.Forms.Button
   Private WithEvents dgvMain As System.Windows.Forms.DataGridView
   Friend WithEvents ctxStage As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxAction As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxFromParcel As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxFromParcelTemp As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxToParcel As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxToGush As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxLegalArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxForcedArea As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxTolerance As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxDiff As System.Windows.Forms.DataGridViewTextBoxColumn
   Friend WithEvents ctxDeviation As System.Windows.Forms.DataGridViewTextBoxColumn
End Class
