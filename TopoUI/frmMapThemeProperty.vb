Option Explicit On
Option Strict On
Public Class frmMapThemeProperty
	Private mtMapThemeData As DMAcadExt.MapThemeData
   Public Sub New(tMapThemeData As DMAcadExt.MapThemeData)

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      mtMapThemeData = tMapThemeData
      Me.prgMain.SelectedObject = tMapThemeData
   End Sub
   Public Sub New(oPropertyView As TopoManager.TopoPropertyView)

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.

      Me.prgMain.SelectedObject = oPropertyView
   End Sub
	Public Property SelectedObject As System.Object
		Get
			Return prgMain.SelectedObject
		End Get
		Set(oValue As System.Object)
			prgMain.SelectedObject = oValue
		End Set
	End Property



End Class