Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Class PgonSetCreator
	'	Inherits MPolygon
	Const msAcObjIDFieldName As String = "AcObjID"
	Const msEntityNoFieldName As String = "EntityNo"
	Const msEntityTypeFieldName As String = "Type"
	Const msStatusFieldName As String = "Status"
	Const msXminFieldName As String = "Xmin"
	Const msXmaxFieldName As String = "Xmax"
	Const msYminFieldName As String = "Ymin"
	Const msYmaxFieldName As String = "Ymax"
	Const msXFieldName As String = "X"
	Const msYFieldName As String = "Y"
	Const msNeigborListFieldName As String = "NeigborList"

	Private moLinks As System.Data.DataTable
	Private moCentroids As System.Data.DataTable
	Private mdicPolylines As IDictionary(Of ObjectId, Polyline)
	Private mdicMPolygons As IDictionary(Of ObjectId, MPolygon)
	Private mdicBlocks As IDictionary(Of ObjectId, BlockReference)

	Public Sub AddPolylines(dicPolylines As IDictionary(Of ObjectId, Polyline))
		mdicPolylines = dicPolylines
		zzFillLinkTable()
	End Sub
	Public Sub AddMPolygons(dicMPolygons As IDictionary(Of ObjectId, MPolygon))
		mdicMPolygons = dicMPolygons
		zzFillMPgonTable()
	End Sub
	Public Sub AddBlocks(dicBlocks As IDictionary(Of ObjectId, BlockReference))
		mdicBlocks = dicBlocks
		zzFillCentroidTable()
	End Sub
	Private Sub zzCreateLinkTable()
		moLinks = New System.Data.DataTable("Links")
		With moLinks.Columns
			'''''''''''.Add(msAcObjIDFieldName, GetType(ObjectId))
			.Add(msAcObjIDFieldName, GetType(System.Int64))
			.Add(msEntityNoFieldName, GetType(System.Int32))
			.Add(msStatusFieldName, GetType(System.Boolean))
			.Add(msXminFieldName, GetType(System.Double))
			.Add(msXmaxFieldName, GetType(System.Double))
			.Add(msYminFieldName, GetType(System.Double))
			.Add(msYmaxFieldName, GetType(System.Double))
			.Add(msNeigborListFieldName, GetType(System.String))
		End With
		moLinks.PrimaryKey = New System.Data.DataColumn() {moLinks.Columns.Item(0)}

	End Sub
	Private Sub zzFillLinkTable()
		Dim oNewRow As DataRow
		Dim oExtents3d As Extents3d
		Dim iRowNo As Integer = 0

		For Each oPolyline As Polyline In mdicPolylines.Values
			iRowNo += 1
			oExtents3d = oPolyline.GeometricExtents
			oNewRow = moLinks.NewRow()
			With oNewRow
				''''''''''''''		.Item(msAcObjIDFieldName) = oPolyline.ObjectId
				.Item(msAcObjIDFieldName) = oPolyline.ObjectId.OldIdPtr.ToInt64()

				.Item(msEntityNoFieldName) = iRowNo

				.Item(msStatusFieldName) = False
				.Item(msXminFieldName) = oExtents3d.MinPoint.X
				.Item(msXmaxFieldName) = oExtents3d.MaxPoint.X
				.Item(msYminFieldName) = oExtents3d.MinPoint.Y
				.Item(msYmaxFieldName) = oExtents3d.MaxPoint.Y


			End With
			moLinks.Rows.Add(oNewRow)
		Next
	End Sub
	Private Sub zzFillMPgonTable()
		Dim oNewRow As DataRow
		Dim oExtents3d As Extents3d
		Dim iRowNo As Integer = 0

		For Each oMPgon As MPolygon In mdicMPolygons.Values
			iRowNo += 1
			oExtents3d = oMPgon.GeometricExtents
			oNewRow = moLinks.NewRow()
			With oNewRow
				''''''''''''''		.Item(msAcObjIDFieldName) = oPolyline.ObjectId
				.Item(msAcObjIDFieldName) = oMPgon.ObjectId.OldIdPtr.ToInt64()

				.Item(msEntityNoFieldName) = iRowNo

				.Item(msStatusFieldName) = False
				.Item(msXminFieldName) = oExtents3d.MinPoint.X
				.Item(msXmaxFieldName) = oExtents3d.MaxPoint.X
				.Item(msYminFieldName) = oExtents3d.MinPoint.Y
				.Item(msYmaxFieldName) = oExtents3d.MaxPoint.Y


			End With
			moLinks.Rows.Add(oNewRow)
		Next
	End Sub
	Private Sub zzCreateCentroidTable()
		moCentroids = New System.Data.DataTable("Centroids")
		'	System.Windows.Forms.MessageBox.Show(CStr(moCentroids Is Nothing), "03_400X")
		With moCentroids.Columns
			.Add(msAcObjIDFieldName, GetType(System.Int64))
			.Add(msStatusFieldName, GetType(System.Boolean))
			.Add(msXFieldName, GetType(System.Double))
			.Add(msYFieldName, GetType(System.Double))

		End With
	End Sub
	Private Sub zzFillCentroidTable()
		Dim oNewRow As DataRow
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		'	System.Windows.Forms.MessageBox.Show(CStr(mdicBlocks.Count), "03_405")
		For Each oBlockRef As BlockReference In mdicBlocks.Values
			If oBlockRef IsNot Nothing Then
				Try
					tPoint = oBlockRef.Position
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf, "04_122")
				End Try
				oNewRow = moCentroids.NewRow()
				With oNewRow
					''	.Item(msAcObjIDFieldName) = oBlockRef.ObjectId
					.Item(msAcObjIDFieldName) = oBlockRef.ObjectId.OldIdPtr.ToInt64()
					.Item(msStatusFieldName) = False
					.Item(msXFieldName) = tPoint.X
					.Item(msYFieldName) = tPoint.Y
				End With
				moCentroids.Rows.Add(oNewRow)
			End If
		Next
		'	System.Windows.Forms.MessageBox.Show(CStr(mdicBlocks.Count) & ":" & CStr(moCentroids.Rows.Count), "03_515")
	End Sub
	 
	Public Sub Calculate()
		Const dToler As Double = 0.001
		Dim bExcludeCrossing As Boolean = True
		'	Dim iTest As Integer = 0
		Dim tExtents3d As Extents3d
		'	System.Windows.Forms.MessageBox.Show(CStr(moLinks.Rows.Count) & ":" & CStr(moLinks.Rows.Count), "03_657")
		Dim oLinkDataView As DataView = New DataView(moLinks)
		Dim oCentroidDataView As DataView = New DataView(moCentroids)
		Dim sRowFilter As String
		Dim oDataRow As DataRow
		Dim oNeigborDataRow As DataRow

		Dim oDataViewRow As DataRowView
		Dim oBaseMPolygon As MPolygon
		Dim oResMPolygon As MPolygon
		Dim iRowIndex As Integer = 0
		Dim iPlineNo As Integer = 0

		'	Dim oCheckMPolygon As MPolygon
		Dim oCheckPolyline As Polyline
		Dim tCheckAcObjID As ObjectId
		Dim tCheckNo As Integer

		Dim iNumMPolygonLoops As Integer
		Dim iDir0, iDir1 As Autodesk.AutoCAD.DatabaseServices.LoopDirection
		Dim iParent As Integer
		Dim oMPgonLoop0 As MPolygonLoop
		Dim oMPgonLoop1 As MPolygonLoop
		Dim bBalanceSuccess As Boolean
		Dim bCrossed0 As Boolean
		Dim bCrossed1 As Boolean
		Dim bLoopCrossing0 As Boolean
		Dim bLoopCrossing1 As Boolean
		Dim dArea0, dAreaEnd As Double
		Dim dPLineArea0, dPLineArea1 As Double
		Dim dPLineLen0 As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim iaLoopInd As IntegerCollection
		Dim oParcel As TopoManager.TPlanGraph.TplnParcel
		Dim oXDataParcel As DMAcadExt.TplnXDataParcel
		'	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp("CPTopo_Parcels")
		Dim colPoints As Autodesk.AutoCAD.Geometry.Point3dCollection = New Autodesk.AutoCAD.Geometry.Point3dCollection()

		'Dim tAcObjIDKey(0) As ObjectId
		Dim tAcObjIDKey(0) As Int64
		Dim bMpgon As Boolean
		Dim lAcObjID As Long
		Dim tIntPtr As System.IntPtr
		Dim oCentroidDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		For Each oPolyline As Polyline In mdicPolylines.Values
			oDataRow = moLinks.Rows.Item(iRowIndex)
			iPlineNo = DirectCast(oDataRow.Item(msEntityNoFieldName), Integer)
			oDataRow.Item(msStatusFieldName) = True
			tExtents3d = oPolyline.GeometricExtents
			sRowFilter = "(" & msStatusFieldName & " = False) AND (" & msXmaxFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & msXminFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & msYmaxFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & msYminFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
			oLinkDataView.RowFilter = sRowFilter
			''''''''''''''''''''''''''''''''''''''	oDataView.RowStateFilter = DataViewRowState.Unchanged

			oResMPolygon = New MPolygon()

			Try
				oResMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception

				DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & "Res1 Err=" & oAcadEx.ErrorStatus.ToString())
			End Try



			If iRowIndex < 1000 Then
				For iIndex As Integer = 0 To oLinkDataView.Count - 1
					bMpgon = False
					bBalanceSuccess = False
					dPLineArea0 = oPolyline.Area
					dPLineLen0 = oPolyline.Length
					oDataViewRow = oLinkDataView.Item(iIndex)
					''''''''''''	tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId)
					lAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), Long)
					tCheckAcObjID = New ObjectId(New System.IntPtr(lAcObjID))


					tCheckNo = DirectCast(oDataViewRow.Item(msEntityNoFieldName), Integer)
					oCheckPolyline = mdicPolylines.Item(tCheckAcObjID)
					dPLineArea1 = oCheckPolyline.Area
					dPLineLen0 = oCheckPolyline.Length

					oBaseMPolygon = New MPolygon()

					Try
						oBaseMPolygon.AppendLoopFromBoundary(oPolyline, bExcludeCrossing, dToler)
						oBaseMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
						dArea0 = oBaseMPolygon.Area

						oBaseMPolygon.BalanceTree()
						bBalanceSuccess = True
					Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
						bBalanceSuccess = False
						DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " BP Err=" & oAcadEx.ErrorStatus.ToString() & ";" & oPolyline.Handle.ToString() & ":" & oCheckPolyline.Handle.ToString())

					End Try
					If bBalanceSuccess AndAlso oBaseMPolygon.IsBalanced Then
						dAreaEnd = oBaseMPolygon.Area
						iNumMPolygonLoops = oBaseMPolygon.NumMPolygonLoops()
						If iNumMPolygonLoops = 2 Then
							oMPgonLoop0 = oBaseMPolygon.GetMPolygonLoopAt(0)
							bCrossed0 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop0, dToler)
							If bCrossed0 Then
								oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
							End If

							oMPgonLoop1 = oBaseMPolygon.GetMPolygonLoopAt(1)
							bCrossed1 = oBaseMPolygon.LoopCrossesMPolygon(oMPgonLoop1, dToler)
							If bCrossed0 Then
								oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
							End If
							If bCrossed0 OrElse bCrossed1 Then
								DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " Crossed = " & bCrossed0.ToString() & " : " & bCrossed1.ToString() & " : " & oPolyline.Handle.ToString() & " : " & oCheckPolyline.Handle.ToString() & vbCrLf)
							End If
							bLoopCrossing0 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop1, dToler)
							bLoopCrossing1 = oMPgonLoop0.LoopCrossesItself(oMPgonLoop0, dToler)


							iDir0 = oBaseMPolygon.GetLoopDirection(0)
							iDir1 = oBaseMPolygon.GetLoopDirection(1)


							If iDir0 = LoopDirection.Interior Then

								oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Red)
								oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
								iParent = oBaseMPolygon.GetParentLoop(0)
								DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " A = " & iDir0.ToString() & " : " & FormatNumber(oCheckPolyline.Length, 3) & " : " & CStr(iParent))
								zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(1), Drawing.Color.Cyan)
								If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
									oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
									oResMPolygon.BalanceTree()
								End If
							End If
							If iDir1 = LoopDirection.Interior Then
								oCheckPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Magenta)
								oPolyline.Color = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.Brown)
								iParent = oBaseMPolygon.GetParentLoop(1)
								zzDispMPgonLoop(oBaseMPolygon.GetMPolygonLoopAt(0), Drawing.Color.DeepPink)
								DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " B = " & iDir1.ToString() & " : " & FormatNumber(oPolyline.Length, 3) & " : " & CStr(iParent) & vbCrLf)
								If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
									oResMPolygon.AppendLoopFromBoundary(oCheckPolyline, bExcludeCrossing, dToler)
									oResMPolygon.BalanceTree()
								End If
							End If
							If iDir0 = LoopDirection.Interior OrElse iDir1 = LoopDirection.Interior Then
								'	oEditor.WriteMessage(CStr(iTest) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " : " & FormatNumber(dArea0, 3) & " : " & FormatNumber(dArea1, 3) & " : " & FormatNumber(dAreaEnd, 3) & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1 & vbCrLf)
								DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " PL0 = " & FormatNumber(dPLineArea0, 3) & " PL1 = " & FormatNumber(dPLineArea1, 3) & " MP: " & FormatNumber(dAreaEnd, 3) & vbCrLf)
								DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & "-IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & " Perim = " & FormatNumber(oBaseMPolygon.Perimeter, 3) & ":" & CStr(oBaseMPolygon.IncludesTouchingLoops(dToler)) & vbCrLf) ' & " LoopCrossing: " & bLoopCrossing0 & ":" & bLoopCrossing1
							Else
								If oBaseMPolygon.IncludesTouchingLoops(dToler) Then
									zzAddNeigbor(tCheckNo, oDataRow)
									tIntPtr = oCheckPolyline.ObjectId.OldIdPtr
									lAcObjID = tIntPtr.ToInt64()
									tAcObjIDKey(0) = lAcObjID
									oNeigborDataRow = moLinks.Rows.Find(lAcObjID)
									'oEditor.WriteMessage(CStr(iRowIndex) & "**" & tAcObjIDKey(0).ToString() & CStr(oNeigborDataRow Is Nothing))
									If oNeigborDataRow IsNot Nothing Then
										zzAddNeigbor(iPlineNo, oNeigborDataRow)
									End If

								End If
							End If
						End If
						'	oDataViewRow.Row.SetModified()
						'jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj
					Else
						oPolyline.IntersectWith(oCheckPolyline, Intersect.OnBothOperands, colPoints, New IntPtr(0), New IntPtr(0))
						If colPoints IsNot Nothing Then
							Dim oPoint As DMAcadExt.TPlnPoint
							DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " IsBalanced = " & CStr(oBaseMPolygon.IsBalanced) & CStr(colPoints.Count))
							For iPointIndex As Integer = 0 To colPoints.Count - 1
								oPoint = New DMAcadExt.TPlnPoint(colPoints.Item(iPointIndex))
								DMAcadExt.AcadDocument.WriteMessage("P= " & oPoint.Coordinates2d)
							Next
						End If
					End If
				Next
			End If
			sRowFilter = "(" & msStatusFieldName & " = False) AND (" & msXFieldName & " >" & (tExtents3d.MinPoint.X).ToString & ") AND (" & msXFieldName & " <" & (tExtents3d.MaxPoint.X).ToString & ") AND (" & msYFieldName & ">" & (tExtents3d.MinPoint.Y).ToString & ") AND (" & msYFieldName & "<" & (tExtents3d.MaxPoint.Y).ToString & ")"
			'	System.Windows.Forms.MessageBox.Show(CStr(sRowFilter), "03_167")
			oCentroidDataView.RowFilter = sRowFilter
			'	System.Windows.Forms.MessageBox.Show(CStr(oCentroidDataView.Count), "03_168")
			'	oEditor.WriteMessage(CStr(iRowIndex) & " CentroidCount=" & oCentroidDataView.Count.ToString() & vbCrLf)
			tCheckAcObjID = ObjectId.Null
			For iIndex As Integer = 0 To oCentroidDataView.Count - 1
				oDataViewRow = oCentroidDataView.Item(iIndex)
				tPoint = New Point3d(DirectCast(oDataViewRow.Item(msXFieldName), System.Double), DirectCast(oDataViewRow.Item(msYFieldName), System.Double), 0.0)



				'tCheckAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), ObjectId) mm
				iaLoopInd = oResMPolygon.IsPointInsideMPolygon(tPoint, dToler)
				If iaLoopInd Is Nothing Then
				ElseIf (iaLoopInd.Count = 1) Then '(iaLoopInd.Count = 1 AndAlso iaLoopInd.Item(0) <> 0) OrElse iaLoopInd.Count > 1 Then
					'	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " LoopInd.Count=" & iaLoopInd.Count & " #" & CStr(iaLoopInd.Item(0)) & vbCrLf)

					lAcObjID = DirectCast(oDataViewRow.Item(msAcObjIDFieldName), Long)
					tCheckAcObjID = New ObjectId(New System.IntPtr(lAcObjID))
					Exit For
				Else
					'	oEditor.WriteMessage(CStr(iRowIndex) & ":" & CStr(iIndex) & " +*LoopInd.Count=" & iaLoopInd.Count & vbCrLf)
				End If
			Next

			DMAcadExt.AcadTransaction.AppendEntity(oResMPolygon)

			'''''''''	Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaBlockAttribIndex)
			'System.Windows.Forms.MessageBox.Show(CStr(oResMPolygon Is Nothing) & ":" & oResMPolygon.GetType().ToString() & ":" & tCheckAcObjID.ToString(), "03_210")
			If iRowIndex < 10 Then
				'	oEditor.WriteMessage(CStr(iRowIndex) & " tCheckAcObjID=" & tCheckAcObjID.ToString() & vbCrLf)
			End If
		
			If Not tCheckAcObjID.IsNull Then
				oParcel = New TopoManager.TPlanGraph.TplnParcel(oResMPolygon, tCheckAcObjID)
				oXDataParcel = New DMAcadExt.TplnXDataParcel()
				oXDataParcel.ImpID = iPlineNo
				oXDataParcel.ID = iPlineNo
				oXDataParcel.XDataType = 1	' iRingType
				oXDataParcel.Block = oParcel.Block
				oXDataParcel.BlockAdd = 0
				If oParcel.Name IsNot Nothing Then
					oXDataParcel.Name = oParcel.Name
				End If

				oXDataParcel.LegalArea = oParcel.LegalArea
				oXDataParcel.Status = 1	' tParcelData.Status

				oXDataParcel.AcadArea = Math.Abs(oResMPolygon.Area)
				oXDataParcel.Perimeter = oResMPolygon.Perimeter
				If Not IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
					oXDataParcel.NeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String)

					oResMPolygon.XData = oXDataParcel.GetResBuffer()

				End If
				oCentroidDBObject = DMAcadExt.AcadTransaction.GetDBObject(tCheckAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				oCentroidDBObject.XData = oXDataParcel.GetResBuffer()
			Else
				DMAcadExt.AcadDocument.WriteMessage(CStr(iRowIndex) & " ??LoopInd.Count=" & oCentroidDataView.Count)
			End If
			iRowIndex += 1

		Next
	End Sub
	Private Sub zzAddNeigborAAA(tAcObjID As ObjectId, ByRef oDataRow As DataRow)
		Dim sNeigborList As String
		If IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
			sNeigborList = String.Empty
		Else
			sNeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String) & ","

		End If
		oDataRow.Item(msNeigborListFieldName) = sNeigborList & tAcObjID.ToString()
	End Sub
	Private Sub zzAddNeigbor(iID As Integer, ByRef oDataRow As DataRow)
		Dim sNeigborList As String
		If IsDBNull(oDataRow.Item(msNeigborListFieldName)) Then
			sNeigborList = String.Empty
		Else
			sNeigborList = DirectCast(oDataRow.Item(msNeigborListFieldName), String) & ","

		End If
		oDataRow.Item(msNeigborListFieldName) = sNeigborList & iID.ToString()
	End Sub
	Private Sub zzDispMPgonLoop(oMPgonLoop As MPolygonLoop, tColor As System.Drawing.Color)
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d

		Dim oCircle As Autodesk.AutoCAD.DatabaseServices.Circle
		Dim tNormal As Autodesk.AutoCAD.Geometry.Vector3d = New Autodesk.AutoCAD.Geometry.Vector3d(0, 0, 1)
		For iIndex As Integer = 0 To oMPgonLoop.Count - 1
			tPoint = oMPgonLoop.Item(iIndex).Vertex
			tPoint3d = New Point3d(tPoint.X, tPoint.Y, 0.0)
			oCircle = New Autodesk.AutoCAD.DatabaseServices.Circle(tPoint3d, tNormal, 1.0)
			If tColor <> System.Drawing.Color.Empty Then
				oCircle.Color = Autodesk.AutoCAD.Colors.Color.FromColor(tColor)
			End If
			DMAcadExt.AcadTransaction.AppendEntity(oCircle)
		Next

	End Sub
	Public Sub New()
		zzCreateLinkTable()
		zzCreateCentroidTable()
	End Sub
End Class
