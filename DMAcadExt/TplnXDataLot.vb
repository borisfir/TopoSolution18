Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXDataLot
	Inherits TplnXDataBasePgon

   Public XDataAppName As String = "CPTopo_Lots"
	Private Enum enMembers
		Landuse = enBaseMembers.Count
		Count
	End Enum
   Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, sXDataAppName As String, Optional bTest As Boolean = False)
      MyBase.New(oResBuffer, sXDataAppName, bTest)
   End Sub
   Public Sub New(sXDataAppName As String)
      MyBase.New(enMembers.Count, sXDataAppName)
   End Sub
	Public Property Landuse As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.Landuse, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.Landuse, iValue)
		End Set
	End Property
	Public Overrides Sub PrintList()
		MyBase.StartPrintList()
		DMAcadExt.AcadDocument.WriteMessage("Landuse = " & Me.Landuse.ToString())
	End Sub
End Class
