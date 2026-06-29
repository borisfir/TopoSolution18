Option Explicit On
Option Strict On
Imports System.Data
Public Class frmEditOwners_231219
	Private miProjectCode As Integer
	Private miDetailNo As Integer

	'Private moPrjOwnersTable As System.Data.DataTable
	Private moMainDataView As System.Data.DataView

	Private miBlockNo As Integer
	Private miBlockAddNo As Integer
	Private miParcelNo As Integer
	Private WithEvents moBoundTable As DataTable
	Private moBoundDataView As DataView
	Private moSumDataView As DataView
	Private mhsOwners As HashSet(Of Integer)
	Private mbOwnersInvalid As Boolean
	Private miNewOverLayIndex As Integer
	Private mbDirty As Boolean
	'Private mdicSumRows As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
	'Private miCurrentOwnerID As Integer
	'Private miCurrentOwnerRowIndex As Integer

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent()
	End Sub

	Private Sub zzMyInitializeComponent()
		Dim sComText As String = "SELECT TOP (100) PERCENT OwnerID, OwnerName FROM dbo.OwnersList ORDER BY Priority"
		Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "OwnerList")
		Me.dgvMain.AutoGenerateColumns = False
		Me.ccbOwner.ValueMember = "OwnerID"
		Me.ccbOwner.DisplayMember = "OwnerName"

		Me.ccbOwner.DataSource = oDataTable

		Me.cchSum.FalseValue = 0
		Me.cchSum.TrueValue = 1

	End Sub
	Public Property MainDataView As System.Data.DataView
		Get
			Return moMainDataView
		End Get
		Set(oValue As System.Data.DataView)
			moMainDataView = oValue
		End Set
	End Property
	Public Property ParcelNo As Integer
		Get
			Return miParcelNo
		End Get
		Set(iValue As Integer)
			miParcelNo = iValue
		End Set
	End Property
	Public Property BlockNo As Integer
		Get
			Return miBlockNo
		End Get
		Set(iValue As Integer)
			miBlockNo = iValue
		End Set
	End Property
	Public Property BlockAddNo As Integer
		Get
			Return miBlockAddNo
		End Get
		Set(iValue As Integer)
			miBlockAddNo = iValue
		End Set

	End Property
	Private Sub zzSetText()
		Me.txtBlockNo.Text = miBlockNo.ToString()
		If miBlockAddNo <> 0 Then
			Me.txtBlockAddNo.Text = miBlockAddNo.ToString()
		End If

		Me.txtParcelNo.Text = miParcelNo.ToString()


	End Sub
	Private Sub zzFillBoundTable()

		Dim oSourceRow As DataRowView
		Dim oBoundRow As DataRow
		Dim iOwnerIndex As Integer = 1
		Dim iRowIndex As Integer = 1
		Dim dSum As Double
		Dim oFraction As DMCommon.IntMat.Fraction
		Dim oSumFraction As DMCommon.IntMat.Fraction = Nothing

		Dim iDividend As Integer
		Dim iDivisor As Integer

		mhsOwners = New HashSet(Of Integer)()
		miNewOverLayIndex = 0
		For iIndex As Integer = 0 To moMainDataView.Count - 1
			oBoundRow = moBoundTable.NewRow()
			oSourceRow = moMainDataView.Item(iIndex)

			oBoundRow.Item("OwnerIndex") = iOwnerIndex
			oBoundRow.Item("RowIndex") = iRowIndex

			oBoundRow.Item("OwnerID") = oSourceRow.Item("OwnerID")
			oBoundRow.Item("PartPct") = oSourceRow.Item("PartPct")
			oBoundRow.Item("Dividend") = oSourceRow.Item("Dividend")
			iDividend = DMCommon.Functions.CIntN(oSourceRow.Item("Dividend"))
			oBoundRow.Item("Divisor") = oSourceRow.Item("Divisor")
			iDivisor = DMCommon.Functions.CIntN(oSourceRow.Item("Divisor"))
			moBoundTable.Rows.Add(oBoundRow)
			dSum += DMCommon.Functions.CDblN(oBoundRow.Item("PartPct"))
			mhsOwners.Add(DMCommon.Functions.CIntN(oBoundRow.Item("OwnerID")))
			If iDividend <> 0 AndAlso iDivisor <> 0 Then
				oFraction = New DMCommon.IntMat.Fraction(iDividend, iDivisor)
				If oSumFraction Is Nothing Then
					oSumFraction = oFraction
				Else
					oSumFraction += oFraction
				End If

			End If


		Next
		miNewOverLayIndex = iOwnerIndex
		zzFormatSumNew(dSum, oSumFraction)


		'''''''''''''''''Me.dgvMain.DataSource = moBoundTable
		'	Me.dgvMain.DataSource = moBoundTable

	End Sub
	Private Sub zzAddNewRow(iOwnerIndex As Integer, iRowIndex As Integer, iDivisor As Integer)
		Dim oNewDataRowView As DataRowView = moBoundDataView.AddNew
		oNewDataRowView.Item("OwnerIndex") = iOwnerIndex
		oNewDataRowView.Item("RowIndex") = iRowIndex
		If iDivisor > 0 Then
			oNewDataRowView.Item("Divisor") = iDivisor
		End If
		oNewDataRowView.EndEdit()

	End Sub
	Private Sub zzClearSource()
		Dim oSourceRow As DataRowView
		DMCommon.Debug.MsgBox("13_127d", "before delete", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
		For iIndex As Integer = moMainDataView.Count - 1 To 0 Step -1

			oSourceRow = moMainDataView.Item(iIndex)
			oSourceRow.Delete()

		Next
	End Sub
	Private Sub zzLoad()
		zzInitUD_Project()
		zzSetText()
		zzCreateBoundTable()
		zzFillBoundTable()
		zzSetDataViews()
		'DMCommon.Debug.MsgBox("13_127f", "Load", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
	End Sub
	Private Sub zzCreateBoundTable()
		Dim oOwnerIndexColumn As DataColumn
		Dim oRowIndexColumn As DataColumn

		moBoundTable = New System.Data.DataTable("Data")
		oOwnerIndexColumn = New DataColumn("OwnerIndex", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add(oOwnerIndexColumn)
		oRowIndexColumn = New DataColumn("RowIndex", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add(oRowIndexColumn)


		moBoundTable.Columns.Add("OwnerID", System.Type.GetType("System.Int32"))

		moBoundTable.Columns.Add("PartPct", System.Type.GetType("System.Double"))

		moBoundTable.Columns.Add("Dividend", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Divisor", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Sum", System.Type.GetType("System.Boolean"))
		moBoundTable.Columns.Add("RowType", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Source", System.Type.GetType("System.String"))
		Dim oPrimaryKey() As DataColumn = {oOwnerIndexColumn, oRowIndexColumn}
		moBoundTable.PrimaryKey = oPrimaryKey



	End Sub
	Private Sub zzSetDataViews()
		moBoundDataView = New DataView(moBoundTable, String.Empty, "OwnerIndex,RowIndex", DataViewRowState.CurrentRows)
		Me.dgvMain.DataSource = moBoundDataView
		moSumDataView = New DataView(moBoundTable, "RowIndex=1", String.Empty, DataViewRowState.CurrentRows)
	End Sub
	Private Sub zzInitUD_Project()
		'TopoManager.TPlanGraph.TplnProject.InitializeProjectDB_SQL()
		'TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(TplnProject.ServerDataSource, ProjectDataBase, True)

		miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
		miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo

	End Sub

	Private Sub zzUpdate()
		'	DMCommon.Debug.MsgBox("13_127c", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
		Dim oNewDataRowView As DataRowView
		For Each oBoundRow As DataRowView In moSumDataView

			oNewDataRowView = moMainDataView.AddNew

			oNewDataRowView.Item("ProjectCode") = miProjectCode
			oNewDataRowView.Item("Detail") = miDetailNo
			oNewDataRowView.Item("BlockNo") = miBlockNo
			oNewDataRowView.Item("BlockAddNo") = miBlockAddNo
			oNewDataRowView.Item("ParcelNo") = miParcelNo


			oNewDataRowView.Item("OwnerID") = oBoundRow.Item("OwnerID")
			oNewDataRowView.Item("PartPct") = oBoundRow.Item("PartPct")
			oNewDataRowView.Item("Dividend") = oBoundRow.Item("Dividend")
			oNewDataRowView.Item("Divisor") = oBoundRow.Item("Divisor")
			oNewDataRowView.EndEdit()

		Next
		'	DMCommon.Debug.MsgBox("13_127e", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
	End Sub

	Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles Button1.Click
		'	DMCommon.ExcelLogAW1.SetNextValue(0, "------------------")
		'	DMCommon.ExcelLogAW1.SetDataTable(moMainDataView, 0)
		'zzRefresh()
	End Sub

	Private Sub frmEditOwners_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
		zzLoad()
	End Sub

	Private Sub txtBlockNo_TextChanged(oSender As System.Object, e As EventArgs) Handles txtBlockNo.TextChanged

	End Sub

	Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
		zzClearSource()

		zzUpdate()
		mbDirty = False
	End Sub

	Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
		If e.RowIndex >= 0 AndAlso e.RowIndex < moBoundDataView.Count Then
			Dim oRow As DataRowView = moBoundDataView.Item(e.RowIndex)
			Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			'	DMCommon.Debug.MsgBox("13_127EndEdit", e.RowIndex, e.ColumnIndex)
			mbDirty = True

			If oRow.IsNew AndAlso e.ColumnIndex = 0 Then
				iOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
				If mhsOwners.Contains(iOwnerID) Then
					DMCommon.Debug.UserMsg("Error", iOwnerID)
				Else


					oRow.Item("OwnerIndex") = miNewOverLayIndex
					oRow.Item("RowIndex") = 1

					miNewOverLayIndex += 1
					oRow.EndEdit()
					mhsOwners.Add(iOwnerID)
				End If

			End If


			Dim iDividend As Integer = DMCommon.Functions.CIntN(oRow.Item("Dividend"))
			Dim iDivisor As Integer = DMCommon.Functions.CIntN(oRow.Item("Divisor"))
			Dim dPartPct As Double = DMCommon.Functions.CDblN(oRow.Item("PartPct"))

			Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
			'	Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))

			'DMCommon.Debug.MsgBox("13_127c", oRow(0), oRow(1), oRow(2), oRow(3), oRow(4), oRow(5), oRow(6), oRow(7), oRow(8))
			Select Case e.ColumnIndex
				Case 0
					If zzUpdateOwnersSet() Then
						DMCommon.Debug.MsgBox("Error", DMCommon.Functions.CIntN(oRow.Item("OwnerID")))
					End If
				Case 3
					'If dPartPct <> 0.0 Then
					oRow.Item("Dividend") = DBNull.Value
					oRow.Item("Divisor") = DBNull.Value
					'End If
					If iRowIndex > 1 Then
						'zzSetPartSum(e.RowIndex, iOwnerID, dPartPct)
						zzCalcOwnerSum(iOwnerIndex)
					End If
					'	zzSetSum()
					zzSetSumNew()

					If iRowIndex > 1 Then
						'zzAddNewRow(iOwnerIndex, iRowIndex + 1)
						zzAddRowIfIsLast(e.RowIndex, iOwnerIndex, iRowIndex, iDivisor)
					End If


				Case 1, 2
					If iDividend <> 0 AndAlso iDivisor <> 0 Then
						dPartPct = Convert.ToDouble(100 * iDividend) / Convert.ToDouble(iDivisor)
						'DMCommon.Debug.MsgBox("13_127n", iDividend, iDivisor, dPartPct)
						oRow.Item("PartPct") = dPartPct
						oRow.EndEdit()

						If iRowIndex > 1 Then
							'zzSetPartSum(e.RowIndex, iOwnerID, dPartPct)
							zzCalcOwnerSum(iOwnerIndex)
						End If
						'zzSetSum()
						zzSetSumNew()

						If iRowIndex > 1 Then
							'zzAddNewRow(iOwnerIndex, iRowIndex + 1)
							zzAddRowIfIsLast(e.RowIndex, iOwnerIndex, iRowIndex, iDivisor)

						End If
					End If

				Case 4
					Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
					If bRowIsSum Then


						'	DMCommon.Debug.MsgBox("13_127r", bRowIsSum, iOwnerIndex, iRowIndex)
						zzAddNewRow(iOwnerIndex, iRowIndex + 1, 0)
						''''''''''''''zzRefresh()
						'miCurrentOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
						'miCurrentOwnerRowIndex = e.RowIndex

					End If
					'	
					'

			End Select



		End If
	End Sub

	Private Function zzIsLast(iDataRowIndex As Integer, iObjectIndex As Integer) As Boolean
		Dim bResp As Boolean = False
		If iDataRowIndex + 1 = moBoundDataView.Count Then
			bResp = True
		Else
			Dim oNextRow As DataRowView = moBoundDataView.Item(iDataRowIndex + 1)
			If DMCommon.Functions.CIntN(oNextRow.Item("OwnerIndex")) <> iObjectIndex Then
				bResp = True
			End If
		End If

		Return bResp
	End Function
	Private Sub zzAddRowIfIsLast(iDataRowIndex As Integer, iObjectIndex As Integer, iObjectRowIndex As Integer, iDivisor As Integer)
		Dim bAdd As Boolean = False
		If iDataRowIndex + 1 = moBoundDataView.Count Then
			bAdd = True
		Else
			Dim oNextRow As DataRowView = moBoundDataView.Item(iDataRowIndex + 1)
			If DMCommon.Functions.CIntN(oNextRow.Item("OwnerIndex")) <> iObjectIndex Then
				bAdd = True
			End If
		End If
		If bAdd Then
			zzAddNewRow(iObjectIndex, iObjectRowIndex + 1, iDivisor)
		End If

	End Sub
	Private Function zzCalcPartSumDown(iRowIndex As Integer, iOwnerIndex As Integer) As Double
		Dim oRow As DataRowView
		Dim dResSum As Double = 0.0
		Do While iRowIndex < moBoundDataView.Count - 1
			oRow = moBoundDataView.Item(iRowIndex)
			If iOwnerIndex = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex")) Then
				dResSum += DMCommon.Functions.CDblN(oRow.Item("PartPct"))
				iRowIndex += 1
			Else
				Exit Do
			End If
		Loop
		Return dResSum
	End Function
	Private Sub zzCalcPartSumUp(iRowIndex As Integer, iOwnerIndex As Integer, dResSum As Double)
		Dim oRow As DataRowView

		Do
			oRow = moBoundDataView.Item(iRowIndex)
			If iOwnerIndex = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex")) Then
				DMCommon.Debug.MsgBox("13_127All", iRowIndex, iOwnerIndex, dResSum)
				If DMCommon.Functions.CIntN(oRow.Item("RowIndex")) = 1 Then
					oRow.Item("PartPct") = dResSum
				Else
					dResSum += DMCommon.Functions.CDblN(oRow.Item("PartPct"))
					iRowIndex -= 1
				End If


			Else
				Exit Do
			End If
		Loop While iRowIndex >= 0

	End Sub
	Private Function zzUpdateOwnersSet() As Boolean 'TRue - Error
		Dim iOwnerID As Integer
		mhsOwners.Clear()

		For Each oRow As DataRowView In moSumDataView
			iOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			If mhsOwners.Contains(iOwnerID) Then
				mbOwnersInvalid = True
				Return True
			Else
				mhsOwners.Add(iOwnerID)
			End If


		Next
		mbOwnersInvalid = False
		Return False


	End Function
	Private Sub zzAfterUndo()
		zzUpdateOwnersSet()
		Dim oDataRowView As DataRowView
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow

		Dim iOwnerIndex As Integer

		If oGridRow IsNot Nothing AndAlso Not oGridRow.IsNewRow Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			iOwnerIndex = DMCommon.Functions.CIntN(oDataRowView.Item("OwnerIndex"))
			zzCalcOwnerSum(iOwnerIndex)
			'	zzSetSum()
			zzSetSumNew()

		End If

	End Sub
	Private Function zzGetSum(dDecSum As Double, oSimpleSum As DMCommon.IntMat.Fraction, Optional iOwnerID As Integer = -1) As Double
		Dim dPartPct As Double
		Dim dSum As Double

		For Each oRow As DataRowView In moSumDataView
			If iOwnerID = -1 OrElse iOwnerID <> DMCommon.Functions.CIntN(oRow.Item("OwnerID")) Then
				dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))
				dSum += dPartPct
			End If

		Next
		'DMCommon.Debug.MsgBox("13_127SUM", iDividend, iDivisor, dPartPct)
		Return dSum

	End Function
	Private Function zzGetSumNew(ByRef dDecSum As Double, ByRef oSimpleSum As DMCommon.IntMat.Fraction, Optional iOwnerID As Integer = -1) As Double
		Dim dPartPct As Double
		Dim oFraction As DMCommon.IntMat.Fraction = Nothing

		DMCommon.Debug.MsgBox("13_128b", iOwnerID, moSumDataView.Count)
		For Each oRow As DataRowView In moSumDataView
			If iOwnerID = -1 OrElse iOwnerID <> DMCommon.Functions.CIntN(oRow.Item("OwnerID")) Then
				zzGetRow(oRow, dPartPct, oFraction)
				If oFraction Is Nothing Then
					DMCommon.Debug.MsgBox("13_128c", iOwnerID)
				Else
					DMCommon.Debug.MsgBox("13_128d", dPartPct, oFraction.DecVaue)
				End If
				dDecSum += dPartPct
				If oSimpleSum Is Nothing Then
					oSimpleSum = oFraction
				Else
					oSimpleSum += oFraction
				End If
			End If

		Next



	End Function
	Private Sub zzGetRow(oRow As DataRowView, ByRef dPartPct As Double, ByRef oFraction As DMCommon.IntMat.Fraction)
		Dim iDividend, iDivisor As Integer
		dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))
		iDividend = DMCommon.Functions.CIntN(oRow.Item("Dividend"))
		iDivisor = DMCommon.Functions.CIntN(oRow.Item("Divisor"))
		oFraction = New DMCommon.IntMat.Fraction(iDividend, iDivisor)



	End Sub

	Private Function zzGetSum(Optional iOwnerID As Integer = -1) As Double
		Dim dPartPct As Double
		Dim dSum As Double

		For Each oRow As DataRowView In moSumDataView
			If iOwnerID = -1 OrElse iOwnerID <> DMCommon.Functions.CIntN(oRow.Item("OwnerID")) Then
				dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))
				dSum += dPartPct
			End If

		Next
		'DMCommon.Debug.MsgBox("13_127SUM", iDividend, iDivisor, dPartPct)
		Return dSum

	End Function
	Private Function zzCalcOwnerSum(iOwnerIndex As Integer) As Double
		Dim oaKeyValue() As System.Object = {iOwnerIndex, 1}
		Dim oSumRow As DataRow = moBoundTable.Rows.Find(oaKeyValue)
		If oSumRow IsNot Nothing Then
			Dim sFilter As String = "(OwnerIndex=" & iOwnerIndex.ToString() & ") AND (RowIndex<>1)"
			Dim oOwnerDataView As DataView = New DataView(moBoundTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
			Dim dPartPct As Double
			Dim dSum As Double = 0.0

			For Each oRow As DataRowView In oOwnerDataView
				dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))
				dSum += dPartPct

			Next
			oSumRow.Item("PartPct") = dSum
			oSumRow.EndEdit()



		End If


	End Function
	Private Function zzCalcSum(Optional iOwnerID As Integer = -1) As Double
		Dim dPartPct As Double
		Dim iRowIndex As Integer
		Dim bRowIsSum As Boolean
		Dim dSum As Double
		Dim dPartSum As Double
		Dim oOwnerRow As DataRowView
		For Each oRow As DataRowView In moBoundDataView
			iRowIndex = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
			If iRowIndex = 1 Then
				bRowIsSum = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
				If bRowIsSum Then
					dPartSum = 0.0
					oOwnerRow = oRow
				Else
					If iOwnerID = -1 OrElse iOwnerID <> DMCommon.Functions.CIntN(oRow.Item("OwnerID")) Then
						dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))
						dSum += dPartPct
					End If
				End If
			Else

			End If

		Next
		Return dSum

	End Function
	Private Sub zzSetPartSum(iRowIndex As Integer, iOwnerIndex As Integer, dSum As Double)
		dSum += zzCalcPartSumDown(iRowIndex + 1, iOwnerIndex)
		DMCommon.Debug.MsgBox("13_127down", iRowIndex, iOwnerIndex, dSum)
		zzCalcPartSumUp(iRowIndex - 1, iOwnerIndex, dSum)

	End Sub
	Private Sub zzSetSum()
		Dim dSum As Double = zzGetSum() 'zzCalcSum()
		zzFormatSum(dSum)
	End Sub
	Private Sub zzSetSumNew()
		Dim dDecSum As Double
		Dim oSimpleSum As DMCommon.IntMat.Fraction = Nothing
		zzGetSumNew(dDecSum, oSimpleSum)
		zzFormatSumNew(dDecSum, oSimpleSum)
		'Me.txtSumSimple.Text = oSimpleSum.ToString()

	End Sub

	Private Sub zzFormatSumNew(dDecSum As Double, oSimpleSum As DMCommon.IntMat.Fraction)

		Me.txtSumDec.Text = FormatNumber(dDecSum, 4, TriState.False)
		If oSimpleSum Is Nothing Then
			Me.txtSumSimple.Text = String.Empty
		Else
			Me.txtSumSimple.Text = oSimpleSum.ToString()
		End If
	End Sub

	Private Sub zzFormatSum(dSum As Double)

		Me.txtSumDec.Text = FormatNumber(dSum, 4, TriState.False)
	End Sub
	Private Sub cmdComplete_Click(oSender As System.Object, e As EventArgs) Handles cmdComplete.Click
		Dim oDataRowView As DataRowView
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim dSum As Double
		Dim dRes As Double
		Dim iOwnerID As Integer
		Dim iDivisor As Integer
		If oGridRow IsNot Nothing AndAlso Not oGridRow.IsNewRow Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			iOwnerID = DMCommon.Functions.CIntN(oDataRowView.Item("OwnerID"))
			dSum = zzGetSum(iOwnerID)
			If dSum <= 100.0 Then
				dRes = 100.0 - dSum
				Me.txtSumDec.Text = "100.00"
			Else
				dRes = 0.0
				zzFormatSum(dSum)
			End If
			oDataRowView.Item("PartPct") = dRes
			iDivisor = DMCommon.Functions.CIntN(oDataRowView.Item("Divisor"))
			If iDivisor > 0 Then
				oDataRowView.Item("Dividend") = CInt(iDivisor * 0.01 * dRes)
			End If
			oDataRowView.EndEdit()

			If dRes = 0.0 Then
				zzSetSum()
			End If

		End If





	End Sub

	Private Sub dgvMain_CellValueChanged(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellValueChanged
		If e.RowIndex >= 0 AndAlso e.RowIndex < moBoundDataView.Count Then
			Dim oRow As DataRowView = moBoundDataView.Item(e.RowIndex)


			'	DMCommon.Debug.MsgBox("13_127CellValueChanged")
			Select Case e.ColumnIndex


				Case 44
					Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
					'DMCommon.Debug.MsgBox("13_127s", bRowIsSum)
					Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))
					Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))


			End Select
		End If
	End Sub

	Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
		'Dim sTest As String = "Init"
		'If e.RowIndex >= 0 AndAlso e.RowIndex < moBoundDataView.Count Then
		'sTest = moBoundDataView.Item(e.RowIndex).IsNew.ToString()
		'End If


		If e.RowIndex >= 0 AndAlso e.RowIndex < moBoundDataView.Count Then
			Dim oRow As DataRowView = moBoundDataView.Item(e.RowIndex)
			If oRow.IsNew AndAlso (DMCommon.Functions.CIntN(oRow.Item("OwnerID")) = 0) OrElse mbOwnersInvalid Then
				If e.ColumnIndex <> 0 Then
					e.Cancel = True
				End If
			Else
				'Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
				Select Case e.ColumnIndex
					Case 0
						Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
						If iRowIndex > 1 Then
							e.Cancel = True
						End If


					Case 1, 2, 3
						Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
						If bRowIsSum Then
							e.Cancel = True
						End If
					Case 4
						Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
						If iRowIndex = 1 Then
							Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
							Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))

							If bRowIsSum And Not zzIsLast(e.RowIndex, iOwnerIndex) Then
								e.Cancel = True
							End If

						Else
							e.Cancel = True
						End If





				End Select
			End If


		ElseIf e.ColumnIndex <> 0 Then
			DMCommon.Debug.MsgBox("13_127BeginEdit", e.RowIndex, e.ColumnIndex, moBoundDataView.Count)
			e.Cancel = True

		End If

	End Sub





	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		If Not mbDirty Then
			Me.Close()
		End If

	End Sub

	Private Sub dgvMain_RowValidating(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.RowValidating
		If mbOwnersInvalid Then
			e.Cancel = True
		End If
	End Sub

	Private Sub dgvMain_DataError(oSender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
		DMCommon.Debug.MsgBox("DataError #2160", e.RowIndex, e.ColumnIndex, e.Exception.Message)
		e.Cancel = True
	End Sub



	Private Sub dgvMain_PreviewKeyDown(oSender As System.Object, e As PreviewKeyDownEventArgs) Handles dgvMain.PreviewKeyDown
		If e.KeyCode = Keys.Escape Then
			'	DMCommon.Debug.MsgBox("13_127u", "PreviewKeyDown")
			zzAfterUndo()
		End If
	End Sub



	Private Sub frmEditOwners_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing

		If e.CloseReason = CloseReason.UserClosing AndAlso mbDirty Then

			e.Cancel = True

		End If

	End Sub

	Private Sub cmdCancel_Click(oSender As System.Object, e As EventArgs) Handles cmdCancel.Click
		If mbDirty Then
			mbDirty = False
			Me.Close()
		End If

	End Sub

	Private Sub txtSum_TextChanged(oSender As System.Object, e As EventArgs) Handles txtSumDec.TextChanged

	End Sub
End Class