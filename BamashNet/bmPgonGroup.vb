Option Explicit On
Option Strict On
Public Class bmPgonGroup
	Private miGroupID As Integer
	Private miBasePgonID As Integer
	Private mdArea As Double
	Private miSubPropNum As Integer
	Private msSubPropCaption As String
	Public Sub New(iGroupID As Integer)
		miGroupID = iGroupID
	End Sub
	Public Sub AddPgon(oPgon As BamashPolygon)
		If oPgon.IsGroupBase Then
			miBasePgonID = oPgon.TopoID
			miSubPropNum = oPgon.SubPropNum
			msSubPropCaption = oPgon.SubPropCaption
		End If
		mdArea += oPgon.Area
	End Sub
	Public ReadOnly Property Area As Double
		Get
			Return mdArea
		End Get
	End Property
	Public ReadOnly Property BasePgonID As Integer
		Get
			Return miBasePgonID
		End Get
	End Property
	Public ReadOnly Property GroupID As Integer
		Get
			Return miGroupID
		End Get
	End Property
	Public Property SubPropNum As Integer
		Get
			Return miSubPropNum
		End Get
		Set(iValue As Integer)
			miSubPropNum = iValue
		End Set
	End Property
	Public ReadOnly Property SubPropCaption As String
		Get
			Return msSubPropCaption
		End Get
	End Property
End Class
