Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXDataParcel
	Inherits TplnXDataBasePgon

   Public Const XDataAppName As String = "CPTopo_Parcels"
	Private Enum enMembers
		Block = enBaseMembers.Count
		BlockAdd
		LegalArea
		Status
		Count
	End Enum
	Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, Optional bTest As Boolean = False)
		MyBase.New(oResBuffer, XDataAppName, bTest)

	End Sub
	Public Sub New()
      MyBase.New(enMembers.Count, XDataAppName)
	End Sub


	Public Property Block As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.Block, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.Block, iValue)
		End Set
	End Property
	Public Property BlockAdd As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.BlockAdd, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.BlockAdd, iValue)
		End Set
	End Property

	Public Property LegalArea As Double
		Get
			Dim dRes As Double = 0.0
			MyBase.GetDbl(enMembers.LegalArea, dRes)
			Return dRes
		End Get
		Set(dValue As Double)
			MyBase.SetDbl(enMembers.LegalArea, dValue)
		End Set
	End Property
	Public Property Status As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.Status, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.Status, iValue)
		End Set
	End Property




	Public Overrides Sub PrintList()
		MyBase.StartPrintList()
		DMAcadExt.AcadDocument.WriteMessage("Gush = " & Me.Block.ToString())
		DMAcadExt.AcadDocument.WriteMessage("Gush Add = " & Me.BlockAdd.ToString())
		DMAcadExt.AcadDocument.WriteMessage("LegalArea = " & Me.LegalArea.ToString())
		DMAcadExt.AcadDocument.WriteMessage("Status = " & Me.Status.ToString())
	End Sub
End Class
