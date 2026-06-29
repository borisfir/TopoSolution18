Option Explicit On
Option Strict On
Public Class bmProperty
	'	Public Shared OutputFullColor As Boolean
	Private miPropID As Integer
	Private mvPropUnits() As bmPropUnit
	Private mpgaBalcony As bmPolygonArray
	Private mcolPolygons As Collection
	Private moSubproperties As bmSubproperties

	''''''''''''''''''''''''''''''''Private mvSubProps(,) As System.Object
	Private mvUnderBld() As System.Object
	Private mvExpro(,) As System.Object

	Private msCaption As String
	Private msMainDescr As String
	Private mdTotalArea As Double
	Private mdMainArea As Double

	Private mbWarehouse As Boolean

	Private miSubPropCount As Integer
	Private miExproCount As Integer
	Private miPropUnitCount As Integer

	Private miPrintIndex As Integer
	Private miColorIndex As Integer = -1

	Private miPolygonCount As Integer

	Private miPropType As Integer

	Private miCurrentBldNo As Integer
	Private miCurrentBldPart As Integer
	Private miCurrentBldEntrance As Integer
	Private miCurrentBldFloor As Integer

	Private msUserID As String

	Private msUserCompany As String
	Private msUserSite As String
	Private msUserBuild As String
	Private msUserApart As String

	Private miMainDataPgonCount As Integer = 0



	'Private mdPolygonArea As Double

	Const miSubPropColDescrID As Integer = 0
	Const miSubPropColDescr As Integer = 1
	Const miSubPropColCaption As Integer = 2
	Const miSubPropColArea As Integer = 3
	Const miSubPropColTopoID As Integer = 4
	Const miSubPropColPropType As Integer = 5
	Const miSubPropColPolygonID As Integer = 6
	Const miSubPropColUserID As Integer = 7

	Const miSubPropColUB As Integer = 7

	Private Shared moMainRepTable As DataTable
	Private Shared miaVarColumnsFieldNums(enVarColumn.VarColumnUB) As Integer

	'Private i As Integer
	Private msApartDescr As String
	Private msApartDescrDOS As String

	Private msSubParcelNo As String

	Private mvPrintInitBookmark As System.Object
	Private miPrintInitRowIndex As Integer
	Private mdicPropUnits As Generic.SortedDictionary(Of bmPropUnitKey, bmPropUnit)
	Private mtDMColor As DMAcadExt.DMColor


	Private miApartCount As Integer = 0
	Private miBalconyCount As Integer = 0
	Private miWarehouseCount As Integer = 0

	Public Property ApartDescr() As String
		Get
			Return msApartDescr
		End Get
		Set(ByVal sValue As String)
			msApartDescr = sValue
		End Set
	End Property
	Public Property ApartDescrDOS() As String
		Get
			Return msApartDescrDOS
		End Get
		Set(ByVal sValue As String)
			msApartDescrDOS = sValue
		End Set
	End Property
	Public ReadOnly Property SubParcelNo() As String
		Get
			Return bmBamash.Parcel & "\" & CStr(miPropID)
		End Get
	End Property
	Public ReadOnly Property Balconies() As BamashPolygon()
		Get
			Dim iBalconyCount As Integer
			For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
				iBalconyCount += oPropUnit.BalconyCount
			Next
			If iBalconyCount > 0 Then
				Dim iIndex As Integer = 0
				Dim oaBalconies(iBalconyCount - 1) As BamashPolygon
				For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
					For iUnitIndex As Integer = 0 To oPropUnit.BalconyCount - 1
						oaBalconies(iIndex) = oPropUnit.Balcony.Item(iUnitIndex)
						iIndex += 1
					Next
				Next
				Return oaBalconies
			Else
				Return Nothing
			End If
		End Get
	End Property


	Public Property ColorIndex() As Integer
		Get
			Return miColorIndex
		End Get
		Set(ByVal iValue As Integer)
			miColorIndex = iValue
			mtDMColor = New DMAcadExt.DMColor(Convert.ToInt16(miColorIndex))
		End Set
	End Property
	Public Property UserID() As String
		Get
			Return msUserID
		End Get
		Set(ByVal sValue As String)
			msUserID = sValue
			Dim saValues() As String = Strings.Split(sValue, ",")
			If saValues.GetUpperBound(0) = 3 Then
				msUserCompany = saValues(0)
				msUserSite = saValues(1)
				msUserBuild = saValues(2)
				msUserApart = saValues(3)
			End If
		End Set
	End Property
	Public ReadOnly Property UserCompany() As String
		Get
			Return msUserCompany
		End Get
	End Property
	Public ReadOnly Property UserSite() As String
		Get
			Return msUserSite
		End Get
	End Property
	Public ReadOnly Property UserBuild() As String
		Get
			Return msUserBuild
		End Get
	End Property
	Public ReadOnly Property UserApart() As String
		Get
			Return msUserApart
		End Get
	End Property

	Public Shared Sub Initialize()
		zzCreatePrintTable()
	End Sub

	Public Shared ReadOnly Property MainRepView() As DataView
		Get
			Dim oDataView As DataView
			Try
				oDataView = New DataView(moMainRepTable, "", "", DataViewRowState.CurrentRows)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Property - MainRepView")

			End Try
			Try
				oDataView = New DataView(moMainRepTable)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Property - MainRepView_1")
				Return Nothing
			End Try
			oDataView.Table = moMainRepTable

			Return oDataView
		End Get
	End Property
	Public Shared Function GetDataColumns(bWin As Boolean) As Integer()
		Dim iaRes() As Integer = {0, 1, 2, -1, -1, 7, 8, 9, 10, -1, -1, 15, 16, 17}
		If bWin Then
			iaRes(3) = 4
			iaRes(4) = 6
			iaRes(9) = 12
			iaRes(10) = 14


		Else
			iaRes(3) = 3
			iaRes(4) = 5
			iaRes(9) = 11
			iaRes(10) = 13
		End If

		Return iaRes
	End Function
	Private Sub zzAddMainProp2009(ByVal oPolygon As BamashPolygon)
		Dim oPropUnit As bmPropUnit
		Try
			If mdicPropUnits.ContainsKey(oPolygon.PropUnitKey) Then
				oPropUnit = mdicPropUnits.Item(oPolygon.PropUnitKey)
			Else
				oPropUnit = New bmPropUnit(miColorIndex)
				mdicPropUnits.Add(oPolygon.PropUnitKey, oPropUnit)
			End If
			If oPolygon.MainData Then
				miColorIndex = oPolygon.ColorIndex
			End If
			oPropUnit.AddPolygon2009(oPolygon)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Property - zzAddMainProp2009")
		End Try

	End Sub
	Private Sub zzAddSubProp2009(ByVal oPolygon As BamashPolygon)
		moSubproperties.AddPolygon2009(oPolygon)
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Subprop", miPropID, moSubproperties.Count, oPolygon.PropertyType)
	End Sub
	Friend ReadOnly Property SubProperties() As bmSubproperties
		Get
			Return moSubproperties
		End Get
	End Property
	Public Sub AddPolygon2009(ByVal oPolygon As BamashPolygon)
		If oPolygon.MainData Then
			miMainDataPgonCount += 1
		End If
		Select Case oPolygon.PropertyType
			Case enPropertyTypes.ApartType
				miApartCount = 1
				zzAddMainProp2009(oPolygon)
			Case enPropertyTypes.BalconyType
				miBalconyCount += 1
				zzAddMainProp2009(oPolygon)
			Case enPropertyTypes.WarehouseType
				miWarehouseCount += 1
				zzAddMainProp2009(oPolygon)
			Case enPropertyTypes.SubPrivateType
				zzAddSubProp2009(oPolygon)
			Case enPropertyTypes.UnderBldTypeColor
				zzAddSubProp2009(oPolygon)
			Case enPropertyTypes.UnderBldTypeWhite
				zzAddSubProp2009(oPolygon)
		End Select
	End Sub
	Public Sub Calculate(ByVal iCalcOption As bmBamash.SubNumerationOptions)
		If miMainDataPgonCount <> 1 AndAlso Me.HasID Then
			Dim sMsg As String = "PropID=" & Convert.ToString(miPropID) & ": Number of main polygons - " & CStr(miMainDataPgonCount)
			'''''''''''''''''''''''''''''''''	DMAcadExt.AppMessages.AddMessage(False, 0.0, 0.0, "", sMsg, True)
		End If

		For Each oPropUnit As bmPropUnit In mdicPropUnits.Values()
			oPropUnit.PropID = miPropID
			If miMainDataPgonCount = 1 Then
				oPropUnit.Color = miColorIndex
			End If ''
			oPropUnit.Calculate()
			mdMainArea += oPropUnit.MainArea
			mdTotalArea += oPropUnit.TotalArea
		Next
		moSubproperties.SetColor(miColorIndex)
		'	MessageBox.Show(CStr(Me.PropID) & ":" & CStr(moSubproperties.Count), "03_010")
		Dim oPgon As BamashPolygon
		If miPropID = 9999 Then
			For iIndex As Integer = 0 To moSubproperties.Count - 1
				oPgon = moSubproperties.Item(iIndex)
				DMAcadExt.AcadDocument.WriteMessage("12_112: " & CStr(oPgon.SubPropNum) & ":" & oPgon.SubParcelNo)
			Next
		End If
		'	moSubproperties.PrintList("---Before---")
		If BamashPolygon.NumberingSeparately Then
			moSubproperties.SortSubpropsNumberingSeparately()
		Else
			moSubproperties.SortSubprops(iCalcOption)
		End If


		'		moSubproperties.PrintList("---After---")
		If miPropID = 9999 Then
			For iIndex As Integer = 0 To moSubproperties.Count - 1
				oPgon = moSubproperties.Item(iIndex)
				DMAcadExt.AcadDocument.WriteMessage("12_114: " & CStr(oPgon.SubPropNum) & ":" & oPgon.SubParcelNo)
			Next
		End If
		moSubproperties.Calculate()
	End Sub
	Public Sub SubpropNumerate()
		Dim oBamashPolygon As BamashPolygon
		Dim iCurrentSubpropNum As Integer = 1
		If Me.HasID Then

			For iIndex As Integer = 0 To moSubproperties.Count - 1
				oBamashPolygon = moSubproperties.PgonReal(iIndex)
				iCurrentSubpropNum = BamashPolygon.CheckSubpropNum(iCurrentSubpropNum)
				oBamashPolygon.SetMainData(-1, enPropertyTypes.Ignore, iCurrentSubpropNum, oBamashPolygon.AprtDescNum, -1)
				oBamashPolygon.UpdateMainData()
				iCurrentSubpropNum += 1
			Next


		End If

	End Sub
	Private Shared Sub zzCreatePrintTable()

		moMainRepTable = New DataTable("MainPrint")
		With moMainRepTable.Columns
			.Add("BldNo", GetType(System.String)) '0
			.Add("BldPart", GetType(System.String)) '1
			.Add("EntranceDescr", GetType(System.String)) '2
			.Add("FloorDescr", GetType(System.String)) '3
			.Add("FloorDescrWin", GetType(System.String)) '4


			.Add("ApartDescr", GetType(System.String)) '5
			.Add("ApartDescrWin", GetType(System.String)) '6

			.Add("MainCaption", GetType(System.String)) '7
			.Add("MainArea", GetType(System.String)) '8
			.Add("WarehouseArea", GetType(System.String)) '9
			.Add("SumArea", GetType(System.String)) '10

			.Add("SubDescr", GetType(System.String)) '11
			.Add("SubDescrWin", GetType(System.String)) '12

			.Add("SubCaption", GetType(System.String)) '13
			.Add("SubCaptionWin", GetType(System.String)) '14

			.Add("SubpropArea", GetType(System.String)) '15


			.Add("Color", GetType(DMAcadExt.DMColor)) '16
			.Add("ColorName", GetType(System.String)) '17
			'	.Add("PropID", TopoManager.Common.GetAppType(System.STRING))
		End With
		Dim iColOrdinal As Integer
		Dim iAllColUB As Integer = moMainRepTable.Columns.Count - 1
		iAllColUB = 14 ' columns in Acad Table
		iColOrdinal = moMainRepTable.Columns.Item("BldNo").Ordinal
		miaVarColumnsFieldNums(enVarColumn.BldNo) = iAllColUB - iColOrdinal - 1
		DMCommon.Debug.ExcelLog.SetValue(0, "!EmpyColA ", iAllColUB, iColOrdinal, enVarColumn.BldNo, CInt(enVarColumn.BldNo), miaVarColumnsFieldNums(enVarColumn.BldNo))

		iColOrdinal = moMainRepTable.Columns.Item("BldPart").Ordinal
		miaVarColumnsFieldNums(enVarColumn.BldPart) = iAllColUB - iColOrdinal - 1
		DMCommon.Debug.ExcelLog.SetValue(7, "", iAllColUB, iColOrdinal, enVarColumn.BldPart, CInt(enVarColumn.BldPart), miaVarColumnsFieldNums(enVarColumn.BldPart))
		iColOrdinal = moMainRepTable.Columns.Item("EntranceDescr").Ordinal
		miaVarColumnsFieldNums(enVarColumn.BldEntr) = iAllColUB - iColOrdinal - 1
		DMCommon.Debug.ExcelLog.SetValue(14, "", iAllColUB, iColOrdinal, enVarColumn.BldEntr, CInt(enVarColumn.BldEntr), miaVarColumnsFieldNums(enVarColumn.BldEntr))
		iColOrdinal = moMainRepTable.Columns.Item("FloorDescr").Ordinal
		miaVarColumnsFieldNums(enVarColumn.BldFloor) = iAllColUB - iColOrdinal - 1
		DMCommon.Debug.ExcelLog.SetNextValue(21, "", iAllColUB, iColOrdinal, enVarColumn.BldFloor, CInt(enVarColumn.BldFloor), miaVarColumnsFieldNums(enVarColumn.BldFloor))

		miaVarColumnsFieldNums(enVarColumn.UserID) = iAllColUB



	End Sub
	Public Shared Function GetVarColumnsFieldNum() As Integer()
		Return miaVarColumnsFieldNums
	End Function

	Public Sub Paint()
		If Me.HasID Then
			zzPaintMainProperty()
			zzPaintSubProperty2009()
			zzPaintExpro()
		End If
	End Sub

	
	Private Sub zzPaintSubProperty2009()
		For iIndex As Integer = 0 To moSubproperties.RealCount - 1
			moSubproperties.PgonReal(iIndex).Paint()
		Next
	End Sub
	Private Sub zzPaintExpro()
		Dim iResp As Integer
		Dim iTopoID As Integer

		For iIndex As Integer = 0 To miExproCount - 1
			iTopoID = DirectCast(mvExpro(miSubPropColTopoID, iIndex), Integer)
			iResp = goTopoMaster.OpenPgon(iTopoID)
			If iResp = 0 Then zzPaintExproPgon()
		Next
		zzPaintExproPgon()
	End Sub

	Private Sub zzPaintExproPgon()
		Dim vvaBodyParms(1) As System.Object
		Dim vdaBorders(1, 0) As Double
		Dim vdaZebra(1, 1) As Double
		Dim iResp As Integer




		vvaBodyParms(0) = 0
		vvaBodyParms(1) = gdPrmZebraAngle
		If gbPrmExproZebra Then

			vdaZebra(0&, 0&) = 0&
			vdaZebra(1&, 0&) = gdPrmZebraWidth
			vdaZebra(0&, 1&) = RGB(255, 0, 0)
			vdaZebra(1&, 1&) = gdPrmZebraWidth
		Else
			vvaBodyParms(0&) = RGB(255, 0, 0)
		End If

		vdaBorders(0, 0) = miColorIndex
		vdaBorders(1, 0) = gdPrmDissolveBorderWidth
		iResp = goTopoMaster.SetActiveScale(gdPrmPaintScale)

		iResp = goTopoMaster.PaintPgon(vvaBodyParms, vdaBorders, vdaZebra)
		goTopoMaster.updateDisplay()

	End Sub

	Private Sub zzPaintMainProperty()
		For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
			oPropUnit.Paint()
		Next
	End Sub

	Public Sub New(ByVal iPropID As Integer)
		miPropID = iPropID
		mpgaBalcony = New bmPolygonArray
		miCurrentBldNo = -1
		miCurrentBldPart = -1
		miCurrentBldEntrance = -1
		mdicPropUnits = New Generic.SortedDictionary(Of bmPropUnitKey, bmPropUnit)()
		moSubproperties = New bmSubproperties()
	End Sub

	Public Sub SubNumerate(ByVal iCalcOption As bmBamash.SubNumerationOptions)
		Dim oBamashPgon As BamashPolygon
		Dim oBamashWhitePgon As BamashPolygon = Nothing
		Dim iColorPgonNum As Integer = 0

		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SubNum1", miPropID, moSubproperties.Count, iCalcOption)
		For iIndex As Integer = 0 To moSubproperties.Count - 1
			oBamashPgon = moSubproperties.Item(iIndex)
			If oBamashPgon.PropertyType = enPropertyTypes.UnderBldTypeWhite Then
				oBamashWhitePgon = oBamashPgon
			Else
				Select Case iCalcOption
					Case bmBamash.SubNumerationOptions.All
						oBamashPgon.SubPropNum = bmBamash.GetNext

					Case bmBamash.SubNumerationOptions.EmptyFirst, bmBamash.SubNumerationOptions.EmptyMax
						If oBamashPgon.SubPropNum = 0 Then
							oBamashPgon.SubPropNum = bmBamash.GetNextFree()
						End If
					Case bmBamash.SubNumerationOptions.None
						oBamashPgon.CheckSubPropNum()
						'DMCommon.Debug.ExcelLog.SetNextValue(2, "!SubPropNum", miPropID, oBamashPgon.SubPropNum)
				End Select
				If oBamashPgon.PropertyType = enPropertyTypes.UnderBldTypeColor Then
					iColorPgonNum = oBamashPgon.SubPropNum
				End If
			End If
		Next
		If oBamashWhitePgon IsNot Nothing AndAlso iColorPgonNum <> 0 Then
			oBamashWhitePgon.SubPropNum = iColorPgonNum
		End If

	End Sub
	Public Sub AddToPgonTable()
		For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
			oPropUnit.AddToPgonTable()
		Next
		For iIndex As Integer = 0 To moSubproperties.Count - 1
			moSubproperties(iIndex).AddDataToMainTable()
		Next
	End Sub
	Public ReadOnly Property PropUnitCount() As Integer
		Get
			Return mdicPropUnits.Count
		End Get
	End Property
	Public ReadOnly Property RepRowCount() As Integer
		Get
			Dim iRes As Integer = 0
			For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
				iRes += oPropUnit.RepRowCount
				If miPropID = 1 Then
					DMAcadExt.AcadDocument.WriteDebugMessage("19_369#" & CStr(oPropUnit.RepRowCount) & ":" & CStr(iRes))
				End If
			Next
			iRes = Math.Max(iRes, moSubproperties.RealCount)
			If miPropID = 1 Then
				DMAcadExt.AcadDocument.WriteDebugMessage("19_379#" & CStr(moSubproperties.RealCount) & ":" & CStr(iRes))
			End If
			If Me.PropUnitCount > 1 Then
				iRes += 1
			End If
			If miPropID = 1 Then
				DMAcadExt.AcadDocument.WriteDebugMessage("19_389#" & CStr(Me.PropUnitCount) & ":" & CStr(iRes))
			End If
			Return iRes
		End Get
	End Property
	Public Sub OutputData()
      Dim oaDataRow() As DataRow
		Dim sTest As String
		Dim iPropUnitIndex As Integer = 0
		Dim iPropUnitNum As Integer = 0
		Dim iApartBalconyIndex As Integer
		Dim iWarehouseIndex As Integer
		Dim pgaPgons As bmPolygonArray
		Dim iRowCount As Integer = Me.RepRowCount

		'	MessageBox.Show(CStr(iRowCountPropUnits) & ":" & CStr(miApartCount) & ":" & CStr(miBalconyCount) & ":" & CStr(iRowCountSubProp) & ":" & CStr(iRowCount))
		'	DMAcadExt.AcadDocument.WriteMessage("19_364#" & CStr(iRowCount) & ":" & CStr(mdicPropUnits.Count))
		Try

			If iRowCount = 0 Then Return
			ReDim oaDataRow(iRowCount - 1)

			For iIndex As Integer = 0 To iRowCount - 1
				oaDataRow(iIndex) = moMainRepTable.NewRow
			Next
			'	DMAcadExt.AcadDocument.WriteMessage(CStr(iRowCount) & ":" & CStr(mdicPropUnits.Count), "19_370")
			For Each oPropUnit As bmPropUnit In mdicPropUnits.Values
				'	iRowCountApartBalc = oPropUnit.ApartBalconyCount
				iApartBalconyIndex = iPropUnitIndex
				pgaPgons = oPropUnit.Apart
				sTest = "x" & CStr(iApartBalconyIndex)
				For iApartIndex As Integer = 0 To pgaPgons.Count - 1
					If iApartBalconyIndex <= iRowCount - 1 Then
						With oaDataRow(iApartBalconyIndex)
							If oPropUnit.BldNo <> 0 Then
								.Item("BldNo") = oPropUnit.BldNoText
							End If
							If oPropUnit.BldPart <> 0 Then
								.Item("BldPart") = oPropUnit.BldPart
							End If
							If oPropUnit.BldEntrance <> 0 Then
								.Item("EntranceDescr") = oPropUnit.EntranceDescr
							End If
							.Item("MainCaption") = Me.SubParcelNo
							.Item("FloorDescr") = DMCommon.Hebrew.ToDOS(oPropUnit.FloorDescr)
							.Item("FloorDescrWin") = oPropUnit.FloorDescr

							If iPropUnitNum = 0 Then
								.Item("ApartDescr") = pgaPgons(iApartIndex).ApartDescDOS
								.Item("ApartDescrWin") = pgaPgons(iApartIndex).ApartDesc

							End If
							If iApartIndex = 0 Then
								.Item("MainArea") = oPropUnit.MainArea
							End If

							'  MessageBox.Show(.Item("MainArea").ToString & vbCrLf & oPropUnit.MainArea.ToString())
						End With
						iApartBalconyIndex += 1
					End If
				Next
				pgaPgons = oPropUnit.Balcony
				sTest = "y" & CStr(iApartBalconyIndex)
				For iBalconyIndex As Integer = 0 To pgaPgons.Count - 1
					If iApartBalconyIndex <= iRowCount - 1 Then
						With oaDataRow(iApartBalconyIndex)
							If iApartBalconyIndex = 0 Then
								If oPropUnit.BldNo <> 0 Then
									.Item("BldNo") = oPropUnit.BldNoText
								End If

								If oPropUnit.BldPart <> 0 Then
									.Item("BldPart") = oPropUnit.BldPart
								End If

								If oPropUnit.BldEntrance <> 0 Then
									.Item("EntranceDescr") = oPropUnit.EntranceDescr
								End If
								.Item("FloorDescr") = DMCommon.Hebrew.ToDOS(oPropUnit.FloorDescr)
								.Item("FloorDescrWin") = oPropUnit.FloorDescr

							End If
							.Item("ApartDescr") = pgaPgons(iBalconyIndex).ApartDescDOS
							.Item("ApartDescrWin") = pgaPgons(iBalconyIndex).ApartDescExcel  'pgaPgons(iBalconyIndex).ApartDescRep

							.Item("MainCaption") = Me.SubParcelNo
							'	DMAcadExt.AcadDocument.WriteMessage("ApartDesc: " & pgaPgons(iBalconyIndex).ApartDescRep)
						End With
						iApartBalconyIndex += 1
					End If
				Next
				iWarehouseIndex = iPropUnitIndex
				pgaPgons = oPropUnit.Warehouse
				'	MessageBox.Show(CStr(pgaPgons.Count), "02_850")
				For iWarehouseRelIndex As Integer = 0 To pgaPgons.Count - 1
					With oaDataRow(iWarehouseRelIndex)
						.Item("ApartDescr") = pgaPgons(iWarehouseRelIndex).ApartDescDOS
						.Item("ApartDescrWin") = pgaPgons(iWarehouseRelIndex).ApartDesc
						.Item("WarehouseArea") = pgaPgons(iWarehouseRelIndex).Area
					End With
					iWarehouseIndex += 1
				Next
				iPropUnitIndex = Math.Max(iApartBalconyIndex, iWarehouseIndex)
				iPropUnitNum += 1
			Next oPropUnit
			sTest = "K"
			If miApartCount = 1 Then
				oaDataRow(0).Item("ApartDescr") = msApartDescrDOS
				oaDataRow(0).Item("ApartDescrWin") = msApartDescr

			End If
			sTest = "L"

			For iIndex As Integer = 0 To moSubproperties.RealCount - 1

				With oaDataRow(iIndex)
					.Item("MainCaption") = Me.SubParcelNo
					.Item("SubpropArea") = moSubproperties.PgonReal(iIndex).CalcArea
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!Spr_OutputData", miPropID, moSubproperties.Count, moSubproperties.PgonReal(iIndex).CaptionDOS, moSubproperties.PgonReal(iIndex).Caption)

					.Item("SubCaption") = moSubproperties.PgonReal(iIndex).CaptionDOS
					.Item("SubCaptionWin") = moSubproperties.PgonReal(iIndex).Caption

					.Item("SubDescr") = moSubproperties.PgonReal(iIndex).ApartDescDOS
					.Item("SubDescrWin") = moSubproperties.PgonReal(iIndex).ApartDesc

					If iIndex <= 10 Then

					End If
					oaDataRow(iIndex).Item("Color") = mtDMColor


				End With
			Next
			oaDataRow(0).Item("ColorName") = mtDMColor.HebColorName
			oaDataRow(0).Item("Color") = mtDMColor

			'	oaDataRow(0).Item("MainCaption") = dApartTotalArea
			If Me.PropUnitCount = 1 Then
				oaDataRow(0).Item("SumArea") = mdTotalArea
			Else
				oaDataRow(iRowCount - 1).Item("SumArea") = mdTotalArea
				oaDataRow(iRowCount - 1).Item("MainCaption") = Me.SubParcelNo
			End If
			For iIndex As Integer = 0 To iRowCount - 1
				moMainRepTable.Rows.Add(oaDataRow(iIndex))
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "bmProperty - OutputData")
		End Try
	End Sub
	Public ReadOnly Property MainArea() As Double
		Get
			Return mdMainArea
		End Get
	End Property
	Public Shared Sub AddCommonData()
		Const sSumLabel As String = "רכוש משותף"

		Dim oDataRow As DataRow = moMainRepTable.NewRow()
		With oDataRow
			.Item("ApartDescr") = DMCommon.Hebrew.InvertHeb(sSumLabel)  'FromDOS(msAprtDescDOS)
			.Item("ApartDescrWin") = sSumLabel 'FromDOS(msAprtDescDOS)

			.Item("MainCaption") = bmBamash.Parcel & "\0"
			.Item("Color") = New DMAcadExt.DMColor()
		End With
		moMainRepTable.Rows.Add(oDataRow)
	End Sub
	Public ReadOnly Property TotalArea() As Double
		Get
			Return mdTotalArea
		End Get
	End Property

	Public ReadOnly Property MainRepTable() As DataTable
		Get
			Return moMainRepTable
		End Get
	End Property


	Public ReadOnly Property HasID() As Boolean
		Get
			Return miPropID <> 0
		End Get
	End Property


	Public Property PropID() As Integer
		Get
			Return miPropID
		End Get
		Set(ByVal iValue As Integer)
			miPropID = iValue
		End Set
	End Property

End Class
