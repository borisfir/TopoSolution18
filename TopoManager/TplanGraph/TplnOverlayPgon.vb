Option Explicit On
Option Strict On
Namespace TPlanGraph

   Public Class TplnOverlayPgon
      Inherits TPlanGraph.TplnTopoPgon

      Public Const msExproTypeIDFieldName As String = "ExproTypeID"
      Public Const msExproTypeNameFieldName As String = "ExproTypeName"
      Public Const msExistingRoadAreaFieldName As String = "ExistingRoadArea"
      Public Const msOldDeclarationAreaFieldName As String = "OldDeclarationArea"
      Public Const msNewDeclarationAreaFieldName As String = "NewDeclarationArea"

      Public Const msDecNoFieldName As String = "DecNo"
      Public Const msDecDateFieldName As String = "DecDate"
      Public Const msBlockFieldName As String = "DecDate"

		Private miFeatureID As Integer
		Private miSourceID As Integer
		Private miOverlayID As Integer
		Private miOverlayID_Add As Integer

		Protected miLotTopoID As Integer

      Protected miParcelTopoID As Integer
      Private miExproTopoID As Integer
		Private miLotGroupID As Integer
		Private miLanduseID As Integer
		Private miBlockNo As Integer
		Private miBlockAddNo As Integer




		Private mdLotAreaPcnt As Double
      Private mdParcelAreaPcnt As Double
      Private mdCalcArea As Double = 0
      Private miTopoPurpose As DMAcadExt.enTopoPurpose
      Private miOverlayMethod As DMAcadExt.enOverlayMethod
      Private mbLotOut As Boolean
      Protected Shared moMainDataTable As System.Data.DataTable
      Public Shared Sub CreateMainDataTable()
         moMainDataTable = New System.Data.DataTable("OverlayPgons")
         Dim oOrder As System.Type
         Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)

         oOrder = GetType(System.Int32)

         With moMainDataTable.Columns
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.ParcelNameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))

				.Add(msExproTypeIDFieldName, GetType(System.Int32))
            .Add(msExproTypeNameFieldName, GetType(System.String))

            .Add(TopoReader.msAreaFldName, GetType(System.Double))
            .Add(msExistingRoadAreaFieldName, GetType(System.Double))
            .Add(msOldDeclarationAreaFieldName, GetType(System.Double))
            .Add(msNewDeclarationAreaFieldName, GetType(System.Double))
            .Add(msDecNoFieldName, GetType(System.String))
            .Add(msDecDateFieldName, GetType(System.DateTime))
				.Add(TplnLot.NameFieldName, GetType(System.String))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
            '19 15
         End With
      End Sub
      Public Shared ReadOnly Property MainView() As System.Data.DataView
         Get
				Dim sSort As String = TplnParcel.BlockFieldName & "," & TplnParcel.ParcelNameFieldName & "," & TplnParcel.LanduseIDFieldName
				If moMainDataTable IsNot Nothing Then
               Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
               oDataView.AllowEdit = False
               oDataView.AllowDelete = False
               oDataView.AllowNew = False
               Return oDataView
            Else
               Return Nothing
            End If

         End Get
      End Property
      Public Shared ReadOnly Property MainDataTable() As System.Data.DataTable
         Get
            Return moMainDataTable
         End Get
      End Property
      Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal tOverlayOD As OverlayOD)
         MyBase.New(oPolygon)
         DMAcadExt.AcadDocument.WriteMessage("AA " & iOverlayMethod.ToString() & ":" & iOverlayMethod.ToString() & ":" & tOverlayOD.ToString())
         miParcelTopoID = tOverlayOD.SourceID
         miLotTopoID = tOverlayOD.OverlayID
         mdLotAreaPcnt = tOverlayOD.OverlayAreaPcnt
         mdParcelAreaPcnt = tOverlayOD.SourceAreaPcnt
         miOverlayMethod = iOverlayMethod
         miTopoPurpose = iTopoPurpose
      End Sub
      Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLotTopoID As Integer, ByVal iParcelTopoID As Integer)
         MyBase.New(oPolygon)
         DMAcadExt.AcadDocument.WriteMessage("BB " & iOverlayMethod.ToString() & ":" & iOverlayMethod.ToString() & ":" & iLotTopoID.ToString())
         miLotTopoID = iLotTopoID
         miParcelTopoID = iParcelTopoID
         miOverlayMethod = iOverlayMethod
         miTopoPurpose = iTopoPurpose
      End Sub
      Public Sub New(ByRef oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline, ByVal iFeatureID As Integer, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLotTopoID As Integer, ByVal iParcelTopoID As Integer)
         MyBase.New(oPolygon, iFeatureID)
         DMAcadExt.AcadDocument.WriteMessage("CC" & iOverlayMethod.ToString() & ":" & iOverlayMethod.ToString() & ":" & iLotTopoID.ToString())
         miLotTopoID = iLotTopoID
         miParcelTopoID = iParcelTopoID
         miOverlayMethod = iOverlayMethod
         miTopoPurpose = iTopoPurpose
         dcolLines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         dcolLines.Add(oPolygon.ObjectId)
      End Sub

      Public Sub New(ByRef oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, dicLines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal iFeatureID As Integer, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLotTopoID As Integer, ByVal iParcelTopoID As Integer)
         MyBase.New(oPolygon, dicLines, iFeatureID)
         DMAcadExt.AcadDocument.WriteMessage("DD" & iOverlayMethod.ToString() & ":" & iOverlayMethod.ToString() & ":" & iLotTopoID.ToString())
         miLotTopoID = iLotTopoID
         miParcelTopoID = iParcelTopoID
         miOverlayMethod = iOverlayMethod
         miTopoPurpose = iTopoPurpose
         '	dcolLines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
         '	dcolLines.Add(oPolygon.ObjectId)
         Dim oExtents3d As Autodesk.AutoCAD.DatabaseServices.Extents3d = oPolygon.GeometricExtents

         MyBase.doBoundingBox = New DMAcadExt.TPlnBoundingBox(oExtents3d)
         Dim oPoint As DMAcadExt.TPlnPoint = doBoundingBox.GetCenterPoint()
         MyBase.ddCentroidX = oPoint.X
         MyBase.ddCentroidY = oPoint.Y
      End Sub

		Public Sub New(ByRef oPolygon As DMAcadExt.MPolygonOverlay, colLines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal iFeatureID As Integer, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			MyBase.New(oPolygon, colLines, oPolygon.FeatureID)
			miFeatureID = oPolygon.FeatureID
			miParcelTopoID = oPolygon.SourceID
			miSourceID = oPolygon.SourceID


			miLotTopoID = oPolygon.OverlayID
			miOverlayID = oPolygon.OverlayID


			miExproTopoID = oPolygon.OverlayID_Add
			miOverlayID_Add = oPolygon.OverlayID_Add

			miOverlayMethod = iOverlayMethod
			miTopoPurpose = iTopoPurpose
			MyBase.TopoID = miFeatureID
			dcolLines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			dcolLines = colLines
			'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!NewOverPgon", miLotTopoID, miParcelTopoID, iFeatureID, colLines.Count)
			'	dcolLines.Add(oPolygon.ObjectId)
		End Sub
      Public Overridable Sub AddDataToMainTable()
         Dim oNewRow As System.Data.DataRow
         If moMainDataTable IsNot Nothing Then
            Dim oLot As TplnLot = TplnProject.GetLot(DMAcadExt.enTopoPurpose.Approved, miLotTopoID)
            Dim oParcel As TplnParcel = TplnProject.GetParcel(miParcelTopoID, "AddDataToMainTable")

            Dim oExpro As TplnExpro = TplnProject.GetExpro(miExproTopoID)

            Try
               oNewRow = moMainDataTable.NewRow()
               With oNewRow
                  If miExproTopoID <> 0 Then
                     If oExpro IsNot Nothing Then
                        .Item(msExproTypeIDFieldName) = oExpro.ExproTypePrev
                        'oExpro.ExproTypeID
                        .Item(msExproTypeNameFieldName) = oExpro.ExproTypeNamePrev
                        Select Case oExpro.ExproTypePrev
                           Case enExproType.ExistingRoad
                              .Item(msExistingRoadAreaFieldName) = Math.Round(MyBase.AcadArea(False), 3)
                           Case enExproType.OldDeclaration
                              .Item(msOldDeclarationAreaFieldName) = Math.Round(MyBase.AcadArea(False), 3)
                           Case enExproType.NewDeclaration
                              .Item(msNewDeclarationAreaFieldName) = Math.Round(MyBase.AcadArea(False), 3)
                        End Select

                        .Item(msDecNoFieldName) = oExpro.DecNo
                        If oExpro.DecDate <> New Date() Then
                           .Item(msDecDateFieldName) = oExpro.DecDate
                        End If

                     Else
                        .Item(msExproTypeNameFieldName) = "<-->"
                     End If
                  End If
                  .Item(TopoReader.msAreaFldName) = MyBase.AcadArea(False)




                  If oLot IsNot Nothing Then
							.Item(TplnLot.NameFieldName) = oLot.Name

							.Item(TplnParcel.LanduseIDFieldName) = oLot.LanduseID

							.Item(TplnParcel.LanduseNameFieldName) = zzGetLanduseName(oLot.LanduseID)
						End If

                  If oParcel IsNot Nothing Then
							.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo

							.Item(TplnParcel.ParcelNameFieldName) = oParcel.Name
                  End If
                  .Item(TopoReader.msTopoIDFldName) = MyBase.TopoID

                  moMainDataTable.Rows.Add(oNewRow)


               End With
            Catch oEx As Exception

            End Try
         End If
      End Sub
      Protected Function zzGetLanduseName(iID As Integer) As String
         Dim sComText As String = "SELECT Name FROM dbo.Landuses_1F WHERE ID =" & CStr(iID)
         Dim oRes As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, System.Data.CommandType.Text)
         If oRes IsNot Nothing Then
            Return DirectCast(oRes, String)
         Else
            Return Nothing
         End If
      End Function

      Public Property LotTopoID() As Integer
         Get
            Return miLotTopoID
         End Get
         Set(ByVal iValue As Integer)
            miLotTopoID = iValue
         End Set
      End Property
      Public Property LotAreaPcnt() As Double
         Get
            Return mdLotAreaPcnt
         End Get
         Set(ByVal dValue As Double)
            mdLotAreaPcnt = dValue
         End Set
      End Property
      Public Property ParcelTopoID() As Integer
         Get
            Return miParcelTopoID
         End Get
         Set(ByVal iValue As Integer)
            miParcelTopoID = iValue
         End Set
      End Property
		Public Property ExproTopoID() As Integer
			Get
				Return miExproTopoID
			End Get
			Set(ByVal iValue As Integer)
				miExproTopoID = iValue
			End Set
		End Property
		Public Property OverlayID() As Integer
			Get
				Return miOverlayID
			End Get
			Set(ByVal iValue As Integer)
				miExproTopoID = iValue
			End Set
		End Property
		Public Property OverlayID_Add() As Integer
			Get
				Return miOverlayID_Add
			End Get
			Set(ByVal iValue As Integer)
				miExproTopoID = iValue
			End Set
		End Property


		Public Property LotOut() As Boolean
			Get
				Return mbLotOut
			End Get
			Set(ByVal bValue As Boolean)
				mbLotOut = bValue
			End Set
		End Property
		Public Property LanduseID() As Integer
			Get
				Return miLanduseID
			End Get
			Set(ByVal iValue As Integer)
				miLanduseID = iValue
			End Set
		End Property



		Public Property BlockNo() As Integer
			Get
				Return miBlockNo
			End Get
			Set(ByVal iValue As Integer)
				miBlockNo = iValue
			End Set
		End Property
		Public Property BlockAddNo() As Integer
			Get
				Return miBlockAddNo
			End Get
			Set(ByVal iValue As Integer)
				miBlockAddNo = iValue
			End Set
		End Property
		Public Property LotGroupID() As Integer
			Get
				Return miLotGroupID
			End Get
			Set(ByVal iValue As Integer)
				miLotGroupID = iValue
			End Set
		End Property

		Public Property ParcelAreaPcnt() As Double
         Get
            Return mdParcelAreaPcnt
         End Get
         Set(ByVal dValue As Double)
            mdParcelAreaPcnt = dValue
         End Set
      End Property
      Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
         Get
            Return miTopoPurpose
         End Get
         Set(ByVal iValue As DMAcadExt.enTopoPurpose)
            miTopoPurpose = iValue
         End Set
      End Property
      Public ReadOnly Property OverlayIndex() As DMAcadExt.enOverlayIndex
         Get
            Return UnionPgonArea.GetOverlayIndex(miOverlayMethod, Me.TopoPurpose)
         End Get
      End Property

      Public Property CalcArea() As Double
         Get
            If mdCalcArea = 0 Then
               Return ddAcadArea '* TplnUnionPgon.ScaleFactor
            Else
               Return mdCalcArea / TplnProject.UnitScaleFactor
            End If

         End Get
         Set(ByVal dValue As Double)
            mdCalcArea = dValue
         End Set
      End Property
      Public ReadOnly Property InPlanCalcArea() As Double
         Get
            If miLotTopoID = 0 Then  '''''Not Only
               Return 0.0
            ElseIf mbLotOut Then
               Return 0.0
            Else
               Return Me.CalcArea
            End If
         End Get
      End Property
      Public ReadOnly Property InPlanArea() As Double
         Get
            If miLotTopoID = 0 Then  '''''Not Only
               Return 0.0  '   Me.AcadArea(False)	 ''''''''''''''''1952 TEMP
            ElseIf mbLotOut Then
               Return 0.0
            Else
               Return Me.AcadArea(False)
            End If
         End Get
      End Property



      Public Overrides Sub Terminate()
         MyBase.OnTerminate()
      End Sub

      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
       
      End Property
      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property

   End Class
End Namespace


