Option Explicit On 
Option Strict On
Imports System.Data

Public NotInheritable Class ServerDB
	Inherits dmDBManager
	Public Const DBServerName As String = "DBSQL01" ' "PLUTO"
	Public Shared CurrentServerDB As TPlServerDB.ServerDB
   Public Shared CurrentProjectDB As TPlServerDB.ProjectDB
	'   Public Shared CurrentExcelReport As E
	'	Public f As dmDBManager

	Private Shared miServerProvider As TPlProvider
	Private Shared mDbCnn As System.Data.Common.DbConnection

	Private Shared mDtTblObjectDefs As System.Data.DataTable
	Private Shared mDtTblSysParameters As System.Data.DataTable


	Private msServerDBName As String
	Private Shared msDBResourceFolder As String
	'	Private Shared mdicObjectSharedDictionary As ObjectSharedDictionary
	Private Shared mbTransactionOpened As Boolean = False


	Private Shared Sub zzOpenData()
		Try
			''''	zzOpenServerTable(mDtTblObjectDefs, "ObjectDefs", "ObjectDefs", "ObjectType")
			''''''''''	zzOpenServerTable(mDtTblSysParameters, "SysParameters", "SysParameters", "ID", "Argument")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "ServerDB - zzOpenData")
		End Try

	End Sub

	

	'  03-560-78-01

	Public Property ServerDBName() As String
		Get
			Return msServerDBName
		End Get
		Set(ByVal sValue As String)
			msServerDBName = sValue
		End Set
	End Property
	Public Shared Function GetSysParameter(ByVal iParamID As Integer, ByVal iArg As Integer) As String
		Dim iaKey(1) As System.Object
		Dim oDataRow As System.Data.DataRow
		iaKey(0) = iParamID
		iaKey(1) = iArg
		oDataRow = mDtTblSysParameters.Rows.Find(iaKey)
		GetSysParameter = DirectCast(oDataRow.Item("Value"), String)
	End Function
	Public ReadOnly Property DBResourceFolder() As String
		Get
			Return msDBResourceFolder
		End Get
	End Property

	Public Shared Function GetConnectionStringAAA(ByVal iProvider As TPlProvider) As String
		Dim sProviderName As String
		Dim sDataSource As String = Space(0)
		sProviderName = GetProviderNameAAA(iProvider)
		Select Case iProvider
			Case TPlProvider.ProviderJet
				Return "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=0;Data Source=" & sDataSource & ";Jet OLEDB:Engine " & _
				"Type=5;Provider=""" & sProviderName & """;Jet OLEDB:System database=;Jet OLEDB:SFP=False;persist security info=False;Extended Properties=;Mode=Share Deny None;" & _
				"Jet OLEDB:Encrypt Database=False;Jet OLEDB:Create System Database=False;Jet OLEDB:" & _
				"Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;" & _
				"User ID=Admin;Jet OLEDB:Global Bulk Transactions=1"
			Case TPlProvider.ProviderSQLServer
				Return "Persist Security Info=False;User ID=sa;Password=bf2679;Initial Catalog=ShopData;Data Source=DS;Packet Size=4096;Workstation ID=DM208"
			Case TPlProvider.ProviderOracle
				Return Space(0)
			Case Else
				Return Space(0)

		End Select
	End Function
	Friend Shared Function GetProviderNameAAA(ByVal iProvider As TPlProvider) As String
		Select Case iProvider
			Case TPlProvider.ProviderJet
				Return "Microsoft.Jet.OLEDB.4.0"
			Case TPlProvider.ProviderSQLServer
				Return Space(0)
			Case TPlProvider.ProviderOracle
				Return Space(0)
			Case Else
				Return Space(0)
		End Select
	End Function


	Protected Overrides Sub Finalize()
		On Error Resume Next

		MyBase.Finalize()
	End Sub
	Public Function GetProjectList(ByVal iListSource As enProjectListSource) As TPlProjectInfo()
		Dim oDbDataReader As Common.DbDataReader = ServerDB.CurrentServerDB.GetDataReader("SELECT * FROM ProjectListExt")
		Dim oaProjectItem() As TPlProjectInfo = Nothing
		Dim iIndex As Integer = 0
		Dim iStage As Integer
		While oDbDataReader.Read()
			ReDim Preserve oaProjectItem(iIndex)
			oaProjectItem(iIndex) = New TPlProjectInfo
			oaProjectItem(iIndex).ProjectID = oDbDataReader.GetInt32(0)
			oaProjectItem(iIndex).ProjectName = oDbDataReader.GetString(1)
			oaProjectItem(iIndex).DataSource = oDbDataReader.GetString(2)
			oaProjectItem(iIndex).CreatedDate = oDbDataReader.GetDateTime(3)
			oaProjectItem(iIndex).ModifiedDate = oDbDataReader.GetDateTime(4)
			oaProjectItem(iIndex).AccessedDate = oDbDataReader.GetDateTime(5)
			oaProjectItem(iIndex).CommitteeName = oDbDataReader.GetString(6)
			oaProjectItem(iIndex).LocalityName = oDbDataReader.GetString(7)
			iStage = oDbDataReader.GetInt32(8)
			oaProjectItem(iIndex).Provider = CType(oDbDataReader.GetInt32(9), TPlProvider)
			oaProjectItem(iIndex).SingleProject = oDbDataReader.GetBoolean(10)
			iIndex += 1
		End While
		oDbDataReader.Close()
		Return oaProjectItem

   End Function
   Public Function GetEmplioyeeName(ByVal sUser As String) As String
      Dim sComText As String = "SELECT Empl_NameFamily FROM ShopData.dbo.Employees WHERE ({ fn UCASE(Empl_username) } = '" & sUser & "')"
      Dim oEmplName As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, CommandType.Text)

		Return DMCommon.Functions.CStrN(oEmplName)
   End Function

   Public Function GetProjectName(ByVal iProjectCode As Integer) As String
      Dim sComText As String = "SELECT PrjName FROM dbo.ProjectList WHERE (PrjCode = " & iProjectCode.ToString() & ")"
      Dim oPrjName As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, CommandType.Text)
      ' System.Windows.Forms.MessageBox.Show(sComText & ":" & DMCommon.Functions.CStrN(oPrjName, "Name is Nothing"), "ServerDB - GetLocalityList")
      Return DMCommon.Functions.CStrN(oPrjName)
   End Function
	Public Function GetProjectData(ByVal iListSource As enProjectListSource) As DataTable
		Dim sComText As String
		Dim sPlanStatus As String = [Enum].Format(GetType(enListType), enListType.PlanStatus, "d")
		Dim sProvider As String = [Enum].Format(GetType(enListType), enListType.Provider, "d")

		sComText = "SELECT ProjectListExt.*, GeneralList.Text1037 AS StatusName, GeneralList_1.Text1037 AS ProviderName FROM (ProjectListExt LEFT JOIN GeneralList AS GeneralList_1 ON ProjectListExt.Provider = GeneralList_1.ID) LEFT JOIN GeneralList ON ProjectListExt.Status = GeneralList.ID WHERE (((GeneralList.Type)=" & sPlanStatus & ") AND ((GeneralList_1.Type)=" & sProvider & "))"

		Return GetDataTable(sComText, CommandType.Text, "ProjectList")

	End Function

	Public Function AddProjectToList(ByVal sName As String, ByVal iCommitteeID As Integer, ByVal iLocalityID As Integer, ByVal iStageNo As Integer, ByVal iProjectDB_ID As Integer) As Integer
		Dim sComText As String
		Dim iProjectID As Integer
		Dim oDbDataReader As Common.DbDataReader
		Dim oDbTransaction As System.Data.Common.DbTransaction
		Dim bResp As Boolean
		sComText = "SELECT Max(ProjectID) AS MaxOfProjectID FROM ProjectList"

		oDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		bResp = oDbDataReader.Read()
		If bResp Then
			If oDbDataReader.IsDBNull(0) Then
				iProjectID = 1
			Else
				iProjectID = oDbDataReader.GetInt32(0) + 1
			End If
		Else
			iProjectID = 1
		End If
		oDbDataReader.Close()
		sName = Replace(sName, "'", "''")
		sComText = "INSERT INTO ProjectList(ProjectID,ProjectName,CreatedDate,ModifiedDate,AccessedDate,CommitteeID,LocalityID,Status,ProjectDB_ID) SELECT " & CStr(iProjectID) & " AS ProjectID,'" & sName & "' AS ProjectName, Now() AS CreatedDate, Now() AS ModifiedDate, Now() AS AccessedDate," & CStr(iCommitteeID) & " AS CommitteeID," & CStr(iLocalityID) & " AS LocalityID," & CStr(iStageNo) & " AS Status," & CStr(iProjectDB_ID) & " AS ProjectDB_ID;"

		Dim oDbCommand As System.Data.Common.DbCommand = GetNewCommand(sComText, CommandType.Text)
		oDbTransaction = mDbCnn.BeginTransaction(IsolationLevel.ReadCommitted)
		oDbCommand.Transaction = oDbTransaction
		mbTransactionOpened = True
		Try
			oDbCommand.ExecuteNonQuery()
			oDbTransaction.Commit()
		Catch oEx As Exception
			Try
				oDbTransaction.Rollback()
			Catch ex As System.Data.OleDb.OleDbException
				If Not oDbTransaction.Connection Is Nothing Then
					Console.WriteLine("An exception of type " & ex.GetType().ToString() & _
					  " was encountered while attempting to roll back the transaction.")
				End If
			End Try
			Console.WriteLine("An exception of type " & oEx.GetType().ToString() & _
			"was encountered while inserting the data.")
			Console.WriteLine("Neither record was written to database.")

		End Try
		Return iProjectID
	End Function
	Private Shared Function zzGetNewConnection() As Common.DbConnection
		If miServerProvider = TPlProvider.ProviderJet Then
			Return New System.Data.OleDb.OleDbConnection()
		ElseIf miServerProvider = TPlProvider.ProviderSQLServer Then
			Return New System.Data.SqlClient.SqlConnection()
		Else
			Return Nothing
		End If
	End Function

	Public Function AddProjectDBToList(ByVal sDataSource As String, ByVal iProvider As TPlProvider) As Integer

		Dim sComText As String

		Dim pOleDbTransaction As System.Data.Common.DbTransaction
		Dim iProjectDB_ID As Integer
		Dim pOleDbDataReader As Common.DbDataReader
		Dim pOleDbCommand As System.Data.Common.DbCommand
		Dim bResp As Boolean
		sComText = "SELECT Max(ProjectDB_ID) AS ProjectDB_IDMax FROM ProjectDBList"		 ' GROUP BY ProjectDB_ID"
		pOleDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		bResp = pOleDbDataReader.Read()
		If bResp Then
			If pOleDbDataReader.IsDBNull(0) Then
				iProjectDB_ID = 1
			Else
				iProjectDB_ID = pOleDbDataReader.GetInt32(0) + 1
			End If
		Else
			iProjectDB_ID = 1
		End If
		pOleDbDataReader.Close()

		pOleDbTransaction = mDbCnn.BeginTransaction(IsolationLevel.ReadCommitted)

		sComText = "INSERT INTO ProjectDBList(ProjectDB_ID,DataSource,Provider,SingleProject) SELECT " & CStr(iProjectDB_ID) & " AS ProjectDB_ID,'" & sDataSource & "' AS DataSource," & CStr(iProvider) & "  AS Provider," & CStr(False) & "  AS SingleProject;"
		pOleDbCommand = GetNewCommand(sComText, CommandType.Text)

		pOleDbCommand.Transaction = pOleDbTransaction

		mbTransactionOpened = True
		Try
			pOleDbCommand.ExecuteNonQuery()
			pOleDbTransaction.Commit()
			Return iProjectDB_ID
		Catch e As Exception
			Try
				pOleDbTransaction.Rollback()
			Catch ex As System.Data.OleDb.OleDbException
				If Not pOleDbTransaction.Connection Is Nothing Then
					Console.WriteLine("An exception of type " & ex.GetType().ToString() & _
					  " was encountered while attempting to roll back the transaction.")
				End If
			End Try
			Console.WriteLine("An exception of type " & e.GetType().ToString() & _
			  "was encountered while inserting the data.")
			Console.WriteLine("Neither record was written to database.")
			Exit Try

		End Try
	End Function
	Public Function GetProjectDBItemList(Optional ByVal iAddItems As Integer = 0, Optional ByVal iProvider As TPlProvider = TPlProvider.ProviderNotDefined) As System.Collections.ArrayList
		Dim oDbDataReader As System.Data.Common.DbDataReader
		Dim oaProjectDBItemList As System.Collections.ArrayList
		Dim oProjectDBItem As TPlProjectDBItem
		Dim iAddItem As enListAddItem
		Dim sComText As String = "SELECT ProjectDB_ID,DataSource,Provider,SingleProject FROM ProjectDBList"

		If iProvider <> TPlProvider.ProviderNotDefined Then
			sComText = sComText & " WHERE Provider=" & CType(iProvider, String)
		End If
		oDbDataReader = GetDataReader(sComText)
		oaProjectDBItemList = New ArrayList
		iAddItem = enListAddItem.[New]
		If (iAddItem And iAddItem) = iAddItem Then
			oProjectDBItem = New TPlProjectDBItem
			oProjectDBItem.ListIndex = enListAddItem.[New]
			oProjectDBItem.ListDispData = TextResource.GetText(-enListAddItem.[New], enResourceTheme.ProjectDBItem, enListType.ProjectDB)
			oaProjectDBItemList.Add(oProjectDBItem)
		End If
		While oDbDataReader.Read()
			oProjectDBItem = New TPlProjectDBItem
			oProjectDBItem.ID = oDbDataReader.GetInt32(0)
			oProjectDBItem.DataSource = oDbDataReader.GetString(1)
			oProjectDBItem.Provider = CType(oDbDataReader.GetInt32(2), TPlProvider)
			oProjectDBItem.SingleProject = oDbDataReader.GetBoolean(3)
			oaProjectDBItemList.Add(oProjectDBItem)
		End While
		oDbDataReader.Close()
		Return oaProjectDBItemList

	End Function
	Public Function GetProjectDBItemDict(Optional ByVal iAddItems As Integer = 0, Optional ByVal iProvider As TPlProvider = TPlProvider.ProviderNotDefined) As DMCommon.ItemDataDict
		Dim oDBDataReader As System.Data.Common.DbDataReader
		Dim odicItemList As DMCommon.ItemDataDict = New DMCommon.ItemDataDict(False)
		Dim oProjectDBItem As TPlProjectDBItem
		Dim iAddItem As enListAddItem
		Dim sComText As String = "SELECT ProjectDB_ID,DataSource,Provider,SingleProject FROM ProjectDBList"

		If iProvider <> TPlProvider.ProviderNotDefined Then
			sComText = sComText & " WHERE Provider=" & CType(iProvider, String)
		End If
		oDBDataReader = GetDataReader(sComText)

		iAddItem = enListAddItem.[New]
		If (iAddItem And iAddItem) = iAddItem Then
			oProjectDBItem = New TPlProjectDBItem
			oProjectDBItem.ListIndex = enListAddItem.[New]
			oProjectDBItem.ListDispData = TextResource.GetText(-enListAddItem.[New], TPlProjectDBItem.ResourceTheme, enListType.ProjectDB)
			odicItemList.Add(oProjectDBItem)
		End If
		While oDBDataReader.Read()
			oProjectDBItem = New TPlProjectDBItem
			oProjectDBItem.ID = oDBDataReader.GetInt32(0)
			oProjectDBItem.DataSource = oDBDataReader.GetString(1)
			oProjectDBItem.Provider = CType(oDBDataReader.GetInt32(2), TPlProvider)
			oProjectDBItem.SingleProject = oDBDataReader.GetBoolean(3)
			odicItemList.Add(oProjectDBItem)
		End While
		oDBDataReader.Close()
		Return odicItemList

	End Function
	Public Function GetSurveyorLicenseView() As DataTable
		'	Dim sComText As String = "SELECT * FROM SurveyorLicense ORDER BY LicenseNo"
		'	Return GetDataTable(sComText, CommandType.Text, "SurveyorLicense")
		Dim sComText As String = "SurveyorLicenseList"
		Return GetDataTable(sComText, CommandType.StoredProcedure, "SurveyorLicense")
	End Function
	Public Function GetLocalityList() As System.Collections.Generic.IList(Of DMCommon.LocalityItem)
		Dim sComText As String = "SELECT * FROM Localities ORDER BY Name"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.LocalityItem) = New System.Collections.Generic.List(Of DMCommon.LocalityItem)
		Dim oItem As DMCommon.LocalityItem
		Dim i As Integer
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				oItem = New DMCommon.LocalityItem(oDataReader.GetInt32(0), oDataReader.GetString(1))
				If Not oDataReader.IsDBNull(3) Then
					oItem.District = oDataReader.GetInt32(3)
				End If
				If Not oDataReader.IsDBNull(4) Then
					oItem.Subdistrict = oDataReader.GetInt32(4)
				End If

				If oDataReader.IsDBNull(6) Then
					oItem.MunicipialStatus = -1
				Else
					oItem.MunicipialStatus = oDataReader.GetInt32(6)
				End If

				If Not oDataReader.IsDBNull(7) Then
					oItem.Committee = oDataReader.GetInt32(7)
				End If



				oList.Add(oItem)
				i += 1
			End While
			oDataReader.Close()
		Else
			System.Windows.Forms.MessageBox.Show("Localities is Empty", "ServerDB - GetLocalityList")
		End If

		Return oList
	End Function
	Public Function GetLocalities() As DMCommon.Localities
		Dim sComText As String = "SELECT * FROM Localities ORDER BY Name"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.LocalityItem) = New System.Collections.Generic.List(Of DMCommon.LocalityItem)
		Dim oDictionary As System.Collections.Generic.IDictionary(Of Integer, DMCommon.LocalityItem) = New System.Collections.Generic.Dictionary(Of Integer, DMCommon.LocalityItem)

		Dim oItem As DMCommon.LocalityItem
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				oItem = New DMCommon.LocalityItem(oDataReader.GetInt32(0), oDataReader.GetString(1))
				oItem.District = oDataReader.GetInt32(3)
				oItem.Subdistrict = oDataReader.GetInt32(4)
				If oDataReader.IsDBNull(6) Then
					oItem.MunicipialStatus = -1
				Else
					oItem.MunicipialStatus = oDataReader.GetInt32(6)
				End If
				If oDataReader.IsDBNull(7) Then
					oItem.Committee = 0
				Else
					oItem.Committee = oDataReader.GetInt32(7)
				End If

				oList.Add(oItem)
				oDictionary.Add(oItem.ListIndex, oItem)
			End While
			oDataReader.Close()
		Else
			System.Windows.Forms.MessageBox.Show("Localities is Empty", "ServerDB - GetLocalityList")
		End If

		Return New DMCommon.Localities(oDictionary, oList)
	End Function
	Public Function GetMunicipialStatusesList() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT * FROM MunicipialStatuses ORDER BY Name"
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
	Public Function GetCommitteesList() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT   ID, ShortName FROM  dbo.CommitteesS ORDER BY ShortName"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.ItemData) = New System.Collections.Generic.List(Of DMCommon.ItemData)
		Dim oItem As DMCommon.ItemData

		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				'	DMCommon.Debug.ExcelLog.SetDataTable(0, "moProjectPlanTable", moProjectPlanTable)
				oItem = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
				oList.Add(oItem)
			End While
			oDataReader.Close()
		Else
			System.Windows.Forms.MessageBox.Show("Committees is Empty", "ServerDB - GetCommitteesList")
		End If

		Return oList
	End Function
	Public Function GetPlanTypeList() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "	Select ID, ShortName FROM dbo.PlanTypes WHERE  (ShortName Is Not NULL)"
		Dim oDataReader As Common.DbDataReader = Me.GetDataReader(sComText)
		Dim oList As System.Collections.Generic.IList(Of DMCommon.ItemData) = New System.Collections.Generic.List(Of DMCommon.ItemData)
		Dim oItem As DMCommon.ItemData

		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				'	DMCommon.Debug.ExcelLog.SetDataTable(0, "moProjectPlanTable", moProjectPlanTable)
				'DMCommon.Debug.MsgBox("!DataReader", oDataReader.GetInt32(0), oDataReader.GetString(1))
				oItem = New DMCommon.ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
				oList.Add(oItem)
			End While
			oDataReader.Close()
		Else
			System.Windows.Forms.MessageBox.Show("Committees is Empty", "ServerDB - GetCommitteesList")
		End If

		Return oList
	End Function




	Public Function GetScalesList() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT * FROM Scales "
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
	Public Function GetDistrictsList() As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT * FROM Districts ORDER BY DistrictName"
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

	Public Function GetSubdistrictsList() As System.Collections.Generic.IList(Of DMCommon.ItemData)

		Dim sComText As String = "SELECT SubdistrictID, SubdistrictName FROM Subdistricts ORDER BY SubdistrictName"


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
	Public Function GetMeasureMethods(bBoundary As Boolean) As System.Collections.Generic.IList(Of DMCommon.ItemData)
		Dim sComText As String = "SELECT ID, Name FROM dbo.MeasurementMethods"
		Dim sWhereText As String = " WHERE (Boundary = 1)"

		If bBoundary Then
			sComText &= sWhereText
		End If


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

	Public Function GetItemList(ByVal iListType As enListType, Optional ByVal iaArgs() As Integer = Nothing) As ArrayList
		Dim oDbDataReader As System.Data.Common.DbDataReader
		Dim oaItemList As ArrayList
		Dim sComText As String
		oaItemList = New ArrayList
		sComText = GetListComText(iListType, , iaArgs)
		oDbDataReader = GetDataReader(sComText)

		While oDbDataReader.Read()
			Select Case iListType
				Case enListType.Committee
					Dim oItem As New TPlCommitteeItem
					oItem.ID = oDbDataReader.GetInt32(0)
					oItem.Name = oDbDataReader.GetString(1)
					oItem.DistrictID = oDbDataReader.GetInt32(2)
					oaItemList.Add(oItem)
				Case enListType.LanduseD
					Dim oItem As New TPlLanduseItem(oDbDataReader.GetInt32(0), oDbDataReader.GetString(1), oDbDataReader.GetInt32(2))
					oaItemList.Add(oItem)
				Case Else
					Dim oItem As New DMCommon.ItemData
					oItem.ListIndex = oDbDataReader.GetInt32(0)
					oItem.ListDispData = oDbDataReader.GetString(1)
					oaItemList.Add(oItem)
			End Select

		End While
		oDbDataReader.Close()

		Return oaItemList
	End Function
	Public Function GetItemListD(ByVal iListType As enListType, Optional ByVal iaArgs() As Integer = Nothing) As TPlItemList
		Dim oDbDataReader As System.Data.Common.DbDataReader
		Dim odicItemList As TPlItemList
		Dim sComText As String
		odicItemList = New TPlItemList(iListType)
		sComText = GetListComText(iListType, , iaArgs)
		oDbDataReader = GetDataReader(sComText)

		While oDbDataReader.Read()
			Select Case iListType
				Case enListType.Committee
					Dim oItem As New TPlCommitteeItem
					oItem.ID = oDbDataReader.GetInt32(0)
					oItem.Name = oDbDataReader.GetString(1)
					oItem.DistrictID = oDbDataReader.GetInt32(2)
					odicItemList.Add(oItem)
				Case Else
					Dim oItem As New DMCommon.ItemData
					oItem.ListIndex = oDbDataReader.GetInt32(0)
					oItem.ListDispData = oDbDataReader.GetString(1)
					odicItemList.Add(oItem)
			End Select
		End While
		oDbDataReader.Close()
		Return odicItemList
	End Function
	Public Function GetItemListValue(ByVal iListType As enListType, ByVal iKey As Integer) As String
		Dim sComText As String
		Dim oDbDataReader As System.Data.Common.DbDataReader
		'	Dim sKeyFieldName As String
		'	Dim bWhereExists As Boolean
		Dim sOutput As String = String.Empty
		sComText = GetListComText(iListType, iKey)

		oDbDataReader = GetDataReader(sComText)

		While oDbDataReader.Read()
			Select Case iListType

			End Select
			sOutput = oDbDataReader.GetString(1)
		End While
		oDbDataReader.Close()
		Return sOutput
	End Function
	Public Function GetItemDict(ByVal iListType As enListType, Optional ByVal iaArgs() As Integer = Nothing, Optional ByVal bHasGrouping As Boolean = False, Optional ByVal iFieldIndex As Integer = -1, Optional ByVal iOptionID As Integer = 1) As DMCommon.ItemDataDict
		Dim oDbDataReader As System.Data.Common.DbDataReader
		Dim odicItemList As DMCommon.ItemDataDict
		Dim sComText As String
		Dim iGroupKey As Integer


		odicItemList = New DMCommon.ItemDataDict(bHasGrouping)
		sComText = GetListComText(iListType, , iaArgs, iOptionID)

		oDbDataReader = GetDataReader(sComText)

		While oDbDataReader.Read()

			Select Case iListType
				Case enListType.Committee

					Dim oItem As New TPlCommitteeItem
					oItem.ID = oDbDataReader.GetInt32(0)
					oItem.Name = oDbDataReader.GetString(1)
					oItem.ShortName = oDbDataReader.GetString(2)
					oItem.DistrictID = oDbDataReader.GetInt32(3)
					oItem.DistrictName = oDbDataReader.GetString(4)
					odicItemList.Add(oItem)

				Case enListType.LanduseD
					Dim oItem As New TPlLanduseItem(oDbDataReader.GetInt32(0), oDbDataReader.GetString(1), oDbDataReader.GetInt32(2))
					odicItemList.Add(oItem)
				Case enListType.LanduseM
					Dim oItem As New TPlLanduseItem(oDbDataReader.GetInt32(0), oDbDataReader.GetString(1), oDbDataReader.GetInt32(2))
					odicItemList.Add(oItem)

				Case Else
					Dim oItem As New DMCommon.ItemData
					oItem.ListIndex = oDbDataReader.GetInt32(0)
					oItem.ListDispData = oDbDataReader.GetString(1)
					If iFieldIndex >= 0 Then iGroupKey = oDbDataReader.GetInt32(iFieldIndex)
					odicItemList.Add(oItem, iGroupKey)
			End Select

		End While
		oDbDataReader.Close()
		Select Case iListType
			Case enListType.MainLanduse
				zzAddLanduseItems(odicItemList)
			Case enListType.Measure, enListType.Charact, enListType.Location
				zzDistributeItems(iListType, odicItemList)
		End Select

		Return odicItemList
	End Function
	
	Public Function GetDataRow(ByVal iKey As Integer) As System.Data.DataRow
		GetDataRow = mDtTblObjectDefs.Rows.Find(iKey)
	End Function
	Public Shared Sub InitCurrentServer()
		Dim oServerDB As ServerDB = New ServerDB()
		ServerDB.CurrentServerDB = oServerDB
	End Sub
	Public Shared Sub InitCurrentProject()
		Dim oProjectDB As ProjectDB = New ProjectDB()
		ServerDB.CurrentProjectDB = oProjectDB
	End Sub
	Public Shared Sub AddInitialize()
		zzOpenData()
		'	msDBResourceFolder = System.IO.Path.GetDirectoryName(sDBResourceFile)
		TextResource.Open()
	End Sub

	
	Public Function GetTableDef(ByVal iTableID As Integer) As DBTableDef
		Dim sComText As String
		Dim sTableName As String
		Dim pOleDbDataReader As System.Data.Common.DbDataReader
		Dim oDBTableDef As DBTableDef
		Dim oDBFieldDef As DBFieldDef
		Dim bResp As Boolean
		sComText = "SELECT TableID,TableName FROM TableList WHERE TableID=" & CStr(iTableID)
		pOleDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		bResp = pOleDbDataReader.Read()

		sTableName = pOleDbDataReader.GetString(1)

		pOleDbDataReader.Close()
		oDBTableDef = New DBTableDef(iTableID, sTableName)
		sComText = "SELECT FieldName,FieldType,FieldSize FROM DBFieldList WHERE (TableID=" & CStr(iTableID) & ")"
		pOleDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		' Always call Read before accessing data.

		While pOleDbDataReader.Read()
			oDBFieldDef = New DBFieldDef
			oDBFieldDef.Name = pOleDbDataReader.GetString(0)
			oDBFieldDef.DataType = pOleDbDataReader.GetInt32(1)
			oDBFieldDef.Size = pOleDbDataReader.GetInt32(2)
			oDBTableDef.FieldDefs.Add(oDBFieldDef)
		End While
		pOleDbDataReader.Close()
		GetTableDef = oDBTableDef
	End Function
	Public Function GetDefaultTableDefs() As System.Collections.ArrayList
		Dim sComText As String
		Dim iIndex As Integer
		Dim sTableName As String
		Dim iTableID As Integer
		Dim oDbDataReader As System.Data.Common.DbDataReader
		Dim oDBTableDef As DBTableDef = Nothing
		Dim oDBFieldDef As DBFieldDef
		Dim oDBIndexDef As DBIndexDef

		Dim iCurrentTableID As Integer = 0
		Dim oArray As New System.Collections.ArrayList

		sComText = "SELECT TableID,TableName,FieldName,FieldType,FieldSize FROM DefaultTables"

		oDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		While oDbDataReader.Read()
			iTableID = oDbDataReader.GetInt32(0)
			If iTableID <> iCurrentTableID Then			 'NewTableDef
				iCurrentTableID = iTableID
				If Not oDBTableDef Is Nothing Then
					oArray.Add(oDBTableDef)
				End If
				sTableName = oDbDataReader.GetString(1)
				oDBTableDef = New DBTableDef(iTableID, sTableName)
			End If
			oDBFieldDef = New DBFieldDef
			With oDBFieldDef
				.Name = oDbDataReader.GetString(2)
				.DataType = oDbDataReader.GetInt32(3)
				.Size = oDbDataReader.GetInt32(4)
			End With
			oDBTableDef.FieldDefs.Add(oDBFieldDef)

		End While
		If Not oDBTableDef Is Nothing Then
			oArray.Add(oDBTableDef)
		End If
		oDbDataReader.Close()
		sComText = "SELECT TableID,IndexNo,IndexName,Fields,Primary,Unique,IgnoreNulls FROM DefaultTableIndexes"

		oDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDbDataReader.HasRows Then
			oDBTableDef = DirectCast(oArray(0), DBTableDef)
			iIndex = 0
			While oDbDataReader.Read()
				If Not oDbDataReader.IsDBNull(3) Then
					iTableID = oDbDataReader.GetInt32(0)
					oDBIndexDef = New DBIndexDef(oDbDataReader.GetString(2))
					oDBIndexDef.FieldList = oDbDataReader.GetString(3)
					oDBIndexDef.PrimaryKey = oDbDataReader.GetBoolean(4)
					oDBIndexDef.Unique = oDbDataReader.GetBoolean(5)

					While iTableID > oDBTableDef.TableID
						iIndex += 1
						oDBTableDef = DirectCast(oArray(iIndex), DBTableDef)
					End While
					oDBTableDef.IndexDefs.Add(oDBIndexDef)
				End If
			End While
			oDbDataReader.Close()
		End If
		Return oArray
	End Function
	Friend Shared Function GetTemplateMDBName() As String
		Return msDBResourceFolder & "\" & [Global].GetTemplateMDBName
	End Function

	Public Function GetResource(ByVal iResourceTheme As enResourceTheme) As TPlResource
		Const iMaxResourceLevel As Integer = 4
		Dim sComText As String = "SELECT ObjectNo,ItemNo,[Level],ResourceString FROM LayoutResource WHERE ResourceTheme=" & CInt(iResourceTheme).ToString() & " ORDER BY ObjectNo,ItemNo DESC"
		'	System.Windows.Forms.MessageBox.Show(sComText, "19_500")
		Dim oDataReader As Common.DbDataReader = GetDataReader(sComText)
		Dim oResource As TPlResource
		Dim oParentResource(iMaxResourceLevel) As TPlResource

		If oDataReader IsNot Nothing Then
			If oDataReader.HasRows Then
				While oDataReader.Read()
					oResource = New TPlResource(iResourceTheme, oDataReader)
					If oResource.Level <= iMaxResourceLevel Then
						oParentResource(oResource.Level) = oResource
					End If
					If oResource.Level > 0 Then
						oParentResource(oResource.Level - 1).AddObject(oResource)
					End If
				End While
			End If
			oDataReader.Close()
			Return oParentResource(0)
		Else
			Return Nothing
		End If
	End Function
	Public Sub New()

	End Sub


#Region "Old Procedures"


#End Region
	Public Shared Function GetListComText(ByVal iListType As enListType, Optional ByVal oKey As System.Object = Nothing, Optional ByVal iaArgs() As Integer = Nothing, Optional ByVal iOptionID As Integer = 1) As String
		Dim sSelect As String = "ID,Name"
		Dim sFrom As String
		Dim sCriteria As String = String.Empty
		Dim sSort As String = String.Empty

		Dim sList As String
		Dim sKeyFieldName As String = "ID"
		Select Case iListType
			Case enListType.Committee
				If Not iaArgs Is Nothing Then
					sCriteria = "(DistrictID=" & CStr(iaArgs(0)) & ")"
				End If
				sSelect = "ID,Name,ShortName,DistrictID,DistrictName"
				sFrom = "CommitteeDistrict"
			Case enListType.Locality
				sCriteria = "Actual"
				If Not iaArgs Is Nothing Then
					sCriteria = " AND (CommitteeID=" & CStr(iaArgs(0)) & ")"
				End If
				sSelect = "ID,Name"
				sFrom = "Localities"
				sSort = "Name"

			Case enListType.BlockType
				sFrom = "t4_BlockTypes"
			Case enListType.Authority
				sFrom = "Auth"
			Case enListType.PlanLevel
				sFrom = "t2_PlanTypes"
			Case enListType.ApprovalLevel
				sFrom = "t3_ApprovalLevels"
			Case enListType.DocumentType
				sFrom = "t5_DocumentTypes"
			Case enListType.InterestType
				sFrom = "t6_InterestTypes"
			Case enListType.Interest
            sKeyFieldName = "InterestID"
				sSelect = "InterestID,LastName + ' ' FirstName"
				sFrom = "zzz"
			Case enListType.PlanValueType
				sFrom = "t7_PlanValueTypes"
			Case enListType.BuildingRightType
				sSelect = "ID,Name,MeasureDflt,CharactDflt,LocationDflt"
				sFrom = "t8_BuildingRightTypes"
			Case enListType.MainLanduse
				sSelect = "ID,Name,0 AS GroupID"
				sFrom = "Landuses"
			Case enListType.LanduseGroup
				sFrom = "LanduseGroups"
			Case enListType.RelationType
				sFrom = "t9_RelationType"
			Case enListType.Measure
				If iaArgs Is Nothing Then
					sFrom = "t10_Measures"
				Else
					sKeyFieldName = "GroupID"
					sSelect = "GroupID,ID"
					sFrom = "Rel_Bldright_Measure"
				End If
			Case enListType.Charact
				If iaArgs Is Nothing Then
					Return "SELECT ID,Name FROM t11_Characts"
				Else
					sKeyFieldName = "GroupID"
					sSelect = "GroupID,ID"
					sFrom = "Rel_Bldright_Charact"
				End If
			Case enListType.Location
				If iaArgs Is Nothing Then
					sFrom = "t12_Locations"
				Else
					sKeyFieldName = "GroupID"
					sSelect = "GroupID,ID"
					sFrom = "Rel_Bldright_Location"
				End If
			Case enListType.GlobalStreet
				If Not iaArgs Is Nothing Then
               '	sList = [Global].JoinInt(iaArgs)
               sList = DMCommon.Functions.JoinInt(iaArgs)
               sCriteria = "LocalityID IN (" & sList & ")"
				End If
				sKeyFieldName = "StreetID"
				sSelect = "StreetID,StreetName,LocalityCode"
				sFrom = "GlobalStreets"
			Case enListType.DWGIfoFormat
				sFrom = "DWGInfoGraphFormat"
			Case enListType.LanduseD
				sSelect = "ID,Name,GroupID"
				sFrom = "LandusesD"
				sCriteria = "FormatID=" & CStr(iOptionID)
			Case enListType.LanduseM
				sSelect = "ID,Name,GroupID"
				sFrom = "Landuses_" & CStr(iOptionID) & "F"
			Case Else

				sSelect = "ID,Text1037"
				sFrom = "GeneralList"
				sCriteria = "(Type = " & CStr(iListType) & ")"
		End Select
		Dim sOutput As String = "SELECT " & sSelect & " FROM " & sFrom
		If Not oKey Is Nothing Then
			If sCriteria.Length <> 0 Then
				sCriteria &= " AND "
			End If
			sCriteria &= "(" & sKeyFieldName & "=" & oKey.ToString & ")"
		End If
		If sCriteria.Length <> 0 Then sOutput &= " WHERE " & sCriteria
		If sSort.Length <> 0 Then sOutput &= " ORDER BY " & sSort
		Return sOutput

	End Function

	Private Sub zzAddLanduseItems(ByRef odicItemList As DMCommon.ItemDataDict)
		Dim oaLanduseGroup As ArrayList = GetItemList(enListType.LanduseGroup)
		'DMObjects.ItemData()
		Dim oItem As New DMCommon.ItemData
		oItem.ListIndex = 81000
		oItem.ListDispData = "aaaaa"

		odicItemList.Add(oItem, 1)		' 1-iGroupKey
	End Sub
	Private Sub zzDistributeItems(ByVal iListType As enListType, ByRef odicItemList As DMCommon.ItemDataDict)
		Dim pOleDbDataReader As System.Data.Common.DbDataReader
		Dim iaArgs() As Integer = {0}
		Dim sComText As String
		Dim iID, iGroupKey As Integer
		'	Dim sKeyFieldName As String
		'	Dim bWhereExists As Boolean
		sComText = GetListComText(iListType, , iaArgs)
		pOleDbDataReader = ServerDB.CurrentServerDB.GetDataReader(sComText)

		While pOleDbDataReader.Read()
			iGroupKey = pOleDbDataReader.GetInt32(0)
			If pOleDbDataReader.IsDBNull(1) Then
				'	odicItemList.Add(iID, iGroupKey)
			Else
				iID = pOleDbDataReader.GetInt32(1)
				odicItemList.AddGroupIndex(iID, iGroupKey)
			End If

		End While
		pOleDbDataReader.Close()
	End Sub
End Class

