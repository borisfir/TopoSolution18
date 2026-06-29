Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Windows.Forms
Public Class ShrinkPolygon
	Inherits Polyline
	Private Structure VertexImage
		Public Direction As Vector2d
		Public Source As Point2d
		Public Proposed As Point2d
		Public PolylineIndex As Integer
		Public PreviousIndex As Integer
		Public NextIndex As Integer
		Public Removed As Boolean
		Public Sub CalcProposed(ByVal dDistance As Double, Optional ByVal bDebug As Boolean = False)
			Proposed = Source.Add(Direction.MultiplyBy(dDistance))
			If bDebug Then
				DMAcadExt.AcadDocument.WriteMessage("CPr " & CStr(PreviousIndex) & "<->" & CStr(NextIndex) & " " & zzDispPoint(Source) & ":" & zzDispPoint(Proposed))

			End If
		End Sub
		Public Function GetSegment() As LineSegment2d
			Return New LineSegment2d(Source, Proposed)
		End Function
		Public Function HasIntersection(ByVal tSegment As LineSegment2d) As Boolean
			Dim oMySegment As LineSegment2d = Me.GetSegment()
			Dim taResPoints() As Point2d = oMySegment.IntersectWith(tSegment)
			Return (taResPoints IsNot Nothing)
		End Function
	End Structure
	Private moAttemptPolygon As Polyline
	Private mtAcObjID As ObjectId
	Private mdStep As Double = 2.0

	'	Private moaBulgeVertex() As BulgeVertex
	Private miUB As Integer
	Private mdDestArea As Double
	Private mdPrevArea As Double

	Private miIndexA As Integer
	Private miIndexB As Integer
	Private miMovingUB As Integer
	Private miIndexPrevA As Integer
	Private miIndexNextB As Integer
	Private msLayerName As String
	Private mtBasePoint As Point2d
	Private moDirectrixA As Line2d
	Private moDirectrixB As Line2d
	Private mdMinSegment As Double
	Private miMinSegmentIndex As Integer = -1
	Private miMinSegmentAddIndex As Integer = -1

	Private mdTraversedA As Double
	Private mdTraversedB As Double


	Private moArcA As CircularArc2d
	Private moArcB As CircularArc2d


	Private mtMovingVectors() As Vector2d
	Private mtaVertexImages() As VertexImage
	Private mtaVertexImagesClone() As VertexImage

	Private Sub zzCopyVertexImages(ByVal taSourceVertexImages() As VertexImage, ByVal taDestVertexImages() As VertexImage)
		If taSourceVertexImages IsNot Nothing Then
			For iIndex As Integer = 0 To taSourceVertexImages.GetUpperBound(0)
				taDestVertexImages(iIndex) = taSourceVertexImages(iIndex)
			Next
		End If

	End Sub
	Private Sub zzGetVertexImagesClone()
		zzCopyVertexImages(mtaVertexImages, mtaVertexImagesClone)
	End Sub
	Private Sub zzSaveVertexImagesClone()
		zzCopyVertexImages(mtaVertexImagesClone, mtaVertexImages)
	End Sub
	Private Sub zzPrepare()

		DMAcadExt.AcadDocument.WriteMessage("#IndexA,B =" & CStr(miIndexA) & "->" & CStr(miIndexB))
		miIndexPrevA = zzGetPreviousIndex(miIndexA)
		miIndexNextB = zzGetNextIndex(miIndexB)
		DMAcadExt.AcadDocument.WriteMessage("#PrevA,NextB =" & CStr(miIndexPrevA) & ";" & CStr(miIndexNextB))
		DMAcadExt.AcadDocument.WriteMessage("NumberOfVert =" & CStr(Me.NumberOfVertices))
		'		moDirectrixA = zzGetLine(miIndexPrevA, miIndexA)
		'		moDirectrixB = zzGetLine(miIndexB, miIndexNextB)
		moArcA = zzGetArc(miIndexPrevA)
		moArcB = zzGetArc(miIndexB)

		'		Dim iDirUB As Integer = miIndexB - miIndexA3
		'		ReDim mtMovingVectors(iDirUB)
		mtMovingVectors(0) = zzGetNormVector(miIndexA, miIndexPrevA)
		mtaVertexImages(0).Direction = zzGetNormVector(miIndexA, miIndexPrevA)
		mtaVertexImages(0).Source = Me.GetPoint2dAt(miIndexA)
		mtaVertexImages(0).PreviousIndex = -1
		mtaVertexImages(0).NextIndex = 1

		mtMovingVectors(miMovingUB) = zzGetNormVector(miIndexB, miIndexNextB)
		mtaVertexImages(miMovingUB).Direction = zzGetNormVector(miIndexB, miIndexNextB, True)
		mtaVertexImages(miMovingUB).Source = Me.GetPoint2dAt(miIndexB)
		DMAcadExt.AcadDocument.WriteMessage("!!!!NB " & zzDispPoint(mtaVertexImages(miMovingUB).Source))
		mtaVertexImages(miMovingUB).PreviousIndex = miMovingUB - 1
		mtaVertexImages(miMovingUB).NextIndex = -1

		Dim dSegmentLenghtA As Double = zzGetVector(miIndexA, miIndexPrevA).Length
		Dim dSegmentLenghtB As Double = zzGetVector(miIndexB, miIndexNextB).Length

		If dSegmentLenghtA > dSegmentLenghtB Then
			mdMinSegment = dSegmentLenghtB
			miMinSegmentIndex = miIndexB
		ElseIf dSegmentLenghtB > dSegmentLenghtA Then
			mdMinSegment = dSegmentLenghtA
			miMinSegmentIndex = miIndexA
		Else
			mdMinSegment = dSegmentLenghtA
			miMinSegmentIndex = miIndexA
			miMinSegmentAddIndex = miIndexB
		End If
		DMAcadExt.AcadDocument.WriteMessage("SegmA=" & CStr(dSegmentLenghtA) & "; SegmB=" & CStr(dSegmentLenghtB))
	End Sub
	Private Function zzRemoveVertex(ByVal iIndex As Integer) As Integer
		'	Dim tCurrent As VertexImage = mtaVertexImagesClone(iIndex)
		'	Dim tPrev As VertexImage = mtaVertexImagesClone(tPrevIndex)
		DMAcadExt.AcadDocument.WriteMessageLog(CStr(iIndex) & "-Remove; All=" & CStr(mtaVertexImagesClone.GetUpperBound(0)))
		Dim tVertexImage As VertexImage
		Try
			tVertexImage = mtaVertexImagesClone(iIndex)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ShrinkPolygon - zzRemoveVertex")
		End Try
		Dim iPrevIndex As Integer = tVertexImage.PreviousIndex
		Dim iNextIndex As Integer = tVertexImage.NextIndex
		DMAcadExt.AcadDocument.WriteMessageLog("Ind:" & CStr(iPrevIndex) & "; Next=" & CStr(iNextIndex))
		Dim oPrevLine As Line2d
		Dim tCurrentStartPoint As Point2d = mtaVertexImagesClone(iIndex).Source
		Dim tCurrentEndPoint As Point2d = mtaVertexImagesClone(iNextIndex).Source
		Dim tPrevEndPoint As Point2d = mtaVertexImagesClone(iPrevIndex).Source	''''''''''''''''''''''
		Dim tPrevVector As Vector2d, tNextVector As Vector2d
		If iPrevIndex = 0 Then
			'	oPrevLine = New Line2d(tPrevEndPoint, mtaVertexImagesClone(tPrevIndex).Direction)
			mtaVertexImagesClone(iIndex).Removed = True
		Else
			Dim iPrevStartIndex As Integer = mtaVertexImagesClone(iPrevIndex).PreviousIndex
			DMAcadExt.AcadDocument.WriteMessageLog("prevStart:" & CStr(iPrevStartIndex) & "; Next=" & CStr(iNextIndex))
			Dim tPrevStartPoint As Point2d = mtaVertexImagesClone(iPrevStartIndex).Source	'''''''''''''''''''''
			oPrevLine = New Line2d(tPrevStartPoint, tPrevEndPoint)
			Dim oNextLine As Line2d = New Line2d(tCurrentStartPoint, tCurrentEndPoint)
			Dim taIntersectPoints() As Point2d = oPrevLine.IntersectWith(oNextLine)
			Dim tNewPoint As Point2d '

			If taIntersectPoints IsNot Nothing AndAlso taIntersectPoints.GetUpperBound(0) = 0 Then
				tNewPoint = taIntersectPoints(0)
				DMAcadExt.AcadDocument.WriteMessageLog(CStr(iIndex) & "-NewPoint = " & zzDispPoint(tNewPoint))
				DMAcadExt.AcadDocument.WriteMessageLog(CStr(iIndex) & "-Prev Next = " & zzDispPoint(tPrevStartPoint) & "; " & zzDispPoint(tCurrentEndPoint))
				tPrevVector = zzGetNormVector(tPrevStartPoint, tNewPoint)
				tNextVector = zzGetNormVector(tNewPoint, tCurrentEndPoint)
				mtaVertexImagesClone(iPrevIndex).Direction = zzCalcDirectOne(tPrevVector, tNextVector)
				mtaVertexImagesClone(iPrevIndex).NextIndex = iNextIndex
				mtaVertexImagesClone(iPrevIndex).Source = tNewPoint
				mtaVertexImagesClone(iIndex).Removed = True
				mtaVertexImagesClone(iNextIndex).PreviousIndex = iPrevIndex
			Else
				If taIntersectPoints Is Nothing Then
					MessageBox.Show(CStr(iIndex), "01_271")
				Else
					MessageBox.Show(CStr(taIntersectPoints.GetUpperBound(0)), "01_272")
				End If
			End If
		End If
	End Function
	Private Function zzCalcDirectOne(ByVal tPrevVector As Vector2d, ByVal tNextVector As Vector2d) As Vector2d
		Const dAngleToler As Double = 0.01
		Dim tCurrentVector As Vector2d
		Dim dAngleTo As Double
		dAngleTo = tNextVector.Angle - tPrevVector.Angle
		If Math.Abs(dAngleTo) < dAngleToler Then
			tCurrentVector = tPrevVector.GetPerpendicularVector()
		Else
			tCurrentVector = tNextVector.Subtract(tPrevVector)
			If (dAngleTo > -Math.PI AndAlso dAngleTo < 0.0) OrElse dAngleTo > Math.PI Then
				tCurrentVector = tCurrentVector.Negate
			Else
				tCurrentVector = tCurrentVector
			End If
		End If
		tCurrentVector = tCurrentVector.GetNormal()
		''''''Adition ????????
		Dim dAngle As Double = tCurrentVector.GetAngleTo(tPrevVector)
		tCurrentVector /= Math.Abs(Math.Sin(dAngle))
		Return tCurrentVector
	End Function
	Private Sub zzCalcDirect()
		Dim tPrevVector, tNextVector As Vector2d
		'	Dim tVertexImage As VertexImage

		tNextVector = zzGetNormVector(miIndexA, zzGetIndex(miIndexA + 1))
		For iIndex As Integer = 1 To miMovingUB - 1
			tPrevVector = tNextVector
			tNextVector = zzGetNormVector(zzGetIndex(miIndexA + iIndex), zzGetIndex(miIndexA + iIndex + 1))

			mtMovingVectors(iIndex) = zzCalcDirectOne(tPrevVector, tNextVector)
			With mtaVertexImages(iIndex)
				.Direction = zzCalcDirectOne(tPrevVector, tNextVector)
				.Source = Me.GetPoint2dAt(zzGetIndex(miIndexA + iIndex))
				.PreviousIndex = iIndex - 1
				.NextIndex = iIndex + 1
			End With
		Next
		'	MessageBox.Show(zzDispPoint(mtaVertexImages(10).Source) & ":" & zzDispPoint(mtaVertexImagesClone(10).Source), "01_240")
	End Sub
	Private Sub zzCalcDirectSource04102010()
		Const dAngleToler As Double = 0.01
		Dim tPrevVector, tCurrentVector, tNextVector As Vector2d
		Dim dAngleTo As Double
		tNextVector = zzGetNormVector(miIndexA, miIndexA + 1)
		For iIndex As Integer = 1 To miMovingUB - 1
			tPrevVector = tNextVector
			tNextVector = zzGetNormVector(miIndexA + iIndex, miIndexA + iIndex + 1)
			dAngleTo = tNextVector.Angle - tPrevVector.Angle
			If Math.Abs(dAngleTo) < dAngleToler Then
				tCurrentVector = tPrevVector.GetPerpendicularVector()
			Else
				tCurrentVector = tNextVector.Subtract(tPrevVector)
				If (dAngleTo > -Math.PI AndAlso dAngleTo < 0.0) OrElse dAngleTo > Math.PI Then
					tCurrentVector = tCurrentVector.Negate
				Else
					tCurrentVector = tCurrentVector
				End If
			End If
			mtMovingVectors(iIndex) = tCurrentVector.GetNormal()
			''''''Adition ????????
			Dim dAngle As Double = tCurrentVector.GetAngleTo(tPrevVector)
			mtMovingVectors(iIndex) /= Math.Abs(Math.Sin(dAngle))
		Next
	End Sub
	Public Sub ShrinkTo(ByVal dDestAreaDun As Double)
		Dim dAttemptArea, dAttemptAreaPrev As Double
		Dim dAttemptDist As Double
		Dim dAttemptDistPrev As Double = 0.0
		Dim bSegment As Boolean
		mdDestArea = 1000.0 * dDestAreaDun
		dAttemptAreaPrev = Me.Area
		Dim iTestA As Integer
		Do
			MessageBox.Show(CStr(mdMinSegment) & ":" & CStr(mdStep), "01_077")
			If mdMinSegment < mdStep Then
				bSegment = True
				dAttemptDist = mdMinSegment
			Else
				bSegment = False
				dAttemptDist = mdStep
			End If

			zzShrinkAttempt(dAttemptDist)
			MessageBox.Show(CStr(mdMinSegment) & ":" & CStr(mdStep), "01_078a")
			dAttemptArea = zzGetAttemptArea()
			zzDispAttemptPolygon()
			iTestA += 1
			If iTestA = 2 Then
				Return
			End If

			' test	ShrinkAttempt(2.0)


			DMAcadExt.AcadDocument.WriteMessage("$$IIIS:" & DispArea(dAttemptArea) & "!-!" & DispArea(Me.mdDestArea) & "!-!" & DispArea(0.0))

			If dAttemptArea > mdDestArea Then

				If bSegment Then
					zzSaveVertexImagesClone()
					zzReduceVertex()
				End If
				mdStep = dAttemptDistPrev + (dAttemptDist - dAttemptDistPrev) * (mdDestArea - dAttemptAreaPrev) / (dAttemptArea - dAttemptAreaPrev)
				dAttemptDistPrev = dAttemptDist
				dAttemptAreaPrev = dAttemptArea
			Else
				Exit Do
			End If
			Return
		Loop

		DMAcadExt.AcadDocument.WriteMessage("$$Count:" & CStr(Me.NumberOfVertices))

		Dim dDist As Double = dAttemptDist
		'	Dim dDelta As Double
		Dim dAreaPlus, dAreaMinus As Double
		Dim dDistPlus, dDistMinus As Double
		'Dim dNewArea As Double
		dAreaMinus = dAttemptArea
		dAreaPlus = dAttemptAreaPrev
		dDistPlus = 0.0
		dDistMinus = dAttemptDist
		'	MessageBox.Show(CStr(dAttemptAreaPrev) & vbCrLf & CStr(mdDestArea) & vbCrLf & CStr(dAttemptArea), "Area Dest Attempt")

		For iIndex As Integer = 0 To 10
			If Math.Abs(dAttemptArea - mdDestArea) < 0.1 Then Exit For
			dDist = dDistPlus + (dAreaPlus - mdDestArea) * Math.Abs(dDistMinus - dDistPlus) / Math.Abs(dAreaPlus - dAreaMinus)

			DMAcadExt.AcadDocument.WriteMessage("!MeArea=" & DispAcadArea(Me.Area) & " Prev=" & DispAcadArea(mdPrevArea) & " Dest=" & DispAcadArea(mdDestArea))
			DMAcadExt.AcadDocument.WriteMessage("!!!!!!" & DispAcadArea((dAreaPlus - mdDestArea)) & " : " & DispAcadArea((dDistMinus - dAreaPlus)) & " : " & DispAcadArea(dAreaPlus - dAreaMinus))
			zzShrinkAttempt(dDist)
			dAttemptArea = zzGetAttemptArea()

			DMAcadExt.AcadDocument.WriteMessage("$$SYS:" & DispArea(dAttemptArea) & " -! " & DispArea(Me.mdDestArea))
			DMAcadExt.AcadDocument.WriteMessage("Delta=" & zzDispDist(dAttemptArea - mdDestArea))



			If dAttemptArea > mdDestArea Then
				dAreaPlus = dAttemptArea
				dDistPlus = dDist
			Else
				dAreaMinus = dAttemptArea
				dDistMinus = dDist
			End If
		Next

		'	zzUpdatePolyline(Me, mtaVertexImagesClone)
		zzDispAttemptPolygon()
	End Sub
	Public Sub ShrinkTo05102010(ByVal dDestAreaDun As Double)
		mdDestArea = 1000.0 * dDestAreaDun
		Do
			zzShrinkSegment()
			DMAcadExt.AcadDocument.WriteMessage("$$IIIS:" & DispArea(Me.Area) & "!-!" & DispArea(Me.mdDestArea))
			If Me.Area > mdDestArea Then
				zzReduceVertex()
			Else
				Exit Do
			End If
		Loop

		Dim dDist As Double = mdMinSegment
		Dim dDelta As Double
		For iIndex As Integer = 0 To 20
			dDelta = (Me.Area - mdDestArea)
			DMAcadExt.AcadDocument.WriteMessage("$$SYS:" & DispArea(Me.Area) & "-!" & DispArea(Me.mdDestArea))
			DMAcadExt.AcadDocument.WriteMessage("Delta=" & zzDispDist(dDelta))
			If Math.Abs(dDelta) < 0.5 Then Exit For
			dDist = dDelta / Math.Abs((Me.Area - mdPrevArea)) * Math.Abs(dDist)
			DMAcadExt.AcadDocument.WriteMessage("!MeArea=" & DispAcadArea(Me.Area) & " Prev=" & DispAcadArea(mdPrevArea) & " Dest=" & DispAcadArea(mdDestArea))

			mdPrevArea = Me.Area
			ShrinkNew(dDist)
		Next
	End Sub
	Private Sub zzShrinkSegment()

		Shrink(mdMinSegment)

	End Sub
	Private Sub zzShrinkSegmentAttemptAAA()

		zzShrinkAttempt(mdMinSegment)

	End Sub
	Private Function zzAngleToSign(ByVal dAngle1 As Double, ByVal dAngle2 As Double) As Boolean
		Select Case dAngle2 - dAngle1
			Case Is < -180.0
				Return True
			Case -180.0 To 0.0
				Return False
			Case 0.0 To 180.0
				Return True
			Case Is > 180.0
				Return False
		End Select
	End Function
	Private Shared Function zzDispPoint(ByVal tPoint As Point2d) As String
		Return FormatNumber(tPoint.X, 2, TriState.False, TriState.False, TriState.False) & "," & FormatNumber(tPoint.Y, 2, TriState.False, TriState.False, TriState.False)
	End Function
	Private Function zzDispVector(ByVal tVector As Vector2d) As String
		Return FormatNumber(tVector.X, 2, TriState.False, TriState.False, TriState.False) & "," & FormatNumber(tVector.Y, 2, TriState.False, TriState.False, TriState.False)
	End Function

	Private Function zzDispAngle(ByVal dAngle As Double) As String
		Return FormatNumber(dAngle * 180 / Math.PI, 2)
	End Function
	Public Shared Function DispArea(ByVal dArea As Double) As String
		Return FormatNumber(dArea, 3, TriState.True)
	End Function
	Public Shared Function DispAcadArea(ByVal dArea As Double) As String
		Return FormatNumber(dArea * 0.001, 3)
	End Function
	Private Function zzDispDist(ByVal dDist As Double) As String
		Return FormatNumber(dDist, 3)
	End Function
	Private Function IndexToPgon(ByVal iIndex As Integer) As Integer
		If iIndex > miUB Then
			Return 0
		Else
			Return iIndex
		End If
	End Function
	Public Sub SetSegmentIndecis(ByVal iIndexA As Integer, ByVal iIndexB As Integer, ByVal sLayerName As String)
		''''''''''''''''	moSourcePolyLine = DirectCast(MyBase.Clone, Polyline)
		mdPrevArea = Me.Area
		miUB = Me.NumberOfVertices - 1
		miIndexA = IndexToPgon(iIndexA)
		miIndexB = IndexToPgon(iIndexB)
		miMovingUB = zzGetIndex(miIndexB - miIndexA)
		msLayerName = sLayerName
		'	System.Windows.Forms.MessageBox.Show(CStr(miUB) & vbCrLf & CStr(miIndexA) & "->" & CStr(miIndexB), "01_414")
		If Me.Exists Then
			ReDim mtMovingVectors(miMovingUB)
			ReDim mtaVertexImages(miMovingUB)
			ReDim mtaVertexImagesClone(miMovingUB)
			zzPrepare()
			zzCalcDirect()
		End If
	End Sub
	Public ReadOnly Property Exists() As Boolean
		Get
			Return miUB <> -1
		End Get
	End Property
	Public Sub Shrink_01(ByVal dDistance As Double)
		Dim tSourcePoint, tDestPoint As Point2d
		For iIndex As Integer = miIndexA To miIndexB
			tSourcePoint = MyBase.GetPoint2dAt(iIndex)
			tDestPoint = zzMoveFromBase(tSourcePoint, dDistance)
			MyBase.SetPointAt(iIndex, tDestPoint)
			If iIndex = miIndexA Then
				DMAcadExt.AcadDocument.WriteMessage(":  Dim=" & DMAcadExt.TPlnPoint.DispPoint(tSourcePoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(tDestPoint))
			End If
		Next
	End Sub
	Public Sub Shrink(ByVal dDistance As Double)
		Dim tSourcePoint, tDestPoint As Point2d

		DMAcadExt.AcadDocument.WriteMessage("Dist=" & zzDispDist(dDistance))
		'		DMAcadExt.AcadDocument.WriteMessage("!MeArea=" & DispArea(Me.Area) & " Prev=" & DispArea(mdPrevArea) & " Dest=" & DispArea(mdDestArea))
		For iIndex As Integer = miIndexA To miIndexB
			tSourcePoint = MyBase.GetPoint2dAt(iIndex)
			If iIndex = miIndexA AndAlso moArcA IsNot Nothing Then
				tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexPrevA), dDistance)
				'''''''''''''''''''	tDestPoint = zzGetArcPoint(tDestPoint, mtMovingVectors(iIndex - miIndexA))
			ElseIf iIndex = miIndexB AndAlso moArcB IsNot Nothing Then
				tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexNextB), dDistance)
			Else
				tDestPoint = tSourcePoint.Add(mtMovingVectors(iIndex - miIndexA).MultiplyBy(dDistance))
				tDestPoint = tSourcePoint.Add(mtaVertexImages(iIndex - miIndexA).Direction.MultiplyBy(dDistance))
			End If
			Me.SetPointAt(iIndex, tDestPoint)
			If iIndex = miIndexB - 1 Then
				DMAcadExt.AcadDocument.WriteMessage(":!!!Dim=" & DMAcadExt.TPlnPoint.DispPoint(tSourcePoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(tDestPoint))
			End If
		Next
	End Sub
	Private Sub zzShrinkAttempt(ByVal dDistance As Double)
		Dim tSourcePoint, tDestPoint As Point2d
		'	Dim tPrevVertexImage
		'	Dim iCurrentIndex As Integer
		Dim iPrevIndex As Integer
		Dim dDistanceC As Double
		Dim iIndex As Integer
		Try
			'DMAcadExt.AcadDocument.WriteMessageLog("Dist=" & zzDispDist(dDistance))
			zzGetVertexImagesClone()
			'DMAcadExt.AcadDocument.WriteMessageLog("VectorEnd=" & zzDispVector(mtaVertexImagesClone(mtaVertexImagesClone.GetUpperBound(0)).Direction))
			'	MessageBox.Show(zzDispPoint(mtaVertexImages(10).Source) & ":" & zzDispPoint(mtaVertexImagesClone(10).Source), "01_250")
			For iCurrentIndex As Integer = 0 To miMovingUB
				iIndex = (iCurrentIndex + miIndexA) Mod (miUB + 1)
				If Not mtaVertexImagesClone(iCurrentIndex).Removed Then
					moArcA = Nothing
					moArcB = Nothing
					If iIndex = miIndexA AndAlso moArcA IsNot Nothing Then

						tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexPrevA), dDistance)
						Me.SetPointAt(iIndex, tDestPoint)
						'''''''''''''''''''	tDestPoint = zzGetArcPoint(tDestPoint, mtMovingVectors(iIndex - miIndexA))
					ElseIf iIndex = miIndexB AndAlso moArcB IsNot Nothing Then
						tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexNextB), dDistance)
						Me.SetPointAt(iIndex, tDestPoint)
					Else
						If iIndex = miIndexA Then
							dDistanceC = dDistance - mdTraversedA
						ElseIf iIndex = miIndexB Then
							dDistanceC = dDistance - mdTraversedB
						Else
							dDistanceC = dDistance
						End If
						Do
							'DMAcadExt.AcadDocument.WriteMessageLog("iii " & CStr(iCurrentIndex) & " : " & CStr(iPrevIndex))
							mtaVertexImagesClone(iCurrentIndex).CalcProposed(dDistanceC, (iCurrentIndex >= miMovingUB - 1))
							iPrevIndex = mtaVertexImagesClone(iCurrentIndex).PreviousIndex
							If iPrevIndex >= 0 AndAlso iPrevIndex <= mtaVertexImagesClone.GetUpperBound(0) Then
								If mtaVertexImagesClone(iCurrentIndex).HasIntersection(mtaVertexImagesClone(iPrevIndex).GetSegment()) Then
									DMAcadExt.AcadDocument.WriteMessageLog("!RemVert| " & CStr(iCurrentIndex) & ": " & CStr(iPrevIndex) & ": " & zzDispPoint(mtaVertexImagesClone(iPrevIndex).Source) & ":" & zzDispPoint(mtaVertexImagesClone(iCurrentIndex).Source))
									zzRemoveVertex(iCurrentIndex)
									iCurrentIndex = iPrevIndex
								Else
									Exit Do
								End If
							Else
								Exit Do
							End If
						Loop
					End If

					If iIndex = miIndexB - 1 Then
						''''''''''''''	DMAcadExt.AcadDocument.WriteMessage(":!!!Dim=" & DMAcadExt.TPlnPoint.DispPoint(tSourcePoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(tDestPoint))
					End If
				End If
			Next

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ShrinkPolygon - ShrinkAttempt")
		End Try
	End Sub
	Public Sub ShrinkNew(ByVal dDistance As Double)
		Dim tSourcePoint, tDestPoint As Point2d
		Dim tPrevVertexImage, tCurrentVertexImage As VertexImage
		Dim iCurrentIndex As Integer
		Dim iPrevIndex As Integer
		DMAcadExt.AcadDocument.WriteMessage("Dist=" & zzDispDist(dDistance))
		'		DMAcadExt.AcadDocument.WriteMessage("!MeArea=" & DispArea(Me.Area) & " Prev=" & DispArea(mdPrevArea) & " Dest=" & DispArea(mdDestArea))
		For iIndex As Integer = miIndexA To miIndexB
			iCurrentIndex = iIndex - miIndexA
			tSourcePoint = MyBase.GetPoint2dAt(iIndex)
			tCurrentVertexImage = mtaVertexImages(iIndex)
			If iIndex = miIndexA AndAlso moArcA IsNot Nothing Then
				tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexPrevA), dDistance)
				Me.SetPointAt(iIndex, tDestPoint)
				'''''''''''''''''''	tDestPoint = zzGetArcPoint(tDestPoint, mtMovingVectors(iIndex - miIndexA))
			ElseIf iIndex = miIndexB AndAlso moArcB IsNot Nothing Then
				tDestPoint = zzGetArcPoint(tSourcePoint, MyBase.GetPoint2dAt(miIndexNextB), dDistance)
				Me.SetPointAt(iIndex, tDestPoint)
			Else
				Do
					tCurrentVertexImage = mtaVertexImages(iCurrentIndex)
					tCurrentVertexImage.CalcProposed(dDistance)
					iPrevIndex = tCurrentVertexImage.PreviousIndex
					If iPrevIndex >= 0 Then
						tPrevVertexImage = mtaVertexImages(iPrevIndex)
						If mtaVertexImages(iCurrentIndex).HasIntersection(tPrevVertexImage.GetSegment()) Then
							zzRemoveVertex(iCurrentIndex)
							iCurrentIndex = iPrevIndex
						Else
							Exit Do
						End If
					Else
						Exit Do
					End If
				Loop
			End If

			If iIndex = miIndexB - 1 Then
				DMAcadExt.AcadDocument.WriteMessage(":!!!Dim=" & DMAcadExt.TPlnPoint.DispPoint(tSourcePoint) & "; " & DMAcadExt.TPlnPoint.DispPoint(tDestPoint))
			End If
		Next
	End Sub
	Private Function zzGetCurrentArea() As Double
		Dim oTempPolyline As Polyline = DirectCast(MyBase.Clone, Polyline)
		zzUpdatePolyline(oTempPolyline, mtaVertexImages)
		Return oTempPolyline.Area
	End Function
	Private Function zzGetAttemptArea() As Double
		moAttemptPolygon = DirectCast(MyBase.Clone, Polyline)
		zzUpdatePolyline(moAttemptPolygon, mtaVertexImagesClone)
		Return moAttemptPolygon.Area
	End Function
	Private Sub zzDispAttemptPolygon()
		EraseDBPolygon()
		Try
			mtAcObjID = DMAcadExt.AcadTransaction.AppendEntity(moAttemptPolygon)
			'MessageBox.Show(mtAcObjID.ToString(), "01_000")
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			MessageBox.Show(oAcadEx.Message & ":" & CStr(miUB), "01_002")
		End Try
	End Sub

	Private Sub zzUpdatePolyline(ByVal oPolyline As Polyline, ByVal taVertexImages() As VertexImage)
		If oPolyline IsNot Nothing AndAlso taVertexImages IsNot Nothing Then
			For iIndex As Integer = taVertexImages.GetUpperBound(0) To 0 Step -1
				If taVertexImages(iIndex).Removed Then
					oPolyline.RemoveVertexAt((miIndexA + iIndex) Mod (miUB + 1))
				Else
					oPolyline.SetPointAt((miIndexA + iIndex) Mod (miUB + 1), taVertexImages(iIndex).Proposed)
				End If
			Next
		End If
	End Sub
	Private Sub zzUpdateMe()
		''''''''''	zzUpdatePolyline(Me)
	End Sub
	Private Sub zzUpdatePolyline()

		For iIndex As Integer = mtaVertexImages.GetUpperBound(0) To 0 Step -1
			If mtaVertexImages(iIndex).Removed Then
				Me.RemoveVertexAt((miIndexA + iIndex) Mod (miUB + 1))
			Else
				Me.SetPointAt((miIndexA + iIndex) Mod (miUB + 1), mtaVertexImages(iIndex).Proposed)
			End If
		Next
	End Sub
	Private Function zzGetArcPoint(ByVal oArc As CircularArc2d, ByVal dDistance As Double) As Point2d
		Dim dSegmentDist As Double = oArc.StartPoint.GetDistanceTo(oArc.EndPoint)
		Dim dDestAngle As Double = oArc.StartAngle + (oArc.EndAngle - oArc.StartAngle) * dDistance / dSegmentDist
		Dim dStartParam As Double = oArc.GetParameterOf(oArc.StartPoint)
		Dim dEndParam As Double = oArc.GetParameterOf(oArc.EndPoint)
		Dim dDestParam As Double = dStartParam + (dEndParam - dStartParam) * dDistance / dSegmentDist
		Return oArc.EvaluatePoint(dDestParam)
	End Function
	Private Function zzGetArcPoint(ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d, ByVal dDistance As Double) As Point2d
		Dim dSegmDist As Double = tStartPoint.GetDistanceTo(tEndPoint)
		If Math.Abs(dSegmDist) > 0.1 Then
			Dim dStartParam As Double = Me.GetParameterAtPoint(DMAcadExt.TPlnPoint.Point2dTo3d(tStartPoint))
			Dim dEndParam As Double = Me.GetParameterAtPoint(DMAcadExt.TPlnPoint.Point2dTo3d(tEndPoint))
			Dim dDestParam As Double = dStartParam + (dEndParam - dStartParam) * dDistance / dSegmDist
			DMAcadExt.AcadDocument.WriteMessage("Spar=" & CStr(dStartParam) & " Epar=" & CStr(dEndParam) & " Segm=" & CStr(dSegmDist) & " Dist=" & CStr(dDistance))
			DMAcadExt.AcadDocument.WriteMessage("!!Dpar=" & CStr(dDestParam))
			Return DMAcadExt.TPlnPoint.Point3dTo2d(Me.GetPointAtParameter(dDestParam))
		Else
			Return tEndPoint
		End If
	End Function
	Private Function zzGetArcPoint(ByVal tSourcePoint As Point2d, ByVal tDirectVector As Vector2d) As Point2d
		Dim tVector3D As Vector3d = New Vector3d(tDirectVector.X, tDirectVector.Y, 0.0)
		Return DMAcadExt.TPlnPoint.Point3dTo2d(MyBase.GetClosestPointTo(DMAcadExt.TPlnPoint.Point2dTo3d(tSourcePoint), tVector3D.GetPerpendicularVector(), False))
	End Function
	Public Sub EraseDBPolygon()
		If Not mtAcObjID.IsNull Then
			DMAcadExt.AcadTransaction.EraseDBObject(mtAcObjID)
		End If

	End Sub
	Public Sub AddToDatabase()
		Try
			Dim oPolygon As Polyline = DirectCast(MyBase.Clone, Polyline)
			oPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(127, 255, 255)
			mtAcObjID = DMAcadExt.AcadTransaction.AppendEntity(oPolygon)
			'	MessageBox.Show(mtAcObjID.ToString(), "01_000")
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			MessageBox.Show(oAcadEx.Message & ":" & CStr(miUB), "26_123")
		End Try

	End Sub
	Public Sub CreateMovingPolyline()
		Dim oPolyline As Polyline = New Polyline(miMovingUB + 1)

		If msLayerName IsNot Nothing AndAlso msLayerName.Length <> 0 Then
			oPolyline.Layer = msLayerName
		Else
			DMAcadExt.AcadDocument.WriteMessageLog("#109 Layer not exists")
		End If

		'	MessageBox.Show(CStr(oPolyline.NumberOfVertices) & ":" & CStr(VertexNum) & ":" & CStr(moaBulgeVertex.GetUpperBound(0)))
		'MessageBox.Show(CStr(moAttemptPolygon Is Nothing), "02_191")
		For iIndex As Integer = 0 To miMovingUB
			Try
				If iIndex >= moAttemptPolygon.NumberOfVertices Then
					MessageBox.Show(CStr(iIndex) & ":" & CStr(moAttemptPolygon.NumberOfVertices) & vbCrLf & CStr(miIndexA) & ":" & CStr(miIndexB), "26_459")
				End If

				'DMAcadExt.AcadDocument.WriteMessageLog("4_400 miUB=" & CStr(miUB) & " b=" & CStr(moaBulgeVertex(iIndex).Bulge) & ":" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
				oPolyline.AddVertexAt(iIndex, moAttemptPolygon.GetPoint2dAt(zzGetIndex(iIndex + miIndexA)), moAttemptPolygon.GetBulgeAt(zzGetIndex(iIndex + miIndexA)), 0.0, 0.0)
				'oPolyline.SetPointAt(iIndex + 1, moaBulgeVertex(iIndex).Vertex)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				MessageBox.Show(oAcadEx.Message & vbCrLf & CStr(iIndex) & ":" & CStr(miUB), "26_778")
			End Try
		Next

		Try
			DMAcadExt.AcadTransaction.AppendEntity(DirectCast(oPolyline, Polyline))
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			MessageBox.Show(oAcadEx.Message, "26_781")

		End Try

	End Sub

	Public Sub CreateMovingPolyline07102010()
		Dim oPolyline As Polyline = New Polyline(miIndexB - miIndexA + 1)

		'	oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255)
		oPolyline.Layer = "UD_PCLP001"
		'	MessageBox.Show(CStr(oPolyline.NumberOfVertices) & ":" & CStr(VertexNum) & ":" & CStr(moaBulgeVertex.GetUpperBound(0)))
		For iIndex As Integer = miIndexA To miIndexB
			Try
				If iIndex >= Me.NumberOfVertices Then
					MessageBox.Show(CStr(iIndex) & ":" & CStr(Me.NumberOfVertices) & vbCrLf & CStr(miIndexA) & ":" & CStr(miIndexB), "26_459")
				End If
				'DMAcadExt.AcadDocument.WriteMessageLog("4_400 miUB=" & CStr(miUB) & " b=" & CStr(moaBulgeVertex(iIndex).Bulge) & ":" & DMAcadExt.TPlnPoint.DispPoint(moaBulgeVertex(iIndex).Vertex))
				oPolyline.AddVertexAt(iIndex - miIndexA, Me.GetPoint2dAt(iIndex), Me.GetBulgeAt(iIndex), 0.0, 0.0)
				'oPolyline.SetPointAt(iIndex + 1, moaBulgeVertex(iIndex).Vertex)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				MessageBox.Show(oAcadEx.Message & vbCrLf & CStr(iIndex) & ":" & CStr(miUB), "26_778")
			End Try
		Next

		DMAcadExt.AcadTransaction.AppendEntity(DirectCast(oPolyline, Polyline))
	End Sub
	Private Sub zzReduceVertex()
		'	MessageBox.Show(CStr(Me.NumberOfVertices) & ":" & CStr(miMinSegmentIndex), "01_088")
		DMAcadExt.AcadDocument.WriteMessage("#PointReduced = " & DMAcadExt.TPlnPoint.DispPoint(MyBase.GetPoint2dAt(miMinSegmentIndex)))

		MyBase.RemoveVertexAt(miMinSegmentIndex)
		miUB = Me.NumberOfVertices - 1
		'	MessageBox.Show(CStr(Me.NumberOfVertices) & ":" & CStr(miIndexA) & ":" & CStr(miIndexB), "01_089")
		If miMinSegmentIndex <= miIndexA Then
			miIndexA = zzGetPreviousIndex(miIndexA)
			mdTraversedA += mdMinSegment
		End If
		If miMinSegmentIndex <= miIndexB Then
			miIndexB = zzGetPreviousIndex(miIndexB)
			mdTraversedB += mdMinSegment
			''''''''''''''''''''''''''''	miIndexB = zzGetNextIndex(miIndexB)
		End If
		'''''''''''''''''''		moSourcePolyLine = DirectCast(MyBase.Clone, Polyline)
		zzPrepare()
		If miMinSegmentAddIndex <> -1 Then
			Try
				MyBase.RemoveVertexAt(miMinSegmentAddIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "ShrinkPolygon - zzReduceVertex")
			End Try
		End If
	End Sub
	Private Sub zzCalcDirection()

	End Sub
	Private Function zzGetLineLength(ByVal oLine2d As Line2d) As Double
		Dim oInterval As Interval
		Try
			oInterval = oLine2d.GetInterval()
			Return oInterval.Length
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "ShrinkPolygon - zzGetLineLength")
			Return 0.0
		End Try
	End Function
	Private Function zzMoveFromBase(ByVal tPoint As Point2d, ByVal dDistance As Double) As Point2d
		Dim tVector As Vector2d = New Vector2d(tPoint.X - mtBasePoint.X, tPoint.Y - mtBasePoint.Y)
		tVector = tVector.MultiplyBy(dDistance / tVector.Length)
		Return tPoint.Add(tVector)

	End Function
	Private Function zzGetBasePoint(ByVal iFrom As Integer, ByVal iTo As Integer) As Point2d
		Dim oStartLine As Line2d = zzGetLine(zzGetPreviousIndex(iFrom), iFrom)
		Dim oEndLine As Line2d = zzGetLine(iTo, zzGetNextIndex(iTo))
		Return oStartLine.IntersectWith(oEndLine)(0)
	End Function
	Private Function zzGetPreviousIndex(ByVal iIndex As Integer) As Integer
		If iIndex = 0 Then
			Return miUB - 1
		Else
			Return iIndex - 1
		End If
	End Function
	Private Function zzGetNextIndex(ByVal iIndex As Integer) As Integer
		'	System.Windows.Forms.MessageBox.Show(CStr(miUB) & ":" & CStr(iIndex), "01_415")
		If iIndex = miUB Then
			Return 1
		Else
			Return iIndex + 1
		End If
	End Function
	Private Function zzGetIndex(ByVal iIndex As Integer) As Integer
		If iIndex < 0 Then
			Return iIndex + 1 + miUB
		ElseIf iIndex > miUB Then
			Return iIndex - 1 - miUB
		Else
			Return iIndex
		End If

	End Function
	Private Function zzGetArc(ByVal iIndex As Integer) As CircularArc2d
		If iIndex < Me.NumberOfVertices Then
			If Me.GetBulgeAt(iIndex) <> 0.0 Then
				Return Me.GetArcSegment2dAt(iIndex)
			Else
				Return Nothing
			End If
		Else
			System.Windows.Forms.MessageBox.Show("Index " & CStr(iIndex) & " >= " & CStr(Me.NumberOfVertices), "01_370")
			Return Nothing
		End If

	End Function
	Private Function zzGetLine(ByVal iIndex1 As Integer, ByVal iIndex2 As Integer) As Line2d
		Try
			Dim tPoint1, tPoint2 As Point2d
			tPoint1 = Me.GetPoint2dAt(iIndex1)
			tPoint2 = Me.GetPoint2dAt(iIndex2)
			''DMAcadExt.AcadDocument.WriteMessage("#Point1:Point2 = " & DMAcadExt.TPlnPoint.DispPoint(tPoint1) & ":" & DMAcadExt.TPlnPoint.DispPoint(tPoint2))
			Return New Line2d(tPoint1, tPoint2)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(iIndex1) & ":" & CStr(iIndex2) & ":" & CStr(Me.NumberOfVertices), "ShrinkPolygon - zzGetLine")
			Return Nothing
		End Try
	End Function
	Private Function zzGetVector(ByVal iIndex1 As Integer, ByVal iIndex2 As Integer) As Vector2d
		Try
			Dim tPoint1, tPoint2 As Point2d
			tPoint1 = Me.GetPoint2dAt(iIndex1)
			tPoint2 = Me.GetPoint2dAt(iIndex2)
			'DMAcadExt.AcadDocument.WriteMessage("#Point1:Point2 = " & DMAcadExt.TPlnPoint.DispPoint(tPoint1) & ":" & DMAcadExt.TPlnPoint.DispPoint(tPoint2))
			Return New Vector2d(tPoint2.X - tPoint1.X, tPoint2.Y - tPoint1.Y)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ShrinkPolygon - zzGetVector")
			Return Nothing
		End Try
	End Function
	Private Function zzGetNormVector(ByVal tPoint1 As Point2d, ByVal tPoint2 As Point2d) As Vector2d
		Try
			Dim tResVector As Vector2d
			tResVector = New Vector2d(tPoint2.X - tPoint1.X, tPoint2.Y - tPoint1.Y)
			Return tResVector.GetNormal()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ShrinkPolygon - zzGetNormVector_2")
			Return Nothing
		End Try
	End Function


	Private Function zzGetNormVector(ByVal iIndex1 As Integer, ByVal iIndex2 As Integer, Optional ByVal bDebug As Boolean = False) As Vector2d
		Try
			Dim tPoint1, tPoint2 As Point2d
			Dim tResVector As Vector2d
			tPoint1 = Me.GetPoint2dAt(iIndex1)
			tPoint2 = Me.GetPoint2dAt(iIndex2)
			If bDebug Then
				DMAcadExt.AcadDocument.WriteMessage("#Point1:Point2 = " & DMAcadExt.TPlnPoint.DispPoint(tPoint1) & ":" & DMAcadExt.TPlnPoint.DispPoint(tPoint2))
			End If
			tResVector = New Vector2d(tPoint2.X - tPoint1.X, tPoint2.Y - tPoint1.Y)
			Return tResVector.GetNormal()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "Ind=" & CStr(iIndex1) & "," & CStr(iIndex2) & "All=" & CStr(Me.NumberOfVertices), "ShrinkPolygon - zzGetNormVector")
			Return Nothing
		End Try
	End Function

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
