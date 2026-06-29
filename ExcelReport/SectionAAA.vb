Option Explicit On
Option Strict On

Friend Enum enTransformType
   [String] = 0
   Invert = 1
   FormatNum = 2
End Enum
Friend MustInherit Class SectionAAA

   Protected ddaRowHeight() As Double

   Protected doTableCell() As Object 'TableCell
   Protected diCellTypeUB As Integer
   Private mdaColWidth() As Double
   Private mdTextHeightAAA As Double

   Protected ddaTextHeight() As Double

   Private moaMergeAAA() As Merge

   Private moaInputValue() As String
   ' Private miRowType As RowType
   Private miFirstRow As Integer = 0
   Private miRowCount As Integer = 0

   Protected diLastRow As Integer = -1


   '   Protected dbIsDataSection As Boolean
   '  Protected diResourceTheme As Integer = TPlServerDB.enResourceTheme.AcRepContent
	Protected diSectionID As Integer = -1
   '' Private mdSectionHeight As Double
   ' Protected doMerges() As Rectangle
   Private moCellResource As TPlServerDB.TPlResource

   Protected Sub New()

   End Sub


   Protected Sub New(ByVal oResource As TPlServerDB.TPlResource)


		Dim iaRes() As Integer = oResource.GetIntItem(0, , True)
      Try
         diSectionID = iaRes(0)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-New-" & CStr(Me.diSectionID))
      End Try

      ddaRowHeight = oResource.GetDblItem(1)
      '  doMerges = oResource.GetRectangleArray(2)
		System.Windows.Forms.MessageBox.Show(CStr(ddaRowHeight(0)), "01_999")

      moCellResource = oResource.GetChild(0)


   End Sub
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
         Me.AfterFirstRowChange()
      End Set
   End Property

   Public ReadOnly Property LastRow() As Integer
      Get
         Return diLastRow
      End Get
   End Property
   Protected Sub InsertRows()
      Dim iRowLB As Integer
      Dim dRowHeight As Double


      If miFirstRow = 0 Then

         '    Report.AcadTable.SetRowHeight(0, ddaRowHeight(0) * Report.DrawingScaleFactor)
         iRowLB = 1

      Else
         iRowLB = miFirstRow

      End If
      Dim iRowIndex As Integer
      Try
         For iRowIndex = iRowLB To diLastRow  'As Integer
            dRowHeight = ddaRowHeight(iRowIndex - miFirstRow)
            '' Report.AcadTable.InsertRows(iRowIndex, dRowHeight * Report.DrawingScaleFactor, 1)
         Next
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-InsertRows-" & CStr(diSectionID))


      End Try


   End Sub

   Public MustOverride Sub Format()
   Public MustOverride Sub Print()
   Protected Sub AfterFirstRowChange()
      diLastRow = miFirstRow + miRowCount - 1
   End Sub
	Protected Sub OnFormattingAAA()
		zzLoadCellProp()
	End Sub

   Private Sub zzLoadCellProp()
      Dim sCellRes As String = String.Empty
      ReDim doTableCell(diCellTypeUB)
      Dim sTest As String = ""
      For i As Integer = 0 To moCellResource.ResItems.GetUpperBound(0)
         sTest &= (moCellResource.ResItems(i) & " : ")
      Next

      For iIndex As Integer = 0 To diCellTypeUB
         Try
            sCellRes = moCellResource.ResItems(iIndex)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(CStr(doTableCell.GetUpperBound(0)) & ":" & CStr(iIndex), "TableCell.GetUpperBound  :  Index-" & CStr(Me.diSectionID))
            System.Windows.Forms.MessageBox.Show(oEx.Message, "Section-zzLoadCellProp" & CStr(Me.diSectionID))
         End Try
         '   doTableCell(iIndex).Open(sCellRes)
      Next
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
