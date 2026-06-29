Option Explicit On
Option Strict On
Namespace TPlanGraph


	Public Class TplnOverlayGroup
		Private Const mlFactor As ULong = 4294967296
		Private miParcelTopoID As Integer
      Private miLotTopoID As Integer
		Private miLotGroupID As Integer
		Private miLanduseID As Integer
		Private miBlockNo As Integer
		Private miBlockAddNo As Integer






		Private miPolygonCount As Integer = 1

		'	Private mdAcadArea As Double
		'	Private mdCalcArea As Double
		'	Private mdCalcArea2 As Double
		'	Private mdRoundedArea As Double
		Private mtAreaSet As TplnAreaSet

		Private mbLotOut As Boolean
		Public Shared Function GetOverlayKey(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer, ByVal sTestMsg As String) As ULong
			Try
				Dim lUp As ULong = Convert.ToUInt64(iParcelTopoID)
				Dim lDown As ULong = Convert.ToUInt64(iLotTopoID)
				Return mlFactor * lUp + lDown
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID) & vbCrLf & sTestMsg, "08_211")
				Return 1
			End Try


		End Function
		Public Shared Sub ParseOverlayKey(ByVal lOverlayKey As ULong, ByRef iParcelTopoID As Integer, ByRef iLotTopoID As Integer)
			Try
				Dim lDown As ULong = lOverlayKey Mod mlFactor
				Dim lUp As ULong = (lOverlayKey - lDown) \ mlFactor
				iParcelTopoID = Convert.ToInt32(lUp)
				iLotTopoID = Convert.ToInt32(lDown)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID), "08_212")

			End Try


		End Sub
		Public Sub New(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer, bLotOut As Boolean, iLotGroupID As Integer, iLanduseID As Integer, iBlockNo As Integer, iBlockAddNo As Integer)
			miParcelTopoID = iParcelTopoID
			miLotTopoID = iLotTopoID
			mbLotOut = bLotOut
			miLotGroupID = iLotGroupID
			miLanduseID = iLanduseID
			miBlockNo = iBlockNo
			miBlockAddNo = iBlockAddNo
		End Sub
		Public ReadOnly Property OverlayKeyAAA() As ULong
			Get
				Return GetOverlayKey(miParcelTopoID, miLotTopoID, "TplnOverlayGroup")
			End Get
		End Property
		Public Property LotOut() As Boolean
			Get
				Return mbLotOut
			End Get
			Set(ByVal bValue As Boolean)
				mbLotOut = bValue
			End Set
		End Property
		Public ReadOnly Property ParcelID() As Integer
			Get
				Return miParcelTopoID
			End Get
		End Property
		Public ReadOnly Property LotID() As Integer
			Get
				Return miLotTopoID
			End Get
		End Property
		Public ReadOnly Property BlockKey() As Integer
			Get

				Return BlockData.GetBlockKey(miBlockNo, miBlockAddNo)
			End Get
		End Property
		Public ReadOnly Property BlockNo() As Integer
			Get
				Return miBlockNo
			End Get
		End Property

		Public ReadOnly Property BlockAddNo() As Integer
			Get
				Return miBlockAddNo
			End Get
		End Property
		Public Property AreaSet As TplnAreaSet
			Get
				Return mtAreaSet
			End Get
			Set(tValue As TplnAreaSet)
				mtAreaset = tValue
			End Set
		End Property

		Public Property CalcArea() As Double
			Get
				'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''Return mtAreaSet.CalcArea
				If mtAreaSet.CalcArea = 0 Then
					Return mtAreaSet.AcadArea	'* TplnUnionPgon.ScaleFactor
				Else
					Return mtAreaSet.CalcArea	' / TplnUnionPgon.ScaleFactor
				End If
			End Get
			Set(ByVal dValue As Double)
				mtAreaSet.CalcArea = dValue
			End Set
		End Property
		Public Property CalcArea2() As Double
			Get
				
				Return mtAreaSet.CalcArea2	' / TplnUnionPgon.ScaleFactor

			End Get
			Set(ByVal dValue As Double)
				'	mdCalcArea2 = dValue
				mtAreaSet.CalcArea2 = dValue
			End Set
      End Property
      Public Property CalcGroupArea() As Double
         Get

            Return mtAreaSet.CalcGroupArea ' / TplnUnionPgon.ScaleFactor

         End Get
         Set(ByVal dValue As Double)

            mtAreaSet.CalcGroupArea = dValue
         End Set
      End Property
      Public Property CalcGroupArea2() As Double
         Get

            Return mtAreaSet.CalcGroupArea2 ' / TplnUnionPgon.ScaleFactor

         End Get
         Set(ByVal dValue As Double)

            mtAreaSet.CalcGroupArea2 = dValue
         End Set
      End Property
		Public Property RoundedArea() As Double
			Get
				Return mtAreaSet.RoundedArea
			End Get
			Set(ByVal dValue As Double)

				mtAreaSet.RoundedArea = dValue
			End Set
		End Property
		Public Property AcadArea() As Double
			Get
            Return mtAreaSet.AcadArea
			End Get
			Set(ByVal dValue As Double)
				'mdAcadArea = dValue
				mtAreaSet.AcadArea = dValue
			End Set
		End Property
		Public ReadOnly Property IsInPlan As Boolean
			Get

				If miLotTopoID = 0 Then
					Return False
				ElseIf mbLotOut Then
					Return False
				Else
					Return True
				End If
			End Get
		End Property
		Public ReadOnly Property InPlanCalcArea() As Double
			Get

				If miLotTopoID = 0 Then
					Return 0.0
				ElseIf mbLotOut Then
					Return 0.0
				Else
					Return Me.CalcArea
				End If
			End Get
		End Property
		Public ReadOnly Property PolygonCount() As Integer
			Get
				Return miPolygonCount
			End Get
		End Property
		Public ReadOnly Property GroupID() As Integer
			Get
				Return miLotGroupID
			End Get
		End Property
		Public ReadOnly Property LanduseID() As Integer
			Get
				Return miLanduseID

			End Get
		End Property
		Public ReadOnly Property LanduseKey() As ULong
			Get
				Dim lUp As ULong = Convert.ToUInt64(miParcelTopoID)
				Dim lDown As ULong = Convert.ToUInt64(miLanduseID)
				Return mlFactor * lUp + lDown
			End Get
		End Property
		Public Function SelectGroup_260421(bAllGroups As Boolean, iGroupID As Integer) As Boolean
			'If bAllGroups AndAlso miLotGroupID <> 0 Then
			If miLotTopoID = 0 Then
				Return False
			ElseIf bAllGroups Then
				Return IsInPlan
			ElseIf miLotGroupID = iGroupID Then
				Return True
			Else
				Return False
			End If

		End Function
		Public Function SelectGroup(bAllGroups As Boolean, iGroupID As Integer, Optional hsGroups As HashSet(Of Integer) = Nothing) As Boolean
			'If bAllGroups AndAlso miLotGroupID <> 0 Then
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!SelectGroup!", bAllGroups, iGroupID, IsInPlan)
			If miLotTopoID = 0 Then
				Return False
			ElseIf bAllGroups Then
				Return IsInPlan
			ElseIf iGroupID <> 0 AndAlso miLotGroupID = iGroupID Then
				Return True
			ElseIf iGroupID = 0 AndAlso hsGroups IsNot Nothing AndAlso hsGroups.Contains(miLotGroupID) Then
				Return True
			Else
				Return False
			End If

		End Function

		Public Sub AddAcadArea(ByVal dArea As Double)
			mtAreaSet.AcadArea += dArea
			'	mdAcadArea += dArea
			miPolygonCount += 1
		End Sub
		Public Function GetOptionArea(ByVal iDataOptions As TPlanGraph.enDataOptions) As Double
			Select Case iDataOptions
				Case enDataOptions.AcadArea
					Return mtAreaSet.AcadArea
				Case enDataOptions.CalcMergeArea
					Return mtAreaSet.CalcArea
				Case enDataOptions.CalcMergeArea2
					Return mtAreaSet.CalcArea2
				Case enDataOptions.RoundedArea
					Return mtAreaSet.RoundedArea
				Case Else
					Return 0.0
			End Select


		End Function
		Public Function GetTest() As String
			Return "(" & CStr(miParcelTopoID) & "," & CStr(miLotTopoID) & ")"
		End Function
	End Class
End Namespace
