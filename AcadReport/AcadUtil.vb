Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Class AcadUtil
	Public Shared Function GetEditor() As Autodesk.AutoCAD.EditorInput.Editor
		Return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
	End Function
   Public Shared Function GetTwoPoints(ByVal sPrompt() As String) As Autodesk.AutoCAD.Geometry.Point3d()
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
      Dim ptopts As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(sPrompt(0))
      ptopts.BasePoint = New Autodesk.AutoCAD.Geometry.Point3d(1, 1, 1)
      ptopts.UseBasePoint = False
      ptopts.UseDashedLine = True
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult = oEditor.GetPoint(ptopts)
      Dim oBasePoint As Autodesk.AutoCAD.Geometry.Point3d = ptRes.Value
      Dim oaOutput(1) As Autodesk.AutoCAD.Geometry.Point3d
      If ptRes.Status <> Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then

         ptopts.Message = sPrompt(1)
         ptopts.BasePoint = oBasePoint
         ptopts.UseBasePoint = True
         ptopts.UseDashedLine = True
         ptRes = oEditor.GetPoint(ptopts)
         'Autodesk.AutoCAD.EditorInput.PromptStatus.OK=5100; Other=5028 None=5000
         If ptRes.Status <> Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then

            Dim oEndPoint As Autodesk.AutoCAD.Geometry.Point3d = ptRes.Value
            oaOutput(0) = oBasePoint
            oaOutput(1) = oEndPoint
            Return oaOutput
         End If

      End If
      Return Nothing
   End Function
	Public Shared Function GetLines() As ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select Line ...")
		Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim colLines As ObjectIdCollection = New ObjectIdCollection
		oPromptOpt.AddAllowedClass(oPolyline.GetType(), True)
		Do
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				colLines.Add(ptRes.ObjectId)
			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
				Exit Do
				Return Nothing
			ElseIf ptRes.Status = (Autodesk.AutoCAD.EditorInput.PromptStatus.None Or Autodesk.AutoCAD.EditorInput.PromptStatus.Other) Then
				Exit Do
			End If
		Loop
		Return colLines
	End Function
   
End Class
