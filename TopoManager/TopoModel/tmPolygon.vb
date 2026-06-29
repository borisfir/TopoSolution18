Option Explicit On
Option Strict On
Public Class tmPolygon
   Inherits SimplePgon

   Public Const PgonTestA As Integer = 4
   Private miID As Integer
   Private moExteriorRing As tmRing
   Private moaInteriorRing() As tmRing
   Private mcolCentroidsObjID As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Public Sub New(oExterior As tmRing)
      miID = oExterior.ID
      moExteriorRing = oExterior
      dcolLines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      dcolLines.Add(oExterior.Polyline.ObjectId)
      ReDim doaBulgeVertexArray(0)
		dtExteriorHandle = moExteriorRing.Handle
		doaBulgeVertexArray(0) = New GeoUtilites.BulgeVertexArray(oExterior.Polyline)
   End Sub
   Public Sub AddInteriorRing(oInterior As tmRing)
      Dim iUB As Integer
      If moaInteriorRing Is Nothing Then
         iUB = 0
      Else
         iUB = moaInteriorRing.GetUpperBound(0) + 1
      End If
      ReDim Preserve moaInteriorRing(iUB)
      moaInteriorRing(iUB) = oInterior
      dcolLines.Add(oInterior.Polyline.ObjectId)
   End Sub
   Public ReadOnly Property ID As Integer
      Get
         Return miID
      End Get
   End Property




	Public ReadOnly Property IslandCount As Integer
      Get
         If moaInteriorRing Is Nothing Then
            Return 0
         Else
            Return moaInteriorRing.GetUpperBound(0) + 1
         End If
      End Get
   End Property
   Public Property Centroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
      Get
         Return mcolCentroidsObjID
      End Get
      Set(colValue As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
         mcolCentroidsObjID = colValue
      End Set
   End Property
   Public Sub InfoToExcel()
      '  Dim iRow As Integer
      Dim iCol As Integer = 1
      '   DMCommon.ExcelLog.SetNextValue(iRow, iCol, moExteriorRing.ID, moExteriorRing.Handle.ToString)
      iCol = 4
      If moaInteriorRing IsNot Nothing Then


         For iRingIndex As Integer = 0 To moaInteriorRing.GetUpperBound(0)

            '  DMCommon.ExcelLog.SetValue(iRow, iCol, moaInteriorRing(iRingIndex).ID, moaInteriorRing(iRingIndex).Handle.ToString)
            iCol += 2
         Next
      End If
      If mcolCentroidsObjID IsNot Nothing Then
         For Each tAcObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId In mcolCentroidsObjID
            '  DMCommon.ExcelLog.SetValue(iRow, iCol, tAcObjId.ToString)
            iCol += 1
         Next
      End If


   End Sub

   Public Sub Build(bHasBoundary As Boolean)
      Dim oRing As tmRing
      moExteriorRing.Build(bHasBoundary)
      If moaInteriorRing IsNot Nothing Then
         For iIndex As Integer = 0 To moaInteriorRing.GetUpperBound(0)
            oRing = moaInteriorRing(iIndex)
            If oRing IsNot Nothing Then
               DMAcadExt.AcadDocument.WriteMessage("PgonID=" & CStr(miID) & "||" & "RingID=" & CStr(oRing.ID) & "||" & "ChainCount=" & CStr(oRing.ChainCount))
               oRing.Build(False)
            End If
         Next
      End If
      '	moExteriorRing.DebugWrite("AfterBuild")
   End Sub
   Public Sub Complete()
      Dim oRing As tmRing
      moExteriorRing.Complete(0)
      If moaInteriorRing IsNot Nothing Then
         For iIndex As Integer = 0 To moaInteriorRing.GetUpperBound(0)
            oRing = moaInteriorRing(iIndex)
            If oRing IsNot Nothing Then
               DMAcadExt.AcadDocument.WriteMessage("PgonID=" & CStr(miID) & "||" & "RingID=" & CStr(oRing.ID) & "||" & "ChainCount=" & CStr(oRing.ChainCount))
               oRing.Complete(moExteriorRing.ID)
            End If
         Next
      End If
   End Sub
   Public Sub CreatePolylines()
      Dim oRing As tmRing
      ''''''	moExteriorRing.DebugWrite("CreateP_Ext", True)
      moExteriorRing.CreatePolylines()

      If moaInteriorRing IsNot Nothing Then
         For iIndex As Integer = 0 To moaInteriorRing.GetUpperBound(0)
            oRing = moaInteriorRing(iIndex)
            If oRing IsNot Nothing Then
               'MessageBox.Show("PgonID=" & CStr(miID) & vbCrLf & "RingID=" & CStr(oRing.ID) & vbCrLf & "ChainCount=" & CStr(oRing.ChainCount), "03_903")

               oRing.CreatePolylines(True)
            End If
         Next

      End If

   End Sub
   Public Sub DrawEdges()
      Dim oRing As tmRing
      ''''''	moExteriorRing.DebugWrite("CreateP_Ext", True)
      moExteriorRing.DrawEdges()

      If moaInteriorRing IsNot Nothing Then
         For iIndex As Integer = 0 To moaInteriorRing.GetUpperBound(0)
            oRing = moaInteriorRing(iIndex)
            If oRing IsNot Nothing Then
               'MessageBox.Show("PgonID=" & CStr(miID) & vbCrLf & "RingID=" & CStr(oRing.ID) & vbCrLf & "ChainCount=" & CStr(oRing.ChainCount), "03_903")

               oRing.DrawEdges(True)
            End If
         Next

      End If

   End Sub
End Class
