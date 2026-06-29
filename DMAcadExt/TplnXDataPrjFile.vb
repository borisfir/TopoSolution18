Option Explicit On
Option Strict On
''''''''Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXDataPrjFile
	Inherits TplnXData
	Public Const XDataAppName As String = "dmPrjFiles"

	Protected Enum enBaseMembers
		ID
		Priority
		ReservedA
		ReservedB
		Count
	End Enum
	Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, Optional bTest As Boolean = False)
		MyBase.New(oResBuffer, XDataAppName, bTest)
	End Sub
	Public Sub New(iCount As Integer)
		MyBase.New(enBaseMembers.Count, XDataAppName)
	End Sub
	Public Property ID As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.ID, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.ID, iValue)
		End Set
	End Property

	Public Property Priority As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.Priority, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.Priority, iValue)
		End Set
	End Property
End Class
