Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Public Class MovingLink
	Public Enum enStatus
		Start
		Polygons
		Executed
	End Enum
	Private mcolLine As ObjectIdCollection
	Private mtMovingLineAcObjID As ObjectId
	Private moShrinkPolygon As ShrinkPolygon
	Private moShrinkPgonJig As ShrinkPgonJig
	Private moPgonJig As PgonJig
	Private mbSelectPolygonsOK As Boolean
	Private miNumber As Integer
	Private miStatus As enStatus = enStatus.Start
	Dim moParcel As UD_Parcel
	Dim moNeighborParcel As UD_Parcel
	Public Sub SelectPolygons()
		Dim oPromptResult As PromptResult


		Dim sParcelTopoName As String = TopoManager.TopoDefs.Item(New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)).Name

		Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		'	DMAcadExt.AcadDocument.WriteMessage("###036 " & CStr(oParcelTopology Is Nothing))
		If oParcelTopology IsNot Nothing Then
			moShrinkPgonJig = New ShrinkPgonJig(oParcelTopology)
			oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(moShrinkPgonJig)
			If oPromptResult.Status = PromptStatus.OK Then
				DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
				DMAcadExt.AcadTransaction.Start()
				DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
				moShrinkPolygon = moShrinkPgonJig.GetEntity()

				moShrinkPolygon.AddToDatabase()

				DMAcadExt.AcadTransaction.CloseModelSpace()

				DMAcadExt.AcadTransaction.Terminate()
				DMAcadExt.AcadDocument.Unlock()


				moPgonJig = New PgonJig(oParcelTopology)
				moPgonJig.JigStatus = enPgonJigStatus.SelectNeighborPgon
				moPgonJig.Parcel = moShrinkPgonJig.Parcel
				oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(moPgonJig)

				If oPromptResult.Status = PromptStatus.OK Then
					DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
					DMAcadExt.AcadTransaction.Start()
					DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
					mtMovingLineAcObjID = DMAcadExt.AcadTransaction.AppendEntity(moPgonJig.GetEntity())
					DMAcadExt.AcadTransaction.CloseModelSpace()

					DMAcadExt.AcadTransaction.Terminate()
					DMAcadExt.AcadDocument.Unlock()

					moShrinkPgonJig.JigStatus = enPgonJigStatus.GetShrinkValue

					moShrinkPgonJig.SetSegmentIndecis(moPgonJig.IndexFrom, moPgonJig.IndexTo, moPgonJig.LayerName)
					mcolLine = moPgonJig.LineCollection
					DMAcadExt.AcadDocument.WriteMessage("Area0=" & ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))


					moParcel = moShrinkPgonJig.Parcel
					moNeighborParcel = moPgonJig.NeighborParcel
					miStatus = enStatus.Polygons
					Dim sMsgTest As String = ""

					If moParcel IsNot Nothing Then
						sMsgTest &= " " & moParcel.Name


						If moNeighborParcel IsNot Nothing Then
							sMsgTest &= " " & moNeighborParcel.Name
							mbSelectPolygonsOK = True
						Else
							sMsgTest &= " moNeighborParcel Is NOTHING 01_987"

						End If
					End If

					DMAcadExt.AcadDocument.WriteMessage("!!!!! " & sMsgTest)
					'	moShrinkPolygon.ShrinkTo(oParcel.Legalarea + oParcel.Tolerance)


					'	DMAcadExt.AcadDocument.WriteMessage("DestArea=" & ShrinkPolygon.DispArea(moParcel.LegalArea + moParcel.Tolerance))

					'	DMAcadExt.AcadDocument.WriteMessage("Area1=" & ShrinkPolygon.DispAcadArea(moShrinkPgonJig.Area))
					'		oShrinkPgonJig.Shrink(15.0)
					'	DMAcadExt.AcadDocument.WriteMessage("Area2=" & CStr(oShrinkPgonJig.Area))


					'	oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)
					'	DMAcadExt.AcadDocument.WriteMessage("!Out: " & oPromptResult.Status.ToString() & ":" & oShrinkPgonJig.PromptResult.StringResult & "!")

					'''''''''''''oPromptResult = Application.DocumentManager.MdiActiveDocument.Editor.Drag(oShrinkPgonJig)


				End If
				moPgonJig.Terminate()
				moPgonJig = Nothing
			End If

			oParcelTopology.Close()
			'	moShrinkPgonJig.Terminate()
			'	System.Windows.Forms.MessageBox.Show(oParcelTopology.Status.ToString(), "01_549")
			oParcelTopology = Nothing
		End If
	End Sub
	Public ReadOnly Property Parcel() As UD_Parcel
		Get
			If mbSelectPolygonsOK Then
				Return moParcel
			Else
				Return Nothing
			End If

		End Get
	End Property
	Public ReadOnly Property NeighborParcel() As UD_Parcel
		Get
			Return moNeighborParcel
		End Get
	End Property
	Public ReadOnly Property Status() As enStatus
		Get
			Return miStatus
		End Get
	End Property
	Public Property Number() As Integer
		Get
			Return miNumber
		End Get
		Set(ByVal iValue As Integer)
			miNumber = iValue
		End Set
	End Property
	Public Sub ShrinkTo(ByVal dDestAreaDun As Double)
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		moShrinkPolygon.ShrinkTo(dDestAreaDun)
		miStatus = enStatus.Executed
		'	moShrinkPolygon.EraseDBPolygon()
		'	moShrinkPolygon.AddToDatabase()
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Public Sub OK()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		If mcolLine IsNot Nothing Then
			DMAcadExt.AcadTransaction.EraseDBObjects(mcolLine)
		End If
		If moShrinkPolygon IsNot Nothing Then
			moShrinkPolygon.CreateMovingPolyline()
			moShrinkPolygon.EraseDBPolygon()
		End If
		
		DMAcadExt.AcadTransaction.EraseDBObject(mtMovingLineAcObjID)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Public Sub Terminate()
		DMAcadExt.AcadDocument.WriteMessage("###308 ")
		If moShrinkPgonJig IsNot Nothing Then
			moShrinkPgonJig.Terminate()
		End If
		If moPgonJig IsNot Nothing Then
			moPgonJig.Terminate()
		End If
	End Sub
End Class
