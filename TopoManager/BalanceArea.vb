Option Explicit On
Option Strict On
Imports System
Imports System.Collections.ObjectModel
Public Class BalanceArea
	Private miDataUB As Integer
	Private mdaSourceArea() As Double
	Private mdaRangeMinArea() As Double
	Private mdaRangeMaxArea() As Double

	Private miaGroupNo() As Integer
   Private mdaConstArea() As Double

   Private mdDestSum As Double
	'  Private mdDestSumRounded As Double
	Private mbDevPositive As Boolean
	Private mlaOutput() As Long
   Private mlDestSumOutput As Long

   Private mtaCells() As Cell
   Private mtaConstAreaGroup As IEnumerable(Of ConstArea)
   Private mtaConstAreaBasicPgon As IEnumerable(Of ConstArea)
   Private miBalanceLevel As enBalanceLevel
	'  Private m As Dictionary(Of Integer, Group)
	Private moGroupBalanceArea As BalanceArea
	Private mdRoundFactor As Double
   Private mbSuppressZero As Boolean
   Private msCaption As String
   Private mdicGroups As Groups
	Private mdicBasicPgons As BasicPgons
	Private mbDebug As Boolean
	Public Enum enBalanceLevel
      AllByCells
      AllByGroups
      AllByBasicPgons
      GroupByBasicPgons
      BasicPgonByCells
   End Enum
   Public Class Cell
      Public Index As Integer
      Public GroupNo As Integer
      Public BasicPgonID As Integer
      Public IsCell As Boolean
      Public IsGroup As Boolean
      Public IsBasicPgon As Boolean

      Public SourceArea As Double
      Public ConstArea As Double
      Public OutputAreaFix As Long
      Public OutputAreaFloat As Double

      Protected MinArea As Double
      Public Sub New(iBasedPgonID As Integer, iGroupNo As Integer)
         BasicPgonID = iBasedPgonID
         GroupNo = iGroupNo
         IsGroup = False
         IsBasicPgon = True
         IsCell = False
      End Sub
      Public Sub New(iGroupNo As Integer)
			GroupNo = iGroupNo
			IsGroup = True
         IsBasicPgon = False
         IsCell = False
      End Sub
      Public Sub New(dSourceArea As Double, iBasicPgonID As Integer, iGroupNo As Integer)
         SourceArea = dSourceArea
         BasicPgonID = iBasicPgonID
         GroupNo = iGroupNo
         IsGroup = False
         IsBasicPgon = False
         IsCell = True
      End Sub
      Public Sub ToOutput(dRoundFactor As Double)
         OutputAreaFloat = ConstArea
         OutputAreaFix = Convert.ToInt64(dRoundFactor * ConstArea)

      End Sub

   End Class
   Public Class Group
      Inherits Cell
      Private mdInnerConstArea As Double
		Private mcolCells As Collections.ObjectModel.Collection(Of Cell)
		Private moaCells() As Cell
		Private mcolBasicPgons As Collections.ObjectModel.Collection(Of BasicPgon)
		Private moaBasicPgons() As BasicPgon

		Private mdRoundFactor As Double
      Private mbSuppressZero As Boolean

		Public Sub New(iGroupNo As Integer, ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean)
         MyBase.New(iGroupNo)
         mcolCells = New System.Collections.ObjectModel.Collection(Of Cell)()
         mcolBasicPgons = New System.Collections.ObjectModel.Collection(Of BasicPgon)()
         mdRoundFactor = dRoundFactor
         mbSuppressZero = bSuppressZero
      End Sub
      Public Sub New(iGroupNo As Integer, dConstArea As Double)
         MyBase.New(iGroupNo)
         MyBase.ConstArea = dConstArea

      End Sub
      Public Sub AddCell(tCell As Cell)
         SourceArea += tCell.SourceArea
         mdInnerConstArea += tCell.ConstArea
         mcolCells.Add(tCell)
      End Sub
      Public Sub AddBasicPgon(oBasicPgon As BasicPgon)
         '  SourceArea += oBasicPgon.SourceArea
         mdInnerConstArea += oBasicPgon.ConstArea
         mcolBasicPgons.Add(oBasicPgon)
      End Sub

      Public Sub Calculate()
         ReDim moaBasicPgons(mcolBasicPgons.Count - 1)
         mcolBasicPgons.CopyTo(moaBasicPgons, 0)
         Dim oBalanceArea As BalanceArea = New BalanceArea(moaBasicPgons, enBalanceLevel.GroupByBasicPgons, OutputAreaFloat, mdRoundFactor, mbSuppressZero, "Pgon")
         For Each oBasicPgon As BasicPgon In moaBasicPgons
            oBasicPgon.Calculate()
         Next
      End Sub
 
   End Class
   Public Class BasicPgon
      Inherits Cell
      Private mdInnerConstArea As Double
		Private mcolCells As Collection(Of Cell)
		Private moaCells() As Cell
      Private mdRoundFactor As Double
      Private mbSuppressZero As Boolean


      'Dim GroupNo As Integer
      'Dim SourceArea As Double
      'Dim ConstArea As Double
      Public Sub New(iBasedPgonID As Integer, iGroupNo As Integer, ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean)
         MyBase.New(iBasedPgonID, iGroupNo)
         mcolCells = New System.Collections.ObjectModel.Collection(Of Cell)()
         mdRoundFactor = dRoundFactor
         mbSuppressZero = bSuppressZero
      End Sub
      Public Sub New(iBasedPgonID As Integer, dConstArea As Double)
         MyBase.New(iBasedPgonID)
         MyBase.ConstArea = dConstArea

      End Sub
      Public Sub AddCell(tCell As Cell)
         SourceArea += tCell.SourceArea
         mdInnerConstArea += tCell.ConstArea
         mcolCells.Add(tCell)

      End Sub
      Public Sub Calculate()
         ReDim moaCells(mcolCells.Count - 1)
         mcolCells.CopyTo(moaCells, 0)
#If Debug Then
         DMAcadExt.AcadDocument.WriteDebugMessage("^507 " & BasicPgonID.ToString & ": " & OutputAreaFloat.ToString() & "; RF=" & mdRoundFactor.ToString() & "; BPgon=" & moaCells(0).IsBasicPgon.ToString())
