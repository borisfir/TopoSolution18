<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmThemeExplorer
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
		Me.lstThemes = New System.Windows.Forms.ListView()
		Me.ThemeName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.GeoType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.imlSmall = New System.Windows.Forms.ImageList(Me.components)
		Me.cmdAddTheme = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.txtProjectCode = New System.Windows.Forms.TextBox()
		Me.txtProjectName = New System.Windows.Forms.TextBox()
		Me.cmdEditTheme = New System.Windows.Forms.Button()
		Me.cmdDelete = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'lstThemes
		'
		Me.lstThemes.BackgroundImageTiled = True
		Me.lstThemes.CheckBoxes = True
		Me.lstThemes.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ThemeName, Me.GeoType, Me.ColumnHeader1})
		Me.lstThemes.Dock = System.Windows.Forms.DockStyle.Bottom
		Me.lstThemes.FullRowSelect = True
		Me.lstThemes.GridLines = True
		Me.lstThemes.Location = New System.Drawing.Point(0, -2212)
		Me.lstThemes.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.lstThemes.MultiSelect = False
		Me.lstThemes.Name = "lstThemes"
		Me.lstThemes.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.lstThemes.RightToLeftLayout = True
		Me.lstThemes.Size = New System.Drawing.Size(1924, 3274)
		Me.lstThemes.SmallImageList = Me.imlSmall
		Me.lstThemes.TabIndex = 0
		Me.lstThemes.UseCompatibleStateImageBehavior = False
		Me.lstThemes.View = System.Windows.Forms.View.Details
		'
		'ThemeName
		'
		Me.ThemeName.Text = "שם"
		Me.ThemeName.Width = 200
		'
		'GeoType
		'
		Me.GeoType.Text = "סוג פלגונים"
		Me.GeoType.Width = 100
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "בבב"
		'
		'imlSmall
		'
		Me.imlSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
		Me.imlSmall.ImageSize = New System.Drawing.Size(16, 16)
		Me.imlSmall.TransparentColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
		'
		'cmdAddTheme
		'
		Me.cmdAddTheme.Location = New System.Drawing.Point(165, 198)
		Me.cmdAddTheme.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.cmdAddTheme.Name = "cmdAddTheme"
		Me.cmdAddTheme.Size = New System.Drawing.Size(1152, 174)
		Me.cmdAddTheme.TabIndex = 1
		Me.cmdAddTheme.Text = "Add Theme"
		Me.cmdAddTheme.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Label1.Location = New System.Drawing.Point(7131, 24)
		Me.Label1.Margin = New System.Windows.Forms.Padding(41, 0, 41, 0)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(55, 16)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "פרויקט:"
		'
		'txtProjectCode
		'
		Me.txtProjectCode.Location = New System.Drawing.Point(6144, 24)
		Me.txtProjectCode.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.txtProjectCode.Name = "txtProjectCode"
		Me.txtProjectCode.Size = New System.Drawing.Size(882, 23)
		Me.txtProjectCode.TabIndex = 3
		Me.txtProjectCode.Text = "110337"
		'
		'txtProjectName
		'
		Me.txtProjectName.BackColor = System.Drawing.SystemColors.Control
		Me.txtProjectName.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.txtProjectName.Location = New System.Drawing.Point(55, 36)
		Me.txtProjectName.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.txtProjectName.Name = "txtProjectName"
		Me.txtProjectName.Size = New System.Drawing.Size(5979, 16)
		Me.txtProjectName.TabIndex = 4
		'
		'cmdEditTheme
		'
		Me.cmdEditTheme.Location = New System.Drawing.Point(1399, 198)
		Me.cmdEditTheme.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.cmdEditTheme.Name = "cmdEditTheme"
		Me.cmdEditTheme.Size = New System.Drawing.Size(1152, 174)
		Me.cmdEditTheme.TabIndex = 5
		Me.cmdEditTheme.Text = "Edit Theme"
		Me.cmdEditTheme.UseVisualStyleBackColor = True
		'
		'cmdDelete
		'
		Me.cmdDelete.Location = New System.Drawing.Point(2633, 198)
		Me.cmdDelete.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.cmdDelete.Name = "cmdDelete"
		Me.cmdDelete.Size = New System.Drawing.Size(1371, 174)
		Me.cmdDelete.TabIndex = 6
		Me.cmdDelete.Text = "Delete Theme"
		Me.cmdDelete.UseVisualStyleBackColor = True
		'
		'frmThemeExplorer
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
		Me.ClientSize = New System.Drawing.Size(1924, 1062)
		Me.Controls.Add(Me.cmdDelete)
		Me.Controls.Add(Me.cmdEditTheme)
		Me.Controls.Add(Me.txtProjectName)
		Me.Controls.Add(Me.txtProjectCode)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.cmdAddTheme)
		Me.Controls.Add(Me.lstThemes)
		Me.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
		Me.Margin = New System.Windows.Forms.Padding(41, 18, 41, 18)
		Me.Name = "frmThemeExplorer"
		Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
		Me.Text = "frmThemeExplorer"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents lstThemes As System.Windows.Forms.ListView
	Private WithEvents ThemeName As System.Windows.Forms.ColumnHeader
	Private WithEvents GeoType As System.Windows.Forms.ColumnHeader
	Private WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
	Private WithEvents cmdAddTheme As System.Windows.Forms.Button
	Private WithEvents Label1 As System.Windows.Forms.Label
	Private WithEvents txtProjectCode As System.Windows.Forms.TextBox
	Private WithEvents txtProjectName As System.Windows.Forms.TextBox
	Private WithEvents cmdEditTheme As System.Windows.Forms.Button
	Private WithEvents cmdDelete As System.Windows.Forms.Button
	Private WithEvents imlSmall As System.Windows.Forms.ImageList
End Class
