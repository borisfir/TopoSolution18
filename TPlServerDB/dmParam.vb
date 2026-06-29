Option Explicit On
Option Strict On
Imports System.Data
Public Class dmParams
	Private miProjectCode As Integer
	Private miDetailNo As Integer
	Private miMapThemeID As Integer 'DMAcadExt.enMapTheme
	Private miParamType As Integer
	Private mdicParamItems As IDictionary(Of Integer, dmParamItem)
	Private mbModified As Boolean
	Public Sub New(iProjectCode As Integer, iDetailNo As Integer, iMapThemeID As Integer, iParamType As Integer)
		Dim tParamItem As dmParamItem
		miProjectCode = iProjectCode
		miDetailNo = iDetailNo
		miMapThemeID = iMapThemeID
		miParamType = iParamType
		'	System.Windows.Forms.MessageBox.Show(CStr(iProjectCode) & ":" & CStr(iDetailNo) & ":" & CStr(iMapThemeID) & ":" & CStr(iParamType), "01_211")
      Try
         Dim oDataReader As System.Data.Common.DbDataReader = Nothing
         Select Case TPlServerDB.ServerDB.CurrentServerDB.Provider
            Case TPlServerDB.TPlProvider.ProviderSQLServer
               oDataReader = zzNewSQL()
            Case TPlServerDB.TPlProvider.ProviderJet
               oDataReader = zzNewOleDB()
         End Select


         If oDataReader IsNot Nothing Then
            mdicParamItems = New Dictionary(Of Integer, dmParamItem)
            Do While oDataReader.Read
               tParamItem = New dmParamItem(oDataReader)
               mdicParamItems.Add(tParamItem.ID, tParamItem)
            Loop
            oDataReader.Close()
         Else
            System.Windows.Forms.MessageBox.Show("", "01_211")
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmParams - New")
      End Try


	End Sub

	Private Function zzNewSQL() As System.Data.Common.DbDataReader
		'	System.Windows.Forms.MessageBox.Show(CStr(iProjectCode) & ":" & CStr(iDetailNo) & ":" & CStr(iMapThemeID) & ":" & CStr(iParamType), "01_211")
		Const sSPName As String = "GetPrjThemeParams"
		Dim oaParams(3) As System.Data.Common.DbParameter
		Dim oErrOut As System.Data.Common.DbException = Nothing


		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, miMapThemeID)
		oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prParamType", DbType.Int32, miParamType)
		Try
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
			Return oDataReader
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmParams - New")
			Return Nothing
		End Try


	End Function
	Private Function zzNewOleDB() As System.Data.Common.DbDataReader
		Dim sComText As String = ""
		Try
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText, CommandType.Text)
			Return oDataReader
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmParams - New")
			Return Nothing
		End Try
	End Function
	Public ReadOnly Property Item(iParamID As Integer) As dmParamItem
		Get
			Try
				Return mdicParamItems.Item(iParamID)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "dmParams - Item")
				Return Nothing
			End Try
		End Get
	End Property
	Public ReadOnly Property Modified As Boolean
		Get
			Return mbModified
		End Get
	End Property
	Public Function GetIntValue(iParamID As Integer) As Integer
		Dim oParamItem As dmParamItem = Me.Item(iParamID)
		If oParamItem IsNot Nothing Then
			Return oParamItem.IntValue
		Else
			Return [Integer].MinValue
		End If
	End Function
	Public Function GetDblValue(iParamID As Integer) As Double
		Dim oParamItem As dmParamItem = Me.Item(iParamID)
		If oParamItem IsNot Nothing Then
			Return oParamItem.DblValue
		Else
			Return [Double].MinValue
		End If
	End Function
	Public Function GetStrValue(iParamID As Integer) As String
		Dim oParamItem As dmParamItem = Me.Item(iParamID)
		If oParamItem IsNot Nothing Then
			Return oParamItem.StrValue
		Else
			Return String.Empty
		End If
	End Function
	Public Sub SetValue(iParamID As Integer, oValue As System.Object)
		Dim oParamItem As dmParamItem = Me.Item(iParamID)
		If oParamItem IsNot Nothing Then
			oParamItem.Value = oValue
			mbModified = True
		Else
			System.Windows.Forms.MessageBox.Show("Parameter #" & CStr(iParamID) & " was not found", "dmParams - SetValue")
		End If
	End Sub
	Public Sub SetIdData(sFileName As String)
		If sFileName IsNot Nothing AndAlso System.Environment.UserName <> "Boris" Then
			Me.SetValue(101, sFileName)
			'	System.Windows.Forms.MessageBox.Show(System.Environment.UserName, "04_231")
			Me.SetValue(102, System.Environment.UserName)
			Me.SetValue(103, System.Environment.MachineName)
			Me.SetValue(104, Date.Now)
		End If
	End Sub

	Public Sub Update()
		If mbModified Then
			Dim sSelectComText As String = "SELECT ProjectCode,Detail,MapThemeID,ParamType, ParamID, [ParamValue] FROM dbo.PrjParams WHERE (ProjectCode = " & CStr(miProjectCode) & ") AND (Detail = " & CStr(miDetailNo) & ") AND (MapThemeID = " & CStr(miMapThemeID) & ") AND (ParamType = " & CStr(miParamType) & ")"
			Dim oDbDataAdapter As System.Data.Common.DbDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSelectComText, CommandType.Text, True)
			If oDbDataAdapter IsNot Nothing Then
				Dim oDataTable As DataTable = New DataTable
				oDbDataAdapter.Fill(oDataTable)
				Dim oPrimaryKey() As DataColumn = {oDataTable.Columns.Item("ParamID")}
				oDataTable.PrimaryKey() = oPrimaryKey
				Dim oDataRow As DataRow

				For Each oParamItem As dmParamItem In mdicParamItems.Values
					If oParamItem.Modified Then
						If oParamItem.Exists Then
							oDataRow = oDataTable.Rows.Find(oParamItem.ID)
							If oDataRow IsNot Nothing Then
								oDataRow.Item("ParamValue") = oParamItem.GetDBValue
							End If
						Else
							oDataRow = oDataTable.NewRow()
							oDataRow.Item("ProjectCode") = miProjectCode
							oDataRow.Item("Detail") = miDetailNo
							oDataRow.Item("MapThemeID") = miMapThemeID
							oDataRow.Item("ParamType") = miParamType
							oDataRow.Item("ParamID") = oParamItem.ID
							oDataRow.Item("ParamValue") = oParamItem.GetDBValue
							oDataTable.Rows.Add(oDataRow)
						End If
						oParamItem.Update()
					End If
				Next
				Dim iRes As Integer = oDbDataAdapter.Update(oDataTable)
			End If
		End If
	End Sub
