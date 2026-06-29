Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Drawing
Public Enum enSectionType
	Autocad = 1
	Excel = 2
	Title = 4
	HeaderBase = 8
	Header = 16
	Data = 128
	Footer = 32
End Enum
Friend Enum enTransformType
   [String] = 0
   Invert = 1
   FormatNum = 2
End Enum
Public Enum enExcelBorderType
	Undefined
	AllVertical = 1

End Enum
Public MustInherit Class Section
	Protected diSectionType As enSectionType
	Protected ddaRowHeight() As Double
	Protected dRowHeightExcelScale As Double = -1.0
	Protected doaTableCell() As TableCell
	Protected doTableCell As TableCell
	Protected diCellTypeResUB As Integer = -1
	Protected diCellTypeUB As Integer = -1

	Protected diGroupColumnStartAAA As Integer
	Protected diGroupColumnUBAAA As Integer
   Protected dbAreaMeter As Boolean

   ' Private mdaColWidth() As Double
   Private mdTextHeightAAA As Double

   Protected ddaTextHeight() As Double

   Private moaMergeAAA() As Merge
   ''  Private moaInputCell() As System.Drawing.Point
   Private moaInputValue() As String
   ' Private miRowType As RowType
   Private miFirstRow As Integer = 0
   Private miRowCount As Integer = 0

   Protected diLastRow As Integer = -1


   '   Protected dbIsDataSection As Boolean
   '  Protected diResourceTheme As Integer = TPlServerDB.enResourceTheme.AcRepContent
	Protected diSectionID As Integer = 0
   Protected dbAutoFit As Boolean = False
   Protected dsAcadFontName As String = String.Empty
	Protected dtExcelFont As ExcelFont
	Protected dsExcelBorderDesc As String
	Protected diExcelBorderType As enExcelBorderType = enExcelBorderType.Undefined

	'' Private mdSectionHeight As Double
	' Protected doMerges() As Rectangle
	Private moCellResource As TPlServerDB.TPlResource

	Protected Sub New()
		diSectionID = -1
	End Sub

	Protected Sub New(ByVal oResource As TPlServerDB.TPlResource)
		Dim iaRes() As Integer = oResource.GetIntItem(0, , False)
		Try
			diSectionID = iaRes(0)


		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-New_1 - " & CStr(Me.diSectionID))
		End Try
		If diSectionID < 0 Then
			RowCount = 0
		Else
			ddaRowHeight = oResource.GetDblItem(1)
			'  doMerges = oResource.GetRectangleArray(2)
			iaRes = oResource.GetIntItem(3)	'AutoFit
			Try
				If iaRes(0) <> 0 Then
					dbAutoFit = True
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-New_2 - " & CStr(Me.diSectionID))
			End Try
			Try
				dsAcadFontName = DMCommon.Functions.CStrN(oResource.ResItems(4))
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-New_3 - " & CStr(Me.diSectionID))
			End Try
			Try
				dtExcelFont = New ExcelFont(DMCommon.Functions.CStrN(oResource.ResItems(5)))
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-New_4a - " & CStr(Me.diSectionID))
			End Try
			If Not dtExcelFont.Exists Then
				dtExcelFont = RepApp.ExcelFontName
			End If
			Try
				If oResource.ResItems.GetUpperBound(0) >= 6 AndAlso oResource.ResItems(6).Length <> 0 Then
					dRowHeightExcelScale = Convert.ToDouble(oResource.ResItems(6))
				End If

			Catch oEx As Exception
				dRowHeightExcelScale = -1.0
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & DMCommon.Functions.CStrN(oResource.ResItems(6)) & vbCrLf & CStr(oResource.ResItems.GetUpperBound(0)), "Section-New_4b - " & CStr(Me.diSectionID))
				DMCommon.Debug.MsgBox("", oResource.ResItems)
			End Try
			Try
				If oResource.ResItems.GetUpperBound(0) >= 7 Then
					Dim sValue As String = DMCommon.Functions.CStrN(oResource.ResItems(7))
					Dim iValue As Integer
					Try
						iValue = Convert.ToInt32(sValue)
					Catch oEx As Exception
						iValue = 0
					End Try
					If [Enum].IsDefined(GetType(enExcelBorderType), iValue) Then
						diExcelBorderType = CType(iValue, enExcelBorderType)
					End If
				End If
			Catch oEx As Exception

				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(oResource.ResItems.GetUpperBound(0)), "Section-New_4c - " & CStr(Me.diSectionID))
			End Try



			moCellResource = oResource.GetChild(0)
		End If

	End Sub
	Public Property TableCell() As TableCell
		Get
			Return doTableCell
		End Get
		Set(ByVal oValue As TableCell)
			doTableCell = oValue
			diCellTypeUB = -1
		End Set
	End Property
	Public ReadOnly Property RowHeight() As Double()
		Get
			Return ddaRowHeight
		End Get

	End Property
	Public Property FirstRow() As Integer
		Get
			Return miFirstRow
		End Get
		Set(ByVal iValue As Integer)
			miFirstRow = iValue
			Me.AfterFirstRowChange()

		End Set
	End Property
	Public Property RowCount() As Integer
		Get
			Return miRowCount
		End Get
		Set(ByVal iValue As Integer)
			miRowCount = iValue
			If miRowCount > 0 Then
				Me.AfterFirstRowChange()
			End If
		End Set
	End Property

	Public ReadOnly Property LastRow() As Integer
		Get
			Return diLastRow
		End Get
	End Property

	Public Function GetSectionHeight() As Double
		Dim dResult As Double = 0.0
		Try
			For iRowIndex As Integer = miFirstRow To diLastRow
				dResult += RepApp.AcadTable.Rows(iRowIndex).Height
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Section - GetSectionHeight")
		End Try
		Return dResult
	End Function
   Public MustOverride Sub Format()
   Public MustOverride Sub Print()
	Protected Sub AfterFirstRowChange()

		diLastRow = miFirstRow + miRowCount - 1
		'	System.Windows.Forms.MessageBox.Show(CStr(miFirstRow) & ":" & CStr(diLastRow) & "=" & CStr(miRowCount), "01_144," & Me.diSectionType.ToString() & "," & Me.diSectionID.ToString())
	End Sub
   Protected Sub OnFormatting()
      zzLoadCellProp()
   End Sub
	Protected Sub Frmt(ByVal oRect As Rectangle)
		diCellTypeUB -= (oRect.Bottom - oRect.Top + 1) * (oRect.Right - oRect.Left + 1) - 1
		diCellTypeResUB = diCellTypeUB
		'	MessageBox.Show(CStr(diCellTypeUB) & ":" & oRect.Top & "," & oRect.Left & "," & oRect.Right, "19_650")
	End Sub
	Private Sub zzLoadCellProp()
		Dim sCellRes As String = String.Empty
      Dim iDataIndex As Integer = 0
      Dim iGroupShift As Integer = 0


		If diCellTypeUB >= 0 Then
			Try
            ReDim doaTableCell(diCellTypeUB)
            'System.Windows.Forms.MessageBox.Show(diSectionID.ToString() & vbCrLf & CStr(diCellTypeResUB) & ":" & CStr(diCellTypeUB) & vbCrLf & doaTableCell.GetUpperBound(0), "05_329c")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Section - zzLoadCellProp ")
			End Try

		End If

		If moCellResource IsNot Nothing Then



			'  System.Windows.Forms.MessageBox.Show(CStr(BaseReport.GroupColumnStart) & ":" & CStr(BaseReport.GroupColumnUB) & vbCrLf & CStr(doaTableCell.GetUpperBound(0)) & ":" & CStr(diCellTypeResUB) & vbCrLf & Me.diSectionType.ToString() & ":" & sTest, "19_266")
			For iResIndex As Integer = 0 To diCellTypeResUB
				Try
					sCellRes = moCellResource.ResItems(iResIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(CStr(doaTableCell.GetUpperBound(0)) & ":" & CStr(diCellTypeResUB) & ":" & CStr(iResIndex) & vbCrLf & CStr(moCellResource.ResItems.GetUpperBound(0)), "TableCell.GetUpperBound: Index-" & CStr(Me.diSectionID))
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Section - zzLoadCellProp" & CStr(Me.diSectionID))
				End Try
            ' System.Windows.Forms.MessageBox.Show(iResIndex.ToString() & ":" & BaseReport.GroupColumnUB.ToString, "20_003 " & CStr(Me.diSectionID))
            If (diSectionType And enSectionType.Title) = 0 And iResIndex = BaseReport.GroupColumnStart Then
               For iIndex As Integer = 0 To BaseReport.GroupColumnUB
                  doaTableCell(iDataIndex).Open(sCellRes, iIndex, iResIndex)
                  iDataIndex += 1
                  iGroupShift += 1
               Next

            ElseIf (diSectionType And enSectionType.Title) = 0 Then
               If iGroupShift = 0 Then
                  doaTableCell(iDataIndex).Open(sCellRes, 0, iResIndex)
               Else
                  doaTableCell(iDataIndex).Open(sCellRes, iGroupShift - 1, iResIndex)
               End If

               iDataIndex += 1
            Else
               doaTableCell(iDataIndex).Open(sCellRes, 0, iResIndex)
               iDataIndex += 1
            End If

			Next
			'	zzDispArray(doaTableCell, "doaTableCell-1532")
			'	System.Windows.Forms.MessageBox.Show(diSectionType.ToString() & vbCrLf & CStr(diCellTypeResUB) & ":" & CStr(diCellTypeUB) & vbCrLf & CStr(moCellResource.ResItems.GetUpperBound(0)), "05_311 Sect")
		End If
	End Sub
	Private Sub zzDispArray(ByVal daValue() As TableCell, ByVal sTitle As String)
		Dim sMsg As String = String.Empty
		Try
			For iIndex As Integer = 0 To daValue.GetUpperBound(0)
				If iIndex <> 0 Then
					sMsg &= vbCrLf
				End If
				sMsg &= daValue(iIndex).ToString
			Next
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispDblArray")
		End Try
		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)

	End Sub
   Private Sub zzGetMerges()

   End Sub




   Private Structure Merge
      Dim FirstRow As Integer
      Dim FirstCol As Integer
      Dim LastRow As Integer
      Dim LastCol As Integer
   End Structure

   


End Class
