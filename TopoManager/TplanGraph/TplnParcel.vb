Option Explicit On
Option Strict On

Imports System.Data
Imports Microsoft.Office.Interop
Imports Microsoft.Office.Interop.Excel


Namespace TPlanGraph
	Public Enum enAreaStatus
		Exact	'colorind=61
		PlusPerm	'colorind=3
		MinusPerm 'colorind=60
		PlusEx 'colorind=4
		MinusEx 'colorind=51
		UB = MinusEx
    End Enum
   Public Structure UD_ParcelKey
      Implements IEqualityComparer, IEquatable(Of System.Object)

      Public Shared BaseBlockNo As Integer
      Public Shared BaseBlockAdd As Integer

      Public BlockNo As Integer
      Public BlockAdd As Integer
      Public ParcelNo As Integer
      Public Original As Boolean
     
      Private mbInstanceExists As Boolean
      Private mbFinalNameInBrackets As Boolean
 

      Public Shared ReadOnly Property BaseBlockName As String
         Get
            Dim sRes As String = Convert.ToString(BaseBlockNo)
            If BaseBlockAdd <> 0 Then
               sRes &= "/" & Convert.ToString(BaseBlockAdd)
            End If
            Return sRes
         End Get
      End Property
      Public ReadOnly Property BlockName As String
         Get
            Dim sRes As String = Convert.ToString(BlockNo)
            If BlockAdd <> 0 Then
               sRes &= "/" & Convert.ToString(BlockAdd)
            End If
            Return sRes
         End Get
      End Property
      Public ReadOnly Property BlockKey As Integer
         Get
            Return BlockData.GetBlockKey(BlockNo, BlockAdd)
         End Get
      End Property
      Public Shared Function UD_ParcelName(iParcelNo As Integer, bOrigin As Boolean) As String
         Dim sParcel As String = Convert.ToString(iParcelNo)
         If Not bOrigin Then
            sParcel = "[" & sParcel & "]"
         End If
         Return sParcel
      End Function
      Public Shared Function ParseParcelName(ByVal sValue As String, ByRef iParcelNo As Integer, ByRef bOrigin As Boolean) As Boolean
         Dim sRes As String = String.Empty
         If sValue.StartsWith("(") AndAlso sValue.EndsWith(")") Then
            sValue = sValue.Substring(1, sValue.Length - 2)
         End If

         If sValue.StartsWith("[") AndAlso sValue.EndsWith("]") Then
            sValue = sValue.Substring(1, sValue.Length - 2)
            bOrigin = False
         Else
            bOrigin = True
         End If
         Return Integer.TryParse(sValue, iParcelNo)
      End Function
      Public Function UD_ParcelName() As String
         Return UD_ParcelName(ParcelNo, Original)
      End Function
		Public Shared Function FromGridNoDb(oParcelNoOrigin As System.Object, oParcelNoTemp As System.Object) As UD_ParcelKey
			Dim bOrigin As Boolean
			Dim sParcelNo As String = Nothing
			Dim sParcelNoOrigin As String = DMCommon.Functions.CStrN(oParcelNoOrigin)
			Dim sParcelNoTemp As String = DMCommon.Functions.CStrN(oParcelNoTemp)

			If sParcelNoOrigin.Length <> 0 AndAlso sParcelNoTemp.Length = 0 Then
				bOrigin = True
				sParcelNo = sParcelNoOrigin
			ElseIf sParcelNoOrigin.Length = 0 AndAlso sParcelNoTemp.Length <> 0 Then
				bOrigin = False
				sParcelNo = sParcelNoTemp
			End If
			Dim iParcelNo As Integer
			If (sParcelNo IsNot Nothing) AndAlso Integer.TryParse(sParcelNo, iParcelNo) Then
				'  DMCommon.Debug.MsgBox("08_320", True, iParcelNo, bOrigin)
				Return New UD_ParcelKey(iParcelNo, bOrigin)
			Else
				Return New UD_ParcelKey()
			End If


		End Function
		Public Shared Function FromGrid(iBlockNo As Integer, iBlockAddNo As Integer, oParcelNoOrigin As System.Object, oParcelNoTemp As System.Object) As UD_ParcelKey
			Dim bOrigin As Boolean
			Dim sParcelNo As String = Nothing
			Dim iParcelNo As Integer = 0

			Dim iParcelNoOrigin As Integer = DMCommon.Functions.CIntN(oParcelNoOrigin)
			Dim iParcelNoTemp As Integer = DMCommon.Functions.CIntN(oParcelNoTemp)
			'  DMCommon.Debug.MsgBox("08_321", iParcelNoOrigin, iParcelNoTemp)
			If iParcelNoOrigin <> 0 AndAlso iParcelNoTemp = 0 Then
				bOrigin = True
				iParcelNo = iParcelNoOrigin
			ElseIf iParcelNoOrigin = 0 AndAlso iParcelNoTemp <> 0 Then
				bOrigin = False
				iParcelNo = iParcelNoTemp
			End If

			If iParcelNo <> 0 Then
				'  DMCommon.Debug.MsgBox("08_320", True, iParcelNo, bOrigin)
				Return New UD_ParcelKey(iBlockNo, iBlockAddNo, iParcelNo, bOrigin)
			Else
				Return New UD_ParcelKey()
			End If


		End Function
		Public ReadOnly Property ParcelNoStr As String
         Get
            Return Convert.ToString(ParcelNo)
         End Get
      End Property
      Public Sub ToJournalDataRow(ByRef oDataRow As DataRow, bFrom As Boolean)

         Dim sFieldName As String
         If bFrom Then
            If Original Then
               sFieldName = "OriginalParcelNo"
            Else
               sFieldName = "NewParcelNo"
            End If
         Else
            sFieldName = "DestParcelNo"
         End If

         oDataRow.Item(sFieldName) = ParcelNoStr


         '   MessageBox.Show(oGridRow.Cells.Item(iColumnIndex).Value.ToString(), "09_679c")
      End Sub
      Public Sub ToGrid(ByRef oGridRow As DataGridViewRow, bFrom As Boolean)
			Dim iColumnIndex As Integer
			Dim iEmptyColumnIndex As Integer

			If bFrom Then
            If Original Then
					iColumnIndex = 2
					iEmptyColumnIndex = 3
				Else
					iColumnIndex = 3
					iEmptyColumnIndex = 2
				End If
         Else
            iColumnIndex = 4
         End If

			oGridRow.Cells.Item(iColumnIndex).Value = ParcelNoStr
			If iEmptyColumnIndex <> 0 Then
				oGridRow.Cells.Item(iEmptyColumnIndex).Value = DBNull.Value
			End If

			' oGridRow.Cells.Item(iColumnIndex).IN()

			'  '   MessageBox.Show(oGridRow.Cells.Item(iColumnIndex).Value.ToString(), "09_679x")
		End Sub
		Public Shared Function FromGrid(iBlockNo As Integer, iBlocAddkNo As Integer, oGridRow As DataGridViewRow) As UD_ParcelKey
			Return FromGrid(iBlockNo, iBlocAddkNo, oGridRow.Cells.Item(2).Value, oGridRow.Cells.Item(3).Value)
		End Function
		Public Shared Function FromGrid(sParcelNoOrigin As String, sParcelNoTemp As String) As UD_ParcelKey
         Dim bOrigin As Boolean
         Dim sParcelNo As String = Nothing
         If Not String.IsNullOrEmpty(sParcelNoOrigin) AndAlso String.IsNullOrEmpty(sParcelNoTemp) Then
            bOrigin = True
            sParcelNo = sParcelNoOrigin
         ElseIf String.IsNullOrEmpty(sParcelNoOrigin) AndAlso Not String.IsNullOrEmpty(sParcelNoTemp) Then
            bOrigin = False
            sParcelNo = sParcelNoTemp
         End If
         Dim iParcelNo As Integer
         If (sParcelNo IsNot Nothing) AndAlso Integer.TryParse(sParcelNo, iParcelNo) Then
            Return New UD_ParcelKey(iParcelNo, bOrigin)
         Else
            Return New UD_ParcelKey()
         End If


      End Function

      Public Sub New(iParcelNo As Integer, bOrigin As Boolean)
         zzNew(iParcelNo, bOrigin)

      End Sub
		Public Sub New(iBlockNo As Integer, iBlockAddNo As Integer, sParcelNo As String)
			Me.New(sParcelNo)
			BlockNo = iBlockNo
			BlockAdd = iBlockAddNo

		End Sub
		Public Sub New(sParcelNo As String)
			If Not String.IsNullOrEmpty(sParcelNo) Then
				If sParcelNo.StartsWith("[") AndAlso sParcelNo.EndsWith("]") Then
					sParcelNo = sParcelNo.Substring(1, sParcelNo.Length - 2)
					Original = False
				Else
					Original = True
				End If
				If Integer.TryParse(sParcelNo, ParcelNo) Then
					BlockNo = BaseBlockNo
					BlockAdd = BaseBlockAdd
					If ParcelNo <> 0 Then
						mbInstanceExists = True
					End If
				End If
			End If
		End Sub

		Public Sub New(iBlockNo As Integer, iParcelNo As Integer, bOrigin As Boolean)
			BlockNo = iBlockNo
			BlockAdd = BaseBlockAdd
			ParcelNo = iParcelNo
			Original = bOrigin
			If iParcelNo <> 0 Then
				mbInstanceExists = True
			End If

		End Sub
		Public Sub New(iBlockNo As Integer, iBlockAdd As Integer, iParcelNo As Integer, bIsOriginal As Boolean)
         BlockNo = iBlockNo
         BlockAdd = iBlockAdd
         ParcelNo = iParcelNo
         Original = bIsOriginal
         If iParcelNo <> 0 Then
            mbInstanceExists = True
         End If

      End Sub
      Private Sub zzNew(iParcelNo As Integer, bOrigin As Boolean)
         BlockNo = BaseBlockNo
         BlockAdd = BaseBlockAdd
         ParcelNo = iParcelNo
         Original = bOrigin
         If iParcelNo <> 0 Then
            mbInstanceExists = True
         End If


      End Sub

      Public ReadOnly Property TempName As String
         Get
            If Original Then
               Return String.Empty
            Else
               Return ParcelNo.ToString()
            End If
         End Get
      End Property
      Public ReadOnly Property FinalName As String
         Get
            Dim sRes As String
            If Original Then
               sRes = ParcelNo.ToString()
               If mbFinalNameInBrackets Then
                  sRes = "(" & sRes & ")"
               End If
            Else
               sRes = String.Empty
            End If
            Return sRes
         End Get
      End Property
      Public Overrides Function ToString() As String
         Dim sRes As String = Convert.ToString(BlockNo)
         If BlockNo <> 0 Then
            sRes = Convert.ToString(BlockNo)
         Else
            sRes = String.Empty
         End If
			'Dim sParcel As String = Convert.ToString(ParcelNo)
			If BlockAdd <> 0 Then
            sRes &= "/" & CStr(BlockAdd)
         End If
         If sRes.Length <> 0 Then
            sRes &= ", "
         End If
         sRes &= UD_ParcelName(ParcelNo, Original)
         Return sRes
      End Function
      Public Sub NextParcel()
         ParcelNo += 1
      End Sub

      Public ReadOnly Property CompareKey As Integer
         Get
            Dim iRes As Integer = ParcelNo
            If Not Original Then
               iRes += 10000
            End If
            Return iRes
         End Get
      End Property
      Public ReadOnly Property IsBase As Boolean
         Get
            Return (BlockNo = BaseBlockNo) AndAlso (BlockAdd = BaseBlockAdd)
         End Get
      End Property
      Public ReadOnly Property IsEssential As Boolean
         Get
            Return (ParcelNo <> 0)
         End Get
      End Property
      Public Property FinalNameInBrackets As Boolean
         Get
            Return mbFinalNameInBrackets

         End Get
         Set(bValue As Boolean)
            mbFinalNameInBrackets = bValue
         End Set
      End Property
      Public Shared Operator =(tParcelA As UD_ParcelKey, tParcelB As UD_ParcelKey) As Boolean
         Return (tParcelA.BlockNo = tParcelB.BlockNo) AndAlso (tParcelA.BlockAdd = tParcelB.BlockAdd) AndAlso (tParcelA.ParcelNo = tParcelB.ParcelNo) AndAlso (tParcelA.Original = tParcelB.Original)
      End Operator
      Public Shared Operator <>(tParcelA As UD_ParcelKey, tParcelB As UD_ParcelKey) As Boolean
         Return (tParcelA.BlockNo <> tParcelB.BlockNo) OrElse (tParcelA.BlockAdd <> tParcelB.BlockAdd) OrElse (tParcelA.ParcelNo <> tParcelB.ParcelNo) OrElse (tParcelA.Original <> tParcelB.Original)
      End Operator

		Public Function EqualsA(oObjectA As System.Object, oObjectB As System.Object) As Boolean Implements IEqualityComparer.Equals
			Dim tParcelA As UD_ParcelKey = DirectCast(oObjectA, UD_ParcelKey)
			Dim tParcelB As UD_ParcelKey = DirectCast(oObjectB, UD_ParcelKey)
			Return (tParcelA = tParcelB)
			'IEqualityComparer.Equals,
		End Function

		Public Function GetHashCodeA(obj As System.Object) As Integer Implements IEqualityComparer.GetHashCode
         Return obj.GetHashCode()
      End Function
      Public ReadOnly Property Exists As Boolean
         Get
            Return mbInstanceExists
         End Get
      End Property


      Public Function Equals3(oOther As System.Object) As Boolean Implements IEquatable(Of System.Object).Equals
         Dim tOtherParcelKey As UD_ParcelKey = DirectCast(oOther, UD_ParcelKey)
         Return (Me = tOtherParcelKey)
      End Function
      Public Sub DebugMsg(Optional sCaption As String = Nothing)
         If sCaption Is Nothing Then
            sCaption = "ParcelKey"
         End If
         DMCommon.Debug.MsgBox(sCaption, BlockNo, ParcelNo, Original)
      End Sub
  
   End Structure
  
   Public Structure ParcelData
      Shared MapThemeID As DMAcadExt.enMapTheme
      Shared BlockName As String
      Dim Name As String
      Dim Number As Integer
      Dim Order As Integer

      Dim Block As Integer
      Dim BlockAdd As Integer
      Dim LegalArea As Double
      Dim DBLegalArea As Double
      Dim DBShapeArea As Double
      Dim IsAnalytic As Boolean
      Dim Owner As Integer
      Dim RoundedArea As Double
      Dim LanduseName As String
      Dim LanduseOrder As Integer
      Dim HasLegalArea As Boolean
      Dim Correct As Boolean
      Dim ErrMessage As String
      Dim Exists As Boolean

      Public Sub New(saValues() As String, Optional saAddValues() As String = Nothing)
         Dim iRow As Integer = 0

         If saValues IsNot Nothing Then
            Dim iAttribUB As Integer = -1
            Try
               iAttribUB = saValues.GetUpperBound(0)
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_1")
            End Try
            If iAttribUB >= 0 Then
               Try
                  Name = saValues(0).Trim()
                  Order = TplnBasicPgon.GetNameOrder(Name, True)
                  If Not Integer.TryParse(Name, Number) Then
                     zzAddErrMesage(DMCommon.dmMessages.Message(308, Name))
                  End If
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnParcel - New_2")
               End Try
            Else
               zzAddErrMesage("msg#1002")
            End If


            If iAttribUB >= 1 Then
               Try
                  zzParseBlockString(saValues(1))
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnParcel - New_3")
               End Try
            Else
               zzAddErrMesage("msg#1003")
            End If
            If iAttribUB >= 2 Then
               Try
                  zzParseBlockAddString(saValues(2))
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnParcel - New_4")
               End Try
            Else
               zzAddErrMesage("msg#1004")
            End If



            If iAttribUB >= 3 Then
               Try
                  zzParseLegalAreaString(saValues(3))
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnParcel - New_5")
               End Try

            Else
               zzAddErrMesage("msg#1004")
               TplnProject.WriteMessageBox("dsaBlockAttribText.GetUpperBound(0)<3: " & CStr(iAttribUB), "New Parcel-11")
            End If
            If iAttribUB >= 4 Then
               Try
                  zzParseIsAnalyticString(saValues(4))
                  Correct = True
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnParcel - New_6")
               End Try
            Else
               zzAddErrMesage("msg#1005")
               TplnProject.WriteMessageBox("dsaBlockAttribText.GetUpperBound(0)<4: " & CStr(iAttribUB), "New Parcel-9")
            End If
            Exists = True
         Else
            zzAddErrMesage(DMCommon.dmMessages.Message(301, BlockName))
				'DMCommon.Debug.UserMsg("Err #3010", BlockName, DMCommon.dmMessages.Message(301, BlockName))
				DMCommon.Debug.MsgBoxLoop("B001001", "NewParcData")
			End If
         If saAddValues IsNot Nothing Then
            ' DMCommon.ExcelLog.SetNextValue(iRow, 7, saAddValues(0))
            Dim iAttribUB As Integer = -1
            Try
               iAttribUB = saAddValues.GetUpperBound(0)
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_1")
            End Try
            If iAttribUB >= 0 Then
               zzParseOwnerString(saAddValues(0))
            End If
         End If
         '  zzLoadDbLegalArea()
         zzLoadDbParcelArea()

      End Sub
      Public ReadOnly Property BlockKey As Integer
         Get
            Return 1000 * Block + BlockAdd
         End Get
      End Property
      Private Sub zzLoadDbParcelArea()
         Const sSPName As String = "GetDBParcelArea"
         If Number <> 0 Then
            Dim oaParams(2) As System.Data.Common.DbParameter

            oaParams(0) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prGUSH_NUM", DbType.Int32, Block)
            oaParams(1) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prGUSH_SUFFI", DbType.Int32, BlockAdd)
            oaParams(2) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prPARCEL", DbType.Int32, Number)

            Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, System.Data.CommandType.StoredProcedure, oaParams)
            If oDataReader IsNot Nothing AndAlso oDataReader.HasRows Then
               If oDataReader.Read Then
                  DBLegalArea = oDataReader.GetDouble(0)
                  DBShapeArea = oDataReader.GetDouble(1)
               End If
            End If
            If oDataReader IsNot Nothing Then
               oDataReader.Close()
            End If
         End If


      End Sub
      Private Sub zzLoadDbLegalArea()
         Const sSPName As String = "GetDBLegalArea"
         If Number <> 0 Then
            Dim oaParams(2) As System.Data.Common.DbParameter

            oaParams(0) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prGUSH_NUM", DbType.Int32, Block)
            oaParams(1) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prGUSH_SUFFI", DbType.Int32, BlockAdd)
            oaParams(2) = TPlServerDB.ServerDB.CurrentServerDB.GetParameter("@prPARCEL", DbType.Int32, Number)

            Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sSPName, System.Data.CommandType.StoredProcedure, oaParams)
            If oRes IsNot Nothing Then
               Try
                  DBLegalArea = DirectCast(oRes, Double)
               Catch oEx As Exception
                  DMCommon.Debug.MsgBox("10_290", Block, BlockAdd, Number, oRes)
               End Try

            End If

         End If
      End Sub
      Private Sub zzAddErrMesage(sMsg As String)
         If String.IsNullOrEmpty(ErrMessage) Then
            ErrMessage = sMsg
         Else
            ErrMessage &= vbCrLf & sMsg
         End If
      End Sub

      Private Shared Function zzNN(sVal As String) As String
         If sVal Is Nothing Then
            Return "<Nothing>"
         Else
            Return sVal
         End If
      End Function


      Private Sub zzParseBlockString(ByVal sValue As String)


         sValue = sValue.Trim()

         If Not Integer.TryParse(sValue, Block) Then
				System.Windows.Forms.MessageBox.Show("מס' גוש הוא לא מספר" & vbCrLf & "'" & sValue & "'", "ParseBlockString")
				Block = 0
         End If

      End Sub
      Private Sub zzParseBlockAddString(ByVal sValue As String)
         sValue = sValue.Trim()
         If IsNumeric(sValue) AndAlso Not sValue.Contains(".") AndAlso Not sValue.Contains("-") Then
            Try
               BlockAdd = Convert.ToInt32(sValue)
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "zzParseBlockAddString")
            End Try
         End If

      End Sub
      Private Sub zzParseOwnerString(ByVal sValue As String)
         sValue = sValue.Trim()

         Try
            If Not Integer.TryParse(sValue, Owner) Then
               'MsgErr
            End If

         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "zzParseOwnerString")
         End Try

      End Sub

      Private Sub zzAfterSetLegalArea(bDunam As Boolean) '!!!!!!!!!
         Dim bLegalAreaExists As Boolean = False
         If bDunam Then
            LegalArea = Math.Round(1000.0 * LegalArea, 6)
         End If
         If LegalArea > 0.0 Then
            HasLegalArea = True
            bLegalAreaExists = True
         End If

      End Sub
      Private Sub zzParseLegalAreaString(ByVal sValue As String)
         '	Dim bLegalAreaExists As Boolean = False
         sValue = sValue.Trim()
         If IsNumeric(sValue) AndAlso Not sValue.Contains("-") Then
            Try
               LegalArea = Convert.ToDouble(sValue)
               zzAfterSetLegalArea(True)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - zzParseLegalAreaString")
            End Try
         Else

         End If
      End Sub
      Private Sub zzParseIsAnalyticString(ByVal sValue As String)
         '	Dim bLegalAreaExists As Boolean = False
         sValue = sValue.Trim()
         If sValue = "1" Then
            IsAnalytic = True
            'MessageBox.Show(CStr(Name) & ":" & CStr(LegalArea), "05_388")
         Else
            IsAnalytic = False
         End If


      End Sub
   End Structure
	Public Structure TplnOwnerArea
		Dim OwnerID As Integer
		Dim AreaSet As TplnAreaSet
		Public Sub New(iOwnerID As Integer, dArea As Double)
			OwnerID = iOwnerID
			AreaSet.CalcArea = dArea
		End Sub
	End Structure
	Public Structure TplnOwnNoteArea
		Private mdAcadArea As Double
		Dim Paragraph19 As Boolean
		Dim Leasing As Boolean

		Public Sub New(bParagraph19 As Boolean, bLeasing As Boolean, dAcadArea As Double)
			Paragraph19 = bParagraph19
			Leasing = bLeasing
			mdAcadArea = dAcadArea
		End Sub
		Public Function GetParagraph19Area() As Double
			If Paragraph19 Then
				Return mdAcadArea
			Else
				Return 0.0
			End If
		End Function
		Public Function GetLeasingArea() As Double
			If Leasing Then
				Return mdAcadArea
			Else
				Return 0.0
			End If
		End Function
		Public Function GetOverlayArea() As Double
			If Paragraph19 AndAlso Leasing Then
				Return mdAcadArea
			Else
				Return 0.0
			End If
		End Function
	End Structure
	Public Class TplnParcel
		Inherits TPlanGraph.TplnBasicPgon
		Shared bTesrErr As Boolean = False
#Region "Declarations"

		Public Const TopoName As String = "Parcels"
		Public Const ID_Debug As Integer = 2026
		Private Shared mtParcelMapThemeData As DMAcadExt.MapThemeData
		Private Shared msCentroidBlockName As String
		Private Shared msAddCentroidBlockName As String

		Private Const msNameAttribTag As String = "PARCEL_NUM"
		Private Const msBlockAttribTag As String = "LOT_NUM"
		Private Const msBlockAddAttribTag As String = "GUSH_SUFFI"

		Private Const msOwnerAttribTag As String = "Owner"

		'''''''''''''''''TEMP	Private Const msLegalAreaAttribTag As String = "AREA_LEGAL"
		Private Const msLegalAreaAttribTag As String = "LEGAL_AREA"
		Private Const msAnalyticAttribTag As String = "ANALYTIC"

		Public Const msToleranceFieldName As String = "Tolerance"
		Public Const msToleranceNeighborFieldName As String = "ToleranceNeighbor"

		Public Const msDeltaAreaFieldName As String = "DeltaArea"
		Public Const msDeltaAreaNeighborFieldName As String = "DeltaAreaNeighbor"

		Public Const msDeviationFieldName As String = "Deviation"
		Public Const msDeviationNeighborFieldName As String = "DeviationNeighbor"


		Private Const msEntireExtName As String = "(ש) "
		Private Const msPartialExtName As String = "(ח) "
		Private Const msEntireName As String = "שלמות"
		Private Const msPartialName As String = "חלק"

		Public Const NameFieldName As String = "ParcelNo"
		Friend Const ParcelOrderFieldName As String = "POrder"

		Public Const LanduseIDFieldName As String = "LanduseID"
		Public Const LanduseNameFieldName As String = "LanduseName"
		Public Const LanduseOrderFieldName As String = "LOrder"
		Public Const LanduseAreaFieldName As String = "LanduseArea"


		Private Const ExproTypeIDFieldName As String = "ExproTypeID"
		Private Const ExproTypeNameFieldName As String = "ExproTypeName"

		Public Const msLotAreaFieldName As String = "LotArea"
		Public Const msLotDifAreaFieldName As String = "LotDifArea"
		Public Const msParcelDifAreaFieldName As String = "ParcelDifArea"




		Public Const BlockFullFieldName As String = "BlockFull"
		Public Const BlockFieldName As String = "BlockNo"
		Public Const BlockAddFieldName As String = "BlockAddNo"


		Public Const BlockStatusFieldName As String = "BlockStatus"  'Yes/No  6/20
		Friend Const BlockStatusNameFieldName As String = "BlockStatusName"
		Public Const BlockIsAnalyticFieldName As String = "IsAnalytic"

		Public Const ParcelNameFieldName As String = "ParcelName"
		Public Const LegalAreaFieldName As String = "LegalArea"
		Public Const CondLegalAreaFieldName As String = "CondLegalArea"

		Public Const msDBLegalAreaFieldName As String = "DBLegalArea"

		Public Const IsAnalyticFieldName As String = "IsAnalytic"

		Public Const PgonAreaFieldName As String = "PgonArea"


		Friend Const msInLotAreaFieldName As String = "InLotArea"
		Friend Const msInLotCalcAreaFieldName As String = "InLotCalcArea"

		Public Const msInPlanAreaApprMergeFieldName As String = "InPlanAreaApprMerge" ' real acad area
		Public Const msInPlanAreaPropMergeFieldName As String = "InPlanAreaPropMerge"

		Public Const msInPlanAreaApprUnionFieldName As String = "InPlanAreaApprUnion"
		Public Const msInPlanAreaPropUnionFieldName As String = "InPlanAreaPropUnion"

		Public Const msInPlanAreaApprFDO_OverlayFieldName As String = "InPlanAreaApprFDO_Overlay"
		Public Const msInPlanAreaPropFDO_OverlayFieldName As String = "InPlanAreaPropFDO_Overlay"

		'	Friend Const msInPlanAreaUnionFieldName As String = "InPlanArea"

		Friend Const msInPlanCalcAreaFieldNameAAA As String = "InPlanCalcArea"

		Public Const msInPlanCalcAreaApprMergeFieldName As String = "InPlanCalcAreaAppr"
		Public Const msInPlanCalcAreaPropMergeFieldName As String = "InPlanCalcAreaProp"

		Public Const InPlanCalcAreaApprFDO_OverlayFieldName As String = "InPlanCalcAreaApprFO"
		Public Const msInPlanCalcAreaPropFDO_OverlayFieldName As String = "InPlanCalcAreaPropFO"


		Public Const msInPlanCalcArea2ApprMergeFieldName As String = "InPlanCalcArea2Appr"
		Public Const msInPlanCalcArea2PropMergeFieldName As String = "InPlanCalcArea2Prop"

		Public Const msInPlanCalcArea2ApprFDO_OverLayFieldName As String = "InPlanCalcArea2ApprFO"
		Public Const msInPlanCalcArea2PropFDO_OverLayFieldName As String = "InPlanCalcArea2PropFO"


		Public Const msInPlanRoundedAreaApprMergeFieldName As String = "InPlanRoundedAreaAppr"
		Public Const msInPlanRoundedAreaPropMergeFieldName As String = "InPlanRoundedAreaProp"

		Public Const msInPlanCalcAreaApprUnFieldName As String = "InPlanCalcAreaApprUn"
		Public Const msInPlanCalcAreaPropUnFieldName As String = "InPlanCalcAreaPropUn"

		Public Const msInPlanRoundedAreaApprFDO_OverlayFieldName As String = "InPlanRoundedAreaApprFO"
		Public Const msInPlanRoundedAreaPropFDO_OverlayFieldName As String = "InPlanRoundedAreaPropFO"


		Public Const msInPlanCalcAreaMergeFieldName As String = "InPlanCalcMArea"
		Public Const msInPlanCalcArea2MergeFieldName As String = "InPlanCalc2MArea"
		Public Const msInPlanRoundedAreaMergeFieldName As String = "InPlanRoundedMArea"

		Public Const msLUseInPlanCalcAreaMergeFieldName As String = "LUInPlanCalcMArea"
		Public Const msLUseInPlanCalcArea2MergeFieldName As String = "LUInPlanCalc2MArea"
		Public Const msLUseInPlanRoundedAreaMergeFieldName As String = "LUInPlanRoundedMArea"


		Public Const msInPlanCalcAreaFDO_OverlayFieldName As String = "InPlanCalcFArea"
		Public Const msInPlanCalcArea2FDO_OverlayFieldName As String = "InPlanCalc2FArea"
		Public Const msInPlanRoundedAreaFDO_OverlayFieldName As String = "InPlanRoundedFArea"

		Public Const msLUseInPlanCalcAreaFDO_OverlayFieldName As String = "LUInPlanCalcFArea"
		Public Const msLUseInPlanCalcArea2FDO_OverlayFieldName As String = "LUInPlanCalc2FArea"
		Public Const msLUseInPlanRoundedAreaFDO_OverlayFieldName As String = "LUInPlanRoundedFArea"

		Public Const msInPlanCalcAreaUnionFieldName As String = "InPlanCalcUArea"

		Public Const msInPlanAreaMergeFieldName As String = "AreaM"
		Public Const msInPlanAreaUnionFieldName As String = "AreaU"
		Public Const msInPlanAreaFDO_OverlayFieldName As String = "AreaF"

		Public Const msLUseInPlanAreaMergeFieldName As String = "LUAreaM"
		Public Const msLUseInPlanAreaUnionFieldName As String = "LUAreaU"
		Public Const msLUseInPlanAreaFDO_OverlayFieldName As String = "LUAreaF"

		Public Const msExproTypeAreaFieldNamePrefix As String = "ExproTypeArea_"
		Public Const msExproTypeAcadAreaFieldNamePrefix As String = "ExproTypeAcadArea_"
		Public Const msExproTypeToleranceFieldNamePrefix As String = "ExproTypeTolerance_"
		Public Const msExproTypeDiffAreaFieldNamePrefix As String = "ExproTypeDiffArea_"
		Public Const msExproTypeDeviationFieldNamePrefix As String = "ExproTypeDeviation_"


		Public Const msExproPgonTypeIDFieldName As String = "ExproPgonTypeID"
		Public Const msExproPgonTypeNameFieldName As String = "ExproPgonTypeName"
		Public Const msExproPgonTypeOrderByFieldName As String = "ExproPgonTypeOrderBy"


		Public Const msExproCalcTypeFieldName As String = "ExproCalcType"



		Public Const msOutPgonIsForcedFieldNamePrefix As String = "OutPgonIsForced_"
		Public Const msOutPgonLegalAreaFieldNamePrefix As String = "OutPgonLegalArea_"
		Public Const msOutPgonAcadAreaFieldNamePrefix As String = "OutPgonAcadArea_"
		Public Const msOutPgonToleranceFieldNamePrefix As String = "OutPgonTolerance_"
		Public Const msOutPgonDiffAreaFieldNamePrefix As String = "OutPgonDiffArea_"
		Public Const msOutPgonDeviationFieldNamePrefix As String = "OutPgonDeviation_"




		Public Const msSumApprPgonAreaMergeFldName As String = "SumAreaA"
		Public Const msSumPropPgonAreaMergeFldName As String = "SumAreaP"
		Public Const msSumApprUnPgonAreaFldName As String = "SumAreaAUn"
		Public Const msSumPropUnPgonAreaFldName As String = "SumAreaPUn"

		Public Const msSumApprPgonAreaFDO_OverlayFldName As String = "SumAreaAFO"
		Public Const msSumPropPgonAreaFDO_OverlayFldName As String = "SumAreaPFO"





		Public Const msCalcAreaPropFieldName As String = "CalcAreaProp"
		Public Const msCalcAreaPropPctFieldName As String = "CalcAreaPropPct"

		Public Const msCalcAreaApprFieldName As String = "CalcAreaAppr"
		Public Const msCalcAreaApprPctFieldName As String = "CalcAreaApprPct"

		Public Const msCalcAreaFieldName As String = "CalcArea"
		Public Const msParcelCalcAreaFieldName As String = "ParcelCalcArea"

		Private Const msPlanStateFieldName As String = "PlanStateApprFDO_Overlay"
		'Private Const msPlanStateFieldName As String = "LocID"
		Private Const msPlanStateTextFieldName As String = "Location"

		Private Const msPlanStateApprMergeFieldName As String = "LocIDApprMerge"
		Private Const msPlanStateTextApprMergeFieldName As String = "LocationApprMerge"

		Private Const msPlanStatePropMergeFieldName As String = "LocIDPropMerge"
		Private Const msPlanStateTextPropMergeFieldName As String = "LocationPropMerge"

		Private Const msPlanStateApprFDO_OverlayFieldName As String = "PlanStateApprFDO_Overlay"
		Private Const msPlanStateTextApprFDO_OverlayFieldName As String = "PlanStateTextApprFDO_Overlay"

		Private Const msPlanStatePropFDO_OverlayFieldName As String = "PlanStatePropFDO_Overlay"
		Private Const msPlanStateTextPropFDO_OverlayFieldName As String = "PlanStateTextPropFDO_Overlay"






		Public Const msParcelEntireFieldName As String = "ParcelEntire"
		Public Const msParcelPartialFieldName As String = "ParcelPartial"

		Public Const msLotNameFieldName As String = "LotName"
		Friend Const msLotOrderFieldName As String = "LotOrder"
		Public Const msInPlanFieldName As String = "InPlan"
		Public Const msInPlanTextFieldName As String = "InOut"

		Public Const msOwnerFieldName As String = "Owner"

		Public Const Paragraph19AreaFieldName As String = "Paragraph19Area"
		Public Const LeasingAreaFieldName As String = "LeasingArea"
		Public Const OverlayPrg5LeasAreaFieldName As String = "OverlayPrg5LeasArea"



		Private Const mdParcelAreaTolearance As Double = 0.002
		Private Shared miaBlockAttribIndex(4) As Integer
		Private Shared miaAddBlockAttribIndex(0) As Integer

		Private Shared miTest As Integer
		Private Shared moMainDataTable As System.Data.DataTable
		Private Shared moMainRegionDataTable As System.Data.DataTable

		'	Private Shared moMainColumnIndices As Dictionary(Of Integer, Integer)
		Private Shared moMainHiddenColumns As Dictionary(Of Integer, Integer)


		Private Shared miRoundDigit As Integer = 3

		Private Shared moLotsContentTable As System.Data.DataTable = Nothing

		Private Shared moLandusePropTable As System.Data.DataTable = Nothing
		Private Shared moLanduseApprTable As System.Data.DataTable = Nothing

		Private Shared moLanduseRegionPropTable As System.Data.DataTable = Nothing
		Private Shared moLanduseRegionApprTable As System.Data.DataTable = Nothing




		Private Shared moaPolygonTable(DMAcadExt.enOverlayIndex.OverlayIndexUB) As System.Data.DataTable
		Private Shared moaBlockTable(DMAcadExt.enOverlayIndex.OverlayIndexUB) As System.Data.DataTable
		Private Shared moBlockRegionTable As System.Data.DataTable

		Private Shared moaLanduseTable(DMAcadExt.enOverlayIndex.OverlayIndexUB) As System.Data.DataTable
		Private Shared moExproTable As System.Data.DataTable
		Private Shared moExproTableByCol As System.Data.DataTable
		Private Shared moExproTablePgons As System.Data.DataTable



		Private Shared moCalcArea As UnionPgonArea = New UnionPgonArea
		Private Shared mbHasLegalArea As Boolean = False
		Private Shared mlstMissingLegalAreaPoints As List(Of DMAcadExt.TPlnPoint)

		Private Shared mhsExproTypes As HashSet(Of Integer) = New HashSet(Of Integer)()
		Private Shared mdicOwnerTotalAreas As Dictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
		Private Shared maOwnerTotalArea As Double
		''    Public Shared mdInPlanSumCalcAreaProp As Double
		''    Public Shared mdInPlanSumCalcAreaAppr As Double


		'	Private miBlock As Integer = 0
		Private miBlockAdd As Integer = 0
		Private mdLegalArea As Double
		Private mdRoundedArea As Double
		Private mtParcelArea As ParcelArea
		'   Private mbHasDeviation As Boolean
		Private mdNeed As Double

		Private mbInPlan As Boolean = False
		Private mbOutPlan As Boolean = False
		Private mdicLandusesAppr As TplnLanduses
		Private mdicLandusesProp As TplnLanduses

		Private mdicLandusesRegionAppr As TplnLanduses
		Private mdicLandusesRegionProp As TplnLanduses
		Private mhsRegions As HashSet(Of Integer)
		Private mhsLots As HashSet(Of Integer)

		'Private mdicOwnershipNotes As TplnOwnershipNotes
		Private mcolOwnNotePgons As ObjectModel.Collection(Of TplnOwnNoteArea)

		Private mdicParcelExproTypes As Dictionary(Of Integer, TplnExproType)
		Private mdicTypeOverlayGroups As Dictionary(Of Integer, TypeOverlayGroup)
		Private mdicTypeOverlayGroupsPlus As Dictionary(Of Integer, TypeOverlayGroup)



		Private mbaInPlan(DMAcadExt.enOverlayIndex.OverlayIndexUB) As Boolean
		Private mbaOutPlan(DMAcadExt.enOverlayIndex.OverlayIndexUB) As Boolean
		Private miaMerhav(DMAcadExt.enOverlayIndex.OverlayIndexUB) As Integer

		Private mtExproRoundArea As ExproArea
		Private mcolOutPgons As ObjectModel.Collection(Of ParcelArea)
		Private mtaOutPgons() As ParcelArea
		Private mtaExproParcelArea() As ParcelArea
		Private mtaOwnerArea() As TplnOwnerArea


		Private mbHasOutPgonsDeviation As Boolean
		Private Shared miMaxOverGrCount As Integer
		Private Shared miMaxOutPgonsCount As Integer
		Private Shared miMaxExproType As Integer


		Private Shared mbHasOwnershipNotes As Boolean
		Private Shared mdicColorSchemes As IDictionary(Of String, DMAcadExt.ColorScheme)
		Private Shared miDebugCounterA As Integer
		Private Shared miDebugCounterB As Integer
		'Dim iTopoID_Debug As Integer =  9469 '9639
		'Dim iTopoID_Debug As Integer = 10280 ' 21	13901  
		'	Dim miTopoID_Debug As Integer = 9760   '13943  100  OK
		'	Dim miTopoID_Debug As Integer = 10280 ' 21	13901
		Dim miTopoID_Debug As Integer = 9745 ' 146	13943


#End Region
#Region "Shared members"

		Public Shared Sub Initialize()
			Try
				msCentroidBlockName = TopoDefs.Item(New DMAcadExt.TopoDefID(enTopoPurpose.Parcel)).CentroidBlocks(0)
				msAddCentroidBlockName = DMAcadExt.AcadBlock.GetAdditionalBlockName(msCentroidBlockName, 1)


				Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
				MessageBox.Show(msCentroidBlockName & ":" & msAddCentroidBlockName, "11_131")
				DMCommon.Functions.DispArray(saBlockAttribTag, "01_551d", True)
				If saBlockAttribTag IsNot Nothing Then
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msNameAttribTag
								miaBlockAttribIndex(0) = iAttribIndex
							Case msBlockAttribTag
								miaBlockAttribIndex(1) = iAttribIndex
							Case msBlockAddAttribTag
								miaBlockAttribIndex(2) = iAttribIndex
							Case msLegalAreaAttribTag
								miaBlockAttribIndex(3) = iAttribIndex
							Case msAnalyticAttribTag
								miaBlockAttribIndex(4) = iAttribIndex
						End Select
					Next
					DMCommon.Functions.DispArray("01_572f", miaBlockAttribIndex)
					'	Erase saBlockAttribTag
				End If
				mlstMissingLegalAreaPoints = New List(Of DMAcadExt.TPlnPoint)
				'07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
			End Try

		End Sub
		Public Shared Sub FillColorSchemesDic(Optional dScale As Double = 1.0)
			Const bFirstColorSchemeField As Integer = 3
			Dim tColorScheme As DMAcadExt.ColorScheme
			'''''''''''''''''	Dim dScale As Double = zzGetSelectedScale() / mdBaseScale
			Dim oaParams(2) As System.Data.Common.DbParameter
			'	Dim oErrOut As System.Data.Common.DbException = Nothing
			Dim iTest As Integer

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetColorSchemeDataReader(3, "GetParcelColorSchemeSet")

			If mdicColorSchemes Is Nothing Then
				mdicColorSchemes = New Dictionary(Of String, DMAcadExt.ColorScheme)


				If oDataReader IsNot Nothing Then
					'	MessageBox.Show("", "01_518a")
					Dim iBlock, iColorSchemeID As Integer
					Dim sParcelName As String
					Dim iSchemeOrder As Integer

					Do While oDataReader.Read
						iBlock = oDataReader.GetInt32(0)
						If oDataReader.IsDBNull(1) Then
							sParcelName = String.Empty
						Else
							sParcelName = oDataReader.GetString(1)
						End If

						If oDataReader.IsDBNull(bFirstColorSchemeField) Then
							iColorSchemeID = 0
						Else
							iColorSchemeID = oDataReader.GetInt32(bFirstColorSchemeField)
						End If
						If Not oDataReader.IsDBNull(2) Then
							iSchemeOrder = oDataReader.GetInt32(2)
						End If


						If iTest < 2 Then
							MessageBox.Show(CStr(iBlock) & "." & sParcelName & vbCrLf & CStr(iColorSchemeID), "01_598")
							iTest += 1
						End If

						If iColorSchemeID <> 0 Then
							tColorScheme = New DMAcadExt.ColorScheme(oDataReader, bFirstColorSchemeField, dScale)
							mdicColorSchemes.Add(GetNameKey(iBlock, sParcelName), tColorScheme)
						End If
					Loop
					oDataReader.Close()


				Else
					MessageBox.Show("", "01_173g")
				End If
			End If
		End Sub
		Public Shared Function GetColorScheme(ByVal sParcelNameKey As String, ByVal dScale As Double) As DMAcadExt.ColorScheme

			Dim tColorScheme As DMAcadExt.ColorScheme = Nothing
			Dim bRes As Boolean = mdicColorSchemes.TryGetValue(sParcelNameKey, tColorScheme)
			If bRes Then
				Return tColorScheme
			Else
				Return New DMAcadExt.ColorScheme()
			End If
		End Function
		Public Shared Sub Initialize(tParcelMapThemeData As DMAcadExt.MapThemeData)
			Try
				mtParcelMapThemeData = tParcelMapThemeData
				msCentroidBlockName = mtParcelMapThemeData.CentroidBlock
				msAddCentroidBlockName = DMAcadExt.AcadBlock.GetAdditionalBlockName(msCentroidBlockName, 1)


				'   msAddCentroidBlockName = DMAcadExt.AcadBlock.GetAdditionalBlockName(msCentroidBlockName, 1)
				'     MessageBox.Show(msCentroidBlockName & ":" & msAddCentroidBlockName, "11_178")
				Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
				Dim saAddBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msAddCentroidBlockName, False)

				' MessageBox.Show(saBlockAttribTag.GetUpperBound(0) & ":" & saAddBlockAttribTag.GetUpperBound(0), "11_179a")
				If saBlockAttribTag IsNot Nothing Then
					'DMCommon.Functions.DispArray(saBlockAttribTag, "01_599Parcel", True)
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msNameAttribTag
								miaBlockAttribIndex(0) = iAttribIndex
							Case msBlockAttribTag
								miaBlockAttribIndex(1) = iAttribIndex
							Case msBlockAddAttribTag
								miaBlockAttribIndex(2) = iAttribIndex
							Case msLegalAreaAttribTag
								miaBlockAttribIndex(3) = iAttribIndex
							Case msAnalyticAttribTag
								miaBlockAttribIndex(4) = iAttribIndex
						End Select
					Next
					'	DMCommon.Functions.DispArray("01_577d", miaBlockAttribIndex)

					'	DMAcadExt.AcadDocument.WriteDebugMessage("#16ee Parcel Block: " & msCentroidBlockName)
					'	Erase saBlockAttribTag
				Else
					'  MessageBox.Show("Definition of '" & msCentroidBlockName & "' was not found", "11_180")
				End If

				If saAddBlockAttribTag IsNot Nothing Then
					DMCommon.Functions.DispArray(saAddBlockAttribTag, "01_598f", True)
					For iAttribIndex As Integer = 0 To saAddBlockAttribTag.GetUpperBound(0)
						Select Case saAddBlockAttribTag(iAttribIndex)
							Case msOwnerAttribTag
								miaAddBlockAttribIndex(0) = iAttribIndex
						End Select
					Next
				End If
				ParcelData.MapThemeID = tParcelMapThemeData.MapThemeID
				ParcelData.BlockName = tParcelMapThemeData.CentroidBlock

				mlstMissingLegalAreaPoints = New List(Of DMAcadExt.TPlnPoint)
				'07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
			End Try
		End Sub

		Public Shared Property HasOwnershipNotes As Boolean
			Get
				Return mbHasOwnershipNotes
			End Get
			Set(bValue As Boolean)
				mbHasOwnershipNotes = bValue
			End Set
		End Property
		Public Shared Function GetLineTopoName() As String
			Return mtParcelMapThemeData.LineTopoName
		End Function
		Public Shared Function GetTopoName() As String
			Return mtParcelMapThemeData.TopoName
		End Function
		Public Shared Function GetNameKey(iBlock As Integer, sParcelName As String) As String
			Const sDelim As String = "."
			Return Convert.ToString(iBlock) & sDelim & sParcelName
		End Function
		Public Shared Sub SharedTerminate()

			If moMainDataTable IsNot Nothing Then
				moMainDataTable.Dispose()
				moMainDataTable = Nothing
			End If
			For iIndex As Integer = 0 To moaBlockTable.GetUpperBound(0)
				If moaBlockTable(iIndex) IsNot Nothing Then
					moaBlockTable(iIndex).Dispose()
					moaBlockTable(iIndex) = Nothing
				End If
			Next



			If moLotsContentTable IsNot Nothing Then
				moLotsContentTable.Dispose()
				moLotsContentTable = Nothing
			End If


			If moLanduseApprTable IsNot Nothing Then
				moLanduseApprTable.Dispose()
				moLanduseApprTable = Nothing
			End If

			If moLandusePropTable IsNot Nothing Then
				moLandusePropTable.Dispose()
				moLandusePropTable = Nothing
			End If






			If moaPolygonTable IsNot Nothing Then
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If moaPolygonTable(iOverlayIndex) IsNot Nothing Then
						moaPolygonTable(iOverlayIndex).Dispose()
						moaPolygonTable(iOverlayIndex) = Nothing
					End If
				Next
			End If
			'	Erase miaBlockAttribIndex
			moCalcArea.Terminate()


			'07/08/07    If mdicSumLanduses IsNot Nothing Then
			'07/08/07    mdicSumLanduses.Clear()
			'07/08/07    mdicSumLanduses = Nothing
			'07/08/07    End If


		End Sub
		Public Shared Sub Reset()
			moCalcArea.Reset()
		End Sub
		Public Shared Sub CreateMainDataTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			If moMainDataTable IsNot Nothing Then
				moMainDataTable.Dispose()
				moMainDataTable = Nothing
			End If
			moMainDataTable = zzCreateMainDataTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
		End Sub
		Public Shared Sub CreateMainRegionDataTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			If moMainRegionDataTable IsNot Nothing Then
				moMainRegionDataTable.Dispose()
				moMainRegionDataTable = Nothing
			End If
			moMainRegionDataTable = zzCreateMainDataTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
		End Sub
		Public Shared Sub CreateMainDataTable_030816(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)

			Dim iColumnIndex As Integer = 0
			Dim iPriorIndex As Integer = 0
			If moMainDataTable IsNot Nothing Then
				moMainDataTable.Dispose()
				moMainDataTable = Nothing
			End If
			moMainDataTable = New Data.DataTable("Parcels")
			'	moMainColumnIndices = New Dictionary(Of Integer, Integer)
			If moMainHiddenColumns IsNot Nothing Then
				moMainHiddenColumns.Clear()
				moMainHiddenColumns = Nothing
			End If
			moMainHiddenColumns = New Dictionary(Of Integer, Integer)
			With moMainDataTable.Columns
				'// 0-3 Parcel data 
				.Add(BlockFullFieldName, GetType(System.String))                '0

				.Add(NameFieldName, GetType(System.String))                        '1
				.Add(LegalAreaFieldName, GetType(System.Double))                '2

				.Add(TopoReader.msAreaFldName, GetType(System.Double))               '3


				.Add(msToleranceFieldName, GetType(System.Double))                  '4
				.Add(msDeltaAreaFieldName, GetType(System.Double))
				.Add(msDeviationFieldName, GetType(System.Double))
				.Add(IsAnalyticFieldName, GetType(System.Boolean))

				'// 4-7  Sum of Polygons Area
				.Add(msSumApprPgonAreaMergeFldName, GetType(System.Double))          '8
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(8, 0)
				End If

				.Add(msSumPropPgonAreaMergeFldName, GetType(System.Double))          '9
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(9, 0)
				End If

				.Add(msSumApprPgonAreaFDO_OverlayFldName, GetType(System.Double))          '10
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(10, 0)
				End If

				.Add(msSumPropPgonAreaFDO_OverlayFldName, GetType(System.Double))          '11
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(11, 0)
				End If


				'// 8-13 InPlan Appr   data 
				.Add(msInPlanAreaApprMergeFieldName, GetType(System.Double))         '12
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(12, 0)
				End If

				.Add(msInPlanCalcAreaApprMergeFieldName, GetType(System.Double))     '13
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(13, 0)
				End If

				.Add(msInPlanCalcArea2ApprMergeFieldName, GetType(System.Double))    '14
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(14, 0)
				End If
				.Add(msInPlanRoundedAreaApprMergeFieldName, GetType(System.Double))     '15
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(15, 0)
				End If

				.Add(msPlanStateApprMergeFieldName, GetType(Integer))                ' 16
				.Add(msPlanStateTextApprMergeFieldName, GetType(String))             ' 17
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(16, 0)
					moMainHiddenColumns.Add(17, 0)
				End If


				'// 14-19 InPlan Prop   data 
				.Add(msInPlanAreaPropMergeFieldName, GetType(System.Double))               '18
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(18, 0)
				End If

				.Add(msInPlanCalcAreaPropMergeFieldName, GetType(System.Double))        '19
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(19, 0)
				End If

				.Add(msInPlanCalcArea2PropMergeFieldName, GetType(System.Double))       '20
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(20, 0)
				End If

				.Add(msInPlanRoundedAreaPropMergeFieldName, GetType(System.Double))        '21
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(21, 0)
				End If

				.Add(msPlanStatePropMergeFieldName, GetType(System.Int32))                 ' 22
				.Add(msPlanStateTextPropMergeFieldName, GetType(System.String))            ' 23
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(22, 0)
					moMainHiddenColumns.Add(23, 0)
				End If


0:          '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



				'// 20-21 InPlan Appr   data 
				.Add(msInPlanAreaApprFDO_OverlayFieldName, GetType(System.Double))         '24
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(24, 0)
				End If

				.Add(InPlanCalcAreaApprFDO_OverlayFieldName, GetType(System.Double))     '25
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(25, 0)
				End If

				.Add(msInPlanCalcArea2ApprFDO_OverLayFieldName, GetType(System.Double))    '26
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(26, 0)
				End If
				.Add(msInPlanRoundedAreaApprFDO_OverlayFieldName, GetType(System.Double))     '27
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(27, 0)
				End If
				.Add(msPlanStateApprFDO_OverlayFieldName, GetType(System.Int32))                 ' 28
				.Add(msPlanStateTextApprFDO_OverlayFieldName, GetType(System.String))            ' 29
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(28, 0)
					moMainHiddenColumns.Add(29, 0)
				End If



				'// 22-25 InPlan Prop   data 
				.Add(msInPlanAreaPropFDO_OverlayFieldName, GetType(System.Double))               '30
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(30, 0)
				End If

				.Add(msInPlanCalcAreaPropFDO_OverlayFieldName, GetType(System.Double))        '31
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(31, 0)
				End If

				.Add(msInPlanCalcArea2PropFDO_OverLayFieldName, GetType(System.Double))       '32
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(32, 0)
				End If

				.Add(msInPlanRoundedAreaPropFDO_OverlayFieldName, GetType(System.Double))        '33
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(33, 0)
				End If
				.Add(msPlanStatePropFDO_OverlayFieldName, GetType(System.Int32))                 ' 34
				.Add(msPlanStateTextPropFDO_OverlayFieldName, GetType(System.String))            ' 35
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(34, 0)
					moMainHiddenColumns.Add(35, 0)
				End If

				'// 32-37 InPlan Additional   data 

				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))   '36
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))    '37
				'			.Add(msPlanStateFieldName, GetType(System.Int32))						'28
				'			.Add(msPlanStateTextFieldName, GetType(System.STRING))				'29
				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))    '38
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))              '39
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))       '40
				.Add(BlockFieldName, GetType(System.Int32))                        '41
				.Add(BlockAddFieldName, GetType(System.Int32))                        '42

				.Add(ParcelOrderFieldName, GetType(System.Int32))                  '43
			End With
		End Sub
		Public Shared Function zzCreateMainDataTableNew(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean) As Data.DataTable

			Dim iColumnIndex As Integer = 0
			Dim iPriorIndex As Integer = 0
			Dim oMainDataTable As Data.DataTable = New Data.DataTable("Parcels")
			'	moMainColumnIndices = New Dictionary(Of Integer, Integer)
			If moMainHiddenColumns IsNot Nothing Then
				moMainHiddenColumns.Clear()
				moMainHiddenColumns = Nothing
			End If

			moMainHiddenColumns = New Dictionary(Of Integer, Integer)
			With oMainDataTable.Columns
				'// 0-3 Parcel data 
				.Add(BlockFullFieldName, GetType(System.String))                '0
				.Add(NameFieldName, GetType(System.String))                     '1
				.Add(LegalAreaFieldName, GetType(System.Double))                '2
				.Add(msDBLegalAreaFieldName, GetType(System.Double))                '3

				.Add(TopoReader.msAreaFldName, GetType(System.Double))               '4

				.Add(msToleranceFieldName, GetType(System.Double))                  '5
				.Add(msDeltaAreaFieldName, GetType(System.Double))
				.Add(msDeviationFieldName, GetType(System.Double))
				.Add(IsAnalyticFieldName, GetType(System.Boolean))

				'// 4-7  Sum of Polygons Area
				.Add(msSumApprPgonAreaMergeFldName, GetType(System.Double))          '9
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(9, 0)
				End If

				.Add(msSumPropPgonAreaMergeFldName, GetType(System.Double))          '10
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(10, 0)
				End If

				.Add(msSumApprPgonAreaFDO_OverlayFldName, GetType(System.Double))          '11
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(11, 0)
				End If

				.Add(msSumPropPgonAreaFDO_OverlayFldName, GetType(System.Double))          '12
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(12, 0)
				End If


				'// 8-13 InPlan Appr   data 
				.Add(msInPlanAreaApprMergeFieldName, GetType(System.Double))         '13
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(13, 0)
				End If

				.Add(msInPlanCalcAreaApprMergeFieldName, GetType(System.Double))     '14
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(14, 0)
				End If

				.Add(msInPlanCalcArea2ApprMergeFieldName, GetType(System.Double))    '15
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(15, 0)
				End If
				.Add(msInPlanRoundedAreaApprMergeFieldName, GetType(System.Double))     '16
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(16, 0)
				End If

				.Add(msPlanStateApprMergeFieldName, GetType(Integer))                ' 17
				.Add(msPlanStateTextApprMergeFieldName, GetType(String))             ' 18
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(17, 0)
					moMainHiddenColumns.Add(18, 0)
				End If


				'// 14-19 InPlan Prop   data 
				.Add(msInPlanAreaPropMergeFieldName, GetType(System.Double))               '19
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(19, 0)
				End If

				.Add(msInPlanCalcAreaPropMergeFieldName, GetType(System.Double))        '20
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(20, 0)
				End If

				.Add(msInPlanCalcArea2PropMergeFieldName, GetType(System.Double))       '21
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(21, 0)
				End If

				.Add(msInPlanRoundedAreaPropMergeFieldName, GetType(System.Double))        '22
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(22, 0)
				End If

				.Add(msPlanStatePropMergeFieldName, GetType(System.Int32))                 ' 23
				.Add(msPlanStateTextPropMergeFieldName, GetType(System.String))            ' 24
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(23, 0)
					moMainHiddenColumns.Add(24, 0)
				End If


0:          '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



				'// 20-21 InPlan Appr   data 
				.Add(msInPlanAreaApprFDO_OverlayFieldName, GetType(System.Double))         '25
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(25, 0)
				End If

				.Add(InPlanCalcAreaApprFDO_OverlayFieldName, GetType(System.Double))     '26
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(26, 0)
				End If

				.Add(msInPlanCalcArea2ApprFDO_OverLayFieldName, GetType(System.Double))    '27
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(27, 0)
				End If
				.Add(msInPlanRoundedAreaApprFDO_OverlayFieldName, GetType(System.Double))     '28
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(28, 0)
				End If
				.Add(msPlanStateApprFDO_OverlayFieldName, GetType(System.Int32))                 ' 29
				.Add(msPlanStateTextApprFDO_OverlayFieldName, GetType(System.String))            '30
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(29, 0)
					moMainHiddenColumns.Add(30, 0)
				End If



				'// 22-25 InPlan Prop   data 
				.Add(msInPlanAreaPropFDO_OverlayFieldName, GetType(System.Double))               '31
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(31, 0)
				End If

				.Add(msInPlanCalcAreaPropFDO_OverlayFieldName, GetType(System.Double))        '32
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(32, 0)
				End If

				.Add(msInPlanCalcArea2PropFDO_OverLayFieldName, GetType(System.Double))       '33
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(33, 0)
				End If

				.Add(msInPlanRoundedAreaPropFDO_OverlayFieldName, GetType(System.Double))        '34
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(34, 0)
				End If
				.Add(msPlanStatePropFDO_OverlayFieldName, GetType(System.Int32))                 ' 35
				.Add(msPlanStateTextPropFDO_OverlayFieldName, GetType(System.String))            ' 36
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(35, 0)
					moMainHiddenColumns.Add(36, 0)
				End If

				.Add(msOwnerFieldName, GetType(System.Int32))


				'// 32-37 InPlan Additional   data 

				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))   '37
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))    '38

				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))    '39
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))              '40
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))       '41
				.Add(BlockFieldName, GetType(System.Int32))                        '42
				.Add(BlockAddFieldName, GetType(System.Int32))                        '43

				.Add(ParcelOrderFieldName, GetType(System.Int32))                  '44

				If mbHasOwnershipNotes Then
					.Add(Paragraph19AreaFieldName, GetType(System.Double))
					.Add(LeasingAreaFieldName, GetType(System.Double))
					.Add(OverlayPrg5LeasAreaFieldName, GetType(System.Double))
				End If

			End With
			Return oMainDataTable
		End Function
		Public Shared Function zzCreateMainDataTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean) As Data.DataTable

			Dim iColumnIndex As Integer = 0
			Dim iPriorIndex As Integer = 0
			Dim oMainDataTable As Data.DataTable = New Data.DataTable("Parcels")
			'	moMainColumnIndices = New Dictionary(Of Integer, Integer)
			If moMainHiddenColumns IsNot Nothing Then
				moMainHiddenColumns.Clear()
				moMainHiddenColumns = Nothing
			End If

			moMainHiddenColumns = New Dictionary(Of Integer, Integer)
			With oMainDataTable.Columns
				'// 0-3 Parcel data 
				.Add(BlockFullFieldName, GetType(System.String))                '0
				.Add(NameFieldName, GetType(System.String))                     '1
				.Add(LegalAreaFieldName, GetType(System.Double))                '2
				.Add(msDBLegalAreaFieldName, GetType(System.Double))                '3

				.Add(TopoReader.msAreaFldName, GetType(System.Double))               '4

				.Add(msToleranceFieldName, GetType(System.Double))                  '5
				.Add(msDeltaAreaFieldName, GetType(System.Double))
				.Add(msDeviationFieldName, GetType(System.Double))
				.Add(IsAnalyticFieldName, GetType(System.Boolean))

				'// 4-7  Sum of Polygons Area
				.Add(msSumApprPgonAreaMergeFldName, GetType(System.Double))          '9
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(9, 0)
				End If

				.Add(msSumPropPgonAreaMergeFldName, GetType(System.Double))          '10
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(10, 0)
				End If

				.Add(msSumApprPgonAreaFDO_OverlayFldName, GetType(System.Double))          '11
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(11, 0)
				End If

				.Add(msSumPropPgonAreaFDO_OverlayFldName, GetType(System.Double))          '12
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(12, 0)
				End If


				'// 8-13 InPlan Appr   data 
				.Add(msInPlanAreaApprMergeFieldName, GetType(System.Double))         '13
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(13, 0)
				End If

				.Add(msInPlanCalcAreaApprMergeFieldName, GetType(System.Double))     '14
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(14, 0)
				End If

				.Add(msInPlanCalcArea2ApprMergeFieldName, GetType(System.Double))    '15
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(15, 0)
				End If
				.Add(msInPlanRoundedAreaApprMergeFieldName, GetType(System.Double))     '16
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(16, 0)
				End If

				.Add(msPlanStateApprMergeFieldName, GetType(Integer))                ' 17
				.Add(msPlanStateTextApprMergeFieldName, GetType(String))             ' 18
				If Not (bMerge AndAlso bApproved) Then
					moMainHiddenColumns.Add(17, 0)
					moMainHiddenColumns.Add(18, 0)
				End If


				'// 14-19 InPlan Prop   data 
				.Add(msInPlanAreaPropMergeFieldName, GetType(System.Double))               '19
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(19, 0)
				End If

				.Add(msInPlanCalcAreaPropMergeFieldName, GetType(System.Double))        '20
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(20, 0)
				End If

				.Add(msInPlanCalcArea2PropMergeFieldName, GetType(System.Double))       '21
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(21, 0)
				End If

				.Add(msInPlanRoundedAreaPropMergeFieldName, GetType(System.Double))        '22
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(22, 0)
				End If

				.Add(msPlanStatePropMergeFieldName, GetType(System.Int32))                 ' 23
				.Add(msPlanStateTextPropMergeFieldName, GetType(System.String))            ' 24
				If Not (bMerge AndAlso bProposed) Then
					moMainHiddenColumns.Add(23, 0)
					moMainHiddenColumns.Add(24, 0)
				End If


0:          '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''



				'// 20-21 InPlan Appr   data 
				.Add(msInPlanAreaApprFDO_OverlayFieldName, GetType(System.Double))         '25
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(25, 0)
				End If

				.Add(InPlanCalcAreaApprFDO_OverlayFieldName, GetType(System.Double))     '26
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(26, 0)
				End If

				.Add(msInPlanCalcArea2ApprFDO_OverLayFieldName, GetType(System.Double))    '27
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(27, 0)
				End If
				.Add(msInPlanRoundedAreaApprFDO_OverlayFieldName, GetType(System.Double))     '28
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(28, 0)
				End If
				.Add(msPlanStateApprFDO_OverlayFieldName, GetType(System.Int32))                 ' 29
				.Add(msPlanStateTextApprFDO_OverlayFieldName, GetType(System.String))            '30
				If Not (bFDO_Overlay AndAlso bApproved) Then
					moMainHiddenColumns.Add(29, 0)
					moMainHiddenColumns.Add(30, 0)
				End If



				'// 22-25 InPlan Prop   data 
				.Add(msInPlanAreaPropFDO_OverlayFieldName, GetType(System.Double))               '31
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(31, 0)
				End If

				.Add(msInPlanCalcAreaPropFDO_OverlayFieldName, GetType(System.Double))        '32
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(32, 0)
				End If

				.Add(msInPlanCalcArea2PropFDO_OverLayFieldName, GetType(System.Double))       '33
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(33, 0)
				End If

				.Add(msInPlanRoundedAreaPropFDO_OverlayFieldName, GetType(System.Double))        '34
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(34, 0)
				End If
				.Add(msPlanStatePropFDO_OverlayFieldName, GetType(System.Int32))                 ' 35
				.Add(msPlanStateTextPropFDO_OverlayFieldName, GetType(System.String))            ' 36
				If Not (bFDO_Overlay AndAlso bProposed) Then
					moMainHiddenColumns.Add(35, 0)
					moMainHiddenColumns.Add(36, 0)
				End If

				.Add(msOwnerFieldName, GetType(System.Int32))


				'// 32-37 InPlan Additional   data 

				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))   '37
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))    '38

				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))    '39
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))              '40
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))       '41
				.Add(BlockFieldName, GetType(System.Int32))                        '42
				.Add(BlockAddFieldName, GetType(System.Int32))                        '43

				.Add(ParcelOrderFieldName, GetType(System.Int32))                  '44

				If mbHasOwnershipNotes Then
					.Add(Paragraph19AreaFieldName, GetType(System.Double))
					.Add(LeasingAreaFieldName, GetType(System.Double))
					.Add(OverlayPrg5LeasAreaFieldName, GetType(System.Double))
				End If

			End With
			Return oMainDataTable
		End Function
		Public Shared Sub GetRegionData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, iRegion As Integer, ByVal bEntirety As Boolean, ByVal iDataOptions As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(iCriteriaValue)
			Dim sSort As String = BlockFieldName & ", " & BlockAddFieldName & ", " & ParcelOrderFieldName
			Dim iAreaColIndex As Integer
			Dim iPartialColIndex As Integer
			'	System.Windows.Forms.MessageBox.Show(CStr(moMainDataTable IsNot Nothing) & vbCrLf & iTopoPurpose.ToString() & vbCrLf & iOverlayMethod.ToString() & vbCrLf & iDataOptions.ToString(), "02_001a")

			If moMainDataTable IsNot Nothing Then
				'DMAcadExt.AcadDocument.WriteMessage("#!!!!099Filter: " & sFilter)
				oDataView = New System.Data.DataView(moMainDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				'	System.Windows.Forms.MessageBox.Show(CStr(moMainDataTable.Rows.Count) & ":" & CStr(oDataView.Count) & vbCrLf & sFilter, "02_002")
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					Select Case iDataOptions
						Case enDataOptions.AcadArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 8 + 3

								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 20 + 4
							End Select

						Case enDataOptions.CalcMergeArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 9 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 21 + 4
							End Select

						Case enDataOptions.CalcMergeArea2
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 10 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 22 + 4
							End Select

						Case enDataOptions.RoundedArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 11 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 23 + 4
							End Select

					End Select

					Select Case iOverlayMethod
						Case DMAcadExt.enOverlayMethod.Merge
							iPartialColIndex = 13 + 3

						Case DMAcadExt.enOverlayMethod.Union
						Case DMAcadExt.enOverlayMethod.FDO_Overlay
							iPartialColIndex = 29
					End Select
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					Select Case iDataOptions
						Case enDataOptions.AcadArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 14 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 26 + 3
							End Select

						Case enDataOptions.CalcMergeArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 15 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 27 + 3
							End Select
						Case enDataOptions.CalcMergeArea2
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 16 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 28 + 3
							End Select
						Case enDataOptions.RoundedArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 17 + 3
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 29 + 3
							End Select
					End Select

					Select Case iOverlayMethod
						Case DMAcadExt.enOverlayMethod.Merge
							iPartialColIndex = 19 + 3

						Case DMAcadExt.enOverlayMethod.Union
						Case DMAcadExt.enOverlayMethod.FDO_Overlay
							iPartialColIndex = 23
					End Select
				End If

				If bEntirety Then
					Dim iaAcadColumns() As Integer = {0, 1, 2, iAreaColIndex, iPartialColIndex}
					''''	iaAcadColumns(3) = iAreaColIndex
					iaColumns = iaAcadColumns
				Else
					'	System.Windows.Forms.MessageBox.Show(iTopoPurpose.ToString(), "01_399j")
					Dim iaAcadColumns() As Integer = {0, 1, 2, iAreaColIndex}
					''	iaAcadColumns(3) = iAreaColIndex
					iaColumns = iaAcadColumns
				End If
				'	DMCommon.Functions.DispArray(iaColumns, "iaColumns")


				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim s As String = ""
				Dim dSumInPlanAppr As Double = 0.0, dSumInPlanProp As Double = 0.0, dSumInPlan As Double = 0.0, dSumLegal As Double = 0.0
				Dim oDataRowView As DataRowView
				For i As Integer = 0 To iaColumns.GetUpperBound(0)
					s &= CStr(iaColumns(i)) & ":" & moMainDataTable.Columns.Item(iaColumns(i)).ColumnName & vbCrLf
				Next
				'	MessageBox.Show(moMainDataTable.Columns.Item(iVarColIndex).ColumnName, "01_681")
				'	MessageBox.Show(s, "01_682")
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					'	dSumInPlanAppr += DMCommon.Functions.CDblN(oDataRowView.Item(msInPlanCalcAreaApprMergeFieldName))
					'	dSumInPlanProp += DMCommon.Functions.CDblN(oDataRowView.Item(msInPlanCalcAreaPropMergeFieldName))
					s = moMainDataTable.Columns(iAreaColIndex).ColumnName
					dSumInPlan += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
					dSumLegal += DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
					'	DMAcadExt.AcadDocument.WriteMessage("#" & CStr(iRowIndex) & ": " & CStr(DirectCast(oDataRowView.Item(msLegalAreaFieldName), Double)) & ", " & CStr(DirectCast(oDataRowView.Item(iVarColIndex), Double)))
				Next
				ReDim oaTotals(1)
				'  oaTotals(0) = FormatNumber(dSumA, 3, TriState.True, , TriState.True)
				'  oaTotals(1) = FormatNumber(dSumB, 3, TriState.True, , TriState.True)
				oaTotals(0) = dSumInPlan  'dSumInPlanProp
				oaTotals(1) = dSumLegal
				'	oaTotals(2) = dSumInPlanAppr
				DMAcadExt.AcadDocument.WriteDebugMessage("# Total A" & ": " & CStr(oaTotals(0)) & ", " & CStr(dSumInPlan))
				DMAcadExt.AcadDocument.WriteDebugMessage("# Total B" & ": " & CStr(oaTotals(1)) & ", " & CStr(dSumLegal))



			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "GetInnPlanData")
			End If


		End Sub
		Public Shared Sub GetInPlanData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOptions As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, bRegion As Boolean, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(iCriteriaValue)
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			Dim iAreaColIndex As Integer
			Dim iPartialColIndex As Integer
			Dim sTest As String
			'	DMCommon.Debug.ExcelLog.SetDataTable(0, "Region", moMainRegionDataTable)


			'130126	DMCommon.Debug.ExcelLog.SetDataTable(0, "Main", moMainDataTable)

			Dim oDataTable As Data.DataTable
			If bRegion Then
				oDataTable = moMainRegionDataTable
				sTest = "moMainRegion"
			Else
				oDataTable = moMainDataTable
				sTest = "moMain Only"
			End If
			If oDataTable IsNot Nothing Then
				'DMAcadExt.AcadDocument.WriteMessage("#!!!!099Filter: " & sFilter)
				oDataView = New System.Data.DataView(oDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
				'	System.Windows.Forms.MessageBox.Show(CStr(oDataTable.Rows.Count) & ":" & CStr(oDataView.Count) & vbCrLf & sFilter, "02_002")

				'DMCommon.Debug.ExcelLog.SetDataTable(0, "InPlan", oDataView, 10)
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					Select Case iDataOptions
						Case enDataOptions.AcadArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 8 + 3 + 1

								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 20 + 4 + 1
							End Select

						Case enDataOptions.CalcMergeArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 9 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 21 + 4 + 1
							End Select

						Case enDataOptions.CalcMergeArea2
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 10 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 22 + 4 + 1
							End Select

						Case enDataOptions.RoundedArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 11 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 23 + 4 + 1
							End Select

					End Select

					Select Case iOverlayMethod
						Case DMAcadExt.enOverlayMethod.Merge
							iPartialColIndex = 13 + 3 + 1

						Case DMAcadExt.enOverlayMethod.Union
						Case DMAcadExt.enOverlayMethod.FDO_Overlay
							iPartialColIndex = 29 + 1
					End Select
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					Select Case iDataOptions
						Case enDataOptions.AcadArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 14 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 26 + 3 + 1
							End Select

						Case enDataOptions.CalcMergeArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 15 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 31 + 1
							End Select
3:




						Case enDataOptions.CalcMergeArea2
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 16 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 32 + 1
							End Select
						Case enDataOptions.RoundedArea
							Select Case iOverlayMethod
								Case DMAcadExt.enOverlayMethod.Merge
									iAreaColIndex = 17 + 3 + 1
								Case DMAcadExt.enOverlayMethod.Union
								Case DMAcadExt.enOverlayMethod.FDO_Overlay
									iAreaColIndex = 33 + 1
							End Select
					End Select

					Select Case iOverlayMethod
						Case DMAcadExt.enOverlayMethod.Merge
							iPartialColIndex = 19 + 3 + 1

						Case DMAcadExt.enOverlayMethod.Union
						Case DMAcadExt.enOverlayMethod.FDO_Overlay
							iPartialColIndex = 35 + 1
					End Select
				End If

				If bEntirety Then
					Dim iaAcadColumns() As Integer = {0, 1, 2, iAreaColIndex, iPartialColIndex}
					''''	iaAcadColumns(3) = iAreaColIndex
					iaColumns = iaAcadColumns
				Else
					'	System.Windows.Forms.MessageBox.Show(iTopoPurpose.ToString(), "01_399j")
					Dim iaAcadColumns() As Integer = {0, 1, 2, iAreaColIndex}
					''	iaAcadColumns(3) = iAreaColIndex
					iaColumns = iaAcadColumns
				End If
				'	DMCommon.Functions.DispArray(iaColumns, "iaColumns")


				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim s As String = ""
				Dim dSumInPlanAppr As Double = 0.0, dSumInPlanProp As Double = 0.0, dSumInPlan As Double = 0.0, dSumLegal As Double = 0.0
				Dim oDataRowView As DataRowView
				For i As Integer = 0 To iaColumns.GetUpperBound(0)
					s &= CStr(iaColumns(i)) & ":" & oDataTable.Columns.Item(iaColumns(i)).ColumnName & vbCrLf
				Next
				'	MessageBox.Show(oDataTable.Columns.Item(iVarColIndex).ColumnName, "01_681")
				'	MessageBox.Show(s, "01_682")
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					'	dSumInPlanAppr += DMCommon.Functions.CDblN(oDataRowView.Item(msInPlanCalcAreaApprMergeFieldName))
					'	dSumInPlanProp += DMCommon.Functions.CDblN(oDataRowView.Item(msInPlanCalcAreaPropMergeFieldName))
					s = oDataTable.Columns(iAreaColIndex).ColumnName
					dSumInPlan += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
					dSumLegal += DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
					'	DMAcadExt.AcadDocument.WriteMessage("#" & CStr(iRowIndex) & ": " & CStr(DirectCast(oDataRowView.Item(msLegalAreaFieldName), Double)) & ", " & CStr(DirectCast(oDataRowView.Item(iVarColIndex), Double)))
				Next
				ReDim oaTotals(1)
				'  oaTotals(0) = FormatNumber(dSumA, 3, TriState.True, , TriState.True)
				'  oaTotals(1) = FormatNumber(dSumB, 3, TriState.True, , TriState.True)
				oaTotals(0) = dSumInPlan  'dSumInPlanProp
				oaTotals(1) = dSumLegal
				'	oaTotals(2) = dSumInPlanAppr
				'	DMAcadExt.AcadDocument.WriteDebugMessage("# Total A" & ": " & CStr(oaTotals(0)) & ", " & CStr(dSumInPlan))
				'	DMAcadExt.AcadDocument.WriteDebugMessage("# Total B" & ": " & CStr(oaTotals(1)) & ", " & CStr(dSumLegal))

			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "GetInPlanData")
			End If
		End Sub

		Public Shared Sub GetLanduseData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, iRegion <> 0, True)
			' Dim sAreaFldName As String
			'DMCommon.Debug.MsgBox("13_101c", DMCommon.Debug.ColCount(oLanduseTable))
			'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_120")
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & LanduseOrderFieldName
			If False AndAlso oLanduseTable IsNot Nothing Then
				MessageBox.Show(iRegion.ToString() & vbCrLf & iTopoPurpose.ToString() & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_121")
			End If
			If oLanduseTable IsNot Nothing AndAlso oLanduseTable.Rows.Count <> 0 Then
				'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLandusePropTable Is Nothing) & ":" & CStr(moLanduseApprTable Is Nothing) & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_122")

				Dim iaAcadColumns() As Integer = {0, 1, 4, -1, -1, 3}
				Dim iAreaColIndex As Integer
				Dim iLuseAreaColIndex As Integer
				'	Dim iLegalAreaColIndex As Integer = 4


				Select Case iOptions
					Case enDataOptions.AcadArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 6
							iLuseAreaColIndex = 14
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 10
							iLuseAreaColIndex = 18
						End If


					Case enDataOptions.CalcMergeArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 7
							iLuseAreaColIndex = 15
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 11
							iLuseAreaColIndex = 19
						End If


					Case enDataOptions.CalcMergeArea2
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 8
							iLuseAreaColIndex = 16
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 12
							iLuseAreaColIndex = 20
						End If
					Case enDataOptions.RoundedArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 9
							iLuseAreaColIndex = 17
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 13
							iLuseAreaColIndex = 21
						End If
				End Select
				iaColumns = iaAcadColumns
				iaAcadColumns(3) = iAreaColIndex
				iaAcadColumns(4) = iLuseAreaColIndex

				oDataView = New System.Data.DataView(oLanduseTable, "LUAreaF<>0", sSort, DataViewRowState.CurrentRows)
				'

				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False

				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0

				Dim oDataRowView As DataRowView
				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0

				Dim iPrevParcelOrder As Integer = 0

				Dim iCurrentBlock As Integer
				Dim iCurrentBlockAdd As Integer

				Dim iCurrentParcelOrder As Integer


				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					dSum0 += DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
					If False AndAlso miDebugCounterA < 14 Then
						TplnProject.WriteMessageBox("ARR " & CStr(iLuseAreaColIndex) & "==" & CStr(DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))), "")
						miDebugCounterA += 1

					End If
					iCurrentBlock = DirectCast(oDataRowView.Item(BlockFieldName), Integer)
					iCurrentBlockAdd = DirectCast(oDataRowView.Item(BlockAddFieldName), Integer)
					iCurrentParcelOrder = DirectCast(oDataRowView.Item(ParcelOrderFieldName), Integer)

					If iPrevBlock <> iCurrentBlock OrElse iPrevBlockAdd <> iCurrentBlockAdd OrElse iPrevParcelOrder <> iCurrentParcelOrder Then
						dSum2 += DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
						dSum1 += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))


						iPrevBlock = iCurrentBlock
						iPrevBlockAdd = iCurrentBlockAdd

						iPrevParcelOrder = iCurrentParcelOrder
					End If
				Next
				ReDim oaTotals(2)
				oaTotals(0) = dSum0
				oaTotals(1) = dSum1
				oaTotals(2) = dSum2

			End If
		End Sub
		Structure LanduseCol
			Sub New(iLanduseID As Integer, sLanduseName As String, iColumnIndex As Integer)
				LanduseID = iLanduseID
				LanduseName = sLanduseName
				ColumnIndex = iColumnIndex
			End Sub
			Dim LanduseID As Integer
			Dim LanduseName As String
			Dim ColumnIndex As Integer

		End Structure
		Public Shared Sub GetLanduseDataByCol_050617(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object, ByRef iLanduseColUB As Integer, ByRef oaCaptions() As System.Object)
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, iRegion <> 0, True)

			Dim sGroupFieldName As String
			TplnProject.GetLusePgon(iTopoPurpose, 0)
			' Dim sAreaFldName As String
			If oLanduseTable IsNot Nothing Then
				'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(oLanduseTable.Rows.Count) & ":" & CStr(88), "05_122q")
			End If

			'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_121s")
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & LanduseOrderFieldName
			If False AndAlso oLanduseTable IsNot Nothing Then
				MessageBox.Show(iRegion.ToString() & vbCrLf & iTopoPurpose.ToString() & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_121")
			End If

			If oLanduseTable IsNot Nothing AndAlso oLanduseTable.Rows.Count <> 0 Then
				Dim dicLanduses As Dictionary(Of Integer, LanduseCol) = New Dictionary(Of Integer, LanduseCol)()
				Dim iLanduseID As Integer
				Dim iColIndex As Integer = 0
				Dim tLanduseCol As LanduseCol = New LanduseCol()
				For iRowIndex As Integer = 0 To oLanduseTable.Rows.Count - 1
					With oLanduseTable.Rows.Item(iRowIndex)
						iLanduseID = DMCommon.Functions.CIntN(.Item(LanduseIDFieldName))
						If iLanduseID <> 0 AndAlso Not dicLanduses.ContainsKey(iLanduseID) Then
							dicLanduses.Add(iLanduseID, New LanduseCol(iLanduseID, DMCommon.Functions.CStrN(.Item(LanduseNameFieldName)), iColIndex))
							iColIndex += 1
						End If

					End With
				Next
				iLanduseColUB = iColIndex - 1
				ReDim oaCaptions(iLanduseColUB)
				Dim iAreaColIndex As Integer
				Dim iLuseAreaColIndex As Integer

				'	Dim iLegalAreaColIndex As Integer = 4
				Select Case iOptions
					Case enDataOptions.AcadArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 6
							iLuseAreaColIndex = 14
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 10
							iLuseAreaColIndex = 18
						End If
					Case enDataOptions.CalcMergeArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 7
							iLuseAreaColIndex = 15
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 11
							iLuseAreaColIndex = 19
						End If
					Case enDataOptions.CalcMergeArea2
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 8
							iLuseAreaColIndex = 16
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 12
							iLuseAreaColIndex = 20
						End If
					Case enDataOptions.RoundedArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 9
							iLuseAreaColIndex = 17
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 13
							iLuseAreaColIndex = 21
						End If
				End Select

				Dim iBaseColUB As Integer = oLanduseTable.Columns.Count - 1
				Dim iaBaseAcadColumns() As Integer = {0, 1, 4, iAreaColIndex} '  0,1, 4  {0, 1, 4, -1, -1, 3}
				Dim iBaseUB As Integer = iaBaseAcadColumns.GetUpperBound(0)
				Dim iGroupIndex As Integer = 1
				iaColumns = iaBaseAcadColumns
				' DMCommon.Functions.DispArray(iaColumns, "iaColumns_1")
				ReDim Preserve iaColumns(iBaseUB + iLanduseColUB + 1)
				' DMCommon.Functions.DispArray(iaColumns, "iaColumns_2")
				'   MessageBox.Show(dicLanduses.Count.ToString() & ":" & iColIndex.ToString(), "04_540")
				For Each tLanduseCol In dicLanduses.Values
					oaCaptions(tLanduseCol.ColumnIndex) = tLanduseCol.LanduseName
					sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)
					If Not oLanduseTable.Columns.Contains(sGroupFieldName) Then
						oLanduseTable.Columns.Add(sGroupFieldName, GetType(System.Double))
					End If

					iaColumns(iBaseUB + iGroupIndex) = iBaseColUB + iGroupIndex
					iGroupIndex += 1
				Next


				'If oaCaptions IsNot Nothing Then
				'  DMCommon.Functions.DispArray(iaColumns, "iaColumns_L")
				'   DMCommon.Functions.DispArray(oaCaptions, "01_882x", True)


				'Else
				'   MessageBox.Show("oaCaptions Is Nothing", "01_882xno")
				'End If
				' MessageBox.Show(dicLanduses.Count.ToString & ":" & iColIndex.ToString(), "09_879")
				'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLandusePropTable Is Nothing) & ":" & CStr(moLanduseApprTable Is Nothing) & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_122")



				'   iaColumns = iaBaseAcadColumns
				''''''''''''''''''''''''    iaBaseAcadColumns(3) = iAreaColIndex
				'''''''''''''''''''''''''    iaBaseAcadColumns(4) = iLuseAreaColIndex

				'' Dim iaAcadColumns(iBaseUB + iLanduseColUB + 1) As Integer
				'''''ReDim iaColumns(iBaseUB + iLanduseColUB + 1)
				'For iIndex As Integer = 0 To iBaseUB
				'   iaColumns(iLanduseColUB + 1 + iIndex) = iaBaseAcadColumns(iIndex)
				'Next


				' 160141 
				oDataView = New System.Data.DataView(oLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				'
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim daLanduseSum(iLanduseColUB + 2) As Double
				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0
				Dim dFragmentArea As Double
				Dim oDataRowView As DataRowView
				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0

				Dim iPrevParcelOrder As Integer = 0

				Dim iCurrentBlock As Integer
				Dim iCurrentBlockAdd As Integer
				Dim iCurrentParcelOrder As Integer
				' Dim tLanduseCol As LanduseCol
				If iRegion <> 0 Then
					'''''''''''''''''''''''''''''  DMCommon.ExcelLog.SetDataTable(oDataView, 0)
				End If

				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					iLanduseID = DMCommon.Functions.CIntN(oDataRowView.Item(LanduseIDFieldName))
					If iLanduseID <> 0 AndAlso dicLanduses.TryGetValue(iLanduseID, tLanduseCol) Then
						sGroupFieldName = "Landuse_" & CStr(iLanduseID)
						oDataRowView.Row.Item(sGroupFieldName) = DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						dFragmentArea = DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						daLanduseSum(iLanduseColUB - tLanduseCol.ColumnIndex) += dFragmentArea
						daLanduseSum(iLanduseColUB + 1) += dFragmentArea
						If False AndAlso miDebugCounterA < 14 Then
							TplnProject.WriteMessageBox("ARR " & CStr(iLuseAreaColIndex) & "==" & CStr(DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))), "")
							miDebugCounterA += 1

						End If
						iCurrentBlock = DirectCast(oDataRowView.Item(BlockFieldName), Integer)
						iCurrentBlockAdd = DirectCast(oDataRowView.Item(BlockAddFieldName), Integer)
						iCurrentParcelOrder = DirectCast(oDataRowView.Item(ParcelOrderFieldName), Integer)
						'   sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)

						If iPrevBlock <> iCurrentBlock OrElse iPrevBlockAdd <> iCurrentBlockAdd OrElse iPrevParcelOrder <> iCurrentParcelOrder Then
							daLanduseSum(iLanduseColUB + 2) += DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
							dSum2 += DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
							dSum1 += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
							iPrevBlock = iCurrentBlock
							iPrevBlockAdd = iCurrentBlockAdd

							iPrevParcelOrder = iCurrentParcelOrder
						End If
					End If
				Next
				'  MessageBox.Show(dSum0.ToString() & vbCrLf & dSum1.ToString() & vbCrLf & dSum2.ToString(), "19_006")
				'  DMCommon.Functions.DispArray(daLanduseSum, "dLanduseSum")
				ReDim oaTotals(daLanduseSum.GetUpperBound(0))
				For iIndex As Integer = 0 To daLanduseSum.GetUpperBound(0)
					oaTotals(iIndex) = daLanduseSum(iIndex)
					' oaTotals(iIndex) = 1000 + iIndex * 10000.0

				Next
				'      DMCommon.Functions.DispArray(oaTotals, "oaTotals", True)
				'oaTotals(0) = dSum0
				'oaTotals(1) = dSum1
				'oaTotals(2) = dSum2
				'oaTotals(3) = 1952.0

				'oaTotals(0) = 100.1
				'oaTotals(1) = 200.1
				'oaTotals(2) = 300.1
				'  oaTotals(3) = "A"
				'oaTotals(0) = "A"
				'oaTotals(1) = "B"
				'oaTotals(2) = "C"
				'oaTotals(3) = "D"
			End If
		End Sub
		Public Shared Sub GetExproDataByCol(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oResDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object, ByRef iLanduseColUB As Integer, ByRef oaCaptions() As System.Object)

			'DMCommon.Debug.ExcelLog.SetDataTable(0, "ExproTable", moExproTable)
			Dim sGroupFieldName As String
			TplnProject.GetLusePgon(iTopoPurpose, 0)
			' Dim sAreaFldName As String


			'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_121s")
			Dim sSort As String = "" ' msBlockFieldName & "," & msBlockAddFieldName & "," & msParcelOrderFieldName & "," & msLanduseOrderFieldName


			If moExproTable IsNot Nothing AndAlso moExproTable.Rows.Count <> 0 Then
				Dim dicLanduses As Dictionary(Of Integer, LanduseCol) = New Dictionary(Of Integer, LanduseCol)()
				Dim iLanduseID As Integer
				Dim iColIndex As Integer = 0
				Dim tLanduseCol As LanduseCol = New LanduseCol()
				For iRowIndex As Integer = 0 To moExproTable.Rows.Count - 1
					With moExproTable.Rows.Item(iRowIndex)
						iLanduseID = DMCommon.Functions.CIntN(.Item(ExproTypeIDFieldName))
						If iLanduseID <> 0 AndAlso Not dicLanduses.ContainsKey(iLanduseID) Then
							dicLanduses.Add(iLanduseID, New LanduseCol(iLanduseID, DMCommon.Functions.CStrN(.Item(ExproTypeNameFieldName)), iColIndex))
							iColIndex += 1
						End If

					End With
				Next
				iLanduseColUB = iColIndex - 1
				ReDim oaCaptions(iLanduseColUB)
				Dim iAreaColIndex As Integer
				Dim iLuseAreaColIndex As Integer

				'	Dim iLegalAreaColIndex As Integer = 4
				Select Case iOptions
					Case enDataOptions.AcadArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 6
							iLuseAreaColIndex = 14
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 10
							iLuseAreaColIndex = 18
						End If
					Case enDataOptions.CalcMergeArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 7
							iLuseAreaColIndex = 15
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 5
							iAreaColIndex = 9

							iLuseAreaColIndex = 7
						End If
					Case enDataOptions.CalcMergeArea2
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 8
							iLuseAreaColIndex = 16
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 12
							iLuseAreaColIndex = 20
						End If
					Case enDataOptions.RoundedArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 9
							iLuseAreaColIndex = 17
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 13
							iLuseAreaColIndex = 21
						End If
				End Select

				Dim iBaseColUB As Integer = moExproTable.Columns.Count - 1
				'	DMCommon.Debug.MsgBox("12_642k", iOptions, iOverlayMethod, iBaseColUB, iAreaColIndex, iLuseAreaColIndex)
				Dim iaBaseAcadColumns() As Integer = {0, 1, 4, 5, 6, 7, 8, iAreaColIndex} '  0,1, 4  {0, 1, 4, -1, -1, 3}
				Dim iBaseUB As Integer = iaBaseAcadColumns.GetUpperBound(0)
				Dim iGroupIndex As Integer = 1
				Dim oResultTable As System.Data.DataTable = moExproTable.Clone

				iaColumns = iaBaseAcadColumns
				' DMCommon.Functions.DispArray(iaColumns, "iaColumns_1")
				'	DMCommon.Debug.MsgBox("12_642x", iBaseUB, iLanduseColUB)
				ReDim Preserve iaColumns(iBaseUB + iLanduseColUB + 1)

				DMCommon.Functions.DispArray("iaColumns_2b", iaColumns)
				'   MessageBox.Show(dicLanduses.Count.ToString() & ":" & iColIndex.ToString(), "04_540")
				For Each tLanduseCol In dicLanduses.Values
					oaCaptions(iLanduseColUB - tLanduseCol.ColumnIndex) = tLanduseCol.LanduseName
					sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)
					If Not oResultTable.Columns.Contains(sGroupFieldName) Then
						oResultTable.Columns.Add(sGroupFieldName, GetType(System.Double))
					End If

					iaColumns(iBaseUB + iGroupIndex) = iBaseColUB + iGroupIndex
					iGroupIndex += 1
				Next
				DMCommon.Functions.DispArray("iaColumns_2a", iaColumns)



				' 160141 

				Dim oDataView As System.Data.DataView = New System.Data.DataView(moExproTable, String.Empty, sSort, DataViewRowState.CurrentRows)

				'	DMCommon.Debug.MsgBox("12_642", oDataView.Count, moExproTable.Rows.Count)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim daLanduseSum(iLanduseColUB + 2) As Double
				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0
				Dim dFragmentArea As Double
				Dim oDataRowView As DataRowView
				Dim oResDataRow As DataRow = Nothing

				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0
				'054 6 810191   Tanya Kurnik
				Dim iPrevParcel As Integer = 0

				'	Dim iPrevParcelOrder As Integer = 0

				Dim iCurrentBlock As Integer
				Dim iCurrentBlockAdd As Integer
				Dim iCurrentParcel As Integer
				'	Dim iCurrentParcelOrder As Integer

				Dim dLegalArea As Double
				'   Dim dLuseArea As Double

				' Dim tLanduseCol As LanduseCol
				If iRegion <> 0 Then
					'''''''''''''''''''''''''''''  DMCommon.ExcelLog.SetDataTable(oDataView, 0)
				End If
				DMCommon.Debug.MsgBox("12_640d", oDataView.Count)
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					iLanduseID = DMCommon.Functions.CIntN(oDataRowView.Item(ExproTypeIDFieldName))
					If iLanduseID <> 0 AndAlso dicLanduses.TryGetValue(iLanduseID, tLanduseCol) Then
						Try
							iCurrentBlock = DirectCast(oDataRowView.Item(BlockFieldName), Integer)
							iCurrentBlockAdd = DMCommon.Functions.CIntN(oDataRowView.Item(BlockAddFieldName))
							iCurrentParcel = DMCommon.Functions.CIntN(oDataRowView.Item(NameFieldName))
						Catch ex As Exception

						End Try
						'	iCurrentParcelOrder = DirectCast(oDataRowView.Item(msParcelOrderFieldName), Integer)
						dLegalArea = DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
						dFragmentArea = Math.Round(DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex)), 0)

						'   sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)

						If iPrevBlock <> iCurrentBlock OrElse iPrevBlockAdd <> iCurrentBlockAdd OrElse iPrevParcel <> iCurrentParcel Then
							If oResDataRow IsNot Nothing Then
								oResultTable.Rows.Add(oResDataRow)
							End If
							oResDataRow = oResultTable.NewRow()

							oResDataRow.Item(BlockFullFieldName) = oDataRowView.Item(BlockFullFieldName)
							If iCurrentBlockAdd <> 0 Then
								oResDataRow.Item(BlockAddFieldName) = iCurrentBlockAdd
							End If
							'oResDataRow.Item(NameFieldName) = iCurrentParcelOrder
							oResDataRow.Item(NameFieldName) = oDataRowView.Item(NameFieldName)
							oResDataRow.Item(LegalAreaFieldName) = dLegalArea ' oDataRowView.Item(msLegalAreaFieldName)




							daLanduseSum(iLanduseColUB + 2) += dLegalArea ' DMCommon.Functions.CDblN(oDataRowView.Item(msLegalAreaFieldName))
							dSum2 += dLegalArea ' DMCommon.Functions.CDblN(oDataRowView.Item(msLegalAreaFieldName))
							dSum1 += dFragmentArea 'DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
							iPrevBlock = iCurrentBlock
							iPrevBlockAdd = iCurrentBlockAdd

							iPrevParcel = iCurrentParcel
						End If



						sGroupFieldName = "Landuse_" & CStr(iLanduseID)
						oResDataRow.Item(sGroupFieldName) = dFragmentArea 'DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						'   dFragmentArea = DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						daLanduseSum(iLanduseColUB - tLanduseCol.ColumnIndex) += dFragmentArea
						daLanduseSum(iLanduseColUB + 1) += dFragmentArea
						oResDataRow.Item(iAreaColIndex) = DMCommon.Functions.CDblN(oResDataRow.Item(iAreaColIndex)) + dFragmentArea ' oDataRowView.Item(iAreaColIndex)
						'    DMCommon.ExcelLogG.SetNextValueInRow(i, 0, iCurrentBlock, oDataRowView.Item(msNameFieldName), dLegalArea, dFragmentArea, sGroupFieldName)
					End If
				Next

				If oResDataRow IsNot Nothing Then
					oResultTable.Rows.Add(oResDataRow)
				End If
				oResDataView = New DataView(oResultTable)
				'  MessageBox.Show(dSum0.ToString() & vbCrLf & dSum1.ToString() & vbCrLf & dSum2.ToString(), "19_006")
				'  DMCommon.Functions.DispArray(daLanduseSum, "dLanduseSum")
				ReDim oaTotals(daLanduseSum.GetUpperBound(0))
				For iIndex As Integer = 0 To daLanduseSum.GetUpperBound(0)
					oaTotals(iIndex) = daLanduseSum(iIndex)
					' oaTotals(iIndex) = 1000 + iIndex * 10000.0

				Next
				'	DMCommon.Functions.DispArray("iaColumns", iaColumns)
				'	DMCommon.Functions.DispArray("oaCaptions", oaCaptions, True)
				'	DMCommon.Debug.MsgBox("iLanduseColUB", iLanduseColUB)

				'      DMCommon.Functions.DispArray(oaTotals, "oaTotals", True)
				'oaTotals(0) = dSum0
				'oaTotals(1) = dSum1
				'oaTotals(2) = dSum2
				'oaTotals(3) = 1952.0

				'oaTotals(0) = 100.1
				'oaTotals(1) = 200.1
				'oaTotals(2) = 300.1
				'  oaTotals(3) = "A"
				'oaTotals(0) = "A"
				'oaTotals(1) = "B"
				'oaTotals(2) = "C"
				'oaTotals(3) = "D"
			End If
		End Sub


		Public Shared Sub GetExproDataByColExt(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oResDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object, ByRef iLanduseColUB As Integer, ByRef oaCaptions() As System.Object)

			TplnProject.GetLusePgon(iTopoPurpose, 0)
			' Dim sAreaFldName As String
			DMCommon.Debug.MsgBox("13_407", moExproTableByCol.Rows.Count)


			'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_121s")
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & NameFieldName


			If moExproTableByCol IsNot Nothing AndAlso moExproTableByCol.Rows.Count <> 0 Then
				Dim iaBaseAcadColumns() As Integer = {0, 1, 2, 3, 4, 5, 6, 7}
				Dim iBaseUB As Integer = iaBaseAcadColumns.GetUpperBound(0)
				iaColumns = iaBaseAcadColumns
				ReDim oaCaptions(mhsExproTypes.Count + 6 * miMaxOutPgonsCount - 1)
				ReDim oaTotals(mhsExproTypes.Count + 6 * miMaxOutPgonsCount - 1 + iBaseUB)
				Dim daTotals(mhsExproTypes.Count - 1) As Double
				Dim dAreaInTotal As Double = 0.0
				iLanduseColUB = mhsExproTypes.Count + 6 * miMaxOutPgonsCount - 1
				ReDim Preserve iaColumns(iBaseUB + mhsExproTypes.Count + 6 * miMaxOutPgonsCount)

				'''''''''''''''''''''iaColumns


				'	Dim iLegalAreaColIndex As Integer = 4


				oResDataView = New System.Data.DataView(moExproTableByCol, String.Empty, sSort, DataViewRowState.CurrentRows)
				'		DMCommon.ExcelLog.SetDataTable(0, "ExproTableNew", oResDataView, 12)

				oResDataView.AllowEdit = False
				oResDataView.AllowDelete = False
				oResDataView.AllowNew = False
				Dim daLanduseSum(iLanduseColUB + 2) As Double
				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0

				'	Dim oDataRowView As DataRowView
				Dim oResDataRow As DataRow = Nothing

				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0
				'054 6 810191   Tanya Kurnik
				Dim iPrevParcel As Integer = 0

				'	Dim iPrevParcelOrder As Integer = 0

				'Dim dLegalArea As Double
				'   Dim dLuseArea As Double

				' Dim tLanduseCol As LanduseCol
				If iRegion <> 0 Then
					'''''''''''''''''''''''''''''  DMCommon.ExcelLog.SetDataTable(oDataView, 0)
				End If


				'	DMCommon.Debug.MsgBox("12_640h", oResDataView.Count)


				Dim iCaptionIndex As Integer = 0

				For iType As Integer = 1 To miMaxExproType
					If mhsExproTypes.Contains(iType) Then
						oaCaptions(iLanduseColUB - iCaptionIndex) = TplnExpro.GetExproTypeName(iType) & " (מ""ר)"
						iaColumns(iBaseUB + 1 + iCaptionIndex) = iBaseUB + iCaptionIndex + 1
						iCaptionIndex += 1
					End If
				Next

				For iPgonIndex As Integer = 0 To miMaxOutPgonsCount - 1
					For iFieldIndex As Integer = 0 To 5

						oaCaptions(iLanduseColUB - iCaptionIndex) = zzGetOutPgonCaption(iPgonIndex + 1, iFieldIndex)
						iaColumns(iBaseUB + 1 + iCaptionIndex) = iBaseUB + iCaptionIndex + 1
						iCaptionIndex += 1
					Next
				Next
				Dim iColumnIndex As Integer
				For Each oDataRowView As DataRowView In oResDataView
					For iIndex As Integer = 0 To daTotals.GetUpperBound(0)
						iColumnIndex = iIndex + 8
						daTotals(iIndex) += DMCommon.Functions.CDblN(oDataRowView.Item(iColumnIndex))
					Next
					dAreaInTotal += DMCommon.Functions.CDblN(oDataRowView.Item(msInPlanCalcAreaFDO_OverlayFieldName))
				Next
				For iIndex As Integer = 0 To daTotals.GetUpperBound(0)

					oaTotals(oaTotals.GetUpperBound(0) - (8 - 1 + iIndex)) = daTotals(iIndex)
				Next
				oaTotals(oaTotals.GetUpperBound(0) - 7) = dAreaInTotal
				oaTotals(oaTotals.GetUpperBound(0) - 6) = 6666.6


				'	DMCommon.Functions.DispArray("iaColumns", iaColumns)
				'	DMCommon.Functions.DispArray("oaCaptions", oaCaptions, True)
				DMCommon.Functions.DispArray("daTotals", daTotals)

				'	DMCommon.Debug.MsgBox("", iLanduseColUB)

			End If
		End Sub
		Private Shared Function zzGetOutPgonCaption(iPgonIndex As Integer, iFieldIndex As Integer) As String

			Return TPlServerDB.TextResource.GetText(iFieldIndex, TPlServerDB.enResourceTheme.AcRepParcelLuseCol, 2) & " " & Chr(223 + iPgonIndex).ToString()

		End Function


		Public Shared Sub GetLanduseDataByCol(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, hsRegions As HashSet(Of Integer), ByRef oResDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object, ByRef iLanduseColUB As Integer, ByRef oaCaptions() As System.Object)
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, iRegion <> 0, True)
			Dim sGroupFieldName As String

			TplnProject.GetLusePgon(iTopoPurpose, 0)
			' Dim sAreaFldName As String
			If oLanduseTable IsNot Nothing Then
				'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(oLanduseTable.Rows.Count) & ":" & CStr(88), "05_122q")
			End If

			'  MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_121s")
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & LanduseOrderFieldName


			If oLanduseTable IsNot Nothing AndAlso oLanduseTable.Rows.Count <> 0 Then
				Dim dicLanduses As Dictionary(Of Integer, LanduseCol) = New Dictionary(Of Integer, LanduseCol)()
				Dim iLanduseID As Integer
				Dim iColIndex As Integer = 0
				Dim tLanduseCol As LanduseCol = New LanduseCol()
				For iRowIndex As Integer = 0 To oLanduseTable.Rows.Count - 1
					With oLanduseTable.Rows.Item(iRowIndex)
						iLanduseID = DMCommon.Functions.CIntN(.Item(LanduseIDFieldName))
						If iLanduseID <> 0 AndAlso Not dicLanduses.ContainsKey(iLanduseID) Then
							dicLanduses.Add(iLanduseID, New LanduseCol(iLanduseID, DMCommon.Functions.CStrN(.Item(LanduseNameFieldName)), iColIndex))
							iColIndex += 1
						End If

					End With
				Next
				iLanduseColUB = iColIndex - 1
				ReDim oaCaptions(iLanduseColUB)
				Dim iAreaColIndex As Integer
				Dim iLuseAreaColIndex As Integer

				'	Dim iLegalAreaColIndex As Integer = 4
				Select Case iOptions
					Case enDataOptions.AcadArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 6
							iLuseAreaColIndex = 14
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 10
							iLuseAreaColIndex = 18
						End If
					Case enDataOptions.CalcMergeArea, enDataOptions.Default
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 7
							iLuseAreaColIndex = 15
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 11
							iLuseAreaColIndex = 19
						End If
					Case enDataOptions.CalcMergeArea2
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 8
							iLuseAreaColIndex = 16
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 12
							iLuseAreaColIndex = 20
						End If
					Case enDataOptions.RoundedArea
						If iOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
							iAreaColIndex = 9
							iLuseAreaColIndex = 17
						ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
							iAreaColIndex = 13
							iLuseAreaColIndex = 21
						End If
				End Select

				Dim iBaseColUB As Integer = oLanduseTable.Columns.Count - 1
				Dim iaBaseAcadColumns() As Integer = {0, 1, 4, iAreaColIndex} '  0,1, 4  {0, 1, 4, -1, -1, 3}
				Dim iBaseUB As Integer = iaBaseAcadColumns.GetUpperBound(0)
				Dim iGroupIndex As Integer = 1
				Dim oResultTable As System.Data.DataTable = oLanduseTable.Clone

				iaColumns = iaBaseAcadColumns
				' DMCommon.Functions.DispArray(iaColumns, "iaColumns_1")
				ReDim Preserve iaColumns(iBaseUB + iLanduseColUB + 1)
				' DMCommon.Functions.DispArray(iaColumns, "iaColumns_2")
				'   MessageBox.Show(dicLanduses.Count.ToString() & ":" & iColIndex.ToString(), "04_540")
				For Each tLanduseCol In dicLanduses.Values
					oaCaptions(iLanduseColUB - tLanduseCol.ColumnIndex) = tLanduseCol.LanduseName
					sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)
					If Not oResultTable.Columns.Contains(sGroupFieldName) Then
						oResultTable.Columns.Add(sGroupFieldName, GetType(System.Double))
					End If

					iaColumns(iBaseUB + iGroupIndex) = iBaseColUB + iGroupIndex
					iGroupIndex += 1
				Next
				ReDim oaCaptions(9)
				oaCaptions(0) = "000"
				oaCaptions(1) = "111"
				oaCaptions(2) = "222"
				oaCaptions(3) = "333"
				oaCaptions(4) = "444"
				oaCaptions(5) = "555"
				oaCaptions(6) = "666"
				oaCaptions(7) = "777"
				oaCaptions(8) = "888"
				oaCaptions(9) = "999"



				'If oaCaptions IsNot Nothing Then
				'  DMCommon.Functions.DispArray(iaColumns, "iaColumns_L")
				'   DMCommon.Functions.DispArray(oaCaptions, "01_882x", True)


				'Else
				'   MessageBox.Show("oaCaptions Is Nothing", "01_882xno")
				'End If
				' MessageBox.Show(dicLanduses.Count.ToString & ":" & iColIndex.ToString(), "09_879")
				'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLandusePropTable Is Nothing) & ":" & CStr(moLanduseApprTable Is Nothing) & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_122")

				' 


				'   iaColumns = iaBaseAcadColumns
				''''''''''''''''''''''''    iaBaseAcadColumns(3) = iAreaColIndex
				'''''''''''''''''''''''''    iaBaseAcadColumns(4) = iLuseAreaColIndex

				'' Dim iaAcadColumns(iBaseUB + iLanduseColUB + 1) As Integer
				'''''ReDim iaColumns(iBaseUB + iLanduseColUB + 1)
				'For iIndex As Integer = 0 To iBaseUB
				'   iaColumns(iLanduseColUB + 1 + iIndex) = iaBaseAcadColumns(iIndex)
				'Next


				' 160141 

				Dim oDataView As System.Data.DataView = New System.Data.DataView(oLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)

				'  DMCommon.Debug.MsgBox("12_642", oDataView.Count, oLanduseTable.Rows.Count)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim daLanduseSum(iLanduseColUB + 2) As Double
				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0
				Dim dFragmentArea As Double
				Dim oDataRowView As DataRowView
				Dim oResDataRow As DataRow = Nothing

				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0
				'054 6 810191   Tanya Kurnik
				Dim iPrevParcelOrder As Integer = 0

				Dim iCurrentBlock As Integer
				Dim iCurrentBlockAdd As Integer
				Dim iCurrentParcelOrder As Integer
				Dim dLegalArea As Double
				'   Dim dLuseArea As Double

				' Dim tLanduseCol As LanduseCol

				'  DMCommon.Debug.MsgBox("12_640", oDataView.Count)
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oDataRowView = oDataView.Item(iRowIndex)
					iLanduseID = DMCommon.Functions.CIntN(oDataRowView.Item(LanduseIDFieldName))
					If iLanduseID <> 0 AndAlso dicLanduses.TryGetValue(iLanduseID, tLanduseCol) Then
						iCurrentBlock = DirectCast(oDataRowView.Item(BlockFieldName), Integer)
						iCurrentBlockAdd = DirectCast(oDataRowView.Item(BlockAddFieldName), Integer)
						iCurrentParcelOrder = DirectCast(oDataRowView.Item(ParcelOrderFieldName), Integer)
						dLegalArea = DMCommon.Functions.CDblN(oDataRowView.Item(LegalAreaFieldName))
						dFragmentArea = Math.Round(DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex)), 0)

						'   sGroupFieldName = "Landuse_" & CStr(tLanduseCol.LanduseID)

						If iPrevBlock <> iCurrentBlock OrElse iPrevBlockAdd <> iCurrentBlockAdd OrElse iPrevParcelOrder <> iCurrentParcelOrder Then
							If oResDataRow IsNot Nothing Then
								oResultTable.Rows.Add(oResDataRow)
							End If
							oResDataRow = oResultTable.NewRow()

							oResDataRow.Item(BlockFullFieldName) = oDataRowView.Item(BlockFullFieldName)
							If iCurrentBlockAdd <> 0 Then
								oResDataRow.Item(BlockAddFieldName) = iCurrentBlockAdd
							End If
							oResDataRow.Item(ParcelOrderFieldName) = iCurrentParcelOrder
							oResDataRow.Item(NameFieldName) = oDataRowView.Item(NameFieldName)
							oResDataRow.Item(LegalAreaFieldName) = dLegalArea ' oDataRowView.Item(msLegalAreaFieldName)




							daLanduseSum(iLanduseColUB + 2) += dLegalArea ' DMCommon.Functions.CDblN(oDataRowView.Item(msLegalAreaFieldName))
							dSum2 += dLegalArea ' DMCommon.Functions.CDblN(oDataRowView.Item(msLegalAreaFieldName))
							dSum1 += dFragmentArea 'DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
							iPrevBlock = iCurrentBlock
							iPrevBlockAdd = iCurrentBlockAdd

							iPrevParcelOrder = iCurrentParcelOrder
						End If



						sGroupFieldName = "Landuse_" & CStr(iLanduseID)
						oResDataRow.Item(sGroupFieldName) = dFragmentArea 'DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						'   dFragmentArea = DMCommon.Functions.CDblN(oDataRowView.Item(iLuseAreaColIndex))
						daLanduseSum(iLanduseColUB - tLanduseCol.ColumnIndex) += dFragmentArea
						daLanduseSum(iLanduseColUB + 1) += dFragmentArea
						oResDataRow.Item(iAreaColIndex) = DMCommon.Functions.CDblN(oResDataRow.Item(iAreaColIndex)) + dFragmentArea ' oDataRowView.Item(iAreaColIndex)
						'    DMCommon.ExcelLogG.SetNextValueInRow(i, 0, iCurrentBlock, oDataRowView.Item(msNameFieldName), dLegalArea, dFragmentArea, sGroupFieldName)
					End If
				Next

				If oResDataRow IsNot Nothing Then
					oResultTable.Rows.Add(oResDataRow)
				End If
				oResDataView = New DataView(oResultTable)
				'  MessageBox.Show(dSum0.ToString() & vbCrLf & dSum1.ToString() & vbCrLf & dSum2.ToString(), "19_006")
				'  DMCommon.Functions.DispArray(daLanduseSum, "dLanduseSum")
				ReDim oaTotals(daLanduseSum.GetUpperBound(0))
				For iIndex As Integer = 0 To daLanduseSum.GetUpperBound(0)
					oaTotals(iIndex) = daLanduseSum(iIndex)
					' oaTotals(iIndex) = 1000 + iIndex * 10000.0

				Next
				'      DMCommon.Functions.DispArray(oaTotals, "oaTotals", True)
				'oaTotals(0) = dSum0
				'oaTotals(1) = dSum1
				'oaTotals(2) = dSum2
				'oaTotals(3) = 1952.0

				'oaTotals(0) = 100.1
				'oaTotals(1) = 200.1
				'oaTotals(2) = 300.1
				'  oaTotals(3) = "A"
				'oaTotals(0) = "A"
				'oaTotals(1) = "B"
				'oaTotals(2) = "C"
				'oaTotals(3) = "D"
			End If
		End Sub

		Public Shared Sub GetLanduseList(ByVal iStatus As DMAcadExt.enTopoPurpose, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer)
			MessageBox.Show(iStatus.ToString() & ":" & CStr(moLandusePropTable Is Nothing) & ":" & CStr(moLanduseApprTable Is Nothing), "17_120")
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iStatus, False)
			MessageBox.Show(CStr(oLanduseTable.Rows.Count) & ":" & CStr(moLanduseApprTable Is Nothing), "17_121")
			' Dim sAreaFldName As String
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & LanduseOrderFieldName

			If oLanduseTable IsNot Nothing Then
				Dim iaAcadColumns() As Integer = {4, 6}
				iaColumns = iaAcadColumns
				oDataView = New System.Data.DataView(oLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
			End If
		End Sub

		Public Shared Sub CreatePolygonTables(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
				If baOverlayArray(iOverlayIndex) Then
					moaPolygonTable(iOverlayIndex) = zzCreatePolygonTable()
				End If
			Next
			''''''''	DispPgonTables()
		End Sub

		Public Shared Sub PaintExceptions(tColorScheme As DMAcadExt.ColorScheme, Optional dScale As Double = 1.0)
			Dim oDataView As DataView = New DataView(moMainDataTable, msDeviationFieldName & " > 0", "", DataViewRowState.CurrentRows)
			Dim oRow As DataRowView
			Dim oParcel As TplnParcel
			Dim iTopoID As Integer
			Dim bCurrentLayerOK As Boolean = True
			Dim oPaintLayer As DMAcadExt.AcadLayerDef = TopoManager.TPlanGraph.TplnProject.PaintTempLayerDef
			bCurrentLayerOK = DMAcadExt.AcadTransaction.CreateLayer(oPaintLayer, True)
			If bCurrentLayerOK Then
				bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(oPaintLayer, True, False, False, False)
			End If


			If bCurrentLayerOK Then
				For iRowIndex As Integer = 0 To oDataView.Count - 1
					oRow = oDataView.Item(iRowIndex)
					iTopoID = DirectCast(oRow.Item(TopoReader.msTopoIDFldName), Integer)
					oParcel = TplnProject.GetParcel(iTopoID)
					oParcel.PaintColorScheme(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, True)
				Next
			End If

		End Sub
		Public Shared Function GetParcelData(tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As ParcelData
			Dim tAddBlockAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing
			Dim saBlockAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(tCentroidAcObjID, True, False, miaBlockAttribIndex)
			Dim saAddBlockAttribText() As String = Nothing

			tAddBlockAcObjID = DMAcadExt.AcadBlock.GetAddBlockObjID(tCentroidAcObjID)
			'   MessageBox.Show(tCentroidAcObjID.ToString() & vbCrLf & tAddBlockAcObjID.ToString(), "11_439")
			DMCommon.Functions.DispArray("01_452su", miaAddBlockAttribIndex)
			If Not tAddBlockAcObjID.IsNull Then
				saAddBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(tAddBlockAcObjID, True, False, miaAddBlockAttribIndex)
			End If




			'   DMCommon.Functions.DispArray(saBlockAttribText, "01_443s", True)
			'    DMCommon.Functions.DispArray(saAddBlockAttribText, "01_443s", True)

			If saBlockAttribText IsNot Nothing Then
				Return New ParcelData(saBlockAttribText, saAddBlockAttribText)
			Else
				Return Nothing
			End If
		End Function
		Private Shared Function zzCreatePolygonTable() As System.Data.DataTable
			Dim oPolygonTable As System.Data.DataTable = New Data.DataTable("Polygons")
			With oPolygonTable.Columns
				.Add(BlockFullFieldName, GetType(System.String))                '0

				.Add(NameFieldName, GetType(System.String))                  '1
				.Add(msLotNameFieldName, GetType(System.String))               '2
				.Add(LanduseIDFieldName, GetType(System.Int32))              '3
				.Add(LanduseNameFieldName, GetType(System.String))           '4

				.Add(TopoReader.msAreaFldName, GetType(System.Double))         '5
				.Add(msParcelDifAreaFieldName, GetType(System.Double))          '6

				.Add(msLotAreaFieldName, GetType(System.Double))               '7
				.Add(msLotDifAreaFieldName, GetType(System.Double))                      '8
				.Add(PgonAreaFieldName, GetType(System.Double))           '9

				'	.Add(msPlanStateFieldName, GetType(System.Int32))				
				.Add(msPlanStateTextFieldName, GetType(System.String))         '10
				.Add(TopoReader.msCentroidXFldName, GetType(System.Double)) '11
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double)) '12


				.Add(msInPlanFieldName, System.Type.GetType("System.Boolean"))                '13
				'		.Add(msInPlanTextFieldName, GetType(System.STRING))				'13

				.Add(TopoReader.msPerimeterFldName, GetType(System.Double)) '14		

				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))        '15
				.Add(TopoReader.msParcelTopoIDFldName, GetType(System.Int32))        '16
				.Add(TopoReader.msLotTopoIDFldName, GetType(System.Int32))        '17

				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId)) '18
				.Add(BlockFieldName, GetType(System.Int32))                  '19
				.Add(BlockAddFieldName, GetType(System.Int32))               '20
				.Add(ParcelOrderFieldName, GetType(System.Int32))            '21
				.Add(msLotOrderFieldName, GetType(System.Int32))               '22

			End With
			Return oPolygonTable
		End Function
		Public Shared Sub ClearDataTable()
			If moMainDataTable IsNot Nothing Then
				moMainDataTable.Rows.Clear()
			End If
		End Sub
		Public Shared Sub CalculateLotsContent(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim oPolygonTable As System.Data.DataTable = zzGetPolygonTable(iOverlayIndex)
			Dim oDataView As DataView
			Dim oDataRowView As DataRowView
			Dim iCurrentBlock As Integer = System.Int32.MinValue
			Dim iCurrentBlockAdd As Integer = 0

			Dim sCurrentLotName As String = String.Empty
			Dim iBlock As Integer
			Dim iBlockAdd As Integer

			Dim sLotName As String = String.Empty
			Dim iTopoID As Integer
			Dim oParcel As TplnParcel
			Dim sParcelName As String
			Dim oNumerationPair As NumerationPair = Nothing
			Dim sRes As String
			Dim iComplexType As NumerationPair.enComplexType
			Dim sFilter As String = msInPlanFieldName & " = True"
			'  Dim sFilter As String = "(" & msBlockFieldName & "=30354) AND (" & msInPlanFieldName & " = True)"

			Dim sSort As String = msLotOrderFieldName & "," & BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If oPolygonTable IsNot Nothing Then
				oDataView = New DataView(oPolygonTable, sFilter, sSort, DataViewRowState.CurrentRows)
				'   System.Windows.Forms.MessageBox.Show(oDataView.Count.ToString(), "09_548")

				zzCreateLotsContentTable()
				Dim oNewRow As System.Data.DataRow
				Try
					For iIndex As Integer = 0 To oDataView.Count - 1
						oDataRowView = oDataView.Item(iIndex)
						sLotName = DMCommon.Functions.CStrN(oDataRowView.Item(msLotNameFieldName))
						iBlock = DMCommon.Functions.CIntN(oDataRowView.Item(BlockFieldName))
						iBlockAdd = DMCommon.Functions.CIntN(oDataRowView.Item(BlockAddFieldName))
						If sLotName <> sCurrentLotName OrElse iBlock <> iCurrentBlock OrElse iBlockAdd <> iCurrentBlockAdd OrElse iBlock = System.Int32.MinValue Then
							If oNumerationPair IsNot Nothing Then
								oNewRow = moLotsContentTable.NewRow()
								oNewRow.Item(msLotNameFieldName) = sCurrentLotName
								oNewRow.Item(BlockFullFieldName) = TplnBlock.GetBlockName(iCurrentBlock, iCurrentBlockAdd)


								sRes = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire)
								If sRes.Length <> 0 Then
									oNewRow.Item(msParcelEntireFieldName) = sRes

								End If
								sRes = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial)
								If sRes.Length <> 0 Then
									oNewRow.Item(msParcelPartialFieldName) = sRes
								End If
								oNewRow.Item(BlockFieldName) = iCurrentBlock
								oNewRow.Item(BlockAddFieldName) = iCurrentBlockAdd

								moLotsContentTable.Rows.Add(oNewRow)


							End If
							sCurrentLotName = sLotName
							iCurrentBlock = iBlock
							oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
						End If
						iTopoID = DirectCast(oDataRowView.Item(TopoReader.msTopoIDFldName), Integer)
						oParcel = TplnProject.GetParcelByPgon(iTopoID, iOverlayIndex)
						sParcelName = DMCommon.Functions.CStrN(oDataRowView.Item(NameFieldName))
						' If oParcel IsNot Nothing AndAlso oParcel.PlanState <> NumerationPair.enComplexType.Undefined Then
						iComplexType = oParcel.SingleLotState(iOverlayIndex)
						If sParcelName.Length <> 0 AndAlso iComplexType <> NumerationPair.enComplexType.Undefined Then
							oNumerationPair.AddComplexNum(sParcelName, iComplexType)
						End If
					Next

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - CalculateLotsContent_1a1")
				End Try
				Dim oBlock As TplnBlock
				If oNumerationPair IsNot Nothing AndAlso oNumerationPair.Count <> 0 Then

					oNewRow = moLotsContentTable.NewRow()
					oNewRow.Item(msLotNameFieldName) = sCurrentLotName
					oNewRow.Item(BlockFullFieldName) = TplnBlock.GetBlockName(iCurrentBlock, iCurrentBlockAdd)
					oNewRow.Item(BlockFieldName) = iCurrentBlock
					oNewRow.Item(BlockAddFieldName) = iCurrentBlockAdd


					oBlock = TplnProject.GetBlock(iCurrentBlock, iCurrentBlockAdd)
					If oBlock IsNot Nothing AndAlso oBlock.BlockStatusName.Length <> 0 Then
						''''''''''''''	oNewRow.Item(msBlockStatusNameFieldName) = oBlock.BlockStatusName
					End If
					oNewRow.Item(msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire)
					oNewRow.Item(msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial)
					moLotsContentTable.Rows.Add(oNewRow)
				End If
			Else
				''Topology Parcels isn't opened
				'' System.Windows.Forms.MessageBox.Show("!!!DataTable Is Nothing", "CalculateBlocks")
			End If

		End Sub
		Public Shared Function CalculateContentBlocks(iOverlayIndex As DMAcadExt.enOverlayIndex, oParcelDataTable As System.Data.DataTable) As System.Data.DataTable

			Dim oDataView As DataView
			Dim oDataRowView As DataRowView
			Dim iCurrentBlock As Integer = [Int32].MinValue
			Dim iCurrentBlockAdd As Integer

			Dim iBlock As Integer
			Dim iBlockAdd As Integer

			Dim iTopoID As Integer
			Dim oParcel As TplnParcel
			Dim oNumerationPair As NumerationPair = Nothing
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(CType(NumerationPair.enComplexType.Undefined, Integer))
			Dim dSumArea As Double = 0.0, dSumLegalArea As Double = 0.0
			Dim dInPlanCalcAreaAppr As Double = 0.0, dInPlanCalcAreaProp As Double = 0.0
			Dim iParcelCount As Integer = 0
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If oParcelDataTable IsNot Nothing Then
				Dim oBlockTable As System.Data.DataTable = CreateBlockTable()


				oDataView = New DataView(oParcelDataTable, sFilter, BlockFieldName & "," & BlockAddFieldName, DataViewRowState.CurrentRows)
				'    DMCommon.Debug.MsgBox("09_772", True, oDataView.Count)
				Try
					'  System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oDataView.Count) & vbCrLf & sFilter, "04_013")
					For iIndex As Integer = 0 To oDataView.Count - 1
						oDataRowView = oDataView.Item(iIndex)
						With oDataRowView
							iBlock = DirectCast(.Item(BlockFieldName), Integer)
							iBlockAdd = DirectCast(.Item(BlockAddFieldName), Integer)

							If iBlock <> iCurrentBlock OrElse iBlockAdd <> iCurrentBlockAdd OrElse iBlock = System.Int32.MinValue Then
								If oNumerationPair IsNot Nothing Then
									zzAddBlockToTable(oBlockTable, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
								End If
								iParcelCount = 0
								iCurrentBlock = iBlock
								iCurrentBlockAdd = iBlockAdd
								oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
							End If
							iTopoID = DirectCast(.Item(TopoReader.msTopoIDFldName), Integer)
							oParcel = TplnProject.GetParcel(iTopoID)
							iParcelCount += 1
							dSumArea += DMCommon.Functions.CDblN(.Item(TopoReader.msAreaFldName))
							dSumLegalArea += DMCommon.Functions.CDblN(.Item(LegalAreaFieldName))
							dInPlanCalcAreaAppr += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaApprMergeFieldName))
							dInPlanCalcAreaProp += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaPropMergeFieldName))
							' If oParcel IsNot Nothing AndAlso oParcel.PlanState <> NumerationPair.enComplexType.Undefined Then
						End With
						If oParcel IsNot Nothing Then
							' TplnProject.WriteMessageBox("PP: " & oParcel.BlockFull & "," & oParcel.Name & "==" & oParcel.PlanState(iOverlayIndex).ToString(), "")
							oNumerationPair.AddComplexNum(oParcel.Name, oParcel.PlanState(iOverlayIndex))
						End If
					Next

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "CalculateBlocks_1")
				End Try

				If oNumerationPair IsNot Nothing AndAlso oNumerationPair.Count <> 0 Then
					zzAddBlockToTable(oBlockTable, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
				End If
				Return oBlockTable
			Else
				Return Nothing
				''Topology Parcels isn't opened
				'' System.Windows.Forms.MessageBox.Show("!!!DataTable Is Nothing", "CalculateBlocks")
			End If



		End Function

		Public Shared Sub CalculateBlocks(iOverlayIndex As DMAcadExt.enOverlayIndex)

			Dim oDataView As DataView
			Dim oDataRowView As DataRowView
			Dim iCurrentBlock As Integer = [Int32].MinValue
			Dim iCurrentBlockAdd As Integer

			Dim iBlock As Integer
			Dim iBlockAdd As Integer

			Dim iTopoID As Integer
			Dim oParcel As TplnParcel
			Dim oNumerationPair As NumerationPair = Nothing
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(CType(NumerationPair.enComplexType.Undefined, Integer))
			Dim dSumArea As Double = 0.0, dSumLegalArea As Double = 0.0
			Dim dInPlanCalcAreaAppr As Double = 0.0, dInPlanCalcAreaProp As Double = 0.0
			Dim iParcelCount As Integer = 0
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If moMainDataTable IsNot Nothing Then
				zzCreateBlockTable(iOverlayIndex)
				'   
				oDataView = New DataView(moMainDataTable, sFilter, BlockFieldName & "," & BlockAddFieldName, DataViewRowState.CurrentRows)

				Try

					For iIndex As Integer = 0 To oDataView.Count - 1
						oDataRowView = oDataView.Item(iIndex)
						With oDataRowView
							iBlock = DirectCast(.Item(BlockFieldName), Integer)
							iBlockAdd = DirectCast(.Item(BlockAddFieldName), Integer)

							If iBlock <> iCurrentBlock OrElse iBlockAdd <> iCurrentBlockAdd OrElse iBlock = System.Int32.MinValue Then
								If oNumerationPair IsNot Nothing Then
									zzAddBlockToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
								End If
								iParcelCount = 0
								iCurrentBlock = iBlock
								iCurrentBlockAdd = iBlockAdd
								oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
							End If
							iTopoID = DirectCast(.Item(TopoReader.msTopoIDFldName), Integer)
							oParcel = TplnProject.GetParcel(iTopoID)
							iParcelCount += 1
							dSumArea += DMCommon.Functions.CDblN(.Item(TopoReader.msAreaFldName))
							dSumLegalArea += DirectCast(.Item(LegalAreaFieldName), Double)
							dInPlanCalcAreaAppr += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaApprMergeFieldName))
							dInPlanCalcAreaProp += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaPropMergeFieldName))
							' If oParcel IsNot Nothing AndAlso oParcel.PlanState <> NumerationPair.enComplexType.Undefined Then
						End With
						If oParcel IsNot Nothing Then
							' TplnProject.WriteMessageBox("PP: " & oParcel.BlockFull & "," & oParcel.Name & "==" & oParcel.PlanState(iOverlayIndex).ToString(), "")
							oNumerationPair.AddComplexNum(oParcel.Name, oParcel.PlanState(iOverlayIndex))
						End If
					Next

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "CalculateBlocks_1")
				End Try

				If oNumerationPair IsNot Nothing AndAlso oNumerationPair.Count <> 0 Then
					zzAddBlockToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
				End If
			Else
				''Topology Parcels isn't opened
				'' System.Windows.Forms.MessageBox.Show("!!!DataTable Is Nothing", "CalculateBlocks")
			End If



		End Sub
		Public Shared Sub CalculateRegionBlocks(iOverlayIndex As DMAcadExt.enOverlayIndex, iRegionNo As Integer)

			Dim oDataView As DataView
			Dim oDataRowView As DataRowView
			Dim iCurrentBlock As Integer = [Int32].MinValue
			Dim iCurrentBlockAdd As Integer

			Dim iBlock As Integer
			Dim iBlockAdd As Integer

			Dim iTopoID As Integer
			Dim oParcel As TplnParcel
			Dim oNumerationPair As NumerationPair = Nothing
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(CType(NumerationPair.enComplexType.Undefined, Integer))
			Dim dSumArea As Double = 0.0, dSumLegalArea As Double = 0.0
			Dim dInPlanCalcAreaAppr As Double = 0.0, dInPlanCalcAreaProp As Double = 0.0
			Dim iParcelCount As Integer = 0
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If moMainDataTable IsNot Nothing Then


				oDataView = New DataView(moMainDataTable, "", sSort, DataViewRowState.CurrentRows)
				'DMCommon.Debug.MsgBox("04_785", moMainDataTable.Rows.Count, oDataView.Count)
				zzCreateBlockRegionTable()
				Try

					For iIndex As Integer = 0 To oDataView.Count - 1
						oDataRowView = oDataView.Item(iIndex)
						With oDataRowView
							iTopoID = DirectCast(.Item(TopoReader.msTopoIDFldName), Integer)
							oParcel = TplnProject.GetParcel(iTopoID)
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!RegionRel", iRegionNo, oParcel.RegionRelation(iOverlayIndex, iRegionNo))
							If oParcel IsNot Nothing AndAlso oParcel.RegionRelation(iOverlayIndex, iRegionNo) = NumerationPair.enComplexType.Entire OrElse oParcel.RegionRelation(iOverlayIndex, iRegionNo) = NumerationPair.enComplexType.Partial Then



								iBlock = DirectCast(.Item(BlockFieldName), Integer)
								iBlockAdd = DirectCast(.Item(BlockAddFieldName), Integer)

								If iBlock <> iCurrentBlock OrElse iBlockAdd <> iCurrentBlockAdd OrElse iBlock = System.Int32.MinValue Then
									If oNumerationPair IsNot Nothing Then
										zzAddBlockRegionToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
									End If
									iParcelCount = 0
									iCurrentBlock = iBlock
									iCurrentBlockAdd = iBlockAdd
									oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
								End If


								iParcelCount += 1
								dSumArea += DMCommon.Functions.CDblN(.Item(TopoReader.msAreaFldName))
								dSumLegalArea += DirectCast(.Item(LegalAreaFieldName), Double)
								dInPlanCalcAreaAppr += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaApprMergeFieldName))
								dInPlanCalcAreaProp += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaPropMergeFieldName))

								' If oParcel IsNot Nothing AndAlso oParcel.PlanState <> NumerationPair.enComplexType.Undefined Then

								If oParcel IsNot Nothing Then
									' TplnProject.WriteMessageBox("PP: " & oParcel.BlockFull & "," & oParcel.Name & "==" & oParcel.PlanState(iOverlayIndex).ToString(), "")
									oNumerationPair.AddComplexNum(oParcel.Name, oParcel.RegionRelation(iOverlayIndex, iRegionNo))
								End If
							End If
						End With
					Next

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "CalculateBlocks_1")
				End Try

				If oNumerationPair IsNot Nothing AndAlso oNumerationPair.Count <> 0 Then
					zzAddBlockRegionToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
				End If
			Else
				''Topology Parcels isn't opened
				'' System.Windows.Forms.MessageBox.Show("!!!DataTable Is Nothing", "CalculateBlocks")
			End If



		End Sub
		Public Shared Sub CalculateRegionBlocks(iOverlayIndex As DMAcadExt.enOverlayIndex)

			Dim oDataView As DataView
			Dim oDataRowView As DataRowView
			Dim iCurrentBlock As Integer = [Int32].MinValue
			Dim iCurrentBlockAdd As Integer

			Dim iBlock As Integer
			Dim iBlockAdd As Integer

			Dim iTopoID As Integer
			Dim oParcel As TplnParcel
			Dim oNumerationPair As NumerationPair = Nothing
			Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(CType(NumerationPair.enComplexType.Undefined, Integer))
			Dim dSumArea As Double = 0.0, dSumLegalArea As Double = 0.0
			Dim dInPlanCalcAreaAppr As Double = 0.0, dInPlanCalcAreaProp As Double = 0.0
			Dim iParcelCount As Integer = 0
			Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If moMainRegionDataTable IsNot Nothing Then


				oDataView = New DataView(moMainRegionDataTable, "", BlockFieldName & "," & BlockAddFieldName, DataViewRowState.CurrentRows)
				zzCreateBlockRegionTable()
				Try

					For iIndex As Integer = 0 To oDataView.Count - 1
						oDataRowView = oDataView.Item(iIndex)
						With oDataRowView
							iBlock = DirectCast(.Item(BlockFieldName), Integer)
							iBlockAdd = DirectCast(.Item(BlockAddFieldName), Integer)

							If iBlock <> iCurrentBlock OrElse iBlockAdd <> iCurrentBlockAdd OrElse iBlock = System.Int32.MinValue Then
								If oNumerationPair IsNot Nothing Then
									zzAddBlockRegionToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
								End If
								iParcelCount = 0
								iCurrentBlock = iBlock
								iCurrentBlockAdd = iBlockAdd
								oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
							End If
							iTopoID = DirectCast(.Item(TopoReader.msTopoIDFldName), Integer)
							oParcel = TplnProject.GetParcel(iTopoID)
							iParcelCount += 1
							dSumArea += DMCommon.Functions.CDblN(.Item(TopoReader.msAreaFldName))
							dSumLegalArea += DirectCast(.Item(LegalAreaFieldName), Double)
							dInPlanCalcAreaAppr += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaApprMergeFieldName))
							dInPlanCalcAreaProp += DMCommon.Functions.CDblN(.Item(msInPlanCalcAreaPropMergeFieldName))
						End With
						If oParcel IsNot Nothing Then
							oNumerationPair.AddComplexNum(oParcel.Name, oParcel.PlanState(iOverlayIndex))
						End If
					Next

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "CalculateBlocks_1")
				End Try

				If oNumerationPair IsNot Nothing AndAlso oNumerationPair.Count <> 0 Then
					zzAddBlockRegionToTable(iOverlayIndex, iCurrentBlock, iCurrentBlockAdd, iParcelCount, oNumerationPair, dSumArea, dSumLegalArea, dInPlanCalcAreaAppr, dInPlanCalcAreaProp)
				End If
			Else
				''Topology Parcels isn't opened
				'' System.Windows.Forms.MessageBox.Show("!!!DataTable Is Nothing", "CalculateBlocks")
			End If



		End Sub
		Private Shared Sub zzAddBlockToTable(ByRef oaBlockTable As System.Data.DataTable, ByVal iCurrentBlock As Integer, ByVal iCurrentBlockAdd As Integer, ByVal iParcelCount As Integer, ByVal oNumerationPair As NumerationPair, ByVal dSumArea As Double, ByVal dSumLegalArea As Double, ByVal dInPlanCalcAreaAppr As Double, ByVal dInPlanCalcAreaProp As Double)
			Const iMaxLength As Integer = 16
			Dim oNewRow As System.Data.DataRow
			Dim oBlock As TplnBlock
			Try
				oNewRow = oaBlockTable.NewRow()
				With oNewRow
					oNewRow.Item(BlockFullFieldName) = TplnBlock.GetBlockName(iCurrentBlock, iCurrentBlockAdd)


					oBlock = TplnProject.GetBlock(iCurrentBlock, iCurrentBlockAdd)
					If oBlock IsNot Nothing Then
						' System.Windows.Forms.MessageBox.Show(iParcelCount.ToString() & vbCrLf & oBlock.Parcels.Count.ToString(), "07_376")
						.Item(BlockStatusFieldName) = oBlock.BlockStatus ''''''''''''''''''''''''''''''''''''' Entry01
						.Item(BlockStatusNameFieldName) = oBlock.BlockStatusName

						.Item(TopoReader.msAreaFldName) = oBlock.AcadArea(False)
						.Item(LegalAreaFieldName) = oBlock.LegalArea
						'   .Item(ParcelCountFieldName) = iParcelCount

						.Item(TopoReader.msCentroidXFldName) = oBlock.CentroidX
						.Item(TopoReader.msCentroidYFldName) = oBlock.CentroidY
						.Item(TopoReader.msPerimeterFldName) = oBlock.Perimiter
						.Item(TopoReader.msTopoIDFldName) = oBlock.TopoID
						.Item(TopoReader.msAcObjIDFldName) = oBlock.CentroidAcObjID ''''''''''''''.OldIdPtr.ToInt64()
					End If
					.Item(msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
					.Item(msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)
					.Item(TopoReader.msSumPgonAreaFldName) = dSumArea
					.Item(TopoReader.msSumLegalAreaFldName) = dSumLegalArea
					.Item(msInPlanCalcAreaApprMergeFieldName) = dInPlanCalcAreaAppr
					.Item(msInPlanCalcAreaPropMergeFieldName) = dInPlanCalcAreaProp
					.Item(BlockFieldName) = iCurrentBlock
					.Item(BlockAddFieldName) = iCurrentBlockAdd
				End With
				oaBlockTable.Rows.Add(oNewRow)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddBlockToTable")
			End Try

		End Sub
		Private Shared Sub zzAddBlockToTable(iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iCurrentBlock As Integer, ByVal iCurrentBlockAdd As Integer, ByVal iParcelCount As Integer, ByVal oNumerationPair As NumerationPair, ByVal dSumArea As Double, ByVal dSumLegalArea As Double, ByVal dInPlanCalcAreaAppr As Double, ByVal dInPlanCalcAreaProp As Double)
			Const iMaxLength As Integer = 16
			Dim oNewRow As System.Data.DataRow
			Dim oBlock As TplnBlock
			Try
				oNewRow = moaBlockTable(iOverlayIndex).NewRow()
				With oNewRow
					oNewRow.Item(BlockFullFieldName) = TplnBlock.GetBlockName(iCurrentBlock, iCurrentBlockAdd)


					oBlock = TplnProject.GetBlock(iCurrentBlock, iCurrentBlockAdd)
					If oBlock IsNot Nothing Then
						' System.Windows.Forms.MessageBox.Show(iParcelCount.ToString() & vbCrLf & oBlock.Parcels.Count.ToString(), "07_376")
						.Item(BlockStatusFieldName) = oBlock.BlockStatus ''''''''''''''''''''''''''''''''''''' Entry01
						.Item(BlockStatusNameFieldName) = oBlock.BlockStatusName

						.Item(TopoReader.msAreaFldName) = oBlock.AcadArea(False)
						.Item(LegalAreaFieldName) = oBlock.LegalArea
						'   .Item(ParcelCountFieldName) = iParcelCount

						.Item(TopoReader.msCentroidXFldName) = oBlock.CentroidX
						.Item(TopoReader.msCentroidYFldName) = oBlock.CentroidY
						.Item(TopoReader.msPerimeterFldName) = oBlock.Perimiter
						.Item(TopoReader.msTopoIDFldName) = oBlock.TopoID
						.Item(TopoReader.msAcObjIDFldName) = oBlock.CentroidAcObjID ''''''''''''''.OldIdPtr.ToInt64()
					End If
					.Item(msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
					.Item(msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)
					.Item(TopoReader.msSumPgonAreaFldName) = dSumArea
					.Item(TopoReader.msSumLegalAreaFldName) = dSumLegalArea
					.Item(msInPlanCalcAreaApprMergeFieldName) = dInPlanCalcAreaAppr
					.Item(msInPlanCalcAreaPropMergeFieldName) = dInPlanCalcAreaProp
					.Item(BlockFieldName) = iCurrentBlock
					.Item(BlockAddFieldName) = iCurrentBlockAdd
				End With
				moaBlockTable(iOverlayIndex).Rows.Add(oNewRow)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddBlockToTable")
			End Try

		End Sub
		Private Shared Sub zzAddBlockRegionToTable(iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iCurrentBlock As Integer, ByVal iCurrentBlockAdd As Integer, ByVal iParcelCount As Integer, ByVal oNumerationPair As NumerationPair, ByVal dSumArea As Double, ByVal dSumLegalArea As Double, ByVal dInPlanCalcAreaAppr As Double, ByVal dInPlanCalcAreaProp As Double)
			Const iMaxLength As Integer = 16
			Dim oNewRow As System.Data.DataRow
			Dim oBlock As TplnBlock
			Try
				oNewRow = moBlockRegionTable.NewRow()
				With oNewRow
					oNewRow.Item(BlockFullFieldName) = TplnBlock.GetBlockName(iCurrentBlock, iCurrentBlockAdd)


					oBlock = TplnProject.GetBlock(iCurrentBlock, iCurrentBlockAdd)
					If oBlock IsNot Nothing Then
						' System.Windows.Forms.MessageBox.Show(iParcelCount.ToString() & vbCrLf & oBlock.Parcels.Count.ToString(), "07_376")
						.Item(BlockStatusFieldName) = oBlock.BlockStatus ''''''''''''''''''''''''''''''''''''' Entry01
						.Item(BlockStatusNameFieldName) = oBlock.BlockStatusName

						.Item(TopoReader.msAreaFldName) = oBlock.AcadArea(False)
						.Item(LegalAreaFieldName) = oBlock.LegalArea
						'   .Item(ParcelCountFieldName) = iParcelCount

						.Item(TopoReader.msCentroidXFldName) = oBlock.CentroidX
						.Item(TopoReader.msCentroidYFldName) = oBlock.CentroidY
						.Item(TopoReader.msPerimeterFldName) = oBlock.Perimiter
						.Item(TopoReader.msTopoIDFldName) = oBlock.TopoID
						.Item(TopoReader.msAcObjIDFldName) = oBlock.CentroidAcObjID ''''''''''''''.OldIdPtr.ToInt64()
					End If
					.Item(msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
					.Item(msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)
					.Item(TopoReader.msSumPgonAreaFldName) = dSumArea
					.Item(TopoReader.msSumLegalAreaFldName) = dSumLegalArea
					.Item(msInPlanCalcAreaApprMergeFieldName) = dInPlanCalcAreaAppr
					.Item(msInPlanCalcAreaPropMergeFieldName) = dInPlanCalcAreaProp
					.Item(BlockFieldName) = iCurrentBlock
					.Item(BlockAddFieldName) = iCurrentBlockAdd
				End With
				moBlockRegionTable.Rows.Add(oNewRow)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddBlockToTable")
			End Try

		End Sub
		Public Shared Sub CreateLanduseTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, bRegion As Boolean)
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, bRegion)

			If oLanduseTable Is Nothing Then
				oLanduseTable = New Data.DataTable("Landuses")
				With oLanduseTable.Columns
					.Add(BlockFullFieldName, GetType(System.String))                   '0
					'   wwwww()
					.Add(NameFieldName, GetType(System.String))                     '1
					.Add(LanduseIDFieldName, GetType(System.Int32))                 '2
					.Add(LanduseNameFieldName, GetType(System.String))              '3
					.Add(LegalAreaFieldName, GetType(System.Double))             '4
					.Add(TopoReader.msAreaFldName, GetType(System.Double))            '5






					.Add(msInPlanAreaMergeFieldName, GetType(System.Double))       '6
					.Add(msInPlanCalcAreaMergeFieldName, GetType(System.Double))      '7
					.Add(msInPlanCalcArea2MergeFieldName, GetType(System.Double))  '8
					.Add(msInPlanRoundedAreaMergeFieldName, GetType(System.Double))   '9

					.Add(msInPlanAreaFDO_OverlayFieldName, GetType(System.Double))       '10  
					.Add(msInPlanCalcAreaFDO_OverlayFieldName, GetType(System.Double))      '11   
					.Add(msInPlanCalcArea2FDO_OverlayFieldName, GetType(System.Double))  '12  
					.Add(msInPlanRoundedAreaFDO_OverlayFieldName, GetType(System.Double))   '13  



					.Add(msLUseInPlanAreaMergeFieldName, GetType(System.Double))          '14	'6
					.Add(msLUseInPlanCalcAreaMergeFieldName, GetType(System.Double))   '15		'7
					.Add(msLUseInPlanCalcArea2MergeFieldName, GetType(System.Double))  '16 '8
					.Add(msLUseInPlanRoundedAreaMergeFieldName, GetType(System.Double))  '17	'9

					.Add(msLUseInPlanAreaFDO_OverlayFieldName, GetType(System.Double))          '18	'10  6
					.Add(msLUseInPlanCalcAreaFDO_OverlayFieldName, GetType(System.Double))   '19	'11  7
					.Add(msLUseInPlanCalcArea2FDO_OverlayFieldName, GetType(System.Double))  '20 '12  8
					.Add(msLUseInPlanRoundedAreaFDO_OverlayFieldName, GetType(System.Double))  '21	'13  9
					.Add(BlockFieldName, GetType(System.Int32))                     '0
					.Add(BlockAddFieldName, GetType(System.Int32))
					.Add(ParcelOrderFieldName, GetType(System.Int32))   '22				'14  10
					.Add(LanduseOrderFieldName, GetType(System.Int32))  '23				'15  11
				End With
			Else
				oLanduseTable.Clear()
			End If

			If iTopoPurpose = enTopoPurpose.Approved Then
				If bRegion Then
					moLanduseRegionApprTable = oLanduseTable
				Else
					moLanduseApprTable = oLanduseTable
				End If

			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then

				If bRegion Then
					moLanduseRegionPropTable = oLanduseTable
				Else
					moLandusePropTable = oLanduseTable
				End If



			End If
		End Sub
		Public Shared Sub DisposeLanduseTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, bRegion As Boolean)
			Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, bRegion)
			If oLanduseTable IsNot Nothing Then
				oLanduseTable.Dispose()
				oLanduseTable = Nothing
			End If

		End Sub


		Public Shared Sub CheckLegalArea()
			If mlstMissingLegalAreaPoints IsNot Nothing AndAlso mlstMissingLegalAreaPoints.Count <> 0 Then
				If mbHasLegalArea Then
					For Each oPoint As DMAcadExt.TPlnPoint In mlstMissingLegalAreaPoints
						DMAcadExt.AppMessages.AddMessage(True, oPoint, "", "Missing Legal Area", False, ParcelData.MapThemeID, 22)
					Next
				End If
				mlstMissingLegalAreaPoints.Clear()
			End If
		End Sub
		Public Shared ReadOnly Property MainDataTable() As System.Data.DataTable
			Get
				Return moMainDataTable
			End Get
		End Property
		Public Shared ReadOnly Property MainRegionDataTable() As System.Data.DataTable
			Get
				Return moMainRegionDataTable
			End Get
		End Property
		Public Shared ReadOnly Property MainHiddenColumns() As Dictionary(Of Integer, Integer)
			Get
				Return moMainHiddenColumns
			End Get
		End Property
		Public ReadOnly Property ExproRoundArea() As ExproArea
			Get
				Return mtExproRoundArea
			End Get
		End Property
		Public Shared ReadOnly Property InPlanFilter() As String
			Get
				Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
				Dim sFilter As String = msPlanStateFieldName & "<>" & CStr(iCriteriaValue)

				Return sFilter
			End Get
		End Property
		Public Shared ReadOnly Property InPlanFilter(iOverlayIndex As DMAcadExt.enOverlayIndex) As String
			Get
				Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
				Dim sFilter As String = zzGetPlanStateFieldName(iOverlayIndex) & "<>" & CStr(iCriteriaValue)

				Return sFilter
			End Get
		End Property
		Public Shared ReadOnly Property PolygonInPlanFilter() As String
			Get
				Return msInPlanFieldName
			End Get
		End Property
		Public Shared ReadOnly Property MainView() As System.Data.DataView
			Get
				Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
				If moMainDataTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property


		Public Shared ReadOnly Property BlockTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As System.Data.DataTable
			Get
				Return moaBlockTable(iOverlayIndex)
			End Get
		End Property
		Public Shared ReadOnly Property BlockView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As System.Data.DataView
			Get
				Dim sSort As String = BlockFieldName & "," & BlockAddFieldName
				Dim sMsg As String = ""
				For i As Integer = 0 To moaBlockTable.GetUpperBound(0)
					If moaBlockTable(i) Is Nothing Then
						sMsg &= CStr("Nothing") & vbCrLf
					Else
						sMsg &= CStr(moaBlockTable(i).Rows.Count) & vbCrLf
					End If
				Next
				'     MessageBox.Show(CStr(iOverlayIndex) & vbCrLf & "____________" & vbCrLf & sMsg, "01_327")

				'	System.Windows.Forms.MessageBox.Show(CStr(moBlockTable IsNot Nothing), "01_387f")
				If moaBlockTable(iOverlayIndex) IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moaBlockTable(iOverlayIndex), String.Empty, sSort, DataViewRowState.CurrentRows)
					'	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oDataView.Count), "01_323")

					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False

					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property

		Public Shared ReadOnly Property BlockRegionView() As System.Data.DataView
			Get
				Dim sSort As String = BlockFieldName & "," & BlockAddFieldName
				Dim sMsg As String = ""

				'  MessageBox.Show(CStr(iOverlayIndex) & vbCrLf & "____________" & vbCrLf & sMsg, "01_327")

				'	System.Windows.Forms.MessageBox.Show(CStr(moBlockTable IsNot Nothing), "01_387f")
				If moBlockRegionTable IsNot Nothing Then
					DMCommon.Debug.MsgBox("01_559", moBlockRegionTable.Rows.Count)

					Dim oDataView As System.Data.DataView = New System.Data.DataView(moBlockRegionTable, String.Empty, sSort, DataViewRowState.CurrentRows)
					'	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oDataView.Count), "01_323")

					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False

					Return oDataView
				Else
					DMCommon.Debug.MsgBox("01_559Null", "moBlockRegionTable Is  Nothing ")
					Return Nothing
				End If

			End Get
		End Property
		Public Shared ReadOnly Property LotContentView() As System.Data.DataView
			Get
				If moLotsContentTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moLotsContentTable, String.Empty, String.Empty, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public Shared Function LotContentAreaView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByRef iaColumns() As Integer) As System.Data.DataView
			Dim oPolygonTable As System.Data.DataTable = zzGetPolygonTable(iOverlayIndex)
			Dim sFilter As String = TplnParcel.PolygonInPlanFilter
			Dim sSort As String = msLotOrderFieldName & "," & BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
			If oPolygonTable IsNot Nothing Then
				Dim oDataView As System.Data.DataView = New System.Data.DataView(oPolygonTable, sFilter, sSort, DataViewRowState.CurrentRows)
				Dim iaAcadColumns() As Integer = {2, 0, 1, 6, 6}
				iaColumns = iaAcadColumns
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Function
		Public Shared ReadOnly Property LanduseView(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As System.Data.DataView
			Get
				Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, False)
				If oLanduseTable IsNot Nothing Then
					Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & LanduseOrderFieldName
					Dim oDataView As System.Data.DataView = New System.Data.DataView(oLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property
		Public ReadOnly Property GetLanduseDic(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TplnLanduses
			Get
				Return zzGetLanduseDic(iTopoPurpose)

			End Get
		End Property
		Public Shared ReadOnly Property PolygonView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As System.Data.DataView
			Get
				Dim oPolygonTable As System.Data.DataTable = zzGetPolygonTable(iOverlayIndex)
				If oPolygonTable IsNot Nothing Then
					Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName & "," & msLotOrderFieldName
					Dim oDataView As System.Data.DataView = New System.Data.DataView(oPolygonTable, String.Empty, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					MessageBox.Show(iOverlayIndex.ToString(), "TplnParcel - PolygonView")
					Return Nothing
				End If
			End Get
		End Property
		Public Shared ReadOnly Property ExproTableByCol As System.Data.DataTable
			Get
				Return moExproTableByCol
			End Get
		End Property
		Public Shared ReadOnly Property ExproTable As System.Data.DataTable
			Get
				Return moExproTable
			End Get
		End Property
		Public Shared ReadOnly Property ExproTablePgons As System.Data.DataTable
			Get
				Return moExproTablePgons
			End Get
		End Property

		'	041224
		Public Shared ReadOnly Property InPlanView() As System.Data.DataView
			Get
				Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
				Dim sFilter As String = msPlanStateFieldName & "<>" & CStr(iCriteriaValue)
				Dim sSort As String = BlockFieldName & "," & BlockAddFieldName & "," & ParcelOrderFieldName
				If moMainDataTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, sFilter, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If
			End Get
		End Property

#End Region
#Region "Constructors & Destructors"

		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)

			MyBase.New(oPolygon)
			MyBase.SetAttributeOrder()  'miaBlockAttribIndex
			MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
			'	
			'   Dim iRow As Integer
			mdicParcelExproTypes = New Dictionary(Of Integer, TplnExproType)()
			mhsRegions = New HashSet(Of Integer)()
			mhsLots = New HashSet(Of Integer)()
			mcolOutPgons = New ObjectModel.Collection(Of ParcelArea)()
			If Not dbAcadPoint Then
				mtParcelData = New ParcelData(dsaBlockAttribText, dsaAddBlockAttribText)
				If Not mtParcelData.Correct Then
					TplnProject.WriteMessageBox("Data is incorrect " & CStr(ddCentroidX) & "," & CStr(ddCentroidY), "TplnParcel - New_14")
				End If
				If Not String.IsNullOrEmpty(mtParcelData.ErrMessage) Then
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "", mtParcelData.ErrMessage, True, ParcelData.MapThemeID, 3, True, True)
					'DMCommon.Debug.MsgBox("ErrMessage", mtParcelData.ErrMessage)
					DMCommon.Debug.MsgBoxLoop("B 1002", "NewParcel")
				End If

				If mtParcelData.Block = 0 Then
					TplnProject.WriteMessageBox("Data is incorrect " & CStr(ddCentroidX) & "," & CStr(ddCentroidY), "TplnParcel - New_15")

					'	DMCommon.Functions.DispArray(dsaBlockAttribText, "01_533s", True)
				End If
				MyBase.dsName = mtParcelData.Name
				MyBase.diOrder = mtParcelData.Order
				'  DMAcadExt.AcadDocument.WriteDebugMessage("06_71! " & mtParcelData.Name & "|" & MyBase.dsName & "'" & Me.Name)
				If Not mtParcelData.HasLegalArea Then
					mlstMissingLegalAreaPoints.Add(New DMAcadExt.TPlnPoint(ddCentroidX, ddCentroidY))
				End If
				If Not mtParcelData.HasLegalArea Then
					mdRoundedArea = Math.Round(Me.AcadArea(False) * TplnProject.CalcRoundFactor, MidpointRounding.AwayFromZero) / TplnProject.CalcRoundFactor
				End If
				mtParcelArea.CalculateArea(mtParcelData.LegalArea, MyBase.ddAcadArea, miRoundDigit)

				If mtParcelArea.HasDeviation Then
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "", DMCommon.dmMessages.Message(303, mtParcelArea.LegalArea, mtParcelArea.CalcArea), False, ParcelData.MapThemeID, 21)
				End If

				If mtParcelData.Block <> 0 AndAlso DBShapeArea = 0 Then
					'  DMCommon.Debug.MsgBox("12+420", DBLegalArea, LegalArea(False), ParcelData.MapThemeID)
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "", DMCommon.dmMessages.Message(304, dsName, BlockFull), False, ParcelData.MapThemeID, 23)
				End If

				If DBLegalArea <> 0.0 AndAlso DBLegalArea <> LegalArea(False) Then
					'  DMCommon.Debug.MsgBox("12+420", DBLegalArea, LegalArea(False), ParcelData.MapThemeID)
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "", zzGetLegalAreaMsgText(), False, ParcelData.MapThemeID, 24)
				End If

				If DBShapeArea <> 0.0 AndAlso Math.Abs(DBShapeArea - AcadArea(False)) > 0.2 Then
					'  DMCommon.Debug.MsgBox("12+420", DBLegalArea, LegalArea(False), ParcelData.MapThemeID)
					DMAcadExt.AppMessages.AddMessage(True, MyBase.CentroidX, MyBase.CentroidY, "", zzGetCalcAreaMsgText(), False, ParcelData.MapThemeID, 25, True, False)
				End If


			End If
			mdicLandusesAppr = New TplnLanduses(DMAcadExt.enTopoPurpose.Approved)
			mdicLandusesProp = New TplnLanduses(DMAcadExt.enTopoPurpose.Proposed)

			mdicTypeOverlayGroups = New Dictionary(Of Integer, TypeOverlayGroup)()
			mdicTypeOverlayGroupsPlus = New Dictionary(Of Integer, TypeOverlayGroup)()
		End Sub
		Private Function zzGetLegalAreaMsgText() As String
			Const s1 As String = "שטח רשום חלקה בקובץ"
			Const s2 As String = "שטח רשום חלקה במאגר מידע"
			Return s1 & " " & FormatNumber(LegalArea(False), 0) & " " & vbCrLf & s2 & " " & FormatNumber(DBLegalArea, 0)
		End Function
		Private Function zzGetCalcAreaMsgText() As String
			Const s1 As String = "שטח חלקה מחושב בקובץ"
			Const s2 As String = "שטח חלקה מחושב במאגר מידע"
			Return DMCommon.dmMessages.Message(305, dsName, BlockFull) & vbCrLf & s1 & " " & FormatNumber(AcadArea(False), 1) & " " & vbCrLf & s2 & " " & FormatNumber(DBShapeArea, 1)
		End Function


		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.Entity)
			MyBase.New(oPolygon)
			MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
			Dim oXDataParcel As DMAcadExt.TplnXDataParcel = New DMAcadExt.TplnXDataParcel(oPolygon.XData)
			MyBase.doTplnXData = oXDataParcel
			MyBase.TopoID = oXDataParcel.DataID
			mtParcelData.Block = oXDataParcel.Block
			mtParcelData.BlockAdd = oXDataParcel.BlockAdd

			miBlockAdd = oXDataParcel.BlockAdd
			dsName = oXDataParcel.Name
			mtParcelData.LegalArea = oXDataParcel.LegalArea
			mdLegalArea = oXDataParcel.LegalArea
			zzAfterSetLegalArea(False)
			ddAcadArea = oXDataParcel.AcadArea * ddAreaScale
			ddPerimeter = oXDataParcel.Perimeter
			mdicLandusesAppr = New TplnLanduses(DMAcadExt.enTopoPurpose.Approved)
			mdicLandusesProp = New TplnLanduses(DMAcadExt.enTopoPurpose.Proposed)
			'''''''''''''''''''''''''''	MyBase.BoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
			mtParcelArea.CalculateArea(mtParcelData.LegalArea, MyBase.ddAcadArea, miRoundDigit)
		End Sub

		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, iFeatureID As Integer, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
			MyBase.New(oPolygon, iFeatureID, tCentroidAcObjID)
			MyBase.SetAttributeOrder()
			DMCommon.Functions.DispArray(dsaBlockAttribText, "dsaBlockAttribText!!", True)
			mtParcelData = New ParcelData(dsaBlockAttribText)
			MyBase.dsName = mtParcelData.Name
			MyBase.diOrder = mtParcelData.Order
		End Sub


		Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, sXDAppName As String)
			MyBase.New(oPolygon, sXDAppName)
			MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
			Dim iAttribUB As Integer = -1
			If MyBase.dbCorrect Then
				If doTplnXData IsNot Nothing Then
					Dim sName As String = Nothing
					doTplnXData.GetInt(2, mtParcelData.Block)
					doTplnXData.GetInt(3, mtParcelData.BlockAdd)

					doTplnXData.GetStr(4, sName)
					If sName IsNot Nothing Then
						MyBase.SetName(sName.Trim(), True)
					End If
					doTplnXData.GetDbl(5, mdLegalArea)
					zzAfterSetLegalArea(True)
				End If
			End If
		End Sub

#End Region
#Region "Instance members"
		Private mtParcelData As ParcelData
		Public Function PaintInPlan(ByVal iPaintMethod As DMAcadExt.PaintMethod, ByVal tColorScheme As DMAcadExt.ColorScheme, ByVal bOpenBlock As Boolean, Optional ByVal sBlockLayer As String = "", Optional ByVal bRecursion As Boolean = False) As DMAcadExt.PaintException
			Dim bYesPaint As Boolean = True
			Dim iRes As DMAcadExt.PaintException = DMAcadExt.PaintException.OK

			If Me.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = NumerationPair.enComplexType.Entire Then

				Return MyBase.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, bOpenBlock, sBlockLayer)

			ElseIf Me.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = NumerationPair.enComplexType.Partial Then

				For Each oOverlayPgon As TopoManager.TPlanGraph.TplnOverlayPgon In Me.GetOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay).Values

					If oOverlayPgon.LotTopoID <> 0 AndAlso Not oOverlayPgon.LotOut Then

						If iRes = DMAcadExt.PaintException.OK Then
							If bYesPaint Then
								iRes = oOverlayPgon.Paint(DMAcadExt.PaintMethod.ColorScheme, tColorScheme, bOpenBlock, sBlockLayer)

								'bYesPaint = Not bYesPaint
							End If

						End If

					End If

				Next
				Return iRes
			Else
				Return DMAcadExt.PaintException.Undefined
			End If
		End Function

		Public Sub zzAddLot(iLotTopoID As Integer)
			If Not mhsLots.Contains(iLotTopoID) Then
				mhsLots.Add(iLotTopoID)
			End If
		End Sub

		Private Sub zzAddRegion(iRegionNo As Integer)
			If Not mhsRegions.Contains(iRegionNo) Then
				mhsRegions.Add(iRegionNo)
			End If
		End Sub
		Public Function LotRelation(iOverlayIndex As DMAcadExt.enOverlayIndex, iLotTopoID As Integer) As NumerationPair.enComplexType
			If mhsLots.Contains(iLotTopoID) Then
				If Not mbaOutPlan(iOverlayIndex) AndAlso mhsLots.Count = 1 Then
					Return NumerationPair.enComplexType.Entire
				Else
					Return NumerationPair.enComplexType.Partial
				End If
			Else
				Return NumerationPair.enComplexType.NotExists
			End If

		End Function




		Public Function RegionRelationText(iRegionNo As Integer) As String
			If mhsRegions.Count > 0 Then
				Dim sRes As String = Nothing
				For Each iRegion As Integer In mhsRegions
					If String.IsNullOrEmpty(sRes) Then
						sRes = iRegion.ToString()
					Else
						sRes &= "," & iRegion.ToString()
					End If
				Next
				If mbOutPlan Then
					Return "P - " & sRes
				Else
					Return "E - " & sRes
				End If


			Else
				Return "None"
			End If

		End Function
		Public Function RegionRelation(iOverlayIndex As DMAcadExt.enOverlayIndex, iRegionNo As Integer, Optional hsRegions As HashSet(Of Integer) = Nothing) As NumerationPair.enComplexType
			'mhsRegions.ToArray()


			If iRegionNo <> 0 AndAlso mhsRegions.Contains(iRegionNo) Then
				If Not Me.OutPlan(iOverlayIndex) AndAlso mhsRegions.Count = 1 Then
					Return NumerationPair.enComplexType.Entire
				Else
					Return NumerationPair.enComplexType.Partial
				End If
			ElseIf iRegionNo = 0 AndAlso hsRegions IsNot Nothing AndAlso mhsRegions.IsSubsetOf(hsRegions) Then
				If Not Me.OutPlan(iOverlayIndex) AndAlso (mhsRegions.Count <= hsRegions.Count) Then
					Return NumerationPair.enComplexType.Entire
				Else
					Return NumerationPair.enComplexType.Partial
				End If
			Else
				Return NumerationPair.enComplexType.NotExists
			End If

		End Function
		Public ReadOnly Property ExproParcelArea() As ParcelArea()
			Get
				Return mtaExproParcelArea
			End Get
		End Property
		Public Property OwnerArea() As TplnOwnerArea()


			Get
				Return mtaOwnerArea
			End Get
			Set(taValue As TplnOwnerArea())
				mtaOwnerArea = taValue
			End Set

		End Property

		Public ReadOnly Property OutPgons() As ParcelArea()
			Get
				Return mtaOutPgons
			End Get
		End Property

		Public ReadOnly Property ExtName() As String
			Get
				Dim sExt As String
				Select Case PlanState
					Case NumerationPair.enComplexType.Entire
						sExt = msEntireExtName
					Case NumerationPair.enComplexType.Partial
						sExt = msPartialExtName
					Case Else
						sExt = String.Empty
				End Select
				Return sExt & Me.Name
			End Get
		End Property

		Public ReadOnly Property PlanStateTextA() As String
			Get
				Dim sExt As String
				Select Case PlanState
					Case NumerationPair.enComplexType.Entire
						sExt = msEntireExtName
					Case NumerationPair.enComplexType.Partial
						sExt = msPartialExtName
					Case Else
						sExt = String.Empty
				End Select
				Return sExt & Me.Name
			End Get
		End Property
		Public ReadOnly Property BlockFull() As String
			Get
				Return TplnBlock.GetBlockName(Me.BlockNo, Me.BlockAdd)

			End Get
		End Property
		Public ReadOnly Property BlockNo() As Integer
			Get
				Return mtParcelData.Block

			End Get
		End Property
		Public ReadOnly Property BlockAdd() As Integer
			Get
				Return mtParcelData.BlockAdd

			End Get
		End Property
		Public ReadOnly Property Owner() As Integer
			Get
				Return mtParcelData.Owner

			End Get
		End Property
		Public ReadOnly Property BlockKey() As Integer
			Get
				'If BlockNo = 66690 Then
				'   DMAcadExt.AcadDocument.WriteMessage("M#1: " & CStr(1000 * BlockNo + BlockAdd))
				'End If
				Return 1000 * BlockNo + BlockAdd
			End Get
		End Property
		Public ReadOnly Property LegalOrAcadArea(ByVal bDun As Boolean) As Double
			Get
				Dim dRes As Double
				'DMAcadExt.AcadDocument.WriteMessage("LEGA: " & CStr(mbHasLegalArea) & ":" & CStr(Me.LegalArea) & ":" & CStr(mdRoundedArea))
				If mtParcelData.HasLegalArea Then
					dRes = Me.LegalArea(False)
				Else
					dRes = mdRoundedArea
				End If
				If bDun Then
					Return dRes / TplnProject.UnitScaleFactor
				Else

					Return dRes
				End If
			End Get
		End Property
		Public ReadOnly Property LegalArea(ByVal bDun As Boolean) As Double
			Get

				If bDun Then
					Return mtParcelData.LegalArea / TplnProject.UnitScaleFactor
				Else

					Return mtParcelData.LegalArea
				End If
			End Get
		End Property
		Public ReadOnly Property DBLegalArea() As Double
			Get
				Return mtParcelData.DBLegalArea
			End Get
		End Property
		Public ReadOnly Property DBShapeArea() As Double
			Get
				Return mtParcelData.DBShapeArea
			End Get
		End Property

		Public ReadOnly Property IsAnalytic() As Boolean
			Get
				Return mtParcelData.IsAnalytic

			End Get
		End Property
		Public ReadOnly Property HasOutPgonsDeviation() As Boolean
			Get
				Return mbHasOutPgonsDeviation

			End Get
		End Property


		Public ReadOnly Property InPlanArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Double
			Get
				Dim oUnionPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
				If oUnionPgons IsNot Nothing Then
					Dim dSum As Double
					For Each oUnionPgon As TplnOverlayPgon In oUnionPgons.Values
						dSum += oUnionPgon.InPlanArea()
						If MyBase.diTopoID = 2128 Then
							DMAcadExt.AcadDocument.WriteMessage("Alla01: " & CStr(oUnionPgon.TopoID) & ":" & oUnionPgon.LotOut & ":" & CStr(oUnionPgon.InPlanArea()))
						End If
					Next
					If MyBase.diTopoID = 2117 Then
						DMAcadExt.AcadDocument.WriteMessage("Alla02: " & CStr(dSum))
					End If
					Return dSum
				Else
					Return 0.0
				End If
			End Get
		End Property
		Public Property InPlan() As Boolean
			Get
				Return mbInPlan
			End Get
			Set(ByVal bValue As Boolean)
				mbInPlan = bValue
			End Set
		End Property
		Public ReadOnly Property HasLanduses() As Boolean
			Get
				If mdicLandusesAppr IsNot Nothing AndAlso mdicLandusesAppr.Count > 0 Then
					Return True
				ElseIf mdicLandusesProp IsNot Nothing AndAlso mdicLandusesProp.Count > 0 Then
					Return True
				Else
					Return False
				End If

			End Get

		End Property
		Public ReadOnly Property HasExpro() As Boolean
			Get
				If mdicParcelExproTypes IsNot Nothing AndAlso mdicParcelExproTypes.Count > 0 Then
					Return True

				Else
					Return False
				End If

			End Get

		End Property
		Public Property InPlan(iOverlayIndex As DMAcadExt.enOverlayIndex) As Boolean
			Get
				Return mbaInPlan(iOverlayIndex)
			End Get
			Set(ByVal bValue As Boolean)
				mbaInPlan(iOverlayIndex) = bValue
			End Set
		End Property
		Public Property OutPlan() As Boolean
			Get
				Return mbOutPlan
			End Get
			Set(ByVal bValue As Boolean)
				mbOutPlan = bValue
			End Set
		End Property
		Public Property OutPlan(iOverlayIndex As DMAcadExt.enOverlayIndex) As Boolean
			Get
				Return mbaOutPlan(iOverlayIndex)
			End Get
			Set(ByVal bValue As Boolean)
				mbaOutPlan(iOverlayIndex) = bValue
			End Set
		End Property
		Public ReadOnly Property Order() As Long
			Get
				Return MyBase.diOrder
			End Get
		End Property

		Public ReadOnly Property ExproTypeCount As Integer
			Get
				Return mdicTypeOverlayGroups.Count
			End Get
		End Property
		Public Shared Function GetPlanStateText(iPlanState As NumerationPair.enComplexType) As String
			If iPlanState = NumerationPair.enComplexType.Partial Then
				Return msPartialName
			ElseIf iPlanState = NumerationPair.enComplexType.Entire Then
				Return msEntireName
			ElseIf iPlanState = NumerationPair.enComplexType.Undefined Then
				Return " - " '"None"
			Else
				Return " -?- " '"Unknown"
			End If
		End Function

		Public ReadOnly Property PlanStateText(iPlanState As NumerationPair.enComplexType) As String
			Get
				If iPlanState = NumerationPair.enComplexType.Partial Then
					Return msPartialName
				ElseIf iPlanState = NumerationPair.enComplexType.Entire Then
					Return msEntireName
				ElseIf iPlanState = NumerationPair.enComplexType.Undefined Then
					Return " - " '"None"
				Else
					Return " ? " '"Unknown"
				End If
			End Get
		End Property
		Public ReadOnly Property PlanStateText(iOverlayIndex As DMAcadExt.enOverlayIndex) As String
			Get
				If mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return msPartialName  '"חלקית"	'"Partially"
				ElseIf mbaInPlan(iOverlayIndex) And Not mbaOutPlan(iOverlayIndex) Then
					Return msEntireName '"בשלמותה" '"Complete"
				ElseIf Not mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return " - " '"None"
				Else
					Return " ? " '"Unknown"
				End If
			End Get
		End Property

		Public ReadOnly Property PlanState() As NumerationPair.enComplexType
			Get
				If mbInPlan And mbOutPlan Then
					Return NumerationPair.enComplexType.Partial
				ElseIf mbInPlan And Not mbOutPlan Then
					Return NumerationPair.enComplexType.Entire
				ElseIf Not mbInPlan And mbOutPlan Then
					Return NumerationPair.enComplexType.Undefined
				Else
					Return NumerationPair.enComplexType.NotExists
				End If

			End Get
		End Property
		Public ReadOnly Property PlanState(iOverlayIndex As DMAcadExt.enOverlayIndex) As NumerationPair.enComplexType
			Get
				If mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return NumerationPair.enComplexType.Partial
				ElseIf mbaInPlan(iOverlayIndex) And Not mbaOutPlan(iOverlayIndex) Then
					If miaMerhav(iOverlayIndex) = -1 Then
						Return NumerationPair.enComplexType.MultiMerhav
					Else
						Return NumerationPair.enComplexType.Entire
					End If

				ElseIf Not mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return NumerationPair.enComplexType.Undefined
				Else
					Return NumerationPair.enComplexType.NotExists
				End If

			End Get
		End Property
		Public ReadOnly Property MerhavPlanState(iOverlayIndex As DMAcadExt.enOverlayIndex) As NumerationPair.enComplexType
			Get
				If mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return NumerationPair.enComplexType.Partial
				ElseIf mbaInPlan(iOverlayIndex) And Not mbaOutPlan(iOverlayIndex) Then
					If miaMerhav(iOverlayIndex) = -1 Then
						Return NumerationPair.enComplexType.Partial
					Else
						Return NumerationPair.enComplexType.Entire
					End If

				ElseIf Not mbaInPlan(iOverlayIndex) And mbaOutPlan(iOverlayIndex) Then
					Return NumerationPair.enComplexType.Undefined
				Else
					Return NumerationPair.enComplexType.NotExists
				End If

			End Get
		End Property
		Public ReadOnly Property SingleLotState(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As NumerationPair.enComplexType
			Get
				If Me.PlanState = NumerationPair.enComplexType.Entire AndAlso Me.doaOverlayPgons(iOverlayIndex).Count = 1 Then
					Return NumerationPair.enComplexType.Entire
				ElseIf Me.PlanState = NumerationPair.enComplexType.Undefined Then
					Return NumerationPair.enComplexType.Undefined
				Else
					Return NumerationPair.enComplexType.Partial
				End If
			End Get
		End Property

		Public Sub SetMerhav(iOverlayIndex As DMAcadExt.enOverlayIndex, iMerhavID As Integer)
			If miaMerhav(iOverlayIndex) = 0 Then
				miaMerhav(iOverlayIndex) = iMerhavID
			ElseIf miaMerhav(iOverlayIndex) <> -1 AndAlso miaMerhav(iOverlayIndex) <> iMerhavID Then
				miaMerhav(iOverlayIndex) = -1
			End If
		End Sub
		Public Overrides Sub AddDataToMainTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim oNewRow As System.Data.DataRow
			Dim dValue As Double
			Dim sTest As String = ""
			Dim tInPlanAreaSet As TplnAreaSet
			If moMainDataTable IsNot Nothing Then
				Try
					oNewRow = moMainDataTable.NewRow()
					With oNewRow
						'// 0-5 Parcel data 
						.Item(BlockFullFieldName) = TplnBlock.GetBlockName(Me.BlockNo, Me.BlockAdd)


						.Item(NameFieldName) = MyBase.Name

						.Item(LegalAreaFieldName) = Me.LegalArea(False) 'Me.LegalOrAcadArea(False)
						.Item(msDBLegalAreaFieldName) = Me.DBLegalArea

						.Item(IsAnalyticFieldName) = Me.IsAnalytic
						If miTest < 0 Then
							DMAcadExt.AcadDocument.WriteMessage("#176 " & CStr(.Item(LegalAreaFieldName)) & ":" & CStr(mdLegalArea) & ":" & CStr(mtParcelData.LegalArea))
						End If
						.Item(TopoReader.msAreaFldName) = Math.Round(MyBase.ddAcadArea, 4)
						.Item(msToleranceFieldName) = Math.Round(Me.Tolerance(False), 4)
						.Item(msDeltaAreaFieldName) = Me.DeltaArea(False)
						'''''''''''''If Me.HasDeviation Then
						.Item(msDeviationFieldName) = Math.Round(Me.Deviation(False), 4)
						''''''''''''''''End If
						.Item(IsAnalyticFieldName) = Me.IsAnalytic
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!AddToMain", Me.LegalArea(False), MyBase.ddAcadArea, Me.DeltaArea(False), Me.Tolerance(True), Me.Tolerance(False), Me.Deviation(True), Me.Deviation(False))
						'Me.InPlanArea
						If bMerge Then
							If bApproved Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Approved)
								.Item(msSumApprPgonAreaMergeFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	'DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - מק" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue), False)
								'End If
							End If
							If bProposed Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Proposed)
								.Item(msSumPropPgonAreaMergeFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - ממ" & " #" & CStr(Me.TopoID), False)
								'End If
							End If
						End If

						If bFDO_Overlay Then
							If bApproved Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
								.Item(msSumApprPgonAreaFDO_OverlayFldName) = Math.Round(dValue, 4)
								'If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'Dim iPgonCount As Integer = MyBase.PolygonCount(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - מק" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue) & "," & CStr(iPgonCount), False)
								'	End If
							End If
							If bProposed Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
								.Item(msSumPropPgonAreaFDO_OverlayFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - ממ" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue), False)
								'End If
							End If
						End If


						'// 6-9 InPlan Appr   data 
						If bMerge And bApproved Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.ApprMerge)
							'	DMAcadExt.AcadDocument.WriteMessage("!!22_3: " & CStr(tInPlanAreaSet.AcadArea) & ":" & CStr(tInPlanAreaSet.CalcArea) & ":" & CStr(tInPlanAreaSet.CalcArea2))
							.Item(msInPlanAreaApprMergeFieldName) = tInPlanAreaSet.AcadArea
							.Item(msInPlanCalcAreaApprMergeFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2ApprMergeFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaApprMergeFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStateApprMergeFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.ApprMerge)             ' 12
							.Item(msPlanStateTextApprMergeFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprMerge)     ' 13
							sTest &= Me.PlanState(DMAcadExt.enOverlayIndex.ApprMerge) & Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprMerge)
						End If

						'// 10-13 InPlan Prop   data 

						If bMerge And bProposed Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.PropMerge)
							.Item(msInPlanAreaPropMergeFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)
							.Item(msInPlanCalcAreaPropMergeFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2PropMergeFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaPropMergeFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStatePropMergeFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.PropMerge)             ' 12
							.Item(msPlanStateTextPropMergeFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.PropMerge)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.PropMerge) & Me.PlanStateText(DMAcadExt.enOverlayIndex.PropMerge)
						End If
						''''''''''''''''''''''''
						'// 14,15 InPlan Prop   data 
						If bFDO_Overlay And False Then
							If bApproved Then
								dValue = MyBase.SumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
								'	DMAcadExt.AcadDocument.WriteMessage("!!44_12: " & CStr(dValue))
								.Item(msSumApprPgonAreaFDO_OverlayFldName) = dValue

								If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
									DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "DVA", "חלקה - מק" & " #" & CStr(Me.TopoID), False)
								End If
							End If
							If bProposed Then
								dValue = MyBase.SumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
								.Item(msSumPropPgonAreaMergeFldName) = dValue
								If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
									DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "DVP", "חלקה - ממ" & " #" & CStr(Me.TopoID), False)
								End If
							End If
						End If

						'// 16-19 InPlan Appr   data 
						If bFDO_Overlay And bApproved Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
							.Item(msInPlanAreaApprFDO_OverlayFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)

							'
							.Item(InPlanCalcAreaApprFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2ApprFDO_OverLayFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaApprFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStateApprFDO_OverlayFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)             ' 12
							.Item(msPlanStateTextApprFDO_OverlayFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) & Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
						End If

						'// 20-23 InPlan Prop   data 
						If bFDO_Overlay And bProposed Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
							.Item(msInPlanAreaPropFDO_OverlayFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)
							.Item(msInPlanCalcAreaPropFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2PropFDO_OverLayFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaPropFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStatePropFDO_OverlayFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.PropFDO_Overlay)             ' 12
							.Item(msPlanStateTextPropFDO_OverlayFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.PropFDO_Overlay)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.PropFDO_Overlay) & Me.PlanStateText(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
						End If
						''''''''''''''''''''''''''''''''
						.Item(msOwnerFieldName) = mtParcelData.Owner
						'// 24-31 InPlan Additional   data 
						.Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
						.Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
						'	.Item(msPlanStateFieldName) = CType(Me.PlanState, Integer)
						'	.Item(msPlanStateTextFieldName) = Me.PlanStateText

						.Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
						.Item(TopoReader.msTopoIDFldName) = MyBase.TopoID
						.Item(TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID   'MyBase.diCentroidAcObjID.OldIdPtr.ToInt64()
						.Item(BlockFieldName) = Me.BlockNo
						.Item(BlockAddFieldName) = Me.BlockAdd
						.Item(ParcelOrderFieldName) = MyBase.diOrder

						If mbHasOwnershipNotes AndAlso mcolOwnNotePgons IsNot Nothing Then
							Dim dParagraph19Area As Double = 0.0
							Dim dLeasingArea As Double = 0.0
							Dim dOverlayArea As Double = 0.0

							For Each oOwnershipNote As TplnOwnNoteArea In mcolOwnNotePgons
								dParagraph19Area += oOwnershipNote.GetParagraph19Area
								dLeasingArea += oOwnershipNote.GetLeasingArea
								dOverlayArea += oOwnershipNote.GetOverlayArea
							Next
							'	DMCommon.Debug.MsgBox("13_120ee", mbHasOwnershipNotes, Paragraph19AreaFieldName, dParagraph19Area, dLeasingArea)
							.Item(Paragraph19AreaFieldName) = dParagraph19Area
							.Item(LeasingAreaFieldName) = dLeasingArea
							.Item(OverlayPrg5LeasAreaFieldName) = dOverlayArea
						End If


					End With
					'	DMAcadExt.AcadDocument.WriteMessage("!!TEST: " & stest)
					moMainDataTable.Rows.Add(oNewRow)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - AddDataToMainTable_1a")
				End Try
			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
			End If
			miTest += 1
		End Sub


		Public Sub AddDataToMainRegionTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegion As Integer)
			Dim oNewRow As System.Data.DataRow
			Dim dValue As Double
			Dim sTest As String = ""
			Dim tInPlanAreaSet As TplnAreaSet
			If moMainRegionDataTable IsNot Nothing Then
				Try
					oNewRow = moMainRegionDataTable.NewRow()
					With oNewRow
						'// 0-5 Parcel data 
						.Item(BlockFullFieldName) = TplnBlock.GetBlockName(Me.BlockNo, Me.BlockAdd)


						.Item(NameFieldName) = MyBase.Name

						.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
						.Item(IsAnalyticFieldName) = Me.IsAnalytic
						If miTest < 0 Then
							DMAcadExt.AcadDocument.WriteMessage("#176 " & CStr(.Item(LegalAreaFieldName)) & ":" & CStr(mdLegalArea) & ":" & CStr(mtParcelData.LegalArea))
						End If
						.Item(TopoReader.msAreaFldName) = Math.Round(MyBase.ddAcadArea, 4)

						.Item(msToleranceFieldName) = Math.Round(Me.Tolerance(True), 4)
						.Item(msDeltaAreaFieldName) = Me.DeltaArea(False)
						If Me.HasDeviation Then
							.Item(msDeviationFieldName) = Math.Round(Me.Deviation(True), 4)
						End If
						.Item(IsAnalyticFieldName) = Me.IsAnalytic




						'Me.InPlanArea
						If bMerge Then
							If bApproved Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Approved)
								.Item(msSumApprPgonAreaMergeFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	'DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - מק" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue), False)
								'End If
							End If
							If bProposed Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Proposed)
								.Item(msSumPropPgonAreaMergeFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - ממ" & " #" & CStr(Me.TopoID), False)
								'End If
							End If
						End If

						If bFDO_Overlay Then
							If bApproved Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
								.Item(msSumApprPgonAreaFDO_OverlayFldName) = Math.Round(dValue, 4)
								'If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'Dim iPgonCount As Integer = MyBase.PolygonCount(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - מק" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue) & "," & CStr(iPgonCount), False)
								'	End If
							End If
							If bProposed Then
								dValue = MyBase.CheckSumPolygonArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
								.Item(msSumPropPgonAreaFDO_OverlayFldName) = dValue
								'	If Not DMCommon.Functions.CheckDeviation(MyBase.ddAcadArea, dValue, mdParcelAreaTolearance) Then
								'	DMAcadExt.AppMessages.AddMessage(True, Me.ddCentroidX, Me.ddCentroidY, "", "חלקה - ממ" & " #" & CStr(Me.TopoID) & "||" & CStr(MyBase.ddAcadArea) & ":" & CStr(dValue), False)
								'End If
							End If
						End If


						'// 6-9 InPlan Appr   data 
						If bMerge And bApproved Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.ApprMerge)

							.Item(msInPlanAreaApprMergeFieldName) = tInPlanAreaSet.AcadArea
							.Item(msInPlanCalcAreaApprMergeFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2ApprMergeFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaApprMergeFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStateApprMergeFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.ApprMerge)             ' 12
							.Item(msPlanStateTextApprMergeFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprMerge)     ' 13
							sTest &= Me.PlanState(DMAcadExt.enOverlayIndex.ApprMerge) & Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprMerge)
						End If

						'// 10-13 InPlan Prop   data 

						If bMerge And bProposed Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.PropMerge)
							.Item(msInPlanAreaPropMergeFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)
							.Item(msInPlanCalcAreaPropMergeFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2PropMergeFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaPropMergeFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStatePropMergeFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.PropMerge)             ' 12
							.Item(msPlanStateTextPropMergeFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.PropMerge)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.PropMerge) & Me.PlanStateText(DMAcadExt.enOverlayIndex.PropMerge)
						End If
						''''''''''''''''''''''''
						'// 14,15 InPlan Prop   data 

						'// 16-19 InPlan Appr   data 
						If bFDO_Overlay And bApproved Then

							tInPlanAreaSet = zzGetRegionAreaset(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, iRegion)
							.Item(msInPlanAreaApprFDO_OverlayFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)

							'
							.Item(InPlanCalcAreaApprFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2ApprFDO_OverLayFieldName) = tInPlanAreaSet.CalcGroupArea
							.Item(msInPlanRoundedAreaApprFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStateApprFDO_OverlayFieldName) = tInPlanAreaSet.PlanState              ' 12
							.Item(msPlanStateTextApprFDO_OverlayFieldName) = GetPlanStateText(tInPlanAreaSet.PlanState)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) & Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
						Else
							System.Windows.Forms.MessageBox.Show("Design Error", "TplnParcel - AddDataToRegionTable_1")
						End If

						'// 20-23 InPlan Prop   data 
						If bFDO_Overlay And bProposed Then
							tInPlanAreaSet = dtaInPlanAreaSet(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
							.Item(msInPlanAreaPropFDO_OverlayFieldName) = Math.Round(tInPlanAreaSet.AcadArea, 4)
							.Item(msInPlanCalcAreaPropFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
							.Item(msInPlanCalcArea2PropFDO_OverLayFieldName) = tInPlanAreaSet.CalcArea2
							.Item(msInPlanRoundedAreaPropFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea
							.Item(msPlanStatePropFDO_OverlayFieldName) = Me.PlanState(DMAcadExt.enOverlayIndex.PropFDO_Overlay)             ' 12
							.Item(msPlanStateTextPropFDO_OverlayFieldName) = Me.PlanStateText(DMAcadExt.enOverlayIndex.PropFDO_Overlay)     ' 13
							sTest &= ":" & Me.PlanState(DMAcadExt.enOverlayIndex.PropFDO_Overlay) & Me.PlanStateText(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
						End If
						''''''''''''''''''''''''''''''''

						'// 24-31 InPlan Additional   data 
						.Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
						.Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
						'	.Item(msPlanStateFieldName) = CType(Me.PlanState, Integer)
						'	.Item(msPlanStateTextFieldName) = Me.PlanStateText

						.Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
						.Item(TopoReader.msTopoIDFldName) = MyBase.TopoID
						.Item(TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID   'MyBase.diCentroidAcObjID.OldIdPtr.ToInt64()
						.Item(BlockFieldName) = Me.BlockNo
						.Item(BlockAddFieldName) = Me.BlockAdd
						.Item(ParcelOrderFieldName) = MyBase.diOrder

					End With
					'	DMAcadExt.AcadDocument.WriteMessage("!!TEST: " & stest)
					If tInPlanAreaSet.PlanState <> NumerationPair.enComplexType.Undefined Then
						moMainRegionDataTable.Rows.Add(oNewRow)
					End If

				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - AddDataToMainTable_1b")
				End Try
			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
			End If
			miTest += 1
		End Sub
		Public Function TestOverlayList(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As String
			Dim oTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			Dim s As String = ""
			If oTopoIDList Is Nothing Then
				Return " Is Nothing "
			Else

				For iIndex As Integer = 0 To oTopoIDList.Count - 1
					If s.Length <> 0 Then s &= ","
					s &= oTopoIDList.Item(iIndex).ToString()
				Next
				Return s
			End If
		End Function
		Public Function TestOverlayListPrint(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Boolean
			Dim oTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			Dim s As String = ""
			If oTopoIDList IsNot Nothing AndAlso oTopoIDList.Count = 2 Then
				DMAcadExt.AcadDocument.WriteMessage("!!51_98: " & Me.TestOverlayList(iOverlayIndex))
				Return True
			Else
				Return False
			End If
		End Function
		Private Sub zzCalculateExproAreaPlusOutPgons()
			Dim iArrayUB As Integer = -1
			Dim iAreaIndex As Integer = 0
			'Dim dInputSum As Double
			Dim tParcelArea As ParcelArea
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!OutPgons1", diTopoID, BlockNo, ParcelNo, mcolOutPgons.Count, miMaxOutPgonsCount)
			If miMaxOutPgonsCount < mcolOutPgons.Count Then
				miMaxOutPgonsCount = mcolOutPgons.Count
			End If
			If mdicTypeOverlayGroups.Count > 0 Then
				iArrayUB = mcolOutPgons.Count + mdicTypeOverlayGroups.Count - 1
				Dim dRangeMin, dRangeMax As Double
				Dim daInput(iArrayUB) As Double
				Dim daRangeMin(iArrayUB) As Double
				Dim daRangeMax(iArrayUB) As Double
				Dim daOutPgonsRangeMin(mcolOutPgons.Count - 1) As Double

				Dim dMinSum As Double
				'Dim taResParcelArea(iArrayUB) As ParcelArea
				ReDim mtaExproParcelArea(iArrayUB)
				iAreaIndex = 0
				For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
					daInput(iAreaIndex) = oTypeOverlayGroup.AcadArea
					daRangeMin(iAreaIndex) = 1.0
					daRangeMax(iAreaIndex) = 1000000.0
					If Not mhsExproTypes.Contains(oTypeOverlayGroup.TypeID) Then
						mhsExproTypes.Add(oTypeOverlayGroup.TypeID)
						If miMaxExproType < oTypeOverlayGroup.TypeID Then
							miMaxExproType = oTypeOverlayGroup.TypeID
						End If
					End If
					dMinSum += daRangeMin(iAreaIndex)
					iAreaIndex += 1
					'	dInputSum += oTypeOverlayGroup.AcadArea
				Next
				'iAreaIndex = 1
				'070626
				tParcelArea.LegalArea = Me.LegalArea(False)
				For iIndex As Integer = 0 To mcolOutPgons.Count - 1
					tParcelArea = mcolOutPgons.Item(iIndex)

					tParcelArea.CalculateRangeNewPlus(Me.LegalArea(False), mcolOutPgons.Count, dRangeMin, dRangeMax)

					If dRangeMin > Me.LegalArea(False) Then
						''''''''''''',,,,,,,,,,,,,,dRangeMin = Me.LegalArea(False)
					End If

					If dRangeMax < Me.LegalArea(False) Then
						'''''''''	dRangeMax = Me.LegalArea(False)
					End If




					daInput(iAreaIndex) = tParcelArea.AcadArea
					daRangeMin(iAreaIndex) = dRangeMin
					daRangeMax(iAreaIndex) = dRangeMax
					daOutPgonsRangeMin(iIndex) = dRangeMin
					dMinSum += daRangeMin(iAreaIndex)
					If TopoID = miTopoID_Debug Then  '
						DMCommon.Debug.ExcelLog.SetNextValue(0, "???ASQ! " & iIndex.ToString(), Me.LegalArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, dRangeMin, dRangeMax, tParcelArea.RangeMin, tParcelArea.RangeMax)
					End If
					iAreaIndex += 1
				Next

				If dMinSum > Me.LegalArea(False) Then
					Dim oBalanceMinArea As BalanceArea = New BalanceArea(daOutPgonsRangeMin, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False) - mdicTypeOverlayGroups.Count, True, "Expro+7")
					For iIndex As Integer = 0 To oBalanceMinArea.OutputFloat.GetUpperBound(0)
						daRangeMin(mdicTypeOverlayGroups.Count + iIndex) = oBalanceMinArea.OutputFloat(iIndex)
					Next

				End If

				'	DMCommon.ExcelLog.SetNextValue(0, "???ASR1!", iArrayUB, mcolOutPgons.Count, mdicTypeOverlayGroups.Count, iAreaIndex)




				Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, daRangeMin, daRangeMax, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "Expro+1")    '   temp 
				Dim oBalanceConditionalCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "Expro+2")    '   temp 
				Dim oBalanceRoundedArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.AcadArea(False), True, "Expro+3")



				iAreaIndex = 0
				If TopoID = miTopoID_Debug Then   '= 9544

					DMCommon.Debug.ExcelLog.SetNextValue(0, "!LegalOrAcadArea", Me.LegalOrAcadArea(False))
					DMCommon.Debug.ExcelLog.SetArray(0, "!daInput", True, daInput)
					DMCommon.Debug.ExcelLog.SetArray(0, "!daRangeMin", True, daRangeMin)
					DMCommon.Debug.ExcelLog.SetArray(0, "!daRangeMax", True, daRangeMax)

					DMCommon.Debug.ExcelLog.SetArray(0, "!CalcArea", True, oBalanceCalcArea.OutputFloat)
					DMCommon.Debug.ExcelLog.SetArray(0, "!Conditional", True, oBalanceConditionalCalcArea.OutputFloat)
					DMCommon.Debug.ExcelLog.SetArray(0, "!Rounded", True, oBalanceRoundedArea.OutputFloat)

					DMCommon.Debug.ExcelLog.SetNextValue(0, "!TypeOverlayGroups.Count", mdicTypeOverlayGroups.Count)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!ConditionalCalcArea", oBalanceConditionalCalcArea.OutputFloat.GetUpperBound(0))

				End If
				For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
					'oTypeOverlayGroup.CalcArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
					'oTypeOverlayGroup.RoundedArea = oBalanceRoundedArea.OutputItemFix(iAreaIndex)
					tParcelArea = New ParcelArea(oTypeOverlayGroup.AcadArea)
					tParcelArea.CalculateArea(oBalanceCalcArea.OutputItemFix(iAreaIndex), 3)
					tParcelArea.ConditionalArea = oBalanceConditionalCalcArea.OutputItemFix(iAreaIndex)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "S1_!", iAreaIndex, oBalanceCalcArea.OutputItemFix(iAreaIndex), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea, oBalanceConditionalCalcArea.OutputItemFix(iAreaIndex), tParcelArea.IsForced)
					tParcelArea.ExproType = oTypeOverlayGroup.TypeID
					mtaExproParcelArea(iAreaIndex) = tParcelArea


					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "S2_!", Me.BlockNo, Me.Name, iAreaIndex, oBalanceCalcArea.OutputItemFix(iAreaIndex), mtaExproParcelArea(iAreaIndex).LegalArea, mtaExproParcelArea(iAreaIndex).AcadArea, mtaExproParcelArea(iAreaIndex).CalcArea, mtaExproParcelArea(iAreaIndex).ConditionalArea)
					iAreaIndex += 1
				Next


				iAreaIndex = mdicTypeOverlayGroups.Count

				If mcolOutPgons.Count > 0 Then
					'ReDim taResParcelArea(mcolOutPgons.Count - 1)
					If TopoID = miTopoID_Debug Then   '= 4878 9544

						DMCommon.Debug.ExcelLog.SetArray(0, "!oBalanceOutput", True, oBalanceCalcArea.Output)
						DMCommon.Debug.ExcelLog.SetArray(0, "!oBalanceConditionalOutput", True, oBalanceConditionalCalcArea.Output)

					End If

					For iIndex As Integer = 0 To mcolOutPgons.Count - 1
						tParcelArea = mcolOutPgons.Item(iIndex)
						'tParcelArea.LegalArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
						tParcelArea.CalculateArea(oBalanceCalcArea.OutputItemFix(iAreaIndex), 3)
						tParcelArea.ConditionalArea = oBalanceConditionalCalcArea.OutputItemFix(iAreaIndex)
						tParcelArea.CalculateArea(3, True)

						tParcelArea.ExproType = 21 + iIndex
						mtaExproParcelArea(iAreaIndex) = tParcelArea
						If tParcelArea.Deviation > 0 Then
							mbHasOutPgonsDeviation = True
						End If
						If TopoID = miTopoID_Debug Then
							DMCommon.Debug.ExcelLog.SetNextValue(0, "!OutpgonDeviation!", Me.BlockNo, Me.Name, mcolOutPgons.Count, iIndex, tParcelArea.LegalArea, tParcelArea.CalcArea, tParcelArea.Deviation, tParcelArea.DeltaArea, tParcelArea.Tolerance, mbHasOutPgonsDeviation)
						End If

						iAreaIndex += 1
					Next
				End If



				If TopoID = miTopoID_Debug Then ' 9544
					DMCommon.Debug.ExcelLog.SetArray(14, "Output+", False, oBalanceCalcArea.Output)
				End If
				'130126	DMCommon.Debug.ExcelLog.SetArray(29, "Output+", False, oBalanceCalcArea.Output)
				Dim iCol As Integer = 37
				Dim iOutPgonIndex As Integer = 0
				Dim dKfParcel As Double = Me.LegalArea(False) / Me.AcadArea(False)
				If mtaExproParcelArea IsNot Nothing Then
					For iIndex As Integer = 0 To iArrayUB
						tParcelArea = mtaExproParcelArea(iIndex)
						If TopoID = miTopoID_Debug Then  '9544
							DMCommon.Debug.ExcelLog.SetValue(iCol, "", tParcelArea.LegalArea, tParcelArea.CalcArea, tParcelArea.CalcArea * dKfParcel * 1000.0, (tParcelArea.LegalArea - tParcelArea.CalcArea * dKfParcel) * 1000.0, tParcelArea.DeltaAreaM, tParcelArea.Tolerance, tParcelArea.Deviation)
						End If
						'130126 DMCommon.Debug.ExcelLog.SetValue(iCol, "", tParcelArea.LegalArea, tParcelArea.CalcArea, tParcelArea.CalcArea * dKfParcel * 1000.0, (tParcelArea.LegalArea - tParcelArea.CalcArea * dKfParcel) * 1000.0, tParcelArea.DeltaAreaM, tParcelArea.Tolerance, tParcelArea.Deviation)
						iCol += 8
						iOutPgonIndex += 1
					Next
					iAreaIndex = 0
					For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
						tParcelArea = mtaExproParcelArea(iAreaIndex)
						oTypeOverlayGroup.CalcAreaPlus = tParcelArea.LegalArea
						If TopoID = miTopoID_Debug Then  '9544
							DMCommon.Debug.ExcelLog.SetNextValue(0, "S3_4878!+", True, tParcelArea.LegalArea, oTypeOverlayGroup.CalcAreaPlus, oTypeOverlayGroup.CalcArea, oTypeOverlayGroup.AcadArea)
						End If
						'	iAreaIndex += 1

						'	dInputSum += oTypeOverlayGroup.AcadArea
						iAreaIndex += 1
					Next
					ReDim mtaOutPgons(mcolOutPgons.Count - 1)
					For iIndex As Integer = 0 To mcolOutPgons.Count - 1
						tParcelArea = mtaExproParcelArea(iAreaIndex)
						mtaOutPgons(iIndex) = tParcelArea

						iAreaIndex += 1
					Next
				End If

			Else
				'DMCommon.ExcelLog.SetNextValue(0, "N Output! NOT", Me.BlockFull, Me.Name)
			End If

			'DMCommon.Debug.MsgBox("MaxExproType", miMaxExproType)
		End Sub
		Private Sub zzCalculateExproArea()
			Dim iArrayUB As Integer = -1
			Dim iAreaIndex As Integer = 0
			Dim dInputSum As Double
			'Dim iTopoID_Debug As Integer = 10280 ' 9469 '9639
			If mdicTypeOverlayGroups.Count > 0 Then

				iArrayUB = mdicTypeOverlayGroups.Count - 1
				Dim daInput(iArrayUB) As Double
				For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
					daInput(iAreaIndex) = oTypeOverlayGroup.AcadArea
					dInputSum += oTypeOverlayGroup.AcadArea
					If TopoID = miTopoID_Debug Then
						DMCommon.Debug.ExcelLog.SetNextValue(0, "Inp4878", mdicTypeOverlayGroups.Count, iAreaIndex, daInput(iAreaIndex), dInputSum)
					End If
					iAreaIndex += 1
				Next
				If dInputSum < Me.AcadArea(False) - 0.1 Then
					ReDim Preserve daInput(iAreaIndex)
					daInput(iAreaIndex) = Me.AcadArea(False) - dInputSum
				End If
				If TopoID = miTopoID_Debug Then
					DMCommon.Debug.ExcelLog.SetArray(0, "Inp4430+", True, daInput)
				End If

				Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "Expro+3")    '   temp 
				Dim oBalanceRoundedArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.AcadArea(False), True, "Expro+4")
				If TopoID = miTopoID_Debug Then
					DMCommon.Debug.ExcelLog.SetNextValue(0, "Out4433", mdicTypeOverlayGroups.Count, iAreaIndex, Me.LegalOrAcadArea(False), Me.AcadArea(False))
					DMCommon.Debug.ExcelLog.SetArray(0, "Out4551", True, oBalanceCalcArea.OutputFloat)
					DMCommon.Debug.ExcelLog.SetArray(0, "Out4552", True, oBalanceRoundedArea.OutputFloat)


				End If

				iAreaIndex = 0
				For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
					oTypeOverlayGroup.CalcArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
					oTypeOverlayGroup.RoundedArea = oBalanceRoundedArea.OutputItemFix(iAreaIndex)

					iAreaIndex += 1
				Next
			End If


		End Sub
		Private Sub zzCalculateArea(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, bRegionExists As Boolean)
			Const iTestPgonId As Integer = 136 '4355
			Dim iArrayUB As Integer = -1
			Dim dInputSum As Double
			Dim sTest As String = "a"
			Dim oLotTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex)
			If oLotTopoIDList Is Nothing OrElse oLotTopoIDList.Count = 0 Then
				Return
			End If

			iArrayUB = oLotTopoIDList.Count - 1
			sTest = "aa"
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
			Dim oaOverlayGroups(iArrayUB) As TplnOverlayGroup
			Dim bRes As Boolean

			sTest = "ab"
			If oAllOverlayGroups IsNot Nothing Then
				If iArrayUB <> -1 Then
					sTest = "ag"
					Dim daInput(iArrayUB) As Double
					Dim iaGroupNo(iArrayUB) As Integer
					Dim laOutput(iArrayUB) As Long
					Dim iGroupNo As Integer
					Dim iAreaIndex As Integer = 0
					Dim oLot As TplnLot = Nothing
					sTest = "ah"
					If iArrayUB > 0 Then
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzCalculateArea", "TplnParcel.Calculate", iArrayUB)
					End If

					Try
						For Each iLotTopoID As Integer In oLotTopoIDList
							bRes = oAllOverlayGroups.TryGetItem(diTopoID, iLotTopoID, oaOverlayGroups(iAreaIndex))
							If bRes Then
								If iLotTopoID = 0 Then
									iGroupNo = 0
								ElseIf bRegionExists Then
									oLot = TplnProject.GetLot(iTopoPurpose, iLotTopoID)
									If oLot IsNot Nothing Then
										iGroupNo = oLot.RegionNo
									Else
										DMAcadExt.AcadDocument.WriteDebugMessage("05_44! iLotTopoID=" & iLotTopoID.ToString())

									End If
								Else

									iGroupNo = 1
								End If
								daInput(iAreaIndex) = oaOverlayGroups(iAreaIndex).AcadArea()
								iaGroupNo(iAreaIndex) = oaOverlayGroups(iAreaIndex).GroupID
								dInputSum += daInput(iAreaIndex)
								If diTopoID = iTestPgonId Then
									DMCommon.Debug.ExcelLog.SetNextValue(0, "Bal1", Name, oLotTopoIDList.Count, iAreaIndex, iLotTopoID, iaGroupNo(iAreaIndex), daInput(iAreaIndex), Me.LegalOrAcadArea(False), TplnLot.HasRegions)
								End If

								iAreaIndex += 1
							Else

								DMAcadExt.AcadDocument.WriteDebugMessage("!239 NOT " & Me.Name & " " & CStr(diTopoID) & " xx " & CStr(iLotTopoID) & "::" & " !!!daInput(" & CStr(iAreaIndex) & ")=" & daInput(iAreaIndex))
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzCalcAreaErr1", diTopoID, iLotTopoID, iAreaIndex, " !!!daInput(" & CStr(iAreaIndex) & ")=" & daInput(iAreaIndex))
								Return
							End If

							'	oAllOverlayGroups.GetItem(diTopoID, iLotTopoID).CalcArea = 3.3
						Next iLotTopoID

						Dim oBalanceCalcArea As BalanceArea
						Dim oBalanceRoundedArea As BalanceArea
						'  Dim iRow As Integer
						sTest = "ba"



						' Me.PlanStateText(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) fff
						Dim b As Boolean
						If diTopoID = iTestPgonId Then
							DMCommon.Debug.ExcelLog.SetEnumerable(0, "oTopoIDList", oLotTopoIDList)

							DMCommon.Debug.ExcelLog.SetNextValue(0, "13_133e", TplnLot.HasRegions, TplnProject.FirstBlueLine, Me.PlanState(iOverlayIndex), iOverlayIndex)
							DMCommon.Debug.ExcelLog.SetEnumerable(0, "daInput", daInput)
							DMCommon.Debug.ExcelLog.SetEnumerable(0, "iaGroupNo", iaGroupNo)

						End If

						If TplnLot.HasRegions OrElse (TplnProject.FirstBlueLine AndAlso Me.PlanState(iOverlayIndex) = NumerationPair.enComplexType.Partial) Then
							sTest = "bk"
							oBalanceCalcArea = New BalanceArea(daInput, iaGroupNo, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "TplnParcel.zzCalculateArea_1", diTopoID = iTestPgonId)    '   temp 
							sTest = "bm"
							oBalanceRoundedArea = New BalanceArea(daInput, iaGroupNo, TplnProject.CalcRoundFactor, Me.AcadArea(False), True, "TplnParcel.zzCalculateArea_2", False)  '   temp 

						Else
							b = True
							sTest = "bl"
							oBalanceCalcArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "TplnParcel.zzCalculateArea_3")    '   temp 
							oBalanceRoundedArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.AcadArea(False), True, "TplnParcel.zzCalculateArea_4")  '   temp 
						End If
						sTest = "cb"
						If DMCommon.Debug.Debug AndAlso oBalanceCalcArea.GroupBalanceArea IsNot Nothing Then
							For iGroupIndex As Integer = 0 To oBalanceCalcArea.GroupBalanceArea.Output.GetUpperBound(0)
								If diTopoID = iTestPgonId Then
									DMCommon.Debug.ExcelLog.SetNextValue(0, "Bal5", iGroupIndex, oBalanceCalcArea.GroupBalanceArea.SourceArea(iGroupIndex), oBalanceCalcArea.GroupBalanceArea.OutputItemFix(iGroupIndex))
								End If
							Next
						End If

						For iIndex As Integer = 0 To iArrayUB
							oaOverlayGroups(iIndex).CalcArea = oBalanceCalcArea.OutputItemFix(iIndex)
							If diTopoID = iTestPgonId Then
								DMCommon.Debug.ExcelLog.SetNextValue(0, "Bal2", oLotTopoIDList.Count, iIndex, oaOverlayGroups(iIndex).GroupID, oBalanceCalcArea.OutputItemFix(iIndex), daInput(iIndex), Me.LegalOrAcadArea(False))
							End If

							oaOverlayGroups(iIndex).RoundedArea = oBalanceRoundedArea.OutputItemFix(iIndex)
						Next

						sTest = "b"
					Catch oEx As Exception
						If Not bTesrErr Then
							bTesrErr = True
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(Me.TopoID) & ":" & Me.Name & vbCrLf & oEx.StackTrace & vbCrLf & bRegionExists.ToString() & vbCrLf & sTest, "TplnParcel - CalculateArea_148")
						End If
					End Try
				End If
			End If

		End Sub
		Public Sub CalculateAreaSource(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim iArrayUB As Integer = -1
			Dim dInputSum As Double
			Dim sTest As String = "a"
			Dim oTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)

			If oTopoIDList Is Nothing OrElse oTopoIDList.Count = 0 Then
				Return
			End If
			'	Protected diaOverlayTopoID(enOverlayIndex.OverlayIndexUB) As List(Of Integer)
			iArrayUB = oTopoIDList.Count - 1
			sTest = "aa"
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)

			Dim oaOverlayGroups(iArrayUB) As TplnOverlayGroup

			sTest = "ab"
			If oAllOverlayGroups IsNot Nothing Then

				If iArrayUB <> -1 Then
					sTest = "ag"
					Dim daInput(iArrayUB) As Double
					Dim laOutput(iArrayUB) As Long


					Dim iAreaIndex As Integer = 0
					sTest = "ah"
					Try
						For Each iLotTopoID As Integer In oTopoIDList
							oaOverlayGroups(iAreaIndex) = oAllOverlayGroups.GetItem(diTopoID, iLotTopoID)
							daInput(iAreaIndex) = oaOverlayGroups(iAreaIndex).AcadArea()
							dInputSum += daInput(iAreaIndex)
							iAreaIndex += 1
							''''''''''''''''''''''''//oAllOverlayGroups.GetItem(diTopoID, iLotTopoID).CalcArea = 3.3
						Next
						Dim oBalanceArea As BalanceArea = New BalanceArea(daInput, 1.0, Me.LegalOrAcadArea(False), False, "TplnParcel.CalculateAreaZZZ") ''''   temp

						For iIndex As Integer = 0 To iArrayUB
							oaOverlayGroups(iIndex).CalcArea = oBalanceArea.OutputItemFix(iIndex)
						Next
						sTest = "b"
					Catch oEx As Exception
						If Not bTesrErr Then
							bTesrErr = True
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - CalculateArea_148src")
						End If
					End Try
				End If
			End If

		End Sub

		Private Sub zzCalculateExproLotAreaNew(bCalcType As Boolean)
			'Dim iArrayUB As Integer = -1
			'	Dim iAreaIndex As Integer = 0
			'	Dim dInputSum As Double
			Dim tParcelArea As ParcelArea
			Dim oExproType As TplnExproType = Nothing
			'	

			'	iArrayUB = mdicExproTypes.Count - 1
			'Dim daInput(iArrayUB) As Double

			If TopoID = 114430 Then

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTypes2:", Me.BlockNo, ParcelNo, DMCommon.Debug.ColCount(mdicParcelExproTypes), DMCommon.Debug.ColCount(mcolOutPgons), mtaExproParcelArea.GetUpperBound(0))
			End If

			If mtaExproParcelArea IsNot Nothing Then
				For iIndex As Integer = 0 To mtaExproParcelArea.GetUpperBound(0)
					tParcelArea = mtaExproParcelArea(iIndex)
					If TopoID = 114430 Then
						DMCommon.Debug.ExcelLog.SetNextValue(3, "!+ExpType:", mtaExproParcelArea.GetUpperBound(0), tParcelArea.ExproType, tParcelArea.ConditionalArea, tParcelArea.LegalArea, mdicParcelExproTypes.Count, tParcelArea.LegalArea, mdicParcelExproTypes.ContainsKey(tParcelArea.ExproType))
					End If

					If mdicParcelExproTypes.TryGetValue(tParcelArea.ExproType, oExproType) Then
						If bCalcType Then
							oExproType.CalcArea = tParcelArea.ConditionalArea
						Else
							oExproType.CalcArea = tParcelArea.LegalArea
						End If
						'DMCommon.Debug.ExcelLog.SetNextValue(4, "!--ExpType:", tParcelArea.LegalArea, oExproType.CalcArea, oExproType.Landuses.Count)

						oExproType.CalculateArea()

					End If
				Next
			End If

		End Sub
		Private Sub zzCalculateExproLotAreaAAA()
			Dim iArrayUB As Integer = -1
			Dim iAreaIndex As Integer = 0
			Dim dInputSum As Double


			DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTypes3:", Me.BlockNo, ParcelNo, DMCommon.Debug.ColCount(mdicParcelExproTypes), DMCommon.Debug.ColCount(mcolOutPgons))

			iArrayUB = mdicParcelExproTypes.Count - 1
			Dim daInput(iArrayUB) As Double
			For Each oExproType As TplnExproType In mdicParcelExproTypes.Values
				daInput(iAreaIndex) = oExproType.AcadArea

				dInputSum += oExproType.AcadArea

				iAreaIndex += 1
			Next
			If dInputSum < Me.AcadArea(False) - 0.1 Then

				ReDim Preserve daInput(iAreaIndex)
				daInput(iAreaIndex) = Me.AcadArea(False) - dInputSum
			End If


			Dim oBalanceCalcArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.LegalOrAcadArea(False), True, "CalcExproLot1")    '   temp 
			Dim oBalanceRoundedArea As BalanceArea = New BalanceArea(daInput, TplnProject.CalcRoundFactor, Me.AcadArea(False), True, "CalcExproLot2")
			iAreaIndex = 0
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "ExproParcel!", BlockNo, ParcelNo, mdicExproTypes.Count, mcolOutPgons.Count)

			For Each oExproType As TplnExproType In mdicParcelExproTypes.Values

				oExproType.CalcArea = oBalanceCalcArea.OutputItemFix(iAreaIndex)
				oExproType.RoundedArea = oBalanceRoundedArea.OutputItemFix(iAreaIndex)
				oExproType.CalculateArea()

				If TopoID > 0 Then
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+ExproLot", mdicTypeOverlayGroups.Count, iAreaIndex, oExproType.ExproTypeID, oExproType.ExproTypeName, oExproType.Landuses.Count, oExproType.AcadArea, oBalanceCalcArea.OutputItemFix(iAreaIndex), oBalanceRoundedArea.OutputItemFix(iAreaIndex))
				End If
				iAreaIndex += 1
			Next

		End Sub
		Public ReadOnly Property Tolerance(bDun As Boolean) As Double
			Get
				If bDun Then
					Return mtParcelArea.ToleranceD
				Else
					Return mtParcelArea.ToleranceM
				End If

			End Get
		End Property
		Public ReadOnly Property DeltaArea(bDun As Boolean) As Double
			Get
				If bDun Then
					Return mtParcelArea.DeltaAreaD
				Else
					Return mtParcelArea.DeltaAreaM
				End If
				'	Return mtParcelArea.DeltaArea
			End Get
		End Property
		Public ReadOnly Property Deviation(bDun As Boolean) As Double
			Get
				If bDun Then
					Return mtParcelArea.Deviation * 0.001
				Else
					Return mtParcelArea.DeviationM
				End If
				'	Return mtParcelArea.Deviation
			End Get
		End Property
		Public ReadOnly Property ParcelArea() As ParcelArea
			Get
				Return mtParcelArea
			End Get
		End Property
		Public ReadOnly Property HasDeviation() As Boolean
			Get
				Return mtParcelArea.HasDeviation
			End Get
		End Property

		Public Sub CalculateArea(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, bRegionExists As Boolean)  'Balance
			Dim iArrayUB As Integer = -1
			Dim sTest As String = "a"

			'DMCommon.Debug.MsgBox("13_208", iOverlayMethod, iTopoPurpose, DMCommon.Debug.ColCount(mdicExproTypes), bRegionExists)
			If mdicTypeOverlayGroups.Count < 0 Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "??ParcelArea", TopoID, Name, iTopoPurpose, mdicTypeOverlayGroups.Count, DMCommon.Debug.ColCount(mdicParcelExproTypes))
			End If

			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
					Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
					zzCalculateArea(iOverlayIndex, bRegionExists)
				Case DMAcadExt.enTopoPurpose.Expro
					zzCalculateExproArea()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "??CalculateExproArea", TopoID, Name, iTopoPurpose, mdicTypeOverlayGroups.Count, DMCommon.Debug.ColCount(mdicExproTypes))
					zzCalculateExproAreaPlusOutPgons()
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "??End", TopoID, Name, miMaxOutPgonsCount)
					If mdicParcelExproTypes IsNot Nothing AndAlso mdicParcelExproTypes.Count > 0 Then
						zzCalculateExproLotAreaNew(True)
					End If

			End Select

			sTest = "aa"

			'Dim oaCurrentOverlayPgons As TplnOverlayPgons = doaOverlayPgons(iOverlayIndex)
			sTest = "ab"

			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExTypes+OutP:", Me.BlockNo, ParcelNo, DMCommon.Debug.ColCount(mdicExproTypes), DMCommon.Debug.ColCount(mcolOutPgons))
		End Sub
		Public Sub RecalcExproLot(bCalcType As Boolean)
			zzCalculateExproLotAreaNew(bCalcType)
		End Sub
		Public Sub AddLot(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iLotTopoID As Integer, ByVal bLotInPlan As Boolean, ByVal iRegionNo As Integer, ByVal iLanduseID As Integer)
			'+ Parcel
			Dim dicLanduses As TplnLanduses = Me.zzGetLanduseDic(iTopoPurpose)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			'	Dim iLanduseID As Integer = TplnProject.GetLanduseIDByLotID(iTopoPurpose, iLotTopoID)
			zzAddLot(iLotTopoID)
			zzAddRegion(iRegionNo)

			dicLanduses.AddLot(iTopoPurpose, iOverlayMethod, iLanduseID, iLotTopoID, bLotInPlan, iRegionNo, diTopoID)  ', diTopoID
		End Sub

		Public Sub AddOwnershipPgon(ByVal oOwnershipNote As TplnOwnershipNote, dArea As Double)
			'+ Parcel

			Dim oOwnNoteArea As TplnOwnNoteArea = New TplnOwnNoteArea(oOwnershipNote.Paragraph19, oOwnershipNote.Leasing, dArea)
			If mcolOwnNotePgons Is Nothing Then
				mcolOwnNotePgons = New ObjectModel.Collection(Of TplnOwnNoteArea)()
			End If
			mcolOwnNotePgons.Add(oOwnNoteArea)
			'	DMCommon.Debug.MsgBox("13_120f", Name, mbHasOwnershipNotes, Paragraph19AreaFieldName, oOwnershipNote.Paragraph19Area, oOwnershipNote.LeasingArea)

		End Sub
		Public Sub GetOwnershipNoteArea(ByRef dParagraph19Area As Double, ByRef dLeasingArea As Double, ByRef dOverlayArea As Double)
			dParagraph19Area = 0.0
			dLeasingArea = 0.0
			dOverlayArea = 0.0
			If mcolOwnNotePgons IsNot Nothing Then
				For Each oOwnershipNote As TplnOwnNoteArea In mcolOwnNotePgons
					dParagraph19Area += oOwnershipNote.GetParagraph19Area
					dLeasingArea += oOwnershipNote.GetLeasingArea
					dOverlayArea += oOwnershipNote.GetOverlayArea
				Next
			End If

		End Sub

		Public Overrides Sub AddOverlayPgon(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByRef oOverlayPgon As TplnOverlayPgon)
			MyBase.AddOverlayPgon(iTopoPurpose, iOverlayMethod, oOverlayPgon)
		End Sub
		Public Sub AddExproLusePolygon(iExproType As Integer, iLanduseID As Integer, dAcadArea As Double)
			'Dim iExproTopoID As Integer = oOverlayPgon.ExproTopoID
			'Dim iLotTopoID As Integer = oOverlayPgon.LotTopoID
			Dim oExproType As TplnExproType = Nothing
			Dim oLanduse As TplnLanduse = Nothing
			'	DMCommon.Debug.MsgBox("13_303", mdicExproTypes, mdicLandusesAppr, mhsExproTypes)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!AddExproLusePgon", iExproType, iLanduseID, dAcadArea)
			'3105
			If Not mdicParcelExproTypes.TryGetValue(iExproType, oExproType) Then

				oExproType = New TplnExproType(iExproType, diTopoID)
				mdicParcelExproTypes.Add(iExproType, oExproType)
				If miMaxExproType < iExproType Then
					miMaxExproType = iExproType
				End If
			End If

			If Not mdicLandusesAppr.TryGetValue(iLanduseID, oLanduse) Then
				oLanduse = New TplnLanduse(iLanduseID, DMAcadExt.enTopoPurpose.Approved)
				mdicLandusesAppr.Add(iLanduseID, oLanduse)
			End If

			'	DMCommon.Debug.ExcelLog.SetValueByCond(0, "!LanduseID_Name", True, oLanduse IsNot Nothing, oLanduse.ID, DMCommon.Functions.CStrN(oLanduse.Name, "LuseIsNth"))
			If oLanduse IsNot Nothing Then
				oLanduse.AddArea(dAcadArea)
				oExproType.AddExproLusePolygon(iLanduseID, dAcadArea)
				oExproType.TestLanduses()

			End If

			If Not mhsExproTypes.Contains(iExproType) Then
				mhsExproTypes.Add(iExproType)
			End If
		End Sub

		Public Sub AddExproLusePolygon(iExproType As Integer, iLanduseID As Integer, iBasicPlanID As Integer, dAcadArea As Double)
			'Dim iExproTopoID As Integer = oOverlayPgon.ExproTopoID
			'Dim iLotTopoID As Integer = oOverlayPgon.LotTopoID
			Dim oExproType As TplnExproType = Nothing
			Dim oLanduse As TplnLanduse = Nothing
			'	DMCommon.Debug.MsgBox("13_303", mdicExproTypes, mdicLandusesAppr, mhsExproTypes)
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!AddExproLusePgon", iExproType, iLanduseID, dAcadArea)
			'3105




			If Not mdicParcelExproTypes.TryGetValue(iExproType, oExproType) Then
				oExproType = New TplnExproType(iExproType, diTopoID)
				mdicParcelExproTypes.Add(iExproType, oExproType)
				If miMaxExproType < iExproType Then
					miMaxExproType = iExproType
				End If
			End If

			If Not mdicLandusesAppr.TryGetValue(iLanduseID, oLanduse) Then
				oLanduse = New TplnLanduse(iLanduseID, DMAcadExt.enTopoPurpose.Approved)
				mdicLandusesAppr.Add(iLanduseID, oLanduse)


			End If

			'	DMCommon.Debug.ExcelLog.SetValueByCond(0, "!LanduseID_Name", True, oLanduse IsNot Nothing, oLanduse.ID, DMCommon.Functions.CStrN(oLanduse.Name, "LuseIsNth"))
			If Me.TopoID = 99914984 Then
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!LanduseID_Name", iExproType, iLanduseID, dAcadArea)
			End If
			If oLanduse IsNot Nothing Then
				'DMCommon.Debug.MsgBoxLoop("B01_02", 3, BlockNo, ParcelNo, iLanduseID, iBasicPlanID, dAcadArea)
				oLanduse.AddArea(dAcadArea)
				oLanduse.AddBasicPlanArea(iBasicPlanID, dAcadArea)
				oExproType.AddExproLusePolygon(iLanduseID, dAcadArea)
				oExproType.AddExproLusePolygon(iLanduseID, iBasicPlanID, dAcadArea)


				oExproType.TestLanduses()

			End If

			If Not mhsExproTypes.Contains(iExproType) Then
				mhsExproTypes.Add(iExproType)
			End If
		End Sub


		Public Sub AddExproPgon(iExproType As Integer, sExproTypeName As String, dPgonArea As Double)

			Dim oTypeOverlayGroup As TypeOverlayGroup = Nothing

			If iExproType = 0 Then   'OrElse iExproType = 5

				mcolOutPgons.Add(New ParcelArea(dPgonArea))
			Else

				If mdicTypeOverlayGroups.TryGetValue(iExproType, oTypeOverlayGroup) Then
					oTypeOverlayGroup.AddAcadArea(dPgonArea)
				Else
					oTypeOverlayGroup = New TypeOverlayGroup(iExproType, sExproTypeName, dPgonArea)
					mdicTypeOverlayGroups.Add(iExproType, oTypeOverlayGroup)
				End If
			End If

			If TopoID = 15003 AndAlso iExproType <> 0 Then

				DMCommon.Debug.ExcelLog.SetNextValue(0, "!!LoadFDO_G", TopoID, Name, iExproType, iExproType, sExproTypeName, dPgonArea, oTypeOverlayGroup.AcadArea)

			End If

			'''''''''''''''''''	DMCommon.ExcelLog.SetNextValue(0, "ExproPgon", Me.TopoID, Me.BlockFull, Me.Name, Me.AcadArea(False), Me.LegalArea(False), mdicTypeOverlayGroups.Count, iExproType, dPgonArea)
			'   TplnParcel.vb:line 2296
			' ccc()
		End Sub

		Public Sub AddExproPgon(oExpro As TplnExpro, dPgonArea As Double)
			Dim tExproArea As ExproArea = New ExproArea(oExpro.ExproTypePrev, dPgonArea)
			Dim oTypeOverlayGroup As TypeOverlayGroup = Nothing
			DMCommon.Debug.ExcelLog.SetNextValue(0, "ExproPgon", Me.BlockFull, Me.Name, Me.AcadArea(False), Me.LegalArea(False), Me.TopoID, mdicTypeOverlayGroups.Count, oExpro.ExproTypeID, dPgonArea)

			mtExproRoundArea += tExproArea


			If mdicTypeOverlayGroups.TryGetValue(oExpro.ExproTypeID, oTypeOverlayGroup) Then
				oTypeOverlayGroup.AddAcadArea(dPgonArea)
			Else
				oTypeOverlayGroup = New TypeOverlayGroup(oExpro.ExproTypeID, oExpro.ExproTypeName, dPgonArea)
				mdicTypeOverlayGroups.Add(oExpro.ExproTypeID, oTypeOverlayGroup)
			End If

			'   TplnParcel.vb:line 2296
			' ccc()
		End Sub

		Public Sub AddDataToLanduseTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex '= UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			'		System.Windows.Forms.MessageBox.Show(CStr(mbaInPlan(iOverlayIndex)) & ":" & iOverlayIndex.ToString(), "01_013w")
			If zzIsInPlan(iTopoPurpose) Then

				Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, False)
				Dim dicLanduses As TplnLanduses = Me.zzGetLanduseDic(iTopoPurpose)
				Dim oNewRow As System.Data.DataRow
				Dim dSumMergeArea As Double = 0.0
				Dim dSumUnionArea As Double = 0.0
				Dim dCalcArea As Double
				Dim tInPlanAreaSet As TplnAreaSet
				Dim sTest As String
				If dicLanduses Is Nothing Then
					sTest = "Null"
				Else
					sTest = CStr(dicLanduses.Count)
				End If


				If dicLanduses Is Nothing Then
					Return
				End If
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay


				If False AndAlso miDebugCounterB < 11 Then
					DMAcadExt.AcadDocument.WriteMessageLog("InxB:" & iOverlayIndex.ToString() & " ^^Area:" & CStr(dtaInPlanAreaSet(iOverlayIndex).CalcArea) & ";" & CStr(dtaInPlanAreaSet(iOverlayIndex).AcadArea))

					miDebugCounterB += 1
				End If
				If oLanduseTable IsNot Nothing Then
					Try
						'	TplnProject.WriteMessageBox(iTopoPurpose.ToString() & ":" & iOverlayMethod.ToString(), "01_529Parcel")
						If miDebugCounterB < 0 Then
							MessageBox.Show(CStr(mtParcelData.Block) & "," & Me.BlockFull & "," & CStr(MyBase.Name) & ":" & CStr(dicLanduses.Count), "01_529Parcel")
						End If

						For Each oLanduse As TplnLanduse In dicLanduses.Values
							If oLanduse IsNot Nothing Then
								If bMerge Then
									oLanduse.Calculate(False, DMAcadExt.enOverlayMethod.Merge)
								End If
								If bFDO_Overlay Then
									oLanduse.Calculate(False, DMAcadExt.enOverlayMethod.FDO_Overlay)
								End If

								'oLanduse.Calculate(False, iOverlayMethod)
								'		If oLanduse.AcadArea(DMAcadExt.enOverlayMethod.Merge, iTopoPurpose) > 0.00001 Then

								oNewRow = oLanduseTable.NewRow()
								With oNewRow
									.Item(BlockFullFieldName) = Me.BlockFull


									.Item(NameFieldName) = Me.ExtName '   MyBase.Name
									.Item(LanduseIDFieldName) = oLanduse.ID
									.Item(LanduseNameFieldName) = TplnLot.GetLanduseNameNew2(iTopoPurpose, oLanduse.ID)
									'	DMCommon.Debug.MsgBox("190731_1", Me.LegalOrAcadArea(False), Me.LegalOrAcadArea(True))
									.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
									.Item(TopoReader.msAreaFldName) = MyBase.ddAcadArea

									If bMerge Then
										iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, iTopoPurpose)

										tInPlanAreaSet = dtaInPlanAreaSet(iOverlayIndex)
										.Item(msInPlanAreaMergeFieldName) = tInPlanAreaSet.AcadArea
										.Item(msInPlanCalcAreaMergeFieldName) = tInPlanAreaSet.CalcArea
										.Item(msInPlanCalcArea2MergeFieldName) = tInPlanAreaSet.CalcArea2
										.Item(msInPlanRoundedAreaMergeFieldName) = tInPlanAreaSet.RoundedArea



										tInPlanAreaSet = oLanduse.InPlanAreaSet(iOverlayIndex)
										.Item(msLUseInPlanAreaMergeFieldName) = tInPlanAreaSet.AcadArea
										.Item(msLUseInPlanCalcAreaMergeFieldName) = tInPlanAreaSet.CalcArea
										.Item(msLUseInPlanCalcArea2MergeFieldName) = tInPlanAreaSet.CalcArea2
										.Item(msLUseInPlanRoundedAreaMergeFieldName) = tInPlanAreaSet.RoundedArea
										dSumMergeArea += dCalcArea
									End If

									If bFDO_Overlay Then
										iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)



										tInPlanAreaSet = dtaInPlanAreaSet(iOverlayIndex)
										.Item(msInPlanAreaFDO_OverlayFieldName) = tInPlanAreaSet.AcadArea
										.Item(msInPlanCalcAreaFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
										.Item(msInPlanCalcArea2FDO_OverlayFieldName) = tInPlanAreaSet.CalcArea2
										.Item(msInPlanRoundedAreaFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea

										tInPlanAreaSet = oLanduse.InPlanAreaSet(iOverlayIndex)
										If False AndAlso miDebugCounterB < 11 Then
											DMAcadExt.AcadDocument.WriteMessageLog("Inx:" & iOverlayIndex.ToString() & " Area:" & CStr(tInPlanAreaSet.CalcArea) & ";" & CStr(dtaInPlanAreaSet(iOverlayIndex).AcadArea))
											'DMCommon.Functions.DispArray(saBlockAttribTag, "01_551d", True)
											miDebugCounterB += 1
										End If
										.Item(msLUseInPlanAreaFDO_OverlayFieldName) = tInPlanAreaSet.AcadArea
										.Item(msLUseInPlanCalcAreaFDO_OverlayFieldName) = tInPlanAreaSet.CalcArea
										.Item(msLUseInPlanCalcArea2FDO_OverlayFieldName) = tInPlanAreaSet.CalcArea2
										.Item(msLUseInPlanRoundedAreaFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea
									End If
									.Item(BlockFieldName) = Me.BlockNo
									.Item(BlockAddFieldName) = Me.BlockAdd
									.Item(ParcelOrderFieldName) = MyBase.diOrder
									.Item(LanduseOrderFieldName) = oLanduse.Order
								End With
								oLanduseTable.Rows.Add(oNewRow)
							End If
							If MyBase.TopoID = 73450 Then
								'	MCommon.ExcelLogAW5.SetNextValue(4, "DataToLuseTable", Me.ExtName, oLanduse.ID, oLanduse.ID, TplnLot.GetLanduseNameNew2(iTopoPurpose, oLanduse.ID))

							End If

						Next

						With moCalcArea
							If iTopoPurpose = enTopoPurpose.Proposed Then
								.Item(DMAcadExt.enOverlayIndex.PropMerge) += dSumMergeArea
								.Item(DMAcadExt.enOverlayIndex.PropUnion) += dSumUnionArea
							ElseIf iTopoPurpose = enTopoPurpose.Approved Then
								.Item(DMAcadExt.enOverlayIndex.ApprMerge) += dSumMergeArea
								.Item(DMAcadExt.enOverlayIndex.ApprUnion) += dSumUnionArea
							End If
						End With
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - AddDataToLanduseTableII")
					End Try
				Else
					System.Windows.Forms.MessageBox.Show("oLanduseTable Is Nothing", "AddDataToLanduseTable - " & Me.Name)
				End If
			Else
				'	MessageBox.Show("???", "05_505")
			End If
		End Sub

		Public Sub AddDataToLanduseRegionTable(ByVal bFDO_Overlay As Boolean, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, iRegion As Integer)
			Dim bMerge As Boolean = False



			Dim iOverlayIndex As DMAcadExt.enOverlayIndex '= UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			'		System.Windows.Forms.MessageBox.Show(CStr(mbaInPlan(iOverlayIndex)) & ":" & iOverlayIndex.ToString(), "01_013w")
			If True Then ' zzIsInPlan(iTopoPurpose)

				Dim oLanduseTable As System.Data.DataTable = zzGetLanduseTable(iTopoPurpose, True)
				Dim dicLanduses As TplnLanduses = Me.zzGetLanduseDic(iTopoPurpose)
				Dim oNewRow As System.Data.DataRow
				Dim dSumMergeArea As Double = 0.0
				Dim dSumUnionArea As Double = 0.0
				Dim dCalcArea As Double
				Dim tInPlanAreaSet As TplnAreaSet
				Dim tRegionAreaSet As TplnAreaSet

				Dim sTest As String
				If dicLanduses Is Nothing Then
					sTest = "Null"
				Else
					sTest = CStr(dicLanduses.Count)
				End If


				If dicLanduses Is Nothing Then
					Return
				End If
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay

				'     Dim iRow As Integer

				If oLanduseTable IsNot Nothing Then
					Try
						'	TplnProject.WriteMessageBox(iTopoPurpose.ToString() & ":" & iOverlayMethod.ToString(), "01_529Parcel")
						'TplnParcel.vb:line 5249


						For Each oLanduse As TplnLanduse In dicLanduses.Values
							If oLanduse IsNot Nothing Then

								'	DMCommon.Debug.ExcelLog.SetNextValue(0, "TplnParcel", "AddDataToLanduseRegionTable", oLanduse.ID, oLanduse.Name, DMCommon.Debug.ColCount(oLanduse.Lots(DMAcadExt.enTopoPurpose.Approved)), TopoID, BlockNo, Name, iRegion)

								If oLanduse.ContainRegion(iTopoPurpose, iRegion) Then

									oLanduse.Calculate(False, DMAcadExt.enOverlayMethod.FDO_Overlay, iRegion)

									oNewRow = oLanduseTable.NewRow()
									With oNewRow
										.Item(BlockFullFieldName) = Me.BlockFull


										.Item(NameFieldName) = Me.ExtName '   MyBase.Name
										.Item(LanduseIDFieldName) = oLanduse.ID
										.Item(LanduseNameFieldName) = TplnLot.GetLanduseNameNew2(iTopoPurpose, oLanduse.ID)
										.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
										.Item(TopoReader.msAreaFldName) = MyBase.ddAcadArea

										If bMerge Then
											iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.Merge, iTopoPurpose)

											tInPlanAreaSet = dtaInPlanAreaSet(iOverlayIndex)
											.Item(msInPlanAreaMergeFieldName) = tInPlanAreaSet.AcadArea
											.Item(msInPlanCalcAreaMergeFieldName) = tInPlanAreaSet.CalcArea
											.Item(msInPlanCalcArea2MergeFieldName) = tInPlanAreaSet.CalcArea2
											.Item(msInPlanRoundedAreaMergeFieldName) = tInPlanAreaSet.RoundedArea



											tInPlanAreaSet = oLanduse.InPlanAreaSet(iOverlayIndex)
											.Item(msLUseInPlanAreaMergeFieldName) = tInPlanAreaSet.AcadArea
											.Item(msLUseInPlanCalcAreaMergeFieldName) = tInPlanAreaSet.CalcArea
											.Item(msLUseInPlanCalcArea2MergeFieldName) = tInPlanAreaSet.CalcArea2
											.Item(msLUseInPlanRoundedAreaMergeFieldName) = tInPlanAreaSet.RoundedArea
											dSumMergeArea += dCalcArea
										End If

										If bFDO_Overlay Then
											iOverlayIndex = UnionPgonArea.GetOverlayIndex(DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)



											tInPlanAreaSet = dtaInPlanAreaSet(iOverlayIndex)
											.Item(msInPlanAreaFDO_OverlayFieldName) = tInPlanAreaSet.AcadArea
											.Item(msInPlanCalcAreaFDO_OverlayFieldName) = tInPlanAreaSet.CalcGroupArea
											.Item(msInPlanCalcArea2FDO_OverlayFieldName) = tInPlanAreaSet.CalcGroupArea2
											.Item(msInPlanRoundedAreaFDO_OverlayFieldName) = tInPlanAreaSet.RoundedArea

											tRegionAreaSet = oLanduse.RegionAreaSet(iOverlayIndex)
											If False AndAlso miDebugCounterB < 11 Then
												DMAcadExt.AcadDocument.WriteMessageLog("Inx:" & iOverlayIndex.ToString() & " Area:" & CStr(tInPlanAreaSet.CalcArea) & ";" & CStr(dtaInPlanAreaSet(iOverlayIndex).AcadArea))
												'DMCommon.Functions.DispArray(saBlockAttribTag, "01_551d", True)
												miDebugCounterB += 1
											End If


											.Item(msLUseInPlanAreaFDO_OverlayFieldName) = tRegionAreaSet.AcadArea
											.Item(msLUseInPlanCalcAreaFDO_OverlayFieldName) = tRegionAreaSet.CalcArea
											.Item(msLUseInPlanCalcArea2FDO_OverlayFieldName) = tRegionAreaSet.CalcGroupArea
											.Item(msLUseInPlanRoundedAreaFDO_OverlayFieldName) = tRegionAreaSet.RoundedArea
										End If
										.Item(BlockFieldName) = Me.BlockNo
										.Item(BlockAddFieldName) = Me.BlockAdd
										.Item(ParcelOrderFieldName) = MyBase.diOrder
										.Item(LanduseOrderFieldName) = oLanduse.Order
									End With
									oLanduseTable.Rows.Add(oNewRow)
								End If
							End If
						Next
						With moCalcArea
							If iTopoPurpose = enTopoPurpose.Proposed Then
								.Item(DMAcadExt.enOverlayIndex.PropMerge) += dSumMergeArea
								.Item(DMAcadExt.enOverlayIndex.PropUnion) += dSumUnionArea
							ElseIf iTopoPurpose = enTopoPurpose.Approved Then
								.Item(DMAcadExt.enOverlayIndex.ApprMerge) += dSumMergeArea
								.Item(DMAcadExt.enOverlayIndex.ApprUnion) += dSumUnionArea
							End If
						End With
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - AddDataToLanduseRegionTable")
					End Try
				Else
					System.Windows.Forms.MessageBox.Show("oLanduseTable Is Nothing", "AddDataToReginLanduseTable - " & Me.Name)
				End If
			Else
				'	MessageBox.Show("???", "05_505")
			End If
		End Sub

		Private Sub zzAddDataToPolygonTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex)
			Dim oPolygonTable As System.Data.DataTable = zzGetPolygonTable(iOverlayIndex)
			Dim dicOverlayPgons As TplnOverlayPgons = Me.GetOverlayPgons(iOverlayIndex)
			Dim oNewRow As System.Data.DataRow
			Dim oLot As TplnLot
			If dicOverlayPgons IsNot Nothing Then
				Try
					For Each oOveralyPgon As TplnOverlayPgon In dicOverlayPgons.Values
						oNewRow = oPolygonTable.NewRow()
						With oNewRow
							.Item(BlockFullFieldName) = Me.BlockFull

							.Item(BlockFieldName) = Me.BlockNo
							.Item(BlockAddFieldName) = Me.BlockAdd

							.Item(NameFieldName) = MyBase.Name
							If oOveralyPgon.LotTopoID <> 0 Then
								oLot = TplnProject.GetLot(iTopoPurpose, oOveralyPgon.LotTopoID)
								If oLot IsNot Nothing Then
									.Item(msLotNameFieldName) = oLot.Name
									.Item(msInPlanFieldName) = oLot.InPlan
									.Item(LanduseIDFieldName) = oLot.LanduseID

									.Item(LanduseNameFieldName) = oLot.LanduseName   'TplnLot.GetLanduseNameNew2(iTopoPurpose, oLot.LanduseID)
									.Item(msLotAreaFieldName) = Math.Round(oLot.AcadArea(False), 3)
									.Item(msLotDifAreaFieldName) = DMCommon.Functions.RelRound(oLot.GetDifArea(iOverlayIndex), 3)
									.Item(TopoReader.msLotTopoIDFldName) = oLot.TopoID
									.Item(msLotOrderFieldName) = oLot.Order
								End If
							End If
							.Item(PgonAreaFieldName) = Math.Round(oOveralyPgon.AcadArea(False), 3)
							.Item(TopoReader.msAreaFldName) = Math.Round(MyBase.ddAcadArea, 3)
							.Item(msParcelDifAreaFieldName) = DMCommon.Functions.RelRound(MyBase.GetDifArea(iOverlayIndex), 3)

							'	.Item(msPlanStateFieldName) = Me.PlanState
							.Item(msPlanStateTextFieldName) = Me.PlanStateText(iOverlayIndex)
							.Item(TopoReader.msCentroidXFldName) = oOveralyPgon.CentroidX
							.Item(TopoReader.msCentroidYFldName) = oOveralyPgon.CentroidY
							.Item(TopoReader.msTopoIDFldName) = oOveralyPgon.TopoID
							.Item(TopoReader.msParcelTopoIDFldName) = Me.TopoID

							.Item(TopoReader.msAcObjIDFldName) = oOveralyPgon.CentroidAcObjID ''''''oOveralyPgon.CentroidAcObjID.OldIdPtr.ToInt64

							.Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
							.Item(ParcelOrderFieldName) = MyBase.diOrder
						End With
						oPolygonTable.Rows.Add(oNewRow)
					Next
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddDataToPolygonTable")
				End Try
			Else
				'''''''''''	TEMP DMAcadExt.AcadDocument.WriteMessageLog("dicUnionPgons Is  Nothing : " & iOverlayIndex.ToString() & vbCrLf & "ID=" & CStr(diTopoID) & "; No=" & dsName, "TplnParcel - 01_260")
			End If
		End Sub

		Public Sub AddDataToPolygonTables(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			Dim bOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
				If bOverlayArray(iOverlayIndex) Then
					zzAddDataToPolygonTable(iOverlayIndex)
				End If
			Next
		End Sub
		Public Shared Sub CreateExproTables()
			zzCreateExproTable()
			zzCreateExproTableByCol()
			zzCreateExproTablePgons()
		End Sub
		Public Shared ReadOnly Property OwnerTotalAreas As Dictionary(Of Integer, Double)
			Get
				Return mdicOwnerTotalAreas
			End Get
		End Property
		Public Shared Property OwnerTotalArea As Double
			Get
				Return maOwnerTotalArea
			End Get
			Set(dValue As Double)
				maOwnerTotalArea = dValue
			End Set
		End Property

		Public Shared ReadOnly Property ExproTypes As HashSet(Of Integer)
			Get
				Return mhsExproTypes
			End Get
		End Property
		Public Sub ExproLuseReport(ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, dicExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType))   '_310520
			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iCurrentRow
			Dim iRowNumber As Integer = 0
			Dim iTypeIndex As Integer
			'Dim tRect As System.Drawing.Rectangle
			'	Dim oRange As Range
			Dim iRow As Integer
			Dim daSumLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim daPartLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim oExproType As TplnExproType = Nothing
			Dim dicLanuseIndecis As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			'Dim dInArea As Double
			'oExcelAppExt.SetValueInRow(iRow, 0, BlockFull, Name, LegalArea(False))
			If mdicLandusesAppr.Count > 0 Then
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oLanduse.AreaSet.AcadArea
					iRowNumber += 1
				Next

				oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False), dInArea)

				iRowNumber = 0
				If True Then
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						'	If oLanduse.AreaSet.AcadArea > 0.000001 Then
						sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
						oLanduse.Name = sLanduseName
						'''''a'''''''''''''''''''''''''''''''''oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 4, oLanduse.ID, sLanduseName, oLanduse.AreaSet.AcadArea)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 4, oLanduse.ID, sLanduseName, oLanduse.AreaSet.CalcArea)

						dicLanuseIndecis.Add(oLanduse.ID, iRowNumber)
						iRowNumber += 1
						'	End If
					Next
				End If

				iCurrentRow += iRowNumber
				'	oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)
				If True Then  '030919
					Dim oLanduse As TplnLanduse = Nothing
					Dim iLanduseIndex As Integer
					dInArea = 0.0
					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					'Dim oExproType As TplnExproType = Nothing
					For Each oExproExproType As TPlanGraph.TplnExpro.ExproType In dicExproTypes.Values
						If TplnParcel.ExproTypes.Contains(oExproExproType.ID) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(oExproExproType.ID, oExproType) Then
								iRow = iFirstRow
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTArea", Me.TopoID, Me.BlockNo, Me.ParcelNo, dicExproTypes.Count, oExproType.CalcArea)
								oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 3 + 5 * iTypeIndex, oExproType.CalcArea)
								dInArea += oExproType.CalcArea
								For Each oExproTypeLanduse As TplnLanduse In oExproType.Landuses.Values
									'	oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, zzCalcPart(oExproTypeLanduse.AreaSet.CalcArea, oExproType.CalcArea))
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, oExproTypeLanduse.AreaSet.Part)
									iRow += 1
									If dicLanuseIndecis.TryGetValue(oExproTypeLanduse.ID, iLanduseIndex) Then
										daSumLanduseArea(iLanduseIndex) += oExproTypeLanduse.AreaSet.CalcArea
									End If
								Next
							End If
						End If
					Next

					If False Then  '100121
						For Each oLanduse1 As TplnLanduse In mdicLandusesAppr.Values
							'	If oLanduse.AreaSet.AcadArea > 0.000001 Then
							sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse1.ID)
							oLanduse1.Name = sLanduseName
							oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 4, oLanduse1.ID, sLanduseName, oLanduse1.AreaSet.AcadArea)
							dicLanuseIndecis.Add(oLanduse1.ID, iRowNumber)
							iRowNumber += 1
							'	End If
						Next
					End If
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BefPart", Me.TopoID, Me.BlockNo, Me.ParcelNo, mdicExproTypes.Count, dInArea)
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						daPartLanduseArea(iIndex) = daSumLanduseArea(iIndex) * 100.0 / dInArea
					Next
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!PartLuseArea", daPartLanduseArea)
					Dim oBalancePart As BalanceArea = New BalanceArea(daPartLanduseArea, 10.0, 100.0, True, "ExproUseRep")

					oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

					iRowNumber = 0
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))  '
						iRowNumber += 1
					Next

				End If

			End If

		End Sub
		Public Sub ExproLuseReport_0126(ByVal bAcadArea As Boolean, ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, dicGlobalExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType))   '_310520

			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iCurrentRow
			Dim iRowNumber As Integer = 0
			Dim iTypeIndex As Integer
			Dim iRow As Integer
			Dim daSumLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim daPartLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim oExproType As TplnExproType = Nothing
			Dim dicLanuseIndecis As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim oTypeLanduse As TplnLanduse = Nothing
			Dim iTest1 As Integer
			Dim sRectAddress As String
			Dim iExproTypesCount As Integer = dicGlobalExproTypes.Count
			Dim iLanduseCount As Integer = mdicLandusesAppr.Count
			Dim sDividendAddress As String
			Dim sDivisorAddress As String
			Dim bLuseInExproExists As Boolean
			Dim iTestA As Integer
			'	oExcelAppExt.SetValueInRow(iFirstRow, 15, "!Debug00", mdicLandusesAppr.Count, dicGlobalExproTypes.Count)
			If mdicLandusesAppr.Count > 0 Then
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oLanduse.AreaSet.AcadArea
					iRowNumber += 1
				Next
				For Each iExproTypeID As Integer In dicGlobalExproTypes.Keys
					'	oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA0", iExproTypeID, dicGlobalExproTypes.Count)
					iTestA += 4
				Next
				For Each iExproTypeID As Integer In mdicParcelExproTypes.Keys
					'	oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA1", iExproTypeID, mdicParcelExproTypes.Count)
					iTestA += 4
				Next


				iRowNumber = 0
				iRowNumber = 0
				If True Then
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						iTypeIndex = 0

						bLuseInExproExists = False

						For Each oGlobalExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
							iTypeIndex += 1
							'	oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 34 + iTypeIndex * 4, "!Debug11", oGlobalExproType.ID)
							If mdicParcelExproTypes.TryGetValue(oGlobalExproType.ID, oExproType) Then
								''''oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Debug01", oLanduse Is Nothing, oExproType Is Nothing)
								''''oExcelAppExt.CurrentRow = iFirstRow + iRowNumber
								''''oExcelAppExt.SetEnumerable(16 + 12 * iTypeIndex, "!Debug11", oExproType.LanduseArray)
								If oExproType.TryGetValue(oLanduse.ID, oTypeLanduse) AndAlso oTypeLanduse IsNot Nothing Then
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 26, "!Debug02", oLanduse Is Nothing, oTypeLanduse Is Nothing, oExproType Is Nothing)


									If bAcadArea Then
										''_0126  oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
									Else
										''_0126 oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.CalcArea)
									End If



									bLuseInExproExists = True
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 3, oTypeLanduse.ID)
								Else

									If mdicParcelExproTypes Is Nothing Then
										iTest1 = -1
									Else
										iTest1 = mdicParcelExproTypes.Count
									End If
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Dbg1", iTest1, iTypeIndex, oTypeLanduse IsNot Nothing, oLanduse.ID, oExproType.LanduseList, oExproType.LandusesCount, oGlobalExproType.ID)
								End If
							Else

								'Debug
								'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 25, "!Dbg2", mdicParcelExproTypes.Count, iTypeIndex, oGlobalExproType.ID)
							End If
						Next
						DMCommon.Debug.MsgBox("140126_1a")
						If bLuseInExproExists Then
							sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
							oLanduse.Name = sLanduseName

							''_0126 oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName)


							iRowNumber += 1
						End If
						sDividendAddress = oExcelAppExt.GetCellAddress(iFirstRow, iExproTypesCount + 5)
						sDivisorAddress = oExcelAppExt.GetCellAddress(iFirstRow, 2)

						''_0126	oExcelAppExt.SetFormulaInRow(iFirstRow, 6 + iExproTypesCount, False, "=" & sDividendAddress & "/" & sDivisorAddress & "*100")
						''_0126	oExcelAppExt.SetFormulaInRow(iFirstRow, 8 + iExproTypesCount, False, "=" & sDivisorAddress & "-" & sDividendAddress)


						dicLanuseIndecis.Add(oLanduse.ID, iRowNumber)

						'	End If
					Next
					''_0126	oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))   ''''''''090322  
					''_0126	sRectAddress = oExcelAppExt.GetRectangleAddress(iFirstRow, 3, iRowNumber, iExproTypesCount)
					''_0126	oExcelAppExt.SetFormulaInRow(iFirstRow, 5 + iExproTypesCount, False, "=SUM(" & sRectAddress & ")")

				End If

				DMCommon.Debug.MsgBox("140126_2")
				iCurrentRow += iRowNumber
				'	oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

				''///////////////////////////////////////////////////////////////////////////////////////////////////////////
				If False Then  ' 010421    '030919
					Dim oLanduse As TplnLanduse = Nothing
					Dim iLanduseIndex As Integer
					dInArea = 0.0
					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					'Dim oExproType As TplnExproType = Nothing
					For Each oExproExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
						If TplnParcel.ExproTypes.Contains(oExproExproType.ID) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(oExproExproType.ID, oExproType) Then
								iRow = iFirstRow
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTArea", Me.TopoID, Me.BlockNo, Me.ParcelNo, dicExproTypes.Count, oExproType.CalcArea)
								''_0126 oExcelAppExt.SetValueInRow(iRowNumber, 2 + iTypeIndex, oExproType.CalcArea)
								dInArea += oExproType.CalcArea
								For Each oExproTypeLanduse As TplnLanduse In oExproType.Landuses.Values
									'	oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, zzCalcPart(oExproTypeLanduse.AreaSet.CalcArea, oExproType.CalcArea))
									''_0126	oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, oExproTypeLanduse.AreaSet.Part)
									iRow += 1
									If dicLanuseIndecis.TryGetValue(oExproTypeLanduse.ID, iLanduseIndex) Then
										daSumLanduseArea(iLanduseIndex) += oExproTypeLanduse.AreaSet.CalcArea
									End If
								Next
							End If
						End If
					Next
					DMCommon.Debug.MsgBox("140126_3")
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BefPart", Me.TopoID, Me.BlockNo, Me.ParcelNo, mdicExproTypes.Count, dInArea)
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						daPartLanduseArea(iIndex) = daSumLanduseArea(iIndex) * 100.0 / dInArea
					Next
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!PartLuseArea", daPartLanduseArea)
					Dim oBalancePart As BalanceArea = New BalanceArea(daPartLanduseArea, 10.0, 100.0, True, "ExproUseRep")

					''_0126	oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

					iRowNumber = 0
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						''_0126	oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))  '
						iRowNumber += 1
					Next

				End If
				'''''''''''''/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			End If

		End Sub

		Public Sub ExproLuseReport_0226(ByVal bAcadArea As Boolean, ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, dicGlobalExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType))   '_310520
			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iCurrentRow
			Dim iRowNumber As Integer = 0
			Dim iLanduseIndex As Integer = 0
			Dim iTypeIndex As Integer

			Dim iRow As Integer
			Dim daSumLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim daPartLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim oExproType As TplnExproType = Nothing
			Dim dicLanuseIndecis As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim oTypeLanduse As TplnLanduse = Nothing
			Dim iTest1 As Integer
			Dim sRectAddress As String
			Dim iExproTypesCount As Integer = dicGlobalExproTypes.Count
			Dim iLanduseCount As Integer = mdicLandusesAppr.Count
			Dim sDividendAddress As String
			Dim sDivisorAddress As String
			Dim bLuseInExproExists As Boolean
			Dim iTestA As Integer


			'oExcelAppExt.SetValueInRow(iFirstRow, 15, "!Debug00", mdicLandusesAppr.Count, dicGlobalExproTypes.Count)
			If mdicLandusesAppr.Count > 0 Then
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oLanduse.AreaSet.AcadArea
					iRowNumber += 1
				Next
				If False Then  '280126
					For Each iExproTypeID As Integer In dicGlobalExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA0", iExproTypeID, dicGlobalExproTypes.Count)
						iTestA += 4
					Next
					For Each iExproTypeID As Integer In mdicParcelExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA1", iExproTypeID, mdicParcelExproTypes.Count)
						iTestA += 4
					Next
				End If
				iRowNumber = 0
				If True Then
					Dim iKey As Integer
					Dim dArea As Double
					Dim oPlan As TplnPlan
					Dim sPlanName As String = ""
					Dim hsPlanNames As HashSet(Of String) = New HashSet(Of String)()
					Dim bFirstPlan As Boolean
					'DMCommon.Debug.MsgBox("13_321", DMCommon.Debug.ColCount(mdicLandusesAppr), DMCommon.Debug.ColCount(dicGlobalExproTypes))
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						iTypeIndex = 0
						bLuseInExproExists = False
						hsPlanNames.Clear()

						For Each oGlobalExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
							iTypeIndex += 1
							'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 34 + iTypeIndex * 4, "!Debug11", oGlobalExproType.ID)
							If mdicParcelExproTypes.TryGetValue(oGlobalExproType.ID, oExproType) Then
								'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Debug01", oLanduse Is Nothing, oExproType Is Nothing)
								oExcelAppExt.CurrentRow = iFirstRow + iRowNumber
								'oExcelAppExt.SetEnumerable(16 + 12 * iTypeIndex, "!Debug11", oExproType.LanduseArray)
								If oExproType.TryGetValue(oLanduse.ID, oTypeLanduse) AndAlso oTypeLanduse IsNot Nothing Then
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 26, "!Debug02", oLanduse Is Nothing, oTypeLanduse Is Nothing, oExproType Is Nothing)
									'DMCommon.Debug.MsgBox("13_322", DMCommon.Debug.ColCount(oTypeLanduse.BasicPlanAreas), BlockNo, ParcelNo)

									If oTypeLanduse.BasicPlanAreas.Count > 0 Then

										For iPlanIndex As Integer = 0 To oTypeLanduse.BasicPlanAreas.Count - 1
											iKey = oTypeLanduse.BasicPlanAreas.Keys(iPlanIndex)
											dArea = oTypeLanduse.BasicPlanAreas.Values(iPlanIndex)
											oPlan = TPlanGraph.TplnProject.GetPlan(iKey)
											If oPlan Is Nothing Then
												sPlanName = ""
											Else
												sPlanName = "'" & oPlan.Name
											End If
											If Not hsPlanNames.Contains(sPlanName) Then
												hsPlanNames.Add(sPlanName)
											End If
										Next iPlanIndex
										'yyyyy
									Else
										iKey = 0
										dArea = 999.99
										sPlanName = ""
									End If
									'DMCommon.Debug.MsgBox("C06_01", bAcadArea, bLuseInExproExists, "oTypeLanduse.BasicPlanAreas.Count=", oTypeLanduse.BasicPlanAreas.Count, iLanduseIndex, oGlobalExproType, iTypeIndex, oExcelAppExt.CurrentRow, sPlanName, oTypeLanduse.BasicPlanAreas.Count, oTypeLanduse.AreaSet.AcadArea, oTypeLanduse.AreaSet.CalcArea, dArea)
									If bAcadArea Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
									Else
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.CalcArea)
									End If
									oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 12 + 4 * iTypeIndex, oTypeLanduse.BasicPlanAreas.Count, iKey, dArea, sPlanName)


									If False Then
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 16 + 2 + iTypeIndex, iTypeIndex, Nothing, oTypeLanduse.AreaSet.CalcArea, Nothing, "xx")
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 24 + 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea, Nothing, "yy")
									End If
									bLuseInExproExists = True
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 3, oTypeLanduse.ID)
								Else

									If mdicParcelExproTypes Is Nothing Then
										iTest1 = -1
									Else
										iTest1 = mdicParcelExproTypes.Count
									End If
									If False Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Dbg1", iTest1, iTypeIndex, oTypeLanduse IsNot Nothing, oLanduse.ID, oExproType.LanduseList, oExproType.LandusesCount, oGlobalExproType.ID)
									End If
								End If
							Else

								'Debug
								If False Then '280126
									oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 25, "!Dbg2", mdicParcelExproTypes.Count, iTypeIndex, oGlobalExproType.ID)
								End If

							End If
						Next oGlobalExproType

						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 33, "Landuse.Name", bLuseInExproExists, oLanduse.ID, TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Parcel, oLanduse.ID), TplnLot.Landuses.Count)

						If bLuseInExproExists Then
							sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
							oLanduse.Name = sLanduseName
							'	oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName, sPlanName) 'yyyyy
							oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName) 'yyyyy
							bFirstPlan = True
							'DMCommon.Debug.MsgBox("C06_02", hsPlanNames.Count, iRowNumber)
							For Each sPlanNameA As String In hsPlanNames
								If Not bFirstPlan Then
									iRowNumber += 1
								End If
								oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 4, sPlanNameA)

								bFirstPlan = False
							Next


							If False Then
								''''''''02032
								If oTypeLanduse.BasicPlanAreas.Count > 0 Then
									iKey = oTypeLanduse.BasicPlanAreas.Keys(0)
									dArea = oTypeLanduse.BasicPlanAreas.Values(0)
									oPlan = TPlanGraph.TplnProject.GetPlan(iKey)
									If oPlan Is Nothing Then
										sPlanName = ""
									Else
										sPlanName = "'" & oPlan.Name
									End If

								Else
									iKey = 0
									dArea = 999.99
									sPlanName = ""
								End If
								oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 4, sPlanName)
							End If
							iRowNumber += 1
						End If
						sDividendAddress = oExcelAppExt.GetCellAddress(iFirstRow, iExproTypesCount + 5)
						sDivisorAddress = oExcelAppExt.GetCellAddress(iFirstRow, 2)

						oExcelAppExt.SetFormulaInRow(iFirstRow, 6 + iExproTypesCount, False, "=" & sDividendAddress & "/" & sDivisorAddress & "*100")
						oExcelAppExt.SetFormulaInRow(iFirstRow, 8 + iExproTypesCount, False, "=" & sDivisorAddress & "-" & sDividendAddress)


						dicLanuseIndecis.Add(oLanduse.ID, iRowNumber)
						iLanduseIndex += 1
						'	End If
					Next oLanduse
					'''''''DMCommon.Debug.MsgBox("!SetValueInRowByCol", iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))
					oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))   ''''''''090322  
					sRectAddress = oExcelAppExt.GetRectangleAddress(iFirstRow, 3, iRowNumber, iExproTypesCount)
					oExcelAppExt.SetFormulaInRow(iFirstRow, 5 + iExproTypesCount, False, "=SUM(" & sRectAddress & ")")

				End If


				iCurrentRow += iRowNumber
				'oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

				''///////////////////////////////////////////////////////////////////////////////////////////////////////////
				If False Then  ' 010421    '030919
					Dim oLanduse As TplnLanduse = Nothing

					dInArea = 0.0
					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					'Dim oExproType As TplnExproType = Nothing
					For Each oExproExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
						If TplnParcel.ExproTypes.Contains(oExproExproType.ID) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(oExproExproType.ID, oExproType) Then
								iRow = iFirstRow
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTArea", Me.TopoID, Me.BlockNo, Me.ParcelNo, dicExproTypes.Count, oExproType.CalcArea)
								oExcelAppExt.SetValueInRow(iRowNumber, 2 + iTypeIndex, oExproType.CalcArea)
								dInArea += oExproType.CalcArea
								For Each oExproTypeLanduse As TplnLanduse In oExproType.Landuses.Values
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, zzCalcPart(oExproTypeLanduse.AreaSet.CalcArea, oExproType.CalcArea))
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, oExproTypeLanduse.AreaSet.Part)
									iRow += 1
									If dicLanuseIndecis.TryGetValue(oExproTypeLanduse.ID, iLanduseIndex) Then
										daSumLanduseArea(iLanduseIndex) += oExproTypeLanduse.AreaSet.CalcArea
									End If
								Next
							End If
						End If
					Next

					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BefPart", Me.TopoID, Me.BlockNo, Me.ParcelNo, mdicExproTypes.Count, dInArea)
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						daPartLanduseArea(iIndex) = daSumLanduseArea(iIndex) * 100.0 / dInArea
					Next
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!PartLuseArea", daPartLanduseArea)
					Dim oBalancePart As BalanceArea = New BalanceArea(daPartLanduseArea, 10.0, 100.0, True, "ExproUseRep")

					'oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

					iRowNumber = 0
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))  '
						iRowNumber += 1
					Next

				End If
				'''''''''''''/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			End If

		End Sub

		Public Sub ExproLuseReport_0321(ByVal bAcadArea As Boolean, ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, dicGlobalExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType))   '_310520

			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iCurrentRow
			Dim iRowNumber As Integer = 0
			Dim iTypeIndex As Integer
			Dim iRow As Integer
			Dim daSumLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim daPartLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim oExproType As TplnExproType = Nothing
			Dim dicLanuseIndecis As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim oTypeLanduse As TplnLanduse = Nothing
			Dim iTest1 As Integer
			Dim sRectAddress As String
			Dim iExproTypesCount As Integer = dicGlobalExproTypes.Count
			Dim iLanduseCount As Integer = mdicLandusesAppr.Count
			Dim sDividendAddress As String
			Dim sDivisorAddress As String
			Dim bLuseInExproExists As Boolean
			Dim iTestA As Integer





			'oExcelAppExt.SetValueInRow(iFirstRow, 15, "!Debug00", mdicLandusesAppr.Count, dicGlobalExproTypes.Count)
			If mdicLandusesAppr.Count > 0 Then
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oLanduse.AreaSet.AcadArea
					iRowNumber += 1
				Next
				If False Then  '280126
					For Each iExproTypeID As Integer In dicGlobalExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA0", iExproTypeID, dicGlobalExproTypes.Count)
						iTestA += 4
					Next
					For Each iExproTypeID As Integer In mdicParcelExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA1", iExproTypeID, mdicParcelExproTypes.Count)
						iTestA += 4
					Next
				End If
				iRowNumber = 0
				If True Then
					'DMCommon.Debug.MsgBox("13_321", DMCommon.Debug.ColCount(mdicLandusesAppr), DMCommon.Debug.ColCount(dicGlobalExproTypes))
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						iTypeIndex = 0
						bLuseInExproExists = False
						For Each oGlobalExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
							iTypeIndex += 1
							'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 34 + iTypeIndex * 4, "!Debug11", oGlobalExproType.ID)
							If mdicParcelExproTypes.TryGetValue(oGlobalExproType.ID, oExproType) Then
								'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Debug01", oLanduse Is Nothing, oExproType Is Nothing)
								oExcelAppExt.CurrentRow = iFirstRow + iRowNumber
								'oExcelAppExt.SetEnumerable(16 + 12 * iTypeIndex, "!Debug11", oExproType.LanduseArray)
								If oExproType.TryGetValue(oLanduse.ID, oTypeLanduse) AndAlso oTypeLanduse IsNot Nothing Then
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 26, "!Debug02", oLanduse Is Nothing, oTypeLanduse Is Nothing, oExproType Is Nothing)


									If bAcadArea Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
									Else
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.CalcArea)
									End If
									If False Then
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 16 + 2 + iTypeIndex, iTypeIndex, Nothing, oTypeLanduse.AreaSet.CalcArea, Nothing, "xx")
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 24 + 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea, Nothing, "yy")
									End If
									bLuseInExproExists = True
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 3, oTypeLanduse.ID)
								Else

									If mdicParcelExproTypes Is Nothing Then
										iTest1 = -1
									Else
										iTest1 = mdicParcelExproTypes.Count
									End If
									If False Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Dbg1", iTest1, iTypeIndex, oTypeLanduse IsNot Nothing, oLanduse.ID, oExproType.LanduseList, oExproType.LandusesCount, oGlobalExproType.ID)
									End If
								End If
							Else

								'Debug
								If False Then '280126
									oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 25, "!Dbg2", mdicParcelExproTypes.Count, iTypeIndex, oGlobalExproType.ID)
								End If

							End If
						Next
						If bLuseInExproExists Then
							sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
							oLanduse.Name = sLanduseName

							oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName)


							iRowNumber += 1
						End If
						sDividendAddress = oExcelAppExt.GetCellAddress(iFirstRow, iExproTypesCount + 5)
						sDivisorAddress = oExcelAppExt.GetCellAddress(iFirstRow, 2)

						oExcelAppExt.SetFormulaInRow(iFirstRow, 6 + iExproTypesCount, False, "=" & sDividendAddress & "/" & sDivisorAddress & "*100")
						oExcelAppExt.SetFormulaInRow(iFirstRow, 8 + iExproTypesCount, False, "=" & sDivisorAddress & "-" & sDividendAddress)


						dicLanuseIndecis.Add(oLanduse.ID, iRowNumber)

						'	End If
					Next
					'''''''DMCommon.Debug.MsgBox("!SetValueInRowByCol", iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))
					oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))   ''''''''090322  
					sRectAddress = oExcelAppExt.GetRectangleAddress(iFirstRow, 3, iRowNumber, iExproTypesCount)
					oExcelAppExt.SetFormulaInRow(iFirstRow, 5 + iExproTypesCount, False, "=SUM(" & sRectAddress & ")")

				End If


				iCurrentRow += iRowNumber
				'oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

				''///////////////////////////////////////////////////////////////////////////////////////////////////////////
				If False Then  ' 010421    '030919
					Dim oLanduse As TplnLanduse = Nothing
					Dim iLanduseIndex As Integer
					dInArea = 0.0
					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					'Dim oExproType As TplnExproType = Nothing
					For Each oExproExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
						If TplnParcel.ExproTypes.Contains(oExproExproType.ID) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(oExproExproType.ID, oExproType) Then
								iRow = iFirstRow
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTArea", Me.TopoID, Me.BlockNo, Me.ParcelNo, dicExproTypes.Count, oExproType.CalcArea)
								oExcelAppExt.SetValueInRow(iRowNumber, 2 + iTypeIndex, oExproType.CalcArea)
								dInArea += oExproType.CalcArea
								For Each oExproTypeLanduse As TplnLanduse In oExproType.Landuses.Values
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, zzCalcPart(oExproTypeLanduse.AreaSet.CalcArea, oExproType.CalcArea))
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, oExproTypeLanduse.AreaSet.Part)
									iRow += 1
									If dicLanuseIndecis.TryGetValue(oExproTypeLanduse.ID, iLanduseIndex) Then
										daSumLanduseArea(iLanduseIndex) += oExproTypeLanduse.AreaSet.CalcArea
									End If
								Next
							End If
						End If
					Next

					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BefPart", Me.TopoID, Me.BlockNo, Me.ParcelNo, mdicExproTypes.Count, dInArea)
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						daPartLanduseArea(iIndex) = daSumLanduseArea(iIndex) * 100.0 / dInArea
					Next
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!PartLuseArea", daPartLanduseArea)
					Dim oBalancePart As BalanceArea = New BalanceArea(daPartLanduseArea, 10.0, 100.0, True, "ExproUseRep")

					'oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)

					iRowNumber = 0
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))  '
						iRowNumber += 1
					Next

				End If
				'''''''''''''/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			End If

		End Sub

		Public Sub ExproLuseReport_0226_1(ByVal bAcadArea As Boolean, ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iCurrentRow As Integer, dicGlobalExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType))   '_310520

			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iCurrentRow
			Dim iRowNumber As Integer = 0
			Dim iTypeIndex As Integer
			Dim iRow As Integer
			Dim daSumLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim daPartLanduseArea(mdicLandusesAppr.Count - 1) As Double
			Dim oExproType As TplnExproType = Nothing
			Dim dicLanuseIndecis As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)()
			Dim oTypeLanduse As TplnLanduse = Nothing
			Dim iTest1 As Integer
			Dim sRectAddress As String
			Dim iExproTypesCount As Integer = dicGlobalExproTypes.Count
			Dim iLanduseCount As Integer = mdicLandusesAppr.Count
			Dim sDividendAddress As String
			Dim sDivisorAddress As String
			Dim bLuseInExproExists As Boolean
			Dim iTestA As Integer





			'oExcelAppExt.SetValueInRow(iFirstRow, 15, "!Debug00", mdicLandusesAppr.Count, dicGlobalExproTypes.Count)
			If mdicLandusesAppr.Count > 0 Then
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oLanduse.AreaSet.AcadArea
					iRowNumber += 1
				Next
				If True Then  '280126


					For Each iExproTypeID As Integer In dicGlobalExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA0", iExproTypeID, dicGlobalExproTypes.Count)
						oExcelAppExt.TextBox.SetValuesToRow(iFirstRow, 19 + iTestA, "!DebugA0", iExproTypeID, dicGlobalExproTypes.Count)

						iTestA += 4
					Next
					For Each iExproTypeID As Integer In mdicParcelExproTypes.Keys
						oExcelAppExt.SetValueInRow(iFirstRow, 19 + iTestA, "!DebugA1", iExproTypeID, mdicParcelExproTypes.Count)
						oExcelAppExt.TextBox.SetValuesToRow(iFirstRow, 19 + iTestA, "!DebugA1", iExproTypeID, mdicParcelExproTypes.Count)

						iTestA += 4
					Next
				End If
				iRowNumber = 0
				If True Then
					'DMCommon.Debug.MsgBox("13_321", DMCommon.Debug.ColCount(mdicLandusesAppr), DMCommon.Debug.ColCount(dicGlobalExproTypes))
					For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
						iTypeIndex = 0

						bLuseInExproExists = False

						For Each oGlobalExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
							iTypeIndex += 1
							'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 34 + iTypeIndex * 4, "!Debug11", oGlobalExproType.ID)
							If mdicParcelExproTypes.TryGetValue(oGlobalExproType.ID, oExproType) Then
								'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Debug01", oLanduse Is Nothing, oExproType Is Nothing)
								oExcelAppExt.CurrentRow = iFirstRow + iRowNumber
								'oExcelAppExt.SetEnumerable(16 + 12 * iTypeIndex, "!Debug11", oExproType.LanduseArray)
								If oExproType.TryGetValue(oLanduse.ID, oTypeLanduse) AndAlso oTypeLanduse IsNot Nothing Then
									'oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 26, "!Debug02", oLanduse Is Nothing, oTypeLanduse Is Nothing, oExproType Is Nothing)


									If bAcadArea Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
										'DMCommon.Debug.MsgBox("!bAcadAreaA", iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
										oExcelAppExt.TextBox.SetValuesToRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
									Else
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.CalcArea)
										'	DMCommon.Debug.MsgBox("!bNotAcadAreaB", iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
										oExcelAppExt.TextBox.SetValuesToRow(iFirstRow + iRowNumber, 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea)
									End If
									If False Then
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 16 + 2 + iTypeIndex, iTypeIndex, Nothing, oTypeLanduse.AreaSet.CalcArea, Nothing, "xx")
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 24 + 2 + iTypeIndex, oTypeLanduse.AreaSet.AcadArea, Nothing, "yy")
									End If
									bLuseInExproExists = True
									If False Then
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 3, oTypeLanduse.ID)
										oExcelAppExt.TextBox.SetValuesToRow(iFirstRow + iRowNumber, 3, oTypeLanduse.ID)
									End If


								Else

									If mdicParcelExproTypes Is Nothing Then
										iTest1 = -1
									Else
										iTest1 = mdicParcelExproTypes.Count
									End If
									If False Then '280126
										oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 17, "!Dbg1", iTest1, iTypeIndex, oTypeLanduse IsNot Nothing, oLanduse.ID, oExproType.LanduseList, oExproType.LandusesCount, oGlobalExproType.ID)
									End If
								End If
							Else

								'Debug
								If False Then '280126
									oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 25, "!Dbg2", mdicParcelExproTypes.Count, iTypeIndex, oGlobalExproType.ID)
								End If

							End If
						Next


						If bLuseInExproExists Then
							sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
							oLanduse.Name = sLanduseName

							oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName)
							oExcelAppExt.TextBox.SetValuesToRow(iFirstRow + iRowNumber, iExproTypesCount + 3, sLanduseName)

							iRowNumber += 1
						End If
						sDividendAddress = oExcelAppExt.GetCellAddress(iFirstRow, iExproTypesCount + 5)
						sDivisorAddress = oExcelAppExt.GetCellAddress(iFirstRow, 2)

						oExcelAppExt.SetFormulaInRow(iFirstRow, 6 + iExproTypesCount, False, "=" & sDividendAddress & "/" & sDivisorAddress & "*100")
						oExcelAppExt.SetFormulaInRow(iFirstRow, 8 + iExproTypesCount, False, "=" & sDivisorAddress & "-" & sDividendAddress)


						dicLanuseIndecis.Add(oLanduse.ID, iRowNumber)

						'	End If
					Next
					oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False))   ''''''''090322  
					oExcelAppExt.TextBox.SetValuesToRow(iFirstRow, 0, BlockFull, Name, LegalArea(False))   ''''''''090322  

					sRectAddress = oExcelAppExt.GetRectangleAddress(iFirstRow, 3, iRowNumber, iExproTypesCount)
					oExcelAppExt.SetFormulaInRow(iFirstRow, 5 + iExproTypesCount, False, "=SUM(" & sRectAddress & ")")

				End If


				iCurrentRow += iRowNumber
				oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)
				oExcelAppExt.TextBox.SetValuesToRow(iFirstRow, 3, dInArea)


				''///////////////////////////////////////////////////////////////////////////////////////////////////////////
				If False Then  ' 010421    '030919
					Dim oLanduse As TplnLanduse = Nothing
					Dim iLanduseIndex As Integer
					dInArea = 0.0
					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					'Dim oExproType As TplnExproType = Nothing
					For Each oExproExproType As TPlanGraph.TplnExpro.ExproType In dicGlobalExproTypes.Values
						If TplnParcel.ExproTypes.Contains(oExproExproType.ID) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(oExproExproType.ID, oExproType) Then
								iRow = iFirstRow
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproTArea", Me.TopoID, Me.BlockNo, Me.ParcelNo, dicExproTypes.Count, oExproType.CalcArea)
								oExcelAppExt.SetValueInRow(iRowNumber, 2 + iTypeIndex, oExproType.CalcArea)
								dInArea += oExproType.CalcArea
								For Each oExproTypeLanduse As TplnLanduse In oExproType.Landuses.Values
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, zzCalcPart(oExproTypeLanduse.AreaSet.CalcArea, oExproType.CalcArea))
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oExproTypeLanduse.ID, oExproTypeLanduse.Name, oExproTypeLanduse.AreaSet.CalcArea, oExproTypeLanduse.AreaSet.Part)
									iRow += 1
									If dicLanuseIndecis.TryGetValue(oExproTypeLanduse.ID, iLanduseIndex) Then
										daSumLanduseArea(iLanduseIndex) += oExproTypeLanduse.AreaSet.CalcArea
									End If
								Next
							End If
						End If
					Next

					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!BefPart", Me.TopoID, Me.BlockNo, Me.ParcelNo, mdicExproTypes.Count, dInArea)
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						daPartLanduseArea(iIndex) = daSumLanduseArea(iIndex) * 100.0 / dInArea
					Next
					'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!PartLuseArea", daPartLanduseArea)
					Dim oBalancePart As BalanceArea = New BalanceArea(daPartLanduseArea, 10.0, 100.0, True, "ExproUseRep")

					oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)
					oExcelAppExt.TextBox.SetValuesToRow(iFirstRow, 3, dInArea)


					iRowNumber = 0
					For iIndex As Integer = 0 To daSumLanduseArea.GetUpperBound(0)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))
						oExcelAppExt.TextBox.SetValuesToRow(iFirstRow + iRowNumber, 6, daSumLanduseArea(iIndex), oBalancePart.OutputItemFloat(iIndex))  '
						'
						iRowNumber += 1
					Next

				End If
				'''''''''''''/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			End If

		End Sub
		Private Function zzCalcPart(dComponent As Double, dSum As Double) As Double

			Return Math.Round(dComponent * 100.0 / dSum, 1, MidpointRounding.AwayFromZero)
		End Function
		Public Sub ExproLuseReport030919(ByRef oExcelAppExt As DMCommon.ExcelAppExt, ByRef iRow As Integer)

			Dim sLanduseName As String
			Dim dInArea As Double = 0.0
			Dim iFirstRow As Integer = iRow
			Dim iRowNumber As Integer = 0
			Dim iTypeIndex As Integer
			Dim tRect As System.Drawing.Rectangle
			Dim oRange As Range
			'oExcelAppExt.SetValueInRow(iRow, 0, BlockFull, Name, LegalArea(False))
			If mdicLandusesAppr.Count > 0 Then


				For Each oL As TplnLanduse In mdicLandusesAppr.Values
					dInArea += oL.AreaSet.AcadArea
					iRowNumber += 1
				Next
				If False Then
					If iRow - 1 > iFirstRow Then
						tRect = New System.Drawing.Rectangle(3, iFirstRow, 0, iRow - 1 - iFirstRow)
						oRange = oExcelAppExt.Merge(tRect, True)
						oRange.Value = dInArea
					Else
						oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)
						oExcelAppExt.SetValueInRow(iRow, 0, BlockFull, Name, LegalArea(False))
					End If
				End If



				oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 0, BlockFull, Name, LegalArea(False), dInArea)

				iRowNumber = 0
				For Each oLanduse As TplnLanduse In mdicLandusesAppr.Values
					sLanduseName = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, oLanduse.ID)
					oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 4, oLanduse.ID, sLanduseName, oLanduse.AreaSet.AcadArea, 100.0 * oLanduse.AreaSet.AcadArea / dInArea)

					iRowNumber += 1
				Next


				'	oExcelAppExt.SetValueInRow(iFirstRow, 3, dInArea)
				If True Then
					tRect = New System.Drawing.Rectangle(0, iFirstRow, 7, iRowNumber - 1)
					oRange = oExcelAppExt.GetRange(tRect)

					With oRange.Borders.Item(XlBordersIndex.xlEdgeBottom)

						.Weight = XlBorderWeight.xlThin
						.LineStyle = XlLineStyle.xlContinuous
					End With
				End If
				If True Then  '030919


					'DMCommon.Debug.MsgBox("190604_2", iRow, BlockFull, Name)
					Dim oExproType As TplnExproType = Nothing
					For Each tTypeID_Name As KeyValuePair(Of Integer, String) In TplnExpro.TypeNames
						If TplnParcel.ExproTypes.Contains(tTypeID_Name.Key) Then
							'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
							iTypeIndex += 1
							If mdicParcelExproTypes.TryGetValue(tTypeID_Name.Key, oExproType) Then
								iRow = iFirstRow
								oExcelAppExt.SetValueInRowByCol(iFirstRow, iRowNumber, 3 + 5 * iTypeIndex, oExproType.CalcArea)
								For Each oLanduse As TplnLanduse In oExproType.Landuses.Values
									oExcelAppExt.SetValueInRow(iRow, 4 + 5 * iTypeIndex, oLanduse.ID, oLanduse.Name, oLanduse.AreaSet.CalcArea)
									iRow += 1
								Next



							End If
						End If


					Next
				End If

			End If

		End Sub

		Private Shared Sub zzCreateExproTable()
			moExproTable = New Data.DataTable("Expro")
			With moExproTable.Columns

				.Add(BlockFullFieldName, GetType(System.String))                   '0
				.Add(NameFieldName, GetType(System.Int32))                     '1
				.Add(ExproTypeIDFieldName, GetType(System.Int32))                 '2
				.Add(ExproTypeNameFieldName, GetType(System.String))              '3
				.Add(LegalAreaFieldName, GetType(System.Double))             '4
				.Add(TopoReader.msAreaFldName, GetType(System.Double))            '5

				If True Then
					.Add(msToleranceFieldName, GetType(System.Double))             '4    ' 100619
					.Add(msDeltaAreaFieldName, GetType(System.Double))             '4    ' 100619
					.Add(msDeviationFieldName, GetType(System.Double))             '4    ' 100619
				End If


				.Add(msInPlanAreaFDO_OverlayFieldName, GetType(System.Double))       '10  6
				.Add(msInPlanCalcAreaFDO_OverlayFieldName, GetType(System.Double))      '11  7
				.Add(msInPlanCalcArea2FDO_OverlayFieldName, GetType(System.Double))  '12  8
				.Add(msInPlanRoundedAreaFDO_OverlayFieldName, GetType(System.Double))   '13  9
				.Add(BlockFieldName, GetType(System.Int32))                     '0
				.Add(BlockAddFieldName, GetType(System.Int32))
			End With
		End Sub
		Private Shared Sub zzCreateExproTableByCol()
			moExproTableByCol = New Data.DataTable("ExproByCol")
			With moExproTableByCol.Columns

				.Add(BlockFullFieldName, GetType(System.String))                   '0
				.Add(NameFieldName, GetType(System.Int32))                     '1

				.Add(LegalAreaFieldName, GetType(System.Double))             '4
				.Add(TopoReader.msAreaFldName, GetType(System.Double))         '4    ' 100619
				.Add(msToleranceFieldName, GetType(System.Double))             '4    ' 100619
				.Add(msDeltaAreaFieldName, GetType(System.Double))             '4    ' 100619
				.Add(msDeviationFieldName, GetType(System.Double))             '4    ' 100619
				.Add(msInPlanCalcAreaFDO_OverlayFieldName, GetType(System.Double))            '5
				'	DMCommon.Debug.ExcelLog.SetEnumerable(0, "!ExproTypes1", mhsExproTypes)
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!miMaxExproType", miMaxExproType, mhsExproTypes.Count, miMaxOutPgonsCount)

				For iType As Integer = 1 To miMaxExproType
					If mhsExproTypes.Contains(iType) Then

						.Add(zzExproTypeAreaFieldName(iType, 0), GetType(System.Double))


					End If
				Next
				For iPgonIndex As Integer = 1 To miMaxOutPgonsCount
					'.Add(msOutPgonFieldNamePrefix & Convert.ToString(iPgonIndex), GetType(System.Double))
					.Add(zzOutPgonFieldName(iPgonIndex, 0), GetType(System.Boolean))
					For iFieldIndex As Integer = 1 To 5
						.Add(zzOutPgonFieldName(iPgonIndex, iFieldIndex), GetType(System.Double))

					Next

				Next



				.Add(BlockFieldName, GetType(System.Int32))                      '0
				.Add(BlockAddFieldName, GetType(System.Int32))
			End With
			'DMCommon.Debug.ExcelLog.SetDataTable(0, "moExproTableByCol", moExproTableByCol)
			'	moExproTableByCol
		End Sub

		Private Shared Sub zzCreateExproTablePgons()
			moExproTablePgons = New Data.DataTable("ExproTablePgons")
			With moExproTablePgons.Columns

				.Add(BlockFieldName, GetType(System.Int32))                     '0
				.Add(BlockAddFieldName, GetType(System.Int32))                  '0
				.Add(NameFieldName, GetType(System.Int32))

				.Add(msExproPgonTypeIDFieldName, GetType(System.Int32))                     '1
				.Add(msExproPgonTypeNameFieldName, GetType(System.String))                     '1



				.Add(msExproCalcTypeFieldName, GetType(System.Int32))                     '1


				.Add(LegalAreaFieldName, GetType(System.Double))             '4

				.Add(TopoReader.msAreaFldName, GetType(System.Double))             '4    ' 100619



				.Add(msToleranceFieldName, GetType(System.Double))             '4    ' 100619
				.Add(msDeltaAreaFieldName, GetType(System.Double))             '4    ' 100619
				.Add(msDeviationFieldName, GetType(System.Double))             '4    ' 100619
			End With





		End Sub

		Private Shared Function zzExproTypeAreaFieldName(iExproType As Integer, iFieldNo As Integer) As String
			Select Case iFieldNo
				Case 0
					Return msExproTypeAreaFieldNamePrefix & Convert.ToString(iExproType)
				Case 1
					Return msExproTypeAcadAreaFieldNamePrefix & Convert.ToString(iExproType)
				Case 2
					Return msExproTypeToleranceFieldNamePrefix & Convert.ToString(iExproType)
				Case 3
					Return msExproTypeDiffAreaFieldNamePrefix & Convert.ToString(iExproType)
				Case 4
					Return msExproTypeDeviationFieldNamePrefix & Convert.ToString(iExproType)
				Case Else
					Return Nothing
			End Select

		End Function
		Private Shared Function zzOutPgonFieldName(iPgonIndex As Integer, iFieldNo As Integer) As String
			Select Case iFieldNo
				Case 0
					Return msOutPgonIsForcedFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case 1
					Return msOutPgonLegalAreaFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case 2
					Return msOutPgonAcadAreaFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case 3
					Return msOutPgonToleranceFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case 4
					Return msOutPgonDiffAreaFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case 5
					Return msOutPgonDeviationFieldNamePrefix & Convert.ToString(iPgonIndex)
				Case Else
					Return Nothing
			End Select

		End Function


		Public Sub AddDataToExproTable()
			Dim oNewRow As System.Data.DataRow

			'

			For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "TypeOverGr", Me.BlockFull, Me.ExtName, Me.LegalArea(False), mdicTypeOverlayGroups.Count, oTypeOverlayGroup.TypeID, oTypeOverlayGroup.AcadArea, oTypeOverlayGroup.CalcArea, oTypeOverlayGroup.RoundedArea)
				oNewRow = moExproTable.NewRow()
				With oNewRow


					.Item(BlockFullFieldName) = Me.BlockFull
					.Item(NameFieldName) = Me.ExtName '   MyBase.Name
					.Item(ExproTypeIDFieldName) = oTypeOverlayGroup.TypeID
					.Item(ExproTypeNameFieldName) = oTypeOverlayGroup.TypeName
					.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
					.Item(TopoReader.msAreaFldName) = MyBase.ddAcadArea

					If True Then
						.Item(msToleranceFieldName) = Me.ParcelArea.ToleranceM         '4    ' 100619
						.Item(msDeltaAreaFieldName) = Me.ParcelArea.DeltaAreaM           '4    ' 100619
						.Item(msDeviationFieldName) = Me.ParcelArea.DeviationM          '4    ' 100619
					End If

					.Item(msInPlanCalcAreaFDO_OverlayFieldName) = oTypeOverlayGroup.CalcArea
					.Item(msInPlanRoundedAreaFDO_OverlayFieldName) = oTypeOverlayGroup.RoundedArea
					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd




				End With
				moExproTable.Rows.Add(oNewRow)

			Next


		End Sub
		Public Sub AddDataToExproTableByCol()

			Dim oNewRow As System.Data.DataRow
			Dim dInPlanCalcAreaFDO_Overlay As Double = 0.00000000
			Dim tParcelArea As ParcelArea
			Dim sFieldName As String
			If mdicTypeOverlayGroups.Count > 0 Then
				'	DMCommon.Debug.MsgBox("05_700K", mdicExpros.Count)
				oNewRow = moExproTableByCol.NewRow()
				With oNewRow

					.Item(BlockFullFieldName) = Me.BlockFull
					.Item(NameFieldName) = Me.ExtName '   MyBase.Name
					.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
					.Item(TopoReader.msAreaFldName) = Me.AcadArea(False)

					.Item(msToleranceFieldName) = Me.Tolerance(False)
					.Item(msDeltaAreaFieldName) = Me.ParcelArea.DeltaAreaM          '4    ' 100619
					.Item(msDeviationFieldName) = Me.ParcelArea.DeviationM   '4    ' 100619




					For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values
						sFieldName = zzExproTypeAreaFieldName(oTypeOverlayGroup.TypeID, 0)
						If moExproTableByCol.Columns.Contains(sFieldName) Then
							.Item(sFieldName) = oTypeOverlayGroup.CalcAreaPlus
							dInPlanCalcAreaFDO_Overlay += oTypeOverlayGroup.CalcAreaPlus
						Else
							DMCommon.Debug.MsgBox("05_707K", sFieldName & " not found")
						End If

					Next
					.Item(msInPlanCalcAreaFDO_OverlayFieldName) = dInPlanCalcAreaFDO_Overlay

					'	DMCommon.ExcelLog.SetNextValue(0, "TypeOverGr", Me.BlockFull, Me.ExtName, Me.LegalArea(False), mdicTypeOverlayGroups.Count, oTypeOverlayGroup.TypeID, oTypeOverlayGroup.AcadArea, oTypeOverlayGroup.CalcArea, oTypeOverlayGroup.RoundedArea)

					For iPgonIndex As Integer = 1 To mcolOutPgons.Count
						tParcelArea = mtaOutPgons(iPgonIndex - 1)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+LegalArea", Me.LegalOrAcadArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.Tolerance, tParcelArea.ConditionalArea, tParcelArea.IsForced)
						.Item(zzOutPgonFieldName(iPgonIndex, 0)) = tParcelArea.IsForced
						.Item(zzOutPgonFieldName(iPgonIndex, 1)) = tParcelArea.LegalAreaM
						.Item(zzOutPgonFieldName(iPgonIndex, 2)) = tParcelArea.AcadArea
						.Item(zzOutPgonFieldName(iPgonIndex, 3)) = tParcelArea.ToleranceM
						.Item(zzOutPgonFieldName(iPgonIndex, 4)) = tParcelArea.DeltaAreaM
						.Item(zzOutPgonFieldName(iPgonIndex, 5)) = tParcelArea.DeviationM

						'DMCommon.Debug.ExcelLog.SetNextValue(0, "OutParcArea", Me.BlockFull, Me.ExtName, iPgonIndex, tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea)
					Next

					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd
				End With
				moExproTableByCol.Rows.Add(oNewRow)
			End If

		End Sub
		Public Sub AddDataToExproTablePgonsNew(ByRef oExproPgonsTable As System.Data.DataTable, ByVal iVersion As Integer, ByVal iCalcType As Integer)
			'	moExproTablePgons
			Dim oNewRow As System.Data.DataRow
			Dim dInPlanCalcAreaFDO_Overlay As Double = 0.00000000
			Dim tParcelArea As ParcelArea
			If mdicTypeOverlayGroups.Count > 0 Then
				'	DMCommon.Debug.MsgBox("05_700K", mdicExpros.Count)
				oNewRow = oExproPgonsTable.NewRow()
				With oNewRow

					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd
					.Item(NameFieldName) = Me.ParcelNo



					.Item(msExproPgonTypeIDFieldName) = 20
					.Item(msExproPgonTypeNameFieldName) = "חלקה"

					.Item(msExproCalcTypeFieldName) = iCalcType


					.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
					.Item(TopoReader.msAreaFldName) = Me.AcadArea(False)
					.Item(msToleranceFieldName) = Me.Tolerance(False)
					.Item(msDeltaAreaFieldName) = Me.ParcelArea.DeltaAreaM          '4    ' 100619
					.Item(msDeviationFieldName) = Me.ParcelArea.DeviationM   '4    ' 100619

				End With
				oExproPgonsTable.Rows.Add(oNewRow)
				oNewRow = oExproPgonsTable.NewRow()
				With oNewRow
					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd
					.Item(NameFieldName) = Me.ParcelNo
					For iIndex As Integer = 0 To mtaExproParcelArea.GetUpperBound(0)
						.Item(msExproPgonTypeIDFieldName) = mtaExproParcelArea(iIndex).ExproType
						.Item(msExproPgonTypeNameFieldName) = TplnExpro.GetExproTypeName(mtaExproParcelArea(iIndex).ExproType)

						If iCalcType = 1 Then
							.Item(msExproCalcTypeFieldName) = 1
							.Item(CondLegalAreaFieldName) = mtaExproParcelArea(iIndex).ConditionalArea
						ElseIf iCalcType = 2 Then
							.Item(LegalAreaFieldName) = mtaExproParcelArea(iIndex).LegalArea
							.Item(msExproCalcTypeFieldName) = 1
						End If


						.Item(TopoReader.msAreaFldName) = mtaExproParcelArea(iIndex).AcadArea
						.Item(msToleranceFieldName) = tParcelArea.ToleranceM
						.Item(msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
						.Item(msDeviationFieldName) = tParcelArea.DeviationM
						dInPlanCalcAreaFDO_Overlay += mtaExproParcelArea(iIndex).LegalArea
					Next







					'	.Item(msInPlanCalcAreaFDO_OverlayFieldName) = dInPlanCalcAreaFDO_Overlay

					'	DMCommon.ExcelLog.SetNextValue(0, "TypeOverGr", Me.BlockFull, Me.ExtName, Me.LegalArea(False), mdicTypeOverlayGroups.Count, oTypeOverlayGroup.TypeID, oTypeOverlayGroup.AcadArea, oTypeOverlayGroup.CalcArea, oTypeOverlayGroup.RoundedArea)
				End With
				oExproPgonsTable.Rows.Add(oNewRow)


				For iPgonIndex As Integer = 1 To mcolOutPgons.Count
					oNewRow = oExproPgonsTable.NewRow()
					With oNewRow

						.Item(BlockFieldName) = Me.BlockNo
						.Item(BlockAddFieldName) = Me.BlockAdd
						.Item(NameFieldName) = Me.ParcelNo

						tParcelArea = mtaOutPgons(iPgonIndex - 1)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+LegalArea", Me.LegalOrAcadArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.Tolerance, tParcelArea.ConditionalArea, tParcelArea.IsForced)

						.Item(msExproPgonTypeIDFieldName) = 20 + iPgonIndex
						.Item(msExproPgonTypeNameFieldName) = "פוליגון" & " " & Chr(223 + iPgonIndex).ToString()

						If iCalcType = 1 Then
							.Item(msExproCalcTypeFieldName) = 1
							.Item(CondLegalAreaFieldName) = tParcelArea.ConditionalArea
						ElseIf iCalcType = 2 Then
							.Item(LegalAreaFieldName) = tParcelArea.LegalArea
							.Item(msExproCalcTypeFieldName) = 1
						End If





						''''''''''''''''.Item(zzOutPgonFieldName(iPgonIndex, 0)) = tParcelArea.IsForced

						.Item(LegalAreaFieldName) = tParcelArea.LegalAreaM
						.Item(TopoReader.msAreaFldName) = tParcelArea.AcadArea

						.Item(msToleranceFieldName) = tParcelArea.ToleranceM
						.Item(msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
						.Item(msDeviationFieldName) = tParcelArea.DeviationM


						DMCommon.Debug.ExcelLog.SetNextValue(0, "OutParcArea", Me.BlockFull, Me.ExtName, iPgonIndex, tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea)
					End With
					oExproPgonsTable.Rows.Add(oNewRow)
				Next
			End If
		End Sub


		Public Sub AddDataToExproTablePgons()
			'	moExproTablePgons
			Dim oNewRow As System.Data.DataRow
			Dim dInPlanCalcAreaFDO_Overlay As Double = 0.00000000
			Dim tParcelArea As ParcelArea
			If mdicTypeOverlayGroups.Count > 0 Then
				'	DMCommon.Debug.MsgBox("05_700K", mdicExpros.Count)
				oNewRow = moExproTablePgons.NewRow()
				With oNewRow

					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd
					.Item(NameFieldName) = Me.ParcelNo



					.Item(msExproPgonTypeIDFieldName) = 20
					.Item(msExproPgonTypeNameFieldName) = "חלקה"

					.Item(msExproCalcTypeFieldName) = 0


					.Item(LegalAreaFieldName) = Me.LegalOrAcadArea(False)
					.Item(TopoReader.msAreaFldName) = Me.AcadArea(False)
					.Item(msToleranceFieldName) = Me.Tolerance(False)
					.Item(msDeltaAreaFieldName) = Me.ParcelArea.DeltaAreaM          '4    ' 100619
					.Item(msDeviationFieldName) = Me.ParcelArea.DeviationM   '4    ' 100619

				End With
				moExproTablePgons.Rows.Add(oNewRow)
				oNewRow = moExproTablePgons.NewRow()
				With oNewRow
					.Item(BlockFieldName) = Me.BlockNo
					.Item(BlockAddFieldName) = Me.BlockAdd
					.Item(NameFieldName) = Me.ParcelNo
					For Each oTypeOverlayGroup As TypeOverlayGroup In mdicTypeOverlayGroups.Values




						.Item(msExproPgonTypeIDFieldName) = oTypeOverlayGroup.TypeID
						.Item(msExproPgonTypeNameFieldName) = TplnExpro.GetExproTypeName(oTypeOverlayGroup.TypeID)


						.Item(msExproCalcTypeFieldName) = 0

						.Item(LegalAreaFieldName) = oTypeOverlayGroup.CalcAreaPlus
						.Item(TopoReader.msAreaFldName) = oTypeOverlayGroup.AcadArea



						.Item(msToleranceFieldName) = tParcelArea.ToleranceM
						.Item(msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
						.Item(msDeviationFieldName) = tParcelArea.DeviationM


						dInPlanCalcAreaFDO_Overlay += oTypeOverlayGroup.CalcAreaPlus

					Next
					'	.Item(msInPlanCalcAreaFDO_OverlayFieldName) = dInPlanCalcAreaFDO_Overlay

					'	DMCommon.ExcelLog.SetNextValue(0, "TypeOverGr", Me.BlockFull, Me.ExtName, Me.LegalArea(False), mdicTypeOverlayGroups.Count, oTypeOverlayGroup.TypeID, oTypeOverlayGroup.AcadArea, oTypeOverlayGroup.CalcArea, oTypeOverlayGroup.RoundedArea)
				End With
				moExproTablePgons.Rows.Add(oNewRow)


				For iPgonIndex As Integer = 1 To mcolOutPgons.Count
					oNewRow = moExproTablePgons.NewRow()
					With oNewRow

						.Item(BlockFieldName) = Me.BlockNo
						.Item(BlockAddFieldName) = Me.BlockAdd
						.Item(NameFieldName) = Me.ParcelNo

						tParcelArea = mtaOutPgons(iPgonIndex - 1)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+LegalArea", Me.LegalOrAcadArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.Tolerance, tParcelArea.ConditionalArea, tParcelArea.IsForced)

						.Item(msExproPgonTypeIDFieldName) = 20 + iPgonIndex
						.Item(msExproPgonTypeNameFieldName) = "פוליגון" & " " & Chr(223 + iPgonIndex).ToString()


						.Item(msExproCalcTypeFieldName) = 0


						''''''''''''''''.Item(zzOutPgonFieldName(iPgonIndex, 0)) = tParcelArea.IsForced

						.Item(LegalAreaFieldName) = tParcelArea.LegalAreaM
						.Item(TopoReader.msAreaFldName) = tParcelArea.AcadArea

						.Item(msToleranceFieldName) = tParcelArea.ToleranceM
						.Item(msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
						.Item(msDeviationFieldName) = tParcelArea.DeviationM


						DMCommon.Debug.ExcelLog.SetNextValue(0, "OutParcArea", Me.BlockFull, Me.ExtName, iPgonIndex, tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea)
					End With
					moExproTablePgons.Rows.Add(oNewRow)


				Next












			End If

		End Sub


		Public Overrides Sub Terminate()
			If False Then
				If mdicLandusesProp IsNot Nothing Then
					For Each oLanduse As TplnLanduse In mdicLandusesProp.Values
						oLanduse.Terminate()
						oLanduse = Nothing
					Next
					mdicLandusesProp.Clear()
					mdicLandusesProp = Nothing
				End If

				If mdicLandusesAppr IsNot Nothing Then
					mdicLandusesAppr.Terminate()
					mdicLandusesAppr.Clear()
					mdicLandusesAppr = Nothing
				End If
			End If

			MyBase.Terminate()
		End Sub
#End Region
#Region "Private members"
		Private Shared ReadOnly Property zzGetLanduseTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, bRegion As Boolean, Optional bDebug As Boolean = False) As Data.DataTable
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					If moLanduseRegionApprTable IsNot Nothing Then
						'  MessageBox.Show(bRegion.ToString() & vbCrLf & iTopoPurpose.ToString() & vbCrLf & CStr(moLanduseRegionApprTable.Rows.Count), "05_123")
					ElseIf bRegion Then
						' MessageBox.Show(bRegion.ToString() & vbCrLf & iTopoPurpose.ToString() & vbCrLf & CStr("moLanduseRegionApprTable Is Nothing"), "05_124")
					End If

					If bRegion Then
						If bRegion And bDebug Then
							''''''''''''''''''''  DMCommon.ExcelLog.SetDataTable(moLanduseRegionApprTable, 0)
						End If
						Return moLanduseRegionApprTable
					Else
						Return moLanduseApprTable
					End If

				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					If bRegion Then
						Return moLanduseRegionPropTable
					Else
						Return moLandusePropTable

					End If

				Else
					Return Nothing
				End If
			End Get
		End Property

		Private Shared Sub zzAddColToListAAA(ByRef iColumnIndex As Integer, ByRef iPriorIndex As Integer, bVisible As Boolean)
			If bVisible Then
				iColumnIndex += 1
			End If
			iPriorIndex += 1

		End Sub
		Private Shared Sub zzTestIntArray(ByVal iaVal() As Integer)
			Dim iUB As Integer = -1
			Dim sMsg As String = "---" & vbCrLf
			Try
				iUB = iaVal.GetUpperBound(0)
				For iIndex As Integer = 0 To iUB
					If iIndex <> 0 Then sMsg &= vbCrLf
					sMsg = sMsg & ":" & iaVal(iIndex).ToString() & ":"

				Next
				sMsg = sMsg & vbCrLf & "---"
			Catch ex As Exception
				System.Windows.Forms.MessageBox.Show(ex.Message, "ex: Parcel-TestIntArray")
			End Try
			System.Windows.Forms.MessageBox.Show(sMsg, "Parcel-TestIntArray")
		End Sub
		Public Shared Function CreateBlockTable() As System.Data.DataTable
			Dim oDataTable As System.Data.DataTable
			oDataTable = New Data.DataTable("Blocks")
			With oDataTable.Columns
				.Add(BlockFullFieldName, GetType(System.String))
				.Add(BlockStatusNameFieldName, GetType(System.String))
				.Add(msParcelEntireFieldName, GetType(System.String))
				.Add(msParcelPartialFieldName, GetType(System.String))
				.Add(BlockStatusFieldName, GetType(System.String))
				.Add(BlockIsAnalyticFieldName, GetType(System.Int32))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(LegalAreaFieldName, GetType(System.Double))

				.Add(TopoReader.msSumPgonAreaFldName, GetType(System.Double))
				.Add(TopoReader.msSumLegalAreaFldName, GetType(System.Double))

				.Add(msInPlanCalcAreaApprMergeFieldName, GetType(System.Double))
				.Add(msInPlanCalcAreaPropMergeFieldName, GetType(System.Double))
				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))
				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
				.Add(BlockFieldName, GetType(System.Int32))
				.Add(BlockAddFieldName, GetType(System.Int32))

			End With
			Return oDataTable
		End Function
		Private Shared Sub zzCreateBlockTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			If moaBlockTable(iOverlayIndex) Is Nothing Then
				moaBlockTable(iOverlayIndex) = CreateBlockTable()
			Else
				moaBlockTable(iOverlayIndex).Rows.Clear()
			End If
		End Sub

		Private Shared Sub zzCreateBlockRegionTable()
			If moBlockRegionTable Is Nothing Then

				moBlockRegionTable = CreateBlockTable()
			Else
				moBlockRegionTable.Rows.Clear()
			End If
		End Sub



		Private Shared Sub zzCreateLotsContentTable()
			If moLotsContentTable Is Nothing Then
				moLotsContentTable = New Data.DataTable("LotContents")
				With moLotsContentTable.Columns
					.Add(msLotNameFieldName, GetType(System.String))
					.Add(BlockFullFieldName, GetType(System.String))


					.Add(msParcelEntireFieldName, GetType(System.String))
					.Add(msParcelPartialFieldName, GetType(System.String))

					.Add(BlockFieldName, GetType(System.Int32))
					.Add(BlockAddFieldName, GetType(System.Int32))

				End With
			Else
				moLotsContentTable.Rows.Clear()
			End If
		End Sub
		Public Shared Sub DispPgonTables()
			Dim s As String = ""
			For i As Integer = 0 To moaPolygonTable.GetUpperBound(0)
				If i <> 0 Then
					s &= vbCrLf
				End If
				If moaPolygonTable(i) IsNot Nothing Then
					s &= CStr(moaPolygonTable(i).Rows.Count)
				Else
					s &= "N"
				End If

			Next

			MessageBox.Show(s, "05_770")
		End Sub

		Public Sub DispInPlanArray()
			Dim s As String = ""
			For i As Integer = 0 To mbaInPlan.GetUpperBound(0)
				If i <> 0 Then
					s &= vbCrLf
				End If
				If moaPolygonTable(i) IsNot Nothing Then
					s &= CStr(mbaInPlan(i))
				Else
					s &= "N"
				End If

			Next

			MessageBox.Show(s, "05_780")
		End Sub
		Private Shared ReadOnly Property zzGetPolygonTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Data.DataTable
			Get
				Try
					'MessageBox.Show(moaPolygonTable.GetUpperBound(0).ToString() & ":" & iOverlayIndex.ToString() & ":" & CInt(iOverlayIndex).ToString(), "01_210 ")
					Return moaPolygonTable(iOverlayIndex)

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - zzGetPolygonTable")
					Return Nothing
				End Try
			End Get
		End Property
		Private ReadOnly Property zzGetLanduseDic(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TplnLanduses
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					Return mdicLandusesAppr
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return mdicLandusesProp
				Else
					Return Nothing
				End If
			End Get
		End Property
		Private ReadOnly Property zzGetLanduseRegionDic(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TplnLanduses
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then
					Return mdicLandusesRegionAppr
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return mdicLandusesRegionProp
				Else
					Return Nothing
				End If
			End Get
		End Property




		Public Shared Function zzGetPlanStateFieldName(iOverlayIndex As DMAcadExt.enOverlayIndex) As String
			Select Case iOverlayIndex
				Case DMAcadExt.enOverlayIndex.ApprMerge
					Return msPlanStateApprMergeFieldName
				Case DMAcadExt.enOverlayIndex.PropMerge
					Return msPlanStatePropMergeFieldName
				Case DMAcadExt.enOverlayIndex.ApprUnion
					Return Nothing
				Case DMAcadExt.enOverlayIndex.PropUnion
					Return Nothing
				Case DMAcadExt.enOverlayIndex.ApprFDO_Overlay
					Return msPlanStateApprFDO_OverlayFieldName
				Case DMAcadExt.enOverlayIndex.PropFDO_Overlay
					Return msPlanStatePropFDO_OverlayFieldName
				Case Else
					Return Nothing
			End Select
		End Function
		Private Function zzIsInPlan(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				Return mbaInPlan(DMAcadExt.enOverlayIndex.ApprMerge) OrElse mbaInPlan(DMAcadExt.enOverlayIndex.ApprUnion) OrElse mbaInPlan(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				Return mbaInPlan(DMAcadExt.enOverlayIndex.PropMerge) OrElse mbaInPlan(DMAcadExt.enOverlayIndex.PropUnion) OrElse mbaInPlan(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
			Else
				Return False
			End If
		End Function
		Private Sub zzAfterSetLegalArea(bDunam As Boolean)
			Dim bLegalAreaExists As Boolean = False
			If bDunam Then
				mdLegalArea = 1000.0 * mdLegalArea
			End If
			If mdLegalArea > 0.0 Then
				mbHasLegalArea = True
				bLegalAreaExists = True
			End If
			If Not mbHasLegalArea Then
				mdRoundedArea = Math.Round(Me.AcadArea(False) * TplnProject.CalcRoundFactor, MidpointRounding.AwayFromZero) / TplnProject.CalcRoundFactor
			End If
		End Sub
		Private Function zzGetRegionAreaset(iOverlayIndex As DMAcadExt.enOverlayIndex, iRegion As Integer) As TplnAreaSet
			Dim oAllOverlayGroups As TplnOverlayGroups = TplnProject.OverlayGroups(iOverlayIndex)
			Dim oOtherTopoIDList As List(Of Integer) = diaOverlayTopoID(iOverlayIndex)
			Dim oOverlayGroup As TplnOverlayGroup
			Dim bParcelLot As Boolean
			Dim bIn As Boolean
			Dim bOut As Boolean
			Dim iPlanState As NumerationPair.enComplexType = Me.PlanState(iOverlayIndex)
			Dim iTest As Integer
			Dim tRegionAreaSet As TplnAreaSet
			If diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel Then
				bParcelLot = True
			ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
				bParcelLot = False
			ElseIf diTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
				bParcelLot = False
			Else
				Return New TplnAreaSet()
			End If



			'		System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & diTopoPurpose.ToString() & vbCrLf & CStr(doaOverlayPgons(iOverlayIndex).Count) & vbCrLf & CStr(oOtherTopoIDList IsNot Nothing), "04_262")
			If oOtherTopoIDList IsNot Nothing Then
				'System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oOtherTopoIDList.Count), "04_766")
				tRegionAreaSet.PlanState = NumerationPair.enComplexType.Undefined
				For Each iTopoID As Integer In oOtherTopoIDList
					oOverlayGroup = oAllOverlayGroups.GetItem(diTopoID, iTopoID, bParcelLot)



					If oOverlayGroup IsNot Nothing Then

						'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayGroup.AreaSet.AcadArea) & vbCrLf & Str(oOverlayGroup.AreaSet.CalcArea) & vbCrLf & iOverlayIndex.ToString() & vbCrLf & diTopoPurpose.ToString() & vbCrLf & CStr(doaOverlayPgons(iOverlayIndex).Count) & vbCrLf & CStr(oOtherTopoIDList IsNot Nothing), "04_263")
						If oOverlayGroup.GroupID = iRegion Then
							tRegionAreaSet.Add(oOverlayGroup.AreaSet)
							bIn = True
						Else
							bOut = True
						End If

					Else
						DMAcadExt.AcadDocument.WriteMessageLog("37_12: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
						'	System.Windows.Forms.MessageBox.Show(CStr(oOtherTopoIDList.Count), "04_266")
					End If

				Next
				If bIn Then
					If bOut OrElse iPlanState <> NumerationPair.enComplexType.Entire Then
						tRegionAreaSet.PlanState = NumerationPair.enComplexType.Partial
					Else
						tRegionAreaSet.PlanState = NumerationPair.enComplexType.Entire
					End If
				Else
					tRegionAreaSet.PlanState = NumerationPair.enComplexType.Undefined
				End If
			Else
				''''''''''''''''''''''''''TEMP DEBUG Only

				''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(Me.AcadArea(False)), "04_975")
				tRegionAreaSet.AcadArea = Me.AcadArea(False)
				tRegionAreaSet.PlanState = NumerationPair.enComplexType.Undefined

				iTest += 1
				If iTest < 0 Then
					System.Windows.Forms.MessageBox.Show(CStr(diTopoID) & ":" & diTopoPurpose.ToString() & vbCrLf & CStr(Me.AcadArea(False)), "03_733a")
				End If

				'Temp 0208 DMAcadExt.AcadDocument.WriteMessageLog("01_500a: " & CStr(diTopoID) & ":" & iOverlayIndex.ToString())
			End If
			Return tRegionAreaSet
		End Function
		Private Sub zzParseLegalAreaString(ByVal sValue As String)
			'	Dim bLegalAreaExists As Boolean = False
			sValue = sValue.Trim()
			If IsNumeric(sValue) AndAlso Not sValue.Contains("-") Then
				Try
					mdLegalArea = Convert.ToDouble(sValue)
					zzAfterSetLegalArea(True)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - zzParseLegalAreaString")
				End Try
			End If
			If Not mbHasLegalArea Then
				mlstMissingLegalAreaPoints.Add(New DMAcadExt.TPlnPoint(ddCentroidX, ddCentroidY))
			End If

		End Sub

#End Region

		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub

		Protected Overrides Sub OnCalculate2(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod)


		End Sub
		Protected Overrides ReadOnly Property PolygonCaption As String
			Get
				Return "חלקה"
			End Get
		End Property
		Protected Overrides ReadOnly Property _PolygonFullName As String
			Get
				Const sGushCaption As String = "גוש"
				Return sGushCaption & ": " & Me.BlockFull & "  " & Me.PolygonCaption & ": " & Me.Name
			End Get
		End Property

		Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
			Get
				Return miaBlockAttribIndex
			End Get

		End Property

		Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
			Get

				Return miaAddBlockAttribIndex
			End Get
		End Property
		Private Class TypeOverlayGroup

			Private miTypeID As Integer
			Private msTypeName As String
			Private mdAcadArea As Double
			Private mdRoundedArea As Double
			Private mdCalcArea As Double
			Private mdCalcArea2 As Double
			Private mdCalcAreaPlus As Double
			Private mtParcelArea As ParcelArea
			Public Sub New(iTypeID As Integer, sTypeName As String, dAcadArea As Double)
				miTypeID = iTypeID

				msTypeName = sTypeName
				mdAcadArea = dAcadArea
				mtParcelArea = New ParcelArea(dAcadArea)
			End Sub

			Public Property TypeID As Integer
				Get
					Return miTypeID
				End Get
				Set(iValue As Integer)
					miTypeID = iValue
				End Set
			End Property
			Public Property TypeName As String
				Get
					Return msTypeName
				End Get
				Set(sValue As String)
					msTypeName = sValue
				End Set
			End Property

			Public Property AcadArea As Double
				Get
					Return mdAcadArea
				End Get
				Set(dValue As Double)
					mdAcadArea = dValue
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
			Public Property CalcArea As Double
				Get
					Return mdCalcArea
				End Get
				Set(dValue As Double)
					mdCalcArea = dValue
					mtParcelArea.CalculateArea(mdCalcArea, 3)
				End Set
			End Property
			Public Property CalcArea2 As Double
				Get
					Return mdCalcArea2
				End Get
				Set(dValue As Double)
					mdCalcArea2 = dValue
				End Set
			End Property
			Public Property CalcAreaPlus As Double
				Get
					Return mdCalcAreaPlus
				End Get
				Set(dValue As Double)
					mdCalcAreaPlus = dValue
				End Set
			End Property
			Public ReadOnly Property ParcelArea As ParcelArea
				Get
					Return mtParcelArea
				End Get

			End Property

			Public Sub AddAcadArea(dAcadArea As Double)
				mdAcadArea += dAcadArea
			End Sub
		End Class

	End Class

	Public Structure ParcelArea
		Const Formula1 As String = "0.3*sqrt[A] + 0.005A"
		Const Formula2 As String = "0.8*sqrt[A] + 0.002A"

		Public LegalArea As Double
		Public AcadArea As Double

		'Public LegalAreaCond As Double
		Public mdAreaM As Double

		Public CalcArea As Double
		Public DeltaArea As Double
		Public Tolerance As Double
		Public Deviation As Double
		Public HasDeviation As Boolean
		Public ForcedArea As Double
		Public ConditionalArea As Double
		Public ExproType As Integer
		Public FormulaType As Integer ' 1 OR 2
		Public Need As Double
		Public AreaStatus As TopoManager.TPlanGraph.enAreaStatus
		Private mdRangeMin As Double
		Private mdRangeMax As Double
		Private mbLegalAreaFromBlock As Boolean


		Public Shared Function CheckArea(dAcadArea As Double, dLegalArea As Double) As ParcelArea
			Dim tPlanArea As ParcelArea = New ParcelArea(dAcadArea)
			tPlanArea.CalculateArea(dLegalArea, 3)
			Return tPlanArea
		End Function
		Public Sub New(dAcadArea As Double)
			AcadArea = dAcadArea
		End Sub
		Public Function CanGive() As Double
			Return Math.Max(CalcArea - LegalArea + Tolerance, 0.0)
		End Function
		Public Function CanReceive() As Double
			Return Math.Max(LegalArea + Tolerance - CalcArea, 0.0)
		End Function
		Public ReadOnly Property AcadAreaD As Double
			Get
				Return 0.001 * AcadArea
			End Get
		End Property
		Public ReadOnly Property AreaM As Double
			Get
				Return mdAreaM
			End Get
		End Property
		Public ReadOnly Property RangeMin As Double
			Get
				Return mdRangeMin
			End Get
		End Property
		Public ReadOnly Property RangeMax As Double
			Get
				Return mdRangeMax
			End Get
		End Property
		Public ReadOnly Property Formula As String
			Get
				Select Case FormulaType
					Case 1
						Return Formula1
					Case 2
						Return Formula2
					Case Else
						Return String.Empty
				End Select
			End Get
		End Property
		Public ReadOnly Property LegalAreaM As Double
			Get
				Return LegalArea
			End Get
		End Property
		Public ReadOnly Property LegalOrForcedArea As Double
			Get
				If ForcedArea = 0.0 Then
					Return LegalArea
				Else
					Return ForcedArea
				End If

			End Get
		End Property

		Public ReadOnly Property IsForced As Boolean
			Get
				Return (LegalArea <> ConditionalArea)
			End Get
		End Property

		Public ReadOnly Property LegalAreaD As Double
			Get
				Return LegalArea * 0.001
			End Get
		End Property


		Public ReadOnly Property DeltaAreaM As Double
			Get
				Return DeltaArea
			End Get
		End Property
		Public ReadOnly Property DeltaAreaD As Double
			Get
				Return DeltaArea * 0.001
			End Get
		End Property

		Public ReadOnly Property ToleranceM As Double
			Get
				Return Tolerance
			End Get
		End Property
		Public ReadOnly Property ToleranceD As Double
			Get
				Return Tolerance * 0.001
			End Get
		End Property
		Public ReadOnly Property DeviationM As Double
			Get
				Return Deviation
			End Get
		End Property
		Public ReadOnly Property DeviationD As Double
			Get
				Return Deviation * 0.001
			End Get
		End Property

		Public ReadOnly Property IsProper() As Boolean
			Get
				Select Case Me.AreaStatus
					Case enAreaStatus.Exact, enAreaStatus.MinusPerm, enAreaStatus.PlusPerm
						Return True
					Case Else
						Return False
				End Select
			End Get
		End Property
		Public ReadOnly Property LegalAreaFromBlock() As Boolean
			Get
				Return mbLegalAreaFromBlock
			End Get
		End Property
		Public ReadOnly Property IsProperText() As String
			Get
				Return zzIsProperText(Me.IsProper, True)
			End Get
		End Property
		Public Sub SetLegalArea(dLegalAreaM As Double, bLegalAreaFromBlock As Boolean)
			LegalArea = dLegalAreaM
			mbLegalAreaFromBlock = bLegalAreaFromBlock
		End Sub
		Public Sub CalculateArea(ByVal taParcelArea As IEnumerable(Of ParcelArea), ByVal iRoundDigit As Integer)
			Dim dLegalAreaM As Double = 0.0
			Dim dAcadAreaM As Double = 0.0
			For Each tParcelArea As ParcelArea In taParcelArea
				dLegalAreaM += tParcelArea.LegalArea
				dAcadAreaM += tParcelArea.AcadArea
				' System.Windows.Forms.MessageBox.Show(dLegalAreaD.ToString() & vbCrLf & dAcadAreaM.ToString(), "09_439")
			Next
			CalculateArea(dLegalAreaM, dAcadAreaM, iRoundDigit)
		End Sub
		Public Sub CalculateArea(ByVal dLegalArea As Double, ByVal iRoundDigit As Integer)
			LegalArea = dLegalArea

			zzCalculateArea(iRoundDigit)
			'  DMCommon.Debug.MsgBox("09_131a", True, LegalArea, AcadArea)
		End Sub
		Public Sub CalculateArea(ByVal iRoundDigit As Integer, Optional bCond As Boolean = False)
			zzCalculateArea(iRoundDigit, bCond)
		End Sub
		Public Sub CalculateArea(ByVal dLegalAreaM As Double, ByVal dAcadAreaM As Double, ByVal iRoundDigit As Integer)
			LegalArea = dLegalAreaM
			AcadArea = dAcadAreaM
			zzCalculateArea(iRoundDigit)
		End Sub
		Public Sub CalculateRangeNewPlusOld(dParcelLegalArea As Double, iOutpgonsCount As Integer, ByRef dRangeMin As Double, ByRef dRangeMax As Double)
			Const dEqPointMax As Double = 27966.66
			Const dEqPointMin As Double = 27588.89


			Dim dSqrtArea As Double
			'(SQRT(0.64+4.08*A15469)-0.8)/2.04
			If AcadArea <= dEqPointMin Then
				dSqrtArea = (Math.Sqrt(4.008 * AcadArea + 0.64) - 0.8) / 2.004
				dSqrtArea = zzInverseMin(AcadArea, 0.002, 0.8)

			Else
				dSqrtArea = (Math.Sqrt(4.02 * AcadArea + 0.09) - 0.2) / 2.01
				dSqrtArea = zzInverseMin(AcadArea, 0.005, 0.3)
			End If
			dRangeMin = dSqrtArea * dSqrtArea

			If AcadArea <= dEqPointMax Then
				dSqrtArea = (Math.Sqrt(4 * 0.998 * AcadArea + 0.64) + 0.8) / 1.996
				dSqrtArea = zzInverseMax(AcadArea, 0.002, 0.8)

			Else
				dSqrtArea = (Math.Sqrt(4 * 0.995 * AcadArea + 0.09) + 0.2) / 1.99
				dSqrtArea = zzInverseMax(AcadArea, 0.005, 0.3)
				'DMCommon.ExcelLog.SetNextValue(0, "!!Ranges", AcadArea, (Math.Sqrt(4 * (1 - 0.005) * AcadArea + 0.3 * 0.3) + 0.3) / (2 * (1 - 0.005)), dSqrtArea, dSqrtArea * dSqrtArea)
			End If

			dRangeMax = dSqrtArea * dSqrtArea

			If dRangeMin > dParcelLegalArea - iOutpgonsCount Then
				dRangeMin = dParcelLegalArea - iOutpgonsCount
			End If
			mdRangeMin = dRangeMin


			If dRangeMax < dParcelLegalArea - iOutpgonsCount Then
				''''''''''''dRangeMax = dParcelLegalArea - iOutpgonsCount
			End If
			mdRangeMax = dRangeMax
		End Sub
		Public Sub CalculateRangeNew(ByRef dRangeMin As Double, ByRef dRangeMax As Double)
			Const dEqPointMax As Double = 27966.66
			Const dEqPointMin As Double = 27588.89


			Dim dSqrtArea As Double
			'(SQRT(0.64+4.08*A15469)-0.8)/2.04
			If AcadArea <= dEqPointMin Then
				dSqrtArea = (Math.Sqrt(4.008 * AcadArea + 0.64) - 0.8) / 2.004
			Else
				dSqrtArea = (Math.Sqrt(4.02 * AcadArea + 0.09) - 0.2) / 2.01
			End If
			dRangeMin = dSqrtArea * dSqrtArea

			If AcadArea <= dEqPointMax Then
				dSqrtArea = (Math.Sqrt(4 * 0.998 * AcadArea + 0.64) + 0.8) / 1.996
			Else
				dSqrtArea = (Math.Sqrt(4 * 0.995 * AcadArea + 0.09) + 0.2) / 1.99
			End If
			'DMCommon.ExcelLog.SetNextValue(0, "!!Ranges", AcadArea, (Math.Sqrt(4 * 0.998 * AcadArea + 0.64) + 0.8) / 1.996, (Math.Sqrt(4 * 0.995 * AcadArea + 0.09) + 0.2) / 1.99)
			dRangeMax = dSqrtArea * dSqrtArea

			mdRangeMin = dRangeMin
			mdRangeMax = dRangeMax
		End Sub
		Private Function zzInverseMin(dArea As Double, dKfLin As Double, dKfSqrt As Double) As Double
			Return (Math.Sqrt(4 * (1.0 + dKfLin) * dArea + dKfSqrt * dKfSqrt) - dKfSqrt) / (2.0 * (1 + dKfLin))
		End Function
		Private Function zzInverseMax(dArea As Double, dKfLin As Double, dKfSqrt As Double) As Double
			Return (Math.Sqrt(4.0 * (1.0 - dKfLin) * dArea + dKfSqrt * dKfSqrt) + dKfSqrt) / (2.0 * (1.0 - dKfLin))
			'         	(SQRT(4 * (1.0-0.005) * H20 + 0.3*0.3) + 0.3) / (2* (1-0.005))

			'	=(SQRT(4 * (1-0.005) *H11877 + 0.3*0.3) + 0.3) / (2* (1-0.005))
			'=(SQRT(4 * (1-0.005) *H20 + 0.3*0.3) + 0.3) / (2* (1-0.005))
			'(SQRT(4 * 0.998 *G3 + 0.64) + 0.8) / 1.996

		End Function

		Public Sub CalculateRange(ByRef dRangeMin As Double, ByRef dRangeMax As Double)
			Dim dSqrtArea As Double
			'(SQRT(0.64+4.08*A15469)-0.8)/2.04
			dSqrtArea = (Math.Sqrt(4.008 * AcadArea + 0.64) - 0.8) / 2.004
			dRangeMin = dSqrtArea * dSqrtArea
			dSqrtArea = (Math.Sqrt(4 * 0.998 * AcadArea + 0.64) + 0.8) / 1.996
			dRangeMax = dSqrtArea * dSqrtArea
			mdRangeMin = dRangeMin
			mdRangeMax = dRangeMax
		End Sub

		Private Shared Function zzIsProperText(bIsProper As Boolean, bReverse As Boolean) As String
			Const sIsNotProper As String = "לא תקין"
			Const sIsProper As String = "תקין"
			Dim sRes As String
			If bIsProper Then
				sRes = sIsProper
			Else
				sRes = sIsNotProper
			End If
			If bReverse Then
				Return DMCommon.Hebrew.Invert(sRes)
			Else
				Return sRes
			End If



		End Function

		Private Sub zzCalculateArea(ByVal iRoundDigit As Integer, Optional bCond As Boolean = False)
			Dim dLegalAreaM As Double
			CalcArea = Math.Round(AcadArea, iRoundDigit)
			If bCond Then
				dLegalAreaM = ConditionalArea
			Else
				dLegalAreaM = LegalArea
			End If
			If dLegalAreaM > 0.0 Then

				mdAreaM = dLegalAreaM
				DeltaArea = Math.Round(dLegalAreaM - CalcArea, iRoundDigit)
				'Dim dSqrt As Double = Math.Sqrt(10.0 * LegalArea)
				'Dim d1 As Double = 0.003 * dSqrt + 0.005 * LegalArea
				'Dim d2 As Double = 0.008 * dSqrt + 0.002 * LegalArea
				'	Dim dLegalAreaM As Double = LegalArea
				Dim dSqrt As Double = Math.Sqrt(dLegalAreaM)
				Dim d1 As Double = 0.3 * dSqrt + 0.005 * dLegalAreaM
				Dim d2 As Double = 0.8 * dSqrt + 0.002 * dLegalAreaM

				Tolerance = Math.Round(Math.Max(d1, d2), iRoundDigit)
				''''''''''''''''''DMCommon.Debug.ExcelLog.SetNextValue(0, "!Toler", dLegalAreaM, d1, d2, Tolerance)
				'=MAX((0.3*SQRT(B2*1000)+0.005*B2*1000),(0.8*SQRT(B2*1000)+0.002*B2*1000))
				If d1 >= d2 Then
					FormulaType = 1
				Else
					FormulaType = 2
				End If


				Deviation = Math.Max(Math.Round(Math.Abs(Me.DeltaArea) - Me.Tolerance, iRoundDigit), 0.0)

				If Deviation > 0.0 Then


					HasDeviation = True
					If DeltaArea > 0.0 Then
						Need = dLegalAreaM - CalcArea - Tolerance
						AreaStatus = TopoManager.TPlanGraph.enAreaStatus.MinusEx
					Else
						Need = dLegalAreaM - CalcArea + Tolerance
						AreaStatus = TopoManager.TPlanGraph.enAreaStatus.PlusEx
					End If
				Else
					HasDeviation = False
					If DeltaArea = 0.0 Then
						AreaStatus = TopoManager.TPlanGraph.enAreaStatus.Exact
					ElseIf DeltaArea > 0.0 Then
						AreaStatus = TopoManager.TPlanGraph.enAreaStatus.MinusPerm
					Else
						AreaStatus = TopoManager.TPlanGraph.enAreaStatus.PlusPerm
					End If
				End If
			End If

		End Sub
		Public Sub CalculateRangeNewPlus(dParcelLegalArea As Double, iOutpgonsCount As Integer, ByRef dRangeMin As Double, ByRef dRangeMax As Double)
			Const dEqPointMax As Double = 27966.66
			Const dEqPointMin As Double = 27588.89


			Dim dSqrtArea As Double
			'(SQRT(0.64+4.08*A15469)-0.8)/2.04
			If AcadArea <= dEqPointMin Then
				dSqrtArea = (Math.Sqrt(4.008 * AcadArea + 0.64) - 0.8) / 2.004
				dSqrtArea = zzInverseMin(AcadArea, 0.002, 0.8)

			Else
				dSqrtArea = (Math.Sqrt(4.02 * AcadArea + 0.09) - 0.2) / 2.01
				dSqrtArea = zzInverseMin(AcadArea, 0.005, 0.3)
			End If
			dRangeMin = dSqrtArea * dSqrtArea

			'Dim iPlus As Integer = Math.Ceiling(dRangeMin)
			'	Dim iMinus As Integer = Math.Floor(dRangeMin)
			'	Dim dCurrentRangeMin As Double = Math.Floor(dRangeMin)
			Dim dCurrentRangeMin As Double = Math.Round(dRangeMin)

			Dim dTolerance As Double = zzCalcTolerance(dCurrentRangeMin, 1, 0)

			If dCurrentRangeMin + dTolerance < AcadArea Then
				Do
					dCurrentRangeMin += 1
					dTolerance = zzCalcTolerance(dCurrentRangeMin, 1, 0)
					If dCurrentRangeMin + dTolerance >= AcadArea Then
						dRangeMin = dCurrentRangeMin
						Exit Do
					End If

				Loop
			Else
				Do
					dCurrentRangeMin -= 1
					dTolerance = zzCalcTolerance(dCurrentRangeMin, 1, 0)
					If dCurrentRangeMin + dTolerance < AcadArea Then
						dRangeMin = dCurrentRangeMin + 1
						Exit Do
					End If
				Loop
			End If


			If AcadArea <= dEqPointMax Then
				dSqrtArea = (Math.Sqrt(4.0 * 0.998 * AcadArea + 0.64) + 0.8) / 1.996
				dSqrtArea = zzInverseMax(AcadArea, 0.002, 0.8)

			Else
				dSqrtArea = (Math.Sqrt(4.0 * 0.995 * AcadArea + 0.09) + 0.2) / 1.99
				dSqrtArea = zzInverseMax(AcadArea, 0.005, 0.3)
				'DMCommon.ExcelLog.SetNextValue(0, "!!Ranges", AcadArea, (Math.Sqrt(4 * (1 - 0.005) * AcadArea + 0.3 * 0.3) + 0.3) / (2 * (1 - 0.005)), dSqrtArea, dSqrtArea * dSqrtArea)
			End If

			dRangeMax = dSqrtArea * dSqrtArea

			Dim dCurrentRangeMax As Double = Math.Round(dRangeMax)

			dTolerance = zzCalcTolerance(dCurrentRangeMax, 1, 0)

			If dCurrentRangeMax - dTolerance >= AcadArea Then

				Do
					dCurrentRangeMax -= 1
					dTolerance = zzCalcTolerance(dCurrentRangeMax, 1, 0)
					If dCurrentRangeMax - dTolerance < AcadArea Then
						dRangeMax = dCurrentRangeMax
						Exit Do
					End If

				Loop
			Else
				Do
					dCurrentRangeMax += 1
					dTolerance = zzCalcTolerance(dCurrentRangeMax, 1, 0)
					If dCurrentRangeMax - dTolerance >= AcadArea Then
						dRangeMax = dCurrentRangeMax - 1
						Exit Do
					End If
				Loop



			End If


			If dRangeMin > dParcelLegalArea - iOutpgonsCount Then
				dRangeMin = dParcelLegalArea - iOutpgonsCount
			End If
			mdRangeMin = dRangeMin




			If dRangeMax < dParcelLegalArea - iOutpgonsCount Then
				'''''''''	dRangeMax = dParcelLegalArea - iOutpgonsCount
			End If
			mdRangeMax = dRangeMax
		End Sub

		Private Function zzCalcTolerance(dLegalAreaM As Double, ByVal iRoundDigit As Integer, ByRef iFormulaType As Integer) As Double
			Dim dSqrt As Double = Math.Sqrt(dLegalAreaM)
			Dim d1 As Double = 0.3 * dSqrt + 0.005 * dLegalAreaM
			Dim d2 As Double = 0.8 * dSqrt + 0.002 * dLegalAreaM

			If d1 >= d2 Then
				iFormulaType = 1
			Else
				iFormulaType = 2
			End If
			Return Math.Round(Math.Max(d1, d2), iRoundDigit)
		End Function

		'	Public miAreaStatus As AreaStatus
	End Structure


End Namespace
