Option Explicit On
Option Strict On
Namespace Expro
	'Imports Microsoft.Office.Interop
	Public Class frmExpro
		Private Enum DataStatus
			None
			DB
			Graph
			DB_And_Graph
		End Enum
		Private Enum enOutputColumns
			BlockName
			ParcelNo
			ParcelArea
			ExproTypeBase
		End Enum
		Private Enum enOutputNColumns
			RowIndex
			BlockName
			ParcelNo
			Section
			ParcelArea
			ExproType1
			ExproType4
			ExproType5

			Locality
			UB = Locality
			ExproTypeBase = ExproType1
		End Enum
		Private Class ExproTypeTemp
			Public ID As Integer
			Public OrderBy As Integer
			Public Name As String
			Public Index As Integer
			Public Sub New(iID As Integer)
				ID = iID
			End Sub
		End Class
		Private Structure ParcelCalcType
			Public TopoID As Integer
			Public CalcType As Boolean
			Public Sub New(iTopoID As Integer, bCalcType As Boolean)
				TopoID = iTopoID
				CalcType = bCalcType
			End Sub
		End Structure
		Const msNewVersion As String = "new"
		Const miAfterActionDividerHeight As Integer = 2
		Const msHasDeviationFieldName As String = "HasDeviation"
		Const msHasOutPgonsDeviationFieldName As String = "HasOutPgonsDeviation"


		'Private moJournalDBTable As System.Data.DataTable
		Private moDBExproTable As Data.DataTable
		Private moExproPgonsTable As Data.DataTable

		Private moActiveTable As System.Data.DataTable
		Private WithEvents mfEntConnected As frmEntConnected
		Private WithEvents mfEditLayerList As frmEditLayerList


		Private moMainView As Data.DataView
		Private moExproDataAdapter As Data.Common.DbDataAdapter
		Private mdicParcels As TopoManager.TPlanGraph.TplnParcels
		Private midgvMainLocationY As Integer
		Private miCurrentVersion As Integer
		Private miLastDBVersion As Integer
		Private miaParcelsSorted() As ParcelCalcType

		Private miProjectCode As Integer
		Private miDetailNo As Integer
		Private mdicGlobalExproTypes As SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType) = New SortedDictionary(Of Integer, TopoManager.TPlanGraph.TplnExpro.ExproType)()

		Private miBlockNo As Integer
		Private miBlockAdd As Integer
		Private miParcelNo As Integer

		'	.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo

		Private miCurrentStatus As DataStatus
		Private mbEventsEnabled As Boolean
		Private moDataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle
		Private moDataGridViewBoldCellStyle As System.Windows.Forms.DataGridViewCellStyle
		Private moDataGridViewRedCellStyle As System.Windows.Forms.DataGridViewCellStyle
		Private moDataGridViewCheckCellStyle As System.Windows.Forms.DataGridViewCellStyle

		Private miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcFrmExpro
		Public Sub New()

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			zzMyInitializeComponent()
		End Sub
		Private Sub zzMyInitializeComponent()
			Me.dgvMain.RowHeadersWidth = 24
			Me.dgvMain.AutoGenerateColumns = False

			midgvMainLocationY = Me.dgvMain.Location.Y
			Me.cchExproCalcType.ThreeState = True
			Me.cchExproCalcType.FalseValue = 2

			Me.cchExproCalcType.TrueValue = 1
			Me.cchExproCalcType.IndeterminateValue = 0

			moDataGridViewCellStyle = Me.dgvMain.DefaultCellStyle
			moDataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
			moDataGridViewBoldCellStyle = moDataGridViewCellStyle.Clone
			moDataGridViewBoldCellStyle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			moDataGridViewBoldCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))

			moDataGridViewRedCellStyle = moDataGridViewCellStyle.Clone
			'	moDataGridViewRedCellStyle.Font = New System.Drawing.Font("Tahoma", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(177, Byte))
			moDataGridViewRedCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
			moDataGridViewRedCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter


			moDataGridViewCheckCellStyle = moDataGridViewCellStyle.Clone
			'moDataGridViewCheckCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
			moDataGridViewCheckCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter




			'moDataGridViewBoldCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
			'moDataGridViewBoldCellStyle.BackColor = System.Drawing.Color.FromArgb(CType(CType(208, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(240, Byte), Integer))


		End Sub
		Private Sub frmExpro_Load(oSender As System.Object, e As EventArgs) Handles MyBase.Load
			miProjectCode = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
			miDetailNo = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
			zzLoadVersions()
			zzLoadData()
			mbEventsEnabled = True
		End Sub
		Private Sub zzLoadVersions()
			Dim sSPName As String = "GetExproVersions"
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, System.Data.CommandType.StoredProcedure, zzGetProjectParameters(0))
			'	Dim iVersion As Integer

			If oDataReader IsNot Nothing Then
				Me.cmbVersions.Items.Clear()

				If oDataReader.HasRows Then
					While oDataReader.Read
						miLastDBVersion = oDataReader.GetInt32(0)
						Me.cmbVersions.Items.Add(miLastDBVersion)
					End While
				End If
				oDataReader.Close()
				Me.cmbVersions.Items.Add(msNewVersion)
				If miLastDBVersion > 0 Then
					miCurrentVersion = miLastDBVersion
					Me.cmbVersions.SelectedItem = miCurrentVersion
					miCurrentStatus = DataStatus.Graph Or DataStatus.DB
				Else
					miCurrentVersion = 1
					Me.cmbVersions.SelectedItem = msNewVersion
					miCurrentStatus = DataStatus.Graph
				End If

				'	DMCommon.Debug.MsgBox("13_310c", miCurrentStatus, miLastDBVersion, miCurrentVersion)


			End If
		End Sub
		Private Sub zzSetReadOnly()
			Dim bReadOnly As Boolean
			bReadOnly = (miCurrentStatus And DataStatus.Graph) <> DataStatus.Graph
			Me.cmdOK.Enabled = Not bReadOnly
			Me.dgvMain.ReadOnly = bReadOnly
		End Sub
		Private Sub zzLoadData()



			zzLoadDBExpro()
			zzLoadGraphData()


		End Sub
		Private Sub zzLoadGraphData()
			Const sSPName As String = "GetForcedData"

			Dim oNewRow As System.Data.DataRow

			Dim oResDBRow As Data.DataRow
			Dim iCalcType As Integer
			'DMCommon.Debug.MsgBox("ParcelsCount", miCurrentStatus, DMCommon.Debug.ColCount(TopoManager.TPlanGraph.TplnProject.Parcels))

			mdicParcels = TopoManager.TPlanGraph.TplnProject.Parcels
			Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
			Dim taExproParcelArea() As TopoManager.TPlanGraph.ParcelArea
			Dim taOutPgons() As TopoManager.TPlanGraph.ParcelArea
			Dim bDBVersionsExists As Boolean
			Dim bCond As Boolean
			Dim dTotalAreaIn As Double
			Dim tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType
			Dim bHasDeviation As Boolean
			Dim bHasOutPgonsDeviation As Boolean
			Dim bDebug As Boolean
			Dim oForcedTable As System.Data.DataTable
			Dim oForcedRow As System.Data.DataRow


			Dim iExproTypeID As Integer
			Dim dLegalArea As Double
			Dim dPgonAreaSum As Double

			If (miCurrentStatus And DataStatus.Graph) = DataStatus.Graph Then
				If moExproPgonsTable Is Nothing Then
					moExproPgonsTable = New Data.DataTable("ExproPgons")
					moExproDataAdapter.FillSchema(moExproPgonsTable, Data.SchemaType.Source)
					zzAddCalcFields(True)
				Else
					moExproPgonsTable.Clear()
				End If
				mdicGlobalExproTypes.Clear()

				If mdicParcels IsNot Nothing Then
					If (moDBExproTable IsNot Nothing) AndAlso (moDBExproTable.Rows.Count <> 0) Then
						bDBVersionsExists = True
					End If
					Me.cmbBlockNames.Items.Clear()
					For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						If oParcel.BlockNo = 13948 AndAlso oParcel.ParcelNo = 116 Then
							bDebug = True
						Else
							bDebug = False
						End If

						If oParcel.ExproTypeCount > 0 Then
							dPgonAreaSum = 0.0
							'ssssssssssss
							'oForcedDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, System.Data.CommandType.StoredProcedure, zzGetProjectParcelParameters(oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo))
							oForcedTable = TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sSPName, System.Data.CommandType.StoredProcedure, "ForcedData")
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ForcedDataReader.Has", oForcedDataReader.HasRows, miProjectCode, miDetailNo, oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo)
							oNewRow = moExproPgonsTable.NewRow()
							With oNewRow
								.Item("ProjectCode") = miProjectCode
								.Item("DetailNo") = miDetailNo
								.Item("Version") = miCurrentVersion

								.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
								.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
								.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo

								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID
								tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID)
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy

								.Item("TopoID") = oParcel.TopoID
								If bDBVersionsExists Then
									oResDBRow = moDBExproTable.Rows.Find(zzGetKeys(oNewRow))
									If oResDBRow IsNot Nothing Then
										bCond = DMCommon.Functions.CBoolN(oResDBRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName))
									Else
										bCond = True
									End If
								Else
									bCond = True
								End If
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = bCond
								'kkkkkkkkkkkkk
								.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = oParcel.LegalOrAcadArea(False) '- oParcel.LegalOrAcadArea(False)
								.Item(TopoManager.TopoReader.msAcadAreaFldName) = oParcel.AcadArea(False)
								.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = oParcel.Tolerance(False)
								.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = oParcel.ParcelArea.DeltaAreaM          '4    ' 100619
								.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = oParcel.ParcelArea.DeviationM          '4    ' 100619
								bHasDeviation = (oParcel.ParcelArea.DeviationM > 0.0)
								bHasOutPgonsDeviation = oParcel.HasOutPgonsDeviation
								.Item(msHasDeviationFieldName) = bHasDeviation
								.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
							End With

							If bDebug Then
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproParcel", oParcel.ExproTypeCount, tExproType.Name, tExproType.OrderBy, oParcel.TopoID, oParcel.LegalOrAcadArea(False), oParcel.AcadArea(False) _
																				 , oParcel.ParcelArea.DeltaAreaM, oParcel.ParcelArea.DeviationM, oParcel.BlockNo, oParcel.ParcelNo)
							End If
							moExproPgonsTable.Rows.Add(oNewRow)
							taExproParcelArea = oParcel.ExproParcelArea
							'	ddddddddddddddddddddddddddd

							taOutPgons = oParcel.OutPgons
							If taExproParcelArea IsNot Nothing Then
								If oForcedTable.Rows.Count > 0 Then
									For iIndex As Integer = 0 To taExproParcelArea.GetUpperBound(0)
										oForcedRow = oForcedTable.Rows.Item(iIndex)
										iExproTypeID = DMCommon.Functions.CIntN(oForcedRow.Item(0))
										dLegalArea = DMCommon.Functions.CDblN(oForcedRow.Item(1))
									Next

								Else
									iExproTypeID = 0
								End If



								For iIndex As Integer = 0 To taExproParcelArea.GetUpperBound(0)
									'taExproParcelArea(iIndex).ConditionalArea = 1234.0
									'taExproParcelArea(iIndex).LegalArea = 5678.0

									If oForcedTable.Rows.Count > 0 Then
										oForcedRow = oForcedTable.Rows.Item(iIndex)
										iExproTypeID = DMCommon.Functions.CIntN(oForcedRow.Item(0))
										dLegalArea = DMCommon.Functions.CDblN(oForcedRow.Item(1))
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!ForcedDataReader.Read", iExproTypeID, dLegalArea)
									Else
										iExproTypeID = 0
									End If
									If iExproTypeID = taExproParcelArea(iIndex).ExproType Then
										taExproParcelArea(iIndex).ForcedArea = dLegalArea
										'vvvv
									End If
									'	rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
									oNewRow = moExproPgonsTable.NewRow()
									With oNewRow
										.Item("ProjectCode") = miProjectCode
										.Item("DetailNo") = miDetailNo
										.Item("Version") = miCurrentVersion

										.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
										.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
										.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = taExproParcelArea(iIndex).ExproType
										tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(taExproParcelArea(iIndex).ExproType)
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy
										.Item(msHasDeviationFieldName) = bHasDeviation
										.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation


										taExproParcelArea(iIndex).CalculateArea(3, bCond)
										zzParcelAreaToRowNew(taExproParcelArea(iIndex), bCond, oNewRow)
										''''''''''
										dTotalAreaIn += DMCommon.Functions.CDblN(.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
										If tExproType.ID < 20 AndAlso Not mdicGlobalExproTypes.ContainsKey(tExproType.OrderBy) Then
											mdicGlobalExproTypes.Add(tExproType.OrderBy, tExproType)
										End If

										If bDebug Then
											DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproType", tExproType.Name, tExproType.OrderBy, bHasDeviation, bHasOutPgonsDeviation, taExproParcelArea(iIndex).ExproType)
										End If
										'gggggggggggggggggggggggggggggggggg
										If False Then
											If taExproParcelArea(iIndex).ExproType < 0 Then
												.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = CheckState.Checked
												If bCond Then
													.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 333 'taExproParcelArea(iIndex).ConditionalArea + 10000
												Else
													.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 444 ' taExproParcelArea(iIndex).LegalArea + 1000
												End If

												.Item(TopoManager.TopoReader.msAcadAreaFldName) = taExproParcelArea(iIndex).AcadArea
												.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM
												.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
												.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM
												.Item(msHasDeviationFieldName) = bHasDeviation
												.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
												dTotalAreaIn += DMCommon.Functions.CDblN(.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
											End If
										End If
										'   ffffffffffffffffffffffffffffffffffffffffff
									End With

									If bDebug Then
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproType<0", tExproType.Name, taExproParcelArea(iIndex).ConditionalArea, taExproParcelArea(iIndex).LegalArea, taExproParcelArea(iIndex).AcadArea, tParcelArea.ToleranceM _
																						 , tParcelArea.DeltaAreaM, tParcelArea.DeviationM)
									End If

									moExproPgonsTable.Rows.Add(oNewRow)
								Next
							End If
1:'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
							If False AndAlso (taExproParcelArea IsNot Nothing) AndAlso (taOutPgons IsNot Nothing) Then
								For iPgonIndex As Integer = 1 To taOutPgons.GetUpperBound(0) + 1
									oNewRow = moExproPgonsTable.NewRow()
									With oNewRow
										.Item("ProjectCode") = miProjectCode
										.Item("DetailNo") = miDetailNo
										.Item("Version") = miCurrentVersion

										.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
										.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
										.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo

										tParcelArea = taOutPgons(iPgonIndex - 1)
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!+LegalArea", oParcel.LegalOrAcadArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.Tolerance, tParcelArea.ConditionalArea, tParcelArea.IsForced)

										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = 20 + iPgonIndex
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = "פוליגון" & " " & Chr(223 + iPgonIndex).ToString()


										.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = 0
										.Item(TopoManager.TopoReader.msAcadAreaFldName) = tParcelArea.AcadArea

										''''''''''''''''.Item(zzOutPgonFieldName(iPgonIndex, 0)) = tParcelArea.IsForced

										If iCalcType = 1 Then
											.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 111 'tParcelArea.ConditionalArea
										Else
											.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 222 'tParcelArea.LegalArea
										End If

										.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM

										.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
										.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM
										.Item(msHasDeviationFieldName) = bHasDeviation
										.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
										DMCommon.Debug.ExcelLog.SetNextValue(0, "OutParcArea", oParcel.BlockFull, oParcel.ExtName, iPgonIndex, tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea)
									End With
									DMCommon.Debug.ExcelLog.SetNextValue(0, "3:NewRow", oNewRow.ItemArray)
									moExproPgonsTable.Rows.Add(oNewRow)
								Next
							End If

