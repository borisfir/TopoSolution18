
Option Explicit On
Option Strict On
Namespace TPlanGraph


	Public Class TplnExproType
		Private mdicLandusesAppr As TplnLanduses
		Private miParcelID As Integer
		Private miExproTypeID As Integer
		Private msExproTypeName As String
		Private mdAcadArea As Double
		Private mdCalcArea As Double
		Private mdRoundedArea As Double


		Public Sub New(iExproTypeID As Integer, iParcelID As Integer)
			miExproTypeID = iExproTypeID
			miParcelID = iParcelID
			mdicLandusesAppr = New TplnLanduses(DMAcadExt.enTopoPurpose.Approved)
		End Sub


		Public Sub AddExproLusePolygon(iLanduseID As Integer, iBasicPlanID As Integer, dAcadArea As Double)
			Dim oExproTypeLanduse As TplnLanduse = Nothing
			'	Dim tAreaset As TplnAreaSet
			If Not mdicLandusesAppr.TryGetValue(iLanduseID, oExproTypeLanduse) Then
				'DMCommon.Debug.ExcelLog.SetValueByCond(0, "2_AddExproLusePgon", True, mdicLandusesAppr IsNot Nothing, "mdicLandusesAppr IsNot Nothing", mdicLandusesAppr.Count)
				oExproTypeLanduse = New TplnLanduse(iLanduseID, DMAcadExt.enTopoPurpose.Approved)
				oExproTypeLanduse.Name = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, iLanduseID)
				'	oExproTypeLanduse.AreaSet = New TplnAreaSet(dAcadArea)

				mdicLandusesAppr.Add(iLanduseID, oExproTypeLanduse)
				'DMCommon.Debug.ExcelLog.SetValueByCond(0, "3_AddExproLusePgon", True, mdicLandusesAppr IsNot Nothing, "mdicLandusesAppr IsNot Nothing", mdicLandusesAppr.Count)
			End If
			'DMCommon.Debug.MsgBoxLoop("B01_01", 3, iLanduseID, iBasicPlanID, dAcadArea)
			oExproTypeLanduse.AddArea(dAcadArea)
			oExproTypeLanduse.AddBasicPlanArea(iBasicPlanID, dAcadArea)

			mdAcadArea += dAcadArea
		End Sub
		Public Sub AddExproLusePolygon(iLanduseID As Integer, dAcadArea As Double)
			'DMCommon.Debug.ExcelLog.SetValueByCond(0, "!AddExproLusePgon", True, oLanduse IsNot Nothing, oOverlayPgon Is Nothing, oLanduse.ID, DMCommon.Functions.CStrN(oLanduse.Name, "2LuseIsNth"))
			'DMCommon.Debug.ExcelLog.SetValueByCond(0, "?AddExproLusePgon", True, mdicLandusesAppr IsNot Nothing, "mdicLandusesAppr IsNot Nothing", mdicLandusesAppr.Count)
			Dim oExproTypeLanduse As TplnLanduse = Nothing
			'	Dim tAreaset As TplnAreaSet
			If Not mdicLandusesAppr.TryGetValue(iLanduseID, oExproTypeLanduse) Then
				'DMCommon.Debug.ExcelLog.SetValueByCond(0, "2_AddExproLusePgon", True, mdicLandusesAppr IsNot Nothing, "mdicLandusesAppr IsNot Nothing", mdicLandusesAppr.Count)
				oExproTypeLanduse = New TplnLanduse(iLanduseID, DMAcadExt.enTopoPurpose.Approved)
				oExproTypeLanduse.Name = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, iLanduseID)
				'	oExproTypeLanduse.AreaSet = New TplnAreaSet(dAcadArea)

				mdicLandusesAppr.Add(iLanduseID, oExproTypeLanduse)
				'DMCommon.Debug.ExcelLog.SetValueByCond(0, "3_AddExproLusePgon", True, mdicLandusesAppr IsNot Nothing, "mdicLandusesAppr IsNot Nothing", mdicLandusesAppr.Count)
			End If
			oExproTypeLanduse.AddArea(dAcadArea)
			mdAcadArea += dAcadArea

			If miParcelID = 15003 Then

				DMCommon.Debug.ExcelLog.SetNextValue(0, "!!AddExproLusePgon", miParcelID, oExproTypeLanduse.Name, oExproTypeLanduse.Name, dAcadArea, oExproTypeLanduse.AreaSet.AcadArea, mdAcadArea)

			End If


		End Sub
		Public Sub TestLanduses()
			For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
				If oLanduse Is Nothing Then

					DMCommon.Debug.MsgBox("!040421", mdicLandusesAppr.Count, miParcelID, miExproTypeID)
				End If
			Next

		End Sub

		Public ReadOnly Property AcadArea As Double
			Get
				Return mdAcadArea
			End Get
		End Property
		Public Property CalcArea As Double
			Get
				Return mdCalcArea
			End Get
			Set(dValue As Double)
				mdCalcArea = dValue
			End Set
		End Property
		Public Property RoundedArea As Double
			Get
				Return mdRoundedArea
			End Get
			Set(dValue As Double)
				mdRoundedArea = dValue
			End Set
		End Property
		Public ReadOnly Property ExproTypeID As Integer
			Get
				Return miExproTypeID
			End Get
		End Property
		Public ReadOnly Property ExproTypeName As String
			Get
				Return msExproTypeName
			End Get
		End Property
		Public ReadOnly Property Landuses As TplnLanduses
			Get
				Return mdicLandusesAppr
			End Get

		End Property
		Public ReadOnly Property LanduseArray As Integer()
			Get
				Dim iIndex As Integer = 0
				Dim oRes(mdicLandusesAppr.Count - 1) As Integer
				If False Then
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						oRes(iIndex) = oLanduse.ID
						iIndex += 1

					Next

					iIndex += 1
				End If
				oRes = mdicLandusesAppr.Keys.ToArray()
				Return oRes
			End Get

		End Property
		Public ReadOnly Property LanduseList As String
			Get

				Dim oRes(mdicLandusesAppr.Count - 1) As Integer
				Dim sRes As String = String.Empty
				oRes = mdicLandusesAppr.Keys.ToArray()
				For iIndex As Integer = 0 To oRes.GetUpperBound(0)
					If iIndex = 0 Then
						sRes = oRes(0).ToString()

					Else
						sRes &= "," & oRes(iIndex).ToString()
					End If

				Next
				Return sRes
			End Get

		End Property

		Public ReadOnly Property LandusesCount As Integer
			Get
				If mdicLandusesAppr Is Nothing Then
					Return -1
				Else
					Return mdicLandusesAppr.Count
				End If

			End Get
		End Property

		Public Function TryGetValue(iLanduseID As Integer, ByRef oLanduse As TplnLanduse) As Boolean
			Return mdicLandusesAppr.TryGetValue(iLanduseID, oLanduse)
		End Function
		Public Sub CalculateArea()
			Dim iArrayUB As Integer = -1
			Dim iAreaIndex As Integer = 0
			Dim dInputSum As Double
			Dim bLanduseExists As Boolean
			iArrayUB = mdicLandusesAppr.Count - 1
			Dim daInput(iArrayUB) As Double

			For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
				'DMCommon.Debug.ExcelLog.SetNextValue(1, "!oLanduse.AreaSet.AcadArea:", iArrayUB, oLanduse.AreaSet.AcadArea)
				If oLanduse.AreaSet.AcadArea > 0 Then
					daInput(iAreaIndex) = oLanduse.AreaSet.AcadArea
					dInputSum += daInput(iAreaIndex)
					'DMCommon.Debug.ExcelLog.SetNextValue(1, "!oLanduse.AreaSet.AcadArea:", iArrayUB, dInputSum, oLanduse.AreaSet.AcadArea, TplnProject.CalcRoundFactor, mdCalcArea)
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!NB_Expro", oLanduse.AreaSet.AcadArea)
					iAreaIndex += 1
				End If
			Next
			bLanduseExists = iAreaIndex > 0
			If bLanduseExists Then
				ReDim Preserve daInput(iAreaIndex - 1)
				Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, mdCalcArea, True, "")    '   temp 
				Dim oBalanceRoundedArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, mdRoundedArea, True, "")
				Dim tLanduseAreaset As TplnAreaSet
				'Dim daPart() As Double
				iAreaIndex = 0
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					tLanduseAreaset = oLanduse.AreaSet
					If oLanduse.AreaSet.AcadArea > 0 Then
						tLanduseAreaset.CalcArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
						daInput(iAreaIndex) = tLanduseAreaset.CalcArea * 100.0 / mdCalcArea

						tLanduseAreaset.RoundedArea = oBalanceRoundedArea.OutputItemFix(iAreaIndex)
						oLanduse.AreaSet = tLanduseAreaset
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproLotLU", mdicLandusesAppr.Count, oLanduse.Name, oLanduse.AreaSet.CalcArea)
						iAreaIndex += 1
					End If

				Next
				Dim oBalanceParts As BalanceArea = New BalanceArea(daInput, 10.0, 100.0, True, "")
				iAreaIndex = 0
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					tLanduseAreaset = oLanduse.AreaSet
					If oLanduse.AreaSet.AcadArea > 0 Then
						tLanduseAreaset.Part = oBalanceParts.OutputItemFix(iAreaIndex) * 0.1
						oLanduse.AreaSet = tLanduseAreaset
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproLotLU", mdicLandusesAppr.Count, oLanduse.Name, oLanduse.AreaSet.CalcArea)
						iAreaIndex += 1
					End If

				Next
			End If

		End Sub

		Public Sub CalculateArea_310520()   '_310520
			Dim iArrayUB As Integer = -1
			Dim iAreaIndex As Integer = 0
			Dim dInputSum As Double

			iArrayUB = mdicLandusesAppr.Count - 1
			Dim daInput(iArrayUB) As Double

			For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
				DMCommon.Debug.ExcelLog.SetNextValue(1, "!oLanduse.AreaSet.AcadArea:", iArrayUB, oLanduse.AreaSet.AcadArea)
				daInput(iAreaIndex) = oLanduse.AreaSet.AcadArea

				dInputSum += daInput(iAreaIndex)
				DMCommon.Debug.ExcelLog.SetNextValue(1, "!oLanduse.AreaSet.AcadArea:", iArrayUB, dInputSum, oLanduse.AreaSet.AcadArea, TplnProject.CalcRoundFactor, mdCalcArea)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!NB_Expro", oLanduse.AreaSet.AcadArea)
				iAreaIndex += 1
			Next

			'	DMCommon.Debug.ExcelLog.SetNextValue(3, "!Luse1:", miParcelID, iArrayUB, dInputSum)
			'	DMCommon.Debug.ExcelLog.SetNextValue(3, "!Luse2:", DMCommon.Debug.GetListArrayA(daInput))


			Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, mdCalcArea, True, "")    '   temp 
			Dim oBalanceRoundedArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, mdRoundedArea, True, "")
			Dim tLanduseAreaset As TplnAreaSet
			'Dim daPart() As Double
			iAreaIndex = 0
			For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
				tLanduseAreaset = oLanduse.AreaSet
				tLanduseAreaset.CalcArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
				daInput(iAreaIndex) = tLanduseAreaset.CalcArea * 100.0 / mdCalcArea

				tLanduseAreaset.RoundedArea = oBalanceRoundedArea.OutputItemFix(iAreaIndex)
				oLanduse.AreaSet = tLanduseAreaset
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproLotLU", mdicLandusesAppr.Count, oLanduse.Name, oLanduse.AreaSet.CalcArea)
				iAreaIndex += 1
			Next
			Dim oBalanceParts As BalanceArea = New BalanceArea(daInput, 10.0, 100.0, True, "")
			iAreaIndex = 0
			For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
				tLanduseAreaset = oLanduse.AreaSet
				tLanduseAreaset.Part = oBalanceParts.OutputItemFix(iAreaIndex) * 0.1
				oLanduse.AreaSet = tLanduseAreaset
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproLotLU", mdicLandusesAppr.Count, oLanduse.Name, oLanduse.AreaSet.CalcArea)
				iAreaIndex += 1
			Next

			'	DMCommon.Debug.ExcelLog.SetNextValue(3, "!Luse3:", DMCommon.Debug.GetListArray(oBalanceCalcArea.Output))
		End Sub

	End Class
End Namespace