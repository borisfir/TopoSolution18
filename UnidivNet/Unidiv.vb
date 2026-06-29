Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Public Class Unidiv
	Private Shared mdicParcels As Dictionary(Of Integer, UD_Parcel)
	Private Shared mcolLine As ObjectIdCollection
	Private Shared moShrinkPolygon As ShrinkPolygon
	Private Shared moShrinkPgonJig As ShrinkPgonJig
	Public Shared Sub Calculate()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)
		LoadParcels()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.CloseLog()
	End Sub
   Public Shared Function LoadFragments() As Boolean
      'DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
      Dim sFragmentTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Fragments)).Name
      Dim oFragmentTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sFragmentTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)
      DMCommon.Debug.MsgBox("10-109", oFragmentTopology IsNot Nothing, sFragmentTopoName)
      If oFragmentTopology IsNot Nothing Then
         Dim oFragment As FragmentPolygon
         Dim colPolygons As PolygonCollection = oFragmentTopology.GetPolygons()



         For Each oPolygon As Polygon In colPolygons
            oFragment = New FragmentPolygon(oPolygon)
            If oFragment.Correct Then  'TEMP

            End If
            oPolygon.Dispose()
            oPolygon = Nothing
            'oParcel.Terminate()
            oFragment = Nothing
         Next


         'colPolygons.Clear()
         '	colPolygons.Dispose()
         colPolygons = Nothing
         oFragmentTopology.Close()
         '	oParcelTopology.Dispose()
         oFragmentTopology = Nothing

         Return True
      Else
         System.Windows.Forms.MessageBox.Show("fRAGMENT Topology Is Nothing")
         Return False
      End If
   End Function
   Public Shared Function LoadParcels() As Boolean
      'DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
      Dim sParcelTopoName As String = "Parcels" 'TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
      Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If oParcelTopology IsNot Nothing Then
         Dim oParcel As UD_Parcel
         Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
         UD_Parcel.Initialize()
         UD_Parcel.CreateMainDataTable()
         UD_Parcel.CreateAdjoiningParcelsTable()


         mdicParcels = New Dictionary(Of Integer, UD_Parcel)

			If True Then
            For Each oPolygon As Polygon In colPolygons
               oParcel = New UD_Parcel(oPolygon)
               oParcel.Calc()
               If oParcel.Correct Then 'TEMP
                  mdicParcels.Add(oParcel.TopoID, oParcel)
                  oParcel.AddDataToMainTable()
               End If
               '		DMAcadExt.AcadDocument.WriteDebugMessage("N Count=" & CStr(oParcel.Neighbors.Count))
               oPolygon.Dispose()
               oPolygon = Nothing
               'oParcel.Terminate()
               oParcel = Nothing
            Next
         End If


         '	DMAcadExt.AcadDocument.WriteMessage("###300")
         'colPolygons.Clear()
         '	colPolygons.Dispose()
         colPolygons = Nothing
         oParcelTopology.Close()
         '	oParcelTopology.Dispose()
         oParcelTopology = Nothing

         Return True
      Else
         System.Windows.Forms.MessageBox.Show("Parcel Topology Is Nothing")
         Return False
      End If
   End Function
   Public Shared Function LoadParcels_140517() As Boolean
      'DMAcadExt.AppMessages.AddMessage(True, oPointList.Item(iIndex), "", "Missing Centroid", False)
      Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
      Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
      If oParcelTopology IsNot Nothing Then
         Dim oParcel As UD_Parcel
         Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
         UD_Parcel.Initialize()
         UD_Parcel.CreateMainDataTable()
         UD_Parcel.CreateAdjoiningParcelsTable()


         mdicParcels = New Dictionary(Of Integer, UD_Parcel)

         If True Then
            For Each oPolygon As Polygon In colPolygons
               oParcel = New UD_Parcel(oPolygon)
               oParcel.Calc()
               If oParcel.Correct Then 'TEMP
                  mdicParcels.Add(oParcel.TopoID, oParcel)
                  oParcel.AddDataToMainTable()
               End If
               '		DMAcadExt.AcadDocument.WriteDebugMessage("N Count=" & CStr(oParcel.Neighbors.Count))
               oPolygon.Dispose()
               oPolygon = Nothing
               'oParcel.Terminate()
               oParcel = Nothing
            Next
         End If
         '	DMAcadExt.AcadDocument.WriteMessage("###200")
         For Each oParcel In mdicParcels.Values
            '	DMAcadExt.AcadDocument.WriteMessage("N Count=" & CStr(oParcel.Neighbors.Count))
            If oParcel.Correct Then
               oParcel.AddDataToAdjoiningParcelsTable()
            End If
         Next
         '	DMAcadExt.AcadDocument.WriteMessage("###300")
         'colPolygons.Clear()
         '	colPolygons.Dispose()
         colPolygons = Nothing
         oParcelTopology.Close()
         '	oParcelTopology.Dispose()
         oParcelTopology = Nothing

         Return True
      Else
         System.Windows.Forms.MessageBox.Show("Parcel Topology Is Nothing")
         Return False
      End If
   End Function
	Public Shared Function TryGetParcel(ByVal iTopoID As Integer, ByRef oParcel As UD_Parcel) As Boolean
		Return mdicParcels.TryGetValue(iTopoID, oParcel)
	End Function
	Public Shared ReadOnly Property Parcels() As Dictionary(Of Integer, UD_Parcel)
		Get
			Return mdicParcels
		End Get
	End Property
	Public Shared Sub Start1()
		Dim oPromptResult As PromptResult
		Dim oPgonJig As PgonJig
		Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		If oParcelTopology IsNot Nothing Then
			oPgonJig = New PgonJig(oParcelTopology)
			oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)
			If oPromptResult.Status = PromptStatus.OK Then
				oPgonJig.JigStatus = enPgonJigStatus.SelectNeighborPgon
				oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)
			End If
			oPgonJig.Terminate()
			oPgonJig = Nothing
			oParcelTopology.Close()
			System.Windows.Forms.MessageBox.Show(oParcelTopology.Status.ToString(), "01_549")
			oParcelTopology = Nothing
		End If

	End Sub
	Public Shared Sub EraseLine()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		DMAcadExt.AcadTransaction.EraseDBObjects(mcolLine)
		moShrinkPolygon.CreateMovingPolyline()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
	End Sub
	Public Shared Sub NatA()
	 
		Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		Dim oBlockRef As BlockReference
		Dim tAcadPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim oPgon As Polygon
		Dim oParcel As UD_Parcel = Nothing
		Dim iBlockNo As Integer
		Dim iBlockRefCounter As Integer = 0
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs("PCLS003")
		For Each tBlockAcObjId As ObjectId In colBlockRefs
			iBlockRefCounter += 1
			iBlockNo = 0
			oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			tAcadPoint = oBlockRef.Position
			oPgon = oParcelTopology.FindPolygon(tAcadPoint)
			If oPgon IsNot Nothing Then
				If TryGetParcel(oPgon.ID, oParcel) Then
					iBlockNo = oParcel.BlockNo
				End If
			End If
			
			If iBlockNo = 0 Then
				DMAcadExt.AcadDocument.WriteMessage(CStr(iBlockRefCounter) & ": " & CStr(iBlockNo) & " - " & DMAcadExt.TPlnPoint.DispPoint(tAcadPoint))
			End If

		Next
		oParcelTopology.Close()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()

		oParcelTopology = Nothing


	End Sub
	Public Shared Sub Start()
		Dim oPromptResult As PromptResult
		Dim oPgonJig As PgonJig
		Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

		If oParcelTopology IsNot Nothing Then
			moShrinkPgonJig = New ShrinkPgonJig(oParcelTopology)
			oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(moShrinkPgonJig)
			If oPromptResult.Status = PromptStatus.OK Then
				oPgonJig = New PgonJig(oParcelTopology)
				oPgonJig.JigStatus = enPgonJigStatus.SelectNeighborPgon
				oPgonJig.Parcel = moShrinkPgonJig.Parcel
				oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)

				If oPromptResult.Status = PromptStatus.OK Then

					moShrinkPgonJig.JigStatus = enPgonJigStatus.GetShrinkValue
					moShrinkPgonJig.SetSegmentIndecis(oPgonJig.IndexFrom, oPgonJig.IndexTo, oPgonJig.LayerName)
					mcolLine = oPgonJig.LineCollection
					DMAcadExt.AcadDocument.WriteMessage("Area0=" & ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))

					Dim oParcel As UD_Parcel

					moShrinkPolygon = moShrinkPgonJig.GetEntity()
					oParcel = moShrinkPgonJig.Parcel
					moShrinkPolygon.ShrinkTo(oParcel.LegalArea + oParcel.Tolerance)

					DMAcadExt.AcadDocument.WriteMessage("DestArea=" & ShrinkPolygon.DispArea(oParcel.LegalArea + oParcel.Tolerance))
					DMAcadExt.AcadDocument.WriteMessage("Area1=" & ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))
					'		oShrinkPgonJig.Shrink(15.0)
					'	DMAcadExt.AcadDocument.WriteMessage("Area2=" & CStr(oShrinkPgonJig.Area))
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
					moShrinkPgonJig.GetEntity().AddToDatabase()

					DMAcadExt.AcadTransaction.CloseModelSpace()

					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()


					'	oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)
					'	DMAcadExt.AcadDocument.WriteMessage("!Out: " & oPromptResult.Status.ToString() & ":" & oShrinkPgonJig.PromptResult.StringResult & "!")

					'''''''''''''oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)


				End If
				oPgonJig.Terminate()
				oPgonJig = Nothing
			End If

			oParcelTopology.Close()
			'	moShrinkPgonJig.Terminate()
			System.Windows.Forms.MessageBox.Show(oParcelTopology.Status.ToString(), "01_549")
			oParcelTopology = Nothing
		End If
	End Sub
	Public Shared Sub Start2()
		Dim oPromptResult As PromptResult
		Dim oShrinkPgonJig As ShrinkPgonJig
		Dim oPgonJig As PgonJig
		Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name
		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

		If oParcelTopology IsNot Nothing Then
			oShrinkPgonJig = New ShrinkPgonJig(oParcelTopology)
			oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)
			If oPromptResult.Status = PromptStatus.OK Then
				oPgonJig = New PgonJig(oParcelTopology)
				oPgonJig.JigStatus = enPgonJigStatus.SelectNeighborPgon
				oPgonJig.Parcel = oShrinkPgonJig.Parcel
				oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oPgonJig)

				If oPromptResult.Status = PromptStatus.OK Then

					oShrinkPgonJig.JigStatus = enPgonJigStatus.GetShrinkValue
					oShrinkPgonJig.SetSegmentIndecis(oPgonJig.IndexFrom, oPgonJig.IndexTo, oPgonJig.LayerName)
					DMAcadExt.AcadDocument.WriteMessage("Area0=" & ShrinkPolygon.DispAcadArea(oShrinkPgonJig.Area))
					oShrinkPgonJig.Shrink(8.0)
					DMAcadExt.AcadDocument.WriteMessage("Area1=" & CStr(oShrinkPgonJig.Area))
					oShrinkPgonJig.Shrink(15.0)
					DMAcadExt.AcadDocument.WriteMessage("Area2=" & CStr(oShrinkPgonJig.Area))
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
					oShrinkPgonJig.GetEntity().AddToDatabase()
					DMAcadExt.AcadTransaction.CloseModelSpace()

					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()
					'	oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)
					'	DMAcadExt.AcadDocument.WriteMessage("!Out: " & oPromptResult.Status.ToString() & ":" & oShrinkPgonJig.PromptResult.StringResult & "!")

					'''''''''''''oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)


				End If
				oPgonJig.Terminate()
				oPgonJig = Nothing
			End If

			oParcelTopology.Close()
			oShrinkPgonJig.Terminate()
			System.Windows.Forms.MessageBox.Show(oParcelTopology.Status.ToString(), "01_549")
			oParcelTopology = Nothing
		End If
	End Sub
End Class
