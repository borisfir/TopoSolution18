Option Explicit On
Option Strict On
Public Enum enApplications
	Undefined = 0
	Taba = 1
	TopoMaster = 2
	Unidiv = 3

End Enum
Public Class DMApp
	Public Shared AppID As enApplications = enApplications.Undefined
	Public Shared Function GetDefaultRepScale() As Double
		Select Case AppID
			Case enApplications.Taba
				Return 1000
			Case enApplications.TopoMaster
				Return 1000
		End Select
	End Function
	Public Shared Function GetDefaultTableStyleName() As String
		Select Case AppID
			Case enApplications.Taba
				Return "Tplanner"
			Case enApplications.TopoMaster
				Return "Bamash"
			Case Else
				Return String.Empty
		End Select
	End Function
End Class
