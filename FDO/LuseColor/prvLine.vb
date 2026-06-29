Option Explicit On
Option Strict On
Public Class prvLine
	Enum enAxis
		enAxisX
		enAxisY
	End Enum
	Enum enSidea
		enMin
		enMax
	End Enum
	Enum enPaintType
		enTypeStrip
		enTypeHairLine
		enTypeBorder
	End Enum
	Private mdAngle As Double
	Private mdTangent As Double
	Private mdKfWidth As Double
	Private mlBoxWidth As Double
	Private mlBoxHeight As Double
	Private mlColor As Integer
	Private mlColorDefault As Integer
	Private mlLineWeight As Integer
	Private mlBasePoint As Integer
	Private mlAxis As enAxis
	Private mlBaseSide As Integer	'enSide
	Private mlElementCount As Integer
	Private mlType As enPaintType
	Private mlStep As Integer
	Private mlaColor() As Integer
	Private mlaStripWidth() As Integer
	Private mdaDashLength() As Double
	Private moPictureBox As dmPictureBox
	Private moGraph As System.Drawing.Graphics
	Private mlShift As Integer
	Private mlStart As Integer
	Private mlEnd As Integer
	Dim mdPi As Object
	Dim mlX1 As Integer
	Dim mlX2 As Integer
	Dim mlY1 As Integer
	Dim mlY2 As Integer


	Public Property Angle() As Double
		Get
			Angle = mdAngle
		End Get
		Set(ByVal Value As Double)
			On Error GoTo ErrorHandler

			'UPGRADE_WARNING: Mod has a new behavior. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
			mdAngle = (Value Mod 180.0#)
			If mdAngle < 0 Then mdAngle = mdAngle + 180.0#
			'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			mdAngle = mdAngle * Math.PI / 180.0#
			'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			If mdAngle < System.Math.Atan(mlBoxHeight / mlBoxWidth) Or mdAngle > Math.PI - System.Math.Atan(mlBoxHeight / mlBoxWidth) Then
				mdTangent = System.Math.Tan(mdAngle)
				mdKfWidth = 1.0# / System.Math.Abs(System.Math.Cos(mdAngle))
				mlAxis = enAxis.enAxisY
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleLeft was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlX1 = CInt(moPictureBox.ScaleLeft)
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleLeft was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlX2 = CInt(moPictureBox.ScaleLeft + mlBoxWidth) - 1
				mlShift = CInt(mdTangent * mlBoxWidth)
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleTop was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlStart = CInt(moPictureBox.ScaleTop)
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleTop was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlEnd = CInt(moPictureBox.ScaleTop + mlBoxHeight) - 1
			Else
				'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				mdTangent = System.Math.Tan(Math.PI * 0.5 - mdAngle)
				'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				mdKfWidth = 1.0# / System.Math.Cos(Math.PI * 0.5 - mdAngle)
				mlAxis = enAxis.enAxisX
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleTop was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlY1 = CInt(moPictureBox.ScaleTop)
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleTop was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlY2 = CInt(moPictureBox.ScaleTop + mlBoxHeight) - 1
				mlShift = CInt(mdTangent * mlBoxHeight)
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleLeft was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlStart = moPictureBox.ScaleLeft
				'UPGRADE_ISSUE: PictureBox property moPictureBox.ScaleLeft was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				mlEnd = CInt(moPictureBox.ScaleLeft + mlBoxWidth) - 1
			End If
			'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			If mdAngle > Math.PI * 0.5 And mdAngle < Math.PI * 1.5 Then
				mlStart = mlStart + mlShift
			Else
				mlEnd = mlEnd + mlShift
			End If
ExitProcedure:
			Exit Property
ErrorHandler:
			MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - Angle")
			Resume ExitProcedure
		End Set
	End Property

	Public ReadOnly Property PaintType() As enPaintType
		Get
			PaintType = mlType
		End Get
	End Property




	Public Property Box() As dmPictureBox
		Get
			Box = moPictureBox
		End Get
		Set(ByVal Value As dmPictureBox)
			moPictureBox = Value
			mlBoxHeight = VB6.PixelsToTwipsY(moPictureBox.ClientRectangle.Height) '+ 1
			mlBoxWidth = VB6.PixelsToTwipsX(moPictureBox.ClientRectangle.Width) '+ 1

		End Set
	End Property

	'UPGRADE_NOTE: Step was upgraded to Step_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Property Step_Renamed() As Integer
		Get
			Step_Renamed = mlStep
		End Get
		Set(ByVal Value As Integer)
			mlStep = Value
			mlType = enPaintType.enTypeHairLine
		End Set
	End Property


	Public Property FillColor() As Integer
		Get
			FillColor = System.Drawing.ColorTranslator.ToOle(moPictureBox.BackColor)
		End Get
		Set(ByVal Value As Integer)
			moPictureBox.BackColor = System.Drawing.ColorTranslator.FromOle(Value)
		End Set
	End Property


	Public Property LineColor() As Integer
		Get
			LineColor = mlColor
		End Get
		Set(ByVal Value As Integer)
			mlColor = Value
		End Set
	End Property

	Public Property LineWeight() As Integer
		Get
			LineWeight = mlLineWeight
		End Get
		Set(ByVal Value As Integer)
			mlLineWeight = Value
		End Set
	End Property




	Public Property ColorDefault() As Integer
		Get
			ColorDefault = mlColorDefault
		End Get
		Set(ByVal Value As Integer)
			mlColorDefault = Value
		End Set
	End Property
	Public Sub AddStrip(ByVal lColor As Integer, ByVal lWidth As Integer)
		On Error GoTo ErrorHandler
		If mlElementCount = 0 Then
			ReDim mlaColor(mlElementCount)
			ReDim mlaStripWidth(mlElementCount)
		Else
			ReDim Preserve mlaColor(mlElementCount)
			ReDim Preserve mlaStripWidth(mlElementCount)
		End If
		mlaColor(mlElementCount) = lColor
		mlaStripWidth(mlElementCount) = lWidth
		mlElementCount = mlElementCount + 1
		mlType = enPaintType.enTypeStrip
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - AddStrip")
		Resume ExitProcedure
	End Sub

	Public Sub AddDash(ByVal lLength As Integer)
		On Error GoTo ErrorHandler
		If mlElementCount = 0 Then
			ReDim mdaDashLength(mlElementCount)
		Else
			ReDim Preserve mdaDashLength(mlElementCount)
		End If
		mdaDashLength(mlElementCount) = lLength
		mlElementCount = mlElementCount + 1
		mlType = enPaintType.enTypeHairLine
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - AddDash")
		Resume ExitProcedure
	End Sub

	'UPGRADE_NOTE: Reset was upgraded to Reset_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub Reset_Renamed()
		mlElementCount = 0
	End Sub

	Public Sub Paint(ByVal lType As enPaintType)
		If lType = enPaintType.enTypeHairLine Then
			If mlElementCount = 0 Then
				zzPaintHairLine()
			Else
				zzPaintPattern()
			End If
		ElseIf lType = enPaintType.enTypeStrip Then
			zzPaintStrip()
		ElseIf lType = enPaintType.enTypeBorder Then
			zzPaintBorder()
		End If
	End Sub

	'UPGRADE_NOTE: Class_Initialize was upgraded to Class_Initialize_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		'UPGRADE_WARNING: Couldn't resolve default property of object mdPi. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		mdPi = System.Math.Atan(1.0#) * 4
		mlStep = 1
		mlColorDefault = QBColor(15)
	End Sub



	Private Sub zzPaintHairLine()
		Dim lStepNo As Integer
		Dim ldX, ldY As Integer
		Dim lIndex As Integer
		On Error GoTo ErrorHandler
		For lStepNo = mlStart To mlEnd Step CInt(mlStep * mdKfWidth)
			If mlAxis = enAxis.enAxisX Then
				mlX1 = lStepNo
				mlX2 = mlX1 - mlShift
				ldX = 1
			Else
				mlY1 = lStepNo
				mlY2 = mlY1 - mlShift
				ldY = 1
			End If
			For lIndex = 0 To mlLineWeight
				'UPGRADE_ISSUE: PictureBox method moPictureBox.Line was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				moPictureBox.Line (mlX1 + lIndex * ldX, mlY1 + lIndex * ldY) - (mlX2 + lIndex * ldX, mlY2 + lIndex * ldY), mlColor
			Next

		Next

ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - PaintHairLine")
		Resume ExitProcedure
	End Sub
	Private Sub zzPaintDashLine(ByVal lX1 As Integer, ByVal lY1 As Integer, ByVal lX2 As Integer, ByVal lY2 As Integer, Optional ByVal lColor As Object = -1)
		Dim lDashIndex As Integer
		Dim lStepX, lNextX As Integer
		Dim lStepY, lNextY As Integer
		Dim dLenth As Double
		Dim plColor As Integer
		Dim dDeltaX As Double
		Dim dDeltaY As Double
		Dim dLineLenth As Double
		On Error GoTo ErrorHandler
		'UPGRADE_WARNING: Couldn't resolve default property of object lColor. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		plColor = IIf(lColor >= 0, lColor, mlColor)
		'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentX was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
		moPictureBox.CurrentX = lX1
		'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentY was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
		moPictureBox.CurrentY = lY1
		dLineLenth = System.Math.Sqrt((lX2 - lX1) * (lX2 - lX1) + (lY2 - lY1) * (lY2 - lY1))
		dDeltaX = (lX2 - lX1) / dLineLenth
		dDeltaY = (lY2 - lY1) / dLineLenth
		Do
			dLenth = dLenth + System.Math.Abs(mdaDashLength(lDashIndex))
			lNextX = lX1 + dDeltaX * System.Math.Abs(dLenth)
			lNextY = lX2 + dDeltaY * System.Math.Abs(dLenth)
			If mdaDashLength(lDashIndex) > 0 Then
				'UPGRADE_ISSUE: PictureBox method moPictureBox.Line was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				moPictureBox.Line (lNextX, lNextY), plColor
			ElseIf dLenth = 0 Then
				'UPGRADE_ISSUE: PictureBox method moPictureBox.PSet was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				moPictureBox.PSet (lNextX, lNextY), plColor
			Else
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentX was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				moPictureBox.CurrentX = lNextX
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentY was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				moPictureBox.CurrentY = lNextY
			End If
			lDashIndex = (lDashIndex + 1) Mod mlElementCount
			If mlAxis = enAxis.enAxisX Then
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentX was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				If lX2 - lX1 > 0 And moPictureBox.CurrentX >= lX2 Then Exit Do
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentX was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				If lX2 - lX1 < 0 And moPictureBox.CurrentX <= lX2 Then Exit Do
			Else
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentY was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				If lStepY > 0 And moPictureBox.CurrentY >= lY2 Then Exit Do
				'UPGRADE_ISSUE: PictureBox property moPictureBox.CurrentY was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
				If lStepY < 0 And moPictureBox.CurrentY <= lY2 Then Exit Do
			End If

		Loop
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - PaintDashLine")
		Resume ExitProcedure
	End Sub

	Private Sub zzPaintStrip()
		Dim lLineIndex As Integer
		Dim lColorIndex As Integer
		Dim lBoxIndex As Integer
		Dim lFlag As Boolean

		On Error GoTo ErrorHandler
		lLineIndex = -1
		If mlElementCount = 0 Then Exit Sub
		For lBoxIndex = mlStart To mlEnd
			lLineIndex = lLineIndex + 1
			If lLineIndex >= CInt(mdKfWidth * mlaStripWidth(lColorIndex)) Then
				lLineIndex = 0
				lColorIndex = lColorIndex + 1
				If lColorIndex = mlElementCount Then lColorIndex = 0
			End If
			mlColor = mlaColor(lColorIndex)
			If mlAxis = enAxis.enAxisX Then
				mlX1 = lBoxIndex
				mlX2 = mlX1 - mlShift
			Else
				mlY1 = lBoxIndex
				mlY2 = mlY1 - mlShift
			End If
			lFlag = True
			'UPGRADE_ISSUE: PictureBox method moPictureBox.Line was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
			If lFlag Then moPictureBox.Line (mlX1, mlY1) - (mlX2, mlY2), mlColor
		Next
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - PaintStrip")
		Resume ExitProcedure
	End Sub

	Private Sub zzPaintPattern()
		Dim lStepNo As Integer
		For lStepNo = mlStart To mlEnd Step mlStep
			If mlAxis = enAxis.enAxisX Then
				mlX1 = lStepNo
				mlX2 = mlX1 - mlShift
			Else
				mlY1 = lStepNo
				mlY2 = mlY1 - mlShift
			End If
			' moPictureBox.Line (mlX1, mlY1)-(mlX2, mlY2), 255
			zzPaintDashLine(mlX1, mlY1, mlX2, mlY2)

			' Debug.Print mlX1, mlY1, mlX2, mlY2
		Next
	End Sub


	Private Sub zzPaintBorder()
		Dim lLineIndex As Integer
		Dim lColorIndex As Integer
		Dim lBoxIndex As Integer

		On Error GoTo ErrorHandler
		For lColorIndex = 0 To mlElementCount - 1
			For lLineIndex = 0 To mlaStripWidth(lColorIndex)
				zzPaintBorderLine(lBoxIndex, mlaColor(lColorIndex))
				lBoxIndex = lBoxIndex + 1
			Next
		Next
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - PaintBorder")
		Resume ExitProcedure
	End Sub

	Private Sub zzPaintBorderLine(ByVal lStep As Integer, ByVal lColor As Integer)
		On Error GoTo ErrorHandler
		'UPGRADE_ISSUE: PictureBox method moPictureBox.Line was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
		moPictureBox.Line (lStep, lStep) - (mlBoxWidth - lStep - 1, mlBoxHeight - lStep - 1), lColor, B
ExitProcedure:
		Exit Sub
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, MsgBoxStyle.Critical, My.Application.Info.Title & " - PaintBorderLine")
		Resume ExitProcedure
	End Sub

	'UPGRADE_NOTE: Class_Terminate was upgraded to Class_Terminate_Renamed. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Terminate_Renamed()
		'UPGRADE_NOTE: Object moPictureBox may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		moPictureBox.Image = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub



	Public Sub Clear()
		'UPGRADE_ISSUE: PictureBox method moPictureBox.Cls was not upgraded. Click for more: 'ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
		moPictureBox.Cls()
		moPictureBox.BackColor = System.Drawing.ColorTranslator.FromOle(mlColorDefault)
	End Sub

End Class