End Class
Public Class dmParamItem
	Public ID As Integer
	Public DataType As DbType
	'	Public DBDefaultValue As String
	'	Public DBValue As String
	Private moValue As System.Object
	Private msValue As String
	Private mbExists As Boolean
	Private mbModified As Boolean
	Public Sub New(oDataReader As Common.DbDataReader)
		Dim sDBDefaultValue As String
		'	Dim sDBValue As String
		'	Dim sValue As String
		ID = oDataReader.GetInt32(0)
		DataType = zzGetDataType(oDataReader.GetInt32(1))
		If oDataReader.IsDBNull(2) Then
			sDBDefaultValue = String.Empty
		Else
			sDBDefaultValue = oDataReader.GetString(2)
		End If

		If oDataReader.IsDBNull(3) Then
			'	sDBValue = String.Empty
			msValue = sDBDefaultValue
			mbExists = False
		Else
			msValue = oDataReader.GetString(3)

			mbExists = True
		End If

		If msValue.Length <> 0 Then
			zzConvert(msValue)
		End If


	End Sub
	Public Property Value As System.Object
		Get
			Return moValue
		End Get
		Set(oValue As System.Object)
			Dim sValue As String = oValue.ToString()
			If msValue <> sValue Then
				moValue = oValue
				msValue = sValue
				mbModified = True
			End If
		End Set
	End Property
	Public ReadOnly Property Modified As Boolean
		Get
			Return mbModified
		End Get
	End Property
	Public ReadOnly Property Exists As Boolean
		Get
			Return mbExists
		End Get
	End Property
	Public Function GetDBValue() As String
		Return Value.ToString()
	End Function
	Public ReadOnly Property ShortValue As Short
		Get
			If DataType = DbType.Int16 Then
				Return DirectCast(Value, Short)
			Else
				Return Short.MinValue
			End If
		End Get
	End Property
	Public Property IntValue As Integer
		Get
			If DataType = DbType.Int32 Then
				Return DirectCast(Value, Integer)
			Else
				Return Integer.MinValue
			End If
		End Get
		Set(iValue As Integer)
			Value = iValue
			mbModified = True
		End Set
	End Property
	Public ReadOnly Property LongValue As Long
		Get
			If DataType = DbType.Int32 Then
				Return DirectCast(Value, Long)
			Else
				Return Long.MinValue
			End If
		End Get
	End Property
	Public Sub Update()
		If mbModified Then
			mbModified = False
		End If
		If Not mbExists Then
			mbExists = True
		End If
	End Sub
	Public Property DblValue As Double
		Get
			If DataType = DbType.Double Then
				Return DirectCast(Value, Double)
			Else
				System.Windows.Forms.MessageBox.Show("ParameterID= #" & CStr(ID) & ":" & DataType.ToString(), "01_033")
				Return Double.MinValue
			End If
		End Get
		Set(dValue As Double)
			Value = dValue
			mbModified = True
		End Set
	End Property
	Public ReadOnly Property StrValue As String
		Get
			If DataType = DbType.String Then
				Return DirectCast(Value, String)
			Else
				Return String.Empty
			End If
		End Get
	End Property

	Private Function zzGetDataType(iDataType As Integer) As DbType
		If [Enum].IsDefined(GetType(DbType), iDataType) Then
			Return CType(iDataType, DbType)
		Else
			Return DbType.String
		End If
	End Function
	Private Sub zzConvert(sValue As String)
		Try
			Select Case DataType
				Case DbType.Double
					moValue = Convert.ToDouble(sValue)
				Case DbType.Int16
					moValue = Convert.ToInt16(sValue)
				Case DbType.Int32
					moValue = Convert.ToInt32(sValue)
				Case DbType.Int64
					moValue = Convert.ToInt64(sValue)
				Case DbType.String
					moValue = sValue
				Case DbType.DateTime
					moValue = Convert.ToDateTime(sValue)
				Case Else
					Try
						moValue = sValue.ToString()
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "dmParams - zzConvert")
					End Try

			End Select
		Catch oEx As Exception

		End Try

	End Sub
End Class