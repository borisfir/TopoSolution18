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
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TPlnBoundingBox - New_2")
			Return
		End Try
		mbEmpty = False
	End Sub
	Public Sub New(ByVal oExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d)

      Try
         mtoMinPoint = New TPlnPoint(oExtents.MinPoint.X, oExtents.MinPoint.Y)
         mtoMaxPoint = New TPlnPoint(oExtents.MaxPoint.X, oExtents.MaxPoint.Y)
      Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TPlnBoundingBox - New_3")
			Return
      End Try
      mbEmpty = False
   End Sub
	Public Sub New(ByVal oPoint1 As TPlnPoint, ByVal oPoint2 As TPlnPoint)
		mtoMinPoint = New TPlnPoint(Math.Min(oPoint1.X, oPoint2.X), Math.Min(oPoint1.Y, oPoint2.Y))
		mtoMaxPoint = New TPlnPoint(Math.Max(oPoint1.X, oPoint2.X), Math.Max(oPoint1.Y, oPoint2.Y))
		mbEmpty = False
	End Sub
	Public Sub New(ByVal oPoint As TPlnPoint)
		mtoMinPoint = New TPlnPoint(oPoint.X, oPoint.Y)
		mtoMaxPoint = New TPlnPoint(oPoint.X, oPoint.Y)
		mbEmpty = False
	End Sub
	Public Sub New()
		mbEmpty = True
	End Sub
	Public Sub Terminate()
		If mtoMinPoint IsNot Nothing Then
			mtoMinPoint.Terminate()
		End If
		If mtoMinPoint IsNot Nothing Then
			mtoMinPoint.Terminate()
		End If
	End Sub
   Public Sub Buffer(dValue As Double)
      If mtoMinPoint IsNot Nothing AndAlso mtoMaxPoint IsNot Nothing Then
         mtoMinPoint = New TPlnPoint(mtoMinPoint.X - dValue, mtoMinPoint.Y - dValue)
         mtoMaxPoint = New TPlnPoint(mtoMaxPoint.X + dValue, mtoMaxPoint.Y + dValue)
      End If
     
   End Sub


	Public ReadOnly Property Coordinates() As String
		Get
			Dim sRes As String
			If mbEmpty Then
				sRes = "Empty"
			Else
				If mtoMinPoint IsNot Nothing Then
					sRes = "(" & mtoMinPoint.Coordinates & ");"
				Else
					sRes = "(min-Empty) ;"
				End If
				If mtoMaxPoint IsNot Nothing Then
					sRes &= " (" & mtoMaxPoint.Coordinates & ");"
				Else
					sRes &= " (max-Empty) ;"
				End If

				'Return "(" & mtoMinPoint.Coordinates & ") ; (" & mtoMaxPoint.Coordinates & ")"
			End If
			Return sRes

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
		If toAddBoundingBox IsNot Nothing Then
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
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TPlnBoundingBox - Union")
				End Try
			End If
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
		Me.Scale(dScale, dScale)
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
	Public ReadOnly Property IsEmpty() As Boolean
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
	Public ReadOnly Property MaxSize As Double
		Get
			Return Math.Max(Width, Height)
		End Get
	End Property
	Public ReadOnly Property AcGePoint(ByVal bXMin As Boolean, ByVal bYMin As Boolean) As Autodesk.AutoCAD.Geometry.Point2d
		Get
         If bXMin And bYMin Then
            If mtoMinPoint IsNot Nothing Then
               Return mtoMinPoint.AcGePoint
            End If

         ElseIf bXMin And Not bYMin Then
            If mtoMinPoint IsNot Nothing AndAlso mtoMinPoint IsNot Nothing Then
               Return New Autodesk.AutoCAD.Geometry.Point2d(mtoMinPoint.X, mtoMaxPoint.Y)
            End If

         ElseIf Not bXMin And bYMin Then
            If mtoMinPoint IsNot Nothing AndAlso mtoMinPoint IsNot Nothing Then
               Return New Autodesk.AutoCAD.Geometry.Point2d(mtoMaxPoint.X, mtoMinPoint.Y)
            End If

         ElseIf Not bXMin And Not bYMin Then
            If mtoMinPoint IsNot Nothing Then
               Return mtoMaxPoint.AcGePoint
            End If

         End If
         Return Autodesk.AutoCAD.Geometry.Point2d.Origin
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
	Public ReadOnly Property Right() As Double
		Get
			If Not mbEmpty Then
				Return mtoMaxPoint.X
			End If
		End Get

	End Property
	Public ReadOnly Property Top() As Double
		Get
			If Not mbEmpty Then
				Return mtoMaxPoint.Y
			End If
		End Get

	End Property
	Public ReadOnly Property Left() As Double
		Get
			If Not mbEmpty Then
				Return mtoMinPoint.X
			End If
		End Get

	End Property
	Public ReadOnly Property Bottom() As Double
		Get
			If Not mbEmpty Then
				Return mtoMinPoint.Y
			End If
		End Get

	End Property

	Public Function IsEqualTo(oBoundingBox As TPlnBoundingBox, dTolearance As Double) As Boolean
		If mbEmpty AndAlso oBoundingBox.IsEmpty Then
			Return True
		ElseIf Not (mbEmpty OrElse oBoundingBox.IsEmpty) Then
			If Me.MinPoint.IsEqualTo(oBoundingBox.MinPoint, dTolearance) Then
				If Me.MaxPoint.IsEqualTo(oBoundingBox.MaxPoint, dTolearance) Then
					Return True
				End If
			End If
		End If

		'	System.Windows.Forms.MessageBox.Show(Me.Coordinates & vbCrLf & oBoundingBox.Coordinates, "01_136_B")
		Return False
	End Function
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
   Public Function GetIndexPoint(iIndex As Integer) As TPlnPoint
      If mbEmpty Then
         Return Nothing
      Else
         Select Case iIndex
            Case 0
         End Select
         Return New TPlnPoint(Middle(TPlnPoint.enAxis.X), Middle(TPlnPoint.enAxis.Y))
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
	Public Function GetClosedPolyline() As Autodesk.AutoCAD.DatabaseServices.Polyline
		Dim oNewPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline = New Autodesk.AutoCAD.DatabaseServices.Polyline(4)

		Try
			oNewPolyline.AddVertexAt(0, AcGePoint(True, True), 0.0, 0.0, 0.0)
			oNewPolyline.AddVertexAt(1, AcGePoint(True, False), 0.0, 0.0, 0.0)
			oNewPolyline.AddVertexAt(2, AcGePoint(False, False), 0.0, 0.0, 0.0)
			oNewPolyline.AddVertexAt(3, AcGePoint(False, True), 0.0, 0.0, 0.0)
			oNewPolyline.Closed = True
			AcadTransaction.AppendEntity(oNewPolyline, False)
			Return oNewPolyline
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "dmLineCleanup - StraightenArc")
			Return Nothing
		End Try
	End Function
End Class

