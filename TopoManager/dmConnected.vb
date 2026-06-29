Imports Autodesk.AutoCAD.DatabaseServices
Public Class dmConnected
	Const msLayerPrefix As String = "dm-"
	Const msAuxiliaryLayer As String = "dm-ezer"
	Public Shared Sub LoadModelSpace()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
		Dim colAllDBObjects As DBObjectCollection = DMAcadExt.AcadTransaction.GetAllDBObjects()
		Dim oEntity As Entity
		Dim sLayer As String
		Dim sRXClassName As String
		'	Dim oPolyLine As Polyline
		'	Dim oPolyline2d As Polyline2d
		'	Dim oLine As Line
		'Dim oArc As Arc
		Dim oCurve As Curve
		Dim bClosed As Boolean
		Dim bIsReallyClosing As Boolean
		Dim tStartPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim tEndPoint As Autodesk.AutoCAD.Geometry.Point3d
		Dim bStartEqualToEnd As Boolean


		For Each oDBObject As DBObject In colAllDBObjects
			oEntity = TryCast(oDBObject, Entity)
			sLayer = oEntity.Layer
			sRXClassName = oDBObject.GetRXClass().Name
			If sLayer.StartsWith(msLayerPrefix) AndAlso sLayer <> msAuxiliaryLayer Then
				Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, DMAcadExt.AcadConst.Acad2dPolylineName, DMAcadExt.AcadConst.AcadLineName, DMAcadExt.AcadConst.AcadArcName
						oCurve = TryCast(oDBObject, Curve)
						bClosed = oCurve.Closed
						bIsReallyClosing = oCurve.IsReallyClosing
						tStartPoint = oCurve.StartPoint
						tEndPoint = oCurve.EndPoint
						bStartEqualToEnd = tStartPoint.IsEqualTo(tEndPoint)

					Case Else
						bClosed = False
						bIsReallyClosing = False
						bStartEqualToEnd = False
				End Select

				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Entity", oDBObject.Handle.Value, oDBObject.Handle.ToString(), sLayer, sRXClassName, bClosed, bStartEqualToEnd, oDBObject.AcadObject.ToString, oDBObject.GetType.ToString)
			End If
		Next


		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()

	End Sub
	Public Shared Sub LoadLayerList()

	End Sub
End Class
