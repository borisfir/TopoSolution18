Option Explicit On
Option Strict On
Public Class LaunchReport
	Private miResourceTheme As TPlServerDB.enResourceTheme
	Private Shared moReportApp As AcadReport.Report
	Private mbAutocad As Boolean
	Private mdPaintFactor As Double
	Public Sub New(bAutocad As Boolean, iResourceTheme As TPlServerDB.enResourceTheme)
		mbAutocad = bAutocad
      miResourceTheme = iResourceTheme
      '  System.Windows.Forms.MessageBox.Show(CStr(mbAutocad) & ":" & CStr(miResourceTheme), "21_457b")
	End Sub
	Public Property PaintFactor As Double
		Get
			Return mdPaintFactor
		End Get
		Set(dValue As Double)
			mdPaintFactor = dValue
		End Set
	End Property
	Public Sub InsertReport(ByVal oDataView As System.Data.DataView, ByVal iaDataColumns() As Integer, ByVal oaTotals() As System.Object, ByVal oaOptionValues() As System.Object, Optional ByVal iaEmptyColumns() As Integer = Nothing)
		If oDataView IsNot Nothing Then
			Dim oRepApp As AcadReport.BaseReport
			Dim bCurrentLayerOK As Boolean = True
			If mbAutocad Then
				oRepApp = New AcadReport.Report
			Else
				oRepApp = New ExcelReport.Report(False)
         End If
         ' System.Windows.Forms.MessageBox.Show("!!" & CStr(miResourceTheme) & "!!", "21_457a")
			If oRepApp.Open(miResourceTheme) Then
				oRepApp.MainView = oDataView
				If iaDataColumns IsNot Nothing Then
					oRepApp.DataColumns = iaDataColumns
				End If
				If oaTotals IsNot Nothing Then
					oRepApp.Totals = oaTotals
				End If
				If oaOptionValues(0) IsNot Nothing Then
					oRepApp.OptionValues = oaOptionValues
				End If
				If iaEmptyColumns IsNot Nothing Then
					oRepApp.EmptyColumns = iaEmptyColumns
            End If
            '   System.Windows.Forms.MessageBox.Show(CStr(oRepApp.AcadModel), "21_462")
            If oRepApp.AcadModel Then
               Dim sRepLayer As String = String.Empty
               DMAcadExt.AcadDocument.SaveVarCmdDia(0)
               DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
               DMAcadExt.AcadTransaction.Start()
               DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
               AcadReport.RepApp.InitDWGScaleFactor()
               AcadReport.RepApp.Init(False)
               bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sRepLayer, DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Report, True, True, False)

               Dim colaPoints() As Autodesk.AutoCAD.Geometry.Point3dCollection
               Dim taColorScheme() As DMAcadExt.ColorScheme
               Dim oReportApp As AcadReport.Report
               If bCurrentLayerOK Then
                  ' System.Windows.Forms.MessageBox.Show(CStr(bCurrentLayerOK) & vbCrLf & sRepLayer, "21_466")
                  TopoManager.Common.SetAcadFocus()
                  AcadReport.RepApp.GetStartPoint()
                  oRepApp.Insert()
                  '   System.Windows.Forms.MessageBox.Show(CStr(bCurrentLayerOK) & vbCrLf & sRepLayer, "21_467")
                  oReportApp = DirectCast(oRepApp, AcadReport.Report)
                  '  System.Windows.Forms.MessageBox.Show(CStr(AcadReport.RepApp.AcadTable IsNot Nothing), "21_468")
                  If AcadReport.RepApp.AcadTable IsNot Nothing Then ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                     ''''''''''''''''''''''''''	AcadReport.Report.AcadTable.RecomputeTableBlock(True)
                     '''''''''''''''''''''	DMAcadExt.AcadDocument.Regen()
                     '''''''''''''''''	AcadReport.Report.ReDrawTable()
                     colaPoints = oReportApp.GetColorCells()
                     moReportApp = oReportApp
                     If colaPoints IsNot Nothing Then

                        taColorScheme = oReportApp.GetColorScheme()

                        Dim oPgon As TopoManager.SimplePgon
                        '	Dim tColorZebra As DMAcadExt.ColorZebra
                        Dim dRepBamashSharedScale As Double
                        Try
                           '	dRepBamashSharedScale = Convert.ToDouble(Me.txtRepBamashSharedScale.Text)
                           If dRepBamashSharedScale < 0.00001 Then
                              dRepBamashSharedScale = 1.0
                           End If
                        Catch oEx As Exception
                           '	dRepBamashSharedScale = Parameters.LegendPaintFactor
                        End Try

                        '		MessageBox.Show(CStr(colaPoints.GetUpperBound(0)), "01_403")
                        MessageBox.Show(CStr(dRepBamashSharedScale) & ":" & CStr(AcadReport.RepApp.DrawingScaleFactor), "01_404")
                        For iIndex As Integer = 0 To colaPoints.GetUpperBound(0)
                           oPgon = New TopoManager.SimplePgon(colaPoints(iIndex))
                           oPgon.SetTagNum("Table", iIndex)
                           If iIndex < 0 Then
                              Dim s As String = ""
                              For Each tPoint As Autodesk.AutoCAD.Geometry.Point3d In colaPoints(iIndex)
                                 s &= DMAcadExt.TPlnPoint.DispPoint(tPoint) & vbCrLf
                              Next

                              DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "; " & s)
                              MessageBox.Show(CStr(iIndex) & "; " & s, "01_405")
                           End If
                           DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
                           DMAcadExt.AcadDocument.WriteMessage("^709 " & CStr(taColorScheme(iIndex).Scale) & ":" & CStr(dRepBamashSharedScale) & ":" & CStr(AcadReport.RepApp.DrawingScaleFactor))
                           taColorScheme(iIndex).Scale = mdPaintFactor * AcadReport.RepApp.DrawingScaleFactor
                           oPgon.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, taColorScheme(iIndex), False)

                           DMAcadExt.AcadTransaction.InsertNewBlock(False)
                        Next
                        moReportApp.ClearZebraCells()
                     End If
                  Else
                     System.Windows.Forms.MessageBox.Show("Table was not found", "21_488")
                  End If

                  DMAcadExt.AcadTransaction.CloseModelSpace()
                  DMAcadExt.AcadTransaction.Terminate()
                  DMAcadExt.AcadDocument.Unlock()
                  DMAcadExt.AcadDocument.RestoreVarCmdDia()
                  '''''''''''''''	zzPaintTable()

               End If
            Else

               oRepApp.Insert()
            End If
			End If
		End If

	End Sub
End Class
