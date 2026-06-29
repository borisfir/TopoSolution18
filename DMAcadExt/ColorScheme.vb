Option Explicit On
Option Strict On
Imports System.Data
Public Enum PaintMethod
	ColorScheme = 1
	Border = 2
	Zebra = 4
	Hatch = 8
	BorderByBuffer = 32
	BorderByTrim = 128
	ZebraByTopo = 1024
End Enum
Public Structure DMColor
	Const DBShift As Integer = &H90000000
	Public Enum enSource
		Empty
		Framework
		Acad
	End Enum
	Public Shared Function GetEmpty() As DMColor
		Dim tDMColor As DMColor = Nothing
		Return tDMColor
	End Function
	Private miSource As enSource
	Private mtFrameworkColor As System.Drawing.Color
	Private moAcadColor As Autodesk.AutoCAD.Colors.Color

	Public Sub New(ByVal tFrameworkColor As System.Drawing.Color)
		mtFrameworkColor = tFrameworkColor
		miSource = enSource.Framework
		moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColor(mtFrameworkColor)

		'	System.Windows.Forms.MessageBox.Show(mtFrameworkColor.ToString() & vbCrLf & CStr(moAcadColor.ColorIndex) & ":" & CStr(moAcadColor.ColorMethod & vbCrLf & CStr(moAcadColor.Red) & ":" & CStr(moAcadColor.Green) & ":" & CStr(moAcadColor.Blue)), "12_102")
	End Sub
	Public Sub New(ByVal sRGBString As String, Optional ByVal sDelim As String = ",")
		Dim saNumbers() As String = Strings.Split(sRGBString, sDelim)
		If saNumbers.GetUpperBound(0) = 2 Then
			Dim iaNumbers(2) As Integer
			Try
				For iIndex As Integer = 0 To 2
					iaNumbers(iIndex) = Convert.ToInt32(saNumbers(iIndex))
				Next
				mtFrameworkColor = Drawing.Color.FromArgb(iaNumbers(0), iaNumbers(1), iaNumbers(2))
				miSource = enSource.Framework
			Catch oEx As Exception
				miSource = enSource.Empty
			End Try
			
		ElseIf saNumbers.GetUpperBound(0) = 3 Then
			Dim iaNumbers(3) As Integer
			Try
				For iIndex As Integer = 0 To 3
					iaNumbers(iIndex) = Convert.ToInt32(saNumbers(iIndex))
				Next
				mtFrameworkColor = Drawing.Color.FromArgb(iaNumbers(0), iaNumbers(1), iaNumbers(2), iaNumbers(3))
				miSource = enSource.Framework
			Catch oEx As Exception
				miSource = enSource.Empty
			End Try
		Else
			miSource = enSource.Empty
		End If
		If mtFrameworkColor <> Drawing.Color.Empty Then
			moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColor(mtFrameworkColor)
		End If
	End Sub
	Public Sub New(ByVal oAcadColor As Autodesk.AutoCAD.Colors.Color)
		moAcadColor = oAcadColor
		miSource = enSource.Acad
		mtFrameworkColor = moAcadColor.ColorValue
	End Sub
	Public Sub New(ByVal shAcadColorIndex As Short)
		'	moAcadColor = New Autodesk.AutoCAD.Colors.Color()
		''''	DMAcadExt.AcadDocument.WriteMessage("shAcadColorIndex=: " & CStr(shAcadColorIndex))
		If shAcadColorIndex >= 0S And shAcadColorIndex <= 256S Then
			moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, shAcadColorIndex)
			mtFrameworkColor = moAcadColor.ColorValue
			miSource = enSource.Acad
		End If
	End Sub
	Public Shared Operator =(tDMColorA As DMColor, tDMColorB As DMColor) As Boolean
		Return tDMColorA.FrameworkColor = tDMColorB.FrameworkColor
	End Operator
	Public Shared Operator <>(tDMColorA As DMColor, tDMColorB As DMColor) As Boolean
		Return Not (tDMColorA = tDMColorB)
	End Operator
	Public Shared Function FromDB(ByVal iDBColor As Integer) As DMColor
		Dim tEmpty As DMColor = Nothing
		'System.Windows.Forms.MessageBox.Show(CStr(iDBColor), "12_980")
		If iDBColor = -1 Then
			Dim oDMColor As DMColor = Nothing
			Return oDMColor
		ElseIf iDBColor < 0 Then
			Dim shAcadColorIndex As Short
			Try
				shAcadColorIndex = Convert.ToInt16(iDBColor - DBShift)
				Return New DMColor(shAcadColorIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iDBColor) & ":" & CStr(DBShift), "DMColor - FromDB")
				Return tEmpty
			End Try
		Else
			Dim iRed, iGreen, iBlue As Integer
			Dim iDBColorSrc As Integer = iDBColor
			iDBColor = Math.DivRem(iDBColor, 256, iRed)
			iDBColor = Math.DivRem(iDBColor, 256, iGreen)
			iDBColor = Math.DivRem(iDBColor, 256, iBlue)
			Dim tFrameColor As System.Drawing.Color = System.Drawing.Color.FromArgb(iRed, iGreen, iBlue)
			tFrameColor.ToKnownColor()
			Dim tDMColor As DMColor = New DMColor(tFrameColor)
			'		System.Windows.Forms.MessageBox.Show(CStr(iDBColorSrc) & vbCrLf & CStr(iRed) & ":" & CStr(iGreen) & ":" & CStr(iBlue) & vbCrLf & CStr(tDMColor.AcadColorIndex) & vbCrLf & CStr(tDMColor.AcadColorIndex), "12_984")

			'	System.Windows.Forms.MessageBox.Show(CStr(tFrameColor.A) & vbCrLf & New DMColor(tFrameColor).ARGBString, "12_987")
			Return New DMColor(tFrameColor)
		End If
	End Function
	ReadOnly Property DBValue() As Integer
		Get
			If miSource = enSource.Framework Then

				'System.Windows.Forms.MessageBox.Show(CStr(Convert.ToUInt32(mtFrameworkColor.ToArgb())) & vbCrLf & CStr(mtFrameworkColor.ToArgb()) & ":" & CStr(mtFrameworkColor.A), "DMColor - DBValue_!!!!!!")

				Return RGB
				'	'''''''''''''''''Return Convert.ToInt32(mtFrameworkColor.R) + Convert.ToInt32(mtFrameworkColor.G) * 256 + Convert.ToInt32(mtFrameworkColor.B) * 65536
				'	Return mtFrameworkColor.ToArgb() - Convert.ToInt32(mtFrameworkColor.A) * 16777216
			ElseIf miSource = enSource.Acad Then
				Return Convert.ToInt32(moAcadColor.ColorIndex) + DBShift
			ElseIf miSource = enSource.Empty Then
				Return -1
			End If
		End Get
	End Property
	Public Function GetComStr(ByVal bValueExists As Boolean) As String
		Dim iValue As Integer = Me.DBValue
		If iValue = -1 AndAlso Not bValueExists Then
			Return "Null"
		Else
			Return Convert.ToString(iValue)
		End If
	End Function
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return (miSource = enSource.Empty)
		End Get
	End Property
	ReadOnly Property AcadColor() As Autodesk.AutoCAD.Colors.Color
		Get
			Return moAcadColor
		End Get
	End Property
	ReadOnly Property FrameworkColor() As System.Drawing.Color
		Get
			Return mtFrameworkColor
		End Get
	End Property
	ReadOnly Property RGB() As Integer
		Get
			Return mtFrameworkColor.R + 256 * (mtFrameworkColor.G + 256 * mtFrameworkColor.B)
		End Get
	End Property
	ReadOnly Property AcadColorIndex() As Short
		Get
			If moAcadColor IsNot Nothing Then
				Return moAcadColor.ColorIndex
			Else
				Return 0
			End If
		End Get
	End Property
	ReadOnly Property Source() As enSource
		Get
			Return miSource
		End Get
	End Property
	ReadOnly Property RGBString(Optional ByVal sDelim As String = ",") As String
		Get
			Return mtFrameworkColor.R & sDelim & mtFrameworkColor.G & sDelim & mtFrameworkColor.B
		End Get
	End Property
	ReadOnly Property ARGBString(Optional ByVal sDelim As String = ",") As String
		Get
			Return mtFrameworkColor.A & sDelim & mtFrameworkColor.R & sDelim & mtFrameworkColor.G & sDelim & mtFrameworkColor.B
		End Get
	End Property
	ReadOnly Property SelectColor() As System.Drawing.Color
		Get
			Return System.Drawing.Color.FromArgb(255 - mtFrameworkColor.R, 255 - mtFrameworkColor.G, 255 - mtFrameworkColor.B)

		End Get
	End Property
	ReadOnly Property CommonString(Optional ByVal sDelim As String = ",") As String
		Get
			If miSource = enSource.Acad Then
				Return moAcadColor.ColorIndex.ToString()
			ElseIf miSource = enSource.Framework Then
				Return mtFrameworkColor.R & sDelim & mtFrameworkColor.G & sDelim & mtFrameworkColor.B
			Else
				Return String.Empty
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
	Public Overrides Function ToString() As String
		Select Case miSource
			Case enSource.Acad
				Return moAcadColor.ToString()
			Case enSource.Framework
				Return mtFrameworkColor.ToString()
			Case Else
				Return String.Empty
		End Select
	End Function
