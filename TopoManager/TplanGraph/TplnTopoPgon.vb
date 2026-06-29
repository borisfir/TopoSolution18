Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD
Imports DMAcadExt
Imports System
Namespace TPlanGraph
	Public MustInherit Class TplnTopoPgon
		Inherits SimplePgon
		Const msPrefixPgonTopoName As String = "Pgon"
		Const msOffsetTopoName As String = "PgonOff"
		Const msTopologyTag As String = "Topo"
		Const miErrClassNo As Integer = 12000 '  iExNo   = 10
		Protected diTopoID As Integer
		Protected ddAcadArea As Double
		Protected ddPerimeter As Double
		Protected dsTopologyName As String
      Protected dsTopologyTableName As String

		'Protected dtExtents As Extents2d

		Protected dcolBoundaryObj As ObjectIdCollection
		'''''''''''''''  Protected doTopology As TplnTopology
		Protected Shared ddAreaScale As Double = 1.0	 '0.001
      'Protected doPolygon As Autodesk.Gis.Map.Topology.Polygon
		Protected doMPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon
		Protected dbDefineDirection As Boolean
      Protected dbAcadPoint As Boolean


		Protected doLinkTable As ODTable
		'Private mcolRings As RingCollection
		'	Private mbExteriorLeftAAA As Boolean
		''''''''''''''''''''''Private moPgonScheme As TopoScheme.Polygon
		Protected dbCorrect As Boolean = True
      Protected dsaBlockAttribText() As String
      Protected dsaAddBlockAttribText() As String

		Protected Shared dtMapThemeData As DMAcadExt.MapThemeData
		Protected ddgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute
		Private mdicAdjoiningLines As Dictionary(Of Integer, ObjectIdCollection)
		Private mdicNeighbors As Dictionary(Of Integer, Integer)
		'	Protected dtaTypedValues() As TypedValue = Nothing
		Protected doTplnXData As DMAcadExt.TplnXData
		Protected dtPgonHandle As Handle
      Protected MustOverride ReadOnly Property BlockAttribIndex As Integer()
      Protected MustOverride ReadOnly Property AddBlockAttribIndex As Integer()

		Public Shared Sub InitTopo(tMapThemeData As DMAcadExt.MapThemeData)
			dtMapThemeData = tMapThemeData

		End Sub
		Public Shared Function GetCentroidAttribTag() As String()
			Dim sCentroidBlockName As String = dtMapThemeData.CentroidBlocks
			'	System.Windows.Forms.MessageBox.Show(sCentroidBlockName, "01_507")
         Return DMAcadExt.AcadTransaction.GetAttribDef(sCentroidBlockName, True)
		End Function
		Public Sub New(ByVal iTopoID As Integer)
			'	System.Windows.Forms.MessageBox.Show(CStr(iTopoID), "01_513k")
			MyBase.diUniqueNum = iTopoID
			diTopoID = iTopoID
			'	System.Windows.Forms.MessageBox.Show(CStr(iTopoID), "01_513x")

		End Sub
		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon, Optional ByVal bDefineDirection As Boolean = False)
         '		Dim sTest As String = "a"
			Try

            '		doPolygon = oPolygon

				dsTopologyName = oPolygon.Topology.Name

				diTopoID = oPolygon.ID

				dsTopologyTableName = ODTable.GetTableName(dsTopologyName, TopoTableType.Link)

				'''''''''''''''''''TEMP doLinkTable = New ODTable(dsTopologyName, TopoTableType.Link)

				'	DMAcadExt.AcadDocument.WriteMessage("$$Topolog= '" & dsTopologyName & "'; TopoID= " & CStr(diTopoID))
				MyBase.dsPgonTopoName = dsTopologyName & "_" & CStr(diTopoID)

				MyBase.SetTagNum(msTopologyTag, diTopoID)

				ddAcadArea = oPolygon.Area * ddAreaScale
            ddPerimeter = Math.Round(oPolygon.Perimeter, 0)
            ddCentroidX = oPolygon.Centroid.X
				ddCentroidY = oPolygon.Centroid.Y
            dtCentroidAcObjID = oPolygon.Entity

            If dtCentroidAcObjID.IsNull Then
               DMCommon.Debug.MsgBox("09_217s", dtCentroidAcObjID)
            End If

            dbDefineDirection = bDefineDirection

            '''''''''''''''''''''''	Me.SetAttributeOrder()
            zzCalcPolygon(oPolygon)

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "#237 Topology:'" & dsTopologyName & "'; Polygon Id = " & CStr(diTopoID) & "; X,Y = " & CStr(ddCentroidX) & "," & CStr(ddCentroidY))
         End Try
      End Sub
      Public Sub New(oPolyline As Polyline, ByVal iFeatureID As Integer)
         MyBase.diUniqueNum = iFeatureID
         MyBase.SetTagNum(msTopologyTag, diTopoID)

         ddAcadArea = oPolyline.Area
         dtPolylineAcObjID = oPolyline.ObjectId
         dtExteriorHandle = oPolyline.Handle

         diTopoID = iFeatureID
         ddCentroidX = oPolyline.GetPoint2dAt(0).X
         ddCentroidY = oPolyline.GetPoint2dAt(0).Y
         doBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolyline.GeometricExtents)
      End Sub
      Public Sub New(oPolygon As TopoScheme.tsPolygon, Optional ByVal bDefineDirection As Boolean = False)
         '		Dim sTest As String = "a"
         Try



            dsTopologyName = oPolygon.TopoName


            diTopoID = oPolygon.ID

            dsTopologyTableName = ODTable.GetTableName(dsTopologyName, TopoTableType.Link)

            '''''''''''''''''''TEMP doLinkTable = New ODTable(dsTopologyName, TopoTableType.Link)

            '	DMAcadExt.AcadDocument.WriteMessage("$$Topolog= '" & dsTopologyName & "'; TopoID= " & CStr(diTopoID))
            MyBase.dsPgonTopoName = dsTopologyName & "_" & CStr(diTopoID)

            MyBase.SetTagNum(msTopologyTag, diTopoID)

            ddAcadArea = oPolygon.Area * ddAreaScale
            ddPerimeter = Math.Round(oPolygon.Perimeter, 0)
            ddCentroidX = oPolygon.Centroid.X
            ddCentroidY = oPolygon.Centroid.Y

            dtCentroidAcObjID = oPolygon.AcObjID

            dbDefineDirection = bDefineDirection

            '''''''''''''''''''''''	Me.SetAttributeOrder()
            '  zzCalc()

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "#237 Topology:'" & dsTopologyName & "'; Polygon Id = " & CStr(diTopoID) & "; X,Y = " & CStr(ddCentroidX) & "," & CStr(ddCentroidY))
         End Try
      End Sub
     
      Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, dicEntities As ObjectIdCollection, ByVal iFeatureID As Integer)
         MyBase.diUniqueNum = iFeatureID
         ddAcadArea = Math.Abs(oPolygon.Area)
         doMPolygon = oPolygon
         '''''''''	diCentroidAcObjID = oPolygon.ObjectId
         diTopoID = iFeatureID
         '	ddCentroidX = 0.0
         '	ddCentroidY = 0.0
         dcolLines = dicEntities
         MyBase.SetTagNum(msTopologyTag, diTopoID)
         doBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
         Dim oCenterPoint As TPlnPoint = doBoundingBox.GetCenterPoint()
         '      DMAcadExt.AcadDocument.WriteDebugMessage("239---")
         'If doBoundingBox Is Nothing Then
         '         DMAcadExt.AcadDocument.WriteDebugMessage("------+------!!! BB Is Nothing")
         '      Else
         '         DMAcadExt.AcadDocument.WriteDebugMessage("- ! BB OK" & oCenterPoint.Coordinates)
         '      End If
         '	MyBase.dsPgonTopoName = dsTopologyName & "_" & CStr(diTopoID)

         ddCentroidX = oCenterPoint.X
         ddCentroidY = oCenterPoint.Y
         zzCalcMPgon()
      End Sub
      Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.Entity)
         '	ddAcadArea = oPolygon.Area
         '	MessageBox.Show("New(oPolygon As Entity", "03_379")
         Try
            Dim tGeoExt As Autodesk.AutoCAD.DatabaseServices.Extents3d = oPolygon.GeometricExtents



            doBoundingBox = New DMAcadExt.TPlnBoundingBox(tGeoExt)
            Dim oPoint As TPlnPoint = doBoundingBox.GetCenterPoint()
            ddCentroidX = oPoint.X
            ddCentroidY = oPoint.Y

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon New")
         End Try
         dtPgonHandle = oPolygon.Handle
      End Sub

      Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, sXDAppName As String)
         ddAcadArea = Math.Abs(oPolygon.Area)
         System.Windows.Forms.MessageBox.Show("_____________++", "TplnTopoPgon New")
         Try
            Dim oGeoExt As Autodesk.AutoCAD.DatabaseServices.Extents3d = oPolygon.GeometricExtents
            doBoundingBox = New DMAcadExt.TPlnBoundingBox()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon New")
         End Try

         Dim oResBuffer As ResultBuffer = oPolygon.GetXDataForApplication(sXDAppName)
         Dim s As String = sXDAppName & vbCrLf
         doTplnXData = New DMAcadExt.TplnXData(oPolygon.XData, sXDAppName)
         If doTplnXData.HasData Then
            '	dtaTypedValues = oResBuffer.AsArray()
            '	For i As Integer = 0 To dtaTypedValues.GetUpperBound(0)
            's &= CStr(dtaTypedValues(i).TypeCode) & ":" & dtaTypedValues(i).Value.ToString() & vbCrLf
            '	Next
            '	MessageBox.Show(s, "02_303")
            '''''''''	diCentroidAcObjID = oPolygon.ObjectId
            Try
               doTplnXData.GetInt(0, diTopoID)
            Catch oEx As Exception
               MessageBox.Show(oEx.Message, "02_128")
            End Try

         Else
            MessageBox.Show(sXDAppName, "02_100a")
         End If

         ''''''''''''	dtPgonHandle = oPolygon.Handle
         ddCentroidX = 0.0
         ddCentroidY = 0.0

      End Sub
		' doaBulgeVertexArray(0) = New GeoUtilites.BulgeVertexArray(oaBulgeVertex)
		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, iFeatureID As Integer, tCentroidAcObjID As ObjectId)
			If oPolygon IsNot Nothing Then
				'  DMCommon.ExcelLog.SetNextValue(iRow, 2, iFeatureID, oPolygon.Area)
				ddAcadArea = Math.Abs(oPolygon.Area)

				doMPolygon = oPolygon
				diTopoID = iFeatureID
				MyBase.diUniqueNum = iFeatureID
				MyBase.SetTagNum(msTopologyTag, diTopoID)

				Try
					Dim oGeoExt As Autodesk.AutoCAD.DatabaseServices.Extents3d = oPolygon.GeometricExtents
					'System.Windows.Forms.MessageBox.Show(CStr("") & ":" & "" & ":" & tCentroidAcObjID.ToString(), "03_248")
					doBoundingBox = New DMAcadExt.TPlnBoundingBox(oGeoExt)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon New")
				End Try
			End If

			dtCentroidAcObjID = tCentroidAcObjID

			If doTplnXData IsNot Nothing AndAlso doTplnXData.HasData Then
				'	dtaTypedValues = oResBuffer.AsArray()
				'	For i As Integer = 0 To dtaTypedValues.GetUpperBound(0)
				's &= CStr(dtaTypedValues(i).TypeCode) & ":" & dtaTypedValues(i).Value.ToString() & vbCrLf
				'	Next
				'	MessageBox.Show(s, "02_303")
				'''''''''	diCentroidAcObjID = oPolygon.ObjectId
				Try
					doTplnXData.GetInt(0, diTopoID)
				Catch oEx As Exception
					MessageBox.Show(oEx.Message, "02_128")
				End Try

			Else
				'MessageBox.Show("sXDAppName", "02_100b")
			End If

			''''''''''''	dtPgonHandle = oPolygon.Handle

			Dim tPoint As Point3d = AcadTransaction.GetBlockRefInsPoint(tCentroidAcObjID)
			ddCentroidX = tPoint.X
			ddCentroidY = tPoint.Y
			zzCalcMPgon()
		End Sub
		Public ReadOnly Property RingCount As Integer
			Get
				Return doaBulgeVertexArray.GetUpperBound(0) + 1
			End Get
		End Property
		Public Function GetBoundary() As Polyline
         If doaBulgeVertexArray IsNot Nothing Then

            doaBulgeVertexArray(0).Complete()
            Return doaBulgeVertexArray(0).CreatePolyline()
         Else
            Return Nothing
         End If
      End Function
      Public Property PgonHandle As Handle
         Get
            Return dtPgonHandle
         End Get
         Set(tValue As Handle)
            dtPgonHandle = tValue
         End Set
      End Property


      Public ReadOnly Property Correct() As Boolean
         Get
            Return dbCorrect
         End Get
      End Property
      Public ReadOnly Property AcadPoint() As Boolean
         Get
            Return dbAcadPoint
         End Get
      End Property
      Public Property TopoID() As Integer
         Get
            Return diTopoID
         End Get
         Set(ByVal iValue As Integer)
            diTopoID = iValue
         End Set
      End Property
		Public Property CentroidAcObjID() As ObjectId
			Get
				Return dtCentroidAcObjID
			End Get
			Set(tValue As ObjectId)
				dtCentroidAcObjID = tValue
			End Set
		End Property
		Public Property BorderAcObjID() As ObjectId
         Get
            Return dtBorderAcObjID
         End Get
         Set(tValue As ObjectId)
            dtBorderAcObjID = tValue
         End Set
      End Property




      Public ReadOnly Property Perimiter() As Double
         Get
            Return ddPerimeter
         End Get
      End Property
      Public ReadOnly Property AcadArea(ByVal bDun As Boolean) As Double
         Get
            If bDun Then
               Return ddAcadArea / TplnProject.UnitScaleFactor
            Else
               Return ddAcadArea
            End If
         End Get
      End Property
      Public ReadOnly Property Neighbors() As Dictionary(Of Integer, Integer)
         Get
            Return mdicNeighbors
         End Get
      End Property
      Public ReadOnly Property AdjoiningLines() As Dictionary(Of Integer, ObjectIdCollection)
         Get
            Return mdicAdjoiningLines
         End Get
      End Property
      Public Function GetBoundary(ByVal iNeighborID As Integer) As ObjectIdCollection
         Dim colRes As ObjectIdCollection = Nothing
         If mdicAdjoiningLines.TryGetValue(iNeighborID, colRes) Then
            Return colRes
         Else
            Return Nothing
         End If
      End Function
      Public Function GetPlinesBoundary(ByVal iNeighborID As Integer, ByVal iSegmentNo As Integer, ByRef iIndexFrom As Integer, ByRef iIndexTo As Integer, ByRef colAcObjIds As ObjectIdCollection, ByRef sLayerName As String) As Polyline
         If doaBulgeVertexArray IsNot Nothing AndAlso doaBulgeVertexArray(0) IsNot Nothing Then
            Return doaBulgeVertexArray(0).GetNeighborBoundary(iNeighborID, iSegmentNo, iIndexFrom, iIndexTo, colAcObjIds, sLayerName)
         Else
            Return Nothing
         End If

      End Function
      Public Sub MoveBoundary(ByVal iIndexFrom As Integer, ByVal iIndexTo As Integer)

      End Sub
      Public ReadOnly Property CentroidX() As Double
         Get
            Return ddCentroidX
         End Get
      End Property
      Public ReadOnly Property CentroidY() As Double
         Get
            Return ddCentroidY
         End Get
      End Property
      Public ReadOnly Property CentroidPoint3d() As Point3d
         Get
            Return New Point3d(ddCentroidX, ddCentroidY, 0.0)
         End Get
      End Property
      Public ReadOnly Property CentroidPoint2d() As Point2d
         Get
            Return New Point2d(ddCentroidX, ddCentroidY)
         End Get
      End Property

      Public ReadOnly Property BoundingBox() As TPlnBoundingBox
         Get
            Return doBoundingBox
         End Get
      End Property
      Public Sub Highlight()
         '	AcadTransaction.Highlight(dcolBoundaryObj)
         If dcolLines IsNot Nothing Then
            AcadTransaction.Highlight(dcolLines)
         End If

      End Sub

      Public Shared Function IsPaintTopoName(ByVal sTopologyName As String) As Boolean
         If zzIsPaintTopoName(dsPgonPrefix & msTopologyTag, sTopologyName) Then
            Return True
         ElseIf zzIsPaintTopoName(dsZebraBoxPrefix & msTopologyTag, sTopologyName) Then
            Return True
         ElseIf zzIsPaintTopoName(dsZebraPgonPrefix & msTopologyTag, sTopologyName) Then
            Return True
         ElseIf zzIsPaintTopoName("PgonPaint" & msTopologyTag, sTopologyName) Then
            Return True
         Else
            Return False
         End If
      End Function
      Protected Sub SetAttributeOrder()
         Dim iaBlockAttribIndex() As Integer = Me.BlockAttribIndex
         Dim iaAddBlockAttribIndex() As Integer = Me.AddBlockAttribIndex
			'   Dim iRow As Integer
			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!BlockAttribIndex", iaBlockAttribIndex)
			If iaBlockAttribIndex IsNot Nothing Then
            Try
					'  DMCommon.Functions.DispArray(iaBlockAttribIndex, "++iaBlockAttribIndex")
					'	DMCommon.ExcelLog.SetNextArray(BlockAttribIndex, 0)
					dsaBlockAttribText = AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, False, dbAcadPoint, iaBlockAttribIndex)
					'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!TopoPgon.InputIndex", iaBlockAttribIndex)
					'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!TopoPgon.InputAttrib", dsaBlockAttribText)
					'	DMCommon.ExcelLog.SetNextArray(dsaBlockAttribText, 1)
					If dsTopologyName = "BlueLineKKKK" Then
						If dsaBlockAttribText Is Nothing Then
							DMCommon.Functions.DispArray(dsTopologyName & "," & MyBase.dtCentroidAcObjID.ToString() & " 477", iaBlockAttribIndex)
						Else
							DMCommon.Functions.DispArray(dsaBlockAttribText, dsTopologyName & vbCrLf & "07_488", True)
						End If
					End If

					If dsaBlockAttribText Is Nothing Then
                  dbCorrect = False
               End If
               ' zzTestStrArray(dsaBlockAttribText)
            Catch oEx As Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnBasicPgon - New")
            End Try
         Else
            System.Windows.Forms.MessageBox.Show("iaBlockAttribIndex Is Nothing " & vbCrLf & dsTopologyName, "SetAttributeOrder")
         End If
         If iaAddBlockAttribIndex IsNot Nothing Then
            Try

               diAddBlockAcObjID = AcadBlock.GetAddBlockObjID(MyBase.dtCentroidAcObjID)
               ' DMCommon.ExcelLog.SetNextValue(iRow, 2, diAddBlockAcObjID.ToString())
               If Not diAddBlockAcObjID.IsNull Then
						dsaAddBlockAttribText = AcadTransaction.GetAttribText(diAddBlockAcObjID, True, dbAcadPoint, iaAddBlockAttribIndex)
						'		DMCommon.ExcelLog.SetNextArray(dsaBlockAttribText, 2)
					End If



               'If dsaBlockAttribText Is Nothing Then
               '   dbCorrect = False
               'End If

            Catch oEx As Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnBasicPgon - New2")
            End Try
         Else
            '   System.Windows.Forms.MessageBox.Show("iaBlockAttribIndex Is Nothing " & vbCrLf & dsTopologyName, "SetAttributeOrder")
         End If
      End Sub
      Protected Sub SetAttributeOrder(ByVal iaBlockAttribIndex() As Integer)
         iaBlockAttribIndex = BlockAttribIndex
         If iaBlockAttribIndex IsNot Nothing Then
            Try
               dsaBlockAttribText = AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, True, dbAcadPoint, iaBlockAttribIndex)
               If dsaBlockAttribText Is Nothing Then
                  dbCorrect = False
               End If
               ' zzTestStrArray(dsaBlockAttribText)
            Catch oEx As Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnBasicPgon - New")
            End Try
         End If
      End Sub
      Protected Sub InputAttributeData(ByVal oAcadBlockDef As DMAcadExt.AcadBlockDef, ByVal dgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute, ByVal bMustExist As Boolean)
         Try
				dsaBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, bMustExist, dbAcadPoint, oAcadBlockDef.AttributeIndices)

			Catch oEx As Exception
         End Try
         If IsArray(dsaBlockAttribText) Then
            For iIndex As Integer = 0 To dsaBlockAttribText.GetUpperBound(0)

               Try
                  dgaParseAttribute(iIndex)(dsaBlockAttribText(iIndex))
               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message, "01_148er")
               End Try

            Next
         End If
      End Sub

      Protected Sub UpdateCentroidAttributes(saValues() As String)
         Dim iaBlockAttribIndex() As Integer = Me.BlockAttribIndex
         '     Dim i As Integer
         '    DMCommon.ExcelLog.SetValue(i, 12, iaBlockAttribIndex.GetUpperBound(0), saValues.GetUpperBound(0), "!!!!")
         '   DMCommon.ExcelLog.SetArray(iaBlockAttribIndex, 15)


         '  DMCommon.ExcelLog.SetArray(saValues, 22)

         If iaBlockAttribIndex IsNot Nothing Then
            AcadTransaction.UpdateAttribText(dtCentroidAcObjID, True, iaBlockAttribIndex, saValues)
         End If

      End Sub
      Private Shared Function zzIsPaintTopoName(ByVal sPrefix As String, ByVal sTopologyName As String) As Boolean
         If sTopologyName.StartsWith(sPrefix) Then
            Dim sAddition As String = sTopologyName.Substring(sPrefix.Length + 1)
            Dim saNum() As String = Split(sAddition, "_")
            Dim bRes As Boolean = True
            For iIndex As Integer = 0 To saNum.GetUpperBound(0)
               bRes = IsNumeric(saNum(iIndex))
               If Not bRes Then
                  Exit For
               End If
            Next
            Return bRes
         Else
            Return False
         End If
      End Function
      Private Sub zzCalcMPgon()
         If doMPolygon IsNot Nothing Then
            Dim oMPolygonLoop As Autodesk.AutoCAD.DatabaseServices.MPolygonLoop
            Dim iInteriorRingIndex As Integer = 0
            '   MessageBox.Show("doMPolygon.NumMPolygonLoops= #" & doMPolygon.NumMPolygonLoops.ToString, "21_101")
            'doMPolygon.
            ReDim doaBulgeVertexArray(doMPolygon.NumMPolygonLoops - 1)
            For iLoopIbdex As Integer = 0 To doMPolygon.NumMPolygonLoops - 1
               oMPolygonLoop = doMPolygon.GetMPolygonLoopAt(iLoopIbdex)
               If doMPolygon.GetLoopDirection(iLoopIbdex) = LoopDirection.Exterior Then
                  zzCalcLoop(oMPolygonLoop, 0)
               Else
                  iInteriorRingIndex += 1
                  zzCalcLoop(oMPolygonLoop, iInteriorRingIndex)
               End If
               '	oMPolygonLoop.
            Next
         End If
      End Sub
      Private Sub zzCalcLoop(ByRef oMPolygonLoop As Autodesk.AutoCAD.DatabaseServices.MPolygonLoop, ByVal iInteriorRingIndex As Integer)
         Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray(oMPolygonLoop)
         If iInteriorRingIndex > doaBulgeVertexArray.GetUpperBound(0) Then
            Dim sMsg As String
            sMsg = CStr(iInteriorRingIndex) & ":" & CStr(doaBulgeVertexArray.GetUpperBound(0)) & "__" & CStr(diTopoID)
            DMAcadExt.AcadDocument.WriteMessage(sMsg)
         Else
            If iInteriorRingIndex = 0 Then
               doaBulgeVertexArray(0) = oBulgeVertexArray
            Else
               doaBulgeVertexArray(iInteriorRingIndex) = oBulgeVertexArray
            End If
         End If


      End Sub
      Private Sub zzCalcPolygon(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         Dim iInteriorRingIndex As Integer = 0
         Dim colRings As RingCollection = Nothing
         dcolLines = New ObjectIdCollection()
         doBoundingBox = New DMAcadExt.TPlnBoundingBox()
         dcolBoundaryObj = New ObjectIdCollection()
         mdicNeighbors = New Dictionary(Of Integer, Integer)()
         mdicAdjoiningLines = New Dictionary(Of Integer, ObjectIdCollection)()
         Try
            colRings = oPolygon.GetBoundary()
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalc 12_662" & "_" & CStr(diTopoID) & ";" & ":" & CStr(ddCentroidX) & "," & CStr(ddCentroidY), oMapEx.StackTrace)

         End Try
         Try


            ''''''''''''''''''		moPgonScheme = New TopoScheme.Polygon(diTopoID, mcolRings.Count)
            If colRings IsNot Nothing Then
               ReDim doaBulgeVertexArray(colRings.Count - 1)
               'DMAcadExt.AcadDocument.WriteMessage("Rings=" & CStr(mcolRings.Count) & " PgonId=" & CStr(diTopoID) & " bPositiveRing=" & CStr("bPositiveRing"))
               For Each oRing As Ring In colRings

						If Not oRing.IsExterior Then
                     iInteriorRingIndex += 1
                  End If
                  '	AcadDocument.WriteDebugMessage("Topo: " & dsTopologyName & ", Pgon# " & CStr(diTopoID))
                  zzCalcRingB(oRing, iInteriorRingIndex)
                  '	DMAcadExt.AcadDocument.WriteMessage("-- - --ENDOF ---PgonId=" & CStr(diTopoID))
                  oRing.Dispose()
                  oRing = Nothing
               Next oRing '
            End If

         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalc 12_665" & "_" & CStr(diTopoID) & ";" & ":" & CStr(ddCentroidX) & "," & CStr(ddCentroidY), oMapEx.StackTrace)

         End Try
         If colRings IsNot Nothing Then
            '	mcolRings.Clear()
            '	colRings.Dispose()
            colRings = Nothing
         End If

      End Sub
		Private Sub zzCalcRingB(ByRef oRing As Ring, ByVal iInteriorRingIndex As Integer)

			Const bDebug As Boolean = False
			Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim oFullEdge As FullEdge = Nothing
			Dim oStartHalfEdge As HalfEdge
			Dim oHalfEdge As HalfEdge = Nothing
			Dim tAcObjID As ObjectId
			Dim colHalfEdges As HalfEdgeCollection
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = Nothing

			Dim dicHalfEdges As Dictionary(Of Integer, HalfEdge) = New Dictionary(Of Integer, HalfEdge)
         Dim sErrMsg As String = Nothing

			'''''''''	Dim oBranch As TopoScheme.Branch
			'''''''''''''	Dim oTSNode As TopoScheme.Node
			colHalfEdges = oRing.GetEdges()

			''''''''''''	Dim oTSRing As TopoScheme.Ring = New TopoScheme.Ring(colHalfEdges.Count)
         '	AcadDocument.WriteDebugMessage("colHalfEdges: " & colHalfEdges.Count.ToString())

			For Each oHEdge As HalfEdge In colHalfEdges
				oFullEdge = oHEdge.FullEdge
				tAcObjID = oFullEdge.Entity
            dcolLines.Add(tAcObjID)

				''''''''''	oBranch = New TopoScheme.Branch(oFullEdge.ID, oHEdge.PreviousNode.ID, oHEdge.NextNode.ID)
				''''''''''''''	oTSRing.AddBranch(oBranch)
				If oFullEdge IsNot Nothing Then
					oFullEdge.Dispose()
					oFullEdge = Nothing
				End If
				dicHalfEdges.Add(oHEdge.PreviousNode.ID, oHEdge)
				If oRing.IsExterior Then  ' Calc Polygon Bounding Box
					oBoundingBox = DMAcadExt.AcadTransaction.GetBoundingBox(tAcObjID, False)
					If oBoundingBox IsNot Nothing AndAlso Not oBoundingBox.IsEmpty Then
						doBoundingBox.Union(oBoundingBox)
					Else
						MessageBox.Show("BoundingBox #" & tAcObjID.ToString & " was not found")
					End If
				End If

			Next

			If oBoundingBox IsNot Nothing Then
				'oBoundingBox.Terminate()
				oBoundingBox = Nothing
			End If


			Dim oNode As Node
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			'Dim oTryHalfEdge As HalfEdge
			Dim iErrNext As Integer = 0
			Dim iFalseNext As Integer = 0
			Dim iMidNext As Integer = 0
			Dim iStartID, iEndID As Integer
			Dim bLeftDir As Boolean
			Dim sTrace As String = ""
			Dim iNeighbor As Integer
			Try
				oHalfEdge = oRing.StartEdge	 'START
				oStartHalfEdge = oHalfEdge
				oFullEdge = oHalfEdge.FullEdge
				bLeftDir = zzGetDirectionPlus(oFullEdge, iNeighbor)
				'	AcadDocument.WriteMessage("####1 iNeighbor=" & CStr(iNeighbor))
				oNode = oHalfEdge.PreviousNode
				iStartID = oNode.ID
				sTrace = CStr(iStartID) & "," & "(" & CStr(oFullEdge.ID) & ")"

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_32")
				Return
			End Try

			If bDebug AndAlso oFullEdge IsNot Nothing AndAlso oHalfEdge IsNot Nothing Then
				AcadDocument.WriteMessage("---" & CStr(oFullEdge.ID) & ":" & CStr(bLeftDir) & "," & CStr(iStartID) & "-->" & CStr(oHalfEdge.NextNode.ID))
			End If



			oBulgeVertexArray.SourceID = diTopoID
			oBulgeVertexArray.SourceTopoName = dsTopologyName

			For iIndex As Integer = 0 To colHalfEdges.Count - 1
				Try
					tAcObjID = oFullEdge.Entity
					zzAddNeighbor(iNeighbor, tAcObjID)

					tPoint = zzPoint3dTo2d(oNode.Location)
					oBulgeVertexArray.AddCurve(tAcObjID, tPoint, iNeighbor)
					If oNode IsNot Nothing Then
						oNode.Dispose()
						oNode = Nothing
					End If
					oNode = oHalfEdge.NextNode
					''''''''''''	oTSNode = New TopoScheme.Node(oNode.ID, tPoint)
					''''''''''''''''''''''	oTSRing.AddNode(oTSNode)
					sTrace &= "," & CStr(oNode.ID)
					'	AcadDocument.WriteMessage("####!" & CStr(colHalfEdges.Count) & ":" & CStr(dbDefineDirection))
					If colHalfEdges.Count > 1 AndAlso True Then 'AndAlso False dbDefineDirection
						If oHalfEdge IsNot Nothing Then
							oHalfEdge.Dispose()
						End If
						oHalfEdge = dicHalfEdges.Item(oNode.ID)
						If oFullEdge IsNot Nothing Then
							oFullEdge.Dispose()
							oFullEdge = Nothing
						End If

						oFullEdge = oHalfEdge.FullEdge
						zzGetDirectionPlus(oFullEdge, iNeighbor)
						'	AcadDocument.WriteMessage("####2 iNeighbor=" & CStr(iNeighbor))
						sTrace &= "," & "(" & CStr(oFullEdge.ID) & ")"
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_21")
				End Try
			Next
			oBulgeVertexArray.CloseSegment()
			Dim bComplete As Boolean = oBulgeVertexArray.Complete()

			iEndID = oNode.ID
			oBulgeVertexArray.PositiveRotation = bLeftDir
			If bDebug Then
				If sErrMsg IsNot Nothing Then
					AcadDocument.WriteMessage(CStr(Me.diTopoID) & ":" & sTrace & "!!" & sErrMsg)
				End If
			End If
			''''''''''''''''''''	moPgonScheme.AddRing(oTSRing, oRing.IsExterior)
			Try

				If oRing.IsExterior Then
					doaBulgeVertexArray(0) = oBulgeVertexArray
					'	doaBulgeVertexArray(0).DispInfo()
				Else
					doaBulgeVertexArray(iInteriorRingIndex) = oBulgeVertexArray
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzCalcRing " & CStr(diTopoID))
			End Try

			Try

				If oFullEdge IsNot Nothing Then
					oFullEdge.Dispose()
					oFullEdge = Nothing
				End If

				For Each oHEdge As HalfEdge In dicHalfEdges.Values
					oHEdge.Dispose()
				Next
				dicHalfEdges = Nothing
				If oHalfEdge IsNot Nothing Then
					oHalfEdge.Dispose()
					oHalfEdge = Nothing
				End If
				If colHalfEdges IsNot Nothing Then
					colHalfEdges.Dispose()
					colHalfEdges = Nothing
				End If
				If oStartHalfEdge IsNot Nothing Then
					oStartHalfEdge.Dispose()
					oStartHalfEdge = Nothing
				End If

				If oNode IsNot Nothing Then
					oNode.Dispose()
					oNode = Nothing
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_12 ")
			End Try
		End Sub
		Private Sub zzCalcRingA(ByRef oRing As Ring, ByVal iInteriorRingIndex As Integer)

			Const bDebug As Boolean = False
			Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim oFullEdge As FullEdge = Nothing
			Dim oStartHalfEdge As HalfEdge
			Dim oHalfEdge As HalfEdge = Nothing
			Dim tAcObjID As ObjectId
			Dim colHalfEdges As HalfEdgeCollection
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = Nothing

			Dim dicHalfEdges As Dictionary(Of Integer, HalfEdge) = New Dictionary(Of Integer, HalfEdge)
			Dim sErrMsg As String = Nothing
			'''''''''	Dim oBranch As TopoScheme.Branch
			'''''''''''''	Dim oTSNode As TopoScheme.Node
			colHalfEdges = oRing.GetEdges()

			''''''''''''	Dim oTSRing As TopoScheme.Ring = New TopoScheme.Ring(colHalfEdges.Count)
         '	AcadDocument.WriteDebugMessage("colHalfEdges: " & colHalfEdges.Count.ToString())

			For Each oHEdge As HalfEdge In colHalfEdges
				oFullEdge = oHEdge.FullEdge
				tAcObjID = oFullEdge.Entity
				dcolLines.Add(tAcObjID)
				''''''''''	oBranch = New TopoScheme.Branch(oFullEdge.ID, oHEdge.PreviousNode.ID, oHEdge.NextNode.ID)
				''''''''''''''	oTSRing.AddBranch(oBranch)
				If oFullEdge IsNot Nothing Then
					oFullEdge.Dispose()
					oFullEdge = Nothing
				End If
				dicHalfEdges.Add(oHEdge.PreviousNode.ID, oHEdge)
				If oRing.IsExterior Then  ' Calc Polygon Bounding Box
					oBoundingBox = DMAcadExt.AcadTransaction.GetBoundingBox(tAcObjID, False)
					If oBoundingBox IsNot Nothing AndAlso Not oBoundingBox.IsEmpty Then
						doBoundingBox.Union(oBoundingBox)
					Else
						MessageBox.Show("BoundingBox #" & tAcObjID.ToString & " was not found")
					End If
				End If
			Next

			If oBoundingBox IsNot Nothing Then
				'oBoundingBox.Terminate()
				oBoundingBox = Nothing
			End If


			Dim oNode As Node
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			'Dim oTryHalfEdge As HalfEdge
			Dim iErrNext As Integer = 0
			Dim iFalseNext As Integer = 0
			Dim iMidNext As Integer = 0
			Dim iStartID, iEndID As Integer
			Dim bLeftDir As Boolean
			Dim sTrace As String = ""

			Try
				oHalfEdge = oRing.StartEdge	 'START
				oStartHalfEdge = oHalfEdge
				oFullEdge = oHalfEdge.FullEdge
				bLeftDir = zzGetDirectionP(oFullEdge)
				oNode = oHalfEdge.PreviousNode
				iStartID = oNode.ID
				sTrace = CStr(iStartID) & "," & "(" & CStr(oFullEdge.ID) & ")"

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_32")
				Return
			End Try

			If bDebug AndAlso oFullEdge IsNot Nothing AndAlso oHalfEdge IsNot Nothing Then
				AcadDocument.WriteMessage("---" & CStr(oFullEdge.ID) & ":" & CStr(bLeftDir) & "," & CStr(iStartID) & "-->" & CStr(oHalfEdge.NextNode.ID))
			End If



			oBulgeVertexArray.SourceID = diTopoID
			oBulgeVertexArray.SourceTopoName = dsTopologyName

			For iIndex As Integer = 0 To colHalfEdges.Count - 1
				Try
					tAcObjID = oFullEdge.Entity
					tPoint = zzPoint3dTo2d(oNode.Location)
					oBulgeVertexArray.AddCurve(tAcObjID, tPoint, 0)
					If oNode IsNot Nothing Then
						oNode.Dispose()
						oNode = Nothing
					End If
					oNode = oHalfEdge.NextNode
					''''''''''''	oTSNode = New TopoScheme.Node(oNode.ID, tPoint)
					''''''''''''''''''''''	oTSRing.AddNode(oTSNode)
					sTrace &= "," & CStr(oNode.ID)
					'	AcadDocument.WriteMessage("####!" & CStr(colHalfEdges.Count) & ":" & CStr(dbDefineDirection))
					If colHalfEdges.Count > 1 AndAlso dbDefineDirection Then	'AndAlso False 
						If oHalfEdge IsNot Nothing Then
							oHalfEdge.Dispose()
						End If
						oHalfEdge = dicHalfEdges.Item(oNode.ID)
						If oFullEdge IsNot Nothing Then
							oFullEdge.Dispose()
							oFullEdge = Nothing
						End If

						oFullEdge = oHalfEdge.FullEdge

						sTrace &= "," & "(" & CStr(oFullEdge.ID) & ")"
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_21")
				End Try
			Next

			Dim bComplete As Boolean = oBulgeVertexArray.Complete()

			iEndID = oNode.ID
			oBulgeVertexArray.PositiveRotation = bLeftDir
			If bDebug Then
				If sErrMsg IsNot Nothing Then
					AcadDocument.WriteMessage(CStr(Me.diTopoID) & ":" & sTrace & "!!" & sErrMsg)
				End If
			End If
			''''''''''''''''''''	moPgonScheme.AddRing(oTSRing, oRing.IsExterior)
			Try
				If oRing.IsExterior Then
					doaBulgeVertexArray(0) = oBulgeVertexArray
					'	doaBulgeVertexArray(0).DispInfo()
				Else
					doaBulgeVertexArray(iInteriorRingIndex) = oBulgeVertexArray
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzCalcRing " & CStr(diTopoID))
			End Try

			Try

				If oFullEdge IsNot Nothing Then
					oFullEdge.Dispose()
					oFullEdge = Nothing
				End If

				For Each oHEdge As HalfEdge In dicHalfEdges.Values
					oHEdge.Dispose()
				Next
				dicHalfEdges = Nothing
				If oHalfEdge IsNot Nothing Then
					oHalfEdge.Dispose()
					oHalfEdge = Nothing
				End If
				If colHalfEdges IsNot Nothing Then
					colHalfEdges.Dispose()
					colHalfEdges = Nothing
				End If
				If oStartHalfEdge IsNot Nothing Then
					oStartHalfEdge.Dispose()
					oStartHalfEdge = Nothing
				End If

				If oNode IsNot Nothing Then
					oNode.Dispose()
					oNode = Nothing
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_12 ")
			End Try
		End Sub

		Private Sub zzCalcRing(ByVal oRing As Ring, ByVal iInteriorRingIndex As Integer)
			Const bDebug As Boolean = False
			Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim oFullEdge As FullEdge
			Dim oStartHalfEdge As HalfEdge
			Dim oHalfEdge As HalfEdge
			Dim tAcObjID As ObjectId
			Dim colHalfEdges As HalfEdgeCollection
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
			Dim dicHalfEdges As Dictionary(Of Integer, HalfEdge) = New Dictionary(Of Integer, HalfEdge)
			Dim sErrMsg As String = Nothing
            Dim oBranch As TopoScheme.tsBranch
            Dim oTSNode As TopoScheme.tsNode
            colHalfEdges = oRing.GetEdges()
            Dim oTSRing As TopoScheme.tsRing = New TopoScheme.tsRing(colHalfEdges.Count)
            AcadDocument.WriteDebugMessage("colHalfEdges: " & dcolLines.Count.ToString())
            For Each oHEdge As HalfEdge In colHalfEdges

                oFullEdge = oHEdge.FullEdge
                tAcObjID = oFullEdge.Entity
                dcolLines.Add(tAcObjID)
                oBranch = New TopoScheme.tsBranch(oFullEdge.ID, oHEdge.PreviousNode.ID, oHEdge.NextNode.ID)
                oTSRing.AddBranch(oBranch)
                dicHalfEdges.Add(oHEdge.PreviousNode.ID, oHEdge)
                If oRing.IsExterior Then  ' Calc Polygon Bounding Box
                    oBoundingBox = DMAcadExt.AcadTransaction.GetBoundingBox(tAcObjID, False)
                    If oBoundingBox IsNot Nothing Then
                        doBoundingBox.Union(oBoundingBox)
                    Else
                        MessageBox.Show("BoundingBox #" & tAcObjID.ToString & " was not found")
                    End If
                End If
            Next
			Dim oNode As Node
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim oTryHalfEdge As HalfEdge
			Dim iErrNext As Integer = 0
			Dim iFalseNext As Integer = 0
			Dim iMidNext As Integer = 0
			Dim iStartID, iEndID As Integer
			Dim bLeftDir As Boolean
			Dim sTrace As String
			Try
				oHalfEdge = oRing.StartEdge	 'START
				oStartHalfEdge = oHalfEdge
				oFullEdge = oHalfEdge.FullEdge
				oNode = oHalfEdge.PreviousNode
				iStartID = oNode.ID
				sTrace = CStr(iStartID) & "," & "(" & CStr(oFullEdge.ID) & ")"
				oTryHalfEdge = oFullEdge.GetHalfEdge(True)
				If Me.zzBelongPgon(oTryHalfEdge) Then
					bLeftDir = True
				Else
					oTryHalfEdge = oFullEdge.GetHalfEdge(False)
					If Me.zzBelongPgon(oTryHalfEdge) Then
						bLeftDir = False
					Else
						sErrMsg = "Start Direction invalid " & CStr(iStartID)
					End If
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_32")
				Return
			End Try

			If bDebug Then
				AcadDocument.WriteMessage("---" & CStr(oFullEdge.ID) & ":" & CStr(bLeftDir) & "," & CStr(iStartID) & "-->" & CStr(oHalfEdge.NextNode.ID))
			End If
			oBulgeVertexArray.SourceID = Me.TopoID
			oBulgeVertexArray.SourceTopoName = dsTopologyName

			For iIndex As Integer = 0 To colHalfEdges.Count - 1
				Try
					tAcObjID = oFullEdge.Entity
					tPoint = zzPoint3dTo2d(oNode.Location)
					If Me.diTopoID = 1031 Then
						'	DMAcadExt.AcadDocument.WriteMessageLog("#3_31 " & CStr(iIndex) & " oNode.ID=" & CStr(oNode.ID) & "; " & DMAcadExt.TPlnPoint.DispPoint(tPoint))
					End If
					oBulgeVertexArray.AddCurve(tAcObjID, tPoint, 0)
					oNode = oHalfEdge.NextNode
               oTSNode = New TopoScheme.tsNode(oNode.ID, tPoint)
					oTSRing.AddNode(oTSNode)
					sTrace &= "," & CStr(oNode.ID)
					'' next  HalfEdge
					'	AcadDocument.WriteMessage("####!" & CStr(colHalfEdges.Count) & ":" & CStr(dbDefineDirection))
					If colHalfEdges.Count > 1 AndAlso dbDefineDirection Then	'AndAlso False 
						Try
							oTryHalfEdge = oHalfEdge.GetNextEdge(bLeftDir)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalcExtents 12_449 =" & dsTopologyName & " _" & CStr(diTopoID) & ";" & CStr(Me.ddCentroidX) & "," & CStr(Me.ddCentroidY))
							zzDispHalfEdge(oHalfEdge, "24_139 " & dsTopologyName)
							AcadDocument.WriteMessageLog("24_140!" & CStr(colHalfEdges.Count) & ":" & "tr:" & sTrace)
						End Try
						oHalfEdge = dicHalfEdges.Item(oNode.ID)
						oFullEdge = oHalfEdge.FullEdge
						sTrace &= "," & "(" & CStr(oFullEdge.ID) & ")"
						If oTryHalfEdge.FullEdge.ID <> oFullEdge.ID Then
							iErrNext += 1
							If bDebug Then
								AcadDocument.WriteMessage(CStr(oTryHalfEdge.FullEdge.ID) & ":" & CStr(oFullEdge.ID))
							End If
						End If
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - zzCalcRing_21")
				End Try
			Next
			If Me.diTopoID = 1031 Then
				'DMAcadExt.AcadDocument.WriteMessageLog("#3_32 " & " tr:" & sTrace & ": " & DMAcadExt.TPlnPoint.DispPoint(tPoint))
			End If
			Dim iComplete As Boolean = oBulgeVertexArray.Complete()
			If iErrNext <> 0 Then
				If sErrMsg IsNot Nothing Then sErrMsg &= ";"
				sErrMsg &= "Dir is not valid - " & CStr(iErrNext)
			End If
			iEndID = oNode.ID
			oBulgeVertexArray.PositiveRotation = bLeftDir
			If bDebug Then
				If sErrMsg IsNot Nothing Then
					AcadDocument.WriteMessage(CStr(Me.diTopoID) & ":" & sTrace & "!!" & sErrMsg)
				End If
			End If
			''''''''''''''''''''''''''		moPgonScheme.AddRing(oTSRing, oRing.IsExterior)
			Try
				If oRing.IsExterior Then
					doaBulgeVertexArray(0) = oBulgeVertexArray
					'	doaBulgeVertexArray(0).DispInfo()
				Else
					doaBulgeVertexArray(iInteriorRingIndex) = oBulgeVertexArray
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzCalcRing " & CStr(diTopoID))
			End Try
			Try
				colHalfEdges.Clear()
				'oStartHalfEdge.Dispose()
				'oHalfEdge.Dispose()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzCalcRing_12 ")

			End Try
			colHalfEdges = Nothing

			oStartHalfEdge = Nothing
			oHalfEdge = Nothing
		End Sub



		Private Sub zzCalcRingWork(ByVal oRing As Ring, ByVal iInteriorRingIndex As Integer)
			Dim oBulgeVertexArray As GeoUtilites.BulgeVertexArray = New GeoUtilites.BulgeVertexArray()
			Dim oFullEdge As FullEdge
			Dim oHalfEdge As HalfEdge
			Dim colHalfEdges As HalfEdgeCollection
			Dim dicHalfEdges As Dictionary(Of Integer, Integer)
			'	Dim tObjectID As ObjectId
			Dim dMinX As Double = 999999.0, dMinY As Double = 999999.0
			Dim dMaxX As Double = 0.0, dMaxY As Double = 0.0
			Dim sTest As String = "a"
			Dim oFirstNode, oNode As Node

			Dim bPositiveRotation As Boolean
			Dim bFirstEdge As Boolean
			Dim tAcObjID As ObjectId
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
			Try
				sTest = "f"
				colHalfEdges = oRing.GetEdges()
				sTest = "cb"
				oHalfEdge = oRing.StartEdge
				oFirstNode = oHalfEdge.PreviousNode


				Try
					sTest = "d"
					oFullEdge = oHalfEdge.FullEdge
					sTest = "e"
					'	bPositiveRotation = zzGetExteriorLeft(oFullEdge)
					sTest = "ex "
					tAcObjID = oFullEdge.Entity
					dcolLines.Add(tAcObjID)

					If oRing.IsExterior Then
						sTest = "mw"
						oBoundingBox = DMAcadExt.AcadTransaction.GetBoundingBox(tAcObjID, False)

						If oBoundingBox IsNot Nothing Then
							doBoundingBox.Union(oBoundingBox)
						Else
							MessageBox.Show("BoundingBox #" & tAcObjID.ToString & " was not found")
						End If
						sTest = "n"
					End If

					sTest = "eY "
					tPoint = zzPoint3dTo2d(oFirstNode.Location)
					sTest = "ezz"
					oBulgeVertexArray.AddCurve(tAcObjID, tPoint, 0)
					';;;;;;;;;;;;;;;;;;

					''''''''PPPPPPPPPPP
				Catch oEx As Exception
					'''''''''''''''' ''''''''''TEMP System.Windows.Forms.MessageBox.Show(oEx.Message, "459 oFullEdge = oHalfEdge.FullEdge " & sTest & CStr(diTopoID))
					oFullEdge = Nothing
				End Try
				sTest = "h"
				bFirstEdge = True
				Dim oTryHalfEdge As HalfEdge
				'	AcadDocument.WriteMessage("colHalfEdges: " & CStr(colHalfEdges.Count))
				dicHalfEdges = New Dictionary(Of Integer, Integer)
				For Each oHEdge As HalfEdge In colHalfEdges
					dicHalfEdges.Add(oHEdge.FullEdge.ID, 0)
				Next
				Do While oFirstNode.ID <> oHalfEdge.NextNode.ID
					sTest = "i"
					Try
						oTryHalfEdge = oHalfEdge.GetNextEdge(True)
						If dicHalfEdges.ContainsKey(oTryHalfEdge.FullEdge.ID) Then
							oHalfEdge = oTryHalfEdge
							bPositiveRotation = True
						Else
							oTryHalfEdge = oHalfEdge.GetNextEdge(False)
							If dicHalfEdges.ContainsKey(oTryHalfEdge.FullEdge.ID) Then
								oHalfEdge = oTryHalfEdge
								bPositiveRotation = False
							Else
								AcadDocument.WriteMessage(CStr(Me.TopoID) & ":" & CStr(oHalfEdge.PreviousNode.ID) & " >-->> " & CStr(oHalfEdge.NextNode.ID))
								MessageBox.Show("HalfEdge Is not valid", "13_651")
								Exit Do
							End If
						End If
						If oHalfEdge Is Nothing Then
							MessageBox.Show("Exit Do", "13_652")
							Exit Do
						End If
						If Me.TopoID = 147 Then
							'	zzDispHalfEdge(oHalfEdge, CStr(oHalfEdge.NextNode.ID))
						End If

						oBulgeVertexArray.PositiveRotation = bPositiveRotation

					Catch oMapEx As Autodesk.Gis.Map.MapException
						sTest = oMapEx.StackTrace
						'''''''''''''''' ''''''''''TEMP  DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalcExtents 12_439" & "_" & CStr(diTopoID) & ";" & sTest)

						''''''''''''''' '''''''''''TEMP DMAcadExt.AcadDocument.WriteMessage("340 oFullEdge = oHalfEdge.FullEdge " & CStr(bPositiveRotation) & ":" & CStr(diTopoID))
						oHalfEdge = Nothing
						Exit Do
					End Try

					Try
						oNode = oHalfEdge.PreviousNode
						oFullEdge = oHalfEdge.FullEdge
						tAcObjID = oFullEdge.Entity
						dcolLines.Add(tAcObjID)
						''''''''''''''' '''''''''FFF??? 	bPositiveRotation = zzGetExteriorLeft(oFullEdge) 
						''zzDispHalfEdge(oHalfEdge, "NEXT " & CStr(bPositiveRotation) & ":" & CStr(diTopoID))
						oBulgeVertexArray.AddCurve(oFullEdge.Entity, zzPoint3dTo2d(oNode.Location), 0)
					Catch oEx As Exception
						'''''''''''''''''	System.Windows.Forms.MessageBox.Show(oEx.Message, "116 oFullEdge = oHalfEdge.FullEdge " & CStr(diTopoID))
						oFullEdge = Nothing
					End Try
					sTest = "ik"

					If oFullEdge IsNot Nothing And False Then
						sTest = "ip"
						If bFirstEdge Then
							'	bPositiveRotation = zzGetExteriorLeft(oFullEdge)
							bFirstEdge = False
						End If
						''DMAcadExt.AcadDocument.WriteMessage("Pos=" & CStr(bPositiveRotation))
						sTest = "j"
						tAcObjID = oFullEdge.Entity
						sTest = "k"
						dcolBoundaryObj.Add(tAcObjID)
						sTest = "L"
						oFullEdge = Nothing
						sTest = "La"
						If oRing.IsExterior Then
							sTest = "m"
							oBoundingBox = DMAcadExt.AcadTransaction.GetBoundingBox(tAcObjID, False)

							If oBoundingBox IsNot Nothing Then
								doBoundingBox.Union(oBoundingBox)
							Else
								MessageBox.Show("BoundingBox #" & tAcObjID.ToString & " was not found")
							End If
							sTest = "n"
						End If
						sTest = "o"
					End If
				Loop
				oHalfEdge = Nothing
				oNode = Nothing
				If oRing.IsExterior Then
					sTest = "mw"
					doaBulgeVertexArray(0) = oBulgeVertexArray
					sTest = "nw"
				Else
					doaBulgeVertexArray(iInteriorRingIndex) = oBulgeVertexArray
				End If


			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzCalcExtents 12_666" & "_" & CStr(diTopoID) & ";" & sTest)
			Finally
				oBulgeVertexArray = Nothing  ''''''''''?????
				oRing = Nothing
				colHalfEdges = Nothing
			End Try
		End Sub

		Private Function zzPoint3dTo2d(ByVal tPoint3d As Autodesk.AutoCAD.Geometry.Point3d) As Autodesk.AutoCAD.Geometry.Point2d
			Return New Autodesk.AutoCAD.Geometry.Point2d(tPoint3d.X, tPoint3d.Y)
		End Function
		Private Sub zzDispHalfEdge(ByVal oHalfEdge As HalfEdge, ByVal sCaption As String)
			Dim sMsg As String
			If oHalfEdge Is Nothing Then
				sMsg = "Nothing"
			Else
				sMsg = "HalfedgePgonID=" & CStr(oHalfEdge.Polygon.ID) & ":" & CStr(oHalfEdge.PreviousNode.ID) & " >-->> " & CStr(oHalfEdge.NextNode.ID)
			End If
			DMAcadExt.AcadDocument.WriteMessageLog(sCaption & " ID=" & CStr(Me.TopoID) & " : " & sMsg & " 13_560")
			'	System.Windows.Forms.MessageBox.Show(sCaption & " ID=" & CStr(Me.TopoID) & " : " & sMsg, "13_560")
		End Sub
		Private Function zzBelongPgonA(ByVal oHalfEdge As HalfEdge) As Boolean
			Return True
		End Function
		Private Function zzGetDirectionA(ByRef oFullEdge As FullEdge) As Boolean
			Return True
		End Function
		Private Function zzGetDirection(ByVal tAcObjID As ObjectId) As Boolean
			Dim oRec As Autodesk.Gis.Map.ObjectData.Record = doLinkTable.GetODRecord(tAcObjID)
			Dim oValue As Autodesk.Gis.Map.Utilities.MapValue
			Dim iLeft, iRight As Integer
			Try
				oValue = oRec.Item(6)
				iLeft = oValue.Int32Value
				If iLeft = Me.diTopoID Then
					Return True
				Else
					oValue = oRec.Item(7)
					iRight = oValue.Int32Value
					If iRight <> Me.diTopoID Then
						AcadDocument.WriteMessageLog("!!Err!!! " & CStr(Me.diTopoID) & ":" & CStr(iLeft) & " OR " & CStr(iRight))
					End If
					Return False
				End If
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "TplnTopoPgon - zzGetDirection" & "_" & CStr(diTopoID))
				Return False
			End Try
		End Function
		Private Function zzGetDirectionP(ByRef oFullEdge As FullEdge) As Boolean
			Dim oTryHalfEdge As HalfEdge
			oTryHalfEdge = oFullEdge.GetHalfEdge(True)
			If Me.zzBelongPgon(oTryHalfEdge) Then
				oTryHalfEdge.Dispose()
				oTryHalfEdge = Nothing
				Return True
			Else
				oTryHalfEdge = oFullEdge.GetHalfEdge(False)
				If Me.zzBelongPgon(oTryHalfEdge) Then
					oTryHalfEdge.Dispose()
					oTryHalfEdge = Nothing
				Else
					Dim sErrMsg As String
					oTryHalfEdge.Dispose()
					oTryHalfEdge = Nothing

					sErrMsg = "Start Direction invalid "
					DMAcadExt.AcadDocument.WriteMessageLog(" ID=" & CStr(Me.TopoID) & " : " & sErrMsg & " 13_566")
				End If
				Return False
			End If
		End Function
		Private Function zzGetDirectionPlus(ByRef oFullEdge As FullEdge, ByRef iNeighbor As Integer) As Boolean
			Dim oLeftHalfEdge, oRightHalfEdge As HalfEdge
			Dim bOnLeft As Boolean
			oLeftHalfEdge = oFullEdge.GetHalfEdge(True)
			oRightHalfEdge = oFullEdge.GetHalfEdge(False)
			Dim iLeftID As Integer = Me.zzGetPgonID(oLeftHalfEdge)
			Dim iRightID As Integer = Me.zzGetPgonID(oRightHalfEdge)
			If iLeftID = Me.diTopoID Then
				iNeighbor = iRightID
				bOnLeft = True
			ElseIf iRightID = Me.diTopoID Then
				iNeighbor = iLeftID
				bOnLeft = False
			Else
				Dim sErrMsg As String = "Start Direction invalid "
				DMAcadExt.AcadDocument.WriteMessageLog(" ID=" & CStr(Me.TopoID) & " : " & sErrMsg & " 13_566")
			End If
			oLeftHalfEdge.Dispose()
			oLeftHalfEdge = Nothing
			oRightHalfEdge.Dispose()
			oRightHalfEdge = Nothing
			Return bOnLeft
		End Function

		Private Function zzBelongPgon(ByRef oHalfEdge As HalfEdge) As Boolean
			Dim oPgon As Polygon = Nothing
			Dim sTestMsg As String = ""
			Try
				oPgon = oHalfEdge.Polygon
			Catch oMapEx As Autodesk.Gis.Map.MapException
				If oMapEx.ErrorCode = 2010 Then
					If oPgon IsNot Nothing Then
						oPgon.Dispose()
						oPgon = Nothing
					End If
					Return False
				Else
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " TplnTopoPgon - zzBelongPgon:" & sTestMsg)
				End If
			End Try
			If oPgon Is Nothing Then
				Return False
			ElseIf oPgon.ID = Me.diTopoID Then
				oPgon.Dispose()
				oPgon = Nothing
				Return True
			Else
				oPgon.Dispose()
				oPgon = Nothing
				Return False
			End If
		End Function

		Private Function zzGetPgonID(ByRef oHalfEdge As HalfEdge) As Integer
			Dim oPgon As Polygon = Nothing
			Dim sTestMsg As String = ""

			Try
				oPgon = oHalfEdge.Polygon
			Catch oMapEx As Autodesk.Gis.Map.MapException
				If oMapEx.ErrorCode = 2010 Then
					If oPgon IsNot Nothing Then
						oPgon.Dispose()
						oPgon = Nothing
					End If
					Return 0
				Else
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " TplnTopoPgon - zzBelongPgon:" & sTestMsg)
				End If
			End Try
			If oPgon Is Nothing Then
				Return 0
			Else
				Dim iID As Integer = oPgon.ID
				oPgon.Dispose()
				oPgon = Nothing
				Return iID
			End If
		End Function

		Private Sub zzAddNeighbor(ByVal iPolygonID As Integer, ByVal tAcObjID As ObjectId)
			If mdicAdjoiningLines IsNot Nothing Then
				Dim colAcObjIDs As ObjectIdCollection = Nothing
				If mdicAdjoiningLines.TryGetValue(iPolygonID, colAcObjIDs) Then
					colAcObjIDs.Add(tAcObjID)
				Else
					colAcObjIDs = New ObjectIdCollection()
					colAcObjIDs.Add(tAcObjID)
					mdicAdjoiningLines.Add(iPolygonID, colAcObjIDs)
				End If
			End If
			If mdicNeighbors IsNot Nothing Then
				If Not mdicNeighbors.ContainsKey(iPolygonID) Then
					mdicNeighbors.Add(iPolygonID, 0)
				End If
			End If
		End Sub
		Public Function IsNeighbor(ByVal iPgonID As Integer) As Boolean
			Return mdicNeighbors.ContainsKey(iPgonID)
		End Function
		Private Function zzDispEdge(ByVal oFullEdge As FullEdge) As Boolean
			Dim sMsg As String = String.Empty
			Dim oHalfEdge As HalfEdge
			Dim iLeftPgonID, iRightPgonID As Integer
			Dim bOut As Boolean
			System.Windows.Forms.MessageBox.Show("zzDispEdge Before", CStr(diTopoID))
			Try
				Try
					oHalfEdge = oFullEdge.GetHalfEdge(True)
					System.Windows.Forms.MessageBox.Show(CStr(oHalfEdge Is Nothing), "Left " & CStr(diTopoID))
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Left" & CStr(diTopoID))
					oHalfEdge = Nothing
					iLeftPgonID = 0
				End Try

				If oHalfEdge IsNot Nothing Then
					Try
						iLeftPgonID = oHalfEdge.Polygon.ID
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " LeftPgonID = oHalfEdge.Polygon.ID")
						System.Windows.Forms.MessageBox.Show(oHalfEdge.ToString() & ":" & CStr(oHalfEdge.IsDisposed), "IsDisposed " & CStr(diTopoID))

					End Try
				Else
					System.Windows.Forms.MessageBox.Show("oHalfEdge Is Nothing", CStr(diTopoID))
				End If

				Try
					oHalfEdge = oFullEdge.GetHalfEdge(False)
					System.Windows.Forms.MessageBox.Show(CStr(oHalfEdge Is Nothing), "Right " & CStr(diTopoID))

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Right" & CStr(diTopoID))
					oHalfEdge = Nothing
					iRightPgonID = 0
				End Try

				If oHalfEdge IsNot Nothing Then
					Try
						iRightPgonID = oHalfEdge.Polygon.ID
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(diTopoID) & " RightPgonID = oHalfEdge.Polygon.ID")
						System.Windows.Forms.MessageBox.Show(oHalfEdge.ToString() & ":" & CStr(oHalfEdge.IsDisposed), "IsDisposed " & CStr(diTopoID))
					End Try

				End If
				If iLeftPgonID = diTopoID Then
					bOut = True
				ElseIf iRightPgonID = diTopoID Then
					bOut = False
				Else
					sMsg = "  Error!!!"
				End If
				MessageBox.Show(CStr(iLeftPgonID) & ":" & CStr(iRightPgonID) & sMsg, "12_249  -" & CStr(diTopoID))

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "zzDispEdge Global After" & CStr(diTopoID))
			End Try
			Return bOut
		End Function
      Public Function Paint(ByVal iPaintMethod As PaintMethod, ByVal tColorScheme As ColorScheme, ByVal bOpenBlock As Boolean, Optional ByVal sBlockLayer As String = "", Optional ByVal bRecursion As Boolean = False) As PaintException ', Optional ByVal sPgonTopoName As String = ""
         Dim iPaintException As PaintException = PaintException.OK
         Try
				If bOpenBlock Then
					DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
				End If
				'DMCommon.Debug.MsgBox("!PaintByCS", (iPaintMethod And PaintMethod.ColorScheme))
				If (iPaintMethod And PaintMethod.ColorScheme) = PaintMethod.ColorScheme Then
					'DMCommon.Debug.MsgBox("!Paint_1", "PaintMethod.ColorScheme")
					iPaintException = MyBase.PaintColorScheme(iPaintMethod, tColorScheme, False, , bRecursion)
            ElseIf (iPaintMethod And PaintMethod.BorderByBuffer) = PaintMethod.BorderByBuffer Then
               If tColorScheme.HasBorder Then
                  AcadDocument.WriteMessageLog("PaintBorder #" & CStr(tColorScheme.ID) & " - " & CStr(tColorScheme.Border.Strip(0).Color.AcadColorIndex))
                  iPaintException = MyBase.PaintBorder(iPaintMethod, tColorScheme, False)
               End If
            ElseIf (iPaintMethod And PaintMethod.Zebra) = PaintMethod.Zebra Then
               If tColorScheme.HasZebra Then
                  AcadDocument.WriteMessageLog("PaintZebra #" & CStr(tColorScheme.ID))
                  iPaintException = zzPaintZebra(iPaintMethod, tColorScheme.Zebra)
               End If
            ElseIf (iPaintMethod And PaintMethod.Hatch) = PaintMethod.Hatch Then
               If tColorScheme.HasHatch Then
						'	AcadDocument.WriteMessageLog("PaintHatch #" & CStr(tColorScheme.ID))
						iPaintException = MyBase.PaintHatch(iPaintMethod, tColorScheme.Hatch)
					End If
            End If
            If bOpenBlock Then
					DMAcadExt.AcadTransaction.InsertNewBlock(True, sBlockLayer)
				End If
            If iPaintException <> PaintException.OK Then
               AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidY, "", iPaintException, True)
               DMAcadExt.AcadDocument.WriteMessageLog("Temp#03 " & iPaintException.ToString())
            End If
            Return iPaintException
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnTopoPgon - Paint")
            Return PaintException.PolygonError
         End Try

      End Function
		Public Overloads Shared ReadOnly Property PgonTopoName(ByVal iTopoID As Integer) As String
			Get
				Return msPrefixPgonTopoName & CStr(iTopoID)
			End Get
		End Property
      Public MustOverride Sub Terminate()
      
		Private Sub zzGetAcadEntity(ByVal oObjectID As ObjectId)
			Dim oTransaction As Transaction = Nothing
			Dim oTransactionManager As TransactionManager = Nothing
			Dim oCurrentDatabase As Database = HostApplicationServices.WorkingDatabase
			Dim oDBObject As DBObject
			oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
			oTransaction = oTransactionManager.StartTransaction()
			oDBObject = oTransactionManager.GetObject(oObjectID, DatabaseServices.OpenMode.ForRead, False, False)
			System.Windows.Forms.MessageBox.Show(oDBObject.GetType().ToString(), "DBObject")
			Try
				oTransaction.Commit()
				oTransaction = Nothing
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - zzGetAcadEntity")
			Finally
				If Not oTransaction Is Nothing Then
					oTransaction.Abort()
					oTransaction = Nothing
				End If
			End Try
		End Sub
      Private Function zzPaintZebra(ByVal iPaintMethod As PaintMethod, ByVal tZebra As ColorZebra) As PaintException
         If (iPaintMethod And PaintMethod.ZebraByTopo) = PaintMethod.ZebraByTopo Then
            MyBase.SetTagNum(msTopologyTag, diTopoID)
            Return Me.PaintZebra(tZebra)
         Else
            Return PaintException.Undefined
         End If
      End Function
		Private Sub zzAddHatchSource(ByVal tHatch As DMHatch, ByVal bSolid As Boolean)
			Dim tAcobjId As ObjectId
			Dim sTest As String = "a"
			Dim oHatch As Hatch = New Hatch()
			Try
				With oHatch
					.Associative = False
					.HatchStyle = HatchStyle.Normal
					If bSolid Then
						.ColorIndex = tHatch.BackColor.AcadColorIndex
						.SetHatchPattern(HatchPatternType.PreDefined, "SOLID")
					Else
						.ColorIndex = tHatch.PatternColor.AcadColorIndex
						.PatternAngle = tHatch.Angle.AngleRad
						.LineWeight = tHatch.LineWeight
						.PatternScale = tHatch.PatternScale
						.SetHatchPattern(HatchPatternType.PreDefined, tHatch.PatternName)
						'MessageBox.Show(.PatternName & ":" & CStr(.PatternAngle) & ":" & CStr(.PatternScale) & ":" & CStr(.LineWeight), "15_310src")
						Try
							sTest = "bn"
							'	oHatch.ResetScaleDependentProperties()
						Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
							System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sTest & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "zzAddHatch_259acad")
						End Try
					End If
				End With


				sTest = "b"
				Me.zzCalcExternalLoop()
				Me.zzCalcInnerLoops()
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


				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "zzAddHatch=249")
				End Try
				Try
					sTest = "bo"
					oHatch.EvaluateHatch(True)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.ToString() & vbCrLf & oEx.Message & vbCrLf & sTest, "zzAddHatch_255")
				End Try

				sTest = "be"
				tAcobjId = DMAcadExt.AcadTransaction.AppendEntity(oHatch, True)
				Try
					oHatch.UpgradeOpen()
					sTest = "bm"
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
				End Try


			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
			End Try
		End Sub


		Private Function zzGetPgonTopologyNameAAA() As String
			Return "Pgon" & CStr(diTopoID)
		End Function
		Private Sub zzAddHatchBorderAAA(ByVal tBorder As ColorBorder)
			Const iExNo As Integer = 10
			Dim oAcobjId As ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As HatchLoop
			Dim oHatchLoopBorder As HatchLoop
			Dim oHatch As Hatch = New Hatch()

			Try
				sTest = "axb"
				oHatch.Associative = False
				sTest = "axc"

				oHatch.ColorIndex = tBorder.Strip(0).Color.AcadColorIndex
				sTest = "axd"
				oHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = HatchStyle.Normal
				sTest = "axe"
				'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)


				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCompositeInner As Autodesk.AutoCAD.Geometry.CompositeCurve2d


				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				Dim dOffsetDist As Double

				'	oHatchLoop = New HatchLoop(HatchLoopTypes.Polyline)
				oHatchLoop = doaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)

				sTest = "baa"
				oComposite = doaBulgeVertexArray(0).GetCompositeCurve(True)
				sTest = "baa1"
				If doaBulgeVertexArray(0).PositiveRotation Then
					dOffsetDist = 10 * tBorder.Strip(0).ScalingWidth	' 4.6
				Else
					dOffsetDist = -10 * tBorder.Strip(0).ScalingWidth
				End If

				Try
					sTest = "baa1a"
					'	dOffsetDist = 0 - dOffsetDist
					oCurves = oComposite.GetTrimmedOffset(dOffsetDist, Geometry.OffsetCurveExtensionType.Extend, New Geometry.Tolerance(0.001, 0.001))
					sTest = "baa2"
				Catch oEx As System.Exception
					DMAcadExt.AcadDocument.WriteException(miErrClassNo, iExNo + 1, CStr(Me.diTopoID) & ":" & oEx.Message)
					Return
				End Try


				DMAcadExt.AcadDocument.WriteMessage("OK!!! " & CStr(oCurves.GetUpperBound(0)) & ":" & CStr(Me.diTopoID))
				If oCurves.GetUpperBound(0) >= 0 Then
					Try
						oCompositeInner = DirectCast(oCurves(0), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
						sTest = "bba1"
						If Not oCompositeInner.IsClosed Then
							sTest = "bba2"
							oCurves(0) = GeoUtilites.CloseCompositeCurve(oCompositeInner)
						End If
						sTest = "bba3"
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
					End Try
				End If
				Try
					sTest = "gbb"

					oHatch.AppendLoop(oHatchLoop)

					sTest = "gc"
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=232bNew")
				End Try

				If oCurves Is Nothing OrElse oCurves.GetUpperBound(0) = -1 Then


				Else

					oHatchLoopBorder = New HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)

					oHatch.AppendLoop(oHatchLoopBorder)
				End If
				oHatch.EvaluateHatch(True)
				sTest = "baa3"
				sTest = "bb"

				oAcobjId = DMAcadExt.AcadTransaction.AppendEntity(oHatch, True)

				sTest = "g"

				Try
					'	oHatch.UpgradeOpen()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest & vbCrLf & CStr(Me.diTopoID), "zzAddHatchBorder_17")
				End Try
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				DMAcadExt.AcadDocument.WriteException(miErrClassNo, iExNo, CStr(Me.diTopoID) & ":" & oAcadEx.Message)

			End Try

			'  System.Windows.Forms.MessageBox.Show("OK!!! " & CStr(Me.diTopoID), "zzAddHatchBorder_19")
			'	Catch oEx As Exception
			'System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest & vbCrLf & CStr(Me.diTopoID), "zzAddHatch_19 !!!")
			'	End Try


		End Sub

		Private Sub zzAddHatchBorderNew(ByVal tColor As DMColor)
			Dim oTransaction As Transaction = Nothing
			Dim oTransactionManager As TransactionManager = Nothing
			Dim oCurrentDatabase As Database = HostApplicationServices.WorkingDatabase
			Dim oBlockTable As BlockTable
			Dim oBlockTableRecord As BlockTableRecord
			Dim oDrawOrderTable As DrawOrderTable
			Dim tDrawOrderTableID As ObjectId

			Dim oAcobjId As ObjectId
			Dim sTest As String = "a"
			Dim oHatchLoop As HatchLoop

			Dim oHatchLoopBorder As HatchLoop
			Dim oHatch As Hatch = New Hatch()

			Try
				oHatch.Associative = False
				oHatch.ColorIndex = tColor.AcadColorIndex + 1

				oHatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID")
				oHatch.HatchStyle = HatchStyle.Normal
				oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
				oTransaction = oTransactionManager.StartTransaction()

				'	DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, DatabaseServices.OpenMode.ForWrite, False, False), BlockTable)

				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), DatabaseServices.OpenMode.ForWrite, False), BlockTableRecord)

				sTest = "b"
				tDrawOrderTableID = oBlockTableRecord.DrawOrderTableId
				oDrawOrderTable = DirectCast(oTransactionManager.GetObject(tDrawOrderTableID, DatabaseServices.OpenMode.ForWrite, False), DrawOrderTable)

				Dim oComposite As Autodesk.AutoCAD.Geometry.CompositeCurve2d
				Dim oCompositeInner As Autodesk.AutoCAD.Geometry.CompositeCurve2d


				Dim oCurves() As Autodesk.AutoCAD.Geometry.Curve2d
				Dim dOffsetDist As Double
				If True Then
					dOffsetDist = 0.6
				Else
					dOffsetDist = -0.6
				End If
				'	oHatchLoop = New HatchLoop(HatchLoopTypes.Polyline)
				oHatchLoop = doaBulgeVertexArray(0).CreateHatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)

				sTest = "baa"
				oComposite = doaBulgeVertexArray(0).GetCompositeCurve(True)
				sTest = "baa1"

				sTest = "baa1a"
				oCurves = oComposite.GetTrimmedOffset(dOffsetDist, Geometry.OffsetCurveExtensionType.Extend, New Geometry.Tolerance(0.1, 0.1))
				sTest = "xbaa2"
				Try
					oCompositeInner = DirectCast(oCurves(0), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					sTest = "bba1"
					If Not oCompositeInner.IsClosed Then
						sTest = "bba2"
						oCurves(0) = GeoUtilites.CloseCompositeCurve(oCompositeInner)
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), "oComposite.IsClosed 12_4423 zzAddHatchBorderNew")
					Else
						System.Windows.Forms.MessageBox.Show(CStr(oCurves(0).IsClosed()), " 12_3167 zzAddHatchBorderNew")
					End If
					sTest = "bba3"
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
				End Try

				Try
					sTest = "gbb"
					oHatch.AppendLoop(oHatchLoop)
					sTest = "gc"
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=232bNew")
				End Try

				If oCurves Is Nothing OrElse oCurves.GetUpperBound(0) = -1 Then
					If oCurves Is Nothing Then
						MessageBox.Show("oCurves Is Nothing ", "12_848")
					Else
						MessageBox.Show(CStr(oCurves.GetUpperBound(0)), "12_873")
					End If

				Else

					oHatchLoopBorder = New HatchLoop(DatabaseServices.HatchLoopTypes.Default Or DatabaseServices.HatchLoopTypes.Polyline)
					sTest = "baa4"
					oHatchLoopBorder = GeoUtilites.CurvesToLoop(oCurves)
					MessageBox.Show(CStr(oHatchLoop.Polyline.Count) & ":" & CStr(oHatchLoopBorder.Polyline.Count), "12_744z")

					oHatch.AppendLoop(oHatchLoopBorder)
				End If
				oHatch.EvaluateHatch(True)
				sTest = "baa3"
				sTest = "bb"
				oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
				sTest = "g"
				oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
				Try
					oHatch.UpgradeOpen()

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11_bnew")
				End Try

				'  oHatch.Associative = True
				oTransaction.Commit()
				''''	DMAcadExt.AcadDocument.Unlock()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=moaBulgeVertex.GetUpperBound(0).ToString()!!!")
			Finally
				oTransaction.Dispose()
			End Try


		End Sub

		Private Sub zzDispPoint(ByVal oPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal sName As String)
			Dim sText As String = sName & ": " & CStr(oPoint.X) & "," & CStr(oPoint.Y) & vbCrLf
			Common.GetEditor().WriteMessage(sText)
		End Sub
		Private Sub zzDispCurves(ByVal oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d, ByVal sTitle As String)
			Dim sText As String
			Dim sType As String
			Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d
			Common.GetEditor().WriteMessage(sTitle & vbCrLf)
			sText = "Curves " & oaCurves.GetUpperBound(0).ToString() & vbCrLf
			Common.GetEditor().WriteMessage(sText)
			For iIndex As Integer = 0 To oaCurves.GetUpperBound(0)
				zzDispPoint(oaCurves(iIndex).StartPoint, "StartPoint " & iIndex.ToString())
				zzDispPoint(oaCurves(iIndex).EndPoint, "EndPoint " & iIndex.ToString())
				sType = oaCurves(iIndex).GetType().ToString()
				sText = sType & vbCrLf
				Common.GetEditor().WriteMessage(sText)
				If sType = "Autodesk.AutoCAD.Geometry.CompositeCurve2d" Then
					oCompositeCurve = DirectCast(oaCurves(iIndex), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
					Common.GetEditor().WriteMessage("Composite No " & CStr(iIndex) & vbCrLf)
					zzDispCurves(oCompositeCurve.GetCurves(), sTitle & CStr(iIndex))

				End If
			Next
			Common.GetEditor().WriteMessage("-- - -- -- -- -- -- -- --" & vbCrLf)
		End Sub
		Protected Shared Sub OnInit()

		End Sub
		Protected Sub OnTerminate()
			If doLinkTable IsNot Nothing Then
				doLinkTable.Terminate()
				doLinkTable = Nothing
			End If
			'	If mcolRings IsNot Nothing Then
			'	mcolRings.Clear()
			'	mcolRings.Dispose()
			'	mcolRings = Nothing
			'	End If

			'MessageBox.Show(CStr(moPolygon IsNot Nothing), "01_752")
			
			If dcolLines IsNot Nothing Then
				Try
					dcolLines.Clear()
					dcolLines.Dispose()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - OnTerminate_2")
				End Try
				dcolLines = Nothing
			End If
			If dcolBoundaryObj IsNot Nothing Then
				Try
					dcolBoundaryObj.Clear()
					dcolBoundaryObj.Dispose()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnTopoPgon - OnTerminate")
				End Try
				dcolBoundaryObj = Nothing
			End If

			If doBoundingBox IsNot Nothing Then
				'doBoundingBox.Terminate()
				'	doBoundingBox = Nothing
			End If
			If doaBulgeVertexArray IsNot Nothing Then
				For iIndex As Integer = 0 To doaBulgeVertexArray.GetUpperBound(0)
					If doaBulgeVertexArray(iIndex) IsNot Nothing Then
						doaBulgeVertexArray(iIndex).Terminate()
					End If
				Next
				Erase doaBulgeVertexArray
				doaBulgeVertexArray = Nothing
			End If
		End Sub

		Private Structure NodePair
			Private miFirstNodeID As Integer
			Private miLastNodeID As Integer
			Public Sub New(ByVal iFirstNodeID As Integer, ByVal iLastNodeID As Integer)
				miFirstNodeID = iFirstNodeID
				miLastNodeID = iLastNodeID
			End Sub
			Public Sub New(ByVal oHalfEdge As HalfEdge)
				miFirstNodeID = oHalfEdge.PreviousNode.ID
				miLastNodeID = oHalfEdge.NextNode.ID
			End Sub
			Public ReadOnly Property FirstNodeID() As Integer
				Get
					Return miFirstNodeID
				End Get
			End Property
			Public ReadOnly Property LastNodeID() As Integer
				Get
					Return miLastNodeID
				End Get
			End Property
		End Structure


	End Class
   
End Namespace
