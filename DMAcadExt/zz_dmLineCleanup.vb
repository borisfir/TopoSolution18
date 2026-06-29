Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.Gis
Imports Autodesk.Gis.Map
'Imports Autodesk.Gis.Map.Topology
Imports System.Windows.Forms
Public Class zz_dmLineCleanup
	Const msXDataAppName As String = "TplnSourceHandles"
	Const miErrClassNo As Integer = 13000 '  iExNo   = 10
	Private Shared mdicEntityCells As EntityCells
	Private Shared moaCurveSegments() As TplnCurveSegment
	Private Shared mdicLines As SegmentDic
	Private Shared miDuplicateLines As Integer = 0
	Public Shared Sub BuildOverlay(ByVal oTopoDef As TopoDef)
		Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
		Dim oCurve As Curve
		Dim saLayers() As String = TopoDef.ValueSplit(oTopoDef.LinkLayers)
		Dim sCurveLayerName As String
		Dim oTopoDefByLayer As TopoDef = Nothing
		Dim bOverlay As Boolean
		Dim sDestLayer As String
		Dim sRXClassName As String
		Dim oLine, oNewLine As Line
		Dim oArc, oNewArc As Arc
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim iArcCounter As Integer = 0, iLineCounter As Integer = 0, iPolylineCounter As Integer = 0, iPolyline2dCounter As Integer = 0
		AcadTransaction.CreateLayer(saLayers, 1, True)
		For Each tObjID As ObjectId In dicModelSpaceObjIDs
			oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)
			If oCurve IsNot Nothing Then
				sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
				bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
				If oTopoDefByLayer IsNot Nothing Then
					sDestLayer = oTopoDef.GetDoubleLayer(oCurve.Layer, False)
					If sDestLayer IsNot Nothing Then
						sRXClassName = oCurve.GetRXClass().Name
						Select Case sRXClassName
							Case AcadConst.AcadLineName
								oLine = DirectCast(oCurve, Line)
								oNewLine = New Line
								oNewLine.CopyFrom(oLine)
								oNewLine.Layer = sDestLayer
								AcadTransaction.AppendEntity(oNewLine)
								zzSetXData(oNewLine, -1, oCurve.Handle, oCurve.Layer, bOverlay)
								iLineCounter += 1
							Case AcadConst.AcadArcName
								oArc = DirectCast(oCurve, Arc)
								oNewArc = New Arc
								oNewArc.CopyFrom(oArc)
								oNewArc.Layer = sDestLayer
								AcadTransaction.AppendEntity(oNewArc)
								zzSetXData(oNewArc, -1, oCurve.Handle, oCurve.Layer, bOverlay)
								iArcCounter += 1
							Case AcadConst.AcadPolylineName
								Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
								zzExplodePolyline(oPolyline, sDestLayer, bOverlay)
								iPolylineCounter += 1
								'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
							Case AcadConst.Acad2dPolylineName
								Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
								zzExplodePolyline(oPolyline2d, sDestLayer, bOverlay)
								iPolyline2dCounter += 1
							Case Else
						End Select
					End If
				End If
			End If
		Next tObjID
		DMAcadExt.AcadDocument.WriteMessage("Overlay layers: " & oTopoDef.LinkLayers)
		Dim sMsg As String = String.Empty
		If iPolylineCounter <> 0 Then
			sMsg = " Polylines:" & CStr(iPolylineCounter)
		End If
		If iPolyline2dCounter <> 0 Then
			sMsg = " 2dPolylines:" & CStr(iPolyline2dCounter)
		End If
		If iLineCounter <> 0 Then
			sMsg = " Lines:" & CStr(iLineCounter)
		End If
		If iArcCounter <> 0 Then
			sMsg = " Arcs:" & CStr(iArcCounter)
		End If

		If sMsg.Length <> 0 Then
			DMAcadExt.AcadDocument.WriteMessage(sMsg)
		End If
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Public Shared Sub Straighten(ByVal oTopoDef As TopoDef)
		Dim dTolerance As Double = 0.1
		Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
		Dim oCurve As Curve
		Dim sIncludeLayers As String = oTopoDef.IncludeLayers
		Dim oTopoDefByLayer As TopoDef = Nothing
		Dim sDestLayer As String = oTopoDef.LinkLayers
		Dim saLayers() As String = TopoDef.ValueSplit(sDestLayer)
		Dim sRXClassName As String
		Dim iArcCounter As Integer = 0
		Dim iEntityCounter As Integer = 0
		Dim iRes As Integer = 0
		DMAcadExt.AcadDocument.WriteMessage("101: " & CStr(dicModelSpaceObjIDs.Count))
		If AcadTransaction.CreateLayer(saLayers, 1, True) Then
			For Each tObjID As ObjectId In dicModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite, TopoDef.AddDelim(sIncludeLayers))
				If oCurve IsNot Nothing Then
					sRXClassName = oCurve.GetRXClass().Name
					'		AcadDocument.WriteMessage("()()()()() " & CStr(sRXClassName))
					Select Case sRXClassName
						Case AcadConst.AcadLineName
						Case AcadConst.AcadArcName
							Dim oArc As Arc = DirectCast(oCurve, Arc)
							StraightenArc(oArc, dTolerance, sDestLayer)
							iArcCounter += 1
							iEntityCounter += 1
						Case AcadConst.AcadPolylineName
							Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
							iRes = StraightenPolylineA(oPolyline, dTolerance, sDestLayer)
						Case AcadConst.Acad2dPolylineName
							Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
							iRes = StraightenPolylineA(oPolyline2d, dTolerance, sDestLayer)
						Case Else
					End Select
					If iRes > 0 Then
						iArcCounter += iRes
						iEntityCounter += iRes
						iRes = 0
					End If
				End If
			Next tObjID
			DMAcadExt.AcadDocument.UpdateScreen()
			DMAcadExt.AcadDocument.WriteMessage("Modified: Entities - " & CStr(iEntityCounter) & ";  Arcs - " & CStr(iArcCounter))

		End If
	End Sub

	Public Shared Sub BuildOverlayZ(ByVal oTopoDef As TopoDef)	' 27/04/2009
		Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
		Dim oCurve As Curve
		Dim saLayers() As String = TopoDef.ValueSplit(oTopoDef.LinkLayers)
		Dim sCurveLayerName As String
		Dim oTopoDefByLayer As TopoDef = Nothing
		Dim bOverlay As Boolean
		Dim sDestLayer As String
		Dim sDuplicateLayer As String = oTopoDef.DuplicateLayer


		Dim sRXClassName As String
		Dim oLine, oNewLine As Line
		Dim oArc, oNewArc As Arc
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		mdicLines = New SegmentDic(0.001)
		EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
		EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
		DMAcadExt.AcadDocument.WriteMessage("100: " & CStr(dicModelSpaceObjIDs.Count))
		AcadTransaction.CreateLayer(saLayers, 1, True)
		AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)
		For Each tObjID As ObjectId In dicModelSpaceObjIDs
			oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)
			If oCurve IsNot Nothing Then
				sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
				'	DMAcadExt.AcadDocument.WriteMessage("300:" & sCurveLayerName)
				bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
				If oTopoDefByLayer IsNot Nothing Then
					sDestLayer = oTopoDef.GetDoubleLayer(oCurve.Layer, False)
					If sDestLayer IsNot Nothing Then

						sRXClassName = oCurve.GetRXClass().Name
						'		AcadDocument.WriteMessage("()()()()() " & CStr(sRXClassName))
						Select Case sRXClassName
							Case AcadConst.AcadLineName
								oLine = DirectCast(oCurve, Line)
								oNewLine = New Line
								oNewLine.CopyFrom(oLine)
								oNewLine.Layer = sDestLayer

								AcadTransaction.AppendEntity(oNewLine)
								mdicEntityCells.AddLine(oNewLine, oTopoDefByLayer, bOverlay)
								zzAddLineSegmentZ(oLine, -1, oCurve.Handle, sDestLayer, bOverlay, sDuplicateLayer)
							Case AcadConst.AcadArcName
								oArc = DirectCast(oCurve, Arc)
								oNewArc = New Arc
								oNewArc.CopyFrom(oArc)
								oNewArc.Layer = sDestLayer
								'		oNewArc.XData = oResBuffer
								AcadTransaction.AppendEntity(oNewArc)
								mdicEntityCells.AddArc(oArc, oTopoDefByLayer, bOverlay)
								oCurve = Nothing
							Case AcadConst.AcadPolylineName
								Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
								zzExplodePolylineZ(oPolyline, sDestLayer, bOverlay, sDuplicateLayer)
								oCurve = Nothing
								'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
							Case AcadConst.Acad2dPolylineName
								Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
								zzExplodePolylineZ(oPolyline2d, sDestLayer, bOverlay, sDuplicateLayer)
								oCurve = Nothing
							Case Else
						End Select
					End If
				End If
			End If
		Next tObjID
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub

	Public Shared Function dmCleanupNew(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		EntityCell.CircleMarkBlock = New MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)

		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0
			Dim bStraightenCond As Boolean = tCleanupOptions.StraightenTolerance > 0.0
			Dim sCurveLayerName As String
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells = New EntityCells()
				EntityCell.Init()
				KeyPoint.SetOrigin(AcadDocument.GetMinPoint())
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance
			End If
			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()
			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()

			Dim sNewLayer As String = tCleanupOptions.DestLayers
			Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)
			Dim oTopoDef As TopoDef = tCleanupOptions.TopoDef
			Dim oTopoDefByLayer As TopoDef = Nothing
			Dim oResTplnPoint As TPlnPoint
			'	DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)


			AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			'PLINETYPE
			Dim bOverlay As Boolean
			Dim iRemain As Integer = 0
			EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
			Dim saLayers() As String = TopoDef.ValueSplit(oTopoDef.LinkLayers)
			Dim sDuplicateLayer As String = Nothing
			Dim oResBuffer As ResultBuffer
			Dim tSegmentBuffer As SegmentBuffer
			Dim oRemoveDuplicatesPointArray As TplnPointArray = Nothing
			mdicLines = New SegmentDic(tCleanupOptions.RemoveDuplicatesTolerance)
			If tCleanupOptions.RemoveDuplicates Then
				sDuplicateLayer = tCleanupOptions.DuplicateLayer
				AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)

				oRemoveDuplicatesPointArray = New TplnPointArray
			End If
			AcadDocument.WriteMessage("SourceLayer=" & sLayersDel & "; ")
			For Each tObjID As ObjectId In dicModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)

				If oCurve IsNot Nothing Then
					oResBuffer = oCurve.XData
					If oResBuffer IsNot Nothing Then
						tSegmentBuffer = New SegmentBuffer(oResBuffer)
					End If
					sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)

					If sLayersDel.Contains(sCurveLayerName) Then
						sRXClassName = oCurve.GetRXClass().Name
						If tCleanupOptions.RemoveDuplicates OrElse tCleanupOptions.Straighten Then
							'	AcadDocument.WriteMessage("%%" & sCurveLayerName & ";" & sRXClassName)
							Select Case sRXClassName
								Case AcadConst.AcadArcName
									Dim oArc As Arc = DirectCast(oCurve, Arc)
									If tCleanupOptions.RemoveDuplicates Then
										oResTplnPoint = zzAddArcSegment(oArc, bFix, sDuplicateLayer)
										If oResTplnPoint IsNot Nothing Then
											oRemoveDuplicatesPointArray.Add(oResTplnPoint)
										End If
									End If
									If tCleanupOptions.Rounding <> enMerging.None Then
										mdicEntityCells.AddArc(oArc, oTopoDef, tSegmentBuffer.Overlay)
									End If


								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
									'	zzCleanupPolyline(oPolyline, sdestlayer, bFix, tCleanupOptions, enClenupMethods.RetainCreateNew, oTopoDef, bOverlay)
									oCurve = Nothing
									'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
									If tCleanupOptions.Straighten Then
										oStraightenPointArray.Add(StraightenPolyline2d(oPolyline2d, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods, oTopoDefByLayer, bOverlay, tCleanupOptions.DestLayers))
									ElseIf tCleanupOptions.CreateNew Then
										Dim oNewPline2d As Polyline2d = New Polyline2d
										oNewPline2d.CopyFrom(oPolyline2d)
										AcadTransaction.AppendEntity(oNewPline2d)
										mdicEntityCells.AddPolyline2d(oNewPline2d, oTopoDefByLayer, bOverlay)
									Else
										mdicEntityCells.AddPolyline2d(oPolyline2d, oTopoDefByLayer, bOverlay)
									End If
								Case AcadConst.AcadLineName
									Dim oLine As Line = DirectCast(oCurve, Line)
									If tCleanupOptions.RemoveDuplicates Then
										oResTplnPoint = zzAddLineSegment(oLine, bFix, sDuplicateLayer)
										If oResTplnPoint IsNot Nothing Then
											oRemoveDuplicatesPointArray.Add(oResTplnPoint)
										End If
									End If
									If tCleanupOptions.Rounding <> enMerging.None Then
										mdicEntityCells.AddLine(oLine, oTopoDef, tSegmentBuffer.Overlay)
									End If
								Case Else
							End Select
						End If
					End If
				End If
			Next
			DMAcadExt.AcadDocument.WriteMessage("Remain=" & CStr(iRemain))

			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If

			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
			If tCleanupOptions.Straighten AndAlso bStraightenCond Then
				tCleanupResult.AddResult(tCleanupOptions.StraightenRowindex, oStraightenPointArray, oStraightenPointArray.UpperBound + 1)
			End If
			If tCleanupOptions.RemoveDuplicates Then
				tCleanupResult.AddResult(tCleanupOptions.RemoveDuplicatesRowIndex, oRemoveDuplicatesPointArray, oRemoveDuplicatesPointArray.UpperBound + 1)
			End If
			DMAcadExt.AcadDocument.WriteMessage("Rounding=" & tCleanupOptions.Rounding.ToString())
			If tCleanupOptions.Rounding <> enMerging.None Then
				DMAcadExt.AcadDocument.WriteMessage("CellNum:" & CStr(mdicEntityCells.Count))
				CheckPointAndSegments()
				mdicEntityCells.Calculate()
				'	MessageBox.Show(CStr(mdicLines.Count), "24_820")
				EntityCell.ExecAll(bFix, tCleanupOptions.Rounding = enMerging.Rounding)
				''	MessageBox.Show(CStr(mdicLines.Count), "24_840")
				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, mdicEntityCells.ErrPoints, mdicEntityCells.SorceErrors)
				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(mdicEntityCells.SorceErrors))
				'	MessageBox.Show(CStr(mdicLines.Count), "24_860")
			End If
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502b")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try

	End Function

	Public Shared Function dmCleanupA(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		EntityCell.CircleMarkBlock = New MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)

		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0
			Dim bStraightenCond As Boolean = tCleanupOptions.StraightenTolerance > 0.0
			Dim sCurveLayerName As String
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells = New EntityCells()
				EntityCell.Init()
				KeyPoint.SetOrigin(AcadDocument.GetMinPoint())
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance
			End If
			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()
			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()

			Dim sNewLayer As String = tCleanupOptions.DestLayers
			Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)
			Dim oTopoDef As TopoDef = tCleanupOptions.TopoDef
			Dim oTopoDefByLayer As TopoDef = Nothing
			Dim oResTplnPoint As TPlnPoint
			'	DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)


			AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			'PLINETYPE
			Dim bOverlay As Boolean
			Dim iRemain As Integer = 0
			EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
			Dim saLayers() As String = TopoDef.ValueSplit(oTopoDef.LinkLayers)
			Dim sDuplicateLayer As String
			Dim oResBuffer As ResultBuffer
			Dim tSegmentBuffer As SegmentBuffer
			Dim oRemoveDuplicatesPointArray As TplnPointArray = Nothing
			mdicLines = New SegmentDic(tCleanupOptions.RemoveDuplicatesTolerance)


			If tCleanupOptions.RemoveDuplicates Then
				sDuplicateLayer = tCleanupOptions.DuplicateLayer
				AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)
				oRemoveDuplicatesPointArray = New TplnPointArray
			Else
				sDuplicateLayer = String.Empty
			End If
			AcadDocument.WriteMessage("SourceLayer=" & sLayersDel & "; ")
			For Each tObjID As ObjectId In dicModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)

				If oCurve IsNot Nothing Then
					oResBuffer = oCurve.XData
					If oResBuffer IsNot Nothing Then
						tSegmentBuffer = New SegmentBuffer(oResBuffer)
					End If
					sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
					If sLayersDel.Contains(sCurveLayerName) Then
						sRXClassName = oCurve.GetRXClass().Name
						If tCleanupOptions.RemoveDuplicates OrElse tCleanupOptions.Rounding <> enMerging.None Then
							'	AcadDocument.WriteMessage("%%" & sCurveLayerName & ";" & sRXClassName)
							Select Case sRXClassName
								Case AcadConst.AcadArcName
									Dim oArc As Arc = DirectCast(oCurve, Arc)
									oResTplnPoint = zzAddArcSegment(oArc, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint)
									End If
									If tCleanupOptions.Rounding <> enMerging.None Then
										mdicEntityCells.AddArc(oArc, oTopoDef, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
									If tCleanupOptions.Rounding <> enMerging.None Then
										zzAddPolyline(oPolyline, oTopoDef, tSegmentBuffer.Overlay)
										'	mdicEntityCells.AddPolyline(oPolyline, oTopoDef, tSegmentBuffer.Overlay)
									End If

									'	zzCleanupPolyline(oPolyline, sdestlayer, bFix, tCleanupOptions, enClenupMethods.RetainCreateNew, oTopoDef, bOverlay)
									oCurve = Nothing
									'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
									If tCleanupOptions.Rounding <> enMerging.None Then
										mdicEntityCells.AddPolyline2d(oPolyline2d, oTopoDef, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadLineName
									Dim oLine As Line = DirectCast(oCurve, Line)
									oResTplnPoint = zzAddLineSegment(oLine, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint) 'result
									End If
									If tCleanupOptions.Rounding <> enMerging.None Then
										mdicEntityCells.AddLine(oLine, oTopoDef, tSegmentBuffer.Overlay)
									End If
								Case Else
							End Select
						End If
					End If
				End If
			Next
			DMAcadExt.AcadDocument.WriteMessage("Remain=" & CStr(iRemain))

			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If

			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)

			If tCleanupOptions.RemoveDuplicates Then
				tCleanupResult.AddResult(tCleanupOptions.RemoveDuplicatesRowIndex, oRemoveDuplicatesPointArray, oRemoveDuplicatesPointArray.UpperBound + 1)
			End If
			DMAcadExt.AcadDocument.WriteMessage("Rounding=" & tCleanupOptions.Rounding.ToString())
			If tCleanupOptions.Rounding <> enMerging.None Then
				DMAcadExt.AcadDocument.WriteMessage("CellNum:" & CStr(mdicEntityCells.Count))
				CheckPointAndSegments()
				mdicEntityCells.Calculate()
				'	MessageBox.Show(CStr(mdicLines.Count), "24_820")
				EntityCell.ExecAll(bFix, tCleanupOptions.Rounding = enMerging.Rounding)
				''	MessageBox.Show(CStr(mdicLines.Count), "24_840")
				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, mdicEntityCells.ErrPoints, mdicEntityCells.SorceErrors)
				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(mdicEntityCells.SorceErrors))
				'	MessageBox.Show(CStr(mdicLines.Count), "24_860")
			End If
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502b")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try

	End Function
	Public Shared Function dmCleanup(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim iStraightenCounter As Integer

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		'  sLayersDel = moTopoDefs.LinkLayers
		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0
			Dim bStraightenCond As Boolean = tCleanupOptions.StraightenTolerance > 0.0
			Dim sCurveLayerName As String
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells = New EntityCells()
				EntityCell.Init()
				KeyPoint.SetOrigin(AcadDocument.GetMinPoint())
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance
			End If
			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()
			Dim oTplnArc As TplnArc
			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()

			Dim sNewLayer As String = tCleanupOptions.DestLayers
			Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)
			Dim oTopoDef As TopoDef = tCleanupOptions.TopoDef
			Dim oTopoDefByLayer As TopoDef = Nothing
			Dim bCurrentLayerOK As Boolean = True
			'DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)


			AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			'PLINETYPE
			Dim bOverlay As Boolean
			Dim iRemain As Integer = 0
			EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef

			For Each tObjID As ObjectId In dicModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)
				If oCurve IsNot Nothing Then
					sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					'	AcadDocument.WriteMessage("@@" & sLayersDel & ";" & sCurveLayerName)
					bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
					If oTopoDefByLayer IsNot Nothing Then

						sRXClassName = oCurve.GetRXClass().Name
						If tCleanupOptions.Straighten OrElse True Then
							'	AcadDocument.WriteMessage("%%" & sCurveLayerName & ";" & sRXClassName)
							Select Case sRXClassName
								Case AcadConst.AcadArcName
									Dim oArc As Arc = DirectCast(oCurve, Arc)
									If bFix AndAlso tCleanupOptions.Straighten Then
										StraightenArc(oArc, tCleanupOptions.StraightenTolerance, tCleanupOptions.EraseSource, oTopoDefByLayer, bOverlay, tCleanupOptions.DestLayers)
									ElseIf tCleanupOptions.CreateNew Then
										Dim oNewArc As Arc = New Arc
										oNewArc.CopyFrom(oArc)
										AcadTransaction.AppendEntity(oNewArc)
										mdicEntityCells.AddArc(oNewArc, oTopoDefByLayer, bOverlay)
										oCurve = Nothing
									Else
										mdicEntityCells.AddArc(oArc, oTopoDefByLayer, bOverlay)
										oCurve = Nothing
									End If
									If tCleanupOptions.Straighten Then
										iStraightenCounter += 1
										oTplnArc = New TplnArc(oArc)
										oStraightenPointArray.Add(oTplnArc.GetMidPoint())
									End If
								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
									If tCleanupOptions.Straighten Then
										oStraightenPointArray.Add(StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods, oTopoDefByLayer, bOverlay, tCleanupOptions.DestLayers))
									ElseIf tCleanupOptions.CreateNew Then
										Dim oNewPline As Polyline = New Polyline
										oNewPline.CopyFrom(oPolyline)
										If tCleanupOptions.DestLayers IsNot Nothing Then
											oNewPline.Layer = tCleanupOptions.DestLayers
										End If
										AcadTransaction.AppendEntity(oNewPline)
										mdicEntityCells.AddPolyline(oNewPline, oTopoDefByLayer, bOverlay)
									Else
										mdicEntityCells.AddPolyline(oPolyline, oTopoDefByLayer, bOverlay)
									End If
									oCurve = Nothing
									'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
									If tCleanupOptions.Straighten Then
										oStraightenPointArray.Add(StraightenPolyline2d(oPolyline2d, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods, oTopoDefByLayer, bOverlay, tCleanupOptions.DestLayers))
									ElseIf tCleanupOptions.CreateNew Then
										Dim oNewPline2d As Polyline2d = New Polyline2d
										oNewPline2d.CopyFrom(oPolyline2d)
										AcadTransaction.AppendEntity(oNewPline2d)
										mdicEntityCells.AddPolyline2d(oNewPline2d, oTopoDefByLayer, bOverlay)
									Else
										mdicEntityCells.AddPolyline2d(oPolyline2d, oTopoDefByLayer, bOverlay)
									End If
								Case AcadConst.AcadLineName
									If sNewLayer IsNot Nothing AndAlso sNewLayer.Length <> 0 Then
										Dim oLine As Line = DirectCast(oCurve, Line)
										Dim oNewPline As Line = New Line
										oNewPline.CopyFrom(oLine)
										oNewPline.Layer = sNewLayer
										AcadTransaction.AppendEntity(oNewPline)
									End If
								Case Else
							End Select
						End If

						If tCleanupOptions.Rounding <> enMerging.None AndAlso oCurve IsNot Nothing Then
							iRemain += 1
							mdicEntityCells.AddCurve(oCurve, tCleanupOptions.TopoDef, bOverlay)
						End If

					End If
				End If
			Next
			DMAcadExt.AcadDocument.WriteMessage("Remain=" & CStr(iRemain))

			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If

			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
			If tCleanupOptions.Straighten AndAlso bStraightenCond Then
				tCleanupResult.AddResult(tCleanupOptions.StraightenRowindex, oStraightenPointArray, oStraightenPointArray.UpperBound + 1)
			End If

			If tCleanupOptions.Rounding <> enMerging.None AndAlso bRoundingCond Then
				DMAcadExt.AcadDocument.WriteMessage("CellNum:" & CStr(mdicEntityCells.Count))
				mdicEntityCells.Calculate()

				EntityCell.ExecAll(bFix, True)

				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, mdicEntityCells.ErrPoints, mdicEntityCells.SorceErrors)
				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(mdicEntityCells.SorceErrors))
			End If
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502b")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try
	End Function
	Public Shared Sub CheckPointAndSegments()
		Dim oTestCurve As Curve
		Dim lTestHandleValue As Long = &H245
		DMAcadExt.AcadDocument.WriteMessage("Src Lines:" & CStr(mdicLines.Count))
		For Each oCurveSegment As TplnCurveSegment In mdicLines.Keys
			oTestCurve = mdicLines.Item(oCurveSegment)
			mdicEntityCells.AddSegment(oCurveSegment, oTestCurve.Handle.Value = lTestHandleValue)
		Next
	End Sub
	Public Shared Sub StraightenArc(ByVal oArc As Arc, ByVal dTolerance As Double, ByVal sNewLineLayer As String)
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))
		Dim oNewPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

		dTolerAngle = oArc.TotalAngle / iPart
		DMAcadExt.AcadDocument.WriteMessage("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		oNewPolyline.AddVertexAt(0, TPlnPoint.Point3dTo2d(oArc.StartPoint), 0.0, 0.0, 0.0)
		For iIndex As Integer = 1 To iPart - 1
			dAngle = oArc.StartAngle + iIndex * dTolerAngle
			tPoint = New Autodesk.AutoCAD.Geometry.Point2d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle))
			oNewPolyline.AddVertexAt(iIndex, tPoint, 0.0, 0.0, 0.0)
		Next
		oNewPolyline.AddVertexAt(iPart, TPlnPoint.Point3dTo2d(oArc.EndPoint), 0.0, 0.0, 0.0)
		If sNewLineLayer.Length <> 0 Then
			oNewPolyline.Layer = sNewLineLayer
		End If
		AcadTransaction.AppendEntity(oNewPolyline, False)

	End Sub

	Public Shared Sub StraightenArc(ByVal oArc As Arc, ByVal dTolerance As Double, ByVal bEraseSource As Boolean, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean, Optional ByVal sNewLineLayer As String = "")
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))
		Dim oNewPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

		dTolerAngle = oArc.TotalAngle / iPart
		DMAcadExt.AcadDocument.WriteMessage("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		oNewPolyline.AddVertexAt(0, TPlnPoint.Point3dTo2d(oArc.StartPoint), 0.0, 0.0, 0.0)
		For iIndex As Integer = 1 To iPart - 1
			dAngle = oArc.StartAngle + iIndex * dTolerAngle
			tPoint = New Autodesk.AutoCAD.Geometry.Point2d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle))
			oNewPolyline.AddVertexAt(iIndex, tPoint, 0.0, 0.0, 0.0)
		Next
		oNewPolyline.AddVertexAt(iPart, TPlnPoint.Point3dTo2d(oArc.EndPoint), 0.0, 0.0, 0.0)
		If sNewLineLayer.Length <> 0 Then
			oNewPolyline.Layer = sNewLineLayer
		End If
		AcadTransaction.AppendEntity(oNewPolyline, False)

		If bEraseSource Then
			oArc.Erase()
		End If
		mdicEntityCells.AddPolyline(oNewPolyline, oTopoDef, bOverlay)

		'oArc.Center
	End Sub

	Public Shared Function StraightenPolyline2d(ByVal oPolyline2d As Polyline2d, ByVal bFix As Boolean, ByVal dTolerance As Double, ByVal iClenupMethods As enClenupMethods, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean, Optional ByVal sNewLineLayer As String = "") As TplnPointArray
		Select Case iClenupMethods
			Case enClenupMethods.Modify
				Return StraightenPolylineA(oPolyline2d, dTolerance, oTopoDef, bOverlay, bFix, bFix)
			Case enClenupMethods.RetainCreateNew, enClenupMethods.DeleteCreateNew
				If bFix Then
					Return StraightenPolylineA(oPolyline2d, dTolerance, oTopoDef, bOverlay, True, False)
				Else
					Dim oNewPline As Polyline2d = New Polyline2d
					oNewPline.CopyFrom(oPolyline2d)
					If sNewLineLayer.Length <> 0 Then
						oNewPline.Layer = sNewLineLayer
					End If
					Return StraightenPolylineA(oPolyline2d, dTolerance, oTopoDef, bOverlay, False, False)
				End If
			Case Else
				Return Nothing
		End Select
	End Function
	Private Shared Function zzCleanupPolyline(ByRef oPolyline As Polyline, ByVal sDestLayer As String, ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions, ByVal iClenupMethods As enClenupMethods, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean, Optional ByVal sNewLineLayer As String = "") As TplnPointArray
		'zzExplodePolyline(oPolyline, sDestLayer)
		Select Case tCleanupOptions.ClenupMethods
			Case enClenupMethods.Modify
				Return StraightenPolylineA(oPolyline, bFix, tCleanupOptions.StraightenTolerance, oTopoDef, bOverlay)
			Case enClenupMethods.RetainCreateNew, enClenupMethods.DeleteCreateNew  ' Main 
				Dim oNewPline As Polyline = New Polyline()
				oNewPline.CopyFrom(oPolyline)
				If sNewLineLayer.Length <> 0 Then
					oNewPline.Layer = sNewLineLayer
				End If

				If bFix AndAlso iClenupMethods = enClenupMethods.DeleteCreateNew Then
					oPolyline.Erase()
				End If
				'	DMAcadExt.AcadDocument.WriteMessage("P_Line=" & oNewPline.Layer)
				AcadTransaction.AppendEntity(oNewPline)
				oPolyline = oNewPline
				Return StraightenPolylineA(oNewPline, bFix, tCleanupOptions.StraightenTolerance, oTopoDef, bOverlay)
			Case Else
				Return Nothing
		End Select
	End Function

	Public Shared Function StraightenPolyline(ByRef oPolyline As Polyline, ByVal bFix As Boolean, ByVal dTolerance As Double, ByVal iClenupMethods As enClenupMethods, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean, Optional ByVal sNewLineLayer As String = "") As TplnPointArray
		Select Case iClenupMethods
			Case enClenupMethods.Modify
				Return StraightenPolylineA(oPolyline, bFix, dTolerance, oTopoDef, bOverlay)
			Case enClenupMethods.RetainCreateNew, enClenupMethods.DeleteCreateNew
				Dim oNewPline As Polyline = New Polyline
				oNewPline.CopyFrom(oPolyline)
				If sNewLineLayer.Length <> 0 Then
					Try
						oNewPline.Layer = sNewLineLayer
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sNewLineLayer, "dmLineCleanup - StraightenPolyline")
					End Try

				End If

				If bFix AndAlso iClenupMethods = enClenupMethods.DeleteCreateNew Then
					oPolyline.Erase()
				End If
				'	DMAcadExt.AcadDocument.WriteMessage("P_Line=" & oNewPline.Layer)
				AcadTransaction.AppendEntity(oNewPline)
				oPolyline = oNewPline
				Return StraightenPolylineA(oNewPline, bFix, dTolerance, oTopoDef, bOverlay)
			Case Else
				Return Nothing
		End Select
	End Function
	Public Shared Function StraightenPolylineA(ByVal oPolyline2d As Polyline2d, ByVal dTolerance As Double, ByVal sNewLineLayer As String) As Integer
		Dim dBulge As Double = 0.0
		Dim oArc2d As CircularArc2d = Nothing
		Dim oTplnArc As DMAcadExt.TplnArc = Nothing

		Dim tObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing

		Dim oaInnerPoints() As Point2d
		Dim tPriorPoint, tCurrentPoint2d As Point2d
		Dim tCurrentPoint As Point3d
		Dim iArcCounter As Integer
		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim colNewPoints As Point3dCollection = New Point3dCollection()
		Dim colNewBulges As DoubleCollection = New DoubleCollection()
		Dim oaResPoints As TplnPointArray = New TplnPointArray()
		Try
			Do While oColEnum.MoveNext()
				tObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tObjID, OpenMode.ForWrite)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				tCurrentPoint = oVertex2d.Position
				tCurrentPoint2d = TPlnPoint.Point3dTo2d(tCurrentPoint)
				If dBulge <> 0.0 Then 'Prior Bulge
					oArc2d = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, True)
					oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tPriorPoint, tCurrentPoint2d, dBulge > 0.0)
					oaResPoints.Add(oTplnArc.GetMidPoint())

					oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
					For iIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
						colNewPoints.Add(TPlnPoint.Point2dTo3d(oaInnerPoints(iIndex)))
						colNewBulges.Add(0.0)
					Next
					iArcCounter += 1

				End If
				dBulge = oVertex2d.Bulge
				colNewPoints.Add(tCurrentPoint)
				If dBulge <> 0 Then
					tPriorPoint = tCurrentPoint2d
				End If
				'	DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iVert) & ":" & TPlnPoint.DispPoint(oVertex2d.Position) & ";" & CStr(oVertex2d.Bulge) & "-" & oVertex2d.VertexType.ToString())
			Loop

			If iArcCounter <> 0 Then
				Dim oNewPolyline2d As Polyline2d = New Polyline2d(Poly2dType.SimplePoly, colNewPoints, oPolyline2d.Elevation, oPolyline2d.Closed, 0.0, 0.0, colNewBulges)
				If sNewLineLayer.Length <> 0 Then
					oNewPolyline2d.Layer = sNewLineLayer
				End If
				AcadTransaction.AppendEntity(oNewPolyline2d, False)
			Else
				colNewPoints.Clear()
				colNewBulges.Clear()
			End If
			Return iArcCounter


		Catch oEx As Exception
			'		DMAcadExt.AcadDocument.WriteMessage("Reset:" & oEx.Message)
		End Try


		'DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")

	End Function
	Public Shared Function StraightenPolylineA(ByVal oPolyline2d As Polyline2d, ByVal dTolerance As Double, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean, ByVal bCreateNew As Boolean, ByVal bEraseSource As Boolean) As TplnPointArray
		Dim dBulge As Double = 0.0
		Dim oArc2d As CircularArc2d = Nothing
		Dim oTplnArc As DMAcadExt.TplnArc = Nothing
		Dim tObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing
		Dim oaInnerPoints() As Point2d
		Dim tPriorPoint, tCurrentPoint2d As Point2d
		Dim tCurrentPoint As Point3d

		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim colNewPoints As Point3dCollection = New Point3dCollection()
		Dim colNewBulges As DoubleCollection = New DoubleCollection()
		Dim oaResPoints As TplnPointArray = New TplnPointArray()
		Try
			Do While oColEnum.MoveNext()
				tObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tObjID, OpenMode.ForWrite)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				tCurrentPoint = oVertex2d.Position
				tCurrentPoint2d = TPlnPoint.Point3dTo2d(tCurrentPoint)
				If dBulge <> 0.0 Then 'Prior Bulge
					oArc2d = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, True)
					oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tPriorPoint, tCurrentPoint2d, dBulge > 0.0)
					oaResPoints.Add(oTplnArc.GetMidPoint())
					If bCreateNew Then
						oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
						For iIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
							colNewPoints.Add(TPlnPoint.Point2dTo3d(oaInnerPoints(iIndex)))
							colNewBulges.Add(0.0)
						Next
					End If

				End If
				dBulge = oVertex2d.Bulge
				colNewPoints.Add(tCurrentPoint)
				If dBulge <> 0 Then
					tPriorPoint = tCurrentPoint2d
				End If
				'	DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iVert) & ":" & TPlnPoint.DispPoint(oVertex2d.Position) & ";" & CStr(oVertex2d.Bulge) & "-" & oVertex2d.VertexType.ToString())
			Loop
			If bEraseSource Then
				oPolyline2d.Erase()
			End If

			If bCreateNew Then
				Dim oNewPolyline2d As Polyline2d = New Polyline2d(Poly2dType.SimplePoly, colNewPoints, oPolyline2d.Elevation, oPolyline2d.Closed, 0.0, 0.0, colNewBulges)
				AcadTransaction.AppendEntity(oNewPolyline2d, False)
				mdicEntityCells.AddPolyline2d(oNewPolyline2d, oTopoDef, bOverlay)
			End If


		Catch oEx As Exception
			'		DMAcadExt.AcadDocument.WriteMessage("Reset:" & oEx.Message)
		End Try


		'DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")
		Return oaResPoints
	End Function
	Public Shared Function StraightenPolylineA(ByVal oPolyline As Polyline, ByVal dTolerance As Double, ByVal sNewPolylineLayer As String) As Integer
		Const iExNo As Integer = 10
		Dim dbulge As Double
		Dim oArc2d As CircularArc2d
		Dim oTplnArc As DMAcadExt.TplnArc
		Dim oArc As Arc

		Dim iSegmentIndex As Integer
		Dim oaInnerPoints() As Point2d
		Dim tStartPoint, tEndPoint As Point2d
		Dim iSegmentType As SegmentType
		Dim iArcCounter As Integer
		Dim oResPoints As TplnPointArray = New TplnPointArray()
		Dim oNewPolyline As Polyline = New Polyline
		oNewPolyline.CopyFrom(oPolyline)
		oNewPolyline.XData = New ResultBuffer()
		If sNewPolylineLayer.Length <> 0 Then
			Try
				oNewPolyline.Layer = sNewPolylineLayer
			Catch oEx As Exception
				AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			End Try

		End If
		Do
			Try
				iSegmentType = oNewPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oNewPolyline.NumberOfVertices) & "!" & CStr(oNewPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				iArcCounter += 1

				Try
					dbulge = oNewPolyline.GetBulgeAt(iSegmentIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oNewPolyline.NumberOfVertices) & "!" & CStr(oNewPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
					Exit Do
				End Try
				Try
					oArc2d = oNewPolyline.GetArcSegment2dAt(iSegmentIndex)
					tStartPoint = oNewPolyline.GetPoint2dAt(iSegmentIndex)
					tEndPoint = oNewPolyline.GetPoint2dAt(iSegmentIndex + 1)
					oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tStartPoint, tEndPoint, dbulge > 0.0)
					oResPoints.Add(oTplnArc.GetMidPoint())
					oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
					oArc = oTplnArc.GetAcadArc()
					oArc.ColorIndex = 1
					''''''	AppendEntity(oArc, False)
					oNewPolyline.SetBulgeAt(iSegmentIndex, 0.0)
					For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
						oNewPolyline.AddVertexAt(iSegmentIndex + 1 + iPointIndex, oaInnerPoints(iPointIndex), 0.0, 0.0, 0.0)
					Next
					iSegmentIndex += oaInnerPoints.GetUpperBound(0) + 1

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145")
				End Try
			End If

			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oNewPolyline.NumberOfVertices - 1)
		If iArcCounter <> 0 Then
			AcadTransaction.AppendEntity(oNewPolyline)
		Else
			oNewPolyline = Nothing
		End If
		Return iArcCounter
		'DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")

	End Function
	Public Shared Function StraightenPolylineA(ByVal oPolyline As Polyline, ByVal bFix As Boolean, ByVal dTolerance As Double, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean) As TplnPointArray
		Dim dbulge As Double
		Dim oArc2d As CircularArc2d
		Dim oTplnArc As DMAcadExt.TplnArc
		Dim oArc As Arc

		Dim iSegmentIndex As Integer
		Dim oaInnerPoints() As Point2d
		Dim tStartPoint, tEndPoint As Point2d
		Dim iSegmentType As SegmentType
		Dim iArcCounter As Integer
		Dim oResPoints As TplnPointArray = New TplnPointArray()
		'	System.Windows.Forms.MessageBox.Show("YYYYYYY", "23_999")
		Do
			Try
				iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				iArcCounter += 1
				If bFix Then
					Try
						dbulge = oPolyline.GetBulgeAt(iSegmentIndex)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
						Exit Do
					End Try
					Try
						oArc2d = oPolyline.GetArcSegment2dAt(iSegmentIndex)
						tStartPoint = oPolyline.GetPoint2dAt(iSegmentIndex)
						tEndPoint = oPolyline.GetPoint2dAt(iSegmentIndex + 1)
						oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tStartPoint, tEndPoint, dbulge > 0.0)
						oResPoints.Add(oTplnArc.GetMidPoint())
						oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
						oArc = oTplnArc.GetAcadArc()
						oArc.ColorIndex = 1
						''''''	AppendEntity(oArc, False)
						oPolyline.SetBulgeAt(iSegmentIndex, 0.0)
						For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
							oPolyline.AddVertexAt(iSegmentIndex + 1 + iPointIndex, oaInnerPoints(iPointIndex), 0.0, 0.0, 0.0)
						Next
						iSegmentIndex += oaInnerPoints.GetUpperBound(0) + 1
						'	dSourceArea = oArc2d.GetArea(oArc2d.GetParameterOf(oArc2d.StartPoint), oArc2d.GetParameterOf(oArc2d.EndPoint))
						'	dTotalSourceArea += dSourceArea
						'	dTotalNewArea += oNewPline.Area
						'	dErrArea = dSourceArea - oNewPline.Area
						'	DMAcadExt.AcadDocument.WriteMessage("ErrArea: " & CStr(iSegmentIndex) & ":" & CStr(dSourceArea) & "-" & CStr(oNewPline.Area) & "=" & CStr(dErrArea) & ", " & CStr(dErrArea * 100.0 / dSourceArea) & "%")
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145")
					End Try
				End If
			End If
			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oPolyline.NumberOfVertices - 1)
		mdicEntityCells.AddPolyline(oPolyline, oTopoDef, bOverlay)
		'DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")
		Return oResPoints
	End Function
	Private Shared Sub zzAddPolyline(ByVal oPolyline As Polyline, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean)
		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
		Dim bInner As Boolean
		Dim tPriorPoint, tCurrentPoint As Point2d
		Dim oLineSegment As TplnCurveSegment
		For iIndex As Integer = 0 To iVerticesUB
			If iIndex = 0 AndAlso iIndex = iVerticesUB Then
				bInner = False
			Else
				bInner = True
			End If
			tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
			mdicEntityCells.zzAddEntity(tCurrentPoint, oPolyline, iIndex, bInner, oTopoDef, bOverlay)
			oLineSegment = New TplnCurveSegment(tPriorPoint, tCurrentPoint)
			If iIndex <> 0 Then
				zzAddCurveSegment(oLineSegment, Nothing, False, String.Empty)
			End If
			tPriorPoint = tCurrentPoint
		Next
		'	Me.zzAddEntity(oPolyline.GetPoint2dAt(0), oPolyline, 0)
		' 	Me.zzAddEntity(oPolyline.GetPoint2dAt(iVerticesUB), oPolyline, iVerticesUB)
	End Sub
	Public Shared Sub StraightenPolylineTest(ByVal oPolyline As Polyline, ByVal dTolerance As Double)
		Dim dbulge As Double
		Dim oArc2d As CircularArc2d
		Dim oTplnArc As DMAcadExt.TplnArc
		Dim oArc As Arc
		Dim oNewPline As Polyline
		Dim iIndex As Integer
		Dim oaInnerPoints() As Point2d
		Dim tStartPoint, tEndPoint As Point2d
		Dim iSegmentType As SegmentType
		Dim dErrArea As Double
		Dim dSourceArea As Double
		Dim dTotalSourceArea, dTotalNewArea As Double
		Do

			Try
				iSegmentType = oPolyline.GetSegmentType(iIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iIndex) & ":" & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iIndex - 1)), "21_125")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				Try
					dbulge = oPolyline.GetBulgeAt(iIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iIndex) & ":" & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iIndex - 1)), "21_125")
					Exit Do
				End Try
				Try
					oArc2d = oPolyline.GetArcSegment2dAt(iIndex)
					tStartPoint = oPolyline.GetPoint2dAt(iIndex)
					tEndPoint = oPolyline.GetPoint2dAt(iIndex + 1)
					oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tStartPoint, tEndPoint, dbulge > 0.0)
					oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
					oArc = oTplnArc.GetAcadArc()
					oArc.ColorIndex = 1
					AcadTransaction.AppendEntity(oArc, False)
					oNewPline = New Polyline(3 + oaInnerPoints.GetUpperBound(0))
					oNewPline.AddVertexAt(0, tStartPoint, 0.0, 0.0, 0.0)
					For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
						oNewPline.AddVertexAt(iPointIndex + 1, oaInnerPoints(iPointIndex), 0.0, 0.0, 0.0)
					Next
					'	oNewPline.Closed
					'tEndPoint = New Point2d(0, -5)
					oNewPline.AddVertexAt(2 + oaInnerPoints.GetUpperBound(0), tEndPoint, 0.0, 0.0, 0.0)
					oNewPline.ColorIndex = 3
					AcadTransaction.AppendEntity(oNewPline, False)
					dSourceArea = oArc2d.GetArea(oArc2d.GetParameterOf(oArc2d.StartPoint), oArc2d.GetParameterOf(oArc2d.EndPoint))
					dTotalSourceArea += dSourceArea
					dTotalNewArea += oNewPline.Area
					dErrArea = dSourceArea - oNewPline.Area
					DMAcadExt.AcadDocument.WriteMessage("ErrArea: " & CStr(iIndex) & ":" & CStr(dSourceArea) & "-" & CStr(oNewPline.Area) & "=" & CStr(dErrArea) & ", " & CStr(dErrArea * 100.0 / dSourceArea) & "%")
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(CStr(iIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145")
				End Try
			End If
			iIndex += 1
			'	DMAcadExt.AcadDocument.WriteMessage("After=" & CStr(iIndex) & ":" & CStr(oPolyline.NumberOfVertices))
		Loop Until iSegmentType = SegmentType.Point
		DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")

	End Sub
	Public Shared Sub UpdateTopoByMerge(ByVal sMergeTopoName As String)

		Dim colRings As Topology.RingCollection
		Dim colHalfEdges As Topology.HalfEdgeCollection
		Dim oFullEdge As Topology.FullEdge
		Dim tAcObjID As ObjectId
		Dim colLines As ObjectIdCollection = New ObjectIdCollection()



		''''Isprav

		Dim oMergeTopology As Autodesk.Gis.Map.Topology.TopologyModel = AcadDocument.GetTopology(sMergeTopoName)
		If oMergeTopology IsNot Nothing Then

			Dim oLotPgon As Topology.Polygon = Nothing

			If oMergeTopology.Status = Topology.Status.Closed Then
				Try
					oMergeTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - UpdateTopoByMerge_21")

				End Try
				If oMergeTopology.Status <> Topology.Status.Closed Then
					Dim colPolygons As Topology.PolygonCollection
					Try
						colPolygons = oMergeTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - UpdateTopoByMerge_8")
						Try
							If oMergeTopology.Status <> Topology.Status.Closed Then
								oMergeTopology.Close()
								oMergeTopology = Nothing
							End If
						Catch oMapExB As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapExB.ErrorCode, False, "TplnProject - UpdateTopoByMerge_26")
						End Try
						Return
					End Try

					For Each oPolygon As Topology.Polygon In colPolygons
						If oPolygon IsNot Nothing Then
							Try
								colRings = oPolygon.GetBoundary()
								For Each oRing As Topology.Ring In colRings
									colHalfEdges = oRing.GetEdges()
									For Each oHalfEdge As Topology.HalfEdge In colHalfEdges
										oFullEdge = oHalfEdge.FullEdge
										tAcObjID = oFullEdge.Entity
										If Not colLines.Contains(tAcObjID) Then
											colLines.Add(tAcObjID)
											zzRestore(tAcObjID)
										End If
									Next
								Next
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - UpdateTopoByMerge_4")
							End Try
						End If

					Next
					'  oLotPgon.Dispose()
					'  oParcelPgon.Dispose()

					oMergeTopology.Close()
					''  oUnionTopology.Dispose()
					oMergeTopology = Nothing

				End If
			End If
		End If

	End Sub
	Private Shared Function zzRestore(ByVal tAcObjID As ObjectId) As Integer
		Dim oCurve As Curve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForRead)
		Dim oNewCurve As Curve
		Dim tSegmentBuffer As SegmentBuffer
		Dim oResBuffer As ResultBuffer = Nothing
		Dim iCurvesUB As Integer
		Dim sLayer As String
		Dim iCurveCounter As Integer = 0
		If oCurve IsNot Nothing Then
			oResBuffer = oCurve.XData
		Else
			AcadDocument.WriteMessage("RestErr:" & tAcObjID.ToString())
		End If

		If oResBuffer IsNot Nothing Then
			tSegmentBuffer = New SegmentBuffer(oResBuffer)
			If tSegmentBuffer.Application Then
				iCurvesUB = tSegmentBuffer.SourceHandles.GetUpperBound(0)
				For iIndex As Integer = 0 To iCurvesUB
					sLayer = tSegmentBuffer.SourceLayers(iIndex)
					oNewCurve = zzCopyFromCurve(oCurve)
					oNewCurve.Layer = sLayer
					oNewCurve.XData = New ResultBuffer()
					AcadTransaction.AppendEntity(oNewCurve)
					iCurveCounter += 1
				Next
			End If
		End If
	End Function
	Private Shared Function zzCopyFromCurve(ByVal oCurve As Curve) As Curve
		Dim sMsg As String = "^^^" & oCurve.GetRXClass().Name
		Select Case oCurve.GetRXClass().Name
			Case AcadConst.AcadArcName
				Dim oArc As Arc = New Arc
				oArc.CopyFrom(oCurve)
				sMsg &= "/Arc"
				Return oArc
			Case AcadConst.AcadLineName
				Dim oLine As Line = New Line
				oLine.CopyFrom(oCurve)
				sMsg &= "/Line"
				Return oLine
			Case Else
				sMsg &= "/?????"
				Return Nothing
		End Select
	End Function
	Private Shared Sub zzExplodePolyline(ByVal oCurve As Curve, ByVal sDestLayer As String, ByVal bOverlay As Boolean)
		Dim colDBobjects As DBObjectCollection = New DBObjectCollection()
		Dim oEntity As Entity
		Dim iSegmentIndex As Integer = 0
		Dim oNewCurve As Curve
		oCurve.Explode(colDBobjects)

		For Each oDBObject As DBObject In colDBobjects
			oEntity = DirectCast(oDBObject, Entity)
			Try
				oEntity.Layer = sDestLayer
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "'" & sDestLayer & "'", "dmLineCleanup - zzExplodePolyline")
			End Try

			AcadTransaction.AppendEntity(oEntity)
			Select Case oDBObject.GetRXClass().Name
				Case AcadConst.AcadLineName, AcadConst.AcadArcName
					oNewCurve = DirectCast(oDBObject, Curve)
					zzSetXData(oNewCurve, iSegmentIndex, oCurve.Handle, oCurve.Layer, bOverlay)
			End Select
			iSegmentIndex += 1
		Next
	End Sub
	Private Shared Sub zzExplodePolylineZ(ByVal oCurve As Curve, ByVal sDestLayer As String, ByVal bOverlay As Boolean, ByVal sDuplicateLayer As String)
		Dim colDBobjects As DBObjectCollection = New DBObjectCollection()
		Dim oEntity As Entity
		Dim iSegmentIndex As Integer = 0
		oCurve.Explode(colDBobjects)

		For Each oDBObject As DBObject In colDBobjects
			oEntity = DirectCast(oDBObject, Entity)
			Try
				oEntity.Layer = sDestLayer
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "'" & sDestLayer & "'", "dmLineCleanup - zzExplodePolyline")
			End Try

			AcadTransaction.AppendEntity(oEntity)
			Select Case oDBObject.GetRXClass().Name
				Case AcadConst.AcadArcName
				Case AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oDBObject, Line)
					zzAddLineSegmentZ(oLine, iSegmentIndex, oCurve.Handle, sDestLayer, bOverlay, sDuplicateLayer)
			End Select
			iSegmentIndex += 1
		Next
	End Sub
	Private Shared Sub zzSetXData(ByVal oCurve As Curve, ByVal iSegmentIndex As Integer, ByVal tSourceHandle As Handle, ByVal sLayer As String, ByVal bOverlay As Boolean)
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oCurve.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oCurve.EndPoint)
		Dim oLineSegment As LineSegment
		Dim oLinePrev As Line = Nothing
		'		Dim oResBuffer, oNewResBuffer As ResultBuffer
		Dim tSegmentBuffer As SegmentBuffer
		'	Dim sTest As String
		oLineSegment = New LineSegment(tStartPoint, tEndPoint)
		tSegmentBuffer = New SegmentBuffer(bOverlay, tSourceHandle, sLayer, iSegmentIndex)
		oCurve.XData = tSegmentBuffer.GetResBuffer()
		'	AcadDocument.WriteMessage("!!!! " & CStr(mdicLines.Count) & ":" & sTest)
	End Sub
	Private Shared Function zzAddCurveSegment(ByVal oCurveSegment As TplnCurveSegment, ByVal oCurve As Curve, ByVal bFix As Boolean, ByVal sDuplicateLayer As String) As TPlnPoint
		Dim oLinePrev As Curve = Nothing
		Dim bLineExists As Boolean
		Dim oResBufferPrev, oResBuffer As ResultBuffer
		Dim tSegmentBufferPrev, tSegmentBuffer As SegmentBuffer
		Dim sTest As String

		bLineExists = mdicLines.TryGetValue(oCurveSegment, oLinePrev)
		If bLineExists Then
			If sDuplicateLayer.Length <> 0 Then
				miDuplicateLines += 1
				If bFix Then
					oResBufferPrev = oLinePrev.XData
					oResBuffer = oCurve.XData
					If oResBufferPrev IsNot Nothing AndAlso oResBuffer IsNot Nothing Then
						tSegmentBufferPrev = New SegmentBuffer(oResBufferPrev)
						tSegmentBuffer = New SegmentBuffer(oResBuffer)
						If tSegmentBufferPrev.Overlay AndAlso Not tSegmentBuffer.Overlay Then
							oLinePrev.Layer = oCurve.Layer
						End If
						oCurve.Layer = sDuplicateLayer
						tSegmentBufferPrev.AddBuffer(tSegmentBuffer)
						'	tSegmentBuffer.AddSourceHandle(tSourceHandle, iSegmentIndex)
						oLinePrev.XData = tSegmentBufferPrev.GetResBuffer()
					End If
				Else
					Dim oMarkBlock As MarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Circle)
					oMarkBlock.MarkPoint(oCurveSegment.StartPoint, 33)
					oMarkBlock.MarkPoint(oCurveSegment.EndPoint, 33)
				End If
				sTest = "OK " & sDuplicateLayer
				Return New TPlnPoint(oCurveSegment.StartPoint, oCurveSegment.EndPoint)
			Else
				Return Nothing
			End If

		Else 'If bLineExists  
			mdicLines.Add(oCurveSegment, oCurve)
			sTest = "New "
			Return Nothing
		End If
		'	AcadDocument.WriteMessage("!!!! " & CStr(mdicLines.Count) & ":" & sTest)
	End Function
	Private Shared Function zzAddPolylineAAA(ByVal oPolyline As Polyline, ByVal bFix As Boolean, ByVal sDuplicateLayer As String) As TPlnPoint
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oPolyline.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oPolyline.EndPoint)
		Dim oLineSegment As TplnCurveSegment

		oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)
		Return zzAddCurveSegment(oLineSegment, oPolyline, bFix, sDuplicateLayer)

	End Function

	Private Shared Function zzAddLineSegment(ByVal oLine As Line, ByVal bFix As Boolean, ByVal sDuplicateLayer As String) As TPlnPoint
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		Dim oLineSegment As TplnCurveSegment

		oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)
		Return zzAddCurveSegment(oLineSegment, oLine, bFix, sDuplicateLayer)

	End Function
	Private Shared Function zzAddArcSegment(ByVal oArc As Arc, ByVal bFix As Boolean, ByVal sDuplicateLayer As String) As TPlnPoint
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oArc.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oArc.EndPoint)
		Dim oCurveSegment As TplnCurveSegment
		oCurveSegment = New TplnArc(oArc)
		Return zzAddCurveSegment(oCurveSegment, oArc, bFix, sDuplicateLayer)
	End Function
	Private Shared Function zzAddLineSegmentOld(ByVal oLine As Line, ByVal bFix As Boolean, ByVal sDuplicateLayer As String) As TPlnPoint
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		Dim oLineSegment As TplnCurveSegment
		'	Dim oLinePrev As Line = Nothing
		Dim oLinePrev As Curve = Nothing
		Dim bLineExists As Boolean
		Dim oResBufferPrev, oResBuffer As ResultBuffer
		Dim tSegmentBufferPrev, tSegmentBuffer As SegmentBuffer
		Dim sTest As String
		oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)
		bLineExists = mdicLines.TryGetValue(oLineSegment, oLinePrev)
		If bLineExists Then
			miDuplicateLines += 1
			If bFix Then
				oResBufferPrev = oLinePrev.XData
				oResBuffer = oLine.XData
				If oResBufferPrev IsNot Nothing AndAlso oResBuffer IsNot Nothing Then
					tSegmentBufferPrev = New SegmentBuffer(oResBufferPrev)
					tSegmentBuffer = New SegmentBuffer(oResBuffer)
					If tSegmentBufferPrev.Overlay AndAlso Not tSegmentBuffer.Overlay Then
						oLinePrev.Layer = oLine.Layer
					End If
					oLine.Layer = sDuplicateLayer
					tSegmentBufferPrev.AddBuffer(tSegmentBuffer)
					'	tSegmentBuffer.AddSourceHandle(tSourceHandle, iSegmentIndex)
					oLinePrev.XData = tSegmentBufferPrev.GetResBuffer()
				End If
			Else
				Dim oMarkBlock As MarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Circle)
				oMarkBlock.MarkPoint(tStartPoint, 33)
				oMarkBlock.MarkPoint(tEndPoint, 33)
			End If
			sTest = "OK " & sDuplicateLayer
			Return New TPlnPoint(tStartPoint, tEndPoint)
		Else
			mdicLines.Add(oLineSegment, oLine)
			sTest = "New "
			Return Nothing
		End If
		'	AcadDocument.WriteMessage("!!!! " & CStr(mdicLines.Count) & ":" & sTest)
	End Function
	Private Shared Sub zzAddLineSegmentZ(ByVal oLine As Line, ByVal iSegmentIndex As Integer, ByVal tSourceHandle As Handle, ByVal sDestLayer As String, ByVal bOverlay As Boolean, ByVal sDuplicateLayer As String)
		Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
		Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)
		Dim oLineSegment As TplnCurveSegment
		'	Dim oLinePrev As Line = Nothing
		Dim oLinePrev As Curve = Nothing
		Dim bLineExists As Boolean
		Dim oResBuffer As ResultBuffer
		Dim tSegmentBuffer As SegmentBuffer
		Dim sTest As String
		oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)
		bLineExists = mdicLines.TryGetValue(oLineSegment, oLinePrev)
		If bLineExists Then
			oResBuffer = oLinePrev.XData
			tSegmentBuffer = New SegmentBuffer(oResBuffer)
			If tSegmentBuffer.Overlay AndAlso Not bOverlay Then
				oLinePrev.Layer = sDestLayer
			End If
			oLine.Layer = sDuplicateLayer
			oLine.XData = oResBuffer
			tSegmentBuffer.AddSourceHandle(tSourceHandle, "", iSegmentIndex)
			oLinePrev.XData = tSegmentBuffer.GetResBuffer()
			sTest = "OK " & sDestLayer
		Else
			tSegmentBuffer = New SegmentBuffer(bOverlay, tSourceHandle, "", iSegmentIndex)
			oLine.XData = tSegmentBuffer.GetResBuffer()
			mdicLines.Add(oLineSegment, oLine)
			sTest = "New "
		End If
		'	AcadDocument.WriteMessage("!!!! " & CStr(mdicLines.Count) & ":" & sTest)
	End Sub
	Private Structure SegmentBuffer
		Dim Overlay As Boolean
		Dim SourceHandles() As Handle
		Dim SourceLayers() As String
		Dim SegmentIndex() As Integer
		Dim Application As Boolean
		Public Sub New(ByVal bOverlay As Boolean, ByVal tSourceHandle As Handle, ByVal tSourceLayer As String, ByVal iSegmentIndex As Integer)
			Overlay = bOverlay
			ReDim SourceHandles(0)
			ReDim SourceLayers(0)
			ReDim SegmentIndex(0)
			SourceHandles(0) = tSourceHandle
			SourceLayers(0) = tSourceLayer
			SegmentIndex(0) = iSegmentIndex
			Application = True
		End Sub
		Public Sub New(ByVal oResBuffer As ResultBuffer)
			Const iExNo As Integer = 20
			Dim taTypedValues() As TypedValue = oResBuffer.AsArray()
			Dim shCode As Short
			Dim oValue As Object
			Dim iHandlesUB As Integer = -1
			Dim iTypedValuesUB As Integer = taTypedValues.GetUpperBound(0)
			Dim iLineIndex As Integer
			Dim iAppIndex As Integer
			Application = False
			For iIndex As Integer = 0 To iTypedValuesUB
				shCode = taTypedValues(iIndex).TypeCode
				oValue = taTypedValues(iIndex).Value
				If shCode = 1001 Then
					If Application Then
						Exit For
					End If
					If DirectCast(oValue, String) = msXDataAppName Then
						Application = True
						iAppIndex = 0
					End If
				End If
				If Application Then
					If iAppIndex = 0 Then
					ElseIf iAppIndex = 1 Then
						Overlay = zzToBoolean(oValue)
					ElseIf iAppIndex = 2 Then
						iHandlesUB = DirectCast(oValue, Integer)
						ReDim SourceHandles(iHandlesUB)
						ReDim SourceLayers(iHandlesUB)
						ReDim SegmentIndex(iHandlesUB)
					Else
						iLineIndex = (iAppIndex - 3) \ 3
						If iAppIndex Mod 3 = 0 Then
							'	AcadDocument.WriteMessage("Not mod 2 oValue=" & oValue.ToString() & ":" & oValue.GetType().ToString())
							SourceHandles(iLineIndex) = zzToHandle(oValue)
						ElseIf iAppIndex Mod 3 = 1 Then
							SourceLayers(iLineIndex) = DirectCast(oValue, String)
						Else
							'AcadDocument.WriteMessage(" mod 2 oValue=" & oValue.ToString() & ":" & oValue.GetType().ToString())
							Try
								SegmentIndex(iLineIndex) = DirectCast(oValue, Integer)
							Catch oEx As Exception
								AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
							End Try

						End If
					End If
					iAppIndex += 1
				End If
			Next
		End Sub
		Public Sub AddBuffer(ByVal tSegmentBuffer As SegmentBuffer)
			Dim iUB As Integer = SourceHandles.GetUpperBound(0)
			Dim iAddUB As Integer = tSegmentBuffer.SourceHandles.GetUpperBound(0)
			Dim iUBNew As Integer = iUB + iAddUB + 1
			Dim iNewIndex As Integer
			ReDim Preserve SourceHandles(iUBNew)
			ReDim Preserve SourceLayers(iUBNew)
			ReDim Preserve SegmentIndex(iUBNew)
			'	AcadDocument.WriteMessage("***" & CStr(iUBNew) & "**" & DispArray(SourceHandles, "SourceHandles", False))
			For iIndex As Integer = 0 To iAddUB
				iNewIndex = iUB + iIndex + 1
				SourceHandles(iNewIndex) = tSegmentBuffer.SourceHandles(iIndex)
				SourceLayers(iNewIndex) = tSegmentBuffer.SourceLayers(iIndex)
				SegmentIndex(iNewIndex) = tSegmentBuffer.SegmentIndex(iIndex)
			Next
			If Overlay AndAlso Not tSegmentBuffer.Overlay Then
				Overlay = False
			End If
		End Sub
		Public Sub AddSourceHandle(ByVal tHandle As Handle, ByVal sSourceLayer As String, ByVal iSegmentIndex As Integer)
			Dim iUB As Integer = SourceHandles.GetUpperBound(0)
			iUB += 1
			ReDim Preserve SourceHandles(iUB)
			ReDim Preserve SourceLayers(iUB)
			ReDim Preserve SegmentIndex(iUB)
			SourceHandles(iUB) = tHandle
			SourceLayers(iUB) = sSourceLayer
			SegmentIndex(iUB) = iSegmentIndex
		End Sub
		Public Function GetResBuffer() As ResultBuffer
			Dim oResBuffer As ResultBuffer = New ResultBuffer()
			Dim iHandlesUB As Integer = SourceHandles.GetUpperBound(0)
			With oResBuffer
				.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1001, msXDataAppName))
				.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1070, Convert.ToInt16(Overlay)))
				.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, iHandlesUB))
				For iIndex As Integer = 0 To iHandlesUB
					.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1005, SourceHandles(iIndex)))
					.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1003, SourceLayers(iIndex)))
					.Add(New Autodesk.AutoCAD.DatabaseServices.TypedValue(1071, SegmentIndex(iIndex)))
				Next
			End With
			Return oResBuffer
		End Function
		Private Function zzToHandle(ByVal oValue As Object) As Handle
			Dim sValue As String = DirectCast(oValue, String)
			Dim lValue As Long = Convert.ToInt64(Val("&H" & sValue))
			Return New Handle(lValue)
		End Function
		Private Function zzToBoolean(ByVal oValue As Object) As Boolean
			Dim shValue As Short = DirectCast(oValue, Int16)
			Return Convert.ToBoolean(shValue)
		End Function
		Public Shared Function zzGetOverlay(ByVal oCurve As Curve) As Boolean
			Dim oResBuffer As ResultBuffer = oCurve.XData
			If oResBuffer IsNot Nothing Then

			End If
		End Function
		Public Shared Function DispArray(ByVal oaValue() As Autodesk.AutoCAD.DatabaseServices.Handle, ByVal sTitle As String, ByVal bMsgBox As Boolean) As String
			Dim sMsg As String = String.Empty

			If oaValue Is Nothing Then
				sMsg = "Array Is Nothing"
			Else
				Try
					For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
						If iIndex <> 0 Then
							sMsg &= ","
						End If
						sMsg &= oaValue(iIndex).ToString
					Next
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
				End Try
			End If
			If bMsgBox Then
				System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
			End If
			Return sMsg

		End Function
	End Structure
End Class
