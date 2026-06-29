Option Explicit On
Option Strict On
Public Class tmPolygons
	Inherits Dictionary(Of Integer, tmPolygon)
   Public Function AddRingPair(oExteriorRing As tmRing, oInterior As tmRing) As tmPolygon
      '	MessageBox.Show(CStr(oExteriorRing.ID) & ":" & CStr(oInterior.ID), "03_466")
      Dim oPolygon As tmPolygon = Nothing
      oExteriorRing.IsExterior = TriState.True
      oInterior.IsExterior = TriState.False

      If Not MyBase.TryGetValue(oExteriorRing.ID, oPolygon) Then
         oPolygon = New tmPolygon(oExteriorRing)
         MyBase.Add(oExteriorRing.ID, oPolygon)
      End If
      oPolygon.AddInteriorRing(oInterior)
      Return oPolygon
   End Function
   Public Function AddIfNotExists(oExteriorRing As tmRing) As tmPolygon
      Dim oPolygon As tmPolygon = Nothing
      If Not MyBase.ContainsKey(oExteriorRing.ID) Then
         oExteriorRing.IsExterior = TriState.True
         oPolygon = New tmPolygon(oExteriorRing)
         MyBase.Add(oExteriorRing.ID, oPolygon)
         Return oPolygon
      Else
         Return Nothing
      End If

   End Function
	Public Sub CreateTopoLinks(bHasBoundary As Boolean)
		For Each oPgon As tmPolygon In MyBase.Values
			'If oPgon.ID = 50 Or oPgon.ID = 54 Then
			DMAcadExt.AcadDocument.WriteMessage("***************** P G O N  ID = " & oPgon.ID.ToString() & "----" & oPgon.ExteriorHandle.ToString() & "; " & "Cnt=" & CStr(MyBase.Count) & " *****************" & CStr(MyBase.Count))

			If True Or (tmPolygon.PgonTestA = 0 OrElse oPgon.ID = tmPolygon.PgonTestA) Then
				oPgon.Build(bHasBoundary)
				DMAcadExt.AcadDocument.WriteMessage("^^^^^^^^^^Create Links")
				oPgon.CreatePolylines()
			End If

			'End If
		Next
   End Sub
   Public Sub InfoToExcel()
      For Each oPgon As tmPolygon In MyBase.Values
         oPgon.InfoToExcel()
      Next
   End Sub
	Public Sub DrawEdges()

		For Each oPgon As tmPolygon In MyBase.Values
			'If oPgon.ID = 50 Or oPgon.ID = 54 Then
			DMAcadExt.AcadDocument.WriteMessage("***************** P G O N  ID = " & oPgon.ID.ToString() & "----" & oPgon.ExteriorHandle.ToString() & "; " & "Cnt=" & CStr(MyBase.Count) & " *****************" & CStr(MyBase.Count))

			If True Or (tmPolygon.PgonTestA = 0 OrElse oPgon.ID = tmPolygon.PgonTestA) Then
				If oPgon.ID < 99999 Then
					oPgon.Complete()
					oPgon.DrawEdges()
				End If
				
			End If

			'End If
		Next
	End Sub

End Class
