Option Explicit On 
Option Strict On

Public Class TPlItemList
	Inherits System.Collections.DictionaryBase
	Private miListType As enListType
	Private mdicBase As System.Collections.IDictionary
	Public ReadOnly Property ListType() As enListType
		Get
			ListType = miListType
		End Get

	End Property

	Public Sub New(ByVal iListType As enListType)
		miListType = iListType
		mdicBase = MyBase.Dictionary
	End Sub
   Public Sub Add(ByVal oItem As DMCommon.ItemData)
      mdicBase.Add(oItem.ListIndex, oItem)
   End Sub
   Public Function GetItem(ByVal iItemID As Integer) As DMCommon.ItemData
      Return CType(mdicBase.Item(iItemID), DMCommon.ItemData)
   End Function
	Public Function GetItemValue(ByVal iItemID As Integer) As String
      Dim oItemData As DMCommon.ItemData
		oItemData = CType(mdicBase.Item(iItemID), DMCommon.ItemData)
		Return oItemData.ListDispData
	End Function
End Class
