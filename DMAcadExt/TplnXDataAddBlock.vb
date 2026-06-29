Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXDataAddBlock
   Inherits TplnXData
   Public Const XDataAppName As String = "dmAddBlock"

   Protected Enum enBaseMembers
      Type
      Handle
      Count
   End Enum
   Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, Optional bTest As Boolean = False)
      MyBase.New(oResBuffer, XDataAppName, bTest)
   End Sub
   Public Sub New(iCount As Integer)
      MyBase.New(enBaseMembers.Count, XDataAppName)
   End Sub
   Public Property Type As Integer
      Get
         Dim iRes As Integer = 0
         MyBase.GetInt(enBaseMembers.Type, iRes)
         Return iRes
      End Get
      Set(iValue As Integer)
         MyBase.SetInt(enBaseMembers.Type, iValue)
      End Set
   End Property
   'DataBase Handle	1005. The handle of an entity.
   Public Property EntityHandle As Handle
      Get
         Dim tHandle As Handle  
         MyBase.GetHandle(enBaseMembers.Handle, tHandle)
         Return tHandle
      End Get
      Set(tValue As Handle)
         MyBase.SetHandle(enBaseMembers.Handle, tValue)
      End Set
   End Property
End Class

