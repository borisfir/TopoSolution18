Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TPlanGraph
	Public Class TplnLusePgon
		Inherits TplnTopoPgon

		Private Const msApprovedTopoName As String = "LandusesK"
		Private Const msProposedTopoName As String = "LandusesM"

		Private miLanduseID As Integer = 0
		Private msLanduseName As String = "-"
		Private miLanduseOrder As Integer = 0
		Private miTopoPurpose As DMAcadExt.enTopoPurpose
		Private Shared moLusePgonApprDataTable As System.Data.DataTable
		Private Shared moLusePgonPropDataTable As System.Data.DataTable

		Public Sub New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon, True)
			miTopoPurpose = iTopoPurpose
		End Sub
		Public Property LanduseID() As Integer
			Get
				Return miLanduseID
			End Get
			Set(ByVal iValue As Integer)
				miLanduseID = iValue
			End Set
		End Property
		Public Property LanduseName() As String
			Get
				Return msLanduseName
			End Get
			Set(ByVal sValue As String)
				msLanduseName = sValue
			End Set
		End Property
		Private Shared Function zzGetLusePgonTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As System.Data.DataTable

			If iTopoPurpose = enTopoPurpose.Proposed Then
				Return moLusePgonPropDataTable
			ElseIf iTopoPurpose = enTopoPurpose.Approved Then
				Return moLusePgonApprDataTable
			Else
				Return Nothing
			End If

		End Function
		Public Shared Sub CreateLusePgonDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			Dim oDataTable As System.Data.DataTable = New System.Data.DataTable(TopoName(iTopoPurpose))

			With oDataTable.Columns
				'.Add(msNameFieldName, GetType(System.STRING))
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))

				.Add(TopoReader.msAreaFldName, GetType(System.Double))

				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))
				'			.Add(msInPlanFieldName, GetType(Odbc.OdbcType.Bit))
				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
				.Add(TplnLot.msLanduseOrderFieldName, GetType(System.Int32))

			End With

			If iTopoPurpose = enTopoPurpose.Proposed Then
				moLusePgonPropDataTable = oDataTable
			ElseIf iTopoPurpose = enTopoPurpose.Approved Then
				moLusePgonApprDataTable = oDataTable
			End If

		End Sub
		Public Shared Sub AddDataToLusePgonTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal oLusePolygon As TopoManager.TPlanGraph.TplnLusePgon, ByVal iLanduseID As Integer, ByVal sLanduseName As String)
			Dim oDataTable As System.Data.DataTable = zzGetLusePgonTable(iTopoPurpose)
			If oDataTable IsNot Nothing Then
				Try
					Dim oNewRow As System.Data.DataRow
					oNewRow = oDataTable.NewRow()
					With oNewRow
						.Item(TplnParcel.LanduseIDFieldName) = iLanduseID
						.Item(TplnParcel.LanduseNameFieldName) = sLanduseName
						.Item(TopoReader.msAreaFldName) = oLusePolygon.AcadArea(False)

						.Item(TopoReader.msCentroidXFldName) = oLusePolygon.CentroidX
						.Item(TopoReader.msCentroidYFldName) = oLusePolygon.CentroidY

						.Item(TopoReader.msPerimeterFldName) = oLusePolygon.Perimiter
						.Item(TopoReader.msTopoIDFldName) = oLusePolygon.TopoID
						.Item(TopoReader.msAcObjIDFldName) = oLusePolygon.CentroidAcObjID ''''''''''''''''.OldIdPtr.ToInt64()


						'	.Item(msLanduseOrderName)

					End With
					oDataTable.Rows.Add(oNewRow)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnLot - AddDataToLusePgonTable_1")
				End Try
			Else
				System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "TplnLot - AddDataToLusePgonTable_2")
			End If


		End Sub
		Public Shared Function GetLusePgonDataTable(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As System.Data.DataView
			Dim oLusePgonDataTable As System.Data.DataTable = zzGetLusePgonTable(iTopoPurpose)
			If oLusePgonDataTable IsNot Nothing Then
            '  System.Windows.Forms.MessageBox.Show(CStr(oLusePgonDataTable.Rows.Count), "01_711")
				Dim sSort As String = "" 'msBlockFieldName & "," & msParcelOrderFieldName & "," & msLanduseOrderFieldName
				Dim oDataView As System.Data.DataView = New System.Data.DataView(oLusePgonDataTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Function
		Public Shared ReadOnly Property TopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
			Get
				If iTopoPurpose = enTopoPurpose.Proposed Then
					Return msProposedTopoName
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					Return msApprovedTopoName
				Else
					Return Nothing
				End If
			End Get
		End Property
		Public Overrides Sub Terminate()

		End Sub
		Public Shared Sub SharedTerminate()


			If moLusePgonApprDataTable IsNot Nothing Then
				moLusePgonApprDataTable.Dispose()
				moLusePgonApprDataTable = Nothing
			End If

			If moLusePgonPropDataTable IsNot Nothing Then
				moLusePgonPropDataTable.Dispose()
				moLusePgonPropDataTable = Nothing
			End If

		End Sub

      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
          
      End Property
      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property

	End Class
End Namespace

