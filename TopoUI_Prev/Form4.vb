Public Class Form4
	Private Shared mdNetScale As Double
	Private Shared mdPointTolerance As Double
	Private Shared mdOriginScale As Double

	Private mBaseNet As tmNet
	Private mPlusNet As tmNet
	Private mMinusNet As tmNet
	Private Structure Point3d
		Public X As Double
		Public Y As Double
		Public Z As Double

		Public Sub New(dX As Double, dY As Double)
			X = dX
			Y = dY
		End Sub
		Public Function Disp() As String
			Return CStr(X) & "," & CStr(Y)
		End Function
	End Structure
	Private Structure Extents3d
		Public MinPoint As Point3d
		Public MaxPoint As Point3d

		Public Sub New(tMinPoint As Point3d, tMaxPoint As Point3d)
			MinPoint = tMinPoint
			MaxPoint = tMaxPoint
		End Sub


	End Structure
	Public Shared Sub SetTolerance(dPointTolerance As Double)
		mdPointTolerance = dPointTolerance
		mdNetScale = 1.0 / (3.0 * mdPointTolerance)
		mdOriginScale = 1.0 / mdPointTolerance

	End Sub
	Private Sub NewI(tExtents As Extents3d)


		Dim dOriginX As Double
		Dim dOriginY As Double
		Dim dOriginDelta As Double = 0.001



	 
		Dim ulColumnCount As ULong



		dOriginX = Math.Floor(tExtents.MinPoint.X * mdOriginScale) * mdPointTolerance - mdPointTolerance
		dOriginY = Math.Floor(tExtents.MinPoint.Y * mdOriginScale) * mdPointTolerance - mdPointTolerance


		Dim s As String
		Dim dMaxX As Double = Math.Ceiling((tExtents.MaxPoint.X - dOriginX) * mdNetScale)
		ulColumnCount = Convert.ToUInt64(dMaxX) + 1UL

		mPlusNet = New tmNet(dOriginX, dOriginY, ulColumnCount)
		dOriginX -= mdPointTolerance
		dOriginY -= mdPointTolerance
		mBaseNet = New tmNet(dOriginX, dOriginY, ulColumnCount)
		dOriginX -= mdPointTolerance
		dOriginY -= mdPointTolerance
		mMinusNet = New tmNet(dOriginX, dOriginY, ulColumnCount)
		Dim tMaxPoint As Point3d = New Point3d(dOriginX + ulColumnCount * (3.0 * mdPointTolerance), tExtents.MaxPoint.Y)
		s = tMaxPoint.Disp()
		Dim p1 As Point3d = New Point3d(168346.174, 616066.072)

	End Sub
	Private Sub AddVertex(iRingID As Integer, iIndex As Integer, tPoint As Point3d, bIsLast As Boolean)
		'Dim oVertex As tmVertex = New tmVertex(iRingID, iIndex, tPoint)
		Dim bContains As Boolean
		'	moaVertices(iIndex) = oVertex
	 
			bContains = False
		mBaseNet.AddVertex(iIndex, tPoint)
		mPlusNet.AddVertex(iIndex, tPoint)
		mMinusNet.AddVertex(iIndex, tPoint)




	 
	End Sub
	Private Class tmNet
		Inherits Dictionary(Of ULong, Integer)
		'	Private Shared miRoundDecimal As Integer
		Private mdOriginX As Double
		Private mdOriginY As Double
		'	Private Shared mtPointTolerance As Autodesk.AutoCAD.Geometry.Tolerance
		'	Private Shared mdRoundScale As Double
		'	Private Shared mdXScale As Double
		'	Private Shared mdYScale As Double
		Private mulColumnCount As ULong
		'	Private miUB As Integer
		'	Private Shared iTestCounter As Integer
		Public Sub New(dOriginX As Double, dOriginY As Double, ulColumnCount As ULong)
			MyBase.New()
			mdOriginX = dOriginX
			mdOriginY = dOriginY
			mulColumnCount = ulColumnCount
		End Sub
		Public Function ContainsPoint(tPoint As Point3d) As Boolean
			Dim ulKey As ULong = zzGetPointKey(tPoint)
			Return MyBase.ContainsKey(ulKey)
		End Function
		Public Sub AddVertex(iVertexIndex As Integer, tPoint As Point3d)
			Dim ulKey As ULong = zzGetPointKey(tPoint)
			Try
				'	DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "All Count:" & CStr(Me.Count) & "," & "Key:" & CStr(lKey))
				If MyBase.ContainsKey(ulKey) Then

				Else

					MyBase.Add(ulKey, iVertexIndex)
				End If

				'DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "OK:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))




			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_623")
			End Try
		End Sub
		Public Function TryGetVertex(tPoint As Point3d, ByRef iVertexIndex As Integer, sHandle As String) As Boolean

			Dim ulKey As ULong = zzGetPointKey(tPoint)
			'		Dim oVertex As tmVertex = Nothing
			If MyBase.TryGetValue(ulKey, iVertexIndex) Then
				'	DMAcadExt.AcadDocument.WriteMessage("$$tmp " & CStr(oVertex.RingID) & "//" & CStr(oVertex.Index) & ", VN=" & sTmp & ", N=" & CStr(oNode.ID) & " P=" & DMAcadExt.TPlnPoint.DispPoint(oNode.Point2d))
				Return True
			Else
				'DMAcadExt.AcadDocument.WriteMessage("Node was not found:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))
				Return False

			End If


		End Function
		Private Function zzGetPointKey(tPoint As Point3d) As ULong
			Dim dX, dY As Double
			Dim ulX, ulY As ULong
			Try
				dX = Math.Floor((tPoint.X - mdOriginX) * mdNetScale)
				dY = Math.Round(tPoint.Y - mdOriginY) * mdNetScale
				ulX = Convert.ToUInt64(dX)
				ulY = Convert.ToUInt64(dY)
				Return mulColumnCount * ulX + ulY
			Catch oEx As Exception


				Return 0UL
			End Try

		End Function
	End Class

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Dim tMinPoint As Point3d = New Point3d(168322.961, 616065.657)
		Dim tMaxPoint As Point3d = New Point3d(168357.665, 616100.607)
		Dim tExt As Extents3d = New Extents3d(tMinPoint, tMaxPoint)
		NewI(tExt)
	End Sub

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
      SetTolerance(0.001)

      Me.ctxAction.DividerWidth = 2
      Me.ctxToGush.DividerWidth = 2
      Me.ctxFromParcelTemp.DividerWidth = 1


	End Sub

   Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
      TopoManager.NumerationPair.SetGroupDelim(TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)

      Dim oNumerationA As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.Entire, TopoManager.NumerationPair.enTextDirection.LeftToRight, "[", "]")
      Dim oNumerationB As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.Entire, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)

      oNumerationA.Add("2")
      oNumerationA.Add("4")

      oNumerationA.Add("5")
      oNumerationA.Add("6")
      oNumerationA.Add("8")
      oNumerationA.Add("9")
      oNumerationA.Add("10")

      oNumerationB.Add("1")
      oNumerationB.Add("3")
      oNumerationB.Add("4")
      oNumerationB.Add("5")
      oNumerationB.Add("6")
      oNumerationB.Add("7")
      oNumerationB.Add("9")
      Dim s As String = oNumerationB.GetPresentation() & "," & oNumerationA.GetPresentation()
      Me.txtLtoR.Text = s
      Me.txtRtoL.Text = s

   End Sub

   Private Sub Button3_Click(oSender As System.Object, e As EventArgs) Handles Button3.Click
      Me.txtMline.Clear()
      For i As Integer = 0 To Me.txtRtoL.Text.Length - 1
         If i > 0 Then
            Me.txtMline.AppendText(vbCrLf)
         End If
         Me.txtMline.AppendText(txtRtoL.Text.Substring(i, 1))
      Next
   End Sub

   Private Sub Button4_Click(oSender As System.Object, e As EventArgs) Handles Button4.Click
      Dim sDefaultFolder As String = "T:\Boris\UD30322"
      Dim sDefaultTxtFileName As String = "Tr_bookH.txt"
      Dim sSourceFullName As String = ""
      Dim sOutputFullName As String = "T:\Boris\UD30322\aa.pdf"

      Try
         MSWordApp.Util.CreatePDF(sDefaultFolder, sDefaultTxtFileName, sOutputFullName)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_179")
      End Try
   End Sub
   Private Sub zz()
      Dim i As ULong = 1234567890123456789
      Dim hh As ULong = i \ (1000000000UL * 1000000000UL)



   End Sub
   Private Sub Button5_Click(oSender As System.Object, e As EventArgs) Handles Button5.Click


      TplnPointKey.SetCenter(Math.Round(400.0), Math.Round(500))
      Dim dc1 As Decimal = TplnPointKey.CoordToKey(283.123466, 178.542823)
      Dim oPoint As DoublePoint = TplnPointKey.PointFromKey(dc1)
   End Sub

   Private Sub Button6_Click(oSender As System.Object, e As EventArgs) Handles Button6.Click
      TplnPointKeyLong.SetCenter(Math.Round(4000.0), Math.Round(5000))

      Dim ulKey As ULong = TplnPointKeyLong.CoordToKey(283.123466, 178.542823)
      Dim oPoint As DoublePoint = TplnPointKeyLong.PointFromKey(ulKey)
   End Sub

   Private Sub DataGridView1_CellContentClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellContentClick

   End Sub
   Private Sub zzFill_UD()
      Dim oGridRow As DataGridViewRow = Nothing
      Me.dgvMain.Rows.Add(12)
      oGridRow = Me.dgvMain.Rows.Item(0)
      oGridRow.Cells.Item("ctxStage").Value = 0
      oGridRow.Cells.Item("ctxAction").Value = "מ' רשום"
      oGridRow.Cells.Item("ctxFromParcel").Value = 6
      oGridRow.Cells.Item("ctxLegalArea").Value = 18.774
      oGridRow.Cells.Item("ctxArea").Value = 18.788
      oGridRow.Cells.Item("ctxTolerance").Value = 0.135




      oGridRow = Me.dgvMain.Rows.Item(1)
      oGridRow.Cells.Item("ctxFromParcel").Value = 9


      oGridRow.Cells.Item("ctxLegalArea").Value = 227.601
      oGridRow.Cells.Item("ctxArea").Value = 228.339
      oGridRow.Cells.Item("ctxTolerance").Value = 0.837

      oGridRow = Me.dgvMain.Rows.Item(2)

      oGridRow.Cells.Item("ctxFromParcel").Value = 10
      oGridRow.DividerHeight = 4

      oGridRow.Cells.Item("ctxLegalArea").Value = 149.101
      oGridRow.Cells.Item("ctxArea").Value = 149.628
      oGridRow.Cells.Item("ctxTolerance").Value = 0.607



      oGridRow = Me.dgvMain.Rows.Item(3)
      oGridRow.Cells.Item("ctxStage").Value = 1

      oGridRow.Cells.Item("ctxAction").Value = "חלוקה"
      oGridRow.Cells.Item("ctxFromParcel").Value = 6
      oGridRow.Cells.Item("ctxToParcel").Value = 12

      oGridRow = Me.dgvMain.Rows.Item(4)
      oGridRow.Cells.Item("ctxToParcel").Value = 13

      oGridRow.DividerHeight = 2


      oGridRow = Me.dgvMain.Rows.Item(5)
      oGridRow.Cells.Item("ctxFromParcel").Value = 9

      oGridRow.Cells.Item("ctxToParcel").Value = 14
      oGridRow = Me.dgvMain.Rows.Item(6)
      oGridRow.Cells.Item("ctxToParcel").Value = 15
      oGridRow.DividerHeight = 2


      oGridRow = Me.dgvMain.Rows.Item(7)
      oGridRow.Cells.Item("ctxFromParcel").Value = 10

      oGridRow.Cells.Item("ctxToParcel").Value = 16
      oGridRow = Me.dgvMain.Rows.Item(8)
      oGridRow.Cells.Item("ctxToParcel").Value = 17
      oGridRow.DividerHeight = 4

      oGridRow = Me.dgvMain.Rows.Item(9)
      oGridRow.Cells.Item("ctxStage").Value = 2

      oGridRow.Cells.Item("ctxAction").Value = "איחוד"


      oGridRow.Cells.Item("ctxFromParcelTemp").Value = 13
      oGridRow.Cells.Item("ctxToParcel").Value = 18

      oGridRow = Me.dgvMain.Rows.Item(10)
     


      oGridRow.Cells.Item("ctxFromParcelTemp").Value = 15
      oGridRow = Me.dgvMain.Rows.Item(11)



      oGridRow.Cells.Item("ctxFromParcelTemp").Value = 17

   End Sub

   Private Sub Form4_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
      zzFill_UD()
   End Sub
