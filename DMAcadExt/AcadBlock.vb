Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class AcadBlock
   Public Const AddDelim As String = "@"
   Public Enum enResults
      Success
      BlockRefWasNotFound
   End Enum
   Private msBlockName As String
   Private msFolder As String
   Private msaFields() As String
   Private mdicFields As Dictionary(Of String, Integer)
	'Private mbHasAttributes As Boolean
	Private miaAttribIndices() As Integer
   Private mtBlockDefObjID As ObjectId

   Private mcolBlockRefObjIds As ObjectIdCollection
   Private moaAttribDefs() As AttributeDefinition
   Private moaAttribData() As AttributeData
   Private mbHasAttributeDefinitions As Boolean
   Private mbaAttribInvisible() As Boolean
   Private moBlockObjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator
   Private moAdditionalBlock As AcadBlock
   Private moPrimaryBlock As AcadBlock

   Public Shared Function GetAdditionalBlockExt(iNumber As Integer) As String
      Return AddDelim & Chr(96 + iNumber)
   End Function
   Public Shared Function GetAdditionalBlockName(sBaseBlockName As String, iNumber As Integer) As String
      Return sBaseBlockName & GetAdditionalBlockExt(iNumber)
   End Function
   Public Shared Function GetAddBlockObjID(tAcObjID As ObjectId) As ObjectId
      Dim iRow As Integer = 0
      Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = AcadTransaction.GetXData(TplnXDataAddBlock.XDataAppName, tAcObjID)

      '    DMCommon.ExcelLog.SetNextValue(iRow, 1, tAcObjID.ToString())
      '     DMCommon.ExcelLog.SetValue(iRow, 3, oResBuffer.AsArray().GetUpperBound(0))


      Dim oXDataAddBlock As TplnXDataAddBlock = New TplnXDataAddBlock(oResBuffer)
      Dim tHandle As Handle = oXDataAddBlock.EntityHandle
      '   DMCommon.ExcelLog.SetValue(iRow, 5, tHandle.ToString())
      If tHandle.Value = 0L Then
         Return ObjectId.Null
      Else
         Return AcadTransaction.GetObjectID(tHandle)
      End If

   End Function
   Public Shared Sub SetResultBuffer(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, iFirstColumn As Integer)
      Dim oaTypedValue() As TypedValue = oResBuffer.AsArray()
      '  Dim iRow As Integer
      Dim oTypedValue As TypedValue
      For iIndex As Integer = 0 To oaTypedValue.GetUpperBound(0)
         oTypedValue = oaTypedValue(iIndex)
         '  DMCommon.ExcelLog.SetNextValue(iRow, iFirstColumn, oTypedValue.Value.ToString(), oTypedValue.Value.GetType().ToString(), oTypedValue.TypeCode.ToString())
      Next

   End Sub

   Public Sub New()

   End Sub
   Public Sub New(sName As String, Optional sFolder As String = Nothing)
      msBlockName = sName
      msFolder = sFolder

   End Sub
   Public Sub New(oBlockRec As BlockTableRecord)
      msBlockName = oBlockRec.Name
      mtBlockDefObjID = oBlockRec.ObjectId
      mbHasAttributeDefinitions = oBlockRec.HasAttributeDefinitions
      moBlockObjects = oBlockRec.GetEnumerator()

   End Sub

   Public Property Fields() As String()
      Get
         Return msaFields
      End Get
      Set(saValue As String())
         msaFields = saValue
 

         zzCalc()
			'mbHasAttributes = True
			mbHasAttributeDefinitions = True
		End Set
   End Property
   'Public ReadOnly Property AdditionalBlocks As System.Collections.ObjectModel.Collection(Of AcadBlock)
   '   Get
   '      Return mcolAdditionalBlocks
   '   End Get
   'End Property
   Public ReadOnly Property BlockObjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator
      Get
         Return moBlockObjects
      End Get
   End Property
   Public ReadOnly Property BlockDefObjID As ObjectId
      Get
         Return mtBlockDefObjID
      End Get
   End Property
   Public ReadOnly Property DefinitionExists As Boolean
      Get
         Return Not mtBlockDefObjID.IsNull
      End Get
   End Property
	Public Function IsAdditionalName(ByRef sPrimaryName As String) As Boolean
		Dim saValues() As String = Split(msBlockName, AddDelim)
		If saValues.GetUpperBound(0) = 1 Then
			sPrimaryName = saValues(0)
			Return True
		Else
			Return False
		End If
	End Function
	Private Function zzGetBlockRefFullData(tBlockRefObjID As ObjectId) As BlockRefData
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForRead)
      If oBlockRef IsNot Nothing Then
         Dim tResData As BlockRefData = New BlockRefData(mdicFields)
         Dim colAttributes As AttributeCollection
         Dim tAttribObjID As ObjectId
         Dim oAttribDBObject As DBObject
         Dim oAttribRef As AttributeReference
         Dim oAttribRefData As AttributeData
			If mbHasAttributeDefinitions Then
				Try
					colAttributes = oBlockRef.AttributeCollection()
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#115 " & tBlockRefObjID.ToString())
					Return New BlockRefData()
				End Try
				If colAttributes IsNot Nothing Then
					Dim oaAttribData(mdicFields.Count - 1) As AttributeData
					For iIndex As Integer = 0 To mdicFields.Count - 1
						tAttribObjID = colAttributes.Item(iIndex)
						oAttribDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForRead)
						If oAttribDBObject IsNot Nothing Then
							oAttribRef = DirectCast(oAttribDBObject, AttributeReference)
							oAttribRefData = New AttributeData(oAttribRef)
							oaAttribData(iIndex) = oAttribRefData
						Else
							DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#111 " & tBlockRefObjID.ToString())
						End If

					Next

					tResData.AttribData = oaAttribData
				Else
					DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#119 " & tBlockRefObjID.ToString())
				End If

			End If

			tResData.AcObjID = tBlockRefObjID
         tResData.Position = oBlockRef.Position
         tResData.Rotation = oBlockRef.Rotation
         tResData.ScaleFactors = oBlockRef.ScaleFactors
         tResData.Layer = oBlockRef.Layer
         tResData.ColorIndex = oBlockRef.ColorIndex


         Return tResData
      Else
         DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#167 " & tBlockRefObjID.ToString())
         Return New BlockRefData()
      End If


   End Function
   Public Function GetBlockRefData(tBlockRefObjID As ObjectId, Optional bFullAttribData As Boolean = False) As BlockRefData
      If bFullAttribData Then
         Return zzGetBlockRefFullData(tBlockRefObjID)
      Else
         Return zzGetBlockRefData(tBlockRefObjID)
      End If
   End Function
   Private Function zzGetBlockRefData(tBlockRefObjID As ObjectId) As BlockRefData
		Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForRead)
		'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Fline2", oBlockRef IsNot Nothing, mdicFields IsNot Nothing, miaAttribIndices IsNot Nothing, mbHasAttributeDefinitions)
		If oBlockRef IsNot Nothing Then
			Dim tResData As BlockRefData
			If mdicFields IsNot Nothing Then

			End If
			tResData = New BlockRefData(mdicFields)

			If mbHasAttributeDefinitions Then
				tResData.AttribValues = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaAttribIndices)
			End If

			tResData.AcObjID = tBlockRefObjID
			tResData.Position = oBlockRef.Position
			tResData.Rotation = oBlockRef.Rotation
			tResData.ColorIndex = oBlockRef.ColorIndex
			tResData.ScaleFactors = oBlockRef.ScaleFactors
			tResData.Layer = oBlockRef.Layer

			Return tResData
		Else
			Return New BlockRefData()
		End If


   End Function
   Public Function GetBlockRefData(iIndex As Integer, Optional bFullAttribData As Boolean = False) As BlockRefData
      Return GetBlockRefData(mcolBlockRefObjIds.Item(iIndex), bFullAttribData)

   End Function
   Public Sub CopyAll()
      Dim oBlockRefData As DMAcadExt.BlockRefData

      For Each tBlockRefObjID As ObjectId In mcolBlockRefObjIds

         oBlockRefData = zzGetBlockRefFullData(tBlockRefObjID)
         '  oTest = oBlockRefData.AttribData(0)
         '   System.Windows.Forms.MessageBox.Show(oBlockRefData.AcObjID.ToString() & vbCrLf & oTest.Tag & vbCrLf & oTest.Text & vbCrLf & oTest.AlignmentPoint.ToString(), "06_231")
         InsertRefAttrib(oBlockRefData, True, False, False)

         '  InsertRef(oBlockRefData)

         ' System.Windows.Forms.MessageBox.Show(oBlockRefData.AcObjID.ToString(), "06_232")
      Next


   End Sub
	Public Sub DeleteAll()
		If mcolBlockRefObjIds IsNot Nothing Then
			AcadTransaction.EraseDBObjects(mcolBlockRefObjIds)
		End If

	End Sub
	Public Function GetBlockRefData(oBlockRef As BlockReference) As BlockRefData
      If miaAttribIndices IsNot Nothing Then
         Dim tResData As BlockRefData = New BlockRefData(mdicFields)
         Dim saAttribText() As String = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, miaAttribIndices)
         tResData.Position = oBlockRef.Position
         tResData.Rotation = oBlockRef.Rotation

         tResData.Layer = oBlockRef.Layer
         tResData.AttribValues = saAttribText
         Return tResData
      Else
         DMCommon.Debug.MsgBox("09_554", "miaAttribIndices Is Nothing")
         Return New BlockRefData()
      End If
  
   End Function


   Public Function InsertRef(tBlockRefData As BlockRefData, Optional bAddCollection As Boolean = True) As ObjectId
      '     DMCommon.Debug.MsgBox("09_045", msaFields.GetUpperBound(0), tBlockRefData.AttribValues.GetUpperBound(0))
      Dim dicAtribValues As Generic.Dictionary(Of String, String) = New Generic.Dictionary(Of String, String)()
		Dim tBlockRefObjID As ObjectId
		If msaFields IsNot Nothing AndAlso tBlockRefData.AttribValues IsNot Nothing Then
         For iIndex As Integer = 0 To tBlockRefData.AttribValues.GetUpperBound(0)
            dicAtribValues.Add(msaFields(iIndex), tBlockRefData.AttribValues(iIndex))
         Next
      End If

		'   System.Windows.Forms.MessageBox.Show("" & vbCrLf & tBlockRefObjID.ToString(), "04_395")
		If Not mtBlockDefObjID.IsNull Then
			tBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtBlockDefObjID, tBlockRefData.Position, moaAttribDefs, dicAtribValues, tBlockRefData.ScaleFactors.X, , tBlockRefData.Layer, tBlockRefData.Rotation)

			If bAddCollection Then
				If mcolBlockRefObjIds Is Nothing Then
					mcolBlockRefObjIds = New ObjectIdCollection(New ObjectId() {tBlockRefObjID})
				Else
					mcolBlockRefObjIds.Add(tBlockRefObjID)
				End If
			End If
		Else
			DMCommon.Debug.MsgBox("Block definition was not found")
		End If

		Return tBlockRefObjID
   End Function
   Public Function InsertRefNew(tBlockRefData As BlockRefData, Optional bAddCollection As Boolean = True) As ObjectId
      Dim dicAtribValues As Generic.Dictionary(Of String, String) = tBlockRefData.AtribValuesDic
      Dim tBlockRefObjID As ObjectId


		'  System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & mtBlockDefObjID.ToString() & vbCrLf & tBlockRefData.Position.ToString() & vbCrLf & tBlockRefData.Layer, "04_350")
		'	DMAcadExt.AcadDocument.WriteDebugMessageN("#291", mtBlockDefObjID, tBlockRefData.Position)
		tBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtBlockDefObjID, tBlockRefData.Position, moaAttribDefs, dicAtribValues, tBlockRefData.ScaleFactors.X, tBlockRefData.ColorIndex, tBlockRefData.Layer, tBlockRefData.Rotation)
      '   DMCommon.Debug.MsgBox("09_561", tBlockRefObjID, msBlockName, tBlockRefData.Position, tBlockRefData.Layer)
      If bAddCollection Then
         If mcolBlockRefObjIds Is Nothing Then
            mcolBlockRefObjIds = New ObjectIdCollection(New ObjectId() {tBlockRefObjID})
         Else
            mcolBlockRefObjIds.Add(tBlockRefObjID)
         End If
      End If



      Return tBlockRefObjID
   End Function
   Public Function InsertRefNewNew(tBlockRefData As BlockRefData, Optional bAddCollection As Boolean = True) As ObjectId
      Dim dicAtribValues As Generic.Dictionary(Of String, String) = tBlockRefData.AtribValuesDic
      Dim tBlockRefObjID As ObjectId

      '  System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & mtBlockDefObjID.ToString() & vbCrLf & tBlockRefData.Position.ToString() & vbCrLf & tBlockRefData.Layer, "04_350")
      tBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtBlockDefObjID, moaAttribDefs, tBlockRefData)
      If bAddCollection Then
         If mcolBlockRefObjIds Is Nothing Then
            mcolBlockRefObjIds = New ObjectIdCollection(New ObjectId() {tBlockRefObjID})
         Else
            mcolBlockRefObjIds.Add(tBlockRefObjID)
         End If
      End If
      Return tBlockRefObjID
   End Function

   Public Function InsertRefAttrib(tBlockRefData As BlockRefData, bSetLayer As Boolean, Optional bAddCollection As Boolean = True, Optional bMsg As Boolean = False) As ObjectId
      Dim dicAttribData As Generic.Dictionary(Of String, AttributeData) = tBlockRefData.AttribDataDic
      Dim tBlockRefObjID As ObjectId
      Dim oBlockRef As BlockReference
      ' Dim tAcObjID As ObjectId
      Dim oAttributeRef As AttributeReference
      Dim tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d = New Autodesk.AutoCAD.Geometry.Point3d(tBlockRefData.Position.X, tBlockRefData.Position.Y, 0.0)

      Dim tAttribRefData As AttributeData = Nothing
      '  Return ObjectId.Null 'temp 22/03/
      ' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & tBlockRecObjId.ToString(), "04_360a")

      oBlockRef = New BlockReference(tInsertPoint, mtBlockDefObjID)
      '   System.Windows.Forms.MessageBox.Show(CStr(dicAttribData.Count), "19_233")

      oBlockRef.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale() 'tBlockRefData.ScaleFactors
      If bSetLayer Then
         oBlockRef.Layer = tBlockRefData.Layer
      End If
      'If AttributeData.DEBUG Then
      '   oBlockRef.ColorIndex = 1
      'Else
      '   oBlockRef.ColorIndex = 4
      'End If
      oBlockRef.ColorIndex = tBlockRefData.ColorIndex
      tBlockRefObjID = AcadTransaction.AppendEntity(oBlockRef, False)

      If tBlockRefData.BaseHandle.Value <> 0 Then
         Dim oXDataAddBlock As TplnXDataAddBlock = New TplnXDataAddBlock(2)
         oXDataAddBlock.Type = 2
         oXDataAddBlock.EntityHandle = tBlockRefData.BaseHandle
         oBlockRef.XData = oXDataAddBlock.GetResBuffer()
      End If

      If moaAttribDefs IsNot Nothing Then
         'System.Windows.Forms.MessageBox.Show(moaAttribDefs.GetUpperBound(0).ToString() & vbCrLf & tInsertPoint.ToString(), "06_755")
         For iIndex As Integer = 0 To moaAttribDefs.GetUpperBound(0)
            oAttributeRef = New AttributeReference()
            '  System.Windows.Forms.MessageBox.Show(moaAttribDefs(iIndex).BlockName, "06_758")
            oAttributeRef.SetAttributeFromBlock(moaAttribDefs(iIndex), oBlockRef.BlockTransform)
            '  System.Windows.Forms.MessageBox.Show(moaAttribDefs(iIndex).BlockName, "06_759+")
            If bMsg Then
               System.Windows.Forms.MessageBox.Show(iIndex.ToString() & vbCrLf & oAttributeRef.Tag, "04_183a")
               If dicAttribData Is Nothing Then
                  System.Windows.Forms.MessageBox.Show(iIndex.ToString() & vbCrLf & "??????????ERROR", "04_190z")
               Else
                  System.Windows.Forms.MessageBox.Show(iIndex.ToString() & vbCrLf & dicAttribData.Count.ToString(), "04_195A")
               End If
            End If
            If dicAttribData IsNot Nothing AndAlso dicAttribData.TryGetValue(oAttributeRef.Tag, tAttribRefData) Then
               ' System.Windows.Forms.MessageBox.Show(oAttributeRef.Tag & vbCrLf & tAttribRefData.Tag & vbCrLf & tAttribRefData.Text & vbCrLf & tAttribRefData.AlignmentPoint.ToString(), "06_278")
               Try
                  If bMsg Then
                     System.Windows.Forms.MessageBox.Show(iIndex.ToString() & vbCrLf & oAttributeRef.Tag & vbCrLf & tAttribRefData.Text & ":" & tAttribRefData.Tag, "04_199k")
                  End If

                  tAttribRefData.Update(oAttributeRef, bMsg)


               Catch oEx As Exception
                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & "|" & oAttributeRef.TextString & "|" & vbCrLf & "|" & dicAttribData.Item(oAttributeRef.Tag).Text & "|", "2_AcadTr-InsertBlockRef")
               End Try

            End If
            oBlockRef.AttributeCollection.AppendAttribute(oAttributeRef)

            AcadTransaction.AddNewlyCreatedDBObject(oAttributeRef)

         Next
      End If


      ' System.Windows.Forms.MessageBox.Show(dicAtribValues.Count.ToString() & vbCrLf & mtBlockDefObjID.ToString() & vbCrLf & tBlockRefData.Position.ToString() & vbCrLf & tBlockRefData.Layer, "04_350")
      If bAddCollection Then
         If mcolBlockRefObjIds Is Nothing Then
            mcolBlockRefObjIds = New ObjectIdCollection(New ObjectId() {tBlockRefObjID})
         Else
            mcolBlockRefObjIds.Add(tBlockRefObjID)
         End If
      End If
      Return tBlockRefObjID
   End Function
   Public Function UpdateAttribData(iIndex As Integer, dicAtribValues As Generic.Dictionary(Of String, String)) As enResults
      Return UpdateAttribData(mcolBlockRefObjIds.Item(iIndex), dicAtribValues)
   End Function
   Public Function UpdateAttribData(tBlockRefObjID As ObjectId, dicAtribValues As Generic.Dictionary(Of String, String)) As enResults
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)
      If oBlockRef Is Nothing Then
         Return enResults.BlockRefWasNotFound
      Else
         DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, dicAtribValues)
         Return enResults.Success
      End If
   End Function
   Public Function UpdateAttribData(tBlockRefObjID As ObjectId, saValues() As String, sLayer As String) As enResults
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)
      If oBlockRef Is Nothing Then
         DMCommon.Debug.MsgBox("09_559c", tBlockRefObjID, "BlockRefWasNotFound")
         Return enResults.BlockRefWasNotFound
      Else
         '  DMCommon.Functions.DispArray(miaAttribIndices, "!miaAttribIndices!")
         '  DMCommon.Functions.DispArray(saValues, "saValues!", True)
         '   DMCommon.Debug.MsgBox("09_559X", tBlockRefObjID, oBlockRef.Layer, sLayer)
         If Not String.IsNullOrEmpty(sLayer) AndAlso oBlockRef.Layer <> sLayer Then
            Try
               oBlockRef.Layer = sLayer
            Catch oEx As Exception

            End Try

         End If
         DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, miaAttribIndices, saValues)

         Return enResults.Success
      End If
   End Function
   Public Sub DeleteRef(iIndex As Integer)
      If mcolBlockRefObjIds IsNot Nothing AndAlso mcolBlockRefObjIds.Count > iIndex Then
         Dim tAcObjId As ObjectId = mcolBlockRefObjIds.Item(iIndex)
         AcadTransaction.EraseDBObject(tAcObjId)
      End If

   End Sub

   Public Function UpdateBlockRef(tBlockRefObjID As ObjectId, tBlockRefData As BlockRefData) As enResults
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)
      Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d
      If oBlockRef Is Nothing Then
         Return enResults.BlockRefWasNotFound
      Else
         tPoint3d = tBlockRefData.Position
         '  DMCommon.Debug.MsgBox("09_661", tPoint3d, Autodesk.AutoCAD.Geometry.Point3d.Origin)
         If tPoint3d <> Autodesk.AutoCAD.Geometry.Point3d.Origin Then
            '   DMCommon.Debug.MsgBox("09_662", tPoint3d, Autodesk.AutoCAD.Geometry.Point3d.Origin)
            oBlockRef.Position = tPoint3d
         End If

         '   oBlockRef.ScaleFactors = tBlockRefData.ScaleFactors
         '   DMAcadExt.AcadDocument.WriteMessage("$$$POS_C " & tPoint3d.ToString() & ", " & oBlockRef.Position.ToString() & "; " & oBlockRef.Layer & "; " & tBlockRefData.Layer)
         oBlockRef.Layer = tBlockRefData.Layer


         Return enResults.Success
      End If


   End Function
   Public Sub SetDefaultAttribVisibility(iIndex As Integer)


      SetDefaultAttribVisibility(mcolBlockRefObjIds.Item(iIndex))

   End Sub
   Public Sub SetDefaultAttribVisibility(tBlockRefObjID As ObjectId)
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)

      DMAcadExt.AcadTransaction.UpdateAttribVisibility(oBlockRef, , mbaAttribInvisible)

   End Sub
   Public Sub SetAllAttribVisibility(iIndex As Integer, bAllInvisible As Boolean)
      SetAllAttribVisibility(mcolBlockRefObjIds.Item(iIndex), bAllInvisible)


   End Sub
   Public Sub SetAllAttribVisibility(tBlockRefObjID As ObjectId, bAllInvisible As Boolean)
      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)

      DMAcadExt.AcadTransaction.UpdateAttribVisibility(oBlockRef, , , bAllInvisible)

   End Sub

   Private Sub zzLoadAttribVisibility()
      ReDim mbaAttribInvisible(moaAttribDefs.GetUpperBound(0))
      For iIndex As Integer = 0 To moaAttribDefs.GetUpperBound(0)
         mbaAttribInvisible(iIndex) = moaAttribDefs(iIndex).Invisible
      Next
   End Sub
   Private Sub zzCalc()

      mdicFields = New Dictionary(Of String, Integer)
      If msaFields IsNot Nothing Then
         For iIndex As Integer = 0 To msaFields.GetUpperBound(0)
            mdicFields.Add(msaFields(iIndex), iIndex)
         Next
      End If
     
      '    DMCommon.Debug.MsgBox("09_522", msaFields.GetUpperBound(0), mdicFields.Count)
      '   Dim hs As HashSet(Of String) = New HashSet(Of String)

   End Sub
	Public Sub LoadAllReferences()
		mcolBlockRefObjIds = DMAcadExt.AcadTransaction.GetAllBlockRefs(msBlockName)

		'DMCommon.Debug.MsgBox("111222_1", msBlockName, mcolBlockRefObjIds IsNot Nothing, DMCommon.Debug.ColCount(mcolBlockRefObjIds))

	End Sub

	Public Sub InsertAddBlockRefs()
      Dim oBlockRef As BlockReference
      Dim tBlockRefData As BlockRefData
      Dim oXDataAddBlock As TplnXDataAddBlock
      Dim tAddBlockRefObjId As ObjectId
      Dim oAddBlockRef As DBObject
      Dim bRes As Boolean = DMAcadExt.AcadTransaction.RegistrateApp(TplnXDataAddBlock.XDataAppName)
      For Each tAcObjID As ObjectId In mcolBlockRefObjIds
         oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForWrite)
         tBlockRefData = New BlockRefData(oBlockRef.Position)
         tBlockRefData.BaseHandle = oBlockRef.Handle
         tAddBlockRefObjId = moAdditionalBlock.InsertRefAttrib(tBlockRefData, False)
         oAddBlockRef = AcadTransaction.GetDBObject(tAddBlockRefObjId, OpenMode.ForRead)
         oXDataAddBlock = New TplnXDataAddBlock(2)
         oXDataAddBlock.Type = 1
         oXDataAddBlock.EntityHandle = oAddBlockRef.Handle
         oBlockRef.XData = oXDataAddBlock.GetResBuffer()
      Next
   End Sub
   Public Sub LoadAllReferencesByLayers(sLayers As String)
      mcolBlockRefObjIds = DMAcadExt.AcadTransaction.GetAllBlockRefs(msBlockName, sLayers)
      '   System.Windows.Forms.MessageBox.Show(mcolBlockRefObjIds.Count.ToString(), "06_229")
   End Sub
   Public ReadOnly Property BlockRefsCount As Integer
      Get
         If mcolBlockRefObjIds Is Nothing Then
            Return -1
         Else
            Return mcolBlockRefObjIds.Count
         End If
      End Get
   End Property
   Public ReadOnly Property HasAttributeDefinitions As Boolean
      Get
         Return mbHasAttributeDefinitions
      End Get
   End Property

   Public ReadOnly Property FieldCount As Integer
      Get
         If mdicFields Is Nothing Then
            Return -1
         Else
            Return mdicFields.Count
         End If
      End Get
   End Property
   Public ReadOnly Property BlockRefObjIds As ObjectIdCollection
      Get
         Return mcolBlockRefObjIds
      End Get
   End Property
   
   Public Function BlockRefValues() As Generic.ICollection(Of BlockRefData)
      Dim oBlockRefDataCollection As BlockRefDataCollection = New BlockRefDataCollection()
      Dim tBlockRefData As BlockRefData
      '   Dim sPointName As String = "XXXXXX"
      '   Dim iC As Integer
      If mcolBlockRefObjIds IsNot Nothing Then
         '  System.Windows.Forms.MessageBox.Show(oBlockRefDataCollection.Count.ToString() & vbCrLf & mcolBlockRefObjIds.Count.ToString(), "09_004bef")
         For Each tAcObjID As ObjectId In mcolBlockRefObjIds
            tBlockRefData = Me.GetBlockRefData(tAcObjID, True)
            oBlockRefDataCollection.Add(Me.GetBlockRefData(tAcObjID, True))
         Next
      End If
      ' System.Windows.Forms.MessageBox.Show(oBlockRefDataCollection.Count.ToString() & vbCrLf & msBlockName, "09_004AB")
      DMAcadExt.AcadDocument.WriteMessage("Block Name: " & msBlockName & "; Count: " & oBlockRefDataCollection.Count.ToString())
      Return oBlockRefDataCollection
   End Function
   Public Function GetEnumerator() As BlockRefEnumerator
      Dim oEnumerator As System.Collections.IEnumerator = mcolBlockRefObjIds.GetEnumerator()
      Dim oBlockRefEnumerator As BlockRefEnumerator = New BlockRefEnumerator()
		Return oBlockRefEnumerator
	End Function


	Public ReadOnly Property ReferenceCount As Integer
		Get
			If mcolBlockRefObjIds Is Nothing Then
				Return -1
			Else
				Return mcolBlockRefObjIds.Count
			End If

		End Get
	End Property
	Public ReadOnly Property HasReferences As Boolean
		Get
			If mcolBlockRefObjIds Is Nothing Then
				Return False
			Else
				Return (mcolBlockRefObjIds.Count > 0)
			End If

		End Get
	End Property
	Public ReadOnly Property AttributeData As AttributeData()
		Get
			Return moaAttribData
		End Get
	End Property
	Public ReadOnly Property AttributeDefs As AttributeDefinition()
		Get
			Return moaAttribDefs
		End Get
	End Property

	Public Sub Open(Optional bAllAttributes As Boolean = True)
		mtBlockDefObjID = DMAcadExt.AcadTransaction.GetAttribDef(msBlockName, moaAttribDefs, False)
		'DMCommon.Debug.MsgBox("09_655c", msBlockName, mtBlockDefObjID.IsNull)
		If Not mtBlockDefObjID.IsNull Then
			mbHasAttributeDefinitions = moaAttribDefs IsNot Nothing
			If mbHasAttributeDefinitions Then
				If bAllAttributes Then
					ReDim moaAttribData(moaAttribDefs.GetUpperBound(0))
					mdicFields = New Dictionary(Of String, Integer)
					'DMCommon.Debug.MsgBox("09_655d", mtBlockDefObjID.IsNull, moaAttribDefs.GetUpperBound(0))
					For iIndex As Integer = 0 To moaAttribDefs.GetUpperBound(0)
						moaAttribData(iIndex) = New AttributeData(moaAttribDefs(iIndex))
						mdicFields.Add(moaAttribDefs(iIndex).Tag, iIndex)
					Next
					'	DMCommon.Debug.MsgBox("09_655e", mdicFields.Count)
				End If
				If mbHasAttributeDefinitions Then
					ReDim miaAttribIndices(mdicFields.Count - 1)
				End If
				'	DMCommon.Debug.MsgBox("09_655k", msBlockName, mdicFields.Count, mbHasAttributeDefinitions, miaAttribIndices.GetUpperBound(0))
				mtBlockDefObjID = AcadTransaction.GetAttribIndices(msBlockName, False, mdicFields, miaAttribIndices)
				'  DMCommon.Functions.DispArray(miaAttribIndices, "Open Indices")


			End If
		Else
			mbHasAttributeDefinitions = False
			DMCommon.Debug.UserMsg("Tplanner", "Block '" & msBlockName & "' was not found")
		End If


	End Sub
   Public Sub OpenForRead()

		If mbHasAttributeDefinitions Then
			ReDim miaAttribIndices(msaFields.GetUpperBound(0))
		End If
		' DMCommon.Debug.MsgBox("09_544", mbHasAttributes, miaAttribIndices)

		mtBlockDefObjID = AcadTransaction.GetAttribIndices(msBlockName, False, mdicFields, miaAttribIndices)

   End Sub
   Public Sub Open(tAcObjID As ObjectId)
      Dim oBlockRef As BlockReference = AcadTransaction.GetBlockRef(tAcObjID, OpenMode.ForRead)
      If oBlockRef IsNot Nothing Then
         msBlockName = oBlockRef.Name
      End If
      Open()



   End Sub
   Public Sub OpenForRight(Optional bVisibility As Boolean = False)
		If mtBlockDefObjID.IsNull Then

			mtBlockDefObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msFolder, msBlockName, moaAttribDefs)

		End If
		'  DMCommon.Debug.MsgBox("09_277", mtBlockDefObjID)
		moaAttribDefs = DMAcadExt.AcadTransaction.GetAttribDef(mtBlockDefObjID)
      If bVisibility Then
         zzLoadAttribVisibility()
      End If
   End Sub
	Public ReadOnly Property BlockName As String
		Get
			Return msBlockName
		End Get
	End Property
	Public ReadOnly Property Folder As String
		Get
			Return msFolder
		End Get
	End Property

	Public Property AdditionalBlock As AcadBlock
      Get
         Return moAdditionalBlock
      End Get
      Set(oValue As AcadBlock)
         moAdditionalBlock = oValue
      End Set
   End Property
   Public Property PrimaryBlock As AcadBlock
      Get
         Return moPrimaryBlock
      End Get
      Set(oValue As AcadBlock)
         moPrimaryBlock = oValue
      End Set
   End Property

   Public Sub OpenAdditionalBlock(sName As String, Optional sFolder As String = Nothing)
      moAdditionalBlock = New AcadBlock(sName, sFolder)
      moAdditionalBlock.OpenForRight()
   End Sub
