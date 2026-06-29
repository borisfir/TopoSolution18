Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map

Imports Autodesk.Gis.Map.Utilities
Imports System.Runtime.InteropServices

Public NotInheritable Class NetApplication
   Private mfDispModelSpace As DispModelSpace
   <CommandMethod("DispModel")> _
      Public Sub DispModel()
      mfDispModelSpace = New DispModelSpace
      mfDispModelSpace.Fill()
      System.Windows.Forms.MessageBox.Show("2000")
      mfDispModelSpace.ShowDialog()
      System.Windows.Forms.MessageBox.Show("3000")
   End Sub
End Class
