Option Explicit On
Option Strict On
Imports System.Data
Public Class ProjectDB
	Inherits dmDBManager
	Private miProjectCode As Integer
	Private miDetailNo As Integer = 0
	Private miEmployeeID As Integer = 0

	Public Shared Function GetProjectDetailCode(iProjectCode As Integer, iDetailNo As Integer) As String
      If iProjectCode > 0 Then
         Dim sRes As String = Convert.ToString(iProjectCode)
         If iDetailNo <> 0 Then
            sRes &= "/" & Convert.ToString(iDetailNo)
         End If
         Return sRes
      Else
         Return String.Empty
      End If
   End Function
	Public Sub GetGeneralList(ByRef oComboBox As System.Windows.Forms.ComboBox, ByVal iTableID As Integer, ByVal iTableVersion As Integer)
		Dim sComText As String = "SELECT ID,Text FROM GeneralList WHERE (TableID=" & CStr(iTableID) & ") AND (TableVersion=" & CStr(iTableVersion) & ")"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		oComboBox.ValueMember = DMCommon.ItemData.ValueMember
		oComboBox.DisplayMember = DMCommon.ItemData.DisplayMember

		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				oComboBox.Items.Add(New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1)))
			End While
		End If
		oDataReader.Close()
		Return
	End Sub

	Public Function GetGeneralList(ByVal iTableID As Integer, ByVal iTableVersion As Integer) As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT ID,Text FROM GeneralList WHERE (TableID=" & CStr(iTableID) & ") AND (TableVersion=" & CStr(iTableVersion) & ")"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.ItemData) = New System.Collections.Generic.List(Of DMCommon.ItemData)

		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				oList.Add(New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1)))
			End While
		End If
		oDataReader.Close()
		Return oList
	End Function
	Public Property ProjectCode As Integer
		Get
			Return miProjectCode
		End Get
		Set(iValue As Integer)
			miProjectCode = iValue
		End Set
	End Property
	Public Property DetailNo As Integer
		Get
			Return miDetailNo
		End Get
		Set(iValue As Integer)
			miDetailNo = iValue
		End Set
	End Property

	Public Property EmployeeID As Integer
		Get
			Return miEmployeeID
		End Get
		Set(iValue As Integer)
			miEmployeeID = iValue
		End Set
	End Property


	Public ReadOnly Property ProjectDetailCode As String
		Get
			Return GetProjectDetailCode(miProjectCode, miDetailNo)
		End Get

	End Property

	Public Function GetColorSchemeDataReader(iMapThemeID As Integer, sDB_SPName As String) As System.Data.Common.DbDataReader



		Dim oaParams(2) As System.Data.Common.DbParameter
		'	Dim oErrOut As System.Data.Common.DbException = Nothing
		oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
		oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, miDetailNo)
		oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, iMapThemeID)
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sDB_SPName, CommandType.StoredProcedure, oaParams)
		Return oDataReader

	End Function
	Public Function GetParcelingTypes() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT ID, Name FROM dbo.ParcelingTypes"
		'Dim sWhereText As String = " WHERE (Boundary = 1)"




		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.ItemData) = New System.Collections.Generic.List(Of DMCommon.ItemData)
		Dim oItem As DMCommon.ItemData
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				oItem = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
				oList.Add(oItem)
			End While
		End If
		oDataReader.Close()
		Return oList
	End Function
End Class