End Structure
Public Structure ColorStrip
	Dim Color As DMColor
	Dim Width As Double
	Dim Scale As Double
	Public Sub New(ByVal tColor As DMColor, ByVal dWidth As Double)
		Color = tColor
		Width = dWidth
		Scale = 1.0
	End Sub
	Public Shared Operator =(tColorStripA As ColorStrip, tColorStripB As ColorStrip) As Boolean
		Return (tColorStripA.Width = tColorStripB.Width) AndAlso (tColorStripA.Color = tColorStripB.Color)
	End Operator
	Public Shared Operator <>(tColorStripA As ColorStrip, tColorStripB As ColorStrip) As Boolean
		Return Not (tColorStripA = tColorStripB)
	End Operator

	Public ReadOnly Property ScalingWidth() As Double
		Get
			'	AcadDocument.WriteMessage("!@! " & CStr(Width) & ":" & CStr(Scale))
			Return Width * Scale
		End Get
	End Property
	Public ReadOnly Property ScalingWidthSingle() As Single
		Get
			'	AcadDocument.WriteMessage("!@! " & CStr(Width) & ":" & CStr(Scale))
			Return Convert.ToSingle(Width * Scale)
		End Get
	End Property
	Public ReadOnly Property IsInstance As Boolean
		Get
			Return Width > 0.001
		End Get
	End Property
End Structure
Public Structure ColorZebra
	Private mtStrips() As ColorStrip
	Dim Angle As LineAngle
	Dim Scale As Double
	 
	Public Sub New(ByVal iZebraNum As Integer)
		ReDim mtStrips(iZebraNum - 1)
		Scale = 1.0
	End Sub
	Public Shared Operator =(tColorZebraA As ColorZebra, tColorZebraB As ColorZebra) As Boolean
		Dim iStripUB As Integer = tColorZebraA.StripUB()
		If (iStripUB = tColorZebraB.StripUB()) AndAlso (tColorZebraA.Angle = tColorZebraB.Angle) Then
			For iIndex As Integer = 0 To iStripUB
				If tColorZebraA.Strip(iIndex) <> tColorZebraB.Strip(iIndex) Then
					Return True
				End If
			Next
			Return True
		Else
			Return False
		End If
	End Operator
	Public Shared Operator <>(tColorZebraA As ColorZebra, tColorZebraB As ColorZebra) As Boolean
		Return Not (tColorZebraA = tColorZebraB)
	End Operator
	Public Sub AddStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		Dim iStripUB As Integer
		If mtStrips Is Nothing Then
			iStripUB = -1
			Scale = 1.0
		Else
			iStripUB = mtStrips.GetUpperBound(0)
		End If
		iStripUB += 1
		ReDim Preserve mtStrips(iStripUB)
		mtStrips(iStripUB).Color = tColor
		mtStrips(iStripUB).Width = dWidth
		mtStrips(iStripUB).Scale = 1.0
	End Sub
	Public Property Strip(ByVal iIndex As Integer) As ColorStrip
		Get
			mtStrips(iIndex).Scale = Scale
			Return mtStrips(iIndex)
		End Get
		Set(ByVal tValue As ColorStrip)
			mtStrips(iIndex) = tValue
		End Set
	End Property
	Public ReadOnly Property StripUB() As Integer
		Get
			If mtStrips Is Nothing Then
				Return -1
			Else
				Return mtStrips.GetUpperBound(0)
			End If

		End Get
	End Property
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return mtStrips Is Nothing
		End Get
	End Property
	ReadOnly Property IsInstance() As Boolean
		Get
			Dim bRes As Boolean
			If IsArray(mtStrips) Then
				bRes = True
				For iIndex As Integer = 0 To mtStrips.GetUpperBound(0)
					bRes = bRes AndAlso mtStrips(iIndex).IsInstance
				Next
			End If

			Return bRes
		End Get
	End Property

	Public Sub GetInsertComStr(ByRef sFieldList As String, ByRef sValueList As String)
		Dim sComma As String = ","
		Dim tStrip As ColorStrip
		If Me.StripUB >= 0 Then
			If sFieldList.Length <> 0 Then
				sFieldList &= sComma
				sValueList &= sComma
			End If
			sFieldList &= "ZebraAngle"
			sValueList &= CStr(Angle.AngleDegree)
		End If
		For iIndex As Integer = 0 To Me.StripUB
			tStrip = mtStrips(iIndex)
			sFieldList &= sComma & zzGetWidthFieldName(iIndex) & sComma & zzGetColorFieldName(iIndex)
			sValueList &= sComma & CStr(tStrip.Width) & sComma & CStr(tStrip.Color.DBValue)
		Next
	End Sub

	Public Function GetUpdateComStr() As String
		Dim sComma As String = ","
		Dim sAngleVal As String
		Dim tStrip As ColorStrip

		If mtStrips Is Nothing Then
			sAngleVal = "Null"
		Else
			sAngleVal = CStr(Angle.AngleDegree)
		End If
		Dim sRes As String = "ZebraAngle=" & sAngleVal & sComma
		For iIndex As Integer = 0 To Me.StripUB
			tStrip = mtStrips(iIndex)
			sRes &= zzGetWidthFieldName(iIndex) & "=" & CStr(tStrip.Width) & sComma
			sRes &= zzGetColorFieldName(iIndex) & "=" & tStrip.Color.GetComStr(True) & sComma
		Next
		For iIndex As Integer = Me.StripUB + 1 To 4
			sRes &= zzGetWidthFieldName(iIndex) & "=Null" & sComma
			If iIndex = 4 Then sComma = String.Empty
			sRes &= zzGetColorFieldName(iIndex) & "=Null" & sComma
		Next
		Return sRes
	End Function
	Public Sub Mirror()
		Angle.Mirror()
	End Sub
	Private Function zzGetColorFieldName(ByVal iIndex As Integer) As String
		Return "ZebraColor" & CStr(iIndex + 1)

	End Function
	Private Function zzGetWidthFieldName(ByVal iIndex As Integer) As String
		Return "ZebraWidth" & CStr(iIndex + 1)
	End Function

