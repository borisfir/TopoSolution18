Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Public Class frmDWGBatch
	Dim moExporter As Autodesk.Gis.Map.ImportExport.Exporter
	Dim mcolExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection

	Private milvwDWGsLocationY As Integer

	Private Sub cmdAddFiles_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdAddFiles.Click
		Me.ofdDWG.FileName = "*.DWG"
		Me.ofdDWG.ShowDialog()
		If Me.ofdDWG.FileName <> "" Then
			For iIndex As Integer = 0 To Me.ofdDWG.FileNames.GetUpperBound(0)
				Me.lvwDWGs.Items.Add(Me.ofdDWG.FileNames(iIndex))
			Next
		End If
	End Sub

	Private Sub cmdAddDirectory_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdAddDirectory.Click
		Me.fbdDWGs.ShowDialog()
		If fbdDWGs.SelectedPath.Length <> 0 Then

		End If
	End Sub

	Private Sub zzInitExporter()
		Try
			moExporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter


			'mcolExpressionTarget = moExporter.GetExportDataMappings()
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "frmDWGBatch - zzInitExporter")
		End Try

	End Sub
	Private Sub zzExec()
		' 	Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
      Const sDBResourceFile As String = "P:\AppData\TopoSolution\tblData.accdb" '"\\Olympus\Project\AppData\TopoSolution\tblData.mdb"
		'	Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
		'	Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
      '  Const sServerName As String = "ARCGIS-VM"
      Const sServerName As String = "Pluto"

      Const sDatabaseName As String = "AshqelonDB" '"Ashqelon" "Ashqelon0214"
      Const sRootPath As String = "P:\2014\140164\new plans\new plans 12.7.2017\" '"P:\2014\140164\new plans\new plans 8.06.2016\" '   "P:\2010\100284\Plan\"
		Const sBaseTopoName As String = "Plan"
		components = New System.ComponentModel.Container
		Dim oListViewItem As ListViewItem
		Dim oListViewSubItem As ListViewItem.ListViewSubItem
		Dim sDWGName As String
		Dim iPlanID As Integer
		Dim sPlanNum As String
		Dim sShapeName As String
		Dim oFileInfo As System.IO.FileInfo
		Dim sPrjFile As String = "Fix.prj"
		Dim sDestPrjFile As String
		Dim sTopoName As String
		DMAcadExt.AcadDocument.LogName = sRootPath & "TopoMsg.log"

		TPlServerDB.ServerDB.InitCurrentServer()
      '  TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, "")
      TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName, True)


		TPlServerDB.ServerDB.InitCurrentProject()
      TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName, True)
		zzInitExporter()
		Dim iFileStatus As Integer
		Dim i As Integer = 0
		Dim iPgonCount As UInteger
      Dim oReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader("SELECT ObjectID,PlanNum,DWGName FROM PlanMain WHERE Portion=1")
		If oReader IsNot Nothing Then
			DMAcadExt.AcadDocument.WriteMessageLog("Start")
         While oReader.Read

            i += 1
            If i > 3000 Then Exit While
            If Not oReader.IsDBNull(2) Then
               sDWGName = sRootPath & oReader.GetString(2)
               sDWGName = sDWGName.Trim()
               iPlanID = oReader.GetInt32(0)
               sPlanNum = oReader.GetString(1)
               DMAcadExt.AcadDocument.WriteMessageLog("***" & sPlanNum & "***")
               oListViewItem = Me.lvwDWGs.Items.Add(sDWGName)
               oFileInfo = New System.IO.FileInfo(sDWGName)

               If oFileInfo.Exists Then

                  oListViewSubItem = oListViewItem.SubItems.Add("Loading ...")
                  DMAcadExt.AcadDocument.OpenDocument(sDWGName, False)

                  DMAcadExt.AcadDocument.Reset()
                  oListViewSubItem.Text = "Success"

                  oListViewSubItem = oListViewItem.SubItems.Add("Creating ...")
                  sTopoName = sBaseTopoName & CStr(iPlanID)
                  iFileStatus = 0
                  If Not TopoManager.TopoCreator.TopologyExists(sTopoName) Then
                     DMAcadExt.AcadDocument.WriteLog(sDWGName)
                     iFileStatus += 1
                     DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
                     DMAcadExt.AcadTransaction.Start()
                     DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
                     DMAcadExt.AcadDocument.WriteMessageLog("FileName - '" & sDWGName & "'")
                     TopoManager.TopoCreator.CreateTopology(sTopoName, "pCellK", 0.01)
                     DMAcadExt.AcadTransaction.CloseModelSpace()
                     DMAcadExt.AcadTransaction.Terminate()
                     DMAcadExt.AcadDocument.Unlock()
                  End If


                  If TopoManager.TopoCreator.TopologyExists(sTopoName) Then
                     If iFileStatus = 1 Then
                        DMAcadExt.AcadDocument.WriteLog("OK!!!")
                     End If
                     iFileStatus += 2
                     oListViewSubItem.Text = "Success"
                     sShapeName = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length) & ".shp"
                     oListViewSubItem = oListViewItem.SubItems.Add("Exporting ...")
                     '	If i < 3 Then
                     iPgonCount = zzExportToShape(sShapeName, sTopoName, iPlanID, sPlanNum)
                     'i += 1
                     'End If
                     If iPgonCount > 0 Then
                        oListViewSubItem.Text = "Success"
                        oListViewSubItem = oListViewItem.SubItems.Add(iPgonCount.ToString())
                     Else
                        oListViewSubItem.Text = "Failed"
                        oListViewSubItem = oListViewItem.SubItems.Add("0")
                     End If

                  Else
                     '    System.Windows.Forms.MessageBox.Show("", "07_021")
                     oListViewSubItem.Text = "Error"
                     oListViewSubItem = oListViewItem.SubItems.Add("-")
                     oListViewSubItem = oListViewItem.SubItems.Add("-")
                  End If
                  sDestPrjFile = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length) & ".prj"
                  oListViewSubItem = oListViewItem.SubItems.Add("Closing ...")
                  If iFileStatus = 3 Then
                     DMAcadExt.AcadDocument.CloseAndSaveActiveDocument(sDWGName)
                  Else
                     DMAcadExt.AcadDocument.CloseAndDiscardActiveDocument()
                  End If
                  '   System.Windows.Forms.MessageBox.Show(sRootPath & sPrjFile & vbCrLf & sDestPrjFile, "07_023")
                  Try
                     FileCopy(sRootPath & sPrjFile, sDestPrjFile)
                  Catch oEx As Exception
                     System.Windows.Forms.MessageBox.Show(oEx.Message, "frmDWGBatch - zzExec")

                  End Try
                  '   System.Windows.Forms.MessageBox.Show(sRootPath & sPrjFile & vbCrLf & sDestPrjFile, "07_025")
                  oListViewSubItem.Text = "Success"
                  Me.Focus()
                  Me.lvwDWGs.Focus()
               Else
                  System.Windows.Forms.MessageBox.Show("File '" & sDWGName & "' was not found", "frmDWGBatch - zzExec")

               End If
            End If
         End While
		End If
		oReader.Close()
		mcolExpressionTarget.Dispose()
		moExporter.Dispose()
		TPlServerDB.ServerDB.CurrentServerDB.Close()
		TPlServerDB.ServerDB.CurrentProjectDB.Close()

	End Sub
	Private Sub zzExecMacroN()
		'Const sRoot As String = "P:\2008\080756\REGISTER\talar\Boris\"
		'Const sRoot As String = "P:\2008\080756\REGISTER\talar\ver-sofi\"
		Const sRoot As String = "P:\2010\100022\register\to-boris\"

		Const sTemplate As String = "TemplateArea"
		Const sBlockName As String = "PCLS003"
		Const sDWGPrefix As String = "UD"
		Const sExcelClass As String = "Excel.Application"
		Dim sTemplateFile As String = sRoot & sTemplate & ".XLS"
		Dim sDWGName As String
		Dim sXLSName As String
		Dim sGushName As String
		Dim sParcelName As String
		Dim sLegalArea, sCalcArea As String
		Dim dLegalArea, dCalcArea As Double

		Dim oRootFolder As IO.DirectoryInfo = New IO.DirectoryInfo(sRoot)
		Dim oaFolders As System.IO.DirectoryInfo() = oRootFolder.GetDirectories()
		Dim oaInnerFolders As System.IO.DirectoryInfo()
		Dim oFolder As System.IO.DirectoryInfo
		Dim oInnerFolder As System.IO.DirectoryInfo

		Dim colBlockRefIds As ObjectIdCollection
		Dim bAcadPoint As Boolean
		Dim iaAttribIndices() As Integer = {0, 2, 3}
		Dim sAttrTexts() As String

		Dim oExcelObject As System.Object
		Dim oExcelApp As Microsoft.Office.Interop.Excel.Application
		Dim oWorkbook As Microsoft.Office.Interop.Excel.Workbook
		Dim oWorksheet As Microsoft.Office.Interop.Excel.Worksheet
		Dim iRow As Integer
		Dim oListViewItem As ListViewItem
		Dim oListViewSubItem As ListViewItem.ListViewSubItem
		Dim bOpened As Boolean
		Dim oFileInfo As IO.FileInfo
		Try
			oExcelObject = GetObject(, sExcelClass)
		Catch oEx As Exception
			Try
				oExcelObject = GetObject(String.Empty, sExcelClass)
			Catch oExA As Exception
				System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - zzInitExcel_1")
				Return
			End Try
		End Try
		Try
			oExcelApp = DirectCast(oExcelObject, Microsoft.Office.Interop.Excel.Application)
			oExcelApp.Visible = True
		Catch oExA As Exception

			System.Windows.Forms.MessageBox.Show(oExA.Message, "Application - zzInitExcel_2")
			Return
		End Try

		'''''''''''''''''''Dim sDoc0 As String = DMAcadExt.AcadDocument.GetCurrentDWGName()
		'	DMAcadExt.AcadDocument.CloseCurrentDocument(False)
		Dim sAddPath As String
		For iIndex As Integer = 0 To oaFolders.GetUpperBound(0)
			bOpened = False
			oFolder = oaFolders(iIndex)
			sGushName = oFolder.Name
			oaInnerFolders = oFolder.GetDirectories()
			If oaInnerFolders.GetUpperBound(0) = 0 Then
				oInnerFolder = oaInnerFolders(0)
				sAddPath = "\" & oInnerFolder.Name
			ElseIf oaInnerFolders.GetUpperBound(0) = -1 Then
				sAddPath = String.Empty
			Else
				System.Windows.Forms.MessageBox.Show(sGushName & vbCrLf & CStr(oaInnerFolders.GetUpperBound(0)), "117")
				sAddPath = ""
			End If
			sDWGName = sRoot & sGushName & sAddPath & "\" & sDWGPrefix & sGushName & ".DWG"
			sXLSName = sRoot & sGushName & sAddPath & "\" & sDWGPrefix & sGushName & ".XLS"
			oListViewItem = Me.lvwDWGs.Items.Add(CStr(iIndex) & " " & sDWGName)
			'	DMAcadExt.AcadDocument.WriteMessage("sDWGName: " & CStr(sDWGName))
			oFileInfo = New IO.FileInfo(sXLSName)
			If oFileInfo.Exists Then
				oListViewSubItem = oListViewItem.SubItems.Add("Exists")
				oFileInfo.Delete()
			End If
			Try
				DMAcadExt.AcadDocument.OpenDocument(sDWGName, False)
				bOpened = True
				'	DMAcadExt.AcadDocument.Reset()
				'	System.Windows.Forms.MessageBox.Show(sDWGName, "111_OK")
			Catch oEx As Exception
				bOpened = False
				oListViewSubItem = oListViewItem.SubItems.Add("Error ...")
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "112")
				'Return
			End Try
			If bOpened Then


				DMAcadExt.AcadTransaction.Start()
				If True Then
					colBlockRefIds = DMAcadExt.AcadTransaction.GetBlockRefs(sBlockName)
					FileCopy(sTemplateFile, sXLSName)
					Try

						oWorkbook = oExcelApp.Workbooks.Open(sXLSName)
						oExcelObject = oWorkbook.ActiveSheet
						oWorksheet = DirectCast(oExcelObject, Microsoft.Office.Interop.Excel.Worksheet)
						oWorksheet.Name = sGushName
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf, "Application - OpenApplication")
						Return
					End Try
					'	System.Windows.Forms.MessageBox.Show(CStr(colBlockRefIds.Count) & vbCrLf, "Application - OpenApplication")
					iRow = 2
					For Each tAcObjId As ObjectId In colBlockRefIds
						sAttrTexts = DMAcadExt.AcadTransaction.GetAttribText(tAcObjId, True, bAcadPoint, iaAttribIndices)
						sParcelName = sAttrTexts(0)
						sCalcArea = sAttrTexts(1)
						sLegalArea = sAttrTexts(2)
						dCalcArea = Val(sCalcArea)
						dLegalArea = Val(sLegalArea)
						oWorksheet.Cells.Item(iRow, 1) = sGushName
						oWorksheet.Cells.Item(iRow, 2) = sParcelName
						oWorksheet.Cells.Item(iRow, 3) = dCalcArea
						oWorksheet.Cells.Item(iRow, 4) = dLegalArea

						iRow += 1
					Next
					For iRow0 As Integer = iRow To 301
						For iCol As Integer = 1 To 8
							oWorksheet.Cells.Item(iRow0, iCol) = ""
						Next
					Next
					oWorkbook.Save()
					oWorkbook.Close()

				End If
				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.CloseAndDiscardActiveDocument()
				oListViewSubItem = oListViewItem.SubItems.Add("Success")
			End If

		Next




	End Sub
	Private Sub zzInputParcelLines()
		prjAshqelon.DB.Open()
		Dim oParcels As prjAshqelon.Parcels = New prjAshqelon.Parcels()
		oParcels.InputParcelLines()


		prjAshqelon.DB.Close()
	End Sub
	Private Sub zzInputParcelCents()
		prjAshqelon.DB.Open()
		Dim oParcels As prjAshqelon.Parcels = New prjAshqelon.Parcels()
		oParcels.InputParcelCentroids()


		prjAshqelon.DB.Close()
	End Sub
	Private Function zzExportToShape(ByVal sFileName As String, ByVal sTopoName As String, ByVal iPlanID As Integer, ByVal sPlanNum As String) As UInteger


		'	Dim oExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
		'System.Windows.Forms.MessageBox.Show(CStr(moExporter Is Nothing), "05_112")
		Dim oRes As Autodesk.Gis.Map.ImportExport.ExportResults
		'	Dim sFileName As String = "E:\aWork\TestMapExp\p105.shp"
		'		Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
		'	Dim sTopoName As String = oTopoDef.Name

		'		System.Windows.Forms.MessageBox.Show(sFileName & vbCrLf & sTopoName, "01_807")
		Try

			moExporter.Init("SHP", sFileName)
			moExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, "")
			'		oMyExporter.LoadExportFormat()	'FileOneEntityType
			moExporter.SetExportFromPolygonTopology(True, sTopoName)
			If mcolExpressionTarget Is Nothing Then
				mcolExpressionTarget = moExporter.GetExportDataMappings()
			Else
				mcolExpressionTarget.Clear()
			End If



			mcolExpressionTarget.Add(CStr(iPlanID), "ID")
			'	mcolExpressionTarget.Add("'" & CStr(iPlanID), "ID")
			'	sPlanNum = Chr(254) & sPlanNum & Chr(254)
			mcolExpressionTarget.Add(sPlanNum, "PlanNum")
			mcolExpressionTarget.Add(":ID@TPMCNTR_" & sTopoName, "PolygonID")

			moExporter.SetExportDataMappings(mcolExpressionTarget)
			Dim mcolTestExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
			mcolTestExpressionTarget = moExporter.GetExportDataMappings()
			Dim oPair As Autodesk.Gis.Map.Utilities.StringPair = mcolTestExpressionTarget.Item("PlanNum")
			DMAcadExt.AcadDocument.WriteMessageLog("2***" & oPair.First & ":" & oPair.Second & "***")
			'	colExpressionTarget.Clear()
			'	colExpressionTarget.Dispose()



			'	oMyExporter.ExportFromPolygonTopology(False, sTopoName)
			oRes = moExporter.Export()
			Return oRes.EntitiesExported
			'	oExporter.Dispose()
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException

			System.Windows.Forms.MessageBox.Show(CStr(oMapImpExpEx.ErrorCode) & vbCrLf & oMapImpExpEx.StackTrace, "01_842")
			DMAcadExt.AcadErrCode.ShowMapError(oMapImpExpEx.ErrorCode, False, "frmDWGBatch - zzExportToShape")

		End Try

	End Function
	Private Sub cmdExec_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmdExec.Click
		zzExec()
	End Sub

	Private Sub frmDWGBatch_Resize(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles Me.Resize
		If milvwDWGsLocationY <> 0 Then
			Try
				Me.lvwDWGs.Height = Me.ClientSize.Height - milvwDWGsLocationY
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmDWGBatch - frmDWGBatch_Resize")
			End Try
		End If
	End Sub

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		milvwDWGsLocationY = Me.lvwDWGs.Location.Y

	End Sub

	Private Sub InputParcels_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdInputParcelLines.Click
		Me.zzInputParcelLines()
	End Sub

	Private Sub cmdInputParcelLCents_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdInputParcelLCents.Click
		zzInputParcelCents()
	End Sub

	Private Sub cmdMacroN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdMacroN.Click
		zzExecMacroN()

	End Sub
End Class
