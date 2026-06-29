Imports sYstem.Windows.Forms
Imports System.Drawing
Public Class Form1
	Enum TrueFalse
		NotDefined
		Tr
		Fls
	End Enum
	Private Shared mdicHebNumbers As Generic.Dictionary(Of String, Integer)
	Private moCursor As Cursor
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		'	Dim ires As Integer = Heb2Num("ä")
		Dim iaRes() As Integer = zzGetArray()
		Dim i As TrueFalse
		System.Windows.Forms.MessageBox.Show(i.ToString())
	End Sub
	Private Function zzGetArray() As Integer()
		Dim iaOut() As Integer = {1, 2, 3}
		Return iaOut
	End Function
	Public Function GetHebNum(ByVal iInput As Integer) As String
		Const sP1 As String = "'"
		Const sP2 As String = """"
		Dim iBase As Integer
		Dim iUnit As Integer, iDec As Integer
		Dim sHeb As String = String.Empty
		Dim iVal As Integer
		Dim iRetVal As Integer
		Dim sOutput As String = String.Empty
		On Error Resume Next
		iBase = Asc("à") - 1
		iUnit = iInput Mod 10
		iDec = CInt((iInput - iUnit) / 10)
		Select Case iInput
			Case 1 To 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400
				zzGetHebLetter(iInput, sHeb, iRetVal)
				Return sHeb & sP1
			Case 15, 16
				Return "è" & sP2 & Chr(iBase + iInput - 9)
			Case Else
				Do
					zzGetHebLetter(iInput, sHeb, iRetVal)
					iInput = iInput - iRetVal
					If iInput = 0& Then
						sOutput &= sP2 & sHeb
					Else
						sOutput &= sHeb
					End If
				Loop While iInput > 0
				Return sOutput
		End Select
	End Function

	Private Sub zzGetHebLetter(ByVal iInput As Integer, ByRef sHeb As String, ByRef iVal As Integer)
		Dim iBase As Integer
		Dim iDec As Integer, iHundred As Integer
		Dim iAddit As Integer
		iBase = Asc("à") - 1
		If iInput <= 10 Then
			sHeb = Chr(iBase + iInput)
			iVal = iInput
		ElseIf iInput <= 100& Then
			iDec = iInput \ 10
			If iDec >= 2 Then iAddit = 1
			If iDec >= 4 Then iAddit = 2
			If iDec >= 5 Then iAddit = 3
			If iDec >= 8 Then iAddit = 4
			If iDec >= 9 Then iAddit = 5
			sHeb = Chr(iBase + 9 + iDec + iAddit)
			iVal = iDec * 10
		Else
			If iInput > 400& Then iInput = 400
			iHundred = CInt(iInput / 100)
			sHeb = Chr(iBase + 23 + iHundred + iAddit)
			iVal = iHundred * 100
		End If
	End Sub
	Public Function Heb2Num(ByVal sValue As String) As Integer
		If mdicHebNumbers Is Nothing Then
			mdicHebNumbers = New Generic.Dictionary(Of String, Integer)
			For iIndex As Integer = 1 To 400
				mdicHebNumbers.Add(GetHebNum(iIndex), iIndex)
			Next
		End If
		If mdicHebNumbers.ContainsKey(sValue) Then
			Return mdicHebNumbers.Item(sValue)
		Else
			Return 0
		End If
	End Function

	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
		Dim i As Integer
		Dim oRnd As Random = New Random
		i = oRnd.Next
		i = oRnd.Next
		i = oRnd.Next
		i = oRnd.Next

	End Sub

	Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
		Dim b1, b2, b3, b4, b5 As Boolean

		b1 = IsDate("1-12")
		b2 = IsDate("13-13")
		b3 = IsDate("1-45")
		b4 = IsDate("45-1")
		b5 = IsDate("1-12-13")
		Dim sOut As String = CStr(b1) & ":" & CStr(b2) & ":" & CStr(b3) & ":" & CStr(b4) & ":" & CStr(b5)
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(10 / 3))
		iPart = Convert.ToInt32(Math.Ceiling(10 / 6))
		iPart = Convert.ToInt32(Math.Ceiling(10 / 2))
		iPart = Convert.ToInt32(Math.Ceiling(-10 / 3))
		iPart = Convert.ToInt32(Math.Ceiling(-10 / 6))
		iPart = Convert.ToInt32(Math.Ceiling(-10 / 2))
		Math.Ceiling(1 / 2)
	End Sub

	Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
		Dim oPgonArray As bmPolygonArray = New bmPolygonArray()
		Dim oPgon As BamashPolygon
		oPgon = New BamashPolygon(4, 5, 2, 2, 2)
		oPgonArray.AddPolygon2009(oPgon)
		oPgon = New BamashPolygon(3, 5, 2, 2, 2)
		oPgonArray.AddPolygon2009(oPgon)
		oPgon = New BamashPolygon(3, 5, 4, 2, 2)
		oPgonArray.AddPolygon2009(oPgon)
		oPgon = New BamashPolygon(2, 5, 2, 2, 2)
		oPgonArray.AddPolygon2009(oPgon)
		oPgonArray.Reset()
		For iIndex As Integer = 0 To oPgonArray.Count - 1
			oPgon = oPgonArray.Item(iIndex)
			Debug.Print(oPgon.Disp)
		Next
	End Sub

	Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
		Dim oaBytes(4) As Byte
		Dim s As String = oaBytes.ToString()
		Dim l As Long
		Dim sa As String = Hex(1507)
		Dim d As Double
		d = Val("&H5E3")
		l = Convert.ToInt64(Val("&H5E3"))
		l = Convert.ToInt64(1.4)
		l = Convert.ToInt64(1.6)


	End Sub

	Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
		System.Windows.Forms.MessageBox.Show("")
	End Sub

	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

	End Sub

	Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
		Dim Sum As Long
		Dim Code As Long
		Dim r As Long

		For Sum = 0 To 10

			Code = Math.DivRem(Sum * (Sum + 1), 2L, r)

			Debug.Print(Code.ToString() & ":" & r.ToString())
		Next

	End Sub

	Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
		Dim d As Double = 723456.12345678906
		Dim dec As Decimal = 123456.123456789123456789D
		d = 123456.0 + 1 / 3
		dec = Convert.ToDecimal(d)

		System.Windows.Forms.MessageBox.Show(CStr(d) & vbCrLf & CStr(dec))
		dec *= 444444
		dec /= 444444
		d *= 444444
		d /= 444444
		System.Windows.Forms.MessageBox.Show(CStr(d) & vbCrLf & CStr(dec))

	End Sub

	Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
		Dim mdicRejectedNames As Dictionary(Of String, String)
		mdicRejectedNames = New Dictionary(Of String, String)
		mdicRejectedNames.Add("ab", String.Empty)
		mdicRejectedNames.Add("bc", String.Empty)
		Dim RejectedNames As System.Collections.Generic.Dictionary(Of String, String).KeyCollection = mdicRejectedNames.Keys
		For Each sKey As String In RejectedNames
			Debug.Print(sKey)
			Stop
		Next
	End Sub

	Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
		'	Dim oItem As System.Windows.Forms.ListViewItem = Me.ListView1.Items(0)

		MoveItem(Windows.Forms.SearchDirectionHint.Down)

	End Sub
	Private Sub MoveItem(ByVal iDir As System.Windows.Forms.SearchDirectionHint)
		Dim oListViewItem As System.Windows.Forms.ListViewItem
		Dim oListViewItemNext As System.Windows.Forms.ListViewItem

		If Me.ListView1.SelectedItems.Count > 0 Then
			oListViewItem = DirectCast(Me.ListView1.SelectedItems.Item(0), System.Windows.Forms.ListViewItem)

			'	oListViewItemNext = Me.ListView1.Items(iIndex + 1)
			oListViewItemNext = oListViewItem.FindNearestItem(iDir)
			If oListViewItemNext IsNot Nothing Then
				Me.ListView1.Items.Remove(oListViewItem)
				Me.ListView1.Items.Remove(oListViewItemNext)
				If Me.ListView1.View = Windows.Forms.View.LargeIcon Then
					Select Case iDir
						Case Windows.Forms.SearchDirectionHint.Down, Windows.Forms.SearchDirectionHint.Right
							Me.ListView1.Items.Add(oListViewItemNext)
							Me.ListView1.Items.Add(oListViewItem)
						Case Else
							Me.ListView1.Items.Add(oListViewItem)
							Me.ListView1.Items.Add(oListViewItemNext)
					End Select
				ElseIf Me.ListView1.View = Windows.Forms.View.SmallIcon Then
					Select Case iDir
						Case Windows.Forms.SearchDirectionHint.Down, Windows.Forms.SearchDirectionHint.Right
							'	Me.ListView1.Items.Add(oListViewItem)
							'	Me.ListView1.Items.Add(oListViewItemNext)
						Case Else
							'	Me.ListView1.Items.Add(oListViewItem)
							'	Me.ListView1.Items.Add(oListViewItemNext)
					End Select
				End If

			End If
		End If
	End Sub

	Private Sub Button10_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button10.Click
		MoveItem(Windows.Forms.SearchDirectionHint.Up)
	End Sub
	Private Sub Button11_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button11.Click
		MoveItem(Windows.Forms.SearchDirectionHint.Left)
	End Sub
	Private Sub Button12_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button12.Click
		MoveItem(Windows.Forms.SearchDirectionHint.Right)
	End Sub

	Private Sub Button13_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button13.Click
		Dim s As String
		Dim oList As List(Of System.Windows.Forms.ListViewItem) = New List(Of System.Windows.Forms.ListViewItem)
		Dim oItemComparer As ItemComparer = New ItemComparer(Windows.Forms.View.LargeIcon)
		For Each oItem As System.Windows.Forms.ListViewItem In Me.ListView1.Items
			oList.Add(oItem)
			s = oItem.Text
			'oItem.Position = New Drawing.Point(3, 6)
		Next

		oList.Sort(oItemComparer)
		For Each oItem As System.Windows.Forms.ListViewItem In oList
			s = oItem.Text
		Next
	End Sub
	Private Class ItemComparer
		Implements System.Collections.Generic.IComparer(Of System.Windows.Forms.ListViewItem)
		Private miViewType As System.Windows.Forms.View
		Public Sub New(ByVal iViewType As System.Windows.Forms.View)
			miViewType = iViewType
		End Sub
		Public Function Compare(ByVal oItemA As System.Windows.Forms.ListViewItem, ByVal oItemB As System.Windows.Forms.ListViewItem) As Integer Implements System.Collections.Generic.IComparer(Of System.Windows.Forms.ListViewItem).Compare
			Dim tPointA As Drawing.Point = oItemA.Position
			Dim tPointB As Drawing.Point = oItemB.Position
			Select Case miViewType
				Case Windows.Forms.View.LargeIcon
					If tPointA.Y < tPointB.Y Then
						Return -1
					ElseIf tPointA.Y > tPointB.Y Then
						Return 0
					ElseIf tPointA.X < tPointB.X Then
						Return -1
					ElseIf tPointA.X > tPointB.X Then
						Return 1
					Else
						Return 0
					End If
			End Select
		End Function
	End Class

	Private Sub ListView1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseDown
		'	Me.Cursor = Windows.Forms.Cursors.Cross
		'	DrawCursorsOnForm(Me.Cursor)

		moCursor = New Cursor("H:\Graphics\Cursors\DRAGPICT.CUR")
		'Dim oCursor As Cursor = New Cursor("C:\Program Files\Microsoft Visual Studio 8\Common7\VS2005ImageLibrary\VS2005ImageLibrary\bitmaps\misc\Arrow.bmp")

		Me.Cursor = moCursor
		'	System.Windows.Forms.MessageBox.Show(sender.ToString())
	End Sub

	Private Sub ListView1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.MouseHover
		'System.Windows.Forms.MessageBox.Show(sender.ToString())

	End Sub

	Private Sub ListView1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles ListView1.MouseLeave
		Me.Cursor = Cursors.No
	End Sub

	Private Sub ListView1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseMove
		If moCursor IsNot Nothing Then
			Me.Cursor = moCursor

		End If

	End Sub

	Private Sub ListView1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView1.MouseUp
		Me.Cursor = Windows.Forms.Cursors.Default
		moCursor = Nothing
	End Sub

	Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged

	End Sub
	Private Sub DrawCursorsOnForm(ByVal cursor As Cursor)
		' If the form's cursor is not the Hand cursor and the 
		' Current cursor is the Default, Draw the specified 
		' cursor on the form in normal size and twice normal size. 
		If (Not Me.Cursor.Equals(Cursors.Hand)) Then


			' Draw the cursor stretched.
			Dim graphics As Graphics = Me.CreateGraphics()
			Dim rectangle As New Rectangle(New Point(10, 10), _
			  New Size(cursor.Size.Width * 4, cursor.Size.Height * 4))
			cursor.DrawStretched(graphics, rectangle)

			' Draw the cursor in normal size.
			rectangle.Location = New Point(rectangle.Width + _
			  rectangle.Location.X, rectangle.Height + rectangle.Location.Y)
			rectangle.Size = cursor.Size
			cursor.Draw(graphics, rectangle)

			' Dispose of the cursor.
			cursor.Dispose()
		End If
	End Sub

End Class
