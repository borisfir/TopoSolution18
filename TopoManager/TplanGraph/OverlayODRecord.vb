Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map.Utilities
Public Structure OverlayODRecord
   Dim NewID As Integer
   Dim SourceID As Integer
   Dim OverlayID As Integer
   Dim SourceAreaPcnt As Double
   Dim OverlayAreaPcnt As Double
   Dim SourcePgonArea As Double
   Dim SourcePgonName As String
   Dim OverlayPgonArea As Double
   Dim OverlayPgonName As String

End Structure
Public Class OverlayODRecordSet
   Const msNameDel As String = "_"
   Const msODOverlayTablePrefix As String = "UNION"
   Const msIDODFldName As String = "TNEW_ID"
   Const msIDSuffix As String = "ID"
   Const msPercentAreaSuffix As String = "PERCENTAREA"
   Private Const msSourcePgonAreaFldName As String = "SourcePgonArea"
   Private Const msOverlayPgonAreaFldName As String = "OverlayPgonArea"
   Private Const msSourceNameFldName As String = "SourceName"
   Private Const msOverlayNameFldName As String = "OverlayName"
   Private msTopoSourceName As String
   Private msTopoOverlayName As String
	Private msODTableName As String
	Private msODTableDescription As String

	Private moFieldDefinitions As FieldDefinitions
	Private msaFieldNames() As String
	' Private moaOverlayODRecord() As OverlayODRecord
	'  Private miOverlayODRecordIndex As Integer = -1
	Public Sub New(ByVal sTopoSourceName As String, ByVal sTopoOverlayName As String, ByVal sODTableName As String)
      Dim oODTable As Table
      msTopoSourceName = sTopoSourceName
      msTopoOverlayName = sTopoOverlayName
		msODTableName = sODTableName
		'DMCommon.Debug.MsgBox("030321_0", "B_", msODTableName)
		oODTable = ODEditor.GetODTable(msODTableName)
		'DMCommon.Debug.MsgBox("030321_1", "A_", msODTableName)
		If oODTable IsNot Nothing Then
			moFieldDefinitions = oODTable.FieldDefinitions
			If moFieldDefinitions IsNot Nothing Then
				zzSetFieldNames()
			Else
				DMCommon.Debug.MsgBox("220321_2", " moFieldDefinitions Is Nothing", msODTableName)
			End If
		Else

			DMCommon.Debug.MsgBox("220321_1", " oODTable Is Nothing", msODTableName)
		End If

	End Sub
	Private Sub zzSetFieldNames()
		Try
			Dim oFieldDef As FieldDefinition = Nothing
			Dim iUB As Integer = moFieldDefinitions.Count - 1
			ReDim msaFieldNames(iUB)
			For iIndex As Integer = 0 To iUB
				Try
					oFieldDef = moFieldDefinitions.Item(iIndex)
				Catch oEx As Exception
					oFieldDef = Nothing
				End Try
				If oFieldDef IsNot Nothing Then
					msaFieldNames(iIndex) = oFieldDef.Name
				End If

			Next
			If oFieldDef IsNot Nothing Then
				oFieldDef.Dispose()
			End If

			'DMCommon.Debug.ExcelLog.SetEnumerable(0, "!msaFieldNames", msaFieldNames)
		Catch oEx As Exception
			DMCommon.Debug.MsgBox("Except-OverlayODRecordSet - New1", oEx.Message, oEx.StackTrace)
		End Try

	End Sub
	Public Sub NewB(ByVal sTopoSourceName As String, ByVal sTopoOverlayName As String)
      msTopoSourceName = sTopoSourceName
      msTopoOverlayName = sTopoOverlayName
   End Sub
   Public Sub NewA(ByVal sODTableName As String)
      Dim saODTableName() As String = Split(sODTableName, msNameDel)
      If saODTableName.GetUpperBound(0) = 2 AndAlso saODTableName(0) = msODOverlayTablePrefix Then
         msTopoSourceName = saODTableName(1).Trim()
         msTopoOverlayName = saODTableName(2).Trim()
      End If
   End Sub
   Public Sub New(ByVal oODTable As Table)
      msODTableName = oODTable.Name
      msODTableDescription = oODTable.Description
      If msODTableDescription.StartsWith("Union: ") Then
         Dim saODTableDescription() As String = Split(msODTableDescription.Substring("Union: ".Length), ",")
         If saODTableDescription.GetUpperBound(0) = 1 Then
            msTopoSourceName = saODTableDescription(0).Trim
            msTopoOverlayName = saODTableDescription(1).Trim
         End If
         moFieldDefinitions = oODTable.FieldDefinitions
      Else
         System.Windows.Forms.MessageBox.Show(msODTableName, "OverlayODRecordSet-7900")
      End If
   End Sub
	Public Function GetOverlayODRecordExample(ByVal oId As Autodesk.AutoCAD.DatabaseServices.ObjectId) As OverlayODRecord
		Dim oFieldDef As FieldDefinition
		Dim oOverlayODRecord As OverlayODRecord
		oOverlayODRecord.SourcePgonName = String.Empty
		oOverlayODRecord.OverlayPgonName = String.Empty

		Dim oODRecord As Autodesk.Gis.Map.ObjectData.Record = ODEditor.GetODRecord(oId, msODTableName)
		If oODRecord IsNot Nothing AndAlso moFieldDefinitions IsNot Nothing Then
			Try
				Dim oMapValue As MapValue
				For iFieldIndex As Integer = 0 To oODRecord.Count - 1
					oFieldDef = moFieldDefinitions.Item(iFieldIndex)
					oMapValue = oODRecord.Item(iFieldIndex)
					'     zzTestVal(oMapValue)
					Select Case oFieldDef.Name
						Case msIDODFldName
							oOverlayODRecord.NewID = oMapValue.Int32Value
						Case msTopoSourceName & msNameDel & msIDSuffix
							oOverlayODRecord.SourceID = oMapValue.Int32Value
						Case msTopoOverlayName & msNameDel & msIDSuffix
							oOverlayODRecord.OverlayID = oMapValue.Int32Value
						Case msTopoSourceName & msNameDel & msPercentAreaSuffix
							oOverlayODRecord.SourceAreaPcnt = oMapValue.DoubleValue
						Case msTopoOverlayName & msNameDel & msPercentAreaSuffix
							oOverlayODRecord.OverlayAreaPcnt = oMapValue.DoubleValue
						Case msSourcePgonAreaFldName
							oOverlayODRecord.SourcePgonArea = oMapValue.DoubleValue
						Case msOverlayPgonAreaFldName
							oOverlayODRecord.OverlayPgonArea = oMapValue.DoubleValue
						Case msSourceNameFldName
							oOverlayODRecord.SourcePgonName = oMapValue.StrValue
						Case msOverlayNameFldName
							oOverlayODRecord.OverlayPgonName = oMapValue.StrValue
					End Select
				Next
			Catch ex As Exception
				System.Windows.Forms.MessageBox.Show(ex.Message, "7201")
			End Try
		Else
			If oODRecord Is Nothing Then
				System.Windows.Forms.MessageBox.Show("3377", "GetOverlayODRecord-oODRecord Is Nothing")
			Else
				System.Windows.Forms.MessageBox.Show("3378", "GetOverlayODRecord-oODRecord Is Nothing")
			End If
		End If
		Return oOverlayODRecord
		'    miOverlayODRecordIndex += 1
		'     ReDim Preserve moaOverlayODRecord(miOverlayODRecordIndex)
	End Function

	Public Function GetOverlayODRecordNew(ByVal tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As OverlayODRecord

		Dim oOverlayODRecord As OverlayODRecord = New OverlayODRecord()
		oOverlayODRecord.SourcePgonName = String.Empty
		oOverlayODRecord.OverlayPgonName = String.Empty
		Dim sTest As String = String.Empty

		Dim oODRecord As Autodesk.Gis.Map.ObjectData.Record = ODEditor.GetODRecord(tAcObjID, msODTableName)


		Dim sFieldName As String = "---"
		If oODRecord IsNot Nothing AndAlso (msaFieldNames IsNot Nothing) Then
			Try
				Dim oMapValue As MapValue
				For iFieldIndex As Integer = 0 To oODRecord.Count - 1
					Try
						sTest = "a"
						'	oFieldDef = moFieldDefinitions.Item(iFieldIndex)
						sTest = "b"
						sFieldName = msaFieldNames(iFieldIndex)
						sTest = "c"
					Catch oEx As Exception
						Dim bFieldDefinitionsExist As Boolean = (moFieldDefinitions IsNot Nothing)
						If bFieldDefinitionsExist Then
							'	sTest = sTest & ": Count=" & moFieldDefinitions.Count & ":"
						End If
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest & vbCrLf & CStr(iFieldIndex) & vbCrLf & CStr(bFieldDefinitionsExist), "7255")
						sFieldName = String.Empty
					End Try
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ODRecord", sFieldName)
					If Not String.IsNullOrEmpty(sFieldName) Then
						Try
							oMapValue = oODRecord.Item(iFieldIndex)
							Select Case sFieldName
								Case msIDODFldName
									oOverlayODRecord.NewID = oMapValue.Int32Value
								Case msTopoSourceName & msNameDel & msIDSuffix
									oOverlayODRecord.SourceID = oMapValue.Int32Value
								Case msTopoOverlayName & msNameDel & msIDSuffix
									oOverlayODRecord.OverlayID = oMapValue.Int32Value
									' zzTestVal(oMapValue)

								Case msTopoSourceName & msNameDel & msPercentAreaSuffix
									oOverlayODRecord.SourceAreaPcnt = oMapValue.DoubleValue
								Case msTopoOverlayName & msNameDel & msPercentAreaSuffix
									oOverlayODRecord.OverlayAreaPcnt = oMapValue.DoubleValue
								Case msSourcePgonAreaFldName
									oOverlayODRecord.SourcePgonArea = oMapValue.DoubleValue
								Case msOverlayPgonAreaFldName
									oOverlayODRecord.OverlayPgonArea = oMapValue.DoubleValue
								Case msSourceNameFldName
									oOverlayODRecord.SourcePgonName = oMapValue.StrValue
								Case msOverlayNameFldName
									oOverlayODRecord.OverlayPgonName = oMapValue.StrValue
								Case Else
									System.Windows.Forms.MessageBox.Show(":" & sFieldName & ":" & msTopoOverlayName & msNameDel & msIDSuffix & ":", "!ELSE1")
							End Select

						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & ":" & sFieldName, "7211")
						End Try
					End If
				Next
				oODRecord.Dispose()
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("250221_2", oEx.Message, oEx.StackTrace, sFieldName, tAcObjID)
			End Try
		Else
			If oODRecord Is Nothing Then
				DMCommon.Debug.MsgBox("Exc #337", tAcObjID, msODTableName, sFieldName)
			Else
				System.Windows.Forms.MessageBox.Show("3381", "GetOverlayODRecord-moFieldDefinitions Is Nothing")
			End If

		End If
		oODRecord.Dispose()
		Return oOverlayODRecord
		'    miOverlayODRecordIndex += 1
		'     ReDim Preserve moaOverlayODRecord(miOverlayODRecordIndex)
	End Function
	Public Function GetOverlayODRecord(ByVal tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As OverlayODRecord
		Dim oFieldDef As FieldDefinition
		Dim oOverlayODRecord As OverlayODRecord = New OverlayODRecord()
		oOverlayODRecord.SourcePgonName = String.Empty
		oOverlayODRecord.OverlayPgonName = String.Empty
		Dim sTest As String = String.Empty
		DMCommon.Debug.ExcelLog.SetValue(0, "!B_GetODRecord", tAcObjID)
		Dim oODRecord As Autodesk.Gis.Map.ObjectData.Record = ODEditor.GetODRecord(tAcObjID, msODTableName)

		DMCommon.Debug.ExcelLog.SetNextValue(3, "!A_GetODRecord", oODRecord)
		Dim sFieldName As String = "---"
		If oODRecord IsNot Nothing AndAlso (moFieldDefinitions IsNot Nothing) Then
			Try
				Dim oMapValue As MapValue
				For iFieldIndex As Integer = 0 To oODRecord.Count - 1
					Try
						sTest = "a"
						oFieldDef = moFieldDefinitions.Item(iFieldIndex)
						sTest = "b"
						sFieldName = oFieldDef.Name
						sTest = "c"
					Catch oEx As Exception
						Dim bFieldDefinitionsExist As Boolean = (moFieldDefinitions IsNot Nothing)
						If bFieldDefinitionsExist Then
							'	sTest = sTest & ": Count=" & moFieldDefinitions.Count & ":"
						End If
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest & vbCrLf & CStr(iFieldIndex) & vbCrLf & CStr(bFieldDefinitionsExist), "7255")
						sFieldName = String.Empty
					End Try
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!ODRecord", sFieldName)
					If sFieldName.Length <> 0 Then
						Try
							oMapValue = oODRecord.Item(iFieldIndex)
							Select Case sFieldName
								Case msIDODFldName
									oOverlayODRecord.NewID = oMapValue.Int32Value
								Case msTopoSourceName & msNameDel & msIDSuffix
									oOverlayODRecord.SourceID = oMapValue.Int32Value
								Case msTopoOverlayName & msNameDel & msIDSuffix
									oOverlayODRecord.OverlayID = oMapValue.Int32Value
									' zzTestVal(oMapValue)

								Case msTopoSourceName & msNameDel & msPercentAreaSuffix
									oOverlayODRecord.SourceAreaPcnt = oMapValue.DoubleValue
								Case msTopoOverlayName & msNameDel & msPercentAreaSuffix
									oOverlayODRecord.OverlayAreaPcnt = oMapValue.DoubleValue
								Case msSourcePgonAreaFldName
									oOverlayODRecord.SourcePgonArea = oMapValue.DoubleValue
								Case msOverlayPgonAreaFldName
									oOverlayODRecord.OverlayPgonArea = oMapValue.DoubleValue
								Case msSourceNameFldName
									oOverlayODRecord.SourcePgonName = oMapValue.StrValue
								Case msOverlayNameFldName
									oOverlayODRecord.OverlayPgonName = oMapValue.StrValue
								Case Else
									System.Windows.Forms.MessageBox.Show(":" & sFieldName & ":" & msTopoOverlayName & msNameDel & msIDSuffix & ":", "!ELSE2")
							End Select

						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & ":" & sFieldName, "7211")
						End Try
					End If
				Next

			Catch oEx As Exception
				DMCommon.Debug.MsgBox("250221_2", oEx.Message, oEx.StackTrace, sFieldName, tAcObjID)
			End Try
		Else
			If oODRecord Is Nothing Then
				System.Windows.Forms.MessageBox.Show("3379", "GetOverlayODRecord-oODRecord Is Nothing")
			Else
				System.Windows.Forms.MessageBox.Show("3381", "GetOverlayODRecord-moFieldDefinitions Is Nothing")
			End If

		End If

		Return oOverlayODRecord
		'    miOverlayODRecordIndex += 1
		'     ReDim Preserve moaOverlayODRecord(miOverlayODRecordIndex)
	End Function
	Public Sub Dispose()
		moFieldDefinitions.Dispose()
	End Sub
	Private Sub zzTestVal(ByVal oMapValue As MapValue)
      Dim valInt As Integer = 0
      Dim valDouble As Double = 0.0
      Dim str As String = Nothing
      Dim sOut As String = Nothing
      Select Case oMapValue.Type
         Case Autodesk.Gis.Map.Constants.DataType.Integer
            valInt = oMapValue.Int32Value
            sOut = valInt.ToString() + "; "

         Case Autodesk.Gis.Map.Constants.DataType.Real
            valDouble = oMapValue.DoubleValue
            sOut = valDouble.ToString() + "; "

         Case Autodesk.Gis.Map.Constants.DataType.Character
            Str = oMapValue.StrValue
            sOut = str + "; "

         Case Autodesk.Gis.Map.Constants.DataType.Point
            Dim pt As Autodesk.AutoCAD.Geometry.Point3d = oMapValue.Point
            Dim x As Double = pt.X
            Dim y As Double = pt.Y
            Dim z As Double = pt.Z
            sOut = "Point(" & x.ToString() & ", " & y.ToString() & ", " & z.ToString() & "); "

         Case Else
				System.Windows.Forms.MessageBox.Show(vbCrLf & "Wrong data type!" & vbCrLf, "6650")
      End Select
      System.Windows.Forms.MessageBox.Show(sOut, "6800")
   End Sub

   Public Property ODTableName() As String
      Get
         Return msODTableName
      End Get
      Set(ByVal sValue As String)
         msODTableName = sValue
      End Set
   End Property
   Public Property ODTableDescription() As String
      Get
         Return msODTableDescription
      End Get
      Set(ByVal sValue As String)
         msODTableDescription = sValue
      End Set
   End Property
   Public Shared Function IsOverlayODTableName(ByVal sODTableName As String) As Boolean
      Dim saODTableName() As String = Split(sODTableName, msNameDel)
      Return (saODTableName.GetUpperBound(0) = 2) AndAlso (saODTableName(0) = msODOverlayTablePrefix)
   End Function

End Class
