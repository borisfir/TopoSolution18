Option Explicit On
Option Strict On

Public Class UD_FLine
   Const msBlockName As String = "C1609"
   Const msLayerBaseName As String = "C1609_"
   Const msRadiusLabel As String = "R="
   Const iK As UInteger = 50000UI
   Private moPrevPoint As UD_Point
   Private moNextPoint As UD_Point
   Private mbIsArc As Boolean
   Private mdRadius As Double
   Private mtMiddlePoint As Autodesk.AutoCAD.Geometry.Point3d
   Private miStage As Integer = -1
   Private mtAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
   Private mtLineVector As Autodesk.AutoCAD.Geometry.Vector2d
   Private moLink As DMAcadExt.IUD_Link
	Private Shared moAcadBlock As DMAcadExt.AcadBlock
	Private Shared mhsPositions As HashSet(Of DMAcadExt.TplnPointKeyULong)
	Public Shared Sub Init()
		Dim tBlockRefData As DMAcadExt.BlockRefData
		Dim tPoint2d As Autodesk.AutoCAD.Geometry.Point2d

		moAcadBlock = New DMAcadExt.AcadBlock(msBlockName, UD_App.BlockPath13)
		moAcadBlock.Fields = New String() {"LINE_NAME", "RADIUS", "LEGAL_LENGTH", "CALC_LENGTH", "TOPO", "COMMENT"}
		moAcadBlock.Open(False)
		'moAcadBlock.OpenForRight()
		moAcadBlock.LoadAllReferences()
		zzInitPositionSet()
		For iIndex As Integer = 0 To moAcadBlock.ReferenceCount - 1
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Fline1", iIndex, moAcadBlock.ReferenceCount)
			tBlockRefData = moAcadBlock.GetBlockRefData(iIndex, False)
			tPoint2d = DMAcadExt.TPlnPoint.Point3dTo2d(tBlockRefData.Position)
			mhsPositions.Add(New DMAcadExt.TplnPointKeyULong(tPoint2d, True, True))
			mhsPositions.Add(New DMAcadExt.TplnPointKeyULong(tPoint2d, True, False))
			mhsPositions.Add(New DMAcadExt.TplnPointKeyULong(tPoint2d, False, True))
			mhsPositions.Add(New DMAcadExt.TplnPointKeyULong(tPoint2d, False, False))
		Next

	End Sub
	Private Shared Sub zzInitPositionSet()
		DMAcadExt.TplnPointKeyULong.SetOrigin(DMAcadExt.AcadDocument.GetExtMinPoint())
		DMAcadExt.TplnPointKeyULong.RoundScale = 1000.0
		mhsPositions = New HashSet(Of DMAcadExt.TplnPointKeyULong)()
	End Sub


	Public Sub New(oPrevPoint As UnidivNet.UD_Point, oNextPoint As UnidivNet.UD_Point, oLink As DMAcadExt.IUD_Link)
      moPrevPoint = oPrevPoint
      moNextPoint = oNextPoint
      moLink = oLink
      miStage = MaxPointStage()
      mbIsArc = moLink.IsArc

      '   DMAcadExt.AcadDocument.WriteDebugMessage("Var_First" & oLink.IsArc & "; " & oLink.Radius)
      'If moLink.IsArc OrElse moLink.Bulge <> 0.0 Then
      '   DMCommon.Debug.MsgBox("12_330", moLink.IsArc, moLink.Bulge)
      'End If
      If mbIsArc Then
         mtMiddlePoint = oLink.GetMidPoint3d()
         mdRadius = oLink.Radius
      End If

   End Sub
   Public Sub New(oPrevPoint As UnidivNet.UD_Point, oNextPoint As UnidivNet.UD_Point, tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
      moPrevPoint = oPrevPoint
      moNextPoint = oNextPoint
      mtAcObjID = tAcObjID
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

		Select Case oDBobject.GetRXClass().Name
			Case DMAcadExt.AcadConst.AcadLineName
			Case DMAcadExt.AcadConst.AcadPolylineName
				Dim oPolyline As Autodesk.AutoCAD.DatabaseServices.Polyline = DirectCast(oDBObject, Autodesk.AutoCAD.DatabaseServices.Polyline)
				If oPolyline.NumberOfVertices = 2 AndAlso oPolyline.GetSegmentType(0) = Autodesk.AutoCAD.DatabaseServices.SegmentType.Arc Then
					Dim oArc As DMAcadExt.TplnArc = New DMAcadExt.TplnArc(oPolyline)
					Dim dStartParam As Double = oPolyline.StartParam
					Dim dEndParam As Double = oPolyline.EndParam
					mbIsArc = True
					mdRadius = oArc.Radius
					mtMiddlePoint = oPolyline.GetPointAtParameter(0.5 * (dStartParam + dEndParam))
				End If
			Case DMAcadExt.AcadConst.AcadArcName
				mbIsArc = True
				Dim oArc As Autodesk.AutoCAD.DatabaseServices.Arc = DirectCast(oDBobject, Autodesk.AutoCAD.DatabaseServices.Arc)
				Dim dStartParam As Double = oArc.StartParam
				Dim dEndParam As Double = oArc.EndParam

				mdRadius = oArc.Radius
				mtMiddlePoint = oArc.GetPointAtParameter(0.5 * (dStartParam + dEndParam))
				'  DMAcadExt.AcadDocument.WriteDebugMessage("Var_II: " & mtAcObjID.ToString & ", " & mbIsArc & ", " & mtMiddlePoint.ToString & ", R=" & mdRadius.ToString)
			Case Else
		End Select

	End Sub

   Public Property Stage As Integer
      Get
         Return miStage
      End Get
      Set(iValue As Integer)
         miStage = iValue
      End Set
   End Property
   Public ReadOnly Property IDKey As UInteger
      Get
         If moPrevPoint.FragmentID < moNextPoint.FragmentID Then
            Return zzToKey(moPrevPoint.FragmentID, moNextPoint.FragmentID)
         Else
            Return zzToKey(moNextPoint.FragmentID, moPrevPoint.FragmentID)
         End If
      End Get
   End Property

   Public Function MaxPointStage() As Integer
		If zzGewtStage(moPrevPoint) = -1 OrElse zzGewtStage(moNextPoint) = -1 Then
			Return -1
		Else
			Return Math.Max(moPrevPoint.Stage, moNextPoint.Stage)
      End If
   End Function
	Public Function MinPointStage() As Integer
		If zzGewtStage(moPrevPoint) = -1 OrElse zzGewtStage(moNextPoint) = -1 Then
			Return -1
		Else
			Return Math.Min(moPrevPoint.Stage, moNextPoint.Stage)
		End If
	End Function
	Private Function zzGewtStage(oPoint As UD_Point) As Integer
		If oPoint Is Nothing Then
			Return -1
		Else
			Return oPoint.Stage
		End If
	End Function
	Public ReadOnly Property Name As String
      Get
         Return zzGetLineName()
      End Get
   End Property
   Public Sub InsertBlock()
      Dim oInsertPoint As DMAcadExt.TPlnPoint ' = New DMAcadExt.TPlnPoint(moPrevPoint.Point, moNextPoint.Point)
		If mbIsArc Then
			oInsertPoint = New DMAcadExt.TPlnPoint(mtMiddlePoint)
		Else
			oInsertPoint = New DMAcadExt.TPlnPoint(moPrevPoint.Point, moNextPoint.Point)
		End If
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "mbIsArc", mbIsArc, mtMiddlePoint, moPrevPoint.Point, moNextPoint.Point)
		mtLineVector = (New Autodesk.AutoCAD.Geometry.Vector2d(moNextPoint.Point.X, moNextPoint.Point.Y)).Subtract(New Autodesk.AutoCAD.Geometry.Vector2d(moPrevPoint.Point.X, moPrevPoint.Point.Y))
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		If Not mhsPositions.Contains(New DMAcadExt.TplnPointKeyULong(oInsertPoint.AcGePoint)) Then
			tBlockRefData.Position = oInsertPoint.AcGePoint3d
			tBlockRefData.Rotation = zzAngleToRotation(mtLineVector.Angle)

			tBlockRefData.AtribValuesDic = zzGetBlockData()
			tBlockRefData.Layer = GetStageFLineLayer()
			tBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()

			moAcadBlock.InsertRefNew(tBlockRefData)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!Fline+", "+++", oInsertPoint.AcGePoint)
		Else
			DMCommon.Debug.ExcelLog.SetNextValue(2, "!Fline-", "---", oInsertPoint.AcGePoint)
		End If
	End Sub
   Private Function zzAngleToRotation(dLineAngle As Double) As Double
      If dLineAngle > 0.5 * Math.PI AndAlso dLineAngle < 1.5 * Math.PI Then

         Return (dLineAngle + Math.PI) Mod (2 * Math.PI)
      Else
         Return dLineAngle
      End If
   End Function
	Private Function zzGetBlockData() As Dictionary(Of String, String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		dicAttribValues.Add("LINE_NAME", zzGetLineName())
		If mbIsArc Then
			dicAttribValues.Add("RADIUS", msRadiusLabel & FormatNumber(mdRadius, 2))
		Else
			dicAttribValues.Add("CALC_LENGTH", FormatNumber(mtLineVector.Length, 2))
		End If

		Return dicAttribValues
	End Function

	Private Function zzGetLineName() As String
      Try
         Return moPrevPoint.Name & "," & moNextPoint.Name
      Catch oEx As Exception
         Return String.Empty
      End Try
   End Function
	Private Function GetStageFLineLayer() As String
		Return GetStageFLineLayer(miStage)
	End Function
	Public Shared Function GetStageFLineLayer(iStage As Integer) As String
      If iStage <> -1 Then
         Return msLayerBaseName & iStage.ToString()
      Else
         Return "0"
      End If
   End Function
   Private Function zzToKey(iMin As Integer, iMax As Integer) As UInteger

      Return Convert.ToUInt32(iMin) + Convert.ToUInt32(iMax) * iK


   End Function
   Private Sub zzFromKey(iKey As UInteger, ByRef iMin As Integer, ByRef iMax As Integer)
      iMin = Convert.ToInt32(iKey \ iK)
      iMax = Convert.ToInt32(iKey - iMin * iK)
   End Sub
End Class
