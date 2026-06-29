Option Explicit On
Option Strict On
Public Class bmPolygonArray
	Inherits Generic.List(Of BamashPolygon)
	Private miCountAAA As Integer
	Private miIndex As Integer

	Private miaTopoID() As Integer
	Private miaPolygonID() As Integer
	Private msaDescr() As String
	Private msaDescr2() As String
	Private mdaArea() As Double
	Private mdTotalArea As Double = 0.0
	'	Private mlstPolygons As Generic.List(Of BamashPolygon)



	Public ReadOnly Property TotalArea() As Double
		Get
			Return mdTotalArea
		End Get
	End Property

	Public Sub SetColor(ByVal iColor As Integer)
		For iIndex As Integer = 0 To MyBase.Count - 1
			If Me.Item(iIndex).PropertyType <> enPropertyTypes.UnderBldTypeWhite Then
				Me.Item(iIndex).PolygonColor = iColor
				Me.Item(iIndex).CreateColorScheme()
			End If
		Next
	End Sub
	Public Overridable Sub AddPolygon2009(ByVal oPolygon As BamashPolygon)
		Try
			MyBase.Add(oPolygon)
			If oPolygon.PropertyType <> enPropertyTypes.UnderBldTypeColor Then
				mdTotalArea += oPolygon.CalcArea
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "PolygonArray - AddPolygon2009")
		End Try
	End Sub
	Public Sub SortPgons()
		Try
			'Dim o As IComparer(Of BamashPolygon) = New PgonComparer
			MyBase.Sort(New PgonComparer())

		Catch oEx As Exception

		End Try
	End Sub
	Public Sub SortSubpropsNumberingSeparately()
		MyBase.Sort(New AprtDescNumComparer())
	End Sub
	Public Sub SortSubprops(ByVal iCalcOption As bmBamash.SubNumerationOptions)
		Try
			Select Case iCalcOption
				Case bmBamash.SubNumerationOptions.All
					MyBase.Sort(New PgonComparer())
				Case bmBamash.SubNumerationOptions.None
					MyBase.Sort(New SubpropComparer())
				Case bmBamash.SubNumerationOptions.EmptyFirst, bmBamash.SubNumerationOptions.EmptyMax
					MyBase.Sort(New MixedComparer())
			End Select


		Catch oEx As Exception

		End Try
	End Sub
	Public ReadOnly Property TopoID(ByVal iIndex As Integer) As Integer
		Get
			Return MyBase.Item(iIndex).TopoID
		End Get
	End Property

	Private Class PgonComparer
		Implements IComparer(Of BamashPolygon)

		Public Function Compare(ByVal oBamashPolygonA As BamashPolygon, ByVal oBamashPolygonB As BamashPolygon) As Integer Implements System.Collections.Generic.IComparer(Of BamashPolygon).Compare
			Return oBamashPolygonA.CompareTo(oBamashPolygonB.PropUnitKey)
		End Function
	End Class
	Private Class SubpropComparer
		Implements IComparer(Of BamashPolygon)

		Public Function Compare(ByVal oBamashPolygonA As BamashPolygon, ByVal oBamashPolygonB As BamashPolygon) As Integer Implements System.Collections.Generic.IComparer(Of BamashPolygon).Compare
			If oBamashPolygonA.SubPropNum < oBamashPolygonB.SubPropNum Then
				Return -1
			ElseIf oBamashPolygonA.SubPropNum > oBamashPolygonB.SubPropNum Then
				Return 1
			Else
				Return 0
			End If
		End Function
	End Class
	Private Class AprtDescNumComparer
		Implements IComparer(Of BamashPolygon)

		Public Function Compare(ByVal oBamashPolygonA As BamashPolygon, ByVal oBamashPolygonB As BamashPolygon) As Integer Implements System.Collections.Generic.IComparer(Of BamashPolygon).Compare
			If oBamashPolygonA.AprtDescNum < oBamashPolygonB.AprtDescNum Then
				Return -1
			ElseIf oBamashPolygonA.AprtDescNum > oBamashPolygonB.AprtDescNum Then
				Return 1
			Else
				Return 0
			End If
		End Function
	End Class

	Private Class MixedComparer
		Implements IComparer(Of BamashPolygon)
		Private moPgonComparer As PgonComparer
		Private moSubpropComparer As SubpropComparer

		Public Sub New()
			moPgonComparer = New PgonComparer()
			moSubpropComparer = New SubpropComparer()
		End Sub
		Public Function Compare(oBamashPolygonA As BamashPolygon, oBamashPolygonB As BamashPolygon) As Integer Implements System.Collections.Generic.IComparer(Of BamashPolygon).Compare
			If oBamashPolygonA.SubPropNum = 0 AndAlso oBamashPolygonB.SubPropNum = 0 Then
				Return moPgonComparer.Compare(oBamashPolygonA, oBamashPolygonB)
			ElseIf oBamashPolygonA.SubPropNum <> 0 AndAlso oBamashPolygonB.SubPropNum = 0 Then
				Return -1
			ElseIf oBamashPolygonA.SubPropNum = 0 AndAlso oBamashPolygonB.SubPropNum <> 0 Then
				Return 1
			ElseIf oBamashPolygonA.SubPropNum <> 0 AndAlso oBamashPolygonB.SubPropNum <> 0 Then
				Return moSubpropComparer.Compare(oBamashPolygonA, oBamashPolygonB)
			End If
		End Function
	End Class
End Class
