Option Explicit On
Option Strict On
Public Class ItemDataDict
   Inherits System.Collections.Generic.Dictionary(Of Integer, ItemData)
   Private mbHasGroup As Boolean
   Public Sub New(ByVal bHasGroup As Boolean)
      MyBase.New()
      mbHasGroup = bHasGroup
   End Sub
   Public Overloads Sub Add(ByVal oItemData As ItemData)
      MyBase.Add(oItemData.ListIndex, oItemData)
   End Sub
   Public Overloads Sub Add(ByVal oItemData As ItemData, ByVal iGroupKey As Integer)
      MyBase.Add(oItemData.ListIndex, oItemData)
   End Sub
   Public Sub AddGroupIndex(ByVal iKey As Integer, ByVal iGroupKey As Integer)

   End Sub
End Class
