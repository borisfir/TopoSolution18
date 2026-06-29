Option Explicit On
Option Strict On
Imports TopoManager
Public Enum enPropertyTypes
	ZeroType = 0
	ApartType = 1
	BalconyType = 2
	WarehouseType = 3
	SubPrivateType = 4
	SubShareType = 5
	UnderBldTypeColor = 6
	UnderBldTypeWhite = 7
	ExproType = 8
	ZeroFreezeType = 9
	[Default] = -1
	Ignore = -2
End Enum


Public Enum enAttribute
	UserID = 0
	PropID = 1
	SubParcelNo = 2
	PropType = 3
	AprtDescNum = 4
	BldFloor = 5
	BldEntr = 6
	BldPart = 7
	BldNo = 8
	AprtDesc = 9
	AprtDesc2 = 10
	SubPropGroup = 11
	BlockMain = 12
	PolygonColor = 13
	Purchaser = 14
	PropID_Old = 15
	AttributesUB = 15

End Enum
Public Enum enVarColumn
	BldNo
	BldPart
	BldEntr
	BldFloor
	UserID
	VarColumnUB = UserID
End Enum
Public Structure bmPolygonData
	Public PropertyID As Integer
	Public PropertyIDs() As Integer

	Public PropertyType As enPropertyTypes

	Public SubParcelNo As String

	Public SubPropNum As Integer

	Public SubPropCaption As String

	Public AprtDescNum As Integer
	Public ColorIndex As Integer
	Public PolygonColor As Integer
	Public AprtDesc As String

	Public BldFloor As String
	Public BldEntr As Integer?
	Public BldPart As Integer?
	Public BldNo As Integer?

	'Public BldFloorExists As Boolean
	'Public BldEntrEmpty As Boolean
	'Public BldPartEmpty As Boolean
	'Public BldNoEmpty As Boolean
	Public Sub New(iPropertyType As enPropertyTypes)
		PropertyType = iPropertyType
	End Sub
