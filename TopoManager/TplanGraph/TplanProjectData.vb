
Option Explicit On
Option Strict On
Imports DMCommon.Functions
Public Class TplanProjectData
	Inherits TopoManager.ProjectData
	Const msAppBuild As String = "3.001"
	Const msLocalityNameKey As String = "LocalityName"
	Const msColorSetIDApprKey As String = "ColorSetIDAppr"
	Const msColorSetIDPropKey As String = "ColorSetIDProp"
	Const msPaintScaleApprKey As String = "PaintScaleAppr"
	Const msPaintScalePropKey As String = "PaintScaleProp"

	 
	Const msVersionKey As String = "Version"
	Const msProjectCodeKey As String = "ProjectCode"
	'	Const msColorSetNameKey As String = "ColorSetName"
	Const msLanduseListApprKey As String = "LanduseListAppr"
	Const msLanduseListPropKey As String = "LanduseListProp"

	Private Name As String = "TplanProjectData"
	Private msLocalityName As String
	Private msLanduseListAppr As String
	Private msLanduseListProp As String
	'Private msBlockNo As String
	Private msParcel As String = String.Empty

	Private msPaintScaleAppr As String
	Private msPaintScaleProp As String

	Private miColorSetIDAppr As Integer
	Private miColorSetIDProp As Integer

	Private msColorSetName As String = String.Empty
	Private msLot As String
	Private msVersion As String
	Private msProjectCode As String


	Public Overrides Sub OpenData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)
		Dim iIndex As Integer = 0
		Dim iaControlTypes() As enControlType = {enControlType.TextBox, enControlType.ComboBoxColorSet, enControlType.TextBox, enControlType.ComboBoxScale, enControlType.ComboBoxColorSet, enControlType.TextBox, enControlType.ComboBoxScale, enControlType.TextBox, enControlType.TextBox}
		MyBase.OpenDictionary(bCreateValues, bReadOnly)
	
		If MyBase.Opened Then
			MyBase.SetControlTypes(iaControlTypes)
			'	MyBase.SetTextBoxUB(6)
			Try
				msLocalityName = CStrN(MyBase.Item(msLocalityNameKey))
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
			MyBase.dsaValues(iIndex) = msLocalityName
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msLocalityName)
			iIndex += 1

			Dim sTest As String = ""
			Try
				miColorSetIDAppr = CType(MyBase.Item(msColorSetIDApprKey), Short)
				MyBase.dsaValues(iIndex) = Convert.ToString(miColorSetIDAppr)
				MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(miColorSetIDAppr)
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, sTest)
			End Try
			iIndex += 1

			Try
				msLanduseListAppr = CStrN(MyBase.Item(msLanduseListApprKey))
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
			MyBase.dsaValues(iIndex) = msLanduseListAppr
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msLanduseListAppr)
			iIndex += 1
			Try
				msPaintScaleAppr = CStrN(MyBase.Item(msPaintScaleApprKey))
				MyBase.dsaValues(iIndex) = Convert.ToString(msPaintScaleAppr)
				MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msPaintScaleAppr)
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, sTest)
			End Try
			iIndex += 1


			Try
				miColorSetIDProp = CType(MyBase.Item(msColorSetIDPropKey), Short)
				MyBase.dsaValues(iIndex) = Convert.ToString(miColorSetIDProp)
				MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(miColorSetIDProp)
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
			iIndex += 1

			Try
				msLanduseListProp = CStrN(MyBase.Item(msLanduseListPropKey))
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, Me.Name)
			End Try
			MyBase.dsaValues(iIndex) = msLanduseListProp
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msLanduseListProp)
			iIndex += 1

			Try
				msPaintScaleProp = CStrN(MyBase.Item(msPaintScalePropKey))
				MyBase.dsaValues(iIndex) = Convert.ToString(msPaintScaleProp)
				MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msPaintScaleProp)
			Catch oEx As Exception
				DMCommon.Functions.ShowEx(oEx, sTest)
			End Try
			iIndex += 1

			msProjectCode = CStrN(MyBase.Item(msProjectCodeKey))
			MyBase.dsaValues(iIndex) = msProjectCode
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msProjectCode)
			iIndex += 1

			msVersion = CStrN(MyBase.Item(msVersionKey))
			MyBase.dsaValues(iIndex) = msVersion
			MyBase.doaDMValues(iIndex) = New DMCommon.DMValue(msVersion)
			iIndex += 1


		End If
	End Sub
	Public ReadOnly Property PaintScale(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
		Get
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					Return msPaintScaleAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					Return msPaintScaleProp
				Case Else
					Return Nothing
			End Select

		End Get


	End Property

	Public Property ColorSetID(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Integer
		Get
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					Return miColorSetIDAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					Return miColorSetIDProp
				Case Else
					Return 0
			End Select

		End Get
		Set(ByVal iValue As Integer)
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					miColorSetIDAppr = iValue
					'	MessageBox.Show(CStr(miColorSetIDAppr), "18_221")
					MyBase.Item(msColorSetIDApprKey) = miColorSetIDAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					miColorSetIDProp = iValue
					MyBase.Item(msColorSetIDPropKey) = miColorSetIDProp
			End Select
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property ColorSetName() As String
		Get
			Return msColorSetName
		End Get
		Set(ByVal sValue As String)
			msColorSetName = sValue
			MyBase.dbDirty = True
		End Set
	End Property
	Public Property LanduseList(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
		Get
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					Return msLanduseListAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					Return msLanduseListProp
				Case Else
					Return String.Empty
			End Select
		End Get
		Set(ByVal sValue As String)
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					msLanduseListAppr = sValue
					Try
						MyBase.Item(msLanduseListApprKey) = msLanduseListAppr
					Catch oEx As Exception
						MessageBox.Show(msLanduseListAppr, "12_230d")
					End Try

				Case DMAcadExt.enTopoPurpose.Proposed
					msLanduseListProp = sValue
					Try
						MyBase.Item(msLanduseListPropKey) = msLanduseListProp
					Catch oEx As Exception
						MessageBox.Show(msLanduseListAppr, "12_230d")
					End Try
			End Select
			MyBase.dbDirty = True
		End Set
	End Property

	Public Overrides Sub Update()
		Dim iIndex As Integer = 0
		Me.OpenDictionary(True, False)

		msLocalityName = MyBase.doaDMValues(iIndex).StrValue
		Try
			MyBase.Item(msLocalityNameKey) = msLocalityName
		Catch oEx As Exception
			MessageBox.Show(msLocalityName, "12_230")
		End Try
		iIndex += 1

		Try
			miColorSetIDAppr = MyBase.doaDMValues(iIndex).IntValue
			MyBase.Item(msColorSetIDApprKey) = miColorSetIDAppr
		Catch oEx As Exception
			MessageBox.Show(msColorSetName, "12_231a")
		End Try
		iIndex += 1


		Try
			msLanduseListAppr = MyBase.doaDMValues(iIndex).StrValue
			MyBase.Item(msLanduseListApprKey) = msLanduseListAppr
		Catch oEx As Exception
			MessageBox.Show(msLanduseListAppr, "12_230d")
		End Try
		iIndex += 1


		Try
			msPaintScaleAppr = MyBase.doaDMValues(iIndex).StrValue
			MyBase.Item(msPaintScaleApprKey) = msPaintScaleAppr
		Catch oEx As Exception
			MessageBox.Show(msPaintScaleAppr, "12_230dx")
		End Try
		iIndex += 1

		Try
			miColorSetIDProp = MyBase.doaDMValues(iIndex).IntValue
			MyBase.Item(msColorSetIDPropKey) = miColorSetIDProp
		Catch oEx As Exception
			MessageBox.Show(MyBase.dsaValues(iIndex), "12_231b")
		End Try
		iIndex += 1


		Try
			msLanduseListProp = MyBase.doaDMValues(iIndex).StrValue
			MyBase.Item(msLanduseListPropKey) = msLanduseListProp
		Catch oEx As Exception
			MessageBox.Show(msLanduseListAppr, "12_230d")
		End Try
		iIndex += 1


		Try
			msPaintScaleProp = MyBase.doaDMValues(iIndex).StrValue
			MyBase.Item(msPaintScalePropKey) = msPaintScaleProp
		Catch oEx As Exception
			MessageBox.Show(msPaintScaleProp, "12_230dx")
		End Try
		iIndex += 1


		msProjectCode = MyBase.doaDMValues(iIndex).StrValue
		MyBase.Item(msProjectCodeKey) = msProjectCode
		iIndex += 1

		msVersion = MyBase.doaDMValues(iIndex).StrValue
		MyBase.Item(msVersionKey) = msVersion
		iIndex += 1
	End Sub

	Public Sub New()
		MyBase.dsAppBuild = msAppBuild
	End Sub
	Public Sub Close()
		MyBase.CloseDictionary()
	End Sub
End Class
