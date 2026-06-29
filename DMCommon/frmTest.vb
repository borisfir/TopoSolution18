Public Class frmTest

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Dim s As String = Hebrew.GetHebNum(115, True)
		s = Hebrew.GetHebNum(115, False)
	End Sub

	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
		Dim s, sW, sDOS As String
		Dim i00, i01, i10, i11, i02, i12 As Integer
		Dim iA As Integer
		Dim sOut1, sOut2 As String
		s = Chr(Asc("א"))
		sW = ChrW(AscW("א"))
		sDOS = Chr(Asc("א") - 96)
		i00 = Asc(s)
		i11 = AscW(sW)
		i10 = AscW(s)
		i01 = Asc(sW)


		i12 = AscW(sDOS)
		i02 = Asc(sDOS)
		iA = i12 \ 32
		Dim sA As String = Hex(i12)
		sOut1 = ChrW(i11)
		sOut2 = ChrW(i10)
	End Sub
End Class