#End If

         Dim oBalanceArea As BalanceArea = New BalanceArea(moaCells, enBalanceLevel.BasicPgonByCells, OutputAreaFloat, mdRoundFactor, mbSuppressZero, "cell-Pgon#" & BasicPgonID.ToString())

      End Sub
     
   End Class
   Public Class BasicPgons
      Inherits Dictionary(Of Integer, BasicPgon)
      Private mdRoundFactor As Double
      Private mbSuppressZero As Boolean
      Public Sub New(ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean)
         mdRoundFactor = dRoundFactor
         mbSuppressZero = bSuppressZero

      End Sub

      Public Function AddCell(tCell As Cell) As BasicPgon
         Dim oBasedPgon As BasicPgon = Nothing
         Dim bPgonNew As Boolean = False
         If Not MyBase.TryGetValue(tCell.BasicPgonID, oBasedPgon) Then
            oBasedPgon = New BasicPgon(tCell.BasicPgonID, tCell.GroupNo, mdRoundFactor, mbSuppressZero)
            bPgonNew = True
            oBasedPgon.Index = MyBase.Count
            MyBase.Add(oBasedPgon.BasicPgonID, oBasedPgon)
         End If
         oBasedPgon.AddCell(tCell)
         If bPgonNew Then
            Return oBasedPgon
         Else
            Return Nothing
         End If

      End Function
      Public Sub SetConstArea(iGroupNo As Integer, dConstArea As Double)
         Dim oBasicPgon As BasicPgon = Nothing
         If MyBase.TryGetValue(iGroupNo, oBasicPgon) Then
            oBasicPgon.ConstArea = dConstArea
         End If
      End Sub
      Public Function GetAsCells() As TopoManager.BalanceArea.Cell()
         Return MyBase.Values.ToArray()
      End Function
   End Class
   Public Class Groups
      Inherits Dictionary(Of Integer, Group)
      Private mdRoundFactor As Double
      Private mbSuppressZero As Boolean
      Public Sub New(ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean)
         mdRoundFactor = dRoundFactor
         mbSuppressZero = bSuppressZero

      End Sub
      Public Sub AddCell(tCell As Cell)
         Dim oGroup As Group = Nothing

         If Not MyBase.TryGetValue(tCell.GroupNo, oGroup) Then
            oGroup = New Group(tCell.GroupNo, mdRoundFactor, mbSuppressZero)
            oGroup.Index = MyBase.Count
            MyBase.Add(oGroup.GroupNo, oGroup)
         End If
         oGroup.AddCell(tCell)
      End Sub
      Public Sub AddBasicPgon(oBasicPgon As BasicPgon)
         Dim oGroup As Group = Nothing

         If MyBase.TryGetValue(oBasicPgon.GroupNo, oGroup) Then
            oGroup.AddBasicPgon(oBasicPgon)
         End If

      End Sub
      Public Sub SetConstArea(iGroupNo As Integer, dConstArea As Double)
         Dim oGroup As Group = Nothing
         If MyBase.TryGetValue(iGroupNo, oGroup) Then
            oGroup.ConstArea = dConstArea
         End If
      End Sub
      Public Function GetAsCells() As TopoManager.BalanceArea.Cell()
         Return MyBase.Values.ToArray()
      End Function
   End Class
	Public Sub New(ByVal daSourceArea() As Double, ByVal dRoundFactor As Double, ByVal dDestSum As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String)
		miDataUB = daSourceArea.GetUpperBound(0)
		mdaSourceArea = daSourceArea
		mdRoundFactor = dRoundFactor

		mdDestSum = dDestSum
		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		ReDim mlaOutput(miDataUB)
		zzCalculate()
	End Sub
	Public Sub New(ByVal daSourceArea() As Double, ByVal daRangeMinArea() As Double, ByVal daRangeMaxArea() As Double, ByVal dRoundFactor As Double, ByVal dDestSum As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String)
		miDataUB = daSourceArea.GetUpperBound(0)
		mdaSourceArea = daSourceArea
		mdaRangeMinArea = daRangeMinArea
		mdaRangeMaxArea = daRangeMaxArea

		mdRoundFactor = dRoundFactor

		mdDestSum = dDestSum
		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		ReDim mlaOutput(miDataUB)
		zzCalculateRange(sDebugCaption)
	End Sub
	Public Structure ConstArea
      Dim ID As Integer
      Dim Area As Double
      Public Sub New(iID As Integer, dArea As Double)
         ID = iID
         Area = dArea
      End Sub
   End Structure
	Public Sub New(ByVal daSourceArea() As Double, ByVal iaGroupNo() As Integer, ByVal dRoundFactor As Double, ByVal dDestSum As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String, bDebug As Boolean)
		'   MessageBox.Show(daSourceArea.GetUpperBound(0).ToString() & ":" & iaGroupNo.GetUpperBound(0).ToString(), "07_100")
		miDataUB = daSourceArea.GetUpperBound(0)
		mdaSourceArea = daSourceArea
		miaGroupNo = iaGroupNo
		mbDebug = bDebug

		mdRoundFactor = dRoundFactor
		mdDestSum = dDestSum
		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		ReDim mlaOutput(miDataUB)
		zzGroupCalculateNew()
	End Sub
	Public Sub New(ByVal daSourceArea() As Double, ByVal iaGroupNo() As Integer, ByVal daConstArea() As Double, ByVal daGroupConstArea() As Double, ByVal dRoundFactor As Double, ByVal dDestSum As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String)
		MessageBox.Show(daSourceArea.GetUpperBound(0).ToString() & ":" & iaGroupNo.GetUpperBound(0).ToString())
		miDataUB = daSourceArea.GetUpperBound(0)
		mdaSourceArea = daSourceArea

		miaGroupNo = iaGroupNo
		mdaConstArea = daConstArea

		mdRoundFactor = dRoundFactor
		mdDestSum = dDestSum
		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		ReDim mlaOutput(miDataUB)
		zzGroupCalculateNew()
	End Sub
	Public Sub New(ByVal taCells() As Cell, taConstAreaBasicPgon As IEnumerable(Of ConstArea), taConstAreaGroup As IEnumerable(Of ConstArea), ByVal iBalanceLevel As enBalanceLevel, ByVal dDestSum As Double, ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String)
		mtaCells = taCells
		'DMCommon.Debug.MsgBoxLoop("03_503", taCells.Count, iBalanceLevel, taConstAreaBasicPgon.Count, taConstAreaGroup, dDestSum, dRoundFactor, bSuppressZero, sDebugCaption)

		mtaConstAreaGroup = taConstAreaGroup
		mtaConstAreaBasicPgon = taConstAreaBasicPgon
		miBalanceLevel = iBalanceLevel
		miDataUB = mtaCells.GetUpperBound(0)
		mdRoundFactor = dRoundFactor
		mdDestSum = dDestSum
		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		mdicGroups = New Groups(mdRoundFactor, mbSuppressZero)
		mdicBasicPgons = New BasicPgons(mdRoundFactor, mbSuppressZero)



		zzCalculateConst()
	End Sub

	Public Sub New(taCells() As Cell, ByVal iBalanceLevel As enBalanceLevel, ByVal dDestSum As Double, ByVal dRoundFactor As Double, ByVal bSuppressZero As Boolean, sDebugCaption As String)
		mtaCells = taCells
		'DMCommon.Debug.MsgBoxLoop("03_502", taCells.Count, iBalanceLevel, dDestSum, dRoundFactor, bSuppressZero, sDebugCaption)

		miBalanceLevel = iBalanceLevel
		miDataUB = mtaCells.GetUpperBound(0)
		mdDestSum = dDestSum
		mdRoundFactor = dRoundFactor

		mbSuppressZero = bSuppressZero
		msCaption = sDebugCaption
		If miBalanceLevel = enBalanceLevel.AllByCells Then

			mdicGroups = New Groups(mdRoundFactor, mbSuppressZero)
			mdicBasicPgons = New BasicPgons(mdRoundFactor, mbSuppressZero)
		End If


		zzCalculateConst()
	End Sub

	Public ReadOnly Property CellType As String
      Get
         If mtaCells Is Nothing OrElse mtaCells.GetUpperBound(0) <= 0 Then
            Return "Undef"
         ElseIf mtaCells(0).IsCell Then
            Return "Cell"
         ElseIf mtaCells(0).IsBasicPgon Then
            Return "Pgon"
         ElseIf mtaCells(0).IsGroup Then
            Return "Group"
         Else
            Return "TypeErr"
         End If
      End Get
   End Property
   Public ReadOnly Property OutputItemFix(ByVal iIndex As Integer) As Long
      Get
         If mtaCells IsNot Nothing Then
