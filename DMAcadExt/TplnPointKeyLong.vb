Option Explicit On
Option Strict On
Public Structure TplnPointKeyLong
	Private Const ShiftX As ULong = 1000000000UL

	Private Shared ShiftSign As ULong = ShiftX * ShiftX

   Public Const RoundDigit As Integer = 4
   Public Const RoundShift As Double = 10000.0
   ' Private Shared mlCenterX As ULong
   ' Private Shared mlCenterY As ULong
   Private Shared mdCenterX As Double
   Private Shared mdCenterY As Double

   Private Shared mdMinX As Double
   Private Shared mdMinY As Double

   Private Shared mdMaxX As Double
   Private Shared mdMaxY As Double


   Public Shared Sub SetCenter(dX As Double, dY As Double)
      mdCenterX = Math.Round(dX)
      mdCenterY = Math.Round(dY)
      DMAcadExt.AcadDocument.WriteMessage("Center: " & CStr(mdCenterX) & "," & CStr(mdCenterY))

   End Sub
	Public Shared Sub SetExtension(tMinPoint As Autodesk.AutoCAD.Geometry.Point2d, tMaxPoint As Autodesk.AutoCAD.Geometry.Point2d)
		mdMinX = Math.Floor(tMinPoint.X) - 1.0
		mdMinY = Math.Floor(tMinPoint.Y) - 1.0

		mdMaxX = Math.Floor(tMaxPoint.X) + 1.0
		mdMaxY = Math.Floor(tMaxPoint.Y) + 1.0

		DMAcadExt.AcadDocument.WriteMessage("Extension: " & tMinPoint.ToString() & " ==> " & tMaxPoint.ToString())
	End Sub


	Public Shared Function PointFromKey(tPointKey As ULong) As TPlnPoint
		Dim lSign As ULong
		Try
         lSign = tPointKey \ ShiftSign
      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage("Try " & CStr(tPointKey) & "; ")
         Return New TPlnPoint()
      End Try

      tPointKey -= lSign * ShiftSign
      Dim lX As ULong = tPointKey \ ShiftX
      Dim lY As ULong = tPointKey - (lX * ShiftSign)
      Dim dX As Double = Convert.ToDouble(lX) / RoundShift
      Dim dY As Double = Convert.ToDouble(lY) / RoundShift

      If lSign Mod (2UL) = 1UL Then
         dX = -dX
      End If
      If lSign >= 2UL Then
         dY = -dY
      End If

		Return New TPlnPoint(mdCenterX + dX, mdCenterY + dY)
   End Function
   
   Public Shared Function CoordToKey(dX As Double, dY As Double) As ULong
      Dim bErrExt As Boolean = False
      If dX >= mdMinX AndAlso dX <= mdMaxX Then
         dX = Math.Round(RoundShift * (dX - mdCenterX), 0, MidpointRounding.AwayFromZero)
      Else
         bErrExt = True
      End If
      If dY >= mdMinY AndAlso dY <= mdMaxY Then
         dY = Math.Round(RoundShift * (dY - mdCenterY), 0, MidpointRounding.AwayFromZero)
      Else
         bErrExt = True
      End If


      If bErrExt Then
         Dim sMsg As String = "Point is out of extension " & dX.ToString() & "," & dY.ToString() & vbCrLf & mdMinX.ToString() & "," & mdMinY.ToString()
         DMAcadExt.AppMessages.AddMessage(True, dX, dY, "", sMsg, False)
         Return 0UL
      Else
         Dim lSignX, lSignY As ULong
         If dX >= 0.0 Then
            lSignX = 0L
         Else
            lSignX = 1L
         End If
         If dX >= 0.0 Then
            lSignY = 0L
         Else
            lSignY = 2L
         End If

         Return (lSignX + lSignY) * ShiftSign + ShiftX * Convert.ToUInt64(Math.Abs(dX)) + Convert.ToUInt64(Math.Abs(dY))
      End If
     

   End Function
   Public Shared Function GetRoundedPoint(dX As Double, dY As Double) As TPlnPoint
      dX = Math.Round(RoundShift * (dX - mdCenterX), 0, MidpointRounding.AwayFromZero)
      dY = Math.Round(RoundShift * (dY - mdCenterY), 0, MidpointRounding.AwayFromZero)
      Dim lSignX, lSignY As ULong
      If dX >= 0.0 Then
         lSignX = 0&
      Else
         lSignX = 1&
      End If
      If dX >= 0.0 Then
         lSignY = 0&
      Else
         lSignY = 2&
      End If

      Return New TPlnPoint(dX, dY)

   End Function
End Structure
