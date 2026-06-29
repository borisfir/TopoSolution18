Option Explicit On
Option Strict On

Imports Autodesk.Gis.Map.Topology
Imports System.Data
Public Class frmTopoView
   Private moTopoModel As TopologyModel
   Private msTopoName As String
   Private msaUnionTopoNames() As String = Nothing
   Private moOverlayODRecords() As OverlayODRecordSet
   Public Sub New()

      ' This call is required by the Windows Form Designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      Dim saAllTopoNames() As String = Nothing



      ODEditor.GetTopoNames(saAllTopoNames, msaUnionTopoNames)
      Try
         Me.cmbTopoNames.Items.AddRange(saAllTopoNames)


      Catch ex As Exception

      End Try
      zzInitUnion()

   End Sub
   Private Sub zzInitUnion()
      Dim iUnionTopoUB As Integer = msaUnionTopoNames.GetUpperBound(0)
      Dim sODTableName As String
      Dim oODTable As Autodesk.Gis.Map.ObjectData.Table
      If iUnionTopoUB >= 0 Then
         ReDim moOverlayODRecords(iUnionTopoUB)
         For iIndex As Integer = 0 To iUnionTopoUB
            sODTableName = msaUnionTopoNames(iIndex)
            oODTable = ODEditor.GetODTable(sODTableName)
            If oODTable IsNot Nothing Then
               moOverlayODRecords(iIndex) = New OverlayODRecordSet(oODTable)
               System.Windows.Forms.MessageBox.Show(oODTable.Description)
               Me.cmbUnions.Items.Add(oODTable.Description)
            End If
         Next
      End If


   End Sub
   Private Sub cmbTopoNames_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbTopoNames.SelectedIndexChanged
      Dim sTopoName As String = Me.cmbTopoNames.Text
      zzSetTopo(sTopoName)
      ' zzSetTopoSource(sTopoName)

   End Sub
   Private Sub zzSetTopo(ByVal sTopoName As String)
      Dim oTopoReader As TopoReader = New TopoReader(sTopoName)
      oTopoReader.ReadTopoData()
      Me.dgvMain.DataSource = oTopoReader.TopoDataTable
   End Sub
   Private Sub zzSetUnion(ByVal iIndex As Integer)
      Try
         Dim oOverlayODRecord As OverlayODRecordSet = moOverlayODRecords(iIndex)

         Dim oTopoReader As TopoReader = New TopoReader(oOverlayODRecord.ODTableName, oOverlayODRecord)
         System.Windows.Forms.MessageBox.Show("1990")
         Dim oDataTable As DataTable
         Dim oTopoModel As TopologyModel
         oDataTable = oTopoReader.TopoDataTable
         oTopoModel = oTopoReader.TopoModel
         System.Windows.Forms.MessageBox.Show("2020")
         oTopoReader.ReadTopoData()
         System.Windows.Forms.MessageBox.Show("2090")
         Me.dgvMain.DataSource = oDataTable
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "2256")
      End Try

   End Sub
   Private Sub zzSetTopoSource(ByVal sTopoName As String)
      Dim oDataTable As System.Data.DataTable = New System.Data.DataTable("Data")
      oDataTable.Columns.Add("Name", System.Type.GetType("System.String"))
      oDataTable.Columns.Add("Path", System.Type.GetType("System.String"))

      oDataTable.Columns.Add("Source", System.Type.GetType("System.String"))

      '   oDataTable.Columns.Add(msAcObjIDFldName, System.Type.GetType("System.Int32"))
      '    oDataTable.Columns.Add(msAreaFldName, System.Type.GetType("System.Double"))
      '    oDataTable.Columns.Add(msCentroidXFldName, System.Type.GetType("System.Double"))
      '   oDataTable.Columns.Add(msCentroidYFldName, System.Type.GetType("System.Double"))
      Dim oTopoSourceCol As Autodesk.Gis.Map.Topology.TopologySourceCollection = Common.GetTopologySourceCol(sTopoName)
      Dim oNewRow As System.Data.DataRow
      


      For Each oTopologySource As Autodesk.Gis.Map.Topology.TopologySource In oTopoSourceCol
         oNewRow = oDataTable.NewRow()
         oNewRow.Item("Name") = oTopologySource.Name
         oNewRow.Item("Path") = oTopologySource.Path
         oNewRow.Item("Source") = oTopologySource.Source

         oDataTable.Rows.Add(oNewRow)
      Next
      Me.dgvMain.DataSource = oDataTable


   End Sub

   Private Sub cmbUnions_SelectedIndexChanged(ByVal oSender As System.Object, ByVal e As System.EventArgs) Handles cmbUnions.SelectedIndexChanged
      zzSetUnion(Me.cmbUnions.SelectedIndex)
   End Sub

End Class