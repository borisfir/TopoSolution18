Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Public Class ShrinkPgonJig
	Inherits EntityJig
	Private miPromptCounter As Integer
	Private m_dims As DynamicDimensionDataCollection
	Private moStartPoint, moNextPoint, moBulgePoint As DMAcadExt.TPlnPoint
	Private mtCurrentPoint As Point3d
	Private mdBulge As Double
	Private miJigStatus As enPgonJigStatus = enPgonJigStatus.SelectPgon
	Private moTopology As TopologyModel
	'Private moPolygon As Polygon
	Private miPreviousPgonID As Integer = 0
	Private miCurrentPgonID As Integer = 0

	Private moParcel As UD_Parcel
	Private moNeighborParcel As UD_Parcel
	Private miIndexFrom As Integer, miIndexTo As Integer
	Private miNeighborID As Integer
	Private msResulttString As String = ""
	Private moPromptResult As Autodesk.AutoCAD.EditorInput.PromptResult
	Private mcolAcObjIds As ObjectIdCollection
	Private msLayerName As String
	Private miCounter As Integer
	Private miUpdateCounter As Integer

	'DMAcadExt.AcadDocument.WriteMessage("###031 ")
	Sub New(ByVal oTopology As TopologyModel)
		MyBase.New(New ShrinkPolygon())
		moTopology = oTopology
		moStartPoint = New DMAcadExt.TPlnPoint(0.0, 0.0)
		moNextPoint = moStartPoint
		Dim oPolyline As Polyline = Me.GetEntity()
		oPolyline.Linetype = "HIDDEN"
		oPolyline.LinetypeScale = 0.001
		'	oPolyline.AddVertexAt(0, moStartPoint.AcGePoint(), 0.0, 0.0, 0.0)
		'	zzAddVertex()
	
		mtCurrentPoint = moStartPoint.AcGePoint3d
		zzInitDims()
	End Sub
	Private Sub zzInitDims()
		'	Return
		m_dims = New DynamicDimensionDataCollection()
		Dim dim0 As Dimension = New AlignedDimension()
		dim0.SetDatabaseDefaults()
		dim0.DynamicDimension = True

		m_dims.Add(New DynamicDimensionData(dim0, False, False))
		'	Dim dim1 As Dimension = New AlignedDimension()
		'	dim1.SetDatabaseDefaults()
		'	dim1.DynamicDimension = True
		'	m_dims.Add(New DynamicDimensionData(dim1, True, False))
	End Sub
	Sub Terminate()
		Dim oPolyline As Polyline

		oPolyline = Me.GetEntity()
		oPolyline.Dispose()
	
	End Sub

	Public Function GetEntity() As ShrinkPolygon
		Return DirectCast(MyBase.Entity, ShrinkPolygon)
	End Function

	Public Property JigStatus() As enPgonJigStatus
		Get
			Return miJigStatus
		End Get
		Set(ByVal iValue As enPgonJigStatus)
			miJigStatus = iValue
			miPreviousPgonID = 0
		End Set
	End Property
	Public ReadOnly Property Parcel() As UD_Parcel
		Get
			Return moParcel
		End Get
	End Property
	


	Public ReadOnly Property ResulttString() As String
		Get
			Return msResulttString
		End Get
	End Property
	Public ReadOnly Property PromptResult() As Autodesk.AutoCAD.EditorInput.PromptResult
		Get
			Return moPromptResult
		End Get
	End Property
	Public ReadOnly Property Area() As Double
		Get
			Dim oShrinkPolygon As ShrinkPolygon = DirectCast(MyBase.Entity, ShrinkPolygon)
			Return oShrinkPolygon.Area
		End Get
	End Property
	Public ReadOnly Property LayerName() As String
		Get
			Return msLayerName
		End Get
	End Property
	Public Sub SetSegmentIndecis(ByVal iIndexA As Integer, ByVal iIndexB As Integer, ByVal sLayerName As String)
		Dim oShrinkPolygon As ShrinkPolygon = DirectCast(MyBase.Entity, ShrinkPolygon)
		oShrinkPolygon.SetSegmentIndecis(iIndexA, iIndexB, sLayerName)
	End Sub
	Public Sub Shrink(ByVal dDistance As Double)
		Dim oShrinkPolygon As ShrinkPolygon = DirectCast(MyBase.Entity, ShrinkPolygon)
		oShrinkPolygon.Shrink(dDistance)
	End Sub
	Private Sub zzAddVertex()
		Dim oPolyline As Polyline = Me.GetEntity()
		Dim iVertNum As Integer = oPolyline.NumberOfVertices()
		oPolyline.AddVertexAt(iVertNum, moNextPoint.AcGePoint(), 0.0, 0.0, 0.0)
		'	moStartPoint = moNextPoint
	End Sub


	Protected Overrides Function Sampler(ByVal oJigPrompts As Autodesk.AutoCAD.EditorInput.JigPrompts) As Autodesk.AutoCAD.EditorInput.SamplerStatus
		Dim oPolygon As Polygon = Nothing
		Dim sMsg As String = "Specify Polygon!!!: "
		Select Case miJigStatus
			Case enPgonJigStatus.SelectPgon
				Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions()
				'	oJigPointOpts.UserInputControls = UserInputControls.   (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)
				oJigPointOpts.UserInputControls = UserInputControls.AcceptOtherInputString
				oJigPointOpts.Cursor = CursorType.Crosshair
				
				Select Case miJigStatus
					Case enPgonJigStatus.SelectPgon
						oJigPointOpts.Message = sMsg
					Case enPgonJigStatus.SelectNeighborPgon
						oJigPointOpts.Message = "----NOTSpecify 2-nd Polygon: "
				End Select
				'	System.Windows.Forms.MessageBox.Show(" ", "01_012")
				Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
				If oResPoint IsNot Nothing Then
					mtCurrentPoint = oResPoint.Value
					Try
						oPolygon = moTopology.FindPolygon(mtCurrentPoint)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'	System.Windows.Forms.MessageBox.Show("", "01_014")
					End Try

					If oPolygon IsNot Nothing Then
						miCurrentPgonID = oPolygon.ID
						oPolygon.Dispose()
					End If
					If miCurrentPgonID = miPreviousPgonID Then
						Return SamplerStatus.NoChange
					Else

						Return SamplerStatus.OK
					End If
				End If
				'System.Windows.Forms.MessageBox.Show("", "01_015")

			Case enPgonJigStatus.SelectNeighborPgon
				If False Then
					Try
						oPolygon = moTopology.FindPolygon(mtCurrentPoint)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'	System.Windows.Forms.MessageBox.Show("", "01_014")
					End Try
					Dim iPgonID As Integer = 0
					If oPolygon IsNot Nothing Then
						iPgonID = oPolygon.ID
						oPolygon.Dispose()
					End If
					If iPgonID = miPreviousPgonID Then
						Return SamplerStatus.NoChange
					Else
						If iPgonID = 0 OrElse iPgonID = moParcel.TopoID OrElse (Not moParcel.IsNeighbor(iPgonID)) Then
							miNeighborID = 0
						Else
							miNeighborID = iPgonID
						End If
						miPreviousPgonID = iPgonID
						Return SamplerStatus.OK
					End If
				End If
				
				Return SamplerStatus.Cancel

			Case enPgonJigStatus.GetShrinkValue
				Dim oJigPromptStringOptions As JigPromptStringOptions = Nothing
				oJigPromptStringOptions = New JigPromptStringOptions(vbCrLf & "[Arc/Halfwidth/Length/Undo/Width]:")
				oJigPromptStringOptions = New JigPromptStringOptions()
				oJigPromptStringOptions.Cursor = CursorType.TargetBox
				oJigPromptStringOptions.UserInputControls = UserInputControls.DoNotUpdateLastPoint Or UserInputControls.AcceptOtherInputString Or UserInputControls.AnyBlankTerminatesInput Or UserInputControls.NoZeroResponseAccepted Or UserInputControls.NoNegativeResponseAccepted

				moPromptResult = oJigPrompts.AcquireString(oJigPromptStringOptions)

				msResulttString = moPromptResult.StringResult

				If msResulttString.Length <> 0 OrElse moPromptResult.ToString() <> "(None,)" Then
					DMAcadExt.AcadDocument.WriteMessage("Sampler: " & moPromptResult.Status.ToString() & ":" & msResulttString & ":" & moPromptResult.ToString())
				End If
				'	DMAcadExt.AcadDocument.WriteMessage(oPromptResult.Status.ToString())
				If moPromptResult.Status = PromptStatus.OK Then
					msResulttString = moPromptResult.StringResult
					'	DMAcadExt.AcadDocument.WriteMessage("Sampler: " & msPromptString)

					Return SamplerStatus.OK
				Else
					miCounter += 1
					If miCounter = 1000 OrElse msResulttString.Length <> 0 Then
						miCounter = 0
						'DMAcadExt.AcadDocument.WriteMessage("SamplerStatus.OK")
						Return SamplerStatus.OK
					Else
						Return SamplerStatus.NoChange
					End If
				End If
		End Select
	End Function
	Protected Overrides Function GetDynamicDimensionData(ByVal dimScale As Double) As DynamicDimensionDataCollection
		Return m_dims
	End Function

	Protected Overrides Sub OnDimensionValueChanged(ByVal e As Autodesk.AutoCAD.DatabaseServices.DynamicDimensionChangedEventArgs)

	End Sub
	Protected Overrides Function Update() As Boolean
		Dim oPolyline, oNewPolyline As Polyline
		'Dim oParcel As UD_Parcel = Nothing
		Dim sTest1 As String = "x"
		oPolyline = Me.GetEntity()
		sTest1 = "y"
		'	DMAcadExt.AcadDocument.WriteMessage(miJigStatus.ToString())
		Try

			Select Case miJigStatus
				Case enPgonJigStatus.SelectPgon
					If miCurrentPgonID <> 0 Then
						If Unidiv.TryGetParcel(miCurrentPgonID, moParcel) Then
							sTest1 = "a1"
							'''''''''''''''''''''''System.Windows.Forms.MessageBox.Show(CStr(moParcel Is Nothing) & ":" & CStr(moPolygon.ID), "01_013a")
							oNewPolyline = moParcel.GetBoundary()
							sTest1 = "a2"
							If oNewPolyline IsNot Nothing Then
								oNewPolyline.ColorIndex = 5
								sTest1 = "a3"
								zzCopyPolyline(oNewPolyline, oPolyline)
								sTest1 = "a4"
								oPolyline.Visible = True
							End If
							'	System.Windows.Forms.MessageBox.Show(CStr(oNewPolyline.NumberOfVertices) & ":" & CStr(oPolyline.NumberOfVertices), "01_401")
						End If
						If miPreviousPgonID <> miCurrentPgonID Then
							zzUpdateDimensions()
							miPreviousPgonID = miCurrentPgonID
						End If
						
					Else
						miPreviousPgonID = 0
						oPolyline.Visible = False
					End If

					'oPolyline.SetPointAt(iVertNum - 1, moNextPoint.AcGePoint())
				Case enPgonJigStatus.SelectNeighborPgon
					Dim sTest As String = "ShrP Neigh=" & miNeighborID.ToString()
					DMAcadExt.AcadDocument.WriteMessage(vbCrLf & "!!!SelectNeighborPgon " & miNeighborID.ToString())
					If miNeighborID <> 0 Then
						sTest1 = "b1"
						oNewPolyline = moParcel.GetPlinesBoundary(miNeighborID, 0, miIndexFrom, miIndexTo, mcolAcObjIds, msLayerName)
						sTest1 = "b2"
						System.Windows.Forms.MessageBox.Show(msLayerName, "02_110")
						If oNewPolyline IsNot Nothing Then
							sTest &= ": " & CStr(oNewPolyline Is Nothing)
							If oNewPolyline IsNot Nothing Then
								sTest &= " Cnt= " & oNewPolyline.NumberOfVertices
							Else
								sTest &= " ???? "
							End If
							sTest1 = "b3"
							zzCopyPolyline(oNewPolyline, oPolyline)
							sTest1 = "b4"
							oPolyline.Visible = True

						End If
					Else
						oPolyline.Visible = False
					End If
					'	DMAcadExt.AcadDocument.WriteMessage(sTest)
				Case enPgonJigStatus.GetShrinkValue
					DMAcadExt.AcadDocument.WriteMessage(vbCrLf & "!!!GetShrinkValue " & miNeighborID.ToString())
					sTest1 = "c1"
					oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 64)
					sTest1 = "c2"
					If msResulttString IsNot Nothing AndAlso msResulttString.Length <> 0 Then
						DMAcadExt.AcadDocument.WriteMessage(vbCrLf & "!!!x" & msResulttString)
						Dim dDist As Double
						If IsNumeric(msResulttString) Then
							dDist = Convert.ToDouble(msResulttString)
							''''''''''''''Me.Shrink(dDist)
						End If
					End If
			End Select

		Catch oEx As System.Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest1, "01_016b")
			Return False
		End Try

		Return True
	End Function
	Private Sub zzCopyPolyline(ByVal oSourcePolyline As Polyline, ByRef oDestPolyline As Polyline)
		Dim dBulge As Double
		Dim tPoint As Point2d
		Dim iPrevVertNum As Integer = oDestPolyline.NumberOfVertices
		For iIndex As Integer = 0 To oSourcePolyline.NumberOfVertices - 1
			dBulge = oSourcePolyline.GetBulgeAt(iIndex)
			tPoint = oSourcePolyline.GetPoint2dAt(iIndex)
			If iIndex < iPrevVertNum Then
				oDestPolyline.SetBulgeAt(iIndex, dBulge)
				oDestPolyline.SetPointAt(iIndex, tPoint)
			Else
				Try
					oDestPolyline.AddVertexAt(iIndex, tPoint, dBulge, 0.0, 0.0)
				Catch oEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & CStr(iIndex) & ":" & CStr(oDestPolyline.NumberOfVertices), "zzCopyPolyline_1")
				End Try

			End If
		Next
		For iIndex As Integer = iPrevVertNum - 1 To oSourcePolyline.NumberOfVertices Step -1
			Try
				oDestPolyline.RemoveVertexAt(iIndex)
			Catch oEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & CStr(iIndex) & ":" & CStr(oDestPolyline.NumberOfVertices), "zzCopyPolyline_2")
			End Try
		Next
		oDestPolyline.Closed = oSourcePolyline.Closed
	End Sub
	Private Sub zzUpdateDimensions()
		Dim sMsg0 As String = ""
		Dim sMsg1 As String = ""
		Dim tCenterPoint As Point3d
		Dim sPrefix As String
		If moParcel IsNot Nothing Then
			Dim dTolerance As Double = moParcel.Tolerance
			Dim dDelta = -moParcel.DeltaArea
			If dDelta < 0.0 AndAlso dDelta < -dTolerance Then
				dDelta += dTolerance
				sPrefix = "-T "
			ElseIf dDelta > 0.0 AndAlso dDelta > dTolerance Then
				dDelta -= dTolerance
				sPrefix = "+T +"
			Else
				sPrefix = String.Empty
			End If

			sMsg0 = "T=" & UnidivNet.ShrinkPolygon.DispArea(dTolerance) & "; d=" & sPrefix & UnidivNet.ShrinkPolygon.DispArea(dDelta)


			sMsg1 = ""

			tCenterPoint = moParcel.CentroidPoint3d
		End If




		Dim dimen0 As AlignedDimension = DirectCast(m_dims.Item(0).Dimension, AlignedDimension)
		dimen0.XLine1Point = tCenterPoint	'mtCurrentPoint
		dimen0.XLine2Point = tCenterPoint	'New Point3d(mtCurrentPoint.X + 300, mtCurrentPoint.Y, mtCurrentPoint.Z)
		dimen0.DimLinePoint = tCenterPoint
		'dimen.IsModifie = False
		dimen0.DimensionText = sMsg0


		'	Dim dimen1 As AlignedDimension = DirectCast(m_dims.Item(1).Dimension, AlignedDimension)
		'	dimen1.XLine1Point = tCenterPoint	'mtCurrentPoint
		'	dimen1.XLine2Point = tCenterPoint	'New Point3d(mtCurrentPoint.X + 300, mtCurrentPoint.Y, mtCurrentPoint.Z)
		'	dimen1.DimLinePoint = New Point3d(tCenterPoint.X, tCenterPoint.Y + 40, 0.0)
		'
		'	dimen1.DimensionText = sMsg1
		'	dimen.TextPosition = mtCurrentPoint

		
	End Sub
End Class
