Option Strict On
Option Explicit On

Namespace TPlanGraph



	Public Class TplnOwnershipNoteOverlayPgon
		Inherits TPlanGraph.TplnTopoPgon
		Private miFeatureID As Integer
		Protected minOwnershipNoteTopoID As Integer

		Protected miParcelTopoID As Integer
		Private miOverlayMethod As DMAcadExt.enOverlayMethod
		Public Sub New(ByRef oPolygon As Autodesk.AutoCAD.DatabaseServices.Polyline, ByVal iFeatureID As Integer, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iOwnershipNoteTopoID As Integer, ByVal iParcelTopoID As Integer)
			MyBase.New(oPolygon, iFeatureID)
			DMAcadExt.AcadDocument.WriteMessage("CC" & iOverlayMethod.ToString() & ":" & iOverlayMethod.ToString() & ":" & iOwnershipNoteTopoID.ToString())
			minOwnershipNoteTopoID = iOwnershipNoteTopoID
			miParcelTopoID = iParcelTopoID
			miOverlayMethod = iOverlayMethod

			dcolLines = New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
			dcolLines.Add(oPolygon.ObjectId)
		End Sub

		Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
			Get
				Throw New NotImplementedException()
			End Get
		End Property

		Public Overrides Sub Terminate()
			Throw New NotImplementedException()
		End Sub
	End Class
End Namespace