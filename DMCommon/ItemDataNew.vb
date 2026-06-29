Option Explicit On
Option Strict On
Public Class ItemDataNew
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
