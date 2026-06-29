Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Public Class prjPrograms
	Public Shared Sub GazAddOD()
		Const sODTableName As String = "GazArea"

		Const sParcelAcObjID As String = "ParcelAcObjID"
		Const sLotTopoID As String = "LotTopoID"
		Dim iOverlayIndex As DMAcadExt.enOverlayIndex = DMAcadExt.enOverlayIndex.ApprMerge
		Dim iTopoPurpose As DMAcadExt.enTopoPurpose = TopoManager.TPlanGraph.UnionPgonArea.GetTopoPurpose(iOverlayIndex)
		Dim iOverlayMethod As DMAcadExt.enOverlayMethod = TopoManager.TPlanGraph.UnionPgonArea.GetOverlayMethod(iOverlayIndex)
		Dim oCurrentDataView As DataView = TopoManager.TPlanGraph.TplnProject.OverlayGroupView(iOverlayIndex)
		Dim iCurrentParcelID As Integer = 0
		Dim iParcelID As Integer
		Dim sLotName As String

		'	Dim sDWGName As String = DMAcadExt.AcadDocument.GetCurrentDWGName()
		'	Dim oFileInfo As System.IO.FileInfo
		Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
		Dim oODTable As ObjectData.Table = oODTables.Item(sODTableName)

		Dim tParcelAcObjID As ObjectId
		Dim dParcelArea As Double
		Dim dArea As Double
		Dim dInnerArea As Double
		Dim dBoundaryArea As Double
		Dim sTest As String
		If oCurrentDataView IsNot Nothing Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
			oCurrentDataView.Sort = TopoManager.TPlanGraph.TplnParcel.msNameFieldName & "," & sLotTopoID
			For Each oRowView As System.Data.DataRowView In oCurrentDataView
				sTest = "A"
				iParcelID = Convert.ToInt32(DirectCast(oRowView.Item(TopoManager.TPlanGraph.TplnParcel.msNameFieldName), String))
				If iParcelID <> iCurrentParcelID Then
					sTest &= "B"

					If iCurrentParcelID <> 0 Then
						sTest &= "C"
						zzAddObjectData(oODTable, dParcelArea, dInnerArea, dBoundaryArea, tParcelAcObjID)
						DMAcadExt.AcadDocument.WriteMessageLog(CStr(iCurrentParcelID) & ":" & CStr(dInnerArea) & ":" & CStr(dBoundaryArea))
					End If
					dParcelArea = DirectCast(oRowView.Item(TopoManager.TPlanGraph.TplnParcel.msParcelCalcAreaFieldName), Double)
					tParcelAcObjID = DirectCast(oRowView.Item(sParcelAcObjID), ObjectId)
					dInnerArea = 0.0
					dBoundaryArea = 0.0
					iCurrentParcelID = iParcelID
				End If
				sLotName = DMCommon.Functions.CStrN(oRowView.Item(TopoManager.TPlanGraph.TplnLot.msNameFieldName))
				dArea = DMCommon.Functions.CDblN(oRowView.Item(TopoManager.TopoReader.msAreaFldName))

				Select Case sLotName
					Case "1"
						dInnerArea += dArea
					Case "2", "3"
						dBoundaryArea += dArea
				End Select

			Next
			zzAddObjectData(oODTable, dParcelArea, dInnerArea, dBoundaryArea, tParcelAcObjID)
			DMAcadExt.AcadDocument.Unlock()
		End If
	End Sub
	Private Shared Sub zzAddObjectData(ByRef oODTable As ObjectData.Table, ByVal dParcelArea As Double, ByVal dInnerArea As Double, ByVal dBoundaryArea As Double, ByVal tParcelAcObjID As ObjectId)
		Dim oMapValue As Utilities.MapValue
		Dim oODRecord As Record
		Try
			oODRecord = Record.Create
			oODTable.InitRecord(oODRecord)
			oMapValue = oODRecord.Item(0)
			oMapValue.Assign(Convert.ToInt32(dParcelArea))
			oMapValue = oODRecord.Item(1)
			oMapValue.Assign(Convert.ToInt32(dInnerArea))
			oMapValue = oODRecord.Item(2)
			oMapValue.Assign(Convert.ToInt32(dBoundaryArea))
			oODTable.AddRecord(oODRecord, tParcelAcObjID)
		Catch oMapEx As Autodesk.Gis.Map.MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "prjPrograms - zzAddObjectData")
		End Try

	End Sub
End Class
