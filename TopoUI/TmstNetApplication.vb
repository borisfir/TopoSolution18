
Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports FDO
Imports Autodesk.AutoCAD.EditorInput
'Imports AcDbSymbolUtilities
'Imports Autodesk.Gis.Map

'Imports Autodesk.Gis.Map.Utilities
'Imports System.Runtime.InteropServices
Imports Autodesk.AutoCAD.Geometry
Imports TopoManager

Public NotInheritable Class TmstNetApplication
   Private Shared miIndex As Integer
   Private mfTopoView As frmTopoView
   Private WithEvents mfTplnView As frmTplnView
   Private WithEvents mfTopoActions As frmTopoActionsBase
   Private WithEvents mfDWGBatch As TopoUI.frmDWGBatch
   Private WithEvents mfApp As TopoUI.frmApp
   Private WithEvents mfPrjThemes As TopoUI.frmPrjThemes
	Private WithEvents mfHanitGeneral As TopoUI.frmHanitGeneral
	Private WithEvents mfEditBlockRefs As TopoUI.frmEditBlockRefs
	Private WithEvents mfHanitControl As TopoUI.frmHanitControl
	Private WithEvents mfProjectFiles As TopoUI.frmProjectFiles
	Private WithEvents mfExecUtil As TopoUI.frmExecUtil

	Private WithEvents mfDataConnect As frmDataConnect
	Private mtShiftVector As Vector3d
	Private WithEvents moPgonRelation As FDO.PgonRelation


	Private moFDO_Manager As FDO.FDO_Manager
	'	Private WithEvents mfTopoActionsB As frmTopoActionsBase
	Private Shared mdTolerance As Double = 0.01
	Private Shared mbToExcel As Boolean = False
	Private Shared moMPolygonA As MPolygon
	Private WithEvents moAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document
	Private Shared WithEvents moEditor As Autodesk.AutoCAD.EditorInput.Editor

	Public Sub New()
		Dim oPromptKeywordOpt As Autodesk.AutoCAD.EditorInput.PromptKeywordOptions = New Autodesk.AutoCAD.EditorInput.PromptKeywordOptions("Select Option ")
	End Sub

	<Autodesk.AutoCAD.Runtime.CommandMethodAttribute("CmdList")>
	Public Shared Sub CmdList()
		Common.GetEditor.WriteMessage(vbCrLf & " TownPlanner Commands : " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : TplnCalc " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepContent  - 301" & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepPlanParcels  - 302 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepLotsK  - 303 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepLotsM  - 304 " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseK  - 305 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseM  - 306 " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepSumLuse  - 307 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepParcels  - 311 " & vbCrLf)


		Common.GetEditor.WriteMessage("** Cmd : DispTpln " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : TplnClose " & vbCrLf)

	End Sub
	<CommandMethod("ToExcel")>
	Public Shared Sub ToExcel()
		Const sYes As String = "Yes"
		Const sNo As String = "No"

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptKeywordOpt As Autodesk.AutoCAD.EditorInput.PromptKeywordOptions = New Autodesk.AutoCAD.EditorInput.PromptKeywordOptions("Select Option ")
		Dim oKeywordRes As PromptResult
		Dim sCurrentKey As String
		If mbToExcel Then
			sCurrentKey = "N"
		Else
			sCurrentKey = "Y"
		End If
		oPromptKeywordOpt.Keywords.Add(sYes)
		oPromptKeywordOpt.Keywords.Add(sNo)

		oPromptKeywordOpt.Message = vbLf & "Enter New value for ToExcel [Yes/No] <" & sCurrentKey & ">"

		oKeywordRes = oEditor.GetKeywords(oPromptKeywordOpt)
		If oKeywordRes.Status = PromptStatus.OK Then
			oEditor.WriteMessage("Res=" & oKeywordRes.StringResult)
		End If
		Select Case oKeywordRes.StringResult
			Case sYes
				mbToExcel = True
				DMCommon.Debug.ExcelLog = New DMCommon.ExcelAppExt()
				If DMCommon.Debug.Debug Then
					DMCommon.Debug.ExcelLog.Open()
				End If
			Case sNo
				mbToExcel = False
		End Select



	End Sub
	<CommandMethod("TestMPa")>
	Public Sub TestMPolygonA()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		'	Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity A")
		'	Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		Dim oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline
		Dim iParent As Integer
		'	Dim oPlineA, oPlineB As Polyline
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		Dim lstPolygons As System.Collections.Generic.IList(Of Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylines("1602", OpenMode.ForRead)  '"pclp004"
		Dim oMPolygonA As MPolygon = New MPolygon()
		Dim oMPgonLoopB As MPolygonLoop
		For iIndex As Integer = 0 To lstPolygons.Count - 1
			oPolygon = lstPolygons.Item(iIndex)

			Try
				oMPolygonA.AppendLoopFromBoundary(oPolygon, False, 0.001)
			Catch oEx As Exception
				oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try

		Next

		oMPolygonA.BalanceTree()



		oEditor.WriteMessage("NumMPgonLoops After: " & CStr(oMPolygonA.NumMPolygonLoops) & vbCrLf)
		Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection = oMPolygonA.GetLoopDirection(1)
		Dim bCrossed As Boolean
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
		For iIndex As Integer = 0 To oMPolygonA.NumMPolygonLoops - 1
			iDir = oMPolygonA.GetLoopDirection(iIndex)
			iParent = 0
			oMPgonLoopB = oMPolygonA.GetMPolygonLoopAt(iIndex)
			bCrossed = oMPolygonA.LoopCrossesMPolygon(oMPgonLoopB, 0.001)
			If bCrossed Or iDir = LoopDirection.Interior Then

				If iDir = LoopDirection.Interior Then
					iParent = oMPolygonA.GetParentLoop(iIndex)
					zzDispMPgonLoop(oMPgonLoopB, Color.Red)
					If iParent >= 0 AndAlso iParent < oMPolygonA.NumMPolygonLoops Then
						oMPgonLoopB = oMPolygonA.GetMPolygonLoopAt(iParent)
						zzDispMPgonLoop(oMPgonLoopB, Color.Yellow)

					End If
				Else
					zzDispMPgonLoop(oMPgonLoopB, Color.Empty)

				End If
				oEditor.WriteMessage("Dir of Loop # " & CStr(iIndex) & " " & iDir.ToString() & " : " & bCrossed.ToString() & " : " & CStr(iParent) & vbCrLf)
			End If
		Next

		'	DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)

		Dim oaBlockRefPoints As DMAcadExt.TplnPointArray = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint("1603")
		Dim oPoint As DMAcadExt.TPlnPoint
		Dim iLoop As Integer
		For iIndex As Integer = 0 To oaBlockRefPoints.UpperBound
			oPoint = oaBlockRefPoints.Item(iIndex)
			iLoop = oMPolygonA.GetClosestLoopTo(oPoint.AcGePoint3d)
		Next
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub

	<CommandMethod("INTERS_")>
	Public Sub Intersection()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity = Nothing
		Dim oEntB As Entity = Nothing
		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				Return
			End Try
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				Try
					oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
				Catch oEx As Exception
					Return
				End Try
			End If
			oEntA.IntersectWith(oEntB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))

			If colPoints IsNot Nothing Then
				Dim oSaltireMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Saltire)
				For Each tPoint As Point3d In colPoints
					' DMAcadExt.AcadTransaction.InsertPoint(tPoint)
					oSaltireMarkBlock.MarkPoint(tPoint, 4S)

				Next
				DMAcadExt.AcadDocument.WriteMessage("Number of Intersect points: " & CStr(colPoints.Count))
			Else
				DMAcadExt.AcadDocument.WriteMessage("Intersect points not exist")
			End If

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestMPb")>
	Public Sub TestMPolygonB()
		Const dToler As Double = 0.001
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim oEnt As DBObject = Nothing
		Dim oPlineA As Polyline
		Dim oPlineB As Polyline


		Dim iParent As Integer
		'	Dim oPlineA, oPlineB As Polyline
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

		Dim lstPolygons As System.Collections.Generic.IList(Of Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylines("LA", OpenMode.ForRead)   '"pclp004"
		oPlineA = lstPolygons.Item(0)
		lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines("LB", OpenMode.ForRead)      '"pclp004"
		'lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines("LC", OpenMode.ForRead)	  '"pclp004"

		oPlineB = lstPolygons.Item(0)

		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

		Dim oMPolygon As MPolygon = New MPolygon()
		Dim oMPgonLoop As MPolygonLoop
		Dim oMPgonLoopA As MPolygonLoop




		Try
			oMPolygon.AppendLoopFromBoundary(oPlineA, False, 0.001)
		Catch oEx As Exception
			oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Return
		End Try

		Try
			oMPolygon.AppendLoopFromBoundary(oPlineB, False, 0.001)
		Catch oEx As Exception
			oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Return
		End Try

		Try
			oMPolygon.BalanceTree()
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			oEditor.WriteMessage(" BP Err=" & oAcadEx.ErrorStatus.ToString() & vbCrLf)
		End Try


		Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection
		Dim bCrossed As Boolean

		Dim bCrossedItself As Boolean
		Dim bCrossedItselfA As Boolean

		Dim bTouch As Boolean
		'	Dim bLoopCrossing0 As Boolean
		'	Dim bLoopCrossing1 As Boolean
		'	Dim oMPgonLoop0 As MPolygonLoop
		'	Dim oMPgonLoop1 As MPolygonLoop

		bTouch = oMPolygon.IncludesTouchingLoops(0.001)


		'	oMPgonLoop0 = oMPolygonA.GetMPolygonLoopAt(0)

		'	bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
		'	bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)
		oPlineA.IntersectWith(oPlineB, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))

		oEditor.WriteMessage("!!Number of Loop : " & CStr(oMPolygon.NumMPolygonLoops) & vbCrLf)
		oEditor.WriteMessage("IncludesTouchingLoops: " & CStr(bTouch) & vbCrLf)
		If colPoints IsNot Nothing Then
			oEditor.WriteMessage("Intersect Points: " & CStr(colPoints.Count) & vbCrLf)
		End If

		For iIndex As Integer = 0 To oMPolygon.NumMPolygonLoops - 1
			iDir = oMPolygon.GetLoopDirection(iIndex)
			iParent = 0
			oMPgonLoop = oMPolygon.GetMPolygonLoopAt(iIndex)
			oMPgonLoopA = oMPolygon.GetMPolygonLoopAt((iIndex + 1) Mod oMPolygon.NumMPolygonLoops)
			bCrossed = oMPolygon.LoopCrossesMPolygon(oMPgonLoop, dToler)
			bCrossedItself = oMPgonLoop.LoopCrossesItself(oMPgonLoop, dToler)
			bCrossedItselfA = oMPgonLoop.LoopCrossesItself(oMPgonLoopA, dToler)

			oEditor.WriteMessage("Dir of Loop # " & CStr(iIndex) & " = " & iDir.ToString() & " : " & bCrossed.ToString() & " : itself=" & CStr(bCrossedItself) & ":" & CStr(bCrossedItselfA) & vbCrLf)
		Next

		'	DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)
		moMPolygonA = oMPolygon

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub
	<CommandMethod("TestMPP")>
	Public Sub TestMPPoint()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult

		Dim iaLoopInd As IntegerCollection
		oPromptOpt.UseBasePoint = False
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		'	DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)

		ptRes = oEditor.GetPoint(oPromptOpt)
		Dim tPoint As Point3d
		If ptRes.Status = PromptStatus.OK Then
			tPoint = ptRes.Value
			oEditor.WriteMessage("Point= " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
			iaLoopInd = moMPolygonA.IsPointInsideMPolygon(tPoint, 0.001)
			If iaLoopInd Is Nothing Then
				oEditor.WriteMessage("Point is Nothing ")
			Else
				For iIndex As Integer = 0 To iaLoopInd.Count - 1
					oEditor.WriteMessage("##" & CStr(iIndex) & ": " & CStr(iaLoopInd.Item(iIndex)))
				Next
			End If

		End If


		'	DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub


	Private Sub zzDispMPgonLoop(oMPgonLoop As MPolygonLoop, tColor As Color)
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

		Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle
		Dim tNormal As Autodesk.AutoCAD.Geometry.Vector3d = New Autodesk.AutoCAD.Geometry.Vector3d(0, 0, 1)
		For iIndex As Integer = 0 To oMPgonLoop.Count - 1
			tPoint = oMPgonLoop.Item(iIndex).Vertex
			tPoint3d = New Point3d(tPoint.X, tPoint.Y, 0.0)
			oCircle = New Autodesk.AutoCAD.DatabaseServices.Circle(tPoint3d, tNormal, 1.0)
			If tColor <> Color.Empty Then
				oCircle.Color = Autodesk.AutoCAD.Colors.Color.FromColor(tColor)
			End If
			DMAcadExt.AcadTransaction.AppendEntity(oCircle)
		Next

	End Sub
	<CommandMethod("PgonR")>
	Public Sub PgonRelation()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'  Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'    DMAcadExt.AcadDocument.OpenLog(False)
		Do
			oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
				Catch oEx As Exception
					Return
				End Try
				If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					If oPolyline Is Nothing Then
						oPolyline = DirectCast(oEnt, Polyline)
						If oBlockRef IsNot Nothing Then
							Exit Do
						End If
					Else
						oPolylineA = DirectCast(oEnt, Polyline)
						Exit Do
					End If
				ElseIf oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then
					If oBlockRef Is Nothing Then
						oBlockRef = DirectCast(oEnt, BlockReference)
					End If
					If oPolyline IsNot Nothing Then
						Exit Do
					End If
				End If
			Else
				Exit Do
			End If

		Loop
		If oPolyline IsNot Nothing AndAlso oPolylineA IsNot Nothing Then
			moPgonRelation = TplnPolygonSet.PolygonRelation(oPolyline, oPolylineA, True)

			If moPgonRelation.ErrorPointsCount > 0 Then
				'''''''''''''''''  moPgonRelation.MarkIntersectionPoints(False)
			End If

		ElseIf oPolyline IsNot Nothing AndAlso oBlockRef IsNot Nothing Then
			TplnPolygonSet.PolygonRelation(oPolyline, oBlockRef)
		End If

		DMAcadExt.AcadDocument.CloseLog()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("MarkP")>
	Public Sub MarkIntersectionPoints()
		If moPgonRelation IsNot Nothing Then

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
			'  DMAcadExt.AcadDocument.OpenLog(False)


			moPgonRelation.MarkIntersectionPoints(True)


			DMAcadExt.AcadDocument.CloseLog()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If


	End Sub
	<CommandMethod("PgonI")>
	Public Sub PgonIntersection()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'   Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		Do
			oEditor.WriteMessage("Select Polyline ...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
				Catch oEx As Exception
					Return
				End Try
				If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					If oPolyline Is Nothing Then
						oPolyline = DirectCast(oEnt, Polyline)
						If oBlockRef IsNot Nothing Then
							Exit Do
						End If
					Else
						oPolylineA = DirectCast(oEnt, Polyline)
						Exit Do
					End If
				ElseIf oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then
					If oBlockRef Is Nothing Then
						oBlockRef = DirectCast(oEnt, BlockReference)
					End If
					If oPolyline IsNot Nothing Then
						Exit Do
					End If
				End If
			Else
				Exit Do
			End If

		Loop
		If oPolyline IsNot Nothing AndAlso oPolylineA IsNot Nothing Then
			TopoManager.tmVertices.SetTolerance(0.000001)
			Dim oRing As tmRing = New tmRing(1, oPolyline)
			Dim oRingA As tmRing = New tmRing(2, oPolylineA)
			Dim oPgonIntersect As tmPgonIntersect = New tmPgonIntersect(oRing, oRingA)


		End If


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("PgonC")>
	Public Sub PgonCorrection()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		'    Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'  Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing



		Dim tStartPoint As Point3d
		Dim tEndPoint As Point3d

		If zzSelectPoint(tStartPoint, True) Then
			'  System.Windows.Forms.MessageBox.Show("!!!!" & CStr("???"), "05_112")
			If zzSelectPoint(tEndPoint, True) Then
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
				moPgonRelation.OpenLinesForWrite()
				moPgonRelation.CorrectBetween(tStartPoint, tEndPoint)

				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
			End If
		End If


	End Sub



	<CommandMethod("ToMP")>
	Public Sub ToMPolygon()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline ...")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		Dim bExists As Boolean

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		'	DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		Dim oMPolygonA As MPolygon = New MPolygon()
		Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

		Do
			oEditor.WriteMessage("Start: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try

					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
					oEditor.WriteMessage("oEnt: " & vbCrLf)
				Catch oEx As Exception
					oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
					DMAcadExt.AcadTransaction.Abort()
					DMAcadExt.AcadDocument.Unlock()
					Return
				End Try
				Select Case oEnt.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							oMPolygonA.AppendLoopFromBoundary(oPolyline, True, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))
							DMAcadExt.AcadTransaction.Abort()
							DMAcadExt.AcadDocument.Unlock()
							Return
						End Try
					Case DMAcadExt.AcadConst.Acad2dPolylineName
						Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							oMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))
							DMAcadExt.AcadTransaction.Abort()
							DMAcadExt.AcadDocument.Unlock()
							Return
						End Try

				End Select
			Else
				Exit Do
			End If
		Loop
		If bExists Then
			Try
				oMPolygonA.BalanceTree()
				For Each oDBObject As DBObject In colDBObject
					oDBObject.Erase()
				Next

				DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
				DMAcadExt.AcadTransaction.AppendEntity(oMPolygonA)
				DMAcadExt.AcadTransaction.CloseModelSpace()


			Catch oEx As Exception
				oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))
				DMAcadExt.AcadTransaction.Abort()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TestFC")>
	Public Sub TestFindCenter()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		Dim oMPgon As MPolygon = zzGetMPolygon(False)
		If oMPgon IsNot Nothing Then
			Dim oFC As FDO.TplnPolygonSet.FindCenter = New FDO.TplnPolygonSet.FindCenter(oMPgon)
			Dim oPoint As DMAcadExt.TPlnPoint = oFC.GetInnerPoint()
			If oPoint IsNot Nothing Then

				DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
				DMAcadExt.AcadTransaction.AppendEntity(New DBPoint(oPoint.GetPoint3d()))
				DMAcadExt.AcadTransaction.CloseModelSpace()

			Else

			End If
		Else
			DMAcadExt.AcadDocument.WriteMessage("MPolygon Is Nothing")
		End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("ListRX")>
	Public Sub ListRX()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)

		DMAcadExt.AcadTransaction.TestModelSpaceCount()
		DMAcadExt.AcadTransaction.ListDBObjects()


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()



	End Sub


	<CommandMethod("ClearXData")>
	Public Sub ClearXData()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


		DMAcadExt.AcadTransaction.ClearXData()


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub
	<CommandMethod("PgonV", CommandFlags.Session)>
	Public Sub PgonView()
		FDO.TplnPolygonSet.PgonView()
	End Sub
	<CommandMethod("PgonL", CommandFlags.Session)>
	Public Sub PgonList()
		FDO.TplnPolygonSet.PgonList()
	End Sub
	<CommandMethod("TmstOpen", CommandFlags.Session)>
	Public Sub TmstOpen()
		zzOpenApp(DMAcadExt.enApplications.TopoMaster)
	End Sub
	<CommandMethod("UDOpen", CommandFlags.Session)>
	Public Sub UDOpen()
		zzOpenApp(DMAcadExt.enApplications.Unidiv)
	End Sub
	<CommandMethod("Uni_Start", CommandFlags.Session)>
	Public Sub Start()
		UnidivNet.Unidiv.Start()
	End Sub
	<CommandMethod("Exut", CommandFlags.Session)>
	Public Sub ExecUtil()
		' FDO.Util.SeparateShapePgons()
		' DMAcadExt.AcadDocument.InitDebug()
		'  FDO.Util.SetScheme()
		FDO.Util.EnumShapePgons()
	End Sub

	<CommandMethod("Nat", CommandFlags.Session)>
	Public Sub Nat()
		UnidivNet.Unidiv.NatA()
	End Sub
	<CommandMethod("Dorit", CommandFlags.Session)>
	Public Sub PolylinesByCentroidLayer()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
		'   MessageBox.Show(tMapThemeData.ClosedPgonsLayers, "03_120")




		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("BN")

		oTopoScheme.Load(False)
		MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "03_124")
		oTopoScheme.CreateDBPolylinesByCentroidLayerForDorit()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TmstClose", CommandFlags.Session)>
	Public Sub TmstClose()
		If mfTopoActions IsNot Nothing Then
			mfTopoActions.Close()
			mfTopoActions.Dispose()
			mfTopoActions = Nothing
		End If

	End Sub
	<CommandMethod("dmList", CommandFlags.Session)>
	Public Sub dmList()

	End Sub
	<CommandMethod("Tst", CommandFlags.Session)>
	Public Sub TestGeo()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Arc")
		'		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim dBulge As Double = 0.0
		Dim oArc2d As CircularArc2d = Nothing
		Dim tPriorPoint, tCurrentPoint2d As Point2d

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		oArc2d = New CircularArc2d(tPriorPoint, tCurrentPoint2d, dBulge, False)

		Try
			Dim oArc As Arc = New Arc(New Point3d(0, 0, 0), 2, 3.0, 2.0)
			oArc.Layer = "Red"

			DMAcadExt.AcadTransaction.AppendEntity(oArc)
			DMAcadExt.AcadDocument.WriteMessage("Total =" & CStr(oArc.TotalAngle) & ":" & oArc.Normal.Z)

			Dim oArc1 As Arc = New Arc(New Point3d(0, 0, 0), 2, 5.0, 4.0)
			oArc1.Layer = "Green"
			DMAcadExt.AcadTransaction.AppendEntity(oArc1)
			DMAcadExt.AcadDocument.WriteMessage("Total1 =" & CStr(oArc1.TotalAngle) & ":" & oArc1.Normal.Z)


			Dim oArc2 As Arc = New Arc(New Point3d(0, 0, 0), 2, 6.0, -1.0)
			oArc2.Layer = "Blue"
			DMAcadExt.AcadTransaction.AppendEntity(oArc2)
			DMAcadExt.AcadDocument.WriteMessage("Total2 =" & CStr(oArc2.TotalAngle) & ":" & oArc2.Normal.Z)
			'		DMAcadExt.AcadDocument.WriteDebugMessage("Center=" & CStr(oArc.Center.X) & "," & CStr(oArc.Center.Y))



		Catch ex As Exception

		End Try



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	<CommandMethod("TplnNatA", CommandFlags.Session)>
	Public Sub TplnNatA()
		Const sInfoBlockName As String = "PCLS014"
		Const sPointBlockName As String = "SRVS007"

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim colBlockRefs As ICollection(Of DMAcadExt.AttachPair)
		Dim colAttachPairs As ICollection(Of DMAcadExt.AttachPair)

		Dim colSegments As ICollection(Of DMAcadExt.TplnSegment)

		Dim oAttach As DMAcadExt.AttachEntities
		Dim oaAttachPairs() As DMAcadExt.AttachPair

		Dim oaBlockRefsA() As ObjectId
		Dim oaBlockRefsB() As ObjectId

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		'   Dim oaPointsA As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray(iMainUB)
		'  Dim oaPointsB As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray(iMainUB)

		Dim oaLength() As Double
		Dim saLabels() As String

		Dim oaPointsA As DMAcadExt.TplnPointArray
		Dim oaPointsB As DMAcadExt.TplnPointArray
		Try

			Dim iMainUB As Integer
			colBlockRefs = DMAcadExt.AcadTransaction.GetAllBlockRefsForAttach(sInfoBlockName)
			colSegments = DMAcadExt.AcadTransaction.GetSegments()
			iMainUB = colBlockRefs.Count - 1
			' MessageBox.Show(CStr(colBlockRefs.Count) & ":" & CStr(colSegments.Count), "03_240")
			oAttach = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToEntity2D, colBlockRefs) '"PCLS014"
			oAttach.AddSegments(colSegments)
			oAttach.Calculate(False)
			System.Windows.Forms.MessageBox.Show("!!!!" & CStr("???"), "03_520b")
			colAttachPairs = oAttach.GetAttachedPairs()   'Line +  "PCLS014"
			oaAttachPairs = oAttach.Output ''Line +  "PCLS014"
			Dim oAttachedSegment As DMAcadExt.AttachedSegment
			'	MessageBox.Show(CStr(colAttachPairs.Count) & ":" & CStr(oaAttachPairs.GetUpperBound(0)) & ":" & CStr(oAttach.AttachedCount), "03_252")
			'	MessageBox.Show(oaAttachPairs(0).ToString, "03_253")


			'	DMAcadExt.AcadDocument.WriteMessage("Total1 =" & CStr(oArc1.TotalAngle) & ":" & oArc1.Normal.Z)
			oaPointsA = New DMAcadExt.TplnPointArray(iMainUB)
			oaPointsB = New DMAcadExt.TplnPointArray(iMainUB)
			ReDim oaLength(iMainUB)
			ReDim saLabels(iMainUB)


			Dim oSegment As DMAcadExt.TplnSegment

			For iIndex As Integer = 0 To oaAttachPairs.GetUpperBound(0)
				If oaAttachPairs(iIndex) IsNot Nothing Then
					Try
						oAttachedSegment = DirectCast(oaAttachPairs(iIndex), DMAcadExt.AttachedSegment)
						oSegment = oAttachedSegment.Segment
						If oSegment IsNot Nothing Then
							oaPointsA.Item(iIndex) = oSegment.StartPoint
							oaPointsB.Item(iIndex) = oSegment.EndPoint
							oaLength(iIndex) = oaPointsA.Item(iIndex).Distance(oaPointsB.Item(iIndex))
							saLabels(iIndex) = oSegment.UD_Label
						End If
					Catch ex As Exception
						MessageBox.Show(ex.Message & vbCrLf & ex.StackTrace, "03_255")
					End Try


				End If

			Next

			oAttach = New DMAcadExt.AttachEntities(DMAcadExt.enAttachType.PointToPoint)

			Dim oPointArray As DMAcadExt.TplnPointArray = DMAcadExt.AcadTransaction.GetAllBlockRefInsPoint(sPointBlockName) 'SRV007
			'	MessageBox.Show(CStr(oPointArray.UpperBound), "03_256b")
			oAttach.AddPointArray(oPointArray, False)
			'	MessageBox.Show(CStr(oaPointsA.UpperBound), "03_256c")
			oAttach.SourcePoints = oaPointsA

			oAttach.Calculate(False)
			oaBlockRefsA = oAttach.OutputAcadObjects

			oAttach.SourcePoints = oaPointsB
			oAttach.Calculate(False)
			oaBlockRefsB = oAttach.OutputAcadObjects


		Catch oEx As Exception
			Return
		End Try


		Dim sFileName As String = DMAcadExt.AcadDocument.GetCurrentDWGName()
		Dim oFileInfo As IO.FileInfo

		Dim saFile() As String
		Dim sBlock As String
		If sFileName IsNot Nothing Then
			oFileInfo = New IO.FileInfo(sFileName)

			saFile = Split(oFileInfo.Name, ".")
			sBlock = saFile(0)
		Else
			sBlock = ""
		End If


		Dim bPoint As Boolean
		Dim iaAttribIndex() As Integer = {0, 1}
		Dim sValuePointA As String
		Dim sValuePointB As String

		Dim saValue() As String
		Dim oRep As ExcelReport.Report = New ExcelReport.Report(False)
		Dim dRef As Double
		oRep.Open()
		oRep.SetValue(0, 0, "גוש מספר")
		oRep.SetValue(0, 1, "ממס' נקודה")
		oRep.SetValue(0, 2, "למס' נקודה")
		oRep.SetValue(0, 3, "מרחק מדוד")
		oRep.SetValue(0, 4, "מרחק מחושב")
		oRep.SetValue(0, 5, "הפרש בין מדוד למחושב")
		oRep.SetHeaderFormat(0, 5)
		Dim sNumFormat As String = oRep.GetWinNumFormat(2, TriState.True)
		For iIndex As Integer = 0 To oaAttachPairs.GetUpperBound(0)
			oRep.SetValue(iIndex + 1, 0, sBlock)
			' If Not oaBlockRefsA(iIndex).IsNull Then
			sValuePointA = DMAcadExt.AcadTransaction.GetAttribText(oaBlockRefsA(iIndex), True, bPoint, 0)
			oRep.SetValue(iIndex + 1, 1, sValuePointA)
			'  End If
			If Not oaBlockRefsB(iIndex).IsNull Then
				sValuePointB = DMAcadExt.AcadTransaction.GetAttribText(oaBlockRefsB(iIndex), True, bPoint, 0)
				oRep.SetValue(iIndex + 1, 2, sValuePointB)
			End If




			If Not oaAttachPairs(iIndex).SourceObjID.IsNull Then
				saValue = DMAcadExt.AcadTransaction.GetAttribText(oaAttachPairs(iIndex).SourceObjID, True, bPoint, iaAttribIndex)
				oRep.SetValue(iIndex + 1, 3, saValue(1), sNumFormat)
				'   oRep.SetValue(iIndex + 1, 4, saValue(0), sNumFormat)
				oRep.SetValue(iIndex + 1, 4, saLabels(iIndex))
				'  oRep.SetValue(iIndex + 1, 7, oaPointsA.Item(iIndex).Coordinates)
				'   oRep.SetValue(iIndex + 1, 8, oaPointsB.Item(iIndex).Coordinates)


				If (saValue(0) & saValue(1)).Length <> 0 Then
					dRef = zzAttribTextToDouble(saValue(1)) - zzAttribTextToDouble(saValue(0))
					oRep.SetValue(iIndex + 1, 5, dRef, sNumFormat)
				End If
				'  oRep.SetValue(iIndex + 1, 6, FormatNumber(oaLength(iIndex), 3), sNumFormat)
				'  oRep.SetValue(iIndex + 1, 6, saLabels(iIndex))


				'	MessageBox.Show(CStr(saValue(0) & ":" & saValue(1)), "03_292")
			End If


		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()


	End Sub
	Private Function zzAttribTextToDouble(sValue As String) As Double
		Dim dRes As Double
		If String.IsNullOrEmpty(sValue) Then
			dRes = 0.0
		Else
			Dim saVal() As String = Split(sValue, "=")
			If saVal.GetUpperBound(0) = 0 Then
				Double.TryParse(saVal(0), dRes)
			Else
				Double.TryParse(saVal(1), dRes)
			End If

		End If
		Return dRes
	End Function
	<CommandMethod("TestOleDB")>
	Public Sub TestOleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB()
		TPlServerDB.dmDBManager.CheckProviderFactory()
	End Sub
	<CommandMethod("TplnV1")>
	Public Sub TplnV1()
		DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		Dim iTest As Integer = TopoDefs.giToposUB
		TopoManager.TPlanGraph.TplnProject.InitAppName()
		Parameters.Init()
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()   'TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB


		If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
			frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
			TPlServerDB.ServerDB.AddInitialize()
			TopoManager.TPlanGraph.TplnProject.InitializeList()
			If mfTopoActions Is Nothing OrElse mfTopoActions.IsDisposed Then
				TopoDefs.InitTaba()
				mfTopoActions = New frmTopoActionsT()
			End If
			TopoManager.TPlanGraph.TplnProject.InitializeView()
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.NonInPlaceMainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActions)
		End If
	End Sub
	<CommandMethod("TplnBatch")>
	Public Sub TplnBatch()
		'		TopoManager.TPlanGraph.TplnProject.InitAppName()
		'		TopoManager.TPlanGraph.TplnProject.InitializeDB()

		DMAcadExt.AcadDocument.SetLogName()
		TopoManager.TPlanGraph.TplnProject.InitializeList()
		If mfDWGBatch Is Nothing OrElse mfDWGBatch.IsDisposed Then
			mfDWGBatch = New TopoUI.frmDWGBatch()
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDWGBatch)

	End Sub

	<CommandMethod("Excv")>
	Public Sub ExecUtilForm()



		DMAcadExt.AcadDocument.SetLogName()

		If mfExecUtil Is Nothing OrElse mfExecUtil.IsDisposed Then
			mfExecUtil = New TopoUI.frmExecUtil
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfExecUtil)

	End Sub


	<CommandMethod("TplnPrc")>
	Public Sub TplnParcels()
		'		TopoManager.TPlanGraph.TplnProject.InitAppName()
		'		TopoManager.TPlanGraph.TplnProject.InitializeDB()


		'	TopoManager.TPlanGraph.TplnProject.InitializeList()

		DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		If mfApp Is Nothing OrElse mfApp.IsDisposed Then
			mfApp = New TopoUI.frmApp()
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfApp)
		'	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")

	End Sub
	<CommandMethod("WWW")>
	Public Sub WWW()
		Dim iLT As UInt32 = DMAcadExt.AcadDocument.GetLayerTransparency
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		oEditor.WriteMessage("LT= " & CStr(iLT))
	End Sub
	'   <CommandMethod("QQQ")> _
	Public Sub Test1()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
				oEntA.Visible = False
			Catch oEx As Exception
				Return
			End Try
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Test2")>
	Public Sub Test2()
		Dim oPolylineJig As UnidivNet.PolylineJig
		Dim oPromptResult As PromptResult
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity
		Dim oCurve As Curve
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEditor.WriteMessage(vbCrLf)
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
				If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					oCurve = DirectCast(oEntA, Curve)
					If oCurve IsNot Nothing Then
						Dim oaCurves() As Curve = {oCurve}



						oPolylineJig = New UnidivNet.PolylineJig(oaCurves)
						oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

						If oPromptResult.Status = PromptStatus.OK Then
							oPolylineJig.CreateEntity()

						End If

					End If


				End If
			Catch oEx As Exception
				Return
			End Try
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()







	End Sub
	'<CommandMethod("QQQ")> _
	Public Sub Test3()
		Dim oPolylineJig As UnidivNet.ShowPolylineJig
		Dim oPromptResult As PromptResult
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity
		Dim oCurve As Polyline
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEditor.WriteMessage(vbCrLf)
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
				If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					oCurve = DirectCast(oEntA, Polyline)
					If oCurve IsNot Nothing Then




						oPolylineJig = New UnidivNet.ShowPolylineJig(oCurve)
						oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

						If oPromptResult.Status = PromptStatus.OK Then


						End If

					End If


				End If
			Catch oEx As Exception
				Return
			End Try
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		' TplnPointKeyLong.vb() : Line 50






	End Sub
	<CommandMethod("Test04")>
	Public Sub Test04()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		'   Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		'  Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

		Dim ptPoint1Res, ptPoint2Res As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptPoint1Res = oEditor.GetPoint("Point1..")
		If ptPoint1Res.Status = PromptStatus.OK Then
			ptPoint2Res = oEditor.GetPoint("Point2..")
			If ptPoint2Res.Status = PromptStatus.OK Then
				oEditor.DrawVector(ptPoint1Res.Value, ptPoint2Res.Value, 1, False)

			End If

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Test04a")>
	Public Sub Test04a()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		'   Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		'  Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

		Dim ptPoint1Res, ptPoint2Res As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oEnt As DBObject = Nothing
		Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
		Dim tTransform As Matrix3d = New Matrix3d()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptPoint1Res = oEditor.GetPoint("Point1..")
		If ptPoint1Res.Status = PromptStatus.OK Then
			ptPoint2Res = oEditor.GetPoint("Point2..")
			If ptPoint2Res.Status = PromptStatus.OK Then
				' Dim oPline As Polyline = New Polyline(2)
				Dim tVector1, tVector2 As Vector3d
				'  oPline.AddVertexAt(0, DMAcadExt.TPlnPoint.Point3dTo2d(ptPoint1Res.Value), 0, 0, 0)
				'  oPline.AddVertexAt(1, DMAcadExt.TPlnPoint.Point3dTo2d(ptPoint2Res.Value), 0, 0, 0)
				' DMAcadExt.AcadTransaction.AppendEntity(oPline, False)
				tVector1 = New Vector3d(ptPoint1Res.Value.X, ptPoint1Res.Value.Y, 0.0)
				tVector2 = New Vector3d(ptPoint2Res.Value.X, ptPoint1Res.Value.Y, 0.0)

				oResBuffer.Add(tVector1)
				oResBuffer.Add(tVector2)

				tTransform = Autodesk.AutoCAD.Geometry.Matrix3d.Scaling(2.0, ptPoint1Res.Value)
				oEditor.DrawVectors(oResBuffer, tTransform)

			End If

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Test05")>
	Public Sub Test05()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		'Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult

		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet = Nothing



		Try
			oPromptOpt.AllowDuplicates = False
			oPromptOpt.SingleOnly = False
			oPromptOpt.MessageForAdding = "Add +***"
			oPromptOpt.MessageForRemoval = "Remove -***"

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try

		ptRes = oEditor.GetSelection(oPromptOpt)




		Dim col As ICollection(Of DMAcadExt.IUD_Link) = New System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
		'Dim oLine As Line = Nothing
		Dim oLink As DMAcadExt.IUD_Link

		'	Dim oUdLine As DMAcadExt.TplnLine

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptRes.Value()

			For i As Integer = 0 To oSelSet.Count - 1

				oLink = DMAcadExt.AcadTransaction.GetLink(oSelSet.Item(i).ObjectId, OpenMode.ForRead, True)

				If oLink IsNot Nothing Then

					DMAcadExt.AcadDocument.WriteDebugMessage("ss " & oLink.GetInfo(4))
					col.Add(oLink)

				End If

			Next

		End If
		'	DMCommon.Debug.MsgBox("13_014J", col.Count, oSelSet.Count)
		Dim tVectorSet As DMAcadExt.DrawVectorSet.VectorSet = New DMAcadExt.DrawVectorSet.VectorSet(col, oSelSet.Count, 1, False)
		'	DMCommon.Debug.MsgBox("13_014K", col.Count, oSelSet.Count)
		DMAcadExt.AcadDocument.SetDrawVectorSet(tVectorSet, -1)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub



	<CommandMethod("QQQ")>
	Public Sub TestBlocks()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim colNodes As ObjectIdCollection
		Dim sNodeBlocks As String = "C1610,C1611,C1615,C1616"
		Dim sNodeLayers As String = "UD_NewPoints,1610_0"


		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		colNodes = DMAcadExt.AcadTransaction.GetBlockRefsNew(sNodeBlocks, sNodeLayers)
		oEditor.WriteMessage("A: " & colNodes.Count.ToString())
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestSnap")>
	Public Sub TestSnap()
		Dim iSnapMode As System.Int32 = CInt(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName))
		Dim tSnapUnitPoint As Point2d = DirectCast(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName), Point2d)

		DMCommon.Debug.MsgBox("231122_1", iSnapMode, tSnapUnitPoint)
	End Sub
	<CommandMethod("TestPJig")>
	Public Sub TestPgonJig()
		'	08-923 27 54 Maya

		Dim oPromptResult As PromptResult
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity
		Dim oCurve As Polyline = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try

				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)

				If oEntA.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					oCurve = DirectCast(oEntA, Polyline)


					Dim oPolylineJig As TopoManager.TopoScheme.TopoPgonJig

					oPolylineJig = New TopoManager.TopoScheme.TopoPgonJig("fragments", oCurve)

					oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPolylineJig)

					If oPromptResult.Status = PromptStatus.OK Then

					End If
				End If

			Catch oEx As Exception
			End Try

		End If
	End Sub
	<CommandMethod("TestRJig")>
	Public Sub TestRJig()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
		'	Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult

		Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapModeSysVarName, 1)
		Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(DMAcadExt.AcadConst.SnapUnitSysVarName, New Point2d(50, 50))


		'Dim oJig As DMAcadExt.PointJig = New DMAcadExt.RectangleJig(400.0, 300.0)
		Dim oJig As DMAcadExt.PointJig = New DMAcadExt.FrameJig(400.0, 300.0, 6.0)

		Dim tPoint As Point2d

		Dim oPromptResult As PromptResult = oEditor.Drag(oJig)


		DMAcadExt.AcadDocument.WriteMessage("after select Ss12" & vbCrLf)
		If oPromptResult.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = oJig.GetPoint()
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

				DMAcadExt.AcadTransaction.InsertPoint(tPoint)
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()


				Dim oPline As Polyline = New Polyline()





			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Unidiv - SelectPoint")
			End Try
		End If
		'	Return oPromptResult.Status
	End Sub

	<CommandMethod("PArea")>
	Public Sub PArea()
		Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
		Dim mcolSelectionBlockRefs As ObjectIdCollection = New ObjectIdCollection()

		oExcelAppExt.Open()

		Try


			Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
			Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
			'Dim oPolyline As Polyline = New Polyline
			Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
			'	Dim colLines As ObjectIdCollection = New ObjectIdCollection
			Dim iTotal As Integer = 0
			Dim iFound As Integer = 0
			mcolSelectionBlockRefs.Clear()
			DMAcadExt.AcadTransaction.Start()
			Try
				oPromptOpt.AllowDuplicates = False
				oPromptOpt.SingleOnly = False
				oPromptOpt.MessageForAdding = "Add ***"
				oPromptOpt.MessageForRemoval = "Remove ***"

			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
			End Try
			Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
			Dim taObjIDs() As ObjectId
			Dim oCurve As Curve
			Dim dSumOfArea As Double = 0.0
			Dim iCount As Integer

			ptRes = oEditor.GetSelection(oPromptOpt)
			' System.Windows.Forms.MessageBox.Show(ptRes.Status.ToString(), "05_377")
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				oSelSet = ptRes.Value()
				'	zzTestSet(oSelSet, True)
				taObjIDs = oSelSet.GetObjectIds()
				For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)

					oCurve = DMAcadExt.AcadTransaction.GetCurve(taObjIDs(iIndex), True, OpenMode.ForRead)
					If oCurve IsNot Nothing Then
						oExcelAppExt.SetNextValue(0, "", oCurve.GetRXClass().Name, oCurve.Layer, oCurve.Area)
						mcolSelectionBlockRefs.Add(taObjIDs(iIndex))
						iCount += 1
						dSumOfArea += oCurve.Area
					End If

				Next
				oExcelAppExt.SetNextValue(0, "Total", iCount, dSumOfArea)

			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
				mcolSelectionBlockRefs.Clear()
				'Exit Do
			Else
				DMAcadExt.AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
				'Exit Do
			End If
			DMAcadExt.AcadTransaction.Terminate()
			'	Loop
			'	oEditor.SelectAll()
		Catch oEx As Exception

		End Try
	End Sub
	<CommandMethod("TplnOpen")>
	Public Sub TplnOpen()
		'  If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
		DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		'    End If


		TopoManager.TPlanGraph.TplnProject.InitAppName()

		'   DMAcadExt.AcadDocument.InitDebug()
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()

		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()

		'TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control

		If mfPrjThemes Is Nothing OrElse mfPrjThemes.IsDisposed Then
			mfPrjThemes = New TopoUI.frmPrjThemes()
		End If

		'MessageBox.Show(CStr(
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfPrjThemes)
		'	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")

		mfPrjThemes.Left = 40
		mfPrjThemes.Top = 40
	End Sub
	<CommandMethod("UD_Gen")>
	Public Sub UD_Gen()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Unidiv
		End If
		'   frmHanitControl.vb() : Line 25

		TopoManager.TPlanGraph.TplnProject.InitAppName()
		'	MessageBox.Show("", "07_117")
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		'	MessageBox.Show("", "07_300")
		'	TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		If mfHanitGeneral Is Nothing OrElse mfHanitGeneral.IsDisposed Then
			mfHanitGeneral = New TopoUI.frmHanitGeneral(False)
		End If

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfHanitGeneral)
		'	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
		mfHanitGeneral.Left = 40
		mfHanitGeneral.Top = 40
	End Sub
	<CommandMethod("BAEd")>
	Public Sub BAEd()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If


		TopoManager.TPlanGraph.TplnProject.InitAppName()

		''''''''''''''''''''''''''''		TopoManager.TPlanGraph.TplnProject.InitializeDB_SQL()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		If mfEditBlockRefs Is Nothing OrElse mfEditBlockRefs.IsDisposed Then
			mfEditBlockRefs = New TopoUI.frmEditBlockRefs
		End If
		'MessageBox.Show(CStr(
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditBlockRefs)
		'	MessageBox.Show(mfApp.DialogResult.ToString(), "01_400")
		mfEditBlockRefs.Left = 40
		mfEditBlockRefs.Top = 40
	End Sub
	<CommandMethod("PrjInfo")>
	Public Sub GetProjectInfo()
		'סָעִיף section
		Dim oDWGProjectData As TopoManager.DWGProjectData
		Dim iProjectCode As Integer
		Dim iDetailNo As Integer
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()


		oDWGProjectData = New TopoManager.DWGProjectData()

		oDWGProjectData.OpenData(False, True)
		iProjectCode = oDWGProjectData.ProjectCode
		iDetailNo = oDWGProjectData.DetailNo

		Dim oMySettings As My.MySettings = New My.MySettings()
		Dim iLastProjectCode As Integer = oMySettings.LastProjectCode
		Dim iLastDetailNo As Integer = oMySettings.LastDetailNo



		oEditor.WriteMessage("Project No: " & iProjectCode.ToString() & vbCrLf & "Work No: " & iDetailNo.ToString() & vbCrLf & "Previous: " & iLastProjectCode.ToString() & "/" & iLastDetailNo.ToString() & vbCrLf)
		'  System.Windows.Forms.MessageBox.Show(CStr(miProjectCode) & ":" & CStr(miDetailNo) & vbCrLf & moDWGProjectData.ProjectCode.ToString() & ":" & moDWGProjectData.DetailNo.ToString(), "02_980b")
		DMAcadExt.AcadTransaction.Terminate()

		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("AddB")>
	Public Sub AdditionalBlock()
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("1603")
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		oAcadBlock.LoadAllReferences()
		oAcadBlock.OpenAdditionalBlock("1603@a", "M:\Dm_Work\Blocks\Additional")
		oAcadBlock.InsertAddBlockRefs()

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TopoPgons")>
	Public Sub TopoPolygons()
		Const sTopoName As String = "ExproLine"

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
		'  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sTopoName)

		oTopoScheme.Load(False)
		Dim colPgons As System.Collections.ObjectModel.Collection(Of TopoScheme.tsPolygon) = oTopoScheme.Polygons
		For Each oPgon As TopoScheme.tsPolygon In colPgons
			DMAcadExt.AcadDocument.WriteDebugMessage(oPgon.ID.ToString())


		Next
		oTopoScheme.Close()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TTMP")>
	Public Sub TempBatch()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		DMAcadExt.AcadTransaction.DrawNet(101, 101, 0.1, 0.1)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("VPNet")>
	Public Sub DrawViewportNet()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.DrawNet(DMAcadExt.KeyPoint.Tolerance, DMAcadExt.KeyPoint.Tolerance)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("LTS")>
	Public Sub LoadTopoScheme()
		'  Dim sToponame As String = "Parcels"
		'  Dim sToponame As String = "LotsK"
		'  Dim sToponame As String = "TopoOne"
		Dim sToponame As String = "fragments"




		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		DMAcadExt.AcadDocument.SetLogName()
		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
		'  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sToponame)

		oTopoScheme.Load(False)
		oTopoScheme.Close()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TTS")>
	Public Sub TestTopoScheme()
		Const bRemovePseudoPoints As Boolean = False
		Const bByCentroidLayer As Boolean = False
		Const bCheckIsthmus As Boolean = False
		Const bCheckIslands As Boolean = True



		'Dim sTopoName As String = "Stage_3_1"
		Dim sToponame As String = "Parcels"

		'  Dim sToponame As String = "LotsK"
		'  Dim sToponame As String = "TopoOne"
		'   Dim sToponame As String = "TopoMany"






		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
		'  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sToponame)



		DMAcadExt.AcadDocument.SetLogName()
		Dim oParcelTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sToponame, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oParcelTopology IsNot Nothing Then


			oTopoScheme.Load(bRemovePseudoPoints, oParcelTopology)
			oParcelTopology.Close()
			'   DMCommon.Debug.MsgBox("08_100", oTopoScheme.Elements.Polygons.Count, oTopoScheme.Elements.Nodes.Count)
			If bCheckIsthmus Then
				oTopoScheme.CalcIsthmus()
			End If

			If bCheckIslands Then
				oTopoScheme.CheckIslands()
			End If


			' DMCommon.Debug.MsgBox("08_100q", oTopoScheme.Elements.Polygons.Count, oTopoScheme.Elements.Nodes.Count)
			If bRemovePseudoPoints Then
				oTopoScheme.RemovePseudoPoints()

			End If
			'   DMCommon.Debug.MsgBox("08_201")
			If bCheckIsthmus Then
				oTopoScheme.CreateDBPolylineMPlus(True, bByCentroidLayer)
			End If
			For Each oPgonScheme As TopoScheme.tsPolygon In oTopoScheme.Polygons
				DMCommon.Debug.MsgBox("TestTopo", oPgonScheme.ID, oPgonScheme.IsInner)

			Next

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			'  DMCommon.Debug.MsgBox("08_101", oTopoScheme.Elements.Polygons.Count, oTopoScheme.Elements.Nodes.Count)
			' oTopoScheme.CreateBulgeVertexArray(False)
			''''''''''''''''''''''''  oTopoScheme.RemovePseudoPoints()
			'  oTopoScheme.CreateDBPolylines()
			'Dim oElem As TopoScheme.tsElement = oTopoScheme.GetElement(1721)
			'If oElem Is Nothing Then
			'   '  MessageBox.Show("oElem Is Nothing", "08_200")
			'Else
			'   ' MessageBox.Show(oElem.ElemType.ToString(), "08_200")
			'End If


			'''''''''   oTopoScheme.PrintInfo()

		Else
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If

	End Sub

	<CommandMethod("TLT")>
	Public Sub TestLineTopo()
		'  Dim sToponame As String = "Parcels"
		'  Dim sToponame As String = "LotsK"
		'  Dim sToponame As String = "TopoOne"
		Dim sTopoName As String = "LineTopoB"

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim colTopoLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
		Dim colNodeEntities As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = Nothing
		Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
		Dim oPrevNode As Autodesk.Gis.Map.Topology.Node
		Dim oNextNode As Autodesk.Gis.Map.Topology.Node
		Dim oPrevLeftFullEdge As Autodesk.Gis.Map.Topology.FullEdge
		Dim oPrevRightFullEdge As Autodesk.Gis.Map.Topology.FullEdge

		Dim oNextLeftFullEdge As Autodesk.Gis.Map.Topology.FullEdge
		Dim oNextRightFullEdge As Autodesk.Gis.Map.Topology.FullEdge

		Dim sPrevLeftFullEdge As String
		Dim sPrevRightFullEdge As String

		Dim sNextLeftFullEdge As String
		Dim sNextrightFullEdge As String
		Dim colHalfEdges As Autodesk.Gis.Map.Topology.HalfEdgeCollection
		Dim dicEdgeCounterd As Dictionary(Of Integer, Counter) = New Dictionary(Of Integer, Counter)()
		If False Then
			Try
				oTopos.Create(sTopoName, colTopoLinks, colNodeEntities, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, Autodesk.Gis.Map.Topology.CreateOptions.IgnoreIncompleteArea, 0.001)
				'   iStageLinksCount = colLinks.Count
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - TestLineTopo")
				DMAcadExt.AcadTransaction.SetLayer(colTopoLinks, "Topo")
			End Try
		End If
		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim colEdges As Autodesk.Gis.Map.Topology.FullEdgeCollection = oTopoModel.GetFullEdges()
		Dim colNodes As Autodesk.Gis.Map.Topology.NodeCollection = oTopoModel.GetNodes()

		For Each oNode As Autodesk.Gis.Map.Topology.Node In colNodes
			Try
				dicEdgeCounterd.Add(oNode.ID, New Counter(oNode.ID))
				colHalfEdges = oNode.GetEdges()
				DMAcadExt.AcadDocument.WriteDebugMessage("Node: ID=" & oNode.ID.ToString() & "; " & colHalfEdges.Count.ToString())
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadDocument.WriteDebugMessage("Node: ID=" & oNode.ID.ToString() & "; colHalfEdges=Nothing")
			End Try

		Next


		For Each oFullEdge As Autodesk.Gis.Map.Topology.FullEdge In colEdges
			oPrevNode = oFullEdge.GetNextNode(False)
			dicEdgeCounterd.Item(oPrevNode.ID).AddOne()
			oNextNode = oFullEdge.GetNextNode(True)
			dicEdgeCounterd.Item(oNextNode.ID).AddOne()
			Try
				oPrevRightFullEdge = oFullEdge.GetNextEdge(False, False)
				If oPrevRightFullEdge IsNot Nothing Then
					sPrevRightFullEdge = oPrevRightFullEdge.ID.ToString()
				Else
					sPrevRightFullEdge = "PrevRightEdge Nothing"
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				sPrevRightFullEdge = "PrevRightEdge "
				If oMapEx.ErrorCode <> 2063 Then
					DMAcadExt.AcadDocument.WriteDebugMessage("ErrCodePrevRight: " & oMapEx.ErrorCode.ToString())
				End If
			End Try

			Try
				oPrevLeftFullEdge = oFullEdge.GetNextEdge(False, True)
				If oPrevLeftFullEdge IsNot Nothing Then
					sPrevLeftFullEdge = oPrevLeftFullEdge.ID.ToString()
				Else
					sPrevLeftFullEdge = "PrevLeftEdge Nothing"
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				sPrevLeftFullEdge = "PrevLeftEdge "
				If oMapEx.ErrorCode <> 2063 Then
					DMAcadExt.AcadDocument.WriteDebugMessage("ErrCodePrevLeft: " & oMapEx.ErrorCode.ToString())
				End If
			End Try

			Try
				oNextRightFullEdge = oFullEdge.GetNextEdge(True, False)
				If oNextRightFullEdge IsNot Nothing Then
					sNextrightFullEdge = oNextRightFullEdge.ID.ToString()
				Else
					sNextrightFullEdge = "NextEdge Nothing"
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				sNextrightFullEdge = "NextRightFullEdge"
				If oMapEx.ErrorCode <> 2063 Then
					DMAcadExt.AcadDocument.WriteDebugMessage("ErrCodeNextRight: " & oMapEx.ErrorCode.ToString())
				End If
			End Try


			Try
				oNextLeftFullEdge = oFullEdge.GetNextEdge(True, True)
				If oNextLeftFullEdge IsNot Nothing Then
					sNextLeftFullEdge = oNextLeftFullEdge.ID.ToString()
				Else
					sNextLeftFullEdge = "NextEdge Nothing"
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				sNextLeftFullEdge = "NextLeftFullEdge"
				If oMapEx.ErrorCode <> 2063 Then
					DMAcadExt.AcadDocument.WriteDebugMessage("ErrCodeNextLeft: " & oMapEx.ErrorCode.ToString())
				End If
			End Try


			DMAcadExt.AcadDocument.WriteDebugMessage("FullEdge: ID=" & oFullEdge.ID.ToString() & "; " & oPrevNode.ID.ToString() & "<->" & oNextNode.ID.ToString() & "; Edge: " & sPrevRightFullEdge & "<->" & sNextrightFullEdge & "; " & sPrevLeftFullEdge & "<->" & sNextLeftFullEdge)

			' DMAcadExt.AcadDocument.WriteDebugMessage(" Edge: " & sPrevFullEdge & "<->" & sNextFullEdge)


		Next

		For Each oCounter As Counter In dicEdgeCounterd.Values
			DMAcadExt.AcadDocument.WriteDebugMessage("Node: ID=" & oCounter.ID.ToString() & " - " & oCounter.Number.ToString())
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TMP")>
	Public Sub TestMarkPoints()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("TopoA")
		'  Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Stage_2")
		'   Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("fragments")
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology("Blocks")



		DMAcadExt.AcadDocument.SetLogName()
		'   Dim bCurrentLayerOK As Boolean






		' bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("1620", DMAcadExt.DMApp.AppID, True, True)
		oTopoScheme.Load(False)

		MessageBox.Show(CStr(oTopoScheme.Elements.Polygons.Count), "08_100")
		Dim oResList As List(Of InitInsertData) = oTopoScheme.GetMarkPoints(14.0 * DMAcadExt.AcadDocument.GetDWGScaleFactor)

		MessageBox.Show(CStr(oResList.Count), "08_110")

		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("1620", "M:\Dm_Work\Blocks\Mavat-2010")
		Dim tBlockRefData As DMAcadExt.BlockRefData
		Dim iParity As Integer = 0
		oAcadBlock.OpenForRight()
		For Each tInitInsertData As InitInsertData In oResList
			'  DMAcadExt.AcadTransaction.InsertPoint(tInitInsertData.Position)
			tBlockRefData = New DMAcadExt.BlockRefData()

			tBlockRefData.Position = tInitInsertData.Position
			If iParity = 0 Then
				tBlockRefData.ColorIndex = 2S
			Else
				tBlockRefData.ColorIndex = 3S
			End If

			'    tBlockRefData.Layer = msTazarMapBlockLayer
			tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(DMAcadExt.AcadDocument.GetDWGScaleFactor)
			tBlockRefData.Rotation = tInitInsertData.Rotation + Convert.ToDouble(iParity) * Math.PI
			oAcadBlock.InsertRefNewNew(tBlockRefData)
			iParity = (iParity + 1) Mod 2

		Next


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestDelP")>
	Public Sub TestDelPgon()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		Dim sTopoName As String = "Test"
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
		DMCommon.Debug.MsgBox("02_113c", sTopoName)

		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oProject.Topologies
		DMCommon.Debug.MsgBox("02_113d", sTopoName)
		Dim sTest As String = "0"
		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel
		Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = Nothing
		Dim iPgonID As Integer
		If sTopoName IsNot Nothing AndAlso oTopos IsNot Nothing AndAlso oTopos.Exists(sTopoName) Then
			oTopoModel = oTopos.Item(sTopoName)
			DMCommon.Debug.MsgBox("02_113e", sTopoName)
			If oTopoModel IsNot Nothing Then
				If False Then


					oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					DMCommon.Debug.MsgBox("02_113f", "Opened")

					Try
						sTest = "a"

						If oTopos.Exists(sTopoName) Then
							sTest &= "b"
							colPolygons = oTopoModel.GetPolygons()
						Else
							DMCommon.Debug.MsgBox("02_113g", sTopoName)

						End If

						sTest &= "c"

						For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
							Try
								sTest &= "d"
								'oTopoModel.DeletePolygon(oPolygon)
								iPgonID = oPolygon.ID
								sTest &= "e"
								oPolygon.Dispose()
								oPolygon = Nothing
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "GetID " & sTest, False)
								sTest &= "f"
							End Try
							Exit For

						Next
						sTest &= "f"
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "GetPolygons " & sTest, False)

					End Try

					oTopoModel.Close()
				End If
				iPgonID = 7
				sTest &= "k"
				oTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
				Try
					sTest &= "L"
					oTopoModel.DeletePolygon(iPgonID)
					sTest &= "M"
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DeletePolygon " & sTest & "|" & iPgonID.ToString(), False)

				End Try
				oTopoModel.Close()
			End If

		End If
		'   System.Windows.Forms.MessageBox.Show(sTopoName, "02_109R")



		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TopoToPl")>
	Public Sub TopoPgonToPolyline()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim sTopoName As String = "Stage_3_1"
		Dim oTopoModel As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim oTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sTopoName)


		Dim tSelectedPoint As Point3d
		Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon = Nothing
		oTopoScheme.Load(True, oTopoModel)
		If zzSelectPoint(tSelectedPoint, False) Then
			Try
				oPolygon = oTopoModel.FindPolygon(tSelectedPoint)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				If oMapEx.ErrorCode <> 3 Then
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & sTopoName & vbCrLf & tSelectedPoint.ToString(), "01_87f")
					DMAcadExt.AcadDocument.WriteDebugMessage(oMapEx.ErrorCode & "; " & sTopoName & "; " & tSelectedPoint.ToString())
				End If

			End Try

			If oPolygon IsNot Nothing Then

				oTopoScheme.CreatePgonDBPolyline(True, oPolygon.ID)
			End If

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("IsExt")>
	Public Sub IsExtend()
		Extend(False)
	End Sub
	<CommandMethod("UnL")>
	Public Sub UnionLinks()
		Extend(True)
	End Sub
	Public Sub RoundSS(ByVal bUnion As Boolean)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity = Nothing
		Dim oEntB As Entity = Nothing
		Dim oLinkA As DMAcadExt.IUD_Link = Nothing
		Dim oLinkB As DMAcadExt.IUD_Link = Nothing
		Dim oTplnArc As DMAcadExt.TplnArc


		Dim oSegment As DMAcadExt.TplnSegment


		Dim oArc As Arc = New Arc()
		Dim oLine As Line = New Line()
		Dim colObjectIDs As ObjectIdCollection = New ObjectIdCollection()
		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		Dim tPointA As Point2d
		Dim tPointB As Point2d
		Dim tPointFocus As Point2d
		Dim dTolerance As Double = 0.001
		Dim iCounter As Integer = 0
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		oPromptOpt.AddAllowedClass(oArc.GetType(), True)
		oPromptOpt.AddAllowedClass(oLine.GetType(), True)


		Do
			ptRes = oEditor.GetEntity(oPromptOpt)

			colObjectIDs.Add(ptRes.ObjectId)
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				Return
			End Try
			oSegment = New DMAcadExt.TplnSegment(oEntA)
			If oEntA IsNot Nothing Then

				Select Case oEntA.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadArcName
						oArc = DirectCast(oEntA, Arc)
						oLinkA = New DMAcadExt.TplnArc(oArc)

					Case DMAcadExt.AcadConst.AcadLineName
						oLine = DirectCast(oEntA, Line)
						oLinkA = New DMAcadExt.TplnLine(oLine)

				End Select
			End If
			If iCounter = 0 Then
				tPointA = oLinkA.StartPoint
				tPointB = oLinkA.EndPoint
				iCounter += 1
			Else
				If tPointA.IsEqualTo(oLinkA.StartPoint) Then
					tPointFocus = tPointA
					oLinkA.SetPoint(tPointFocus, True)
				ElseIf tPointA.IsEqualTo(oLinkA.EndPoint) Then
					tPointFocus = tPointA
					oLinkA.SetPoint(tPointFocus, False)
				ElseIf tPointB.IsEqualTo(oLinkA.StartPoint) Then
					oLinkB.SetPoint(tPointFocus, True)
				ElseIf tPointB.IsEqualTo(oLinkA.EndPoint) Then
					oLinkB.SetPoint(tPointFocus, False)
				End If
				If oLinkA.IsArc Then
					oTplnArc = DirectCast(oLinkA, DMAcadExt.TplnArc)
					oArc.Center = DMAcadExt.TPlnPoint.Point2dTo3d(oTplnArc.Center)
					oArc.StartAngle = oTplnArc.StartAngle
					oArc.EndAngle = oTplnArc.EndAngle
					oArc.Radius = oTplnArc.Radius
				Else
					oLine.StartPoint = DMAcadExt.TPlnPoint.Point2dTo3d(oLinkA.StartPoint)
					oLine.StartPoint = DMAcadExt.TPlnPoint.Point2dTo3d(oLinkA.StartPoint)
				End If

			End If
		Loop

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public Sub Extend(ByVal bUnion As Boolean)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity = Nothing
		Dim oEntB As Entity = Nothing
		Dim oLinkA As DMAcadExt.IUD_Link = Nothing
		Dim oLinkB As DMAcadExt.IUD_Link = Nothing
		Dim bIsExt As Boolean
		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		'	Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
		Dim tTolerance As DMAcadExt.dmTolerance = New DMAcadExt.dmTolerance(TopoManager.TopoScheme.tsRing.mdAngleTolerance, TopoManager.TopoScheme.tsRing.mdDistanceTolerance, TopoManager.TopoScheme.tsRing.mdAreaTolerance)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				Return
			End Try

			Select Case oEntA.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oEntA, Arc)
					zzArcInfo(oArc)
					oLinkA = New DMAcadExt.TplnArc(oArc)
				Case DMAcadExt.AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oEntA, Line)
					oLinkA = New DMAcadExt.TplnLine(oLine)
				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oEntA, Polyline)
					If oPolyline.NumberOfVertices = 2 AndAlso oPolyline.GetSegmentType(0) = SegmentType.Arc Then
						oLinkA = New DMAcadExt.TplnArc(oPolyline)
					End If
			End Select
			oEditor.WriteMessage(vbCrLf)
			oPromptOpt.Message = "Select Next Entity"
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				oEditor.WriteMessage(vbCrLf)
				Try
					oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
				Catch oEx As Exception
					Return
				End Try
				Select Case oEntB.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadArcName
						Dim oArc As Arc = DirectCast(oEntB, Arc)
						zzArcInfo(oArc)
						oLinkB = New DMAcadExt.TplnArc(oArc)
					Case DMAcadExt.AcadConst.AcadLineName
						Dim oLine As Line = DirectCast(oEntB, Line)
						oLinkB = New DMAcadExt.TplnLine(oLinkA.EndPoint, oLine)
						If oLinkB.Length = 0.0 Then
							oLinkA.Reverse()
							oLinkB = New DMAcadExt.TplnLine(oLinkA.EndPoint, oLine)
						End If
					Case DMAcadExt.AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oEntB, Polyline)
						If oPolyline.NumberOfVertices = 2 AndAlso oPolyline.GetSegmentType(0) = SegmentType.Arc Then
							oLinkB = New DMAcadExt.TplnArc(oPolyline)
						End If
				End Select
				If oLinkA IsNot Nothing AndAlso oLinkB IsNot Nothing Then


					bIsExt = oLinkA.IsExtend(oLinkB, tTolerance, True)
					DMAcadExt.AcadDocument.WriteMessage("IsExtend: " & CStr(bIsExt))
					If bUnion AndAlso bIsExt Then
						Dim tPlineStart As Point2d = oLinkA.StartPoint
						Dim tPlineEnd As Point2d = oLinkB.EndPoint

						oLinkA.Extend(oLinkB)
						Dim oNewLink As Entity = oLinkA.GetNewEntity()
						DMAcadExt.AcadTransaction.AppendEntity(oNewLink, False)
						Dim oNewPline As Polyline = New Polyline(2)
						oNewPline.AddVertexAt(0, oLinkA.StartPoint, oLinkA.Bulge, 0.0, 0.0)
						oNewPline.AddVertexAt(1, oLinkA.EndPoint, 0.0, 0.0, 0.0)
						oNewPline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
						DMAcadExt.AcadTransaction.AppendEntity(oNewPline, False)

						Dim oNewPlineA As Polyline = New Polyline(2)
						oLinkA.Reverse()
						DMAcadExt.AcadDocument.WriteMessage("IsExtend: " & oLinkA.StartPoint.ToString() & "; " & oLinkA.EndPoint.ToString() & "; B=" & oLinkA.Bulge.ToString())
						oNewPlineA.AddVertexAt(0, oLinkA.StartPoint, oLinkA.Bulge, 0.0, 0.0)
						oNewPlineA.AddVertexAt(1, oLinkA.EndPoint, 0.0, 0.0, 0.0)
						oNewPlineA.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Yellow)
						DMAcadExt.AcadTransaction.AppendEntity(oNewPlineA, False)

					End If
				End If
			End If

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("HL")>
	Public Sub HideLayer()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As Entity = Nothing
		'   Dim bExists As Boolean



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		Do
			oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
				Catch oEx As Exception
					Return
				End Try
				DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
			Else
				Exit Do
			End If

		Loop

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("HE")>
	Public Sub HideEntity()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As Entity = Nothing
		'   Dim bExists As Boolean



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		' DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		Do
			oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
				Catch oEx As Exception
					Return
				End Try
				oEnt.Visible = False
				DMAcadExt.AcadDocument.UpdateScreen()
				' DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
			Else
				Exit Do
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadTransaction.Start()
		Loop

		'  DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("ColE")>
	Public Sub ColorEntity()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As Entity = Nothing
		'   Dim bExists As Boolean



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		' DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		Do
			oEditor.WriteMessage("SelectEntity ...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
				Catch oEx As Exception
					Return
				End Try
				oEnt.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
				DMAcadExt.AcadDocument.UpdateScreen()
				' DMAcadExt.AcadTransaction.SetLayersOffStatus(oEnt.LayerId, True)
			Else
				Exit Do
			End If
			'   DMAcadExt.AcadTransaction.Terminate()
			'   DMAcadExt.AcadTransaction.Start()
		Loop

		'  DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("UD_Hanit")>
	Public Sub UD_Hanit()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If

		'    DMAcadExt.AcadDocument.InitDebug()
		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		If TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState = System.Data.ConnectionState.Open Then
			TPlServerDB.ServerDB.AddInitialize()
		End If




		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		If mfHanitControl Is Nothing OrElse mfHanitControl.IsDisposed Then
			mfHanitControl = New TopoUI.frmHanitControl
		End If
		'MessageBox.Show(CStr(
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfHanitControl)

		'

	End Sub
	<CommandMethod("ODCC")>
	Public Sub OpenDataConnect()
		mfDataConnect = New frmDataConnect()

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfDataConnect)
	End Sub
	<CommandMethod("FormView")>
	Public Sub ActiveFormView()
		If mfHanitControl IsNot Nothing Then
			mfHanitControl.GeneralView(True)
		End If
		If mfPrjThemes IsNot Nothing Then
			mfPrjThemes.ActiveFormView(True)
		End If

	End Sub
	<CommandMethod("TestPrjF")>
	Public Sub TestPrjF()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If


		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()

		If mfProjectFiles Is Nothing OrElse mfHanitControl.IsDisposed Then
			mfProjectFiles = New TopoUI.frmProjectFiles
		End If
		'MessageBox.Show(CStr(
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfProjectFiles)
	End Sub

	<CommandMethod("UD_Rnd0")>
	Public Sub UD_Rnd0()


		UnidivNet.UD_App.RoundAll(0)




	End Sub

	<CommandMethod("UD_Rnd2")>
	Public Sub UD_Rnd2()


		UnidivNet.UD_App.RoundAll(2)




	End Sub

	<CommandMethod("UD_Rnd3")>
	Public Sub UD_Rnd3()


		UnidivNet.UD_App.RoundAll(3)




	End Sub

	<CommandMethod("UD_Rnd6")>
	Public Sub UD_Rnd6()


		UnidivNet.UD_App.RoundAll(6)




	End Sub
	<CommandMethod("AllStretch")>
	Public Sub Stretch()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		DMAcadExt.AcadTransaction.Stretch(10)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()






	End Sub
	<CommandMethod("UD_RndE")>
	Public Sub UD_RndEnt()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity = Nothing
		Dim iDigits As Integer = 3
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForWrite)
			Catch oEx As Exception
				Return
			End Try

			Select Case oEntA.GetRXClass().Name

				Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadLineName
					DMAcadExt.TplnSegment.Round(oEntA, True, iDigits)
			End Select
			oEditor.WriteMessage(vbCrLf)


		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

		' UnidivNet.UD_App.RoundAll(3)




	End Sub
	<CommandMethod("UD_HL")>
	Public Sub UD_HL()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If


		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		UnidivNet.UD_App.OpenDB()

		UnidivNet.UD_App.DrawHanitEntities()
		'	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")
		TPlServerDB.ServerDB.CurrentServerDB.Close()
		If TPlServerDB.ServerDB.CurrentProjectDB IsNot Nothing Then
			TPlServerDB.ServerDB.CurrentProjectDB.Close()

		End If
		'

	End Sub
	<CommandMethod("UD_Br")>
	Public Sub UD_Border()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If

		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		'  TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		' UnidivNet.UD_App.OpenDB()



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


		UnidivNet.UD_App.HanitBorders()
		'	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")




		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("ReMarks")>
	Public Sub Reset_MarkBlocks()




		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		DMAcadExt.MarkBlock.ResetAllRefs()





		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TopoEx")>
	Public Sub TopoExists()
		Dim bRes As Boolean = TopoManager.TopoCreator.TopologyExists("Stage_0")

		System.Windows.Forms.MessageBox.Show(bRes.ToString() & vbCrLf & "", "05_375")
	End Sub
	<CommandMethod("UD_Un")>
	Public Sub UD_Union()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If

		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		'  TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control
		' UnidivNet.UD_App.OpenDB()



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)


		UnidivNet.UD_App.ImportHanitBorders()
		'	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")




		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("UD_Points")>
	Public Sub UD_PointsOut()
		If DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Undefined Then
			DMAcadExt.DMApp.AppID = DMAcadExt.enApplications.Taba
		End If


		TopoManager.TPlanGraph.TplnProject.InitAppName()

		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		TopoManager.TPlanGraph.TplnProject.InitializePrjUD_OleDB()
		'	TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()


		'	frmEditDetails.DetailID_Control = TopoManager.TPlanGraph.TplnProject.Detail_ID_Control


		UnidivNet.UD_App.DrawPointsOut()
		'	System.Windows.Forms.MessageBox.Show(CStr(112), "04_210")
		TPlServerDB.ServerDB.CurrentServerDB.Close()
		If TPlServerDB.ServerDB.CurrentProjectDB IsNot Nothing Then
			TPlServerDB.ServerDB.CurrentProjectDB.Close()

		End If
		'

	End Sub
	<CommandMethod("IsLocked")>
	Public Sub IsLocked()
		Dim bLocked As Boolean = DMAcadExt.AcadDocument.IsLocked()

		DMAcadExt.AcadDocument.WriteMessage("Is Locked? " & CStr(bLocked) & "; " & Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode.ToString())
	End Sub
	<CommandMethod("Unlock")>
	Public Sub Unlock()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TplnClearMap")>
	Public Sub ClearAllMap()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		FDO_Manager.RemoveAllConnections()
		FDO_Manager.RemoveAllResources()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TplnDelR")>
	Public Sub TplnPlatfTest()
		Dim oGeoSpatPl As FDO.GeoSpatPl = New FDO.GeoSpatPl()

		FDO.GeoSpatPl.DeleteRecource()
	End Sub
	<CommandMethod("TplnPlt")>
	Public Sub TplnDeleteRes()
		Dim oGeoSpatPl As FDO.GeoSpatPl = New FDO.GeoSpatPl()

		oGeoSpatPl.Ex_DefiningVectorfeatureSource()
	End Sub
	Public Sub TplnCloseA()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadTransaction.Terminate()
	End Sub


	<CommandMethod("TplnTest")>
	Public Sub TplnLayerTest()
		'	Const sPath As String = "C:\Users\Boris\AppData\Local\Autodesk, Inc\AutoCAD\R22.0.49.0.0\ParcelsLine\ParcelsLine.shp"
		Const sPath As String = "R:\Gushim\Gushim-Vectorized\2792\p2792.shp"

		'	Dim sFeature As String = "shpParcelsLine"
		Dim sFeature As String = "shp_p2792"
		'Dim sLayerName As String = "ParcelsLine"
		Dim sLayerName As String = "p2792"

		'	FDO.FDO_Manager.NewTestB()
		'FDO.FDO_Manager.NewTestQ("ParcelsLine")
		'FDO.FDO_Manager.NewTestK()
		'	FDO.Util.EnumAllResources()
		'FDO.Util.AutoCreateLayersMy()
		FDO.Util.AutoCreateLayersMyA(sPath, sFeature, sLayerName)

		'FDO.FDO_Manager.zzCreateMapLayers(sPath)
	End Sub
	<CommandMethod("TplnLPrint")>
	Public Sub TplnLPrint()
		'	FDO.FDO_Manager.NewTestB()
		'FDO.FDO_Manager.NewTestQ("ParcelsLine")
		'FDO.Util.EnumAllResources()
		FDO.Util.TestAddLayer()



	End Sub


	<CommandMethod("ResetLinks")>
	Public Sub ResetLinks()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim sLayers As String = "pclp001,pclp004,pCellK"
		Dim sBlockName As String = "1603"
		Dim sBlockNameA As String = "Cellno"

		DMAcadExt.dmLineCleanup.CopyLinks(sLayers)
		DMAcadExt.dmLineCleanup.CopyBlockRefs(sBlockName)
		DMAcadExt.dmLineCleanup.CopyBlockRefs(sBlockNameA)




		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub




	<CommandMethod("TplnGazAddOD")>
	Public Sub TplnGazAddOD()
		prjPrograms.GazAddOD()
	End Sub
	Private Sub zzOpenApp(ByVal iApp As DMAcadExt.enApplications)
		DMAcadExt.DMApp.AppID = iApp

		TopoManager.TPlanGraph.TplnProject.InitAppName()
		TopoManager.TPlanGraph.TplnProject.InitializeDB_OleDB()
		If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then

			TopoManager.TPlanGraph.TplnProject.InitializeList()
			System.Windows.Forms.MessageBox.Show(iApp.ToString(), "07_018")
			If mfTopoActions Is Nothing OrElse mfTopoActions.IsDisposed Then
				Select Case iApp
					Case DMAcadExt.enApplications.Taba
						TopoDefs.InitTaba()
						mfTopoActions = New frmTopoActionsT
					Case DMAcadExt.enApplications.TopoMaster
						DMAcadExt.TopoDef.Format = 1
						TopoDefs.InitTopoMaster()
						mfTopoActions = New frmTopoActionsM
					Case DMAcadExt.enApplications.Unidiv
						DMAcadExt.TopoDef.Format = 1
						TopoDefs.InitUnidiv()
						System.Windows.Forms.MessageBox.Show("zzOpenApp", "")
						mfTopoActions = New frmTopoActionsUD
				End Select
			End If
			TopoManager.TPlanGraph.TplnProject.InitializeView()
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActions, True)
		End If
	End Sub

	<CommandMethod("TplnCleanup")>
	Public Sub TplnCleanup()
		If mfTopoActions IsNot Nothing Then
			mfTopoActions.Mark()
		End If
	End Sub

	Public Shared Sub TestLayer()
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.TestSetLayer("1602", "TestLayer")
		DMAcadExt.AcadTransaction.Terminate()
	End Sub
	<CommandMethod("TestMPgon")>
	Public Shared Sub TestMPgon()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		Dim oList As IList(Of Entity) = DMAcadExt.AcadTransaction.GetMPolygons("TplnParcelMPgon", OpenMode.ForRead)
		DMAcadExt.AcadDocument.WriteMessage("EntityCnt: " & CStr(oList.Count))



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestBlock")>
	Public Shared Sub TestBlock()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		'DMAcadExt.AcadTransaction.OpenNewBlockTest("*U")
		DMAcadExt.AcadTransaction.OpenNewBlock("Table")


		Dim tGeoPoint As Point3d = New Point3d(0.0, 0.0, 0.0)
		Dim oPoint As DBPoint = New DBPoint(tGeoPoint)
		'	DMAcadExt.AcadTransaction.AddToNewBlock(oPoint)
		'	DMAcadExt.AcadTransaction.InsertNewBlock(False)
		'	DMAcadExt.AcadTransaction.GetAllBlocks()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestB")>
	Public Shared Sub TestUDBlock()
		Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit_1.3"
		Const msHeaderBlockName As String = "UD_ComStampHeader"
		Const msFooterBlockName As String = "UD_ComStampFooter"
		Const msRowBlockName As String = "UD_ComStampRow"

		Const msTazarMapBlockLayer As String = "C1680"
		Const sX As String = "„—…‡ ˆ‰˜™/‰‹ ˜” '"
		Const sY As String = "מספר תכנית/תשרית חלוקה"  'מספר
		Const sZ As String = "סוג תכנית"
		Const sZ1 As String = "בתוקף מיום"
		Const dX As Double = -92.63091049
		Const dY As Double = 6.37174961
		Dim oBlockRef As BlockReference
		Dim oaHeaderAttribDefs() As AttributeDefinition = Nothing
		Dim oaRowAttribDefs() As AttributeDefinition = Nothing
		Dim oaFooterAttribDefs() As AttributeDefinition = Nothing

		Dim dicAttributes As Dictionary(Of String, String) = Nothing
		'Dim oTable As DataTable
		'222679.606 754374.644

		Dim oHebText1 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sY, True)
		Dim oHebText2 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sZ, True)

		Dim oHebText3 As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sZ1, True)

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		'	Dim tInsertPoint As Point3d = New Point3d(1, 1, 0)
		DMAcadExt.AcadTransaction.SetCurrentLayer(msTazarMapBlockLayer, 1, True, True)
		Dim tHeaderInsertPoint As Point3d = New Point3d(222600.0, 754500.0, 0)
		Dim tRowInsertPoint As Point3d
		Dim tFirstRowLeft As Point3d
		Dim tFirstRowRight As Point3d

		'DMAcadExt.AcadTransaction.OpenNewBlockTest("*U")
		Dim tHeaderBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msHeaderBlockName, oaHeaderAttribDefs)
		Dim tFooterBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msFooterBlockName, oaFooterAttribDefs)
		Dim tRowBlockObjId As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msRowBlockName, oaRowAttribDefs)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oaAttribDefs", oaAttribDefs Is Nothing)
		Dim oRow As DataCellCollection
		Dim oCell As DataCell
		DMAcadExt.AcadTransaction.OpenNewBlock("Table")

		'	oBlockRef = New BlockReference(tInsertPoint, tHeaderBlockObjId)
		'DMAcadExt.AcadTransaction.AddToNewBlock(oBlockRef)

		dicAttributes = New Dictionary(Of String, String)
		dicAttributes.Add("ComName", "!ComName")
		dicAttributes.Add("MeetingDate", "!MeetingDate")
		dicAttributes.Add("MeetingNum", "!MeetingNum")


		DMAcadExt.AcadTransaction.InsertBlockRef(tHeaderBlockObjId, tHeaderInsertPoint, oaHeaderAttribDefs, dicAttributes,,,,,, True)

		dicAttributes = New Dictionary(Of String, String)
		dicAttributes.Add("PlanNum", oHebText1.GetDOSDest())
		dicAttributes.Add("PlanType", oHebText2.GetDOSDest())
		dicAttributes.Add("InForceDate", oHebText3.GetDOSDest())


		tFirstRowLeft = New Point3d(tHeaderInsertPoint.X - 100.0 + 7.369, tHeaderInsertPoint.Y + 26.0, 0)
		tFirstRowRight = New Point3d(tHeaderInsertPoint.X, tHeaderInsertPoint.Y + 26.0, 0)
		tRowInsertPoint = New Point3d(tHeaderInsertPoint.X + 100.0 - 7.369, tFirstRowRight.Y - dY, 0.0)

		Dim oLine As Line = New Line(tFirstRowLeft, tFirstRowRight)
		DMAcadExt.AcadTransaction.AddToNewBlock(oLine)
		DMAcadExt.AcadTransaction.InsertBlockRef(tRowBlockObjId, tRowInsertPoint, oaRowAttribDefs, dicAttributes,,,,,, True)


		''''''DMAcadExt.AcadTransaction.InsertBlockRef(tHeaderBlockObjId, tHeaderInsertPoint, oaHeaderAttribDefs, dicAttributes,,,,,, True)

		dicAttributes = New Dictionary(Of String, String)
		dicAttributes.Add("PlanNum", "!אבג")
		dicAttributes.Add("PlanType", "!דה")
		dicAttributes.Add("InForceDate", "!כלמ")

		tRowInsertPoint = New Point3d(tHeaderInsertPoint.X + 100.0 - 7.369, tFirstRowRight.Y - 2.0 * dY, 0.0)



		DMAcadExt.AcadTransaction.InsertBlockRef(tRowBlockObjId, tRowInsertPoint, oaRowAttribDefs, dicAttributes,,,,,, True)

		'	DMAcadExt.AcadTransaction.AddToNewBlock(oBlockRef)


		oBlockRef = New BlockReference(tHeaderInsertPoint, tFooterBlockObjId)
		DMAcadExt.AcadTransaction.AddToNewBlock(oBlockRef)

		DMAcadExt.AcadTransaction.InsertNewBlock(False)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestDoc")>
	Public Shared Sub TestDocLock()
		'DMAcadExt.AcadDocument.SetActiveDoc(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		MessageBox.Show("TestDoc", "01_184a")

		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()




		ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(oPromptOpt)

		Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ptRes.Status.ToString())
		Dim oLine As Line
		oLine = New Line(New Point3d(194100, 654000, 0), New Point3d(194200, 6541000, 0))
		DMAcadExt.AcadTransaction.AppendEntity(oLine)
	End Sub
	<CommandMethod("TestUnlock")>
	Public Shared Sub TestUnlock()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		MessageBox.Show("TestUnlock", "01_184b")
	End Sub
	<CommandMethod("TestL")>
	Public Sub GetLinksSet()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()

		Dim colSelectionLinks As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		'Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		'Dim ptResA As Autodesk.AutoCAD.EditorInput.PromptSelectionResult

		'	Dim colLines As ObjectIdCollection = New ObjectIdCollection
		Dim iTotal As Integer = 0
		Dim iFound As Integer = 0
		colSelectionLinks.Clear()

		Try
			oPromptOpt.AllowDuplicates = False
			oPromptOpt.SingleOnly = False
			oPromptOpt.MessageForAdding = "Add ***"
			oPromptOpt.MessageForRemoval = "Remove ***"

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try

		'	Dim oReslSet As Autodesk.AutoCAD.EditorInput.SelectionSet '= New Autodesk.AutoCAD.EditorInput.SelectionSetDelayMarshalled()
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet

		Dim taObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId

		'Dim oSelEntity As Autodesk.AutoCAD.DatabaseServices.Entity

		'Dim bAcadPoint As Boolean

		ptRes = oEditor.GetSelection(oPromptOpt)

		DMAcadExt.AcadDocument.WriteSingleMessage(ptRes.Status.ToString & "; Cnt=" & ptRes.Value().Count.ToString & "; ")
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptRes.Value()
			' DMAcadExt.AcadDocument.WriteSingleMessage(oSelSet.ToString & "; Cnt=" & oSelSet.GetType().ToString & "; ")
			'	zzTestSet(oSelSet, True)
			taObjIDs = oSelSet.GetObjectIds()
			colSelectionLinks.Add(taObjIDs(0))

			iFound = colSelectionLinks.Count - iTotal
			iTotal = colSelectionLinks.Count
			DMAcadExt.AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
			'    ptResA = oEditor.SelectImplied()
			' oEditor.SetImpliedSelection(oReslSet)
			'    DMAcadExt.AcadDocument.WriteSingleMessage("++ " & ptRes.Status.ToString & "; Cnt=" & ptRes.Value().Count.ToString & "; ")
		ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			colSelectionLinks.Clear()
			'Exit Do
		Else
			DMAcadExt.AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
			'Exit Do
		End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TestSS")>
	Public Sub TestSS()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim iIndex As Integer
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		'  oPromptOpt.
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception

				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try
			ReDim Preserve taAcObjIDs(iIndex)
			taAcObjIDs(iIndex) = ptRes.ObjectId
			oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

			oEditor.SetImpliedSelection(oSelSet)

		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	<CommandMethod("TestOded", CommandFlags.Session)> Public Sub TestOdedMethod()

		Const sTopoName As String = "Links"
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Link")
		'oPromptOpt.SetMessageAndKeywords("Finish? [Y]:", "Y")
		oPromptOpt.AllowNone = True
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'	Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim colAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
		Dim colSumAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()

		Dim iIndex As Integer = -1
		'DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		'DMAcadExt.AcadTransaction.Start()
		'DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		'  oPromptOpt.
		Dim oLinkTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim colFullEdges As Autodesk.Gis.Map.Topology.FullEdgeCollection = oLinkTopology.GetFullEdges()
		Dim dicFullEdges As Dictionary(Of ObjectId, Integer) = New Dictionary(Of ObjectId, Integer)()
		'Dim iFullEdgeID As Integer
		'Dim iFullEdgeIDAlt As Integer
		'Dim bDirection As Boolean
		'Dim iFullEdgeStartID As Integer
		'Dim iFullEdgeRightID As Integer
		Dim oFullEdge As Autodesk.Gis.Map.Topology.FullEdge
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		Dim taNullAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()

		For Each oFullEdge In colFullEdges
			dicFullEdges.Add(oFullEdge.Entity, oFullEdge.ID)
		Next
		Do


			ptRes = oEditor.GetEntity(oPromptOpt)
			'	DMCommon.Debug.MsgBox("13_045H", ptRes.Status.ToString(), ptRes.StringResult)
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then

				'If dicFullEdges.TryGetValue(ptRes.ObjectId, iFullEdgeStartID) Then
				'	iFullEdgeID = iFullEdgeStartID
				'	oFullEdge = oLinkTopology.GetFullEdge(iFullEdgeID)
				'	DMCommon.Debug.MsgBox("13_044d", oFullEdge.ID, oFullEdge.GetNextEdge(False, False).ID, oFullEdge.GetNextEdge(False, True).ID, oFullEdge.GetNextEdge(True, False).ID, oFullEdge.GetNextEdge(True, True).ID)
				'	For iDirection As Integer = 0 To -1
				'		bDirection = Convert.ToBoolean(iDirection)

				'		iFullEdgeID = iFullEdgeStartID
				'		Do
				'			oFullEdge = oLinkTopology.GetFullEdge(iFullEdgeID)
				'			If iIndex >= 0 Then
				'				ReDim Preserve taAcObjIDs(iIndex)
				'				taAcObjIDs(iIndex) = oFullEdge.Entity
				'			End If

				'			iIndex += 1
				'			iFullEdgeID = oFullEdge.GetNextEdge(bDirection, False).ID
				'			iFullEdgeIDAlt = oFullEdge.GetNextEdge(bDirection, True).ID
				'			DMCommon.Debug.MsgBox("13_044", bDirection, iFullEdgeID, iFullEdgeIDAlt)
				'		Loop While iFullEdgeID <> 0 AndAlso iFullEdgeID = iFullEdgeIDAlt

				'	Next

				Dim oTopoNetWork As TopoManager.TopoNetWork = New TopoNetWork(sTopoName)
				oTopoNetWork.Load(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				colAcObjIDs = oTopoNetWork.GetRoute(ptRes.ObjectId)
				For Each tAcObjID As ObjectId In colAcObjIDs
					If Not colSumAcObjIDs.Contains(tAcObjID) Then
						colSumAcObjIDs.Add(tAcObjID)
						DMAcadExt.AcadTransaction.Highlight(tAcObjID)
					End If
				Next



				' oSelSet.Count,
				'	DMCommon.Debug.MsgBox("13_045I", DMAcadExt.AcadDocument.IsLocked, colAcObjIDs.Count, colSumAcObjIDs.Count, taAcObjIDs.GetUpperBound(0), ptRes.Status.ToString(), ptRes.StringResult)
			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
				For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colSumAcObjIDs
					DMAcadExt.AcadTransaction.Unhighlight(tAcObjID)
				Next
				If False Then
					oEditor.SetImpliedSelection(taNullAcObjIDs)
					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
				End If

				Exit Do
			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.None Then
				ReDim taAcObjIDs(colSumAcObjIDs.Count - 1)
				colSumAcObjIDs.CopyTo(taAcObjIDs, 0)
				'oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)


				oEditor.SetImpliedSelection(taAcObjIDs)

				Exit Do
			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Keyword Then

				'DMAcadExt.AcadTransaction.Terminate()
				'	DMAcadExt.AcadDocument.Unlock()
				Exit Do
			End If

		Loop


		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()




	End Sub
	Private Shared Function CallB(tPoint As Point3d, ByRef tTransform As Matrix3d) As Autodesk.AutoCAD.EditorInput.SamplerStatus
		Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("CallBack")
		tTransform = Autodesk.AutoCAD.Geometry.Matrix3d.Scaling(2.0, tPoint)
	End Function
	<CommandMethod("TestDrag")>
	Public Shared Sub TestDrag()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptSelOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptSelRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptDragOptions
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim oDragCallback As DragCallback = New DragCallback(AddressOf CallB)
		Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		ptSelRes = oEditor.GetSelection(oPromptSelOpt)

		If ptSelRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptSelRes.Value
			oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptDragOptions(oSelSet, "DRAG", oDragCallback)
			ptPointRes = oEditor.Drag(oPromptOpt)
			oEditor.WriteMessage(ptPointRes.Status.ToString() & ":" & ptPointRes.StringResult)
		End If

	End Sub
	<CommandMethod("TestTr")>
	Public Shared Sub TestTr()
		Dim sTopoName As String = Nothing
		Try
			sTopoName = DMAcadExt.AcadUtil.GetName("Topology Name ...")
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
			sTopoName = Nothing
		End Try
		If sTopoName IsNot Nothing Then
			TopoCreator.TestTrace(sTopoName, 0)
		End If

	End Sub
	<CommandMethod("TestDicL")>
	Public Shared Sub TestDictionaryList()

		Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary
		Dim iNamedMode As OpenMode
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()


		dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(OpenMode.ForRead)
		'   MessageBox.Show(CStr(dicNamed.Count) & ":" & dsDictionaryName, "01_492")
		If dicNamed IsNot Nothing Then
			Try
				For Each oEntry As DBDictionaryEntry In dicNamed

					DMAcadExt.AcadDocument.WriteMessage("#199" & oEntry.m_key & "//" & oEntry.m_value.ToString())
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "ProjectData - TestDictionaryList")
			End Try

		Else
			MessageBox.Show("NamedDictionary is nothing !!!" & iNamedMode.ToString())
		End If

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestSSet")> Public Shared Sub TestSSet()
		Dim moEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oEnt As DBObject = Nothing
		Dim oDxfCode As System.Object = DxfCode.Color

		Dim oaValues() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Operator), "<or") _
												 , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5F))) _
												 , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5E))) _
												 , New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5D))) _
												 , New TypedValue(Convert.ToInt32(DxfCode.Operator), "or>")}
		Dim oaValues1() As TypedValue = {New TypedValue(DxfCode.LayerName, "pclp011"), New TypedValue(DxfCode.Color, 254)}
		'   Dim oaValues2() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Handle), New Handle(Convert.ToInt64(&H4B5F)).ToString())}
		Dim oaValues2() As TypedValue = {New TypedValue(Convert.ToInt32(DxfCode.Handle), "19295")}


		Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter
		Dim oFilter1 As Autodesk.AutoCAD.EditorInput.SelectionFilter
		Dim oFilter2 As Autodesk.AutoCAD.EditorInput.SelectionFilter

		'     (or(And objType="*POLYLINE,CIRCLE"                 ))
		moEditor.WriteMessage(oDxfCode.GetType().ToString() & vbCrLf)
		moEditor.WriteMessage(New Handle(Convert.ToInt64(&H4B5F)).ToString() & vbCrLf)


		oPromptOpt.MessageForRemoval = "Remove ..."

		oPromptOpt.MessageForAdding = "Add ..."
		'  oPromptOpt.SingleOnly = False
		'  oPromptOpt.SinglePickInSpace = True
		oPromptOpt.AllowDuplicates = False

		'oaValues(0) =
		'oaValues(1) =
		'oaValues(2) =
		oFilter = New SelectionFilter(oaValues) '
		oFilter1 = New SelectionFilter(oaValues1)
		oFilter2 = New SelectionFilter(oaValues2)



		ptRes = moEditor.GetSelection(oPromptOpt, oFilter1)
		'ptRes = moEditor.GetSelection(oPromptOpt, oFilter1)



		'	oEditor.WriteMessage(ptRes.StringResult)
		'ptRes.Value
		'	Dim tAcObjID As ObjectId = ptRes.ObjectId
		If ptRes.Value IsNot Nothing Then
			'  moEditor.WriteMessage(CStr(ptRes.Value.Count))
		End If
		' moEditor.WriteMessage(CStr(ptRes.Value.Count), ptRes.Status.ToString())
		moEditor.WriteMessage(ptRes.Status.ToString() & " !!!!!!!!!!!!!!!-!!!!!!!!!!!!!!-!!!!!!!!!-!!!!!!!!!!-!!!!!!!!!!!!!-!!!!!!!!!!-!!!!!")
	End Sub
	<CommandMethod("Test11")>
	Public Sub SelectionTest()
		Dim ed As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim pso As PromptSelectionOptions = New PromptSelectionOptions()
		pso.SingleOnly = True
		pso.SinglePickInSpace = True
		Dim psr As PromptSelectionResult
		Dim ids As ObjectIdCollection = New ObjectIdCollection()
		Do
			psr = ed.GetSelection(pso)
			If (psr.Status <> PromptStatus.OK) Then
				Exit Do
			End If
			ids.Add(psr.Value.Item(0).ObjectId)
			ed.WriteMessage("\n{0} selected objects", ids.Count)
		Loop
	End Sub
	<CommandMethod("ZEH")>
	Public Sub ZoomEntityByHandle()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		DMAcadExt.AcadTransaction.OpenHandleDictionary()
		Dim oPromptResult As PromptResult
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptStringOptions = New PromptStringOptions("Handle: ")
		Dim sHandle As String
		'Dim tHandle As Handle
		oPromptResult = oEditor.GetString(oPromptOpt)
		sHandle = oPromptResult.StringResult

		Dim tAcObjID As ObjectId = DMAcadExt.AcadTransaction.GetObjectID(sHandle)
		If Not tAcObjID.IsNull Then
			DMAcadExt.AcadDocument.Zoom(tAcObjID, 2.0)
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TestSSetA")> Public Shared Sub TestSSetA()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions '= New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oEnt As DBObject = Nothing
		'  Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taAcObjIDs() As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
		Dim colSelected As ObjectIdCollection = New ObjectIdCollection()
		Dim iIndex As Integer = -1
		Dim tAcObjID As ObjectId
		Dim oaValues(2) As TypedValue
		Dim oFilter As Autodesk.AutoCAD.EditorInput.SelectionFilter
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		oPromptOpt = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		'    oPromptOpt.SingleOnly = True


		oaValues(0) = New TypedValue(DxfCode.Handle, New Handle(&H4B5F))
		oaValues(1) = New TypedValue(DxfCode.Handle, New Handle(&H4B5E))
		oaValues(2) = New TypedValue(DxfCode.Handle, New Handle(&H4B5D))
		oFilter = New SelectionFilter(oaValues)
		oPromptOpt.SingleOnly = True
		oPromptOpt.SinglePickInSpace = False

		'  oPromptOpt.AllowSubSelections = True

		Do
			iIndex += 1
			oEditor.WriteMessage("8ii= " & iIndex.ToString() & "; " & oPromptOpt.SingleOnly.ToString & vbCrLf)


			ptRes = oEditor.GetSelection(oPromptOpt, oFilter)

			If ptRes.Status <> PromptStatus.OK Then
				Exit Do
			Else
				oEditor.WriteMessage("AS: " & iIndex.ToString() & "; " & CStr(ptRes.Value.Count) & "; " & ptRes.Status.ToString() & vbCrLf)
			End If





			For iSelIndex As Integer = 0 To ptRes.Value.Count - 1
				tAcObjID = ptRes.Value.Item(iSelIndex).ObjectId
				If Not colSelected.Contains(tAcObjID) Then
					colSelected.Add(tAcObjID)

					DMAcadExt.AcadTransaction.Highlight(tAcObjID)


				End If
			Next
			'  oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjIDs)

			'  oEditor.SetImpliedSelection(oSelSet)
			'  oEditor.SetImpliedSelection(taAcObjIDs)

			' oPromptOpt.ForceSubSelections = True
			oEditor.WriteMessage("TT: " & iIndex.ToString() & "; " & CStr(colSelected.Count) & vbCrLf)
		Loop
		DMCommon.Debug.MsgBox("09_763a", colSelected.Count, ptRes.Status)
		If False Then

			ReDim Preserve taAcObjIDs(colSelected.Count - 1)
			colSelected.CopyTo(taAcObjIDs, 0)

			DMCommon.Debug.MsgBox("09_764a", taAcObjIDs.GetUpperBound(0))
		End If

		'If taAcObjIDs.GetUpperBound(0) >= 0 Then
		'   oEditor.SetImpliedSelection(taAcObjIDs)
		'End If
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	<CommandMethod("TestSub")> Public Shared Sub TestSubentity()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptNestedEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptNestedEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptNestedEntityResult
		Dim oEnt As DBObject = Nothing
		ptRes = oEditor.GetNestedEntity(oPromptOpt)
		oEditor.WriteMessage(ptRes.StringResult)
		Dim tAcObjID As ObjectId = ptRes.ObjectId
	End Sub
	<CommandMethod("VPort")> Public Shared Sub VPort()
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadDocument.GetMinPoint()
		DMAcadExt.AcadTransaction.Terminate()
	End Sub
	<CommandMethod("TestSel")>
	Public Shared Sub TestSel()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point Or 1")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		Dim tVector As Vector2d
		oPromptOpt.AllowNone = True
		ptRes = oEditor.GetPoint(oPromptOpt)
		tVector = New Vector2d(ptRes.Value.X, ptRes.Value.Y)

		'DMCommon.Debug.MsgBox("Status!", ptRes.Status, ptRes.Value, tVector.Length, tVector.Angle, 180 * tVector.Angle / Math.PI, ptRes.StringResult)
	End Sub
	<CommandMethod("TestKW")>
	Public Shared Sub TestKeyWords()
		' Autodesk.AutoCAD.EditorInput
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		'	Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point Or [Arc/Halfwidth/Length/Undo/Width]", "[Arc/Halfwidth/Length/Undo/Width]")
		Dim oPromptPointOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point ")
		Dim oPromptKeywordOpt As Autodesk.AutoCAD.EditorInput.PromptKeywordOptions = New Autodesk.AutoCAD.EditorInput.PromptKeywordOptions("Select Option ")
		'Dim saKeyWordList() As String = {"2_balcony", "3_warehouse", "4_unit", "Next"}
		'Dim saKeyWordList() As String = {"2balcony", "3warehouse", "4unit", "Next"}
		'Dim saKeyWordList() As String = {"2-balcony", "3-warehouse", "4-unit", "Next"}
		'!!!	Dim saKeyWordList() As String = {"2-מרפסת", "3-מחסן", "4-יחמשנה", "Next"}
		Dim saKeyWordList() As String = {"2-מרפסת", "3-מחסן", "4-צמ'פרטית", "5-צמ'משותפת", "Next"}




		Dim iProp As Integer = 11
		'	oPromptOpt.Message = vbLf & "Enter an option "
		oPromptPointOpt.Keywords.Add("2_מרפסת")
		oPromptPointOpt.Keywords.Add("3_מחסן")
		oPromptPointOpt.Keywords.Add("4_יח_משנה")
		oPromptPointOpt.Keywords.Add("Next")
		oPromptPointOpt.AllowNone = False
		If False Then
			oPromptKeywordOpt.Keywords.Add("2_מרפסת")
			oPromptKeywordOpt.Keywords.Add("3_מחסן")
			oPromptKeywordOpt.Keywords.Add("4_משנה")
			oPromptKeywordOpt.Keywords.Add("Next")
		End If
		oPromptKeywordOpt.Message = vbLf & "Enter an option "
		For iIndex As Integer = 0 To saKeyWordList.GetUpperBound(0)
			oPromptKeywordOpt.Keywords.Add(saKeyWordList(iIndex))
		Next
		oPromptKeywordOpt.AllowNone = False

		Dim oPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oKeywordRes As PromptResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()

		Dim oKW1 As Keyword = oPromptKeywordOpt.Keywords.Item(0)
		Application.ShowAlertDialog(oKW1.DisplayName & vbCrLf & oKW1.GlobalName & vbCrLf & oKW1.LocalName)
		Dim i As Integer = 0
		Do
			If False Then
				oPointRes = oEditor.GetPoint(oPromptPointOpt)
				Application.ShowAlertDialog("Entered Point: " & oPointRes.Status.ToString() & vbCrLf & oPointRes.Value.ToString() & vbCrLf & oPointRes.StringResult & "")
				If oPointRes.Status = PromptStatus.Cancel Then
					Exit Do
				End If
			End If

			oKeywordRes = oEditor.GetKeywords(oPromptKeywordOpt)
			Application.ShowAlertDialog("Entered keyword: " & oKeywordRes.Status.ToString() & vbCrLf & oKeywordRes.StringResult & "")
			If oKeywordRes.StringResult = "Next" Then
				Exit Do
			End If
			If oKeywordRes.Status = PromptStatus.Cancel Then
				Exit Do
			End If
			i += 1
			If i > 10 Then
				Exit Do
			End If
		Loop




		'DMCommon.Debug.MsgBox("Status!", ptRes.Status, ptRes.Value, tVector.Length, tVector.Angle, 180 * tVector.Angle / Math.PI, ptRes.StringResult)
	End Sub
	<CommandMethod("GetKeyw")>
	Public Sub GetKeywordFromUser()
		Dim acDoc As Document = Application.DocumentManager.MdiActiveDocument
		Dim pKeyOpts As PromptKeywordOptions = New PromptKeywordOptions("")
		pKeyOpts.Message = vbLf & "Enter an option "
		pKeyOpts.Keywords.Add("Line")
		pKeyOpts.Keywords.Add("Circle")
		pKeyOpts.Keywords.Add("Arc")
		pKeyOpts.AllowNone = False
		Dim pKeyRes As PromptResult = acDoc.Editor.GetKeywords(pKeyOpts)
		Application.ShowAlertDialog("Entered keyword: " &
										pKeyRes.StringResult)
	End Sub

	<CommandMethod("TestDB")>
	Public Shared Sub TestDB()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		DMAcadExt.AcadTransaction.OpenNewBlockDB()
		Dim tGeoPoint As Point3d = New Point3d(0.0, 0.0, 0.0)
		Dim oPoint As DBPoint = New DBPoint(tGeoPoint)

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestPrj")>
	Public Shared Sub TestPrj()
		Dim oDB As Database = Nothing
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
		If oDB Is Nothing Then
			oProject = oMapApplication.ActiveProject
		Else

			System.Windows.Forms.MessageBox.Show(CStr(oDB.NumberOfSaves), "05_411")
			oProject = oMapApplication.GetProjectForDB(oDB)
			System.Windows.Forms.MessageBox.Show(CStr(oProject.MapActive), "05_412")
		End If
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oProject.Topologies

		System.Windows.Forms.MessageBox.Show(CStr(oTopos Is Nothing), "05_413")
	End Sub
	<CommandMethod("TplnBuild")>
	Public Sub TplnBuild()
		If mfTopoActions IsNot Nothing Then
			mfTopoActions.CreateTopo()
		End If
	End Sub
	<CommandMethod("TplnKill")>
	Public Sub TplnKill()

		If False Then
			If mfTopoActions IsNot Nothing Then
				mfTopoActions.DeleteTopo()
			End If
		End If

	End Sub
	<CommandMethod("cnnn")>
	Public Sub TestDmConnected()
		DMCommon.Debug.ExcelLog = New DMCommon.ExcelAppExt()
		If DMCommon.Debug.Debug Then
			DMCommon.Debug.ExcelLog.Open()
		End If
		TopoManager.dmConnected.LoadModelSpace()
	End Sub

	<CommandMethod("ExecUnion")>
	Public Sub ExecUnion()
		TopoOverlay.TopoSourceName = "TopoA"
		TopoOverlay.TopoOverlayName = "TopoB"
		TopoOverlay.Exec()
	End Sub
	<CommandMethod("UnionTplnM")>
	Public Sub UnionTplnM()
		TopoOverlay.TopoSourceName = TopoManager.TPlanGraph.TplnLot.TopoName(DMAcadExt.enTopoPurpose.Proposed)
		TopoOverlay.TopoOverlayName = TopoManager.TPlanGraph.TplnParcel.TopoName
		TopoOverlay.TopoResultName = TopoManager.TPlanGraph.TplnLot.UnionTopoNameB(TopoManager.TPlanGraph.enTopoPurpose.Proposed)
		TopoOverlay.ExecTplan()
	End Sub
	<CommandMethod("UnionTplnK")>
	Public Sub UnionTplnK()
		TopoOverlay.TopoSourceName = TopoManager.TPlanGraph.TplnLot.TopoName(DMAcadExt.enTopoPurpose.Approved)
		TopoOverlay.TopoOverlayName = TopoManager.TPlanGraph.TplnParcel.TopoName
		TopoOverlay.TopoResultName = TopoManager.TPlanGraph.TplnLot.UnionTopoNameB(TopoManager.TPlanGraph.enTopoPurpose.Approved)
		TopoOverlay.ExecTplan()
	End Sub
	<CommandMethod("H2Pl")>
	Public Sub HatchToPolyline()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		Dim oHatch As Hatch
		Dim oHatchLoop As HatchLoop
		'Dim iHatchLoopType As HatchLoopTypes
		'Dim colCurves As Autodesk.AutoCAD.Geometry.Curve2dCollection
		Dim oPolyline As Polyline

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
				If oEnt.GetRXClass.Name = DMAcadExt.AcadConst.AcadHatchName Then
					oHatch = DirectCast(oEnt, Hatch)
					For iIndex As Integer = 0 To oHatch.NumberOfLoops - 1
						oHatchLoop = oHatch.GetLoopAt(iIndex)
						oPolyline = GeoUtilites.HatchLoopToPolyline(oHatchLoop)
						If oPolyline IsNot Nothing Then
							oPolyline.Layer = oHatch.Layer
							oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
							DMAcadExt.AcadTransaction.AppendEntity(oPolyline, False)
						Else
							MessageBox.Show("Polyline IsNot Nothing ")
						End If
					Next
				End If
			Catch oEx As Exception
				Return
			End Try
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("AH2Pl")>
	Public Sub AllHatchToPolyline()
		'	Dim dHatchToPolyline As DMAcadExt.AcadTransaction.Procedure = New DMAcadExt.AcadTransaction.Procedure(AddressOf GeoUtilites.HatchToPolyline)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		''''''''''DMAcadExt.AcadTransaction.EnumModelSpaceObjectsBuffer(dHatchToPolyline, OpenMode.ForRead)

		Dim colDBObjects As Autodesk.AutoCAD.DatabaseServices.DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
		For Each oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject In colDBObjects
			'	DMAcadExt.AcadDocument.WriteMessage("Entity: " & oDBObject.GetRXClass().Name & ":" & oDBObject.Handle.ToString())
			GeoUtilites.HatchToPolyline(oDBObject)


		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Dst")>
	Public Sub Dist()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select 1st Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEntA As Entity = Nothing
		Dim oEntB As Entity = Nothing
		Dim oCurve As Curve = Nothing
		Dim oDBPoint As DBPoint
		Dim oBlockRef As BlockReference
		Dim tPointA As Point3d
		Dim tPointB As Point3d
		Dim iOption As Integer = 0
		Dim dDist As Double
		Dim bContinue As Boolean = True
		Dim colPoints As Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEntA = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
				DMAcadExt.AcadDocument.WriteMessage("A ID: " & oEntA.ObjectId.ToString() & vbCrLf)
			Catch oEx As Exception
				bContinue = False
			End Try
			If bContinue Then

				oPromptOpt.Message = "Select 2nd Entity"
				ptRes = oEditor.GetEntity(oPromptOpt)
				If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
					Try
						oEntB = DMAcadExt.AcadTransaction.GetEntity(ptRes.ObjectId, OpenMode.ForRead)
						DMAcadExt.AcadDocument.WriteMessage("B ID: " & oEntB.ObjectId.ToString() & vbCrLf)
					Catch oEx As Exception
						bContinue = False
					End Try
				End If
				If bContinue Then


					Select Case oEntA.GetRXClass.Name
						Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName
							oCurve = DirectCast(oEntA, Curve)
							iOption = 1
						Case DMAcadExt.AcadConst.AcadPointName
							oDBPoint = DirectCast(oEntA, DBPoint)
							tPointA = oDBPoint.Position
							iOption = 2
						Case DMAcadExt.AcadConst.AcadBlockRefName
							oBlockRef = DirectCast(oEntA, BlockReference)
							tPointA = oBlockRef.Position
							iOption = 2
						Case Else
							DMAcadExt.AcadDocument.WriteMessage("EntityA: " & oEntA.GetRXClass().Name & ":" & oEntA.GetType().ToString())
							bContinue = False
					End Select
					If bContinue And iOption <> 0 Then


						Select Case oEntB.GetRXClass.Name
							Case DMAcadExt.AcadConst.AcadArcName, DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName
								oCurve = DirectCast(oEntB, Curve)
								If iOption = 1 Then
									bContinue = False
								Else
									iOption = 3
								End If
							Case DMAcadExt.AcadConst.AcadPointName
								oDBPoint = DirectCast(oEntB, DBPoint)
								If iOption = 1 Then
									tPointA = oDBPoint.Position
									iOption = 3
								ElseIf iOption = 2 Then
									tPointB = oDBPoint.Position
									iOption = 4
								End If

							Case DMAcadExt.AcadConst.AcadBlockRefName
								oBlockRef = DirectCast(oEntB, BlockReference)
								If iOption = 1 Then
									tPointA = oBlockRef.Position
									iOption = 3
								ElseIf iOption = 2 Then
									tPointB = oBlockRef.Position
									iOption = 4
								End If

							Case Else
								DMAcadExt.AcadDocument.WriteMessage("EntityB: " & oEntA.GetRXClass().Name & ":" & oEntA.GetType().ToString() & vbCrLf)
								bContinue = False
						End Select
						If bContinue Then
							DMAcadExt.AcadDocument.WriteMessage("Option: " & iOption.ToString() & vbCrLf)
							Select Case iOption
								Case 3
									Dim tClosestPointOnCurve As Point3d
									Try
										tClosestPointOnCurve = oCurve.GetClosestPointTo(tPointA, False)
										dDist = tPointA.DistanceTo(tClosestPointOnCurve)
									Catch oEx As Exception
										System.Windows.Forms.MessageBox.Show(oEx.Message, "21_211")
									End Try

								Case 4
									dDist = tPointA.DistanceTo(tPointB)
							End Select
						End If
					End If
					DMAcadExt.AcadDocument.WriteMessage("Dist: " & dDist.ToString() & vbCrLf)
				End If
			End If
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Test03")>
	Public Sub Test03()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try
		End If
		If oEnt IsNot Nothing Then
			Dim tPickedPoint As Point3d = ptRes.PickedPoint
			Select Case oEnt.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oEnt, Arc)

				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)

				Case DMAcadExt.AcadConst.Acad2dPolylineName
					Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)

				Case DMAcadExt.AcadConst.AcadLineName
					Dim dVal As Double
					Dim oLine As Line = DirectCast(oEnt, Line)
					Dim oCurveSegment As DMAcadExt.TplnCurveSegment = New DMAcadExt.TplnCurveSegment(DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint), DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint))
					ptPointRes = oEditor.GetPoint("Point..")
					If ptPointRes.Status = PromptStatus.OK Then
						oEditor.WriteMessage("P " & CStr(ptPointRes.Value.X) & "," & CStr(ptPointRes.Value.Y) & vbCrLf)
						oEditor.WriteMessage("Dist= " & CStr(oCurveSegment.GetDistanceTo(DMAcadExt.TPlnPoint.Point3dTo2d(ptPointRes.Value))) & " ")
						dVal = oCurveSegment.IsOn(DMAcadExt.TPlnPoint.Point3dTo2d(ptPointRes.Value))
						'  oEditor.WriteMessage("; IsOn= " & CStr(dVal))
					End If

				Case DMAcadExt.AcadConst.AcadPointName

				Case DMAcadExt.AcadConst.AcadHatchName

				Case DMAcadExt.AcadConst.AcadBlockRefName

				Case Else
					DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
			End Select
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("Test06")>
	Public Sub TestGetEntity()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Link ...")
		oPromptOpt.SetMessageAndKeywords("\nContinue [Yes/No]: ", "Yes No")

		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'  Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'    DMAcadExt.AcadDocument.OpenLog(False)
		Do
			oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)


			DMCommon.Debug.MsgBox("13_045H", ptRes.Status.ToString(), ptRes.StringResult)
		Loop Until ptRes.Status = PromptStatus.Cancel


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	<CommandMethod("VertInfo")>
	Public Sub VertexInfo()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline/BlockRef... ")
		oPromptOpt.AllowNone = True

		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'  Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing
		Dim tBasePoint As Point2d = Point2d.Origin
		Dim tFirstPointA As Point2d
		Dim tFirstPointB As Point2d
		Dim tPointA As Point2d
		Dim tPointB As Point2d
		Dim dDistAA, dDistAB, dDistBA, dDistBB As Double



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'    DMAcadExt.AcadDocument.OpenLog(False)
		Do
			'	oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			'	DMAcadExt.AcadDocument.WriteMessage("Status: " & ptRes.Status.ToString())
			If ptRes.Status = PromptStatus.OK Then
				If zzGetEntityPoints(ptRes.ObjectId, tPointA, tPointB) Then
					If tBasePoint.IsEqualTo(Point2d.Origin) Then
						If tFirstPointA.IsEqualTo(Point2d.Origin) Then
							If tPointB.IsEqualTo(Point2d.Origin) Then
								tBasePoint = tPointA
								'	DMAcadExt.AcadDocument.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
								oEditor.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
							Else
								tFirstPointA = tPointA
								tFirstPointB = tPointB
							End If
							tPointA = Point2d.Origin
						Else
							dDistAA = DMAcadExt.TPlnPoint.GetDistance(tFirstPointA, tPointA)
							dDistAB = DMAcadExt.TPlnPoint.GetDistance(tFirstPointA, tPointB)
							dDistBA = DMAcadExt.TPlnPoint.GetDistance(tFirstPointB, tPointA)
							dDistBB = DMAcadExt.TPlnPoint.GetDistance(tFirstPointB, tPointB)
							If dDistAA < dDistAB AndAlso dDistAA < dDistBA AndAlso dDistAA < dDistBB Then
								tBasePoint = tFirstPointA

							ElseIf dDistAB < dDistBA AndAlso dDistAB < dDistBB Then
								tBasePoint = tFirstPointA
								tPointA = tPointB
							ElseIf dDistBA < dDistBB Then
								tBasePoint = tFirstPointB
							Else
								tBasePoint = tFirstPointB
								tPointA = tPointB
							End If
							'DMAcadExt.AcadDocument.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
							oEditor.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)

						End If

					End If
					If Not tPointA.IsEqualTo(Point2d.Origin) Then
						If DMAcadExt.TPlnPoint.GetDistance(tBasePoint, tPointA) > DMAcadExt.TPlnPoint.GetDistance(tBasePoint, tPointB) Then
							tPointA = tPointB
						End If
						'DMAcadExt.AcadDocument.WriteMessage("Delta:" & (tPointA.X - tBasePoint.X).ToString() & "," & (tPointA.Y - tBasePoint.Y).ToString() & vbCrLf)
						oEditor.WriteMessage("Delta:" & (tPointA.X - tBasePoint.X).ToString() & "," & (tPointA.Y - tBasePoint.Y).ToString() & vbCrLf)

					End If
				End If
			End If




			'DMCommon.Debug.MsgBox("13_045H", ptRes.Status.ToString(), ptRes.StringResult)
		Loop Until ptRes.Status = PromptStatus.Cancel OrElse ptRes.Status = PromptStatus.None


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("ToPoint")>
	Public Sub ToPoint()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Polyline/BlockRef... ")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		'  Dim bExists As Boolean
		Dim oPolyline As Polyline = Nothing
		Dim oPolylineA As Polyline = Nothing
		Dim oBlockRef As BlockReference = Nothing
		Dim tBasePoint As Point2d = Point2d.Origin
		Dim tFirstPointA As Point2d
		Dim tFirstPointB As Point2d
		Dim tPointA As Point2d
		Dim tPointB As Point2d
		Dim dDistAA, dDistAB, dDistBA, dDistBB As Double

		oPromptOpt.AllowNone = True

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		'    DMAcadExt.AcadDocument.OpenLog(False)
		Do
			'	oEditor.WriteMessage("Select Polyline/BlockRef...: " & vbCrLf)
			ptRes = oEditor.GetEntity(oPromptOpt)
			'	DMAcadExt.AcadDocument.WriteMessage("Status: " & ptRes.Status.ToString())
			If ptRes.Status = PromptStatus.OK Then
				If zzGetEntityPoints(ptRes.ObjectId, tPointA, tPointB) Then
					If tBasePoint.IsEqualTo(Point2d.Origin) Then
						If tFirstPointA.IsEqualTo(Point2d.Origin) Then
							If tPointB.IsEqualTo(Point2d.Origin) Then
								tBasePoint = tPointA
								'	DMAcadExt.AcadDocument.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
								oEditor.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
							Else
								tFirstPointA = tPointA
								tFirstPointB = tPointB
							End If
							tPointA = Point2d.Origin
						Else
							dDistAA = DMAcadExt.TPlnPoint.GetDistance(tFirstPointA, tPointA)
							dDistAB = DMAcadExt.TPlnPoint.GetDistance(tFirstPointA, tPointB)
							dDistBA = DMAcadExt.TPlnPoint.GetDistance(tFirstPointB, tPointA)
							dDistBB = DMAcadExt.TPlnPoint.GetDistance(tFirstPointB, tPointB)
							If dDistAA < dDistAB AndAlso dDistAA < dDistBA AndAlso dDistAA < dDistBB Then
								tBasePoint = tFirstPointA

							ElseIf dDistAB < dDistBA AndAlso dDistAB < dDistBB Then
								tBasePoint = tFirstPointA
								tPointA = tPointB
							ElseIf dDistBA < dDistBB Then
								tBasePoint = tFirstPointB
							Else
								tBasePoint = tFirstPointB
								tPointA = tPointB
							End If
							'DMAcadExt.AcadDocument.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)
							oEditor.WriteMessage("Point: " & tBasePoint.ToString() & vbCrLf)

						End If

					End If
					If Not tPointA.IsEqualTo(Point2d.Origin) Then
						If DMAcadExt.TPlnPoint.GetDistance(tBasePoint, tPointA) > DMAcadExt.TPlnPoint.GetDistance(tBasePoint, tPointB) Then
							tPointA = tPointB
						End If
						'DMAcadExt.AcadDocument.WriteMessage("Delta:" & (tPointA.X - tBasePoint.X).ToString() & "," & (tPointA.Y - tBasePoint.Y).ToString() & vbCrLf)
						oEditor.WriteMessage("Delta:" & (tPointA.X - tBasePoint.X).ToString() & "," & (tPointA.Y - tBasePoint.Y).ToString() & vbCrLf)
						If Not DMAcadExt.TPlnPoint.IsEqualPoint(tBasePoint, tPointA) Then
							oEditor.WriteMessage("Delta:" & (tPointA.X - tBasePoint.X).ToString() & "," & (tPointA.Y - tBasePoint.Y).ToString() & vbCrLf)


						End If
					End If
				End If
			End If




			'DMCommon.Debug.MsgBox("13_045H", ptRes.Status.ToString(), ptRes.StringResult)
		Loop Until ptRes.Status = PromptStatus.Cancel OrElse ptRes.Status = PromptStatus.None


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("LnInfo")>
	Public Sub LnInfo()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try
		End If
		If oEnt IsNot Nothing Then
			Dim tPickedPoint As Point3d = ptRes.PickedPoint
			Select Case oEnt.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oEnt, Arc)
					zzArcInfo(oArc)
				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
					zzSegmentInfo(oPolyline, tPickedPoint)
				Case DMAcadExt.AcadConst.Acad2dPolylineName
					Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
					zzPolyline2dInfo(oPolyline)
				Case DMAcadExt.AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oEnt, Line)
					zzLineInfo(oLine)
				Case DMAcadExt.AcadConst.AcadPointName

				Case DMAcadExt.AcadConst.AcadHatchName

				Case DMAcadExt.AcadConst.AcadBlockRefName

				Case Else
					DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
			End Select
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("IntInf")>
	Public Sub IntersectionInfo()
		Const sIntersectionLayer As String = "LotKParcel"
		Const sXDataAppName As String = "ImportShape"

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oPolyline As Polyline = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

		ptRes = oEditor.GetEntity(oPromptOpt)
		Do
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				Try
					oPolyline = DMAcadExt.AcadTransaction.GetPolyline(ptRes.ObjectId, OpenMode.ForRead, True)
				Catch oEx As Exception
					Return
				End Try
				If oPolyline IsNot Nothing AndAlso oPolyline.Layer <> sIntersectionLayer Then
					oPolyline = Nothing
				End If
			Else
				Exit Do
			End If
		Loop While oPolyline Is Nothing
		If oPolyline IsNot Nothing Then
			Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
			Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
			Dim oValue As System.Object
			Dim iFeatureID As Integer
			Dim iSourceID, iOverlayID, iOverlayID_Add As Integer
			Dim bIDAddExists As Boolean = False
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim oLot As TopoManager.TPlanGraph.TplnLot

			oResBuffer = oPolyline.GetXDataForApplication(sXDataAppName)
			taTypedValues = oResBuffer.AsArray()


			oValue = taTypedValues(2).Value
			'	MessageBox.Show(AcadMapApp.DispArray(taTypedValues, "2MP", False) & vbCrLf & oValue.ToString(), "02_008")

			iFeatureID = CInt(oValue)
			oValue = taTypedValues(3).Value
			iSourceID = CInt(oValue)
			oValue = taTypedValues(4).Value
			iOverlayID = CInt(oValue)
			If bIDAddExists AndAlso (taTypedValues(5).TypeCode = DxfCode.ExtendedDataInteger32) Then
				oValue = taTypedValues(5).Value
				iOverlayID_Add = CInt(oValue)
			End If
			oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iSourceID)
			oLot = TopoManager.TPlanGraph.TplnProject.GetLot(DMAcadExt.enTopoPurpose.Approved, iOverlayID)
			If oParcel IsNot Nothing Then
				oEditor.WriteMessage(vbCrLf & "Parcel: " & oParcel.BlockFull & "," & oParcel.Name)
			End If

			If oLot IsNot Nothing Then
				oEditor.WriteMessage(vbCrLf & "Lot: " & oLot.Name)
			End If


		End If


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("ChIntPoint")>
	Public Sub EntCheckIntersectionPoint()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		Dim tBasePoint As Point3d
		Dim tAddPoint As Point3d
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim tMinPoint As Point3d = DMAcadExt.AcadDocument.GetExtMinPoint3d(True)
		Dim iCompareRes As Integer
		mtShiftVector = New Vector3d(tMinPoint.X, tMinPoint.Y, tMinPoint.Z)

		Do
			ptRes = oEditor.GetEntity(oPromptOpt)

			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				Try
					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
				Catch oEx As Exception
					Return
				End Try
				If oEnt IsNot Nothing Then

					Select Case oEnt.GetRXClass().Name
						Case DMAcadExt.AcadConst.AcadArcName
							Dim oArc As Arc = DirectCast(oEnt, Arc)
							zzCompareTwoPoints(oArc.StartPoint, oArc.EndPoint, tBasePoint, tAddPoint)
						Case DMAcadExt.AcadConst.AcadPolylineName
							Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
							'DMAcadExt.AcadDocument.WriteDebugMessage("!DBG2 " & oPolyline.StartPoint.ToString() & "; " & oPolyline.EndPoint.ToString() & "; " & tBasePoint.ToString() & "; " & tAddPoint.ToString())
							'	zzPrintPoint(tBasePoint, True, 11, False)
							'	zzPrintPoint(tAddPoint, True, 12, False)

							iCompareRes = zzCompareTwoPoints(oPolyline.StartPoint, oPolyline.EndPoint, tBasePoint, tAddPoint)
							If iCompareRes = 1 Then
								'oPolyline.StartPoint = tBasePoint
								zzShiftPoint(mtShiftVector, True, tBasePoint)
								oPolyline.SetPointAt(0, DMAcadExt.TPlnPoint.Point3dTo2d(tBasePoint))
							ElseIf iCompareRes = 2 Then
								zzShiftPoint(mtShiftVector, True, tBasePoint)
								oPolyline.SetPointAt(oPolyline.NumberOfVertices - 1, DMAcadExt.TPlnPoint.Point3dTo2d(tBasePoint))
							End If

						Case DMAcadExt.AcadConst.AcadLineName
							Dim oLine As Line = DirectCast(oEnt, Line)
							zzCompareTwoPoints(oLine.StartPoint, oLine.EndPoint, tBasePoint, tAddPoint)
						Case DMAcadExt.AcadConst.AcadBlockRefName
							Dim oBlockReference As BlockReference = DirectCast(oEnt, BlockReference)
							zzCompareOnePoint(oBlockReference.Position, tBasePoint, tAddPoint)
					End Select
				End If
			Else
				oEditor.WriteMessage("End of Command" & vbCrLf)
				Exit Do
			End If
		Loop

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Private Sub zz1()
		Dim tMinPoint As Point3d = DMAcadExt.AcadDocument.GetExtMinPoint3d(True)
		Dim tVector As Vector3d = New Vector3d(tMinPoint.X, tMinPoint.Y, tMinPoint.Z)
	End Sub
	Private Sub zzShiftPoint(tShift As Vector3d, bDirection As Boolean, ByRef tPoint As Point3d)
		If bDirection Then
			tPoint = tPoint.Add(tShift)
		Else
			tPoint = tPoint.Subtract(tShift)
		End If

	End Sub
	Private Function zzCompareTwoPoints(tFirstPoint As Point3d, tLastPoint As Point3d, ByRef tBasePoint As Point3d, ByRef tAddPoint As Point3d) As Integer
		Dim iRes As Integer
		zzShiftPoint(mtShiftVector, False, tFirstPoint)
		zzShiftPoint(mtShiftVector, False, tLastPoint)


		If tBasePoint = New Point3d Then
			tBasePoint = tFirstPoint
			tAddPoint = tLastPoint
			DMAcadExt.AcadDocument.WriteMessage("1s" & vbCrLf)
			iRes = -1
		ElseIf tAddPoint = New Point3d Then
			If tFirstPoint.DistanceTo(tBasePoint) > tLastPoint.DistanceTo(tBasePoint) Then
				zzCompareTwoPoints(tLastPoint, tBasePoint)
				iRes = 2
			Else
				zzCompareTwoPoints(tFirstPoint, tBasePoint)
				iRes = 1
			End If
		Else
			Dim dDistMin As Double
			Dim tPoint As Point3d
			Dim tNewBasePoint As Point3d
			'	DMAcadExt.AcadDocument.WriteDebugMessage("!DBG1 F:" & tFirstPoint.ToString() & " L:" & tLastPoint.ToString() & " B:" & tBasePoint.ToString() & " A:" & tAddPoint.ToString() & vbCrLf)
			If tFirstPoint.DistanceTo(tBasePoint) > tFirstPoint.DistanceTo(tAddPoint) Then
				dDistMin = tFirstPoint.DistanceTo(tAddPoint)
				tPoint = tFirstPoint
				tNewBasePoint = tAddPoint
				iRes = 1
				'DMAcadExt.AcadDocument.WriteDebugMessage("!DBG2 " & dDistMin.ToString())
			Else
				dDistMin = tFirstPoint.DistanceTo(tBasePoint)
				tPoint = tFirstPoint
				tNewBasePoint = tBasePoint
				iRes = 1
				'DMAcadExt.AcadDocument.WriteDebugMessage("!DBG3 " & dDistMin.ToString())
			End If

			If tLastPoint.DistanceTo(tBasePoint) < dDistMin Then
				dDistMin = tLastPoint.DistanceTo(tBasePoint)
				tPoint = tLastPoint
				tNewBasePoint = tBasePoint
				iRes = 2
				'	DMAcadExt.AcadDocument.WriteDebugMessage("!DBG4 " & dDistMin.ToString())
			End If
			If tLastPoint.DistanceTo(tAddPoint) < dDistMin Then
				dDistMin = tLastPoint.DistanceTo(tAddPoint)
				tPoint = tLastPoint
				tNewBasePoint = tAddPoint
				iRes = 2
				'DMAcadExt.AcadDocument.WriteDebugMessage("!DBG5 " & dDistMin.ToString())
			End If
			tBasePoint = tNewBasePoint
			tAddPoint = New Point3d()
			'	DMAcadExt.AcadDocument.WriteDebugMessage("!DBG1 " & tPoint.ToString() & "; " & tBasePoint.ToString())
			zzCompareTwoPoints(tPoint, tBasePoint)

		End If
		Return iRes
	End Function
	Private Sub zzCompareTwoPoints(tPoint As Point3d, tBasePoint As Point3d)
		If tPoint.X <> tBasePoint.X OrElse tPoint.Y <> tBasePoint.Y Then
			zzPrintPoint(tPoint, True, 1, True)
			zzPrintPoint(tBasePoint, True, 2, True)
		Else
			'zzPrintPoint(tPoint, True, 101)
			'zzPrintPoint(tBasePoint, True, 102)
			DMAcadExt.AcadDocument.WriteDebugMessage("OK!" & vbCrLf)
		End If
	End Sub
	Private Sub zzCompareTwoPoints_202223(tPoint As Point3d, tBasePoint As Point3d)
		If tPoint.X <> tBasePoint.X OrElse tPoint.Y <> tBasePoint.Y Then
			DMAcadExt.AcadDocument.WriteDebugMessage("Point 1 - " & tPoint.ToString() & vbCrLf & "Point 2 - " & tBasePoint.ToString() & vbCrLf)
		Else
			DMAcadExt.AcadDocument.WriteDebugMessage("OK!")
		End If
	End Sub
	Private Function zzToDblString(dVal As Double) As String
		Dim dInt As Double = Math.Floor(dVal)
		Dim dFraction As Double = dVal - dInt
		Dim dFractionToInt As Double = dFraction * 1.0E+16#
		Dim sFraction As String = CStr(Convert.ToInt64(dFractionToInt))
		Dim iAdd As Integer = 16 - sFraction.Length
		If iAdd > 0 Then
			sFraction = Strings.StrDup(iAdd, "0") & sFraction
		End If

		Return CStr(Convert.ToInt64(dInt)) & "." & sFraction

	End Function
	Private Sub zzPrintPoint(tPoint As Point3d, bShift As Boolean, iNo As Integer, Optional bIncreasedAccuracy As Boolean = False)
		Dim sOut As String
		If bShift Then
			'	DMAcadExt.AcadDocument.WriteDebugMessage("!Dbg2 " & (tPoint.X.ToString()) & " " & mtShiftVector.X.ToString() & vbCrLf)
			'	DMAcadExt.AcadDocument.WriteDebugMessage("!Dbg3 " & (tPoint.Y.ToString()) & " " & mtShiftVector.Y.ToString() & vbCrLf)

			tPoint = New Point3d(tPoint.X + mtShiftVector.X, tPoint.Y + mtShiftVector.Y, 0)

			sOut = "Point " & iNo.ToString() & " ??? " & CStr(tPoint.X) & ", " & CStr(tPoint.Y)
			'zzShiftPoint(mtShiftVector, True, tPoint)
		End If




		If bIncreasedAccuracy Then
			sOut = "Point " & iNo.ToString() & " --- " & zzToDblString(tPoint.X) & ", " & zzToDblString(tPoint.Y)

		Else
			sOut = "Point " & iNo.ToString() & " --- " & CStr(tPoint.X) & ", " & CStr(tPoint.Y)
		End If
		DMAcadExt.AcadDocument.WriteMessage(sOut & vbCrLf)
	End Sub
	Private Sub zzCompareOnePoint(tPoint As Point3d, ByRef tBasePoint As Point3d, ByRef tAddPoint As Point3d)
		zzShiftPoint(mtShiftVector, False, tPoint)
		If tBasePoint = New Point3d Then
			tBasePoint = tPoint
			DMAcadExt.AcadDocument.WriteMessage(vbCrLf)
		ElseIf tAddPoint = New Point3d Then
			zzCompareTwoPoints(tPoint, tBasePoint)
		Else
			If tPoint.DistanceTo(tBasePoint) > tPoint.DistanceTo(tAddPoint) Then
				zzCompareTwoPoints(tPoint, tAddPoint)
			Else
				zzCompareTwoPoints(tPoint, tBasePoint)
			End If
		End If

	End Sub

	<CommandMethod("EntInfo")>
	Public Sub EntInfo()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				Return
			End Try
		End If
		If oEnt IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(" Handle:" & oEnt.Handle.ToString() & "; ID:" & oEnt.ObjectId.ToString())
			Select Case oEnt.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oEnt, Arc)
					zzArcInfo(oArc)
				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
					zzPolylineInfo(oPolyline, True)
				Case DMAcadExt.AcadConst.Acad2dPolylineName
					Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
					zzPolyline2dInfo(oPolyline)
				Case DMAcadExt.AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oEnt, Line)
					zzLineInfo(oLine)
				Case DMAcadExt.AcadConst.AcadPointName
					Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
					zzPointInfo(oDBPoint)
				Case DMAcadExt.AcadConst.AcadPointName
					Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
					zzPointInfo(oDBPoint)
				Case DMAcadExt.AcadConst.AcadHatchName
					Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = DirectCast(oEnt, Hatch)
					zzHatchInfo(oHatch)
				Case DMAcadExt.AcadConst.AcadBlockRefName
					Dim oBlockReference As BlockReference = DirectCast(oEnt, BlockReference)
					zzBlockRefInfo(oBlockReference)
				Case Else
					DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
			End Select
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("NodeList")>
	Public Sub NodeList()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Link")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
			Catch oEx As Exception
				Return
			End Try
		End If
		If oEnt IsNot Nothing Then
			Select Case oEnt.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oEnt, Arc)
					zzArcInfo(oArc)
				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
					zzPolylineInfo(oPolyline)
				Case DMAcadExt.AcadConst.Acad2dPolylineName
					Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
					zzPolyline2dInfo(oPolyline)
				Case DMAcadExt.AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oEnt, Line)
					zzLineInfo(oLine)
				Case DMAcadExt.AcadConst.AcadPointName
					Dim oDBPoint As DBPoint = DirectCast(oEnt, DBPoint)
					zzPointInfo(oDBPoint)
				Case Else
					DMAcadExt.AcadDocument.WriteMessage("Entity: " & oEnt.GetRXClass().Name & ":" & oEnt.GetType().ToString())
			End Select
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TopoList")>
	Public Sub TopoList()

		Dim saAllTopoNames() As String = Nothing
		Dim saUnionTopoNames() As String = Nothing


		ODEditor.GetTopoNames(saAllTopoNames, saUnionTopoNames)
		Dim sAllOut As String = Join(saAllTopoNames, vbCrLf)
		Dim sUnionOut As String = Join(saAllTopoNames, vbCrLf)

		System.Windows.Forms.MessageBox.Show(sAllOut & vbCrLf & sUnionOut)
	End Sub
	<CommandMethod("DispTopo")>
	Public Sub DispTopo()
		mfTopoView = New frmTopoView
		mfTopoView.ShowDialog()
		mfTopoView.Dispose()
	End Sub
	<CommandMethod("DispTpln")>
	Public Sub DispTpln()
		' mfTplnView = New TPlanGraph.frmTplnView
		'  mfTplnView.ShowDialog()
		'  mfTplnView.Dispose()
		TopoManager.TPlanGraph.TplnProject.InitializeServerDB_SQL()
		If TopoManager.TPlanGraph.TplnProject.InitializedServerDB Then
			If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
				mfTplnView = New frmTplnView(New DMAcadExt.MapThemeData)
			End If

			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
		End If
	End Sub




	<CommandMethod("TplnCalc")> Public Sub TplnCalc()
		UnionTplnM()
		UnionTplnK()


	End Sub
	<CommandMethod("TestColor", CommandFlags.Session)> Public Shared Sub TestColor()
		'Autodesk.AutoCAD.Windows
		Dim oColorDialog As Autodesk.AutoCAD.Windows.ColorDialog
		oColorDialog = New Autodesk.AutoCAD.Windows.ColorDialog()
		oColorDialog.ShowDialog()
		Dim oColor As Autodesk.AutoCAD.Colors.Color = oColorDialog.Color

		MessageBox.Show(oColor.ColorValue.Name, "17_998")
		MessageBox.Show(oColor.ColorNameForDisplay, "17_999")
	End Sub
	<CommandMethod("ToMPoly", CommandFlags.Session)> Public Shared Sub GetToMPolygon()
		Const sPolylineLayer As String = "TplnParcelsClosed"
		Const sMPolygonLayer As String = "TplnParcelMpgons"

		Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
		Dim oMPoly As MPolygon
		Dim tAcObjID As ObjectId

		For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolylines
			oMPoly = New MPolygon()
			oMPoly.AppendLoopFromBoundary(oPolygon, False, 0.001)
			oMPoly.Layer = sMPolygonLayer
			tAcObjID = DMAcadExt.AcadTransaction.AppendEntity(oMPoly)
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("SegmTypes", CommandFlags.Session)> Public Shared Sub TestSegmentTypes()
		Dim iSegmentIndex As Integer = 0
		Dim iSegmentType As SegmentType

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try

				Dim oDBObject As DBObject
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForRead)

				If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
					Do
						Try
							iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ": " & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_129")
							Exit Do
						End Try
						oEditor.WriteMessage("SegmTypes: " & iSegmentIndex.ToString() & " of " & CStr(oPolyline.NumberOfVertices) & ": " & iSegmentType.ToString() & vbCrLf)
						iSegmentIndex += 1


					Loop Until (iSegmentIndex = oPolyline.NumberOfVertices)

				End If


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()





	End Sub
	<CommandMethod("TestMPoly", CommandFlags.Session)> Public Shared Sub TestMPoly()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try

				Dim oDBObject As DBObject
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

				If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)

					Dim oMPoly As MPolygon = New MPolygon()

					oMPoly.AppendLoopFromBoundary(oPolyline, False, 0.001)
					DMAcadExt.AcadDocument.WriteMessage("Before: " & oMPoly.GetType().ToString & ";" & oMPoly.GetRXClass().Name)
					Dim tAcObjID As ObjectId = DMAcadExt.AcadTransaction.AppendEntity(oMPoly)
					DMAcadExt.AcadDocument.WriteMessage("after: " & oMPoly.GetType().ToString & ";" & oMPoly.GetRXClass().Name)
					DMAcadExt.AcadDocument.WriteMessage("MPolyId: " & tAcObjID.ToString() & ";" & oMPoly.ObjectId.ToString())
					Dim oMPDBObject As DBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
					DMAcadExt.AcadDocument.WriteMessage("Type: " & oMPDBObject.GetType().ToString & ";" & oMPDBObject.GetRXClass().Name)
					Dim colObj As Autodesk.AutoCAD.DatabaseServices.DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()

					For i As Integer = 0 To colObj.Count - 1
						oDBObject = colObj.Item(i)
						DMAcadExt.AcadDocument.WriteMessage("All: " & oDBObject.GetType().ToString & ";" & oDBObject.GetRXClass().Name & ";" & oDBObject.ObjectId.ToString())
					Next
				End If

				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub


	<CommandMethod("TestPMP", CommandFlags.Session)> Public Shared Sub TestPointInMPoly()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim ptPointRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		'		Dim oMPoly As MPolygon
		Dim colLoops As IntegerCollection
		Dim bExists As Boolean
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


		moMPolygonA = New MPolygon()
		Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

		Do
			oEditor.WriteMessage("Start: " & vbCrLf)
			ptEntRes = oEditor.GetEntity(oPromptOpt)
			If ptEntRes.Status = PromptStatus.OK Then
				Try

					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)
					oEditor.WriteMessage("oEnt: " & vbCrLf)
				Catch oEx As Exception
					oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
					DMAcadExt.AcadTransaction.Abort()
					DMAcadExt.AcadDocument.Unlock()
					Return
				End Try
				Select Case oEnt.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							moMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))
							DMAcadExt.AcadTransaction.Abort()
							DMAcadExt.AcadDocument.Unlock()
							Return
						End Try
					Case DMAcadExt.AcadConst.Acad2dPolylineName
						Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							moMPolygonA.AppendLoopFromBoundary(oPolyline, False, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))
							DMAcadExt.AcadTransaction.Abort()
							DMAcadExt.AcadDocument.Unlock()
							Return
						End Try

				End Select
			Else
				Exit Do
			End If
		Loop
		If bExists Then
			Try
				moMPolygonA.BalanceTree()
				For Each oDBObject As DBObject In colDBObject
					'	oDBObject.Erase()
				Next

				DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
				moMPolygonA.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
				DMAcadExt.AcadTransaction.AppendEntity(moMPolygonA)
				DMAcadExt.AcadTransaction.CloseModelSpace()


			Catch oEx As Exception
				oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))
				DMAcadExt.AcadTransaction.Abort()
				DMAcadExt.AcadDocument.Unlock()
				Return
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("???", "21_223")
		End If

		Try
			Dim iLoopInd As Integer
			Dim iDir As Autodesk.AutoCAD.DatabaseServices.LoopDirection '= oMPolygonA.GetLoopDirection(1)
			If moMPolygonA IsNot Nothing Then
				ptPointRes = oEditor.GetPoint("Point..")
				If ptPointRes.Status = PromptStatus.OK Then
					oEditor.WriteMessage("P " & CStr(ptPointRes.Value.X) & "," & CStr(ptPointRes.Value.Y) & vbCrLf)
					colLoops = moMPolygonA.IsPointInsideMPolygon(ptPointRes.Value, 0.001)
					oEditor.WriteMessage("LoopCount " & CStr(colLoops.Count) & vbCrLf)
					For iIndex As Integer = 0 To colLoops.Count - 1
						iLoopInd = colLoops.Item(iIndex)
						iDir = moMPolygonA.GetLoopDirection(iLoopInd)

						oEditor.WriteMessage("&& " & CStr(iIndex) & ":" & "N=" & CStr(moMPolygonA.NumMPolygonLoops) & ";" & iDir.ToString() & "||" & CStr(colLoops.Count) & vbCrLf)
					Next
				End If
			End If
			'&& 0:1||1
			DMAcadExt.AcadDocument.CloseMessage()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
		End Try




		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub



	<CommandMethod("TestPLine", CommandFlags.Session)> Public Shared Sub TestPLine()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim ptDoubleRes As Autodesk.AutoCAD.EditorInput.PromptDoubleResult

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim tStartPoint As Point2d
				Dim tNextPoint As Point2d
				Dim tPoint As Point2d
				Dim oDBObject As DBObject
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

				If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
					oPolyline.Reset(True, 2)

					If False Then
						ptDoubleRes = oEditor.GetDouble("Enter new value for tolerance <" & CStr(mdTolerance) & ">:")
						If ptDoubleRes.Status = PromptStatus.OK Then
							mdTolerance = ptDoubleRes.Value
						End If
						If ptDoubleRes.Status <> PromptStatus.Cancel Then
							DMAcadExt.dmLineCleanup.StraightenPolylineTest(oPolyline, mdTolerance)
						End If
					End If


					If False Then
						For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
							tPoint = oPolyline.GetPoint2dAt(iIndex)
							DMAcadExt.AcadDocument.WriteMessage("Before=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
							If iIndex = 0 Then
								tStartPoint = oPolyline.GetPoint2dAt(iIndex)
								DMAcadExt.AcadDocument.WriteMessage("Start=" & CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y))
							ElseIf iIndex = 1 Then
								tNextPoint = oPolyline.GetPoint2dAt(iIndex)
								oPolyline.AddVertexAt(1, New Point2d(tStartPoint.X, tNextPoint.Y), 0.0, 0, 0)
							End If
							tPoint = oPolyline.GetPoint2dAt(iIndex)
							DMAcadExt.AcadDocument.WriteMessage("After=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
						Next
					End If
				End If

				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("PLineList", CommandFlags.Session)> Public Shared Sub PLineList()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try

				Dim tPoint As Point2d
				Dim iSegmentType As SegmentType
				Dim oDBObject As DBObject
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

				If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
					For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
						tPoint = oPolyline.GetPoint2dAt(iIndex)
						iSegmentType = oPolyline.GetSegmentType(iIndex)
						DMAcadExt.AcadDocument.WriteMessage("iIndex=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y) & " Type=" & iSegmentType.ToString())

					Next
				End If


				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestPLineA", CommandFlags.Session)> Public Shared Sub TestPLineA()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim ptDoubleRes As Autodesk.AutoCAD.EditorInput.PromptDoubleResult
		Dim oTopoDef As DMAcadExt.TopoDef = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim tStartPoint As Point2d
				Dim tNextPoint As Point2d
				Dim tPoint As Point2d
				Dim oDBObject As DBObject

				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

				If oDBObject.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then


					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)

					ptDoubleRes = oEditor.GetDouble("Enter new value for tolerance <" & CStr(mdTolerance) & ">:")
					If ptDoubleRes.Status = PromptStatus.OK Then
						mdTolerance = ptDoubleRes.Value
					End If
					If ptDoubleRes.Status <> PromptStatus.Cancel Then
						DMAcadExt.dmLineCleanup.StraightenPolylineZ(oPolyline, True, mdTolerance, oTopoDef, False)
					End If

					If False Then
						For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 1
							tPoint = oPolyline.GetPoint2dAt(iIndex)
							DMAcadExt.AcadDocument.WriteMessage("Before=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
							If iIndex = 0 Then
								tStartPoint = oPolyline.GetPoint2dAt(iIndex)
								DMAcadExt.AcadDocument.WriteMessage("Start=" & CStr(tStartPoint.X) & "," & CStr(tStartPoint.Y))
							ElseIf iIndex = 1 Then
								tNextPoint = oPolyline.GetPoint2dAt(iIndex)
								oPolyline.AddVertexAt(1, New Point2d(tStartPoint.X, tNextPoint.Y), 0.0, 0, 0)
							End If
							tPoint = oPolyline.GetPoint2dAt(iIndex)
							DMAcadExt.AcadDocument.WriteMessage("After=" & CStr(iIndex) & ":" & CStr(tPoint.X) & "," & CStr(tPoint.Y))
						Next
					End If
				End If

				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "21_123")
			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("EraseOD", CommandFlags.Session)> Public Shared Sub EraseAllODTables()

		' Dim oLinkTable As DMAcadExt.ODTable
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sTableName As String = "LotKParcel"
		Dim sPolylineLayer As String = "LotKParcel"
		'    Dim iTestCounter As Integer
		Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing



		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()

		DMAcadExt.ODTable.EraseAllTables()

		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub

	<CommandMethod("TestOD", CommandFlags.Session)> Public Shared Sub TestOD()
		Dim oLinkTable As DMAcadExt.ODTable
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sTableName As String = "LotKParcel"
		Dim sPolylineLayer As String = "LotKParcel"
		Dim iTestCounter As Integer
		Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing

		Dim dValue As Double
		Dim sHandle As String
		Dim lstPolylines As System.Collections.Generic.IList(Of Autodesk.AutoCAD.DatabaseServices.Polyline)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()

		oLinkTable = New DMAcadExt.ODTable(sTableName)
		If oLinkTable.Exists Then
			iTestCounter = 0

			lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, OpenMode.ForRead)
			For Each oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline In lstPolylines

				iTestCounter += 1
				sHandle = oPolygon.Handle.ToString()
				Try
					oODRec = oLinkTable.GetODRecord(oPolygon.ObjectId)

				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadFDO_Overlay_9")
					System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sTableName, "04_200yy")
				End Try
				If oODRec IsNot Nothing Then
					'	oODRec.Init()
					Dim iFeatureID As Integer
					Dim iParcelID As Integer
					Dim iLotID As Integer

					Try
						dValue = oODRec.Item(0).DoubleValue
						iFeatureID = Convert.ToInt32(dValue)
						dValue = oODRec.Item(1).DoubleValue
						iParcelID = Convert.ToInt32(dValue)
						dValue = oODRec.Item(3).DoubleValue
						iLotID = Convert.ToInt32(dValue)
						If iTestCounter < 24 Then
							oEditor.WriteMessage("27-- " & ":" & CStr(iFeatureID) & ":" & CStr(iParcelID) & ":" & CStr(iLotID) & vbCrLf)
						End If

					Catch oEx As Exception

						System.Windows.Forms.MessageBox.Show(oEx.Message, "04_208")
					End Try

				End If

			Next
		Else
			oEditor.WriteMessage("25-- Not exists" & ":" & CStr(sTableName) & vbCrLf)
		End If



		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestOD1", CommandFlags.Session)> Public Shared Sub TestODTables()

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim sTableName As String = "LotKParcel"
		Dim sPolylineLayer As String = "LotKParcel"

		Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()

		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Dim oODEditor As TopoManager.ODEditor = New ODEditor()
			oODEditor.GetObjectData(ptRes.ObjectId)
		End If



		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestPrjXData", CommandFlags.Session)> Public Shared Sub TestPrjXData()

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		'Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()

		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taObjIDs() As ObjectId
		Dim oXDataPrjFile As DMAcadExt.TplnXDataPrjFile
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(DMAcadExt.TplnXDataPrjFile.XDataAppName)
		'   ptRes = oEditor.GetEntity(oPromptOpt)
		Dim oPromptSelOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptSelRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim oEnt As DBObject = Nothing
		ptSelRes = oEditor.GetSelection(oPromptSelOpt)

		If ptSelRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptSelRes.Value
			taObjIDs = oSelSet.GetObjectIds()
			For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)

				oEnt = DMAcadExt.AcadTransaction.GetDBObject(taObjIDs(iIndex), OpenMode.ForWrite)
				oXDataPrjFile = New DMAcadExt.TplnXDataPrjFile(2)
				oXDataPrjFile.ID = 3
				oXDataPrjFile.Priority = 300
				oEnt.XData = oXDataPrjFile.GetResBuffer()
			Next
		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	<CommandMethod("TestXData", CommandFlags.Session)> Public Shared Sub TestXData()
		Const sAppName As String = "SourceID"
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		Dim oVal As Autodesk.AutoCAD.DatabaseServices.TypedValue
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(sAppName)
				If bRes Then
					Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
					oVal = New TypedValue(1001, sAppName)
					oResBuffer.Add(oVal)
					oVal = New TypedValue(1071, 32768)
					oResBuffer.Add(oVal)
					oVal = New TypedValue(1005, oEnt.Handle)
					oResBuffer.Add(oVal)

					oEnt.XData = oResBuffer
				End If
				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
			End Try

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("GetXData", CommandFlags.Session)> Public Shared Sub GetXData()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oResBuffer As ResultBuffer
		Dim shCode As Short
		Dim oValue As System.Object
		Dim sValue As String
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)
				oResBuffer = oEnt.XData
				Dim taTypedValues() As TypedValue = oResBuffer.AsArray()
				For iIndex As Integer = 0 To taTypedValues.GetUpperBound(0)
					shCode = taTypedValues(iIndex).TypeCode
					oValue = taTypedValues(iIndex).Value
					If shCode = 1004S Then
						Dim baVal() As Byte = DirectCast(oValue, Byte())
						For iIndexAr As Integer = 0 To baVal.GetUpperBound(0)
							DMAcadExt.AcadDocument.WriteMessage("Index=" & CStr(iIndexAr) & ", Value=" & baVal(iIndexAr).ToString() & ", Type=" & baVal(iIndexAr).GetType().ToString())
							'	DMAcadExt.AcadDocument.WriteMessage("Type=" & oValue.GetType().ToString())

						Next
					Else
						sValue = Replace(oValue.ToString(), """", """""")
						DMAcadExt.AcadDocument.WriteMessage("Code=" & CStr(shCode) & ", Value=" & sValue & ", Type=" & oValue.GetType().ToString())

					End If
					'	DMAcadExt.AcadDocument.WriteMessage("Type=" & oValue.GetType().ToString())

				Next

				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception

			End Try

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("ClearOD", CommandFlags.Session)> Public Shared Sub ClearOD()

		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim oEnt As DBObject = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForWrite)
				DMAcadExt.AcadMapApp.ClearOD(oEnt)
				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
			End Try

		End If
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestArc", CommandFlags.Session)> Public Shared Sub TestArc()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Arc")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		'     DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		ptRes = oEditor.GetEntity(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				Dim oArc As Arc = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead), Arc)
				DMAcadExt.AcadDocument.WriteMessage("Center=" & CStr(oArc.Center.X) & "," & CStr(oArc.Center.Y))
				DMAcadExt.AcadDocument.WriteMessage("Start,End-Total=" & CStr(oArc.StartAngle) & "," & CStr(oArc.EndAngle) & "<>" & CStr(oArc.TotalAngle))
				DMAcadExt.AcadDocument.WriteMessage("Radius,Length=" & CStr(oArc.Radius) & "," & CStr(oArc.Length))
				Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc)

				DMAcadExt.AcadDocument.WriteMessage("MidPoint=" & CStr(oTplnArc.GetMidPoint().ToString()))


				''''''''''''''''''''  DMAcadExt.dmLineCleanup.StraightenArcTest(oArc, 0.4, False, Nothing, False)
				DMAcadExt.AcadDocument.CloseMessage()
			Catch oEx As Exception

			End Try

		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("DocIsLock")> Public Sub DocIsLock()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("CreateTopo", CommandFlags.Session)> Public Shared Sub CreateTopo()
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim colLines As ObjectIdCollection
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		Dim tResultODTable As Autodesk.Gis.Map.Topology.ObjectDataTable = New Autodesk.Gis.Map.Topology.ObjectDataTable()
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
		Dim sTopoName As String = Nothing
		Try
			'	sTopoName = DMAcadExt.AcadUtil.GetName("Topology Name ...")
			sTopoName = "aaa"
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
			sTopoName = Nothing
		End Try
		If sTopoName IsNot Nothing Then
			'colLines = DMAcadExt.AcadUtil.GetLinesA()

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
			colLines = DMAcadExt.AcadTransaction.GetLinksNew("C1662")

			If colLines IsNot Nothing Then

				Try
					'oTopos.Create(sTopoName, colLines, colNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Polygon, Autodesk.Gis.Map.Topology.CreateOptions.IgnoreIncompleteArea, 1.1)
					oTopos.Create(sTopoName, colLines, colNodes, colCentroids, Autodesk.Gis.Map.Topology.TopologyTypes.Linear)

				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "")
				End Try

				DMAcadExt.AcadTransaction.CloseModelSpace()
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
			End If
		End If
	End Sub


	<CommandMethod("ScanLayer")> Public Sub ScanLayer()
		Dim sLayerName As String = Nothing
		Try
			sLayerName = DMAcadExt.AcadUtil.GetName("Enter Layer Name ...")

		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_2")
			sLayerName = Nothing
		End Try
		If sLayerName IsNot Nothing Then
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForRead)
			DMAcadExt.AcadTransaction.ScanLayer(sLayerName)
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
		End If

	End Sub
	<CommandMethod("TplnClose")> Public Sub TplnClose()

		If True Then
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadTransaction.Terminate()

			If mfTopoActions IsNot Nothing Then
				If Not mfTopoActions.IsDisposed Then
					mfTopoActions.Close()
					mfTopoActions.Dispose()
				End If
				mfTopoActions = Nothing
			End If
			If mfTplnView IsNot Nothing Then
				If Not mfTplnView.IsDisposed Then
					mfTplnView.Close()
					mfTplnView.Dispose()
				End If
				mfTplnView = Nothing
			End If
			TopoManager.TPlanGraph.TplnProject.Close()
			BamashNet.bmBamash.Close()

		End If

	End Sub
	<CommandMethod("StrC")>
	Public Sub StraightenCurve()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Entity")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim oEnt As DBObject = Nothing
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		ptRes = oEditor.GetEntity(oPromptOpt)

		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			DMAcadExt.dmLineCleanup.StraightenCurve(ptRes.ObjectId, 0.001, True, "")


		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	<CommandMethod("TestLockA")>
	Public Sub TestLockA()
		DMAcadExt.AcadDocument.TestAcadDoc("TestLockA")
	End Sub

	<CommandMethod("TestLockB")>
	Public Sub TestLockB()
		DMAcadExt.AcadDocument.TestAcadDoc("Bef:TestLockA")
		DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Read, True)
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.TestAcadDoc("Aft:TestLockA")

	End Sub
	Public Sub TestLockC()
		DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Read, True)
	End Sub
	<CommandMethod("TestIE")> Public Sub TestIsEqual()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oBlockRefA As BlockReference
		Dim oBlockRefB As BlockReference
		Dim bIsEqual As Boolean
		DMAcadExt.AcadTransaction.Start()
		oBlockRefA = zzGetBlockRef()
		oBlockRefB = zzGetBlockRef()
		If oBlockRefA IsNot Nothing AndAlso oBlockRefB IsNot Nothing Then
			bIsEqual = oBlockRefA.Position.IsEqualTo(oBlockRefB.Position)
			oEditor.WriteMessage("IsEqualTo " & bIsEqual.ToString & vbCrLf)

			bIsEqual = oBlockRefA.Position = oBlockRefB.Position
			oEditor.WriteMessage(" = " & bIsEqual.ToString & vbCrLf)

			bIsEqual = oBlockRefA.Position.X = oBlockRefB.Position.X AndAlso oBlockRefA.Position.Y = oBlockRefB.Position.Y
			oEditor.WriteMessage("= by X,Y " & bIsEqual.ToString & vbCrLf)
		End If
		DMAcadExt.AcadTransaction.Terminate()

	End Sub
	<CommandMethod("ToC1662")> Public Sub ToC1662()
		Dim taHandle() As Handle = zzGetHandleList()
		Dim oEntity As Entity
		DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Write, True)

		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
		DMAcadExt.AcadTransaction.OpenHandleDictionary()

		For iIndex As Integer = 0 To taHandle.GetUpperBound(0)
			oEntity = DMAcadExt.AcadTransaction.GetEntity(taHandle(iIndex), OpenMode.ForWrite)
			If oEntity IsNot Nothing Then
				oEntity.Layer = "C1662"
			End If
		Next
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	<CommandMethod("TestClosestDistances")> Public Sub TestClosestDistances()

		DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Write, True)

		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

		Dim ed As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		Dim ids As List(Of ObjectId) = New List(Of ObjectId)


		' use the transaction to open these objects.
		' the transaction will automatically dispose these objects when done
		' so we don't have to worry about manually disposing them.

		'Dim bt As BlockTable = tr.GetObject(db.BlockTableId, OpenMode.ForRead) As BlockTable;
		'bleRecord ms = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;            

		Using oCurve As Line = New Line(New Point3d(0, 20, 0), New Point3d(200, 20, 0))

			Using handrailLine As Line = New Line(New Point3d(-500, 50, 0), New Point3d(500, 50, 0))

				Dim pointOnCurve3d() As PointOnCurve3d = oCurve.GetGeCurve().GetClosestPointTo(handrailLine.GetGeCurve())

				' check how many points you have here
				Dim pointOnCurveClosestToHandrailLine As Point3d = pointOnCurve3d.First().Point

				Dim pointOnHandrailLineClosestToCurve As Point3d = handrailLine.GetClosestPointTo(pointOnCurveClosestToHandrailLine, False)

				' distance should be 30;
				Dim distanceBetweenTwoCurves As Double = pointOnCurveClosestToHandrailLine.DistanceTo(pointOnHandrailLineClosestToCurve)


			End Using
		End Using

	End Sub
	<CommandMethod("TestCls")> Public Sub TestClosest()
		DMAcadExt.AcadDocument.DocLock(DocumentLockMode.Write, True)

		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
		Dim oDBPoint As DBPoint
		Dim oPLine As Polyline = Nothing
		Dim tResPoint As Point3d
		Dim colPLinesObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllDBObjectsByRxClass(DMAcadExt.AcadConst.AcadPolylineName)
		Dim dParam As Double
		For Each tPointObjID As ObjectId In colPLinesObjIDs
			oPLine = DMAcadExt.AcadTransaction.GetPolyline(tPointObjID, OpenMode.ForRead)
			If oPLine.Layer = "pclp004" AndAlso oPLine.Handle.Value = &HDE3 Then

				Exit For
			End If
		Next

		If oPLine IsNot Nothing Then

			Dim colPointObjIDs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllDBObjectsByRxClass(DMAcadExt.AcadConst.AcadPointName)
			For Each tPointObjID As ObjectId In colPointObjIDs
				oDBPoint = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tPointObjID, OpenMode.ForRead), DBPoint)
				tResPoint = oPLine.GetClosestPointTo(oDBPoint.Position, True)

				DMAcadExt.AcadTransaction.InsertPoint(tResPoint,, 1)
				dParam = oPLine.GetParameterAtPoint(tResPoint)
				DMAcadExt.AcadDocument.WriteMessage(dParam.ToString())
			Next


		End If



		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub

	Private Sub mfTopoActions_AppExit() Handles mfTopoActions.AppExit
		If mfTplnView IsNot Nothing Then
			mfTplnView.Close()
			mfTplnView.Dispose()
			mfTplnView = Nothing
		End If

		If mfTopoActions IsNot Nothing Then

			If mfTopoActions.Visible Then
				mfTopoActions.Close()
			End If
			mfTopoActions.Dispose()
			mfTopoActions = Nothing
		End If

	End Sub
	Private Sub mfTopoActions_Calculate() Handles mfTopoActions.Calculate
		If mfTplnView IsNot Nothing Then
			mfTplnView.Reset()
		End If
	End Sub

	Private Sub mfTplnView_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mfTplnView.Deactivate
		If mfTplnView.DialogResult = DialogResult.Yes Then
			mfTplnView.DialogResult = DialogResult.No
			mfTplnView.Dispose()
			mfTplnView = Nothing
		End If
	End Sub


	Private Sub moAcadDocument_CommandEnded(ByVal oSender As System.Object, ByVal e As Autodesk.AutoCAD.ApplicationServices.CommandEventArgs) Handles moAcadDocument.CommandEnded
		System.Windows.Forms.MessageBox.Show(e.GlobalCommandName, "29_767")
		Select Case e.GlobalCommandName
			Case "LAYER"
				mfTopoActions.RefreshLayers()
			Case "MAPTOPOCREATE", "MAPTOPODEL", "MAPTOPOREN"
				mfTopoActions.RefreshTopo()
			Case Else
				' System.Windows.Forms.MessageBox.Show(e.GlobalCommandName, "29_769")
		End Select

	End Sub

	Private Sub mfTopoActions_DisplayBasePgon(iOverlayIndex As DMAcadExt.enOverlayIndex, bParcel As Boolean, iTopoID As Integer) Handles mfTopoActions.DisplayBasePgon
		If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
			mfTplnView = New frmTplnView(New DMAcadExt.MapThemeData)
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
		End If



		If mfTplnView IsNot Nothing Then
			mfTplnView.Visible = True
			mfTplnView.SetPgonFilter(iOverlayIndex, bParcel, iTopoID)
		End If
	End Sub

	Private Sub mfTopoActions_FormatChanged() Handles mfTopoActions.FormatChanged
		If mfTplnView IsNot Nothing Then
			mfTplnView.RefreshFormat()
		End If
	End Sub
	Private Sub mfTopoActions_PaintScaleChanged() Handles mfTopoActions.PaintScaleChanged
		If mfTplnView IsNot Nothing Then
			mfTplnView.RefreshPaintScale()
		End If
	End Sub


	Private Sub mfTopoActions_Deactivate(ByVal oSender As System.Object, ByVal e As System.EventArgs)   '''''''''''''''' Handles mfTopoActions.Deactivate
		If mfTopoActions IsNot Nothing AndAlso mfTopoActions.DialogResult = DialogResult.Yes Then
			mfTopoActions.DialogResult = DialogResult.No
			mfTopoActions.Dispose()
			mfTopoActions = Nothing
			Try
				BamashNet.bmBamash.Close()
			Catch oEx As Exception
			End Try
		End If
	End Sub
	Private Shared Function zzGetMPolygon(bEraseSource As Boolean) As MPolygon
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Pline")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult

		Dim bExists As Boolean
		Dim oEnt As DBObject = Nothing



		Dim oMPolygon As MPolygon = New MPolygon()
		Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)

		Do
			oEditor.WriteMessage("Start: " & vbCrLf)
			ptEntRes = oEditor.GetEntity(oPromptOpt)
			If ptEntRes.Status = PromptStatus.OK Then
				Try

					oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)
					oEditor.WriteMessage("oEnt: " & vbCrLf)
				Catch oEx As Exception
					oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
					DMAcadExt.AcadTransaction.Abort()
					DMAcadExt.AcadDocument.Unlock()
					Return Nothing
				End Try
				Select Case oEnt.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadPolylineName
						Dim oPolyline As Polyline = DirectCast(oEnt, Polyline)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrB: " & CStr(oEx.Message))

							Return Nothing
						End Try
					Case DMAcadExt.AcadConst.Acad2dPolylineName
						Dim oPolyline As Polyline2d = DirectCast(oEnt, Polyline2d)
						If oPolyline IsNot Nothing AndAlso Not oPolyline.Closed Then
							oPolyline.Closed = True
						End If

						Try
							oMPolygon.AppendLoopFromBoundary(oPolyline, False, 0.001)
							bExists = True
							colDBObject.Add(oEnt)
						Catch oEx As Exception
							oEditor.WriteMessage("ErrC: " & CStr(oEx.Message))

							Return Nothing
						End Try

				End Select
			Else
				Exit Do
			End If
		Loop
		If bExists Then
			Try
				oMPolygon.BalanceTree()
				If bEraseSource Then
					For Each oDBObject As DBObject In colDBObject
						oDBObject.Erase()
					Next
				End If


				DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
				oMPolygon.Color = Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red)
				DMAcadExt.AcadTransaction.AppendEntity(oMPolygon)
				DMAcadExt.AcadTransaction.CloseModelSpace()


			Catch oEx As Exception
				oEditor.WriteMessage("ErrD: " & CStr(oEx.Message))

				Return Nothing
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("???", "21_223")
		End If
		Return oMPolygon
	End Function

	Private Sub zzArcInfo(ByVal oArc As Arc)
		Dim tCurrentPoint3d As Point3d
		Dim tCurrentPoint As Point2d

		Dim sHandle As String = CStr(oArc.Handle.Value)
		tCurrentPoint3d = oArc.StartPoint

		DMAcadExt.AcadDocument.WriteMessage("A R C   Handle: " & sHandle)
		DMAcadExt.AcadDocument.WriteMessage("Start Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint3d))
		tCurrentPoint3d = oArc.EndPoint
		DMAcadExt.AcadDocument.WriteMessage("End Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint3d))
		DMAcadExt.AcadDocument.WriteMessage("Radius:" & CStr(oArc.Radius))

		Dim oTplnArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oArc)
		tCurrentPoint = oTplnArc.GetMidPoint()
		DMAcadExt.AcadDocument.WriteMessage("Middle Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))

		'   DMAcadExt.AcadDocument.WriteMessage("Start Angle:" & oTplnArc.StartAngle)
		'   DMAcadExt.AcadDocument.WriteMessage("End Angle:" & oTplnArc.EndAngle)
		oTplnArc.PrintInfo()

	End Sub
	Private Shared Function zzGetBlockRef() As BlockReference
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Block:")
		Dim ptEntRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult


		Dim oEnt As DBObject = Nothing




		Dim colDBObject As ICollection(Of DBObject) = New System.Collections.ObjectModel.Collection(Of DBObject)
		Dim oBlockRef As BlockReference = Nothing


		ptEntRes = oEditor.GetEntity(oPromptOpt)
		If ptEntRes.Status = PromptStatus.OK Then
			Try

				oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptEntRes.ObjectId, OpenMode.ForWrite)

			Catch oEx As Exception
				oEditor.WriteMessage("ErrA: " & CStr(oEx.Message))
				DMAcadExt.AcadTransaction.Abort()
				DMAcadExt.AcadDocument.Unlock()
				Return Nothing
			End Try
			Select Case oEnt.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadBlockRefName
					oBlockRef = DirectCast(oEnt, BlockReference)




			End Select

		End If


		Return oBlockRef
	End Function
	Private Shared Sub zzPolylineInfo(ByVal oPolyline As Polyline, Optional bOrigin As Boolean = False)

		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
		Dim tCurrentPoint As Point2d
		Dim tPointByOrigin As Point2d
		If bOrigin Then
			DMAcadExt.KeyPoint.SetOrigin(DMAcadExt.AcadDocument.GetExtMinPoint())
		End If
		If mbToExcel Then
			DMCommon.Debug.ExcelLog.SetNextValue(0, "Polyline Points:",iVerticesUB)
		Else
			DMAcadExt.AcadDocument.WriteMessage("Polyline Points(" & CStr(iVerticesUB) & "):")
		End If


		For iIndex As Integer = 0 To iVerticesUB
			tCurrentPoint = oPolyline.GetPoint2dAt(iIndex)
			tPointByOrigin = DMAcadExt.KeyPoint.GetPointByOrigin(tCurrentPoint)
			If mbToExcel Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "'" & CStr(iIndex) & ":", tCurrentPoint.X, tCurrentPoint.Y, DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint), " B=", oPolyline.GetBulgeAt(iIndex), tPointByOrigin.X, tPointByOrigin.Y)
			Else
				DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & ":" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint) & "; B=" & oPolyline.GetBulgeAt(iIndex), tPointByOrigin.X, tPointByOrigin.Y)
				If bOrigin Then
					DMAcadExt.AcadDocument.WriteMessage("       " & DMAcadExt.TPlnPoint.DispPoint(DMAcadExt.KeyPoint.GetPointByOrigin(tCurrentPoint)))
				End If
			End If

		Next
	End Sub
	Private Shared Sub zzSegmentInfo(ByVal oPolyline As Polyline, tPoint As Point3d)
		Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
		Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
		Dim iIndex As Integer = Convert.ToInt32(Math.Floor(dPointParameter))
		Dim iNextIndex As Integer = (iIndex + 1) Mod oPolyline.NumberOfVertices
		Dim tNextVertex, tVertex As Point3d
		Dim dNextBulge, dBulge As Double
		Dim sNextBulge, sBulge As String
		' Return
		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
		Dim iSegmentType As SegmentType
		iSegmentType = oPolyline.GetSegmentType(iIndex)
		tVertex = oPolyline.GetPoint3dAt(iIndex)
		dBulge = oPolyline.GetBulgeAt(iIndex)
		If dBulge <> 0.0 Then
			sBulge = ";B=" & FormatNumber(dBulge, 4)
		Else
			sBulge = String.Empty
		End If
		tNextVertex = oPolyline.GetPoint3dAt(iNextIndex)
		dNextBulge = oPolyline.GetBulgeAt(iNextIndex)

		If dNextBulge <> 0.0 Then
			sNextBulge = ";B=" & FormatNumber(dNextBulge, 4)
		Else
			sNextBulge = String.Empty
		End If
		Dim dAngleX As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.XAxis)
		Dim dAngleY As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.YAxis)

		'  DMAcadExt.AcadDocument.WriteMessage("Polyline Points: " & tClosestPointOnCurve.ToString() & "; " & dPointParameter.ToString())
		DMAcadExt.AcadDocument.WriteMessage(vbCrLf & ":" & iSegmentType.ToString() & "; " & zzGetDir(tVertex, tNextVertex) & ", Angle X: " & (180.0 * dAngleX / Math.PI).ToString() & " Angle Y: " & (180.0 * dAngleY / Math.PI).ToString() & vbCrLf & CStr(iIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tVertex) & sBulge & vbCrLf & CStr(iNextIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tNextVertex) & sNextBulge)
	End Sub
	Private Shared Function zzGetDir(dFirst As Double, dLast As Double) As String
		Select Case Math.Sign(dLast - dFirst)
			Case 1
				Return "+"
			Case 0
				Return "="
			Case -1
				Return "-"
			Case Else
				Return String.Empty
		End Select
	End Function
	Private Shared Function zzGetDir(tFirstPoint As Point3d, tLastPoint As Point3d) As String
		Return zzGetDir(tFirstPoint.X, tLastPoint.X) & zzGetDir(tFirstPoint.Y, tLastPoint.Y)
	End Function
	Private Shared Sub zzSegmentInfoold(ByVal oPolyline As Polyline, tPoint As Point3d)
		Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
		Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
		Dim tPrevVertex, tVertex As Point3d
		Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
		Dim iSegmentType As SegmentType
		Dim dParam As Double
		DMAcadExt.AcadDocument.WriteMessage("Polyline Points: " & tClosestPointOnCurve.ToString() & "; " & dPointParameter.ToString())
		For iIndex As Integer = 1 To iVerticesUB
			tVertex = oPolyline.GetPoint3dAt(iIndex)
			dParam = oPolyline.GetParameterAtPoint(tVertex)
			If dPointParameter <dParam Then
				tPrevVertex= oPolyline.GetPoint3dAt(iIndex - 1)
				iSegmentType = oPolyline.GetSegmentType(iIndex - 1)
				DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex - 1) & ":" & iSegmentType.ToString() & "; " & DMAcadExt.TPlnPoint.DispPoint(tPrevVertex) & "<=>" & DMAcadExt.TPlnPoint.DispPoint(tVertex) & "; " & dParam.ToString())
		Exit For
		End If

		Next
	End Sub
	Private Shared Sub zzPolyline2dInfo(ByVal oPolyline As Polyline2d)
		Dim iVerticesUB As Integer = -1
		Dim iIndex As Integer = 0
		Dim tCurrentPoint As Point3d
		Dim tAcObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing
		Dim oColEnum As Collections.IEnumerator = oPolyline.GetEnumerator()
		DMAcadExt.AcadDocument.WriteMessage("Polyline2d Points:")
		Do While oColEnum.MoveNext()
			tAcObjID = DirectCast(oColEnum.Current, ObjectId)
			oDBObj = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForWrite)
			oVertex2d = DirectCast(oDBObj, Vertex2d)
			tCurrentPoint = oVertex2d.Position
			DMAcadExt.AcadDocument.WriteMessage("2d-" & CStr(iIndex) & ":" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint) & "; Bulge= " & CStr(oVertex2d.Bulge))
			iIndex += 1
		Loop
	End Sub
	Private Shared Sub zzLineInfo(ByVal oLine As Line)
		Dim tCurrentPoint As Point3d
		Dim tDelta As Autodesk.AutoCAD.Geometry.Vector3d
		tCurrentPoint = oLine.StartPoint
		DMAcadExt.AcadDocument.WriteMessage("Line")
		DMAcadExt.AcadDocument.WriteMessage("StartPoint:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
		tCurrentPoint = oLine.EndPoint
		DMAcadExt.AcadDocument.WriteMessage("EndPoint:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
		tDelta = oLine.Delta
		DMAcadExt.AcadDocument.WriteMessage("Delta:" & tDelta.ToString() & " Tg=" & CStr(tDelta.Y / tDelta.X))

	End Sub
	Private Shared Sub zzPointInfo(ByVal oDBPoint As DBPoint)
		Dim tCurrentPoint As Point3d = oDBPoint.Position
		DMAcadExt.AcadDocument.WriteMessage("Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
	End Sub
	Private Shared Sub zzBlockRefInfo(ByVal oBlockRef As BlockReference)
		Dim tCurrentPoint As Point3d = oBlockRef.Position
		Dim oAttribRef As AttributeReference
		DMAcadExt.AcadDocument.WriteMessage("Insert Point:" & DMAcadExt.TPlnPoint.DispPoint(tCurrentPoint))
		For Each tAttrRefObjID As ObjectId In oBlockRef.AttributeCollection
			oAttribRef = DMAcadExt.AcadTransaction.GetAttribRef(tAttrRefObjID, OpenMode.ForRead)
			DMAcadExt.AcadDocument.WriteMessage(oAttribRef.Tag & ": " & oAttribRef.Position.ToString & ", " & oAttribRef.AlignmentPoint.ToString())
		Next
	End Sub
	Private Shared Sub zzHatchInfo(ByVal oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch)
		Dim oHatchLoop As HatchLoop
		Dim iHatchLoopType As HatchLoopTypes
		Dim colCurves As Autodesk.AutoCAD.Geometry.Curve2dCollection
		Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.BulgeVertexCollection
		Dim iCurvesCount As Integer
		Dim iBulgeVertexCount As Integer
		Dim oCurve As Autodesk.AutoCAD.Geometry.Curve2d


		For iIndex As Integer = 0 To oHatch.NumberOfLoops - 1
			iCurvesCount = -1
			iBulgeVertexCount = -1
			oHatchLoop = oHatch.GetLoopAt(iIndex)
			iHatchLoopType = oHatchLoop.LoopType
			DMAcadExt.AcadDocument.WriteMessage("HatchLoop:" & iHatchLoopType.ToString() & "; IsPline:" & CStr(oHatchLoop.IsPolyline))
			colCurves = oHatchLoop.Curves
			If colCurves IsNot Nothing Then
				iCurvesCount = colCurves.Count
			End If
			oPolyline = oHatchLoop.Polyline
			If oPolyline IsNot Nothing Then
				iBulgeVertexCount = oPolyline.Count
			End If
			DMAcadExt.AcadDocument.WriteMessage("Curves:" & iCurvesCount.ToString() & "; Vertices:" & CStr(iBulgeVertexCount))
			If iCurvesCount > 0 Then
				For iCurveIndex As Integer = 0 To iCurvesCount - 1
					oCurve = colCurves.Item(iCurveIndex)

					DMAcadExt.AcadDocument.WriteMessage("Curve#" & iCurveIndex.ToString() & ": Type:" & oCurve.GetType().ToString())
				Next
			End If
		Next
		' GeoUtilites.BulgeVertexArray(oMPolygonLoop)
	End Sub
	Private Shared Function zzSelectPoint(ByRef tPoint As Point3d, bLockDoc As Boolean) As Boolean
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select Point")
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		Dim bRes As Boolean
		If bLockDoc Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
		End If


		ptRes = oEditor.GetPoint(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = ptRes.Value
				bRes = True
				oEditor.WriteMessage("OK!" & vbCrLf)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "NetApp - TestXData")
			End Try
		Else
			bRes = False
		End If
		If bLockDoc Then
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If

		Return bRes
	End Function

	Private Shared Sub moEditor_PromptedForSelection(oSsender As System.Object, e As PromptSelectionResultEventArgs) Handles moEditor.PromptedForSelection
		Dim sL As String = "P/Selection: "
		If e.Result.Value Is Nothing Then
			moEditor.WriteMessage(sL & e.Result.Status.ToString() & vbCrLf)
		Else
			moEditor.WriteMessage(sL & e.Result.Status.ToString() & "; " & e.Result.Value.Count.ToString() & vbCrLf)
		End If
	End Sub


	Private Shared Sub moEditor_PromptForEntityEnding(oSsender As System.Object, e As PromptForEntityEndingEventArgs) Handles moEditor.PromptForEntityEnding
		Dim sL As String = "P/EntityEnd: "
		moEditor.WriteMessage(sL & e.Result.Status.ToString() & "; " & e.Result.ObjectId.ToString() & "; " & e.Result.PickedPoint.ToString() & vbCrLf)
	End Sub

	Private Shared Sub moEditor_PromptForSelectionEnding(oSsender As System.Object, e As PromptForSelectionEndingEventArgs) Handles moEditor.PromptForSelectionEnding
      Dim sL As String = "P/SelectionEnd: "
      moEditor.WriteMessage(sL & e.Selection.Count.ToString() & "; " & e.Flags.ToString() & "; " & vbCrLf)
   End Sub
   Private Shared Sub zzPrintRes(sL As String, oRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult)

   End Sub


	Private Shared Sub moEditor_PromptingForSelection(oSsender As System.Object, e As PromptSelectionOptionsEventArgs) Handles moEditor.PromptingForSelection
		Dim sL As String = "PromptingSel: "
		Dim oSelFilter As SelectionFilter = e.Filter
		moEditor.WriteMessage(sL & vbCrLf)
		If oSelFilter IsNot Nothing Then
			Dim oaValues() As TypedValue = oSelFilter.GetFilter()

			If oaValues IsNot Nothing Then
				moEditor.WriteMessage(sL & oaValues.GetUpperBound(0) & "; " & vbCrLf)
			End If
		End If

	End Sub

	Private Function zzGetEntityPoints(tAcObjID As ObjectId, ByRef tPointA As Point2d, ByRef tPointB As Point2d) As Boolean
		Dim oDBObject As DBObject
		Dim oBlockRef As BlockReference = Nothing
		Dim oDBPoint As DBPoint = Nothing

		Try
			oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
		Catch oEx As Exception

			Return False
		End Try

		If oDBObject IsNot Nothing Then

			Select Case oDBObject.GetRXClass().Name
				Case DMAcadExt.AcadConst.AcadArcName
					Dim oArc As Arc = DirectCast(oDBObject, Arc)
					tPointA = DMAcadExt.TPlnPoint.Point3dTo2d(oArc.StartPoint)
					tPointB = DMAcadExt.TPlnPoint.Point3dTo2d(oArc.EndPoint)
				Case DMAcadExt.AcadConst.AcadPolylineName
					Dim oPolyline As Polyline = DirectCast(oDBObject, Polyline)
					tPointA = oPolyline.GetPoint2dAt(0)
					tPointB = oPolyline.GetPoint2dAt(oPolyline.NumberOfVertices - 1)

				'	zzSegmentInfo(oPolyline, tPickedPoint)
				Case DMAcadExt.AcadConst.Acad2dPolylineName

				Case DMAcadExt.AcadConst.AcadLineName
					Dim oLine As Line = DirectCast(oDBObject, Line)
					tPointA = DMAcadExt.TPlnPoint.Point3dTo2d(oLine.StartPoint)
					tPointB = DMAcadExt.TPlnPoint.Point3dTo2d(oLine.EndPoint)


				Case DMAcadExt.AcadConst.AcadPointName

					oDBPoint = DirectCast(oDBObject, DBPoint)
					tPointA = DMAcadExt.TPlnPoint.Point3dTo2d(oDBPoint.Position)
					tPointB = Point2d.Origin
				Case DMAcadExt.AcadConst.AcadBlockRefName
					oBlockRef = DirectCast(oDBObject, BlockReference)
					tPointA = DMAcadExt.TPlnPoint.Point3dTo2d(oBlockRef.Position)
					tPointB = Point2d.Origin
				Case Else
					DMAcadExt.AcadDocument.WriteMessage("Entity: " & oDBObject.GetRXClass().Name & ":" & oDBObject.GetType().ToString())
					Return False
			End Select
			Return True
		End If


	End Function
	Private Function zzGetHandleList() As Handle()
		Dim oaHandle(74) As Integer
		Dim tRes(74) As Handle


		Dim l As Long = &H345A
		oaHandle(0) = &H29AF
		oaHandle(1) = &H242A
		oaHandle(2) = &H242B
		oaHandle(3) = &H243D
		oaHandle(4) = &H243E
		oaHandle(5) = &H243F
		oaHandle(6) = &H29B4
		oaHandle(7) = &H2440
		oaHandle(8) = &H29B3
		oaHandle(9) = &H2441
		oaHandle(10) = &H2442
		oaHandle(11) = &H2443
		oaHandle(12) = &H2444
		oaHandle(13) = &H2445
		oaHandle(14) = &H2446
		oaHandle(15) = &H2447
		oaHandle(16) = &H2A1E
		oaHandle(17) = &H29B2
		oaHandle(18) = &H2448
		oaHandle(19) = &H2449
		oaHandle(20) = &H244A
		oaHandle(21) = &H29BD
		oaHandle(22) = &H2320
		oaHandle(23) = &H29C0
		oaHandle(24) = &H2210
		oaHandle(25) = &H221B
		oaHandle(26) = &H29E4
		oaHandle(27) = &H2226
		oaHandle(28) = &H2231
		oaHandle(29) = &H2A0B
		oaHandle(30) = &H2A22
		oaHandle(31) = &H223C
		oaHandle(32) = &H2247
		oaHandle(33) = &H29E1
		oaHandle(34) = &H2252
		oaHandle(35) = &H225D
		oaHandle(36) = &H2268
		oaHandle(37) = &H2273
		oaHandle(38) = &H227E
		oaHandle(39) = &H2289
		oaHandle(40) = &H2294
		oaHandle(41) = &H229F
		oaHandle(42) = &H29DE
		oaHandle(43) = &H22AA
		oaHandle(44) = &H29D5
		oaHandle(45) = &H29D8
		oaHandle(46) = &H29DB
		oaHandle(47) = &H29B5
		oaHandle(48) = &H2A2E
		oaHandle(49) = &H243A
		oaHandle(50) = &H2439
		oaHandle(51) = &H2989
		oaHandle(52) = &H2A2C
		oaHandle(53) = &H2A2B
		oaHandle(54) = &H29B6
		oaHandle(55) = &H242E
		oaHandle(56) = &H2A11
		oaHandle(57) = &H242F
		oaHandle(58) = &H2430
		oaHandle(59) = &H2431
		oaHandle(60) = &H2432
		oaHandle(61) = &H2434
		oaHandle(62) = &H2433
		oaHandle(63) = &H2A25
		oaHandle(64) = &H2A26
		oaHandle(65) = &H2435
		oaHandle(66) = &H2436
		oaHandle(67) = &H2A27
		oaHandle(68) = &H2A28
		oaHandle(69) = &H2A29
		oaHandle(70) = &H2A2A
		oaHandle(71) = &H30BF
		oaHandle(72) = &H244C
		oaHandle(73) = &H244D
		oaHandle(74) = &H29B1

		For iIndex As Integer = 0 To oaHandle.GetUpperBound(0)
			tRes(iIndex) = New Handle(oaHandle(iIndex))
		Next
		Return tRes
	End Function

	Private Class Counter
      Public ID As Integer
      Public Number As Integer = 0
      Public Sub New(iID As Integer)
         ID = iID
      End Sub
      Public Sub AddOne()
         Number += 1
      End Sub
   End Class
End Class


