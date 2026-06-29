Option Strict On
Option Explicit On
Imports System.ComponentModel
Imports System.Data
Public Class frmOwnership
	Const miMaxOwnerUB As Integer = 20
	Const miPaintAngleDflt As Integer = 45
	Enum enPaintingType
		[Default]
		Solid
	End Enum
	Private Const msRegulationName As String = "בהליך רישום / לא נמצא נסח"
	Private Const mdBaseScale As Double = 1000.0
	Private moOwnershipTable As System.Data.DataTable
	Private moParcelOwnershipDataView As System.Data.DataView

	Private moPrjOwnersTable As System.Data.DataTable

	Private miaActual() As Integer

	'	Private mhsOwnersWithField As HashSet(Of Integer) = New HashSet(Of Integer)()
	Private moOwnershipDataAdapter As Data.Common.DbDataAdapter
	Private moPrjOwnersDataAdapter As Data.Common.DbDataAdapter
	Private mdicLegendColoSchemes As Dictionary(Of Integer, DMAcadExt.ColorScheme)
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private msaOwnerNames() As String

	Private miOwnersCount As Integer
	Private midgvMainLocationY As Integer
	Private mfEditOwners As frmEditOwners
	'	Private mfEditNotes As frmEditNotes

	Private moPrevGridCell As DataGridViewCell
	Private moCurrentGridCell As DataGridViewCell
	Private mdTotalLegalInArea As Double
	Private mbParagraph19Exists As Boolean
	Private mbLeasingExists As Boolean

	Private mbDirty As Boolean
	Private mdicColorsA As Dictionary(Of Integer, Short)
	'	Private mbRegulationExists As Boolean
	Private moOwners As dmOwners
	Private moDataGridViewCellStyleError As System.Windows.Forms.DataGridViewCellStyle
	Private msaStatusValues() As String = {"כן", "VV", "Empty", "<>100", "I<>II", "R+"}
	Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmOwnership
	Private mdicDataRows As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
	Private miPaintingType As enPaintingType = enPaintingType.Default

	Public Event FormHided()

	Private Class TplnOwner
		Public ID As Integer
		Public FieldName As String
		Public Name As String

		Public AcadColor As Short
		Public OwnerCode As Integer
		Public Index As Integer
		Public MAPIMandatory As Boolean
		Private msShortName As String
		'Public Factor As Integer


		Public Sub New(iID As Integer, sFieldName As String, sName As String, iAcadColor As Integer, iOwnerCode As Integer)
			ID = iID
			FieldName = sFieldName
			Name = sName
			AcadColor = Convert.ToInt16(iAcadColor)
			OwnerCode = iOwnerCode
		End Sub
		Public Sub New(oDataReader As System.Data.Common.DbDataReader)
			Dim iAcadColor As Integer
			For iFieldIndex As Integer = 0 To oDataReader.FieldCount - 1
				If Not oDataReader.IsDBNull(iFieldIndex) Then
					Select Case oDataReader.GetName(iFieldIndex)
						Case "OwnerID"
							ID = oDataReader.GetInt32(iFieldIndex)
							'ShortName = "?" & ID.ToString()
						Case "FieldName"
							FieldName = oDataReader.GetString(iFieldIndex)
						Case "OwnerName"
							Name = oDataReader.GetString(iFieldIndex)
						Case "OwnerShortName"
							msShortName = oDataReader.GetString(iFieldIndex)
						Case "AcadColor"
							iAcadColor = oDataReader.GetInt32(iFieldIndex)
							AcadColor = Convert.ToInt16(iAcadColor)
						Case "Code"
							OwnerCode = oDataReader.GetInt32(iFieldIndex)
						Case "MAPIMandatory"
							MAPIMandatory = oDataReader.GetBoolean(iFieldIndex)

					End Select
				End If
			Next

		End Sub
		Public ReadOnly Property ShortName As String
			Get
				If String.IsNullOrEmpty(msShortName) Then
					Return Name
				Else
					Return msShortName
				End If
			End Get
		End Property

		Public Function GetInfo() As String()
			Return {ID.ToString, FieldName, Name, AcadColor.ToString(), OwnerCode.ToString, Index.ToString(), MAPIMandatory.ToString()}
		End Function
		Public Function HasField() As Boolean
			Return Not String.IsNullOrEmpty(FieldName)
		End Function
	End Class
	Private Structure dmOwnerSet
		Dim WinText As String
		Dim RepText As String
		Dim AcadColors() As Short
		Dim OwnerCode As Integer
		Dim Leasing As Boolean
		Dim Paragraph19 As Boolean
		Dim Paragraph126 As Boolean
		Dim SharedHouse As Boolean

		Dim BaseAngle As Double

		Dim Count As Integer

		Public Sub New(sWinText As String, sRepOwnerText As String, iaAcadColors() As Short, iOwnerCode As Integer, iCount As Integer, bParagraph19 As Boolean, bLeasing As Boolean, bParagraph126 As Boolean, bSharedHouse As Boolean, dBaseAngle As Double)
			WinText = sWinText
			RepText = sRepOwnerText
			AcadColors = iaAcadColors
			OwnerCode = iOwnerCode
			ReDim Preserve AcadColors(iCount - 1)
			Count = iCount
			Paragraph19 = bParagraph19
			Leasing = bLeasing
			Paragraph126 = bParagraph126
			SharedHouse = bSharedHouse
			BaseAngle = dBaseAngle
		End Sub
		Public Sub Add(sText As String, iAcadColor As Short, iOwnerCode As Integer)
			If String.IsNullOrEmpty(WinText) Then
				WinText = sText
				RepText = sText
			Else

				WinText &= vbCrLf
				RepText = vbCrLf & RepText

				WinText &= sText
				RepText = sText & RepText

			End If

			Dim iUB As Integer
			If AcadColors Is Nothing Then
				iUB = 0
			Else
				iUB = AcadColors.GetUpperBound(0) + 1

			End If
			ReDim Preserve AcadColors(Count)
			AcadColors(Count) = iAcadColor
			OwnerCode += iOwnerCode
			Count += 1
		End Sub
		Public Function Angle90() As Double
			Return (BaseAngle + 45.0) Mod 360.0
		End Function
	End Structure
	Private Class dmOwners
		Private mdicOwnersByFieldName As Dictionary(Of String, TplnOwner)
		Private mdicOwnersByID As Dictionary(Of Integer, TplnOwner)
		Private mhsOwnersWithField As HashSet(Of Integer)
		Private mhsMAPIOwners As HashSet(Of Integer)
		Private mhsAddActualOwners As HashSet(Of Integer)
		Private miOutputIndex As Integer
		Public Sub New()
			mdicOwnersByFieldName = New Dictionary(Of String, TplnOwner)()
			mdicOwnersByID = New Dictionary(Of Integer, TplnOwner)()
			mhsOwnersWithField = New HashSet(Of Integer)()
			mhsMAPIOwners = New HashSet(Of Integer)()
			mhsAddActualOwners = New HashSet(Of Integer)()
		End Sub
		Public Function TryGetValue(sFieldName As String, ByRef tOwner As TplnOwner) As Boolean
			Return mdicOwnersByFieldName.TryGetValue(sFieldName, tOwner)
		End Function
		Public Function TryGetValue(iID As Integer, ByRef tOwner As TplnOwner) As Boolean
			Return mdicOwnersByID.TryGetValue(iID, tOwner)
		End Function
		Public Sub Add(sFieldName As String, tOwner As TplnOwner)
			mdicOwnersByFieldName.Add(sFieldName, tOwner)
		End Sub
		Public Function MAPICaptions() As String()
			Dim saResult(mhsMAPIOwners.Count - 1 + mhsAddActualOwners.Count) As String
			Dim iResIndex As Integer = 0
			Dim oOwner As TplnOwner
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!mhsMAPI", mhsMAPIOwners.Count, mhsAddActualOwners.Count)

			For Each iOwnerID As Integer In mhsMAPIOwners
				oOwner = mdicOwnersByID.Item(iOwnerID)
				saResult(iResIndex) = oOwner.Name
				iResIndex += 1
			Next
			For Each iOwnerID As Integer In mhsAddActualOwners
				oOwner = mdicOwnersByID.Item(iOwnerID)
				saResult(iResIndex) = oOwner.Name
				iResIndex += 1
			Next
			Return saResult
		End Function
		Public Function MAPICaptionsNew() As String()
			Dim saResult(mhsMAPIOwners.Count - 1 + mhsAddActualOwners.Count) As String
			Dim iResIndex As Integer = 0
			'Dim oOwner As TplnOwner
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!mhsMAPI", mhsMAPIOwners.Count, mhsAddActualOwners.Count)
			For Each oOwner As TplnOwner In mdicOwnersByID.Values
				If mhsMAPIOwners.Contains(oOwner.ID) OrElse mhsAddActualOwners.Contains(oOwner.ID) Then
					saResult(iResIndex) = oOwner.Name
					oOwner.Index = iResIndex
					iResIndex += 1
				End If
			Next

			Return saResult
		End Function
		Public Sub ClearAddActual()

			'	tOwner.Index = miOutputIndex
			miOutputIndex = mhsMAPIOwners.Count
			mhsAddActualOwners.Clear()

		End Sub
		Public Sub AddActual(iOwnerID As Integer)
			If Not mhsMAPIOwners.Contains(iOwnerID) Then
				'	tOwner.Index = miOutputIndex
				'miOutputIndex += 1
				mhsAddActualOwners.Add(iOwnerID)
			End If
		End Sub
		Public Sub Add(iID As Integer, tOwner As TplnOwner)
			mdicOwnersByID.Add(iID, tOwner)
		End Sub
		Public Sub Add(tOwner As TplnOwner)

			mdicOwnersByID.Add(tOwner.ID, tOwner)

			If tOwner.HasField Then

				mdicOwnersByFieldName.Add(tOwner.FieldName, tOwner)
				mhsOwnersWithField.Add(tOwner.ID)
			End If
			If tOwner.MAPIMandatory Then
				tOwner.Index = miOutputIndex
				miOutputIndex += 1
				mhsMAPIOwners.Add(tOwner.ID)

			End If
		End Sub
		Public Function HasField(iOwnerID As Integer) As Boolean
			Dim oOwner As TplnOwner = Me.Item(iOwnerID)

			Return Not String.IsNullOrEmpty(oOwner.FieldName)
		End Function
		Public ReadOnly Property OwnersWithField As HashSet(Of Integer)
			Get
				Return mhsOwnersWithField
			End Get
		End Property
		Public Property Item(iID As Integer) As TplnOwner
			Get
				Return mdicOwnersByID.Item(iID)
			End Get
			Set(oValue As TplnOwner)
				mdicOwnersByID.Item(iID) = oValue
			End Set
		End Property
		Public Property Item(sFieldName As String) As TplnOwner
			Get
				Return mdicOwnersByFieldName.Item(sFieldName)
			End Get
			Set(oValue As TplnOwner)
				mdicOwnersByFieldName.Item(sFieldName) = oValue
			End Set
		End Property
		Public ReadOnly Property Values As Dictionary(Of Integer, TplnOwner).ValueCollection
			Get
				Return mdicOwnersByID.Values
			End Get
		End Property
		Public ReadOnly Property Count As Integer
			Get
				Return mdicOwnersByID.Count
			End Get
		End Property
	End Class
	Public Shared Sub GetReportData(ByVal iOptions As TopoManager.TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
		Dim oResTable As DataTable = New DataTable("Result")
		Dim oRow As DataRow

		oResTable.Columns.Add("BlockFullName", GetType(System.String))
		oResTable.Columns.Add("ParcelName", GetType(System.String))
		oResTable.Columns.Add("LegalArea", GetType(System.Double))
		oResTable.Columns.Add("CalcInArea", GetType(System.Double))
		oResTable.Columns.Add("Owners", GetType(System.String))
		oResTable.Columns.Add("Note", GetType(System.String))
		oResTable.Columns.Add("Paragraph19Area", GetType(System.Double))
		oResTable.Columns.Add("LeasingArea", GetType(System.Double))

		oRow = oResTable.NewRow()


		oRow.Item("BlockFullName") = "1234"
		oRow.Item("ParcelName") = 34
		oRow.Item("LegalArea") = 18.1
		oRow.Item("CalcInArea") = 9.2
		oRow.Item("Owners") = "אאאאאאאאאאאאאא"
		oRow.Item("Note") = "בבבבבבבבבבבבבבבבב"
		oRow.Item("Paragraph19Area") = 2.3
		oRow.Item("LeasingArea") = 3.2

		oResTable.Rows.Add(oRow)




		oDataView = oResTable.DefaultView

	End Sub
	Public Sub GetDataByDB(ByVal iOptions As TopoManager.TPlanGraph.enDataOptions, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
		'Dim sSQLCom As String = "SELECT TOP (100) PERCENT OwnerID,FieldName, OwnerName,AcadColor FROM dbo.OwnersList ORDER BY Priority"
		Const sSPName As String = "GetOwnershipData"
		Dim iBlockNo As Integer = 0
		Dim iBlockAddNo As Integer = 0
		Dim iParcelNo As Integer = 0

		Dim iRowBlockNo As Integer
		Dim iRowBlockAddNo As Integer
		Dim iRowParcelNo As Integer
		Dim dParcelInLegalArea As Double
		Dim dParagraph19Area As Double
		Dim dLeasingArea As Double

		Dim dTotalParcelInLegalArea As Double
		Dim dTotalParagraph19Area As Double
		Dim dTotalLeasingArea As Double
		Dim oReportTable As DataTable = zzCreateRepTable()
		Dim oReportRow As DataRow = Nothing
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, zzGetParameters())
		Dim tOwnerSet As dmOwnerSet = New dmOwnerSet()
		Dim sOwnerText As String
		Dim shAcadColor As Short
		Dim iOwnerCode As Integer
		Dim lKey As Long
		Dim tAreaSet As TopoManager.TPlanGraph.TplnAreaSet
		Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
		Dim dicParcelArea As Dictionary(Of Long, TopoManager.TPlanGraph.TplnParcel) = New Dictionary(Of Long, TopoManager.TPlanGraph.TplnParcel)()

		zzLoadParcels(iOptions)
		Dim dicParcels As TopoManager.TPlanGraph.TplnParcels = TopoManager.TPlanGraph.TplnProject.Parcels
		For Each oParcel In dicParcels.Values
			lKey = zzGetParcelKey(oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo)
			Try
				dicParcelArea.Add(lKey, oParcel)
			Catch oEx As Exception
				DMCommon.Debug.UserMsg("#857", oEx.Message, dicParcelArea.Count, oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo, lKey)
			End Try
		Next

		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read
					iRowBlockNo = oDataReader.GetInt32(2)
					iRowBlockAddNo = oDataReader.GetInt32(3)
					iRowParcelNo = oDataReader.GetInt32(4)
					lKey = zzGetParcelKey(iRowBlockNo, iRowBlockAddNo, iRowParcelNo)
					If iRowBlockNo <> iBlockNo OrElse iRowBlockAddNo <> iBlockAddNo OrElse iRowParcelNo <> iParcelNo Then
						If oReportRow IsNot Nothing Then
							If tOwnerSet.Count <> 0 Then
								oReportRow.Item("Owners") = tOwnerSet.RepText
							End If
							oReportTable.Rows.Add(oReportRow)
						End If

						oReportRow = oReportTable.NewRow()
						oReportRow.Item("BlockFullName") = iRowBlockNo.ToString()
						oReportRow.Item("ParcelName") = iRowParcelNo.ToString()
						oDataReader.GetDataTypeName(6)

						Try
							'DMCommon.Debug.MsgBox("13_141s", oDataReader.GetDouble(6))
							oReportRow.Item("LegalArea") = oDataReader.GetDouble(6)
						Catch oEx As Exception

						End Try

						dParcelInLegalArea = DMCommon.Functions.CDblN(oDataReader, 7, 0.0)
						If dicParcelArea.TryGetValue(lKey, oParcel) Then
							tAreaSet = oParcel.InPlanAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)

							Select Case iOptions
								Case TopoManager.TPlanGraph.enDataOptions.AcadArea
									dParcelInLegalArea = tAreaSet.AcadArea
								Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea
									dParcelInLegalArea = tAreaSet.CalcArea
								Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea2
									dParcelInLegalArea = tAreaSet.CalcArea2
								Case TopoManager.TPlanGraph.enDataOptions.RoundedArea
									dParcelInLegalArea = tAreaSet.RoundedArea
								Case TopoManager.TPlanGraph.enDataOptions.CalcRoundedArea
									If oParcel.IsAnalytic Then
										dParcelInLegalArea = tAreaSet.CalcArea
									Else
										dParcelInLegalArea = tAreaSet.RoundedArea
									End If
								Case Else
									dParcelInLegalArea = 0.0
							End Select
						End If

						dParagraph19Area = DMCommon.Functions.CDblN(oDataReader, 13, 0.0)
						dLeasingArea = DMCommon.Functions.CDblN(oDataReader, 14, 0.0)

						oReportRow.Item("CalcInArea") = dParcelInLegalArea

						dTotalParcelInLegalArea += dParcelInLegalArea
						dTotalParagraph19Area += dParagraph19Area
						dTotalLeasingArea += dLeasingArea



						tOwnerSet = New dmOwnerSet()
						iBlockNo = iRowBlockNo
						iBlockAddNo = iRowBlockAddNo
						iParcelNo = iRowParcelNo

					End If
					sOwnerText = oDataReader.GetString(19)
					shAcadColor = Convert.ToInt16(oDataReader.GetInt32(21))
					iOwnerCode = oDataReader.GetInt32(23)
					tOwnerSet.Add(sOwnerText, shAcadColor, iOwnerCode)



					If oReportRow IsNot Nothing Then

						oReportRow.Item("Note") = DMCommon.Functions.CStrN(oDataReader, 12)
						If dParagraph19Area <> 0.0 Then
							oReportRow.Item("Paragraph19Area") = dParagraph19Area
						End If
						If dLeasingArea <> 0.0 Then
							oReportRow.Item("LeasingArea") = dLeasingArea
						End If

					End If






				Loop
				If oReportRow IsNot Nothing Then
					If tOwnerSet.Count <> 0 Then
						oReportRow.Item("Owners") = tOwnerSet.RepText
					End If
					oReportTable.Rows.Add(oReportRow)
				End If

			End If
			oDataReader.Close()

			oDataView = oReportTable.DefaultView
			'	DMCommon.Debug.MsgBox("13_141", dTotalParcelInLegalArea, dTotalParagraph19Area, dTotalLeasingArea)
			oaTotals = {dTotalLeasingArea, dTotalParagraph19Area, dTotalParcelInLegalArea}

		End If

	End Sub
	Private Sub zzCalcCheckData1()


		'	txtTotalArea.Text = Convert.ToString(TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea)
		txtTotalArea.Text = FormatNumber(TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea, 0, TriState.False,, TriState.True)

	End Sub
	Private Sub zzCalcCheckData()

		Dim oDataRow As Data.DataRowView
		Dim oGridRow As DataGridViewRow
		TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea = 0.0
		'For Each oDataRow1 As Data.DataRowView In moParcelOwnershipDataView
		'zzCheckRow(oDataRow1)
		'	Next
		mdicDataRows.Clear()
		For iRowIndex As Integer = 0 To moParcelOwnershipDataView.Count - 1
			oDataRow = moParcelOwnershipDataView.Item(iRowIndex)
			oGridRow = Me.dgvMain.Rows.Item(iRowIndex)



			mdicDataRows.Add(DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID")), iRowIndex)


			zzCalcCheckRow(True, oDataRow, oGridRow)
		Next
		'DMCommon.Debug.MsgBox("13_128da", moParcelOwnershipDataView.Count, DMCommon.Debug.ColCount(moOwnershipTable.Rows), DMCommon.Debug.ColCount(mdicDataRows))
		'	txtTotalArea.Text = Convert.ToString(TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea)
		txtTotalArea.Text = FormatNumber(TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea, 0, TriState.False,, TriState.True)

	End Sub
	Private Function zzGetParcel(oDataRow As Data.DataRow) As TopoManager.TPlanGraph.TplnParcel
		Dim iTopoID As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID"))

		If iTopoID <> 0 Then
			Return TopoManager.TPlanGraph.TplnProject.GetParcel(iTopoID)
		Else
			Return Nothing
		End If
	End Function
	Private Function zzGetParcel(oDataRow As Data.DataRowView) As TopoManager.TPlanGraph.TplnParcel
		Dim iTopoID As Integer = DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID"))

		If iTopoID <> 0 Then
			Return TopoManager.TPlanGraph.TplnProject.GetParcel(iTopoID)
		Else
			Return Nothing
		End If
	End Function
	Private Sub zzCalcCheckRow(bCalcTotal As Boolean, oDataRow As Data.DataRowView, ByRef oGridRow As DataGridViewRow)
		Dim iStatus As Integer = 0
		Dim iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer
		Dim hsOwnersBefore As HashSet(Of Integer) = zzGetOwnerRowList(oDataRow)
		Dim hsOwnersAfter As HashSet(Of Integer) = Nothing
		Dim colPartPct As System.Collections.ObjectModel.Collection(Of Double) = Nothing
		Dim oBalanceAcadArea As TopoManager.BalanceArea
		Dim oDataView As DataView = zzGetParcelDataView(True, iBlockNo, iBlockAddNo, iParcelNo, oDataRow)
		Dim dParcelInLegalArea As Double
		Dim iTestBefore As Integer = hsOwnersBefore.Count
		Dim iTestAfter As Integer = 0

		Dim oParcel As TopoManager.TPlanGraph.TplnParcel
		Dim taOwnerArea() As TopoManager.TPlanGraph.TplnOwnerArea = Nothing

		Dim dParagraph19Area As Double = 0.0
		Dim dLeasingArea As Double = 0.0
		Dim dOverlayPrg5LeasArea As Double = 0.0
		'	DMCommon.Debug.MsgBox("13_129r")
		Dim bParagraph19 As Boolean = DMCommon.Functions.CBoolN(oDataRow.Item("Paragraph19")) OrElse DMCommon.Functions.CDblN(oDataRow.Item("Paragraph19Area")) > 0.0
		Dim bLeasing As Boolean = DMCommon.Functions.CBoolN(oDataRow.Item("Leasing")) OrElse DMCommon.Functions.CDblN(oDataRow.Item("LeasingArea")) > 0.0
		Dim bHasRegulation As Boolean = DMCommon.Functions.CBoolN(oDataRow.Item("Regulation"))
		'DMCommon.Debug.MsgBox("13_129s")
		Dim tAreaSet As TopoManager.TPlanGraph.TplnAreaSet
		Dim dParcelArea As Double
		Dim iOptions As TopoManager.TPlanGraph.enDataOptions = TopoManager.TPlanGraph.enDataOptions.CalcMergeArea

		oParcel = zzGetParcel(oDataRow)
		'	DMCommon.Debug.MsgBox("13_129u")
		dParcelInLegalArea = DMCommon.Functions.CDblN(oDataRow.Item("ParcelInLegalArea"))
		oParcel.GetOwnershipNoteArea(dParagraph19Area, dLeasingArea, dOverlayPrg5LeasArea)
		'DMCommon.Debug.MsgBox("13_129z")
		tAreaSet = oParcel.InPlanAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
		Select Case iOptions
			Case TopoManager.TPlanGraph.enDataOptions.AcadArea
				dParcelArea = tAreaSet.AcadArea
			Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea
				dParcelArea = tAreaSet.CalcArea
			Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea2
				dParcelArea = tAreaSet.CalcArea2
			Case TopoManager.TPlanGraph.enDataOptions.RoundedArea
				dParcelArea = tAreaSet.RoundedArea
			Case TopoManager.TPlanGraph.enDataOptions.CalcRoundedArea
				If oParcel.IsAnalytic Then
					dParcelArea = tAreaSet.CalcArea
				Else
					dParcelArea = tAreaSet.RoundedArea
				End If
		End Select

		If Not mbParagraph19Exists AndAlso bParagraph19 Then
			mbParagraph19Exists = True
		End If
		If Not mbLeasingExists AndAlso bLeasing Then
			mbLeasingExists = True
		End If

		If bParagraph19 Then
		Else

		End If



		If bParagraph19 Then
			oDataRow.Item("Paragraph19Area") = oDataRow.Item("ParcelInLegalArea")
		Else
			'oDataRow.Item("Paragraph19Area") = Math.Round(dParagraph19Area * dParcelArea * 0.001 / tAreaSet.AcadArea, 3)
			oDataRow.Item("Paragraph19Area") = Math.Round(dParagraph19Area * dParcelArea / tAreaSet.AcadArea, 3)

		End If

		If DMCommon.Functions.CBoolN(oDataRow.Item("Leasing")) Then
			oDataRow.Item("LeasingArea") = oDataRow.Item("ParcelInLegalArea")
		Else
			oDataRow.Item("LeasingArea") = Math.Round(dLeasingArea * dParcelArea * 0.001 / tAreaSet.AcadArea, 3)
		End If
		If DMCommon.Functions.CBoolN(oDataRow.Item("Paragraph19")) AndAlso DMCommon.Functions.CBoolN(oDataRow.Item("Leasing")) Then

			oDataRow.Item("OverlayPrg5LeasArea") = oDataRow.Item("ParcelInLegalArea")
		ElseIf DMCommon.Functions.CBoolN(oDataRow.Item("Paragraph19")) Then

			oDataRow.Item("OverlayPrg5LeasArea") = Math.Round(dLeasingArea * dParcelArea / tAreaSet.AcadArea, 3)

		ElseIf DMCommon.Functions.CBoolN(oDataRow.Item("Leasing")) Then
			oDataRow.Item("OverlayPrg5LeasArea") = Math.Round(dParagraph19Area * dParcelArea / tAreaSet.AcadArea, 3)
		Else
			oDataRow.Item("OverlayPrg5LeasArea") = Math.Round(dOverlayPrg5LeasArea * dParcelArea / tAreaSet.AcadArea, 3)
		End If


		If oDataView.Count = 0 Then
			If hsOwnersBefore.Count > 1 Then
				'Err 1
				iStatus = 1
			ElseIf hsOwnersBefore.Count = 0 Then
				'	Err 2
				iStatus = 2  'Empty
			Else
				iStatus = 0
				taOwnerArea = {New TopoManager.TPlanGraph.TplnOwnerArea(hsOwnersBefore.ElementAt(0), dParcelInLegalArea)}
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Check1", hsOwnersBefore.ElementAt(0), taOwnerArea(0).OwnerID, taOwnerArea(0).AreaSet.CalcArea)
				If bCalcTotal Then
					zzAddOwnerArea(taOwnerArea(0))
				End If


			End If
		Else
			If zzCheckPctSum(oDataView, hsOwnersAfter, colPartPct) Then
				'	Err 3
				iStatus = 3
			Else
				Dim daPartPct(colPartPct.Count - 1) As Double
				'Dim tOwnerArea As TopoManager.TPlanGraph.TplnOwnerArea
				ReDim taOwnerArea(hsOwnersAfter.Count - 1)
				''''''''''''''''	Dim iaOwnersID(hsOwnersAfter.Count - 1) As Integer

				colPartPct.CopyTo(daPartPct, 0)
				oBalanceAcadArea = New TopoManager.BalanceArea(daPartPct, 1.0, dParcelInLegalArea, True, "ParcOwner")
				For iIndex As Integer = 0 To hsOwnersAfter.Count - 1
					taOwnerArea(iIndex) = New TopoManager.TPlanGraph.TplnOwnerArea(hsOwnersAfter.ElementAt(iIndex), oBalanceAcadArea.OutputItemFloat(iIndex))
					If bCalcTotal Then
						zzAddOwnerArea(taOwnerArea(iIndex))
					End If
				Next

			End If
			iTestAfter = hsOwnersAfter.Count
			hsOwnersBefore.SymmetricExceptWith(hsOwnersAfter)
			Dim bRes As Boolean
			If hsOwnersBefore.Count > 0 Then

				'	Err 4
				For Each iOwnerID As Integer In hsOwnersBefore
					If moOwners.HasField(iOwnerID) Then
						iStatus = 4
					End If
				Next


			End If
		End If
		If hsOwnersBefore.Count > 1 AndAlso iStatus = 0 AndAlso bHasRegulation Then
			iStatus = 5
		End If
		If iStatus <> 0 Then
			oGridRow.Cells.Item("ctxStatus").Style = moDataGridViewCellStyleError
		Else
			oGridRow.Cells.Item("ctxStatus").Style = Me.dgvMain.DefaultCellStyle
			If oParcel IsNot Nothing AndAlso taOwnerArea IsNot Nothing Then
				oParcel.OwnerArea = taOwnerArea
			End If
		End If

		oGridRow.Cells.Item("ctxStatus").Value = msaStatusValues(iStatus)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Check", oDataRow.Item("BlockNo"), oDataRow.Item("ParcelNo"), iTestBefore, oDataView.Count, iTestAfter, hsOwnersBefore.Count, iStatus)
	End Sub
	Private Sub zzAddOwnerArea(tOwnerArea As TopoManager.TPlanGraph.TplnOwnerArea)
		Dim dPrevArea As Double = 0.0
		Dim iOwnerID As Integer = tOwnerArea.OwnerID
		Dim dArea As Double = tOwnerArea.AreaSet.CalcArea
		If TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.TryGetValue(iOwnerID, dPrevArea) Then
			TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.Remove(iOwnerID)
		End If
		TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.Add(iOwnerID, dPrevArea + dArea)
		TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea += dArea
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddArea", dArea, iOwnerID, dPrevArea, TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.Count)
	End Sub
	Private Function zzCheckPctSum(oDataView As DataView, ByRef hsOwnersAfter As HashSet(Of Integer), ByRef colPartPct As System.Collections.ObjectModel.Collection(Of Double)) As Boolean
		Const iFactor As Double = 10000.0
		Const i100Pct As Integer = 1000000
		Dim iSum As Integer = 0
		Dim iOwnerID As Integer
		Dim dPartPct As Double
		hsOwnersAfter = New HashSet(Of Integer)()

		colPartPct = New System.Collections.ObjectModel.Collection(Of Double)()
		For iRowIndex As Integer = 0 To oDataView.Count - 1

			dPartPct = DMCommon.Functions.CDblN(oDataView.Item(iRowIndex).Item("PartPct"))
			iSum += CInt(dPartPct * iFactor)
			colPartPct.Add(dPartPct)
			'	iSum += DMCommon.Functions.CDblToIntN(oDataView.Item(iRowIndex).Item("PartPct"), iFactor)
			iOwnerID = DMCommon.Functions.CIntN(oDataView.Item(iRowIndex).Item("OwnerID"))
			If Not hsOwnersAfter.Contains(iOwnerID) Then
				hsOwnersAfter.Add(iOwnerID)
			End If
		Next

		'DMCommon.Debug.ExcelLog.SetNextValue(0, "<>100", iSum, i100Pct)
		Return iSum <> i100Pct
	End Function
	Private Function zzGetParcelKey(iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer) As Long
		Dim lBlockNo As Long = Convert.ToInt64(iBlockNo)
		Dim lBlockAddNo As Long = Convert.ToInt64(iBlockAddNo)
		Dim lParcelNo As Long = Convert.ToInt64(iParcelNo)
		Return (lBlockNo * 1000L + lBlockAddNo) * 10000L + lParcelNo
	End Function
	Public Sub GetOwnersSumNew(ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaCaptions() As System.Object, ByRef oaTotals() As System.Object)
		Dim oResTable As DataTable = New DataTable("TotalArea")
		Dim dOwnerArea As Double
		Dim dTotalArea As Double = TopoManager.TPlanGraph.TplnParcel.OwnerTotalArea

		Dim oDataRow As DataRow
		If dTotalArea <> 0.0 Then
			Dim daPct(moOwners.Count - 1) As Double
			Dim iIndex As Integer = 0
			Dim dPctSum As Double
			oResTable.Columns.Add("OwnerID", GetType(System.Int32))
			oResTable.Columns.Add("OwnerName", GetType(System.String))
			oResTable.Columns.Add("Area", GetType(System.Double))
			oResTable.Columns.Add("AreaPct", GetType(System.Double))
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!moOwners", moOwners.Count, moOwners.Values.Count, TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.Count)
			For Each tOwner As TplnOwner In moOwners.Values
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!OwnerArea", tOwner.ID)
				If TopoManager.TPlanGraph.TplnParcel.OwnerTotalAreas.TryGetValue(tOwner.ID, dOwnerArea) AndAlso dOwnerArea > 0.0 Then
					oDataRow = oResTable.NewRow()
					oDataRow.Item("OwnerID") = tOwner.ID
					oDataRow.Item("OwnerName") = tOwner.Name
					oDataRow.Item("Area") = dOwnerArea
					daPct(iIndex) = Math.Round(dOwnerArea * 100.0 / dTotalArea, 3, MidpointRounding.AwayFromZero)
					dPctSum += daPct(iIndex)
					oDataRow.Item("AreaPct") = daPct(iIndex)
					oResTable.Rows.Add(oDataRow)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!OwnerRow", oResTable.Rows.Count)
					iIndex += 1
				End If

			Next
			mdTotalLegalInArea = dTotalArea
			If Math.Abs(dPctSum - 100.0) > 0.0001 Then
				ReDim Preserve daPct(iIndex - 1)
				Dim oBalanceAcadArea As TopoManager.BalanceArea = New TopoManager.BalanceArea(daPct, 1000.0, 100.0, True, "Os")
				For iRowIndex As Integer = 0 To oResTable.Rows.Count - 1
					oDataRow = oResTable.Rows.Item(iRowIndex)
					oDataRow.Item("AreaPct") = oBalanceAcadArea.OutputItemFloat(iRowIndex)
				Next
			End If

			oDataView = New DataView(oResTable)
		End If
		iaColumns = {1, 2, 3}
		oaCaptions = {"אאאאאאאא", "אבאאאאבב", "גאאאאג"}
		oaTotals = {"100.0", dTotalArea, zzGetText(6, 1)}
	End Sub
	Public Sub GetOwnersSum(ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaCaptions() As System.Object, ByRef oaTotals() As System.Object)
		Const sSPName As String = "GetOwnersSum"
		Dim oResTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sSPName, CommandType.StoredProcedure, "OwnersSum", zzGetParameters())
		Dim dTotalArea As Double
		For Each oRow As DataRow In oResTable.Rows
			dTotalArea += DMCommon.Functions.CDblN(oRow.Item("TotalArea"))
		Next
		mdTotalLegalInArea = dTotalArea
		oResTable.Columns.Add("AreaPct", GetType(System.Double))
		For Each oRow As DataRow In oResTable.Rows
			oRow.Item("AreaPct") = DMCommon.Functions.CDblN(oRow.Item("TotalArea")) * 100 / dTotalArea
		Next

		oDataView = New DataView(oResTable)

		iaColumns = {1, 2, 3}
		oaCaptions = {"אאאאאאאא", "אבאאאאבב", "גאאאאג"}
		oaTotals = {"100.0", dTotalArea, zzGetText(6, 1)}
	End Sub
	Public Sub GetStateOwnersSum(ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaCaptions() As System.Object, ByRef oaTotals() As System.Object)
		Const sSPName As String = "GetStateOwnersSum"
		Dim oDataReader As System.Data.Common.DbDataReader
		Dim dGrossArea As Double
		Dim dParagraph19Area As Double
		Dim dLeasingArea As Double
		Dim dOverlayPrg19LeasArea As Double

		Dim dNetArea As Double

		If mdTotalLegalInArea = 0.0 Then
			GetOwnersSum(oDataView, iaColumns, oaCaptions, oaTotals)
		End If
		oDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, zzGetParameters())
		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read
					dGrossArea = DMCommon.Functions.CDblN(oDataReader, 0, 0.0)
					dParagraph19Area = DMCommon.Functions.CDblN(oDataReader, 1, 0.0)
					dLeasingArea = DMCommon.Functions.CDblN(oDataReader, 2, 0.0)
					dOverlayPrg19LeasArea = DMCommon.Functions.CDblN(oDataReader, 3, 0.0)

				Loop
				dNetArea = dGrossArea - dParagraph19Area - dLeasingArea + dOverlayPrg19LeasArea
			End If
			oDataReader.Close()
		End If

		Dim oResTable As DataTable = New DataTable("Result")
		Dim oNewRow As DataRow

		oResTable.Columns.Add("Caption", GetType(System.String))
		oResTable.Columns.Add("Area", GetType(System.Double))
		oResTable.Columns.Add("PartPct", GetType(System.Double))

		oNewRow = oResTable.NewRow()
		oNewRow.Item("Caption") = zzGetText(0, 1)
		oNewRow.Item("Area") = dGrossArea
		oResTable.Rows.Add(oNewRow)

		oNewRow = oResTable.NewRow()
		oNewRow.Item("Caption") = zzGetText(1, 1)
		oNewRow.Item("Area") = dParagraph19Area
		oResTable.Rows.Add(oNewRow)

		oNewRow = oResTable.NewRow()
		oNewRow.Item("Caption") = zzGetText(2, 1)
		oNewRow.Item("Area") = dLeasingArea
		oResTable.Rows.Add(oNewRow)


		If dOverlayPrg19LeasArea <> 0.0 Then
			oNewRow = oResTable.NewRow()
			oNewRow.Item("Caption") = zzGetText(3, 1)
			oNewRow.Item("Area") = dOverlayPrg19LeasArea
			oResTable.Rows.Add(oNewRow)
		End If


		oNewRow = oResTable.NewRow()
		oNewRow.Item("Caption") = zzGetText(4, 1)
		If mdTotalLegalInArea <> 0 Then
			oNewRow.Item("Area") = dNetArea
			oNewRow.Item("PartPct") = dNetArea * 100.0 / mdTotalLegalInArea
		End If
		oResTable.Rows.Add(oNewRow)



		oDataView = New DataView(oResTable)

		iaColumns = {0, 1, 2}
		oaCaptions = {"בבבבבבב"}
		oaTotals = {FormatNumber(dNetArea * 100.0 / mdTotalLegalInArea, 3) & " %", String.Empty, zzGetText(5, 1)}
	End Sub
	Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
		Return TPlServerDB.TextResource.GetText(iItemID, TPlServerDB.enResourceTheme.AcFrmOwnership, iSectionID, True)
	End Function





	Public Sub New() 'Optional oAppWin As Autodesk.AutoCAD.Windows.Window = Nothing

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()

		midgvMainLocationY = Me.dgvMain.Location.Y
		zzInitUD_Project()
		zzLoadOwnershipTable(False)
		'''''''''''''''''''''	zzAddStatusField()
		'zzLoadParcelMainTable()
		zzLoadPrjOwnersTable(False)

		zzAddPrjOwnersFiels()
		zzLoadParcels(TopoManager.TPlanGraph.enDataOptions.CalcMergeArea)


		'	zzGetOwnerNames()
		zzGetOwners()

	End Sub
	Private Sub zzAddPrjOwnersFiels()
		moPrjOwnersTable.Columns.Add("OwnerDisp", System.Type.GetType("System.Int32"))
		moPrjOwnersTable.Columns.Add("OwnerIndex", System.Type.GetType("System.Int32"))

	End Sub

	Private Sub zzSetCheckBoxValue()
		Dim oCheckBoxColumn As DataGridViewCheckBoxColumn
		For Each oColumn As DataGridViewColumn In Me.dgvMain.Columns
			oCheckBoxColumn = TryCast(oColumn, DataGridViewCheckBoxColumn)
			If oCheckBoxColumn IsNot Nothing Then
				oCheckBoxColumn.FalseValue = 0
				oCheckBoxColumn.TrueValue = 1
			End If
		Next
	End Sub
	Private Sub zzCreateOwnersDic()

	End Sub
	Private Function zzCreateRepTable() As DataTable
		Dim oResTable As DataTable = New DataTable("Report")

		oResTable.Columns.Add("BlockFullName", GetType(System.String))
		oResTable.Columns.Add("ParcelName", GetType(System.String))
		oResTable.Columns.Add("LegalArea", GetType(System.Double))
		oResTable.Columns.Add("CalcInArea", GetType(System.Double))
		oResTable.Columns.Add("Owners", GetType(System.String))
		oResTable.Columns.Add("Note", GetType(System.String))
		oResTable.Columns.Add("Paragraph19Area", GetType(System.Double))
		oResTable.Columns.Add("LeasingArea", GetType(System.Double))
		Return oResTable
	End Function


	Private Function zzRowOwnerSet(oOwnershipRow As DataRow) As dmOwnerSet
		Const iFirstOwnerCol As Integer = 10

		'Const sDelim As String = " ו"

		Dim sRepDelim As String = vbCrLf ' & "ו"
		Dim bOwnerExist As Boolean
		Dim sWinOwnerText As String = Nothing
		Dim sRepOwnerText As String = Nothing

		Dim sColumnName As String
		Dim tOwner As TplnOwner = Nothing
		Dim iaAcadColors() As Short
		Dim iOwnerSetCode As Integer = 0

		Dim iOwnerIndex As Integer = 0
		Dim bParagraph19 As Boolean = DMCommon.Functions.CBoolN(oOwnershipRow.Item("Paragraph19")) '(oOwnershipRow.Item("Paragraph19"), Boolean)
		Dim bLeasing As Boolean = DMCommon.Functions.CBoolN(oOwnershipRow.Item("Leasing"))
		Dim bParagraph126 As Boolean = DMCommon.Functions.CBoolN(oOwnershipRow.Item("Paragraph126"))
		Dim bSharedHouse As Boolean = DMCommon.Functions.CBoolN(oOwnershipRow.Item("SharedHouse"))
		'Dim iPaintCode As Integer = 0
		'Dim iFactor As Integer = 1
		Dim shColorA As Short = 0S
		Dim dBaseAngle As Double = DMCommon.Functions.CDblN(oOwnershipRow.Item("PaintAngle"), 45.0)
		ReDim iaAcadColors(miOwnersCount - 1)
		Dim colColumns As DataColumnCollection = oOwnershipRow.Table.Columns
		For iPriority As Integer = 0 To 7
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!DataRowPrior", oOwnershipRow.Item(iFirstOwnerCol + iPriority), colColumns.Item(iFirstOwnerCol + iPriority).ColumnName)
			'	DMCommon.Debug.MsgBox("13_122", moOwnershipTable.Columns.Item(iFirstOwnerCol + iPriotity).ColumnName, moOwnershipTable.Columns.Item(iFirstOwnerCol + iPriotity).DataType, oRow.Item(iFirstOwnerCol + iPriotity))
			bOwnerExist = DMCommon.Functions.CBoolN(oOwnershipRow.Item(iFirstOwnerCol + iPriority))
			If bOwnerExist Then
				sColumnName = moOwnershipTable.Columns.Item(iFirstOwnerCol + iPriority).ColumnName
				If sWinOwnerText IsNot Nothing Then
					sWinOwnerText &= vbCrLf
					sRepOwnerText = vbCrLf & sRepOwnerText
				End If
				If sColumnName IsNot Nothing AndAlso moOwners.TryGetValue(sColumnName, tOwner) Then
					sWinOwnerText &= tOwner.Name
					sRepOwnerText = tOwner.Name & sRepOwnerText

					iaAcadColors(iOwnerIndex) = tOwner.AcadColor
					iOwnerSetCode += tOwner.OwnerCode

					iOwnerIndex += 1
				End If
			End If

		Next
		If Not IsDBNull(oOwnershipRow.Item("OwnerIDs")) Then
			Dim sOwnerIDs As String = DirectCast(oOwnershipRow.Item("OwnerIDs"), String)
			Dim iaOwnersID() As Integer = DMCommon.Functions.StringToIntArray(sOwnerIDs)
			For iIndex As Integer = 0 To iaOwnersID.GetUpperBound(0)
				If sWinOwnerText IsNot Nothing Then
					sWinOwnerText &= vbCrLf
					sRepOwnerText = vbCrLf & sRepOwnerText
				End If
				If moOwners.TryGetValue(iaOwnersID(iIndex), tOwner) Then
					sWinOwnerText &= tOwner.Name
					sRepOwnerText = tOwner.Name & sRepOwnerText

					iaAcadColors(iOwnerIndex) = tOwner.AcadColor
					iOwnerSetCode += tOwner.OwnerCode

					iOwnerIndex += 1
				End If
			Next
		End If

		If miPaintingType = enPaintingType.Solid AndAlso iOwnerIndex > 1 AndAlso mdicColorsA IsNot Nothing Then
			If mdicColorsA.TryGetValue(iOwnerSetCode, shColorA) Then
				iaAcadColors(0) = shColorA
				iOwnerIndex = 1
			Else
				Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oOwnershipRow.Item("BlockNo"))
				Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(oOwnershipRow.Item("BlockAddNo"))
				Dim iParcelNo As Integer = DMCommon.Functions.CIntN(oOwnershipRow.Item("ParcelNo"))
				Dim sBlock As String = CStr(iBlockNo)
				If iBlockAddNo <> 0 Then
					sBlock &= "/" & CStr(iBlockAddNo)
				End If
				DMCommon.Debug.MsgBox("Err #329", "Code '" & iOwnerSetCode.ToString() & "' was not found", "גוש " & sBlock, "חלקה " & CStr(iParcelNo))
			End If



		End If
		Return New dmOwnerSet(sWinOwnerText, sRepOwnerText, iaAcadColors, iOwnerSetCode, iOwnerIndex, bParagraph19, bLeasing, bParagraph126, bSharedHouse, dBaseAngle)
	End Function
	Private Sub zzGetOwners()
		Dim sSQLCom As String = "SELECT TOP (100) PERCENT OwnerID,FieldName,OwnerName,AcadColor,Code,MAPIMandatory,OwnerShortName FROM dbo.OwnersList  ORDER BY MAPIOrder"
		ReDim msaOwnerNames(miMaxOwnerUB)

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSQLCom, CommandType.Text)
		Dim tOwner As TplnOwner

		moOwners = New dmOwners()

		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read

					tOwner = New TplnOwner(oDataReader)
					moOwners.Add(tOwner)

					miOwnersCount += 1
				Loop
			End If

			oDataReader.Close()
		End If


	End Sub
	Private Sub zzGetOwnerNames()
		Dim sSQLCom As String = "SELECT TOP (100) PERCENT OwnerID,FieldName, OwnerName,AcadColor,Code FROM dbo.OwnersList  ORDER BY MAPIOrder"
		ReDim msaOwnerNames(miMaxOwnerUB)

		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSQLCom, CommandType.Text)
		Dim tOwner As TplnOwner
		Dim iID As Integer
		Dim sFieldName As String
		Dim sOwnerName As String
		Dim iAcadColor As Integer
		Dim iOwnerCode As Integer
		moOwners = New dmOwners()

		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read
					iID = oDataReader.GetInt32(0)
					If oDataReader.IsDBNull(1) Then
						sFieldName = Nothing
					Else
						sFieldName = oDataReader.GetString(1)
					End If

					sOwnerName = oDataReader.GetString(2)
					iAcadColor = oDataReader.GetInt32(3)
					iOwnerCode = oDataReader.GetInt32(4)
					'	tOwner = New dmOwner(iID, sFieldName, sOwnerName, iAcadColor, iOwnerCode)
					tOwner = New TplnOwner(oDataReader)
					moOwners.Add(iID, tOwner)
					If Not String.IsNullOrEmpty(sFieldName) Then
						moOwners.Add(sFieldName, tOwner)
						'	mhsOwnersWithField.Add(iID)
					End If

					miOwnersCount += 1
				Loop
			End If

			oDataReader.Close()
		End If


	End Sub
	Private Sub zzInitUD_Project()
		'TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		'TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub

	Private Sub zzLoadOwnershipTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetOwnership"

		moOwnershipTable = New System.Data.DataTable("Ownership")
		moOwnershipDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)

		If bSchemaOnly Then
			moOwnershipDataAdapter.FillSchema(moOwnershipTable, SchemaType.Source)
		Else
			moOwnershipDataAdapter.Fill(moOwnershipTable)
		End If
		'	DMCommon.Debug.ExcelLog.SetDataTable(0, "OwnershipTable", moOwnershipTable)

	End Sub
	Private Sub zzLoadPrjOwnersTable(bSchemaOnly As Boolean)
		'  Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		'  Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

		Const sSPName As String = "GetPrjOwners"

		moPrjOwnersTable = New System.Data.DataTable("PrjOwners")
		moPrjOwnersDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSPName, CommandType.StoredProcedure, zzGetParameters(), True, String.Empty, True, True)
		'   moOwnershipDataAdapter.FillLoadOption
		If bSchemaOnly Then
			moPrjOwnersDataAdapter.FillSchema(moPrjOwnersTable, SchemaType.Source)
		Else
			moPrjOwnersDataAdapter.Fill(moPrjOwnersTable)
		End If
		'   moOwnershipTable.Columns.Add("AcObjID", GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))

		'DMCommon.Debug.MsgBox("13_122i", moPrjOwnersTable.Rows.Count)
	End Sub

	Private Sub zzLoadParcels(ByVal iOptions As TopoManager.TPlanGraph.enDataOptions)
		Dim saPrimaryColumnNames() As String = {"ProjectCode", "Detail", "BlockNo", "BlockAddNo", "ParcelNo"}
		Dim oPrimaryKey(saPrimaryColumnNames.GetUpperBound(0)) As DataColumn
		'Dim oParcelTable As System.Data.DataTable = TopoManager.TPlanGraph.TplnParcel.MainDataTable
		'	Dim oParcelView As System.Data.DataView = New DataView(oParcelTable, String.Empty, "BlockNo,BlockAddNo,ParcelNo", DataViewRowState.Unchanged)
		Dim dicParcels As TopoManager.TPlanGraph.TplnParcels = TopoManager.TPlanGraph.TplnProject.Parcels
		'Dim sBlockFullName As String
		'Dim sParcelName As String
		'Dim dLegalArea As Double
		'Dim dInPlanArea As Double

		Dim dParagraph19Area As Double = 0.0
		Dim dLeasingArea As Double = 0.0
		Dim dOverlayPrg5LeasArea As Double = 0.0
		'	Dim iParcelTopoID As Integer



		'Dim bIsAnalitic As Boolean
		'Dim iBlock As Integer, iBlockAdd As Integer
		'Dim iParcelNo As Integer
		Dim oOwnershipRow As DataRow
		Dim oaKeyValue() As System.Object = {miProjectCode, miDetailNo, 0, 0, 0}
		Dim tAreaSet As TopoManager.TPlanGraph.TplnAreaSet
		Dim dParcelArea As Double
		Dim iRowIndex As Integer = 0
		For iIndex As Integer = 0 To saPrimaryColumnNames.GetUpperBound(0)
			oPrimaryKey(iIndex) = moOwnershipTable.Columns.Item(saPrimaryColumnNames(iIndex))
		Next
		mdicDataRows.Clear()
		moOwnershipTable.PrimaryKey = oPrimaryKey
		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In dicParcels.Values
			'ApprFDO_Overlay = 4 GetOverlayPgons

			If (oParcel.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = TopoManager.NumerationPair.enComplexType.Entire) OrElse (oParcel.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = TopoManager.NumerationPair.enComplexType.Partial) Then
				oaKeyValue(2) = oParcel.BlockNo
				oaKeyValue(3) = oParcel.BlockAdd
				oaKeyValue(4) = oParcel.ParcelNo

				oOwnershipRow = moOwnershipTable.Rows.Find(oaKeyValue)
				If oOwnershipRow Is Nothing Then
					oOwnershipRow = moOwnershipTable.NewRow()
					For iIndex As Integer = 0 To saPrimaryColumnNames.GetUpperBound(0)
						oOwnershipRow.Item(saPrimaryColumnNames(iIndex)) = oaKeyValue(iIndex)
					Next
					oOwnershipRow.Item("DevelAuthority") = 0
					oOwnershipRow.Item("KKL") = 0
					oOwnershipRow.Item("IsrState") = 0
					oOwnershipRow.Item("LocalAuthority") = 0
					oOwnershipRow.Item("Private") = 0
					oOwnershipRow.Item("KKLAdd") = 0
					oOwnershipRow.Item("Regulation") = 0
					oOwnershipRow.Item("IsNotRegulated") = 0
					oOwnershipRow.Item("SharedHouse") = 0
					oOwnershipRow.Item("IsActive") = 0
					oOwnershipRow.Item("Foreclosure") = 0
					oOwnershipRow.Item("Verdict") = 0
					oOwnershipRow.Item("AntiqueSite") = 0
					oOwnershipRow.Item("Mortgage") = 0
					oOwnershipRow.Item("Paragraph5") = 0
					oOwnershipRow.Item("Paragraph7") = 0
					oOwnershipRow.Item("Paragraph19") = 0
					oOwnershipRow.Item("Paragraph126") = 0
					oOwnershipRow.Item("Leasing") = 0
					oOwnershipRow.Item("NoteEdited") = 0
					oOwnershipRow.Item("TreasurerNote") = 0
					oOwnershipRow.Item("AntiqueSite") = 0
					oOwnershipRow.Item("Paragraph123") = 0
					oOwnershipRow.Item("Regulation29") = 0
					oOwnershipRow.Item("RoadOrdinance") = 0
					oOwnershipRow.Item("DemolitionOrder") = 0
					oOwnershipRow.Item("Paragraph11a") = 0


					'oOwnershipRow.Item("Note")
				End If

				oOwnershipRow.Item("IsAnalitic") = oParcel.IsAnalytic
				oOwnershipRow.Item("ParcelLegalArea") = oParcel.LegalArea(False)
				tAreaSet = oParcel.InPlanAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				Select Case iOptions
					Case TopoManager.TPlanGraph.enDataOptions.AcadArea
						dParcelArea = tAreaSet.AcadArea
					Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea
						dParcelArea = tAreaSet.CalcArea
					Case TopoManager.TPlanGraph.enDataOptions.CalcMergeArea2
						dParcelArea = tAreaSet.CalcArea2
					Case TopoManager.TPlanGraph.enDataOptions.RoundedArea
						dParcelArea = tAreaSet.RoundedArea
					Case TopoManager.TPlanGraph.enDataOptions.CalcRoundedArea
						If oParcel.IsAnalytic Then
							dParcelArea = tAreaSet.CalcArea
						Else
							dParcelArea = tAreaSet.RoundedArea
						End If
				End Select

				oOwnershipRow.Item("ParcelInLegalArea") = dParcelArea
				oParcel.GetOwnershipNoteArea(dParagraph19Area, dLeasingArea, dOverlayPrg5LeasArea)

				If DMCommon.Functions.CBoolN(oOwnershipRow.Item("Paragraph19")) Then
					oOwnershipRow.Item("Paragraph19Area") = oOwnershipRow.Item("ParcelInLegalArea")
				Else
					'oOwnershipRow.Item("Paragraph19Area") = Math.Round(dParagraph19Area * dParcelArea * 0.001 / tAreaSet.AcadArea, 3)
					oOwnershipRow.Item("Paragraph19Area") = Math.Round(dParagraph19Area * dParcelArea / tAreaSet.AcadArea, 3)

				End If

				If DMCommon.Functions.CBoolN(oOwnershipRow.Item("Leasing")) Then
					oOwnershipRow.Item("LeasingArea") = oOwnershipRow.Item("ParcelInLegalArea")
				Else
					oOwnershipRow.Item("LeasingArea") = Math.Round(dLeasingArea * dParcelArea * 0.001 / tAreaSet.AcadArea, 3)
				End If
				If DMCommon.Functions.CBoolN(oOwnershipRow.Item("Paragraph19")) AndAlso DMCommon.Functions.CBoolN(oOwnershipRow.Item("Leasing")) Then

					oOwnershipRow.Item("OverlayPrg5LeasArea") = oOwnershipRow.Item("ParcelInLegalArea")
				ElseIf DMCommon.Functions.CBoolN(oOwnershipRow.Item("Paragraph19")) Then

					oOwnershipRow.Item("OverlayPrg5LeasArea") = Math.Round(dLeasingArea * dParcelArea / tAreaSet.AcadArea, 3)

				ElseIf DMCommon.Functions.CBoolN(oOwnershipRow.Item("Leasing")) Then
					oOwnershipRow.Item("OverlayPrg5LeasArea") = Math.Round(dParagraph19Area * dParcelArea / tAreaSet.AcadArea, 3)
				Else
					oOwnershipRow.Item("OverlayPrg5LeasArea") = Math.Round(dOverlayPrg5LeasArea * dParcelArea / tAreaSet.AcadArea, 3)
				End If
				oOwnershipRow.Item("ParcelTopoID") = oParcel.TopoID
				oOwnershipRow.Item("IsActive") = 1
				mdicDataRows.Add(oParcel.TopoID, iRowIndex)
				iRowIndex += 1
				If oOwnershipRow.RowState = DataRowState.Detached Then
					moOwnershipTable.Rows.Add(oOwnershipRow)
				End If
			End If

		Next



		For Each oDataRow As DataRow In moOwnershipTable.Rows
			If oDataRow.RowState = DataRowState.Unchanged Then
				oDataRow.Item("IsActive") = 0

			End If
			'oDataRow.Item("Status") = 0
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "OwnT", oDataRow.Item("BlockNo"), oDataRow.Item("BlockAddNo"), oDataRow.Item("ParcelNo"), oDataRow.Item("IsActive"), oDataRow.RowState, CInt(oDataRow.RowState))
		Next

	End Sub


	Private Sub zzMyInitializeComponent()
		Me.dgvMain.AutoGenerateColumns = False
		Me.cchRegulation.ToolTipText = msRegulationName
		moDataGridViewCellStyleError = Me.dgvMain.DefaultCellStyle.Clone
		moDataGridViewCellStyleError.BackColor = Color.Red

		With cmbPaintAngle
			.AutoSize = False
			''	.Size = New System.Drawing.Size(48, 16)
			'	.Location = New System.Drawing.Point(88, 18)

			For iAngle As Integer = 0 To 165 Step 15
				.Items.Add(iAngle)
			Next
			.SelectedItem = miPaintAngleDflt
		End With

		If System.Environment.UserName.ToUpper = "BORIS" Then
			Me.dgvMain.Columns.Item("ctxOwnerIDs").Visible = True
		End If
		Dim oMySettings As My.MySettings = New My.MySettings()
		Me.chkUseBalance.Checked = oMySettings.UseBalance
	End Sub
	Private Sub zzFillStatusCombo()

		Dim saStatusValues() As String = {"כן", "אא", "בב", "גג", "דד"}
		'Dim oItemData As DMCommon.ItemData
		Dim oInitItemData As DMCommon.ItemData = Nothing

		'With Me.ccbStatus
		'	.ValueMember = DMCommon.ItemData.ValueMember
		'	.DisplayMember = DMCommon.ItemData.DisplayMember
		'	For iIndex As Integer = 0 To saStatusValues.GetUpperBound(0)
		'		oItemData = New DMCommon.ItemData(iIndex, saStatusValues(iIndex))
		'		.Items.Add(oItemData)
		'	Next
		'End With
	End Sub
	Private Function zzGetParameters() As System.Data.Common.DbParameter()
		Dim oaParams(1) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing

		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)


		'oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prOriginalBlockAddNo", DbType.Int32, miBlockAddNo)
		Return oaParams
	End Function
	Private Sub zzSetDataSource()
		If Me.chkAllParcels.Checked Then
			Me.dgvMain.DataSource = moOwnershipTable

			'DMCommon.Debug.MsgBox("13_122L", moOwnershipTable.Rows.Count)
		Else
			moParcelOwnershipDataView = New DataView(moOwnershipTable, "IsActive=1", "BlockNo,BlockADdNo,ParcelNo", DataViewRowState.ModifiedCurrent Or DataViewRowState.Added)  ' 
			Me.dgvMain.DataSource = moParcelOwnershipDataView
			'DMCommon.Debug.MsgBox("13_122O", moParcelOwnershipDataView.Count, moOwnershipTable.Rows.Count)
		End If

	End Sub
	Private Function zzGetDataRow(iRowIndex As Integer) As DataRow
		If Me.chkAllParcels.Checked Then
			Return moOwnershipTable.Rows.Item(iRowIndex)
			'DMCommon.Debug.MsgBox("13_122L", moOwnershipTable.Rows.Count)
		Else
			Return moParcelOwnershipDataView.Item(iRowIndex).Row
			'DMCommon.Debug.MsgBox("13_122O", moParcelOwnershipDataView.Count, moOwnershipTable.Rows.Count)
		End If

	End Function

	Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
		Dim iResOwnership As Integer
		Dim iResPrjOwners As Integer
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim iGridRowIndex As Integer = oGridRow.Index

		If moOwnershipTable IsNot Nothing Then
			For Each oRow As DataRow In moOwnershipTable.Rows
				'''''''''''''oRow.Item("ParcelInLegalArea") = 0.0

				oRow.EndEdit()
			Next

			iResOwnership = moOwnershipDataAdapter.Update(moOwnershipTable)
		End If
		zzLoadParcels(TopoManager.TPlanGraph.enDataOptions.CalcMergeArea)
		If moPrjOwnersTable IsNot Nothing Then
			For Each oRow As DataRow In moPrjOwnersTable.Rows
				'oRow.EndEdit()
			Next

			iResPrjOwners = moPrjOwnersDataAdapter.Update(moPrjOwnersTable)
		End If
		zzSetCurrentRow(iGridRowIndex)
		'	oGridRow = Me.dgvMain.Rows.Item(iGridRowIndex)
		'Dim oCell As DataGridViewCell = oGridRow.Cells.Item(0)
		'Me.dgvMain.CurrentCell = oCell


		mbDirty = False

		'	DMCommon.Debug.MsgBox("13_122c", iResOwnership, moOwnershipTable.Rows.Count, iResPrjOwners, moPrjOwnersTable.Rows.Count)
	End Sub
	Private Sub zzSetCurrentRow(iGridRowIndex As Integer)
		Dim oGridRow As DataGridViewRow
		oGridRow = Me.dgvMain.Rows.Item(iGridRowIndex)
		Dim oCell As DataGridViewCell = oGridRow.Cells.Item(0)
		Me.dgvMain.CurrentCell = oCell
	End Sub



	Private Sub frmOwnership_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If False AndAlso e.CloseReason = CloseReason.UserClosing Then
			Me.Hide()
			e.Cancel = True
			RaiseEvent FormHided()
		End If

		Dim oMySettings As My.MySettings = New My.MySettings()
		If Me.chkUseBalance.Checked <> oMySettings.UseBalance Then
			oMySettings.UseBalance = Me.chkUseBalance.Checked
			oMySettings.Save()
		End If
	End Sub
	Private Sub zzPaintAllParcelsPlusHatch(bPaint As Boolean)
		Const sParagraph19Text As String = "סעיף 19"
		Const sLeasingText As String = "חכירה"
		Const sParagraph126Text As String = "סעיף 126"
		Const sSharedHouseText As String = "בית משותף"
		Const iParagraph19ID As Integer = 101
		Const iLeasingID As Integer = 102
		Const iParagraph126ID As Integer = 103
		Const iSharedHouseID As Integer = 104

		Dim tPaintLayerDef As DMAcadExt.AcadLayerDef
		Dim tParagraph19LayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef()
		Dim tLeasingLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef()
		Dim tParagraph126LayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef()
		Dim tSharedHouseLayerDef As DMAcadExt.AcadLayerDef = New DMAcadExt.AcadLayerDef()


		Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		'	Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
		Dim dicParcels As TopoManager.TPlanGraph.TplnParcels = TopoManager.TPlanGraph.TplnProject.Parcels
		Dim sLanduseNotFoundList As String = String.Empty
		Dim sPaintLayer As String = String.Empty
		Dim bCurrentLayerOK As Boolean
		Dim bParagraph19LayerOK As Boolean
		Dim bLeasingLayerOK As Boolean
		Dim bParagraph126LayerOK As Boolean
		Dim bSharedHouseLayerOK As Boolean


		Dim oParcel As TopoManager.TPlanGraph.TplnParcel
		Dim iTopoID As Integer
		Dim tZebra As DMAcadExt.ColorZebra
		Dim tOwnerSet As dmOwnerSet = New dmOwnerSet()
		Dim dZebraWidth As Double = 2.0
		Dim tColorScheme As DMAcadExt.ColorScheme
		Dim tParagraph19ColorScheme As DMAcadExt.ColorScheme
		Dim tLeasingColorScheme As DMAcadExt.ColorScheme
		Dim tParagraph126ColorScheme As DMAcadExt.ColorScheme
		Dim tSharedHouseColorScheme As DMAcadExt.ColorScheme
		Dim tLegendColorScheme As DMAcadExt.ColorScheme
		Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
		Dim tDMHatch As DMAcadExt.DMHatch

		If bPaint Then
			tPaintLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Parcels, 0)
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tPaintLayerDef, True, False, False, False)
			tParagraph19LayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 1)
			tLeasingLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 2)
			tParagraph126LayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 3)
			tSharedHouseLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 4)

		Else
			tPaintLayerDef = New DMAcadExt.AcadLayerDef()
		End If
		DMCommon.Debug.ExcelLog.SetDataTable(0, "OwnershipT_Paint", moOwnershipTable)

		mdicLegendColoSchemes = New Dictionary(Of Integer, DMAcadExt.ColorScheme)()
		'Dim oAcadColor As Autodesk.AutoCAD.Colors.Color
		For Each oDataRow As DataRow In moOwnershipTable.Rows
			iTopoID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID"))
			If iTopoID <> 0 Then
				oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iTopoID)

				'	oAcadColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, shAcadColorIndex)


				If oParcel IsNot Nothing Then
					tOwnerSet = zzRowOwnerSet(oDataRow)
					If tOwnerSet.Count <> 0 Then
						tColorScheme = New DMAcadExt.ColorScheme(dScale)
						'DMCommon.Debug.MsgBox("13_125f", tOwnerSet.RepText, tOwnerSet.OwnerCode, tOwnerSet.WinText)

						tLegendColorScheme = New DMAcadExt.ColorScheme(dScale)
						tLegendColorScheme.Name = tOwnerSet.RepText
						If tOwnerSet.Count = 1 Then
							If False AndAlso tOwnerSet.Leasing Then
								tDMHatch = New DMAcadExt.DMHatch("ANSI31")
								tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
								tDMHatch.PatternScale = 2.0
								tDMHatch.Angle = New DMAcadExt.LineAngle(90, False)
							Else
								tDMHatch = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(tOwnerSet.AcadColors(0)))
								tLegendColorScheme.Hatch = tDMHatch
							End If
							tColorScheme.Hatch = tDMHatch

						ElseIf tOwnerSet.Count > 1 Then
							tZebra = New DMAcadExt.ColorZebra(tOwnerSet.Count)
							'New DMAcadExt.DMColor(tOwnerSet.AcadColors(iStripIndex)), dZebraWidth
							tZebra.Angle = New DMAcadExt.LineAngle(tOwnerSet.BaseAngle, False)
							For iStripIndex As Integer = 0 To tOwnerSet.Count - 1
								tZebra.Strip(iStripIndex) = New DMAcadExt.ColorStrip(New DMAcadExt.DMColor(tOwnerSet.AcadColors(iStripIndex)), dZebraWidth)
							Next
							tColorScheme.Zebra = tZebra
							tLegendColorScheme.Zebra = tZebra
							If False AndAlso tOwnerSet.Leasing Then
								tDMHatch = New DMAcadExt.DMHatch("ANSI31")
								tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
								tDMHatch.PatternScale = 20.0
								tDMHatch.Scale = dScale
								tColorScheme.Hatch = tDMHatch
							End If

							'	DMCommon.Debug.MsgBox("13_125f", tColorScheme.IsInstance, tZebra.StripUB, tZebra.Strip(0).Color.AcadColorIndex, tZebra.Strip(0).Width, tZebra.Strip(0).Scale, tZebra.Strip(0).ScalingWidth, tZebra.Strip(1).Color.AcadColorIndex, tZebra.Strip(1).Width, tZebra.Strip(1).Scale, tZebra.Strip(1).ScalingWidth)
						End If
						If tOwnerSet.Paragraph19 Then
							tDMHatch = New DMAcadExt.DMHatch("ANSI37")
							'tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
							tDMHatch.PatternScale = 2.0
							tDMHatch.Angle = New DMAcadExt.LineAngle(tOwnerSet.BaseAngle, False)
							tParagraph19ColorScheme = New DMAcadExt.ColorScheme(dScale)
							tParagraph19ColorScheme.Hatch = tDMHatch
							If tParagraph19ColorScheme.IsInstance Then
								If Not mdicLegendColoSchemes.ContainsKey(iParagraph19ID) Then
									tParagraph19ColorScheme.Name = sParagraph19Text
									mdicLegendColoSchemes.Add(iParagraph19ID, tParagraph19ColorScheme)
								End If
							End If
						Else
							tParagraph19ColorScheme = New DMAcadExt.ColorScheme()

						End If
						If tOwnerSet.Leasing Then
							tDMHatch = New DMAcadExt.DMHatch("ANSI31")
							'tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
							tDMHatch.PatternScale = 0.7
							tDMHatch.Angle = New DMAcadExt.LineAngle(tOwnerSet.Angle90, False)
							tDMHatch.PatternColor = New DMAcadExt.DMColor(150S)
							tLeasingColorScheme = New DMAcadExt.ColorScheme(dScale)
							tLeasingColorScheme.Hatch = tDMHatch

							If tLeasingColorScheme.IsInstance Then
								If Not mdicLegendColoSchemes.ContainsKey(iLeasingID) Then
									tLeasingColorScheme.Name = sLeasingText
									mdicLegendColoSchemes.Add(iLeasingID, tLeasingColorScheme)
								End If
							End If

						Else
							tLeasingColorScheme = New DMAcadExt.ColorScheme()
						End If
						If tOwnerSet.Paragraph126 Then
							tDMHatch = New DMAcadExt.DMHatch("ANSI31")
							'tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
							tDMHatch.PatternScale = 2.0
							tDMHatch.Angle = New DMAcadExt.LineAngle(tOwnerSet.Angle90, False)
							tParagraph126ColorScheme = New DMAcadExt.ColorScheme(dScale)
							tParagraph126ColorScheme.Hatch = tDMHatch

							If tParagraph126ColorScheme.IsInstance Then
								If Not mdicLegendColoSchemes.ContainsKey(iParagraph126ID) Then
									tParagraph126ColorScheme.Name = sParagraph126Text
									mdicLegendColoSchemes.Add(iParagraph126ID, tParagraph126ColorScheme)
								End If
							End If

						Else
							tParagraph126ColorScheme = New DMAcadExt.ColorScheme()

						End If

						If tOwnerSet.SharedHouse Then
							tDMHatch = New DMAcadExt.DMHatch("ANSI37")
							'tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
							tDMHatch.PatternScale = 2.0
							tDMHatch.Angle = New DMAcadExt.LineAngle(tOwnerSet.Angle90, False)
							tDMHatch.PatternColor = New DMAcadExt.DMColor(200S)
							tSharedHouseColorScheme = New DMAcadExt.ColorScheme(dScale)
							tSharedHouseColorScheme.Hatch = tDMHatch

							'''''''''''''''''''''''''''''
							If tSharedHouseColorScheme.IsInstance Then
								If Not mdicLegendColoSchemes.ContainsKey(iSharedHouseID) Then
									tSharedHouseColorScheme.Name = sSharedHouseText
									mdicLegendColoSchemes.Add(iSharedHouseID, tSharedHouseColorScheme)
								End If
							End If


						Else
							tSharedHouseColorScheme = New DMAcadExt.ColorScheme()

						End If
						'oParcel.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
						If bPaint Then
							If tColorScheme.IsInstance Then
								bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tPaintLayerDef, True, False, False, False)
								If bCurrentLayerOK Then
									oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True, tPaintLayerDef.Name)
								End If


								'	tLeasingColorScheme
							End If
							If tOwnerSet.Paragraph19 AndAlso tParagraph19ColorScheme.IsInstance Then
								bParagraph19LayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tParagraph19LayerDef, True, False, False, False)
								If bParagraph19LayerOK Then
									oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tParagraph19ColorScheme, True, tParagraph19LayerDef.Name)
								End If

							End If

							If tOwnerSet.Leasing AndAlso tLeasingColorScheme.IsInstance Then
								bLeasingLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tLeasingLayerDef, True, False, False, False)
								If bLeasingLayerOK Then
									oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tLeasingColorScheme, True, tLeasingLayerDef.Name)
								End If

							End If

							If tOwnerSet.Paragraph126 AndAlso tParagraph126ColorScheme.IsInstance Then
								bParagraph126LayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tParagraph126LayerDef, True, False, False, False)
								If bParagraph126LayerOK Then
									oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tParagraph126ColorScheme, True, tParagraph126LayerDef.Name)
								End If
							End If

							If tOwnerSet.SharedHouse AndAlso tSharedHouseColorScheme.IsInstance Then
								bSharedHouseLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tSharedHouseLayerDef, True, False, False, False)
								If bSharedHouseLayerOK Then
									oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tSharedHouseColorScheme, True, tSharedHouseLayerDef.Name)
								End If
							End If
						End If 'bPaint
						If tLegendColorScheme.IsInstance Then
							If Not mdicLegendColoSchemes.ContainsKey(tOwnerSet.OwnerCode) Then
								mdicLegendColoSchemes.Add(tOwnerSet.OwnerCode, tLegendColorScheme)
							End If
						End If
					End If
				End If
			End If
		Next oDataRow
		Dim sOwnNotesTopoName As String = "OwnNotesLine"
		Dim oOwnNotesTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sOwnNotesTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim colOwnNotesPgons As Autodesk.Gis.Map.Topology.PolygonCollection
		Dim oColorPgon As TopoManager.ColorPolygon
		If oOwnNotesTopology IsNot Nothing Then
			colOwnNotesPgons = oOwnNotesTopology.GetPolygons()
			tDMHatch = New DMAcadExt.DMHatch("ANSI37")
			'tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
			tDMHatch.PatternScale = 2.0
			tDMHatch.Angle = New DMAcadExt.LineAngle(tOwnerSet.Angle90, False)
			tParagraph19ColorScheme = New DMAcadExt.ColorScheme(dScale)
			tParagraph19ColorScheme.Hatch = tDMHatch
			If tParagraph19ColorScheme.IsInstance Then
				If Not mdicLegendColoSchemes.ContainsKey(iParagraph19ID) Then
					tParagraph19ColorScheme.Name = sParagraph19Text
					mdicLegendColoSchemes.Add(iParagraph19ID, tParagraph19ColorScheme)
				End If
			End If

			bParagraph19LayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tParagraph19LayerDef, True, False, False, False)
			If bParagraph19LayerOK Then

				For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colOwnNotesPgons
					oColorPgon = New TopoManager.ColorPolygon(oPolygon)
					oColorPgon.Paint(DMAcadExt.PaintMethod.Hatch, tParagraph19ColorScheme, True, tParagraph19LayerDef.Name)

				Next
			End If

		End If






	End Sub
	Private Sub zzPaintAllParcels_AAA(bPaint As Boolean)
		Dim tPaintLayerDef As DMAcadExt.AcadLayerDef
		Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
		'	Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, iMapThemeID)
		Dim dicParcels As TopoManager.TPlanGraph.TplnParcels = TopoManager.TPlanGraph.TplnProject.Parcels
		Dim sLanduseNotFoundList As String = String.Empty
		Dim sPaintLayer As String = String.Empty


		Dim oParcel As TopoManager.TPlanGraph.TplnParcel
		Dim iTopoID As Integer
		Dim tZebra As DMAcadExt.ColorZebra
		Dim tOwnerSet As dmOwnerSet
		Dim dZebraWidth As Double = 2.0
		Dim tColorScheme As DMAcadExt.ColorScheme
		Dim tLegendColorScheme As DMAcadExt.ColorScheme
		Dim dPaintAngle As Double = CDbl(miPaintAngleDflt)
		Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
		Dim tDMHatch As DMAcadExt.DMHatch

		If bPaint Then
			tPaintLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Parcels)

		End If
		mdicLegendColoSchemes = New Dictionary(Of Integer, DMAcadExt.ColorScheme)()
		'Dim oAcadColor As Autodesk.AutoCAD.Colors.Color
		For Each oDataRow As DataRow In moOwnershipTable.Rows
			iTopoID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID"))
			If iTopoID <> 0 Then
				oParcel = TopoManager.TPlanGraph.TplnProject.GetParcel(iTopoID)

				'	oAcadColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, shAcadColorIndex)


				If oParcel IsNot Nothing Then
					tOwnerSet = zzRowOwnerSet(oDataRow)
					If tOwnerSet.Count <> 0 Then
						tColorScheme = New DMAcadExt.ColorScheme(dScale)

						tLegendColorScheme = New DMAcadExt.ColorScheme(dScale)
						tLegendColorScheme.Name = tOwnerSet.RepText
						If tOwnerSet.Count = 1 Then
							If tOwnerSet.Leasing Then
								tDMHatch = New DMAcadExt.DMHatch("ANSI31")
								tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
								tDMHatch.PatternScale = 2.0
								tDMHatch.Angle = New DMAcadExt.LineAngle(90, False)
							Else
								tDMHatch = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(tOwnerSet.AcadColors(0)))
								tLegendColorScheme.Hatch = tDMHatch
							End If
							tColorScheme.Hatch = tDMHatch

						ElseIf tOwnerSet.Count > 1 Then
							tZebra = New DMAcadExt.ColorZebra(tOwnerSet.Count)
							'New DMAcadExt.DMColor(tOwnerSet.AcadColors(iStripIndex)), dZebraWidth
							tZebra.Angle = New DMAcadExt.LineAngle(dPaintAngle, False)
							For iStripIndex As Integer = 0 To tOwnerSet.Count - 1
								tZebra.Strip(iStripIndex) = New DMAcadExt.ColorStrip(New DMAcadExt.DMColor(tOwnerSet.AcadColors(iStripIndex)), dZebraWidth)
							Next
							tColorScheme.Zebra = tZebra
							tLegendColorScheme.Zebra = tZebra
							If tOwnerSet.Leasing Then
								tDMHatch = New DMAcadExt.DMHatch("ANSI31")
								tDMHatch.BackColor = New DMAcadExt.DMColor(tOwnerSet.AcadColors(0))
								tDMHatch.PatternScale = 2.0
								tColorScheme.Hatch = tDMHatch
							End If

						End If
						If tColorScheme.IsInstance Then

							'oParcel.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
							If bPaint Then

								oParcel.PaintInPlan(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
							End If

							If Not mdicLegendColoSchemes.ContainsKey(tOwnerSet.OwnerCode) Then
								mdicLegendColoSchemes.Add(tOwnerSet.OwnerCode, tLegendColorScheme)
							End If
						End If
					End If

				End If


			End If

		Next



		DMCommon.Debug.MsgBox("13_140", mdicLegendColoSchemes.Count)
		'	Me.prbPaint.Maximum = dicParcels.Count
		'	Me.prbPaint.Step = 1


		'tColorScheme = mdicColorSchemes.Item(oLot.LanduseID)
		'MessageBox.Show(CStr(mdicColorSchemes.Count) & vbCrLf & CStr(oLot.LanduseID) & ";" & CStr(oLot.Name), "04_466")

		'  DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True, oParcel.Owner)







		'prbPaint.Increment(1)


		'	MessageBox.Show(iPaintException.ToString(), "01_780")


	End Sub
	Private Sub zzFillPaintScale()
		Const sComText As String = "SELECT ID,Name FROM Scales"
		Me.cmbPaintScale.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "Scales")
		Me.cmbPaintScale.SelectedIndex = 4
	End Sub
	Private Function zzGetSelectedScale() As Double
		Dim sScale As String

		Dim dScale As Double = 0.0
		Dim oDyn As System.Object
		If Me.cmbPaintScale.SelectedIndex >= 0 Then
			oDyn = Me.cmbPaintScale.SelectedItem
			If oDyn IsNot Nothing Then
				sScale = Me.cmbPaintScale.GetItemText(oDyn)
				dScale = DMCommon.Functions.TextToScale(sScale, mdBaseScale)
			End If
		Else
			MessageBox.Show(Me.cmbPaintScale.SelectedIndex.ToString(), "16_711")
		End If
		Return dScale
	End Function

	Private Sub cmdPaintParcels_Click(oSender As System.Object, e As EventArgs) Handles cmdPaintParcels.Click
		Me.Cursor = Cursors.WaitCursor

		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		DMAcadExt.AcadTransaction.SaveCurrentLayer()
		zzPaintAllParcelsPlusHatch(True)
		DMAcadExt.AcadTransaction.RestoreCurrentLayer()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()




		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default


	End Sub



	Private Sub cmdRefreshData_Click(oSender As System.Object, e As EventArgs) Handles cmdRefreshData.Click
		Dim oGridCell As DataGridViewCell = Me.dgvMain.CurrentCell
		Dim iGridRowIndex As Integer = oGridCell.RowIndex
		Dim iGridColumnIndex As Integer = oGridCell.ColumnIndex

		zzLoadOwnershipTable(False)
		zzLoadParcels(TopoManager.TPlanGraph.enDataOptions.CalcMergeArea)
		zzSetDataSource()
		zzCalcCheckData()

		If iGridRowIndex >= 0 AndAlso iGridRowIndex < Me.dgvMain.Rows.Count AndAlso iGridColumnIndex >= 0 Then
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iGridRowIndex)
			Dim oCell As DataGridViewCell = oGridRow.Cells.Item(iGridColumnIndex)
			Me.dgvMain.CurrentCell = oCell
		End If
	End Sub

	Private Sub chkAllParcels_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkAllParcels.CheckedChanged
		zzSetDataSource()
		zzCalcCheckData()
	End Sub

	Private Sub cmdClearPaint_Click(oSender As System.Object, e As EventArgs) Handles cmdClearPaint.Click
		Dim sPaintLayer As String = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef().Name
		Dim tParagraph19LayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 1)
		Dim tLeasingLayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 2)
		Dim tParagraph126LayerDef As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintLayerDef(DMAcadExt.enMapTheme.Ownership, 3)

		Me.Cursor = Cursors.WaitCursor
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
		TopoManager.TPlanGraph.TplnProject.ClearPaintNew(DMAcadExt.enMapTheme.Parcels)  ', iTopoPurpose
		DMAcadExt.AcadTransaction.ClearLayerList(sPaintLayer)

		DMAcadExt.AcadTransaction.ClearLayers(tParagraph19LayerDef)
		DMAcadExt.AcadTransaction.ClearLayers(tLeasingLayerDef)
		DMAcadExt.AcadTransaction.ClearLayers(tParagraph126LayerDef)


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
		Me.Cursor = Cursors.Default
	End Sub

	Private Sub frmOwnership_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
		If midgvMainLocationY <> 0 Then
			Try
				Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmOwnership_Resize")
			End Try
		End If
	End Sub

	Private Sub cmdOpenEditor_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenEditor.Click

		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		zzOpenEditorOwners(oGridRow)
	End Sub
	Private Sub zzShowOwnerColumns(bVisible As Boolean)
		Dim oPrevColumn As DataGridViewColumn = Me.dgvMain.Columns.Item(5)
		Dim iDividerWidth As Integer
		zzShowColumns(bVisible, 6, 13)
		If bVisible Then
			iDividerWidth = 0
		Else
			iDividerWidth = 3
		End If
		oPrevColumn.DividerWidth = iDividerWidth
	End Sub
	Private Sub zzShowNoteColumns(bVisible As Boolean)
		zzShowColumns(bVisible, 15, Me.dgvMain.ColumnCount - 1)
	End Sub

	Private Sub zzShowColumns(bVisible As Boolean, iStartColumn As Integer, iEndColumn As Integer)
		For iColIndex As Integer = iStartColumn To iEndColumn
			Me.dgvMain.Columns.Item(iColIndex).Visible = bVisible
		Next
	End Sub

	Private Sub zzOpenEditorOwners(oGridRow As DataGridViewRow)
		Dim oDataRowView As DataRowView
		Dim bHasRegulation As Boolean
		If Not DMCommon.IntMat.IsLoaded Then
			zzInitIntMat()
		End If

		If oGridRow IsNot Nothing Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			bHasRegulation = DMCommon.Functions.CBoolN(oDataRowView.Item("Regulation"))

			If Not bHasRegulation AndAlso zzOpenOwnerEditor(oDataRowView) Then
				zzCalcCheckRow(False, oDataRowView, oGridRow)
				Me.dgvMain.InvalidateRow(oGridRow.Index)
			End If
		End If
	End Sub
	Private Sub zzOpenEditorNote(oGridRow As DataGridViewRow)
		Dim oDataRowView As DataRowView
		If oGridRow IsNot Nothing Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)

			Dim bNoteEdited As Boolean = DMCommon.Functions.CBoolN(oDataRowView.Item("NoteEdited"))


			If Not bNoteEdited Then
				oGridRow.Cells.Item("ctxNote").Value = zzOpenNoteEditor(oDataRowView)
				If False Then
					dgvMain.RefreshEdit()
					dgvMain.Refresh()

					dgvMain.InvalidateCell(oGridRow.Cells.Item("ctxNote"))
				End If
				zzCalcCheckRow(False, oDataRowView, oGridRow)
			End If
		End If
	End Sub
	Private Sub zzOpenEditorNote(oaGridRow() As DataGridViewRow)
		If oaGridRow IsNot Nothing Then
			Dim iRowsUB As Integer = oaGridRow.GetUpperBound(0)
			Dim oaDataRowView(iRowsUB) As DataRowView
			Dim saRes(iRowsUB) As String
			Dim baNoteEdited(iRowsUB) As Boolean

			For iIndex As Integer = 0 To oaGridRow.GetUpperBound(0)
				oaDataRowView(iIndex) = DirectCast(oaGridRow(iIndex).DataBoundItem, DataRowView)
				baNoteEdited(iIndex) = DMCommon.Functions.CBoolN(oaDataRowView(iIndex).Item("NoteEdited"))
			Next

			saRes = zzOpenNoteEditor(oaDataRowView)

			For iIndex As Integer = 0 To oaGridRow.GetUpperBound(0)
				If Not baNoteEdited(iIndex) Then
					oaGridRow(iIndex).Cells.Item("ctxNote").Value = saRes(iIndex)
					zzCalcCheckRow(False, oaDataRowView(iIndex), oaGridRow(iIndex))
				End If
			Next

		End If
	End Sub

	Private Sub zzInitIntMat()
		Dim iNumCount As Integer = 500
		Dim sComText As String = "SELECT TOP (" & iNumCount.ToString() & ") Value FROM SimpleNumbers"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText, CommandType.Text, zzGetParameters())
		Dim iaSimpleNumbers(iNumCount - 1) As Integer
		Dim iIndex As Integer
		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				Do While oDataReader.Read And iIndex < iNumCount
					iaSimpleNumbers(iIndex) = oDataReader.GetInt32(0)

					iIndex += 1
				Loop
				If iIndex < iNumCount Then
					ReDim Preserve iaSimpleNumbers(iIndex - 1)
				End If
				DMCommon.IntMat.AllDivisors = iaSimpleNumbers
				DMCommon.IntMat.SimpleNumUB = iIndex - 1
				DMCommon.IntMat.IsLoaded = True
			End If
			oDataReader.Close()
		End If

	End Sub
	Private Function zzOpenNoteEditor(ByRef oDataRow As DataRowView) As String
		Dim tOwner As TplnOwner = Nothing
		Dim sRes As String

		mfEditOwners = New frmEditOwners(False)

		mfEditOwners.Visible = False
		mfEditOwners.Show()
		mfEditOwners.Visible = False
		mfEditOwners.ParcelRowView = oDataRow
		mfEditOwners.InitNote()

		sRes = mfEditOwners.NoteCalc()
		mfEditOwners.Close()
		'	oDataRow.Item("Note") = mfEditOwners.NoteCalc()
		Return sRes
	End Function
	Private Function zzOpenNoteEditor(ByRef oaDataRow() As DataRowView) As String()
		Dim tOwner As TplnOwner = Nothing
		Dim saRes(oaDataRow.GetUpperBound(0)) As String
		mfEditOwners = New frmEditOwners(False)
		mfEditOwners.Visible = False
		mfEditOwners.ParcelRowView = oaDataRow(0)
		mfEditOwners.InitNote()
		mfEditOwners.Show()
		mfEditOwners.Visible = False
		saRes(0) = mfEditOwners.NoteCalc()

		For iIndex As Integer = 1 To oaDataRow.GetUpperBound(0)
			mfEditOwners.ParcelRowView = oaDataRow(iIndex)
			saRes(iIndex) = mfEditOwners.NoteCalc()
		Next
		'	oDataRow.Item("Note") = mfEditOwners.NoteCalc()

		mfEditOwners.Close()

		Return saRes
	End Function

	Private Function zzOpenOwnerEditor(ByRef oDataRow As DataRowView) As Boolean
		Dim iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer
		Dim tOwner As TplnOwner = Nothing
		mfEditOwners = New frmEditOwners(True)

		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		mfEditOwners.Owner = Me
		Dim sFilter As String = "(BlockNo=" & iBlockNo.ToString() & ") AND (BlockAddNo=" & iBlockAddNo.ToString() & ") AND (ParcelNo=" & iParcelNo.ToString() & ")"
		Dim sSort As String = "OwnerID,RowIndex"
		'	Dim oMainDataView As DataView = New DataView(moPrjOwnersTable, sFilter, sSort, DataViewRowState.CurrentRows)
		Dim oMainDataView As DataView = zzGetParcelDataView(False, iBlockNo, iBlockAddNo, iParcelNo, oDataRow)
		Dim oOwnerRow As DataRowView
		Dim hsOwnersBefore As HashSet(Of Integer) ' = New HashSet(Of Integer)()
		Dim hsOwnersExistAfter As HashSet(Of Integer) = New HashSet(Of Integer)()

		mfEditOwners.ParcelRowView = oDataRow
		mfEditOwners.MainDataView = oMainDataView

		mfEditOwners.MainDataAdapter = moPrjOwnersDataAdapter
		mfEditOwners.BlockNo = iBlockNo
		mfEditOwners.BlockAddNo = iBlockAddNo
		mfEditOwners.ParcelNo = iParcelNo
		hsOwnersBefore = zzGetOwnerRowList(oDataRow)
		mfEditOwners.AddOwners(hsOwnersBefore)
		Dim iOwnerID As Integer
		Dim iOwnerIndex As Integer
		Dim oNewRow As DataRowView




		hsOwnersExistAfter = New HashSet(Of Integer)(moOwners.OwnersWithField)
		For Each oDataRowView As DataRowView In oMainDataView
			iOwnerID = DMCommon.Functions.CIntN(oDataRowView.Item("OwnerID"))
			If hsOwnersBefore.Contains(iOwnerID) Then
				hsOwnersBefore.Remove(iOwnerID)
			End If
		Next

		'@@@1
		mfEditOwners.InitNote()
		mfEditOwners.OwnersWithField = moOwners.OwnersWithField

		Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, mfEditOwners)
		oMainDataView = mfEditOwners.MainDataView
		If mfEditOwners.DialogResult = DialogResult.OK Then
			DMCommon.Debug.ExcelLog.SetDataRow(0, "!OutDRow", oDataRow.Row)
			zzSetOwnerRowList(mfEditOwners.Owners, oDataRow)

			Return True
			Dim sOwnerText As String = Nothing
			If False Then
				For iIndex As Integer = oMainDataView.Count - 1 To 0 Step -1
					oOwnerRow = oMainDataView.Item(iIndex)
					If zzRowIsEmpty(oOwnerRow) Then
						oOwnerRow.Delete()
					Else
						iOwnerID = DMCommon.Functions.CIntN(oOwnerRow.Item("OwnerID"))
						If iOwnerID <> 0 AndAlso moOwners.TryGetValue(iOwnerID, tOwner) Then
							If Not String.IsNullOrEmpty(tOwner.FieldName) Then
								oDataRow.Item(tOwner.FieldName) = True
								hsOwnersExistAfter.Remove(iOwnerID)
							End If
							If False Then
								If String.IsNullOrEmpty(sOwnerText) Then
									sOwnerText = tOwner.Name
								Else
									sOwnerText &= vbCrLf & tOwner.Name
								End If
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!OwnerTEXT", iOwnerID, tOwner.Name, sOwnerText)
							End If

						End If
					End If
				Next
				For Each iOwnerID In hsOwnersExistAfter
					tOwner = moOwners.Item(iOwnerID)
					oDataRow.Item(tOwner.FieldName) = False
				Next
			End If

			Return False
		End If



	End Function
	Private Function zzGetParcelDataView(bOwnerRows As Boolean, ByRef iBlockNo As Integer, ByRef iBlockAddNo As Integer, ByRef iParcelNo As Integer, ByRef oDataRow As DataRowView) As DataView
		'	Dim tOwner As dmOwner = New dmOwner()
		'	Dim iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer
		iBlockNo = DMCommon.Functions.CIntN(oDataRow.Item("BlockNo"))
		iBlockAddNo = DMCommon.Functions.CIntN(oDataRow.Item("BlockAddNo"))
		iParcelNo = DMCommon.Functions.CIntN(oDataRow.Item("ParcelNo"))

		Dim sFilter As String = "(BlockNo=" & iBlockNo.ToString() & ") AND (BlockAddNo=" & iBlockAddNo.ToString() & ") AND (ParcelNo=" & iParcelNo.ToString() & ")"
		Dim sSort As String = "OwnerID"

		If bOwnerRows Then
			sFilter &= " AND (RowIndex=1)"
		Else
			sSort &= ",RowIndex"
		End If

		Return New DataView(moPrjOwnersTable, sFilter, sSort, DataViewRowState.CurrentRows)
	End Function

	Private Function zzGetOwnerRowList(oDataRow As DataRowView) As HashSet(Of Integer)
		Dim hsOwnersExistBefore As HashSet(Of Integer) = New HashSet(Of Integer)()
		Dim tOwner As TplnOwner = Nothing
		For Each tOwner In moOwners.Values
			If tOwner.HasField AndAlso DMCommon.Functions.CBoolN(oDataRow.Item(tOwner.FieldName)) Then
				hsOwnersExistBefore.Add(tOwner.ID)
			End If
		Next
		Dim sOwnerText As String = DMCommon.Functions.CStrN(oDataRow.Item("OwnerIDs"))
		Dim iaOwnersID() As Integer = DMCommon.Functions.StringToIntArray(sOwnerText)

		If iaOwnersID IsNot Nothing Then
			For iIndex As Integer = 0 To iaOwnersID.GetUpperBound(0)
				If moOwners.TryGetValue(iaOwnersID(iIndex), tOwner) AndAlso Not tOwner.HasField Then
					hsOwnersExistBefore.Add(tOwner.ID)
				End If
			Next
		End If
		Return hsOwnersExistBefore
	End Function
	Private Sub zzSetOwnerRowList(hsOwners As HashSet(Of Integer), ByRef oDataRow As DataRowView)
		Dim bExistsBefore As Boolean
		Dim bExistsAfter As Boolean
		Dim sOwnerIDs As String = String.Empty
		Dim sOwnerNames As String = String.Empty

		For Each oOwner As TplnOwner In moOwners.Values
			If oOwner.HasField Then
				bExistsBefore = DMCommon.Functions.CBoolN(oDataRow.Item(oOwner.FieldName))
				bExistsAfter = hsOwners.Contains(oOwner.ID)
				If bExistsBefore <> bExistsAfter Then
					oDataRow.Item(oOwner.FieldName) = bExistsAfter
				End If
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!OutOwnHas", oOwner.ID, bExistsBefore, bExistsAfter)
			ElseIf hsOwners.Contains(oOwner.ID) Then
				sOwnerIDs &= oOwner.ID.ToString()
				sOwnerNames &= oOwner.ShortName
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!OutOwnNot", oOwner.ID, sOwnerIDs, sOwnerNames)
			End If
		Next
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!OutOwner", oDataRow.Item("OwnerIDs"), oDataRow.Item("OwnerText"), sOwnerIDs, sOwnerNames)
		If Not String.IsNullOrEmpty(sOwnerIDs) Then
			oDataRow.Item("OwnerIDs") = sOwnerIDs
			oDataRow.Item("OwnerText") = sOwnerNames
		ElseIf Not IsDBNull(oDataRow.Item("OwnerIDs")) Then
			oDataRow.Item("OwnerIDs") = DBNull.Value
			oDataRow.Item("OwnerText") = DBNull.Value
		End If
	End Sub

	Private Function zzRowIsEmpty(oRowView As DataRowView) As Boolean
		Dim dValue As Double = DMCommon.Functions.CDblN(oRowView.Item("PartPct"))
		Return (dValue = 0.0)
	End Function

	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		If Not mbDirty Then
			Me.Close()
		End If
	End Sub



	Private Sub frmOwnership_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Bottom - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			Me.Size = New System.Drawing.Size(1324, 488)
			Me.WindowState = FormWindowState.Normal
		End If
	End Sub

	Private Sub dgvMain_CellClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellClick
		If moPrevGridCell IsNot Nothing Then
			If moPrevGridCell.RowIndex = e.RowIndex AndAlso moPrevGridCell.ColumnIndex = e.ColumnIndex Then



			End If
		End If
		moPrevGridCell = dgvMain.CurrentCell

	End Sub

	Private Sub cmdCancel_Click(oSender As System.Object, e As EventArgs) Handles cmdCancel.Click
		If mbDirty Then
			mbDirty = False
			Me.Close()
		End If
	End Sub

	Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
		mbDirty = True
		If e.ColumnIndex >= 0 AndAlso e.ColumnIndex < Me.dgvMain.Columns.Count AndAlso e.RowIndex >= 0 AndAlso e.RowIndex < Me.dgvMain.Rows.Count Then
			Dim sColumnName As String = Me.dgvMain.Columns.Item(e.ColumnIndex).Name
			Dim oDataRow As DataRow = zzGetDataRow(e.RowIndex)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel
			Dim dParagraph19Area As Double = 0.0
			Dim dLeasingArea As Double = 0.0
			Dim dOverlayPrg5LeasArea As Double = 0.0

			Select Case sColumnName
				Case "cchParagraph19"

					If DMCommon.Functions.CBoolN(oDataRow.Item("Paragraph19")) Then
						oDataRow.Item("Paragraph19Area") = oDataRow.Item("ParcelInLegalArea")
					Else
						oParcel = zzGetParcel(oDataRow)
						oParcel.GetOwnershipNoteArea(dParagraph19Area, dLeasingArea, dOverlayPrg5LeasArea)
						oDataRow.Item("Paragraph19Area") = dParagraph19Area
					End If

				Case "cchLeasing"

					If DMCommon.Functions.CBoolN(oDataRow.Item("Leasing")) Then
						oDataRow.Item("LeasingArea") = oDataRow.Item("ParcelInLegalArea")
					Else
						oParcel = zzGetParcel(oDataRow)
						oParcel.GetOwnershipNoteArea(dParagraph19Area, dLeasingArea, dOverlayPrg5LeasArea)
						oDataRow.Item("LeasingArea") = dLeasingArea
					End If

				Case "ctxNote"
					oDataRow.Item("NoteEdited") = True
			End Select
		End If

	End Sub

	Private Sub dgvMain_CellDoubleClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellDoubleClick
		Dim oViewRow As DataGridViewRow = Me.dgvMain.Rows.Item(e.RowIndex)
		If e.ColumnIndex <= 13 Then
			zzOpenEditorOwners(oViewRow)
		End If

	End Sub

	Private Sub chkOwnersColumnsVisible_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkOwnersColumnsVisible.CheckedChanged
		zzShowOwnerColumns(chkOwnersColumnsVisible.Checked)

	End Sub

	Private Sub chkNoteColumnsVisible_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkNoteColumnsVisible.CheckedChanged
		zzShowNoteColumns(Me.chkNoteColumnsVisible.Checked)
	End Sub



	Private Sub frmOwnership_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		zzSetCheckBoxValue()
		zzFillPaintScale()
		zzSetDataSource()
		zzCalcCheckData()


	End Sub
	Private Sub zzAddStatusField()
		moOwnershipTable.Columns.Add("Status", System.Type.GetType("System.Int32"))
	End Sub

	Private Sub Button1_Click(oSender As System.Object, e As EventArgs)
		zzLaunchReport()
	End Sub
	Private Sub zzLaunchReport()
		'Dim oReport As AcadReport.BaseReport = New AcadReport.Report
		Dim oAcadReport As AcadReport.Report = New AcadReport.Report



		Dim iOptionIndex As Integer = -1
		Dim bTopoPurpose As Boolean = False
		Dim bAllArea As Boolean = False

		Dim oaOptionValues(1) As System.Object
		Dim oaMultiOptionValues()() As System.Object = Nothing
		'	Dim iSelectedRegion As Integer
		'	Dim bRegion As Boolean
		Dim sTopoPurposeText As String = Nothing
		'   DMCommon.ExcelLogW.OpenA()
		Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
		Dim taColorScheme() As DMAcadExt.ColorScheme










		Me.Cursor = Cursors.WaitCursor



		Dim oDataView As DataView = Nothing
		Dim oaDataView() As DataView = Nothing
		Dim saModData() As String = Nothing
		Dim iaDataColumns() As Integer = Nothing
		Dim iGroupColumnStart As Integer = -1
		Dim iGroupColumnUB As Integer = 0


		Dim oaCaptions() As System.Object = Nothing
		Dim oaTotals() As System.Object = Nothing
		'    DMCommon.ExcelLogG.Open()

		AcadReport.RepApp.InitDWGScaleFactor()











		'frmReports.vb : line 531





		'  DMCommon.ExcelLogG.SetDataTable(oDataView, 0)




		'  oaCaptions = TPlanGraph.TplnOwner.GetCaptions(True)



		''''''''''''''''''''''''Case TPlServerDB.enResourceTheme.AcRepLegendK

		If True Then
			'!!!!!!!!!!!''''''''''''''''''''''''''''''''''''''''''''''''''''	TPlanGraph.TplnLot.GetLanduseList(moSelectedReportItem.TopoPurpose, oDataView, (Parameters.LegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor))
		End If

		'Case TPlServerDB.enResourceTheme.AcRepLegendM
		'TPlanGraph.TplnLot.GetLanduseList(DMAcadExt.enTopoPurpose.Proposed, oDataView, Parameters.LegendPaintFactor * AcadReport.RepApp.DrawingScaleFactor * zzGetSelectedScale())


		zzGetColorSchemeList(oDataView)

		DMCommon.Debug.MsgBox("01_399P", DMCommon.Debug.ColCount(oDataView))



		'DMCommon.Debug.MsgBox("01_399Q", DMCommon.Debug.ColCount(oDataView))





		'




		oAcadReport.MainView = oDataView
		DMCommon.Debug.MsgBox("13_130d", oAcadReport.MainView.Count)
		If iaDataColumns IsNot Nothing Then
			oAcadReport.DataColumns = iaDataColumns
		End If
		If True Then
			Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
			Dim bCurrentLayerOK As Boolean = True
			Try
				oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", String.Empty, True)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				DMAcadExt.AcadErrCode.ShowAcadError(oAcadEx, False, "frmTopoActions - zzInsertReport_02")
			End Try
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(String.Empty, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, True)
			'System.Windows.Forms.MessageBox.Show(CStr(bCurrentLayerOK And False), "05_010")
			If bCurrentLayerOK Then
				' System.Windows.Forms.MessageBox.Show(CStr(999), "05_024")
				Me.Hide()
				TopoManager.Common.SetAcadFocus()
				AcadReport.RepApp.GetStartPoint()
				If oAcadReport.Insert() Then
					AcadReport.RepApp.AcadTable.RecomputeTableBlock(True)
					''''''''''		DMAcadExt.AcadDocument.Regen()
					AcadReport.RepApp.AcadTable.Draw()
					AcadReport.RepApp.PaintCells()
				End If
				Me.Show()
			End If
			'  System.Windows.Forms.MessageBox.Show("", "05_040")
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			If oDocLock IsNot Nothing Then
				oDocLock.Dispose()
				oDocLock = Nothing
			End If
		Else
			''''''''''oRepApp.Insert()
		End If

		'If oaCaptions IsNot Nothing Then
		'   DMCommon.Functions.DispArray(oaCaptions, "01_882", True)
		'   oReport.Captions = oaCaptions
		'Else
		'   MessageBox.Show("oaCaptions Is Nothing", "01_882no")
		'End If
		If oaOptionValues IsNot Nothing Then
			'   DMCommon.Functions.DispArray(oaOptionValues, "01_887b", True)
		End If
		'	DMCommon.Debug.MsgBox("01_399M", moSelectedReportItem.ReportModification)


		If oDataView IsNot Nothing Then
			'DMCommon.Functions.DispArray(oaOptionValues, "13_130K", True)
			If AcadReport.RepApp.AcadTable IsNot Nothing Then
				''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
				'''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
				'''''''''''''''''	AcadReport.Report.ReDrawTable()
				colaPoints = oAcadReport.GetColorCells()

				If colaPoints IsNot Nothing Then
					taColorScheme = oAcadReport.GetColorScheme()
					Dim oPgon As TopoManager.SimplePgon

					'		Dim dRepBamashSharedScale As Double

					'		Try
					'dRepBamashSharedScale = Convert.ToDouble(Me.txtRepBamashSharedScale.Text)
					'	If dRepBamashSharedScale < 0.00001 Then
					'dRepBamashSharedScale = 1.0
					'	End If
					'	Catch oEx As Exception
					'dRepBamashSharedScale = 1.0
					'			End Try



					For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
						oPgon = New TopoManager.SimplePgon(colaPoints(iIndex))
						oPgon.SetTagNum("Table", iIndex)
						If iIndex < 0 Then
							Dim s As String = ""
							For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colaPoints(iIndex)
								s &= DMAcadExt.TPlnPoint.DispPoint(tPoint) & vbCrLf
							Next

							DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "; " & s)
						End If
						DMAcadExt.AcadTransaction.OpenNewAnonymBlock()

						'	taColorScheme(iIndex).Scale = dRepBamashSharedScale * AcadReport.RepApp.DrawingScaleFactor
						oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, taColorScheme(iIndex), False)

						DMAcadExt.AcadTransaction.InsertNewBlock(False)
					Next
					oAcadReport.ClearZebraCells()
				End If

			End If
			Me.Show()
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.RestoreVarCmdDia()
		End If







		Me.Cursor = Cursors.Default
	End Sub
	Private Sub zzGetColorSchemeList(ByRef oDataView As System.Data.DataView)
		'	MessageBox.Show(CStr("GetLanduseList") & vbCrLf & "", "04_306d")
		Const msNameFieldName As String = "LotName"
		Const msLanduseOrderFieldName As String = "LanduseOrder"
		Const msColorFieldName As String = "Color"

		Dim oDataTable As System.Data.DataTable = New DataTable("LegendColoSchemes")
		Dim iLanduseOrder As Integer
		With oDataTable.Columns
			.Add(msNameFieldName, GetType(System.String))
			.Add(msColorFieldName, GetType(DMAcadExt.ColorScheme))
			.Add(msLanduseOrderFieldName, GetType(System.Int32))
		End With

		Dim oNewRow As System.Data.DataRow
		'Dim tColorScheme As DMAcadExt.ColorScheme
		For Each tColorScheme As DMAcadExt.ColorScheme In mdicLegendColoSchemes.Values

			oNewRow = oDataTable.NewRow()

			oNewRow.Item(msNameFieldName) = tColorScheme.Name

			oNewRow.Item(msColorFieldName) = tColorScheme
			oNewRow.Item(msLanduseOrderFieldName) = iLanduseOrder
			oDataTable.Rows.Add(oNewRow)
		Next

		oDataView = New DataView(oDataTable)
		oDataView.Sort = msLanduseOrderFieldName

	End Sub

	Private Sub cmdDrawLegend_Click(oSender As System.Object, e As EventArgs) Handles cmdDrawLegend.Click

		'	Dim iTopoPurpose As DMAcadExt.enTopoPurpose = mtMapThemeData.TopoPurpose
		Dim iaDataColumns() As Integer = Nothing
		Dim oaTotals() As System.Object = Nothing
		Dim oaOptionValues(0) As System.Object
		Dim dLegendPaintFactor As Double


		oaOptionValues(0) = ""
		Try
			dLegendPaintFactor = zzGetSelectedScale() / mdBaseScale

		Catch oEx As Exception
			dLegendPaintFactor = 1
		End Try
		' System.Windows.Forms.MessageBox.Show(CStr(dLegendPaintFactor) & vbCrLf & CStr(AcadReport.RepApp.DrawingScaleFactor), "21_455")
		Dim oDataView As DataView = Nothing
		If mdicLegendColoSchemes Is Nothing Then
			zzPaintAllParcelsPlusHatch(False)
		End If
		zzGetColorSchemeList(oDataView)
		'   System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count), "21_456")
		Dim oLaunchReport As LaunchReport = New LaunchReport(True, TPlServerDB.enResourceTheme.AcRepLegendK)
		oLaunchReport.PaintFactor = dLegendPaintFactor

		Me.Hide()
		oLaunchReport.InsertReport(oDataView, iaDataColumns, oaTotals, oaOptionValues)
		Me.Show()

	End Sub
	Private Sub zzCheckOwnersPresence()
		For Each oOwner As TplnOwner In moOwners.Values
			If Not oOwner.MAPIMandatory Then
				zzCheckOwnerPresence(oOwner)
			End If
		Next
	End Sub
	Private Sub zzCheckOwnerPresence(oOwner As TplnOwner)
		Dim sOwnerText As String
		Dim bPresence As Boolean

		For Each oRow As Data.DataRowView In moParcelOwnershipDataView
			If oOwner.HasField Then
				bPresence = DMCommon.Functions.CBoolN(oRow.Item(oOwner.FieldName))
			Else
				sOwnerText = DMCommon.Functions.CStrN(oRow.Item("OwnerIDs"))
				If String.IsNullOrEmpty(sOwnerText) Then
					bPresence = False
				Else
					bPresence = InStr(sOwnerText, oOwner.ID.ToString()) <> 0
				End If
			End If
			If bPresence Then
				moOwners.AddActual(oOwner.ID)
				Exit For
			End If


		Next

	End Sub
	Private Sub zzCheckRegulation()
		Dim tOwner As TplnOwner = Nothing
		For Each oRow As Data.DataRowView In moParcelOwnershipDataView
			If DMCommon.Functions.CBoolN(oRow.Item("Regulation")) Then
				If moOwners.TryGetValue("Regulation", tOwner) Then
					moOwners.AddActual(tOwner.ID)
					Exit For
				End If
			End If

		Next

	End Sub
	Private Sub cmdExcelReport_Click(sender As System.Object, e As EventArgs) Handles cmdExcelReport.Click
		Me.Cursor = Cursors.WaitCursor
		Const sSheetName As String = "בעלויות"
		Const sTotalACaption As String = "סה""כ"
		Const sTotalBCaption As String = "סה""כ רמ""י"

		Dim iOutputRow As Integer = 0
		Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
		Dim daColumnWidth(13) As Double
		Dim daColumnWidthA() As Double = {6.5, 6.5, 7.5, 10.5, 26.0, 12.75}
		Dim oDataView As DataView
		Dim oaCaptions() As System.Object
		Dim iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer
		Dim iParagraph19Column As Integer = -1
		Dim iLeasingColumn As Integer = -1
		Dim iNoteColumn As Integer
		Dim oBalanceArea As TopoManager.BalanceArea

		Dim iLastColumn As Integer
		Dim iLastOwnerColumn As Integer
		Dim bUseBalance As Boolean = Me.chkUseBalance.Checked


		'Dim oaCaptions(15) As system.Object
		Dim tRect As Rectangle
		'	Dim colOwners As System.Collections.ObjectModel.Collection(Of dmOwner) = zzGetMAPIOwners()
		Dim hsParcelOwners As HashSet(Of Integer) = Nothing
		Dim colPartPct As System.Collections.ObjectModel.Collection(Of Double) = Nothing
		'zzCheckRegulation()
		zzCheckOwnersPresence()
		Dim oaOwnerCaption() As String = moOwners.MAPICaptionsNew

		Dim iOwnerCurrentIndex As Integer
		Dim iOwnerID As Integer
		Dim dArea As Double
		Dim dLegalArea As Double
		Dim dInLegalArea As Double

		Dim tOwner As TplnOwner = Nothing
		Dim sOwnerNameExt As String = Nothing

		Dim oParcel As TopoManager.TPlanGraph.TplnParcel

		Dim iCurrentBlockNo As Integer
		Dim iLastBlockNo As Integer

		Dim iLastBlockAddNo As Integer
		Dim iLastBlockRow As Integer
		Dim sCellAddress As String
		Dim sRectAddress As String

		Dim sDividendAddress As String
		Dim sDivisorAddress As String


		Dim iStartTableRow As Integer = 0
		Dim iEndTableRow As Integer = 0
		Dim iFirstTotalBRow As Integer

		Dim oaTotalFormulasSum(oaOwnerCaption.GetUpperBound(0) + 1) As System.Object
		Dim oaTotalFormulasPct(oaOwnerCaption.GetUpperBound(0) + 1) As System.Object
		Dim hsOwners As HashSet(Of Integer)
		Dim tTotalColor As Color = Color.FromArgb(255, 243, 209)
		Dim dParagraph19Area As Double
		Dim dLeasingArea As Double

		'	oExcelAppExt.Close()

		If oExcelAppExt.Open() Then


			oExcelAppExt.SetSheetName(sSheetName)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "oaOwnerCaption", oaOwnerCaption)
			daColumnWidth(0) = 6.5
			daColumnWidth(1) = 6.5

			For iIndex As Integer = 2 To daColumnWidth.GetUpperBound(0)
				daColumnWidth(iIndex) = 25
			Next
			oExcelAppExt.SetFormatColumns(0, daColumnWidthA)
			tRect = New Rectangle(0, 0, 4, 0)
			oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(0, 3))
			iOutputRow += 1
			ReDim oaCaptions(4)
			For iColumn As Integer = 0 To 4
				oaCaptions(iColumn) = zzGetText(iColumn + 1, 3)
			Next
			oExcelAppExt.SetValueInHeaderRow(iOutputRow, 0, True, oaCaptions)

			tRect = New Rectangle(5, 0, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(6, 3))

			tRect = New Rectangle(6, 0, oaOwnerCaption.Count - 1, 0)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(7, 3))
			oExcelAppExt.SetValueInHeaderRow(1, 6, True, oaOwnerCaption)
			iLastOwnerColumn = 5 + oaOwnerCaption.GetUpperBound(0) + 1
			iLastColumn = iLastOwnerColumn
			'iNoteColumn = 5 + oaOwnerCaption.GetUpperBound(0) + 2
			If mbParagraph19Exists Then
				iLastColumn += 1
				iParagraph19Column = iLastColumn
				tRect = New Rectangle(iParagraph19Column, 0, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(8, 3))
			End If
			If mbLeasingExists Then
				iLastColumn += 1
				iLeasingColumn = iLastColumn
				tRect = New Rectangle(iLeasingColumn, 0, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(9, 3))
			End If
			iLastColumn += 1
			iNoteColumn = iLastColumn
			tRect = New Rectangle(iNoteColumn, 0, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(10, 3))
			oExcelAppExt.SetFormatColumns(iNoteColumn, 36.0)

			If False Then
				tRect = New Rectangle(iNoteColumn, 0, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(8, 3))
				oExcelAppExt.SetFormatColumns(iNoteColumn, 16.0)


				If mbLeasingExists Then
					iLeasingColumn = iNoteColumn + 1
					iLastColumn = iLeasingColumn

					tRect = New Rectangle(iLeasingColumn, 0, 0, 1)
					oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(9, 3))

				Else
					iLastColumn = iNoteColumn
				End If
			End If

			iStartTableRow = 2
			iEndTableRow = moParcelOwnershipDataView.Count + 1

			oExcelAppExt.SetBorders(iStartTableRow, 0, iEndTableRow - iStartTableRow + 1, iLastColumn + 1)

			oExcelAppExt.SetBorders(iEndTableRow + 1, 5, 2, oaOwnerCaption.GetUpperBound(0) + 2)
			oExcelAppExt.SetBorders(iEndTableRow + 4, 5, 2, 4)


			'	DMCommon.Debug.MsgBox("AfterHeader", moParcelOwnershipDataView.Count, mbLeasingExists, zzGetText(9, 3), mbParagraph19Exists)
			For Each oDataRow As Data.DataRowView In moParcelOwnershipDataView
				iOutputRow += 1
				hsOwners = zzGetOwnerRowList(oDataRow)
				'	dParcelInLegalArea = DMCommon.Functions.CDblN(oDataRow.Item("ParcelInLegalArea"))
				oDataView = zzGetParcelDataView(True, iBlockNo, iBlockAddNo, iParcelNo, oDataRow)
				dLegalArea = DMCommon.Functions.CDblN(oDataRow.Item("ParcelLegalArea")) * 0.001
				dInLegalArea = DMCommon.Functions.CDblN(oDataRow.Item("ParcelInLegalArea")) * 0.001
				If oDataView IsNot Nothing AndAlso oDataView.Count > 0 Then
					iOwnerCurrentIndex = 0

					oParcel = zzGetParcel(oDataRow)
					If oParcel.OwnerArea IsNot Nothing Then
						For iIndex As Integer = 0 To oParcel.OwnerArea.GetUpperBound(0)
							iOwnerID = oParcel.OwnerArea(iIndex).OwnerID
							dArea = oParcel.OwnerArea(iIndex).AreaSet.CalcArea * 0.001
							'tOwner = moOwners.Item(iOwnerID)
							If moOwners.TryGetValue(iOwnerID, tOwner) Then
								oExcelAppExt.SetValueInRow(iOutputRow, 6 + tOwner.Index, dArea)
								If iIndex = 0 Then
									sOwnerNameExt = tOwner.Name
								Else
									sOwnerNameExt &= ", " & tOwner.Name
								End If
							Else
								oExcelAppExt.SetValueInRow(iOutputRow, 18, iOwnerID)
							End If

						Next
					Else
						oExcelAppExt.SetValueInRow(iOutputRow, 18, "oParcel.OwnerArea Is Nothing")
					End If
				Else
					iOwnerID = hsOwners.ElementAt(0)
					tOwner = moOwners.Item(iOwnerID)
					sOwnerNameExt = tOwner.Name

					oExcelAppExt.SetValueInRow(iOutputRow, 6 + moOwners.Item(iOwnerID).Index, dInLegalArea)

				End If

				iCurrentBlockNo = DMCommon.Functions.CIntN(oDataRow.Item("BlockNo"))
				If iLastBlockNo <> iCurrentBlockNo Then
					If iLastBlockNo <> 0 Then
						tRect = New Rectangle(0, iLastBlockRow + 1, 0, iOutputRow - iLastBlockRow - 2)
						oExcelAppExt.Merge(tRect, True, iLastBlockNo)
						tRect = New Rectangle(0, iOutputRow - 1, iLastColumn, 0)
						oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Medium)
					End If
					iLastBlockNo = iCurrentBlockNo
					iLastBlockRow = iOutputRow - 1
				End If
				oExcelAppExt.SetValueInRow(iOutputRow, 1, oDataRow.Item("ParcelNo"), zzGetBlockStatus(oDataRow), dLegalArea, sOwnerNameExt, dInLegalArea)

				If mbParagraph19Exists Then
					dParagraph19Area = DMCommon.Functions.CDblN(oDataRow.Item("Paragraph19Area")) * 0.001
					If dParagraph19Area > 0.0 Then
						oExcelAppExt.SetValueInRow(iOutputRow, iParagraph19Column, dParagraph19Area)
					End If
				End If

				If mbLeasingExists Then
					dLeasingArea = DMCommon.Functions.CDblN(oDataRow.Item("LeasingArea")) * 0.001
					If dLeasingArea > 0.0 Then
						oExcelAppExt.SetValueInRow(iOutputRow, iLeasingColumn, dLeasingArea)
					End If
				End If

				oExcelAppExt.SetValueInRow(iOutputRow, iNoteColumn, oDataRow.Item("Note"))
			Next

			iOutputRow += 1

			tRect = New Rectangle(0, iLastBlockRow + 1, 0, iOutputRow - iLastBlockRow - 2)
			oExcelAppExt.Merge(tRect, True, iLastBlockNo)
			tRect = New Rectangle(0, iOutputRow - 1, iLastColumn, 0)
			oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Medium)

			For iColumn As Integer = 5 To oaOwnerCaption.GetUpperBound(0) + 6
				'	oTableColumnRectangle = New Rectangle(iColumn, iStartTableRow, 1, iEndTableRow - iStartTableRow + 1)
				sRectAddress = oExcelAppExt.GetRectangleAddress(iStartTableRow, iColumn, iEndTableRow - iStartTableRow + 1, 1)
				oaTotalFormulasSum(iColumn - 5) = "=SUM(" & sRectAddress & ")"

				If iColumn = 5 Then
					sRectAddress = oExcelAppExt.GetRectangleAddress(iEndTableRow + 2, 6, 1, oaOwnerCaption.GetUpperBound(0) + 1)
					oaTotalFormulasPct(iColumn - 5) = "=SUM(" & sRectAddress & ")"
				Else

					sDividendAddress = oExcelAppExt.GetCellAddress(iEndTableRow + 1, iColumn)
					sDivisorAddress = oExcelAppExt.GetCellAddress(iEndTableRow + 1, 5)
					If Not bUseBalance Then
						oaTotalFormulasPct(iColumn - 5) = "=" & sDividendAddress & "/" & sDivisorAddress


					End If
					oExcelAppExt.SetFormatValue(iOutputRow + 1, iColumn,       , "0.0%")
				End If

			Next


			oExcelAppExt.SetFormulaInRow(iOutputRow, 5, True, oaTotalFormulasSum)
			Dim oaValues() As System.Object = oExcelAppExt.GetRowValues(iOutputRow, 6, oaTotalFormulasSum.GetUpperBound(0))
			Dim daValues(oaValues.GetUpperBound(0)) As Double
			Dim dSum As Double
			For iIndex As Integer = 0 To oaValues.GetUpperBound(0)
				daValues(iIndex) = DMCommon.Functions.CDblN(oaValues(iIndex))
				dSum += daValues(iIndex)
			Next
			'oExcelAppExt.SetValueInRow(iOutputRow, 16, oaValues)
			DMCommon.Debug.MsgBox("!dSUM", bUseBalance, dSum)

			'	oExcelAppExt.SetValueInRow(iOutputRow + 1, 16, oBalanceArea.OutputValues())
			If bUseBalance AndAlso dSum > 0.0 Then
				oBalanceArea = New TopoManager.BalanceArea(daValues, 1000.0, 1.0, True, "")
				oExcelAppExt.SetValueInRow(iOutputRow + 1, 6, oBalanceArea.OutputValues())

			End If

			iOutputRow += 1
			oExcelAppExt.SetFormulaInRow(iOutputRow, 5, True, oaTotalFormulasPct)

			tRect = New Rectangle(5, 0, 0, iEndTableRow + 2)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium)

			tRect = New Rectangle(6, 0, 0, iEndTableRow + 2)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium)

			tRect = New Rectangle(9, 1, 0, iEndTableRow + 1)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium)


			'	sssss
			tRect = New Rectangle(iLastOwnerColumn, 1, 0, iEndTableRow + 1)
			oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Medium)

			If mbParagraph19Exists Then
				tRect = New Rectangle(iParagraph19Column, 2, 0, iEndTableRow - 1)
				oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium)

				sRectAddress = oExcelAppExt.GetRectangleAddress(iStartTableRow, iParagraph19Column, iEndTableRow - iStartTableRow + 1, 1)

				'	oaTotalFormulasSum(iColumn - 5) = 
				oExcelAppExt.SetFormulaInRow(iEndTableRow + 1, iParagraph19Column, True, {"=SUM(" & sRectAddress & ")"})
				'oExcelAppExt.SetBackColor(tRect, Color.FloralWhite)
			End If
			If mbLeasingExists Then
				tRect = New Rectangle(iLeasingColumn, 2, 0, iEndTableRow - 1)
				oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium)
				'''''''''	oExcelAppExt.SetBackColor(tRect, Color.DarkKhaki)
				sRectAddress = oExcelAppExt.GetRectangleAddress(iStartTableRow, iLeasingColumn, iEndTableRow - iStartTableRow + 1, 1)

				'	oaTotalFormulasSum(iColumn - 5) = 
				oExcelAppExt.SetFormulaInRow(iEndTableRow + 1, iLeasingColumn, True, {"=SUM(" & sRectAddress & ")"})
			End If


			tRect = New Rectangle(iNoteColumn, 0, 0, iEndTableRow)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium,,, DMCommon.ExcelAppExt.enBorderWeight.Medium)


			tRect = New Rectangle(4, iEndTableRow + 1, oaOwnerCaption.GetUpperBound(0) + 2, 1)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium,, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium)  ',, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium
			oExcelAppExt.SetBackColor(tRect, tTotalColor)

			tRect = New Rectangle(4, iEndTableRow + 1, 0, 1)
			oExcelAppExt.Merge(tRect, True, sTotalACaption, True)


			ReDim oaTotalFormulasSum(3)
			ReDim oaTotalFormulasPct(3)
			iFirstTotalBRow = iEndTableRow + 4

			For iColumn As Integer = 5 To 8
				If iColumn = 5 Then

					sRectAddress = oExcelAppExt.GetRectangleAddress(iEndTableRow + 4, 6, 1, 3)
					oaTotalFormulasSum(iColumn - 5) = "=SUM(" & sRectAddress & ")"
					sRectAddress = oExcelAppExt.GetRectangleAddress(iEndTableRow + 5, 6, 1, 3)
					oaTotalFormulasPct(iColumn - 5) = "=SUM(" & sRectAddress & ")"

				Else
					sCellAddress = oExcelAppExt.GetCellAddress(iEndTableRow + 1, iColumn)
					oaTotalFormulasSum(iColumn - 5) = "=" & sCellAddress
					sDividendAddress = oExcelAppExt.GetCellAddress(iFirstTotalBRow, iColumn)
					sDivisorAddress = oExcelAppExt.GetCellAddress(iFirstTotalBRow, 5)

					If Not bUseBalance Then
						oaTotalFormulasPct(iColumn - 5) = "=" & sDividendAddress & "/" & sDivisorAddress
						oaTotalFormulasPct(iColumn - 5) = "=IF(" & sDivisorAddress & "=0,," & sDividendAddress & "/" & sDivisorAddress & ")"
						'=IF(F7=0,,I7/F7)
					End If

					oExcelAppExt.SetFormatValue(iFirstTotalBRow + 1, iColumn, , "0.0%")
				End If

			Next
			oExcelAppExt.SetFormulaInRow(iFirstTotalBRow, 5, True, oaTotalFormulasSum)
			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''oaValues = oExcelAppExt.GetRowValues(iOutputRow, 6, oaTotalFormulasSum.GetUpperBound(0))


			'	oExcelAppExt.SetValueInRow(iOutputRow + 3, 16, oBalanceArea.OutputValues())
			If bUseBalance AndAlso dSum > 0.0 Then
				ReDim Preserve daValues(2)
				oBalanceArea = New TopoManager.BalanceArea(daValues, 1000.0, 1.0, True, "")
				oExcelAppExt.SetValueInRow(iOutputRow + 3, 6, oBalanceArea.OutputValues())
			End If
			'''''''''''''''''''''''''''''''''''''''
			oExcelAppExt.SetFormulaInRow(iFirstTotalBRow + 1, 5, True, oaTotalFormulasPct)

			tRect = New Rectangle(4, iFirstTotalBRow, 4, 1)
			oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium, DMCommon.ExcelAppExt.enBorderWeight.Medium)
			oExcelAppExt.SetBackColor(tRect, tTotalColor)
			tRect = New Rectangle(4, iFirstTotalBRow, 0, 1)
			oExcelAppExt.Merge(tRect, True, sTotalBCaption, True)
		End If
		Me.Cursor = Cursors.Default
	End Sub
	Private Function zzGetBlockStatus(oDataRow As Data.DataRowView) As String
		Dim iBlockNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("BlockNo"))
		Dim iBlockAddNo As Integer = DMCommon.Functions.CIntN(oDataRow.Item("BlockAddNo"))
		Dim iKey As Integer
		Dim oBlock As TopoManager.TPlanGraph.TplnBlock = Nothing
		If iBlockNo <> 0 Then
			iKey = TopoManager.TPlanGraph.BlockData.GetBlockKey(iBlockNo, iBlockAddNo)
			If TopoManager.TPlanGraph.TplnProject.Blocks.TryGetValue(iKey, oBlock) Then
				Return oBlock.BlockStatusName
			End If
		End If
		Return Nothing

	End Function

	Private Sub cmdCheckData_Click(oSender As System.Object, e As EventArgs) Handles cmdCheckData.Click
		Me.Cursor = Cursors.WaitCursor
		zzCalcCheckData()
		Me.cmdExcelReport.Enabled = True
		Me.cmdPaintParcels.Enabled = True
		Me.Cursor = Cursors.Default
	End Sub



	Private Sub cmdZoom_Click(oSender As System.Object, e As EventArgs) Handles cmdZoom.Click
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim iGridRowIndex As Integer = oGridRow.Index
		Dim oDataRowView As DataRowView
		Dim oBoundingBox As DMAcadExt.TPlnBoundingBox
		If oGridRow IsNot Nothing Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = zzGetParcel(oDataRowView)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			oParcel.Highlight()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			oBoundingBox = oParcel.BoundingBox
			DMAcadExt.AcadDocument.Zoom(oBoundingBox)
		End If




	End Sub




	Private Sub cmdCalc_Click(oSender As System.Object, e As EventArgs) Handles cmdCalc.Click


		If Me.dgvMain.SelectedRows.Count > 0 Then
			Dim oaGridRow(Me.dgvMain.SelectedRows.Count - 1) As DataGridViewRow
			For iSelectedIndex As Integer = 0 To Me.dgvMain.SelectedRows.Count - 1
				oaGridRow(iSelectedIndex) = Me.dgvMain.SelectedRows.Item(iSelectedIndex)

			Next
			zzOpenEditorNote(oaGridRow)
		Else

			Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
			zzOpenEditorNote(oGridRow)
		End If




	End Sub

	Private Sub cmdSelectParcelByPoint_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectParcelByPoint.Click
		Dim oParcelsTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Parcels", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		Dim bOneOnly As Boolean = False
		Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim colRes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		Dim hsTopoIDs As HashSet(Of Integer) = New HashSet(Of Integer)()
		Dim oDataRow As Data.DataRowView
		Dim iTopoID As Integer
		Dim dNewAngle As Double
		Dim sNewAngle As String = Me.cmbPaintAngle.Text
		If Double.TryParse(sNewAngle, dNewAngle) Then
			dNewAngle = dNewAngle Mod 180.0
			oParcelsTopology = TopoManager.TopoCreator.GetOpenedTopology("Parcels", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oParcelsTopology IsNot Nothing Then
				Me.Visible = False

				Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
				Do
					iSelectStatus = zzSelectPoint(tPoint, bOneOnly)

					If iSelectStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
						Try
							oPolygon = oParcelsTopology.FindPolygon(tPoint)
							If Not hsTopoIDs.Contains(oPolygon.ID) Then
								hsTopoIDs.Add(oPolygon.ID)
							End If

						Catch oEx As Exception

						End Try
					Else
						Exit Do
					End If
				Loop
				If iSelectStatus <> Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
					If False Then
						For iRowIndex As Integer = 0 To moParcelOwnershipDataView.Count - 1
							oDataRow = moParcelOwnershipDataView.Item(iRowIndex)
							iTopoID = DMCommon.Functions.CIntN(oDataRow.Item("ParcelTopoID"))
							If hsTopoIDs.Contains(iTopoID) Then
								oDataRow.Item("PaintAngle") = dNewAngle
							End If
						Next
					End If
					zzSet(dNewAngle, hsTopoIDs)
				End If
				DMCommon.Debug.MsgBox("13_147w", iSelectStatus, hsTopoIds.Count)
				Me.Visible = True
				oParcelsTopology.Close()
			End If
		End If

	End Sub

	Private Shared Function zzSelectPoint(ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d, bOneOnly As Boolean) As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptOptDebug As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions("")
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(" Select Point")

		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult
		oPromptOpt.AllowNone = Not bOneOnly

		'    Dim tVector As Autodesk.AutoCAD.Geometry.Vector2d
		'   Dim bRes As Boolean
		'  DMAcadExt.AcadDocument.WriteMessage("Start select Ss10b " & oPromptOpt.AllowNone & "; " & oPromptOpt.AllowArbitraryInput & vbCrLf)
		ptRes = oEditor.GetPoint(oPromptOpt)
		'   DMAcadExt.AcadDocument.WriteMessage("after select Ss10a" & vbCrLf)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Try
				tPoint = ptRes.Value
				'     tVector = New Autodesk.AutoCAD.Geometry.Vector2d(tPoint.X, tPoint.Y)
				'  bRes = True
				' oEditor.WriteMessage("OK! " & tPoint.ToString() & vbCrLf)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "1:Ownership-SelectPoint")
			End Try
		End If
		Return ptRes.Status
	End Function
	Private Sub zzSet(dNewAngle As Double, hsTopoIDs As HashSet(Of Integer))
		Dim oDataRow As Data.DataRowView
		Dim iRowIndex As Integer
		''Dim oGridRow As DataGridViewRow
		For Each iTopoID As Integer In hsTopoIDs
			If mdicDataRows.TryGetValue(iTopoID, iRowIndex) Then
				oDataRow = moParcelOwnershipDataView.Item(iRowIndex)
				oDataRow.Item("PaintAngle") = dNewAngle
			End If
		Next
	End Sub

	Private Sub cmdSelectByPick_Click(oSender As System.Object, e As EventArgs) Handles cmdSelectByPick.Click
		Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bOneOnly As Boolean = True
		Dim iSelectStatus As Autodesk.AutoCAD.EditorInput.PromptStatus
		Dim iRowIndex As Integer
		Dim oParcelsTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Parcels", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oParcelsTopology IsNot Nothing Then
			Me.Visible = False
			Autodesk.AutoCAD.Internal.Utils.SetFocusToDwgView()
			iSelectStatus = zzSelectPoint(tPoint, bOneOnly)
			If iSelectStatus = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				DMCommon.Debug.MsgBox("!ByPick1", iSelectStatus)
				Try
					oPolygon = oParcelsTopology.FindPolygon(tPoint)

					DMCommon.Debug.MsgBox("!ByPick22", tPoint, oPolygon.ID, mdicDataRows.Count)
					If mdicDataRows.TryGetValue(oPolygon.ID, iRowIndex) Then
						DMCommon.Debug.MsgBox("!ByPick3", iRowIndex)
						zzSetCurrentRow(iRowIndex)
					End If
				Catch oEx As Exception

				End Try

			End If

			Me.Visible = True
			oParcelsTopology.Close()

		End If

	End Sub

	Private Sub chkPaintingA_CheckedChanged(sender As System.Object, e As EventArgs) Handles chkPaintingA.CheckedChanged
		If Me.chkPaintingA.Checked Then
			'Dim iaCodes() As Integer = {17, 20, 21, 3, 25, 48, 24}
			Dim iaCodes() As Integer = {17, 272, 129, 49, 5, 145, 20, 24, 144, 12, 273}
			'	Dim shaColors() As Short = {150S, 130S, 84S, 21S, 220S, 191S, 226S}
			Dim shaColors() As Short = {150S, 130S, 84S, 21S, 220S, 191S, 226S, 232S, 41S, 177S, 91S}
			mdicColorsA = New Dictionary(Of Integer, Short)()
			For iIndex As Integer = 0 To iaCodes.GetUpperBound(0)
				mdicColorsA.Add(iaCodes(iIndex), shaColors(iIndex))
			Next
			miPaintingType = enPaintingType.Solid
		Else
			miPaintingType = enPaintingType.Default
		End If
	End Sub

End Class