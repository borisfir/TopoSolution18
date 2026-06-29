Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology
Namespace TopoScheme


   Public Class TopoPgonJig
      Inherits EntityJig
      Private moTopoModel As TopologyModel
      Private moTopoScheme As TopoManager.TopoScheme.tsTopology = Nothing
      Private m_dims As DynamicDimensionDataCollection
      Private mtCurrentPoint As Point3d
      Private miPreviousPgonID As Integer = 0
      Private miCurrentPgonID As Integer = 0
      Private moPolygon As Polygon = Nothing
      Sub New(ByVal sTopoName As String, oPline As Polyline)
         MyBase.New(oPline)
         moTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
         If moTopoModel IsNot Nothing Then
            moTopoScheme = New TopoManager.TopoScheme.tsTopology(sTopoName)
            moTopoScheme.Load(False, moTopoModel)
         End If

         Dim oPolyline As Polyline = Me.GetEntity()
         oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
         zzInitDims()
      End Sub
      Protected Overrides Function Sampler(ByVal oJigPrompts As Autodesk.AutoCAD.EditorInput.JigPrompts) As Autodesk.AutoCAD.EditorInput.SamplerStatus
         Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions()
         oJigPointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)

         oJigPointOpts.Cursor = CursorType.Crosshair


         Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
         '	System.Windows.Forms.MessageBox.Show(CStr(oResPoint Is Nothing), "01_013")
         If oResPoint IsNot Nothing Then

         End If
         mtCurrentPoint = oResPoint.Value

         '	DMAcadExt.AcadDocument.WriteMessage(oResPoint.Status.ToString())


         'System.Windows.Forms.MessageBox.Show(CStr(moTopology Is Nothing), "01_013a")

         Try
            '	System.Windows.Forms.MessageBox.Show("", "01_013b")
            moPolygon = moTopoModel.FindPolygon(mtCurrentPoint)
         Catch oMapEx As Autodesk.Gis.Map.MapException
            ' System.Windows.Forms.MessageBox.Show(mtCurrentPoint.ToString(), "01_014")
         End Try

         If moPolygon IsNot Nothing Then
            miCurrentPgonID = moPolygon.ID

         End If
         If miCurrentPgonID = miPreviousPgonID Then
            Return SamplerStatus.NoChange
         Else
            Return SamplerStatus.OK
         End If
      End Function

      Protected Overrides Function Update() As Boolean
         ' Dim colRings As RingCollection '= oPgon.GetBoundary()
         Dim oPgon As TopoManager.TopoScheme.tsPolygon
         Try
            If moTopoScheme Is Nothing Then
               System.Windows.Forms.MessageBox.Show(" moTopoScheme Is Nothing", "09_014")
            End If
            oPgon = moTopoScheme.GetPolygon(miCurrentPgonID)
            If oPgon IsNot Nothing Then
               oPgon.CreateDBPolyline(Me.GetEntity())
            End If


         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "tsPolygon - Load")
            Return False
         End Try
         Return True


      End Function
      Public Function GetEntity() As Polyline
         MessageBox.Show(MyBase.Entity.GetType().ToString(), "09_780")
         Return DirectCast(MyBase.Entity, Polyline)
      End Function
      Private Sub zzInitDims()
         '	Return
         m_dims = New DynamicDimensionDataCollection()
         Dim dim0 As Dimension = New AlignedDimension()
         dim0.SetDatabaseDefaults()
         dim0.DynamicDimension = True

         m_dims.Add(New DynamicDimensionData(dim0, False, False))
         '	Dim dim1 As Dimension = New AlignedDimension()
         '	dim1.SetDatabaseDefaults()
         '	dim1.DynamicDimension = True
         '	m_dims.Add(New DynamicDimensionData(dim1, True, False))
      End Sub
   End Class
End Namespace