Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Public Structure dmTolerance
	Public AngleTolerance As Double
	Public DistanceTolerance As Double
	Public AreaTolerance As Double

	Public Sub New(dAngleTolerance As Double, dDistanceTolerance As Double, dAreaTolerance As Double)
		AngleTolerance = dAngleTolerance
		DistanceTolerance = dDistanceTolerance
		AreaTolerance = dAreaTolerance
	End Sub
End Structure
Public Class AcadUtil
	Public Shared Function ToTypedValue(ByVal oValue As System.Object, b5000 As Boolean) As Autodesk.AutoCAD.DatabaseServices.TypedValue
		Dim iValueType As AcadConst.AcadValueType2018

		Select Case oValue.GetType().ToString
			Case "System.Double"
				If b5000 Then
					iValueType = AcadConst.AcadValueType2018.Double5000
				Else
					iValueType = AcadConst.AcadValueType2018.Double
				End If

			Case "System.Date", "System.DateTime"
				Dim dtValue As Date = DirectCast(oValue, Date)
				oValue = dtValue.ToOADate()
				If b5000 Then
					iValueType = AcadConst.AcadValueType2018.Double5000
				Else
					iValueType = AcadConst.AcadValueType2018.Double
				End If

			Case "System.String"
				If b5000 Then
					iValueType = AcadConst.AcadValueType2018.String5000
				Else
					iValueType = AcadConst.AcadValueType2018.String
				End If

			Case "System.Int32"
				If b5000 Then
					iValueType = AcadConst.AcadValueType2018.Integer5000
				Else
					iValueType = AcadConst.AcadValueType2018.Integer
				End If

			Case "Autodesk.AutoCAD.DatabaseServices.Handle"
				iValueType = AcadConst.AcadValueType2018.Handle
			Case Else
				iValueType = AcadConst.AcadValueType2018.Undefined
				System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString, "Design Err# 2790")
		End Select


		Try
			If iValueType <> AcadConst.AcadValueType2018.Undefined Then
				' System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString & ":" & CStr(oValue) & ":" & iValueType.ToString(), "12_338")
				Return New Autodesk.AutoCAD.DatabaseServices.TypedValue(Convert.ToInt16(iValueType), oValue)
			Else
				Return Nothing
			End If
		Catch oEx As Exception
			Return Nothing
		End Try

	End Function
	Public Shared Function ToTypedValueOld(ByVal oValue As System.Object) As Autodesk.AutoCAD.DatabaseServices.TypedValue
		Dim iValueType As AcadConst.AcadValueType

		Select Case oValue.GetType().ToString
			Case "System.Double"
				iValueType = AcadConst.AcadValueType.Double
			Case "System.Date", "System.DateTime"
				Dim dtValue As Date = DirectCast(oValue, Date)
				oValue = dtValue.ToOADate()
				iValueType = AcadConst.AcadValueType.Double
			Case "System.String"
				iValueType = AcadConst.AcadValueType.String
			Case "System.Int32"
				iValueType = AcadConst.AcadValueType.Integer

			Case "Autodesk.AutoCAD.DatabaseServices.Handle"
				iValueType = AcadConst.AcadValueType.Handle
			Case Else
				iValueType = AcadConst.AcadValueType.Undefined
				System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString, "Design Err# 2790")
		End Select


		Try
			If iValueType <> AcadConst.AcadValueType.Undefined Then
				' System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString & ":" & CStr(oValue) & ":" & iValueType.ToString(), "12_338")
				Return New Autodesk.AutoCAD.DatabaseServices.TypedValue(Convert.ToInt16(iValueType), oValue)
			Else
				Return Nothing
			End If
		Catch oEx As Exception
			Return Nothing
		End Try

	End Function
	Public Shared Sub AddObjectIDCollection(ByRef colSourceQAcObjIDs As ObjectIdCollection, colAddQAcObjIDs As ObjectIdCollection)
		If colAddQAcObjIDs IsNot Nothing Then
			For Each tAcObjID As ObjectId In colAddQAcObjIDs
				If Not colSourceQAcObjIDs.Contains(tAcObjID) Then
					colSourceQAcObjIDs.Add(tAcObjID)
				End If
			Next
		End If
	End Sub
	Public Shared Sub AddObjectIDCollection(ByRef colSourceQAcObjIDs As ObjectIdCollection, taAddQAcObjIDs() As ObjectId)
		If taAddQAcObjIDs IsNot Nothing Then
			For Each tAcObjID As ObjectId In taAddQAcObjIDs
				If Not colSourceQAcObjIDs.Contains(tAcObjID) Then
					colSourceQAcObjIDs.Add(tAcObjID)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!_Add", colSourceQAcObjIDs.Count, tAcObjID)
				End If
			Next
		End If
	End Sub
	Public Shared Function GetResBuffer(ByVal oValue As System.Object, b5000 As Boolean) As ResultBuffer
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		Dim oTypedValue As TypedValue = ToTypedValue(oValue, b5000)
		'DMCommon.Debug.MsgBox("12_333", oTypedValue.TypeCode, oTypedValue.TypeCode.GetType(), oTypedValue.TypeCode.GetTypeCode())
		oResBuffer.Add(oTypedValue)

		Return oResBuffer
	End Function
	Public Shared Function GetResBuffer(ByVal oaValue() As System.Object, b5000 As Boolean) As ResultBuffer
		Dim oResBuffer As ResultBuffer = New ResultBuffer()
		Dim oTypedValue As TypedValue
		For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
			oTypedValue = ToTypedValue(oaValue(iIndex), b5000)
			oResBuffer.Add(oTypedValue)
		Next
		'  System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString & ":" & CStr(oValue) & vbCrLf & oTypedValue.TypeCode.GetType().ToString() & ":" & oTypedValue.TypeCode.ToString() & ":" & oTypedValue.Value.ToString(), "12_397b")

		Return oResBuffer
	End Function

	Public Shared Function GetEditor() As Autodesk.AutoCAD.EditorInput.Editor
		Return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
	End Function
	Public Shared Function GetPoint(ByVal sPrompt As String, ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d) As Boolean
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim ptopts As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(sPrompt)
		ptopts.BasePoint = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 1.0)
		ptopts.UseBasePoint = False
		ptopts.UseDashedLine = True
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptPointResult = oEditor.GetPoint(ptopts)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			Return False
		Else
			tPoint = ptRes.Value
			Return True
		End If
	End Function
	Public Shared Function GetTwoPoints(ByVal sPrompt() As String) As Autodesk.AutoCAD.Geometry.Point3d()
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		DMAcadExt.AcadDocument.WriteMessage(String.Empty)
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
				AcadDocument.CommandLine(False)
				Return oaOutput
			End If

		End If
		Return Nothing
	End Function
	Public Shared Function GetDistance(ByVal sPrompt As String, ByRef dDistance As Double) As Boolean
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim dsOpts As Autodesk.AutoCAD.EditorInput.PromptDistanceOptions = New Autodesk.AutoCAD.EditorInput.PromptDistanceOptions(sPrompt)
		dsOpts.BasePoint = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 1.0)
		dsOpts.UseBasePoint = False
		dsOpts.UseDashedLine = True
		Dim dsRes As Autodesk.AutoCAD.EditorInput.PromptDoubleResult = oEditor.GetDistance(dsOpts)
		If dsRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			Return False
		Else
			dDistance = dsRes.Value
			Return True
		End If
	End Function
	Public Shared Function GetName(ByVal sPrompt As String) As String
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptStringOptions = New Autodesk.AutoCAD.EditorInput.PromptStringOptions(sPrompt)
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptResult
		Try
			oPromptOpt.AllowSpaces = False
			ptRes = oEditor.GetString(oPromptOpt)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
			Return Nothing
		End Try
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			Return ptRes.StringResult
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetLinesA() As ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		'Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim colLines As ObjectIdCollection = New ObjectIdCollection
		Dim iTotal As Integer = 0
		Dim iFound As Integer = 0
		Try
			oPromptOpt.AllowDuplicates = False
			oPromptOpt.SingleOnly = False
			oPromptOpt.MessageForAdding = "Add ***"
			oPromptOpt.MessageForRemoval = "Remove ***"

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taObjIDs() As ObjectId
		'Do
		ptRes = oEditor.GetSelection(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptRes.Value()
			taObjIDs = oSelSet.GetObjectIds
			For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)
				colLines.Add(taObjIDs(iIndex))
			Next

			iFound = colLines.Count - iTotal
			iTotal = colLines.Count
			AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
		ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			colLines = Nothing
			'Exit Do
		Else
			AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
			'Exit Do
		End If
		'	Loop

		Return colLines
	End Function
   Public Shared Sub DispAcadEnt(iNumber As Integer, oAcadEnt As Entity)
      Dim sMsg As String = Nothing
      Select Case oAcadEnt.GetRXClass().Name
         Case AcadConst.Acad2dPolylineName
         Case AcadConst.Acad2dVertexName
            Dim oVertex2d As Vertex2d = DirectCast(oAcadEnt, Vertex2d)
            sMsg = "#" & iNumber.ToString() & " Vertex2d: " & oVertex2d.VertexType.ToString() & ", B=" & CStr(oVertex2d.Bulge) & ", Pos=" & TPlnPoint.DispPoint(oVertex2d.Position) ' & ", Tng=" & CStr(oVertex2d.Tangent) & "," & CStr(oVertex2d.TangentUsed)

      End Select
      If sMsg IsNot Nothing Then
         DMAcadExt.AcadDocument.WriteMessage(sMsg)
      End If

   End Sub

	Public Shared Function GetEntities() As ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		'	Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim colLines As ObjectIdCollection = New ObjectIdCollection
		Dim iTotal As Integer = 0
		Dim iFound As Integer = 0
		Try
			oPromptOpt.AllowDuplicates = False
			oPromptOpt.SingleOnly = False
			oPromptOpt.MessageForAdding = "Add ***"
			oPromptOpt.MessageForRemoval = "Remove ***"

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taObjIDs() As ObjectId
		'Do
		ptRes = oEditor.GetSelection(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			oSelSet = ptRes.Value()
			taObjIDs = oSelSet.GetObjectIds
			For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)
				colLines.Add(taObjIDs(iIndex))
			Next

			iFound = colLines.Count - iTotal
			iTotal = colLines.Count
			AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
		ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
			colLines = Nothing
			'Exit Do
		Else
			AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
			'Exit Do
		End If
		'	Loop

		Return colLines
	End Function
	Public Shared Function GetLines() As ObjectIdCollection
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions("Select link")
		Dim oPolyline As Polyline = New Polyline
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
		Dim colLines As ObjectIdCollection = New ObjectIdCollection
		Dim iTotal As Integer = 0
		Dim iFound As Integer = 0
		Try
			oPromptOpt.SetRejectMessage("OOO!!!")
			oPromptOpt.AllowNone = True
			oPromptOpt.AddAllowedClass(oPolyline.GetType(), True)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try
		Do
			ptRes = oEditor.GetEntity(oPromptOpt)
			If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
				colLines.Add(ptRes.ObjectId)
				iFound = colLines.Count - iTotal
				iTotal = colLines.Count
				AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
			ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
				colLines = Nothing
				Exit Do
			Else
				Exit Do
			End If
		Loop

		Return colLines
	End Function
	Public Shared Sub UpdateAttribText(ByVal iaAttribIndices() As Integer, ByVal saAttribText() As String, Optional ByVal sBlockName As String = "", Optional ByVal sLayerName As String = "")
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
		Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
		Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
		Dim iTotal As Integer = 0
		Dim iFound As Integer = 0
		Dim sMsgBlockName As String
		If sBlockName.Length = 0 Then
			sMsgBlockName = String.Empty
		Else
			sMsgBlockName = " Block '" & sBlockName & "'"
		End If


		Try
			oPromptOpt.AllowDuplicates = False
			oPromptOpt.SingleOnly = False
			oPromptOpt.MessageForAdding = "Select" & sMsgBlockName & ":"
			oPromptOpt.MessageForRemoval = "Remove ..."
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ToString() & vbCrLf & oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.Source & vbCrLf & oAcadEx.StackTrace, "GetLines")
		End Try
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet
		Dim taObjIDs() As ObjectId

		ptRes = oEditor.GetSelection(oPromptOpt)
		If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			oSelSet = ptRes.Value()
			taObjIDs = oSelSet.GetObjectIds
			For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)
				If AcadTransaction.UpdateAttribText(taObjIDs(iIndex), False, iaAttribIndices, saAttribText, sBlockName, sLayerName) Then
					iFound = 1
					iTotal += iFound
				End If
			Next
			AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then

			'Exit Do
		Else
			AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
			'Exit Do
		End If
   End Sub
   Public Shared Function GetAcadColorIndex(iIndex As Integer) As Integer
      Dim iRow, iCol As Integer

      iRow = Math.DivRem(iIndex, 24, iCol)
      iCol = (7 * iCol) Mod 24
      iRow = iRow Mod 10

      Return 10 + 10 * iCol + iRow
   End Function
End Class
