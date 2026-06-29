Option Explicit On
Option Strict On

Public Class BamashProjectData
	Inherits TopoManager.ProjectData
	Const msAppBuild As String = "2.001"
	Const msLocalityNameKey As String = "LocalityName"
	Const msStreetKey As String = "Street"
	Const msBuildNumKey As String = "BuildNum"

	Const msBlockNoKey As String = "BlockNo"
	Const msParcelKey As String = "Parcel"
	Const msLotKey As String = "Lot"
	Const msTotalAreaKey As String = "TotalArea"
	Const msVersionKey As String = "Version"
	Const msProjectCodeKey As String = "ProjectCode"


	Private msLocalityName As String

	Private msStreet As String
	Private msBuildNum As String

	Private msBlockNo As String
	Private msParcel As String = String.Empty
	Private msLot As String
	Private mdTotalArea As Double
	Private msVersion As String
	Private msProjectCode As String


	Public Overrides Sub OpenData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)
		Dim iIndex As Integer = 0
		Dim iaControlTypes() As enControlType = {enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox, enControlType.TextBox}

		MyBase.OpenDictionary(bCreateValues, bReadOnly)
		If MyBase.Opened Then
			MyBase.SetControlTypes(iaControlTypes)

			msLocalityName = CStrN(MyBase.Item(msLocalityNameKey))
			MyBase.dsaValues(iIndex) = msLocalityName
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msLocalityName)
			iIndex += 1

			msStreet = CStrN(MyBase.Item(msStreetKey))
			MyBase.dsaValues(iIndex) = msStreet
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msStreet)
			iIndex += 1

			msBuildNum = CStrN(MyBase.Item(msBuildNumKey))
			MyBase.dsaValues(iIndex) = msBuildNum
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msBuildNum)
			iIndex += 1


			msBlockNo = CStrN(MyBase.Item(msBlockNoKey))
			MyBase.dsaValues(iIndex) = msBlockNo
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msBlockNo)
			iIndex += 1

			msParcel = modFunctions.CStrN(MyBase.Item(msParcelKey))
			MyBase.dsaValues(iIndex) = msParcel
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msParcel)
			iIndex += 1

			msLot = modFunctions.CStrN(MyBase.Item(msLotKey))
			MyBase.dsaValues(iIndex) = msLot
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msLot)
			iIndex += 1

			mdTotalArea = modFunctions.CDblNth(MyBase.Item(msTotalAreaKey))
			MyBase.dsaValues(iIndex) = CStr(mdTotalArea)
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(mdTotalArea)
			'	MyBase.dmItem(iIndex) = New DMCommon.DMValue(mdTotalArea)
			iIndex += 1

			msVersion = CStrN(MyBase.Item(msVersionKey))
			MyBase.dsaValues(iIndex) = msVersion
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msVersion)
			iIndex += 1

			msProjectCode = modFunctions.CStrN(MyBase.Item(msProjectCodeKey))
			MyBase.dsaValues(iIndex) = msProjectCode
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msProjectCode)
			iIndex += 1
		End If
	End Sub
	Public Property BlockNo() As String
		Get
			Return msBlockNo
		End Get
		Set(ByVal sValue As String)
			msBlockNo = sValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property Parcel() As String
		Get
			Return msParcel
		End Get
		Set(ByVal sValue As String)
			msParcel = sValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property TotalArea() As Double
		Get
			Return mdTotalArea
		End Get
		Set(ByVal dValue As Double)
			mdTotalArea = dValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property Street() As String
		Get
			Return msStreet
		End Get
		Set(ByVal sValue As String)
			msStreet = sValue
			MyBase.dbDirty = True
		End Set
	End Property



	Public Property BuildNum() As String
		Get
			Return msBuildNum
		End Get
		Set(ByVal sValue As String)
			msBuildNum = sValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property LocalityName() As String
		Get
			Return msLocalityName
		End Get
		Set(ByVal sValue As String)
			msLocalityName = sValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Overrides Sub Update()
		Dim iIndex As Integer = 0
		Me.OpenDictionary(False, False)

		msLocalityName = MyBase.dsaValues(iIndex)
		Try
			MyBase.Item(msLocalityNameKey) = msLocalityName
		Catch oEx As Exception
			MessageBox.Show(msLocalityName, "12_230")
		End Try
		iIndex += 1


		msStreet = MyBase.dsaValues(iIndex)
		Try
			MyBase.Item(msStreetKey) = msStreet
		Catch oEx As Exception
			MessageBox.Show(msStreet, "12_231")
		End Try
		iIndex += 1


		msBuildNum = MyBase.dsaValues(iIndex)
		Try
			MyBase.Item(msBuildNumKey) = msBuildNum
		Catch oEx As Exception
			MessageBox.Show(msBuildNum, "12_231a")
		End Try
		iIndex += 1


		msBlockNo = MyBase.dsaValues(iIndex)
		Try
			MyBase.Item(msBlockNoKey) = Convert.ToInt32(msBlockNo)
		Catch oEx As Exception
			MessageBox.Show(msBlockNo, "12_232")
		End Try

		iIndex += 1

		msParcel = MyBase.dsaValues(iIndex)
		MyBase.Item(msParcelKey) = msParcel
		iIndex += 1

		msLot = MyBase.dsaValues(iIndex)
		MyBase.Item(msLotKey) = msLot
		iIndex += 1

		If IsNumeric(MyBase.dsaValues(iIndex)) Then
			mdTotalArea = Convert.ToDouble(MyBase.dsaValues(iIndex))
			MyBase.Item(msTotalAreaKey) = mdTotalArea
		End If
		iIndex += 1

		msVersion = MyBase.dsaValues(iIndex)
		MyBase.Item(msVersionKey) = msVersion
		iIndex += 1

		msProjectCode = MyBase.dsaValues(iIndex)
		MyBase.Item(msProjectCodeKey) = msProjectCode
		iIndex += 1
	End Sub

	Public Sub New()
		MyBase.dsAppBuild = msAppBuild
	End Sub
End Class
