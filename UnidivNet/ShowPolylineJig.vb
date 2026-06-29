Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology

Public Class ShowPolylineJig
   Inherits EntityJig
   Private moSourcePolyline As Polyline
   Sub New(oSourcePolyline As Polyline)
      MyBase.New(New Polyline())
      moSourcePolyline = oSourcePolyline
      Dim oPolyline As Polyline = GetEntity()
      zzCopyPolyline(moSourcePolyline, oPolyline)


   End Sub
   Public Function GetEntity() As Polyline
      Return DirectCast(MyBase.Entity, Polyline)
   End Function
   Protected Overrides Function Sampler(oJigPrompts As JigPrompts) As SamplerStatus
     

      Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions("Select segment:")

      Dim oClosestCurve As Curve = Nothing



      oJigPointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)

      oJigPointOpts.Cursor = CursorType.Crosshair

      Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
      '	System.Windows.Forms.MessageBox.Show(CStr(oResPoint Is Nothing), "01_013")
      If oResPoint IsNot Nothing Then
         Dim oPolyline As Polyline = GetEntity()
         oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
         zzCopyPolyline(moSourcePolyline, oPolyline)
      End If
   End Function

   Protected Overrides Function Update() As Boolean

   End Function
   Private Sub zzCopyPolyline(ByVal oSourcePolyline As Polyline, ByRef oDestPolyline As Polyline)
      Dim dBulge As Double
      Dim tPoint As Point2d
      Dim iPrevVertNum As Integer = oDestPolyline.NumberOfVertices
      For iIndex As Integer = 0 To oSourcePolyline.NumberOfVertices - 1
         dBulge = oSourcePolyline.GetBulgeAt(iIndex)
         tPoint = oSourcePolyline.GetPoint2dAt(iIndex)
         If iIndex < iPrevVertNum Then
            oDestPolyline.SetBulgeAt(iIndex, dBulge)
            oDestPolyline.SetPointAt(iIndex, tPoint)
         Else
            Try
               oDestPolyline.AddVertexAt(iIndex, tPoint, dBulge, 0.0, 0.0)
            Catch oEx As Autodesk.AutoCAD.Runtime.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & CStr(iIndex) & ":" & CStr(oDestPolyline.NumberOfVertices), "zzCopyPolyline_1")
            End Try

         End If
      Next
      For iIndex As Integer = iPrevVertNum - 1 To oSourcePolyline.NumberOfVertices Step -1
         Try
            oDestPolyline.RemoveVertexAt(iIndex)
         Catch oEx As Autodesk.AutoCAD.Runtime.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.ErrorStatus.ToString() & vbCrLf & CStr(iIndex) & ":" & CStr(oDestPolyline.NumberOfVertices), "zzCopyPolyline_2")
         End Try
      Next
      oDestPolyline.Closed = oSourcePolyline.Closed
   End Sub
End Class
