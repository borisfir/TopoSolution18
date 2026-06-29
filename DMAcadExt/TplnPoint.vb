
Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Public Class TPlnPoint
   Const msListDelim As String = ","
   Private mdsaValue As System.Array
   Private mdaValue(2) As Double

   Private miColor As Integer = -1
   Private mbIs3D As Boolean
   Private mtAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
   Enum enAxis
      X
      Y
      Z
   End Enum
   Sub New()

   End Sub
#Region "Autodesk.AutoCAD"

   Public Sub New(ByVal tPoint2d As Autodesk.AutoCAD.Geometry.Point2d)
      mdaValue(0) = tPoint2d.X
      mdaValue(1) = tPoint2d.Y
      mbIs3D = False
   End Sub
   Public Sub New(ByVal tPoint3d As Autodesk.AutoCAD.Geometry.Point3d, Optional ByVal bTo2d As Boolean = False)
      Try
         mdaValue(0) = tPoint3d.X
         mdaValue(1) = tPoint3d.Y
         If bTo2d Then
            mbIs3D = False
         Else
            mbIs3D = True
            mdaValue(2) = tPoint3d.Z
         End If
      Catch oEx As Exception
      End Try
   End Sub
   Public Sub New(ByVal oDBPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint)
      Me.New(oDBPoint.Position)
      mtAcObjID = oDBPoint.ObjectId
   End Sub
   


   Public Sub Terminate()
      Erase mdsaValue
      Erase mdaValue
   End Sub
   Public Property AcObjID() As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Get
         Return mtAcObjID
      End Get
      Set(ByVal tValue As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         mtAcObjID = tValue
      End Set
   End Property
   Public Property AcGePoint() As Autodesk.AutoCAD.Geometry.Point2d
      Get
         Return New Autodesk.AutoCAD.Geometry.Point2d(mdaValue(0), mdaValue(1))
      End Get
      Set(ByVal oValue As Autodesk.AutoCAD.Geometry.Point2d)
         mdaValue(0) = oValue.X
         mdaValue(1) = oValue.Y
         mdaValue(2) = 0.0
      End Set
   End Property
   Public Property AcGePoint3d() As Autodesk.AutoCAD.Geometry.Point3d
      Get
         Return New Autodesk.AutoCAD.Geometry.Point3d(mdaValue(0), mdaValue(1), mdaValue(2))
      End Get
      Set(ByVal oValue As Autodesk.AutoCAD.Geometry.Point3d)
         mdaValue(0) = oValue.X
         mdaValue(1) = oValue.Y
         mdaValue(2) = oValue.Z
      End Set
   End Property
   Public Function GetPoint2d() As Autodesk.AutoCAD.Geometry.Point2d
      Return New Autodesk.AutoCAD.Geometry.Point2d(mdaValue(0), mdaValue(1))
   End Function
   Public Function GetPoint3d() As Autodesk.AutoCAD.Geometry.Point3d
      Return New Autodesk.AutoCAD.Geometry.Point3d(mdaValue(0), mdaValue(1), mdaValue(2))
   End Function
#End Region
   Public Sub New(ByVal dX As Double, ByVal dY As Double)
      mdaValue(0) = dX
      mdaValue(1) = dY
      mbIs3D = False
   End Sub
   Sub New(ByVal daValue() As Double)
      mdaValue(0) = daValue(0)
      mdaValue(1) = daValue(1)
      mbIs3D = False
   End Sub
	Sub New(ByVal oPoint As System.Object)
		mdsaValue = CType(oPoint, System.Array)
		mdaValue(0) = DirectCast(mdsaValue.GetValue(0), Double)
		mdaValue(1) = DirectCast(mdsaValue.GetValue(1), Double)
		mbIs3D = False
	End Sub
	Sub New(ByVal oPoint1 As TPlnPoint, ByVal oPoint2 As TPlnPoint)
      mdaValue(0) = 0.5 * (oPoint1.X + oPoint2.X)
      mdaValue(1) = 0.5 * (oPoint1.Y + oPoint2.Y)
      mbIs3D = False
   End Sub
   Sub New(ByVal oPoint1 As Point2d, ByVal oPoint2 As Point2d)
      mdaValue(0) = 0.5 * (oPoint1.X + oPoint2.X)
      mdaValue(1) = 0.5 * (oPoint1.Y + oPoint2.Y)
      mbIs3D = False
   End Sub
   Sub New(ByVal oPoint1 As Point3d, ByVal oPoint2 As Point3d)
      mdaValue(0) = 0.5 * (oPoint1.X + oPoint2.X)
      mdaValue(1) = 0.5 * (oPoint1.Y + oPoint2.Y)
      mdaValue(2) = 0.5 * (oPoint1.Z + oPoint2.Z)

      mbIs3D = True
   End Sub

   Sub New(ByVal sXY As String)
      Dim saXY() As String = Strings.Split(sXY, msListDelim)
      If saXY.GetUpperBound(0) > 0 Then
         mdaValue(0) = CType(saXY(0), Double)
         mdaValue(1) = CType(saXY(1), Double)
      End If
   End Sub
   Sub New(ByVal oXMLElem As Xml.XmlNode)
      Dim oXmlAttribute As Xml.XmlAttribute
      Dim oXmlAttributeCollection As Xml.XmlAttributeCollection = oXMLElem.Attributes

      For iIndex As Integer = 0 To 1
         oXmlAttribute = DirectCast(oXmlAttributeCollection.Item(iIndex), Xml.XmlAttribute)
         mdaValue(iIndex) = CType(oXmlAttribute.Value, Double)
      Next
      mbIs3D = False
   End Sub

   Public Property X() As Double
      Get
         Return mdaValue(enAxis.X)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(enAxis.X) = dValue
      End Set
   End Property
   Public Property Y() As Double
      Get
         Return mdaValue(enAxis.Y)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(enAxis.Y) = dValue
      End Set
   End Property
   Public Property Z() As Double
      Get
         Return mdaValue(enAxis.Z)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(enAxis.Z) = dValue
         mbIs3D = True
      End Set
   End Property
   Public Property XYZ() As Double()
      Get
         Return mdaValue
      End Get
      Set(ByVal dValue As Double())
         mdaValue = dValue
      End Set
   End Property
   Public Property XY() As Double()
      Get
         Dim mda2DValue(1) As Double
         mda2DValue(0) = mdaValue(0)
         mda2DValue(1) = mdaValue(1)
         Return mda2DValue
      End Get
      Set(ByVal dValue As Double())
         mdaValue(0) = dValue(0)
         mdaValue(1) = dValue(1)

      End Set
   End Property
   Public ReadOnly Property Coordinates2d() As String
      Get
         Dim sOut As String
         Try
            sOut = mdaValue(enAxis.X).ToString() & msListDelim & mdaValue(enAxis.Y).ToString()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TPlnPoint - Coordinates")
            sOut = String.Empty
         End Try

         Return sOut
      End Get
   End Property

   Public ReadOnly Property Coordinates() As String
      Get
         Dim sOut As String
         Try
            sOut = mdaValue(enAxis.X).ToString() & msListDelim & mdaValue(enAxis.Y).ToString()
            If mbIs3D Then
               sOut &= msListDelim & mdaValue(enAxis.Z).ToString()
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TPlnPoint - Coordinates")
            sOut = String.Empty
         End Try

         Return sOut
      End Get
   End Property
   Public Property Value(ByVal iAxis As enAxis) As Double
      Get
         Return mdaValue(iAxis)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(iAxis) = dValue
      End Set
   End Property
   Public ReadOnly Property PointKey As ULong
      Get
         Return DMAcadExt.TplnPointKeyLong.CoordToKey(mdaValue(0), mdaValue(1))
      End Get
   End Property
   Public Property Value(ByVal iAxis As Integer) As Double
      Get
         Return mdaValue(iAxis)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(iAxis) = dValue
      End Set
   End Property
   Public Function IsEqualTo(oPoint As TPlnPoint, dTolearance As Double) As Boolean
      If DMCommon.Functions.CheckDeviation(Me.X, oPoint.X, dTolearance) Then
         If DMCommon.Functions.CheckDeviation(Me.Y, oPoint.Y, dTolearance) Then
            'If DMCommon.Functions.CheckDeviation(Me.Z, oPoint.Z, dTolearance) Then
            Return True
            'End If
         End If
      Else
         '		System.Windows.Forms.MessageBox.Show(CStr(Me.X) & vbCrLf & CStr(oPoint.X), "01_136_Point_X")
      End If
      '	System.Windows.Forms.MessageBox.Show(Me.Coordinates & vbCrLf & oPoint.Coordinates, "01_136_Point")
      Return False
   End Function
   Public Sub Move(ByVal dX As Double, ByVal dY As Double)
      mdaValue(0) += dX
      mdaValue(1) += dY
   End Sub
   Public Sub Move(ByVal oAddPoint As TPlnPoint)
      mdaValue(0) += oAddPoint.X
      mdaValue(1) += oAddPoint.Y
   End Sub
   Public Sub Move(ByVal oBasePoint As TPlnPoint, ByVal dXFactor As Double, ByVal dYFactor As Double)
      mdaValue(0) = oBasePoint.X + ((mdaValue(0) - oBasePoint.X) * dXFactor)
      mdaValue(1) = oBasePoint.Y + ((mdaValue(1) - oBasePoint.Y) * dYFactor)
   End Sub
   Public Sub Scale(ByVal dScale As Double)
      mdaValue(0) *= dScale
      mdaValue(1) *= dScale
   End Sub
   Public Sub Scale(ByVal dScaleX As Double, ByVal dScaleY As Double)
      mdaValue(0) *= dScaleX
      mdaValue(1) *= dScaleY
   End Sub
   Public Function GetSubstract(oPoint As TPlnPoint) As Vector3d

      Return New Vector3d(oPoint.X - Me.X, oPoint.Y - Me.Y, oPoint.Z - Me.Z)
   End Function

   Public Function GetClone() As TPlnPoint
      Return New TPlnPoint(mdaValue)
   End Function
   Public Function GetMoved(ByVal dX As Double, ByVal dY As Double) As TPlnPoint
      Return New TPlnPoint(mdaValue(0) + dX, mdaValue(1) + dY)
   End Function
   Public Function GetScaled(ByVal dScaleX As Double, ByVal dScaleY As Double) As TPlnPoint
      Return New TPlnPoint(mdaValue(0) * dScaleX, mdaValue(1) * dScaleY)
   End Function
   Public Function GetScaled(ByVal dScale As Double) As TPlnPoint
      Return New TPlnPoint(mdaValue(0) * dScale, mdaValue(1) * dScale)
   End Function
   Public Sub SetBy(ByVal oPoint As TPlnPoint)
      mdaValue(0) = oPoint.X
      mdaValue(1) = oPoint.Y

   End Sub
   Public Shared Function GetEntity2dCenter(oEntity As Autodesk.AutoCAD.DatabaseServices.Entity) As TPlnPoint
      Dim tExtents3d As Autodesk.AutoCAD.DatabaseServices.Extents3d
      Return New TPlnPoint(New TPlnPoint(tExtents3d.MinPoint), New TPlnPoint(tExtents3d.MaxPoint))
   End Function
   Public Shared Function RoundPoint(tPoint3d As Point3d, Optional iDigits As Integer = 0) As Point3d
      Dim daRoundedValues(2) As Double
      Dim dInterimValue As Double
      For iIndex As Integer = 0 To 2
         dInterimValue = Math.Round(tPoint3d.Coordinate(iIndex), 6, MidpointRounding.AwayFromZero)
         daRoundedValues(iIndex) = Math.Round(dInterimValue, iDigits, MidpointRounding.AwayFromZero)
      Next
      Return New Point3d(daRoundedValues)
   End Function
   Public Shared Function RoundPoint(tPoint2d As Point2d, Optional iDigits As Integer = 0) As Point3d
      Dim daRoundedValues(2) As Double
      Dim dInterimValue As Double
      For iIndex As Integer = 0 To 1
         dInterimValue = Math.Round(tPoint2d.Coordinate(iIndex), 6, MidpointRounding.AwayFromZero)
         daRoundedValues(iIndex) = Math.Round(dInterimValue, iDigits, MidpointRounding.AwayFromZero)
      Next
      Return New Point3d(daRoundedValues)
   End Function
   Public Shared Function RoundPointTo2d(tPoint3d As Point3d, Optional iDigits As Integer = 0) As Point2d
      Dim daRoundedValues(1) As Double
      For iIndex As Integer = 0 To 1
         daRoundedValues(iIndex) = Math.Round(tPoint3d.Coordinate(iIndex), iDigits, MidpointRounding.AwayFromZero)
      Next
      Return New Point2d(daRoundedValues)
   End Function
   Public Shared Function RoundPointTo2d(tPoint2d As Point2d, Optional iDigits As Integer = 0) As Point2d
      Dim daRoundedValues(1) As Double
      Dim dInterimValue As Double
      For iIndex As Integer = 0 To 1
         dInterimValue = Math.Round(tPoint2d.Coordinate(iIndex), 6, MidpointRounding.AwayFromZero)
         daRoundedValues(iIndex) = Math.Round(dInterimValue, iDigits, MidpointRounding.AwayFromZero)
      Next
      Return New Point2d(daRoundedValues)
   End Function
   Public Shared Function RoundPoint(ByRef oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference, Optional iDigits As Integer = 0) As Point3d
      oBlockRef.Position = RoundPoint(oBlockRef.Position, iDigits)


   End Function

   Public Shared Operator =(ByVal oPoint1 As TPlnPoint, ByVal oPoint2 As TPlnPoint) As Boolean
      If oPoint1 Is Nothing OrElse oPoint2 Is Nothing Then
         Return False
      Else
         Return (oPoint1.X = oPoint2.X) AndAlso (oPoint1.Y = oPoint2.Y)
      End If

   End Operator

   Public Shared Operator <>(ByVal oPoint1 As TPlnPoint, ByVal oPoint2 As TPlnPoint) As Boolean
      Return Not (oPoint1 = oPoint2)
   End Operator
   Public Sub Min(ByVal oAddPoint As TPlnPoint)
      Try
         mdaValue(0) = Math.Min(mdaValue(0), oAddPoint.X)
         mdaValue(1) = Math.Min(mdaValue(1), oAddPoint.Y)
      Catch oEx As Exception
         Stop
      End Try


   End Sub
   Public Sub Max(ByVal oAddPoint As TPlnPoint)
      mdaValue(0) = Math.Max(mdaValue(0), oAddPoint.X)
      mdaValue(1) = Math.Max(mdaValue(1), oAddPoint.Y)

   End Sub
   Public Function Distance(ByVal oPoint As TPlnPoint) As Double
      Dim dDeltaX As Double = mdaValue(0) - oPoint.X
      Dim dDeltaY As Double = mdaValue(1) - oPoint.Y
      Return Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY)


   End Function
   Public Function Distance(ByVal dX As Double, ByVal dY As Double) As Double
      Dim dDeltaX As Double = mdaValue(0) - dX
      Dim dDeltaY As Double = mdaValue(1) - dY
      Return Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY)


   End Function
   Public Function GetIntPoint() As System.Drawing.Point
      Return New System.Drawing.Point(CInt(Me.X), CInt(Me.Y))
   End Function
   Public Function GetXFormated(iNumDigits As Integer) As String

      Return FormatNumber(mdaValue(0), iNumDigits, , , TriState.False)

   End Function
   Public Function GetYFormated(iNumDigits As Integer) As String

      Return FormatNumber(mdaValue(1), iNumDigits, , , TriState.False)

   End Function
   Public Function Deviation(ByVal oPoint As TPlnPoint) As Double
      Dim daDeviation(2) As Double
      For iIndex As Integer = 0 To 2
         daDeviation(iIndex) = Math.Abs(mdaValue(iIndex) - oPoint.Value(iIndex))
      Next
   End Function
   Public Function InNeighborhood(ByVal oPoint As TPlnPoint, ByVal dTolerance As Double) As Boolean
      Dim bResp As Boolean = True
      For iIndex As Integer = 0 To 2
         If Math.Abs(mdaValue(iIndex) - oPoint.Value(iIndex)) > dTolerance Then
            bResp = False
            Exit For
         End If
      Next
      Return bResp
   End Function
   Public Shared Function GetMiddlePoint(ByVal oPointA As TPlnPoint, ByVal oPointB As TPlnPoint) As TPlnPoint

      Return New TPlnPoint(0.5 * (oPointA.X + oPointB.X), 0.5 * (oPointA.Y + oPointB.Y))
   End Function
   Public Function GetMiddlePoint(ByVal oPoint As TPlnPoint) As TPlnPoint
      Return New TPlnPoint(0.5 * (mdaValue(0) + oPoint.X), 0.5 * (mdaValue(1) + oPoint.Y))


   End Function
   Public Sub FillXMLElement(ByVal oXMLElem As Xml.XmlElement)
      oXMLElem.SetAttribute("X", mdaValue(enAxis.X).ToString)
      oXMLElem.SetAttribute("Y", mdaValue(enAxis.Y).ToString)
   End Sub
   Public Overrides Function ToString() As String

      Return "(" & Me.Coordinates & ")"
   End Function
   Public Shared Function Point2dTo3d(ByVal tPoint2d As Point2d) As Point3d
      Return New Point3d(tPoint2d.X, tPoint2d.Y, 0.0)
   End Function
   Public Shared Function Point3dTo2d(ByVal tPoint3d As Point3d) As Point2d
      Return New Point2d(tPoint3d.X, tPoint3d.Y)
   End Function

	Public Shared Function DispPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point3d) As String
      If tPoint.Z = 0.0 Then
         'Return Convert.ToDecimal(oPoint.X) & msListDelim & Convert.ToDecimal(oPoint.Y)
         Return Convert.ToString(tPoint.X) & msListDelim & Convert.ToString(tPoint.Y)
      Else
         'Return Convert.ToDecimal(oPoint.X) & msListDelim & Convert.ToDecimal(oPoint.Y) & msListDelim & Convert.ToDecimal(oPoint.Z)
         Return Convert.ToString(tPoint.X) & msListDelim & Convert.ToString(tPoint.Y) & msListDelim & Convert.ToString(tPoint.Z)
      End If
   End Function
   Public Shared Function EqPointsFormat(ByVal oPoint1 As Autodesk.AutoCAD.Geometry.Point2d, ByVal oPoint2 As Autodesk.AutoCAD.Geometry.Point2d) As Boolean
      Dim sX1 As String = Convert.ToString(oPoint1.X)
      Dim sY1 As String = Convert.ToString(oPoint1.Y)
      Dim sX2 As String = Convert.ToString(oPoint2.X)
      Dim sY2 As String = Convert.ToString(oPoint2.Y)

      Return (sX1 = sX2) AndAlso (sY1 = sY2)

   End Function
	Public Shared Function IsEqualPoint(ByVal oPoint1 As Autodesk.AutoCAD.Geometry.Point2d, ByVal oPoint2 As Autodesk.AutoCAD.Geometry.Point2d) As Boolean


		Return (oPoint1.X = oPoint2.X) AndAlso (oPoint1.Y = oPoint2.Y)

	End Function
	Public Shared Function IsEqualPoint(ByVal oPoint1 As Autodesk.AutoCAD.Geometry.Point3d, ByVal oPoint2 As Autodesk.AutoCAD.Geometry.Point3d) As Boolean


		Return (oPoint1.X = oPoint2.X) AndAlso (oPoint1.Y = oPoint2.Y)

	End Function
	Public Shared Function IsEqualPoint(ByVal oPoint1 As Autodesk.AutoCAD.Geometry.Point2d, ByVal oPoint2 As Autodesk.AutoCAD.Geometry.Point3d) As Boolean


		Return (oPoint1.X = oPoint2.X) AndAlso (oPoint1.Y = oPoint2.Y)

	End Function
	Public Shared Function IsEqualPoint(ByVal oPoint1 As Autodesk.AutoCAD.Geometry.Point3d, ByVal oPoint2 As Autodesk.AutoCAD.Geometry.Point2d) As Boolean


		Return (oPoint1.X = oPoint2.X) AndAlso (oPoint1.Y = oPoint2.Y)

	End Function
	Public Shared Function GetDistance(tPointA As Point2d, tPointB As Point2d) As Double

		Return Math.Max(Math.Abs(tPointA.X - tPointB.X), Math.Abs(tPointA.Y - tPointB.Y))
	End Function


	Public Shared Function DispPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point2d) As String
      '	Return Convert.ToDecimal(oPoint.X) & msListDelim & Convert.ToDecimal(oPoint.Y)
      Return Convert.ToString(tPoint.X) & msListDelim & Convert.ToString(tPoint.Y)

   End Function


   Public Shared Function DispVector(ByVal tVector As Vector2d, ByVal sTitle As String) As String
      DMAcadExt.AcadDocument.WriteMessage(sTitle & ": " & "Angle=" & CStr(tVector.Angle) & "; Len=" & CStr(tVector.Length))
      Return Convert.ToString(tVector.X) & msListDelim & Convert.ToString(tVector.Y)

   End Function


   Public Shared Function OtherFlatAxis(ByVal iAxis As enAxis) As enAxis
      Select Case iAxis
         Case enAxis.X
            Return enAxis.Y
         Case enAxis.Y
            Return enAxis.X
      End Select
   End Function
End Class

