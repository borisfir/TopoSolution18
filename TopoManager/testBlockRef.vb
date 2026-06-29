Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.GraphicsInterface
Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map

Imports Autodesk.Gis.Map.Utilities
Imports System.Runtime.InteropServices
Public Class testBlockRef
   Inherits BlockReference
   Public Sub New(ByVal position As Autodesk.AutoCAD.Geometry.Point3d, ByVal blockTableRecord As Autodesk.AutoCAD.DatabaseServices.ObjectId)
      MyBase.New(position, blockTableRecord)
      Common.GetEditor.WriteMessage("Created")
      System.Windows.Forms.MessageBox.Show("1333-Created")
   End Sub
	Public Function OnWorldDraw(ByVal oWorldDraw As WorldDraw) As Boolean 'Overrides
		MyBase.WorldDraw(oWorldDraw)
		System.Windows.Forms.MessageBox.Show("1300")
		Return MyBase.WorldDraw(oWorldDraw)

		' // world-only

	End Function
	Public Sub OnViewportDraw(ByVal oViewportDraw As ViewportDraw)	 'Overrides

		MyBase.ViewportDraw(oViewportDraw)
		System.Windows.Forms.MessageBox.Show("2500")
		MyBase.ViewportDraw(oViewportDraw)

		' // world-only

	End Sub
  
   Public Function GetMyName() As String
      Return "Boris"
   End Function
   Public Overloads Property Material() As String
      Get
         Return "Global"
      End Get
      Set(ByVal sValue As String)
         MyBase.Material = sValue
      End Set
   End Property
   Public Overloads Sub Draw()
      System.Windows.Forms.MessageBox.Show("2780")
      '   MyBase.Draw()
   End Sub
End Class
