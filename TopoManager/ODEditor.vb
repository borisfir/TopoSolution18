Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map.Utilities
Imports Autodesk.Gis.Map.Project
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Public Class ODEditor
	Private Shared moTables As Tables = Nothing

	Public Sub New()

	End Sub

	Public Sub GetObjectData(ByVal id As ObjectId)
		Dim eErrCode As Constants.ErrorCode = Constants.ErrorCode.OK
		zzInitTables()
		Try
			Dim objId As ObjectId = id

			'Get and Initialize Records
			Dim colRecords As ObjectData.Records = moTables.GetObjectRecords(UInt32.Parse("0"), objId, Constants.OpenMode.OpenForRead, False)

			'Check open mode
			If colRecords Is Nothing OrElse colRecords.OpenMode <> Constants.OpenMode.OpenForRead Then
				Common.GetEditor.WriteMessage(vbCrLf & "Wrong iterator mode." & vbCrLf)
			Else

				'Iterate through all records


				Static Dim times As Integer = 0

				'Enumerate record in Records, print out the infomation of Record
				For Each oRecord As ObjectData.Record In colRecords
					Common.GetEditor.WriteMessage(vbCrLf + "Record " + times.ToString() + " : " & vbCrLf)
					times = times + 1

					'Get the table
					Dim table As ObjectData.Table = Nothing
					table = moTables(oRecord.TableName)

					Dim valInt As Integer = 0
					Dim valDouble As Double = 0.0
					Dim str As String = Nothing

					'Get record info
					Dim i As Integer
					For i = 0 To oRecord.Count - 1
						Dim tblDef As FieldDefinitions = table.FieldDefinitions
						Dim column As FieldDefinition = Nothing
						column = tblDef(i)
						Dim colName As String = column.Name
						Dim val As MapValue = oRecord.Item(i)

						Dim msg As String = Nothing

						Select Case val.Type
							Case Constants.DataType.Integer
								valInt = val.Int32Value
								msg = oRecord.TableName & ": " & valInt.ToString() & "; " & vbCrLf
								Common.GetEditor.WriteMessage(msg)
							Case Constants.DataType.Real
								valDouble = val.DoubleValue
								msg = oRecord.TableName & ": " & valDouble.ToString() & "; " & vbCrLf
								Common.GetEditor.WriteMessage(msg)
							Case Constants.DataType.Character
								str = val.StrValue
								msg = oRecord.TableName & ": " & str + "; " & vbCrLf
								Common.GetEditor.WriteMessage(msg)
							Case Constants.DataType.Point
								Dim pt As Point3d = val.Point
								Dim x As Double = pt.X
								Dim y As Double = pt.Y
								Dim z As Double = pt.Z
								msg = oRecord.TableName & ": " & "Point(" & x.ToString() & ", " & y.ToString() & ", " & z.ToString() & "); " & vbCrLf
								Common.GetEditor.WriteMessage(msg)
							Case Else
								Common.GetEditor.WriteMessage(vbCrLf & "Wrong data type!" & vbCrLf)
						End Select
					Next i
				Next oRecord
			End If
		Catch err As MapException
			eErrCode = CType(err.ErrorCode, Constants.ErrorCode)
			Common.GetEditor.WriteMessage(vbCrLf & "MapException..." & vbCrLf)
		End Try
	End Sub
	Public Sub GetOverlayResOD(ByVal id As ObjectId, ByVal sOverlayTableName As String _
, ByVal sSourceTopoName As String, ByVal sOverlayTopoName As String)
      Dim eErrCode As Constants.ErrorCode = Constants.ErrorCode.OK
      zzInitTables()
      Try
         Dim objId As ObjectId = id

         'Get and Initialize Records
			Dim recordCol As ObjectData.Records = moTables.GetObjectRecords(UInt32.Parse("0"), objId, Constants.OpenMode.OpenForRead, False)

         'Check open mode
         If recordCol.OpenMode <> Constants.OpenMode.OpenForRead Then
				Common.GetEditor.WriteMessage(vbCrLf & "Wrong iterator mode." & vbCrLf)
         End If

         'Iterate through all records
         Dim record As ObjectData.Record = Nothing
         Dim oODTable As ObjectData.Table = Nothing
         Static Dim times As Integer = 0

         'Enumerate record in Records, print out the infomation of Record
         For Each record In recordCol
				Common.GetEditor.WriteMessage(vbCrLf + "Record " + times.ToString() + " : " & vbCrLf)
            times = times + 1

            'Get the table

            oODTable = moTables(record.TableName)
            If oODTable.Name = sOverlayTableName Then
               Dim valInt As Integer = 0
               Dim valDouble As Double = 0.0
               Dim str As String = Nothing

               Dim tblDef As FieldDefinitions = oODTable.FieldDefinitions
               Dim column As FieldDefinition = Nothing

               Dim colName As String
               Dim val As MapValue

               Dim msg As String = Nothing

               Dim i As Integer
               For i = 0 To record.Count - 1
                  column = tblDef(i)
                  colName = column.Name

                  val = record(i)
                  Select Case val.Type
                     Case Constants.DataType.Integer

                        valInt = val.Int32Value
								msg = valInt.ToString() + "; " & vbCrLf
                        Common.GetEditor.WriteMessage(msg)
                     Case Constants.DataType.Real
                        valDouble = val.DoubleValue
								msg = valDouble.ToString() + "; " & vbCrLf
                        Common.GetEditor.WriteMessage(msg)
                     Case Constants.DataType.Character
                        str = val.StrValue
								msg = str + "; " & vbCrLf
                        Common.GetEditor.WriteMessage(msg)
                     Case Constants.DataType.Point
                        Dim pt As Autodesk.AutoCAD.Geometry.Point3d = val.Point
                        Dim x As Double = pt.X
                        Dim y As Double = pt.Y
                        Dim z As Double = pt.Z
								msg = "Point(" & x.ToString() & ", " & y.ToString() & ", " & z.ToString() & "); " & vbCrLf
                        Common.GetEditor.WriteMessage(msg)
                     Case Else
								Common.GetEditor.WriteMessage(vbCrLf & "Wrong data type!" & vbCrLf)
                  End Select
               Next i
            End If
         Next record
      Catch err As MapException
         eErrCode = CType(err.ErrorCode, Constants.ErrorCode)
			Common.GetEditor.WriteMessage(vbCrLf & "MapException..." & vbCrLf)
      End Try
   End Sub
	Public Shared Function GetODRecord(ByVal tAcObjID As ObjectId, ByVal sODTableName As String) As ObjectData.Record
		'	moTables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
		zzInitTables()
		Dim colODRecords As ObjectData.Records = Nothing
		Dim oODRecord As ObjectData.Record
		Dim sODRecord_TableName As String = Nothing
		Try
			colODRecords = moTables.GetObjectRecords(Convert.ToUInt32(0), tAcObjID, Constants.OpenMode.OpenForRead, True)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "ODEditor - GetODRecord_1")
		End Try


		If colODRecords IsNot Nothing AndAlso colODRecords.OpenMode = Constants.OpenMode.OpenForRead AndAlso colODRecords.Count > 0 Then
			For iIndex As Integer = 0 To colODRecords.Count - 1
				Try
					oODRecord = colODRecords.Item(iIndex)

					'For Each oODRecord As ObjectData.Record In colODRecords
					If oODRecord.TableName = sODTableName Then
						Return oODRecord

					End If
					sODRecord_TableName = oODRecord.TableName
					'	Next
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "ODEditor - GetODRecord_2")
				End Try

			Next

			'	DMCommon.Debug.MsgBox("oODRecord is wrong", colODRecords.Count, sODRecord_TableName, sODTableName)
		Else
			DMAcadExt.AcadDocument.WriteMessage("Wrong iterator mode.")
		End If
		Return Nothing
	End Function
	Public Shared Function GetODRecord(ByVal oDBObject As DBObject, ByVal sODTableName As String) As ObjectData.Record
		zzInitTables()
		Dim colODRecords As ObjectData.Records
		Try
			colODRecords = moTables.GetObjectRecords(Convert.ToUInt32(0), oDBObject, Constants.OpenMode.OpenForRead, True)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "GetODRecord")
			Return Nothing
		End Try


		If colODRecords IsNot Nothing AndAlso colODRecords.OpenMode = Constants.OpenMode.OpenForRead Then
			For Each oODRecord As ObjectData.Record In colODRecords
				If oODRecord.TableName = sODTableName Then
					Return oODRecord
				End If
			Next
		Else
			DMAcadExt.AcadDocument.WriteMessage("Wrong iterator mode.")
		End If
		Return Nothing
	End Function
   Public Shared Sub GetTopoNames(ByRef saAllTopoNames As String(), ByRef saUnionTopoNames As String())
      zzInitTables()
      Dim oTableNames As Specialized.StringCollection = moTables.GetTableNames()
      Dim iTablesUB As Integer = oTableNames.Count - 1
      Dim sTableName As String
      Dim saTableName() As String
      Dim iTopoNameIndex As Integer = -1
      Dim iUnionTopoIndex As Integer = -1

      '  Dim saTopoNames() As String = Nothing

      If iTablesUB >= 0 Then
         ReDim saAllTopoNames(iTablesUB)
         ReDim saUnionTopoNames(iTablesUB)

         For Each sTableName In oTableNames
            ' sTableName = Replace(sTableName, "_", " ", , 1)
            saTableName = Common.NameSplit(sTableName)
            If saTableName.GetUpperBound(0) = 1 Then
               If StrComp(saTableName(0), "TPMCNTR", CompareMethod.Text) = 0 Then
                  iTopoNameIndex += 1
                  saAllTopoNames(iTopoNameIndex) = saTableName(1)
               ElseIf StrComp(saTableName(0), Common.UnionTopoPrefix, CompareMethod.Text) = 0 Then
                  iUnionTopoIndex += 1
                  saUnionTopoNames(iUnionTopoIndex) = sTableName
               End If
            End If
         Next
         If iTopoNameIndex <> -1 Then
            ReDim Preserve saAllTopoNames(iTopoNameIndex)
         End If
         If iUnionTopoIndex <> -1 Then
            ReDim Preserve saUnionTopoNames(iUnionTopoIndex)
         End If
      End If

   End Sub

   Public Shared Function GetODTable(ByVal sODTableName As String) As ObjectData.Table
      Try
         zzInitTables()
         If moTables IsNot Nothing Then
            If moTables.IsTableDefined(sODTableName) Then
               Return moTables(sODTableName)
            Else
               Dim sMsg As String = "ODTable: " & sODTableName & " was not found"
               System.Windows.Forms.MessageBox.Show(sMsg, "ODEditor - GetODTable_1")
               Return Nothing
            End If


         Else
            Return Nothing
         End If
			' Return moTables(sODTableName)
		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "ODEditor - GetODTable_2")
         Return Nothing
      End Try

   End Function

 
   Private Shared Sub zzInitTables()
      If moTables Is Nothing Then
			moTables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
      End If

   End Sub
End Class
