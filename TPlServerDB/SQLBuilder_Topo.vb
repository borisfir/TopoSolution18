Option Explicit On
Option Strict On
Public Enum TplnDataType
	[Short]
	Int
	Float
	[Text]
	[Date]
	Bool
	Expresion
End Enum
Public Enum FindType
	All
	Whole
	Start
End Enum
Public Class SQLBuilder
	Const msAnd As String = " AND "
	Const mgsOr As String = " OR "
	Const msSpace As String = " "



	Private moListFields As System.Collections.Generic.List(Of Field)
	Private msTableName As String
	Public Sub New(ByVal sTableName As String)
		moListFields = New System.Collections.Generic.List(Of Field)
		msTableName = sTableName
	End Sub
	Public Shared Function GetFindComText(sFindText As String, sFieldName As String, iType As FindType) As String
		'	Dim sWhereStr As String = String.Empty
		Dim lInputLen As Long

		lInputLen = Len(sFindText)
		sFindText = Replace(sFindText, "'", "''", 1&, -1&, vbBinaryCompare)
		Select Case iType
			Case FindType.All
				If Len(sFindText) <> 0 Then
					Return "CHARINDEX( '" & sFindText & "'," & sFieldName & ") <> 0"
				End If
			Case FindType.Whole
				If Len(sFindText) <> 0 Then
					sFindText = msSpace & sFindText & msSpace
					Return "CHARINDEX( '" & sFindText & "',' '+" & sFieldName & "+' ') <> 0"

				End If

			Case FindType.Start
				If Len(sFindText) <> 0& Then

					Return "(SUBSTRING(" & sFieldName & ",1," & CStr(lInputLen) & ")='" & sFindText & "')"

				End If


        End Select
        Return String.Empty
	End Function
	Public Sub AddField(ByVal sFieldName As String, ByVal iDataType As TplnDataType, ByVal oValue As System.Object, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		moListFields.Add(New Field(sFieldName, iDataType, oValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal sValue As String, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Text
		moListFields.Add(New Field(sFieldName, iDataType, sValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal iValue As Integer, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Int
		moListFields.Add(New Field(sFieldName, iDataType, iValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal shValue As Short, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Short
		moListFields.Add(New Field(sFieldName, iDataType, shValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal dValue As Double, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Float
		moListFields.Add(New Field(sFieldName, iDataType, dValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal dtValue As Date, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Float
		moListFields.Add(New Field(sFieldName, iDataType, dtValue, bUpdatable, bCreatable))
	End Sub
	Public Sub AddField(ByVal sFieldName As String, ByVal bValue As Boolean, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
		Dim iDataType As TplnDataType = TplnDataType.Float
		moListFields.Add(New Field(sFieldName, iDataType, bValue, bUpdatable, bCreatable))
	End Sub
	Public Function GetAppendValueComText() As String

		Const sComma As String = ","
		Dim oField As Field
		Dim sFieldList As String = String.Empty
		Dim sValueList As String = String.Empty
		For iIndex As Integer = 0 To moListFields.Count - 1
			oField = moListFields.Item(iIndex)
			If oField.Creatable AndAlso oField.HasValue Then
				If sFieldList.Length <> 0 Then
					sFieldList &= sComma
					sValueList &= sComma
				End If
				sFieldList &= oField.FieldName
				sValueList &= oField.GetComText()
			End If
		Next
		Return "INSERT INTO " & msTableName & " (" & sFieldList & ") SELECT " & sValueList
	End Function
	Public Function GetDeleteComText() As String
		Const sAnd As String = " AND "
		Dim oField As Field
		Dim sComText As String = String.Empty

		For iIndex As Integer = 0 To moListFields.Count - 1
			oField = moListFields.Item(iIndex)
			If oField.HasValue Then
				If sComText.Length <> 0 Then
					sComText &= sAnd
				End If
				sComText &= oField.GetCriteriaComText()
			End If
		Next
		If sComText.Length = 0 Then
			Return String.Empty
		Else
			Return "DELETE FROM " & msTableName & " WHERE " & sComText
		End If

	End Function

	Public Function GetUpdateComText(sCriteria As String) As String
		Const sComma As String = ","
		Dim oField As Field
		Dim sComText As String = String.Empty

		For iIndex As Integer = 0 To moListFields.Count - 1
			oField = moListFields.Item(iIndex)
			If oField.Updatable Then
				If sComText.Length <> 0 Then
					sComText &= sComma
				End If
				sComText &= oField.GetUpdateComText()
			End If
		Next
		'	Return "INSERT INTO " & msTableName & " (" & sFieldList & ") SELECT " & sValueList
		Return "UPDATE " & msTableName & " SET " & sComText
	End Function
	Private Class Field
		Private msFieldName As String
		Private miDataType As TplnDataType
		Private moValue As System.Object
		Private mbUpdatable As Boolean
		Private mbCreatable As Boolean

		Public Sub New(ByVal sFieldName As String, ByVal iDataType As TplnDataType, ByVal oValue As System.Object, Optional bUpdatable As Boolean = True, Optional bCreatable As Boolean = True)
			msFieldName = sFieldName
			miDataType = iDataType
			moValue = oValue
			mbUpdatable = bUpdatable
			mbCreatable = bCreatable
		End Sub
		Public ReadOnly Property FieldName As String
			Get
				Return msFieldName
			End Get
		End Property
		Public ReadOnly Property Updatable As Boolean
			Get
				Return mbUpdatable
			End Get
		End Property
		Public ReadOnly Property Creatable As Boolean
			Get
				Return mbCreatable
			End Get
		End Property
		Public ReadOnly Property HasValue As Boolean
			Get
				If moValue Is Nothing Then
					Return False
				Else
					Select Case miDataType
						Case TPlServerDB.TplnDataType.Text
							Dim sValue As String = DirectCast(moValue, String)
							If sValue.Length = 0 Then
								Return False
							End If
						Case TPlServerDB.TplnDataType.Int
							Dim iValue As Integer = DirectCast(moValue, Integer)
							If iValue = Integer.MinValue Then
								Return False
							End If
						Case TPlServerDB.TplnDataType.Short
							Dim shValue As Short = DirectCast(moValue, Short)
							If shValue = Short.MinValue Then
								Return False
							End If
						Case TPlServerDB.TplnDataType.Date
							Try
								Dim dtValue As Date = DirectCast(moValue, Date)
							Catch oEx As Exception
								'StopA
							End Try


					End Select
				End If
				Return True
			End Get
		End Property
		Public Function GetComText() As String
			Return zzGetValueComText() & " AS " & msFieldName
		End Function
		Public Function GetUpdateComText() As String
			Return msFieldName & " = " & zzGetValueComText()
		End Function
		Public Function GetCriteriaComText() As String
			Return "(" & msFieldName & " = " & zzGetValueComText() & ")"
		End Function
		Private Function zzGetValueComText() As String
			Const sDel As String = "'"
			If moValue Is Nothing Then
				Return " NULL "
			Else
				Select Case miDataType
					Case TPlServerDB.TplnDataType.Short
						Dim iValue As Integer = Convert.ToInt16(moValue)
						Return Convert.ToString(iValue)
					Case TPlServerDB.TplnDataType.Int
						Dim iValue As Integer = DirectCast(moValue, Integer)
						Return Convert.ToString(iValue)
					Case TPlServerDB.TplnDataType.Float
						Dim dValue As Double = DirectCast(moValue, Double)
						Return Convert.ToString(dValue)
					Case TPlServerDB.TplnDataType.Text
						Dim sValue As String = DirectCast(moValue, String)
						Return sDel & Replace(sValue, sDel, sDel & sDel) & sDel
					Case TPlServerDB.TplnDataType.Date
						Dim dtValue As Date = DirectCast(moValue, Date)
						Return sDel & zzGetSQLDate(dtValue, True, True) & sDel
					Case TPlServerDB.TplnDataType.Expresion
						Dim sExpresion As String = DirectCast(moValue, String)
						Return sExpresion
					Case Else
						Return String.Empty
				End Select
			End If

		
		End Function
		Private Shared Function zzGetSQLDate(ByVal dtValue As Date, ByVal bSQLDate As Boolean, Optional ByVal bTime As Boolean = False) As String
			Const sSQLDateDel As String = "/"
			Const sNumStyle As String = "00"
			If bSQLDate Then
				Dim sTime As String
				If bTime Then
					sTime = " " & Strings.Format(dtValue, "HH:mm")
				Else
					sTime = String.Empty
				End If
				Return Format(dtValue.Month, sNumStyle) & sSQLDateDel & Format(dtValue.Day, sNumStyle) & sSQLDateDel & Format(dtValue.Year, sNumStyle) & sTime
			Else
				Return dtValue.ToShortDateString
			End If
		End Function
	End Class
End Class

