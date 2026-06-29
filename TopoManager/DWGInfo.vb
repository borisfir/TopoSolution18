Option Explicit On
Option Strict On
'Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.EditorInput
Public NotInheritable Class DWGInfo
   Const sDel As String = ","
   Private Shared moaLayers() As LayerInfo
   Private Shared miLayerUB As Integer = -1
   Private Shared msaLayersDel() As String
   Private Shared mbActual As Boolean = True
   Public Shared Sub AddLayer(ByVal iIndex As Integer, ByVal sName As String)
      miLayerUB += 1
      ReDim moaLayers(miLayerUB)
      moaLayers(miLayerUB) = New LayerInfo(iIndex, sName)
   End Sub

   Public Shared Sub Open()
      mbActual = True
      zzScan()
   End Sub
   Public Shared Sub Close()
      mbActual = False
   End Sub
   Private Shared Sub zzScan()
      Dim oTransaction As Transaction = Nothing

      Dim oBlockTable As BlockTable
      Dim oBlockTableRecord As BlockTableRecord
      Dim oEntity As Entity

      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

      Dim sIbjTest As String = ""

      Dim oTestBlockTableRecord As BlockTableRecord
      Try
         oTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
         oBlockTable = DirectCast(oTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)


         oBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
         oTestBlockTableRecord = DirectCast(oTransaction.GetObject(oBlockTable.Item("Centroid"), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
         oTestBlockTableRecord.GetBlockReferenceIds(True, True)
         Dim sRXClassName As String

         Dim sEntityLayerName As String
         For Each objId As ObjectId In oBlockTableRecord
            oEntity = DirectCast(oTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
            sRXClassName = oEntity.GetRXClass().Name

            Select Case sRXClassName
					Case DMAcadExt.AcadConst.AcadPolylineName, Common.AcadLWPolylineName
						sEntityLayerName = DMAcadExt.TopoDef.AddDelim(oEntity.Layer)
						For iIndex As Integer = 0 To miLayerUB
							If msaLayersDel(iIndex).Contains(sEntityLayerName) Then
								moaLayers(iIndex).HasEntity = True
							End If
						Next
				End Select

         Next

         oTransaction.Commit()
         oTransaction = Nothing
      Catch e As Exception
         System.Windows.Forms.MessageBox.Show(e.Message, "e100")
      Finally
         If Not oTransaction Is Nothing Then
            oTransaction.Abort()
            oTransaction = Nothing

         End If

      End Try


   End Sub
   Public Shared Sub LayerStatus(ByRef iLayerIndex() As Integer, ByRef bHasEntity() As Boolean)
      ReDim iLayerIndex(miLayerUB), bHasEntity(miLayerUB)
      For iIndex As Integer = 0 To miLayerUB
         iLayerIndex(iIndex) = moaLayers(iIndex).Index
         bHasEntity(iIndex) = moaLayers(iIndex).HasEntity
      Next
   End Sub
   Private Sub zzGetLayersStr()
      ReDim msaLayersDel(miLayerUB)
      For iIndex As Integer = 0 To miLayerUB
			msaLayersDel(iIndex) = DMAcadExt.TopoDef.AddDelim(moaLayers(iIndex).Name)
      Next

   End Sub
   Private Structure LayerInfo
      Sub New(ByVal iIndex As Integer, ByVal sName As String)
         Index = iIndex
         Name = sName
         Exists = False
         HasEntity = False
      End Sub
      Dim Index As Integer
      Dim Name As String
      Dim Exists As Boolean
      Dim HasEntity As Boolean
   End Structure
End Class
