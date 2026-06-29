Option Explicit On
Option Strict On
Imports DMAcadExt
Imports Autodesk.Gis.Map.Topology
Imports System.Data
Namespace TPlanGraph
   Public Structure LotData

		Dim Name As String
      Dim Order As Integer
      Dim LanduseID As Integer
      Dim LanduseName As String
      Dim LanduseOrder As Integer
      Dim PlanName As String
      Dim InPlan As Boolean
		Dim RegionNo As Integer
		Dim ForcedArea As Double
		Dim Exists As Boolean
		Public Sub New(saValues() As String)

			If saValues IsNot Nothing Then
				'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!saValues", saValues)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "miaApprovedBlockAttribIndex", miaApprovedBlockAttribIndex)
				Dim iAttribUB As Integer = -1
				'DMCommon.Debug.ExcelLog.SetEnumerable(0, "LotData_New", saValues)
				Try

					iAttribUB = saValues.GetUpperBound(0)

				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_1")
				End Try
				If iAttribUB >= 0 Then
					'   DMCommon.ExcelLogC.SetNextValue(iRow, 0, iAttribUB)
					'   DMCommon.ExcelLogC.SetArray(saValues, 4)

					Try
						Name = saValues(0).Trim()
						Order = TplnBasicPgon.GetNameOrder(Name, True)
						'	MyBase.SetName(saValues(0).Trim(), TplnLot.NameIsNum)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_2")
					End Try
				End If
				If iAttribUB >= 1 Then
					Try
						zzParseLanduseString(saValues(1))
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_3")
					End Try
				End If

				If iAttribUB >= 2 Then
					Try
						'zzParseInPlanStringOld(saValues(2))
						TplnRegion.ParseInPlanString(saValues(2), InPlan, RegionNo)

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New Lot_4")
					End Try
				Else
					TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnLot - New_9")
				End If
				If iAttribUB >= 3 Then
					Try
						PlanName = saValues(3)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New Lot_5")
					End Try
				Else
					TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnLot - New_11")
				End If
				If iAttribUB >= 4 AndAlso saValues(4) IsNot Nothing Then
					Try

						zzParseForcedAreaString(saValues(4), True)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New Lot_6")
					End Try
				Else
					'  TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnLot - New_12")
				End If
				'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "LotBlockAttr", saValues)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Block In", iAttribUB, Name, InPlan, RegionNo)

				Exists = True
			End If

		End Sub
		Public Function GetValues() As String()
			Dim sName As String = DMCommon.Functions.CStrN(Name)
			Dim sLanduseID As String = DMCommon.Functions.IntToString(LanduseID)

			Dim InPlan As String = zzInPlanToString()

			Dim sPlanName As String = DMCommon.Functions.CStrN(PlanName)
			Dim sForcedArea As String = ForcedArea.ToString()
			Return New String() {sName, sLanduseID, InPlan, sPlanName, sForcedArea}

		End Function

		Private Shared Function zzNN(sVal As String) As String
			If sVal Is Nothing Then
				Return "<Nothing>"
			Else
				Return sVal
			End If
		End Function
		Private Sub zzParseLanduseString(ByVal sValue As String)
			'MyBase.diTopoPurpose
			sValue = sValue.Trim()
			If sValue.Length = 0 Then
				LanduseID = 0
			Else
				Try
					LanduseID = Convert.ToInt32(sValue)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & vbCrLf & vbCrLf & "'" & sValue & "'" & ":" & CStr(sValue.Length) & ":" & CStr(Asc(sValue)), "TplnLot - zzParseLanduseString_1")
				End Try
				Try
					If False And LanduseID <> 0 Then
						Dim oLanduseItem As TPlServerDB.TPlLanduseItem = TplnProject.GetLanduseItem(LanduseID)
						If oLanduseItem IsNot Nothing Then
							LanduseName = oLanduseItem.Name
							LanduseOrder = oLanduseItem.OrderID
						End If
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - zzParseLanduseString_2")
				End Try
			End If
		End Sub
		Private Sub zzParseForcedAreaString(ByVal sValue As String, bDunam As Boolean)
			'MyBase.diTopoPurpose
			sValue = sValue.Trim()
			If sValue.Length = 0 Then
				ForcedArea = 0.0
			Else
				If Double.TryParse(sValue, ForcedArea) AndAlso bDunam Then

					ForcedArea = ForcedArea * 1000.0
					'  DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "DVA", "חלקה - מק" & " #" & CStr(Me.TopoID), False)
				End If

			End If
		End Sub

		Private Function zzInPlanToString() As String
			If RegionNo = TplnRegion.RegionIn Then
				If InPlan Then
					Return "IN"
				Else
					Return "OUT"
				End If

			Else
				Return Convert.ToString(RegionNo)
			End If

		End Function
	End Structure
	Public Class TplnLot
		Inherits TPlanGraph.TplnBasicPgon



#Region "Declarations"
		Public Const ID_Debug As Integer = 346
#Region "Constants"
		Private Enum enLotCentroidAttribIndices
			Name
			LanduseCode
			'Area
			InPlan
			PlanName
			ForcedArea
			UB = ForcedArea
		End Enum
		Private Const msApprovedTopoName As String = "LotsK"
		Private Const msProposedTopoName As String = "LotsM"
		Private Const msMergeProposedTopoName As String = "LotMParcel"
		Private Const msMergeApprovedTopoName As String = "LotKParcel"
		Private Const msUnionProposedTopoName As String = "LotMParcel_U"
		Private Const msUnionApprovedTopoName As String = "LotKParcel_U"

		Private Shared mtApprMapThemeData As DMAcadExt.MapThemeData
		Private Shared mtPropMapThemeData As DMAcadExt.MapThemeData

		Private Shared msCentroidBlockName As String   '' = "CellNo" ' "PLNS008"  default
		Private Shared msCentroidApprovedBlockName As String
		Private Shared msCentroidProposedBlockName As String


		Private Const msNameAttribTag As String = "CELLNO"
		Private Const msLanduseAttribTag As String = "CODE"
		Private Const msInPlanAttribTag As String = "PLAN"
		Private Const msPlanNameAttribTag As String = "PLANNUM"
		Private Const msForcedAreaAttribTag As String = "FORCED_AREA"




		Public Const NameFieldName As String = "LotName"
		Friend Const LotOrderFieldName As String = "LotOrder"

		Public Const msCalcAreaFieldName As String = "CalcArea"
		Public Const msCalcArea2FieldName As String = "CalcArea2"
		Public Const msRoundedAreaFieldName As String = "RoundedArea"
		Public Const msCalcGroupArea2FieldName As String = "CalcGroupArea2"

		Public Const msCalcAreaUnionFieldName As String = "CalcAreaUn"
		Public Const msCalcArea2UnionFieldName As String = "CalcArea2Un"
		Public Const msRoundedAreaUnionFieldName As String = "RoundedAreaUn"

		Public Const msCalcAreaFDO_OverlayFieldName As String = "CalcAreaFO"
		Public Const msCalcArea2FDO_OverlayFieldName As String = "CalcArea2FO"
		Public Const msRoundedAreaFDO_OverlayFieldName As String = "RoundedAreaFO"
		Public Const msCalcGroupArea2FDO_OverlayFieldName As String = "CalcGroupArea2FO"

		Public Const msForcedAreaFieldName As String = "ForcedArea"
		Public Const msRegionFieldName As String = "Region"
		Public Const msRegionNameFieldName As String = "RegionName"






		Friend Const msLanduseOrderFieldName As String = "LanduseOrder"
		Public Const msAreaApprFieldName As String = "AreaAppr"
		Public Const msAreaPctApprFieldName As String = "AreaPctAppr"
		Public Const msAreaPropFieldName As String = "AreaProp"
		Public Const msAreaPctPropFieldName As String = "AreaPctProp"
		Private Const msInPlanFieldName As String = "InPlan"
		Private Const msColorFieldName As String = "Color"
		Private Const mdPgonAreaTolearance As Double = 0.002

#End Region
#Region "Instance Vars"
		Private mtLotData As LotData
		'	Private mbInPlan As Boolean
		'	Private miLanduseID As Integer = 0
		Private msLanduseName As String = "-"
		'	Private msLanduseNameAppr As String = "-"
		'	Private msLanduseNameProp As String = "-"
		Private miLanduseOrder As Integer = 0
		Private miBasicPlanID As Integer
		Private Shared miInstanceCount As Integer

#End Region
#Region "Shared Vars"
		Private Shared miaApprovedBlockAttribIndex(enLotCentroidAttribIndices.UB) As Integer
		Private Shared miaProposedBlockAttribIndex(enLotCentroidAttribIndices.UB) As Integer



		Private Shared moApprovedDataTable As System.Data.DataTable
		Private Shared moProposedDataTable As System.Data.DataTable





		Private Shared mdicSumLanduses As TplnLanduses

		Private Shared mdInPlanSumCalcAreaProp As Double
		Private Shared mdInPlanSumCalcAreaUnProp As Double
		Private Shared mdInPlanSumCalcAreaAppr As Double
		Private Shared mdInPlanSumCalcAreaUnAppr As Double

		Private Shared mdInPlanSumAcadAreaProp As Double
		Private Shared mdInPlanSumAcadAreaAppr As Double
		Private Shared mbNameIsNum As Boolean
		Private Shared mdInPlanSumCalcArea As UnionPgonArea = New UnionPgonArea
		Private Shared mdInPlanSumAcadArea As UnionPgonArea = New UnionPgonArea
		Private Shared moMainHiddenColumnsAppr As Dictionary(Of Integer, Integer)
		Private Shared moMainHiddenColumnsProp As Dictionary(Of Integer, Integer)
		Private Shared mbHasRegions As Boolean

