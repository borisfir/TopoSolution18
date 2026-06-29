Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TPlanGraph
   Public Structure BlockData
      Const IsAnalyticYes As String = "1"
      Const IsAnalyticNo As String = "0"

      Dim BlockNo As Integer
      Dim BlockAddNo As Integer
      Dim LegalArea As Double
      Dim Status As Integer
      Dim IsAnalytic As Boolean
      Dim Exists As Boolean
      Dim IsError As Boolean
      Public Shared Function GetBlockKey(iBlockNo As Integer, iBlockAddNo As Integer) As Integer
         Return 1000 * iBlockNo + iBlockAddNo
      End Function
      Public Sub New(saValues() As String)
         'MessageBox.Show(CStr(saValues IsNot Nothing), "08_549")
         If saValues IsNot Nothing Then
            '  DMCommon.Functions.DispArray(saValues, "04_760", True, "|")
            Dim iAttribUB As Integer = -1

            Try
               If saValues IsNot Nothing Then
                  iAttribUB = saValues.GetUpperBound(0)
                  If iAttribUB < 5 Then
                     Dim sMsg As String = DMCommon.Functions.GetErrorText(1101, CStr(iAttribUB + 1), "6")
                     DMAcadExt.AppMessages.AddMessage(False, 0.0, 0.0, "", sMsg, False)
                     IsError = True
                  End If


               End If
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_1")
            End Try
            If iAttribUB >= 0 Then
               Try
                  Dim sName As String = saValues(0).Trim()
                  Integer.TryParse(sName, BlockNo)

               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_2")
               End Try
            End If
            If iAttribUB >= 2 Then
               Try
						Dim sAddName As String = saValues(2).Trim()
						If Not String.IsNullOrEmpty(sAddName) Then
							Integer.TryParse(sAddName, BlockAddNo)
						End If



					Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_3")
               End Try
            End If

            If iAttribUB >= 3 Then
               Try
                  Dim sLegalArea As String = saValues(3).Trim()
                  '  TplnProject.WriteMessageBox(CStr(sLegalArea), "sLegalArea")
                  If sLegalArea.Length > 0 Then
                     Double.TryParse(sLegalArea, LegalArea)
                  End If

               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_4")
               End Try
            Else
               TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnBlock - New_9")
            End If

            If iAttribUB >= 4 Then
               Try
                  Dim sValue As String = saValues(4).Trim()
                  '    MessageBox.Show("AttribUB = " & CStr(iAttribUB) & vbCrLf & saValues(0).Trim() & vbCrLf & saValues(1).Trim() & vbCrLf & saValues(2).Trim() & vbCrLf & saValues(3).Trim() & vbCrLf & sValue, "07_330")
                  Select Case sValue
                     Case "6"
                        Status = 6
                     Case "20"
                        Status = 20
                     Case Else
                        Status = 0
                  End Select
                  '  MessageBox.Show(sValue & vbCrLf & Status.ToString(), "04_572")
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_5")
               End Try
            Else
               TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnBlock - New_11")
            End If

            If iAttribUB >= 5 Then
               Try
                  Dim sValue As String = saValues(5).Trim()
                  '  MessageBox.Show(sValue, "04_569")
                  ParseAnalitic(sValue)


               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - New_6")
               End Try
            Else
               TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnBlock - New_9")
            End If

            Exists = True
         End If
         '   DMCommon.Functions.DispArray(saValues, "04_770 END", True, vbCrLf)
      End Sub
      Public Sub ParseAnalitic(sValue As String)
         Select Case sValue
            Case IsAnalyticNo
               IsAnalytic = False
            Case IsAnalyticYes
               IsAnalytic = True
            Case Else
               IsAnalytic = False
         End Select
      End Sub
      Public ReadOnly Property IsAnalyticStr() As String
         Get
            If IsAnalytic Then
               Return IsAnalyticYes
            Else
               Return IsAnalyticNo
            End If
         End Get
       
      End Property
      Public ReadOnly Property BlockKey() As Integer
         Get
            Return GetBlockKey(BlockNo, BlockAddNo)
         End Get
      End Property

      Private Shared Function zzNN(sVal As String) As String
         If sVal Is Nothing Then
            Return "<Nothing>"
         Else
            Return sVal
         End If
      End Function

   End Structure

   Public Class TplnBlock
      Inherits TPlanGraph.TplnDissolvePgon