#If Not Debug Then
            'DMAcadExt.AcadDocument.WriteDebugMessage("^611 " & mtaCells(iIndex).OutputAreaFix.ToString() & "; Float=" & mtaCells(iIndex).OutputAreaFloat.ToString())
#End If

            Return mtaCells(iIndex).OutputAreaFix
         ElseIf mlaOutput IsNot Nothing Then
            Return mlaOutput(iIndex)
         Else
            Return 0
         End If



         Return mlaOutput(iIndex)
      End Get

   End Property
   Public ReadOnly Property OutputItemFloat(ByVal iIndex As Integer) As Double
      Get
         If mtaCells IsNot Nothing Then
            Return mtaCells(iIndex).OutputAreaFloat
         ElseIf mlaOutput IsNot Nothing Then
            Return mlaOutput(iIndex) / mdRoundFactor
         Else
            Return 0.0
         End If

      End Get

   End Property
	Public ReadOnly Property Output() As Long()
		Get
			Return mlaOutput
		End Get

	End Property
	Public ReadOnly Property GroupBalanceArea() As BalanceArea
		Get
			Return moGroupBalanceArea
		End Get

	End Property
	Public ReadOnly Property SourceArea() As Double()
		Get
			Return mdaSourceArea
		End Get

	End Property

	Public ReadOnly Property OutputFloat() As Double()
		Get
			Dim daOutputFloat(mlaOutput.GetUpperBound(0)) As Double
			For iIndex As Integer = 0 To mlaOutput.GetUpperBound(0)
				daOutputFloat(iIndex) = mlaOutput(iIndex) / mdRoundFactor
			Next
			Return daOutputFloat
		End Get

	End Property
	Public ReadOnly Property OutputValues() As System.Object()
		Get
			Dim oaOutputValues(mlaOutput.GetUpperBound(0)) As System.Object
			For iIndex As Integer = 0 To mlaOutput.GetUpperBound(0)
				oaOutputValues(iIndex) = mlaOutput(iIndex) / mdRoundFactor
			Next
			Return oaOutputValues
		End Get

	End Property

	Public Sub WriteTextWin()

   End Sub
   Private Sub zzCalcGroupsNew()


		Dim oGroupBalanceArea As BalanceArea = New BalanceArea(mdicGroups.GetAsCells(), enBalanceLevel.AllByGroups, mdDestSum, mdRoundFactor, mbSuppressZero, "BGroup")
		Dim oaCells() As Cell = oGroupBalanceArea.Cells
      Dim oGroup As Group
		If oaCells IsNot Nothing Then
			For Each oGroupAsCell As Cell In oaCells
				oGroup = DirectCast(oGroupAsCell, Group)
				oGroup.Calculate()
			Next
		Else
			DMCommon.Debug.MsgBoxLoop("L001")
		End If


	End Sub
   Private Sub zzCalcBasicPgon()
		'If mtaConstAreaBasicPgon IsNot Nothing Then
		'   For Each oConstAreaGroup As BasicPgon In mtaConstAreaBasicPgon
		'      mdicBasicPgons.SetConstArea(oConstAreaGroup.GroupNo, oConstAreaGroup.ConstArea)
		'   Next
		'End If

		Dim oBasicPgonBalanceArea As BalanceArea = New BalanceArea(mdicBasicPgons.GetAsCells(), enBalanceLevel.GroupByBasicPgons, mdDestSum, mdRoundFactor, mbSuppressZero, "BBasicPgon")
		Dim oaCells() As Cell = oBasicPgonBalanceArea.Cells
      Dim oBasicPgon As BasicPgon
#If Debug Then
For i As Integer = 0 To oaCells.GetUpperBound(0)
         DMAcadExt.AcadDocument.WriteDebugMessage("PgonOutput: " & OutputItemFloat(i).ToString())
      Next
