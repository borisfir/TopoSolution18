Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme
   Public Class tsBranch
      Inherits tsElement
      Private miLeftPolygon As Integer = -1
      Private miRightPolygon As Integer = -1
      ' Private mtAcObjID As ObjectId
      Private miPreviousNodeID As Integer
      Private miNextNodeID As Integer

      Private miStartRingNumber As Integer = 0
      Private miEndRingNumber As Integer = 0
      Private miChainIndex As Integer = -1


      Private miLeftPrev As Integer
      Private miLeftNext As Integer

      Private miRightPrev As Integer
      Private miRightNext As Integer

      '     Private miPreviousNode As Integer
      'Private miNextNode As Integer
      Private mbLeftDirection As Boolean = True
      Private mbRightDirection As Boolean = True



      Public Sub New(ByVal oFullEdge As FullEdge)
         MyBase.New(oFullEdge.ID, TopoElemType.Branch)
         MyBase.AcObjID = oFullEdge.Entity
         Dim oHalfEdge As HalfEdge = Nothing
			'Dim oNode As Node

			Try
            oHalfEdge = oFullEdge.GetHalfEdge(True)
            miLeftPolygon = zzGetPgonID(oHalfEdge)

            miLeftPrev = oHalfEdge.PreviousNode.ID
            miLeftNext = oHalfEdge.NextNode.ID
            ' DMAcadExt.AcadDocument.WriteMessage("Ds!! LEFT Prev: " & CStr(oHalfEdge.PreviousNode.ID) & " Next: " & CStr(oHalfEdge.NextNode.ID))
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New")
         End Try
         Try
            oHalfEdge = oFullEdge.GetHalfEdge(False)
            miRightPolygon = zzGetPgonID(oHalfEdge)

            miRightPrev = oHalfEdge.PreviousNode.ID
            miRightNext = oHalfEdge.NextNode.ID
         Catch oMapEx As Autodesk.Gis.Map.MapException
            DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New_2")
         End Try
         Try
            miPreviousNodeID = oFullEdge.GetNextNode(False).ID
         Catch oMapEx As Autodesk.Gis.Map.MapException
            miPreviousNodeID = 0
            If oMapEx.ErrorCode = 2063 Then
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New_3t")
            Else
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New_3")
            End If


         End Try
         Try
            miNextNodeID = oFullEdge.GetNextNode(True).ID
         Catch oMapEx As Autodesk.Gis.Map.MapException
            miNextNodeID = 0
            If oMapEx.ErrorCode = 2063 Then
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New_4t")
            Else
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(Me.ID) & " tsBranch = New_4")
            End If
         End Try
         If oHalfEdge IsNot Nothing AndAlso tsTopology.MapObjectsDispose Then
            oHalfEdge.Dispose()
         End If

      End Sub
   
      Public Sub New(ByVal iID As Integer, ByVal iPreviousNodeID As Integer, ByVal iNextNodeID As Integer)
         MyBase.New(iID, TopoElemType.Branch)
         miPreviousNodeID = iPreviousNodeID
         miNextNodeID = iNextNodeID
      End Sub
		Public Shared Function GetLink(tAcObjID As ObjectId, bSameDirection As Boolean) As DMAcadExt.IUD_Link
			Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(tAcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Dim oLine As Line
			Dim oArc As Arc
			Dim oPolyline As Polyline
			Dim oTplnLine As DMAcadExt.TplnLine
			Dim oTplnArc As DMAcadExt.TplnArc
			Dim oResLink As DMAcadExt.IUD_Link = Nothing

			If oCurve IsNot Nothing Then
				Select Case oCurve.GetRXClass().Name
					Case DMAcadExt.AcadConst.AcadLineName
						oLine = DirectCast(oCurve, Line)
						oTplnLine = New DMAcadExt.TplnLine(oLine, bSameDirection)
						oResLink = oTplnLine


					Case DMAcadExt.AcadConst.AcadArcName
						oArc = DirectCast(oCurve, Arc)
						oTplnArc = New DMAcadExt.TplnArc(oArc, bSameDirection)


						oResLink = oTplnArc
						'  Case DMAcadExt.AcadConst.AcadPolylineName
					Case DMAcadExt.AcadConst.AcadPolylineName
						oPolyline = DirectCast(oCurve, Polyline)

						If oPolyline.NumberOfVertices = 2 AndAlso oPolyline.GetSegmentType(0) = SegmentType.Arc Then
							oTplnArc = New DMAcadExt.TplnArc(oPolyline, bSameDirection)  '17/06 bSameDirection

							oResLink = oTplnArc
						ElseIf oPolyline.NumberOfVertices = 2 AndAlso oPolyline.GetSegmentType(0) = SegmentType.Line Then


							oTplnLine = New DMAcadExt.TplnLine(oPolyline, bSameDirection)

							oResLink = oTplnLine
							'	DMAcadExt.AcadDocument.WriteMessage("++!!!!Polyline Type, Number Of Vertices: " & oPolyline.NumberOfVertices().ToString() & "!")
						End If

					Case Else
						DMAcadExt.AcadDocument.WriteMessage("++!!!!Curve Type:" & oCurve.GetType().ToString() & "!")

						'   MessageBox.Show(oCurve.GetType().ToString(), "12_079f")


				End Select
			End If
			Return oResLink
		End Function
		Public Function GetInnerPoints() As DMAcadExt.TplnPointArray
			Dim oCurve As Curve = DMAcadExt.AcadTransaction.GetCurve(Me.AcObjID, False, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

			Dim oPolyline As Polyline
			Dim oTplnPoint As DMAcadExt.TPlnPoint

			Dim oResLink As DMAcadExt.IUD_Link = Nothing

			If oCurve IsNot Nothing Then
				Select Case oCurve.GetRXClass().Name

					Case DMAcadExt.AcadConst.AcadPolylineName
						oPolyline = DirectCast(oCurve, Polyline)

						If oPolyline.NumberOfVertices > 2 Then
							Dim oResPointArray As DMAcadExt.TplnPointArray = New DMAcadExt.TplnPointArray(oPolyline.NumberOfVertices - 3)

							For iIndex As Integer = 0 To oPolyline.NumberOfVertices - 3
								oTplnPoint = New DMAcadExt.TPlnPoint(oPolyline.GetPoint2dAt(iIndex + 1))
								oResPointArray.Item(iIndex) = oTplnPoint
							Next

							Return oResPointArray

						Else

							Return Nothing
						End If

					Case Else

						Return Nothing
				End Select
			Else
				Return Nothing
			End If

		End Function

		Public Sub MarkHugeArcRadius(dRadiusMax As Double, tMarkColor As Autodesk.AutoCAD.Colors.Color)
         Dim oArc As Arc
         Dim oEntity As Entity = DMAcadExt.AcadTransaction.GetEntity(Me.AcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
         If oEntity.GetRXClass.Name = DMAcadExt.AcadConst.AcadArcName Then
            oArc = DirectCast(oEntity, Arc)

            If oArc.Radius > dRadiusMax Then

               oArc.Color = tMarkColor
               ' DMAcadExt.AcadDocument.WriteMessage("R= " & CStr(oArc.Radius) & "; " & oArc.Layer & "; " & oArc.Color.ToString())
            End If

         End If
      End Sub

      Public Function GetDirection(iPolygonID As Integer, bIsExterior As Boolean) As enRingDirection
         If iPolygonID = 0 Then
            Return enRingDirection.NotDefined
         ElseIf miLeftPolygon = iPolygonID Then
            If bIsExterior Then
               Return enRingDirection.CounterClockwise
            Else
               Return enRingDirection.Clockwise
            End If

         ElseIf miRightPolygon = iPolygonID Then
            If bIsExterior Then
               Return enRingDirection.Clockwise
            Else
               Return enRingDirection.CounterClockwise
            End If


         Else
            Return enRingDirection.NotDefined
         End If
      End Function
      Public Function IsRingDirection(iPolygonID As Integer, bIsExterior As Boolean) As Boolean

         If iPolygonID = 0 Then
            Return True
         Else
            Dim bRingDir As Boolean

            If miLeftPolygon = iPolygonID Then
               bRingDir = False
            ElseIf miRightPolygon = iPolygonID Then
               bRingDir = True
            End If
            If Not bIsExterior Then
               bRingDir = Not bRingDir
            End If
            Return bRingDir
         End If
      End Function
      Public Function IsNext(oBranch As tsBranch, iPolygonID As Integer, bIsExterior As Boolean) As Boolean
         Return Me.GetNextNode(iPolygonID, bIsExterior) = oBranch.GetPreviousNode(iPolygonID, bIsExterior)
      End Function
      Public Function IsPrevious(oBranch As tsBranch, iPolygonID As Integer, bIsExterior As Boolean) As Boolean
         Return Me.GetPreviousNode(iPolygonID, bIsExterior) = oBranch.GetNextNode(iPolygonID, bIsExterior)
      End Function
      Public Function IsNext(oBranch As tsBranch) As Boolean
         Return Me.NextNodeID = oBranch.PreviousNodeID
      End Function
      Public Function IsPrevious(oBranch As tsBranch) As Boolean
         Return Me.PreviousNodeID = oBranch.NextNodeID
      End Function

      Public ReadOnly Property PreviousNodeID() As Integer
         Get
            Return miPreviousNodeID
         End Get
      End Property
      Public ReadOnly Property NextNodeID() As Integer
         Get
            Return miNextNodeID
         End Get
      End Property
      Public ReadOnly Property StartRingNumber() As Integer
         Get
            Return miStartRingNumber
         End Get
         'Set(iValue As Integer)
         '    miStartRingNumber = iValue
         'End Set
      End Property
      Public Property ChainIndex() As Integer
         Get
            Return miChainIndex
         End Get
         Set(iValue As Integer)
            miChainIndex = iValue
         End Set
      End Property
      Public ReadOnly Property EndRingNumber() As Integer
         Get
            Return miEndRingNumber
         End Get
         'Set(iValue As Integer)
         '    miEndRingNumber = iValue
         'End Set
      End Property
      Public ReadOnly Property LeftPolygon() As Integer
         Get
            Return miLeftPolygon
         End Get
      End Property
      Public ReadOnly Property RightPolygon() As Integer
         Get
            Return miRightPolygon
         End Get
      End Property
      Public ReadOnly Property IsIsthmus() As Boolean
         Get
            Return miLeftPolygon <> 0 AndAlso (miLeftPolygon = miRightPolygon)
         End Get
      End Property
      Public ReadOnly Property IsFirstPolygon(iPgonID As Integer) As Boolean
         Get
            Dim iPgonIDMin As Integer = Math.Min(miLeftPolygon, miRightPolygon)
            If iPgonIDMin = 0 Then
               If iPgonID = Math.Max(miLeftPolygon, miRightPolygon) Then
                  Return True
               Else
                  'Design Error
                  Return False
               End If
            Else
               Return iPgonID = iPgonIDMin
            End If

         End Get
      End Property

      Public Property LeftDirection As Boolean
         Get
            Return mbLeftDirection
         End Get
         Set(bValue As Boolean)
            mbLeftDirection = bValue
         End Set
      End Property
      Public Property RightDirection As Boolean
         Get
            Return mbRightDirection
         End Get
         Set(bValue As Boolean)
            mbRightDirection = bValue
         End Set
      End Property
      Public Sub SetRingNumber(iRingNumber As Integer)
         If iRingNumber <> 0 Then
            miStartRingNumber = iRingNumber
            miEndRingNumber = iRingNumber
         End If
      End Sub
      Public Sub SetRingNumber(iStartRingNumber As Integer, iEndRingNumber As Integer)

         miStartRingNumber = iStartRingNumber
         miEndRingNumber = iEndRingNumber

      End Sub
      Public Function GetOtherPolygon(iPolygonID As Integer) As Integer
         If iPolygonID = miLeftPolygon AndAlso iPolygonID <> miRightPolygon Then
            Return miRightPolygon
         ElseIf iPolygonID <> miLeftPolygon AndAlso iPolygonID = miRightPolygon Then
            Return miLeftPolygon
         Else
            Return -1
         End If
      End Function
      Public Function GetOtherNode(iNodeID As Integer) As Integer
         If iNodeID = miPreviousNodeID AndAlso iNodeID <> miNextNodeID Then
            Return miNextNodeID
         ElseIf iNodeID = miNextNodeID AndAlso iNodeID <> miPreviousNodeID Then
            Return miPreviousNodeID
         Else
            Return -1
         End If
      End Function

      Public Sub PrintInfo()
         DMAcadExt.AcadDocument.WriteMessage("--------Branch#: " & CStr(MyBase.ID) & " Handle " & MyBase.EntityHandle.ToString())
         DMAcadExt.AcadDocument.WriteMessage("Left Nodes: " & CStr(miLeftPrev) & " => " & CStr(miLeftNext) & ";  Right Nodes: " & CStr(miRightPrev) & " => " & CStr(miRightNext))
         DMAcadExt.AcadDocument.WriteMessage("Nodes: " & CStr(miPreviousNodeID) & " ==> " & CStr(miNextNodeID))
         DMAcadExt.AcadDocument.WriteMessage("Left Pgon: " & CStr(miLeftPolygon) & ", Right Pgon: " & CStr(miRightPolygon))

      End Sub
      Public Overrides Function ToString() As String
         Return CStr(miPreviousNodeID) & " ==> " & CStr(miNextNodeID)
      End Function

      Public Function GetInfo(iPgonID As Integer, bIsExterior As Boolean) As String
         Return "Branch#" & CStr(ID) & " Nodes " & GetNodeStr(iPgonID, bIsExterior)
      End Function
      Public Function GetInfo() As String
         Return "Branch#" & CStr(ID) & " Nodes " & CStr(miPreviousNodeID) & "==>" & CStr(miNextNodeID)
      End Function
      Public Function GetNodeStr(iPgonID As Integer, bIsExterior As Boolean) As String
         Select Case Me.GetDirection(iPgonID, bIsExterior)
            Case enRingDirection.Clockwise
               Return CStr(miLeftPrev) & "=>" & CStr(miLeftNext)
            Case enRingDirection.CounterClockwise
               Return CStr(miRightPrev) & "=>" & CStr(miRightNext)
            Case Else
               Return "DirErr"
         End Select

      End Function
      Public Function GetPreviousNode(iPgonID As Integer, bIsExterior As Boolean) As Integer
         Select Case Me.GetDirection(iPgonID, bIsExterior)
            Case enRingDirection.Clockwise
               Return miLeftPrev
            Case enRingDirection.CounterClockwise
               Return miRightPrev
            Case Else
               Return 0
         End Select

      End Function
      Public Function GetNextNode(iPgonID As Integer, bIsExterior As Boolean) As Integer
         Select Case Me.GetDirection(iPgonID, bIsExterior)
            Case enRingDirection.Clockwise
               Return miRightPrev
            Case enRingDirection.CounterClockwise
               Return miLeftPrev
            Case Else
               Return 0
         End Select

      End Function
      Private Function zzGetPgonID(ByRef oHalfEdge As HalfEdge) As Integer
         Dim oPgon As Polygon = Nothing
         Dim sTestMsg As String = ""

         Try
            oPgon = oHalfEdge.Polygon
         Catch oMapEx As Autodesk.Gis.Map.MapException
            If oMapEx.ErrorCode = 2010 Then
               If oPgon IsNot Nothing AndAlso tsTopology.MapObjectsDispose Then
                  oPgon.Dispose()
                  oPgon = Nothing
               End If
               Return 0
            Else
               DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(oHalfEdge.FullEdge.Entity.ToString()) & " TplnTopoPgon - zzBelongPgon:" & sTestMsg)
            End If
         End Try
         If oPgon Is Nothing Then
            Return 0
         Else
            Dim iID As Integer = oPgon.ID
            If tsTopology.MapObjectsDispose Then
               oPgon.Dispose()
            End If
            oPgon = Nothing
            Return iID
         End If
      End Function
   End Class

   Public Class tsBranchChain
      Private miIndex As Integer
      Private mhsBranches As HashSet(Of tsBranch)
      Private mcolBranches As System.Collections.ObjectModel.Collection(Of tsBranch)
      Private mcolDirs As System.Collections.ObjectModel.Collection(Of Boolean)
      '  Private mhslinks As HashSet(Of Isthmus)
      Private miPreviousNodeID As Integer
      Private miNextNodeID As Integer
      Private miExteriorNodeID As Integer
      Private miInnerNodeID As Integer
      Private miInnerRingIndex As Integer
      '   tIsthmus = New Isthmus(oBranch.PreviousNodeID, iInnerRingIndex, oBranch.NextNodeID, oBranch.ID, oBranch.AcObjID, True)
      Private mbReverse As Boolean
      Private mtExteriorRingPoint As Autodesk.AutoCAD.Geometry.Point3d
      Private mtInnerRingPoint As Autodesk.AutoCAD.Geometry.Point3d

      Public Sub New(oBranch As tsBranch)
         mhsBranches = New HashSet(Of tsBranch)()
         mcolBranches = New ObjectModel.Collection(Of tsBranch)()
         mcolDirs = New ObjectModel.Collection(Of Boolean)()
         mhsBranches.Add(oBranch)
         mcolBranches.Add(oBranch)
         miPreviousNodeID = oBranch.PreviousNodeID
         miNextNodeID = oBranch.NextNodeID

      End Sub
      Public Sub New(iIndex As Integer, iExteriorNodeID As Integer, oBranch As tsBranch, bReverse As Boolean)
         miIndex = iIndex
         mhsBranches = New HashSet(Of tsBranch)()
         mcolBranches = New ObjectModel.Collection(Of tsBranch)()
         mcolDirs = New ObjectModel.Collection(Of Boolean)()
         mhsBranches.Add(oBranch)
         mcolBranches.Add(oBranch)
         mcolDirs.Add(bReverse)
         miExteriorNodeID = iExteriorNodeID

      End Sub
      Public Sub AddBranch(oBranch As tsBranch)
         mhsBranches.Add(oBranch)
      End Sub
      Public Sub AddBranch(oBranch As tsBranch, bDir As Boolean)
         mcolBranches.Add(oBranch)
         mcolDirs.Add(bDir)
      End Sub
      Public Property ExteriorRingPoint As Autodesk.AutoCAD.Geometry.Point3d
         Get
            Return mtExteriorRingPoint
         End Get
         Set(tValue As Autodesk.AutoCAD.Geometry.Point3d)
            mtExteriorRingPoint = tValue
         End Set
      End Property
      Public Property InnerRingPoint As Autodesk.AutoCAD.Geometry.Point3d
         Get
            Return mtInnerRingPoint
         End Get
         Set(tValue As Autodesk.AutoCAD.Geometry.Point3d)
            mtInnerRingPoint = tValue
         End Set
      End Property
      Public ReadOnly Property PreviousNodeID() As Integer
         Get
            Return miPreviousNodeID
         End Get
      End Property
      Public ReadOnly Property NextNodeID() As Integer
         Get
            Return miNextNodeID
         End Get
      End Property
      Public ReadOnly Property ExteriorNodeID() As Integer
         Get
            Return miExteriorNodeID
         End Get
      End Property
      Public ReadOnly Property Info() As String
         Get
            Dim sRes As String = String.Empty
            Dim sLink As String
            For iIndex As Integer = 0 To mcolBranches.Count - 1
               If mcolDirs(iIndex) Then
                  sLink = mcolBranches(iIndex).PreviousNodeID.ToString() & "<=>" & mcolBranches(iIndex).NextNodeID.ToString()
               Else
                  sLink = mcolBranches(iIndex).NextNodeID.ToString() & "<=>" & mcolBranches(iIndex).PreviousNodeID.ToString()
               End If
               If iIndex <> 0 Then
                  sRes &= ", "
               End If
               sRes &= sLink
            Next
            Return sRes
         End Get
      End Property

      Public Property InnerNodeID() As Integer
         Get
            Return miInnerNodeID
         End Get
         Set(iValue As Integer)
            miInnerNodeID = iValue
         End Set
      End Property
      Public Property InnerRingIndex() As Integer
         Get
            Return miInnerRingIndex
         End Get
         Set(iValue As Integer)
            miInnerRingIndex = iValue
         End Set
      End Property

      Public ReadOnly Property Index() As Integer
         Get
            Return miIndex
         End Get
      End Property
      Public ReadOnly Property Branches As HashSet(Of tsBranch)
         Get
            Return mhsBranches
         End Get
      End Property
      Public ReadOnly Property Item(iIndex As Integer) As tsBranch
         Get
            Return mcolBranches.Item(iIndex)
         End Get
      End Property
      Public ReadOnly Property Dir(iIndex As Integer) As Boolean
         Get
            Return mcolDirs.Item(iIndex)
         End Get
      End Property
      Public ReadOnly Property Count As Integer
         Get
            Return mcolBranches.Count
         End Get
      End Property
      Public Function GetLink(iIndex As Integer) As DMAcadExt.IUD_Link
         Return tsBranch.GetLink(mcolBranches.Item(iIndex).AcObjID, mcolDirs(iIndex))
      End Function

		Public Function GetSegment() As DMAcadExt.IUD_Link
         Return New DMAcadExt.TplnLine(mtExteriorRingPoint, mtInnerRingPoint)
      End Function
      Public Function GetReverseLink(iIndex As Integer) As DMAcadExt.IUD_Link
         Return tsBranch.GetLink(mcolBranches.Item(iIndex).AcObjID, Not mcolDirs(iIndex))
      End Function
      Public Function GetReverseSegment() As DMAcadExt.IUD_Link
         Return New DMAcadExt.TplnLine(mtInnerRingPoint, mtExteriorRingPoint)
      End Function
      Public Function CopyTo() As tsBranch()
         Dim oaBranches(Me.Count - 1) As tsBranch
         mhsBranches.CopyTo(oaBranches)
         Return oaBranches
      End Function

      Public Property Reverse As Boolean
         Get
            Return mbReverse
         End Get
         Set(bValue As Boolean)
            mbReverse = bValue
         End Set
      End Property
      Public Sub UnionWith(oChain As tsBranchChain, iMutualKey As Integer)
         Dim oaBranches() As tsBranch
         If miNextNodeID = iMutualKey AndAlso oChain.NextNodeID = iMutualKey Then ' miNextNodeID=oChain.NextNodeID
            miNextNodeID = oChain.PreviousNodeID
            oaBranches = oChain.CopyTo()
            For iIndex As Integer = 0 To oaBranches.GetUpperBound(0)
               mhsBranches.Add(oaBranches(oaBranches.GetUpperBound(0) - iIndex))
            Next
         ElseIf miNextNodeID = iMutualKey AndAlso oChain.PreviousNodeID = iMutualKey Then ' miNextNodeID=oChain.PreviousNodeID
            miNextNodeID = oChain.NextNodeID
            mhsBranches.UnionWith(oChain.Branches)
         ElseIf miPreviousNodeID = iMutualKey AndAlso oChain.NextNodeID = iMutualKey Then ' miPreviousNodeID=oChain.NextNodeID
            miPreviousNodeID = oChain.PreviousNodeID
            oChain.Branches.UnionWith(mhsBranches)
            mhsBranches = oChain.Branches

         ElseIf miPreviousNodeID = iMutualKey AndAlso oChain.PreviousNodeID = iMutualKey Then ' miPreviousNodeID=oChain.PreviousNodeID
            miPreviousNodeID = oChain.NextNodeID
            oaBranches = CopyTo()
            mhsBranches = New HashSet(Of tsBranch)()
            For iIndex As Integer = 0 To oaBranches.GetUpperBound(0)
               mhsBranches.Add(oaBranches(oaBranches.GetUpperBound(0) - iIndex))
            Next
            mhsBranches.UnionWith(oChain.Branches)
         End If
      End Sub
   End Class
End Namespace