#Region "Constants"
		'	Public Const IsAnalitic As String = "מוסדר"
		'	Public Const IsNotAnalitic As String = "לא מוסדר"

		Private Enum enBlockCentroidAttribIndices
         BlockNo
         Slash
         BlockAddNo
         LegalArea
         Status
         IsAnalytic
         UB = IsAnalytic
      End Enum
		Public Const StatusNameYes As String = "מוסדר"
		Public Const StatusNameNo As String = "לא מוסדר"

		Private Const miAcadBlockID As DMAcadExt.enAcadBlocks = DMAcadExt.enAcadBlocks.Block
      Private Const msBlockNoAttribTag As String = "LOT_NUM"
      Private Const msSlashAttribTag As String = "SLASH"
      Private Const msBlockAddNoAttribTag As String = "GUSH_SUFFI"

      Private Const msLegalAreaAttribTag As String = "LEGAL_AREA"
      Private Const msStatusAttribTag As String = "STATUS"
      Private Const msIsAnalyticAttribTag As String = "ANALYTIC"
#End Region


      Private mtBlockData As BlockData
      '	Private miBlockNo As Integer
      Private miBlockAddNo As Integer

      Private mdLegalArea As Double = 0.0
      '   Private miBlockStatus As Integer
      '  Private mbIsAnalytic As Boolean
      Private Shared mtBlockMapThemeData As DMAcadExt.MapThemeData
      Private Shared msCentroidBlockName As String '!!!!!! 

      Private mdicParcels As TPlanGraph.TplnParcels
      Private mdSumParcelLegalArea As Double = 0.0
      Private mdSumArea As Double = 0.0
      Private mdSumLegalArea As Double = 0.0
      Private mbIsCentroid As Boolean

      Private Shared moAcadBlockDef As DMAcadExt.AcadBlockDef
      Private Shared miaAttributesID() As DMAcadExt.enAcadAttributes = {DMAcadExt.enAcadAttributes.NameNum, DMAcadExt.enAcadAttributes.LegalArea, DMAcadExt.enAcadAttributes.BlockStatus}
      Private Shared miaBlockAttribIndex(enBlockCentroidAttribIndices.UB) As Integer
      '		Private mdgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute = {New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetBlockNo), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLegalArea), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetBlockType)}

      Private Shared mdicBlockType As DMCommon.ItemDataDict
		Private Shared moBlockTable As System.Data.DataTable
		Public Shared Function GetBlockFolder(iBlock As Integer, iBlockAdd As Integer) As String
			Return DMCommon.Functions.GetComplexName(iBlock, iBlockAdd, "_")

		End Function
		Public Sub New(ByVal tBlockAcObjId As ObjectId)
         MyBase.New(tBlockAcObjId)


         zzNew()
         '  diCentroidAcObjID = tBlockAcObjId
         '25/08/10	Me.LoadBlockRefData(tBlockAcObjId)
         Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRefForRead(tBlockAcObjId, False, False)
         MyBase.ddCentroidX = oBlockRef.Position.X
         MyBase.ddCentroidY = oBlockRef.Position.Y

         Me.zzLoadBlockRefData(tBlockAcObjId)
      End Sub
      Public Sub Join(ByVal tBlockAcObjId As ObjectId)
         dtCentroidAcObjID = tBlockAcObjId
         Me.zzLoadBlockRefData(tBlockAcObjId)
      End Sub
      '054-4909269
      Private Sub zzLoadBlockRefData(ByVal tBlockAcObjId As ObjectId)
         Dim iaBlockAttribIndex() As Integer = miaBlockAttribIndex '{0, 2, 3, 4, 5}
         '	ddgaParseAttribute = mdgaParseAttribute
         '''''''''''''''''''	dsaBlockAttribText = moAcadBlockDef.GetBlockInfo(tBlockAcObjId, moAcadBlockDef.AttributesID, dbAcadPoint)
         dsaBlockAttribText = DMAcadExt.AcadTransaction.GetAttribText(MyBase.dtCentroidAcObjID, True, dbAcadPoint, iaBlockAttribIndex)



         If IsArray(dsaBlockAttribText) Then

            '  DMCommon.Functions.DispArray(dsaBlockAttribText, "LoadBlockRefData", True, "|" & vbCrLf)
            mtBlockData = New BlockData(dsaBlockAttribText)
            If mtBlockData.IsError Then
               DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidX, "", "Attribute problem 2", False)
            End If

            If False Then


               If dsaBlockAttribText.GetUpperBound(0) >= 4 Then
                  If dsaBlockAttribText(0) IsNot Nothing AndAlso dsaBlockAttribText(1) IsNot Nothing AndAlso dsaBlockAttribText(2) IsNot Nothing AndAlso dsaBlockAttribText(3) IsNot Nothing AndAlso dsaBlockAttribText(4) IsNot Nothing Then


                     Integer.TryParse(dsaBlockAttribText(0), mtBlockData.BlockNo)
                     Integer.TryParse(dsaBlockAttribText(1), miBlockAddNo)
                     Double.TryParse(dsaBlockAttribText(2), mdLegalArea)
                     Integer.TryParse(dsaBlockAttribText(3), mtBlockData.Status)
                     mtBlockData.IsAnalytic = (dsaBlockAttribText(4).Trim() = "1")
                  End If
               End If
            End If
            '   System.Windows.Forms.MessageBox.Show(CStr(BlockStatus) & vbCrLf & Me.BlockStatusName, "07_301")
         End If
      End Sub
      Public Shared Sub CreateBlockTable()
         If moBlockTable Is Nothing Then
            moBlockTable = New System.Data.DataTable("Blocks")
            With moBlockTable.Columns
					.Add(TplnParcel.BlockFullFieldName, GetType(System.String))

					.Add(TplnParcel.BlockStatusNameFieldName, GetType(System.String))

					.Add(TplnParcel.BlockStatusFieldName, GetType(System.String))
					.Add(TplnParcel.BlockIsAnalyticFieldName, GetType(System.Int32))
					.Add(TopoReader.msAreaFldName, GetType(System.Double))
					.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))

					.Add(TopoReader.msSumPgonAreaFldName, GetType(System.Double))
					.Add(TopoReader.msSumLegalAreaFldName, GetType(System.Double))

					.Add(TopoReader.BasePgonCountFieldName, GetType(System.Int32))
					.Add(TopoReader.BlockExistsFieldName, GetType(System.Boolean))
					.Add(TopoReader.PgonExistsFieldName, GetType(System.Boolean))



					.Add(TopoReader.msCentroidXFldName, GetType(System.Double))
					.Add(TopoReader.msCentroidYFldName, GetType(System.Double))
					.Add(TopoReader.msPerimeterFldName, GetType(System.Double))
					.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
					.Add(TopoReader.msAcObjIDFldName, GetType(ObjectId))
					.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
					.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))
				End With
			Else
				moBlockTable.Clear()
			End If

			Dim oNewRow As System.Data.DataRow
			Dim dicBlocks As TPlanGraph.TplnBlocks = TopoManager.TPlanGraph.TplnProject.Blocks


			Try
				For Each oBlock As TplnBlock In dicBlocks.Values
					oNewRow = moBlockTable.NewRow()
					With oNewRow
						.Item(TplnParcel.BlockFullFieldName) = oBlock.BlockName

						.Item(TplnParcel.BlockStatusFieldName) = oBlock.BlockStatusStr

						.Item(TplnParcel.BlockStatusNameFieldName) = oBlock.BlockStatusName
						.Item(TplnParcel.BlockIsAnalyticFieldName) = oBlock.IsAnalytic

						.Item(TopoReader.BasePgonCountFieldName) = oBlock.Parcels.Count

						.Item(TopoReader.BlockExistsFieldName) = oBlock.BlockExists

						.Item(TopoReader.PgonExistsFieldName) = oBlock.PgonExists

						'      System.Windows.Forms.MessageBox.Show(oBlock.PgonExists.ToString() & vbCrLf & oBlock.BlockExists.ToString(), "05_450")

						.Item(TopoReader.msAreaFldName) = oBlock.AcadArea(False)

						.Item(TplnParcel.LegalAreaFieldName) = oBlock.LegalArea
						.Item(TopoReader.msCentroidXFldName) = oBlock.CentroidX
                  .Item(TopoReader.msCentroidYFldName) = oBlock.CentroidY
                  .Item(TopoReader.msPerimeterFldName) = oBlock.Perimiter
                  .Item(TopoReader.msTopoIDFldName) = oBlock.TopoID
                  If Not oBlock.CentroidAcObjID.IsNull Then
                     .Item(TopoReader.msAcObjIDFldName) = oBlock.CentroidAcObjID   'oBlock.CentroidAcObjID.OldIdPtr.ToInt64()
                  End If
                  .Item(TplnParcel.BlockFieldName) = oBlock.BlockNo
						.Item(TplnParcel.BlockAddFieldName) = oBlock.BlockAddNo

					End With
               moBlockTable.Rows.Add(oNewRow)
            Next

         Catch oEx As Exception
            TplnProject.WriteMessageBox(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBlock - CreateBlockTable")
         End Try

         


      End Sub
      Public Shared Sub Initialize(sCentroidBlockName As String)
         Try
            '  DMAcadExt.AcadDocument.WriteDebugMessage("#18 " & sCentroidBlockName)
            msCentroidBlockName = sCentroidBlockName
            Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
            ' DMCommon.Functions.DispArray(saBlockAttribTag, "!!saBlockAttribTag", True)
            If saBlockAttribTag IsNot Nothing Then
               For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
                  Select Case saBlockAttribTag(iAttribIndex)
                     Case msBlockNoAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.BlockNo) = iAttribIndex
                     Case msSlashAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.Slash) = iAttribIndex
                     Case msBlockAddNoAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.BlockAddNo) = iAttribIndex
                     Case msLegalAreaAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.LegalArea) = iAttribIndex
                     Case msStatusAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.Status) = iAttribIndex
                     Case msIsAnalyticAttribTag
                        miaBlockAttribIndex(enBlockCentroidAttribIndices.IsAnalytic) = iAttribIndex
                  End Select
               Next

            End If

         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnBlock - Initialize")
         End Try
         '    DMCommon.Functions.DispArray(miaBlockAttribIndex, "!!Initialize")
      End Sub
      Public Shared Sub Initialize(tBlockMapThemeData As DMAcadExt.MapThemeData)
         Try

            If msCentroidBlockName Is Nothing Then
               mtBlockMapThemeData = tBlockMapThemeData
               msCentroidBlockName = tBlockMapThemeData.CentroidBlocks

               '  System.Windows.Forms.MessageBox.Show(tBlockMapThemeData.MapThemeID.ToString() & vbCrLf & msCentroidBlockName, "TplnBlock - Initialize")
               Initialize(msCentroidBlockName)

               '  DMCommon.Functions.DispArray(miaBlockAttribIndex, "Block !!iaBlockAttribIndex")
            End If

         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
         End Try

      End Sub
      Public Shared ReadOnly Property BlockTable() As System.Data.DataTable
         Get
            Return moBlockTable
         End Get
      End Property
      Public ReadOnly Property Order() As Long
         Get
            Return GetBlockOrder(mtBlockData.BlockNo, mtBlockData.BlockAddNo)
         End Get
      End Property

      Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         MyBase.New(oPolygon)
         ' 


         MyBase.SetAttributeOrder()   'miaBlockAttribIndex

         '    DMCommon.Functions.DispArray(miaBlockAttribIndex, "iaBlockAttribIndex")
         '    DMCommon.Functions.DispArray(dsaBlockAttribText, "saBlockAttribValue", True)

         '  System.Windows.Forms.MessageBox.Show(MyBase.diCentroidAcObjID.ToString() & vbCrLf & oPolygon.Entity.ToString() & vbCrLf & mbPgonExists.ToString(), "04_422a")
         mtBlockData = New BlockData(dsaBlockAttribText)
         If mtBlockData.IsError Then
            DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidX, "", "Attribute problem 1", False)
         End If


         zzNew()

         '   System.Windows.Forms.MessageBox.Show(MyBase.diCentroidAcObjID.ToString() & vbCrLf & Me.BlockStatusStr & vbCrLf & Me.IsAnalytic.ToString, "04_422b")
      End Sub
      Public Sub New(ByVal oParcel As TplnParcel)
         MyBase.New(oParcel.BlockKey)
         mtBlockData.BlockNo = oParcel.BlockNo

         mtBlockData.BlockAddNo = oParcel.BlockAdd
         '   TplnProject.WriteMessageBox("QQ: " & CStr(Me.BlockNo) & "," & CStr(miBlockAddNo & " = " & Me.BlockName) & "," & CStr(miBlockAddNo), "")
         zzNew()
      End Sub
      Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         MyBase.New(oPolygon, 0, tCentroidAcObjID)
         MyBase.SetAttributeOrder()

         mtBlockData = New BlockData(dsaBlockAttribText)

         '  MyBase.dsName = mtBlockData.BlockName
         '  MyBase.diOrder = mtBlockData.Order
      End Sub

      Public Shared Function GetAttribIndex() As Integer()
         Return miaBlockAttribIndex
      End Function
      Public Sub UpdateCentroid()

         Dim iaBlockAttribIndex() As Integer = miaBlockAttribIndex
         Dim saBlockAttribValue(iaBlockAttribIndex.GetUpperBound(0)) As String

         saBlockAttribValue(enBlockCentroidAttribIndices.BlockNo) = Convert.ToString(mtBlockData.BlockNo)
         iaBlockAttribIndex(enBlockCentroidAttribIndices.BlockAddNo) = -1
         If LegalArea < 0.000001 Then
            saBlockAttribValue(enBlockCentroidAttribIndices.LegalArea) = String.Empty
         Else
            saBlockAttribValue(enBlockCentroidAttribIndices.LegalArea) = Convert.ToString(LegalArea)
         End If

         saBlockAttribValue(enBlockCentroidAttribIndices.Status) = Convert.ToString(BlockStatus)
         If Me.IsAnalytic Then
            saBlockAttribValue(enBlockCentroidAttribIndices.IsAnalytic) = "1"
         Else
            saBlockAttribValue(enBlockCentroidAttribIndices.IsAnalytic) = "0"
         End If

         '   DMCommon.Functions.DispArray(iaBlockAttribIndex, "iaBlockAttribIndex")
         '   DMCommon.Functions.DispArray(saBlockAttribValue, "saBlockAttribValue", True)

         DMAcadExt.AcadTransaction.UpdateAttribText(dtCentroidAcObjID, True, iaBlockAttribIndex, saBlockAttribValue)
      End Sub
      Private Sub zzNew()
         mdicParcels = New TPlanGraph.TplnParcels(False, False)
      End Sub

      Private Sub zzGetLegalArea(ByVal sAttribValue As String)
         Try
            If sAttribValue.Length <> 0 Then
               mdLegalArea = Convert.ToDouble(sAttribValue)
            End If

         Catch oEx As Exception
            TplnProject.WriteMessageBox(oEx.Message & vbCrLf & sAttribValue, "TplnBlock - zzGetLegalArea")
         End Try
      End Sub
      Private Sub zzGetBlockType(ByVal sAttribValue As String)
         Try
            If sAttribValue.Length <> 0 Then
               mtBlockData.Status = Convert.ToInt32(sAttribValue)
            End If
         Catch oEx As Exception
            TplnProject.WriteMessageBox(oEx.Message & vbCrLf & sAttribValue, "TplnBlock - zzGetLegalArea")
         End Try
      End Sub
      Public Shared Sub SharedTerminate()
         If moAcadBlockDef IsNot Nothing Then
            moAcadBlockDef.Terminate()
         End If
         If mdicBlockType IsNot Nothing Then
            mdicBlockType.Clear()
            mdicBlockType = Nothing
         End If
         '''''''''''''''TEMP	Erase miaAttributesID
      End Sub
      Public Sub AddParcel(ByVal oParcel As TplnParcel)
         Try
            mdicParcels.AddParcel(oParcel)
            mdSumParcelLegalArea += oParcel.LegalOrAcadArea(False)

            '	.Item(msInPlanCalcAreaApprFieldName) = MyBase.InPlanCalcArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Approved)
            '	.Item(msInPlanCalcAreaPropFieldName) = MyBase.InPlanCalcArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Proposed)

         Catch oEx As Exception
            TplnProject.WriteMessageBox(oEx.Message, "TplnBlock - AddParcel")
         End Try
      End Sub
      Public Function GetNumerationPair(iOverlayIndex As DMAcadExt.enOverlayIndex) As NumerationPair
         Dim oNumerationPair As NumerationPair
         Dim oParcel As TplnParcel
         oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
         For Each oMerhavParcel As TplnParcel In mdicParcels.Values
            oParcel = TplnProject.GetParcel(oMerhavParcel.TopoID, "GetNumerationPair")
            oNumerationPair.AddComplexNum(oParcel.Name, oParcel.PlanState(iOverlayIndex))
         Next
         Return oNumerationPair
      End Function
      Public ReadOnly Property Parcels As TPlanGraph.TplnParcels
         Get
            Return mdicParcels
         End Get
      End Property


      Public ReadOnly Property HasCentroid() As Boolean
         Get
            Return Not dtCentroidAcObjID.IsNull
         End Get
      End Property
      Public ReadOnly Property SumParcelLegalArea() As Double
         Get
            Return mdSumParcelLegalArea
         End Get
      End Property
      Public ReadOnly Property DifLegalArea() As Double
         Get
            Return mdLegalArea - mdSumParcelLegalArea
         End Get
      End Property
		'Gush Name
		Public Shared Function GetBlockName(iBlock As Integer, iBlockAdd As Integer) As String
			Return DMCommon.Functions.GetComplexName(iBlock, iBlockAdd, "/")

		End Function
		Public Shared Function GetBlockNo(sFullBlockName As String, ByRef iBlock As Integer, ByRef iBlockAdd As Integer) As Boolean
			Return DMCommon.Functions.DecomposeComplexName(sFullBlockName, "/", iBlock, iBlockAdd)
		End Function

		Public Shared Function GetBlockOrder(iBlock As Integer, iBlockAdd As Integer) As Long
         Return Convert.ToInt64(iBlock) * 1000L + Convert.ToInt64(iBlockAdd)
      End Function
      Public Shared Sub Initialize()
         moAcadBlockDef = New DMAcadExt.AcadBlockDef(DMAcadExt.enAcadBlocks.Block)
         moAcadBlockDef.AttributesID = miaAttributesID
         mdicBlockType = TPlServerDB.ServerDB.CurrentServerDB.GetItemDict(TPlServerDB.enListType.BlockType)
         '''''''''''''''''''''temp	moAcadBlockDef.LoadDWG()
      End Sub

      Public Shared ReadOnly Property CentroidBlockName() As String
         Get
            Return msCentroidBlockName
            'moAcadBlockDef.BlockName
         End Get
      End Property
      Public Shared Function GetTopoName() As String
         Return mtBlockMapThemeData.LineTopoName
      End Function
      Public ReadOnly Property BlockName() As String
         Get
            Return GetBlockName(Me.BlockNo, Me.BlockAddNo)
         End Get
      End Property

      Public Property BlockNo() As Integer
         Get
            Return mtBlockData.BlockNo
         End Get
         Set(ByVal iValue As Integer)
            mtBlockData.BlockNo = iValue
         End Set
      End Property
      Public Property BlockAddNo() As Integer
         Get
            Return mtBlockData.BlockAddNo
         End Get
         Set(ByVal iValue As Integer)
            mtBlockData.BlockAddNo = iValue
         End Set
      End Property
      Public ReadOnly Property Key() As Integer
         Get
				' Return Me.BlockNo * 1000 + Me.BlockAddNo
				Return BlockData.GetBlockKey(Me.BlockNo, Me.BlockAddNo)
         End Get

      End Property
      Public Property LegalArea() As Double
         Get
            Return mtBlockData.LegalArea
         End Get
         Set(ByVal dValue As Double)
            mtBlockData.LegalArea = dValue
         End Set
      End Property
      Public Property BlockStatus() As Integer
         Get
            Return mtBlockData.Status
         End Get
         Set(ByVal iValue As Integer)
            mtBlockData.Status = iValue
         End Set
      End Property
      Public Sub UpdateDataByParcel(oParcel As TplnParcel, bUpdateCentroid As Boolean)

         If Me.BlockNo = 0 Then
            Me.BlockNo = oParcel.BlockNo
            Me.BlockAddNo = oParcel.BlockAdd
            If bUpdateCentroid Then
               UpdateCentroid()
            End If
         ElseIf oParcel.BlockNo <> 0 Then
            If Me.BlockNo = oParcel.BlockNo AndAlso Me.BlockAddNo = oParcel.BlockAdd Then
            Else
               Dim sMsgText1 As String = " נתוני גוש "
               '     Dim sMsgText1 As String = "     '|'  נתוני גוש '|' וחלקה"
               Dim sMsgText2 As String = " וחלקה "
               Dim sBlockName As String = "'" & Me.BlockName & "'"
               Dim sParcelName As String = "'" & oParcel.Name & "'"


               Dim sMsgText3 As String = " אינם מתאימים זה לזה "
               '   sMsgText1 = Strings.Replace(sMsgText1, "|", Me.BlockName, , 1)
               '   sMsgText1 = Strings.Replace(sMsgText1, "|", oParcel.Name, , 1)

               DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidY, "", sMsgText1 & sBlockName & sMsgText2 & sParcelName & vbCrLf & sMsgText3, False)
               DMAcadExt.AppMessages.AddMessage(True, oParcel.CentroidX, oParcel.CentroidY, "", sMsgText1 & sBlockName & sMsgText2 & sParcelName & vbCrLf & sMsgText3, False)

            End If

         End If

      End Sub
      Public Property BlockStatusStr() As String
         Get
            Return mtBlockData.Status.ToString()
         End Get
         Set(ByVal sValue As String)
            Dim iValue As Integer
            iValue = Integer.Parse(sValue)
            mtBlockData.Status = iValue
         End Set
      End Property
      Public Property BlockStatusYes() As TriState
         Get
            Select Case BlockStatus
               Case 6
                  Return TriState.True
               Case 20
                  Return TriState.False
               Case Else
                  Return TriState.UseDefault
            End Select

         End Get
         Set(ByVal iValue As TriState)
            Select Case iValue
               Case TriState.True
                  mtBlockData.Status = 6
               Case TriState.False
                  mtBlockData.Status = 20
               Case Else
                  mtBlockData.Status = 0
            End Select

         End Set
      End Property
      Public Property IsAnalytic() As Boolean
         Get
            Return mtBlockData.IsAnalytic
         End Get
         Set(ByVal bValue As Boolean)
            mtBlockData.IsAnalytic = bValue
         End Set
      End Property
      Public Property IsAnalyticStr() As String
         Get

            Return mtBlockData.IsAnalyticStr


         End Get
         Set(ByVal bValue As String)
            mtBlockData.ParseAnalitic(bValue)
         End Set
      End Property

      Public Property IsCentroid() As Boolean
         Get
            Return mbIsCentroid
         End Get
         Set(ByVal bValue As Boolean)
            mbIsCentroid = bValue
         End Set
      End Property
      Public ReadOnly Property BlockStatusName() As String
         Get
            If BlockStatus = 6 Then
               Return StatusNameYes
            Else
               Return StatusNameNo
            End If
         End Get
      End Property
      Public ReadOnly Property BlockTypeName() As String
         Get
            If (mdicBlockType IsNot Nothing) AndAlso mdicBlockType.ContainsKey(mtBlockData.Status) Then
               Return mdicBlockType.Item(BlockStatus).ListDispData
            Else
               Return String.Empty
            End If
         End Get
      End Property

      Public Overrides Sub Terminate()
         If mdicParcels IsNot Nothing Then
            mdicParcels.Terminate()
            mdicParcels.Clear()
            mdicParcels = Nothing
         End If
      End Sub

      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            Return miaBlockAttribIndex
         End Get
      End Property
      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property

      Public Overrides ReadOnly Property BlockExists As Boolean
         Get
            Return BlockNo <> 0 AndAlso HasCentroid
         End Get
      End Property
       
   End Class
End Namespace

