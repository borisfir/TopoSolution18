
Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class RectangularPgonAAA
	Inherits TPlanGraph.TplnTopoPgon
	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon)
	End Sub

	Public ReadOnly Property ExternalLoop() As HatchLoop
		Get
			If MyBase.doExternalLoop Is Nothing Then
				MyBase.zzCalcExternalLoop()
			End If
			Return MyBase.doExternalLoop
		End Get
	End Property


	Public Overrides Sub Terminate()
		'MyBase.OnTerminate()
	End Sub
End Class

