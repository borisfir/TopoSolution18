Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Public Class UD_Points
   Inherits Dictionary(Of ULong, UD_Point)
   Private mdicNames As SortedDictionary(Of String, UD_Point)
	Private mdicObjectIDs As Dictionary(Of ObjectId, UD_Point) = New Dictionary(Of ObjectId, UD_Point)()
	'Private miPointExistsMaxNum As Integer
	'Private miMinDBNum As Integer = 0
	Private miStartNewNum As Integer
	Private moTriangleMarkBlock As DMAcadExt.MarkBlock '= New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
	'Private mbNeedRecalc As Boolean
	Public Sub AddPoint(oPoint As UD_Point)
		If String.IsNullOrEmpty(oPoint.Name) Then
			System.Windows.Forms.MessageBox.Show("Point Name is Empty", "09_583")
		Else
			'If oPoint.Name = "658" Then
			'   Dim tKey As ULong = oPoint.PointKey
			'   '   Dim oTestPoint As DMAcadExt.TPlnPoint = DMAcadExt.TplnPointKeyLong.PointFromKey(tKey)
			'   Dim oRndPoint As DMAcadExt.TPlnPoint = DMAcadExt.TplnPointKeyLong.GetRoundedPoint(oPoint.Point.X, oPoint.Point.Y)
			'   DMAcadExt.AcadDocument.WriteMessage("KeyIn " & CStr(tKey) & "; " & CStr(oPoint.Coordinates) & ";Rnd=" & CStr(oRndPoint.Coordinates))
			'End If
			'	
			MyBase.Add(oPoint.PointKey, oPoint)
			If mdicNames.ContainsKey(oPoint.Name) Then
				DMAcadExt.AcadDocument.WriteMessage("Double Point Name " & CStr(oPoint.Name) & "; " & CStr(oPoint.Coordinates))
			ElseIf Not String.IsNullOrEmpty(oPoint.Name) Then
				mdicNames.Add(oPoint.Name, oPoint)
			End If
		End If
	End Sub
	Public Sub AddForeignName(sPointName As String)
		mdicNames.Add(sPointName, Nothing)

	End Sub
	Public Sub OpenMarkBlocks()
		moTriangleMarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Triangle)
		'	DMCommon.Debug.MsgBox("09_266K", moTriangleMarkBlock)
	End Sub
	Public Sub AddNode(oPoint As UD_Point)
      Dim oPrevPoint As UD_Point = Nothing
      If Not oPoint.AcObjID.IsNull Then
         If MyBase.ContainsKey(oPoint.PointKey) Then
				'  DMCommon.Debug.MsgBox("09_266f", oPoint.PointKey, oPoint.Coordinates)
				moTriangleMarkBlock.MarkPoint(oPoint.Point.AcGePoint3d, 2S)

         Else
            MyBase.Add(oPoint.PointKey, oPoint)
         End If

         mdicObjectIDs.Add(oPoint.AcObjID, oPoint)
         If Not String.IsNullOrEmpty(oPoint.Name) Then
				If mdicNames.TryGetValue(oPoint.Name, oPrevPoint) Then
					If oPoint.IsOld Then
						oPoint.IsRepeating = True
					Else
						''''''''''''''''''''''DMCommon.Debug.UserMsg("09_266", "Point '" & oPoint.Name & "' already exists")
						'	DMCommon.Debug.MsgBox("09_266h", moTriangleMarkBlock, oPoint, oPrevPoint)
						'	DMCommon.Debug.MsgBox("09_266j", moTriangleMarkBlock, oPoint.Point)

						moTriangleMarkBlock.MarkPoint(oPoint.Point.AcGePoint3d, 4S)
						If oPrevPoint IsNot Nothing Then
							moTriangleMarkBlock.MarkPoint(oPrevPoint.Point.AcGePoint3d, 4S)
						End If


					End If
					'	DMCommon.Debug.MsgBox("09_266P")

				Else
					mdicNames.Add(oPoint.Name, oPoint)
            End If
         End If

      End If

   End Sub
	Public Sub SetNewPointNumber(bMaxNumber As Boolean, ByRef oPoint As UD_Point)
		Dim iNumStart As Integer
		Dim sNumber As String

		iNumStart = miStartNewNum

		Do

			sNumber = Convert.ToString(iNumStart)
			iNumStart += 1

		Loop While mdicNames.ContainsKey(sNumber)
		miStartNewNum = iNumStart
		oPoint.Name = sNumber
		mdicNames.Add(sNumber, oPoint)
		'	DMCommon.ExcelLogAW.SetValue(8, bMaxNumber, sNumber, iNumStart, miPointExistsMaxNum)
	End Sub
	Public Sub ResetPoint(oPoint As UD_Point)
		If Not oPoint.IsOriginalName Then
			mdicNames.Remove(oPoint.Name)
			oPoint.Name = String.Empty

			oPoint.UpdateBlock(UnidivNet.UD_App.FragmentNewNodesLayer)
		End If
		oPoint.Stage = -1


	End Sub
	Public Sub RemoveName(sPointName As String)
		Dim iPointNum As Integer
		If Integer.TryParse(sPointName, iPointNum) AndAlso mdicNames.ContainsKey(sPointName) Then
			mdicNames.Remove(sPointName)

		End If
	End Sub

	Public Function GetPoint(tAcObjID As ObjectId) As UD_Point
      Dim oPoint As UD_Point = Nothing
      If mdicObjectIDs.TryGetValue(tAcObjID, oPoint) Then
         Return oPoint
      Else
         Return Nothing
      End If
   End Function
   Public Sub UpdatePointName(tBlockRefData As DMAcadExt.BlockRefData)
      Dim saAttribValues() As String = tBlockRefData.AttribValues
      Dim sAcadName As String = saAttribValues(0)
      '    Dim oPoint As UD_Point = Nothing

      Dim tPointKey As ULong = DMAcadExt.TplnPointKeyLong.CoordToKey(tBlockRefData.Position.X, tBlockRefData.Position.Y)

      '	DMAcadExt.AcadDocument.WriteMessage("KeyIn " & CStr(oPoint.PointKey) & "; " & CStr(oPoint.Coordinates))
      Dim oPointExisting As UD_Point = Nothing

      If MyBase.TryGetValue(tPointKey, oPointExisting) Then
         If oPointExisting.Name <> sAcadName Then
            DMAcadExt.AcadDocument.WriteMessage("Do & New  " & CStr(oPointExisting.Name) & "<->" & sAcadName & "; " & tPointKey.ToString())
            '   DMAcadExt.AcadDocument.WriteMessage("New: " & CStr(sAcadName))

            mdicNames.Remove(oPointExisting.Name)
            '  oPointExisting.Name = "ZZZZ" & sAcadName
            oPointExisting.UpdateByAcad(sAcadName)
            '  DMAcadExt.AcadDocument.WriteMessage("After Acad: " & oPointExisting.AcadName)
            '  DMAcadExt.AcadDocument.WriteMessage("After Name: " & oPointExisting.Name)
            Try
               mdicNames.Add(oPointExisting.Name, oPointExisting)
            Catch ex As Exception
               DMAcadExt.AcadDocument.WriteMessage("After  " & CStr(ex.Message))
            End Try

         End If
      End If
   End Sub
   Sub AddBlockRefData(tBlockRefData As DMAcadExt.BlockRefData)
      Dim saAttribValues() As String = tBlockRefData.AttribValues
      Dim oPoint As UD_Point = Nothing
      If mdicNames.TryGetValue(saAttribValues(0), oPoint) Then
         oPoint.ScaleFactors = tBlockRefData.ScaleFactors
      End If

   End Sub
   Public Sub BlockingPoint(tPosition As Autodesk.AutoCAD.Geometry.Point3d)
      Dim oPoint As UD_Point = Nothing
      If Me.TryGetPoint(tPosition, oPoint) Then
         oPoint.IsUserBlocking = True
      End If
   End Sub

   Public Function GetSortedArray() As System.Collections.ObjectModel.Collection(Of UD_Point)
      Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Point) = New System.Collections.ObjectModel.Collection(Of UD_Point)
      For Each oUD_Point As UD_Point In mdicNames.Values
         If oUD_Point.Stage <> -1 Then
            colRes.Add(oUD_Point)
         End If
      Next
      Return colRes
   End Function
   Public Function TryGetPoint(dX As Double, dY As Double, ByRef oPoint As UD_Point) As Boolean
      Dim ulKey As ULong = DMAcadExt.TplnPointKeyLong.CoordToKey(dX, dY)
      Return MyBase.TryGetValue(ulKey, oPoint)
   End Function

   Public Function TryGetPoint(tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByRef oPoint As UD_Point) As Boolean
      Dim ulKey As ULong = DMAcadExt.TplnPointKeyLong.CoordToKey(tPoint.X, tPoint.Y)
      '	DMAcadExt.AcadDocument.WriteMessage("Try " & CStr(dcKey) & "; ")
      Return MyBase.TryGetValue(ulKey, oPoint)
   End Function

   Public Function TryGetPoint(sPointName As String, ByRef oPoint As UD_Point) As Boolean

      Return mdicNames.TryGetValue(sPointName, oPoint)
   End Function
   Public Function TryGetPoint(tAcObjID As ObjectId, ByRef oPoint As UD_Point) As Boolean

      Return mdicObjectIDs.TryGetValue(tAcObjID, oPoint)
   End Function
	Public Sub SetStartNum(iInputNum As Integer)

	End Sub
	Property StartNewNum As Integer
		Get
			Return miStartNewNum
		End Get
		Set(iValue As Integer)
			miStartNewNum = iValue
		End Set
	End Property

	Public Sub New()
      mdicNames = New SortedDictionary(Of String, UD_Point)(New PointNameComparer())
      Dim tCenterPoint As Autodesk.AutoCAD.Geometry.Point2d = DMAcadExt.AcadDocument.GetCenterPoint()
      Dim tMinPoint As Autodesk.AutoCAD.Geometry.Point2d = DMAcadExt.AcadDocument.GetExtMinPoint()
      Dim tMaxPoint As Autodesk.AutoCAD.Geometry.Point2d = DMAcadExt.AcadDocument.GetExtMaxPoint()


      DMAcadExt.TplnPointKey.SetCenter(Math.Round(tCenterPoint.X), Math.Round(tCenterPoint.Y))
      DMAcadExt.TplnPointKeyLong.SetCenter(Math.Round(tCenterPoint.X), Math.Round(tCenterPoint.Y))
      DMAcadExt.TplnPointKeyLong.SetExtension(tMinPoint, tMaxPoint)

   End Sub
   Private Class PointNameComparer
      Inherits System.Collections.Generic.Comparer(Of String)



      Public Overrides Function Compare(sPointNameA As String, sPointNameB As String) As Integer
         Dim sStrValueA As String = Nothing
         Dim sStrValueB As String = Nothing
         Dim iNumValueA As Integer = zzToInt(sPointNameA, sStrValueA)
         Dim iNumValueB As Integer = zzToInt(sPointNameB, sStrValueB)

         If iNumValueA = iNumValueB Then
            Return sStrValueA.CompareTo(sStrValueB)
         Else
            Return iNumValueA.CompareTo(iNumValueB)
         End If




      End Function
      Private Function zzToInt(sValue As String, ByRef sRes As String) As Integer
         Dim dNumValue As Double = Val(sValue)
         If dNumValue > 0 Then
            Dim iResValue As Integer = Convert.ToInt32(Math.Ceiling(dNumValue))
            sRes = sValue.Substring(Convert.ToString(iResValue).Length)
            Return iResValue
         Else
            sRes = sValue
            Return Integer.MaxValue
         End If


      End Function
   End Class
End Class
