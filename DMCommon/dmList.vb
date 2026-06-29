Option Explicit On
Option Strict On

Public Structure dmList
   Const msRegularAll As String = "*"
   Private msList As String
	Private msListPlusDelim As String
	Private msDelim As String
	Private msExtList As String
	Private mhsValues As HashSet(Of String)
	Private mbExists As Boolean
	Private mbHashset As Boolean

	Private mbIsList As Boolean
	Private mbIsRegular As Boolean
	Private miUB As Integer
	Private msaValues() As String
	Private msaExtValues() As String
	'Private msDebugTag As String

	Private msStart As String
	Private msEnd As String
	Private msaStart() As String
	Private msaEnd() As String
	Public Sub New(sList As String, Optional sDelim As String = ",")
		msList = sList
		If String.IsNullOrEmpty(sList) Then
			mbExists = False
		Else
			mbExists = True
			If sList.Contains(msRegularAll) Then
				Dim saVal() As String = Split(sList, msRegularAll)
				If saVal.GetUpperBound(0) = 1 Then
					msStart = saVal(0)
					msEnd = saVal(1)
					mbIsRegular = True
				Else
					System.Windows.Forms.MessageBox.Show("System Err #4125", "Tplanner")
				End If
			Else
				msDelim = sDelim
				mbIsList = msList.Contains(msDelim)
				If mbIsList Then
					msListPlusDelim = zzAddDelim(msList)
					msaValues = Split(msList, msDelim)
				End If
			End If
		End If

	End Sub
	Public Sub ItemsSplit(sItemDelim As String)
		Dim saValue() As String
		If mbIsList Then
			ReDim msaExtValues(UpperBound)
			For iIndex As Integer = 0 To UpperBound
				If Not String.IsNullOrEmpty(msaValues(iIndex)) Then
					saValue = Split(msaValues(iIndex), sItemDelim)
					If saValue.GetUpperBound(0) > 0 Then
						msaValues(iIndex) = saValue(0)
						msaExtValues(iIndex) = saValue(1)
					End If
				End If
			Next
		ElseIf mbExists Then
			saValue = Split(msList, sItemDelim)
			If saValue.GetUpperBound(0) > 0 Then
				msList = saValue(0)
				msExtList = saValue(1)

			End If
		End If
	End Sub
	Public Sub New(saValues() As String)
		msDelim = ","
		msList = Join(saValues, msDelim)

		miUB = saValues.GetUpperBound(0)
		mbExists = (miUB >= 0)
		msaValues = saValues


		mbIsRegular = msList.Contains(msRegularAll)
		If mbIsRegular Then
			zzInitRegular()

		Else
			mbIsList = (miUB > 0)
			If miUB > 0 Then
				msListPlusDelim = zzAddDelim(msList)

			End If

		End If

	End Sub
	Public Sub New(hsValues As HashSet(Of String))
		mbHashset = True
		mhsValues = hsValues
		mbExists = mhsValues IsNot Nothing AndAlso mhsValues.Count > 0

	End Sub
	Public Sub Add(saValues() As String)

	End Sub
	Public Sub Add(sValue As String)


	End Sub
	Private Sub zzInitRegular()
		Dim saTempVal() As String
		ReDim msaStart(miUB)
		ReDim msaEnd(miUB)

		For iIndex As Integer = 0 To miUB
			saTempVal = Strings.Split(msaValues(iIndex), msRegularAll)
			msaStart(iIndex) = saTempVal(0)
			If saTempVal.GetUpperBound(0) = 0 Then
				msaEnd(iIndex) = Nothing
			Else
				msaEnd(iIndex) = saTempVal(1)
			End If
		Next
	End Sub
	Public ReadOnly Property SingleValue As String
		Get
			If Not mbIsList Then
				Return msList
			Else
				Return String.Empty
			End If
		End Get
	End Property
	Public ReadOnly Property Exists As Boolean
		Get
			Return mbExists
		End Get
	End Property
	Public ReadOnly Property IsList As Boolean
		Get
			Return mbIsList
		End Get
	End Property
	Public Function Contains_300516(sValue As String) As Boolean

		If mbExists Then
			If String.IsNullOrEmpty(sValue) Then
				'  System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_121")
				Return False
			Else
				If mbIsList Then
					'   System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_122")
					Return msListPlusDelim.Contains(zzAddDelim(sValue))
				ElseIf msStart IsNot Nothing Then
					If sValue.StartsWith(msStart) Then
						If msEnd IsNot Nothing Then
							Return sValue.EndsWith(msEnd)
						Else
							Return True
						End If
					Else
						Return False
					End If
				ElseIf msEnd IsNot Nothing Then
					Return sValue.EndsWith(msEnd)
				Else
					' System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_123")
					Return (msList = sValue)
				End If
			End If
		Else
			'System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_124")
			Return True
		End If

	End Function
	Public Function Contains(sValue As String) As Boolean

		If mbExists Then


			If String.IsNullOrEmpty(sValue) Then
				Return False
			Else
				If mbHashset Then
					Return mhsValues.Contains(sValue)

				ElseIf mbIsList Then

					Return msListPlusDelim.Contains(zzAddDelim(sValue))
				Else
					If mbIsRegular Then
						If miUB = 0 Then
							Return zzCheckRegularCond(sValue)
						Else
							Return zzCheckMultiRegularCond(sValue)
						End If
					Else
						Return (msList = sValue)
					End If
				End If
			End If
		Else

			Return True
      End If

   End Function
   Private Function zzCheckRegularCond(sValue As String) As Boolean
      If msStart IsNot Nothing Then
         If sValue.StartsWith(msStart) Then
            If msEnd IsNot Nothing Then
               Return sValue.EndsWith(msEnd)
            Else
               Return True
            End If
         Else
            Return False
         End If
      ElseIf msEnd IsNot Nothing Then
         Return sValue.EndsWith(msEnd)
      Else
         ' System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_126")
         Return (msList = sValue)
      End If

   End Function
   Private Function zzCheckRegularCond(sValue As String, iIndex As Integer) As Boolean
      If Not String.IsNullOrEmpty(msaStart(iIndex)) Then
         If sValue.StartsWith(msaStart(iIndex)) Then
            If String.IsNullOrEmpty(msaEnd(iIndex)) Then
               Return True
            Else
               Return sValue.EndsWith(msaEnd(iIndex))

            End If
         Else
            Return False
         End If
      ElseIf Not String.IsNullOrEmpty(msaEnd(iIndex)) Then
         Return sValue.EndsWith(msaEnd(iIndex))
      Else
         ' System.Windows.Forms.MessageBox.Show(msList & vbCrLf & sValue, "09_125")
         Return (msaValues(iIndex) = sValue)
      End If

   End Function
   Private Function zzCheckMultiRegularCond(sValue As String) As Boolean
      Dim bRes As Boolean = False
      For iIndex As Integer = 0 To miUB
			bRes = zzCheckRegularCond(sValue, iIndex)

			If bRes Then
            Return True
         End If
      Next
      Return False
   End Function

	Public Function GetItemNo(sValue As String) As Integer
      If String.IsNullOrEmpty(sValue) Then
         Return -1
      Else
         If mbIsList Then
            For iNo As Integer = 0 To miUB
               If msaValues(iNo) = sValue Then
                  Return iNo
               End If
            Next
            Return -1
         ElseIf msList = sValue Then
            Return 0
         End If
      End If
   End Function

	Public ReadOnly Property First As String
		Get
			If mbIsList Then
				'	Dim saValue() As String = Strings.Split(msList, msDelim)
				Return msaValues(0)
			Else
				Return msList
			End If
		End Get
	End Property
	Public ReadOnly Property FirstExt As String
		Get
			If mbIsList AndAlso msaExtValues.GetUpperBound(0) >= 0 Then
				Return msaExtValues(0)
			Else
				Return msExtList
			End If
		End Get
	End Property
	Public ReadOnly Property List As String
      Get
         Return msList
      End Get
   End Property
	Public ReadOnly Property Values() As String()
		Get
			If msaValues Is Nothing Then
				Return {msList}
			Else
				Return msaValues
			End If


		End Get
	End Property
	Public ReadOnly Property ExtValues() As String()
		Get
			Return msaExtValues
		End Get
	End Property

	Public ReadOnly Property Item(iIndex As Integer) As String
		Get
			If mbIsList Then
				Return msaValues(iIndex)
			Else
				Return msList
			End If
		End Get
	End Property
	Public ReadOnly Property ItemExt(iIndex As Integer) As String
		Get

			If mbIsList AndAlso msaExtValues.GetUpperBound(0) >= 0 Then
				Return msaExtValues(iIndex)

			ElseIf Not mbIsList Then
				Return msExtList
			Else
				Return Nothing
			End If

		End Get
	End Property

	Public ReadOnly Property UpperBound As Integer
      Get
         If mbIsList Then
            Return msaValues.GetUpperBound(0)
         ElseIf mbExists Then
            Return 0
         Else
            Return -1
         End If
      End Get
   End Property
   Private Function zzAddDelim(sValue As String) As String
      Return msDelim & sValue & msDelim
   End Function
End Structure
