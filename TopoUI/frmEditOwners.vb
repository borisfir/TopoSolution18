Option Explicit On
Option Strict On
Imports System.Data
Public Class frmEditOwners
	Private Structure OwnerRow
		Public OwnerID As Integer
		Public OwnerIndex As Integer
		Public RowIndex As Integer
		Public Sub New(iOwnerID As Integer, iOwnerIndex As Integer, iRowIndex As Integer)
			OwnerID = iOwnerID
			OwnerIndex = iOwnerIndex
			RowIndex = iRowIndex
		End Sub
	End Structure
	Private Structure CheckText
		Const msIP As String = "י.פ."
		Const msPage As String = "עמ'"

		Dim chkBox As CheckBox
		Dim chkBoxAdd As CheckBox

		Dim txtBoxA As MaskedTextBox
		Dim txtBoxB As MaskedTextBox
		Public Sub New(oCheckBox As CheckBox)
			chkBox = oCheckBox
		End Sub
		Public Sub New(oCheckBox As CheckBox, oCheckBoxAdd As CheckBox)
			chkBox = oCheckBox
			chkBoxAdd = oCheckBoxAdd

		End Sub
		Public Sub New(oCheckBox As CheckBox, oTextBoxA As MaskedTextBox, oTextBoxB As MaskedTextBox)
			chkBox = oCheckBox
			txtBoxA = oTextBoxA
			txtBoxB = oTextBoxB

		End Sub
		Public Sub New(oCheckBox As CheckBox, oCheckBoxAdd As CheckBox, oTextBoxA As MaskedTextBox, oTextBoxB As MaskedTextBox)
			chkBox = oCheckBox
			chkBoxAdd = oCheckBoxAdd
			txtBoxA = oTextBoxA
			txtBoxB = oTextBoxB

		End Sub
		Public Function Checked() As Boolean
			Try
				If chkBoxAdd Is Nothing Then
					Return chkBox.Checked
				Else
					Return chkBoxAdd.Checked OrElse chkBox.Checked
				End If

			Catch oEx As Exception
				DMCommon.Debug.ExcelLog.SetNextValue(0, "Err chkBox", oEx.Message)
			End Try

		End Function
		Public ReadOnly Property CheckName As String
			Get
				Return chkBox.Name
			End Get
		End Property
		Public ReadOnly Property FieldName As String
			Get
				Return chkBox.Name
			End Get
		End Property
		Public ReadOnly Property PropertyName As String
			Get
				If chkBox.DataBindings.Count = 1 Then
					Return chkBox.DataBindings.Item(0).PropertyName
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public ReadOnly Property BindingField As String
			Get
				If chkBox.DataBindings.Count = 1 Then
					'Dim oBindingSource As System.Windows.Forms.BindingSource = TryCast(chkBox.DataBindings.Item(0).DataSource, System.Windows.Forms.BindingSource)
					Return chkBox.DataBindings.Item(0).BindingMemberInfo.BindingField
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public ReadOnly Property BindingMember As String
			Get
				If chkBox.DataBindings.Count = 1 Then
					'Dim oBindingSource As System.Windows.Forms.BindingSource = TryCast(chkBox.DataBindings.Item(0).DataSource, System.Windows.Forms.BindingSource)
					Return chkBox.DataBindings.Item(0).BindingMemberInfo.BindingMember
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public Sub ReadValue()
			chkBox.DataBindings.Item(0).ReadValue()
		End Sub

		Public Function GetText() As String


			Dim sRes As String = chkBox.Text
			If sRes Is Nothing Then
				DMCommon.Debug.MsgBox("13_127f", chkBox.Name)
			End If

			If txtBoxA IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtBoxA.Text) Then
				sRes &= " " & msIP & " " & txtBoxA.Text
			End If
			If txtBoxB IsNot Nothing AndAlso Not String.IsNullOrEmpty(txtBoxB.Text) Then
				sRes &= " " & msPage & " " & txtBoxB.Text
			End If
			Return sRes
		End Function
	End Structure

	Public Const msDelim As String = ", "
	Private miProjectCode As Integer
	Private miDetailNo As Integer

	'Private moPrjOwnersTable As System.Data.DataTable
	Private moParcelRowView As DataRowView
	Private moMainDataView As System.Data.DataView
	Private WithEvents moMainDataTable As System.Data.DataTable

	Private moMainDataAdapter As Data.Common.DbDataAdapter
	Private miBlockNo As Integer
	Private miBlockAddNo As Integer
	Private miParcelNo As Integer
	Private WithEvents moBoundTable As DataTable
	'Private moBoundDataView As DataView
	Private moSumDataView As DataView
	Private mhsOwners As HashSet(Of Integer)
	Private mhsOwnersWithField As HashSet(Of Integer)

	Private mdDecTotal As Double
	Private mdDecCount As Integer
	Private mdaTerms() As Double
	Private miaRowIndecis() As Integer
	Private mdMaxDeviation As Double
	Private miMaxDeviationIndex As Integer
	Private moSimpleTotal As DMCommon.IntMat.Fraction = Nothing
	Private mhsOwnersBefore As HashSet(Of Integer)
	Private mbOwnersInvalid As Boolean
	Private miNewOwnerIndex As Integer
	Private mbEventsEnabled As Boolean = False
	Private mbDirty As Boolean
	Private moaCheckText() As CheckText '
	Private mbSplitData As Boolean
	'Private mdicSumRows As Dictionary(Of Integer, Integer) = New Dictionary(Of Integ")er, Integer)()
	'Private miCurrentOwnerID As Integer
	'Private miCurrentOwnerRowIndex As Integer

	Public Sub New(bSplitData As Boolean)
		mbSplitData = bSplitData
		' This call is required by the designer.
		InitializeComponent()


		' Add any initialization after the InitializeComponent() call.
		zzMyInitializeComponent(bSplitData)

	End Sub

	Private Sub zzMyInitializeComponent(bSplitData As Boolean)
		If bSplitData Then
			Dim sComText As String = "Select TOP (100) PERCENT OwnerID, OwnerName, ISNULL(OwnerShortName, OwnerName) AS DispName
 FROM dbo.OwnersList WHERE IsPart= 1 ORDER BY Priority"
			Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "OwnerList")
			Me.dgvMain.AutoGenerateColumns = False
			Me.ccbOwner.ValueMember = "OwnerID"
			Me.ccbOwner.DisplayMember = "DispName"
			Me.ccbOwner.DataSource = oDataTable
			Me.cchSum.FalseValue = 0
			Me.cchSum.TrueValue = 1
		End If

		moaCheckText = {New CheckText(Me.chkParagraph19, chkParagraph19AreaExists, Me.mskParagraph19IP, Me.mskParagraph19Page) _
													 , New CheckText(Me.chkParagraph5, Me.mskParagraph5IP, Me.mskParagraph5Page) _
													 , New CheckText(Me.chkParagraph123, Me.mskParagraph123IP, Me.mskParagraph123Page) _
													 , New CheckText(Me.chkRegulation29, Me.mskRegulation29IP, Me.mskRegulation29Page) _
													 , New CheckText(Me.chkParagraph126) _
													 , New CheckText(Me.chkSharedHouse) _
													 , New CheckText(Me.chkAntiqueSite) _
													 , New CheckText(Me.chkLeasing, chkLeasingAreaExists) _
													 , New CheckText(Me.chkMortgage) _
													 , New CheckText(Me.chkForeclosure) _
													 , New CheckText(Me.chkVerdict) _
													 , New CheckText(Me.chkRoadOrdinance) _
													 , New CheckText(Me.chkDemolitionOrder) _
													 , New CheckText(Me.chkParagraph11a)} _

		'zzInitNote()

	End Sub
	Private Sub zzIniOwners()
		Dim sComText As String = "Select TOP (100) PERCENT OwnerID, OwnerName FROM dbo.OwnersList WHERE IsPart=1 ORDER BY Priority"
		Dim oDataTable As DataTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, CommandType.Text, "OwnerList")
		Me.dgvMain.AutoGenerateColumns = False
		Me.ccbOwner.ValueMember = "OwnerID"
		Me.ccbOwner.DisplayMember = "OwnerName"

		Me.ccbOwner.DataSource = oDataTable

		Me.cchSum.FalseValue = 0
		Me.cchSum.TrueValue = 1


	End Sub
	Public Sub InitNote()
		'Dim moInquiryDateBinding As Binding

		'	moIssueDateBinding = New Binding("Value", Me.bnsMain, "IssueDate", True) ', DataSourceUpdateMode.Never, DBNull.Value, "d")
		'	AddHandler moIssueDateBinding.Parse, AddressOf zzParseDateTimePicker
		'	AddHandler moIssueDateBinding.Format, AddressOf zzFormatDateTimePicker



		Dim oNoteEditedBinding As Binding
		Dim oParagraph19Binding As Binding
		Dim oParagraph5Binding As Binding
		Dim oParagraph123Binding As Binding
		Dim oRegulation29Binding As Binding

		Dim oParagraph126Binding As Binding

		Dim oSharedHouseBinding As Binding
		Dim oAntiqueSiteBinding As Binding

		Dim oLeasingBinding As Binding

		Dim oMortgageBinding As Binding

		Dim oForeclosureBinding As Binding

		Dim oVerdictSiteBinding As Binding

		Dim oRoadOrdinanceBinding As Binding
		Dim oDemolitionOrderBinding As Binding
		Dim oParagraph11aBinding As Binding
		'Dim oVerdictSiteBinding As Binding







		Const sCheckedLabel As String = "V"

		'	Me.txtNote.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Note", False, System.Windows.Forms.DataSourceUpdateMode.OnValidation, Nothing, Nothing))

		Me.txtNote.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Note"))

		oNoteEditedBinding = New Binding("Checked", Me.bnsParcel, "NoteEdited")
		AddHandler oNoteEditedBinding.Format, AddressOf zzFormatCheckBox
		Me.chkNoteEdited.DataBindings.Add(oNoteEditedBinding)

		oParagraph19Binding = New Binding("Checked", Me.bnsParcel, "NoteEdited")
		AddHandler oNoteEditedBinding.Format, AddressOf zzFormatCheckBox
		Me.chkParagraph19.DataBindings.Add(oParagraph19Binding)
		Me.mskParagraph19IP.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph19IP"))
		Me.mskParagraph19Page.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph19Page"))



		oParagraph5Binding = New Binding("Checked", Me.bnsParcel, "Paragraph5")
		AddHandler oParagraph5Binding.Format, AddressOf zzFormatCheckBox
		Me.chkParagraph5.DataBindings.Add(oParagraph5Binding)
		Me.mskParagraph5IP.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph5IP"))
		Me.mskParagraph5Page.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph5Page"))

		oParagraph123Binding = New Binding("Checked", Me.bnsParcel, "Paragraph123")
		AddHandler oParagraph123Binding.Format, AddressOf zzFormatCheckBox
		Me.chkParagraph123.DataBindings.Add(oParagraph123Binding)
		Me.mskParagraph123IP.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph123IP"))
		Me.mskParagraph123Page.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Paragraph123Page"))

		oRegulation29Binding = New Binding("Checked", Me.bnsParcel, "Regulation29")
		AddHandler oRegulation29Binding.Format, AddressOf zzFormatCheckBox
		Me.chkRegulation29.DataBindings.Add(oRegulation29Binding)
		Me.mskRegulation29IP.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Regulation29IP"))
		Me.mskRegulation29Page.DataBindings.Add(New Binding("Text", Me.bnsParcel, "Regulation29Page"))

		oParagraph126Binding = New Binding("Checked", Me.bnsParcel, "Paragraph126")
		AddHandler oParagraph126Binding.Format, AddressOf zzFormatCheckBox
		Me.chkParagraph126.DataBindings.Add(oParagraph126Binding)

		oSharedHouseBinding = New Binding("Checked", Me.bnsParcel, "SharedHouse")
		AddHandler oSharedHouseBinding.Format, AddressOf zzFormatCheckBox
		Me.chkSharedHouse.DataBindings.Add(oSharedHouseBinding)

		oAntiqueSiteBinding = New Binding("Checked", Me.bnsParcel, "AntiqueSite")
		AddHandler oAntiqueSiteBinding.Format, AddressOf zzFormatCheckBox
		Me.chkAntiqueSite.DataBindings.Add(oAntiqueSiteBinding)
		'	Me.chkAntiqueSite.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "AntiqueSite"))

		oLeasingBinding = New Binding("Checked", Me.bnsParcel, "Leasing")
		AddHandler oLeasingBinding.Format, AddressOf zzFormatCheckBox
		Me.chkLeasing.DataBindings.Add(oLeasingBinding)
		'Me.chkLeasing.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Leasing"))

		oMortgageBinding = New Binding("Checked", Me.bnsParcel, "Mortgage")
		AddHandler oMortgageBinding.Format, AddressOf zzFormatCheckBox
		Me.chkMortgage.DataBindings.Add(oMortgageBinding)
		'Me.chkMortgage.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Mortgage"))

		oForeclosureBinding = New Binding("Checked", Me.bnsParcel, "Foreclosure")
		AddHandler oForeclosureBinding.Format, AddressOf zzFormatCheckBox
		Me.chkForeclosure.DataBindings.Add(oForeclosureBinding)
		'	Me.chkForeclosure.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Foreclosure"))

		oVerdictSiteBinding = New Binding("Checked", Me.bnsParcel, "Verdict")
		AddHandler oVerdictSiteBinding.Format, AddressOf zzFormatCheckBox
		Me.chkVerdict.DataBindings.Add(oVerdictSiteBinding)
		'	Me.chkVerdict.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "Verdict"))

		oRoadOrdinanceBinding = New Binding("Checked", Me.bnsParcel, "RoadOrdinance")
		AddHandler oRoadOrdinanceBinding.Format, AddressOf zzFormatCheckBox
		Me.chkRoadOrdinance.DataBindings.Add(oRoadOrdinanceBinding)
		'Me.chkRoadOrdinance.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "RoadOrdinance"))

		oDemolitionOrderBinding = New Binding("Checked", Me.bnsParcel, "DemolitionOrder")
		AddHandler oDemolitionOrderBinding.Format, AddressOf zzFormatCheckBox
		Me.chkDemolitionOrder.DataBindings.Add(oDemolitionOrderBinding)
		'	Me.chkDemolitionOrder.DataBindings.Add(New Binding("Checked", Me.bnsParcel, "DemolitionOrder"))

		oParagraph11aBinding = New Binding("Checked", Me.bnsParcel, "Paragraph11a")
		AddHandler oParagraph11aBinding.Format, AddressOf zzFormatCheckBox
		Me.chkParagraph11a.DataBindings.Add(oParagraph11aBinding)
		'	Me.chkParagraph11a.DataBindings.Add(NewoParagraph11aBindingBinding("Checked", Me.bnsParcel, "Paragraph11a"))



		If DMCommon.Functions.CDblN(moParcelRowView.Item("Paragraph19Area")) > 0.0 Then
			Me.chkParagraph19AreaExists.Checked = True
			Me.chkParagraph19AreaExists.Text = sCheckedLabel

		End If

		If DMCommon.Functions.CDblN(moParcelRowView.Item("LeasingArea")) > 0.0 Then
			Me.chkLeasingAreaExists.Checked = True
			Me.chkLeasingAreaExists.Text = sCheckedLabel

		End If


	End Sub

	Public Property ParcelRowView As System.Data.DataRowView
		Get
			Return moParcelRowView
		End Get
		Set(oValue As System.Data.DataRowView)
			moParcelRowView = oValue
			Me.bnsParcel.DataSource = moParcelRowView 'moMainData

		End Set
	End Property


	Public Property MainDataView As System.Data.DataView
		Get
			Return moMainDataView
		End Get
		Set(oValue As System.Data.DataView)
			moMainDataView = oValue
			moMainDataTable = moMainDataView.Table
		End Set
	End Property
	Public Property MainDataAdapter As Data.Common.DbDataAdapter
		Get
			Return moMainDataAdapter
		End Get
		Set(oValue As Data.Common.DbDataAdapter)
			moMainDataAdapter = oValue
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
	Public Property OwnersWithField As HashSet(Of Integer)
		Get
			Return mhsOwnersWithField
		End Get
		Set(oValue As HashSet(Of Integer))
			mhsOwnersWithField = oValue
		End Set

	End Property
	Public ReadOnly Property Owners As HashSet(Of Integer)
		Get
			Return mhsOwners
		End Get
	End Property

	Public Sub AddOwners(hsOwnersBefore As HashSet(Of Integer))
		mhsOwnersBefore = hsOwnersBefore
	End Sub
	Private Sub zzFormatCheckBox(oSender As System.Object, e As System.Windows.Forms.ConvertEventArgs)
		Dim oBinding As Binding = DirectCast(oSender, Binding)
		Dim oCheckBox As CheckBox = DirectCast(oBinding.Control, CheckBox)
		Dim bChecked As Boolean

		If IsDBNull(e.Value) Then
			oCheckBox.Checked = False
		Else
			bChecked = DirectCast(e.Value, Boolean)
			oCheckBox.Checked = bChecked
		End If
	End Sub

	Private ReadOnly Property Paragraph19AreaExists As Boolean
		Get
			Return (DMCommon.Functions.CDblN(moParcelRowView.Item("Paragraph19Area")) > 0.0)

		End Get

	End Property
	Private ReadOnly Property LeasingAreaExists As Boolean
		Get
			Return (DMCommon.Functions.CDblN(moParcelRowView.Item("LeasingArea")) > 0.0)

		End Get

	End Property

	Private Sub zzSetText()
		Me.txtBlockNo.Text = miBlockNo.ToString()
		If miBlockAddNo <> 0 Then
			Me.txtBlockAddNo.Text = miBlockAddNo.ToString()
		End If

		Me.txtParcelNo.Text = miParcelNo.ToString()


	End Sub

	Private Sub zzFillAddData()

		Dim oSourceRow As DataRowView
		'	Dim oBoundRow As DataRow
		Dim iOwnerID As Integer = 0
		Dim iOwnerIndex As Integer = 0

		Dim iRowIndex As Integer = 0
		Dim iPrevOwnerIndex As Integer = 0
		Dim iPrevOwnerID As Integer = 0
		Dim iPrevRowIndex As Integer = 0
		Dim colOwnerRows As System.Collections.ObjectModel.Collection(Of OwnerRow) = New ObjectModel.Collection(Of OwnerRow)()

		mhsOwners = New HashSet(Of Integer)()
		moMainDataView.Sort = "OwnerID,RowIndex"
		miNewOwnerIndex = 0
		'DMCommon.Debug.MsgBox("13_128kk", mbSplitData, "zzFillAddData")
		For iIndex As Integer = 0 To moMainDataView.Count - 1

			oSourceRow = moMainDataView.Item(iIndex)
			'	iOwnerIndex = DMCommon.Functions.CIntN(oSourceRow.Item("OwnerIndex"))
			iRowIndex = DMCommon.Functions.CIntN(oSourceRow.Item("RowIndex"))
			iOwnerID = DMCommon.Functions.CIntN(oSourceRow.Item("OwnerID"))

			If iRowIndex = 1 Then
				oSourceRow.Item("OwnerDisp") = iOwnerID
				If mhsOwners.Contains(iOwnerID) Then
					DMCommon.Debug.UserMsg("Err #1711", iOwnerID)
				Else
					mhsOwners.Add(iOwnerID)
				End If
				iPrevOwnerIndex = iOwnerIndex
				iOwnerIndex += 1
			Else
				'iPrevOwnerID = DMCommon.Functions.CIntN(oSourceRow.Item("OwnerID"))
				'iPrevOwnerIndex = DMCommon.Functions.CIntN(oSourceRow.Item("OwnerIndex"))
			End If
			oSourceRow.Item("OwnerIndex") = iOwnerIndex
			'DMCommon.Debug.MsgBox("13_128mm", mbSplitData, "zzFillAddData")
			If iPrevRowIndex > 1 AndAlso iPrevOwnerID <> iOwnerID Then
				colOwnerRows.Add(New OwnerRow(iPrevOwnerID, iPrevOwnerIndex, iPrevRowIndex + 1))
			End If
			iPrevOwnerID = iOwnerID
			iPrevRowIndex = iRowIndex
		Next
		miNewOwnerIndex = iOwnerIndex + 1

		For Each tOwnerRow As OwnerRow In colOwnerRows
			zzAddNewRow(tOwnerRow.OwnerID, tOwnerRow.OwnerIndex, tOwnerRow.RowIndex, 0)
		Next
		For Each iOwnerBeforeID As Integer In mhsOwnersBefore
			If Not mhsOwners.Contains(iOwnerBeforeID) Then
				zzAddNewRow(iOwnerBeforeID, miNewOwnerIndex, 1, 0, True)
				mhsOwners.Add(iOwnerBeforeID)
				miNewOwnerIndex += 1
			End If

		Next
		'''''''''''''''''Me.dgvMain.DataSource = moBoundTable
		'	Me.dgvMain.DataSource = moBoundTable
		moMainDataView.Sort = "OwnerIndex,RowIndex"
		'DMCommon.Debug.MsgBox("13_128nn", "zzFillAddDataEnd")
	End Sub


	Private Sub zzAddNewRow(iOwnerID As Integer, iOwnerIndex As Integer, iRowIndex As Integer, iDivisor As Long, Optional bOwnerDisp As Boolean = False)
		Dim oNewDataRowView As DataRowView = moMainDataView.AddNew

		zzFillNewRow(oNewDataRowView, iOwnerID, iOwnerIndex, iRowIndex)
		'	oNewDataRowView.Item("OwnerIndex") = iOwnerIndex
		'	oNewDataRowView.Item("RowIndex") = iRowIndex
		If iDivisor > 0 Then
			oNewDataRowView.Item("Divisor") = iDivisor
		End If
		If bOwnerDisp Then
			oNewDataRowView.Item("OwnerDisp") = iOwnerID
		End If
		oNewDataRowView.EndEdit()

	End Sub
	Private Sub zzClearSource()
		Dim oSourceRow As DataRowView
		'DMCommon.Debug.MsgBox("13_127D", "before delete", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
		For iIndex As Integer = moMainDataView.Count - 1 To 0 Step -1

			oSourceRow = moMainDataView.Item(iIndex)
			oSourceRow.Delete()

		Next
	End Sub
	Private Sub zzLoad()
		'DMCommon.Debug.MsgBox("13_127H", "Start of Load")
		If mbSplitData Then


			zzInitUD_Project()

			zzSetText()
			'zzCreateBoundTable()
			'zzFillBoundTable()

			zzFillAddData()

			zzSetDataViews()

			zzSetSumNew()
		End If

		'	DMCommon.Debug.MsgBox("13_127F", "End of Load", DMCommon.Debug.ColCount(moBoundTable), DMCommon.Debug.ColCount(moMainDataView))
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

		moBoundTable.Columns.Add("Dividend", System.Type.GetType("System.Int64"))
		moBoundTable.Columns.Add("Divisor", System.Type.GetType("System.Int64"))
		moBoundTable.Columns.Add("Sum", System.Type.GetType("System.Boolean"))
		moBoundTable.Columns.Add("RowType", System.Type.GetType("System.Int32"))
		moBoundTable.Columns.Add("Source", System.Type.GetType("System.String"))
		Dim oPrimaryKey() As DataColumn = {oOwnerIndexColumn, oRowIndexColumn}
		moBoundTable.PrimaryKey = oPrimaryKey



	End Sub
	Private Sub zzSetDataViews()
		'''''''''''''''''''	moBoundDataView = New DataView(moMainTable, String.Empty, "OwnerIndex,RowIndex", DataViewRowState.CurrentRows)
		'''''''''''Me.dgvMain.DataSource = moBoundDataView
		''''''''''''''moSumDataView = New DataView(moBoundTable, "RowIndex=1", String.Empty, DataViewRowState.CurrentRows)
		'DMCommon.Debug.MsgBox("13_132B", moMainDataView.Count)
		Me.dgvMain.DataSource = moMainDataView
		'	DMCommon.Debug.MsgBox("13_132A", moMainDataView.Count)
		Dim sFilter As String = moMainDataView.RowFilter
		If String.IsNullOrEmpty(sFilter) Then
			sFilter = "RowIndex=1"
		Else

			sFilter &= " And RowIndex=1"
		End If

		moSumDataView = New DataView(moMainDataView.Table, sFilter, String.Empty, DataViewRowState.CurrentRows)

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



	Private Sub frmEditOwners_Load(oSender As System.Object, e As EventArgs) Handles Me.Load

		zzLoad()
		mbEventsEnabled = True
	End Sub


	Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
		If False Then
			'zzClearSource()

			'zzUpdate()
		End If
		For Each oRow As DataRowView In moMainDataView
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!Ok", oRow.Row.RowState, oRow.Item("OwnerID"))
		Next
		Dim sTest As String = "I"
		zzUpdateOwnersSet()
		zzDeleteSingleRow()

		Try
			moMainDataAdapter.Update(moMainDataView.Table)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, Me.Name & "_1")
		End Try


		mbDirty = False
	End Sub
	Private Sub zzDeleteSingleRow()
		'	DMCommon.Debug.MsgBox("13_120", moMainDataView.Count)
		If moMainDataView IsNot Nothing AndAlso moMainDataView.Count = 1 Then
			Dim oRow As DataRowView = moMainDataView.Item(0)
			Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			'If mhsOwnersWithField.Contains(iOwnerID) Then
			oRow.Row.Delete()
			'	End If
		End If

	End Sub
	Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
		DMCommon.Debug.ExcelLog.SetNextValue(0, "EndEd01", e.RowIndex, e.ColumnIndex)
		'	DMCommon.Debug.MsgBox("13_127EndEdit", e.RowIndex, e.ColumnIndex, mbEventsEnabled, moMainDataView.Count)
		If mbEventsEnabled AndAlso e.RowIndex >= 0 AndAlso e.RowIndex < moMainDataView.Count Then
			Dim oRow As DataRowView = moMainDataView.Item(e.RowIndex)
			Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			'	DMCommon.Debug.MsgBox("13_127EndEdit", e.RowIndex, e.ColumnIndex)
			mbDirty = True
			If e.ColumnIndex = 0 Then
				iOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerDisp"))
				If Not mhsOwners.Contains(iOwnerID) Then
					oRow.Item("OwnerID") = iOwnerID
				End If
			End If
			If oRow.IsNew AndAlso e.ColumnIndex = 0 Then


				If iOwnerID = 0 OrElse mhsOwners.Contains(iOwnerID) Then
					DMCommon.Debug.UserMsg("Err #1276", iOwnerID)
				Else
					zzFillNewRow(oRow, iOwnerID, miNewOwnerIndex, 1)

					'	oRow.Item("OwnerIndex") = miNewOverLayIndex
					'	oRow.Item("RowIndex") = 1

					miNewOwnerIndex += 1
					oRow.EndEdit()
					mhsOwners.Add(iOwnerID)
				End If
			Else
				'iOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			End If

			Dim iDividend As Long = DMCommon.Functions.CLngN(oRow.Item("Dividend"))
			Dim iDivisor As Long = DMCommon.Functions.CLngN(oRow.Item("Divisor"))
			Dim dPartPct As Double = DMCommon.Functions.CDblN(oRow.Item("PartPct"))

			Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
			'	Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))

			Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))


			'DMCommon.Debug.MsgBox("13_127c", oRow(0), oRow(1), oRow(2), oRow(3), oRow(4), oRow(5), oRow(6), oRow(7), oRow(8))
			Select Case e.ColumnIndex
				Case 0
					If zzUpdateOwnersSet() Then
						DMCommon.Debug.MsgBox("Err #2512", DMCommon.Functions.CIntN(oRow.Item("OwnerID")))
					End If
				Case 3
					'If dPartPct <> 0.0 Then
					oRow.Item("Dividend") = DBNull.Value
					oRow.Item("Divisor") = DBNull.Value
					'End If
					If iRowIndex > 1 Then

						''''''''	zzCalcOwnerSum(iOwnerIndex)
					End If
					'	zzSetSum()
					zzSetSumNew()

					If iRowIndex > 1 Then
						'zzAddNewRow(iOwnerIndex, iRowIndex + 1)
						zzAddRowIfIsLastNew(e.RowIndex, iOwnerID, iOwnerIndex, iRowIndex, iDivisor)
					End If


				Case 1, 2
					If iDividend <> 0 AndAlso iDivisor <> 0 Then
						dPartPct = Math.Round(Convert.ToDouble(100L * iDividend) / Convert.ToDouble(iDivisor), 4, MidpointRounding.AwayFromZero)
						'DMCommon.Debug.MsgBox("13_127n", iDividend, iDivisor, dPartPct)
						oRow.Item("PartPct") = dPartPct
						oRow.EndEdit()

						If iRowIndex > 1 Then

							''''''''''''''	zzCalcOwnerSum(iOwnerIndex)
						End If
						'zzSetSum()
						zzSetSumNew()

						If iRowIndex > 1 Then

							'	zzAddRowIfIsLast(e.RowIndex, iOwnerIndex, iRowIndex, iDivisor)
							zzAddRowIfIsLastNew(e.RowIndex, iOwnerID, iOwnerIndex, iRowIndex, iDivisor)
						End If
					End If

				Case 4
					Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
					If bRowIsSum Then


						'DMCommon.Debug.MsgBox("13_127R", bRowIsSum, iOwnerID, iOwnerIndex, iRowIndex)
						zzAddNewRow(iOwnerID, iOwnerIndex, iRowIndex + 1, 0)
						''''''''''''''zzRefresh()
						'miCurrentOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
						'miCurrentOwnerRowIndex = e.RowIndex

					End If
					'	
					'

			End Select



		End If

	End Sub
	Private Sub zzFillNewRow(ByRef oRow As DataRowView, iOwnerID As Integer, iOwnerIndex As Integer, iRowIndex As Integer)

		oRow.Item("ProjectCode") = miProjectCode
		oRow.Item("Detail") = miDetailNo

		oRow.Item("BlockNo") = miBlockNo
		oRow.Item("BlockAddNo") = miBlockAddNo
		oRow.Item("ParcelNo") = miParcelNo

		oRow.Item("OwnerID") = iOwnerID
		oRow.Item("OwnerIndex") = iOwnerIndex
		oRow.Item("RowIndex") = iRowIndex
		oRow.Item("Sum") = 0

		''''''''''''''oRow.EndEdit()
	End Sub

	Private Function zzIsLast(iDataRowIndex As Integer, iObjectIndex As Integer) As Boolean
		Dim bResp As Boolean = False
		If iDataRowIndex + 1 = moMainDataView.Count Then
			bResp = True
		Else
			Dim oNextRow As DataRowView = moMainDataView.Item(iDataRowIndex + 1)
			If DMCommon.Functions.CIntN(oNextRow.Item("OwnerIndex")) <> iObjectIndex Then
				bResp = True
			End If
		End If

		Return bResp
	End Function
	Private Sub zzAddRowIfIsLastNew(iDataRowIndex As Integer, iOwnerID As Integer, iOwnerIndex As Integer, iRowIndex As Integer, iDivisor As Long)
		Dim bAdd As Boolean = False
		If iDataRowIndex + 1 = moMainDataView.Count Then
			bAdd = True
		Else
			Dim oNextRow As DataRowView = moMainDataView.Item(iDataRowIndex + 1)
			If DMCommon.Functions.CIntN(oNextRow.Item("OwnerIndex")) <> iOwnerIndex Then
				bAdd = True
			End If
		End If
		If bAdd Then
			'	zzAddNewRow(iObjectIndex, iObjectRowIndex + 1, iDivisor)
			zzAddNewRow(iOwnerID, iOwnerIndex, iRowIndex + 1, iDivisor)
		End If

	End Sub
	Private Sub zzAddRowIfIsLast(iDataRowIndex As Integer, iObjectIndex As Integer, iObjectRowIndex As Integer, iDivisor As Integer)
		Dim bAdd As Boolean = False
		If iDataRowIndex + 1 = moMainDataView.Count Then
			bAdd = True
		Else
			Dim oNextRow As DataRowView = moMainDataView.Item(iDataRowIndex + 1)
			If DMCommon.Functions.CIntN(oNextRow.Item("OwnerIndex")) <> iObjectIndex Then
				bAdd = True
			End If
		End If
		If bAdd Then
			'	zzAddNewRow(iObjectIndex, iObjectRowIndex + 1, iDivisor)

		End If

	End Sub
	Private Function zzCalcPartSumDown(iRowIndex As Integer, iOwnerIndex As Integer) As Double
		Dim oRow As DataRowView
		Dim dResSum As Double = 0.0
		Do While iRowIndex < moMainDataView.Count - 1
			oRow = moMainDataView.Item(iRowIndex)
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
			oRow = moMainDataView.Item(iRowIndex)
			If iOwnerIndex = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex")) Then
				'	DMCommon.Debug.MsgBox("13_127All", iRowIndex, iOwnerIndex, dResSum)
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

		For Each oRow As DataRowView In moMainDataView
			iOwnerID = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
			DMCommon.Debug.ExcelLog.SetNextValue(0, "-RowValidA", mhsOwners.Count)
			If mhsOwners.Contains(iOwnerID) Then
				mbOwnersInvalid = True
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!RowValid_T", mhsOwners.Count, mbOwnersInvalid)
				Return True
			Else
				mhsOwners.Add(iOwnerID)
			End If


		Next
		Dim iTest As Integer
		mbOwnersInvalid = False
		If mhsOwners.Count = 1 Then
			iTest = mhsOwners.ElementAt(0)

		End If
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!RowValid_F", mhsOwners.Count, iTest, mbOwnersInvalid)
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
			'''''''''''''''''''	zzCalcOwnerSum(iOwnerIndex)
			'	zzSetSum()
			zzSetSumNew()

		End If

	End Sub
	Private Function zzGetSumAAA(dDecSum As Double, oSimpleSum As DMCommon.IntMat.Fraction, Optional iOwnerID As Integer = -1) As Double
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
	Private Function zzGetSumNew(Optional iSkipOwnerID As Integer = -1, Optional iSkipRowIndex As Integer = -1) As Double
		Dim dPartPct As Double
		Dim dDeviation As Double
		Dim oFraction As DMCommon.IntMat.Fraction = Nothing

		'	Dim oTotalFraction As DMCommon.IntMat.Fraction = Nothing
		Dim oSimpleOwner As DMCommon.IntMat.Fraction = Nothing
		'	Dim dTotalPartPct As Double
		Dim dOwnerPartPct As Double
		Dim iOwnerCount As Integer
		Dim oRow As DataRowView
		Dim bSimpleIsProper As Boolean = True
		Dim iRowIndex As Integer
		Dim bIsSum As Boolean
		Dim iRowsUB As Integer = moMainDataView.Count - 1
		DMCommon.Debug.ExcelLog.SetNextValue(0, "------", "------", moMainDataView.Count, iSkipOwnerID, "------", "------")
		mdDecTotal = 0.0
		mdDecCount = 0
		ReDim mdaTerms(iRowsUB)
		ReDim miaRowIndecis(iRowsUB)
		mdMaxDeviation = 0.0
		miMaxDeviationIndex = -1

		moSimpleTotal = Nothing
		For iIndex As Integer = iRowsUB To 0 Step -1
			oRow = moMainDataView.Item(iIndex)
			If iSkipOwnerID = -1 OrElse (iSkipOwnerID <> DMCommon.Functions.CIntN(oRow.Item("OwnerID"))) OrElse (iSkipRowIndex <> DMCommon.Functions.CIntN(oRow.Item("RowIndex"))) Then
				zzGetRow(oRow, iRowIndex, bIsSum, dPartPct, dDeviation, oFraction)


				DMCommon.Debug.ExcelLog.SetNextValue(0, "!SumBef", moMainDataView.Count - 1, iIndex, iRowIndex, bIsSum, dPartPct, zzTestFraction(oFraction))
				If iRowIndex > 1 Then


					zzAddFraction(oSimpleOwner, oFraction)
					dOwnerPartPct += dPartPct
					iOwnerCount += 1
					zzFillTerms(iIndex, dPartPct)
					If dDeviation > mdMaxDeviation Then
						mdMaxDeviation = dDeviation
						miMaxDeviationIndex = iIndex
					End If
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!Row>1", moMainDataView.Count - 1, iIndex, iRowIndex, bIsSum, dPartPct, zzTestFraction(oFraction), zzTestFraction(oSimpleOwner))
				ElseIf iRowIndex = 1 Then
					If bIsSum Then
						mbEventsEnabled = False
						If iOwnerCount = 0 Then
							oRow.Item("Sum") = 0
							zzAddFraction(moSimpleTotal, oFraction)
							mdDecTotal += dPartPct
							zzFillTerms(iIndex, dPartPct)
							If dDeviation > mdMaxDeviation Then
								mdMaxDeviation = dDeviation
								miMaxDeviationIndex = iIndex
							End If
						Else
							oRow.Item("PartPct") = dOwnerPartPct
							mdDecTotal += dOwnerPartPct

							If oSimpleOwner IsNot Nothing Then


								zzAddFraction(moSimpleTotal, oSimpleOwner)
								oRow.Item("Dividend") = oSimpleOwner.Numerator
								oRow.Item("Divisor") = oSimpleOwner.IntDenominator

							Else
								oRow.Item("Dividend") = DBNull.Value
								oRow.Item("Divisor") = DBNull.Value
							End If


						End If

						mbEventsEnabled = True
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!IsSum", moMainDataView.Count - 1, iIndex, iRowIndex, bIsSum, dPartPct, zzTestFraction(oSimpleOwner))
						dOwnerPartPct = 0.0
						iOwnerCount = 0
						oSimpleOwner = Nothing
					Else


						zzAddFraction(moSimpleTotal, oFraction)
						mdDecTotal += dPartPct
						zzFillTerms(iIndex, dPartPct)
						If dDeviation > mdMaxDeviation Then
							mdMaxDeviation = dDeviation
							miMaxDeviationIndex = iIndex
						End If
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!Current", moMainDataView.Count - 1, iIndex, iRowIndex, bIsSum, dPartPct, zzTestFraction(moSimpleTotal), zzTestFraction(oFraction))
					End If

				End If




			End If

		Next



	End Function
	Private Sub zzFillTerms(iIndex As Integer, dPartPct As Double)
		mdaTerms(mdDecCount) = dPartPct
		miaRowIndecis(mdDecCount) = iIndex
		mdDecCount += 1
	End Sub
	Private Sub zzAddFraction(ByRef oBase As DMCommon.IntMat.Fraction, oAddition As DMCommon.IntMat.Fraction)
		If oAddition IsNot Nothing Then
			If oBase Is Nothing Then
				oBase = oAddition.Clone
			Else
				oBase += oAddition
			End If
		End If
	End Sub
	Private Function zzTestFraction(oFraction As DMCommon.IntMat.Fraction) As String
		If oFraction Is Nothing Then
			Return "FNothing"
		Else
			Return "'" & oFraction.ToString()
		End If
	End Function
	Private Sub zzGetRow(oRow As DataRowView, ByRef iRowIndex As Integer, ByRef bIsSum As Boolean, ByRef dPartPct As Double, ByRef dDeviation As Double, ByRef oFraction As DMCommon.IntMat.Fraction)
		Dim iDividend, iDivisor As Long
		iRowIndex = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
		bIsSum = DMCommon.Functions.CBoolN(oRow.Item("Sum"))


		dPartPct = DMCommon.Functions.CDblN(oRow.Item("PartPct"))

		iDividend = DMCommon.Functions.CLngN(oRow.Item("Dividend"))
		iDivisor = DMCommon.Functions.CLngN(oRow.Item("Divisor"))

		If iDivisor <> 0L AndAlso iDividend <> 0L Then
			dDeviation = Math.Abs(iDividend / iDivisor - dPartPct)
			oFraction = New DMCommon.IntMat.Fraction(iDividend, iDivisor)
			If Me.chkReduction.Checked Then
				oFraction.Reduction()
			End If
		Else
			oFraction = Nothing
		End If




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
		For Each oRow As DataRowView In moMainDataView
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
		'DMCommon.Debug.MsgBox("13_127down", iRowIndex, iOwnerIndex, dSum)
		zzCalcPartSumUp(iRowIndex - 1, iOwnerIndex, dSum)

	End Sub
	Private Sub zzSetSum_AAA()
		Dim dSum As Double = zzGetSum() 'zzCalcSum()
		zzFormatSum(dSum)
	End Sub
	Private Sub zzSetSumNew()
		zzGetSumNew()

		zzFormatSumNew()
		'Me.txtSumSimple.Text = oSimpleSum.ToString()

	End Sub

	Private Sub zzFormatSumNew()
		Dim tColor As Color

		If moSimpleTotal Is Nothing Then
			Me.txtSumSimple.Text = String.Empty
			tColor = Color.FromKnownColor(KnownColor.Control)
		Else
			If moSimpleTotal.IsOne Then
				tColor = Color.LightGreen
				If Math.Abs(mdDecTotal - 100.0) > 0.00005 Then
					zzDecBalance()
					mdDecTotal = 100.0

				End If

			Else
				tColor = Color.Salmon
			End If
			'255, 192, 192
			If Me.chkReduction.Checked Then
				moSimpleTotal.Reduction()
			End If
			Me.txtSumSimple.Text = moSimpleTotal.ToString()
				'	Me.DialogResult = DialogResult.OK
			End If
			Me.txtSumSimple.BackColor = tColor
		Me.txtSumDec.Text = FormatNumber(mdDecTotal, 4, TriState.False)
	End Sub

	Private Sub zzDecBalance()
		'DMCommon.Debug.MsgBox("13_122d", miMaxDeviationIndex, mdMaxDeviation)
		If miMaxDeviationIndex >= 0 Then
			Dim oRow As DataRowView = moMainDataView.Item(miMaxDeviationIndex)
			Dim dPartPct As Double = DMCommon.Functions.CDblN(oRow.Item("PartPct"))

			oRow.Item("PartPct") = dPartPct + 100.0 - mdDecTotal
		End If



	End Sub
	Private Sub zzFormatSum(dSum As Double)

		Me.txtSumDec.Text = FormatNumber(dSum, 4, TriState.False)
	End Sub
	Private Sub cmdComplete_Click_251219(oSender As System.Object, e As EventArgs) 'Handles cmdComplete.Click
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
				zzSetSum_AAA()
			End If

		End If





	End Sub

	Private Sub dgvMain_CellValueChanged(oSender As System.Object, e As DataGridViewCellEventArgs) 'Handles dgvMain.CellValueChanged
		If e.RowIndex >= 0 AndAlso e.RowIndex < moMainDataView.Count Then
			Dim oRow As DataRowView = moMainDataView.Item(e.RowIndex)


			'	DMCommon.Debug.MsgBox("13_127CellValueChanged")
			Select Case e.ColumnIndex


				Case 444
					Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
					'DMCommon.Debug.MsgBox("13_127s", bRowIsSum)
					Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))
					Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))


			End Select
		End If
	End Sub
	Private Function zzRowIndexIsProper(iRowIndex As Integer) As Boolean
		Return iRowIndex >= 0 AndAlso iRowIndex < moMainDataView.Count
	End Function

	Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
		Dim sTest As String = "a"
		If mbEventsEnabled Then


			If zzRowIndexIsProper(e.RowIndex) Then
				Dim oRow As DataRowView = moMainDataView.Item(e.RowIndex)
				sTest = "ax"
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "?BeginEdit", e.RowIndex, e.ColumnIndex, oRow.IsNew, DMCommon.Functions.CIntN(oRow.Item("OwnerID")), e.Cancel, mbOwnersInvalid)
				If oRow.IsNew AndAlso (DMCommon.Functions.CIntN(oRow.Item("OwnerID")) = 0) OrElse mbOwnersInvalid Then
					sTest = "ax1"
					If e.ColumnIndex <> 0 Then
						sTest = "b"
						e.Cancel = True
					End If
				Else
					sTest = "ay1"
					'Dim iOwnerID As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerID"))
					Select Case e.ColumnIndex
						Case 0
							Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
							sTest = "ay2"
							If iRowIndex > 1 Then
								sTest = "c"
								e.Cancel = True
							End If


						Case 1, 2, 3
							Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
							sTest = "ay3"
							If bRowIsSum Then
								sTest = "d"
								e.Cancel = True
							End If
						Case 4
							Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("RowIndex"))
							sTest = "ay4"
							If iRowIndex = 1 Then
								Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oRow.Item("Sum"))
								Dim iOwnerIndex As Integer = DMCommon.Functions.CIntN(oRow.Item("OwnerIndex"))

								If bRowIsSum And Not zzIsLast(e.RowIndex, iOwnerIndex) Then
									sTest = "e"
									e.Cancel = True
								End If

							Else
								sTest = "f"
								e.Cancel = True
							End If
					End Select
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BegEditA", e.RowIndex, e.ColumnIndex, oRow.IsNew, DMCommon.Functions.CIntN(oRow.Item("OwnerID")), e.Cancel, mbOwnersInvalid, sTest)
				End If


			ElseIf e.ColumnIndex <> 0 Then
				'DMCommon.Debug.MsgBox("13_127BeginEdit", e.RowIndex, e.ColumnIndex, moMainDataView.Count)
				e.Cancel = True

			End If
		End If
	End Sub





	Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
		If Not mbDirty Then
			Me.Close()
		End If
		Me.Close()
	End Sub

	Private Sub dgvMain_RowValidating(oSender As System.Object, e As DataGridViewCellCancelEventArgs)
		If mbOwnersInvalid Then
			e.Cancel = True
		End If
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!Validating", e.Cancel, mhsOwners.Count, mbOwnersInvalid)
	End Sub

	Private Sub dgvMain_DataError1(oSender As System.Object, e As DataGridViewDataErrorEventArgs)
		DMCommon.Debug.MsgBox("DataError #2160", e.RowIndex.ToString() & ":" & e.ColumnIndex.ToString(), e.Exception.Message, e.Exception.StackTrace)
		e.Cancel = True
	End Sub



	Private Sub dgvMain_PreviewKeyDown(oSender As System.Object, e As PreviewKeyDownEventArgs)
		If e.KeyCode = Keys.Escape Then
			'DMCommon.Debug.MsgBox("13_127u", "PreviewKeyDown")
			zzAfterUndo()
		End If
	End Sub



	Private Sub frmEditOwners_FormClosing(oSender As System.Object, e As FormClosingEventArgs) Handles Me.FormClosing
		zzDeleteSingleRow()

		Return
		If e.CloseReason = CloseReason.UserClosing AndAlso mbDirty Then

			e.Cancel = True

		End If

	End Sub

	Private Sub cmdCancel_Click(oSender As System.Object, e As EventArgs) Handles cmdCancel.Click
		If mbDirty Then
			mbDirty = False
			For Each oRow As DataRowView In moMainDataView
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Cancel", oRow.Row.RowState, oRow.Item("OwnerID"))
				oRow.Row.RejectChanges()
			Next
			Me.Close()
		End If
	End Sub

	Private Sub cmdComplete_Click(oSender As System.Object, e As EventArgs) Handles cmdComplete.Click

		Dim oDataRowView As DataRowView
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		'Dim dSum As Double
		Dim dRes As Double
		Dim oComplementary As DMCommon.IntMat.Fraction = Nothing
		Dim iOwnerID As Integer
		Dim iRowIndex As Integer

		'	Dim iDivisor As Integer
		If oGridRow IsNot Nothing AndAlso Not oGridRow.IsNewRow AndAlso moMainDataView.Count > 1 Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			DMCommon.Debug.MsgBox("181222_1", oGridRow.IsNewRow, oDataRowView.IsNew, oDataRowView.Row.RowState)
			iOwnerID = DMCommon.Functions.CIntN(oDataRowView.Item("OwnerID"))
			iRowIndex = DMCommon.Functions.CIntN(oDataRowView.Item("RowIndex"))

			zzGetSumNew(iOwnerID, iRowIndex)
			If mdDecTotal <= 100.0 Then
				dRes = 100.0 - mdDecTotal
				Me.txtSumDec.Text = "100.00"
			Else
				dRes = 0.0

			End If

			oDataRowView.Item("PartPct") = dRes
			mbEventsEnabled = False
			If moSimpleTotal IsNot Nothing Then
				oComplementary = moSimpleTotal.AdditionToOne()
				If oComplementary IsNot Nothing Then
					oDataRowView.Item("Dividend") = oComplementary.Numerator

					oDataRowView.Item("Divisor") = oComplementary.IntDenominator
					'DMCommon.Debug.MsgBox("13_348d", oComplementary.Numerator, oComplementary.IntDenominator)
				End If
			End If
			oDataRowView.EndEdit()
			mbEventsEnabled = True
			zzSetSumNew()
		End If
	End Sub

	Private Sub dgvMain_UserDeletingRow(oSender As System.Object, e As DataGridViewRowCancelEventArgs)
		Dim oGridRow As DataGridViewRow = e.Row


		If oGridRow IsNot Nothing AndAlso Not oGridRow.IsNewRow Then
			Dim oDataRowView As DataRowView
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			Dim bRowIsSum As Boolean = DMCommon.Functions.CBoolN(oDataRowView.Item("Sum"))
			Dim bRowIsEmpty As Boolean = (DMCommon.Functions.CDblToIntN(oDataRowView.Item("PartPct"), 1.0) = 0)
			Dim iRowIndex As Integer = DMCommon.Functions.CIntN(oDataRowView.Item("RowIndex"))
			If bRowIsSum OrElse (bRowIsEmpty AndAlso iRowIndex > 1) Then
				e.Cancel = True
			End If
		Else
			e.Cancel = True
		End If




	End Sub

	Private Sub dgvMain_UserDeletedRow(oSender As System.Object, e As DataGridViewRowEventArgs)
		zzSetSumNew()
	End Sub
	Private Sub chkReduction_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkReduction.CheckedChanged
		zzSetSumNew()
	End Sub

	Private Sub zzGetCheckValue(chkBoxPlus As CheckText, ByRef sResult As String)
		chkBoxPlus.ReadValue()
		Dim bCheckValue As Boolean = DMCommon.Functions.CBoolN(moParcelRowView.Item(chkBoxPlus.BindingField))
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "zzGetCheckValue", chkBoxPlus.CheckName, chkBoxPlus.PropertyName, chkBoxPlus.BindingField, chkBoxPlus.BindingMember, bCheckValue, chkBoxPlus.Checked, chkBoxPlus.GetText())
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzGetCheckValue", chkBoxPlus.Checked)
		bCheckValue = chkBoxPlus.Checked
		If bCheckValue Then  'chkBoxPlus.Checked
			If Not String.IsNullOrEmpty(sResult) Then
				sResult &= msDelim
			End If
			sResult &= chkBoxPlus.GetText()
		End If

	End Sub

	Private Sub cmdCalc_Click(oSender As System.Object, e As EventArgs) Handles cmdCalc.Click
		If Not Me.chkNoteEdited.Checked Then
			mbEventsEnabled = False
			Me.txtNote.Text = NoteCalc()
			Me.txtNote.Focus()
			mbEventsEnabled = True
		End If

	End Sub

	Public Function NoteCalc() As String
		Dim sResult As String = String.Empty
		'DMCommon.Debug.MsgBox("13_128k", moaCheckText.GetUpperBound(0), "Bef NoteCalc")
		For iIndex As Integer = 0 To moaCheckText.GetUpperBound(0)
			zzGetCheckValue(moaCheckText(iIndex), sResult)
		Next
		'	DMCommon.Debug.MsgBox("13_128L", sResult, "NoteCalc")
		Return sResult
	End Function


	Private Sub txtNote_Validated(oSender As System.Object, e As EventArgs) Handles txtNote.TextChanged
		If mbEventsEnabled AndAlso Not Me.chkNoteEdited.Checked Then
			Me.chkNoteEdited.Checked = True
		End If
	End Sub


	Private Sub dgvMain_DataError(sender As System.Object, e As DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
		e.ThrowException = False
		e.Cancel = True
	End Sub



	Private Sub moMainDataTable_TableNewRow(sender As System.Object, e As DataTableNewRowEventArgs) Handles moMainDataTable.TableNewRow
		Dim oNewRow As DataRow = e.Row

		oNewRow.Item("ProjectCode") = miProjectCode
		oNewRow.Item("Detail") = miDetailNo

		oNewRow.Item("BlockNo") = miBlockNo
		oNewRow.Item("BlockAddNo") = miBlockAddNo
		oNewRow.Item("ParcelNo") = miParcelNo

	End Sub

	Private Sub Button2_Click(sender As System.Object, e As EventArgs)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.CurrentRow
		Dim oDataRowView As DataRowView



		'	Dim iDivisor As Integer
		If oGridRow IsNot Nothing Then
			oDataRowView = DirectCast(oGridRow.DataBoundItem, DataRowView)
			DMCommon.Debug.MsgBox("181222_1", oGridRow.IsNewRow, oDataRowView.IsNew, oDataRowView.Row.RowState)
		End If
	End Sub
End Class