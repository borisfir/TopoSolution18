<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTopoOverlay
   Inherits frmMapThemeBase


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
      Me.SuspendLayout()
      '
      'frmTopoOverlay
      '
      Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
      '   Me.ClientSize = New System.Drawing.Size(453, 285)
      Me.Name = "frmTopoOverlay"
      Me.Text = "frmTopoOverlay"
      Me.ResumeLayout(False)

   End Sub
	Public Overrides Sub ExecDefaultAction()

	End Sub
	Protected Overrides Sub AfterChangeCurrent(iNewIndex As Integer)
      MyBase.OnChangeCurrent(iNewIndex)
      Select Case iNewIndex
         Case 0, 1
            Me.doaPanels(iNewIndex).Controls.Add(Me.tstTopology)
      End Select
   End Sub

   Public Overrides ReadOnly Property IsDone As Boolean
      Get
         Return mbIsDone
      End Get
   End Property

   Public Overrides ReadOnly Property IsDoneA As Boolean
      Get
         Return mbIsDone
      End Get
   End Property
End Class
