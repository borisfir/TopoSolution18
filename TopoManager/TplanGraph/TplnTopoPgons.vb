Option Explicit On
Option Strict On
Namespace TPlanGraph
   Public Interface PgonDictionary
      Sub Terminate()
   End Interface
	Public Class TplnOverlayPgons
		Inherits Dictionary(Of Integer, TplnOverlayPgon)
		Implements PgonDictionary
		Private mdTotalArea As Double = 0.0
		Private miPolygonCount As Integer = 0

		Public Shadows Sub Add(oOverlayPgon As TplnOverlayPgon)
			MyBase.Add(oOverlayPgon.TopoID, oOverlayPgon)
			mdTotalArea += oOverlayPgon.AcadArea(False)
			miPolygonCount += 1
        End Sub
        Public Overridable Sub AddPgon(oOverlayPgon As TplnOverlayPgon)
            MyBase.Add(oOverlayPgon.TopoID, oOverlayPgon)
            mdTotalArea += oOverlayPgon.AcadArea(False)
            miPolygonCount += 1
        End Sub

		Public Sub Terminate() Implements PgonDictionary.Terminate
			For Each oUnionPgon As TplnOverlayPgon In MyBase.Values
				oUnionPgon.Terminate()
			Next
			MyBase.Clear()
		End Sub
		Public ReadOnly Property TotalArea As Double
			Get
				Return mdTotalArea
			End Get
		End Property
		Public ReadOnly Property PolygonCount As Integer
			Get
				Return miPolygonCount
			End Get
		End Property
		Public Function GetEntities() As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			Dim dicRes As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()

			For Each oOverlayPgon As TplnOverlayPgon In MyBase.Values
				For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In oOverlayPgon.Lines
					dicRes.Add(tAcObjID)
				Next
			Next
			Return dicRes
		End Function
	End Class

	Public Class TplnLots
		Inherits Dictionary(Of Integer, TplnLot)
		Implements PgonDictionary

		Private moDoubleNames As CheckDoubleString
		Private miPlanStatus As enTopoPurpose

		Public Sub New()
         moDoubleNames = New CheckDoubleString()
		End Sub

		Public Sub AddLot(ByVal oLot As TplnLot)
			Dim sName As String = oLot.Name
			Try
				MyBase.Add(oLot.TopoID, oLot)
			Catch oEx As Exception
				DMCommon.Debug.MsgBoxLoop("L016", 3, Me.Values.Count, oLot.TopoID, oLot.Name, oEx.Message, oEx.StackTrace)
			End Try


			If Not String.IsNullOrEmpty(sName) AndAlso Trim(sName) <> "0" Then
				'DMCommon.Debug.MsgBox("!IsNullOrEmpty", sName)
				moDoubleNames.Add(sName, True, oLot.TopoID)
			End If

		End Sub
		Public Function GetBorderLinks(iTopoPurpose As DMAcadExt.enTopoPurpose) As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			Dim sTopoName As String = TplnLot.MapThemeData(iTopoPurpose).TopoName
			Dim oLotTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim colLinks As Autodesk.Gis.Map.Topology.FullEdgeCollection = oLotTopology.GetFullEdges()
			Dim iLeftPgonID, iRightPgonID As Integer
			Dim oLeftLot, oiRightLot As TplnLot
			'	Dim iL0, iR0, iInIn, iInOut As Integer
			Dim colResult As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection()
			For Each oFullEdge As Autodesk.Gis.Map.Topology.FullEdge In colLinks
				iLeftPgonID = zzGetPolygon(oFullEdge, True)
				If iLeftPgonID = 0 Then
					colResult.Add(oFullEdge.Entity)
					'	iL0 += 1
				Else
					iRightPgonID = zzGetPolygon(oFullEdge, False)
					If iRightPgonID = 0 Then
						colResult.Add(oFullEdge.Entity)
						'	iR0 += 1
					Else
						oLeftLot = Me.Item(iLeftPgonID)
						oiRightLot = Me.Item(iRightPgonID)
						If oLeftLot.InPlan Xor oiRightLot.InPlan Then
							colResult.Add(oFullEdge.Entity)
							'	iInOut += 1
						Else
							'	iInIn += 1
						End If
					End If



				End If
			Next
			'	DMCommon.Debug.MsgBox("GetBorderLinks", iL0, iR0, iInIn, iInOut, colResult.Count)
			Return colResult
		End Function
		Private Function zzGetPolygon(oFullEdge As Autodesk.Gis.Map.Topology.FullEdge, bOnLeft As Boolean) As Integer
			Dim oPgon As Autodesk.Gis.Map.Topology.Polygon
			Try
				oPgon = oFullEdge.GetPolygon(bOnLeft)
				Return oPgon.ID
			Catch oMapEx As Autodesk.Gis.Map.MapException
				Return 0
			End Try

		End Function

		Public Sub SetDoubleNameMsg()
         Dim oLot As TplnLot
         Dim sMsg As String = "מספר מגרש '|' כפול"

         For Each oQueue As Queue(Of Integer) In moDoubleNames.DoubleValues.Values
            For Each iTopoID As Integer In oQueue.ToArray()
               oLot = Me.Item(iTopoID)
               If oLot IsNot Nothing Then
						DMAcadExt.AppMessages.AddMessage(True, oLot.CentroidX, oLot.CentroidY, "", Strings.Replace(sMsg, "|", oLot.Name), False, DMAcadExt.enMapTheme.LotApproved, 11, True)

					End If
            Next
         Next
      End Sub
		Public Function GetDoubleNamesCriteria() As String
			Return moDoubleNames.GetCriteria(TplnLot.NameFieldName)
		End Function
		Public Property PlanStatus() As TPlanGraph.enTopoPurpose
			Get
				Return miPlanStatus
			End Get
			Set(ByVal iValue As TPlanGraph.enTopoPurpose)
				miPlanStatus = iValue
			End Set
		End Property

		Public Sub Terminate() Implements PgonDictionary.Terminate
			For Each oLot As TplnLot In MyBase.Values
				oLot.Terminate()

			Next
			MyBase.Clear()
			moDoubleNames.Terminate()
		End Sub
	End Class
	Public Class TplnBlocks
		Inherits Dictionary(Of Integer, TplnBlock)
      Implements PgonDictionary
      Private mhsBlocksFromParcels As HashSet(Of String) = New HashSet(Of String)()
		Private mdicTopo As Dictionary(Of Integer, TplnBlock)
		Public Sub New()
			MyBase.New()
			mdicTopo = New Dictionary(Of Integer, TplnBlock)

		End Sub
      Public Sub AddBlock(ByVal oBlock As TplnBlock, bPgonExists As Boolean)
         Dim oBlockExisted As TplnBlock
         If Not MyBase.ContainsKey(oBlock.Key) Then
            MyBase.Add(oBlock.Key, oBlock)
         ElseIf bPgonExists Then
            oBlockExisted = Me.Item(oBlock.Key)
            oBlockExisted.TopoID = oBlock.TopoID
            oBlockExisted.PgonExists = oBlock.PgonExists
         End If
         If oBlock.TopoID <> 0 AndAlso Not mdicTopo.ContainsKey(oBlock.TopoID) Then
            mdicTopo.Add(oBlock.TopoID, oBlock)
         End If
      End Sub
		Public Sub AddParcel(ByVal oParcel As TplnParcel)
         Dim oBlock As TplnBlock = Nothing
         '  If oParcel.BlockNo > -1 Then

         '  End If
         If MyBase.ContainsKey(oParcel.BlockKey) Then
            
            Try
               oBlock = MyBase.Item(oParcel.BlockKey)
               oBlock.AddParcel(oParcel)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnBlocks - AddParcel")
            End Try
         Else
            oBlock = New TplnBlock(oParcel)
            '  If oParcel.BlockKey > -1000 Then
            '   DMAcadExt.AcadDocument.WriteMessageLog("M#2: " & CStr(oBlock.Key) & "; " & CStr(oParcel.BlockKey))
            'End If

            MyBase.Add(oBlock.Key, oBlock)
            mhsBlocksFromParcels.Add(oBlock.BlockName)
            'System.Windows.Forms.MessageBox.Show(oBlock.BlockName, "05_866")
         End If
      End Sub
      Public ReadOnly Property BlocksFromParcels As HashSet(Of String)
         Get
            Return mhsBlocksFromParcels
         End Get
      End Property

      Public ReadOnly Property TopoItem(ByVal iTopoID As Integer) As TplnBlock
         Get
            Dim oBlock As TplnBlock = Nothing
            mdicTopo.TryGetValue(iTopoID, oBlock)
            Return oBlock
         End Get
      End Property

      Public Overloads ReadOnly Property Item(ByVal iBlockNo As Integer, ByVal iBlockAddNo As Integer) As TplnBlock
         Get
            Dim oBlock As TplnBlock = Nothing
            Dim iKey As Integer = 1000 * iBlockNo + iBlockAddNo
            MyBase.TryGetValue(iKey, oBlock)
            Return oBlock
         End Get
      End Property
      Public Overloads ReadOnly Property Block(ByVal iBlockNo As Integer, ByVal iBlockAddNo As Integer) As TplnBlock
         Get
            Dim oBlock As TplnBlock = Nothing
            Dim iKey As Integer = 1000 * iBlockNo + iBlockAddNo

            Dim bRes As Boolean = MyBase.TryGetValue(iKey, oBlock)
            '	System.Windows.Forms.MessageBox.Show(CStr(iKey) & vbCrLf & bRes.ToString() & vbCrLf & (oBlock IsNot Nothing).ToString(), "21_907")
            Return oBlock
         End Get
      End Property
      Public Sub Terminate() Implements PgonDictionary.Terminate
         For Each oBlock As TplnBlock In MyBase.Values
            oBlock.Terminate()
         Next
         MyBase.Clear()
         If mdicTopo IsNot Nothing Then
            mdicTopo.Clear()
            mdicTopo = Nothing
         End If
      End Sub
   End Class
   Public Class TplnPlans
      Inherits Dictionary(Of Integer, TplnPlan)
      Implements PgonDictionary
      Private moDoubleNames As CheckDoubleString
      Private mbUseHandles As Boolean
      Private mdicHandles As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.Handle, Integer)
      Public Sub New(bUseHandles As Boolean, bCheckDoubleName As Boolean)
         mbUseHandles = bUseHandles
         If bCheckDoubleName Then
            moDoubleNames = New CheckDoubleString()
         End If

         If mbUseHandles Then
            mdicHandles = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.Handle, Integer)()
         End If
      End Sub
      Public Sub AddPlan(ByVal oPlan As TplnPlan)
         If Not MyBase.ContainsKey(oPlan.TopoID) Then
            MyBase.Add(oPlan.TopoID, oPlan)
         Else
            'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
         End If

         '  moDoubleNames.Add(CStr(oParcel.BlockNo), False, oParcel.Name, True, oParcel.TopoID)
         If mbUseHandles Then
            mdicHandles.Add(oPlan.PgonHandle, oPlan.TopoID)
         End If
      End Sub
       
      Public Function GetDoubleNamesCriteria() As String
			Return moDoubleNames.GetCriteria(TplnParcel.BlockFieldName, TplnParcel.NameFieldName)
		End Function
      Public Sub Terminate() Implements PgonDictionary.Terminate
         For Each oPlan As TplnPlan In MyBase.Values
            oPlan.Terminate()
         Next
         MyBase.Clear()
         moDoubleNames.Terminate()
      End Sub
   End Class
	Public Class TplnParcels
		Inherits Dictionary(Of Integer, TplnParcel)
		Implements PgonDictionary
		Private moDoubleNames As CheckDoubleString
		Private mbUseHandles As Boolean
		Private mdicHandles As Dictionary(Of Autodesk.AutoCAD.DatabaseServices.Handle, Integer)
      Public Sub New(bUseHandles As Boolean, bCheckDoubleName As Boolean)
         mbUseHandles = bUseHandles
         If bCheckDoubleName Then
            moDoubleNames = New CheckDoubleString()
         End If

         If mbUseHandles Then
            mdicHandles = New Dictionary(Of Autodesk.AutoCAD.DatabaseServices.Handle, Integer)()
         End If
      End Sub
		Public Sub AddParcel(ByVal oParcel As TplnParcel)
			If Not MyBase.ContainsKey(oParcel.TopoID) Then
				MyBase.Add(oParcel.TopoID, oParcel)
			Else
				'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
			End If
         If moDoubleNames IsNot Nothing Then
            moDoubleNames.Add(CStr(oParcel.BlockNo), False, oParcel.Name, True, oParcel.TopoID)
            ' DMAcadExt.AcadDocument.WriteMessage("%$:" & CStr(oParcel.BlockNo) & ":" & CStr(oParcel.Name))
         End If
       
			If mbUseHandles Then
				mdicHandles.Add(oParcel.PgonHandle, oParcel.TopoID)
			End If
      End Sub
      Public Overloads Sub Clear()
         MyBase.Clear()
         If moDoubleNames IsNot Nothing Then
            moDoubleNames.Clear()
         End If
      End Sub
		Public Function GetItemByHandleAAA(tHandle As Autodesk.AutoCAD.DatabaseServices.Handle) As TplnParcel
			If mbUseHandles Then
				Dim oParcel As TplnParcel = Nothing
				Dim iID As Integer
				If mdicHandles.TryGetValue(tHandle, iID) Then
					If MyBase.TryGetValue(iID, oParcel) Then
						Return oParcel
					Else
						Return Nothing
					End If
				Else
					Return Nothing
				End If
			Else
				Return Nothing
			End If
		End Function
		Public Function GetDoubleNamesCriteria() As String
			Return moDoubleNames.GetCriteria(TplnParcel.BlockFieldName, TplnParcel.NameFieldName)
		End Function
      Public Function GetDoubleNamesCount() As Integer
         Return moDoubleNames.Count
      End Function
      Public Sub SetDoubleNameMsg()
         Dim oParcel As TplnParcel
         Dim sMsg As String = "מספר מגרש '|' כפול"
         Dim bMain As Boolean
         '  DMCommon.Debug.MsgBox("09_782a", moDoubleNames.DoubleValues.Values.Count)
         For Each oQueue As Queue(Of Integer) In moDoubleNames.DoubleValues.Values
            bMain = True
            For Each iTopoID As Integer In oQueue.ToArray()
               oParcel = Me.Item(iTopoID)

               If oParcel IsNot Nothing Then


                  DMAcadExt.AppMessages.AddMessage(True, oParcel.CentroidX, oParcel.CentroidY, "", DMCommon.dmMessages.Message(309, oParcel.Name, oParcel.BlockFull), False, DMAcadExt.enMapTheme.Parcels, 11, bMain)
                  bMain = False
               End If
            Next
         Next


      End Sub

		Public Sub Terminate() Implements PgonDictionary.Terminate
			For Each oParcel As TplnParcel In MyBase.Values
				oParcel.Terminate()
			Next
         MyBase.Clear()
         If moDoubleNames IsNot Nothing Then
            moDoubleNames.Terminate()
         End If

		End Sub
	End Class
	Public Class TplnExpros
		Inherits Dictionary(Of Integer, TplnExpro)
		Implements PgonDictionary

		Public Sub New()

		End Sub
		Public Sub AddExpro(ByVal oExpro As TplnExpro)
			If Not MyBase.ContainsKey(oExpro.TopoID) Then
				MyBase.Add(oExpro.TopoID, oExpro)
			Else
				'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
			End If


		End Sub
		
	
		Public Sub Terminate() Implements PgonDictionary.Terminate
			For Each oExpro As TplnExpro In MyBase.Values
				oExpro.Terminate()
			Next
			MyBase.Clear()

		End Sub
   End Class

	Public Class TplnZones
      Inherits Dictionary(Of Integer, TplnZone)
      Implements PgonDictionary
      Public Sub New()

      End Sub
      Public Sub AddZone(ByVal oZone As TplnZone)
         If Not MyBase.ContainsKey(oZone.TopoID) Then
            MyBase.Add(oZone.TopoID, oZone)
         Else
            'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
         End If


      End Sub


      Public Sub Terminate() Implements PgonDictionary.Terminate
         For Each oZone As TplnZone In MyBase.Values
            oZone.Terminate()
         Next
         MyBase.Clear()

      End Sub
   End Class

   Public Class TplnRegions
		Inherits SortedDictionary(Of Integer, TplnRegion)
		Private mdicRegionsByTopoID As Dictionary(Of Integer, TplnRegion)
		Public Sub New()
			mdicRegionsByTopoID = New Dictionary(Of Integer, TplnRegion)()
		End Sub
		Public Sub AddRegion(oRegion As TplnRegion)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddRegion", oRegion.RegionNo, oRegion.RegionName)
			If Not MyBase.ContainsKey(oRegion.RegionNo) Then
				MyBase.Add(oRegion.RegionNo, oRegion)
			End If
			If oRegion.TopoID <> 0 Then
				mdicRegionsByTopoID.Add(oRegion.TopoID, oRegion)
			End If

		End Sub
		Public Sub AddRegionNo(iRegionNo As Integer)
			If Not MyBase.ContainsKey(iRegionNo) Then
				Dim oRegion As TplnRegion = New TplnRegion(iRegionNo)
				MyBase.Add(iRegionNo, oRegion)
			End If


		End Sub

		Public Function ContainsTopoID(iTopoID As Integer) As Boolean
			Return mdicRegionsByTopoID.ContainsKey(iTopoID)
		End Function
		Public Function TryGetRegion(iTopoID As Integer, ByRef oRegion As TplnRegion) As Boolean
			Return mdicRegionsByTopoID.TryGetValue(iTopoID, oRegion)
		End Function

		Public Overloads Sub Clear()
			MyBase.Clear()
			mdicRegionsByTopoID.Clear()
		End Sub

	End Class
   Public Class TplnMerhavDic
      Inherits Dictionary(Of Integer, TplnMerhav)
      Implements PgonDictionary
      Public Sub New()

      End Sub
      Public Sub AddMerhav(ByVal oMerhav As TplnMerhav)
         If Not MyBase.ContainsKey(oMerhav.TopoID) Then
            MyBase.Add(oMerhav.TopoID, oMerhav)
         Else
            'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
         End If


      End Sub


      Public Sub Terminate() Implements PgonDictionary.Terminate
         For Each oMerhav As TplnMerhav In MyBase.Values
            oMerhav.Terminate()
         Next
         MyBase.Clear()

      End Sub
   End Class
   Public Class TplnLanduses
      Inherits SortedDictionary(Of Integer, TplnLanduse)
      Implements PgonDictionary

      Private miTopoPurpose As DMAcadExt.enTopoPurpose

      Public Sub New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
         MyBase.New()
         miTopoPurpose = iTopoPurpose

      End Sub
      Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
         Get
            Return miTopoPurpose
         End Get
         Set(ByVal iValue As DMAcadExt.enTopoPurpose)
            miTopoPurpose = iValue
         End Set
      End Property
      Public Sub AddLanduseArea(ByVal iTopoPurpose As enTopoPurpose, ByVal iLanduseID As Integer, ByVal dCalcArea As Double)



      End Sub
      Public Function AddLotNew(ByVal oLot As TplnLot, bContainsColorScheme As Boolean) As TplnLanduse '21/06/10
         Dim iLanduseID As Integer = oLot.LanduseID
         Dim oLanduse As TplnLanduse

			If iLanduseID <> 0 Then
            If MyBase.ContainsKey(iLanduseID) Then
               oLanduse = MyBase.Item(iLanduseID)
            Else

               oLanduse = New TplnLanduse(iLanduseID, miTopoPurpose)
               MyBase.Add(iLanduseID, oLanduse)
            End If

				If bContainsColorScheme AndAlso oLanduse.ID <> 0 Then
               '	oLanduse.SetColorScheme(oLot.TopoPurpose, False)
            End If
            '	oLanduse.AddAcadArea(oLot.TopoPurpose, oLot.AcadArea(False))
            '	oLanduse.AddCalcArea(DMAcadExt.enOverlayMethod.Merge, oLot.TopoPurpose, oLot.CalcArea(False, DMAcadExt.enOverlayMethod.Merge))
            '	oLanduse.AddCalcArea(DMAcadExt.enOverlayMethod.Union, oLot.TopoPurpose, oLot.CalcArea(True, DMAcadExt.enOverlayMethod.Union))

            oLanduse.AddLot(oLot)

            Return oLanduse
         Else

            Return Nothing
         End If


      End Function
      Public Function AddLotOldVer(ByVal oLot As TplnLot, bContainsColorScheme As Boolean) As TplnLanduse '21/06/10
         Dim iLanduseID As Integer = oLot.LanduseID
         Dim oLanduse As TplnLanduse
         If iLanduseID <> 0 Then
            If MyBase.ContainsKey(iLanduseID) Then
               oLanduse = MyBase.Item(iLanduseID)
            Else
               oLanduse = New TplnLanduse(iLanduseID, miTopoPurpose)
               MyBase.Add(iLanduseID, oLanduse)
            End If
            If bContainsColorScheme AndAlso oLanduse.ID <> 0 Then
               oLanduse.SetColorScheme(oLot.TopoPurpose, False)
            End If
            '	oLanduse.AddAcadArea(oLot.TopoPurpose, oLot.AcadArea(False))
            '	oLanduse.AddCalcArea(DMAcadExt.enOverlayMethod.Merge, oLot.TopoPurpose, oLot.CalcArea(False, DMAcadExt.enOverlayMethod.Merge))
            '	oLanduse.AddCalcArea(DMAcadExt.enOverlayMethod.Union, oLot.TopoPurpose, oLot.CalcArea(True, DMAcadExt.enOverlayMethod.Union))
            oLanduse.AddLot(oLot)
            Return oLanduse
         Else
            Return Nothing
         End If


      End Function

      Public Sub AddLot(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iLanduseID As Integer, ByVal iLotID As Integer, bLotInPlan As Boolean, ByVal iRegion As Integer, ByVal iParcelID As Integer)

         Dim oLanduse As TplnLanduse
         If MyBase.ContainsKey(iLanduseID) Then
            oLanduse = MyBase.Item(iLanduseID)
         Else
            oLanduse = New TplnLanduse(iLanduseID, miTopoPurpose, iParcelID)
            MyBase.Add(iLanduseID, oLanduse)
         End If
         Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
         '	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(iLanduseID) & ":" & CStr(iLotID), "05_301")
         If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
            ''''''''''		System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotID), "05_705")
         End If



         oLanduse.AddLot(iOverlayIndex, iLotID, bLotInPlan, iRegion)
      End Sub
		Public Function GetLanduseName(iLanduseID As Integer) As String
			Dim oLanduse As TplnLanduse = Nothing
			If MyBase.TryGetValue(iLanduseID, oLanduse) Then
				Return oLanduse.Name
			Else
				Return Nothing
			End If

		End Function
		Public Sub Terminate() Implements PgonDictionary.Terminate
         For Each oLanduse As TplnLanduse In MyBase.Values
            oLanduse.Terminate()
            oLanduse = Nothing
         Next
      End Sub
   End Class
   Public Class TplnUnionGroups
      Inherits Dictionary(Of UnionKey, TplnUnionGroup)
      Implements PgonDictionary
      Public Sub AddUnionPgon(ByVal iOverlayMethod As enOverlayMethod, ByRef oUnionPgon As TplnOverlayPgon)
         Dim oUnionGroup As TplnUnionGroup
         Dim sTest As String = "a"
         Try

            Dim iParcelTopoID As Integer = oUnionPgon.ParcelTopoID
            sTest = "b"
            Dim iLotTopoID As Integer = oUnionPgon.LotTopoID
            sTest = "c"
            Dim tUnionKey As New UnionKey(iParcelTopoID, iLotTopoID)
            sTest = "d"
            If MyBase.ContainsKey(tUnionKey) Then
               sTest &= "e"
               oUnionGroup = MyBase.Item(tUnionKey)
               sTest &= "f"
            Else
               sTest &= "g"
               oUnionGroup = New TplnUnionGroup(iParcelTopoID, iLotTopoID)
               sTest &= "h"
               MyBase.Add(tUnionKey, oUnionGroup)
               sTest &= "i"
            End If
            sTest &= "j"
            If oUnionGroup Is Nothing Then
               sTest &= "k"
            Else
               sTest &= "l"
            End If
            oUnionGroup.AddUnionPgon(iOverlayMethod, oUnionPgon)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnUnionGroups - AddUnionPgon")
         End Try
      End Sub
      Public Sub Terminate() Implements PgonDictionary.Terminate
      End Sub
   End Class
   Public Class TplnOverlayGroups
      Inherits Dictionary(Of ULong, TplnOverlayGroup)
      Private miInPlanCount As Integer = 0

		Public Sub New()

		End Sub

		Public Function AddOverlayPgon(ByRef oOverlayPgon As TplnOverlayPgon, sTestMsg As String) As Boolean
			Try
				Dim iParcelTopoID As Integer = oOverlayPgon.ParcelTopoID
				Dim iLotTopoID As Integer = oOverlayPgon.LotTopoID
				Dim bLotOut As Boolean = oOverlayPgon.LotOut
				Dim iLotGroupID As Integer = oOverlayPgon.LotGroupID
				Dim iLanduseID As Integer = oOverlayPgon.LanduseID
				Dim iBlockNo As Integer = oOverlayPgon.BlockNo
				Dim iBlockAddNo As Integer = oOverlayPgon.BlockAddNo
				'  DMAcadExt.AcadDocument.WriteDebugMessage("21_23:" & iLotTopoID.ToString() & ", " & iLotGroupID.ToString() & ",Out? " & bLotOut.ToString())
				Dim sTest As String
				Dim bRes As Boolean

				Dim iTestParcelID As Integer
				Dim iTestLotID As Integer
				Dim lOverlayKey As ULong = TplnOverlayGroup.GetOverlayKey(iParcelTopoID, iLotTopoID, "TplnOverlayGroups-AddOverlayPgon" & vbCrLf & sTestMsg)
				Dim oOverlayGroup As TplnOverlayGroup = Nothing
				If MyBase.TryGetValue(lOverlayKey, oOverlayGroup) Then
					oOverlayGroup.AddAcadArea(oOverlayPgon.AcadArea(False))
					TplnOverlayGroup.ParseOverlayKey(lOverlayKey, iTestParcelID, iTestLotID)
					sTest = iTestParcelID.ToString() & "," & iTestLotID.ToString() & " A=" & oOverlayPgon.AcadArea(False).ToString
					bRes = False
				Else
					oOverlayGroup = New TplnOverlayGroup(iParcelTopoID, iLotTopoID, bLotOut, iLotGroupID, iLanduseID, iBlockNo, iBlockAddNo)
					oOverlayGroup.AcadArea = oOverlayPgon.AcadArea(False)
					MyBase.Add(lOverlayKey, oOverlayGroup)
					If oOverlayGroup.IsInPlan Then
						miInPlanCount += 1
					End If
					sTest = "N " & iParcelTopoID.ToString() & "," & iLotTopoID.ToString()
					bRes = True
				End If
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddOver", iParcelTopoID, iLotTopoID, sTest, Me.Count)
				Return bRes
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnUnionGroups - AddUnionPgon_2")
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!ERRAddOver", oEx.Message, oEx.StackTrace, Me.Count)
				Return False
			End Try
		End Function
		Public Function TryGetItem(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer, ByRef oResOverlayGroup As TplnOverlayGroup) As Boolean
         Dim lOverlayKey As ULong = TplnOverlayGroup.GetOverlayKey(iParcelTopoID, iLotTopoID, "TplnOverlayGroups-TryGetItem")
         Return Me.TryGetValue(lOverlayKey, oResOverlayGroup)
      End Function
      Public Function GetItem(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer) As TplnOverlayGroup
         Dim lOverlayKey As ULong = TplnOverlayGroup.GetOverlayKey(iParcelTopoID, iLotTopoID, "TplnOverlayGroups-GetItem")
         Dim oResOverlayGroup As TplnOverlayGroup = Nothing

         If Not Me.TryGetValue(lOverlayKey, oResOverlayGroup) Then
            DMAcadExt.AcadDocument.WriteMessageLog(CStr(iParcelTopoID) & ":" & CStr(iLotTopoID), "02_150")
         End If
         Return oResOverlayGroup
      End Function
      Public Function GetItem(ByVal iTopoID1 As Integer, ByVal iTopoID2 As Integer, bParcelLot As Boolean) As TplnOverlayGroup
         If bParcelLot Then
            Return GetItem(iTopoID1, iTopoID2)
         Else
            Return GetItem(iTopoID2, iTopoID1)
         End If

      End Function
      Public Function GetTest(Optional ByVal iCount As Integer = 0) As String
         Dim sRes As String = ""
         Dim i As Integer = 0

         If Me.Count < iCount OrElse iCount = 0 Then
            iCount = Me.Count
         End If
         For Each oGroup As TplnOverlayGroup In MyBase.Values
            If i = iCount Then
               Exit For
            End If
            If sRes.Length <> 0 Then
               sRes &= vbCrLf
            End If
            sRes &= oGroup.GetTest()
            i += 1
         Next
         Return sRes
      End Function
		Public Sub Calculate2New(colLotForcedArea As System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea), Optional colRegionForcedArea As System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea) = Nothing)
			Dim oaCells(Me.Values.Count - 1) As TopoManager.BalanceArea.Cell
			Dim oaCellsA(Me.Values.Count - 1) As TopoManager.BalanceArea.Cell

			'  Dim taConstGroups As IEnumerable(Of TopoManager.BalanceArea.ConstArea)
			'Dim taConstPgons As IEnumerable(Of TopoManager.BalanceArea.ConstArea)

			Dim daInput(Me.Values.Count - 1) As Double
			Dim iaGroupNo(Me.Values.Count - 1) As Integer
			Dim oaOverlayGroups(Me.Values.Count - 1) As TplnOverlayGroup
			Dim dPlanCalcArea As Double
			Dim dPlanAcadArea As Double
			Dim dPlanCheckCalc2Area As Double
			Dim iOverlayGroupIndex As Integer = 0
			' Dim iIndex As Integer

			'DMCommon.Debug.MsgBox("01_554c", Me.Values.Count)
			For Each oOverlayGroup As TplnOverlayGroup In Me.Values
				If oOverlayGroup.IsInPlan Then

					oaCells(iOverlayGroupIndex) = New TopoManager.BalanceArea.Cell(oOverlayGroup.AcadArea, oOverlayGroup.LotID, oOverlayGroup.GroupID)
					oaCellsA(iOverlayGroupIndex) = New TopoManager.BalanceArea.Cell(oOverlayGroup.AcadArea, oOverlayGroup.LotID, oOverlayGroup.GroupID)
					If oOverlayGroup.AcadArea = 0 OrElse oOverlayGroup.CalcArea = 0 Then
						DMCommon.Debug.MsgBox("01_554k", Me.Values.Count, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.BlockNo, oOverlayGroup.ParcelID, oOverlayGroup.ParcelID)
					End If
					dPlanAcadArea += oOverlayGroup.AcadArea
					dPlanCalcArea += oOverlayGroup.CalcArea
					Try
						oaOverlayGroups(iOverlayGroupIndex) = oOverlayGroup
					Catch oEx As Exception
						'MessageBo  .Show(CStr(Me.Count) & ":" & CStr(miInPlanCount) & ":" & CStr(iOverlayGroupIndex) & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace, "02_122new")
						DMCommon.Debug.MsgBoxLoop("L006", Me.Values.Count, miInPlanCount, iOverlayGroupIndex, oEx.Message, oEx.StackTrace)

					End Try
					iOverlayGroupIndex += 1
				Else
					'DMCommon.Debug.MsgBoxLoop("L007", iOverlayGroupIndex, Me.Values.Count)

				End If
			Next
			ReDim Preserve oaCells(iOverlayGroupIndex - 1)
			ReDim Preserve oaCellsA(iOverlayGroupIndex - 1)

			'  oaCells.CopyTo(oaCellsA, 0)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Calc2New", Me.Values.Count, dPlanAcadArea, dPlanCalcArea)

			Dim iRowA As Integer = 0
			If oaCells IsNot Nothing AndAlso oaCellsA IsNot Nothing Then
				Dim oBalanceCalcArea As BalanceArea = New BalanceArea(oaCells, colLotForcedArea, colRegionForcedArea, BalanceArea.enBalanceLevel.AllByCells, dPlanCalcArea, TplnProject.CalcRoundFactor, True, "TplnTopoPgons.Calculte2_7")  '   temp
				Dim oBalanceAcadArea As BalanceArea = New BalanceArea(oaCellsA, BalanceArea.enBalanceLevel.AllByCells, dPlanAcadArea, TplnProject.CalcRoundFactor, True, "TplnTopoPgons.Calculte2_2")    '   temp 


				For iIndex As Integer = 0 To Me.Values.Count - 1
					If oaOverlayGroups(iIndex) IsNot Nothing Then
						oaOverlayGroups(iIndex).CalcArea2 = oBalanceCalcArea.OutputItemFloat(iIndex)
						oaOverlayGroups(iIndex).RoundedArea = oBalanceAcadArea.OutputItemFloat(iIndex)
						dPlanCheckCalc2Area += oBalanceCalcArea.OutputItemFloat(iIndex)
					End If
				Next

			End If



		End Sub

		Public Sub Calculate2(iRegion As Integer)
         Dim daInput(miInPlanCount - 1) As Double
         '    Dim iaGroupNo(miInPlanCount - 1) As Integer
         Dim oaOverlayGroups(miInPlanCount - 1) As TplnOverlayGroup
         Dim dPlanCalcArea As Double
         Dim dPlanAcadArea As Double
         Dim iOverlayGroupIndex As Integer = 0
         '  Dim iGroupCount As Integer
         Dim iRow As Integer = 0
         For Each oOverlayGroup As TplnOverlayGroup In Me.Values
            If oOverlayGroup.GroupID = iRegion Then

               daInput(iOverlayGroupIndex) = oOverlayGroup.AcadArea
               '  iaGroupNo(iOverlayGroupIndex) = oOverlayGroup.GroupID
               dPlanAcadArea += oOverlayGroup.AcadArea
               dPlanCalcArea += oOverlayGroup.CalcArea
               '''''''''''''''''''''''''     DMCommon.ExcelLog.SetNextValue(iRow, 2, iOverlayGroupIndex, dPlanAcadArea, dPlanCalcArea)
               '	MessageBox.Show(CStr(dPlanCalcArea) & ":" & CStr(oOverlayGroup.CalcArea) & vbCrLf & CStr(oOverlayGroup.ParcelID) & "-" & CStr(oOverlayGroup.LotID), "02_118")
               Try
                  oaOverlayGroups(iOverlayGroupIndex) = oOverlayGroup
               Catch oEx As Exception
                  MessageBox.Show(CStr(Me.Count) & ":" & CStr(miInPlanCount) & ":" & CStr(iOverlayGroupIndex) & vbCrLf & oEx.Message, "02_122")
               End Try

               iOverlayGroupIndex += 1
            End If

         Next

         ReDim Preserve daInput(iOverlayGroupIndex - 1)
         ReDim Preserve oaOverlayGroups(iOverlayGroupIndex - 1)

         '   MessageBox.Show(CStr(dPlanAcadArea) & ":" & CStr(dPlanCalcArea) & ":" & CStr(TplnProject.CalcRoundFactor), "02_120")

         Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, dPlanCalcArea, True, "TplnTopoPgons.Calculte2_1")  '   temp 
         '  Dim oBalanceAcadArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, dPlanAcadArea, True, "TplnTopoPgons.Calculte2_2")    '   temp 
         Dim s As String = "אבגד"

         Dim oLot As TplnLot
         Dim oParcel As TplnParcel
         Dim sLotName, sGush, sParcelName As String
         ''''''''''''''''''''''   DMCommon.ExcelLog.Reset()
         For iIndex As Integer = 0 To iOverlayGroupIndex - 1
            If oaOverlayGroups(iIndex) IsNot Nothing Then
               oaOverlayGroups(iIndex).CalcGroupArea = oBalanceCalcArea.OutputItemFloat(iIndex)
               oaOverlayGroups(iIndex).CalcGroupArea2 = oBalanceCalcArea.OutputItemFloat(iIndex)


               oLot = TplnProject.GetLot(DMAcadExt.enTopoPurpose.Approved, oaOverlayGroups(iIndex).LotID)
               If oLot IsNot Nothing Then
                  sLotName = oLot.Name
               Else
                  sLotName = String.Empty
               End If

               oParcel = TplnProject.GetParcel(oaOverlayGroups(iIndex).ParcelID)
               If oParcel IsNot Nothing Then
                  sGush = oParcel.BlockFull
                  sParcelName = oParcel.Name
               Else
                  sGush = ""
                  sParcelName = String.Empty
               End If
               'oaOverlayGroups(iIndex).AcadArea
               'oaOverlayGroups(iIndex).CalcArea
               'oaOverlayGroups(iIndex).CalcArea2
               'oaOverlayGroups(iIndex).CalcGroupArea

               'oaOverlayGroups(iIndex).CalcGroupArea2
               If iRegion <> 0 Then
                  ''''''''''''''''' DMCommon.ExcelLog.SetNextValue(i, 0, oaOverlayGroups(iIndex).LotID, oaOverlayGroups(iIndex).GroupID, sLotName, oaOverlayGroups(iIndex).ParcelID, sGush, sParcelName, oaOverlayGroups(iIndex).AcadArea, oaOverlayGroups(iIndex).CalcArea, oaOverlayGroups(iIndex).CalcArea2, oaOverlayGroups(iIndex).CalcGroupArea, oaOverlayGroups(iIndex).CalcGroupArea2)
               End If
               '  oaOverlayGroups(iIndex).RoundedArea = oBalanceAcadArea.OutputItemFloat(iIndex)
               If iIndex < 0 Then
                  DMAcadExt.AcadDocument.WriteMessageLog(CStr(oaOverlayGroups(iIndex).AcadArea) & ":" & CStr(oaOverlayGroups(iIndex).RoundedArea), "02_150")
                  DMAcadExt.AcadDocument.WriteMessageLog(CStr(oaOverlayGroups(iIndex).CalcArea) & ":" & CStr(oaOverlayGroups(iIndex).CalcArea2), "02_160")
               End If
            End If
         Next
      End Sub
   End Class

	Public Class TplnOwnershipNotes
		Inherits Dictionary(Of Integer, TplnOwnershipNote)
		Implements PgonDictionary
		Public Sub New()

		End Sub
		Public Sub AddOwnershipNote(ByVal oOwnershipNote As TplnOwnershipNote)
			If Not MyBase.ContainsKey(oOwnershipNote.TopoID) Then
				MyBase.Add(oOwnershipNote.TopoID, oOwnershipNote)
			Else
				'DMAcadExt.AcadDocument.WriteMessage(CStr(oParcel.TopoID) & ":" & CStr(MyBase.Count) & ":" & CStr(oParcel.Block) & ":" & CStr(oParcel.Name) & ":" & CStr(oParcel.LegalArea), "02_150")
			End If


		End Sub


		Public Sub Terminate() Implements PgonDictionary.Terminate
			For Each oOwnershipNote As TplnOwnershipNote In MyBase.Values
				oOwnershipNote.Terminate()
			Next
			MyBase.Clear()

		End Sub
	End Class

End Namespace

