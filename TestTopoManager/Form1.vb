Imports TopoManager
Imports TopoManager.TPlanGraph
Public Class Form1
   Private Shared moBlockTable As System.Data.DataTable = Nothing
   Private Const msBlockFieldName As String = "Block"
   Private Const msLegalAreaFieldName As String = "LegalArea"
   Private Const msAreaFieldName As String = "Area"
   Private Const msInPlanCalcAreaFieldName As String = "CalcArea"

   Private Const msPlanStateFieldName As String = "LocID"
   Private Const msPlanStateTextFieldName As String = "Location"


   Private Const msParcelEntireFieldName As String = "ParcelEntire"
   Private Const msParcelPartialFieldName As String = "ParcelPartial"
   Private moMainView As System.Data.DataView
   Private zzz() As String

   Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
      Dim oTopoDef As TopoDef = New TopoDef("lots")
      Dim sLayersDel As String
      Dim bResp As Boolean
      sLayersDel = oTopoDef.LinkLayer
      sLayersDel = TopoDef.AddDelim(sLayersDel)
      bResp = sLayersDel.Contains(sLayersDel)
   End Sub

   Private Sub ButtonA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonA.Click
      Dim iUB As Integer
      Stop
      Try
         iUB = zzz.GetUpperBound(0)

      Catch ex As Exception
         Stop
      End Try
      Try
         zzz = Nothing
      Catch ex As Exception
         Stop
      End Try
      Stop
   End Sub

   Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
      Dim oNumPair As NumerationPair = New NumerationPair()
      Dim sOutE As String, sOutP As String
      oNumPair.AddComplexNum("5", NumerationPair.enComplexType.Partial)
      oNumPair.AddComplexNum("6", NumerationPair.enComplexType.Partial)
      oNumPair.AddComplexNum("7", NumerationPair.enComplexType.Partial)
      sOutP = oNumPair.GetPresentation(NumerationPair.enComplexType.Partial)
      sOutE = oNumPair.GetPresentation(NumerationPair.enComplexType.Entire)
      MessageBox.Show(sOutE & ":" & sOutP)

   End Sub

   Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
      Stop
 
      Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
      TPlServerDB.ServerDB.Initialize(sDBResourceFile)
      Dim oAcadReport As AcadReport.TestRes = New AcadReport.TestRes(AcadReport.TplnReportID.LotsM)
      oAcadReport.LoadInfo()
      '  TPlanGraph.TplnProject.Terminate()
      TPlServerDB.ServerDB.Close()


   End Sub

   Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
      zzOpenBlockTable()
      FillData()
      disp()

   End Sub
   Private Shared Sub zzOpenBlockTable()
      If moBlockTable Is Nothing Then
         moBlockTable = New DataTable("Blocks")
         moBlockTable.Columns.Add(msBlockFieldName, GetAppType(Odbc.OdbcType.Int))
         moBlockTable.Columns.Add(msParcelEntireFieldName, GetAppType(Odbc.OdbcType.Text))
         moBlockTable.Columns.Add(msParcelPartialFieldName, GetAppType(Odbc.OdbcType.Text))
      Else
         moBlockTable.Clear()
      End If
   End Sub
   Friend Shared Function GetAppType(ByVal iOdbcType As Odbc.OdbcType) As System.Type
      Select Case iOdbcType
         Case Odbc.OdbcType.Int
            Return System.Type.GetType("System.Int32")
         Case Odbc.OdbcType.Double
            Return System.Type.GetType("System.Double")
         Case Odbc.OdbcType.Text
            Return System.Type.GetType("System.String")
         Case Odbc.OdbcType.Bit
            Return System.Type.GetType("System.Boolean")

         Case Else
            Return Nothing
      End Select
   End Function
   Private Sub FillData()
      Dim oNewRow As System.Data.DataRow
      For iRow As Integer = 0 To 1
         oNewRow = moBlockTable.NewRow()
         oNewRow.Item(msBlockFieldName) = 10 * iRow

         oNewRow.Item(msParcelEntireFieldName) = "Ent" & CStr(iRow)


         oNewRow.Item(msParcelPartialFieldName) = "Part" & CStr(iRow)
         moBlockTable.Rows.Add(oNewRow)
      Next





   End Sub
   Sub disp()
      moMainView = BlockView
      Dim oDataRowView As DataRowView
      Dim oVal As Object
      For iRowIndex As Integer = 0 To moMainView.Count - 1
         oDataRowView = moMainView.Item(iRowIndex)

         For iColIndex As Integer = 0 To 2

            oVal = oDataRowView.Item(iColIndex)

         Next
      Next
   End Sub
   Public Shared ReadOnly Property BlockView() As System.Data.DataView
      Get
         Dim sSort As String = msBlockFieldName
         Dim oDataView As System.Data.DataView = New System.Data.DataView(moBlockTable, String.Empty, sSort, DataViewRowState.CurrentRows)
         oDataView.AllowEdit = False
         oDataView.AllowDelete = False
         oDataView.AllowNew = False

         Return oDataView
      End Get
   End Property

   Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
      Dim oRect As Rectangle = New Rectangle(2, 3, 4, 5)
      Stop
   End Sub

   Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged

   End Sub

   Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
      Stop
      Dim iaInput() As Double = {505.993967476832, 0.00603252316838608}
      Dim iaOutput() As Integer = zzBalance(iaInput)
      Stop
   End Sub

   Private Function zzBalance(ByVal daInput() As Double) As Integer()
      Dim iDataUB As Integer
      Dim iIndex As Integer
      Dim dSum As Double
      Dim iSum As Integer

      Dim dLimit As Double
      Dim dStep As Double
      Dim iCurrentSum As Integer

      Dim iDirection As Integer = 0

      dLimit = 0.5
      dStep = dLimit * 0.5
      iDataUB = UBound(daInput)

      Dim iaOutput(iDataUB) As Integer
      Dim iaOutputPrev(iDataUB) As Integer
      For iIndex = 0 To iDataUB
         dSum = dSum + daInput(iIndex)
      Next
      iSum = zzIntRound(dSum, 0.499999999999999)

      Do While dStep > 0.000000000000001
         iCurrentSum = 0
         For iIndex = 0 To iDataUB
            iaOutput(iIndex) = zzIntRound(daInput(iIndex), dLimit)
            iCurrentSum = iCurrentSum + iaOutput(iIndex)
         Next
         Select Case iCurrentSum - iSum
            Case Is = 0
               Exit Do
            Case Is > 0
               dLimit = dLimit + dStep
               If iDirection = 1 Then
                  iaOutputPrev = iaOutput
               Else
                  iDirection = 1
               End If
            Case Is < 0
               dLimit = dLimit - dStep
               If iDirection = -1 Then
                  iaOutputPrev = iaOutput
               Else
                  iDirection = -1
               End If
         End Select
         If dLimit < 0.01 Then
            '  AcadReport.AcadUtil.GetEditor().WriteMessage("Balance: " & "Data out of range")

            Return Nothing
         End If
         dStep = dStep / 2.0#
      Loop
      iIndex = 0

      Do While (iCurrentSum <> iSum) And (iIndex <= iDataUB)
         If iaOutput(iIndex) <> iaOutputPrev(iIndex) Then
            iCurrentSum = iCurrentSum - iaOutput(iIndex) + iaOutputPrev(iIndex)
            iaOutput(iIndex) = iaOutputPrev(iIndex)
         End If
         iIndex += 1
      Loop

      Return iaOutput
   End Function
   Public Function CDblN(ByVal oValue As System.Object, Optional ByVal dValue As Double = 0.0) As Double
      If IsDBNull(oValue) Then
         Return dValue
      Else
         Return DirectCast(oValue, Double)
      End If
   End Function


   Private Function zzIntRound(ByVal dValue As Double, ByVal dLimit As Double) As Integer
      Dim dIntVal As Double


      dIntVal = Math.Floor(dValue)
      If (dValue - dIntVal >= dLimit) Or dIntVal = 0.0 Then
         Return CInt(dIntVal) + 1
      Else
         Return CInt(dIntVal)
      End If
   End Function

   Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
      Dim oExcelApp As Excel.Application
      Dim oWorkbook As Excel.Workbook
      Dim oWorkSheet As Excel.Worksheet
      Dim oRange As Excel.Range
      Dim oRangeA As Excel.Range
      Dim oRangeB As Excel.Range


      Dim o As Object

      '    oExcelApp = New Excel.Application()

      Try
         o = GetObject(, "Excel.Application")
      Catch ex As Exception
         o = GetObject("", "Excel.Application")
      End Try

      oExcelApp = CType(o, Excel.Application)

      oExcelApp.Visible = True
      oWorkbook = oExcelApp.ActiveWorkbook
      If oWorkbook Is Nothing Then
         oWorkbook = oExcelApp.Workbooks.Add()
      End If


      ' 
      o = oWorkbook.Worksheets.Add()
      Stop
      oWorkSheet = CType(o, Excel.Worksheet)
      oWorkSheet.Name = "Report1"
      oWorkSheet.Cells.Item(1, 1) = "abcdx"

      o = oWorkSheet.Range("A2")
      oRangeA = CType(o, Excel.Range)


      Stop
      oRangeA.Font.Background = 999
      Stop

      o = oWorkSheet.Range("B2", "D4")
      oRange = CType(o, Excel.Range)

      oRange.Merge()
      o = oWorkSheet.Range("B2")
      oRangeA = CType(o, Excel.Range)
      oRangeB = oRangeA.MergeArea
      Stop
      o = oWorkSheet.Range("B3")
      oRangeA = CType(o, Excel.Range)
      oRangeB = oRangeA.MergeArea
      Stop

      o = oWorkSheet.Range("B4")
      oRangeA = CType(o, Excel.Range)
      oRangeB = oRangeA.MergeArea
      Stop

      o = oWorkSheet.Range("B5")
      oRangeA = CType(o, Excel.Range)
      oRangeB = oRangeA.MergeArea
      Stop


      oRange.Value = "3. рсту"
      oRange.HorizontalAlignment = -4108
      oRange.VerticalAlignment = -4108
      Stop
      oRange.Font.Size = 2 * DirectCast(oRange.Font.Size, Double) + 1
      oRange.RowHeight = 2 * DirectCast(oRange.RowHeight, Double) + 1


      oRange.ReadingOrder = -5002
      oRange.ReadingOrder = -5003
      oRange.ReadingOrder = -5004

      For i As Integer = -5010 To -4000
         Try
            ' oRange.ReadingOrder = i
            ' Stop
         Catch ex As Exception

         End Try
      Next

   End Sub

   Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
      Dim oApp As TopoManager.NetApplication = New TopoManager.NetApplication()
      Dim mfTplnView As TPlanGraph.frmTplnView = Nothing
      If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
         mfTplnView = New TPlanGraph.frmTplnView
      End If
      mfTplnView.ShowDialog()

   End Sub

   Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
      Dim sText As String
      '  sText = TopoManager.Common.GetTest()
      sText = Class1.GetActiveWinText()
   End Sub

   Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
      If Me.CheckBox1.Checked Then
         Me.CheckBox1.Image = Global.TestTopoManager.My.Resources.Resources.NOTE
      Else
         Me.CheckBox1.Image = Global.TestTopoManager.My.Resources.PointName
      End If


   End Sub
End Class
