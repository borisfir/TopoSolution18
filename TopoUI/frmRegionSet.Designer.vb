<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRegionSet
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmRegionSet))
		Me.clbRegions = New System.Windows.Forms.CheckedListBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtRecordSetName = New System.Windows.Forms.TextBox()
		Me.lstRegionSets = New System.Windows.Forms.ListBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.cmdExit = New System.Windows.Forms.Button()
		Me.imlButtonImages = New System.Windows.Forms.ImageList(Me.components)
		Me.cmdDelete = New System.Windows.Forms.Button()
		Me.cmdSave = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'clbRegions
		'
		Me.clbRegions.FormattingEnabled = True
		Me.clbRegions.Location = New System.Drawing.Point(11, 178)
		Me.clbRegions.Name = "clbRegions"
		Me.clbRegions.Size = New System.Drawing.Size(180, 132)
		Me.clbRegions.TabIndex = 0
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(152, 130)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(52, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "שם חדש"
		'
		'txtRecordSetName
		'
		Me.txtRecordSetName.Location = New System.Drawing.Point(12, 152)
		Me.txtRecordSetName.Name = "txtRecordSetName"
		Me.txtRecordSetName.Size = New System.Drawing.Size(179, 21)
		Me.txtRecordSetName.TabIndex = 2
		'
		'lstRegionSets
		'
		Me.lstRegionSets.FormattingEnabled = True
		Me.lstRegionSets.Location = New System.Drawing.Point(12, 29)
		Me.lstRegionSets.Name = "lstRegionSets"
		Me.lstRegionSets.Size = New System.Drawing.Size(180, 95)
		Me.lstRegionSets.TabIndex = 3
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(12, 5)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(98, 13)
		Me.Label2.TabIndex = 4
		Me.Label2.Text = "מתחמים מורכבים"
		'
		'cmdExit
		'
		Me.cmdExit.Image = Global.TopoUI.My.Resources.Resources._Exit
		Me.cmdExit.Location = New System.Drawing.Point(170, 5)
		Me.cmdExit.Name = "cmdExit"
		Me.cmdExit.Size = New System.Drawing.Size(21, 21)
		Me.cmdExit.TabIndex = 5
		Me.cmdExit.UseVisualStyleBackColor = True
		'
		'imlButtonImages
		'
		Me.imlButtonImages.ImageStream = CType(resources.GetObject("imlButtonImages.ImageStream"), System.Windows.Forms.ImageListStreamer)
		Me.imlButtonImages.TransparentColor = System.Drawing.Color.Transparent
		Me.imlButtonImages.Images.SetKeyName(0, "Delete")
		Me.imlButtonImages.Images.SetKeyName(1, "Edit")
		Me.imlButtonImages.Images.SetKeyName(2, "New")
		Me.imlButtonImages.Images.SetKeyName(3, "Save")
		'
		'cmdDelete
		'
		Me.cmdDelete.ImageKey = "Delete"
		Me.cmdDelete.ImageList = Me.imlButtonImages
		Me.cmdDelete.Location = New System.Drawing.Point(116, 5)
		Me.cmdDelete.Name = "cmdDelete"
		Me.cmdDelete.Size = New System.Drawing.Size(21, 21)
		Me.cmdDelete.TabIndex = 6
		Me.cmdDelete.UseVisualStyleBackColor = True
		'
		'cmdSave
		'
		Me.cmdSave.ImageKey = "Save"
		Me.cmdSave.ImageList = Me.imlButtonImages
		Me.cmdSave.Location = New System.Drawing.Point(143, 5)
		Me.cmdSave.Name = "cmdSave"
		Me.cmdSave.Size = New System.Drawing.Size(21, 21)
		Me.cmdSave.TabIndex = 7
		Me.cmdSave.UseVisualStyleBackColor = True
		'
		'frmRegionSet
		'
		Me.AcceptButton = Me.cmdSave
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(203, 329)
		Me.Controls.Add(Me.cmdSave)
		Me.Controls.Add(Me.cmdDelete)
		Me.Controls.Add(Me.cmdExit)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.lstRegionSets)
		Me.Controls.Add(Me.txtRecordSetName)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.clbRegions)
		Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.Name = "frmRegionSet"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.RightToLeftLayout = True
		Me.Text = "מתחמים"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents clbRegions As CheckedListBox
	Private WithEvents lstRegionSets As ListBox
	Friend WithEvents Label2 As Label
	Friend WithEvents cmdDelete As Button
	Private WithEvents cmdSave As Button
	Private WithEvents imlButtonImages As ImageList
	Private WithEvents txtRecordSetName As TextBox
	Private WithEvents Label1 As Label
	Private WithEvents cmdExit As Button
End Class
