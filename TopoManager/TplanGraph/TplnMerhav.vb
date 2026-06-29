Option Explicit On
Option Strict On
Namespace TPlanGraph
   Public Structure MerhavData
      'Dim Name As String
      Dim Code As Integer
      Dim InputName As String
      Dim Correct As Boolean
      Dim Exists As Boolean

      Public Sub New(saValues() As String)
         If saValues IsNot Nothing Then
            Dim iAttribUB As Integer = -1
            'DMCommon.Functions.DispArray(saValues, "03_954", True)
            Try
               iAttribUB = saValues.GetUpperBound(0)
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_1")
            End Try
            If iAttribUB >= 0 Then
               Try

                  InputName = saValues(0).Trim()

               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_3")
               End Try
            End If


            If iAttribUB >= 1 Then
               Try
                  Dim sCode As String = saValues(1).Trim()

                  '	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
                  If Integer.TryParse(sCode, Code) Then


                     Correct = True

                  End If


               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
               End Try
            End If






            Exists = True
         Else
            System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "MerhavData - 4")
         End If

      End Sub
      Private Shared Function zzNN(sVal As String) As String
         If sVal Is Nothing Then
            Return "<Nothing>"
         Else
            Return sVal
         End If
      End Function

   End Structure

   Public Class TplnMerhav

      Inherits TPlanGraph.TplnBasicPgon
      Public Const msCodeFieldName As String = "MerhavCode"
      Public Const msInputNameFieldName As String = "InputName"
      Public Const msNameFieldName As String = "ExproTypeName"
      '  Private Const msPlanStateFieldName As String = "PlanState"
      Private mtMerhavData As MerhavData


      Private Shared moAcadBlock As DMAcadExt.AcadBlock
      Private Shared mtMerhavMapThemeData As DMAcadExt.MapThemeData
      Private Shared moMainDataTable As System.Data.DataTable
      Private Shared msCentroidBlockName As String
      Private Shared miaBlockAttribIndex(1) As Integer
      Private Const msNameTypeAttribTag As String = "NAME"
      Private Const msCodeAttribTag As String = "CODE"

      Private Shared mdicNames As Dictionary(Of Integer, String)

		Private mdicAllBlocks As TPlanGraph.TplnBlocks
		Private mhsMerhavBlocks As HashSet(Of Integer)
      Private mhsMerhavParcels As HashSet(Of Integer)

      Private moBlockTable As System.Data.DataTable
      Private moParcelTable As System.Data.DataTable


      Public Shared Sub Initialize(tMerhavMapThemeData As DMAcadExt.MapThemeData)
         Try
            zzFillNames()

            mtMerhavMapThemeData = tMerhavMapThemeData
            msCentroidBlockName = mtMerhavMapThemeData.CentroidBlocks

            moAcadBlock = New DMAcadExt.AcadBlock(mtMerhavMapThemeData.CentroidBlock)
            Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
				If saBlockAttribTag IsNot Nothing Then
					'  DMCommon.Functions.DispArray(saBlockAttribTag, "01_599f", True)
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msNameTypeAttribTag
								miaBlockAttribIndex(0) = iAttribIndex
							Case msCodeAttribTag
								miaBlockAttribIndex(1) = iAttribIndex

						End Select
					Next
				Else
					MessageBox.Show("saBlockAttribTag Is Nothing" & vbCrLf & msCentroidBlockName & vbCrLf & tMerhavMapThemeData.TopoPriority, "11_189")
            End If
            moMainDataTable = New System.Data.DataTable("Merhav")
            '07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
         End Try
      End Sub


      Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         MyBase.New(oPolygon)
         MyBase.SetAttributeOrder()
			mtMerhavData = New MerhavData(dsaBlockAttribText)

			Dim sName As String = Nothing
         mdicNames.TryGetValue(Code, sName)

         MyBase.SetName(sName, False)
         mhsMerhavBlocks = New HashSet(Of Integer)()
         mhsMerhavParcels = New HashSet(Of Integer)()


      End Sub

      Public Shared Sub CreateMainDataTable()

         Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
         moMainDataTable = New System.Data.DataTable("Merhav")


         With moMainDataTable.Columns
            .Add(msCodeFieldName, GetType(System.Int32))
            .Add(msInputNameFieldName, GetType(System.String))
            .Add(msNameFieldName, GetType(System.String))

            .Add(TopoReader.msAreaFldName, GetType(System.Double))



            '	.Add(TopoReader.msSumPgonAreaUnFldName, GetType(System.Double))
            .Add(TopoReader.msCentroidXFldName, GetType(System.Double))             '12 8
            .Add(TopoReader.msCentroidYFldName, GetType(System.Double))             '13 9


            .Add(TopoReader.msPerimeterFldName, GetType(System.Double))             '15 11
            .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))                     '16 12
            .Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))              '17 13
            '19 15
         End With
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

      Public Property AllBlocks As TPlanGraph.TplnBlocks
         Get
            Return mdicAllBlocks
         End Get
         Set(dicValue As TPlanGraph.TplnBlocks)
            mdicAllBlocks = dicValue
         End Set
      End Property

      Public ReadOnly Property Code As Integer
         Get
            Return mtMerhavData.Code
         End Get

      End Property
      Public ReadOnly Property InputName As String
         Get
            Return mtMerhavData.InputName
         End Get

      End Property
      Public ReadOnly Property BlockCount As Integer
         Get

            Return mhsMerhavBlocks.Count
         End Get

      End Property
      Public Sub CalculateBlocks_030717(iOverlayIndex As DMAcadExt.enOverlayIndex)
         Const iMaxLength As Integer = 16


         Dim oNumerationPair As NumerationPair
         Dim oBlock As TplnBlock = Nothing
         moBlockTable = TplnParcel.CreateBlockTable()
         Dim oNewRow As System.Data.DataRow
         '   MessageBox.Show(mhsMerhavBlocks.Count.ToString() & vbCrLf & mdicAllBlocks.Count.ToString(), "09_548")
         For Each iBlockID As Integer In mhsMerhavBlocks
            Try
               oBlock = mdicAllBlocks.Item(iBlockID)
            Catch ex As Exception
            End Try

            If oBlock IsNot Nothing Then
               Try
                  oNewRow = moBlockTable.NewRow()
                  With oNewRow
							oNewRow.Item(TplnParcel.BlockFullFieldName) = oBlock.BlockName
							' TplnProject.WriteMessageBox(oBlock.BlockName, "WW: ")


							If oBlock IsNot Nothing Then
								.Item(TplnParcel.BlockFieldName) = oBlock.BlockNo
								.Item(TplnParcel.BlockAddFieldName) = oBlock.BlockAddNo

								oNumerationPair = oBlock.GetNumerationPair(iOverlayIndex)
								.Item(TplnParcel.BlockStatusFieldName) = 6 'oBlock.BlockStatus


								.Item(TplnParcel.BlockStatusNameFieldName) = oBlock.BlockStatusName

								.Item(TopoReader.msAreaFldName) = oBlock.AcadArea(False)
								.Item(TplnParcel.LegalAreaFieldName) = oBlock.LegalArea
								.Item(TopoReader.msCentroidXFldName) = oBlock.CentroidX
								.Item(TopoReader.msCentroidYFldName) = oBlock.CentroidY
								.Item(TopoReader.msPerimeterFldName) = oBlock.Perimiter
								.Item(TopoReader.msTopoIDFldName) = oBlock.TopoID
								.Item(TopoReader.msAcObjIDFldName) = oBlock.CentroidAcObjID ''''''''''''''.OldIdPtr.ToInt64()
								.Item(TplnParcel.msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
								.Item(TplnParcel.msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)
							End If

							'    .Item(TplnParcel.msSumAreaFldName) = dSumArea
							'    .Item(TplnParcel.msSumLegalAreaFldName) = dSumLegalArea
							'    .Item(TplnParcel.msInPlanCalcAreaApprMergeFieldName) = dInPlanCalcAreaAppr
							'   .Item(TplnParcel.msInPlanCalcAreaPropMergeFieldName) = dInPlanCalcAreaProp

						End With
						moBlockTable.Rows.Add(oNewRow)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddBlockToTable")

					End Try
				End If
			Next
			'  System.Windows.Forms.MessageBox.Show(mhsMerhavBlocks.Count.ToString() & vbCrLf & moBlockTable.Rows.Count.ToString(), "07_043")
		End Sub
		Public Sub CalculateBlocks(iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim oParcel As TplnParcel = Nothing
			Dim oBlock As TplnBlock = Nothing
			zzCreateParcelTable()

			Dim oNewRow As System.Data.DataRow
			'  DMCommon.Debug.MsgBox("09_548", False, mhsMerhavBlocks.Count.ToString() & vbCrLf & mhsMerhavParcels.Count.ToString() & vbCrLf & mdicAllBlocks.Count.ToString())
			'  DMCommon.Debug.MsgBox("09_5489", False, mhsMerhavBlocks.Count.ToString() & vbCrLf & mhsMerhavParcels.Count.ToString() & vbCrLf & mdicAllBlocks.Count.ToString())

			For Each iParcelID As Integer In mhsMerhavParcels
				Try
					oParcel = TplnProject.GetParcel(iParcelID)
				Catch oEx As Exception
				End Try

				If oParcel IsNot Nothing Then
					Try
						oNewRow = moParcelTable.NewRow()
						With oNewRow
							.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
							.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo
							.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
							.Item(TplnParcel.NameFieldName) = oParcel.Name
							.Item(TplnParcel.zzGetPlanStateFieldName(iOverlayIndex)) = oParcel.MerhavPlanState(iOverlayIndex)
							.Item(TopoReader.msTopoIDFldName) = oParcel.TopoID
							.Item(TplnParcel.ParcelOrderFieldName) = oParcel.Order


							moParcelTable.Rows.Add(oNewRow)
						End With
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - zzAddBlockToTable")

					End Try
				End If
			Next


			moBlockTable = TplnParcel.CalculateContentBlocks(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, moParcelTable)
			'    DMCommon.ExcelLogY.SetNextValue(30)
			'    DMCommon.ExcelLogY.SetNextValue(5, "-----", "moParcelTable", "-----")
			'   DMCommon.ExcelLogY.SetDataTable(moParcelTable, 0)
			'   DMCommon.ExcelLogY.SetNextValue(30)
			'  System.Windows.Forms.MessageBox.Show(mhsMerhavBlocks.Count.ToString() & vbCrLf & moBlockTable.Rows.Count.ToString(), "07_043")
		End Sub
		Public Sub AddParcel(ByVal oParcel As TplnParcel)
			' Dim oBlock As TplnBlock = Nothing
			'If Not mdicBlocks.TryGetValue(oParcel.BlockKey, oBlock) Then
			'   oBlock = New TplnBlock(oParcel)
			'   ' TplnProject.WriteMessageBox("MM: " & CStr(oBlock.BlockNo) & "," & CStr(oBlock.BlockAddNo & " !! " & oBlock.BlockName), "")
			'   mdicBlocks.AddBlock(oBlock, False)

			'End If
			If Not mhsMerhavBlocks.Contains(oParcel.BlockKey) Then
				mhsMerhavBlocks.Add(oParcel.BlockKey)
			End If
			If Not mhsMerhavParcels.Contains(oParcel.TopoID) Then
				mhsMerhavParcels.Add(oParcel.TopoID)
			End If


			'    oBlock.AddParcel(oParcel)
			'TplnProject.WriteMessageBox("SS: " & CStr(oBlock.BlockNo) & "," & CStr(oBlock.BlockAddNo & " = " & oBlock.BlockName), "")
		End Sub
		Public ReadOnly Property BlockView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As System.Data.DataView
			Get
				Dim sSort As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName
				Dim sMsg As String = ""

				'	MessageBox.Show(CStr(iOverlayIndex) & vbCrLf & "____________" & vbCrLf & sMsg, "01_327")

				'	System.Windows.Forms.MessageBox.Show(CStr(moBlockTable IsNot Nothing), "01_387f")
				If moBlockTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moBlockTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
					'  MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(oDataView.Count), "01_323")

					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False

					Return oDataView
				Else
					Return Nothing
				End If

			End Get
		End Property
		Private Shared Sub zzFillNames()
			Dim sComText As String = "SELECT  ID, Name  FROM  dbo.Committees"
			mdicNames = New Dictionary(Of Integer, String)

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			If oDataReader IsNot Nothing Then



				Do While oDataReader.Read
					mdicNames.Add(oDataReader.GetInt32(0), oDataReader.GetString(1))
				Loop
				oDataReader.Close()
			End If

		End Sub

		Public Overrides Sub AddDataToMainTable(bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean, bApproved As Boolean, bProposed As Boolean)
			Dim oNewRow As System.Data.DataRow

			If moMainDataTable IsNot Nothing Then
				Try
					oNewRow = moMainDataTable.NewRow()
					'   System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CIntN(Me.Code) & vbCrLf & DMCommon.Functions.CStrN(Me.InputName) & vbCrLf & DMCommon.Functions.CStrN(Me.Name), "01_987d")
					With oNewRow
						.Item(msCodeFieldName) = Me.Code
						.Item(msInputNameFieldName) = Me.InputName
						.Item(msNameFieldName) = Me.Name
						.Item(TopoReader.msAreaFldName) = MyBase.AcadArea(False)
						.Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
						.Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY

						.Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
						.Item(TopoReader.msTopoIDFldName) = MyBase.TopoID
						.Item(TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID
					End With
					moMainDataTable.Rows.Add(oNewRow)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "01_439")
				End Try

			Else
				System.Windows.Forms.MessageBox.Show("moMainDataTable Is  Nothing" & vbCrLf & CStr(2015), "01_488")
			End If
			'// 0-5 Parcel data 

		End Sub
		Public Shared ReadOnly Property MainView() As System.Data.DataView
			Get
				Dim sSort As String = String.Empty
				If moMainDataTable IsNot Nothing Then
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					Return Nothing
				End If
			End Get
		End Property


		Public Overrides Sub Terminate()

		End Sub

		Protected Overrides ReadOnly Property _PolygonFullName As String
			Get
				Return String.Empty
			End Get
		End Property



		Protected Overrides Sub OnCalculate2(iOverlayMethod As DMAcadExt.enOverlayMethod)

		End Sub

		Protected Overrides ReadOnly Property PolygonCaption As String
			Get
				Return Nothing
			End Get
		End Property
		Public Shared Function GetTopoName() As String
			Return mtMerhavMapThemeData.LineTopoName
		End Function
		Private Sub zzCreateParcelTable()
			moParcelTable = New System.Data.DataTable("Parcels")
			'	moMainColumnIndices = New Dictionary(Of Integer, Integer)


			With moParcelTable.Columns
				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))

				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.zzGetPlanStateFieldName(DMAcadExt.enOverlayIndex.ApprFDO_Overlay), GetType(NumerationPair.enComplexType))

				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))
				.Add(TplnParcel.msInPlanCalcAreaApprMergeFieldName, GetType(System.Double))
            .Add(TplnParcel.msInPlanCalcAreaPropMergeFieldName, GetType(System.Double))
            .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int64))


			End With
         '// 0-3 Parcel data 


      End Sub
   End Class
End Namespace