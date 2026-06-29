Imports Autodesk.AutoCAD.DatabaseServices
Public Class Ud_ScriptODTable
	Inherits DMAcadExt.ODTable
	Public Const ODTableName As String = "Ud_Script"
	Public Sub New()
		MyBase.New(ODTableName)
	End Sub
	Public Function GetData(tAcObjID As ObjectId) As ScriptData
		Dim oRec As Autodesk.Gis.Map.ObjectData.Record

		oRec = MyBase.GetODRecord(tAcObjID)
		Return zzGetData(oRec)
	End Function
	Public Function GetData(ByVal oDBObject As DBObject) As ScriptData
		Dim oRec As Autodesk.Gis.Map.ObjectData.Record

		oRec = MyBase.GetODRecord(oDBObject)
		Return zzGetData(oRec)
	End Function
	Public Sub SetData(tAcObjID As ObjectId, tData As ScriptData, Optional bRemove As Boolean = True)
		Dim oNewODRecord As Autodesk.Gis.Map.ObjectData.Record
		If bRemove Then
			MyBase.RemoveODRecord(tAcObjID)
		End If


		oNewODRecord = MyBase.GetNewRecord()

		zzSetData(oNewODRecord, tData)

		MyBase.AddRecord(oNewODRecord, tAcObjID)

	End Sub
	Public Sub SetData(ByVal oDBObject As DBObject, tData As ScriptData)
		Dim oNewODRecord As Autodesk.Gis.Map.ObjectData.Record

		MyBase.RemoveODRecord(oDBObject)

		oNewODRecord = MyBase.GetNewRecord()



		zzSetData(oNewODRecord, tData)



		MyBase.AddRecord(oNewODRecord, oDBObject)

	End Sub
	Private Sub zzSetData(oRec As Autodesk.Gis.Map.ObjectData.Record, tData As ScriptData)

		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue


		oMapValue = oRec.Item(0)
		'	DMCommon.Debug.MsgBox("13_044d", oMapValue)
		If oMapValue IsNot Nothing Then
			oMapValue.Assign(tData.Stage)  '
		End If

		oMapValue = oRec.Item(1)
		If oMapValue IsNot Nothing AndAlso tData.SourceName IsNot Nothing Then
			oMapValue.Assign(tData.SourceName)
		End If

		oMapValue = oRec.Item(2)
		If oMapValue IsNot Nothing AndAlso tData.SourceLayer IsNot Nothing Then
			oMapValue.Assign(tData.SourceLayer)
		End If
		If oRec.Count > 3 Then

			oMapValue = oRec.Item(3)
			If oMapValue IsNot Nothing AndAlso tData.SourceArea <> 0.0 Then
				oMapValue.Assign(tData.SourceArea)
			End If

		End If



	End Sub
	Public Sub CreateScriptODTable()
		Dim oaFieldDef(2) As Autodesk.Gis.Map.ObjectData.FieldDefinition
		If Not MyBase.Exists Then
			oaFieldDef(0) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("Stage", "Stage Number", 0)
			oaFieldDef(1) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("SourceName", "Source Name", "")
			oaFieldDef(2) = Autodesk.Gis.Map.ObjectData.FieldDefinition.Create("SourceLayer", "Source Layer", "")



			MyBase.CreateTable(oaFieldDef)
		End If
	End Sub

	Private Function zzGetData(oRec As Autodesk.Gis.Map.ObjectData.Record) As ScriptData
		Dim tScriptData As ScriptData = New ScriptData()
		Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
		If oRec IsNot Nothing Then
			tScriptData.RecordExists = True
			oMapValue = oRec.Item(0)
			If oMapValue IsNot Nothing Then
				tScriptData.Stage = oMapValue.Int32Value
			End If
			oMapValue = oRec.Item(1)
			If oMapValue IsNot Nothing Then
				tScriptData.SourceName = oMapValue.StrValue
			End If
			oMapValue = oRec.Item(2)
			If oMapValue IsNot Nothing Then
				tScriptData.SourceLayer = oMapValue.StrValue
			End If

			If oRec.Count > 3 Then
				oMapValue = oRec.Item(3)
				If oMapValue IsNot Nothing Then
					tScriptData.SourceArea = oMapValue.DoubleValue
				End If
			End If

		End If
		Return tScriptData
	End Function
	Public Structure ScriptData
		Const NewObject As String = "New"
		Dim Stage As Integer
		Dim SourceName As String
		Dim SourceLayer As String
		Dim SourceArea As Double

		Dim RecordExists As Boolean
		Dim Exists As Boolean
		Public Sub New(iStage As Integer, sSourceName As String, sSourceLayer As String, sSourceArea As Double)
			Stage = iStage
			SourceName = sSourceName
			SourceLayer = sSourceLayer
			SourceArea = sSourceArea
			Exists = True
		End Sub
		Public Function SourceLayerExists() As Boolean
			Return Not String.IsNullOrEmpty(SourceLayer)
		End Function
		Public Function SourceNameIsEmpty() As Boolean
			Return SourceName IsNot Nothing AndAlso SourceName.Length = 0
		End Function
		Public Function SourceNameIsNEW() As Boolean
			Return SourceName IsNot Nothing AndAlso SourceName = NewObject

		End Function
		Public Function SourceLayerIsEmpty() As Boolean
			Return SourceLayer IsNot Nothing AndAlso SourceLayer.Length = 0

		End Function

	End Structure
End Class
