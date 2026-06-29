Option Explicit On 
Option Strict On
Public Enum TPlProvider
	ProviderNotDefined
	ProviderJet = 1
	ProviderSQLServer = 2
	ProviderOracle = 3
End Enum
Public Enum enListAddItem As Integer
	Non = 0
	[New] = -1
	All = -2
End Enum
Public Enum enListType As Integer
	BlockType
	BuildRight
	Committee
	Locality
	GlobalStreet
	ProjectDB
	PlanLevel
	PlanValueType
	ApprovalLevel
	DocumentType
	InterestType
	Interest
	Authority
	MainLanduse
	LanduseGroup
	BuildingRightType
	RelationType
	Measure
	Charact
	Location
   DWGIfoFormat
	LanduseD
	LanduseM
	PlanStatus = 256
	Provider = 257
	CorrespondenceType = 258
	PlanUnidiv = 103
	PlanCharacter = 104
	DocumentContentType = 105
	Range = 106
End Enum
Public Enum enResourceTheme
	Undefined = 0
	ObjectCaption = 2
	ObjectShortCaption = 3
	FormAddProject = 11
	ProjectDBItem = 12
	FormMDIMain = 13
	FormProjectList = 14
	FormLocalities = 15
	FormSelectMixedLanduse = 16
	FormDWGIfoDefs = 17

	Message = 201
	'Graphics
	AcRepContent = 301
	AcRepPlanParcels = 302
	AcRepLotsK = 303
	AcRepLotsM = 304

	AcRepParcelLuseK = 305
	AcRepParcelLuseCol = 306
	AcRepParcelLuseM = 306
	AcRepSumLuse = 307
	AcRepJointLuse = 308

	AcRepLotContentAreaByParcel = 309
	AcRepLotContent = 310
	AcRepLotContentArea = 311
	AcRepLotContentAreaByParcel1 = 312
	'AcRepParcels = 311
	AcRepLegendK = 313
	AcRepLegendM = 314
	AcRepLegalParcels = 315
	AcRepLotContentAreaByBlock = 316
	AcRepOwnership = 317
	AcRepOwnersSum = 318
	AcRepExproLuse = 319

	AcRepAAAAAAAAAAAAA = 1999
	AcRepOwners = 331
	AcRepBamashSum = 341
	AcRepBamashMain = 342
	AcRepBamashShared = 343
	AcRepBamashAfricaTitle = 344
	AcRepBamashAfricaData = 345
	AcRepBamashStamp = 346
	AcRepUD_AreaTable = 351


	AcApplication = 400
	AcFrmActionsBase = 401
	AcFrmView = 402
	AcFrmProjectThemes = 403
	AcFrmOwnership = 404



	AcFrmTPlan = 410
	AcFrmTopoMaster = 411
	AcFrmUnidiv = 412
	AcFrmExpro = 413
	AcFrmBamash = 414
	AcFrmEntConnected = 415
	AcFrmExproLotConn = 416

	AcFrmMapThemeBase = 420



End Enum

Public Enum enProjectListSource As Integer
	ProjectDatabase
	SharedResource
	CurrentUser
	LocalMachine
End Enum

Friend NotInheritable Class [Global]
	Public Const Comma As String = ","
	Public Const EmptyString As String = ""
	Public Const AppName As String = "TPlanner"
	Friend Shared Function GetProviderName(ByVal iProvider As TPlProvider) As String
		Select Case iProvider
			Case TPlProvider.ProviderJet
				Return "Microsoft.ACE.OLEDB.12.0" ''''''''''''''''''''''''''''''''''''' "Microsoft.Jet.OLEDB.4.0"
			Case TPlProvider.ProviderSQLServer
            Return Strings.Space(0)
			Case TPlProvider.ProviderOracle
            Return Strings.Space(0)
         Case Else
            Return Strings.Space(0)
      End Select
	End Function
	Friend Shared Function GetConnectionString(ByVal iProvider As TPlProvider, ByVal sDataSource As String, ByVal sSysDB As String) As String
		Dim sProviderName As String
		sProviderName = GetProviderName(iProvider)
		Select Case iProvider
			Case TPlProvider.ProviderJet
				Return "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=0;Data Source=" & sDataSource & ";Jet OLEDB:Engine " & _
				"Type=5;Provider=""" & sProviderName & """;Jet OLEDB:System database=" & sSysDB & ";Jet OLEDB:SFP=False;persist security info=False;Extended Properties=;Mode=Share Deny None;" & _
				"Jet OLEDB:Encrypt Database=False;Jet OLEDB:Create System Database=False;Jet OLEDB:" & _
				"Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;" & _
				"User ID=Admin;Jet OLEDB:Global Bulk Transactions=1"
			Case TPlProvider.ProviderSQLServer
				Return String.Empty
			Case TPlProvider.ProviderOracle
				Return String.Empty
			Case Else
				Return String.Empty
		End Select
	End Function
	Friend Shared Function GetTemplateMDBName() As String
		GetTemplateMDBName = "PrjTemplate.mdb"
	End Function
	Friend Shared Function JoinInt(ByVal iaValue() As Integer) As String
		Dim iIndex As Integer
		Dim iValue As Integer
      Dim sOut As String = String.Empty
		For iIndex = 0 To iaValue.GetUpperBound(0)
			iValue = iaValue(iIndex)
			If sOut.Length <> 0 Then sOut += [Global].Comma
			sOut += iValue.ToString
		Next
		Return sOut
	End Function


End Class