End Structure
Public Structure ColorBorder
	Private mtStrips() As ColorStrip
	Public Scale As Double
	Private miRecursionCounter As Integer
	Public Sub New(ByVal iBorderNum As Integer)
		ReDim mtStrips(iBorderNum - 1)
		Scale = 1
	End Sub
	Public Shared Operator =(tColorBorderA As ColorBorder, tColorBorderB As ColorBorder) As Boolean
		Dim iStripUB As Integer = tColorBorderA.StripUB()
		If iStripUB = tColorBorderB.StripUB() Then
			For iIndex As Integer = 0 To iStripUB
				If tColorBorderA.Strip(iIndex) <> tColorBorderB.Strip(iIndex) Then
					Return True
				End If
			Next
			Return True
		Else
			Return False
		End If
	End Operator
	Public Shared Operator <>(tColorBorderA As ColorBorder, tColorBorderB As ColorBorder) As Boolean
		Return Not (tColorBorderA = tColorBorderB)
	End Operator


	Public Sub DeleteStrip()
		If mtStrips IsNot Nothing Then
			Dim iStripUB As Integer = mtStrips.GetUpperBound(0)
			If iStripUB = 0 Then
				Erase mtStrips
			Else
				ReDim Preserve mtStrips(iStripUB - 1)
			End If
		End If
	End Sub
	Public Sub AddStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		Dim iStripUB As Integer
		If mtStrips Is Nothing Then
			iStripUB = -1
			Scale = 1
		Else
			iStripUB = mtStrips.GetUpperBound(0)
		End If
		iStripUB += 1
		ReDim Preserve mtStrips(iStripUB)
		mtStrips(iStripUB) = New ColorStrip(tColor, dWidth)
	End Sub
	Public Function GetRecursionStrip() As ColorStrip
		Return Me.Strip(miRecursionCounter)
	End Function
	Public Property Strip(ByVal iIndex As Integer) As ColorStrip
		Get
			If Me.IsEmpty Then
				Err.Raise(vbObjectError + 512 + 13, "ColorStrip", "Object is nothing")
				Return Nothing
			Else
				mtStrips(iIndex).Scale = Scale
				Return mtStrips(iIndex)
			End If
		End Get
		Set(ByVal tValue As ColorStrip)
			mtStrips(iIndex) = tValue
		End Set
	End Property
	Public ReadOnly Property StripUB() As Integer
		Get
			If mtStrips Is Nothing Then
				Return -1
			Else
				Return mtStrips.GetUpperBound(0)
			End If
		End Get
	End Property
	ReadOnly Property IsEmpty() As Boolean
		Get
			Return mtStrips Is Nothing
		End Get
	End Property
	Public Sub GetInsertComStr(ByRef sFieldList As String, ByRef sValueList As String)
		Dim sComma As String = ","
		Dim tStrip As ColorStrip

		For iIndex As Integer = 0 To Me.StripUB
			tStrip = mtStrips(iIndex)
			If sFieldList.Length <> 0 Then
				sFieldList &= sComma
				sValueList &= sComma
			End If
			sFieldList &= zzGetWidthFieldName(iIndex) & sComma
			sFieldList &= zzGetColorFieldName(iIndex)
			sValueList &= CStr(tStrip.Width) & sComma
			sValueList &= CStr(tStrip.Color.DBValue)
		Next
	End Sub
	Public Function NextRecursion() As Boolean
		miRecursionCounter += 1
		Return HasRecursionStrip()
	End Function
	Public Function HasRecursionStrip() As Boolean
		If mtStrips IsNot Nothing Then
			'	System.Windows.Forms.MessageBox.Show(CStr((mtStrips IsNot Nothing)) & ":" & CStr(miRecursionCounter) & ":" & CStr(mtStrips.GetUpperBound(0)), "20_201")
		Else
			'	System.Windows.Forms.MessageBox.Show(CStr((mtStrips IsNot Nothing)), "20_202")
		End If

		Return (mtStrips IsNot Nothing) AndAlso (miRecursionCounter <= mtStrips.GetUpperBound(0))
	End Function
	Public Function GetOffset(ByVal bMinus As Boolean) As String
		Dim dOffset As Double
		Dim oStrip As ColorStrip = Me.GetRecursionStrip()
		If bMinus Then
			dOffset = -oStrip.ScalingWidth
		Else
			dOffset = oStrip.ScalingWidth
		End If
		Return Convert.ToString(dOffset)
	End Function
	Public Function GetUpdateComStr() As String
		Dim sComma As String = ","
		Dim tStrip As ColorStrip
		Dim sRes As String = String.Empty
		For iIndex As Integer = 0 To Me.StripUB
			tStrip = mtStrips(iIndex)
			sRes &= zzGetWidthFieldName(iIndex) & "=" & CStr(tStrip.Width) & sComma
			sRes &= zzGetColorFieldName(iIndex) & "=" & CStr(tStrip.Color.DBValue) & sComma
		Next
		For iIndex As Integer = Me.StripUB + 1 To 4
			sRes &= zzGetWidthFieldName(iIndex) & "=Null" & sComma
			If iIndex = 4 Then sComma = String.Empty
			sRes &= zzGetColorFieldName(iIndex) & "=Null" & sComma
		Next
		Return sRes
	End Function
	Private Function zzGetColorFieldName(ByVal iIndex As Integer) As String
		Return "BorderColor" & CStr(iIndex + 1)
	End Function
	Private Function zzGetWidthFieldName(ByVal iIndex As Integer) As String
		Return "BorderWidth" & CStr(iIndex + 1)
	End Function
