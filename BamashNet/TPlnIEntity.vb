Option Explicit On 
Option Strict On

Friend Interface TPlnIEntity
	ReadOnly Property AcEntity() As Autodesk.AutoCAD.DatabaseServices.Entity
	ReadOnly Property AcObjID() As Autodesk.AutoCAD.DatabaseServices.ObjectId
   Property Color() As Integer
   Property Layer() As String
   Sub InsertToModelSpace()
   ReadOnly Property EntityExists() As Boolean
   Sub Delete()
End Interface