End Class

Public Structure TplnPointKey
   Public Const ShiftX As Decimal = 1000000000000
   Public Const RoundDigit As Integer = 3
   Private Shared mdcCenterX As Decimal
   Private Shared mdcCenterY As Decimal
   Public Shared Sub SetCenter(dX As Double, dY As Double)
      Dim dValue As Double = Math.Round(dX)
      mdcCenterX = Convert.ToDecimal(dValue)

      dValue = Math.Round(dY)
      mdcCenterY = Convert.ToDecimal(dValue)
   End Sub
   Public Shared Function PointFromKey(tPointKey As Decimal) As DoublePoint
      Dim decY As Decimal = Math.Floor(tPointKey / ShiftX)
      Dim decX As Decimal = tPointKey - ShiftX * decY

      Return New DoublePoint(Convert.ToDouble(decX + mdcCenterX), Convert.ToDouble(decY + mdcCenterY))
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
Public Structure TplnPointKeyLong
   Private Const ShiftX As ULong = 1000000000UL
   Private Shared ShiftSign As ULong = ShiftX * ShiftX

   Public Const RoundDigit As Integer = 3
   Public Const RoundShift As Double = 1000.0
   ' Private Shared mlCenterX As ULong
   ' Private Shared mlCenterY As ULong
   Private Shared mdCenterX As Double
   Private Shared mdCenterY As Double
   Public Shared Sub SetCenter(dX As Double, dY As Double)
      mdCenterX = Math.Round(dX)
      mdCenterY = Math.Round(dY)
   End Sub
   Public Shared Function PointFromKey(tPointKey As ULong) As DoublePoint
      '   Dim decY As Decimal = Math.Floor(tPointKey / ShiftX)
      '   Dim decX As Decimal = tPointKey - ShiftX * decY

      '   Return New TPlnPoint(Convert.ToDouble(decX + mdcCenterX), Convert.ToDouble(decY + mdcCenterY))
      Dim lSign As ULong = tPointKey \ ShiftSign
      tPointKey -= lSign * ShiftSign
      Dim lX As ULong = tPointKey \ ShiftX
      Dim lY As ULong = tPointKey - lX * ShiftX
      Dim dX As Double = Convert.ToDouble(lX) / RoundShift
      Dim dY As Double = Convert.ToDouble(lY) / RoundShift

      If lSign Mod (2UL) = 1UL Then
         dX = -dX
      End If
      If lSign >= 2UL Then
         dY = -dY
      End If
      Return New DoublePoint(mdCenterX + dX, mdCenterY + dY)
   End Function

   Public Shared Function CoordToKey(dX As Double, dY As Double) As ULong
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

      Return (lSignX + lSignY) * ShiftSign + ShiftX * Convert.ToUInt64(Math.Abs(dX)) + Convert.ToUInt64(Math.Abs(dY))

   End Function
End Structure

Public Structure DoublePoint
   Public mdX As Double
   Public mdY As Double
   Public Sub New(dX As Double, dY As Double)
      mdX = dX
      mdY = dY

   End Sub

End Structure


Public Class Geom

End Class