#End Region
#End Region
#Region "Shared members"
		Public Shared Sub InitializeOldVer()
			Try
				'	System.Windows.Forms.MessageBox.Show(CStr(miaApprovedBlockAttribIndex Is Nothing), "01_400")
				msCentroidApprovedBlockName = TopoDefs.Item(New DMAcadExt.TopoDefID(enTopoPurpose.Approved)).CentroidBlocks(0)
				msCentroidProposedBlockName = TopoDefs.Item(New DMAcadExt.TopoDefID(enTopoPurpose.Proposed)).CentroidBlocks(0)
				zzInitBlockAttribIndex(msCentroidApprovedBlockName, miaApprovedBlockAttribIndex)
				zzInitBlockAttribIndex(msCentroidProposedBlockName, miaProposedBlockAttribIndex)
				mdicSumLanduses = New TplnLanduses(DMAcadExt.enTopoPurpose.Parcel)
				' DMCommon.Functions.DispArray(miaApprovedBlockAttribIndex, "miaApprovedBlockAttribIndex")
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - Initialize")
			End Try
		End Sub
		Public Shared Sub Initialize(tMapThemeData As DMAcadExt.MapThemeData)
			'DMCommon.Debug.MsgBox("03_704", miInstanceCount, tMapThemeData.TopoPurpose)
			If miInstanceCount = 0 Then

				Try



					miInstanceCount += 1

					If tMapThemeData.TopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
						mtApprMapThemeData = tMapThemeData
						msCentroidApprovedBlockName = mtApprMapThemeData.CentroidBlocks
					ElseIf tMapThemeData.TopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
						mtPropMapThemeData = tMapThemeData
						msCentroidProposedBlockName = mtPropMapThemeData.CentroidBlocks
					End If

					'DMCommon.Debug.MsgBox("04_596", msCentroidApprovedBlockName, msCentroidProposedBlockName, tMapThemeData.MapThemeID)

					If Not String.IsNullOrEmpty(msCentroidApprovedBlockName) Then
						zzInitBlockAttribIndex(msCentroidApprovedBlockName, miaApprovedBlockAttribIndex)
					End If
					If Not String.IsNullOrEmpty(msCentroidProposedBlockName) Then
						zzInitBlockAttribIndex(msCentroidProposedBlockName, miaProposedBlockAttribIndex)
					End If


					If mdicSumLanduses Is Nothing Then

						mdicSumLanduses = New TplnLanduses(DMAcadExt.enTopoPurpose.Parcel)
					Else
						mdicSumLanduses.Clear()
					End If


					'DMCommon.Functions.DispArray("miaApprovedBlockAttribIndex", miaApprovedBlockAttribIndex)
					'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!Lot.InitApprove", miaApprovedBlockAttribIndex)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, msCentroidApprovedBlockName)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - Initialize")
				End Try

			End If

		End Sub

		Public Shared Sub Initialize(tApprMapThemeData As DMAcadExt.MapThemeData, tPropMapThemeData As DMAcadExt.MapThemeData)
			If miInstanceCount = 0 Then


				Try
					miInstanceCount += 1
					'MapThemeData.
					'	System.Windows.Forms.MessageBox.Show(tApprMapThemeData.TopoName, "##TplnLot - Initialize")
					DMCommon.Debug.MsgBoxLoop("C04_02", 3, miInstanceCount, tApprMapThemeData.TopoName, tPropMapThemeData.TopoName, GetSumLanduseCount())
					mtApprMapThemeData = tApprMapThemeData
					mtPropMapThemeData = tPropMapThemeData

					msCentroidApprovedBlockName = mtApprMapThemeData.CentroidBlocks
					msCentroidProposedBlockName = mtPropMapThemeData.CentroidBlocks

					'DMCommon.Debug.MsgBox("07_391", msCentroidApprovedBlockName, tApprMapThemeData.MapThemeID, tPropMapThemeData.MapThemeID)
					If Not String.IsNullOrEmpty(msCentroidApprovedBlockName) Then
						zzInitBlockAttribIndex(msCentroidApprovedBlockName, miaApprovedBlockAttribIndex)


					End If
					If Not String.IsNullOrEmpty(msCentroidProposedBlockName) Then
						zzInitBlockAttribIndex(msCentroidProposedBlockName, miaProposedBlockAttribIndex)
					End If


					If Not String.IsNullOrEmpty(msCentroidProposedBlockName) Then
						zzInitBlockAttribIndex(msCentroidProposedBlockName, miaProposedBlockAttribIndex)
						' DMCommon.Functions.DispArray(miaProposedBlockAttribIndex, "miaProposedBlockAttribIndex")
					End If

					DMCommon.Debug.MsgBoxLoop("C04_06", 3, GetSumLanduseCount(), GetSumLanduseTopoPurpose())

					If mdicSumLanduses IsNot Nothing Then

						mdicSumLanduses = New TplnLanduses(DMAcadExt.enTopoPurpose.Parcel)
					End If

					'  DMCommon.Functions.DispArray(miaApprovedBlockAttribIndex, "2miaApprovedBlockAttribIndex")
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - Initialize")
				End Try
			End If

		End Sub
		Public Shared ReadOnly Property MapThemeData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As DMAcadExt.MapThemeData
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					Return mtApprMapThemeData
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return mtPropMapThemeData
				Else
					Return Nothing
				End If
			End Get

		End Property
		Public Shared Function GetTopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Dim sLotLineTopoName As String
			If iTopoPurpose = enTopoPurpose.Approved Then

				sLotLineTopoName = mtApprMapThemeData.LineTopoName
				If TopoCreator.TopologyExists(sLotLineTopoName) Then
					Return sLotLineTopoName
				Else
					Return mtApprMapThemeData.TopoName
				End If
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				sLotLineTopoName = mtPropMapThemeData.LineTopoName
				If TopoCreator.TopologyExists(sLotLineTopoName) Then
					Return sLotLineTopoName
				Else
					Return mtPropMapThemeData.TopoName
				End If



			Else
				Return Nothing
			End If

		End Function
		Public Shared Function GetDissolveTopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				Return mtApprMapThemeData.DissolveTopoName
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				Return mtPropMapThemeData.DissolveTopoName
			Else
				Return Nothing
			End If

		End Function
		Public Shared ReadOnly Property HasRegions As Boolean
			Get
				Return mbHasRegions
			End Get
		End Property

		Public Shared ReadOnly Property InPlanAttribTag As String
			Get
				Return msInPlanAttribTag
			End Get
		End Property
		Private Shared Sub zzInitBlockAttribIndex(ByVal sCentroidBlockName As String, ByRef iaBlockAttribIndex() As Integer)
			Try
				Dim saBlockAttribTag() As String = AcadTransaction.GetAttribDef(sCentroidBlockName, True, True)
				'   AcadDocument.WriteMessageLog("^^^ " & DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", False))
				'    DMCommon.Functions.DispArray(miaApprovedBlockAttribIndex, "miaApprovedBlockAttribIndex")
				'	DMCommon.Functions.DispArray(saBlockAttribTag, "Lots:saBlockAttribTag", True)
				'	DMCommon.Debug.MsgBox("_496_" & sCentroidBlockName, saBlockAttribTag)
				If saBlockAttribTag IsNot Nothing Then
					'DMCommon.Functions.DispArray(saBlockAttribTag, "01_599Lot", True)
					For iAttribIndex As Integer = 0 To iaBlockAttribIndex.GetUpperBound(0)
						iaBlockAttribIndex(iAttribIndex) = -1
					Next
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case Strings.UCase(saBlockAttribTag(iAttribIndex))
							Case msNameAttribTag
								iaBlockAttribIndex(enLotCentroidAttribIndices.Name) = iAttribIndex
							Case msLanduseAttribTag
								iaBlockAttribIndex(enLotCentroidAttribIndices.LanduseCode) = iAttribIndex
							Case msInPlanAttribTag
								iaBlockAttribIndex(enLotCentroidAttribIndices.InPlan) = iAttribIndex
								'DMCommon.Debug.MsgBox("_4446_" & sCentroidBlockName, enLotCentroidAttribIndices.InPlan, iAttribIndex)
							Case msPlanNameAttribTag
								iaBlockAttribIndex(enLotCentroidAttribIndices.PlanName) = iAttribIndex
							Case msForcedAreaAttribTag
								iaBlockAttribIndex(enLotCentroidAttribIndices.ForcedArea) = iAttribIndex
							Case Else

						End Select
					Next
					'	DMCommon.Debug.MsgBox("04_596c", iaBlockAttribIndex)
				End If
				'   DMCommon.Functions.DispArray(iaBlockAttribIndex, "!!iaBlockAttribIndex")
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & DMCommon.Functions.CStrN(sCentroidBlockName, "Nothing"), "TplnLot - zzInitBlockAttribIndex")
			End Try
		End Sub
		Public Shared Sub SharedTerminate()
			If moProposedDataTable IsNot Nothing Then
				moProposedDataTable.Dispose()
				moProposedDataTable = Nothing
			End If

			If moApprovedDataTable IsNot Nothing Then
				moApprovedDataTable.Dispose()
				moApprovedDataTable = Nothing
			End If


			If mdicSumLanduses IsNot Nothing Then
				mdicSumLanduses.Clear()
				mdicSumLanduses = Nothing
			End If
		End Sub
		Public Shared Sub Reset(ByVal iTopoPurpose As enTopoPurpose)
			mdicSumLanduses = Nothing
			Select Case iTopoPurpose
				Case enTopoPurpose.Approved
					mdInPlanSumCalcAreaAppr = 0.0
					mdInPlanSumCalcAreaUnAppr = 0.0
					mdInPlanSumAcadAreaAppr = 0.0
				Case enTopoPurpose.Proposed
					mdInPlanSumCalcAreaProp = 0.0
					mdInPlanSumCalcAreaUnProp = 0.0
					mdInPlanSumAcadAreaProp = 0.0
			End Select
		End Sub
		Public Shared Sub Dispose(ByVal iTopoPurpose As enTopoPurpose)
			Select Case iTopoPurpose
				Case enTopoPurpose.Approved
					If moApprovedDataTable IsNot Nothing Then
						moApprovedDataTable.Dispose()
						moApprovedDataTable = Nothing
					End If
				Case enTopoPurpose.Proposed
					If moProposedDataTable IsNot Nothing Then
						moProposedDataTable.Dispose()
						moProposedDataTable = Nothing
					End If
			End Select
		End Sub
		Public Shared Sub GetLanduseList(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef oDataView As System.Data.DataView, ByVal dColorSchemeScale As Double)
			'	MessageBox.Show(CStr("GetLanduseList") & vbCrLf & "", "04_306d")
			Dim oDataTable As System.Data.DataTable = New DataTable(TopoName(iTopoPurpose))
			Dim iLanduseOrder As Integer
			With oDataTable.Columns
				.Add(NameFieldName, GetType(System.String))
				.Add(msColorFieldName, GetType(DMAcadExt.ColorScheme))
				.Add(msLanduseOrderFieldName, GetType(System.Int32))
			End With

			Dim oNewRow As System.Data.DataRow
			Dim tColorScheme As ColorScheme
			For Each oLanduse As TplnLanduse In mdicSumLanduses.Values
				If oLanduse.HasLots(iTopoPurpose) Then
					oNewRow = oDataTable.NewRow()
					tColorScheme = oLanduse.GetColorScheme(iTopoPurpose, dColorSchemeScale, iLanduseOrder)
					oNewRow.Item(NameFieldName) = tColorScheme.Name

					oNewRow.Item(msColorFieldName) = tColorScheme
					oNewRow.Item(msLanduseOrderFieldName) = iLanduseOrder
					oDataTable.Rows.Add(oNewRow)
				End If
			Next
			oDataView = New DataView(oDataTable)
			oDataView.Sort = msLanduseOrderFieldName

		End Sub
		Public Shared Sub RefreshColorSchemes(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

			If mdicSumLanduses IsNot Nothing Then
				MessageBox.Show(CStr(mdicSumLanduses.Count), "04_120")
				For Each oLanduse As TplnLanduse In mdicSumLanduses.Values
					If oLanduse.HasLots(iTopoPurpose) Then
						oLanduse.SetColorScheme(iTopoPurpose, True)
					End If
				Next
			Else
				MessageBox.Show(CStr("mdicSumLanduses IsNot Nothing"), "04_123")
			End If
		End Sub
		Public Shared Sub CreateMainDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			Dim oDataTable As System.Data.DataTable = New DataTable(TopoName(iTopoPurpose))
			Dim oOrder As System.Type
			Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
			'	If mbNameIsNum Then
			oOrder = GetType(System.Int32)
			'		Else
			'	oOrder = GetType(System.STRING)
			'	End If
			With oDataTable.Columns
				.Add(NameFieldName, GetType(System.String))                           '0
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))            '1
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))         '2
				.Add(TopoReader.msAreaFldName, GetType(System.Double))                  '3


				.Add(TopoReader.msSumPgonAreaFldName, GetType(System.Double))        '4

				.Add(msCalcAreaFieldName, GetType(System.Double))                    '5
				.Add(msCalcArea2FieldName, GetType(System.Double))                   '6
				.Add(msRoundedAreaFieldName, GetType(System.Double))                 '7

				.Add(TopoReader.msSumPgonAreaFDO_OverlayFldName, GetType(System.Double))         '8
				.Add(msCalcAreaFDO_OverlayFieldName, GetType(System.Double))                     '9
				.Add(msCalcArea2FDO_OverlayFieldName, GetType(System.Double))                 '10
				.Add(msRoundedAreaFDO_OverlayFieldName, GetType(System.Double))                  '11
				.Add(msForcedAreaFieldName, GetType(System.Double))
				.Add(TplnParcel.msToleranceFieldName, GetType(System.Double)) '081025
				.Add(TplnParcel.msDeltaAreaFieldName, GetType(System.Double)) '081025
				.Add(TplnParcel.msDeviationFieldName, GetType(System.Double)) '081025

				'	.Add(msCalcAreaUnFieldName, GetType(System.Double))

				'	.Add(TopoReader.msSumPgonAreaUnFldName, GetType(System.Double))

				.Add(msRegionFieldName, GetType(System.Int32))
				.Add(msRegionNameFieldName, GetType(System.String))


				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))          '12 8
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))          '13 9

				.Add(msInPlanFieldName, GetType(System.Boolean))                    '14 10
				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))          '15 11
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))                 '16 12
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))          '17 13
				.Add(msLanduseOrderFieldName, GetType(System.Int32))                    '18 14
				.Add(LotOrderFieldName, oOrder)                                                      '19 15
				.Add(msCalcGroupArea2FieldName, GetType(System.Double))                                                      '19 15

			End With
			If iTopoPurpose = enTopoPurpose.Approved Then
				moApprovedDataTable = oDataTable
				moMainHiddenColumnsAppr = dicMainHiddenColumns
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				moProposedDataTable = oDataTable
				moMainHiddenColumnsProp = dicMainHiddenColumns
			End If
		End Sub


		Public Shared ReadOnly Property MainHiddenColumns(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Dictionary(Of Integer, Integer)
			Get
				Select Case iTopoPurpose
					Case DMAcadExt.enTopoPurpose.Approved
						Return moMainHiddenColumnsAppr
					Case DMAcadExt.enTopoPurpose.Approved
						Return moMainHiddenColumnsProp
					Case Else
						Return Nothing
				End Select
			End Get
		End Property
		Public Shared Property NameIsNum() As Boolean
			Get
				Return mbNameIsNum
			End Get
			Set(ByVal bValue As Boolean)
				mbNameIsNum = bValue
			End Set
		End Property
		Public Shared ReadOnly Property InPlanFilter() As String
			Get
				Return msInPlanFieldName
			End Get
		End Property
		Public Shared ReadOnly Property PlanDissolveAttribExpr() As String
			Get
				Return "@" & msInPlanAttribTag
			End Get
		End Property
		Public Shared ReadOnly Property MainDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As System.Data.DataTable
			Get
				Return zzDataTable(iTopoPurpose)
			End Get
		End Property
		Public Shared ReadOnly Property MainView(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bInPlan As Boolean) As System.Data.DataView
			Get
				Dim oDataTable As System.Data.DataTable = zzDataTable(iTopoPurpose)
				If oDataTable IsNot Nothing Then
					Dim sFilter As String = msInPlanFieldName
					If bInPlan Then
						sFilter = msInPlanFieldName
					Else
						sFilter = String.Empty
					End If
					Dim sSort As String = msLanduseOrderFieldName & "," & LotOrderFieldName
					Dim oDataView As System.Data.DataView = New System.Data.DataView(oDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Shared Sub GetMainData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim oDataTable As System.Data.DataTable = zzDataTable(iTopoPurpose)
			Dim sAreaFldName As String
			Dim iAreaFldIndex As Integer

			DMCommon.Debug.ExcelLog.SetNextValue(0, "Param Lots", iTopoPurpose, iOptions, iOverlayMethod, iRegion)
			DMCommon.Debug.ExcelLog.SetDataTable(0, "LotMainData", oDataTable)

			If oDataTable IsNot Nothing Then

				ReDim iaColumns(2)

				Select Case iOptions
					Case enDataOptions.AcadArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = TopoReader.msAreaFldName
							iAreaFldIndex = 3
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							sAreaFldName = TopoReader.msSumPgonAreaFDO_OverlayFldName
							iAreaFldIndex = 8
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined Then
							sAreaFldName = TopoReader.msAreaFldName
							iAreaFldIndex = 3

						End If
					Case enDataOptions.CalcMergeArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = msCalcAreaFieldName
							iAreaFldIndex = 5
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							sAreaFldName = msCalcAreaFDO_OverlayFieldName
							iAreaFldIndex = 9
						End If
					Case enDataOptions.CalcMergeArea2
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = msCalcArea2FieldName
							iAreaFldIndex = 6
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							If True OrElse iRegion = 0 Then
								sAreaFldName = msCalcArea2FDO_OverlayFieldName
								iAreaFldIndex = 10
							Else
								sAreaFldName = msCalcGroupArea2FDO_OverlayFieldName
								iAreaFldIndex = 21
							End If

						End If

					Case enDataOptions.RoundedArea

						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = msRoundedAreaFieldName
							iAreaFldIndex = 7
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							sAreaFldName = msRoundedAreaFDO_OverlayFieldName
							iAreaFldIndex = 11
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined Then
							sAreaFldName = msRoundedAreaFieldName
							iAreaFldIndex = 7
						End If

					Case Else
						sAreaFldName = String.Empty
				End Select
				'    DMCommon.Debug.MsgBox("iAreaFldIndex=", iAreaFldIndex.ToString())
				'   DMCommon.Debug.MsgBox("Item(iAreaFldIndex).ColumnName", oDataTable.Columns.Item(iAreaFldIndex).ColumnName)


				iaColumns(0) = 0
				iaColumns(1) = 2
				iaColumns(2) = iAreaFldIndex
				If iRegion = 0 Then
				Else
				End If

				Dim sFilter As String = Nothing
				If iRegion = 0 AndAlso hsRegions Is Nothing Then
					sFilter = "" ' msRegionFieldName & "<>0" ' msInPlanFieldName
					sFilter = msInPlanFieldName & "=True"

				ElseIf iRegion <> 0 Then
					sFilter = msRegionFieldName & "=" & iRegion.ToString()
				ElseIf iRegion = 0 AndAlso hsRegions IsNot Nothing Then
					zzAddRegionsFilter(hsRegions, sFilter)
				End If

				Dim sSort As String = msLanduseOrderFieldName & "," & LotOrderFieldName    'msLanduseOrderFieldName & "," &

				'  Dim sSort As String = msRoundedAreaFieldName & " DESC"   'msLanduseOrderFieldName & "," &
				'DEBUG	System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count) & vbCrLf & iTopoPurpose.ToString() & vbCrLf & iOptions.ToString() & vbCrLf & sFilter & vbCrLf & sSort, "03_310")
				oDataView = New System.Data.DataView(oDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				'DEBUG	System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count), "03_311a")
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False

				Dim dSumA As Double = 0.0, dSumB As Double = 0.0
				Dim oDataRowView As DataRowView

				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					dSumA += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaFldIndex))
				Next
				ReDim oaTotals(0)
				oaTotals(0) = dSumA
				DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaColumns Lots", iaColumns)
				DMCommon.Debug.MsgBox("dSumA", dSumA.ToString(), oDataView.Count, iAreaFldIndex.ToString())
			End If
		End Sub
		Private Shared Sub zzAddRegionsFilter(hsRegions As HashSet(Of Integer), ByRef sFilter As String)
			For Each iRegion As Integer In hsRegions
				If Not String.IsNullOrEmpty(sFilter) Then
					sFilter &= " OR "
				End If
				sFilter &= msRegionFieldName & "=" & iRegion.ToString()
			Next
		End Sub
		Public Shared Sub GetMainData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal bEntirety As Boolean, iRegion As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim oDataTable As System.Data.DataTable = zzDataTable(iTopoPurpose)
			Dim sAreaFldName As String
			Dim iAreaFldIndex As Integer
			Dim iOverlayMethod As DMAcadExt.enOverlayMethod

			DMCommon.Debug.ExcelLog.SetNextValue(0, "Param Lots", iTopoPurpose, iOptions, iRegion)
			DMCommon.Debug.ExcelLog.SetDataTable(0, "LotMainData", oDataTable)

			If oDataTable IsNot Nothing Then

				ReDim iaColumns(2)

				Select Case iOptions
					Case enDataOptions.AcadArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = TopoReader.msAreaFldName
							iAreaFldIndex = 3
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							sAreaFldName = TopoReader.msSumPgonAreaFDO_OverlayFldName
							iAreaFldIndex = 8
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined Then
							sAreaFldName = TopoReader.msAreaFldName
							iAreaFldIndex = 3

						End If
						sAreaFldName = TopoReader.msAreaFldName
						iAreaFldIndex = 3

					Case enDataOptions.RoundedArea

						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							sAreaFldName = msRoundedAreaFieldName
							iAreaFldIndex = 7
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							sAreaFldName = msRoundedAreaFDO_OverlayFieldName
							iAreaFldIndex = 11
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined Then
							sAreaFldName = msRoundedAreaFieldName
							iAreaFldIndex = 7
						End If
						sAreaFldName = msRoundedAreaFieldName
						iAreaFldIndex = 7
					Case Else
						sAreaFldName = String.Empty
				End Select
				'    DMCommon.Debug.MsgBox("iAreaFldIndex=", iAreaFldIndex.ToString())
				'   DMCommon.Debug.MsgBox("Item(iAreaFldIndex).ColumnName", oDataTable.Columns.Item(iAreaFldIndex).ColumnName)


				iaColumns(0) = 0
				iaColumns(1) = 2
				iaColumns(2) = iAreaFldIndex
				If iRegion = 0 Then
				Else
				End If

				Dim sFilter As String = Nothing
				If iRegion = TPlanGraph.TplnRegion.RegionAllIn AndAlso hsRegions Is Nothing Then

					sFilter = msInPlanFieldName & "=True"

				ElseIf iRegion <> TPlanGraph.TplnRegion.RegionAllIn Then
					sFilter = msRegionFieldName & "=" & iRegion.ToString()
				ElseIf iRegion = TPlanGraph.TplnRegion.RegionAllIn AndAlso hsRegions IsNot Nothing Then
					zzAddRegionsFilter(hsRegions, sFilter)
				End If

				Dim sSort As String = msLanduseOrderFieldName & "," & LotOrderFieldName    'msLanduseOrderFieldName & "," &

				'  Dim sSort As String = msRoundedAreaFieldName & " DESC"   'msLanduseOrderFieldName & "," &
				'DEBUG	System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count) & vbCrLf & iTopoPurpose.ToString() & vbCrLf & iOptions.ToString() & vbCrLf & sFilter & vbCrLf & sSort, "03_310")
				oDataView = New System.Data.DataView(oDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				DMCommon.Debug.MsgBox("!GetMainData", oDataTable.Rows.Count, sFilter, sSort, oDataView.Count, iOptions)
				'DEBUG	System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count), "03_311a")
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False

				Dim dSumA As Double = 0.0, dSumB As Double = 0.0
				Dim oDataRowView As DataRowView

				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					dSumA += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaFldIndex))
				Next
				ReDim oaTotals(0)
				oaTotals(0) = dSumA
				DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaColumns Lots", iaColumns)
				DMCommon.Debug.MsgBox("dSumA", dSumA.ToString(), oDataView.Count, iAreaFldIndex.ToString())
			End If
		End Sub
		Public Shared Sub GetMainData290712(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim oDataTable As System.Data.DataTable = zzDataTable(iTopoPurpose)
			Dim sAreaFldName As String
			Dim iAreaFldIndex As Integer

			If oDataTable IsNot Nothing Then
				Select Case iOptions
					Case enDataOptions.AcadArea, enDataOptions.Default
						Dim iaAcadColumns() As Integer = {0, 2, 3}
						iaColumns = iaAcadColumns
						sAreaFldName = TopoReader.msAreaFldName
						iAreaFldIndex = 3
					Case enDataOptions.CalcMergeArea
						Dim iaAcadColumns() As Integer = {0, 2, 5}
						iaColumns = iaAcadColumns
						sAreaFldName = msCalcAreaFieldName
						iAreaFldIndex = 5
					Case enDataOptions.CalcMergeArea2
						Dim iaAcadColumns() As Integer = {0, 2, 6}
						iaColumns = iaAcadColumns
						sAreaFldName = msCalcArea2FieldName
						iAreaFldIndex = 6
					Case enDataOptions.RoundedArea
						Dim iaAcadColumns() As Integer = {0, 2, 7}
						iaColumns = iaAcadColumns
						sAreaFldName = msRoundedAreaFieldName
						iAreaFldIndex = 7
					Case enDataOptions.CalcMergeArea, enDataOptions.CalcUnionArea
						sAreaFldName = msCalcAreaFieldName
					Case enDataOptions.CalcMergeArea2
						sAreaFldName = msCalcAreaFieldName
					Case Else
						sAreaFldName = String.Empty
				End Select

				Dim sFilter As String = msInPlanFieldName
				Dim sSort As String = msLanduseOrderFieldName & "," & LotOrderFieldName
				oDataView = New System.Data.DataView(oDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False

				Dim dSumA As Double = 0.0, dSumB As Double = 0.0
				Dim oDataRowView As DataRowView
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					dSumA += DirectCast(oDataRowView.Item(iAreaFldIndex), Double)
				Next
				ReDim oaTotals(0)
				oaTotals(0) = dSumA
			End If
		End Sub
		Public Shared ReadOnly Property TopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Get
				If iTopoPurpose = enTopoPurpose.Proposed Then
					Return msProposedTopoName
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					Return msApprovedTopoName
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Shared ReadOnly Property UnionTopoNameB(ByVal iStatus As enTopoPurpose) As String
			Get
				If iStatus = enTopoPurpose.Proposed Then
					Return Common.UnionTopoPrefix & "_" & msUnionProposedTopoName
				ElseIf iStatus = enTopoPurpose.Approved Then
					Return Common.UnionTopoPrefix & "_" & msUnionApprovedTopoName
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Shared ReadOnly Property MergeTopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Get
				If iTopoPurpose = enTopoPurpose.Proposed Then
					Return msMergeProposedTopoName
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					Return msMergeApprovedTopoName
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Shared ReadOnly Property UnionTopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Get
				If iTopoPurpose = enTopoPurpose.Proposed Then
					Return msUnionProposedTopoName
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					Return msUnionApprovedTopoName
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Shared Sub ClearDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			Dim oDataTable As System.Data.DataTable = zzDataTable(iTopoPurpose)
			oDataTable.Rows.Clear()
		End Sub
		Public Shared Sub GetLanduseData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iOptions As enDataOptions, ByRef oDataView As System.Data.DataView, ByRef oaTotals() As System.Object)
			MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(mdInPlanSumAcadAreaAppr) & ":" & iOptions.ToString() & ":" & CStr(mdInPlanSumAcadAreaAppr) & ":" & iOptions.ToString(), "07_179")
			Dim oJointLanduseTable As System.Data.DataTable
			Dim sLanduseNameApprFieldName As String = TplnParcel.LanduseNameFieldName & "Appr"
			Dim sLanduseNamePropFieldName As String = TplnParcel.LanduseNameFieldName & "Prop"
			Dim sLanduseIDApprFieldName As String = TplnParcel.LanduseIDFieldName & "Appr"
			Dim sLanduseIDPropFieldName As String = TplnParcel.LanduseIDFieldName & "Prop"
			Dim iOverlayIndex As enOverlayIndex
			oJointLanduseTable = New DataTable("JointLanduse")
			With oJointLanduseTable.Columns
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					.Add(sLanduseNameApprFieldName, GetType(System.String))
					.Add(msAreaApprFieldName, GetType(System.Double))
					.Add(msAreaPctApprFieldName, GetType(System.Double))
					.Add(sLanduseIDApprFieldName, GetType(System.Int32))
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					.Add(sLanduseNamePropFieldName, GetType(System.String))
					.Add(msAreaPropFieldName, GetType(System.Double))
					.Add(msAreaPctPropFieldName, GetType(System.Double))
					.Add(sLanduseIDPropFieldName, GetType(System.Int32))
				End If
				'	.Add(TplnParcel.msLanduseOrderFieldName, GetType(System.Int32))
			End With

			If mdicSumLanduses Is Nothing OrElse mdicSumLanduses.Count = 0 Then Return
			Dim oNewRow As System.Data.DataRow
			Dim oLanduse As TplnLanduse

			Dim daResAreaAppr(mdicSumLanduses.Count - 1) As Double
			Dim daResAreaProp(mdicSumLanduses.Count - 1) As Double


			Dim daPcntAppr(mdicSumLanduses.Count - 1) As Double
			Dim daPcntProp(mdicSumLanduses.Count - 1) As Double

			Dim oaLandusesAppr(mdicSumLanduses.Count - 1) As TplnLanduse
			Dim oaLandusesProp(mdicSumLanduses.Count - 1) As TplnLanduse

			Dim iIndexAppr As Integer = 0
			Dim iIndexProp As Integer = 0
			Dim iIndexApprUB As Integer
			Dim iIndexPropUB As Integer

			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double

			If iOptions = enDataOptions.CalcMergeArea Then
				dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iOptions = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = Math.Round(dInPlanSumAreaAppr * 0.001, 3, MidpointRounding.AwayFromZero)
				dInPlanSumAreaProp = Math.Round(dInPlanSumAreaProp * 0.001, 3, MidpointRounding.AwayFromZero)
			End If
			dInPlanSumAreaAppr = 0.0
			dInPlanSumAreaProp = 0.0

			Try
				For Each oLanduse In mdicSumLanduses.Values
					If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Approved) Then
						oaLandusesAppr(iIndexAppr) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved)
						daResAreaAppr(iIndexAppr) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, False)
						'	TplnProject.WriteMessageBox(iOverlayIndex.ToString() & vbCrLf & CStr(daResAreaAppr(iIndexAppr)), "01_555")
						dInPlanSumAreaAppr += daResAreaAppr(iIndexAppr)
						iIndexAppr += 1
					End If
					If iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Proposed) Then
						oaLandusesProp(iIndexProp) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed)
						daResAreaProp(iIndexProp) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, False)
						TplnProject.WriteMessageBox(CStr(daResAreaProp(iIndexAppr)), "01_556")
						dInPlanSumAreaProp += daResAreaProp(iIndexProp)
						iIndexProp += 1
					End If
				Next
				iIndexApprUB = iIndexAppr - 1
				iIndexPropUB = iIndexProp - 1

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicSumLanduses.Count.ToString(), "TplnLot - GetSumLanduseData_1c")
			End Try
			ReDim Preserve daResAreaAppr(iIndexApprUB)
			ReDim Preserve daResAreaProp(iIndexPropUB)


			iIndexAppr = 0
			iIndexProp = 0

			''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0 Then
				'	iaPcntAppr = Balance(daPcntAppr)
				oPcntAppr = New BalanceArea(daResAreaAppr, 100.0, 100.0, True, "TplnLot.GetLanduseData_1")
			End If
			If dInPlanSumAreaProp <> 0 Then
				'	iaPcntProp = Balance(daPcntProp)
				oPcntProp = New BalanceArea(daResAreaProp, 100.0, 100.0, True, "TplnLot.GetLanduseData_2")
			End If


			Try

				iIndexAppr = 0
				iIndexProp = 0
				Do
					If iIndexAppr <= iIndexApprUB OrElse iIndexProp <= iIndexPropUB Then
						oNewRow = oJointLanduseTable.NewRow()
					Else
						Exit Do
					End If
					If iIndexAppr <= iIndexApprUB Then
						oLanduse = oaLandusesAppr(iIndexAppr)
						oNewRow.Item(sLanduseNameApprFieldName) = oLanduse.GetName(DMAcadExt.enTopoPurpose.Approved)
						oNewRow.Item(msAreaApprFieldName) = daResAreaAppr(iIndexAppr) 'oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions, True)

						If dInPlanSumAreaAppr <> 0 Then
							oNewRow.Item(msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndexAppr)
						End If
						oNewRow.Item(sLanduseIDApprFieldName) = oLanduse.ID
						iIndexAppr += 1
					End If

					If iIndexProp <= iIndexPropUB Then
						oLanduse = oaLandusesProp(iIndexProp)
						oNewRow.Item(sLanduseNamePropFieldName) = oLanduse.GetName(DMAcadExt.enTopoPurpose.Proposed)
						oNewRow.Item(msAreaPropFieldName) = daResAreaProp(iIndexProp) ' oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions, True)
						If dInPlanSumAreaProp <> 0 Then
							oNewRow.Item(msAreaPctPropFieldName) = oPcntProp.OutputItemFloat(iIndexProp)
						End If
						oNewRow.Item(sLanduseIDPropFieldName) = oLanduse.ID
						iIndexProp += 1
					End If
					oJointLanduseTable.Rows.Add(oNewRow)
				Loop

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - GetSumLanduseData_2")
			End Try
			'''''''''''''''''''


			Dim sSort As String = String.Empty 'TplnParcel.msLanduseOrderFieldName


			oDataView = New System.Data.DataView(oJointLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(1)
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaAppr
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 0.0
				oaTotals(1) = dInPlanSumAreaProp
			End If


		End Sub
		Public Shared Sub GetLanduseDataNewTest(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iOptions As enDataOptions, ByVal iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef oaTotals() As System.Object)
			Dim oJointLanduseTable As System.Data.DataTable
			Dim sLanduseNameApprFieldName As String = TplnParcel.LanduseNameFieldName & "Appr"
			Dim sLanduseNamePropFieldName As String = TplnParcel.LanduseNameFieldName & "Prop"
			Dim sLanduseIDApprFieldName As String = TplnParcel.LanduseIDFieldName & "Appr"
			Dim sLanduseIDPropFieldName As String = TplnParcel.LanduseIDFieldName & "Prop"
			Dim iOverlayIndex As enOverlayIndex
			oJointLanduseTable = New DataTable("JointLanduse")
			With oJointLanduseTable.Columns
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					.Add(sLanduseNameApprFieldName, GetType(System.String))
					.Add(msAreaApprFieldName, GetType(System.Double))
					.Add(msAreaPctApprFieldName, GetType(System.Double))
					.Add(sLanduseIDApprFieldName, GetType(System.Int32))
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					.Add(sLanduseNamePropFieldName, GetType(System.String))
					.Add(msAreaPropFieldName, GetType(System.Double))
					.Add(msAreaPctPropFieldName, GetType(System.Double))
					.Add(sLanduseIDPropFieldName, GetType(System.Int32))
				End If
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))
			End With

			If mdicSumLanduses Is Nothing OrElse mdicSumLanduses.Count = 0 Then
				MessageBox.Show("Landuse Data was not found", "01_489")
				Return
			End If

			Dim oNewRow As System.Data.DataRow
			Dim oLanduse As TplnLanduse

			Dim daResAreaAppr(mdicSumLanduses.Count - 1) As Double
			Dim daResAreaProp(mdicSumLanduses.Count - 1) As Double


			Dim daPcntAppr(mdicSumLanduses.Count - 1) As Double
			Dim daPcntProp(mdicSumLanduses.Count - 1) As Double

			Dim oaLandusesAppr(mdicSumLanduses.Count - 1) As TplnLanduse
			Dim oaLandusesProp(mdicSumLanduses.Count - 1) As TplnLanduse

			Dim iIndexAppr As Integer = 0
			Dim iIndexProp As Integer = 0
			Dim iIndexApprUB As Integer
			Dim iIndexPropUB As Integer

			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double
			If mdInPlanSumCalcAreaAppr = 0.0 Then


			End If
			If iOptions = enDataOptions.CalcMergeArea Then
				dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iOptions = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = Math.Round(dInPlanSumAreaAppr * 0.001, 3, MidpointRounding.AwayFromZero)
				dInPlanSumAreaProp = Math.Round(dInPlanSumAreaProp * 0.001, 3, MidpointRounding.AwayFromZero)
			End If
			dInPlanSumAreaAppr = 0.0
			dInPlanSumAreaProp = 0.0

			Try
				For Each oLanduse In mdicSumLanduses.Values
					'   oLanduse.TestToExcel()
					If (iRegion = 0 AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Approved)) OrElse (iRegion <> 0 AndAlso oLanduse.ContainRegion(DMAcadExt.enTopoPurpose.Approved, iRegion)) Then
						oaLandusesAppr(iIndexAppr) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved)
						'		MessageBox.Show(CStr(mdicSumLanduses.Count) & ":" & CStr(oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions)) & vbCrLf & iOverlayIndex.ToString, "05_778")
						daResAreaAppr(iIndexAppr) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, iRegion <> 0)
						'	TplnProject.WriteMessageBox(iOverlayIndex.ToString() & vbCrLf & CStr(daResAreaAppr(iIndexAppr)), "01_555")
						dInPlanSumAreaAppr += daResAreaAppr(iIndexAppr)
						iIndexAppr += 1
					End If
					If (iRegion = 0 AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Proposed)) Then
						oaLandusesProp(iIndexProp) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed)
						daResAreaProp(iIndexProp) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, iRegion <> 0)
						'   TplnProject.WriteMessageBox(iOverlayIndex.ToString() & ":" & CStr(daResAreaProp(iIndexProp)), "01_556")
						dInPlanSumAreaProp += daResAreaProp(iIndexProp)
						iIndexProp += 1
					End If
				Next

				iIndexApprUB = iIndexAppr - 1
				iIndexPropUB = iIndexProp - 1

				'          
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & mdicSumLanduses.Count.ToString() & vbCrLf & iIndexAppr.ToString(), "TplnLot - GetSumLanduseData_1d")
			End Try
			'   System.Windows.Forms.MessageBox.Show(CStr(iIndexApprUB) & ":" & iIndexPropUB & vbCrLf & daResAreaProp.GetUpperBound(0), "07_340")
			ReDim Preserve daResAreaAppr(iIndexApprUB)
			ReDim Preserve daResAreaProp(iIndexPropUB)


			iIndexAppr = 0
			iIndexProp = 0

			''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0 Then
				'	iaPcntAppr = Balance(daPcntAppr)
				oPcntAppr = New BalanceArea(daResAreaAppr, 100.0, 100.0, True, "TplnLot.GetLanduseData_1")
			End If
			If dInPlanSumAreaProp <> 0 Then
				'	iaPcntProp = Balance(daPcntProp)
				oPcntProp = New BalanceArea(daResAreaProp, 100.0, 100.0, True, "TplnLot.GetLanduseData_2")
			End If


			Try

				iIndexAppr = 0
				iIndexProp = 0
				Do
					If iIndexAppr <= iIndexApprUB OrElse iIndexProp <= iIndexPropUB Then
						oNewRow = oJointLanduseTable.NewRow()
					Else
						Exit Do
					End If
					If iIndexAppr <= iIndexApprUB Then
						oLanduse = oaLandusesAppr(iIndexAppr)
						oNewRow.Item(sLanduseNameApprFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Approved)
						oNewRow.Item(msAreaApprFieldName) = daResAreaAppr(iIndexAppr) 'oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions, True)

						If dInPlanSumAreaAppr <> 0 Then
							oNewRow.Item(msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndexAppr)
						End If
						oNewRow.Item(sLanduseIDApprFieldName) = oLanduse.ID
						oNewRow.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.Order
						DMAcadExt.AcadDocument.WriteDebugMessage("Order:  " & CStr(oLanduse.Order) & " || " & CStr(oLanduse.ID))
						iIndexAppr += 1
					End If

					If iIndexProp <= iIndexPropUB Then
						oLanduse = oaLandusesProp(iIndexProp)
						oNewRow.Item(sLanduseNamePropFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Proposed)
						oNewRow.Item(msAreaPropFieldName) = daResAreaProp(iIndexProp) ' oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions, True)
						If dInPlanSumAreaProp <> 0 Then
							oNewRow.Item(msAreaPctPropFieldName) = oPcntProp.OutputItemFloat(iIndexProp)
						End If
						oNewRow.Item(sLanduseIDPropFieldName) = oLanduse.ID
						oNewRow.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.Order

						iIndexProp += 1
					End If
					oJointLanduseTable.Rows.Add(oNewRow)
				Loop

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - GetSumLanduseData_2")
			End Try
			'''''''''''''''''''
			'     DMCommon.ExcelLog.SetDataTable(oJointLanduseTable, 0)

			Dim sSort As String = TplnParcel.LanduseOrderFieldName 'String.Empty '


			oDataView = New System.Data.DataView(oJointLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(1)
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaAppr
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaProp
			End If
			'

		End Sub


		''' <summary>
		''' Without Intersection
		''' </summary>
		''' <param name="iTopoPurpose"></param>
		''' <param name="iOverlayMethod"></param>
		''' iOverlayMethod=-1
		''' <param name="iOptions"></param>
		''' <param name="iRegion"></param>
		''' <param name="oDataView"></param>
		''' <param name="oaTotals"></param>
		Public Shared Sub GetLanduseDataNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iOptions As enDataOptions, ByVal iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef oaTotals() As System.Object)
			Dim oJointLanduseTable As System.Data.DataTable
			Dim sLanduseNameApprFieldName As String = TplnParcel.LanduseNameFieldName & "Appr"
			Dim sLanduseNamePropFieldName As String = TplnParcel.LanduseNameFieldName & "Prop"
			Dim sLanduseIDApprFieldName As String = TplnParcel.LanduseIDFieldName & "Appr"
			Dim sLanduseIDPropFieldName As String = TplnParcel.LanduseIDFieldName & "Prop"
			Dim iOverlayIndex As enOverlayIndex
			oJointLanduseTable = New DataTable("JointLanduse")
			With oJointLanduseTable.Columns
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					.Add(sLanduseNameApprFieldName, GetType(System.String))
					.Add(msAreaApprFieldName, GetType(System.Double))
					.Add(msAreaPctApprFieldName, GetType(System.Double))
					.Add(sLanduseIDApprFieldName, GetType(System.Int32))
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					.Add(sLanduseNamePropFieldName, GetType(System.String))
					.Add(msAreaPropFieldName, GetType(System.Double))
					.Add(msAreaPctPropFieldName, GetType(System.Double))
					.Add(sLanduseIDPropFieldName, GetType(System.Int32))
				End If
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))
			End With

			If mdicSumLanduses Is Nothing OrElse mdicSumLanduses.Count = 0 Then
				MessageBox.Show("Landuse Data was not found", "01_489")
				Return
			End If

			Dim oNewRow As System.Data.DataRow
			Dim oLanduse As TplnLanduse

			Dim daResAreaAppr(mdicSumLanduses.Count - 1) As Double
			Dim daResAreaProp(mdicSumLanduses.Count - 1) As Double


			Dim daPcntAppr(mdicSumLanduses.Count - 1) As Double
			Dim daPcntProp(mdicSumLanduses.Count - 1) As Double

			Dim oaLandusesAppr(mdicSumLanduses.Count - 1) As TplnLanduse
			Dim oaLandusesProp(mdicSumLanduses.Count - 1) As TplnLanduse

			Dim iIndexAppr As Integer = 0
			Dim iIndexProp As Integer = 0
			Dim iIndexApprUB As Integer
			Dim iIndexPropUB As Integer

			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double
			If mdInPlanSumCalcAreaAppr = 0.0 Then


			End If
			If iOptions = enDataOptions.CalcMergeArea Then
				dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iOptions = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = Math.Round(dInPlanSumAreaAppr * 0.001, 3, MidpointRounding.AwayFromZero)
				dInPlanSumAreaProp = Math.Round(dInPlanSumAreaProp * 0.001, 3, MidpointRounding.AwayFromZero)
			End If
			dInPlanSumAreaAppr = 0.0
			dInPlanSumAreaProp = 0.0

			Try
				DMCommon.Debug.ExcelLog.SetNextValue(0, "iRegion", iRegion, mdicSumLanduses.Count)
				For Each oLanduse In mdicSumLanduses.Values

					oLanduse.TestToExcel()
					DMCommon.Debug.ExcelLog.SetNextValue(1, "*%5", iRegion, iTopoPurpose, oLanduse.HasLots(DMAcadExt.enTopoPurpose.Approved))

					If (iRegion = TplnRegion.RegionAllIn AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Approved)) OrElse (iRegion <> 0 AndAlso oLanduse.ContainRegion(DMAcadExt.enTopoPurpose.Approved, iRegion)) Then

						oaLandusesAppr(iIndexAppr) = oLanduse
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Undefined Then
							iOverlayIndex = 0
						Else
							iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved)
						End If

						'		MessageBox.Show(CStr(mdicSumLanduses.Count) & ":" & CStr(oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions)) & vbCrLf & iOverlayIndex.ToString, "05_778")
						DMCommon.Debug.ExcelLog.SetNextValue(1, "*%6", iTopoPurpose, iOverlayMethod, iOverlayIndex, oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, iRegion <> TplnRegion.RegionAllIn))

						daResAreaAppr(iIndexAppr) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, iRegion <> TplnRegion.RegionAllIn)
						DMCommon.Debug.ExcelLog.SetNextValue(5, "ResAreaAppr", iIndexAppr, iOverlayIndex, iOptions, daResAreaAppr(iIndexAppr))

						'	TplnProject.WriteMessageBox(iOverlayIndex.ToString() & vbCrLf & CStr(daResAreaAppr(iIndexAppr)), "01_555")
						dInPlanSumAreaAppr += daResAreaAppr(iIndexAppr)
						iIndexAppr += 1
					End If
					If (iRegion = TplnRegion.RegionAllIn AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso oLanduse.HasLots(DMAcadExt.enTopoPurpose.Proposed)) Then
						oaLandusesProp(iIndexProp) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed)
						daResAreaProp(iIndexProp) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, iRegion <> TplnRegion.RegionAllIn)
						'   TplnProject.WriteMessageBox(iOverlayIndex.ToString() & ":" & CStr(daResAreaProp(iIndexProp)), "01_556")
						dInPlanSumAreaProp += daResAreaProp(iIndexProp)
						iIndexProp += 1
					End If
				Next

				iIndexApprUB = iIndexAppr - 1
				iIndexPropUB = iIndexProp - 1

				'          DMCommon.ExcelLog.SetNextArray(daResAreaAppr, 0)
				'          DMCommon.ExcelLog.SetNextArray(daResAreaProp, 1)

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicSumLanduses.Count.ToString(), "TplnLot - GetSumLanduseData_1e")
			End Try
			'   System.Windows.Forms.MessageBox.Show(CStr(iIndexApprUB) & ":" & iIndexPropUB & vbCrLf & daResAreaProp.GetUpperBound(0), "07_340")
			ReDim Preserve daResAreaAppr(iIndexApprUB)
			ReDim Preserve daResAreaProp(iIndexPropUB)


			iIndexAppr = 0
			iIndexProp = 0

			''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0 Then
				'	iaPcntAppr = Balance(daPcntAppr)
				oPcntAppr = New BalanceArea(daResAreaAppr, 100.0, 100.0, True, "TplnLot.GetLanduseData_1")
			End If
			If dInPlanSumAreaProp <> 0 Then
				'	iaPcntProp = Balance(daPcntProp)
				oPcntProp = New BalanceArea(daResAreaProp, 100.0, 100.0, True, "TplnLot.GetLanduseData_2")
			End If

			''''''''''''''''''TEST
			''''''
			Dim oTestDic As IDictionary(Of Integer, DMAcadExt.ColorScheme) = TplnLanduse.GetColorSchemeDic(DMAcadExt.enTopoPurpose.Approved)

			For Each tColorScheme As DMAcadExt.ColorScheme In oTestDic.Values
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!TestDic", tColorScheme.ID, tColorScheme.Name, tColorScheme.TableName, tColorScheme.HasName)
			Next

			Try

				iIndexAppr = 0
				iIndexProp = 0
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!IndexApprUB", iIndexApprUB, iIndexPropUB)
				Do
					If iIndexAppr <= iIndexApprUB OrElse iIndexProp <= iIndexPropUB Then
						oNewRow = oJointLanduseTable.NewRow()
					Else
						Exit Do
					End If
					If iIndexAppr <= iIndexApprUB Then
						oLanduse = oaLandusesAppr(iIndexAppr)
						If oLanduse IsNot Nothing Then

							''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
							oNewRow.Item(sLanduseNameApprFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Approved)
							oNewRow.Item(msAreaApprFieldName) = daResAreaAppr(iIndexAppr) 'oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions, True)

							If dInPlanSumAreaAppr <> 0 Then
								oNewRow.Item(msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndexAppr)
							End If
							oNewRow.Item(sLanduseIDApprFieldName) = oLanduse.ID
							oNewRow.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.Order
							DMAcadExt.AcadDocument.WriteDebugMessage("Order:  " & CStr(oLanduse.Order) & " || " & CStr(oLanduse.ID))
							iIndexAppr += 1
						End If
					End If

					If iIndexProp <= iIndexPropUB Then
						oLanduse = oaLandusesProp(iIndexProp)
						oNewRow.Item(sLanduseNamePropFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Proposed)
						oNewRow.Item(msAreaPropFieldName) = daResAreaProp(iIndexProp) ' oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions, True)
						If dInPlanSumAreaProp <> 0 Then
							oNewRow.Item(msAreaPctPropFieldName) = oPcntProp.OutputItemFloat(iIndexProp)
						End If
						oNewRow.Item(sLanduseIDPropFieldName) = oLanduse.ID
						oNewRow.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.Order

						iIndexProp += 1
					End If
					oJointLanduseTable.Rows.Add(oNewRow)
				Loop

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - GetSumLanduseData_2")
			End Try
			'''''''''''''''''''
			DMCommon.Debug.ExcelLog.SetDataTable(0, "oJointLanduseTable", oJointLanduseTable)

			Dim sSort As String = TplnParcel.LanduseOrderFieldName 'String.Empty '


			oDataView = New System.Data.DataView(oJointLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(1)
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaAppr
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaProp
			End If
			'

		End Sub
		Public Shared Sub GetJointLanduseData(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iOptions As enDataOptions, ByRef oDataView As System.Data.DataView, ByRef oaTotals() As System.Object)

			Dim oJointLanduseTable As System.Data.DataTable
			Dim sLanduseNameApprFieldName As String = TplnParcel.LanduseNameFieldName & "Appr"
			Dim sLanduseNamePropFieldName As String = TplnParcel.LanduseNameFieldName & "Prop"
			Dim sLanduseIDApprFieldName As String = TplnParcel.LanduseIDFieldName & "Appr"
			Dim sLanduseIDPropFieldName As String = TplnParcel.LanduseIDFieldName & "Prop"
			Dim iOverlayIndex As enOverlayIndex
			oJointLanduseTable = New DataTable("JointLanduse")
			With oJointLanduseTable.Columns
				.Add(sLanduseNameApprFieldName, GetType(System.String))
				.Add(msAreaApprFieldName, GetType(System.Double))
				.Add(msAreaPctApprFieldName, GetType(System.Double))

				.Add(sLanduseNamePropFieldName, GetType(System.String))
				.Add(msAreaPropFieldName, GetType(System.Double))
				.Add(msAreaPctPropFieldName, GetType(System.Double))

				.Add(sLanduseIDApprFieldName, GetType(System.Int32))
				.Add(sLanduseIDPropFieldName, GetType(System.Int32))

				'	.Add(TplnParcel.msLanduseOrderFieldName, GetType(System.Int32))
			End With

			If mdicSumLanduses Is Nothing OrElse mdicSumLanduses.Count = 0 Then Return
			Dim oNewRow As System.Data.DataRow
			Dim oLanduse As TplnLanduse

			Dim daResAreaAppr(mdicSumLanduses.Count - 1) As Double
			Dim daResAreaProp(mdicSumLanduses.Count - 1) As Double


			Dim daPcntAppr(mdicSumLanduses.Count - 1) As Double
			Dim daPcntProp(mdicSumLanduses.Count - 1) As Double
			'		Dim oaLanduses(mdicSumLanduses.Count - 1) As TplnLanduse
			Dim oaLandusesAppr(mdicSumLanduses.Count - 1) As TplnLanduse
			Dim oaLandusesProp(mdicSumLanduses.Count - 1) As TplnLanduse



			'           Dim iaPcntAppr(mdicSumLanduses.Count - 1) As Integer
			'           Dim iaPcntProp(mdicSumLanduses.Count - 1) As Integer

			'Dim iIndex As Integer = 0
			Dim iIndexAppr As Integer = 0
			Dim iIndexProp As Integer = 0
			Dim iIndexApprUB As Integer = 0
			Dim iIndexPropUB As Integer = 0

			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double

			If iOptions = enDataOptions.CalcMergeArea Then
				dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iOptions = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = Math.Round(dInPlanSumAreaAppr * 0.001, 3, MidpointRounding.AwayFromZero)
				dInPlanSumAreaProp = Math.Round(dInPlanSumAreaProp * 0.001, 3, MidpointRounding.AwayFromZero)
			End If
			dInPlanSumAreaAppr = 0.0
			dInPlanSumAreaProp = 0.0

			Try
				For Each oLanduse In mdicSumLanduses.Values
					If oLanduse.HasLots(DMAcadExt.enTopoPurpose.Approved) Then
						oaLandusesAppr(iIndexAppr) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved)
						'		MessageBox.Show(CStr(mdicSumLanduses.Count) & ":" & CStr(oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions)) & vbCrLf & iOverlayIndex.ToString, "05_778")
						daResAreaAppr(iIndexAppr) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, False)
						'	TplnProject.WriteMessageBox(iOverlayIndex.ToString() & vbCrLf & CStr(daResAreaAppr(iIndexAppr)), "01_555")
						dInPlanSumAreaAppr += daResAreaAppr(iIndexAppr)
						iIndexAppr += 1
					End If
					If oLanduse.HasLots(DMAcadExt.enTopoPurpose.Proposed) Then
						oaLandusesProp(iIndexProp) = oLanduse
						iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed)
						daResAreaProp(iIndexProp) = oLanduse.OptionAreaByIndex(iOverlayIndex, iOptions, False)
						'	TplnProject.WriteMessageBox(CStr(daResAreaProp(iIndexAppr)), "01_556")
						dInPlanSumAreaProp += daResAreaProp(iIndexProp)
						iIndexProp += 1
					End If
				Next
				iIndexApprUB = iIndexAppr - 1
				iIndexPropUB = iIndexProp - 1

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicSumLanduses.Count.ToString(), "TplnLot - GetSumLanduseData_1a")
			End Try
			ReDim Preserve daResAreaAppr(iIndexApprUB)
			ReDim Preserve daResAreaProp(iIndexPropUB)


			iIndexAppr = 0
			iIndexProp = 0

			''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0 Then
				'	iaPcntAppr = Balance(daPcntAppr)
				oPcntAppr = New BalanceArea(daResAreaAppr, 100.0, 100.0, True, "TplnLot.GetJointLanduseData_1")
			End If
			If dInPlanSumAreaProp <> 0 Then
				'	iaPcntProp = Balance(daPcntProp)
				oPcntProp = New BalanceArea(daResAreaProp, 100.0, 100.0, True, "TplnLot.GetJointLanduseData_2")
			End If


			Try

				iIndexAppr = 0
				iIndexProp = 0
				Do
					If iIndexAppr <= iIndexApprUB OrElse iIndexProp <= iIndexPropUB Then
						oNewRow = oJointLanduseTable.NewRow()
					Else
						Exit Do
					End If
					If iIndexAppr <= iIndexApprUB Then
						oLanduse = oaLandusesAppr(iIndexAppr)
						oNewRow.Item(sLanduseNameApprFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Approved)
						oNewRow.Item(msAreaApprFieldName) = daResAreaAppr(iIndexAppr) 'oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions, True)

						If dInPlanSumAreaAppr <> 0 Then
							oNewRow.Item(msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndexAppr)
						End If
						oNewRow.Item(sLanduseIDApprFieldName) = oLanduse.ID
						iIndexAppr += 1
					End If

					If iIndexProp <= iIndexPropUB Then
						oLanduse = oaLandusesProp(iIndexProp)
						oNewRow.Item(sLanduseNamePropFieldName) = oLanduse.GetNameNew(DMAcadExt.enTopoPurpose.Proposed)
						oNewRow.Item(msAreaPropFieldName) = daResAreaProp(iIndexProp) ' oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions, True)
						If dInPlanSumAreaProp <> 0 Then
							oNewRow.Item(msAreaPctPropFieldName) = oPcntProp.OutputItemFloat(iIndexProp)
						End If
						oNewRow.Item(sLanduseIDPropFieldName) = oLanduse.ID
						iIndexProp += 1
					End If
					oJointLanduseTable.Rows.Add(oNewRow)
				Loop

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - GetSumLanduseData_2a")
			End Try
			'''''''''''''''''''


			Dim sSort As String = String.Empty 'TplnParcel.msLanduseOrderFieldName


			oDataView = New System.Data.DataView(oJointLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(3)
			If dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 100.0
			Else
				' oaTotals(0) = 0.0
			End If
			oaTotals(1) = dInPlanSumAreaProp
			If dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(2) = 100.0
			Else
				' oaTotals(2) = 0.0
			End If
			oaTotals(3) = dInPlanSumAreaAppr
			'''''''		DMCommon.Functions.DispArray(oaTotals, "oaTotals", True)
		End Sub
		Public Shared Function GetSumLanduseCount() As Integer
			Dim iResCount As Integer
			If mdicSumLanduses Is Nothing Then
				iResCount = -1
			Else
				Try

					iResCount = mdicSumLanduses.Count
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "TplnLot - GetSumLanduseCount")
					iResCount = -2
				End Try

			End If
			Return iResCount
		End Function
		Public Shared Function GetSumLanduseTopoPurpose() As DMAcadExt.enTopoPurpose
			If mdicSumLanduses Is Nothing Then
				Return DMAcadExt.enTopoPurpose.Undefined
			Else
				Return mdicSumLanduses.TopoPurpose
			End If
		End Function

		Public Shared Sub GetSumLanduseData(ByRef oDataView As System.Data.DataView, ByVal iOptions As enDataOptions, ByRef oaTotals() As System.Object)
			Dim oSumLanduseTable As System.Data.DataTable

			oSumLanduseTable = New DataTable("SumLanduse")
			With oSumLanduseTable.Columns
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(msAreaApprFieldName, GetType(System.Double))
				.Add(msAreaPctApprFieldName, GetType(System.Double))
				.Add(msAreaPropFieldName, GetType(System.Double))
				.Add(msAreaPctPropFieldName, GetType(System.Double))
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))
			End With

			If mdicSumLanduses Is Nothing OrElse mdicSumLanduses.Count = 0 Then Return
			Dim oNewRow As System.Data.DataRow
			Dim oLanduse As TplnLanduse
			Dim daPcntAppr(mdicSumLanduses.Count - 1) As Double
			Dim daPcntProp(mdicSumLanduses.Count - 1) As Double
			Dim oaLanduses(mdicSumLanduses.Count - 1) As TplnLanduse
			'           Dim iaPcntAppr(mdicSumLanduses.Count - 1) As Integer
			'           Dim iaPcntProp(mdicSumLanduses.Count - 1) As Integer
			'		Dim iaPcntAppr() As Long
			'		Dim iaPcntProp() As Long
			Dim iIndex As Integer = 0
			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double

			If iOptions = enDataOptions.CalcMergeArea Then
				dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iOptions = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = mdInPlanSumAcadAreaAppr
				dInPlanSumAreaProp = mdInPlanSumAcadAreaProp
			End If

			Try
				For Each oLanduse In mdicSumLanduses.Values
					oaLanduses(iIndex) = oLanduse
					If dInPlanSumAreaAppr <> 0 Then
						daPcntAppr(iIndex) = 10000 * oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions) / dInPlanSumAreaAppr
					End If
					If dInPlanSumAreaProp <> 0 Then
						daPcntProp(iIndex) = 10000 * oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions) / dInPlanSumAreaProp
					End If
					iIndex += 1
				Next

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicSumLanduses.Count.ToString(), "TplnLot - GetSumLanduseData_1b")
			End Try
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0.0 Then
				oPcntAppr = New BalanceArea(daPcntAppr, 1000.0, 100.0, True, "TplnLot.GetSumLanduseData_1")

				'	iaPcntAppr = Balance(daPcntAppr)
			End If

			If dInPlanSumAreaProp <> 0.0 Then
				oPcntProp = New BalanceArea(daPcntProp, 1000.0, 100.0, True, "TplnLot.GetSumLanduseData_2")
			End If


			Try
				For iIndex = 0 To mdicSumLanduses.Count - 1
					oLanduse = oaLanduses(iIndex)
					oNewRow = oSumLanduseTable.NewRow()
					oNewRow.Item(TplnParcel.LanduseNameFieldName) = oLanduse.Name
					oNewRow.Item(msAreaApprFieldName) = oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Approved, iOptions)
					If dInPlanSumAreaAppr <> 0 Then
						oNewRow.Item(msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndex) 'iaPcntAppr(iIndex) * 0.01
					End If
					oNewRow.Item(msAreaPropFieldName) = oLanduse.OptionArea(DMAcadExt.enTopoPurpose.Proposed, iOptions)
					If dInPlanSumAreaProp <> 0 Then
						oNewRow.Item(msAreaPctPropFieldName) = oPcntProp.OutputItemFloat(iIndex)
					End If

					oNewRow.Item(TplnParcel.LanduseIDFieldName) = oLanduse.ID
					oNewRow.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.Order
					oSumLanduseTable.Rows.Add(oNewRow)
				Next

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - GetSumLanduseData_2")
			End Try
			'''''''''''''''''''


			Dim sSort As String = TplnParcel.LanduseOrderFieldName
			oDataView = New System.Data.DataView(oSumLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(3)
			If dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 100.0
			Else
				' oaTotals(0) = 0.0
			End If
			oaTotals(1) = dInPlanSumAreaProp
			If dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(2) = 100.0
			Else
				' oaTotals(2) = 0.0
			End If
			oaTotals(3) = dInPlanSumAreaAppr
			'	DMCommon.Functions.DispArray(oaTotals, "oaTotals", True)
		End Sub
		Public Shared Function GetLotData(tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As LotData

			'    Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = DMAcadExt.AcadTransaction.GetDBObject(tCentroidAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

			Dim saBlockAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(tCentroidAcObjID, True, False, miaApprovedBlockAttribIndex)


			If saBlockAttribText IsNot Nothing Then
				Return New LotData(saBlockAttribText)
			Else
				Return Nothing
			End If
		End Function
		Private Shared ReadOnly Property zzDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As System.Data.DataTable
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					Return moApprovedDataTable
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return moProposedDataTable
				Else
					Return Nothing
				End If
			End Get
		End Property

		Private Shared Sub zzTestIntArray(ByVal iaVal() As Integer)
			Dim iUB As Integer = -1
			Dim sMsg As String = "---" & vbCrLf
			Try
				iUB = iaVal.GetUpperBound(0)
				For iIndex As Integer = 0 To iUB
					If iIndex <> 0 Then sMsg &= vbCrLf
					sMsg = sMsg & ":" & iaVal(iIndex).ToString() & ":"

				Next
				sMsg = sMsg & vbCrLf & "---"
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "ex: Lot-TestIntArray")
			End Try
			System.Windows.Forms.MessageBox.Show(sMsg, "Lot-TestIntArray")
		End Sub
		Public Shared Sub CalcLanduses(ByVal bByPoligons As Boolean, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)

			If mdicSumLanduses IsNot Nothing Then
				For Each oLanduse As TplnLanduse In mdicSumLanduses.Values
					oLanduse.Calculate(bByPoligons, iOverlayMethod)
				Next
			End If

		End Sub
		Public Shared Sub CalcRegionLanduses(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegion As Integer)

			If mdicSumLanduses IsNot Nothing Then
				If bApproved Then
					For Each oLanduse As TplnLanduse In mdicSumLanduses.Values
						'  oLanduse.Calculate(False, iOverlayMethod, iRegion)

						oLanduse.CalcByOverlayGroups(DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enOverlayMethod.FDO_Overlay, iRegion)

					Next
				End If
				If bProposed Then
					For Each oLanduse As TplnLanduse In mdicSumLanduses.Values
						'  oLanduse.Calculate(False, iOverlayMethod, iRegion)

						oLanduse.CalcByOverlayGroups(DMAcadExt.enTopoPurpose.Proposed, DMAcadExt.enOverlayMethod.FDO_Overlay, iRegion)

					Next
				End If

			End If

		End Sub

		Public Shared Function GetColorScheme(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLanduseID As Integer, ByVal dScale As Double) As DMAcadExt.ColorScheme
			'''''''''''	MessageBox.Show(CStr(iLanduseID), "04_371j")
			Dim oLanduse As TplnLanduse = Nothing
			Dim bRes As Boolean = mdicSumLanduses.TryGetValue(iLanduseID, oLanduse)
			Dim iLanduseOrder As Integer
			If bRes Then
				Return oLanduse.GetColorScheme(iTopoPurpose, dScale, iLanduseOrder)
			Else
				DMAcadExt.AcadDocument.WriteDebugMessage("$$$$17 " & CStr(iLanduseID) & " from " & CStr(mdicSumLanduses.Count))
				Return Nothing
			End If
		End Function
		Public Shared Function GetLanduseNameNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLanduseID As Integer) As String
			Dim oLanduse As TplnLanduse = Nothing
			'	Dim tColorScheme As ColorScheme
			Dim bRes As Boolean
			If mdicSumLanduses IsNot Nothing Then
				bRes = mdicSumLanduses.TryGetValue(iLanduseID, oLanduse)
			Else
				bRes = False '
			End If

			'DMCommon.Debug.MsgBoxLoop("C03_01", 3, "!!!!!!!!!!!!", bRes, iTopoPurpose, iLanduseID, GetSumLanduseCount())
			If bRes AndAlso oLanduse IsNot Nothing Then
				'tColorScheme = oLanduse.GetColorScheme(iTopoPurpose, 1, 1)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!LotLUName", iLanduseID, oLanduse.Name, oLanduse.GetNameNew(iTopoPurpose), "END")
				Return oLanduse.GetNameNew(iTopoPurpose)
				'Return tColorScheme.Name
			Else
				DMCommon.Debug.MsgBoxLoop("C03_11")
				DMCommon.Debug.MsgBoxLoop("C03_12", 3, bRes, iTopoPurpose, iLanduseID, GetSumLanduseCount())
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "---LotLUName", iLanduseID, mdicSumLanduses.Count)
				If iLanduseID <> 0 OrElse GetSumLanduseCount() > 1 Then
					DMAcadExt.AcadDocument.WriteMessage("$$$$19 " & CStr(iLanduseID) & " from " & CStr(GetSumLanduseCount()))
				End If
				DMCommon.Debug.MsgBoxLoop("C03_13")
				Return Nothing
			End If
		End Function
		Public Shared Function GetLanduseNameNew2(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLanduseID As Integer) As String
			Dim oLanduse As TplnLanduse = Nothing

			Dim bRes As Boolean = mdicSumLanduses.TryGetValue(iLanduseID, oLanduse)

			If bRes Then
				Return oLanduse.GetNameNew(iTopoPurpose)


			Else
				If iLanduseID <> 0 OrElse mdicSumLanduses.Count <> 0 Then
					DMAcadExt.AcadDocument.WriteMessage("$$$$21 " & CStr(iLanduseID) & " from " & CStr(mdicSumLanduses.Count) & "; ")
				End If

				Return Nothing
			End If
		End Function

		Public Shared Function GetLanduseName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLanduseID As Integer) As String
			Dim oLanduse As TplnLanduse = Nothing

			Dim bRes As Boolean = mdicSumLanduses.TryGetValue(iLanduseID, oLanduse)

			If bRes Then

				Return oLanduse.Name
			Else
				If iLanduseID <> 0 OrElse mdicSumLanduses.Count <> 0 Then
					DMAcadExt.AcadDocument.WriteMessage("$$$$31 " & CStr(iLanduseID) & " from " & CStr(mdicSumLanduses.Count))
				End If

				Return Nothing
			End If
		End Function
		Public Shared ReadOnly Property Landuses() As TplnLanduses
			Get
				Return mdicSumLanduses
			End Get
		End Property
		Public Shared ReadOnly Property LandusesCount() As Integer
			Get
				If mdicSumLanduses IsNot Nothing Then
					Return mdicSumLanduses.Count
				Else
					Return -1
				End If

			End Get
		End Property

		Public Shared Function GetLanduseName(iLanduseID As Integer) As String
			Dim oLanduse As TplnLanduse = Nothing

			If mdicSumLanduses.TryGetValue(iLanduseID, oLanduse) Then
				Return oLanduse.Name
			Else
				Return Nothing
			End If
		End Function
#End Region
#Region "Constructors & Destructors"
		Public Sub New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon, True)

			'	DMCommon.ExcelLog.SetNextValue(11, iTopoPurpose, iTopoPurpose.ToString())
			MyBase.diTopoPurpose = iTopoPurpose
			MyBase.SetAttributeOrder()




			'  DMCommon.Functions.DispArray(dsaBlockAttribText, "dsaBlockAttribText", True)


			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "dsaBlockAttribText", dsaBlockAttribText)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "dsaBlockAttribText", dsaBlockAttribText)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "miaApprovedBlockAttribIndex", miaApprovedBlockAttribIndex)
			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!dsaBlockAttribText", dsaBlockAttribText)
			'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "dsaBloc!miaApprovedBlockAttribIndex", miaApprovedBlockAttribIndex)
			'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!dsaBlockAttrT", dsaBlockAttribText)
			mtLotData = New LotData(dsaBlockAttribText)



			MyBase.dsName = mtLotData.Name
			MyBase.diOrder = mtLotData.Order
			If mtLotData.RegionNo > 0 AndAlso mtLotData.RegionNo <> TplnRegion.RegionIn Then
				mbHasRegions = True
			End If

		End Sub


		Public Sub New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, iFeatureID As Integer, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
			MyBase.New(oPolygon, iFeatureID, tCentroidAcObjID)
			MyBase.diTopoPurpose = iTopoPurpose
			MyBase.SetAttributeOrder()

			mtLotData = New LotData(dsaBlockAttribText)

			MyBase.dsName = mtLotData.Name

			MyBase.diOrder = mtLotData.Order
		End Sub

#End Region
#Region "Shared members"

		Public Shared Function GetLotData(iTopoPurpose As DMAcadExt.enTopoPurpose, iCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As LotData
			Dim saBlockAttribText() As String = Nothing
			Dim bAcadPoint As Boolean
			If iTopoPurpose = enTopoPurpose.Approved Then
				saBlockAttribText = AcadTransaction.GetAttribText(iCentroidAcObjID, True, bAcadPoint, miaApprovedBlockAttribIndex)
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				saBlockAttribText = AcadTransaction.GetAttribText(iCentroidAcObjID, True, bAcadPoint, miaProposedBlockAttribIndex)
			End If

			'		DMCommon.Functions.DispArray(saBlockAttribText, "saBlockAttribText", True)
			'		DMCommon.Functions.DispArray(miaApprovedBlockAttribIndex, "miaApprovedBlockAttribIndex")

			Return New LotData(saBlockAttribText)
		End Function
		Public Shared Sub AddLotToLandusesOldVer(ByVal oLot As TplnLot) ' of Project
			'	MessageBox.Show(CStr(oLot.InPlan), "01_333a")
			If oLot.InPlan Then
				mdicSumLanduses.AddLotOldVer(oLot, True)
			End If
		End Sub
		Public Shared Sub AddLotToLanduses(ByVal oLot As TplnLot) ' of Project
			'	If True OrElse oLot.InPlan Then
			'	mdicSumLanduses.AddLotNew(oLot, False)

			'	End If

			If oLot IsNot Nothing Then

				If oLot.InPlan Then

					If mdicSumLanduses Is Nothing Then
						DMCommon.Debug.MsgBoxLoop("C02_28", 1, GetSumLanduseCount(), GetSumLanduseTopoPurpose())
					Else
						mdicSumLanduses.AddLotNew(oLot, False)
					End If

					'	DMCommon.Debug.MsgBoxLoop("C02_02", 1, GetSumLanduseCount(), GetSumLanduseTopoPurpose())
				End If
			End If


		End Sub

#End Region
#Region "Instance members"
		Public ReadOnly Property InPlan() As Boolean
			Get
				Return mtLotData.InPlan    'mbInPlan
			End Get
		End Property
		Public ReadOnly Property PlanName() As String
			Get
				Return mtLotData.PlanName    'mbInPlan
			End Get
		End Property
		Public ReadOnly Property Order() As Long
			Get
				Return MyBase.diOrder
			End Get
		End Property

		Public Overrides Sub AddDataToMainTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim oDataTable As System.Data.DataTable = zzDataTable(MyBase.diTopoPurpose)
			Dim iMergeOverlayIndex As DMAcadExt.enOverlayIndex
			Dim dValue As Double
			Dim tAreaSet As TplnAreaSet
			Dim bAreaSetExists As Boolean
			Dim bAreaRounded As Boolean = False
			Dim oRegion As TplnRegion = Nothing
			Dim tParcelArea As ParcelArea
			If MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				iMergeOverlayIndex = DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				iMergeOverlayIndex = DMAcadExt.enOverlayIndex.PropMerge
			End If
			If oDataTable IsNot Nothing Then
				Try
					Dim oLanduse As TplnLanduse = Nothing
					Dim oNewRow As System.Data.DataRow
					oNewRow = oDataTable.NewRow()
					oNewRow.Item(NameFieldName) = MyBase.Name '& "|" & Me.PlanName

					If Me.InPlan AndAlso Me.LanduseID <> 0 Then

						If mdicSumLanduses.TryGetValue(Me.LanduseID, oLanduse) AndAlso oLanduse IsNot Nothing Then
							oNewRow.Item(TplnParcel.LanduseNameFieldName) = oLanduse.GetNameNew(MyBase.diTopoPurpose)
							oNewRow.Item(msLanduseOrderFieldName) = oLanduse.GetOrder(MyBase.diTopoPurpose)
						End If
					End If
					oNewRow.Item(TopoReader.msAreaFldName) = Math.Round(MyBase.ddAcadArea, 4)
					If bMerge Then
						dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.Merge)
						oNewRow.Item(TopoReader.msSumPgonAreaFldName) = dValue
					End If
					If bFDO_Overlay Then
						dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay)
						oNewRow.Item(TopoReader.msSumPgonAreaFDO_OverlayFldName) = Math.Round(dValue, 4)

					End If
					'		050 2066 983 Izhar
					Dim iOverlayIndex As DMAcadExt.enOverlayIndex
					If bMerge AndAlso MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
						iOverlayIndex = enOverlayIndex.ApprMerge
						bAreaSetExists = True
					ElseIf bMerge AndAlso MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
						iOverlayIndex = enOverlayIndex.PropMerge
						bAreaSetExists = True
					Else
						bAreaSetExists = False
					End If
					If bAreaSetExists Then

						oNewRow.Item(msCalcAreaFieldName) = tAreaSet.CalcArea
						oNewRow.Item(msCalcArea2FieldName) = tAreaSet.CalcArea2
						oNewRow.Item(msRoundedAreaFieldName) = tAreaSet.RoundedArea
						oNewRow.Item(msCalcGroupArea2FieldName) = tAreaSet.CalcGroupArea2



						bAreaRounded = True
					Else
						oNewRow.Item(msCalcAreaFieldName) = 0.0
						oNewRow.Item(msCalcArea2FieldName) = 0.0
						oNewRow.Item(msRoundedAreaFieldName) = 0.0 ' Math.Round(MyBase.ddAcadArea, 0, MidpointRounding.AwayFromZero)
					End If

					If bFDO_Overlay AndAlso MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
						iOverlayIndex = enOverlayIndex.ApprFDO_Overlay
						bAreaSetExists = True
					ElseIf bFDO_Overlay AndAlso MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
						iOverlayIndex = enOverlayIndex.PropFDO_Overlay
						bAreaSetExists = True
					Else
						bAreaSetExists = False
					End If

					If bAreaSetExists Then
						tAreaSet = dtaInPlanAreaSet(iOverlayIndex)
						If Me.TopoID = ID_Debug Then
							TplnProject.WriteMessageBox("065Lot:" & tAreaSet.ToString(), "")

							TplnProject.WriteMessageBox("066LotIn:" & dtaInPlanAreaSet(iOverlayIndex).ToString(), "")
						End If
						tParcelArea = New ParcelArea(MyBase.ddAcadArea)
						tParcelArea.CalculateArea(tAreaSet.CalcArea, 3)
						tAreaSet = dtaInPlanAreaSet(iOverlayIndex)

						oNewRow.Item(msCalcAreaFDO_OverlayFieldName) = tAreaSet.CalcArea
						oNewRow.Item(msCalcArea2FDO_OverlayFieldName) = tAreaSet.CalcArea2
						oNewRow.Item(msRoundedAreaFDO_OverlayFieldName) = tAreaSet.RoundedArea
						oNewRow.Item(msCalcGroupArea2FieldName) = tAreaSet.CalcGroupArea2
						oNewRow.Item(TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM
						oNewRow.Item(TplnParcel.msToleranceFieldName) = tParcelArea.Tolerance
						oNewRow.Item(TplnParcel.msDeviationFieldName) = tParcelArea.Deviation


						bAreaRounded = True
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!CalcToler", Name, tParcelArea.AcadArea, tParcelArea.LegalArea, tParcelArea.DeltaAreaM)
					Else
						oNewRow.Item(msCalcAreaFDO_OverlayFieldName) = 0.0
						oNewRow.Item(msCalcArea2FDO_OverlayFieldName) = 0.0
						If bAreaRounded Then
							oNewRow.Item(msRoundedAreaFDO_OverlayFieldName) = 0.0
						Else
							oNewRow.Item(msRoundedAreaFDO_OverlayFieldName) = Math.Round(MyBase.ddAcadArea, 0, MidpointRounding.AwayFromZero)
						End If


						'''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "Test_lots_0102")
					End If
					If Not bAreaRounded Then
						oNewRow.Item(msRoundedAreaFieldName) = Math.Round(MyBase.ddAcadArea, 0, MidpointRounding.AwayFromZero)
					End If
					oNewRow.Item(msForcedAreaFieldName) = Me.ForcedArea

					oNewRow.Item(msRegionFieldName) = mtLotData.RegionNo

					If TopoManager.TPlanGraph.TplnProject.Regions.TryGetValue(mtLotData.RegionNo, oRegion) Then
						oNewRow.Item(msRegionNameFieldName) = oRegion.RegionName
					End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!CalcRegName", TopoManager.TPlanGraph.TplnProject.Regions.Count, mtLotData.RegionNo, oRegion Is Nothing)
					'	oNewRow.Item(TopoReader.msSumPgonAreaUnFldName) = MyBase.SumPolygonArea(DMAcadExt.enOverlayMethod.Union)
					oNewRow.Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
					oNewRow.Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
					oNewRow.Item(TplnParcel.LanduseIDFieldName) = Me.LanduseID
					oNewRow.Item(msInPlanFieldName) = Me.InPlan
					oNewRow.Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
					oNewRow.Item(TopoReader.msTopoIDFldName) = MyBase.TopoID
					oNewRow.Item(TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID    ' MyBase.diCentroidAcObjID.OldIdPtr.ToInt64()

					'	If mbNameIsNum Then
					oNewRow.Item(LotOrderFieldName) = MyBase.diOrder
					'	Else
					'	oNewRow.Item(msLotOrderFieldName) = MyBase.Name
					'	End If

					oDataTable.Rows.Add(oNewRow)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - AddDataToMainTable_1")
				End Try
			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "TplnLot - AddDataToMainTable_2")
			End If

			If Me.InPlan And False Then
				''''''mdicSumLanduses.AddLot(MyBase.diTopoPurpose, Me.LanduseID, MyBase.CalcArea(enOverlayMethod.Merge), MyBase.CalcArea(enOverlayMethod.Union), MyBase.ddAcadArea)

				If MyBase.diTopoPurpose = enTopoPurpose.Approved Then
					'	mdInPlanSumCalcArea.Item(enOverlayIndex.ApprMerge) += MyBase.CalcArea(DMAcadExt.enOverlayMethod.Merge)
					'test 25/07/10
					mdInPlanSumCalcArea.Item(enOverlayIndex.ApprMerge) += MyBase.CalcArea(False, DMAcadExt.enOverlayMethod.Merge)
					mdInPlanSumCalcArea.Item(enOverlayIndex.ApprUnion) += MyBase.CalcArea(True, DMAcadExt.enOverlayMethod.Union)

					mdInPlanSumAcadAreaAppr += MyBase.ddAcadArea
					mdInPlanSumCalcAreaAppr += MyBase.CalcArea(False, DMAcadExt.enOverlayMethod.Merge) '  MergeOnly

				ElseIf MyBase.diTopoPurpose = enTopoPurpose.Proposed Then
					mdInPlanSumCalcArea.Item(enOverlayIndex.PropMerge) += MyBase.CalcArea(False, DMAcadExt.enOverlayMethod.Merge)
					mdInPlanSumCalcArea.Item(enOverlayIndex.PropUnion) += MyBase.CalcArea(True, DMAcadExt.enOverlayMethod.Union)

					mdInPlanSumCalcAreaProp += MyBase.CalcArea(False, DMAcadExt.enOverlayMethod.Merge) '  MergeOnly
					mdInPlanSumAcadAreaProp += MyBase.ddAcadArea
				End If
			End If
		End Sub
		Public Shadows ReadOnly Property CalcArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod) As Double
			Get
				Return MyBase.CalcArea(False, iOverlayMethod, DMAcadExt.enTopoPurpose.Undefined) 'diTopoPurpose
			End Get
		End Property

		Public ReadOnly Property Landuse() As TplnLanduse
			Get
				Dim oLanduse As TplnLanduse = Nothing
				mdicSumLanduses.TryGetValue(Me.LanduseID, oLanduse)
				Return oLanduse
			End Get
		End Property
		Public ReadOnly Property RegionNo() As Integer
			Get
				Return mtLotData.RegionNo
			End Get
		End Property
		Public ReadOnly Property ForcedArea() As Double
			Get
				Return mtLotData.ForcedArea
			End Get
		End Property

		Public Property LanduseID() As Integer
			Get
				Return mtLotData.LanduseID '   miLanduseID
			End Get
			Set(ByVal iValue As Integer)
				mtLotData.LanduseID = iValue
			End Set
		End Property
		Public Property BasicPlanID() As Integer
			Get
				Return miBasicPlanID
			End Get
			Set(ByVal iValue As Integer)
				miBasicPlanID = iValue
			End Set
		End Property

		Public Property LanduseOrder() As Integer
			Get
				Return miLanduseOrder
			End Get
			Set(ByVal iValue As Integer)
				miLanduseOrder = iValue
			End Set
		End Property


		Public Shared Sub GetLanduseInfo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLanduseID As Integer, ByRef sLanduseName As String, ByRef iOrder As Integer)
			Dim oLanduse As TplnLanduse = Nothing
			If mdicSumLanduses.TryGetValue(iLanduseID, oLanduse) Then
				oLanduse.GetColorSchemeInfo(iTopoPurpose, iLanduseID, sLanduseName, iOrder)
			Else
				sLanduseName = iLanduseID.ToString()
				iOrder = 0
			End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetLanduseInfo", iTopoPurpose, iLanduseID, sLanduseName, iOrder, mdicSumLanduses.Count)
		End Sub
		Public Property LanduseName() As String
			Get
				Dim sMsg As String = " יעוד  | לא נמצא "

				Dim oLanduse As TplnLanduse = Nothing
				If mdicSumLanduses IsNot Nothing Then
					mdicSumLanduses.TryGetValue(Me.LanduseID, oLanduse)
				End If



				If oLanduse IsNot Nothing Then

					'	oNewRow.Item(TplnParcel.msLanduseNameFieldName) = oLanduse.GetNameNew(MyBase.diTopoPurpose)

					'	oNewRow.Item(msLanduseOrderFieldName) = oLanduse.GetOrder(MyBase.diTopoPurpose)

					miLanduseOrder = oLanduse.GetOrder(MyBase.diTopoPurpose)
					Return oLanduse.GetNameNew(MyBase.diTopoPurpose)
				ElseIf Me.InPlan AndAlso (mdicSumLanduses IsNot Nothing) AndAlso mdicSumLanduses.Count <> 0 Then
					If Me.LanduseID <> 0 Then
						sMsg = Strings.Replace(sMsg, "|", CStr(Me.LanduseID))
						System.Windows.Forms.MessageBox.Show(sMsg & vbCrLf & CStr(mdicSumLanduses.Count), "#2760")
					End If

					For Each oLanduseA As TplnLanduse In mdicSumLanduses.Values
						'	DMAcadExt.AcadDocument.WriteMessage("LU_ID: " & CStr(oLanduseA.ID) & ":" & oLanduseA.GetNameNew(DMAcadExt.enTopoPurpose.Approved))
					Next


					Return " - "
				Else
					Return ""
				End If

				'msLanduseName
			End Get
			Set(ByVal sValue As String)
				msLanduseName = sValue
			End Set
		End Property
		Public Sub CalcTolerance()

		End Sub
		Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
			Get
				Return MyBase.diTopoPurpose
			End Get
			Set(ByVal iValue As DMAcadExt.enTopoPurpose)
				MyBase.diTopoPurpose = iValue
				System.Windows.Forms.MessageBox.Show("???", "Obsolete: Lot - PlanStatus")
			End Set
		End Property
		Public Sub UpdatePlan(iPlanID As Integer, sPlanName As String)
			BasicPlanID = iPlanID
			mtLotData.PlanName = sPlanName

		End Sub
		Public Sub UpdatePlanName(sPlanName As String)
			mtLotData.PlanName = sPlanName
			MyBase.UpdateCentroidAttributes(mtLotData.GetValues())


		End Sub

		Public Sub UpdateRegion(iRegion As Integer)


			mtLotData.RegionNo = iRegion

			'	MyBase.UpdateCentroidAttributes(mtLotData.GetValues())

			Dim iaAttribIndex() As Integer = {miaApprovedBlockAttribIndex(enLotCentroidAttribIndices.InPlan)}
			Dim saAttribText() As String = {iRegion.ToString()}



			DMAcadExt.AcadTransaction.UpdateAttribText(dtCentroidAcObjID, True, iaAttribIndex, saAttribText)

		End Sub
      Public Overloads Sub AddOverlayPgon(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oUnionPgon As TplnOverlayPgon)
         Dim iTopoPurpose As DMAcadExt.enTopoPurpose = MyBase.diTopoPurpose
         MyBase.AddOverlayPgon(iTopoPurpose, iOverlayMethod, oUnionPgon)
      End Sub
      Public Shared Sub PaintAll(iMapThemeID As DMAcadExt.enMapTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dScale As Double, ByVal sLayer As String)
         Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
         Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
         Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
         Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)
         Dim iPaintException As PaintException
         If dicLots IsNot Nothing Then
            Dim tColorScheme As DMAcadExt.ColorScheme
            For Each oLot As TplnLot In dicLots.Values
               tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, oLot.LanduseID, dScale)
               If tColorScheme.ID <> 0 Then
                  iPaintException = oLot.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayer)
                  MessageBox.Show(iPaintException.ToString(), "01_780")
                  If iPaintException <> PaintException.OK Then
                     Exit For
                  End If
               End If
            Next
         End If
      End Sub
      Public Shared Sub PaintByLanduseTopo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dScale As Double, ByVal sLayer As String)
         Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon) = TplnProject.GetLusePgons(iTopoPurpose)
         If dicLusePgons IsNot Nothing Then
            MessageBox.Show(dicLusePgons.ToString(), "01_783")
            Dim tColorScheme As DMAcadExt.ColorScheme
            For Each oLusePgon As TplnLusePgon In dicLusePgons.Values
               tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, oLusePgon.LanduseID, dScale)
               If tColorScheme.ID <> 0 Then
                  If oLusePgon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayer) <> PaintException.OK Then
                     Exit For
                  End If
               End If
            Next
         End If
      End Sub
      Public Shared Sub PaintByLanduseTopo(iMapThemeID As DMAcadExt.enMapTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dScale As Double, ByVal sLayer As String)
         Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
         Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
         Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon) = TplnProject.GetLusePgons(iTopoPurpose)
         Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)

         If dicLusePgons IsNot Nothing Then
            Dim tColorScheme As DMAcadExt.ColorScheme
            Dim tLanduseData As LanduseData = Nothing
            '	MessageBox.Show(CStr(dicLusePgons.Count) & ":" & CStr(dicLanduseData.Count), "04_259c")
            For Each oLusePgon As TplnLusePgon In dicLusePgons.Values
               If dicLanduseData.TryGetValue(oLusePgon.LanduseID, tLanduseData) Then
                  '	tColorScheme = tLanduseData.LuseColorScheme
                  tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, tLanduseData.LanduseID, dScale)
                  If tColorScheme.ID <> 0 Then
                     oLusePgon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, sLayer)
                  End If
               Else
                  MessageBox.Show(CStr(oLusePgon.LanduseID) & ":" & CStr(dicLanduseData.Count), "04_264c")
               End If
            Next
         Else
            System.Windows.Forms.MessageBox.Show("Topology Landuse is corrupted", "Tplnlot - PaintByLanduseTopo")
         End If
      End Sub
      'TplnProject.LoadLusePgons(iTopoPurpose)
      Public Shared Sub PaintByLanduse(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dScale As Double, ByVal bStraighten As Boolean, ByVal bBlueLine As Boolean)
         '	Dim sPrefix As String
         Dim oLotTopoModel As TopologyModel
         Dim sTopoName As String
         Dim sDissolveTopoName As String
         Dim sDissolveBaseTopoName As String
         Dim oDissolveTopoModel As Autodesk.Gis.Map.Topology.TopologyModel
         Dim tColorScheme As DMAcadExt.ColorScheme
         Dim oColorPolygon As TopoManager.ColorPolygon
         Dim bCurrentLayerOK As Boolean = True
         Dim iLanduseID As Integer
         MessageBox.Show(CStr(iLanduseID), "04_384p")
         If bStraighten Then
            Dim tAddTopoDefID As DMAcadExt.TopoDefID = TPlanGraph.TplnProject.GetAdditionalID(iTopoPurpose)
            sTopoName = TopoDefs.Item(tAddTopoDefID).Name
            'iPaintTopoPurpose = CType(TPlanGraph.TplnProject.GetAdditionalID(iTopoPurpose).ID, DMAcadExt.enTopoPurpose)
         Else
            sTopoName = TopoDefs.GetTopoName(iTopoPurpose)
         End If

         Dim oLayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(enMapTheme.LotApproved)
         Dim oTempLayer As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef
         Dim tDisTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose, DMAcadExt.enOverlayMethod.Dissolve)

         sDissolveBaseTopoName = TopoDefs.GetTopoName(tDisTopoDefID)
         Dim bDissolveTopologyExists As Boolean = TopoCreator.TopologyExists(sDissolveBaseTopoName)
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
         MessageBox.Show(CStr(iTopoPurpose), "15_423")
         bCurrentLayerOK = DMAcadExt.AcadTransaction.CreateLayer(oLayerDef, True)

         If bCurrentLayerOK Then
            bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(oTempLayer, True, False, False, False)
         End If

         If bDissolveTopologyExists Then
            PaintByLanduseTopo(iTopoPurpose, dScale, oLayerDef.Name)
         ElseIf sDissolveBaseTopoName.Length <> 0 AndAlso bCurrentLayerOK AndAlso mdicSumLanduses IsNot Nothing Then
            MessageBox.Show(" mdicSumLanduses.Count = " & CStr(mdicSumLanduses.Count), "15_432")

            oLotTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
            If oLotTopoModel IsNot Nothing Then ' AndAlso oLotTopoModel.Status <> Status.Closed 
               sDissolveTopoName = TopoManager.TopoCreator.GetTopologyName(sDissolveBaseTopoName)
               Try
                  oLotTopoModel.Dissolve("@CODE", sDissolveTopoName) '@PLAN   '.DWGNAME
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "PaintByLanduse_2")
               End Try

               oDissolveTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
               Dim oDissolvePgon As Autodesk.Gis.Map.Topology.Polygon
               Dim oLotPgon As Autodesk.Gis.Map.Topology.Polygon = Nothing
               Dim oLot As TplnLot
               Dim bRes As Boolean
               '	MessageBox.Show(tColorScheme.Border.Strip(0).Color.CommonString, "19_560")
               If oDissolveTopoModel IsNot Nothing Then
                  Dim colDissolvePolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oDissolveTopoModel.GetPolygons()
                  Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)
                  Dim oLanduse As TplnLanduse = Nothing
                  Dim iDissolvePgon As Integer = 0
                  AcadDocument.WriteMessageLog("DissolveTopo contains " & CStr(colDissolvePolygons.Count) & " poligon(s)")
                  For Each oDissolvePgon In colDissolvePolygons
                     iDissolvePgon += 1
                     Try
                        oLotPgon = oLotTopoModel.FindPolygon(oDissolvePgon.Centroid)
                     Catch oMapEx As Autodesk.Gis.Map.MapException
                        DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "PaintByLanduse", False, "CenterDisPgon=" & TPlnPoint.DispPoint(oDissolvePgon.Centroid))
                     End Try

                     AcadDocument.WriteDebugMessage("$$$$ " & TPlnPoint.DispPoint(oDissolvePgon.Centroid))
                     AcadDocument.WriteMessageLog("DissolvePgonCentroid #" & CStr(iDissolvePgon) & ": " & TPlnPoint.DispPoint(oDissolvePgon.Centroid))
                     If oLotPgon IsNot Nothing AndAlso dicLots.ContainsKey(oLotPgon.ID) Then ''''Lena(xotela)
                        oLot = dicLots.Item(oLotPgon.ID)
                        iLanduseID = oLot.LanduseID
                        If iLanduseID <> 0 Then
                           bRes = mdicSumLanduses.TryGetValue(iLanduseID, oLanduse)
                           If bRes Then
                              Dim iLanduseOrder As Integer
                              '	tColorScheme = New ColorScheme(iLanduseID, dScale)
                              AcadDocument.WriteMessageLog("LanduseID #" & CStr(oLanduse.ID))
                              tColorScheme = oLanduse.GetColorScheme(iTopoPurpose, dScale, iLanduseOrder) ' New DMAcadExt.ColorScheme(oLanduse.ID, dScale)
                              oColorPolygon = New TopoManager.ColorPolygon(oDissolvePgon)
                              oColorPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, oLayerDef.Name)
                              AcadDocument.WriteMessageLog("Paint pgon #" & CStr(iDissolvePgon) & " was finished!")
                           Else
                              Dim iLanduseOrder As Integer
                              oLanduse = New TplnLanduse(iLanduseID, iTopoPurpose)
                              tColorScheme = oLanduse.GetColorScheme(iTopoPurpose, dScale, iLanduseOrder)
                              '	tColorScheme = New ColorScheme(8411, dScale)
                              oColorPolygon = New TopoManager.ColorPolygon(oDissolvePgon)

                              ''''''''''''''''''''	oColorPolygon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
                              AcadDocument.WriteMessage("!!!^-- " & CStr(iLanduseID) & ": " & CStr(mdicSumLanduses.Count))
                           End If
                        Else
                           AcadDocument.WriteMessage("####-- " & "iLanduseID = 0")
                        End If
                     Else
                        AcadDocument.WriteMessage("####-- " & CStr(oLotPgon.ID) & " - was not found")
                     End If
                  Next
                  oDissolveTopoModel.Close()

                  '''''''''''''''TopoManager.TopoCreator.DeleteTopoLinks(sDissolveTopoName)
                  ''''''''''''''''''''''	TopoManager.TopoCreator.DeleteTopology(sDissolveTopoName, False)
               End If
               Try
                  oLotTopoModel.Close()
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "TplnLot - PaintByLanduse_1")
               End Try
            End If
         End If
         If bBlueLine Then
            zzBlueLine(iTopoPurpose, oLayerDef.Name)
         End If
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         '	zzUpdateBlockShare()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.CommandLine(True)
         DMAcadExt.AcadDocument.UpdateScreen()
      End Sub
      


      Public Shared ReadOnly Property LanduseCodeAttribIndex(ByVal iTopoPurpose As enTopoPurpose) As Integer
         Get
            If msCentroidProposedBlockName IsNot Nothing Then
               Select Case iTopoPurpose
                  Case enTopoPurpose.Approved
                     Return miaApprovedBlockAttribIndex(enLotCentroidAttribIndices.LanduseCode)
                  Case enTopoPurpose.Proposed
                     Return miaProposedBlockAttribIndex(enLotCentroidAttribIndices.LanduseCode)
                  Case Else
                     Return -1
               End Select
            Else
               Return -1
            End If

         End Get
      End Property

      Public Shared ReadOnly Property LanduseCodeAttribIndexNew(ByVal iTopoPurpose As enTopoPurpose) As Integer
         Get
            If True Then   'msCentroidProposedBlockName IsNot Nothing
               Select Case iTopoPurpose
                  Case enTopoPurpose.Approved
                     If miaApprovedBlockAttribIndex IsNot Nothing Then
                        Return miaApprovedBlockAttribIndex(enLotCentroidAttribIndices.LanduseCode)
                     Else
                        Return -1
                     End If

                  Case enTopoPurpose.Proposed
                     If miaProposedBlockAttribIndex IsNot Nothing Then
                        Return miaProposedBlockAttribIndex(enLotCentroidAttribIndices.LanduseCode)
                     Else
                        Return -1
                     End If

                  Case Else
                     Return -1
               End Select
            Else
               Return -1
            End If

         End Get
      End Property

      Private Shared Sub zzBlueLine(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal sLayerName As String)
         DMAcadExt.AcadDocument.WriteMessage("$$$$Start of BlueLine")
         Dim oLotTopoModel As TopologyModel '= Common.GetTopology(TopoDefs.GetTopoName(iTopoPurpose))
         Dim oDissolveTopoModel As TopologyModel
         Dim oColorPolygon As TopoManager.ColorPolygon
         Dim sBaseTopoName As String
         Dim sTopoName As String = TopoDefs.GetTopoName(iTopoPurpose)
         Dim sDissolveTopoName As String
         Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(999, 1.0)
         Select Case iTopoPurpose
            Case DMAcadExt.enTopoPurpose.Approved
               sBaseTopoName = "PlanOffsetK"
            Case DMAcadExt.enTopoPurpose.Proposed
               sBaseTopoName = "PlanOffsetM"
            Case Else
               sBaseTopoName = String.Empty
         End Select
         If sBaseTopoName.Length <> 0 Then  'AndAlso oTopoModel IsNot Nothing
            oLotTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
            If oLotTopoModel IsNot Nothing Then ' AndAlso oLotTopoModel.Status <> Status.Closed 
               sDissolveTopoName = TopoManager.TopoCreator.GetTopologyName(sBaseTopoName)
               Try
                  oLotTopoModel.Dissolve("@" & msInPlanAttribTag, sDissolveTopoName)   '@PLAN   '.DWGNAME
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "zzBlueLine")
               End Try

               oDissolveTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
               Dim oDissolvePgon As Autodesk.Gis.Map.Topology.Polygon
               Dim oLotPgon As Autodesk.Gis.Map.Topology.Polygon
               Dim oLot As TplnLot
               '	MessageBox.Show(tColorScheme.Border.Strip(0).Color.CommonString, "19_560")
               If oDissolveTopoModel IsNot Nothing Then
                  Dim colDissolvePolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oDissolveTopoModel.GetPolygons()
                  Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)

                  If colDissolvePolygons.Count = 1 Then
                     oDissolvePgon = colDissolvePolygons.Item(0)
                     oColorPolygon = New TopoManager.ColorPolygon(oDissolvePgon)
                     oColorPolygon.Paint(DMAcadExt.PaintMethod.Border Or DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True, sLayerName) ', Optional ByVal sPgonTopoName As String = ""
                  Else
                     For Each oDissolvePgon In colDissolvePolygons
                        oLotPgon = oLotTopoModel.FindPolygon(oDissolvePgon.Centroid)
                        AcadDocument.WriteDebugMessage("$$$$ " & TPlnPoint.DispPoint(oDissolvePgon.Centroid))
                        If oLotPgon IsNot Nothing AndAlso dicLots IsNot Nothing AndAlso dicLots.ContainsKey(oLotPgon.ID) Then
                           oLot = dicLots.Item(oLotPgon.ID)
                           If oLot.InPlan Then
                              'AcadDocument.WriteMessage("$$$$++ " & "In")
                              'AcadDocument.WriteMessage("!!4!!Start of New Color Pgon")
                              oColorPolygon = New TopoManager.ColorPolygon(oDissolvePgon)
                              'AcadDocument.WriteMessage("&&&Start of BlueLine Paint____")
                              oColorPolygon.Paint(DMAcadExt.PaintMethod.Border Or DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True, sLayerName)
                              '	AcadDocument.WriteMessage("End of BlueLine Paint ")
                           Else
                              AcadDocument.WriteMessage("$$$$-- " & "OutIn")
                           End If
                        Else
                           AcadDocument.WriteMessage("$$$$-- " & "OutOut")
                        End If
                     Next
                  End If
                  oDissolveTopoModel.Close()
                  TopoManager.TopoCreator.DeleteTopology(sDissolveTopoName, False, False)
               End If
               Try
                  oLotTopoModel.Close()
               Catch oMapEx As Autodesk.Gis.Map.MapException
                  DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "zzBlueLine_1")
               End Try

            End If
         End If
         DMAcadExt.AcadDocument.WriteDebugMessage("$$$-$$$$End of BlueLine")

      End Sub

		Public Overrides Sub Terminate()
         MyBase.Terminate()
      End Sub
#End Region
#Region "Private members"
      Private Sub zzParseInPlanString(ByVal sValue As String)
         If StrComp(sValue, "OUT", CompareMethod.Text) = 0 Then
            '	mbInPlan = False
         Else
            '	mbInPlan = True
         End If
      End Sub
      Public Shared Function StringToASCII(ByVal sValue As String) As String
         Dim sRes As String = String.Empty
         Try
            For iIndex As Integer = 0 To sValue.Length - 1
               sRes &= CStr(Asc(sValue.Substring(iIndex, 1))) & "|"
            Next
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - StringToASCII")
         End Try

         Return sRes
      End Function
      Private Sub zzParseLanduseString(ByVal sValue As String)
         'MyBase.diTopoPurpose
         Dim s As String = StringToASCII(sValue)
         sValue = sValue.Trim()
         Dim s1 As String = StringToASCII(sValue)
         If sValue.Length = 0 OrElse sValue = " " Then
            ''''			miLanduseID = 0
            '			ElseIf sValue.Length = 1 AndAlso (sValue = vbCr) Then
         Else
            Try
               '''''	miLanduseID = Convert.ToInt32(sValue)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & s & vbCrLf & s1 & vbCrLf & "'" & sValue & "'" & ":" & CStr(sValue.Length) & ":" & CStr(Asc(sValue)) & vbCrLf & CStr(Me.diTopoID), "TplnLot - zzParseLanduseString_1")
            End Try
            Try
               ''''		If miLanduseID <> 0 Then
               Dim oLanduseItem As TPlServerDB.TPlLanduseItem = TplnProject.GetLanduseItem(Me.LanduseID)
               If oLanduseItem IsNot Nothing Then
                  msLanduseName = oLanduseItem.Name
                  miLanduseOrder = oLanduseItem.OrderID
               End If
               ''''''''		End If
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - zzParseLanduseString_2")
            End Try
         End If
      End Sub
