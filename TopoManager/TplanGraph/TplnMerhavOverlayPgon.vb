Option Strict On
Option Explicit On

Namespace TPlanGraph

    Public Class TplnMerhavOverlayPgon
        Inherits TplnOverlayPgon

        Public Const msMerhavCodeFieldName As String = "MerhavCode"
        Public Const msMerhavNameFieldName As String = "MerhavName"
        Private miMerhavCode As Integer
        Private msMerhavName As String

        Public Sub New(ByRef oPolygon As DMAcadExt.MPolygonOverlay, dicLines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, ByVal iFeatureID As Integer, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
            MyBase.New(oPolygon, dicLines, iFeatureID, iOverlayMethod, iTopoPurpose)
            miMerhavCode = oPolygon.OverlayID_Add
        End Sub
        Public Overloads Shared Sub CreateMainDataTable()
            moMainDataTable = New System.Data.DataTable("OverlayPgons")
            Dim oOrder As System.Type
            Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)

            oOrder = GetType(System.Int32)

            With moMainDataTable.Columns
                .Add(msMerhavCodeFieldName, GetType(System.Int32))
                .Add(msMerhavNameFieldName, GetType(System.String))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.ParcelNameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TplnLot.NameFieldName, GetType(System.String))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
 
                .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
                '19 15
            End With
        End Sub
        Public Overrides Sub AddDataToMainTable()
            Dim oNewRow As System.Data.DataRow
            If moMainDataTable IsNot Nothing Then
                Dim oLot As TplnLot = TplnProject.GetLot(DMAcadExt.enTopoPurpose.Approved, miLotTopoID)
            Dim oParcel As TplnParcel = TplnProject.GetParcel(miParcelTopoID, " AddDataToMainTable")

                Dim oMerhav As TplnMerhav = TplnProject.GetMerhav(miMerhavCode)

                Try
                    oNewRow = moMainDataTable.NewRow()
                    With oNewRow
                        If miMerhavCode <> 0 Then
                            If oMerhav IsNot Nothing Then
                                .Item(msMerhavCodeFieldName) = oMerhav.Code
                                'oExpro.ExproTypeID
                                .Item(msMerhavNameFieldName) = oMerhav.Name

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
    End Class


End Namespace
