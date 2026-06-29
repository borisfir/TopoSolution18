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
		Me.PolygonNum = New System.Windows.Forms.ColumnHeader
		Me.CloseDWG = New System.Windows.Forms.ColumnHeader
		Me.cmdAddFiles = New System.Windows.Forms.Button
		Me.cmdAddDirectory = New System.Windows.Forms.Button
		Me.ofdDWG = New System.Windows.Forms.OpenFileDialog
		Me.fbdDWGs = New System.Windows.Forms.FolderBrowserDialog
		Me.cmdExec = New System.Windows.Forms.Button
		Me.cmdInputParcelLines = New System.Windows.Forms.Button
		Me.cmdInputParcelLCents = New System.Windows.Forms.Button
		Me.cmdMacroN = New System.Windows.Forms.Button
		Me.SuspendLayout()
		'
		'lvwDWGs
		'
		Me.lvwDWGs.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.DWGName, Me.OpenDWG, Me.Topology, Me.Shape, Me.PolygonNum, Me.CloseDWG})
		Me.lvwDWGs.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.lvwDWGs.GridLines = True
		Me.lvwDWGs.Location = New System.Drawing.Point(0, 38)
		Me.lvwDWGs.Name = "lvwDWGs"
		Me.lvwDWGs.Size = New System.Drawing.Size(945, 531)
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
		'PolygonNum
		'
		Me.PolygonNum.Text = "Count"
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
		'cmdExec
		'
		Me.cmdExec.Location = New System.Drawing.Point(239, 8)
		Me.cmdExec.Name = "cmdExec"
		Me.cmdExec.Size = New System.Drawing.Size(63, 22)
		Me.cmdExec.TabIndex = 3
		Me.cmdExec.Text = "Execute"
		Me.cmdExec.UseVisualStyleBackColor = True
		'
		'cmdInputParcelLines
		'
		Me.cmdInputParcelLines.Location = New System.Drawing.Point(322, 2)
		Me.cmdInputParcelLines.Name = "cmdInputParcelLines"
		Me.cmdInputParcelLines.Size = New System.Drawing.Size(63, 34)
		Me.cmdInputParcelLines.TabIndex = 4
		Me.cmdInputParcelLines.Text = "Input Parcels(L)"
		Me.cmdInputParcelLines.UseVisualStyleBackColor = True
		'
		'cmdInputParcelLCents
		'
		Me.cmdInputParcelLCents.Location = New System.Drawing.Point(391, 2)
		Me.cmdInputParcelLCents.Name = "cmdInputParcelLCents"
		Me.cmdInputParcelLCents.Size = New System.Drawing.Size(67, 34)
		Me.cmdInputParcelLCents.TabIndex = 5
		Me.cmdInputParcelLCents.Text = "Input Parcels(C)"
		Me.cmdInputParcelLCents.UseVisualStyleBackColor = True
		'
		'cmdMacroN
		'
		Me.cmdMacroN.Location = New System.Drawing.Point(582, 8)
		Me.cmdMacroN.Name = "cmdMacroN"
		Me.cmdMacroN.Size = New System.Drawing.Size(72, 22)
		Me.cmdMacroN.TabIndex = 6
		Me.cmdMacroN.Text = "Add Files"
		Me.cmdMacroN.UseVisualStyleBackColor = True
		'
		'frmDWGBatch
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(945, 569)
		Me.Controls.Add(Me.cmdMacroN)
		Me.Controls.Add(Me.cmdInputParcelLCents)
		Me.Controls.Add(Me.cmdInputParcelLines)
		Me.Controls.Add(Me.cmdExec)
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
	Private WithEvents cmdExec As System.Windows.Forms.Button
	Private WithEvents DWGName As System.Windows.Forms.ColumnHeader
	Private WithEvents OpenDWG As System.Windows.Forms.ColumnHeader
	Private WithEvents Topology As System.Windows.Forms.ColumnHeader
	Private WithEvents Shape As System.Windows.Forms.ColumnHeader
	Private WithEvents CloseDWG As System.Windows.Forms.ColumnHeader
	Private WithEvents PolygonNum As System.Windows.Forms.ColumnHeader
	Private WithEvents cmdInputParcelLines As System.Windows.Forms.Button

	Private Sub cmdExec_BackColorChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExec.BackColorChanged

	End Sub
	Private WithEvents cmdInputParcelLCents As System.Windows.Forms.Button
	Private WithEvents cmdMacroN As System.Windows.Forms.Button
End Class
