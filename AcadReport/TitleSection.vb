Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.ApplicationServices.Application
Imports Autodesk.AutoCAD.Geometry

Friend Class TitleSection

   Inherits Section
   Private Shared mtTextStyleTableRecordID As ObjectId
	Private moaData() As System.Object '= Nothing
	Private mdicData As Dictionary(Of Integer, System.Object)

	Private moaDBText() As Autodesk.AutoCAD.DatabaseServices.DBText
   Private mtBasePoint As Autodesk.AutoCAD.Geometry.Point3d
   Private mdCurrentY As Double
   Private miaMultiReport() As RepApp.enReportType
	Public Sub New(ByVal oResource As TPlServerDB.TPlResource, Optional ByVal oaData() As System.Object = Nothing, Optional ByVal dicData As Dictionary(Of Integer, System.Object) = Nothing)
		MyBase.New(oResource)
		'DMCommon.Debug.MsgBox("Title1", oResource.ResItems)
		'DMCommon.Debug.MsgBox("!oaData", oaData)

		MyBase.diSectionType = enSectionType.Autocad Or enSectionType.Title
		Dim iaInput() As Integer = Nothing
		'    DMCommon.Functions.DispArray(ddaRowHeight, "ddaRowHeight")
		'  DMCommon.Functions.DispArray(oaData, "oaData", True)
		Dim sAcadFont As String
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!diSectionID", MyBase.diSectionID)
		'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!ddaRowHeight", ddaRowHeight)
		If MyBase.diSectionID >= 0 Then
			If ddaRowHeight IsNot Nothing Then
				MyBase.RowCount = ddaRowHeight.GetUpperBound(0) + 1
			End If
			MyBase.FirstRow = 0

			DMCommon.Debug.ExcelLog.SetNextValue(0, "!!TitleRowCount", MyBase.RowCount, MyBase.LastRow)
			If MyBase.RowCount > 0 Then
				ReDim moaDBText(MyBase.RowCount - 1)
			End If
			moaData = oaData
			mdicData = dicData
			mtBasePoint = RepApp.BasePoint
			mdCurrentY = mtBasePoint.Y
			sAcadFont = oResource.ResItems(5)
			Try
				iaInput = oResource.GetIntItem(6)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(CStr(oResource.ResItems(4)) & ":" & CStr(oResource.ResItems(5)), "05_313 Title")
			End Try
			If iaInput IsNot Nothing Then
				ReDim miaMultiReport(iaInput.GetUpperBound(0))
				For iIndex As Integer = 0 To iaInput.GetUpperBound(0)
					miaMultiReport(iIndex) = CType(iaInput(iIndex), RepApp.enReportType)
				Next
			End If


			zzInitTextStyle(sAcadFont)
		End If
	End Sub
	Public Overrides Sub Format()
      If MyBase.diSectionID >= 0 AndAlso MyBase.RowCount > 0 Then
         Try
            MyBase.diCellTypeResUB = MyBase.RowCount - 1
            MyBase.diCellTypeUB = MyBase.RowCount - 1

				MyBase.OnFormatting()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection - Format")
         End Try
      End If
   End Sub
   Public Overrides Sub Print()
      Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText
      Try

         If miaMultiReport IsNot Nothing Then
            ' System.Windows.Forms.MessageBox.Show(CStr(MyBase.FirstRow) & ":" & CStr(MyBase.diLastRow), "23_050 FirstRow To  LastRow") 'April
            ' DMCommon.Functions.DispArray(miaMultiReport, "miaMultiReport")
            For iRowIndex As Integer = MyBase.FirstRow To MyBase.diLastRow
               If miaMultiReport(iRowIndex).HasFlag(RepApp.ReportType) Then
                  zzDrawTitleText(iRowIndex)
                  oText = moaDBText(iRowIndex)
                  If oText IsNot Nothing Then
                     oText.HorizontalMode = TextHorizontalMode.TextMid
                     oText.VerticalMode = TextVerticalMode.TextTop
                     mdCurrentY -= MyBase.ddaRowHeight(iRowIndex) * RepApp.DrawingScaleFactor
                  End If
               End If
            Next
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TitleSection-Print")
      End Try
   End Sub
   Public Function GetTitleHeight() As Double
      Dim dResult As Double = 0.0

      If miaMultiReport IsNot Nothing Then
         Try
            For iRowIndex As Integer = MyBase.FirstRow To MyBase.diLastRow
               If miaMultiReport(iRowIndex).HasFlag(RepApp.ReportType) Then
                  dResult += MyBase.RowHeight(iRowIndex)
               End If


            Next
         Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & miaMultiReport.GetUpperBound(0) & vbCrLf & CStr(MyBase.FirstRow) & " ==> " & CStr(MyBase.diLastRow) & vbCrLf & "RowHeights: " & MyBase.RowHeight.GetUpperBound(0).ToString(), "!DrawTitle")
			End Try
      End If
     
      Return dResult * RepApp.DrawingScaleFactor
   End Function
   Private Sub zzDrawTitleText(ByVal iTextIndex As Integer)
      Dim sTitleText As String
      Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText
      Dim oTableCell As TableCell
      Try
         oTableCell = MyBase.doaTableCell(iTextIndex)
         oTableCell.DestType = enDestType.AcadText
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection -zzDrawTitleText_1")
         Return
      End Try
      Try
         oText = New DBText
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection -zzDrawTitleText_2")
         Return
      End Try
      Try
         moaDBText(iTextIndex) = oText
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection -zzDrawTitleText_3")
      End Try
      Try
         sTitleText = BaseReport.GetText(iTextIndex, MyBase.diSectionID)
         '  System.Windows.Forms.MessageBox.Show(sTitleText, "sTitleText")
         ' DMCommon.ExcelLogF.SetNextValue(i, 7, "sTitleText", sTitleText)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection - zzDrawTitleText_4")
         Exit Sub
      End Try
      Try
         If Not mtTextStyleTableRecordID.IsNull Then
            oText.TextStyleId = mtTextStyleTableRecordID
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "TitleSection - zzDrawTitleText_5")
      End Try
      Try
         oText.Position = zzGetLeftTop(0.5 * oTableCell.TextHeight)

      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawTitle TextPoint")
      End Try


		' DMCommon.Functions.DispArray(moaData, sTitleText, True)
		'  DMCommon.ExcelLogF.SetNextValue(i, 3, sTitleText)
		'  DMCommon.ExcelLogF.SetArray(moaData, 5)

		sTitleText = oTableCell.Format(sTitleText, False, dbAreaMeter, moaData, mdicData)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!sTitleText", iTextIndex, sTitleText)
		DMCommon.Debug.ExcelLog.SetEnumerable(0, "!moaData", moaData)
		If mdicData IsNot Nothing Then
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "!mdicData", mdicData.Values)
		End If



		oText.TextString = sTitleText
      'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
      oText.HorizontalMode = TextHorizontalMode.TextMid
      oText.VerticalMode = TextVerticalMode.TextVerticalMid
      '		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
      oText.Height = oTableCell.TextHeight * RepApp.DrawingScaleFactor
      Try
         oText.AlignmentPoint = zzGetMiddleTop(iTextIndex, 0.5 * oTableCell.TextHeight * RepApp.DrawingScaleFactor)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawTitle AlignmentPoint")
      End Try
      oText.Visible = True
      DMAcadExt.AcadTransaction.AppendEntity(oText)
   End Sub
   Private Function zzGetMiddleTop(ByVal iTextIndex As Integer, ByVal dShiftY As Double) As Point3d
      Dim dX As Double = 0.5 * (RepApp.TableBasePoint.X + RepApp.TopRightPoint.X)
      Dim dY As Double = mdCurrentY - dShiftY '0.5 * (moBasePoint.Y + Report.TopRightPoint.Y)
      Return New Point3d(dX, dY, 0.0)

   End Function
   Private Function zzGetLeftTop(ByVal dShiftY As Double) As Point3d
      Dim dX As Double = mtBasePoint.X
      Dim dY As Double = mdCurrentY - dShiftY
      Return New Point3d(dX, dY, 0.0)
   End Function
   Private Shared Sub zzInitTextStyle(ByVal sAcadFont As String)
      mtTextStyleTableRecordID = DMAcadExt.AcadTransaction.GetTextStyle(sAcadFont)     '
   End Sub

End Class
