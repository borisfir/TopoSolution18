Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Class SegmentDic
	Inherits Dictionary(Of TplnCurveSegment, Curve)
	Private Shared moTolerance As Tolerance
	Private Shared mdBulgeTolerance As Double
	Public Sub New(ByVal dTolerance As Double)
		MyBase.New(New CurveSegmentComparer())
		moTolerance = New Tolerance(dTolerance, dTolerance)
	End Sub

    Public Function AddLine(ByVal oLine As Line) As Boolean
        Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.StartPoint)
        Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oLine.EndPoint)
        Dim oLineSegment As TplnCurveSegment
        Dim oCurveE As Curve = Nothing
        Dim bLineExists As Boolean
        oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)

        bLineExists = MyBase.TryGetValue(oLineSegment, oCurveE)

        If bLineExists Then
            oCurveE.XData = oLine.XData  'Pribavit
            Return False
        Else
            MyBase.Add(oLineSegment, oLine)
            Return True
        End If
    End Function
    Public Function AddArc(ByVal oArc As Arc) As Boolean
        Dim tStartPoint As Point2d = TPlnPoint.Point3dTo2d(oArc.StartPoint)
        Dim tEndPoint As Point2d = TPlnPoint.Point3dTo2d(oArc.EndPoint)
        Dim oLineSegment As TplnCurveSegment
        Dim oCurveE As Curve = Nothing
        Dim bLineExists As Boolean
        oLineSegment = New TplnCurveSegment(tStartPoint, tEndPoint)
        bLineExists = MyBase.TryGetValue(oLineSegment, oCurveE)
        If bLineExists Then
            oCurveE.XData = oArc.XData   'Pribavit
            Return False
        Else
            MyBase.Add(oLineSegment, oArc)
            Return True
        End If
    End Function

	Private Class CurveSegmentComparer
		Implements IEqualityComparer(Of TplnCurveSegment)
		Public Sub New()
			'	AcadDocument.WriteMessage("-- -- New Line2dComparer")
		End Sub

		Public Function EqualsLines(ByVal oLineA As TplnCurveSegment, ByVal oLineB As TplnCurveSegment) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of TplnCurveSegment).Equals

			Dim bResp As Boolean = False
			Try
				'		AcadDocument.WriteMessage(TPlnPoint.DispPoint(oLineA.StartPoint) & ":" & TPlnPoint.DispPoint(oLineA.EndPoint) & ":" & TPlnPoint.DispPoint(oLineB.StartPoint))
				'		bResp = (oLineA.StartPoint = oLineB.StartPoint) AndAlso (oLineA.EndPoint = oLineB.EndPoint) OrElse (oLineA.StartPoint = oLineB.EndPoint) AndAlso (oLineA.EndPoint = oLineB.StartPoint)

				bResp = ((oLineA.StartPoint.IsEqualTo(oLineB.StartPoint, moTolerance)) AndAlso (oLineA.EndPoint.IsEqualTo(oLineB.EndPoint, moTolerance))) OrElse ((oLineA.StartPoint.IsEqualTo(oLineB.EndPoint, moTolerance)) AndAlso (oLineA.EndPoint.IsEqualTo(oLineB.StartPoint, moTolerance)))
				If bResp Then
					If oLineA.HasBulge AndAlso oLineB.HasBulge Then
						Return Math.Abs(oLineA.GetBulge() - oLineB.GetBulge()) <= moTolerance.EqualPoint
					ElseIf oLineA.HasBulge OrElse oLineB.HasBulge Then
						Return False
					End If
				End If
			Catch oEx As Exception
				AcadDocument.WriteMessage(oEx.Message & vbCrLf & oEx.StackTrace)
			End Try
			Return bResp
		End Function

		Public Function GetHashCode1(ByVal obj As TplnCurveSegment) As Integer Implements System.Collections.Generic.IEqualityComparer(Of TplnCurveSegment).GetHashCode
			'	AcadDocument.WriteMessage("??????")
			Return 0
		End Function

	End Class
	Private Class Line2dComparer
		Implements IEqualityComparer(Of LineSegment)
		Public Sub New()
			'	AcadDocument.WriteMessage("-- -- New Line2dComparer")
		End Sub

		Public Function EqualsLines(ByVal oLineA As LineSegment, ByVal oLineB As LineSegment) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of LineSegment).Equals

			Dim bResp As Boolean
			Try
				'		AcadDocument.WriteMessage(TPlnPoint.DispPoint(oLineA.StartPoint) & ":" & TPlnPoint.DispPoint(oLineA.EndPoint) & ":" & TPlnPoint.DispPoint(oLineB.StartPoint))
				'		bResp = (oLineA.StartPoint = oLineB.StartPoint) AndAlso (oLineA.EndPoint = oLineB.EndPoint) OrElse (oLineA.StartPoint = oLineB.EndPoint) AndAlso (oLineA.EndPoint = oLineB.StartPoint)
				bResp = ((oLineA.StartPoint.IsEqualTo(oLineB.StartPoint, moTolerance)) AndAlso (oLineA.EndPoint.IsEqualTo(oLineB.EndPoint, moTolerance))) OrElse ((oLineA.StartPoint.IsEqualTo(oLineB.EndPoint, moTolerance)) AndAlso (oLineA.EndPoint.IsEqualTo(oLineB.StartPoint, moTolerance)))

			Catch oEx As Exception
				AcadDocument.WriteMessage(oEx.Message & vbCrLf & oEx.StackTrace)
			End Try
			Return bResp
		End Function

		Public Function GetHashCode1(ByVal obj As LineSegment) As Integer Implements System.Collections.Generic.IEqualityComparer(Of LineSegment).GetHashCode
			'	AcadDocument.WriteMessage("??????")
			Return 0
		End Function

	End Class
End Class
