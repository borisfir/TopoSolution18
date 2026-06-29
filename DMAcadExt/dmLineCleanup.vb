Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.Gis
Imports Autodesk.Gis.Map
'Imports Autodesk.Gis.Map.Topology
Imports System.Windows.Forms
Public Class dmLineCleanup
	Const msXDataAppName As String = "TplnSourceHandles"
	Const miErrClassNo As Integer = 13000	 '  iExNo   = 10
	Public Const miDebugHandleValue1 As Integer = 14851
	Public Const miDebugHandleValue2 As Integer = 14852
   Public Shared moProgress As ProgressBar
	Private Shared mdicEntityCells As EntityCells
	Private Shared mdicCoarseEntityCells As EntityCells

	Private Shared mdicPointCells As TplnPointCells
	Private Shared mcolTinyLinks As ObjectIdCollection
	Private Shared mcolShortLinks As ObjectIdCollection
	Private Shared moTinyLinkSquareMarkBlock As DMAcadExt.MarkBlock '= New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square) 'IgnoreLink
	Private Shared moShortLinkSquareMarkBlock As DMAcadExt.MarkBlock ' = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square) 'ShortLink

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

		If DMAcadExt.AcadTransaction.LinkExists(oTopoDef.LinkLayers) Then
			System.Windows.Forms.MessageBox.Show("Entities on layers " & oTopoDef.LinkLayers & " already exist", "TownPlanner")
		Else
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
									oNewLine = DirectCast(oLine.Clone, Line)

									oNewLine.Layer = sDestLayer
									AcadTransaction.AppendEntity(oNewLine)
									AcadMapApp.ClearOD(oNewLine)
									AcadDocument.WriteDebugMessage("oNewLine ObjID=" & oNewLine.ObjectId.ToString())
									zzSetXData(oNewLine, -1, oCurve.Handle, oCurve.Layer, bOverlay)
									iLineCounter += 1
								Case AcadConst.AcadArcName
									oArc = DirectCast(oCurve, Arc)
									oNewArc = New Arc
									oNewArc.CopyFrom(oArc)
									oNewArc.Layer = sDestLayer
									AcadTransaction.AppendEntity(oNewArc)
									AcadMapApp.ClearOD(oNewArc)
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
		End If
	End Sub

	Public Shared Sub Straighten(ByVal oTopoDef As TopoDef, ByVal dTolerance As Double)
		Dim sIncludeLayers As String = oTopoDef.IncludeLayers
		Dim sDestLayer As String = oTopoDef.LinkLayers
		Straighten(sIncludeLayers, sDestLayer, dTolerance, 0.0)
	End Sub
	Public Shared Sub GetArcCount(sSourceLayers As String)
		Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
		Dim oCurve As Curve
		Dim sRXClassName As String
		Dim iArcCounter As Integer = 0
		Dim iEntityCounter As Integer = 0
		Dim iRes As Integer = 0

		For Each tAcObjID As ObjectId In dicModelSpaceObjIDs
			oCurve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForWrite, TopoDef.AddDelim(sSourceLayers))
			If oCurve IsNot Nothing Then
				sRXClassName = oCurve.GetRXClass().Name
				Select Case sRXClassName
					Case AcadConst.AcadLineName
					Case AcadConst.AcadArcName
						iRes = 1
					Case AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
						iRes = zzGetArcCount(oPolyline)
					Case AcadConst.Acad2dPolylineName
						Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
						iRes = zzGetArcCount(oPolyline2d)
					Case Else
						System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "06_347??")
				End Select
				If iRes > 0 Then
					iArcCounter += iRes
					iEntityCounter += 1
					iRes = 0
				End If
			End If
		Next tAcObjID
		DMAcadExt.AcadDocument.UpdateScreen()
		DMAcadExt.AcadDocument.WriteMessage("Modified: Entities - " & CStr(iEntityCounter) & ";  Arcs - " & CStr(iArcCounter))
	End Sub

	Public Shared Function GetArcCountByClass(oEntity As Entity, sRXClassName As String) As Integer
		Dim iRes As Integer = 0
		Select Case sRXClassName
			Case AcadConst.AcadLineName
			Case AcadConst.AcadArcName
				iRes = 1
			Case AcadConst.AcadPolylineName
				Dim oPolyline As Polyline = DirectCast(oEntity, Polyline)
				iRes = zzGetArcCount(oPolyline)
			Case AcadConst.Acad2dPolylineName
				Dim oPolyline2d As Polyline2d = DirectCast(oEntity, Polyline2d)
				iRes = zzGetArcCount(oPolyline2d)
		End Select
		Return iRes
	End Function
	Public Shared Sub CopyBlockRefs(sBlockName As String)
		Dim oAcadBlock As DMAcadExt.AcadBlock = New AcadBlock(sBlockName)
		oAcadBlock.Open()
		oAcadBlock.LoadAllReferences()
		'  DMAcadExt.AttributeData.DEBUG = False
		oAcadBlock.CopyAll()
		'  DMAcadExt.AttributeData.DEBUG = True
		'   oAcadBlock.CopyAll()
		oAcadBlock.DeleteAll()
	End Sub
	Public Shared Sub CopyLinks(sSourceLayers As String)

		Dim colLinks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks(sSourceLayers)
		Dim oCurve As Curve
		Dim oTopoDefByLayer As TopoDef = Nothing

		Dim sRXClassName As String
		Dim iArcCounter As Integer = 0
		Dim iEntityCounter As Integer = 0
		Dim iRes As Integer = 0
		'   DMAcadExt.AcadDocument.WriteMessageLog("101a: " & CStr(colLinks.Count))

		For Each tAcObjID As ObjectId In colLinks
			oCurve = AcadTransaction.GetCurve(tAcObjID, False, OpenMode.ForWrite)

			If oCurve IsNot Nothing Then
				'	oCurve.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0)
				sRXClassName = oCurve.GetRXClass().Name
				' DMAcadExt.AcadDocument.WriteMessageLog("4_39 CurveID=" & tAcObjID.ToString() & ":" & sRXClassName)
				'AcadDocument.WriteMessage("()()()()() " & CStr(sRXClassName))
				Select Case sRXClassName
					Case AcadConst.AcadLineName
						Dim oLine As Line = DirectCast(oCurve, Line)
						CopyLine(oLine)
					Case AcadConst.AcadArcName
						Dim oArc As Arc = DirectCast(oCurve, Arc)
						CopyArc(oArc)
					Case AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
						CopyPolyline(oPolyline)
					Case AcadConst.Acad2dPolylineName
						Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)

					Case Else
						System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "06_347??")
				End Select

			End If

		Next tAcObjID
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub
	Public Shared Sub Straighten(sSourceLayers As String, sNewLineLayer As String, ByVal dTolerance As Double, ByVal dMinRadius As Double, Optional dicBreakPointBlocks As IDictionary(Of Long, System.Collections.ObjectModel.Collection(Of BlockReference)) = Nothing)
		Dim colModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
		Dim oCurve As Curve
		Dim oTopoDefByLayer As TopoDef = Nothing
		Dim saLayers() As String = TopoDef.ValueSplit(sNewLineLayer)
		Dim sRXClassName As String = ")000"
		Dim iArcCounter As Integer = 0
		Dim iEntityCounter As Integer = 0
		Dim iRes As Integer = 0
		Dim sNewCurrentLayer As String = Nothing
		Dim colBlockRefs As System.Collections.ObjectModel.Collection(Of BlockReference) = Nothing
		Dim lHandleVal As Long
		If AcadTransaction.CreateLayer(saLayers, 1, True) Then

			Dim dicLayers As Dictionary(Of String, String) = zzGetLayersDictionary(sSourceLayers, sNewLineLayer)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "dicLayers", dicLayers Is Nothing, sSourceLayers, sNewLineLayer)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!dicBreakPointBlocks", dicBreakPointBlocks.Count)


			If dicLayers IsNot Nothing Then
				Dim saTest(dicLayers.Count - 1) As String
				dicLayers.Keys.CopyTo(saTest, 0)
				DMCommon.Debug.ExcelLog.SetArray(0, "dicLayers.Keys", True, saTest)
				dicLayers.Values.CopyTo(saTest, 0)

			End If


			For Each tAcObjID As ObjectId In colModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForWrite, TopoDef.AddDelim(sSourceLayers))
				If oCurve IsNot Nothing Then
					sRXClassName = oCurve.GetRXClass().Name
					If dicBreakPointBlocks IsNot Nothing Then
						lHandleVal = Convert.ToInt64(Val("&H" & oCurve.Handle.ToString()))
						dicBreakPointBlocks.TryGetValue(lHandleVal, colBlockRefs)
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!AcadPolylineName", oCurve.Handle.Value, lHandleVal, oCurve.Layer, sRXClassName, colBlockRefs Is Nothing)
					End If



					'	oCurve.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0)


					If dicLayers Is Nothing OrElse Not dicLayers.TryGetValue(oCurve.Layer, sNewCurrentLayer) Then
						sNewCurrentLayer = sNewLineLayer
					End If
					Select Case sRXClassName
						Case AcadConst.AcadLineName

						Case AcadConst.AcadArcName
							Dim oArc As Arc = DirectCast(oCurve, Arc)
							If oArc.Radius >= dMinRadius Then
								StraightenArc(oArc, dTolerance, sNewCurrentLayer)  'PartStraightenArc
								iArcCounter += 1
								iEntityCounter += 1
							End If

						Case AcadConst.AcadPolylineName
							Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)

							iRes = StraightenPolyline(oPolyline, dTolerance, sNewCurrentLayer, colBlockRefs)
						Case AcadConst.Acad2dPolylineName
							Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
							iRes = StraightenPolyline(oPolyline2d, dTolerance, sNewCurrentLayer)
						Case Else
							System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "06_347??")
					End Select
					If iRes > 0 Then
						iArcCounter += iRes
						iEntityCounter += iRes
						iRes = 0
					End If
				End If

			Next tAcObjID
			DMAcadExt.AcadDocument.UpdateScreen()
			DMAcadExt.AcadDocument.WriteMessage("Modified: Entities - " & CStr(iEntityCounter) & ";  Arcs - " & CStr(iArcCounter))

		End If
	End Sub
	Private Shared Function zzGetLayersDictionary(sSourceLayers As String, ByRef sDestLayers As String) As Dictionary(Of String, String)
		Dim saDestLayers() As String = Split(sDestLayers, ",")
		If saDestLayers.GetUpperBound(0) = 0 Then
			Return Nothing
		Else
			Dim saSourceLayers() As String = Split(sSourceLayers, ",")
			If saSourceLayers.GetUpperBound(0) = saDestLayers.GetUpperBound(0) Then
				Dim dicResult As Dictionary(Of String, String) = New Dictionary(Of String, String)()
				For iIndex As Integer = 0 To saSourceLayers.GetUpperBound(0)
					dicResult.Add(saSourceLayers(iIndex), saDestLayers(iIndex))
				Next
				Return dicResult
			Else
				sDestLayers = saDestLayers(0)
				Return Nothing
			End If
		End If
	End Function

	Public Shared Sub StraightenCurve(tAcObjID As ObjectId, ByVal dTolerance As Double, bErase As Boolean, Optional sNewLineLayer As String = "")
		Dim oCurve As Curve
		Dim iRes As Integer
		oCurve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForWrite)
		If oCurve IsNot Nothing Then
			If String.IsNullOrEmpty(sNewLineLayer) Then
				sNewLineLayer = oCurve.Layer
			End If
			'	oCurve.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0)

			'	DMAcadExt.AcadDocument.WriteMessageLog("4_39 CurveID=" & tAcObjID.ToString() & ":" & sRXClassName)

			Select Case oCurve.GetRXClass().Name
				Case AcadConst.AcadLineName
					'	System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "06_345")
				Case AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oCurve, Arc)
					StraightenArc(oArc, dTolerance, sNewLineLayer)  'PartStraightenArc

				Case AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)
					iRes = StraightenPolyline(oPolyline, dTolerance, sNewLineLayer)
				Case AcadConst.Acad2dPolylineName
					Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)
					iRes = StraightenPolyline(oPolyline2d, dTolerance, sNewLineLayer)
				Case Else
					System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "06_347??")
			End Select
			If bErase Then
				oCurve.Erase()
			End If
		End If

	End Sub
	Public Shared Function dmCleanupNew(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		MessageBox.Show("dmCleanupNew", "05_107")

		MessageBox.Show(tLayerDef.Name, "05_101New")
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)

		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			'	Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0

			Dim sCurveLayerName As String
			MessageBox.Show(tCleanupOptions.Rounding.ToString(), "25_101")
			If tCleanupOptions.Rounding <> enMerging.None OrElse tCleanupOptions.PointLine Then
				KeyPoint.SetOrigin(AcadDocument.GetExtMinPoint())
			End If
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells = New EntityCells(False)
				EntityCell.Init()
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance
			End If

			If tCleanupOptions.PointLine Then
				mdicCoarseEntityCells = New EntityCells(True, tCleanupOptions.PointLineTolerance)
				KeyPoint.CoarseRoundScale = 5.0
			End If
			Dim tMapThemeData As MapThemeData = tCleanupOptions.MapThemeData
			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()
			'	Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()
			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(tMapThemeData.LinkLayers, tMapThemeData.LineLinkLayers)

			MessageBox.Show(tMapThemeData.LinkLayers & vbCrLf & tMapThemeData.LineLinkLayers & vbCrLf & dicModelSpaceObjIDs.Count, "03_412")

			Dim sNewLayer As String = tCleanupOptions.DestLayers
			If sNewLayer Is Nothing Then
				sNewLayer = "Nothing"
			End If
			System.Windows.Forms.MessageBox.Show(sNewLayer, "05_391")
			Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)

			'	Dim oTopoDefByLayer As TopoDef = Nothing
			Dim oResTplnPoint As TPlnPoint
			'	DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)

			Try
				AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "e1502h")
			End Try

			'PLINETYPE
			''''''''''''''		Dim bOverlay As Boolean
			''''''
			''''''''''''''''''		EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			'''''''''''''''''''''''''''''	EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
			Dim saLayers() As String = TopoDef.ValueSplit(tMapThemeData.LinkLayers)
			Dim sDuplicateLayer As String
			Dim oResBuffer As ResultBuffer
			Dim tSegmentBuffer As SegmentBuffer
			Dim oRemoveDuplicatesPointArray As TplnPointArray = Nothing
			Dim bPoint As Boolean = tCleanupOptions.Rounding <> enMerging.None, bPointLine As Boolean = tCleanupOptions.PointLine
			mdicLines = New SegmentDic(tCleanupOptions.RemoveDuplicatesTolerance)


			If tCleanupOptions.RemoveDuplicates Then
				sDuplicateLayer = tCleanupOptions.DuplicateLayer
				AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)
				oRemoveDuplicatesPointArray = New TplnPointArray
			Else
				sDuplicateLayer = String.Empty
			End If
			AcadDocument.WriteMessage("SourceLayer=" & sLayersDel & "; ")
			For Each tAcObjID As ObjectId In dicModelSpaceObjIDs
				oCurve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForWrite)

				If oCurve IsNot Nothing Then
					If oCurve.Handle.Value = miDebugHandleValue1 OrElse oCurve.Handle.Value = miDebugHandleValue2 Then

						DMAcadExt.AcadDocument.WriteMessage("&&*000 " & "ID=" & tAcObjID.ToString() & ", H=" & oCurve.Handle.ToString())
					End If

					oResBuffer = oCurve.XData
					If oResBuffer IsNot Nothing Then
						tSegmentBuffer = New SegmentBuffer(oResBuffer)
					End If
					sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					'''''''''''''''''''''''' 	bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
					If True Then
						sRXClassName = oCurve.GetRXClass().Name
						If tCleanupOptions.RemoveDuplicates OrElse tCleanupOptions.Rounding <> enMerging.None Then
							AcadDocument.WriteMessage("%%%%" & oCurve.Layer & ";" & sRXClassName)
							Select Case sRXClassName
								Case AcadConst.AcadArcName
									Dim oArc As Arc = DirectCast(oCurve, Arc)
									oResTplnPoint = zzAddArcSegment(oArc, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint)
									End If
									If bPoint Then
										mdicEntityCells.AddArc(oArc, tSegmentBuffer.Overlay)
									End If
									If bPointLine Then
										mdicCoarseEntityCells.AddArc(oArc, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)

									If bPoint OrElse bPointLine Then
										zzAddPolyline(oPolyline, tSegmentBuffer.Overlay, bPoint, bPointLine)
									End If
									oCurve = Nothing
									'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)

									If bPoint OrElse bPointLine Then
										zzAddPolyline2d(oPolyline2d, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadLineName
									Dim oLine As Line = DirectCast(oCurve, Line)
									oResTplnPoint = zzAddLineSegment(oLine, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint) 'result
									End If
									If bPoint Then
										mdicEntityCells.AddLine(oLine, tSegmentBuffer.Overlay)
									End If
									If bPointLine Then
										mdicCoarseEntityCells.AddLine(oLine, tSegmentBuffer.Overlay)
									End If
								Case Else
							End Select
						End If
					End If
				End If
			Next
			MessageBox.Show("", "05_461")
			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If

			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
			If tCleanupOptions.RemoveDuplicates Then
				tCleanupResult.AddResult(tCleanupOptions.RemoveDuplicatesRowIndex, oRemoveDuplicatesPointArray, oRemoveDuplicatesPointArray.UpperBound + 1)
			End If
			DMAcadExt.AcadDocument.WriteMessage("Rounding=" & tCleanupOptions.Rounding.ToString())
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells.CalculateNew()
				EntityCell.ExecAll(bFix, tCleanupOptions.Rounding = enMerging.Rounding)

				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, EntityCell.ErrPoints, EntityCell.SourceErrors)
				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(EntityCell.SourceErrors))
				DMAcadExt.AcadDocument.WriteMessage("Max Err Distance:" & CStr(EntityCell.MaxErrDist))
				DMAcadExt.AcadDocument.WriteMessage("Max update:" & CStr(EntityCell.MaxUpdateDist))
			End If
			If tCleanupOptions.PointLine Then
				zzCheckPointAndSegments()
				tCleanupResult.AddResult(tCleanupOptions.PointLineRowindex, mdicCoarseEntityCells.ErrPoints, mdicCoarseEntityCells.SorceErrors)
			End If
			MessageBox.Show("", "05_990")
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502c")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try
	End Function
	Public Shared Function ExplodePLines(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim oRes As dmCleanupResult = New dmCleanupResult(tCleanupOptions.ExplodePLinesRowIndex)

		Dim sLayers As String = tCleanupOptions.SourceLayers
		Dim oaPoints As TplnPointArray
		If Not String.IsNullOrEmpty(sLayers) Then
			oaPoints = AcadTransaction.ExplodePLines(sLayers, bFix, False)
			oRes.AddResult(tCleanupOptions.ExplodePLinesRowIndex, oaPoints, oaPoints.Count)
		End If

		Return oRes
	End Function
	Public Shared Function ConvertArcs(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim oRes As dmCleanupResult = New dmCleanupResult(tCleanupOptions.ExplodePLinesRowIndex)

		Dim sLayers As String = tCleanupOptions.SourceLayers
		Dim oaPoints As TplnPointArray = New TplnPointArray()
		Dim oPoint As TPlnPoint

		If Not String.IsNullOrEmpty(sLayers) Then
			Dim colArcs As ICollection(Of Arc) = AcadTransaction.GetArcs(sLayers, OpenMode.ForWrite)
			For Each oArc As Arc In colArcs
				'	DMCommon.Debug.MsgBox("13_019c", oArc.Radius, tCleanupOptions.StraightenMinRadius)
				If oArc.Radius > tCleanupOptions.StraightenMinRadius AndAlso oArc.Radius * oArc.Radius * (oArc.TotalAngle - Math.Sin(oArc.TotalAngle)) * 0.5 < tCleanupOptions.StraightenMaxArea Then

					oPoint = dmLineCleanup.ConvertArcToLine(bFix, oArc, tCleanupOptions.StraightenTolerance)

				Else
					oPoint = dmLineCleanup.ConvertArcToPolyline(bFix, oArc)
					'If oPoint IsNot Nothing Then
					'	oaPoints.Add(oPoint)
					'	oArc.Erase()
					'End If
				End If
				If oPoint IsNot Nothing Then
					oaPoints.Add(oPoint)
					If bFix Then
						oArc.Erase()
					End If

				End If
			Next



			oRes.AddResult(tCleanupOptions.ConvertArcsRowIndex, oaPoints, oaPoints.Count)

		End If


		Return oRes
	End Function
	Public Shared Function RoundAll(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim oRes As dmCleanupResult = New dmCleanupResult(tCleanupOptions.RoundAllRowIndex)
		If bFix Then
			Dim sLayers As String = tCleanupOptions.SourceLayers

			Dim oaPoints As TplnPointArray = New TplnPointArray()
			If Not String.IsNullOrEmpty(sLayers) Then


				AcadTransaction.RoundAll(tCleanupOptions.SourceLayers, tCleanupOptions.BlockNames, tCleanupOptions.RoundAllDecimals)
				'oRes.AddResult(tCleanupOptions.ExplodePLinesRowIndex, oaPoints, oaPoints.Count)

			End If

		End If
		Return oRes
	End Function
	Public Shared Function dmCleanupSPoints(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sBlock As String = tCleanupOptions.MapThemeData.SPointsBlocks
		If String.IsNullOrEmpty(sBlock) Then
			sBlock = "C1610,C1611"
		End If

		'  Dim sLinkLayers As String = "pclp001,pclp004,PCLP001,PCLP004"
		Dim sLinkLayers As String = "C1650,C1660,C1662,pclp013,PCLP013"

		Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(sBlock)
		Dim dicSPoints As Dictionary(Of Decimal, TPlnPoint)
		Dim oPointsArray As TplnPointArray = New TplnPointArray()
		' Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(0)
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		Dim oResPoints As TplnPointArray = New TplnPointArray()
		Dim oCleanupCurves As CleanupCurves = New CleanupCurves()
		MessageBox.Show("sBlock: " & sBlock & vbCrLf & "", "05_100A")
		Dim colLinkObjIDs As ObjectIdCollection = AcadTransaction.GetEntitiesByLayers(sLinkLayers)
		MessageBox.Show("colLinkObjIDs: " & CStr(colLinkObjIDs.Count) & vbCrLf & "", "05_200")
		dicSPoints = DMAcadExt.AcadTransaction.CleanupRoundInserts(tBlockNames, TplnPointKey.RoundDigit, True)
		MessageBox.Show("dicSPoints: " & CStr(dicSPoints.Count) & vbCrLf & "", "05_300")
		oCleanupCurves.NetPoints = dicSPoints
		MessageBox.Show(" ", "05_400")
		oCleanupCurves.Curves = colLinkObjIDs
		MessageBox.Show("Links: " & colLinkObjIDs.Count.ToString & vbCrLf & "Points: " & dicSPoints.Count.ToString, "dmCleanupSPoints!")
		oCleanupCurves.RoundByNet()
		'''''''''''''''''''''''''''''   oCleanupCurves.CheckCloseVertices()

		tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, oCleanupCurves.ResPoints, oCleanupCurves.ResPoints.Count)
		MessageBox.Show(colLinkObjIDs.Count.ToString & vbCrLf & dicSPoints.Count.ToString & vbCrLf & oPointsArray.UpperBound.ToString, "05_126")
		Return tCleanupResult
	End Function
	Public Shared Function dmCleanupPointsLines(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim sBlock As String = tCleanupOptions.MapThemeData.SPointsBlocks
		Dim sLinkLayers As String = "pclp001,pclp004,PCLP001,PCLP004"
		Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(sBlock)
		Dim dicSPoints As Dictionary(Of Decimal, TPlnPoint)
		Dim oPointsArray As TplnPointArray = New TplnPointArray()

		Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		Dim oResPoints As TplnPointArray = New TplnPointArray()
		Dim oCleanupCurves As CleanupCurves = New CleanupCurves()
		Dim colLinkObjIDs As ObjectIdCollection = AcadTransaction.GetEntitiesByLayers(sLinkLayers)

		dicSPoints = DMAcadExt.AcadTransaction.CleanupRoundInserts(tBlockNames, TplnPointKey.RoundDigit, True)
		oCleanupCurves.NetPoints = dicSPoints
		oCleanupCurves.Curves = colLinkObjIDs
		oCleanupCurves.CheckNetPointsLines()
		tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, oCleanupCurves.ResPoints, oCleanupCurves.ResPoints.Count)
		' System.Windows.Forms.MessageBox.Show(oCleanupCurves.ResPoints.UpperBound.ToString & vbCrLf & oCleanupCurves.ResPoints.ToString, "05_143")
		Return tCleanupResult
	End Function
	Public Shared Function dmCleanupRoundingLines(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		' New Rounding
		Dim tTestMapThemeData As MapThemeData = tCleanupOptions.MapThemeData
		'  System.Windows.Forms.MessageBox.Show(tTestMapThemeData.CleanupType.ToString(), "05_261")
		System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN(tTestMapThemeData.LinkLayers, "NOTHING"), "05_262")
		Dim sLinkLayers As String = tCleanupOptions.MapThemeData.LinkLayers
		'  System.Windows.Forms.MessageBox.Show(tCleanupOptions.PointLineTolerance.ToString(), "05_263")
		Dim oLink As DBObject
		Dim oPointsArray As TplnPointArray = New TplnPointArray()

		' Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(0)
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		Dim oResPoints As TplnPointArray = New TplnPointArray()

		Dim oCleanupCurves As CleanupCurves = New CleanupCurves()
		Dim colLinkObjIDs As ObjectIdCollection = AcadTransaction.GetEntitiesByLayers(sLinkLayers)
		For Each tLinkObjID As ObjectId In colLinkObjIDs
			oLink = DMAcadExt.AcadTransaction.GetDBObject(tLinkObjID, OpenMode.ForWrite)
			oResPoints.Add(TplnSegment.Round(oLink, bFix, tCleanupOptions.PointLineDigit))
		Next





		tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, oResPoints, oResPoints.Count)
		' System.Windows.Forms.MessageBox.Show(oCleanupCurves.ResPoints.UpperBound.ToString & vbCrLf & oCleanupCurves.ResPoints.ToString, "05_143")
		Return tCleanupResult
	End Function
	Public Shared Function MergingPoints(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		Dim dShortLimit As Double = 0.1
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		Dim tMapThemeData As MapThemeData = tCleanupOptions.MapThemeData

		'31/08/21
		'	DMCommon.Debug.MsgBox("05_993Merge", "PointMerging", tCleanupOptions.Rounding, tCleanupOptions.PointLine, tCleanupOptions.SourceLayers, tCleanupOptions.DuplicateLayer, tCleanupOptions.RoundingTolerance, tCleanupOptions.PointLineTolerance, tMapThemeData.LinkLayers, tMapThemeData.LineLinkLayers, tMapThemeData.NodeBlocks, tMapThemeData.NodeLayers)
		Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(tMapThemeData.LinkLayers, "")

		Dim tExtents As Extents3d
		Dim oLinkBoundingBox As TPlnBoundingBox
		Dim oLine As Line = Nothing
		Dim oPolyline As Polyline = Nothing
		Dim dLinkMaxSize As Double
		mdicPointCells = New TplnPointCells()

		moTinyLinkSquareMarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Square)
		moShortLinkSquareMarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Square)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)

		'	Dim oShortObjectSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square)
		TplnPointKeyULong.SetOrigin(AcadDocument.GetExtMinPoint())
		TplnPointKeyULong.Tolerance = tCleanupOptions.RoundingTolerance
		mcolTinyLinks = New ObjectIdCollection()
		mcolShortLinks = New ObjectIdCollection()
		TplnPointCell.Init(tCleanupOptions.RoundingTolerance)
		EntityPoint.Init()
		Dim oResMergingMarkPoints As TplnPointArray = New TplnPointArray()
		Dim oResMergingFixPoints As TplnPointArray = New TplnPointArray()
		Dim oResShortLinesMarkPoints As TplnPointArray = New TplnPointArray()
		Dim oResShortLinesFixPoints As TplnPointArray = New TplnPointArray()

		Dim oTPlnPoint As TPlnPoint
		dShortLimit = tCleanupOptions.ShortLinesTolerance
		DMCommon.Debug.MsgBox("13_142c", dicModelSpaceObjIDs.Count, tCleanupOptions.PointMergingRowIndex, tCleanupOptions.ShortLinesTolerance, tCleanupOptions.ShortLinesRowIndex)
		For Each tAcObjID As ObjectId In dicModelSpaceObjIDs
			If AcadTransaction.GetUD_Link(tAcObjID, OpenMode.ForRead, oLine, oPolyline) Then

				If oLine IsNot Nothing Then
					Try
						tExtents = oLine.GeometricExtents
					Catch oEx As Exception

					End Try

					oLinkBoundingBox = New TPlnBoundingBox(tExtents)

					dLinkMaxSize = oLinkBoundingBox.MaxSize
					If dLinkMaxSize > 2.0 * TplnPointCell.CellSize Then

						mdicPointCells.AddLine(oLine, 2)


						If dLinkMaxSize <= dShortLimit Then
							mcolShortLinks.Add(tAcObjID)
							oTPlnPoint = New TPlnPoint(oLine.StartPoint, oLine.EndPoint)
							moShortLinkSquareMarkBlock.MarkPoint(oTPlnPoint.AcGePoint, 141S)
							oResShortLinesMarkPoints.Add(oTPlnPoint)
						End If
					Else

						mcolTinyLinks.Add(tAcObjID)

						If Not bFix Then
							oResMergingMarkPoints.Add(oLine.StartPoint)
							oTPlnPoint = New TPlnPoint(oLine.StartPoint, oLine.EndPoint)
							moTinyLinkSquareMarkBlock.MarkPoint(oTPlnPoint.AcGePoint, 41S)
						End If
					End If

				ElseIf oPolyline IsNot Nothing Then
					If oPolyline.NumberOfVertices = 2 Then
						Try
							tExtents = oPolyline.GeometricExtents
						Catch oEx As Exception

						End Try
						oLinkBoundingBox = New TPlnBoundingBox(tExtents)
						dLinkMaxSize = oLinkBoundingBox.MaxSize

						If dLinkMaxSize > 2.0 * TplnPointCell.CellSize Then

							mdicPointCells.AddPolyline(oPolyline, 2, 3)

							If dLinkMaxSize <= dShortLimit Then
								mcolShortLinks.Add(tAcObjID)
								moShortLinkSquareMarkBlock.MarkPoint(oPolyline.StartPoint, 41S)
							End If
						Else
							mcolTinyLinks.Add(tAcObjID)
							If Not bFix Then
								moShortLinkSquareMarkBlock.MarkPoint(oLine.StartPoint, 18S)
							End If

							If Not bFix Then

								moShortLinkSquareMarkBlock.MarkPoint(oPolyline.StartPoint, 41S)
							End If

						End If
					Else
						mdicPointCells.AddPolyline(oPolyline, 2, 3)
					End If

				End If
			Else
				DMCommon.Debug.ExcelLog.SetNextValue(0, "GetUD_Link Failed")
			End If
		Next
		'mdicPointCells.PrintSummary()
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!BeforeGetBlockRefsNew", tMapThemeData.NodeBlocks, tMapThemeData.NodeLayers, "EndOfRow")
		If tMapThemeData.HasNodes Then


			dicModelSpaceObjIDs = DMAcadExt.AcadTransaction.GetBlockRefsNew(tMapThemeData.NodeBlocks, tMapThemeData.NodeLayers)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterGetBlockRefsNew")
			Dim oBlockRef As BlockReference
			Dim iPriority As Integer
			'	DMCommon.Debug.MsgBox("13_142d", mdicPointCells.Count, dicModelSpaceObjIDs.Count)
			For Each tAcObjID As ObjectId In dicModelSpaceObjIDs
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead)
				Select Case oBlockRef.Name
					Case "C1610", "C1615", "C1616"
						iPriority = 1
					Case Else
						iPriority = 4
				End Select
				mdicPointCells.AddBlockRef(oBlockRef, iPriority)
			Next
		End If
		Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)


		'	DMCommon.Debug.MsgBox("13_142", tCleanupOptions.RoundingRowIndex, mdicPointCells.Count)

		mdicPointCells.CalculateCluster()
		mdicPointCells.PrintSummary()

		AcadDocument.WriteMessage("Tiny Links:" & CStr(mcolTinyLinks.Count))
		AcadDocument.WriteMessage("Short Links:" & CStr(mcolShortLinks.Count))

		'''''''''''''	mdicPointCells.DumpToExcel()
		DMCommon.Debug.ExcelLog.SetNextValue(4, "DUMP", "---------")


		mdicPointCells.CleanupClusters(bFix, oResMergingMarkPoints, oResMergingFixPoints)

		DMCommon.Debug.ExcelLog.SetNextValue(5, "Update", oResMergingMarkPoints.Count, oResMergingFixPoints.Count, "---------")
		tCleanupResult.AddResultNew(tCleanupOptions.RoundingRowIndex, oResMergingMarkPoints, oResMergingFixPoints)
		tCleanupResult.AddResultNew(tCleanupOptions.ShortLinesRowIndex, oResShortLinesMarkPoints, oResShortLinesFixPoints)

		'	ddd
		If bFix Then
			DMAcadExt.AcadTransaction.EraseDBObjects(mcolTinyLinks)
		End If
		Return tCleanupResult
	End Function
	Public Shared Function dmCleanupA(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		'Merging Points - Yes
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		Dim tMapThemeData As MapThemeData = tCleanupOptions.MapThemeData
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)
		'MessageBox.Show("dmCleanupA", "05_992")
		DMCommon.Debug.MsgBox("05_992", "dmCleanupA", tCleanupOptions.Rounding, tCleanupOptions.PointLine, tCleanupOptions.SourceLayers, tCleanupOptions.DuplicateLayer, tCleanupOptions.RoundingTolerance, tCleanupOptions.PointLineTolerance, tMapThemeData.LinkLayers, tMapThemeData.LineLinkLayers, tMapThemeData.NodeBlocks, tMapThemeData.NodeLayers)
		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			'	Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0

			'     Dim sCurveLayerName As String
			'   MessageBox.Show(tCleanupOptions.Rounding.ToString(), "25_103")
			If tCleanupOptions.Rounding <> enMerging.None OrElse tCleanupOptions.PointLine Then
				KeyPoint.SetOrigin(AcadDocument.GetExtMinPoint())

				'  MessageBox.Show("dmCleanup" & vbCrLf & tCleanupOptions.Rounding.ToString() & vbCrLf & tCleanupOptions.PointLine.ToString() & vbCrLf & tCleanupOptions.RemoveDuplicates.ToString() & vbCrLf & tCleanupOptions.RoundingTolerance.ToString() & vbCrLf & tCleanupOptions.PointLineTolerance.ToString(), "05_102")

				mdicEntityCells = New EntityCells(False)
				If tCleanupOptions.PointLineTolerance <> 0.0 Then
					EntityCell.PointLineTolerance = tCleanupOptions.PointLineTolerance
				End If
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance

				EntityCell.Init()
				'   
			End If
			If tCleanupOptions.PointLine Then
				'	mdicCoarseEntityCells = New EntityCells(True, tCleanupOptions.PointLineTolerance)
				KeyPoint.CoarseRoundScale = 5
				'    MessageBox.Show(KeyPoint.Tolerance.ToString() & vbCrLf & tCleanupOptions.PointLineTolerance.ToString(), "05_175")
				mdicEntityCells.ArcStep = tCleanupOptions.PointLineTolerance * 0.5
				'    EntityCell.UpdateDist
			End If

			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()


			Dim sNewLayer As String = tCleanupOptions.DestLayers
			If sNewLayer Is Nothing Then
				sNewLayer = "Nothing"
			End If
			'     System.Windows.Forms.MessageBox.Show(sNewLayer, "05_391")
			'	Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)

			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinksNew(tMapThemeData.LinkLayers, "")
			'	Dim oTopoDefByLayer As TopoDef = Nothing
			'  Dim oResTplnPoint As TPlnPoint
			'	DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)

			Try
				AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "e1502h")
			End Try

			'PLINETYPE
			''''''''''''''		Dim bOverlay As Boolean
			''''''
			''''''''''''''''''		EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			'''''''''''''''''''''''''''''	EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
			Dim saLayers() As String = TopoDef.ValueSplit(tMapThemeData.LinkLayers)
			Dim sDuplicateLayer As String
			'   Dim oResBuffer As ResultBuffer
			Dim tSegmentBuffer As SegmentBuffer
			Dim oRemoveDuplicatesPointArray As TplnPointArray = Nothing
			Dim bPoint As Boolean = tCleanupOptions.Rounding <> enMerging.None, bPointLine As Boolean = tCleanupOptions.PointLine
			mdicLines = New SegmentDic(tCleanupOptions.RemoveDuplicatesTolerance)


			If tCleanupOptions.RemoveDuplicates Then
				sDuplicateLayer = tCleanupOptions.DuplicateLayer
				AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)
				oRemoveDuplicatesPointArray = New TplnPointArray
			Else
				sDuplicateLayer = String.Empty
			End If
			AcadDocument.WriteMessage("SourceLayer=" & tCleanupOptions.SourceLayers & "; ")
			' MessageBox.Show(dicModelSpaceObjIDs.Count.ToString(), "05_990a")
			Dim iProgressCount As Integer = 0
			moProgress.Visible = True
			moProgress.Maximum = dicModelSpaceObjIDs.Count
			moProgress.Value = 0
			For Each tAcObjID As ObjectId In dicModelSpaceObjIDs

				'  AcadDocument.WriteMessage("SourceLayer=" & tObjID.ToString() & "; ")
				oCurve = AcadTransaction.GetCurve(tAcObjID, True, OpenMode.ForWrite)

				If oCurve IsNot Nothing Then
					'   If oCurve.Handle.Value = miDebugHandleValue1 OrElse oCurve.Handle.Value = miDebugHandleValue2 Then

					''''''''''''DMAcadExt.AcadDocument.WriteMessage("&&*000 " & "ID=" & tObjID.ToString() & ", H=" & oCurve.Handle.ToString())
					'End If

					'   oResBuffer = oCurve.XData
					'  If oResBuffer IsNot Nothing Then
					'tSegmentBuffer = New SegmentBuffer(oResBuffer)
					' End If
					'    sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					'''''''''''''''''''''''' 	bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
					' If sLayersDel.Contains(sCurveLayerName) Then
					sRXClassName = oCurve.GetRXClass().Name
					If iProgressCount >= 84 Then
						'  MessageBox.Show(iProgressCount.ToString() & vbCrLf & sRXClassName & vbCrLf & oCurve.Handle.ToString(), "05_991c")


					End If
					If tCleanupOptions.RemoveDuplicates OrElse tCleanupOptions.Rounding <> enMerging.None OrElse tCleanupOptions.PointLine Then

						'  AcadDocument.WriteMessage("%%%%" & sCurveLayerName & ";" & sRXClassName)
						Select Case sRXClassName
							Case AcadConst.AcadArcName
								Dim oArc As Arc = DirectCast(oCurve, Arc)

								If bPoint OrElse bPointLine Then
									mdicEntityCells.AddArc(oArc, tSegmentBuffer.Overlay)
								End If

							Case AcadConst.AcadPolylineName
								Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)

								If bPoint OrElse bPointLine Then
									zzAddPolyline(oPolyline, tSegmentBuffer.Overlay, bPoint, bPointLine)
								End If
								oCurve = Nothing
								'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
							Case AcadConst.Acad2dPolylineName
								Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)

								If bPoint OrElse bPointLine Then
									''''''''  zzAddPolyline2d(oPolyline2d, tSegmentBuffer.Overlay)
								End If
							Case AcadConst.AcadLineName
								Dim oLine As Line = DirectCast(oCurve, Line)

								If bPoint OrElse bPointLine Then
									mdicEntityCells.AddLine(oLine, tSegmentBuffer.Overlay)
								End If

							Case Else
						End Select
					End If
				End If
				' End If
				iProgressCount += 1
				If iProgressCount Mod 10 = 0 Then
					moProgress.Increment(10)
				End If
			Next

			dicModelSpaceObjIDs = DMAcadExt.AcadTransaction.GetBlockRefsNew(tMapThemeData.NodeBlocks, tMapThemeData.NodeLayers)
			Dim oBlockRef As BlockReference
			Dim iPriority As Integer
			For Each tAcObjID As ObjectId In dicModelSpaceObjIDs
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead)
				Select Case oBlockRef.Name
					Case "C1610", "C1615", "C1616"
						iPriority = 1
					Case Else
						iPriority = 4
				End Select
				mdicEntityCells.AddBlockRef(oBlockRef, iPriority)
			Next


			mdicEntityCells.CalculateNew()

			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If
			'  MessageBox.Show("", "05_999a")
			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
			If tCleanupOptions.RemoveDuplicates Then
				MessageBox.Show(tCleanupOptions.Rounding.ToString(), "05_470")
				tCleanupResult.AddResult(tCleanupOptions.RemoveDuplicatesRowIndex, oRemoveDuplicatesPointArray, oRemoveDuplicatesPointArray.Count)
			End If
			DMAcadExt.AcadDocument.WriteMessage("Rounding=" & tCleanupOptions.Rounding.ToString())
			If tCleanupOptions.Rounding <> enMerging.None OrElse tCleanupOptions.PointLine Then
				'  MessageBox.Show(tCleanupOptions.Rounding.ToString() & vbCrLf & mdicEntityCells.Count.ToString(), "05_471")
				'''''''''''  mdicEntityCells.Calculate()
				zzSetLayer()

				'   MessageBox.Show(tCleanupOptions.Rounding.ToString() & vbCrLf & mdicEntityCells.Count.ToString(), "05_472")
				''''''''''''''''''''   mdicEntityCells.DrawIntersectingLinesCells()
				''''''''''''''   mdicEntityCells.DrawCurvePointsCells()
				'   mdicEntityCells.TestCells()
				'  MessageBox.Show(tCleanupOptions.Rounding.ToString(), "05_484")
				EntityCell.ExecAll(bFix, tCleanupOptions.Rounding = enMerging.Rounding)
				'  MessageBox.Show("", "05_999b")
				CurvePoint.RemoveZeroLenSegments()
				'  MessageBox.Show("", "05_999c")
				'   mdicEntityCells.TestCells()
				''''''''''''''''''''''''  EntityCell.DrawNodes()
				'''''''''''''''    mdicEntityCells.TestNeigbours()
				EntityCell.MarkErrPointLines()
				'   MessageBox.Show("", "05_999d")
				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, EntityCell.ErrPoints, EntityCell.SourceErrors)

				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(EntityCell.SourceErrors))

				DMAcadExt.AcadDocument.WriteMessage("Max Err Distance:" & CStr(EntityCell.MaxErrDist))
				DMAcadExt.AcadDocument.WriteMessage("Max update:" & CStr(EntityCell.MaxUpdateDist))

				moProgress.Visible = False
			End If

			If tCleanupOptions.PointLine Then

				''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   zzCheckPointAndSegments()

				tCleanupResult.AddResult(tCleanupOptions.PointLineRowindex, EntityCell.GetErrPointLines(), EntityCell.ErrPointLinesCount)
			End If
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502d")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try
	End Function
	Public Shared Function dmCleanup(ByVal bFix As Boolean, ByVal tCleanupOptions As dmCleanupOptions) As DMAcadExt.dmCleanupResult
		'Merging Points - Yes
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		Dim oCurve As Curve
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(msXDataAppName)
		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean

		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)
		MessageBox.Show("dmCleanup", "05_992")
		sLayersDel = TopoDef.AddDelim(tCleanupOptions.SourceLayers)
		Try
			Dim sRXClassName As String
			'	Dim bRoundingCond As Boolean = tCleanupOptions.RoundingTolerance > 0.0

			Dim sCurveLayerName As String
			'   MessageBox.Show(tCleanupOptions.Rounding.ToString(), "25_103")
			If tCleanupOptions.Rounding <> enMerging.None OrElse tCleanupOptions.PointLine Then
				KeyPoint.SetOrigin(AcadDocument.GetExtMinPoint())
			End If
			'     MessageBox.Show("dmCleanup" & vbCrLf & tCleanupOptions.Rounding.ToString() & vbCrLf & tCleanupOptions.PointLine.ToString() & vbCrLf & tCleanupOptions.RemoveDuplicates.ToString() & vbCrLf & tCleanupOptions.RoundingTolerance.ToString() & vbCrLf & tCleanupOptions.PointLineTolerance.ToString(), "05_102")
			If tCleanupOptions.Rounding <> enMerging.None Then
				mdicEntityCells = New EntityCells(False)
				KeyPoint.Tolerance = tCleanupOptions.RoundingTolerance
				EntityCell.PointLineTolerance = tCleanupOptions.PointLineTolerance
				EntityCell.Init()
				'   
			End If
			If tCleanupOptions.PointLine Then
				'	mdicCoarseEntityCells = New EntityCells(True, tCleanupOptions.PointLineTolerance)
				KeyPoint.CoarseRoundScale = 5
				'  MessageBox.Show(KeyPoint.Tolerance.ToString() & vbCrLf & tCleanupOptions.PointLineTolerance.ToString(), "05_175")
				mdicEntityCells.ArcStep = tCleanupOptions.PointLineTolerance * 0.5
				'    EntityCell.UpdateDist
			End If

			Dim oStraightenPointArray As TplnPointArray = New TplnPointArray()
			Dim dicModelSpaceObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetModelSpaceObjects()

			Dim sNewLayer As String = tCleanupOptions.DestLayers
			If sNewLayer Is Nothing Then
				sNewLayer = "Nothing"
			End If
			'     System.Windows.Forms.MessageBox.Show(sNewLayer, "05_391")
			'	Dim saNewLayers() As String = TopoDef.ValueSplit(sNewLayer)
			Dim tMapThemeData As MapThemeData = tCleanupOptions.MapThemeData
			'	Dim oTopoDefByLayer As TopoDef = Nothing
			Dim oResTplnPoint As TPlnPoint
			'	DMAcadExt.AcadTransaction.zzGetLayers(1, saNewLayers, True)
			'	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sNewLayer, 1, enLayerFunction.CleanupErrMarks, True, True, True)

			Try
				AcadDocument.WriteMessage("ClenupMethods=" & tCleanupOptions.ClenupMethods.ToString() & "; new layer='" & sNewLayer & "'")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "e1502h")
			End Try

			'PLINETYPE
			''''''''''''''		Dim bOverlay As Boolean
			''''''
			''''''''''''''''''		EntityCell.SourceTopoDef = oTopoDef.BaseOrSourceTopoDef
			'''''''''''''''''''''''''''''	EntityCell.OverlayTopoDef = oTopoDef.OverlayTopoDef
			Dim saLayers() As String = TopoDef.ValueSplit(tMapThemeData.LinkLayers)
			Dim sDuplicateLayer As String
			'	Dim oResBuffer As ResultBuffer
			Dim tSegmentBuffer As SegmentBuffer
			Dim oRemoveDuplicatesPointArray As TplnPointArray = Nothing
			Dim bPoint As Boolean = tCleanupOptions.Rounding <> enMerging.None, bPointLine As Boolean = tCleanupOptions.PointLine
			mdicLines = New SegmentDic(tCleanupOptions.RemoveDuplicatesTolerance)


			If tCleanupOptions.RemoveDuplicates Then
				sDuplicateLayer = tCleanupOptions.DuplicateLayer
				AcadTransaction.CreateLayer(sDuplicateLayer, 1, enLayerFunction.Default, True)
				oRemoveDuplicatesPointArray = New TplnPointArray
			Else
				sDuplicateLayer = String.Empty
			End If
			AcadDocument.WriteMessage("SourceLayer=" & tCleanupOptions.SourceLayers & "; ")
			MessageBox.Show(dicModelSpaceObjIDs.Count.ToString(), "05_990a")
			For Each tObjID As ObjectId In dicModelSpaceObjIDs
				'  AcadDocument.WriteMessage("SourceLayer=" & tObjID.ToString() & "; ")
				oCurve = AcadTransaction.GetCurve(tObjID, True, OpenMode.ForWrite)

				If oCurve IsNot Nothing Then
					'   If oCurve.Handle.Value = miDebugHandleValue1 OrElse oCurve.Handle.Value = miDebugHandleValue2 Then

					''''''''''''DMAcadExt.AcadDocument.WriteMessage("&&*000 " & "ID=" & tObjID.ToString() & ", H=" & oCurve.Handle.ToString())
					'End If

					'   oResBuffer = oCurve.XData
					'  If oResBuffer IsNot Nothing Then
					'tSegmentBuffer = New SegmentBuffer(oResBuffer)
					' End If
					sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
					'''''''''''''''''''''''' 	bOverlay = oTopoDef.GetTopoDefByLayer(sCurveLayerName, oTopoDefByLayer)
					If sLayersDel.Contains(sCurveLayerName) Then
						sRXClassName = oCurve.GetRXClass().Name
						If tCleanupOptions.RemoveDuplicates OrElse tCleanupOptions.Rounding <> enMerging.None Then
							'  AcadDocument.WriteMessage("%%%%" & sCurveLayerName & ";" & sRXClassName)
							Select Case sRXClassName
								Case AcadConst.AcadArcName
									Dim oArc As Arc = DirectCast(oCurve, Arc)
									oResTplnPoint = zzAddArcSegment(oArc, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint)
									End If
									If bPoint Then
										mdicEntityCells.AddArc(oArc, tSegmentBuffer.Overlay)
									End If
									If False And bPointLine Then
										mdicCoarseEntityCells.AddArc(oArc, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oCurve, Polyline)

									If bPoint OrElse bPointLine Then
										zzAddPolyline(oPolyline, tSegmentBuffer.Overlay, bPoint, bPointLine)
									End If
									oCurve = Nothing
									'	iStraightenCounter += StraightenPolyline(oPolyline, bFix, tCleanupOptions.StraightenTolerance, tCleanupOptions.ClenupMethods)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oCurve, Polyline2d)

									If bPoint OrElse bPointLine Then
										zzAddPolyline2d(oPolyline2d, tSegmentBuffer.Overlay)
									End If
								Case AcadConst.AcadLineName
									Dim oLine As Line = DirectCast(oCurve, Line)
									oResTplnPoint = zzAddLineSegment(oLine, bFix, sDuplicateLayer)
									If oResTplnPoint IsNot Nothing Then
										oRemoveDuplicatesPointArray.Add(oResTplnPoint) 'result
									End If
									If bPoint Then
										mdicEntityCells.AddLine(oLine, tSegmentBuffer.Overlay)
									End If
									If False And bPointLine Then
										mdicCoarseEntityCells.AddLine(oLine, tSegmentBuffer.Overlay)
									End If
								Case Else
							End Select
						End If
					End If
				End If
			Next
			MessageBox.Show(mdicEntityCells.Count.ToString(), "05_990b")
			mdicEntityCells.CalculateNew()
			MessageBox.Show(mdicEntityCells.Count.ToString(), "05_990c")
			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.PrintSummary()
			End If
			'  MessageBox.Show("", "05_999a")
			Dim tCleanupResult As DMAcadExt.dmCleanupResult = New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
			If tCleanupOptions.RemoveDuplicates Then
				MessageBox.Show(tCleanupOptions.Rounding.ToString(), "05_470")
				tCleanupResult.AddResult(tCleanupOptions.RemoveDuplicatesRowIndex, oRemoveDuplicatesPointArray, oRemoveDuplicatesPointArray.UpperBound + 1)
			End If
			DMAcadExt.AcadDocument.WriteMessage("Rounding=" & tCleanupOptions.Rounding.ToString())
			If tCleanupOptions.Rounding <> enMerging.None Then
				'  MessageBox.Show(tCleanupOptions.Rounding.ToString() & vbCrLf & mdicEntityCells.Count.ToString(), "05_471")
				'''''''''''  mdicEntityCells.Calculate()
				zzSetLayer()

				'   MessageBox.Show(tCleanupOptions.Rounding.ToString() & vbCrLf & mdicEntityCells.Count.ToString(), "05_472")
				'''''''''''''   mdicEntityCells.DrawIntersectingLinesCells()
				''''''''''''''   mdicEntityCells.DrawCurvePointsCells()
				'   mdicEntityCells.TestCells()
				'  MessageBox.Show(tCleanupOptions.Rounding.ToString(), "05_484")
				EntityCell.ExecAll(bFix, tCleanupOptions.Rounding = enMerging.Rounding)
				CurvePoint.RemoveZeroLenSegments()
				'   mdicEntityCells.TestCells()
				EntityCell.DrawNodes()
				mdicEntityCells.TestNeigbours()
				EntityCell.MarkErrPointLines()
				tCleanupResult.AddResult(tCleanupOptions.RoundingRowIndex, EntityCell.ErrPoints, EntityCell.SourceErrors)

				DMAcadExt.AcadDocument.WriteMessage("Nodes:" & CStr(EntityCell.NodesCount) & "; Errs:" & CStr(EntityCell.SourceErrors))

				DMAcadExt.AcadDocument.WriteMessage("Max Err Distance:" & CStr(EntityCell.MaxErrDist))
				DMAcadExt.AcadDocument.WriteMessage("Max update:" & CStr(EntityCell.MaxUpdateDist))


			End If

			If tCleanupOptions.PointLine Then

				''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   zzCheckPointAndSegments()

				tCleanupResult.AddResult(tCleanupOptions.PointLineRowindex, EntityCell.GetErrPointLines(), EntityCell.ErrPointLinesCount)
			End If
			Return tCleanupResult
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1502b")
			Return New DMAcadExt.dmCleanupResult(dmCleanupOptions.dmActionsUB)
		End Try
	End Function
	Private Shared Sub zzSetLayer()

		Dim tLayerDef As AcadLayerDef = New AcadLayerDef(1, enLayerFunction.CleanupErrMarks)
		Dim bCurrentLayerOK As Boolean
		'''''''''   EntityCell.CircleMarkBlock = New MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLayerDef, True, True, True, True)

	End Sub

	Private Shared Sub zzCheckPointAndSegments()
		Dim oTestCurve As Curve
		Dim lTestHandleValue As Long = &H245
		Dim bTestMark As Boolean
		DMAcadExt.AcadDocument.WriteMessage("Src Lines:" & CStr(mdicLines.Count))

		For Each oCurveSegment As TplnCurveSegment In mdicLines.Keys
			bTestMark = False
			oTestCurve = mdicLines.Item(oCurveSegment)
			If oTestCurve IsNot Nothing AndAlso oTestCurve.Handle.Value = lTestHandleValue Then
				bTestMark = True
			End If
			mdicCoarseEntityCells.AddSegment(oCurveSegment, bTestMark)

		Next
	End Sub
	Public Shared Sub PartStraightenArc(ByVal oArc As Arc, ByVal dTolerance As Double, ByVal sNewLineLayer As String)
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))
		Dim oNewPolyline As Polyline = New Polyline(4)
		Dim dAngle As Double
		Dim dArcAngleNew As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim dBulge As Double
		dTolerAngle = oArc.TotalAngle / iPart
		dArcAngleNew = Math.Abs(oArc.TotalAngle) - dTolerAngle - dTolerAngle
		DMAcadExt.AcadDocument.WriteMessageLog("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		oNewPolyline.AddVertexAt(0, TPlnPoint.Point3dTo2d(oArc.StartPoint), 0.0, 0.0, 0.0)

		dAngle = oArc.StartAngle + dTolerAngle
		dBulge = Math.Tan(0.25 * dArcAngleNew)
		tPoint = New Autodesk.AutoCAD.Geometry.Point2d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle))
		oNewPolyline.AddVertexAt(1, tPoint, dBulge, 0.0, 0.0)
		dAngle = oArc.EndAngle - dTolerAngle
		tPoint = New Autodesk.AutoCAD.Geometry.Point2d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle))
		oNewPolyline.AddVertexAt(2, tPoint, 0.0, 0.0, 0.0)
		oNewPolyline.AddVertexAt(3, TPlnPoint.Point3dTo2d(oArc.EndPoint), 0.0, 0.0, 0.0)

		If sNewLineLayer.Length <> 0 Then
			oNewPolyline.Layer = sNewLineLayer
		End If
		Try
			AcadTransaction.AppendEntity(oNewPolyline, False)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - StraightenArc")
		End Try
	End Sub
	Public Shared Sub StraightenArc(ByVal oArc As Arc, ByVal dTolerance As Double, ByVal sNewLineLayer As String)
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))
		Dim oNewPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d

		dTolerAngle = oArc.TotalAngle / iPart
		''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
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
		Try
			AcadTransaction.AppendEntity(oNewPolyline, False)

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - StraightenArc")

		End Try


	End Sub
	Public Shared Function ConvertArcToLine(bFix As Boolean, ByVal oArc As Arc, ByVal dTolerance As Double, Optional ByVal sNewLineLayer As String = Nothing) As TPlnPoint
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)

		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)

		'	Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))

		Dim oPoint As TPlnPoint = Nothing
		Dim oLine As Line
		'	Dim oNewPolyline As Polyline = New Polyline(iPart + 1)
		'Dim dAngle As Double
		Dim tStartPoint As Autodesk.AutoCAD.Geometry.Point3d = oArc.StartPoint
		'Dim tEndPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim sResLayer As String
		'Dim oTplnArc As TplnArc
		If String.IsNullOrEmpty(sNewLineLayer) Then
			sResLayer = oArc.Layer
		Else
			sResLayer = sNewLineLayer
		End If

		oPoint = New TPlnPoint(oArc.StartPoint)
		''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		If bFix Then


			oLine = New Line(oArc.StartPoint, oArc.EndPoint)
			oLine.Layer = sResLayer
			Try
				AcadTransaction.AppendEntity(oLine, False)

			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - ConvertArcToLines")

			End Try


		End If


		Return oPoint


	End Function
	Public Shared Function ConvertArcToLine_Question(bFix As Boolean, ByVal oArc As Arc, ByVal dTolerance As Double, Optional ByVal sNewLineLayer As String = Nothing) As TPlnPoint
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)

		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)

		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))

		Dim oPoint As TPlnPoint = Nothing
		Dim oLine As Line
		'	Dim oNewPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tStartPoint As Autodesk.AutoCAD.Geometry.Point3d = oArc.StartPoint
		Dim tEndPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim sResLayer As String
		Dim oTplnArc As TplnArc
		If String.IsNullOrEmpty(sNewLineLayer) Then
			sResLayer = oArc.Layer
		Else
			sResLayer = sNewLineLayer
		End If

		dTolerAngle = oArc.TotalAngle / iPart
		oTplnArc = New TplnArc(oArc, True)
		oPoint = New TPlnPoint(oTplnArc.GetMidPoint())
		''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		If bFix Then
			For iIndex As Integer = 1 To iPart
				dAngle = oArc.StartAngle + iIndex * dTolerAngle
				tEndPoint = New Autodesk.AutoCAD.Geometry.Point3d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle), 0.0)
				oLine = New Line(tStartPoint, tEndPoint)
				oLine.Layer = sResLayer
				Try
					AcadTransaction.AppendEntity(oLine, False)


				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - ConvertArcToLines")

				End Try
				tStartPoint = tEndPoint
			Next


		End If


		Return oPoint


	End Function
	Public Shared Function ConvertArcToPolyline(bFix As Boolean, ByVal oArc As Arc, Optional ByVal sNewLineLayer As String = Nothing) As TPlnPoint
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = TPlnPoint.Point3dTo2d(oArc.Center)

		'	Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)

		'Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(oArc.TotalAngle / dTolerAngle))

		Dim oPoint As TPlnPoint = Nothing
		Dim oPolyline As Polyline


		Dim sResLayer As String
		Dim oTplnArc As TplnArc
		If String.IsNullOrEmpty(sNewLineLayer) Then
			sResLayer = oArc.Layer
		Else
			sResLayer = sNewLineLayer
		End If


		oTplnArc = New TplnArc(oArc, True)
		oPoint = New TPlnPoint(oTplnArc.GetMidPoint())
		''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog("Res=" & CStr(iPart) & "," & CStr(dTolerAngle))
		If bFix Then




			oPolyline = New Polyline(2)
			oPolyline.Layer = sResLayer
			oPolyline.AddVertexAt(0, oTplnArc.StartPoint, oTplnArc.Bulge, 0.0, 0.0)
			oPolyline.AddVertexAt(1, oTplnArc.EndPoint, 0.0, 0.0, 0.0)

			Try
				AcadTransaction.AppendEntity(oPolyline, False)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - ConvertArcToLines")

			End Try




		End If


		Return oPoint
		'dSegmentArea=R*R*(A-Sin(A))* 0.5

	End Function
	Public Shared Function StraightenPolyline(ByVal oPolyline2d As Polyline2d, ByVal dTolerance As Double, ByVal sNewLineLayer As String) As Integer
		Dim dBulge As Double = 0.0
		Dim dBulge_T, dBulge_F As Double
		Dim oArc2d_T As CircularArc2d = Nothing
		Dim oArc2d_F As CircularArc2d = Nothing
		Dim tCenterPoint As Point2d
		Dim oTplnArc As DMAcadExt.TplnArc = Nothing
		Dim bVertexIsNotFirst As Boolean = False
		Dim tAcObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing

		Dim oaInnerPoints() As Point2d
		Dim tPriorPoint, tCurrentPoint2d As Point2d
		Dim tCurrentPoint3d As Point3d
		Dim iArcCounter As Integer
		Dim iSegmentCounter As Integer

		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim colNewPoints As Point3dCollection = New Point3dCollection()
		Dim colNewBulges As DoubleCollection = New DoubleCollection()
		'	Dim oaResPoints As TplnPointArray = New TplnPointArray()
		Try
			DMAcadExt.AcadDocument.WriteMessageLog("4_89b Normal=" & CStr(oPolyline2d.Normal.Z))
			'	oPolyline2d.Normal = New Vector3d(0.0, 0.0, -1.0)
			'	DMAcadExt.AcadDocument.WriteMessageLog("4_89a Normal=" & CStr(oPolyline2d.Normal.Z))
			Do While oColEnum.MoveNext()
				tAcObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForWrite)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				DMAcadExt.AcadUtil.DispAcadEnt(iSegmentCounter, oVertex2d)
				tCurrentPoint3d = oVertex2d.Position
				tCurrentPoint2d = TPlnPoint.Point3dTo2d(tCurrentPoint3d)
				If dBulge <> 0.0 Then 'Prior Bulge
					oArc2d_T = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, True)
					oArc2d_F = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, False)
					dBulge_T = Math.Tan(oArc2d_T.EndAngle / 4.0)
					dBulge_F = Math.Tan(oArc2d_F.EndAngle / 4.0)
					If iSegmentCounter = 23 Then
						DMAcadExt.AcadDocument.WriteMessage("4_76: " & tPriorPoint.ToString() & " - " & tCurrentPoint2d.ToString() & " B= " & dBulge.ToString())
						DMAcadExt.AcadDocument.WriteMessage("4_77: " & oArc2d_T.StartAngle.ToString() & " - " & (oArc2d_T.EndAngle * 180.0 / Math.PI).ToString() & " C= " & oArc2d_T.Center.ToString())

						DMAcadExt.AcadDocument.WriteMessage("4_78: " & oArc2d_F.StartAngle.ToString() & " - " & (oArc2d_F.EndAngle * 180.0 / Math.PI).ToString() & " C= " & oArc2d_F.Center.ToString())

					End If
					If Math.Abs(Math.Abs(dBulge) - Math.Abs(dBulge_T)) > Math.Abs(Math.Abs(dBulge) - Math.Abs(dBulge_F)) Then
						tCenterPoint = oArc2d_F.Center
					Else
						tCenterPoint = oArc2d_T.Center
					End If


					oTplnArc = New DMAcadExt.TplnArc(tCenterPoint, tPriorPoint, tCurrentPoint2d, dBulge > 0.0)
					'	oTplnArc = New DMAcadExt.TplnArc(tPriorPoint, tCurrentPoint2d, dBulge)  Alter


					'		DMAcadExt.AcadDocument.WriteMessageLog("4_91 CurveID=" & TPlnPoint.DispPoint(oArc2d.Center) & "; " & CStr(oArc2d.StartAngle) & "; " & CStr(oArc2d.EndAngle) & "; " & CStr(oArc2d.Radius) & "; " & TPlnPoint.DispVector(oArc2d.ReferenceVector, "V "))

					'	oaResPoints.Add(oTplnArc.GetMidPoint())
					'	DMAcadExt.AcadDocument.WriteMessageLog("4_92 Tol=" & CStr(dTolerance))


					oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)
					'	DMAcadExt.AcadDocument.WriteMessageLog("4_94 InnerPointsUB=" & CStr(oaInnerPoints.GetUpperBound(0)))
					For iIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
						If iSegmentCounter = 23 Then
							DMAcadExt.AcadDocument.WriteMessage("4_92: " & colNewPoints.Count.ToString & "; " & oaInnerPoints(iIndex).ToString())
						End If
						colNewPoints.Add(TPlnPoint.Point2dTo3d(oaInnerPoints(iIndex)))
						colNewBulges.Add(0.0)
					Next
					DMAcadExt.AcadDocument.WriteMessageLog("4_88 Bulges=" & CStr(dBulge) & "; " & CStr(dBulge_T) & ":" & CStr(dBulge_F) & "; C=" & tCenterPoint.ToString())
					DMAcadExt.AcadDocument.WriteMessageLog("4_90 CurveID=" & TPlnPoint.DispPoint(tPriorPoint) & "; " & TPlnPoint.DispPoint(tCurrentPoint2d) & "; Nv=" & oaInnerPoints.GetUpperBound(0).ToString())
					iArcCounter += 1
				End If
				dBulge = oVertex2d.Bulge
				bVertexIsNotFirst = True
				colNewPoints.Add(tCurrentPoint3d)
				colNewBulges.Add(0.0)
				If dBulge <> 0 Then
					tPriorPoint = tCurrentPoint2d
				End If
				iSegmentCounter += 1
				'	DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iVert) & ":" & TPlnPoint.DispPoint(oVertex2d.Position) & ";" & CStr(oVertex2d.Bulge) & "-" & oVertex2d.VertexType.ToString())
			Loop
			'	DMAcadExt.AcadDocument.WriteMessageLog("4_98  colNewPoints=" & CStr(colNewPoints.Count) & "; colNewBulges=" & CStr(colNewBulges.Count))
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

	Public Shared Function PartStraightenPolyline(ByVal oPolyline As Polyline, ByVal dTolerance As Double, ByVal sNewPolylineLayer As String) As Integer
		Const iExNo As Integer = 10
		Dim dBulge As Double
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

		'	AcadMapApp.ClearOD(oNewPolyline)
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
					dBulge = oNewPolyline.GetBulgeAt(iSegmentIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oNewPolyline.NumberOfVertices) & "!" & CStr(oNewPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
					Exit Do
				End Try
				Try
					oArc2d = oNewPolyline.GetArcSegment2dAt(iSegmentIndex)
					tStartPoint = oNewPolyline.GetPoint2dAt(iSegmentIndex)
					tEndPoint = oNewPolyline.GetPoint2dAt(iSegmentIndex + 1)
					oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tStartPoint, tEndPoint, dBulge > 0.0)
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
					System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145b")
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
	Public Shared Function StraightenPolyline(ByVal oPolyline As Polyline, ByVal dTolerance As Double, ByVal sNewPolylineLayer As String, Optional colBlockRefs As System.Collections.ObjectModel.Collection(Of BlockReference) = Nothing) As Integer
		Const iExNo As Integer = 10
		Dim dBulge As Double
		Dim oArc2d As CircularArc2d
		Dim oTplnArc As DMAcadExt.TplnArc
		Dim oArc As Arc

		Dim iSegmentIndex As Integer = 0
		Dim iNewIndex As Integer
		Dim oaInnerPoints() As Point2d
		Dim tStartPoint, tEndPoint As Point2d
		Dim tCurrentPoint As Point2d

		Dim iSegmentType As SegmentType
		Dim iArcCounter As Integer
		''''''''''''	Dim oResPoints As TplnPointArray = Nothing
		Dim oNewPolyline As Polyline
		Dim iLastIndex As Integer = zzGetPLineLastIndex(oPolyline)
		Dim tPlineColor As Autodesk.AutoCAD.Colors.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 2S)
		Dim bDebug As Boolean = (oPolyline.Color Is tPlineColor)
		Dim oPointOnCurve2d As PointOnCurve2d
		Dim oaPointOnCurve2d() As PointOnCurve2d

		Dim tClosestPointOnCurve As Point3d
		Dim dPointParameter As Double
		Dim iIndex As Integer
		Dim iNextIndex As Integer
		Dim bBreakPoints As Boolean
		Dim oaPointList(oPolyline.NumberOfVertices - 1) As List(Of Double)
		Dim oCircularArc2d As CircularArc2d
		Dim tVector As Vector3d
		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		Dim oPointOn As PointOnCurve3d
		Dim oLine2d As Line2d
		Dim taPoint2d() As Point2d
		Dim tBlockPosition As Point2d

		If colBlockRefs IsNot Nothing Then
			bBreakPoints = True
			'	DMCommon.Debug.MsgBox("161122_1", oPolyline Is Nothing, colBlockRefs.Count)
			If colBlockRefs.Count = -1 Then

			Else
				For Each oBlockReference As BlockReference In colBlockRefs
					tBlockPosition = New Point2d(oBlockReference.Position.X, oBlockReference.Position.Y)
					AcadTransaction.InsertPoint(oBlockReference.Position,, 3)
					'	oCircularArc2d = oPolyline.GetArcSegment2dAt(0)
					'AcadTransaction.InsertPoint(oCircularArc2d.Center,, 4)
					'	oBlockReference.IntersectWith(oPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
					'tVector = New Vector3d(oBlockReference.Position.X - oCircularArc2d.Center.X, oBlockReference.Position.Y - oCircularArc2d.Center.Y, 0)

					tClosestPointOnCurve = oPolyline.GetClosestPointTo(oBlockReference.Position, True)



					AcadTransaction.InsertPoint(tClosestPointOnCurve,, 4)
					dPointParameter = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
					iIndex = Convert.ToInt32(Math.Floor(dPointParameter))
					'	oLine2d = New Line2d(tBlockPosition, oCircularArc2d.Center)
					'	oaPointOnCurve2d = oCircularArc2d.GetClosestPointTo(oLine2d)
					'	taPoint2d = oCircularArc2d.IntersectWith(oLine2d)
					'	If taPoint2d Is Nothing OrElse taPoint2d.GetUpperBound(0) < 0 Then
					'DMCommon.Debug.MsgBox("211122_1", taPoint2d Is Nothing)
					'	Else
					'AcadTransaction.InsertPoint(taPoint2d(0),, 5)
					'End If
					'AcadTransaction.InsertPoint(oaPointOnCurve2d(0).Point,, 2)



					DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl1_Param", dPointParameter, iIndex, tClosestPointOnCurve)

					'	oCircularArc3d.Center
					'	Dim tVector As Vector3d = New Vector3d(oBlockReference.Position.X - oCircularArc3d.Center.X, oBlockReference.Position.Y - oCircularArc3d.Center.Y, oBlockReference.Position.Z - oCircularArc3d.Center.Z)
					'	tClosestPointOnCurve = oPolyline.GetClosestPointTo(oBlockReference.Position, tVector, True)
					'	AcadTransaction.InsertPoint(tClosestPointOnCurve,, 3)

					If oaPointList(iIndex) Is Nothing Then
						oaPointList(iIndex) = New List(Of Double)
					End If
					oaPointList(iIndex).Add(dPointParameter - iIndex)
					oaPointList(iIndex).Sort()
				Next

			End If

		End If

		oNewPolyline = New Polyline()
		Do
			Try
				iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oNewPolyline.NumberOfVertices) & "!" & CStr(oNewPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
				Exit Do
			End Try
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----", "-----")
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl0", oPolyline.ObjectId, iSegmentIndex, iSegmentType, oPolyline.GetPoint2dAt(iSegmentIndex), dBulge)
			If iSegmentType = SegmentType.Arc Then
				If iArcCounter = 0 Then

					'''''''''''''''''''oResPoints = New TplnPointArray()
					If Not String.IsNullOrEmpty(sNewPolylineLayer) Then
						Try
							oNewPolyline.Layer = sNewPolylineLayer
						Catch oEx As Exception
							AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
						End Try
					End If
				End If
				iArcCounter += 1
				Try
					dBulge = oPolyline.GetBulgeAt(iSegmentIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ":" & CStr(oNewPolyline.NumberOfVertices) & "!" & CStr(oNewPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_125")
					Exit Do
				End Try
				Try
					oArc2d = oPolyline.GetArcSegment2dAt(iSegmentIndex)
					tStartPoint = oPolyline.GetPoint2dAt(iSegmentIndex)
					If iSegmentIndex = oPolyline.NumberOfVertices - 1 Then
						tEndPoint = oPolyline.GetPoint2dAt(0)
					Else
						tEndPoint = oPolyline.GetPoint2dAt(iSegmentIndex + 1)
					End If

					DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl1a", oPolyline.NumberOfVertices, iSegmentIndex, tStartPoint, tEndPoint, dBulge)


					If bBreakPoints AndAlso oaPointList(iIndex) IsNot Nothing Then
						For iBreakPointIndex As Integer = 0 To oaPointList(iIndex).Count - 1

							tCurrentPoint = TPlnPoint.Point3dTo2d(oPolyline.GetPointAtParameter(oaPointList(iIndex).Item(iBreakPointIndex)))
							'	AcadTransaction.InsertPoint(tCurrentPoint,, 2)
							zzAddNewSegment(oArc2d.Center, tStartPoint, tCurrentPoint, dBulge > 0.0, dTolerance, oNewPolyline)
							tStartPoint = tCurrentPoint
						Next
						zzAddNewSegment(oArc2d.Center, tCurrentPoint, tEndPoint, dBulge > 0.0, dTolerance, oNewPolyline)
						'DMCommon.Debug.MsgBox("!151122_2", oaPointList(iIndex).Count)

					Else
						zzAddNewSegment(oArc2d.Center, tStartPoint, tEndPoint, dBulge > 0.0, dTolerance, oNewPolyline)
					End If



					If False Then
						oTplnArc = New DMAcadExt.TplnArc(oArc2d.Center, tStartPoint, tEndPoint, dBulge > 0.0)
						'''''''''''''''''''	oResPoints.Add(oTplnArc.GetMidPoint())
						oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)

						If oaInnerPoints Is Nothing Then
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2N", "NOTHING")
						ElseIf oaInnerPoints.GetUpperBound(0) >= 0 Then
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2", oaInnerPoints.GetUpperBound(0), oaInnerPoints(0), oaInnerPoints(oaInnerPoints.GetUpperBound(0)))
						Else

							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2-0", oaInnerPoints.GetUpperBound(0))
						End If


						oArc = oTplnArc.GetAcadArc()
						oArc.ColorIndex = 1
						''''''	AppendEntity(oArc, False)
						iNewIndex = oNewPolyline.NumberOfVertices
						oNewPolyline.AddVertexAt(iNewIndex, tStartPoint, 0.0, 0.0, 0.0)
						For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
							oNewPolyline.AddVertexAt(iNewIndex + 1 + iPointIndex, oaInnerPoints(iPointIndex), 0.0, 0.0, 0.0)
						Next

					End If

				Catch oEx As Exception
					tStartPoint = oPolyline.GetPoint2dAt(0)
					tEndPoint = oPolyline.GetPoint2dAt(iSegmentIndex)
					'	System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & " < " & CStr(oPolyline.NumberOfVertices) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y) & vbCrLf & CStr(tEndPoint.X) & "," & CStr(tEndPoint.Y), "21_144dd")
					DMAcadExt.AcadDocument.WriteDebugMessageN("21_144dd", CStr(iSegmentIndex) & " < " & CStr(oPolyline.NumberOfVertices), oEx.Message, oEx.StackTrace, CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y), CStr(tEndPoint.X) & "," & CStr(tEndPoint.Y))
				End Try

			ElseIf iSegmentType = SegmentType.Line OrElse iSegmentType = SegmentType.Point Then
				If iSegmentIndex = oPolyline.NumberOfVertices Then
					tStartPoint = oPolyline.GetPoint2dAt(0)
				Else
					tStartPoint = oPolyline.GetPoint2dAt(iSegmentIndex)
				End If
				iNewIndex = oNewPolyline.NumberOfVertices
				oNewPolyline.AddVertexAt(iNewIndex, tStartPoint, 0.0, 0.0, 0.0)

				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl1_ln", oPolyline.NumberOfVertices, iSegmentIndex, iNewIndex, tStartPoint, oNewPolyline.GetPoint2dAt(iNewIndex))

			Else
				Dim tAcadPoint As Autodesk.AutoCAD.Geometry.Point2d = oPolyline.GetPoint2dAt(iSegmentIndex)
				AcadDocument.WriteMessage("230a: " & iSegmentType.ToString() & " " & CStr(tAcadPoint.X) & "," & CStr(tAcadPoint.Y))
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!SegmentType??", iSegmentType, tAcadPoint)
			End If
			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = iLastIndex)
		If iArcCounter <> 0 Then
			AcadTransaction.AppendEntity(oNewPolyline)
		Else
			oNewPolyline = Nothing
		End If
		Return iArcCounter

	End Function
	Private Shared Sub zzAddNewSegment(tArcCenter As Point2d, tStartPoint As Point2d, tEndPoint As Point2d, bBulgeIsPositive As Boolean, ByVal dTolerance As Double, ByRef oNewPolyline As Polyline)
		Dim oTplnArc As DMAcadExt.TplnArc
		Dim oArc As Arc
		Dim iNewIndex As Integer
		Dim oaInnerPoints() As Point2d
		oTplnArc = New DMAcadExt.TplnArc(tArcCenter, tStartPoint, tEndPoint, bBulgeIsPositive)
		'''''''''''''''''''	oResPoints.Add(oTplnArc.GetMidPoint())
		oaInnerPoints = oTplnArc.GetInnerPoints(dTolerance)

		If oaInnerPoints Is Nothing Then
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2N", "NOTHING")
		ElseIf oaInnerPoints.GetUpperBound(0) >= 0 Then
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2", oaInnerPoints.GetUpperBound(0), oaInnerPoints(0), oaInnerPoints(oaInnerPoints.GetUpperBound(0)))
		Else

			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!StrPl2-0", oaInnerPoints.GetUpperBound(0))
		End If


		oArc = oTplnArc.GetAcadArc()
		oArc.ColorIndex = 1
		''''''	AppendEntity(oArc, False)
		iNewIndex = oNewPolyline.NumberOfVertices
		oNewPolyline.AddVertexAt(iNewIndex, tStartPoint, 0.0, 0.0, 0.0)
		For iPointIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
			oNewPolyline.AddVertexAt(iNewIndex + 1 + iPointIndex, oaInnerPoints(iPointIndex), 0.0, 0.0, 0.0)
		Next

	End Sub
	Public Shared Function StraightenPolyline190213(ByVal oPolyline As Polyline, ByVal dTolerance As Double, ByVal sNewPolylineLayer As String) As Integer
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

		'	AcadMapApp.ClearOD(oNewPolyline)
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
					System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & " < " & CStr(oNewPolyline.NumberOfVertices) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_144dold")
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
	Public Shared Function StraightenPolylineZ(ByVal oPolyline As Polyline, ByVal bFix As Boolean, ByVal dTolerance As Double, ByVal oTopoDef As TopoDef, ByVal bOverlay As Boolean) As TplnPointArray
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
						System.Windows.Forms.MessageBox.Show(CStr(iSegmentIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145c")
					End Try
				End If
			End If
			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oPolyline.NumberOfVertices - 1)
		mdicEntityCells.AddPolylineZ(oPolyline, oTopoDef, bOverlay)
		'DMAcadExt.AcadDocument.WriteMessage("TotalErrArea: " & CStr(dTotalSourceArea) & "-" & CStr(dTotalNewArea) & "=" & CStr(dTotalSourceArea - dTotalNewArea) & ", " & CStr((dTotalSourceArea - dTotalNewArea) * 100.0 / dTotalSourceArea) & "%")
		Return oResPoints
	End Function

	Public Shared Function PolylineHasNotArc(ByVal oPolyline As Polyline) As Boolean
		Dim iSegmentIndex As Integer = 0
		Dim iSegmentType As SegmentType
		Dim iLastIndex As Integer = zzGetPLineLastIndex(oPolyline)
		Do
			Try
				iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ": " & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_129")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				Return False
			End If
			iSegmentIndex += 1
			'	Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oPolyline.NumberOfVertices - 1)
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = iLastIndex)
		Return True
	End Function
	Public Shared Sub CopyPolyline(ByVal oPolyline As Polyline, Optional ByVal sNewPolylineLayer As String = "")
		Dim oNewPolyline As Polyline
		oNewPolyline = New Polyline(oPolyline.NumberOfVertices)
		oNewPolyline.Closed = oPolyline.Closed
		If String.IsNullOrEmpty(sNewPolylineLayer) Then
			oNewPolyline.Layer = oPolyline.Layer
		Else
			oNewPolyline.Layer = sNewPolylineLayer
		End If
		For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
			oNewPolyline.AddVertexAt(iIndex, oPolyline.GetPoint2dAt(iIndex), oPolyline.GetBulgeAt(iIndex), 0.0, 0.0)
		Next
		AcadTransaction.AppendEntity(oNewPolyline)
		oPolyline.Erase()
	End Sub
	Public Shared Sub CopyLine(ByVal oLine As Line, Optional ByVal sNewPolylineLayer As String = "")
		Dim oNewLine As Line
		oNewLine = New Line(oLine.StartPoint, oLine.EndPoint)

		If String.IsNullOrEmpty(sNewPolylineLayer) Then
			oNewLine.Layer = oLine.Layer
		Else
			oNewLine.Layer = sNewPolylineLayer
		End If
		AcadTransaction.AppendEntity(oNewLine)
		oLine.Erase()
	End Sub
	Public Shared Sub CopyArc(ByVal oArc As Arc, Optional ByVal sNewPolylineLayer As String = "")
		Dim oNewArc As Arc
		oNewArc = New Arc(oArc.Center, oArc.Radius, oArc.StartAngle, oArc.EndAngle)


		If String.IsNullOrEmpty(sNewPolylineLayer) Then
			oNewArc.Layer = oArc.Layer
		Else
			oNewArc.Layer = sNewPolylineLayer
		End If
		AcadTransaction.AppendEntity(oNewArc)
		oArc.Erase()
	End Sub
	Private Shared Sub zzAddPolyline2d(ByVal oPolyline2d As Polyline2d, ByVal bOverlay As Boolean)
		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim tPriorPoint, tCurrentPoint2d As Point2d
		Dim tCurrentPoint As Point3d
		Dim oLineSegment As TplnCurveSegment
		Dim tObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing
		Dim iIndex As Integer = 0
		Dim bInner As Boolean
		Try
			Do While oColEnum.MoveNext()
				If iIndex <> 0 Then
					bInner = (iIndex <> 1)
					If mdicEntityCells IsNot Nothing Then
						mdicEntityCells.zzAddEntity(tCurrentPoint2d, oPolyline2d, iIndex, bInner, bOverlay)
						mdicEntityCells.AddPolyline2dCounter()
					End If
					If False Then
						If mdicCoarseEntityCells IsNot Nothing Then
							mdicCoarseEntityCells.zzAddEntity(tCurrentPoint2d, oPolyline2d, iIndex, bInner, bOverlay)
							mdicCoarseEntityCells.AddPolyline2dCounter()
						End If
					End If


					tPriorPoint = tCurrentPoint2d
				End If
				tObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tObjID, OpenMode.ForWrite)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				tCurrentPoint = oVertex2d.Position
				tCurrentPoint2d = TPlnPoint.Point3dTo2d(tCurrentPoint)
				If iIndex <> 0 Then
					oLineSegment = New TplnCurveSegment(tPriorPoint, tCurrentPoint2d)
					zzAddCurveSegment(oLineSegment, Nothing, False, String.Empty)
				End If
				iIndex += 1
			Loop
			If mdicEntityCells IsNot Nothing Then
				mdicEntityCells.zzAddEntity(tCurrentPoint2d, oPolyline2d, iIndex, False, bOverlay)
			End If
			If False Then
				If mdicCoarseEntityCells IsNot Nothing Then
					mdicCoarseEntityCells.zzAddEntity(tCurrentPoint2d, oPolyline2d, iIndex, False, bOverlay)
				End If
			End If


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmLineCleanupA")
		End Try
	End Sub
	Private Shared Function zzGetPLineLastIndex(ByVal oPolyline As Polyline) As Integer
		'iLastIndex =
		If oPolyline.Closed Then
			If oPolyline.GetPoint2dAt(0).IsEqualTo(oPolyline.GetPoint2dAt(oPolyline.NumberOfVertices - 1)) Then
				Return oPolyline.NumberOfVertices
			Else
				Return oPolyline.NumberOfVertices + 1
				'	dAddLine = True
			End If

		Else
			Return oPolyline.NumberOfVertices
		End If
	End Function
	Private Shared Sub zzAddPolyline(ByVal oPolyline As Polyline, ByVal bOverlay As Boolean, ByVal bPoint As Boolean, ByVal bPointLine As Boolean)
		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
		Dim bInner As Boolean
		Dim tPriorPoint, tCurrentPoint As Point2d
		Dim oLineSegment As TplnCurveSegment

		'   DMAcadExt.AcadDocument.WriteMessage("_PL " & CStr(iVerticesUB) & "; bPoint=" & bPoint.ToString() & "; bPointLine=" & bPointLine.ToString())
		For iIndex As Integer = 0 To iVerticesUB
			If iIndex = 0 OrElse iIndex = iVerticesUB Then
				bInner = False
			Else
				bInner = True
			End If
			tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
			If False And bPoint Then
				mdicEntityCells.zzAddEntity(tCurrentPoint, oPolyline, iIndex, bInner, bOverlay)
				If iIndex = 0 Then
					mdicEntityCells.AddPolylineCounter()
					' hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh()
				End If

			End If
			If False And bPointLine Then
				mdicCoarseEntityCells.zzAddEntity(tCurrentPoint, oPolyline, iIndex, bInner, bOverlay)
				If iIndex = 0 Then
					mdicCoarseEntityCells.AddPolylineCounter()
				End If
			End If

			If False And iIndex <> 0 Then
				oLineSegment = New TplnCurveSegment(tPriorPoint, tCurrentPoint)
				zzAddCurveSegment(oLineSegment, Nothing, False, String.Empty)
			End If
			tPriorPoint = tCurrentPoint
		Next
		mdicEntityCells.AddPolyline(oPolyline, bOverlay, bPoint, bPointLine)

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
					System.Windows.Forms.MessageBox.Show(CStr(iIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "21_145a")
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

		Dim oMergeTopology As Autodesk.Gis.Map.Topology.TopologyModel = AcadMapApp.GetTopology(sMergeTopoName)
		If oMergeTopology IsNot Nothing Then

			Dim oLotPgon As Topology.Polygon = Nothing

			If oMergeTopology.Status = Topology.Status.Closed Then
				Try
					oMergeTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "dmLineCleanup - UpdateTopoByMerge_21")

				End Try
				If oMergeTopology.Status <> Topology.Status.Closed Then
					Dim colPolygons As Topology.PolygonCollection
					Try
						colPolygons = oMergeTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "dmLineCleanup - UpdateTopoByMerge_8")
						Try
							If oMergeTopology.Status <> Topology.Status.Closed Then
								oMergeTopology.Close()
								oMergeTopology = Nothing
							End If
						Catch oMapExB As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapExB.ErrorCode, False, "dmLineCleanup - UpdateTopoByMerge_26")
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
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "dmLineCleanup - UpdateTopoByMerge_4")
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
					AcadTransaction.AppendEntity(oNewCurve)
					AcadMapApp.ClearOD(oNewCurve)
					oNewCurve.XData = New ResultBuffer()
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
	Private Shared Sub zzExplodeLine(ByVal oCurve As Curve, ByVal sDestLayer As String, ByVal bOverlay As Boolean)
		Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
		Dim oEntity As Entity
		Dim iSegmentIndex As Integer = -1
		Dim oNewCurve As Curve
		Dim oDBObject As DBObject
		oCurve.Explode(colDBObjects)
		If colDBObjects.Count = 1 Then
			oDBObject = colDBObjects.Item(0)
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
		End If
	End Sub

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
	'For RemoveDuplicate & PointLine
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
						oCurve.XData = New ResultBuffer()
						tSegmentBufferPrev.AddBuffer(tSegmentBuffer)
						'	tSegmentBuffer.AddSourceHandle(tSourceHandle, iSegmentIndex)
						oLinePrev.XData = tSegmentBufferPrev.GetResBuffer()
					End If
				Else
					Dim oCircleMarkBlock As MarkBlock = New MarkBlock(MarkBlock.enMarkBlockType.Circle)
					oCircleMarkBlock.MarkPoint(oCurveSegment.StartPoint, 33S)
					oCircleMarkBlock.MarkPoint(oCurveSegment.EndPoint, 33S)
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

	'For RemoveDuplicate & PointLine
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
		oCurveSegment = New TplnArc(oArc, True)
		Return zzAddCurveSegment(oCurveSegment, oArc, bFix, sDuplicateLayer)
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
			Const iExNo As Integer = 22
			Dim taTypedValues() As TypedValue = oResBuffer.AsArray()
			Dim shCode As Short
			Dim oValue As System.Object
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
		Private Function zzToHandle(ByVal oValue As System.Object) As Handle
			Dim sValue As String = DirectCast(oValue, String)
			Dim lValue As Long = Convert.ToInt64(Val("&H" & sValue))
			Return New Handle(lValue)
		End Function
		Private Function zzToBoolean(ByVal oValue As System.Object) As Boolean
			Dim shValue As Short = DirectCast(oValue, Int16)
			Return Convert.ToBoolean(shValue)
		End Function
		Private Shared Function zzGetOverlay(ByVal oCurve As Curve) As Boolean
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
	Private Shared Function zzGetArcCount(ByVal oPolyline As Polyline) As Integer
		Dim iSegmentIndex As Integer = 0
		Dim iSegmentType As SegmentType
		Dim iArcCounter As Integer

		Do
			Try
				iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex), "21_132")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				iArcCounter += 1
			End If
			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oPolyline.NumberOfVertices)
		Return iArcCounter
	End Function
	Private Shared Function zzGetArcCount(ByVal oPolyline2d As Polyline2d) As Integer
		Dim dBulge As Double = 0.0

		Dim tAcObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing

		Dim iArcCounter As Integer
		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Try

			Do While oColEnum.MoveNext()
				tAcObjID = DirectCast(oColEnum.Current, ObjectId)
				oDBObj = AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
				oVertex2d = DirectCast(oDBObj, Vertex2d)
				'''''''''''	DMAcadExt.AcadUtil.DispAcadEnt(oVertex2d)

				If dBulge <> 0.0 Then 'Prior Bulge
					iArcCounter += 1
				End If
				dBulge = oVertex2d.Bulge
			Loop
			Return iArcCounter

		Catch oEx As Exception
			'		DMAcadExt.AcadDocument.WriteMessage("Reset:" & oEx.Message)
		End Try
	End Function

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
