Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices

Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.Gis.Map
Imports Autodesk.Gis.Map.Project
Public Class AcadMapApp
	'taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
	Public Shared Function DispArray(ByVal oaValue() As Autodesk.AutoCAD.DatabaseServices.TypedValue, ByVal sTitle As String, ByVal bMsgBox As Boolean) As String
		Dim sMsg As String = String.Empty
		If oaValue Is Nothing Then
			sMsg = "Array Is Nothing"
		Else
			sMsg = "||"
			Try
				For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
					If iIndex <> 0 Then
						sMsg &= vbCrLf
					End If

					sMsg &= oaValue(iIndex).Value.ToString


				Next
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
			End Try
		End If
		If bMsgBox Then
			System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
		End If
		Return sMsg

	End Function
	Public Shared Sub ClearOD(ByVal oDBObject As DBObject)
		'	Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTables As ObjectData.Tables
		Dim oRecords As ObjectData.Records = Nothing
		Dim oEnum As IEnumerator
		Dim tAcObjID As ObjectId
		oTables = HostMapApplicationServices.Application.ActiveProject.ODTables
		AcadDocument.WriteMessage("Tables: " & CStr(oTables.TablesCount))
		Try
			oRecords = oTables.GetObjectRecords(Convert.ToUInt32(0), oDBObject, Constants.OpenMode.OpenForWrite, False)
		Catch oMapEx As MapException
			DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "dmLineCleanup - ClearOD")
		End Try
		If oRecords IsNot Nothing Then
			Try

				AcadDocument.WriteMessage("Records: " & CStr(oRecords.Count))
				oEnum = oRecords.GetEnumerator()
				AcadDocument.WriteMessage("oEnum: ")
				oEnum.Reset()
				AcadDocument.WriteMessage("Before ObjID=" & oDBObject.ObjectId.ToString())
				Do While oEnum.MoveNext()
					Try
						tAcObjID = oRecords.CurrentObjectId()
						oRecords.RemoveRecord()
						AcadDocument.WriteMessage("ObjID=" & tAcObjID.ToString())
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "AcadMapApp - ClearOD")
					End Try
				Loop
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "21_325")
			Finally
				oRecords.Dispose()
			End Try

		End If




	End Sub
	Public Shared Function GetTopology(ByVal sTopoName As String) As Autodesk.Gis.Map.Topology.TopologyModel
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oProject.Topologies
		If sTopoName Is Nothing Then
			Return Nothing
		ElseIf oTopos.Exists(sTopoName) Then
			Return oTopos.Item(sTopoName)
		Else
			AcadDocument.WriteMessage(String.Format(vbCrLf & "The topology {0} doesn't exist!!!" & vbCrLf, sTopoName))
			Return Nothing
		End If
	End Function
	Public Shared Function GetTopology(ByVal sTopoName As String, ByVal sAltTopoName As String) As Autodesk.Gis.Map.Topology.TopologyModel
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oProject As Autodesk.Gis.Map.Project.ProjectModel = oMapApplication.ActiveProject
		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = oProject.Topologies
		If sTopoName Is Nothing Then
			Return Nothing
		ElseIf oTopos.Exists(sTopoName) Then
			Return oTopos.Item(sTopoName)
		ElseIf oTopos.Exists(sAltTopoName) Then
			Return oTopos.Item(sAltTopoName)
		Else
			AcadDocument.WriteMessage(String.Format(vbCrLf & "The topology {0} doesn't exist!!!" & vbCrLf, sTopoName))
			Return Nothing
		End If
   End Function
   Public Shared Function GetDrawingSet() As DrawingSet
      Dim oMapApplication As MapApplication = HostMapApplicationServices.Application
      Dim oActiveProject As ProjectModel = oMapApplication.ActiveProject
      Dim colProjects As ProjectCollection = oMapApplication.Projects
      Return oActiveProject.DrawingSet
   End Function
End Class
