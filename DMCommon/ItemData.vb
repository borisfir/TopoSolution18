
Option Explicit On
Option Strict On
Imports System

Public Enum SortType
	IgnoreCase
   MatchCase
   Numerical
End Enum
Public Class ItemData
	Private miListIndex As Integer = 0
	Private msListDispData As String

	Public Sub New(ByVal iListIndex As Integer, ByVal sListDispData As String)
		miListIndex = iListIndex
		msListDispData = sListDispData
	End Sub
	Public Sub New()

	End Sub
	Public Property ListIndex() As Integer
		Get
			Return miListIndex
		End Get
		Set(ByVal iValue As Integer)
			miListIndex = iValue

		End Set
	End Property
	Public ReadOnly Property ListIndexStr() As String
		Get
			Return miListIndex.ToString()
		End Get
	End Property
	Public Property ListDispData() As String
		Get
			Return msListDispData
		End Get
		Set(ByVal sValue As String)
			msListDispData = sValue
		End Set
	End Property
	Public Overrides Function ToString() As String
		Return msListDispData
	End Function

	Public Shared ReadOnly Property ValueMember() As String
		Get
			Return "ListIndex"
		End Get
	End Property
	Public Shared ReadOnly Property DisplayMember() As String
		Get
			Return "ListDispData"
		End Get
	End Property
End Class
Public Class DataItems
	Private mdicItems As Generic.IDictionary(Of Integer, ItemData)
	Private moList As Generic.IList(Of ItemData)
	Public Sub New(oDataReader As System.Data.Common.DbDataReader)
		If oDataReader.HasRows Then
			Dim oItem As ItemData
			mdicItems = New Dictionary(Of Integer, ItemData)
			moList = New Generic.List(Of ItemData)
			While oDataReader.Read()
				oItem = New ItemData(oDataReader.GetInt32(0), oDataReader.GetString(1))
				mdicItems.Add(oItem.ListIndex, oItem)
				moList.Add(oItem)
			End While
		End If
		oDataReader.Close()
	End Sub
	Public ReadOnly Property ItemList As Generic.IList(Of ItemData)
		Get
			Return moList
		End Get
	End Property

End Class
Public Class LocalityItem
	Inherits ItemData
	Private miDistrict As Integer
	Private miSubdistrict As Integer
	Private miMunicipialStatus As Integer
	Private miCommittee As Integer

	Public Sub New(ByVal iListIndex As Integer, ByVal sListDispData As String)
		MyBase.New(iListIndex, sListDispData)
	End Sub
	Public Property District() As Integer
		Get
			Return miDistrict
		End Get
		Set(ByVal iValue As Integer)
			miDistrict = iValue
		End Set
	End Property
	Public Property Subdistrict() As Integer
		Get
			Return miSubdistrict
		End Get
		Set(ByVal iValue As Integer)
			miSubdistrict = iValue
		End Set
	End Property
	Public Property MunicipialStatus() As Integer
		Get
			Return miMunicipialStatus
		End Get
		Set(ByVal iValue As Integer)
			miMunicipialStatus = iValue
		End Set
	End Property
	Public Property Committee() As Integer
		Get
			Return miCommittee
		End Get
		Set(ByVal iValue As Integer)
			miCommittee = iValue
		End Set
	End Property
End Class
Public Structure Localities
	Dim Dictionary As Generic.IDictionary(Of Integer, LocalityItem)
	Dim List As Generic.IList(Of LocalityItem)
	Public Sub New(ByVal oDictionary As Generic.IDictionary(Of Integer, LocalityItem), ByVal oList As Generic.IList(Of LocalityItem))
		Dictionary = oDictionary
		List = oList
	End Sub

End Structure