2:'yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
						End If
					Next oParcel

					'''''''''''''''''''''''''''	DMCommon.Debug.ExcelLog.SetDataTable(0, "moExproPgonsTable", moExproPgonsTable)
					Me.txtTotalAreaIn.Text = FormatNumber(dTotalAreaIn, 0)
					zzSetMainView(String.Empty)
					zzSetLayout()
					zzSortType()
					'DMCommon.Debug.MsgBox("13_310dd", moExproPgonsTable.Rows.Count)
				End If
			End If 'miCurrentStatus = DataStatus.Graph

		End Sub

		Private Sub zzLoadGraphData_250626()
			Const sSPName As String = "GetForcedData"

			Dim oNewRow As System.Data.DataRow

			Dim oResDBRow As Data.DataRow
			Dim iCalcType As Integer
			'DMCommon.Debug.MsgBox("ParcelsCount", miCurrentStatus, DMCommon.Debug.ColCount(TopoManager.TPlanGraph.TplnProject.Parcels))

			mdicParcels = TopoManager.TPlanGraph.TplnProject.Parcels
			Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
			Dim taExproParcelArea() As TopoManager.TPlanGraph.ParcelArea
			Dim taOutPgons() As TopoManager.TPlanGraph.ParcelArea
			Dim bDBVersionsExists As Boolean
			Dim bCond As Boolean
			Dim dTotalAreaIn As Double
			Dim tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType
			Dim bHasDeviation As Boolean
			Dim bHasOutPgonsDeviation As Boolean
			Dim bDebug As Boolean
			Dim oForcedDataReader As System.Data.Common.DbDataReader
			Dim iExproTypeID As Integer
			Dim dLegalArea As Double
			Dim dPgonAreaSum As Double

			If (miCurrentStatus And DataStatus.Graph) = DataStatus.Graph Then
				If moExproPgonsTable Is Nothing Then
					moExproPgonsTable = New Data.DataTable("ExproPgons")
					moExproDataAdapter.FillSchema(moExproPgonsTable, Data.SchemaType.Source)
					zzAddCalcFields(True)
				Else
					moExproPgonsTable.Clear()
				End If
				mdicGlobalExproTypes.Clear()

				If mdicParcels IsNot Nothing Then
					If (moDBExproTable IsNot Nothing) AndAlso (moDBExproTable.Rows.Count <> 0) Then
						bDBVersionsExists = True
					End If
					Me.cmbBlockNames.Items.Clear()
					For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						If oParcel.BlockNo = 13948 AndAlso oParcel.ParcelNo = 116 Then
							bDebug = True
						Else
							bDebug = False
						End If

						If oParcel.ExproTypeCount > 0 Then
							dPgonAreaSum = 0.0

							oForcedDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, System.Data.CommandType.StoredProcedure, zzGetProjectParcelParameters(oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo))
							TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sSPName, System.Data.CommandType.StoredProcedure, "List")
							'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!ForcedDataReader.Has", oForcedDataReader.HasRows, miProjectCode, miDetailNo, oParcel.BlockNo, oParcel.BlockAdd, oParcel.ParcelNo)
							oNewRow = moExproPgonsTable.NewRow()
							With oNewRow
								.Item("ProjectCode") = miProjectCode
								.Item("DetailNo") = miDetailNo
								.Item("Version") = miCurrentVersion

								.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
								.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
								.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo

								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID
								tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID)
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy

								.Item("TopoID") = oParcel.TopoID
								If bDBVersionsExists Then
									oResDBRow = moDBExproTable.Rows.Find(zzGetKeys(oNewRow))
									If oResDBRow IsNot Nothing Then
										bCond = DMCommon.Functions.CBoolN(oResDBRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName))
									Else
										bCond = True
									End If
								Else
									bCond = True
								End If
								.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = bCond

								.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = oParcel.LegalOrAcadArea(False) '- oParcel.LegalOrAcadArea(False)
								.Item(TopoManager.TopoReader.msAcadAreaFldName) = oParcel.AcadArea(False)
								.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = oParcel.Tolerance(False)
								.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = oParcel.ParcelArea.DeltaAreaM          '4    ' 100619
								.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = oParcel.ParcelArea.DeviationM          '4    ' 100619
								bHasDeviation = (oParcel.ParcelArea.DeviationM > 0.0)
								bHasOutPgonsDeviation = oParcel.HasOutPgonsDeviation
								.Item(msHasDeviationFieldName) = bHasDeviation
								.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
							End With

							If bDebug Then
								DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproParcel", oParcel.ExproTypeCount, tExproType.Name, tExproType.OrderBy, oParcel.TopoID, oParcel.LegalOrAcadArea(False), oParcel.AcadArea(False) _
																				 , oParcel.ParcelArea.DeltaAreaM, oParcel.ParcelArea.DeviationM, oParcel.BlockNo, oParcel.ParcelNo)
							End If
							moExproPgonsTable.Rows.Add(oNewRow)
							taExproParcelArea = oParcel.ExproParcelArea
							'	

							taOutPgons = oParcel.OutPgons
							If taExproParcelArea IsNot Nothing Then
								If oForcedDataReader.HasRows Then


								End If



								For iIndex As Integer = 0 To taExproParcelArea.GetUpperBound(0)
									'taExproParcelArea(iIndex).ConditionalArea = 1234.0
									'taExproParcelArea(iIndex).LegalArea = 5678.0

									If oForcedDataReader.Read Then
										iExproTypeID = oForcedDataReader.GetInt32(0)
										dLegalArea = oForcedDataReader.GetDouble(1)
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!ForcedDataReader.Read", iExproTypeID, dLegalArea)
									Else
										iExproTypeID = 0
									End If
									If iExproTypeID = taExproParcelArea(iIndex).ExproType Then
										taExproParcelArea(iIndex).ForcedArea = dLegalArea

									End If
									'	rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
									oNewRow = moExproPgonsTable.NewRow()
									With oNewRow
										.Item("ProjectCode") = miProjectCode
										.Item("DetailNo") = miDetailNo
										.Item("Version") = miCurrentVersion

										.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
										.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
										.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = taExproParcelArea(iIndex).ExproType
										tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(taExproParcelArea(iIndex).ExproType)
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy
										.Item(msHasDeviationFieldName) = bHasDeviation
										.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation


										taExproParcelArea(iIndex).CalculateArea(3, bCond)
										zzParcelAreaToRowNew(taExproParcelArea(iIndex), bCond, oNewRow)
										''''''''''
										dTotalAreaIn += DMCommon.Functions.CDblN(.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
										If tExproType.ID < 20 AndAlso Not mdicGlobalExproTypes.ContainsKey(tExproType.OrderBy) Then
											mdicGlobalExproTypes.Add(tExproType.OrderBy, tExproType)
										End If

										If bDebug Then
											DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproType", tExproType.Name, tExproType.OrderBy, bHasDeviation, bHasOutPgonsDeviation, taExproParcelArea(iIndex).ExproType)
										End If
										'gggggggggggggggggggggggggggggggggg
										If False Then
											If taExproParcelArea(iIndex).ExproType < 0 Then
												.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = CheckState.Checked
												If bCond Then
													.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 333 'taExproParcelArea(iIndex).ConditionalArea + 10000
												Else
													.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 444 ' taExproParcelArea(iIndex).LegalArea + 1000
												End If

												.Item(TopoManager.TopoReader.msAcadAreaFldName) = taExproParcelArea(iIndex).AcadArea
												.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM
												.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
												.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM
												.Item(msHasDeviationFieldName) = bHasDeviation
												.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
												dTotalAreaIn += DMCommon.Functions.CDblN(.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
											End If
										End If
										'   ffffffffffffffffffffffffffffffffffffffffff
									End With

									If bDebug Then
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!!ExproType<0", tExproType.Name, taExproParcelArea(iIndex).ConditionalArea, taExproParcelArea(iIndex).LegalArea, taExproParcelArea(iIndex).AcadArea, tParcelArea.ToleranceM _
																						 , tParcelArea.DeltaAreaM, tParcelArea.DeviationM)
									End If

									moExproPgonsTable.Rows.Add(oNewRow)
								Next
							End If
1:'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
							If False AndAlso (taExproParcelArea IsNot Nothing) AndAlso (taOutPgons IsNot Nothing) Then
								For iPgonIndex As Integer = 1 To taOutPgons.GetUpperBound(0) + 1
									oNewRow = moExproPgonsTable.NewRow()
									With oNewRow
										.Item("ProjectCode") = miProjectCode
										.Item("DetailNo") = miDetailNo
										.Item("Version") = miCurrentVersion

										.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
										.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
										.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo

										tParcelArea = taOutPgons(iPgonIndex - 1)
										DMCommon.Debug.ExcelLog.SetNextValue(0, "!+LegalArea", oParcel.LegalOrAcadArea(False), tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.Tolerance, tParcelArea.ConditionalArea, tParcelArea.IsForced)

										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = 20 + iPgonIndex
										.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = "פוליגון" & " " & Chr(223 + iPgonIndex).ToString()


										.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = 0
										.Item(TopoManager.TopoReader.msAcadAreaFldName) = tParcelArea.AcadArea

										''''''''''''''''.Item(zzOutPgonFieldName(iPgonIndex, 0)) = tParcelArea.IsForced

										If iCalcType = 1 Then
											.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 111 'tParcelArea.ConditionalArea
										Else
											.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = 222 'tParcelArea.LegalArea
										End If

										.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM

										.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
										.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM
										.Item(msHasDeviationFieldName) = bHasDeviation
										.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation
										DMCommon.Debug.ExcelLog.SetNextValue(0, "OutParcArea", oParcel.BlockFull, oParcel.ExtName, iPgonIndex, tParcelArea.LegalArea, tParcelArea.AcadArea, tParcelArea.CalcArea, tParcelArea.ConditionalArea)
									End With
									DMCommon.Debug.ExcelLog.SetNextValue(0, "3:NewRow", oNewRow.ItemArray)
									moExproPgonsTable.Rows.Add(oNewRow)
								Next
							End If
							oForcedDataReader.Close()
2:'yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
						End If
					Next oParcel

					'''''''''''''''''''''''''''	DMCommon.Debug.ExcelLog.SetDataTable(0, "moExproPgonsTable", moExproPgonsTable)
					Me.txtTotalAreaIn.Text = FormatNumber(dTotalAreaIn, 0)
					zzSetMainView(String.Empty)
					zzSetLayout()
					zzSortType()
					'DMCommon.Debug.MsgBox("13_310dd", moExproPgonsTable.Rows.Count)
				End If
			End If 'miCurrentStatus = DataStatus.Graph

		End Sub
		Private Sub zzSetMainView(sFilter As String)
			Dim oMainTable As Data.DataTable

			If (miCurrentStatus And DataStatus.Graph) = DataStatus.Graph Then
				oMainTable = moExproPgonsTable
			Else
				oMainTable = moDBExproTable
			End If
			moMainView = New Data.DataView(oMainTable, sFilter, TopoManager.TPlanGraph.TplnParcel.BlockFieldName & "," & TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName & "," & TopoManager.TPlanGraph.TplnParcel.NameFieldName & "," & TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName, Data.DataViewRowState.CurrentRows)
			'	moMainView = New Data.DataView(oMainTable, "", TopoManager.TPlanGraph.TplnParcel.BlockFieldName & "," & TopoManager.TPlanGraph.TplnParcel.msBlockAddFieldName & "," & TopoManager.TPlanGraph.TplnParcel.NameFieldName & "," & TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName, Data.DataViewRowState.CurrentRows)

			Me.dgvMain.DataSource = moMainView

		End Sub
		Private Sub zzDB_X()
			moMainView = New Data.DataView(moDBExproTable, "", TopoManager.TPlanGraph.TplnParcel.BlockFieldName & "," & TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName & "," & TopoManager.TPlanGraph.TplnParcel.NameFieldName & "," & TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName, Data.DataViewRowState.CurrentRows)
			Me.dgvMain.DataSource = moMainView
			zzSetLayout()
			zzSortType()
		End Sub
		Private Sub zzParcelAreaToRowNew(ByVal tParcelArea As TopoManager.TPlanGraph.ParcelArea, bCond As Boolean, ByRef oRow As Data.DataRow)
			Dim tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType
			With oRow
				.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName) = tParcelArea.ExproType
				tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(tParcelArea.ExproType)

				.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
				.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy
				.Item(TopoManager.TopoReader.msAcadAreaFldName) = tParcelArea.AcadArea

				If bCond Then
					.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = tParcelArea.ConditionalArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = bCond
				Else
					.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = tParcelArea.LegalOrForcedArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = Not tParcelArea.IsForced
				End If

				.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM
				.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
				.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM

			End With

		End Sub

		Private Sub zzParcelAreaToRow_AAA(ByVal tParcelArea As TopoManager.TPlanGraph.ParcelArea, bCond As Boolean, ByRef oRow As Data.DataRow)
			With oRow



				.Item(TopoManager.TopoReader.msAcadAreaFldName) = tParcelArea.AcadArea


				If bCond Then
					.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = tParcelArea.ConditionalArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = bCond
				Else
					.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName) = tParcelArea.LegalArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = Not tParcelArea.IsForced
				End If

				.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM

				.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM          '4    ' 100619
				.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM

			End With

		End Sub

		Private Sub zzAddCalcFields(bGraph As Boolean)
			Dim oTable As Data.DataTable
			If bGraph Then
				oTable = moExproPgonsTable
			Else
				oTable = moDBExproTable
			End If

			' moMainTable.Columns.Add("Deviation", GetType(System.Double))
			With oTable.Columns

				.Add("BlockNameView", GetType(System.String))
				.Add("ParcelNoView", GetType(System.Int32))

				.Add("TopoID", GetType(System.Int32))
				.Add(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName, GetType(System.String))
				.Add(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName, GetType(System.Int32))
				.Add(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName, GetType(System.Double))

				.Add(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName, GetType(System.Double))
				.Add(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName, GetType(System.Double))
				.Add(msHasDeviationFieldName, GetType(System.Boolean))
				.Add(msHasOutPgonsDeviationFieldName, GetType(System.Boolean))


			End With



		End Sub
		Private Sub zzCalcDBTable()
			Dim iExproType As Integer
			Dim dAcadArea As Double
			Dim dLegalArea As Double
			Dim tParcelArea As TopoManager.TPlanGraph.ParcelArea
			Dim tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType
			Dim bHasDeviation As Boolean
			Dim bHasOutPgonsDeviation As Boolean
			For Each oRow As Data.DataRow In moDBExproTable.Rows
				With oRow

					iExproType = DMCommon.Functions.CIntN(.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))
					tExproType = TopoManager.TPlanGraph.TplnExpro.GetExproType(iExproType)
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeNameFieldName) = tExproType.Name
					.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName) = tExproType.OrderBy



					dLegalArea = DMCommon.Functions.CDblN(.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
					dAcadArea = DMCommon.Functions.CDblN(.Item(TopoManager.TopoReader.msAcadAreaFldName))
					tParcelArea = New TopoManager.TPlanGraph.ParcelArea(dAcadArea)
					tParcelArea.CalculateArea(dLegalArea, 3)

					.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = tParcelArea.ToleranceM
					.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = tParcelArea.DeltaAreaM
					.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = tParcelArea.DeviationM
					If iExproType = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then
						bHasDeviation = (tParcelArea.DeviationM > 0.0)
						'bHasOutPgonsDeviation=
					End If

					.Item(msHasDeviationFieldName) = bHasDeviation
					.Item(msHasOutPgonsDeviationFieldName) = bHasOutPgonsDeviation

				End With
			Next
		End Sub
		Private Sub zzSetLayout()
			Dim oGridRow As DataGridViewRow = Nothing
			'	Dim oPrevGridRow As DataGridViewRow = Nothing

			'	.Add("BlockNameView", GetType(System.String))
			'	.Add("ParcelNoView", GetType(System.Int32))


			'.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName) = oParcel.BlockNo
			'.Item(TopoManager.TPlanGraph.TplnParcel.msBlockAddFieldName) = oParcel.BlockAdd
			'.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName) = oParcel.ParcelNo



			Dim iPgonTypeID As Integer
			Dim iRowIndex As Integer
			Dim iBlockNo As Integer
			Dim iBlockAddNo As Integer
			Dim bEventsEnabled As Boolean = mbEventsEnabled
			Dim sBlockName As String
			Dim bFillBlockList As Boolean = (Me.cmbBlockNames.Items.Count = 0)
			Dim iParcelIndex As Integer = 0
			Dim dDeviation As Double
			mbEventsEnabled = False
			'	Me.cmbBlockNames.Items.Clear()
			If bFillBlockList Then
				Me.cmbBlockNames.Items.Add("--All--")
			End If
			ReDim miaParcelsSorted(moMainView.Count - 1)
			'	Dim bHasDeviation As Boolean
			For Each oRow As Data.DataRowView In moMainView
				iPgonTypeID = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))


				If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then

					iBlockNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName))
					iBlockAddNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName))

					sBlockName = TopoManager.TPlanGraph.TplnBlock.GetBlockName(iBlockNo, iBlockAddNo)
					oRow.Item("BlockNameView") = sBlockName
					oRow.Item("ParcelNoView") = oRow.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName)

					oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
					oGridRow.DefaultCellStyle = moDataGridViewBoldCellStyle
					If bFillBlockList AndAlso Not Me.cmbBlockNames.Items.Contains(sBlockName) Then
						Me.cmbBlockNames.Items.Add(sBlockName)
					End If
					miaParcelsSorted(iParcelIndex) = New ParcelCalcType(DMCommon.Functions.CIntN(oRow.Item("TopoID")), DMCommon.Functions.CBoolN(oRow.Item("ExproCalcType")))
					iParcelIndex += 1


					If iRowIndex > 0 Then
						'	oGridRow.DividerHeight = miAfterActionDividerHeight
					End If
				ElseIf iPgonTypeID > 20 Then
					dDeviation = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName))
					oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
					Dim oGridColumn As DataGridViewColumn = dgvMain.Columns.Item("cchExproCalcType")
					oGridColumn.DefaultCellStyle = moDataGridViewCheckCellStyle
					If dDeviation > 0.0 Then
						oGridRow.DefaultCellStyle = moDataGridViewRedCellStyle  ' moDataGridViewRedCellStyle 
					Else
						oGridRow.DefaultCellStyle = moDataGridViewCellStyle  ' moDataGridViewRedCellStyle 
					End If
				End If

				If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID AndAlso iRowIndex > 0 Then
					oGridRow = Me.dgvMain.Rows.Item(iRowIndex - 1)
					oGridRow.DividerHeight = miAfterActionDividerHeight
				End If
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "?SORT", iPgonTypeID, iRowIndex)
				'oPrevGridRow = Me.dgvMain.Rows.Item(iRowIndex)
				iRowIndex += 1
			Next
			If bFillBlockList Then
				Me.cmbBlockNames.SelectedIndex = 0
			End If

			mbEventsEnabled = bEventsEnabled
		End Sub

		Private Function zzGetKeys(oRow As System.Data.DataRow) As System.Object()
			Return New Object() {miProjectCode, miDetailNo, oRow.Item("Version"), oRow.Item("BlockNo"), oRow.Item("BlockAddNo"), oRow.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName), TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID}

		End Function
		Private Sub zzLoadDBExpro()
			Const sSPName As String = "GetExpro"
			moExproDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sSPName, System.Data.CommandType.StoredProcedure, zzGetProjectParameters(miCurrentVersion), True, "", True, True)
			If (miCurrentStatus And DataStatus.DB) = DataStatus.DB Then

				If moDBExproTable IsNot Nothing Then
					moDBExproTable.Dispose()
					moDBExproTable = Nothing
				End If

				moDBExproTable = New System.Data.DataTable("DBExpro")

				moExproDataAdapter.Fill(moDBExproTable)
				Me.cmbBlockNames.Items.Clear()
				Dim oPrimaryKey() As System.Data.DataColumn = {moDBExproTable.Columns.Item("ProjectCode"), moDBExproTable.Columns.Item("DetailNo"), moDBExproTable.Columns.Item("Version"), moDBExproTable.Columns.Item("BlockNo"), moDBExproTable.Columns.Item("BlockAddNo"), moDBExproTable.Columns.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName), moDBExproTable.Columns.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName)}
				moDBExproTable.PrimaryKey = oPrimaryKey
			End If
		End Sub




		Private Sub frmExpro_Resize(oSender As System.Object, e As EventArgs) Handles Me.Resize
			If midgvMainLocationY <> 0 Then
				Try
					Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "frmExpro - Resize")
				End Try
			End If
		End Sub

		Private Function zzGetProjectParameters(Optional iVersion As Integer = 0) As System.Data.Common.DbParameter()
			Dim iParamUB As Integer
			If iVersion = 0 Then
				iParamUB = 1
			Else
				iParamUB = 2
			End If
			Dim oaParams(iParamUB) As System.Data.Common.DbParameter

			oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", System.Data.DbType.Int32, miProjectCode)
			oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetailNo", System.Data.DbType.Int32, miDetailNo)
			If iVersion <> 0 Then
				oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prVersion", System.Data.DbType.Int32, iVersion)
			End If
			Return oaParams
		End Function
		Private Function zzGetProjectParcelParameters(iBlockNo As Integer, iBlockAdd As Integer, iParcelNo As Integer) As System.Data.Common.DbParameter()
			Dim iParamUB As Integer = 4

			Dim oaParams(iParamUB) As System.Data.Common.DbParameter

			oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", System.Data.DbType.Int32, miProjectCode)
			oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetailNo", System.Data.DbType.Int32, miDetailNo)
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prBlockNo", System.Data.DbType.Int32, iBlockNo)
			oaParams(3) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prBlockAdd", System.Data.DbType.Int32, iBlockAdd)
			oaParams(4) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prParcelNo", System.Data.DbType.Int32, iParcelNo)

			Return oaParams
		End Function
		Private Sub cmbVersions_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbVersions.SelectedIndexChanged
			If mbEventsEnabled Then

				Dim iVer As Integer
				Dim oSelectedItem As System.Object = Me.cmbVersions.SelectedItem
				If oSelectedItem IsNot Nothing Then

					If oSelectedItem Is msNewVersion Then
						miCurrentStatus = DataStatus.Graph
						miCurrentVersion = miLastDBVersion + 1
						moDBExproTable.Clear()
						zzLoadGraphData()
					Else
						iVer = DirectCast(oSelectedItem, Integer)

						miCurrentVersion = iVer
						If miCurrentVersion = miLastDBVersion Then
							miCurrentStatus = DataStatus.Graph Or DataStatus.DB
						Else
							miCurrentStatus = DataStatus.DB
						End If
						zzLoadData()
						If miCurrentStatus = DataStatus.DB Then
							zzAddCalcFields(False)
							zzCalcDBTable()

							'	zzDB_X()
							zzSetMainView("")
							zzSetLayout()
							zzSortType()
						End If

					End If
					zzSetReadOnly()

				End If

				Return

			End If

		End Sub
		Private Function zzGetFilter() As String
			Dim iBlockNo As Integer
			Dim iBlockAddNo As Integer
			Dim sBlockName As String
			Dim sResFilter As String = Nothing

			If cmbBlockNames.SelectedIndex = 0 Then
				sResFilter = String.Empty
			Else
				sBlockName = DirectCast(cmbBlockNames.SelectedItem, String)
				If TopoManager.TPlanGraph.TplnBlock.GetBlockNo(sBlockName, iBlockNo, iBlockAddNo) Then
					sResFilter = "(" & TopoManager.TPlanGraph.TplnParcel.BlockFieldName & "=" & iBlockNo.ToString() & ")"
					If iBlockAddNo <> 0 Then
						sResFilter &= " AND (" & TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName & "=" & iBlockAddNo.ToString() & ")"
					End If
				End If
			End If
			If chkHasDeviation.Checked Then
				If Not String.IsNullOrEmpty(sResFilter) Then
					sResFilter &= " AND "
				End If
				sResFilter &= msHasDeviationFieldName
			End If
			If chkHasOutPgonsDeviation.Checked Then
				If Not String.IsNullOrEmpty(sResFilter) Then
					sResFilter &= " AND "
				End If
				sResFilter &= msHasOutPgonsDeviationFieldName
			End If
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!sResFilter", sResFilter)
			Return sResFilter
		End Function
		Private Function zzGetParcelFilter() As String

			'Dim sBlockName As String
			Dim sResFilter As String = Nothing


			sResFilter = "(" & TopoManager.TPlanGraph.TplnParcel.BlockFieldName & "=" & miBlockNo.ToString() & ")"
			If miBlockAdd <> 0 Then
				sResFilter &= " AND (" & TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName & "=" & miBlockAdd.ToString() & ")"
			End If
			If miParcelNo <> 0 Then
				sResFilter &= " AND (" & TopoManager.TPlanGraph.TplnParcel.NameFieldName & "=" & miParcelNo.ToString() & ")"
			End If


			DMCommon.Debug.ExcelLog.SetNextValue(0, "!sResPParcelFilter", sResFilter)
			Return sResFilter
		End Function
		Private Sub zzClearExpro()

			'	Const sSPName As String = "ClearBlockData"
			Const sSPName As String = "ClearExpro"


			Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, Data.CommandType.StoredProcedure, zzGetProjectParameters(miCurrentVersion))
			'   DMCommon.Debug.MsgBox("zzClearGushData", iRes)
		End Sub
		Private Sub zzClearParcelExpro()
			'	Const sSPName As String = "ClearBlockData"
			Const sSPName As String = "ClearParcelExpro"

			Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, Data.CommandType.StoredProcedure, zzGetProjectParcelParameters(miBlockNo, miBlockAdd, miParcelNo))
			'   DMCommon.Debug.MsgBox("zzClearGushData", iRes)
		End Sub
		Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
			Me.Cursor = Cursors.WaitCursor
			zzClearExpro()
			moExproDataAdapter.Update(moExproPgonsTable)
			DMCommon.Debug.MsgBox("moExproPgonsTable", moExproPgonsTable.Rows.Count)
			zzLoadVersions()
			Me.Cursor = Cursors.Default

		End Sub


		Private Sub cmdExit_Click(oSender As System.Object, e As EventArgs) Handles cmdExit.Click
			Me.Close()
		End Sub

		Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit
			Dim oDataRow As Data.DataRowView = Nothing
			Dim oColumn As DataGridViewColumn = Nothing

			Dim iPgonTypeID As Integer
			If e.RowIndex >= 0 Then
				oDataRow = moMainView.Item(e.RowIndex)
				oColumn = Me.dgvMain.Columns.Item(e.ColumnIndex)
				iPgonTypeID = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))
				Select Case oColumn.Name
					Case "cchExproCalcType"
						If iPgonTypeID <> TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then
							e.Cancel = True
						End If
					Case "ctxLegalArea"
						If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then
							e.Cancel = True
						End If

				End Select

			End If
		End Sub
		Private Sub zzRecalcParcel(oParcel As TopoManager.TPlanGraph.TplnParcel, bCond As Boolean, iParcelRowIndex As Integer)
			Dim taExproParcelArea() As TopoManager.TPlanGraph.ParcelArea = oParcel.ExproParcelArea
			Dim oRowView As Data.DataRowView

			If taExproParcelArea IsNot Nothing Then
				Dim oaRows(taExproParcelArea.GetUpperBound(0)) As Data.DataRow
				For iIndex As Integer = 0 To taExproParcelArea.GetUpperBound(0)
					iParcelRowIndex += 1
					oRowView = moMainView.Item(iParcelRowIndex)
					oaRows(iIndex) = oRowView.Row
				Next
				For iIndex As Integer = 0 To taExproParcelArea.GetUpperBound(0)
					'iParcelRowIndex += 1
					If oParcel.TopoID = 9544 Then
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!ExproParcelArea:", taExproParcelArea(iIndex).AcadArea, taExproParcelArea(iIndex).AcadAreaD, taExproParcelArea(iIndex).ExproType, taExproParcelArea(iIndex).LegalArea, taExproParcelArea(iIndex).ConditionalArea, taExproParcelArea(iIndex).IsForced)
					End If

					taExproParcelArea(iIndex).CalculateArea(3, bCond)
					'oRowView = moMainView.Item(iParcelRowIndex)
					'oRow = oRowView.Row
					zzParcelAreaToRowNew(taExproParcelArea(iIndex), bCond, oaRows(iIndex))
				Next

			End If

		End Sub
		Private Sub dgvMain_CellEndEdit(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellEndEdit
			Select Case e.ColumnIndex
				Case 4
					zzSetCond(e.RowIndex, CheckState.Indeterminate)
					zzSetLayout()
			End Select

			Dim oDataRow As Data.DataRowView = Nothing
			Dim oColumn As DataGridViewColumn = Nothing

			Dim iPgonTypeID As Integer
			If e.RowIndex >= 0 Then
				oDataRow = moMainView.Item(e.RowIndex)
				oColumn = Me.dgvMain.Columns.Item(e.ColumnIndex)
				iPgonTypeID = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))
				Select Case oColumn.Name
					Case "cchExproCalcType"
						zzSetCond(e.RowIndex, CheckState.Indeterminate)
						zzSetLayout()
					Case "ctxLegalArea"
						miBlockNo = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName))
						miBlockAdd = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName))

						miParcelNo = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.NameFieldName))
						'	DMCommon.Debug.MsgBox("01_010", miBlockNo, miBlockAdd, miParcelNo)

				End Select

			End If
		End Sub


		Private Sub zzSetCond(iRowIndex As Integer, iCheckState As CheckState)
			Dim oDataRow As Data.DataRowView = Nothing
			Dim iPgonTypeID As Integer
			Dim iParcelTopoID As Integer
			Dim bCond As Boolean
			Dim bRecalc As Boolean

			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
			If iRowIndex >= 0 Then
				oDataRow = moMainView.Item(iRowIndex)
				iPgonTypeID = DMCommon.Functions.CIntN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))



				If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then
					iParcelTopoID = DMCommon.Functions.CIntN(oDataRow.Item("TopoID"))
					If mdicParcels IsNot Nothing AndAlso mdicParcels.TryGetValue(iParcelTopoID, oParcel) Then
						bCond = DMCommon.Functions.CBoolN(oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName))
						If iCheckState = CheckState.Indeterminate Then
							bRecalc = True
						ElseIf iCheckState = CheckState.Unchecked Then
							If bCond Then
								oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = False
								bCond = False
								bRecalc = True
							End If
						ElseIf iCheckState = CheckState.Checked Then
							If Not bCond Then
								oDataRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproCalcTypeFieldName) = True
								bCond = True
								bRecalc = True
							End If
						End If
						'oParcel.CalcArea()
						If bRecalc Then
							moMainView.Table.BeginLoadData()
							zzRecalcParcel(oParcel, bCond, iRowIndex)
							moMainView.Table.EndLoadData()
						End If

						''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''TEMP 	oParcel.RecalcExproLot(bCond)

					End If
				End If

			End If
		End Sub
		Private Sub zzSetExproNewReportA()
			'Const sSheetName As String = "הפקעה"
			Const sSheetName As String = "הכרזה"

			Const sUnit As String = " מ""ר"
			Const sProjectNo As String = "מס' פרויקט: "
			Const sDatamap As String = "דטהמפ מערכות מידע גיאוגרפיות בע""מ"
			Const sTotal As String = "סה""כ שטח:"
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()

			Dim iPgonTypeID As Integer
			Dim sBlockName As String
			Dim iBlockNo As Integer
			Dim iBlockAddNo As Integer

			Dim iParcelNo As Integer

			Dim dParcelLegalArea As Double
			Dim dTypeArea As Double
			'	Dim iConstColumns As Integer = 3

			Dim iTypeOrder As Integer
			Dim iTypeIndex As Integer
			Dim iTypeID As Integer
			Dim iColumnUB As Integer = enOutputNColumns.UB
			Dim tType As TopoManager.TPlanGraph.TplnExpro.ExproType = Nothing
			Dim oaCaptions(iColumnUB) As System.Object
			Dim oaTotals(iColumnUB) As System.Object

			Dim oaOutputRow(iColumnUB) As System.Object
			Dim daColumnWidth(iColumnUB) As Double
			Dim oaTotalFormulas(3) As System.Object
			'Dim sProjectName As String
			Dim iStartTableRow As Integer = 0
			Dim iEndTableRow As Integer = 0
			'Dim iResourceID As Integer
			Dim iOutputRow As Integer = 0
			Dim iNo As Integer
			oExcelAppExt.Open()
			oExcelAppExt.SetSheetName(sSheetName)

			zzSetTitlePart(oExcelAppExt)

			'	oExcelAppExt.SetHeader("&[Date]", sSheetName, sDatamap)


			daColumnWidth(0) = 10.0
			daColumnWidth(1) = 10.0

			For iIndex As Integer = 2 To daColumnWidth.GetUpperBound(0) - 1
				daColumnWidth(iIndex) = 12.5
			Next
			daColumnWidth(daColumnWidth.GetUpperBound(0)) = 15.0
			oExcelAppExt.SetFormatColumns(0, daColumnWidth)

			Dim oaHeader(8) As System.Object


			For iCol As Integer = 0 To oaHeader.GetUpperBound(0)
				oaHeader(iCol) = zzGetText(iCol, 5)
			Next
			oExcelAppExt.SetValueInHeaderRow(0, 0, False, oaHeader)


			iStartTableRow = iOutputRow + 1
			'DMCommon.Debug.MsgBox("13_310n", iStartTableRow)
			'oExcelAppExt.SetDataTable(0, "MainView", moMainView)
			For Each oRow As Data.DataRowView In moMainView
				iPgonTypeID = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))



				If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then

					If oaOutputRow(0) IsNot Nothing Then
						oExcelAppExt.SetValueInRow(iOutputRow, 0, oaOutputRow)
						zzClear(oaOutputRow)
					End If
					iNo += 1
					iOutputRow += 1
					sBlockName = DMCommon.Functions.CStrN(oRow.Item("BlockNameView"))
					iBlockNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName))
					iBlockAddNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName))

					iParcelNo = DMCommon.Functions.CIntN(oRow.Item("ParcelNoView"))
					dParcelLegalArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
					oaOutputRow(enOutputNColumns.RowIndex) = iNo
					oaOutputRow(enOutputNColumns.BlockName) = sBlockName

					oaOutputRow(enOutputNColumns.ParcelNo) = iParcelNo
					oaOutputRow(enOutputNColumns.ParcelArea) = dParcelLegalArea



				ElseIf iPgonTypeID < 20 Then
					iTypeOrder = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName))
					If mdicGlobalExproTypes.TryGetValue(iTypeOrder, tType) Then
						iTypeIndex = tType.Index
						iTypeID = tType.ID
						If Me.chkAcadArea.Checked Then
							dTypeArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TopoReader.msAcadAreaFldName))
						Else
							dTypeArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
						End If
						Select Case iTypeID
							Case 1
								oaOutputRow(enOutputNColumns.ExproType1) = dTypeArea
							Case 4
								oaOutputRow(enOutputNColumns.ExproType4) = dTypeArea
							Case 5
								oaOutputRow(enOutputNColumns.ExproType5) = dTypeArea
						End Select

						'oaOutputRow(enOutputNColumns.ExproTypeBase + iTypeIndex) = dTypeArea
					End If
				End If


				'oaOutputRow(enOutputNColumns.ExproTypeBase + iTypeIndex) = zzGetMuniName(iBlockNo, iBlockAddNo, iParcelNo)

				oaOutputRow(enOutputNColumns.Locality) = zzGetMuniName(iBlockNo, iBlockAddNo, iParcelNo)
				'oPrevGridRow = Me.dgvMain.Rows.Item(iRowIndex)

			Next
			If oaOutputRow(0) IsNot Nothing Then
				oExcelAppExt.SetValueInRow(iOutputRow, 0, oaOutputRow)
				iEndTableRow = iOutputRow
			End If
			'	Dim oTableColumnRectangle As Rectangle
			Dim sRectAddress As String
			oaTotals(0) = sTotal
			oExcelAppExt.SetValueInHeaderRow(iEndTableRow + 1, 0, True, oaTotals)

			For iColumn As Integer = enOutputNColumns.ParcelArea To enOutputNColumns.ExproType5

				sRectAddress = oExcelAppExt.GetRectangleAddress(iStartTableRow, iColumn, iEndTableRow - iStartTableRow + 1, 1)
				oaTotalFormulas(iColumn - enOutputNColumns.ParcelArea) = "=SUM(" & sRectAddress & ")"
			Next

			oExcelAppExt.SetFormulaInRow(iEndTableRow + 1, enOutputNColumns.ParcelArea, False, oaTotalFormulas)
			Dim tRect As Rectangle = New Rectangle(enOutputNColumns.ParcelArea, iStartTableRow, 4, iEndTableRow - iStartTableRow + 1)
			oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)



			Dim oAreaRectangle As Rectangle = New Rectangle(2, 1, mdicGlobalExproTypes.Count, iOutputRow)
			'	oExcelAppExt.SetFormat(oAreaRectangle, 0, TriState.True)
			oExcelAppExt.SetBorders(iStartTableRow, 0, iEndTableRow - iStartTableRow + 1, enOutputNColumns.UB + 1)

		End Sub
		Private Sub cmdExcelReport_Click(oSender As System.Object, e As EventArgs) Handles cmdExcelReport.Click
			'Const sSheetName As String = "הפקעה"
			Const sSheetName As String = "הכרזה"

			Const sUnit As String = " מ""ר"
			Const sProjectNo As String = "מס' פרויקט: "
			Const sDatamap As String = "דטהמפ מערכות מידע גיאוגרפיות בע""מ"
			Const sTotal As String = "סה""כ שטח:"
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()

			Dim iPgonTypeID As Integer
			Dim sBlockName As String
			Dim iBlockNo As Integer
			Dim iBlockAddNo As Integer

			Dim iParcelNo As Integer

			Dim dParcelLegalArea As Double
			Dim dTypeArea As Double
			'	Dim iConstColumns As Integer = 3

			Dim iTypeOrder As Integer
			Dim iTypeIndex As Integer
			Dim tType As TopoManager.TPlanGraph.TplnExpro.ExproType = Nothing
			Dim oaCaptions(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) As System.Object
			Dim oaTotals(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) As System.Object

			Dim oaOutputRow(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) As System.Object
			Dim daColumnWidth(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) As Double
			Dim oaTotalFormulas(mdicGlobalExproTypes.Count - 1) As System.Object
			Dim sProjectName As String
			Dim iStartTableRow As Integer = 0
			Dim iEndTableRow As Integer = 0
			Dim iResourceID As Integer
			Dim iOutputRow As Integer = 0

			oExcelAppExt.Open()
			oExcelAppExt.SetSheetName(sSheetName)


			zzSetTitlePart(oExcelAppExt)

			'	oExcelAppExt.SetHeader("&[Date]", sSheetName, sDatamap)


			daColumnWidth(0) = 10.0
			daColumnWidth(1) = 10.0

			For iIndex As Integer = 2 To daColumnWidth.GetUpperBound(0)
				daColumnWidth(iIndex) = 12.5
			Next
			oExcelAppExt.SetFormatColumns(0, daColumnWidth)


			For iColIndex As Integer = 0 To enOutputColumns.ExproTypeBase - 1
				iResourceID = iColIndex
				oaCaptions(iColIndex) = zzGetText(iResourceID, 1)
			Next

			For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
				oaCaptions(enOutputColumns.ExproTypeBase + tExproType.Index) = tExproType.Name & sUnit
			Next

			oaCaptions(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) = zzGetText(iResourceID + 1, 1)

			oExcelAppExt.SetValueInHeaderRow(iOutputRow, 0, True, oaCaptions)






			iStartTableRow = iOutputRow + 1
			'DMCommon.Debug.MsgBox("13_310n", iStartTableRow)
			'oExcelAppExt.SetDataTable(0, "MainView", moMainView)
			For Each oRow As Data.DataRowView In moMainView
				iPgonTypeID = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeIDFieldName))



				If iPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then

					If oaOutputRow(0) IsNot Nothing Then
						oExcelAppExt.SetValueInRow(iOutputRow, 0, oaOutputRow)
						zzClear(oaOutputRow)
					End If

					iOutputRow += 1
					sBlockName = DMCommon.Functions.CStrN(oRow.Item("BlockNameView"))
					iBlockNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockFieldName))
					iBlockAddNo = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.BlockAddFieldName))

					iParcelNo = DMCommon.Functions.CIntN(oRow.Item("ParcelNoView"))
					dParcelLegalArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
					oaOutputRow(enOutputColumns.BlockName) = sBlockName
					oaOutputRow(enOutputColumns.ParcelNo) = iParcelNo
					oaOutputRow(enOutputColumns.ParcelArea) = dParcelLegalArea



				ElseIf iPgonTypeID < 20 Then
					iTypeOrder = DMCommon.Functions.CIntN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName))
					If mdicGlobalExproTypes.TryGetValue(iTypeOrder, tType) Then
						iTypeIndex = tType.Index
						If Me.chkAcadArea.Checked Then
							dTypeArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TopoReader.msAcadAreaFldName))
						Else
							dTypeArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
						End If


						oaOutputRow(enOutputColumns.ExproTypeBase + iTypeIndex) = dTypeArea
					End If
				End If

				'oaOutputRow(enOutputColumns.ExproTypeBase + iTypeIndex) = zzGetMuniName(iBlockNo, iBlockAddNo, iParcelNo)

				oaOutputRow(enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count) = zzGetMuniName(iBlockNo, iBlockAddNo, iParcelNo)
				'oPrevGridRow = Me.dgvMain.Rows.Item(iRowIndex)

			Next
			If oaOutputRow(0) IsNot Nothing Then
				oExcelAppExt.SetValueInRow(iOutputRow, 0, oaOutputRow)
				iEndTableRow = iOutputRow
			End If
			'	Dim oTableColumnRectangle As Rectangle
			Dim sRectAddress As String
			oaTotals(0) = sTotal
			oExcelAppExt.SetValueInHeaderRow(iEndTableRow + 1, 0, True, oaTotals)

			For iColumn As Integer = enOutputColumns.ExproTypeBase To enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count - 1
				'	oTableColumnRectangle = New Rectangle(iColumn, iStartTableRow, 1, iEndTableRow - iStartTableRow + 1)
				sRectAddress = oExcelAppExt.GetRectangleAddress(iStartTableRow, iColumn, iEndTableRow - iStartTableRow + 1, 1)
				oaTotalFormulas(iColumn - enOutputColumns.ExproTypeBase) = "=SUM(" & sRectAddress & ")"
			Next

			oExcelAppExt.SetFormulaInRow(iEndTableRow + 1, enOutputColumns.ExproTypeBase, False, oaTotalFormulas)

			If True Then
				Dim oAreaRectangle As Rectangle = New Rectangle(2, 1, mdicGlobalExproTypes.Count, iOutputRow)
				'	oExcelAppExt.SetFormat(oAreaRectangle, 0, TriState.True)
				oExcelAppExt.SetBorders(iStartTableRow, 0, iEndTableRow - iStartTableRow + 1, enOutputColumns.ExproTypeBase + mdicGlobalExproTypes.Count + 1)
			End If


		End Sub
		Private Sub zzSetTitlePart(ByRef oExcelAppExt As DMCommon.ExcelAppExt)
			Const sDatamap As String = "דטהמפ מערכות מידע גיאוגרפיות בע""מ"
			Const sProjectNo As String = "מס' פרויקט: "

			Dim iOutputRow As Integer
			Dim sProjectName As String

			oExcelAppExt.SetHeader("&[Date]", "&A", sDatamap)

			oExcelAppExt.SetFooter("&[Path] &[File]", "", "&[Page] of &[Pages]")
			'	oExcelAppExt.PrintGridlines(True)
			iOutputRow = 1
			If miProjectCode <> 0 Then
				sProjectName = TPlServerDB.ServerDB.CurrentServerDB.GetProjectName(miProjectCode)
				oExcelAppExt.SetValueInCol(iOutputRow, 0, sProjectName)
				iOutputRow += 1
				oExcelAppExt.SetValueInCol(iOutputRow, 0, sProjectNo & miProjectCode.ToString)
				iOutputRow += 2

			End If
		End Sub

		Private Sub zzSetTitlePartI(ByRef oExcelAppExt As DMCommon.ExcelAppExt)
			Const sDatamap As String = "דטהמפ מערכות מידע גיאוגרפיות בע""מ"
			Const sProjectNo As String = "מס' פרויקט: "


			Dim sProjectName As String
			Dim tCaptionRect As Rectangle = New Rectangle(0, 0, 32, 3)
			Dim sCaptionValue As String
			Dim tRect As Rectangle
			oExcelAppExt.SetHeader("&[Date]", "&A", sDatamap)

			oExcelAppExt.SetFooter("&[Path] &[File]", "", "&[Page] of &[Pages]")
			'	oExcelAppExt.PrintGridlines(True)


			If miProjectCode <> 0 Then
				sProjectName = TPlServerDB.ServerDB.CurrentServerDB.GetProjectName(miProjectCode)


				'	oExcelAppExt.SetValueInCol(iOutputRow, 0, sProjectNo & miProjectCode.ToString)

				sCaptionValue = sProjectName & vbCrLf & sProjectNo & miProjectCode.ToString() & vbCrLf
				'	oExcelAppExt.SetValueInRowByCol(0, 3, 0, sCaptionValue)
				tRect = New Rectangle(0, 0, 7 + mdicGlobalExproTypes.Count * 5, 3)
				'27/01/26  oExcelAppExt.Merge(tRect, False, sCaptionValue)
				oExcelAppExt.Merge(tRect, False)

			End If
		End Sub
		Private Sub zzSetTitlePartII(ByRef oExcelAppExt As DMCommon.ExcelAppExt)
			Const sDatamap As String = "דטהמפ מערכות מידע גיאוגרפיות בע""מ"
			Const sProjectNo As String = "מס' פרויקט: "


			Dim sProjectName As String
			Dim tCaptionRect As Rectangle = New Rectangle(0, 0, 32, 3)
			Dim sCaptionValue As String
			Dim tRect As Rectangle
			oExcelAppExt.SetHeader("&[Date]", "&A", sDatamap)

			oExcelAppExt.SetFooter("&[Path] &[File]", "", "&[Page] of &[Pages]")
			'	oExcelAppExt.PrintGridlines(True)


			If miProjectCode <> 0 Then
				sProjectName = TPlServerDB.ServerDB.CurrentServerDB.GetProjectName(miProjectCode)


				'	oExcelAppExt.SetValueInCol(iOutputRow, 0, sProjectNo & miProjectCode.ToString)

				sCaptionValue = sProjectName & vbCrLf & sProjectNo & miProjectCode.ToString() & vbCrLf
				'	oExcelAppExt.SetValueInRowByCol(0, 3, 0, sCaptionValue)
				tRect = New Rectangle(0, 0, 12 + mdicGlobalExproTypes.Count, 3)
				oExcelAppExt.Merge(tRect, False, sCaptionValue)
			End If
		End Sub

		Private Sub zzSetExproLuseReport() ' AcadReport.BaseReport
			Dim iRow As Integer = 0
			Dim iRowCaption As Integer = 0
			Dim tRect As Rectangle
			Dim iColIndex As Integer
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim oaSubHeader(4) As System.Object
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
			For iCol As Integer = 0 To oaSubHeader.GetUpperBound(0)
				oaSubHeader(iCol) = zzGetText(4 + iCol, 2)
			Next


			oExcelAppExt.Open()
			oExcelAppExt.Activate()

			'zzSetTitlePart(oExcelAppExt)
			zzSetTitlePartI(oExcelAppExt)


			'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
			'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
			iRowCaption = 3
			iRow = 4
			For iCol As Integer = 0 To 2
				tRect = New Rectangle(iCol, iRow, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(iCol, 2))
			Next

			tRect = New Rectangle(3, iRow, 4, 0)
			oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(3, 2))
			'	zzSubgroupHeader(3, oExcelAppExt)
			oExcelAppExt.SetValueInHeaderRow(iRow + 1, 3, True, oaSubHeader)
			Dim iTypeIndex As Integer = 1

			'For Each tTypeID_Name As KeyValuePair(Of Integer, String) In TplnExpro.TypeNames

			'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)



			'	Next

			For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values

				tRect = New Rectangle(3 + 5 * iTypeIndex, iRow, 4, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, False, tExproType.Name)
				oExcelAppExt.SetValueInHeaderRow(iRow + 1, 3 + 5 * iTypeIndex, True, oaSubHeader)
				iTypeIndex += 1
			Next


			'Next
			iRow += 2
			'	Dim i As Integer = 0
			'	DMCommon.Debug.MsgBox("13_314", DMCommon.Debug.ColCount(mdicParcels))

			DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicParcels:", DMCommon.Debug.ColCount(mdicParcels))




			If mdicParcels IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted))
				For iParcelIndex As Integer = 0 To miaParcelsSorted.GetUpperBound(0)
					If mdicParcels.TryGetValue(miaParcelsSorted(iParcelIndex).TopoID, oParcel) Then
						'Debug	oExcelAppExt.SetValueInHeaderRow(iRow, 20, False, miaParcelsSorted(iParcelIndex), oParcel.ParcelNo, oParcel.HasLanduses, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count)
						oParcel.RecalcExproLot(miaParcelsSorted(iParcelIndex).CalcType)
						'		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelB:", oParcel.HasLanduses, oParcel.ExproTypeCount)
						If oParcel.HasLanduses AndAlso oParcel.HasExpro Then
							oParcel.ExproLuseReport(oExcelAppExt, iRow, mdicGlobalExproTypes)
							tRect = New System.Drawing.Rectangle(0, iRow - 1, 7 + mdicGlobalExproTypes.Count * 5, 0)

							oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
						End If



					End If

				Next
				tRect = New Rectangle(0, iRowCaption + 2, 2, iRow - 5)
				oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

				tRect = New Rectangle(0, iRowCaption + 2, 2, iRow - 5)
				oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
				tRect = New Rectangle(3, iRowCaption + 2, 4, iRow - 5)
				oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
				iColIndex = 8
				For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
					tRect = New Rectangle(iColIndex, iRowCaption + 2, 4, iRow - 5)
					oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(iColIndex, iRowCaption + 2, 0, iRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
					tRect = New Rectangle(iColIndex + 3, iRowCaption + 2, 0, iRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

					iColIndex += 5
				Next
				tRect = New Rectangle(2, iRowCaption + 2, 1, iRow - 5)
				oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

				tRect = New Rectangle(6, iRowCaption + 2, 0, iRow - 5)
				oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

			End If

		End Sub



		Private Sub zzSetExproLuseReport_0321() ' AcadReport.BaseReport
			Dim iParcelRow As Integer = 0
			Dim iParcelExtRow As Integer = 0

			Dim iRowCaption As Integer = 0
			Dim tRect As Rectangle
			Dim saValues() As String
			Dim iColIndex As Integer
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim oaSubHeader(4) As System.Object
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
			'oExcelAppExt.SetValueInHeaderRow(iRow + 1, iColIndex, True, tExproType.Name)
			Dim sSqM As String = zzGetText(15, 3)
			Dim sPct As String = zzGetText(16, 3)
			Dim sSqM_M As String = zzGetText(17, 3)
			Dim iExproTypesCount As Integer = mdicGlobalExproTypes.Count
			Dim dicParcelExts As Dictionary(Of Integer, frmEntConnected.ParcelExt) = frmEntConnected.ParcelExts

			For iCol As Integer = 0 To oaSubHeader.GetUpperBound(0)
				oaSubHeader(iCol) = zzGetText(4 + iCol, 2)
			Next


			oExcelAppExt.Open()
			oExcelAppExt.Activate()
			'oExcelAppExt.OpenTextBox()
			'zzSetTitlePart(oExcelAppExt)
			'Dim oRange As Microsoft.Office.Interop.Excel.Range
			'	oRange = oExcelAppExt.GetRange(12, 12)
			If True Then 'Header

				zzSetTitlePartII(oExcelAppExt)

				'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				iRowCaption = 3
				iParcelRow = 5
				For iCol As Integer = 0 To 2
					tRect = New Rectangle(iCol, iParcelRow, 0, 2)
					oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(iCol, 2))
				Next


				tRect = New Rectangle(3, iParcelRow, 5 + iExproTypesCount, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(3, 3))
				iColIndex = 3
				'Debug.MsgBox("060521_1", mdicGlobalExproTypes.Count)
				For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
					'oExcelAppExt.SetValueInHeaderCell(tRect, False, tExproType.Name)
					oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, tExproType.Name)
					oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, sSqM)

					iColIndex += 1
				Next

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(6, 3))

				iColIndex += 1

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)

				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(7, 3))

				saValues = zzGetTextArray(8, 4, 3)
				iColIndex += 1

				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, saValues)
				saValues = {sSqM, sPct, sSqM, sSqM}
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, saValues)
				iColIndex += 4

				tRect = New Rectangle(iColIndex, iParcelRow, 0, 2)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(4, 3))

				iColIndex += 1

				tRect = New Rectangle(iColIndex, iParcelRow, 2, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(5, 3))

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(12, 3))

				iColIndex += 1
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, zzGetText(13, 3))
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, sSqM_M)

				iColIndex += 1
				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(14, 3))
				'DMCommon.Debug.MsgBox("310321_1", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(mdicExproTypes))
				iParcelRow += 3
			End If

			'''''''''''''''''''''Body

			'Dim iFirstRow As Integer
			Const sEntConnectedIsNothing As String = "אין מחוברים בשטח חלקה זו"
			Dim iParcelID As Integer
			Dim oParcelExt As frmEntConnected.ParcelExt = Nothing
			Dim bCalcArea As Boolean = Me.chkAcadArea.Checked
			If mdicParcels IsNot Nothing Then




				iParcelExtRow = iParcelRow
				'	DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted))
				'oExcelAppExt.SetValueInRow(10, 16, 999)
				For iParcelIndex As Integer = 0 To miaParcelsSorted.GetUpperBound(0)
					If iParcelIndex > 1000000 Then
						Exit For
					End If
					iParcelID = miaParcelsSorted(iParcelIndex).TopoID
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'Debug	oExcelAppExt.SetValueInHeaderRow(iRow, 20, False, miaParcelsSorted(iParcelIndex), oParcel.ParcelNo, oParcel.HasLanduses, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count)
						oParcel.RecalcExproLot(miaParcelsSorted(iParcelIndex).CalcType)
						'		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelA:", oParcel.HasLanduses, oParcel.HasExpro, oParcel.ExproTypeCount, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count, mdicGlobalExproTypes.Count)
						'DMCommon.Debug.MsgBox("13_318", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted), oParcel.HasLanduses, oParcel.HasExpro, iParcelRow)
						If oParcel.HasLanduses AndAlso oParcel.HasExpro Then
							'oParcel.ExproLuseReport_0321(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)
							oParcel.ExproLuseReport_0226(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)

						End If
						'DMCommon.Debug.MsgBox("13_319", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted), dicParcelExts IsNot Nothing)
						If dicParcelExts IsNot Nothing AndAlso dicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
							oParcelExt.ExproLuseReport_0321(oExcelAppExt, iParcelExtRow, 10 + mdicGlobalExproTypes.Count)
						Else
							oExcelAppExt.SetValueInRow(iParcelExtRow, 10 + mdicGlobalExproTypes.Count, sEntConnectedIsNothing)
						End If
						If iParcelRow > iParcelExtRow Then
							iParcelExtRow = iParcelRow
						ElseIf iParcelExtRow > iParcelRow Then
							iParcelRow = iParcelExtRow
						End If
						tRect = New System.Drawing.Rectangle(0, iParcelRow - 1, 12 + mdicGlobalExproTypes.Count, 0)
						oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
					End If
				Next
				If oExcelAppExt.TextBoxExists Then
					oExcelAppExt.TextBox.ToBox()
				End If

				If False Then



					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
					tRect = New Rectangle(3, iRowCaption + 2, 4, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(13 + mdicGlobalExproTypes.Count, iRowCaption + 5, 1, iParcelRow - 9)
					oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					iColIndex = 2
					For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
						tRect = New Rectangle(iColIndex, iRowCaption + 2, 4, iParcelRow - 5)
						''''''''''''''''''''''''''''''''''''''''oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

						'tRect = New Rectangle(iColIndex, iRowCaption + 2, 0, iParcelRow - 5)
						'oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
						'tRect = New Rectangle(iColIndex + 3, iRowCaption + 2, 0, iParcelRow - 5)
						'	oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)


					Next
					tRect = New Rectangle(iColIndex, iRowCaption + 2, mdicGlobalExproTypes.Count, iParcelRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

					tRect = New Rectangle(mdicGlobalExproTypes.Count + 5, iRowCaption + 2, 3, iParcelRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
				End If

			End If
		End Sub
		Private Sub zzSetExproNewReport()
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim iParcelRow As Integer = 0
			Dim iRowNumber As Integer = 0
			Dim iFirstRow As Integer = 0


			Dim iParcelExtRow As Integer = 0
			Dim iParcelID As Integer
			Dim oParcelExt As frmEntConnected.ParcelExt = Nothing
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
			Dim bCalcArea As Boolean = Me.chkAcadArea.Checked
			Dim dicParcelExts As Dictionary(Of Integer, frmEntConnected.ParcelExt) = frmEntConnected.ParcelExts
			Dim tRect As Rectangle
			oExcelAppExt.Open()
			oExcelAppExt.Activate()

			Dim oaHeader(8) As System.Object





			For iCol As Integer = 0 To oaHeader.GetUpperBound(0)
				oaHeader(iCol) = zzGetText(iCol, 5)
			Next
			oExcelAppExt.SetValueInHeaderRow(0, 0, False, oaHeader)

			iFirstRow = 2
			If mdicParcels IsNot Nothing Then
				iParcelExtRow = iParcelRow
				'	DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted))
				'oExcelAppExt.SetValueInRow(10, 16, 999)
				DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), miaParcelsSorted.GetUpperBound(0))

				For iParcelIndex As Integer = 0 To miaParcelsSorted.GetUpperBound(0)
					If iParcelIndex > 8000 Then
						Exit For
					End If
					iParcelID = miaParcelsSorted(iParcelIndex).TopoID
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'Debug	oExcelAppExt.SetValueInHeaderRow(iRow, 20, False, miaParcelsSorted(iParcelIndex), oParcel.ParcelNo, oParcel.HasLanduses, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count)
						oParcel.RecalcExproLot(miaParcelsSorted(iParcelIndex).CalcType)
						'		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelA:", oParcel.HasLanduses, oParcel.HasExpro, oParcel.ExproTypeCount, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count, mdicGlobalExproTypes.Count)
						'DMCommon.Debug.MsgBox("13_318", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted), oParcel.HasLanduses, oParcel.HasExpro, iParcelRow)
						If oParcel.HasExpro Then
							'oParcel.ExproLuseReport_0321(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)
							'	oParcel.ExproLuseReport_0226(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)

						End If

						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 1, oParcel.BlockNo)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 2, oParcel.BlockAdd)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 3, oParcel.ParcelNo)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 4, oParcel.HasExpro)
						oExcelAppExt.SetValueInRow(iFirstRow + iRowNumber, 5, oParcel.LegalArea(False))





						iRowNumber += 1

						If False Then


							If iParcelRow > iParcelExtRow Then
								iParcelExtRow = iParcelRow
							ElseIf iParcelExtRow > iParcelRow Then
								iParcelRow = iParcelExtRow
							End If
							tRect = New System.Drawing.Rectangle(0, iParcelRow - 1, 12 + mdicGlobalExproTypes.Count, 0)
							oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

						End If
					End If
				Next iParcelIndex
				If oExcelAppExt.TextBoxExists Then
					oExcelAppExt.TextBox.ToBox()
				End If



			End If
		End Sub

		Private Sub zzSetExproLuseReport_0226() ' AcadReport.BaseReport
			Dim iParcelRow As Integer = 0
			Dim iParcelExtRow As Integer = 0

			Dim iRowCaption As Integer = 0
			Dim tRect As Rectangle
			Dim saValues() As String
			Dim iColIndex As Integer
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim oaSubHeader(4) As System.Object
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing


			'oExcelAppExt.SetValueInHeaderRow(iRow + 1, iColIndex, True, tExproType.Name)
			Dim sSqM As String = zzGetText(15, 3)
			Dim sPct As String = zzGetText(16, 3)
			Dim sSqM_M As String = zzGetText(17, 3)
			Dim iExproTypesCount As Integer = mdicGlobalExproTypes.Count
			Dim dicParcelExts As Dictionary(Of Integer, frmEntConnected.ParcelExt) = frmEntConnected.ParcelExts

			For iCol As Integer = 0 To oaSubHeader.GetUpperBound(0)
				oaSubHeader(iCol) = zzGetText(4 + iCol, 2)
			Next
			Dim oPlanTopology As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TopoManager.TPlanGraph.TplnPlan.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)

			'System.Windows.Forms.MessageBox.Show("LoadPlans" & vbCrLf & CStr(oPlanTopology IsNot Nothing), "#2758K")
			If oPlanTopology IsNot Nothing Then

			End If


			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			'''''''''''



			'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString(), "TplnProject - SetInitView")

			If oPlanTopology IsNot Nothing Then
				oPlanTopology.Close()
			End If




			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

			oExcelAppExt.Open()
			oExcelAppExt.Activate()
			'oExcelAppExt.OpenTextBox()
			'zzSetTitlePart(oExcelAppExt)
			'Dim oRange As Microsoft.Office.Interop.Excel.Range
			'	oRange = oExcelAppExt.GetRange(12, 12)
			If True Then 'Header

				zzSetTitlePartII(oExcelAppExt)

				'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
				iRowCaption = 3
				iParcelRow = 5
				For iCol As Integer = 0 To 2
					tRect = New Rectangle(iCol, iParcelRow, 0, 2)
					oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(iCol, 2))
				Next


				tRect = New Rectangle(3, iParcelRow, 5 + iExproTypesCount, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(3, 3))
				iColIndex = 3
				'Debug.MsgBox("060521_1", mdicGlobalExproTypes.Count)
				For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
					'oExcelAppExt.SetValueInHeaderCell(tRect, False, tExproType.Name)
					oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, tExproType.Name)
					oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, sSqM)

					iColIndex += 1
				Next

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(6, 3))

				iColIndex += 1

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)

				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(7, 3))

				saValues = zzGetTextArray(8, 4, 3)
				iColIndex += 1

				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, saValues)
				saValues = {sSqM, sPct, sSqM, sSqM}
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, saValues)
				iColIndex += 4

				tRect = New Rectangle(iColIndex, iParcelRow, 0, 2)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(4, 3))

				iColIndex += 1

				tRect = New Rectangle(iColIndex, iParcelRow, 2, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(5, 3))

				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(12, 3))

				iColIndex += 1
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, zzGetText(13, 3))
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, sSqM_M)

				iColIndex += 1
				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(14, 3))
				'DMCommon.Debug.MsgBox("310321_1", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(mdicExproTypes))
				iParcelRow += 3
			End If

			'''''''''''''''''''''Body

			'Dim iFirstRow As Integer
			Const sEntConnectedIsNothing As String = "אין מחוברים בשטח חלקה זו"
			Dim iParcelID As Integer
			Dim oParcelExt As frmEntConnected.ParcelExt = Nothing
			Dim bCalcArea As Boolean = Me.chkAcadArea.Checked
			If mdicParcels IsNot Nothing Then
				iParcelExtRow = iParcelRow
				'	DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted))
				'oExcelAppExt.SetValueInRow(10, 16, 999)


				For iParcelIndex As Integer = 0 To miaParcelsSorted.GetUpperBound(0)
					If iParcelIndex > 8 Then
						Exit For
					End If
					iParcelID = miaParcelsSorted(iParcelIndex).TopoID
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'Debug	oExcelAppExt.SetValueInHeaderRow(iRow, 20, False, miaParcelsSorted(iParcelIndex), oParcel.ParcelNo, oParcel.HasLanduses, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count)
						oParcel.RecalcExproLot(miaParcelsSorted(iParcelIndex).CalcType)
						'		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelA:", oParcel.HasLanduses, oParcel.HasExpro, oParcel.ExproTypeCount, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count, mdicGlobalExproTypes.Count)
						'DMCommon.Debug.MsgBox("13_318", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted), oParcel.HasLanduses, oParcel.HasExpro, iParcelRow)
						If oParcel.HasLanduses AndAlso oParcel.HasExpro Then
							'oParcel.ExproLuseReport_0321(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)
							oParcel.ExproLuseReport_0226(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)

						End If
						'DMCommon.Debug.MsgBox("13_319", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted), dicParcelExts IsNot Nothing)
						If dicParcelExts IsNot Nothing AndAlso dicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
							oParcelExt.ExproLuseReport_0321(oExcelAppExt, iParcelExtRow, 10 + mdicGlobalExproTypes.Count)
						Else
							oExcelAppExt.SetValueInRow(iParcelExtRow, 10 + mdicGlobalExproTypes.Count, sEntConnectedIsNothing)
						End If
						If iParcelRow > iParcelExtRow Then
							iParcelExtRow = iParcelRow
						ElseIf iParcelExtRow > iParcelRow Then
							iParcelRow = iParcelExtRow
						End If
						tRect = New System.Drawing.Rectangle(0, iParcelRow - 1, 12 + mdicGlobalExproTypes.Count, 0)
						oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
					End If
				Next iParcelIndex
				If oExcelAppExt.TextBoxExists Then
					oExcelAppExt.TextBox.ToBox()
				End If

				If False Then



					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
					tRect = New Rectangle(3, iRowCaption + 2, 4, iParcelRow - 5)
					'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
					oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					tRect = New Rectangle(13 + mdicGlobalExproTypes.Count, iRowCaption + 5, 1, iParcelRow - 9)
					oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					iColIndex = 2
					For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
						tRect = New Rectangle(iColIndex, iRowCaption + 2, 4, iParcelRow - 5)
						''''''''''''''''''''''''''''''''''''''''oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

						'tRect = New Rectangle(iColIndex, iRowCaption + 2, 0, iParcelRow - 5)
						'oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
						'tRect = New Rectangle(iColIndex + 3, iRowCaption + 2, 0, iParcelRow - 5)
						'	oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)


					Next
					tRect = New Rectangle(iColIndex, iRowCaption + 2, mdicGlobalExproTypes.Count, iParcelRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

					tRect = New Rectangle(mdicGlobalExproTypes.Count + 5, iRowCaption + 2, 3, iParcelRow - 5)
					oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
				End If

			End If
		End Sub


		Private Sub zzSetExproLuseReport_0126() ' AcadReport.BaseReport
			Dim iParcelRow As Integer = 0
			Dim iParcelExtRow As Integer = 0

			Dim iRowCaption As Integer = 0
			Dim tRect As Rectangle
			Dim saValues() As String
			Dim iColIndex As Integer
			Dim oExcelAppExt As DMCommon.ExcelAppExt = New DMCommon.ExcelAppExt()
			Dim oaSubHeader(4) As System.Object
			Dim oParcel As TopoManager.TPlanGraph.TplnParcel = Nothing
			'oExcelAppExt.SetValueInHeaderRow(iRow + 1, iColIndex, True, tExproType.Name)
			Dim sSqM As String = zzGetText(15, 3)
			Dim sPct As String = zzGetText(16, 3)
			Dim sSqM_M As String = zzGetText(17, 3)
			Dim iExproTypesCount As Integer = mdicGlobalExproTypes.Count
			Dim dicParcelExts As Dictionary(Of Integer, frmEntConnected.ParcelExt) = frmEntConnected.ParcelExts

			For iCol As Integer = 0 To oaSubHeader.GetUpperBound(0)
				oaSubHeader(iCol) = zzGetText(4 + iCol, 2)
			Next


			oExcelAppExt.Open()
			oExcelAppExt.Activate()

			'zzSetTitlePart(oExcelAppExt)
			zzSetTitlePartII(oExcelAppExt)

			'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
			'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
			iRowCaption = 3
			iParcelRow = 5
			For iCol As Integer = 0 To 2
				tRect = New Rectangle(iCol, iParcelRow, 0, 2)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(iCol, 2))
			Next

			Dim iDEBUG As Integer = 0
			Dim oDebugVal As Object
			tRect = New Rectangle(3, iParcelRow, 5 + iExproTypesCount, 0)
			oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(3, 3))


			iColIndex = 3
			'	DMCommon.Debug.MsgBox("060521_1", mdicGlobalExproTypes.Values.Count, iParcelRow)

			For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
				'oExcelAppExt.SetValueInHeaderCell(tRect, False, tExproType.Name)
				oDebugVal = tExproType.Name
				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 0)
				'1 oExcelAppExt.SetValueInHeaderCell(iParcelRow + 1, iColIndex + iDEBUG, True, oDebugVal)
				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 0)


				tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 2)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, oDebugVal)


				'''''	oExcelAppExt.SetValueInRow(iParcelRow + 1, iColIndex, oDebugVal)


				tRect = New Rectangle(iColIndex, iParcelRow + 2, 0, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, True, sSqM)

				iColIndex += 1
			Next

			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(6, 3))

			iColIndex += 1
			'	DMCommon.Debug.MsgBox("130126_1a", iColIndex, iParcelRow + 1, 0, 1)
			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(7, 3))


			saValues = zzGetTextArray(8, 4, 3)
			'	DMCommon.Functions.DispArray(saValues, "GetTextArray", True)
			iColIndex += 1

			''''''''	oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, saValues)
			'''''''''''	saValues = {sSqM, sPct, sSqM, sSqM}
			'oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, saValues)
			'"P:\2023\230385\hafkaot\general\omdan\topol\boris\sample\sample-topo-area-length-point.dwg"


			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, saValues(0))
			iColIndex += 1

			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, saValues(1))
			iColIndex += 1

			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, saValues(2))
			iColIndex += 1

			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, saValues(3))
			iColIndex += 1


			'iColIndex += 4

			tRect = New Rectangle(iColIndex, iParcelRow, 0, 2)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(4, 3))

			iColIndex += 1

			tRect = New Rectangle(iColIndex, iParcelRow, 2, 0)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(5, 3))

			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(12, 3))

			iColIndex += 1

			If True Then
				saValues(0) = "aaaa"
				saValues(1) = "bbbb"
				saValues(2) = "cccc"
				saValues(3) = "dddd"

				oExcelAppExt.SetStringInHeaderRow(iParcelRow + 2, iColIndex, True, saValues)
			End If


			If False Then
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 1, iColIndex, True, zzGetText(13, 3))
				oExcelAppExt.SetValueInHeaderRow(iParcelRow + 2, iColIndex, True, sSqM_M)
			End If
			iColIndex += 1
			tRect = New Rectangle(iColIndex, iParcelRow + 1, 0, 1)
			oExcelAppExt.SetValueInHeaderCell(tRect, True, zzGetText(14, 3))
			DMCommon.Debug.MsgBox("310321_1", DMCommon.Debug.ColCount(mdicParcels))
			iParcelRow += 3


			'''''''''''''''''''''Body

			'Dim iFirstRow As Integer
			Const sEntConnectedIsNothing As String = "אין מחוברים בשטח חלקה זו"
			Dim iParcelID As Integer
			Dim oParcelExt As frmEntConnected.ParcelExt = Nothing
			Dim bCalcArea As Boolean = Me.chkAcadArea.Checked
			Dim iDebugIndex As Integer = 0

			If True AndAlso mdicParcels IsNot Nothing Then
				iParcelExtRow = iParcelRow
				'	DMCommon.Debug.MsgBox("13_317", DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(miaParcelsSorted))
				For iParcelIndex As Integer = 0 To miaParcelsSorted.GetUpperBound(0)
					If iDebugIndex > 0 Then
						Exit For
					End If
					iParcelID = miaParcelsSorted(iParcelIndex).TopoID
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'Debug	oExcelAppExt.SetValueInHeaderRow(iRow, 20, False, miaParcelsSorted(iParcelIndex), oParcel.ParcelNo, oParcel.HasLanduses, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count)
						oParcel.RecalcExproLot(miaParcelsSorted(iParcelIndex).CalcType)
						'		For Each oParcel As TopoManager.TPlanGraph.TplnParcel In mdicParcels.Values
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ParcelA:", oParcel.HasLanduses, oParcel.HasExpro, oParcel.ExproTypeCount, oParcel.GetLanduseDic(DMAcadExt.enTopoPurpose.Approved).Count, mdicGlobalExproTypes.Count)

						If oParcel.HasLanduses AndAlso oParcel.HasExpro Then
							oParcel.ExproLuseReport_0126(bCalcArea, oExcelAppExt, iParcelRow, mdicGlobalExproTypes)
						End If
						If dicParcelExts IsNot Nothing AndAlso dicParcelExts.TryGetValue(iParcelID, oParcelExt) Then
							'' _0126  oParcelExt.ExproLuseReport_0321(oExcelAppExt, iParcelExtRow, 10 + mdicGlobalExproTypes.Count)
						Else
							oExcelAppExt.SetValueInRow(iParcelExtRow, 10 + mdicGlobalExproTypes.Count, sEntConnectedIsNothing)
						End If
						If iParcelRow > iParcelExtRow Then
							iParcelExtRow = iParcelRow
						ElseIf iParcelExtRow > iParcelRow Then
							iParcelRow = iParcelExtRow
						End If
						tRect = New System.Drawing.Rectangle(0, iParcelRow - 1, 12 + mdicGlobalExproTypes.Count, 0)
						oExcelAppExt.SetBorders(tRect,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
					End If
					iDebugIndex += 1
				Next
				Exit Sub

				tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
				'oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

				tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
				'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)
				tRect = New Rectangle(3, iRowCaption + 2, 4, iParcelRow - 5)
				'oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

				tRect = New Rectangle(0, iRowCaption + 2, 2, iParcelRow - 5)
				oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

				tRect = New Rectangle(13 + mdicGlobalExproTypes.Count, iRowCaption + 5, 1, iParcelRow - 9)
				oExcelAppExt.SetBorders(tRect, DMCommon.ExcelAppExt.enBorderWeight.Thin)

				iColIndex = 2
				For Each tExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
					tRect = New Rectangle(iColIndex, iRowCaption + 2, 4, iParcelRow - 5)
					''''''''''''''''''''''''''''''''''''''''oExcelAppExt.SetBorders(tRect,,,, DMCommon.ExcelAppExt.enBorderWeight.Thin)

					'tRect = New Rectangle(iColIndex, iRowCaption + 2, 0, iParcelRow - 5)
					'oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
					'tRect = New Rectangle(iColIndex + 3, iRowCaption + 2, 0, iParcelRow - 5)
					'	oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)


				Next
				tRect = New Rectangle(iColIndex, iRowCaption + 2, mdicGlobalExproTypes.Count, iParcelRow - 5)
				oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)

				tRect = New Rectangle(mdicGlobalExproTypes.Count + 5, iRowCaption + 2, 3, iParcelRow - 5)
				oExcelAppExt.SetNumberFormat(tRect, 0, TriState.True)
			End If
		End Sub
		Public Sub zzSubgroupHeader(iFirstColumn As Integer, ByRef oExcelAppExt As DMCommon.ExcelAppExt)
			Dim tRect As Rectangle
			tRect = New Rectangle(iFirstColumn, 1, iFirstColumn + 4, 0)
			For iIndex As Integer = 0 To 4
				tRect = New Rectangle(iFirstColumn + iIndex, 1, 0, 0)
				oExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(iIndex, 2))
			Next

		End Sub
		Private Sub zzClear(ByRef oaOutputRow() As System.Object)
			For iIndex As Integer = 0 To oaOutputRow.GetUpperBound(0)
				oaOutputRow(iIndex) = Nothing
			Next
		End Sub
		Private Sub zzSortType()
			Dim iIndex As Integer = 0
			For Each oExproType As TopoManager.TPlanGraph.TplnExpro.ExproType In mdicGlobalExproTypes.Values
				oExproType.Index = iIndex
				iIndex += 1
			Next

		End Sub
		Private Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
			Return TPlServerDB.TextResource.GetText(iItemID, miResourceTheme, iSectionID, True)
		End Function
		Private Function zzGetTextArray(ByVal iItemID As Integer, ByVal iCount As Integer, ByVal iSectionID As Integer) As String()
			Return TPlServerDB.TextResource.GetTextArray(iItemID, iCount, miResourceTheme, iSectionID, True)
		End Function
		Private Function zzGetMuniName(ByVal iBlockNo As Integer, ByVal iBlockAddNo As Integer, iParcelNo As Integer) As String
			Const sPrefix_Muni99 As String = "מועצה מקומית "
			Const sPrefix_RegCouncil As String = "מועצה אזורית "

			Dim iLocalityCode As Integer
			Dim sLocalityName As String

			Dim iMuniStatus As Integer = -1
			Dim sRegCouncilName As String


			Dim sSPName As String = "GetParcelData"
			Dim sRes As String
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, System.Data.CommandType.StoredProcedure, zzGetParcelParameters(iBlockNo, iBlockAddNo, iParcelNo))
			'	Dim iVersion As Integer

			If oDataReader IsNot Nothing Then
				If oDataReader.HasRows Then
					If oDataReader.Read Then
						If Not oDataReader.IsDBNull(1) Then
							iLocalityCode = oDataReader.GetInt32(1)
							sLocalityName = oDataReader.GetString(2)


							If Not oDataReader.IsDBNull(3) Then
								iMuniStatus = oDataReader.GetInt32(3)
							End If
							Select Case iMuniStatus
								Case -1
									sRes = String.Empty
								Case 0
									sRes = sLocalityName
								Case 99
									sRes = sPrefix_Muni99 & sLocalityName
								Case Else
									sRegCouncilName = oDataReader.GetString(4)
									sRes = sPrefix_RegCouncil & sRegCouncilName
							End Select
						Else
							sRes = String.Empty
						End If



					Else
						sRes = String.Empty
					End If
				Else
					sRes = String.Empty

				End If
				oDataReader.Close()
				Return sRes
			Else
				Return String.Empty
			End If




		End Function
		Private Function zzGetParcelParameters(iBlockNo As Integer, iBlockAddNo As Integer, iParcelNo As Integer) As System.Data.Common.DbParameter()
			Dim iParamUB As Integer = 2

			Dim oaParams(iParamUB) As System.Data.Common.DbParameter

			oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prBlockNo", System.Data.DbType.Int32, iBlockNo)
			oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prBlockAdd", System.Data.DbType.Int32, iBlockAddNo)
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prParcelNo", System.Data.DbType.Int32, iParcelNo)

			Return oaParams
		End Function
		Private Sub cmbBlockNames_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbBlockNames.SelectedIndexChanged
			If mbEventsEnabled Then
				zzSetMainView(zzGetFilter())
				zzSetLayout()
			End If


		End Sub

		Private Sub chkHasDeviation_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkHasDeviation.CheckedChanged
			If mbEventsEnabled Then
				Me.Cursor = Cursors.WaitCursor
				zzSetMainView(zzGetFilter())
				zzSetLayout()
				Me.Cursor = Cursors.Default
			End If

		End Sub
		Private Sub chkHasOutPgonsDeviation_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkHasOutPgonsDeviation.CheckedChanged
			If mbEventsEnabled Then
				Me.Cursor = Cursors.WaitCursor
				zzSetMainView(zzGetFilter())
				zzSetLayout()
				Me.Cursor = Cursors.Default
			End If
		End Sub
		Private Sub cmdExcelAddReport_Click(oSender As System.Object, e As EventArgs) Handles cmdExcelAddReport.Click

			zzSetExproLuseReport_0226()
			''''''''''''zzSetExproLuseReport_0321()
		End Sub




		Private Sub cmdRefresh_Click(oSender As System.Object, e As EventArgs) Handles cmdRefresh.Click
			zzSetMainView(zzGetFilter())
			zzSetLayout()
			DMCommon.Debug.ExcelLog.SetDataTable(0, "moMainView", moMainView)
		End Sub

		Private Sub chkSetAllRowsCond_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkSetAllRowsCond.CheckedChanged
			If mbEventsEnabled Then
				Me.Cursor = Cursors.WaitCursor
				Dim iCurrentCheckState As CheckState = Me.chkSetAllRowsCond.CheckState
				mbEventsEnabled = False
				For iRowIndex As Integer = 0 To moMainView.Count - 1
					zzSetCond(iRowIndex, iCurrentCheckState)
				Next
				zzSetLayout()
				mbEventsEnabled = True
				Me.Cursor = Cursors.Default
			End If

		End Sub


		Private Sub cmdOpenEntConnected_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenEntConnected.Click
			mfEntConnected = New Expro.frmEntConnected()
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

			'  DMCommon.Debug.MsgBox("11_096", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEntConnected)
			Me.Visible = False
			mfEntConnected.Owner = Me
		End Sub

		Private Sub mfEntConnected_FormClosed(oSender As System.Object, e As FormClosedEventArgs) Handles mfEntConnected.FormClosed
			Me.Visible = True
		End Sub

		Private Sub cmdCalcEntConnected_Click(oSender As System.Object, e As EventArgs) Handles cmdCalcEntConnected.Click
			Me.Cursor = Cursors.WaitCursor
			mfEntConnected = New Expro.frmEntConnected()

			mfEntConnected.Owner = Me
			mfEntConnected.Calculate()
			'mfEntConnected.Dispose()
			'mfEntConnected

			Me.Cursor = Cursors.Default
		End Sub

		Private Sub cmdOpenEditlayerList_Click(oSender As System.Object, e As EventArgs) Handles cmdOpenEditlayerList.Click
			mfEditLayerList = New frmEditLayerList()
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)

			'  DMCommon.Debug.MsgBox("11_096", Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text, Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.WindowState)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfEditLayerList)
			Me.Visible = False
			mfEditLayerList.Owner = Me
		End Sub

		Private Sub frmExpro_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
			If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top - 100 Then
				Me.Location = New System.Drawing.Point(240, 240)
				Me.Size = New System.Drawing.Size(960, 600)
				Me.WindowState = FormWindowState.Normal
			End If
		End Sub

		Private Sub mfEditLayerList_FormClosed(sender As System.Object, e As FormClosedEventArgs) Handles mfEditLayerList.FormClosed
			Me.Visible = True
		End Sub

		Private Sub cmdExcelNewReport_Click(sender As Object, e As EventArgs) Handles cmdExcelNewReport.Click
			zzSetExproNewReportA()
		End Sub



		Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
			Dim sFilter As String = zzGetParcelFilter()
			Dim oRow As Data.DataRowView
			Dim iExproPgonTypeID As Integer
			Dim dParcelLegalArea As Double
			Dim dLegalAreaSum As Double


			Me.ctxLegalArea.ReadOnly = True
			Me.dgvMain.Columns.Item("ctxLegalArea").ReadOnly = True
			Dim oDataView As Data.DataView = New Data.DataView(moExproPgonsTable, sFilter, TopoManager.TPlanGraph.TplnParcel.msExproPgonTypeOrderByFieldName, Data.DataViewRowState.CurrentRows)
			Dim oaDataRows(oDataView.Count - 2) As Data.DataRow
			Dim iPgonIndex As Integer = 0
			DMCommon.Debug.MsgBox("01_020", oDataView.Count, miBlockNo, miBlockAdd, miParcelNo, oaDataRows.GetUpperBound(0))
			For iIndex As Integer = 0 To oDataView.Count - 1
				oRow = oDataView.Item(iIndex)
				iExproPgonTypeID = DMCommon.Functions.CIntN(oRow.Item("ExproPgonTypeID"))
				If iExproPgonTypeID = TopoManager.TPlanGraph.TplnExpro.ParcelExproTypeID Then
					dParcelLegalArea = DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
				Else
					dLegalAreaSum += DMCommon.Functions.CDblN(oRow.Item(TopoManager.TPlanGraph.TplnParcel.LegalAreaFieldName))
					DMCommon.Debug.MsgBox("01_050", oDataView.Count, oaDataRows.GetUpperBound(0), iPgonIndex)
					oaDataRows(iPgonIndex) = oRow.Row
					oaDataRows(iPgonIndex).Item("Version") = 0
					iPgonIndex += 1
				End If
			Next
			DMCommon.Debug.MsgBox("01_030", dLegalAreaSum, dParcelLegalArea, oaDataRows.GetUpperBound(0))
			If dLegalAreaSum = dParcelLegalArea Then
				zzClearParcelExpro()
				moExproDataAdapter.Update(oaDataRows)
				For iIndex As Integer = 0 To oaDataRows.GetUpperBound(0)
					oaDataRows(iIndex).Item("Version") = miCurrentVersion
				Next
				mbEventsEnabled = False
				Me.chbEdit.Checked = False
				mbEventsEnabled = True
			End If
		End Sub

		Private Sub chbEdit_CheckedChanged(sender As Object, e As EventArgs) Handles chbEdit.CheckedChanged
			If mbEventsEnabled Then
				If Me.chbEdit.Checked Then
					Dim oSelectedItem As System.Object = Me.cmbVersions.SelectedItem
					If oSelectedItem IsNot Nothing Then
						If oSelectedItem Is msNewVersion Then

						End If
					End If
					Me.ctxLegalArea.ReadOnly = False
					Me.dgvMain.Columns.Item("ctxLegalArea").ReadOnly = False
				Else
				End If
			End If

		End Sub
	End Class
End Namespace