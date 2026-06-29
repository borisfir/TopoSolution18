Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
'Imports Autodesk.AutoCAD.GraphicsInterface
Public Enum enLineJigStatus
	[Default]
	Bulge

End Enum
Friend Class PLineJig
	Inherits EntityJig
	Private miPromptCounter As Integer
	Private m_dims As DynamicDimensionDataCollection
	Private moStartPoint, moNextPoint, moBulgePoint As DMAcadExt.TPlnPoint
	Private mdBulge As Double
	Private miJigStatus As enLineJigStatus
	Sub New(ByVal oStartPoint As DMAcadExt.TPlnPoint)
		MyBase.New(New Polyline(2))
		moStartPoint = oStartPoint
		moNextPoint = moStartPoint
		Dim oPolyline As Polyline = Me.GetEntity()
		oPolyline.AddVertexAt(0, moStartPoint.AcGePoint(), 0.0, 0.0, 0.0)
		zzAddVertex()
	End Sub
	Public Function GetEntity() As Polyline
		Return DirectCast(MyBase.Entity, Polyline)
	End Function
	Public Property JigStatus() As enLineJigStatus
		Get
			Return miJigStatus
		End Get
		Set(ByVal iValue As enLineJigStatus)
			miJigStatus = iValue
		End Set
	End Property
	Private Sub zzAddVertex()
		Dim oPolyline As Polyline = Me.GetEntity()
		Dim iVertNum As Integer = oPolyline.NumberOfVertices()
		oPolyline.AddVertexAt(iVertNum, moNextPoint.AcGePoint(), 0.0, 0.0, 0.0)
		'	moStartPoint = moNextPoint
	End Sub
	Public Sub SetBulge()
		Dim oPolyline As Polyline = Me.GetEntity()
		Dim iVertNum As Integer = oPolyline.NumberOfVertices()
		Try
			oPolyline.SetBulgeAt(iVertNum - 2, mdBulge)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

		End Try

	End Sub
	Public Sub ClearBulge()
		Dim oPolyline As Polyline = Me.GetEntity()
		mdBulge = 0.0
		Try
			oPolyline.SetBulgeAt(0, mdBulge)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

		End Try

	End Sub
	Protected Overrides Function Sampler(ByVal oJigPrompts As Autodesk.AutoCAD.EditorInput.JigPrompts) As Autodesk.AutoCAD.EditorInput.SamplerStatus
		Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions()
		oJigPointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)

		oJigPointOpts.Message = vbCrLf & "Specify next point: "
		'	oJigPointOpts.Cursor = CursorType.Invisible
		Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
		Select Case miJigStatus
			Case enLineJigStatus.Default
				Dim oNextPointTemp As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oResPoint.Value)
				If (oNextPointTemp <> moNextPoint) Then
					moNextPoint = oNextPointTemp
				Else
					Return SamplerStatus.NoChange
				End If
				If (oResPoint.Status = PromptStatus.Cancel) Then
					Return SamplerStatus.Cancel
				Else
					Return SamplerStatus.OK
				End If
			Case enLineJigStatus.Bulge
				Dim oBulgePointTemp As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oResPoint.Value)
				If (oBulgePointTemp <> moBulgePoint) Then
					moBulgePoint = oBulgePointTemp
				Else
					Return SamplerStatus.NoChange
				End If
				If (oResPoint.Status = PromptStatus.Cancel) Then
					Return SamplerStatus.Cancel
				Else
					Return SamplerStatus.OK
				End If
			Case Else
				Return Nothing

		End Select

	End Function
	Protected Overrides Function Update() As Boolean
		Dim oPolyline As Polyline = Me.GetEntity()
		Dim iVertNum As Integer = oPolyline.NumberOfVertices()

		Try

			Select Case miJigStatus
				Case enLineJigStatus.Default
					oPolyline.SetPointAt(iVertNum - 1, moNextPoint.AcGePoint())
				Case enLineJigStatus.Bulge

					Dim tLastPoint, tPrevPoint As Point2d
					tLastPoint = oPolyline.GetPoint2dAt(iVertNum - 1)	'iVertNum - 1
					tPrevPoint = oPolyline.GetPoint2dAt(iVertNum - 2)	 'iVertNum - 2


					Dim dDia As Double = tPrevPoint.GetDistanceTo(tLastPoint)
					Dim oRay As Ray2d = New Ray2d(tPrevPoint, tLastPoint)
					Dim tAcadPoint As Point2d = moBulgePoint.AcGePoint()
					Dim dHeight As Double = oRay.GetDistanceTo(tAcadPoint)
					Dim tVector As Vector2d = New Vector2d(tAcadPoint.X - tPrevPoint.X, tAcadPoint.Y - tPrevPoint.Y)
					 

					mdBulge = 2 * dHeight / dDia
					If mdBulge > 127 Then
						mdBulge = 127
					ElseIf mdBulge < -127 Then
						mdBulge = -127
					End If
					Dim tRayDir As Vector2d = oRay.Direction
					If tVector.X * tRayDir.Y - tVector.Y * tRayDir.X < 0 Then
						mdBulge = -mdBulge
					End If
					'		ed.WriteMessage(vbNewLine & CStr(iVertNum - 1) & "- Angle: " & oRay.Direction.GetAngleTo(tVector).ToString() & "- Bulge: " & mdBulge.ToString())
					'	oPolyline.SetBulgeAt(iVertNum - 2, mdBulge)
					oPolyline.SetBulgeAt(0, mdBulge)
					'	oPolyline.SetBulgeAt(1, mdBulge)
			End Select

		Catch oEx As System.Exception
			Return False
		End Try
		'zzUpdateDimensions()

		Return True

	End Function
End Class

