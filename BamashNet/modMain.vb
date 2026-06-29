Option Explicit On
Option Strict On
'Imports Autodesk.AutoCAD.Interop
'Imports Autodesk.AutoCAD.Interop.Common
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Colors
Imports Excel
Imports Excel.Constants
Module modMain
	Private Const msAutocadClassName As String = "AutoCAD.Application.16"
	Public ThisDrawing As AcadDocument
	Const sBlockFolder As String = "\\Zeus\dm_app\dm_work\blocks\FrameTempl"
	Public goaPrintEnt() As Entity

	Public giPrintEntCount As Integer
	Public moAutocadApp As AcadApplication



	


	Public Sub TestInit()
	End Sub


	Public Function GetApartDescr(ByVal lPolygonID As Long, ByVal sTag As String, ByVal sValue As String) As String
		Dim sMsg As String
		Dim lValue As Long
		On Error Resume Next
		lValue = CLng(sValue)
		If Err.Number <> 0& Then
			zzGetAttribErr(lPolygonID, sTag, sValue)
			Return Nothing
		Else
			Select Case lValue
				Case 1
					Return "דירה"
				Case 2
					Return "חנות"
				Case 3
					Return "אולם"
				Case 4
					Return "משרד"
				Case 5
					Return "מחסן"
				Case 6
					Return "חניה"
				Case 7
					Return "גג"
				Case 8
					Return "קרקע"
				Case 9
					Return "קרקע לרבות הקרקע מתחת לדירה וכל הבנוי עליה"
					' 11-01-2002 Boris
					'      return "קרקע וכל הבנוי מעליה לרבות הקרקע מתחת למבנה"
				Case 10
					Return "סככה"
				Case 11
					Return "דירת קוטג'"
				Case 12
					Return "מרפסת לא מקורה"
				Case 13
					Return "מרפסת לא מקורה"

				Case 99
					Return String.Empty
				Case Else
					zzGetAttribErr(lPolygonID, sTag, sValue)
					Return "שגיאה"
			End Select
		End If
	End Function


	Public Sub MsgView()
		'  Dim fMsg As frmMsg
		gfrmMsg.Show()
	End Sub
	Public Sub Parameters()
		gfrmParam = New frmParam
		gfrmParam.Show()
	End Sub

	Public Sub InputData()

		Dim lResp As Long
		Dim oBamash As bmBamash
		zzInitAcadApp()
		zzResetGlobal()
		zzEraseTable()
		oBamash = New bmBamash

		oBamash.InitMemory()
		'  oBamash.InitFile sMDBName
		'  oBamash.TestTable
		lResp = oBamash.OpenTopo
		If lResp = 0& Then
			oBamash.FloorPropEnum()
			oBamash.SubShareEnum()
			oBamash.UpdateDraw()
		End If
		'  InsertTable
		oBamash = Nothing
	End Sub


	Public Sub InsertTable()
		Const sMainHeaderBlock As String = "drws022"
		Const sMainRowBlock As String = "drws023"
		Const sMainRowAddBlock As String = "drws032"
		Const sShareHeaderBlock As String = "drws024"
		Const sShareRowBlock As String = "drws025"
		Const sSumHeaderBlock As String = "drws028"
		Const sExproBlock As String = "drws053"
		Const sSumRowBlock As String = "drws029"
		Const sTotalBlock As String = "drws030"
		Const sDeclareBlock As String = "drws026"
		Const sCaptionBlock As String = "DRWS027"
		'  zzPaintPolygon = goAcComI.PaintSinglePolygon(lLowHandle, lHighHandle, sFileName, True, vvaBodyParms, vdaBorders, vdaZebras, lHatchID, lBoundID)
		Dim lResp As Long
		Dim vLowerLeftPoint As TPlnPoint, vLowerRightPoint As TPlnPoint

		Dim iRowIndex As Long
		Dim oAcadBlockReference As IAcadBlockReference
		Dim lIndex As Long
		'	Dim vAttribRefs
		Dim oAttribRef As IAcadAttributeReference
		Dim oAcadLine As IAcadLine
		Dim lCurrPropID As Long
		Dim oDrawZebra As DrawZebra
		Dim lPolygonID As Long
		Dim sPropID As String
		Dim vlaPropID() As Integer
		Dim vlaColors() As ACAD_COLOR
		If grsPrint Is Nothing Then Exit Sub
		If grsPrint.State = ADODB.ObjectStateEnum.adStateClosed Then Exit Sub

		lResp = GetCaptionBlock(sCaptionBlock, vLowerLeftPoint, vLowerRightPoint)
		If lResp <> 0 Then Exit Sub
		SetActiveLayer(gsPrmLayerTable)
		If gdSumExproArea > 0.0# Then
			oAcadBlockReference = bmACADNet.InsertBlock(sExproBlock, vLowerLeftPoint, vLowerRightPoint, gdPrmInterval * gdTableYScale)
			zzUpdateExproAttr(oAcadBlockReference)
		End If
		oAcadBlockReference = bmACADNet.InsertBlock(sSumHeaderBlock, vLowerLeftPoint, vLowerRightPoint, gdPrmInterval * gdTableYScale)

		mrsTotal.MoveFirst()
		Do Until mrsTotal.EOF
			oAcadBlockReference = zzInsertBlock(sSumRowBlock, vLowerLeftPoint, vLowerRightPoint, 0.0#)
			If Not oAcadBlockReference Is Nothing Then zzUpdateSumRowAttr(oAcadBlockReference)
			mrsTotal.MoveNext()
		Loop

		oAcadBlockReference = bmACADNet.InsertBlock(sTotalBlock, vLowerLeftPoint, vLowerRightPoint, 0.0#)
		If Not oAcadBlockReference Is Nothing Then
			zzUpdateTotalAttr(oAcadBlockReference)
		End If
		oAcadBlockReference = bmACADNet.InsertBlock(sMainHeaderBlock, vLowerLeftPoint, vLowerRightPoint, gdPrmInterval * gdTableYScale)
		If oAcadBlockReference Is Nothing Then Exit Sub
		''''   zzUpdateMainHeaderAttr oAcadBlockReference
		grsPrint.MoveFirst()
		lCurrPropID = CIntN(grsPrint.Fields("PropID").Value)
		Dim sInsertBlock As String
		Do
			If CIntN(grsPrint.Fields("ColorID").Value) = 0& Then
				sInsertBlock = sMainRowAddBlock
			Else
				sInsertBlock = sMainRowBlock
			End If
			oAcadBlockReference = bmACADNet.InsertBlock(sInsertBlock, vLowerLeftPoint, vLowerRightPoint, 0.0#)
			If Not oAcadBlockReference Is Nothing Then
				If Not IsDBNull(grsPrint.Fields("ColorID").Value) Then oAcadBlockReference.color = CType(CIntN(grsPrint.Fields("ColorID").Value), ACAD_COLOR)
				zzUpdateMainRowAttr(oAcadBlockReference)
			End If
			grsPrint.MoveNext()
			If grsPrint.EOF Then
				oAcadLine = ThisDrawing.ModelSpace.AddLine(vLowerLeftPoint, vLowerRightPoint)
				AddTableEntity(CType(oAcadLine, AcadEntity))

				oAcadBlockReference = bmACADNet.InsertBlock(sMainRowAddBlock, vLowerLeftPoint, vLowerRightPoint, 0.0#)
				If Not oAcadBlockReference Is Nothing Then
					zzUpdateMainFooterAttr(oAcadBlockReference)
				End If
				oAcadLine = ThisDrawing.ModelSpace.AddLine(vLowerLeftPoint, vLowerRightPoint)
				AddTableEntity(CType(oAcadLine, AcadEntity))
				oAcadLine.Lineweight = ACAD_LWEIGHT.acLnWt025
				Exit Do
			Else
				If lCurrPropID <> DirectCast(grsPrint.Fields("PropID").Value, Long) Then
					lCurrPropID = DirectCast(grsPrint.Fields("PropID").Value, Long)
					oAcadLine = ThisDrawing.ModelSpace.AddLine(vLowerLeftPoint, vLowerRightPoint)
					AddTableEntity(CType(oAcadLine, AcadEntity))
				End If
			End If
		Loop

		'   grsPrint.Close
		'   Set grsPrint = Nothing
		If mrsShare.RecordCount > 0 Then
			oAcadBlockReference = bmACADNet.InsertBlock(sShareHeaderBlock, vLowerLeftPoint, vLowerRightPoint, gdPrmInterval * gdTableYScale)
			mrsShare.MoveFirst()
			oDrawZebra = New DrawZebra
			oDrawZebra.BaseHeight = 6.5 * gdTableYScale
			oDrawZebra.BaseWidth = 27.5 * gdTableYScale

			Do
				oAcadBlockReference = bmACADNet.InsertBlock(sShareRowBlock, vLowerLeftPoint, vLowerRightPoint, 0.0#)
				zzUpdateShareRowAttr(oAcadBlockReference)
				oDrawZebra.SetBasePoint(vLowerRightPoint.X - 73.2 * gdTableXScale, vLowerRightPoint.Y + 1 * gdTableXScale)
				sPropID = DirectCast(mrsShare.Fields("PropID").Value, String)
				lPolygonID = DirectCast(mrsShare.Fields("ID").Value, Long)
				vlaPropID = SplitToInt(sPropID, lPolygonID)
				vlaColors = zzGetZebraColors(vlaPropID)
				oDrawZebra.Paint(vlaColors)

				mrsShare.MoveNext()
			Loop Until mrsShare.EOF
			oAcadLine = ThisDrawing.ModelSpace.AddLine(vLowerLeftPoint, vLowerRightPoint)
			''  ReDim Preserve goaPrintEnt(giPrintEntCount) As AutoCAD.AcadEntity
			''   Set goaPrintEnt(giPrintEntCount) = oAcadLine
			''   giPrintEntCount = giPrintEntCount + 1
			AddTableEntity(CType(oAcadLine, Entity))
		End If
		oAcadBlockReference = zzInsertBlock(sDeclareBlock, vLowerLeftPoint, vLowerRightPoint, gdPrmInterval * gdTableYScale)
		If Not oAcadBlockReference Is Nothing Then zzUpdateDeclareAttr(oAcadBlockReference)
		bmACADNet.Regen()
	End Sub

	Private Function zzInsertMainHeaderI(ByRef vLowerRightPoint As TPlnPoint) As Long
		Const dInitScale As Double = 1.0#
		Const sMainHeaderBlock As String = "drws022.dwg"
		'Const sMainHeaderBlock = "f:\dw_work\blocks\builds\blds015.dwg"
		Dim oAcadBlockReference As IAcadBlockReference
		Dim sCommand As String
		Dim lResp As Long

		Dim vMinPoint, vMaxPoint As System.Object

		Dim vUpperRightPoint As TPlnPoint
		Dim vLowerLeftPoint As TPlnPoint



		Dim sBlockPath As String
		On Error Resume Next

		ThisDrawing.SetVariable("INSNAME", sMainHeaderBlock)
		ThisDrawing.SetVariable("DRAGMODE", 2)
		sBlockPath = sBlockFolder & "\" & sMainHeaderBlock & ".dwg"
		sCommand = "-insert " & sBlockPath & vbCr & " s 1 r 0 ps " & CStr(dInitScale) & " "		 '& Chr(13) '& Chr(13)
		ThisDrawing.SendCommand(sCommand)
		lResp = zzGetBlockRef(sMainHeaderBlock, oAcadBlockReference)
		If lResp = 0& Then
			oAcadBlockReference.GetBoundingBox(vMinPoint, vMaxPoint)

			If IsArray(vMinPoint) And IsArray(vMaxPoint) Then


				vLowerLeftPoint = New TPlnPoint(vMinPoint)
				vUpperRightPoint = New TPlnPoint(vMaxPoint)
				vLowerRightPoint = zzGetLowerRightPoint(vLowerLeftPoint, vUpperRightPoint)
				vLowerRightPoint = zzGetLowerRightPoint(vLowerLeftPoint, vUpperRightPoint)


			Else
				zzInsertMainHeaderI = 300067
			End If

		Else
			zzInsertMainHeaderI = lResp
		End If

	End Function

	Private Function zzInsertBlock(ByVal sBlockName As String, ByRef vLowerLeftPoint As TPlnPoint _
	, ByRef vLowerRightPoint As TPlnPoint, ByVal dYShift As Double) As BlockReference
		Const dInitScale As Double = 1.0#
		Dim oAcadBlockReference As BlockReference
		Dim lResp As Long
		Dim vMinPoint As System.Object = Nothing
		Dim vMaxPoint As System.Object = Nothing


		Dim vUpperRightPoint As TPlnPoint

		Dim sBlockPath As String
		Dim sMsg As String
		On Error Resume Next
		sBlockPath = sBlockFolder & "\" & sBlockName & ".dwg"
		vLowerRightPoint.Y = vLowerRightPoint.Y - dYShift
		oAcadBlockReference = ThisDrawing.ModelSpace.InsertBlock(vLowerRightPoint, sBlockPath, gdTableXScale, gdTableYScale, 1.0#, 0.0#)

		If Err.Number = 0& Then
			oAcadBlockReference.GetBoundingBox(vMinPoint, vMaxPoint)
			If IsArray(vMinPoint) And IsArray(vMaxPoint) Then
				vLowerLeftPoint = New TPlnPoint(vMinPoint)
				vUpperRightPoint = New TPlnPoint(vMaxPoint)
				vLowerRightPoint = zzGetLowerRightPoint(vLowerLeftPoint, vUpperRightPoint)
				zzInsertBlock = oAcadBlockReference
				AddTableEntity(CType(oAcadBlockReference, AcadEntity))

			Else
				sMsg = "Operation 'InsertBlock " & sBlockName & "'  is failed"
				gfrmMsg.AddText(sMsg)
			End If
		Else
			sMsg = "Operation 'InsertBlock " & sBlockName & "'  is failed"
			gfrmMsg.AddText(sMsg)
		End If
	End Function
	Private Function zzInsertMainRow(ByVal vLowerLeftPoint As TPlnPoint, ByVal vLowerRightPoint As TPlnPoint) As IAcadBlockReference
		Const sMainRowBlock As String = "BLDS016A"
		'Const sMainHeaderBlock = "f:\dw_work\blocks\builds\blds015.dwg"
		Dim oAcadBlockReference As IAcadBlockReference
		Dim sCommand As String
		Dim lResp As Long
		Dim vMinPoint, vMaxPoint As System.Object

		Dim vUpperRightPoint As TPlnPoint

		Dim sBlockPath As String
		On Error Resume Next

		sBlockPath = sBlockFolder & "\" & sMainRowBlock & ".dwg"
		sBlockPath = "d:\" & sMainRowBlock & ".dwg"
		oAcadBlockReference = ThisDrawing.ModelSpace.InsertBlock(vLowerRightPoint, sBlockPath, gdTableXScale, gdTableYScale, 1.0#, 0.0#)

		If Err.Number = 0& Then
			oAcadBlockReference.GetBoundingBox(vMinPoint, vMaxPoint)
			If IsArray(vMinPoint) And IsArray(vMaxPoint) Then
				vLowerLeftPoint = New TPlnPoint(vMinPoint)
				vUpperRightPoint = New TPlnPoint(vMaxPoint)
				vLowerRightPoint = zzGetLowerRightPoint(vLowerLeftPoint, vUpperRightPoint)
				zzInsertMainRow = oAcadBlockReference
			Else
				zzInsertMainRow = Nothing
			End If

		Else
			zzInsertMainRow = Nothing
		End If


	End Function

	Private Function zzGetBlockRef(ByVal sName As String, ByVal oAcadBlockReference As IAcadBlockReference) As Long
		Dim lIndex As Long
		Dim oAcadEntity As IAcadEntity
		Dim lResp As Long
		Dim sMsg As String
		Const lBlockRefNotFound As Long = 20005&
		On Error Resume Next

		lResp = lBlockRefNotFound
		For lIndex = 0& To ThisDrawing.ModelSpace.Count - 1&
			oAcadEntity = ThisDrawing.ModelSpace.Item(lIndex)

			If oAcadEntity.ObjectName = "AcDbBlockReference" Then
				oAcadBlockReference = DirectCast(oAcadEntity, IAcadBlockReference)
				If StrComp(oAcadBlockReference.Name, sName, CompareMethod.Text) = 0 Then
					lResp = 0&
					Exit For
				End If
			End If
		Next
		If lResp = lBlockRefNotFound Then
			sMsg = "Block Reference " & sName & " is not found"
			gfrmMsg.AddText(sMsg)
		End If
		zzGetBlockRef = lResp
	End Function



	Private Function zzGetLowerRightPoint(ByVal vMinPoint As TPlnPoint, ByVal vMaxPoint As TPlnPoint) As TPlnPoint
		Return New TPlnPoint(vMaxPoint.X, vMinPoint.Y)

	End Function


	Private Sub zzUpdateMainRowAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lSystemID As Long, lObjectID As Long, lParentID As Long
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		Dim sNewValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "BLDNO"
					oAttribRef.TextString = CStrN(grsPrint.Fields("BldNo").Value)
				Case "BLDPART"
					oAttribRef.TextString = CStrN(grsPrint.Fields("Part"))
				Case "BLDENTR"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("EntranceDescr")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "BLDFLOOR"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("FloorDescr")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "APRTDESC"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("ApartDescr")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "SUBPARCELNO"
					oAttribRef.TextString = CStrN(grsPrint.Fields("MainCaption"))
				Case "AREAAPRT"
					oAttribRef.TextString = CStrNSpace(grsPrint.Fields("MainArea"))
				Case "AREASTORAGE"
					oAttribRef.TextString = CStrNSpace(grsPrint.Fields("WarehouseArea"))
				Case "AREASUM"
					oAttribRef.TextString = CStrNSpace(grsPrint.Fields("SumArea"))
				Case "ATTACHDESC"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("SubDescr")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "ATTACHMARK"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("SubCaption")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "ATTACHAREA"
					oAttribRef.TextString = CStrNSpace(grsPrint.Fields("Area"))
				Case "POLYGONCOLOR"
					sNewValue = ToDOS(CStrN(grsPrint.Fields("Color")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
					oAttribRef.Invisible = False

			End Select

		Next
		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash" & "-zzUpdateAttributes")
				Resume ExitProcedure
		End Select
	End Sub

	Private Sub zzUpdateSumRowAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lSystemID As Long, lObjectID As Long, lParentID As Long
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		Dim sNewValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			' If oAttribRef.Invisible Then Stop
			Select Case sCurrentTag
				Case "BLDNO"
					If DirectCast(mrsTotal.Fields("BldNo").Value, Integer) <> 0 Then oAttribRef.TextString = CStrN(mrsTotal.Fields("BldNo").Value)
				Case "BLDPART"
					If CIntN(mrsTotal.Fields("BldPart").Value) <> 0 Then oAttribRef.TextString = CStrN(mrsTotal.Fields("BldPart").Value)
				Case "BLDENTR"
					If CIntN(mrsTotal.Fields("BldEntrance").Value) <> 0 Then oAttribRef.TextString = ToDOS(GetHebNum(CIntN(mrsTotal.Fields("BldEntrance").Value)))
				Case "BLDFLOOR"
					sNewValue = ToDOS(CStrN(mrsTotal.Fields("BldFloor")))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "BLDAPRT"
					oAttribRef.TextString = CStrN(mrsTotal.Fields("PropCount").Value)

			End Select

		Next
		' If mlBldEntrance <> 0& Then grsPrint!EntranceDescr = GetHebNum(mlBldEntrance)
		' If mlBldPart <> 0& Then grsPrint!Part = mlBldPart
		' If mlBldNo <> 0& Then grsPrint!BldNo = mlBldNo

		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "modMain" & "-zzUpdateAttributes")
				Resume ExitProcedure
		End Select
	End Sub



	Private Sub zzUpdateMainHeaderAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference = Nothing
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "PARCELNO"
					oAttribRef.TextString = gsInpParcel & "/0"
			End Select

		Next
		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "modMain" & "-zzUpdateAttributes")
				Resume ExitProcedure
		End Select
	End Sub


	Private Sub zzUpdateMainFooterAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference = Nothing
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)

		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "SUBPARCELNO"
					oAttribRef.TextString = gsInpParcel & "/0"
				Case "APRTDESC"
					'  oAttribRef.TextString = ToDOS("ףתושמ שוכר")
					oAttribRef.TextString = ToDOS("רכוש משותף")
			End Select

		Next
		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "modMain" & "-zzUpdateAttributes")
				Resume ExitProcedure
		End Select
	End Sub

	Private Sub zzUpdateDeclareAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "NAME"
					oAttribRef.TextString = "Name"
				Case "STREET"
					If Len(gsInpStreet) <> 0& Then oAttribRef.TextString = gsInpStreet
				Case "BLDNUM"
					If Len(gsInpBldNum) <> 0& Then oAttribRef.TextString = gsInpBldNum
				Case "CITY"
					If Len(gsInpCity) <> 0 Then oAttribRef.TextString = gsInpCity
				Case "PARCELNO"
					If Len(gsInpParcel) <> 0 Then oAttribRef.TextString = gsInpParcel
				Case "BLOCKNO"
					If Len(gsInpBlock) <> 0 Then oAttribRef.TextString = gsInpBlock
			End Select

		Next

		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash")
				Resume ExitProcedure
		End Select
	End Sub
	Private Sub zzUpdateTotalAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference = Nothing
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "FLOORNUM"
					oAttribRef.TextString = CStr(glTotalProp)
				Case "PROPNUM"
					oAttribRef.TextString = CStr(glTotalProp)
			End Select

		Next
		oAttribRef.Update()
ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash" & "-zzUpdateTotalAttr")
				Resume ExitProcedure
		End Select
	End Sub

	Private Sub zzUpdateShareRowAttr(ByVal oAcadBlockReference As IAcadBlockReference)
		Dim lResp As Long
		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference
		Dim sBlock As String, sParcel As String, sPlan As String, sLot As String
		Dim sAttributeValue As String
		Dim sPropID As String
		Dim lPolygonID As Long
		Dim vlaPropID() As Integer
		Dim vlaColors() As ACAD_COLOR
		Dim sNewValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "ATTACHDESC"
					sNewValue = ToDOS(CStrN(mrsShare.Fields("ApartDescr").Value))
					If Len(sNewValue) <> 0& Then oAttribRef.TextString = sNewValue
				Case "ATTACHMARK"
					oAttribRef.TextString = ToDOS(CStrN(mrsShare.Fields("Caption").Value))
				Case "POLYGONCOLOR"
					sPropID = CStrN(mrsShare.Fields("PropID").Value)
					' lTopoID = mrsShare!TopoID
					lPolygonID = CIntN(mrsShare.Fields("ID").Value)
					vlaPropID = SplitToInt(sPropID, lPolygonID)
					vlaColors = zzGetZebraColors(vlaPropID)
					oAttribRef.TextString = ToDOS(zzGetColorStr(vlaColors))
				Case "ATTACHAREA"
					oAttribRef.TextString = Strings.Format(mrsShare.Fields("PolygonArea").Value, gsAreaFmt)
				Case "ATTACHEQAL"
					oAttribRef.TextString = CStrN(mrsShare.Fields("PropID").Value)
			End Select
			oAttribRef.Update()
		Next

ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash")
				Resume ExitProcedure
		End Select
	End Sub

	Private Sub zzUpdateExproAttr(ByVal oAcadBlockReference As IAcadBlockReference)

		Dim vAttribRefs As System.Array
		Dim lAttribIndex As Long
		Dim sCurrentTag As String
		Dim oAttribRef As IAcadAttributeReference


		Dim sPropID As String
		Dim lPolygonID As Long
		Dim vlaPropID() As Integer

		Dim sNewValue As String
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "AREA_PARCEL"
					oAttribRef.TextString = Strings.Format(gdParcelArea, gsAreaFmt)
				Case "AREA_BEF_EXP"
					oAttribRef.TextString = Strings.Format(gdSumExproArea, gsAreaFmt)
				Case "AREA_AFTER_EXP"
					oAttribRef.TextString = Strings.Format(gdParcelArea - gdSumExproArea, gsAreaFmt)
			End Select
			oAttribRef.Update()
		Next

ExitProcedure:
		Exit Sub
ErrorHandler:
		Select Case Err.Number
			Case Else
				MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash")
				Resume ExitProcedure
		End Select
	End Sub





	Private Sub zzInitAcadApp()
		moAutocadApp = CType(GetObject(, msAutocadClassName), AcadApplication)
		ThisDrawing = moAutocadApp.ActiveDocument
	End Sub

	Private Function zzGetUserBlock(ByVal oAcadBlockReference As IAcadBlockReference) As Long
		Dim oAttribRef As IAcadAttributeReference
		Dim vAttribRefs As System.Array
		Dim sCurrentTag As String, sAttributeValue As String
		Dim lAttribIndex As Long
		On Error GoTo ErrorHandler
		vAttribRefs = DirectCast(oAcadBlockReference.GetAttributes(), System.Array)
		For lAttribIndex = 0& To UBound(vAttribRefs)
			oAttribRef = DirectCast(vAttribRefs.GetValue(lAttribIndex), IAcadAttributeReference)
			sCurrentTag = oAttribRef.TagString
			Select Case sCurrentTag
				Case "BLOCKNO"
					gsInpBlock = oAttribRef.TextString
				Case "PARCELNO"
					gsInpParcel = oAttribRef.TextString
				Case "TOTALAREA"
					' oAttribRef.TextString = Format(gdTotalArea, gsAreaFmt)
					gdParcelArea = Val(oAttribRef.TextString)
				Case "CITY"
					gsInpCity = oAttribRef.TextString
				Case "STREET"
					gsInpStreet = oAttribRef.TextString
				Case "BLDNUM"
					gsInpBldNum = oAttribRef.TextString
			End Select
		Next
ExitProcedure:
		Exit Function
ErrorHandler:
		MsgBox("Status=" & CStr(Err.Number) & vbCrLf & Err.Description, vbCritical, "Bamash")
		Resume ExitProcedure

	End Function
	Public Function GetCaptionBlock(ByVal sCaptionBlock As String, ByRef vLowerLeftPoint As TPlnPoint, ByRef vLowerRightPoint As TPlnPoint) As Long
		Dim lResp As Long
		Dim oBlockRef As IAcadBlockReference
		Dim vMinPoint, vMaxPoint As System.Object
		Dim vBasePoint, dScaleFactor As Double
		Dim vUpperRightPoint As TPlnPoint

		lResp = zzGetBlockRef(sCaptionBlock, oBlockRef)
		If lResp = 0& Then
			lResp = zzGetUserBlock(oBlockRef)
			If lResp = 0& Then
				oBlockRef.GetBoundingBox(vMinPoint, vMaxPoint)
				'   oBlockRef.ScaleEntity vBasePoint, dScaleFactor
				If IsArray(vMinPoint) And IsArray(vMaxPoint) Then
					vLowerLeftPoint = New TPlnPoint(vMinPoint)
					vUpperRightPoint = New TPlnPoint(vMaxPoint)
					vLowerRightPoint = zzGetLowerRightPoint(vLowerLeftPoint, vUpperRightPoint)
				Else
					GetCaptionBlock = 601&
				End If
				gdTableXScale = oBlockRef.XScaleFactor
				gdTableYScale = oBlockRef.YScaleFactor
			Else
				GetCaptionBlock = 602&
			End If
		Else
			GetCaptionBlock = 603&
		End If
	End Function

	Public Sub TestInter()
		Dim daStart(2) As Double, daEnd(2) As Double
		Dim daBase(2) As Double
		Dim vVertex1(7) As Double
		Dim vVertex2(3) As Double
		Dim vVertex3(11) As Double
		Dim oAcadMLine As AcadMLine
		Dim oAcadLine As AcadLine
		Dim oAcadLWPolyline1 As AcadLWPolyline
		Dim oAcadLWPolyline2 As AcadLWPolyline
		Dim oAcadPolyline As AcadPolyline
		Dim vRetVal As System.Object
		daStart(0) = 0.5
		daStart(1) = 1
		daEnd(0) = 2.5
		daEnd(1) = 2
		daBase(0) = 4 : daBase(1) = 7
		vVertex3(0) = 4 : vVertex3(1) = 7 : vVertex3(2) = 0
		vVertex3(3) = 5 : vVertex3(4) = 7 : vVertex3(5) = 0
		vVertex3(6) = 6 : vVertex3(7) = 7 : vVertex3(8) = 0
		vVertex3(9) = 7 : vVertex3(10) = 6 : vVertex3(11) = 0
		' vVertex3(12) = 5: vVertex3(13) = 6: vVertex3(14) = 0
		' vVertex3(15) = 6: vVertex3(16) = 6: vVertex3(17) = 0
		oAcadMLine = ThisDrawing.ModelSpace.AddMLine(vVertex3)
		'   oAcadMLine.Rotate daBase, 45 / (2 * 3.14159)

		oAcadPolyline = ThisDrawing.ModelSpace.AddPolyline(vVertex3)
		oAcadPolyline.color = ACAD_COLOR.acRed
		Exit Sub
		oAcadLine = ThisDrawing.ModelSpace.AddLine(daStart, daEnd)

		oAcadLine.color = ACAD_COLOR.acYellow
		vVertex1(0) = 1
		vVertex1(1) = 1
		vVertex1(2) = 1
		vVertex1(3) = 2
		vVertex1(4) = 2
		vVertex1(5) = 2
		vVertex1(6) = 2
		vVertex1(7) = 1
		oAcadLWPolyline1 = ThisDrawing.ModelSpace.AddLightWeightPolyline(vVertex1)
		oAcadLWPolyline1.Closed = True

		vVertex2(0) = 0.5
		vVertex2(1) = 1
		vVertex2(2) = 2.5
		vVertex2(3) = 2
		'  vVertex2(4) = 3
		'  vVertex2(5) = 1.5
		'   vVertex1(6) = 2
		'  vVertex1(7) = 1
		'Set oAcadLWPolyline2 = ThisDrawing.ModelSpace.AddLightWeightPolyline(vVertex2)
		'   oAcadLWPolyline2.Closed = True
		vRetVal = oAcadLWPolyline1.IntersectWith(oAcadLine, AcExtendOption.acExtendNone)
		oAcadPolyline = ThisDrawing.ModelSpace.AddPolyline(vRetVal)

		oAcadPolyline.color = ACAD_COLOR.acRed
		moAutocadApp.ZoomExtents()

	End Sub

	Private Function zzGetColorStr(ByVal vlaColors() As ACAD_COLOR) As String

		Dim iErr As Integer

		Dim iColorUB As Integer

		Dim sOut As String = String.Empty
		iColorUB = vlaColors.GetUpperBound(0)
		iErr = 1

		For iIndex As Integer = 0& To iColorUB
			If Len(sOut) <> 0& Then sOut = sOut & ","
			sOut = sOut & zzGetColorName(vlaColors(iIndex))
		Next
		Return sOut
	End Function

	Public Sub TestZebra()
		Dim oDrawZebra As DrawZebra
		Dim vlaColors(3&) As Long
		vlaColors(0&) = ACAD_COLOR.acRed
		vlaColors(1&) = ACAD_COLOR.acGreen
		vlaColors(2&) = ACAD_COLOR.acBlue
		vlaColors(3&) = ACAD_COLOR.acYellow
		gdPrmZebraAngle = 45.0#
		gdPrmZebraWidth = 10.0#
		oDrawZebra = New DrawZebra
		oDrawZebra.BaseHeight = 100.0#
		oDrawZebra.BaseWidth = 200.0#
		oDrawZebra.SetBasePoint(200.0#, 0)
		oDrawZebra.Paint(vlaColors)
	End Sub

	Private Sub zzResetGlobal()
		gdTableXScale = 0.0#
		gdTableYScale = 0.0#

		If Not grsPrint Is Nothing Then
			If grsPrint.State <> 0& Then grsPrint.Close()
			grsPrint = Nothing
		End If

		If Not mrsShare Is Nothing Then
			If mrsShare.State <> 0 Then mrsShare.Close()
			mrsShare = Nothing
		End If

		If Not mrsTotal Is Nothing Then
			If mrsTotal.State <> 0& Then mrsTotal.Close()
			mrsTotal = Nothing
		End If
		If Not grsAfrica Is Nothing Then
			If grsAfrica.State <> 0& Then grsAfrica.Close()
			grsAfrica = Nothing
		End If



		gdicColors = Nothing

		gdTotalArea = 0.0#
		gdSumExproArea = 0.0#
		glTotalProp = 0&
		giPrintEntCount = 0&
	End Sub

	Private Sub zzEraseTable()

		For iIndex As Integer = 0& To giPrintEntCount - 1
			goaPrintEnt(iIndex).Delete()
		Next
		giPrintEntCount = 0&

	End Sub

	Public Sub AddTableEntity(ByVal oAcadEntity As AcadEntity)
		If oAcadEntity Is Nothing Then Stop

		If giPrintEntCount = 0& Then
			ReDim goaPrintEnt(giPrintEntCount)
		Else
			ReDim Preserve goaPrintEnt(giPrintEntCount)
		End If
		goaPrintEnt(giPrintEntCount) = oAcadEntity
		giPrintEntCount = giPrintEntCount + 1
	End Sub

	Public Sub TestObj()
		goTopoMaster.updateDisplay()
	End Sub
	Public Sub SelectBlock()
		Dim oObject As System.Object = Nothing
		Dim oAcadObject As AcadObject
		Dim oAcadBlockRef As IAcadBlockReference
		Dim vPickedPoint As System.Object = Nothing
		Dim lStatus As Long
		On Error Resume Next
		Do
			Err.Clear()
			ThisDrawing.Utility.GetEntity(oObject, vPickedPoint, "Select Block " & gsCentrBlock & ":")
			oAcadObject = DirectCast(oObject, AcadObject)
			If Err.Number <> 0& OrElse oAcadObject.ObjectName <> "AcDbBlockReference" Then
				If lStatus = 0& Or lStatus = 3& Then
					Err.Clear()
					lStatus = 1& ' after esc-1
				ElseIf lStatus = 1& Then
					lStatus = 2& ' after esc-2 EXIT
				End If
			Else
				Err.Clear()
				oAcadBlockRef = DirectCast(oAcadObject, IAcadBlockReference)
				If Err.Number = 0& Then
					If oAcadBlockRef.Name = gsCentrBlock Then
						lStatus = 0&
						zzInitEditBlockAttrForm()
						gfrmEditBlockAttr.BlockRef = oAcadBlockRef
						gfrmEditBlockAttr.Show()
						oAcadObject = Nothing
						oAcadBlockRef = Nothing
						gfrmEditBlockAttr = Nothing
					Else
						Err.Clear()
						lStatus = 3&
						Beep()
					End If
				Else
					Err.Clear()
					lStatus = 3&
					Beep()
				End If
			End If
		Loop While (lStatus = 1&) Or (lStatus = 3&)
	End Sub

	Private Sub zzInitEditBlockAttrForm()
		gfrmEditBlockAttr = New frmEditBlockAttr
	End Sub
	Private Sub zzInitMsgForm()
		gfrmMsg = New frmMsg
	End Sub

	Public Sub OutputSheet()
		Const sTemplate As String = "\\Zeus\Dm_app\AcadVBA\Bamash\Support\ReportTempl.xls"
		Dim moExcel As Excel.Application
		Dim moWorkbooks As Excel.Workbooks
		Dim moWorkbook As Excel.Workbook

		Dim moWorksheets As Excel.Worksheets
		Dim moWorksheet As Excel.Worksheet
		Dim moSheetRange As Excel.Range
		Dim lRow As Long
		Dim lCol As Long
		Dim lFldCount As Long
		Dim lNextRow As Long	'  0 --same property
		'  1 --other property
		Dim lCurrentPropID As Long

		Dim oCurrentCell As Range
		Dim oInterior As Interior
		Dim iAcadColor As Integer
		Dim sPropID As String
		Dim lPolygonID As Long
		Dim vlaPropID() As Integer
		Dim sOut As String
		Dim iIndex As Integer
		Dim sNewName As String
		On Error Resume Next
		If grsPrint Is Nothing Then Exit Sub
		moExcel = CType(GetObject(, "Excel.Application"), Excel.Application)
		If moExcel Is Nothing Then moExcel = CType(GetObject("", "Excel.Application"), Excel.Application)
		If moExcel Is Nothing Then Exit Sub
		moWorkbooks = CType(moExcel.Workbooks, Excel.Workbooks)
		moWorkbook = moExcel.Workbooks.Add(sTemplate)
		moExcel.Visible = False

		'   Set moWorksheet = moExcel.ActiveWorkbook.ActiveSheet
		moWorksheets = CType(moWorkbook.Worksheets, Worksheets)
		moWorksheet = CType(moWorksheets.Item(1&), Worksheet)
		moSheetRange = moWorksheet.Cells
		lRow = 3&
		moSheetRange(lRow, 2&) = gsInpBlock
		moSheetRange(lRow, 3&) = gsInpParcel
		moSheetRange(lRow, 4&) = FromDOS(gsInpStreet)
		moSheetRange(lRow, 5&) = FromDOS(gsInpBldNum)
		moSheetRange(lRow, 6&) = FromDOS(gsInpCity)
		moSheetRange(lRow, 7&) = glTotalProp




		moWorksheet = CType(moWorksheets.Item(2&), Worksheet)
		moSheetRange = moWorksheet.Cells
		lRow = 4&
		lFldCount = grsPrint.Fields.Count
		grsPrint.MoveFirst()

		Do Until grsPrint.EOF
			lCurrentPropID = CIntN(grsPrint.Fields("PropID").Value)
			grsPrint.MoveNext()
			If grsPrint.EOF Then
				lNextRow = 1&
			ElseIf lCurrentPropID = CIntN(grsPrint.Fields("PropID").Value) Then
				lNextRow = 0&
			Else
				lNextRow = 1&
			End If
			grsPrint.MovePrevious()
			For lCol = 2& To 15&
				oCurrentCell = CType(moSheetRange.Item(lRow, lCol), Range)
				Select Case lCol
					Case 14&
						oInterior = oCurrentCell.Interior
						iAcadColor = CIntN(grsPrint.Fields(0&).Value)
						If iAcadColor <> 0& Then oInterior.Color = CLng(GetWinColor(iAcadColor))
						zzDrawCellBorder(oCurrentCell, False, lNextRow = 1&)

					Case 15&
						If CIntN((grsPrint.Fields("ColorID").Value)) <> 0 Then
							oCurrentCell.Value = grsPrint.Fields("Color").Value
						End If
						zzDrawCellBorder(oCurrentCell, True, lNextRow = 1&)

					Case Else
						oCurrentCell.Value = grsPrint.Fields(15& - lCol)
						zzDrawCellBorder(oCurrentCell, False, lNextRow = 1&)
				End Select

			Next lCol
			lRow = lRow + 1&
			grsPrint.MoveNext()
		Loop
		For lCol = 2& To 15&
			oCurrentCell = CType(moSheetRange.Item(lRow, lCol), Range)
			zzDrawCellBorder(oCurrentCell, False, True)
			If lCol = 15& Then zzDrawCellBorder(oCurrentCell, True, True)
			If lCol = 6& Then
				oCurrentCell.Value = "רכוש משותף"
			ElseIf lCol = 7& Then
				oCurrentCell.Value = gsInpParcel & "/0"
			End If
		Next
		grsPrint.Close()
		grsPrint = Nothing
		If mrsShare.RecordCount > 0& Then
			mrsShare.MoveFirst()
			moWorksheet = CType(moWorksheets.Item(3&), Worksheet)
			moSheetRange = moWorksheet.Cells
			lRow = 3&
			Do Until mrsShare.EOF
				oCurrentCell = CType(moSheetRange.Item(lRow, 2&), Range)
				oCurrentCell.Value = mrsShare.Fields("ApartDescr").Value
				zzDrawCellBorder(oCurrentCell, False, True)

				oCurrentCell = CType(moSheetRange.Item(lRow, 3&), Range)
				oCurrentCell.Value = mrsShare.Fields("Caption").Value
				zzDrawCellBorder(oCurrentCell, False, True)
				sPropID = CStrN(mrsShare.Fields("PropID").Value)
				lPolygonID = CIntN(mrsShare.Fields("ID").Value)
				vlaPropID = SplitToInt(sPropID, lPolygonID)
				sOut = ""
				For iIndex = 0& To UBound(vlaPropID)
					If Len(sOut) <> 0& Then sOut = sOut & ", "
					sOut = sOut & zzGetColorName(CType(vlaPropID(iIndex), ACAD_COLOR))
				Next

				oCurrentCell = CType(moSheetRange.Item(lRow, 4&), Range)
				oCurrentCell.Value = sOut
				zzDrawCellBorder(oCurrentCell, False, True)

				oCurrentCell = CType(moSheetRange.Item(lRow, 5&), Range)
				oCurrentCell.Value = Strings.Format(mrsShare.Fields("PolygonArea").Value, gsAreaFmt)
				zzDrawCellBorder(oCurrentCell, False, True)

				oCurrentCell = CType(moSheetRange.Item(lRow, 6&), Range)
				oCurrentCell.Value = mrsShare.Fields("PropID").Value
				zzDrawCellBorder(oCurrentCell, True, True)

				mrsShare.MoveNext()
				lRow = lRow + 1&
			Loop
		End If
		sNewName = zzGetFreeName()
		moWorkbook.SaveAs(sNewName)
		moExcel.Visible = True
		moExcel = Nothing
	End Sub

	Public Sub OutputAfrica()
		Const sTemplate As String = "\\Zeus\Dm_app\AcadVBA\Bamash\Support\AfricaTempl.xls"
		Dim moExcel As Excel.Application
		Dim moWorkbooks As Excel.Workbooks
		Dim moWorkbook As Excel.Workbook

		Dim moWorksheets As Excel.Worksheets
		Dim moWorksheet As Excel.Worksheet

		Dim moSheetRange As Excel.Range
		Dim lRow As Long
		Dim lCol As Long
		Dim lFldCount As Long
		Dim lNextRow As Long	'  0 --same property
		'  1 --other property
		Dim lCurrentPropID As Long

		Dim oCurrentCell As Range
		Dim oInterior As Interior
		Dim lAcadColor As Long
		Dim sPropID As String
		Dim lPolygonID As Long

		Dim sOut As String
		Dim lIndex As Long
		Dim sNewName As String
		On Error Resume Next

		moExcel = CType(GetObject(, "Excel.Application"), Excel.Application)
		If moExcel Is Nothing Then moExcel = CType(GetObject("", "Excel.Application"), Excel.Application)
		If moExcel Is Nothing Then Exit Sub
		moWorkbooks = CType(moExcel.Workbooks, Excel.Workbooks)
		moWorkbook = CType(moWorkbooks.Add(sTemplate), Excel.Workbook)
		moExcel.Visible = False

		'   Set moWorksheet = moExcel.ActiveWorkbook.ActiveSheet
		moWorksheets = CType(moWorkbook.Worksheets, Worksheets)
		moWorksheet = CType(moWorksheets.Item(1&), Worksheet)
		moSheetRange = CType(moWorksheet.Cells, Range)
		lRow = 3&
		moSheetRange.Item(lRow, 2&) = gsInpBlock
		moSheetRange(lRow, 3&) = gsInpParcel
		moSheetRange(lRow, 4&) = FromDOS(gsInpStreet)
		moSheetRange(lRow, 5&) = FromDOS(gsInpBldNum)
		moSheetRange(lRow, 6&) = FromDOS(gsInpCity)
		moSheetRange(lRow, 7&) = glTotalProp




		moWorksheet = CType(moWorksheets.Item(2), Worksheet)
		moSheetRange = moWorksheet.Cells
		lRow = 3&
		lFldCount = grsAfrica.Fields.Count
		grsAfrica.MoveFirst()

		Do Until grsAfrica.EOF
			For lCol = 1& To lFldCount
				oCurrentCell = DirectCast(moSheetRange.Item(lRow, lCol), Range)
				oCurrentCell.Value = grsAfrica.Fields(lCol - 1)
			Next lCol
			lRow = lRow + 1&
			grsAfrica.MoveNext()
		Loop

		grsAfrica.Close()
		grsAfrica = Nothing
		sNewName = zzGetFreeName()
		'   moWorkbook.Save
		'  moWorkbook.SaveAs sNewName
		moExcel.Visible = True
		moExcel = Nothing
	End Sub


	Private Sub zzDrawCellBorder(ByVal oCell As Range, ByVal bRight As Boolean, ByVal bBottom As Boolean)
		On Error Resume Next
		With oCell.Borders(XlBordersIndex.xlEdgeLeft)
			.LineStyle = XlLineStyle.xlContinuous
			.Weight = 3
		End With
		If bRight Then
			With oCell.Borders(XlBordersIndex.xlEdgeRight)
				.LineStyle = XlLineStyle.xlContinuous
				.Weight = 3
			End With
		End If
		If bBottom Then
			With oCell.Borders(XlBordersIndex.xlEdgeBottom)
				.LineStyle = XlLineStyle.xlContinuous
				.Weight = 3
			End With
		End If
	End Sub


	Public Function zzGetFreeName() As String	'Private
		Dim lIndex As Long
		Dim sPath As String
		Dim sBaseName As String
		Dim sOutput As String
		On Error Resume Next
		sPath = ThisDrawing.Path
		sBaseName = Strings.Left$(ThisDrawing.Name, Len(ThisDrawing.Name) - 4)
		lIndex = 1&
		Do
			sOutput = sPath & "\" & sBaseName & CStr(lIndex) & ".xls"
			If Len(Dir(sOutput)) = 0& Then
				Exit Do
			End If
			lIndex = lIndex + 1&
		Loop

		zzGetFreeName = sOutput
	End Function

End Module
