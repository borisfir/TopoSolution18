Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Public Class ColorPolygon
	Inherits TPlanGraph.TplnTopoPgon

	Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon, True)
	End Sub
	Public ReadOnly Property ExternalLoopAAA() As HatchLoop
		Get
			If MyBase.doExternalLoop Is Nothing Then
				MyBase.zzCalcExternalLoop()
			End If
			Return MyBase.doExternalLoop
		End Get
	End Property


	Public Property CurrentPgonTopoName() As String
		Get
			Return MyBase.dsCurrentPgonTopoName
		End Get
		Set(ByVal sValue As String)
			MyBase.dsCurrentPgonTopoName = sValue
		End Set
	End Property

	Public Overrides Sub Terminate()
		'MyBase.OnTerminate()
	End Sub

   Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
   End Property
   Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
   End Property

End Class
