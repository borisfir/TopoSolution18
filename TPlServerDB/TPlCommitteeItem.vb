Option Explicit On
Option Strict On

Public Class TPlCommitteeItem

	Inherits DMCommon.ItemData

	Private miID As Integer
	Private msName As String
	Private miDistrictID As Integer
	Private msShortName As String
	Private msDistrictName As String

	'	Private mbSingleProject As Boolean
	Public Property ID() As Integer
		Get
			ID = miID
		End Get
		Set(ByVal iValue As Integer)
			miID = iValue
			MyBase.ListIndex = iValue
		End Set
	End Property
	Public Property Name() As String
		Get
			Name = msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
			MyBase.ListDispData = sValue
		End Set
	End Property
	Public Property ShortName() As String
		Get
			ShortName = msShortName
		End Get
		Set(ByVal sValue As String)
			msShortName = sValue
		End Set
	End Property
	Public Property DistrictID() As Integer
		Get
			DistrictID = miDistrictID
		End Get
		Set(ByVal iValue As Integer)
			miDistrictID = iValue
		End Set
	End Property
	Public Property DistrictName() As String
		Get
			DistrictName = msDistrictName
		End Get
		Set(ByVal sValue As String)
			msDistrictName = sValue
		End Set
	End Property
End Class
