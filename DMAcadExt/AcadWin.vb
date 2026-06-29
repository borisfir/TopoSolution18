Option Explicit On
Option Strict On
Public Class AcadWin

	Implements System.Windows.Forms.IWin32Window
	Private moHandle As System.IntPtr
	Public Sub New(ByVal oHandle As System.IntPtr)
		moHandle = oHandle
	End Sub
	Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
		Get
			Return moHandle
		End Get
	End Property

End Class
