Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map
Public Enum TopoTableType
	Centroid
	Link
	Node
	ID
	Description
End Enum
Public Class ODTable
	'	Private Shared moODTables As Tables = Nothing
	Private moODTable As ObjectData.Table
	Private msTableName As String
	Private moFieldDefinitions As FieldDefinitions
	Private msFieldDefNames() As String
	Public Shared Function GetTableName(ByVal sTopologyName As String, ByVal iTopoTableType As TopoTableType) As String
		Dim sPrefix As String = Nothing
		Select Case iTopoTableType
			Case TopoTableType.Centroid
				sPrefix = "TPMCNTR_"
			Case TopoTableType.Link
				sPrefix = "TPMLINK_"
			Case TopoTableType.Node
				sPrefix = "TPMNODE_"
			Case TopoTableType.ID
				sPrefix = "TPMID_"
			Case TopoTableType.Description
				sPrefix = "TPMDESC_"
		End Select
		If sPrefix IsNot Nothing AndAlso sTopologyName IsNot Nothing Then
			Return sPrefix & sTopologyName
		Else
			Return Nothing
		End If
	End Function
	Public Shared Sub EraseAllTables()
		Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables

		For Each sTableName As String In oODTables.GetTableNames
			If oODTables.IsTableDefined(sTableName) Then
				Try
					oODTables.RemoveTable(sTableName)
				Catch oMapEx As MapException
					AcadDocument.WriteMessage("Table: " & sTableName & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace)
				End Try

			End If
		Next


	End Sub
	Public Shared Sub EraseTable(sTableName As String)
		Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables


		If oODTables.IsTableDefined(sTableName) Then
			Try
				oODTables.RemoveTable(sTableName)
			Catch oMapEx As MapException
				AcadDocument.WriteMessage("Table: " & sTableName & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace)
			End Try

		End If
	End Sub


	Public Function GetODRecord(ByVal oDBObject As DBObject) As ObjectData.Record
		Dim colODRecords As Records
		Dim sHandle As String = oDBObject.Handle.ToString()
		'	System.Windows.Forms.MessageBox.Show("START of GetODRecord", "01_151! ")
		Dim sTest As String = "a,"
		Try

			colODRecords = moODTable.GetObjectTableRecords(0, oDBObject, Constants.OpenMode.OpenForRead, True) 'Convert.ToUInt32(0)
		Catch oEx As Exception
			colODRecords = Nothing
			AcadDocument.WriteDebugMessageN("#0285 ", oEx.Message, moODTable, oDBObject, sHandle)
			''''''''''System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrODTable - GetODRecord_1!")
			Return Nothing
		End Try

		If colODRecords IsNot Nothing AndAlso colODRecords.Count > 0 Then
			If colODRecords.OpenMode <> Constants.OpenMode.AdeClosed Then

				Try
					sTest &= "b,"
					For Each oODRecord As ObjectData.Record In colODRecords
						sTest &= "c,"
						If oODRecord.TableName = msTableName Then
							sTest &= "d,"
							'	System.Windows.Forms.MessageBox.Show("END of GetODRecord", "01_152! ")
							Return oODRecord
						End If
					Next
				Catch oMapEx As MapException
					AcadDocument.WriteMessage("!!! " & sTest & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace)
					System.Windows.Forms.MessageBox.Show(sTest & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace, "IstrODTable - GetODRecord_2")
				End Try
			Else
				Dim sMsg As String
				If colODRecords Is Nothing Then
					sMsg = "colODRecords Is Nothing"
				ElseIf colODRecords.Count = 0 Then
					sMsg = "colODRecords.Count=0"
				Else
					sMsg = colODRecords.OpenMode.ToString()
				End If
				AcadDocument.WriteMessage("Wrong iterator mode.: " & sMsg)
			End If
		End If

		'	IstrDrawing.WriteMessage("ODRecord Is Nothing !!!!")
		Return Nothing
	End Function
	Public ReadOnly Property TableName As String
		Get
			Return msTableName
		End Get
	End Property
	Public Function GetODRecord(ByVal tAcObjID As ObjectId) As ObjectData.Record
		Dim colODRecords As Records
		Try
			colODRecords = moODTable.GetObjectTableRecords(0, tAcObjID, Constants.OpenMode.OpenForRead, False) 'Convert.ToUInt32(0)
		Catch oEx As Exception
			AcadDocument.WriteMessage("##! " & oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tAcObjID.ToString)
			''''''''''System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrODTable - GetODRecord_1!")
			Return Nothing
		End Try

		If colODRecords IsNot Nothing AndAlso colODRecords.Count > 0 AndAlso colODRecords.OpenMode <> Constants.OpenMode.AdeClosed Then
			Try
				For Each oODRecord As ObjectData.Record In colODRecords
					If oODRecord.TableName = msTableName Then
						Return oODRecord
					End If
				Next
			Catch oMapEx As MapException
				AcadDocument.WriteMessage("!!! " & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace)
				System.Windows.Forms.MessageBox.Show(CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace, "IstrODTable - GetODRecord_2")
			End Try

		Else
			Dim sMsg As String
			If colODRecords Is Nothing Then
				sMsg = "colODRecords Is Nothing"
			ElseIf colODRecords.Count = 0 Then
				sMsg = "colODRecords.Count=0"
			Else
				sMsg = colODRecords.OpenMode.ToString()
			End If
			AcadDocument.WriteMessage("Wrong iterator mode.: " & sMsg)
		End If
		'	IstrDrawing.WriteMessage("ODRecord Is Nothing !!!!")
		Return Nothing
	End Function
	Public Sub RemoveODRecord(ByVal oDBObject As DBObject)
		Dim colODRecords As Records = Nothing
		Try
			colODRecords = moODTable.GetObjectTableRecords(0, oDBObject, Constants.OpenMode.OpenForWrite, False) 'Convert.ToUInt32(0)
		Catch oEx As Exception
			AcadDocument.WriteMessage("#0274 " & oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oDBObject.ToString)
			''''''''''System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrODTable - GetODRecord_1!")

		End Try
		'DMCommon.Debug.MsgBox("09_917b", colODRecords.Count)
		zzRemoveODRecord(colODRecords)

	End Sub
	Public Sub RemoveODRecord(ByVal tAcObjID As ObjectId)
		Dim colODRecords As Records = Nothing
		Try
			colODRecords = moODTable.GetObjectTableRecords(0, tAcObjID, Constants.OpenMode.OpenForWrite, False) 'Convert.ToUInt32(0)
		Catch oEx As Exception
			AcadDocument.WriteMessage("##! " & oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tAcObjID.ToString)
		End Try

		zzRemoveODRecord(colODRecords)

	End Sub

	Private Sub zzRemoveODRecord(colODRecords As Records)
		Dim sTest As String = ""
		If colODRecords IsNot Nothing AndAlso colODRecords.Count > 0 Then
			'DMCommon.Debug.MsgBox("09_917e", colODRecords.OpenMode, colODRecords.Count)
			If colODRecords.OpenMode <> Constants.OpenMode.AdeClosed Then
				Try
					Dim oODRecord As ObjectData.Record
					sTest = "a"
					If True Then
						For iIndex As Integer = 0 To colODRecords.Count - 1
							oODRecord = colODRecords.Item(iIndex)
							If oODRecord.TableName = msTableName Then
								sTest = "c"
								colODRecords.RemoveRecord()
								sTest = "d"
								Exit For
							End If
						Next
					End If
					If False Then
						For Each oODRecord1 As ObjectData.Record In colODRecords
							sTest = "b"
							If oODRecord1.TableName = msTableName Then
								sTest = "c"
								colODRecords.RemoveRecord()
								sTest = "d"
								Exit For
							End If
						Next
					End If

				Catch oMapEx As MapException
					AcadDocument.WriteMessage("!!! " & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & sTest)
					DMCommon.Debug.UserMsg("IstrODTable - RemoveODRecord_3", CType(oMapEx.ErrorCode, Constants.ErrorCode), oMapEx.Message, oMapEx.StackTrace, colODRecords.OpenMode, msTableName, colODRecords.Count)
				End Try

			Else
				Dim sMsg As String
				If colODRecords Is Nothing Then
					sMsg = "colODRecords Is Nothing"
				ElseIf colODRecords.Count = 0 Then
					sMsg = "colODRecords.Count=0"
				Else
					sMsg = colODRecords.OpenMode.ToString()
				End If
				AcadDocument.WriteMessage("Wrong iterator mode.: " & sMsg)
			End If
		End If

	End Sub

	Public Shared Function GetDBValue(ByVal oMapValue As Autodesk.Gis.Map.Utilities.MapValue) As System.Object
		Select Case oMapValue.Type()
			Case Constants.DataType.Character
				Return oMapValue.StrValue()
			Case Constants.DataType.Integer
				Return oMapValue.Int32Value
			Case Constants.DataType.Real
				Return oMapValue.DoubleValue
			Case Else
				Return Nothing
		End Select
	End Function

	

	Public Sub New(ByVal sTableName As String)
		msTableName = sTableName

		zzInit()
	End Sub
	Public Sub New(ByVal sTopologyName As String, ByVal iTopoTableType As TopoTableType)
		msTableName = GetTableName(sTopologyName, iTopoTableType)
		zzInit()
	End Sub
	Public ReadOnly Property Exists As Boolean
		Get
			Return (moODTable IsNot Nothing)
		End Get
	End Property
	Public Function GetIntValue(iFieldNo As Integer, tAcObjID As ObjectId) As Integer
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		Dim oRecords As Records

		If moODTable IsNot Nothing Then
			oRecords = moODTable.GetObjectTableRecords(0, tAcObjID, Constants.OpenMode.OpenForWrite, True)
			For Each oRecord As Record In oRecords
				oMapValue = oRecord.Item(iFieldNo)
				Return oMapValue.Int32Value
			Next

		Else
			DMCommon.Debug.MsgBox("09_908v")
		End If

		Return -1
	End Function
	Public Function GetIntValue(iFieldNo As Integer, oAcadObject As DBObject) As Integer
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		Dim oRecords As Records

		If moODTable IsNot Nothing Then
			oRecords = moODTable.GetObjectTableRecords(0, oAcadObject, Constants.OpenMode.OpenForWrite, True)
			For Each oRecord As Record In oRecords
				oMapValue = oRecord.Item(iFieldNo)
				Return oMapValue.Int32Value
			Next

		Else
			DMCommon.Debug.MsgBox("09_908v")
		End If

		Return -1
	End Function
	Public Function GetStrValue(iFieldNo As Integer, tAcObjID As ObjectId) As String
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		Dim oRecords As Records

		If moODTable IsNot Nothing Then
			oRecords = moODTable.GetObjectTableRecords(0, tAcObjID, Constants.OpenMode.OpenForWrite, True)
			For Each oRecord As Record In oRecords
				oMapValue = oRecord.Item(iFieldNo)
				Return oMapValue.StrValue
			Next

		Else
			DMCommon.Debug.MsgBox("09_908y")
		End If

		Return Nothing
	End Function
	Public Function GetStrValue(iFieldNo As Integer, oAcadObject As DBObject) As String
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		Dim oRecords As Records

		If moODTable IsNot Nothing Then
			oRecords = moODTable.GetObjectTableRecords(0, oAcadObject, Constants.OpenMode.OpenForWrite, True)
			If oRecords IsNot Nothing Then
				For Each oRecord As Record In oRecords
					oMapValue = oRecord.Item(iFieldNo)
					Return oMapValue.StrValue
				Next
			End If


		Else
			DMCommon.Debug.MsgBox("09_908z")
		End If

		Return Nothing
	End Function
	Public Function GetNewRecord() As Autodesk.Gis.Map.ObjectData.Record
		Dim oNewODRecord As Record
		'Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		oNewODRecord = Autodesk.Gis.Map.ObjectData.Record.Create
		If moODTable IsNot Nothing Then
			moODTable.InitRecord(oNewODRecord)
			'DMCommon.Debug.MsgBox("13_044c", oNewODRecord.Count, iFieldNo)

			'	moODTable.AddRecord(oNewODRecord, tAcObjID)
			Return oNewODRecord
		Else
			DMCommon.Debug.MsgBox("09_908aa")
			Return Nothing
		End If


	End Function
	Public Sub AddRecord(oODRecord As Record, ByVal oDBObject As DBObject)

		If moODTable IsNot Nothing Then
			moODTable.AddRecord(oODRecord, oDBObject)

		Else
			DMCommon.Debug.MsgBox("09_908at")
		End If

	End Sub
	Public Sub AddRecord(oODRecord As Record, tAcObjID As ObjectId)

		If moODTable IsNot Nothing Then
			Try
				moODTable.AddRecord(oODRecord, tAcObjID)
			Catch oMapEx As MapException
				AcadDocument.WriteMessage("Err #491; Table: " & msTableName & vbCrLf & CType(oMapEx.ErrorCode, Constants.ErrorCode).ToString() & vbCrLf & oMapEx.Message & vbCrLf & oMapEx.StackTrace & vbCrLf & tAcObjID.ToString())
			End Try


		Else
			DMCommon.Debug.MsgBox("09_908as")
		End If

	End Sub
	Public Sub SetIntValue(iFieldNo As Integer, iValue As Integer, tAcObjID As ObjectId)
		Dim oNewODRecord As Record
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		oNewODRecord = Autodesk.Gis.Map.ObjectData.Record.Create
		If moODTable IsNot Nothing Then
			moODTable.InitRecord(oNewODRecord)
			'DMCommon.Debug.MsgBox("13_044c", oNewODRecord.Count, iFieldNo)
			If iFieldNo < oNewODRecord.Count Then
				oMapValue = oNewODRecord.Item(iFieldNo)
				'	DMCommon.Debug.MsgBox("13_044d", oMapValue)
				If oMapValue IsNot Nothing Then
					oMapValue.Assign(iValue)
				End If

				moODTable.AddRecord(oNewODRecord, tAcObjID)
			End If


		Else
			DMCommon.Debug.MsgBox("09_908aa")
		End If


	End Sub

	Public Sub SetStrValue(iFieldNo As Integer, sValue As String, tAcObjID As ObjectId)
		Dim oNewODRecord As Record
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		oNewODRecord = Autodesk.Gis.Map.ObjectData.Record.Create
		If moODTable IsNot Nothing Then
			moODTable.InitRecord(oNewODRecord)
			'DMCommon.Debug.MsgBox("13_044c", oNewODRecord.Count, iFieldNo)
			If iFieldNo < oNewODRecord.Count Then
				oMapValue = oNewODRecord.Item(iFieldNo)
				'DMCommon.Debug.MsgBox("13_044d", oMapValue)
				If oMapValue IsNot Nothing Then
					oMapValue.Assign(sValue)
				End If

				moODTable.AddRecord(oNewODRecord, tAcObjID)
			End If


		Else
			DMCommon.Debug.MsgBox("09_908aa")
		End If


	End Sub

	Public Sub CreateTable(oaFieldDef() As ObjectData.FieldDefinition)
      Dim oFieldDefs As ObjectData.FieldDefinitions = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.MapUtility.NewODFieldDefinitions()
      For iIndex As Integer = 0 To oaFieldDef.GetUpperBound(0)
         oFieldDefs.AddColumn(oaFieldDef(iIndex), iIndex)
      Next
		Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables

		oODTables.Add(msTableName, oFieldDefs, "", True)
		moODTable = oODTables.Item(msTableName)

	End Sub
	Private Sub zzInit()
		Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
		If oODTables.IsTableDefined(msTableName) Then
			moODTable = oODTables.Item(msTableName)
		End If
	End Sub
	Public Sub Terminate()
		moODTable = Nothing
		'	moODTables = Nothing
   End Sub
   Private Sub zz()
      Dim fieldDefs As ObjectData.FieldDefinitions
      fieldDefs = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.MapUtility.NewODFieldDefinitions()
      Dim def1 As ObjectData.FieldDefinition
      def1 = fieldDefs.Add("FIRST_FIELD", "Owner name", Autodesk.Gis.Map.Constants.DataType.Character, 0)
      Dim def2 As ObjectData.FieldDefinition = ObjectData.FieldDefinition.Create("FIELD", "", Constants.DataType.Character)

      Dim def3 As ObjectData.FieldDefinition
      def3 = fieldDefs.Add("SECOND_FIELD", "Assessment year", Autodesk.Gis.Map.Constants.DataType.Integer, 1)
      '  def3.DefaultValue(0)

      'Get a reference to the ODTables property for the drawing, and add the field defintions to create a new table.

      Dim tables As ObjectData.Tables
      tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
      tables.Add("NewTable", fieldDefs, "Description", True)

   End Sub
End Class