#End If





      For Each oBasicPgonAsCell As Cell In oaCells
         oBasicPgon = DirectCast(oBasicPgonAsCell, BasicPgon)
         oBasicPgon.Calculate()
      Next

   End Sub
	Private Sub zzCalculateConst()
		' Dim daArea(miDataUB) As Double
		Dim dSourceSum As Double = 0.0
		Dim dConstSum As Double = 0.0
		Dim iConstCount As Integer = 0
		Dim dDestVar As Double
		Dim dSoureVar As Double

		Dim iDataUB_Var As Integer
		Dim lDestSumOutputVar As Long
		'	Dim iDirection As Integer = 0
		'	Dim dLimit As Double = 0.5
		'	Dim dStep As Double = dLimit * 0."
		'	Dim laOutput(miDataUB) As Long
		'   Dim laOutputPrev(miDataUB) As Long
		Dim sHist As String = "|"
		Dim oList As List(Of DoubleInd) = New List(Of DoubleInd)()
		Dim oBasedPgon As BasicPgon = Nothing
		Try
			For iIndex As Integer = 0 To miDataUB
				If mtaCells(iIndex) IsNot Nothing Then
					mtaCells(iIndex).Index = iIndex
					dSourceSum += mtaCells(iIndex).SourceArea
					If mtaCells(iIndex).ConstArea <> 0.0 Then
						dConstSum += mtaCells(iIndex).ConstArea
						iConstCount += 1
					Else
						dSoureVar += mtaCells(iIndex).SourceArea
					End If
					'If mtaCells(iIndex).IsCell Then
					If miBalanceLevel = enBalanceLevel.AllByCells Then
						mdicGroups.AddCell(mtaCells(iIndex))
					End If
					If miBalanceLevel = enBalanceLevel.AllByCells Then
						oBasedPgon = mdicBasicPgons.AddCell(mtaCells(iIndex))
						If oBasedPgon IsNot Nothing Then
							mdicGroups.AddBasicPgon(oBasedPgon)
						End If
					End If
				Else
					DMCommon.Debug.MsgBoxLoop("L008", msCaption, iIndex, miDataUB)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "L008a", msCaption, miDataUB, iIndex)
				End If

			Next
		Catch oEx As Exception
			DMCommon.Debug.MsgBoxLoop("L002", oEx.Message, oEx.StackTrace)
		End Try

		dDestVar = mdDestSum - dConstSum
		iDataUB_Var = miDataUB - iConstCount

		sHist &= CStr(miDataUB) & "," & CStr(dSourceSum) & "|"
		Try
			If mtaConstAreaGroup IsNot Nothing Then
				For Each oConstAreaGroup As ConstArea In mtaConstAreaGroup
					mdicGroups.SetConstArea(oConstAreaGroup.ID, oConstAreaGroup.Area)
				Next
			End If
		Catch oEx As Exception
			DMCommon.Debug.MsgBoxLoop("L003", oEx.Message, oEx.StackTrace)
		End Try
		Try
			If mtaConstAreaBasicPgon IsNot Nothing Then
				For Each oConstAreaBasicPgon As ConstArea In mtaConstAreaBasicPgon
					mdicBasicPgons.SetConstArea(oConstAreaBasicPgon.ID, oConstAreaBasicPgon.Area)
				Next
			End If
		Catch oEx As Exception
			DMCommon.Debug.MsgBoxLoop("L004", oEx.Message, oEx.StackTrace)
		End Try



		If mdicGroups IsNot Nothing AndAlso mdicGroups.Count > 1 Then

			zzCalcGroupsNew()
		End If
		If mdicBasicPgons IsNot Nothing AndAlso mdicBasicPgons.Count > 0 Then
			zzCalcBasicPgon()
		Else

			Dim dFactor As Double = mdRoundFactor * dDestVar / dSoureVar
			Dim dMin, dMax As Double
			Dim dArea As Double

			For iIndex As Integer = 0 To miDataUB

				If mtaCells(iIndex).ConstArea = 0.0 Then
					dArea = dFactor * mtaCells(iIndex).SourceArea
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "+" & msCaption, dArea, mtaCells(iIndex).SourceArea)
					oList.Add(New DoubleInd(iIndex, dArea, msCaption))
					dMin += Math.Floor(dArea)
					dMax += Math.Ceiling(dArea)
				Else
					mtaCells(iIndex).ToOutput(mdRoundFactor)
				End If

			Next
			oList.Sort(New ComparerFractions())
			'  mlDestSumOutput = Convert.ToInt64(mdRoundFactor * dDestVar)
			mlDestSumOutput = Convert.ToInt64(mdRoundFactor * mdDestSum)
			lDestSumOutputVar = Convert.ToInt64(mdRoundFactor * dDestVar)

			'     mdDestSumRounded = Math.Round(mlDestSumOutput / mlDestSumOutput, 8)
			If lDestSumOutputVar < CLng(dMin) OrElse lDestSumOutputVar > CLng(dMax) OrElse CLng(iDataUB_Var + 1) - (lDestSumOutputVar - CLng(dMin)) < 0L Then
				'MessageBox.Show(CStr(miDataUB) & ":" & CStr(mdDestSum) & ":" & CStr(mlDestSumOutput) & vbCrLf & CStr(CInt(dMin)) & vbCrLf & CStr(CInt(dMax)), "03_500b " & msCaption)
				DMCommon.Debug.MsgBoxLoop("L005", msCaption, miDataUB, mdDestSum, dMin, dMax)

				'  DMCommon.Functions.DispArray(mdaSourceArea, "03_888")
			Else
				Dim lNum1 As Long = lDestSumOutputVar - CLng(dMin)
				'  Dim lDelta0 As Long = CLng(dMax) - mlDestSumOutput
				Dim lMax0 As Long = CLng(iDataUB_Var + 1) - lNum1
				Dim lSum0, lSum1 As Long
				Dim tVal As DoubleInd
				For iIndex As Integer = 0 To oList.Count - 1
					tVal = oList.Item(iIndex)
					If lSum0 < lMax0 Then
						If Not (mbSuppressZero AndAlso tVal.LongValue = 0) Then
							lSum0 += 1
							mtaCells(tVal.Index).OutputAreaFix = tVal.LongValue
							mtaCells(tVal.Index).OutputAreaFloat = tVal.LongValue / mdRoundFactor

						ElseIf lSum1 < lNum1 Then
							lSum1 += 1
							mtaCells(tVal.Index).OutputAreaFix = tVal.LongValue + 1
							mtaCells(tVal.Index).OutputAreaFloat = (tVal.LongValue + 1) / mdRoundFactor
						Else
							lSum0 += 1
							mtaCells(tVal.Index).OutputAreaFix = tVal.LongValue
							mtaCells(tVal.Index).OutputAreaFloat = tVal.LongValue / mdRoundFactor
							'	DMAcadExt.AcadDocument.WriteMessage("Balance_A1:" & CStr(lSum0) & "," & CStr(lSum1) & " : " & CStr(lMax0) & "," & CStr(lNum1) & " " & msCaption)
							'	DMCommon.Functions.DispArray(mdaSourceArea, "03_877 " & CStr(mlDestSumOutput))
						End If
					ElseIf lSum1 < lNum1 Then
						lSum1 += 1
						mtaCells(tVal.Index).OutputAreaFix = tVal.LongValue + 1
						mtaCells(tVal.Index).OutputAreaFloat = (tVal.LongValue + 1) / mdRoundFactor
					Else
						MessageBox.Show(CStr(lSum0) & ":" & CStr(lSum1) & ":" & CStr(lMax0) & vbCrLf & CStr(lNum1), "03_510 " & msCaption)
						DMCommon.Functions.DispArray("03_899", mdaSourceArea)
						DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaColumns Lots", mdaSourceArea)
					End If
				Next

				Dim lCheck As Long
				For iIndex As Integer = 0 To miDataUB
					lCheck += mtaCells(iIndex).OutputAreaFix
				Next
				If lCheck <> mlDestSumOutput Then
					MessageBox.Show(CStr(lCheck) & vbCrLf & CStr(mlDestSumOutput) & vbCrLf & CStr(mdDestSum), "03_602 " & msCaption)
					DMCommon.Functions.DispArray("03_902 " & CStr(mlDestSumOutput), mdaSourceArea)
				End If
			End If
		End If



	End Sub

	Public ReadOnly Property Cells() As Cell()
      Get
         Return mtaCells
      End Get
   End Property
   Public ReadOnly Property GroupsAsCells() As Cell()
      Get
         Return mdicGroups.GetAsCells()
      End Get
   End Property
   Public ReadOnly Property BasicPgonsAsCells() As Cell()
      Get
         Return mdicBasicPgons.GetAsCells()
      End Get
   End Property

	Private Sub zzCalculate()
		Dim daArea(miDataUB) As Double
		Dim dSourceSum As Double = 0.0
		'	Dim iDirection As Integer = 0
		'	Dim dLimit As Double = 0.5
		'	Dim dStep As Double = dLimit * 0.5
		'	Dim laOutput(miDataUB) As Long
		Dim laOutputPrev(miDataUB) As Long
		Dim sHist As String = "|"
		Dim oList As List(Of DoubleInd) = New List(Of DoubleInd)()

		For iIndex As Integer = 0 To miDataUB
			dSourceSum += mdaSourceArea(iIndex)
		Next
		'	DMCommon.Debug.MsgBox("13_210", dSourceSum)
		'	DMCommon.Functions.DispArray(mdaSourceArea, "03_888c")
		sHist &= CStr(miDataUB) & "," & CStr(dSourceSum) & "|"
		Dim dFactor As Double = mdRoundFactor * mdDestSum / dSourceSum
		Dim dMin, dMax As Double
		For iIndex As Integer = 0 To miDataUB
			daArea(iIndex) = dFactor * mdaSourceArea(iIndex)
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!" & msCaption, daArea(iIndex), dFactor, mdaSourceArea(iIndex))
			oList.Add(New DoubleInd(iIndex, daArea(iIndex), msCaption))
			dMin += Math.Floor(daArea(iIndex))
			dMax += Math.Ceiling(daArea(iIndex))
		Next


		oList.Sort(New ComparerFractions())
		mlDestSumOutput = Convert.ToInt64(mdRoundFactor * mdDestSum)


		'    mdDestSumRounded = Math.Round(mlDestSumOutput / mlDestSumOutput, 8)
		If mlDestSumOutput < CLng(dMin) OrElse mlDestSumOutput > CLng(dMax) OrElse CLng(miDataUB + 1) - (mlDestSumOutput - CLng(dMin)) < 0L Then
			'''''''''''''''''''''''''''''''''''''''''''' TEMP  MessageBox.Show(CStr(miDataUB) & ":" & CStr(mdDestSum) & ":" & CStr(mlDestSumOutput) & vbCrLf & CStr(CInt(dMin)) & vbCrLf & CStr(CInt(dMax)), "03_500a " & msCaption)
			''''''''''''''''''''''''''''''''''''''''''''TEMP DMCommon.Functions.DispArray("03_888", mdaSourceArea)
		Else
			Dim lNum1 As Long = mlDestSumOutput - CLng(dMin)
			'		Dim lDelta0 As Long = CLng(dMax) - mlDestSumOutput
			Dim lMax0 As Long = CLng(miDataUB + 1) - lNum1
			Dim lSum0, lSum1 As Long
			Dim tVal As DoubleInd
			For iIndex As Integer = 0 To miDataUB
				tVal = oList.Item(iIndex)
				If lSum0 < lMax0 Then
					If Not (mbSuppressZero AndAlso tVal.LongValue = 0 AndAlso (Not tVal.IsZero)) Then
						lSum0 += 1
						mlaOutput(tVal.Index) = tVal.LongValue
					ElseIf lSum1 < lNum1 Then
						lSum1 += 1
						mlaOutput(tVal.Index) = tVal.LongValue + 1
					Else
						lSum0 += 1
						mlaOutput(tVal.Index) = tVal.LongValue

						'	DMAcadExt.AcadDocument.WriteMessage("Balance_A1:" & CStr(lSum0) & "," & CStr(lSum1) & " : " & CStr(lMax0) & "," & CStr(lNum1) & " " & msCaption)
						'	DMCommon.Functions.DispArray(mdaSourceArea, "03_877 " & CStr(mlDestSumOutput))
					End If
				ElseIf lSum1 < lNum1 Then
					lSum1 += 1
					mlaOutput(tVal.Index) = tVal.LongValue + 1
				Else
					MessageBox.Show(CStr(lSum0) & ":" & CStr(lSum1) & ":" & CStr(lMax0) & vbCrLf & CStr(lNum1), "03_510 " & msCaption)
					DMCommon.Functions.DispArray("03_899", mdaSourceArea)
				End If
			Next

			Dim lCheck As Long
			For iIndex As Integer = 0 To miDataUB
				lCheck += mlaOutput(iIndex)
			Next
			If lCheck <> mlDestSumOutput Then
				MessageBox.Show(CStr(lCheck) & vbCrLf & CStr(mlDestSumOutput) & vbCrLf & CStr(mdDestSum), "03_601 " & msCaption)
				DMCommon.Functions.DispArray("03_901 " & CStr(mlDestSumOutput), mdaSourceArea)
			End If
		End If


	End Sub
	Private Function zzGetCellsSourceArea() As Double()
		Dim daRes(mtaCells.GetUpperBound(0)) As Double
		For iIndex As Integer = 0 To mtaCells.GetUpperBound(0)
			daRes(iIndex) = mtaCells(iIndex).SourceArea
		Next
		Return daRes
	End Function
	Private Sub zzCalculateRange(sDebugCaption As String)
		Dim daArea(miDataUB) As Double
		Dim dSourceSum As Double = 0.0
		'	Dim iDirection As Integer = 0
		'	Dim dLimit As Double = 0.5
		'	Dim dStep As Double = dLimit * 0.5
		'	Dim laOutput(miDataUB) As Long
		Dim laOutputPrev(miDataUB) As Long
		Dim sHist As String = "|"
		Dim oList As List(Of DoubleInd) = New List(Of DoubleInd)()
		Dim tDoubleInd As DoubleInd
		For iIndex As Integer = 0 To miDataUB
			dSourceSum += mdaSourceArea(iIndex)
		Next
		mbDevPositive = (dSourceSum >= mdDestSum)
		sHist &= CStr(miDataUB) & "," & CStr(dSourceSum) & "|"
		Dim dFactor As Double = mdRoundFactor * mdDestSum / dSourceSum
		Dim dTotalMin, dTotalMax As Double
		Dim lTotalRangeMin, lTotalRangeMax As Long
		Dim bNeedCalculate As Boolean

		For iIndex As Integer = 0 To miDataUB
			daArea(iIndex) = dFactor * mdaSourceArea(iIndex)
			tDoubleInd = New DoubleInd(iIndex, mdaSourceArea(iIndex), dFactor)
			tDoubleInd.RangeMin = CLng(Math.Ceiling(mdRoundFactor * mdaRangeMinArea(iIndex)))
			tDoubleInd.RangeMax = CLng(Math.Floor(mdRoundFactor * mdaRangeMaxArea(iIndex)))

			oList.Add(tDoubleInd)
			dTotalMin += Math.Floor(daArea(iIndex))
			dTotalMax += Math.Ceiling(daArea(iIndex))
			lTotalRangeMin += tDoubleInd.RangeMin
			lTotalRangeMax += tDoubleInd.RangeMax


		Next
		oList.Sort(New ComparerDevFractions(mbDevPositive))
		mlDestSumOutput = Convert.ToInt64(mdRoundFactor * mdDestSum)

		'DMCommon.Debug.MsgBox("13_025", dTotalMin, dTotalMax, lTotalRangeMin, lTotalRangeMax)
		'    mdDestSumRounded = Math.Round(mlDestSumOutput / mlDestSumOutput, 8)
		If mlDestSumOutput < CLng(dTotalMin) OrElse mlDestSumOutput > CLng(dTotalMax) OrElse CLng(miDataUB + 1) - (mlDestSumOutput - CLng(dTotalMin)) < 0L Then
			MessageBox.Show(CStr(miDataUB) & ":" & CStr(mdDestSum) & ":" & CStr(mlDestSumOutput) & vbCrLf & CStr(CInt(dTotalMin)) & vbCrLf & CStr(CInt(dTotalMax)), "03_500a " & msCaption)
			DMCommon.Functions.DispArray("03_888", mdaSourceArea)
		Else
			Dim lNum1 As Long = mlDestSumOutput - CLng(dTotalMin)
			'		Dim lDelta0 As Long = CLng(dMax) - mlDestSumOutput
			Dim lMax0 As Long = CLng(miDataUB + 1) - lNum1

			Dim tVal As DoubleInd
			Dim iRemIndex As Integer = 0

			Do
				bNeedCalculate = False
				For iIndex As Integer = 0 To oList.Count - 1
					tVal = oList.Item(iIndex)
					If tVal.SetValueByRange Then
						mdDestSum -= tVal.Result
						dSourceSum -= tVal.SourceValue
						mlaOutput(tVal.Index) = tVal.Result
						oList.RemoveAt(iIndex)
						bNeedCalculate = True
						Exit For
					End If

				Next
				If bNeedCalculate Then
					dFactor = mdRoundFactor * mdDestSum / dSourceSum
					For iIndex As Integer = 0 To oList.Count - 1
						tVal = oList.Item(iIndex)
						tVal.Calculate(dFactor)

					Next
					oList.Sort(New ComparerDevFractions(mbDevPositive))
				Else
					Exit Do
				End If

			Loop
			If oList.Count > 0 Then
				Dim daRemArea(oList.Count - 1) As Double
				Dim iaRemIndecis(oList.Count - 1) As Integer

				For iIndex As Integer = 0 To oList.Count - 1
					tVal = oList.Item(iIndex)
					'	DMCommon.Debug.MsgBox("13_025a", tVal.LongValue, tVal.RangeMin, tVal.RangeMax)

					daRemArea(iRemIndex) = tVal.DoubleValue
					iaRemIndecis(iRemIndex) = tVal.Index
					iRemIndex += 1
				Next

				Dim oBalanceArea As BalanceArea
				oBalanceArea = New TopoManager.BalanceArea(daRemArea, mdRoundFactor, mdDestSum, mbSuppressZero, msCaption & "_Add")
				For iNewIndex As Integer = 0 To iRemIndex - 1
					mlaOutput(iaRemIndecis(iNewIndex)) = oBalanceArea.OutputItemFix(iNewIndex)
				Next

			End If


		End If
	End Sub



	Private Sub zzGroupCalculateNew()


		Dim daGroupArea() As Double = Nothing
		Dim iGroupUB As Integer = -1
		Dim iGroupNo As Integer
		Dim iGroupIndex As Integer = -1
		Dim iGroupNewIndex As Integer

		Dim dicNoToIndex As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
		Dim hsaIndices() As HashSet(Of Integer) = Nothing
		For iIndex As Integer = 0 To miDataUB
			iGroupNo = miaGroupNo(iIndex)
			If Not dicNoToIndex.TryGetValue(iGroupNo, iGroupNewIndex) Then
				iGroupIndex += 1
				dicNoToIndex.Add(iGroupNo, iGroupIndex)
			End If
		Next
		iGroupUB = dicNoToIndex.Count - 1
		ReDim Preserve daGroupArea(iGroupUB)
		ReDim Preserve hsaIndices(iGroupUB)

		For iIndex As Integer = 0 To miDataUB
			iGroupNo = miaGroupNo(iIndex)
			iGroupIndex = dicNoToIndex.Item(iGroupNo)
			daGroupArea(iGroupIndex) += mdaSourceArea(iIndex)

			If hsaIndices(iGroupIndex) Is Nothing Then
				hsaIndices(iGroupIndex) = New HashSet(Of Integer)()
			End If
			hsaIndices(iGroupIndex).Add(iIndex)

		Next


		Dim oBalanceArea As BalanceArea
		Dim daSourceArea() As Double
		Dim iInnerIndex As Integer
		Dim iaTest0() As Integer
		'Dim iaTest1() As Integer
		If mbDebug Then
			''''''''''''DMCommon.ExcelLogAW3.SetNextArray(daGroupArea, 2, "daGroupArea")

			''''''''''''''''	DMCommon.ExcelLogAW3.SetNextValue(1, mdDestSum, " mdDestSum")
			If hsaIndices(0) IsNot Nothing AndAlso hsaIndices(0).Count > 0 Then
				ReDim iaTest0(hsaIndices(0).Count - 1)
				iaTest0 = hsaIndices(0).ToArray()
				DMCommon.Debug.ExcelLog.SetArray(0, " 1hsaIndices(0).ToArray", True, iaTest0)
			Else
				DMCommon.Debug.ExcelLog.SetNextValue(1, " 2hsaIndices(0).ToArray Is NOTHING")
			End If

			DMCommon.Debug.ExcelLog.SetArray(0, "Bal3", True, daGroupArea)
		End If



		'	Dim oGroupBalanceArea As BalanceArea = New BalanceArea(daGroupArea, mdRoundFactor, mdDestSum, mbSuppressZero )
		moGroupBalanceArea = New BalanceArea(daGroupArea, mdRoundFactor, mdDestSum, mbSuppressZero, msCaption)





		For iGroupNo = 0 To iGroupUB
			If hsaIndices(iGroupNo) IsNot Nothing AndAlso hsaIndices(iGroupNo).Count > 0 Then
				ReDim daSourceArea(hsaIndices(iGroupNo).Count - 1)
				iInnerIndex = 0
				For Each iIndex As Integer In hsaIndices(iGroupNo)
					daSourceArea(iInnerIndex) = mdaSourceArea(iIndex)
					iInnerIndex += 1
				Next
				If mbDebug Then
					''''''	DMCommon.ExcelLogAW3.SetValue(5, iGroupNo, "oGroupBalanceArea", oGroupBalanceArea.OutputItemFloat(iGroupNo))
					''''''''''''''''	DMCommon.ExcelLogAW3.SetNextArray(daSourceArea, 3)
				End If

				oBalanceArea = New BalanceArea(daSourceArea, mdRoundFactor, moGroupBalanceArea.OutputItemFloat(iGroupNo), mbSuppressZero, msCaption)
				iInnerIndex = 0
				For Each iIndex As Integer In hsaIndices(iGroupNo)
					mlaOutput(iIndex) = oBalanceArea.Output(iInnerIndex)
					iInnerIndex += 1
				Next
			End If
		Next
	End Sub

	Private Sub zzGroupCalculate()
      Dim daGroupArea() As Double = Nothing
      Dim iGroupUB As Integer = -1
      Dim iGroupNo As Integer
      Dim hsaIndices() As HashSet(Of Integer) = Nothing
      For iIndex As Integer = 0 To miDataUB
         iGroupNo = miaGroupNo(iIndex)
         If iGroupUB < iGroupNo Then
            iGroupUB = iGroupNo
            ReDim Preserve daGroupArea(iGroupUB)
            ReDim Preserve hsaIndices(iGroupUB)

         End If
         daGroupArea(iGroupNo) += mdaSourceArea(iIndex)
         If hsaIndices(iGroupNo) Is Nothing Then
            hsaIndices(iGroupNo) = New HashSet(Of Integer)()
         End If
         hsaIndices(iGroupNo).Add(iIndex)
			'''''''''''''''''''DMCommon.ExcelLogAW3.SetNextArray(daGroupArea, 2, "daGroupArea")
		Next
      Dim oBalanceArea As BalanceArea
      Dim daSourceArea() As Double
      Dim iInnerIndex As Integer
		Dim iaTest0() As Integer
		Dim iaTest1() As Integer
		If mbDebug Then
			''''''''''''DMCommon.ExcelLogAW3.SetNextArray(daGroupArea, 2, "daGroupArea")

			''''''''''''''''	DMCommon.ExcelLogAW3.SetNextValue(1, mdDestSum, " mdDestSum")
			If hsaIndices(0) IsNot Nothing AndAlso hsaIndices(0).Count > 0 Then
				ReDim iaTest0(hsaIndices(0).Count - 1)
				iaTest0 = hsaIndices(0).ToArray()
				DMCommon.Debug.ExcelLog.SetArray(0, " hsaIndices(0).ToArray", True, iaTest0)
			Else
				DMCommon.Debug.ExcelLog.SetNextValue(1, " hsaIndices(0).ToArray Is NOTHING")
			End If

			If hsaIndices(1) IsNot Nothing AndAlso hsaIndices(1).Count > 0 Then
				ReDim iaTest1(hsaIndices(1).Count - 1)
				iaTest1 = hsaIndices(1).ToArray()
				'''''''''''DMCommon.ExcelLogAW3.SetNextArray(iaTest1, 1, " hsaIndices(1).ToArray")
			Else
				'''''''''''''DMCommon.ExcelLogAW3.SetNextValue(1, " hsaIndices(1).ToArray Is NOTHING")
			End If

			DMCommon.Debug.ExcelLog.SetArray(0, "Bal3", True, daGroupArea)
		End If



		'	Dim oGroupBalanceArea As BalanceArea = New BalanceArea(daGroupArea, mdRoundFactor, mdDestSum, mbSuppressZero )
		Dim oGroupBalanceArea As BalanceArea = New BalanceArea(daGroupArea, mdRoundFactor, mdDestSum, False, msCaption)





		For iGroupNo = 0 To iGroupUB
         If hsaIndices(iGroupNo) IsNot Nothing AndAlso hsaIndices(iGroupNo).Count > 0 Then
            ReDim daSourceArea(hsaIndices(iGroupNo).Count - 1)
            iInnerIndex = 0
            For Each iIndex As Integer In hsaIndices(iGroupNo)
               daSourceArea(iInnerIndex) = mdaSourceArea(iIndex)
               iInnerIndex += 1
            Next
				If mbDebug Then
					''''''	DMCommon.ExcelLogAW3.SetValue(5, iGroupNo, "oGroupBalanceArea", oGroupBalanceArea.OutputItemFloat(iGroupNo))
					''''''''''''''''	DMCommon.ExcelLogAW3.SetNextArray(daSourceArea, 3)
				End If
				oBalanceArea = New BalanceArea(daSourceArea, mdRoundFactor, oGroupBalanceArea.OutputItemFloat(iGroupNo), mbSuppressZero, msCaption)
				iInnerIndex = 0
            For Each iIndex As Integer In hsaIndices(iGroupNo)
               mlaOutput(iIndex) = oBalanceArea.Output(iInnerIndex)
               iInnerIndex += 1
            Next
         End If
      Next
   End Sub
	'053 621 29 85 Vova L
	'   Ermiyhu 37
	'  119
	' 84 
	'BG - 55
	Private Function zzGetMinSum(dFactor As Double) As Integer
      Dim iRes As Integer = 0
      For iIndex As Integer = 0 To miDataUB

         iRes = iRes + CInt(Math.Floor(dFactor * mdaSourceArea(iIndex)))
      Next
      Return iRes
   End Function
   Private Function zzGetMaxSum(dFactor As Double) As Integer
      Dim iRes As Integer = 0
      For iIndex As Integer = 0 To miDataUB

         iRes = iRes + CInt(Math.Ceiling(dFactor * mdaSourceArea(iIndex)))
      Next
      Return iRes
   End Function

   Private Shared Function zzIntRound(ByVal dValue As Double, ByVal dLimit As Double, ByVal bSuppressZero As Boolean) As Long
      Dim dIntVal As Double
      Dim lIntVal As Long

      dIntVal = Math.Floor(dValue)
      lIntVal = CLng(dIntVal)
      If (dValue - dIntVal >= dLimit) Then


         Return lIntVal + 1L
      ElseIf lIntVal = 0 AndAlso bSuppressZero Then
         Return 1L
      Else
         Return lIntVal
      End If
   End Function
   Private Shared Sub zzDispArray(ByVal iaVal() As Double)
      Dim sOut As String = String.Empty
      For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
         sOut += ":" & Convert.ToString(iaVal(iIndex))
      Next

      DMAcadExt.AcadDocument.WriteMessageLog("Array:" & sOut & vbCrLf)
   End Sub
   Private Shared Sub zzDispIntArray(ByVal iaVal() As Integer)

      Dim sOut As String = String.Empty

      For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
         sOut += ":" & iaVal(iIndex).ToString()
      Next

      DMAcadExt.AcadDocument.WriteMessageLog("Array:" & sOut & vbCrLf)
      DMAcadExt.AcadDocument.WriteMessageLog("Int Array:" & sOut & vbCrLf)
   End Sub
   Private Sub zzDispIntArray(ByVal iaVal() As Long, ByVal sTitle As String)
      Dim sOut As String = String.Empty
      For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
         sOut += ":" & iaVal(iIndex).ToString()
      Next

      'DMAcadExt.AcadDocument.WriteMessageLog("Array:" & sOut & vbCrLf)
      DMAcadExt.AcadDocument.WriteMessageLog(sTitle & ":" & sOut & vbCrLf)
   End Sub

	Private Class ComparerFractions
		Implements IComparer(Of DoubleInd)

		Public Function Compare(dValue1 As DoubleInd, dValue2 As DoubleInd) As Integer Implements System.Collections.Generic.IComparer(Of DoubleInd).Compare
			' Dim dFraction1 As Double = dValue1.DoubleValue - Math.Truncate(dValue1.DoubleValue)
			'Dim dFraction2 As Double = dValue2.DoubleValue - Math.Truncate(dValue2.DoubleValue)
			If dValue1.Index = dValue2.Index Then
				Return 0
			ElseIf dValue1.Fraction <= dValue2.Fraction Then
				Return -1
			Else
				Return 1
			End If
		End Function
	End Class
	Private Class ComparerDevFractions
		Implements IComparer(Of DoubleInd)
		Private miDevPositive As Boolean
		Public Sub New(iDevPositive As Boolean)
			miDevPositive = iDevPositive
		End Sub
		Public Function Compare(dValue1 As DoubleInd, dValue2 As DoubleInd) As Integer Implements System.Collections.Generic.IComparer(Of DoubleInd).Compare
			' Dim dFraction1 As Double = dValue1.DoubleValue - Math.Truncate(dValue1.DoubleValue)
			'Dim dFraction2 As Double = dValue2.DoubleValue - Math.Truncate(dValue2.DoubleValue)
			If dValue1.Index = dValue2.Index Then
				Return 0
			ElseIf dValue1.Overdeviation(miDevPositive) < dValue2.Overdeviation(miDevPositive) Then
				Return -1
			ElseIf dValue1.Overdeviation(miDevPositive) > dValue2.Overdeviation(miDevPositive) Then
				Return -1
			ElseIf dValue1.Fraction <= dValue2.Fraction Then
				Return -1
			Else
				Return 1
			End If
		End Function
	End Class
	Private Class DoubleInd

		Public Index As Integer
		Public SourceValue As Double

		Public DoubleValue As Double
		Public RangeMin As Long
		Public RangeMax As Long

		Public LongValue As Long
		Public Fraction As Double
		Public IsZero As Boolean
		Public Result As Long

		Public Sub New(iIndex As Integer, dDoubleValue As Double, sDebugCaption As String)
			Index = iIndex
			DoubleValue = dDoubleValue
			If DoubleValue = 0.0 Then
				IsZero = True
			Else
				IsZero = False
				Dim dInt As Double = Math.Truncate(DoubleValue)
				Fraction = DoubleValue - dInt
				Try
					LongValue = CLng(dInt)

				Catch oEx As Exception
					DMCommon.Debug.MsgBox("Err #271", oEx.Message, sDebugCaption, dInt, dDoubleValue)
				End Try
			End If

		End Sub
		Public Sub New(iIndex As Integer, dSourceValue As Double, dFactor As Double)
			Index = iIndex
			SourceValue = dSourceValue
			Calculate(dFactor)

		End Sub

		Public Function Overdeviation(bDevPositive As Boolean) As Long
			Dim lRes As Long
			If bDevPositive Then
				lRes = LongValue + 1L - RangeMax

			Else
				lRes = RangeMin - LongValue

			End If
			If lRes > 0L Then
				Return lRes
			Else
				Return 0L
			End If
		End Function
		Public Function SetValueByRange() As Boolean

			If LongValue + 1L >= RangeMax Then
				Result = RangeMax
				Return True
			ElseIf LongValue <= RangeMin Then
				Result = RangeMin
				Return True
			Else
				Return False
			End If
		End Function
		Public Function SetValueByRange(bDevPositive As Boolean) As Boolean
			'Dim lRes As Long
			If bDevPositive Then
				If LongValue + 1L >= RangeMax Then
					Result = RangeMax
					Return True
				Else
					Return False
				End If
			Else
				If LongValue <= RangeMin Then
					Result = RangeMin
					Return True
				Else
					Return False
				End If
			End If


		End Function
		Public Sub Calculate(dFactor As Double)
			DoubleValue = SourceValue * dFactor
			Dim dInt As Double = Math.Truncate(DoubleValue)
			Fraction = DoubleValue - dInt
			Try
				LongValue = CLng(dInt)

			Catch oEx As Exception
				DMCommon.Debug.MsgBox("Err #272", oEx.Message, dInt, SourceValue, dFactor)
			End Try
		End Sub
	End Class
End Class
