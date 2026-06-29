Option Explicit On
Option Strict On

Public Class TopoElemDic
   Inherits System.Collections.Specialized.StringDictionary
   Private Const sValueDel As String = ","
   Private miItemIndex As Integer
   Private miaCurrentItem() As String
   Private miCurrentItemUB As Integer
   Public Overloads Sub Add(ByVal sElemName As String, ByVal iTopologyIndex As Integer)
      If MyBase.ContainsKey(sElemName) Then
         Dim sValue As String = MyBase.Item(sElemName)
         sValue &= sValueDel & iTopologyIndex.ToString()
      Else
         MyBase.Add(sElemName, iTopologyIndex.ToString())
      End If
	End Sub
   Public Function OpenElem(ByVal sElemName As String) As Boolean
		Dim bExists As Boolean = MyBase.ContainsKey(sElemName)
      If bExists Then
         miItemIndex = 0
         miaCurrentItem = Strings.Split(MyBase.Item(sElemName), sValueDel)
         miCurrentItemUB = miaCurrentItem.GetUpperBound(0)
      Else
         miItemIndex = -1
      End If
      Return bExists
   End Function
   Public ReadOnly Property Current() As Integer
      Get
         If miItemIndex >= 0 Then
            Try
               Return CType(miaCurrentItem(miItemIndex), Integer)

				Catch oEx As Exception
					Return 0
				End Try
			Else
				Return 0
			End If
      End Get
   End Property
   Public Function MoveNext() As Boolean
      miItemIndex += 1
      If miItemIndex <= miCurrentItemUB Then
         Return True
      Else
         miItemIndex = -1
         Return False
      End If
   End Function
End Class
