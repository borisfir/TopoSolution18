Option Explicit On 
Option Strict On
Imports Autodesk.AutoCAD
'Imports Autodesk.AutoCAD.Interop.Common
Friend Class TPlnPoint
   Implements TPlnIEntity
   Private mdsaValue As System.Array
   Private mdaValue(2) As Double
	Private moAcadPoint As Autodesk.AutoCAD.DatabaseServices.DBPoint
   Private miColor As Integer = -1
   Enum enAxis
      X
      Y
      Z
   End Enum
   Sub New()

   End Sub
   Public Sub New(ByVal dX As Double, ByVal dY As Double)
      mdaValue(0) = dX
      mdaValue(1) = dY

   End Sub
   Sub New(ByVal daValue() As Double)
      mdaValue(0) = daValue(0)
      mdaValue(1) = daValue(1)
   End Sub
   Sub New(ByVal oPoint As Object)
      mdsaValue = CType(oPoint, System.Array)
      mdaValue(0) = DirectCast(mdsaValue.GetValue(0), Double)
      mdaValue(1) = DirectCast(mdsaValue.GetValue(1), Double)

   End Sub
   Sub New(ByVal oPoint1 As TPlnPoint, ByVal oPoint2 As TPlnPoint)
      mdaValue(0) = 0.5 * (oPoint1.X + oPoint2.X)
      mdaValue(1) = 0.5 * (oPoint1.Y + oPoint2.Y)

   End Sub
   Sub New(ByVal sXY As String)
      Dim saXY() As String = Strings.Split(sXY, ",")
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

   End Sub
#Region "Implements AcadEntity"
	Public ReadOnly Property AcEntity() As Autodesk.AutoCAD.DatabaseServices.Entity Implements TPlnIEntity.AcEntity


		Get
			If Me.EntityExists Then
				Return DirectCast(moAcadPoint, Autodesk.AutoCAD.DatabaseServices.Entity)
			Else
				Return Nothing
			End If

		End Get

	End Property

	Public ReadOnly Property AcObjID() As Autodesk.AutoCAD.DatabaseServices.ObjectId Implements TPlnIEntity.AcObjID
		Get
			If Me.EntityExists Then
				Return moAcadPoint.ObjectId
			Else
				Return New Autodesk.AutoCAD.DatabaseServices.ObjectId()
			End If

		End Get

	End Property
   Public Sub InsertToModelSpace() Implements TPlnIEntity.InsertToModelSpace
      zzInsertToModelSpace()
   End Sub
   Public ReadOnly Property Coordinate() As String
      Get
         Return mdaValue(0).ToString & "," & mdaValue(1).ToString
      End Get
   End Property
   Public Property Color() As Integer Implements TPlnIEntity.Color
      Get
         If Me.EntityExists Then
				Return CType(moAcadPoint.Color.ColorIndex, Integer)
         Else
            Return miColor
         End If

      End Get
      Set(ByVal iValue As Integer)
         If Me.EntityExists Then
				moAcadPoint.Color = moAcadPoint.Color
         Else
            miColor = iValue
         End If

      End Set
   End Property
   Public Property Layer() As String Implements TPlnIEntity.Layer
      Get
         Return moAcadPoint.Layer
      End Get
      Set(ByVal sValue As String)
         moAcadPoint.Layer = sValue
      End Set
   End Property
   Public ReadOnly Property EntityExists() As Boolean Implements TPlnIEntity.EntityExists
      Get
         Return moAcadPoint IsNot Nothing
      End Get
   End Property
   Public Sub EraseZ()
      mdaValue(2) = 0.0#
   End Sub
	Public Sub Delete() Implements TPlnIEntity.Delete
		If Me.EntityExists Then
			moAcadPoint.Erase()
		End If

	End Sub
#End Region
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
   Public Property Value(ByVal iAxis As enAxis) As Double
      Get
         Return mdaValue(iAxis)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(iAxis) = dValue
      End Set
   End Property
   Public Property Value(ByVal iAxis As Integer) As Double
      Get
         Return mdaValue(iAxis)
      End Get
      Set(ByVal dValue As Double)
         mdaValue(iAxis) = dValue
      End Set
   End Property
   Public Sub Move(ByVal dX As Double, ByVal dY As Double)
      mdaValue(0) += dX
      mdaValue(1) += dY
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
   Public Function GetClone() As TPlnPoint
      Return New TPlnPoint(mdaValue)
	End Function
	Public Function GetAcadPoint() As Autodesk.AutoCAD.Geometry.Point3d
		Return New Autodesk.AutoCAD.Geometry.Point3d(mdaValue)
	End Function
	Public Function GetMoved(ByVal dX As Double, ByVal dY As Double) As TPlnPoint
		Return New TPlnPoint(mdaValue(0) + dX, mdaValue(1) + dY)
	End Function
   Public Sub SetBy(ByVal oPoint As TPlnPoint)
      mdaValue(0) = oPoint.X
      mdaValue(1) = oPoint.Y

   End Sub
   Public Sub Min(ByVal oAddPoint As TPlnPoint)
      Try
         mdaValue(0) = Math.Min(mdaValue(0), oAddPoint.X)
         mdaValue(1) = Math.Min(mdaValue(1), oAddPoint.Y)
      Catch ex As Exception
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
   Public Shared Function OtherFlatAxis(ByVal iAxis As enAxis) As enAxis
      Select Case iAxis
         Case enAxis.X
            Return enAxis.Y
         Case enAxis.Y
            Return enAxis.X
      End Select
   End Function

   Private Sub zzInsertToModelSpace()
		Dim tWinColor As System.Drawing.Color = System.Drawing.Color.FromArgb(miColor)
		''	moAcadPoint = ThisDrawing.ModelSpace().AddPoint(mdaValue)
		If miColor <> -1 Then

			moAcadPoint.Color = Colors.Color.FromColor(tWinColor)
		End If
   End Sub
End Class
