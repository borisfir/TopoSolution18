Option Explicit On
Option Strict On
Public Structure Debug
	Public Shared ExcelLog As ExcelAppExt

	Private Shared mbInitialized As Boolean = False
	Private Shared mbDebug As Boolean = False
	Private Shared ReadOnly mbDebugBoris As Boolean = True
	Private Shared mhsLoopMsg As HashSet(Of String) = New HashSet(Of String)()
	Public Shared ReadOnly Property Debug As Boolean
		Get
			zzInitialize()
			Return mbDebug
		End Get
	End Property
	Public Shared ReadOnly Property ExcelLogIsOpened As Boolean
		Get
			Return ExcelLog IsNot Nothing AndAlso ExcelLog.Worksheet IsNot Nothing
		End Get
	End Property

	Public Shared Sub MsgBox(ByVal sCaption As String, ByVal ParamArray oaParams() As System.Object)

		If Debug Then

			System.Windows.Forms.MessageBox.Show(GetMsg(True, oaParams), sCaption)
		End If
	End Sub
	Public Shared Sub MsgBox(ByVal sCaption As String, ByVal iaParams() As Integer)

		If Debug Then
			Dim oaParams(iaParams.GetUpperBound(0)) As System.Object
			For iIndex As Integer = 0 To iaParams.GetUpperBound(0)
				oaParams(iIndex) = iaParams(iIndex)
			Next
			System.Windows.Forms.MessageBox.Show(GetMsg(True, oaParams), sCaption)
		End If
	End Sub
	Public Shared Sub MsgBox(ByVal sCaption As String, ByVal daParams() As Double)

		If Debug Then
			Dim oaParams(daParams.GetUpperBound(0)) As System.Object
			For iIndex As Integer = 0 To daParams.GetUpperBound(0)
				oaParams(iIndex) = daParams(iIndex)
			Next
			System.Windows.Forms.MessageBox.Show(GetMsg(True, oaParams), sCaption)
		End If
	End Sub
	Public Shared Sub MsgBox(ByVal sCaption As String, ByVal saParams() As String)

		If Debug Then
			If saParams Is Nothing Then
				System.Windows.Forms.MessageBox.Show("Params Is Nothing", sCaption)
			Else
				Dim oaParams(saParams.GetUpperBound(0)) As System.Object
				For iIndex As Integer = 0 To saParams.GetUpperBound(0)
					oaParams(iIndex) = saParams(iIndex)
				Next
				System.Windows.Forms.MessageBox.Show(GetMsg(True, oaParams), sCaption)
			End If

		End If
	End Sub
	Public Shared Sub ErrMsgBox(ByVal sCaption As String, oEx As Exception, ByVal ParamArray oaParams() As System.Object)

		If Debug AndAlso oEx IsNot Nothing Then
			Dim sPart1 As String = oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oEx.GetType().ToString()
			Dim sPart2 As String = GetMsg(True, oaParams)
			If Not String.IsNullOrEmpty(sPart2) Then

				sPart1 &= vbCrLf & sPart2
			End If
			System.Windows.Forms.MessageBox.Show(sPart1, sCaption)


		End If
	End Sub
	Public Shared Sub MsgBoxLoop(ByVal sCaption As String, iLoopCount As Integer, ByVal ParamArray oParams() As System.Object)
		'L001-L008
		If Debug Then
			Dim sKey As String
			For iIndex As Integer = 0 To iLoopCount - 1
				sKey = sCaption & "_" & iIndex.ToString()
				If Not mhsLoopMsg.Contains(sKey) Then
					System.Windows.Forms.MessageBox.Show(GetMsg(True, oParams), sCaption)
					mhsLoopMsg.Add(sKey)
					Exit For
				End If
			Next

		End If
	End Sub
	Public Shared Sub MsgBoxLoop(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)
		'L001-L008
		If Debug Then
			If Not mhsLoopMsg.Contains(sCaption) Then
				System.Windows.Forms.MessageBox.Show(GetMsg(True, oParams), sCaption)
				mhsLoopMsg.Add(sCaption)
			End If
		End If
	End Sub

	Public Shared Sub UserMsg(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)
		System.Windows.Forms.MessageBox.Show(GetMsg(True, oParams), sCaption)

	End Sub

	Public Shared Function GetMsg(bCrLf As Boolean, ByVal ParamArray oParams() As System.Object) As String
		Dim sMsg As String = String.Empty
		If oParams IsNot Nothing Then
			For iIndex As Integer = 0 To oParams.GetUpperBound(0)
				If Not String.IsNullOrEmpty(sMsg) Then
					If bCrLf Then
						sMsg &= vbCrLf
					Else
						sMsg &= "; "
					End If
				End If
				If oParams(iIndex) Is Nothing Then
					sMsg &= "#" & iIndex.ToString() & " Nothing"
				ElseIf oParams(iIndex).ToString() = String.Empty Then
					sMsg &= "#" & iIndex.ToString() & " Empty"
				Else
					sMsg &= oParams(iIndex).ToString()
				End If

			Next
		End If
		Return sMsg
	End Function

	Public Shared Function ColCount(col As IDictionary) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Count
		End If
	End Function
	Public Shared Function ColCount1(col As Dictionary(Of Integer, Double)) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Count
		End If
	End Function

	Public Shared Function ColCount(col As ICollection) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Count
		End If
	End Function

	Public Shared Function ColCount(col As ICollection(Of Integer)) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Count
		End If
	End Function
	Public Shared Function ColCount(Of col As IEnumerable)() As Integer
		Dim iCount As Integer
		Dim colVal As col
		For Each a As System.Object In colVal
			iCount += 1
		Next

		Return iCount

	End Function
	Public Shared Function ColCount(col As ICollection(Of Object)) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Count
		End If
	End Function
	Public Shared Function ColCount(col As Data.DataTable) As Integer
		If col Is Nothing Then
			Return -1
		Else
			Return col.Rows.Count
		End If
	End Function

	Public Shared Function ColCount(array As String()) As Integer
		If array Is Nothing Then
			Return -1
		Else
			Return array.GetUpperBound(0) + 1
		End If
	End Function


	Public Shared Function GetListArray(col As ICollection(Of Long)) As System.Object()
		Dim oaRes(col.Count - 1) As System.Object
		Dim iIndex As Integer = 0
		For Each iItem As Integer In col

			oaRes(iIndex) = iItem
			iIndex += 1
		Next
		Return oaRes
	End Function
	Public Shared Function GetListArray(col As ICollection(Of Integer)) As System.Object()
		Dim oaRes(col.Count - 1) As System.Object
		Dim iIndex As Integer = 0
		For Each iItem As Integer In col

			oaRes(iIndex) = iItem
			iIndex += 1
		Next
		Return oaRes
	End Function

	Public Shared Function GetListArrayA(oArray As Array) As System.Object()
		Dim oaRes(oArray.GetUpperBound(0)) As System.Object
		Dim iIndex As Integer = 0
		For Each oItem As System.Object In oArray

			oaRes(iIndex) = oItem
			iIndex += 1
		Next
		Return oaRes
	End Function
	Public Shared Function GetListArray(saValues() As String) As System.Object()
		Dim oaRes(saValues.GetUpperBound(0)) As System.Object
		Dim iIndex As Integer = 0
		For Each sItem As String In saValues

			oaRes(iIndex) = sItem
			iIndex += 1
		Next
		Return oaRes
	End Function
	Public Shared Function GetListArray(iUB As Integer, saValues As IEnumerable(Of String)) As System.Object()
		Dim oaRes(iUB) As System.Object
		Dim iIndex As Integer = 0
		For Each sItem As String
			In saValues

			oaRes(iIndex) = sItem
			iIndex += 1
		Next
		Return oaRes
	End Function

	Public Shared Function GetList(col As ICollection(Of Integer)) As String
		Dim sRes As String = Nothing
		For Each iItem As Integer In col
			If sRes Is Nothing Then
				sRes = iItem.ToString()
			Else
				sRes &= "," & iItem.ToString()
			End If
		Next
		Return sRes
	End Function


	Private Shared Sub zzInitialize()
#Enable Warning IDE1006 ' Naming Styles
		If Not mbInitialized Then
			If System.Environment.UserName.ToUpper = "BORIS" Or System.Environment.UserName.ToUpper = "OLEGB1111" Then


				mbDebug = mbDebugBoris
			End If
			mbInitialized = True
		End If

	End Sub
End Structure
