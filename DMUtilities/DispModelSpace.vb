Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map

Imports Autodesk.Gis.Map.Utilities
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Class DispModelSpace
   Private moTransaction As Transaction = Nothing
   Private moTable As System.Data.DataTable = New System.Data.DataTable("ModelSpaceData")
   Public Sub Fill()
      zzCreateTable()
      zzFill()
   End Sub
   Private Sub zzFill()

      '  Dim sLayersDel As String
      Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

      Dim oBlockTable As BlockTable
      Dim oBlockTableRecord As BlockTableRecord
      Dim oEntity As Entity

      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Try
         moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
         oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
         oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
         Dim sRXClassName As String
         '   Dim sLayerName As String
         Dim oNewRow As DataRow


         For Each objId As ObjectId In oBlockTableRecord
            oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
            sRXClassName = oEntity.GetRXClass().Name
            oNewRow = moTable.NewRow()
            oNewRow.Item("ObjID") = objId.OldId
            oNewRow.Item("ObjName") = sRXClassName
            oNewRow.Item("Layer") = oEntity.Layer
            moTable.Rows.Add(oNewRow)
         Next


         moTransaction.Commit()
         moTransaction = Nothing

      Catch e As Exception
         System.Windows.Forms.MessageBox.Show(e.Message, "e100")

      Finally
         If Not moTransaction Is Nothing Then
            moTransaction.Abort()
            moTransaction = Nothing

         End If
         Me.dgvMain.DataSource = moTable
      End Try
   End Sub
   Private Sub zzCreateTable()
      moTable.Columns.Add("ObjID", GetType(System.Int32))
      moTable.Columns.Add("ObjName", GetType(System.String))
      moTable.Columns.Add("Layer", GetType(System.String))
   End Sub

   Private Sub SelectByCurrent_Click(ByVal oSender As System.Object, ByVal e As System.EventArgs)
      Dim oButton As System.Windows.Forms.ToolStripButton = DirectCast(oSender, System.Windows.Forms.ToolStripButton)
      Select Case oButton.Name
         Case "SelectByCurrent"

      End Select
   End Sub
   Private Sub zzSelectByCurrent()
      Dim oRow As DataGridViewRow
      Dim iObjID As Integer
      Dim oEntity As Entity
      Dim oExtents3D As Extents3d
      Dim oClientViewInfo As Autodesk.AutoCAD.GraphicsSystem.ClientViewInfo = New Autodesk.AutoCAD.GraphicsSystem.ClientViewInfo()
      oClientViewInfo.AcadWindowId = 66
      Dim iWindowId As Integer = oClientViewInfo.AcadWindowId
      Dim iViewportId As Integer = oClientViewInfo.ViewportId
      Dim iViewportObjectId As Integer = oClientViewInfo.ViewportObjectId

      MessageBox.Show(iWindowId.ToString() & ":" & iViewportId.ToString() & ":" & iViewportObjectId.ToString(), "ClientViewInfo")
      oRow = Me.dgvMain.CurrentRow
      Dim oView As Autodesk.AutoCAD.GraphicsSystem.View = New Autodesk.AutoCAD.GraphicsSystem.View(oClientViewInfo, True)
      oView.Show()
      Try
         moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
         iObjID = DirectCast(oRow.Cells("ObjID").Value, Integer)
         Dim oObjID As ObjectId = New ObjectId(iObjID)
         oEntity = DirectCast(moTransaction.GetObject(oObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
         oExtents3D = oEntity.GeometricExtents
         MessageBox.Show(Format(oExtents3D.MaxPoint.X, "0.00"), "X")
         Dim oLowerLeftPoint As Point2d = ToPoint2d(oExtents3D.MinPoint)
         Dim oUpperRightPoint As Point2d = ToPoint2d(oExtents3D.MaxPoint)

         oView.ZoomWindow(oLowerLeftPoint, oUpperRightPoint)
      Catch ex As Exception
         MessageBox.Show(ex.Message)
      End Try
      zzZoom()
   End Sub
   Private Sub zzZoom()
      Dim oViewTable As ViewTable
      Dim oViewTableRecord As ViewTableRecord
      ' Dim oEntity As Entity
      Dim oEnumerator As SymbolTableEnumerator
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Try
         System.Windows.Forms.MessageBox.Show("2100")
         moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
         oViewTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.ViewTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), ViewTable)

         oViewTableRecord = CType(moTransaction.GetObject(oViewTable.Item("Test"), OpenMode.ForRead), ViewTableRecord)
         System.Windows.Forms.MessageBox.Show(oViewTable.GetRXClass().Name, "ViewTable_RXClass")
         '     oViewTableRecord = DirectCast(moTransaction.GetObject(oViewTable.Item(ViewTableRecord.), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)

         oEnumerator = oViewTable.GetEnumerator()
         System.Windows.Forms.MessageBox.Show("2110")
         oEnumerator.Reset()
         System.Windows.Forms.MessageBox.Show("2120")

         Dim oObjID As ObjectId
         Dim oDBObject As DBObject

         Do While oEnumerator.MoveNext()
            System.Windows.Forms.MessageBox.Show("2130")
            oObjID = oEnumerator.Current
            System.Windows.Forms.MessageBox.Show("2140")
            oDBObject = moTransaction.GetObject(oObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
            System.Windows.Forms.MessageBox.Show(oDBObject.GetRXClass().Name, "ViewTableRecord-???")
            oViewTableRecord = CType(oDBObject, ViewTableRecord)
            System.Windows.Forms.MessageBox.Show(oViewTableRecord.Name, "oViewTableRecord.Name")

            '  

         Loop
         System.Windows.Forms.MessageBox.Show("2199")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message)
      End Try

   End Sub
   Private Sub toolstrUp_ItemClicked(ByVal oSender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles toolstrUp.ItemClicked
      Dim oClickedItem As System.Windows.Forms.ToolStripItem = e.ClickedItem
      Select Case oClickedItem.Name
         Case "SelectByCurrent"
            zzSelectByCurrent()
      End Select



   End Sub
   Private Function ToPoint2d(ByVal oPoint3d As Point3d) As Point2d
      Return New Point2d(oPoint3d.X, oPoint3d.Y)
   End Function
End Class