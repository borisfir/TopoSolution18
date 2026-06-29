Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class BlockReport
	Private msBlockPath As String
	Private msBlockName As String
	Private mdicAttribValues As Dictionary(Of String, String)
	Private mtInsertPoint As Autodesk.AutoCAD.Geometry.Point3d

	Public Sub New(ByVal sBlockPath As String, ByVal sBlockName As String)
		msBlockPath = sBlockPath
		msBlockName = sBlockName
	End Sub
	Public Property AttribValues() As Dictionary(Of String, String)
		Get
			Return mdicAttribValues
		End Get
		Set(ByVal dicValue As Dictionary(Of String, String))
			mdicAttribValues = dicValue
		End Set
	End Property

	Public Sub InsertBlockRef()
		Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim tBlockObjID As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msBlockName, oaAttribDefs)

		If Not tBlockObjID.IsNull Then
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim sPrompt As String = vbCrLf & "Enter insert point of the block"
			Dim bRes As Boolean
			bRes = DMAcadExt.AcadUtil.GetPoint(sPrompt, tPoint)
			If bRes Then
				mtInsertPoint = tPoint
				DMAcadExt.AcadTransaction.InsertBlockRef(tBlockObjID, mtInsertPoint, oaAttribDefs, mdicAttribValues, RepApp.DrawingScaleFactor)
			End If
		End If
	End Sub
End Class
