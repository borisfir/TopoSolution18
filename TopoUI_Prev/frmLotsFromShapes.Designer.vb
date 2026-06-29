<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLotsFromShapes
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
      Me.fbdParcels = New System.Windows.Forms.FolderBrowserDialog()
      Me.cmdSelectFile = New System.Windows.Forms.Button()
      Me.opdLots = New System.Windows.Forms.OpenFileDialog()
      Me.txtName = New System.Windows.Forms.TextBox()
      Me.cmdOK = New System.Windows.Forms.Button()
      Me.cmdCancel = New System.Windows.Forms.Button()
      Me.tmrDelay = New System.Windows.Forms.Timer(Me.components)
      Me.rdbFolder = New System.Windows.Forms.RadioButton()
      Me.rdbBlockFile = New System.Windows.Forms.RadioButton()
      Me.rdbParcelFile = New System.Windows.Forms.RadioButton()
      Me.cmdClear = New System.Windows.Forms.Button()
      Me.SuspendLayout()
      '
      'cmdSelectFile
      '
      Me.cmdSelectFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
      Me.cmdSelectFile.Location = New System.Drawing.Point(436, 12)
      Me.cmdSelectFile.Name = "cmdSelectFile"
      Me.cmdSelectFile.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.cmdSelectFile.Size = New System.Drawing.Size(30, 24)
      Me.cmdSelectFile.TabIndex = 0
      Me.cmdSelectFile.Text = "..."
      Me.cmdSelectFile.UseVisualStyleBackColor = True
      '
      'opdLots
      '
      Me.opdLots.DefaultExt = "shp"
      Me.opdLots.Filter = "Shape Files(*.shp)|*.shp|All Files (*.*)|*.*"
      '
      'txtName
      '
      Me.txtName.Location = New System.Drawing.Point(12, 12)
      Me.txtName.Name = "txtName"
      Me.txtName.RightToLeft = System.Windows.Forms.RightToLeft.No
      Me.txtName.Size = New System.Drawing.Size(420, 22)
      Me.txtName.TabIndex = 3
      '
      'cmdOK
      '
      Me.cmdOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
      Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
      Me.cmdOK.Location = New System.Drawing.Point(520, 10)
      Me.cmdOK.Name = "cmdOK"
      Me.cmdOK.Size = New System.Drawing.Size(52, 28)
      Me.cmdOK.TabIndex = 4
      Me.cmdOK.Text = "Insert"
      Me.cmdOK.UseVisualStyleBackColor = True
      '
      'cmdCancel
      '
      Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
      Me.cmdCancel.Location = New System.Drawing.Point(640, 10)
      Me.cmdCancel.Name = "cmdCancel"
      Me.cmdCancel.Size = New System.Drawing.Size(52, 28)
      Me.cmdCancel.TabIndex = 5
      Me.cmdCancel.Text = "Close"
      Me.cmdCancel.UseVisualStyleBackColor = True
      '
      'tmrDelay
      '
      Me.tmrDelay.Interval = 5000
      '
      'rdbFolder
      '
      Me.rdbFolder.Checked = True
      Me.rdbFolder.Location = New System.Drawing.Point(720, 8)
      Me.rdbFolder.Name = "rdbFolder"
      Me.rdbFolder.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.rdbFolder.Size = New System.Drawing.Size(100, 22)
      Me.rdbFolder.TabIndex = 6
      Me.rdbFolder.TabStop = True
      Me.rdbFolder.Text = "מחיצה"
      Me.rdbFolder.UseVisualStyleBackColor = True
      Me.rdbFolder.Visible = False
      '
      'rdbBlockFile
      '
      Me.rdbBlockFile.Location = New System.Drawing.Point(720, 28)
      Me.rdbBlockFile.Name = "rdbBlockFile"
      Me.rdbBlockFile.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.rdbBlockFile.Size = New System.Drawing.Size(100, 22)
      Me.rdbBlockFile.TabIndex = 7
      Me.rdbBlockFile.Text = "קובץ גושים"
      Me.rdbBlockFile.UseVisualStyleBackColor = True
      Me.rdbBlockFile.Visible = False
      '
      'rdbParcelFile
      '
      Me.rdbParcelFile.Location = New System.Drawing.Point(720, 48)
      Me.rdbParcelFile.Name = "rdbParcelFile"
      Me.rdbParcelFile.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.rdbParcelFile.Size = New System.Drawing.Size(100, 22)
      Me.rdbParcelFile.TabIndex = 8
      Me.rdbParcelFile.Text = "קובץ חלקות"
      Me.rdbParcelFile.UseVisualStyleBackColor = True
      Me.rdbParcelFile.Visible = False
      '
      'cmdClear
      '
      Me.cmdClear.Location = New System.Drawing.Point(580, 10)
      Me.cmdClear.Name = "cmdClear"
      Me.cmdClear.Size = New System.Drawing.Size(52, 28)
      Me.cmdClear.TabIndex = 11
      Me.cmdClear.Text = "Clear"
      Me.cmdClear.UseVisualStyleBackColor = True
      Me.cmdClear.Visible = False
      '
      'frmLotsFromShapes
      '
      Me.AcceptButton = Me.cmdOK
      Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
      Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
      Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
      Me.CancelButton = Me.cmdCancel
      Me.ClientSize = New System.Drawing.Size(710, 64)
      Me.Controls.Add(Me.cmdClear)
      Me.Controls.Add(Me.rdbParcelFile)
      Me.Controls.Add(Me.rdbBlockFile)
      Me.Controls.Add(Me.rdbFolder)
      Me.Controls.Add(Me.cmdCancel)
      Me.Controls.Add(Me.cmdOK)
      Me.Controls.Add(Me.txtName)
      Me.Controls.Add(Me.cmdSelectFile)
      Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      Me.Margin = New System.Windows.Forms.Padding(4)
      Me.Name = "frmLotsFromShapes"
      Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
      Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
      Me.Text = "מגרשים"
      Me.ResumeLayout(False)
      Me.PerformLayout()

   End Sub
   Private WithEvents fbdParcels As System.Windows.Forms.FolderBrowserDialog
   Private WithEvents cmdSelectFile As System.Windows.Forms.Button
   Private WithEvents opdLots As System.Windows.Forms.OpenFileDialog
   Private WithEvents txtName As System.Windows.Forms.TextBox
   Private WithEvents cmdOK As System.Windows.Forms.Button
   Private WithEvents cmdCancel As System.Windows.Forms.Button
   Private WithEvents tmrDelay As System.Windows.Forms.Timer
   Private WithEvents rdbFolder As System.Windows.Forms.RadioButton
   Private WithEvents rdbBlockFile As System.Windows.Forms.RadioButton
   Private WithEvents rdbParcelFile As System.Windows.Forms.RadioButton
   Private WithEvents cmdClear As System.Windows.Forms.Button
End Class
