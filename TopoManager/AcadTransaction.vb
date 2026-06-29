Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Public Class AcadTransaction
	Private Shared moTransaction As Transaction = Nothing
	Private Shared moTransactionFW As Transaction = Nothing
	Private Shared moBlockTable As BlockTable = Nothing
	Private Shared moModelSpaceTableRecord As BlockTableRecord
	Private Shared moDrawOrderTable As Autodesk.AutoCAD.DatabaseServices.DrawOrderTable
	Private Shared mcolDrawOrderIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Const miErrClassNo As Integer = 11000 '  iExNo   = 20
	Public Shared Sub Start()
		Try
			moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Start")
		End Try
	End Sub
	Public Shared Sub Terminate()
		If moTransaction IsNot Nothing Then
			Try
				''System.Windows.Forms.MessageBox.Show(CStr(moTransaction Is Nothing), "Terminate 12_402")
				moTransaction.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Terminate_1")
				Try
					moTransaction.Abort()
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "AcadTransaction - Terminate_2")
				End Try
			Finally
				moTransaction = Nothing
			End Try
		End If
	End Sub
	Private Shared Sub zzTerminateFW()
		If Not moTransactionFW Is Nothing Then
			Try
				moTransactionFW.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - TerminateFW_1")
				Try
					moTransactionFW.Abort()
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "AcadTransaction - TerminateFW_2")
				End Try
			Finally
				moTransactionFW = Nothing
			End Try
		End If
	End Sub

	Friend Shared ReadOnly Property TransactionExists() As Boolean
		Get
			Return moTransaction IsNot Nothing AndAlso (Not moTransaction.IsDisposed)
		End Get
	End Property
	Public Shared Function GetXrecord(ByVal tAcObjID As ObjectId) As Xrecord
		Const iExNo As Integer = 30
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Return DirectCast(oDBObject, Xrecord)
		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetBlockRef(ByVal tAcObjID As ObjectId) As BlockReference
		Const iExNo As Integer = 10
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Return DirectCast(oDBObject, BlockReference)
		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetCurve(ByVal tAcObjID As ObjectId) As Curve
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Return DirectCast(oDBObject, Curve)
		Catch oEx As System.Exception
			Return Nothing
		End Try
	End Function

	Public Shared Function GetAllBlockRefs(ByVal sBlockName As String) As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oBlockAcObjId As ObjectId
		Dim oAllBlockCol As ObjectIdCollection = Nothing
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAllBlockRefs")
		End Try
		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try
				oBlockAcObjId = oBlockTable.Item(sBlockName)
				oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetAllBlockRefs")
				oAllBlockCol = Nothing
			End Try
		End If
		Return oAllBlockCol
	End Function
	Public Shared Function GetAAA(ByVal oAcObjID As ObjectId) As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Dim oBlockTableRecord As BlockTableRecord


		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)

		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(oAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			Return Nothing
		Catch oEx As System.Exception
			Return Nothing
		End Try
	End Function
	Private Shared Function zzGetLayer(ByRef sLayer As String, ByVal iLayerFunction As enLayerFunction, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean) As ObjectId
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim tLayerAcObjId As ObjectId = New ObjectId()
		Dim iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode
		Dim oLayerDef As AcadLayerDef = Nothing
		If bCreate Then
			iMode = OpenMode.ForWrite
		Else
			iMode = OpenMode.ForRead
		End If

		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, iMode), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "Mode=" & iMode.ToString(), "zzGetLayer_2")
			Return tLayerAcObjId
		End Try
		If sLayer.Length = 0 Then
			oLayerDef = New AcadLayerDef(iLayerFunction)
			If oLayerDef.Correct Then
				sLayer = oLayerDef.Name
			End If
		End If
		If sLayer.Length <> 0 Then
			If oLayerTable.Has(sLayer) Then
				tLayerAcObjId = oLayerTable.Item(sLayer)
				If bCheckIsOn Then
					Try
						oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForWrite), LayerTableRecord)
						If oLayerTableRecord.IsOff Then
							Dim sMsg As String = "The current layer turned off." & vbCrLf & "Turn on the current layer?" & vbCrLf

							Dim iRes As System.Windows.Forms.DialogResult = System.Windows.Forms.MessageBox.Show(sMsg, Common.AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
							Select Case iRes
								Case DialogResult.Cancel
									Return New ObjectId()
								Case DialogResult.Yes
									oLayerTableRecord.IsOff = False
							End Select
							'   System.Windows.Forms.MessageBox.Show(oLayerTableRecord.IsOff.ToString(), "29_069:IsOff")
						End If

					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetLayer")
					End Try
				End If



			ElseIf bCreate Then
				If oLayerDef Is Nothing Then
					oLayerDef = New AcadLayerDef(sLayer)
				End If

				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer

				If oLayerDef IsNot Nothing Then
					If oLayerDef.Color IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.Color

					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
				End If
				oLayerTable.UpgradeOpen()
				tLayerAcObjId = oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
			End If
		End If

		oLayerTable = Nothing
		Return tLayerAcObjId
	End Function

	Public Shared Function AppendEntity(ByVal oEntity As Entity, Optional ByVal bAddDrawOrderTable As Boolean = False) As ObjectId
		Dim tAcObjID As ObjectId
		If moModelSpaceTableRecord IsNot Nothing Then
			Try
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oEntity)
				'	AcadDocument.WriteMessage("Ent: " & tAcObjID.ToString() & ":" & oEntity.GetRXClass.Name)
				moTransaction.AddNewlyCreatedDBObject(oEntity, True)
				If bAddDrawOrderTable AndAlso mcolDrawOrderIDs IsNot Nothing Then
					mcolDrawOrderIDs.Add(tAcObjID)
				End If
				Return tAcObjID
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - AppendEntity")
				Return New ObjectId(New System.IntPtr(0))
			End Try
		Else
			MessageBox.Show("ERRRRROR", "12_651")
			Return New ObjectId(New System.IntPtr(0))
		End If

	End Function
	Public Shared Sub AppendDBObject(ByVal oDBObject As DBObject)
		If moTransaction IsNot Nothing Then
			Try
				moTransaction.AddNewlyCreatedDBObject(oDBObject, True)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.Message, "AcadTransaction - AppendDBObject")
				AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - AppendDBObject_2")

			End Try
		Else
			MessageBox.Show("ERRRRROR " & CStr(moTransaction Is Nothing), "12_657")
		End If
	End Sub
	Private Shared Function zzGetLayers(ByVal saLayers() As String, ByVal bCreate As Boolean) As ObjectId()
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord

		Dim iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode

		If bCreate Then
			iMode = OpenMode.ForWrite
		Else
			iMode = OpenMode.ForRead
		End If
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & iMode.ToString(), "AcadTransaction - zzGetLayers!!!")
			Return Nothing
		End Try

		Dim iArrayUB As Integer = saLayers.GetUpperBound(0)
		Dim taObjectIDs(iArrayUB) As ObjectId
		Dim sLayer As String
		For iIndex As Integer = 0 To iArrayUB
			sLayer = saLayers(iIndex)
			If oLayerTable.Has(sLayer) Then
				taObjectIDs(iIndex) = oLayerTable.Item(sLayer)
			ElseIf bCreate Then
				Dim oLayerDef As AcadLayerDef = New AcadLayerDef(sLayer)
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer
				If oLayerDef IsNot Nothing Then
					If oLayerDef.Color IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.Color
					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
				End If
				oLayerTable.UpgradeOpen()
				taObjectIDs(iIndex) = oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
			Else
				taObjectIDs(iIndex) = New ObjectId()
			End If
		Next
		oLayerTable.Dispose()
		oLayerTable = Nothing
		Return taObjectIDs
	End Function
	Public Shared Function SetCurrentLayer(ByRef sLayer As String, ByVal iLayerFunction As enLayerFunction, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean, ByVal bAutoStartTransaction As Boolean) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)

		Dim tLayerObjID As ObjectId = zzGetLayer(sLayer, iLayerFunction, bCreate, bCheckIsOn)

		If bAutoStartTransaction Then
			Terminate()
		End If
		If tLayerObjID.IsNull Then
			Return False
		ElseIf oCurrentDatabase.Clayer <> tLayerObjID Then
			oCurrentDatabase.Clayer = tLayerObjID
		End If
		Return True

	End Function
	Public Shared Function CreateLayer(ByVal sLayer As String, ByVal iLayerFunction As enLayerFunction, ByVal bAutoStartTransaction As Boolean) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim oLayerDef As AcadLayerDef = Nothing
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_1")
			Return False
		End Try
		If sLayer.Length = 0 Then
			oLayerDef = New AcadLayerDef(iLayerFunction)
			sLayer = oLayerDef.Name
		End If

		If Not oLayerTable.Has(sLayer) Then

			If oLayerDef Is Nothing Then
				oLayerDef = New AcadLayerDef(sLayer)
			End If

			Try
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer

				If oLayerDef IsNot Nothing Then
					If oLayerDef.Color IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.Color
					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
				End If
				oLayerTable.UpgradeOpen()
				oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_2")
				Return False
			End Try

		End If
		oLayerTable = Nothing


		Return True

	End Function
	Public Shared Sub TestSetLayer(ByVal sLayer As String, ByVal sCaption As String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim tObjectID As ObjectId
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
		Try
			oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", "", True)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, sCaption & " AcadTransaction - TestSetLayer_0!!!")
		End Try


		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, sCaption & " AcadTransaction - TestSetLayer_1!!!")
			Exit Sub
		End Try
		If oLayerTable.Has(sLayer) Then
			tObjectID = oLayerTable.Item(sLayer)
		Else
			System.Windows.Forms.MessageBox.Show("Layer '" & sLayer & " was not found!", "AcadTransaction - TestLayers!!!")
			Exit Sub
		End If

		Try
			oLayerTableRecord = DirectCast(moTransaction.GetObject(tObjectID, OpenMode.ForWrite), LayerTableRecord)
			oLayerTableRecord.IsOff = Not oLayerTableRecord.IsOff
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tObjectID.ToString(), sCaption & " AcadTransaction - TestSetLayer_2!!!")
		End Try


		oLayerTable = Nothing
		If oDocLock IsNot Nothing Then
			oDocLock.Dispose()
		End If


	End Sub
	Public Shared Function SetLayersOn(ByVal saLayers() As String, ByVal bOn As Boolean, ByVal bAutoStartTransaction As Boolean) As System.Windows.Forms.CheckState
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTableRecord As LayerTableRecord = Nothing
		Dim taLayerObjID() As ObjectId
		Dim bOff As Boolean
		Dim iState As CheckState = CheckState.Checked

		zzAutoStart(bAutoStartTransaction)
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing


		Try
			oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", "", True)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOn_0")
		End Try
		taLayerObjID = zzGetLayers(saLayers, False)

		If taLayerObjID Is Nothing Then
			If oDocLock IsNot Nothing Then
				oDocLock.Dispose()
			End If
			Exit Function
		End If
		Dim bFirstLayer As Boolean = True
		For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
			If Not taLayerObjID(iIndex).IsNull Then
				Try
					oLayerTableRecord = DirectCast(moTransaction.GetObject(taLayerObjID(iIndex), OpenMode.ForWrite), LayerTableRecord)
					If oLayerTableRecord.IsOff = bOn Then
						oLayerTableRecord.IsOff = Not bOn
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iIndex) & vbCrLf & taLayerObjID(iIndex).ToString(), "SetLayersOn_1")
				End Try
				If bFirstLayer Then
					bOff = oLayerTableRecord.IsOff
					bFirstLayer = False
				Else
					If oLayerTableRecord.IsOff <> bOff AndAlso iState <> CheckState.Indeterminate Then
						iState = CheckState.Indeterminate
					End If
				End If
			End If
		Next
		If oDocLock IsNot Nothing Then
			oDocLock.Dispose()
		End If
		If bAutoStartTransaction Then
			Terminate()
		End If

		If iState <> CheckState.Indeterminate Then
			If bOff Then
				iState = CheckState.Unchecked
			Else
				iState = CheckState.Checked
			End If
		End If
		Return iState
	End Function
	Public Shared Function LayersIsOn(ByVal saLayers() As String, ByVal bAutoStartTransaction As Boolean) As System.Windows.Forms.CheckState
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTableRecord As LayerTableRecord
		Dim iState As CheckState = CheckState.Checked
		Dim bOff As Boolean
		Dim taLayerObjID() As ObjectId
		zzAutoStart(bAutoStartTransaction)
		taLayerObjID = zzGetLayers(saLayers, False)
		Dim bFirstLayer As Boolean = True
		For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
			If Not taLayerObjID(iIndex).IsNull Then
				Try
					oLayerTableRecord = DirectCast(moTransaction.GetObject(taLayerObjID(iIndex), OpenMode.ForRead), LayerTableRecord)
					' zzTestLayerRec(oLayerTableRecord)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "LayerIsOn")
					oLayerTableRecord = Nothing
				End Try

				If oLayerTableRecord IsNot Nothing Then
					If bFirstLayer Then
						bOff = oLayerTableRecord.IsOff
						bFirstLayer = False
					Else
						If oLayerTableRecord.IsOff <> bOff Then
							iState = CheckState.Indeterminate
							Exit For
						End If
					End If
				End If
			End If
		Next
		If bAutoStartTransaction Then
			Terminate()
		End If

		If iState <> CheckState.Indeterminate Then
			If bOff Then
				iState = CheckState.Unchecked
			Else
				iState = CheckState.Checked
			End If
		End If

		Return iState
	End Function
	Private Shared Sub zzTestLayerRec(ByVal oLayerTableRecord As LayerTableRecord)
		Dim sName As String = oLayerTableRecord.Name
		Dim IsOff As Boolean = oLayerTableRecord.IsOff
		Dim IsHidden As Boolean = oLayerTableRecord.IsHidden
		Dim sOut As String = "Name=" & sName & vbCrLf
		sOut &= "IsOff=" & CStr(IsOff) & vbCrLf
		sOut &= "IsHidden=" & CStr(IsHidden)
		System.Windows.Forms.MessageBox.Show(sOut, "LayerRecord")
	End Sub
	Public Shared Sub OpenModelSpace(ByVal iMode As OpenMode, Optional ByVal bOpenDrawOrderTable As Boolean = False)
		Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			moModelSpaceTableRecord = DirectCast(moTransaction.GetObject(moBlockTable(BlockTableRecord.ModelSpace), iMode, False), BlockTableRecord)
			If bOpenDrawOrderTable Then
				tDrawOrderTableID = moModelSpaceTableRecord.DrawOrderTableId
				moDrawOrderTable = DirectCast(moTransaction.GetObject(tDrawOrderTableID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DrawOrderTable)
				mcolDrawOrderIDs = New ObjectIdCollection()
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - OpenModelSpace")
		End Try
	End Sub


	Public Shared Sub CloseModelSpace()
		If moDrawOrderTable IsNot Nothing AndAlso mcolDrawOrderIDs IsNot Nothing Then
			If mcolDrawOrderIDs.Count > 0 Then
				Try
					moDrawOrderTable.MoveToBottom(mcolDrawOrderIDs)
					mcolDrawOrderIDs.Clear()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CloseModelSpace")
				End Try
			End If
			mcolDrawOrderIDs = Nothing
		End If
		moModelSpaceTableRecord = Nothing
		moBlockTable = Nothing
	End Sub
	Public Shared Function GetExtents2d(ByVal tAcObjID As ObjectId) As Extents2d
		Dim oEntity As Entity = zzGetEntity(tAcObjID)
		Dim tExtents3d As Extents3d = oEntity.GeometricExtents
		Return New Extents2d(tExtents3d.MinPoint.X, tExtents3d.MinPoint.Y, tExtents3d.MaxPoint.X, tExtents3d.MaxPoint.Y)
	End Function
	Public Shared Function GetBoundingBox(ByVal tAcObjID As ObjectId) As TPlnBoundingBox
		Dim oEntity As Entity = zzGetEntity(tAcObjID)
		Dim tExtents3d As Extents3d = oEntity.GeometricExtents

		Return New TPlnBoundingBox(tExtents3d)
	End Function
	Public Shared Function GetAttribDef(ByVal sBlockName As String) As String()
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim oAttribDefIndex As Integer = -1
		Dim oBlockObjID As ObjectId = New ObjectId()
		Dim saOut() As String = Nothing

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Try
			If oBlockTable.Has(sBlockName) Then
				oBlockObjID = oBlockTable.Item(sBlockName)
			Else
				System.Windows.Forms.MessageBox.Show("Block " & sBlockName & " was not found", "AcadTransaction - GetAttribDef")
			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - GetAttribDef_2")
		End Try

		If Not oBlockObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If
		If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
			For Each oObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(oObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name
				If sRXClassName = "AcDbAttributeDefinition" Then
					oAttribDefIndex += 1
					ReDim Preserve saOut(oAttribDefIndex)
					oAttributeDef = DirectCast(oDBObject, AttributeDefinition)
					saOut(oAttribDefIndex) = oAttributeDef.Tag
				End If
			Next
			Return saOut
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetAttribText(ByVal oAcObjID As ObjectId, ByVal iaAttribIndices() As Integer) As String()
		Const iExNo As Integer = 20
		Dim colAttributes As AttributeCollection = zzGetBlockAttrib(oAcObjID)
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1
		Dim sTest As String = "a"
		If colAttributes IsNot Nothing Then
			Try
				iValuesUB = iaAttribIndices.GetUpperBound(0)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "AcadTransaction - GetAttribText_1")
			End Try
			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 AndAlso (colAttributes.Count = iValuesUB + 1) Then
				Dim saOutText(iValuesUB) As String
				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				'    Start()

				Dim iAttribIndex As Integer
				'    Start()


				sTest = "b"
				For iIndex As Integer = 0 To iValuesUB
					Try
						sTest = "b"
						iAttribIndex = iaAttribIndices(iIndex)
						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iaAttribIndices(iIndex))
							sTest = "c"
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
							sTest = "d"
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							sTest = "f"
							saOutText(iAttribIndex) = oAttribRef.TextString
						End If
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "AcadTransaction - GetAttribText_2")
					End Try
				Next
				Return saOutText
			Else
				AcadDocument.WriteException(miErrClassNo, iExNo, "Invalid Block Attributes number")
				'System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - GetAttribText_3")
				Return Nothing
			End If
		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_4" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If
		Exit Function
	End Function

	Public Shared Sub UpdateAttribText(ByVal oAcObjID As ObjectId, ByVal iaAttribIndices() As Integer, ByVal saAttribText() As String)
		Dim colAttributes As AttributeCollection = zzGetBlockAttrib(oAcObjID)
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1
		Dim sTestAttribValue As String = "---"
		Dim sTestAttribTag As String = "tg"
		If colAttributes IsNot Nothing Then
			Try
				iValuesUB = iaAttribIndices.GetUpperBound(0)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - UpdateAttribText_1")
			End Try
			Dim tAttribObjID As ObjectId
			Dim sTest As String = "a"
			If iValuesUB >= 0 Then

				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				Dim iAttribIndex As Integer
				'    Start()
				For iIndex As Integer = 0 To iValuesUB
					Try
						sTest = "b"
						iAttribIndex = iaAttribIndices(iIndex)
						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iAttribIndex)
							sTest = "c"
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
							sTest = "d"
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							sTest = "e"
							sTestAttribValue = oAttribRef.TextString
							sTest = "ea"
							sTestAttribTag = oAttribRef.Tag
							sTest = "eb"
							If oAttribRef.TextString <> saAttribText(iIndex) Then
								sTest = "ebc"
								oAttribRef.TextString = saAttribText(iIndex)
							End If
							sTest = "f"
							oAttribRef.Draw()
						End If
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - UpdateAttribText_2a_" & sTest & "," & sTestAttribTag & ": '" & sTestAttribValue & "'" & "-'" & saAttribText(iAttribIndex) & "'")
					End Try
				Next
			Else
				System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - UpdateAttribText_3")
			End If
		Else
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_41" & ": Block Attributes were not found " & oAcObjID.ToString)
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
		End If
	End Sub
	Public Shared Sub Highlight(ByVal colObjectIds As ObjectIdCollection)
		Dim oEntity As Entity
		Try
			For Each tObjID As ObjectId In colObjectIds
				oEntity = DirectCast(moTransaction.GetObject(tObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				oEntity.Highlight()
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - HighLight")
		End Try
	End Sub
	Public Shared Function GetViewAAA() As Object
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oViewTable As ViewTable
		Dim oViewAcObjId As ObjectId
		Dim oViewTableRecord As ViewTableRecord
		Dim sTest As String = ""
		Dim oSymbEnum As SymbolTableEnumerator
		Try
			oViewTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.ViewTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), ViewTable)
			oSymbEnum = oViewTable.GetEnumerator()
			oSymbEnum.Reset()
			Do While oSymbEnum.MoveNext
				oViewAcObjId = oSymbEnum.Current
				oViewTableRecord = DirectCast(moTransaction.GetObject(oViewAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), ViewTableRecord)
				sTest &= oViewTableRecord.Name & vbCrLf
			Loop
			oViewTableRecord = New ViewTableRecord()
			oViewTableRecord.Width = 100
			System.Windows.Forms.MessageBox.Show(sTest, "27_529")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetView")
			Return Nothing
		End Try
		'   System.Windows.Forms.MessageBox.Show(oViewTable., "GetAllBlockRefs")
		If oViewTable IsNot Nothing AndAlso oViewTable.Has("Current") Then
			Try
				oViewAcObjId = oViewTable.Item("Current")
				oViewTableRecord = DirectCast(moTransaction.GetObject(oViewAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), ViewTableRecord)

				Return Nothing
			Catch oEx As Exception
				Return Nothing
			End Try
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetAcadPointsCount(ByVal sLayer As String, ByVal bAutoStartTransaction As Boolean) As Integer
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim iOut As Integer = 0
		Dim sRXClassName As String
		Dim oEntity As Entity
		zzAutoStart(bAutoStartTransaction)

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)
		For Each oObjID As ObjectId In oBlockTableRecord
			oDBObject = moTransaction.GetObject(oObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			sRXClassName = oDBObject.GetRXClass().Name
			If sRXClassName = "AcDbPoint" Then
				Try
					oEntity = DirectCast(oDBObject, Entity)
					If oEntity.Layer = sLayer Then
						iOut += 1
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - ClearAcadPoints" & vbCrLf & oEx.Message)
				End Try
			End If
		Next
		If bAutoStartTransaction Then
			Terminate()
		End If
		Return iOut

	End Function
	Public Shared Function LinkExists(ByVal sLayers As String) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

		Dim sLayersDel As String
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity
		Dim bResult As Boolean = False

		sLayersDel = TopoDef.AddDelim(sLayers)
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			Dim sRXClassName As String
			Dim sEntityLayerName As String

			For Each objId As ObjectId In oBlockTableRecord
				oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name

				Select Case sRXClassName
					Case Common.AcadPolylineName, Common.AcadLineName, Common.AcadLWPolylineName
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							bResult = True
							Exit For
						End If
					Case Else
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							System.Windows.Forms.MessageBox.Show(sRXClassName, "???sRXClassName???zzLinkExists")
						End If
				End Select

			Next

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LinkExists")
		End Try
		Return bResult
	End Function
	Public Shared Sub EraseLinks(ByVal sLayers As String, ByVal bAutoStartTransaction As Boolean)

		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecModelSpace As BlockTableRecord = zzGetModelSpaceFW()

		If oBlockTableRecModelSpace IsNot Nothing Then
			Dim sLayersDel As String
			Dim oEntity As Entity

			'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
			'  sLayersDel = moTopoDefs.LinkLayers
			sLayersDel = TopoDef.AddDelim(sLayers)
			Dim sTest As String = "A"

			Try

				sTest = "D"
				Dim sRXClassName As String
				Dim sEntityLayerName As String


				For Each objId As ObjectId In oBlockTableRecModelSpace
					sTest = "Kaa"
					oEntity = DirectCast(moTransactionFW.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
					sTest = "Kb"
					sRXClassName = oEntity.GetRXClass().Name
					sTest = "Kc"
					Select Case sRXClassName
						Case Common.AcadPolylineName, Common.AcadLWPolylineName
							sTest = "Kca"
							sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
							sTest = "Kcb"
							If sLayersDel.Contains(sEntityLayerName) Then
								sTest = "Kcc"
								oEntity = Nothing
								sTest = "Kcd"
								oEntity = DirectCast(moTransactionFW.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)

								sTest = "M"
								oEntity.Erase()
								sTest = "N"
								oEntity.Dispose()
								sTest = "P"
							End If
					End Select

				Next
				sTest = "W"

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Eraselinks " & sTest)
			End Try

		End If
		AcadDocument.Unlock()
		zzTerminateFW()

	End Sub

	Public Shared Sub ClearLayerByClassName(ByVal sLayer As String, ByVal sRXClassName As String, ByVal bAutoStartTransaction As Boolean)
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecModelSpace As BlockTableRecord = zzGetModelSpaceFW()
		Dim oDBObject As DBObject
		Dim oEntity As Entity

		Dim iOut As Integer = 0

		Dim bAllClasses As Boolean = (sRXClassName.Length = 0)
		'	zzAutoStart(bAutoStartTransaction)

		If oBlockTableRecModelSpace IsNot Nothing Then
			For Each oObjID As ObjectId In oBlockTableRecModelSpace
				oDBObject = moTransactionFW.GetObject(oObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				If bAllClasses OrElse oDBObject.GetRXClass().Name = sRXClassName Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If oEntity.Layer = sLayer Then
							oDBObject.Erase()
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByClassName_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If

		zzTerminateFW()

		''	AcadDocument.Unlock()
	End Sub
	Public Shared Function GetBlockRefs(ByVal sBlockName As String, Optional ByVal sLayers As String = "") As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oBlockAcObjId As ObjectId
		Dim oOutBlockCol As ObjectIdCollection
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetBlockRefs")
			oOutBlockCol = New ObjectIdCollection()
		End Try

		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try
				oBlockAcObjId = oBlockTable.Item(sBlockName)
				oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				Dim oAllBlockCol As ObjectIdCollection
				oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
				If sLayers.Length = 0 Then
					oOutBlockCol = oAllBlockCol
				Else
					oOutBlockCol = New ObjectIdCollection()
					Dim oEntity As Entity
					Dim sEntityLayerName As String
					sLayers = TopoDef.AddDelim(sLayers)
					For Each objId As ObjectId In oAllBlockCol
						oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayers.Contains(sEntityLayerName) Then
							oOutBlockCol.Add(objId)
						End If
					Next
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "e200a")

				oOutBlockCol = New ObjectIdCollection()

			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Block " & sBlockName & " was not found", "AcadTransaction - zzGetBlockRefs")
			oOutBlockCol = New ObjectIdCollection()
		End If
		Return oOutBlockCol
	End Function
	Public Shared Function GetLinks(ByVal sLayers As String) As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		'  Dim oTransaction As Transaction = Nothing
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord
		Dim oEntity As Entity

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		'  sLayersDel = moTopoDefs.LinkLayers
		sLayersDel = TopoDef.AddDelim(sLayers)
		Dim sTest As String = "a"
		Try

			sTest = "b"
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			sTest = "c"
			oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			sTest = "d"
			Dim sRXClassName As String
			Dim sEntityLayerName As String
			For Each objId As ObjectId In oBlockTableRecord
				sTest = "e"
				oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sTest = "f"
				sRXClassName = oEntity.GetRXClass().Name
				sTest = "g"
				Select Case sRXClassName
					Case Common.AcadPolylineName, Common.AcadLWPolylineName, Common.AcadLineName, Common.Acad2dPolylineName
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							sTest = "h"
							colResIds.Add(objId)
							sTest = "i"
						End If
					Case Common.AcadBlockRefName
					Case Else
				End Select
			Next

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "e1400")
		End Try
		Return colResIds
	End Function

	Private Shared Sub zzAutoStart(ByRef bAutoStartTransaction As Boolean)
		If bAutoStartTransaction Then
			If moTransaction Is Nothing Then
				Start()
			Else
				bAutoStartTransaction = False
			End If
		End If
	End Sub
	Private Shared Function zzGetModelSpaceFW() As BlockTableRecord
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim sTest As String = "A"

		Try
			moTransactionFW = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetModelSpaceFW")
			Return Nothing
		End Try


		Try
			sTest = "Ap"
			oBlockTable = DirectCast(moTransactionFW.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForWrite, False, False), BlockTable)
			sTest = "B"
			oBlockTableRecord = DirectCast(moTransactionFW.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False, False), BlockTableRecord)

			Return oBlockTableRecord
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "AcadTransaction - zzGetModelSpaceFW")
			AcadDocument.WriteMessage("AcadTransaction - zzGetModelSpaceFW" & vbCrLf & oEx.Message)
			Return Nothing
		End Try
	End Function
	Private Shared Function zzGetBlockAttrib(ByVal oAcObjID As ObjectId) As AttributeCollection

		Dim colAttributes As AttributeCollection
		Dim oBlockRef As BlockReference = zzGetBlockRef(oAcObjID)
		If oBlockRef IsNot Nothing Then
			Try
				colAttributes = oBlockRef.AttributeCollection()
				Return colAttributes
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetBlockAttrib_1")
				Return Nothing
			End Try
		Else
			AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return Nothing
		End If


	End Function
	Public Shared Function GetNamedDictionary(ByVal iOpenMode As OpenMode) As DBDictionary
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tNamedDicObjID As ObjectId = oCurrentDatabase.NamedObjectsDictionaryId()
		Dim oDBObject As DBObject = moTransaction.GetObject(tNamedDicObjID, iOpenMode)
		Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary = DirectCast(oDBObject, DBDictionary)

		dicNamed = DirectCast(AcadTransaction.GetDBObject(tNamedDicObjID, OpenMode.ForWrite), DBDictionary)
		If dicNamed Is Nothing Then
			Return Nothing
		Else
			Return DirectCast(dicNamed, DBDictionary)
		End If
	End Function
	Public Shared Function GetDBObject(ByVal oAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As DBObject

		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Try

			oDBObject = moTransaction.GetObject(oAcObjID, iOpenMode)	'''''''''' BORIS 
			Return oDBObject
		Catch oEx As System.Exception
			AcadDocument.WriteMessage("#122AcadTransaction - zzGetBlockRef " & oAcObjID.ToString() & vbCrLf & oEx.Message)
			'   System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")

			Return Nothing
		End Try
	End Function
	Private Shared Function zzGetBlockRef(ByVal oAcObjID As ObjectId) As BlockReference

		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Try
			oDBObject = moTransaction.GetObject(oAcObjID, OpenMode.ForRead) '''''''''' BORIS 
			Return DirectCast(oDBObject, BlockReference)
		Catch oEx As System.Exception
			AcadDocument.WriteMessage("#121AcadTransaction - zzGetBlockRef " & oAcObjID.ToString() & vbCrLf & oEx.Message)
			'   System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")

			Return Nothing
		End Try

	End Function
	Private Shared Function zzGetEntity(ByVal oAcObjID As ObjectId) As Entity

		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Try
			oDBObject = moTransaction.GetObject(oAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

			Return DirectCast(oDBObject, Entity)
		Catch oEx As System.Exception
			Dim sTest As String = ""
			If moTransaction Is Nothing Then
				sTest = vbCrLf & "moTransaction Is Nothing"
			End If
			System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetEntity")

			Return Nothing
		End Try

	End Function
End Class


