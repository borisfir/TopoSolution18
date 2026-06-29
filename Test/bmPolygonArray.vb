Option Explicit On
Option Strict On
Friend Class bmPolygonArray
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

	Public ReadOnly Property CountAAA() As Integer
		Get
			Return 0
		End Get

	End Property
	 
	Public ReadOnly Property TotalArea() As Double
		Get
			Return mdTotalArea
		End Get
	End Property
	
	 
	Public Overridable Sub AddPolygon2009(ByVal oPolygon As BamashPolygon)
		Try
			MyBase.Add(oPolygon)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "PolygonArray - AddPolygon2009")
		End Try
	End Sub
	Public Sub Reset()

		Try
			'	Dim o As IComparer(Of BamashPolygon) = New MyComparer
			MyBase.Sort(New MyComparer)
		Catch oEx As Exception

		End Try

	End Sub
	 

	Public Sub New()
		MyBase.New()


	End Sub
	Private Class MyComparer
		Implements IComparer(Of BamashPolygon)

		Public Function Compare(ByVal x As BamashPolygon, ByVal y As BamashPolygon) As Integer Implements System.Collections.Generic.IComparer(Of BamashPolygon).Compare
			Return x.CompareTo(y.PropUnitKey)
		End Function
	End Class
End Class
