Option Explicit On
Option Strict On
Namespace TPlanGraph
	Public Class TplnOwnerProject
		Private Shared moOwnerTopo As TplnOwnerTopo

		Public Shared Sub Init(tMapThemeData As DMAcadExt.MapThemeData)
			moOwnerTopo = New TplnOwnerTopo(tMapThemeData)

		End Sub
		Public Shared Sub Calculate()
		
			moOwnerTopo.LoadTopo()

		
		End Sub
		Public Shared Function GetOwnerPgon(ByVal iTopoID As Integer) As TplnOwnerPgon

			Return moOwnerTopo.GetPgon(iTopoID)

		End Function
		Public Shared ReadOnly Property TopologyName As String
			Get
				Return moOwnerTopo.TopologyName
			End Get
		End Property
	End Class
End Namespace