Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.Geometry
Imports DMAcadExt
Public Class SimplePgon

	Protected doBoundingBox As DMAcadExt.TPlnBoundingBox
	Protected dcolLines As ObjectIdCollection
   Protected dtCentroidAcObjID As ObjectId
   Protected diAddBlockAcObjID As ObjectId
   Protected dtBorderAcObjID As ObjectId
   Protected dtPolylineAcObjID As ObjectId
   Protected dtExteriorHandle As Handle

   Protected dsPgonTopoName As String  ' only name of pgon topology
   Protected dsCurrentPgonTopoName As String  ' name of topology exists
   Protected dsUniqueTag As String
   Protected diUniqueNum As Integer

   Protected dsZebraBoxTopoName As String
   Protected dsZebraPgonTopoName As String
   Protected doaBulgeVertexArray() As GeoUtilites.BulgeVertexArray

   Protected doExternalLoop As HatchLoop
   Protected doInnerLoops() As HatchLoop = Nothing
   Protected doCentroidBlock As Integer
   Protected dtExternalCenter As Point3d
   Protected ddCentroidX As Double
   Protected ddCentroidY As Double
   Protected Const dsPgonPrefix As String = "Pg"
   Protected Const dsZebraBoxPrefix As String = "Zeb"
   Protected Const dsZebraPgonPrefix As String = "Pnt"

   Private mbHasAddPolyline As Boolean = False
   Public Sub SetTagNum(ByVal sTag As String, ByVal iNum As Integer)
      dsUniqueTag = sTag
      diUniqueNum = iNum
      dsPgonTopoName = dsPgonPrefix & zzGetTagNum()
      dsZebraBoxTopoName = dsZebraBoxPrefix & zzGetTagNum()
      dsZebraPgonTopoName = dsZebraPgonPrefix & zzGetTagNum()
   End Sub
   Public ReadOnly Property Coordinates() As String
      Get
         Return CStr(ddCentroidX) & "," & CStr(ddCentroidY)
      End Get
   End Property
   Public ReadOnly Property OutsideVertexUB() As Integer
      Get
         Return doaBulgeVertexArray(0).UB - 1
      End Get
   End Property
   Public Property Lines As ObjectIdCollection
      Get
         Return dcolLines
      End Get
      Set(colValue As ObjectIdCollection)
         dcolLines = colValue
      End Set
   End Property
   Public Property ExteriorHandle As Handle
      Get
         Return dtExteriorHandle
      End Get
      Set(tValue As Handle)
         dtExteriorHandle = tValue
      End Set
   End Property
   Public Sub AddCentroid(oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference)
      Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d = oBlockRef.Position
      '	System.Windows.Forms.MessageBox.Show(tPoint.ToString(), "01_528a")
      ddCentroidX = tPoint.X
      ddCentroidY = tPoint.Y
      dtCentroidAcObjID = oBlockRef.ObjectId
   End Sub
	Protected Function PaintZebra(ByVal tZebra As DMAcadExt.ColorZebra) As PaintException
		'DMCommon.Debug.MsgBox("13_125c", tZebra.StripUB, tZebra.Strip(0).Color.AcadColorIndex, tZebra.Strip(0).Width, tZebra.Strip(0).Scale, tZebra.Strip(0).ScalingWidth, tZebra.Strip(1).Color.AcadColorIndex, tZebra.Strip(1).Width, tZebra.Strip(1).Scale, tZebra.Strip(1).ScalingWidth)
		AcadDocument.WriteLog("-**Zebra - " & AcadTransaction.GetNewBlockName(), True, 0)
		'	DMAcadExt.AcadDocument.WriteDebugMessage("^728 " & CStr(doBoundingBox Is Nothing))
		If doBoundingBox Is Nothing Then
			AcadDocument.WriteMessageLog("PolygonExtentsNotExists")
			Return PaintException.PolygonExtentsNotExists
		End If
		'	DMAcadExt.AcadDocument.WriteDebugMessage("^729 " & CStr(doBoundingBox Is Nothing))

		Dim colZebraLines As ObjectIdCollection
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		Dim colCenterSourcePgon As ObjectIdCollection = New ObjectIdCollection()
		Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim sLayer As String = AcadTransaction.GetCurrentLayer()
		Dim iProcRes As PaintException = PaintException.OK
		'   DMAcadExt.AcadDocument.WriteDebugMessage("^730 " & CStr(doBoundingBox Is Nothing))
		If (doBoundingBox Is Nothing) OrElse doBoundingBox.IsEmpty OrElse tZebra.IsEmpty Then
			'DMCommon.Debug.MsgBox("!Paint_5a", "!doBoundingBox--")
			Dim sMsgAdd As String = ""
			If doBoundingBox Is Nothing Then
				sMsgAdd = "BB is Nothing"
			ElseIf doBoundingBox.IsEmpty Then
				sMsgAdd = "BB is Empty"
			End If
			If tZebra.IsEmpty Then
				sMsgAdd &= ", " & "tZebra Is Empty"
			End If
			DMAcadExt.AcadDocument.WriteMessageLog("Bad Input! " & sMsgAdd)
			AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.BadInput, True)
			Return PaintException.BadInput
		Else
			'	DMCommon.Debug.MsgBox("!Paint_5b", "!doBoundingBox ++")
		End If


		'   DMAcadExt.AcadDocument.WriteDebugMessage("!BoundBox=" & doBoundingBox.Coordinates)
		'AcadDocument.WriteLog("-**Label 1 " & doBoundingBox.AcGePoint(False, False).ToString() & "; " & doBoundingBox.Width() & "; " & doBoundingBox.Height(), True, 0)
		Dim oZebraBox As ZebraBox = New ZebraBox(doBoundingBox, tZebra, 0.05)
		'	Dim sPgonTopoName As String = TopoManager.TopoCreator.GetTopologyName(dsPgonTopoName)
		''''//Create Topology of Polygon

		'AcadDocument.WriteLog("-**Label 2", True, 0)
		'	Return PaintException.OK
		If (Me.dsPgonTopoName IsNot Nothing) AndAlso (Me.dsPgonTopoName.Length <> 0) Then
			' DMAcadExt.AcadDocument.WriteDebugMessage("^732 ")
			'DMCommon.Debug.MsgBox("!Paint_6", "!" & dsPgonTopoName)
			If zzCreatePgonTopology() <> PaintException.OK Then
				DMAcadExt.AcadDocument.WriteMessageLog("CreatingPgonTopologyForZebraFailed; " & dsCurrentPgonTopoName)
				AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.CreatingPgonTopologyForZebraFailed, True)
				Return PaintException.CreatingPgonTopologyForZebraFailed
			End If
		Else

			AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.BadInput, True)
			Return PaintException.BadInput
		End If
		AcadDocument.WriteLog("-**Label 3", True, 0)
		''''//ZebraBox
		'	DMCommon.Debug.MsgBox("13_129c")
		oZebraBox.DrawLines()
		'	DMCommon.Debug.MsgBox("13_129d")
		AcadDocument.WriteLog("-**Label 4", True, 0)
		colZebraLines = oZebraBox.Lines
		'DMCommon.Debug.MsgBox("13_129f", colZebraLines.Count)
		Dim sZebraBoxTopoName As String = TopoManager.TopoCreator.GetTopologyName(dsZebraBoxTopoName)
		''''//Create Topology   ZebraBox
		'DMCommon.Debug.MsgBox("13_129g", dsZebraBoxTopoName, sZebraBoxTopoName)
		AcadDocument.WriteLog("-**Label 5 " & sZebraBoxTopoName, True, 0)
		Try
			oTopos.Create(sZebraBoxTopoName, colZebraLines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "TplnTopoPgon - zzPaintZebra_10 " & sZebraBoxTopoName)
			DMAcadExt.AcadDocument.WriteMessageLog("Creation topology ZebraBox Is failed. Lines =" & CStr(colZebraLines.Count))
			oZebraBox.DeleteLines()
			iProcRes = PaintException.CreatingZebraBoxFailed
		End Try
		'DMCommon.Debug.MsgBox("13_129h", dsZebraBoxTopoName, sZebraBoxTopoName)
		AcadDocument.WriteLog("-**Label 6 ", True, 0)
		''''//Prepare  Topology    
		Dim oZebraBoxTopoModel As TopologyModel = Nothing
		Dim oPgonTopoModel As TopologyModel = Nothing
		Dim oZebraPgonTopoModel As TopologyModel = Nothing
		Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(sLayer, 0)
		Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(sLayer, 0, True, String.Empty)
		Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings(sLayer, 0, False, String.Empty)
		Dim tCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oTopoPgon As Autodesk.Gis.Map.Topology.Polygon
		Dim colZebraPolygons As Autodesk.Gis.Map.Topology.PolygonCollection
		Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection

		Dim iTopoPgonID As Integer = 0
		AcadDocument.WriteLog("-**Label 7 " & iProcRes & iProcRes.ToString(), True, 0)
		If iProcRes = PaintException.OK Then
			oPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(dsCurrentPgonTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oPgonTopoModel IsNot Nothing Then
				colPolygons = oPgonTopoModel.GetPolygons()
				colCenterSourcePgon = New ObjectIdCollection()
				For Each oPgon As Polygon In colPolygons
					''''''''''''  decem colCentrSourcePgon.Add(oPgon.Entity)
				Next
				'	For iIndex As Integer = 0 To colPolygons.Count - 1

				'	Next
				If colPolygons.Count > 1 Then ' there are islands
					Dim oPolygon As Polygon = Nothing
					Try
						oPolygon = oPgonTopoModel.FindPolygon(New Point3d(Me.ddCentroidX, Me.ddCentroidY, 0.0))
					Catch oMapEx As Autodesk.Gis.Map.MapException
						If oMapEx.ErrorCode <> 3 Then
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "SimplePgon - PaintZebra_13")
						End If
						iProcRes = PaintException.FindPgonError
					End Try
					If oPolygon IsNot Nothing Then
						iTopoPgonID = oPolygon.ID
					End If
				End If 'oPgonTopoModel.GetPolygons().Count > 1
				colPolygons.Dispose() ''''''''''''''''????
			Else
				AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.CreatingPgonTopologyForZebraFailed, True)
				DMAcadExt.AcadDocument.WriteMessageLog("Topology(Zebra) " & dsCurrentPgonTopoName & " was Not found")
				iProcRes = PaintException.CreatingPgonTopologyForZebraFailed
			End If
		End If

		AcadDocument.WriteLog("-**Label 8 " & iProcRes.ToString(), True, 0)
		''''//"Painting" ZebraBox
		If iProcRes = PaintException.OK Then
			oZebraBoxTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sZebraBoxTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oZebraBoxTopoModel IsNot Nothing Then
				For iIndex As Integer = 0 To oZebraBox.PolygonUB
					tCenterPoint = oZebraBox.CenterPoint(iIndex)
					oTopoPgon = oZebraBoxTopoModel.FindPolygon(tCenterPoint)
					If oTopoPgon IsNot Nothing Then
						oZebraBox.PgonTopoID(iIndex) = oTopoPgon.ID
					Else
						MessageBox.Show("FindPolygon Not " & CStr(iIndex) & ":" & DMAcadExt.TPlnPoint.DispPoint(tCenterPoint), "12_982 CenterPoint")
					End If
					oTopoPgon.Dispose()   '????
				Next
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("Topology " & sZebraBoxTopoName & " was not found")
				iProcRes = PaintException.CreatingPgonTopologyForZebraFailed
			End If
		End If

		''''//Prepare Intersect
		Dim sZebraPgonTopoName As String = TopoManager.TopoCreator.GetTopologyName(dsZebraPgonTopoName)
		AcadDocument.WriteLog("-**Label 9 " & sZebraPgonTopoName, True, 0)
		If iProcRes = PaintException.OK Then
			Try
				oPgonTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
				oPgonTopoModel.SetEdgeCreationSettings(oEdgeCreationSettings)
				oPgonTopoModel.SetNodeCreationSettings(oNodeCreationSettings)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_17a")
			End Try
			''''//Prepare Intersect Polygon And ZebraBox
			AcadDocument.WriteLog("-**Label 10 " & sZebraPgonTopoName, True, 0)
			Try
				oPgonTopoModel.Intersect(oZebraBoxTopoModel, sZebraPgonTopoName, String.Empty, tResultODTable)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_18", sZebraPgonTopoName)
				iProcRes = PaintException.CreatingIntersectTopologyForZebraFailed
			End Try
		End If
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		If iProcRes = PaintException.OK Then
			oZebraPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sZebraPgonTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oZebraPgonTopoModel IsNot Nothing Then
				Dim oColorPgon As ColorPolygon
				Dim oBaseTopoPgon As Autodesk.Gis.Map.Topology.Polygon = Nothing
				Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme()
				Dim iTestIndex As Integer = 0
				colZebraPolygons = oZebraPgonTopoModel.GetPolygons()
				For Each oPolygon As Polygon In colZebraPolygons
					iTestIndex += 1
					oColorPgon = New ColorPolygon(oPolygon)
					tCenterPoint = New Point3d(oColorPgon.CentroidX, oColorPgon.CentroidY, 0.0)
					If iTopoPgonID <> 0 Then
						Try
							oBaseTopoPgon = oPgonTopoModel.FindPolygon(tCenterPoint)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							If oMapEx.ErrorCode <> 3 Then
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "SimplePgon - PaintZebra_13")
							End If
						End Try
					End If

					If (iTopoPgonID = 0) OrElse (oBaseTopoPgon IsNot Nothing AndAlso oBaseTopoPgon.ID = iTopoPgonID) Then   'Or True
						Try
							oTopoPgon = oZebraBoxTopoModel.FindPolygon(tCenterPoint)  'Select Color
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_27")
							oTopoPgon = Nothing
						End Try
						If oTopoPgon IsNot Nothing Then
							tColorScheme.SetBackColor(oZebraBox.GetColor(oTopoPgon.ID))
							iProcRes = oColorPgon.Paint(DMAcadExt.PaintMethod.Hatch, tColorScheme, False)
						End If
					End If
					oPolygon.Dispose()
					oPolygon = Nothing
				Next
				colZebraPolygons = Nothing
				oZebraPgonTopoModel.Close()
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("Topology " & sZebraPgonTopoName & " was not found")
				AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.CreatingIntersectTopologyForZebraFailed, True)
				Return PaintException.CreatingIntersectTopologyForZebraFailed
			End If
		End If

		AcadDocument.WriteLog("-**Label 11 " & sZebraPgonTopoName, True, 0)
		If (oPgonTopoModel IsNot Nothing) AndAlso oPgonTopoModel.Status <> Status.Closed Then
			Try
				colPolygons = oPgonTopoModel.GetPolygons()
				Dim taCentroids(colPolygons.Count - 1) As ObjectId

				For iIndex As Integer = 0 To taCentroids.GetUpperBound(0)
					taCentroids(iIndex) = colPolygons.Item(iIndex).Entity
				Next
				oPgonTopoModel.Close()
				If iTopoPgonID >= 0 Then
					oTopos.Delete(oPgonTopoModel.Name, False) '  mbHasAddPolyline  ''''''''' 15/08/19
					''''''''  ????  oTopos.Delete(oPgonTopoModel.Name, True) '  mbHasAddPolyline


				End If
				If colCenterSourcePgon IsNot Nothing Then
					DMAcadExt.AcadTransaction.EraseDBObjects(colCenterSourcePgon)
				End If

			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_21")
			End Try
		End If '
		If colZebraLines IsNot Nothing Then
			''''''''''''''''  MessageBox.Show(colZebraLines.Count.ToString() & ":" & diUniqueNum.ToString(), "20_206")
		End If

		If (oZebraBoxTopoModel IsNot Nothing) AndAlso (oZebraBoxTopoModel.Status <> Status.Closed) Then
			Try
				oZebraBoxTopoModel.Close()
				oTopos.Delete(oZebraBoxTopoModel.Name, True)  ''''''''''''''''''''	15/08/19 
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_22")
			End Try
		End If '

		Dim sX As String
		AcadDocument.WriteLog("-**Label 13 ", True, 0)
		Try
			If oZebraPgonTopoModel IsNot Nothing Then
				sX = oZebraPgonTopoModel.Name
			Else
				sX = "oZebraPgonTopoModel Is Nothing"
			End If
			'AcadDocument.WriteMessageLog(CStr(diUniqueNum) & " -Attention ")
			''''''''''''''''     MessageBox.Show(sX & vbCrLf & iTopoPgonID.ToString(), "11_340")
			If oZebraPgonTopoModel IsNot Nothing AndAlso iTopoPgonID >= 0 Then
				oTopos.Delete(oZebraPgonTopoModel.Name, True)   ''''''''''''''15/08/19
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("Problem of rings")
			End If

			'AcadDocument.WriteMessageLog(CStr(diUniqueNum) & " -OK ")
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzPaintZebra_27")
		End Try

		DMAcadExt.AcadDocument.WriteLog("Painting Pgon #" & CStr(diUniqueNum) & " (Zebra) is finished " & iProcRes.ToString(), True, 0)
		If iProcRes <> PaintException.OK Then
			DMAcadExt.AcadDocument.WriteMessageLog("Centroid Insert: " & Coordinates)
			AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", iProcRes, True)
		End If

		Return iProcRes
	End Function
	Public ReadOnly Property CenterPosition As TPlnPoint
      Get
         Return New TPlnPoint(ddCentroidX, ddCentroidY)
      End Get
   End Property

   Public Function PaintColorScheme(ByVal iPaintMethod As PaintMethod, ByVal tColorScheme As ColorScheme, ByVal bOpenBlock As Boolean, Optional ByVal sBlockLayer As String = "", Optional ByVal bRecursion As Boolean = False) As PaintException
      Dim iRes As PaintException = PaintException.OK
      Dim iLocRes As PaintException = PaintException.OK
      Dim iIncrement As Integer
		'DMAcadExt.AcadDocument.WriteMessage("^711 " & CStr(tColorScheme.Scale))
		Dim sBlockName As String
		'DMCommon.Debug.MsgBox("!Paint_2", iPaintMethod, tColorScheme.HasHatch, tColorScheme.HasZebra)
		If bOpenBlock Then
         sBlockName = DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
      Else
         sBlockName = DMAcadExt.AcadTransaction.GetNewBlockName()
      End If
      If bRecursion Then
         iIncrement = 0
      Else
         iIncrement = 0
      End If
      AcadDocument.WriteLog("Paint polygon #" & CStr(diUniqueNum) & " ColorScheme #" & CStr(tColorScheme.ID) & " Block " & sBlockName, True, iIncrement)


		If tColorScheme.HasRecursionBorder Then
         iRes = Me.PaintBorder(iPaintMethod, tColorScheme, False)
      Else
			If tColorScheme.HasHatch Then
				iRes = Me.PaintSolid(iPaintMethod, tColorScheme.Hatch)

			End If
			'DMCommon.Debug.MsgBox("!Paint_3", tColorScheme.HasZebra)
			If tColorScheme.HasZebra Then
				Dim tStrip As DMAcadExt.ColorStrip
				For i As Integer = 0 To tColorScheme.Zebra.StripUB
					tStrip = tColorScheme.Zebra.Strip(i)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "2_Strip", tStrip.Width, tStrip.Color.AcadColor)
				Next
				iLocRes = Me.PaintZebra(tColorScheme.Zebra)
			End If
			If tColorScheme.HasHatch Then
				iRes = Me.PaintHatchPattern(iPaintMethod, tColorScheme.Hatch)
			End If
		End If
      If bOpenBlock Then
         DMAcadExt.AcadTransaction.InsertNewBlock(True, sBlockLayer)
      End If
      If iLocRes = PaintException.OK Then
         Return iRes
      Else
         DMAcadExt.AcadDocument.WriteMessageLog("Temp#02; " & iLocRes.ToString())
         Return iLocRes
      End If

   End Function
   Public Sub CreateClosedPolygon(bExteriorRingOnly As Boolean)
      If doaBulgeVertexArray IsNot Nothing Then
         Dim iRingsUB As Integer
         If bExteriorRingOnly Then
            iRingsUB = 0
         Else
            iRingsUB = doaBulgeVertexArray.GetUpperBound(0)

         End If
         For iIndex As Integer = 0 To iRingsUB
            doaBulgeVertexArray(iIndex).Complete()
            doaBulgeVertexArray(iIndex).CreateDBPolyline(True)
         Next
      End If
   End Sub
   Public Function GetCentroid() As TPlnPoint
      Return New TPlnPoint(Me.ddCentroidX, Me.ddCentroidY)
   End Function
   Protected Function PaintSolid(ByVal iPaintMethod As PaintMethod, ByVal tHatch As DMHatch) As PaintException
      Dim iRes As PaintException
      If Not tHatch.BackColor.IsEmpty Then
         iRes = zzAddHatch(True, tHatch)
      End If
      Return iRes
   End Function
   Protected Function PaintHatchPattern(ByVal iPaintMethod As PaintMethod, ByVal tHatch As DMHatch) As PaintException
      Dim iRes As PaintException
      If Not tHatch.SolidOnly Then
         iRes = zzAddHatch(False, tHatch)
      Else
         iRes = PaintException.OK
      End If
      Return iRes
   End Function
   Protected Function PaintHatch(ByVal iPaintMethod As PaintMethod, ByVal tHatch As DMHatch) As PaintException
      Dim iRes, iResA As PaintException
      If Not tHatch.BackColor.IsEmpty Then
         iRes = zzAddHatch(True, tHatch)
      Else
         iRes = PaintException.HatchBackColorIsEmpty
      End If
      If Not tHatch.SolidOnly Then
         iResA = zzAddHatch(False, tHatch)
      Else
         iResA = PaintException.OK
      End If
      Return AppMessages.GetPaintResp(iRes, iResA)
   End Function

   Private Function zzAddHatch(ByVal bSolid As Boolean, ByVal tHatch As DMHatch) As PaintException
      Dim sTest As String = "a"
      'Dim tAcobjId As ObjectId
      Dim oHatch As Hatch = New Hatch()
      Dim oPatternDefinition As PatternDefinition
      Dim iRes As PaintException = PaintException.OK
      Try
         With oHatch
            .Associative = False
				.HatchStyle = HatchStyle.Normal

				If bSolid Then
               .ColorIndex = tHatch.BackColor.AcadColorIndex
               .SetHatchPattern(HatchPatternType.PreDefined, DMHatch.SolidHatchName) '
               '	MessageBox.Show(.PatternName & ":" & CStr(.PatternAngle) & ":" & CStr(.PatternScale) & ":" & CStr(.LineWeight), "15_311")

            Else
               .ColorIndex = tHatch.PatternColor.AcadColorIndex
               'System.Windows.Forms.MessageBox.Show(tHatch.Angle.AngleRad.ToString() & vbCrLf & CStr(.PatternAngle), "12_100")
               Try
                  If tHatch.Angle.AngleRad <> 0.0 Then
                     .PatternAngle = tHatch.Angle.AngleRad
                  End If

               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  '	System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sTest & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "zzAddHatch_123acad")
               End Try

               '      MessageBox.Show("'" & .PatternName & "':" & CStr(.PatternAngle) & ":" & CStr(.PatternScale) & ":" & CStr(.LineWeight) & vbCrLf & "Solid: " & bSolid.ToString() & vbCrLf & .LineWeight.ToString(), "15_310")


               Try
                  sTest = "bn"

						.LineWeight = tHatch.LineWeight
						sTest = "bp"
						'	AcadDocument.WriteMessage("Before PatternScale=" & CStr(tHatch.ScalingPatternScale) & ";" & DMHatch.PatternUnit.ToString())
						'	MessageBox.Show("Before PatternScale=" & CStr(tHatch.ScalingPatternScale) & ";" & DMHatch.PatternUnit.ToString() & vbCrLf & CStr(tHatch.Scale), "01_853")
						.PatternScale = tHatch.ScalingPatternScale
						sTest = "bq"
						.SetHatchPattern(HatchPatternType.PreDefined, tHatch.PatternName)
                  '	oHatch.ResetScaleDependentProperties()
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

						System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sTest & vbCrLf & oAcadEx.Source & vbCrLf &
																		 oAcadEx.StackTrace & vbCrLf & tHatch.ScalingPatternScale.ToString() & vbCrLf & tHatch.PatternName.ToString() & vbCrLf & tHatch.LineWeight.ToString(), "zzAddHatch_254acad")
					End Try
               Try

                  '	If DMHatch.PatternUnit = DMHatch.HatchUnit.NotDefined Then
                  If oHatch.NumberOfPatternDefinitions > 0 Then
                     oPatternDefinition = oHatch.GetPatternDefinitionAt(0)
                     If oHatch.PatternSpace > 0.000001 Then
                        '	AcadDocument.WriteMessage("Before PatternDefinition=" & CStr(oPatternDefinition.OffsetX) & ":" & CStr(oPatternDefinition.OffsetY) & ":" & oHatch.PatternSpace)

                        DMHatch.SetHatchUnit(oPatternDefinition, oHatch.PatternSpace)
                        '	AcadDocument.WriteMessage("Scale=" & CStr(.PatternScale) & ":" & CStr(tHatch.ScalingPatternScale))
                        If DMHatch.PatternUnit = DMHatch.HatchUnit.Inch Then
                           .PatternScale = tHatch.ScalingPatternScale
                        End If

                     End If
                     '	AcadDocument.WriteMessage("After PatternDefinition=" & CStr(oPatternDefinition.OffsetX) & ":" & CStr(oPatternDefinition.OffsetY) & ":" & oHatch.PatternSpace)
                     'AcadDocument.WriteMessage("PatternDefinitionII=" & CStr(oPatternDefinition.BaseX) & ":" & CStr(oPatternDefinition.BaseY) & ":" & CStr(oPatternDefinition.Angle))
                  End If
                  '	End If

               Catch oEx As Exception
                  MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "26_897")
               End Try
            End If
         End With

         sTest = "b"
         '		AcadDocument.WriteMessage("In Paint - BulgeVertexArrayUB= " & CStr(doaBulgeVertexArray.GetUpperBound(0)))
         Me.zzCalcExternalLoop()
         Me.zzCalcInnerLoops()

         If doExternalLoop IsNot Nothing Then
            Try
               sTest = "gbb"
               oHatch.AppendLoop(doExternalLoop)
               sTest = "gbbc"
               If doInnerLoops IsNot Nothing Then
                  sTest = "gbbd"
                  For iIndex As Integer = 0 To doInnerLoops.GetUpperBound(0)
                     sTest = "gbbx" & CStr(iIndex)
                     oHatch.AppendLoop(doInnerLoops(iIndex))
                     sTest = "gbby" & CStr(iIndex)
                  Next
               End If
               sTest = "gc"
            Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
               System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "zzAddHatch=277x")
            End Try
            sTest = "bo"
            oHatch.EvaluateHatch(True)


            sTest = "bp"

            DMAcadExt.AcadTransaction.AddToNewBlock(oHatch)
            sTest = "be"
            '''''''''''''	tAcobjId = DMAcadExt.AcadTransaction.AppendEntity(oHatch, True)
            Try
               '	oHatch.UpgradeOpen()
               sTest = "bm"
            Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
               AcadErrCode.ShowAcadError(oAcadEx, False, "SimplePgon")
            End Try
         Else
            Return PaintException.ExternalLoopIsNothing
         End If
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "zzAddHatch_15")
         Return PaintException.AddHatchError
      End Try
      Return PaintException.OK
   End Function
	Protected Function PaintBorder(ByVal iPaintMethod As PaintMethod, ByVal tColorScheme As ColorScheme, ByVal bTopologyExists As Boolean) As PaintException
		'id=3971
		'	If (iPaintMethod And PaintMethod.BorderByBuffer) = PaintMethod.BorderByBuffer Then
		'	AcadDocument.WriteMessage("&&&Start of Paint of Border")
		Return Me.zzAddHatchBorderByBuffer(tColorScheme)
		'	AcadDocument.WriteMessage("&3&&End of BorderPaint")
		'	End If
	End Function
	Private Function zzGetTagNum() As String
      Return dsUniqueTag & CStr(diUniqueNum)
   End Function
   Protected Shared Function GetHatch(ByVal bSolid As Boolean, ByVal tHatch As DMHatch) As Hatch
      Dim oHatch As Hatch = New Hatch()
      Dim oPatternDefinition As PatternDefinition
      Try
         With oHatch
            .Associative = False
            .HatchStyle = HatchStyle.Normal
            If bSolid Then
               .ColorIndex = tHatch.BackColor.AcadColorIndex
               .SetHatchPattern(HatchPatternType.PreDefined, DMHatch.SolidHatchName) '
            Else
               .ColorIndex = tHatch.PatternColor.AcadColorIndex
               .PatternAngle = tHatch.Angle.AngleRad
               .LineWeight = tHatch.LineWeight
               .PatternScale = tHatch.ScalingPatternScale
               .SetHatchPattern(HatchPatternType.PreDefined, tHatch.PatternName)
               Try
                  If oHatch.NumberOfPatternDefinitions > 0 Then
                     oPatternDefinition = oHatch.GetPatternDefinitionAt(0)
                     If oHatch.PatternSpace > 0.000001 Then
                        DMHatch.SetHatchUnit(oPatternDefinition, oHatch.PatternSpace)
                        If DMHatch.PatternUnit = DMHatch.HatchUnit.Inch Then
                           .PatternScale = tHatch.ScalingPatternScale
                        End If
                     End If
                  End If
               Catch oEx As Exception
                  MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "26_893")
               End Try
            End If
         End With
      Catch oEx As Exception

      End Try
      Return oHatch
   End Function
   Private Function zzAddHatchBorderByBuffer(ByVal tColorScheme As ColorScheme) As PaintException
      AcadDocument.WriteLog("-**AddHatchBorderByBuffer - " & AcadTransaction.GetNewBlockName(), True, 0)
      Dim tBorder As ColorBorder = tColorScheme.Border
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim colNodes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
      Dim colCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New ObjectIdCollection()
      Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
      Dim bTopologyExists As Boolean = True
      Dim sLayer As String = AcadTransaction.GetCurrentLayer()
      Dim iProcRes As PaintException = PaintException.OK
      Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(sLayer, 0)
      Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(sLayer, 0, True, String.Empty)
      Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings(sLayer, 0, False, String.Empty)

      If (Me.dsPgonTopoName IsNot Nothing) AndAlso (Me.dsPgonTopoName.Length <> 0) Then
         If zzCreatePgonTopology() <> PaintException.OK Then
            AcadDocument.WriteMessageLog("4_082a")
            Return PaintException.CreatingPolygonTopologyFailed
         End If
         bTopologyExists = False  '????????????
      End If

      Dim tStrip As ColorStrip
      ''''''''''''	tStrip = tBorder.Strip(0)
      tStrip = tBorder.GetRecursionStrip()
      If dsCurrentPgonTopoName Is Nothing Then dsCurrentPgonTopoName = String.Empty
      Dim sBufferPgonTopoName As String = dsPgonTopoName & "_b"
      Dim oPgonTopoModel As TopologyModel = Nothing

      Dim oDMHatch As DMHatch = New DMHatch(tStrip.Color)

      Dim oBorderHatch As Hatch = GetHatch(True, oDMHatch)

      Dim oInnerBorderHatch As Hatch = Nothing
      Dim oBufferPgonTopoModel As TopologyModel = Nothing
      Dim oInnerBufferPgonTopoModel As TopologyModel = Nothing
      Dim iEraseTopoCounter As Integer = 0
      Dim sEraseTopoName As String = String.Empty
      Dim sTest As String = ""

      sBufferPgonTopoName = TopoManager.TopoCreator.GetTopologyName(sBufferPgonTopoName)
      If doaBulgeVertexArray.GetUpperBound(0) > 0 Then
         AcadDocument.WriteMessageLog("Rings=" & CStr(doaBulgeVertexArray.GetUpperBound(0) + 1))
      End If

      Me.zzCalcExternalLoop()
      Me.zzCalcInnerLoops()

      If doExternalLoop IsNot Nothing AndAlso oBorderHatch IsNot Nothing Then
         Try
            oBorderHatch.AppendLoop(doExternalLoop)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(CStr(diUniqueNum) & vbCrLf & oEx.Message & vbCrLf & CStr(oBorderHatch.NumberOfLoops) & " - " & CStr(oBorderHatch Is Nothing), "SimplePgon - zzAddHatchBorderByBuffer_10")
         End Try

         If doInnerLoops IsNot Nothing Then
            oInnerBorderHatch = GetHatch(True, oDMHatch)
            If oInnerBorderHatch IsNot Nothing Then
               For iIndex As Integer = 0 To doInnerLoops.GetUpperBound(0)

                  If doInnerLoops(iIndex) IsNot Nothing Then
                     '	MessageBox.Show(CStr(Me.diUniqueNum) & vbCrLf & CStr(iIndex) & vbCrLf & CStr(doInnerLoops(iIndex).LoopType.ToString()), "02_360")
                     AcadDocument.WriteMessageLog("*8rt " & CStr(Me.diUniqueNum) & vbCrLf & CStr(iIndex) & vbCrLf & CStr(doInnerLoops(iIndex).LoopType.ToString()))
                     Try
                        oInnerBorderHatch.AppendLoop(doInnerLoops(iIndex))
                     Catch oEx As Exception
                        System.Windows.Forms.MessageBox.Show(CStr(diUniqueNum) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(oBorderHatch Is Nothing), "SimplePgon - zzAddHatchBorderByBuffer_11")
                     End Try
                  Else
                     '	System.Windows.Forms.MessageBox.Show(CStr(diUniqueNum) & vbCrLf & "doInnerLoops(" & CStr(iIndex) & ") IsNot Nothing Then" & vbCrLf & CStr(oBorderHatch Is Nothing), "SimplePgon - zzAddHatchBorderByBuffer_11a")
                  End If

               Next
            End If
         Else
            '	MessageBox.Show("doInnerLoops Is Nothing", "19_105n doInnerLoops " & zzGetTagNum())
            'AcadDocument.WriteMessage("*17 doInnerLoops Is Nothing", "19_105n doInnerLoops " & zzGetTagNum())
         End If
      Else
         MessageBox.Show("doExternalLoop Is Nothing", "19_104n doExternalLoop " & zzGetTagNum())
      End If

      Dim colBufferPolygons As Autodesk.Gis.Map.Topology.PolygonCollection
      Dim colInnerBufferPolygons As Autodesk.Gis.Map.Topology.PolygonCollection
      Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection
      Dim oPgon As ColorPolygon
      DMAcadExt.AcadDocument.WriteDebugMessage("#dsCurrentPgonTopoName: " & dsCurrentPgonTopoName)
      oPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(dsCurrentPgonTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
      If oPgonTopoModel IsNot Nothing Then
         Dim sOffset As String = String.Empty
         Try
            sOffset = tBorder.GetOffset(True)
            oPgonTopoModel.Buffer(sOffset, sBufferPgonTopoName)    ', "", 0.5
         Catch oMapEx As Autodesk.Gis.Map.MapException
            AcadDocument.WriteMessageLog("Creating Buffer Is Failed. Offset=" & sOffset & "; " & "TopoName: " & oPgonTopoModel.Name & "; " & ddCentroidX.ToString() & "," & ddCentroidY.ToString() & " BufferName: " & sBufferPgonTopoName & "; ")

            AcadErrCode.ShowMapError(oMapEx, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_3")
            iProcRes = PaintException.CreatingBufferFailed
         End Try
         If iProcRes = PaintException.OK Then

            oBufferPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sBufferPgonTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
            If oBufferPgonTopoModel IsNot Nothing Then
               'zzTestRingInfo(oBufferPgonTopoModel, "Buffer")
               colBufferPolygons = oBufferPgonTopoModel.GetPolygons()    ''''AAAAAAAAAAAAAAA
               '	DMAcadExt.AcadDocument.WriteMessage("Polygons.Count= " & CStr(colBufferPolygons.Count))

               For Each oPolygon As Polygon In colBufferPolygons
                  oPgon = New ColorPolygon(oPolygon)
                  oPgon.zzCalcExternalLoop()
                  If oPgon.doExternalLoop IsNot Nothing Then
                     oBorderHatch.AppendLoop(oPgon.doExternalLoop)
                  Else
                     AcadDocument.WriteMessage("ExternalLoop was not found", "SimplePgon - zzAddHatchBorderByBuffer_12")
                  End If
               Next
               Try
                  oBorderHatch.EvaluateHatch(True)
               Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                  AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "SimplePgon - zzAddHatchBorderByBuffer_21")
               End Try
               DMAcadExt.AcadTransaction.AddToNewBlock(oBorderHatch) ''Paint outmost buffer
               '		AcadDocument.WriteMessageLog("*22rtm " & CStr(Me.diUniqueNum) & ":" & CStr(colBufferPolygons.Count))
               If oInnerBorderHatch IsNot Nothing Then
                  ' Inner rings
                  Dim colRings As RingCollection
                  colPolygons = oPgonTopoModel.GetPolygons()
                  AcadDocument.WriteMessageLog("*25rtm " & CStr(Me.diUniqueNum) & ":" & CStr(colPolygons.Count))
                  Dim oEraseTopoModel As TopologyModel = Nothing
                  Dim iInnerTopoCounter As Integer = 0
                  Dim sInnerTopoName As String
                  '	sOffset = CStr(tBorder.Strip(0).ScalingWidth)
                  sOffset = tBorder.GetOffset(False)
                  For Each oPolygon As Polygon In colPolygons
                     colRings = oPolygon.GetBoundary()
                     '	MessageBox.Show(CStr(colRings.Count), "01_1001")
                     AcadDocument.WriteMessageLog("*30rtp " & CStr(Me.diUniqueNum) & ":" & CStr(colRings.Count))
                     For Each oRing As Ring In colRings
                        If Not oRing.IsExterior Then
                           sInnerTopoName = dsCurrentPgonTopoName & Strings.Chr(65 + iInnerTopoCounter)
                           sInnerTopoName = zzBuildInnerBufferTopo(oRing, sInnerTopoName, sOffset)
                           iInnerTopoCounter += 1
                           ''''''''''''''''' ''''''''
                           oInnerBufferPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sInnerTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
                           If oInnerBufferPgonTopoModel IsNot Nothing Then
                              colInnerBufferPolygons = oInnerBufferPgonTopoModel.GetPolygons()
                              For Each oInnerPolygon As Polygon In colInnerBufferPolygons
                                 oPgon = New ColorPolygon(oInnerPolygon)
                                 oPgon.zzCalcExternalLoop()
                                 If oPgon.doExternalLoop IsNot Nothing Then
                                    oInnerBorderHatch.AppendLoop(oPgon.doExternalLoop)
                                 Else
                                    '	AcadDocument.WriteMessage("ExternalLoop was not found", "SimplePgon - zzAddHatchBorderByBuffer_12")
                                 End If
                              Next

                              'erase inner ring topo from outermost
                              ''''''''''''''''' '''''''''''''''''''
                              ''''''''''''''''' '''''''''''''''' '''
                              If iEraseTopoCounter <> 0 Then
                                 oBufferPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sEraseTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
                              End If
                              If oBufferPgonTopoModel IsNot Nothing Then
                                 sEraseTopoName = dsCurrentPgonTopoName & "_ers" & Strings.Chr(65 + iEraseTopoCounter)
                                 sEraseTopoName = TopoManager.TopoCreator.GetTopologyName(sEraseTopoName)
                                 iEraseTopoCounter += 1
                                 Try
                                    oBufferPgonTopoModel.SetCentroidCreationSettings(oCentroidCreationSettings)
                                    oBufferPgonTopoModel.SetEdgeCreationSettings(oEdgeCreationSettings)
                                    oBufferPgonTopoModel.SetNodeCreationSettings(oNodeCreationSettings)
                                 Catch oMapEx As Autodesk.Gis.Map.MapException
                                    DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "SimplePgon - zzAddHatchBorderByBuffer_17a")
                                 End Try

                                 Try
                                    oBufferPgonTopoModel.Erase(oInnerBufferPgonTopoModel, sEraseTopoName, String.Empty, tResultODTable)
                                 Catch oMapEx As Autodesk.Gis.Map.MapException
                                    DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "SimplePgon - zzAddHatchBorderByBuffer_18 erase")
                                 End Try
                                 oBufferPgonTopoModel.Close()
											oTopos.Delete(oBufferPgonTopoModel.Name, True) '' Comm 18/02
											'oTopos.Delete(oBufferPgonTopoModel.Name, False) ' TEst 18/02
											oBufferPgonTopoModel = Nothing
                              End If
                              Try
                                 oInnerBufferPgonTopoModel.Close()
											oTopos.Delete(oInnerBufferPgonTopoModel.Name, True) ' Comm 18/02
											'''''''	oTopos.Delete(oInnerBufferPgonTopoModel.Name, False) ' TEst 18/02
										Catch oMapEx As Autodesk.Gis.Map.MapException
                                 AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "SimplePgon - zzAddHatchBorderByBuffer_14")
                              End Try
                           Else
                              System.Windows.Forms.MessageBox.Show(CStr(diUniqueNum) & ":" & CStr(dsUniqueTag) & ":" & CStr(tColorScheme.ID), "26_115")
                           End If
                        End If
                     Next
                  Next
                  Try
                     oInnerBorderHatch.EvaluateHatch(True)

                  Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                     AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "SimplePgon - zzAddHatchBorderByBuffer_1")
                  End Try
                  Try
                     DMAcadExt.AcadTransaction.AddToNewBlock(oInnerBorderHatch)  ''Paint Inners buffer
                  Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
                     AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "SimplePgon - zzAddHatchBorderByBuffer_5")
                  End Try

               End If
               tColorScheme.DeleteBorderStrip()
               tColorScheme.NextBorderStrip()
               If iEraseTopoCounter <> 0 Then
                  oBufferPgonTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sEraseTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
               End If

               Dim colNextPolygons As PolygonCollection
               '	zzTestRingInfo(oBufferPgonTopoModel, "Buffer")
               If oBufferPgonTopoModel IsNot Nothing Then
                  colNextPolygons = oBufferPgonTopoModel.GetPolygons()
                  For Each oPolygon As Polygon In colNextPolygons
                     oPgon = New ColorPolygon(oPolygon)
                     oPgon.Paint(PaintMethod.ColorScheme, tColorScheme, False, , True)
                  Next
                  oBufferPgonTopoModel.Close()
						oTopos.Delete(oBufferPgonTopoModel.Name, True)  'comm 8/02
						'	oTopos.Delete(oBufferPgonTopoModel.Name, False) ' TEst 18/02
					End If
            End If

         Else
            '	AcadDocument.WriteMessage("Buffer of Topology '" & dsCurrentPgonTopoName & "' was not found #4")
            AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, dsCurrentPgonTopoName, DMAcadExt.PaintException.CreatingTopologyByBufferFailed, True)
         End If
         If Not bTopologyExists Then
            Try
               oPgonTopoModel.Close()
               oTopos.Delete(oPgonTopoModel.Name, False)
               dsCurrentPgonTopoName = String.Empty
            Catch oMapEx As Autodesk.Gis.Map.MapException
               AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnTopoPgon - zzAddHatchBorderByBuffer_17")
            End Try
         End If
      Else
         AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", DMAcadExt.PaintException.SourcePgonTopologyForBufferNotExists, True)
         '	AcadDocument.WriteMessage("Topology '" & dsCurrentPgonTopoName & "' was not found #3")
      End If

      DMAcadExt.AcadDocument.WriteLog("Painting Pgon #" & CStr(diUniqueNum) & " (Border) is finished " & iProcRes.ToString(), True, 0)
      Return iProcRes
   End Function
   Private Function zzBuildInnerBufferTopo(ByVal oRing As Ring, ByVal sTopoName As String, ByVal sOffset As String) As String
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies

      Dim colLines As ObjectIdCollection = New ObjectIdCollection
      Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim oFullEdge As FullEdge
      Dim tAcObjID As ObjectId
      Dim colHalfEdges As HalfEdgeCollection
      Dim oRingTopoModel As TopologyModel = Nothing
      Dim sResult As String = String.Empty
      colHalfEdges = oRing.GetEdges()
      '	AcadDocument.WriteMessage("colHalfEdges: " & dcolLines.Count.ToString())
      For Each oHEdge As HalfEdge In colHalfEdges
         oFullEdge = oHEdge.FullEdge
         tAcObjID = oFullEdge.Entity
         colLines.Add(tAcObjID)
      Next
      Dim sBufferPgonTopoName As String = sTopoName & "_buf"
      sBufferPgonTopoName = TopoManager.TopoCreator.GetTopologyName(sBufferPgonTopoName)
      sTopoName = TopoManager.TopoCreator.GetTopologyName(sTopoName)
      Try
         oTopos.Create(sTopoName, colLines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
      Catch oMapEx As Autodesk.Gis.Map.MapException
         DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "SimplePgon - zzBuildInnerBufferTopo ")
      End Try
      oRingTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
      If oRingTopoModel IsNot Nothing Then
         Try
            oRingTopoModel.Buffer(sOffset, sBufferPgonTopoName)    ', "", 0.5
            sResult = sBufferPgonTopoName
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadDocument.WriteMessage("SourceTopoName: " & oRingTopoModel.Name & "; Offset=" & sOffset)
            AcadErrCode.ShowMapError(oMapEx, False, "TplnTopoPgon - zzBuildInnerBufferTopo_4")
         End Try
         Try
            oRingTopoModel.Close()
            oTopos.Delete(oRingTopoModel.Name, False) 'True
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "SimplePgon - zzBuildInnerBufferTopo_3 ")
         End Try

      End If
      Return sResult
   End Function

   Protected Sub zzCalcExternalLoop()
		Try

			If doaBulgeVertexArray IsNot Nothing AndAlso doaBulgeVertexArray.GetUpperBound(0) >= 0 Then

				If doaBulgeVertexArray(0) IsNot Nothing Then


					doExternalLoop = doaBulgeVertexArray(0).CreateHatchLoop(HatchLoopTypes.Default Or HatchLoopTypes.Polyline)
				Else
					DMAcadExt.AcadDocument.WriteMessageLog("Polygon is invalid", "SimplePgon - zzCalcExternalLoop_1")
				End If
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("Polygon is invalid (1)", "SimplePgon - zzCalcExternalLoop_2")
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "SimplePgon - zzCalcExternalLoop")
      End Try

   End Sub
   Protected Sub zzCalcInnerLoops()
      Try
         If doaBulgeVertexArray IsNot Nothing Then
            Dim iUB As Integer = doaBulgeVertexArray.GetUpperBound(0)
            If iUB > 0 Then
               ReDim doInnerLoops(iUB - 1)
               For iIndex As Integer = 0 To iUB - 1
                  If doaBulgeVertexArray(iIndex + 1) IsNot Nothing Then
                     doInnerLoops(iIndex) = doaBulgeVertexArray(iIndex + 1).CreateHatchLoop(HatchLoopTypes.Default Or HatchLoopTypes.Polyline)
                     AcadDocument.WriteDebugMessage("InternalLoop #" & CStr(iIndex + 1) & " IsPolyline - " & CStr(doExternalLoop.IsPolyline))
                  Else
                     '	System.Windows.Forms.MessageBox.Show("doaBulgeVertexArray(" & CStr(iIndex + 1) & ") Is Nothing ", "SimplePgon - zzCalcInnerLoops_1")
                  End If

               Next
            End If
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "SimplePgon - zzCalcInnerLoops")
      End Try

   End Sub
   Private Sub zzTestRingInfo(ByVal oPgonTopoModel As TopologyModel, ByVal sCaption As String)
      Dim sMsg As String = ""
      Dim colRings As RingCollection
      If oPgonTopoModel.Status <> Status.Closed Then
         Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oPgonTopoModel.GetPolygons()
         sMsg = "Pgons - " & CStr(colPolygons.Count)
         For Each oPolygon As Polygon In colPolygons
            colRings = oPolygon.GetBoundary()
            sMsg &= "; Rings - " & CStr(colRings.Count) & ";"
            For Each oRing As Ring In colRings
               sMsg &= "IsExt - " & CStr(oRing.IsExterior)
            Next
         Next
      End If
      DMAcadExt.AcadDocument.WriteMessage("TopoInfo " & oPgonTopoModel.Name & "-" & sCaption & ":" & sMsg)
   End Sub
   Private Function zzCreatePgonTopology() As PaintException
      Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
      Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
      Dim bErrMemory As Boolean = False
      Dim bProcRes As PaintException = PaintException.OK
      Dim colPgonLinks As ObjectIdCollection
      If Not dtCentroidAcObjID.IsNull Then
         colCentroids.Add(dtCentroidAcObjID)
      End If
      dsCurrentPgonTopoName = TopoManager.TopoCreator.GetTopologyName(dsPgonTopoName, "_")

      If (dsPgonTopoName Is Nothing) OrElse (dcolLines Is Nothing) Then
         MessageBox.Show(zzGetTagNum() & "-" & CStr(dsPgonTopoName Is Nothing) & ":" & CStr(dcolLines Is Nothing), "19_888")
         bProcRes = PaintException.CreatingPolygonTopologyFailed
         Return bProcRes
      End If
		'DMCommon.Debug.MsgBox("12_760", dtBorderAcObjID, DMCommon.Debug.ColCount(dcolLines), dtBorderAcObjID.IsNull)
		If dtBorderAcObjID.IsNull Then
			colPgonLinks = dcolLines
		Else
			colPgonLinks = New ObjectIdCollection()
			colPgonLinks.Add(dtBorderAcObjID)
		End If
		'ffff
		Try
			AcadTransaction.SetColor(colPgonLinks, Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red))
			Dim oEntity As Entity
			'	DMCommon.Debug.MsgBox("04_219b", dsCurrentPgonTopoName, CStr(dcolLines.Count), CStr(colPgonLinks.Count), CStr(colCentroids.Count), DMAcadExt.AcadTransaction.IsActive, AcadDocument.DocumentLockMode)
			For Each tAcObjID As ObjectId In colPgonLinks
				oEntity = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				AcadDocument.WriteMessageLog("!PgonLinks:" & oEntity.Handle.ToString & vbCrLf)
			Next
			For Each tAcObjID As ObjectId In colCentroids
				oEntity = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				AcadDocument.WriteMessageLog("!PgonCntr:" & oEntity.Handle.ToString & vbCrLf)
			Next

			oTopos.Create(dsCurrentPgonTopoName, colPgonLinks, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors, 1.0)
		Catch oEx As System.Exception
			DMCommon.Debug.MsgBox("!Ex01", oEx.Message, colPgonLinks.Count, DMAcadExt.AcadTransaction.IsActive, AcadDocument.DocumentLockMode)
			AcadTransaction.SetColor(colPgonLinks, Autodesk.AutoCAD.Colors.Color.FromColor(Color.Red), TriState.True)
			'AcadTransaction.Terminate()
			'AcadDocument.Unlock()

			If oEx.GetType().ToString() = "Autodesk.Gis.Map.MapException" Then
				Dim oMapEx As Autodesk.Gis.Map.MapException = DirectCast(oEx, Autodesk.Gis.Map.MapException)
				DMCommon.Debug.MsgBox("!Ex02", oMapEx.ErrorCode, oMapEx.Message, DMAcadExt.AcadTransaction.IsActive, AcadDocument.DocumentLockMode)
				'DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Create Topology")
				'err=2005
				If oMapEx.ErrorCode = 1907 Then
               bErrMemory = True
               For Each tAcObjID As ObjectId In dcolLines
                  AcadDocument.WriteMessageLog("_ " & tAcObjID.ToString(), "SimplePgon")
               Next
            End If
            AcadDocument.WriteMessageLog("Create Pgon Topo is Failed (lines) " & CStr(oMapEx.ErrorCode) & "; " & Me.Coordinates & "; " & colPgonLinks.Count.ToString())
				AcadErrCode.ShowMapError(oMapEx, False, "SimplePgon - zzCreatePgonTopology_map", , "Pgon# " & dsCurrentPgonTopoName & "; " & Me.Coordinates)
				DMAcadExt.AcadTransaction.SetExceptionLayer(colPgonLinks)

			Else
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TopoCreator - CreateTopology_12")
         End If
         Return PaintException.PolygonError
      End Try
      If oTopos.Exists(dsCurrentPgonTopoName) Then

         Return PaintException.OK
      End If

      dsCurrentPgonTopoName = dsPgonTopoName
      Dim tPlineObjID As ObjectId
      Dim colPlines As ObjectIdCollection = New ObjectIdCollection()
      If bErrMemory Then
         Return PaintException.CreatingPolygonTopologyFailed
      End If
      If Not oTopos.Exists(dsCurrentPgonTopoName) AndAlso doaBulgeVertexArray IsNot Nothing Then
         Try
            For iIndex As Integer = 0 To doaBulgeVertexArray.GetUpperBound(0)
               tPlineObjID = doaBulgeVertexArray(iIndex).CreateDBPolyline(True)
               If Not tPlineObjID.IsNull Then
                  colPlines.Add(tPlineObjID)
               End If
            Next
            mbHasAddPolyline = True
         Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
            System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "SimplePgon - zzCreatePgonTopology_3")
         End Try
      Else
         AcadDocument.WriteMessageLog("4_082qq" & dsCurrentPgonTopoName & " - " & CStr(colPlines.Count))
      End If
      'AcadDocument.WriteMessageLog("4_082r" & dsCurrentPgonTopoName & " - " & CStr(oTopos.Exists(dsCurrentPgonTopoName)) & " - " & CStr(doaBulgeVertexArray Is Nothing))
      If colPlines.Count > 0 Then
         '	AcadDocument.WriteMessageLog("4_082ra_" & CStr(colPlines.Count))
         Try
            'If dsCurrentPgonTopoName <> "PgonTopo2884" Then
            oTopos.Create(dsCurrentPgonTopoName, colPlines, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.UsePersistentMarkers Or CreateOptions.HighlightErrors, 0.1)
            'End If
            'MessageBox.Show(dsCurrentPgonTopoName, "OK 19_262")
            Return PaintException.OK
         Catch oMapEx As Autodesk.Gis.Map.MapException
            System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & dsCurrentPgonTopoName & vbCrLf & oMapEx.StackTrace, "SimplePgon - zzCreatePgonTopology_2")
            AcadDocument.WriteMessageLog("Create Pgon Topo is Failed (Points) " & CStr(oMapEx.ErrorCode) & "; " & Me.Coordinates)
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "SimplePgon - zzCreatePgonTopology_2", , dsCurrentPgonTopoName)
            Return PaintException.CreatingPolygonTopologyFailed
         End Try
      Else
         Return PaintException.CreatingPolygonTopologyFailed
      End If

   End Function
	Public Sub New()

	End Sub
	Public Shared ReadOnly Property PgonTopoName(ByVal i As Integer) As String
		Get
			Return String.Empty
		End Get

	End Property
	Public Property PgonTopoName() As String
		Get
			Return dsPgonTopoName
		End Get
		Set(ByVal sValue As String)
			dsPgonTopoName = sValue
		End Set
	End Property
	Public Sub New(ByVal colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection)
		Dim iVertCount As Integer = colPoints.Count
		Dim oPolyline As Polyline = New Polyline(iVertCount)
		Dim tPoint As Point2d
		Dim tAcObjID As ObjectId
		Dim oMinPoint As DMAcadExt.TPlnPoint = Nothing
		Dim oMaxPoint As DMAcadExt.TPlnPoint = Nothing

		Dim sTest As String = "a"

		If iVertCount = 4 Then
			Dim taPoints(3) As Point2d
			Dim oaBulgeVertex(4) As BulgeVertex
			Try
				tPoint = DMAcadExt.TPlnPoint.Point3dTo2d(colPoints.Item(0))
				taPoints(0) = tPoint
				oaBulgeVertex(0) = New BulgeVertex(tPoint, 0)
				oaBulgeVertex(4) = New BulgeVertex(tPoint, 0)
				sTest = "f_" & CStr(tPoint.X) & "," & CStr(tPoint.Y)
				sTest = "b"
				tPoint = DMAcadExt.TPlnPoint.Point3dTo2d(colPoints.Item(1))
				sTest = "c_" & CStr(tPoint.X) & "," & CStr(tPoint.Y)
				taPoints(1) = tPoint
				oaBulgeVertex(1) = New BulgeVertex(tPoint, 0)
				sTest = "d"
				oMaxPoint = New DMAcadExt.TPlnPoint(tPoint)
				sTest = "e"
				sTest = "ga"
				tPoint = DMAcadExt.TPlnPoint.Point3dTo2d(colPoints.Item(3))
				sTest = "h_" & CStr(tPoint.X) & "," & CStr(tPoint.Y)
				taPoints(2) = tPoint
				oaBulgeVertex(2) = New BulgeVertex(tPoint, 0)
				sTest = "i"
				tPoint = DMAcadExt.TPlnPoint.Point3dTo2d(colPoints.Item(2))
				sTest = "k"
				taPoints(3) = tPoint
				oaBulgeVertex(3) = New BulgeVertex(tPoint, 0)
				sTest = "j"
				sTest = "l"
				oMinPoint = New DMAcadExt.TPlnPoint(tPoint)
				For iIndex As Integer = 0 To iVertCount - 1
					sTest = "x_" & CStr(iIndex)
					oPolyline.AddVertexAt(iIndex, taPoints(iIndex), 0.0, 0.0, 0.0)
				Next
				oPolyline.Closed = True
				sTest = "m"
				ReDim doaBulgeVertexArray(0)
				doaBulgeVertexArray(0) = New GeoUtilites.BulgeVertexArray(oaBulgeVertex)

				'	doaBulgeVertexArray(0).LoopInfo()

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "SimplePgon - New")
				Return
			End Try
		End If
		tAcObjID = DMAcadExt.AcadTransaction.AppendEntity(oPolyline, False)
		dcolLines = New ObjectIdCollection
		dcolLines.Add(tAcObjID)
		If oMinPoint IsNot Nothing AndAlso oMaxPoint IsNot Nothing Then
			doBoundingBox = New DMAcadExt.TPlnBoundingBox(oMinPoint, oMaxPoint)
		Else
			System.Windows.Forms.MessageBox.Show("Bounding Box Is Nothing", "SimplePgon - New_2")

		End If


	End Sub


End Class
