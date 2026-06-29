Option Explicit On
Option Strict On
Namespace TPlanGraph
   Public Class TplnUnionPgonPair
		Private moMergePgon As TplnOverlayPgon = Nothing
		Private moUnionPgon As TplnOverlayPgon = Nothing

		Public Sub New(ByVal iOverlayMethod As enOverlayMethod, ByVal oUnionPgon As TplnOverlayPgon)
			Me.Add(iOverlayMethod, oUnionPgon)
		End Sub
		Public Sub Add(ByVal iOverlayMethod As enOverlayMethod, ByVal oUnionPgon As TplnOverlayPgon)
			Select Case iOverlayMethod
				Case enOverlayMethod.Merge
					moMergePgon = oUnionPgon
				Case enOverlayMethod.Union
					moUnionPgon = oUnionPgon
			End Select
		End Sub
		Public ReadOnly Property MergePgon() As TplnOverlayPgon
			Get
				Try
					Return moMergePgon
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "moMergePgon - 45_760")
					Return Nothing
				End Try

			End Get
		End Property
		Public ReadOnly Property UnionPgon() As TplnOverlayPgon
			Get
				Return moUnionPgon
			End Get
		End Property
		Public Sub Has(ByVal iOverlayMethod As enOverlayMethod, ByVal UnionPgon As TplnOverlayPgon)

		End Sub
      'similar
   End Class

End Namespace