#End Region

      Protected Overrides Sub OnCalculate2(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)
         TplnProject.WriteMessageBox(dtaAreaSet(0).AcadArea.ToString() & ":" & diTopoPurpose.ToString() & ":" & CStr(Me.InPlan), "01_503 Lot")
         If Me.InPlan Then
            Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, diTopoPurpose)
            MyBase.dtaInPlanAreaSet(iOverlayIndex) = MyBase.AreaSet(iOverlayIndex)
            TplnProject.WriteMessageBox(dtaInPlanAreaSet(0).ToString() & ":" & diTopoPurpose.ToString(), "01_504 Lot")
         End If
      End Sub

      Protected Overrides ReadOnly Property PolygonCaption As String
         Get
            Return "מגרש"
         End Get
      End Property

      Protected Overrides ReadOnly Property _PolygonFullName As String
         Get
            Return Me.PolygonCaption & ": " & Me.Name
         End Get
      End Property



      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
				'MessageBox.Show(MyBase.diTopoPurpose.ToString(), "04_003")bb
				'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!Lot.BlockAttribIndex", miaApprovedBlockAttribIndex)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Lot.diTopoPurpose", diTopoPurpose)

				Select Case MyBase.diTopoPurpose
					Case DMAcadExt.enTopoPurpose.Approved
						Return miaApprovedBlockAttribIndex
					Case DMAcadExt.enTopoPurpose.Proposed
						Return miaProposedBlockAttribIndex
					Case Else
						Return Nothing
				End Select
			End Get
      End Property

      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            '	MessageBox.Show(MyBase.diTopoPurpose.ToString(), "04_003")
            Select Case MyBase.diTopoPurpose
               Case DMAcadExt.enTopoPurpose.Approved
                  Return miaApprovedBlockAttribIndex
               Case DMAcadExt.enTopoPurpose.Proposed
                  Return miaProposedBlockAttribIndex
               Case Else
                  Return Nothing
            End Select

         End Get

      End Property

   End Class
End Namespace
 