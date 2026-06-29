
Option Explicit On
Option Strict On
Namespace TPlanGraph

	Public Class TplnBNProject
		Private Shared moBNTopo As TplnBNTopo
		Public Shared Sub Init(tMapThemeData As DMAcadExt.MapThemeData)
			moBNTopo = New TplnBNTopo(tMapThemeData)

		End Sub
		Public Shared Sub Calculate()

			moBNTopo.LoadTopo()


		End Sub
		Public Shared Function GetBNPgon(ByVal iTopoID As Integer) As TplnBNPgon

			Return moBNTopo.GetPgon(iTopoID)

		End Function
		Public Shared ReadOnly Property TopologyName As String
			Get
				Return moBNTopo.TopologyName
			End Get
		End Property

	End Class
End Namespace

