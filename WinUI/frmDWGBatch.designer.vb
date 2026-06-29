<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDWGBatch
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
		Me.lvwDWGs = New System.Windows.Forms.ListView
		Me.DWGName = New System.Windows.Forms.ColumnHeader
		Me.OpenDWG = New System.Windows.Forms.ColumnHeader
		Me.Topology = New System.Windows.Forms.ColumnHeader
		Me.Shape = New System.Windows.Forms.ColumnHeader
		Me.CloseDWG = New System.Windows.Forms.ColumnHeader
		Me.cmdAddFiles = New System.Windows.Forms.Button
		Me.cmdAddDirectory = New System.Windows.Forms.Button
		Me.ofdDWG = New System.Windows.Forms.OpenFileDialog
		Me.fbdDWGs = New System.Windows.Forms.FolderBrowserDialog
		Me.cmdExec1 = New System.Windows.Forms.Button
		Me.SuspendLayout()
		'
		'lvwDWGs
		'
		Me.lvwDWGs.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.DWGName, Me.OpenDWG, Me.Topology, Me.Shape, Me.CloseDWG})
		Me.lvwDWGs.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.lvwDWGs.GridLines = True
		Me.lvwDWGs.Location = New System.Drawing.Point(0, 39)
		Me.lvwDWGs.Name = "lvwDWGs"
		Me.lvwDWGs.Size = New System.Drawing.Size(945, 225)
		Me.lvwDWGs.TabIndex = 0
		Me.lvwDWGs.UseCompatibleStateImageBehavior = False
		Me.lvwDWGs.View = System.Windows.Forms.View.Details
		'
		'DWGName
		'
		Me.DWGName.Text = "DWG Name"
		Me.DWGName.Width = 419
		'
		'OpenDWG
		'
		Me.OpenDWG.Text = "Open DWG"
		Me.OpenDWG.Width = 79
		'
		'Topology
		'
		Me.Topology.Text = "Topology"
		Me.Topology.Width = 85
		'
		'Shape
		'
		Me.Shape.Text = "Shape"
		Me.Shape.Width = 91
		'
		'CloseDWG
		'
		Me.CloseDWG.Text = "Close DWG"
		Me.CloseDWG.Width = 74
		'
		'cmdAddFiles
		'
		Me.cmdAddFiles.Location = New System.Drawing.Point(16, 8)
		Me.cmdAddFiles.Name = "cmdAddFiles"
		Me.cmdAddFiles.Size = New System.Drawing.Size(72, 22)
		Me.cmdAddFiles.TabIndex = 1
		Me.cmdAddFiles.Text = "Add Files"
		Me.cmdAddFiles.UseVisualStyleBackColor = True
		'
		'cmdAddDirectory
		'
		Me.cmdAddDirectory.Location = New System.Drawing.Point(115, 8)
		Me.cmdAddDirectory.Name = "cmdAddDirectory"
		Me.cmdAddDirectory.Size = New System.Drawing.Size(92, 22)
		Me.cmdAddDirectory.TabIndex = 2
		Me.cmdAddDirectory.Text = "Add Directory"
		Me.cmdAddDirectory.UseVisualStyleBackColor = True
		'
		'ofdDWG
		'
		Me.ofdDWG.DefaultExt = "DWG"
		Me.ofdDWG.Filter = "Autocad Drawings (*.dwg)|*.dwg|All Files (*.*)|*.*"
		Me.ofdDWG.Multiselect = True
		'
		'fbdDWGs
		'
		Me.fbdDWGs.Description = "wwwwwwwwwwwwwwwwwww"
		Me.fbdDWGs.RootFolder = System.Environment.SpecialFolder.DesktopDirectory
		'
		'cmdExec1
		'
		Me.cmdExec1.Location = New System.Drawing.Point(355, 8)
		Me.cmdExec1.Name = "cmdExec1"
		Me.cmdExec1.Size = New System.Drawing.Size(92, 22)
		Me.cmdExec1.TabIndex = 3
		Me.cmdExec1.Text = "Execute"
		Me.cmdExec1.UseVisualStyleBackColor = True
		'
		'frmDWGBatch
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(945, 264)
		Me.Controls.Add(Me.cmdExec1)
		Me.Controls.Add(Me.cmdAddDirectory)
		Me.Controls.Add(Me.cmdAddFiles)
		Me.Controls.Add(Me.lvwDWGs)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!)
		Me.Name = "frmDWGBatch"
		Me.Text = "frmDWGBatch"
		Me.ResumeLayout(False)

	End Sub
	Private WithEvents lvwDWGs As System.Windows.Forms.ListView

	Private WithEvents cmdAddFiles As System.Windows.Forms.Button
	Private WithEvents cmdAddDirectory As System.Windows.Forms.Button
	Private WithEvents ofdDWG As System.Windows.Forms.OpenFileDialog
	Private WithEvents fbdDWGs As System.Windows.Forms.FolderBrowserDialog
	Private WithEvents cmdExec1 As System.Windows.Forms.Button
	Private WithEvents DWGName As System.Windows.Forms.ColumnHeader
	Private WithEvents OpenDWG As System.Windows.Forms.ColumnHeader
	Private WithEvents Topology As System.Windows.Forms.ColumnHeader
	Private WithEvents Shape As System.Windows.Forms.ColumnHeader
	Private WithEvents CloseDWG As System.Windows.Forms.ColumnHeader
End Class
