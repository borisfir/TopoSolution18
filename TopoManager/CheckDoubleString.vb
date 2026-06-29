Option Explicit On
Option Strict On
Public Class CheckDoubleString
   Private Shared miItemTest As Integer
   Private Shared miInstanceTest As Integer

   Private Const msDelim As String = "|"
   Private Const msSQLDelim As String = ","
   Private Const msStrQualifier As String = "'"
   Private mdicBase As Generic.Dictionary(Of String, Integer)
   Private mdicDoubleValues As Generic.Dictionary(Of String, Queue(Of Integer))
   Private miDimension As Integer

   Public Sub New()
      mdicBase = New Generic.Dictionary(Of String, Integer)
      mdicDoubleValues = New Generic.Dictionary(Of String, Queue(Of Integer))
      miInstanceTest += 1

   End Sub
   Public Sub Add(ByVal sValue As String, ByVal bString As Boolean, iTopoID As Integer)
      miDimension = 1
      zzTransformer(sValue, bString)
      zzAdd(sValue, iTopoID)

   End Sub
   Public Sub Add(ByVal sValue1 As String, ByVal bString1 As Boolean, ByVal sValue2 As String, ByVal bString2 As Boolean, iTopoID As Integer)
      zzTransformer(sValue1, bString1)
      zzTransformer(sValue2, bString2)
      Dim sKey As String = zzGetKey(sValue1, sValue2)
      miDimension = 2
      zzAdd(sKey, iTopoID)
      '  DMAcadExt.AcadDocument.WriteMessage("%$:" & CStr(sKey) & ":" & CStr(iTopoID) & ":" & CStr(miInstanceTest) & ":" & CStr(miItemTest))
      miItemTest += 1
   End Sub
   Public ReadOnly Property DoubleValues As Generic.Dictionary(Of String, Queue(Of Integer))
      Get
         Return mdicDoubleValues
      End Get
   End Property
   Public Sub Clear()
      mdicBase.Clear()
      mdicDoubleValues.Clear()
   End Sub
   Private Sub zzTransformer(ByRef sValue As String, ByVal bString As Boolean)
      If bString Then
         sValue = msStrQualifier & sValue & msStrQualifier
      End If
   End Sub

   Private Sub zzAdd(ByVal sKey As String, iTopoID As Integer)
      If mdicBase.ContainsKey(sKey) Then
			' DMAcadExt.AcadDocument.WriteMessage("%$:" & CStr(sKey) & ":" & CStr(iTopoID) & ":" & CStr(mdicBase.Count))
			'	DMCommon.ExcelLogAX.SetNextValue(0, sKey)
			Dim oQueue As Queue(Of Integer)
         If mdicDoubleValues.ContainsKey(sKey) Then
            oQueue = mdicDoubleValues.Item(sKey)
            oQueue.Enqueue(iTopoID)
         Else
            oQueue = New Queue(Of Integer)()
            oQueue.Enqueue(mdicBase.Item(sKey))
            oQueue.Enqueue(iTopoID)
            mdicDoubleValues.Add(sKey, oQueue)
         End If
      Else
         mdicBase.Add(sKey, iTopoID)
      End If
   End Sub
   Public Sub Terminate()
      mdicBase.Clear()
      mdicBase = Nothing
      mdicDoubleValues.Clear()
      mdicDoubleValues = Nothing
   End Sub
   Public ReadOnly Property Count() As Integer
      Get
         Return mdicDoubleValues.Count
      End Get
   End Property
   Public Function GetCriteria(ByVal sFieldName As String) As String

      If mdicDoubleValues.Count <> 0 Then
         Dim sValueList As String = String.Empty

         For Each sKey As String In mdicDoubleValues.Keys
            If sValueList.Length <> 0 Then
               sValueList &= msSQLDelim
            End If
            sValueList &= sKey
         Next
         Return "(" & sFieldName & " IN (" & sValueList & "))"
      Else
         Return String.Empty
      End If
   End Function
   Public Function GetCriteria(ByVal sFieldName1 As String, ByVal sFieldName2 As String) As String
      MessageBox.Show(CStr(mdicDoubleValues.Count), "08_349")
      If mdicDoubleValues.Count <> 0 Then
         Dim sValueList1 As String = String.Empty
         Dim sValueList2 As String = String.Empty
         Dim sKey1 As String = String.Empty, sKey2 As String = String.Empty

         For Each sKey As String In mdicDoubleValues.Keys
            If sValueList1.Length <> 0 Then
               sValueList1 &= msSQLDelim
               sValueList2 &= msSQLDelim
            End If
            zzSplitKey(sKey, sKey1, sKey2)
            sValueList1 &= sKey1
            sValueList2 &= sKey2
         Next

         Return "(" & sFieldName1 & " IN (" & sValueList1 & ")" & ") AND (" & sFieldName2 & " IN (" & sValueList2 & "))"
      Else
         Return String.Empty
      End If
   End Function
   Private Function zzGetKey(ByVal sValue1 As String, ByVal sValue2 As String) As String
      Return sValue1 & msDelim & sValue2
   End Function
   Private Sub zzSplitKey(ByVal sKey As String, ByRef sKey1 As String, ByRef sKey2 As String)
      Dim saKeys() As String = Split(sKey, msDelim)
      If saKeys.GetUpperBound(0) >= 1 Then
         sKey1 = saKeys(0)
         sKey2 = saKeys(1)
      End If
   End Sub
End Class
