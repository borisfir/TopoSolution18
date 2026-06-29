Option Explicit On
Option Strict On
Namespace TPlanGraph
	Public Structure LanduseData
		Dim LanduseID As Integer
		Dim LanduseName As String
		Dim ColorSchemeID As Integer
		Dim LuseColorScheme As DMAcadExt.ColorScheme
	End Structure
	Public Class TplnLanduse
		Private miID As Integer
		Private msName As String
		Private miOrder As Integer
		' Private mdCalcArea As Double
		Private mdSumArea As Double
		Private moaUnionMergePgons() As TplnOverlayPgon = Nothing
		Private moaUnionUnionPgons() As TplnOverlayPgon = Nothing
		Private miUnionMergePgonCounter As Integer = -1
		Private miUnionUnionPgonCounter As Integer = -1

		Private miTopoPurpose As DMAcadExt.enTopoPurpose
		Private mbPlanStatusDefined As Boolean = False

		Private mdAcadAreaProposed As Double
		Private mdAcadAreaApproved As Double

		Private mdCalcAreaProposed As Double
		Private mdCalcAreaApproved As Double
		Private mdRndAreaApproved As Double

		Private mdicBasicPlanAreas As Dictionary(Of Integer, Double)


		Dim mtApprAreaSet As TplnAreaSet	 '--->obsolete
		Dim mtPropAreaSet As TplnAreaSet	 '--->obsolete

		Private mtaAreaSet(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnAreaSet

      Private mtApprInPlanAreaSet As TplnAreaSet
      Private mtPropInPlanAreaSet As TplnAreaSet

      Private mtaInPlanAreaSet(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnAreaSet

		Private mtAreaSet As TplnAreaSet

		Private mtaRegionAreaSet(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnAreaSet


		Private moCalcArea As UnionPgonArea = New UnionPgonArea
		Private mbInPlanAppr As Boolean
		Private mbInPlanProp As Boolean

		Private mdPercentProposed As Double
		Private mdPercentApproved As Double
		'	Private mdicLots As TplnLots
		Private mdicLotsAppr As TPlanGraph.TplnLots
		Private mdicLotsProp As TPlanGraph.TplnLots
		Private mdicLots As TPlanGraph.TplnLots

		'		Private mlistLotsAppr As List(Of Integer) ' for summary
		'		Private mlistLotsProp As List(Of Integer) ' for summary
		'		Private mlistLots As List(Of Integer) ' for Parcel
		Private mlistLotsAppr As List(Of LotIn) ' for summary
      Private mlistLotsProp As List(Of LotIn)   ' for summary
      Private mhsRegionsAppr As HashSet(Of Integer)
      Private mhsRegionsProp As HashSet(Of Integer)

      '	Private mlistLots As List(Of LotIn)	' for Parcel
		Private mlistaLots(DMAcadExt.enOverlayIndex.OverlayIndexUB) As List(Of LotIn)

		Private mtColorSchemeAppr As DMAcadExt.ColorScheme
		Private mtColorSchemeProp As DMAcadExt.ColorScheme
		Private miOrderAppr As Integer
		Private miOrderProp As Integer
		Private msNameAppr As String
		Private msNameProp As String


		Private miParcelID As Integer

		Private Shared mdicColorSetAppr As IDictionary(Of Integer, LanduseItem)
		Private Shared mdicColorSetProp As IDictionary(Of Integer, LanduseItem)

		Private Shared mdicColorSchemesAppr As IDictionary(Of Integer, DMAcadExt.ColorScheme)
		Private Shared mdicColorSchemesProp As IDictionary(Of Integer, DMAcadExt.ColorScheme)
		Public Shared Function GetColorSchemeDic(iTopoPurpose As DMAcadExt.enTopoPurpose) As IDictionary(Of Integer, DMAcadExt.ColorScheme)
			'DEBUG	MessageBox.Show(CStr(mdicColorSchemesAppr.Count) & vbCrLf & iTopoPurpose.ToString(), "05_240")
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				Return mdicColorSchemesAppr
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				Return mdicColorSchemesProp
			Else
				Return Nothing
			End If
		End Function
		

		'	Set(dicValue As IDictionary(Of Integer, DMAcadExt.ColorScheme,iToiTopoPurpose As DMAcadExt.enTopoPurpose))
		'			mdicColorSchemesAppr = dicValue
		'		End Set



		Public Property ID() As Integer
			Get
				Return miID
			End Get
			Set(ByVal iValue As Integer)
				miID = iValue
			End Set
		End Property
		Public Property Name() As String
			Get
				Return msName
			End Get
			Set(ByVal sValue As String)
				msName = sValue
			End Set
		End Property
		Public Property Order() As Integer
			Get
				Return miOrder
			End Get
			Set(ByVal iValue As Integer)
				miOrder = iValue
			End Set
		End Property
		Public Property AreaSet() As TplnAreaSet
			Get
				Return mtAreaSet
			End Get
			Set(tValue As TplnAreaSet)
				mtAreaSet = tValue
			End Set
		End Property

		Public Sub AddArea(dAcadArea As Double)
			mtAreaSet.AcadArea += dAcadArea
		End Sub
		Public Sub AddBasicPlanArea(iBasicPlanID As Integer, dAcadArea As Double)
			Dim dPrevArea As Double = 0
			If mdicBasicPlanAreas Is Nothing Then
				mdicBasicPlanAreas = New Dictionary(Of Integer, Double)
			End If
			mtAreaSet.AcadArea += dAcadArea
			'DMCommon.Debug.MsgBoxLoop("B01_03", 3, miParcelID, ID, iBasicPlanID, dAcadArea)
			If mdicBasicPlanAreas.TryGetValue(iBasicPlanID, dPrevArea) Then
				mdicBasicPlanAreas.Remove(iBasicPlanID)

			End If
			mdicBasicPlanAreas.Add(iBasicPlanID, dPrevArea + dAcadArea)


		End Sub
		Public Sub AddLot(ByVal oLot As TplnLot)

			If oLot.TopoPurpose = enTopoPurpose.Approved Then
				mdicLotsAppr.AddLot(oLot)
				Try
					mlistLotsAppr.Add(New LotIn(oLot.TopoID, oLot.InPlan, oLot.RegionNo))
				Catch oEx As Exception
					DMCommon.Debug.MsgBoxLoop("L027", oLot.TopoID, oLot.Name, oLot.InPlan, oLot.RegionNo)
				End Try

				If Not mhsRegionsAppr.Contains(oLot.RegionNo) Then
               mhsRegionsAppr.Add(oLot.RegionNo)
            End If
				If oLot.InPlan Then
					mbInPlanAppr = True
				End If


			ElseIf oLot.TopoPurpose = enTopoPurpose.Proposed Then
            mdicLotsProp.AddLot(oLot)
            mlistLotsProp.Add(New LotIn(oLot.TopoID, oLot.InPlan, oLot.RegionNo))
				If Not mhsRegionsProp.Contains(oLot.RegionNo) Then
					mhsRegionsProp.Add(oLot.RegionNo)
				End If
				If oLot.InPlan Then
					mbInPlanProp = True
				End If
			End If

      End Sub

		Public Sub AddLot(iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iLotTopoID As Integer, bInPlan As Boolean, iRegion As Integer)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Lu AddLot2", iOverlayIndex, iLotTopoID, bInPlan, iRegion)

			If mlistaLots(iOverlayIndex) Is Nothing Then

				mlistaLots(iOverlayIndex) = New List(Of LotIn)()
			End If

			mlistaLots(iOverlayIndex).Add(New LotIn(iLotTopoID, bInPlan, iRegion))
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex)
			Dim hsRegions As HashSet(Of Integer) = Nothing
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					hsRegions = mhsRegionsAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					hsRegions = mhsRegionsProp
				Case Else

			End Select
			If hsRegions IsNot Nothing Then

				If Not hsRegions.Contains(iRegion) Then
					hsRegions.Add(iRegion)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("hsRegions IsNot Nothing", "05_742")
			End If
		End Sub

		Public Function ContainRegion(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, iRegion As Integer) As Boolean
         Select Case iTopoPurpose
            Case DMAcadExt.enTopoPurpose.Approved
               Return mhsRegionsAppr.Contains(iRegion)
            Case DMAcadExt.enTopoPurpose.Proposed
               Return mhsRegionsProp.Contains(iRegion)
            Case Else
               Return False
         End Select

      End Function
      Public Function RegionCount(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Integer
         Select Case iTopoPurpose
            Case DMAcadExt.enTopoPurpose.Approved
               Return mhsRegionsAppr.Count
            Case DMAcadExt.enTopoPurpose.Proposed
               Return mhsRegionsProp.Count
            Case Else
               Return 0
         End Select

      End Function
      Public Sub Calculate(ByVal bByPoligons As Boolean, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)
			'temp for expro

			If miParcelID = 0 Then
				'	TplnProject.WriteMessageBox(CStr(bByPoligons) & ":" & iOverlayMethod.ToString(), "01_542")
				'	TplnProject.WriteMessageBox(CStr(mlistLotsAppr.Count) & ":" & mlistLotsProp.Count.ToString(), "01_542x")
				If Me.HasLots(DMAcadExt.enTopoPurpose.Approved) Then

					zzCalcByLotList(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved, mlistLotsAppr)
				End If

				If Me.HasLots(DMAcadExt.enTopoPurpose.Proposed) Then
					zzCalcByLotList(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed, mlistLotsProp)
				End If

				'''''''''''''''''''''''''''''''''	zzCalcByLotList(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed, mlistLotsProp)
			ElseIf bByPoligons Then
				zzCalcByPolygons(iOverlayMethod)
			Else

				zzCalcByOverlayGroups(iOverlayMethod)
         End If
      End Sub
      Public Sub Calculate(ByVal bByPoligons As Boolean, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer)
			'temp for expro
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "TplnLanduse", "Calculate", miParcelID, bByPoligons, iOverlayMethod, iRegion, iOverlayMethod, miTopoPurpose, miID, msName)
			If miParcelID = 0 Then
				'	TplnProject.WriteMessageBox(CStr(bByPoligons) & ":" & iOverlayMethod.ToString(), "01_542")
				'	TplnProject.WriteMessageBox(CStr(mlistLotsAppr.Count) & ":" & mlistLotsProp.Count.ToString(), "01_542x")
				zzCalcByLotList(iOverlayMethod, DMAcadExt.enTopoPurpose.Approved, mlistLotsAppr, iRegion)
				'''''''''''''''''''''''''''''''''	zzCalcByLotList(iOverlayMethod, DMAcadExt.enTopoPurpose.Proposed, mlistLotsProp)
			ElseIf bByPoligons Then
				zzCalcByPolygons(iOverlayMethod)
			Else
				zzCalcRegionByOverlayGroups(miTopoPurpose, iOverlayMethod, iRegion)
			End If

		End Sub



		Private Sub zzCalcByLotList(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal listLots As List(Of LotIn))

			' For  ParcelID = 0 - entire plan
			Dim oLot As TplnLot
			Dim dAcadAreaSum As Double = 0.0
			Dim dCalcAreaSum As Double = 0.0
			Dim tInPlanAreaSet As TplnAreaSet
			Dim tAreaSet As TplnAreaSet
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)

			If listLots IsNot Nothing Then
				For Each tLotIn As LotIn In listLots

					oLot = TplnProject.GetLot(iTopoPurpose, tLotIn.LotTopoID)

					tAreaSet.Add(oLot.InPlanAreaSet(iOverlayIndex))

					If tLotIn.InPlan Then
						'	TplnProject.WriteMessageBox(CStr(oLot.AcadArea(False)) & ":" & oLot.CalcArea(iOverlayMethod).ToString(), "01_520z")
						dAcadAreaSum += oLot.AcadArea(False)

						dCalcAreaSum += oLot.CalcArea(iOverlayMethod)

						tInPlanAreaSet.Add(oLot.AreaSet(iOverlayIndex))

					End If
				Next

			Else
				System.Windows.Forms.MessageBox.Show("Err #1257", "TplnProject - zzCalcByLotList")
         End If


			zzSetArea(iOverlayIndex, tAreaSet, tInPlanAreaSet)
      End Sub

		Private Sub zzCalcByLotList(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal listLots As List(Of LotIn), iRegion As Integer)

			' For  ParcelID = 0 - entire plan
			Dim oLot As TplnLot
			Dim dAcadAreaSum As Double = 0.0
			Dim dCalcAreaSum As Double = 0.0
			Dim tInPlanAreaSet As TplnAreaSet
			Dim tAreaSet As TplnAreaSet
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			'	TplnProject.WriteMessageBox(iOverlayMethod.ToString() & ":" & iOverlayIndex.ToString(), "01_520s")
			If listLots IsNot Nothing Then

				'TplnProject.WriteMessageBox(listLots.Count.ToString() & ":" & "#", "01_521x")
				For Each tLotIn As LotIn In listLots
					If tLotIn.Region = iRegion Then
						oLot = TplnProject.GetLot(iTopoPurpose, tLotIn.LotTopoID)
						tAreaSet.Add(oLot.InPlanAreaSet(iOverlayIndex))
						If tLotIn.InPlan Then
							'	TplnProject.WriteMessageBox(CStr(oLot.AcadArea(False)) & ":" & oLot.CalcArea(iOverlayMethod).ToString(), "01_520z")
							dAcadAreaSum += oLot.AcadArea(False)
							dCalcAreaSum += oLot.CalcArea(iOverlayMethod)
							tInPlanAreaSet.Add(oLot.AreaSet(iOverlayIndex))
						End If
					End If

				Next

			Else
				System.Windows.Forms.MessageBox.Show("Err #1257", "TplnProject - zzCalcByLotList")
			End If


			zzSetArea(iOverlayIndex, tAreaSet, tInPlanAreaSet)
		End Sub
		Public Property CalcAreaApproved As Double
			Get
				Return mdCalcAreaApproved
			End Get
			Set(dValue As Double)
				mdCalcAreaApproved = dValue
			End Set
		End Property
		Public Property RndAreaApproved As Double
			Get
				Return mdRndAreaApproved
			End Get
			Set(dValue As Double)
				mdRndAreaApproved = dValue
			End Set
		End Property




		Private Sub zzSetArea(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dAcadAreaSum As Double, ByVal dCalcAreaSum As Double)
        
         If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
            mdAcadAreaApproved = dAcadAreaSum
            mdCalcAreaApproved = dCalcAreaSum
         ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
            mdAcadAreaProposed = dAcadAreaSum
            mdCalcAreaProposed = dCalcAreaSum
         End If
      End Sub
      Private Sub zzSetArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal tAreaSet As TplnAreaSet, ByVal tInPlanAreaSet As TplnAreaSet)
       
         mtaAreaSet(iOverlayIndex) = tAreaSet
         mtaInPlanAreaSet(iOverlayIndex) = tInPlanAreaSet


      End Sub
      Private Sub zzSetRegionArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal tRegionAreaSet As TplnAreaSet)

         mtaRegionAreaSet(iOverlayIndex) = tRegionAreaSet


      End Sub
      Public Sub CalcByOverlayGroups(iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer)
         If iRegion = 0 Then
            zzCalcByOverlayGroups(iOverlayMethod)
         Else
            zzCalcRegionByOverlayGroups(iTopoPurpose, iOverlayMethod, iRegion)
         End If

      End Sub
      Private Sub zzCalcByOverlayGroups(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)
         ' For  ParcelID <> 0 , miTopoPurpose = DMAcadExt.enTopoPurpose.Approved   or DMAcadExt.enTopoPurpose.Proposed
         '    MessageBox.Show("zzCalcByOverlayGroups", "01_833t")
         Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, miTopoPurpose)
         '	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & miTopoPurpose.ToString(), "05_100")
         '	System.Windows.Forms.MessageBox.Show(CStr(mlistLotsAppr Is Nothing) & vbCrLf & CStr(mlistLotsProp Is Nothing) & vbCrLf & CStr(mlistLots Is Nothing), "05_110")

         Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
         If oAllOverlayGroups IsNot Nothing Then
            Dim oOverlayGroup As TplnOverlayGroup
            Dim dAcadAreaSum As Double = 0.0
            Dim dCalcAreaSum As Double = 0.0
            Dim tAreaSet As TplnAreaSet
            Dim tInPlanAreaSet As TplnAreaSet
            '  Dim tRegionAreaSet As TplnAreaSet


            Dim listLots As List(Of LotIn) = mlistaLots(iOverlayIndex)
            If listLots IsNot Nothing AndAlso oAllOverlayGroups IsNot Nothing Then
               For Each tLotIn As LotIn In listLots
                  oOverlayGroup = oAllOverlayGroups.GetItem(miParcelID, tLotIn.LotTopoID)
                  If oOverlayGroup IsNot Nothing Then
                     tAreaSet.Add(oOverlayGroup.AreaSet)
                     If tLotIn.InPlan Then
                        dAcadAreaSum += oOverlayGroup.AcadArea
                        dCalcAreaSum += oOverlayGroup.CalcArea
                        tInPlanAreaSet.Add(oOverlayGroup.AreaSet)
                     End If
                  End If
                  



               Next

               '  DMCommon.ExcelLog.SetNextValue(i, 15, "zzSetArea023", iOverlayIndex.ToString(), tAreaSet.AcadArea, tInPlanAreaSet.AcadArea)
               zzSetArea(iOverlayIndex, tAreaSet, tInPlanAreaSet)


            Else
               System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(miID) & ":" & msName & vbCrLf & CStr(miParcelID), "05_238")
            End If

         Else
            System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "05_188")
         End If


         '	zzSetArea(miTopoPurpose, dAcadAreaSum, dCalcAreaSum)


      End Sub
      Private Sub zzCalcRegionByOverlayGroups(iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer)
			' For  ParcelID <> 0 , miTopoPurpose = DMAcadExt.enTopoPurpose.Approved   or DMAcadExt.enTopoPurpose.Proposed

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)

			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "zzCalcRegionByOverlayGroups#1", iTopoPurpose, iOverlayMethod, iRegion, iOverlayIndex, iOverlayMethod, iTopoPurpose)
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "M#02")
			If oAllOverlayGroups IsNot Nothing Then
            Dim oOverlayGroup As TplnOverlayGroup
            Dim dAcadAreaSum As Double = 0.0
            Dim dCalcAreaSum As Double = 0.0
            '  Dim tAreaSet As TplnAreaSet
            ' Dim tInPlanAreaSet As TplnAreaSet
            Dim tRegionAreaSet As TplnAreaSet


            Dim listLots As List(Of LotIn) = mlistaLots(iOverlayIndex)
            If listLots IsNot Nothing Then
               For Each tLotIn As LotIn In listLots
                  oOverlayGroup = oAllOverlayGroups.GetItem(miParcelID, tLotIn.LotTopoID)
                 

                  If tLotIn.Region = iRegion Then
                     tRegionAreaSet.Add(oOverlayGroup.AreaSet)
                     '1952
                  End If

               Next
					zzSetRegionArea(iOverlayIndex, tRegionAreaSet)


				Else
					'TEMP System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(miID) & ":" & msName & vbCrLf & CStr(miParcelID), "05_233")

					DMCommon.Debug.ExcelLog.SetNextValue(0, "zzCalcRegionByOverlayGroups#2", iTopoPurpose, iOverlayMethod, iRegion, iOverlayIndex, iOverlayMethod, iTopoPurpose, miID, msName, miParcelID)
				End If

         Else
            System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "05_188")
         End If


         '	zzSetArea(miTopoPurpose, dAcadAreaSum, dCalcAreaSum)


      End Sub
		Private Sub zzCalcByPolygons(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)
			MessageBox.Show("zzCalcByPolygons", "01_822s")
			' For  ParcelID <> 0 , miTopoPurpose = DMAcadExt.enTopoPurpose.Approved   or DMAcadExt.enTopoPurpose.Proposed
			Dim oaUnionPgons() As TplnOverlayPgon = zzUnionPgons(iOverlayMethod)
			Dim dAcadAreaSum As Double = 0.0
			Dim dCalcAreaSum As Double = 0.0

			If oaUnionPgons IsNot Nothing Then
				For iIndex As Integer = 0 To oaUnionPgons.GetUpperBound(0)
					dAcadAreaSum += oaUnionPgons(iIndex).AcadArea(False)
					dCalcAreaSum += oaUnionPgons(iIndex).CalcArea()
				Next
				zzSetArea(miTopoPurpose, dAcadAreaSum, dCalcAreaSum)
			Else
				System.Windows.Forms.MessageBox.Show("Err #1227", "TplnLanduse - zzCalcByPolygons")
			End If
		End Sub


      Public ReadOnly Property AreaSetAAA(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TplnAreaSet
         Get
            If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
               Return mtApprAreaSet
            ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
               Return mtPropAreaSet
            End If
         End Get
      End Property

      Public ReadOnly Property InPlanAreaSet(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
         Get
            Return mtaInPlanAreaSet(iOverlayIndex)

         End Get
      End Property
      Public ReadOnly Property RegionAreaSet(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
         Get
            Return mtaRegionAreaSet(iOverlayIndex)

         End Get
      End Property

	
		Public Property AcadAreaProposed() As Double
			Get
				Return mdAcadAreaProposed
			End Get
			Set(ByVal dValue As Double)
				mdAcadAreaProposed = dValue
			End Set
		End Property
		Public Property AcadAreaApproved() As Double
			Get
				Return mdAcadAreaApproved
			End Get
			Set(ByVal dValue As Double)
				mdAcadAreaApproved = dValue
			End Set
		End Property

		Public ReadOnly Property BasicPlanAreas() As Dictionary(Of Integer, Double)
			Get

				Return mdicBasicPlanAreas

			End Get
		End Property



		Public ReadOnly Property Lots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TPlanGraph.TplnLots
			Get
				If iTopoPurpose = enTopoPurpose.Proposed Then
					Return mdicLotsProp
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					Return mdicLotsAppr
				Else
					System.Windows.Forms.MessageBox.Show("Err #1243", "TplnLanduse - zzLots")
					Return Nothing
				End If
			End Get
		End Property

		Public Function GetColorSchemeInfo(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef iColorSchemeID As Integer, ByRef sColorSchemeName As String, ByRef iColorSchemeOrder As Integer) As Boolean
			'Dim sRes1 As String = Nothing
			Dim bRes As Boolean
			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
			Dim iTest As Integer = -1
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					If mdicColorSchemesAppr IsNot Nothing AndAlso mdicColorSchemesAppr.TryGetValue(miID, tColorScheme) Then
						iColorSchemeID = miID
						sColorSchemeName = tColorScheme.Name

						iColorSchemeOrder = tColorScheme.Order
						bRes = True
						iTest = mdicColorSchemesAppr.Count
					Else
						'MessageBox.Show(CStr(miID) & ":" & CStr(mdicColorSchemesAppr.Count), "06_122")
						iColorSchemeID = Me.ID
						sColorSchemeName = iColorSchemeID.ToString()
						iColorSchemeOrder = 0
						bRes = False
					End If

				Case DMAcadExt.enTopoPurpose.Proposed
					If mdicColorSchemesProp IsNot Nothing AndAlso mdicColorSchemesProp.TryGetValue(miID, tColorScheme) Then
						iColorSchemeID = miID
						sColorSchemeName = tColorScheme.Name
						iColorSchemeOrder = tColorScheme.Order

						bRes = True
					Else
						iColorSchemeID = Me.ID
						sColorSchemeName = iColorSchemeID.ToString()
						iColorSchemeOrder = 0
						bRes = False
					End If
				Case Else
					If msName IsNot Nothing Then
						sColorSchemeName = msName
						bRes = True
					Else
						sColorSchemeName = "_" & CStr(Me.ID)
						bRes = False
					End If


			End Select
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetColorSchemeInfo", iTopoPurpose, iColorSchemeID, sColorSchemeName, iColorSchemeOrder, iTest)
			Return bRes
		End Function
		Public Function GetNameNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Dim sRes As String = Nothing
			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
			Dim i As Integer
			'DMCommon.Debug.MsgBoxLoop("C03_02", 3, iTopoPurpose, "miID=", miID)
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					If mdicColorSchemesAppr IsNot Nothing AndAlso mdicColorSchemesAppr.TryGetValue(miID, tColorScheme) Then
						Return tColorScheme.Name
					Else
						'MessageBox.Show(CStr(miID) & ":" & CStr(mdicColorSchemesAppr.Count), "06_122")
						Return CStr(Me.ID)
					End If

				Case DMAcadExt.enTopoPurpose.Proposed
					If mdicColorSchemesProp.TryGetValue(miID, tColorScheme) Then
						Return tColorScheme.Name
					Else
						Return CStr(Me.ID)
					End If
				Case Else
					If msName IsNot Nothing Then
						Return msName
					Else
						Return "_" & CStr(Me.ID)
					End If


			End Select
		End Function
		Public Function GetOrder(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Integer
			Dim sRes As String = Nothing
			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing

			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
               If mdicColorSchemesAppr IsNot Nothing AndAlso mdicColorSchemesAppr.TryGetValue(miID, tColorScheme) Then
                  Return tColorScheme.Order
               Else
                  'MessageBox.Show(CStr(miID) & ":" & CStr(mdicColorSchemesAppr.Count), "06_122")
                  Return 0
               End If

				Case DMAcadExt.enTopoPurpose.Proposed
               If mdicColorSchemesProp IsNot Nothing AndAlso mdicColorSchemesProp.TryGetValue(miID, tColorScheme) Then
                  Return tColorScheme.Order
               Else
                  Return 0
               End If
				Case Else
					Return 0
			End Select
		End Function
		Public Function GetName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					Return msNameAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					Return msNameProp
				Case Else
					Return msName
			End Select
		End Function

		Public Sub SetColorScheme(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bForce As Boolean)
			'	Dim iColorSchemeID As Integer
			Dim tLanduseItem As LanduseItem


			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					If mtColorSchemeAppr.ID = 0 OrElse bForce Then
						If mdicColorSetAppr IsNot Nothing AndAlso mdicColorSetAppr.ContainsKey(miID) Then
							tLanduseItem = mdicColorSetAppr.Item(miID)
						Else
							tLanduseItem = New LanduseItem(miID)
							mdicColorSetAppr.Add(miID, tLanduseItem)
						End If
						mtColorSchemeAppr = New DMAcadExt.ColorScheme(tLanduseItem.ColorSchemeID, 1.0)

						miOrderAppr = tLanduseItem.LanduseOrder
						msNameAppr = mtColorSchemeAppr.Name

					End If

				Case DMAcadExt.enTopoPurpose.Proposed
					If mtColorSchemeProp.ID = 0 Then
						If mdicColorSetProp IsNot Nothing Then
							If mdicColorSetProp.ContainsKey(miID) Then
								tLanduseItem = mdicColorSetProp.Item(miID)
							Else
								tLanduseItem = New LanduseItem(miID)
								mdicColorSetProp.Add(miID, tLanduseItem)
							End If
							mtColorSchemeProp = New DMAcadExt.ColorScheme(tLanduseItem.ColorSchemeID, 1.0)
							miOrderProp = miOrderAppr
							msNameProp = mtColorSchemeProp.Name
						End If
						If msNameProp Is Nothing OrElse msNameProp.Length = 0 Then
							Dim sMsg As String = CStr(mtColorSchemeAppr.ID) & ":"
							If mdicColorSetProp IsNot Nothing Then
								sMsg &= CStr(mdicColorSetProp.Count) & ":"
							Else
								sMsg &= CStr("Nothing") & ":"
							End If
							sMsg &= iTopoPurpose.ToString()
							MessageBox.Show(sMsg, "30_312 SetColorSc")
						End If
					End If

			End Select
			If tLanduseItem.LanduseID = 0 Then
				'	MessageBox.Show(CStr(mtColorSchemeAppr.ID) & ":" & CStr(mdicColorSetAppr.Count) & ":" & iTopoPurpose.ToString(), "30_300 SetColorSc")
			End If
		End Sub
	
		Public Function GetColorScheme(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dScale As Double, ByRef iLanduseOrder As Integer) As DMAcadExt.ColorScheme
			'	Dim iColorSchemeID As Integer
			'	DMAcadExt.AcadDocument.WriteMessage("254 GetColorScheme: " & CStr(mtColorSchemeAppr.ID) & ":" & CStr(dScale))
			Dim tLanduseItem As LanduseItem
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					If mtColorSchemeAppr.ID = 0 Then
						'	MessageBox.Show(CStr(miID) & vbCrLf & CStr(mdicColorSetAppr IsNot Nothing), "04_312c")
						If mdicColorSetAppr IsNot Nothing AndAlso mdicColorSetAppr.ContainsKey(miID) Then
							tLanduseItem = mdicColorSetAppr.Item(miID)
						Else
							tLanduseItem = New LanduseItem(miID)

						End If

						mtColorSchemeAppr = New DMAcadExt.ColorScheme(tLanduseItem.ColorSchemeID, dScale)
						miOrderAppr = tLanduseItem.LanduseOrder
						msNameAppr = mtColorSchemeAppr.Name
					Else
						mtColorSchemeAppr.Scale = dScale

					End If
					iLanduseOrder = miOrderAppr
					'		TplnProject.WriteMessageBox(miOrderAppr.ToString() & " : " & CStr(dScale), "769 ")
					Return mtColorSchemeAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					If mtColorSchemeProp.ID = 0 Then
						If mdicColorSetProp IsNot Nothing AndAlso mdicColorSetProp.ContainsKey(miID) Then
							tLanduseItem = mdicColorSetProp.Item(miID)
						Else
							tLanduseItem = New LanduseItem(miID)
						End If
						mtColorSchemeProp = New DMAcadExt.ColorScheme(tLanduseItem.ColorSchemeID, dScale)
						miOrderProp = miOrderAppr
						msNameProp = mtColorSchemeProp.Name
					Else
						mtColorSchemeProp.Scale = dScale
					End If
					iLanduseOrder = miOrderProp
					Return mtColorSchemeProp
				Case Else
					Return Nothing
			End Select
		End Function
      Public ReadOnly Property OptionAreaByIndex(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, iDataOptions As enDataOptions, bRegion As Boolean) As Double
         Get
            Dim s As String = iOverlayIndex.ToString() & ": "
            s &= CStr(mtaInPlanAreaSet(0).AcadArea) & "; "
            s &= CStr(mtaInPlanAreaSet(1).AcadArea) & "; "
            s &= CStr(mtaInPlanAreaSet(4).AcadArea) & "; "
            s &= CStr(mtaInPlanAreaSet(5).AcadArea) & "; "
				DMCommon.Debug.ExcelLog.SetNextValue(5, "!sNB!", s, iOverlayIndex, mtaInPlanAreaSet(0).AcadArea, mtaInPlanAreaSet(1).AcadArea, mtaInPlanAreaSet(2).AcadArea, mtaInPlanAreaSet(3).AcadArea, mtaInPlanAreaSet(4).AcadArea, mtaInPlanAreaSet(5).AcadArea)
				If bRegion Then
					Return mtaRegionAreaSet(iOverlayIndex).GetArea(iDataOptions)
				Else
					Return mtaInPlanAreaSet(iOverlayIndex).GetArea(iDataOptions)
				End If
				'	mtaInPlanAreaSet
			End Get
      End Property
		Public ReadOnly Property OptionAreaSet(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
			Get

				Return mtaInPlanAreaSet(iOverlayIndex)
			End Get
		End Property
		Public ReadOnly Property OptionArea(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOption As TPlanGraph.enDataOptions) As Double
			Get
				Dim dRes As Double
				'	TplnProject.WriteMessageBox(CStr(mtPropInPlanAreaSet.AcadArea), "01_549")
				'	TplnProject.WriteMessageBox(CStr(mtApprInPlanAreaSet.AcadArea), "01_548")

				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.AcadArea Then

					dRes = mtApprInPlanAreaSet.AcadArea
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.CalcMergeArea Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.ApprMerge)
					dRes = mdCalcAreaApproved
					dRes = mtApprInPlanAreaSet.CalcArea

				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.CalcMergeArea2 Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.ApprMerge)
					dRes = mdCalcAreaApproved
					dRes = mtApprInPlanAreaSet.CalcArea
					dRes = mtApprInPlanAreaSet.CalcArea2
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.RoundedArea Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.ApprMerge)
					dRes = mdCalcAreaApproved
					dRes = mtApprInPlanAreaSet.CalcArea
					dRes = mtApprInPlanAreaSet.RoundedArea
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.AcadArea Then

					dRes = mtPropInPlanAreaSet.AcadArea
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.CalcMergeArea Then

					dRes = mtPropInPlanAreaSet.CalcArea
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.CalcMergeArea2 Then

					dRes = mtPropInPlanAreaSet.CalcArea2
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.RoundedArea Then

					dRes = mtPropInPlanAreaSet.RoundedArea
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.CalcUnionArea Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.ApprUnion)
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso iOption = enDataOptions.AcadArea Then
					dRes = mdAcadAreaApproved
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.CalcMergeArea Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.PropMerge)
					dRes = mdCalcAreaProposed
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.CalcUnionArea Then
					dRes = moCalcArea.Item(DMAcadExt.enOverlayIndex.PropUnion)
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso iOption = enDataOptions.AcadArea Then
					dRes = mdAcadAreaProposed
				Else
					dRes = 0.0
				End If

				Return dRes / TplnProject.CalcRoundFactor	  'dRes / (TplnProject.UnitScaleFactor * TplnProject.CalcRoundFactor)
			End Get


		End Property




		Public Property PercentProposed() As Double
			Get
				Return mdPercentProposed
			End Get
			Set(ByVal dValue As Double)
				mdPercentProposed = dValue
			End Set
		End Property
		Public Property PercentApproved() As Double
			Get
				Return mdPercentApproved
			End Get
			Set(ByVal dValue As Double)
				mdPercentApproved = dValue
			End Set
		End Property
		Public Overridable Sub AddCalcArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal dParcelLanduseArea As Double)
			moCalcArea.Item(iOverlayMethod, iTopoPurpose) += dParcelLanduseArea
		End Sub
		
		Public Shared Sub FillColorSchemesDic(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, iMapThemeID As DMAcadExt.enMapTheme)
			'DEBUG		MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(iMapThemeID), "05_911")
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				FillColorSchemesDic(iMapThemeID, mdicColorSchemesAppr)
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				FillColorSchemesDic(iMapThemeID, mdicColorSchemesProp)
			End If
			'DEBUG	MessageBox.Show(mdicColorSchemesAppr.Count.ToString() & ":" & CStr(iTopoPurpose.ToString()), "05_945")
		End Sub
		Public Shared Sub FillColorSchemesDic(iMapThemeID As DMAcadExt.enMapTheme, ByRef dicColorSchemes As IDictionary(Of Integer, DMAcadExt.ColorScheme), Optional dScale As Double = 1.0)
			Const sSPNameLuse As String = "GetColorSchemeSet"
			Const sExproSPName As String = "GetExproColorSchemeSet"




			Const bFirstColorSchemeField As Integer = 3
			Dim tColorScheme As DMAcadExt.ColorScheme
			Dim oaParams(2) As System.Data.Common.DbParameter
			'	Dim oErrOut As System.Data.Common.DbException = Nothing
			Dim sDB_SPName As String
			Select Case iMapThemeID
				Case DMAcadExt.enMapTheme.Expropriation
					sDB_SPName = sExproSPName
				Case Else
					sDB_SPName = sSPNameLuse
			End Select
			'	MessageBox.Show(sDB_SPName & ":" & CStr(iMapThemeID), "05_912")

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetColorSchemeDataReader(iMapThemeID, sDB_SPName)
			If dicColorSchemes Is Nothing Then
				dicColorSchemes = New Dictionary(Of Integer, DMAcadExt.ColorScheme)
			End If
			If dicColorSchemes.Count <> 0 Then
				dicColorSchemes.Clear()
			End If
			If oDataReader IsNot Nothing Then
				Dim iLanduseID, iColorSchemeID As Integer
				Dim sLanduseName As String
				Dim iSchemeOrder As Integer
				'DMCommon.Debug.MsgBox("13_200g", iMapThemeID, oDataReader.HasRows)
				Do While oDataReader.Read
					iLanduseID = oDataReader.GetInt32(0)

					If oDataReader.IsDBNull(bFirstColorSchemeField) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(bFirstColorSchemeField)
					End If
					If Not oDataReader.IsDBNull(2) Then
						iSchemeOrder = oDataReader.GetInt32(2)
					End If

					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
					Else
						sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
					End If


					If iColorSchemeID <> 0 Then
						tColorScheme = New DMAcadExt.ColorScheme(oDataReader, bFirstColorSchemeField, dScale)
						tColorScheme.Order = iSchemeOrder
						'DMCommon.Debug.MsgBox("13_210c", iColorSchemeID, iLanduseID, tColorScheme.Name)
						dicColorSchemes.Add(iLanduseID, tColorScheme)
					End If




				Loop
				oDataReader.Close()
				'	System.Windows.Forms.MessageBox.Show(CStr(dicColorSchemes.Count), "07_110")

			Else
				MessageBox.Show("", "01_173g")
			End If
		End Sub
		Public Shared Sub SetNamedColorSet(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iColorSetID As Integer)

			Dim sComText As String = "SELECT LanduseID, ColorSchemeID,SchemeOrder FROM ColorSets WHERE (ID=" & CStr(iColorSetID) & ")"
			'	Dim sComText As String = "SELECT LanduseID, ColorSchemeID,SchemeOrder FROM ColorSets WHERE (ID=" & CStr(iColorSetID) & ") AND (ColorSchemeID>" & DMAcadExt.ColorScheme.MaxLanduseID & ")"

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			Dim sTest As String = ""
			'	MessageBox.Show("SetNamedColorSet #" & CStr(iColorSetID), "01_905")
			If oDataReader IsNot Nothing Then
				Dim dicColorSet As IDictionary(Of Integer, LanduseItem) = New Dictionary(Of Integer, LanduseItem)
				Dim iLanduseID As Integer
				Dim iLanduseOrder As Integer

				Dim iColorSchemeID As Integer
				While oDataReader.Read
					iLanduseID = oDataReader.GetInt32(0)
					iColorSchemeID = oDataReader.GetInt32(1)
					iLanduseOrder = oDataReader.GetInt32(2)
					'	TplnProject.WriteMessageBox(iLanduseID.ToString() & " : " & CStr(iColorSchemeID) & " : " & CStr(iLanduseOrder), "740 ")
					dicColorSet.Add(iLanduseID, New LanduseItem(iLanduseID, iLanduseOrder, iColorSchemeID))
					If iLanduseID = 4007 Then
						'	MessageBox.Show(CStr(iColorSchemeID) & ":" & CStr(dicColorSet.Count), "01_962 ColorSchemeID")
					End If
				End While
				oDataReader.Close()
				Select Case iTopoPurpose
					Case DMAcadExt.enTopoPurpose.Approved
						mdicColorSetAppr = dicColorSet

					Case DMAcadExt.enTopoPurpose.Proposed
						mdicColorSetProp = dicColorSet
				End Select
				sTest = CStr(dicColorSet.Count)
			End If
			'	MessageBox.Show(iTopoPurpose.ToString() & ":" & sTest, "26_871")

		End Sub
		Public Sub New(ByVal iLanduseID As Integer, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, Optional ByVal iParcelID As Integer = 0)
			miID = iLanduseID
			miTopoPurpose = iTopoPurpose
			' System.Windows.Forms.MessageBox.Show(CStr(miID), "Landuse-1456")

			mhsRegionsAppr = New HashSet(Of Integer)
         mhsRegionsProp = New HashSet(Of Integer)

			If iTopoPurpose = enTopoPurpose.Parcel Then
				mdicLotsAppr = New TplnLots()
				mdicLotsProp = New TplnLots()
				mlistLotsAppr = New List(Of LotIn)()
            mlistLotsProp = New List(Of LotIn)()
     

				miParcelID = 0
			Else
				mdicLots = New TplnLots()
				miParcelID = iParcelID

			End If
			'	mdicLots = New TplnLots()
		End Sub
		Public ReadOnly Property HasLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					Return (mdicLotsAppr IsNot Nothing) AndAlso (mdicLotsAppr.Count <> 0)
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return (mdicLotsProp IsNot Nothing) AndAlso (mdicLotsProp.Count <> 0)
				Else
					Return False
				End If
			End Get
		End Property
		Public Property InPlan(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Get
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					Return mbInPlanAppr
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					Return mbInPlanProp
				Else
					Return False
				End If

			End Get
			Set(bValue As Boolean)
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					mbInPlanAppr = bValue
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					mbInPlanProp = bValue

				End If

				mbInPlanAppr = bValue
			End Set
		End Property

		Public Sub TestToExcel()
         Dim oVal() As Double
         Dim iOvInd As DMAcadExt.enOverlayIndex
			DMCommon.Debug.ExcelLog.SetNextValue(0, "miID", miID)
			DMCommon.Debug.ExcelLog.SetArray(0, "RegionsAppr", True, mhsRegionsAppr.ToArray())
			For i As Integer = 0 To 5
				oVal = mtaRegionAreaSet(i).ToArray()
				iOvInd = CType(i, DMAcadExt.enOverlayIndex)

				DMCommon.Debug.ExcelLog.SetNextValue(1, "**%4", miID, i, iOvInd)
				DMCommon.Debug.ExcelLog.SetArray(0, "oVal", True, oVal)
			Next


		End Sub
		Public Sub Terminate()
			If mdicLotsAppr IsNot Nothing Then
				mdicLotsAppr.Terminate()
				mdicLotsAppr = Nothing
			End If
			If mdicLotsProp IsNot Nothing Then
				mdicLotsProp.Terminate()
				mdicLotsProp = Nothing
			End If

			If moaUnionMergePgons IsNot Nothing Then
				Erase moaUnionMergePgons
			End If
			If moaUnionUnionPgons IsNot Nothing Then
				Erase moaUnionUnionPgons
			End If
		End Sub
		Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
			Get
				Return miTopoPurpose
				If mbPlanStatusDefined Then
					Return miTopoPurpose
				Else
					System.Windows.Forms.MessageBox.Show("Plan status error", "Landuse-PlanStatus")
					Return Nothing
				End If

			End Get
			Set(ByVal iValue As DMAcadExt.enTopoPurpose)
				miTopoPurpose = iValue
			End Set
		End Property
		Private ReadOnly Property zzUnionPgons(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod) As TplnOverlayPgon()
			Get
				If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
					Return moaUnionMergePgons
				ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.Union Then
					Return moaUnionUnionPgons
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Structure LanduseItem
			Dim LanduseID As Integer
			Dim LanduseOrder As Integer
			Dim ColorSchemeID As Integer
			Public Sub New(ByVal iLanduseID As Integer, ByVal iLanduseOrder As Integer, ByVal iColorSchemeID As Integer)
				LanduseID = iLanduseID
				LanduseOrder = iLanduseOrder
				ColorSchemeID = iColorSchemeID
			End Sub
			Public Sub New(ByVal iLanduseID As Integer)
				LanduseID = iLanduseID
				LanduseOrder = iLanduseID
				ColorSchemeID = iLanduseID
			End Sub
		End Structure
		Private Structure LotIn
			Dim LotTopoID As Integer
         Dim InPlan As Boolean
         Dim Region As Integer

         Public Sub New(iLotTopoID As Integer, bInPlan As Boolean, iRegion As Integer)
            LotTopoID = iLotTopoID
            InPlan = bInPlan
            Region = iRegion
         End Sub
		End Structure
	End Class

End Namespace