Option Explicit On
Option Strict On
Public Class TplnPointObject
   Public Point As TPlnPoint
   Public ThisObject As System.Object
   Public Sub New(oPoint As TPlnPoint, oThisObject As System.Object)
      Point = oPoint
      ThisObject = oThisObject
   End Sub
   Public Overrides Function ToString() As String
      Return Point.ToString() & "; " & ThisObject.ToString()
   End Function
End Class
Public Class TplnPointArray
   Implements ICollection

   Private moaPoints() As TPlnPoint
   Private moaPointObjects() As TplnPointObject

   Private miUpperBound As Integer
   Private miCurrentIndex As Integer = 0
   Public Sub New()
      miUpperBound = -1
   End Sub
   Public Sub New(oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline)
      miUpperBound = oPolyline.NumberOfVertices - 1
      ReDim moaPoints(miUpperBound)
      For iVertexIndex As Integer = 0 To miUpperBound
         moaPoints(iVertexIndex) = New TPlnPoint(oPolyline.GetPoint2dAt(iVertexIndex))
      Next
   End Sub
   Public Sub New(oLine As Autodesk.AutoCAD.DatabaseServices.Line)
      miUpperBound = 1
      ReDim moaPoints(miUpperBound)
      moaPoints(0) = New TPlnPoint(oLine.StartPoint)
      moaPoints(1) = New TPlnPoint(oLine.EndPoint)

   End Sub
   Public Sub New(oArc As Autodesk.AutoCAD.DatabaseServices.Arc)
      miUpperBound = 1
      ReDim moaPoints(miUpperBound)
      moaPoints(0) = New TPlnPoint(oArc.StartPoint)
      moaPoints(1) = New TPlnPoint(oArc.EndPoint)

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
   Public Sub AddObject(ByVal oaPointArray As TplnPointArray)
      Dim oPointObject As TplnPointObject
      Dim iStartIndex As Integer = miUpperBound + 1
      Dim iIndex As Integer
      Me.AddDimObject(oaPointArray.UpperBound)
      For iIndex = 0 To oaPointArray.UpperBound
         oPointObject = oaPointArray.ItemObject(iIndex)
         Me.AddObject(oPointObject, iStartIndex + iIndex)
      Next

   End Sub
   Public Sub Add(ByVal oaPointArray As TplnPointArray)
      If oaPointArray IsNot Nothing AndAlso oaPointArray.UpperBound >= 0 Then
         Dim oPoint As TPlnPoint
         Dim iStartIndex As Integer = miUpperBound + 1
         Dim iIndex As Integer
         Me.AddDim(oaPointArray.UpperBound)
         For iIndex = 0 To oaPointArray.UpperBound
            oPoint = oaPointArray.Item(iIndex)
            Me.Add(oPoint, iStartIndex + iIndex)
         Next
      End If
   End Sub
   Public Sub AddDimObject(ByVal iUpperBound As Integer)
      miUpperBound += iUpperBound + 1
      If iUpperBound >= 0 Then
         ReDim Preserve moaPointObjects(miUpperBound)
      End If
   End Sub
   Public Sub AddDim(ByVal iUpperBound As Integer)
      miUpperBound += iUpperBound + 1
      If iUpperBound >= 0 Then
         ReDim Preserve moaPoints(miUpperBound)
      End If
   End Sub
   Public Sub Add(ByVal oPoint As TPlnPoint, ByVal iIndex As Integer)
      moaPoints(iIndex) = oPoint

   End Sub
   Public Sub AddObject(ByVal oPointObject As TplnPointObject, ByVal iIndex As Integer)
      moaPointObjects(iIndex) = oPointObject

   End Sub
	Public Sub AddObject(ByVal oPoint As TPlnPoint, oValue As System.Object)
		miUpperBound += 1
		ReDim Preserve moaPointObjects(miUpperBound)
		moaPointObjects(miUpperBound) = New TplnPointObject(oPoint, oValue)
		If False Then


			Dim s As String
			If oValue Is Nothing Then
				s = "Nothing!!"
			Else
				s = oValue.GetType().ToString() & vbCrLf & oValue.ToString()
			End If


			System.Windows.Forms.MessageBox.Show(CStr(miUpperBound) & vbCrLf & s, "05_400")
		End If
	End Sub
	Public Sub Add(ByVal oPoint As TPlnPoint)
      miUpperBound += 1
      ReDim Preserve moaPoints(miUpperBound)
      moaPoints(miUpperBound) = oPoint

   End Sub
   Public Sub Add(ByVal tAcadPoint As Autodesk.AutoCAD.Geometry.Point2d)
      Dim oPoint As TPlnPoint = New TPlnPoint(tAcadPoint)
      Me.Add(oPoint)
   End Sub
   Public Sub Add(ByVal tAcadPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal iIndex As Integer)
      Dim oPoint As TPlnPoint = New TPlnPoint(tAcadPoint)
      Me.Add(oPoint, iIndex)
   End Sub
   Public Sub Add(ByVal tAcadPoint As Autodesk.AutoCAD.Geometry.Point3d)
      Dim oPoint As TPlnPoint = New TPlnPoint(tAcadPoint)
      Me.Add(oPoint)
   End Sub
   Public Sub Add(ByVal tAcadPoint As Autodesk.AutoCAD.Geometry.Point3d, ByVal iIndex As Integer)
      Dim oPoint As TPlnPoint = New TPlnPoint(tAcadPoint)
      Me.Add(oPoint, iIndex)
   End Sub
   Public Sub Add(ByVal tAcadPoint As Autodesk.AutoCAD.Geometry.Point3d, tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, ByVal iIndex As Integer)
      Dim oPoint As TPlnPoint = New TPlnPoint(tAcadPoint)
      oPoint.AcObjID = tAcObjID
      Me.Add(oPoint, iIndex)
   End Sub
   Public Sub Reset()
      If miUpperBound >= 0 Then
         miCurrentIndex = 0
      End If
   End Sub
   Public Function MoveNext() As Boolean
      If miUpperBound >= 0 AndAlso miCurrentIndex <= miUpperBound Then
         miCurrentIndex += 1
         Return miCurrentIndex <= miUpperBound
      Else
         Return False
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
    
   Public ReadOnly Property Array As TPlnPoint()
      Get
         Return moaPoints
      End Get
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
   Public Property ItemObject(ByVal iIndex As Integer) As TplnPointObject
      Get
         Try
            If moaPointObjects IsNot Nothing Then
               'System.Windows.Forms.MessageBox.Show(CStr(moaPointObjects.GetUpperBound(0)), "03_321b")
            Else
               System.Windows.Forms.MessageBox.Show(CStr("NOTHING"), "03_377b")
            End If
            If iIndex > moaPointObjects.GetUpperBound(0) Or (iIndex < 0) Then
               System.Windows.Forms.MessageBox.Show(CStr(iIndex) & ":" & Str(moaPointObjects.GetUpperBound(0)), "03_402b")
               Return Nothing
            Else
               Return moaPointObjects(iIndex)
            End If

         Catch oEx As Exception
            Return Nothing
         End Try

      End Get
      Set(ByVal oValue As TplnPointObject)
         Try
            moaPointObjects(iIndex) = oValue
         Catch oEx As Exception

         End Try
      End Set
   End Property

   Public Sub CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo

   End Sub

   Public ReadOnly Property Count As Integer Implements ICollection.Count
      Get
         Return miUpperBound + 1
      End Get
   End Property

   Public ReadOnly Property IsSynchronized As Boolean Implements ICollection.IsSynchronized
      Get
         Return moaPoints.IsSynchronized
      End Get
   End Property

	Public ReadOnly Property SyncRoot As System.Object Implements ICollection.SyncRoot
		Get
			Return moaPoints.SyncRoot
		End Get
	End Property

	Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
      Return moaPoints.GetEnumerator()
   End Function
End Class
