Option Explicit On
Option Strict On
Public Class TPlLanduseItem
	Inherits DMCommon.ItemData

   Private miID As Integer
   Private miOrderID As Integer

   Private msName As String


   Public Sub New()

   End Sub

   Public Sub New(ByVal iID As Integer, ByVal sName As String, ByVal iOrderID As Integer)
      ID = iID
      msName = sName
      miOrderID = iOrderID

      MyBase.ListIndex = ID
      MyBase.ListDispData = sName
   End Sub

   Public Property ID() As Integer
      Get
         Return miID
      End Get
      Set(ByVal iValue As Integer)
         miID = iValue
         MyBase.ListIndex = iValue
      End Set
   End Property
   Public Property OrderID() As Integer
      Get
         Return miOrderID
      End Get
      Set(ByVal iValue As Integer)
         miOrderID = iValue

      End Set
   End Property
   Public Property Name() As String
      Get
         Return msName
      End Get
      Set(ByVal sValue As String)
         msName = sValue
         MyBase.ListDispData = sValue

      End Set
   End Property
  
 

End Class
