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
		Public Shared Operator +(a As TplnAreaSet, b As TplnAreaSet) As TplnAreaSet
			Dim tResp As TplnAreaSet
			tResp.AcadArea = a.AcadArea + b.AcadArea
			tResp.RoundedArea = a.RoundedArea + b.RoundedArea
			tResp.CalcArea = a.CalcArea + b.CalcArea
			tResp.CalcArea2 = a.CalcArea2 + b.CalcArea2
			Return tResp
		End Operator
		Public Sub Reset()
			AcadArea = 0.0
			RoundedArea = 0.0
			CalcArea = 0.0
			CalcArea2 = 0.0
		End Sub

		Public Sub Add(b As TplnAreaSet)
			AcadArea += b.AcadArea
			RoundedArea += b.RoundedArea
			CalcArea += b.CalcArea
			CalcArea2 += b.CalcArea2
		End Sub
	End Structure
	Public MustInherit Class TplnBasicPgon
		Inherits TPlanGraph.TplnTopoPgon
		'   Private mdicUnionPgons As TPlanGraph.TplnUnionPgons
		' Private miaBlockAttribIndex() As Integer

		Protected dsName As String = String.Empty
		'	Protected doaMergePgonsAppr() As TplnUnionPgon = Nothing
		'   Protected doaUnionPgonsProp() As TplnUnionPgon = Nothing
		'   Protected doaUnionPgonsAppr() As TplnUnionPgon = Nothing
		Protected diOverlayPgonCounter() As Integer = {-1, -1, -1, -1}
		Protected diOrder As Integer = 0
		Protected diTopoPurpose As DMAcadExt.enTopoPurpose

		Protected doaUnionPgon(enOverlayIndex.OverlayIndexUB) As TplnUnionPgons
		Protected diaOverlayTopoID(enOverlayIndex.OverlayIndexUB) As List(Of Integer)	  '
		Protected dtaAreaSet(enOverlayIndex.OverlayIndexUB) As TplnAreaSet
		Protected dtaInPlanAreaSet(enOverlayIndex.OverlayIndexUB) As TplnAreaSet

		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon, Optional ByVal bDefineDirection As Boolean = False)
			MyBase.New(oPolygon, bDefineDirection)
		End Sub
		Public Sub SetAttributeOrder(ByVal iaBlockAttribIndex() As Integer)
			Dim x, o, a As TplnAreaSet
			x = a + o
			If iaBlockAttribIndex IsNot Nothing Then
				Try
					dsaBlockAttribText = AcadTransaction.GetAttribText(MyBase.diCentroidAcObjID, True, dbAcadPoint, iaBlockAttribIndex)
					If dsaBlockAttribText Is Nothing Then
						dbCorrect = False
					End If
					' zzTestStrArray(dsaBlockAttribText)
				Catch oEx As Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnBasicPgon - New")
				End Try
			End If
		End Sub
		Public Overridable Sub AddOverlayPgon(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByRef oUnionPgon As TplnOverlayPgon)
			If doaUnionPgon(iOverlayIndex) Is Nothing Then
				doaUnionPgon(iOverlayIndex) = New TplnUnionPgons()
			End If
			doaUnionPgon(iOverlayIndex).Add(oUnionPgon.TopoID, oUnionPgon)

		End Sub
		Public Overridable Sub AddOverlayPgon(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oUnionPgon As TplnOverlayPgon)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			'mdicUnionPgons.Add(oUnionPgon.TopoID, oUnionPgon)
			'Temp          
			Me.AddOverlayPgon(iOverlayIndex, oUnionPgon)
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


		Public ReadOnly Property Name() As String
			Get
				Return dsName
			End Get

		End Property
		Public Sub SetName(ByVal sName As String, ByVal bNameIsNum As Boolean)
			Dim oComplexNum As NumerationPair.ComplexNum
			dsName = sName
			If Name.Length <> 0 And bNameIsNum Then
				Try
					oComplexNum = New NumerationPair.ComplexNum(sName)
					diOrder = oComplexNum.Order
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, " BasicPgon-Name")
				End Try
			End If
		End Sub
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
					Dim oaUnionPgons As TplnUnionPgons = doaUnionPgon(iOverlayIndex)
					If oaUnionPgons IsNot Nothing Then
						For Each oUnionPgon As TplnOverlayPgon In oaUnionPgons.Values
							dRes += oUnionPgon.CalcArea()
						Next
						Return dRes	' / TplnUnionPgon.ScaleFactor ' 090609
					Else
						Return 0
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

				If oTopoIDList IsNot Nothing Then
					For Each iOtherTopoID As Integer In oTopoIDList
						If bParcel Then
							oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iOtherTopoID)
						Else
							oOverlayGroup = oAllOverlayGroups.GetItem(iOtherTopoID, diTopoID)
							'MessageBox.Show(CStr(iOtherTopoID) & ":" & CStr(diTopoID), "02_160")
						End If

						If oOverlayGroup IsNot Nothing Then
							dRes += oOverlayGroup.CalcArea
						Else
							'	MessageBox.Show(CStr(diTopoID) & ":" & CStr(iOtherTopoID) & ":" & CStr(bParcel) & ":" & Me.diTopoPurpose.ToString(), "01_888")
						End If
					Next
					Return dRes
				Else
					Return 0.0
				End If
			End Get

		End Property
		Public Sub Calculate2(iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim i As Integer = diTopoPurpose
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
			Dim oOtherTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			'	Dim tAreaSet As TplnAreaSet = dtaAreaSet(iOverlayIndex)
			'	Dim tInPlanAreaSet As TplnAreaSet = dtaInPlanAreaSet(iOverlayIndex)

			Dim oOverlayGroup As TplnOverlayGroup
			Dim dRes As Double
			Dim bParcelLot As Boolean
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
			If oOtherTopoIDList IsNot Nothing Then

				For Each iTopoID As Integer In oOtherTopoIDList
					oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iTopoID, bParcelLot)
					If oOverlayGroup IsNot Nothing Then
						dtaAreaSet(iOverlayIndex).Add(oOverlayGroup.AreaSet)

						If oOverlayGroup.IsInPlan Then
							dtaInPlanAreaSet(iOverlayIndex).Add(oOverlayGroup.AreaSet)
						End If
						'	DMAcadExt.AcadDocument.WriteMessage("!!22_37: " & CStr(tAreaSet.AcadArea) & ":" & CStr(oOverlayGroup.AreaSet.CalcArea) & ":" & CStr(tInPlanAreaSet.CalcArea))
					Else
						DMAcadExt.AcadDocument.WriteMessageLog("37_12: " & CStr(diTopoID) & ":" & CStr(iTopoID))
					End If
				Next
			Else
				DMAcadExt.AcadDocument.WriteMessageLog("01_500a: " & CStr(diTopoID))
			End If
		End Sub

		Public Sub Calculate2(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bApproved, bProposed)
			For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
				If baOverlayArray(iOverlayIndex) Then
					Me.Calculate2(iOverlayIndex)
				End If
			Next
			OnCalculate2()
		End Sub
		Public Sub Calculate2(iOverlayMethod As DMAcadExt.enOverlayMethod)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex

			iOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, diTopoPurpose)
			Calculate2(iOverlayIndex)


		End Sub
		Public Sub Calculate2(ByVal bMerge As Boolean, ByVal bUnion As Boolean)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			If bMerge Then
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, diTopoPurpose)
				Calculate2(iOverlayIndex)
			ElseIf bUnion Then
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Union, diTopoPurpose)
				Calculate2(iOverlayIndex)
			Else
				TplnProject.WriteMessageBox(ddAcadArea.ToString() & ":" & diTopoPurpose.ToString(), "01_502 BasicPgon")
				iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, diTopoPurpose)
				dtaAreaSet(iOverlayIndex).Reset()
				dtaAreaSet(iOverlayIndex).AcadArea = ddAcadArea
				dtaAreaSet(iOverlayIndex).RoundedArea = Math.Round(Me.AcadArea(False) * TplnProject.CalcRoundFactor, MidpointRounding.AwayFromZero) / TplnProject.CalcRoundFactor
				OnCalculate2()
			End If
		End Sub
		Public ReadOnly Property InPlanAreaSet(iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
			Get
				Return dtaInPlanAreaSet(iOverlayIndex)
			End Get
		End Property
		Public ReadOnly Property AreaSet(iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnAreaSet
			Get
				Return dtaAreaSet(iOverlayIndex)
			End Get
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
						Dim oaUnionPgons As TplnUnionPgons = doaUnionPgon(iOverlayIndex)
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
		Protected MustOverride Sub OnCalculate2()

		Public MustOverride Sub AddDataToMainTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
		Public ReadOnly Property SumPolygonArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, Optional ByVal iTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined) As Double
			Get
				Dim dRes As Double
				If iTopoPurpose = enTopoPurpose.Undefined Then
					iTopoPurpose = diTopoPurpose
				End If
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)

				Dim oaUnionPgons As TplnUnionPgons = doaUnionPgon(iOverlayIndex)
				If oaUnionPgons IsNot Nothing Then
					For Each oUnionPgon As TplnOverlayPgon In oaUnionPgons.Values
						dRes += oUnionPgon.AcadArea(False)
						If diTopoID = 2128 Then
							DMAcadExt.AcadDocument.WriteMessage("Alla30: " & CStr(oUnionPgon.AcadArea(False)))
						End If
					Next
					Return dRes
				Else
					Return 0.0
				End If
			End Get
		End Property

		Public Overrides Sub Terminate()
			'Erase doaUnionPgonsProp
			'Erase doaUnionPgonsAppr
			If dsaBlockAttribText IsNot Nothing Then
				Erase dsaBlockAttribText
			End If
			MyBase.OnTerminate()
		End Sub
		Protected Function GetUnionPgons(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnUnionPgons
			Return doaUnionPgon(iOverlayIndex)
		End Function
	End Class
End Namespace
