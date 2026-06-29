Option Explicit On
Option Strict On
Public Class DrawVectorSet
	Const mdArcTolerance As Double = 0.001
	Private moComponents As System.ComponentModel.IContainer
   Protected WithEvents tmrDelay As System.Windows.Forms.Timer
   Private mbViewChanged As Boolean
   Private mbActive As Boolean
   Private moEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
   'Private moaVectorSetArray As IEnumerable(Of VectorSet)
   Private moaVectorSetArray() As VectorSet
   Private moaPoints As TplnPointArray
   ' Dim oaVectorSet(0) As VectorSet
   Public Structure VectorSet
      '  Dim Points As IEnumerable(Of TPlnPoint)
      Dim Links As IEnumerable(Of IUD_Link)

      Dim Color As Integer
      Dim HighLighted As Boolean
      Dim LinkCount As Integer

      Public Sub New(colLinks As IEnumerable(Of IUD_Link), iLinkCount As Integer, iColor As Integer, bHighLighted As Boolean)
         Links = colLinks
         Color = iColor
         HighLighted = bHighLighted
         LinkCount = iLinkCount
      End Sub

   End Structure





   Public Sub New(oVectorSet As VectorSet)
      ReDim moaVectorSetArray(0)
      moaVectorSetArray(0) = oVectorSet

      mbActive = True
      zzMyInitialize()


   End Sub
   Public Sub New(oPoint As TPlnPoint)
      moaPoints = New TplnPointArray(New TPlnPoint() {oPoint})

      mbActive = True
      zzMyInitialize()


   End Sub
   Public Sub Add(oVectorSet As VectorSet)
      If moaVectorSetArray IsNot Nothing Then
         Dim iUB As Integer = moaVectorSetArray.GetUpperBound(0)
         iUB += 1
         ReDim Preserve moaVectorSetArray(iUB)
         moaVectorSetArray(iUB) = oVectorSet
      End If
   End Sub
   Public Sub Replace(oVectorSet As VectorSet, iIndex As Integer)
      If moaVectorSetArray IsNot Nothing Then
         Dim iUB As Integer = moaVectorSetArray.GetUpperBound(0)
         If iIndex >= 0 AndAlso iIndex <= iUB Then
            moaVectorSetArray(iIndex) = oVectorSet
            mbViewChanged = False
         Else
            Add(oVectorSet)
         End If

      End If
   End Sub
   Public Sub InfoMsg()
      If moaVectorSetArray Is Nothing Then
         DMCommon.Debug.MsgBox("12_205a", "moaVectorSetArray Is Nothing ")
      Else
         DMCommon.Debug.MsgBox("12_206", mbActive, moComponents, moaVectorSetArray.GetUpperBound(0))
      End If

   End Sub
   Public Sub Clear()
      mbActive = False
      If moComponents IsNot Nothing Then
         '   moComponents.Dispose()
         '  moComponents = Nothing
      End If
      If moaVectorSetArray IsNot Nothing Then
         Erase moaVectorSetArray
         moaVectorSetArray = Nothing
      End If

   End Sub

   Public Property ViewChanged As Boolean
      Get
         Return mbViewChanged
      End Get
      Set(bValue As Boolean)
         mbViewChanged = bValue
      End Set
   End Property
   Public Property Active As Boolean
      Get
         Return mbActive
      End Get
      Set(bValue As Boolean)
         mbActive = bValue
      End Set
   End Property


   Private Sub zzMyInitialize()
      moComponents = New System.ComponentModel.Container
      Me.tmrDelay = New System.Windows.Forms.Timer(Me.moComponents)
      tmrDelay.Interval = 200
      tmrDelay.Start()
   End Sub
   Public ReadOnly Property VectorSetArray() As VectorSet()
      Get
         Return moaVectorSetArray
      End Get
   End Property
   Public ReadOnly Property VectorSetUB() As Integer
      Get
         If moaVectorSetArray IsNot Nothing Then
            Return moaVectorSetArray.GetUpperBound(0)
         Else
            Return -1
         End If

      End Get
   End Property
   Public Sub Draw(Optional sLabel As String = "")
      ' AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      '   Dim sTest As String = ""
      If mbActive Then
         moEditor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
         If moaVectorSetArray IsNot Nothing Then
            For Each tVectorSet As VectorSet In moaVectorSetArray
               '   zzDrawVectorSet(tVectorSet)
               '    sTest &= CStr(tVectorSet.LinkCount) & ","
               zzDrawLinks(tVectorSet)
            Next
         End If

			'   AcadDocument.Unlock()

			If moaPoints IsNot Nothing Then
            For Each oPoint As TPlnPoint In moaPoints.Array
					AcadDocument.DrawShape(MarkBlock.enMarkBlockType.Square, DMAcadExt.AcadDocument.GetDWGScaleFactor * 10, oPoint, 1, True)
				Next
         End If

         moEditor.UpdateScreen()
      End If

   End Sub
   Public Sub AddPoint(oPoint As TPlnPoint)
      If moaPoints Is Nothing Then
         moaPoints = New TplnPointArray(New TPlnPoint() {oPoint})
      Else
         moaPoints.Add(oPoint)
      End If
   End Sub
   Private Sub zzDrawLink(oLink As IUD_Link, iColor As Integer, bHighLighted As Boolean)
		If oLink.IsArc Then
			zzDrawArc(DirectCast(oLink, TplnArc), iColor, bHighLighted)
		Else
			zzDrawLine(DirectCast(oLink, TplnLine), iColor, bHighLighted)
      End If
   End Sub
	Private Sub zzDrawArc(oArc As TplnArc, iColor As Integer, bHighLighted As Boolean)

		Dim tStartSegment As Autodesk.AutoCAD.Geometry.Point3d = DMAcadExt.TPlnPoint.Point2dTo3d(oArc.StartPoint)
		Dim tEndSegment As Autodesk.AutoCAD.Geometry.Point3d

		Dim oaInnerPoints() As Autodesk.AutoCAD.Geometry.Point2d = oArc.GetInnerPointsNew(mdArcTolerance)

		If oaInnerPoints Is Nothing Then
			tEndSegment = DMAcadExt.TPlnPoint.Point2dTo3d(oArc.EndPoint)
			moEditor.DrawVector(tStartSegment, tEndSegment, iColor, bHighLighted)

		Else

			For iIndex As Integer = 0 To oaInnerPoints.GetUpperBound(0)
				tEndSegment = DMAcadExt.TPlnPoint.Point2dTo3d(oaInnerPoints(iIndex))
				moEditor.DrawVector(tStartSegment, tEndSegment, iColor, bHighLighted)
				tStartSegment = tEndSegment
			Next
			tEndSegment = DMAcadExt.TPlnPoint.Point2dTo3d(oArc.EndPoint)
			moEditor.DrawVector(tStartSegment, tEndSegment, iColor, bHighLighted)
			' AcadDocument.WriteDebugMessageN("?Kama", oaInnerPoints.GetUpperBound(0), oaInnerPoints(0), oaInnerPoints(oaInnerPoints.GetUpperBound(0)))
		End If

	End Sub

	Private Sub zzDrawLine(oLine As TplnLine, iColor As Integer, bHighLighted As Boolean)
      moEditor.DrawVector(oLine.StartPoint3d, oLine.EndPoint3d, iColor, bHighLighted)
   End Sub
   Private Sub zzDrawLinks(tVectorSet As VectorSet)
      If tVectorSet.Links IsNot Nothing Then
         For Each oLink As IUD_Link In tVectorSet.Links
            If oLink IsNot Nothing Then
               zzDrawLink(oLink, tVectorSet.Color, tVectorSet.HighLighted)
            End If

         Next
      End If

   End Sub


   Private Sub tmrDelay_Tick(oSender As System.Object, e As EventArgs) Handles tmrDelay.Tick
      '   AcadDocument.WriteMessage("mbViewChanged=" & mbViewChanged.ToString())
      If mbActive AndAlso mbViewChanged Then
         Draw("Tick")
         mbViewChanged = False
      End If
   End Sub
End Class
