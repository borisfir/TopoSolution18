Option Explicit On
Option Strict On

Public Class TplnPointArray
   Private moaPoints() As TPlnPoint
   Private miUpperBound As Integer
   Private miCurrentIndex As Integer = 0
   Public Sub New()
      miUpperBound = -1
   End Sub
   Public Sub New(ByVal iUpperBound As Integer)
      miUpperBound = iUpperBound
      If miUpperBound >= 0 Then
         ReDim Preserve moaPoints(miUpperBound)
      End If
   End Sub
   Public Sub New(ByVal oaPoints() As TPlnPoint)
      moaPoints = oaPoints
      miUpperBound = moaPoints.GetUpperBound(0)
   End Sub
   Public Sub AddDim(ByVal iUpperBound As Integer)
      miUpperBound += iUpperBound + 1
      If iUpperBound >= 0 Then
         ReDim Preserve moaPoints(miUpperBound)
      End If
   End Sub
   Public Sub Add(ByVal oPoint As TPlnPoint)
      miUpperBound += 1
      ReDim Preserve moaPoints(miUpperBound)
      moaPoints(miUpperBound) = oPoint
   End Sub

   Public Sub Reset()
      If miUpperBound >= 0 Then
         miCurrentIndex = 0
      End If
   End Sub
   Public Function MoveNext() As Boolean
      If miUpperBound >= 0 AndAlso miCurrentIndex <= miUpperBound Then
         miCurrentIndex += 1
      End If
   End Function
   Public ReadOnly Property Current() As TPlnPoint
      Get
         If miUpperBound >= 0 AndAlso miCurrentIndex <= miUpperBound Then
            Return Me.Item(miCurrentIndex)
         Else
            Return Nothing
         End If
      End Get
   End Property
   Public Property UpperBound() As Integer
      Get
         Return miUpperBound
      End Get
      Set(ByVal iValue As Integer)
         Try
            ReDim Preserve moaPoints(iValue)
            miUpperBound = iValue
         Catch oEx As Exception

         End Try
      End Set
   End Property
   Public Property Item(ByVal iIndex As Integer) As TPlnPoint
      Get
         Try
            Return moaPoints(iIndex)
         Catch oEx As Exception
            Return Nothing
         End Try

      End Get
      Set(ByVal oValue As TPlnPoint)
         Try
            moaPoints(iIndex) = oValue
         Catch oEx As Exception

         End Try
      End Set
   End Property

End Class
