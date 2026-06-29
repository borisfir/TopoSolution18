Option Explicit On
Option Strict On
Imports System.Data
Public Enum PaintException
	Undefined
	OK
	BadInput
	FindPgonError
   PolygonExtentsNotExists
   CreatingLayerFailed
	CreatingPolygonTopologyFailed
	SourcePgonTopologyForBufferNotExists
	CreatingPgonTopologyForBufferFailed
	CreatingBufferFailed
	CreatingPgonTopologyForZebraFailed
	CreatingIntersectTopologyForZebraFailed
	CreatingTopologyByBufferFailed
	CreatingZebraBoxFailed
	ExternalLoopIsNothing
	AddHatchError
	HatchBackColorIsEmpty
	PolygonError
	ComplexError
End Enum
Public Class AppMessages
	Private Const msParselSysKey As String = "Parcel"
	Private Const msIntersectSysKey As String = "DVA"

	Private Shared mdZoomRadius As Double = 100.0
	Public Structure Action
		Dim ActionID As Integer
		Dim ActionName As String
		Dim ActionDescription As String
		Dim MapThemeID As enMapTheme
		Public Sub New(iActionID As Integer, sActionName As String, sActionDescription As String)
			ActionID = iActionID
			ActionName = sActionName
			ActionDescription = sActionDescription
		End Sub


	End Structure
	Private Shared mdicActions As IDictionary(Of Integer, Action)
	Private Shared mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
	Private Shared moMsgTable As DataTable
	Private Shared mhsMapThemes As HashSet(Of DMAcadExt.MapThemeData) = New HashSet(Of MapThemeData)
	Private Shared mcolActions As ObjectModel.Collection(Of Action) = New ObjectModel.Collection(Of Action)()
	Public Shared ReadOnly Property MsgTable() As DataTable
		Get
			zzCreateMsgTable()
			Return moMsgTable
		End Get
	End Property
	Public Shared Property MapThemes() As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
		Get
			Return mdicMapThemes
		End Get
		Set(bValue As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData))
			mdicMapThemes = bValue
		End Set
	End Property
	Public Shared ReadOnly Property Actions() As ObjectModel.Collection(Of Action)
		Get
			Return mcolActions
		End Get
	End Property
	Public Shared ReadOnly Property ActionDictionary() As IDictionary(Of Integer, Action)
		Get
			Return mdicActions
		End Get
	End Property
	Public Shared Sub Zoom(ByVal oDataRow As DataRow)
		Dim bPointExists As Boolean
		Dim bExtentsExist As Boolean

		Dim oPoint As TPlnPoint
		Dim oBoundingBox As TPlnBoundingBox


		Dim dX, dY As Double
		Dim dXmin, dYmin As Double
		Dim dXmax, dYmax As Double



		bPointExists = DirectCast(oDataRow.Item("CoordinatesExist"), Boolean)
		bExtentsExist = DirectCast(oDataRow.Item("ExtentsExist"), Boolean)
		If bExtentsExist Then
			dXmin = DMCommon.Functions.CDblN(oDataRow.Item("Xmin"))
			dYmin = DMCommon.Functions.CDblN(oDataRow.Item("Ymin"))
			dXmax = DMCommon.Functions.CDblN(oDataRow.Item("Xmax"))
			dYmax = DMCommon.Functions.CDblN(oDataRow.Item("Ymax"))
			oBoundingBox = New TPlnBoundingBox(New TPlnPoint(dXmin, dYmin), New TPlnPoint(dXmax, dYmax))
			AcadDocument.Zoom(oBoundingBox)
		ElseIf bPointExists Then
			dX = DMCommon.Functions.CDblN(oDataRow.Item("X"))
			dY = DMCommon.Functions.CDblN(oDataRow.Item("Y"))
			oPoint = New TPlnPoint(dX, dY)
			AcadDocument.Zoom(oPoint, mdZoomRadius * DMAcadExt.AcadDocument.GetDWGScaleFactor)
		End If


	End Sub
	Public Shared Function GetPaintResp(iResp As PaintException, iLocRespB As PaintException) As PaintException
		If iResp = PaintException.Undefined OrElse iResp = PaintException.OK Then
			Return iLocRespB
		ElseIf iLocRespB = PaintException.OK Then
			If iResp = PaintException.Undefined Then
				Return PaintException.OK
			Else
				Return iResp
			End If
		Else
			Return PaintException.ComplexError
		End If
	End Function
	Private Shared Sub zzCreateMsgTable()
		If moMsgTable Is Nothing Then
			moMsgTable = New DataTable("MsgTable")
			With moMsgTable.Columns
				.Add("CoordinatesExist", GetType(System.Boolean))
				.Add("ExtentsExist", GetType(System.Boolean))
				.Add("X", GetType(System.Double))
				.Add("Y", GetType(System.Double))

				.Add("Xmin", GetType(System.Double))
				.Add("Ymin", GetType(System.Double))
				.Add("Xmax", GetType(System.Double))
				.Add("Ymax", GetType(System.Double))
				.Add("SysID", System.String.Empty.GetType())
				.Add("MapThemeName", GetType(System.String))
				.Add("MapThemeID", GetType(System.Int32))
				.Add("ActionName", GetType(System.String))
				.Add("ActionID", GetType(System.Int32))

				.Add("Main", GetType(System.Boolean))
				.Add("Err", GetType(System.Boolean))


				.Add("Text", System.String.Empty.GetType())
			End With

		End If
	End Sub
	Private Shared Sub zzInit()
		If mdicActions Is Nothing Then

			Dim sComText As String = "SELECT TOP (100) PERCENT ActionID,ActionName,ActionDescription FROM dbo.CheckActionTypes"
			'      sSelectComText = "SELECT TopologyType,Step,ActionNo,ActionID,Tolerance FROM CleanupActions WHERE (CleanupActions.TopologyType=" & Convert.ToString(ptMapThemeData.CleanupType) & ") ORDER BY CleanupActions.Step,CleanupActions.ActionNo"

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)

			Dim iActionID As Integer, sActionName As String, sActionDescription As String

			If oDataReader IsNot Nothing Then
				mdicActions = New Dictionary(Of Integer, Action)
				Do While oDataReader.Read
					iActionID = oDataReader.GetInt32(0)
					sActionName = oDataReader.GetString(1)
					sActionDescription = oDataReader.GetString(2)
					mdicActions.Add(iActionID, New Action(iActionID, sActionName, sActionDescription))


				Loop




				oDataReader.Close()
			End If




		End If
	End Sub
	Public Shared Sub AddMessage(ByVal bCoordinatesExist As Boolean, ByVal dX As Double, ByVal dY As Double, ByVal sSysID As String, ByVal sText As String, ByVal bAcadTextWin As Boolean, Optional iMapThemeID As DMAcadExt.enMapTheme = enMapTheme.Undefined, Optional iActionID As Integer = 0, Optional bMain As Boolean = True, Optional bErr As Boolean = True)
		zzCreateMsgTable()
		zzInit()
		Dim oNewRow As DataRow = moMsgTable.NewRow
		Dim tAction As Action = New Action()
		Dim tMapThemeData As DMAcadExt.MapThemeData = New MapThemeData()
		Dim sThemeName As String = Nothing
		With oNewRow
			.Item("CoordinatesExist") = bCoordinatesExist
			.Item("ExtentsExist") = False
			If bCoordinatesExist Then
				.Item("X") = Math.Round(dX, 2)
				.Item("Y") = Math.Round(dY, 2)
			End If
			.Item("SysID") = sSysID
			.Item("Text") = sText
			.Item("ActionID") = iActionID
			If mdicActions.TryGetValue(iActionID, tAction) Then
				.Item("ActionName") = tAction.ActionName
				tAction.MapThemeID = iMapThemeID
				mcolActions.Add(tAction)
			End If
			If mdicMapThemes.TryGetValue(iMapThemeID, tMapThemeData) Then
				.Item("MapThemeName") = tMapThemeData.MapThemeName
			End If

			.Item("MapThemeID") = iMapThemeID


			.Item("Main") = bMain
			.Item("Err") = bErr
		End With
		moMsgTable.Rows.Add(oNewRow)
		If Not mhsMapThemes.Contains(tMapThemeData) Then
			mhsMapThemes.Add(tMapThemeData)
		End If
		If bAcadTextWin Then
			AcadDocument.WriteMessage(sText)
		End If
		'	DMCommon.Debug.MsgBox("13_134f", iMapThemeID, iActionID, moMsgTable.Rows.Count)
	End Sub
	Public Shared Function GetMessagesView(iMapThemeID As DMAcadExt.enMapTheme, Optional iActionID As Integer = 0) As DataView

		If (moMsgTable IsNot Nothing) AndAlso iMapThemeID <> 0 Then
			Dim sFilter As String
			If iActionID = 0 Then
				sFilter = "(MapThemeID=" & Convert.ToString(iMapThemeID) & ") AND (" & "Main" & ")"
			Else
				sFilter = "(MapThemeID=" & Convert.ToString(iMapThemeID) & ") AND (" & "ActionID=" & Convert.ToString(iActionID) & ") AND (" & "Main" & ")"
			End If

			Return New DataView(moMsgTable, sFilter, String.Empty, DataViewRowState.CurrentRows)

			'   DMCommon.Debug.MsgBox("12+422", sFilter, iCount)
		Else
			Return Nothing
		End If

	End Function

	Public Shared Function GetMessagesCount(iMapThemeID As DMAcadExt.enMapTheme, Optional iActionID As Integer = 0, Optional bErrOnly As Boolean = False) As Integer
		Dim iCount As Integer
		Dim sAddFilter As String
		If bErrOnly Then
			sAddFilter = ") AND (Main AND Err)"
		Else
			sAddFilter = ") AND Main"
		End If

		If (moMsgTable IsNot Nothing) AndAlso iMapThemeID <> enMapTheme.Undefined Then
			Dim sFilter As String
			Dim sMapThemeID As String = Convert.ToString(CType(iMapThemeID, Integer))
			If iActionID = 0 Then
				sFilter = "(MapThemeID=" & sMapThemeID & sAddFilter
			Else
				sFilter = "(MapThemeID=" & sMapThemeID & ") AND (" & "ActionID=" & Convert.ToString(iActionID) & sAddFilter
			End If

			Dim oView As DataView = New DataView(moMsgTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
			iCount = oView.Count
			'   DMCommon.Debug.MsgBox("12+422", sFilter, iCount)
		Else
			iCount = 0
		End If
		Return iCount
	End Function
	Public Shared ReadOnly Property RecordCount As Integer
		Get
			If moMsgTable Is Nothing Then
				Return -1
			Else
				Return moMsgTable.Rows.Count
			End If
		End Get
	End Property
	Public Shared Sub ClearMessages(iMapThemeID As DMAcadExt.enMapTheme, Optional iActionID As Integer = 0)

		If (moMsgTable IsNot Nothing) AndAlso iMapThemeID <> enMapTheme.Undefined Then
			Dim sFilter As String
			Dim sMapThemeID As String = Convert.ToString(CType(iMapThemeID, Integer))

			If iActionID = 0 Then
				sFilter = "(MapThemeID=" & sMapThemeID & ")"
			Else
				sFilter = "(MapThemeID=" & sMapThemeID & ") AND (" & "ActionID=" & Convert.ToString(iActionID) & ") AND (" & "Main" & ")"
			End If

			Dim oView As DataView = New DataView(moMsgTable, sFilter, String.Empty, DataViewRowState.CurrentRows)
			'   DMCommon.Debug.MsgBox("12_422", sFilter, oView.Count)
			For Each oDataRowView As DataRowView In oView
				oDataRowView.Delete()
			Next
			'  

		End If

	End Sub
	Public Shared Sub AddMessage(ByVal bCoordinatesExist As Boolean, ByVal dX As Double, ByVal dY As Double, ByVal dXmin As Double, ByVal dYmin As Double, ByVal dXmax As Double, ByVal dYmax As Double, ByVal sSysID As String, ByVal sText As String _
										  , ByVal bAcadTextWin As Boolean, Optional iMapThemeID As DMAcadExt.enMapTheme = enMapTheme.Undefined, Optional iActionID As Integer = 0, Optional bMain As Boolean = True, Optional bErr As Boolean = True)
		zzCreateMsgTable()
		zzInit()
		Dim oNewRow As DataRow = moMsgTable.NewRow
		Dim tAction As Action = New Action()
		Dim tMapThemeData As DMAcadExt.MapThemeData = New MapThemeData()
		With oNewRow
			.Item("CoordinatesExist") = bCoordinatesExist
			.Item("ExtentsExist") = True
			If bCoordinatesExist Then
				.Item("X") = Math.Round(dX, 2)
				.Item("Y") = Math.Round(dY, 2)
			End If
			.Item("Xmin") = dXmin
			.Item("Ymin") = dYmin
			.Item("Xmax") = dXmax
			.Item("Ymax") = dYmax

			.Item("SysID") = sSysID
			.Item("Text") = sText

			.Item("ActionID") = iActionID
			If mdicActions.TryGetValue(iActionID, tAction) Then
				.Item("ActionName") = tAction.ActionName

				tAction.MapThemeID = iMapThemeID
				mcolActions.Add(tAction)
			End If
			If mdicMapThemes.TryGetValue(iMapThemeID, tMapThemeData) Then
				.Item("MapThemeName") = tMapThemeData.MapThemeName
			End If

			.Item("MapThemeID") = iMapThemeID

			.Item("Main") = bMain
			.Item("Err") = bErr

		End With
		moMsgTable.Rows.Add(oNewRow)
		If bAcadTextWin Then
			AcadDocument.WriteMessage(sText)
		End If
	End Sub
	Public Shared Sub AddMessage(ByVal bCoordinatesExist As Boolean, ByVal dX As Double, ByVal dY As Double, ByVal sSysID As String, ByVal iPaintException As PaintException, ByVal bAcadTextWin As Boolean)
		AddMessage(bCoordinatesExist, dX, dY, sSysID, zzGetExceptionText(iPaintException), bAcadTextWin)
	End Sub
	Public Shared Sub AddMessage(ByVal bCoordinatesExist As Boolean, ByVal oPoint As TPlnPoint, ByVal sSysID As String, ByVal sText As String, ByVal bAcadTextWin As Boolean, Optional iMapThemeID As DMAcadExt.enMapTheme = enMapTheme.Undefined, Optional iActionID As Integer = 0, Optional bMain As Boolean = True, Optional bErr As Boolean = True)
		AddMessage(bCoordinatesExist, oPoint.X, oPoint.Y, sSysID, sText, bAcadTextWin, iMapThemeID, iActionID, bMain, bErr)
	End Sub
	Public Shared Sub AddMessage(ByVal bCoordinatesExist As Boolean, ByVal oPoint As TPlnPoint, ByVal oBoundingBox As TPlnBoundingBox, ByVal sSysID As String, ByVal sText As String _
										  , ByVal bAcadTextWin As Boolean, Optional iMapThemeID As DMAcadExt.enMapTheme = enMapTheme.Undefined, Optional iActionID As Integer = 0, Optional bMain As Boolean = True, Optional bErr As Boolean = True)
		If oBoundingBox IsNot Nothing AndAlso oPoint IsNot Nothing Then
			Try
				AddMessage(bCoordinatesExist, oPoint.X, oPoint.Y, oBoundingBox.MinPoint.X, oBoundingBox.MinPoint.Y, oBoundingBox.MaxPoint.X, oBoundingBox.MaxPoint.Y, sSysID, sText, bAcadTextWin, iMapThemeID, iActionID, bMain, bErr)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AddMessage:AppMessages_1")
			End Try

		Else
			AddMessage(bCoordinatesExist, oPoint.X, oPoint.Y, sSysID, sText, bAcadTextWin, iMapThemeID, iActionID, bMain, bErr)
		End If

	End Sub
	Public Shared Sub ClearAll()
		If moMsgTable IsNot Nothing Then
			moMsgTable.Rows.Clear()
		End If
	End Sub
	Public Shared Sub Terminate()
		If moMsgTable IsNot Nothing Then
			moMsgTable.Dispose()
			moMsgTable = Nothing

		End If
	End Sub
	Public Shared Function GetTopoPolygonSysID(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iPgonTopoID As Integer) As String

		Select Case iTopoPurpose
			Case DMAcadExt.enTopoPurpose.Parcel
				Return msParselSysKey & "," & iPgonTopoID.ToString()
			Case Else
				Return Nothing
		End Select
	End Function
	Public Shared Function GetIntersectSysID(iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iPgonTopoID As Integer) As String
		Dim bIsParcel As Boolean = (iTopoPurpose = enTopoPurpose.Parcel)

		Return msIntersectSysKey & "," & CInt(iOverlayIndex).ToString() & "," & bIsParcel.ToString() & "," & iPgonTopoID.ToString()
	End Function
	Public Shared Function ParseSysID(ByVal sSysID As String, ByRef iOverlayIndex As DMAcadExt.enOverlayIndex, ByRef iParcelID As Integer, ByRef iOverlayTopoID As Integer) As Boolean
		Dim saSysID() As String = Split(sSysID, ",")

		If saSysID.GetUpperBound(0) >= 1 Then
			Select Case saSysID(0)
				Case msParselSysKey
					Return Integer.TryParse(saSysID(1), iParcelID)
				Case msIntersectSysKey
					Dim bIsParcel As Boolean
					Dim iTopoID As Integer
					If saSysID.GetUpperBound(0) >= 3 AndAlso [Enum].TryParse(Of DMAcadExt.enOverlayIndex)(saSysID(1), iOverlayIndex) AndAlso Boolean.TryParse(saSysID(2), bIsParcel) AndAlso Integer.TryParse(saSysID(3), iTopoID) Then
						If bIsParcel Then
							iParcelID = iTopoID
						Else
							iOverlayTopoID = iTopoID
						End If
						Return True
					Else
						DMCommon.Debug.MsgBox("221120_9", saSysID.GetUpperBound(0), saSysID, [Enum].TryParse(Of DMAcadExt.enOverlayIndex)(saSysID(1), iOverlayIndex), Boolean.TryParse(saSysID(2), bIsParcel), Integer.TryParse(saSysID(3), iTopoID))
						Return False
					End If
				Case Else
					Return False
			End Select
		Else
			Return False
		End If
	End Function
	Public Shared Property ZoomRadius() As Double
		Get
			Return mdZoomRadius
		End Get
		Set(ByVal dValue As Double)
			mdZoomRadius = dValue
		End Set
	End Property
	Private Shared Function zzGetExceptionText(ByVal iPaintException As PaintException) As String
		Select Case iPaintException
			Case PaintException.CreatingPolygonTopologyFailed
				Return "Creating Polygon Topology is failed"
			Case PaintException.SourcePgonTopologyForBufferNotExists
				Return "The source Polygon Topology for Buffer does not exist"
			Case PaintException.CreatingPgonTopologyForBufferFailed
				Return "Creating Polygon Topology for Buffer is failed"
			Case PaintException.CreatingPgonTopologyForZebraFailed
				Return "Creating Polygon Topology for Zebra is failed"
			Case PaintException.CreatingTopologyByBufferFailed
				Return "Creating Topology by Buffer is failed"
			Case PaintException.CreatingIntersectTopologyForZebraFailed
				Return "Creating Intersect Topology for Zebra is failed"
			Case Else
				Return "+" & iPaintException.ToString()
		End Select
	End Function

End Class
