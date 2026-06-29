Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXDataOverlay_AAA
	Inherits TplnXData
	Const msXDataAppName As String = "CPTopo_Parcels"
	Private Enum enMembers
		FeatID
		SourceID
		SourceArea
		OverlayID
		OverlayArea
		Count
	End Enum
	Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer)
		MyBase.New(oResBuffer, msXDataAppName)
	End Sub
	Public Sub New()
		MyBase.New(enMembers.Count, msXDataAppName)
	End Sub
	Public Property SourceID As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.SourceID, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.SourceID, iValue)
		End Set
	End Property
	Public Property SourceArea As Double
		Get
			Dim dRes As Double = 0.0
			MyBase.GetDbl(enMembers.SourceArea, dRes)
			Return dRes
		End Get
		Set(dValue As Double)
			MyBase.SetDbl(enMembers.SourceArea, dValue)
		End Set
	End Property

	Public Property OverlayID As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enMembers.OverlayID, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enMembers.OverlayID, iValue)
		End Set
	End Property
	Public Property OverlayArea As Double
		Get
			Dim dRes As Double = 0.0
			MyBase.GetDbl(enMembers.OverlayArea, dRes)
			Return dRes
		End Get
		Set(dValue As Double)
			MyBase.SetDbl(enMembers.OverlayArea, dValue)
		End Set
	End Property
End Class
