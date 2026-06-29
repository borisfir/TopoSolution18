Namespace Expro
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
	Partial Class frmEntConnectedLot
		Inherits System.Windows.Forms.Form

		'Form overrides dispose to clean up the component list.
		<System.Diagnostics.DebuggerNonUserCode()>
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
		<System.Diagnostics.DebuggerStepThrough()>
		Private Sub InitializeComponent()
			Me.Label1 = New System.Windows.Forms.Label()
			Me.lvwEntLayers = New System.Windows.Forms.ListView()
			Me.colEntLayerName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
			Me.colEntCount = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
			Me.cmdTopo_Overlay = New System.Windows.Forms.Button()
			Me.cmdCalculate = New System.Windows.Forms.Button()
			Me.cmdExcel = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'Label1
			'
			Me.Label1.AutoSize = True
			Me.Label1.Location = New System.Drawing.Point(44, 56)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(90, 15)
			Me.Label1.TabIndex = 0
			Me.Label1.Text = "שכבות מחוברים"
			'
			'lvwEntLayers
			'
			Me.lvwEntLayers.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colEntLayerName, Me.colEntCount})
			Me.lvwEntLayers.Dock = System.Windows.Forms.DockStyle.Bottom
			Me.lvwEntLayers.HideSelection = False
			Me.lvwEntLayers.Location = New System.Drawing.Point(0, 74)
			Me.lvwEntLayers.MultiSelect = False
			Me.lvwEntLayers.Name = "lvwEntLayers"
			Me.lvwEntLayers.RightToLeftLayout = True
			Me.lvwEntLayers.Size = New System.Drawing.Size(243, 215)
			Me.lvwEntLayers.TabIndex = 1
			Me.lvwEntLayers.UseCompatibleStateImageBehavior = False
			Me.lvwEntLayers.View = System.Windows.Forms.View.Details
			'
			'colEntLayerName
			'
			Me.colEntLayerName.Text = "שם"
			Me.colEntLayerName.Width = 144
			'
			'colEntCount
			'
			Me.colEntCount.Text = "כמות"
			'
			'cmdTopo_Overlay
			'
			Me.cmdTopo_Overlay.FlatAppearance.BorderSize = 0
			Me.cmdTopo_Overlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdTopo_Overlay.Location = New System.Drawing.Point(3, 12)
			Me.cmdTopo_Overlay.Name = "cmdTopo_Overlay"
			Me.cmdTopo_Overlay.Size = New System.Drawing.Size(75, 23)
			Me.cmdTopo_Overlay.TabIndex = 2
			Me.cmdTopo_Overlay.Text = "Topology"
			Me.cmdTopo_Overlay.UseVisualStyleBackColor = True
			'
			'cmdCalculate
			'
			Me.cmdCalculate.FlatAppearance.BorderSize = 0
			Me.cmdCalculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdCalculate.Location = New System.Drawing.Point(84, 12)
			Me.cmdCalculate.Name = "cmdCalculate"
			Me.cmdCalculate.Size = New System.Drawing.Size(75, 23)
			Me.cmdCalculate.TabIndex = 3
			Me.cmdCalculate.Text = "Calculate"
			Me.cmdCalculate.UseVisualStyleBackColor = True
			'
			'cmdExcel
			'
			Me.cmdExcel.FlatAppearance.BorderSize = 0
			Me.cmdExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
			Me.cmdExcel.Location = New System.Drawing.Point(165, 12)
			Me.cmdExcel.Name = "cmdExcel"
			Me.cmdExcel.Size = New System.Drawing.Size(75, 23)
			Me.cmdExcel.TabIndex = 4
			Me.cmdExcel.Text = "Excel"
			Me.cmdExcel.UseVisualStyleBackColor = True
			'
			'frmEntConnectedLot
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
			Me.ClientSize = New System.Drawing.Size(243, 289)
			Me.Controls.Add(Me.cmdExcel)
			Me.Controls.Add(Me.cmdCalculate)
			Me.Controls.Add(Me.cmdTopo_Overlay)
			Me.Controls.Add(Me.lvwEntLayers)
			Me.Controls.Add(Me.Label1)
			Me.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
			Me.Name = "frmEntConnectedLot"
			Me.RightToLeft = System.Windows.Forms.RightToLeft.Yes
			Me.RightToLeftLayout = True
			Me.Text = "מחוברים"
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

		Friend WithEvents Label1 As Label
		Private WithEvents lvwEntLayers As ListView
		Private WithEvents colEntLayerName As ColumnHeader
		Private WithEvents colEntCount As ColumnHeader
		Private WithEvents cmdTopo_Overlay As Button
		Private WithEvents cmdCalculate As Button
		Private WithEvents cmdExcel As Button
	End Class
End Namespace
