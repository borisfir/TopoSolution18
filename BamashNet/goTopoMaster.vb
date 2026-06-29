Option Explicit On
Option Strict On
Public Class goTopoMaster
	Public Shared Function GetPgonArea(ByVal iTopoID As Integer) As Double

	End Function
	Public Shared Sub updateDisplay()

	End Sub

	Public Shared Function IsNothing() As Boolean

	End Function
	Public Shared Function StartTopo(ByVal sTopoName As String, ByVal dScale As Double) As Integer

	End Function
	Public Shared Function OpenTopo(ByVal dTopologyID As Double) As Integer

	End Function

	Public Shared Function GetIdPairs(ByVal iPolygonCount As Integer, ByRef mvTopoRefs(,) As System.Object) As Integer

	End Function

	Public Shared Function OpenPgon(ByVal iTopoID As Integer) As Integer

	End Function
	Public Shared Function BordPgon(ByVal oColorStrip As Integer) As Integer

	End Function
	Public Shared Function PutPgonAtts(ByVal iTopoID As Integer, ByVal vsaAttributes(,) As System.Object) As Integer

	End Function
	Public Shared Function SetActiveScale(ByVal dScale As Double) As Integer

	End Function
	Public Shared Function FillPgon(ByVal iColor As Integer) As Integer

	End Function
	Public Shared Function PaintPgon(ByVal vvaBodyParms() As System.Object, ByVal vdaBorders(,) As Double, ByVal vdaZebra(,) As Double) As Integer

	End Function
	Public Shared Sub DelPgonPaint(ByVal iTopoID As Integer)

	End Sub
	Public Shared Function GetPgonAtts(ByVal iTopoID As Integer, ByVal vsaAttributes(,) As System.Object) As Integer

	End Function
End Class
