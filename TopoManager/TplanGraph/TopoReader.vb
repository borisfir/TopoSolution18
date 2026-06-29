Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Public Class TopoReader
	Public Const msAreaFldName As String = "Area"
	Public Const msAcadAreaFldName As String = "AcadArea"

	Public Const msCalcAreaFldName As String = "CalcArea"
	Public Const msCalcArea2FldName As String = "CalcArea2"
	Public Const msRoundedAreaFldName As String = "RoundedArea"



	Public Const msSumPgonAreaFldName As String = "SumArea"
	Public Const msSumPgonAreaUnionFldName As String = "SumAreaUn"
   Public Const msSumPgonAreaFDO_OverlayFldName As String = "SumAreaFO"

   Public Const msSumAreaFldName As String = "SumArea"
   Public Const msSumLegalAreaFldName As String = "SumLegalArea"

   Public Const BasePgonCountFieldName As String = "BasePgonCount"

	Public Const msCentroidXFldName As String = "CentroidX"
	Public Const msCentroidYFldName As String = "CentroidY"
	Public Const msAcObjIDFldName As String = "AcObjID"
	Public Const msTopoIDFldName As String = "TopoID"
	Public Const msLotTopoIDFldName As String = "LotTopoID"

	Public Const msParcelTopoIDFldName As String = "ParcelTopoID"

   Public Const msPerimeterFldName As String = "Perimeter"

   Public Const BlockExistsFieldName As String = "BlockExists"
   Public Const PgonExistsFieldName As String = "PgonExists"


   Const msSourceAreaFldName As String = "SourceArea"
   Const msSourceNameFldName As String = "SourceName"
   Const msOverlayAreaFldName As String = "OverlayArea"
   Const msOverlayNameFldName As String = "OverlayName"

   Private msTopoName As String
   Private moTopoModel As TopologyModel
	Private mcolPolygons As PolygonCollection
	Private mbIsOverlayResult As Boolean
	'  Private moDataTable As System.Data.DataTable
	Private moDataTable As System.Data.DataTable
	Private moOverlayODRecordSet As OverlayODRecordSet
	Public Sub New(ByVal sTopoName As String)
		msTopoName = sTopoName
		moTopoModel = DMAcadExt.AcadMapApp.GetTopology(msTopoName)
		zzCreateTable(False)
	End Sub
	Public Sub New(ByVal sTopoName As String, ByVal oOverlayODRecord As OverlayODRecordSet)
		msTopoName = sTopoName
		moTopoModel = DMAcadExt.AcadMapApp.GetTopology(msTopoName)
		moOverlayODRecordSet = oOverlayODRecord
		zzCreateTable(True)
	End Sub

	Public Sub ReadTopoData()
		Dim oNewRow As System.Data.DataRow

		If moTopoModel IsNot Nothing Then
			Try
				moTopoModel.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
				mcolPolygons = moTopoModel.GetPolygons()
				Dim oOverlayODRecord As OverlayODRecord

				For Each oPolygon As Polygon In mcolPolygons
					oNewRow = moDataTable.NewRow()
					oNewRow.Item(msTopoIDFldName) = oPolygon.ID
					oNewRow.Item(msAcObjIDFldName) = oPolygon.Entity.ToString()
					oNewRow.Item(msCentroidXFldName) = oPolygon.Centroid.X
					oNewRow.Item(msCentroidYFldName) = oPolygon.Centroid.Y
					oNewRow.Item(msAreaFldName) = oPolygon.Area
					oNewRow.Item(msPerimeterFldName) = oPolygon.Perimeter
					If moOverlayODRecordSet IsNot Nothing Then
						oOverlayODRecord = moOverlayODRecordSet.GetOverlayODRecord(oPolygon.Entity)
						oNewRow.Item(msSourceAreaFldName) = oOverlayODRecord.SourcePgonArea
						oNewRow.Item(msSourceNameFldName) = oOverlayODRecord.SourcePgonName
						oNewRow.Item(msOverlayAreaFldName) = oOverlayODRecord.OverlayPgonArea
						oNewRow.Item(msOverlayNameFldName) = oOverlayODRecord.OverlayPgonName


						moDataTable.Rows.Add(oNewRow)
					End If
				Next oPolygon

			Catch ex As Exception
				System.Windows.Forms.MessageBox.Show(ex.Message, "3080")

			End Try
			If moTopoModel.Status <> Status.Closed Then
				moTopoModel.Close()
			End If


		End If
	End Sub
   Public ReadOnly Property TopoModel() As TopologyModel
      Get
         Return moTopoModel
      End Get
   End Property
   Private Sub zzCreateTable(ByVal bUnionFields As Boolean)
      moDataTable = New System.Data.DataTable("TopoData")
      moDataTable.Columns.Add(msTopoIDFldName, System.Type.GetType("System.Int32"))
		moDataTable.Columns.Add(msAcObjIDFldName, System.Type.GetType("System.Int64"))
      moDataTable.Columns.Add(msAreaFldName, System.Type.GetType("System.Double"))
      moDataTable.Columns.Add(msCentroidXFldName, System.Type.GetType("System.Double"))
      moDataTable.Columns.Add(msCentroidYFldName, System.Type.GetType("System.Double"))
      moDataTable.Columns.Add(msPerimeterFldName, System.Type.GetType("System.Double"))
      If bUnionFields Then
         moDataTable.Columns.Add(msSourceAreaFldName, System.Type.GetType("System.Double"))
         moDataTable.Columns.Add(msSourceNameFldName, System.Type.GetType("System.String"))
         moDataTable.Columns.Add(msOverlayAreaFldName, System.Type.GetType("System.Double"))
         moDataTable.Columns.Add(msOverlayNameFldName, System.Type.GetType("System.String"))
      End If




   End Sub

   Public ReadOnly Property TopoDataTable() As System.Data.DataTable
      Get
         Return moDataTable
      End Get
   End Property


End Class