End Structure
Public Class BamashPolygon
	Inherits TPlanGraph.TplnTopoPgon

	Implements System.IComparable(Of bmPropUnitKey)

	Private Const msUserIDAttribTag As String = "USERID"						'0
	Private Const msPropIDAttribTag As String = "PROPID"						'1
	Private Const msSubParcelNoAttribTag As String = "SUBPARCELNO"			'2
	Private Const msPropTypeAttribTag As String = "PROPTYPE"					'3  
	Private Const msAprtDescNumAttribTag As String = "APRTDESCNUM"			'4

	Private Const msBldFloorAttribTag As String = "BLDFLOOR"					'5
	Private Const msBldEntrAttribTag As String = "BLDENTR"					'6
	Private Const msBldPartAttribTag As String = "BLDPART"					'7
	Private Const msBldNoAttribTag As String = "BLDNO"							'8


	Private Const msAprtDescAttribTag As String = "APRTDESC"					'9
	Private Const msAprtDesc2AttribTag As String = "APRTDESC2"				'10

	Private Const msSubPropGroupAttribTag As String = "SubPropGroup"				'11

	Private Const msBlockMainAttribTag As String = "BlockMain"				'12
	Private Const msPolygonColorAttribTag As String = "POLYGONCOLOR"		'13

	Private Const msPurchaserAttribTag As String = "NameP"					'14
	Private Const msPropIDOldAttribTag As String = "NumberPropID_old"	'15

	Private Const msUserIDFieldName As String = "UserID"
	Private Const msPropIDFieldName As String = "PropID"
	Private Const msPropKeyFieldName As String = "PropKey"
	Private Const msPgonKeyFieldName As String = "PgonKey"


	Private Const msSubParcelNoFieldName As String = "SubParcelNo"
	Private Const msCaptionFieldName As String = "Caption"

	Private Const msPropTypeFieldName As String = "PropType"
	Private Const msAprtDescNumFieldName As String = "AprtDescNum"
	Private Const msBldFloorFieldName As String = "BldFloor"
	Private Const msPolygonColorFieldName As String = "PolygonColor"
	Private Const msBldEntrFieldName As String = "BldEntr"

	Private Const msBldPartFieldName As String = "BldPart"
	Private Const msBldNoFieldName As String = "BldNo"

	Private Const msAprtDescFieldName As String = "AprtDesc"
	Private Const msAprtDesc2FieldName As String = "AprtDesc2"
	Private Const msBldFloorDescFieldName As String = "BldFloorDesc"
	Private Const msMainDataFieldName As String = "Main"

	Private Const msPurchaserFieldName As String = "Purchaser"
	Private Const msPropIDOldFieldName As String = "PropID_Old"
	Private Const msPolygonAreaFieldName As String = "PolygonArea"
	Private Const msPgonGroupFieldName As String = "PgonGroup"
	Private Const msGroupBaseFieldName As String = "GroupBase"
	Private Const msGroupAreaFieldName As String = "GroupArea"

	Private Const msChangedFieldName As String = "Changed"
	Private Const msHasValueFieldName As String = "HasValue"


	Private Const miDeafault As Integer = -1


	'Private Shared mbBldNoExists As Boolean
	Private Shared mbBldPartExists As Boolean
	Private Shared mbBldEntrExists As Boolean
	Private Shared mbBldFloorExists As Boolean

	'Private Shared mbUserIDExists As Boolean


	Private Shared mbaVarColumnExists(enVarColumn.VarColumnUB) As Boolean
	Private Shared mbColorUnique As Boolean = True
	Private Shared mbUseNumeration As Boolean = True
	Private Shared miMinPropertyID As Integer
	Private Shared miMaxPropertyID As Integer

	Private Shared miMinSubPropNum As Integer
	Private Shared miMaxSubPropNum As Integer
	Private Shared mhsPropertiesID As HashSet(Of Integer) = New HashSet(Of Integer)()
	Private Shared mhsSubPropNums As HashSet(Of Integer) = New HashSet(Of Integer)()




	Private msUserID As String

	Private msPropertyID As String
	Private miPropertyID As Integer
	Private miaPropertyIDs() As Integer
	Private moNumeration As NumerationPair.Numeration

	Private mtPropUnitKey As bmPropUnitKey
	Private miPropertyType As enPropertyTypes = enPropertyTypes.Default
	Private mbPropertyTypeExists As Boolean
	Private msSubParcelNoInput As String
	Private msSubParcelNo As String
	Private mbSubPropConst As Boolean
	Private miSubPropNum As Integer
	Private miAprtDescNum As Integer


	Private msSubPropCaption As String
	Private msSubPropCaptionToAcad As String




	'Private miColorIndex As Integer = 0
	Private miPolygonColor As Integer = -1 '-1 - nothing;  -2 - ignore

	Private miaPolygonColors() As Integer
	Private mdicPolygonColors As IDictionary(Of Integer, Integer)
	Private msBldFloor As String
	Private miBldEntr As Integer?
	Private miBldPart As Integer?
	Private miBldNo As Integer?
	Private miBldFloor As Integer?
	Private miBldSubFloor As Integer?

	Private msAprtDesc As String
	Private msAprtDescDOS As String

	Private msAprtDesc2 As String = String.Empty
	Private msAprtDesc2DOS As String = String.Empty
	Private msAprtDesc2Table As String = String.Empty
	Private msAprtDesc2Excel As String = String.Empty


	Private msBldFloorDesc As String

	Private mbMainData As Boolean = False
	Private miPolygonID As Integer
	Private msPurchaser As String
	Private miPropID_Old As Integer
	Private miGroupID As Integer
	Private mbIsGroupBase As Boolean
	Private mdGroupArea As Double

	'Private mdPolygonArea As Double
	Private mbIsSubProp As Boolean = False
	Private mbIsProperty As Boolean = False
	Private miTableRowIndex As Integer
	Private Shared miAreaDigits As Integer = 2
	Private Shared moMainDataTable As System.Data.DataTable
	'---> To  TplnTopoPgon
	Private Shared msaBlockAttribTag() As String
	Private Shared miaInputBlockAttribIndex(enAttribute.AttributesUB) As Integer
	'	Private Shared miaInputBlockAttribTag(miAttributesUB) As String
	Private mdAdditionalArea As Double = 0.0
	Private Shared miaUpdateBlockAttribIndex(2) As Integer

	'Protected dbCorrect As Boolean = True
	Private Shared msCentroidBlockName As String
	Private Shared mbNumberingSeparately As Boolean

	Private mtColorScheme As DMAcadExt.ColorScheme
	'	Private Shared miTestCounter As Integer
	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon, True)
		If mbColorUnique Then
			mdicPolygonColors = New Dictionary(Of Integer, Integer)
		End If

		'	Dim saDebugTag() As String = {"PROPID", "BlockMain"}
		'		Dim saDebugOutput() As String
		If miaInputBlockAttribIndex IsNot Nothing Then

			Try
				dsaBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, True, dbAcadPoint, miaInputBlockAttribIndex)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Pgon", MyBase.diTopoID, MyBase.dtCentroidAcObjID, dbAcadPoint)
				If dsaBlockAttribText Is Nothing Then
					DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "Block attributes are not valid", True)

					dbCorrect = False
				Else
					'	saDebugOutput = DMAcadExt.AcadTransaction.GetAttribText(MyBase.diCentroidAcObjID, True, dbAcadPoint, saDebugTag)
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!BlockAttribText", dsaBlockAttribText)
					zzSetData(oPolygon.Centroid)

				End If
				' zzTestStrArray(dsaBlockAttribText)
			Catch oEx As Exception
				TopoManager.TPlanGraph.TplnProject.WriteMessageBox("PgonID=" & Me.TopoID.ToString & ":" & oEx.Message & "," & oEx.StackTrace, "BamashPolygon - New")
			End Try
		Else
			MessageBox.Show("BlockAttribIndex Is Nothing", "12_411")
		End If
	End Sub
	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon, iPropertyID As Integer, iPropertyType As enPropertyTypes, iPolygonColor As Integer, bMainData As Boolean)
		MyBase.New(oPolygon, True)

		miPropertyID = iPropertyID
		miPropertyType = iPropertyType
		miPolygonColor = iPolygonColor
		mbMainData = bMainData

		' msBldFloor As String
		' miBldEntr As Integer
		' miBldPart As Integer
		' miBldNo As Integer
		' miBldFloor As Integer = 0
		' miBldSubFloor As Integer = 0





		'	Dim saDebugTag() As String = {"PROPID", "BlockMain"}
		'		Dim saDebugOutput() As String
		If miaInputBlockAttribIndex IsNot Nothing Then

			Try
				dsaBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, True, dbAcadPoint, miaInputBlockAttribIndex)
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Pgon", MyBase.diTopoID, MyBase.dtCentroidAcObjID, dbAcadPoint)
				If dsaBlockAttribText Is Nothing Then
					'
				End If
				' zzTestStrArray(dsaBlockAttribText)
			Catch oEx As Exception
				TopoManager.TPlanGraph.TplnProject.WriteMessageBox("PgonID=" & Me.TopoID.ToString & ":" & oEx.Message & "," & oEx.StackTrace, "BamashPolygon - New")
			End Try
		Else
			MessageBox.Show("BlockAttribIndex Is Nothing", "12_411")
		End If
	End Sub
	Public Shared Property NumberingSeparately As Boolean
		Get
			Return mbNumberingSeparately
		End Get
		Set(bValue As Boolean)
			mbNumberingSeparately = bValue
		End Set
	End Property



	Public Shared Function GetAttributeTag(ByVal iIndex As Integer) As String

		Dim iAttribUB As Integer = miaInputBlockAttribIndex.GetUpperBound(0)
		Dim iIndex1 As Integer
		If iIndex <= iAttribUB Then
			iIndex1 = miaInputBlockAttribIndex(iIndex)
			Dim iAttribUB1 As Integer = msaBlockAttribTag.GetUpperBound(0)
			If iIndex1 <= iAttribUB1 Then
				Return msaBlockAttribTag(iIndex1)
			End If

		End If
		Return String.Empty

	End Function
	Public Shared Sub Reset()
		miMinPropertyID = 0
		miMaxPropertyID = 0
		miMinSubPropNum = 0
		miMaxSubPropNum = 0
		If mhsPropertiesID Is Nothing Then
			mhsPropertiesID = New HashSet(Of Integer)()

		Else
			mhsPropertiesID.Clear()
		End If
		If mhsSubPropNums Is Nothing Then
			mhsSubPropNums = New HashSet(Of Integer)()

		Else
			mhsSubPropNums.Clear()
		End If

	End Sub
	Public Shared Function CheckPropertyID(iPropertyID As Integer) As Integer
		If mhsPropertiesID IsNot Nothing Then
			Do
				If mhsPropertiesID.Contains(iPropertyID) Then
					iPropertyID += 1
				Else
					Exit Do
				End If
			Loop
		End If

		Return iPropertyID
	End Function
	Public Shared Function CheckSubpropNum(iSubpropNum As Integer) As Integer
		Do
			If mhsSubPropNums.Contains(iSubpropNum) Then
				iSubpropNum += 1
			Else
				Exit Do
			End If
		Loop
		Return iSubpropNum
	End Function
	Public Shared ReadOnly Property VarColumnExists As Boolean()
		Get
			Return mbaVarColumnExists
		End Get
	End Property
	Public Shared ReadOnly Property MinPropertyID As Integer
		Get
			Return miMinPropertyID
		End Get
	End Property
	Public Shared ReadOnly Property MaxPropertyID As Integer
		Get
			Return miMaxPropertyID
		End Get
	End Property
	Public Shared ReadOnly Property MinSubPropNum As Integer
		Get
			Return miMinSubPropNum
		End Get
	End Property
	Public Shared ReadOnly Property MaxSubPropNum As Integer
		Get
			Return miMaxSubPropNum
		End Get
	End Property
	Public Shared Function GetEmptyColumns() As Integer()
		Dim iaEmptyColumns(enVarColumn.VarColumnUB) As Integer
		Dim iResUB As Integer = -1
		For iIndex As Integer = 0 To enVarColumn.VarColumnUB
			iaEmptyColumns(iIndex) = -1
		Next
		Try

			Dim iaVarColumnsFieldNum() As Integer = bmProperty.GetVarColumnsFieldNum()
			For iIndex As Integer = 0 To enVarColumn.VarColumnUB
				If Not mbaVarColumnExists(iIndex) Then
					iResUB += 1
					iaEmptyColumns(iResUB) = iaVarColumnsFieldNum(iIndex)
				End If
			Next
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaVarColumnsFieldNum", iaVarColumnsFieldNum)
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "mbaVarColumnExists", mbaVarColumnExists)

		Catch ex As Exception

		End Try





		If iResUB = -1 Then
			Return Nothing
		Else
			ReDim Preserve iaEmptyColumns(iResUB)
			DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaEmptyColumns", iaEmptyColumns)
			Return iaEmptyColumns
		End If

	End Function

	Public ReadOnly Property ColorScheme() As DMAcadExt.ColorScheme
		Get
			Return mtColorScheme
		End Get

	End Property
	Public Shared ReadOnly Property PropTypeFieldName As String
		Get
			Return msPropTypeFieldName
		End Get
	End Property

	Public Shared ReadOnly Property PropIDFieldName As String
		Get
			Return msPropIDFieldName
		End Get
	End Property
	Public Shared ReadOnly Property MainDataFieldName As String
		Get
			Return msMainDataFieldName
		End Get
	End Property

	Public Shared ReadOnly Property PropKeyFieldName As String
		Get
			Return msPropKeyFieldName
		End Get
	End Property
	Public Shared ReadOnly Property PgonKeyFieldName As String
		Get
			Return msPgonKeyFieldName
		End Get
	End Property
	Public Shared ReadOnly Property SubParcelNoFieldName As String
		Get
			Return msSubParcelNoFieldName
		End Get
	End Property

	Public Shared ReadOnly Property PolygonColorFieldName As String
		Get
			Return msPolygonColorFieldName
		End Get
	End Property

	Public ReadOnly Property ColorIndex() As Integer
		Get
			Return miPolygonColor
		End Get

	End Property

	Public Property SubParcelNo() As String
		Get
			Return msSubParcelNo
		End Get
		Set(ByVal sValue As String)
			msSubParcelNo = sValue
		End Set
	End Property
	Public ReadOnly Property CaptionDOS() As String
		Get
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!CaptionDOS", miPropertyType, msPropertyID, miSubPropNum, GetCaption(miPropertyType, msPropertyID, miSubPropNum), mbIsSubProp, DMCommon.Hebrew.WordToDOS(Me.Caption, False), msSubPropCaption)
			If mbIsSubProp Then
				Return DMCommon.Hebrew.WordToDOS(Me.Caption, False)
			Else
				Return msSubPropCaption
				Return msSubPropCaptionToAcad
			End If
		End Get
	End Property
	Public ReadOnly Property Caption() As String
		Get
			'Return miPropertyType.ToString() & "," & msPropertyID.ToString() & "," & miSubPropNum.ToString()
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Caption", miPropertyType, msPropertyID, miSubPropNum, msSubParcelNo, GetCaption(miPropertyType, msPropertyID, miSubPropNum))
			If mbSubPropConst Then
				Return msSubPropCaption
			Else
				Return GetCaption(miPropertyType, msPropertyID, miSubPropNum)
			End If



		End Get
	End Property

	Public Property SubPropCaption() As String
		Get
			Return msSubPropCaption
		End Get
		Set(sValue As String)
			msSubPropCaption = sValue
		End Set
	End Property
	Public ReadOnly Property CaptionUNICODE() As String
		Get
			If mbIsSubProp Then
				Return DMCommon.Hebrew.WordToUnicode(Me.Caption)
			Else
				Return msSubPropCaption
			End If
		End Get
	End Property
	Public Shared Function GetCaption(iPropertyType As enPropertyTypes, sPropID As String, iSubPropNum As Integer) As String
		Dim bIsProperty, bIsSubProp As Boolean
		Dim sSubPropCaption As String
		zzGetPropertyType(iPropertyType, bIsProperty, bIsSubProp)

		If bIsProperty Then
			If bIsSubProp Then
				If iSubPropNum = 0 Then

					Return String.Empty
				Else
					If mbNumberingSeparately Then
						'	If CInt(sPropID) Mod 2 = 0 Then
						'		Return DMCommon.Hebrew.Invert(sPropID) & " " & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
						'	Else
						'		Return sPropID & " " & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
						'	End If

						'	If CInt(sPropID) Mod 2 = 0 Then
						'Return DMCommon.Hebrew.InvertTest(CInt(sPropID), True) & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
						'	Return sPropID & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
						'Else
						'Return DMCommon.Hebrew.InvertInt(CInt(sPropID)) & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
						'End If
						Return DMCommon.Hebrew.InvertInt(CInt(sPropID)) & DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
					Else
						Return DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
					End If

				End If
			Else
				Return CStr(sPropID)
			End If
		Else
			Return String.Empty
		End If
	End Function
	Public Shared Function GetCaption_210425(iPropertyType As enPropertyTypes, sPropID As String, iSubPropNum As Integer) As String
		Dim bIsProperty, bIsSubProp As Boolean
		zzGetPropertyType(iPropertyType, bIsProperty, bIsSubProp)

		If bIsProperty Then
			If bIsSubProp Then
				If iSubPropNum = 0 Then

					Return String.Empty
				Else
					Return DMCommon.Hebrew.GetHebNum(iSubPropNum, True)
				End If
			Else
				Return CStr(sPropID)
			End If
		Else
			Return String.Empty
		End If
	End Function
	Public ReadOnly Property CaptionOld() As String
		Get

			If mbIsProperty Then
				If mbIsSubProp Then
					If miSubPropNum = 0 Then
						Return String.Empty
					Else
						Return DMCommon.Hebrew.GetHebNum(miSubPropNum, True)
					End If
				Else
					Return CStr(miPropertyID)
				End If
			Else
				Return String.Empty
			End If
		End Get
	End Property
	Public Property AprtDescNum() As Integer
		Get
			Return miAprtDescNum
		End Get
		Set(ByVal iValue As Integer)
			miAprtDescNum = iValue
		End Set
	End Property

	Public Property BldFloor() As String
		Get
			Return msBldFloor
		End Get
		Set(ByVal sValue As String)
			msBldFloor = sValue
		End Set
	End Property


	Public Property BldEntr() As Integer?
		Get
			Return miBldEntr
		End Get
		Set(ByVal iValue As Integer?)
			miBldEntr = iValue
		End Set
	End Property


	Public Property BldPart() As Integer?
		Get
			Return miBldPart
		End Get
		Set(ByVal iValue As Integer?)
			miBldPart = iValue
		End Set
	End Property

	Public Property AdditionalArea() As Double
		Get
			Return mdAdditionalArea
		End Get
		Set(ByVal dValue As Double)
			mdAdditionalArea = dValue
		End Set
	End Property

	Public ReadOnly Property Area() As Double
		Get
         Return Math.Round(MyBase.ddAcadArea + mdAdditionalArea, 2, MidpointRounding.AwayFromZero)
		End Get
	End Property
	Public ReadOnly Property ExtArea() As Double
		Get
			If mbIsGroupBase Then
				Return Math.Round(mdGroupArea + mdAdditionalArea, 2, MidpointRounding.AwayFromZero)
			Else
				Return Me.Area
			End If

		End Get
	End Property
	Public Property GroupArea() As Double
		Get
			Return Math.Round(mdGroupArea, 2, MidpointRounding.AwayFromZero)
		End Get
		Set(dValue As Double)
			mdGroupArea = dValue
		End Set
	End Property
	Public Sub SetGroupData(oGroup As bmPgonGroup)
		If IsGroupBase Then
			mdGroupArea = oGroup.Area
		Else
			miSubPropNum = oGroup.SubPropNum
			If miSubPropNum <> 0 Then
				mbSubPropConst = False
			End If
			msSubPropCaption = oGroup.SubPropCaption
		End If

	End Sub

	Public ReadOnly Property FormatArea() As String
		Get
			Return FormatNumber(Me.Area, 1, TriState.True)
		End Get
	End Property


	Public Property BldNo() As Integer?
		Get
			Return miBldNo
		End Get
		Set(ByVal iValue As Integer?)
			miBldNo = iValue
		End Set
	End Property
	Public Property SubPropNum() As Integer
		Get
			Return miSubPropNum
		End Get
		Set(ByVal iValue As Integer)
			miSubPropNum = iValue
			If miSubPropNum <> 0 Then
				msSubPropCaption = DMCommon.Hebrew.GetHebNum(miSubPropNum, True)
				mbSubPropConst = False
			End If

		End Set

	End Property
	Public Sub CheckSubPropNum()
		If mbIsSubProp AndAlso String.IsNullOrEmpty(msSubParcelNo) Then
			Dim sMsg As String = "מספר לא נמצא"
			DMAcadExt.AppMessages.AddMessage(True, MyBase.ddCentroidX, MyBase.ddCentroidY, "", sMsg, False)
		End If
	End Sub
	Public ReadOnly Property CalcArea() As Double
		Get
			Return Math.Round(Me.Area, miAreaDigits, MidpointRounding.AwayFromZero)
		End Get
	End Property

	Public Property PolygonID() As Integer
		Get
			Return miPolygonID
		End Get
		Set(ByVal iValue As Integer)
			miPolygonID = iValue
		End Set
	End Property
	Public Property SharedPropertyID() As String
		Get
			If moNumeration IsNot Nothing Then
				Return moNumeration.GetPresentation()
			Else
				Return msPropertyID
			End If

		End Get
		Set(ByVal sValue As String)
			msPropertyID = sValue
		End Set
	End Property
	Public ReadOnly Property PropertyID(ByVal iIndex As Integer) As Integer
		Get
			If miaPropertyIDs IsNot Nothing Then
				Return miaPropertyIDs(iIndex)
			End If
		End Get

	End Property
	Public ReadOnly Property PropertiesUB() As Integer
		Get
			If miaPropertyIDs Is Nothing Then
				Return -1
			Else
				Return miaPropertyIDs.GetUpperBound(0)
			End If
		End Get
	End Property
	Public ReadOnly Property PropertyID() As Integer
		Get
			Return miPropertyID
		End Get

	End Property
	Public ReadOnly Property PropertyType() As enPropertyTypes
		Get
			Return miPropertyType
		End Get

	End Property
	Public ReadOnly Property GroupID() As Integer
		Get
			Return miGroupID
		End Get

	End Property
	Public Property PolygonColor() As Integer
		Get
			Return miPolygonColor
		End Get
		Set(ByVal iValue As Integer)
			miPolygonColor = iValue
		End Set
	End Property

	Public Sub SetMainData(iPropertyID As Integer, iPropertyType As enPropertyTypes, iSubPropNum As Integer, iAprtDescNum As Integer, iPolygonColor As Integer)
		If iPropertyID > -1 Then
			miPropertyID = iPropertyID
		End If

		If miPropertyID > 0 Then
			msPropertyID = miPropertyID.ToString()
		Else
			msPropertyID = String.Empty
		End If

		If iPropertyType <> enPropertyTypes.Ignore Then
			miPropertyType = iPropertyType
		End If

		If iSubPropNum > -1 Then
			miSubPropNum = iSubPropNum
		End If
		If iAprtDescNum > -1 Then
			miAprtDescNum = iAprtDescNum
		End If
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SetMainD", Me.PropertyID, Me.SubPropNum, Me.AprtDescNum, miSubPropNum, iSubPropNum)
		If miPolygonColor > -1 Then
			miPolygonColor = iPolygonColor
		End If

		If miPropertyType = enPropertyTypes.ApartType Then
			mbMainData = True
		Else
			mbMainData = False
		End If

		zzAfterInput(False, False)

	End Sub
	Public Sub SetAllData(tBamashData As bmPolygonData)
		If tBamashData.PropertyID > 0 Then
			miPropertyID = tBamashData.PropertyID
			msPropertyID = miPropertyID.ToString()
		End If

		If tBamashData.PropertyType <> enPropertyTypes.Default Then
			miPropertyType = tBamashData.PropertyType
		End If
		If tBamashData.SubPropNum > 0 Then
			miSubPropNum = tBamashData.SubPropNum
		End If
		If tBamashData.PolygonColor > 0 Then
			miPolygonColor = tBamashData.PolygonColor
		End If
		If tBamashData.AprtDescNum > 0 Then
			miAprtDescNum = tBamashData.AprtDescNum
		End If

		If miPropertyType = enPropertyTypes.ApartType Then
			mbMainData = True
		ElseIf miPropertyType <> enPropertyTypes.Default Then
			mbMainData = False
		End If



		zzAfterInput(False, False)

	End Sub
	Public Sub SetAllData(Optional iPropertyID As Integer = miDeafault, Optional iPropertyType As enPropertyTypes = enPropertyTypes.Default, Optional iSubpropNum As Integer = miDeafault, Optional iPolygonColor As Integer = miDeafault)
		If iPropertyID <> miDeafault Then
			miPropertyID = iPropertyID
			msPropertyID = miPropertyID.ToString()
		End If
		If iPropertyType <> enPropertyTypes.Default Then
			miPropertyType = iPropertyType
		End If
		If iSubpropNum <> miDeafault Then
			miSubPropNum = iSubpropNum
		End If
		If iPolygonColor <> miDeafault Then
			miPolygonColor = iPolygonColor
		End If


		If miPropertyType = enPropertyTypes.ApartType Then
			mbMainData = True
		ElseIf miPropertyType <> enPropertyTypes.Default Then
			mbMainData = False
		End If



		zzAfterInput(False, False)

	End Sub

	Public Sub AddPolygonColor(ByVal iColorIndex As Integer)
		If Not mdicPolygonColors.ContainsKey(iColorIndex) Then
			mdicPolygonColors.Add(iColorIndex, 0)
		End If

	End Sub
	Public Property PolygonColor(ByVal iIndex As Integer) As Integer
		Get
			Return miaPolygonColors(iIndex)
		End Get
		Set(ByVal iValue As Integer)
			miaPolygonColors(iIndex) = iValue
		End Set
	End Property
	Public Sub SetColor(ByVal iColorIndex As Integer)
		If mbColorUnique Then
			AddPolygonColor(iColorIndex)
		Else
			Me.miPolygonColor = iColorIndex
		End If
	End Sub
	Public Property UserID() As String
		Get
			Return msUserID
		End Get
		Set(ByVal sValue As String)
			msUserID = sValue
		End Set
	End Property
	Public Property ApartDesc() As String
		Get
			Return msAprtDesc
		End Get
		Set(ByVal sValue As String)
			msAprtDesc = sValue
		End Set
	End Property
	Public Property ApartDescDOS() As String
		Get
			Return msAprtDescDOS
		End Get
		Set(ByVal sValue As String)
			msAprtDescDOS = sValue
		End Set
	End Property
	Public ReadOnly Property ApartDescRep() As String
		Get
			Return "+" & msAprtDesc & " " & msAprtDesc2Table
		End Get

	End Property
	Public ReadOnly Property ApartDescExcel() As String
		Get
			Return "+" & msAprtDesc & " " & msAprtDesc2Excel
		End Get

	End Property
	Public Property BldFloorDesc() As String
		Get
			Return msBldFloorDesc
		End Get
		Set(ByVal sValue As String)
			msBldFloorDesc = sValue
		End Set
	End Property
	Public ReadOnly Property PropUnitKey() As bmPropUnitKey
		Get
			Return mtPropUnitKey
		End Get
	End Property
	Public ReadOnly Property IsGroupBase As Boolean
		Get

			Return mbIsGroupBase
		End Get
	End Property
	Public ReadOnly Property IsGroupNotBase As Boolean
		Get
			Return (miGroupID <> 0) AndAlso (Not mbIsGroupBase)
		End Get
	End Property

	Private Sub zzSetData_300123(tCentroid As Autodesk.AutoCAD.Geometry.Point3d)
		Dim sMsg As String
		Dim iAttribIndex As Integer = 0
		Dim iAttribUB As Integer = -1
		Dim sAttribText As String

		Try
			If dsaBlockAttribText IsNot Nothing Then
				iAttribUB = Me.dsaBlockAttribText.GetUpperBound(0)
			End If
		Catch oEx As System.Exception
			sMsg = "Bamash - zzSetData_" & "Start: " & oEx.Message
			DMAcadExt.AcadDocument.WriteMessage(sMsg)

		End Try




		If iAttribUB >= iAttribIndex Then  '0
			Try
				msUserID = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(msUserID) Then
					mbaVarColumnExists(enVarColumn.UserID) = True
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '1
			Try
				msPropertyID = Strings.Trim(Me.dsaBlockAttribText(iAttribIndex))

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '2
			Try
				msSubParcelNoInput = Me.dsaBlockAttribText(iAttribIndex)
				If msSubParcelNoInput IsNot Nothing Then
					If msSubParcelNoInput.Length = 0 OrElse IsNumeric(msSubParcelNoInput) Then
						msSubParcelNo = String.Empty

					Else
						' msSubParcelNo = DMCommon.Hebrew.ToUnicode(msSubParcelNoInput)
						' msSubParcelNoInput 'DMCommon.Hebrew.FromDOS(msSubParcelNoInput, False)
						msSubParcelNo = DMCommon.Hebrew.FromAcad(msSubParcelNoInput)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!msPropID", msSubParcelNoInput, msPropertyID, msSubParcelNo)
					End If
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If
		If iAttribUB >= iAttribIndex Then '3
			Try
				Dim sPropertyType As String = Me.dsaBlockAttribText(iAttribIndex)
				If String.IsNullOrEmpty(sPropertyType) Then
					mbPropertyTypeExists = False
				ElseIf [Enum].TryParse(Of enPropertyTypes)(sPropertyType, miPropertyType) Then
					mbPropertyTypeExists = True
				Else
					'miPropertyType = CType(Convert.ToInt32(sPropertyType), enPropertyTypes)
					mbPropertyTypeExists = False
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If


		If iAttribUB >= iAttribIndex Then '4
			Try
				sAttribText = Me.dsaBlockAttribText(iAttribIndex)
				If sAttribText IsNot Nothing Then
					miAprtDescNum = bmBamash.Text2Int(sAttribText, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If

		If iAttribUB >= iAttribIndex Then '5
			Try
				msBldFloor = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(msBldFloor) Then
					mbaVarColumnExists(enVarColumn.BldFloor) = True
					mbBldFloorExists = True
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '6
			Try
				Dim sBldEntr As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldEntr) Then
					miBldEntr = bmBamash.Text2Int(sBldEntr, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldEntr <> 0 Then
						mbaVarColumnExists(enVarColumn.BldEntr) = True
						'mbBldEntrExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '7
			Try
				Dim sBldPart As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldPart) Then
					miBldPart = bmBamash.Text2Int(sBldPart, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldPart <> 0 Then
						mbaVarColumnExists(enVarColumn.BldPart) = True
						'mbBldPartExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '8
			Try
				Dim sBldNo As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldNo) Then
					miBldNo = bmBamash.Text2Int(sBldNo, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldNo <> 0 Then
						mbaVarColumnExists(enVarColumn.BldNo) = True
						'mbBldNoExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '9
			Try
				msAprtDescDOS = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try

			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '10
			Try
				msAprtDesc2DOS = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				msAprtDesc2 = DMCommon.Hebrew.FromDOS(msAprtDesc2DOS, True)
				'msAprtDesc2 = DMCommon.Hebrew.WordToDOS(Me.dsaBlockAttribText(iAttribIndex), False)
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '11
			Try
				Dim sGroupID As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				If sGroupID.Length = 0 Then
					miGroupID = 0
				Else
					zzParseGroup(sGroupID)
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '12
			Dim sMainData As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
			'		DMAcadExt.AcadDocument.WriteMessage("^24 :" & msPropertyID & ":" & sMainData & ":" & CStr(iAttribIndex))
			'		DMAcadExt.AcadDocument.WriteMessage("^|" & DMCommon.Functions.DispArray(Me.dsaBlockAttribText, "", False, "|"))
			'	Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(ddCentroidX, ddCentroidX, 0.0)
			If sMainData = "1" Then
				mbMainData = True
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "L1")
			ElseIf sMainData = "0" Then
				mbMainData = False
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "L0")
			Else
				mbMainData = False
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "LEr")
				''Error
			End If

			iAttribIndex += 1
		End If
		If mbMainData Then
			If iAttribUB >= iAttribIndex Then '13
				Try
					Dim sPolygonColor As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
					If Not String.IsNullOrEmpty(sPolygonColor) Then
						miPolygonColor = bmBamash.Text2Int(sPolygonColor, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					End If
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
			If iAttribUB >= iAttribIndex Then '14
				Try
					msPurchaser = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
			If iAttribUB >= iAttribIndex Then '15
				Try
					Dim sPropID_Old As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
					If sPropID_Old.Length = 0 Then
						miPropID_Old = 0
					Else
						miPropID_Old = bmBamash.Text2Int(sPropID_Old, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					End If
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
		End If

		zzAfterInput(True, False) '***
	End Sub


	Private Sub zzSetData(tCentroid As Autodesk.AutoCAD.Geometry.Point3d)
		Dim sMsg As String
		Dim iAttribIndex As Integer = 0
		Dim iAttribUB As Integer = -1
		Dim sAttribText As String

		Try
			If dsaBlockAttribText IsNot Nothing Then
				iAttribUB = Me.dsaBlockAttribText.GetUpperBound(0)
			End If
		Catch oEx As System.Exception
			sMsg = "Bamash - zzSetData_" & "Start: " & oEx.Message
			DMAcadExt.AcadDocument.WriteMessage(sMsg)
		End Try

		If iAttribUB >= iAttribIndex Then  '0
			Try
				msUserID = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(msUserID) Then
					mbaVarColumnExists(enVarColumn.UserID) = True
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '1
			Try
				msPropertyID = Strings.Trim(Me.dsaBlockAttribText(iAttribIndex))
				
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '2
			Try
				'hhhhhhhhhhhh
				msSubParcelNoInput = Strings.Trim(Me.dsaBlockAttribText(iAttribIndex))

				If msSubParcelNoInput IsNot Nothing Then
					If msSubParcelNoInput.Length = 0 OrElse IsNumeric(msSubParcelNoInput) Then
						msSubParcelNo = String.Empty

					Else
						' msSubParcelNo = DMCommon.Hebrew.ToUnicode(msSubParcelNoInput)
						' msSubParcelNoInput 'DMCommon.Hebrew.FromDOS(msSubParcelNoInput, False)
						If mbNumberingSeparately Then
							Dim sHebPart As String = ""

							If False Then


								If msSubParcelNoInput.StartsWith(msPropertyID) Then
									sHebPart = msSubParcelNoInput.Substring(msPropertyID.Length)

									msSubParcelNo = DMCommon.Hebrew.FromAcad(sHebPart)

								End If
							End If
							If msSubParcelNoInput.EndsWith(msPropertyID) Then
								sHebPart = msSubParcelNoInput.Substring(0, msSubParcelNoInput.Length - msPropertyID.Length)

								msSubParcelNo = DMCommon.Hebrew.FromAcad(sHebPart)

							End If


							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SetData", msSubParcelNoInput, msSubParcelNo, sHebPart, sHebPart.Length, msSubParcelNoInput.StartsWith(msPropertyID), msSubParcelNoInput.EndsWith(msPropertyID))
						Else
							msSubParcelNo = DMCommon.Hebrew.FromAcad(msSubParcelNoInput)
						End If


						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!msPropID", msSubParcelNoInput, msPropertyID, msSubParcelNo)
					End If
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If
		If iAttribUB >= iAttribIndex Then '3
			Try
				Dim sPropertyType As String = Me.dsaBlockAttribText(iAttribIndex)
				If String.IsNullOrEmpty(sPropertyType) Then
					mbPropertyTypeExists = False
				ElseIf [Enum].TryParse(Of enPropertyTypes)(sPropertyType, miPropertyType) Then
					mbPropertyTypeExists = True
				Else
					'miPropertyType = CType(Convert.ToInt32(sPropertyType), enPropertyTypes)
					mbPropertyTypeExists = False
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If

		If miPropertyType = enPropertyTypes.SubShareType Then

			'	DMCommon.Debug.MsgBox("!New", miPropertyID, miPropertyType, msSubParcelNo, mbIsProperty, mbUseNumeration, msPropertyID)
		End If

		If iAttribUB >= iAttribIndex Then '4
			Try
				sAttribText = Me.dsaBlockAttribText(iAttribIndex)
				If sAttribText IsNot Nothing Then
					miAprtDescNum = bmBamash.Text2Int(sAttribText, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			Finally
				iAttribIndex += 1
			End Try
		End If

		If iAttribUB >= iAttribIndex Then '5
			Try
				msBldFloor = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(msBldFloor) Then
					mbaVarColumnExists(enVarColumn.BldFloor) = True
					mbBldFloorExists = True
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '6
			Try
				Dim sBldEntr As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldEntr) Then
					miBldEntr = bmBamash.Text2Int(sBldEntr, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldEntr <> 0 Then
						mbaVarColumnExists(enVarColumn.BldEntr) = True
						'mbBldEntrExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '7
			Try
				Dim sBldPart As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldPart) Then
					miBldPart = bmBamash.Text2Int(sBldPart, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldPart <> 0 Then
						mbaVarColumnExists(enVarColumn.BldPart) = True
						'mbBldPartExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '8
			Try
				Dim sBldNo As String = Me.dsaBlockAttribText(iAttribIndex)
				If Not String.IsNullOrEmpty(sBldNo) Then
					miBldNo = bmBamash.Text2Int(sBldNo, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					If miBldNo <> 0 Then
						mbaVarColumnExists(enVarColumn.BldNo) = True
						'mbBldNoExists = True
					End If
				End If

			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '9
			Try
				msAprtDescDOS = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try

			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '10
			Try
				msAprtDesc2DOS = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				msAprtDesc2 = DMCommon.Hebrew.FromDOS(msAprtDesc2DOS, True)
				'msAprtDesc2 = DMCommon.Hebrew.WordToDOS(Me.dsaBlockAttribText(iAttribIndex), False)
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If

		If iAttribUB >= iAttribIndex Then '11
			Try
				Dim sGroupID As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				If sGroupID.Length = 0 Then
					miGroupID = 0
				Else
					zzParseGroup(sGroupID)
				End If
			Catch oEx As System.Exception
				sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
				DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
			End Try
			iAttribIndex += 1
		End If
		If iAttribUB >= iAttribIndex Then '12
			Dim sMainData As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
			'		DMAcadExt.AcadDocument.WriteMessage("^24 :" & msPropertyID & ":" & sMainData & ":" & CStr(iAttribIndex))
			'		DMAcadExt.AcadDocument.WriteMessage("^|" & DMCommon.Functions.DispArray(Me.dsaBlockAttribText, "", False, "|"))
			'	Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(ddCentroidX, ddCentroidX, 0.0)
			If sMainData = "1" Then
				mbMainData = True
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "L1")
			ElseIf sMainData = "0" Then
				mbMainData = False
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "L0")
			Else
				mbMainData = False
				'	DMAcadExt.AcadTransaction.InsertPoint(tCentroid, , "LEr")
				''Error
			End If

			iAttribIndex += 1
		End If
		If mbMainData Then
			If iAttribUB >= iAttribIndex Then '13
				Try
					Dim sPolygonColor As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
					If Not String.IsNullOrEmpty(sPolygonColor) Then
						miPolygonColor = bmBamash.Text2Int(sPolygonColor, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					End If
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
			If iAttribUB >= iAttribIndex Then '14
				Try
					msPurchaser = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
			If iAttribUB >= iAttribIndex Then '15
				Try
					Dim sPropID_Old As String = DMCommon.Functions.CStrN(Me.dsaBlockAttribText(iAttribIndex))
					If sPropID_Old.Length = 0 Then
						miPropID_Old = 0
					Else
						miPropID_Old = bmBamash.Text2Int(sPropID_Old, "BamashPolygon - zzSetData_" & CStr(iAttribIndex) & ": " & GetAttributeTag(iAttribIndex))
					End If
				Catch oEx As System.Exception
					sMsg = zzGetAttribMsg(iAttribIndex) & oEx.Message
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "BamashPolygon-zzSetData_" & CStr(iAttribIndex), oEx.Message, True)
				End Try
				iAttribIndex += 1
			End If
		End If

		zzAfterInput(True, False) '***
	End Sub
	Private Sub zzParseGroup(ByVal sValue As String)

		If sValue.StartsWith("*") Then
			sValue = sValue.Substring(1)
			mbIsGroupBase = True
		Else
			mbIsGroupBase = False
		End If
		miGroupID = bmBamash.Text2Int(sValue)

	End Sub
	Private Sub zzAfterInput(ByVal bFromDWG As Boolean, ByVal bFromTable As Boolean)

		zzGetPropertyType(miPropertyType, mbIsProperty, mbIsSubProp)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "zzGetPropertyType", miPropertyType, mbIsProperty, mbIsSubProp)
		zzSetPropertyID()
		If mbIsProperty Then
			zzSetSubParcel(bFromDWG, bFromTable)
			zzSetDescription(bFromDWG)

			If miBldNo.HasValue Then
				mtPropUnitKey.BldNo = miBldNo.Value
			End If
			If miBldPart.HasValue Then
				mtPropUnitKey.BldPart = miBldPart.Value
			End If
			If miBldEntr.HasValue Then
				mtPropUnitKey.BldEntr = miBldEntr.Value
			End If

			If miBldFloor.HasValue Then
				mtPropUnitKey.BldFloor = miBldFloor.Value
			End If
			If miBldSubFloor.HasValue Then
				mtPropUnitKey.BldSubFloor = miBldSubFloor.Value
			End If

			mtPropUnitKey.Number = miSubPropNum
		End If
		If mbSubPropConst Then
			'msSubPropCaption = mssubpro

		Else
			If mbNumberingSeparately Then
				Dim iRem As Integer
				Math.DivRem(miPropertyID, 2, iRem)

				If iRem = 0 Then
					msSubPropCaption = DMCommon.Hebrew.GetHebNum(miSubPropNum, True) & miPropertyID.ToString()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterInputRight", msSubPropCaption)
					msSubPropCaption = DMCommon.Hebrew.GetHebNum(5, True)
				Else
					msSubPropCaption = miPropertyID.ToString() & DMCommon.Hebrew.GetHebNum(miSubPropNum, True)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AfterInputLeft", msSubPropCaption)
					msSubPropCaption = DMCommon.Hebrew.GetHebNum(9, True)
				End If

			Else
				msSubPropCaption = DMCommon.Hebrew.GetHebNum(miSubPropNum, True)   '''''''??????????????????????
			End If


		End If
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzAfterInput", msSubPropCaptionToAcad, msSubPropCaption, msAprtDesc, msAprtDescDOS)


	End Sub
	Private Function zzGetAttribMsg(ByVal iAttribIndex As Integer) As String
		Dim sTag As String = GetAttributeTag(iAttribIndex)
		If Not String.IsNullOrEmpty(sTag) AndAlso dsaBlockAttribText.GetUpperBound(0) >= iAttribIndex Then
			Return "Attribute " & CStr(iAttribIndex) & ", " & sTag & "=" & dsaBlockAttribText(iAttribIndex) & ","
		Else
			Return String.Empty
		End If

	End Function
	Public Overrides Sub Terminate()
		If moMainDataTable IsNot Nothing Then
			moMainDataTable.Dispose()
			moMainDataTable = Nothing
		End If
		MyBase.OnTerminate()
	End Sub
	Public Shared Sub Initialize(tMapThemeData As DMAcadExt.MapThemeData)
		Try
			For iIndex As Integer = 0 To miaInputBlockAttribIndex.GetUpperBound(0)
				miaInputBlockAttribIndex(iIndex) = -1
			Next
			'	msCentroidBlockName = TopoDefs.Item(New DMAcadExt.TopoDefID(TopoManager.TPlanGraph.enTopoPurpose.Bamash)).CentroidBlocks(0)
			msCentroidBlockName = tMapThemeData.CentroidBlock
			msaBlockAttribTag = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)

			If msaBlockAttribTag IsNot Nothing Then
				For iAttribIndex As Integer = 0 To msaBlockAttribTag.GetUpperBound(0)
					Select Case msaBlockAttribTag(iAttribIndex)
						Case msUserIDAttribTag
							miaInputBlockAttribIndex(enAttribute.UserID) = iAttribIndex
						Case msPropIDAttribTag
							miaInputBlockAttribIndex(enAttribute.PropID) = iAttribIndex
						Case msSubParcelNoAttribTag
							miaInputBlockAttribIndex(enAttribute.SubParcelNo) = iAttribIndex
							miaUpdateBlockAttribIndex(0) = iAttribIndex
						Case msPropTypeAttribTag
							miaInputBlockAttribIndex(enAttribute.PropType) = iAttribIndex
						Case msAprtDescNumAttribTag
							miaInputBlockAttribIndex(enAttribute.AprtDescNum) = iAttribIndex
						Case msBldFloorAttribTag
							miaInputBlockAttribIndex(enAttribute.BldFloor) = iAttribIndex
						Case msBldEntrAttribTag
							miaInputBlockAttribIndex(enAttribute.BldEntr) = iAttribIndex
						Case msBldPartAttribTag
							miaInputBlockAttribIndex(enAttribute.BldPart) = iAttribIndex
						Case msBldNoAttribTag
							miaInputBlockAttribIndex(enAttribute.BldNo) = iAttribIndex
						Case msAprtDescAttribTag
							miaInputBlockAttribIndex(enAttribute.AprtDesc) = iAttribIndex
							miaUpdateBlockAttribIndex(1) = iAttribIndex
						Case msAprtDesc2AttribTag
							miaInputBlockAttribIndex(enAttribute.AprtDesc2) = iAttribIndex
							miaUpdateBlockAttribIndex(2) = iAttribIndex
						Case msSubPropGroupAttribTag
							miaInputBlockAttribIndex(enAttribute.SubPropGroup) = iAttribIndex
						Case msBlockMainAttribTag
							miaInputBlockAttribIndex(enAttribute.BlockMain) = iAttribIndex
							' MessageBox.Show(CStr(enAttribute.BlockMain) & ":" & CStr(miaInputBlockAttribIndex(enAttribute.BlockMain)), "04_400")
						Case msPolygonColorAttribTag
							miaInputBlockAttribIndex(enAttribute.PolygonColor) = iAttribIndex
						Case msPurchaserAttribTag
							miaInputBlockAttribIndex(enAttribute.Purchaser) = iAttribIndex
						Case msPropIDOldAttribTag
							miaInputBlockAttribIndex(enAttribute.PropID_Old) = iAttribIndex
						Case Else
							System.Windows.Forms.MessageBox.Show(msaBlockAttribTag(iAttribIndex) & ":" & CStr(msSubPropGroupAttribTag) & ":" & CStr(iAttribIndex), "BamashPolygon - Initialize")
					End Select
				Next
				'DMCommon.Functions.DispArray("miaInputBlockAttribIndex", miaInputBlockAttribIndex)
			End If
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "BamashPolygon - Initialize")
		End Try
	End Sub
	Public Shared Function GetPropKey(sPropertyID As String, iPropertyType As enPropertyTypes) As Integer
		Dim iPropertyID As Integer

		If iPropertyType = enPropertyTypes.SubShareType Then
			Return zzSharePropIDToKey(sPropertyID)
		ElseIf Integer.TryParse(sPropertyID, iPropertyID) Then
			Return iPropertyID
		End If
	End Function
	Public Shared Function GetPgonKey(bMainData As Boolean, iPropertyType As enPropertyTypes, iSubPropNum As Integer) As Integer
		If bMainData Then
			Return -100
		ElseIf iPropertyType = enPropertyTypes.ApartType Then
			Return -90
		ElseIf iPropertyType = enPropertyTypes.BalconyType Then
			Return -80
		ElseIf iPropertyType = enPropertyTypes.WarehouseType Then
			Return -70
		ElseIf iPropertyType = enPropertyTypes.SubPrivateType OrElse iPropertyType = enPropertyTypes.SubShareType Then
			Return iSubPropNum
		Else
			Return 0
		End If
	End Function
	Public Shared Sub OpenMainDataTable()
		Dim oDataColumn As DataColumn
		moMainDataTable = New DataTable("Bamash")

		With moMainDataTable.Columns
			.Add(msUserIDFieldName, GetType(System.String))                    '0
			.Add(msPropIDFieldName, GetType(System.String))                    '1
			.Add(msSubParcelNoFieldName, GetType(System.String))                 '2
			oDataColumn = .Add(msCaptionFieldName, GetType(System.String))       '3

			.Add(msPropTypeFieldName, GetType(System.Int32))                     '4
			.Add(msAprtDescNumFieldName, GetType(System.Int32))                  '5

			.Add(msBldFloorFieldName, GetType(System.String))                 '6
			.Add(msBldEntrFieldName, GetType(System.Int32))                   '7
			.Add(msBldPartFieldName, GetType(System.Int32))                   '8
			.Add(msBldNoFieldName, GetType(System.Int32))                     '9

			.Add(msAprtDescFieldName, GetType(System.String))                 '10
			oDataColumn = .Add(msAprtDesc2FieldName, GetType(System.String))  '11
			'	oDataColumn.ReadOnly = True
			oDataColumn = .Add(msBldFloorDescFieldName, GetType(System.String)) '12
			'oDataColumn.ReadOnly = True
			.Add(msMainDataFieldName, GetType(System.Boolean))                     '13
			.Add(msPolygonColorFieldName, GetType(System.Int32))               '14
			.Add(msPurchaserFieldName, GetType(System.String))           '15 
			.Add(msPropIDOldFieldName, GetType(System.Int32))            '16
			.Add(msPgonGroupFieldName, GetType(System.Int32))            '17
			.Add(msGroupBaseFieldName, GetType(System.Boolean))             '18
			.Add(msGroupAreaFieldName, GetType(System.Double))           '19
			.Add(msPropKeyFieldName, GetType(System.Int32))                    '20
			.Add(msPgonKeyFieldName, GetType(System.Int32))                    '20




			oDataColumn = .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))  '20
			oDataColumn.ReadOnly = True
			oDataColumn = .Add(TopoReader.msAreaFldName, GetType(System.Double)) '21
			oDataColumn.ReadOnly = True
			oDataColumn = New System.Data.DataColumn(msChangedFieldName, GetType(System.Boolean))
			oDataColumn.DefaultValue = False
			.Add(oDataColumn)
			oDataColumn = New System.Data.DataColumn(msHasValueFieldName, GetType(System.Boolean))
			oDataColumn.DefaultValue = False
			.Add(oDataColumn)
		End With

	End Sub

	Public Sub AddDataToMainTable()
		Dim oNewRow As System.Data.DataRow

		Dim bHasValue As Boolean
		If moMainDataTable IsNot Nothing Then
			Try
				oNewRow = moMainDataTable.NewRow()
				With oNewRow
					.Item(msUserIDFieldName) = msUserID
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddDataToMainTable", diTopoID, msPropertyID, Me.Caption, miPropertyType, miSubPropNum)

					If Not String.IsNullOrEmpty(msPropertyID) Then
						.Item(msPropIDFieldName) = msPropertyID
						bHasValue = True
					End If
					If Not String.IsNullOrEmpty(msSubParcelNo) Then
						.Item(msSubParcelNoFieldName) = msSubParcelNo
						bHasValue = True
					End If
					If Not String.IsNullOrEmpty(Me.Caption) Then
						.Item(msCaptionFieldName) = Me.Caption
						bHasValue = True
					End If
					If miPropertyType <> enPropertyTypes.Default Then
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "MTable:Caption", Me.Caption, miPropertyType, msPropertyID, miSubPropNum)
					End If


					If miPropertyType <> enPropertyTypes.Default Then
						.Item(msPropTypeFieldName) = miPropertyType
						bHasValue = True

					End If


					.Item(msAprtDescNumFieldName) = miAprtDescNum
					If miPropertyID <= 10 Then
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!AprtDescNum", Me.Caption, miPropertyType, msPropertyID, miSubPropNum)
					End If

					.Item(msBldFloorFieldName) = msBldFloor

					If miBldEntr.HasValue Then
						.Item(msBldEntrFieldName) = miBldEntr.Value
						bHasValue = True
					End If

					If miBldPart.HasValue Then
						.Item(msBldPartFieldName) = miBldPart.Value
						bHasValue = True
					End If
					If miBldNo.HasValue Then
						.Item(msBldNoFieldName) = miBldNo.Value
						bHasValue = True
					End If


					.Item(msAprtDescFieldName) = msAprtDesc
					.Item(msAprtDesc2FieldName) = msAprtDesc2
					.Item(msBldFloorDescFieldName) = msBldFloorDesc

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddDataToMainTable", msBldFloorDesc)

					.Item(msMainDataFieldName) = mbMainData

					If miPolygonColor <> -1 Then
						.Item(msPolygonColorFieldName) = miPolygonColor
					End If

					.Item(msPgonGroupFieldName) = miGroupID
					.Item(msGroupBaseFieldName) = mbIsGroupBase
					.Item(msGroupAreaFieldName) = mdGroupArea


					.Item(TopoReader.msTopoIDFldName) = MyBase.diTopoID
					.Item(TopoReader.msAreaFldName) = Math.Round(MyBase.ddAcadArea, 4, MidpointRounding.AwayFromZero)


					If False Then
						If mbMainData Then
							.Item(msPgonKeyFieldName) = -100
						ElseIf miPropertyType = enPropertyTypes.ApartType Then
							.Item(msPgonKeyFieldName) = -90
						ElseIf miPropertyType = enPropertyTypes.BalconyType Then
							.Item(msPgonKeyFieldName) = -80
						ElseIf miPropertyType = enPropertyTypes.WarehouseType Then
							.Item(msPgonKeyFieldName) = -70
						ElseIf miPropertyType = enPropertyTypes.SubPrivateType OrElse miPropertyType = enPropertyTypes.SubShareType Then
							.Item(msPgonKeyFieldName) = miSubPropNum
						End If
					End If
					.Item(msPropKeyFieldName) = GetPropKey(msPropertyID, miPropertyType)
					.Item(msPgonKeyFieldName) = GetPgonKey(mbMainData, miPropertyType, miSubPropNum)
					.Item(msHasValueFieldName) = bHasValue

				End With



				moMainDataTable.Rows.Add(oNewRow)
				miTableRowIndex = moMainDataTable.Rows.Count - 1
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "BamashPolygon - AddDataToMainTable")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable")
		End If

	End Sub
	Private Shared Function zzSharePropIDToKey(sPropID As String) As Integer
		Const iKeyUB As Integer = 2
		Dim iSharePropKey As Integer = 0 '= 1000000000
		Dim saPropID() As String = Split(sPropID, ",")
		Dim iComp As Integer
		ReDim Preserve saPropID(iKeyUB)
		For iIndex As Integer = 0 To iKeyUB
			iSharePropKey *= 1000
			If Not String.IsNullOrEmpty(saPropID(iIndex)) AndAlso Integer.TryParse(saPropID(iIndex), iComp) Then
				iSharePropKey += iComp

			End If
		Next
		Return 1000000000 + iSharePropKey
	End Function
	Public Sub GetDataFromTable_020820(ByVal oDataRow As DataRow)

		If oDataRow IsNot Nothing Then
			Try
				With oDataRow
					msUserID = CStrN(.Item(msUserIDFieldName))
					msPropertyID = CStrN(.Item(msPropIDFieldName))

					msSubParcelNo = CStrN(.Item(msSubParcelNoFieldName))

					zzPropertyTypeFromTable(.Item(msPropTypeFieldName))

					miAprtDescNum = CIntN(.Item(msAprtDescNumFieldName))
					miPolygonColor = CIntN(.Item(msPolygonColorFieldName))

					msBldFloor = CStrN(.Item(msBldFloorFieldName))
					miBldEntr = CIntN(.Item(msBldEntrFieldName))
					miBldPart = CIntN(.Item(msBldPartFieldName))
					miBldNo = CIntN(.Item(msBldNoFieldName))

					msAprtDesc = CStrN(.Item(msAprtDescFieldName))
					'	.Item(msAprtDesc2FieldName) = msAprtDesc2
					'		.Item(msBldFloorDescFieldName) = msBldFloorDesc

					'  .Item(TopoReader.msTopoIDFldName) = MyBase.diTopoID
					'	.Item(TopoReader.msAreaFldName) = MyBase.ddAcadArea
				End With
				zzAfterInput(False, True)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "BamashPolygon - GetDataFromTable")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "GetDataFromTable")
		End If

	End Sub
	Public Sub GetDataFromTable(ByVal oDataRow As DataRow)

		If oDataRow IsNot Nothing Then
			Try
				With oDataRow
					msUserID = CStrN(.Item(msUserIDFieldName))
					msPropertyID = CStrN(.Item(msPropIDFieldName))

					msSubParcelNo = CStrN(.Item(msSubParcelNoFieldName))

					zzPropertyTypeFromTable(.Item(msPropTypeFieldName))

					miAprtDescNum = CIntN(.Item(msAprtDescNumFieldName))
					miPolygonColor = CIntN(.Item(msPolygonColorFieldName), -1)

					msBldFloor = CStrN(.Item(msBldFloorFieldName))

					CIntN(.Item(msBldEntrFieldName), miBldEntr)
					CIntN(.Item(msBldPartFieldName), miBldPart)
					CIntN(.Item(msBldNoFieldName), miBldNo)

					msAprtDesc = CStrN(.Item(msAprtDescFieldName))

					DMCommon.Debug.ExcelLog.SetNextValue(0, "!FromTab", msAprtDesc, msBldFloor)

					'	.Item(msAprtDesc2FieldName) = msAprtDesc2
					'		.Item(msBldFloorDescFieldName) = msBldFloorDesc

					'  .Item(TopoReader.msTopoIDFldName) = MyBase.diTopoID
					'	.Item(TopoReader.msAreaFldName) = MyBase.ddAcadArea
				End With
				zzAfterInput(False, True)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "BamashPolygon - GetDataFromTable")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "GetDataFromTable")
		End If

	End Sub
	Private Sub zzSetDescription(ByVal bFromDOS As Boolean)

		Select Case miAprtDescNum
			Case 0, 99
				If bFromDOS Then
					msAprtDesc = DMCommon.Hebrew.FromAcad(msAprtDescDOS)
				Else
					msAprtDescDOS = DMCommon.Hebrew.WordToDOS(msAprtDesc, False)
				End If

			Case 1 To 98
				msAprtDesc = GetApartDescr(miPolygonID, "Aprtdescnum", miAprtDescNum)
				msAprtDescDOS = DMCommon.Hebrew.WordToDOS(msAprtDesc, False)
			Case Else
				msAprtDesc = String.Empty
		End Select



		If miPropertyType = enPropertyTypes.BalconyType Then
			msAprtDesc2 = zzGetAdditionDescr(Me.FormatArea, False, False)
			msAprtDesc2DOS = zzGetAdditionDescr(Me.FormatArea, True, False)
			msAprtDesc2Table = zzGetAdditionDescr(Me.FormatArea, False, True)
			msAprtDesc2Excel = msAprtDesc2
		End If

		'Dim saValue() As String

		If Not String.IsNullOrEmpty(msBldFloor) Then
			ParseBldFloor(msBldFloor, miBldFloor, miBldSubFloor)
			'	saValue = Split(msBldFloor, ".")
			'	miBldFloor = Convert.ToInt32(saValue(0))
			'If saValue.GetUpperBound(0) > 0 Then
			'miBldSubFloor = Convert.ToInt32(saValue(1))
			'End If
			Dim iBldFloorVal, iBldSubFloorVal As Integer
			If miBldFloor.HasValue Then
				iBldFloorVal = miBldFloor.Value
			End If
			If miBldSubFloor.HasValue Then
				iBldSubFloorVal = miBldSubFloor.Value
			End If
			msBldFloorDesc = GetFloorDescr2015(miPolygonID, "Bldfloordesc", iBldFloorVal, iBldSubFloorVal)
			'GetFloorDescr2015(ByVal iPolygonID As Integer, ByVal sTag As String, sBldFloor As String) As String
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzSetDescription", msBldFloorDesc, miBldFloor, miBldSubFloor)
		End If

	End Sub
	Private Function zzGetAdditionDescr(ByVal sArea As String, ByVal bDOS As Boolean, ByVal bNumInvert As Boolean) As String
		'   Const sPrev = "מרפסת פתוחה בשטח "
		Const sPrev As String = "בשטח"
		Const sPast As String = "מ""ר"

		'	Invert()
		If bNumInvert Then
			sArea = DMCommon.Hebrew.Invert(sArea)
		End If
		If bDOS Then
			Return DMCommon.Hebrew.WordToDOS(sPast, False) & " " & sArea & " " & DMCommon.Hebrew.WordToDOS(sPrev, False)
		Else
			Return sPrev & " " & sArea & " " & sPast
		End If


	End Function
	Private Shared Sub zzGetPropertyType(iPropertyType As enPropertyTypes, ByRef bIsProperty As Boolean, ByRef bIsSubProp As Boolean)
		Select Case iPropertyType
			Case enPropertyTypes.ZeroType
				bIsProperty = False
				bIsSubProp = True
			Case enPropertyTypes.ZeroFreezeType
				bIsProperty = False
				bIsSubProp = True
			Case enPropertyTypes.ApartType, enPropertyTypes.BalconyType, enPropertyTypes.WarehouseType
				bIsSubProp = False
				bIsProperty = True
			Case Else
				bIsSubProp = True
				bIsProperty = True
		End Select
	End Sub

	Private Sub zzSetPropertyID()
		If miPropertyType = enPropertyTypes.SubShareType Then

			Try
				If Not mbIsProperty Then
					msPropertyID = "0"
					miPropertyID = 0
				ElseIf msPropertyID.Length = 0 Then
					DMAcadExt.AcadDocument.WriteMessage("BamashPolygon - zzSetData aPropertyIDs was not found")
				Else
					If mbUseNumeration Then
						moNumeration = New NumerationPair.Numeration(NumerationPair.enComplexType.Undefined, NumerationPair.enTextDirection.LeftToRight)
						moNumeration.Input(msPropertyID)
						miaPropertyIDs = moNumeration.GetBaseArray()
					Else
						miaPropertyIDs = SplitToInt(msPropertyID, MyBase.diTopoID)
					End If
					DMCommon.Debug.MsgBox("!SetMainData", miaPropertyIDs)


					If miaPropertyIDs.GetUpperBound(0) = 0 Then
						DMAcadExt.AcadDocument.WriteMessage("BamashPolygon - zzSetData aPropertyIDs only single")
					End If
					If Not mbColorUnique Then
						ReDim miaPolygonColors(miaPropertyIDs.GetUpperBound(0))
					Else

					End If

				End If
			Catch oEx As System.Exception
				DMAcadExt.AcadDocument.WriteMessage("BamashPolygon - zzSetData aPropertyIDs" & ": " & oEx.Message)
			End Try
		ElseIf miPropertyID <> -1 Then

			If Integer.TryParse(msPropertyID, miPropertyID) Then
				If miPropertyID < miMinPropertyID OrElse miMinPropertyID = 0 Then
					miMinPropertyID = miPropertyID
				End If
				If miPropertyID > miMaxPropertyID Then
					miMaxPropertyID = miPropertyID
				End If
				If Not mhsPropertiesID.Contains(miPropertyID) Then
					mhsPropertiesID.Add(miPropertyID)
				End If
			End If
		End If

	End Sub
	Private Sub zzSetSubParcel(ByVal bFromDWG As Boolean, ByVal bFromTable As Boolean)
		If mbIsSubProp Then
			If bFromDWG OrElse bFromTable Then
				If String.IsNullOrEmpty(msSubParcelNo) Then
					miSubPropNum = 0
				Else
					miSubPropNum = bmBamash.Heb2Num(msSubParcelNo)
					If miSubPropNum = 0 Then
						mbSubPropConst = True
					Else
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SetSubP", msSubParcelNo, miSubPropNum, miMinSubPropNum, miMaxSubPropNum)
						If miSubPropNum < miMinSubPropNum OrElse miMinSubPropNum = 0 Then
							miMinSubPropNum = miSubPropNum
						End If
						If miSubPropNum > miMaxSubPropNum Then
							miMaxSubPropNum = miSubPropNum
						End If
						If Not mhsSubPropNums.Contains(miSubPropNum) Then
							mhsSubPropNums.Add(miSubPropNum)
						End If
					End If

				End If
			End If


			If miSubPropNum = 0 Then
				Dim sMsg As String
				If Not String.IsNullOrEmpty(msSubParcelNo) Then
					sMsg = MyBase.Coordinates & " - Number isn't right: <" & msSubParcelNo & ">"
					sMsg &= vbCrLf & msSubParcelNo
					sMsg &= vbCrLf & TestStr(msSubParcelNo)
					DMAcadExt.AppMessages.AddMessage(True, MyBase.ddCentroidX, MyBase.ddCentroidY, String.Empty, sMsg, True)
				End If


				If mbSubPropConst Then
					msSubPropCaption = msSubParcelNo
					msSubPropCaptionToAcad = msSubParcelNoInput
				Else
					msSubPropCaption = String.Empty
					msSubPropCaptionToAcad = String.Empty
				End If

			Else
				If mbNumberingSeparately Then
					Dim sHebPart As String = DMCommon.Hebrew.GetHebNum(miSubPropNum, True)
					msSubPropCaption = miPropertyID.ToString() & sHebPart '''''''??????????????????????
					msSubPropCaptionToAcad = miPropertyID.ToString() & DMCommon.Hebrew.ToDOS(sHebPart)
					msSubPropCaptionToAcad = DMCommon.Hebrew.ToDOS(sHebPart) & miPropertyID.ToString()

				Else
					msSubPropCaption = DMCommon.Hebrew.GetHebNum(miSubPropNum, True)   '''''''??????????????????????
					msSubPropCaptionToAcad = DMCommon.Hebrew.ToDOS(msSubPropCaption)
				End If

			End If

		Else
			msSubPropCaption = Convert.ToString(miPropertyID)
			msSubPropCaptionToAcad = msSubPropCaption
		End If
	End Sub
	Public Sub UpdateNewData(tBamashData As bmPolygonData)
		Dim saUpdateBlockAttribText(enAttribute.AttributesUB) As String
		Dim iaUpdateBlockAttribIndex(enAttribute.AttributesUB) As Integer
		Dim iAttribIndex As Integer = 0
		Dim bCaption As Boolean = False
		'DMCommon.Debug.MsgBox("2907_2", miPropertyID, miPropertyType, miTableRowIndex, tBamashData.BldFloor, tBamashData.BldEntr, tBamashData.BldPart, tBamashData.BldNo)
		Dim oDataRow As DataRow = moMainDataTable.Rows.Item(miTableRowIndex)
		If tBamashData.PropertyID > 0 Then
			miPropertyID = tBamashData.PropertyID
			msPropertyID = miPropertyID.ToString()
			saUpdateBlockAttribText(iAttribIndex) = msPropertyID
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.PropID)
			iAttribIndex += 1
			bCaption = True
		End If

		If tBamashData.PropertyType <> enPropertyTypes.Default Then
			miPropertyType = tBamashData.PropertyType
			saUpdateBlockAttribText(iAttribIndex) = CStr(Convert.ToInt32(miPropertyType))
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.PropType)
			iAttribIndex += 1
			bCaption = True
		End If

		If tBamashData.SubPropNum > 0 Then
			miSubPropNum = tBamashData.SubPropNum
			saUpdateBlockAttribText(iAttribIndex) = CStr(miSubPropNum)
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.SubParcelNo)
			iAttribIndex += 1
			bCaption = True
		End If
		If bCaption Then
			saUpdateBlockAttribText(iAttribIndex) = Me.Caption
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.SubParcelNo)
			iAttribIndex += 1
		End If
		If miPropertyType = enPropertyTypes.ApartType Then
			If tBamashData.PolygonColor > 0 Then
				miPolygonColor = tBamashData.PolygonColor
				saUpdateBlockAttribText(iAttribIndex) = CStr(miPolygonColor)
				iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.PolygonColor)
				iAttribIndex += 1
			End If
		End If

		If tBamashData.AprtDescNum > 0 Then
			miAprtDescNum = tBamashData.AprtDescNum
			oDataRow.Item(msAprtDescNumFieldName) = miAprtDescNum
			saUpdateBlockAttribText(iAttribIndex) = CStr(miAprtDescNum)
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.AprtDescNum)
			iAttribIndex += 1
			msAprtDesc = tBamashData.AprtDesc
			oDataRow.Item(msAprtDescFieldName) = msAprtDesc
			saUpdateBlockAttribText(iAttribIndex) = ToDOS(msAprtDesc)
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.AprtDesc)
			iAttribIndex += 1
		End If
		If tBamashData.BldFloor IsNot Nothing Then
			msBldFloor = tBamashData.BldFloor
			oDataRow.Item(msBldFloorFieldName) = msBldFloor
			saUpdateBlockAttribText(iAttribIndex) = msBldFloor
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.BldFloor)
			iAttribIndex += 1
		End If
		If tBamashData.BldNo.HasValue Then
			miBldNo = tBamashData.BldNo.Value
			oDataRow.Item(msBldNoFieldName) = miBldNo
			saUpdateBlockAttribText(iAttribIndex) = miBldNo.ToString()
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.BldNo)
			iAttribIndex += 1
		End If
		If tBamashData.BldEntr.HasValue Then
			miBldEntr = tBamashData.BldEntr.Value
			oDataRow.Item(msBldEntrFieldName) = miBldEntr
			saUpdateBlockAttribText(iAttribIndex) = miBldEntr.ToString()
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.BldEntr)
			iAttribIndex += 1
		End If

		If tBamashData.BldPart.HasValue Then
			miBldPart = tBamashData.BldPart.Value
			oDataRow.Item(msBldPartFieldName) = miBldPart
			saUpdateBlockAttribText(iAttribIndex) = miBldPart.ToString()
			iaUpdateBlockAttribIndex(iAttribIndex) = miaInputBlockAttribIndex(enAttribute.BldPart)
			iAttribIndex += 1
		End If
		zzAfterInput(False, False)

		ReDim Preserve saUpdateBlockAttribText(iAttribIndex - 1)
		ReDim Preserve iaUpdateBlockAttribIndex(iAttribIndex - 1)
		'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!AttribText", saUpdateBlockAttribText)
		'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!AttribIndex", iaUpdateBlockAttribIndex)

		If False Then
			If mbMainData Then
				saUpdateBlockAttribText(enAttribute.PolygonColor) = CStr(miPolygonColor)
				saUpdateBlockAttribText(enAttribute.BlockMain) = "1"
			Else
				saUpdateBlockAttribText(enAttribute.PolygonColor) = String.Empty
				saUpdateBlockAttribText(enAttribute.BlockMain) = "0"
			End If
		End If



		DMAcadExt.AcadTransaction.UpdateAttribText(MyBase.dtCentroidAcObjID, True, iaUpdateBlockAttribIndex, saUpdateBlockAttribText)
		'MyBase.UpdateCentroidAttributes(saUpdateBlockAttribText)
	End Sub
	Public Sub UpdateMainData()
		Dim saUpdateBlockAttribText(enAttribute.AttributesUB) As String
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2_1", miPropertyID, msPropertyID, miPropertyType, msSubPropCaptionToAcad, mbMainData)
		saUpdateBlockAttribText(enAttribute.PropID) = msPropertyID
		If miPropertyType = enPropertyTypes.Default Then
			saUpdateBlockAttribText(enAttribute.PropType) = String.Empty
		Else
			saUpdateBlockAttribText(enAttribute.PropType) = CStr(Convert.ToInt32(miPropertyType))

		End If

		saUpdateBlockAttribText(enAttribute.SubParcelNo) = msSubPropCaptionToAcad
		saUpdateBlockAttribText(enAttribute.AprtDescNum) = miAprtDescNum.ToString()
		saUpdateBlockAttribText(enAttribute.AprtDesc) = msAprtDescDOS




		If mbMainData Then
			If miPolygonColor = -1 Then
				saUpdateBlockAttribText(enAttribute.PolygonColor) = String.Empty
			Else
				saUpdateBlockAttribText(enAttribute.PolygonColor) = CStr(miPolygonColor)
			End If

			saUpdateBlockAttribText(enAttribute.BlockMain) = "1"
		Else
			saUpdateBlockAttribText(enAttribute.PolygonColor) = String.Empty
			saUpdateBlockAttribText(enAttribute.BlockMain) = "0"
		End If

		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2_2", saUpdateBlockAttribText)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ByPick_Al2_3", miaInputBlockAttribIndex)



		DMAcadExt.AcadTransaction.UpdateAttribText(MyBase.dtCentroidAcObjID, True, miaInputBlockAttribIndex, saUpdateBlockAttribText)

		'MyBase.UpdateCentroidAttributes(saUpdateBlockAttribText)
	End Sub
	Public Sub UpdateAllData()
		Dim saUpdateBlockAttribText(enAttribute.AttributesUB) As String
		saUpdateBlockAttribText(enAttribute.UserID) = msUserID
		saUpdateBlockAttribText(enAttribute.PropID) = msPropertyID
		saUpdateBlockAttribText(enAttribute.SubParcelNo) = msSubPropCaptionToAcad

		saUpdateBlockAttribText(enAttribute.PropType) = CStr(Convert.ToInt32(miPropertyType))
		saUpdateBlockAttribText(enAttribute.AprtDescNum) = CStr(miAprtDescNum)

		If mbMainData Then
			saUpdateBlockAttribText(enAttribute.PolygonColor) = CStr(miPolygonColor)
			saUpdateBlockAttribText(enAttribute.BlockMain) = "1"
		Else
			saUpdateBlockAttribText(enAttribute.PolygonColor) = String.Empty
			saUpdateBlockAttribText(enAttribute.BlockMain) = "0"
		End If

		saUpdateBlockAttribText(enAttribute.BldFloor) = msBldFloor
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!UpdAll", saUpdateBlockAttribText(enAttribute.BldFloor), msBldFloor)
		If miBldEntr.HasValue Then
			saUpdateBlockAttribText(enAttribute.BldEntr) = CStr(miBldEntr.Value)
		End If
		If miBldPart.HasValue Then
			saUpdateBlockAttribText(enAttribute.BldPart) = CStr(miBldPart.Value)
		End If
		If miBldNo.HasValue Then
			saUpdateBlockAttribText(enAttribute.BldNo) = CStr(miBldNo.Value)
		End If


		saUpdateBlockAttribText(enAttribute.AprtDesc) = msAprtDescDOS
		saUpdateBlockAttribText(enAttribute.AprtDesc2) = msAprtDesc2DOS
		saUpdateBlockAttribText(enAttribute.Purchaser) = msPurchaser
		'	DMCommon.Functions.Disp2Arrays(miaInputBlockAttribIndex, saUpdateBlockAttribText, "Update")

		DMAcadExt.AcadTransaction.UpdateAttribText(MyBase.dtCentroidAcObjID, True, miaInputBlockAttribIndex, saUpdateBlockAttribText)
		DMCommon.Debug.ExcelLog.SetEnumerable(0, "!miaInputBlockAtt", miaInputBlockAttribIndex)
		DMCommon.Debug.ExcelLog.SetEnumerable(0, "!saUpdateBlockAtt", saUpdateBlockAttribText)
		DMCommon.Debug.ExcelLog.SetNextValue(0, "!Centroid", MyBase.dtCentroidAcObjID)


		'MyBase.UpdateCentroidAttributes(saUpdateBlockAttribText)
	End Sub
	Public Sub UpdateDrawing()
		Dim saUpdateBlockAttribText() As String = {Me.CaptionUNICODE, msAprtDescDOS, msAprtDesc2DOS}

		'		If MyBase.diTopoID = 685 Or MyBase.diTopoID = 659 Then
		'		MessageBox.Show("!" & msAprtDescDOS & ":" & msAprtDesc & "!")
		'		DispArray(miaUpdateBlockAttribIndex, "BlockAttribIndex")
		'		DispArray(saUpdateBlockAttribText, "BlockAttribIndex")
		'	bTest = True
		'	End If


		Try
         DMAcadExt.AcadTransaction.UpdateAttribText(MyBase.dtCentroidAcObjID, True, miaUpdateBlockAttribIndex, saUpdateBlockAttribText)
		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage("BamashPolygon - UpdateData: " & oEx.Message)
		End Try

	End Sub

	Public Overloads Sub CreateColorScheme()
		Dim colColorIndices As System.Collections.Generic.ICollection(Of Integer)

		If mbColorUnique AndAlso (mdicPolygonColors IsNot Nothing) AndAlso mdicPolygonColors.Count <> 0 Then
			colColorIndices = mdicPolygonColors.Keys()
			If colColorIndices IsNot Nothing AndAlso colColorIndices.Count <> 0 Then
				ReDim miaPolygonColors(colColorIndices.Count - 1)
				colColorIndices.CopyTo(miaPolygonColors, 0)
			End If
		End If
		Select Case miPropertyType
			Case enPropertyTypes.ApartType, enPropertyTypes.BalconyType, enPropertyTypes.WarehouseType
				mtColorScheme = New DMAcadExt.ColorScheme(1.0)
				mtColorScheme.AddBorderStrip(New DMAcadExt.DMColor(Convert.ToInt16(miPolygonColor)), bmBamash.BufferOffset)

			Case enPropertyTypes.SubPrivateType, enPropertyTypes.UnderBldTypeColor
				mtColorScheme = New DMAcadExt.ColorScheme(1.0)
				mtColorScheme.SetBackColor(New DMAcadExt.DMColor(Convert.ToInt16(miPolygonColor)))
			Case enPropertyTypes.SubShareType
				mtColorScheme = New DMAcadExt.ColorScheme(1.0)
				If miaPolygonColors IsNot Nothing Then
					For iIndex As Integer = 0 To miaPolygonColors.GetUpperBound(0)
						mtColorScheme.AddZebraStrip(New DMAcadExt.DMColor(Convert.ToInt16(miaPolygonColors(iIndex))), bmBamash.ZebraWidthDrawing)
					Next
					mtColorScheme.ZebraAngle = New DMAcadExt.LineAngle(45.0, False)
				End If


		End Select
	End Sub
	Public Overloads Sub Paint()
		Select Case miPropertyType
			Case enPropertyTypes.ApartType, enPropertyTypes.BalconyType, enPropertyTypes.WarehouseType
            MyBase.Paint(DMAcadExt.PaintMethod.Border Or DMAcadExt.PaintMethod.BorderByBuffer, mtColorScheme, True)
			Case enPropertyTypes.SubPrivateType, enPropertyTypes.UnderBldTypeColor
				MyBase.Paint(DMAcadExt.PaintMethod.Hatch, mtColorScheme, True)
			Case enPropertyTypes.SubShareType
				MyBase.Paint(DMAcadExt.PaintMethod.Zebra Or DMAcadExt.PaintMethod.ZebraByTopo, mtColorScheme, True)
		End Select
	End Sub
	Public Shared ReadOnly Property MainView() As System.Data.DataView
		Get
			If moMainDataTable IsNot Nothing Then
				'msBldFloorFieldName
				'	Dim sSort As String = msPropKeyFieldName & "," & msMainDataFieldName & " " & "DESC" & "," & msBldNoFieldName & "," & msBldPartFieldName & "," & msBldEntrFieldName
				'	Dim sSort As String = msPropKeyFieldName & "," & msMainDataFieldName & " " & "DESC" & "," & msSubParcelNoFieldName ' & "," & msBldPartFieldName & "," & msBldEntrFieldName
				Dim sSort As String = msPropKeyFieldName & "," & msPgonKeyFieldName


				Dim sFilter As String = "HasValue"
				Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = True
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "sSort", sSort)
				'DMCommon.Debug.ExcelLog.SetDataTable(0, "!moMainData", oDataView)

				Return oDataView
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public ReadOnly Property IsSubProp() As Boolean
		Get
			Return mbIsSubProp
		End Get
	End Property
	Public ReadOnly Property IsProperty() As Boolean
		Get
			Return mbIsProperty
		End Get
	End Property
	Public ReadOnly Property MainData() As Boolean
		Get
			Return mbMainData
		End Get
	End Property
	Private Sub zzPropertyTypeFromTable(ByVal oValue As System.Object)
		Dim iValue As Integer = CIntN(oValue)
		If [Enum].IsDefined(GetType(enPropertyTypes), iValue) Then
			miPropertyType = DirectCast(iValue, enPropertyTypes)
		End If
	End Sub

	Public Function CompareTo(ByVal tOtherPropUnitKey As bmPropUnitKey) As Integer Implements System.IComparable(Of bmPropUnitKey).CompareTo
		Return mtPropUnitKey.CompareTo(tOtherPropUnitKey)
	End Function
	Public Function GetSubpropKey() As Integer
		'מרפסת, קרקע, מחסן, חניה
		'14,12,8,5,6
		Dim iAprtDeskOrder As Integer
		Select Case miAprtDescNum
			Case 14
				iAprtDeskOrder = 1
			Case 12
				iAprtDeskOrder = 2
			Case 8
				iAprtDeskOrder = 3

			Case 5
				iAprtDeskOrder = 4
			Case 6
				iAprtDeskOrder = 5
			Case Else
				iAprtDeskOrder = 99

		End Select

		Return 100 * miPropertyID + iAprtDeskOrder
	End Function

	Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
      
   End Property
   Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
   End Property

End Class
