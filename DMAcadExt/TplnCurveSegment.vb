Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnCurveSegment

	Protected dtStartPoint As Point2d
	Protected dtEndPoint As Point2d
	Protected dtMinPoint As Point2d
	Protected dtMaxPoint As Point2d

	Protected dbHasBulge As Boolean = False
	Private ddBulge As Double
	Private mdKx As Double
	Private mdMainAngle As Double
	Protected ddVectorAngle As Double
	Protected db As Boolean
	Private mbDirX As Boolean

	Private moAcadCurve As Curve2d



   Protected Sub New()

   End Sub
   Public Overridable Function GetBulge() As Double
      Return 0
   End Function
   Public Sub New(ByVal tStartPoint As Point2d, ByVal tEndPoint As Point2d)
      dtStartPoint = tStartPoint
      dtEndPoint = tEndPoint
      Init()
   End Sub


   Public ReadOnly Property StartPoint() As Point2d
      Get
         Return dtStartPoint
      End Get
   End Property
   Public Overridable Function GetX(ByVal dY As Double) As Double
      Return dtMinPoint.X + mdKx * (dY - dtMinPoint.Y)
   End Function
   Public Overridable Function GetY(ByVal dX As Double) As Double
      Return dtMinPoint.Y + mdKx * (dX - dtMinPoint.X)
   End Function
   Public ReadOnly Property Kx() As Double
      Get
         Return mdKx
      End Get
   End Property
   Public ReadOnly Property MainAngle() As Double
      Get
         Return mdMainAngle
      End Get
   End Property
   Public ReadOnly Property HasBulge() As Boolean
      Get
         Return dbHasBulge
      End Get
   End Property

   Public ReadOnly Property DirX() As Boolean
      Get
         Return mbDirX
      End Get
   End Property

   Public ReadOnly Property EndPoint() As Point2d
      Get
         Return dtEndPoint
      End Get
   End Property
   Public ReadOnly Property MinPoint() As Point2d
      Get
         Return dtMinPoint
      End Get
   End Property
   Public ReadOnly Property MaxPoint() As Point2d
      Get
         Return dtMaxPoint
      End Get
   End Property
   Public Function GetDistanceTo(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point2d) As Double
      If moAcadCurve Is Nothing Then
         If dbHasBulge Then
            moAcadCurve = New CircularArc2d()
         Else
            moAcadCurve = New Line2d(dtStartPoint, dtEndPoint)
         End If
      End If
      Return moAcadCurve.GetDistanceTo(tPoint)
   End Function
   Public Function IsOn(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point2d) As Double
      If moAcadCurve Is Nothing Then
         If dbHasBulge Then
            moAcadCurve = New CircularArc2d()
         Else
            moAcadCurve = New Line2d(dtStartPoint, dtEndPoint)
         End If
      End If
      Dim dVal As Double
      Dim tTolerance As Tolerance = New Tolerance(1.0, 1.0)
      Dim b As Boolean = moAcadCurve.IsOn(tPoint, dVal, tTolerance)
      AcadDocument.WriteDebugMessage("IsOn=" & b.ToString() & "; Val=" & dVal.ToString())
      Return dVal
   End Function
   Protected Sub Init()
      Dim tVector As Vector2d = New Vector2d(dtEndPoint.X - dtStartPoint.X, dtEndPoint.Y - dtStartPoint.Y)
      ddVectorAngle = tVector.Angle
      Select Case ddVectorAngle
         Case Is < 0.25 * Math.PI, Is > 1.75 * Math.PI
            dtMinPoint = dtStartPoint
            dtMaxPoint = dtEndPoint
            mbDirX = True
            mdMainAngle = tVector.Angle
            db = True
         Case 0.25 * Math.PI To 0.75 * Math.PI
            dtMinPoint = dtStartPoint
            dtMaxPoint = dtEndPoint
            mbDirX = False
            mdMainAngle = tVector.Angle - 0.5 * Math.PI
            db = False
         Case 0.75 * Math.PI To 1.25 * Math.PI
            dtMinPoint = dtEndPoint
            dtMaxPoint = dtStartPoint
            mbDirX = True
            mdMainAngle = tVector.Angle - Math.PI
            db = False
         Case 1.25 * Math.PI To 1.75 * Math.PI
            dtMinPoint = dtEndPoint
            dtMaxPoint = dtStartPoint
            mbDirX = False
            mdMainAngle = tVector.Angle - 1.5 * Math.PI
            db = True
      End Select
      Dim dK As Double = 180 / Math.PI
      '	System.Windows.Forms.MessageBox.Show(CStr(tVector.Angle * dK) & ":" & CStr(mdMainAngle * dK), "24_666")

      If mbDirX Then
         mdKx = (dtMaxPoint.Y - dtMinPoint.Y) / (dtMaxPoint.X - dtMinPoint.X)
      Else
         mdKx = (dtMaxPoint.X - dtMinPoint.X) / (dtMaxPoint.Y - dtMinPoint.Y)
      End If
   End Sub
End Class
