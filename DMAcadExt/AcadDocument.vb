Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.EditorInput
Imports Autodesk.AutoCAD.Geometry

Public Class AcadDocument

   Private Const msCommandDialogAcadVar As String = "CMDDIA"
   Private Const msScaleSysVarName As String = "USERR1"

   Private Const msPDModeSysVarName As String = "PDMODE"
   Private Const msPDSizeSysVarName As String = "PDSIZE"

	Private Shared WithEvents moDocumentCollection As Autodesk.AutoCAD.ApplicationServices.DocumentCollection = Application.DocumentManager
	Private Shared moEditor As Editor = Application.DocumentManager.MdiActiveDocument.Editor
	Private Shared mbMessageSended As Boolean
	Private Shared moDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
	Private Shared miDocumentLockMode As DocumentLockMode = DocumentLockMode.None


	Private Shared mbLogOpened As Boolean = False
	Private Shared msLogName As String
	Private Shared miCounter As Integer

	Private Shared moStreamWriter As IO.StreamWriter = Nothing
	Private Shared WithEvents moActiveDrawing As Autodesk.AutoCAD.ApplicationServices.Document
	Private Shared moDrawVectorSet As DrawVectorSet = Nothing  'WithEvents
	Private Shared msOneTimeCommandName As String
	Private Shared miOneTimeCommandNumber As Integer

	Private Shared miCommandCounter As Integer = -1

	Private Shared moOneTimeAfterComProc As AfterCommandProc

	Private Shared mshCmdDia As Short
	Private Shared mbCmdDiaSaved As Boolean = False

	Private Shared mshPDMode As Short
	Private Shared mdPDSize As Double
	Private Shared mbPointFormatSaved As Boolean = False

	Public Sub New()

	End Sub

	Public Delegate Sub AfterCommandProc(bDrawingClose As Boolean)
	'   Private Shared mbDebug As Boolean
	'   Private Shared mbDebugBoris As Boolean = True

	Public Shared Sub SaveVarCmdDia(ByVal shNewValue As Short)
		If Not mbCmdDiaSaved Then
			Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msCommandDialogAcadVar)
			mshCmdDia = DirectCast(oValue, System.Int16)

			If mshCmdDia <> shNewValue Then
				Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msCommandDialogAcadVar, shNewValue)
				mbCmdDiaSaved = True
			End If
			' DMCommon.Debug.MsgBox("09_202bef", mshCmdDia, shNewValue, Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msCommandDialogAcadVar))
		End If
	End Sub
	Public Shared Sub RestoreVarCmdDia()
		If mbCmdDiaSaved Then

			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msCommandDialogAcadVar, mshCmdDia)
			mbCmdDiaSaved = False
			'  DMCommon.Debug.MsgBox("09_203a", mshCmdDia, Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msCommandDialogAcadVar))
		End If
	End Sub
	Public Shared Sub SavePointFormat(ByVal shModeNewValue As Short, ByVal dSizeNewValue As Double)
		If Not mbPointFormatSaved Then
			Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msPDModeSysVarName)
			mshPDMode = DirectCast(oValue, System.Int16)

			oValue = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msPDSizeSysVarName)
			mdPDSize = DirectCast(oValue, System.Double)

			If mshPDMode <> shModeNewValue OrElse mdPDSize <> dSizeNewValue Then
				Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msPDModeSysVarName, shModeNewValue)
				Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msPDSizeSysVarName, dSizeNewValue)
				mbPointFormatSaved = True
			End If

			'DMCommon.Debug.MsgBox("09_202bef", mshPDMode, shModeNewValue, mdPDSize, dSizeNewValue)
		End If
	End Sub
	Public Shared Sub RestorePointFormat()
		If mbPointFormatSaved Then

			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msPDModeSysVarName, mshPDMode)
			Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable(msPDSizeSysVarName, mdPDSize)

			mbPointFormatSaved = False
			'  DMCommon.Debug.MsgBox("09_203a", mshCmdDia, Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msCommandDialogAcadVar))
		End If
	End Sub
	Public Shared ReadOnly Property PDSize As Double
		Get
			Return mdPDSize
		End Get

	End Property

	Public Shared ReadOnly Property PDMode As Short
		Get
			Return mshPDMode
		End Get

	End Property
	Public Shared ReadOnly Property PointFormatSaved As Boolean
		Get
			Return mbPointFormatSaved
		End Get

	End Property
	Public Shared Sub CloseCurrentDocument(ByVal bSave As Boolean)
		'	moDocumentCollection.MdiActiveDocument.CloseAndDiscard()
		'	Return
		Dim oDocument As Document = moDocumentCollection.CurrentDocument

		If bSave Then
			oDocument.CloseAndSave(oDocument.Name)
		Else
			oDocument.CloseAndDiscard()
		End If

	End Sub
	Public Shared Function SaveCurrentDocument() As Boolean
		Dim bRes As Boolean
		Dim oDocument As Document = moDocumentCollection.MdiActiveDocument
		Dim oDatabase As Autodesk.AutoCAD.DatabaseServices.Database = oDocument.Database
		'	DMCommon.Debug.MsgBox("!oDocument.Name", oDocument.Name)
		Try
			'oDatabase.Save()MdiActiveDocument		
			'	oDocument.Database.Save()
			oDatabase.SaveAs(oDocument.Name, True, Autodesk.AutoCAD.DatabaseServices.DwgVersion.Current, oDocument.Database.SecurityParameters)
			bRes = True
		Catch oEx As Exception
			DMCommon.Debug.ErrMsgBox("!SaveCurrentDocument", oEx)
			bRes = False
		End Try

		Return bRes

	End Function
	Public Shared Sub DrawShape(iMarkType As MarkBlock.enMarkBlockType, dScale As Double, tPoint As TPlnPoint, iColor As Integer, bHighlighted As Boolean)
		Dim taPoints() As TPlnPoint
		Dim iPointsLen As Integer
		Select Case iMarkType
			Case MarkBlock.enMarkBlockType.Square
				taPoints = MarkBlock.GetLinePoints(iMarkType)
				iPointsLen = taPoints.Length
				For iIndex As Integer = 0 To iPointsLen - 1
					taPoints(iIndex).Scale(dScale)
					taPoints(iIndex).Move(tPoint)

				Next

				For iIndex As Integer = 0 To iPointsLen - 1
					Try
						moEditor.DrawVector(taPoints(iIndex).GetPoint3d(), taPoints((iIndex + 1) Mod iPointsLen).GetPoint3d(), iColor, bHighlighted)
					Catch ex As Exception

					End Try

				Next
		End Select
	End Sub
	Public Shared Function GetCurrentDWGName() As String
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document = Application.DocumentManager.GetDocument(oCurrentDatabase)
		Return oAcadDocument.Name
	End Function
	Public Shared Function GetCurrentDirectory() As String
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document = Application.DocumentManager.GetDocument(oCurrentDatabase)
		Dim oFile As IO.FileInfo = New IO.FileInfo(oAcadDocument.Name)
		Return oFile.DirectoryName
	End Function
	Public Shared Function GetFileName() As String
		Dim oActiveDocument As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		If oActiveDocument IsNot Nothing Then
			Return oActiveDocument.Name
		Else
			Return Nothing
		End If
	End Function
	'Public Shared Sub InitDebug()
	'   If System.Environment.UserName.ToUpper = "BORIS" Then
	'      mbDebug = mbDebugBoris
	'   End If
	'End Sub


	Public Shared Function GetDWGScaleFactor() As Double
		Try
			Dim dDWGScale As System.Double = Convert.ToDouble(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msScaleSysVarName))
			If dDWGScale < 1.0 Then   ' Double.Epsilon
				dDWGScale = 1000.0
			End If
			Return dDWGScale / DMAcadExt.DMApp.GetDefaultRepScale()

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetDWGScaleFactor")
		End Try
	End Function
	Public Shared Function GetDWGScaleText() As String
		Try
			Dim dDWGScale As System.Double = Convert.ToDouble(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(msScaleSysVarName))
			Dim idDWGScale As System.Int32 = 25 * CInt(dDWGScale / 25.0)

			Return "1:" & idDWGScale.ToString()
		Catch oEx As Exception
			Return String.Empty
		End Try
	End Function
	Public Shared Function GetDWGScale() As Autodesk.AutoCAD.Geometry.Scale3d
		Try
			Return New Autodesk.AutoCAD.Geometry.Scale3d(GetDWGScaleFactor())
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetDWGScale")
		End Try
	End Function
	Public Shared Sub OpenLog(bAppend As Boolean)
		'	moEditor.Document
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		'	Dim oAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document = Application.DocumentManager.GetDocument(oCurrentDatabase)
		'	Dim sAcadDocumentName As String = oAcadDocument.Name
		Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(GetCurrentDWGName())
		Dim sAdd As String = String.Empty

		For iIndex As Integer = 1 To 10
			msLogName = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length) & sAdd & ".log"
			'  System.Windows.Forms.MessageBox.Show(msLogName, "21_573a")
			Try
				moStreamWriter = New IO.StreamWriter(msLogName, bAppend, System.Text.Encoding.Default)
				mbLogOpened = True
				Exit For
			Catch oEx As Exception
				'System.Windows.Forms.MessageBox.Show(msLogName, "21_573")
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & msLogName, "AcadDocument - OpenLog")
				sAdd = "_" & CStr(iIndex)
			End Try

		Next
	End Sub
	Public Shared Sub SetLogName()

		Dim oFileInfo As System.IO.FileInfo = New System.IO.FileInfo(GetCurrentDWGName())
		msLogName = oFileInfo.FullName.Substring(0, oFileInfo.FullName.Length - oFileInfo.Extension.Length) & ".log"

	End Sub

	Public Shared Property LogName() As String
		Get
			Return msLogName
		End Get
		Set(ByVal sValue As String)
			msLogName = sValue
		End Set
	End Property

	Public Shared Sub Reset()
		moDocumentCollection = Application.DocumentManager
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
	End Sub

	Public Shared Sub CloseLog()
		Try
			If mbLogOpened AndAlso moStreamWriter IsNot Nothing Then
				mbLogOpened = False
				moStreamWriter.Close()
			End If

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - CloseLog")
		End Try

	End Sub
	Public Shared Sub SetView(ByVal tCenterPoint As Point2d, ByVal dWidth As Double, ByVal dHeight As Double)
		Try
			Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
			oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
			moEditor = Application.DocumentManager.MdiActiveDocument.Editor
			With oViewTableRecord
				.CenterPoint = tCenterPoint
				.Width = dWidth
				.Height = dHeight
			End With
			moEditor.SetCurrentView(oViewTableRecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - SetView")
		End Try
	End Sub
	Public Shared Function GetExtMaxPoint() As Point2d
		Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("EXTMAX")
		If oValue Is Nothing Then
			Return Nothing
		Else
			Try
				Dim tPoint3d As Point3d = DirectCast(oValue, Point3d)
				Return TPlnPoint.Point3dTo2d(tPoint3d)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - GetExtMaxPoint")
			End Try
		End If
	End Function
	Public Shared Function GetExtMinPoint() As Point2d
		Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("EXTMIN")
		If oValue Is Nothing Then
			Return Nothing
		Else
			Try
				Dim tPoint3d As Point3d = DirectCast(oValue, Point3d)
				Return TPlnPoint.Point3dTo2d(tPoint3d)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - GetExtMinPoint")
			End Try
		End If
	End Function
	Public Shared Function GetExtMinPoint3d(bFloor As Boolean) As Point3d
		Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("EXTMIN")
		If oValue Is Nothing Then
			Return Nothing
		Else
			Try
				Dim tPoint3d As Point3d = DirectCast(oValue, Point3d)
				If bFloor Then
					tPoint3d = New Point3d(tPoint3d.X, tPoint3d.Y, tPoint3d.Z)
				End If
				Return tPoint3d
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - GetExtMinPoint3d")
			End Try
		End If
	End Function
	Public Shared Function GetCenterPoint() As Point2d
		Dim oValue As System.Object = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("EXTMIN")
		If oValue Is Nothing Then
			Return Point2d.Origin
		Else
			Try
				Dim tMinPoint3d As Point3d = DirectCast(oValue, Point3d)
				oValue = Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("EXTMAX")
				If oValue Is Nothing Then
					Return Point2d.Origin
				Else
					Dim tMaxPoint3d As Point3d = DirectCast(oValue, Point3d)
					Dim oPoint As TPlnPoint = New TPlnPoint(tMinPoint3d, tMaxPoint3d)
					Return oPoint.AcGePoint

				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - GetExtMinPoint")
				Return Point2d.Origin
			End Try
		End If
	End Function

	Public Shared Function GetMinPoint() As Point2d
		'	moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim sTest As String
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		tAcObjID = moEditor.CurrentViewportObjectId  'ActiveViewportId
		sTest = tAcObjID.ToString()
		tAcObjID = moEditor.ActiveViewportId
		sTest &= ":" & tAcObjID.ToString()
		'		System.Windows.Forms.MessageBox.Show(sTest, "tObjID Current:Active ViewportObjectId ")

		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = AcadTransaction.GetDBObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
		'	Dim oViewport As Autodesk.AutoCAD.DatabaseServices.Viewport
		Dim oViewportRec As Autodesk.AutoCAD.DatabaseServices.ViewportTableRecord
		Dim tPoint As Point2d
		Dim dMinX, dMinY As Double
		'Dim dMaxX, dMaxY As Double


		If oDBObject IsNot Nothing Then
			oViewportRec = DirectCast(oDBObject, Autodesk.AutoCAD.DatabaseServices.ViewportTableRecord)

			tPoint = oViewportRec.CenterPoint()
			dMinX = tPoint.X - 0.5 * oViewportRec.Width
			dMinY = tPoint.Y - 0.5 * oViewportRec.Height

			'	dMaxX = tPoint.X + 0.5 * oViewportRec.Width
			'	dMaxY = tPoint.Y + 0.5 * oViewportRec.Height

			WriteMessage("Min=" & CStr(dMinX) & "," & CStr(dMinY) & vbCrLf)
			'	moEditor.WriteMessage(CStr(dMaxX) & "," & CStr(dMaxY) & vbCrLf)
			'		WriteMessage("Viewport=" & CStr(oViewportRec.Width) & "," & CStr(oViewportRec.Height) & vbCrLf)
			Return New Point2d(dMinX, dMinY)

		End If
	End Function
	Public Shared Sub TestAcadDoc(sCaption As String)
		Dim s As String
		Dim oDoc As Autodesk.AutoCAD.ApplicationServices.Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		s = oDoc.IsActive.ToString() & vbCrLf & oDoc.IsReadOnly.ToString() & vbCrLf & oDoc.LockMode.ToString() & vbCrLf & oDoc.LockMode(False).ToString() & vbCrLf & oDoc.LockMode(True).ToString()
		System.Windows.Forms.MessageBox.Show(s, sCaption)

	End Sub
	Public Shared Sub WriteException(ByVal iErrClassNo As Integer, ByVal iExceptionNo As Integer, ByVal sMsg As String, ByVal ParamArray oParams() As System.Object)
		sMsg = "Ex #" & CStr(iErrClassNo + iExceptionNo) & ":" & sMsg
		WriteMessage(sMsg, oParams)
	End Sub
	Public Shared Sub WriteSingleMessage(ByVal sMsg As String)
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		moEditor.WriteMessage(sMsg)
	End Sub
	Public Shared Sub WriteDebugMessage(ByVal sMsg As String, ByVal ParamArray oParams() As System.Object)
		If DMCommon.Debug.Debug Then
			WriteMessage(sMsg, oParams)
		End If
	End Sub
	Public Shared Sub WriteDebugMessageN(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)
		If DMCommon.Debug.Debug Then
			WriteMessage(sCaption & ": " & DMCommon.Debug.GetMsg(False, oParams))
		End If
	End Sub
	Public Shared Sub WriteUserMessage(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)

		WriteMessage(sCaption & ": " & DMCommon.Debug.GetMsg(False, oParams))

	End Sub
	Public Shared Sub WriteLog(ByVal sMsg As String)
		Try
			If Not mbLogOpened Then
				moStreamWriter = New IO.StreamWriter(msLogName, True, System.Text.Encoding.Default)
			End If

			moStreamWriter.WriteLine(sMsg)
			If Not mbLogOpened AndAlso moStreamWriter IsNot Nothing Then
				moStreamWriter.Close()
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(msLogName, "21_555")
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - WriteLog")
		End Try
	End Sub
	Public Shared Sub WriteLog(ByVal sMsg As String, ByVal bCounter As Boolean, iIncrement As Integer)

		If Application.DocumentManager.MdiActiveDocument IsNot Nothing AndAlso Not String.IsNullOrEmpty(msLogName) Then

			If mbMessageSended Then
				'sMsg = vbCrLf & sMsg
				moEditor.WriteMessage(vbCrLf)
			End If
			Try
				If sMsg IsNot Nothing Then
					If Not mbLogOpened Then
						moStreamWriter = New IO.StreamWriter(msLogName, True, System.Text.Encoding.Default)
					End If
					If moStreamWriter IsNot Nothing Then
						If iIncrement <> 0 Then
							miCounter += iIncrement
						End If

						If bCounter Then
							sMsg = zzGetCounterPrefix() & sMsg
						End If
						moStreamWriter.WriteLine(sMsg)
					End If

				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & DMCommon.Functions.CStrN(msLogName, "Nothing"), "AcadDocument - 1WriteMessageLog_1")
			End Try

			Try
				If Not mbLogOpened AndAlso moStreamWriter IsNot Nothing Then
					moStreamWriter.Close()
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadDocument - !WriteMessageLog")
			End Try
		End If
	End Sub
	Public Shared Sub WriteMessageLog(ByVal sMsg As String, ByVal ParamArray oParams() As System.Object)

		If Application.DocumentManager.MdiActiveDocument IsNot Nothing Then


			moEditor = Application.DocumentManager.MdiActiveDocument.Editor
			If mbMessageSended Then
				'sMsg = vbCrLf & sMsg
				moEditor.WriteMessage(vbCrLf)
			End If
			Try
				If Not String.IsNullOrEmpty(msLogName) AndAlso sMsg IsNot Nothing Then
					If Not mbLogOpened Then
						moStreamWriter = New IO.StreamWriter(msLogName, True, System.Text.Encoding.Default)
					End If
					If moStreamWriter IsNot Nothing Then
						moStreamWriter.WriteLine(sMsg)
					End If

				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & DMCommon.Functions.CStrN(msLogName, "nothing"), "AcadDocument - 1:WriteMessageLog_1")
			End Try
			Try
				moEditor.WriteMessage(sMsg, oParams)
				mbMessageSended = True
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sMsg, "AcadDocument - 2WriteMessagelog_2")
			End Try
			Try
				If Not mbLogOpened AndAlso moStreamWriter IsNot Nothing Then
					moStreamWriter.Close()
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadDocument - !WriteMessageLog")
			End Try
		End If
	End Sub
	Public Shared Sub WriteMessage(ByVal sMsg As String, ByVal ParamArray oaParams() As System.Object)
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		If mbMessageSended Then
			sMsg = vbCrLf & sMsg
		End If

		Try
			If mbLogOpened Then
				'		moStreamWriter.WriteLine(sMsg)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - WriteMessage_1")
		End Try
		Try
			moEditor.WriteMessage(sMsg, oaParams)
			mbMessageSended = True
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sMsg, "AcadDocument - WriteMessage_2")
		End Try

	End Sub
	Public Shared Sub CloseMessage()
		If mbMessageSended Then
			CommandLine(True)
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
	Public Shared Sub AddToCounter(iStep As Integer)
		miCounter += iStep
	End Sub
	Public Shared Sub ResetCounter()

		miCounter = 0
	End Sub
	Public Shared Property Counter As Integer
		Get
			Return miCounter
		End Get
		Set(iValue As Integer)
			miCounter = iValue
		End Set
	End Property


	Public Shared Function GetPoint(ByVal sPrompt As String, ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d) As Boolean
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Dim oPromptPointOptions As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(vbCrLf & sPrompt)
		''  ptopts.BasePoint = New Autodesk.AutoCAD.Geometry.Point3d(1, 1, 1)
		oPromptPointOptions.UseBasePoint = False
		oPromptPointOptions.UseDashedLine = True
		oPromptPointOptions.AllowArbitraryInput = False

		Dim oPromptPointResult As Autodesk.AutoCAD.EditorInput.PromptPointResult = oEditor.GetPoint(oPromptPointOptions)


		Dim oaOutput(1) As Autodesk.AutoCAD.Geometry.Point3d
		If oPromptPointResult.Status <> Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			tPoint = oPromptPointResult.Value
			Return True
		Else
			Return False
		End If

	End Function

	Public Shared Function GetTopoErrBlockScale() As Double
		'Return GetCurrentView().Height * 0.025
		Return GetCurrentView().Height * 0.01

	End Function
	Public Shared Function GetCurrentView() As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		Try
			Return moEditor.GetCurrentView()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadDocument - GetCurrentView")
			Return Nothing
		End Try

	End Function
	Public Shared Sub SetCurrentView(ByVal oView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord)
		moEditor = Application.DocumentManager.MdiActiveDocument.Editor
		moEditor.SetCurrentView(oView)
	End Sub
	Public Shared Function GetCurrentViewDim() As Double
		Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = GetCurrentView()
		If oViewTableRecord IsNot Nothing Then
			With oViewTableRecord
				Return (Math.Min(.Height, .Width))
			End With
		End If
	End Function
	Public Shared Function GetColor() As Autodesk.AutoCAD.Colors.Color
		Dim oColorDialog As Autodesk.AutoCAD.Windows.ColorDialog = New Autodesk.AutoCAD.Windows.ColorDialog()
		Dim iResult As System.Windows.Forms.DialogResult = oColorDialog.ShowDialog()
		If iResult = Windows.Forms.DialogResult.OK Then
			Return oColorDialog.Color
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetLayerTransparency() As UInt32
		Dim oLayerTransparencyDialog As Autodesk.AutoCAD.Windows.LayerTransparencyDialog = New Autodesk.AutoCAD.Windows.LayerTransparencyDialog
		Dim iResult As System.Windows.Forms.DialogResult = oLayerTransparencyDialog.ShowDialog()
		If iResult = Windows.Forms.DialogResult.OK Then
			Return oLayerTransparencyDialog.Percent
		Else
			Return Nothing
		End If
	End Function


	Public Shared Function OpenDocument(ByVal sFileName As String, ByVal bReadOnly As Boolean, Optional ByVal bActivate As Boolean = False) As Autodesk.AutoCAD.ApplicationServices.Document
		Try
			Dim oDocument As Autodesk.AutoCAD.ApplicationServices.Document

			moDocumentCollection = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager
			oDocument = moDocumentCollection.Open(sFileName, bReadOnly)

			'     System.Windows.Forms.MessageBox.Show(sFileName & vbCrLf & bReadOnly.ToString() & vbCrLf & bActivate.ToString(), "AcadDocument - OpenDocument")
			If bActivate Then
				If moDocumentCollection.MdiActiveDocument.Name <> oDocument.Name Then
					'  System.Windows.Forms.MessageBox.Show(moDocumentCollection.MdiActiveDocument.Name & vbCrLf & oDocument.Name, "05_801")
					moDocumentCollection.MdiActiveDocument = oDocument
					'  System.Windows.Forms.MessageBox.Show(moDocumentCollection.MdiActiveDocument.Name & vbCrLf & oDocument.Name, "05_802")
				End If

				System.Windows.Forms.MessageBox.Show(moDocumentCollection.MdiActiveDocument.Name & vbCrLf & bReadOnly.ToString(), "MdiActiveDocument = oDocument")
			End If
			Return oDocument
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - OpenDocument")
			Return Nothing
		End Try

		'	Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument = moDocumentCollection.Open(sFileName, bReadOnly)

		'	DMAcadExt.AcadDocument.Reset()
		'	System.Windows.Forms.MessageBox.Show(sFileName, "108 ")
		''''''''''''''moEditor = Application.DocumentManager.MdiActiveDocument.Editor
	End Function
	Public Shared Sub CloseAndDiscardActiveDocument()
		Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CloseAndDiscard()
	End Sub
	Public Shared Function CloseAndSaveActiveDocument(Optional ByVal sAsFileName As String = "") As Boolean
		Try
			If String.IsNullOrEmpty(sAsFileName) Then
				sAsFileName = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Name

			End If
			Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CloseAndSave(sAsFileName)
			Return True
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - CloseAndSaveActiveDocument")
			Return False
		End Try

	End Function
	Public Shared Sub SendCommand(sCommand As String)
		Dim oActiveDocument As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		Dim oAcadDoc As System.Object = oActiveDocument.GetAcadDocument()
		Dim oData() As System.Object = {sCommand & vbCrLf}
		oAcadDoc.GetType().InvokeMember("SendCommand", System.Reflection.BindingFlags.InvokeMethod, Nothing, oAcadDoc, oData)
	End Sub
	Public Shared Sub DocLock(ByVal iMode As DocumentLockMode, ByVal bCommandLine As Boolean)
		Dim sCommandName As String
		If bCommandLine Then
			sCommandName = "CommandLine"
		Else
			sCommandName = String.Empty
		End If
		'   System.Windows.Forms.MessageBox.Show(CStr(moDocLock Is Nothing) & vbCrLf & bMode.ToString(), "DocLock 12_411")
		If moDocLock IsNot Nothing Then
			System.Windows.Forms.MessageBox.Show(moDocLock.ToString(), "21_100 DocLock!!!!!")
		Else
			Dim oActiveDocument As Document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument

			If oActiveDocument IsNot Nothing AndAlso oActiveDocument.LockMode(False) <> DocumentLockMode.NotLocked Then
				System.Windows.Forms.MessageBox.Show("Status: " & oActiveDocument.LockMode(False).ToString() & vbCrLf & "Command in progress: " & oActiveDocument.CommandInProgress, "21_200 DocLock")
			Else
				Try
					moDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(iMode, sCommandName, String.Empty, True)
					miDocumentLockMode = iMode
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					moDocLock.Dispose()
					moDocLock = Nothing
					miDocumentLockMode = DocumentLockMode.None
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & sCommandName & vbCrLf & Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode.ToString(), "AcadDocument - DocLock")
				End Try
			End If
		End If
	End Sub

	Public Shared ReadOnly Property DocumentLockMode() As DocumentLockMode
		Get
			Return miDocumentLockMode
		End Get
	End Property


	Public Shared Sub SetActiveDoc(ByVal iMode As DocumentLockMode, ByVal bCommandLine As Boolean)
		Dim sCommandName As String
		If bCommandLine Then
			sCommandName = "CommandLine"
		Else
			sCommandName = String.Empty
		End If
		'   System.Windows.Forms.MessageBox.Show(CStr(moDocLock Is Nothing) & vbCrLf & bMode.ToString(), "DocLock 12_411")
		If moDocLock IsNot Nothing Then
			System.Windows.Forms.MessageBox.Show(moDocLock.ToString(), "21_209 DocLock!!!!!")
		Else
			moActiveDrawing = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument

			If moActiveDrawing IsNot Nothing AndAlso moActiveDrawing.LockMode(False) <> DocumentLockMode.NotLocked Then
				System.Windows.Forms.MessageBox.Show("Status: " & moActiveDrawing.LockMode(False).ToString() & vbCrLf & "Command in progress: " & moActiveDrawing.CommandInProgress, "21_200 DocLock")
			Else
				Try
					moDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(iMode, sCommandName, String.Empty, True)
					miDocumentLockMode = iMode
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					moDocLock.Dispose()
					moDocLock = Nothing
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & sCommandName & vbCrLf & Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode.ToString(), "AcadDocument - DocLock")
				End Try
			End If
		End If
	End Sub
	Public Shared Sub SetOneTimeActiveDoc(sCommandName As String, iCommandNumber As Integer, oAfterCommand As AfterCommandProc)

		moActiveDrawing = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		msOneTimeCommandName = sCommandName
		miOneTimeCommandNumber = iCommandNumber

		miCommandCounter = 0
		moOneTimeAfterComProc = oAfterCommand

	End Sub
	Public Shared Sub SetDrawVectorSet(oVectorSet As DrawVectorSet.VectorSet, iIndex As Integer)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		Return
		If moActiveDrawing Is Nothing Then
			moActiveDrawing = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		End If
		If moDrawVectorSet Is Nothing Then
			moDrawVectorSet = New DrawVectorSet(oVectorSet)

		Else
			moDrawVectorSet.Replace(oVectorSet, iIndex)
			moDrawVectorSet.ViewChanged = False
			Regen()
		End If

		' oEditor.WriteMessage("A347")
		moDrawVectorSet.Draw("SetDraw")
	End Sub
	Public Shared Sub SetDrawPoint(oPoint As TPlnPoint)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor

		If moActiveDrawing Is Nothing Then
			moActiveDrawing = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument
		End If
		If moDrawVectorSet Is Nothing Then
			moDrawVectorSet = New DrawVectorSet(oPoint)

		Else
			moDrawVectorSet.AddPoint(oPoint)
			moDrawVectorSet.ViewChanged = False
			Regen()
		End If

		' oEditor.WriteMessage("A347")
		moDrawVectorSet.Draw("SetDraw")
	End Sub
	Public Shared Sub SetDrawVectorSetActive(bActive As Boolean)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor


		If moDrawVectorSet IsNot Nothing Then
			moDrawVectorSet.Active = bActive
			moDrawVectorSet.Draw("SetDraw")
		End If

		' oEditor.WriteMessage("A347")

	End Sub

	Public Shared Sub DrawVectorSetInfo()
		If moDrawVectorSet Is Nothing Then
			DMCommon.Debug.MsgBox("12_204b", "moDrawVectorSet Is Nothing ")
		Else
			moDrawVectorSet.InfoMsg()
		End If

	End Sub
	Public Shared Sub ClearDrawVectorSet()
		If moDrawVectorSet IsNot Nothing Then
			moDrawVectorSet.Active = False
			moDrawVectorSet.Clear()

			moDrawVectorSet = Nothing
			DMAcadExt.AcadDocument.UpdateScreen()
			Regen()
		End If
	End Sub

	Public Shared Function GetDrawVectorSetUB() As Integer
		If moDrawVectorSet IsNot Nothing Then
			Return moDrawVectorSet.VectorSetUB
		Else
			Return -1
		End If
	End Function
	Public Shared Function GetDrawVectorSetActive() As Boolean
		If moDrawVectorSet IsNot Nothing Then
			Return moDrawVectorSet.Active
		Else
			Return False
		End If
	End Function
	Public Shared Sub Unlock()
		'System.Windows.Forms.MessageBox.Show(CStr(moDocLock Is Nothing), "Unlock 12_412")
		If moDocLock IsNot Nothing Then
			Try
				moDocLock.Dispose()
			Catch oMapEx As Autodesk.Gis.Map.MapException
				AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "AcadDocument - Unlock")
			Finally
				moDocLock = Nothing
				miDocumentLockMode = DocumentLockMode.None
			End Try
         If Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode = DocumentLockMode.Read Then
            System.Windows.Forms.MessageBox.Show("Locked: " & Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockMode.ToString(), "AcadDoc - Unlock")
         End If
      End If
   End Sub
   Public Shared Function IsLocked() As Boolean
      Return (moDocLock IsNot Nothing)
   End Function
   Public Shared Sub RegenDoc()
      Try
			Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
			Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("Regen ", True, False, False)
		Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadDocument - RegenDoc")
      End Try
   End Sub
   Public Shared Sub Regen()
      Try
         moEditor = Application.DocumentManager.MdiActiveDocument.Editor
         moEditor.Regen()
         CommandLine(True)
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadDocument - Regen")
      End Try
   End Sub
   Public Shared Sub Zoom(ByVal oBoundingBox As DMAcadExt.TPlnBoundingBox)
      Dim sTest As String = "a"
      Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
      Dim oPoint As DMAcadExt.TPlnPoint
      Try
         moEditor = Application.DocumentManager.MdiActiveDocument.Editor
         sTest = "b"
         oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
         sTest = "c"
         oPoint = oBoundingBox.GetCenterPoint()
         sTest = "d"
         If oPoint IsNot Nothing Then
            sTest = "e"
            oViewTableRecord.CenterPoint = oPoint.AcGePoint
            sTest = "f"
            oViewTableRecord.Width = oBoundingBox.Width
            sTest = "g"
            oViewTableRecord.Height = oBoundingBox.Height
            sTest = "h"
            moEditor.SetCurrentView(oViewTableRecord)
            sTest = "i"
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "Zoom:AcadDocument")
      End Try
   End Sub
   Public Shared Sub Zoom(ByVal dXmin As Double, dYmin As Double, dXmax As Double, dYmax As Double)
      Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(New TPlnPoint(dXmin, dYmin), New TPlnPoint(dXmax, dYmax))
      Zoom(oBoundingBox)
   End Sub
   Public Shared Sub Zoom(ByVal oPoint As DMAcadExt.TPlnPoint, ByVal dRadius As Double)
      Dim sTest As String = "a"
      Dim oViewTableRecord As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord
      Try
         moEditor = Application.DocumentManager.MdiActiveDocument.Editor
         sTest = "b"
         oViewTableRecord = New Autodesk.AutoCAD.DatabaseServices.ViewTableRecord()
         sTest = "c"

         sTest = "d"
         If oPoint IsNot Nothing Then
            sTest = "e"
            oViewTableRecord.CenterPoint = oPoint.AcGePoint
            sTest = "f"
            oViewTableRecord.Width = dRadius
            sTest = "g"
            oViewTableRecord.Height = dRadius
            sTest = "h"
            moEditor.SetCurrentView(oViewTableRecord)
            DrawShape(MarkBlock.enMarkBlockType.Square, dRadius * 0.1, oPoint, 1, True)
            sTest = "i"
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbNewLine & sTest, "Zoom2:AcadDocument")
      End Try
   End Sub
   Public Shared Sub Zoom(ByVal tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, ByVal dScale As Double)
      Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity = AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
      Dim tExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d = oEntity.GeometricExtents
      Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(tExtents)
      oBoundingBox.Scale(dScale)
      Zoom(oBoundingBox)
   End Sub
   Public Shared Sub UpdateScreen()
      moEditor = Application.DocumentManager.MdiActiveDocument.Editor
      moEditor.UpdateScreen()
   End Sub
   Public Shared Sub CommandLine(ByVal bActivate As Boolean)
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("CommandLine ", bActivate, False, False)
   End Sub
   Public Shared Sub SendExec(sCommand As String, ByVal bActivate As Boolean)
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute(sCommand & vbCr, bActivate, False, True)
   End Sub
   Public Shared Sub SendRegenAll()
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("RegenAll" & vbCrLf, True, False, True)
   End Sub
   Public Shared Sub PrintList(taAcObjID() As Autodesk.AutoCAD.DatabaseServices.ObjectId)
      Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjID)
      oEditor.SetImpliedSelection(oSelSet)
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.SendStringToExecute("List" & vbCrLf, True, False, False)
   End Sub

   Private Shared Function zzGetCounterPrefix() As String
      Return Convert.ToString(miCounter) & ":"

   End Function

	Private Shared Sub moActiveDrawing_CommandCancelled(oSender As System.Object, e As CommandEventArgs) Handles moActiveDrawing.CommandCancelled
		'   DMCommon.Debug.MsgBox("01_550a", e.GlobalCommandName)
	End Sub



	Private Shared Sub moActiveDrawing_CommandEnded(oSender As System.Object, e As CommandEventArgs) Handles moActiveDrawing.CommandEnded


		If moDrawVectorSet IsNot Nothing Then
			moDrawVectorSet.Draw("CommandEnded")
		End If


		If msOneTimeCommandName IsNot Nothing AndAlso e.GlobalCommandName = msOneTimeCommandName Then
			miCommandCounter += 1
			'   DMCommon.Debug.MsgBox("01_533d", e.GlobalCommandName, msOneTimeCommandName, miCommandCounter)
			If miCommandCounter = miOneTimeCommandNumber Then
				moOneTimeAfterComProc(False)
				msOneTimeCommandName = Nothing
			End If
		End If
		' WriteMessage("CommandM: " & e.GlobalCommandName & "; N=" & miCommandCounter.ToString())
	End Sub



	Private Shared Sub moActiveDrawing_ViewChanged(oSender As System.Object, e As EventArgs) Handles moActiveDrawing.ViewChanged
      If moDrawVectorSet IsNot Nothing Then
         moDrawVectorSet.ViewChanged = True
         ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''  moDrawVectorSet.Draw()
      Else
         '   DMCommon.Debug.MsgBox("11_100")
      End If
      '   WriteMessage("ViewChanged")
   End Sub
End Class

