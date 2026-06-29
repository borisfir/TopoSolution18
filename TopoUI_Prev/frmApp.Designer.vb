<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmApp
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
		Me.cmdSelectFolders = New System.Windows.Forms.Button()
		Me.opdParcels = New System.Windows.Forms.OpenFileDialog()
		Me.clbParcels = New System.Windows.Forms.CheckedListBox()
		Me.txtName = New System.Windows.Forms.TextBox()
		Me.cmdOK = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.tmrDelay = New System.Windows.Forms.Timer(Me.components)
		Me.rdbFolder = New System.Windows.Forms.RadioButton()
		Me.rdbBlockFile = New System.Windows.Forms.RadioButton()
		Me.rdbParcelFile = New System.Windows.Forms.RadioButton()
		Me.txtFileList = New System.Windows.Forms.TextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.cmdClear = New System.Windows.Forms.Button()
		Me.cmdInsertBlocks = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'cmdSelectFolders
		'
		Me.cmdSelectFolders.Location = New System.Drawing.Point(436, 12)
		Me.cmdSelectFolders.Name = "cmdSelectFolders"
		Me.cmdSelectFolders.Size = New System.Drawing.Size(30, 24)
		Me.cmdSelectFolders.TabIndex = 0
		Me.cmdSelectFolders.Text = "..."
		Me.cmdSelectFolders.UseVisualStyleBackColor = True
		'
		'opdParcels
		'
		Me.opdParcels.DefaultExt = "shp"
		Me.opdParcels.Filter = "Shape Files(*.shp)|*.shp|All Files (*.*)|*.*"
		'
		'clbParcels
		'
		Me.clbParcels.CheckOnClick = True
		Me.clbParcels.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.clbParcels.FormattingEnabled = True
		Me.clbParcels.Location = New System.Drawing.Point(0, 90)
		Me.clbParcels.MultiColumn = True
		Me.clbParcels.Name = "clbParcels"
		Me.clbParcels.Size = New System.Drawing.Size(904, 480)
		Me.clbParcels.TabIndex = 2
		Me.clbParcels.ThreeDCheckBoxes = True
		'
		'txtName
		'
		Me.txtName.Location = New System.Drawing.Point(12, 12)
		Me.txtName.Name = "txtName"
		Me.txtName.Size = New System.Drawing.Size(420, 22)
		Me.txtName.TabIndex = 3
		'
		'cmdOK
		'
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
		'
		'txtFileList
		'
		Me.txtFileList.Location = New System.Drawing.Point(12, 50)
		Me.txtFileList.Name = "txtFileList"
		Me.txtFileList.ReadOnly = True
		Me.txtFileList.Size = New System.Drawing.Size(600, 22)
		Me.txtFileList.TabIndex = 9
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(618, 50)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(79, 14)
		Me.Label1.TabIndex = 10
		Me.Label1.Text = "רשימת גושים"
		'
		'cmdClear
		'
		Me.cmdClear.Location = New System.Drawing.Point(580, 10)
		Me.cmdClear.Name = "cmdClear"
		Me.cmdClear.Size = New System.Drawing.Size(52, 28)
		Me.cmdClear.TabIndex = 11
		Me.cmdClear.Text = "Clear"
		Me.cmdClear.UseVisualStyleBackColor = True
		'
		'cmdInsertBlocks
		'
		Me.cmdInsertBlocks.Location = New System.Drawing.Point(826, 48)
		Me.cmdInsertBlocks.Name = "cmdInsertBlocks"
		Me.cmdInsertBlocks.Size = New System.Drawing.Size(78, 28)
		Me.cmdInsertBlocks.TabIndex = 12
		Me.cmdInsertBlocks.Text = "Ins Blocks"
		Me.cmdInsertBlocks.UseVisualStyleBackColor = True
		'
		'frmApp
		'
		Me.AcceptButton = Me.cmdOK
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.CancelButton = Me.cmdCancel
		Me.ClientSize = New System.Drawing.Size(904, 570)
		Me.Controls.Add(Me.cmdInsertBlocks)
		Me.Controls.Add(Me.cmdClear)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.txtFileList)
		Me.Controls.Add(Me.rdbParcelFile)
		Me.Controls.Add(Me.rdbBlockFile)
		Me.Controls.Add(Me.rdbFolder)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdOK)
		Me.Controls.Add(Me.txtName)
		Me.Controls.Add(Me.clbParcels)
		Me.Controls.Add(Me.cmdSelectFolders)
		Me.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Margin = New System.Windows.Forms.Padding(4, 4, 4, 4)
		Me.Name = "frmApp"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "גושים"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents fbdParcels As System.Windows.Forms.FolderBrowserDialog
	Private WithEvents cmdSelectFolders As System.Windows.Forms.Button
	Private WithEvents opdParcels As System.Windows.Forms.OpenFileDialog
	Private WithEvents clbParcels As System.Windows.Forms.CheckedListBox
	Private WithEvents txtName As System.Windows.Forms.TextBox
	Private WithEvents cmdOK As System.Windows.Forms.Button
	Private WithEvents cmdCancel As System.Windows.Forms.Button
	Private WithEvents tmrDelay As System.Windows.Forms.Timer
	Private WithEvents rdbFolder As System.Windows.Forms.RadioButton
	Private WithEvents rdbBlockFile As System.Windows.Forms.RadioButton
	Private WithEvents rdbParcelFile As System.Windows.Forms.RadioButton
	Private WithEvents txtFileList As System.Windows.Forms.TextBox
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents cmdClear As System.Windows.Forms.Button
	Private WithEvents cmdInsertBlocks As System.Windows.Forms.Button
End Class
