Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Enum TplnReportID
   LotsM = 1
   Parcels = 11
End Enum
Public Class Report
	Inherits BaseReport

	'Private miResourceTheme As TPlServerDB.enResourceTheme
	Private miReportID As TplnReportID

	Private msColHeaders() As String
	Private mdaColWidthsAAA() As Double
	Private miColumnUB As Integer
	Private miCurrentRow As Integer = 0
	Private mdDataRowHeight As Double = 5
	Private mdDataTextHeight As Double = 4
	Private miHeaderRowCount As Integer
	'  Private moBasePoint As Point3d

	Private mdReportHeight As Double
	Public Sub New()
		MyBase.dbAcadModel = True
	End Sub


	Public Sub New(ByVal iReportID As TPlServerDB.enResourceTheme)
		MyBase.ResourceTheme = iReportID
		MyBase.dbAcadModel = True
		Try
			BaseReport.BaseResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(ResourceTheme)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "ResourceTheme: " & CStr(ResourceTheme), "Report-Init0")
		End Try


	End Sub
	Public Function Insert_160815() As Boolean 'Overrides
		Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
		Dim saPrompt(1) As String
		'  Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock

		'''''''''''''''''''''''''	DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
		saPrompt(0) = "Enter start point of the report"
		saPrompt(1) = vbCrLf & "Enter end point of the report"

		oaPoints = DMAcadExt.AcadUtil.GetTwoPoints(saPrompt)
		If oaPoints IsNot Nothing Then

			RepApp.SetPoints(oaPoints)
			If BaseResource IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(CStr(999), "05_090")
				zzLayout2009()
				'   System.Windows.Forms.MessageBox.Show(CStr(666), "05_096")
			End If
			Return True

			'	Report.PaintCells()



			'   oDocLock.Dispose()
		Else
			Return False
		End If

		''''''''''   Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", False, False, False)
	End Function
	Public Overrides Function Insert() As Boolean
		If RepApp.HasStartPoint AndAlso BaseResource IsNot Nothing Then

			zzLayout2009()

			'	Report.PaintCells()

			Return True
		Else
			Return False
		End If

		''''''''''   Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", False, False, False)
	End Function
	Public Overrides Function InsertMulti() As Boolean
		Dim oaPoints As Autodesk.AutoCAD.Geometry.Point3d()
		Dim saPrompt(1) As String
		'  Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock

		'''''''''''''''''''''''''	DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
		saPrompt(0) = "Enter start point of the report"
		saPrompt(1) = vbCrLf & "Enter end point of the report"

		oaPoints = DMAcadExt.AcadUtil.GetTwoPoints(saPrompt)
		If oaPoints IsNot Nothing Then

			RepApp.SetPoints(oaPoints)
			If BaseResource IsNot Nothing Then

				zzLayout2009()

			End If
			Return True

			'	Report.PaintCells()



			'   oDocLock.Dispose()
		Else
			Return False
		End If
	End Function
	Public Function GetColorCells() As Autodesk.AutoCAD.Geometry.Point3dCollection()
		Return RepApp.GetColorCells()
	End Function
	Public Function GetColorScheme() As DMAcadExt.ColorScheme()
		Return RepApp.GetColorScheme()
	End Function
	Public Sub ClearZebraCells()
		RepApp.ClearZebraCells()
	End Sub
	Public Overrides Function Open(Optional ByVal iResourceTheme As TPlServerDB.enResourceTheme = 0) As Boolean
		Return MyBase.OnOpen(iResourceTheme)

	End Function

	Private Sub zzLayout2009()
		Dim oResource As TPlServerDB.TPlResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(diResourceTheme)
		Dim sRepStatus As String = BaseResource.ResItems(0)

		Dim iColumnLB As Integer = 0
		Dim iRowLB As Integer = 0
		Dim dTitleHeight As Double
		Dim dHeaderHeight As Double
		Dim dFooterHeight As Double
		Dim bContinue As Boolean
		' Dim iERow As Integer

		Dim oFooter As Section = Nothing
		Dim iRow As Integer
		Dim bTemp As Boolean = True
		Dim oTitleResource As TPlServerDB.TPlResource = BaseResource.GetChild(0)
		If doaOptionValues IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessage(DMCommon.Functions.DispArray("doaOptionValues", doaOptionValues, False))
		End If

		Dim oTitle As TitleSection = New TitleSection(oTitleResource, doaOptionValues, ddicOptionValues)
		Dim oHeader As Section = Nothing
		Dim oData As DataSection = Nothing

		oTitle.Format()
		dTitleHeight = oTitle.GetTitleHeight()
		'	dTitleHeight = Report.TitleHeight

		RepApp.CalcBasePoint(dTitleHeight)
		Dim oHeaderResource As TPlServerDB.TPlResource = BaseResource.GetChild(1)
		'   DMCommon.Functions.DispArray(doaCaptions, "doaCaptions", True)
		'   DMCommon.Functions.DispArray(diaDataColumns, "diaDataColumns")





		If oHeaderResource IsNot Nothing Then
			oHeader = New HeaderSection(oHeaderResource, dbAreaMeter, doaCaptions)
			oHeader.Format()
		End If
		'  
		Dim oDataResource As TPlServerDB.TPlResource = BaseResource.GetChild(2)

		If bTemp Then
			If oDataResource IsNot Nothing Then
				oData = New DataSection(oDataResource, doMainView, dbAreaMeter, dbMergeRows, diaDataColumns)
				oData.Format()
			End If
		End If


		Dim oFooterResource As TPlServerDB.TPlResource = BaseResource.GetChild(3)
		If oFooterResource Is Nothing Then
			oFooter = New HeaderSection(dbAreaMeter)
		Else
			oFooter = New HeaderSection(oFooterResource, dbAreaMeter, doaTotals)
		End If
		oFooter.Format()
		RepApp.NewTable()

		dFooterHeight = oFooter.GetSectionHeight()

		RepApp.DrawTable()
		oTitle.Print()

		If oHeader IsNot Nothing Then
			oHeader.Print()
			iRow = oHeader.LastRow
		End If

		dHeaderHeight = oHeader.GetSectionHeight()

		If bTemp Then
			If oData IsNot Nothing Then
				oData.SetHeaderFooterHeight(dHeaderHeight, dFooterHeight)

				oData.FirstRow = iRow + 1
				oData.MergeReset()
				oData.Print()
			End If
		End If

		If bContinue Then

			'	Report.DrawTable()
			'	Report.NextPage()
		End If
		If bTemp Then
			iRow = oData.LastRow
		End If

		If True Then
			If oFooter IsNot Nothing Then

				oFooter.FirstRow = iRow + 1

				oFooter.Print()

			End If
		End If

		If diaEmptyColumns IsNot Nothing Then

			RepApp.DelEmptyColumns(diaEmptyColumns)
		Else


		End If

	End Sub





	Public Overrides Sub NextTable()

	End Sub

	Public Overrides Function InsertSpec() As Boolean
		Throw New NotImplementedException()
	End Function
End Class
