Option Explicit On
Option Strict On
Friend Class bmPropUnit
	Implements System.IComparable(Of bmPropUnitKey)


	Public Shared mbWarehouseUnion As Boolean
	Private miPropID As Integer
	Private miBldNo As Integer
	Private miBldPart As Integer
	Private miBldEntrance As Integer
	Private miBldFloor As Integer
	Private mtPropUnitKey As bmPropUnitKey
	Private mdBalcony As Double
	Private miApartDescrID As Integer

	Private msCaption As String
	Private msApartDescr As String
	Private msFloorDescr As String

	Private mdMainArea As Double
	Private mdWarehouseArea As Double
	Private mdBalconyArea As Double

	Private mpgaApart As bmPolygonArray
	Private mpgaWarehouse As bmPolygonArray
	Private mpgaBalcony As bmPolygonArray
	Private miaApartBalconyTopoIDs() As Integer
	'Private mrsPrint As ADODB.Recordset
	Private mbNewPrintRecord As Boolean
	Private miPrintRowIndex As Integer
	Private mlRowCountAAA As Long

	Private miColor As Integer = 0




	Public Property PropID() As Integer
		Get
			Return miPropID
		End Get
		Set(ByVal Value As Integer)
			miPropID = Value
		End Set
	End Property

	Public ReadOnly Property BldNo() As Integer
		Get
			Return mtPropUnitKey.BldNo
		End Get
	End Property
	Public ReadOnly Property BldNoText() As String
		Get
			If mtPropUnitKey.BldNo = 0 Then
				Return String.Empty
			Else
				Return RomanNum.ToRoman(mtPropUnitKey.BldNo)
			End If

		End Get
	End Property
	Public ReadOnly Property BldPart() As Integer
		Get
			Return mtPropUnitKey.BldPart
		End Get

	End Property
	Public ReadOnly Property BldEntrance() As Integer
		Get
			Return mtPropUnitKey.BldEntr
		End Get
	End Property
	 
	Public ReadOnly Property BldFloor() As Integer
		Get
			Return mtPropUnitKey.BldFloor
		End Get

	End Property
	Public ReadOnly Property EntranceDescr() As String
		Get
			Return DMCommon.Hebrew.GetHebNumRev(mtPropUnitKey.BldEntr, True)
		End Get

	End Property
	Public ReadOnly Property ApartCount() As Integer
		Get
			Return mpgaApart.Count
		End Get
	End Property
	Public ReadOnly Property RepRowCount() As Integer
		Get
			Dim iWarehouseRowCount As Integer
			If mbWarehouseUnion Then
				iWarehouseRowCount = 1
			Else
				iWarehouseRowCount = Me.WarehouseCount
				If iWarehouseRowCount > 1 Then
					iWarehouseRowCount += 1
				End If
			End If
			'	DMAcadExt.AcadDocument.WriteMessage("! Math.Max(iWarehouseRowCount, Me.ApartBalconyCount)=" & CStr(Math.Max(iWarehouseRowCount, Me.ApartBalconyCount)))
			Return Math.Max(iWarehouseRowCount, Me.ApartBalconyCount)
		End Get
	End Property
	Public ReadOnly Property ApartBalconyCount() As Integer
		Get
			'	DMAcadExt.AcadDocument.WriteMessage("!ApartBalconyCount=" & CStr(Me.ApartCount + Me.BalconyCount))
			Return Me.ApartCount + Me.BalconyCount
		End Get
	End Property
	Public ReadOnly Property WarehouseCount() As Integer
		Get
			Return mpgaWarehouse.Count
		End Get
	End Property
	Public ReadOnly Property BalconyCount() As Integer
		Get
			Return mpgaBalcony.Count
		End Get
	End Property
	Public ReadOnly Property FloorDescr() As String
		Get
			Try
            Return GetFloorDescr2015(11, "Tag", mtPropUnitKey.BldFloor, mtPropUnitKey.BldSubFloor)
			Catch oEx As Exception
				Return "Error #129"
			End Try

		End Get
	 
	End Property
	Public ReadOnly Property Apart() As bmPolygonArray
		Get
			Return mpgaApart
		End Get
	End Property
	Public ReadOnly Property Balcony() As bmPolygonArray
		Get
			Return mpgaBalcony
		End Get
	End Property
	Public ReadOnly Property Warehouse() As bmPolygonArray
		Get
			Return mpgaWarehouse
		End Get
	End Property
	Public Sub AddPolygon2009(ByVal oPolygon As BamashPolygon)
		If mpgaApart.Count = 0 AndAlso mpgaBalcony.Count = 0 AndAlso mpgaWarehouse.Count = 0 Then
			mtPropUnitKey = oPolygon.PropUnitKey
		End If

		Select Case oPolygon.PropertyType
			Case enPropertyTypes.ApartType
				mpgaApart.AddPolygon2009(oPolygon)
			Case enPropertyTypes.BalconyType
				mpgaBalcony.AddPolygon2009(oPolygon)
			Case enPropertyTypes.WarehouseType
				mpgaWarehouse.AddPolygon2009(oPolygon)
		End Select
	End Sub


	Public Sub AddToPgonTable()
		For iIndex As Integer = 0 To mpgaApart.Count - 1
			mpgaApart(iIndex).AddDataToMainTable()
		Next
		For iIndex As Integer = 0 To mpgaBalcony.Count - 1
			mpgaBalcony(iIndex).AddDataToMainTable()
		Next
		For iIndex As Integer = 0 To mpgaWarehouse.Count - 1
			mpgaWarehouse(iIndex).AddDataToMainTable()
		Next
	End Sub
	Public Sub Paint()
		DMAcadExt.AcadDocument.WriteMessage("$$-" & CStr(miPropID) & " Key: " & mtPropUnitKey.ToString() & " Color: " & CStr(miColor))
		If miColor > 0 Then
			If miaApartBalconyTopoIDs Is Nothing Then
				For iIndex As Integer = 0 To mpgaApart.Count - 1
					mpgaApart(iIndex).Paint()
				Next
				For iIndex As Integer = 0 To mpgaBalcony.Count - 1
					mpgaBalcony(iIndex).Paint()
				Next
				For iIndex As Integer = 0 To mpgaWarehouse.Count - 1
					mpgaWarehouse(iIndex).Paint()
				Next
			Else
				zzPaintApartBalcony()
			End If

		End If
	End Sub

	Private Sub zzPaintApartBalcony()
		Dim sDissolveTopoName As String = TopoManager.TPlanGraph.TplnTopoPgon.PgonTopoName(miaApartBalconyTopoIDs(0))
		DMAcadExt.AcadDocument.WriteMessage("***Start of Dissolve # " & sDissolveTopoName & ":" & CStr(miaApartBalconyTopoIDs(0)))
		zzDispArray(miaApartBalconyTopoIDs, "***Start of Dissolve ")
		Dim oDissolveTopoModel As Autodesk.Gis.Map.Topology.TopologyModel
		Dim oColorPolygon As TopoManager.ColorPolygon
		Dim tColorScheme As DMAcadExt.ColorScheme = New DMAcadExt.ColorScheme(1.0)

		TopoManager.TopoCreator.DissolveMy(bmBamash.BamashTopo, miaApartBalconyTopoIDs, sDissolveTopoName)

		'	End If
		oDissolveTopoModel = TopoManager.TopoCreator.GetOpenedTopology(sDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
		If oDissolveTopoModel IsNot Nothing Then
			Dim colPolygons As Autodesk.Gis.Map.Topology.PolygonCollection = oDissolveTopoModel.GetPolygons()
			Dim bAllCentroidsExist As Boolean = True
			DMAcadExt.AcadDocument.WriteMessage("oDissolveTopoModel=: " & oDissolveTopoModel.Name & "; bmBamash.BufferOffset= " & CStr(bmBamash.BufferOffset))
			'	DMAcadExt.AcadDocument.WriteMessage("miColor=: " & CStr(miColor) & ":" & CStr(miColor))
			tColorScheme.AddBorderStrip(New DMAcadExt.DMColor(Convert.ToInt16(miColor)), bmBamash.BufferOffset)
			Dim tCentroObjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
			Try

				If colPolygons.Count = 1 Then
					Dim oPolygon As Autodesk.Gis.Map.Topology.Polygon = colPolygons.Item(0)
					Dim colRingsTest As Autodesk.Gis.Map.Topology.RingCollection = Nothing
					tCentroObjId = oPolygon.Entity
					If DMAcadExt.AcadTransaction.IsBlockRef(tCentroObjId) Then


						colRingsTest = oPolygon.GetBoundary()
						 
						Dim testStrip As DMAcadExt.ColorStrip = tColorScheme.Border.Strip(0)
						Dim tDMColor As DMAcadExt.DMColor
						tDMColor = testStrip.Color
						oColorPolygon = New TopoManager.ColorPolygon(oPolygon)
					 
						oColorPolygon.CurrentPgonTopoName = sDissolveTopoName
						DMAcadExt.AcadDocument.WriteMessage("zzPaintApartBalcony: " & CStr(tDMColor.AcadColorIndex) & ":" & CStr(testStrip.Width))
						oColorPolygon.Paint(DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True)	 ', sDissolveTopoName
					End If
				Else
					For Each oPolygon As Autodesk.Gis.Map.Topology.Polygon In colPolygons
						tCentroObjId = oPolygon.Entity

						If DMAcadExt.AcadTransaction.IsBlockRef(tCentroObjId) Then
							oColorPolygon = New TopoManager.ColorPolygon(oPolygon)
							oColorPolygon.Paint(DMAcadExt.PaintMethod.BorderByBuffer, tColorScheme, True)	  ', sDissolveTopoName
						End If

					Next
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "bmPropUnit - zzPaintApartBalcony")
			End Try
			If oDissolveTopoModel.Status <> Autodesk.Gis.Map.Topology.Status.Closed Then
				oDissolveTopoModel.Close()
			End If

			TopoManager.TopoCreator.DeleteTopology(sDissolveTopoName, True, False)

		Else
			DMAcadExt.AcadDocument.WriteMessage("Dissolve Topology '" & sDissolveTopoName & "' was not created" & CStr(miaApartBalconyTopoIDs(0)))
		End If
		DMAcadExt.AcadDocument.WriteMessage("---End of Dissolve #" & CStr(miaApartBalconyTopoIDs(0)))
	End Sub

	Private Sub zzDispArray(ByVal iaVal() As Integer, ByVal sTitle As String)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			If sOut.Length <> 0 Then sOut &= ","
			sOut &= Convert.ToString(iaVal(iIndex))
		Next

		DMAcadExt.AcadDocument.WriteMessage(sTitle & ":" & sOut)
	End Sub
	Public Sub New(ByVal iColorIndex As Integer)
		mpgaApart = New bmPolygonArray()
		mpgaWarehouse = New bmPolygonArray()
		mpgaBalcony = New bmPolygonArray()
	End Sub

	Public Sub Calculate()

		If mpgaApart.Count = 0 And mpgaBalcony.Count = 0 Then
			'OK

		ElseIf mpgaApart.Count = 0 And mpgaBalcony.Count > 0 Then
			'Error Error
		ElseIf mpgaApart.Count = 1 And mpgaBalcony.Count = 0 And mpgaWarehouse.Count = 0 Then

		Else	 ' Dissolve
			Dim iPolygonDissolveUB As Integer = mpgaApart.Count + mpgaBalcony.Count + mpgaWarehouse.Count - 1
			ReDim miaApartBalconyTopoIDs(iPolygonDissolveUB)
			mdMainArea = 0.0
			For iIndex As Integer = 0 To mpgaApart.Count - 1
				miaApartBalconyTopoIDs(iIndex) = mpgaApart.Item(iIndex).TopoID
			Next
			For iIndex As Integer = 0 To mpgaBalcony.Count - 1
				miaApartBalconyTopoIDs(mpgaApart.Count + iIndex) = mpgaBalcony.Item(iIndex).TopoID
			Next
			For iIndex As Integer = 0 To mpgaWarehouse.Count - 1
				miaApartBalconyTopoIDs(mpgaApart.Count + mpgaBalcony.Count + iIndex) = mpgaWarehouse.Item(iIndex).TopoID
			Next

		End If
		mdMainArea = mpgaApart.TotalArea

		mdBalconyArea = mpgaBalcony.TotalArea
		mdWarehouseArea = mpgaWarehouse.TotalArea
	End Sub
	Public Property NewPrintRecordAAA() As Boolean
		Get
			Return mbNewPrintRecord
		End Get
		Set(ByVal bValue As Boolean)
			mbNewPrintRecord = bValue
		End Set
	End Property

	Public Property PrintRowIndexAAA() As Integer
		Get
			Return miPrintRowIndex
		End Get
		Set(ByVal iValue As Integer)
			miPrintRowIndex = iValue
		End Set
	End Property



	Public ReadOnly Property WarehouseArea() As Double
		Get
			Return mdWarehouseArea
		End Get
	End Property
	Public ReadOnly Property MainArea() As Double
		Get
			Return mdMainArea
		End Get
	End Property
	Public ReadOnly Property TotalArea() As Double
		Get
			Return mdMainArea + mdWarehouseArea	'+ mdBalconyArea
		End Get
	End Property
	Public ReadOnly Property PropUnitKey() As bmPropUnitKey
		Get
			Return mtPropUnitKey
		End Get
	End Property
	Public Property Color() As Integer
		Get
			Return miColor
		End Get
		Set(ByVal iValue As Integer)
			miColor = iValue
			''	DMAcadExt.AcadDocument.WriteMessage("&&-&" & CStr(miPropID) & " SetColor: " & CStr(miColor))

			mpgaApart.SetColor(miColor)
			mpgaWarehouse.SetColor(miColor)
			mpgaBalcony.SetColor(miColor)
		End Set
	End Property

	Public Function CompareTo(ByVal tOtherPropUnitKey As bmPropUnitKey) As Integer Implements System.IComparable(Of bmPropUnitKey).CompareTo
		Return mtPropUnitKey.CompareTo(tOtherPropUnitKey)
	End Function
End Class
