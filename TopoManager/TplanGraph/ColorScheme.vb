Option Explicit On
Option Strict On

Public Enum PaintMethod
	FillPgon = 1
	BorderByTrim = 2
	ZebraByTopo = 3
	BorderByBuffer = 4
	BorderByBufferTopoExists = 5
End Enum
Public Structure DMColor
	Private Enum Source
		Empty
		Framework
		Acad
	End Enum
	Private miSource As Source
	Private mtFrameworkColor As System.Drawing.Color
	Private moAcadColor As Autodesk.AutoCAD.Colors.Color
	Public Sub New(ByVal tFrameworkColor As System.Drawing.Color)
		mtFrameworkColor = tFrameworkColor
		miSource = Source.Framework
	End Sub
	Public Sub New(ByVal oAcadColor As Autodesk.AutoCAD.Colors.Color)
		moAcadColor = oAcadColor
		miSource = Source.Acad
		mtFrameworkColor = moAcadColor.ColorValue
	End Sub
	Public Sub New(ByVal shAcadColorIndex As Short)
		moAcadColor = New Autodesk.AutoCAD.Colors.Color()
		moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Colors.ColorMethod.ByAci, shAcadColorIndex)
		mtFrameworkColor = moAcadColor.ColorValue
		'	MessageBox.Show(CStr(moAcadColor.ColorIndex), "12_790")

		miSource = Source.Acad
	End Sub
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return (miSource = Source.Empty)
		End Get
	End Property
	ReadOnly Property AcadColor() As Autodesk.AutoCAD.Colors.Color
		Get
			Return moAcadColor
		End Get
	End Property
	ReadOnly Property AcadColorIndex() As Integer
		Get
			If miSource = Source.Acad Then
				Return moAcadColor.ColorIndex
			Else
				Return 6
			End If

		End Get
	End Property
	ReadOnly Property HebColorName() As String
		Get
			Return GetColorName(mtFrameworkColor)
		End Get
	End Property
	Public Shared Function GetColorName(ByVal iAcadColor As System.Drawing.Color) As String
		Select Case iAcadColor.ToArgb()
			Case System.Drawing.Color.Blue.ToArgb()
				Return "כחול"
			Case Is = System.Drawing.Color.Cyan.ToArgb()
				'  return "טורקיז"
				Return "תכלת"
			Case &HFF00FF00, System.Drawing.Color.Green.ToArgb(), System.Drawing.Color.DarkGreen.ToArgb(), System.Drawing.Color.LightGreen.ToArgb(), System.Drawing.Color.LightSeaGreen.ToArgb()
				Return "ירוק"
			Case Is = System.Drawing.Color.Magenta.ToArgb()
				Return "סגול"
			Case System.Drawing.Color.Red.ToArgb()
				Return "אדום"
			Case System.Drawing.Color.White.ToArgb()
				Return "לבן"
			Case Is = System.Drawing.Color.Yellow.ToArgb()
				Return "צהוב"
			Case Else
				Return CStr(iAcadColor.ToArgb) & ":" & CStr(iAcadColor.ToKnownColor()) & vbNewLine & CStr(System.Drawing.Color.Blue.ToArgb) & ":" & CStr(System.Drawing.Color.DarkGreen.ToArgb)
				'	Return iAcadColor.ToString()
				'	Return "שגיאה"
		End Select
	End Function
End Structure
Public Structure ColorStrip
	Dim Color As DMColor
	Dim Width As Double
End Structure
Public Structure ColorZebra
	Dim Strips() As ColorStrip
	Dim dAngle As Double
	Public Sub AddStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		Dim iStripUB As Integer
		If Strips Is Nothing Then
			iStripUB = -1
		Else
			iStripUB = Strips.GetUpperBound(0)
		End If
		iStripUB += 1
		ReDim Preserve Strips(iStripUB)
		Strips(iStripUB).Color = tColor
		Strips(iStripUB).Width = dWidth

	End Sub
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return Strips Is Nothing
		End Get
	End Property
End Structure
Public Structure ColorBorder
	Public Strips() As ColorStrip
	Public Sub AddStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		Dim iStripUB As Integer
		If Strips Is Nothing Then
			iStripUB = -1
		Else
			iStripUB = Strips.GetUpperBound(0)
		End If
		iStripUB += 1
		ReDim Preserve Strips(iStripUB)
		Strips(iStripUB).Color = tColor
		Strips(iStripUB).Width = dWidth

	End Sub
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return Strips Is Nothing
		End Get
	End Property
End Structure
Public Structure TplnHatch
	Dim Name As String
	Dim Scale As Double
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return Name Is Nothing
		End Get
	End Property
End Structure
Public Structure ColorScheme
	Dim BackColor As DMColor
	Dim Border As ColorBorder
	Dim Zebra As ColorZebra
	Dim Hatch As TplnHatch

	Public Sub New(ByVal iBorderNum As Integer, ByVal iZebraNum As Integer)
		If iBorderNum > 0 Then
			ReDim Border.Strips(iBorderNum - 1)
		End If
		If iZebraNum > 0 Then
			ReDim Zebra.Strips(iZebraNum - 1)
		End If
	End Sub
End Structure


