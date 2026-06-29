Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.DatabaseServices
Public Class dmEntitySet
   Private moaEntities() As dmEntityPlus
   Private mcolEntityAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Public Sub New(colEntityAcObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, iColorIndex As Integer)
      Dim iIndex As Integer = 0
      mcolEntityAcObjIDs = colEntityAcObjIDs
      ReDim moaEntities(colEntityAcObjIDs.Count - 1)
      For Each tAcObjID As ObjectId In colEntityAcObjIDs
         moaEntities(iIndex) = New dmEntityPlus(tAcObjID)
         moaEntities(iIndex).SetColor(iColorIndex)
         DMAcadExt.AcadTransaction.SetAttributesByBlock(tAcObjID)
         '  DMCommon.Debug.MsgBox("12_281bA", iIndex, moaEntities(iIndex).SourceColorIndex, moaEntities(iIndex).ColorIndex, iColorIndex)
         '    DMAcadExt.AcadDocument.WriteDebugMessage("Center=" & CStr(iIndex) & "," & CStr(moaEntities(iIndex).ColorIndex))
         '   DMAcadExt.AcadTransaction.DBObjectInfo(tAcObjID)
         iIndex += 1
      Next

   End Sub
   Public ReadOnly Property Count As Integer
      Get
         Return mcolEntityAcObjIDs.Count
      End Get
   End Property
   Public Sub RestoreColor()
      For iIndex As Integer = 0 To moaEntities.GetUpperBound(0)
         moaEntities(iIndex).RestoreColor()
      Next
   End Sub
End Class
Public Class dmEntityPlus
   Public dmEntity As Entity
   Private mbVisible As Boolean
   Private miSourceColorIndex As Integer
   Private mtAcObjID As ObjectId
   Public Sub New(tAcObjID As ObjectId)
      mtAcObjID = tAcObjID
      zzOpenEntity()
   End Sub
   Public Sub SetVisible(bVisible As Boolean)
      mbVisible = dmEntity.Visible
      dmEntity.Visible = bVisible
   End Sub
   Public Sub RestoreVisible()
      dmEntity.Visible = mbVisible
   End Sub
   Public Sub SetColor(iColorIndex As Integer)
      If dmEntity IsNot Nothing Then
         miSourceColorIndex = dmEntity.ColorIndex
         dmEntity.ColorIndex = iColorIndex
         '  DMCommon.Debug.MsgBox("12_281xp", miSourceColorIndex, dmEntity.ColorIndex, iColorIndex)
         If Not dmEntity.Visible Then
            dmEntity.Visible = True
         End If
      End If
   End Sub
   Public Sub RestoreColor()
      zzOpenEntity()
      If dmEntity IsNot Nothing Then
         dmEntity.ColorIndex = miSourceColorIndex
      End If

   End Sub
   Public ReadOnly Property SourceColorIndex As Integer
      Get
         Return miSourceColorIndex
      End Get
   End Property
   Public ReadOnly Property ColorIndex As Integer
      Get
         Return dmEntity.ColorIndex
      End Get
   End Property
   Private Sub zzOpenEntity()
      dmEntity = DMAcadExt.AcadTransaction.GetEntity(mtAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
   End Sub
End Class
