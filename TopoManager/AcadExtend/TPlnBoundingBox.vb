Option Explicit On
Option Strict On

Public Class TPlnBoundingBox
   Private mtoMinPoint, mtoMaxPoint As TPlnPoint
   Private mbEmpty As Boolean = True

   Public Sub New(ByVal oExtents As Autodesk.AutoCAD.DatabaseServices.Extents2d)

      Try
         mtoMinPoint = New TPlnPoint(oExtents.MinPoint.X, oExtents.MinPoint.Y)
         mtoMaxPoint = New TPlnPoint(oExtents.MaxPoint.X, oExtents.MaxPoint.Y)
      Catch oEx As Exception
         Exit Sub
      End Try
      mbEmpty = False
   End Sub
   Public Sub New(ByVal oExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d)

      Try
         mtoMinPoint = New TPlnPoint(oExtents.MinPoint.X, oExtents.MinPoint.Y)
         mtoMaxPoint = New TPlnPoint(oExtents.MaxPoint.X, oExtents.MaxPoint.Y)
      Catch oEx As Exception
         Exit Sub
      End Try
      mbEmpty = False
   End Sub
   Public Sub New(ByVal oMinPoint As TPlnPoint, ByVal oMaxPoint As TPlnPoint)
      mtoMinPoint = New TPlnPoint(Math.Min(oMinPoint.X, oMaxPoint.X), Math.Min(oMinPoint.Y, oMaxPoint.Y))
      mtoMaxPoint = New TPlnPoint(Math.Max(oMinPoint.X, oMaxPoint.X), Math.Max(oMinPoint.Y, oMaxPoint.Y))
      mbEmpty = False
   End Sub
   Public Sub New()
		mbEmpty = True
	End Sub
	Public ReadOnly Property Coordinates() As String
		Get
			
			If mbEmpty Then
				Return "Empty"
			Else
				Return "(" & mtoMinPoint.Coordinates & ") ; (" & mtoMaxPoint.Coordinates & ")"
			End If


		End Get
	End Property
   Public ReadOnly Property MinPoint() As TPlnPoint
      Get
         Return mtoMinPoint
      End Get
   End Property
   Public ReadOnly Property MaxPoint() As TPlnPoint
      Get
         Return mtoMaxPoint
      End Get
   End Property
   Public Sub Union(ByVal toAddBoundingBox As TPlnBoundingBox)
      If mbEmpty Then
         mtoMinPoint = toAddBoundingBox.MinPoint
         mtoMaxPoint = toAddBoundingBox.MaxPoint
         mbEmpty = False
      Else
         Try
            Dim toAddMinPoint As TPlnPoint = toAddBoundingBox.MinPoint
            Dim toAddMaxPoint As TPlnPoint = toAddBoundingBox.MaxPoint
            mtoMinPoint.Min(toAddMinPoint)
            mtoMaxPoint.Max(toAddMaxPoint)
         Catch oEx As Exception
            MessageBox.Show(oEx.Message, "TPlnBoundingBox - Union")
         End Try

      End If

   End Sub
   Public Sub Union(ByVal oPoint As TPlnPoint)
      If mbEmpty Then
         mtoMinPoint = oPoint.GetClone()
         mtoMaxPoint = oPoint.GetClone()
         mbEmpty = False
      Else
         mtoMinPoint.Min(oPoint)
         mtoMaxPoint.Max(oPoint)
      End If

   End Sub
   Public Sub AbsScale(ByVal dScale As Double)
      mtoMinPoint.Scale(dScale)
      mtoMaxPoint.Scale(dScale)
   End Sub
   Public Sub AbsScale(ByVal dScaleX As Double, ByVal dScaleY As Double)
      mtoMinPoint.Scale(dScaleX, dScaleY)
      mtoMaxPoint.Scale(dScaleX, dScaleY)

   End Sub
   Public Sub Scale(ByVal dScale As Double)
      Scale(dScale, dScale)
   End Sub
   Public Sub Scale(ByVal dScaleX As Double, ByVal dScaleY As Double)
      Dim oCenter As TPlnPoint = GetCenterPoint()
      mtoMinPoint.Move(oCenter, dScaleX, dScaleY)
      mtoMaxPoint.Move(oCenter, dScaleX, dScaleY)

   End Sub
   Public Sub Expand(ByVal dValueX As Double, ByVal dValueY As Double)
      mtoMaxPoint.Move(dValueX, dValueY)
      mtoMinPoint.Move(-dValueX, -dValueY)
   End Sub
   Public Sub Expand(ByVal dValue As Double)
      Me.Expand(dValue, dValue)
   End Sub
   Public Function GetVerticesList() As Double()
      Dim oVerticesList(7) As Double
      oVerticesList(0) = mtoMinPoint.X
      oVerticesList(1) = mtoMinPoint.Y

      oVerticesList(2) = mtoMinPoint.X
      oVerticesList(3) = mtoMaxPoint.Y

      oVerticesList(4) = mtoMaxPoint.X
      oVerticesList(5) = mtoMaxPoint.Y

      oVerticesList(6) = mtoMaxPoint.X
      oVerticesList(7) = mtoMinPoint.Y
      Return oVerticesList
	End Function
	Public ReadOnly Property Empty() As Boolean
		Get
			Return mbEmpty
		End Get
	End Property
   Public Property Width() As Double
      Get
         Return Size(TPlnPoint.enAxis.X)
      End Get
      Set(ByVal dValue As Double)
         Size(TPlnPoint.enAxis.X) = dValue
      End Set
   End Property
   Public Property Height() As Double
      Get
         Return Size(TPlnPoint.enAxis.Y)
      End Get
      Set(ByVal dValue As Double)
         Size(TPlnPoint.enAxis.Y) = dValue
      End Set
	End Property
	Public ReadOnly Property AcGePoint(ByVal bXMin As Boolean, ByVal bYMin As Boolean) As Autodesk.AutoCAD.Geometry.Point2d
		Get
			If bXMin And bYMin Then
				Return mtoMinPoint.AcGePoint
			ElseIf bXMin And Not bYMin Then
				Return New Autodesk.AutoCAD.Geometry.Point2d(mtoMinPoint.X, mtoMaxPoint.Y)
			ElseIf Not bXMin And bYMin Then
				Return New Autodesk.AutoCAD.Geometry.Point2d(mtoMaxPoint.X, mtoMinPoint.Y)
			ElseIf Not bXMin And Not bYMin Then
				Return mtoMaxPoint.AcGePoint
			End If
		End Get
	End Property
   Public Property Size(ByVal iAxis As TPlnPoint.enAxis) As Double
      Get
         Return mtoMaxPoint.Value(iAxis) - mtoMinPoint.Value(iAxis)
      End Get
      Set(ByVal dValue As Double)
         mtoMaxPoint.Value(iAxis) = mtoMinPoint.Value(iAxis) + dValue
      End Set
   End Property
   Public ReadOnly Property Middle(ByVal iAxis As TPlnPoint.enAxis) As Double
      Get
         If Not mbEmpty Then
            Return (mtoMaxPoint.Value(iAxis) + mtoMinPoint.Value(iAxis)) * 0.5
         End If
      End Get

   End Property
   Public Sub Resize(ByVal dFactor As Double)
      If Not mbEmpty Then
         Dim oCenterPoint As TPlnPoint = Me.GetCenterPoint()
         '   mtoMinPoint.Value() = oCenterPoint.Value() + (mtoMinPoint.Value() - oCenterPoint.Value()) * iFactor
         zzResize(dFactor, TPlnPoint.enAxis.X, oCenterPoint, mtoMinPoint)
         zzResize(dFactor, TPlnPoint.enAxis.Y, oCenterPoint, mtoMinPoint)
         zzResize(dFactor, TPlnPoint.enAxis.X, oCenterPoint, mtoMaxPoint)
         zzResize(dFactor, TPlnPoint.enAxis.Y, oCenterPoint, mtoMaxPoint)
      End If
   End Sub
   Private Sub zzResize(ByVal dFactor As Double, ByVal iAxis As TPlnPoint.enAxis _
   , ByRef oCenterPoint As TPlnPoint, ByRef oCornerPoint As TPlnPoint)
      If Not mbEmpty Then
         oCornerPoint.Value(iAxis) = oCenterPoint.Value(iAxis) + (oCornerPoint.Value(iAxis) - oCenterPoint.Value(iAxis)) * dFactor
      End If
   End Sub

   Public Sub SetRelationByIncrease(ByVal dRelation As Double)

      Select Case GetRelation() - dRelation
         Case Is < 0
            Size(TPlnPoint.enAxis.Y) = Size(TPlnPoint.enAxis.X) * dRelation
         Case Is > 0
            Size(TPlnPoint.enAxis.X) = Size(TPlnPoint.enAxis.X) / dRelation
      End Select
   End Sub
   Public Function GetRelation() As Double
      If Not mbEmpty Then
         Return Size(TPlnPoint.enAxis.Y) / Size(TPlnPoint.enAxis.X)
      End If

   End Function
   Public Function GetCenterPoint() As TPlnPoint
      If mbEmpty Then
         Return Nothing
      Else
         Return New TPlnPoint(Middle(TPlnPoint.enAxis.X), Middle(TPlnPoint.enAxis.Y))
      End If

   End Function
   Public Function GetRadius() As Double
      Dim dHalfWidth As Double = Size(TPlnPoint.enAxis.X) * 0.5

      Dim dHalfHeight As Double = Size(TPlnPoint.enAxis.Y) * 0.5

      Return Math.Sqrt(dHalfWidth * dHalfWidth + dHalfHeight * dHalfHeight)

   End Function

End Class