End Structure
Public Structure DMHatch
	Public Enum HatchUnit
		NotDefined
		Mm
		Inch
	End Enum
	Const ToMm As Double = 25.4
	Const OffsetMm As Double = 3.175
	Const SolidHatchName As String = "SOLID"
	Const LineHatchName As String = "LINE"
	Const NetHatchName As String = "NET"
	Shared PatternUnit As HatchUnit
	Dim BackColor As DMColor
	Dim PatternName As String
	Dim Pattern As DMPattern
	Dim Angle As LineAngle
	Dim Mirror As Boolean
	Dim PatternColor As DMColor
	Dim LineWeight As Autodesk.AutoCAD.DatabaseServices.LineWeight
	Dim PatternScale As Double
	Dim Scale As Double

	Public Sub New(ByVal tDMColor As DMColor)
		BackColor = tDMColor
	End Sub
	Public Sub New(ByVal sPatternName As String)
		PatternName = sPatternName
	End Sub
	Public Sub New(ByVal tPatternName As DMPattern)
		Pattern = tPatternName
		PatternName = tPatternName.Name

	End Sub
	Public Shared Operator =(tDMHatchA As DMHatch, tDMHatchB As DMHatch) As Boolean
		If tDMHatchA.BackColor = tDMHatchA.BackColor Then
			If tDMHatchA.SolidOnly AndAlso tDMHatchB.SolidOnly Then
				Return True
			ElseIf Not tDMHatchA.SolidOnly AndAlso Not tDMHatchB.SolidOnly Then
				Return tDMHatchA.Pattern = tDMHatchB.Pattern
			End If
		Else
			Return False
		End If
	End Operator
	Public Shared Operator <>(tDMHatchA As DMHatch, tDMHatchB As DMHatch) As Boolean
		Return Not (tDMHatchA = tDMHatchB)
	End Operator
	Public Shared Sub SetHatchUnit(ByVal oPatternDefinition As Autodesk.AutoCAD.DatabaseServices.PatternDefinition, ByVal dSpacing As Double)
		Const dEps As Double = 0.1
		Dim dSpaceMm As Double = OffsetMm * dSpacing	 '3.96875
		Dim dSpaceInch As Double = dSpaceMm / ToMm	 '0.15625
		Dim dOffsetSq As Double

		With oPatternDefinition
			dOffsetSq = .OffsetX * .OffsetX + .OffsetY * .OffsetY
		End With
		'	AcadDocument.WriteMessage("dOffsetSq=" & CStr(dOffsetSq) & "; Mm=" & CStr(dSpaceMm * dSpaceMm) & "; Inch=" & CStr(dSpaceInch * dSpaceInch))
		If Math.Abs(dOffsetSq - dSpaceMm * dSpaceMm) < dEps Then
			PatternUnit = HatchUnit.Mm
		ElseIf Math.Abs(dOffsetSq - dSpaceInch * dSpaceInch) < dEps Then
			PatternUnit = HatchUnit.Inch
		End If
		'	AcadDocument.WriteMessage("PatternUnit=" & PatternUnit.ToString())
	End Sub
	ReadOnly Property IsEmpty() As Boolean
		Get
         Return (SolidOnly AndAlso BackColor.IsEmpty)
		End Get
	End Property
	ReadOnly Property SolidOnly() As Boolean
		Get
         Return String.IsNullOrEmpty(PatternName)
		End Get
	End Property
	ReadOnly Property LineWeightInt() As Integer
		Get
			Return CType(LineWeight, Integer)
		End Get
	End Property
	Public ReadOnly Property ScalingPatternScale() As Double
		Get
			If PatternUnit = HatchUnit.Inch Then
				Return PatternScale * Scale * ToMm
			Else
				Return PatternScale * Scale
			End If
		End Get
	End Property
	Public Sub GetInsertComStr(ByRef sFieldList As String, ByRef sValueList As String)
		Dim sComma As String = ","
		If Not BackColor.IsEmpty Then
			If sFieldList.Length <> 0 Then
				sFieldList &= sComma
				sValueList &= sComma
			End If
			sFieldList &= "FillColor"
			sValueList &= CStr(BackColor.DBValue)
		End If
		If Not Me.SolidOnly Then
			If sFieldList.Length <> 0 Then
				sFieldList &= sComma
				sValueList &= sComma
			End If
			sFieldList &= "HatchName" & sComma
			sValueList &= "'" & Me.PatternName & "'" & sComma
			sFieldList &= "HatchAngle" & sComma
			sValueList &= Convert.ToString(Angle.AngleDegree) & sComma

			sFieldList &= "HatchScale" & sComma
			sValueList &= Convert.ToString(PatternScale) & sComma

			sFieldList &= "HatchColor" & sComma
			sValueList &= Convert.ToString(PatternColor.DBValue) & sComma

			sFieldList &= "HatchLineWeight"
			sValueList &= Convert.ToString(Me.LineWeightInt)

		End If
	End Sub


	Public Function GetUpdateComStr() As String
		Const sComma As String = ","
		Dim sRes As String = "FillColor=" & BackColor.GetComStr(Not Me.SolidOnly) & sComma
		Dim sPatternNameVal As String
		Dim sAngleVal As String
		Dim sScaleVal As String
		Dim sColor As String
		Dim sLineWeight As String
		If Me.SolidOnly Then
			sPatternNameVal = "Null"
			sAngleVal = "Null"
			sScaleVal = "Null"
			sColor = "Null"
			sLineWeight = "Null"
		Else
			sPatternNameVal = "'" & Me.PatternName & "'"
			sAngleVal = Convert.ToString(Angle.AngleDegree)
			sScaleVal = Convert.ToString(PatternScale)
			sColor = PatternColor.GetComStr(False)
			sLineWeight = Convert.ToString(Me.LineWeightInt)
		End If
		sRes &= "HatchName=" & sPatternNameVal & sComma
		sRes &= "HatchAngle=" & sAngleVal & sComma
		sRes &= "HatchScale=" & sScaleVal & sComma
		sRes &= "HatchColor=" & sColor & sComma
		sRes &= "HatchLineWeight=" & sLineWeight
		Return sRes
	End Function




