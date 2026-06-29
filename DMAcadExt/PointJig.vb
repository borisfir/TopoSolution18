
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Public MustInherit Class PointJig
	Inherits EntityJig
	Protected ptCurrentPoint As Point2d
	Public Sub New(oEntity As Entity)
		MyBase.New(oEntity)
	End Sub

	Public Function GetPoint() As Point2d

		Return ptCurrentPoint
	End Function


End Class



