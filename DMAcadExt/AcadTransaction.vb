Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports System.Windows.Forms

Public Class AcadTransaction
	Private Enum enPolygonStatus
		None
		Polyline
		ClosedPolyline
	End Enum
	Private Structure ClosedPgonID
		Dim MinID As Integer
		Dim Count As Integer
		Public Sub AddID(iID As Integer)
			If MinID > iID Then
				MinID = iID
			End If
			Count += 1
		End Sub
		Public Shared Operator =(tA As ClosedPgonID, tB As ClosedPgonID) As Boolean
			Return (tA.MinID = tB.MinID) AndAlso (tA.Count = tB.Count)
		End Operator
		Public Shared Operator <>(tA As ClosedPgonID, tB As ClosedPgonID) As Boolean
			Return (tA.MinID <> tB.MinID) OrElse (tA.Count <> tB.Count)
		End Operator
	End Structure
	Private Const msEmpty As String = ""
	Private Const msExceptionLayer As String = "zzException"
	Private Const msFreezeLayer As String = "zzFreeze"

	Private Shared moTransaction As Transaction = Nothing
	Private Shared moTransactionFW As Transaction = Nothing
	Private Shared moBlockTable As BlockTable = Nothing
	Private Shared moModelSpaceTableRecord As BlockTableRecord
	Private Shared miModelSpaceOpenMode As OpenMode
	Private Shared moDrawOrderTable As Autodesk.AutoCAD.DatabaseServices.DrawOrderTable
	Private Shared mcolDrawOrderIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	'	Private Shared mcolDBObjectOrderIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection

	Private Shared moNewBlockDB As Database
	Private Shared moBlockTableRecord As BlockTableRecord
	Private Shared mtNewBlockObjId As ObjectId
	Private Shared miBlockNameCounter As Integer = 0
	Private Shared mdicHandles As Dictionary(Of Handle, ObjectId)
	Private Shared mtCurrentLayerObjID As ObjectId
	Private Shared mtPrevCurrentLayerObjID As ObjectId

	Const miErrClassNo As Integer = 11000 '  iExNo   = 20
	Public Delegate Sub Procedure(ByVal oDBObject As DBObject)
	Public Delegate Sub EntityProc(ByVal oEntity As Entity, bCond As Boolean)

	Public Delegate Function BlockDrawing() As Entity()

	Public Shared Sub Start()
		Try
         moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Start")
		End Try
   End Sub
   Public Shared Sub ReStart()
      Terminate()
      Start()
   End Sub
   Public Shared Sub Start(oDocument As Autodesk.AutoCAD.ApplicationServices.Document)
      Try
         moTransaction = oDocument.TransactionManager.StartTransaction()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Start")
      End Try
   End Sub
	Public Shared Function IsActive() As Boolean
		Try
			Return (moTransaction IsNot Nothing)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - IsActive")
		End Try
	End Function

	Public Shared Sub Terminate()
		If moTransaction IsNot Nothing Then
			Try
				moTransaction.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Terminate_1")
				Try
					moTransaction.Abort()
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "AcadTransaction - Terminate_2")
				End Try
			Finally
				If moTransaction IsNot Nothing Then
					moBlockTableRecord = Nothing
            End If
            moTransaction.Dispose()
				moTransaction = Nothing
			End Try
		End If
	End Sub
	Public Shared Sub Abort()
		If moTransaction IsNot Nothing Then
			Try
				moTransaction.Abort()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Abort_1")
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
   'DataBase Handle	1005. The handle of an entity.
   Public Shared ReadOnly Property TransactionExists() As Boolean
      Get
         Return moTransaction IsNot Nothing AndAlso (Not moTransaction.IsDisposed)
      End Get
   End Property
	Public Shared Sub OpenHandleDictionary()
		Dim oAcadObj As DBObject
		If moModelSpaceTableRecord IsNot Nothing Then
			mdicHandles = New Dictionary(Of Handle, ObjectId)()
			For Each tObjID As ObjectId In moModelSpaceTableRecord
				oAcadObj = GetDBObject(tObjID, OpenMode.ForRead)
				mdicHandles.Add(oAcadObj.Handle, tObjID)

			Next
		Else
			System.Windows.Forms.MessageBox.Show("ModelSpaceTableRecord is Nothing", "02_780")
		End If

	End Sub
	Public Shared Sub DebugExcelObjIdCol(iFirstColumn As Integer, sCaption As String, colAcadObjIDs As ObjectIdCollection)
		'OpenHandleDictionary()
		Dim oDBObject As DBObject
		Dim iIndex As Integer = 0
		Dim taHandles(colAcadObjIDs.Count - 1) As Handle
		Dim bCloseTransaction As Boolean
		If moTransaction Is Nothing Then
			Start()
			bCloseTransaction = True
		End If
		For Each tAcObjID As ObjectId In colAcadObjIDs
			oDBObject = GetDBObject(tAcObjID, OpenMode.ForRead)
			taHandles(iIndex) = oDBObject.Handle

			iIndex += 1
		Next
		DMCommon.Debug.ExcelLog.SetEnumerable(iFirstColumn, sCaption, taHandles)
		If bCloseTransaction Then
			Terminate()
		End If
	End Sub
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
   Public Shared Function GetXData(sAppName As String, ByVal tAcObjID As ObjectId) As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
      Dim oDBObject As DBObject

      oDBObject = GetDBObject(tAcObjID, OpenMode.ForRead)

      Return oDBObject.GetXDataForApplication(sAppName)
   End Function
	Public Shared Function GetXData(sAppName As String, ByVal tHandle As Handle) As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
      '  Dim iRow As Integer
		If mdicHandles IsNot Nothing Then
			Dim tAcObjID As ObjectId
         '	Dim oDBObject As DBObject
         If mdicHandles.TryGetValue(tHandle, tAcObjID) Then

            ' tAcObjID As ObjectId
            'oDBObject = GetDBObject(tAcObjID, OpenMode.ForRead)
            'Return oDBObject.GetXDataForApplication(sAppName)
            Return GetXData(sAppName, tAcObjID)
         Else
          
            AcadDocument.WriteMessage("#A7 Handle '" & tHandle.ToString() & "' not found")
            Return Nothing
         End If
		Else
         System.Windows.Forms.MessageBox.Show("mdicHandles Is Nothing", "1AcadTransaction - GetXData")
			Return Nothing
		End If
   End Function
	Public Shared Function GetObjectID(ByVal tHandle As Handle) As Autodesk.AutoCAD.DatabaseServices.ObjectId

		If mdicHandles IsNot Nothing Then
			Dim tAcObjID As ObjectId

			If mdicHandles.TryGetValue(tHandle, tAcObjID) Then
				'	DMCommon.ExcelLogAW5.SetNextValue(1, "ToHandle", tHandle, tHandle.Value, tAcObjID)
				Return tAcObjID
			Else
				AcadDocument.WriteMessage("#A8 Handle '" & tHandle.ToString() & "' not found")
				Return ObjectId.Null
			End If
		Else
         System.Windows.Forms.MessageBox.Show("mdicHandles Is Nothing", "2AcadTransaction - GetObjectID")
			Return ObjectId.Null
		End If


	End Function
	Public Shared Function GetObjectID(ByVal sHandle As String) As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Dim tHandle As Handle = zzToHandle(sHandle)
		Return GetObjectID(tHandle)


	End Function

	Public Shared Function GetEntity(ByVal tHandle As Handle, ByVal iOpenMode As Autodesk.AutoCAD.DatabaseServices.OpenMode) As Autodesk.AutoCAD.DatabaseServices.Entity

		If mdicHandles IsNot Nothing Then
			Dim tAcObjID As ObjectId
			Dim oEntity As Entity
			If mdicHandles.TryGetValue(tHandle, tAcObjID) Then
				oEntity = GetEntity(tAcObjID, iOpenMode)
				Return oEntity
			Else
				AcadDocument.WriteMessage("#A7 Handle '" & tHandle.ToString() & "' not found")
				Return Nothing
			End If
		Else
			System.Windows.Forms.MessageBox.Show("mdicHandles Is Nothing", "AcadTransaction - GetXData")
			Return Nothing
		End If


	End Function

	Public Shared Sub ProcByFilter(ByVal dlEntityProc As EntityProc, Optional ByVal sLinkLayers As String = msEmpty, Optional ByVal sLineLayers As String = msEmpty, Optional ByVal sBlockName As String = msEmpty, Optional ByVal sBlockRefLayers As String = msEmpty)
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim bLinkFilterExists As Boolean = Not String.IsNullOrEmpty(sLinkLayers)
			Dim bBlockRefFilterExists As Boolean = Not String.IsNullOrEmpty(sBlockName)
			Dim oLinkCondition As LinkCondition = Nothing
			Dim oInsertCondition As InsertCondition = Nothing
			If bLinkFilterExists Then
				oLinkCondition = New LinkCondition(sLinkLayers, sLineLayers)
			End If
			If bBlockRefFilterExists Then
				oInsertCondition = New InsertCondition(False, sBlockName, sBlockRefLayers)
			End If


			Dim iLinkType As enLinkType
			Dim iInsertType As Integer
			Dim oEntity As Entity
			Dim bCond As Boolean

			Dim iTest As Integer
			Try
				'	MessageBox.Show(CStr(bLinkFilterExists) & ":" & CStr(bInsertFilterExists), "02_760")
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					bCond = False
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
					If bLinkFilterExists Then
						iLinkType = oLinkCondition.GetLinkType(oEntity)
						If iLinkType <> enLinkType.OtherLayer AndAlso iLinkType <> enLinkType.SourceLayerArc Then '	i.e.	SourceLayerLine SourceLayerAny LineLayer
							bCond = True

						End If
					End If
					If Not bCond AndAlso bBlockRefFilterExists Then
						iInsertType = oInsertCondition.GetInsertType(oEntity)
						'	AcadDocument.WriteMessage("!!!237 " & CStr(iInsertType))
						If iInsertType <> -1 Then
							bCond = True
						End If
					End If
					If bCond Then
						iTest += 1
					End If
					dlEntityProc(oEntity, bCond)
				Next
				'	System.Windows.Forms.MessageBox.Show(CStr(iLinkCount) & vbCrLf & CStr(iLineCount), "04_120")
				'	System.Windows.Forms.MessageBox.Show(CStr(iTest) & vbCrLf & "", "04_177")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1443")
			End Try
		End If
	End Sub
	Public Shared Sub AddToLayer(colAcadObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection, sLayer As String)
		Dim oEntity As Entity
		Dim oEntityClone As Entity

		'	DMCommon.Debug.MsgBox("13_035u", colAcadObjIDs.Count, DMAcadExt.AcadDocument.IsLocked, DMAcadExt.AcadDocument.DocumentLockMode(), DMAcadExt.AcadTransaction.TransactionExists)

		If True Then
			DMAcadExt.AcadTransaction.CreateLayer(sLayer)
			For Each tAcObjID As ObjectId In colAcadObjIDs
				'DMCommon.ExcelLogAW5.SetNextValue(0, "tAcObjID", tAcObjID)
				Try

					oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

					If oEntity IsNot Nothing Then

						oEntityClone = TryCast(oEntity.Clone(), Entity)

						If oEntityClone IsNot Nothing Then
							oEntityClone.XData = New ResultBuffer()
							oEntityClone.Layer = sLayer
							oEntityClone.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 0S)
							DMAcadExt.AcadTransaction.AppendEntity(oEntityClone)
						Else

							System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadTransaction.TransactionExists & vbCrLf & tAcObjID.ToString(), "Ex #143")
						End If
					Else
						System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadTransaction.TransactionExists & vbCrLf & tAcObjID.ToString(), "Ex #144")
					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oEx.ToString() & vbCrLf & oEx.GetType().ToString() & vbCrLf & tAcObjID.ToString(), "Ex #148")
				End Try

			Next tAcObjID
		End If
	End Sub
	Public Shared Sub SetExceptionLayer(colAcadObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		Dim oEntity As Entity
		Dim oEntityClone As Entity
		Dim sTest As String = "a"
		'	DMCommon.Debug.MsgBox("13_035E", colAcadObjIDs.Count, DMAcadExt.AcadDocument.IsLocked, DMAcadExt.AcadDocument.DocumentLockMode(), DMAcadExt.AcadTransaction.TransactionExists)

		If True Then
			DMAcadExt.AcadTransaction.CreateLayer(msExceptionLayer)
			For Each tAcObjID As ObjectId In colAcadObjIDs
				'DMCommon.ExcelLogAW5.SetNextValue(0, "tAcObjID", tAcObjID)
				Try
					sTest = "b"
					oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					sTest = "c"
					If oEntity IsNot Nothing Then
						sTest = "d"
						oEntityClone = TryCast(oEntity.Clone(), Entity)
						sTest = "e"
						If oEntityClone IsNot Nothing Then
							sTest = "f"
							oEntityClone.Layer = msExceptionLayer
							sTest = "g"
							DMAcadExt.AcadTransaction.AppendEntity(oEntityClone)
							sTest = "h"
						Else
							sTest = "i"
							System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadTransaction.TransactionExists & vbCrLf & tAcObjID.ToString(), "Ex #143")
						End If
					Else
						sTest = "j"
						System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadTransaction.TransactionExists & vbCrLf & tAcObjID.ToString(), "Ex #144")
					End If

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oEx.ToString() & vbCrLf & oEx.GetType().ToString() & vbCrLf & tAcObjID.ToString(), "Ex #142")
				End Try

			Next tAcObjID
		End If

	End Sub
	Public Shared Sub SetFreezeLayer(colAcadObjIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection)
		Dim oEntity As Entity

		DMCommon.Debug.MsgBox("13_035Freeze", colAcadObjIDs.Count)
		DMAcadExt.AcadTransaction.CreateLayer(msFreezeLayer)
		For Each tAcObjID As ObjectId In colAcadObjIDs
			Try
				oEntity = DMAcadExt.AcadTransaction.GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				oEntity.Layer = msFreezeLayer

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1438")
			End Try

		Next
	End Sub
	Public Shared Sub ProcByList(ByVal dlEntityProc As EntityProc, colEntitiesIds As ObjectIdCollection)
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim oEntity As Entity
			Dim bCond As Boolean

			Try

				For Each tAcObjId As ObjectId In moModelSpaceTableRecord

					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
					bCond = colEntitiesIds.Contains(tAcObjId)
					dlEntityProc(oEntity, bCond)
				Next
				'	System.Windows.Forms.MessageBox.Show(CStr(iLinkCount) & vbCrLf & CStr(iLineCount), "04_120")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1443")
			End Try
		End If
	End Sub
	Public Shared Sub ProcAll(ByVal dlEntityProc As EntityProc, ByVal bCond As Boolean)
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim oEntity As Entity

			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
					dlEntityProc(oEntity, bCond)

				Next

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1447")
			End Try
		End If
	End Sub
	Public Shared Sub Zoom(ByVal dicObjIDs As ObjectIdCollection, ByVal bHighlight As Boolean)
		Dim oBoundingBox As TPlnBoundingBox = New TPlnBoundingBox()
		For Each tAcObjID As ObjectId In dicObjIDs
			oBoundingBox.Union(GetBoundingBox(tAcObjID, bHighlight))
		Next
		AcadDocument.Zoom(oBoundingBox)
	End Sub
	Public Shared Sub MoveBlockRef(oBlockRef As BlockReference, tNewPoint As Point3d)
		Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection
		Dim tBlockRefPosition As Point3d = oBlockRef.Position
		Dim tVector As Vector3d = tNewPoint - tBlockRefPosition
		Dim oAttribDBobject As DBObject
		Dim oAttribRef As AttributeReference
		oBlockRef.Position = tNewPoint
		For Each tAttrObjID As ObjectId In colAttributes
			oAttribDBobject = GetDBObject(tAttrObjID, OpenMode.ForWrite)
			oAttribRef = DirectCast(oAttribDBobject, AttributeReference)
			oAttribRef.Position = oAttribRef.Position + tVector
			If oAttribRef.HorizontalMode <> TextHorizontalMode.TextLeft OrElse oAttribRef.VerticalMode <> TextVerticalMode.TextBase Then
				oAttribRef.AlignmentPoint = oAttribRef.AlignmentPoint + tVector
			End If
			'	oAttribDBobject.ViewportDraw(
		Next
		oBlockRef.UpgradeOpen()
		oBlockRef.Database.Regenmode = True
		'oBlockRef.Database.
   End Sub
	Public Shared Sub SetLayer(colAcadObjIDs As ObjectIdCollection, ByVal sLayer As String, Optional bPrintHandle As Boolean = False)
		Dim oEntity As Entity
		Dim iCounter As Integer
		For Each tAcObjID As ObjectId In colAcadObjIDs
			oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
			If bPrintHandle Then
				iCounter += 1
				AcadDocument.WriteMessage("SelSet Handle: " & iCounter.ToString() & " - " & oEntity.Handle.ToString)
			End If
			Try
				oEntity.Layer = sLayer
			Catch oEx As Exception

			End Try

		Next



	End Sub






















	Public Shared Sub SetVisible(colAcadObjIDs As ObjectIdCollection, ByVal bVisible As Boolean)
      If colAcadObjIDs IsNot Nothing Then
         Dim oEntity As Entity

         For Each tAcObjID As ObjectId In colAcadObjIDs
            oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
				Try
					oEntity.Visible = bVisible
				Catch oEx As Exception
					AcadDocument.WriteMessage("#347: " & oEx.Message)
				End Try
         Next
      End If
   End Sub
   Public Shared Sub SetVisible(sLayer As String, colExceptAcadObjIDs As ObjectIdCollection, ByVal bVisible As Boolean)
      Dim bExceptNotExists As Boolean = (colExceptAcadObjIDs Is Nothing) OrElse (colExceptAcadObjIDs.Count = 0)
      Dim oEntity As Entity
      Dim iTest As Integer = 0
      For Each tAcObjID As ObjectId In moModelSpaceTableRecord
         oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
         If oEntity.Layer = sLayer AndAlso (bExceptNotExists OrElse Not colExceptAcadObjIDs.Contains(tAcObjID)) Then
            Try
               oEntity.Visible = bVisible
               iTest += 1

            Catch oEx As Exception

            End Try
         End If

      Next
      '    DMCommon.Debug.MsgBox("12_320", sLayer, colExceptAcadObjIDs.Count, iTest, bVisible)
   End Sub
	Public Shared Sub SetColor(colAcadObjIDs As ObjectIdCollection, ByVal tColor As Autodesk.AutoCAD.Colors.Color, Optional iHighLight As TriState = TriState.UseDefault)
		Dim oEntity As Entity

		For Each tAcObjID As ObjectId In colAcadObjIDs
			oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
			Try
				oEntity.Color = tColor
				If iHighLight = TriState.True Then
					oEntity.Highlight()
				ElseIf iHighLight = TriState.FALSE Then
					oEntity.Unhighlight()
				End If


			Catch oEx As Exception

			End Try
		Next
	End Sub
	Public Shared Sub SetColor(tAcObjID As ObjectId, ByVal tColor As Autodesk.AutoCAD.Colors.Color)
      Dim oEntity As Entity


      oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
      Try
         oEntity.Color = tColor

		Catch oEx As Exception

      End Try

   End Sub
   Public Shared Function GetVisibleAttributes(tBlockRefObjID As ObjectId) As ObjectIdCollection
      Dim bAcadPoint As Boolean
      Dim oBlockRef As BlockReference = GetBlockRefForRead(tBlockRefObjID, True, bAcadPoint)
      Dim dicResAttribRefIDs As ObjectIdCollection = New ObjectIdCollection()
      If oBlockRef IsNot Nothing Then
         Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection()
         'Dim tAttribObjID As ObjectId
         Dim oAttribRef As AttributeReference
         For Each tAttribObjID As ObjectId In colAttributes
            oAttribRef = GetAttribRef(tAttribObjID, OpenMode.ForRead)
            If Not oAttribRef.Invisible Then
               dicResAttribRefIDs.Add(tAttribObjID)
            End If
         Next
      End If
      Return dicResAttribRefIDs

   End Function
   Public Shared Sub MoveToTopOrder(tAcObjID As ObjectId)
      Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim colObjectIDs As ObjectIdCollection = New ObjectIdCollection()
      Try
         tDrawOrderTableID = moModelSpaceTableRecord.DrawOrderTableId
         moDrawOrderTable = DirectCast(moTransaction.GetObject(tDrawOrderTableID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DrawOrderTable)
         colObjectIDs.Add(tAcObjID)
         moDrawOrderTable.MoveToTop(colObjectIDs)
			' DMCommon.Debug.MsgBox("12_360", colObjectIDs.Count)
			DBObjectInfo(tAcObjID, "MoveToTop: ")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.GetType().ToString & vbCrLf & oEx.StackTrace, "04_788d")
		End Try
     
      moDrawOrderTable = Nothing
   End Sub
   Public Shared Sub SetAttributesByBlock(tBlockRefObjID As ObjectId)

      Dim oBlockRef As BlockReference = GetBlockRef(tBlockRefObjID, OpenMode.ForWrite, False)
      '    Dim dicResAttribRefIDs As ObjectIdCollection = New ObjectIdCollection()
      If oBlockRef IsNot Nothing Then
         Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection()
         'Dim tAttribObjID As ObjectId
         Dim oAttribRef As AttributeReference
         For Each tAttribObjID As ObjectId In colAttributes
            oAttribRef = GetAttribRef(tAttribObjID, OpenMode.ForWrite)
            If Not oAttribRef.Invisible Then
               oAttribRef.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByBlock, 0S)
               '  DMCommon.Debug.MsgBox("12_330", oAttribRef.Color, oAttribRef.Color.IsByBlock, oAttribRef.Color.ColorIndex, oAttribRef.Color.ColorMethod)
               '   dicResAttribRefIDs.Add(tAttribObjID)
            End If
         Next
      End If
      '  Return dicResAttribRefIDs

   End Sub
   Public Shared Sub SetLayer(tAcObjID As ObjectId, ByVal sLayer As String)
      Dim oEntity As Entity

      oEntity = zzGetEntity(tAcObjID, OpenMode.ForWrite)
      Try
         oEntity.Layer = sLayer 
      Catch oEx As Exception

      End Try
   End Sub
	Public Shared Function GetSingleBlockRef(ByVal sBlockName As String, ByRef tBlockRefObjID As ObjectId, ByRef tBlockRecObjID As ObjectId, Optional ByRef oaAttribDefs() As AttributeDefinition = Nothing) As Integer


		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		'		Dim oBlockAcObjId As ObjectId
		Dim oOutBlockCol As ObjectIdCollection
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim iAttribDefIndex As Integer = -1
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			DMAcadExt.AcadDocument.UpdateScreen()
			oOutBlockCol = New ObjectIdCollection()
			System.Windows.Forms.MessageBox.Show(CStr(oBlockTable Is Nothing) & vbCrLf & oEx.GetType().ToString & vbCrLf & oEx.Message, "04_788c")
		End Try


		'	System.Windows.Forms.MessageBox.Show(CStr(oBlockTable Is Nothing), "04_790")
		If oBlockTable IsNot Nothing Then
			If oBlockTable.Has(sBlockName) Then
				Try
					'System.Windows.Forms.MessageBox.Show(sBlockName, "119")
					tBlockRecObjID = oBlockTable.Item(sBlockName)
					oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockRecObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)


					If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
						For Each tObjID As ObjectId In oBlockTableRecord
							oDBObject = moTransaction.GetObject(tObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
							sRXClassName = oDBObject.GetRXClass().Name
							If sRXClassName = AcadConst.AcadAttributeDefName Then
								iAttribDefIndex += 1
								ReDim Preserve oaAttribDefs(iAttribDefIndex)
								oaAttribDefs(iAttribDefIndex) = DirectCast(oDBObject, AttributeDefinition)
							End If
						Next


					End If






					Dim oAllBlockCol As ObjectIdCollection

					oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, True)
					'System.Windows.Forms.MessageBox.Show(sBlockName, "04_119")
					'''''Temp Test

					'	System.Windows.Forms.MessageBox.Show(CStr(oAllBlockCol.Count), sBlockName)

					If oAllBlockCol.Count > 0 Then
						tBlockRefObjID = oAllBlockCol.Item(0)
					End If
					Return oAllBlockCol.Count

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "e200a")

					oOutBlockCol = New ObjectIdCollection()
					Return -1
				End Try
			Else
				tBlockRecObjID = ObjectId.Null

				Return -1
			End If
		Else
			System.Windows.Forms.MessageBox.Show("BlockTable Is Nothing", "AcadTransaction - zzGetBlockRefs")
		End If



   End Function
	Public Shared Function GetAcadPoint(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As DBPoint
		Const iExNo As Integer = 80
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			If oDBObject Is Nothing Then
				Return Nothing
			Else
				Return DirectCast(oDBObject, DBPoint)
			End If

		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetAcadTable(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As Table
		Const iExNo As Integer = 120
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			If oDBObject Is Nothing Then
				Return Nothing
			Else
				Return DirectCast(oDBObject, Table)
			End If

		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetBlockRef(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode, Optional bMust As Boolean = True) As BlockReference
      Const iExNo As Integer = 10
		Dim oDBObject As DBObject = Nothing
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)

			If oDBObject Is Nothing Then
				DMCommon.Debug.MsgBox("09_341", tAcObjID, "oDBObject Is Nothing")
				Return Nothing
			ElseIf oDBObject.IsErased Then
				DMCommon.Debug.MsgBox("09_342", tAcObjID, "oDBObject Is Erased")
				Return Nothing
			ElseIf oDBObject.GetRXClass().Name = AcadConst.AcadBlockRefName Then
				Return DirectCast(oDBObject, BlockReference)
			ElseIf bMust Then
				DMCommon.Debug.MsgBox("09_559f", tAcObjID, oDBObject.GetRXClass().Name, iOpenMode.ToString())
				Return Nothing
			Else
				'DMCommon.Debug.MsgBox("09_559h", tAcObjID, oDBObject.GetRXClass().Name, iOpenMode.ToString())
				Return Nothing
			End If
      Catch oEx As System.Exception
			DMCommon.Debug.MsgBox("09_559b", tAcObjID, oEx.Message, oEx.StackTrace, iOpenMode.ToString(), oDBObject Is Nothing)
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
         Return Nothing
      End Try
   End Function
	Public Shared Function GetBlockTableRecord(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As BlockTableRecord
		Const iExNo As Integer = 70
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			Return DirectCast(oDBObject, BlockTableRecord)
		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetAttribRef(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As AttributeReference
		Const iExNo As Integer = 60
		Dim oDBObject As DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			Return DirectCast(oDBObject, AttributeReference)
		Catch oEx As System.Exception
			AcadDocument.WriteException(miErrClassNo, iExNo, oEx.Message)
			Return Nothing
		End Try
	End Function
   Public Shared Function GetLink(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode, Optional bCheckRXClass As Boolean = False) As IUD_Link
      Const iExNo As Integer = 21
      Dim oDBObject As DBObject = Nothing

      Try
         oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
         Select Case oDBObject.GetRXClass().Name
            Case AcadConst.AcadLineName
               Return New TplnLine(DirectCast(oDBObject, Line))
            Case AcadConst.AcadArcName
               Return New TplnArc(DirectCast(oDBObject, Arc))
				Case AcadConst.AcadPolylineName
					Return New TplnArc(DirectCast(oDBObject, Polyline))
				Case Else
               Return Nothing
         End Select
         


      Catch oEx As System.Exception
         Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace
         If moTransaction Is Nothing Then
            sMsg &= vbCrLf & "Transaction is nothing"
         ElseIf oDBObject Is Nothing Then
            sMsg &= vbCrLf & "ObjectId=" & tAcObjID.ToString() & " was not found"
         Else
            sMsg &= vbCrLf & "Invalid Line Object Type: " & oDBObject.GetRXClass().Name
         End If
         AcadDocument.WriteException(miErrClassNo, iExNo, sMsg)
         Return Nothing
      End Try
   End Function
	Public Shared Function GetPolyline(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode, Optional bCheckRXClass As Boolean = False) As Polyline
		Const iExNo As Integer = 21
		Dim oDBObject As DBObject = Nothing

		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			If bCheckRXClass AndAlso oDBObject.GetRXClass().Name <> AcadConst.AcadPolylineName Then
				Return Nothing
			Else
				Return DirectCast(oDBObject, Polyline)
			End If


		Catch oEx As System.Exception
			Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace
			If oDBObject Is Nothing Then
				sMsg &= vbCrLf & "ObjectId=" & tAcObjID.ToString() & " was not found"
			Else
				sMsg &= vbCrLf & "Invalid Polyline Object Type: " & oDBObject.GetRXClass().Name
			End If
			AcadDocument.WriteException(miErrClassNo, iExNo, sMsg)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetUD_Link(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode, ByRef oLine As Line, ByRef oPolyline As Polyline) As Boolean
		Dim oDBObject As DBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
		DMCommon.Debug.ExcelLog.SetNextValue(2, "!GetRXClass", oDBObject.GetRXClass().Name)
		Select Case oDBObject.GetRXClass().Name
			Case AcadConst.AcadLineName
				oLine = DirectCast(oDBObject, Line)
				oPolyline = Nothing
				Return True
			Case AcadConst.AcadPolylineName
				oPolyline = DirectCast(oDBObject, Polyline)
				oLine = Nothing
				Return True
			Case Else
				Return False
		End Select

	End Function
	Public Shared Function GetCurve(ByVal tAcObjID As ObjectId, ByVal bCheck As Boolean, ByVal iOpenMode As OpenMode, Optional ByVal sLayersDel As String = msEmpty) As Curve
		Const iExNo As Integer = 21
		Dim oDBObject As DBObject = Nothing
		Dim oCurve As Curve
		Dim sCurveLayerName As String
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			If bCheck Then
				Select Case oDBObject.GetRXClass().Name
					Case AcadConst.AcadPolylineName, AcadConst.AcadLineName, AcadConst.AcadArcName, AcadConst.Acad2dPolylineName
						oCurve = DirectCast(oDBObject, Curve)

						If sLayersDel.Length = 0 Then
							Return oCurve
						Else
							sCurveLayerName = TopoDef.AddDelim(oCurve.Layer)
							If sLayersDel.Contains(sCurveLayerName) Then
								Return oCurve
							Else
								Return Nothing
							End If
						End If
					Case Else
						Return Nothing
				End Select
			Else
				Return DirectCast(oDBObject, Curve)
			End If
		Catch oEx As System.Exception
			Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace
			If oDBObject Is Nothing Then
				sMsg &= vbCrLf & "ObjectId=" & tAcObjID.ToString() & " was not found"
			Else
				sMsg &= vbCrLf & "Invalid Curve Object Type: " & oDBObject.GetRXClass().Name
			End If
			AcadDocument.WriteException(miErrClassNo, iExNo, sMsg)
			Return Nothing
		End Try
	End Function
	Public Shared Function GetAllBlockRefInsPoint(ByVal sBlockName As String) As TplnPointArray
		Dim colBlockRefObjIDs As ObjectIdCollection = GetAllBlockRefs(sBlockName, String.Empty)
		Dim iIndex As Integer = 0

		If colBlockRefObjIDs IsNot Nothing Then
			Dim oResTplnPointArray As TplnPointArray = New TplnPointArray(colBlockRefObjIDs.Count - 1)
			Dim oBlockRef As BlockReference
			For Each tAcObjID As ObjectId In colBlockRefObjIDs
				oBlockRef = GetBlockRef(tAcObjID, OpenMode.ForRead)
				If oBlockRef IsNot Nothing Then
					oResTplnPointArray.Add(oBlockRef.Position, tAcObjID, iIndex)
				Else
					AcadDocument.WriteMessage("!!!oBlockRef Is Nothing")
				End If
				iIndex += 1
			Next
			Return oResTplnPointArray
		Else
			Return Nothing
		End If

   End Function


	Public Shared Function GetAllBlockRefsForAttach(ByVal sBlockName As String) As ICollection(Of AttachPair)	'TEMP
		Dim colBlockRefObjIDs As ObjectIdCollection = GetAllBlockRefs(sBlockName, String.Empty)
		Dim iIndex As Integer = 0
		Dim colAttachedPairs As ICollection(Of AttachPair) = New System.Collections.ObjectModel.Collection(Of AttachPair)
		Dim oAttachPair As AttachPair
		If colBlockRefObjIDs IsNot Nothing Then
			Dim oResTplnPointArray As TplnPointArray = New TplnPointArray(colBlockRefObjIDs.Count - 1)
			Dim oBlockRef As BlockReference
			For Each tAcObjID As ObjectId In colBlockRefObjIDs
				'MessageBox.Show(tObjID.GetType().ToString())
				'Return Nothing
				oBlockRef = GetBlockRef(tAcObjID, OpenMode.ForRead)
				If oBlockRef IsNot Nothing Then
					oAttachPair = New AttachPair(New TPlnPoint(oBlockRef.Position), tAcObjID)
					colAttachedPairs.Add(oAttachPair)
				Else
					AcadDocument.WriteMessage("!!!oBlockRef Is Nothing")
				End If
				iIndex += 1
			Next
			Return colAttachedPairs
		Else
			Return Nothing
		End If

	End Function
	Public Shared Function GetAllBlockRefs(ByVal sBlockName As String, Optional ByVal sLayers As String = Nothing) As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim tBlockAcObjId As ObjectId
		Dim colAllBlockCol As ObjectIdCollection = Nothing
		Dim colAllBlockColByLayer As ObjectIdCollection = Nothing
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAllBlockRefs")
		End Try
		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try
				tBlockAcObjId = oBlockTable.Item(sBlockName)

				oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				colAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
				'  DMCommon.Debug.MsgBox("13_004", DMCommon.Debug.ColCount(colAllBlockCol))

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetAllBlockRefs")
				colAllBlockCol = Nothing
			End Try
		Else
			Return New ObjectIdCollection()
		End If
		If Not String.IsNullOrEmpty(sLayers) AndAlso colAllBlockCol IsNot Nothing AndAlso colAllBlockCol.Count <> 0 Then
			Dim oEntity As Entity
			Dim sLayersDel As String = TopoDef.AddDelim(sLayers)
			Dim sBlockRefLayerDel As String
			colAllBlockColByLayer = New ObjectIdCollection()
			For Each tBlockAcObjId In colAllBlockCol
				oEntity = zzGetEntity(tBlockAcObjId)
				sBlockRefLayerDel = TopoDef.AddDelim(oEntity.Layer)
				If sLayersDel.Contains(sBlockRefLayerDel) Then
					colAllBlockColByLayer.Add(tBlockAcObjId)
				End If
			Next
			Return colAllBlockColByLayer
		ElseIf colAllBlockCol Is Nothing Then
			Return New ObjectIdCollection()
		Else
			Return colAllBlockCol
		End If
	End Function
	Public Shared Function GetBlockRefsDic(ByVal sBlockName As String, iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode, Optional ByVal sLayers As String = msEmpty) As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference)

		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim tBlockAcObjId As ObjectId
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Dim colAllBlockCol As ObjectIdCollection = Nothing
		Dim colAllBlockColByLayer As ObjectIdCollection = Nothing
		Dim oResDict As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference) = New System.Collections.Generic.Dictionary(Of ObjectId, BlockReference)
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAllBlockRefs")
		End Try
		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try
				tBlockAcObjId = oBlockTable.Item(sBlockName)
				oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				colAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetAllBlockRefs")
				colAllBlockCol = Nothing
			End Try
		End If
		If colAllBlockCol IsNot Nothing Then


			Dim oBlockRef As BlockReference
			Dim tLayersList As DMCommon.dmList = New DMCommon.dmList(sLayers)

			For Each tAcObjId As ObjectId In colAllBlockCol
				oDBObject = GetDBObject(tAcObjId, iMode)
				oBlockRef = DirectCast(oDBObject, BlockReference)
				If (Not tLayersList.Exists) OrElse tLayersList.Contains(oBlockRef.Layer) AndAlso (oBlockRef IsNot Nothing) Then
					oResDict.Add(tAcObjId, oBlockRef)
				End If
			Next
		End If
		Return oResDict
	End Function
	Public Shared Sub ScanLayer(ByVal sLayerName As String)
		Dim oEntity As Entity
		Dim sRXClassName As String
		Dim iCount As Integer
		Dim iAllCount As Integer = 0
		Dim dicEntityCounter As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
		'		Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
		For Each tAcObjID As ObjectId In moModelSpaceTableRecord

			oEntity = zzGetEntity(tAcObjID)
			If oEntity IsNot Nothing AndAlso oEntity.Layer = sLayerName Then
				iAllCount += 1
				sRXClassName = oEntity.GetRXClass().Name
				If sRXClassName = "CAdeTopView" Then
					Try
						AcadDocument.WriteMessage("CAdeTopView - " & oEntity.GetType().ToString())
					Catch oEx As Exception
						AcadDocument.WriteMessage("Ex1:" & oEx.Message)
					End Try
					Try
						AcadDocument.WriteMessage("CAdeTopView Color - " & oEntity.Color.ToString())
					Catch oEx As Exception
						AcadDocument.WriteMessage("Ex2:" & oEx.Message)
					End Try
					Try
						AcadDocument.WriteMessage("Layer - " & oEntity.Layer)
					Catch oEx As Exception
						AcadDocument.WriteMessage("Ex3:" & oEx.Message)
					End Try
					Dim oExt As Extents3d
					Try
						oExt = oEntity.GeometricExtents()
						AcadDocument.WriteMessage("Extents - " & oExt.ToString())
					Catch oEx As Exception
						AcadDocument.WriteMessage("Ex3:" & oEx.Message)
					End Try

					'oEntity.SaveAs(
				End If
				If dicEntityCounter.ContainsKey(sRXClassName) Then
					iCount = dicEntityCounter.Item(sRXClassName)
					dicEntityCounter.Remove(sRXClassName)
				Else
					iCount = 0
				End If
				dicEntityCounter.Add(sRXClassName, iCount + 1)
			End If
		Next
		AcadDocument.WriteMessage("All Objects - " & CStr(iAllCount))
		For Each oKeyValuePair As KeyValuePair(Of String, Integer) In dicEntityCounter
			AcadDocument.WriteMessage(oKeyValuePair.Key & " - " & CStr(oKeyValuePair.Value))
		Next
	End Sub
	Public Shared Sub GetAllBlocks()	' As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim tBlockAcObjId As ObjectId

		Dim oSymbTableEnum As SymbolTableEnumerator
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAllBlocks")
		End Try

		Try
			oSymbTableEnum = oBlockTable.GetEnumerator()
			oSymbTableEnum.Reset()

			Do While oSymbTableEnum.MoveNext()
				tBlockAcObjId = oSymbTableEnum.Current
				oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				AcadDocument.WriteMessage("Block=" & oBlockTableRecord.Name & ":" & CStr(oBlockTableRecord.IsAnonymous) & ":" & CStr(oBlockTableRecord.Explodable) & ":" & CStr(oBlockTableRecord.PathName) & ";")
			Loop

      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
		End Try
	End Sub



   Private Shared Function zzGetLayer(ByVal oLayerDef As AcadLayerDef, ByVal bCreate As Boolean, ByVal bClear As Boolean, ByVal bCheckIsOn As Boolean, Optional iExtensionNumber As Integer = 0) As ObjectId
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oLayerTable As LayerTable = Nothing
      Dim oLayerTableRecord As LayerTableRecord
      Dim tLayerAcObjId As ObjectId = New ObjectId()
      Dim sLayer As String
      Dim iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode

      If bCreate Then
         iMode = OpenMode.ForWrite
      Else
         iMode = OpenMode.ForRead
      End If
      If oLayerDef.Correct Then
         If iExtensionNumber > 0 Then
            sLayer = oLayerDef.NamePlusExtension(iExtensionNumber)
         Else
            sLayer = oLayerDef.Name
         End If


         Try
            oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, iMode), LayerTable)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "Mode=" & iMode.ToString(), "zzGetLayer_7")
            Return tLayerAcObjId
         End Try

         If oLayerTable.Has(sLayer) Then
            tLayerAcObjId = oLayerTable.Item(sLayer)
            If bClear Then
               ClearLayerByClassName(sLayer, String.Empty)
            End If
            If bCheckIsOn Then
               Try
                  oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForWrite), LayerTableRecord)
                  If oLayerTableRecord.IsOff Then
                     Dim sMsg As String = "The current layer turned off." & vbCrLf & "Turn on the current layer?" & vbCrLf

                     Dim iRes As System.Windows.Forms.DialogResult = System.Windows.Forms.MessageBox.Show(sMsg, "AppID=" & CStr(oLayerDef.AppID), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
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

            oLayerTableRecord = New LayerTableRecord()
            oLayerTableRecord.Name = sLayer

            If oLayerDef.Exists Then
               If oLayerDef.AcadColor IsNot Nothing Then
                  oLayerTableRecord.Color = oLayerDef.AcadColor
               End If
               If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
                  oLayerTableRecord.LineWeight = oLayerDef.LineWeight
               End If
               If oLayerTableRecord.IsOff = oLayerDef.On Then
                  oLayerTableRecord.IsOff = Not oLayerDef.On
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
#Disable Warning IDE1006 ' Naming Styles
	Private Shared Function zzGetLayer(ByRef sLayer As String, ByVal iAppID As Integer, ByVal iLayerFunction As enLayerFunction, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean, Optional sTemplate As String = Nothing) As ObjectId
#Enable Warning IDE1006 ' Naming Styles
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
		If String.IsNullOrEmpty(sLayer) Then
			oLayerDef = New AcadLayerDef(iAppID, iLayerFunction)
			If oLayerDef.Correct Then
				sLayer = oLayerDef.Name
			Else
				Return ObjectId.Null
			End If
		End If
		If sLayer IsNot Nothing AndAlso sLayer.Length <> 0 Then
			If oLayerTable.Has(sLayer) Then
				tLayerAcObjId = oLayerTable.Item(sLayer)

				If bCheckIsOn Then
					Try
						oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForWrite), LayerTableRecord)
						If oLayerTableRecord.IsFrozen Then
							oLayerTableRecord.IsFrozen = False
						End If
						If oLayerTableRecord.IsOff Then
							Dim sMsg As String = "The current layer turned off." & vbCrLf & "Turn on the current layer?" & vbCrLf & sLayer

							Dim iRes As System.Windows.Forms.DialogResult = DialogResult.Yes 'System.Windows.Forms.MessageBox.Show(sMsg, "AppID=" & CStr(iAppID), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly, False)
							Select Case iRes
								Case DialogResult.Cancel
									Return New ObjectId()
								Case DialogResult.Yes
									oLayerTableRecord.IsOff = False
							End Select
							'   System.Windows.Forms.MessageBox.Show(oLayerTableRecord.IsOff.ToString(), "29_069:IsOff")
						End If

					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - zzGetLayer")
					End Try
				End If



			ElseIf bCreate Then
				If Not oLayerDef.Exists Then
					oLayerDef = New AcadLayerDef(iAppID, sLayer, sTemplate)
				End If
				If oLayerDef.Correct Then


					oLayerTableRecord = New LayerTableRecord()
					Try
						oLayerTableRecord.Name = sLayer
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "'" & sLayer & "'", "AcadTransaction - zzGetLayer")
					End Try


					If oLayerDef.Exists Then
						' DMCommon.Debug.MsgBox("08_230", True, oLayerDef.AcadColor.ToString(), iAppID, sLayer)
						If oLayerDef.AcadColor IsNot Nothing Then
							oLayerTableRecord.Color = oLayerDef.AcadColor
						End If
						If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
							oLayerTableRecord.LineWeight = oLayerDef.LineWeight
						End If
						If oLayerTableRecord.IsOff = oLayerDef.On Then
							oLayerTableRecord.IsOff = Not oLayerDef.On
						End If
					End If
					oLayerTable.UpgradeOpen()
					tLayerAcObjId = oLayerTable.Add(oLayerTableRecord)
					moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
				End If
			End If
		End If

		oLayerTable = Nothing
		Return tLayerAcObjId
	End Function

	Public Shared Function GetCPStatistics(tMapThemeData As DMAcadExt.MapThemeData) As MapThemeInfo
		Dim tMapThemeInfo As MapThemeInfo

		If moModelSpaceTableRecord IsNot Nothing Then
			Dim oEntity As Entity
			Dim sRXClassName As String
			Dim sEntityLayer As String
			Dim tPolylineLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.LinkLayers)
			'Dim tLineLinkLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.LineLinkLayers)
			Dim tCentroidLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidLayers)
			Dim tCentroidBlocksList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidBlocks)
			'MessageBox.Show(tMapThemeData.CentroidBlocks & vbCrLf & tMapThemeData.CentroidLayers, "06_320")
			Dim tMPgonLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.MPgonLayers)
			Dim oXDataParcel As TplnXDataParcel
			Dim tMPgons_ID As ClosedPgonID
			Dim tBlockRefs_ID As ClosedPgonID

			tMapThemeInfo.Reset()

			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				oEntity = Nothing
				Try
					'		oEntity = DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					oEntity = GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) ' DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					If oEntity IsNot Nothing Then
						sRXClassName = oEntity.GetRXClass().Name
						sEntityLayer = oEntity.Layer
						'AcadDocument.WriteMessage("_11 " & sRXClassName & "," & sEntityLayer & "," & oEntity.Handle.ToString())	'
						If LinkCondition.IsLink(sRXClassName) Then
							If tPolylineLayersList.Contains(sEntityLayer) Then
								tMapThemeInfo.AddLink(1)
								'tMapThemeInfo.AddArcCount(dmLineCleanup.GetArcCountByClass(oEntity, sRXClassName))
							Else
								'	AcadDocument.WriteMessage("_NOT 13 " & sRXClassName & "," & sEntityLayer & "," & oEntity.Handle.ToString())
							End If
							'End If
						ElseIf tCentroidLayersList.Contains(sEntityLayer) Then
							If sRXClassName = AcadConst.AcadBlockRefName Then
								If tCentroidBlocksList.Contains(zzGetBlockName(oEntity)) Then
									tMapThemeInfo.AddCentroidBlocks(1)
									oXDataParcel = New TplnXDataParcel(oEntity.XData)
									tMPgons_ID.AddID(oXDataParcel.DataID)
								End If
							End If
						ElseIf tMPgonLayersList.Contains(sEntityLayer) Then
							If sRXClassName = AcadConst.AcadMPolygonName Then
								tMapThemeInfo.AddMpgons(1)
								oXDataParcel = New TplnXDataParcel(oEntity.XData)
								tBlockRefs_ID.AddID(oXDataParcel.DataID)
							End If

						End If
					Else
						AcadDocument.WriteMessage("#129AcadTrans " & tAcObjID.ToString())
					End If

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByClassName_3")

				End Try
			Next
			If tBlockRefs_ID = tMPgons_ID AndAlso tMapThemeInfo.LinkCount = tMPgons_ID.Count AndAlso tMapThemeInfo.CentroidBlocksCount = tMPgons_ID.Count Then
				tMapThemeInfo.PgonSetCount = tMPgons_ID.Count
				tMapThemeInfo.PgonSetIsCorrect = True
			End If

			'	MessageBox.Show(tMapThemeInfo.LinkCount & vbCrLf & tMapThemeInfo.LinkWithArcCount & vbCrLf & tMapThemeInfo.LineLinkCount & vbCrLf & tMapThemeInfo.CentroidBlocksCount, "03_443")
		End If
		'	MessageBox.Show(CStr(tMapThemeInfo.CentroidBlocksCount) & vbCrLf & "", "06_321")
		Return tMapThemeInfo
	End Function

	Public Shared Function GetStatistics(tMapThemeData As DMAcadExt.MapThemeData) As MapThemeInfo
		Dim tMapThemeInfo As MapThemeInfo

		If moModelSpaceTableRecord IsNot Nothing Then
			Dim oEntity As Entity
			Dim sRXClassName As String
			Dim sEntityLayer As String
			Dim tLinkLayersList As DMCommon.dmList
			Dim tLineLinkLayersList As DMCommon.dmList


			If tMapThemeData.GraphType = enGraphType.Topology Then
				tLinkLayersList = New DMCommon.dmList(tMapThemeData.LinkLayers)
				tLineLinkLayersList = New DMCommon.dmList(tMapThemeData.LineLinkLayers)
			ElseIf tMapThemeData.GraphType = enGraphType.ClosedPolygons Then
				tLinkLayersList = New DMCommon.dmList(tMapThemeData.ClosedPgonsLayers)
				tLineLinkLayersList = New DMCommon.dmList(String.Empty)

			Else
				Return New MapThemeInfo()
			End If

			Dim tCentroidLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidLayers)
			Dim tCentroidBlocksList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidBlocks)
			Dim iPolygonStatus As enPolygonStatus
			'     DMCommon.Debug.MsgBox("06_327", tMapThemeData.CentroidBlocks, tCentroidBlocksList.List, tMapThemeData.CentroidLayers, tCentroidLayersList.List, tMapThemeData.ClosedPgonsLayers)
			tMapThemeInfo.Reset()
			'   DMCommon.ExcelLogX()
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				oEntity = Nothing
				Try
					oEntity = GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) ' DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					If oEntity IsNot Nothing Then
						sRXClassName = oEntity.GetRXClass().Name
						sEntityLayer = oEntity.Layer

						If tMapThemeData.GraphType = enGraphType.Topology Then
							If LinkCondition.IsLink(sRXClassName) Then
								If tLinkLayersList.Contains(sEntityLayer) Then
									tMapThemeInfo.AddLink(1)
									tMapThemeInfo.AddArcCount(dmLineCleanup.GetArcCountByClass(oEntity, sRXClassName))
								ElseIf tLineLinkLayersList.Contains(sEntityLayer) Then
									tMapThemeInfo.AddLineLinkCount(1)
								ElseIf sEntityLayer = DMAcadExt.MapThemeData.WorkAreaBoundaryLayer Then
									tMapThemeInfo.AddWorkAreaRing(1)
								Else
									'	AcadDocument.WriteMessage("_NOT 13 " & sRXClassName & "," & sEntityLayer & "," & oEntity.Handle.ToString())
								End If
							End If
						ElseIf tMapThemeData.GraphType = enGraphType.ClosedPolygons Then
							iPolygonStatus = LinkCondition.GetPolygonStatus(oEntity)
							Select Case iPolygonStatus
								Case enPolygonStatus.Polyline
									If tLinkLayersList.Contains(sEntityLayer) Then
										tMapThemeInfo.AddLink(1)
									End If

								Case enPolygonStatus.ClosedPolyline
									If tLinkLayersList.Contains(sEntityLayer) Then
										tMapThemeInfo.AddPgons(1)
										tMapThemeInfo.AddLink(1)
									ElseIf sEntityLayer = DMAcadExt.MapThemeData.WorkAreaBoundaryLayer Then
										tMapThemeInfo.AddWorkAreaRing(1)
									End If
							End Select

						End If

						If tCentroidLayersList.Contains(sEntityLayer) Then

							If sRXClassName = AcadConst.AcadBlockRefName Then

								If tCentroidBlocksList.Contains(zzGetBlockName(oEntity)) Then

									tMapThemeInfo.AddCentroidBlocks(1)
								End If
							End If
						End If
					Else
						AcadDocument.WriteMessage("#129AcadTrans " & tAcObjID.ToString())
					End If

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByClassName_3")

				End Try
			Next

			'	MessageBox.Show(tMapThemeInfo.LinkCount & vbCrLf & tMapThemeInfo.LinkWithArcCount & vbCrLf & tMapThemeInfo.LineLinkCount & vbCrLf & tMapThemeInfo.CentroidBlocksCount, "03_443")
		End If
		'	MessageBox.Show(CStr(tMapThemeInfo.CentroidBlocksCount) & vbCrLf & "", "06_321")
		Return tMapThemeInfo
	End Function


	Public Shared Function GetStatistics(tMapThemeData As DMAcadExt.MapThemeData, hsLayers As HashSet(Of String)) As MapThemeInfo()
		Dim tMapThemeInfo() As MapThemeInfo
		Dim tLayerMapThemeInfo As MapThemeInfo = New MapThemeInfo()
		Dim iLayerIndex As Integer = 0
		If moModelSpaceTableRecord IsNot Nothing AndAlso hsLayers IsNot Nothing AndAlso hsLayers.Count > 0 Then
			Dim oEntity As Entity
			Dim sRXClassName As String
			Dim sEntityLayer As String
			Dim tLinkLayersList As DMCommon.dmList
			Dim tLineLinkLayersList As DMCommon.dmList
			Dim dicMapThemeInfo As Dictionary(Of String, MapThemeInfo) = New Dictionary(Of String, MapThemeInfo)()
			Dim dicMapThemeInfoIndex As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)

			If tMapThemeData.GraphType = enGraphType.Topology Then
				tLinkLayersList = New DMCommon.dmList(tMapThemeData.LinkLayers)
				tLineLinkLayersList = New DMCommon.dmList(tMapThemeData.LineLinkLayers)
				Return Nothing
			ElseIf tMapThemeData.GraphType = enGraphType.ClosedPolygons Then
				tLinkLayersList = New DMCommon.dmList(tMapThemeData.ClosedPgonsLayers)
				tLineLinkLayersList = New DMCommon.dmList(String.Empty)
				Return Nothing
			ElseIf tMapThemeData.GraphType = enGraphType.TopologyList Then
				ReDim tMapThemeInfo(hsLayers.Count - 1)

				For Each sLayer As String In hsLayers
					dicMapThemeInfo.Add(sLayer, New MapThemeInfo())
					tMapThemeInfo(iLayerIndex) = New MapThemeInfo()
					dicMapThemeInfoIndex.Add(sLayer, iLayerIndex)
					iLayerIndex += 1
				Next
			Else
				Return {New MapThemeInfo()}
			End If

			Dim tCentroidLayersList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidLayers)
			Dim tCentroidBlocksList As DMCommon.dmList = New DMCommon.dmList(tMapThemeData.CentroidBlocks)

			'     DMCommon.Debug.MsgBox("06_327", tMapThemeData.CentroidBlocks, tCentroidBlocksList.List, tMapThemeData.CentroidLayers, tCentroidLayersList.List, tMapThemeData.ClosedPgonsLayers)

			iLayerIndex = 0
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				oEntity = Nothing
				Try
					oEntity = GetEntity(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) ' DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					If oEntity IsNot Nothing Then
						sRXClassName = oEntity.GetRXClass().Name
						sEntityLayer = oEntity.Layer

						If tMapThemeData.GraphType = enGraphType.TopologyList Then
							If LinkCondition.IsLink(sRXClassName) Then
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetStat", sRXClassName, sEntityLayer, dicMapThemeInfoIndex.ContainsKey(sEntityLayer))

								'If dicMapThemeInfo.TryGetValue(sEntityLayer, tLayerMapThemeInfo) Then
								If dicMapThemeInfoIndex.TryGetValue(sEntityLayer, iLayerIndex) Then
									tLayerMapThemeInfo = tMapThemeInfo(iLayerIndex)
									'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetStat_1", sRXClassName, sEntityLayer, tLayerMapThemeInfo.LinkCount)
									tMapThemeInfo(iLayerIndex).AddLink(1)
									'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetStat_2", sRXClassName, sEntityLayer, tLayerMapThemeInfo.LinkCount)
								End If
							ElseIf sRXClassName = AcadConst.AcadBlockRefName Then
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!GetStat_1", sRXClassName, sEntityLayer, zzGetBlockName(oEntity), tCentroidBlocksList.List, tCentroidBlocksList.Contains(zzGetBlockName(oEntity)))
								If tCentroidBlocksList.Contains(zzGetBlockName(oEntity)) Then

									If dicMapThemeInfoIndex.TryGetValue(sEntityLayer, iLayerIndex) Then

										tMapThemeInfo(iLayerIndex).AddCentroidBlocks(1)
									End If


								End If
							End If

						ElseIf tMapThemeData.GraphType = enGraphType.ClosedPolygons Then


						End If

						If tCentroidLayersList.Contains(sEntityLayer) Then


						End If
					Else
						AcadDocument.WriteMessage("#129AcadTrans " & tAcObjID.ToString())
					End If

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByClassName_3")
					Return Nothing
				End Try
			Next
			Return tMapThemeInfo
		Else
			Return Nothing
		End If


	End Function
	Public Shared Function AppendEntity(ByVal oEntity As Entity, Optional ByVal bAddDrawOrderTable As Boolean = False) As ObjectId
		Dim tAcObjID As ObjectId
		If moModelSpaceTableRecord IsNot Nothing Then
			Try
				If oEntity IsNot Nothing Then
					'DMCommon.ExcelLogAW5.SetNextValue(0, "Bef Append", oEntity.ObjectId, oEntity.ObjectId, oEntity.Handle, oEntity.GetRXClass.Name)
					tAcObjID = moModelSpaceTableRecord.AppendEntity(oEntity)
					'DMCommon.ExcelLogAW5.SetNextValue(0, "After Append", oEntity.ObjectId, oEntity.ObjectId, oEntity.Handle, oEntity.GetRXClass.Name)
					moTransaction.AddNewlyCreatedDBObject(oEntity, True)
				End If


				If bAddDrawOrderTable AndAlso mcolDrawOrderIDs IsNot Nothing Then
					';;;;;;;;;;;;;;;;;;;;;;;;;	mcolDrawOrderIDs.Add(tAcObjID)
				End If
				''''''''''''''''''''''''''''''''''''	AcadDocument.WriteMessage("Ent: " & tAcObjID.ToString() & ":" & mcolDrawOrderIDs.Count)

			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				'	System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oEntity.GetType().ToString() & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - AppendEntity")
				AcadDocument.WriteMessage("Err #2173 " & oAcadEx.ErrorStatus.ToString & "; " & oAcadEx.Message.ToString & "; " & oEntity.GetType().ToString() & "; " & oAcadEx.StackTrace)
				'	System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - AppendEntity")
				tAcObjID = ObjectId.Null
			End Try
		Else
			MessageBox.Show("Exception #1847", DMCommon.Functions.AppName)
			tAcObjID = ObjectId.Null
		End If
		Return tAcObjID
	End Function

	Public Shared Sub InsertPoint(ByVal tPoint2d As Point2d, Optional oResBuffer As ResultBuffer = Nothing, Optional ByVal iColorIndex As Integer = -1, Optional ByVal sLayerName As String = msEmpty)
		Dim tPoint3d As Point3d = New Point3d(tPoint2d.X, tPoint2d.Y, 0.0)
		Dim oPoint As DBPoint = New DBPoint(tPoint3d)
		zzInsertPoint(oPoint, oResBuffer, iColorIndex, sLayerName)
	End Sub
	Public Shared Sub InsertPoint(ByVal tPoint3d As Point3d, Optional oResBuffer As ResultBuffer = Nothing, Optional ByVal iColorIndex As Integer = -1, Optional ByVal sLayerName As String = msEmpty)
		Dim oPoint As DBPoint = New DBPoint(tPoint3d)
		zzInsertPoint(oPoint, oResBuffer, iColorIndex, sLayerName)
	End Sub
	Private Shared Sub zzInsertPoint(ByVal oPoint As DBPoint, Optional oResBuffer As ResultBuffer = Nothing, Optional ByVal iColorIndex As Integer = -1, Optional ByVal sLayerName As String = msEmpty)

		oPoint.Visible = True
		If iColorIndex <> -1 Then
			oPoint.ColorIndex = 1 + (iColorIndex - 1) Mod 255
		End If

		If Not String.IsNullOrEmpty(sLayerName) Then
			Try
				oPoint.Layer = sLayerName
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.Message & vbCrLf & "Layer: " & sLayerName, "AcadTransaction - AppendDBObject")
				AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - AppendDBObject_2")
			End Try
		End If
		If oResBuffer IsNot Nothing Then
			oPoint.XData = oResBuffer
		End If
		AppendEntity(oPoint)
	End Sub
	Public Shared Sub AppendDBObject(ByVal oDBObject As DBObject)
		If moTransaction IsNot Nothing Then
			Try
				moTransaction.AddNewlyCreatedDBObject(oDBObject, True)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.Message, "AcadTransaction - AppendDBObject")
				AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - AppendDBObject_3")
			End Try
		Else
			MessageBox.Show("ERRRRROR " & CStr(moTransaction Is Nothing), "12_657")
		End If
	End Sub
	Private Shared Function zzGetUniqueBlockName(ByVal sPrefix As String) As String
		Dim oBlockTable As BlockTable
		Dim sName As String
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			For iIndex As Integer = miBlockNameCounter + 1 To 4095
				sName = sPrefix & Convert.ToString(iIndex)
				If Not oBlockTable.Has(sName) Then
					miBlockNameCounter = iIndex
					Return sName
				End If
			Next
			Return Nothing
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "zzGetUniqueBlockName")
			Return Nothing
		End Try
	End Function

	Private Shared Function zzGetBlockName(ByVal oEntity As Entity) As String
		Dim oBlockRef As BlockReference
		Try
			oBlockRef = DirectCast(oEntity, BlockReference)
			If oBlockRef IsNot Nothing Then
				Return oBlockRef.Name
			Else
				Return String.Empty
			End If
		Catch oEx As Exception
			Return String.Empty
		End Try
	End Function
	Public Shared Function GetBoxBoundingBlock(ByVal sBlockName As String, oBox As TPlnBoundingBox) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetBoxBoundingBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(4)

				Dim dSegment As Double = Math.Sin(Math.PI / 4.0)


				oPolyline.AddVertexAt(0, New Point2d(dSegment, dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(dSegment, -dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(-dSegment, -dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(3, New Point2d(-dSegment, dSegment), 0.0, 0.0, 0.0)


				oPolyline.Closed = True

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetBoxBoundingBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetCellBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetCellBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(4)
				Dim dSegment As Double = 1.0


				oPolyline.AddVertexAt(0, New Point2d(0.0, 0.0), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(dSegment, 0.0), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(dSegment, dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(3, New Point2d(0.0, dSegment), 0.0, 0.0, 0.0)


				oPolyline.Closed = True

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetCellBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetSquareBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetSquareBlock1")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(4)
				Dim dSegment As Double = Math.Sin(Math.PI / 4.0)


				oPolyline.AddVertexAt(0, New Point2d(dSegment, dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(dSegment, -dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(-dSegment, -dSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(3, New Point2d(-dSegment, dSegment), 0.0, 0.0, 0.0)


				oPolyline.Closed = True

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sBlockName, "AcadTransaction - GetSquareBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetTriangleBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetTriangleBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(3)
				Dim dY As Double = -Math.Sin(Math.PI / 6.0)
				Dim dX As Double = Math.Cos(Math.PI / 6.0)

				oPolyline.AddVertexAt(0, New Point2d(0.0, 1.0), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(dX, dY), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(-dX, dY), 0.0, 0.0, 0.0)
				oPolyline.Closed = True

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetTriangleBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetVBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetVBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(3)
				Dim dY As Double = Math.Sin(Math.PI / 3.0)
				Dim dX As Double = Math.Cos(Math.PI / 3.0)

				oPolyline.AddVertexAt(0, New Point2d(-dX, dY), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(0.0, 0.0), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(dX, dY), 0.0, 0.0, 0.0)

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetTriangleBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetCircleBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetCircleBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oCircle As Circle = New Circle(New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0), New Vector3d(0.0, 0.0, 1.0), 1.0)
				oCircle.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oCircle)
				moTransaction.AddNewlyCreatedDBObject(oCircle, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetCircleBlock_1")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function

	Public Shared Function GetRhombusBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetRhombusBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oPolyline As Polyline = New Polyline(4)
				Dim dVertSegment As Double = 0.5
				Dim dHorSegment As Double = 0.25




				oPolyline.AddVertexAt(0, New Point2d(0.0, dVertSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(1, New Point2d(-dHorSegment, 0), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(2, New Point2d(0.0, -dVertSegment), 0.0, 0.0, 0.0)
				oPolyline.AddVertexAt(3, New Point2d(dHorSegment, 0), 0.0, 0.0, 0.0)


				oPolyline.Closed = True

				oPolyline.ColorIndex = 0
				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oPolyline)
				moTransaction.AddNewlyCreatedDBObject(oPolyline, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetTriangleBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Function GetSalltireBlock(ByVal sBlockName As String) As ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTr - GetSalltireBlock")
		End Try
		If Not moBlockTable.Has(sBlockName) Then
			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				Dim oLineA As Line = New Line(New Point3d(0.5, 0.5, 0.0), New Point3d(-0.5, -0.5, 0.0))
				Dim oLineB As Line = New Line(New Point3d(-0.5, 0.5, 0.0), New Point3d(0.5, -0.5, 0.0))

				oLineA.ColorIndex = 0
				oLineB.ColorIndex = 0


				Dim tResObjectID As ObjectId
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockTableRecord.Name = sBlockName
				tResObjectID = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
				oBlockTableRecord.AppendEntity(oLineA)
				moTransaction.AddNewlyCreatedDBObject(oLineA, True)
				oBlockTableRecord.AppendEntity(oLineB)
				moTransaction.AddNewlyCreatedDBObject(oLineB, True)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - GetTriangleBlock")
			End Try

		End If
		Return moBlockTable.Item(sBlockName)


	End Function
	Public Shared Sub OpenNewBlockDB()
		moNewBlockDB = New Database()
	End Sub
	Public Shared Sub InsertNewBlockDB(ByVal sPrefix As String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sName As String = zzGetUniqueBlockName(sPrefix)

		Try
			oCurrentDatabase.Insert(sName, moNewBlockDB, True)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
		End Try

	End Sub
	Public Shared Function OpenBlockDB_AAA(ByVal sBlockName As String, Optional ByVal sPath As String = Nothing) As ObjectId
		Dim oBlockTable As BlockTable
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

		Dim tBlockObjID As ObjectId
		Dim oNewBlockDB As Database
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim iAttribDefIndex As Integer = -1
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			Return ObjectId.Null
		End Try
		If oBlockTable.Has(sBlockName) Then
			tBlockObjID = oBlockTable.Item(sBlockName)
		ElseIf sPath IsNot Nothing Then
			Dim sFullName As String = sPath & "\" & sBlockName & ".dwg"
			Dim oFile As System.IO.FileInfo = New System.IO.FileInfo(sFullName)
			If oFile.Exists Then
				oNewBlockDB = New Database()
				Try
					oNewBlockDB.ReadDwgFile(sFullName, System.IO.FileShare.Read, True, String.Empty)
					tBlockObjID = oCurrentDatabase.Insert(sBlockName, oNewBlockDB, True)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "File '" & sFullName & "'" & vbCrLf & "Block '" & sBlockName & "'", "AcadTr - OpenBlockDB")
				End Try

			Else
				System.Windows.Forms.MessageBox.Show("File '" & sFullName & "' was not found", "04_340b")
			End If

		End If

		Return tBlockObjID

	End Function

	Public Shared Function OpenBlockDB(ByVal sPath As String, ByVal sBlockName As String, ByRef oaAttribDefs() As AttributeDefinition) As ObjectId
		Dim oBlockTable As BlockTable
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim tBlockObjID As ObjectId
		Dim oNewBlockDB As Database
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim iAttribDefIndex As Integer = -1
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			Return ObjectId.Null
		End Try
		If oBlockTable.Has(sBlockName) Then
			tBlockObjID = oBlockTable.Item(sBlockName)
		ElseIf sPath IsNot Nothing Then
			Dim sFullName As String = sPath & "\" & sBlockName & ".dwg"
			Dim oFile As System.IO.FileInfo = New System.IO.FileInfo(sFullName)
			'   System.Windows.Forms.MessageBox.Show(oFile.Exists.ToString() & vbCrLf & sFullName & vbCrLf & sBlockName, "04_187")
			If oFile.Exists Then
				oNewBlockDB = New Database()
				Try
					oNewBlockDB.ReadDwgFile(sFullName, System.IO.FileShare.Read, True, String.Empty)

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "File '" & sFullName & "'" & vbCrLf & "Block '" & sBlockName & "'", "1-OpenBlockDB:AcadTr")
					Return ObjectId.Null
				End Try
				Try

					tBlockObjID = oCurrentDatabase.Insert(sBlockName, oNewBlockDB, True)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "File '" & sFullName & "'" & vbCrLf & "Block '" & sBlockName & "'", "2-OpenBlockDB:AcadTr")
					Return ObjectId.Null
				End Try

			Else
				System.Windows.Forms.MessageBox.Show("File '" & sFullName & "' was not found", "04_340a")
			End If
		Else
			System.Windows.Forms.MessageBox.Show("Block: '" & sBlockName & "' was not found" & vbCrLf & "Folder '" & sPath & "' not exists!", "02_560")
		End If
		If Not tBlockObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If
		If oBlockTableRecord IsNot Nothing Then
			If oBlockTableRecord.HasAttributeDefinitions Then
				For Each tObjID As ObjectId In oBlockTableRecord
					oDBObject = moTransaction.GetObject(tObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					sRXClassName = oDBObject.GetRXClass().Name
					If sRXClassName = AcadConst.AcadAttributeDefName Then
						iAttribDefIndex += 1
						ReDim Preserve oaAttribDefs(iAttribDefIndex)
						oaAttribDefs(iAttribDefIndex) = DirectCast(oDBObject, AttributeDefinition)
					End If
				Next
			End If
			Return tBlockObjID
		Else
			Return ObjectId.Null
		End If

	End Function
	Public Shared Function OpenBlockDB(ByVal oEntity As Entity, ByVal sBlockName As String) As ObjectId
		Dim oBlockTable As BlockTable
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTableRecord As BlockTableRecord
		Dim tBlockObjID As ObjectId
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			Return ObjectId.Null
		End Try
		If oBlockTable.Has(sBlockName) Then
			tBlockObjID = oBlockTable.Item(sBlockName)
		ElseIf oEntity IsNot Nothing Then
			oBlockTableRecord = New BlockTableRecord()
			oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
			Try
				oBlockTableRecord.Name = sBlockName
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			End Try
			mtNewBlockObjId = moBlockTable.Add(oBlockTableRecord)
			moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)
			oBlockTableRecord.AppendEntity(oEntity)
			moTransaction.AddNewlyCreatedDBObject(oEntity, True)


		End If


	End Function
	Public Shared Sub OpenNewBlock(ByVal sName As String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sBlockName As String = sName
		Dim tBlockTableRecordObjID As ObjectId
		Dim oDBObject As DBObject
		'	Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch ex As Exception

		End Try

		Dim sTest As String = "a"
		'sBlockName = ""
		Try

			moBlockTableRecord = New BlockTableRecord()
			moBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)


			If Not String.IsNullOrEmpty(sName) Then
				If moBlockTable.Has(sName) Then
					tBlockTableRecordObjID = moBlockTable.Item(sName)
					moBlockTableRecord = GetBlockTableRecord(tBlockTableRecordObjID, OpenMode.ForWrite)
					For Each tAcObjId As ObjectId In moBlockTableRecord
						oDBObject = GetDBObject(tAcObjId, OpenMode.ForWrite)
						Try
							oDBObject.Erase()
						Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
							System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction1")
						End Try

					Next

				Else

					Try
						moBlockTableRecord.Name = sBlockName
					Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
						System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
					End Try

					'	Dim oNewDB As Database = New Database()


					mtNewBlockObjId = moBlockTable.Add(moBlockTableRecord)


					'	Dim oTestDbObj As DBObject = GetDBObject(mtNewBlockObjId, OpenMode.ForRead)
					Dim tGeoPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
					moTransaction.AddNewlyCreatedDBObject(moBlockTableRecord, True)
				End If


			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
		End Try


	End Sub
	Public Shared Sub OpenNewBlockTest(ByVal sName As String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sBlockName As String = sName


		'	Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch ex As Exception

		End Try

		Dim sTest As String = "a"
		'sBlockName = ""
		Try

			moBlockTableRecord = New BlockTableRecord()
			moBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)


			If sBlockName.Length <> 0 Then
				Try
					moBlockTableRecord.Name = sBlockName
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
				End Try
			End If


			'	moBlockTable.UpgradeOpen()
			Dim newdb As Database = New Database()

			'newdb.ReadDwgFile("M:\Dm_Work\Blocks\Builds\BLDS002.dwg", System.IO.FileShare.Read, True, "")
			'	Dim tDBObjID As ObjectId = oCurrentDatabase.Insert("xxx", newdb, True)
			'	System.Windows.Forms.MessageBox.Show(tDBObjID.ToString(), "15_658")
			'	Dim oNewDBObject As DBObject = GetDBObject(mtNewBlockObjId, OpenMode.ForRead)
			'	System.Windows.Forms.MessageBox.Show(oNewDBObject.GetType().ToString(), "15_559p")
			mtNewBlockObjId = moBlockTable.Add(moBlockTableRecord)


			'	Dim oTestDbObj As DBObject = GetDBObject(mtNewBlockObjId, OpenMode.ForRead)
			Dim tGeoPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
			moTransaction.AddNewlyCreatedDBObject(moBlockTableRecord, True)
			MessageBox.Show(CStr(moBlockTableRecord.IsAnonymous), "Anon")
			MessageBox.Show(CStr(moBlockTableRecord.Name), "Name")

			'	oTestDbObj = GetDBObject(mtNewBlockObjId, OpenMode.ForRead)
			'	oBlockTableRecord = DirectCast(oTestDbObj, BlockTableRecord)
			'	System.Windows.Forms.MessageBox.Show(oBlockTableRecord.Name, "15_662vv")
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
		End Try
		sTest = "b"
		sTest = "c"

	End Sub
	Public Shared Function GetBlockListNew() As System.Collections.Generic.IList(Of AcadBlock)
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception

		End Try

		Dim sTest As String = "a"
		'sBlockName = ""
		Try
			Dim oBlockEnum As Autodesk.AutoCAD.DatabaseServices.SymbolTableEnumerator = moBlockTable.GetEnumerator()
			oBlockEnum.Reset()
			Dim tBlockID As ObjectId
			Dim oBlockRec As BlockTableRecord
			Dim oList As System.Collections.Generic.IList(Of AcadBlock) = New System.Collections.Generic.List(Of AcadBlock)
			Dim dicBlocks As Dictionary(Of String, AcadBlock) = New Dictionary(Of String, AcadBlock)()
			Dim sBlockName As String
			Dim sPrimaryBlockName As String = Nothing
			Dim colAdditionalBlockRec As System.Collections.ObjectModel.Collection(Of KeyValuePair(Of String, AcadBlock)) = New System.Collections.ObjectModel.Collection(Of KeyValuePair(Of String, AcadBlock))


			Dim oAcadBlock As AcadBlock = Nothing
			Dim oPaperSpaceTableRecord As BlockTableRecord = DirectCast(moTransaction.GetObject(moBlockTable.Item(BlockTableRecord.PaperSpace), OpenMode.ForRead, False), BlockTableRecord)
			Do While oBlockEnum.MoveNext
				tBlockID = oBlockEnum.Current
				If tBlockID <> moModelSpaceTableRecord.ObjectId AndAlso tBlockID <> oPaperSpaceTableRecord.ObjectId Then
					oBlockRec = GetBlockTableRecord(tBlockID, OpenMode.ForRead)
					If Not oBlockRec.IsAnonymous AndAlso Not oBlockRec.IsDependent AndAlso Not oBlockRec.IsFromExternalReference Then
						If oBlockRec.GetBlockReferenceIds(True, False).Count <> 0 Then
							sBlockName = oBlockRec.Name
							oAcadBlock = New AcadBlock(oBlockRec)
							oList.Add(oAcadBlock)
							dicBlocks.Add(sBlockName, oAcadBlock)
							If oAcadBlock.IsAdditionalName(sPrimaryBlockName) Then
								colAdditionalBlockRec.Add(New KeyValuePair(Of String, AcadBlock)(sPrimaryBlockName, oAcadBlock))
							End If
						End If
					End If
				End If
			Loop
			For Each oKeyValuePair As KeyValuePair(Of String, AcadBlock) In colAdditionalBlockRec
				If dicBlocks.TryGetValue(oKeyValuePair.Key, oAcadBlock) Then
					oAcadBlock.AdditionalBlock = oKeyValuePair.Value
					oKeyValuePair.Value.PrimaryBlock = oAcadBlock
				End If

			Next
			Return oList
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			Return Nothing
		End Try


	End Function
	Public Shared Function GetBlockListNewErr() As System.Collections.Generic.IList(Of AcadBlock)
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception

		End Try

		Dim sTest As String = "a"
		'sBlockName = ""
		Try
			Dim oBlockEnum As Autodesk.AutoCAD.DatabaseServices.SymbolTableEnumerator = moBlockTable.GetEnumerator()
			oBlockEnum.Reset()
			Dim tBlockID As ObjectId
			Dim oBlockRec As BlockTableRecord
			Dim oList As System.Collections.Generic.IList(Of AcadBlock) = New System.Collections.Generic.List(Of AcadBlock)
			Dim dicBlocks As Dictionary(Of String, AcadBlock) = New Dictionary(Of String, AcadBlock)()
			Dim sBlockName As String
			Dim sPrimaryBlockName As String = Nothing
			Dim colAdditionalBlockRec As System.Collections.ObjectModel.Collection(Of KeyValuePair(Of String, BlockTableRecord)) = New System.Collections.ObjectModel.Collection(Of KeyValuePair(Of String, BlockTableRecord))

			Dim oAcadBlock As AcadBlock = Nothing
			Dim oPaperSpaceTableRecord As BlockTableRecord = DirectCast(moTransaction.GetObject(moBlockTable.Item(BlockTableRecord.PaperSpace), OpenMode.ForRead, False), BlockTableRecord)
			Do While oBlockEnum.MoveNext
				tBlockID = oBlockEnum.Current
				If tBlockID <> moModelSpaceTableRecord.ObjectId AndAlso tBlockID <> oPaperSpaceTableRecord.ObjectId Then
					oBlockRec = GetBlockTableRecord(tBlockID, OpenMode.ForRead)
					If Not oBlockRec.IsAnonymous AndAlso Not oBlockRec.IsDependent AndAlso Not oBlockRec.IsFromExternalReference Then
						If oBlockRec.GetBlockReferenceIds(True, False).Count <> 0 Then
							sBlockName = oBlockRec.Name
							oAcadBlock = New AcadBlock(oBlockRec)
							oList.Add(oAcadBlock)
							dicBlocks.Add(sBlockName, oAcadBlock)
							If oAcadBlock.IsAdditionalName(sPrimaryBlockName) Then
								colAdditionalBlockRec.Add(New KeyValuePair(Of String, BlockTableRecord)(sPrimaryBlockName, oBlockRec))
							End If
						End If
					End If
				End If
			Loop
			For Each oKeyValuePair As KeyValuePair(Of String, BlockTableRecord) In colAdditionalBlockRec
				If dicBlocks.TryGetValue(oKeyValuePair.Key, oAcadBlock) Then
					System.Windows.Forms.MessageBox.Show(oKeyValuePair.Key & ":" & oAcadBlock.BlockName, "09_700")
					oAcadBlock.AdditionalBlock = New AcadBlock(oKeyValuePair.Value)
				End If

			Next
			Return oList
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			Return Nothing
		End Try


	End Function
	Public Shared Function GetBlockList() As System.Collections.Generic.IList(Of BlockTableRecord)
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase

		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch ex As Exception

		End Try

		Dim sTest As String = "a"
		'sBlockName = ""
		Try
			Dim oBlockEnum As Autodesk.AutoCAD.DatabaseServices.SymbolTableEnumerator = moBlockTable.GetEnumerator()
			oBlockEnum.Reset()
			Dim tBlockID As ObjectId
			Dim oBlockRec As BlockTableRecord
			Dim oList As System.Collections.Generic.IList(Of BlockTableRecord) = New System.Collections.Generic.List(Of BlockTableRecord)
			Dim oPaperSpaceTableRecord As BlockTableRecord = DirectCast(moTransaction.GetObject(moBlockTable.Item(BlockTableRecord.PaperSpace), OpenMode.ForRead, False), BlockTableRecord)
			Do While oBlockEnum.MoveNext
				tBlockID = oBlockEnum.Current
				If tBlockID <> moModelSpaceTableRecord.ObjectId AndAlso tBlockID <> oPaperSpaceTableRecord.ObjectId Then
					oBlockRec = GetBlockTableRecord(tBlockID, OpenMode.ForRead)
					If Not oBlockRec.IsAnonymous AndAlso Not oBlockRec.IsDependent AndAlso Not oBlockRec.IsFromExternalReference Then
						If oBlockRec.GetBlockReferenceIds(True, False).Count <> 0 Then
							oList.Add(oBlockRec)
						End If
					End If
				End If

			Loop

			Return oList
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			Return Nothing
		End Try


	End Function
	Public Shared Function CreateNewBlock(sBlockName As String) As ObjectId
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oEx As Exception
			Return ObjectId.Null
		End Try
		If oBlockTable.Has(sBlockName) Then
			Return moBlockTable.Item(sBlockName)
		Else

			Try
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				Try
					oBlockTableRecord.Name = sBlockName
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
				End Try
				Dim tNewBlockObjId As ObjectId = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)

				Dim oLine As Line = New Line(New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0), New Autodesk.AutoCAD.Geometry.Point3d(4.0, 2.0, 0.0))
				Dim tEn1ObjID As ObjectId = oBlockTableRecord.AppendEntity(oLine)
				moTransaction.AddNewlyCreatedDBObject(oLine, True)

				'	MessageBox.Show(sBlockName & vbCrLf & CStr(moBlockTable.Has(sBlockName) & vbCrLf & tEn1ObjID.ToString()), "02_200")
				Return tNewBlockObjId
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				Return Nothing
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			End Try
		End If

	End Function
	Public Shared Sub ClearXData()

		Dim oDBObject As DBObject
		Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
		Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
		Dim oEmptyBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
		If moModelSpaceTableRecord IsNot Nothing Then
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForWrite)
				oResBuffer = oDBObject.XData
				If oResBuffer IsNot Nothing Then
					taTypedValues = oResBuffer.AsArray()
					AcadDocument.WriteMessage("BEFORE XX_DATA: " & CStr(taTypedValues.GetUpperBound(0)))

					oDBObject.XData = oEmptyBuffer

					oResBuffer = oDBObject.XData
					If oResBuffer IsNot Nothing Then
						taTypedValues = oResBuffer.AsArray()
						AcadDocument.WriteMessage("AFTER XX_DATA: " & CStr(taTypedValues.GetUpperBound(0)))
					Else
						AcadDocument.WriteMessage("AFTER XX_DATA: " & CStr("NOTHING"))
					End If


				Else

				End If
			Next

		End If
	End Sub
	Public Shared Sub DrawNet(iColumnCount As Integer, iRowCount As Integer, dXStep As Double, dYStep As Double)
		Dim oLine As Line
		Dim tStartPoint As Point3d
		Dim tEndPoint As Point3d
		Dim dXEnd, dYEnd As Double
		Dim dXCurrent, dYCurrent As Double

		dXEnd = dXStep * (iColumnCount - 1)
		dYEnd = dYStep * (iRowCount - 1)

		For iRow As Integer = 0 To iRowCount - 1
			dYCurrent = iRow * dYStep
			tStartPoint = New Point3d(0.0, dYCurrent, 0.0)
			tEndPoint = New Point3d(dXEnd, dYCurrent, 0.0)
			oLine = New Line(tStartPoint, tEndPoint)
			AppendEntity(oLine)
		Next

		For iColumn As Integer = 0 To iColumnCount - 1
			dXCurrent = iColumn * dXStep

			tStartPoint = New Point3d(dXCurrent, 0.0, 0.0)
			tEndPoint = New Point3d(dXCurrent, dYEnd, 0.0)

			oLine = New Line(tStartPoint, tEndPoint)
			AppendEntity(oLine)
		Next
		'   AcadDocument.GetExtMinPoint()

	End Sub
	Public Shared Sub DrawNet(dXStep As Double, dYStep As Double)
		Dim dXCurrent, dYCurrent As Double
		Dim tStartPoint As Point3d
		Dim tEndPoint As Point3d

		Dim oLine As Line

		Dim tMinPoint As Point2d = AcadDocument.GetExtMinPoint()
		Dim tMaxPoint As Point2d = AcadDocument.GetExtMaxPoint()
		tMinPoint = New Point2d(Math.Floor(tMinPoint.X), Math.Floor(tMinPoint.Y))
		tMaxPoint = New Point2d(Math.Ceiling(tMaxPoint.X), Math.Ceiling(tMaxPoint.Y))
		dXCurrent = tMinPoint.X
		dYCurrent = tMinPoint.Y

		Do While dXCurrent <= tMaxPoint.X
			dXCurrent += dXStep
			tStartPoint = New Point3d(dXCurrent, tMinPoint.Y, 0.0)
			tEndPoint = New Point3d(dXCurrent, tMaxPoint.Y, 0.0)
			oLine = New Line(tStartPoint, tEndPoint)
			AppendEntity(oLine, True)
		Loop

		Do While dYCurrent <= tMaxPoint.Y
			dYCurrent += dYStep
			tStartPoint = New Point3d(tMinPoint.X, dYCurrent, 0.0)
			tEndPoint = New Point3d(tMaxPoint.X, dYCurrent, 0.0)
			oLine = New Line(tStartPoint, tEndPoint)
			AppendEntity(oLine, True)
		Loop

	End Sub
	Public Shared Function CreateNewBlock(sBlockName As String, dlBlockDrawing As BlockDrawing) As ObjectId
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tTestObjID As ObjectId

		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction_1")
		End Try

		If moBlockTable.Has(sBlockName) Then

			Return moBlockTable.Item(sBlockName)
		Else

			Try
				'System.Windows.Forms.MessageBox.Show(CStr(sBlockName), "03_943A")
				Dim oBlockTableRecord As BlockTableRecord = New BlockTableRecord()
				oBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				Try
					oBlockTableRecord.Name = sBlockName
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction_2")
				End Try
				'	System.Windows.Forms.MessageBox.Show(CStr(sBlockName), "03_943C")
				Dim tNewBlockObjId As ObjectId = moBlockTable.Add(oBlockTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oBlockTableRecord, True)

				Dim oaEntity() As Entity = dlBlockDrawing()

				If oaEntity Is Nothing Then
					System.Windows.Forms.MessageBox.Show(CStr("oaEntity Is Nothing"), "03_955")
				Else
					'System.Windows.Forms.MessageBox.Show(CStr(oaEntity.GetUpperBound(0)), "03_956")
				End If

				For iIndex As Integer = 0 To oaEntity.GetUpperBound(0)
					tTestObjID = oBlockTableRecord.AppendEntity(oaEntity(iIndex))
					moTransaction.AddNewlyCreatedDBObject(oaEntity(iIndex), True)
				Next
				Return tNewBlockObjId
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				Return Nothing
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			End Try
		End If

	End Function

	Public Shared Sub TestModelSpaceCount()
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colAddedDBObjectIDs As ObjectIdCollection = New ObjectIdCollection
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					colAddedDBObjectIDs.Add(tAcObjId)
				Next
				System.Windows.Forms.MessageBox.Show(CStr(colAddedDBObjectIDs.Count) & vbCrLf & "", "04_011")
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1511")
			End Try
		End If
	End Sub

	Public Shared Function OpenNewAnonymBlock() As String
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sBlockName As String = "*U"
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), BlockTable)
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction-OpenNewAnonymBlock")
		End Try

		Try
			moBlockTableRecord = New BlockTableRecord()
			moBlockTableRecord.Origin = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
			Try
				moBlockTableRecord.Name = sBlockName
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
			End Try
			mtNewBlockObjId = moBlockTable.Add(moBlockTableRecord)
			moTransaction.AddNewlyCreatedDBObject(moBlockTableRecord, True)
			'	AcadDocument.WriteMessage("$$56: " & moBlockTableRecord.Name)
			Return moBlockTableRecord.Name
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			Return Nothing
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction")
		End Try
	End Function
	Public Shared Function GetTextStyle(ByVal sTextStyleName As String) As ObjectId
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oTextStyleTable As TextStyleTable
		Try
			oTextStyleTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.TextStyleTableId, OpenMode.ForRead, False, False), TextStyleTable)
			If oTextStyleTable.Has(sTextStyleName) Then
				Dim tTextStyleID As ObjectId
				Try
					tTextStyleID = oTextStyleTable.Item(sTextStyleName)

				Catch oEx As Exception
					DMCommon.Functions.ShowEx(oEx, "GetTextStyle_1")
					Return ObjectId.Null
				End Try
				Dim oTextStyle As TextStyleTableRecord = DirectCast(moTransaction.GetObject(tTextStyleID, OpenMode.ForWrite, False, False), TextStyleTableRecord)
				'	AcadDocument.WriteDebugMessage("*TextStyle='" & sTextStyleName & "' ,TextSize=" & CStr(oTextStyle.TextSize))	'"19_485"
				'oTextStyle.TextSize = 0.15
				Return tTextStyleID
			Else
				System.Windows.Forms.MessageBox.Show("Text Style '" & sTextStyleName & "' was not found", "AcadTransaction - GetTextStyle")
				Return ObjectId.Null
			End If

		Catch oEx As Exception
			DMCommon.Functions.ShowEx(oEx, "GetTextStyle_2")
			Return ObjectId.Null
		End Try
	End Function
	''' <summary>
	''' opt 5/1
	''' </summary>
	''' <param name="tBlockAcObjId"></param>
	''' <param name="colPoints"></param>
	''' <param name="oaAtrribDefs"></param>
	''' <param name="dicAtribValues"></param>
	''' <param name="dScaleFactor"></param>
	''' <param name="iAcadColor"></param>
	''' <returns></returns>
	Public Shared Function InsertBlockRef_AAA(ByVal tBlockAcObjId As ObjectId, ByVal colPoints As Point3dCollection, Optional ByVal oaAtrribDefs() As AttributeDefinition = Nothing, Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256) As ObjectIdCollection
		Dim oBlockRef As BlockReference
		Dim tAcObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		If moModelSpaceTableRecord IsNot Nothing Then

			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim colBlockRefs As ObjectIdCollection = New ObjectIdCollection()
			For Each tPoint As Point3d In colPoints
				tInsertPoint = tPoint
				oBlockRef = New BlockReference(tInsertPoint, tBlockAcObjId)
				If dScaleFactor <> 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				End If

				oBlockRef.ColorIndex = iAcadColor
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
				moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
				colBlockRefs.Add(tAcObjID)
				If oaAtrribDefs IsNot Nothing Then
					For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
						oAttributeRef = New AttributeReference()
						oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
						If dicAtribValues IsNot Nothing AndAlso dicAtribValues.ContainsKey(oAttributeRef.Tag) Then
							Try
								If oAttributeRef.TextString <> dicAtribValues.Item(oAttributeRef.Tag) Then
									oAttributeRef.TextString = dicAtribValues.Item(oAttributeRef.Tag)
								End If
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAtribValues.Item(oAttributeRef.Tag) & "|", "AcadTr - InsertBlockRef")
							End Try
						End If
						oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
						moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
					Next
				End If
			Next
			Return colBlockRefs
		Else
			Return Nothing
		End If
	End Function
	''' <summary>
	''' opt 2
	''' </summary>
	''' <param name="tBlockAcObjId"></param>
	''' <param name="oPointList"></param>
	''' <param name="oaAtrribDefs"></param>
	''' <param name="dicAtribValues"></param>
	''' <param name="dScaleFactor"></param>
	''' <param name="iAcadColor"></param>
	''' <returns></returns>
	Public Shared Function InsertBlockRef_AAA(ByVal tBlockAcObjId As ObjectId, ByVal oPointList As System.Collections.Generic.IList(Of TPlnPoint), Optional ByVal oaAtrribDefs() As AttributeDefinition = Nothing, Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256S) As ObjectIdCollection
		Dim oBlockRef As BlockReference
		Dim tAcObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		If moModelSpaceTableRecord IsNot Nothing Then

			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim colBlockRefs As ObjectIdCollection = New ObjectIdCollection()
			For Each oPoint As TPlnPoint In oPointList
				tInsertPoint = oPoint.AcGePoint3d
				oBlockRef = New BlockReference(tInsertPoint, tBlockAcObjId)
				If dScaleFactor <> 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				End If

				oBlockRef.ColorIndex = iAcadColor
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
				moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
				colBlockRefs.Add(tAcObjID)
				If oaAtrribDefs IsNot Nothing Then
					For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
						oAttributeRef = New AttributeReference()
						oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
						If dicAtribValues IsNot Nothing AndAlso dicAtribValues.ContainsKey(oAttributeRef.Tag) Then
							Try
								If oAttributeRef.TextString <> dicAtribValues.Item(oAttributeRef.Tag) Then
									oAttributeRef.TextString = dicAtribValues.Item(oAttributeRef.Tag)
								End If
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAtribValues.Item(oAttributeRef.Tag) & "|", "AcadTr - InsertBlockRef")
							End Try
						End If
						oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
						moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
					Next
				End If
			Next
			Return colBlockRefs
		Else
			Return Nothing
		End If
	End Function
	''' <summary>
	''' opt 4/3
	''' </summary>
	''' <param name="tBlockRecObjId"></param>
	''' <param name="tInsertPoint"></param>
	''' <param name="dScaleFactor"></param>
	''' <param name="iAcadColor"></param>
	''' <param name="sLayer"></param>
	''' <param name="dRotation"></param>
	''' <returns></returns>


	Public Shared Function InsertBlockRef_AAA(ByVal tBlockRecObjId As ObjectId, ByVal tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d, ByVal oaAtrribDefs() As AttributeDefinition _
													  , Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256 _
													  , Optional sLayer As String = Nothing, Optional dRotation As Double = 0.0, Optional ByVal dScaleFactorY As Double = 0.0, Optional bAddNewBlock As Boolean = False) As ObjectId
		Dim oBlockRef As BlockReference
		Dim tBlockRefObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")
			oBlockRef = New BlockReference(tInsertPoint, tBlockRecObjId)

			If dScaleFactor <> 0.0 Then
				If dScaleFactorY = 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				Else
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor, dScaleFactorY, 1.0)
				End If

			End If
			If Not String.IsNullOrEmpty(sLayer) Then
				Try
					oBlockRef.Layer = sLayer
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & "Layer='" & sLayer & "'", "InsertBlockRef_AcadTransaction")
				End Try

			End If
			oBlockRef.Rotation = dRotation
			oBlockRef.ColorIndex = iAcadColor

			tBlockRefObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)

			moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)





			Return tBlockRefObjID
		Else
			Return Nothing
		End If
	End Function
	''' <summary>
	''' opt 1/5
	''' </summary>
	''' <param name="tBlockRecObjId"></param>
	''' <param name="oaAtrribDefs"></param>
	''' <param name="tBlockRefData"></param>
	''' <returns></returns>
	Public Shared Function InsertBlockRef_AAA(ByVal tBlockRecObjId As ObjectId, ByVal oaAtrribDefs() As AttributeDefinition, ByVal tBlockRefData As BlockRefData) As ObjectId
		Dim oBlockRef As BlockReference
		Dim tBlockRefObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")
			oBlockRef = New BlockReference(tBlockRefData.Position, tBlockRecObjId)
			'''''''''      System.Windows.Forms.MessageBox.Show(CStr(dScaleFactor), "19_233")


			oBlockRef.ScaleFactors = tBlockRefData.ScaleFactors



			If Not String.IsNullOrEmpty(tBlockRefData.Layer) Then
				Try
					oBlockRef.Layer = tBlockRefData.Layer

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "InsertBlockRef_1")
				End Try

			End If
			oBlockRef.Rotation = tBlockRefData.Rotation
			oBlockRef.ColorIndex = tBlockRefData.ColorIndex
			tBlockRefObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)

			'  System.Windows.Forms.MessageBox.Show("" & vbCrLf & tBlockRefObjID.ToString(), "04_370b")
			moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
			Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection()


			If oaAtrribDefs IsNot Nothing AndAlso tBlockRefData.AtribValuesDic IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & oaAtrribDefs.GetUpperBound(0).ToString(), "04_319e")
				For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
					oAttributeRef = New AttributeReference()
					oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
					If tBlockRefData.AtribValuesDic.ContainsKey(oAttributeRef.Tag) Then
						Try
							If oAttributeRef.TextString <> tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag) Then
								oAttributeRef.TextString = tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag) & "|", "1_AcadTr-InsertBlockRef")
						End Try
					End If
					oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
					moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
				Next
			End If
			'msEmpty
			Return tBlockRefObjID
		Else
			Return ObjectId.Null
		End If
	End Function

	'Option 1
	Public Shared Function InsertBlockRef(ByVal tBlockAcObjId As ObjectId, ByVal colPoints As Point3dCollection, Optional ByVal oaAtrribDefs() As AttributeDefinition = Nothing, Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256) As ObjectIdCollection
		Dim oBlockRef As BlockReference
		Dim tAcObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		If moModelSpaceTableRecord IsNot Nothing Then

			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim colBlockRefs As ObjectIdCollection = New ObjectIdCollection()
			For Each tPoint As Point3d In colPoints
				tInsertPoint = tPoint
				oBlockRef = New BlockReference(tInsertPoint, tBlockAcObjId)
				If dScaleFactor <> 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				End If

				oBlockRef.ColorIndex = iAcadColor
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
				moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
				colBlockRefs.Add(tAcObjID)
				If oaAtrribDefs IsNot Nothing Then
					For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
						oAttributeRef = New AttributeReference()
						oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
						If dicAtribValues IsNot Nothing AndAlso dicAtribValues.ContainsKey(oAttributeRef.Tag) Then
							Try
								If oAttributeRef.TextString <> dicAtribValues.Item(oAttributeRef.Tag) Then
									oAttributeRef.TextString = dicAtribValues.Item(oAttributeRef.Tag)
								End If
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAtribValues.Item(oAttributeRef.Tag) & "|", "AcadTr - InsertBlockRef")
							End Try
						End If
						oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
						moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
					Next
				End If
			Next
			Return colBlockRefs
		Else
			Return Nothing
		End If
	End Function
	'Option 2
	Public Shared Function InsertBlockRef(ByVal tBlockAcObjId As ObjectId, ByVal oPointList As System.Collections.Generic.IList(Of TPlnPoint), Optional ByVal oaAtrribDefs() As AttributeDefinition = Nothing, Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256S) As ObjectIdCollection
		Dim oBlockRef As BlockReference
		Dim tAcObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d
		If moModelSpaceTableRecord IsNot Nothing Then

			'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim colBlockRefs As ObjectIdCollection = New ObjectIdCollection()
			For Each oPoint As TPlnPoint In oPointList
				tInsertPoint = oPoint.AcGePoint3d
				oBlockRef = New BlockReference(tInsertPoint, tBlockAcObjId)
				If dScaleFactor <> 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				End If

				oBlockRef.ColorIndex = iAcadColor
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
				moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
				colBlockRefs.Add(tAcObjID)
				If oaAtrribDefs IsNot Nothing Then
					For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
						oAttributeRef = New AttributeReference()
						oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
						If dicAtribValues IsNot Nothing AndAlso dicAtribValues.ContainsKey(oAttributeRef.Tag) Then
							Try
								If oAttributeRef.TextString <> dicAtribValues.Item(oAttributeRef.Tag) Then
									oAttributeRef.TextString = dicAtribValues.Item(oAttributeRef.Tag)
								End If
							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAtribValues.Item(oAttributeRef.Tag) & "|", "AcadTr - InsertBlockRef")
							End Try
						End If
						oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
						moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
					Next
				End If
			Next
			Return colBlockRefs
		Else
			Return Nothing
		End If
	End Function
	'Option 3
	Public Shared Function InsertBlockRef(ByVal tBlockRecObjId As ObjectId, ByVal tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d, Optional ByVal oaAtrribDefs() As AttributeDefinition = Nothing, Optional ByVal dicAtribValues As Generic.Dictionary(Of String, String) = Nothing, Optional ByVal dScaleFactor As Double = 1.0, Optional ByVal iAcadColor As Integer = 256, Optional sLayer As String = Nothing, Optional dRotation As Double = 0.0, Optional ByVal dScaleFactorY As Double = 0.0, Optional bAddNewBlock As Boolean = False) As ObjectId
		Dim oBlockRef As BlockReference
		Dim tBlockRefObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")
			oBlockRef = New BlockReference(tInsertPoint, tBlockRecObjId)
			'  System.Windows.Forms.MessageBox.Show(CStr(dScaleFactor), "19_233")
			If dScaleFactor <> 0.0 Then
				If dScaleFactorY = 0.0 Then
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)
				Else
					oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor, dScaleFactorY, 1.0)
				End If

			End If
			If Not String.IsNullOrEmpty(sLayer) Then
				Try
					oBlockRef.Layer = sLayer
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & "Layer='" & sLayer & "'", "InsertBlockRef_AcadTransaction")
				End Try

			End If
			oBlockRef.Rotation = dRotation
			oBlockRef.ColorIndex = iAcadColor
			tBlockRefObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)

			'    System.Windows.Forms.MessageBox.Show("" & vbCrLf & tBlockRefObjID.ToString(), "04_370b")
			moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
			Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection()


			If oaAtrribDefs IsNot Nothing Then
				' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & oaAtrribDefs.GetUpperBound(0).ToString(), "04_319e")
				For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
					oAttributeRef = New AttributeReference()
					oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
					If dicAtribValues IsNot Nothing AndAlso dicAtribValues.ContainsKey(oAttributeRef.Tag) Then
						Try
							If oAttributeRef.TextString <> dicAtribValues.Item(oAttributeRef.Tag) Then
								oAttributeRef.TextString = dicAtribValues.Item(oAttributeRef.Tag)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAtribValues.Item(oAttributeRef.Tag) & "|", "1_AcadTr-InsertBlockRef")
						End Try
					End If
					oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
					moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
				Next
			End If

			Return tBlockRefObjID
		Else
			Return ObjectId.Null
		End If
	End Function
	'Option 4
	Public Shared Function InsertBlockRef(ByVal tBlockRecObjId As ObjectId, ByVal oaAtrribDefs() As AttributeDefinition, ByVal tBlockRefData As BlockRefData) As ObjectId
		Dim oBlockRef As BlockReference
		Dim tBlockRefObjID As ObjectId
		Dim oAttributeRef As AttributeReference
		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")
			oBlockRef = New BlockReference(tBlockRefData.Position, tBlockRecObjId)
			'''''''''      System.Windows.Forms.MessageBox.Show(CStr(dScaleFactor), "19_233")


			oBlockRef.ScaleFactors = tBlockRefData.ScaleFactors



			If Not String.IsNullOrEmpty(tBlockRefData.Layer) Then
				Try
					oBlockRef.Layer = tBlockRefData.Layer

				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "InsertBlockRef_1")
				End Try

			End If
			oBlockRef.Rotation = tBlockRefData.Rotation
			oBlockRef.ColorIndex = tBlockRefData.ColorIndex
			tBlockRefObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)

			'  System.Windows.Forms.MessageBox.Show("" & vbCrLf & tBlockRefObjID.ToString(), "04_370b")
			moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
			Dim colAttributes As AttributeCollection = oBlockRef.AttributeCollection()


			If oaAtrribDefs IsNot Nothing AndAlso tBlockRefData.AtribValuesDic IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & oaAtrribDefs.GetUpperBound(0).ToString(), "04_319e")
				For iIndex As Integer = 0 To oaAtrribDefs.GetUpperBound(0)
					oAttributeRef = New AttributeReference()
					oAttributeRef.SetAttributeFromBlock(oaAtrribDefs(iIndex), oBlockRef.BlockTransform)
					If tBlockRefData.AtribValuesDic.ContainsKey(oAttributeRef.Tag) Then
						Try
							If oAttributeRef.TextString <> tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag) Then
								oAttributeRef.TextString = tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & tBlockRefData.AtribValuesDic.Item(oAttributeRef.Tag) & "|", "1_AcadTr-InsertBlockRef")
						End Try
					End If
					oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)
					moTransaction.AddNewlyCreatedDBObject(oAttributeRef, True)
				Next
			End If

			Return tBlockRefObjID
		Else
			Return ObjectId.Null
		End If
	End Function

	Public Shared Function InsertGetBlockRef(ByVal tBlockRecObjId As ObjectId, ByVal tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d, Optional ByVal dScaleFactor As Double = 1.0,
														  Optional ByVal iAcadColor As Integer = 256, Optional sLayer As String = Nothing, Optional dRotation As Double = 0.0) As BlockReference

		Dim oBlockRef As BlockReference
		Dim tBlockRefObjID As ObjectId

		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")
			oBlockRef = New BlockReference(tInsertPoint, tBlockRecObjId)

			If dScaleFactor <> 0.0 Then
				DMCommon.Debug.MsgBox("141122_4", dScaleFactor)
				oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScaleFactor)


			End If
			If Not String.IsNullOrEmpty(sLayer) Then
				Try
					oBlockRef.Layer = sLayer
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & "Layer='" & sLayer & "'", "InsertBlockRef_AcadTransaction")
				End Try

			End If
			oBlockRef.Rotation = dRotation
			oBlockRef.ColorIndex = iAcadColor
			tBlockRefObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
			DMCommon.Debug.MsgBox("141122_5", tBlockRefObjID)

			moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)


			DMCommon.Debug.MsgBox("141122_6", oBlockRef Is Nothing)


			Return oBlockRef
		Else
			Return Nothing
		End If

	End Function
	Public Shared Sub AddNewlyCreatedDBObject(ByVal oDBObject As DBObject)
		moTransaction.AddNewlyCreatedDBObject(oDBObject, True)
	End Sub
	Public Shared Sub InsertNewBlock(ByVal bAddDrawOrderTable As Boolean, Optional ByVal sLayer As String = Nothing)
		Dim tAcObjID As ObjectId
		Dim oBlockRef As BlockReference
		If moModelSpaceTableRecord IsNot Nothing Then
			Try
				Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(0.0, 0.0, 0.0)
				oBlockRef = New BlockReference(tInsertPoint, mtNewBlockObjId)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "InsertNewBlock_1")
				Return
			End Try
			Try
				If Not String.IsNullOrEmpty(sLayer) Then
					oBlockRef.Layer = sLayer
				End If
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & sLayer, "InsertNewBlock_2")
			End Try
			Try
				tAcObjID = moModelSpaceTableRecord.AppendEntity(oBlockRef)
				moTransaction.AddNewlyCreatedDBObject(oBlockRef, True)
				If bAddDrawOrderTable AndAlso mcolDrawOrderIDs IsNot Nothing Then
					mcolDrawOrderIDs.Add(tAcObjID)
				End If
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "InsertNewBlock")
			End Try
		Else
			MessageBox.Show("SysError #1845", DMCommon.Functions.AppName)
		End If
	End Sub
	Public Shared Function AddDBobjectToNewBlock(ByVal oDBObject As DBObject) As ObjectId
		Try
			Dim tObjID As ObjectId = moNewBlockDB.AddDBObject(oDBObject)
			Return tObjID
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & oDBObject.ToString(), "AddDBobjectToNewBlock")
		End Try
	End Function
	Public Shared Sub AddToNewBlock(ByVal oEntity As Entity)

		Try
			moBlockTableRecord.AppendEntity(oEntity)
			moTransaction.AddNewlyCreatedDBObject(oEntity, True)

			'	Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
		Catch oEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oEntity.ToString(), "AcadTransaction - AddToNewBlock_278")

			'	System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AppentNewBlock")
		End Try
	End Sub
	Public Shared Function GetNewBlockName() As String
		If moBlockTableRecord IsNot Nothing Then
			Return moBlockTableRecord.Name
		Else
			Return String.Empty
		End If
	End Function

	Public Shared Function zzGetLayers(ByVal iAppID As Integer, ByVal saLayers() As String, ByVal bCreate As Boolean) As ObjectId()
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		'April DMCommon.Functions.DispArray(saLayers, "zzGetLayers 23_888") 
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
				Dim oLayerDef As AcadLayerDef = New AcadLayerDef(iAppID, sLayer)
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer
				If oLayerDef.Exists Then
					If oLayerDef.AcadColor IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.AcadColor
					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
					If oLayerTableRecord.IsOff = oLayerDef.On Then
						oLayerTableRecord.IsOff = Not oLayerDef.On
					End If
				End If
				oLayerTable.UpgradeOpen()
				taObjectIDs(iIndex) = oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
			Else
				taObjectIDs(iIndex) = ObjectId.Null
			End If
		Next
		oLayerTable.Dispose()
		oLayerTable = Nothing
		'April	System.Windows.Forms.MessageBox.Show("after", "zzGetLayers 23_889") 'April
		Return taObjectIDs
	End Function
	Public Shared Function SetCurrentLayer(ByVal oLayerDef As AcadLayerDef, ByVal bCreate As Boolean, ByVal bClear As Boolean, ByVal bCheckIsOn As Boolean, ByVal bAutoStartTransaction As Boolean, Optional iExtensionNumber As Integer = 0) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)

		Dim tLayerObjID As ObjectId = zzGetLayer(oLayerDef, bCreate, bClear, bCheckIsOn, iExtensionNumber)
		If tLayerObjID.IsNull Then
			Return False
		Else
			If bAutoStartTransaction Then
				Terminate()
			End If
			If tLayerObjID.IsNull Then
				Return False
			ElseIf oCurrentDatabase.Clayer <> tLayerObjID Then
				Try
					oCurrentDatabase.Clayer = tLayerObjID
					mtCurrentLayerObjID = tLayerObjID
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "-SetCurrentLayer")
				End Try
			End If
			Return True
		End If
	End Function
	Public Shared Sub SaveCurrentLayer()
		Try
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			mtPrevCurrentLayerObjID = oCurrentDatabase.Clayer

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			DMCommon.Debug.MsgBox("-SaveCurrentLayer", oAcadEx.ErrorStatus.ToString(), oAcadEx.Message, oAcadEx.StackTrace)
		End Try

	End Sub
	Public Shared Sub RestoreCurrentLayer()

		Try
			If Not mtPrevCurrentLayerObjID.IsNull Then
				Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
				oCurrentDatabase.Clayer = mtPrevCurrentLayerObjID
			End If


		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			DMCommon.Debug.MsgBox("-RestoreCurrentLayer", oAcadEx.ErrorStatus.ToString(), oAcadEx.Message, oAcadEx.StackTrace)
		End Try

	End Sub
	Public Shared Function GetCurrentLayer() As String
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tLayerAcObjId As ObjectId
		Dim oLayerTableRecord As LayerTableRecord
		tLayerAcObjId = oCurrentDatabase.Clayer
		oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForRead), LayerTableRecord)
		Return oLayerTableRecord.Name
	End Function
	Public Shared Function SetCurrentLayer(sLayer As String, ByVal iAppID As Integer, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean) As Boolean 'jjj
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tLayerObjID As ObjectId = zzGetLayer(sLayer, iAppID, 0, bCreate, bCheckIsOn)
		'DMCommon.Debug.MsgBox("13_070", sLayer, tLayerObjID)
		If tLayerObjID.IsNull Then
			Return False
		ElseIf oCurrentDatabase.Clayer <> tLayerObjID Then
			oCurrentDatabase.Clayer = tLayerObjID
			mtCurrentLayerObjID = tLayerObjID
		Else
			mtCurrentLayerObjID = tLayerObjID
		End If
		Return True

	End Function
	Public Shared Function SetCurrentLayer(sLayer As String, sLayerTemplate As String, ByVal iAppID As Integer, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean) As Boolean 'jjj
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tLayerObjID As ObjectId = zzGetLayer(sLayer, iAppID, 0, bCreate, bCheckIsOn, sLayerTemplate)

		If tLayerObjID.IsNull Then
			Return False
		ElseIf oCurrentDatabase.Clayer <> tLayerObjID Then
			oCurrentDatabase.Clayer = tLayerObjID
			mtCurrentLayerObjID = tLayerObjID
		Else
			mtCurrentLayerObjID = tLayerObjID
		End If
		Return True

	End Function
	Public Shared Function SetCurrentLayer(ByRef sLayer As String, ByVal iAppID As Integer, ByVal iLayerFunction As enLayerFunction, ByVal bCreate As Boolean, ByVal bCheckIsOn As Boolean, ByVal bAutoStartTransaction As Boolean) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)

		Dim tLayerObjID As ObjectId = zzGetLayer(sLayer, iAppID, iLayerFunction, bCreate, bCheckIsOn)
		AcadDocument.WriteDebugMessage("Set Layer=" & sLayer)
		If bAutoStartTransaction Then
			Terminate()
		End If

		If tLayerObjID.IsNull Then
			Return False
		ElseIf oCurrentDatabase.Clayer <> tLayerObjID Then
			Try
				oCurrentDatabase.Clayer = tLayerObjID
			Catch oEx As Exception
				Dim sMsg As String
				If sLayer.Length = 0 Then
					sMsg = iLayerFunction.ToString()
				Else
					sMsg = sLayer
				End If
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sMsg, "AcadTransaction - SetCurrentLayer")
			End Try

			mtCurrentLayerObjID = tLayerObjID
		Else
			mtCurrentLayerObjID = tLayerObjID
		End If
		Return True

	End Function
	Public Shared Function CreateLayer(ByVal saLayers() As String, ByVal iAppID As Integer, ByVal bAutoStartTransaction As Boolean) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim oLayerDef As AcadLayerDef = Nothing
		Dim sLayer As String
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_1a")
			Return False
		End Try
		For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
			sLayer = saLayers(iIndex)
			If Not oLayerTable.Has(sLayer) Then
				oLayerDef = New AcadLayerDef(iAppID, sLayer)
				If oLayerDef.Exists Then
					Try
						oLayerTableRecord = New LayerTableRecord()
						oLayerTableRecord.Name = sLayer
						If oLayerDef.Exists Then
							If oLayerDef.AcadColor IsNot Nothing Then
								oLayerTableRecord.Color = oLayerDef.AcadColor
							End If
							If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
								oLayerTableRecord.LineWeight = oLayerDef.LineWeight
							End If
							If oLayerTableRecord.IsOff = oLayerDef.On Then
								oLayerTableRecord.IsOff = Not oLayerDef.On
							End If
						End If
						oLayerTable.UpgradeOpen()
						oLayerTable.Add(oLayerTableRecord)
						moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_2")
						Return False
					End Try
				Else
					System.Windows.Forms.MessageBox.Show("Layer '" & sLayer & "' was not found", "AcadTransaction - CreateLayer_3")
					Return False
				End If
			End If
		Next
		oLayerTable = Nothing
		Return True
	End Function

	Public Shared Function CreateLayer(ByVal sLayer As String, Optional shColorIndex As Short = -1S, Optional iLineWeight As Autodesk.AutoCAD.DatabaseServices.LineWeight = LineWeight.ByLayer, Optional bIsOn As Boolean = True) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord

		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(moTransaction Is Nothing), "AcadTransaction - CreateLayer_1b")
			Return False
		End Try

		If Not oLayerTable.Has(sLayer) Then
			Try
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer

				If shColorIndex <> -1S Then
					oLayerTableRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, shColorIndex)
				End If
				If iLineWeight <> LineWeight.ByLayer Then
					oLayerTableRecord.LineWeight = iLineWeight
				End If
				If Not bIsOn Then
					oLayerTableRecord.IsOff = True
				End If
				oLayerTableRecord.Description = "Boris"
				oLayerTable.UpgradeOpen()
				oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show("Layer '" & sLayer & "' was not found", "AcadTransaction - CreateLayer_3")
				Return False
			End Try


		End If


		oLayerTable = Nothing
		Return True

	End Function

	Public Shared Function IsRegistrated(ByVal sAppName As String) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oRegAppTable As RegAppTable = Nothing
		Try
			oRegAppTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.RegAppTableId, OpenMode.ForWrite), RegAppTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - RegistrateApp_1")
			Return False
		End Try
		Return oRegAppTable.Has(sAppName)
	End Function
	Public Shared Function RegistrateApp(ByVal sAppName As String) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oRegAppTable As RegAppTable = Nothing
		Dim oRegAppTableRecord As RegAppTableRecord

		Try
			oRegAppTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.RegAppTableId, OpenMode.ForWrite), RegAppTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - RegistrateApp_1")
			Return False
		End Try

		If Not oRegAppTable.Has(sAppName) Then
			Try
				oRegAppTableRecord = New RegAppTableRecord()
				oRegAppTableRecord.Name = sAppName
				oRegAppTable.UpgradeOpen()
				oRegAppTable.Add(oRegAppTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oRegAppTableRecord, True)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - RegistrateApp_2")
				Return False
			End Try
		End If
		oRegAppTable = Nothing
		Return True
	End Function
	Public Shared Function CreateLayer(ByVal oLayerDef As AcadLayerDef, ByVal bAutoStartTransaction As Boolean, Optional iExtensionNumber As Integer = 0) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim sLayer As String = oLayerDef.NamePlusExtension(iExtensionNumber)
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_1c")
			Return False
		End Try
		Dim bRes As Boolean
		If oLayerTable.Has(sLayer) Then
			bRes = True
		Else
			Try
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer
				If oLayerDef.Exists Then
					If oLayerDef.AcadColor IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.AcadColor
					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
					If oLayerTableRecord.IsOff = oLayerDef.On Then
						oLayerTableRecord.IsOff = Not oLayerDef.On
					End If
				End If
				oLayerTable.UpgradeOpen()
				oLayerTable.Add(oLayerTableRecord)
				moTransaction.AddNewlyCreatedDBObject(oLayerTableRecord, True)
				bRes = True
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & oLayerDef.Name, "AcadTransaction - CreateLayer_12")
				bRes = False
			End Try
		End If
		oLayerTable = Nothing
		Return bRes
	End Function
	Public Shared Function GetAllLayers() As System.Collections.Generic.IList(Of String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetAllLayers")

		End Try
		Dim oLayerEnum As Autodesk.AutoCAD.DatabaseServices.SymbolTableEnumerator = oLayerTable.GetEnumerator()
		oLayerEnum.Reset()
		oLayerEnum.MoveNext()

		Dim tLayerAcObjID As ObjectId

		Dim oList As System.Collections.Generic.IList(Of String) = New System.Collections.Generic.List(Of String)

		Do While oLayerEnum.MoveNext()
			tLayerAcObjID = oLayerEnum.Current
			oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), LayerTableRecord)
			'	AcadDocument.WriteMessage("Block=" & oBlockTableRecord.Name & ":" & CStr(oBlockTableRecord.IsAnonymous) & ":" & CStr(oBlockTableRecord.Explodable) & ":" & CStr(oBlockTableRecord.PathName) & ";")
			oList.Add(oLayerTableRecord.Name)
		Loop
		Return oList
	End Function
	Public Shared Function CreateLayer(ByVal sLayer As String, ByVal iAppID As Integer, ByVal iLayerFunction As enLayerFunction, ByVal bAutoStartTransaction As Boolean) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		zzAutoStart(bAutoStartTransaction)
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord
		Dim oLayerDef As AcadLayerDef = Nothing
		Try
			oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - CreateLayer_1d")
			Return False
		End Try
		If sLayer.Length = 0 Then
			oLayerDef = New AcadLayerDef(iAppID, iLayerFunction)
			sLayer = oLayerDef.Name
		End If
		If sLayer.Length = 0 Then
			System.Windows.Forms.MessageBox.Show("Layer doesn'n exist", "AcadTransaction - CreateLayer_21")
		End If
		If Not oLayerTable.Has(sLayer) Then
			If Not oLayerDef.Exists Then
				oLayerDef = New AcadLayerDef(iAppID, sLayer)
			End If
			Try
				oLayerTableRecord = New LayerTableRecord()
				oLayerTableRecord.Name = sLayer
				If oLayerDef.Exists Then
					If oLayerDef.AcadColor IsNot Nothing Then
						oLayerTableRecord.Color = oLayerDef.AcadColor
					End If
					If oLayerTableRecord.LineWeight <> oLayerDef.LineWeight Then
						oLayerTableRecord.LineWeight = oLayerDef.LineWeight
					End If
					If oLayerTableRecord.IsOff = oLayerDef.On Then
						oLayerTableRecord.IsOff = Not oLayerDef.On
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
			oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", msEmpty, True)
			Start()
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
			System.Windows.Forms.MessageBox.Show("Layer '" & sLayer & " was not found!!", "AcadTransaction - TestLayers!!!")
			Exit Sub
		End If

		Try
			oLayerTableRecord = DirectCast(moTransaction.GetObject(tObjectID, OpenMode.ForWrite), LayerTableRecord)
			oLayerTableRecord.IsOff = Not oLayerTableRecord.IsOff
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tObjectID.ToString(), sCaption & " AcadTransaction - TestSetLayer_2!!!")
		End Try
		oLayerTable = Nothing
		Terminate()
		If oDocLock IsNot Nothing Then
			oDocLock.Dispose()
		End If

	End Sub
	Public Shared Function GetLayersOffStatus(Optional tLayerList As DMCommon.dmList = Nothing) As Dictionary(Of ObjectId, Boolean)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord = Nothing
		Dim dicLayersOff As Dictionary(Of ObjectId, Boolean) = New Dictionary(Of ObjectId, Boolean)()
		Dim bLayerListExists As Boolean = tLayerList.Exists
		Dim bContainsLayer As Boolean

		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
				For Each tLayerAcObjId As ObjectId In oLayerTable
					oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForWrite), LayerTableRecord)
					dicLayersOff.Add(tLayerAcObjId, oLayerTableRecord.IsOff)
					If bLayerListExists Then
						bContainsLayer = tLayerList.Contains(oLayerTableRecord.Name)
						If bContainsLayer = oLayerTableRecord.IsOff Then
							oLayerTableRecord.IsOff = Not bContainsLayer
						End If
					End If


				Next

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LayerExists")
			End Try
			Return dicLayersOff
		Else
			System.Windows.Forms.MessageBox.Show("!!!Design", "AcadTransaction - LayerExists")
			Return Nothing
		End If
	End Function
	Public Shared Sub SetLayerOffStatus(sLayer As String, bIsOff As Boolean)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord = Nothing


		Dim tLayerObjID As ObjectId

		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
				tLayerObjID = oLayerTable.Item(sLayer)
				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
				oLayerTableRecord.IsOff = bIsOff
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LayerExists")
			End Try

		Else
			System.Windows.Forms.MessageBox.Show("Design", "AcadTransaction - LayerExists")

		End If
	End Sub
	Public Shared Sub SetLayersOffStatus(tLayerObjID As ObjectId, bIsOff As Boolean)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord = Nothing




		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)

				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
				oLayerTableRecord.IsOff = bIsOff
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOffStatus")
			End Try

		Else
			System.Windows.Forms.MessageBox.Show("Design", "AcadTransaction - SetLayersOffStatus")

		End If
	End Sub
	Public Shared Sub SetLayersOffStatus(colLayers As IEnumerable(Of String), bIsOff As Boolean)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord = Nothing
		Dim bOff As Boolean
		Dim tLayerObjID As ObjectId
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
				For Each sLayer As String In colLayers
					'oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
					tLayerObjID = oLayerTable.Item(sLayer)
					oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
					If oLayerTableRecord.IsOff <> bOff Then
						oLayerTableRecord.IsOff = bOff
					End If
				Next

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "SetLayersOffStatus-AcadTransaction")
			End Try

		Else
			System.Windows.Forms.MessageBox.Show("Design", "2:SetLayersOffStatus-AcadTransaction")

		End If
	End Sub
	Public Shared Sub SetLayersOffStatus(dicLayersOff As Dictionary(Of ObjectId, Boolean))
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim oLayerTableRecord As LayerTableRecord = Nothing
		Dim bOff As Boolean
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
				For Each tLayerAcObjId As ObjectId In oLayerTable
					oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerAcObjId, OpenMode.ForWrite), LayerTableRecord)
					If dicLayersOff.TryGetValue(tLayerAcObjId, bOff) Then
						If oLayerTableRecord.IsOff <> bOff Then
							oLayerTableRecord.IsOff = bOff
						End If
					End If
				Next

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LayerExists")
			End Try

		Else
			System.Windows.Forms.MessageBox.Show("Design", "AcadTransaction - LayerExists")

		End If
	End Sub
	Public Shared Function LayerExists(sLayer As String) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LayerExists")
			End Try
			Return oLayerTable.Has(sLayer)
		Else
			System.Windows.Forms.MessageBox.Show("!!!Design", "AcadTransaction - LayerExists")
			Return False
		End If
	End Function
	Public Shared Function GetLayerInfo(sLayer As String, ByRef bIsOff As Boolean, ByRef iColor As Autodesk.AutoCAD.Colors.Color) As Boolean
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim tLayerObjID As ObjectId
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - LayerExists")
			End Try
			If oLayerTable.Has(sLayer) Then
				tLayerObjID = oLayerTable.Item(sLayer)
				Dim oLayerTableRecord As LayerTableRecord
				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
				bIsOff = oLayerTableRecord.IsOff
				iColor = oLayerTableRecord.Color

				Return True
			Else
				Return False
			End If
		Else
			System.Windows.Forms.MessageBox.Show("!!!Design", "AcadTransaction - LayerExists")
			Return False
		End If
	End Function

	Public Shared Function GetLayersExist(sLayerList As String) As String
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		Dim saLayers() As String = Split(sLayerList, ",")
		Dim sResLayerList As String = String.Empty
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForWrite), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetLayersExist")
			End Try
			For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
				If oLayerTable.Has(saLayers(iIndex)) Then
					If sResLayerList.Length = 0 Then
						sResLayerList &= ","
					End If
					sResLayerList &= saLayers(iIndex)
				End If
			Next
		End If
		Return sResLayerList
	End Function

	Public Shared Function GetLayersExist(colLayers As System.Collections.Generic.IEnumerable(Of String)) As HashSet(Of String)
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTable As LayerTable = Nothing
		'Dim saLayers() As String = Split(sLayerList, ",")
		'Dim colLayers As System.Collections.ObjectModel.Collection(Of String) = New System.Collections.ObjectModel.Collection(Of String)
		Dim hsLayers As HashSet(Of String) = New HashSet(Of String)()
		Dim sResLayerList As String = String.Empty
		If moTransaction IsNot Nothing Then
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetLayersExist")
			End Try

			For Each sLayer As String In colLayers
				If oLayerTable.Has(sLayer) Then
					'colLayers.Add(sLayer)
					hsLayers.Add(sLayer)
				End If
			Next
		End If
		Return hsLayers
	End Function
	Public Shared Function LayersIsEmpty(sLayerList As String) As Boolean
		Dim oBlockTable As BlockTable = Nothing
		Dim oModelSpaceBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim oEntity As Entity
		Dim oResList As System.Collections.Generic.IList(Of Polyline) = New System.Collections.Generic.List(Of Polyline)
		Dim iLayerCount As Integer
		Dim sCheckLayer As String

		sLayerList = GetLayersExist(sLayerList)
		If Not String.IsNullOrEmpty(sLayerList) Then
			Dim saLayers() As String = Split(sLayerList, ",")
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
			oModelSpaceBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)

			For Each tAcObjID As ObjectId In oModelSpaceBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead)
				oEntity = DirectCast(oDBObject, Entity)
				sCheckLayer = oEntity.Layer
				For iIndex As Integer = 0 To iLayerCount
					If sCheckLayer = saLayers(iIndex) Then
						Return False
					End If
				Next
			Next
		End If
		Return True
	End Function
	Public Shared Sub CheckLayerExists(ByVal iAppID As Integer, ByVal saLayers() As String)

	End Sub
	Public Shared Function SetLayersOn(ByVal iAppID As Integer, ByVal saLayers() As String, ByVal bOn As Boolean, ByVal bAutoStartTransaction As Boolean) As System.Windows.Forms.CheckState
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTableRecord As LayerTableRecord = Nothing
		Dim taLayerObjID() As ObjectId
		Dim bOff As Boolean
		Dim iState As CheckState = CheckState.Checked

		zzAutoStart(bAutoStartTransaction)
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing

		Try
			oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", msEmpty, True)

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOn_0")
		End Try
		taLayerObjID = zzGetLayers(iAppID, saLayers, False)

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
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iIndex) & vbCrLf & taLayerObjID(iIndex).ToString(), "SetLayersOn_2")
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
	Public Shared Function LayersExist(ByVal saLayers() As String, ByVal bAll As Boolean) As Boolean
		If saLayers IsNot Nothing AndAlso saLayers.GetUpperBound(0) >= 0 Then
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			Dim oLayerTable As LayerTable = Nothing

			Dim oLayerTableRecord As LayerTableRecord = Nothing


			Dim bRes As Boolean = True
			Dim bAllExist As Boolean = True
			Dim bOneExists As Boolean = False
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOn!!!")
				Return False
			End Try


			Dim bFirstLayer As Boolean = True
			For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
				If oLayerTable.Has(saLayers(iIndex)) Then
					bOneExists = True
				Else
					bAllExist = False
					' System.Windows.Forms.MessageBox.Show("Layer '" & saLayers(iIndex) & "' was not found", "04_350")
				End If
			Next
			If bAll Then
				Return bAllExist
			Else
				Return bOneExists
			End If
		Else
			Return False
		End If


	End Function
	Public Shared Sub SetLayersOnExcept(ByVal hsLayers As HashSet(Of String), ByVal bOn As Boolean) 'As IEnumerable(Of String)
		If hsLayers IsNot Nothing Then
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			Dim oLayerTable As LayerTable = Nothing
			Dim oLayerTableRecord As LayerTableRecord = Nothing

			Dim bRes As Boolean = True
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOnExc!!!")

			End Try
			For Each tLayerObjID As ObjectId In oLayerTable
				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
				If Not hsLayers.Contains(oLayerTableRecord.Name) Then

					If oLayerTableRecord.IsOff = bOn AndAlso Not oLayerTableRecord.IsFrozen Then
						Try
							oLayerTableRecord.IsOff = Not bOn
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "SetLayersOnExc_1")
						End Try

					End If
				End If
			Next
		End If
	End Sub
	Public Shared Sub SetLayersOn(ByVal tLayerList As DMCommon.dmList, ByVal bOn As Boolean)
		If tLayerList.Exists Then
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			Dim oLayerTable As LayerTable = Nothing
			Dim oLayerTableRecord As LayerTableRecord = Nothing
			Dim colResLayers As ObjectModel.ObservableCollection(Of String) = New ObjectModel.ObservableCollection(Of String)()
			Dim bRes As Boolean = True
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOn!!!")

			End Try
			For Each tLayerObjID As ObjectId In oLayerTable
				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)

				'  AcadDocument.WriteDebugMessage("!!AWe " & tLayerList.List & "; " & oLayerTableRecord.Name & "; " & CStr(tLayerList.Contains(oLayerTableRecord.Name)))


				If tLayerList.Contains(oLayerTableRecord.Name) Then

					If oLayerTableRecord.IsOff = bOn AndAlso Not oLayerTableRecord.IsFrozen Then
						Try
							oLayerTableRecord.IsOff = Not bOn
							colResLayers.Add(oLayerTableRecord.Name)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "SetLayersOn_1")
						End Try

					End If
				End If
			Next

		End If
	End Sub
	Public Shared Function SetLayersOnExcept(ByVal oLayerList As DMCommon.dmList, ByVal bOn As Boolean) As IEnumerable(Of String)
		If oLayerList.Exists Then
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			Dim oLayerTable As LayerTable = Nothing
			Dim oLayerTableRecord As LayerTableRecord = Nothing
			Dim colResLayers As ObjectModel.ObservableCollection(Of String) = New ObjectModel.ObservableCollection(Of String)()
			Dim bRes As Boolean = True
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOnExc!!!")

			End Try

			For Each tLayerObjID As ObjectId In oLayerTable
				oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
				If Not oLayerList.Contains(oLayerTableRecord.Name) Then

					If oLayerTableRecord.IsOff = bOn AndAlso Not oLayerTableRecord.IsFrozen Then
						Try
							oLayerTableRecord.IsOff = Not bOn
							colResLayers.Add(oLayerTableRecord.Name)
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "SetLayersOnExc_1")
						End Try

					End If

				Else


				End If
			Next
			Return colResLayers
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function SetLayersOn(ByVal saLayers() As String, ByVal bOn As Boolean) As Boolean

		If saLayers IsNot Nothing AndAlso saLayers.GetUpperBound(0) >= 0 Then
			Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
			Dim oLayerTable As LayerTable = Nothing

			Dim oLayerTableRecord As LayerTableRecord = Nothing

			Dim tLayerObjID As ObjectId

			Dim iState As CheckState = CheckState.Checked
			Dim bRes As Boolean = True
			Try
				oLayerTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.LayerTableId, OpenMode.ForRead), LayerTable)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - SetLayersOn!!!")
				Return False
			End Try
			Dim bBefore, bAfter As Boolean

			' Dim bFirstLayer As Boolean = True
			For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
				If oLayerTable.Has(saLayers(iIndex)) Then
					tLayerObjID = oLayerTable.Item(saLayers(iIndex))
					If Not tLayerObjID.IsNull Then
						Try

							oLayerTableRecord = DirectCast(moTransaction.GetObject(tLayerObjID, OpenMode.ForWrite), LayerTableRecord)
							bBefore = oLayerTableRecord.IsOff
							If oLayerTableRecord.IsOff = bOn Then
								oLayerTableRecord.IsOff = Not bOn
							End If
							bAfter = oLayerTableRecord.IsOff
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & CStr(iIndex) & vbCrLf & saLayers(iIndex), "SetLayersOn_1")
						End Try
						'  System.Windows.Forms.MessageBox.Show(bOn.ToString() & vbCrLf & bBefore.ToString() & ":" & bAfter.ToString(), "04_357")
					End If
				Else
					bRes = False
					' System.Windows.Forms.MessageBox.Show("Layer '" & saLayers(iIndex) & "' was not found", "04_350")
				End If
			Next
			Return bRes
		Else
			Return False
		End If


	End Function
	Public Shared Function LayersIsOn(ByVal iAppID As Integer, ByVal saLayers() As String, ByVal bAutoStartTransaction As Boolean) As System.Windows.Forms.CheckState
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oLayerTableRecord As LayerTableRecord
		Dim iState As CheckState = CheckState.Checked
		Dim bOff As Boolean
		Dim taLayerObjID() As ObjectId
		zzAutoStart(bAutoStartTransaction)
		taLayerObjID = zzGetLayers(iAppID, saLayers, False)
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
	Public Shared Sub EraseDBObject(ByVal tAcObjID As ObjectId)
		If miModelSpaceOpenMode = OpenMode.ForWrite Then
			Dim oDBObject As DBObject
			Try
				oDBObject = GetDBObject(tAcObjID, OpenMode.ForWrite)
				If oDBObject IsNot Nothing Then
					oDBObject.Erase()
				End If
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - EraseDBObject")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Modelspace is Readonly", "AcadTransaction - EraseDBObjects_2")
		End If

	End Sub
	Public Shared Sub EraseDBObject(ByVal oDBObject As DBObject)
		If miModelSpaceOpenMode = OpenMode.ForWrite Then

			Try

				If oDBObject IsNot Nothing Then
					oDBObject.Erase()
				End If
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - EraseDBObject")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Modelspace is Readonly", "AcadTransaction - EraseDBObjects_3")
		End If

	End Sub

	Public Shared Sub EraseDBObjects(ByVal colAcObjIDs As ObjectIdCollection)
		If miModelSpaceOpenMode = OpenMode.ForWrite Then
			Dim oDBObject As DBObject
			Try
				For Each tAcObjID As ObjectId In colAcObjIDs
					oDBObject = GetDBObject(tAcObjID, OpenMode.ForWrite)
					If oDBObject IsNot Nothing Then
						oDBObject.Erase()
					End If

				Next
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - EraseDBObjects")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Modelspace is Readonly", "AcadTransaction - EraseDBObjects_2")

		End If

	End Sub
	Public Shared Sub EraseDBPoints(ByVal oPoints As System.Collections.Generic.IList(Of TPlnPoint))
		If miModelSpaceOpenMode = OpenMode.ForWrite Then
			Dim oDBObject As DBObject
			Dim tAcObjID As ObjectId
			Try
				For Each oPoint As TPlnPoint In oPoints
					tAcObjID = oPoint.AcObjID
					If Not tAcObjID.IsNull Then
						oDBObject = GetDBObject(tAcObjID, OpenMode.ForWrite)
						oDBObject.Erase()
					End If
				Next
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - EraseDBObjects")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("Modelspace is Readonly", "AcadTransaction - EraseDBObjects_2")

		End If

	End Sub
	Public Shared ReadOnly Property ModelSpaceObjID() As ObjectId
		Get
			If moModelSpaceTableRecord IsNot Nothing Then
				Return moModelSpaceTableRecord.ObjectId
			Else
				Return ObjectId.Null
			End If

		End Get
	End Property
	Public Shared Function GetModelSpaceMode(ByRef iModelSpaceOpenMode As OpenMode) As Boolean
		If moModelSpaceTableRecord IsNot Nothing Then
			miModelSpaceOpenMode = iModelSpaceOpenMode
			Return True
		Else
			Return False
		End If
	End Function
	Public Shared ReadOnly Property FreezeLayer() As String
		Get
			Return msFreezeLayer
		End Get
	End Property

	Public Shared Sub OpenModelSpace(ByVal iMode As OpenMode, Optional ByVal bOpenDrawOrderTable As Boolean = False)
		Dim tDrawOrderTableID As Autodesk.AutoCAD.DatabaseServices.ObjectId
		Try
			moBlockTable = DirectCast(moTransaction.GetObject(HostApplicationServices.WorkingDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			moModelSpaceTableRecord = DirectCast(moTransaction.GetObject(moBlockTable.Item(BlockTableRecord.ModelSpace), iMode, False), BlockTableRecord)
			miModelSpaceOpenMode = iMode
			If bOpenDrawOrderTable AndAlso iMode = OpenMode.ForWrite Then
				tDrawOrderTableID = moModelSpaceTableRecord.DrawOrderTableId
				moDrawOrderTable = DirectCast(moTransaction.GetObject(tDrawOrderTableID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DrawOrderTable)
				mcolDrawOrderIDs = New ObjectIdCollection()
			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "OpenModelSpace")
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
		If mdicHandles IsNot Nothing Then

			mdicHandles = Nothing
		End If
	End Sub
	Public Shared Function GetExtents2d(ByVal tAcObjID As ObjectId) As Extents2d
		Dim oEntity As Entity = zzGetEntity(tAcObjID)
		Dim tExtents3d As Extents3d = oEntity.GeometricExtents
		Return New Extents2d(tExtents3d.MinPoint.X, tExtents3d.MinPoint.Y, tExtents3d.MaxPoint.X, tExtents3d.MaxPoint.Y)
	End Function
	Public Shared Function GetBoundingBox(ByVal tAcObjID As ObjectId, ByVal bHighlight As Boolean) As TPlnBoundingBox
		Dim oEntity As Entity = zzGetEntity(tAcObjID)
		If oEntity IsNot Nothing Then
			Dim tExtents3d As Extents3d = oEntity.GeometricExtents
			If bHighlight Then
				Try
					oEntity.Highlight()
				Catch oEx As Exception
				End Try
			End If
			Return New TPlnBoundingBox(tExtents3d)
		Else
			Return Nothing
		End If

	End Function
	Public Shared Function GetAttribDef(ByVal sBlockName As String, ByVal bMustExist As Boolean, Optional ByVal bAutoStartTransaction As Boolean = False) As String()
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim iAttribDefIndex As Integer = -1
		Dim tBlockAcObjID As ObjectId = New ObjectId()
		Dim saOut() As String = Nothing
		zzAutoStart(bAutoStartTransaction)
		'DMCommon.Debug.MsgBox("09_549ss", sBlockName, moTransaction)
		If moTransaction IsNot Nothing Then
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			Try
				If oBlockTable.Has(sBlockName) Then
					tBlockAcObjID = oBlockTable.Item(sBlockName)
				ElseIf bMustExist Then
					AcadDocument.WriteMessage("AcadTransaction - GetAttribDef: Block '" & sBlockName & "' was not found!")
				End If
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - GetAttribDef_2")
			End Try

			If Not tBlockAcObjID.IsNull Then
				oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			End If

			If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
				For Each tAcObjID As ObjectId In oBlockTableRecord
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					sRXClassName = oDBObject.GetRXClass().Name
					If sRXClassName = AcadConst.AcadAttributeDefName Then
						iAttribDefIndex += 1
						ReDim Preserve saOut(iAttribDefIndex)
						Try
							oAttributeDef = DirectCast(oDBObject, AttributeDefinition)
							saOut(iAttribDefIndex) = oAttributeDef.Tag
						Catch oEx As Exception
						End Try
					End If
				Next
				oBlockTableRecord = Nothing
				oBlockTable = Nothing
				Return saOut
			Else
				oBlockTable = Nothing
				Return Nothing
			End If
		Else
			System.Windows.Forms.MessageBox.Show("Transaction Is Nothing", "AcadTransaction-GetAttribDef")
			Return Nothing
		End If
		If bAutoStartTransaction Then
			Terminate()
		End If
	End Function
	Public Shared Function GetAttribDef(ByVal sBlockName As String, ByRef oaAttribDefs As AttributeDefinition(), Optional bIfNotExistsMsg As Boolean = False) As ObjectId
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim iAttribDefIndex As Integer = -1
		Dim tBlockAcObjID As ObjectId = New ObjectId()
		' Dim saOut() As String = Nothing

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Try
			If oBlockTable.Has(sBlockName) Then
				tBlockAcObjID = oBlockTable.Item(sBlockName)
			Else
				If bIfNotExistsMsg Then
					AcadDocument.WriteMessage("AcadTransaction - GetAttribDef: Block '" & sBlockName & "' was not found")
				End If
				Return ObjectId.Null
			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - GetAttribDef_2")
		End Try

		If Not tBlockAcObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If
		If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
			For Each tAcObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name
				If sRXClassName = AcadConst.AcadAttributeDefName Then
					iAttribDefIndex += 1
					ReDim Preserve oaAttribDefs(iAttribDefIndex)
					Try
						oAttributeDef = DirectCast(oDBObject, AttributeDefinition)
						oaAttribDefs(iAttribDefIndex) = oAttributeDef
					Catch oEx As Exception
					End Try
				End If
			Next

			oBlockTable = Nothing
		End If
		Return tBlockAcObjID
	End Function
	Public Shared Function GetAttribDef(ByVal tBlockDefObjID As ObjectId) As AttributeDefinition()
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim iAttribDefIndex As Integer = -1
		'	Dim tBlockAcObjID As ObjectId = New ObjectId()
		Dim oaAttribDefs() As AttributeDefinition = Nothing

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)


		If Not tBlockDefObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockDefObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If
		If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
			'oBlockTableRecord.
			For Each tAcObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name
				If sRXClassName = AcadConst.AcadAttributeDefName Then
					iAttribDefIndex += 1
					ReDim Preserve oaAttribDefs(iAttribDefIndex)
					Try
						oAttributeDef = DirectCast(oDBObject, AttributeDefinition)
						oaAttribDefs(iAttribDefIndex) = oAttributeDef
					Catch oEx As Exception
					End Try
				End If
			Next

			oBlockTableRecord = Nothing
			oBlockTable = Nothing
			Return oaAttribDefs
		Else
			oBlockTable = Nothing
			Return Nothing
		End If


	End Function
	Public Shared Function GetAttribIndex(ByVal sBlockName As String, ByVal sTag As String) As Integer
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim iAttribDefIndex As Integer = -1
		Dim tBlockAcObjID As ObjectId = New ObjectId()
		Dim saOut() As String = Nothing

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Try
			If oBlockTable.Has(sBlockName) Then
				tBlockAcObjID = oBlockTable.Item(sBlockName)
			Else
				AcadDocument.WriteMessage("AcadTransaction - GetAttribIndex: " & "Block '" & sBlockName & "' was not found")
			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - GetAttribDef_2")
		End Try

		If Not tBlockAcObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If
		If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
			For Each tAcObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name
				If sRXClassName = AcadConst.AcadAttributeDefName Then
					iAttribDefIndex += 1

					Try
						oAttributeDef = DirectCast(oDBObject, AttributeDefinition)
						If oAttributeDef.Tag = sTag Then
							Return iAttribDefIndex
						End If
					Catch oEx As Exception
					End Try
				End If
			Next

			oBlockTableRecord = Nothing
			oBlockTable = Nothing
			Return -1
		Else
			oBlockTable = Nothing
			Return Nothing
		End If
	End Function

	Public Shared Function GetAttribIndices(ByVal sBlockName As String, ByVal bMustExist As Boolean, ByVal dicTags As Dictionary(Of String, Integer), ByRef iaIndices() As Integer) As ObjectId
		Dim oBlockTable As BlockTable
		Dim oBlockTableRecord As BlockTableRecord = Nothing
		Dim oDBObject As DBObject
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sRXClassName As String
		Dim oAttributeDef As AttributeDefinition
		Dim iAttribDefIndex As Integer = -1
		Dim tBlockAcObjID As ObjectId = New ObjectId()
		Dim saOut() As String = Nothing
		'   Dim i As Integer 
		' DMCommon.Debug.MsgBox("09_333", sBlockName, moTransaction)
		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		'  DMCommon.ExcelLog.SetNextValue(i, 0, sBlockName, sBlockName, sBlockName, sBlockName, sBlockName, sBlockName, sBlockName)
		Try
			If oBlockTable.Has(sBlockName) Then
				tBlockAcObjID = oBlockTable.Item(sBlockName)
			Else
				If bMustExist Then
					AcadDocument.WriteMessage("AcadTransaction - GetAttribIndices: Block Def '" & sBlockName & "' was not found")
				End If

				Return ObjectId.Null
			End If
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "AcadTransaction - GetAttribDef_2")
			Return ObjectId.Null
		End Try

		If Not tBlockAcObjID.IsNull Then
			oBlockTableRecord = DirectCast(moTransaction.GetObject(tBlockAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
		End If


		If oBlockTableRecord IsNot Nothing AndAlso oBlockTableRecord.HasAttributeDefinitions Then
			Dim iFieldIndex As Integer
			For Each tAcObjID As ObjectId In oBlockTableRecord

				oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name

				If sRXClassName = AcadConst.AcadAttributeDefName Then
					iAttribDefIndex += 1

					Try
						oAttributeDef = DirectCast(oDBObject, AttributeDefinition)

						If dicTags IsNot Nothing AndAlso dicTags.TryGetValue(oAttributeDef.Tag, iFieldIndex) Then
							'   DMCommon.ExcelLog.SetValue(i, 20, oAttributeDef.Tag, iFieldIndex)
							iaIndices(iFieldIndex) = iAttribDefIndex

						End If
					Catch oEx As Exception
					End Try
				End If
			Next

			oBlockTableRecord = Nothing
			oBlockTable = Nothing


		End If
		Return tBlockAcObjID
	End Function
	Public Shared Function GetAttribText(ByVal oBlockRef As BlockReference) As String()
		Dim colAttributes As AttributeCollection
		Try
			colAttributes = oBlockRef.AttributeCollection()
		Catch oEx As Exception
			Dim sBlockName As String
			If oBlockRef IsNot Nothing Then
				sBlockName = oBlockRef.Name
			Else
				sBlockName = "BlockRef Is Nothing"
			End If
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_74:" & oEx.Message & "; " & sBlockName)
			Return Nothing
		End Try
		Return zzGetAttribText(colAttributes)
	End Function

	Public Shared Function GetAttribText(ByVal oBlockRef As BlockReference, ByVal iaAttribIndices() As Integer) As String()
		Dim colAttributes As AttributeCollection
		Try
			colAttributes = oBlockRef.AttributeCollection()
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_51:" & oEx.Message)
			Return Nothing
		End Try
		Return zzGetAttribText(colAttributes, iaAttribIndices)
	End Function
	Public Shared Function GetAttribText(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, ByVal iaAttribIndices() As Integer) As String()
		Const iExNo As Integer = 20
		Dim colAttributes As AttributeCollection
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesIndUB As Integer = -1
		Dim iValuesAttrUB As Integer = -1
		'	Dim iValuesUB As Integer = -1

		Dim sTest As String = "a"
		Try
			colAttributes = zzGetBlockAttrib(tAcObjID, bMustExist, bAcadPoint)


		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_51:" & oEx.Message)
			Return Nothing
		End Try

		If colAttributes IsNot Nothing AndAlso colAttributes.Count > 0 Then
			Try
				iValuesIndUB = iaAttribIndices.GetUpperBound(0)
				If iValuesIndUB = -1 Then
					System.Windows.Forms.MessageBox.Show("Attrib Def Count = 0", "AcadTransaction - GetAttribText_12")
				End If
				iValuesAttrUB = colAttributes.Count - 1

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetAttribText_1")
			End Try
			Dim tAttribObjID As ObjectId
			Dim iOutIndex As Integer = 0
			If iValuesIndUB >= 0 Then  'AndAlso (colAttributes.Count = iValuesUB + 1)
				Dim saOutText(iValuesIndUB) As String
				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				Dim iAttribIndex As Integer
				sTest = "b"
				For iIndex As Integer = 0 To iValuesIndUB
					Try
						sTest = "b"

						iAttribIndex = iaAttribIndices(iIndex)
						sTest = "bs"
						If iAttribIndex >= 0 AndAlso iAttribIndex <= iValuesAttrUB Then
							sTest = "bt"
							tAttribObjID = colAttributes.Item(iAttribIndex)
							sTest = "c"
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
							sTest = "d"
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							sTest = "f"
							saOutText(iIndex) = (oAttribRef.TextString).Trim
						End If
					Catch oEx As System.Exception
						Dim sMsg As String = tAcObjID.ToString()
						If iaAttribIndices Is Nothing Then
							sMsg &= "iaAttribIndices Is Nothing"
						Else
							sMsg &= " AttribIndicesUB=" & CStr(iaAttribIndices.GetUpperBound(0))
						End If
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "Index=" & CStr(iIndex) & vbCrLf & "AttribIndex=" & CStr(iAttribIndex) & vbCrLf & sMsg & vbCrLf & sTest, "AcadTransaction")
					End Try
				Next
				Return saOutText
			Else
				AcadDocument.WriteException(miErrClassNo, iExNo, "Invalid Block Attributes number")
				AcadDocument.WriteMessage(CStr(iValuesIndUB) & ":" & CStr(iValuesAttrUB))

				'System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - GetAttribText_3")
				Return Nothing
			End If
		Else
			If bMustExist Then
				AcadDocument.WriteMessage("AcadTransaction - GetAttribText_4a" & ": Block Attributes were not found ?- " & bMustExist.ToString() & "; point? " & bAcadPoint.ToString())
			End If

			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function

	Public Shared Function GetAttribText(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, ByVal iAttribIndex As Integer) As String
		'	Const iExNo As Integer = 24
		Dim colAttributes As AttributeCollection
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		'	Dim iValuesIndUB As Integer = -1
		'	Dim iValuesAttrUB As Integer = -1
		'	Dim iValuesUB As Integer = -1

		Dim sTest As String = "a"
		Try
			colAttributes = zzGetBlockAttrib(tAcObjID, bMustExist, bAcadPoint)
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_5:" & oEx.Message)
			Return Nothing
		End Try

		If colAttributes IsNot Nothing AndAlso colAttributes.Count > 0 Then

			Dim tAttribObjID As ObjectId
			Dim iOutIndex As Integer = 0
			'
			'	Dim saOutText(iValuesIndUB) As String
			'  Dim oDBObject As DBObject
			Dim oAttribRef As AttributeReference

			sTest = "b"

			Try

				sTest = "bt"
				tAttribObjID = colAttributes.Item(iAttribIndex)
				sTest = "c"
				oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
				sTest = "d"
				oAttribRef = DirectCast(oDBObject, AttributeReference)
				sTest = "f"
				Return (oAttribRef.TextString).Trim

			Catch oEx As System.Exception
				Dim sMsg As String = tAcObjID.ToString()

				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "AttribIndex=" & CStr(iAttribIndex) & vbCrLf & sMsg & vbCrLf & sTest, "AcadTransaction")
				Return Nothing
			End Try


			'
		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_4s " & tAcObjID.ToString() & " : Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function
	Private Shared Function zzGetAttribText(ByVal colAttributes As AttributeCollection, ByVal sAttribTag As String) As String
		'Const iExNo As Integer = 80

		If colAttributes IsNot Nothing Then
			Dim iValuesUB As Integer = -1
			Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
			Dim tAttribObjID As ObjectId
			Dim oAttribRef As AttributeReference
			For iAttributeIndex As Integer = 0 To colAttributes.Count - 1
				Try
					tAttribObjID = colAttributes.Item(iAttributeIndex)
					oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
					oAttribRef = DirectCast(oDBObject, AttributeReference)
					If sAttribTag = oAttribRef.Tag Then
						Return oAttribRef.TextString
					End If
				Catch oEx As System.Exception
					Dim sMsg As String = String.Empty
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "zzGetAttribText-AcadTransaction")
				End Try
			Next

		End If
		Return Nothing
	End Function
	Private Shared Function zzGetAttribText(ByVal colAttributes As AttributeCollection, ByVal saAttribTags() As String) As String()
		Const iExNo As Integer = 70
		Dim iValuesUB As Integer = -1
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		If colAttributes IsNot Nothing Then
			Try
				iValuesUB = saAttribTags.GetUpperBound(0)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetAttribText_1b")
			End Try
			Dim tAttribObjID As ObjectId
			Dim sInputAttribTag As String



			If iValuesUB >= 0 Then  'AndAlso (colAttributes.Count = iValuesUB + 1)
				Dim saOutText(saAttribTags.GetUpperBound(0)) As String
				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				'Dim iAttribIndex As Integer

				For iAttributeIndex As Integer = 0 To colAttributes.Count - 1
					Try

						tAttribObjID = colAttributes.Item(iAttributeIndex)
						oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
						oAttribRef = DirectCast(oDBObject, AttributeReference)
						sInputAttribTag = oAttribRef.Tag
						For iValueIndex As Integer = 0 To saAttribTags.GetUpperBound(0)
							If sInputAttribTag = saAttribTags(iValueIndex) Then
								saOutText(iValueIndex) = oAttribRef.TextString
							End If
						Next

					Catch oEx As System.Exception
						Dim sMsg As String = String.Empty

						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "Index=" & CStr(iAttributeIndex) & vbCrLf & sMsg, "AcadTransaction")
					End Try
				Next
				Return saOutText
			Else
				AcadDocument.WriteException(miErrClassNo, iExNo, "Invalid Block Attributes number")
				AcadDocument.WriteMessage(CStr(iValuesUB) & ":" & CStr(colAttributes.Count))

				'System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - GetAttribText_3")
				Return Nothing
			End If
		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_14" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function
	Private Shared Function zzGetAttribText(ByVal colAttributes As AttributeCollection) As String()
		'	Const iExNo As Integer = 40
		Dim iValuesUB As Integer = -1
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		If colAttributes IsNot Nothing Then
			Try
				iValuesUB = colAttributes.Count - 1
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetAttribText_1c")
			End Try
			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 Then  'AndAlso (colAttributes.Count = iValuesUB + 1)
				Dim saOutText(iValuesUB) As String
				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				For iAttribIndex As Integer = 0 To iValuesUB
					Try

						tAttribObjID = colAttributes.Item(iAttribIndex)
						oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
						oAttribRef = DirectCast(oDBObject, AttributeReference)

						saOutText(iAttribIndex) = oAttribRef.TextString

					Catch oEx As System.Exception
						Dim sMsg As String = String.Empty

						sMsg &= "AttribIndicesUB=" & CStr(iValuesUB)

						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sMsg, "AcadTransaction")
					End Try
				Next
				Return saOutText
			Else
				'AcadDocument.WriteException(miErrClassNo, iExNo, "Invalid Block Attributes number")
				'AcadDocument.WriteMessage(CStr(iValuesUB) & ":" & CStr(colAttributes.Count))

				'System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - GetAttribText_3")
				Return Nothing
			End If
		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_4" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function


	Private Shared Function zzGetAttribText(ByVal colAttributes As AttributeCollection, ByVal iaAttribIndices() As Integer) As String()
		Const iExNo As Integer = 40
		Dim iValuesUB As Integer = -1
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		If colAttributes IsNot Nothing Then
			Try
				iValuesUB = iaAttribIndices.GetUpperBound(0)
			Catch oEx As System.Exception
				DMCommon.Debug.MsgBox("1a:zzGetAttribText_1a-AcadTransaction", oEx.Message, iaAttribIndices, iaAttribIndices.GetUpperBound(0), colAttributes, colAttributes.Count)

			End Try
			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 Then  'AndAlso (colAttributes.Count = iValuesUB + 1)
				Dim saOutText(iValuesUB) As String
				'  Dim oDBObject As DBObject
				Dim oAttribRef As AttributeReference
				Dim iAttribIndex As Integer

				For iIndex As Integer = 0 To iValuesUB
					Try

						iAttribIndex = iaAttribIndices(iIndex)

						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iaAttribIndices(iIndex))
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForRead)
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							saOutText(iIndex) = oAttribRef.TextString

						End If
					Catch oEx As System.Exception
						Dim sMsg As String = String.Empty
						If iaAttribIndices Is Nothing Then
							sMsg &= "iaAttribIndices Is Nothing"
						Else
							sMsg &= "AttribIndicesUB=" & CStr(iaAttribIndices.GetUpperBound(0))
						End If
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "Index=" & CStr(iIndex) & vbCrLf & sMsg, "AcadTransaction")
					End Try
				Next
				Return saOutText
			Else
				AcadDocument.WriteException(miErrClassNo, iExNo, "Invalid Block Attributes number")
				AcadDocument.WriteMessage(CStr(iValuesUB) & ":" & CStr(colAttributes.Count))

				'System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - GetAttribText_3")
				Return Nothing
			End If
		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_4" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function
	Public Shared Function GetAttribText(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, ByVal sAttribTags As String) As String

		Dim colAttributes As AttributeCollection
		Dim iValuesIndUB As Integer = -1
		Dim iValuesAttrUB As Integer = -1
		'	Dim iValuesUB As Integer = -1

		Dim sTest As String = "a"
		Try
			colAttributes = zzGetBlockAttrib(tAcObjID, bMustExist, bAcadPoint)
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_5:" & oEx.Message)
			Return Nothing
		End Try

		If colAttributes IsNot Nothing AndAlso colAttributes.Count > 0 Then
			Return zzGetAttribText(colAttributes, sAttribTags)



		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_25a" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function
	Public Shared Function GetAttribText(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, ByVal saAttribTags() As String) As String()

		Dim colAttributes As AttributeCollection
		Dim iValuesIndUB As Integer = -1
		Dim iValuesAttrUB As Integer = -1
		'	Dim iValuesUB As Integer = -1

		Dim sTest As String = "a"
		Try
			colAttributes = zzGetBlockAttrib(tAcObjID, bMustExist, bAcadPoint)
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_5:" & oEx.Message)
			Return Nothing
		End Try

		If colAttributes IsNot Nothing AndAlso colAttributes.Count > 0 Then
			Return zzGetAttribText(colAttributes, saAttribTags)



		Else
			AcadDocument.WriteMessage("AcadTransaction - GetAttribText_22a" & ": Block Attributes were not found")
			'System.Windows.Forms.MessageBox.Show("Block Attributes were not found", "AcadTransaction - GetAttribText_4")
			Return Nothing
		End If

	End Function
	Public Shared Function UpdateAttribText(ByVal tBlockRefAcObjID As ObjectId, ByVal saAttribText() As String, ByVal sLayer As String) As Boolean
		Dim oBlockRef As BlockReference = GetBlockRef(tBlockRefAcObjID, OpenMode.ForWrite)
		Dim colAttributes As AttributeCollection
		Dim bLayerSuccess As Boolean
		Dim bAttribSuccess As Boolean

		If Not String.IsNullOrEmpty(sLayer) AndAlso oBlockRef.Layer <> sLayer Then
			Try
				oBlockRef.Layer = sLayer
				bLayerSuccess = True
			Catch oEx As Exception
				bLayerSuccess = False
			End Try
		Else
			bLayerSuccess = True
		End If


		Try
			colAttributes = oBlockRef.AttributeCollection
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_40" & ": Block Attributes were not found " & tBlockRefAcObjID.ToString)
			Return False
		End Try



		If colAttributes IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				bAttribSuccess = zzUpdateAttribText(colAttributes, saAttribText)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetBlockAttrib_1e")
				bAttribSuccess = False
			End Try
		Else
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_49" & ": Block Attributes were not found " & tBlockRefAcObjID.ToString)
			bAttribSuccess = False
		End If

		Return bLayerSuccess AndAlso bAttribSuccess



	End Function
	Public Shared Function UpdateAttribText(ByVal tBlockRefAcObjID As ObjectId, ByVal dicAtribValues As Generic.Dictionary(Of String, String)) As Boolean
		Dim colAttributes As AttributeCollection
		Try
			colAttributes = zzGetBlockAttrib(tBlockRefAcObjID)
			If colAttributes IsNot Nothing Then
				Return zzUpdateAttribText(colAttributes, dicAtribValues)
			Else
				Return False
			End If

		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_47" & ": Block Attributes were not found " & tBlockRefAcObjID.ToString)
			Return False
		End Try
		'	Dim iValuesUB As Integer = -1


	End Function
	Public Shared Function UpdateAttribText(oBlockRef As BlockReference, ByVal dicAtribValues As Generic.Dictionary(Of String, String)) As Boolean
		Dim colAttributes As AttributeCollection

		'	Dim iValuesUB As Integer = -1

		If oBlockRef IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				Return zzUpdateAttribText(colAttributes, dicAtribValues)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetBlockAttrib_1g")
				Return False
			End Try
		Else
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return False
		End If
	End Function
	Public Shared Function UpdateAttribVisibility(oBlockRef As BlockReference, Optional ByVal iaAttribIndices() As Integer = Nothing, Optional ByVal baAttribInvisible() As Boolean = Nothing, Optional bAllInvisible As Boolean = False) As Boolean
		Dim colAttributes As AttributeCollection

		'	Dim iValuesUB As Integer = -1

		If oBlockRef IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				If colAttributes IsNot Nothing Then
					Return zzUpdateAttribVisibility(colAttributes, iaAttribIndices, baAttribInvisible, bAllInvisible)
				Else
					Return True
				End If

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetBlockAttrib_1f")
				Return False
			End Try
		Else
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return False
		End If
	End Function
	Public Shared Function UpdateAttribText(oBlockRef As BlockReference, ByVal iaAttribIndices() As Integer, ByVal saAttribText() As String) As Boolean
		Dim colAttributes As AttributeCollection

		'	Dim iValuesUB As Integer = -1

		If oBlockRef IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				''  DMCommon.Debug.MsgBox("09_793", DMCommon.Debug.ColCount(colAttributes), DMCommon.Debug.ColCount(iaAttribIndices), DMCommon.Debug.ColCount(saAttribText))
				' DMCommon.Functions.DispArray(iaAttribIndices, "iaAttribIndices ")
				Return zzUpdateAttribText(colAttributes, iaAttribIndices, saAttribText)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "2:UpdateAttribText-AcadTransaction")
				Return False
			End Try
		Else
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return False
		End If
	End Function
	Public Shared Function UpdateAttribText(ByVal tBlockRefAcObjID As ObjectId, ByVal bMustExist As Boolean, ByVal iaAttribIndices() As Integer, ByVal saAttribText() As String, Optional ByVal sBlockName As String = msEmpty, Optional ByVal sLayerName As String = msEmpty) As Boolean
		Dim colAttributes As AttributeCollection
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1
		'	Dim sTestAttribValue As String = "-|-|-"
		'	Dim sTestAttribTag As String = "tg"
		Dim bAcadPoint As Boolean
		Try
			colAttributes = zzGetBlockAttrib(tBlockRefAcObjID, bMustExist, bAcadPoint, sBlockName, sLayerName)
		Catch oEx As Exception
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_40" & ": Block Attributes were not found " & tBlockRefAcObjID.ToString)
			Return False
		End Try
		If colAttributes Is Nothing Then
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_41" & ": Block Attributes were not found " & tBlockRefAcObjID.ToString)
			Return False
		Else

			Try
				iValuesUB = iaAttribIndices.GetUpperBound(0)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - UpdateAttribText_1b")
				Return False
			End Try
			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 Then
				Dim oAttribRef As AttributeReference
				Dim iAttribIndex As Integer
				Dim sTestTag As String = ""
				For iIndex As Integer = 0 To colAttributes.Count - 1
					tAttribObjID = colAttributes.Item(iIndex)
					oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
					oAttribRef = DirectCast(oDBObject, AttributeReference)
					sTestTag &= oAttribRef.Tag & "|"
				Next
				'	AcadDocument.WriteMessage("AttribTags= " & sTestTag)
				For iIndex As Integer = 0 To iValuesUB
					Try
						iAttribIndex = iaAttribIndices(iIndex)
						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iAttribIndex)
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							'
							If saAttribText(iIndex) IsNot Nothing AndAlso oAttribRef.TextString <> saAttribText(iIndex) Then
								oAttribRef.TextString = saAttribText(iIndex)
							End If
							oAttribRef.Draw()
						End If
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "index=" & CStr(iIndex), "AcadTransaction - UpdateAttribText_2a_" & ": '" & saAttribText(iAttribIndex) & "'")
					End Try
				Next

				Return True
			Else
				System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - UpdateAttribText_3")
				Return False
			End If

		End If
	End Function

	Public Shared Sub Highlight(ByVal colObjectIds As ObjectIdCollection)
		Dim oEntity As Entity
		Try
			Dim iIndex As Integer = 0
			For Each tAcObjID As ObjectId In colObjectIds
				oEntity = DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				oEntity.Highlight()
				iIndex += 1
			Next
			AcadDocument.WriteMessage("Highlight=" & CStr(iIndex))
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - HighLight")
		End Try
	End Sub
	Public Shared Sub Highlight(ByVal tAcObjID As ObjectId)
		Dim oEntity As Entity
		Try

			oEntity = DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
			If oEntity IsNot Nothing Then
				oEntity.Highlight()
			Else
				System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "AcadTransaction1 - HighLight")
			End If


		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - HighLight")
		End Try
	End Sub
	Public Shared Sub Unhighlight(ByVal tAcObjID As ObjectId)
		Dim oEntity As Entity
		Try

			oEntity = DirectCast(moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)
			If oEntity IsNot Nothing Then
				oEntity.Unhighlight()
			Else
				System.Windows.Forms.MessageBox.Show(tAcObjID.ToString(), "AcadTransaction1 - Unhighlight")
			End If


		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace, "AcadTransaction - Unhighlight")
		End Try
	End Sub
	Public Shared Function HasAcadPoint(ByVal sLayer As String, ByVal bAutoStartTransaction As Boolean) As Boolean
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
			If sRXClassName = AcadConst.AcadPointName Then
				Try
					oEntity = DirectCast(oDBObject, Entity)
					If oEntity.Layer = sLayer Then
						Return True
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - ClearAcadPoints" & vbCrLf & oEx.Message)
				End Try
			End If
		Next
		If bAutoStartTransaction Then
			Terminate()
		End If
		Return False

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
			If sRXClassName = AcadConst.AcadPointName Then
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
	Public Shared Function PointsToBlocks(ByVal sLayer As String, ByVal sBlockPath As String, ByVal sBlockName As String, ByVal bAutoStartTransaction As Boolean) As ObjectIdCollection 'New 9
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oDBPoint As DBPoint
		Dim oResList As System.Collections.Generic.IList(Of TPlnPoint) = New System.Collections.Generic.List(Of TPlnPoint)
		Dim tBlockRefAcObjID As ObjectId
		zzAutoStart(bAutoStartTransaction)
		Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim tBlockAcObjID As ObjectId = DMAcadExt.AcadTransaction.OpenBlockDB(sBlockPath, sBlockName, oaAttribDefs)
		Dim colBlockRefs As ObjectIdCollection = New ObjectIdCollection()
		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)

		If Not tBlockAcObjID.IsNull Then
			For Each tAcObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				sRXClassName = oDBObject.GetRXClass().Name
				If sRXClassName = AcadConst.AcadPointName Then
					Try
						oDBPoint = DirectCast(oDBObject, DBPoint)
						If oDBPoint.Layer = sLayer Then
							tBlockRefAcObjID = InsertBlockRef(tBlockAcObjID, oDBPoint.Position)
							If Not tBlockRefAcObjID.IsNull Then
								colBlockRefs.Add(tBlockRefAcObjID)
							End If

						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - GetAcadPoints" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If

		If bAutoStartTransaction Then
			Terminate()
		End If
		Return colBlockRefs

	End Function
	Public Shared Function GetAcadPoints(ByVal sLayer As String, ByVal bAutoStartTransaction As Boolean) As System.Collections.Generic.IList(Of TPlnPoint)
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oDBPoint As DBPoint
		Dim oResList As System.Collections.Generic.IList(Of TPlnPoint) = New System.Collections.Generic.List(Of TPlnPoint)
		zzAutoStart(bAutoStartTransaction)

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)
		For Each tAcObjID As ObjectId In oBlockTableRecord
			oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			sRXClassName = oDBObject.GetRXClass().Name
			If sRXClassName = AcadConst.AcadPointName Then
				Try
					oDBPoint = DirectCast(oDBObject, DBPoint)
					If oDBPoint.Layer = sLayer Then
						oResList.Add(New TPlnPoint(oDBPoint))
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - GetAcadPoints" & vbCrLf & oEx.Message)
				End Try
			End If
		Next
		If bAutoStartTransaction Then
			Terminate()
		End If
		Return oResList
	End Function
	Public Shared Function GetEntitiesByLayers(ByVal sLayers As String) As ObjectIdCollection

		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
			Dim sEntityLayer As String
			Dim oEntity As Entity
			Dim tLayersList As DMCommon.dmList = New DMCommon.dmList(sLayers)

			Try

				Dim sRXClassName As String
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					sRXClassName = oEntity.GetRXClass().Name
					sEntityLayer = oEntity.Layer
					If tLayersList.Contains(sEntityLayer) Then
						colResIds.Add(tAcObjId)
					End If
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1487")
			End Try
			Return colResIds
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetEntitiesByLayer(ByVal sLayer As String) As ObjectIdCollection
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim oEntity As Entity

		'	Dim sRXClassName As String
		Dim sEntityLayer As String
		Dim colObjIDs As ObjectIdCollection = New ObjectIdCollection()

		'	Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)

		For Each tAcObjID As ObjectId In oBlockTableRecord
			oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead)
			'sRXClassName = oDBObject.GetRXClass().Name

			oEntity = TryCast(oDBObject, Entity)

			If oEntity IsNot Nothing Then
				sEntityLayer = oEntity.Layer
				If sEntityLayer = sLayer Then
					colObjIDs.Add(tAcObjID)
					'	AcadDocument.WriteMessage("##23: " & oEntity.GetType().ToString())
				End If
			End If
		Next
		Return colObjIDs
	End Function
	Public Shared Function GetMPolygons(ByVal sLayer As String, iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode) As System.Collections.Generic.IList(Of Entity)
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oPgonEntity As Entity
		Dim oResList As System.Collections.Generic.IList(Of Entity) = New System.Collections.Generic.List(Of Entity)
		'Dim o As Autodesk.AutoCAD.DatabaseServices.ImpEntity
		If moTransaction IsNot Nothing Then


			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
			oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)

			For Each tAcObjID As ObjectId In oBlockTableRecord
				oDBObject = moTransaction.GetObject(tAcObjID, iMode)
				oPgonEntity = TryCast(oDBObject, Entity)
				If oPgonEntity IsNot Nothing AndAlso oPgonEntity.Layer = sLayer Then
					sRXClassName = oDBObject.GetRXClass().Name
					If sRXClassName = AcadConst.AcadMPolygonName Then
						Try
							oResList.Add(oPgonEntity)
							'	AcadDocument.WriteMessage("OK!!!!" & sRXClassName & vbCrLf & oDBObject.GetType().ToString() & ":" & oDBObject.GetRXClass().Name)
						Catch oEx As Exception
							AcadDocument.WriteMessage("AcadTransaction - GetMPolygons" & vbCrLf & oEx.Message & vbCrLf & oDBObject.GetType().ToString() & ":" & oDBObject.GetRXClass().Name)
						End Try
					Else
						AcadDocument.WriteMessage("OK - GetMPolygons" & vbCrLf & oDBObject.GetType().ToString() & ":" & oDBObject.GetRXClass().Name)
					End If

				End If
			Next
			'		MessageBox.Show(CStr(oResList.Count) & vbCrLf & sLayer, "01_950")
		Else
			MessageBox.Show("Transaction is Closed", "01_951")
		End If
		Return oResList
	End Function
	Public Shared Function GetAcadPolylines2MPolygons(ByVal sLayer As String, sXDataAppName As String, bIDAddExists As Boolean) As IDictionary(Of Integer, MPolygonOverlay)

		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oDBPolyline As Polyline
		'		Dim oResList As System.Collections.Generic.IList(Of Polyline) = New System.Collections.Generic.List(Of Polyline)
		Dim dicMPolygons As Dictionary(Of Integer, MPolygonOverlay) = New Dictionary(Of Integer, MPolygonOverlay)
		Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
		Dim taTypedValues() As Autodesk.AutoCAD.DatabaseServices.TypedValue
		Dim oValue As System.Object
		Dim iFeatureID As Integer
		Dim iSourceID, iOverlayID, iOverlayID_Add As Integer
		'DMCommon.Debug.MsgBox("13_139S", sLayer)
		Dim oPgon As MPolygonOverlay
		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)
		Dim bIsland As Boolean
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "GetAcadPolylines2MPolygons", sLayer, sXDataAppName, bIDAddExists)
		For Each tAcObjID As ObjectId In oBlockTableRecord
			oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead)
			sRXClassName = oDBObject.GetRXClass().Name
			If sRXClassName = AcadConst.AcadPolylineName Then
				bIsland = False
				Try
					oDBPolyline = DirectCast(oDBObject, Polyline)
					If oDBPolyline.Layer = sLayer Then

						oResBuffer = oDBPolyline.GetXDataForApplication(sXDataAppName)
						taTypedValues = oResBuffer.AsArray()


						oValue = taTypedValues(2).Value
						'	MessageBox.Show(AcadMapApp.DispArray(taTypedValues, "2MP", False) & vbCrLf & oValue.ToString(), "02_008")

						iFeatureID = CInt(oValue)
						oValue = taTypedValues(3).Value
						iSourceID = CInt(oValue)
						oValue = taTypedValues(4).Value
						iOverlayID = CInt(oValue)
						If bIDAddExists AndAlso (taTypedValues(5).TypeCode = DxfCode.ExtendedDataInteger32) Then
							oValue = taTypedValues(5).Value
							iOverlayID_Add = CInt(oValue)
						End If


						If Not dicMPolygons.ContainsKey(iFeatureID) Then
							oPgon = New MPolygonOverlay(tAcObjID)
							oPgon.FeatureID = iFeatureID
							oPgon.SourceID = iSourceID
							oPgon.OverlayID = iOverlayID
							oPgon.FirstHandle = oDBPolyline.Handle

							If bIDAddExists Then
								oPgon.OverlayID_Add = iOverlayID_Add
							End If
						Else
							'MessageBox.Show(CStr(iFeatureID), "05_688")
							bIsland = True
							oPgon = dicMPolygons.Item(iFeatureID)
							oPgon.AddEntity(tAcObjID)
						End If
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Pline->MPolygons", iFeatureID, iSourceID, iOverlayID, iOverlayID_Add, oDBPolyline.StartPoint, bIsland)
						If bIsland Then
							'	AcadDocument.WriteDebugMessage("!!before " & CStr(oPgon.Area))
						End If

						oPgon.AppendLoopFromBoundary(oDBPolyline, False, 0.0001)
						oPgon.BalanceTree()
						If oPgon.Area = 0 Then
							AcadDocument.WriteMessage("Area=0 " & oDBPolyline.Handle.ToString())
						End If

						If bIsland Then
							'	AcadDocument.WriteDebugMessage("!!After " & CStr(oPgon.Area))
						End If
						If bIsland Then
							dicMPolygons.Remove(iFeatureID)
						End If
						If oPgon.Area <> 0 Then
							dicMPolygons.Add(iFeatureID, oPgon)
						End If
						If iSourceID = 69505 Then
							DMCommon.Debug.ExcelLog.SetNextValue(0, "2MPolygons", sLayer, iFeatureID, iSourceID, iOverlayID, oDBPolyline.Area)
						End If
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - GetAcadPolylines2MPolygons" & vbCrLf & oEx.Message & vbCrLf & sLayer & vbCrLf & oEx.StackTrace)
				End Try
			End If
		Next
		'  DMCommon.Debug.MsgBox("07_100", sLayer, sXDataAppName, bIDAddExists, DMCommon.Debug.ColCount(dicMPolygons))
		Return dicMPolygons
	End Function
	Public Shared Function GetAcadPolylines(ByVal sLayer As String, iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode) As System.Collections.Generic.IList(Of Polyline)
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oDBPolyline As Polyline
		Dim oResList As System.Collections.Generic.IList(Of Polyline) = New System.Collections.Generic.List(Of Polyline)

		oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
		oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForRead, False), BlockTableRecord)

		For Each tAcObjID As ObjectId In oBlockTableRecord
			oDBObject = moTransaction.GetObject(tAcObjID, iMode)
			sRXClassName = oDBObject.GetRXClass().Name
			If sRXClassName = AcadConst.AcadPolylineName Then
				Try
					oDBPolyline = DirectCast(oDBObject, Polyline)
					If oDBPolyline.Layer = sLayer Then

						oResList.Add(oDBPolyline)
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - GetAcadPolylines" & vbCrLf & oEx.Message)
				End Try
			End If
		Next

		Return oResList
	End Function
	Public Shared Function GetAcadPolylinesDic(ByVal sLayer As String, iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode) As System.Collections.Generic.IDictionary(Of ObjectId, Polyline)
		'	Dim oBlockTable As BlockTable = Nothing
		'	Dim oBlockTableRecord As BlockTableRecord
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As DBObject
		Dim sRXClassName As String
		Dim oDBPolyline As Polyline
		Dim oResDict As System.Collections.Generic.IDictionary(Of ObjectId, Polyline) = New System.Collections.Generic.Dictionary(Of ObjectId, Polyline)



		For Each tAcObjID As ObjectId In moModelSpaceTableRecord
			oDBObject = moTransaction.GetObject(tAcObjID, iMode)
			sRXClassName = oDBObject.GetRXClass().Name
			If sRXClassName = AcadConst.AcadPolylineName Then
				Try
					oDBPolyline = DirectCast(oDBObject, Polyline)
					If oDBPolyline.Layer = sLayer Then

						oResDict.Add(tAcObjID, oDBPolyline)
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - GetAcadPoints" & vbCrLf & oEx.Message)
				End Try
			End If
		Next

		Return oResDict
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
					Case AcadConst.AcadPolylineName, AcadConst.AcadLineName, AcadConst.AcadArcName, AcadConst.Acad2dPolylineName
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
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - LinkExists")
		End Try
		Return bResult
	End Function

	Public Shared Function LayerListIsDirty(ByVal sLayerList As String) As Boolean
		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity
		Dim tLayerList As DMCommon.dmList = New DMCommon.dmList(sLayerList)
		Dim bOpened As Boolean

		If moModelSpaceTableRecord IsNot Nothing Then
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")
					bOpened = False
				End Try

				If bOpened Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If tLayerList.Contains(oEntity.Layer) Then
							Return True
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		Else
			System.Windows.Forms.MessageBox.Show("!!!Design", "AcadTransaction - LayerListIsDirty")
		End If
		Return False
	End Function
	Public Shared Sub ClearLayers(ByVal tLayerDef As AcadLayerDef)
		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity

		Dim bOpened As Boolean

		If moModelSpaceTableRecord IsNot Nothing Then
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")
					bOpened = False
				End Try

				If bOpened Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If tLayerDef.Contains(oEntity.Layer) Then
							oDBObject.Erase()
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If
	End Sub


	Public Shared Sub ClearLayerList(ByVal sLayerList As String)
		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity
		Dim tLayerList As DMCommon.dmList = New DMCommon.dmList(sLayerList)
		Dim bOpened As Boolean

		If moModelSpaceTableRecord IsNot Nothing Then
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")
					bOpened = False
				End Try

				If bOpened Then
					Try
						oEntity = DirectCast(oDBObject, Entity)


						If tLayerList.Contains(oEntity.Layer) Then
							oDBObject.Erase()
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If
	End Sub
	Public Shared Sub ClearLayerSet(ByVal hsLayers As HashSet(Of String))

		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity
		Dim bOpened As Boolean

		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(AcadDocument.IsLocked().ToString() & vbCrLf & DMCommon.Functions.DispArray(hsLayers, "", True), "21_999 DocLock")

			'DMCommon.Debug.MsgBox("hsLayers", DMCommon.Debug.GetListArray(hsLayers.Count - 1, hsLayers))


			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")
					bOpened = False
				End Try

				If bOpened Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If hsLayers.Contains(oEntity.Layer) Then
							oDBObject.Erase()
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If
	End Sub
	Public Shared Function HasEntity(ByVal hsLayers As HashSet(Of String)) As Boolean

		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity


		If moModelSpaceTableRecord IsNot Nothing Then

			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")

				End Try


				Try
					oEntity = DirectCast(oDBObject, Entity)
					If hsLayers.Contains(oEntity.Layer) Then
						'DMCommon.Debug.MsgBox("13_001f", oEntity.Layer, hsLayers.Count)
						Return True
					End If
				Catch oEx As Exception
					AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
				End Try

			Next
		End If
		Return False
	End Function
	Public Shared Sub ChangeLayerSet(ByVal hsSourceLayers As HashSet(Of String), sDestinationLayer As String)

		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity

		Dim bOpened As Boolean

		If moModelSpaceTableRecord IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(AcadDocument.IsLocked().ToString() & vbCrLf & DMCommon.Functions.DispArray(hsLayers, "", True), "21_999 DocLock")

			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByLayerList_3")
					bOpened = False
				End Try

				If bOpened Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If hsSourceLayers.Contains(oEntity.Layer) Then
							oEntity.Layer = sDestinationLayer
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByLayerList_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If
	End Sub
	Public Shared Sub ClearLayerByClassName(ByVal sLayer As String, Optional ByVal sRXClassName As String = msEmpty)

		Dim oDBObject As DBObject = Nothing
		Dim oEntity As Entity
		Dim bOneLayer As Boolean = TopoDef.IsOneLayer(sLayer)
		Dim bForErase As Boolean
		Dim bAllClasses As Boolean = String.IsNullOrEmpty(sRXClassName)
		Dim bOpened As Boolean

		If Not bOneLayer Then
			sLayer = TopoDef.AddDelim(sLayer)
		End If
		If moModelSpaceTableRecord IsNot Nothing Then
			For Each tAcObjID As ObjectId In moModelSpaceTableRecord
				Try
					oDBObject = moTransaction.GetObject(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					bOpened = True
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					AcadDocument.WriteMessage(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & "AcadTransaction - ClearLayerByClassName_3")
					bOpened = False
				End Try

				If bOpened AndAlso (bAllClasses OrElse oDBObject.GetRXClass().Name = sRXClassName) Then
					Try
						oEntity = DirectCast(oDBObject, Entity)
						If bOneLayer Then
							bForErase = (oEntity.Layer = sLayer)
						Else
							bForErase = sLayer.Contains(TopoDef.AddDelim(oEntity.Layer))
						End If
						If bForErase Then
							oDBObject.Erase()
						End If
					Catch oEx As Exception
						AcadDocument.WriteMessage("AcadTransaction - ClearLayerByClassName_2" & vbCrLf & oEx.Message)
					End Try
				End If
			Next
		End If
	End Sub
	Public Shared Function GetBlockRefs(ByVal sBlockName As String, Optional ByVal sLayers As String = Nothing) As ObjectIdCollection
		Dim saBlockNames() As String = Strings.Split(sBlockName, ",")
		If saBlockNames.GetUpperBound(0) > 1 Then
			Return GetBlockRefs(saBlockNames, sLayers)
		End If
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oBlockAcObjId As ObjectId
		Dim oOutBlockCol As ObjectIdCollection
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			DMAcadExt.AcadDocument.UpdateScreen()
			oOutBlockCol = New ObjectIdCollection()
		End Try

		If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
			Try
				oBlockAcObjId = oBlockTable.Item(sBlockName)
				oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
				Dim oAllBlockCol As ObjectIdCollection
				oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, True)
				'''''Temp Test

				If String.IsNullOrEmpty(sLayers) Then
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
			If oBlockTable Is Nothing Then
				System.Windows.Forms.MessageBox.Show("BlockTable Is Nothing", "AcadTransaction - zzGetBlockRefs")
			Else
				System.Windows.Forms.MessageBox.Show("Block " & sBlockName & " was not found", "AcadTransaction -!zzGetBlockRefs")
			End If

			oOutBlockCol = New ObjectIdCollection()
		End If
		Return oOutBlockCol
	End Function
	Public Shared Function GetBlockRefs(ByVal saBlockName() As String, Optional ByVal sLayers As String = Nothing) As ObjectIdCollection
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable = Nothing
		Dim oBlockTableRecord As BlockTableRecord
		Dim oBlockAcObjId As ObjectId
		Dim oOutBlockCol As ObjectIdCollection = Nothing
		Dim sBlockName As String
		Dim oEntity As Entity
		Dim sEntityLayerName As String
		Dim sBlockList As String = Nothing
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - GetBlockRefs")
			Return New ObjectIdCollection()
		End Try
		If oBlockTable IsNot Nothing Then
			For iIndex As Integer = 0 To saBlockName.GetUpperBound(0)
				sBlockName = saBlockName(iIndex)
				If sBlockList Is Nothing Then
					sBlockList = sBlockName
				Else
					sBlockList &= "," & sBlockName
				End If
				If oBlockTable.Has(sBlockName) Then
					Try
						oBlockAcObjId = oBlockTable.Item(sBlockName)
						oBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
						Dim oAllBlockCol As ObjectIdCollection
						oAllBlockCol = oBlockTableRecord.GetBlockReferenceIds(True, False)
						If String.IsNullOrEmpty(sLayers) Then
							If oOutBlockCol Is Nothing Then
								oOutBlockCol = oAllBlockCol
							Else
								For Each tObjId As ObjectId In oAllBlockCol
									oOutBlockCol.Add(tObjId)
								Next
							End If
						Else
							If oOutBlockCol Is Nothing Then
								oOutBlockCol = New ObjectIdCollection()
							End If
							sLayers = TopoDef.AddDelim(sLayers)
							For Each tObjId As ObjectId In oAllBlockCol
								oEntity = DirectCast(moTransaction.GetObject(tObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
								sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
								If sLayers.Contains(sEntityLayerName) Then
									oOutBlockCol.Add(tObjId)
								End If
							Next
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "e200a")
						oOutBlockCol = New ObjectIdCollection()
					End Try
				End If
			Next
		End If
		If oOutBlockCol Is Nothing OrElse oOutBlockCol.Count = 0 Then
			System.Windows.Forms.MessageBox.Show("Block (" & sBlockList & ") was not found", "1AcadTransaction - zzGetBlockRefs")
			oOutBlockCol = New ObjectIdCollection()
		End If
		Return oOutBlockCol
	End Function
	Public Shared Function GetBlockRefsNew(ByVal sBlockNames As String, Optional ByVal sLayers As String = msEmpty, Optional colPrevIDs As ObjectIdCollection = Nothing) As ObjectIdCollection
		'	DMCommon.Debug.MsgBox("01_280_BB", moModelSpaceTableRecord IsNot Nothing, DMCommon.Functions.CStrN(sBlockNames, "BlockNames Nothing") & vbCrLf & DMCommon.Functions.CStrN(sLayers, "Line Nothing"))
		Dim colResIds As ObjectIdCollection
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim bAcadPoint As Boolean = String.IsNullOrEmpty(sBlockNames)
			Dim oInsertCondition As InsertCondition = New InsertCondition(bAcadPoint, sBlockNames, sLayers)
			Dim iItemNo As Integer
			Dim oEntity As Entity
			If colPrevIDs IsNot Nothing Then
				colResIds = colPrevIDs
			Else
				colResIds = New ObjectIdCollection()
			End If
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					iItemNo = oInsertCondition.GetInsertType(oEntity)
					If iItemNo <> -1 Then
						colResIds.Add(tAcObjId)
					End If
				Next

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "'" & sBlockNames & "'" & vbCrLf & "'" & sLayers & "'", "e1512")
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!e1512", sBlockNames, sLayers)
			End Try

		Else
			colResIds = New ObjectIdCollection()
			System.Windows.Forms.MessageBox.Show("Modelspace is Nothing", "2:GetBlockRefsNew_AcadTransaction")
		End If
		Return colResIds
	End Function
	Public Shared Function Cleanup(ByVal sLayers As String) As ObjectIdCollection
		Dim oCurrentDatabaseAAA As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim sLayersDel As String
		Dim colResIds As ObjectIdCollection = New ObjectIdCollection()

		Dim oEntity As Entity

		'  Dim iToposUB As Integer = moaTopoDefs.GetUpperBound(0)
		'  sLayersDel = moTopoDefs.LinkLayers
		sLayersDel = TopoDef.AddDelim(sLayers)
		Dim sTest As String = "a"
		Try
			Dim sRXClassName As String
			Dim sEntityLayerName As String
			For Each objId As ObjectId In moModelSpaceTableRecord
				oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
				sRXClassName = oEntity.GetRXClass().Name
				Select Case sRXClassName
					Case AcadConst.AcadArcName
					Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName
						sEntityLayerName = TopoDef.AddDelim(oEntity.Layer)
						If sLayersDel.Contains(sEntityLayerName) Then
							colResIds.Add(objId)
						End If
					Case AcadConst.AcadBlockRefName
					Case Else
				End Select
			Next

		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "e1402")
		End Try
		Return colResIds
	End Function
	Public Shared Function GetLinks(ByVal sLayers As String, Optional ByVal sLineLayers As String = msEmpty) As ObjectIdCollection

		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
			Dim sEntityLayer As String
			Dim oEntity As Entity
			Dim tLinkLayersList As DMCommon.dmList = New DMCommon.dmList(sLayers)
			Dim tLineLinkLayersList As DMCommon.dmList = New DMCommon.dmList(sLineLayers)
			Dim bLineLayerExists As Boolean = Not String.IsNullOrEmpty(sLineLayers)
			Try

				Dim sRXClassName As String
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					sRXClassName = oEntity.GetRXClass().Name
					sEntityLayer = oEntity.Layer
					If bLineLayerExists AndAlso tLineLinkLayersList.Contains(sLineLayers) Then
						Select Case sRXClassName
							Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName
								colResIds.Add(tAcObjId)
						End Select
					ElseIf tLinkLayersList.Contains(sEntityLayer) Then
						If bLineLayerExists Then
							Select Case sRXClassName
								Case AcadConst.AcadPolylineName
									Dim oPolyline As Polyline = DirectCast(oEntity, Polyline)
									If dmLineCleanup.PolylineHasNotArc(oPolyline) Then
										colResIds.Add(tAcObjId)
									End If
								Case AcadConst.AcadLineName
									colResIds.Add(tAcObjId)
								Case AcadConst.Acad2dPolylineName
									Dim oPolyline2d As Polyline2d = DirectCast(oEntity, Polyline2d)
									If zzPolylineHasNotArc(oPolyline2d) Then
										colResIds.Add(tAcObjId)
									End If
							End Select
						Else
							Select Case sRXClassName
								Case AcadConst.AcadPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName
									colResIds.Add(tAcObjId)
							End Select
						End If
						Select Case sRXClassName
							Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName
								'	colResIds.Add(tAcObjId)
						End Select
					End If
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1433")
			End Try
			Return colResIds
		Else
			Return Nothing
		End If
	End Function
	Public Shared Sub Stretch(dValue As Double)
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim oDBObject As DBObject
			Dim sRXClassName As String
			Dim tPoint2d As Point2d
			Dim tPoint3d As Point3d

			Dim tPoint3dAdd As Point3d
			Try

				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oDBObject = GetDBObject(tAcObjId, OpenMode.ForWrite)
					sRXClassName = oDBObject.GetRXClass().Name

					Select Case sRXClassName
						Case AcadConst.AcadLineName
							Dim oLine As Line = DirectCast(oDBObject, Line)

							tPoint3d = oLine.StartPoint
							tPoint3dAdd = oLine.EndPoint
							oLine.StartPoint = tPoint3d.MultiplyBy(dValue)
							oLine.EndPoint = tPoint3dAdd.MultiplyBy(dValue)
						Case AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName


						Case AcadConst.AcadPolylineName
							Dim oDBPolyline As Polyline

							oDBPolyline = DirectCast(oDBObject, Polyline)
							For iIndex As Integer = 0 To oDBPolyline.NumberOfVertices - 1
								tPoint2d = oDBPolyline.GetPoint2dAt(iIndex)
								oDBPolyline.SetPointAt(iIndex, tPoint2d.MultiplyBy(dValue))
							Next

						Case AcadConst.AcadBlockRefName
							Dim oBlockRef As BlockReference = DirectCast(oDBObject, BlockReference)
							Dim colAttributes As AttributeCollection
							Dim tAttribObjID As ObjectId
							Dim oAttribRef As AttributeReference

							tPoint3d = oBlockRef.Position
							oBlockRef.Position = tPoint3d.MultiplyBy(dValue)
							colAttributes = oBlockRef.AttributeCollection()
							For iIndex As Integer = 0 To colAttributes.Count - 1
								tAttribObjID = colAttributes.Item(iIndex)
								oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
								oAttribRef = DirectCast(oDBObject, AttributeReference)
								tPoint3d = oAttribRef.Position
								oAttribRef.Position = tPoint3d.MultiplyBy(dValue)
								DMAcadExt.AcadDocument.WriteMessage(iIndex.ToString() & "; " & oAttribRef.TextString & "; " & tPoint3d.ToString() & "; " & oAttribRef.Position.ToString())

							Next
							DMAcadExt.AcadDocument.WriteMessage(dValue.ToString() & "; " & tPoint3d.ToString() & "; " & oBlockRef.Position.ToString())
						Case AcadConst.AcadPointName
							Dim oDBPoint As DBPoint = DirectCast(oDBObject, DBPoint)
							tPoint3d = oDBPoint.Position
					End Select




				Next
				'	DMCommon.Debug.MsgBox("12_997PP", "CleanupRoundLinks", iTest)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1449")
			End Try

		End If



	End Sub

	Public Shared Sub CleanupRoundLinks(tLayers As DMCommon.dmList, iDigits As Integer)
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim iTest As Integer
			Dim oEntity As Entity
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = GetEntity(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)

					If tLayers.Contains(oEntity.Layer) Then

						TplnSegment.Round(oEntity, True, iDigits)
						iTest += 1
					End If

				Next
				'	DMCommon.Debug.MsgBox("12_997PP", "CleanupRoundLinks", iTest)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1449")
			End Try

		End If

	End Sub
	Public Shared Function CleanupRoundInserts(tBlockNames As DMCommon.dmList, iDigits As Integer, Optional bMarkDouble As Boolean = False) As Dictionary(Of Decimal, TPlnPoint) ' tLayers As DMCommon.dmList,
		If moModelSpaceTableRecord IsNot Nothing Then

			Dim oEntity As Entity
			Dim oBlockRef As BlockReference
			Dim dicPoints As Dictionary(Of Decimal, TPlnPoint) = New Dictionary(Of Decimal, TPlnPoint)()
			Dim tPoint As Point3d
			Dim tPointKey As Decimal
			Dim oSquareMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(MarkBlock.enMarkBlockType.Square)
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = GetEntity(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
					If oEntity.GetRXClass.Name = AcadConst.AcadBlockRefName Then
						oBlockRef = DirectCast(oEntity, BlockReference)
						If tBlockNames.Contains(oBlockRef.Name) Then
							TPlnPoint.RoundPoint(oBlockRef, iDigits)
							tPoint = oBlockRef.Position
							tPointKey = DMAcadExt.TplnPointKey.CoordToKey(tPoint.X, tPoint.Y)
							If bMarkDouble Then
								If dicPoints.ContainsKey(tPointKey) Then
									oSquareMarkBlock.MarkPoint(tPoint, 4S)
								Else
									dicPoints.Add(tPointKey, New TPlnPoint(tPoint))
								End If
							End If

						End If
					End If


				Next
				Return dicPoints
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1449")
				Return dicPoints
			End Try
		Else
			Return Nothing
		End If

	End Function
	Public Shared Function GetLinksNew(ByVal sLayers As String, Optional ByVal sLineLayers As String = msEmpty) As ObjectIdCollection
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colResIds As ObjectIdCollection = New ObjectIdCollection()
			Dim oLinkCondition As LinkCondition = New LinkCondition(sLayers, sLineLayers)
			Dim iLinkType As enLinkType
			Dim oEntity As Entity
			'	Dim tLinkLayersList As DMCommon.dmList = New DMCommon.dmList(sLayers)
			'	Dim tLineLinkLayersList As DMCommon.dmList = New DMCommon.dmList(sLineLayers)
			'Dim bLineLayerExists As Boolean = Not String.IsNullOrEmpty(sLineLayers)
			Try


				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
					iLinkType = oLinkCondition.GetLinkType(oEntity)
					If iLinkType <> enLinkType.OtherLayer AndAlso iLinkType <> enLinkType.SourceLayerArc Then
						colResIds.Add(tAcObjId)
					End If
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1437")
			End Try
			Return colResIds
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function ExplodePLines(ByVal sLayers As String, ByVal bExplode As Boolean, bAllpolylines As Boolean) As TplnPointArray
		'	System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN(sLayers, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(sLineLayers, "Line Nothing"), "01_270_AA")
		'	DMCommon.Debug.MsgBox("01_270_AA", sLayers)
		If moModelSpaceTableRecord IsNot Nothing Then

			Dim oLinkCondition As LinkCondition = New LinkCondition(sLayers)
			Dim oaResPoints As TplnPointArray = New TplnPointArray()
			Dim oEntity As Entity
			Dim oResEntity As Entity
			'Dim oCurve As Curve
			Dim oPolyline As Polyline
			Dim tResObjID As ObjectId
			Dim tList As DMCommon.dmList = New DMCommon.dmList(sLayers)
			Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
			'Dim colResEntities As System.Collections.ObjectModel.Collection(Of Entity) = New ObjectModel.Collection(Of Entity)()

			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord

					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)

					If oLinkCondition.IsPolyline(oEntity) AndAlso tList.Contains(oEntity.Layer) Then
						oPolyline = DirectCast(oEntity, Polyline)
						If bAllpolylines OrElse oPolyline.NumberOfVertices() > 2 OrElse oPolyline.GetSegmentType(0) = SegmentType.Line Then

							oaResPoints.Add(oPolyline.StartPoint)
							If bExplode Then
								oEntity.Explode(colDBObjects)


								oEntity.Erase()
							End If
						End If

					End If
				Next

				For Each oResEntity In colDBObjects

					tResObjID = AcadTransaction.AppendEntity(oResEntity)


					'	AcadTransaction.AppendEntity(oResEntity)

				Next
			Catch oEx As Exception

				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sLayers, "Err #1439")
			End Try
			Return oaResPoints
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function ExplodePLinesOld(ByVal sLayers As String, ByVal bExplode As Boolean, bAllpolylines As Boolean) As TplnPointArray
		'	System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN(sLayers, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(sLineLayers, "Line Nothing"), "01_270_AA")
		'	DMCommon.Debug.MsgBox("01_270_AA", sLayers)
		If moModelSpaceTableRecord IsNot Nothing Then

			Dim oLinkCondition As LinkCondition = New LinkCondition(sLayers)
			Dim oaResPoints As TplnPointArray = New TplnPointArray()
			Dim oEntity As Entity
			Dim oResEntity As Entity
			'Dim oCurve As Curve
			Dim oPolyline As Polyline
			Dim tResObjID As ObjectId
			Dim tList As DMCommon.dmList = New DMCommon.dmList(sLayers)
			Dim colDBObjects As DBObjectCollection = New DBObjectCollection()
			Dim colResEntities As System.Collections.ObjectModel.Collection(Of Entity) = New ObjectModel.Collection(Of Entity)()

			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					colDBObjects.Clear()
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)

					If oLinkCondition.IsPolyline(oEntity) AndAlso tList.Contains(oEntity.Layer) Then
						oPolyline = DirectCast(oEntity, Polyline)
						If bAllpolylines OrElse oPolyline.NumberOfVertices() > 2 OrElse oPolyline.GetSegmentType(0) = SegmentType.Line Then

							oaResPoints.Add(oPolyline.StartPoint)
							If bExplode Then
								oEntity.Explode(colDBObjects)
								For Each oDBObject As DBObject In colDBObjects
									oResEntity = DirectCast(oDBObject, Entity)
									colResEntities.Add(oResEntity)
									'AcadTransaction.AppendEntity(oResEntity)
								Next

								oEntity.Erase()
							End If
						End If

					End If
				Next

				For Each oResEntity In colResEntities

					tResObjID = AcadTransaction.AppendEntity(oResEntity)


					'	AcadTransaction.AppendEntity(oResEntity)

				Next
			Catch oEx As Exception

				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Err #1439")
			End Try
			Return oaResPoints
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function ExplodePLinesNEW(ByVal sLayers As String, ByVal bExplode As Boolean) As TplnPointArray
		'	System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN(sLayers, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(sLineLayers, "Line Nothing"), "01_270_AA")
		If moModelSpaceTableRecord IsNot Nothing Then

			Dim oLinkCondition As LinkCondition = New LinkCondition(sLayers)
			Dim oaResPoints As TplnPointArray = New TplnPointArray()
			Dim oEntity As Entity
			Dim oResEntity As Entity
			Dim oCurve As Curve
			Dim colDBObjects As DBObjectCollection = New DBObjectCollection()

			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oEntity = DirectCast(moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite), Entity)

					If oLinkCondition.IsPolyline(oEntity) Then
						oCurve = DirectCast(oEntity, Curve)
						oEntity.Explode(colDBObjects)
						If colDBObjects.Count > 1 OrElse colDBObjects.Item(0).GetRXClass().Name <> AcadConst.AcadArcName Then


							oaResPoints.Add(oCurve.StartPoint)

							If bExplode Then

								For Each oDBObject As DBObject In colDBObjects
									oResEntity = DirectCast(oDBObject, Entity)
									AcadTransaction.AppendEntity(oResEntity)
								Next
								oEntity.Erase()
							End If

						End If
					End If
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "e1439")
			End Try
			Return oaResPoints
		Else
			Return Nothing
		End If
	End Function
	Public Shared Sub RoundAll(sLinkLayers As String, sBlockNames As String, iDigits As Integer)
		'	DMCommon.Debug.MsgBox("12_997", sLinkLayers, sBlockNames, DMAcadExt.AcadDocument.IsLocked, DMAcadExt.AcadTransaction.TransactionExists, DMAcadExt.AcadTransaction.ModelSpaceObjID)
		'  Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(New String() {"UD_PCLP001", "UD_PCLP004", "UD_PCLP0011", "UD_PCLP0013"})
		' Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(New String() {"1602", "pCellK", "PCLP001", "PCLP002", "PCLP003", "PCLP004", "PCLP005", "PCLP006", "PCLP011", "PCLP013", "UD_PCLP001", "UD_PCLP002", "UD_PCLP003", "UD_PCLP004", "UD_PCLP005", "UD_PCLP006", "UD_PCLP011", "UD_PCLP013"})
		Dim tLinkLayer As DMCommon.dmList = New DMCommon.dmList(sLinkLayers) ''''''''New DMCommon.dmList(New String() {"C1650", "C1660", "C1662", "pclp013"})

		'  Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(New String() {"SRVS001", "SRVS002", "SRVS003", "SRVS006", "SRVS007"})
		Dim tBlockNames As DMCommon.dmList = New DMCommon.dmList(sBlockNames)   'New DMCommon.dmList(New String() {"C1610", "C1611"})
		If False Then
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
		End If


		DMAcadExt.AcadTransaction.CleanupRoundInserts(tBlockNames, iDigits)
		If tLinkLayer.Values.GetUpperBound(0) >= 0 Then
			DMAcadExt.AcadTransaction.CleanupRoundLinks(tLinkLayer, iDigits)
		End If
		If False Then
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If


	End Sub
	Public Shared Function GetArcs(ByVal sLayers As String, iMode As Autodesk.AutoCAD.DatabaseServices.OpenMode) As ICollection(Of Arc)

		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colArcs As ICollection(Of Arc) = New System.Collections.ObjectModel.Collection(Of Arc)
			Dim oDBObject As DBObject
			Dim oArc As Arc
			Dim tLayersList As DMCommon.dmList = New DMCommon.dmList(sLayers)
			Dim tTestObjID As ObjectId
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					tTestObjID = tAcObjId
					oDBObject = moTransaction.GetObject(tAcObjId, iMode)
					Select Case oDBObject.GetRXClass().Name
						Case AcadConst.AcadArcName
							oArc = DirectCast(oDBObject, Arc)
							If tLayersList.Contains(oArc.Layer) Then
								colArcs.Add(oArc)
							End If
					End Select

				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tTestObjID.ToString(), "e1434")
			End Try
			Return colArcs
		Else
			Return Nothing
		End If
	End Function

	Public Shared Function GetSegments(Optional ByVal sLayers As String = msEmpty) As ICollection(Of TplnSegment)
		'	System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN(sLayers, "Nothing") & vbCrLf & DMCommon.Functions.CStrN(sLineLayers, "Line Nothing"), "01_270_AA")
		If moModelSpaceTableRecord IsNot Nothing Then
			Dim colSegments As ICollection(Of TplnSegment) = New System.Collections.ObjectModel.Collection(Of TplnSegment)
			Dim oDBObject As DBObject
			Dim oSegment As TplnSegment
			Dim tTestObjID As ObjectId
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					tTestObjID = tAcObjId
					oDBObject = moTransaction.GetObject(tAcObjId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
					oSegment = New TplnSegment(oDBObject)
					If oSegment.IsNotEmpty Then
						colSegments.Add(oSegment)
					End If
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tTestObjID.ToString(), "e1434")
			End Try
			Return colSegments
		Else
			Return Nothing
		End If
	End Function

	Private Shared Function zzPolylineHasNotArc(ByVal oPolyline2d As Polyline2d) As Boolean
		Dim oColEnum As Collections.IEnumerator = oPolyline2d.GetEnumerator()
		Dim tObjID As ObjectId
		Dim oDBObj As DBObject
		Dim oVertex2d As Vertex2d = Nothing
		Do While oColEnum.MoveNext()
			tObjID = DirectCast(oColEnum.Current, ObjectId)
			oDBObj = AcadTransaction.GetDBObject(tObjID, OpenMode.ForWrite)
			oVertex2d = DirectCast(oDBObj, Vertex2d)

			If oVertex2d.Bulge <> 0.0 Then 'Prior Bulge
				Return False
			End If
		Loop
		Return True
	End Function

	Private Shared Function zzPolylineHasNotArc(ByVal oPolyline As Polyline) As Boolean
		Dim iSegmentIndex As Integer = 0
		Dim iSegmentType As SegmentType
		Do
			Try
				iSegmentType = oPolyline.GetSegmentType(iSegmentIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iSegmentIndex) & ": " & CStr(oPolyline.NumberOfVertices) & "!" & CStr(oPolyline.GetBulgeAt(iSegmentIndex - 1)), "21_129")
				Exit Do
			End Try
			If iSegmentType = SegmentType.Arc Then
				Return False
			End If
			iSegmentIndex += 1
		Loop Until (iSegmentType = SegmentType.Point) OrElse (iSegmentIndex = oPolyline.NumberOfVertices - 1)
		Return True
	End Function
	Private Shared Function zzArcToPoints(ByVal oArc As CircularArc2d, ByVal dTolerance As Double) As Point2d()
		Dim tCenter As Autodesk.AutoCAD.Geometry.Point2d = oArc.Center
		Dim dTolerAngle As Double = 2 * Math.Acos(1 - dTolerance / oArc.Radius)
		Dim dTotalAngle As Double = oArc.EndAngle - oArc.StartAngle
		If dTotalAngle < 0.0 Then dTotalAngle += Math.PI + Math.PI
		Dim iPart As Integer = Convert.ToInt32(Math.Ceiling(dTotalAngle / dTolerAngle))
		'Dim oPolyline As Polyline = New Polyline(iPart + 1)
		Dim dAngle As Double
		Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d
		Dim taRes(iPart - 1) As Point2d
		dTolerAngle = dTotalAngle / iPart
		For iIndex As Integer = 1 To iPart - 1
			dAngle = oArc.StartAngle + iIndex * dTolerAngle
			tPoint = New Autodesk.AutoCAD.Geometry.Point2d(tCenter.X + oArc.Radius * Math.Cos(dAngle), tCenter.Y + oArc.Radius * Math.Sin(dAngle))
			taRes(iIndex - 1) = tPoint
		Next
		Return taRes
	End Function
	Public Shared Function GetModelSpaceObjects() As ObjectIdCollection
		Dim colModelSpaceObjIDs As ObjectIdCollection = New ObjectIdCollection()
		For Each tObjID As ObjectId In moModelSpaceTableRecord
			colModelSpaceObjIDs.Add(tObjID)
		Next
		Return colModelSpaceObjIDs
	End Function
	Public Shared Sub EnumModelSpaceObjectsBuffer(ByVal dlProcedure As Procedure, ByVal iOpenMode As OpenMode)
		Dim oDBObject As DBObject
		Dim colAllObj As ObjectIdCollection = New ObjectIdCollection()
		For Each tAcObjID As ObjectId In moModelSpaceTableRecord
			colAllObj.Add(tAcObjID)
		Next
		For Each tAcObjID As ObjectId In colAllObj
			oDBObject = Nothing
			Try
				oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tAcObjID.ToString() & vbCrLf & Str(oDBObject IsNot Nothing), "#114 AcadTransaction - EnumModelSpaceObjects")
			End Try
			Try
				If oDBObject IsNot Nothing Then
					dlProcedure(oDBObject)
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tAcObjID.ToString() & vbCrLf & Str(oDBObject IsNot Nothing), "#118 AcadTransaction - EnumModelSpaceObjects")
				Exit For
			End Try

		Next
		'	System.Windows.Forms.MessageBox.Show(CStr(iTest), "01_900")
	End Sub
	Public Shared Sub EnumModelSpaceObjects(ByVal dlProcedure As Procedure, ByVal iOpenMode As OpenMode)
		Dim oDBObject As DBObject
		For Each tAcObjID As ObjectId In moModelSpaceTableRecord
			oDBObject = Nothing
			Try
				oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tAcObjID.ToString() & vbCrLf & Str(oDBObject IsNot Nothing), "#114 AcadTransaction - EnumModelSpaceObjects")
			End Try
			Try
				If oDBObject IsNot Nothing Then
					dlProcedure(oDBObject)
				End If

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tAcObjID.ToString() & vbCrLf & Str(oDBObject IsNot Nothing), "#118 AcadTransaction - EnumModelSpaceObjects")
				Exit For
			End Try

		Next
		'	System.Windows.Forms.MessageBox.Show(CStr(iTest), "01_900")
	End Sub



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
	Private Shared Function zzGetBlockAttrib(ByVal tAcObjID As ObjectId) As AttributeCollection
		Dim colAttributes As AttributeCollection

		Dim oBlockRef As BlockReference = GetBlockRef(tAcObjID, OpenMode.ForWrite)

		If oBlockRef IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				Return colAttributes
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - zzGetBlockAttrib_1b")
				Return Nothing
			End Try
		Else
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return Nothing
		End If
	End Function
	Private Shared Function zzGetBlockAttrib(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, Optional ByVal sBlockName As String = msEmpty, Optional ByVal sLayerName As String = msEmpty, Optional ByRef bAltBlock As Boolean = False, Optional ByVal sAltBlockName As String = msEmpty, Optional ByVal sAltLayerName As String = msEmpty) As AttributeCollection
		Dim colAttributes As AttributeCollection

		Dim oBlockRef As BlockReference = GetBlockRefForRead(tAcObjID, bMustExist, bAcadPoint, sBlockName, sLayerName, bAltBlock, sAltBlockName, sAltLayerName)

		If oBlockRef IsNot Nothing Then
			'	AcadDocument.WriteMessage("IP: " & oBlockRef.Position.X & "," & oBlockRef.Position.Y)
			Try
				colAttributes = oBlockRef.AttributeCollection()
				Return colAttributes
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show("oBlockRef IsNot Nothing", "AcadTransaction - zzGetBlockAttrib_1cz")
				Return Nothing
			End Try
		ElseIf bMustExist Then

			Dim oDBObject As DBObject = GetDBObject(tAcObjID, OpenMode.ForRead)
			MessageBox.Show("oBlockRef Is Nothing" & vbCrLf & tAcObjID.ToString() & vbCrLf & oDBObject.Handle.ToString() & vbCrLf & oDBObject.GetRXClass.Name.ToString())
			'	AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return Nothing
		Else
			'  DMCommon.Debug.MsgBox("12_262", "")
			Return Nothing
		End If
	End Function
	Private Shared Function zzGetBlockAttrib(ByVal oBlockRef As BlockReference, ByVal bMustExist As Boolean, Optional ByVal sBlockName As String = msEmpty, Optional ByVal sLayerName As String = msEmpty) As AttributeCollection
		Dim colAttributes As AttributeCollection
		If oBlockRef IsNot Nothing Then
			Try
				colAttributes = oBlockRef.AttributeCollection()
				Return colAttributes
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "2AcadTransaction - zzGetBlockAttrib_1d")
				Return Nothing
			End Try
		Else
			AcadDocument.WriteMessage("AcadTransaction - zzGetBlockAttrib_2" & vbCrLf & "BlockRef Is Nothing")
			'	System.Windows.Forms.MessageBox.Show("BlockRef Is Nothing", "AcadTransaction-zzGetBlockAttrib_2")
			Return Nothing
		End If
	End Function
	Public Shared Function GetTableStyle(ByVal sTableStyleName As String, ByVal iOpenMode As OpenMode) As TableStyle
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim dicTableStyles As DBDictionary

		Try
			dicTableStyles = DirectCast(moTransaction.GetObject(oCurrentDatabase.TableStyleDictionaryId, OpenMode.ForRead, False, False), DBDictionary)
			If dicTableStyles.Contains(sTableStyleName) Then
				Dim tTableStyleID As ObjectId = DirectCast(dicTableStyles.Item(sTableStyleName), ObjectId)
				Return DirectCast(GetDBObject(tTableStyleID, iOpenMode), TableStyle)
			Else
				MessageBox.Show("TableStyle '" & sTableStyleName & "' was not found", "AcadTransaction - GetTableStyle_3")
				Return Nothing
			End If
		Catch oEx As Exception
			Return Nothing
			DMCommon.Functions.ShowEx(oEx, "GetTableStyle_4")
		End Try
	End Function

	Public Shared Function AddTableStyle(ByVal sTableStyleName As String) As ObjectId
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim dicTableStyles As DBDictionary
		Dim tNullObjID As ObjectId
		Dim tResObjID As ObjectId
		Dim oaStyles As ArrayList
		Try
			dicTableStyles = DirectCast(moTransaction.GetObject(oCurrentDatabase.TableStyleDictionaryId, OpenMode.ForWrite, False, False), DBDictionary)
			If dicTableStyles.Contains(sTableStyleName) Then
				Return DirectCast(dicTableStyles.Item(sTableStyleName), ObjectId)
			Else
				Dim oTableStyle As TableStyle = New TableStyle()
				oaStyles = oTableStyle.CellStyles()
				'		MessageBox.Show(CStr(oaStyles.Count) & ":" & oaStyles.Item(0).GetType().ToString() & "-" & oaStyles.Item(0).ToString() & "," & oaStyles.Item(1).ToString() & "," & oaStyles.Item(2).ToString(), "19_011")
				'		MessageBox.Show(CStr(oTableStyle.SetFromStyle()) & ":" & oTableStyle.TextHeight(RowType.TitleRow) & ":" & oTableStyle.TextStyle(RowType.TitleRow).ToString(), "19_018")
				'	oaStyles.Insert()




				Try
					tResObjID = dicTableStyles.SetAt(sTableStyleName, oTableStyle)
				Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & sTableStyleName, "AcadTransaction - AddTableStyle")
					Return tNullObjID
				End Try
				AppendDBObject(oTableStyle)
				dicTableStyles.UpgradeOpen()
				Return tResObjID
			End If
		Catch oEx As Exception
			Return ObjectId.Null
			DMCommon.Functions.ShowEx(oEx, "GetTableStyle_1")
		End Try
	End Function

	Public Shared Function GetNamedDictionary(ByVal iOpenMode As OpenMode) As DBDictionary
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim tNamedDicObjID As ObjectId = oCurrentDatabase.NamedObjectsDictionaryId
		'	DMCommon.Debug.MsgBox("081120_2", "71", moTransaction Is Nothing, iOpenMode, tNamedDicObjID)
		Try
			Dim oDBObject As DBObject = moTransaction.GetObject(tNamedDicObjID, iOpenMode)
			Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary = DirectCast(oDBObject, DBDictionary)

			If dicNamed Is Nothing Then

				Return Nothing
			Else
				Return DirectCast(dicNamed, DBDictionary)
			End If
		Catch oEx As Exception
			Return Nothing
		End Try

	End Function
	Public Shared Sub ListRXClasses()
		Dim oDBObject As DBObject
		Dim dicRXClass As IDictionary(Of String, Integer)
		Dim sRXClassName As String
		Dim iClassCount As Integer
		If moModelSpaceTableRecord IsNot Nothing Then
			dicRXClass = New Dictionary(Of String, Integer)
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					iClassCount = 0
					oDBObject = GetDBObject(tAcObjId, OpenMode.ForRead)
					sRXClassName = oDBObject.GetRXClass().Name
					If dicRXClass.ContainsKey(sRXClassName) Then
						iClassCount = dicRXClass.Item(sRXClassName)
						dicRXClass.Remove(sRXClassName)
					End If
					dicRXClass.Add(sRXClassName, iClassCount + 1)
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1472")
			End Try
			dicRXClass.GetEnumerator()
			For Each sRXClassName In dicRXClass.Keys
				AcadDocument.WriteMessage("RX: " & sRXClassName & " - " & dicRXClass.Item(sRXClassName).ToString())
			Next

		End If
	End Sub
	Public Shared Sub ListDBObjects()
		Dim oDBObject As DBObject
		'	Dim dicRXClass As IDictionary(Of String, Integer)
		Dim sRXClassName As String
		Dim iClassCount As Integer
		If moModelSpaceTableRecord IsNot Nothing Then

			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					iClassCount = 0
					oDBObject = GetDBObject(tAcObjId, OpenMode.ForRead)
					sRXClassName = oDBObject.GetRXClass().Name
					Select Case sRXClassName
						Case "AcMapBulkFeature"

							AcadDocument.WriteMessage("RX: " & sRXClassName & " - " & oDBObject.ObjectId.ToString() & "," & oDBObject.Handle.ToString() & " T:" & oDBObject.GetType().ToString())
					End Select

				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1472")
			End Try


		End If
	End Sub
	Public Shared Function GetAllDBObjectsByRxClass(ByVal sRXClassName As String) As ObjectIdCollection
		Dim oDBObject As DBObject
		Dim colDBObjectsID As ObjectIdCollection = New ObjectIdCollection()
		Dim sCurrentRXClassName As String
		If moModelSpaceTableRecord IsNot Nothing Then
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oDBObject = GetDBObject(tAcObjId, OpenMode.ForRead)
					sCurrentRXClassName = oDBObject.GetRXClass().Name
					If sCurrentRXClassName = sRXClassName Then
						colDBObjectsID.Add(tAcObjId)
					End If

				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1433")
			End Try
			Return colDBObjectsID
		Else
			Return Nothing
		End If


	End Function

	Public Shared Function GetAllDBObjects() As DBObjectCollection
		Dim oDBObject As DBObject
		Dim colDBObjects As DBObjectCollection = New DBObjectCollection
		If moModelSpaceTableRecord IsNot Nothing Then
			Try
				For Each tAcObjId As ObjectId In moModelSpaceTableRecord
					oDBObject = GetDBObject(tAcObjId, OpenMode.ForRead)
					colDBObjects.Add(oDBObject)
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "", "e1433")
			End Try
			Return colDBObjects
		Else
			Return Nothing
		End If


	End Function
	Public Shared Function IsBlockRef(ByVal tAcObjID As ObjectId) As Boolean
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead)
			Return oDBObject.GetRXClass().Name = AcadConst.AcadBlockRefName
		Catch oEx As System.Exception

			'   System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")

			Return Nothing
		End Try
	End Function
	Public Shared Function GetEntity(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As Entity

		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
			If oDBObject IsNot Nothing Then
				oEntity = TryCast(oDBObject, Entity)
				Return oEntity
			Else
				AcadDocument.WriteMessage("#131 Entity Is Nothing " & tAcObjID.ToString())
				Return Nothing
			End If

		Catch oEx As System.Exception
			AcadDocument.WriteMessage("#117 AcadTransaction - GetEntity " & tAcObjID.ToString() & vbCrLf & oEx.Message)
			'   System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")

			Return Nothing
		End Try
	End Function
	Public Shared Function GetLayer(ByVal tAcObjID As ObjectId) As String
		Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity = GetEntity(tAcObjID, OpenMode.ForRead)
		If oEntity IsNot Nothing Then
			Return oEntity.Layer
		Else
			Return Nothing
		End If

	End Function
	Public Shared Function GetBlockRefInsPoint(ByVal tAcObjID As ObjectId) As Point3d
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject = GetDBObject(tAcObjID, OpenMode.ForRead)
		If oDBObject Is Nothing Then
			System.Windows.Forms.MessageBox.Show(tAcObjID.ToString() & " was not found", "3AcadTransaction - zzGetBlockRef")
		ElseIf oDBObject.GetRXClass().Name = AcadConst.AcadBlockRefName Then
			Dim oBlockRef As BlockReference = DirectCast(oDBObject, BlockReference)
			Return oBlockRef.Position
		Else
			Return Point3d.Origin
		End If

	End Function
	Public Shared Function GetDBObject(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode) As DBObject
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Try
			If moTransaction IsNot Nothing Then

				oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode) '''''''''' BORIS 
				Return oDBObject
			Else
				System.Windows.Forms.MessageBox.Show("Transaction is nothing", "04_340x")
				Return Nothing
			End If

		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadDocument.WriteMessage("#122x AcadTransaction - GetDBObject " & tAcObjID.ToString() & "; " & oAcadEx.Message & "; " & iOpenMode.ToString())
			' System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")
			For Each oDBObj As DBObject In moTransaction.GetAllObjects
				AcadDocument.WriteMessage("#157  " & oDBObj.ObjectId.ToString() & "; " & oDBObj.Handle.ToString())
			Next
			Return Nothing
		End Try
	End Function
	Public Shared Function GetDBObject(ByVal tAcObjID As ObjectId, ByVal iOpenMode As OpenMode, ByRef oResDBObject As DBObject) As Boolean
		Try
			oResDBObject = moTransaction.GetObject(tAcObjID, iOpenMode) '''''''''' BORIS 
			Return True
		Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
			AcadDocument.WriteMessage("#123x AcadTransaction - GetDBObject " & tAcObjID.ToString() & vbCrLf & oAcadEx.Message)
			'   System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")

			Return False
		End Try
	End Function
	Public Shared Sub DBObjectInfo(ByVal tAcObjID As ObjectId, Optional sLabel As String = Nothing, Optional bMsgBox As Boolean = False)
		Const sDfltLabel As String = "DBObj: "
		'	Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		If sLabel Is Nothing Then
			sLabel = sDfltLabel
		End If
		If tAcObjID.IsNull Then
			AcadDocument.WriteMessage(sLabel & tAcObjID.ToString() & "; ObjectId Is Null")
			If bMsgBox Then
				DMCommon.Debug.MsgBox(sLabel, tAcObjID, "ObjectId Is Null")
			End If
		Else
			Try
				If moTransaction IsNot Nothing Then
					'  System.Windows.Forms.MessageBox.Show(moTransaction.GetAllObjects.Count.ToString() & ":" & iOpenMode.ToString(), "04_333")
					oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead) '''''''''' BORIS 
					Dim tHandle As Handle = oDBObject.Handle
					Dim sRXClassName As String = oDBObject.GetRXClass.Name
					Dim sCoordXY As String = String.Empty
					Dim tPoint3d As Point3d

					Select Case sRXClassName
						Case AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName
						Case AcadConst.AcadPolylineName

							Dim oDBPolyline As Polyline

							oDBPolyline = DirectCast(oDBObject, Polyline)
							tPoint3d = oDBPolyline.GetPoint3dAt(0)
						Case AcadConst.AcadBlockRefName
							Dim oBlockRef As BlockReference = DirectCast(oDBObject, BlockReference)
							tPoint3d = oBlockRef.Position

						Case AcadConst.AcadPointName
							Dim oDBPoint As DBPoint = DirectCast(oDBObject, DBPoint)
							tPoint3d = oDBPoint.Position
					End Select
					If Not tPoint3d.IsEqualTo(Point3d.Origin) Then
						sCoordXY = tPoint3d.ToString()
					End If

					AcadDocument.WriteMessage(sLabel & tAcObjID.ToString() & "; H=" & tHandle.ToString() & "; " & sRXClassName & "; " & sCoordXY)
					If bMsgBox Then
						DMAcadExt.DMApp.MsgBox(sLabel, tAcObjID, "H=" & tHandle.ToString(), sRXClassName, sCoordXY)

					End If

				Else
					System.Windows.Forms.MessageBox.Show("Transaction is nothing", "04_340s")

				End If

			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				AcadDocument.WriteMessage("#122x AcadTransaction - GetDBObject " & tAcObjID.ToString() & "; " & oAcadEx.Message & "; ")
				' System.Windows.Forms.MessageBox.Show(oEx.Message & sTest, "AcadTransaction - zzGetBlockRef")
				For Each oDBObj As DBObject In moTransaction.GetAllObjects
					AcadDocument.WriteMessage("#156  " & oDBObj.ObjectId.ToString() & "<>" & oDBObj.Handle.ToString())
				Next

			End Try
		End If




	End Sub
	Public Shared Sub GetNodeInfo(tAcObjID As ObjectId, ByRef sLayerName As String, ByRef sBlockName As String, ByRef bDbPoint As Boolean)
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oBlockRef As BlockReference
		Dim oDBPoint As DBPoint

		Try
			oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead)
			If oDBObject.GetType() Is GetType(BlockReference) Then
				oBlockRef = DirectCast(oDBObject, BlockReference)

				sBlockName = oBlockRef.Name
				sLayerName = oBlockRef.Layer
				bDbPoint = False
			ElseIf (oDBObject.GetType() Is GetType(DBPoint)) Then
				oDBPoint = DirectCast(oDBObject, DBPoint)
				sBlockName = Nothing
				sLayerName = oDBPoint.Layer
				bDbPoint = True


			End If

		Catch oEx As System.Exception
			AcadDocument.WriteMessage("#179 AcadTransaction - zzGetBlockRef " & tAcObjID.ToString() & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace)


		End Try

	End Sub

	Public Shared Function GetBlockRefForRead(ByVal tAcObjID As ObjectId, ByVal bMustExist As Boolean, ByRef bAcadPoint As Boolean, Optional ByVal sBlockName As String = msEmpty, Optional ByVal sLayerName As String = msEmpty, Optional ByRef bAltBlock As Boolean = False, Optional ByVal sAltBlockName As String = msEmpty, Optional ByVal sAltLayerName As String = msEmpty) As BlockReference
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oBlockRef As BlockReference
		Try
			oDBObject = moTransaction.GetObject(tAcObjID, OpenMode.ForRead) '''''''''' BORIS 
			If oDBObject.GetType() Is GetType(BlockReference) Then
				oBlockRef = DirectCast(oDBObject, BlockReference)
				If sBlockName.Length = 0 OrElse String.Compare(sBlockName, oBlockRef.Name, True) = 0 Then
					If sLayerName.Length = 0 OrElse DMCommon.Functions.InList(sLayerName, oBlockRef.Layer) Then
						Return oBlockRef
					End If
				End If
				If sAltBlockName.Length <> 0 AndAlso String.Compare(sAltBlockName, oBlockRef.Name, True) = 0 Then
					If sAltLayerName.Length = 0 OrElse DMCommon.Functions.InList(sAltLayerName, oBlockRef.Layer) Then

						bAltBlock = oBlockRef IsNot Nothing

						Return oBlockRef
					End If
				End If
				If bMustExist Then
					AppMessages.AddMessage(True, oBlockRef.Position.X, oBlockRef.Position.Y, "AcadTransaction - zzGetBlockRef", "Block - Centroid '" & sBlockName & "' is wrong; " & oDBObject.GetType().ToString(), True)
				End If
				Return Nothing

			ElseIf bMustExist AndAlso (oDBObject.GetType() Is GetType(DBPoint)) Then
				Dim oPoint As DBPoint = DirectCast(oDBObject, DBPoint)
				bAcadPoint = True
				AppMessages.AddMessage(True, oPoint.Position.X, oPoint.Position.Y, "AcadTransaction - zzGetBlockRef", "Block - Centroid was not found; " & oDBObject.GetType().ToString(), True)
				Return Nothing
			ElseIf bMustExist Then
				AppMessages.AddMessage(False, 0, 0, "AcadTransaction - zzGetBlockRef", "Block - Centroid was not found; " & oDBObject.GetType().ToString(), True)
				Return Nothing
			ElseIf (oDBObject.GetType() Is GetType(DBPoint)) Then
				bAcadPoint = True
				Return Nothing
			Else
				Return Nothing
			End If

		Catch oEx As System.Exception
			AcadDocument.WriteMessage("#174 AcadTransaction - zzGetBlockRef " & tAcObjID.ToString() & vbCrLf & oEx.Message & vbCrLf & oEx.StackTrace)
			Return Nothing
		End Try

	End Function

	Private Shared Sub zzOpenHandleDictionary()
		If mdicHandles Is Nothing Then
			Dim oAcadObj As DBObject
			If moModelSpaceTableRecord IsNot Nothing Then
				mdicHandles = New Dictionary(Of Handle, ObjectId)()
				For Each tObjID As ObjectId In moModelSpaceTableRecord
					oAcadObj = GetDBObject(tObjID, OpenMode.ForRead)
					mdicHandles.Add(oAcadObj.Handle, tObjID)
				Next
			Else
				System.Windows.Forms.MessageBox.Show("ModelSpaceTableRecord is Nothing", "02_780")
			End If
		End If
	End Sub
	Private Shared Function zzUpdateAttribText(colAttributes As AttributeCollection, ByVal saAttribText() As String) As Boolean
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1

		If colAttributes Is Nothing Then
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_47" & ": Block Attributes were not found ")
			Return False
		Else

			Try
				iValuesUB = Math.Min(saAttribText.GetUpperBound(0), colAttributes.Count - 1)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - UpdateAttribText_1")
				Return False
			End Try

			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 Then
				Dim oAttribRef As AttributeReference
				For iAttribIndex As Integer = 0 To iValuesUB
					Try
						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iAttribIndex)
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							If oAttribRef.TextString <> saAttribText(iAttribIndex) Then
								oAttribRef.TextString = saAttribText(iAttribIndex)
							End If
							oAttribRef.Draw()
						End If
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - UpdateAttribText_2a_" & ": '" & saAttribText(iAttribIndex) & "'")
					End Try
				Next

				Return True
			Else
				System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - UpdateAttribText_7")
				Return False
			End If

		End If
	End Function
	Private Shared Function zzUpdateAttribText(colAttributes As AttributeCollection, ByVal dicAtribValues As Generic.Dictionary(Of String, String)) As Boolean
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim tAttribObjID As ObjectId
		Dim oAttribRef As AttributeReference
		Dim sNewValue As String = Nothing

		For iIndex As Integer = 0 To colAttributes.Count - 1
			tAttribObjID = colAttributes.Item(iIndex)
			oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
			oAttribRef = DirectCast(oDBObject, AttributeReference)
			If dicAtribValues.TryGetValue(oAttribRef.Tag, sNewValue) Then
				If oAttribRef.TextString <> sNewValue Then
					oAttribRef.TextString = sNewValue
				End If
			End If

		Next
		'	AcadDocument.WriteMessage("AttribTags= " & sTestTag)
		Return True
	End Function
	Private Shared Function zzUpdateAttribText(colAttributes As AttributeCollection, ByVal iaAttribIndices() As Integer, ByVal saAttribText() As String) As Boolean

		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1
		'	Dim sTestAttribValue As String = "-|-|-"
		'	Dim sTestAttribTag As String = "tg"


		If colAttributes Is Nothing Then
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribText_41" & ": Block Attributes were not found ")
			Return False
		Else

			Try
				iValuesUB = iaAttribIndices.GetUpperBound(0)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "IsArray = " & IsArray(iaAttribIndices), "AcadTransaction - UpdateAttribText_1a")
				Return False
			End Try
			Dim tAttribObjID As ObjectId
			If iValuesUB >= 0 Then
				Dim oAttribRef As AttributeReference
				Dim iAttribIndex As Integer

				For iIndex As Integer = 0 To colAttributes.Count - 1
					tAttribObjID = colAttributes.Item(iIndex)
					oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
					oAttribRef = DirectCast(oDBObject, AttributeReference)

				Next
				'	AcadDocument.WriteMessage("AttribTags= " & sTestTag)
				For iIndex As Integer = 0 To iValuesUB
					Try
						iAttribIndex = iaAttribIndices(iIndex)
						If iAttribIndex >= 0 Then
							tAttribObjID = colAttributes.Item(iAttribIndex)
							oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
							oAttribRef = DirectCast(oDBObject, AttributeReference)
							'
							If saAttribText(iIndex) IsNot Nothing AndAlso oAttribRef.TextString <> saAttribText(iIndex) Then
								oAttribRef.TextString = saAttribText(iIndex)
							End If
							oAttribRef.Draw()
						End If
					Catch oEx As System.Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "21:UpdateAttribText-AcadTransaction " & ": '" & saAttribText(iAttribIndex) & "'" & vbCrLf & iValuesUB.ToString() & ":" & iaAttribIndices.GetUpperBound(0).ToString() & ":cnt=" & colAttributes.Count.ToString())
					End Try
				Next

				Return True
			Else
				System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - UpdateAttribText_3")
				Return False
			End If

		End If
	End Function

	Private Shared Function zzUpdateAttribVisibility(colAttributes As AttributeCollection, Optional ByVal iaAttribIndices() As Integer = Nothing, Optional ByVal baAttribInvisible() As Boolean = Nothing, Optional bAll As Boolean = False) As Boolean

		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim iValuesUB As Integer = -1



		If colAttributes Is Nothing Then
			AcadDocument.WriteMessage("AcadTransaction - UpdateAttribVisible_41" & ": Block Attributes were not found ")
			Return False
		Else
			If baAttribInvisible Is Nothing Then

				Dim oAttributeRef As AttributeReference
				Dim oEnum As System.Collections.IEnumerator = colAttributes.GetEnumerator()
				Dim tAttribObjID As ObjectId
				Do While oEnum.MoveNext
					tAttribObjID = DirectCast(oEnum.Current, ObjectId)
					oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
					oAttributeRef = DirectCast(oDBObject, AttributeReference)
					oAttributeRef.Invisible = bAll
				Loop
				Return True
			Else
				Try
					iValuesUB = colAttributes.Count - 1
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - UpdateAttribVisibility_1")
					Return False
				End Try
				Dim tAttribObjID As ObjectId
				If iValuesUB >= 0 Then
					Dim oAttribRef As AttributeReference
					Dim iAttribIndex As Integer

					'	AcadDocument.WriteMessage("AttribTags= " & sTestTag)
					For iIndex As Integer = 0 To iValuesUB
						Try
							If iaAttribIndices Is Nothing Then
								iAttribIndex = iIndex
							Else
								iAttribIndex = iaAttribIndices(iIndex)
							End If

							If iAttribIndex >= 0 Then
								tAttribObjID = colAttributes.Item(iAttribIndex)
								oDBObject = moTransaction.GetObject(tAttribObjID, OpenMode.ForWrite, False, True)
								oAttribRef = DirectCast(oDBObject, AttributeReference)
								'
								oAttribRef.Invisible = baAttribInvisible(iAttribIndex)
								oAttribRef.Draw()
							End If
						Catch oEx As System.Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadTransaction - UpdateAttribText_2a_" & ": '" & baAttribInvisible(iAttribIndex).ToString() & "'")
						End Try
					Next

					Return True
				Else
					System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "AcadTransaction - UpdateAttribText_3")
					Return False
				End If
			End If


		End If
	End Function
	Private Shared Function zzToHandle(sHandleString As String) As Handle
		Dim iHexNumberStyle As System.Globalization.NumberStyles = Globalization.NumberStyles.HexNumber
		'    Dim iTopoID As Integer
		'	Dim iNumberStyle As System.Globalization.NumberStyles = Globalization.NumberStyles.HexNumber
		Dim oCulture As System.Globalization.CultureInfo = New System.Globalization.CultureInfo("he-IL")
		Dim lHandle As Long
		Dim tHandle As Handle



		If Long.TryParse(sHandleString, iHexNumberStyle, oCulture, lHandle) Then
			'DMCommon.Debug.MsgBox("13_022s", sHandleString, lHandle)
			tHandle = New Handle(lHandle)



			Return tHandle
		Else
			Return New Handle()
		End If
	End Function

	Private Shared Function zzGetEntity(ByVal tAcObjID As ObjectId, Optional iOpenMode As OpenMode = OpenMode.ForRead) As Entity

		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject

		Try
         oDBObject = moTransaction.GetObject(tAcObjID, iOpenMode)
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

   Private Enum enLinkType
      OtherLayer
      SourceLayerLine
      SourceLayerArc
      SourceLayerAny
      LineLayer
      AdditionalLayer
   End Enum
   Private Class LinkCondition
      '	Private Const msEmpty As String = ""
      Private mtLinkLayersList As DMCommon.dmList
      Private mtLineLinkLayersList As DMCommon.dmList
      Private mtAdditionalLinkLayersList As DMCommon.dmList

      Private mbLineLayerExists As Boolean
      Private mbAdditionalLayerExists As Boolean

		Public Sub New(ByVal sLayers As String, Optional ByVal sLineLayers As String = msEmpty, Optional ByVal sAdditionalLayers As String = msEmpty)
			mtLinkLayersList = New DMCommon.dmList(sLayers)
			mbLineLayerExists = Not String.IsNullOrEmpty(sLineLayers)
			If mbLineLayerExists Then
				mtLineLinkLayersList = New DMCommon.dmList(sLineLayers)
			End If

			mbAdditionalLayerExists = Not String.IsNullOrEmpty(sAdditionalLayers)
			If mbAdditionalLayerExists Then
				mtAdditionalLinkLayersList = New DMCommon.dmList(sAdditionalLayers)
			End If

		End Sub
		Public Sub New(ByVal hsLayers As HashSet(Of String))
			mtLinkLayersList = New DMCommon.dmList(hsLayers)
			mbLineLayerExists = False


		End Sub
		Public Shared Function IsLink(sRXClassName As String) As Boolean
         Select Case sRXClassName
            Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName
               Return True
            Case Else
               Return False
         End Select
      End Function
      Public Shared Function GetPolygonStatus(oEntity As Entity) As enPolygonStatus
         Dim sRXClassName As String = oEntity.GetRXClass().Name
         Select Case sRXClassName
            Case AcadConst.AcadPolylineName, AcadConst.Acad2dPolylineName
               Dim oCurve As Curve = DirectCast(oEntity, Curve)
               If oCurve.Closed Then
                  Return enPolygonStatus.ClosedPolyline
               Else
                  Return enPolygonStatus.Polyline
               End If

            Case Else
               Return enPolygonStatus.None
         End Select
      End Function
      Public Function GetLinkType(oEntity As Entity) As enLinkType
         Dim sRXClassName As String
         Dim sEntityLayer As String

         sRXClassName = oEntity.GetRXClass().Name
         sEntityLayer = oEntity.Layer
         If mbLineLayerExists AndAlso mtLineLinkLayersList.Contains(sEntityLayer) Then
            Select Case sRXClassName
               Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName
                  Return enLinkType.LineLayer
               Case Else
                  Return enLinkType.OtherLayer
            End Select

         ElseIf mtLinkLayersList.Contains(sEntityLayer) Then
            If mbLineLayerExists Then
               Select Case sRXClassName
                  Case AcadConst.AcadPolylineName
							Dim oPolyline As Polyline = DirectCast(oEntity, Polyline)


							If dmLineCleanup.PolylineHasNotArc(oPolyline) Then
								Return enLinkType.SourceLayerLine
							Else
								Return enLinkType.SourceLayerArc
                     End If
                  Case AcadConst.AcadLineName
                     Return enLinkType.SourceLayerLine
                  Case AcadConst.Acad2dPolylineName
							Dim oPolyline2d As Polyline2d = DirectCast(oEntity, Polyline2d)

							If zzPolylineHasNotArc(oPolyline2d) Then
								Return enLinkType.SourceLayerLine
							Else
								Return enLinkType.SourceLayerArc
                     End If
                  Case AcadConst.AcadArcName
                     Return enLinkType.SourceLayerArc
                  Case Else
                     Return enLinkType.OtherLayer
               End Select
            Else  ' Not mbLineLayerExists
               Select Case sRXClassName
                  Case AcadConst.AcadPolylineName, AcadConst.AcadLineName, AcadConst.Acad2dPolylineName, AcadConst.AcadArcName
                     Return enLinkType.SourceLayerAny
                  Case Else
                     Return enLinkType.OtherLayer
               End Select
            End If
         Else ' not Contains
            Return enLinkType.OtherLayer
         End If
      End Function
      Public Function IsPolyline(oEntity As Entity) As Boolean
         Dim sRXClassName As String
			'   Dim sEntityLayer As String

			sRXClassName = oEntity.GetRXClass().Name
			'  sEntityLayer = oEntity.Layer

			Select Case sRXClassName
            Case AcadConst.AcadPolylineName, AcadConst.AcadLWPolylineName, AcadConst.Acad2dPolylineName
               Return True
            Case Else
               Return False
         End Select

      End Function
   End Class 'LinkCondition
   Private Class InsertCondition
      '	Private Const msEmpty As String = ""
      Private mtLayersList As DMCommon.dmList
      Private mtBlockNamesList As DMCommon.dmList
		Private mhsPairs As HashSet(Of String)

		'	Private mtLineLinkLayersList As DMCommon.dmList
		Private mtAdditionalLinkLayersList As DMCommon.dmList
		Private mbAcadPoint As Boolean
		Private mbBlockNameExists As Boolean
		Private mbLayerExists As Boolean

		'Private mbIsBlockRef As Boolean
		Private mbIsPairSet As Boolean


		Private mbAdditionalLayerExists As Boolean
		'	Private msBlockName As String
		Public Sub New(Optional bAcadPoint As Boolean = False, Optional ByVal sBlockNames As String = msEmpty, Optional ByVal sLayers As String = msEmpty)
			Me.zzNew(bAcadPoint, sBlockNames, sLayers)
		End Sub

		Private Sub zzNew(bAcadPoint As Boolean, ByVal sBlockNames As String, ByVal sLayers As String)
			mbAcadPoint = bAcadPoint
			mbBlockNameExists = Not String.IsNullOrEmpty(sBlockNames)
			mbLayerExists = Not String.IsNullOrEmpty(sLayers)


			If mbBlockNameExists Then
				mtBlockNamesList = New DMCommon.dmList(sBlockNames)
			End If
			If mbLayerExists Then
				mtLayersList = New DMCommon.dmList(sLayers)
			End If
			If mtBlockNamesList.UpperBound > 0 AndAlso mtBlockNamesList.UpperBound = mtLayersList.UpperBound Then
				mbIsPairSet = True
				mhsPairs = New HashSet(Of String)()
				For iIndex As Integer = 0 To mtBlockNamesList.UpperBound
					mhsPairs.Add(zzGetPair(mtBlockNamesList.Item(iIndex), mtLayersList.Item(iIndex)))

				Next

			End If
		End Sub
		Private Function zzGetPair(ByVal sValue1 As String, ByVal sValue2 As String) As String
			Const sDelim As String = ","
			Return sValue1 & sDelim & sValue2
		End Function


		Public Function GetInsertType(oEntity As Entity) As Integer
         Dim sRXClassName As String
         Dim sEntityLayer As String

         sRXClassName = oEntity.GetRXClass().Name
         sEntityLayer = oEntity.Layer

         If (mbLayerExists AndAlso mtLayersList.Contains(sEntityLayer)) OrElse (Not mbLayerExists) Then
				If mbBlockNameExists AndAlso (sRXClassName = AcadConst.AcadBlockRefName) OrElse (Not mbBlockNameExists) Then

					Dim oBlockRef As BlockReference = DirectCast(oEntity, BlockReference)

					If mbIsPairSet Then
						If mhsPairs.Contains(zzGetPair(oBlockRef.Name, sEntityLayer)) Then
							Return 0
						Else
							Return -1
						End If

					ElseIf mtBlockNamesList.Contains(oBlockRef.Name) Then
						Return 0
					Else
						Return -1
					End If

				ElseIf mbAcadPoint AndAlso (sRXClassName = AcadConst.AcadPointName) Then
					Return 0
				Else
					Return -1
            End If

         Else
            Return -1
         End If
      End Function


   End Class 'InsertCondition


End Class 'AcadTransaction



