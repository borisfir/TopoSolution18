Option Explicit On
Option Strict On
Public Class UD_FLines
	Inherits Dictionary(Of Autodesk.AutoCAD.DatabaseServices.ObjectId, UD_FLine)
	Private mdicFlinesByID As IDictionary(Of UInteger, UD_FLine)
   Public Sub New()
      mdicFlinesByID = New Dictionary(Of UInteger, UD_FLine)()
   End Sub
   Public Sub AddFLine(oLine As UD_FLine)
      Dim iKey As UInteger = oLine.IDKey
      ' DMCommon.Debug.MsgBox("09_791a", oPrevPoint.Name, oNextPoint.Name)
      If Not mdicFlinesByID.ContainsKey(iKey) Then
         mdicFlinesByID.Add(iKey, oLine)
      End If
   End Sub
   Public Overloads ReadOnly Property Count As Integer
      Get
         If MyBase.Count = 0 Then
            Return mdicFlinesByID.Count
         Else
            Return MyBase.Count
         End If
      End Get
   End Property
   Public Overloads ReadOnly Property Values As System.Collections.Generic.ICollection(Of UD_FLine)
      Get
         If MyBase.Count = 0 Then
            Return mdicFlinesByID.Values
         Else
            Return MyBase.Values
         End If

      End Get
   End Property
End Class
