Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports DMAcadExt
Namespace TPlanGraph
	Public Structure TplnAreaSet
		Dim AcadArea As Double
		Dim RoundedArea As Double
		Dim CalcArea As Double
      Dim CalcArea2 As Double
      Dim CalcGroupArea As Double
      Dim CalcGroupArea2 As Double
		Dim Part As Double
		Dim PlanState As NumerationPair.enComplexType

		Public Shared Operator +(a As TplnAreaSet, b As TplnAreaSet) As TplnAreaSet
			Dim tResp As TplnAreaSet
			tResp.AcadArea = a.AcadArea + b.AcadArea
			tResp.RoundedArea = a.RoundedArea + b.RoundedArea
			tResp.CalcArea = a.CalcArea + b.CalcArea
         tResp.CalcArea2 = a.CalcArea2 + b.CalcArea2
         tResp.CalcGroupArea = a.CalcGroupArea + b.CalcGroupArea
         tResp.CalcGroupArea2 = a.CalcGroupArea2 + b.CalcGroupArea2

         Return tResp
		End Operator
		Public Sub Reset()
			AcadArea = 0.0
			RoundedArea = 0.0
			CalcArea = 0.0
         CalcArea2 = 0.0
         CalcGroupArea = 0.0
         CalcGroupArea2 = 0.0

      End Sub
      Public Sub ResetGroup()
        
         CalcGroupArea = 0.0
         CalcGroupArea2 = 0.0

      End Sub
		Public Sub New(dAcadArea As Double)
			AcadArea = dAcadArea
		End Sub
		Public Sub Add(b As TplnAreaSet)
			AcadArea += b.AcadArea
			RoundedArea += b.RoundedArea
			CalcArea += b.CalcArea
         CalcArea2 += b.CalcArea2
         CalcGroupArea += b.CalcGroupArea
         CalcGroupArea2 += b.CalcGroupArea2

      End Sub
      Public Sub AddGroup(b As TplnAreaSet)
         CalcGroupArea += b.CalcArea
         CalcGroupArea2 += b.CalcGroupArea2

      End Sub
		Public Function GetArea(iDataOptions As enDataOptions) As Double
			Select Case iDataOptions
				Case enDataOptions.AcadArea
					Return AcadArea
				Case enDataOptions.RoundedArea
					Return RoundedArea
				Case enDataOptions.CalcMergeArea
					Return CalcArea
				Case enDataOptions.CalcMergeArea2
               Return CalcArea2
            Case enDataOptions.CalcGroupArea
               Return CalcGroupArea
            Case enDataOptions.CalcGroupArea2
               Return CalcGroupArea2
				Case Else
					Return 0.0
			End Select
      End Function
      Public Function ToArray() As Double()
         Return New Double() {AcadArea, RoundedArea, CalcArea, CalcArea2, CalcGroupArea, CalcGroupArea2}
      End Function
		Public Overrides Function ToString() As String
			Dim sRes As String = String.Empty
			If AcadArea <> 0.0 Then
				If sRes.Length <> 0 Then
					sRes &= vbCrLf
				End If
				sRes &= "Acad=" & AcadArea.ToString()
			End If
			If RoundedArea <> 0.0 Then
				If sRes.Length <> 0 Then
					sRes &= vbCrLf
				End If
				sRes &= "Rounded=" & RoundedArea.ToString()
			End If
			If CalcArea <> 0.0 Then
				If sRes.Length <> 0 Then
					sRes &= vbCrLf
				End If
				sRes &= "Calc=" & CalcArea.ToString()
			End If
			If CalcArea2 <> 0.0 Then
				If sRes.Length <> 0 Then
					sRes &= vbCrLf
				End If
				sRes &= "Calc2=" & CalcArea2.ToString()
         End If
         If CalcGroupArea <> 0.0 Then
            If sRes.Length <> 0 Then
               sRes &= vbCrLf
            End If
            sRes &= "CalcGroup=" & CalcGroupArea.ToString()
         End If

			Return sRes
		End Function

	End Structure
	Public MustInherit Class TplnBasicPgon
		Inherits TPlanGraph.TplnTopoPgon
		Private Const mdPgonAreaTolearance As Double = 0.002
		Protected dsName As String = String.Empty
		Protected diOverlayPgonCounter() As Integer = {-1, -1, -1, -1}
      Protected diOrder As Long = 0
		Protected diTopoPurpose As DMAcadExt.enTopoPurpose
		Protected doaOverlayPgons(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnOverlayPgons
		Protected diaOverlayTopoID(DMAcadExt.enOverlayIndex.OverlayIndexUB) As List(Of Integer)	  '
		Protected dtaAreaSet(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnAreaSet
		Protected dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TplnAreaSet
		Protected doaAddOverlayPgons As TplnOverlayPgons
		Protected MustOverride ReadOnly Property PolygonCaption As String
		Protected MustOverride ReadOnly Property _PolygonFullName As String
		Private Shared miDebugCounterA As Integer
		Private Shared miDebugCounterB As Integer

		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon, Optional ByVal bDefineDirection As Boolean = False)

			MyBase.New(oPolygon, bDefineDirection)

		End Sub
		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, sXDAppName As String)
			MyBase.New(oPolygon, sXDAppName)

		End Sub
      Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, iFeatureID As Integer, tCentroidAcObjID As ObjectId)
         MyBase.New(oPolygon, iFeatureID, tCentroidAcObjID)

      End Sub
		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.Entity)
			MyBase.New(oPolygon)
		End Sub
		Public Overridable Sub AddOverlayPgon(ByRef oOverlayPgon As TplnOverlayPgon)
			If doaAddOverlayPgons Is Nothing Then
				doaAddOverlayPgons = New TplnOverlayPgons()
			End If
			doaAddOverlayPgons.AddPgon(oOverlayPgon)
		End Sub
		Public Overridable Sub AddOverlayPgon(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByRef oOverlayPgon As TplnOverlayPgon)
			If doaOverlayPgons(iOverlayIndex) Is Nothing Then
				doaOverlayPgons(iOverlayIndex) = New TplnOverlayPgons()
			End If
			Try
				'doaOverlayPgons(iOverlayIndex).Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
				doaOverlayPgons(iOverlayIndex).Add(oOverlayPgon)

				''''''''''''''''''''''''DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddOverlayPgon", diTopoPurpose, iOverlayIndex, oOverlayPgon.ParcelTopoID, oOverlayPgon.LotTopoID, oOverlayPgon.LotGroupNo, oOverlayPgon.LotOut)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("!!04_12:" & CStr(oOverlayPgon.TopoID))
			End Try

		End Sub
		Public Overridable Sub AddOverlayPgon(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oOverlayPgon As TplnOverlayPgon)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)

			Me.AddOverlayPgon(iOverlayIndex, oOverlayPgon)
		End Sub
		Public Overridable Sub AddOverlay(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iTopoID As Integer)
			Try
				If diaOverlayTopoID(iOverlayIndex) Is Nothing Then
					diaOverlayTopoID(iOverlayIndex) = New List(Of Integer)
				End If
				diaOverlayTopoID(iOverlayIndex).Add(iTopoID)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBasicPgon - AddOverlay")
			End Try

      End Sub
      Public Overridable ReadOnly Property OverlayCount(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Integer
         Get
            If diaOverlayTopoID(iOverlayIndex) Is Nothing Then
               Return -1
            Else
               Return diaOverlayTopoID(iOverlayIndex).Count
            End If
         End Get
      End Property
      Public Overridable Sub ClearOverlay(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iTopoID As Integer)
         Try
            If diaOverlayTopoID(iOverlayIndex) IsNot Nothing Then
               diaOverlayTopoID(iOverlayIndex).Clear()
            End If


         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBasicPgon - ClearOverlay")
         End Try

      End Sub
		Public ReadOnly Property PolygonFullName As String
			Get
				Return Me._PolygonFullName
			End Get
		End Property
	

		Public ReadOnly Property Name() As String
			Get
            Return dsName
			End Get

		End Property
		Public ReadOnly Property ParcelNo As Integer

			Get
				Dim iParcelNo As Integer = 0

				If Integer.TryParse(dsName, iParcelNo) Then
					Return iParcelNo
				Else
					Return 0
				End If
			End Get

		End Property
		Public Sub SetName(ByVal sName As String, ByVal bNameIsNum As Boolean)
			dsName = sName
			diOrder = GetNameOrder(sName, bNameIsNum)
		End Sub
		Public Shared Function GetNameOrder(ByVal sName As String, ByVal bNameIsNum As Boolean) As Integer
			Dim oComplexNum As NumerationPair.ComplexNum
            If bNameIsNum AndAlso sName IsNot Nothing AndAlso sName.Length <> 0 Then
                Try
                    oComplexNum = New NumerationPair.ComplexNum(sName)
                    Return oComplexNum.Order
                Catch oEx As Exception
                    System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sName, "BasicPgon-Name")
                    Return -1
                End Try
            Else
                Return -1
            End If
		End Function
		Private Sub zzTestStrArray(ByVal saVal() As String)
			Dim iUB As Integer = -1
			Dim sMsg As String = "---" & vbCrLf
			Try
				iUB = saVal.GetUpperBound(0)
				For iIndex As Integer = 0 To iUB
					If iIndex <> 0 Then sMsg &= vbCrLf
					sMsg = sMsg & ":" & saVal(iIndex) & ":"

				Next
				sMsg = sMsg & vbCrLf & "---"
			Catch ex As Exception
				System.Windows.Forms.MessageBox.Show(ex.Message, "ex: BasicPgon-New")
			End Try
			System.Windows.Forms.MessageBox.Show(sMsg, "ex: BasicPgon-New")
		End Sub
		Public Overridable ReadOnly Property CalcArea(ByVal bByPoligons As Boolean, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Double
			Get
				Dim dRes As Double
				If iTopoPurpose = enTopoPurpose.Undefined Then
					iTopoPurpose = diTopoPurpose
				End If
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
				If bByPoligons Then
					Dim oaUnionPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
					If oaUnionPgons IsNot Nothing Then
						For Each oUnionPgon As TplnOverlayPgon In oaUnionPgons.Values
							dRes += oUnionPgon.CalcArea()
						Next
						Return dRes	' / TplnUnionPgon.ScaleFactor ' 090609
					Else
						Return 0.0
					End If
				Else
					Dim bParcel As Boolean = (iTopoPurpose = DMAcadExt.enTopoPurpose.Parcel)
					Return Me.CalcArea(iOverlayIndex, bParcel)
				End If
			End Get

		End Property
		Public ReadOnly Property CalcArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal bParcel As Boolean) As Double
			Get
				Dim dRes As Double
				Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
				Dim oTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
				Dim oOverlayGroup As TplnOverlayGroup
				For i As Integer = 0 To 5

					'	DMCommon.ExcelLog.SetNextValue(8, "OverlayGroups", i, iOverlayIndex, TplnProject.OverlayGroups(CType(i, DMAcadExt.enOverlayIndex)).Keys.Count)
				Next
				If oTopoIDList IsNot Nothing Then
					For Each iOtherTopoID As Integer In oTopoIDList
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "", iOtherTopoID, iOverlayIndex, bParcel)
						If bParcel Then
							oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iOtherTopoID)
						Else
							oOverlayGroup = oAllOverlayGroups.GetItem(iOtherTopoID, diTopoID)
							'MessageBox.Show(CStr(iOtherTopoID) & ":" & CStr(diTopoID), "02_160")
						End If

						If oOverlayGroup IsNot Nothing Then
							dRes += oOverlayGroup.CalcArea
						Else

							DMCommon.Debug.MsgBox("01_888", diTopoID, iOtherTopoID, bParcel, Me.diTopoPurpose, iOverlayIndex, oAllOverlayGroups, oAllOverlayGroups.Count)
						End If
					Next
					Return dRes
				Else
					' Temp 0208 DMAcadExt.AcadDocument.WriteMessage("!!02_200: " & iOverlayIndex.ToString() & ":" & CStr(bParcel))
					Return Math.Round(Me.AcadArea(False) * TplnProject.CalcRoundFactor, MidpointRounding.AwayFromZero) / TplnProject.CalcRoundFactor
				End If
			End Get
		End Property
		Public Sub Calculate2(iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
			Dim oOtherTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			Dim oOverlayGroup As TplnOverlayGroup
			Dim bParcelLot As Boolean
			Dim iTest As Integer
			If diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel Then
				bParcelLot = True
			ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				bParcelLot = False
			ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				bParcelLot = False
			Else
				Return
			End If

			dtaAreaSet(iOverlayIndex).Reset()
			dtaInPlanAreaSet(iOverlayIndex).Reset()

			'		System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & diTopoPurpose.ToString() & vbCrLf & CStr(doaOverlayPgons(iOverlayIndex).Count) & vbCrLf & CStr(oOtherTopoIDList IsNot Nothing), "04_262")
			If oOtherTopoIDList IsNot Nothing Then
				'System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oOtherTopoIDList.Count), "04_766")
				For Each iTopoID As Integer In oOtherTopoIDList
					oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iTopoID, bParcelLot)
					If oOverlayGroup IsNot Nothing Then
						dtaAreaSet(iOverlayIndex).Add(oOverlayGroup.AreaSet)

						If oOverlayGroup.IsInPlan Then
							dtaInPlanAreaSet(iOverlayIndex).Add(oOverlayGroup.AreaSet)
						End If

					Else
						DMAcadExt.AcadDocument.WriteMessageLog("37_12: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
						'	System.Windows.Forms.MessageBox.Show(CStr(oOtherTopoIDList.Count), "04_266")
					End If
				Next
			Else
				iOverlayIndex = enOverlayIndex.ApprMerge ''''''''''''''''''''''''''TEMP DEBUG Only

				dtaInPlanAreaSet(iOverlayIndex).AcadArea = Me.AcadArea(False)
				iTest += 1


				'Temp 0208 DMAcadExt.AcadDocument.WriteMessageLog("01_500a: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
			End If
			If Me.TopoID = TplnLot.ID_Debug Then
				DMAcadExt.AcadDocument.WriteMessageLog("@51: " & dtaAreaSet(enOverlayIndex.ApprFDO_Overlay).ToString())
				dtaAreaSet(enOverlayIndex.ApprFDO_Overlay).ToString()
			End If
		End Sub

		''' <summary>
		''' <Active Condition="iRegion !=0">
		'''  </Active>
		'''
		''' </summary>
		''' <param name="iOverlayIndex"></param>
		''' <param name="iRegion"></param>
		Public Sub CalculateGroup(iOverlayIndex As DMAcadExt.enOverlayIndex, iRegion As Integer)
         Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
         Dim oOtherTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
         Dim oOverlayGroup As TplnOverlayGroup
         Dim bParcelLot As Boolean
         Dim iTest As Integer
         '   Dim iRow As Integer
         If diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel Then
            bParcelLot = True
         ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
            bParcelLot = False
         ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
            bParcelLot = False
         Else
            Return
         End If

         '   dtaAreaSet(iOverlayIndex).Reset()
         '    dtaInPlanAreaSet(iOverlayIndex).Reset()
         dtaInPlanAreaSet(iOverlayIndex).ResetGroup()
         '		System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & diTopoPurpose.ToString() & vbCrLf & CStr(doaOverlayPgons(iOverlayIndex).Count) & vbCrLf & CStr(oOtherTopoIDList IsNot Nothing), "04_262")
         If oOtherTopoIDList IsNot Nothing Then
            'System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oOtherTopoIDList.Count), "04_766")
            For Each iOtherTopoID As Integer In oOtherTopoIDList
               oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iOtherTopoID, bParcelLot)
               If oOverlayGroup IsNot Nothing Then
                  'dtaAreaSet(iOverlayIndex).Add(oOverlayGroup.AreaSet)
                  '	System.Windows.Forms.MessageBox.Show(CStr(oOverlayGroup.AreaSet.AcadArea) & vbCrLf & Str(oOverlayGroup.AreaSet.CalcArea) & vbCrLf & iOverlayIndex.ToString() & vbCrLf & diTopoPurpose.ToString() & vbCrLf & CStr(doaOverlayPgons(iOverlayIndex).Count) & vbCrLf & CStr(oOtherTopoIDList IsNot Nothing), "04_263")
                  If oOverlayGroup.GroupID = iRegion Then
                     dtaInPlanAreaSet(iOverlayIndex).AddGroup(oOverlayGroup.AreaSet)
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "CalculateGroup", iRegion, diTopoID, iOtherTopoID, oOverlayGroup.AreaSet.CalcArea, oOverlayGroup.AreaSet.CalcArea2, oOverlayGroup.AreaSet.CalcGroupArea)
						End If

               Else
                  DMAcadExt.AcadDocument.WriteMessageLog("37_12: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
                  '	System.Windows.Forms.MessageBox.Show(CStr(oOtherTopoIDList.Count), "04_266")
               End If
            Next
         Else
            iOverlayIndex = enOverlayIndex.ApprMerge ''''''''''''''''''''''''''TEMP DEBUG Only

            ''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(Me.AcadArea(False)), "04_975")
            dtaInPlanAreaSet(iOverlayIndex).AcadArea = Me.AcadArea(False)
            iTest += 1
            If iTest < 0 Then
               System.Windows.Forms.MessageBox.Show(CStr(diTopoID) & ":" & diTopoPurpose.ToString() & vbCrLf & CStr(Me.AcadArea(False)), "03_733a")
            End If

            'Temp 0208 DMAcadExt.AcadDocument.WriteMessageLog("01_500a: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
         End If
         If Me.TopoID = TplnLot.ID_Debug Then
            DMAcadExt.AcadDocument.WriteMessageLog("@51: " & dtaAreaSet(enOverlayIndex.ApprFDO_Overlay).ToString())
            dtaAreaSet(enOverlayIndex.ApprFDO_Overlay).ToString()
         End If
      End Sub
		Public Sub Calculate2(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
				If baOverlayArray(iOverlayIndex) Then
					Me.Calculate2(iOverlayIndex)
				End If
			Next

			If bMerge Then
				OnCalculate2(DMAcadExt.enOverlayMethod.Merge)
			End If
			If bUnion Then
				OnCalculate2(DMAcadExt.enOverlayMethod.Union)
			End If
			If bFDO_Overlay Then
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = enOverlayIndex.ApprFDO_Overlay
				If False AndAlso miDebugCounterA < 13 Then
					DMAcadExt.AcadDocument.WriteMessage("!!22_44: " & iOverlayIndex.ToString() & "--" & CStr(dtaInPlanAreaSet(iOverlayIndex).AcadArea) & ":" & CStr(dtaInPlanAreaSet(iOverlayIndex).CalcArea))
					miDebugCounterA += 1
				End If

				OnCalculate2(DMAcadExt.enOverlayMethod.FDO_Overlay)

				If False AndAlso miDebugCounterB < 9 Then

					miDebugCounterB += 1
				End If
			End If
		End Sub

		Public Sub Calculate2AAA(iOverlayMethod As DMAcadExt.enOverlayMethod)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex

			iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, diTopoPurpose)
			Calculate2(iOverlayIndex)


		End Sub
		Public Sub Calculate2(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim bOverlayExists As Boolean = False
			If bMerge Then
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, diTopoPurpose)
				Calculate2(iOverlayIndex)
				bOverlayExists = True
			End If
			If bUnion Then
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Union, diTopoPurpose)
				Calculate2(iOverlayIndex)
				bOverlayExists = True
			End If
			If bFDO_Overlay Then
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.FDO_Overlay, diTopoPurpose)
				Calculate2(iOverlayIndex)
				bOverlayExists = True
			End If
			If Not bOverlayExists Then
				'System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(ddAcadArea), "04_500")
				''''''''''''''''''''	TplnProject.WriteMessageBox(ddAcadArea.ToString() & ":" & diTopoPurpose.ToString(), "????01_502 BasicPgon")
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, diTopoPurpose)
				dtaAreaSet(iOverlayIndex).Reset()
				dtaAreaSet(iOverlayIndex).AcadArea = ddAcadArea
				dtaAreaSet(iOverlayIndex).RoundedArea = Math.Round(Me.AcadArea(False) * TplnProject.CalcRoundFactor, MidpointRounding.AwayFromZero) / TplnProject.CalcRoundFactor

				'	OnCalculate2()
			End If
		End Sub
		Public Sub TestAreaSet(iOverlayIndex As DMAcadExt.enOverlayIndex)
			zzTestWriteAreaSet(dtaAreaSet(iOverlayIndex), "All!! " & iOverlayIndex.ToString())
			zzTestWriteAreaSet(dtaInPlanAreaSet(iOverlayIndex), "InPlan!! " & iOverlayIndex.ToString())
		End Sub
		Private Sub zzTestWriteAreaSet(tAreaSet As TplnAreaSet, sLabel As String)
			Dim sText As String = sLabel & ": "
			sText &= CStr(tAreaSet.AcadArea) & "; "
			sText &= CStr(tAreaSet.RoundedArea) & "; "
			sText &= CStr(tAreaSet.CalcArea) & "; "
			sText &= CStr(tAreaSet.CalcArea2) & ";"

			DMAcadExt.AcadDocument.WriteMessage(sText)
		End Sub

		Public ReadOnly Property InPlanAreaSet(iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
			Get
				Return dtaInPlanAreaSet(iOverlayIndex)
			End Get
		End Property
      Public Property AreaSet(iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
         Get
            Return dtaAreaSet(iOverlayIndex)
         End Get
         Set(value As TplnAreaSet)

         End Set
      End Property
		''' <summary>
		''' Obsolete
		''' </summary>
		''' <param name="bByPolygons"></param>
		''' <param name="iOverlayMethod"></param>
		''' <param name="iTopoPurpose"></param>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property InPlanCalcArea(ByVal bByPolygons As Boolean, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Double
			Get
				Dim dSum As Double
				Try
					Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
					If bByPolygons Then
						Dim oaUnionPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
						If oaUnionPgons IsNot Nothing Then
							For Each oUnionPgon As TplnOverlayPgon In oaUnionPgons.Values
								dSum += oUnionPgon.InPlanCalcArea()
								If MyBase.diTopoID = 2117 Then
									DMAcadExt.AcadDocument.WriteMessage("Alla11: " & CStr(oUnionPgon.TopoID) & ":" & oUnionPgon.LotOut & ":" & CStr(oUnionPgon.InPlanCalcArea()))

								End If
							Next
							If MyBase.diTopoID = 2117 Then
								DMAcadExt.AcadDocument.WriteMessage("Alla22: " & CStr(dSum))
							End If
							Return dSum	'/ TplnUnionPgon.ScaleFactor
						Else
							Return 0.0
						End If
					Else
						Dim bParcel As Boolean = (iTopoPurpose = DMAcadExt.enTopoPurpose.Parcel)
						Return Me.InPlanCalcArea(iOverlayIndex, bParcel)
					End If

				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBasicPgon - InPlanCalcArea")
					Return 0.0
				End Try



			End Get

		End Property

		Public ReadOnly Property InPlanCalcArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal bParcel As Boolean) As Double
			Get
				Try
					Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
					Dim oTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
					Dim oOverlayGroup As TplnOverlayGroup
					Dim dRes As Double
					If oTopoIDList IsNot Nothing Then
						'	MessageBox.Show(CStr(oAllOverlayGroups.Count) & ":" & CStr(oTopoIDList.Count), "01_777")
						For Each iLotTopoID As Integer In oTopoIDList
							oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iLotTopoID)
							If oOverlayGroup IsNot Nothing Then
								dRes += oOverlayGroup.InPlanCalcArea
							Else
								DMAcadExt.AcadDocument.WriteMessageLog("37_12: " & CStr(diTopoID) & ":" & CStr(iLotTopoID))
							End If

						Next
						Return dRes
					Else
						Return 0.0
					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBasicPgon - InPlanCalcArea")
					Return 0.0
				End Try

			End Get
		End Property
		Protected MustOverride Sub OnCalculate2(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)

		Public MustOverride Sub AddDataToMainTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)

		Public ReadOnly Property TotalPolygonArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Double
			Get

				Dim oaOverlayPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
				If oaOverlayPgons IsNot Nothing Then
					Return oaOverlayPgons.TotalArea
				Else
					DMAcadExt.AcadDocument.WriteMessage("41_18: " & iOverlayIndex.ToString())
					Return 0.0
				End If
			End Get
		End Property

		Protected ReadOnly Property SumPolygonArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Double
			Get
				If iTopoPurpose = enTopoPurpose.Undefined Then
					iTopoPurpose = diTopoPurpose
				End If
				Dim dRes As Double
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
				Dim oaOverlayPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)

				If oaOverlayPgons IsNot Nothing Then
					For Each oOverlayPgon As TplnOverlayPgon In oaOverlayPgons.Values
						dRes += oOverlayPgon.AcadArea(False)
					Next
					Return dRes
				Else
					DMAcadExt.AcadDocument.WriteMessage("!SumPolygonArea: " & iOverlayMethod.ToString() & ":" & iTopoPurpose.ToString() & ":" & Me.CenterPosition.Coordinates2d)
					Return 0.0
				End If
			End Get
		End Property
		Public Function GetOuterPgons(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As TplnOverlayPgons
			If iTopoPurpose = enTopoPurpose.Undefined Then
				iTopoPurpose = diTopoPurpose
			End If

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oaOverlayPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
			Dim oRingPolyline As Polyline
			Dim oOverlayPgonPolyline As Polyline
			Dim colResPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()
			Dim iResPointsCount As Integer
			Dim dicOuterPgons As TplnOverlayPgons = New TplnOverlayPgons()
			Dim iLineIndex As Integer
			If oaOverlayPgons IsNot Nothing Then

				For Each oOverlayPgon As TplnOverlayPgon In oaOverlayPgons.Values
					iResPointsCount = 0
					iLineIndex = 0

					For Each tLineObjID As ObjectId In oOverlayPgon.Lines
						oOverlayPgonPolyline = AcadTransaction.GetPolyline(tLineObjID, OpenMode.ForWrite, True)
						If oOverlayPgonPolyline IsNot Nothing Then
							For iRingIndex As Integer = 0 To doaBulgeVertexArray.GetUpperBound(0)
								oRingPolyline = doaBulgeVertexArray(iRingIndex).CreatePolyline()
								oRingPolyline.IntersectWith(oOverlayPgonPolyline, Intersect.OnBothOperands, colResPoints, New IntPtr(0), New IntPtr(0))
								iResPointsCount = colResPoints.Count
								If False AndAlso iResPointsCount = 0 Then
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckBasicPgon", iLineIndex, iRingIndex, Me.GetCentroid(), Me.Name, iOverlayMethod, iTopoPurpose, Me.TopoID, MyBase.ddAcadArea, oOverlayPgonPolyline.Handle, iResPointsCount)
									DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckBasicPgon1", oRingPolyline.NumberOfVertices)
									For i As Integer = 0 To oRingPolyline.NumberOfVertices - 1
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckBasicPgon2", i, oRingPolyline.GetPoint2dAt(i).ToString())
									Next
								End If

								'TplnOverlayPgon.
								If iResPointsCount > 0 Then
									Exit For
								End If
							Next

						End If
						If iResPointsCount > 0 Then
							Exit For
						End If
						iLineIndex += 1
						oOverlayPgonPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 1S)
					Next
					If iResPointsCount = 0 Then
						dicOuterPgons.AddPgon(oOverlayPgon)
					End If
				Next
				Return dicOuterPgons
			Else
				DMAcadExt.AcadDocument.WriteMessage("!New??: " & iOverlayMethod.ToString() & ":" & iTopoPurpose.ToString() & ":" & Me.CenterPosition.Coordinates2d)
				Return dicOuterPgons
			End If
		End Function
		Public ReadOnly Property PolygonCount(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Integer
			Get
				If iTopoPurpose = enTopoPurpose.Undefined Then
					iTopoPurpose = diTopoPurpose
				End If
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
				Dim oaOverlayPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)

				If oaOverlayPgons IsNot Nothing Then
					Return oaOverlayPgons.Count
				Else
					Return 0
				End If
			End Get
		End Property

		Public Function CheckSumPolygonArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Double
			Dim sMsgText As String = "?"

			If iTopoPurpose = DMAcadExt.enTopoPurpose.Undefined Then
				iTopoPurpose = diTopoPurpose
			End If
         Dim dValue As Double = SumPolygonArea(iOverlayMethod, iTopoPurpose)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			If Not DMCommon.Functions.CheckAbsDeviation(MyBase.ddAcadArea, dValue, mdPgonAreaTolearance) Then
				Dim iPgonCount As Integer = PolygonCount(DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
				Dim bParcel As Boolean = (diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel)
				Dim sParcel As String
				Dim dicOuterPgons As TplnOverlayPgons
				If diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel Then
					sParcel = "1"
				Else
					sParcel = "0"
				End If

				Dim sSysID As String = "DVA," & CStr(CInt(iOverlayIndex)) & "," & sParcel & "," & CStr(Me.TopoID)
				sSysID = AppMessages.GetIntersectSysID(iOverlayIndex, diTopoPurpose, Me.TopoID)
				'	dMsgText = Me.PolygonCaption & ": " & Me.Name & "   #" & CStr(Me.TopoID) & "   " & "שטח:" & " " & DMCommon.Functions.RelRound(MyBase.ddAcadArea, 3)
				Try
					sMsgText = Me.PolygonFullName & "   #" & CStr(Me.TopoID) & "   " & "שטח:" & " " & DMCommon.Functions.RelRound(MyBase.ddAcadArea, 3)
					sMsgText &= vbCrLf & " " & "מס' פ':" & " " & CStr(iPgonCount) & "  " & "סה""כ שטח:" & " " & CStr(DMCommon.Functions.RelRound(dValue, 3))
					sMsgText &= vbCrLf & "הפרש: " & CStr(Math.Abs(DMCommon.Functions.RelRound(ddAcadArea - dValue, 3)))
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBasicPgon CheckSumPolygonArea")
				End Try
				Dim oBoundingBox As TPlnBoundingBox = Me.BoundingBox

				DMAcadExt.AppMessages.AddMessage(True, Me.GetCentroid(), oBoundingBox, sSysID, sMsgText, False, DMAcadExt.enMapTheme.ParcelsXApproved, 26, True, True)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!CheckSumPolygonArea", Me.GetCentroid(), Me.Name, iOverlayMethod, iTopoPurpose, Me.TopoID, MyBase.ddAcadArea, dValue, mdPgonAreaTolearance, DMCommon.Functions.CheckAbsDeviation(MyBase.ddAcadArea, dValue, mdPgonAreaTolearance), sMsgText)
				dicOuterPgons = GetOuterPgons(iOverlayMethod, iTopoPurpose)
				If dicOuterPgons.Count > 0 Then
					sMsgText = "מס' פוליגונים  " & dicOuterPgons.Count.ToString()
					For Each oOverlayPgon As TplnOverlayPgon In dicOuterPgons.Values
						sMsgText = Me.PolygonFullName & "   #" & CStr(Me.TopoID) & "   " & "שטח:" & " " & DMCommon.Functions.RelRound(oOverlayPgon.AcadArea(False), 3)
						oBoundingBox = oOverlayPgon.BoundingBox

						DMAcadExt.AppMessages.AddMessage(True, oOverlayPgon.GetCentroid(), oBoundingBox, sSysID, sMsgText, False, DMAcadExt.enMapTheme.ParcelsXApproved, 27, False, True)

					Next
				End If
			End If
			Return dValue
		End Function
		Public Function GetDifArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Double
			Dim dDif As Double = Math.Abs(ddAcadArea - Me.TotalPolygonArea(iOverlayIndex))
			If dDif < mdPgonAreaTolearance Then
				dDif = 0.0
			End If
			Return dDif
		End Function
		Public Overrides Sub Terminate()
			'Erase doaUnionPgonsProp
			'Erase doaUnionPgonsAppr
			If dsaBlockAttribText IsNot Nothing Then
				Erase dsaBlockAttribText
			End If
			MyBase.OnTerminate()
		End Sub
		Public Function GetOverlayPgons(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnOverlayPgons
			Return doaOverlayPgons(iOverlayIndex)
		End Function


	End Class

End Namespace
