Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.ObjectData
Imports System.Data
Namespace prjAshqelon
	Public Class DB

		Public Shared Sub Open()
			Const sServerName As String = "neptune"
			Const sDatabaseName As String = "Ashqelon"

			TPlServerDB.ServerDB.InitCurrentProject()
			TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName, True, False)

		End Sub
		Public Shared Sub Close()
			TPlServerDB.ServerDB.CurrentProjectDB.Close()
		End Sub
	End Class
	Public Class Parcels
		Const msODTableName As String = "helkot"
		Const msLineLayerName As String = "helkot"
		Const msCentroidLayerName As String = "PCLS003"
		Const msCentroidBlockName As String = "PCLS003"


		Private moODTable As Table
		Private moFieldDefinitions As Autodesk.Gis.Map.ObjectData.FieldDefinitions
		Private moDbDataAdapter As System.Data.Common.DbDataAdapter
		Private moLineDataTable As DataTable
		Private moCentDataTable As DataTable
		Private miaBlockAttribIndex(2) As Integer
		Private miParcelCounter As Integer = 0
		Public Sub InputParcelLines()
			Const iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead
			Dim sSelectComText As String = "SELECT * FROM AshqParcels"

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(iMode)
			Dim dlInParcelProc As DMAcadExt.AcadTransaction.Procedure = New DMAcadExt.AcadTransaction.Procedure(AddressOf zzInParcelLine)


			System.Windows.Forms.MessageBox.Show("", "Ver 006")


			moDbDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSelectComText, CommandType.Text, True)
			moLineDataTable = New DataTable()
			moDbDataAdapter.FillSchema(moLineDataTable, SchemaType.Source)

			DMAcadExt.AcadTransaction.EnumModelSpaceObjects(dlInParcelProc, iMode)
			System.Windows.Forms.MessageBox.Show(CStr(moLineDataTable.Rows.Count), "01_910")
			Try
				moDbDataAdapter.Update(moLineDataTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Err #7179")
			End Try


			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzInParcelLine(ByVal oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject)
			'		Const sBlockNoODFldName As String = "GUSH_NO"
			'		Const sParcelODFldName As String = "PARCEL"
			'		Const sLegalAreaODFldName As String = "LEGAL_AREA"
			'		Const sStatusIdODFldName As String = "STATUS_ID"
			'	Const sStatusNameODFldName As String = "STATUS"
			'		Const sPerimeterODFldName As String = "Shape_Leng"
			'	Const sAreaODFldName As String = "Shape_Area"

			Dim sRXClassName As String = oDBObject.GetRXClass().Name
			'	DMAcadExt.AcadDocument.WriteMessage("A: Parcel #" & CStr(miParcelCounter) & ":" & sRXClassName)
			Dim oFieldDef As FieldDefinition
			If sRXClassName = DMAcadExt.AcadConst.AcadPolylineName Then
				Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline = DirectCast(oDBObject, Autodesk.AutoCAD.DatabaseServices.Polyline)
				If oPolyline.Layer = msLineLayerName Then
					Dim oExt As Autodesk.AutoCAD.DatabaseServices.Extents3d
					Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
					Dim oODRecord As Record = Nothing
					Dim oNewRow As DataRow = moLineDataTable.NewRow()

					miParcelCounter += 1
					If miParcelCounter > 0 Then


						DMAcadExt.AcadDocument.WriteMessage("Parcel #" & CStr(miParcelCounter) & ":" & oDBObject.Handle.ToString())

						With oNewRow
							.Item(DBLineFldName.ParcelID) = miParcelCounter


							Try
								oExt = oPolyline.GeometricExtents()
								tPoint = oExt.MinPoint
								.Item(DBLineFldName.Xmin) = tPoint.X
								.Item(DBLineFldName.Ymin) = tPoint.Y
								tPoint = oExt.MaxPoint
								.Item(DBLineFldName.Xmax) = tPoint.X
								.Item(DBLineFldName.Ymax) = tPoint.Y
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message, "7331s")
							End Try


							Try
								.Item(DBLineFldName.Area) = oPolyline.Area
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 1")
								DMAcadExt.AcadDocument.WriteMessage("12 Parcel #" & CStr(miParcelCounter))
							End Try
							Try
								.Item(DBLineFldName.Perimeter) = oPolyline.Length
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 2")
								DMAcadExt.AcadDocument.WriteMessage("13 Parcel #" & CStr(miParcelCounter))
							End Try
							Try
								.Item(DBLineFldName.LineObjID) = oPolyline.ObjectId ''''''''''''.OldIdPtr.ToInt64()
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 3")
							End Try
							Try
								.Item(DBLineFldName.LineHandle) = oPolyline.Handle.Value
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 4")
							End Try


							Try
								'''''''''''''oODRecord = TopoManager.ODEditor.GetODRecord(oDBObject.ObjectId, msODTableName)

							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "zzInParcel_1")
							End Try
							oFieldDef = moFieldDefinitions.Item(3)
							If False AndAlso oODRecord IsNot Nothing AndAlso moFieldDefinitions IsNot Nothing Then	  'False AndAlso
								'	Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
								For iFieldIndex As Integer = 0 To moFieldDefinitions.Count - 1
									Try
										oFieldDef = moFieldDefinitions.Item(iFieldIndex)
										'oMapValue = oODRecord.Item(iFieldIndex)
										'	DMAcadExt.AcadDocument.WriteMessage(oFieldDef.Name & " - ind= " & CStr(iFieldIndex))
										Select Case oFieldDef.Name
											Case ODFldName.BlockNo
												''''''''''''DMAcadExt.AcadDocument.WriteMessage("ind= " & CStr(iFieldIndex))
												''''''''''''''''''''DMAcadExt.AcadDocument.WriteMessageLog(oMapValue.Type.ToString())
												''''''''''''''	DMAcadExt.AcadDocument.WriteMessageLog(CStr(oMapValue.Int32Value))
												'''''''''''''''''	.Item(DBFldName.BlockNo) = oMapValue.Int32Value
												.Item(DBLineFldName.BlockAdd) = 0
											Case ODFldName.ParcelName
												''''''''''''''''	.Item(DBFldName.ParcelName) = oMapValue.Int32Value
											Case ODFldName.StatusId
												'''''''''''''''''''	.Item(DBFldName.StatusId) = oMapValue.Int32Value
											Case ODFldName.StatusName

												'	otest = oMapValue.StrValue
												'	sTestA = otest.GetType().ToString()
												'sTestb= AscW sTestA.
												'	DMAcadExt.AcadDocument.WriteMessage(otest.ToString())
												'	DMAcadExt.AcadDocument.WriteMessage(sTestA)
												'.Item(DBFldName.StatusName) = 
											Case ODFldName.LegalArea
												''''''''''''''''	.Item(DBFldName.LegalArea) = oMapValue.DoubleValue
										End Select

									Catch oMapEx As Autodesk.Gis.Map.MapException
										DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "zzInParcel")

									End Try

								Next
							End If
							If IsDBNull(.Item(DBLineFldName.BlockNo)) Then
								.Item(DBLineFldName.BlockNo) = 0
							End If
							If IsDBNull(.Item(DBLineFldName.BlockAdd)) Then
								.Item(DBLineFldName.BlockAdd) = 0
							End If
							If IsDBNull(.Item(DBLineFldName.ParcelName)) Then
								.Item(DBLineFldName.ParcelName) = "a" & CStr(miParcelCounter)
							End If
						End With
						Try
							moLineDataTable.Rows.Add(oNewRow)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "7612")
						End Try
					End If
				End If
			End If
		End Sub
		Public Sub InputParcelCentroids()
			Const iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode = Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead
			Dim sSelectComText As String = "SELECT * FROM AshqParcelCents"

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(iMode)
			Dim dlInParcelProc As DMAcadExt.AcadTransaction.Procedure = New DMAcadExt.AcadTransaction.Procedure(AddressOf zzInParcelCentroid)
			zzInitCentroidAttribIndex(msCentroidBlockName, miaBlockAttribIndex)


			System.Windows.Forms.MessageBox.Show("", "Ver 002")


         moDbDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataAdapter(sSelectComText, CommandType.Text, True)
         '   moDbDataAdapter = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sSelectComText, CommandType.Text, True)

			moCentDataTable = New DataTable()
			moDbDataAdapter.FillSchema(moCentDataTable, SchemaType.Source)

			DMAcadExt.AcadTransaction.EnumModelSpaceObjects(dlInParcelProc, iMode)
         '	System.Windows.Forms.MessageBox.Show(CStr(moCentDataTable.Rows.Count), "01_911")
			Try
				moDbDataAdapter.Update(moCentDataTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Err #7183")
			End Try


			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()

		End Sub
		Private Sub zzInParcelCentroid(ByVal oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject)
			Dim sRXClassName As String = oDBObject.GetRXClass().Name


			If sRXClassName = DMAcadExt.AcadConst.AcadBlockRefName Then
				Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference = DirectCast(oDBObject, Autodesk.AutoCAD.DatabaseServices.BlockReference)
				If oBlockRef.Name = msCentroidBlockName Then

					Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
					Dim oODRecord As Record = Nothing
					Dim oNewRow As DataRow = moCentDataTable.NewRow()
					Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaBlockAttribIndex)
					miParcelCounter += 1
					If miParcelCounter > 0 Then


						DMAcadExt.AcadDocument.WriteMessage("Parcel #" & CStr(miParcelCounter) & ":" & oDBObject.Handle.ToString())

						With oNewRow
							Try

								tPoint = oBlockRef.Position
								.Item(DBCentFldName.X) = tPoint.X
								.Item(DBCentFldName.Y) = tPoint.Y
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message, "7331w")
							End Try


							Try
								.Item(DBCentFldName.ParcelName) = saAttribText(0)
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 1")
								DMAcadExt.AcadDocument.WriteMessage("12 Parcel #" & CStr(miParcelCounter))
							End Try
							Try
								.Item(DBCentFldName.BlockNo) = saAttribText(1)
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 2")
								DMAcadExt.AcadDocument.WriteMessage("13 Parcel #" & CStr(miParcelCounter))
							End Try
							Try
								.Item(DBCentFldName.LegalArea) = Convert.ToDouble(saAttribText(2))
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 2")
								DMAcadExt.AcadDocument.WriteMessage("13 Parcel #" & CStr(miParcelCounter))
							End Try
							Try
								.Item(DBCentFldName.CentObjID) = oBlockRef.ObjectId '''''''''''''''.OldIdPtr.ToInt64()
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 3")
							End Try
							Try
								.Item(DBCentFldName.CentHandle) = oBlockRef.Handle.Value
							Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
								System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message, "Err 4")
							End Try


							If IsDBNull(.Item(DBCentFldName.BlockNo)) Then
								.Item(DBLineFldName.BlockNo) = 0
							End If
							 
						End With
						Try
							moCentDataTable.Rows.Add(oNewRow)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "Err #7617")
						End Try
					End If
				End If
			End If
		End Sub


		Private Shared Sub zzInitCentroidAttribIndex(ByVal sCentroidBlockName As String, ByRef iaBlockAttribIndex() As Integer)
			Const sParcelAttribTag As String = "PARCEL"
			Const sBlockAttribTag As String = "BLOCK"
			Const sLegalAreaAttribTag As String = "AREA_LEGAL"

			Try
            Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(sCentroidBlockName, True)
				'		AcadDocument.WriteMessageLog("^^^ " & DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", False))

				If saBlockAttribTag IsNot Nothing Then
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case Strings.UCase(saBlockAttribTag(iAttribIndex))
							Case sParcelAttribTag
								iaBlockAttribIndex(0) = iAttribIndex
							Case sBlockAttribTag
								iaBlockAttribIndex(1) = iAttribIndex
							Case sLegalAreaAttribTag
								iaBlockAttribIndex(2) = iAttribIndex
						End Select
					Next
				End If
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & oEx.StackTrace, "TplnLot - zzInitCentroidAttribIndex")
			End Try
		End Sub

		Private Class ODFldName
			Public Const BlockNo As String = "GUSH_NO"
			Public Const ParcelName As String = "PARCEL"
			Public Const LegalArea As String = "LEGAL_AREA"
			Public Const StatusId As String = "STATUS_ID"
			Public Const StatusName As String = "STATUS"
			Public Const Perimeter As String = "Shape_Leng"
			Public Const Area As String = "Shape_Area"
		End Class
		Private Class DBLineFldName
			Public Const ParcelID As String = "ParcelID"
			Public Const BlockNo As String = "Block"
			Public Const BlockAdd As String = "BlockAdd"
			Public Const ParcelName As String = "ParcelName"
			Public Const LegalArea As String = "LegalArea"
			Public Const Area As String = "Area"
			Public Const StatusId As String = "StatusID"
			Public Const StatusName As String = "StatusName"
			Public Const Perimeter As String = "Perimeter"
			Public Const Xmin As String = "Xmin"
			Public Const Ymin As String = "Ymin"
			Public Const Xmax As String = "Xmax"
			Public Const Ymax As String = "Ymax"
			Public Const LineObjID As String = "LineObjID"
			Public Const LineHandle As String = "LineHandle"

		End Class
		Private Class DBCentFldName
			'Public Const ParcelID As String = "ParcelID"
			Public Const BlockNo As String = "Block"
			Public Const ParcelName As String = "ParcelName"
			Public Const LegalArea As String = "LegalArea"
			Public Const X As String = "X"
			Public Const Y As String = "Y"
			Public Const CentObjID As String = "CentObjID"
			Public Const CentHandle As String = "CentHandle"

		End Class
	End Class

End Namespace

