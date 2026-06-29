Option Explicit On
Option Strict On
Namespace TPlanGraph
   Public Enum enExproType
      OutOfArea
      ExistingRoad = 820
      OldDeclaration = 4435
      NewDeclaration = 4434
   End Enum
	Public Enum enExproStatus
		DefaultStatus
	End Enum
	Public Structure ExproData
		'Dim Name As String
		Dim DecNo As String
		Dim ExproTypeID As Integer

		Dim Order As Integer
		Dim DecDate As Date
		Dim Status As enExproStatus
		Dim Note As String

		Dim Correct As Boolean
		Dim Exists As Boolean
		Public Sub New(saValues() As String)

			If saValues IsNot Nothing Then
				Dim iAttribUB As Integer = -1

				Try
					iAttribUB = saValues.GetUpperBound(0)
				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_1")
				End Try
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "Expro!", iAttribUB, saValues(0), saValues(1), saValues(2), saValues(3))



				If iAttribUB >= 0 Then
					Try
						DecNo = saValues(0).Trim()
						'Dim iExproType As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)



					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If
				If iAttribUB >= 1 Then
					Try
						Dim sExproType As String = saValues(1).Trim()

						If Integer.TryParse(sExproType, ExproTypeID) Then
							Correct = True
						End If


					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If

				If iAttribUB >= 2 Then
					Try
						Dim sStatus As String = saValues(2).Trim()
						Dim iStatusID As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sStatus, iStatusID) Then

							If [Enum].IsDefined(GetType(enExproStatus), iStatusID) Then
								Status = CType(iStatusID, enExproStatus)
								Correct = True
							End If
						End If

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_3")
					End Try
				End If
				If iAttribUB >= 3 Then
					Try
						Dim sDecDate As String = saValues(3).Trim()
						Date.TryParse(sDecDate, DecDate)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_4")
					End Try
				End If
				If iAttribUB >= 3 Then
					Note = saValues(3).Trim()
				End If

				'	DMCommon.ExcelLog.SetNextValue(0, "psdl", iAttribUB, saValues(0), saValues(1), saValues(2))
				Exists = True
			Else
				System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "TplnExproData - 13")
			End If

		End Sub
		Public Sub NewRoniA(saValues() As String)

			If saValues IsNot Nothing Then
				Dim iAttribUB As Integer = -1
				DMCommon.Functions.DispArray(saValues, "03_954", True)
				Try
					iAttribUB = saValues.GetUpperBound(0)
				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_1")
				End Try
				If iAttribUB >= 0 Then

					Try
						DecNo = saValues(0).Trim()
						Order = TplnBasicPgon.GetNameOrder(DecNo, True)
						'	MyBase.SetName(saValues(0).Trim(), TplnLot.NameIsNum)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_2")
					End Try
				End If
				If iAttribUB >= 1 Then
					Try
						Dim sExproType As String = saValues(1).Trim()
						Dim iExproType As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sExproType, iExproType) Then

							If [Enum].IsDefined(GetType(enExproType), iExproType) Then
								ExproTypeID = CType(iExproType, enExproType)
								Correct = True
							End If
						End If


					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If


				If iAttribUB >= 2 Then
					Try
						Dim sStatus As String = saValues(2).Trim()
						Dim iStatusID As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sStatus, iStatusID) Then

							If [Enum].IsDefined(GetType(enExproStatus), iStatusID) Then
								Status = CType(iStatusID, enExproStatus)
								Correct = True
							End If
						End If

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_3")
					End Try
				End If
				If iAttribUB >= 3 Then
					Try
						Dim sDecDate As String = saValues(3).Trim()
						Date.TryParse(sDecDate, DecDate)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_4")
					End Try
				End If
				If iAttribUB >= 4 Then
					Note = saValues(4).Trim()
				End If


				Exists = True
			Else
				System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "TplnExproData - 12")
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

	Public Structure ExproDataPrev
		'Dim Name As String
		Dim DecNo As String
		Dim ExproType As enExproType

		Dim Order As Integer
		Dim DecDate As Date
		Dim Status As enExproStatus
		Dim Note As String

		Dim Correct As Boolean
		Dim Exists As Boolean
		Public Sub New(saValues() As String)

			If saValues IsNot Nothing Then
				Dim iAttribUB As Integer = -1

				Try
					iAttribUB = saValues.GetUpperBound(0)
				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_1")
				End Try
				DMCommon.Debug.ExcelLog.SetNextValue(0, "psdl", iAttribUB, saValues(0), saValues(1), saValues(2))
				If iAttribUB >= 0 Then
					Try
						DecNo = saValues(0).Trim()

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If


				If iAttribUB >= 1 Then
					Try
						Dim sExproType As String = saValues(1).Trim()
						Dim iExproType As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sExproType, iExproType) Then

							If [Enum].IsDefined(GetType(enExproType), iExproType) Then
								ExproType = CType(iExproType, enExproType)
								Correct = True
							End If
						End If


					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If


				If iAttribUB >= 2 Then
					Try
						Dim sStatus As String = saValues(2).Trim()
						Dim iStatusID As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sStatus, iStatusID) Then

							If [Enum].IsDefined(GetType(enExproStatus), iStatusID) Then
								Status = CType(iStatusID, enExproStatus)
								Correct = True
							End If
						End If

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_3")
					End Try
				End If
				If iAttribUB >= 3 Then
					Try
						Dim sDecDate As String = saValues(3).Trim()
						Date.TryParse(sDecDate, DecDate)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_4")
					End Try
				End If
				If iAttribUB >= 3 Then
					Note = saValues(3).Trim()
				End If

				'DMCommon.ExcelLog.SetNextValue(0, "psdl", iAttribUB, saValues(0), saValues(1), saValues(2))
				Exists = True
			Else
				System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "TplnExproData - 15")
			End If

		End Sub
		Public Sub NewRoniA(saValues() As String)

			If saValues IsNot Nothing Then
				Dim iAttribUB As Integer = -1
				DMCommon.Functions.DispArray(saValues, "03_954", True)
				Try
					iAttribUB = saValues.GetUpperBound(0)
				Catch oEx As System.Exception
					TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_1")
				End Try
				If iAttribUB >= 0 Then

					Try
						DecNo = saValues(0).Trim()
						Order = TplnBasicPgon.GetNameOrder(DecNo, True)
						'	MyBase.SetName(saValues(0).Trim(), TplnLot.NameIsNum)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnLot - New_2")
					End Try
				End If
				If iAttribUB >= 1 Then
					Try
						Dim sExproType As String = saValues(1).Trim()
						Dim iExproType As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sExproType, iExproType) Then

							If [Enum].IsDefined(GetType(enExproType), iExproType) Then
								ExproType = CType(iExproType, enExproType)
								Correct = True
							End If
						End If


					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_2")
					End Try
				End If


				If iAttribUB >= 2 Then
					Try
						Dim sStatus As String = saValues(2).Trim()
						Dim iStatusID As Integer
						'	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
						If Integer.TryParse(sStatus, iStatusID) Then

							If [Enum].IsDefined(GetType(enExproStatus), iStatusID) Then
								Status = CType(iStatusID, enExproStatus)
								Correct = True
							End If
						End If

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_3")
					End Try
				End If
				If iAttribUB >= 3 Then
					Try
						Dim sDecDate As String = saValues(3).Trim()
						Date.TryParse(sDecDate, DecDate)
					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "TplnExpro - New_4")
					End Try
				End If
				If iAttribUB >= 4 Then
					Note = saValues(4).Trim()
				End If


				Exists = True
			Else
				System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "TplnExproData - 14")
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
	Public Structure ExproArea
		Public ExistingRoad As Double
		Public OldDeclaration As Double
		Public NewDeclaration As Double
		Public Sub New(iExproType As enExproType, dArea As Double)
			Select Case iExproType
				Case enExproType.ExistingRoad
					ExistingRoad = dArea
				Case enExproType.OldDeclaration
					OldDeclaration = dArea
				Case enExproType.NewDeclaration
					NewDeclaration = dArea
			End Select
		End Sub
		Public Shared Operator +(oExproAreaA As ExproArea, oExproAreaB As ExproArea) As ExproArea
			oExproAreaA.ExistingRoad += oExproAreaB.ExistingRoad
			oExproAreaA.OldDeclaration += oExproAreaB.OldDeclaration
			oExproAreaA.NewDeclaration += oExproAreaB.NewDeclaration
			Return oExproAreaA
		End Operator

	End Structure
	Public Class TplnExpro
		Inherits TPlanGraph.TplnBasicPgon

		Public Const msDecNoFieldName As String = "DecNo"
		Public Const msExproTypeIDFieldName As String = "ExproTypeID"
		Public Const msExproTypeNameFieldName As String = "ExproTypeName"

		Public Const msStatusIDFieldName As String = "StatusID"
		Public Const msStatusNameFieldName As String = "StatusName"


		Public Const msDecDateFieldName As String = "DecDate"


		Public Const msNoteFieldName As String = "Note"

		Public Const ParcelExproTypeID As Integer = -5
		'	Public Const msRoundedAreaFieldName As String = "RoundedArea"

		Private Shared mtExproMapThemeData As DMAcadExt.MapThemeData
		Private Shared msCentroidBlockName As String
		Private Shared miaBlockAttribIndex(4) As Integer
		Private Shared moMainHiddenColumns As Dictionary(Of Integer, Integer)

		Private Const msDecNoAttribTag As String = "No"
		Private Const msExproTypeAttribTag As String = "ExproType"
		Private Const msStatusAttribTag As String = "Status"
		Private Const msDateAttribTag As String = "Date"
		Private Const msNoteAttribTag As String = "Note"

		Private mtExproData As ExproData
		Private mtExproDataPrev As ExproDataPrev
		Private mtExproRoundArea As ExproArea
		Private msExproTypeName As String
		Private mbExproTypeExists As Boolean



		Private Shared moMainDataTable As System.Data.DataTable
		Private Shared mdicTypeNames As Dictionary(Of Integer, String)
		Private Shared mdicExproTypes As Dictionary(Of Integer, ExproType)

		Public Class ExproType
			Public ID As Integer
			Public Name As String
			Public OrderBy As Integer
			Public Exists As Boolean
			Public Index As Integer

			Public Sub New(iID As Integer, sName As String, iOrderBy As Integer, bExists As Boolean)
				ID = iID
				Name = sName
				OrderBy = iOrderBy
				Exists = bExists
			End Sub
		End Class
		Public Shared Sub Initialize(tExproMapThemeData As DMAcadExt.MapThemeData)
			Try
				mtExproMapThemeData = tExproMapThemeData
				msCentroidBlockName = mtExproMapThemeData.CentroidBlocks
				'	MessageBox.Show(msCentroidBlockName, "11_179")
				Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
				If saBlockAttribTag IsNot Nothing Then
					'DMCommon.Functions.DispArray(saBlockAttribTag, "01_599d", True)
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msDecNoAttribTag
								miaBlockAttribIndex(0) = iAttribIndex
							Case msExproTypeAttribTag
								miaBlockAttribIndex(1) = iAttribIndex
							Case msStatusAttribTag
								miaBlockAttribIndex(2) = iAttribIndex
							Case msDateAttribTag
								miaBlockAttribIndex(3) = iAttribIndex
							Case msNoteAttribTag
								miaBlockAttribIndex(4) = iAttribIndex

						End Select
					Next

					'		DMCommon.Functions.DispArray(miaBlockAttribIndex, "01_577d")


					'	Erase saBlockAttribTag
				Else
					MessageBox.Show("saBlockAttribTag Is Nothing" & vbCrLf & msCentroidBlockName, "11_182")
				End If

				zzFillExproTypeNames()

				'07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
			End Try
		End Sub
		Private Shared Sub zzFillExproTypeNames()
			Dim sComText As String = "SELECT ID, Name, OrderBy, [Exists] FROM dbo.ExproTypes  Order By OrderBy"
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText, Data.CommandType.Text)
			Dim iExprtoTypeID As Integer
			Dim sExproTypeName As String
			Dim iOrderBy As Integer
			Dim bExists As Boolean
			mdicTypeNames = New Dictionary(Of Integer, String)()
			mdicExproTypes = New Dictionary(Of Integer, ExproType)()
			If oDataReader IsNot Nothing Then
				While oDataReader.Read
					iExprtoTypeID = oDataReader.GetInt32(0)
					sExproTypeName = oDataReader.GetString(1)
					iOrderBy = oDataReader.GetInt32(2)
					bExists = oDataReader.GetBoolean(3)
					mdicTypeNames.Add(iExprtoTypeID, sExproTypeName)
					mdicExproTypes.Add(iExprtoTypeID, New ExproType(iExprtoTypeID, sExproTypeName, iOrderBy, bExists))
				End While
				oDataReader.Close()
			End If
		End Sub
		Public Shared Sub CreateMainDataTable()
			moMainDataTable = New System.Data.DataTable("Expro")

			Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
			moMainHiddenColumns = New Dictionary(Of Integer, Integer)


			With moMainDataTable.Columns
				.Add(msDecNoFieldName, GetType(System.String))
				.Add(msExproTypeIDFieldName, GetType(System.Int32))
				.Add(msExproTypeNameFieldName, GetType(System.String))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(msStatusIDFieldName, GetType(System.Int32))
				.Add(msStatusNameFieldName, GetType(System.String))

				.Add(msDecDateFieldName, GetType(System.DateTime))

				.Add(msNoteFieldName, GetType(System.String))






				'	.Add(TopoReader.msSumPgonAreaUnFldName, GetType(System.Double))
				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))          '12 8
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))          '13 9


				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))          '15 11
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))                 '16 12
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))          '17 13
				'19 15
			End With
			'    DMCommon.Debug.MsgBox("CreateMainDataTable", moMainDataTable.Columns.Count)
		End Sub
		Public Shared Function GetTopoName() As String
			Return mtExproMapThemeData.LineTopoName
		End Function
		Public Shared Function GetExproTypeName(iExproTypeID As Integer) As String
			Dim sExproTypeName As String = Nothing
			If iExproTypeID = ParcelExproTypeID Then
				sExproTypeName = "חלקה"
			ElseIf iExproTypeID < 20 Then
				mdicTypeNames.TryGetValue(iExproTypeID, sExproTypeName)
			Else
				sExproTypeName = "פוליגון" & " " & Chr(223 + iExproTypeID - 20).ToString()
			End If

			Return sExproTypeName
		End Function
		Public Shared Function GetExproType(iExproTypeID As Integer) As ExproType
			Dim sExproTypeName As String = Nothing
			Dim iExproTypeOrder As Integer
			Dim tResExproType As ExproType = Nothing
			If iExproTypeID = ParcelExproTypeID Then
				sExproTypeName = "חלקה"
				iExproTypeOrder = ParcelExproTypeID
				tResExproType = New ExproType(iExproTypeID, sExproTypeName, iExproTypeID, False)
			ElseIf iExproTypeID < 20 Then
				mdicExproTypes.TryGetValue(iExproTypeID, tResExproType)
			Else
				sExproTypeName = "פוליגון" & " " & Chr(223 + iExproTypeID - 20).ToString()
				iExproTypeOrder = iExproTypeID
				tResExproType = New ExproType(iExproTypeID, sExproTypeName, iExproTypeID, False)
			End If

			Return tResExproType
		End Function
		'281024_1
		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon)
			MyBase.SetAttributeOrder()  'miaBlockAttribIndex
			'	MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
			'	
			'DMCommon.Functions.DispArray(dsaBlockAttribText, "03_878", True)
			Dim oExproType As ExproType = Nothing
			mtExproData = New ExproData(dsaBlockAttribText)
			If mdicExproTypes.TryGetValue(mtExproData.ExproTypeID, oExproType) Then
				msExproTypeName = oExproType.Name
				mbExproTypeExists = oExproType.Exists
				If Not mdicTypeNames.ContainsKey(mtExproData.ExproTypeID) Then

					mdicTypeNames.Add(mtExproData.ExproTypeID, msExproTypeName)

				End If
			End If


			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Expro+", diTopoID, mtExproData.ExproTypeID, msExproTypeName)
			If Not mtExproData.Correct Then
				TplnProject.WriteMessageBox("Data is incorrect " & CStr(ddCentroidX) & "," & CStr(ddCentroidY), "TplnExpro - New_17")
			End If

			If False Then
				mtExproDataPrev = New ExproDataPrev(dsaBlockAttribText)
				If Not mtExproDataPrev.Correct Then
					TplnProject.WriteMessageBox("Data is incorrect " & CStr(ddCentroidX) & "," & CStr(ddCentroidY), "TplnExpro - New_14")
				End If
			End If
			'	mtExproRoundArea = New ExproArea(mtExproDataPrev.ExproType, Math.Round(MyBase.AcadArea(False), 3))
		End Sub
		Public ReadOnly Property ExproRoundArea() As ExproArea
			Get
				Return mtExproRoundArea
			End Get
		End Property
		Protected Overrides ReadOnly Property _PolygonFullName As String
			Get
				Return String.Empty
			End Get
		End Property

		Public Overrides Sub AddDataToMainTable(bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean, bApproved As Boolean, bProposed As Boolean)
			Dim oNewRow As System.Data.DataRow

			If moMainDataTable IsNot Nothing Then
				Try
					oNewRow = moMainDataTable.NewRow()


					With oNewRow
						.Item(msDecNoFieldName) = Me.DecNo

						.Item(msExproTypeIDFieldName) = Me.ExproTypeID
						.Item(msExproTypeNameFieldName) = Me.ExproTypeName

						.Item(msStatusIDFieldName) = Me.Status
						.Item(msStatusNameFieldName) = Me.StatusName


						.Item(msDecDateFieldName) = Me.DecDate
						.Item(msNoteFieldName) = Me.Note


						.Item(TopoReader.msAreaFldName) = MyBase.AcadArea(False)
						.Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
						.Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
						'	.Item(msPlanStateFieldName) = CType(Me.PlanState, Integer)
						'	.Item(msPlanStateTextFieldName) = Me.PlanStateText

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
		Public Shared ReadOnly Property TypeNames As Dictionary(Of Integer, String)
			Get
				Return mdicTypeNames
			End Get
		End Property
		Public Shared ReadOnly Property InPlanFilter() As String
			Get
				'  Dim iCriteriaValue As Integer = CType(NumerationPair.enComplexType.Undefined, Integer)
				Dim sFilter As String = msExproTypeIDFieldName & "<> 0"

				Return sFilter
			End Get
		End Property
		Public Shared ReadOnly Property MainHiddenColumns() As Dictionary(Of Integer, Integer)
			Get
				Return moMainHiddenColumns
			End Get
		End Property

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
		Protected Overrides Sub OnCalculate2(iOverlayMethod As DMAcadExt.enOverlayMethod)

		End Sub
		Public ReadOnly Property ExproTypeID() As Integer
			Get
				Return mtExproData.ExproTypeID

			End Get
		End Property
		Public ReadOnly Property ExproTypeName() As String
			Get
				Return zzGetExproTypeName(Me.ExproTypeID)

			End Get
		End Property
		Public ReadOnly Property ExproExists() As Boolean
			Get
				Return mbExproTypeExists

			End Get
		End Property
		Public ReadOnly Property ExproTypePrev() As enExproType
			Get
				Return mtExproDataPrev.ExproType

			End Get
		End Property
		Public ReadOnly Property ExproTypeNamePrev() As String
			Get
				Return zzGetLanduseName(Me.ExproTypePrev)

			End Get
		End Property

		Public ReadOnly Property Status() As enExproStatus
			Get
				Return mtExproDataPrev.Status

			End Get
		End Property
		Public ReadOnly Property StatusName() As String
			Get
				If mtExproDataPrev.Status <> 0 Then
					Return mtExproDataPrev.Status.ToString()
				Else
					Return String.Empty
				End If


			End Get
		End Property

		Public ReadOnly Property DecNo() As String
			Get
				Return mtExproDataPrev.DecNo

			End Get
		End Property
		Public ReadOnly Property DecDate() As Date
			Get
				Return mtExproDataPrev.DecDate

			End Get
		End Property
		Public ReadOnly Property Note() As String
			Get
				Return mtExproDataPrev.Note

			End Get
		End Property
		Public Overrides Function ToString() As String
			Return ExproTypePrev.ToString()
		End Function
		Private Function zzGetExproTypeName(iID As Integer) As String
			Dim sComText As String = "SELECT Name FROM dbo.ExproTypes WHERE ID =" & CStr(iID)



			Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, System.Data.CommandType.Text)
			If oRes IsNot Nothing Then
				Return DirectCast(oRes, String)
			Else
				Return Nothing
			End If
		End Function
		Private Function zzGetLanduseName(iID As Integer) As String
			Dim sComText As String = "SELECT Name FROM dbo.Landuses_1F WHERE ID =" & CStr(iID)
			Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, System.Data.CommandType.Text)
			If oRes IsNot Nothing Then
				Return DirectCast(oRes, String)
			Else
				Return Nothing
			End If
		End Function

		Protected Overrides ReadOnly Property PolygonCaption As String
			Get
				Return String.Empty
			End Get
		End Property
	End Class
End Namespace