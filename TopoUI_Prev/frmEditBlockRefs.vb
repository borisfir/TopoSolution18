Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmEditBlockRefs


   Const msObjectIDFieldName As String = "ObjectID"
   Const msAddObjectIDFieldName As String = "AddObjectID"

   Private moTagDataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()

	Private moaGridColumns() As System.Windows.Forms.DataGridViewColumn
   Private miFirstAttribColIndex As Integer
   Private miLastAttribColIndex As Integer


   Private miFirstAttribFieldIndex As Integer
   Private miFirstAddAttribColIndex As Integer
   Private miLastAddAttribColIndex As Integer


   Private mdicBlockRefsOld As IDictionary(Of Long, Integer)
   Private mdicBlockRefs As IDictionary(Of ObjectId, Integer)
   Private mdicBlockRefsByFilter As IDictionary(Of ObjectId, Integer)
   Private msFindLayer As String
   Private msLayerSelected As String
	Private mbEventsEnabled As Boolean = False
	Private midgvMainLocationY As Integer
	Private mcolTagPrompts As System.Collections.ObjectModel.Collection(Of TagPrompt) = New System.Collections.ObjectModel.Collection(Of TagPrompt)
   Private mcolSelectionBlockRefs As ObjectIdCollection = New ObjectIdCollection()
   Private mbFilterSelected As Boolean = False

   Private moCurrentAcadBlock As DMAcadExt.AcadBlock

   Private mtGraphicsSystemMarkerPtr As IntPtr
   Private moTagGridCellStyle As System.Windows.Forms.DataGridViewCellStyle
   Private miRecordCount As Integer
   Private mbAdditionalBlockExists As Boolean
   Private moMainTable As System.Data.DataTable

   Private moMainDataView As System.Data.DataView

   Private Structure TagPrompt
      Public Tag As String
      Public Prompt As String
      Public Sub New(sTag As String, sPrompt As String)
         Tag = sTag
         Prompt = sPrompt
      End Sub
   End Structure
   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()
      Me.dgvMain.AutoGenerateColumns = False
      midgvMainLocationY = Me.dgvMain.Location.Y
      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()
   End Sub
   Private Sub zzMyInitializeComponent()
      miFirstAttribColIndex = Me.dgvMain.ColumnCount
      mdicBlockRefs = New Dictionary(Of ObjectId, Integer)()
      mdicBlockRefsByFilter = New Dictionary(Of ObjectId, Integer)()
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, True)
      Me.cmbBlockList.ValueMember = "BlockDefObjID"
      Me.cmbBlockList.DisplayMember = "BlockName"
      Dim lstBlocks As System.Collections.Generic.IList(Of BlockTableRecord) = DMAcadExt.AcadTransaction.GetBlockList()
      Dim lstBlocksNew As System.Collections.Generic.IList(Of DMAcadExt.AcadBlock) = DMAcadExt.AcadTransaction.GetBlockListNew()

      Dim lstLayers As System.Collections.Generic.IList(Of String) = DMAcadExt.AcadTransaction.GetAllLayers()



      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_528")
      '''''''''''''''   Me.cmbBlockList.DataSource = lstBlocks
      Me.cmbBlockList.DataSource = lstBlocksNew

      Me.ccbLayer.DataSource = lstLayers





      zzCreateMainTable()

      moTagGridCellStyle = Me.dgvMain.ColumnHeadersDefaultCellStyle.Clone

   End Sub
   Private Sub zzSetBlockSource()
      Dim oDyn As System.Object = Me.cmbBlockList.SelectedItem
      Dim bLock As Boolean

      If oDyn IsNot Nothing Then
         '	If Not DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
         bLock = True
         'End If
         'Me.dgvMain.Columns.Clear()
         '	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.ColumnCount) & ":" & CStr(miFirstAttribColIndex), "21_548")


         Dim oDBObject As DBObject
         Dim sRXClassName As String
         Dim oAttribDef As AttributeDefinition
         Dim enBlockPbjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator
         Dim enAddBlockPbjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator
         Dim tAcObjID As ObjectId

         mcolTagPrompts.Clear()
         zzRemoveAttribColumns()
         moCurrentAcadBlock = DirectCast(oDyn, DMAcadExt.AcadBlock)


         Me.chkAdditionalBlock.Enabled = moCurrentAcadBlock.AdditionalBlock IsNot Nothing OrElse moCurrentAcadBlock.PrimaryBlock IsNot Nothing
         mbAdditionalBlockExists = Me.chkAdditionalBlock.Checked

         ReDim moaGridColumns(63)
         If moCurrentAcadBlock.HasAttributeDefinitions Then
            'oBlock.Database()

            moCurrentAcadBlock.LoadAllReferences()
            enBlockPbjects = moCurrentAcadBlock.BlockObjects
            enBlockPbjects.Reset()
            Do While enBlockPbjects.MoveNext
               tAcObjID = enBlockPbjects.Current
               oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               sRXClassName = oDBObject.GetRXClass().Name
               If sRXClassName = DMAcadExt.AcadConst.AcadAttributeDefName Then
                  oAttribDef = DirectCast(oDBObject, AttributeDefinition)
                  zzAddGridColumn(oAttribDef)
                  'DMAcadExt.AcadDocument.WriteMessage(sRXClassName)

               End If
            Loop
         End If
         If mbAdditionalBlockExists Then
            enAddBlockPbjects = moCurrentAcadBlock.AdditionalBlock.BlockObjects
            enAddBlockPbjects.Reset()
            Do While enAddBlockPbjects.MoveNext
               tAcObjID = enAddBlockPbjects.Current
               oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               sRXClassName = oDBObject.GetRXClass().Name
               If sRXClassName = DMAcadExt.AcadConst.AcadAttributeDefName Then
                  oAttribDef = DirectCast(oDBObject, AttributeDefinition)
                  zzAddGridColumn(oAttribDef)
                  'DMAcadExt.AcadDocument.WriteMessage(sRXClassName)

               End If
            Loop
         End If

         '  System.Windows.Forms.MessageBox.Show(CStr(moMainTable Is Nothing), "21_090")
         zzFill()
         moMainDataView = New System.Data.DataView(moMainTable, String.Empty, msObjectIDFieldName, Data.DataViewRowState.CurrentRows)
         '   System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count) & ":" & CStr(moMainDataView.Count), "21_099")
         dgvMain.DataSource = moMainDataView
         zzSetRecCount()
         mcolSelectionBlockRefs.Clear()
         '		Me.dgvMain.Columns.AddRange(moaGridColumns)
         '	If bLock Then
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_532")
         DMAcadExt.AcadDocument.Unlock()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_533")
         'End If
      End If
   End Sub
   Private Sub zzSetBlockNew()
      Dim oDyn As System.Object = Me.cmbBlockList.SelectedItem
      Dim bLock As Boolean

      If oDyn IsNot Nothing Then
         '	If Not DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
         bLock = True
         'End If
         'Me.dgvMain.Columns.Clear()
         '	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.ColumnCount) & ":" & CStr(miFirstAttribColIndex), "21_548")

         
         Dim oDBObject As DBObject
         Dim sRXClassName As String
         Dim oAttribDef As AttributeDefinition
         Dim enBlockPbjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator
         Dim enAddBlockPbjects As Autodesk.AutoCAD.DatabaseServices.BlockTableRecordEnumerator = Nothing
         Dim tAcObjID As ObjectId

         mcolTagPrompts.Clear()
         zzRemoveAttribColumns()
         moCurrentAcadBlock = DirectCast(oDyn, DMAcadExt.AcadBlock)


         Me.chkAdditionalBlock.Enabled = moCurrentAcadBlock.AdditionalBlock IsNot Nothing OrElse moCurrentAcadBlock.PrimaryBlock IsNot Nothing
         mbAdditionalBlockExists = Me.chkAdditionalBlock.Checked

         If mbAdditionalBlockExists Then
            If moCurrentAcadBlock.PrimaryBlock IsNot Nothing Then
               enAddBlockPbjects = moCurrentAcadBlock.BlockObjects
               moCurrentAcadBlock = moCurrentAcadBlock.PrimaryBlock
            ElseIf moCurrentAcadBlock.AdditionalBlock IsNot Nothing Then
               enAddBlockPbjects = moCurrentAcadBlock.AdditionalBlock.BlockObjects
            Else
               System.Windows.Forms.MessageBox.Show("Design Error #1732", "21_099")
               DMAcadExt.AcadTransaction.CloseModelSpace()
               DMAcadExt.AcadTransaction.Terminate()
               '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_532")
               DMAcadExt.AcadDocument.Unlock()
               enAddBlockPbjects = Nothing
               Return
            End If
            enAddBlockPbjects.Reset()
         End If





         ReDim moaGridColumns(63)
         If moCurrentAcadBlock.HasAttributeDefinitions Then
            'oBlock.Database()

            moCurrentAcadBlock.LoadAllReferences()
            enBlockPbjects = moCurrentAcadBlock.BlockObjects
            enBlockPbjects.Reset()
            Do While enBlockPbjects.MoveNext
               tAcObjID = enBlockPbjects.Current
               oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               sRXClassName = oDBObject.GetRXClass().Name
               If sRXClassName = DMAcadExt.AcadConst.AcadAttributeDefName Then
                  oAttribDef = DirectCast(oDBObject, AttributeDefinition)
                  zzAddGridColumn(oAttribDef)
                  'DMAcadExt.AcadDocument.WriteMessage(sRXClassName)

               End If
            Loop
         End If
         miLastAttribColIndex = Me.dgvMain.ColumnCount - 1
         If mbAdditionalBlockExists Then
            miFirstAddAttribColIndex = Me.dgvMain.ColumnCount
            Do While enAddBlockPbjects.MoveNext
               tAcObjID = enAddBlockPbjects.Current
               oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               sRXClassName = oDBObject.GetRXClass().Name
               If sRXClassName = DMAcadExt.AcadConst.AcadAttributeDefName Then
                  oAttribDef = DirectCast(oDBObject, AttributeDefinition)
                  zzAddGridColumn(oAttribDef)
                  'DMAcadExt.AcadDocument.WriteMessage(sRXClassName)

               End If
            Loop
            miLastAddAttribColIndex = Me.dgvMain.ColumnCount - 1
         End If

         '  System.Windows.Forms.MessageBox.Show(CStr(moMainTable Is Nothing), "21_090")
         zzFill()
         moMainDataView = New System.Data.DataView(moMainTable, String.Empty, msObjectIDFieldName, Data.DataViewRowState.CurrentRows)
         '   System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count) & ":" & CStr(moMainDataView.Count), "21_099")
         dgvMain.DataSource = moMainDataView
         zzSetRecCount()
         mcolSelectionBlockRefs.Clear()
         '		Me.dgvMain.Columns.AddRange(moaGridColumns)
         '	If bLock Then
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_532")
         DMAcadExt.AcadDocument.Unlock()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_533")
         'End If
      End If
   End Sub
   Private Sub zzSetBlock241016()
      Dim oDyn As System.Object = Me.cmbBlockList.SelectedItem
      Dim bLock As Boolean

      If oDyn IsNot Nothing Then
         '	If Not DMAcadExt.AcadDocument.IsLocked Then
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
         bLock = True
         'End If
         'Me.dgvMain.Columns.Clear()
         '	System.Windows.Forms.MessageBox.Show(CStr(Me.dgvMain.ColumnCount) & ":" & CStr(miFirstAttribColIndex), "21_548")

         zzRemoveAttribColumns()
         Dim oBlock As BlockTableRecord = DirectCast(oDyn, BlockTableRecord)
         Dim oDBObject As DBObject
         Dim sRXClassName As String
         Dim oAttribDef As AttributeDefinition

         mcolTagPrompts.Clear()

         If oBlock.HasAttributeDefinitions Then
            'oBlock.Database()
            ReDim moaGridColumns(63)
            For Each tAcObjID As ObjectId In oBlock
               oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAcObjID, OpenMode.ForRead)
               sRXClassName = oDBObject.GetRXClass().Name
               If sRXClassName = DMAcadExt.AcadConst.AcadAttributeDefName Then
                  oAttribDef = DirectCast(oDBObject, AttributeDefinition)
                  zzAddGridColumn(oAttribDef)
                  'DMAcadExt.AcadDocument.WriteMessage(sRXClassName)

               End If
            Next

         End If


         '  System.Windows.Forms.MessageBox.Show(CStr(moMainTable Is Nothing), "21_090")
         zzFill()
         moMainDataView = New System.Data.DataView(moMainTable, String.Empty, msObjectIDFieldName, Data.DataViewRowState.CurrentRows)
         '   System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count) & ":" & CStr(moMainDataView.Count), "21_099")
         dgvMain.DataSource = moMainDataView
         zzSetRecCount()
         mcolSelectionBlockRefs.Clear()
         '		Me.dgvMain.Columns.AddRange(moaGridColumns)
         '	If bLock Then
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_532")
         DMAcadExt.AcadDocument.Unlock()
         '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_533")
         'End If
      End If
   End Sub
   Private Sub zzAddAttribField(sFieldName As String)
      Dim oDataColumn As System.Data.DataColumn = New System.Data.DataColumn(sFieldName, GetType(System.String))
      moMainTable.Columns.Add(oDataColumn)

   End Sub
   Private Sub zzCreateMainTable()
      Dim oDataColumn As System.Data.DataColumn
      Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId

      Dim oDataType As System.Type = tAcObjID.GetType()
      '	Dim tLines As System.Data.DataTable
      moMainTable = New System.Data.DataTable("MainTable")
      oDataColumn = New System.Data.DataColumn(msObjectIDFieldName, oDataType)
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn(msAddObjectIDFieldName, oDataType)
      moMainTable.Columns.Add(oDataColumn)


      oDataColumn = New System.Data.DataColumn("PositionX", GetType(System.Double))
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("PositionY", GetType(System.Double))
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("Scale", GetType(System.Double))
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("Selected", GetType(System.Boolean))
      oDataColumn.DefaultValue = False
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("Modified", GetType(System.Boolean))
      oDataColumn.DefaultValue = False
      moMainTable.Columns.Add(oDataColumn)

      oDataColumn = New System.Data.DataColumn("Changed", GetType(System.Boolean))
      oDataColumn.DefaultValue = False
      moMainTable.Columns.Add(oDataColumn)
      oDataColumn = New System.Data.DataColumn("Layer", GetType(System.String))
      moMainTable.Columns.Add(oDataColumn)

      '	oDataColumn = New System.Data.DataColumn(msEntityFldName, System.Type.GetType("Autodesk.AutoCAD.DatabaseServices.ObjectId"))
      miFirstAttribFieldIndex = moMainTable.Columns.Count

   End Sub
   Private Sub cmbBlockList_SelectedIndexChanged(oSender As System.Object, e As System.EventArgs) Handles cmbBlockList.SelectedIndexChanged
      If mbEventsEnabled Then
         mbEventsEnabled = False
         zzSetBlockNew()
         mbEventsEnabled = True
      End If
      'frmEditBlockRefs.vb:line 40
   End Sub
   Private Sub zzRemoveAttribColumns()
      For iColIndex As Integer = Me.dgvMain.ColumnCount - 1 To miFirstAttribColIndex Step -1
         Me.dgvMain.Columns.RemoveAt(iColIndex)

      Next
      For iColIndex As Integer = Me.moMainTable.Columns.Count - 1 To miFirstAttribFieldIndex Step -1

         moMainTable.Columns.RemoveAt(iColIndex)
      Next


   End Sub
   Private Sub zzSetColumnHeader(bTag As Boolean)
      Dim tTagPrompt As TagPrompt

      '		System.Windows.Forms.MessageBox.Show(CStr(mcolTagPrompts.Count) & ":" & CStr(Me.dgvMain.Columns.Count), "21_538")
      For iColIndex As Integer = miFirstAttribColIndex To Me.dgvMain.Columns.Count - 1
         tTagPrompt = mcolTagPrompts.Item(iColIndex - miFirstAttribColIndex)
         If bTag OrElse String.IsNullOrEmpty(tTagPrompt.Prompt) Then
            '	moaGridColumns(iColIndex).HeaderText = tTagPrompt.Tag
            Me.dgvMain.Columns.Item(iColIndex).HeaderText = tTagPrompt.Tag
         Else
            'moaGridColumns(iColIndex).HeaderText = DMCommon.Hebrew.FromAcadA(tTagPrompt.Prompt)
            Me.dgvMain.Columns.Item(iColIndex).HeaderText = DMCommon.Hebrew.FromAcadA(tTagPrompt.Prompt)
            '	DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.Spell(oAttribDef.Prompt))
         End If
         'frmEditBlockRefs.vb() : Line(109)
         '\frmEditBlockRefs.vb:line 109

      Next

   End Sub
   Private Sub zzAddGridColumn(oAttribDef As AttributeDefinition)
      Dim iColIndex As Integer = Me.dgvMain.ColumnCount

      moaGridColumns(iColIndex) = New System.Windows.Forms.DataGridViewTextBoxColumn()
      '
      'Column1A
      '
      mcolTagPrompts.Add(New TagPrompt(oAttribDef.Tag, oAttribDef.Prompt))
      '	DMAcadExt.AcadDocument.WriteMessage("TAG: " & mcolTagPrompts.Item(mcolTagPrompts.Count - 1).Tag)
      With moaGridColumns(iColIndex)
         If Me.rdbTag.Checked OrElse String.IsNullOrEmpty(oAttribDef.Prompt) Then
            .HeaderText = oAttribDef.Tag
         Else
            .HeaderText = DMCommon.Hebrew.FromAcadA(oAttribDef.Prompt)
            '	DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.Spell(oAttribDef.Prompt))
         End If
         .ReadOnly = False
         .Name = oAttribDef.Tag
         .DataPropertyName = oAttribDef.Tag
         zzAddAttribField(oAttribDef.Tag)
      End With
      Me.dgvMain.Columns.Add(moaGridColumns(iColIndex))
   End Sub
   Private Sub zzFill()
      '
      moCurrentAcadBlock.LoadAllReferences()
      Dim colBlockRefIDs As ObjectIdCollection = moCurrentAcadBlock.BlockRefObjIds
      ' System.Windows.Forms.MessageBox.Show(CStr(colBlockRefIDs.Count) & ":" & msCurrentBlockName, "04_543")
      Dim oBlockRef As BlockReference
      Dim oAddBlockRef As BlockReference = Nothing

      Dim colAttributes As AttributeCollection
      Dim colAddAttributes As AttributeCollection = Nothing


      '  Dim oGridRow As DataGridViewRow = Nothing
      Dim iRowIndex As Integer
      '   Dim lObjID As Long
      Dim iAttribRefUB As Integer = -1
      Dim iAddAttribRefUB As Integer = -1

      Dim oAttribRef As AttributeReference
      Dim iColIndex As Integer
      Dim iFieldIndex As Integer
      Dim oXDataAddBlock As DMAcadExt.TplnXDataAddBlock
      Dim bErr As Boolean
      Dim oNewRow As System.Data.DataRow
      Dim hsLayers As HashSet(Of String) = New HashSet(Of String)()
      Dim oHebrewTrans As DMCommon.HebrewTrans

      Dim tAddBlockRefObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId = Nothing






      '   cmbBlockLayers

      mbEventsEnabled = False
      mbFilterSelected = False
      mdicBlockRefs.Clear()
      Me.cmbBlockLayers.Items.Clear()
      Me.cmbBlockLayers.Items.Add("--All--")
      moMainTable.Rows.Clear()

      If mbAdditionalBlockExists Then
         DMAcadExt.AcadTransaction.OpenHandleDictionary()
      End If

      zzFillFirstRow(False)
      zzFillFirstRow(True)



      For Each tBlockRefObjID As ObjectId In colBlockRefIDs
         bErr = False
         oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForRead)
         Try
            colAttributes = oBlockRef.AttributeCollection()
            iAttribRefUB = colAttributes.Count - 1
         Catch oEx As Exception
            DMAcadExt.AcadDocument.WriteMessage("AcadTransaction - GetAttribText_97:" & oEx.Message)
            iAttribRefUB = -1
            colAttributes = Nothing
         End Try

         If mbAdditionalBlockExists Then
            oXDataAddBlock = New DMAcadExt.TplnXDataAddBlock(oBlockRef.XData)
            tAddBlockRefObjID = DMAcadExt.AcadTransaction.GetObjectID(oXDataAddBlock.EntityHandle)
            ' System.Windows.Forms.MessageBox.Show(tAddBlockRefObjID.ToString() & ":" & oXDataAddBlock.EntityHandle.ToString(), "21_150")
            oAddBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAddBlockRefObjID, OpenMode.ForRead)
            '   System.Windows.Forms.MessageBox.Show(CStr(oAddBlockRef Is Nothing), "21_155")
            Try
               colAddAttributes = oAddBlockRef.AttributeCollection()
               iAddAttribRefUB = colAddAttributes.Count - 1
            Catch oEx As Exception
               DMAcadExt.AcadDocument.WriteMessage("AcadTransaction - GetAttribText_98:" & oEx.Message)
               iAttribRefUB = -1
               colAddAttributes = Nothing
            End Try
         End If


         ' System.Windows.Forms.MessageBox.Show(CStr(moMainTable Is Nothing), "21_091")
         oNewRow = moMainTable.NewRow()

         With oNewRow
            .Item("Layer") = oBlockRef.Layer
            If Not hsLayers.Contains(oBlockRef.Layer) Then
               hsLayers.Add(oBlockRef.Layer)
               Me.cmbBlockLayers.Items.Add(oBlockRef.Layer)
            End If

            '    lObjID = oBlockRef.ObjectId.OldIdPtr.ToInt64()

            .Item(msObjectIDFieldName) = oBlockRef.ObjectId
            If mbAdditionalBlockExists Then
               .Item(msAddObjectIDFieldName) = oAddBlockRef.ObjectId
            End If
            .Item("PositionX") = oBlockRef.Position.X
            .Item("PositionY") = oBlockRef.Position.Y
            .Item("Scale") = oBlockRef.ScaleFactors.X

            For iAttribIndex As Integer = 0 To iAttribRefUB
               oAttribRef = DMAcadExt.AcadTransaction.GetAttribRef(colAttributes.Item(iAttribIndex), OpenMode.ForRead)
               iColIndex = miFirstAttribColIndex + iAttribIndex
               iFieldIndex = miFirstAttribFieldIndex + iAttribIndex
               If oAttribRef IsNot Nothing AndAlso moaGridColumns(iColIndex) IsNot Nothing Then

                  If moaGridColumns(iColIndex).Name = oAttribRef.Tag Then
                     .Item(iFieldIndex) = oAttribRef.TextString
                     If False Then
                        oHebrewTrans = New DMCommon.HebrewTrans(oAttribRef.TextString, False)
                        .Item(iFieldIndex) = oHebrewTrans.GetWinDest(False)
                        'oAttribRef.TextString
                        '   DMCommon.Hebrew.FromAcadA(oAttribRef.TextString)

                        DMAcadExt.AcadDocument.WriteMessage(oAttribRef.TextString, False)
                        DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.ToASCCode(oAttribRef.TextString, False))
                        DMAcadExt.AcadDocument.WriteMessage(DMCommon.Hebrew.ToASCCode(oAttribRef.TextString, True))

                     End If



                  Else
                     .Item(iFieldIndex) = "***" & oAttribRef.Tag
                     '  oGridRow.Cells.Item(iColIndex).Style = moErrGridCellStyle
                     '   oGridRow.Cells.Item("cchChanged").Value = System.Windows.Forms.CheckState.Indeterminate
                     bErr = True
                  End If
               Else
                  bErr = True
                  'oGridRow.Cells.Item(iColIndex).Value = "*******"
               End If
            Next
            For iAttribIndex As Integer = 0 To iAddAttribRefUB
               oAttribRef = DMAcadExt.AcadTransaction.GetAttribRef(colAddAttributes.Item(iAttribIndex), OpenMode.ForRead)
               iColIndex = miFirstAttribColIndex + iAttribRefUB + 1 + iAttribIndex
               iFieldIndex = miFirstAttribFieldIndex + iAttribRefUB + 1 + iAttribIndex
               If oAttribRef IsNot Nothing AndAlso moaGridColumns(iColIndex) IsNot Nothing Then
                  If moaGridColumns(iColIndex).Name = oAttribRef.Tag Then
                     .Item(iFieldIndex) = oAttribRef.TextString
                  Else
                     .Item(iFieldIndex) = "***" & oAttribRef.Tag
                     '  oGridRow.Cells.Item(iColIndex).Style = moErrGridCellStyle
                     '   oGridRow.Cells.Item("cchChanged").Value = System.Windows.Forms.CheckState.Indeterminate
                     bErr = True
                  End If
               End If

            Next
         End With

         Try
            cmbBlockLayers.SelectedIndex = 0
         Catch oEx As Exception
            MessageBox.Show(oEx.Message & vbCrLf & "Events Enabled " & mbEventsEnabled.ToString(), "04_478")
         End Try

         msLayerSelected = String.Empty
         moMainTable.Rows.Add(oNewRow)
         '   lObjID = oBlockRef.ObjectId.OldIdPtr.ToInt64()
         iRowIndex += 1
         mdicBlockRefs.Add(oBlockRef.ObjectId, iRowIndex)
         '	oGridRow = Me.dgvMain.Rows.Item(iRowIndex)

         '	oGridRow.Cells.Item("cchChanged").Value = System.Windows.Forms.CheckState.Unchecked


      Next


      miRecordCount = colBlockRefIDs.Count

      mbEventsEnabled = True
      ' System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count) & vbCrLf & CStr(dgvMain.Rows.Count), "21_092")
   End Sub
   Private Sub zzFillFirstRow(bTag As Boolean)
      Dim tTagPrompt As TagPrompt

      '		System.Windows.Forms.MessageBox.Show(CStr(mcolTagPrompts.Count) & ":" & CStr(Me.dgvMain.Columns.Count), "21_538")





      If Me.chkFirstRow_Tag.Checked Then
         '  System.Windows.Forms.MessageBox.Show(CStr(mcolTagPrompts.Count), "21_097b")
         Dim oNewRow As System.Data.DataRow
         '  Dim iColIndex As Integer
         Dim iFieldIndex As Integer
         oNewRow = moMainTable.NewRow()
         oNewRow.Item("Layer") = String.Empty

         With oNewRow
            For iAttribIndex As Integer = 0 To mcolTagPrompts.Count - 1

               tTagPrompt = mcolTagPrompts.Item(iAttribIndex)

               '   iColIndex = miFirstAttribColIndex + iAttribIndex
               iFieldIndex = miFirstAttribFieldIndex + iAttribIndex
               If bTag Then
                  .Item(iFieldIndex) = tTagPrompt.Tag
               Else
                  .Item(iFieldIndex) = tTagPrompt.Prompt
               End If

               '  System.Windows.Forms.MessageBox.Show(CStr(iAttribIndex) & ":" & CStr(iFieldIndex) & vbCrLf & tTagPrompt.Tag, "21_100")
            Next

         End With
         moMainTable.Rows.Add(oNewRow)
         '   System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count), "21_098")
         Dim oGridRow As DataGridViewRow

         oGridRow = dgvMain.Rows.Item(dgvMain.Rows.Count - 1)
         oGridRow.DefaultCellStyle = moTagGridCellStyle
         oGridRow.Frozen = True
         oGridRow.ReadOnly = True
         Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)
         oCheckCell.Style = dgvMain.DefaultCellStyle

      End If



   End Sub
   Private Sub zzSetRecCount()
      Dim sRecText As String = CStr(miRecordCount) & " rec"
      Dim iFirstRow As Integer
      If Me.chkFirstRow_Tag.Checked Then
         iFirstRow = 1
      Else
         iFirstRow = 0
      End If
      Dim iCurrentRecCount As Integer = moMainDataView.Count - iFirstRow
      If iCurrentRecCount = 0 Then
         lblRecCount.Text = sRecText
      Else
         lblRecCount.Text = CStr(iCurrentRecCount) & " of " & sRecText
      End If

   End Sub


   Private Function zzGetBlockRefFromDrawing() As BlockReference
      Const sPromptMsg As String = "Select Block Reference ..."
      Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
      Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptEntityOptions = New Autodesk.AutoCAD.EditorInput.PromptEntityOptions(sPromptMsg)
      Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptEntityResult
      Dim oEnt As DBObject = Nothing
      '		Dim bExists As Boolean

      Dim oBlockRef As BlockReference = Nothing


      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

      Do
         oEditor.WriteMessage(sPromptMsg & vbCrLf)
         DMAcadExt.AcadDocument.CommandLine(True)
         ptRes = oEditor.GetEntity(oPromptOpt)
         If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
            Try
               oEnt = DMAcadExt.AcadTransaction.GetDBObject(ptRes.ObjectId, OpenMode.ForRead)

               If oEnt.GetRXClass().Name = DMAcadExt.AcadConst.AcadBlockRefName Then

                  oBlockRef = DirectCast(oEnt, BlockReference)
                  Exit Do



               End If
            Catch oEx As Exception

            End Try


         Else
            Exit Do
         End If
      Loop


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      Return oBlockRef
   End Function
   Private Sub zzGetBlockRefsSet()
      Try


         Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = DMAcadExt.AcadUtil.GetEditor()
         Dim oPromptOpt As Autodesk.AutoCAD.EditorInput.PromptSelectionOptions = New Autodesk.AutoCAD.EditorInput.PromptSelectionOptions()
         'Dim oPolyline As Polyline = New Polyline
         Dim ptRes As Autodesk.AutoCAD.EditorInput.PromptSelectionResult
         '	Dim colLines As ObjectIdCollection = New ObjectIdCollection
         Dim iTotal As Integer = 0
         Dim iFound As Integer = 0
         mcolSelectionBlockRefs.Clear()

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
         Dim oBlockRef As BlockReference
         Dim bAcadPoint As Boolean
         mbEventsEnabled = False
         ptRes = oEditor.GetSelection(oPromptOpt)
         ' System.Windows.Forms.MessageBox.Show(ptRes.Status.ToString(), "05_377")
         If ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.OK Then
            oSelSet = ptRes.Value()
            '	zzTestSet(oSelSet, True)
            taObjIDs = oSelSet.GetObjectIds()
            For iIndex As Integer = 0 To taObjIDs.GetUpperBound(0)
               oBlockRef = DMAcadExt.AcadTransaction.GetBlockRefForRead(taObjIDs(iIndex), False, bAcadPoint, moCurrentAcadBlock.BlockName)
               If oBlockRef IsNot Nothing Then
                  mcolSelectionBlockRefs.Add(taObjIDs(iIndex))
               End If

            Next

            iFound = mcolSelectionBlockRefs.Count - iTotal
            iTotal = mcolSelectionBlockRefs.Count
            DMAcadExt.AcadDocument.WriteSingleMessage(CStr(iFound) & " found, " & CStr(iTotal) & " total" & vbCrLf)
         ElseIf ptRes.Status = Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
            mcolSelectionBlockRefs.Clear()
            'Exit Do
         Else
            DMAcadExt.AcadDocument.WriteSingleMessage("Status: " & ptRes.Status.ToString())
            'Exit Do
         End If
         mbEventsEnabled = True
         '	Loop
         '	oEditor.SelectAll()
      Catch oEx As Exception
         mbEventsEnabled = True
      End Try
   End Sub
   Private Sub zzDispSelectRowsNew()
      If mbEventsEnabled Then
         Dim colSelectedRows As System.Windows.Forms.DataGridViewSelectedRowCollection = Me.dgvMain.SelectedRows
         Dim tAcObjID As ObjectId
         Dim bContains As Boolean
         Dim bSelected As Boolean
         Dim oGridRow As DataGridViewRow
         Dim oDataRowView As System.Data.DataRowView
         If rdbFilter.Checked Then
            bSelected = False
         Else
            bSelected = True
         End If
         For iRowIndex As Integer = 0 To moMainDataView.Count - 1
            oGridRow = dgvMain.Rows.Item(iRowIndex)
            oDataRowView = Me.moMainDataView.Item(iRowIndex)
            If mcolSelectionBlockRefs.Count = 0 Then
               '  oGridRow.Visible = True
               oGridRow.Selected = False
            Else
               tAcObjID = zzGetRowObjectIDNew(oGridRow)
               bContains = mcolSelectionBlockRefs.Contains(tAcObjID)

               If bSelected Then

                  oGridRow.Selected = bContains
               Else
                  oDataRowView.Item("Selected") = bContains
                  oGridRow.Selected = False
               End If
            End If

         Next
         If Not bSelected Then
            mbFilterSelected = True
            moMainDataView.RowFilter = zzGetRowFilter(mcolSelectionBlockRefs.Count <> 0)
         ElseIf mbFilterSelected Then
            mbFilterSelected = False
            moMainDataView.RowFilter = zzGetRowFilter(False)
         End If
         zzSetRecCount()
      End If
   End Sub
	Private Sub zzDispSelectRows()
		If mbEventsEnabled Then
			Dim colSelectedRows As System.Windows.Forms.DataGridViewSelectedRowCollection = Me.dgvMain.SelectedRows
			Dim tAcObjID As ObjectId
			Dim bContains As Boolean
			Dim bSelected As Boolean
			If rdbFilter.Checked Then
				bSelected = False
			Else
				bSelected = True
			End If
			For Each oGridRow As DataGridViewRow In Me.dgvMain.Rows
				If mcolSelectionBlockRefs.Count = 0 Then
					oGridRow.Visible = True
					oGridRow.Selected = False
				Else
               tAcObjID = zzGetRowObjectIDNew(oGridRow)
					bContains = mcolSelectionBlockRefs.Contains(tAcObjID)

					If bSelected Then
						oGridRow.Visible = True
						oGridRow.Selected = bContains
					Else
						oGridRow.Visible = bContains
						oGridRow.Selected = False
					End If
				End If

			Next
			zzSetRecCount()
		End If
	End Sub

	Private Sub zzSetRowChanged(iRowIndex As Integer)
		Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)
		zzSetRowChanged(oGridRow, True)
   End Sub
    
   Private Sub zzSetRowChangedTable(iRowIndex As Integer)
      Dim oDataRow As System.Data.DataRow = Me.moMainTable.Rows.Item(iRowIndex)
      zzSetRowChangedTable(oDataRow, True)
   End Sub
  



   Private Sub zzSetRowChangedView(iRowIndex As Integer)
      Dim oDataRowView As System.Data.DataRowView = Me.moMainDataView.Item(iRowIndex)
      '  Dim oDataRow As System.Data.DataRow = DirectCast(oDataRowView.Row, System.Data.DataRow)
      '  MessageBox.Show(oDataRow.RowState.ToString(), "04_499")
      zzSetRowChangedView(oDataRowView, True)
   End Sub

   Private Sub zzSetRowChangedNew(iRowIndex As Integer, bNewValue As Boolean)
      Dim oDataRowView As System.Data.DataRowView = Me.moMainDataView.Item(iRowIndex)

      Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iRowIndex)

      Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)

      '  Dim oDataRow As System.Data.DataRow = DirectCast(oDataRowView.Row, System.Data.DataRow)

      Dim bOldValue As Boolean = Not (IsDBNull(oDataRowView.Item("Changed")) OrElse Not DirectCast(oDataRowView.Item("Changed"), Boolean))
      Dim bModifiedValue As Boolean = DirectCast(oDataRowView.Item("Modified"), Boolean)
      ' MessageBox.Show(bModifiedValue.ToString() & vbCrLf & bOldValue.ToString() & vbCrLf & bNewValue.ToString(), "04_499")
      If bModifiedValue <> bNewValue Then
         oDataRowView.Item("Modified") = bNewValue
      End If

      If bNewValue <> bOldValue Then
         If bNewValue Then
            oCheckCell.Value = CheckState.Checked
            '   MessageBox.Show(oCheckCell.Value.ToString(), "04_499a")
         Else
            oCheckCell.Value = CheckState.Unchecked
         End If

      End If


     

   End Sub


	Private Sub cmdZoom_Click(oSender As System.Object, e As System.EventArgs) Handles cmdZoom.Click
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell

		If oCurrentCell IsNot Nothing Then
			Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
			Dim dX, dY, dScale As Double
         Dim dDWGScale As System.Double = CDbl(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable(DMAcadExt.AcadConst.ScaleSysVarName))
         Dim tAcObjID As ObjectId = zzGetRowObjectIDNew(oGridRow)
			dX = Convert.ToDouble(oGridRow.Cells.Item("ctxPositionX").Value)
			dY = Convert.ToDouble(oGridRow.Cells.Item("ctxPositionY").Value)
			dScale = Convert.ToDouble(oGridRow.Cells.Item("ctxScale").Value)

			Dim oPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(dX, dY)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadDocument.Zoom(oPoint, dDWGScale * dScale * 0.005)

			DMAcadExt.AcadTransaction.Highlight(tAcObjID)
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()

		End If





	End Sub
	 
	Private Sub cmdSetValue_Click(sender As System.Object, e As System.EventArgs) Handles cmdSetValue.Click
		Dim oSelCells As System.Windows.Forms.DataGridViewSelectedCellCollection = Me.dgvMain.SelectedCells
      Dim oGridRow As DataGridViewRow
      Dim oDataRowView As System.Data.DataRowView
		Dim oCheckCell As DataGridViewCheckBoxCell
      '  Dim iValue As System.Windows.Forms.CheckState
      Dim bValue As Boolean
      Dim bModifiedValue As Boolean
      Dim bChanged As Boolean =
      mbEventsEnabled = False
		For Each oCell As DataGridViewCell In oSelCells
         oGridRow = Me.dgvMain.Rows.Item(oCell.RowIndex)
         oDataRowView = moMainDataView.Item(oCell.RowIndex)
         bModifiedValue = DirectCast(oDataRowView.Item("Modified"), Boolean)
         oCheckCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)

         '   MessageBox.Show(oCheckCell.Value.GetType().ToString() & vbCrLf & oCheckCell.Value.ToString(), "04_465")
         If Not bModifiedValue Then
            oDataRowView.Item("Modified") = True
         End If
         bValue = DirectCast(oCheckCell.Value, Boolean)
         If Not bValue Then
            oCheckCell.Value = CheckState.Checked
         End If
         oCell.Value = Me.txtValue.Text
         
      Next
		mbEventsEnabled = True
	End Sub
  
	Private Sub cmdFindBlockRef_Click(sender As System.Object, e As System.EventArgs) Handles cmdFindBlockRef.Click
      Dim oBlockRef As BlockReference = zzGetBlockRefFromDrawing()
      Dim iRowIndex As Integer = -1
      Dim oRow As DataGridViewRow
      If oBlockRef IsNot Nothing Then

         If Me.cmbBlockList.Text <> oBlockRef.Name Then
            Me.cmbBlockList.Text = oBlockRef.Name
         Else
            iRowIndex = moMainDataView.Find(oBlockRef.ObjectId)
            If msLayerSelected = oBlockRef.Layer Then

            Else
            End If
         End If

         oRow = Me.dgvMain.Rows.Item(iRowIndex)

         Me.dgvMain.CurrentCell = oRow.Cells.Item(miFirstAttribColIndex)
         If False Then

            Dim lObjID As Long = oBlockRef.ObjectId.OldIdPtr.ToInt64()
            If mdicBlockRefs.TryGetValue(oBlockRef.ObjectId, iRowIndex) Then
               MessageBox.Show(mdicBlockRefs.Count.ToString() & ":" & CStr(iRowIndex), "05_934")
               oRow = Me.dgvMain.Rows.Item(iRowIndex)
               Dim oCell As DataGridViewCell = oRow.Cells.Item(miFirstAttribColIndex)
               Me.dgvMain.CurrentCell = oCell

            End If
         End If
      End If
      '   moMainTable.Rows.Find()
	End Sub
	Private Function zzGetRowObjectID(oGridRow As DataGridViewRow) As ObjectId
      Dim lObjID As Long
      MessageBox.Show(oGridRow.Cells.Item("ctxObjectID").Value.GetType().ToString, "05_989")
      ' lObjID = TryCast(oGridRow.Cells.Item("ctxObjectID").Value  Int64.GetType())
		Return New ObjectId(New System.IntPtr(lObjID))
	End Function
   Private Function zzGetRowObjectIDNew(oGridRow As DataGridViewRow) As ObjectId

      '  MessageBox.Show(oGridRow.Cells.Item("ctxObjectID").Value.GetType().ToString, "05_989")
      ' lObjID = TryCast(oGridRow.Cells.Item("ctxObjectID").Value  Int64.GetType())
      Return DirectCast(oGridRow.Cells.Item("ctxObjectID").Value, ObjectId)
   End Function
   Private Function zzGetRowAddObjectIDNew(oGridRow As DataGridViewRow) As ObjectId


      Return DirectCast(oGridRow.Cells.Item("ctxAddObjectID").Value, ObjectId)
   End Function
	Private Sub cmdUpdateCurrent_Click(sender As System.Object, e As System.EventArgs) Handles cmdUpdateCurrent.Click
      Dim saValues(miLastAttribColIndex - miFirstAttribColIndex) As String
      Dim saAddValues() As String = Nothing
      If mbAdditionalBlockExists AndAlso miLastAddAttribColIndex - miFirstAddAttribColIndex >= 0 Then
         ReDim saAddValues(miLastAddAttribColIndex - miFirstAddAttribColIndex)
      End If

      Dim tAcObjID As ObjectId
      Dim tAddAcObjID As ObjectId

      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
      Dim sLayer As String
      If oCurrentCell IsNot Nothing Then
         Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
         Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
         Dim oDataRow As System.Data.DataRowView = DirectCast(oGridRow.DataBoundItem, System.Data.DataRowView)



         If zzRowIsChangedNew(oGridRow) Then
            tAcObjID = zzGetRowObjectIDNew(oGridRow)
            For iIndex As Integer = miFirstAttribColIndex To miLastAttribColIndex
               saValues(iIndex - miFirstAttribColIndex) = DMCommon.Functions.CStrN(oGridRow.Cells.Item(iIndex).Value)
            Next

            If mbAdditionalBlockExists Then
               tAddAcObjID = zzGetRowAddObjectIDNew(oGridRow)
               For iIndex As Integer = miFirstAddAttribColIndex To miLastAddAttribColIndex
                  saAddValues(iIndex - miFirstAddAttribColIndex) = DMCommon.Functions.CStrN(oGridRow.Cells.Item(iIndex).Value)
               Next
            End If
            sLayer = zzGetLayer(oGridRow)
            DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
            DMAcadExt.AcadTransaction.Start()
            DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

            Dim bRes As Boolean = DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID, saValues, sLayer)
            Dim bAddRes As Boolean = True
            If mbAdditionalBlockExists Then
               bAddRes = DMAcadExt.AcadTransaction.UpdateAttribText(tAddAcObjID, saAddValues, sLayer)
            End If


            If bRes AndAlso bAddRes Then
               zzSetRowChangedNew(iCurrentRowIndex, False)
            End If
            '	MessageBox.Show(b.ToString(), "04_466")
            DMAcadExt.AcadTransaction.CloseModelSpace()
            DMAcadExt.AcadTransaction.Terminate()
            DMAcadExt.AcadDocument.Unlock()
            DMAcadExt.AcadDocument.UpdateScreen()
         End If
      End If
   End Sub
   Private Function zzRowIsChanged(oGridRow As DataGridViewRow) As System.Windows.Forms.CheckState
      ' MessageBox.Show(oGridRow.Cells.Item("cchChanged").GetType().ToString,"05_980"
      Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)

      Return DirectCast(oCheckCell.Value, System.Windows.Forms.CheckState)
   End Function
   Private Function zzRowIsChangedNew(oGridRow As DataGridViewRow) As Boolean
      'MessageBox.Show(oGridRow.Cells.Item("cchChanged").GetType().ToString,"05_980"
      Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)

      Return DirectCast(oCheckCell.Value, Boolean)
   End Function
	Private Function zzGetLayer(oGridRow As DataGridViewRow) As String
		Return DirectCast(oGridRow.Cells.Item("ccbLayer").Value, String)
   End Function
   Private Sub zzSetRowChangedTable(oDataRow As System.Data.DataRow, bNewValue As Boolean)
      Try
         If mbEventsEnabled Then
            Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oDataRow.Item("cchChanged"), DataGridViewCheckBoxCell)
            Dim iValue As System.Windows.Forms.CheckState = DirectCast(oCheckCell.Value, System.Windows.Forms.CheckState)

            Dim bOldValue As Boolean = Not IsDBNull(oDataRow.Item("cchChanged")) OrElse DirectCast(oDataRow.Item("cchChanged"), Boolean)


            If bNewValue AndAlso Not bOldValue Then
               oDataRow.Item("cchChanged") = True
            ElseIf Not bNewValue AndAlso bOldValue Then
               oDataRow.Item("cchChanged") = False
            End If
         End If
      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_454")
      End Try

   End Sub
   Private Sub zzSetRowChangedView(oDataRow As System.Data.DataRowView, bNewValue As Boolean)
      Try

         ' Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oDataRow.Item("cchChanged"), DataGridViewCheckBoxCell)
         '  Dim iValue As System.Windows.Forms.CheckState = DirectCast(oCheckCell.Value, System.Windows.Forms.CheckState)

         Dim bOldValue As Boolean = Not (IsDBNull(oDataRow.Item("Changed")) OrElse Not DirectCast(oDataRow.Item("Changed"), Boolean))


         If bNewValue AndAlso Not bOldValue Then
            oDataRow.Item("Changed") = True
            'ElseIf Not bNewValue AndAlso bOldValue Then
            ' oDataRow.Item("Changed") = False
         End If

      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_454")
      End Try

   End Sub


   Private Sub zzSetRowChanged(oGridRow As DataGridViewRow, bNewValue As Boolean)
      Try
         If mbEventsEnabled Then
            Dim oCheckCell As DataGridViewCheckBoxCell = DirectCast(oGridRow.Cells.Item("cchChanged"), DataGridViewCheckBoxCell)
            Dim iValue As System.Windows.Forms.CheckState = DirectCast(oCheckCell.Value, System.Windows.Forms.CheckState)
            If bNewValue AndAlso iValue = CheckState.Unchecked Then
               oCheckCell.Value = CheckState.Checked
            ElseIf Not bNewValue AndAlso iValue = CheckState.Checked Then
               oCheckCell.Value = CheckState.Unchecked
            End If
         End If
      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_454")
      End Try

   End Sub
	Private Sub cmdUpdateAll_Click(sender As System.Object, e As System.EventArgs) Handles cmdUpdateAll.Click
		Dim oGridRow As DataGridViewRow
      Dim saValues(miLastAttribColIndex - miFirstAttribColIndex) As String
      Dim saAddValues() As String = Nothing
      If mbAdditionalBlockExists AndAlso miLastAddAttribColIndex - miFirstAddAttribColIndex >= 0 Then
         ReDim saAddValues(miLastAddAttribColIndex - miFirstAddAttribColIndex)
      End If

      Dim tAcObjID As ObjectId
      Dim tAddAcObjID As ObjectId


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

		For iRowIndex As Integer = 0 To Me.dgvMain.RowCount - 1
			Try
				oGridRow = Me.dgvMain.Rows.Item(iRowIndex)
            If zzRowIsChangedNew(oGridRow) Then
               'lObjID = DirectCast(oGridRow.Cells.Item("ctxObjectID").Value, Int64)
               'tAcObjID = New ObjectId(New System.IntPtr(lObjID))
               tAcObjID = DirectCast(oGridRow.Cells.Item("ctxObjectID").Value, ObjectId)
              

               For iIndex As Integer = miFirstAttribColIndex To miLastAttribColIndex
                  saValues(iIndex - miFirstAttribColIndex) = DMCommon.Functions.CStrN(oGridRow.Cells.Item(iIndex).Value)
               Next

               If mbAdditionalBlockExists Then
                  tAddAcObjID = zzGetRowAddObjectIDNew(oGridRow)
                  For iIndex As Integer = miFirstAddAttribColIndex To miLastAddAttribColIndex
                     saAddValues(iIndex - miFirstAddAttribColIndex) = DMCommon.Functions.CStrN(oGridRow.Cells.Item(iIndex).Value)
                  Next
               End If



               Dim bRes As Boolean = DMAcadExt.AcadTransaction.UpdateAttribText(tAcObjID, saValues, String.Empty)
               Dim bAddRes As Boolean = True
               If mbAdditionalBlockExists Then
                  bAddRes = DMAcadExt.AcadTransaction.UpdateAttribText(tAddAcObjID, saAddValues, String.Empty)
               End If

               If bRes AndAlso bAddRes Then
                  zzSetRowChangedNew(iRowIndex, False)
               End If

            End If
         Catch oEx As Exception

         End Try
      Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub

   Private Sub dgvMain_CellBeginEdit(oSender As System.Object, e As System.Windows.Forms.DataGridViewCellCancelEventArgs) Handles dgvMain.CellBeginEdit

      If e.ColumnIndex = 4 Then
         Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(e.RowIndex)
         Dim oDataRowView As System.Data.DataRowView
         oDataRowView = moMainDataView.Item(e.RowIndex)

         Dim bModifiedValue As Boolean = DirectCast(oDataRowView.Item("Modified"), Boolean)

         Dim bOldValue As Boolean = Not (IsDBNull(oDataRowView.Item("Changed")) OrElse Not DirectCast(oDataRowView.Item("Changed"), Boolean))


         '    MessageBox.Show(oGridRow.Cells.Item("cchChanged").Value.GetType().ToString() & vbCrLf & oDataRowView.Item("Changed").GetType().ToString(), "04_458")
         If bModifiedValue = False Then
            e.Cancel = True
         End If

         'If DirectCast(oGridRow.Cells.Item("cchChanged").Value, System.Windows.Forms.CheckState) = System.Windows.Forms.CheckState.Indeterminate Then
         'e.Cancel = True
         ' End If



      End If
   End Sub

   Private Sub dgvMain_CellContentDoubleClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvMain.CellContentDoubleClick

   End Sub

   Private Sub dgvMain_CellValueChanged(oSender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvMain.CellValueChanged
      If mbEventsEnabled Then
         Dim iRowIndex As Integer = e.RowIndex
         '   MessageBox.Show(CStr(e.RowIndex) & ":" & CStr(e.ColumnIndex), "04_800")
         If e.ColumnIndex > 4 Then
            mbEventsEnabled = False
            ' zzSetRowChangedView(iRowIndex)
            zzSetRowChangedNew(iRowIndex, True)
            mbEventsEnabled = True
         End If

      End If
   End Sub

    

   Private Sub dgvMain_DataError(oSender As System.Object, e As System.Windows.Forms.DataGridViewDataErrorEventArgs) Handles dgvMain.DataError
      e.Cancel = True
   End Sub


   Private Sub frmEditBlockRefs_FormClosing(oSender As System.Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub frmEditBlockRefs_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
      mbEventsEnabled = True
   End Sub

	 

   Private Sub frmEditBlockRefs_Resize(oSender As System.Object, e As System.EventArgs) Handles Me.Resize
      If midgvMainLocationY <> 0 Then
         Try
            Me.dgvMain.Height = Me.ClientSize.Height - midgvMainLocationY
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "frmEditBlockRef_Resize")
         End Try
      End If
   End Sub

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
		Dim sVal As String
		Dim sTypeVal As String

		If oCurrentCell IsNot Nothing Then
			If oCurrentCell.Value IsNot Nothing Then
				sVal = oCurrentCell.Value.ToString()
				sTypeVal = oCurrentCell.Value.GetType().ToString()


            System.Windows.Forms.MessageBox.Show(sVal & vbCrLf & sTypeVal, "03_887A")
			Else
				System.Windows.Forms.MessageBox.Show("" & vbCrLf & "Nothing", "03_889")
				'	zzDispVal(sVal)
			End If

		End If
	End Sub
	Private Sub zzDispVal(sValue As String)
		Dim caVal() As Char = sValue.ToCharArray()
		Dim iLetter As Integer
		Dim sLetter As String
		Dim sMsg As String = ""
		For i As Integer = 0 To sValue.Length - 1
			iLetter = Convert.ToInt32(caVal(i))
			sLetter = CStr(iLetter)
			If i = 0 Then
				sMsg = sLetter
			Else
				sMsg &= vbCrLf & sLetter
			End If
		Next
		System.Windows.Forms.MessageBox.Show(sMsg, "04_349")
	End Sub

	Private Sub rdbPrompt_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbPrompt.CheckedChanged
      If mbEventsEnabled AndAlso Me.rdbPrompt.Checked Then
         mbEventsEnabled = False
         zzSetColumnHeader(False)
         mbEventsEnabled = True
      End If
	End Sub

	Private Sub rdbTag_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdbTag.CheckedChanged
      If mbEventsEnabled AndAlso Me.rdbTag.Checked Then
         mbEventsEnabled = False
         zzSetColumnHeader(True)
         mbEventsEnabled = True
      End If
	End Sub

	 
   Private Sub cmdDispSelectionSet_Click(sender As System.Object, e As System.EventArgs) Handles cmdDispSelectionSet.Click
      If mbEventsEnabled Then


         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

         zzGetBlockRefsSet()
         zzDispSelectRowsNew()

         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.Unlock()
      End If
   End Sub

	Private Sub rdbSelection_CheckedChanged(oSender As System.Object, e As System.EventArgs) Handles rdbSelection.CheckedChanged
      zzDispSelectRowsNew()
	End Sub

	Private Sub rdbFilter_CheckedChanged(oSender As System.Object, e As System.EventArgs) Handles rdbFilter.CheckedChanged
      zzDispSelectRowsNew()
	End Sub

	Private Sub cmdRemoveFilter_Click(oSender As System.Object, e As System.EventArgs) Handles cmdRemoveFilter.Click
		mcolSelectionBlockRefs.Clear()
      zzDispSelectRowsNew()
	End Sub

	Private Sub cmdDispSelSet_Click(oSender As System.Object, e As System.EventArgs) Handles cmdDispSelSet.Click

		Dim colSelectedRows As System.Windows.Forms.DataGridViewSelectedRowCollection = Me.dgvMain.SelectedRows
		'	Dim tAcObjID As ObjectId
		Dim taAcObjID() As ObjectId
		Dim iIndex As Integer = 0
		Dim oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet


		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()


		





		ReDim taAcObjID(Me.dgvMain.RowCount - 1)

		For Each oGridRow As System.Windows.Forms.DataGridViewRow In colSelectedRows
         taAcObjID(iIndex) = zzGetRowObjectIDNew(oGridRow)
			mcolSelectionBlockRefs.Add(taAcObjID(iIndex))
			If Not taAcObjID(iIndex).IsNull Then
				'''''''''''''DMAcadExt.AcadTransaction.Highlight(taAcObjID(iIndex))
			End If

			iIndex += 1
		Next
		If iIndex <> Me.dgvMain.RowCount Then
			ReDim Preserve taAcObjID(iIndex - 1)
		End If
		oSelSet = Autodesk.AutoCAD.EditorInput.SelectionSet.FromObjectIds(taAcObjID)
		Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		oEditor.SetImpliedSelection(oSelSet)
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()
	
		zzSetRecCount()
		
	End Sub
	Private Sub zzTestSet(oSelSet As Autodesk.AutoCAD.EditorInput.SelectionSet, bPlus As Boolean)
		Dim oaSelObj(oSelSet.Count - 1) As Autodesk.AutoCAD.EditorInput.SelectedObject
		oSelSet.CopyTo(oaSelObj, 0)

		MessageBox.Show(CStr(oSelSet.Count) & vbCrLf & oSelSet.ToString() & vbCrLf & oSelSet.GetType().ToString() & vbCrLf & oaSelObj(0).ToString() & vbCrLf & oaSelObj(0).GraphicsSystemMarkerPtr.ToInt64().ToString(), "04_230")
		Dim oSelectionSetDelayMarshalled As Autodesk.AutoCAD.EditorInput.SelectionSetDelayMarshalled = DirectCast(oSelSet, Autodesk.AutoCAD.EditorInput.SelectionSetDelayMarshalled)
		If bPlus Then
			mtGraphicsSystemMarkerPtr = oaSelObj(0).GraphicsSystemMarkerPtr
		End If
		Dim oSelected As Autodesk.AutoCAD.EditorInput.SelectedObject
		Dim iSelMethod As Autodesk.AutoCAD.EditorInput.SelectionMethod
		For i As Integer = 0 To oSelSet.Count - 1
			oSelected = New Autodesk.AutoCAD.EditorInput.SelectedObject(oaSelObj(i).ObjectId, iSelMethod, mtGraphicsSystemMarkerPtr)
		Next
	End Sub

	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
		Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell

		If oCurrentCell IsNot Nothing Then
			Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
			Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
			oGridRow.Cells.Item(0).Value = System.Windows.Forms.CheckState.Indeterminate

			 

		End If
	End Sub

   Private Sub frmEditBlockRefs_Shown(oSender As System.Object, e As System.EventArgs) Handles Me.Shown
      '	System.Windows.Forms.MessageBox.Show("Me.Shown", "04_549")
      zzSetBlockNew()
   End Sub

	
   Private Sub cmbBlockLayers_SelectedIndexChanged(oSender As System.Object, e As EventArgs) Handles cmbBlockLayers.SelectedIndexChanged
      If mbEventsEnabled Then

         If Me.cmbBlockLayers.SelectedIndex = 0 Then
            moMainDataView = New System.Data.DataView(moMainTable)
            msLayerSelected = String.Empty
         Else
            msLayerSelected = DirectCast(Me.cmbBlockLayers.SelectedItem, String)
            moMainDataView = New System.Data.DataView(moMainTable, zzGetRowFilter(False), msObjectIDFieldName, Data.DataViewRowState.CurrentRows)
         End If
         '  System.Windows.Forms.MessageBox.Show(moMainDataView.Count.ToString & ":" & moMainTable.Rows.Count.ToString() & vbCrLf & "Layer='" & sLayerSelected & "'", "04_558")
         Me.dgvMain.DataSource = moMainDataView
         zzSetRecCount()
         '  cmbBlockLayers.SelectedItem
         ' System.Windows.Forms.MessageBox.Show(CStr(cmbBlockLayers.SelectedItem.ToString) & vbCrLf & CStr(cmbBlockLayers.SelectedItem.GetType().ToString), "21_095")
      End If

   End Sub
   Private Function zzGetRowFilter(bSelection As Boolean) As String


      Dim sLayerCond, sSeleciontCond As String
      Dim bLayer As Boolean = msLayerSelected.Length <> 0
      If bLayer Then
         sLayerCond = "Layer='" & msLayerSelected & "' OR Layer=''"
      Else
         sLayerCond = String.Empty
      End If
      If bSelection Then
         sSeleciontCond = "Selected"
      Else
         sSeleciontCond = String.Empty
      End If
      If bLayer AndAlso bSelection Then
         Return sLayerCond & " AND " & sSeleciontCond
      Else
         Return sLayerCond & sSeleciontCond


      End If
   End Function
   Private Sub Button3_Click(oSender As System.Object, e As EventArgs) Handles Button3.Click
      System.Windows.Forms.MessageBox.Show(CStr(moMainTable.Rows.Count) & vbCrLf & CStr(dgvMain.Rows.Count), "21_095")

   End Sub

   
   Private Sub chkFirstRow_Tag_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkFirstRow_Tag.CheckedChanged
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

      zzFill()

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      '	System.Windows.Forms.MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked()), "21_532")
      DMAcadExt.AcadDocument.Unlock()
   End Sub

  
   Private Sub cmdImportExcel_Click(oSender As System.Object, e As EventArgs) Handles cmdImportExcel.Click
      Dim iRowIndex As Integer = -1
      Dim iColumnIndex As Integer = -1
      Dim oValue As System.Object
      Dim oRow As DataGridViewRow
      '  Dim oCell As DataGridViewCell
      If DMCommon.ExcelImport.OpenExcelApp() Then


         Dim oaValues As System.Object(,) = DMCommon.ExcelImport.GetSelectionValue()
         Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
         '  System.Windows.Forms.MessageBox.Show(CStr(oaValues.GetUpperBound(0)) & ":" & CStr(oaValues.GetUpperBound(1)), "10_001")
         If oCurrentCell IsNot Nothing AndAlso oaValues IsNot Nothing Then
            '  System.Windows.Forms.MessageBox.Show(CStr(oCurrentCell.RowIndex) & ":" & CStr(oCurrentCell.ColumnIndex), "10_002")

            For iInputRowIndex As Integer = 1 To oaValues.GetUpperBound(0)
               iRowIndex = iInputRowIndex + oCurrentCell.RowIndex - 1
               If iRowIndex < Me.dgvMain.Rows.Count Then
                  oRow = Me.dgvMain.Rows.Item(iRowIndex)

                  For iInputColumnIndex As Integer = 1 To oaValues.GetUpperBound(1)
                     iColumnIndex = iInputColumnIndex + oCurrentCell.ColumnIndex - 1
                     If iColumnIndex < oRow.Cells.Count Then
                        ' System.Windows.Forms.MessageBox.Show(CStr(oRow.Cells.Count) & vbCrLf & CStr(iRowIndex) & ":" & CStr(iColumnIndex) & vbCrLf & CStr(iInputRowIndex) & ":" & CStr(iInputColumnIndex), "10_003")
                        oValue = oaValues(iInputRowIndex, iInputColumnIndex)
                        If oValue IsNot Nothing Then
                           '  System.Windows.Forms.MessageBox.Show(CStr(oValue), "10_004")
                           oRow.Cells.Item(iColumnIndex).Value = oValue
                        End If
                     End If
                  Next
               End If
            Next
         End If

      End If



   End Sub

   Private Sub chkAdditionalBlock_CheckedChanged(oSender As System.Object, e As EventArgs) Handles chkAdditionalBlock.CheckedChanged
      If mbEventsEnabled Then
         mbEventsEnabled = False
         zzSetBlockNew()
         mbEventsEnabled = True
      End If
   End Sub
   Private Sub zzImportExcel_ParcelLegalArea()
      Dim iRowIndex As Integer = -1
      Dim iColumnIndex As Integer = -1
      Dim oValue As Object

      Dim dicParcelInput As Dictionary(Of ULong, Double) = New Dictionary(Of ULong, Double)()
      Dim iGush As UInteger, iGushAdd As UInteger, iParcel As UInteger
      Dim dArea As Double
      Dim dAreaUpd As Double


      Dim sGush, sGushAdd, sParcel, sArea As String
      Dim oRow As System.Data.DataRow
      If moCurrentAcadBlock.BlockName = "1603" Then


         '  Dim oCell As DataGridViewCell
         DMCommon.ExcelImport.OpenExcelApp()
         Dim oaValues As Object(,) = DMCommon.ExcelImport.GetSelectionValue()
         '   Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
         '   System.Windows.Forms.MessageBox.Show(CStr(oaValues.GetUpperBound(0)) & ":" & CStr(oaValues.GetUpperBound(1) & vbCrLf & ""), "10_001I")
         If oaValues IsNot Nothing AndAlso (oaValues.GetUpperBound(1) = 2 OrElse oaValues.GetUpperBound(1) = 3) Then
            '  System.Windows.Forms.MessageBox.Show(CStr(oCurrentCell.RowIndex) & ":" & CStr(oCurrentCell.ColumnIndex), "10_002")
            If oaValues.GetUpperBound(1) = 4 Then
               For iInputRowIndex As Integer = 1 To oaValues.GetUpperBound(0)
                  If zzToUInt(oaValues(iInputRowIndex, 1), True, iGush) AndAlso zzToUInt(oaValues(iInputRowIndex, 2), False, iGushAdd) AndAlso zzToUInt(oaValues(iInputRowIndex, 3), True, iParcel) AndAlso zzToDouble(oaValues(iInputRowIndex, 4), True, dArea) Then
                     If Not dicParcelInput.ContainsKey(zzGetParcelKey(iGush, iGushAdd, iParcel)) Then
                        dicParcelInput.Add(zzGetParcelKey(iGush, iGushAdd, iParcel), dArea)
                     End If

                  Else

                     MessageBox.Show("Row #" & iInputRowIndex.ToString(), "Input Error 1")
                     Return
                  End If
               Next
            End If
            '   System.Windows.Forms.MessageBox.Show(CStr(dicParcelInput.Count), "10_002")
            If oaValues.GetUpperBound(1) = 3 Then
               For iInputRowIndex As Integer = 1 To oaValues.GetUpperBound(0)
                  If zzToUInt(oaValues(iInputRowIndex, 1), True, iGush) AndAlso zzToUInt(oaValues(iInputRowIndex, 2), True, iParcel) AndAlso zzToDouble(oaValues(iInputRowIndex, 3), True, dArea) Then
                     If Not dicParcelInput.ContainsKey(zzGetParcelKey(iGush, 0, iParcel)) Then
                        dicParcelInput.Add(zzGetParcelKey(iGush, 0UI, iParcel), dArea)
                     End If

                  Else
                     MessageBox.Show("Row #" & iInputRowIndex.ToString(), "Input Error 2")
                     Return
                  End If
               Next
            End If

            '   System.Windows.Forms.MessageBox.Show(CStr(dicParcelInput.Count), "10_003")
            Dim iCnt As Integer
            Dim iCnt1 As Integer
            Dim iCnt2 As Integer


            For iRowIndex = 0 To Me.dgvMain.Rows.Count - 1

               dArea = 0.0
               oRow = moMainTable.Rows.Item(iRowIndex)
               sArea = DMCommon.Functions.CStrN(oRow.Item("LEGAL_AREA"))
               If zzToDouble(sArea, False, dArea) AndAlso dArea = 0.0 Then
                  iCnt1 += 1
                  sGush = DMCommon.Functions.CStrN(oRow.Item("LOT_NUM"))
                  sGushAdd = DMCommon.Functions.CStrN(oRow.Item("GUSH_SUFFI"))
                  sParcel = DMCommon.Functions.CStrN(oRow.Item("PARCEL_NUM"))
                  If zzToUInt(sGush, True, iGush) AndAlso zzToUInt(sGushAdd, False, iGushAdd) AndAlso zzToUInt(sParcel, True, iParcel) Then
                     iCnt2 += 1
                     If dicParcelInput.TryGetValue(zzGetParcelKey(iGush, iGushAdd, iParcel), dAreaUpd) Then
                        oRow.Item("LEGAL_AREA") = dAreaUpd
                        oRow.Item("Modified") = True
                        oRow.Item("Changed") = True
                        iCnt += 1
                     End If
                  End If
               End If
            Next

            System.Windows.Forms.MessageBox.Show(CStr(iCnt) & ":" & CStr(iCnt1) & ":" & CStr(iCnt2), "10_004")
         End If
      End If
   End Sub
   Private Function zzGetParcelKey(iGush As UInteger, iGushAdd As UInteger, iParcel As UInteger) As ULong
      Try
         Dim lRes As ULong = Convert.ToUInt64(iGush * 10000000UL + iGushAdd * 10000UL + iParcel)
         Return lRes
      Catch oEx As Exception
         MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & iGush.ToString() & vbCrLf & iParcel.ToString(), "04_454")
      End Try



   End Function
   Private Function zzToUInt(oValue As System.Object, bRequired As Boolean, ByRef iRes As UInteger) As Boolean
      If oValue IsNot Nothing Then
         Try
            iRes = Convert.ToUInt32(oValue)
            Return True
         Catch oEx As Exception
            Return False
         End Try


      ElseIf bRequired Then
         Return False
      Else
         iRes = 0UI
         Return True
      End If

   End Function
   Private Function zzToUInt(sValue As String, bRequired As Boolean, ByRef iRes As UInteger) As Boolean
      If Not String.IsNullOrEmpty(sValue) Then
         Try
            Return UInt32.TryParse(sValue, iRes)
         Catch oEx As Exception
            Return False
         End Try
      ElseIf bRequired Then
         Return False
      Else
         iRes = 0UI
         Return True
      End If
   End Function
   Private Function zzToDouble(oValue As System.Object, bRequired As Boolean, ByRef dRes As Double) As Boolean
      If oValue IsNot Nothing Then
         Try
            dRes = Convert.ToDouble(oValue)
            Return True
         Catch oEx As Exception
            Return False
         End Try

      ElseIf bRequired Then
         Return False
      Else
         dRes = 0.0
         Return True
      End If

   End Function
   Private Function zzToDouble(sValue As String, bRequired As Boolean, ByRef dRes As Double) As Boolean
      If Not String.IsNullOrEmpty(sValue) Then
         Try
            Return Double.TryParse(sValue, dRes)

         Catch oEx As Exception
            Return False
         End Try

      ElseIf bRequired Then
         Return False
      Else
         dRes = 0.0
         Return True
      End If

   End Function

   Private Sub Button4A_Click(oSender As System.Object, e As EventArgs)
      Dim oCurrentCell As System.Windows.Forms.DataGridViewCell = Me.dgvMain.CurrentCell
      Dim oVal As Object
      If oCurrentCell IsNot Nothing Then
         Dim iCurrentRowIndex As Integer = oCurrentCell.RowIndex
         Dim oGridRow As DataGridViewRow = Me.dgvMain.Rows.Item(iCurrentRowIndex)
         oVal = oCurrentCell.Value
         MessageBox.Show(oVal.ToString() & vbCrLf & oVal.GetType.ToString())
         Dim Row As System.Data.DataRow = moMainTable.Rows.Item(iCurrentRowIndex)
         MessageBox.Show(Row.Item("LEGAL_AREA").GetType().ToString() & vbCrLf & Row.Item("PARCEL_NUM").GetType().ToString() & vbCrLf & Row.Item("LOT_NUM").GetType().ToString() & vbCrLf & Row.Item("GUSH_SUFFI").GetType().ToString(), "09_100")
      End If



   End Sub

   Private Sub Button4_Click(oSender As System.Object, e As EventArgs) Handles Button4.Click
      zzImportExcel_ParcelLegalArea()
   End Sub

End Class