Option Explicit On
Option Strict On
Imports DMAcadExt
Imports System.Data
Namespace TPlanGraph
   Public Enum enOwnerBlockType
      Unknown
      Owner
      Lot
   End Enum
   Public Structure OwnerPgonData
      Shared BlockType As enOwnerBlockType
      Dim OwnerName As String
      Dim OwnerID As Integer
      Dim OwnerPgonNo As Integer
      Dim OwnerPgonLetter As String
      Dim IsPublic As Boolean
      Dim ErrText As String
      Dim Exists As Boolean
      Public Event ErrMessage(sText As String)
      Public Sub New(saValues() As String)
         Select Case BlockType
            Case enOwnerBlockType.Owner
               zzNewOwnerBlock(saValues)
            Case enOwnerBlockType.Lot
               zzNewLotBlock(saValues)
         End Select
      End Sub

      Private Sub zzNewOwnerBlock(saValues() As String)
         If saValues IsNot Nothing Then
            '  DMCommon.ExcelLogA.SetNextArray(saValues, 0)
            Dim iAttribUB As Integer = -1
            Try
               If saValues IsNot Nothing Then
                  iAttribUB = saValues.GetUpperBound(0)
               End If
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnOwnerPgon - New_1")
            End Try
            If iAttribUB >= 0 Then
               Try
                  Dim sVal As String = saValues(0).Trim()
                  '	MessageBox.Show(sVal, "01_561")
                  If Integer.TryParse(sVal, OwnerID) Then

                  End If
                  '	MessageBox.Show(CStr(OwnerID), "01_562")
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnOwnerPgon - New_2")
               End Try
            End If
            If iAttribUB >= 1 Then
               Try
                  zzParseOwnerPgonLetter(saValues(1))
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message & ":" & saValues(1), "TplnOwnerPgon - New_3")
               End Try
            End If

            If iAttribUB >= 2 Then
               Try
                  OwnerName = saValues(2).Trim
                  OwnerName = DMCommon.Hebrew.FromAcad(OwnerName)
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnOwnerPgon - New _4")
               End Try
            Else
               TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnOwnerPgon - New_9")
            End If
            Exists = True
         End If

      End Sub

    
      Private Sub zzNewLotBlock(saValues() As String)
         If saValues IsNot Nothing Then
            Dim iAttribUB As Integer = -1
            Try
               If saValues IsNot Nothing Then
                  iAttribUB = saValues.GetUpperBound(0)
               End If
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnOwnerPgon - New_11")
            End Try
            If iAttribUB >= 0 Then
               Try
                  Dim sVal As String = saValues(0).Trim()
                  '	MessageBox.Show(sVal, "01_561")
                  ' nnn()
                  zzParseOwnerID_PgonLetter(sVal)
                  '	MessageBox.Show(CStr(OwnerID), "01_562")
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnOwnerPgon - New_12")
               End Try
            End If
            If iAttribUB >= 1 Then
               
            End If

            If iAttribUB >= 2 Then
              
            Else
               TplnProject.WriteMessageBox("Attribute problem: " & CStr(iAttribUB), "TplnLot - New_9")
            End If
            Exists = True
         End If

      End Sub
      Private Sub zzParseOwnerID_PgonLetter(ByVal sValue As String)
         Dim bEmpty As Boolean
         If sValue Is Nothing Then
            bEmpty = True

         Else
            sValue.Trim()
            If sValue.Length = 0 Then
               bEmpty = True

            Else
               bEmpty = False
            End If
         End If
         If bEmpty Then

            OwnerPgonNo = -1
            OwnerPgonLetter = String.Empty
            IsPublic = True
         Else
            IsPublic = False
            '	MessageBox.Show(":" & sValue & ":", "07_120")
            Dim dOwnerID As Double = Val(sValue)
            If dOwnerID > 0.0 Then
               OwnerID = Convert.ToInt32(dOwnerID)

               Dim sOwnerID As String = Convert.ToString(OwnerID)
               Dim sLetter As String = ""
               ' TplnProject.WriteMessageBox("Attribute problem: " & sValue & ";" & sOwnerID, "AWEQ")
               If sValue.StartsWith(sOwnerID) Then
                  If sValue = sOwnerID Then
                     If OwnerID > 5000 Then
                        OwnerID -= 5000
                        sLetter = "J"
                     ElseIf OwnerID > 4000 Then
                        OwnerID -= 4000
                        sLetter = "I"

                     ElseIf OwnerID > 3000 Then
                        OwnerID -= 3000
                        sLetter = "H"
                     ElseIf OwnerID > 2000 Then
                        OwnerID -= 2000
                        sLetter = "G"

                     ElseIf OwnerID > 1000 Then
                        OwnerID -= 1000
                        sLetter = "F"
                     Else
                        sLetter = "A"
                     End If

                  Else
                     sLetter = sValue.Substring(sOwnerID.Length)
                     End If

                     zzLetterToCode(sLetter)
                  End If
               '   TplnProject.WriteMessageBox(": " & sValue & ";" & sOwnerID & "; '" & sLetter & "'", "AWET")
            End If

         End If

         '	MessageBox.Show(CStr(i) & vbCrLf & CStr(iW) & vbCrLf & sValue, "01_566")
      End Sub
      Private Sub zzParseOwnerPgonLetter(ByVal sValue As String)
         Dim bEmpty As Boolean
         If sValue Is Nothing Then
            bEmpty = True

         Else
            sValue.Trim()
            If sValue.Length = 0 Then
               bEmpty = True

            Else
               bEmpty = False
            End If
         End If
         If bEmpty Then

            OwnerPgonNo = -1
            OwnerPgonLetter = String.Empty
            IsPublic = True
         Else
            IsPublic = False
            '	MessageBox.Show(":" & sValue & ":", "07_120")
            If IsNumeric(sValue) Then
               Dim iValue As Integer
               If Integer.TryParse(sValue, iValue) Then
                  OwnerPgonNo = iValue - 1
                  OwnerPgonLetter = sValue
                  TplnOwner.DisplayType = enDisplayType.Numbers
               End If
            Else
               zzLetterToCode(sValue)
            End If
         End If

         '	MessageBox.Show(CStr(i) & vbCrLf & CStr(iW) & vbCrLf & sValue, "01_566")
      End Sub
      Private Sub zzLetterToCode(ByVal sLetter As String)
         '    Dim iRow As Integer
         Dim iLetterAsc As Integer = Asc(sLetter)

         Dim iCode As Integer
         '	Dim iW As Integer = AscW(sValue)
         If iLetterAsc > 127 Then
            iCode = iLetterAsc - 128
            OwnerPgonNo = DMCommon.Hebrew.GetBaseNum(iCode)
         ElseIf iLetterAsc > 96 Then
            iCode = iLetterAsc - 97
         ElseIf iLetterAsc > 64 Then
            iCode = iLetterAsc - 65
            OwnerPgonNo = iCode
         End If
         If OwnerPgonNo > 0 Then
            ' TplnProject.WriteMessageBox("OwnerPgonNo: " & OwnerPgonNo.ToString() & "; '" & sLetter & "'", "AWEW")
         End If


         If OwnerPgonNo < 0 Or OwnerPgonNo > TplnOwner.OwnerPgonMax Then

            MessageBox.Show("מס' משק " & CStr(OwnerID) & vbCrLf & CStr(iCode) & vbCrLf & sLetter, "01_564a")
            Dim sMsgText As String = "מס' משק " & CStr(OwnerID)
            ErrText = sMsgText
            RaiseEvent ErrMessage(sMsgText)
            OwnerPgonNo = -1
            OwnerPgonLetter = String.Empty
         Else
            OwnerPgonLetter = ChrW(iCode + AscW("א"))
         End If


      End Sub
   End Structure
   Public Class TplnOwnerPgon
      Inherits TPlanGraph.TplnTopoPgon

		Const msOwnerIDAttribTag As String = "NUM"
      Const msOwnerPgonLetterAttribTag As String = "CHAR"
      Const msOwnerNameAttribTag As String = "OWNER"

      Const msOwnerIDPgonLetterAttribTag As String = "CELLNO"
      Const msCodeAttribTag As String = "CODE"
      Const msAreaAttribTag As String = "AREA"
      Const msPlanAttribTag As String = "PLAN"




      Const miDigitRounding As Integer = 3
      Private Enum enAttribIndex
         OwnerID
         OwnerPgonLetter
         OwnerName
         OwnerIDPgonLetter = 0
         Code = 1
         Area = 2
         Plan = 3
         '	UB = OwnerName
      End Enum
      Private Const msOwnerIDFieldName As String = "OwnerID"
      Private Const msOwnerNameFieldName As String = "OwnerName"
      Private Const msOwnerPgonNoFieldName As String = "OwnerPgonNo"
      Private Const msOwnerPgonLetterFieldName As String = "OwnerPgonLetter"
      '	Private Const msPgonAreaFieldName As String = "OwnerName"



      '	Dim OwnerName As String
      '	Dim OwnerID As Integer
      '	Dim OwnerPgonNo As Integer
      '	Dim OwnerPgonLetter As String
      Private Shared moMainDataTable As System.Data.DataTable

      Private Shared miaBlockAttribIndex() As Integer
      '	Private Shared miaApprovedBlockAttribIndex(99) As Integer
      Private mtOwnerPgonData As OwnerPgonData
      '	Dim miaApprovedBlockAttribIndex As Integer()
      Private mdRoundedArea As Double

      Public Shared Sub Initialize(tOwnerMapThemeData As DMAcadExt.MapThemeData)
         Try

            InitTopo(tOwnerMapThemeData)
            Dim saBlockAttribTag() As String = GetCentroidAttribTag()
            Dim msCentroidBlockName As String = ""
            ReDim miaBlockAttribIndex(2)
            '	Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName)
            If saBlockAttribTag IsNot Nothing Then
               '   DMCommon.Functions.DispArray(saBlockAttribTag, "01_534", True)
               For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
                  Select Case saBlockAttribTag(iAttribIndex)
                     Case msOwnerIDAttribTag
                        miaBlockAttribIndex(enAttribIndex.OwnerID) = iAttribIndex
                        OwnerPgonData.BlockType = enOwnerBlockType.Owner
                     Case msOwnerPgonLetterAttribTag
                        miaBlockAttribIndex(enAttribIndex.OwnerPgonLetter) = iAttribIndex
                     Case msOwnerNameAttribTag
                        miaBlockAttribIndex(enAttribIndex.OwnerName) = iAttribIndex
                     Case msOwnerIDPgonLetterAttribTag
                        miaBlockAttribIndex(enAttribIndex.OwnerIDPgonLetter) = iAttribIndex
                        OwnerPgonData.BlockType = enOwnerBlockType.Lot
                     Case msOwnerIDPgonLetterAttribTag
                        miaBlockAttribIndex(enAttribIndex.Code) = iAttribIndex
                     Case msOwnerIDPgonLetterAttribTag
                        miaBlockAttribIndex(enAttribIndex.Area) = iAttribIndex
                     Case msOwnerIDPgonLetterAttribTag
                        miaBlockAttribIndex(enAttribIndex.Plan) = iAttribIndex




                  End Select
               Next

               '	Erase saBlockAttribTag
            End If
            '  System.Windows.Forms.MessageBox.Show(OwnerPgonData.BlockType.ToString(), "05_111")
            '07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnOwnerPgon - Initialize")
         End Try
      End Sub
      Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         MyBase.New(oPolygon, True)
         MyBase.SetAttributeOrder()
         AddHandler mtOwnerPgonData.ErrMessage, AddressOf zzAddMessage

         '	MyBase.SetAttributeOrder(BlockAttribIndex)
         '	DMCommon.Functions.DispArray(dsaBlockAttribText, "01_542m", True)
         mtOwnerPgonData = New OwnerPgonData(dsaBlockAttribText)
         If Not String.IsNullOrEmpty(mtOwnerPgonData.ErrText) Then
            zzAddMessage(mtOwnerPgonData.ErrText)
         End If
         mdRoundedArea = Math.Round(MyBase.AcadArea(True), miDigitRounding, MidpointRounding.AwayFromZero)
      End Sub
      Private Sub zzAddMessage(sMsgText As String)
         Dim oCenter As TPlnPoint = New TPlnPoint(Me.ddCentroidX, Me.ddCentroidY)

         DMAcadExt.AppMessages.AddMessage(True, oCenter, Me.BoundingBox, "", sMsgText, False)
      End Sub
      Public ReadOnly Property Data As OwnerPgonData
         Get
            Return mtOwnerPgonData
         End Get
      End Property
      Public Shared Sub CreateMainDataTable()
         moMainDataTable = New DataTable("Ownership")
         '	moMainColumnIndices = New Dictionary(Of Integer, Integer)



         With moMainDataTable.Columns
            .Add(msOwnerIDFieldName, GetType(System.Int32))                      '0
            .Add(msOwnerNameFieldName, GetType(System.String))                      '1
            .Add(msOwnerPgonNoFieldName, GetType(System.Double))                 '2
            .Add(msOwnerPgonLetterFieldName, GetType(System.String))

            .Add(TopoReader.msAreaFldName, GetType(System.Double))
            .Add(TopoReader.msCentroidXFldName, GetType(System.Double))   '32
            .Add(TopoReader.msCentroidYFldName, GetType(System.Double))    '33

            .Add(TopoReader.msPerimeterFldName, GetType(System.Double))    '34
            .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))              '35
            .Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))       '36

         End With
      End Sub
      Public ReadOnly Property RoundedArea As Double
         Get
            Return mdRoundedArea
         End Get
      End Property
      Public Sub AddDataToMainTable()
         If moMainDataTable IsNot Nothing Then
            Dim oNewRow As System.Data.DataRow
            Try
               oNewRow = moMainDataTable.NewRow()
               With oNewRow
                  .Item(msOwnerIDFieldName) = mtOwnerPgonData.OwnerID                '0
                  If mtOwnerPgonData.OwnerName IsNot Nothing Then
                     .Item(msOwnerNameFieldName) = mtOwnerPgonData.OwnerName
                  End If

                  '1
                  .Item(msOwnerPgonNoFieldName) = mtOwnerPgonData.OwnerPgonNo
                  '2
                  If mtOwnerPgonData.OwnerPgonLetter IsNot Nothing Then
                     .Item(msOwnerPgonLetterFieldName) = mtOwnerPgonData.OwnerPgonLetter
                  End If


                  .Item(TopoReader.msAreaFldName) = Me.RoundedArea
                  .Item(TopoReader.msCentroidXFldName) = Math.Round(MyBase.ddCentroidX, 3, MidpointRounding.AwayFromZero)
                  .Item(TopoReader.msCentroidYFldName) = Math.Round(MyBase.ddCentroidY, 3, MidpointRounding.AwayFromZero)

                  .Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
                  .Item(TopoReader.msTopoIDFldName) = MyBase.diTopoID
                  .Item(TopoReader.msAcObjIDFldName) = MyBase.CentroidAcObjID '''''''''''''.OldIdPtr.ToInt64()


               End With
               moMainDataTable.Rows.Add(oNewRow)
            Catch oEx As Exception
               TplnProject.WriteMessageBox(oEx.Message & vbCrLf & oEx.StackTrace, "TplnOwnerPgon - AddDataToMainTable")
            End Try

         End If
      End Sub
      Public Shared ReadOnly Property MainDataTable() As System.Data.DataTable
         Get
            Return moMainDataTable
         End Get
      End Property
      Public Shared ReadOnly Property MainView() As System.Data.DataView
         Get
            Dim sSort As String = msOwnerIDFieldName & "," & msOwnerPgonNoFieldName
            If moMainDataTable IsNot Nothing Then
               Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, DataViewRowState.CurrentRows)
               oDataView.AllowEdit = False
               oDataView.AllowDelete = False
               oDataView.AllowNew = False
               '	System.Windows.Forms.MessageBox.Show(CStr(oDataView.Count), "01_548a")
               Return oDataView
            Else
               Return Nothing
            End If

         End Get
      End Property
      Public Overrides Sub Terminate()

      End Sub

      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            '	MessageBox.Show(CStr(miaBlockAttribIndex IsNot Nothing), "01_571")
            '	DMCommon.Functions.DispArray(miaBlockAttribIndex, "01_537")
            Return miaBlockAttribIndex
         End Get
         
      End Property
      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property

   End Class
End Namespace