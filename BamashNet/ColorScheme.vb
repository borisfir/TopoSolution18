Option Explicit On
Option Strict On
' From TopoManager

Public Enum PaintMethod
	FillPgon = 1
	BorderByTopoBuffer = 2
End Enum
Public Structure ColorStrip
	Dim Color As Integer
	Dim Width As Double
End Structure
Public Structure Zebra
	Dim Strips() As ColorStrip
	Dim dAngle As Double
End Structure
Public Structure Border
	Dim Strips() As ColorStrip
End Structure
Public Structure TplnHatch
	Dim Name As String
	Dim Scale As Double
End Structure
Public Structure ColorScheme
	Dim BackColor As System.Drawing.Color
	Dim Border As Border
	Dim Zebra As Zebra
	Dim Hatch As TplnHatch
End Structure




