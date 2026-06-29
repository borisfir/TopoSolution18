Option Explicit On
Option Strict On
Public MustInherit Class BaseReport
	Protected doMainView As System.Data.DataView
	Protected doaCaptions() As System.Object
	Protected doaTotals() As System.Object
	Protected doaOptionValues() As System.Object
	Protected ddicOptionValues As Dictionary(Of Integer, System.Object)
	Protected diaDataColumns() As Integer = Nothing
	Protected diaEmptyColumns() As Integer = Nothing
	Protected dbAreaMeter As Boolean
	Protected dbMergeRows As Boolean = True


	Protected dbAcadModel As Boolean
	Protected dtDataTableCell As AcadReport.TableCell
	Protected dbDataTableCellExists As Boolean = False

	Protected miaColumnWidthAAA() As Integer
	Public Shared ColumnCaptions() As String
	Public Shared Caption As String
	Public Shared ColWidths() As Double
	Public Shared ColumnResUB As Integer
	Public Shared NumColumnsAAA As Integer
	Public Shared GroupColumnStart As Integer = -1
	Public Shared GroupColumnUB As Integer
	Private Shared moReportApp As AcadReport.Report
	Protected Shared diResourceTheme As TPlServerDB.enResourceTheme
	Public Shared BaseResource As TPlServerDB.TPlResource
	Public Shared ReadOnly Property ColumnUB As Integer
		Get
         Return ColumnResUB + GroupColumnUB
		End Get
	End Property
	Public Property MainView() As System.Data.DataView
		Get
			Return doMainView
		End Get
		Set(ByVal oValue As System.Data.DataView)
			doMainView = oValue
		End Set
	End Property

	Public Property DataTableCell() As AcadReport.TableCell
		Get
			Return dtDataTableCell
		End Get
		Set(ByVal tValue As AcadReport.TableCell)
			dtDataTableCell = tValue
			dbDataTableCellExists = True
		End Set
	End Property
	Public ReadOnly Property AcadModel() As Boolean
		Get
			Return dbAcadModel
		End Get
	End Property
	Public Property AreaMeter() As Boolean
		Get
			Return dbAreaMeter
		End Get
		Set(bValue As Boolean)
			dbAreaMeter = bValue
		End Set
	End Property
	Public Property MergeRows() As Boolean
		Get
			Return dbMergeRows
		End Get
		Set(bValue As Boolean)
			dbMergeRows = bValue
		End Set
	End Property

	Public Property Captions() As System.Object()
		Get
			Return doaCaptions
		End Get
		Set(ByVal oaValue As System.Object())
			doaCaptions = oaValue
		End Set
	End Property

	Public Property Totals() As System.Object()
		Get
			Return doaTotals
		End Get
		Set(ByVal oaValue As System.Object())
			doaTotals = oaValue
		End Set
	End Property

	Public Property OptionValues() As System.Object()
		Get
			Return doaOptionValues
		End Get
		Set(ByVal oaValue As System.Object())
			doaOptionValues = oaValue
		End Set
	End Property
	Public Property OptionValuesDic() As Dictionary(Of Integer, System.Object)
		Get
			Return ddicOptionValues
		End Get
		Set(ByVal dicValue As Dictionary(Of Integer, System.Object))
			ddicOptionValues = dicValue
		End Set
	End Property
	Public Property DataColumns() As Integer()
		Get
			Return diaDataColumns
		End Get
		Set(ByVal iaValue As Integer())
			diaDataColumns = iaValue
		End Set
	End Property
	Public Property EmptyColumns() As Integer()
		Get
			Return diaEmptyColumns
		End Get
		Set(ByVal iaValue As Integer())
			diaEmptyColumns = iaValue
		End Set
	End Property



	Public Property ResourceTheme() As TPlServerDB.enResourceTheme
		Get
			Return diResourceTheme
		End Get
		Set(ByVal iValue As TPlServerDB.enResourceTheme)
			diResourceTheme = iValue
		End Set
	End Property


	Protected Function OnOpen(ByVal iResourceTheme As TPlServerDB.enResourceTheme) As Boolean
		diResourceTheme = iResourceTheme
		If iResourceTheme <> TPlServerDB.enResourceTheme.Undefined Then
			BaseResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(iResourceTheme)
			If BaseResource IsNot Nothing Then
				Dim iaResColWidths() As Double
				Try
					iaResColWidths = BaseReport.BaseResource.GetDblItem(1)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "BaseReport - OnOpen")
					Return False
				End Try

				If iaResColWidths IsNot Nothing Then

					ColumnResUB = iaResColWidths.GetUpperBound(0)
					If GroupColumnStart >= 0 Then
						ReDim ColWidths(ColumnUB)
						Dim iValIndex As Integer = 0
						For iResIndex As Integer = 0 To ColumnResUB
							If iResIndex = GroupColumnStart Then
								For iIndex As Integer = 0 To GroupColumnUB
									ColWidths(iValIndex) = iaResColWidths(iResIndex)
									iValIndex += 1
								Next
							Else
								ColWidths(iValIndex) = iaResColWidths(iResIndex)
								iValIndex += 1
							End If

						Next

					Else
						ColWidths = iaResColWidths
					End If
               '     System.Windows.Forms.MessageBox.Show(CStr(ColWidths.GetUpperBound(0)) & vbCrLf & iaResColWidths.GetUpperBound(0).ToString() & vbCrLf & CStr(GroupColumnStart) & ":" & CStr(GroupColumnUB), "BaseReport - OnOpen")


					'	NumColumns = ColumnUB + 1
				End If
				Return True
			Else
				Return False
			End If
		Else
			BaseResource = Nothing
			Return True
		End If


	End Function
   Public Shared Function GetText(ByVal iItemID As Integer, ByVal iSectionID As Integer, Optional ByVal iMsgDbg As Integer = 0) As String
      'System.Windows.Forms.MessageBox.Show(CStr(iItemID) & vbCrLf & iSectionID.ToString, "07_560")
      Return TPlServerDB.TextResource.GetText(iItemID, diResourceTheme, iSectionID, , iMsgDbg)

   End Function

    Public MustOverride Function Insert() As Boolean
	Public MustOverride Function InsertMulti() As Boolean
	Public MustOverride Function InsertSpec() As Boolean

	Public MustOverride Function Open(Optional ByVal iResourceTheme As TPlServerDB.enResourceTheme = 0) As Boolean
    Public MustOverride Sub NextTable()
End Class
