Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology

'Imports Autodesk.AutoCAD.GraphicsInterface
Public Enum enPgonJigStatus
	SelectPgon
	SelectNeighborPgon
	GetShrinkValue
End Enum
Public Class PgonJig
	Inherits EntityJig
	Private miPromptCounter As Integer
	Private m_dims As DynamicDimensionDataCollection
	Private moStartPoint, moNextPoint, moBulgePoint As DMAcadExt.TPlnPoint
	Private mtCurrentPoint As Point3d
	Private mdBulge As Double
	Private miJigStatus As enPgonJigStatus = enPgonJigStatus.SelectPgon
	Private moTopology As TopologyModel
	'	Private moPolygon As Polygon
	Private miPreviousPgonID As Integer = 0
	Private miCurrentPgonID As Integer = 0
	Private moParcel As UD_Parcel
	Private moNeighborParcel As UD_Parcel
	Private miIndexFrom As Integer, miIndexTo As Integer
	Private mcolAcObjIds As ObjectIdCollection
	Private miNeighborID As Integer
	Private msLayerName As String
	Sub New(ByVal oTopology As TopologyModel)
      MyBase.New(New Polyline())
		moTopology = oTopology
		moStartPoint = New DMAcadExt.TPlnPoint(0.0, 0.0)
		moNextPoint = moStartPoint
		Dim oPolyline As Polyline = Me.GetEntity()
		oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
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
	Protected Overrides Function GetDynamicDimensionData(ByVal dimScale As Double) As DynamicDimensionDataCollection
		Return m_dims
	End Function

	Protected Overrides Sub OnDimensionValueChanged(ByVal e As Autodesk.AutoCAD.DatabaseServices.DynamicDimensionChangedEventArgs)

	End Sub
	Sub Terminate()
		Dim oPolyline As Polyline
	 

		oPolyline = Me.GetEntity()
		oPolyline.Dispose()
	End Sub
	Public Function GetEntity() As Polyline
		Return DirectCast(MyBase.Entity, Polyline)
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
	Public Property Parcel() As UD_Parcel
		Get
			Return moParcel
		End Get
		Set(ByVal oValue As UD_Parcel)
			moParcel = oValue
		End Set
	End Property
	Public ReadOnly Property NeighborParcel() As UD_Parcel
		Get
			Return moNeighborParcel
		End Get
	End Property
	Public ReadOnly Property IndexFrom() As Integer
		Get
			Return miIndexFrom
		End Get
	End Property
	Public ReadOnly Property IndexTo() As Integer
		Get
			Return miIndexTo
		End Get
	End Property
	Public ReadOnly Property LineCollection() As ObjectIdCollection
		Get
			Return mcolAcObjIds
		End Get
	End Property
	Public ReadOnly Property LayerName() As String
		Get
			Return msLayerName
		End Get
	End Property


	Private Sub zzAddVertex()
		Dim oPolyline As Polyline = Me.GetEntity()
		Dim iVertNum As Integer = oPolyline.NumberOfVertices()
		oPolyline.AddVertexAt(iVertNum, moNextPoint.AcGePoint(), 0.0, 0.0, 0.0)
		'	moStartPoint = moNextPoint
	End Sub

	Protected Overrides Function Sampler(ByVal oJigPrompts As Autodesk.AutoCAD.EditorInput.JigPrompts) As Autodesk.AutoCAD.EditorInput.SamplerStatus
		Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions()
		oJigPointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)

		oJigPointOpts.Cursor = CursorType.Crosshair
		Select Case miJigStatus
			Case enPgonJigStatus.SelectPgon
				oJigPointOpts.Message = "Specify Polygon: "
			Case enPgonJigStatus.SelectNeighborPgon
				oJigPointOpts.Message = "Specify 2-nd Polygon: "

		End Select
		'	System.Windows.Forms.MessageBox.Show(" ", "01_012")
		Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
		'	System.Windows.Forms.MessageBox.Show(CStr(oResPoint Is Nothing), "01_013")
		If oResPoint IsNot Nothing Then
			mtCurrentPoint = oResPoint.Value
			Dim oPolygon As Polygon = Nothing
			'	DMAcadExt.AcadDocument.WriteMessage(oResPoint.Status.ToString())
			Select Case miJigStatus
				Case enPgonJigStatus.SelectPgon

					'System.Windows.Forms.MessageBox.Show(CStr(moTopology Is Nothing), "01_013a")

					Try
						'	System.Windows.Forms.MessageBox.Show("", "01_013b")
						oPolygon = moTopology.FindPolygon(mtCurrentPoint)
					Catch oMapEx As Autodesk.Gis.Map.MapException
                  ' System.Windows.Forms.MessageBox.Show(mtCurrentPoint.ToString(), "01_014")
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

					'System.Windows.Forms.MessageBox.Show("", "01_015")

				Case enPgonJigStatus.SelectNeighborPgon
					Try
						Dim sTest As String = "a"
						'	System.Windows.Forms.MessageBox.Show("", "01_013b")
						oPolygon = moTopology.FindPolygon(mtCurrentPoint)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'System.Windows.Forms.MessageBox.Show("", "01_014s")
					End Try
					'	Dim iPgonID As Integer = 0
					If oPolygon IsNot Nothing Then
						miCurrentPgonID = oPolygon.ID
						oPolygon.Dispose()
					Else
						miCurrentPgonID = 0
					End If
					If miCurrentPgonID = miPreviousPgonID Then
						Return SamplerStatus.NoChange
					Else
						If miCurrentPgonID = 0 OrElse miCurrentPgonID = moParcel.TopoID OrElse (Not moParcel.IsNeighbor(miCurrentPgonID)) Then
							miNeighborID = 0
						Else
							miNeighborID = miCurrentPgonID
						End If
						miPreviousPgonID = miCurrentPgonID
						DMAcadExt.AcadDocument.WriteDebugMessage("SM__OK-" & CStr(miPreviousPgonID) & "&&" & CStr(miCurrentPgonID))
						Return SamplerStatus.OK
					End If
			End Select
		Else
			Return SamplerStatus.NoChange
		End If
	End Function
	Protected Overrides Function Update() As Boolean
		Dim oPolyline, oNewPolyline As Polyline
		Dim sTest1 As String = "a"
		'Dim oParcel As UD_Parcel = Nothing
		'	System.Windows.Forms.MessageBox.Show("", "01_016")
		oPolyline = Me.GetEntity()
		'		DMAcadExt.AcadDocument.WriteMessage("2: " & miJigStatus.ToString())
		Try

			Select Case miJigStatus
				Case enPgonJigStatus.SelectPgon
					If miCurrentPgonID <> 0 Then
						If Unidiv.TryGetParcel(miCurrentPgonID, moParcel) Then
							sTest1 = "a1"
							'''''''''''''''''''''''System.Windows.Forms.MessageBox.Show(CStr(moParcel Is Nothing) & ":" & CStr(moPolygon.ID), "01_013a")
							oNewPolyline = moParcel.GetBoundary()

							zzCopyPolyline(oNewPolyline, oPolyline)
							oPolyline.Visible = True
							'System.Windows.Forms.MessageBox.Show(CStr(oPolyline Is Nothing), "01_018")
						End If

						miPreviousPgonID = miCurrentPgonID
					Else
						miPreviousPgonID = 0
						oPolyline.Visible = False
					End If
					'oPolyline.SetPointAt(iVertNum - 1, moNextPoint.AcGePoint())
				Case enPgonJigStatus.SelectNeighborPgon
					Dim sTest As String = "Pgon Neigh=" & miNeighborID.ToString() & "; "
					If moParcel Is Nothing Then
						sTest &= "moParcel Is Nothing"
					Else
						sTest &= moParcel.Name
					End If
					'DMAcadExt.AcadDocument.WriteMessage(sTest)
					If miNeighborID <> 0 Then
						sTest1 = "b1"
						If moParcel.IsNeighbor(miNeighborID) Then
							oNewPolyline = moParcel.GetPlinesBoundary(miNeighborID, 0, miIndexFrom, miIndexTo, mcolAcObjIds, msLayerName)
							'System.Windows.Forms.MessageBox.Show(CStr(moParcel.OutsideVertexUB) & vbCrLf & CStr(miPreviousPgonID) & "&&" & CStr(miCurrentPgonID) & vbCrLf & CStr(miIndexFrom) & "->" & CStr(miIndexTo), "01_411a")
							sTest1 = "b2"
							DMAcadExt.AcadDocument.WriteMessage(msLayerName, "02_111")
							If Not Unidiv.TryGetParcel(miNeighborID, moNeighborParcel) Then
								'	DMAcadExt.AcadDocument.WriteMessage("!!!  moNeighborParcel Not Found " & CStr(miNeighborID))
							ElseIf moNeighborParcel Is Nothing Then
								'	DMAcadExt.AcadDocument.WriteMessage("!!!  moNeighborParcel  Found But Nothing" & CStr(miNeighborID))
							Else
								'	DMAcadExt.AcadDocument.WriteMessage("!!!  moNeighborParcel OK " & CStr(miNeighborID) & ":" & moNeighborParcel.Name)
							End If


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
						End If
						'	DMAcadExt.AcadDocument.WriteMessage(sTest)
					Else
						oPolyline.Visible = False
						'System.Windows.Forms.MessageBox.Show("", "01_700")
					End If

					miPreviousPgonID = miCurrentPgonID
			End Select

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & miJigStatus.ToString() & vbCrLf & CStr(moParcel Is Nothing) & vbCrLf & sTest1, "01_016a")
			Return False
		End Try
		zzUpdateDimensions()
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
		If moNeighborParcel IsNot Nothing Then
			Dim dTolerance As Double = moNeighborParcel.Tolerance
			Dim dDelta = moNeighborParcel.DeltaArea
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

			tCenterPoint = moNeighborParcel.CentroidPoint3d





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
		End If

	End Sub
End Class

