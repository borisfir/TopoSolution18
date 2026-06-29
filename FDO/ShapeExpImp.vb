Option Explicit On
Option Strict On
Public Enum enExpImp
	Export
	Import
End Enum

Public Class ShapeExpImp
	Const msShapeFormat = "SHP"
	Private moExporter As Autodesk.Gis.Map.ImportExport.Exporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter
	Private moImporter As Autodesk.Gis.Map.ImportExport.Importer = Autodesk.Gis.Map.HostMapApplicationServices.Application.Importer
	Private miExpImp As enExpImp
	Private msTopoName As String
	Private msShapeFileName As String
	Private mtResExp As Autodesk.Gis.Map.ImportExport.ExportResults
	Private mtResImp As Autodesk.Gis.Map.ImportExport.ImportResults

	Public Sub New(iExpImp As enExpImp, sShapeFileName As String)
		Try
			Dim oFile As IO.FileInfo = New IO.FileInfo(sShapeFileName)
			' System.Windows.Forms.MessageBox.Show(oFile.Exists.ToString(), "09_889")
			'	DMCommon.Debug.MsgBox("13_028C", oFile.FullName, iExpImp, msShapeFormat, sShapeFileName)
			msShapeFileName = sShapeFileName
			miExpImp = iExpImp
			If miExpImp = enExpImp.Export Then
				moExporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Exporter

				'System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked) & vbCrLf & DMAcadExt.AcadTransaction.IsActive.ToString() & vbCrLf & moExporter.FeatureClassFilter.ToString() & vbCrLf & msShapeFormat & vbCrLf & sShapeFileName, "01_824x")
				moExporter.Init(msShapeFormat, sShapeFileName)

			ElseIf miExpImp = enExpImp.Import Then
				moImporter = Autodesk.Gis.Map.HostMapApplicationServices.Application.Importer
				moImporter.Init(msShapeFormat, sShapeFileName)
				moImporter.AuditClassifiedAfterImport = True

			End If

		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			Dim sExpMsg As String = oMapImpExpEx.Message
			If oMapImpExpEx.ErrorCode <> 0 Then
				sExpMsg &= vbCrLf & CStr(oMapImpExpEx.ErrorCode)
			End If
			DMCommon.Debug.MsgBox("01_809c", miExpImp, oMapImpExpEx.ErrorCode.ToString(), sExpMsg, sShapeFileName)
		End Try
	End Sub
	Public Sub LoadFomat(sFomatFile As String)
		If miExpImp = enExpImp.Export Then
			moExporter.LoadExportFormat(sFomatFile)
		ElseIf miExpImp = enExpImp.Import Then
			moImporter.LoadImportFormat(sFomatFile)
		End If
	End Sub
	Public Sub Exec()
		'	System.Windows.Forms.MessageBox.Show(CStr(moExporter.ExportAll) & vbCrLf & moExporter.LayerFilter & vbCrLf & moExporter.FeatureClassFilter & vbCrLf & CStr(iPoints) & vbCrLf & CStr(iPgons) & vbCrLf & CStr(iTotal), "01_717")

		If miExpImp = enExpImp.Export Then
			'Dim iPoints, iLines, iPgons, iText, iTotal As Integer
			Try
				mtResExp = moExporter.Export()
			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show(msShapeFileName & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), "Export Err")

			End Try

			'From 2012  moExporter.CountObjects(iPoints, iLines, iPgons, iText, iTotal)
			'	System.Windows.Forms.MessageBox.Show(CStr(iPoints) & ":" & CStr(iPoints) & ":" & CStr(iPoints) & ":" & CStr(iPoints) & ":" & CStr(iPoints), "02_455")
			zzPrintRes(mtResExp.EntitiesExported, " exported")
		ElseIf miExpImp = enExpImp.Import Then
			Try
            mtResImp = moImporter.Import(True)

			Catch oMapEx As Autodesk.Gis.Map.MapException
				System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & CStr(oMapEx.ErrorCode) & vbCrLf & DMAcadExt.AcadErrCode.GetMapErrText(oMapEx.ErrorCode), "Import Err")
			End Try
         zzPrintRes(mtResImp.EntitiesImported, " imported")
         mtResImp = Nothing
		End If
	End Sub
	Public Sub FromTopology(sTopoName As String)
		msTopoName = sTopoName
		'	System.Windows.Forms.MessageBox.Show("FromTopology: " & sTopoName, "08_705")
		Try
			moExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, String.Empty)
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_712")
		End Try

		If Not String.IsNullOrEmpty(sTopoName) Then
			Try
				'System.Windows.Forms.MessageBox.Show("Locked: " & DMAcadExt.AcadDocument.IsLocked & vbCrLf & sTopoName, "08_898-kl")
				moExporter.SetExportFromPolygonTopology(False, sTopoName)
			Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
				System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode & vbCrLf & sTopoName, "01_719")
			End Try
		End If
		'	moExporter.LayerFilter = "0"
		moExporter.ExportAll = False
		moExporter.ClosedPolylinesAsPolygons = False
		'	moExporter.InvokeDriverOptionsDialog()
		'	System.Windows.Forms.MessageBox.Show(moExporter.LayerFilter & vbCrLf & "ExportAll " & CStr(moExporter.ExportAll) & vbCrLf & sTopoName, "08_610")
		'	Dim oLayers As Autodesk.Gis.Map.ImportExport.NameValueCollection = Nothing
		'	moExporter.LayerLevelMapping(True, oLayers)
		Dim colPlines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
		moExporter.SetSelectionSet(colPlines)
		'	moExporter.ClassMappingType = Autodesk.Gis.Map.ImportExport.ExportClassMappingType.ByObjectClass
	End Sub
	Public Sub FromPoligons(sLayerList As String)
		Try
			moExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, String.Empty)
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_711b")
		End Try
		moExporter.ExportAll = False
		moExporter.LayerFilter = sLayerList
		moExporter.ExportAll = False
		'	System.Windows.Forms.MessageBox.Show(moExporter.LayerFilter, "08_145")

		moExporter.ClosedPolylinesAsPolygons = True
	End Sub
   Public Function FromPoligons(colPgones As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As Boolean
      Dim oAcadObject As Autodesk.AutoCAD.DatabaseServices.DBObject
      Dim sErrMsg As String
      Try
         moExporter.SetStorageOptions(Autodesk.Gis.Map.ImportExport.StorageType.FileOneEntityType, Autodesk.Gis.Map.ImportExport.GeometryType.Polygon, String.Empty)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_711a")
      End Try
      moExporter.ExportAll = False
      moExporter.ClosedPolylinesAsPolygons = True
      '	moExporter.LayerFilter = ""
      Try
         moExporter.SetSelectionSet(colPgones)
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         If colPgones.Count = 1 Then
            oAcadObject = DMAcadExt.AcadTransaction.GetDBObject(colPgones.Item(0), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
            sErrMsg = oAcadObject.Handle.ToString()
         Else
            sErrMsg = "Count=" & CStr(colPgones.Count)
         End If
         System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sErrMsg & vbCrLf & colPgones.Count.ToString(), "01_717a")
         Return False
      End Try
      Return True
   End Function

	Public Sub AddTopoData(sIDName As String, sAreaName As String)
		'Return
		Try
			Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
			colExpressionTarget = moExporter.GetExportDataMappings()
			'	System.Windows.Forms.MessageBox.Show(CStr(colExpressionTarget.Count) & vbCrLf & msTopoName, "01_807")
			colExpressionTarget.Clear()
			colExpressionTarget.Add(":ID@TPMCNTR_" & msTopoName, sIDName)
			colExpressionTarget.Add(":AREA@TPMCNTR_" & msTopoName, sAreaName)
			moExporter.SetExportDataMappings(colExpressionTarget)
		Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
			System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_814")

			Return
		End Try
	End Sub
	Public Sub AddAgamLayerData(sODTableName As String)
      Try

         Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
         colExpressionTarget = moExporter.GetExportDataMappings()

         colExpressionTarget.Clear()
         colExpressionTarget.Add(":AGAM_ID@" & sODTableName, "AGAM_ID")
         colExpressionTarget.Add(":TASRIT@" & sODTableName, "TASRIT")
         colExpressionTarget.Add(":MAVAT_COD@" & sODTableName, "MAVAT_COD")
         colExpressionTarget.Add(":TYPE_NAME@" & sODTableName, "TYPE_NAME")
         colExpressionTarget.Add(":LABEL@" & sODTableName, "LABEL")
         colExpressionTarget.Add(":STATUS@" & sODTableName, "STATUS")
         colExpressionTarget.Add(":PL_CHANGE@" & sODTableName, "PL_CHANGE")
         colExpressionTarget.Add(":STAGE@" & sODTableName, "STAGE")
         colExpressionTarget.Add(":DATE_DEC@" & sODTableName, "DATE_DEC")
         colExpressionTarget.Add(":NAME@" & sODTableName, "NAME")
         colExpressionTarget.Add(":PLAN_NAME@" & sODTableName, "PLAN_NAME")

         colExpressionTarget.Add(":SOURCE_COD@" & sODTableName, "SOURCE_COD")
         colExpressionTarget.Add(":DEFQ@" & sODTableName, "DEFQ")
         colExpressionTarget.Add(":PLANE_NO@" & sODTableName, "PLANE_NO")
         colExpressionTarget.Add(":ADDRESS@" & sODTableName, "ADDRESS")
         colExpressionTarget.Add(":RADIUS@" & sODTableName, "RADIUS")
         colExpressionTarget.Add(":LENGTH@" & sODTableName, "LENGTH")
         colExpressionTarget.Add(":WIDTH@" & sODTableName, "WIDTH")
         colExpressionTarget.Add(":REMARKS@" & sODTableName, "REMARKS")
         colExpressionTarget.Add(":PRIMETER@" & sODTableName, "PRIMETER")
         colExpressionTarget.Add(":AREA@" & sODTableName, "AREA")
         colExpressionTarget.Add(":TYPE_CODE@" & sODTableName, "TYPE_CODE")
         moExporter.SetExportDataMappings(colExpressionTarget)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_814")

         Return
      End Try
   End Sub
   Public Sub AddLotLanduseData(sODTableName As String, sTopoName As String)
      '    Dim oHebText As DMCommon.HebrewTrans
      Try
         Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
         colExpressionTarget = moExporter.GetExportDataMappings()
         '    System.Windows.Forms.MessageBox.Show(":AREA@TPMCNTR_" & sTopoName & vbCrLf & "", "01_812a")
         colExpressionTarget.Clear()
         colExpressionTarget.Add(":LanduseID@" & sODTableName, "LanduseID")
         colExpressionTarget.Add(":LuseName@" & sODTableName, "LuseName")
         colExpressionTarget.Add(":PlanName@" & sODTableName, "PlanName")

         '    51 001 50
         If Not String.IsNullOrEmpty(sTopoName) Then
            colExpressionTarget.Add(":AREA@TPMCNTR_" & sTopoName, "AREA")
         End If


         moExporter.SetExportDataMappings(colExpressionTarget)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_814")

         Return
      End Try
   End Sub
   Public Sub AddImpShapeData()

      Try
         Dim tableType As Autodesk.Gis.Map.ImportExport.ImportDataMapping
         Dim tableName As String
         tableType = Autodesk.Gis.Map.ImportExport.ImportDataMapping.NewObjectDataOnly
         Dim layer As Autodesk.Gis.Map.ImportExport.InputLayer = Nothing
         Dim iIndex As Integer
         Dim iImportDataMapping As Autodesk.Gis.Map.ImportExport.ImportDataMapping
         Dim s As String = "N"
         Dim s1 As String = "V"

         Dim newFieldName As String = Nothing


         For Each layer In moImporter
            '  System.Windows.Forms.MessageBox.Show(layer.Name, "09_877")
            tableName = GetODTableName(layer.Name)
            layer.SetDataMapping(tableType, tableName)
            For Each col As Autodesk.Gis.Map.ImportExport.Column In layer
					'From 2012 s = col.ColumnDataMapping(iImportDataMapping)
					If iIndex <> 0 Then
                  iImportDataMapping = ImportExport.ImportDataMapping.LinkOnly
                  iImportDataMapping = ImportExport.ImportDataMapping.ExistingObjectDataOnly
                  iImportDataMapping = ImportExport.ImportDataMapping.NewObjectDataOnly

                  newFieldName = String.Concat("dm_", col.ColumnName)
						'From 2012 s1 = col.ColumnDataMapping(iImportDataMapping)
						col.SetColumnDataMapping(newFieldName)
                  '  System.Windows.Forms.MessageBox.Show(col.ColumnName & vbCrLf & col.ColumnClassMapping & vbCrLf & s & vbCrLf & s1 & vbCrLf & iImportDataMapping.ToString(), "09_878")
                  '  col.SetColumnDataMapping(newFieldName)
               Else
                  ''''''''  col.Dispose()
               End If

             
               iIndex += 1
            Next col
         Next

      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_815")

         Return
      End Try




   End Sub
   Private Function GetODTableName(sLayerName As String) As String
      Dim saName() As String = Split(sLayerName, ":")
      If saName.GetUpperBound(0) = 0 Then
         Return sLayerName
      Else
         Return saName(1)
      End If
   End Function

   Public Sub AddShapeData(iDataset As Integer)

      Try
         Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
         colExpressionTarget = moExporter.GetExportDataMappings()

         colExpressionTarget.Clear()
         Select Case iDataset
            Case 1
               colExpressionTarget.Add(".EHANDLE", "ID")
               colExpressionTarget.Add("^AcDbMPolygon.Misc.""Total Area""", "AREA")
            Case 2
               colExpressionTarget.Add(":Taba_Num@BlueLines", "Taba_Num")
               colExpressionTarget.Add(":Taba_Numer@BlueLines", "Taba_Numer")

               colExpressionTarget.Add(":taba_name@BlueLines", "taba_name")
               '  colExpressionTarget.Add(":Date@BlueLines", "Date")
         End Select

         moExporter.SetExportDataMappings(colExpressionTarget)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_814")

         Return
      End Try
   End Sub
   Public Sub AddHandleData()
      Try
         Dim colExpressionTarget As Autodesk.Gis.Map.ImportExport.ExpressionTargetCollection
         colExpressionTarget = moExporter.GetExportDataMappings()

         colExpressionTarget.Clear()
         colExpressionTarget.Add(".EHANDLE", "ID")
         moExporter.SetExportDataMappings(colExpressionTarget)
      Catch oMapImpExpEx As Autodesk.Gis.Map.MapImportExportException
         System.Windows.Forms.MessageBox.Show(oMapImpExpEx.Message & vbCrLf & oMapImpExpEx.ErrorCode, "01_815")

         Return
      End Try
   End Sub
   Public Sub Dispose()
      If miExpImp = enExpImp.Export Then

         moExporter.Dispose()
      Else
         moImporter.Dispose()
         moImporter = Nothing
      End If
   End Sub
   Public Sub AddLayerFilter(sList As String)
      moExporter.LayerFilter = sList
   End Sub
   Public Sub SetDestLayer(sLayer As String)
      Dim oEnum As IEnumerator
      Dim oInputLayer As Autodesk.Gis.Map.ImportExport.InputLayer = Nothing
		Dim oDynamic As System.Object
		Dim iLayerNameType As Autodesk.Gis.Map.ImportExport.LayerNameType
      Dim sDefaultLayer As String = Nothing
      oEnum = moImporter.GetEnumerator()
      oEnum.Reset()
      Do While oEnum.MoveNext
         oDynamic = oEnum.Current
         If oDynamic Is Nothing Then
            DMAcadExt.AcadDocument.WriteMessage(" 130: " & " oDynamic Is Nothing:" & vbCrLf)
         Else
            oInputLayer = DirectCast(oDynamic, Autodesk.Gis.Map.ImportExport.InputLayer)
				'From 2012   oInputLayer.LayerName(iLayerNameType, sDefaultLayer)
				oInputLayer.SetLayerName(iLayerNameType, sLayer)
            oInputLayer.ImportFromInputLayerOn = True
         End If
      Loop
   End Sub
   Public Property ImportPolygonsAsClosedPolylines As Boolean
      Get
         Try
            Return moImporter.ImportPolygonsAsClosedPolylines
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "01_817")
            Return False
         End Try
      End Get
      Set(bValue As Boolean)
         Try
            moImporter.ImportPolygonsAsClosedPolylines = bValue
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "01_818")

         End Try
      End Set
   End Property

   Public ReadOnly Property PolygonCount As UInteger
      Get
         If miExpImp = enExpImp.Export Then
            Return mtResExp.EntitiesExported
         ElseIf miExpImp = enExpImp.Import Then
            Return mtResImp.EntitiesImported
         Else
            Return 0
         End If
      End Get
   End Property
   Private Sub zzPrintRes(iCount As UInteger, sAction As String)
      Dim sMsg As String
      If Not String.IsNullOrEmpty(msTopoName) Then
         sMsg = "Topology '" & msTopoName & "': "
      Else
         sMsg = String.Empty
      End If

      If iCount = Convert.ToUInt32(1) Then
         sMsg &= "one polygon"
      Else
         sMsg &= CStr(iCount) & " polygons"
      End If
      sMsg &= sAction
      DMAcadExt.AcadDocument.WriteMessage(sMsg)
   End Sub
End Class