End Class
Public Structure AttributeData
   'Public Shared DEBUG As Boolean
   Public AcObjID As ObjectId
   Public Tag As String
   Public Text As String
   Public Position As Autodesk.AutoCAD.Geometry.Point3d
   Public AlignmentPoint As Autodesk.AutoCAD.Geometry.Point3d


   Public Height As Double
   Public Rotation As Double
   Public WidthFactor As Double
   Public Oblique As Double
   Public HorizontalMode As Autodesk.AutoCAD.DatabaseServices.TextHorizontalMode
   Public VerticalMode As Autodesk.AutoCAD.DatabaseServices.TextVerticalMode
   Public Invisible As Boolean
   Public Layer As String
   Public Prompt As String


   Public Sub New(oAttribRef As AttributeReference)
      AcObjID = oAttribRef.ObjectId
      Tag = oAttribRef.Tag
      Text = oAttribRef.TextString
      '   DMAcadExt.AcadDocument.WriteMessage("##Text " & Tag & ", " & Text)
      Position = oAttribRef.Position
      AlignmentPoint = oAttribRef.AlignmentPoint
      Height = oAttribRef.Height
      Rotation = oAttribRef.Rotation
      WidthFactor = oAttribRef.WidthFactor
      Oblique = oAttribRef.Oblique
      HorizontalMode = oAttribRef.HorizontalMode
      VerticalMode = oAttribRef.VerticalMode
      Invisible = oAttribRef.Invisible

   End Sub
   Public Sub New(oAttribDef As AttributeDefinition)
      AcObjID = oAttribDef.ObjectId
      Tag = oAttribDef.Tag
      Text = oAttribDef.TextString
      AlignmentPoint = oAttribDef.AlignmentPoint
      Position = oAttribDef.Position
      Rotation = oAttribDef.Rotation
      Height = oAttribDef.Height
      WidthFactor = oAttribDef.WidthFactor
      Oblique = oAttribDef.Oblique
      HorizontalMode = oAttribDef.HorizontalMode
      VerticalMode = oAttribDef.VerticalMode
      Invisible = oAttribDef.Invisible
      Prompt = oAttribDef.Prompt
   End Sub
   Public Sub Update(ByRef oAttribRef As AttributeReference, bMsg As Boolean)
      Dim i As Integer
      Try

         oAttribRef.Invisible = Invisible
         i = 1
         oAttribRef.TextString = Text
         ' DMAcadExt.AcadDocument.WriteDebugMessage("##Text " & Tag & ", " & Text)
         i = 2
       
         i = 4
         oAttribRef.Height = Height
         i = 5
         oAttribRef.Rotation = Rotation
         i = 6
         oAttribRef.WidthFactor = WidthFactor
         i = 7
         oAttribRef.Oblique = Oblique
         i = 8
         oAttribRef.HorizontalMode = HorizontalMode
         i = 9
         oAttribRef.VerticalMode = VerticalMode
         i = 10


         oAttribRef.Position = Position
         i = 11
         If oAttribRef.AlignmentPoint <> AlignmentPoint AndAlso AlignmentPoint <> Autodesk.AutoCAD.Geometry.Point3d.Origin Then
            'If DEBUG Then
            oAttribRef.AlignmentPoint = AlignmentPoint
            'End If
            ''''''''''''''''''' 
            i = 12
         End If
         If oAttribRef.Tag = "PARCEL_NUM" Then
            '  DMAcadExt.AcadDocument.WriteMessage("OK " & oAttribRef.Tag & ", " & oAttribRef.Position.ToString() & ", " & oAttribRef.AlignmentPoint.ToString() & "|" & AlignmentPoint.ToString())
         End If
         i = 13
      Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
         ' System.Windows.Forms.MessageBox.Show(oAcadEx.ErrorStatus.ToString() & vbCrLf & oAcadEx.Message & vbCrLf & oAcadEx.StackTrace & vbCrLf & oAttribRef.AlignmentPoint.ToString() & vbCrLf & AlignmentPoint.ToString() & vbCrLf & Rotation.ToString(), "1_AcadBlock-Update")

         DMAcadExt.AcadDocument.WriteMessage("+++POS " & oAttribRef.Tag & ", " & oAttribRef.Position.ToString() & ", " & i.ToString() & ", " & oAttribRef.AlignmentPoint.ToString() & "|" & AlignmentPoint.ToString())
      End Try

      If bMsg Then
         System.Windows.Forms.MessageBox.Show(oAttribRef.Tag.ToString() & vbCrLf & oAttribRef.TextString.ToString() & vbCrLf & Text & vbCrLf & Position.ToString() & ":" & "", "04_103z")
         DMAcadExt.AcadDocument.WriteMessage("$$$POS " & oAttribRef.Tag & ", " & oAttribRef.Position.ToString())
      End If



   End Sub
