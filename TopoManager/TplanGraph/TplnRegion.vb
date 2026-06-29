Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Namespace TPlanGraph
	Public Structure RegionData
		Dim RegionNo As Integer
		Dim RegionName As String
		Dim ForcedArea As Double
		Dim Status As Integer
		Dim IsAnalytic As Boolean
		Dim InPlan As Boolean

		Dim Exists As Boolean

		Dim IsError As Boolean

		Public Sub New(saValues() As String)

			If saValues IsNot Nothing Then


				Dim iAttribUB As Integer = -1
				Try
					If saValues IsNot Nothing Then
						iAttribUB = saValues.GetUpperBound(0)
					End If
				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnRegion - New_1")
				End Try

				If iAttribUB >= 0 Then
					Try
						Dim sMitNo As String = saValues(0).Trim()
						Integer.TryParse(sMitNo, RegionNo)
						TplnRegion.ParseInPlanString(sMitNo, InPlan, RegionNo)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnRegion - New_2")
					End Try
				End If
				If iAttribUB >= 1 Then
					Try
						RegionName = DMCommon.Hebrew.Invert(saValues(1).Trim())
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnRegion - New_3")
					End Try
				End If

				If iAttribUB >= 2 Then
					Try
						Dim sForcedArea As String = saValues(2).Trim()
						'  TplnProject.WriteMessageBox(CStr(sLegalArea), "sLegalArea")
						Double.TryParse(sForcedArea, ForcedArea)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnRegion - New Lot_4")
					End Try
				Else
					TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnBlock - New_9")
				End If
				' DMCommon.Debug.MsgBox("09_420", RegionNo, RegionName, ForcedArea)
			End If
		End Sub

		Public Sub New(iRegionNo As Integer)
			RegionNo = iRegionNo
		End Sub
	End Structure

	Public Class TplnRegion
		Inherits TPlanGraph.TplnDissolvePgon
		Public Const RegionOut As Integer = 0
		Public Const RegionAllIn As Integer = 100
		Public Const RegionAllInName As String = "כל המתחמים"
		Public Const RegionOutName As String = "מחוץ לתחום"
		Public Const RegionIn As Integer = 999
		Public Const RegionDfltName As String = "עודף"
		Public Const msRegionNameFieldName As String = "RegionName"

		Private Const msMithamNoAttribTag As String = "MITNO"
		Private Const msMithamNameAttribTag As String = "MithamName"
		Private Const msForcedAreaAttribTag As String = "ForcedArea"

		Private Const msRegionNoFieldName As String = "RegionNo"

		Private Const msForcedAreaFieldName As String = "ForcedArea"
		'  Private Const zzCountFieldName As String = "ParcelCount"

		Private Shared mtRegionMapThemeData As DMAcadExt.MapThemeData
		Private Shared mtRegionProxMapThemeData As DMAcadExt.MapThemeData

		Private Shared msCentroidBlockName As String
		Private Shared msCentroidProxBlockName As String

		Private Shared miaBlockAttribIndex(enRegionCentroidAttribIndices.UB) As Integer
		Private Shared moMainDataTable As System.Data.DataTable

		Private mdicaOverlayGroups(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnOverlayGroups
		'	Private Shared mdicOverlayGroups(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TPlanGraph.TplnOverlayGroups
		Private mcolLotForcedArea As System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea)
		Private mtAreaSet As TplnAreaSet
		Private mhsLots As HashSet(Of TplnLot)
		Private mdAcadArea As Double

		Private mbPgonExists As Boolean
		Private mtRegionData As RegionData
		Private moBoundingBox As DMAcadExt.TPlnBoundingBox
		Private Shared moMainDataTable1 As System.Data.DataTable


		Private Enum enRegionCentroidAttribIndices
			MithamNo
			MithamName
			ForcedArea
			UB = ForcedArea
		End Enum
		Public Shared Sub ParseInPlanString(ByVal sValue As String, ByRef bInPlan As Boolean, ByRef iRegionNo As Integer)
			If sValue Is Nothing Then
				bInPlan = True
				iRegionNo = TplnRegion.RegionIn
			Else
				sValue.Trim()
				If StrComp(sValue, "OUT", CompareMethod.Text) = 0 Then
					bInPlan = False
				ElseIf StrComp(sValue, "IN", CompareMethod.Text) = 0 OrElse sValue.Length = 0 Then
					bInPlan = True

					iRegionNo = TplnRegion.RegionIn
				Else
					Dim iValue As Integer

					If Integer.TryParse(sValue, iValue) Then
						iRegionNo = iValue
						bInPlan = (iRegionNo <> 0)
					Else
						bInPlan = True
						iRegionNo = TplnRegion.RegionIn
					End If
				End If
			End If

		End Sub

		Public Shared Sub CreateRegionTable()
			If moMainDataTable Is Nothing Then
				moMainDataTable = New System.Data.DataTable("Regions")
				With moMainDataTable.Columns
					.Add(msRegionNoFieldName, GetType(System.Int32))
					.Add(msRegionNameFieldName, GetType(System.String))
					.Add(msForcedAreaFieldName, GetType(System.Double))
					.Add(TopoReader.msSumPgonAreaFldName, GetType(System.Double))


					.Add(TplnLot.msCalcAreaFDO_OverlayFieldName, GetType(System.Double))
					.Add(TopoReader.msAreaFldName, GetType(System.Double))


					.Add(TopoReader.BasePgonCountFieldName, GetType(System.Int32))
					.Add(TopoReader.BlockExistsFieldName, GetType(System.Boolean))
					.Add(TopoReader.PgonExistsFieldName, GetType(System.Boolean))

					.Add(TplnParcel.msDeltaAreaFieldName, GetType(System.Double))
					.Add(TplnParcel.msToleranceFieldName, GetType(System.Double))
					.Add(TplnParcel.msDeviationFieldName, GetType(System.Double))

					.Add(TopoReader.msCentroidXFldName, GetType(System.Double))
					.Add(TopoReader.msCentroidYFldName, GetType(System.Double))
					.Add(TopoReader.msPerimeterFldName, GetType(System.Double))
					.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
					.Add(TopoReader.msAcObjIDFldName, GetType(ObjectId))

				End With
			Else
				moMainDataTable.Clear()
			End If

			Dim oNewRow As System.Data.DataRow
			Dim dicRegions As TPlanGraph.TplnRegions = TopoManager.TPlanGraph.TplnProject.Regions
			Dim tParcelArea As ParcelArea

			Try
				For Each oRegion As TplnRegion In dicRegions.Values
					oNewRow = moMainDataTable.NewRow()
					tParcelArea = New ParcelArea(oRegion.AreaSet.AcadArea)
					tParcelArea.CalculateArea(oRegion.mtAreaSet.CalcArea, 3)

					With oNewRow
						.Item(msRegionNoFieldName) = oRegion.RegionNo
						.Item(msRegionNameFieldName) = oRegion.RegionName
						.Item(msForcedAreaFieldName) = oRegion.ForcedArea

						.Item(TopoReader.msSumPgonAreaFldName) = 999.99

						.Item(TplnLot.msCalcAreaFDO_OverlayFieldName) = Math.Round(oRegion.AreaSet.CalcArea, 4)

						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!oRegion.AreaSet", oRegion.AreaSet.AcadArea, oRegion.AreaSet.CalcArea, oRegion.AreaSet.CalcArea2, oRegion.AreaSet.CalcGroupArea, oRegion.AreaSet.CalcGroupArea2, oRegion.AreaSet.RoundedArea)

						.Item(TopoReader.msAreaFldName) = Math.Round(oRegion.AreaSet.AcadArea, 4)

						.Item(TopoReader.BasePgonCountFieldName) = 99
						.Item(TopoReader.BlockExistsFieldName) = oRegion.BlockExists
						.Item(TopoReader.PgonExistsFieldName) = oRegion.PgonExists

						.Item(TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM
						.Item(TplnParcel.msToleranceFieldName) = tParcelArea.Tolerance
						.Item(TplnParcel.msDeviationFieldName) = tParcelArea.Deviation

						.Item(TopoReader.msCentroidXFldName) = oRegion.CentroidX
						.Item(TopoReader.msCentroidYFldName) = oRegion.CentroidY
						.Item(TopoReader.msPerimeterFldName) = oRegion.Perimiter
						.Item(TopoReader.msTopoIDFldName) = oRegion.TopoID
						If Not oRegion.CentroidAcObjID.IsNull Then
							.Item(TopoReader.msAcObjIDFldName) = oRegion.CentroidAcObjID   'oBlock.CentroidAcObjID.OldIdPtr.ToInt64()
						End If

					End With



					moMainDataTable.Rows.Add(oNewRow)
				Next

			Catch oEx As Exception
				TplnProject.WriteMessageBox(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBlock - CreateBlockTable")
			End Try
		End Sub
		Public Shared ReadOnly Property MainView() As System.Data.DataView
			Get
				Dim sSort As String = ""
				If moMainDataTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public Shared ReadOnly Property MainDataTable() As System.Data.DataTable
			Get
				Return moMainDataTable
			End Get
		End Property


		Public Overridable Sub AddOverlayGroup(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByRef oOverlayGroup As TplnOverlayGroup)

			If mdicaOverlayGroups(iOverlayIndex) Is Nothing Then
				mdicaOverlayGroups(iOverlayIndex) = New TplnOverlayGroups()
			End If
			Dim lOverlayKey As ULong = TplnOverlayGroup.GetOverlayKey(oOverlayGroup.ParcelID, oOverlayGroup.LotID, "")
			Try
				mdicaOverlayGroups(iOverlayIndex).Add(lOverlayKey, oOverlayGroup)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "AddOverGr", "OK", iOverlayIndex, RegionNo, moaOverlayGroups(iOverlayIndex).Count, oOverlayGroup.ParcelID, oOverlayGroup.LotID)
			Catch oEx As Exception
				Dim iParcelID As Integer
				Dim iLotID As Integer
				TplnOverlayGroup.ParseOverlayKey(lOverlayKey, iParcelID, iLotID)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "Err-Add", oEx.Message, iOverlayIndex, mdicaOverlayGroups(iOverlayIndex).Count, oOverlayGroup.ParcelID, oOverlayGroup.LotID)
			End Try


		End Sub
		Public Sub CalculateOverlayGroups(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				'DMCommon.Debug.MsgBox("01_554b", RegionNo, bMerge, bUnion, bFDO_Overlay)
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If baOverlayArray(iOverlayIndex) Then
						If mdicaOverlayGroups(iOverlayIndex) IsNot Nothing Then
							'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!RegionC", RegionNo, RegionName)
							mdicaOverlayGroups(iOverlayIndex).Calculate2New(mcolLotForcedArea)
						Else
							DMCommon.Debug.MsgBox("01_554N", iOverlayIndex)
						End If
					End If
				Next
			End If
		End Sub


		Public Shared Sub Initialize(tRegionMapThemeData As DMAcadExt.MapThemeData)
			Try


				mtRegionMapThemeData = tRegionMapThemeData
				msCentroidBlockName = mtRegionMapThemeData.CentroidBlocks
				' System.Windows.Forms.MessageBox.Show(tRegionMapThemeData.MapThemeID.ToString() & vbCrLf & msCentroidBlockName, "TplnRegion - Initialize")
				zzInitCentroid()

				'  DMCommon.Functions.DispArray(miaBlockAttribIndex, "Block !!iaBlockAttribIndex")


			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
			End Try

		End Sub
		Public Shared Sub InitializeProx(tRegionProxMapThemeData As DMAcadExt.MapThemeData)
			Try


				mtRegionProxMapThemeData = tRegionProxMapThemeData
				msCentroidProxBlockName = mtRegionProxMapThemeData.CentroidBlocks
				'  System.Windows.Forms.MessageBox.Show(tRegionProxMapThemeData.MapThemeID.ToString() & vbCrLf & msCentroidBlockName, "InitializeProx-TplnRegion")
				zzInitCentroid()

				'  DMCommon.Functions.DispArray(miaBlockAttribIndex, "Block !!iaBlockAttribIndex")


			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - InitializeProx")
			End Try

		End Sub
		Public ReadOnly Property HasCentroid() As Boolean
			Get
				Return Not dtCentroidAcObjID.IsNull
			End Get
		End Property

		Public Shared Function GetAllInRegion() As TplnRegion
			Return New TplnRegion(RegionAllIn)
		End Function

		Public Shared Function GetTopoName() As String
			Return mtRegionMapThemeData.TopoName
			Return mtRegionMapThemeData.LineTopoName

		End Function
		Public Shared Function GetProxTopoName() As String
			'	DMCommon.Debug.MsgBox("09_423", mtRegionProxMapThemeData.MapThemeID, mtRegionProxMapThemeData.TopoName, mtRegionProxMapThemeData.LineTopoName)
			Return mtRegionProxMapThemeData.TopoName
		End Function
		Public Shared Function GetTopoNameAlt() As String
			If mtRegionMapThemeData.IsNotEmpty Then
				Return mtRegionMapThemeData.LineTopoName
			Else
				Return mtRegionProxMapThemeData.LineTopoName
			End If




		End Function

		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon)
			mhsLots = New HashSet(Of TplnLot)()
			MyBase.SetAttributeOrder()

			mtRegionData = New RegionData(dsaBlockAttribText)
			If mtRegionData.IsError Then
				DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidX, "", "Attribute problem 1", False)
			End If
			zzNew()
		End Sub


		Public Sub New(iNo As Integer)
			MyBase.New(0)
			mtRegionData = New RegionData(iNo)

			mhsLots = New HashSet(Of TplnLot)()
			zzNew()
		End Sub
		Public Sub New(ByVal tBlockAcObjId As ObjectId)
			MyBase.New(0)
			mbPgonExists = False

			zzNew()
			dtCentroidAcObjID = tBlockAcObjId
			'25/08/10	Me.LoadBlockRefData(tBlockAcObjId)
			Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRefForRead(tBlockAcObjId, False, False)
			MyBase.ddCentroidX = oBlockRef.Position.X
			MyBase.ddCentroidY = oBlockRef.Position.Y

			Me.zzLoadBlockRefData(tBlockAcObjId)
		End Sub

		Public Shared ReadOnly Property CentroidBlockName() As String
			Get
				Return msCentroidBlockName
				'moAcadBlockDef.BlockName
			End Get
		End Property
		Public Shared ReadOnly Property CentroidProxBlockName() As String
			Get
				Return msCentroidProxBlockName
				'moAcadBlockDef.BlockName
			End Get
		End Property
		Public Property AreaSet As TplnAreaSet
			Get
				Return mtAreaSet
			End Get
			Set(tValue As TplnAreaSet)
				mtAreaSet = tValue
			End Set
		End Property
		Public ReadOnly Property RegionNo As Integer
			Get
				Return mtRegionData.RegionNo
			End Get
		End Property
		Public ReadOnly Property RegionName As String
			Get
				If String.IsNullOrEmpty(mtRegionData.RegionName) Then
					If mtRegionData.RegionNo = RegionIn Then
						Return RegionDfltName
					ElseIf mtRegionData.RegionNo = RegionAllIn Then
						Return RegionAllInName
					ElseIf mtRegionData.RegionNo = RegionOut Then
						Return RegionOutName
					Else
						Return "# " & mtRegionData.RegionNo.ToString()
					End If

				Else
					Return mtRegionData.RegionName
				End If

			End Get
		End Property
		Public ReadOnly Property RegionID_Name As String
			Get
				If String.IsNullOrEmpty(mtRegionData.RegionName) Then

					Return Me.RegionName


				Else
					Return mtRegionData.RegionNo.ToString() & "-" & mtRegionData.RegionName
				End If

			End Get
		End Property
		Public Overrides Function ToString() As String
			Return RegionName
		End Function
		Public ReadOnly Property ForcedArea As Double
			Get
				Return mtRegionData.ForcedArea
			End Get
		End Property
		Public Sub AddAreaset(tAreaSet As TplnAreaSet)
			mtAreaSet += tAreaSet
		End Sub
		Private Sub zzNew()
			' mdicParcels = New TPlanGraph.TplnParcels(False, True)
			mcolLotForcedArea = New ObjectModel.Collection(Of BalanceArea.ConstArea)()
			doBoundingBox = New DMAcadExt.TPlnBoundingBox()
			'doBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolyline.GeometricExtents)
		End Sub
		Public Sub AddLot(oLot As TplnLot)
			If Not mhsLots.Contains(oLot) Then
				mhsLots.Add(oLot)
				mdAcadArea += oLot.AcadArea(False)
				If oLot.ForcedArea <> 0.0 Then
					mcolLotForcedArea.Add(New BalanceArea.ConstArea(oLot.TopoID, oLot.ForcedArea))
				End If

				doBoundingBox.Union(oLot.BoundingBox)
			End If

		End Sub



		Private Shared Sub zzInitCentroid()
			Try


				Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidProxBlockName, True)
				'    DMCommon.Functions.DispArray(saBlockAttribTag, "!!saBlockAttribTag", True)
				If saBlockAttribTag IsNot Nothing Then
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msMithamNoAttribTag
								miaBlockAttribIndex(enRegionCentroidAttribIndices.MithamNo) = iAttribIndex
							Case msMithamNameAttribTag
								miaBlockAttribIndex(enRegionCentroidAttribIndices.MithamName) = iAttribIndex
							Case msForcedAreaAttribTag
								miaBlockAttribIndex(enRegionCentroidAttribIndices.ForcedArea) = iAttribIndex

						End Select
					Next

				End If
				'   DMCommon.Functions.DispArray(miaBlockAttribIndex, "!!saBlockAttribTag")
				'		DMCommon.Debug.ExcelLog.SetNextValue(0, "msCentroidBlockName", msCentroidBlockName, msCentroidProxBlockName)
				'		DMCommon.Debug.ExcelLog.SetEnumerable(0, "saBlockAttribTag", saBlockAttribTag)
				'		DMCommon.Debug.ExcelLog.SetEnumerable(0, "miaBlockAttribIndex", miaBlockAttribIndex)

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBlock - Initialize")
			End Try
			'    DMCommon.Functions.DispArray(miaBlockAttribIndex, "!!Initialize")
		End Sub
		Private Sub zzLoadBlockRefData(ByVal tBlockAcObjId As ObjectId)
			Dim iaBlockAttribIndex() As Integer = miaBlockAttribIndex '{0, 2, 3, 4, 5}
			'	ddgaParseAttribute = mdgaParseAttribute
			'''''''''''''''''''	dsaBlockAttribText = moAcadBlockDef.GetBlockInfo(tBlockAcObjId, moAcadBlockDef.AttributesID, dbAcadPoint)
			dsaBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, True, dbAcadPoint, iaBlockAttribIndex)



			If IsArray(dsaBlockAttribText) Then

				'  DMCommon.Functions.DispArray(dsaBlockAttribText, "LoadBlockRefData", True, "|" & vbCrLf)
				mtRegionData = New RegionData(dsaBlockAttribText)
				If mtRegionData.IsError Then
					DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidX, "", "Attribute problem 2", False)
				End If


				'   System.Windows.Forms.MessageBox.Show(CStr(BlockStatus) & vbCrLf & Me.BlockStatusName, "07_301")
			End If
		End Sub

		Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
			Get
				Return New Integer() {1, 2, 3}
			End Get
		End Property

		Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
			Get
				Return miaBlockAttribIndex
			End Get
		End Property

		Public Overrides Sub Terminate()

		End Sub

		Public Overrides ReadOnly Property BlockExists As Boolean
			Get
				Return RegionNo <> 0 AndAlso HasCentroid
			End Get
		End Property
	End Class
End Namespace