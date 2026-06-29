Option Explicit On
Option Strict On
Public Enum TPlProviderAAA
	ProviderNotDefined
	ProviderJet = 1
	ProviderSQLServer = 2
	ProviderOracle = 3
End Enum

Public NotInheritable Class Common

	Const MOUSE_MOVED As Integer = &H1
	Const MOUSEEVENTF_ABSOLUTE As Integer = &H8000
	Const MOUSEEVENTF_LEFTDOWN As Integer = &H2
	Const MOUSEEVENTF_LEFTUP As Integer = &H4
	Const MOUSEEVENTF_MIDDLEDOWN As Integer = &H20
	Const MOUSEEVENTF_MIDDLEUP As Integer = &H40
	Const MOUSEEVENTF_MOVE As Integer = &H1
	Const MOUSEEVENTF_RIGHTDOWN As Integer = &H8
	Const MOUSEEVENTF_RIGHTUP As Integer = &H10

	Public Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As Integer
	Public Declare Function SetActiveWindow Lib "user32" Alias "SetActiveWindow" (ByVal hwnd As Integer) As Integer

	Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As Integer, ByVal lpString As String, ByVal cch As Integer) As Integer
	Public Declare Function GetWindowTextLength Lib "user32" Alias "GetWindowTextLengthA" (ByVal hwnd As Integer) As Integer
	Public Declare Function GetFocus Lib "user32" Alias "GetFocus" () As Integer
	Public Declare Function GetInputState Lib "user32" Alias "GetInputState" () As Integer
	Public Declare Function GetWindowDC Lib "user32" Alias "GetWindowDC" (ByVal hwnd As Long) As Integer
	Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)


	Private Const miResourceTheme As Integer = TPlServerDB.enResourceTheme.FormMDIMain
	'Public Const AcadBlockRefName As String = "AcDbBlockReference"
	'Public Const AcadPolylineName As String = "AcDbPolyline"
	Public Const AcadLineName As String = "AcDbLine"

	Public Const AcadLWPolylineName As String = "AcadLWPolyline"
	'	Public Const Acad2dPolylineName As String = "AcDb2dPolyline"

	Public Const UnionTopoPrefix As String = "dmUn"

	Public Const AppFile As String = " "
	Public Const TabaAppName As String = "TownPlanner"
	Public Const TopoMasterAppName As String = "TopoMaster"
	Public Const UnidivAppName As String = "Unidiv"

	Public Const SettingSectionName As String = "Settings"
	Public Const DBName As String = "tblData.mdb"
	Public Shared AppName As String

	'	Private Shared moMapApplication As Autodesk.Gis.Map.MapApplication = Nothing
	'	Private Shared moProject As Autodesk.Gis.Map.Project.ProjectModel = Nothing
	Public Shared Function GetTest() As String

		Return "1952"
	End Function

	Public Shared Function GetActiveWinText() As String
		Dim iHWnd As Integer = GetActiveWindow()
		Dim iWinTextLen As Integer = GetWindowTextLength(iHWnd)
		Dim cch As Integer = 255
		Dim sBuffer As String = Strings.Space(255)
		Dim iResp As Integer = GetWindowText(iHWnd, sBuffer, cch)
		Dim sOut As String = Left(sBuffer, iWinTextLen)
		Return sOut
	End Function
	Public Shared Sub MouseClick()
		mouse_event(MOUSEEVENTF_LEFTDOWN Or MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)


	End Sub


	


	
	'	Friend Shared Function GetNetAppPath() As String
	'	Return NetAppPath & AppFile
	'End Function
	'Friend Shared Function ToolbarIconFolder() As String
	'Return NetAppPath & "ToolbarIcons"
	'End Function

	Friend Shared Function GetTopologySourceCol(ByVal sTopoName As String) As Autodesk.Gis.Map.Topology.TopologySourceCollection
		'	zzInit()

		Dim oTopos As Autodesk.Gis.Map.Topology.Topologies = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.Topologies
		Dim oTopoSourceCol As Autodesk.Gis.Map.Topology.TopologySourceCollection = oTopos.GetSource(sTopoName)
		For Each oTopologySource As Autodesk.Gis.Map.Topology.TopologySource In oTopoSourceCol

		Next
		Return oTopoSourceCol
		' GetEditor.WriteMessage(String.Format(vbCrLf & "The topology {0} doesn't exist!!!", topologyName))


		'  oTopos.GetSource(

	End Function
	Public Shared Function GetEditor() As Autodesk.AutoCAD.EditorInput.Editor
		Return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
	End Function
	Public Shared Function GetMenu() As String
		Return Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.Database.Menu
	End Function
	Public Shared Function GetDocument() As Autodesk.AutoCAD.DatabaseServices.Database
		Return Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.Database
	End Function
	Public Shared Function NameSplit(ByVal sName As String) As String()
		Return Split(sName, "_", 2)
	End Function
	Public Shared Sub GetMapEx(ByVal oMapEx As Autodesk.Gis.Map.MapException, Optional ByVal sTitle As String = "")

		If sTitle.Length = 0 Then sTitle = "e300"

		Try
			System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.ErrorCode), sTitle)
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "e301")
		End Try
	End Sub

	Public Shared Sub GetMapTopoEx(ByVal SysEx As System.Exception, Optional ByVal sTitle As String = "")
		Dim oMapEx As Autodesk.Gis.Map.MapTopologyException
		If sTitle.Length = 0 Then sTitle = "e300"

		Try
			oMapEx = DirectCast(SysEx, Autodesk.Gis.Map.MapTopologyException)
			System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & CStr(oMapEx.AdsErrorCode), sTitle)
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - GetMapTopoEx_" & sTitle)
		End Try
	End Sub
	Public Shared Sub DispIntArray(ByVal iaValue() As Integer, ByVal sTitle As String)
		Dim sMsg As String = String.Empty
		Try
			For iIndex As Integer = 0 To iaValue.GetUpperBound(0)
				If iIndex <> 0 Then
					sMsg &= vbCrLf
				End If
				sMsg &= iaValue(iIndex).ToString
			Next
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispIntArray")
		End Try
		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)

	End Sub
	Public Shared Sub DispObjArray(ByVal oaValue() As System.Object, ByVal sTitle As String)
		Dim sMsg As String = String.Empty
		If oaValue Is Nothing Then
			sMsg = "Array Is Nothing"
		Else
			Try
				For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
					If iIndex <> 0 Then
						sMsg &= vbCrLf
					End If
					If oaValue(iIndex) IsNot Nothing Then
						sMsg &= oaValue(iIndex).ToString()
					Else
						sMsg &= "Nothing"
					End If

				Next
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Common-DispObjArray")
			End Try
		End If

		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)

	End Sub
	Public Shared Sub DispStrArray(ByVal saValue() As String, ByVal sTitle As String)
		Dim sMsg As String = String.Empty
		If saValue Is Nothing Then
			sMsg = "Array Is Nothing"
		Else
			Try
				For iIndex As Integer = 0 To saValue.GetUpperBound(0)
					If iIndex <> 0 Then
						sMsg &= vbCrLf
					End If
					sMsg &= saValue(iIndex)
				Next
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
			End Try
		End If

		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)

	End Sub
	Public Shared Function NumberFilter(ByVal sValue As System.String) As Integer
		Dim chaValue As Char() = sValue.ToCharArray()
		Dim chSymbol As Char
		Dim sOutput As String = String.Empty
		Dim iRes As Integer = 0
		For iIndex As Integer = 0& To chaValue.GetUpperBound(0)
			chSymbol = chaValue(iIndex)
			If System.Char.IsDigit(chSymbol) Then
				sOutput &= chSymbol.ToString()
			End If
		Next
		Try
			iRes = Convert.ToInt32(sOutput)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "'" & sOutput & "'", "Common - NumberFilter")
		End Try
		Return iRes
	End Function



	Private Function DB2Int(ByVal oValue As System.Object, Optional ByVal iNullValue As Integer = 0) As Integer
		Dim sTypeName As String = oValue.GetType().Name
		Select Case sTypeName
			Case "DBNull"
				Return iNullValue
			Case "Int32"
				Return DirectCast(oValue, System.Int32)
			Case Else
				Return System.Int32.MinValue
		End Select

	End Function

	Public Shared Sub SetAcadFocus()
		Dim sTitle As String = Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text

		Try
			Microsoft.VisualBasic.Interaction.AppActivate(sTitle)
		Catch oEx As Exception
			Windows.Forms.MessageBox.Show(oEx.Message, "Common - SetAcadFocus")
		End Try

	End Sub
	Private Shared Sub zzInit()
		'	If moMapApplication Is Nothing OrElse moProject Is Nothing Then
		'moMapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		'moProject = moMapApplication.ActiveProject
		'	End If

	End Sub

End Class