End Structure
Public Structure BlockRefData

   Public AcObjID As ObjectId
   Public Position As Autodesk.AutoCAD.Geometry.Point3d
   Public Rotation As Double

   Public ScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d
   Public Layer As String
   Public ColorIndex As Integer
   Public BaseHandle As Handle

   Public AttribValues() As String
   Public AttribData() As AttributeData

   Private mdicFields As Dictionary(Of String, Integer)
   Private mdicAtribValues As Generic.Dictionary(Of String, String)
   Private mdicAtribData As Generic.Dictionary(Of String, AttributeData)

   Public Sub New(dicFields As Dictionary(Of String, Integer))
      mdicFields = dicFields
   End Sub
   Public Sub New(tPosition As Autodesk.AutoCAD.Geometry.Point3d)
      Position = tPosition
      Rotation = 0.0
      ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()
   End Sub
   Public Function GetAttribValue(sFieldName As String) As String
      Dim iFieldIndex As Integer = -1
      Dim tAttribData As AttributeData
      If mdicFields IsNot Nothing Then
         If mdicFields.TryGetValue(sFieldName, iFieldIndex) Then

            If AttribData IsNot Nothing Then
               tAttribData = AttribData(iFieldIndex)
               Return tAttribData.Text
            ElseIf AttribValues IsNot Nothing Then
               Return AttribValues(iFieldIndex)
            Else
               DMAcadExt.AcadDocument.WriteMessage("$$$Err#152 " & sFieldName.ToString())
               Return Nothing
            End If


         Else
            DMAcadExt.AcadDocument.WriteMessage("$$$$$ " & sFieldName.ToString() & " Cnt=" & (mdicFields.Count).ToString())
            Return Nothing
         End If
      Else
         DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#201 " & sFieldName.ToString())
         Return Nothing
      End If

   End Function
   Public Sub SetAttribValue(sFieldName As String, sValue As String)
      Dim iFieldIndex As Integer = -1
      Dim tAttribData As AttributeData
      If mdicFields IsNot Nothing Then
         If mdicFields.TryGetValue(sFieldName, iFieldIndex) Then

            If AttribData IsNot Nothing Then
               tAttribData = AttribData(iFieldIndex)
               tAttribData.Text = sValue
            ElseIf AttribValues IsNot Nothing Then
               AttribValues(iFieldIndex) = sValue
            Else
               DMAcadExt.AcadDocument.WriteMessage("$$$Err#171 " & sFieldName.ToString())

            End If


         Else
            DMAcadExt.AcadDocument.WriteMessage("$$$$$ " & sFieldName.ToString() & " Cnt=" & (mdicFields.Count).ToString())

         End If
      Else
         DMAcadExt.AcadDocument.WriteMessage("$$$ERROR#201 " & sFieldName.ToString())

      End If

   End Sub
   Public Property AtribValuesDic As Generic.Dictionary(Of String, String)
      Get
         If mdicAtribValues Is Nothing Then
            mdicAtribValues = New Generic.Dictionary(Of String, String)()

            If mdicFields IsNot Nothing AndAlso AttribValues IsNot Nothing Then
               Dim saFields(mdicFields.Count - 1) As String
               mdicFields.Keys.CopyTo(saFields, 0)
               For iIndex As Integer = 0 To AttribValues.GetUpperBound(0)
                  mdicAtribValues.Add(saFields(iIndex), AttribValues(iIndex))
               Next
            End If

         End If

         Return mdicAtribValues
      End Get
      Set(dicValue As Generic.Dictionary(Of String, String))
         mdicAtribValues = dicValue
         '  System.Windows.Forms.MessageBox.Show(mdicAtribValues.Count.ToString() & vbCrLf & "" & vbCrLf & "", "04_340c")
      End Set
   End Property
   Public Property AttribDataDic As Generic.Dictionary(Of String, AttributeData)
      Get
         If mdicAtribData Is Nothing Then
            mdicAtribData = New Generic.Dictionary(Of String, AttributeData)()
            ''''''''''''''''''''''' AttribData(0)
            '     System.Windows.Forms.MessageBox.Show(mdicFields.Count.ToString() & vbCrLf & AttribData.GetUpperBound(0).ToString() & vbCrLf & "", "04_377X")
            If mdicFields IsNot Nothing AndAlso AttribData IsNot Nothing Then
               ' System.Windows.Forms.MessageBox.Show(mdicFields.Count.ToString() & vbCrLf & AttribData.GetUpperBound(0).ToString() & vbCrLf & "", "04_377X")
               Dim saFields(mdicFields.Count - 1) As String
               mdicFields.Keys.CopyTo(saFields, 0)
               For iIndex As Integer = 0 To AttribData.GetUpperBound(0)
                  mdicAtribData.Add(saFields(iIndex), AttribData(iIndex))
               Next
            End If

         End If

         Return mdicAtribData
      End Get
      Set(dicValue As Generic.Dictionary(Of String, AttributeData))
         mdicAtribData = dicValue
         '  System.Windows.Forms.MessageBox.Show(mdicAtribValues.Count.ToString() & vbCrLf & "" & vbCrLf & "", "04_340c")
      End Set
   End Property
   Public Function GetAttribValue(iFieldIndex As Integer) As String
      Try
         Return AttribValues(iFieldIndex)
      Catch oEx As Exception
         Return Nothing
      End Try
   End Function

   Public ReadOnly Property IsNotEmpty As Boolean
      Get
         Return Not AcObjID.IsNull
      End Get
   End Property
End Structure
Public Class BlockRefDataCollection
   Implements System.Collections.Generic.ICollection(Of BlockRefData)
   Private mCollection As System.Collections.ObjectModel.Collection(Of BlockRefData)
   Public Sub New()
      mCollection = New System.Collections.ObjectModel.Collection(Of BlockRefData)
   End Sub
   Public Sub Add(oBlockRefData As BlockRefData) Implements ICollection(Of BlockRefData).Add
      mCollection.Add(oBlockRefData)
   End Sub

   Public Sub Clear() Implements ICollection(Of BlockRefData).Clear
      mCollection.Clear()
   End Sub

   Public Function Contains(oBlockRefData As BlockRefData) As Boolean Implements ICollection(Of BlockRefData).Contains
      Return mCollection.Contains(oBlockRefData)
   End Function

   Public Sub CopyTo(oaBlockRefData() As BlockRefData, iArrayIndex As Integer) Implements ICollection(Of BlockRefData).CopyTo
      mCollection.CopyTo(oaBlockRefData, iArrayIndex)
   End Sub

   Public ReadOnly Property Count As Integer Implements ICollection(Of BlockRefData).Count
      Get
         Return mCollection.Count
      End Get
   End Property

   Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of BlockRefData).IsReadOnly
      Get
         Return False
      End Get
   End Property

   Public Function Remove(oBlockRefData As BlockRefData) As Boolean Implements ICollection(Of BlockRefData).Remove
      mCollection.Remove(oBlockRefData)
   End Function
   Public Function GetEnumerator() As IEnumerator(Of BlockRefData) Implements IEnumerable(Of BlockRefData).GetEnumerator
      Return mCollection.GetEnumerator()
   End Function

   Public Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
      Return mCollection.GetEnumerator()
   End Function
End Class
Public Structure BlockRefEnumerator
   Implements System.Collections.Generic.IEnumerator(Of BlockRefData)

   Private mcolBlockRefObjIds As ObjectIdCollection
   Private moEnumer As System.Collections.IEnumerator
   Private Sub New(colBlockRefObjIds As ObjectIdCollection)
      mcolBlockRefObjIds = colBlockRefObjIds
   End Sub
	Public ReadOnly Property Current As BlockRefData Implements IEnumerator(Of BlockRefData).Current
		Get
			Dim tObjID As ObjectId = DirectCast(moEnumer.Current, ObjectId)
			Return New BlockRefData()
		End Get
	End Property

	Public ReadOnly Property CurrentObject As System.Object Implements IEnumerator.Current
      Get
         Return Nothing
      End Get
   End Property

   Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
      Return moEnumer.MoveNext()
   End Function

   Public Sub Reset() Implements IEnumerator.Reset
      moEnumer = mcolBlockRefObjIds.GetEnumerator()
   End Sub

   Public Sub Dispose() Implements IDisposable.Dispose
      mcolBlockRefObjIds.Dispose()
   End Sub
End Structure
