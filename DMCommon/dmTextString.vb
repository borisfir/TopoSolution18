Option Explicit On
Option Strict On
Public Class dmTextLine
   Private miaWidth() As Integer
   Private miUB As Integer
   Private msaFieldValue() As String
   Private msDelim As String
   Private msResValue As String
   Public Sub New(iaWidth() As Integer)
      miaWidth = iaWidth
      miUB = miaWidth.GetUpperBound(0)
      ReDim msaFieldValue(miUB)
   End Sub
   Public Sub New(sDelim As String)
      msDelim = sDelim
     
   End Sub
   Public Sub AddValue(sValue As String)
      If msResValue IsNot Nothing Then
         msResValue &= msDelim & sValue
      Else
         msResValue = sValue
      End If
   End Sub
   Public Sub AddAttribute(sName As String, oValue As System.Object)
      Me.AddValue(sName & "=" & oValue.ToString())
   End Sub
   Public Sub Close(sValue As String)
      If msResValue IsNot Nothing Then
         msResValue &= sValue
      End If
   End Sub
   Public Property FieldValue(iIndex As Integer) As String
      Get
         Return msaFieldValue(iIndex)
      End Get
      Set(sValue As String)
         msaFieldValue(iIndex) = sValue
      End Set
   End Property
   Public ReadOnly Property LineValue As String
      Get
         If msDelim Is Nothing Then
            Dim sRes As String = String.Empty
            For iIndex As Integer = 0 To miUB
               sRes &= zzFieldFormat(msaFieldValue(iIndex), miaWidth(iIndex))
            Next
            Return sRes
         Else
            Return msResValue
         End If
       
      End Get
   End Property
   Private Function zzFieldFormat(sValue As String, iWidth As Integer) As String
      If String.IsNullOrEmpty(sValue) Then
         Return Strings.Space(iWidth)
      Else
         Select Case sValue.Length
            Case Is < iWidth
               Return Space(iWidth - sValue.Length) & sValue
            Case Is = iWidth
               Return sValue
            Case Else
               Return sValue.Substring(0, iWidth)
         End Select
      End If

   End Function
End Class
