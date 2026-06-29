Option Explicit On
Option Strict On
Public Structure TplnPointKey
   Private Const ShiftX As Decimal = 1000000000000
   Public Const RoundDigit As Integer = 3
	Private Shared mdcCenterX As Decimal
	Private Shared mdcCenterY As Decimal
   Public Shared Sub SetCenter(dX As Double, dY As Double)
      Dim dValue As Double = Math.Round(dX)
      mdcCenterX = Convert.ToDecimal(dValue)

      dValue = Math.Round(dY)
      mdcCenterY = Convert.ToDecimal(dValue)
   End Sub
   Public Shared Function PointFromKey(tPointKey As Decimal) As TPlnPoint
      Dim decY As Decimal = Math.Floor(tPointKey / ShiftX)
      Dim decX As Decimal = tPointKey - ShiftX * decY

      Return New TPlnPoint(Convert.ToDouble(decX + mdcCenterX), Convert.ToDouble(decY + mdcCenterY))
   End Function
   Public Shared Function CoordToKey(dcX As Decimal, dcY As Decimal) As Decimal
      Return (dcX - mdcCenterX) * ShiftX + (dcY - mdcCenterY)
   End Function
   Public Shared Function CoordToKey(dX As Double, dY As Double) As Decimal
      dX = Math.Round(dX, RoundDigit, MidpointRounding.AwayFromZero)
      dY = Math.Round(dY, RoundDigit, MidpointRounding.AwayFromZero)
      Return CoordToKey(Convert.ToDecimal(dX), Convert.ToDecimal(dY))
   End Function
End Structure

