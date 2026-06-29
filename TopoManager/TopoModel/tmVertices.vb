Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Geometry
Public Enum enStatusVertexSegment
	PseudoPoints
	DoubtPoints
	Break
End Enum
Public Class tmVertices
	Private moaVertices() As tmVertex
	'  ObjectModel.Collection(Of tmVertex)
	'	Inherits Dictionary(Of ULong, tmVertex)
	Private mBaseNet As tmNet
	Private mPlusNet As tmNet
	Private mMinusNet As tmNet
	Private msHandle As String


	Private Shared miRoundDecimal As Integer

	'Private Shared mtPointTolerance As Autodesk.AutoCAD.Geometry.Tolerance
	Private Shared mdRoundScale As Double
	Private Shared mdPointTolerance As Double
	Private Shared mdNetOffset As Double

	'	Private Shared mdNetScale As Double
	Private Shared mdOriginScale As Double

	'	Private Shared mdXScale As Double
	'	Private Shared mdYScale As Double
	Private Shared mtCheckPseudoTolerance As Tolerance = New Tolerance(0.01, 0.01)	  'New Tolerance(0.05, 0.05)   'New Tolerance(0.35, 0.35) OK
	Private Shared mtCheckDoubtTolerance As Tolerance = New Tolerance(0.5, 0.5)	  'New Tolerance(0.05, 0.05) 

	' New Tolerance(0.5, 0.5) A,b -Yes  All-No
	'New Tolerance(0.2, 0.2)   All-No,C-No
	Private mulXShift As ULong
	Private miUB As Integer
	Private mbLastIsFirst As Boolean
	Private mdOriginX As Double
	Private mdOriginY As Double
	Private mbDirection As Boolean
	'Public X As Long
	'Public Y As Long
	Public Shared Sub SetRoundDec(iRoundDecimal As Integer)
		miRoundDecimal = iRoundDecimal
		mdRoundScale = Math.Pow(10.0, iRoundDecimal)

	End Sub
	Public Shared Sub SetTolerance(dPointTolerance As Double)
		mdPointTolerance = dPointTolerance
		'	mdNetScale = 1.0 / (3.0 * mdPointTolerance)
      mdOriginScale = Math.Round(1.0 / mdPointTolerance)
		mdNetOffset = mdPointTolerance / 3.0
	End Sub
	Public Sub New(tExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d, iUB As Integer, sHandle As String)
		MyBase.New()
		msHandle = sHandle
		miUB = iUB
		ReDim moaVertices(iUB)

		Dim dOriginX As Double
		Dim dOriginY As Double
		'	Dim dOriginDelta As Double = 0.001




		Dim ulColumnCount As ULong



		dOriginX = Math.Floor(tExtents.MinPoint.X * mdOriginScale) * mdPointTolerance - mdPointTolerance
		dOriginY = Math.Floor(tExtents.MinPoint.Y * mdOriginScale) * mdPointTolerance - mdPointTolerance


		'Dim s As String
		Dim dMaxX As Double = Math.Ceiling((tExtents.MaxPoint.X - dOriginX) * mdOriginScale)
		'	Dim dOffset As Double  = mdPointTolerance / 3.0
		ulColumnCount = Convert.ToUInt64(dMaxX) + 1UL

		mPlusNet = New tmNet(dOriginX + mdNetOffset, dOriginY + mdNetOffset, ulColumnCount)
		mMinusNet = New tmNet(dOriginX - mdNetOffset, dOriginY - mdNetOffset, ulColumnCount)
		mBaseNet = New tmNet(dOriginX, dOriginY, ulColumnCount)


      '	DMAcadExt.AcadDocument.WriteMessage("BASE " & CStr(mBaseNet.OriginX) & "," & CStr(mBaseNet.OriginY))
      '	DMAcadExt.AcadDocument.WriteMessage("Plus " & CStr(mPlusNet.OriginX) & "," & CStr(mPlusNet.OriginY))
      '	DMAcadExt.AcadDocument.WriteMessage("Min " & CStr(mMinusNet.OriginX) & "," & CStr(mMinusNet.OriginY))



	End Sub

	Public Sub NewOld(tExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d, iUB As Integer, sHandle As String)
		'	MyBase.New()
		msHandle = sHandle
		Dim dOriginX As Double
		Dim dOriginY As Double
		Dim dOriginDelta As Double = 0.001

		Dim dMaxY As Double = tExtents.MaxPoint.Y - tExtents.MinPoint.Y
		dMaxY = Math.Ceiling(dMaxY)
		Dim iMax As Integer = Convert.ToInt32(dMaxY)
		Dim iPower As Integer = Convert.ToString(iMax).Length
		Dim iRoundMax As Integer = 1
		For iIndex As Integer = 1 To iPower + miRoundDecimal
			iRoundMax *= 10
		Next
		mulXShift = Convert.ToUInt64(iRoundMax)
		miUB = iUB
		ReDim moaVertices(iUB)
		dOriginX = Math.Floor(tExtents.MinPoint.X * 1000) * dOriginDelta - dOriginDelta
		dOriginY = Math.Floor(tExtents.MinPoint.Y * 1000) * dOriginDelta - dOriginDelta
		mPlusNet = New tmNet(dOriginX, dOriginY, mulXShift)
		dOriginX -= dOriginDelta
		dOriginY -= dOriginDelta
		mBaseNet = New tmNet(dOriginX, dOriginY, mulXShift)
		dOriginX -= dOriginDelta
		dOriginY -= dOriginDelta
		mMinusNet = New tmNet(dOriginX, dOriginY, mulXShift)
		If dOriginX < 0 Or dOriginY < 0 Then
			MessageBox.Show(dOriginX.ToString() & vbCrLf & dOriginY.ToString() & vbCrLf & mulXShift.ToString() & vbCrLf, "02_544s")
		End If
	End Sub

	Public Sub DrawBreakPoints()
		Dim oVertex As tmVertex
		For iIndex As Integer = 0 To miUB

			oVertex = moaVertices(iIndex)
			If oVertex.Node Is Nothing AndAlso Not oVertex.IsPseudo Then
				DMAcadExt.AcadTransaction.InsertPoint(oVertex.Point2d, , , "break")
			End If

		Next
   End Sub
   Public Sub PrintVertices()
      For iIndex As Integer = 0 To moaVertices.GetUpperBound(0)
         DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "; " & moaVertices(iIndex).Point2d.ToString())

      Next

   End Sub

	Public Function GetCurve() As Autodesk.AutoCAD.Geometry.CompositeCurve2d
		Dim oaCurves(miUB) As Curve2d
		Dim oNextVertex, oVertex As tmVertex
		For iIndex As Integer = 0 To miUB
			oVertex = moaVertices(iIndex)
			oNextVertex = zzGetVertex(iIndex + 1)
			oaCurves(iIndex) = New Line2d(oVertex.Point2d, oNextVertex.Point2d)
		Next
		Return New CompositeCurve2d(oaCurves)
	End Function
   Public Function AddVertex(iRingID As Integer, iIndex As Integer, tPoint As Point3d, bIsLast As Boolean) As Boolean
      Dim oVertex As tmVertex = New tmVertex(iRingID, iIndex, tPoint)
      Dim bContains As Boolean
      Dim bBaseContains As Boolean
      Dim bPlusContains As Boolean
      Dim bMinusContains As Boolean
      Dim iIndexExists As Integer = -1

      moaVertices(iIndex) = oVertex
      bBaseContains = mBaseNet.TryGetVertex(tPoint, iIndexExists)
      '  DMAcadExt.AcadDocument.WriteMessage(bBaseContains.ToString() & " BaseIndex:" & iIndexExists.ToString() & " cnt=" & mBaseNet.Count.ToString())
      bPlusContains = mPlusNet.TryGetVertex(tPoint, iIndexExists)
      '  DMAcadExt.AcadDocument.WriteMessage(bPlusContains.ToString() & " PlusIndex:" & iIndexExists.ToString() & " cnt=" & mBaseNet.Count.ToString())
      bMinusContains = mMinusNet.TryGetVertex(tPoint, iIndexExists)
      '  DMAcadExt.AcadDocument.WriteMessage(bMinusContains.ToString() & " MinusIndex:" & iIndexExists.ToString() & " cnt=" & mBaseNet.Count.ToString())

      If bBaseContains OrElse bPlusContains OrElse bMinusContains Then
         bContains = True
         DMAcadExt.AcadDocument.WriteDebugMessage("bContains:" & iIndex.ToString() & ", " & tPoint.ToString() & ";" & mBaseNet.ContainsPoint(tPoint).ToString() & ";" & mPlusNet.ContainsPoint(tPoint).ToString() & ";" & mMinusNet.ContainsPoint(tPoint).ToString())
         If iIndex = iIndexExists + 1 Then
            miUB -= 1
            Return False
         End If

      Else
         bContains = False

         mBaseNet.AddVertex(iIndex, tPoint)
         mPlusNet.AddVertex(iIndex, tPoint)
         mMinusNet.AddVertex(iIndex, tPoint)

      End If
      Try
         '	DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "All Count:" & CStr(Me.Count) & "," & "Key:" & CStr(lKey))
         If bContains Then
            If bIsLast Then
               mbLastIsFirst = True
               miUB -= 1
            Else
               If iIndex < 999999 Then
                  Dim bBase As Boolean = mBaseNet.ContainsPoint(tPoint)
                  Dim bPlus As Boolean = mPlusNet.ContainsPoint(tPoint)
                  Dim bMinus As Boolean = mMinusNet.ContainsPoint(tPoint)
                  Dim iVertexIndex As Integer
                  Dim oVertexExists As tmVertex = Nothing
                  Dim tPointExists As Point3d
                  Dim ulKey As ULong
                  Dim ulKeyExists As ULong

                  If bBase Then
                     If mBaseNet.TryGetVertex(tPoint, iVertexIndex) Then
                        oVertexExists = moaVertices(iVertexIndex)
                        tPointExists = oVertexExists.Point3d
                        ulKey = mBaseNet.GetKey(tPoint, False)
                        ulKeyExists = mBaseNet.GetKey(tPointExists)
                        '	DMAcadExt.AcadDocument.WriteMessageLog("Base:" & CStr(ulKey) & ";" & CStr(ulKeyExists) & "--" & tPoint.ToString() & ";" & tPointExists.ToString())
                     End If
                  Else
                     If bPlus Then
                        If mPlusNet.TryGetVertex(tPoint, iVertexIndex) Then
                           oVertexExists = moaVertices(iVertexIndex)
                           tPointExists = oVertexExists.Point3d
                           ulKey = mPlusNet.GetKey(tPoint)
                           ulKeyExists = mPlusNet.GetKey(tPointExists)
                           DMAcadExt.AcadDocument.WriteMessageLog("Plus:" & CStr(ulKey) & ";" & CStr(ulKeyExists) & "--" & tPoint.ToString() & ";" & tPointExists.ToString())
                        End If
                     Else
                        If bMinus Then
                           If mMinusNet.TryGetVertex(tPoint, iVertexIndex) Then
                              oVertexExists = moaVertices(iVertexIndex)
                              tPointExists = oVertexExists.Point3d
                              ulKey = mMinusNet.GetKey(tPoint)
                              ulKeyExists = mMinusNet.GetKey(tPointExists)
                              DMAcadExt.AcadDocument.WriteMessageLog("Minus:" & CStr(ulKey) & ";" & CStr(ulKeyExists) & "--" & tPoint.ToString() & ";" & tPointExists.ToString())
                           End If
                        End If
                     End If
                  End If
                  '	DMAcadExt.AcadDocument.WriteMessageLog("R=" & CStr(iRingID) & " i=" & CStr(iIndex) & "," & "Double Vertex:" & DMAcadExt.TPlnPoint.DispPoint(tPoint) & ";" & DMAcadExt.TPlnPoint.DispPoint(oVertexExists.Point2d))
               End If
            End If
         End If
         Return True
         'DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "OK:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))
      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_670")
         Return False

      End Try
   End Function
   Public Sub CheckPseudo(iID As Integer)
      Dim oPrevVertex, oVertex, oNextVertex As tmVertex
      Dim tNextVector As Vector3d
      Dim tPrevVector As Vector3d
      Dim dAngle As Double
      Dim dAngleSum As Double = 0.0
      Dim tProd As Vector3d
      Dim iPseudoCount As Integer = 0
      For iIndex As Integer = 0 To miUB
         oVertex = moaVertices(iIndex)
         oPrevVertex = zzGetVertex(iIndex - 1)
         oNextVertex = zzGetVertex(iIndex + 1)

         tPrevVector = oPrevVertex.Point3d.GetVectorTo(oVertex.Point3d)
         tNextVector = oVertex.Point3d.GetVectorTo(oNextVertex.Point3d)

         If tPrevVector.IsCodirectionalTo(tNextVector, mtCheckPseudoTolerance) Then
            oVertex.IsPseudo = True
            iPseudoCount += 1
            '	DMAcadExt.AcadDocument.WriteMessage("CH i=" & CStr(iIndex) & ", " & "pt: " & DMAcadExt.TPlnPoint.DispPoint(oVertex.Point3d))
            DMAcadExt.AcadTransaction.InsertPoint(oVertex.Point3d, , , "pseudo")
         ElseIf tPrevVector.IsCodirectionalTo(tNextVector, mtCheckDoubtTolerance) Then
            oVertex.IsDoubt = True
         Else

         End If
         dAngle = tNextVector.GetAngleTo(tPrevVector)
         tProd = tPrevVector.CrossProduct(tNextVector)
         'DMAcadExt.AcadDocument.WriteMessage("&&-!Prod=" & CStr(tProd.Z) & "; ind=" & CStr(iIndex))
         dAngleSum += dAngle * Math.Sign(tProd.Z)
      Next
      mbDirection = (dAngleSum > 0.0)
      '	DMAcadExt.AcadDocument.WriteMessage("H=" & msHandle & " PseudoCount: " & iPseudoCount.ToString() & "; Tol=" & mtCheckPseudoTolerance.EqualVector.ToString())
      '	DMAcadExt.AcadDocument.WriteMessage("--!dAngle=" & CStr(dAngleSum) & "; i=" & CStr(i))
      '	DMAcadExt.AcadDocument.WriteMessage("@@@dAngle=" & CStr(dA) & "; iLine=" & CStr(i))

   End Sub
	Public Function ConnectNode(oNode As tmNode, sDebugNote As String) As Integer
		Dim iVertexIndex As Integer = -1
		Dim bExists As Boolean
		Dim oVertex As tmVertex = Nothing
		If Not mBaseNet.TryGetVertex(oNode.Point3d, iVertexIndex) Then
			If Not mPlusNet.TryGetVertex(oNode.Point3d, iVertexIndex) Then
				If mMinusNet.TryGetVertex(oNode.Point3d, iVertexIndex) Then
					bExists = True
				Else
               DMAcadExt.AcadDocument.WriteDebugMessage("--tmp " & sDebugNote & " H=" & msHandle & " pt=" & DMAcadExt.TPlnPoint.DispPoint(oNode.Point3d))

               bExists = False


            End If
			Else
				bExists = True
			End If
		Else

			bExists = True
		End If
		If bExists Then
			oVertex = moaVertices(iVertexIndex)
			oVertex.ConnectNode(oNode)
			'DMAcadExt.AcadDocument.WriteMessage("!M01 " & oNode.Point3d.ToString() & "; VertI=" & CStr(oVertex.Index) & "; I=" & CStr(iVertexIndex))
		Else
         DMAcadExt.AcadDocument.WriteLog("Node was not found:" & DMAcadExt.TPlnPoint.DispPoint(oNode.Point3d))
			iVertexIndex = -1
		End If
		Return iVertexIndex
	End Function
	
	Public ReadOnly Property Direction As Boolean
		Get
			Return mbDirection
		End Get
	End Property
	Public ReadOnly Property UB As Integer
		Get
			Return miUB
		End Get
	End Property
	Public ReadOnly Property Vertex(iIndex As Integer) As tmVertex
		Get
			Return moaVertices(iIndex)
		End Get
	End Property
	Public ReadOnly Property LastIsFirst() As Boolean
		Get
			Return mbLastIsFirst
		End Get
	End Property
	Public ReadOnly Property LastVertex As tmVertex
		Get
			Return moaVertices(miUB)
		End Get
	End Property
	Public Function VertexCount(iStartVertex As Integer, iEndVertex As Integer) As Integer
		Dim iVertexCount As Integer = iEndVertex - iStartVertex
		If iVertexCount <= 1 Then
			iVertexCount += miUB + 1
		End If
		Return iVertexCount
	End Function
	Public Function GetAllNodes() As tmNode()
		Dim oaNodes(miUB) As tmNode
		For iIndex As Integer = 0 To miUB
			oaNodes(iIndex) = Me.Vertex(iIndex).Node
		Next
		Return oaNodes
	End Function
	Public Function IsNextVertex(iStartIndex As Integer, iEndIndex As Integer, bForwardDirection As Boolean) As Boolean
      Dim iCalcIndex As Integer = GetNextIndex(iStartIndex, bForwardDirection)
      If False Then
         If bForwardDirection Then

            iCalcIndex = zzGetNextIndex(iStartIndex)
         Else
            iCalcIndex = zzGetPreviousIndex(iStartIndex)
         End If
      End If

      Return (iCalcIndex = iEndIndex)
   End Function
   Public Function GetPseudoVertices(iStartIndex As Integer, iEndIndex As Integer, ByRef oaPseudoVertices() As tmVertex, ByRef oaDoubtfulVertices() As tmVertex, bForwardDirection As Boolean) As Boolean
      Dim iIndex As Integer = iStartIndex
      Dim iUB As Integer = iEndIndex - iStartIndex - 1
      Dim iResPseudoIndex As Integer = 0
      Dim iResDoubtfulIndex As Integer = 0

      Dim oVertex As tmVertex
      If bForwardDirection Then
         iUB = iEndIndex - iStartIndex - 1
      Else
         iUB = iStartIndex - iEndIndex - 1
      End If
      If iUB < 0 Then
         iUB += miUB + 1
      End If
      ReDim oaPseudoVertices(iUB)
      ReDim oaDoubtfulVertices(iUB)

      'DMAcadExt.AcadDocument.WriteMessage("Pseudo: " & CStr(iStartIndex) & "<=>" & CStr(iEndIndex) & "; UB=" & CStr(iUB))
      Do
         iIndex = GetNextIndex(iIndex, bForwardDirection)
         If iIndex = iEndIndex Then
            Return True
         Else
            oVertex = moaVertices(iIndex)
            If oVertex.IsPseudo Then
               oaPseudoVertices(iResPseudoIndex) = oVertex
               iResPseudoIndex += 1
            Else
               Return False
            End If
         End If
         Try
            'oaVertices(iCurrentResIndex) = moaVertices(iIndex)
            'iCurrentResIndex += 1
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("???##" & oEx.Message & "; H=" & msHandle & "||" & "; UB=" & CStr(miUB) & ";" & CStr(oaPseudoVertices.GetUpperBound(0)) & "<>" & CStr(99))
            '
         End Try
      Loop

   End Function
   Public Function GetPseudoVertices(iStartIndex As Integer, iEndIndex As Integer, ByRef oaVertices() As tmVertex, bForwardDirection As Boolean) As enStatusVertexSegment
      Dim iIndex As Integer = iStartIndex
      Dim iUB As Integer = iEndIndex - iStartIndex - 1
      Dim iCurrentResIndex As Integer = 0
      Dim oVertex As tmVertex
      Dim bHasDoubt As Boolean
      If bForwardDirection Then
         iUB = iEndIndex - iStartIndex - 1
      Else
         iUB = iStartIndex - iEndIndex - 1
      End If
      If iUB < 0 Then
         iUB += miUB + 1
      End If
      ReDim oaVertices(iUB)
      'DMAcadExt.AcadDocument.WriteMessage("Pseudo: " & CStr(iStartIndex) & "<=>" & CStr(iEndIndex) & "; UB=" & CStr(iUB))
      Do
         iIndex = GetNextIndex(iIndex, bForwardDirection)
         If iIndex = iEndIndex Then
            ReDim Preserve oaVertices(iCurrentResIndex - 1)
            If bHasDoubt Then
               Return enStatusVertexSegment.DoubtPoints
            Else
               Return enStatusVertexSegment.PseudoPoints
            End If

         Else
            oVertex = moaVertices(iIndex)
            If oVertex.IsPseudo Then
               oaVertices(iCurrentResIndex) = oVertex
               iCurrentResIndex += 1
            ElseIf oVertex.IsDoubt Then
               oaVertices(iCurrentResIndex) = oVertex
               bHasDoubt = True
               iCurrentResIndex += 1
            Else

               Return enStatusVertexSegment.Break
            End If
         End If
         Try
            'oaVertices(iCurrentResIndex) = moaVertices(iIndex)
            'iCurrentResIndex += 1
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("???##" & oEx.Message & "; H=" & msHandle & "||" & "; UB=" & CStr(miUB) & ";" & CStr(oaVertices.GetUpperBound(0)) & "<>" & CStr(99))
            '
         End Try
      Loop

   End Function
   Public Sub GetPseudoVertices(iStartIndex As Integer, iEndIndex As Integer, ByRef oaVertices() As tmVertex, ByRef iCurrentResIndex As Integer, ByRef bErr As Boolean)

      Dim sTestPrint As String = CStr(iStartIndex) & "<->" & CStr(iEndIndex) & "!P=" & CStr(iCurrentResIndex)
      Dim iIndex As Integer = iStartIndex
      Do
         iIndex = zzGetNextIndex(iIndex)
         If iIndex = iEndIndex Then
            Exit Do
         End If
         Try
            If iStartIndex > 700 And iStartIndex < 720 Then
               DMAcadExt.AcadDocument.WriteMessage("+++##" & sTestPrint & "; UB=" & CStr(miUB) & ";" & CStr(iIndex) & ";IsPs-" & CStr(moaVertices(iIndex).IsPseudo))
            End If
            oaVertices(iCurrentResIndex) = moaVertices(iIndex)
            iCurrentResIndex += 1
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("???##" & oEx.Message & "; H=" & msHandle & "||" & sTestPrint & "; UB=" & CStr(miUB) & ";" & CStr(oaVertices.GetUpperBound(0)) & "<>" & CStr(iCurrentResIndex))
            bErr = True
            Return
         End Try
      Loop
   End Sub
   Public Function RealVertexiExists(iVertexANo As Integer, iVertexBNo As Integer) As Boolean
      Dim iIndex As Integer = iVertexANo
      Do
         iIndex = zzGetNextIndex(iIndex)
         If iIndex = iVertexBNo Then
            Return False
         End If
      Loop While moaVertices(iIndex).IsPseudo
      '	DMAcadExt.AcadDocument.WriteMessage("ISNOT Pseudo! " & CStr(iIndex))
      Return True
   End Function
   Public Function GetDir(iStartIndex As Integer, iMidIndex As Integer, iEndIndex As Integer) As Boolean
      If iStartIndex > iEndIndex Then
         If iMidIndex > iStartIndex OrElse iMidIndex < iEndIndex Then
            Return True
         Else
            Return False
         End If
      Else
         If iMidIndex > iStartIndex AndAlso iMidIndex < iEndIndex Then
            Return True
         Else
            Return False
         End If
      End If
   End Function
  
   Public Function GetNextVertex(iVertexIndex As Integer) As tmVertex
      Return moaVertices(zzGetNextIndex(iVertexIndex))
   End Function
   Public Function GetNextVertex(iVertexIndex As Integer, bForwardDirection As Boolean) As tmVertex
      Return moaVertices(GetNextIndex(iVertexIndex, bForwardDirection))
   End Function
   Public Sub GetPseudoVertices(iStartIndex As Integer, iEndIndex As Integer, iStep As Integer, ByRef oaVertices() As tmVertex, ByRef iCurrentResIndex As Integer, ByRef bErr As Boolean)
      Dim oVertex As tmVertex
      Dim sTestPrint As String = CStr(iStartIndex) & "<->" & CStr(iEndIndex) & ",s=" & CStr(iStep)
      iStartIndex = zzGetVertex(iStartIndex + iStep).Index
      If iStartIndex <> iEndIndex Then
         iEndIndex = zzGetVertex(iEndIndex - iStep).Index
         For iIndex As Integer = iStartIndex To iEndIndex Step iStep
            oVertex = zzGetVertex(iIndex)
            '	DMAcadExt.AcadDocument.WriteMessage("i=" & CStr(iIndex) & "; VInd=" & CStr(oVertex.Index))
            If oVertex.IsPseudo Then

               ''''''''''''''''''''''''''		DMAcadExt.AcadDocument.WriteMessage("!!## vInd=" & CStr(oVertex.Index) & "; H=" & msHandle & "; " & sTestPrint & " UB=" & CStr(miUB) & ";" & CStr(oaVertices.GetUpperBound(0)) & "-->" & CStr(iCurrentResIndex))
               Try
                  oaVertices(iCurrentResIndex) = oVertex
                  iCurrentResIndex += 1
               Catch oEx As Exception
                  DMAcadExt.AcadDocument.WriteMessage("??-##" & oEx.Message & "; H=" & msHandle & "; UB=" & CStr(miUB) & ";" & CStr(oaVertices.GetUpperBound(0)) & "<>" & CStr(iCurrentResIndex))
                  bErr = True
                  Exit Sub
               End Try


            End If

         Next

      End If

   End Sub

   Private Function zzGetPreviousIndex(iIndex As Integer) As Integer
      If iIndex > 0 Then
         Return iIndex - 1
      Else
         Return miUB
      End If

   End Function
   Private Function zzGetNextIndex(iIndex As Integer) As Integer
      Return (iIndex + 1) Mod (miUB + 1)
   End Function
   Public Function GetNextIndex(iIndex As Integer, bForwardDirection As Boolean) As Integer
      If bForwardDirection Then
         Return zzGetNextIndex(iIndex)
      Else
         Return zzGetPreviousIndex(iIndex)
      End If

   End Function
   Public Function GetNextIndexForAdd(iIndex As Integer, bForwardDirection As Boolean) As Integer
      If bForwardDirection Then
         Return iIndex + 1
      Else
         Return iIndex

      End If

   End Function

	Private Function zzGetVertex(iNumber As Integer) As tmVertex
		Dim iVertexNo As Integer
		If iNumber >= 0 Then
			iVertexNo = iNumber Mod (miUB + 1)
		Else
			iVertexNo = (iNumber + miUB + 1) Mod (miUB + 1)
		End If
		Return moaVertices(iVertexNo)
	End Function
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
         Dim ulKey As ULong = zzGetPointKey(tPoint, False)
			Return MyBase.ContainsKey(ulKey)
		End Function
		Public Sub AddVertex(iVertexIndex As Integer, tPoint As Point3d)
			Dim ulKey As ULong = zzGetPointKey(tPoint)
			Try
				'	DMAcadExt.AcadDocument.WriteMessage("AddV:" & CStr(iVertexIndex) & "," & "All Count:" & CStr(Me.Count) & "," & "Key:" & CStr(ulKey))
				MyBase.Add(ulKey, iVertexIndex)

				'DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "OK:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))

			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_623")
			End Try
		End Sub
		Public Function TryGetVertex(tPoint As Point3d, ByRef iVertexIndex As Integer) As Boolean

			Dim ulKey As ULong = zzGetPointKey(tPoint)
			'		Dim oVertex As tmVertex = Nothing
			If MyBase.TryGetValue(ulKey, iVertexIndex) Then
				'	DMAcadExt.AcadDocument.WriteMessage("$$tmp " & CStr(oVertex.RingID) & "//" & CStr(oVertex.Index) & ", VN=" & sTmp & ", N=" & CStr(oNode.ID) & " P=" & DMAcadExt.TPlnPoint.DispPoint(oNode.Point2d))
				Return True
			Else
            '	DMAcadExt.AcadDocument.WriteMessage("Node was not found:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))
            iVertexIndex = -1
				Return False

			End If


      End Function
     
		Public Function GetKey(tPoint As Point3d, Optional bDebug As Boolean = False) As ULong

			Return zzGetPointKey(tPoint, bDebug)



		End Function
		Public ReadOnly Property OriginX As Double
			Get
				Return mdOriginX
			End Get
		End Property
		Public ReadOnly Property OriginY As Double
			Get
				Return mdOriginY
			End Get
      End Property
      Private Function zzGetPointKeyTEST(tPoint As Point3d, Optional bDebug As Boolean = False) As ULong
         Dim dX, dY As Double
         Dim ulX, ulY As ULong
         'DMAcadExt.TplnPointKeyLong.CoordToKey(
         Try
            dX = Math.Round((tPoint.X - mdOriginX) * mdOriginScale)
            dY = Math.Round((tPoint.Y - mdOriginY) * mdOriginScale)
            ulX = Convert.ToUInt64(dX)
            ulY = Convert.ToUInt64(dY)
            If bDebug Then
               DMAcadExt.AcadDocument.WriteMessage("%%%" & CStr(ulX) & ";" & CStr(ulY) & " Scale:" & CStr(mdOriginScale))
            End If
            Return ulX + ulY * mulColumnCount
         Catch oEx As Exception
            Return 0UL
         End Try
      End Function


		Private Function zzGetPointKey(tPoint As Point3d, Optional bDebug As Boolean = False) As ULong
			Dim dX, dY As Double
			Dim ulX, ulY As ULong
			Try
				dX = Math.Round((tPoint.X - mdOriginX) * mdOriginScale)
				dY = Math.Round((tPoint.Y - mdOriginY) * mdOriginScale)
				ulX = Convert.ToUInt64(dX)
				ulY = Convert.ToUInt64(dY)
				If bDebug Then
					DMAcadExt.AcadDocument.WriteMessage("%%%" & CStr(ulX) & ";" & CStr(ulY) & " Scale:" & CStr(mdOriginScale))
				End If
				Return ulX + ulY * mulColumnCount
			Catch oEx As Exception
				Return 0UL
			End Try
      End Function
      'Private Function CoordToKey(dX As Double, dY As Double) As ULong
      '   dX = Math.Round(RoundShift * (dX - mdCenterX), 0, MidpointRounding.AwayFromZero)
      '   dY = Math.Round(RoundShift * (dY - mdCenterY), 0, MidpointRounding.AwayFromZero)
      '   Dim lSignX, lSignY As ULong
      '   If dX >= 0.0 Then
      '      lSignX = 0&
      '   Else
      '      lSignX = 1&
      '   End If
      '   If dX >= 0.0 Then
      '      lSignY = 0&
      '   Else
      '      lSignY = 2&
      '   End If

      '   Return (lSignX + lSignY) * ShiftSign + ShiftX * Convert.ToUInt64(Math.Abs(dX)) + Convert.ToUInt64(Math.Abs(dY))

      'End Function
	End Class
	Private Class tmNetOld
		Inherits Dictionary(Of ULong, Integer)
		'	Private Shared miRoundDecimal As Integer
		Private mdOriginX As Double
		Private mdOriginY As Double
		Private Shared mtPointTolerance As Autodesk.AutoCAD.Geometry.Tolerance
		'	Private Shared mdRoundScale As Double
		Private Shared mdXScale As Double
		Private Shared mdYScale As Double
		Private mulXShift As ULong
		Private miUB As Integer
		Private Shared iTestCounter As Integer
		Public Sub New(dOriginX As Double, dOriginY As Double, ulXShift As ULong)
			MyBase.New()
			mdOriginX = dOriginX
			mdOriginY = dOriginY
			mulXShift = ulXShift
		End Sub
		Public Function ContainsPoint(tPoint As Point3d, sHandle As String) As Boolean
			Dim ulKey As ULong = zzGetPointKey(tPoint, sHandle)
			Return MyBase.ContainsKey(ulKey)
		End Function
		Public Sub AddVertex(iVertexIndex As Integer, tPoint As Point3d, sHandle As String)

			Dim lKey As ULong = zzGetPointKey(tPoint, sHandle)


			Try
				'	DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "All Count:" & CStr(Me.Count) & "," & "Key:" & CStr(lKey))
				If MyBase.ContainsKey(lKey) Then

				Else

					MyBase.Add(lKey, iVertexIndex)
				End If

				'DMAcadExt.AcadDocument.WriteMessage(CStr(iIndex) & "," & "OK:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))




			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_623")
			End Try
		End Sub
		Public Function TryGetVertex(tPoint As Point3d, ByRef iVertexIndex As Integer, sHandle As String) As Boolean
			If tPoint.X > mdOriginX AndAlso tPoint.Y > mdOriginY Then
				Dim lKey As ULong = zzGetPointKey(tPoint, sHandle)
				'		Dim oVertex As tmVertex = Nothing
				If MyBase.TryGetValue(lKey, iVertexIndex) Then
					'	DMAcadExt.AcadDocument.WriteMessage("$$tmp " & CStr(oVertex.RingID) & "//" & CStr(oVertex.Index) & ", VN=" & sTmp & ", N=" & CStr(oNode.ID) & " P=" & DMAcadExt.TPlnPoint.DispPoint(oNode.Point2d))
					Return True
				Else
					'DMAcadExt.AcadDocument.WriteMessage("Node was not found:" & DMAcadExt.TPlnPoint.DispPoint(tPoint))
					Return False

				End If
			Else
				Return False
			End If

		End Function
		Private Function zzGetPointKey(tPoint As Point3d, sHandle As String) As ULong
			Dim dX, dY As Double
			Dim lX, lY As ULong
			Try
				dX = Math.Round(tPoint.X - mdOriginX, miRoundDecimal) * mdRoundScale
				dY = Math.Round(tPoint.Y - mdOriginY, miRoundDecimal) * mdRoundScale
				lX = Convert.ToUInt64(dX)
				lY = Convert.ToUInt64(dY)
				Return mulXShift * lX + lY
			Catch oEx As Exception
				If iTestCounter < 20 Then
					DMAcadExt.AcadDocument.WriteMessage("!@!" & sHandle & vbCrLf & tPoint.ToString() & vbCrLf & mdOriginX.ToString() & vbCrLf & mdOriginY.ToString() & vbCrLf & dX.ToString() & vbCrLf & dY.ToString() & vbCrLf & lX.ToString() & vbCrLf & lY.ToString() & vbCrLf & mulXShift.ToString())
					iTestCounter += 1
				End If

				Return 0UL
			End Try

		End Function
	End Class
End Class
