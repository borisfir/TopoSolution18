

Module Module1

	Public Function Balance(ByVal daInput() As Double) As Long()
		Dim iDataUB As Integer
		Dim iIndex As Integer
		Dim dSum As Double
		Dim lSum As Long

		Dim dLimit As Double
		Dim dStep As Double
		Dim iCurrentSum As Long

		Dim iDirection As Integer = 0

		dLimit = 0.5
		dStep = dLimit * 0.5
		iDataUB = daInput.GetUpperBound(0)

		Dim laOutput(iDataUB) As Long
		Dim laOutputPrev(iDataUB) As Long
		For iIndex = 0 To iDataUB
			dSum = dSum + daInput(iIndex)
		Next
		Try
			lSum = Convert.ToInt64(dSum)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & dSum.ToString, "Balance_1")
			For i As Integer = 0 To daInput.GetUpperBound(0)
				DMAcadExt.AcadDocument.WriteMessage("_" & CStr(i) & ": " & daInput(i).ToString())
			Next

		End Try


		Do While dStep > 0.00000000000001
			iCurrentSum = 0L
			For iIndex = 0 To iDataUB
				laOutput(iIndex) = zzIntRound(daInput(iIndex), dLimit)
				iCurrentSum = iCurrentSum + laOutput(iIndex)
			Next
			'   AcadReport.AcadUtil.GetEditor().WriteMessage("iCurrentSum,iSum: " & CStr(iCurrentSum) & "," & CStr(iSum))
			Select Case iCurrentSum - lSum
				Case Is = 0
					Exit Do
				Case Is > 0
					dLimit = dLimit + dStep
					If iDirection = 1 Then
						laOutputPrev = laOutput
					Else
						iDirection = 1
					End If
				Case Is < 0
					dLimit = dLimit - dStep
					If iDirection = -1 Then
						laOutputPrev = laOutput
					Else
						iDirection = -1
					End If
			End Select
			If dLimit < 0.01 Then
				Common.GetEditor().WriteMessage("Balance: " & "Data out of range" & vbCrLf)

				Return Nothing
			End If
			dStep = dStep / 2.0
		Loop
		iIndex = 0

		If iCurrentSum <> lSum Then
			Common.GetEditor().WriteMessage("M/Balance Additional : " & CStr(iCurrentSum) & "," & CStr(lSum) & "," & CStr(dLimit) & "," & CStr(dStep) & vbCrLf)
			zzDispArray(daInput)
			zzDispIntArray(laOutput)
		End If
		Do While (iCurrentSum <> lSum) And (iIndex <= iDataUB)
			If laOutput(iIndex) <> laOutputPrev(iIndex) Then
				iCurrentSum = iCurrentSum - laOutput(iIndex) + laOutputPrev(iIndex)
				laOutput(iIndex) = laOutputPrev(iIndex)
			End If
			iIndex += 1
		Loop
		Return laOutput
	End Function
	 

	Private Function zzIntRound(ByVal dValue As Double, ByVal dLimit As Double) As Long
		Dim dIntVal As Double
		Dim lIntVal As Long

		dIntVal = Math.Floor(dValue)
		lIntVal = CLng(dIntVal)
		If (dValue - dIntVal >= dLimit) Then
			'       If (dValue - dIntVal >= dLimit) Or dIntVal = 0.0 Then

			Return lIntVal + 1L
		ElseIf lIntVal = 0 Then
			Return 1L
		Else
			Return lIntVal
		End If
	End Function
	Private Sub zzDispArray(ByVal iaVal() As Double)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & Convert.ToString(iaVal(iIndex))
		Next

		Common.GetEditor().WriteMessage("Array:" & sOut & vbCrLf)
	End Sub
	Private Sub zzDispIntArray(ByVal iaVal() As Integer)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & iaVal(iIndex).ToString()
		Next

		Common.GetEditor().WriteMessage("Int Array:" & sOut & vbCrLf)
	End Sub
	Private Sub zzDispIntArray(ByVal iaVal() As Long)
		Dim sOut As String = String.Empty
		For iIndex As Integer = 0 To iaVal.GetUpperBound(0)
			sOut += ":" & iaVal(iIndex).ToString()
		Next

		Common.GetEditor().WriteMessage("Array:" & sOut & vbCrLf)
		Common.GetEditor().WriteMessage("Int Array:" & sOut & vbCrLf)
	End Sub
End Module
