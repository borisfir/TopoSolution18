
Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data

Imports Autodesk.Gis.Map.Topology
Namespace TPlanGraph

	Public Class TplnBNPgon
		Inherits TPlanGraph.TplnTopoPgon
		Private Shared moMainDataTable As System.Data.DataTable

		Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon, True)
			'	MyBase.SetAttributeOrder()


			'	MyBase.SetAttributeOrder(BlockAttribIndex)
			'	DMCommon.Functions.DispArray(dsaBlockAttribText, "01_542m", True)


		End Sub

		Public Shared Sub Initialize(tBNMapThemeData As DMAcadExt.MapThemeData)

		End Sub
		Public Shared Sub CreateMainDataTable()
			moMainDataTable = New System.Data.DataTable("Ownership")
			'	moMainColumnIndices = New Dictionary(Of Integer, Integer)



			With moMainDataTable.Columns


				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TopoReader.msCentroidXFldName, GetType(System.Double))
				.Add(TopoReader.msCentroidYFldName, GetType(System.Double))

				.Add(TopoReader.msPerimeterFldName, GetType(System.Double))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
				.Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))

			End With
		End Sub
		Public Sub AddDataToMainTable()
			If moMainDataTable IsNot Nothing Then
				Dim oNewRow As System.Data.DataRow
				Try
					oNewRow = moMainDataTable.NewRow()
					With oNewRow



						.Item(TopoReader.msAreaFldName) = Me.AcadArea(False)
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

		Public Shared ReadOnly Property MainView() As System.Data.DataView
			Get
				Dim sSort As String = ""
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

		Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
			Get
				Throw New NotImplementedException()
			End Get
		End Property
		Public Shared ReadOnly Property MainDataTable() As System.Data.DataTable
			Get
				Return moMainDataTable
			End Get
		End Property
		Public Overrides Sub Terminate()
			Throw New NotImplementedException()
		End Sub
	End Class
End Namespace