End Structure
Public Structure ColorScheme
	Public Const MaxStandardID As Integer = 4000
	Public Const MaxLanduseID As Integer = 14000
	Public Const MaxColorSchemeID As Integer = 200000
	Public Const StandardName As String = "מבא""ת"
	Public Const AllLanduseName As String = "כל היעודים"
	Public Const LocalName As String = "לוקלי"
	Public Const NameDelim As String = " - "
	Const mdScaleMin As Double = 0.0001
	Public Scale As Double
	Public Order As Integer
	Private mtBorder As ColorBorder
	Private mtZebra As ColorZebra
	Private mtHatch As DMHatch
	Private mbIsInstance As Boolean
	Private miID As Integer
	Private msName As String
	Private msTableName As String
	Public Sub New(ByVal iBorderNum As Integer, ByVal iZebraNum As Integer)
		mbIsInstance = True
		Scale = 1
		If iBorderNum > 0 Then
			mtBorder = New ColorBorder(iBorderNum)
		End If
		If iZebraNum > 0 Then
			mtZebra = New ColorZebra(iZebraNum)
		End If
	End Sub
	Public Sub New(ByVal dScale As Double)
		mbIsInstance = True
		zzSetScale(dScale)


	End Sub
	Private Sub zzSetScale(ByVal dScale As Double)
		If dScale < mdScaleMin Then
			Scale = 1
		Else
			Scale = dScale
		End If
	End Sub
	Public Sub New(ByRef oDataReader As Common.DbDataReader, ByVal iFieldShift As Integer, ByVal dScale As Double)
		'	Me.zzNewSrc121212(oDataReader, iFieldShift, dScale)
		Me.zzNew(oDataReader, iFieldShift, dScale)
	End Sub
	Public Sub New(ByVal iID As Integer, ByVal dScale As Double, Optional sTableName As String = "ColorSchemes")
		'System.Windows.Forms.MessageBox.Show(CStr(iRes), "18_540 Res of Command")       
		msTableName = sTableName
		Dim sComText As String = "SELECT * FROM " & sTableName & " WHERE (ID=" & CStr(iID) & ")"
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then
			Me.zzNew(oDataReader, 0, dScale)
			oDataReader.Close()
			'	System.Windows.Forms.MessageBox.Show(CStr(oDataReader.RecordsAffected), "18_998 DataReader.Close")
		End If
	End Sub

	Public Shared Operator =(tColorSchemeA As ColorScheme, tColorSchemeB As ColorScheme) As Boolean
		Return (tColorSchemeA.Hatch = tColorSchemeB.Hatch) AndAlso (tColorSchemeA.Border = tColorSchemeB.Border) AndAlso (tColorSchemeA.Zebra = tColorSchemeB.Zebra)
	End Operator
	Public Shared Operator <>(tColorSchemeA As ColorScheme, tColorSchemeB As ColorScheme) As Boolean
		Return Not (tColorSchemeA = tColorSchemeB)
	End Operator
	Private Sub zzNew(ByRef oDataReader As Common.DbDataReader, ByVal iFieldShift As Integer, ByVal dScale As Double)
		DMAcadExt.DMPatterns.Init()
		zzSetScale(dScale)
		If oDataReader IsNot Nothing Then
			mbIsInstance = True
			Dim iColor As Integer
			Dim dWidth As Double
			'	System.Windows.Forms.MessageBox.Show(CStr(iFieldShift), "26_885")	'& ":" & CStr(iColorSchemeID)
			If iFieldShift > 0 OrElse oDataReader.Read Then
				mbIsInstance = True
				miID = oDataReader.GetInt32(iFieldShift)
				If Not oDataReader.IsDBNull(1 + iFieldShift) Then
					msName = oDataReader.GetString(1 + iFieldShift)
				End If
				If Not oDataReader.IsDBNull(4 + iFieldShift) AndAlso Not oDataReader.IsDBNull(5 + iFieldShift) Then
					iColor = oDataReader.GetInt32(4 + iFieldShift)
					dWidth = oDataReader.GetDouble(5 + iFieldShift)
					mtBorder.AddStrip(DMColor.FromDB(iColor), dWidth)
					If Not oDataReader.IsDBNull(6 + iFieldShift) AndAlso Not oDataReader.IsDBNull(7 + iFieldShift) Then
						iColor = oDataReader.GetInt32(6 + iFieldShift)
						dWidth = oDataReader.GetDouble(7 + iFieldShift)
						mtBorder.AddStrip(DMColor.FromDB(iColor), dWidth)
						If Not oDataReader.IsDBNull(8 + iFieldShift) AndAlso Not oDataReader.IsDBNull(9 + iFieldShift) Then
							iColor = oDataReader.GetInt32(8 + iFieldShift)
							dWidth = oDataReader.GetDouble(9 + iFieldShift)
							mtBorder.AddStrip(DMColor.FromDB(iColor), dWidth)
							If Not oDataReader.IsDBNull(10 + iFieldShift) AndAlso Not oDataReader.IsDBNull(11 + iFieldShift) Then
								iColor = oDataReader.GetInt32(10 + iFieldShift)
								dWidth = oDataReader.GetDouble(11 + iFieldShift)
								mtBorder.AddStrip(DMColor.FromDB(iColor), dWidth)
								If Not oDataReader.IsDBNull(12 + iFieldShift) AndAlso Not oDataReader.IsDBNull(13 + iFieldShift) Then
									iColor = oDataReader.GetInt32(12 + iFieldShift)
									dWidth = oDataReader.GetDouble(13 + iFieldShift)
									mtBorder.AddStrip(DMColor.FromDB(iColor), dWidth)
								End If
							End If
						End If
					End If
				End If
				If Not oDataReader.IsDBNull(14 + iFieldShift) AndAlso Not oDataReader.IsDBNull(15 + iFieldShift) AndAlso Not oDataReader.IsDBNull(16 + iFieldShift) Then
					mtZebra.Angle = New LineAngle(oDataReader.GetDouble(14 + iFieldShift), False)
					iColor = oDataReader.GetInt32(15 + iFieldShift)
					dWidth = oDataReader.GetDouble(16 + iFieldShift)
					mtZebra.AddStrip(DMColor.FromDB(iColor), dWidth)
					If Not oDataReader.IsDBNull(17 + iFieldShift) AndAlso Not oDataReader.IsDBNull(18 + iFieldShift) Then
						iColor = oDataReader.GetInt32(17 + iFieldShift)
						dWidth = oDataReader.GetDouble(18 + iFieldShift)
						mtZebra.AddStrip(DMColor.FromDB(iColor), dWidth)
						If Not oDataReader.IsDBNull(19 + iFieldShift) AndAlso Not oDataReader.IsDBNull(20 + iFieldShift) Then
							iColor = oDataReader.GetInt32(19 + iFieldShift)
							dWidth = oDataReader.GetDouble(20 + iFieldShift)
							mtZebra.AddStrip(DMColor.FromDB(iColor), dWidth)
							If Not oDataReader.IsDBNull(21 + iFieldShift) AndAlso Not oDataReader.IsDBNull(22 + iFieldShift) Then
								iColor = oDataReader.GetInt32(21 + iFieldShift)
								dWidth = oDataReader.GetDouble(22 + iFieldShift)
								mtZebra.AddStrip(DMColor.FromDB(iColor), dWidth)
								If Not oDataReader.IsDBNull(23 + iFieldShift) AndAlso Not oDataReader.IsDBNull(24 + iFieldShift) Then
									iColor = oDataReader.GetInt32(23 + iFieldShift)
									dWidth = oDataReader.GetDouble(24 + iFieldShift)
									mtZebra.AddStrip(DMColor.FromDB(iColor), dWidth)
								End If
							End If
						End If
					End If
				End If

				If Not oDataReader.IsDBNull(25 + iFieldShift) Then
					Dim sPatternName As String = oDataReader.GetString(25 + iFieldShift)
					Dim tPattern As DMAcadExt.DMPattern = Nothing

					If DMPatterns.PatternsExist AndAlso DMPatterns.Item(sPatternName, tPattern) Then
						mtHatch.Pattern = tPattern
						mtHatch.PatternName = sPatternName
						If Not oDataReader.IsDBNull(26 + iFieldShift) Then
							mtHatch.Angle = New LineAngle(oDataReader.GetDouble(26 + iFieldShift), False)
						End If
						If oDataReader.IsDBNull(27 + iFieldShift) Then
							mtHatch.PatternScale = 1.0
						Else
							mtHatch.PatternScale = oDataReader.GetDouble(27 + iFieldShift)
						End If
						If Not oDataReader.IsDBNull(28 + iFieldShift) Then
							mtHatch.PatternColor = DMColor.FromDB(oDataReader.GetInt32(28 + iFieldShift))
						End If
						If Not oDataReader.IsDBNull(29 + iFieldShift) Then
							Dim iLineWeight As Integer = oDataReader.GetInt32(29 + iFieldShift)
							If [Enum].IsDefined(GetType(Autodesk.AutoCAD.DatabaseServices.LineWeight), oDataReader.GetInt32(29 + iFieldShift)) Then
								mtHatch.LineWeight = CType(iLineWeight, Autodesk.AutoCAD.DatabaseServices.LineWeight)
							End If
						End If
					ElseIf DMPatterns.PatternsExist Then
						System.Windows.Forms.MessageBox.Show("Pattern '" & sPatternName & "' was not found" & vbCrLf & "Count of Patterns is " & CStr(DMPatterns.Count), "ColorScheme")
					Else
						System.Windows.Forms.MessageBox.Show("Pattern Table was not found", "ColorScheme!!")
					End If
				End If
				If Not oDataReader.IsDBNull(3 + iFieldShift) Then
					'''''''''''''''''''''''cccccccccccccc 
					mtHatch.BackColor = DMColor.FromDB(oDataReader.GetInt32(3 + iFieldShift))
				End If
			End If
		End If

	End Sub





	



	Public Shared Function GetFullName(ByVal iLanduseID As Integer, ByVal iColorSchemeID As Integer, ByVal sName As String) As String

		Dim sPrefix As String = String.Empty
		If iLanduseID <> iColorSchemeID AndAlso iLanduseID <> 0 Then
			sPrefix = Convert.ToString(iColorSchemeID) & "/" '& Convert.ToString(iLanduseID)
		End If
		Return sPrefix & Convert.ToString(iLanduseID) & " - " & sName	' mtColorScheme.ID_Name
	End Function
	Public Function UpdateDB(ByVal iID As Integer) As Boolean
		Dim sUpdate As String
		Dim sCom As String
		Dim sNameMod As String = DMCommon.Functions.StringToSQL(msName)
		If iID = 0 OrElse msName Is Nothing OrElse msName.Length = 0 Then
			Return False
		Else
			sUpdate = "Name = '" & sNameMod & "'," & mtHatch.GetUpdateComStr & "," & mtBorder.GetUpdateComStr & "," & mtZebra.GetUpdateComStr
			sCom = "UPDATE " & msTableName & " SET " & sUpdate & " WHERE (((ID)=" & CStr(iID) & "))"
			AcadDocument.WriteDebugMessage("UpdateDB: " & sCom)
			Return TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sCom, CommandType.Text) = 1
		End If
	End Function
	Public Function AddNewToDB(ByVal iID As Integer) As Boolean
		Dim sFieldList As String = "ID,Name"
		Dim sNameMod As String = Strings.Replace(msName, "'", "''")
		Dim sValueList As String = CStr(iID) & ",'" & sNameMod & "'"
		Dim iRes As Integer
		Dim sCom As String
		mtHatch.GetInsertComStr(sFieldList, sValueList)
		mtBorder.GetInsertComStr(sFieldList, sValueList)
		mtZebra.GetInsertComStr(sFieldList, sValueList)
		sCom = "INSERT INTO " & msTableName & " (" & sFieldList & ") SELECT " & sValueList
		AcadDocument.WriteMessage(sCom)
        iRes = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sCom, CommandType.Text)
		'	System.Windows.Forms.MessageBox.Show(CStr(iRes), "18_540 Res of Command")
		'Return (TPlServerDB.ServerDB.RunCommand(sCom) = 1)
		Return iRes = 1
	End Function
	Public ReadOnly Property ID() As Integer
		Get
			Return miID
		End Get
	End Property
	Public ReadOnly Property HasName() As Boolean
		Get
			Return (msName IsNot Nothing) AndAlso (msName.Length <> 0)
		End Get
	End Property
	Public Property Name() As String
		Get
			Return msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
		End Set
	End Property
	Public ReadOnly Property ID_Name() As String
		Get
			Return DMAcadExt.ColorScheme.GetFullName(miID, 0, msName)
		End Get
	End Property
	Public Sub AddBorderStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		mtBorder.AddStrip(tColor, dWidth)
		mtBorder.Scale = Scale
	End Sub
	Public Sub NextBorderStrip()
		mtBorder.NextRecursion()
	End Sub
	Public Sub DeleteBorderStrip()
		mtBorder.DeleteStrip()
	End Sub
	Public Sub AddZebraStrip(ByVal tColor As DMColor, ByVal dWidth As Double)
		mtZebra.AddStrip(tColor, dWidth)
	End Sub
	Public ReadOnly Property IsInstance() As Boolean
		Get
			Return mbIsInstance
		End Get
	End Property
	Public ReadOnly Property IsStandard() As Boolean
		Get
			Return (miID <= MaxStandardID)
		End Get
	End Property

	Public ReadOnly Property IsLanduse() As Boolean
		Get
			Return (miID <= MaxLanduseID)
		End Get
	End Property
	Public ReadOnly Property HasBorder() As Boolean
		Get
			Return (Not mtBorder.IsEmpty)
		End Get
	End Property
	Public ReadOnly Property HasRecursionBorder() As Boolean
		Get
			Return mtBorder.HasRecursionStrip
		End Get
	End Property
	Public ReadOnly Property HasZebra() As Boolean
		Get
			Return (Not mtZebra.IsEmpty)
		End Get
	End Property
	Public ReadOnly Property HasHatch() As Boolean
		Get
			Return (Not mtHatch.IsEmpty)
		End Get
	End Property
	Public Function GetBackColor() As DMColor
		Return mtHatch.BackColor
	End Function
	Public Sub SetBackColor(ByVal tColor As DMColor)
		mtHatch.BackColor = tColor
		'	mtHatch.PatternName = "SOLID"
   End Sub
   Public ReadOnly Property NeedTopology() As Boolean
      Get
         Return (Not mtBorder.IsEmpty) OrElse (Not mtZebra.IsEmpty)
      End Get
   End Property
	Public Property TableName() As String
		Get

			Return msTableName
		End Get
		Set(ByVal sValue As String)
			msTableName = sValue
		End Set
	End Property
	Public Property Border() As ColorBorder
		Get
			mtBorder.Scale = Scale
			Return mtBorder
		End Get
		Set(ByVal tValue As ColorBorder)
			mtBorder = tValue
		End Set
	End Property
	Public Property Zebra() As ColorZebra
		Get
			mtZebra.Scale = Scale
			Return mtZebra
		End Get
		Set(ByVal tValue As ColorZebra)
			mtZebra = tValue
			If Not mbIsInstance Then
				mbIsInstance = tValue.IsInstance
			End If

		End Set
	End Property
	Public Property Hatch() As DMHatch
		Get
			mtHatch.Scale = Scale
			Return mtHatch
		End Get
		Set(ByVal tValue As DMHatch)
			mtHatch = tValue
			If Not mbIsInstance Then
				mbIsInstance = Not tValue.IsEmpty
			End If


		End Set
	End Property
	Public Property ZebraAngle() As LineAngle
		Get
			Return mtZebra.Angle
		End Get
		Set(ByVal dValue As LineAngle)
			mtZebra.Angle = dValue
		End Set
	End Property
	Public Sub Mirror()
		If Me.HasHatch Then
			If Not mtHatch.SolidOnly Then
				mtHatch.Mirror = True
			End If
		End If
		If Me.HasZebra Then
			mtZebra.Mirror()
		End If
	End Sub
	Public Shared Function GetEmpty() As ColorScheme
		Dim tColorScheme As ColorScheme = Nothing
		Return tColorScheme
	End Function
End Structure
Public Enum enQuadrants
	QuadrantI = 1
	QuadrantII = 2
End Enum
Public Structure LineAngle
	Dim AngleRad As Double
	Dim AngleDegree As Double
	Dim Tolerance As Double

	Private mdAngleRadI As Double
	Private mdAngleDegI As Double
	Private miQuadrant As enQuadrants
	Public Sub New(ByVal dAngle As Double, ByVal bRadian As Boolean)
		Dim dAngleDegree As Double
		If bRadian Then
			dAngleDegree = dAngle * 180.0 / Math.PI
		Else
			dAngleDegree = dAngle
		End If
		'dAngle * Math.PI / 180.0
		dAngleDegree = dAngleDegree Mod 180.0
		If dAngleDegree < 0.0 Then
			dAngleDegree += 180
		End If
		zzCalc(dAngleDegree)
	End Sub
	Public Sub New(ByVal dAngle As Double, ByVal iQuadrant As Integer, ByVal bRadian As Boolean)
		Dim dAngleDegree As Double


		If bRadian Then
			dAngleDegree = dAngle * 180.0 / Math.PI
		Else
			dAngleDegree = dAngle
		End If
		If dAngleDegree <> 90.0 Then
			dAngleDegree = dAngleDegree Mod 90.0
			If dAngleDegree < 0.0 Then
				dAngleDegree += 90
			End If
		End If
		
		mdAngleDegI = dAngleDegree
		mdAngleRadI = dAngleDegree * Math.PI / 180.0

		Select Case iQuadrant
			Case 1
				miQuadrant = enQuadrants.QuadrantI
			Case 2
				miQuadrant = enQuadrants.QuadrantII
				dAngleDegree = 180 - dAngleDegree
		End Select

		'dAngle * Math.PI / 180.0

		AngleDegree = dAngleDegree
		AngleRad = dAngle * Math.PI / 180.0



	End Sub

	Public Shared Operator =(tLineAngleA As LineAngle, tLineAngleB As LineAngle) As Boolean
		Return (tLineAngleA.AngleRad = tLineAngleB.AngleRad)
	End Operator
	Public Shared Operator <>(tLineAngleA As LineAngle, tLineAngleB As LineAngle) As Boolean
		Return Not (tLineAngleA = tLineAngleB)
	End Operator



	Public ReadOnly Property BaseAngle(ByVal bRadian As Boolean) As Double
		Get
			If bRadian Then
				Return mdAngleRadI
			Else
				Return mdAngleDegI
			End If
		End Get
	End Property
	Public ReadOnly Property Quadrant() As enQuadrants
		Get
			Return miQuadrant
		End Get
	End Property
	Public ReadOnly Property Tangent() As Double
		Get
			Return Math.Tan(mdAngleRadI)
		End Get
	End Property
	Public ReadOnly Property XDirection() As Boolean
		Get
			Return mdAngleDegI >= 45.0
		End Get
	End Property
	Public ReadOnly Property DirectTan() As Double
		Get
			If XDirection Then
				Return Math.Tan(0.5 * Math.PI - mdAngleRadI)
			Else
				Return Math.Tan(mdAngleRadI)
			End If
		End Get
	End Property
	Public ReadOnly Property DirectSec() As Double
		Get
			If XDirection Then
				Return 1 / Math.Cos(0.5 * Math.PI - mdAngleRadI)
			Else
				Return 1 / Math.Cos(mdAngleRadI)
			End If
		End Get
	End Property

	Public Function GetDirection() As Autodesk.AutoCAD.Geometry.Vector2d
		Dim dAngle As Double
		If miQuadrant = enQuadrants.QuadrantI Then
			dAngle = mdAngleRadI
		ElseIf miQuadrant = enQuadrants.QuadrantII Then
			dAngle = 0.5 * Math.PI - mdAngleRadI
		End If
		If dAngle > 45.0 Then
			Return New Autodesk.AutoCAD.Geometry.Vector2d(Math.Cos(dAngle) / Math.Sin(dAngle), 1.0)
		Else
			Return New Autodesk.AutoCAD.Geometry.Vector2d(1.0, Math.Sin(dAngle) / Math.Cos(dAngle))
		End If
	End Function
	Public Function GetPerpendicular() As LineAngle

		If miQuadrant = enQuadrants.QuadrantI Then
			Return New LineAngle(AngleDegree + 90.0, False)
		ElseIf miQuadrant = enQuadrants.QuadrantII Then
			Return New LineAngle(AngleDegree - 90.0, False)
		End If

	End Function
	Public Sub Mirror()
		If miQuadrant = enQuadrants.QuadrantI Then
			miQuadrant = enQuadrants.QuadrantII
			AngleDegree = 180.0 - AngleDegree
		ElseIf miQuadrant = enQuadrants.QuadrantII Then
			miQuadrant = enQuadrants.QuadrantI
			AngleDegree = 180.0 - AngleDegree
		End If
	End Sub
	Public Sub Mirror_020613()
		If miQuadrant = enQuadrants.QuadrantI Then
			miQuadrant = enQuadrants.QuadrantII
			AngleDegree = 180.0 - AngleDegree
		ElseIf miQuadrant = enQuadrants.QuadrantII Then
			miQuadrant = enQuadrants.QuadrantI
			AngleDegree = 180.0 - AngleDegree
		End If
	End Sub
	Public Sub Rotate(ByVal tAngle As LineAngle)
		Dim dAngleDegree As Double = (AngleDegree + tAngle.AngleDegree) Mod 180
		zzCalc(dAngleDegree)
	End Sub
	Private Sub zzCalc(ByVal dAngleDegree As Double)
		AngleDegree = dAngleDegree
		AngleRad = dAngleDegree * Math.PI / 180.0
		If dAngleDegree <= 90 Then
			miQuadrant = enQuadrants.QuadrantI
		Else
			dAngleDegree = 180 - dAngleDegree
			miQuadrant = enQuadrants.QuadrantII
		End If
		mdAngleDegI = dAngleDegree
		mdAngleRadI = dAngleDegree * Math.PI / 180.0
	End Sub
End Structure
Public Structure DMPatternLine
	Public Angle As LineAngle
	Public Origin As TPlnPoint
	Public DeltaX As Double
	Public DeltaY As Double
	Public HasDash As Boolean
	Public Descript As String
	
	Public Sub New(ByVal sDescript As String)
		Descript = sDescript
		Dim saValue() As String = Strings.Split(sDescript, ",")
		If saValue.GetUpperBound(0) >= 4 Then
			Try
				Dim dAngle As Double = Convert.ToDouble(saValue(0))
				Angle = New LineAngle(dAngle, False)
			Catch oEx As Exception
			End Try
			Try
				Dim dX As Double = Convert.ToDouble(saValue(1))
				Dim dY As Double = Convert.ToDouble(saValue(2))
				Origin = New TPlnPoint(dX, dY)
			Catch oEx As Exception
			End Try
			Try
				DeltaX = Convert.ToDouble(saValue(3))
				DeltaY = Convert.ToDouble(saValue(4))
			Catch oEx As Exception
			End Try
			If saValue.GetUpperBound(0) >= 6 Then
				HasDash = True
			End If
		Else
			AcadDocument.WriteMessage(CStr(saValue.GetUpperBound(0)) & "@@@" & sDescript)
		End If

	End Sub
	Public Function LineStep(ByVal dScale As Double) As Integer
		Return CInt(dScale * DeltaY)
	End Function
	Public Function GetShift(ByVal dScale As Double) As Integer
		Return CInt(dScale * Origin.X * Math.Cos(Angle.AngleRad))
	End Function
End Structure
Public Structure DMPattern
	Public Name As String
	Public Active As Boolean
	Private moaLines() As DMPatternLine
	Private mbHasDash As Boolean
	Private mbExists As Boolean
	
	Public Sub New(ByVal sName As String)
		Name = sName
		mbExists = True
      '  AcadDocument.WriteDebugMessage("!#Name=" & sName)
	End Sub
	Public Shared Operator =(tDMPatternA As DMPattern, tDMPatternB As DMPattern) As Boolean
		Return tDMPatternA.Name = tDMPatternB.Name
	End Operator
	Public Shared Operator <>(tDMPatternA As DMPattern, tDMPatternB As DMPattern) As Boolean
		Return Not (tDMPatternA = tDMPatternB)
	End Operator

	Public Sub AddLine(ByVal sDescript As String)
		Dim iNewUB As Integer = Me.LinesUB + 1
		ReDim Preserve moaLines(iNewUB)
		moaLines(iNewUB) = New DMPatternLine(sDescript)
      '	AcadDocument.WriteDebugMessage("#i=" & CStr(iNewUB) & ":" & CStr(moaLines(iNewUB).Origin Is Nothing) & "-" & moaLines(iNewUB).Descript)
		If Not mbHasDash AndAlso moaLines(iNewUB).HasDash Then
			mbHasDash = True
		End If
	End Sub
	
	Public ReadOnly Property LinesUB() As Integer
		Get
			If moaLines Is Nothing Then
				Return -1
			Else
				Return moaLines.GetUpperBound(0)
			End If
		End Get
	End Property
	Public ReadOnly Property Line(ByVal iIndex As Integer) As DMPatternLine
		Get
			Return moaLines(iIndex)
		End Get
	End Property
	Public ReadOnly Property HasDash() As Boolean
		Get
			Return mbHasDash
		End Get
	End Property
	Public ReadOnly Property Exists() As Boolean
		Get
			Return mbExists
		End Get
	End Property
	Public Overrides Function ToString() As String
		Return Name
	End Function
End Structure
Public Class DMPatterns
	Public Shared AcadPatPath As String = "C:\Program Files\TownPlanner\Support\acad.pat"
	Private Shared mdicPatterns As Dictionary(Of String, DMPattern)
	Public Shared Sub Init()
		zzInit()
	End Sub

	Private Shared Function zzInit() As Boolean
		If mdicPatterns Is Nothing Then
			mdicPatterns = New Dictionary(Of String, DMPattern)
			Dim oStreamReader As IO.StreamReader
			Dim sLine As String
			Dim tPattern As DMPattern = Nothing
			Dim saValues() As String
			Dim oFileInfo As IO.FileInfo = New IO.FileInfo(AcadPatPath)

			If oFileInfo.Exists Then
				Try
					oStreamReader = oFileInfo.OpenText()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show("Error File Reading'" & AcadPatPath & "'", "ColorScheme_03")
					Return False
				End Try
				If oStreamReader IsNot Nothing Then
					Do Until oStreamReader.EndOfStream
						sLine = oStreamReader.ReadLine()
						If Not String.IsNullOrEmpty(sLine) Then
							Select Case sLine.Substring(0, 1)
								Case ";"
								Case "*"
									If tPattern.Exists Then
										mdicPatterns.Add(tPattern.Name, tPattern)
									End If
									saValues = Split(sLine.Substring(1), ",")
									tPattern = New DMPattern(saValues(0))
								Case Else
									tPattern.AddLine(sLine)
							End Select
						End If
					Loop
					If tPattern.Exists Then

						mdicPatterns.Add(tPattern.Name, tPattern)
					End If
					oStreamReader.Close()

					Return True
				End If
			Else
				System.Windows.Forms.MessageBox.Show("File '" & AcadPatPath & "' was not found", "ColorScheme")
			End If
		End If
	End Function
	Public Shared ReadOnly Property PatternsExist As Boolean
		Get
			Return (mdicPatterns IsNot Nothing) AndAlso (mdicPatterns.Count > 0)
		End Get
	End Property
	Public Shared ReadOnly Property Count As Integer
		Get
			If (mdicPatterns IsNot Nothing) Then
				Return mdicPatterns.Count
			Else
				Return -1
			End If

		End Get
	End Property
	Public Shared Function Item(ByVal sName As String, ByRef tPattern As DMPattern) As Boolean
		zzInit()
		Return mdicPatterns.TryGetValue(sName, tPattern)
	End Function
	Public Shared Function GetAllNames() As String()

		zzInit()
		Dim saRes() As String = Nothing

		If mdicPatterns IsNot Nothing AndAlso mdicPatterns.Count <> 0 Then
			Dim oKeyCollection As System.Collections.Generic.Dictionary(Of String, DMPattern).KeyCollection = mdicPatterns.Keys
			ReDim saRes(mdicPatterns.Count - 1)
			Try
				oKeyCollection.CopyTo(saRes, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DMPaterns - GetAllNames")
			End Try

			Return saRes
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetNameR() As System.Object()
		Dim saRes() As String = Nothing
		mdicPatterns.Keys.CopyTo(saRes, 0)
		Return saRes
	End Function
End Class

