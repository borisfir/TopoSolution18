Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry
Public Class AcadDocumentB

	Private Shared WithEvents moEditor As Editor = Application.DocumentManager.MdiActiveDocument.Editor
	Private ptopts As PromptPointOptions = New PromptPointOptions("Enter start point of the line!!!")
	Private Shared mbMessageSended As Boolean
	Private Shared moDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
	Private Shared mbLogOpened As Boolean = False
	Private Shared moStreamWriter As IO.StreamWriter = Nothing
	Public Sub zz()
		ptopts.BasePoint = New Point3d(1, 1, 1)
		ptopts.UseDashedLine = True
		ptopts.Message = "Enter start point of the line"
		Dim ptRes As PromptPointResult = moEditor.GetPoint(ptopts)
		Dim vP As Point3d = ptRes.Value
		System.Windows.Forms.MessageBox.Show(CStr(vP.X), "X")

	End Sub
	Public Shared Sub OpenLog()
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document = Application.DocumentManager.GetDocument(oCurrentDatabase)
		Dim sAcadDocumentName As String = oAcadDocument.Name
		Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(sAcadDocumentName)
		Dim sLogName As String = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length) & ".log"


		Try
			moStreamWriter = New IO.StreamWriter(sLogName, False, System.Text.Encoding.Default)
			mbLogOpened = True
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(sLogName, "21_573")
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - OpenLog")
		End Try


	End Sub
	Public Shared Sub CloseLog()
		Try
			If mbLogOpened Then
				moStreamWriter.Close()
				mbLogOpened = False
			End If

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - CloseLog")
		End Try

	End Sub
	
	Public Shared Sub WriteMessage(ByVal sMsg As String, ByVal ParamArray oParams() As System.Object)
		If mbMessageSended Then
			sMsg = vbCrLf & sMsg
		End If
		Try
			If mbLogOpened Then
				moStreamWriter.WriteLine(sMsg)
			End If
			moEditor.WriteMessage(sMsg, oParams)
			mbMessageSended = True
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sMsg, "AcadDocument - WriteMessage")
		End Try
	End Sub
	Public Shared Sub CloseMessage()
		If mbMessageSended Then
			zzCommandLine(True)
			mbMessageSended = False
			If mbLogOpened Then
				Try
					If Not (moStreamWriter Is Nothing) Then
						moStreamWriter.Close()
					End If
				Catch oEx As Exception

				End Try
				mbLogOpened = False
			End If
		End If
	End Sub
	Public Shared Sub DocLock(ByVal bMode As DocumentLockMode, ByVal bCommandLine As Boolean)
		Dim sCommandName As String
		If bCommandLine Then
			sCommandName = "CommandLine"
		Else
			sCommandName = String.Empty
		End If
		If moDocLock IsNot Nothing Then
			System.Windows.Forms.MessageBox.Show(moDocLock.ToString(), "21_100 DocLock!!!!!")
		Else
			Try
				moDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(bMode, sCommandName, String.Empty, True)
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "AcadDocument - DocLock")
			End Try
		End If

	End Sub
	Public Shared Sub Unlock()
		If moDocLock IsNot Nothing Then
			Try
				moDocLock.Dispose()
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "AcadDocument - Unlock")
			End Try

			moDocLock = Nothing
		End If
	End Sub
	Public Shared Function IsLocked() As Boolean
		Return (moDocLock IsNot Nothing)
	End Function

	Private Shared Sub zzCommandLine(ByVal bActivate As Boolean)
		Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", bActivate, False, False)
	End Sub

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
End Class
