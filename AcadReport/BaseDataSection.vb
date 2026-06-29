Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Drawing
Public Structure mergeCol
   Dim iColNo As Integer
   Dim il As Integer
   Dim im As Integer
   Sub New(ByVal sValue As String)
      Dim saValue() As String = Strings.Split(sValue, ",")
      Try
         iColNo = Convert.ToInt32(saValue(0))
      Catch oEx As Exception
         Return
      End Try

   End Sub
End Structure
Public MustInherit Class BaseDataSection
	Inherits Section
	Protected dbRightToLeft As Boolean = True
   Protected doMainView As System.Data.DataView
   Protected doaDataType(BaseReport.ColumnUB) As System.Type
   Protected doaMergeColumns(BaseReport.ColumnUB) As DataMerge

   Protected diaDataColumns(BaseReport.ColumnUB) As Integer
   Protected dbDataColumnsExist As Boolean = False

   Private miRowLB_AAA As Integer
   Protected diFirstDataRow As Integer = 0
   Protected diDataRowCount As Integer = -1
   '   Private mdHeaderHeight As Double
   '  Private mdFooterHeight As Double
   '  Private mdReportHeight As Double
   '   Private mdMaxTableHeight As Double
	Private mbRepContinue As Boolean = False
	Public Sub New()

	End Sub
	Public Sub New(ByVal oMainView As System.Data.DataView, ByVal bRightToLeft As Boolean, Optional ByVal iaDataColumns() As Integer = Nothing)
		MyBase.New()

		doMainView = oMainView
		'	System.Windows.Forms.MessageBox.Show(CStr(doMainView.Count), "02_234")
		dbRightToLeft = bRightToLeft
		'	DMAcadExt.AcadDocument.WriteMessage("###101: " & doMainView.Item(0).Item(0).ToString() & "|" & doMainView.Item(0).Item(2).ToString())
		zzGetDataTypes()
		MyBase.diCellTypeResUB = -1
		MyBase.diCellTypeUB = -1


		MyBase.RowCount = doMainView.Count
		diDataRowCount = doMainView.Count
		MyBase.FirstRow = 0




		If iaDataColumns IsNot Nothing Then
			dbDataColumnsExist = True
			Dim iIndex As Integer = 0
			Do While iIndex <= Math.Min(BaseReport.ColumnUB, iaDataColumns.GetUpperBound(0))
				diaDataColumns(iIndex) = iaDataColumns(iIndex)
				iIndex += 1
			Loop
			Do While iIndex <= BaseReport.ColumnUB
				diaDataColumns(iIndex) = iIndex
				iIndex += 1
			Loop
		End If
	End Sub


	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, ByVal oMainView As System.Data.DataView, ByVal bRightToLeft As Boolean, ByVal bMergeRows As Boolean, Optional ByVal iaDataColumns() As Integer = Nothing)
		MyBase.New(oResource)
		doMainView = oMainView
		dbRightToLeft = bRightToLeft
		zzGetDataTypes()
		MyBase.RowCount = doMainView.Count
		MyBase.FirstRow = 0
		diDataRowCount = doMainView.Count
		'	DMCommon.Debug.MsgBox("13_126", bMergeRows, oResource.GetStrItem(2))
		If bMergeRows Then
			Dim saColMerges() As String = oResource.GetStrItem(2)
			'DMCommon.Functions.DispArray(saColMerges, "saColMerges", True)
			If saColMerges IsNot Nothing Then
				zzSetMergeColumns(saColMerges)
				'  zzDispMergeColumns("01_659 MergeColumns")
			End If
		End If


		If iaDataColumns IsNot Nothing Then
			dbDataColumnsExist = True

			Dim iIndex As Integer = 0
			Do While iIndex <= Math.Min(BaseReport.ColumnUB, iaDataColumns.GetUpperBound(0))
				diaDataColumns(iIndex) = iaDataColumns(iIndex)
				iIndex += 1
			Loop
			Do While iIndex <= BaseReport.ColumnUB
				diaDataColumns(iIndex) = iIndex
				iIndex += 1
			Loop

		End If

	End Sub

	Public Property MainView() As System.Data.DataView
		Get
			Return doMainView
		End Get
		Set(ByVal oValue As System.Data.DataView)
			doMainView = oValue
			MyBase.RowCount = doMainView.Count
		End Set
	End Property
	Public Property FirstDataRow() As Integer
		Get
			Return diFirstDataRow
		End Get
		Set(ByVal iValue As Integer)
			diFirstDataRow = iValue
		End Set
	End Property
	Public Property DataRowCount() As Integer
		Get
			Return diDataRowCount
		End Get
		Set(ByVal iValue As Integer)
			diDataRowCount = iValue
		End Set
	End Property

	Public Overrides Sub Format()
		'	System.Windows.Forms.MessageBox.Show(CStr(MyBase.diCellTypeUB) & ":" & CStr(BaseReport.ColumnUB), "01_191 BaseDataSect")
		If MyBase.diSectionID >= 0 AndAlso BaseReport.ColumnUB >= 0 Then
			MyBase.diCellTypeResUB = BaseReport.ColumnResUB
			MyBase.diCellTypeUB = BaseReport.ColumnUB	'- BaseReport.GroupColumnUB
			'	System.Windows.Forms.MessageBox.Show(CStr(MyBase.diCellTypeResUB) & ":" & CStr(MyBase.diCellTypeUB) & ":" & CStr(BaseReport.ColumnUB), "01_139a!! BaseDataSect")
			'	MyBase.diGroupColumnStart = BaseReport.GroupColumnStart
			'	MyBase.diGroupColumnUB = BaseReport.GroupColumnUB

			'	System.Windows.Forms.MessageBox.Show(CStr(diCellTypeResUB) & ":" & CStr(diCellTypeUB), "05_325 BaseD")
			MyBase.OnFormatting()
			'''''''''New 
		End If
	End Sub
	Private Sub zzSetMergeColumns(ByVal saColumns() As String)
		Dim iColumnIndex As Integer
		Dim oDataMerge As DataMerge
		DMCommon.Debug.ExcelLog.SetEnumerable(0, "!saColumns", saColumns)

		For iIndex As Integer = 0 To saColumns.GetUpperBound(0)
			oDataMerge = New DataMerge(saColumns(iIndex))
			If iIndex = 0 Then
				oDataMerge.First = True
			End If

			iColumnIndex = oDataMerge.ColumnNo
			If dbRightToLeft Then
				oDataMerge.Invert()
			End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "MergeColUB", BaseReport.ColumnUB, oDataMerge.ColumnNo)
			If oDataMerge.ColumnNo <= BaseReport.ColumnUB Then
				doaMergeColumns(oDataMerge.ColumnNo) = oDataMerge

				Dim oaDataMerge() As DataMerge = oDataMerge.GetSubMergeColumns()
				Dim iCurrentColumnNo As Integer
				Dim iCurrentColumnIndex As Integer
				DMCommon.Debug.ExcelLog.SetNextValue(0, "oaDataMerge.GetUpperBound(0)", oaDataMerge.GetUpperBound(0))
				For iSubColIndex As Integer = 0 To oaDataMerge.GetUpperBound(0)
					iCurrentColumnIndex = oaDataMerge(iSubColIndex).ColumnNo
					iCurrentColumnNo = iCurrentColumnIndex
					If doaMergeColumns(iCurrentColumnNo) Is Nothing Then
						doaMergeColumns(iCurrentColumnNo) = oaDataMerge(iSubColIndex)
					Else
						System.Windows.Forms.MessageBox.Show(CStr(iCurrentColumnIndex) & ":" & CStr(iCurrentColumnNo) & ":" & CStr(iSubColIndex), "BaseDataSection - zzSetMergeColumns")
					End If
				Next
			Else
				System.Windows.Forms.MessageBox.Show("BaseDataSection - zzSetMergeColumns")
			End If
		Next
	End Sub



	Public MustOverride Overrides Sub Print()




	Public Sub MergeReset()
		For iColIndex As Integer = 0 To BaseReport.ColumnUB
			If doaMergeColumns(iColIndex) IsNot Nothing Then
				doaMergeColumns(iColIndex).Reset()
			End If
		Next

	End Sub



	Private Sub zzGetDataTypes()
		Dim sTest As String = "a"
		If doMainView IsNot Nothing Then
			sTest = "b"
			Try
				Dim oTable As System.Data.DataTable = doMainView.Table
				sTest = "c"
				For iColIndex As Integer = 0 To BaseReport.ColumnUB
					sTest = "d" & CStr(iColIndex)
					doaDataType(iColIndex) = oTable.Columns.Item(iColIndex).DataType
					sTest = "e" & CStr(iColIndex)
				Next
				sTest = "x"
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DataSection - zzGetDataTypes")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("MainView  Is Nothing", "DataSection - zzGetDataTypes_1")
		End If


	End Sub
	Protected Class DataMerge
		Private miColumnNo As Integer
		Private msCurrentValue As String = String.Empty
		Private moCurrentBackColor As DMAcadExt.DMColor
		Private miFeature As CellFeatures
		Private miFirstRegionRow As Integer = -1
		Private miLastRegionRow As Integer = -1
		Private mbClosing As Boolean = False
		Private miaColumns(-1) As Integer
		Private miMergeBaseColumnNo As Integer
		Protected diGridLineType As Integer = -1
		Protected diGridLineWeight As Integer = -1
		Protected diGridLineStyle As Integer = -1
		Protected dbFirst As Boolean = False
		Public Event Merge(ByVal oRect As Rectangle, ByVal iSubColIndex As Integer, ByVal iGridLineWeight As Integer, ByVal iGridLineStyle As Integer, ByVal bWholeLine As Boolean)
		Public Event Cell(ByVal iRow As Integer, ByVal iCol As Integer)
		Public Sub New(ByVal sValue As String)
			miMergeBaseColumnNo = -1
			Dim saColumns() As String = Strings.Split(sValue, ":")
			ReDim miaColumns(saColumns.GetUpperBound(0))
			If saColumns.GetUpperBound(0) > 0 Then
				For iIndex As Integer = 1 To saColumns.GetUpperBound(0)
					Try
						miaColumns(iIndex) = Convert.ToInt32(saColumns(iIndex))
					Catch oEx As Exception
					End Try
				Next
			End If
			Dim saVar() As String = Strings.Split(saColumns(0), "|")
			Try
				miColumnNo = Convert.ToInt32(saVar(0))
				miaColumns(0) = miColumnNo
			Catch oEx As Exception
				Return
			End Try
			If saVar.GetUpperBound(0) >= 1 Then
				Try
					diGridLineWeight = Convert.ToInt32(saVar(1))
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "BaseDataSection - New_1")
					Return
				End Try
			End If
			If saVar.GetUpperBound(0) >= 2 Then
				Try
					diGridLineStyle = Convert.ToInt32(saVar(2))
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "BaseDataSection - New_2")
				End Try
			End If
		End Sub
		Public Function GetSubMergeColumns() As DataMerge()
			Dim iUB As Integer = miaColumns.GetUpperBound(0) - 1
			Dim oaDataMerge(iUB) As DataMerge
			If iUB >= 0 Then
				For iIndex As Integer = 0 To iUB
					oaDataMerge(iIndex) = New DataMerge(miColumnNo, miaColumns(iIndex + 1))
				Next
			End If
			Return oaDataMerge
		End Function

		Public Sub New(iMergeBaseColumnNo As Integer, iColumnNo As Integer)
			miMergeBaseColumnNo = iMergeBaseColumnNo
			miColumnNo = iColumnNo
		End Sub
		Public Sub Invert()
			miColumnNo = BaseReport.ColumnUB - miColumnNo
			For iIndex As Integer = 0 To miaColumns.GetUpperBound(0)
				miaColumns(iIndex) = BaseReport.ColumnUB - miaColumns(iIndex)
			Next
		End Sub
		Public ReadOnly Property IsMergeBase As Boolean
			Get
				Return (miMergeBaseColumnNo = -1)
			End Get
		End Property
		Public ReadOnly Property CurrentValue() As String
			Get

				Return msCurrentValue
			End Get
		End Property

		Public ReadOnly Property CurrentBackColor As DMAcadExt.DMColor
			Get
				Return moCurrentBackColor
			End Get
		End Property
		Public Property ColumnNo() As Integer
			Get
				Return miColumnNo
			End Get
			Set(ByVal iValue As Integer)
				miColumnNo = iValue
			End Set
		End Property
		Public Sub SetValue(ByVal iRow As Integer, ByVal sValue As String, ByVal oCurrentBackColor As DMAcadExt.DMColor)
			''''''''''''		System.Windows.Forms.MessageBox.Show(sValue & vbCrLf & CStr(miColumnNo), "DataMerge - SetValue 01_827")
			Try
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!MergeA", miMergeBaseColumnNo, miColumnNo, miFirstRegionRow, iRow, msCurrentValue, sValue)
				'DMCommon.Debug.MsgBox("13_341d", miFirstRegionRow, miMergeBaseColumnNo, msCurrentValue, sValue)
				If miFirstRegionRow = -1 Then
					miFirstRegionRow = iRow

				ElseIf msCurrentValue <> sValue AndAlso (miMergeBaseColumnNo = -1) Then
					zzMerge()
					miFirstRegionRow = iRow

				End If
				msCurrentValue = sValue
				moCurrentBackColor = oCurrentBackColor
				miLastRegionRow = iRow
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DataMerge - SetValue")
			End Try
		End Sub
		Public Sub Close()
			mbClosing = True


			If miFirstRegionRow >= 0 Then
				zzMerge()
			End If

		End Sub
		Public ReadOnly Property Closing() As Boolean
			Get
				Return mbClosing
			End Get
		End Property
		Public Function GetTestData() As String

			Dim sOut As String = "(" & CStr(miColumnNo) & ";" & CStr(miMergeBaseColumnNo) & ") - "
			For iIndex As Integer = 0 To miaColumns.GetUpperBound(0)
				sOut &= CStr(miaColumns(iIndex)) & ":"
			Next
			Return sOut
		End Function
		Private Sub zzMerge()
			'?????
			'  If miLastRegionRow > Report.AcadTable.NumRows - 1 Then
			'miLastRegionRow = Report.AcadTable.NumRows - 1
			'End If
			'DMCommon.Debug.ExcelLog.SetNextValue(4, "!MergeEx", Me.IsMergeBase, miFirstRegionRow, miLastRegionRow, miaColumns.GetUpperBound(0))

			If Me.IsMergeBase Then
				Dim iColRight As Integer = -1
				If miLastRegionRow >= miFirstRegionRow Then '????????????? 23/02/09
					Dim oRect As System.Drawing.Rectangle
					For iIndex As Integer = 0 To miaColumns.GetUpperBound(0)
						Try
							oRect = New System.Drawing.Rectangle(miaColumns(iIndex), miFirstRegionRow, 0, miLastRegionRow - miFirstRegionRow)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "BaseDataSection - zzMerge")
						End Try

						'''''''''''''''''''''25/02/07 CHECK
						''''	System.Windows.Forms.MessageBox.Show(CStr(diGridLineWeight) & ":" & CStr(diGridLineStyle), "02_377")
						'DMCommon.Debug.MsgBox("13_341f", oRect.X, oRect.Y, oRect.Width, oRect.Height, miaColumns(iIndex))
						If oRect.Width >= 0 OrElse oRect.Height >= 0 Then ''''280819
							RaiseEvent Merge(oRect, miaColumns(iIndex), diGridLineWeight, diGridLineStyle, dbFirst)
						End If

					Next

				ElseIf miLastRegionRow = miFirstRegionRow Then
					RaiseEvent Cell(miFirstRegionRow, miColumnNo)

				End If
				If miLastRegionRow = miFirstRegionRow Then
					RaiseEvent Cell(miFirstRegionRow, miColumnNo)
					miFirstRegionRow = -1
				End If
			End If

		End Sub
		Public Sub Reset()
			miFirstRegionRow = -1
		End Sub
		Public Property First() As Boolean
			Get
				Return dbFirst
			End Get
			Set(ByVal bValue As Boolean)
				dbFirst = bValue
			End Set
		End Property


	End Class
End Class

