Option Explicit On
Option Strict On
Public Class tmEdge
	Const miNullIndex As Integer = -5
	Private miRingID As Integer
	Private miAdjRingID As Integer
	Private moaNodes() As tmNode
	Private miNodeIndex As Integer
	Private miStartAVertexIndex As Integer
	Private miStartBVertexIndex As Integer
	'	Private mbBdirection As Boolean
	Private mtStartVertexPair As tmVertexPair
	Private mtEndVertexPair As tmVertexPair
	Private mbClosed As Boolean
	Private mbDirection As Boolean

	Public Sub AddNodeAAA(oNode As tmNode)
		miNodeIndex += 1
		moaNodes(miNodeIndex) = oNode
	End Sub
	Public Sub Close()
		ReDim Preserve moaNodes(miNodeIndex)
	End Sub

	Public Sub New(iStartIndex As Integer, iEndIndex As Integer, iRingID As Integer, iExteriorRingID As Integer, bDirection As Boolean)
		mtStartVertexPair = New tmVertexPair(iStartIndex)
		mtEndVertexPair = New tmVertexPair(iEndIndex)
		miRingID = iRingID
		miAdjRingID = iExteriorRingID
		mbDirection = bDirection
	End Sub
	Public Sub New(tStartVertexPair As tmVertexPair, iRingID As Integer, iAdjRingID As Integer, bDirection As Boolean)
		mtStartVertexPair = tStartVertexPair
		miRingID = iRingID
		miAdjRingID = iAdjRingID
		mbDirection = bDirection
	End Sub
	Public Property Closed As Boolean
		Get
			Return mbClosed
		End Get
		Set(bValue As Boolean)
			mbClosed = bValue
		End Set
	End Property
	Public ReadOnly Property Direction As Boolean
		Get
			Return mbDirection
		End Get
	End Property
	Public Function TryAdd(tEndVertexPair As tmVertexPair) As Boolean
		If mtStartVertexPair <> tEndVertexPair Then
			mtEndVertexPair = tEndVertexPair
			Return True
		Else
			Return False
		End If
	End Function
	Public Function TryJoin(oEdge As tmEdge) As Boolean
		'If mtEndVertexPair = oEdge.Start Then
		'mtEndVertexPair = oEdge.End
		'Return True
		'Else
		If mtStartVertexPair = oEdge.End Then
			mtStartVertexPair = oEdge.Start
			Return True
		Else
			Return False
		End If
	End Function
	Public ReadOnly Property RingID As Integer
		Get
			Return miRingID
		End Get
	End Property
	Public ReadOnly Property IsRight As Boolean
		Get
			Return mtStartVertexPair.IsInstance AndAlso mtEndVertexPair.IsInstance
		End Get
	End Property

	Public ReadOnly Property AdjRingID As Integer
		Get
			Return miAdjRingID
		End Get
	End Property
	Public ReadOnly Property Start As tmVertexPair
		Get
			Return mtStartVertexPair
		End Get
	End Property
	Public ReadOnly Property [End] As tmVertexPair
		Get
			Return mtEndVertexPair
		End Get
	End Property
	Public Function StartVertex(iRingID As Integer, bDirection As Boolean) As Integer

		Select Case iRingID
			Case miRingID
				If bDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return miNullIndex
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return miNullIndex
					End If

				End If

			Case miAdjRingID
				If bDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return miNullIndex
					End If

				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return miNullIndex
					End If

				End If

			Case Else
				Return -1
		End Select

	End Function

	Public Function EndVertex(iRingID As Integer, bDirection As Boolean) As Integer

		Return StartVertex(iRingID, Not bDirection)
	End Function
	Public Function StartVertex(iRingID As Integer) As Integer
		Dim bDirection As Boolean = True
		Select Case iRingID
			Case miRingID
				If bDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return miNullIndex
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return miNullIndex
					End If

				End If

			Case miAdjRingID
				If mbDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return miNullIndex
					End If

				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return miNullIndex
					End If

				End If

			Case Else
				Return -1
		End Select

	End Function

	Public Function EndVertex(iRingID As Integer) As Integer
		Dim bDirection As Boolean = True
		Select Case iRingID
			Case miRingID
				If bDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return -1
					End If

				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return -1
					End If

				End If

			Case miAdjRingID
				If mbDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				End If

			Case Else
				Return -1
		End Select

	End Function

	Public Function OtherRingStartVertex050315(iRingID As Integer, bDirection As Boolean) As Integer
		Select Case iRingID
			Case miAdjRingID
				If bDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return -1
					End If

				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return -1
					End If

				End If

			Case miRingID
				If bDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				End If

			Case Else
				Return -1
		End Select
	End Function

	 
	Public Function OtherRingStartVertex(iRingID As Integer) As Integer
		Dim bDirection As Boolean = True
		Select Case iRingID
			Case miRingID
				If bDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return -1
					End If

				End If


			Case miAdjRingID
				If mbDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return -1
					End If

				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return -1
					End If

				End If



			Case Else
				Return -1
		End Select
	End Function


	Public Function OtherRingEndVertex(iRingID As Integer) As Integer
		Dim bDirection As Boolean = True
		Select Case iRingID
			Case miRingID
				If bDirection Then
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.NBVertexIndex
					Else
						Return -1
					End If
				Else
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.NBVertexIndex
					Else
						Return -1
					End If
				End If


			Case miAdjRingID
				If mbDirection Then
					If mtStartVertexPair.IsInstance Then
						Return mtStartVertexPair.VertexIndex
					Else
						Return -1
					End If

				Else
					If mtEndVertexPair.IsInstance Then
						Return mtEndVertexPair.VertexIndex
					Else
						Return -1
					End If



				End If



			Case Else
				Return -1
		End Select
	End Function

	Public Function EndVertexOld(iRingID As Integer) As Integer
		Select Case iRingID
			Case miRingID
				Return mtEndVertexPair.VertexIndex
			Case miAdjRingID
				Return mtEndVertexPair.NBVertexIndex
			Case Else
				Return -1
		End Select
	End Function
	Public Sub DebugWrite(iRingID As Integer, sCaption As String)
		Dim sClosed As String
		If mbClosed Then
			sClosed = " Cl"
		Else
			sClosed = ""
		End If
		Select Case iRingID
			Case miRingID
				DMAcadExt.AcadDocument.WriteMessage(sCaption & " aRings:" & CStr(miRingID) & "," & CStr(miAdjRingID) & "; Vert:" & CStr(mtStartVertexPair.VertexIndex) & " <==> " & CStr(mtEndVertexPair.VertexIndex) & sClosed)
			Case miAdjRingID
				DMAcadExt.AcadDocument.WriteMessage(sCaption & " bRings:" & CStr(miRingID) & "," & CStr(miAdjRingID) & "; Vert:" & CStr(mtStartVertexPair.NBVertexIndex) & " <==> " & CStr(mtEndVertexPair.NBVertexIndex) & sClosed)
		End Select

	End Sub
	Public Sub DebugWrite050315(sCaption As String, iRingID As Integer, bDirection As Boolean)
		Dim sRings As String
		Dim iOtherRing As Integer
		Select Case iRingID
			Case miRingID
				sRings = CStr(miRingID) & "," & CStr(miAdjRingID)
				iOtherRing = miAdjRingID
			Case miAdjRingID
				sRings = CStr(miAdjRingID) & "," & CStr(miRingID)
				iOtherRing = miRingID
			Case Else
				sRings = "Err#29"
		End Select

		'	DMAcadExt.AcadDocument.WriteMessage(sCaption & " R:" & sRings & "; Vert:" & CStr(Me.StartVertex(iRingID, bDirection)) & "<=>" & CStr(Me.EndVertex(iRingID, bDirection)) & "; " & CStr(Me.OtherRingStartVertex(iRingID, bDirection)) & "<=>" & CStr(Me.OtherRingEndVertex(iRingID, bDirection)))
	End Sub
	Public Sub DebugWrite(sCaption As String, iRingID As Integer)
		Dim sRings As String
		Dim iOtherRing As Integer
		Select Case iRingID
			Case miRingID
				sRings = CStr(miRingID) & "," & CStr(miAdjRingID)
				iOtherRing = miAdjRingID
			Case miAdjRingID
				sRings = CStr(miAdjRingID) & "," & CStr(miRingID)
				iOtherRing = miRingID
			Case Else
				sRings = "Err#29"
		End Select

      DMAcadExt.AcadDocument.WriteLog(sCaption & " R:" & sRings & "; Dir:" & CStr(mbDirection) & "; Vert:" & CStr(Me.StartVertex(iRingID)) & "<=>" & CStr(Me.EndVertex(iRingID)) & "; " & CStr(Me.OtherRingStartVertex(iRingID)) & "<=>" & CStr(Me.OtherRingEndVertex(iRingID)))
	End Sub
	Public Function Check(iRingID As Integer) As Boolean
		If Me.StartVertex(iRingID) < 0 OrElse Me.EndVertex(iRingID) < 0 OrElse Me.OtherRingStartVertex(iRingID) < 0 OrElse Me.OtherRingEndVertex(iRingID) < 0 Then
			Return True
		Else
			Return False
		End If
	End Function
	Public Sub DebugWriteOld(sCaption As String, iRingID As Integer, bDirection As Boolean)
		Const sNull As String = " Null "
		Dim sS0, sSN, sE0, sEN As String
		Me.StartVertex(iRingID, bDirection)
		If mtStartVertexPair.IsInstance Then
			sS0 = CStr(mtStartVertexPair.VertexIndex)
			sSN = CStr(mtStartVertexPair.NBVertexIndex)

		Else
			sS0 = sNull
			sSN = sNull

		End If
		If mtEndVertexPair.IsInstance Then
			sE0 = CStr(mtEndVertexPair.VertexIndex)
			sEN = CStr(mtEndVertexPair.NBVertexIndex)

		Else
			sE0 = sNull
			sEN = sNull

		End If


		'DMAcadExt.AcadDocument.WriteMessage(sCaption & " R:" & CStr(miRingID) & "," & CStr(miAdjRingID) & "; Vert:" & CStr(mtStartVertexPair.VertexIndex) & "<=>" & CStr(mtEndVertexPair.VertexIndex) & "; " & CStr(mtStartVertexPair.NBVertexIndex) & "<=>" & CStr(mtEndVertexPair.NBVertexIndex))
		DMAcadExt.AcadDocument.WriteMessage(sCaption & " R:" & CStr(miRingID) & "," & CStr(miAdjRingID) & "; Vert:" & sS0 & "<=>" & sE0 & "; " & sSN & "<=>" & sEN)


	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
