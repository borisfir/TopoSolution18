Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmUD_General
	'P:\2006\060169\060169_מסמכי מחלקת תצר\hanit_blocks_20141118\tzr_hanit_sample2.dwg
	Private Const msBorderBlockName As String = "BORDER"
   Private Const msOldBlockName As String = "SU77_2"

   Private Const msTazarFrameBlockName As String = "TAZAR_RISH-MAX"
   Private Const msGenBlockName As String = "C1640"
   Private Const msEllipseBlockName As String = "C1642"
   Private Const msStampBlockName As String = "C1643"


   Private Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit"
   Private Const msDistrictAttribTag As String = "REGION"
   Private Const msSubdistrictAttribTag As String = "COUNTY"
   Private Const msLocalityNameAttribTag As String = "SETTLEMENT"
   Private Const msLocalityCodeAttribTag As String = "SETTLEMENT_CODE"
   Private Const msGushAttribTag As String = "GUSH_NUM"
   Private Const msParcelsAttribTag As String = "PARCELS"
   Private Const msOrdererAttribTag As String = "ORDERER"

   Private Const msPlanTypeAttribTag As String = "PROCESS_TYPE"

   Private Const msWorkOrderNumAttribTag As String = "WORK_ORDER"

   Private Const msNetAttribTag As String = "GRID_NAME"
   Private Const msScaleAttribTag As String = "SCALE"

   Private Const msTabaNameAttribTag As String = "TABA_NAMES"
   Private Const msSurveyDateAttribTag As String = "SURVEY_DATE"
   Private Const msEndDateAttribTag As String = "FINISH_DATE"
   Private Const msUpdateDateAttribTag As String = "UPDATE_DATE"
   Private Const msGushLegalAreaAttribTag As String = "GUSH_LEGAL_AREA"
   Private Const msGushRegStatusNameAttribTag As String = "GUSH_REGISTER_STATUS"

   Private Const msGushLastParcelNameAttribTag As String = "GUSH_LAST_PARCEL"
   Private Const msGeneralCommentNameAttribTag As String = "GENERAL_COMMENT"
   Private Const msGBPrefix As String = "ג.ב."
   Private Const msProcessNameAttribTag As String = "PROCESS_NAME"
   Private Const msSerialNumAttribTag As String = "SERIAL"

   Private Const msPlaceAttribTag As String = "PLACE"
   Private Const msSurveyorIDAttribTag As String = "SURVEYOR"
   Private Const msSurveyorNameAttribTag As String = "SURVEYOR_NAME"


   Private msGBDOSEnd As String = New String({Chr(46), Chr(129), Chr(46), Chr(130)})

   Private moaNoteChecks(17) As NoteCheck

   '	Dim c As Char = New Char()

   Dim chars() As Char = {ChrW(&H61), ChrW(&H308)}











   Private Const mdDeltaPosX As Double = 400
   Private Const mdDeltaPosY As Double = 250
   Private mcolInvisibleAttribs As ObjectIdCollection = New ObjectIdCollection()
   Private mtGenBlockRecObjID As ObjectId
   Private moaAttribDefs() As AttributeDefinition = Nothing
   Private mtGenBlockRefObjID As ObjectId
   Private mtGenInsPoint As Autodesk.AutoCAD.Geometry.Point3d

   Private mtEllipseBlockRefObjID As ObjectId
   Private mtEllipseInsPoint As Autodesk.AutoCAD.Geometry.Point3d

   Private mtStampBlockRefObjID As ObjectId
   Private mtStampInsPoint As Autodesk.AutoCAD.Geometry.Point3d


   Private mbInsPointExists As Boolean = False
   Private Shared moNoteInsPoint As DMAcadExt.TPlnPoint
   Private mdNoteMarginX As Double = 44.2
   Private mdNoteMarginY As Double = 18.8
   Private mdNoteRowHeight As Double = 6

   Private moFrameAcadBlock As DMAcadExt.AcadBlock

   Private moGenAcadBlock As DMAcadExt.AcadBlock
   Private moEllipseAcadBlock As DMAcadExt.AcadBlock
   Private moStampAcadBlock As DMAcadExt.AcadBlock




   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.
      zzMyInitializeComponent()
      zzInit()
      zzSetAttribTags()
      AcadReport.RepApp.InitDWGScaleFactor()
   End Sub
   Public Sub zzInit()
      moaNoteChecks(0) = New NoteCheck(Me.chkNote01)
      moaNoteChecks(1) = New NoteCheck(Me.chkNote02)
      moaNoteChecks(2) = New NoteCheck(Me.chkNote03)
      moaNoteChecks(3) = New NoteCheck(Me.chkNote04)
      moaNoteChecks(4) = New NoteCheck(Me.chkNote05)
      moaNoteChecks(5) = New NoteCheck(Me.chkNote06)
      moaNoteChecks(6) = New NoteCheck(Me.chkNote07)
      moaNoteChecks(7) = New NoteCheck(Me.chkNote08)
      moaNoteChecks(8) = New NoteCheck(Me.chkNote09)
      moaNoteChecks(9) = New NoteCheck(Me.chkNote10)
      moaNoteChecks(10) = New NoteCheck(Me.chkNote11)
      moaNoteChecks(11) = New NoteCheck(Me.chkNote12)
      moaNoteChecks(12) = New NoteCheck(Me.chkNote13)
      moaNoteChecks(13) = New NoteCheck(Me.chkNote14)
      moaNoteChecks(14) = New NoteCheck(Me.chkNote15)
      moaNoteChecks(15) = New NoteCheck(Me.chkNote16)
      moaNoteChecks(16) = New NoteCheck(Me.chkNote17)





   End Sub

   Private Sub zzMyInitializeComponent()
      '	System.Windows.Forms.MessageBox.Show("OK:", "ServerDB - GetLocalityList")
      'Dim oDistricts As System.Collections.Generic.IList(Of DMCommon.ItemData) = TPlServerDB.ServerDB.CurrentServerDB.GetDistrictsList()

      Me.cmbDistrict.ValueMember = DMCommon.ItemData.ValueMember
      Me.cmbDistrict.DisplayMember = DMCommon.ItemData.DisplayMember

      Me.cmbDistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetDistrictsList()


      Me.cmbSubdistrict.ValueMember = DMCommon.ItemData.ValueMember
      Me.cmbSubdistrict.DisplayMember = DMCommon.ItemData.DisplayMember

      Me.cmbSubdistrict.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSubdistrictsList()

      Me.cmbLocality.ValueMember = DMCommon.ItemData.ValueMember
      Me.cmbLocality.DisplayMember = DMCommon.ItemData.DisplayMember

      Me.cmbLocality.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetLocalityList()


      Me.cmbScales.ValueMember = DMCommon.ItemData.ValueMember
      Me.cmbScales.DisplayMember = DMCommon.ItemData.DisplayMember

      Me.cmbScales.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetScalesList()


      Me.cmbSurveyor.DataSource = TPlServerDB.ServerDB.CurrentServerDB.GetSurveyorLicenseView
      Me.cmbSurveyor.ValueMember = "LicenseNo"
      Me.cmbSurveyor.DisplayMember = "Empl_name"
      'חדשה/2005/2012
      If False Then
         Me.dtpEndDate.Value = DateTime.Today
         Me.dtpEndDate.Checked = False
         MessageBox.Show(Me.dtpSurveyDate.Value.ToString(), "04_230")
         Me.dtpSurveyDate.Value = DateTime.Today.AddMonths(-12)

         MessageBox.Show(Me.dtpSurveyDate.Value.ToString(), "04_240")
         Me.dtpUpdateDate.Value = DateTime.Today
      End If

   End Sub

   Private Sub frmUD_General_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub

   Private Sub frmUD_General_Load(oSender As System.Object, e As System.EventArgs) Handles Me.Load
      '	Me.cmbDistrict.SelectedIndex = -1

      Me.cmbDistrict.Text = String.Empty
      '	Me.cmbSubdistrict.Focus()
      Me.cmbSubdistrict.Text = String.Empty
      Me.cmbLocality.Text = String.Empty
      zzLoadBlockRefData(True)
      zzSetScale()

   End Sub
   Private Sub zzSetInsertionPoint()
     
      Dim oaAttribDefs() As AttributeDefinition = Nothing
      Dim oBlockRefData As DMAcadExt.BlockRefData
      moFrameAcadBlock = New DMAcadExt.AcadBlock(msBorderBlockName)
      moFrameAcadBlock.OpenForRead()
      moFrameAcadBlock.LoadAllReferences()
      If moFrameAcadBlock.ReferenceCount = 0 Then
         moFrameAcadBlock = New DMAcadExt.AcadBlock(msTazarFrameBlockName)
         moFrameAcadBlock.OpenForRead()
         moFrameAcadBlock.LoadAllReferences()
         If moFrameAcadBlock.ReferenceCount <> 1 Then
            'Error
            MessageBox.Show("Number of Frames: " & CStr(moFrameAcadBlock.ReferenceCount), "04_928")
         End If

      ElseIf moFrameAcadBlock.ReferenceCount <> 1 Then
         'Error


      End If


      oBlockRefData = moFrameAcadBlock.GetBlockRefData(0)
       

      If oBlockRefData.IsNotEmpty Then




         Dim oInsPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBlockRefData.Position)
         '	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
         Dim oBlockInsPoint As DMAcadExt.TPlnPoint

         mtStampInsPoint = oInsPoint.AcGePoint3d

         oBlockInsPoint = oInsPoint.GetMoved(0.0, oBlockRefData.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
         '	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
         mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


         oBlockInsPoint = oInsPoint.GetMoved(oBlockRefData.ScaleFactors.X, oBlockRefData.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
         '	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
         mtGenInsPoint = oBlockInsPoint.AcGePoint3d


         mbInsPointExists = True

         moNoteInsPoint = oInsPoint.GetMoved(oBlockRefData.ScaleFactors.X - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
         MessageBox.Show(CStr(mtStampInsPoint.ToString()) & vbCrLf & mtEllipseInsPoint.ToString() & vbCrLf & moNoteInsPoint.ToString(), "04_929")



         '   HebrewTrans.vb() : Line 334


      End If


      




   End Sub
   Private Sub zzSetInsertionPointOld()
      Dim tBorderBlockRecObjID As ObjectId
      Dim tBorderBlockRefObjID As ObjectId
      Dim oaAttribDefs() As AttributeDefinition = Nothing
      Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(msBorderBlockName, tBorderBlockRefObjID, tBorderBlockRecObjID, oaAttribDefs)


      If iBlockRefCount = 1 Then
         Dim oBorderBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBorderBlockRefObjID, OpenMode.ForRead)

         Dim oInsPoint As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(oBorderBlockRef.Position)
         '	DMAcadExt.AcadDocument.WriteMessage("01:" & oInsPoint.Coordinates2d)
         Dim oBlockInsPoint As DMAcadExt.TPlnPoint

         mtStampInsPoint = oInsPoint.AcGePoint3d

         oBlockInsPoint = oInsPoint.GetMoved(0.0, oBorderBlockRef.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
         '	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
         mtEllipseInsPoint = oBlockInsPoint.AcGePoint3d


         oBlockInsPoint = oInsPoint.GetMoved(oBorderBlockRef.ScaleFactors.X, oBorderBlockRef.ScaleFactors.Y)  '- AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosX  - AcadReport.RepApp.DrawingScaleFactor * mdDeltaPosY
         '	DMAcadExt.AcadDocument.WriteMessage("02:" & oGenInsPoint.Coordinates2d)
         mtGenInsPoint = oBlockInsPoint.AcGePoint3d


         mbInsPointExists = True

         moNoteInsPoint = oInsPoint.GetMoved(oBorderBlockRef.ScaleFactors.X - mdNoteMarginX * AcadReport.RepApp.DrawingScaleFactor, mdNoteMarginY * AcadReport.RepApp.DrawingScaleFactor)
         MessageBox.Show(CStr(moNoteInsPoint.Coordinates) & vbCrLf & "", "04_929")
      Else
         MessageBox.Show("number of Frames: " & CStr(iBlockRefCount), "04_928")
      End If

   End Sub
   Private Sub zzSetScale()
      Dim dScale As Double = 1000 * AcadReport.RepApp.DrawingScaleFactor
      Dim sScale As String = "1:" & Convert.ToString(dScale)
      Me.cmbScales.Text = sScale
   End Sub
   Private Sub cmdOK_Click(oSender As System.Object, e As System.EventArgs) Handles cmdOK.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)



      MessageBox.Show(CStr(mbInsPointExists) & vbCrLf & msBorderBlockName, "04_944")
      If mbInsPointExists Then
         Dim oDynValue As Object
         Dim oHebText As DMCommon.HebrewTrans

         '	Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(colBorderBlockRefs.Item(0), OpenMode.ForWrite)
         'MessageBox.Show(CStr(oBlockRef Is Nothing) & vbCrLf & colBorderBlockRefs.Item(0).ToString(), "04_130")


         Dim oaAttribDefs() As AttributeDefinition = Nothing
         '	Dim sBlockName As String = "C1640"
         Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
         If cmbDistrict.SelectedIndex <> -1 Then
            dicAttribValues.Add(msDistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbDistrict.Text, True))
         End If
         If cmbSubdistrict.SelectedIndex <> -1 Then
            dicAttribValues.Add(msSubdistrictAttribTag, DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, True)) '  DMCommon.Hebrew.WordToDOS(cmbSubdistrict.Text, 
         End If
         If cmbLocality.SelectedIndex <> -1 Then
            dicAttribValues.Add(msLocalityNameAttribTag, DMCommon.Hebrew.WordToDOS(cmbLocality.Text, True))
            oDynValue = cmbLocality.SelectedValue
            dicAttribValues.Add(msLocalityCodeAttribTag, oDynValue.ToString())

         End If
         dicAttribValues.Add(msGushAttribTag, Me.txtGush.Text)
         dicAttribValues.Add(msParcelsAttribTag, Me.txtParcels.Text)

         '''''''''''''''''	dicAttribValues.Add(msOrdererAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtOrderer.Text, True))
         oHebText = New DMCommon.HebrewTrans(Me.txtOrderer.Text, True)
         dicAttribValues.Add(msOrdererAttribTag, oHebText.GetDOSDest())


         dicAttribValues.Add(msProcessNameAttribTag, Me.txtProcessName.Text)


         dicAttribValues.Add(msPlanTypeAttribTag, zzGetPlanType())


         '	dicAttribValues.Add(msSerialNumAttribTag, Me.txtSerialNum.Text & " " & DMCommon.Hebrew.WordToDOS(msGBPrefix, True))
         dicAttribValues.Add(msSerialNumAttribTag, Me.txtSerialNum.Text & " " & msGBDOSEnd)


         dicAttribValues.Add(msWorkOrderNumAttribTag, Me.txtWorkOrder.Text)
         dicAttribValues.Add(msScaleAttribTag, Me.cmbScales.Text)
         dicAttribValues.Add(msNetAttribTag, zzGetNetType())
         dicAttribValues.Add(msSurveyorIDAttribTag, zzGetLicenseNo())

         oHebText = New DMCommon.HebrewTrans(Me.txtPlanNum.Text, True)
         dicAttribValues.Add(msTabaNameAttribTag, oHebText.GetDOSDest())
         '	dicAttribValues.Add(msTabaNameAttribTag, DMCommon.Hebrew.WinToAcadC(Me.txtPlanNum.Text))
         If dtpSurveyDate.Checked Then
            dicAttribValues.Add(msSurveyDateAttribTag, dtpSurveyDate.Text)
         End If
         If dtpEndDate.Checked Then
            dicAttribValues.Add(msEndDateAttribTag, Me.dtpEndDate.Text)
         End If


         If Me.dtpUpdateDate.Checked Then
            dicAttribValues.Add(msUpdateDateAttribTag, Me.dtpUpdateDate.Text)
         End If

         dicAttribValues.Add(msGushLegalAreaAttribTag, Me.txtGushLegalArea.Text)
         dicAttribValues.Add(msGushRegStatusNameAttribTag, DMCommon.Hebrew.WordToDOS(zzGetGushStatus(), True))

         dicAttribValues.Add(msGushLastParcelNameAttribTag, Me.txtLastParcelName.Text)
         dicAttribValues.Add(msGeneralCommentNameAttribTag, DMCommon.Hebrew.WordToDOS(Me.txtGeneralComment.Text, True))

         If mtGenBlockRefObjID.IsNull Then

            If mtGenBlockRecObjID.IsNull Then
               mtGenBlockRecObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, msGenBlockName, oaAttribDefs)
            Else
               oaAttribDefs = moaAttribDefs
            End If

            mtGenBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtGenBlockRecObjID, mtGenInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor)
         Else
            Dim oGenBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(mtGenBlockRefObjID, OpenMode.ForWrite)
            If oGenBlockRef IsNot Nothing Then
               DMAcadExt.AcadTransaction.UpdateAttribText(oGenBlockRef, dicAttribValues)

            End If

         End If

         If Not mtGenBlockRefObjID.IsNull Then
            Me.chkAllAttribVisible.Enabled = True
            zzLoadAttribData(False)
         End If

      End If


      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
      DMAcadExt.AcadDocument.UpdateScreen()
      '	MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "04_806")
   End Sub
   Private Function zzGetLicenseNo() As String
      If cmbSurveyor.SelectedIndex <> -1 Then
         Dim oRow As System.Data.DataRowView = TryCast(cmbSurveyor.SelectedItem, System.Data.DataRowView)
         If oRow IsNot Nothing Then
            Return oRow.Item("LicenseNo").ToString()

         End If
         MessageBox.Show(cmbSurveyor.SelectedItem.ToString(), "04_840")
         Return "999"
      Else
         Return String.Empty
      End If
   End Function
   Private Function zzGetPlanType() As String
      Dim iPlanType As Integer
      If Me.rdbPlanType1.Checked Then
         iPlanType = 1
      ElseIf Me.rdbPlanType2.Checked Then
         iPlanType = 2
      ElseIf Me.rdbPlanType12.Checked Then
         iPlanType = 12
      End If

      If iPlanType <> 0 Then
         Return Convert.ToString(iPlanType)
      Else
         Return String.Empty
      End If
   End Function
   Private Sub zzParseDate(sAttribValue As String, ByRef oDatePic As DateTimePicker)
      If sAttribValue.Length = 0 Then
         oDatePic.Checked = False
      Else
         Dim dtValue As DateTime
         If DateTime.TryParse(sAttribValue, dtValue) Then
            oDatePic.Value = dtValue
         End If
      End If

   End Sub
   Private Sub zzParsePlanType(sAttribValue As String)
      Select Case sAttribValue
         Case "1"
            Me.rdbPlanType1.Checked = True
         Case "2"
            Me.rdbPlanType2.Checked = True
         Case "12"
            Me.rdbPlanType12.Checked = True
         Case String.Empty
            If Me.rdbPlanType1.Checked Then
               Me.rdbPlanType1.Checked = False
            ElseIf Me.rdbPlanType2.Checked Then
               Me.rdbPlanType2.Checked = False
            ElseIf Me.rdbPlanType12.Checked Then
               Me.rdbPlanType12.Checked = False
            End If
         Case Else
            MessageBox.Show(sAttribValue & vbCrLf & "", "Err: " & msPlanTypeAttribTag)
      End Select

   End Sub
   Private Function zzGetNetType() As String
      'IG05/12
      If Me.rdbNetNew.Checked Then
         Return DMCommon.Hebrew.WordToDOS("חדשה", True)
      ElseIf Me.rdbNetIG05.Checked Then
         Return "2012"
      ElseIf Me.rdbNetIG12.Checked Then
         Return "2005"
      Else
         Return String.Empty
      End If

   End Function
   Private Sub zzParseNetType(sAttribValue As String)
      Select Case sAttribValue
         Case DMCommon.Hebrew.WordToDOS("חדשה", True)
            Me.rdbNetNew.Checked = True

         Case "2012"
            Me.rdbNetIG05.Checked = True
         Case "2005"
            Me.rdbNetIG12.Checked = True
         Case String.Empty
            If Me.rdbNetIG05.Checked Then
               Me.rdbNetIG05.Checked = False
            ElseIf Me.rdbNetIG12.Checked Then
               Me.rdbNetIG12.Checked = False
            ElseIf Me.rdbNetNew.Checked Then
               Me.rdbNetNew.Checked = False
            End If
         Case Else
            MessageBox.Show(DMCommon.Hebrew.FromAcad(sAttribValue) & vbCrLf & "חדשה" & vbCrLf & "2012" & vbCrLf & "2005", "Err: " & msNetAttribTag)

      End Select


   End Sub
   Private Sub zzParseGushStatus(sAttribValue As String)
      Dim sWinValue As String = DMCommon.Hebrew.FromAcad(sAttribValue)
      Select Case sWinValue
         Case String.Empty
            Me.chkGushStatus.CheckState = CheckState.Indeterminate
         Case "מוסדר"
            Me.chkGushStatus.Checked = True
         Case "לא מוסדר"
            Me.chkGushStatus.Checked = False
         Case Else
            MessageBox.Show(sWinValue & vbCrLf & "מוסדר" & vbCrLf & "לא מוסדר", "Err: " & msGushRegStatusNameAttribTag)

      End Select
   End Sub
   Private Function zzGetGushStatus() As String

      If Me.chkGushStatus.Checked Then
         Return "מוסדר"
      Else
         Return "לא מוסדר"
      End If





   End Function
   Private Sub zzLoadAttribData()
      Dim oBlockRefData As DMAcadExt.BlockRefData
      Dim sLocalityCode, sLocalityName As String
      Dim sPlace As String

      Dim sSurveyorName As String
      Dim oHebText As DMCommon.HebrewTrans
      moGenAcadBlock = New DMAcadExt.AcadBlock(msGenBlockName)
      moGenAcadBlock.Fields = New String() {msDistrictAttribTag, msSubdistrictAttribTag, msLocalityNameAttribTag, msGushAttribTag, msParcelsAttribTag, msOrdererAttribTag _
                        , msPlanTypeAttribTag, msNetAttribTag, msScaleAttribTag, msGushLegalAreaAttribTag, msGushRegStatusNameAttribTag, msGushLastParcelNameAttribTag _
                        , msLocalityCodeAttribTag}
      moGenAcadBlock.OpenForRead()
      moGenAcadBlock.LoadAllReferences()
      If moGenAcadBlock.ReferenceCount = 1 Then
         oBlockRefData = moGenAcadBlock.GetBlockRefData(0)

         Me.cmbDistrict.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msDistrictAttribTag))
         Me.cmbSubdistrict.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msSubdistrictAttribTag))
         Me.cmbLocality.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msLocalityNameAttribTag))
         Me.txtGush.Text = oBlockRefData.GetAttribValue(msGushAttribTag)
         Me.txtParcels.Text = oBlockRefData.GetAttribValue(msParcelsAttribTag)

         '	Me.txtOrderer.Text = DMCommon.Hebrew.ToUnicode(oAttribRef.TextString) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, True) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, False)
         'System.Windows.Forms.MessageBox.Show(oAttribRef.TextString, "04_215")
         '	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
         '''''''''''''''''''''''''''''''''''''   oHebText = New DMCommon.HebrewTrans(oAttribRef.TextString, False)
         Me.txtOrderer.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msOrdererAttribTag))

         zzParsePlanType(oBlockRefData.GetAttribValue(msPlanTypeAttribTag))
         zzParseNetType(oBlockRefData.GetAttribValue(msNetAttribTag))
         Me.cmbScales.Text = oBlockRefData.GetAttribValue(msScaleAttribTag)
         Me.txtGushLegalArea.Text = oBlockRefData.GetAttribValue(msGushLegalAreaAttribTag)
         zzParseGushStatus(oBlockRefData.GetAttribValue(msGushRegStatusNameAttribTag))
         Me.txtLastParcelName.Text = oBlockRefData.GetAttribValue(msGushLastParcelNameAttribTag)
         sLocalityCode = oBlockRefData.GetAttribValue(msLocalityCodeAttribTag)








         '''''''''''''''''''''''''''''''''''??????????????/////   Me.txtWorkOrder.Text = oBlockRefData.GetAttribValue(msWorkOrderNumAttribTag)




         '	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
         oHebText = New DMCommon.HebrewTrans(oBlockRefData.GetAttribValue(msTabaNameAttribTag), False)
         '	Me.txtPlanNum.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
         Me.txtPlanNum.Text = oHebText.GetWinDest(False)


         '	dtpSurveyDate.Text = oAttribRef.TextString
      

         Me.txtGeneralComment.Text = DMCommon.Hebrew.FromAcad(oBlockRefData.GetAttribValue(msGeneralCommentNameAttribTag))


        
      Else

      End If
      moEllipseAcadBlock = New DMAcadExt.AcadBlock(msEllipseBlockName)
      moEllipseAcadBlock.Fields = New String() {msProcessNameAttribTag, msSerialNumAttribTag}
      moEllipseAcadBlock.OpenForRead()
      moEllipseAcadBlock.LoadAllReferences()
      If moEllipseAcadBlock.ReferenceCount = 1 Then
         oBlockRefData = moEllipseAcadBlock.GetBlockRefData(0)
         Me.txtProcessName.Text = oBlockRefData.GetAttribValue(msProcessNameAttribTag)
         Me.txtSerialNum.Text = oBlockRefData.GetAttribValue(msSerialNumAttribTag)
         zzParseSerialNum(oBlockRefData.GetAttribValue(msSerialNumAttribTag))


      End If

      moStampAcadBlock = New DMAcadExt.AcadBlock(msStampBlockName)
      moStampAcadBlock.Fields = New String() {msSurveyDateAttribTag, msEndDateAttribTag, msUpdateDateAttribTag, msPlaceAttribTag, msSurveyorIDAttribTag, msSurveyorNameAttribTag}
      moStampAcadBlock.OpenForRead()
      moStampAcadBlock.LoadAllReferences()
      If moStampAcadBlock.ReferenceCount = 1 Then
         oBlockRefData = moStampAcadBlock.GetBlockRefData(0)
         zzParseDate(oBlockRefData.GetAttribValue(msSurveyDateAttribTag), dtpSurveyDate)

         zzParseDate(oBlockRefData.GetAttribValue(msEndDateAttribTag), dtpEndDate)

         zzParseDate(oBlockRefData.GetAttribValue(msUpdateDateAttribTag), dtpUpdateDate)

         sPlace = oBlockRefData.GetAttribValue(msPlaceAttribTag)

         Me.cmbSurveyor.SelectedValue = oBlockRefData.GetAttribValue(msSurveyorIDAttribTag)
         sSurveyorName = oBlockRefData.GetAttribValue(msSurveyorNameAttribTag)


      End If

     

   End Sub

   Private Sub zzLoadAttribData(bLoadData As Boolean)


      Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(mtGenBlockRefObjID, OpenMode.ForRead)

      If oBlockRef IsNot Nothing Then

         Dim colAttributes As AttributeCollection
         Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
         Dim tAttribObjID As ObjectId
         Dim oAttribRef As AttributeReference
         Try
            colAttributes = oBlockRef.AttributeCollection
         Catch oEx As Exception
            Return
         End Try

         mcolInvisibleAttribs.Clear()
         If colAttributes IsNot Nothing AndAlso colAttributes.Count > 0 Then
            Dim sLocalityCode, sLocalityName As String
            Dim oHebText As DMCommon.HebrewTrans
            For iIndex As Integer = 0 To colAttributes.Count - 1
               Try
                  tAttribObjID = colAttributes.Item(iIndex)
                  DMAcadExt.AcadDocument.WriteMessage("#193a " & tAttribObjID.ToString())
                  oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForRead)
                  oAttribRef = DirectCast(oDBObject, AttributeReference)
                  If oAttribRef.Invisible Then
                     mcolInvisibleAttribs.Add(tAttribObjID)
                  End If
                  If bLoadData Then
                     Select Case oAttribRef.Tag
                        Case msDistrictAttribTag
                           Me.cmbDistrict.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                        Case msSubdistrictAttribTag
                           Me.cmbSubdistrict.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                        Case msLocalityNameAttribTag
                           sLocalityName = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                           Me.cmbLocality.Text = sLocalityName
                        Case msLocalityCodeAttribTag
                           sLocalityCode = oAttribRef.TextString

                        Case msGushAttribTag
                           Me.txtGush.Text = oAttribRef.TextString
                        Case msParcelsAttribTag
                           Me.txtParcels.Text = oAttribRef.TextString
                        Case msOrdererAttribTag
                           '	Me.txtOrderer.Text = DMCommon.Hebrew.ToUnicode(oAttribRef.TextString) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, True) & "  " & DMCommon.Hebrew.FromDOS(oAttribRef.TextString, False)
                           'System.Windows.Forms.MessageBox.Show(oAttribRef.TextString, "04_215")
                           '	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
                           oHebText = New DMCommon.HebrewTrans(oAttribRef.TextString, False)
                           Me.txtOrderer.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                           '	Me.txtOrderer.Text = oHebText.GetWinDest(False)

                        Case msProcessNameAttribTag
                           Me.txtProcessName.Text = oAttribRef.TextString
                        Case msPlanTypeAttribTag
                           zzParsePlanType(oAttribRef.TextString)
                        Case msSerialNumAttribTag
                           '  Me.txtSerialNum.Text = oAttribRef.TextString
                           zzParseSerialNum(oAttribRef.TextString)
                        Case msWorkOrderNumAttribTag
                           Me.txtWorkOrder.Text = oAttribRef.TextString
                        Case msNetAttribTag
                           zzParseNetType(oAttribRef.TextString)
                        Case msScaleAttribTag
                           Me.cmbScales.Text = oAttribRef.TextString

                        Case msSurveyorIDAttribTag
                           cmbSurveyor.SelectedValue = oAttribRef.TextString
                        Case msTabaNameAttribTag
                           '	DMCommon.Hebrew.DispASC(oAttribRef.TextString, True)
                           oHebText = New DMCommon.HebrewTrans(oAttribRef.TextString, False)
                           '	Me.txtPlanNum.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                           Me.txtPlanNum.Text = oHebText.GetWinDest(False)

                        Case msSurveyDateAttribTag
                           '	dtpSurveyDate.Text = oAttribRef.TextString
                           zzParseDate(oAttribRef.TextString, dtpSurveyDate)
                        Case msEndDateAttribTag
                           zzParseDate(oAttribRef.TextString, dtpEndDate)
                        Case msUpdateDateAttribTag
                           zzParseDate(oAttribRef.TextString, dtpUpdateDate)
                        Case msGushLegalAreaAttribTag
                           Me.txtGushLegalArea.Text = oAttribRef.TextString
                        Case msGushRegStatusNameAttribTag
                           zzParseGushStatus(oAttribRef.TextString)
                        Case msGushLastParcelNameAttribTag
                           Me.txtLastParcelName.Text = oAttribRef.TextString
                        Case msGeneralCommentNameAttribTag
                           Me.txtGeneralComment.Text = DMCommon.Hebrew.FromAcad(oAttribRef.TextString)
                     End Select
                  End If






               Catch oEx As System.Exception
                  Dim sMsg As String = String.Empty

                  System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & "Index=" & CStr(iIndex) & vbCrLf & sMsg, "AcadTransaction")
               End Try



            Next
         End If
      End If

   End Sub
   Private Sub zzParseSerialNum(sSerialNum As String)
      sSerialNum = sSerialNum.Trim()
      If sSerialNum.EndsWith(msGBDOSEnd) Then
         Dim sOut As String = sSerialNum.Substring(0, sSerialNum.Length - 4)
         Me.txtSerialNum.Text = sOut.Trim()
      End If


   End Sub
   Private Sub zzCheckLocality(sLocalityCode As String, sLocalityName As String)
      If sLocalityCode IsNot Nothing AndAlso sLocalityName IsNot Nothing Then
         If cmbLocality.SelectedValue.ToString() <> sLocalityCode Then
            MessageBox.Show(cmbLocality.SelectedValue.ToString() & vbCrLf & sLocalityCode, "Err: " & msLocalityCodeAttribTag)
         End If
      End If
   End Sub
   Private Sub zzLoadBlockRefData(bLoadData As Boolean)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)


      '	MessageBox.Show(CStr(iGenBlockRefCount) & vbCrLf & msDataBlockName, "04_925")
      zzSetInsertionPoint()
      zzLoadAttribData()

      If moGenAcadBlock.ReferenceCount = 1 Then
         Me.chkAllAttribVisible.Enabled = False
      Else
         Me.chkAllAttribVisible.Enabled = True
      End If

      mcolInvisibleAttribs = New ObjectIdCollection()
      

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub
   Private Sub zzLoadBlockRefDataOld(bLoadData As Boolean)
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Read, True)
      DMAcadExt.AcadTransaction.Start()
      DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead, False)

      Dim iGenBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(msGenBlockName, mtGenBlockRefObjID, mtGenBlockRecObjID, moaAttribDefs)
      '	MessageBox.Show(CStr(iGenBlockRefCount) & vbCrLf & msDataBlockName, "04_925")
      zzSetInsertionPoint()
      If mtGenBlockRefObjID.IsNull Then
         Me.chkAllAttribVisible.Enabled = False
      Else
         Me.chkAllAttribVisible.Enabled = True
      End If

      mcolInvisibleAttribs = New ObjectIdCollection()
      If iGenBlockRefCount = 1 Then
         zzLoadAttribData(bLoadData)
      End If

      DMAcadExt.AcadTransaction.CloseModelSpace()
      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()


   End Sub


	Private Sub chkAllAttribVisible_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkAllAttribVisible.CheckedChanged
		Dim oDBObject As Autodesk.AutoCAD.DatabaseServices.DBObject
		Dim oAttribRef As AttributeReference
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		If Me.chkAllAttribVisible.Checked Then
			For Each tAttribObjID As ObjectId In mcolInvisibleAttribs
				'	DMAcadExt.AcadDocument.WriteMessage("#191a " & tAttribObjID.ToString())
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForWrite)
				oAttribRef = DirectCast(oDBObject, AttributeReference)
				If oAttribRef.Invisible Then
					oAttribRef.Invisible = False
				End If
			Next
		Else
			For Each tAttribObjID As ObjectId In mcolInvisibleAttribs
				DMAcadExt.AcadDocument.WriteMessage("#192a " & tAttribObjID.ToString())
				oDBObject = DMAcadExt.AcadTransaction.GetDBObject(tAttribObjID, OpenMode.ForWrite)
				oAttribRef = DirectCast(oDBObject, AttributeReference)
				If Not oAttribRef.Invisible Then
					oAttribRef.Invisible = True
				End If
			Next
		End If

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub



	Private Sub cmdLoadBlockData_Click(sender As System.Object, e As System.EventArgs) Handles cmdLoadBlockData.Click
		zzLoadBlockRefData(True)
	End Sub

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		If False Then
			MessageBox.Show(CStr(DMAcadExt.AcadDocument.IsLocked), "04_807")
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()

			DMAcadExt.AcadTransaction.CreateLayer("1602")

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
		End If
		'924  464
		'	Me.Width = Me.Width + Me.Width
	End Sub




	Private Sub cmdExit_Click(sender As System.Object, e As System.EventArgs) Handles cmdExit.Click
		Me.Close()
	End Sub

	Private Function zzFilterList(ByVal sValue As String) As String
		Dim sRes As String = String.Empty
		Dim saGroups() As String = Split(sValue, ",")

		For iIndex As Integer = 0 To saGroups.GetUpperBound(0)
			saGroups(iIndex).Trim()
			If (Not saGroups(iIndex).StartsWith("[")) AndAlso (Not saGroups(iIndex).EndsWith("]")) Then
				If sRes.Length <> 0 Then
					sRes &= ","
				End If
				sRes &= saGroups(iIndex)
			End If
		Next
		Return sRes
	End Function



	Private Sub cmdParcelInvert_Click(sender As System.Object, e As System.EventArgs) Handles cmdParcelInvert.Click
		Me.txtParcels.Text = DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
	End Sub
	Private Class NoteCheck
		Const msPlaceHolderChar As String = "_"
		Const miVarPlacesMax As Integer = 5
		Const msFontStyle As String = "HEBTEXT"
		Private Shared mtTextStyleTableRecordID As ObjectId
		Private Shared mdLeft As Double
		Private Shared mdTextHeight As Double
		Private moCheckControl As CheckBox
		Private miControlIndex As Integer
		Private msText As String
		Private miPlaceHolderMin As Integer = 3
		Private msPlaceHolder As String = StrDup(miPlaceHolderMin, msPlaceHolderChar)
		Private miCurrentPosition As Integer = 1
		Private miCurrentPlace As Integer
		Dim taVarPlaces(miVarPlacesMax) As VarPlace
		Private miVarPlacesCount As Integer
		Private msaValue() As String
		Private msaAttribTag() As String

		Private mdNoteRowHeight As Double = 5.6

		Private mtGenBlockRecObjID As ObjectId
		Private mtGenBlockRefObjID As ObjectId

		Public Shared Sub InitPrint()
			'	zzInitTextSile(msFontStyle)
			mdLeft = moNoteInsPoint.X
			mdTextHeight = 2.0
		End Sub
		Public Sub New(oCheckControl As CheckBox)
			Dim sName As String = oCheckControl.Name
			miControlIndex = Convert.ToInt32(sName.Substring(7)) - 1
			moCheckControl = oCheckControl
			msText = moCheckControl.Text


			For miCurrentPlace = 0 To miVarPlacesMax
				If Not CalcPlace() Then

					miVarPlacesCount = miCurrentPlace
					Return
				End If
			Next
			miVarPlacesCount = miVarPlacesMax + 1
		End Sub
		Private Function CalcPlace() As Boolean
			Dim sChar As String
			Dim sTest As String = DMCommon.Hebrew.DispASC(msText, False)
			Dim iPos As Integer = InStr(miCurrentPosition, msText, msPlaceHolder)
			 

			If iPos > 0 Then
				taVarPlaces(miCurrentPlace).StartPosition = iPos

				For iIndex As Integer = iPos + miPlaceHolderMin To msText.Length
					sChar = Mid(msText, iIndex, 1)
					If sChar <> msPlaceHolderChar Then
						taVarPlaces(miCurrentPlace).EndPosition = iIndex - 1
						miCurrentPosition = iIndex
						Return True
					End If
				Next
				taVarPlaces(miCurrentPlace).EndPosition = msText.Length
				miCurrentPosition = msText.Length + 1
				Return True

			Else
				Return False
			End If






		End Function
		Public Sub InsertNoteBlock(ByRef mdTop As Double)
			'	Dim iGenBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(msDataBlockName, mtGenBlockRefObjID, mtGenBlockRecObjID)
			'	MessageBox.Show(CStr(iGenBlockRefCount) & vbCrLf & msDataBlockName, "04_925")

			Dim tBlockRefObjID, tBlockRecObjID As ObjectId
			'	MessageBox.Show(CStr(msBlockPath) & vbCrLf & Me.BlockName, "04_924")
			Dim oaAttribDefs() As AttributeDefinition = Nothing
			Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(Me.BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)
			Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()
			'Dim oaAttribDefs() As AttributeDefinition = Nothing
			Dim mtInsPoint As Autodesk.AutoCAD.Geometry.Point3d
			Dim bSuccess As Boolean
			Dim dTopBefore As Double = mdTop
			mdTop += mdNoteRowHeight * (RowsNumber - 1) * AcadReport.RepApp.DrawingScaleFactor
			For iIndex As Integer = 0 To Math.Min(msaAttribTag.GetUpperBound(0), msaValue.GetUpperBound(0))
				dicAttribValues.Add(msaAttribTag(iIndex), msaValue(iIndex))
			Next
			mtInsPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0.0)

			If tBlockRefObjID.IsNull Then

				If tBlockRecObjID.IsNull Then
					'	
					tBlockRecObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, Me.BlockName, oaAttribDefs)
				End If

				If Not tBlockRecObjID.IsNull Then
					Dim tNewBlockRefObjID As ObjectId = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockRecObjID, mtInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor * 0.4)
					If Not tNewBlockRefObjID.IsNull Then
						bSuccess = True
					End If

				End If
			Else
				DMAcadExt.AcadTransaction.EraseDBObject(tBlockRefObjID)
				Dim tNewBlockRefObjID As ObjectId = DMAcadExt.AcadTransaction.InsertBlockRef(tBlockRecObjID, mtInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor * 0.4)
				If Not tNewBlockRefObjID.IsNull Then
					bSuccess = True
				End If

			End If
			If False And Not tBlockRefObjID.IsNull Then
				Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tBlockRefObjID, OpenMode.ForWrite)
				If oBlockRef IsNot Nothing Then
					'MessageBox.Show(tBlockRefObjID.ToString() & vbCrLf & tBlockRecObjID.ToString(), "04_926")

					DMAcadExt.AcadTransaction.UpdateAttribText(oBlockRef, dicAttribValues)
					'''''''''''''''''''	oBlockRef.Position = mtInsPoint
					bSuccess = True
				End If
			End If
			If bSuccess = True Then
				mdTop += mdNoteRowHeight * AcadReport.RepApp.DrawingScaleFactor
			Else
				mdTop = dTopBefore
			End If

		End Sub

		Public Sub EraseNoteBlockRef()


			Dim tBlockRefObjID, tBlockRecObjID As ObjectId
			'	MessageBox.Show(CStr(msBlockPath) & vbCrLf & Me.BlockName, "04_924")
            Dim oaAttribDefs() As AttributeDefinition = Nothing
            Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(Me.BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)

			If Not tBlockRecObjID.IsNull Then
				DMAcadExt.AcadTransaction.EraseDBObject(tBlockRefObjID)
			End If

		End Sub
		Public ReadOnly Property RowsNumber As Integer
			Get
				Return (moCheckControl.Height - 4) \ 16
			End Get
		End Property
		Public ReadOnly Property BlockNameOld As String
			Get
				Return "HEAN" & CStr(miControlIndex)
			End Get
		End Property
		Public ReadOnly Property BlockName As String
			Get
				Return "UD_Note" & Format(miControlIndex + 1, "00")
			End Get
		End Property
		Public Property Checked As Boolean
			Get
				Return moCheckControl.Checked
			End Get
			Set(bValue As Boolean)
				moCheckControl.Checked = bValue
			End Set
		End Property
		Public Property NoteNo As Integer
			Get
				If String.IsNullOrEmpty(msaValue(0)) Then
					Return 0
				Else
					Return Convert.ToInt32(msaValue(0))
				End If

			End Get
			Set(iValue As Integer)
				If msaValue IsNot Nothing AndAlso msaValue.GetUpperBound(0) >= 0 Then
					msaValue(0) = Convert.ToString(iValue)
				End If

			End Set
		End Property

		Public Sub AddVarData(saValue() As String)
			msaValue = saValue
         If miControlIndex > 105 Then
            DMCommon.Functions.DispArray(msaValue, "msaValue", True)

            MessageBox.Show(zzGetFullText(msaValue), "04_984")
         End If
			moCheckControl.Text = zzGetFullText(msaValue)
			 
		End Sub
		Public Sub AddAttribTag(saValue() As String)
			msaAttribTag = saValue


		End Sub
		Private Function zzGetFullText(saValue() As String) As String '
			Dim iShift As Integer = 0
			Dim sRes As String = msText
			If miControlIndex = 15 Then
				'MessageBox.Show(CStr(msaValue.GetUpperBound(0)) & vbCrLf & sRes, "04_988a")

			End If
			For iIndex As Integer = 1 To msaValue.GetUpperBound(0)
				Dim iPrevLen As Integer = taVarPlaces(iIndex - 1).PlaceHolderLen
				If iPrevLen > 0 Then
					If msaValue(iIndex) Is Nothing Then
						taVarPlaces(iIndex - 1).Clear()
					Else
						DMCommon.Functions.Mid(sRes, taVarPlaces(iIndex - 1).StartPosition + iShift, iPrevLen, msaValue(iIndex))
						taVarPlaces(iIndex - 1).CurrentLen = msaValue(iIndex).Length
						iShift += taVarPlaces(iIndex - 1).Shift
					End If
					 
				End If
			Next
			Return sRes
		End Function
		Private Function GetAcadText() As String()
			Dim saRes(msaValue.GetUpperBound(0)) As String
			For iIndex As Integer = 0 To msaValue.GetUpperBound(0)
				saRes(iIndex) = DMCommon.Hebrew.Invert(msaValue(iIndex))
			Next
			Return saRes
		End Function
		Public Sub Clear()
			For iIndex As Integer = 0 To miVarPlacesCount - 1
				'	taVarPlaces(iIndex).Clear()
			Next

			moCheckControl.Text = msText
		End Sub
		Public Sub DrawMText(mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.MText = New Autodesk.AutoCAD.DatabaseServices.MText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Location = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			oText.Direction = New Autodesk.AutoCAD.Geometry.Vector3d(-1.0, 0.0, 0)
			oText.FlowDirection = FlowDirection.TopToBottom
			'oText.AlignChange = ""
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.Contents = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			'	oText.HorizontalMode = TextHorizontalMode.TextRight
			'	oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor * 0.1
			Try
				'	oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)



		End Sub

		Public Sub DrawTextA(mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.TextString = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			oText.HorizontalMode = TextHorizontalMode.TextRight
			oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor
			Try
				oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)



		End Sub
		Public Sub InsertBlock(ByRef mdTop As Double)
			 
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			Dim sText As String = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)


			Dim sDel As String = vbCrLf
			Dim caCharDel() As Char = sDel.ToCharArray()
			Dim saText() As String = sText.Split(caCharDel, StringSplitOptions.RemoveEmptyEntries)		'Split(sText, vbCrLf)
			MessageBox.Show(CStr(saText.GetUpperBound(0)), "04_455")
			For iIndex As Integer = 0 To saText.GetUpperBound(0)
				zzDrawLine(mdTop, saText(iIndex))
			Next

		End Sub
		Public Sub DrawTextByLine(ByRef mdTop As Double)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			Dim sText As String = DMCommon.Hebrew.WordToDOS(zzGetFullText(saValue), True)


			Dim sDel As String = vbCrLf
			Dim caCharDel() As Char = sDel.ToCharArray()
			Dim saText() As String = sText.Split(caCharDel, StringSplitOptions.RemoveEmptyEntries)		'Split(sText, vbCrLf)
			MessageBox.Show(CStr(saText.GetUpperBound(0)), "04_455")
			For iIndex As Integer = 0 To saText.GetUpperBound(0)
				zzDrawLine(mdTop, saText(iIndex))
			Next

		End Sub
		Private Sub zzDrawLine(ByRef mdTop As Double, sText As String)
			Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New Autodesk.AutoCAD.DatabaseServices.DBText()
			oText.TextStyleId = mtTextStyleTableRecordID
			oText.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft - 60, mdTop, 0)
			Dim saValue() As String = GetAcadText()  ' msaValue	'

			oText.TextString = sText
			'	DMCommon.Hebrew.InvertList(Me.txtParcels.Text)
			'System.Windows.Forms.MessageBox.Show(sTitleText, "23_080 sTitleText")
			oText.HorizontalMode = TextHorizontalMode.TextRight
			oText.VerticalMode = TextVerticalMode.TextVerticalMid
			'		MessageBox.Show(CStr(oTableCell.TextHeight) & ":" & CStr(Report.DrawingScaleFactor), "12_260 TextHeight:DrawingScaleFactor")
			oText.Height = mdTextHeight * AcadReport.RepApp.DrawingScaleFactor
			Try
				oText.AlignmentPoint = New Autodesk.AutoCAD.Geometry.Point3d(mdLeft, mdTop, 0)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "DrawNote AlignmentPoint")
			End Try
			oText.Visible = True
			DMAcadExt.AcadTransaction.AppendEntity(oText)
			mdTop -= mdNoteRowHeight * AcadReport.RepApp.DrawingScaleFactor


		End Sub

		Private Function zzGetLeftTop(ByVal dShiftY As Double) As Autodesk.AutoCAD.Geometry.Point3d
			'	Dim dX As Double = moBasePoint.X
			'	Dim dY As Double = mdCurrentY - dShiftY
			'	Return New Autodesk.AutoCAD.Geometry.Point3d(dX, dY, 0.0)
		End Function

		Private Shared Sub zzInitTextSile(ByVal sAcadFont As String)
			mtTextStyleTableRecordID = DMAcadExt.AcadTransaction.GetTextStyle(sAcadFont)	 '
		End Sub
		Private Structure VarPlace
			Public StartPosition As Integer
			Public PlaceHolderLen As Integer
			Public CurrentLen As Integer

			Public Property EndPosition As Integer
				Get
					Return StartPosition + PlaceHolderLen - 1
				End Get
				Set(iValue As Integer)
					PlaceHolderLen = iValue - StartPosition + 1
					CurrentLen = PlaceHolderLen
				End Set
			End Property
			Public Function Shift() As Integer
				Return CurrentLen - PlaceHolderLen
			End Function
			Public Sub Clear()
				CurrentLen = PlaceHolderLen
			End Sub
		End Structure
	End Class

	Private Sub cmdCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckAll.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Checked = True
		Next
	End Sub

	Private Sub cmdCheckClear_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckClear.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Checked = False
		Next
	End Sub
	Private Sub zzSetAttribTags()
		Const sNoteNo As String = "NO"
		Dim saAttribTag0() As String = {sNoteNo, "N_GHOSH"}
		moaNoteChecks(0).AddAttribTag(saAttribTag0)

		Dim saAttribTag1() As String = {sNoteNo, "N_PROJ", "DATE", "N_ELLIPSE", "N_GHOSH"}
		moaNoteChecks(1).AddAttribTag(saAttribTag1)


		Dim saAttribTag2() As String = {sNoteNo, "N_GHOSH", "N_PROJ", "DATE", "N_ELLIPSE"}
		moaNoteChecks(2).AddAttribTag(saAttribTag2)


		Dim saAttribTag3() As String = {sNoteNo, "BORDER", "N_GHOSH"}
		moaNoteChecks(3).AddAttribTag(saAttribTag3)


		Dim saAttribTag4() As String = {sNoteNo}
		moaNoteChecks(4).AddAttribTag(saAttribTag4)


		Dim saAttribTag5() As String = {sNoteNo}
		moaNoteChecks(5).AddAttribTag(saAttribTag5)

		Dim saAttribTag6() As String = {sNoteNo}
		moaNoteChecks(6).AddAttribTag(saAttribTag6)

		Dim saAttribTag7() As String = {sNoteNo}
		moaNoteChecks(7).AddAttribTag(saAttribTag7)

		Dim saAttribTag8() As String = {sNoteNo}
		moaNoteChecks(8).AddAttribTag(saAttribTag8)

		Dim saAttribTag9() As String = {sNoteNo}
		moaNoteChecks(9).AddAttribTag(saAttribTag9)

		Dim saAttribTag10() As String = {sNoteNo}
		moaNoteChecks(10).AddAttribTag(saAttribTag10)

		Dim saAttribTag11() As String = {sNoteNo}
		moaNoteChecks(11).AddAttribTag(saAttribTag11)

		Dim saAttribTag12() As String = {sNoteNo}
		moaNoteChecks(12).AddAttribTag(saAttribTag12)

		Dim saAttribTag13() As String = {sNoteNo}
		moaNoteChecks(13).AddAttribTag(saAttribTag13)

		Dim saAttribTag14() As String = {sNoteNo, "N_TABA"}
		moaNoteChecks(14).AddAttribTag(saAttribTag14)

		Dim saAttribTag15() As String = {sNoteNo, "N_TASHAZ"}
		moaNoteChecks(15).AddAttribTag(saAttribTag15)

		Dim saAttribTag16() As String = {sNoteNo, "N_JOB"}
		moaNoteChecks(16).AddAttribTag(saAttribTag16)

	End Sub
   Private Sub cmdPreview_Click(sender As System.Object, e As System.EventArgs) Handles cmdPreview.Click

      zzFillData()


   End Sub
	Private Sub zzFillData()
      Dim saValue0() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text)}
		moaNoteChecks(0).AddVarData(saValue0)

		Dim saValue1() As String = {String.Empty, zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		moaNoteChecks(1).AddVarData(saValue1)


      Dim saValue2() As String = {String.Empty, zzTextToNothing(Me.txtGush.Text), zzTextToNothing(Me.txtBaseSerialNumber.Text), zzDatePickerToNothing(Me.dtpBaseApproveDate), zzTextToNothing(Me.txtBaseProcessName.Text), zzTextToNothing(Me.txtBaseGush.Text)}
		moaNoteChecks(2).AddVarData(saValue2)


      Dim saValue3() As String = {String.Empty, zzTextToNothing(zzFilterList(Me.txtParcels.Text)), zzTextToNothing(Me.txtGush.Text)}
		moaNoteChecks(3).AddVarData(saValue3)



		Dim saValue4() As String = {String.Empty}
		moaNoteChecks(4).AddVarData(saValue4)
		Dim saValue5() As String = {String.Empty}
		moaNoteChecks(5).AddVarData(saValue5)
		Dim saValue6() As String = {String.Empty}
		moaNoteChecks(6).AddVarData(saValue6)
		Dim saValue7() As String = {String.Empty}
		moaNoteChecks(7).AddVarData(saValue7)
		Dim saValue8() As String = {String.Empty}
		moaNoteChecks(8).AddVarData(saValue8)
		Dim saValue9() As String = {String.Empty}
		moaNoteChecks(9).AddVarData(saValue9)
		Dim saValue10() As String = {String.Empty}
		moaNoteChecks(10).AddVarData(saValue10)
		Dim saValue11() As String = {String.Empty}
		moaNoteChecks(11).AddVarData(saValue11)
		Dim saValue12() As String = {String.Empty}
		moaNoteChecks(12).AddVarData(saValue12)
		Dim saValue13() As String = {String.Empty}
		moaNoteChecks(13).AddVarData(saValue13)
		Dim saValue14() As String = {String.Empty, zzTextToNothing(Me.txtPlanNum.Text)}
		moaNoteChecks(14).AddVarData(saValue14)


		Dim saValue15() As String = {String.Empty, zzTextToNothing(Me.txtPlanPHNum.Text)}
		moaNoteChecks(15).AddVarData(saValue15)


      Dim saValue16() As String = {String.Empty, zzTextToNothing(Me.txtTaskNo.Text)}
      moaNoteChecks(16).AddVarData(saValue16)

      Dim saValue17() As String = {String.Empty, zzTextToNothing(Me.txtProjectNo.Text)}
      moaNoteChecks(17).AddVarData(saValue17)

	End Sub
	Private Sub cmdUndo_Click(sender As System.Object, e As System.EventArgs) Handles cmdUndo.Click
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			moaNoteChecks(iIndex).Clear()
		Next
	End Sub
	Private Function zzTextToNothing(sText As String) As String
		If String.IsNullOrEmpty(sText) Then
			Return Nothing
		Else
			Return sText
		End If
	End Function
	Private Function zzDatePickerToNothing(dtpControl As System.Windows.Forms.DateTimePicker) As String
		If dtpControl.Checked Then
			Return dtpControl.Text
		Else
			Return Nothing
		End If
	End Function
	Private Sub zzInsertNote(iIndex As Integer)
		Dim oaAttribDefs() As AttributeDefinition = Nothing
		Dim tBlockRefObjID, tBlockRecObjID As ObjectId
		Dim iBlockRefCount As Integer = DMAcadExt.AcadTransaction.GetSingleBlockRef(moaNoteChecks(iIndex).BlockName, tBlockRefObjID, tBlockRecObjID, oaAttribDefs)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)()

		If tBlockRefObjID.IsNull Then
			If tBlockRecObjID.IsNull Then
				tBlockRecObjID = DMAcadExt.AcadTransaction.OpenBlockDB(msBlockPath, moaNoteChecks(iIndex).BlockName, oaAttribDefs)
			End If
			mtGenBlockRefObjID = DMAcadExt.AcadTransaction.InsertBlockRef(mtGenBlockRecObjID, mtGenInsPoint, oaAttribDefs, dicAttribValues, AcadReport.RepApp.DrawingScaleFactor)
		Else
			Dim oGenBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(mtGenBlockRefObjID, OpenMode.ForWrite)
			If oGenBlockRef IsNot Nothing Then
				DMAcadExt.AcadTransaction.UpdateAttribText(oGenBlockRef, dicAttribValues)
			End If
		End If
	End Sub

	Private Sub cmdInsertNotes_Click(sender As System.Object, e As System.EventArgs) Handles cmdInsertNotes.Click
		zzFillData()
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)

		NoteCheck.InitPrint()

		Dim dTop As Double = moNoteInsPoint.Y

		Dim iNo As Integer = 0
		Dim iRowsCount As Integer = 0
		For iIndex As Integer = 0 To moaNoteChecks.GetUpperBound(0)
			If moaNoteChecks(iIndex).Checked Then
				iNo += 1
				moaNoteChecks(iIndex).NoteNo = iNo

			End If
		Next
		For iIndex As Integer = moaNoteChecks.GetUpperBound(0) To 0 Step -1
			If moaNoteChecks(iIndex).Checked Then
				moaNoteChecks(iIndex).InsertNoteBlock(dTop)
			Else
				moaNoteChecks(iIndex).EraseNoteBlockRef()
			End If
		Next
		If moaNoteChecks.GetUpperBound(0) > 0 Then
			zzSetCaption(dTop)
		End If

		'	DMAcadExt.AcadDocument.WriteMessage("09:" & moNoteInsPoint.Coordinates2d)
		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.UpdateScreen()

	End Sub
	Private Sub zzSetCaption(ByRef mdTop As Double)
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("UD_NoteCaption", msBlockPath)
		oAcadBlock.OpenForRight()

		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()

		tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(moNoteInsPoint.X, mdTop, 0.0)
		tBlockRefData.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(AcadReport.RepApp.DrawingScaleFactor * 0.4)

		'	MessageBox.Show(tBlockRefData.Position.ToString(), "01_325a")
		oAcadBlock.InsertRef(tBlockRefData)
	End Sub
	Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
		Dim sComText As String = "SELECT * FROM Journal"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
		Dim iBlock As Short
		Dim s As String = String.Empty
		If oDataReader IsNot Nothing Then


			'	MessageBox.Show("", "01_518a")


			Do While oDataReader.Read
				iBlock = oDataReader.GetInt16(0)
				s &= CStr(iBlock)

			Loop
			MessageBox.Show(s, "01_519a")
		End If
	End Sub

	
   
   Private Sub chkNote01_CheckedChanged(sender As Object, e As EventArgs) Handles chkNote01.CheckedChanged

   End Sub

   Private Sub rdbNetNew_CheckedChanged(sender As Object, e As EventArgs) Handles rdbNetNew.CheckedChanged

   End Sub
End Class