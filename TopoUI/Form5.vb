Public Class Form5
	Private moaNoteChecks(16) As NoteCheck
	Private Class NoteCheck
		Const msPlaceHolderChar As String = "_"
		Const miVarPlacesMax As Integer = 5

		Private moCheckControl As CheckBox
		Private miControlIndex As Integer
		Private msText As String
		Private miPlaceHolderMin As Integer = 3
		Private msPlaceHolder As String = StrDup(miPlaceHolderMin, msPlaceHolderChar)
		Private miCurrentPosition As Integer = 1
		Private miCurrentPlace As Integer
		Dim taVarPlaces(miVarPlacesMax) As VarPlace
		Private miVarPlacesCount As Integer
		Public Sub New(oCheckControl As CheckBox)
			Dim sName As String = oCheckControl.Name
			miControlIndex = Convert.ToInt32(sName.Substring(7)) - 1
			moCheckControl = oCheckControl
			msText = moCheckControl.Text


			For miCurrentPlace = 0 To miVarPlacesMax
				If Not CalcPlace() Then

					miVarPlacesCount = miCurrentPlace
					Return
				End If
			Next
			'	miVarPlacesCount = miVarPlacesMax + 1
		End Sub
		Private Function CalcPlace() As Boolean
			Dim sChar As String
			Dim sTest As String = DMCommon.Hebrew.DispASC(msText, False)
			Dim iPos As Integer = InStr(miCurrentPosition, msText, msPlaceHolder)
			If iPos > 0 Then
				taVarPlaces(miCurrentPlace).StartPosition = iPos

				For iIndex As Integer = iPos + miPlaceHolderMin To msText.Length
					sChar = Mid(msText, iIndex, 1)
					If sChar <> msPlaceHolderChar Then
						taVarPlaces(miCurrentPlace).EndPosition = iIndex - 1
						miCurrentPosition = iIndex
						Return True
					End If
				Next
				taVarPlaces(miCurrentPlace).EndPosition = msText.Length
				miCurrentPosition = msText.Length + 1
				Return True

			Else
				Return False
			End If






		End Function
		Public Sub AddVarData(saValue() As String)
			Dim iShift As Integer = 0
			Dim sRes As String = msText
			For iIndex As Integer = 0 To saValue.GetUpperBound(0)
				Dim iPrevLen As Integer = taVarPlaces(iIndex).PlaceHolderLen
				If saValue(iIndex) IsNot Nothing Then
					DMCommon.Functions.Mid(sRes, taVarPlaces(iIndex).StartPosition + iShift, iPrevLen, saValue(iIndex))
					taVarPlaces(iIndex).CurrentLen = saValue(iIndex).Length
					iShift += taVarPlaces(iIndex).Shift
				Else
					taVarPlaces(iIndex).Clear()
				End If

			Next
			moCheckControl.Text = sRes
		End Sub
		Public Sub Clear()
			For iIndex As Integer = 0 To miVarPlacesCount - 1
				'	taVarPlaces(iIndex).Clear()
			Next

			moCheckControl.Text = msText
		End Sub
		Private Structure VarPlace
			Public StartPosition As Integer
			Public PlaceHolderLen As Integer
			Public CurrentLen As Integer

			Public Property EndPosition As Integer
				Get
					Return StartPosition + PlaceHolderLen - 1
				End Get
				Set(iValue As Integer)
					PlaceHolderLen = iValue - StartPosition + 1
					CurrentLen = PlaceHolderLen
				End Set
			End Property
			Public Function Shift() As Integer
				Return CurrentLen - PlaceHolderLen
			End Function
			Public Sub Clear()
				CurrentLen = PlaceHolderLen
			End Sub
		End Structure
	End Class

	Public Sub New()

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		zzInit()
	End Sub
	Public Sub zzInit()
		moaNoteChecks(0) = New NoteCheck(Me.chkNote01)
		moaNoteChecks(1) = New NoteCheck(Me.chkNote02)
		moaNoteChecks(2) = New NoteCheck(Me.chkNote03)
		moaNoteChecks(3) = New NoteCheck(Me.chkNote04)

      '   Me.txtInputDOS.Text = "49|54|55|47|142|129|47|55"
      Me.txtInputDOS.Text = "142|129|47|55"


	End Sub

	Private Sub cmdAdd_Click(sender As System.Object, e As System.EventArgs) Handles cmdAdd.Click
		Dim saValue0() As String = {"12"}
		moaNoteChecks(0).AddVarData(saValue0)

		Dim saValue1() As String = {"123/45"}
		moaNoteChecks(1).AddVarData(saValue1)


		Dim saValue2() As String = {"12345", "6", "78"}
		moaNoteChecks(2).AddVarData(saValue2)
	End Sub

	Private Sub cmdClear_Click(sender As System.Object, e As System.EventArgs) Handles cmdClear.Click
		moaNoteChecks(0).Clear()
		moaNoteChecks(1).Clear()

		moaNoteChecks(2).Clear()
		moaNoteChecks(3).Clear()


	End Sub

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Me.chkNote02.Text = Me.chkNote02.Text.Substring(0, 16) & vbCrLf & Me.chkNote02.Text.Substring(17)
	End Sub


	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
		'DMCommon.Hebrew.zzParseChar(Me.txtInput.Text)
		'Return
      Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(Me.txtLtoR.Text, True)
		'	Me.txtLtoR.Text = Me.txtInput.Text
		Me.txtDisp.Text = oHebText.MultiLineSource
		Me.txtDispSort.Text = oHebText.GetWinDest(True)

	End Sub

	Private Sub txtInput_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtInput.TextChanged
		Me.txtLtoR.Text = txtInput.Text
	End Sub

	Private Sub txtDisp_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtDisp.TextChanged

	End Sub

	Private Sub txtDispSort_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtDispSort.TextChanged

	End Sub

	Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
		Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(Me.txtInputDOS.Text, False)
		'	Me.txtLtoR.Text = Me.txtInput.Text
		Me.txtDisp.Text = oHebText.MultiLineSource
		Me.txtDispSort.Text = oHebText.GetWinDest(True)
      Me.txtWinDestRtoL.Text = oHebText.GetWinDest(False)
   End Sub

   Private Sub Button4_Click(oSender As System.Object, e As EventArgs) Handles Button4.Click
      Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(Me.txtInputDOS.Text, False, True)
      '	Me.txtLtoR.Text = Me.txtInput.Text
      Me.txtDisp.Text = oHebText.MultiLineSource
      Me.txtDispSort.Text = oHebText.GetWinDest(True)
      Dim sWin As String = oHebText.GetWinDest(False)
      Me.txtWinDest.Text = sWin
      Me.txtWinDestRtoL.Text = sWin

   End Sub

  

   Private Sub cmdToCode_Click(oSender As System.Object, e As EventArgs) Handles cmdToCode.Click
      Me.txtCode.Text = DMCommon.Hebrew.ToASCCode(txtWinDest.Text, True)
   End Sub
 
End Class