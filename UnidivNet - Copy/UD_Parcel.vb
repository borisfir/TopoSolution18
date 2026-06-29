Option Explicit On
Option Strict On
Public Class UD_Parcel
	Inherits TopoManager.TPlanGraph.TplnTopoPgon
	Public Enum enAreaStatus
		Exact	'colorind=61
		PlusPerm	'colorind=3
		MinusPerm 'colorind=60
		PlusEx 'colorind=4
		MinusEx 'colorind=51
		UB = MinusEx
	End Enum
	Friend Const msNameFieldName As String = "Parcel"
	Friend Const msNameNeighborFieldName As String = "ParcelNeighbor"
	Friend Const msParcelOrderFieldName As String = "POrder"
	Friend Const msParcelOrderNeighborFieldName As String = "POrderNeighbor"

	Friend Const msBlockFieldName As String = "Block"
	Friend Const msBlockNeighborFieldName As String = "BlockNeighbor"

	Friend Const msBlockTypeNameFieldName As String = "BlockType"

	Friend Const msAreaNeighborFldName As String = "AreaNeighbor"

	Public Const msLegalAreaFieldName As String = "LegalArea"
	Public Const msLegalAreaNeighborFieldName As String = "LegalAreaNeighbor"
	Public Const msMoveAreaFieldName As String = "MoveArea"

	Public Const msToleranceFieldName As String = "Tolerance"
	Public Const msToleranceNeighborFieldName As String = "ToleranceNeighbor"

	Public Const msDeltaAreaFieldName As String = "DeltaArea"
	Public Const msDeltaAreaNeighborFieldName As String = "DeltaAreaNeighbor"

	Public Const msDeviationFieldName As String = "Deviation"
	Public Const msDeviationNeighborFieldName As String = "DeviationNeighbor"

	Public Const msTopoIDNeighborFldName As String = "NeighborTopoID"


	Public Const msPgonAreaFieldName As String = "PgonArea"
	Friend Const msInLotAreaFieldName As String = "InLotArea"
	Friend Const msInLotCalcAreaFieldName As String = "InLotCalcArea"

	Friend Const msInPlanAreaFieldName As String = "InPlanArea"
	Friend Const msInPlanCalcAreaFieldName As String = "InPlanCalcArea"

	Public Const msInPlanCalcAreaApprFieldName As String = "InPlanCalcAreaAppr"
	Public Const msInPlanCalcAreaPropFieldName As String = "InPlanCalcAreaProp"

	Public Const msInPlanCalcAreaApprUnFieldName As String = "InPlanCalcAreaApprUn"
	Public Const msInPlanCalcAreaPropUnFieldName As String = "InPlanCalcAreaPropUn"
	Private Shared moAcadBlockDef As DMAcadExt.AcadBlockDef
	Private mdgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute = {New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetParcelName), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetBlockNo), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLegalArea)}
	Private Shared miaAttributesID() As DMAcadExt.enAcadAttributes = {DMAcadExt.enAcadAttributes.NameStr, DMAcadExt.enAcadAttributes.ParentNameNum, DMAcadExt.enAcadAttributes.LegalArea}
	Private Shared PaintAreaHatch(enAreaStatus.UB) As DMAcadExt.DMHatch
	Private miBlockNo As Integer
	Private msName As String
	Private miOrder As Integer
	Private mtParcelArea As ParcelArea
	Private mdLegalArea As Double = 0.0
	Private Shared miRoundDigit As Integer = 3

	'Private mdCalcArea As Double
	'	Private mdDeltaArea As Double
	'	Private mdTolerance As Double
	'	Private mdDeviation As Double
	Private mbHasDeviation As Boolean
	Private mdNeed As Double
	'Private miAreaStatus As enAreaStatus

	Private Shared moMainDataTable As System.Data.DataTable
	Private Shared moAdjoiningParcelsTable As System.Data.DataTable

	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon, False)
		'	System.Windows.Forms.MessageBox.Show("", "01_144")
		MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute)
		'	System.Windows.Forms.MessageBox.Show(CStr(miBlockNo) & ":" & msName & ":" & CStr(mdLegalArea), "UD_Parcel - New")


	End Sub
	Public ReadOnly Property BlockNo() As Integer
		Get
			Return miBlockNo
		End Get
	End Property
	Public ReadOnly Property Name() As String
		Get
			Return msName
		End Get
	End Property
	Public ReadOnly Property LegalArea() As Double
		Get
			Return mdLegalArea
		End Get
	End Property
	Public ReadOnly Property Order() As Integer
		Get
			Return miOrder
		End Get
	End Property
	Public ReadOnly Property ParcelArea() As ParcelArea
		Get
			Return mtParcelArea
		End Get
	End Property
	Public Sub Calc()
		mtParcelArea.CalculateArea(mdLegalArea, MyBase.ddAcadArea, miRoundDigit)
	End Sub

	Public Sub CalcOld()
		Dim mdDeviation As Double
		Dim mbHasDeviation As Boolean
		Dim mdNeed As Double
		Dim miAreaStatus As enAreaStatus
		Dim mdTolerance As enAreaStatus
		mtParcelArea.CalcArea = Math.Round(0.001 * MyBase.ddAcadArea, miRoundDigit)
		mtParcelArea.DeltaArea = Math.Round(mdLegalArea - mtParcelArea.CalcArea, miRoundDigit)
		Dim dSqrt As Double = Math.Sqrt(10.0 * mdLegalArea)
		Dim d1 As Double = 0.003 * dSqrt + 0.005 * mdLegalArea
		Dim d2 As Double = 0.008 * dSqrt + 0.002 * mdLegalArea
		mtParcelArea.Tolerance = Math.Round(Math.Min(d1, d2), miRoundDigit)
		mdDeviation = Math.Max(Math.Round(Math.Abs(Me.DeltaArea) - mdTolerance, miRoundDigit), 0.0)
		If mdDeviation > 0.0 Then
			mbHasDeviation = True
			If mtParcelArea.DeltaArea > 0.0 Then
				mdNeed = mdLegalArea - mtParcelArea.CalcArea - mdTolerance
				miAreaStatus = enAreaStatus.MinusEx
			Else
				mdNeed = mdLegalArea - mtParcelArea.CalcArea + mdTolerance
				miAreaStatus = enAreaStatus.PlusEx
			End If
		Else
			mbHasDeviation = False
			If mtParcelArea.DeltaArea = 0.0 Then
				miAreaStatus = enAreaStatus.Exact
			ElseIf mtParcelArea.DeltaArea > 0.0 Then
				miAreaStatus = enAreaStatus.MinusPerm
			Else
				miAreaStatus = enAreaStatus.PlusPerm
			End If
		End If
	End Sub
	Public Shared Sub Initialize()
		moAcadBlockDef = New DMAcadExt.AcadBlockDef(DMAcadExt.enAcadBlocks.UD_Parcel)
		moAcadBlockDef.AttributesID = miaAttributesID
		moAcadBlockDef.LoadDWG()
	End Sub
	Public Shared Sub CreateAdjoiningParcelsTable()
		moAdjoiningParcelsTable = New DataTable("AdjoiningParcels")
		With moAdjoiningParcelsTable.Columns
			.Add(msBlockFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))						'0
			.Add(msNameFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Text))
			.Add(msMoveAreaFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msLegalAreaFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))				'2

			.Add(TopoManager.TopoReader.msAreaFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msToleranceFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))						'4
			.Add(msDeltaAreaFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msDeviationFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))						'6


			.Add(msBlockNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))
			.Add(msNameNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Text))					'8
			.Add(msLegalAreaNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msAreaNeighborFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))				'10
			.Add(msToleranceNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msDeltaAreaNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))						'12
			.Add(msDeviationNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(TopoManager.TopoReader.msTopoIDFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))
			.Add(msTopoIDNeighborFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))
			.Add(msParcelOrderFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))
			.Add(msParcelOrderNeighborFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))
		End With
	End Sub
	Public Shared Sub CreateMainDataTable()
		moMainDataTable = New DataTable("Parcels")
		With moMainDataTable.Columns
			.Add(msBlockFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))						'0
			.Add(msNameFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Text))
			.Add(msLegalAreaFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))				'2
			.Add(TopoManager.TopoReader.msAreaFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msToleranceFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))							'4
			.Add(msDeltaAreaFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(msDeviationFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))							'6
			.Add(TopoManager.TopoReader.msCentroidXFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(TopoManager.TopoReader.msCentroidYFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))	'8

			.Add(TopoManager.TopoReader.msPerimeterFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Double))
			.Add(TopoManager.TopoReader.msTopoIDFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))			'10
			.Add(TopoManager.TopoReader.msAcObjIDFldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Numeric))
			.Add(msParcelOrderFieldName, TopoManager.Common.GetAppType(Odbc.OdbcType.Int))								'12
		End With
	End Sub
	Public ReadOnly Property CalcArea() As Double
		Get
			Return mtParcelArea.CalcArea
		End Get

	End Property

	Public ReadOnly Property Tolerance() As Double
		Get
			Return mtParcelArea.Tolerance
		End Get
	End Property
	Public ReadOnly Property DeltaArea() As Double
		Get
			Return mtParcelArea.DeltaArea
		End Get
	End Property
	Public ReadOnly Property Deviation() As Double
		Get
			Return mtParcelArea.Deviation
		End Get
	End Property
	Public ReadOnly Property HasDeviation() As Boolean
		Get
			Return mbHasDeviation
		End Get
	End Property
	Public ReadOnly Property Need() As Double
		Get
			Return mdNeed
		End Get
	End Property
	Public Shared ReadOnly Property MainView() As System.Data.DataView
		Get
			Dim sSort As String = msBlockFieldName & "," & msParcelOrderFieldName
			If moMainDataTable IsNot Nothing Then
				Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public Shared Sub PaintAllByArea()
		zzInitAreaColors()
		Dim dicParcels As Dictionary(Of Integer, UD_Parcel) = Unidiv.Parcels
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


		For Each oParcel As UD_Parcel In dicParcels.Values
			oParcel.PaintByArea()
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()

		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CommandLine(True)
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Public Sub PaintByArea()

		Dim tHatch As DMAcadExt.DMHatch = PaintAreaHatch(CInt(mtParcelArea.AreaStatus))
		'	DMAcadExt.AcadDocument.WriteMessage("###123 " & miAreaStatus.ToString())
		If mtParcelArea.AreaStatus <> enAreaStatus.Exact Then
			DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
			Me.PaintHatch(DMAcadExt.PaintMethod.Hatch, tHatch)
			DMAcadExt.AcadTransaction.InsertNewBlock(True)
		End If

	End Sub
	Public Function CanGive() As Double
		Return mtParcelArea.CanGive()
	End Function
	Public Function CanReceive() As Double
		Return mtParcelArea.CanReceive()
	End Function

	Public Shared Function GetMoveArea(ByVal oParcelA As UD_Parcel, ByVal oParcelB As UD_Parcel) As Double
		Dim dAtoB, dBtoA As Double
		If oParcelA.HasDeviation Then
			dAtoB = zzGetMoveArea(oParcelA, oParcelB)
		End If
		If oParcelB.HasDeviation Then
			dBtoA = -zzGetMoveArea(oParcelB, oParcelA)
		End If
		If Math.Abs(dAtoB) > Math.Abs(dBtoA) Then
			Return dAtoB
		Else
			Return dBtoA
		End If
	End Function
	Private Shared Function zzGetMoveArea(ByVal oNeed As UD_Parcel, ByVal oCan As UD_Parcel) As Double
		If oNeed.Need >= 0.0 Then
			Return Math.Round(Math.Min(oNeed.Need, oCan.CanGive()), miRoundDigit)
		Else
			Return Math.Round(Math.Max(oNeed.Need, -oCan.CanReceive()), miRoundDigit)
		End If
	End Function
	Public Shared ReadOnly Property AdjoiningParcelsView() As System.Data.DataView
		Get
			Dim sSort As String = msBlockFieldName & "," & msParcelOrderFieldName & "," & msParcelOrderNeighborFieldName
			If moAdjoiningParcelsTable IsNot Nothing Then
				Dim oDataView As System.Data.DataView = New System.Data.DataView(moAdjoiningParcelsTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public Sub AddDataToAdjoiningParcelsTable()
		Dim oNewRow As System.Data.DataRow
		Dim oNeighborParcel As UD_Parcel = Nothing
		If moAdjoiningParcelsTable IsNot Nothing Then
			Try
				DMAcadExt.AcadDocument.WriteDebugMessage("Naa Count=" & CStr(MyBase.Neighbors.Count))
				For Each iParcelID As Integer In MyBase.Neighbors.Keys
					DMAcadExt.AcadDocument.WriteDebugMessage("Nab iParcelID=" & CStr(iParcelID))
					If iParcelID <> 0 AndAlso Unidiv.TryGetParcel(iParcelID, oNeighborParcel) Then
						oNewRow = moAdjoiningParcelsTable.NewRow()
						With oNewRow
							.Item(msBlockFieldName) = miBlockNo
							.Item(msNameFieldName) = msName
							.Item(msMoveAreaFieldName) = GetMoveArea(Me, oNeighborParcel)
							.Item(msLegalAreaFieldName) = mdLegalArea

							.Item(TopoManager.TopoReader.msAreaFldName) = Me.CalcArea
							.Item(msToleranceFieldName) = Me.Tolerance
							.Item(msDeltaAreaFieldName) = Me.DeltaArea
							.Item(msDeviationFieldName) = Me.Deviation
							.Item(TopoManager.TopoReader.msTopoIDFldName) = Me.TopoID
							.Item(msParcelOrderFieldName) = Me.Order

							.Item(msBlockNeighborFieldName) = oNeighborParcel.BlockNo
							.Item(msNameNeighborFieldName) = oNeighborParcel.Name
							.Item(msLegalAreaNeighborFieldName) = oNeighborParcel.LegalArea
							.Item(msAreaNeighborFldName) = oNeighborParcel.CalcArea
							.Item(msToleranceNeighborFieldName) = oNeighborParcel.Tolerance
							.Item(msDeltaAreaNeighborFieldName) = oNeighborParcel.DeltaArea
							.Item(msDeviationNeighborFieldName) = oNeighborParcel.Deviation
							.Item(msTopoIDNeighborFldName) = oNeighborParcel.TopoID

						End With
						moAdjoiningParcelsTable.Rows.Add(oNewRow)
					End If
				Next

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - AddDataToMainTable_1")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
		End If

	End Sub

	Public Sub AddDataToMainTable()
		Dim oNewRow As System.Data.DataRow
		If moMainDataTable IsNot Nothing Then
			Try
				oNewRow = moMainDataTable.NewRow()
				With oNewRow
					.Item(msBlockFieldName) = miBlockNo
					.Item(msNameFieldName) = msName
					.Item(msLegalAreaFieldName) = mdLegalArea
					.Item(TopoManager.TopoReader.msAreaFldName) = Me.CalcArea
					.Item(msToleranceFieldName) = Me.Tolerance
					.Item(msDeltaAreaFieldName) = Me.DeltaArea
					.Item(msDeviationFieldName) = Me.Deviation
					.Item(TopoManager.TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
					.Item(TopoManager.TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
					.Item(TopoManager.TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
					.Item(TopoManager.TopoReader.msTopoIDFldName) = MyBase.TopoID
					.Item(TopoManager.TopoReader.msAcObjIDFldName) = MyBase.diCentroidAcObjID.OldIdPtr.ToInt64

					.Item(msParcelOrderFieldName) = miOrder
				End With
				moMainDataTable.Rows.Add(oNewRow)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - AddDataToMainTable_1")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
		End If

	End Sub
	Private Sub zzGetParcelName(ByVal sAttribValue As String)
		Dim oComplexNum As TopoManager.NumerationPair.ComplexNum
		msName = sAttribValue.Trim()
		If msName.Length <> 0 Then
			Try
				oComplexNum = New TopoManager.NumerationPair.ComplexNum(msName)
				miOrder = oComplexNum.Order
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, " zzGetParcelName")
			End Try
		End If
	End Sub
	Private Sub zzGetBlockNo(ByVal sAttribValue As String)
		Try
			'System.Windows.Forms.MessageBox.Show(sAttribValue, " 01_777")
			If sAttribValue.Length <> 0 Then
				miBlockNo = TopoManager.Common.NumberFilter(sAttribValue)
			End If

		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_808")
		End Try
	End Sub
	Private Sub zzGetLegalArea(ByVal sAttribValue As String)
		Try
			If sAttribValue.Length <> 0 Then
				mdLegalArea = Convert.ToDouble(sAttribValue)

			End If

		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_809")
		End Try
	End Sub
	Private Shared Sub zzInitAreaColors()

		PaintAreaHatch(0) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 61)))
		PaintAreaHatch(1) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 3)))
		PaintAreaHatch(2) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 60)))
		PaintAreaHatch(3) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 4)))
		PaintAreaHatch(4) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 51)))

	End Sub

	Public Overrides Sub Terminate()
		MyBase.OnTerminate()
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
Public Structure ParcelArea
	Public LegalArea As Double
	Public CalcArea As Double
	Public DeltaArea As Double
	Public Tolerance As Double
	Public Deviation As Double
	Public HasDeviation As Boolean
	Public Need As Double
	Public AreaStatus As UD_Parcel.enAreaStatus
	Public Function CanGive() As Double
		Return Math.Max(CalcArea - LegalArea + Tolerance, 0.0)
	End Function
	Public Function CanReceive() As Double
		Return Math.Max(LegalArea + Tolerance - CalcArea, 0.0)
	End Function
	Public Sub CalculateArea(ByVal dLegalArea As Double, ByVal dAcadArea As Double, ByVal iRoundDigit As Integer)
		LegalArea = dLegalArea
		CalcArea = Math.Round(0.001 * dAcadArea, iRoundDigit)
		DeltaArea = Math.Round(LegalArea - CalcArea, iRoundDigit)
		Dim dSqrt As Double = Math.Sqrt(10.0 * LegalArea)
		Dim d1 As Double = 0.003 * dSqrt + 0.005 * LegalArea
		Dim d2 As Double = 0.008 * dSqrt + 0.002 * LegalArea
		Tolerance = Math.Round(Math.Min(d1, d2), iRoundDigit)
		Deviation = Math.Max(Math.Round(Math.Abs(Me.DeltaArea) - Tolerance, iRoundDigit), 0.0)
		If Deviation > 0.0 Then
			HasDeviation = True
			If DeltaArea > 0.0 Then
				Need = LegalArea - CalcArea - Tolerance
				AreaStatus = UD_Parcel.enAreaStatus.MinusEx
			Else
				Need = LegalArea - CalcArea + Tolerance
				AreaStatus = UD_Parcel.enAreaStatus.PlusEx
			End If
		Else
			HasDeviation = False
			If DeltaArea = 0.0 Then
				AreaStatus = UD_Parcel.enAreaStatus.Exact
			ElseIf DeltaArea > 0.0 Then
				AreaStatus = UD_Parcel.enAreaStatus.MinusPerm
			Else
				AreaStatus = UD_Parcel.enAreaStatus.PlusPerm
			End If
		End If
	End Sub
	'	Public miAreaStatus As AreaStatus
End Structure
