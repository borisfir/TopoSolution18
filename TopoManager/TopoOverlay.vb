Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput

Public Class TopoOverlay
   Private Shared msTopoSourceName As String
   Private Shared msTopoOverlayName As String
   Private Shared msTopoResultName As String '= "dmUnion_TopoAAAA_TopoBBBB"


   Public Shared Sub ExecTplan()
      Dim oTopoModelSource As TopologyModel = Nothing
      Dim oTopoModelOverlay As TopologyModel = Nothing

      Dim oApp As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopologies As Topologies = oApp.ActiveProject.Topologies
      Dim oODTableResult As ObjectDataTable
      Dim oSourceDataCol As OverlayDataCollection = New OverlayDataCollection()
      Dim oOverlayDataCol As OverlayDataCollection = New OverlayDataCollection()
      '  Dim sExpression As String
      '  Dim oOverlayData As OverlayData

      Dim nodeCreationSettings As PointCreationSettings = New PointCreationSettings("result", 1, True, "ACAD_POINT")

      'Set the Node settings

      If Not oTopologies.Exists(msTopoResultName) Then
         oODTableResult.ODTableName = msTopoResultName
         oODTableResult.ODTableDescription = "Union: " & msTopoSourceName & ", " & msTopoOverlayName
         Try
            oTopoModelSource = oTopologies(msTopoSourceName)
            oTopoModelOverlay = oTopologies(msTopoOverlayName)
            oTopoModelSource.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
            oTopoModelOverlay.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

            oTopoModelSource.SetNodeCreationSettings(nodeCreationSettings)
            oTopoModelSource.Union(oTopoModelOverlay, msTopoResultName, "", oODTableResult, oSourceDataCol, oOverlayDataCol)
            ''    System.Windows.Forms.MessageBox.Show("OK!!!")
         Catch ex As Autodesk.Gis.Map.MapException
            System.Windows.Forms.MessageBox.Show("Status: " & CStr(ex.ErrorCode) & vbCrLf & Err.Source, "Err")
				''   Utility.GetEditor.WriteMessage(String.Format(vbCrLf & "Exception throwed containing the error code: {0}", err.ErrorCode))
         End Try
         oTopoModelSource.Close()
         oTopoModelOverlay.Close()
         Try
            Dim oTopoModelRes As TopologyModel = Nothing
            oTopoModelRes = oTopologies(msTopoResultName)
            oTopoModelRes.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
            oTopoModelRes.Description = "Union Topologies: '" & msTopoSourceName & "' and '" & msTopoOverlayName & "'"
            oTopoModelRes.Close()
         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message, "ExecTplan")
         End Try
      End If
   End Sub
	Public Sub CreateOverlayAAA(ByVal oTopoDef As DMAcadExt.TopoDef)
		'    Dim iResultID As TopoDef = oTopoDef.ID
		'    If TPlanGraph.TplnProject.TopoIsUnion_B(iResultID) Then

		'  End If
	End Sub
   Public Shared Sub Exec()
      Dim oTopoModelSource As TopologyModel = Nothing
      Dim oTopoModelOverlay As TopologyModel = Nothing

      Dim oApp As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
      Dim oTopologies As Topologies = oApp.ActiveProject.Topologies
      Dim oODTableResult As ObjectDataTable
      Dim oSourceDataCol As OverlayDataCollection = New OverlayDataCollection()
      Dim oOverlayDataCol As OverlayDataCollection = New OverlayDataCollection()
      Dim sExpression As String
      Dim oOverlayData As OverlayData

      sExpression = String.Format(":AREA@TPMCNTR_{0}", msTopoSourceName)

      oOverlayData = New OverlayData(sExpression, "SourcePgonArea", Autodesk.Gis.Map.Constants.DataType.Real)
      oSourceDataCol.Add(oOverlayData)
      oOverlayData = New OverlayData("@Name", "SourceName", Autodesk.Gis.Map.Constants.DataType.Character)
      oSourceDataCol.Add(oOverlayData)


      sExpression = String.Format(":AREA@TPMCNTR_{0}", msTopoOverlayName)

      oOverlayData = New OverlayData(sExpression, "OverlayPgonArea", Autodesk.Gis.Map.Constants.DataType.Real)
      oOverlayDataCol.Add(oOverlayData)
      oOverlayData = New OverlayData("@Name", "OverlayName", Autodesk.Gis.Map.Constants.DataType.Character)
      oOverlayDataCol.Add(oOverlayData)

      Dim nodeCreationSettings As PointCreationSettings = New PointCreationSettings("result", 1, True, "ACAD_POINT")

      'Set the Node settings

      Try

         For iTopoNameIndex As Integer = 1 To 99
            msTopoResultName = Common.UnionTopoPrefix & "_" & iTopoNameIndex.ToString()
            If Not oTopologies.Exists(msTopoResultName) Then Exit For
            If iTopoNameIndex = 99 Then Exit Sub
         Next

      Catch ex As Exception

      End Try
      oODTableResult.ODTableName = msTopoResultName
      oODTableResult.ODTableDescription = "Union: " & msTopoSourceName & "," & msTopoOverlayName
      Try
         oTopoModelSource = oTopologies(msTopoSourceName)

         oTopoModelOverlay = oTopologies(msTopoOverlayName)


         oTopoModelSource.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

         oTopoModelOverlay.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)

         oTopoModelSource.SetNodeCreationSettings(nodeCreationSettings)

         oTopoModelSource.Union(oTopoModelOverlay, msTopoResultName, "", oODTableResult, oSourceDataCol, oOverlayDataCol)

         System.Windows.Forms.MessageBox.Show("OK!!!")
      Catch err As Autodesk.Gis.Map.MapException
         System.Windows.Forms.MessageBox.Show("Status: " & CStr(err.ErrorCode) & vbCrLf & err.Source, "Err")
			''   Utility.GetEditor.WriteMessage(String.Format(vbCrLf & "Exception throwed containing the error code: {0}", err.ErrorCode))
      End Try
      oTopoModelSource.Close()
      oTopoModelOverlay.Close()
      Try
         Dim oTopoModelRes As TopologyModel = Nothing
         oTopoModelRes = oTopologies(msTopoResultName)
         oTopoModelRes.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
         oTopoModelRes.Description = "Union Topologies: '" & msTopoSourceName & "' and '" & msTopoOverlayName & "'"
      Catch ex As Exception

      End Try
   End Sub
   Public Shared Property TopoSourceName() As String
      Get
         Return msTopoSourceName
      End Get
      Set(ByVal sValue As String)
         msTopoSourceName = sValue
      End Set
   End Property
   Public Shared Property TopoOverlayName() As String
      Get
         Return msTopoOverlayName
      End Get
      Set(ByVal sValue As String)
         msTopoOverlayName = sValue
      End Set
   End Property
   Public Shared Property TopoResultName() As String
      Get
         Return msTopoResultName
      End Get
      Set(ByVal sValue As String)
         msTopoResultName = sValue
      End Set
   End Property
End Class
