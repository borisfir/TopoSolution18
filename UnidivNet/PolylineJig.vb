Option Explicit On
Option Strict On
Imports System
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map.Topology

Public Class PolylineJig
	Inherits EntityJig
	Private moaCurves() As Curve
   Private Shared mdMaxDistToCurve As Double = 1.0
   Private mtCurrentPoint As Point3d
   Private miSamplerCounter As Integer = 0
   Private miUpdateCounter As Integer = 0

   Sub New(ByVal oaCurves() As Curve)
      MyBase.New(New Line())
      moaCurves = oaCurves


      Dim oLine As Line = Me.GetEntity()
      oLine.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
      '   zzInitDims()
   End Sub
   Protected Overrides Function Sampler(oJigPrompts As JigPrompts) As SamplerStatus
      miSamplerCounter += 1


      Dim oJigPointOpts As JigPromptPointOptions = New JigPromptPointOptions("Select segment:")
      Dim tClosestPointOnCurve As Point3d
      Dim oClosestCurve As Curve = Nothing
      Dim tClosestPointOnClosestCurve As Point3d

      Dim dDistToCurveMin As Double = mdMaxDistToCurve
      Dim dDistToCurve As Double = 999999.0

      oJigPointOpts.UserInputControls = (UserInputControls.Accept3dCoordinates And UserInputControls.NoZeroResponseAccepted And UserInputControls.NoNegativeResponseAccepted)

      oJigPointOpts.Cursor = CursorType.Crosshair

      Dim oResPoint As PromptPointResult = oJigPrompts.AcquirePoint(oJigPointOpts)
      '	System.Windows.Forms.MessageBox.Show(CStr(oResPoint Is Nothing), "01_013")
      If oResPoint IsNot Nothing Then
         mtCurrentPoint = oResPoint.Value
         For iIndex As Integer = 0 To moaCurves.GetUpperBound(0)
            tClosestPointOnCurve = moaCurves(iIndex).GetClosestPointTo(mtCurrentPoint, False)
            dDistToCurve = mtCurrentPoint.DistanceTo(tClosestPointOnCurve)
            If dDistToCurve <= dDistToCurveMin Then
               dDistToCurveMin = dDistToCurve
               oClosestCurve = moaCurves(iIndex)
               tClosestPointOnClosestCurve = tClosestPointOnCurve
            End If

         Next
      End If
      If oClosestCurve IsNot Nothing Then
         Dim dPointParameter As Double = oClosestCurve.GetParameterAtPoint(tClosestPointOnClosestCurve)
         Dim oPolyline As Polyline

         Dim iSegmentType As SegmentType
         Dim oLine As Line
         '  Dim tVertex As Vertex
         '  Dim dBulge As Double ' = oPolyline.GetBulgeAt(iIndex)

         If oClosestCurve.GetRXClass().Name = DMAcadExt.AcadConst.AcadPolylineName Then
            oPolyline = DirectCast(oClosestCurve, Polyline)
            Dim iIndex As Integer = Convert.ToInt32(Math.Floor(dPointParameter))
            Dim iNextIndex As Integer = (iIndex + 1) Mod oPolyline.NumberOfVertices

            iSegmentType = oPolyline.GetSegmentType(iIndex)

            If iSegmentType = SegmentType.Line Then
               oLine = GetEntity()

               oLine.StartPoint = oPolyline.GetPoint3dAt(iIndex)
               oLine.EndPoint = oPolyline.GetPoint3dAt(iNextIndex)
               '   oLine.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 64, 64)
            End If
         End If

         '   tVertex = oPolyline.GetPoint3dAt(iIndex)
         '   dBulge = oPolyline.GetBulgeAt(iIndex)

      End If
      zzPrintMsg(oResPoint)
   End Function
   Public Function GetEntity() As Line
      Return DirectCast(MyBase.Entity, Line)
   End Function
   Public Sub CreateEntity()
      Dim oEntity As Entity = MyBase.Entity
      If oEntity IsNot Nothing Then
         oEntity.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 128, 128)
         oEntity.Linetype = "DASHED2"
         DMAcadExt.AcadTransaction.AppendEntity(oEntity)
      End If

   End Sub
   Protected Overrides Function Update() As Boolean
      Return True
      Dim oLine, oNewLine As Line
      miUpdateCounter += 1
      oLine = Me.GetEntity()
      If oLine IsNot Nothing Then
         oLine.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 255)
         oLine.Visible = True
         oNewLine = New Line(oLine.StartPoint, oLine.EndPoint)
         oLine.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 128, 0)
         oLine.Linetype = "DASHED2"
         '   DMAcadExt.AcadTransaction.AppendEntity(oLine, False)

      End If
      zzPrintMsg(Nothing)
   End Function
   Private Sub zzPrintMsg(oResPoint As PromptPointResult)
      If oResPoint Is Nothing Then
         DMAcadExt.AcadDocument.WriteMessage("Sampler: " & miSamplerCounter.ToString() & "; Update: " & miUpdateCounter.ToString())
      Else
         DMAcadExt.AcadDocument.WriteMessage("Status: " & oResPoint.Status.ToString() & "; Sampler: " & miSamplerCounter.ToString() & "; Update: " & miUpdateCounter.ToString())
      End If
   End Sub
   Private Shared Sub zzSegmentInfo(ByVal oPolyline As Polyline, tPoint As Point3d)
      Dim tClosestPointOnCurve As Point3d = oPolyline.GetClosestPointTo(tPoint, False)
      Dim dPointParameter As Double = oPolyline.GetParameterAtPoint(tClosestPointOnCurve)
      Dim iIndex As Integer = Convert.ToInt32(Math.Floor(dPointParameter))
      Dim iNextIndex As Integer = (iIndex + 1) Mod oPolyline.NumberOfVertices
      '   DMAcadExt.AcadDocument.WriteMessage(vbCrLf & CStr(999) & ":" & dPointParameter.ToString() & "; " & DMAcadExt.TPlnPoint.DispPoint(tClosestPointOnCurve))
      Dim tNextVertex, tVertex As Point3d
      Dim dNextBulge, dBulge As Double
      Dim sNextBulge, sBulge As String
      ' Return
      Dim iVerticesUB As Integer = oPolyline.NumberOfVertices - 1
      Dim iSegmentType As SegmentType
      iSegmentType = oPolyline.GetSegmentType(iIndex)
      tVertex = oPolyline.GetPoint3dAt(iIndex)
      dBulge = oPolyline.GetBulgeAt(iIndex)
      If dBulge <> 0.0 Then
         sBulge = ";B=" & FormatNumber(dBulge, 4)
      Else
         sBulge = String.Empty
      End If
      tNextVertex = oPolyline.GetPoint3dAt(iNextIndex)
      dNextBulge = oPolyline.GetBulgeAt(iNextIndex)

      If dNextBulge <> 0.0 Then
         sNextBulge = ";B=" & FormatNumber(dNextBulge, 4)
      Else
         sNextBulge = String.Empty
      End If
      Dim dAngleX As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.XAxis)
      Dim dAngleY As Double = oPolyline.GetFirstDerivative(tClosestPointOnCurve).GetAngleTo(Vector3d.YAxis)

      '  DMAcadExt.AcadDocument.WriteMessage("Polyline Points: " & tClosestPointOnCurve.ToString() & "; " & dPointParameter.ToString())
      '  DMAcadExt.AcadDocument.WriteMessage(vbCrLf & ":" & iSegmentType.ToString() & "; " & zzGetDir(tVertex, tNextVertex) & ", Angle X: " & (180.0 * dAngleX / Math.PI).ToString() & " Angle Y: " & (180.0 * dAngleY / Math.PI).ToString() & vbCrLf & CStr(iIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tVertex) & sBulge & vbCrLf & CStr(iNextIndex + 1) & "; " & DMAcadExt.TPlnPoint.DispPoint(tNextVertex) & sNextBulge)
   End Sub
End Class
