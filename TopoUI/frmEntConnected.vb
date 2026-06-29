Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Imports Autodesk.Gis.Map.Topology
Namespace Expro
	Public Enum enGeometricType
		Undefined
		Point
		Line
		Area
	End Enum
	Public Class frmEntConnected
		Const msLayerPrefix As String = "dm-"
		Const msAuxiliaryLayer As String = "dm-ezer"
		Const miAfterParcelDividerHeight As Integer = 2

		'--- Topology names ---
		Const msParcelTopoName As String = "ParcelsLine"
		Const msExproTopoName As String = "ExproLine"
		Const msExproRealTopoName As String = "ExproReal"
		Const msExproBoundaryTopoName As String = "ExproBoundary"
		Const msEntAreaTopoName As String = "EntAreas"

		Const msUnionTopoName As String = "Union"
		Const msLineTopoName As String = "EntLines"
		Const msLineInTopoName As String = "EntLinesIn"
		Const msPolylineClosedTopoName As String = "PolylineClosed"
		Const msPolylineClosedClipTopoName As String = "PLineArea"






		'--- Layers ---
		Const msEntAreaLinkLayer As String = "EntAreas"
		Const msEntLineLayer As String = "EntLines"
		Const msEntAreas_x_UnionLayer As String = "EntAreasParcel"






		'--- Object Data Tables ---
		Const msUnionODTableName As String = "ODUnion"
		Const msLinesODTableName As String = "ODLines"
		Const msPolylinesClosedODTableName As String = "ODPLinesClosed_"


		Private Shared miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmExpro

		Private mdicLayers As Dictionary(Of String, LayerConnected)
		Private mcolEntPoints As ObjectIdCollection
		Private mcolEntLines As ObjectIdCollection
		Private mcolEntPLinesClosed As ObjectIdCollection

		Private mcolEntAreasLinks As ObjectIdCollection
		Private mcolEntAreasCentroids As ObjectIdCollection

		Private mcolEntConnPoints As System.Collections.ObjectModel.Collection(Of EntityConnected)
		Private Shared mdicParcelExts As Dictionary(Of Integer, ParcelExt)
		Private mdicParcelExproPgons As Dictionary(Of Integer, ParcelExt)
		Private moEntParcels As System.Data.DataTable
		Private msSourceTopoName As String

		'Private sOverlayTopoName As String = msUnionTopoName
		'sOverlayTopoName = "ParcelsLine"


		Private mbSimpleVersion As Boolean = True
		Private mbClosedPgonVersion As Boolean = True


		Public Sub New()

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			zzInitializeComponent()

			If mbSimpleVersion Then
				msSourceTopoName = msParcelTopoName
			Else
				msSourceTopoName = msUnionTopoName
			End If
		End Sub
		Private Sub zzInitializeComponent()
			zzFillGeometricTypes(Me.ccbGeometricType)
		End Sub
		Private Class ResourceData
			Public Shared Function GetGeometricTypeName(iGeometricType As enGeometricType) As String
				Const iGeometricTypeSectionID As Integer = 4
				Return TPlServerDB.TextResource.GetText(iGeometricType, miResourceTheme, iGeometricTypeSectionID, True)
			End Function
		End Class
		Public Structure LayerConnected
			Dim ID As Integer
			Dim Layer As String
			Dim Description As String
			Dim GeometricType As enGeometricType
			Public Index As Integer
			Public Sub New(iID As Integer, sLayer As String, sDescription As String, iGeometricType As enGeometricType)
				ID = iID
				Layer = sLayer
				Description = sDescription
				GeometricType = iGeometricType
			End Sub
			Public Sub New(oDataReader As IDataReader)
				Dim iGeometricType As Integer
				ID = oDataReader.GetInt32(0)
				Layer = oDataReader.GetString(1)
				Description = oDataReader.GetString(2)
				iGeometricType = oDataReader.GetInt32(3)
				If [Enum].IsDefined(GetType(enGeometricType), iGeometricType) Then
					GeometricType = CType(iGeometricType, enGeometricType)
				End If
			End Sub
		End Structure
		Public Class EntityConnected
			Private Structure ParcelFragment
				Public ParcelTopoID As Integer
				Public Value As Double
				Public Sub New(iParcelTopoID As Integer, dValue As Double)
					ParcelTopoID = iParcelTopoID
					Value = dValue
				End Sub
			End Structure
			Private Class ParcelFragmentComparer
				Implements System.Collections.Generic.IComparer(Of ParcelFragment)

				Public Function Compare(oParcelFragmentA As ParcelFragment, oParcelFragmentB As ParcelFragment) As Integer Implements IComparer(Of ParcelFragment).Compare
					Return oParcelFragmentB.Value.CompareTo(oParcelFragmentA.Value)
				End Function
			End Class
			Private mtLayer As LayerConnected
			Private mtAcObjID As ObjectId
			Private miGeometricType As enGeometricType
			Private mtBlockRefPosition As Autodesk.AutoCAD.Geometry.Point3d
			Private miCount As Integer
			Private mdValue As Double
			Private moComparer As IComparer(Of ParcelFragment) = New ParcelFragmentComparer()
			Private mssParcelFragmentSet As SortedSet(Of ParcelFragment) = New SortedSet(Of ParcelFragment)(moComparer)

			Public Sub New()
				miCount = 0
			End Sub
			Public Sub New(iGeometricType As enGeometricType, tLayer As LayerConnected, tAcObjID As ObjectId, Optional dValue As Double = 0.0)
				miGeometricType = iGeometricType
				mtLayer = tLayer
				mtAcObjID = tAcObjID
				miCount = 1
				mdValue = dValue
			End Sub
			Public Sub New(tLayer As LayerConnected, tAcObjID As ObjectId, Optional dValue As Double = 0.0)
				miGeometricType = tLayer.GeometricType
				mtLayer = tLayer
				mtAcObjID = tAcObjID
				miCount = 1
				mdValue = dValue
			End Sub
			Public Sub AddFragment(iParcelID As Integer, dValue As Double)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!6EntLinesIn", iParcelID, dValue, mtAcObjID)
				mssParcelFragmentSet.Add(New ParcelFragment(iParcelID, dValue))
			End Sub
			Public Function GetMaxValueParcel() As Integer
				If mssParcelFragmentSet.Count > 0 Then
					Dim oParcelFragment As ParcelFragment = mssParcelFragmentSet.First
					Return oParcelFragment.ParcelTopoID
				Else
					Return 0
				End If

			End Function
			Public Function GetParcelFragmentCount() As Integer
				Return mssParcelFragmentSet.Count
			End Function
			Public Property BlockRefPosition As Autodesk.AutoCAD.Geometry.Point3d
				Get
					Return mtBlockRefPosition
				End Get
				Set(tValue As Autodesk.AutoCAD.Geometry.Point3d)
					mtBlockRefPosition = tValue
				End Set
			End Property
			Public ReadOnly Property LayerID As Integer
				Get
					Return mtLayer.ID
				End Get
			End Property

			Public ReadOnly Property AcObjID As ObjectId
				Get
					Return mtAcObjID
				End Get
			End Property
			Public ReadOnly Property Layer As LayerConnected
				Get
					Return mtLayer
				End Get
			End Property
			Public ReadOnly Property GeometricType As enGeometricType
				Get
					Return miGeometricType


				End Get
			End Property
			Public ReadOnly Property LayerIndex As Integer
				Get
					Return mtLayer.Index
				End Get
			End Property
			Private Function zzGetGeometricTypeName(iGeometricType As enGeometricType) As String
				Const iGeometricTypeSectionID As Integer = 4

				Return TPlServerDB.TextResource.GetText(iGeometricType, miResourceTheme, iGeometricTypeSectionID, True)

			End Function
			Public ReadOnly Property LayerDescription As String
				Get
					Return mtLayer.Description
				End Get
			End Property

			Public ReadOnly Property Count As Integer
				Get
					Return miCount
				End Get
			End Property
			Public ReadOnly Property Value As Double
				Get
					Return mdValue
				End Get
			End Property
			Public Sub Add(oEntityConnected As EntityConnected)
				If miGeometricType = enGeometricType.Undefined Then
					miGeometricType = oEntityConnected.GeometricType
					mtLayer = oEntityConnected.Layer
					mtAcObjID = oEntityConnected.AcObjID
				End If
				miCount += oEntityConnected.Count
				mdValue += oEntityConnected.Value
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN5", mtLayer.Description, miCount, oEntityConnected.Count)
			End Sub
		End Class
		Private Sub zzFillGeometricTypes(ByRef oComboBoxColumn As DataGridViewComboBoxColumn)

			Dim iaGeometricType() As enGeometricType = DirectCast([Enum].GetValues(GetType(enGeometricType)), enGeometricType())
			Dim oItemData As DMCommon.ItemData

			oComboBoxColumn.ValueMember = DMCommon.ItemData.ValueMember
			oComboBoxColumn.DisplayMember = DMCommon.ItemData.DisplayMember
			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!GeometricType", iaGeometricType)
			For iIndex As Integer = 0 To iaGeometricType.GetUpperBound(0)
				oItemData = New DMCommon.ItemData(iaGeometricType(iIndex), ResourceData.GetGeometricTypeName(iaGeometricType(iIndex)))

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GeometricTypeI", iIndex, iaGeometricType(iIndex), zzGetGeometricTypeName(iaGeometricType(iIndex)))
				oComboBoxColumn.Items.Add(oItemData)
			Next

		End Sub

		Private Class EntConnectedSet
			Inherits Dictionary(Of Integer, EntityConnected)
			Private miParcelID As Integer
			Private mbIn As Boolean
			Private mdArea As Double
			'	Private mdicEntityConnected As Dictionary(Of Integer, EntityConnected)
			Public Sub New(dArea As Double)
				mdArea = dArea
			End Sub
			Public Sub AddPolygon(dArea As Double)
				mdArea += dArea
			End Sub
			Public Sub AddEntityConnected(oEntityConnected As EntityConnected)
				Dim oSumEntityConnected As EntityConnected = Nothing
				If Not MyBase.TryGetValue(oEntityConnected.LayerID, oSumEntityConnected) Then
					oSumEntityConnected = New EntityConnected()
					MyBase.Add(oEntityConnected.LayerID, oSumEntityConnected)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN4a", oSumEntityConnected.Count, oSumEntityConnected.Value, oEntityConnected.LayerDescription)
				Else
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN4b", oSumEntityConnected.Count, oSumEntityConnected.Value, oEntityConnected.LayerDescription)
				End If

				oSumEntityConnected.Add(oEntityConnected)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN4c", oSumEntityConnected.Count)
			End Sub
		End Class
		Public Class ParcelExt
			Private miParcelID As Integer
			Private miBlockNo As Integer
			Private miBlockAdd As Integer
			Private miParcelNo As Integer
			Private mdLegalArea As Double
			Private mbIsAnalytic As Boolean
			Private moInsideEntConnectedSet As EntConnectedSet
			Private moOutsideEntConnectedSet As EntConnectedSet

			Private mcolEntConn As ObjectModel.Collection(Of EntityConnected)
			Private mhsEntConn As HashSet(Of Integer)

			Private mdicPolygons As Dictionary(Of Integer, Boolean)

			Public Sub New(oParcel As TopoManager.TPlanGraph.TplnParcel)
				moInsideEntConnectedSet = New EntConnectedSet(0.0)
				moOutsideEntConnectedSet = New EntConnectedSet(0.0)
				miParcelID = oParcel.TopoID
				miBlockNo = oParcel.BlockNo
				miParcelNo = oParcel.ParcelNo
				mdLegalArea = oParcel.LegalArea(False)
				mdicPolygons = New Dictionary(Of Integer, Boolean)()
				mcolEntConn = New ObjectModel.Collection(Of EntityConnected)()
				mhsEntConn = New HashSet(Of Integer)()
			End Sub
			Public Sub AddPolygon(iParcelExproPgonID As Integer, bIn As Boolean, dArea As Double)
				mdicPolygons.Add(iParcelExproPgonID, bIn)
				If bIn Then
					moInsideEntConnectedSet.AddPolygon(dArea)
				Else
					moOutsideEntConnectedSet.AddPolygon(dArea)
				End If
			End Sub
			Public Sub AddEntityConnected(oEntityConnected As EntityConnected)
				'Dim bIn As Boolean
				If oEntityConnected IsNot Nothing Then
					If Not mhsEntConn.Contains(oEntityConnected.LayerID) Then
						mhsEntConn.Add(oEntityConnected.LayerID)
						mcolEntConn.Add(oEntityConnected)
					End If
					'DMCommon.Debug.ExcelLog.NextRow()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN3", mdicPolygons.Count, oEntityConnected.LayerDescription)
					moInsideEntConnectedSet.AddEntityConnected(oEntityConnected)
				End If

			End Sub

			Public Sub AddEntityConnected(iParcelExproPgonID As Integer, oEntityConnected As EntityConnected)
				Dim bIn As Boolean
				If oEntityConnected IsNot Nothing Then
					If Not mhsEntConn.Contains(oEntityConnected.LayerID) Then
						mhsEntConn.Add(oEntityConnected.LayerID)
						mcolEntConn.Add(oEntityConnected)
					End If
					'	DMCommon.Debug.ExcelLog.NextRow()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN8", mdicPolygons.Count)
					If mdicPolygons.TryGetValue(iParcelExproPgonID, bIn) Then
						If bIn Then
							moInsideEntConnectedSet.AddEntityConnected(oEntityConnected)
						Else
							moOutsideEntConnectedSet.AddEntityConnected(oEntityConnected)
						End If
					Else
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonNotFound", iParcelExproPgonID, oEntityConnected.LayerID, oEntityConnected.LayerDescription)
					End If
				End If
			End Sub

			Public Sub ToTable(ByRef oDataTable As Data.DataTable)
				Dim oEntityConnected As EntityConnected = Nothing
				Dim colLayerIDs As IEnumerable(Of Integer) = From oEntityConn In mcolEntConn
																			Order By oEntityConn.LayerIndex
																			Select oEntityConn.LayerID
				Dim oNewRow As DataRow
				Dim bFirstRow As Boolean = True
				Dim iGeometricType As enGeometricType
				For Each iLayerID As Integer In colLayerIDs
					oNewRow = oDataTable.NewRow()
					If bFirstRow Then
						oNewRow.Item("BlockNo") = miBlockNo
						oNewRow.Item("BlockAddNo") = miBlockAdd
						oNewRow.Item("ParcelNo") = miParcelNo
					End If

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!2LayerID", iLayerID, moInsideEntConnectedSet.ContainsKey(iLayerID), moOutsideEntConnectedSet.ContainsKey(iLayerID), GetEntConnectedCount(True, iLayerID))

					If moInsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
						'DMCommon.Debug.ExcelLog.SetValue(2, "", oEntityConnected.GeometricType, oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)

						oNewRow.Item("LayerDescription") = oEntityConnected.LayerDescription
						oNewRow.Item("GeometricType") = oEntityConnected.GeometricType
						iGeometricType = oEntityConnected.GeometricType
						oNewRow.Item("GeometricTypeName") = ResourceData.GetGeometricTypeName(iGeometricType)
						oNewRow.Item("EntCountIn") = oEntityConnected.Count
						If oEntityConnected.Value >= 0.001 Then
							oNewRow.Item("EntValueIn") = oEntityConnected.Value

							'moEntParcels.Columns.Add("EntCountIn", GetType(System.Int32))
							'moEntParcels.Columns.Add("EntValueIn", GetType(System.Double))

						End If
					End If


					If moOutsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
						'DMCommon.Debug.ExcelLog.SetValue(6, "", oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)
						oNewRow.Item("LayerDescription") = oEntityConnected.LayerDescription
						oNewRow.Item("EntCountOut") = oEntityConnected.Count
						If oEntityConnected.Value >= 0.001 Then
							oNewRow.Item("EntValueOut") = oEntityConnected.Value
						End If
					End If
					oDataTable.Rows.Add(oNewRow)
					bFirstRow = False
				Next
			End Sub

			Public Sub ExproLuseReport_0321(ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, iFirstColumn As Integer)   '_310520
				Const sEntConnectedIsNothing As String = "אין מחוברים בשטח חלקה זו"
				Dim oEntityConnected As EntityConnected = Nothing

				Dim colLayerIDs As IEnumerable(Of Integer) = From oEntityConn In mcolEntConn
																			Order By oEntityConn.LayerIndex
																			Select oEntityConn.LayerID
				Dim iFirstRow As Integer = iCurrentRow
				'	oExcelAppExt.SetValueInRow(iCurrentRow, 20, colLayerIDs.Count, BlockNo, ParcelNo, "AAA", "BBB")
				If colLayerIDs.Count = 0 Then
					oExcelAppExt.SetValueInRow(iCurrentRow, iFirstColumn, sEntConnectedIsNothing)
					oExcelAppExt.TextBox.SetValuesToRow(iCurrentRow, iFirstColumn, sEntConnectedIsNothing)
				Else
					For Each iLayerID As Integer In colLayerIDs
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!1LayerID", iLayerID, moInsideEntConnectedSet.ContainsKey(iLayerID), moOutsideEntConnectedSet.ContainsKey(iLayerID))
						If moInsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
							'DMCommon.Debug.ExcelLog.SetValue(2, "", oEntityConnected.GeometricType, oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)
							oExcelAppExt.SetValueInRow(iCurrentRow, iFirstColumn, oEntityConnected.LayerDescription)
							oExcelAppExt.TextBox.SetValuesToRow(iCurrentRow, iFirstColumn, oEntityConnected.LayerDescription)
							If oEntityConnected.GeometricType = enGeometricType.Point Then
								oExcelAppExt.SetValueInRow(iCurrentRow, iFirstColumn + 2, oEntityConnected.Count)
								oExcelAppExt.TextBox.SetValuesToRow(iCurrentRow, iFirstColumn + 2, oEntityConnected.Count)

							Else
								oExcelAppExt.SetValueInRow(iCurrentRow, iFirstColumn + 1, Math.Round(oEntityConnected.Value, 0))
								oExcelAppExt.TextBox.SetValuesToRow(iCurrentRow, iFirstColumn + 1, Math.Round(oEntityConnected.Value, 0))

							End If
							'	oExcelAppExt.SetValueInRow(iCurrentRow, 20, miBlockNo, miParcelNo, colLayerIDs.Count)

							iCurrentRow += 1
						End If

						If moOutsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
							'	DMCommon.Debug.ExcelLog.SetValue(6, "", oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)
						End If

					Next

				End If

			End Sub
			Public Function GetEntConnectedCount(bIn As Boolean, iLayerID As Integer) As Integer
				Dim oEntConnectedSet As EntConnectedSet
				Dim oResEntConnected As EntityConnected = Nothing
				If bIn Then
					oEntConnectedSet = moInsideEntConnectedSet
				Else
					oEntConnectedSet = moOutsideEntConnectedSet
				End If
				If oEntConnectedSet IsNot Nothing Then
					If oEntConnectedSet.TryGetValue(iLayerID, oResEntConnected) Then
						Return oResEntConnected.Count
					End If
				End If
				Return 0
			End Function
			Public Sub ToExcel()
				Dim oEntityConnected As EntityConnected = Nothing
				Dim colLayerIDs As IEnumerable(Of Integer) = From oEntityConn In mcolEntConn
																			Order By oEntityConn.LayerIndex
																			Select oEntityConn.LayerID
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ToExcel", mcolEntConn.Count, colLayerIDs.Count, moInsideEntConnectedSet.Count, moOutsideEntConnectedSet.Count)
				For Each iLayerID As Integer In colLayerIDs
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LayerID", iLayerID, moInsideEntConnectedSet.ContainsKey(iLayerID), moOutsideEntConnectedSet.ContainsKey(iLayerID))
					If moInsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
						DMCommon.Debug.ExcelLog.SetValue(2, "", oEntityConnected.GeometricType, oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)
					End If

					If moOutsideEntConnectedSet.TryGetValue(iLayerID, oEntityConnected) Then
						'DMCommon.Debug.ExcelLog.SetValue(6, "", oEntityConnected.LayerDescription, oEntityConnected.Count, oEntityConnected.Value)

					End If
					'DMCommon.Debug.ExcelLog.NextRow()
				Next
			End Sub
			Public ReadOnly Property BlockNo As Integer
				Get
					Return miBlockNo
				End Get
			End Property
			Public ReadOnly Property BlockAdd As Integer
				Get
					Return miBlockAdd
				End Get
			End Property
			Public ReadOnly Property ParcelNo As Integer
				Get
					Return miParcelNo
				End Get
			End Property
			Public ReadOnly Property ParcelID As Integer
				Get
					Return miParcelID
				End Get
			End Property

		End Class
		Public Shared ReadOnly Property ParcelExts As Dictionary(Of Integer, ParcelExt)
			Get
				Return mdicParcelExts
			End Get
		End Property
		Private Sub zzLoadModelSpace()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
			Dim colAllDBObjects As DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
			Dim oEntity As Entity
			Dim sLayer As String
			Dim sRXClassName As String

			Dim oCurve As Curve
			'	Dim bClosed As Boolean
			'	Dim bIsReallyClosing As Boolean
			'	Dim tStartPoint As Autodesk.AutoCAD.Geometry.Point3d
			'	Dim tEndPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d

			'Dim bStartEqualToEnd As Boolean
			Dim oLayerConnected As LayerConnected = Nothing
			Dim bDBLayerExists As Boolean
			Dim iGeometricType As enGeometricType
			Dim oEntityConnected As EntityConnected

			mcolEntPoints = New ObjectIdCollection()
			mcolEntLines = New ObjectIdCollection()
			'mcolEntAreas = New ObjectIdCollection()
			mcolEntPLinesClosed = New ObjectIdCollection()
			mcolEntAreasLinks = New ObjectIdCollection()
			mcolEntAreasCentroids = New ObjectIdCollection()


			mcolEntConnPoints = New ObjectModel.Collection(Of EntityConnected)()

			For Each oDBObject As DBObject In colAllDBObjects
				oEntity = TryCast(oDBObject, Entity)
				sLayer = oEntity.Layer
				sRXClassName = oDBObject.GetRXClass().Name
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadMS2", mcolEntConnPoints.Count)
				bDBLayerExists = mdicLayers.TryGetValue(sLayer, oLayerConnected)
				If bDBLayerExists Then
					iGeometricType = oLayerConnected.GeometricType
				Else
					iGeometricType = enGeometricType.Undefined
					If sLayer.StartsWith(msLayerPrefix, System.StringComparison.InvariantCultureIgnoreCase) Then
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LayerNotFound", sLayer)
					End If
				End If

				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName, DMAcadExt.AcadConst.AcadArcName
						oCurve = TryCast(oDBObject, Curve)
						If sLayer = msEntAreaLinkLayer Then
							mcolEntAreasLinks.Add(oDBObject.ObjectId)
						ElseIf bDBLayerExists Then

							If iGeometricType = enGeometricType.Line Then
								mcolEntLines.Add(oDBObject.ObjectId)
							ElseIf iGeometricType = enGeometricType.Area AndAlso sRXClassName = DMAcadExt.AcadConst.AcadPolylineName Then
								mcolEntPLinesClosed.Add(oDBObject.ObjectId)
							End If
						Else

						End If
					Case DMAcadExt.AcadConst.AcadBlockRefName
						If bDBLayerExists Then
							tInsertPoint = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(oDBObject.ObjectId)
							oEntityConnected = New EntityConnected(oLayerConnected, oDBObject.ObjectId)
							oEntityConnected.BlockRefPosition = tInsertPoint

							If oLayerConnected.GeometricType = enGeometricType.Point Then
								mcolEntPoints.Add(oDBObject.ObjectId)
								mcolEntConnPoints.Add(oEntityConnected)
							ElseIf oLayerConnected.GeometricType = enGeometricType.Area Then
								mcolEntAreasCentroids.Add(oDBObject.ObjectId)
							Else

							End If

						Else

						End If

				End Select
			Next
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadModelSpace", mcolEntAreasLinks.Count, mcolEntLines.Count, mcolEntPLinesClosed.Count, mcolEntPoints.Count, mcolEntConnPoints.Count, mcolEntAreasCentroids.Count)
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub

		Private Sub zzLoadModelSpaceOld()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
			Dim colAllDBObjects As DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
			Dim oEntity As Entity
			Dim sLayer As String
			Dim sRXClassName As String
			'Dim oPolyLine As Polyline
			'	Dim oPolyline2d As Polyline2d
			'	Dim oLine As Line
			'	Dim oArc As Arc
			Dim oCurve As Curve
			Dim bClosed As Boolean
			Dim bIsReallyClosing As Boolean
			Dim tStartPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim tEndPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d

			Dim bStartEqualToEnd As Boolean
			Dim oLayerConnected As LayerConnected = Nothing
			Dim bDBLayerExists As Boolean
			Dim tGeoType As enGeometricType
			Dim oEntityConnected As EntityConnected

			mcolEntPoints = New ObjectIdCollection()
			mcolEntLines = New ObjectIdCollection()
			'mcolEntAreas = New ObjectIdCollection()

			mcolEntAreasLinks = New ObjectIdCollection()
			mcolEntAreasCentroids = New ObjectIdCollection()


			mcolEntConnPoints = New ObjectModel.Collection(Of EntityConnected)()
			For Each oDBObject As DBObject In colAllDBObjects
				oEntity = TryCast(oDBObject, Entity)
				sLayer = oEntity.Layer
				sRXClassName = oDBObject.GetRXClass().Name
				If sLayer = msEntAreaLinkLayer Then
					mcolEntAreasLinks.Add(oDBObject.ObjectId)
				ElseIf sLayer <> msAuxiliaryLayer Then   'sLayer.StartsWith(msLayerPrefix) AndAlso 
					bDBLayerExists = mdicLayers.TryGetValue(sLayer, oLayerConnected)
					If bDBLayerExists Then
						tGeoType = oLayerConnected.GeometricType
					Else
						tGeoType = enGeometricType.Undefined
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LayerNotFound", sLayer)
					End If
					Select Case sRXClassName
						Case DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName, DMAcadExt.AcadConst.AcadArcName
							oCurve = TryCast(oDBObject, Curve)
							bClosed = oCurve.Closed
							bIsReallyClosing = oCurve.IsReallyClosing
							tStartPoint = oCurve.StartPoint
							tEndPoint = oCurve.EndPoint
							bStartEqualToEnd = tStartPoint.IsEqualTo(tEndPoint)
							If bDBLayerExists Then
								If (bClosed OrElse bStartEqualToEnd) AndAlso oLayerConnected.GeometricType = enGeometricType.Area Then
									'	mcolEntAreas.Add(oDBObject.ObjectId)
								ElseIf (Not bClosed AndAlso Not bStartEqualToEnd) AndAlso oLayerConnected.GeometricType = enGeometricType.Line Then
									'	mcolEntLines.Add(oDBObject.ObjectId)
								Else

								End If
							Else

							End If
						Case DMAcadExt.AcadConst.AcadBlockRefName
							If bDBLayerExists AndAlso oLayerConnected.GeometricType = enGeometricType.Point Then
								tInsertPoint = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(oDBObject.ObjectId)
								mcolEntPoints.Add(oDBObject.ObjectId)
								oEntityConnected = New EntityConnected(enGeometricType.Point, oLayerConnected, oDBObject.ObjectId)
								oEntityConnected.BlockRefPosition = tInsertPoint
								mcolEntConnPoints.Add(oEntityConnected)
							Else

							End If
						Case Else
							bClosed = False
							bIsReallyClosing = False
							bStartEqualToEnd = False
					End Select

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Entity", oDBObject.Handle.Value, oDBObject.Handle.ToString(), sLayer, sRXClassName, tGeoType, bClosed, bStartEqualToEnd, bDBLayerExists, mcolEntPoints.Count, mcolEntLines.Count, mcolEntConnPoints.Count, mcolEntAreas.Count)
				End If
			Next

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzLoadGlossary()
			Dim sComText As String = "SELECT * FROM EntConnectedLayers ORDER BY Description"
			Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			Dim oLayerConnected As LayerConnected
			Dim iIndex As Integer
			If oDataReader IsNot Nothing Then
				mdicLayers = New Dictionary(Of String, LayerConnected)()
				While oDataReader.Read

					oLayerConnected = New LayerConnected(oDataReader)
					If Not mdicLayers.ContainsKey(oLayerConnected.Layer) Then
						mdicLayers.Add(oLayerConnected.Layer, oLayerConnected)
						iIndex += 1
					End If

				End While

				oDataReader.Close()
			End If
		End Sub

		Private Sub Button1_Click(oSender As System.Object, e As EventArgs)
			Dim sExproTopoName As String = TopoManager.TPlanGraph.TplnExpro.GetTopoName()
			Dim dicExpros As TopoManager.TPlanGraph.TplnExpros = TopoManager.TPlanGraph.TplnProject.Expros
			Dim sTest As String = "0"
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sExproTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			If oExproTopology IsNot Nothing AndAlso dicExpros IsNot Nothing Then


				For Each oExpro As TopoManager.TPlanGraph.TplnExpro In dicExpros.Values
					If oExpro.ExproExists Then
						Try
							sTest = "b"
							oExproTopology.DeletePolygon(oExpro.TopoID)
							sTest = "a"
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DeletePolygon " & sTest, False)
						End Try

					End If
				Next

				oExproTopology.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub

		Private Sub Button2_Click(oSender As System.Object, e As EventArgs)
			Dim sExproTopoName As String = TopoManager.TPlanGraph.TplnExpro.GetTopoName()
			Dim dicExpros As TopoManager.TPlanGraph.TplnExpros = TopoManager.TPlanGraph.TplnProject.Expros
			Dim sTest As String = "0"
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel

			oProject = oMapApplication.ActiveProject



			sExproTopoName = "ExproLine"
			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sExproTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			If oExproTopology IsNot Nothing Then


				For Each oExpro As Autodesk.Gis.Map.Topology.Polygon In oExproTopology.GetPolygons()


					Try
						sTest = "b"
						oExproTopology.DeletePolygon(oExpro.ID)
						sTest = "a"
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DeletePolygon " & sTest, False)
					End Try

					Exit For
				Next

				oExproTopology.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub

		Private Sub cmdDissolve_Click(sender As System.Object, e As EventArgs) Handles cmdDissolve.Click
			Me.Cursor = Cursors.WaitCursor


			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			'	Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			'	Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
			'	Dim iPgonID As Integer
			Dim sLayerName As String = "TplnExproBoundary"
			Dim sBlockName As String = "MRK"

			Dim sAttributeExpression As String = ".LAYER"
			Dim hsExproPgons As HashSet(Of Integer) = zzGetExpros(2)
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(sLayerName, 0)
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings(sLayerName, 256, True, sBlockName)
			Dim i As Integer
			TopoManager.TopoCreator.CopyTopology(msExproTopoName, msExproRealTopoName)



			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msExproRealTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			If oExproTopology IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("231220_2a", oExproTopology.Status, oExproTopology.IsComplete, oExproTopology.NeedsRefresh, oExproTopology.Type, hsExproPgons.Count)



				For Each iPgonID As Integer In hsExproPgons
					Try
						i += 1
						oExproTopology.DeletePolygon(iPgonID)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DeletePolygon " & iPgonID.ToString() & "," & i.ToString, False)
					End Try
				Next

				'	DMCommon.Debug.MsgBox("231220_3", oExproTopology.GetPolygons().Count, hsExproPgons.Count)
				If oEdgeCreationSettings IsNot Nothing Then
					oExproTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
				Else
					DMCommon.Debug.UserMsg("Err #2711", sLayerName)
				End If

				If oCentroidCreationSettings IsNot Nothing Then
					Try
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!CentroidCreationSettings", sLayerName, sBlockName)

						oExproTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!SetCentroidCreationSettings", sAttributeExpression, msExproBoundaryTopoName)
					End Try

				Else
					DMCommon.Debug.UserMsg("Err #2712", sLayerName, sBlockName)
				End If

				Try
					oExproTopology.Dissolve(sAttributeExpression, msExproBoundaryTopoName)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!Dissolve", sAttributeExpression, msExproBoundaryTopoName)

				End Try

				oExproTopology.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End Sub


		Private Function zzGetExpros(iExproType As Integer) As HashSet(Of Integer)
			Dim dicResult As HashSet(Of Integer) = New HashSet(Of Integer)()
			'	TopoManager.TPlanGraph.TplnProject.LoadExpros()
			'DMCommon.Debug.MsgBox("080421_1", TopoManager.TPlanGraph.TplnProject.Expros.Values.Count)
			For Each oExpro As TopoManager.TPlanGraph.TplnExpro In TopoManager.TPlanGraph.TplnProject.Expros.Values
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproA ", oExpro.ExproTypeID, iExproType, oExpro.TopoID, dicResult.Count)


				If oExpro.ExproTypeID = iExproType Then
					dicResult.Add(oExpro.TopoID)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproD ", oExpro.ExproTypeID, iExproType, oExpro.TopoID, dicResult.Count)
				End If
			Next
			Return dicResult
		End Function

		Private Sub cmdUnion_Click(oSender As System.Object, e As EventArgs) Handles cmdUnion.Click
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
			'	Dim iPgonID As Integer = 171
			oProject = oMapApplication.ActiveProject
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 256, True, "MRK")
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings("ExproParcels", 0)
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 0, False, "")

			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msUnionODTableName
			'	Dim sParcelTopoName As String = "ParcelsLine"
			Dim oParcelTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			'	Dim sExproTopoName As String = "ExproLine"

			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msExproBoundaryTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			Dim colOverlayData As Autodesk.Gis.Map.Topology.OverlayDataCollection = New Autodesk.Gis.Map.Topology.OverlayDataCollection()

			Dim iCodeRow As Integer = 0
			If oParcelTopology IsNot Nothing AndAlso oExproTopology IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("231220_2a", oExproTopology.Status, oExproTopology.IsComplete, oExproTopology.NeedsRefresh, oExproTopology.Type)
				Try
					iCodeRow = 1
					oParcelTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
					iCodeRow = 2
					oParcelTopology.SetNodeCreationSettings(oNodeCreationSettings)
					iCodeRow = 3
					oParcelTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
					iCodeRow = 4
					oParcelTopology.Union(oExproTopology, msUnionTopoName, "", tResultODTable)
					'oParcelTopology.Union(oExproTopology, "Union", Nothing, Nothing, colOverlayData)


				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "cmdUnion " & "CodeRow=" & iCodeRow.ToString(), False)
					DMCommon.Debug.MsgBox("!Union4", oParcelTopology.Name, oExproTopology.Name, msUnionTopoName, tResultODTable.ODTableName)
				End Try


				'DMCommon.Debug.MsgBox("231220_3", oParcelTopology.GetPolygons().Count)
				oParcelTopology.Close()
				oExproTopology.Close()

			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdClip_Click(oSender As System.Object, e As EventArgs) Handles cmdClip.Click
			Dim hExternalPolygons As HashSet(Of Integer) = zzGetExternalPgons()
			Dim sUnionTopoName As String = "Union"
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
			Dim i As Integer
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()

			oProject = oMapApplication.ActiveProject
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!DelPgon", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~", "~~~~~~")
			Dim oUnionTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sUnionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, True, False)
			If oUnionTopology IsNot Nothing Then
				For Each iPgonID As Integer In hExternalPolygons
					Try
						i += 1
						DMAcadExt.AcadDocument.WriteMessage("Prev " & CStr(i) & " , " & CStr(iPgonID))
						oUnionTopology.DeletePolygon(iPgonID)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!DeletedPolygon", iPgonID)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ErrPolygon", iPgonID)
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "DeletePolygon " & iPgonID.ToString() & "," & i.ToString, False)
					End Try
				Next
				oUnionTopology.Close()
			End If

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub
		Private Function zzGetExternalPgons() As HashSet(Of Integer)
			Dim sParcelTopoName As String = "ParcelsLine"
			Dim sExproTopoName As String = "ExproBoundary"
			Dim sResultODTableName As String = "ODUnion"
			Dim sUnionTopoName As String = "Union"
			Dim hExternalPolygons As HashSet(Of Integer) = New HashSet(Of Integer)()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMCommon.Debug.MsgBox("020321_0", "Step 0")
			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(sParcelTopoName, sExproTopoName, sResultODTableName)
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			Dim colPolygons As PolygonCollection

			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			'	Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
			'Dim iPgonID As Integer = 171
			'oProject = oMapApplication.ActiveProject
			DMCommon.Debug.MsgBox("020321_1", "Step 1")
			Dim oUnionTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sUnionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oUnionTopology IsNot Nothing Then
				colPolygons = oUnionTopology.GetPolygons()
				DMCommon.Debug.MsgBox("020321_1a", "Step 2", colPolygons.Count)
				For Each oUnionPgon As Autodesk.Gis.Map.Topology.Polygon In colPolygons

					oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oUnionPgon.Entity)
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!UnionPgon.ID", oUnionPgon.ID, oOverlayODRecord.SourceAreaPcnt)
					If oOverlayODRecord.SourceAreaPcnt = 100.0 Then
						hExternalPolygons.Add(oUnionPgon.ID)
					End If
				Next
				colPolygons.Dispose()
				oUnionTopology.Close()
				'DMCommon.Debug.MsgBox("250221_1", hExternalPolygons.Count)
			Else
				DMCommon.Debug.MsgBox("020321_2", " oUnionTopology Is Nothing")
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMCommon.Debug.MsgBox("250221_3", hExternalPolygons.Count)
			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!ExternalPolygons", hExternalPolygons)
			Return hExternalPolygons

		End Function
		Private Sub zzLoadParcelSimpleVersion()
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()

			Dim oParcelTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)


			If oParcelTopology IsNot Nothing Then
				If mdicParcelExproPgons Is Nothing Then
					mdicParcelExproPgons = New Dictionary(Of Integer, ParcelExt)()
				Else
					mdicParcelExproPgons.Clear()
				End If

				If mdicParcelExts Is Nothing Then
					mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
				Else
					mdicParcelExts.Clear()
				End If


			End If

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub
		Private Sub zzLoadParcelExproUnion()
			'		Dim sParcelTopoName As String = "ParcelsLine"
			Dim sExproTopoName As String = "ExproBoundary"
			Dim sResultODTableName As String = "ODUnion"
			Dim sUnionTopoName As String = "Union"
			Dim hExternalPolygons As HashSet(Of Integer) = New HashSet(Of Integer)()
			Dim oParcelExt As ParcelExt = Nothing
			Dim iParcelID As Integer
			Dim iParcelExproPgonID As Integer
			Dim iExproPgonID As Integer
			Dim bInside As Boolean
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()

			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(msParcelTopoName, sExproTopoName, sResultODTableName)
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			Dim colPolygons As PolygonCollection
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			'oProject = oMapApplication.ActiveProject
			'	DMCommon.Debug.MsgBox("180321_1", "Step 1")
			Dim oUnionTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sUnionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oUnionTopology IsNot Nothing Then
				If mdicParcelExproPgons Is Nothing Then
					mdicParcelExproPgons = New Dictionary(Of Integer, ParcelExt)()
				Else
					mdicParcelExproPgons.Clear()
				End If

				If mdicParcelExts Is Nothing Then
					mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
				Else
					mdicParcelExts.Clear()
				End If
				colPolygons = oUnionTopology.GetPolygons()
				'	DMCommon.Debug.MsgBox("020321_1", "Step 2", colPolygons.Count)
				For Each oUnionPgon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
					oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oUnionPgon.Entity)

					iParcelExproPgonID = oOverlayODRecord.NewID
					iParcelID = oOverlayODRecord.SourceID
					iExproPgonID = oOverlayODRecord.OverlayID
					bInside = (iExproPgonID <> 0)

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!UnionPgon.ID", iParcelExproPgonID, iParcelID, iExproPgonID, oOverlayODRecord.SourceAreaPcnt, oOverlayODRecord.OverlayAreaPcnt, oUnionPgon.ID)

					If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
						oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
						If oParcel IsNot Nothing Then
							oParcelExt = New ParcelExt(oParcel)
							mdicParcelExts.Add(iParcelID, oParcelExt)
						End If

					End If
					If mdicParcelExproPgons.ContainsKey(iParcelExproPgonID) Then
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!DblKey", iParcelExproPgonID, mdicParcelExproPgons.Count)
					Else
						mdicParcelExproPgons.Add(iParcelExproPgonID, oParcelExt)
					End If

					Try
						oParcelExt.AddPolygon(iParcelExproPgonID, bInside, oUnionPgon.Area)
					Catch oEx As Exception

					End Try
				Next
				colPolygons.Dispose()
				oUnionTopology.Close()
			Else
				DMCommon.Debug.MsgBox("020321_2", " oUnionTopology Is Nothing")
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadParcelExproUnion", mdicParcelExts.Count, mdicParcelExproPgons.Count)


		End Sub
		Public Sub Calculate()
			zzLoadGlossary()
			zzLoadModelSpace()
			zzLoadParcelExproUnion()
			zzLoadEntPoints()
			zzLoadEntLines()

			zzLoadEntAreas()

		End Sub
		Private Sub cmdLoadModel_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadModel.Click, cmdLoadEnt.Click
			Me.Cursor = Cursors.WaitCursor
			zzLoadGlossary()
			zzLoadModelSpace()
			If mdicParcelExts Is Nothing Then
				mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
			Else
				mdicParcelExts.Clear()
			End If

			Me.Cursor = Cursors.Default
		End Sub

		Private Sub zzCreateEntTopos()
			Const sPointTopoName As String = "EntNodes"
			'	Const sLineTopoName As String = "EntLines"
			Const sAreaTopoName As String = "EntPolygons"

			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim colEmpty As ObjectIdCollection = New ObjectIdCollection()
			Dim colEmpty1 As ObjectIdCollection = New ObjectIdCollection()
			Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies

			If oTopos.Exists(sPointTopoName) Then
				oTopos.Delete(sPointTopoName, False)
			End If
			DMCommon.Debug.MsgBox("!zzCreateEntTopos", sPointTopoName, mcolEntPoints.Count)
			If mcolEntPoints.Count > 0 Then
				Try
					oTopos.Create(sPointTopoName, colEmpty, mcolEntPoints, colEmpty, TopologyTypes.Point)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!CreateNodeTopology: " & sPointTopoName)
				End Try
			End If


			If oTopos.Exists(msLineTopoName) Then
				oTopos.Delete(msLineTopoName, False)
			End If
			DMCommon.Debug.MsgBox("!zzCreateLinesTopos", msLineTopoName, mcolEntLines.Count)
			If mcolEntLines.Count > 0 Then
				Try
					oTopos.Create(msLineTopoName, mcolEntLines, colEmpty, colEmpty, TopologyTypes.Linear)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!CreateLinearTopology: " & msLineTopoName)
				End Try
			End If


			If mcolEntAreasLinks.Count > 0 Then
				If oTopos.Exists(msEntAreaTopoName) Then
					oTopos.Delete(msEntAreaTopoName, False)
				End If
				Try
					oTopos.Create(msEntAreaTopoName, mcolEntAreasLinks, colEmpty, mcolEntAreasCentroids, TopologyTypes.Polygon)
				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!CreatePolygonTopology: " & sAreaTopoName)
				End Try
			End If

		End Sub

		Private Sub cmdTopo_Click(oSender As System.Object, e As EventArgs) Handles cmdTopo.Click
			Me.Cursor = Cursors.WaitCursor
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			zzCreateEntTopos()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			Me.Cursor = Cursors.Default
		End Sub



		Private Sub cmdParcExMapLayer_Click(oSender As System.Object, e As EventArgs) Handles cmdParcExMapLayer.Click
			Me.Cursor = Cursors.WaitCursor
			Const sPrefix As String = "shp_"
			Dim taColor As System.Drawing.Color() = {New System.Drawing.Color(), System.Drawing.Color.FromArgb(0, 0, 255), System.Drawing.Color.FromArgb(0, 255, 0)}
			Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
			Dim sTopoName As String = "Union"
			Dim iGraphType As DMAcadExt.enGraphType = DMAcadExt.enGraphType.Topology
			Dim sFeatureClass As String = sTopoName
			Dim sShapeConnection As String = sPrefix & sTopoName
			Dim iIndex As Integer = 1
			oFDO_Manager.CreateMapLayer(sTopoName, iGraphType, sFeatureClass, sShapeConnection, CStr(iIndex), taColor(iIndex))
			DMAcadExt.AcadDocument.UpdateScreen()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdEntAreaMapLayer_Click(oSender As System.Object, e As EventArgs) Handles cmdEntAreaMapLayer.Click
			Const sPrefix As String = "shp_"
			Me.Cursor = Cursors.WaitCursor
			Dim taColor As System.Drawing.Color() = {New System.Drawing.Color(), System.Drawing.Color.FromArgb(0, 0, 255), System.Drawing.Color.FromArgb(0, 255, 0)}
			Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
			Dim sTopoName As String = "EntAreas"
			Dim iGraphType As DMAcadExt.enGraphType = DMAcadExt.enGraphType.Topology
			Dim sFeatureClass As String = sTopoName
			Dim sShapeConnection As String = sPrefix & sTopoName
			Dim iIndex As Integer = 2
			oFDO_Manager.CreateMapLayer(sTopoName, iGraphType, sFeatureClass, sShapeConnection, CStr(iIndex), taColor(iIndex))
			DMAcadExt.AcadDocument.UpdateScreen()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdIntersect_Click(oSender As System.Object, e As EventArgs) Handles cmdIntersect.Click
			Me.Cursor = Cursors.WaitCursor
			zzUnion()
			Me.Cursor = Cursors.Default
		End Sub
		Private Sub zzUnion()
			Dim sSourceLayer As String, sOverlayLayer As String, sResultLayer As String
			Dim sOverlayLayer_A As String = Nothing, sResultLayer_A As String = Nothing
			Dim oFDO_Manager As FDO.FDO_Manager = New FDO.FDO_Manager()
			Dim oMySettings As My.MySettings = New My.MySettings()

			Dim bAllSlivers As Boolean = oMySettings.AllSlivers
			Dim dMinFDOSliverTolerance, dMaxFDOSliverTolerance As Double
			Dim sSHPFileName As String

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

			If bAllSlivers Then
				dMinFDOSliverTolerance = 0.0
				dMaxFDOSliverTolerance = -1.0
			Else
				'	dMinFDOSliverTolerance = MinFDOSliverTolerance
				dMinFDOSliverTolerance = oMySettings.MinFDOSliverTolerance
				dMaxFDOSliverTolerance = oMySettings.MaxFDOSliverTolerance
			End If

			sSourceLayer = "Union"
			sOverlayLayer = "EntAreas"
			sResultLayer = "EntAreasParcel"




			sSHPFileName = oFDO_Manager.Union(sSourceLayer, sOverlayLayer, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			DMCommon.Debug.MsgBox("zzUN 142by", sSourceLayer, sOverlayLayer, sResultLayer_A, sOverlayLayer_A, sResultLayer, dMinFDOSliverTolerance, dMaxFDOSliverTolerance)
			If sSHPFileName IsNot Nothing Then

				Dim oFileInfo As IO.FileInfo = New IO.FileInfo(sSHPFileName)
				Dim saParseVal() As String = Strings.Split(oFileInfo.Name, ".")
				Dim sFCName As String = saParseVal(0)


				Dim iResPgonCount As Integer = oFDO_Manager.DBF2XDataTopo(sSHPFileName, TopoManager.TPlanGraph.TplnProject.XDataAppName, False, False)


			Else

			End If
			DMAcadExt.AcadDocument.UpdateScreen()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzRemoveAllLayers()

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			'	System.Windows.Forms.MessageBox.Show(oTopoDef.FDOConnectionName & vbCrLf & oTopoDef.FDOLayerName, "zzClearMapLayer - 01_821")
			'	FDO.FDO_Manager.RemoveConnectionB(oMapThemeData.ShapeConnection)
			FDO.FDO_Manager.RemoveAllResources()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub
		Private Sub cmdClearMapLayers_Click(oSender As System.Object, e As EventArgs) Handles cmdClearMapLayers.Click

			Me.Cursor = Cursors.WaitCursor
			zzRemoveAllLayers()
			Me.Cursor = Cursors.Default

		End Sub

		Private Sub cmdLoad_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadEntPoints.Click
			Me.Cursor = Cursors.WaitCursor
			zzLoadEntPoints()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub zzLoadEntPointsSimpleVersion()
			'	Dim sUnionTopoName As String = "Union"
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim sTest As String
			If oSourceTopology IsNot Nothing Then
				Dim oLinkTable As DMAcadExt.ODTable
				'Dim sTableName As String = "ODUnion"
				Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
				Dim oParcelExt As ParcelExt = Nothing
				Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon

				'	Dim iParcelTopoID As Integer
				oLinkTable = New DMAcadExt.ODTable(msUnionODTableName)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN0", mcolEntConnPoints.Count)
				For Each oEntityConnected As EntityConnected In mcolEntConnPoints
					Try
						oPolygon = oSourceTopology.FindPolygon(oEntityConnected.BlockRefPosition)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN1", oPolygon IsNot Nothing)
						If oPolygon IsNot Nothing Then

							If Not mdicParcelExts.TryGetValue(oPolygon.ID, oParcelExt) Then
								oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(oPolygon.ID)
								oParcelExt = New ParcelExt(oParcel)
								sTest = "Not Exists"
								mdicParcelExts.Add(oPolygon.ID, oParcelExt)
							Else
								sTest = "Yes Exists"
							End If
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, oParcelExt.BlockNo, oParcelExt.ParcelNo, oEntityConnected.LayerDescription, mdicParcelExts.Count)


							oParcelExt.AddEntityConnected(oEntityConnected)

							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN5", oParcelExt.BlockNo, oParcelExt.ParcelNo, oParcelExt.GetEntConnectedCount(True, oEntityConnected.LayerID))
						Else
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntConn not found", oEntityConnected.BlockRefPosition)
						End If

					Catch oEx As Exception

					End Try

				Next
				oLinkTable.Terminate()
			End If

		End Sub
		Private Sub zzLoadEntPoints()
			If mbSimpleVersion Then
				zzLoadEntPointsSimpleVersion()
			Else
				zzLoadEntPointsBaseVersion()
			End If
		End Sub

		Private Sub zzLoadEntPointsBaseVersion()
			'	Dim sUnionTopoName As String = "Union"
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			'DMCommon.Debug.ExcelLog.SetNextValue(4, "!zzLoadEntPoints", "---", "---", "---", "---", "---", "---")
			If oSourceTopology IsNot Nothing Then
				Dim oLinkTable As DMAcadExt.ODTable
				'Dim sTableName As String = "ODUnion"
				Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing

				Dim oParcelExt As ParcelExt = Nothing
				Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
				'	Dim iParcelTopoID As Integer
				oLinkTable = New DMAcadExt.ODTable(msUnionODTableName)



				For Each oEntityConnected As EntityConnected In mcolEntConnPoints
					Try
						oPolygon = oSourceTopology.FindPolygon(oEntityConnected.BlockRefPosition)
						If oPolygon IsNot Nothing Then
							If mdicParcelExproPgons.TryGetValue(oPolygon.ID, oParcelExt) Then
								oParcelExt.AddEntityConnected(oPolygon.ID, oEntityConnected)

							Else
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Parcel not found_Pnt", oPolygon.ID)
							End If
						Else
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntConn not found", oEntityConnected.BlockRefPosition)
						End If

					Catch oEx As Exception

					End Try

				Next
				oLinkTable.Terminate()
			End If

		End Sub

		Private Sub cmdLoadParcels_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadParcels.Click
			Me.Cursor = Cursors.WaitCursor
			zzLoadParcelExproUnion()
			Me.Cursor = Cursors.Default

		End Sub

		Private Sub cmdTest_Click(oSender As System.Object, e As EventArgs) Handles cmdTest.Click
			zzShowResults()
		End Sub
		Private Sub zzShowResults()
			Dim oGridRow As DataGridViewRow = Nothing
			Dim moEntParcelsUB As Integer
			Me.Cursor = Cursors.WaitCursor
			DMCommon.Debug.MsgBox("!100126_1", "zzShowResults", mdicParcelExts.Count)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Test", "", "ParcelExproPgons", "ParcelExts", mdicParcelExts.Count, "EntConnPoints", mcolEntConnPoints.Count) 'mdicParcelExproPgons.Count,
			zzCreateEntParcelTable()

			Dim colParcelExts As IEnumerable(Of ParcelExt) = From oParcelExt In mdicParcelExts.Values
																			 Order By oParcelExt.BlockNo, oParcelExt.ParcelNo
																			 Select oParcelExt

			'		Me.dgvMain.DataSource = moEntParcels

			For Each oParcelExt As ParcelExt In colParcelExts
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelExt0", oParcelExt.BlockNo, oParcelExt.ParcelNo)
				oParcelExt.ToTable(moEntParcels)
				moEntParcelsUB = moEntParcels.Rows.Count - 1
				'	oParcelExt.ToExcel()
				If moEntParcelsUB >= 0 AndAlso moEntParcelsUB < Me.dgvMain.Rows.Count Then
					oGridRow = Me.dgvMain.Rows.Item(moEntParcelsUB)
					oGridRow.DividerHeight = miAfterParcelDividerHeight
				End If
			Next
			Me.dgvMain.DataSource = moEntParcels
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdLoadEntLines_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadEntLines.Click
			Me.Cursor = Cursors.WaitCursor
			zzLoadEntLines()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub zzLoadEntLines()
			If mbSimpleVersion Then
				zzLoadEntLinesSimpleVersion()
			Else
				zzLoadEntLinesBaseVersion()
			End If
		End Sub
		Private Sub zzLoadEntLinesSimpleVersion()
			DMCommon.Debug.MsgBox("!LoadEntLinesSimpleVersion")
			'Dim sUnionTopoName As String = "Union"
			'Dim sLineTopoName As String = "EntLines"
			'Dim sLineInTopoName As String = "EntLinesIn"
			Dim dicLineLayers As Dictionary(Of Integer, LayerConnected) = New Dictionary(Of Integer, LayerConnected)()
			Dim dicEntConnected As Dictionary(Of Integer, EntityConnected) = New Dictionary(Of Integer, EntityConnected)()

			'	Dim sResultODTableName As String = "EntLinesIn"
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			Dim sLayer As String
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim oEntityConnected As EntityConnected = Nothing
			Dim oEntityFragmentConnected As EntityConnected
			Dim oPolyline As Polyline
			Dim colEdges As FullEdgeCollection
			Dim oLineTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oLineTopology IsNot Nothing Then
				colEdges = oLineTopology.GetFullEdges()
				For Each oFullEdge As FullEdge In colEdges
					sLayer = DMAcadExt.AcadTransaction.GetLayer(oFullEdge.Entity)
					If sLayer IsNot Nothing AndAlso mdicLayers IsNot Nothing AndAlso mdicLayers.TryGetValue(sLayer, tLayerConnected) AndAlso Not dicLineLayers.ContainsKey(oFullEdge.ID) Then
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!1EntLines", colEdges.Count, sLayer, tLayerConnected.Layer, tLayerConnected.GeometricType)
						If tLayerConnected.GeometricType = enGeometricType.Line Then
							dicLineLayers.Add(oFullEdge.ID, tLayerConnected)
							oPolyline = DMAcadExt.AcadTransaction.GetPolyline(oFullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							If oPolyline IsNot Nothing Then
								oEntityConnected = New EntityConnected(enGeometricType.Line, tLayerConnected, oFullEdge.Entity, oPolyline.Length)  'oPolyline.Length
								dicEntConnected.Add(oFullEdge.ID, oEntityConnected)
							End If
						End If

					End If
				Next
				colEdges.Dispose()
				oLineTopology.Close()
			End If
			'-------
			Dim oLineInTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineInTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			'DMCommon.Debug.ExcelLog.SetNextValue(4, "!!!!!!!!(1)zzLoadEntLinesIn", msLineInTopoName, oLineInTopology IsNot Nothing, "---", "---", "---", "---", "---", "---")
			If oLineInTopology IsNot Nothing Then
				Dim oLinkTable As DMAcadExt.ODTable
				Dim sTableName As String = "ODLinesIn"
				Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
				Dim iLineID As Integer
				Dim iLineInID As Integer
				Dim sTest As String

				Dim oParcelExt As ParcelExt = Nothing
				Dim oParcel As TopoManager.TPlanGraph.TplnParcel
				Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(msLineTopoName, msParcelTopoName, msLinesODTableName)
				Dim oOverlayODRecord As TopoManager.OverlayODRecord
				'	Dim tLayerConnected As LayerConnected = Nothing
				Dim iParcelID As Integer
				colEdges = oLineInTopology.GetFullEdges()
				oLinkTable = New DMAcadExt.ODTable(sTableName)
				'DMCommon.Debug.ExcelLog.SetNextValue(4, "!2EntLinesIn", colEdges.Count)

				For Each oFullEdge As FullEdge In colEdges
					Try
						oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oFullEdge.Entity)
						iLineInID = oOverlayODRecord.NewID
						iLineID = oOverlayODRecord.SourceID
						iParcelID = oOverlayODRecord.OverlayID

						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!3EntLinesIn", colEdges.Count, oFullEdge.Entity, iLineInID, iLineID, iParcelID)

						If dicLineLayers.TryGetValue(iLineID, tLayerConnected) AndAlso dicEntConnected.TryGetValue(iLineID, oEntityConnected) Then
							If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
								oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
								If oParcel IsNot Nothing Then
									oParcelExt = New ParcelExt(oParcel)
									sTest = "Not Exists"
									mdicParcelExts.Add(iParcelID, oParcelExt)
								Else
									DMCommon.Debug.MsgBox("!Parcel Not found_Ar", iParcelID, mdicParcelExts.Count, TopoManager.TPlanGraph.TplnProject.Parcels.Count)
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!Parcel not found_Ar", iParcelID, mdicParcelExts.Count, TopoManager.TPlanGraph.TplnProject.Parcels.Count)
								End If
							End If

							oPolyline = DMAcadExt.AcadTransaction.GetPolyline(oFullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							oEntityFragmentConnected = New EntityConnected(enGeometricType.Line, tLayerConnected, oFullEdge.Entity, oPolyline.Length)
							'''''''''''''''''''''''070621 I variant  oParcelExt.AddEntityConnected(iParcelExproPgonID, oEntityFragmentConnected)
							oEntityConnected.AddFragment(iParcelID, oPolyline.Length)

							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!4EntLinesIn", iLineInID, iLineID, iParcelID, dicLineLayers.ContainsKey(iLineID), dicEntConnected.ContainsKey(iLineID), mdicParcelExts.ContainsKey(iParcelID))

						End If


					Catch oEx As Exception

					End Try
				Next
				oLinkTable.Terminate()
				DMCommon.Debug.MsgBox("!dicEntConnected.Count", dicEntConnected.Count, colEdges, dicLineLayers.Count)
				For Each oEntityConnectedA As EntityConnected In dicEntConnected.Values
					iParcelID = oEntityConnectedA.GetMaxValueParcel()
					If iParcelID <> 0 Then
						If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
							oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
							If oParcel IsNot Nothing Then
								oParcelExt = New ParcelExt(oParcel)
								sTest = "Not Exists"
								mdicParcelExts.Add(iParcelID, oParcelExt)
							Else
								'		DMCommon.Debug.ExcelLog.SetNextValue(0, "!Parcel not found_Ar", iParcelID, mdicParcelExts.Count, TopoManager.TPlanGraph.TplnProject.Parcels.Count)
							End If
						End If
						oParcelExt.AddEntityConnected(oEntityConnectedA)
					Else
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!5EntLinesIn", colEdges.Count, iLineInID, iLineID, iParcelID, oEntityConnectedA.GetParcelFragmentCount)
					End If
				Next
			Else
				DMCommon.Debug.MsgBox("!oLineInTopology Is Nothing")
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzLoadEntLinesBaseVersion()
			'Dim sUnionTopoName As String = "Union"
			'Dim sLineTopoName As String = "EntLines"
			'Dim sLineInTopoName As String = "EntLinesIn"
			Dim dicLineLayers As Dictionary(Of Integer, LayerConnected) = New Dictionary(Of Integer, LayerConnected)()
			Dim dicEntConnected As Dictionary(Of Integer, EntityConnected) = New Dictionary(Of Integer, EntityConnected)()

			'	Dim sResultODTableName As String = "EntLinesIn"
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			Dim sLayer As String
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim oEntityConnected As EntityConnected = Nothing
			Dim oEntityFragmentConnected As EntityConnected
			Dim oPolyline As Polyline
			Dim colEdges As FullEdgeCollection
			Dim oLineTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oLineTopology IsNot Nothing Then
				colEdges = oLineTopology.GetFullEdges()
				For Each oFullEdge As FullEdge In colEdges
					sLayer = DMAcadExt.AcadTransaction.GetLayer(oFullEdge.Entity)
					If sLayer IsNot Nothing AndAlso mdicLayers IsNot Nothing AndAlso mdicLayers.TryGetValue(sLayer, tLayerConnected) AndAlso Not dicLineLayers.ContainsKey(oFullEdge.ID) Then
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!1EntLines", colEdges.Count, colEdges.Count, sLayer, tLayerConnected.Layer, tLayerConnected.GeometricType)
						If tLayerConnected.GeometricType = enGeometricType.Line Then
							dicLineLayers.Add(oFullEdge.ID, tLayerConnected)
							oPolyline = DMAcadExt.AcadTransaction.GetPolyline(oFullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							oEntityConnected = New EntityConnected(enGeometricType.Line, tLayerConnected, oFullEdge.Entity, oPolyline.Length)
							dicEntConnected.Add(oFullEdge.ID, oEntityConnected)
						End If
					End If
				Next
				colEdges.Dispose()
				oLineTopology.Close()
			End If

			Dim oLineInTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineInTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			'DMCommon.Debug.ExcelLog.SetNextValue(4, "!zzLoadEntLinesIn", "---", "---", "---", "---", "---", "---")
			If oLineInTopology IsNot Nothing Then
				Dim oLinkTable As DMAcadExt.ODTable
				Dim sTableName As String = "ODLinesIn"
				Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
				Dim iLineID As Integer
				Dim iLineInID As Integer


				Dim oParcelExt As ParcelExt = Nothing
				Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet = New TopoManager.OverlayODRecordSet(msLineTopoName, msUnionTopoName, msLinesODTableName)
				Dim oOverlayODRecord As TopoManager.OverlayODRecord
				'	Dim tLayerConnected As LayerConnected = Nothing
				Dim iParcelExproPgonID As Integer
				colEdges = oLineInTopology.GetFullEdges()
				oLinkTable = New DMAcadExt.ODTable(sTableName)


				For Each oFullEdge As FullEdge In colEdges
					Try
						oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oFullEdge.Entity)

						iLineInID = oOverlayODRecord.NewID
						iLineID = oOverlayODRecord.SourceID
						iParcelExproPgonID = oOverlayODRecord.OverlayID

						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!2EntLinesIn", colEdges.Count, oFullEdge.Entity, oOverlayODRecord, iLineInID, iLineID, iParcelExproPgonID)

						If dicLineLayers.TryGetValue(iLineID, tLayerConnected) AndAlso dicEntConnected.TryGetValue(iLineID, oEntityConnected) AndAlso mdicParcelExproPgons.TryGetValue(iParcelExproPgonID, oParcelExt) Then
							oPolyline = DMAcadExt.AcadTransaction.GetPolyline(oFullEdge.Entity, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							oEntityFragmentConnected = New EntityConnected(enGeometricType.Line, tLayerConnected, oFullEdge.Entity, oPolyline.Length)
							'''''''''''''''''''''''070621 I variant  oParcelExt.AddEntityConnected(iParcelExproPgonID, oEntityFragmentConnected)
							oEntityConnected.AddFragment(iParcelExproPgonID, oPolyline.Length)
						End If


					Catch oEx As Exception

					End Try

				Next
				oLinkTable.Terminate()

				For Each oEntityConnectedA As EntityConnected In dicEntConnected.Values
					iParcelExproPgonID = oEntityConnectedA.GetMaxValueParcel()
					If iParcelExproPgonID <> 0 AndAlso mdicParcelExproPgons.TryGetValue(iParcelExproPgonID, oParcelExt) Then
						oParcelExt.AddEntityConnected(iParcelExproPgonID, oEntityConnectedA)
					End If

				Next
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub



		Private Sub cmdLoadEntAreas_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadEntAreas.Click
			Me.Cursor = Cursors.WaitCursor
			zzLoadEntAreas()
			Me.Cursor = Cursors.Default



		End Sub
		Private Sub zzLoadEntAreasPolylinesClosed_050125()
			'	Dim sUnionTopoName As String = "Union"
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel '''' = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim colEmpty As ObjectIdCollection = New ObjectIdCollection()
			Dim sTest As String
			'	DMCommon.Debug.MsgBox("Start of zzLoadEntAreasPolylinesClosed")


			'	If oSourceTopology IsNot Nothing Then
			'	Dim oLinkTable As DMAcadExt.ODTable
			'Dim sTableName As String = "ODUnion"
			Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet
			Dim oParcelExt As ParcelExt = Nothing
			Dim oOverlayTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oClipTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel

			Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msPolylinesClosedODTableName
			Dim oOverlayODRecord As TopoManager.OverlayODRecord

			'	Dim iParcelTopoID As Integer
			'	oLinkTable = New DMAcadExt.ODTable(msUnionODTableName)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN0", mcolEntConnPoints.Count)
			Dim oPolyline As Polyline
			Dim iParcelID As Integer
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("0", 256, True, "BORS022")
			Dim oEdgeCreationSettings As EntityCreationSettings
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("0", 0, False, "MRK")
			Dim oEntityConnected As EntityConnected
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim colEdges As ObjectIdCollection = New ObjectIdCollection()
			Dim colCentroids As ObjectIdCollection = Nothing
			Dim sClipTopoName As String = msPolylineClosedClipTopoName
			Dim iPgonIndex As Integer = 1
			Dim oExproPgon As Polygon

			Dim oParcelPgon As Polygon
			Dim oPLineClosedPgon As Polygon
			Dim colClipPgons As PolygonCollection
			Dim sCentroidLayer As String
			Dim bCentroidExists As Boolean
			Dim bProblem As Boolean
			Dim iLineIndex As Integer = 0
			Dim sLineIndex As String
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			colCentroids = DMAcadExt.AcadTransaction.GetAllBlockRefs("BORS022")
			oSourceTopology = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oSourceTopology IsNot Nothing Then
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!msSourceTopoName ", msSourceTopoName, mcolEntPLinesClosed, colCentroids.Count)
				oExproTopology = TopoManager.TopoCreator.GetOpenedTopology(msExproTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
				For Each tAcObjID As ObjectId In mcolEntPLinesClosed
					sLineIndex = "'" & iLineIndex.ToString() & "/" & mcolEntPLinesClosed.Count.ToString()
									sCentroidLayer = "EMPTY"
					oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
					If mdicLayers.TryGetValue(oPolyline.Layer, tLayerConnected) Then   'AndAlso oPolyline.Closed
						tPoint = oPolyline.GetPoint2dAt(0)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Begin of ", sLineIndex, tPoint.X, tPoint.Y, oPolyline.Handle, mcolEntPLinesClosed.Count, oPolyline.Layer)
						If oPolyline.Closed Then
							'oPolyline.Closed = True

							If oTopos.Exists(msPolylineClosedTopoName) Then
								oTopos.Delete(msPolylineClosedTopoName, False)
							End If

							colEdges.Clear()
							colEdges.Add(tAcObjID)

							Try
								oTopos.Create(msPolylineClosedTopoName, colEdges, colEmpty, colCentroids, TopologyTypes.Polygon)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology0: " & msPolylineClosedTopoName, False, oPolyline.GetPoint2dAt(0).ToString())
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!---MapExcep0", sLineIndex, oPolyline.GetPoint2dAt(0).ToString(), oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), mcolEntPLinesClosed.Count, oPolyline.Layer)
							End Try

							oOverlayTopology = TopoManager.TopoCreator.GetOpenedTopology(msPolylineClosedTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
							If oOverlayTopology IsNot Nothing Then
								Dim colPolygons As PolygonCollection = oOverlayTopology.GetPolygons()
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!colPgons ", sLineIndex, colPolygons Is Nothing, colPolygons.Count)
								If colPolygons.Count = 1 Then


									oPLineClosedPgon = colPolygons.Item(0)
									If oPLineClosedPgon IsNot Nothing AndAlso colCentroids.Contains(oPLineClosedPgon.Entity) Then
										sCentroidLayer = DMAcadExt.AcadTransaction.GetLayer(oPLineClosedPgon.Entity)

										If String.Equals(sCentroidLayer, oPolyline.Layer, StringComparison.InvariantCultureIgnoreCase) Then
											bCentroidExists = True
										Else
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!Not Layer<>", sCentroidLayer, oPolyline.Layer)
											bCentroidExists = False
											bProblem = True
										End If
									Else
										bCentroidExists = False
									End If


									If bCentroidExists Then

										Try
											oExproPgon = oExproTopology.FindPolygon(oPLineClosedPgon.Centroid)
										Catch oEx As Exception
											oExproPgon = Nothing

										End Try

										If oExproPgon IsNot Nothing Then
											Try
												oParcelPgon = oSourceTopology.FindPolygon(oPLineClosedPgon.Centroid)
											Catch oEx As Exception
												oParcelPgon = Nothing

											End Try
										Else
											oParcelPgon = Nothing
										End If


										If oExproPgon IsNot Nothing AndAlso oParcelPgon IsNot Nothing Then  'oExproPgon IsNot Nothing AndAlso
											iParcelID = oParcelPgon.ID
											oEntityConnected = New EntityConnected(tLayerConnected, oPLineClosedPgon.Entity, oPLineClosedPgon.Area)
											If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
												oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
												If oParcel IsNot Nothing Then
													oParcelExt = New ParcelExt(oParcel)
													sTest = "Not Exists"
													mdicParcelExts.Add(iParcelID, oParcelExt)
												Else
													DMCommon.Debug.MsgBox("#2783", "Parcel was not found", iParcelID)
												End If


											Else
												sTest = "Yes Exists"
											End If
											oParcelExt.AddEntityConnected(oEntityConnected)
										End If


										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, oParcelExt.BlockNo, oParcelExt.ParcelNo, oEntityConnected.LayerDescription, mdicParcelExts.Count)
									Else
										Try
											oSourceTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology1: " & msPolylineClosedTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep1", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, mcolEntPLinesClosed.Count, oPolyline.Layer)
										End Try
										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!A_LoadPlinesCl", oPolyline Is Nothing)
										oSourceTopology.SetNodeCreationSettings(oNodeCreationSettings)
										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!B_LoadPlinesCl", oPolyline.Layer)
										oEdgeCreationSettings = New EntityCreationSettings(oPolyline.Layer, 0)
										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!C_LoadPlinesCl", oEdgeCreationSettings Is Nothing)
										Try
											oSourceTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology2: " & msPolylineClosedTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep2", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, mcolEntPLinesClosed.Count, oPolyline.Layer)
										End Try


										tResultODTable = New ObjectDataTable()
										tResultODTable.ODTableName = msPolylinesClosedODTableName & CStr(iPgonIndex)
										iPgonIndex += 1
										'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Before Clip", sLineIndex, oSourceTopology.Name, oOverlayTopology.Name, sClipTopoName, tResultODTable.ODTableName)

										Try
											oSourceTopology.Clip(oOverlayTopology, sClipTopoName, "", tResultODTable)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!Clip2: " & sClipTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!---ExClip", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, oPLineClosedPgon.Centroid, oPolyline.Layer)
										End Try
									End If


									oClipTopology = TopoManager.TopoCreator.GetOpenedTopology(sClipTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, False)
									If oClipTopology IsNot Nothing Then
										'oOverlayODRecordSet = New TopoManager.OverlayODRecordSet(msSourceTopoName, msPolylineClosedTopoName, tResultODTable.ODTableName)
										colClipPgons = oClipTopology.GetPolygons()
										For Each oPgon As Polygon In colClipPgons
											If True Then
												'oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oPgon.Entity)
												'iParcelID = oOverlayODRecord.SourceID
												oParcelPgon = oSourceTopology.FindPolygon(oPgon.Centroid)
												If oParcelPgon IsNot Nothing Then
													iParcelID = oParcelPgon.ID
													If bProblem Then
														DMCommon.Debug.ExcelLog.SetNextValue(0, "!---Problem", sLineIndex, sCentroidLayer, oPolyline.Layer, colClipPgons.Count, iParcelID, oPgon.Centroid)
													End If
													oEntityConnected = New EntityConnected(tLayerConnected, oPgon.Entity, oPgon.Area)

													If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
														oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
														oParcelExt = New ParcelExt(oParcel)
														sTest = "Not Exists"
														mdicParcelExts.Add(iParcelID, oParcelExt)
													Else
														sTest = "Yes Exists"
													End If
													oParcelExt.AddEntityConnected(oEntityConnected)
												End If


											End If
										Next
										'''''''''''''''''''''''''''''''oPolyline.Layer = "EntAreas"
										'oOverlayODRecordSet.Dispose()
										If colClipPgons IsNot Nothing Then
											colClipPgons.Dispose()
										End If

										If oClipTopology IsNot Nothing Then
											oClipTopology.Close()
										End If

										Try
											oTopos.Delete(sClipTopoName, False)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											'DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!!DelClip+: ", msPolylineClosedClipTopoName, sX)
											DMCommon.Debug.MsgBox("!+DelClip+", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), sClipTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExDel", sLineIndex, oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), mcolEntPLinesClosed.Count, oPolyline.Layer)
										End Try
									End If
								Else
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!---OverLayTopo1", sLineIndex, "Number of pgons", colPolygons.Count)
								End If

								oOverlayTopology.Close()
							Else
								'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!OverLayTopo", sLineIndex, "OverLayTopo is Nothing")
							End If
						Else
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!---Not Closed", sLineIndex, oPolyline.Layer)
						End If
					Else
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Not Layer", sLineIndex, oPolyline.Layer)

					End If
					'	If iPgonIndex = 36 Then
					'If iPgonIndex = 42 Then
					If iPgonIndex = 12 Then

						'Exit For
					End If
					iLineIndex += 1
				Next
				oSourceTopology.Close()
				oExproTopology.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			'	DMCommon.Debug.MsgBox("End of zzLoadEntAreasPolylinesClosed")
		End Sub


		Private Sub zzLoadEntAreasPolylinesClosed()
			'	Dim sUnionTopoName As String = "Union"
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel '''' = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim colEmpty As ObjectIdCollection = New ObjectIdCollection()
			Dim sTest As String
			'	DMCommon.Debug.MsgBox("Start of zzLoadEntAreasPolylinesClosed")


			'	If oSourceTopology IsNot Nothing Then
			'	Dim oLinkTable As DMAcadExt.ODTable
			'Dim sTableName As String = "ODUnion"
			Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet
			Dim oParcelExt As ParcelExt = Nothing
			Dim oOverlayTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oClipTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oExproTopology As Autodesk.Gis.Map.Topology.TopologyModel

			Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msPolylinesClosedODTableName
			Dim oOverlayODRecord As TopoManager.OverlayODRecord
			'DMCommon.Debug.MsgBox("!100126_1", "zzShowResults", mdicParcelExts.Count)
			'	Dim iParcelTopoID As Integer
			'	oLinkTable = New DMAcadExt.ODTable(msUnionODTableName)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN0", mcolEntConnPoints.Count)
			Dim oPolyline As Polyline
			Dim sPolylineLayer As String

			Dim iParcelID As Integer
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("0", 256, True, "BORS022")
			Dim oEdgeCreationSettings As EntityCreationSettings
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("0", 0, False, "MRK")
			Dim oEntityConnected As EntityConnected
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim colEdges As ObjectIdCollection = New ObjectIdCollection()
			Dim colCentroids As ObjectIdCollection = Nothing
			Dim colAllCentroids As ObjectIdCollection = Nothing

			Dim colCentroidsByLayers As ObjectIdCollection = Nothing

			Dim sClipTopoName As String = msPolylineClosedClipTopoName
			Dim iPgonIndex As Integer = 1
			Dim oExproPgon As Polygon
			Dim dicCentroidsByLayers As Dictionary(Of String, ObjectIdCollection) = New Dictionary(Of String, ObjectIdCollection)()
			Dim sCentroidLayer As String
			Dim oParcelPgon As Polygon
			Dim oPLineClosedPgon As Polygon
			Dim colClipPgons As PolygonCollection
			''''''''''''Dim sCentroidLayer As String
			Dim bCentroidExists As Boolean
			Dim bProblem As Boolean
			Dim iLineIndex As Integer = 0
			Dim sLineIndex As String
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
			Dim bDebug As Boolean
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			colAllCentroids = DMAcadExt.AcadTransaction.GetAllBlockRefs("BORS022")
			For Each tAcObjID As ObjectId In colAllCentroids
				sCentroidLayer = DMAcadExt.AcadTransaction.GetLayer(tAcObjID)
				If Not dicCentroidsByLayers.TryGetValue(sCentroidLayer, colCentroidsByLayers) Then
					colCentroidsByLayers = New ObjectIdCollection()
					dicCentroidsByLayers.Add(sCentroidLayer, colCentroidsByLayers)
				End If
				colCentroidsByLayers.Add(tAcObjID)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!colCentroidsByLayers ", sCentroidLayer, colCentroidsByLayers.Count, colAllCentroids.Count)
			Next

			oSourceTopology = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oSourceTopology IsNot Nothing Then
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!msSourceTopoName ", msSourceTopoName, mcolEntPLinesClosed, colAllCentroids.Count)
				oExproTopology = TopoManager.TopoCreator.GetOpenedTopology(msExproTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
				For Each tAcObjID As ObjectId In mcolEntPLinesClosed
					bDebug = False
					sLineIndex = "'" & iLineIndex.ToString() & "/" & mcolEntPLinesClosed.Count.ToString()
					sCentroidLayer = "EMPTY"
					oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
					sPolylineLayer = oPolyline.Layer
					If mdicLayers.TryGetValue(sPolylineLayer, tLayerConnected) Then
						tPoint = oPolyline.GetPoint2dAt(0)
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Begin of ", sLineIndex, tPoint.X, tPoint.Y, oPolyline.Handle, mcolEntPLinesClosed.Count, sPolylineLayer)
						If oPolyline.Closed Then
							bDebug = (oPolyline.Handle.ToString() = "B6CC")
							If oTopos.Exists(msPolylineClosedTopoName) Then
								oTopos.Delete(msPolylineClosedTopoName, False)
							End If
							colEdges.Clear()
							colEdges.Add(tAcObjID)
							If dicCentroidsByLayers.TryGetValue(sPolylineLayer, colCentroidsByLayers) Then
								colCentroids = colCentroidsByLayers
							Else
								colCentroids = colAllCentroids
							End If
							If colCentroids.Count > 0 Then
								Try
									oTopos.Create(msPolylineClosedTopoName, colEdges, colEmpty, colCentroids, TopologyTypes.Polygon)
									oOverlayTopology = TopoManager.TopoCreator.GetOpenedTopology(msPolylineClosedTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
								Catch oMapEx As Autodesk.Gis.Map.MapException
									'DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology0: " & msPolylineClosedTopoName, False, oPolyline.GetPoint2dAt(0).ToString())
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!---MapExcep0", sLineIndex, oPolyline.GetPoint2dAt(0).ToString(), oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), mcolEntPLinesClosed.Count, oPolyline.Layer)
									oOverlayTopology = Nothing
								End Try
							Else
								oOverlayTopology = Nothing
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!Err", "colCentroids.Count > 0")
							End If

							If oOverlayTopology IsNot Nothing Then
								Dim colPolygons As PolygonCollection = oOverlayTopology.GetPolygons()
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!colPgons ", sLineIndex, colPolygons Is Nothing, colPolygons.Count)
								If colPolygons IsNot Nothing AndAlso colPolygons.Count = 1 Then
									oPLineClosedPgon = colPolygons.Item(0)
									If oPLineClosedPgon IsNot Nothing AndAlso colCentroids.Contains(oPLineClosedPgon.Entity) Then
										sCentroidLayer = DMAcadExt.AcadTransaction.GetLayer(oPLineClosedPgon.Entity)

										If Not String.IsNullOrEmpty(sCentroidLayer) AndAlso String.Equals(sCentroidLayer, oPolyline.Layer, StringComparison.InvariantCultureIgnoreCase) Then
											bCentroidExists = True
										Else
											'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!Not Layer<>", sCentroidLayer, oPolyline.Layer)
											bCentroidExists = False
											bProblem = True
										End If
									Else

										bCentroidExists = False
									End If

									'DMCommon.Debug.ExcelLog.SetNextValue(0, "!CentroidExists  ", bCentroidExists)
									If bCentroidExists Then

										Try
											oExproPgon = oExproTopology.FindPolygon(oPLineClosedPgon.Centroid)
										Catch oEx As Exception
											oExproPgon = Nothing

										End Try

										If oExproPgon IsNot Nothing Then
											Try
												oParcelPgon = oSourceTopology.FindPolygon(oPLineClosedPgon.Centroid)
											Catch oEx As Exception
												oParcelPgon = Nothing

											End Try
										Else
											oParcelPgon = Nothing
										End If


										If oExproPgon IsNot Nothing AndAlso oParcelPgon IsNot Nothing Then  'oExproPgon IsNot Nothing AndAlso
											iParcelID = oParcelPgon.ID
											oEntityConnected = New EntityConnected(tLayerConnected, oPLineClosedPgon.Entity, oPLineClosedPgon.Area)
											If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
												oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
												If oParcel IsNot Nothing Then
													oParcelExt = New ParcelExt(oParcel)
													sTest = "Not Exists"
													mdicParcelExts.Add(iParcelID, oParcelExt)
												Else
													DMCommon.Debug.MsgBox("#2783", "Parcel was not found", iParcelID)
												End If


											Else
												sTest = "Yes Exists"
											End If
											oParcelExt.AddEntityConnected(oEntityConnected)
										End If


										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, oParcelExt.BlockNo, oParcelExt.ParcelNo, oEntityConnected.LayerDescription, mdicParcelExts.Count)
									Else
										Try
											oSourceTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology1: " & msPolylineClosedTopoName)
											'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep1", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, mcolEntPLinesClosed.Count, sPolylineLayer)
										End Try
										'DMCommon.Debug.ExcelLog.SetNextValue(0, "!A_LoadPlinesCl", oPolyline Is Nothing)
										Try
											oSourceTopology.SetNodeCreationSettings(oNodeCreationSettings)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!SetNodeCreationSettings: " & msPolylineClosedTopoName)
											'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep4", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, mcolEntPLinesClosed.Count, sPolylineLayer)
										End Try

										DMCommon.Debug.ExcelLog.SetNextValue(0, "!B_LoadPlinesCl", sPolylineLayer)
										oEdgeCreationSettings = New EntityCreationSettings(sPolylineLayer, 0)
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!C_LoadPlinesCl", oEdgeCreationSettings Is Nothing)
										Try
											oSourceTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology2: " & msPolylineClosedTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep2", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, mcolEntPLinesClosed.Count, sPolylineLayer)
										End Try


										tResultODTable = New ObjectDataTable()
										tResultODTable.ODTableName = msPolylinesClosedODTableName & CStr(iPgonIndex)
										iPgonIndex += 1
										'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Before Clip", sLineIndex, oSourceTopology.Name, oOverlayTopology.Name, sClipTopoName, tResultODTable.ODTableName)

										Try
											oSourceTopology.Clip(oOverlayTopology, sClipTopoName, "", tResultODTable)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!Clip21: " & sClipTopoName)
											'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!---ExClip", sLineIndex, oMapEx.ErrorCode, oMapEx.Message, oPLineClosedPgon.Centroid, sPolylineLayer)
										End Try
									End If


									oClipTopology = TopoManager.TopoCreator.GetOpenedTopology(sClipTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, False)
									If oClipTopology IsNot Nothing Then
										'oOverlayODRecordSet = New TopoManager.OverlayODRecordSet(msSourceTopoName, msPolylineClosedTopoName, tResultODTable.ODTableName)
										colClipPgons = oClipTopology.GetPolygons()
										For Each oPgon As Polygon In colClipPgons
											If True Then
												'oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oPgon.Entity)
												'iParcelID = oOverlayODRecord.SourceID
												oParcelPgon = oSourceTopology.FindPolygon(oPgon.Centroid)
												If oParcelPgon IsNot Nothing Then
													iParcelID = oParcelPgon.ID
													If bProblem Then
														'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!---Problem", sLineIndex, sCentroidLayer, oPolyline.Layer, colClipPgons.Count, iParcelID, oPgon.Centroid)
													End If
													oEntityConnected = New EntityConnected(tLayerConnected, oPgon.Entity, oPgon.Area)

													If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
														oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
														oParcelExt = New ParcelExt(oParcel)
														sTest = "Not Exists"
														mdicParcelExts.Add(iParcelID, oParcelExt)
													Else
														sTest = "Yes Exists"
													End If
													oParcelExt.AddEntityConnected(oEntityConnected)
												End If


											End If
										Next
										'''''''''''''''''''''''''''''''oPolyline.Layer = "EntAreas"
										'oOverlayODRecordSet.Dispose()
										If colClipPgons IsNot Nothing Then
											colClipPgons.Dispose()
										End If

										If oClipTopology IsNot Nothing Then
											oClipTopology.Close()
										End If

										Try
											oTopos.Delete(sClipTopoName, False)
										Catch oMapEx As Autodesk.Gis.Map.MapException
											'DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!!DelClip+: ", msPolylineClosedClipTopoName, sX)
											DMCommon.Debug.MsgBox("!+DelClip+", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), sClipTopoName)
											'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExDel", sLineIndex, oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), mcolEntPLinesClosed.Count, oPolyline.Layer)
										End Try
									End If
								Else
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!---OverLayTopo1", sLineIndex, "Number of pgons", colPolygons.Count)
								End If

								oOverlayTopology.Close()
							Else
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!OverLayTopo", sLineIndex, "OverLayTopo is Nothing")
							End If
						Else
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!---Not Closed", sLineIndex, oPolyline.Layer)
						End If
					Else
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Not Layer", sLineIndex, oPolyline.Layer)

					End If
					'	If iPgonIndex = 36 Then
					'If iPgonIndex = 42 Then
					If iPgonIndex = 12 Then

						'Exit For
					End If
					iLineIndex += 1
				Next
				oSourceTopology.Close()
				oExproTopology.Close()
			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

			'	DMCommon.Debug.MsgBox("End of zzLoadEntAreasPolylinesClosed")
		End Sub

		Private Sub zzLoadEntAreasPolylinesClosed_1()
			'	Dim sUnionTopoName As String = "Union"
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel '''' = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim colEmpty As ObjectIdCollection = New ObjectIdCollection()
			Dim sTest As String
			'DMCommon.Debug.MsgBox("Start of zzLoadEntAreasPolylinesClosed")


			'	If oSourceTopology IsNot Nothing Then
			'	Dim oLinkTable As DMAcadExt.ODTable
			'Dim sTableName As String = "ODUnion"
			Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			Dim oOverlayODRecordSet As TopoManager.OverlayODRecordSet
			Dim oParcelExt As ParcelExt = Nothing
			Dim oOverlayTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oClipTopology As Autodesk.Gis.Map.Topology.TopologyModel
			Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msPolylinesClosedODTableName
			Dim oOverlayODRecord As TopoManager.OverlayODRecord

			'	Dim iParcelTopoID As Integer
			'	oLinkTable = New DMAcadExt.ODTable(msUnionODTableName)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN0", mcolEntConnPoints.Count)
			Dim oPolyline As Polyline
			Dim iParcelID As Integer
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("0", 256, True, "BORS022")
			Dim sX As String
			Dim oEdgeCreationSettings As EntityCreationSettings
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("0", 0, False, "MRK")
			Dim oEntityConnected As EntityConnected
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim colEdges As ObjectIdCollection = New ObjectIdCollection()
			Dim colCentroids As ObjectIdCollection = Nothing
			Dim sClipTopoName As String = msPolylineClosedClipTopoName
			Dim iPgonIndex As Integer = 1
			Dim oParcelPgon As Polygon
			Dim oPLineClosedPgon As Polygon
			Dim colClipPgons As PolygonCollection
			sClipTopoName = msPolylineClosedClipTopoName
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, o
			Dim bDebug As Boolean
			For Each tAcObjID As ObjectId In mcolEntPLinesClosed
				bDebug = False

				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
				DMAcadExt.AcadTransaction.Start()
				If colCentroids Is Nothing Then
					colCentroids = DMAcadExt.AcadTransaction.GetAllBlockRefs("BORS022")
				End If
				If oTopos.Exists(sClipTopoName) Then
					'	sClipTopoName = msPolylineClosedClipTopoName & "_" & CStr(iPgonIndex)
					Try
						oTopos.Delete(sClipTopoName, False)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMCommon.Debug.MsgBox("!!1_DelClip+", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), sClipTopoName)
						sClipTopoName = msPolylineClosedClipTopoName & "_" & CStr(iPgonIndex)
					End Try
				End If


				oPolyline = DMAcadExt.AcadTransaction.GetPolyline(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
				If mdicLayers.TryGetValue(oPolyline.Layer, tLayerConnected) AndAlso oPolyline.Closed Then

					bDebug = oPolyline.Handle.ToString() = "B6CC"

					If Not oPolyline.Closed Then
						oPolyline.Closed = True
					End If

					If oTopos.Exists(msPolylineClosedTopoName) Then
						oTopos.Delete(msPolylineClosedTopoName, False)
					End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Begin of ", oPolyline.GetPoint2dAt(0), oPolyline.Handle, mcolEntPLinesClosed.Count, oPolyline.Layer, tAcObjID.OldIdPtr.ToInt32)

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SetEdgeCreationSettings", oEdgeCreationSettings.ToString)
					colEdges.Clear()
					colEdges.Add(tAcObjID)

					Try
						oTopos.Create(msPolylineClosedTopoName, colEdges, colEmpty, colCentroids, TopologyTypes.Polygon)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology: " & msPolylineClosedTopoName)
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep2", oMapEx.Message, oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), mcolEntPLinesClosed.Count, oPolyline.Layer)
					End Try ' autocad.Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, False)
					oSourceTopology = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
					oOverlayTopology = TopoManager.TopoCreator.GetOpenedTopology(msPolylineClosedTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
					If oSourceTopology IsNot Nothing AndAlso oOverlayTopology IsNot Nothing Then
						Dim colPolygons As PolygonCollection = oOverlayTopology.GetPolygons()
						oPLineClosedPgon = colPolygons.Item(0)
						If colCentroids.Contains(oPLineClosedPgon.Entity) Then
							Try
								oParcelPgon = oSourceTopology.FindPolygon(oPLineClosedPgon.Centroid)
							Catch oEx As Exception
								oParcelPgon = Nothing

							End Try
							If oParcelPgon IsNot Nothing Then
								iParcelID = oParcelPgon.ID
								oEntityConnected = New EntityConnected(tLayerConnected, oPLineClosedPgon.Entity, oPLineClosedPgon.Area)
								If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
									oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
									If oParcel IsNot Nothing Then
										oParcelExt = New ParcelExt(oParcel)
										sTest = "Not Exists"
										mdicParcelExts.Add(iParcelID, oParcelExt)
									Else
										DMCommon.Debug.MsgBox("#2783", "Parcel was not found", iParcelID)
									End If


								Else
									sTest = "Yes Exists"
								End If
								oParcelExt.AddEntityConnected(oEntityConnected)
							End If
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, oParcelExt.BlockNo, oParcelExt.ParcelNo, oEntityConnected.LayerDescription, mdicParcelExts.Count)
						Else
							Try
								oSourceTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!PolylineClosedTopology: " & msPolylineClosedTopoName)
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!MapExcep1", oMapEx.Message, oMapEx.ErrorCode, mcolEntPLinesClosed.Count, oPolyline.Layer)
							End Try

							oSourceTopology.SetNodeCreationSettings(oNodeCreationSettings)
							oEdgeCreationSettings = New EntityCreationSettings(oPolyline.Layer, 0)
							oSourceTopology.SetEdgeCreationSettings(oEdgeCreationSettings)

							tResultODTable = New ObjectDataTable()
							tResultODTable.ODTableName = msPolylinesClosedODTableName & CStr(iPgonIndex)
							iPgonIndex += 1
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Before Clip", oSourceTopology.Name, oOverlayTopology.Name, sClipTopoName, tResultODTable.ODTableName)

							Try
								oSourceTopology.Clip(oOverlayTopology, sClipTopoName, "", tResultODTable)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!Clip1: " & sClipTopoName)
							End Try

						End If
						oClipTopology = TopoManager.TopoCreator.GetOpenedTopology(sClipTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, False)
						If oClipTopology IsNot Nothing Then
							oOverlayODRecordSet = New TopoManager.OverlayODRecordSet(msSourceTopoName, msPolylineClosedTopoName, tResultODTable.ODTableName)
							colClipPgons = oClipTopology.GetPolygons()
							For Each oPgon As Polygon In colClipPgons
								If True Then
									oOverlayODRecord = oOverlayODRecordSet.GetOverlayODRecordNew(oPgon.Entity)
									iParcelID = oOverlayODRecord.SourceID
									oEntityConnected = New EntityConnected(tLayerConnected, oPgon.Entity, oPgon.Area)

									If Not mdicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
										oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iParcelID)
										oParcelExt = New ParcelExt(oParcel)
										sTest = "Not Exists"
										mdicParcelExts.Add(iParcelID, oParcelExt)
									Else
										sTest = "Yes Exists"
									End If
									'DMCommon.Debug.ExcelLog.SetNextValue(0, "!PgonN2", sTest, oPolygon.ID, oParcelExt.BlockNo, oParcelExt.ParcelNo, oEntityConnected.LayerDescription, mdicParcelExts.Count)zzLoadEntAreasPolylinesClosed

									oParcelExt.AddEntityConnected(oEntityConnected)
								End If
							Next
							oPolyline.Layer = "EntAreas"
							oOverlayODRecordSet.Dispose()
							colClipPgons.Dispose()

							sX = oClipTopology.Name
							oClipTopology.Close()
							Try
								'''''''''	oTopos.Delete(msPolylineClosedClipTopoName, False)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								'DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "!!DelClip+: ", msPolylineClosedClipTopoName, sX)
								DMCommon.Debug.MsgBox("!+DelClip+", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), msPolylineClosedClipTopoName, sX)
							End Try

						End If


					End If
					If oSourceTopology IsNot Nothing Then
						oSourceTopology.Close()
					End If

					If oOverlayTopology IsNot Nothing Then
						oOverlayTopology.Close()

					End If
				End If
				'	If iPgonIndex = 36 Then
				'If iPgonIndex = 42 Then
				If iPgonIndex = 12 Then


					Exit For
				End If

				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()
				'End If
			Next
			'	oSourceTopology.Close()
			'	End If

			'''''DMCommon.Debug.MsgBox("End of zzLoadEntAreasPolylinesClosed")
		End Sub
		Private Sub zzLoadEntAreasSimpleVersion()
			'Dim sAreaTopoName As String = "EntAreas"
			Dim iPgonID As Integer
			Dim iAreaIn_ID As Integer
			Dim iParcelExproPgonID As Integer
			Dim oParcelExt As ParcelExt = Nothing
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			Dim sLayer As String
			Dim oSourcePolygon As Polygon
			Dim dicCentroidLayers As Dictionary(Of Integer, LayerConnected) = New Dictionary(Of Integer, LayerConnected)()
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim oAreaTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msEntAreaTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim colPolygons As PolygonCollection = oAreaTopology.GetPolygons()
			Dim oSourceTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msSourceTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			Dim tCenterAcObjID As ObjectId
			Dim oEntityConnected As EntityConnected
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel

			Dim sTest As String
			For Each oPolygon As Polygon In colPolygons
				Try
					tCenterAcObjID = oPolygon.Entity
				Catch oEx As Exception
					tCenterAcObjID = ObjectId.Null
				End Try
				If Not tCenterAcObjID.IsNull Then
					sLayer = DMAcadExt.AcadTransaction.GetLayer(tCenterAcObjID)
					If sLayer IsNot Nothing AndAlso mdicLayers IsNot Nothing AndAlso mdicLayers.TryGetValue(sLayer, tLayerConnected) AndAlso Not dicCentroidLayers.ContainsKey(oPolygon.ID) Then
						dicCentroidLayers.Add(oPolygon.ID, tLayerConnected)
						oEntityConnected = New EntityConnected(tLayerConnected, tCenterAcObjID, oPolygon.Area)

						oSourcePolygon = oSourceTopology.FindPolygon(oPolygon.Centroid)
						If oSourcePolygon IsNot Nothing Then

							oEntityConnected.BlockRefPosition = oSourcePolygon.Centroid

							If Not mdicParcelExts.TryGetValue(oSourcePolygon.ID, oParcelExt) Then
								oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(oSourcePolygon.ID)
								If oParcel IsNot Nothing Then
									oParcelExt = New ParcelExt(oParcel)
									sTest = "Not Exists"
									mdicParcelExts.Add(oSourcePolygon.ID, oParcelExt)
								Else
									'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Parcel not found_Ar", oSourcePolygon.ID, mdicParcelExts.Count, TopoManager.TPlanGraph.TplnProject.Parcels.Count)
								End If

							Else
								sTest = "Yes Exists"
							End If

							'If mdicParcelExts.TryGetValue(oPolygon.ID, oParcelExt) Then
							If oParcelExt IsNot Nothing Then
								oParcelExt.AddEntityConnected(oEntityConnected)
							End If
							'Else

							'end  If
						Else
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!EntConn not found", oEntityConnected.BlockRefPosition)
						End If
					End If
				End If

			Next
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!0!LoadAreas", colPolygons.Count, dicCentroidLayers.Count)
			colPolygons.Dispose()
			oAreaTopology.Close()

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub
		Private Sub zzLoadEntAreas()
			If mbSimpleVersion Then
				If mbClosedPgonVersion Then
					zzLoadEntAreasPolylinesClosed()
				Else
					zzLoadEntAreasSimpleVersion()
				End If

			Else
				zzLoadEntAreasBaseVersion()
			End If
		End Sub
		Private Sub zzLoadEntAreasBaseVersion()

			'Dim sAreaTopoName As String = "EntAreas"
			Dim iPgonID As Integer

			Dim iAreaIn_ID As Integer
			Dim iParcelExproPgonID As Integer
			Dim oParcelExt As ParcelExt = Nothing
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			Dim sLayer As String
			Dim dicCentroidLayers As Dictionary(Of Integer, LayerConnected) = New Dictionary(Of Integer, LayerConnected)()
			Dim tLayerConnected As LayerConnected = New LayerConnected()
			Dim oAreaTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msEntAreaTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim colPolygons As PolygonCollection = oAreaTopology.GetPolygons()
			Dim tCenterAcObjID As ObjectId

			For Each oPolygon As Polygon In colPolygons
				Try
					tCenterAcObjID = oPolygon.Entity
				Catch oEx As Exception
					tCenterAcObjID = ObjectId.Null
				End Try
				If Not tCenterAcObjID.IsNull Then
					sLayer = DMAcadExt.AcadTransaction.GetLayer(tCenterAcObjID)
					If sLayer IsNot Nothing AndAlso mdicLayers IsNot Nothing AndAlso mdicLayers.TryGetValue(sLayer, tLayerConnected) AndAlso Not dicCentroidLayers.ContainsKey(oPolygon.ID) Then
						dicCentroidLayers.Add(oPolygon.ID, tLayerConnected)
					End If
				End If
			Next
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!0!LoadAreas", colPolygons.Count, dicCentroidLayers.Count)
			colPolygons.Dispose()
			oAreaTopology.Close()

			Dim oEntityConnected As EntityConnected
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay) = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(msEntAreas_x_UnionLayer, TopoManager.TPlanGraph.TplnProject.XDataAppName, False)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!1!LoadAreas", lstPolygons.Count)
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				iPgonID = oPolygon.FeatureID
				iParcelExproPgonID = oPolygon.SourceID
				iAreaIn_ID = oPolygon.OverlayID
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadAreas", lstPolygons.Count, iParcelExproPgonID, dicCentroidLayers.ContainsKey(iAreaIn_ID), mdicParcelExproPgons.ContainsKey(iParcelExproPgonID))
				If dicCentroidLayers.TryGetValue(iAreaIn_ID, tLayerConnected) AndAlso mdicParcelExproPgons.TryGetValue(iParcelExproPgonID, oParcelExt) Then
					oEntityConnected = New EntityConnected(tLayerConnected, New ObjectId(), oPolygon.Area)
					oParcelExt.AddEntityConnected(iParcelExproPgonID, oEntityConnected)
				End If

			Next
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()



		End Sub

		Private Sub frmEntConnected_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
			If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
				Me.Location = New System.Drawing.Point(240, 240)
				Me.Size = New System.Drawing.Size(960, 600)
				Me.WindowState = FormWindowState.Normal
			End If
		End Sub


		Private Sub zzCreateEntParcelTable()
			moEntParcels = New Data.DataTable("EntParcels")
			moEntParcels.Columns.Add("BlockNo", GetType(System.Int32))
			moEntParcels.Columns.Add("BlockAddNo", GetType(System.Int32))
			moEntParcels.Columns.Add("ParcelNo", GetType(System.Int32))
			moEntParcels.Columns.Add("LayerDescription", GetType(System.String))
			moEntParcels.Columns.Add("GeometricType", GetType(System.Int32))
			moEntParcels.Columns.Add("GeometricTypeName", GetType(System.String))


			moEntParcels.Columns.Add("EntCountIn", GetType(System.Int32))
			moEntParcels.Columns.Add("EntValueIn", GetType(System.Double))

			moEntParcels.Columns.Add("EntCountOut", GetType(System.Int32))
			moEntParcels.Columns.Add("EntValueOut", GetType(System.Double))

			'
			'oDataColumn = New System.Data.DataColumn("PositionX", GetType(System.Double))
		End Sub

		Private Sub cmdIntersectLines_Click(oSender As System.Object, e As EventArgs) Handles cmdIntersectLines.Click
			Me.Cursor = Cursors.WaitCursor
			zzIntersectLines()
			Me.Cursor = Cursors.Default

		End Sub
		Private Sub zzIntersectLines()
			Dim sOverlayTopoName As String = msSourceTopoName

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel

			oProject = oMapApplication.ActiveProject
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 256, True, "MRK")
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(msEntLineLayer, 0)
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 0, False, "")

			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msLinesODTableName
			'	Dim sParcelTopoName As String = "ParcelsLine"
			Dim oLineTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			Dim oOverlayTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sOverlayTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			'	Dim sExproTopoName As String = "ExproLine"


			Dim colOverlayData As Autodesk.Gis.Map.Topology.OverlayDataCollection = New Autodesk.Gis.Map.Topology.OverlayDataCollection()

			Dim iCodeRow As Integer = 0
			If oLineTopology IsNot Nothing AndAlso oOverlayTopology IsNot Nothing Then
				DMCommon.Debug.MsgBox("231220_2c", "zzIntersectLines", oLineTopology.Status, oLineTopology.IsComplete, oLineTopology.NeedsRefresh, oLineTopology.Type, oLineTopology.Name, oOverlayTopology.Name, oOverlayTopology.Type, msLineInTopoName)
				Try
					iCodeRow = 1
					oLineTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
					iCodeRow = 2
					oLineTopology.SetNodeCreationSettings(oNodeCreationSettings)
					iCodeRow = 3
					oLineTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
					iCodeRow = 4
					'	DMCommon.Debug.ExcelLog.SetNextValue(4, "!!!!!!++(2)zzLoadEntLinesIn", msLineInTopoName, "!!!")
					oLineTopology.Intersect(oOverlayTopology, msLineInTopoName, "", tResultODTable)
					'oParcelTopology.Union(oExproTopology, "Union", Nothing, Nothing, colOverlayData)


				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "cmdUnion1 " & "CodeRow=" & iCodeRow.ToString() & vbCrLf & oLineTopology.Name & vbCrLf & oOverlayTopology.Name & vbCrLf & msLineInTopoName, False)
					DMCommon.Debug.MsgBox("!Union1", oLineTopology.Name, oOverlayTopology.Name, msLineInTopoName, tResultODTable.ODTableName)
				End Try


				'DMCommon.Debug.MsgBox("231220_3", oParcelTopology.GetPolygons().Count)
				oLineTopology.Close()
				oOverlayTopology.Close()

			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzIntersectLines_2019()
			Dim sOverlayTopoName As String = msSourceTopoName
			'	sOverlayTopoName = "ParcelsLine"
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oProject As Autodesk.Gis.Map.Project.ProjectModel
			'	Dim iPgonID As Integer = 171
			oProject = oMapApplication.ActiveProject
			Dim oCentroidCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 256, True, "MRK")
			Dim oEdgeCreationSettings As EntityCreationSettings = New EntityCreationSettings(msEntLineLayer, 0)
			Dim oNodeCreationSettings As PointCreationSettings = New PointCreationSettings("MRK", 0, False, "")

			Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
			tResultODTable.ODTableName = msLinesODTableName
			'	Dim sParcelTopoName As String = "ParcelsLine"
			Dim oLineTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(msLineTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			Dim oOverlayTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sOverlayTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForWrite, False, True)
			'	Dim sExproTopoName As String = "ExproLine"


			Dim colOverlayData As Autodesk.Gis.Map.Topology.OverlayDataCollection = New Autodesk.Gis.Map.Topology.OverlayDataCollection()

			Dim iCodeRow As Integer = 0
			If oLineTopology IsNot Nothing AndAlso oOverlayTopology IsNot Nothing Then
				DMCommon.Debug.MsgBox("231220_2a", oLineTopology.Status, oLineTopology.IsComplete, oLineTopology.NeedsRefresh, oLineTopology.Type, oLineTopology.Name, oOverlayTopology.Name, oOverlayTopology.Type, msLineInTopoName)
				Try
					iCodeRow = 1
					oLineTopology.SetCentroidCreationSettings(oCentroidCreationSettings)
					iCodeRow = 2
					oLineTopology.SetNodeCreationSettings(oNodeCreationSettings)
					iCodeRow = 3
					oLineTopology.SetEdgeCreationSettings(oEdgeCreationSettings)
					iCodeRow = 4
					oLineTopology.Intersect(oOverlayTopology, msLineInTopoName, "", tResultODTable)
					'oParcelTopology.Union(oExproTopology, "Union", Nothing, Nothing, colOverlayData)


				Catch oMapEx As Autodesk.Gis.Map.MapException
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "cmdUnion1 " & "CodeRow=" & iCodeRow.ToString(), False)
					DMCommon.Debug.MsgBox("!Union1", oLineTopology.Name, oOverlayTopology.Name, msLineInTopoName, tResultODTable.ODTableName)
				End Try


				'DMCommon.Debug.MsgBox("231220_3", oParcelTopology.GetPolygons().Count)
				oLineTopology.Close()
				oOverlayTopology.Close()

			End If
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End Sub

		Private Sub cmdLoadAll_Click(oSender As System.Object, e As EventArgs) Handles cmdLoadAll.Click
			Me.Cursor = Cursors.WaitCursor
			If False Then
				zzLoadGlossary()
				zzLoadModelSpace()
			End If


			If mdicParcelExts Is Nothing Then
				mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
			Else
				'mdicParcelExts.Clear()
			End If
			zzLoadEntPoints()
			DMCommon.Debug.MsgBox("!End Load Points")

			zzLoadEntLines()
			DMCommon.Debug.MsgBox("!End Load Lines")
			If mbClosedPgonVersion Then
				zzLoadEntAreasPolylinesClosed()
				DMCommon.Debug.MsgBox("!End Load Areas1")
			Else
				zzLoadEntAreasSimpleVersion()
				DMCommon.Debug.MsgBox("!End Load Areas2")
			End If
			DMCommon.Debug.MsgBox("!Start ShowResults")
			zzShowResults()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdTopoPlus_Click(oSender As System.Object, e As EventArgs) Handles cmdTopoPlus.Click
			If False Then
				zzLoadGlossary()
				zzLoadModelSpace()
			End If


			If mdicParcelExts Is Nothing Then
				mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
			Else
				'mdicParcelExts.Clear()
			End If
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			zzCreateEntTopos()

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			zzIntersectLines()
			''''''''''''''	zzLoadEntLines()
		End Sub
		Private Sub cmdTopoPlus_Click_2019Vers(oSender As System.Object, e As EventArgs)
			zzLoadGlossary()
			zzLoadModelSpace()
			If mdicParcelExts Is Nothing Then
				mdicParcelExts = New Dictionary(Of Integer, ParcelExt)()
			Else
				mdicParcelExts.Clear()
			End If
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			zzCreateEntTopos()

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			zzIntersectLines()

		End Sub


		Private Sub cmdClear_Click(sender As System.Object, e As EventArgs) Handles cmdClear.Click
			Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
			Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oMapApplication.ActiveProject.Topologies
			Dim sClipTopoName As String = msPolylineClosedClipTopoName
			Dim iPgonIndex As Integer = 2
			Do While iPgonIndex < 12
				sClipTopoName = msPolylineClosedClipTopoName & "_" & CStr(iPgonIndex)
				If oTopos.Exists(sClipTopoName) Then

					Try
						oTopos.Delete(sClipTopoName, False)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMCommon.Debug.MsgBox("!+DelClip+", oMapEx.ErrorCode, DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), msPolylineClosedClipTopoName)
						'sClipTopoName = msPolylineClosedClipTopoName & "_" & CStr(iPgonIndex)
					End Try
					sClipTopoName = msPolylineClosedClipTopoName & "_" & CStr(iPgonIndex)
					iPgonIndex += 1
				End If
			Loop

		End Sub

		Private Sub dgvMain_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellContentClick

		End Sub

		Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
			zzLoadGlossary()
			zzLoadModelSpace()
		End Sub
	End Class
End Namespace