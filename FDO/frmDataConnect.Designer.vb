Imports Autodesk.Gis.Map.DataConnect.UI
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDataConnect
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
		Me.btnFind = New System.Windows.Forms.Button()
		Me.btn1 = New System.Windows.Forms.Button()
		Me.btn2 = New System.Windows.Forms.Button()
		Me.btn3 = New System.Windows.Forms.Button()
		Me.btn4 = New System.Windows.Forms.Button()
		Me.btnRefresh = New System.Windows.Forms.Button()
		Me.btn5 = New System.Windows.Forms.Button()
		Me.btnDelete = New System.Windows.Forms.Button()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'btnFind
		'
		Me.btnFind.Location = New System.Drawing.Point(613, 4)
		Me.btnFind.Name = "btnFind"
		Me.btnFind.Size = New System.Drawing.Size(75, 23)
		Me.btnFind.TabIndex = 0
		Me.btnFind.Text = "Button"
		Me.btnFind.UseVisualStyleBackColor = True
		'
		'btn1
		'
		Me.btn1.Location = New System.Drawing.Point(613, 36)
		Me.btn1.Name = "btn1"
		Me.btn1.Size = New System.Drawing.Size(75, 23)
		Me.btn1.TabIndex = 3
		Me.btn1.Text = "btn1"
		Me.btn1.UseVisualStyleBackColor = True
		'
		'btn2
		'
		Me.btn2.Location = New System.Drawing.Point(612, 65)
		Me.btn2.Name = "btn2"
		Me.btn2.Size = New System.Drawing.Size(75, 23)
		Me.btn2.TabIndex = 4
		Me.btn2.Text = "btn2"
		Me.btn2.UseVisualStyleBackColor = True
		'
		'btn3
		'
		Me.btn3.Location = New System.Drawing.Point(612, 94)
		Me.btn3.Name = "btn3"
		Me.btn3.Size = New System.Drawing.Size(75, 23)
		Me.btn3.TabIndex = 5
		Me.btn3.Text = "btn3"
		Me.btn3.UseVisualStyleBackColor = True
		'
		'btn4
		'
		Me.btn4.Location = New System.Drawing.Point(612, 123)
		Me.btn4.Name = "btn4"
		Me.btn4.Size = New System.Drawing.Size(75, 23)
		Me.btn4.TabIndex = 6
		Me.btn4.Text = "btn4"
		Me.btn4.UseVisualStyleBackColor = True
		'
		'btnRefresh
		'
		Me.btnRefresh.Location = New System.Drawing.Point(703, 4)
		Me.btnRefresh.Name = "btnRefresh"
		Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
		Me.btnRefresh.TabIndex = 8
		Me.btnRefresh.Text = "Refresh"
		Me.btnRefresh.UseVisualStyleBackColor = True
		'
		'btn5
		'
		Me.btn5.Location = New System.Drawing.Point(613, 152)
		Me.btn5.Name = "btn5"
		Me.btn5.Size = New System.Drawing.Size(75, 23)
		Me.btn5.TabIndex = 7
		Me.btn5.Text = "btn5"
		Me.btn5.UseVisualStyleBackColor = True
		'
		'btnDelete
		'
		Me.btnDelete.Location = New System.Drawing.Point(691, 219)
		Me.btnDelete.Name = "btnDelete"
		Me.btnDelete.Size = New System.Drawing.Size(75, 23)
		Me.btnDelete.TabIndex = 9
		Me.btnDelete.Text = "Delete"
		Me.btnDelete.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(691, 180)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(75, 23)
		Me.Button1.TabIndex = 10
		Me.Button1.Text = "ShapeCon"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.Location = New System.Drawing.Point(613, 273)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(75, 23)
		Me.Button2.TabIndex = 11
		Me.Button2.Text = "Button2"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'frmDataConnect
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.AutoScroll = True
		Me.ClientSize = New System.Drawing.Size(1065, 1061)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.btnDelete)
		Me.Controls.Add(Me.btn5)
		Me.Controls.Add(Me.btnRefresh)
		Me.Controls.Add(Me.btn1)
		Me.Controls.Add(Me.btnFind)
		Me.Controls.Add(Me.btn4)
		Me.Controls.Add(Me.btn2)
		Me.Controls.Add(Me.btn3)
		Me.Name = "frmDataConnect"
		Me.Text = "frmDataConnect"
		Me.ResumeLayout(False)

	End Sub
	Private WithEvents btnFind As System.Windows.Forms.Button
	Private WithEvents btn1 As System.Windows.Forms.Button
	Private WithEvents btn2 As System.Windows.Forms.Button
	Private WithEvents btn3 As System.Windows.Forms.Button
	Private WithEvents btn4 As System.Windows.Forms.Button
	Private WithEvents btnRefresh As System.Windows.Forms.Button
	Private WithEvents btn5 As System.Windows.Forms.Button
	Private WithEvents btnDelete As System.Windows.Forms.Button
	Private WithEvents Button1 As System.Windows.Forms.Button
	Private WithEvents Button2 As Windows.Forms.Button


End Class
