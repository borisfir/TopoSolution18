Option Explicit On
Option Strict On
Namespace TPlanGraph
   Public Structure UnionKey
      Public ParcelTopoID As Integer
      Public LotTopoID As Integer
      Public Sub New(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer)
         ParcelTopoID = iParcelTopoID
         LotTopoID = iLotTopoID
      End Sub
   End Structure
   Public Class TplnUnionGroup
      Private miParcelTopoID As Integer
      Private miLotTopoID As Integer
      Private moaUnionPgonPair() As TplnUnionPgonPair
      Private miUnionPgonPairUB As Integer = -1
      Private miInitOverlayMethod As enOverlayMethod
      Public Sub New(ByVal iParcelTopoID As Integer, ByVal iLotTopoID As Integer)
         miParcelTopoID = iParcelTopoID
         miLotTopoID = iLotTopoID
      End Sub
		Public Sub AddUnionPgon(ByVal iOverlayMethod As enOverlayMethod, ByVal oUnionPgon As TplnOverlayPgon)
			Dim sTestM As String = "a"
			If miUnionPgonPairUB = -1 Then
				sTestM &= "1"
				miInitOverlayMethod = iOverlayMethod
			End If
			sTestM &= "2"
			Try
				If miInitOverlayMethod = iOverlayMethod Then
					sTestM &= "3"
					miUnionPgonPairUB += 1
					ReDim Preserve moaUnionPgonPair(miUnionPgonPairUB)
					sTestM &= "4"
					moaUnionPgonPair(miUnionPgonPairUB) = New TplnUnionPgonPair(iOverlayMethod, oUnionPgon)
					sTestM &= "5"
				ElseIf miUnionPgonPairUB = 0 Then
					sTestM &= "6"
					moaUnionPgonPair(0).Add(iOverlayMethod, oUnionPgon)
					sTestM &= "7"
				Else
					sTestM &= "8"
					For iIndex As Integer = 0 To miUnionPgonPairUB
						'  moaUnionPgonPair(iIndex).Has(iOverlayMethod, )
					Next
				End If
				sTestM &= "9"
				If moaUnionPgonPair(miUnionPgonPairUB) Is Nothing Then
					sTestM &= "u"
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnUnionGroup - AddUnionPgon")
			End Try

			Try
				If miUnionPgonPairUB = 1 AndAlso moaUnionPgonPair IsNot Nothing Then
					sTestM &= "x"
					Dim sTest As String

					Dim oPgon As TplnOverlayPgon
               sTest = CStr(miParcelTopoID & ":" & CStr(miLotTopoID)) & "--?*-"
					sTestM &= "b"
					oPgon = moaUnionPgonPair(0).MergePgon
					sTestM &= "c"
					If oPgon Is Nothing Then
						sTest &= "Nothing: "
						sTestM &= "d"

					Else
						sTestM &= "e"
						sTest &= CStr(oPgon.AcadArea(False)) & ":"

					End If
					sTestM &= "f"
					oPgon = moaUnionPgonPair(1).MergePgon
					sTestM &= "g"
					If oPgon Is Nothing Then
						sTest &= "Nothing "
						sTestM &= "h"
					Else
						sTestM &= "i"
						sTest &= CStr(oPgon.AcadArea(False))
					End If
					sTestM &= "j"
               '	DMAcadExt.AcadDocument.WriteDebugMessage(sTest)
					sTestM &= "k"
				ElseIf moaUnionPgonPair Is Nothing Then
					System.Windows.Forms.MessageBox.Show("moaUnionPgonPair Is Nothing", "45_437")
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTestM & vbCrLf & CStr(miUnionPgonPairUB), "TplnUnionGroup - AddUnionPgon-Test")
			End Try

		End Sub
		Public ReadOnly Property AcadArea(ByVal bDun As Boolean) As Double
			Get
				If moaUnionPgonPair IsNot Nothing Then
					Dim dAcadArea As Double = 0.0
					Dim oUnionPgon As TplnOverlayPgon
					For iIndex As Integer = 0 To miUnionPgonPairUB
						If moaUnionPgonPair(iIndex) IsNot Nothing Then
							oUnionPgon = moaUnionPgonPair(iIndex).MergePgon
							If oUnionPgon IsNot Nothing Then
								dAcadArea += oUnionPgon.AcadArea(False)
							End If
						End If
					Next
					If bDun Then
						Return dAcadArea / TplnProject.UnitScaleFactor
					Else
						Return dAcadArea
					End If
				Else
					Return 0.0
				End If
			End Get
		End Property
		' ssssss

		Public ReadOnly Property CalcArea(ByVal bDun As Boolean) As Double
			Get
				If moaUnionPgonPair IsNot Nothing Then
					Dim dRes As Double = 0.0
					Dim oUnionPgon As TplnOverlayPgon
					For iIndex As Integer = 0 To miUnionPgonPairUB
						If moaUnionPgonPair(iIndex) IsNot Nothing Then
							oUnionPgon = moaUnionPgonPair(iIndex).MergePgon
							If oUnionPgon IsNot Nothing Then
								dRes += oUnionPgon.CalcArea()
							End If
						End If

						'  dCalcArea += 1000.0
					Next
					If bDun Then
						Return dRes / TplnProject.UnitScaleFactor
					Else
						Return dRes
					End If
				Else
					Return 0.0
				End If
			End Get
		End Property
      Public ReadOnly Property TUnionKey() As UnionKey
         Get
            Return New UnionKey(miParcelTopoID, miLotTopoID)
         End Get
      End Property
   End Class
End Namespace