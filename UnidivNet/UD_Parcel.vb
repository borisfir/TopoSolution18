Option Explicit On
Option Strict On
Imports System.Data
Imports TopoManager.TPlanGraph
Public Class UD_Parcel
   Inherits TopoManager.TPlanGraph.TplnTopoPgon
   '   Private o As UD_Action

   Friend Const msNameFieldName As String = "Parcel"
   Friend Const msNameNeighborFieldName As String = "ParcelNeighbor"
   Friend Const msParcelOrderFieldName As String = "POrder"
   Friend Const msParcelOrderNeighborFieldName As String = "POrderNeighbor"

   Friend Const msBlockFieldName As String = "Block"
   Friend Const msBlockNeighborFieldName As String = "BlockNeighbor"

   Friend Const msBlockTypeNameFieldName As String = "BlockType"

   Friend Const msAreaNeighborFldName As String = "AreaNeighbor"

   Public Const msLegalAreaFieldName As String = "LegalArea"
   Public Const msLegalAreaNeighborFieldName As String = "LegalAreaNeighbor"
   Public Const msMoveAreaFieldName As String = "MoveArea"

   Public Const msTopoIDNeighborFldName As String = "NeighborTopoID"
   Public Const msPgonAreaFieldName As String = "PgonArea"
   Friend Const msInLotAreaFieldName As String = "InLotArea"
   Friend Const msInLotCalcAreaFieldName As String = "InLotCalcArea"

   Friend Const msInPlanAreaFieldName As String = "InPlanArea"
   Friend Const msInPlanCalcAreaFieldName As String = "InPlanCalcArea"

   Public Const msInPlanCalcAreaApprFieldName As String = "InPlanCalcAreaAppr"
   Public Const msInPlanCalcAreaPropFieldName As String = "InPlanCalcAreaProp"

   Public Const msInPlanCalcAreaApprUnFieldName As String = "InPlanCalcAreaApprUn"
   Public Const msInPlanCalcAreaPropUnFieldName As String = "InPlanCalcAreaPropUn"




   Private Const msNameAttribTag As String = "PARCEL_NAME"
   Private Const msBlockAttribTag As String = "GUSH"
   Private Const msBlockAddAttribTag As String = ""

   Private Const msCrossAttribTag As String = "CROSS"
   Private Const msStatusAttribTag As String = "PARCEL_STATUS"
   Private Const msParcelSourceAttribTag As String = "PARCEL_SOURCE"

   Private Const msLegalAreaAttribTag As String = "LEGAL_AREA"
   Private Const msCalcAreaAttribTag As String = "CALC_AREA"
   Private Const msParcePrevAttribTag As String = "PARCEL_PREVIOUS"
   Private Const msGushPrevAttribTag As String = "GUSH_PREVIOUS"
   Private Const msTabaPlanAttribTag As String = "TABA_PLAN"
   Private Const msTabaMigrashAttribTag As String = "TABA_MIGRASH"
   Private Const msTabaYeudAttribTag As String = "TABA_YEUD"
   Private Const msCommentAttribTag As String = "COMMENT"



   Private Const msBlockName As String = "C1603"
   Private Const msLotBlockName As String = "CellnoDM"

   Private Shared moAcadBlock As DMAcadExt.AcadBlock

   Private Shared moAcadBlockDef As DMAcadExt.AcadBlockDef
   Private mdgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute = {New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetParcelName) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetBlockNo) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLegalArea) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetCalcArea) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetParcelPrev) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetGushPrev) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetPlan) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLot) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLanduse) _
                                                                           , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetComment)}
   Private Shared miaAttributesID() As DMAcadExt.enAcadAttributes = {DMAcadExt.enAcadAttributes.NameStr _
                                                                    , DMAcadExt.enAcadAttributes.ParentNameNum _
                                                                    , DMAcadExt.enAcadAttributes.LegalArea _
                                                                    , DMAcadExt.enAcadAttributes.AcadArea _
                                                                    , DMAcadExt.enAcadAttributes.ParcelPrev _
                                                                    , DMAcadExt.enAcadAttributes.GushPrev _
                                                                    , DMAcadExt.enAcadAttributes.Plan _
                                                                    , DMAcadExt.enAcadAttributes.Lot _
                                                                    , DMAcadExt.enAcadAttributes.LanduseName _
                                                                    , DMAcadExt.enAcadAttributes.Comment}

   Private Shared moAltAcadBlockDef As DMAcadExt.AcadBlockDef
   Private mdgaParseAltAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute = {New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLotName) _
                                                                    , New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetParcelName)}
   Private Shared miaAltAttributesID() As DMAcadExt.enAcadAttributes = {DMAcadExt.enAcadAttributes.NameStr _
                                                                    , DMAcadExt.enAcadAttributes.NewParcelNo}
   Private Shared PaintAreaHatch(TopoManager.TPlanGraph.enAreaStatus.UB) As DMAcadExt.DMHatch
   Private Shared mtParcelMapThemeData As DMAcadExt.MapThemeData
   Private Shared msCentroidBlockName As String

	Private Shared miLastDbID As Integer


	Private miBlockNo As Integer
	Private miBlockAdd As Integer

	Private msName As String
	Private miOrder As Integer

	Private msLotName As String  'From Base plan
	Private mtParcelArea As TopoManager.TPlanGraph.ParcelArea
	'	Private mdLegalArea As Double = 0.0
	Private Shared miRoundDigit As Integer = 5
	Private moNewParcel As UD_Parcel = Nothing
	Private moPreviousParcel As UD_Parcel
	Private mlstPreviousParcels As List(Of UD_Parcel) = New List(Of UD_Parcel)
	Private mdicPreviousParcels As Dictionary(Of UD_ParcelKey, UD_Parcel) = New Dictionary(Of UD_ParcelKey, UD_Parcel)
	Private mtFragmentCenter As Autodesk.AutoCAD.Geometry.Point3d
	'	Private mdicFragments As Dictionary(Of Integer,
	Private mhsFragments As HashSet(Of Integer) = New HashSet(Of Integer)
	Private mtParcelKey As UD_ParcelKey
	Private miDbID As Integer = 0
	Private miTmpDbID As Integer = 0
	Private miTmpParcelNo As Integer = 0


	Private mbIDFromDB As Boolean

	Private miStage As Integer = -1

	Private msDispName As String
	Private msPrevParcelName As String
	Private msPrevGushName As String

	Private msLot As String
	Private msLotDos As String

	Private msPlan As String
	Private msPlanDos As String

	Private miLanduseID As String
	Private msLanduseName As String
	Private msLanduseNameDos As String


	Private msComment As String
	Private mbAltBlock As Boolean
	Private miFinalBlockKey As Integer
	'Private mdCalcArea As Double
	'	Private mdDeltaArea As Double
	'	Private mdTolerance As Double
	'	Private mdDeviation As Double
	Private mbHasDeviation As Boolean
	Private mdNeed As Double
	'Private miAreaStatus As enAreaStatus
	Private mbOut As Boolean
	Private mbIsOriginal As Boolean
	Private mbIsActive As Boolean
	Private mbIsOut As Boolean
	Private mbIsCanceled As Boolean = False
	Private mbIsMoved As Boolean = False
	Private mbSelected As Boolean = False
	Private mbHasNewBlock As Boolean = False
	Private mbForcedNumber As Boolean

	'  Private mtBorderObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
	Private moPolygonScheme As TopoManager.TopoScheme.tsPolygon
	'	Private mbIsResult As Boolean
	Private miActionType As enActionType
	Private mcolLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
	Private mcolPointLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink)
	Private mbPlanDataFromBlock As Boolean
	Private msSourceCentroidLayer As String
	Private mtCentroidScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d
	Private Shared moMainDataTable As System.Data.DataTable
	Private Shared moAdjoiningParcelsTable As System.Data.DataTable
	Private Shared miaBlockAttribIndex(12) As Integer          '''''''''''''''''	enBlockCentroidAttribIndices.UB

	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon, False)
		Dim oCentroidBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
		Dim bAcadPoint As Boolean

		oCentroidBlockRef = DMAcadExt.AcadTransaction.GetBlockRefForRead(dtCentroidAcObjID, True, bAcadPoint, moAcadBlockDef.BlockName, "", mbAltBlock, moAltAcadBlockDef.BlockName, "")

		If oCentroidBlockRef IsNot Nothing Then
			msSourceCentroidLayer = oCentroidBlockRef.Layer
			mtCentroidScaleFactors = oCentroidBlockRef.ScaleFactors
		End If

		dbAcadPoint = bAcadPoint
		If bAcadPoint Then
			mbHasNewBlock = True
		End If
		'DMCommon.Debug.MsgBox("12_720!!", dbAcadPoint, bAcadPoint, mbHasNewBlock, msName, moAcadBlockDef.BlockName, mbAltBlock, moAltAcadBlockDef.BlockName)
		If mbAltBlock Then
			If moAltAcadBlockDef.AttributeIndices IsNot Nothing Then
				DMCommon.Functions.DispArray("01_565alt", moAltAcadBlockDef.AttributeIndices)
				MyBase.InputAttributeData(moAltAcadBlockDef, mdgaParseAltAttribute, False)

			Else
				DMCommon.Debug.MsgBox(" moAltAcadBlockDef.AttributeIndices Is Nothing ")
			End If
		Else
			If moAcadBlockDef.AttributeIndices IsNot Nothing Then
				MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute, False)

			End If
		End If

		SetParcelKey(New UD_ParcelKey(miBlockNo, miBlockAdd, msName))

		zzSetDbID()
	End Sub
	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon, iDbID As Integer, tParcelKey As UD_ParcelKey)
		MyBase.New(oPolygon, False)


		If moAcadBlockDef.AttributeIndices IsNot Nothing Then
			MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute, False)
			If dbAcadPoint Then
				mbHasNewBlock = True
			End If

			'	DMCommon.Debug.MsgBox("11_512", dbAcadPoint, mbHasNewBlock, miBlockNo, msName, tParcelKey.BlockNo)
			If miBlockNo = 0 Then
				miBlockNo = tParcelKey.BlockNo

				miBlockAdd = tParcelKey.BlockAdd
			End If
			If dbAcadPoint Then
				SetParcelKey(tParcelKey)
			ElseIf Not String.IsNullOrEmpty(msName) Then
				mtParcelKey = New UD_ParcelKey(miBlockNo, miBlockAdd, msName)
				mbIsOriginal = mtParcelKey.Original
			Else
				SetParcelKey(tParcelKey)
			End If
		Else
			DMCommon.Debug.MsgBox("SysErr #1942")
		End If

		'   DMCommon.Debug.MsgBox("11_512a", dbAcadPoint, Me.CentroidPoint2d.ToString, miBlockNo, msName, mtParcelKey.BlockNo, mtParcelKey.UD_ParcelName)



		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''   MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute)
		' DMCommon.Debug.MsgBox("09_271a", miBlockNo, BlockFull, mtParcelKey.BlockNo)
		'Dim iParcelNo As Integer

		'If Integer.TryParse(msName, iParcelNo) Then
		'   mtParcelKey = New UD_ParcelKey(iParcelNo, True)
		'End If
		If iDbID = 0 Then
			zzSetDbID()
		Else

			miDbID = iDbID
			If miDbID > miLastDbID Then
				miLastDbID = miDbID
			End If
		End If

	End Sub
	Public Sub New(ByVal oPolygon As TopoManager.TopoScheme.tsPolygon, tBlockRefData As DMAcadExt.BlockRefData)
		MyBase.New(oPolygon, False)
		zzSetDbID()
		'  System.Windows.Forms.MessageBox.Show(moAcadBlockDef.AttributeIndices.GetUpperBound(0).ToString, "01_144")
		'   MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute)
		'	System.Windows.Forms.MessageBox.Show(CStr(miBlockNo) & ":" & msName & ":" & CStr(mdLegalArea), "UD_Parcel - New")


		mtParcelKey = New UD_ParcelKey(tBlockRefData.AttribValues(0))

		'   mtParcelKey.DebugMsg("New UD_Parcel")

	End Sub
	Public Sub New(ByVal oPolygonScheme As TopoManager.TopoScheme.tsPolygon, tParcelKey As UD_ParcelKey)
		MyBase.New(oPolygonScheme, False)

		'  System.Windows.Forms.MessageBox.Show(moAcadBlockDef.AttributeIndices.GetUpperBound(0).ToString, "01_144")
		'   MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute)
		'	System.Windows.Forms.MessageBox.Show(CStr(miBlockNo) & ":" & msName & ":" & CStr(mdLegalArea), "UD_Parcel - New")
		SetParcelKey(tParcelKey)
		mtParcelArea.AcadArea = ddAcadArea
		moPolygonScheme = oPolygonScheme
		zzSetDbID()
		'   mtParcelKey.DebugMsg("New UD_Parcel")

	End Sub
	Public Sub New(oPolygon As Autodesk.AutoCAD.DatabaseServices.MPolygon, tCentroidAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId)
		MyBase.New(oPolygon, 0, tCentroidAcObjID)
		MyBase.SetAttributeOrder()
		If dsaBlockAttribText.GetUpperBound(0) >= 0 Then
			msName = dsaBlockAttribText(0)
		End If
		DMCommon.Functions.DispArray(dsaBlockAttribText, "01_596d", True)
		If dsaBlockAttribText.GetUpperBound(0) >= 1 Then
			Integer.TryParse(dsaBlockAttribText(1), miBlockNo)
			mtParcelKey.BlockNo = miBlockNo
		End If
		If dsaBlockAttribText.GetUpperBound(0) >= 5 Then
			zzGetLegalArea(dsaBlockAttribText(5))

			'      System.Windows.Forms.MessageBox.Show(CStr(mdLegalArea), "08_278")
		End If

		'mtParcelData = New ParcelData(dsaBlockAttribText)
		'MyBase.dsName = mtParcelData.Name
		'MyBase.diOrder = mtParcelData.Order
	End Sub
	Public Sub New(ByVal tParcelKey As UD_ParcelKey)
		MyBase.New(0)
		SetParcelKey(tParcelKey)
		zzSetDbID()
	End Sub
	Public Sub New(ByVal oSourceParcel As UD_Parcel, tParcelKey As UD_ParcelKey)
		MyBase.New(0)
		SetParcelKey(tParcelKey)
		mtParcelArea = oSourceParcel.ParcelArea
		mhsFragments = oSourceParcel.Fragments
		zzSetDbID()
	End Sub
	Public Sub New(ByVal tParcelKey As UD_ParcelKey, iDbID As Integer)
		MyBase.New(0)
		mtParcelKey = tParcelKey
		miDbID = iDbID
	End Sub

	Public Sub New(ByVal tParcelKey As UD_ParcelKey, hsFragments As HashSet(Of Integer), tParcelArea As TopoManager.TPlanGraph.ParcelArea)
		Me.New(tParcelKey)
		mhsFragments = hsFragments
		mtParcelArea = tParcelArea
		msName = tParcelKey.UD_ParcelName
		zzSetDbID()
	End Sub
	Public Sub New(ByVal tParcelKey As UD_ParcelKey, hsFragments As HashSet(Of Integer))
		Me.New(tParcelKey)
		mhsFragments = hsFragments

		msName = tParcelKey.UD_ParcelName
		zzSetDbID()
	End Sub
	Public ReadOnly Property _ParcelArea() As ParcelArea
		Get
			Return mtParcelArea
		End Get
	End Property
	Public Shared ReadOnly Property DbIDCounter() As Integer
		Get
			Return miLastDbID
		End Get
	End Property
	Public ReadOnly Property IsOut() As Boolean
		Get
			Return mbIsOut
		End Get
	End Property
	Public Property BlockNo() As Integer
		Get
			Return mtParcelKey.BlockNo
		End Get
		Set(iValue As Integer)
			miBlockNo = iValue
			mtParcelKey.BlockNo = iValue
		End Set
	End Property
	Public Property BlockAdd() As Integer
		Get
			Return mtParcelKey.BlockAdd

		End Get
		Set(iValue As Integer)
			miBlockAdd = iValue
			mtParcelKey.BlockAdd = iValue
		End Set
	End Property
	Public ReadOnly Property BlockFull() As String
		Get
			Return TopoManager.TPlanGraph.TplnBlock.GetBlockName(Me.BlockNo, Me.BlockAdd)

		End Get
	End Property
	Public ReadOnly Property Name() As String
		Get
			Return msName
		End Get
	End Property
	Public Sub LegalToForcedArea()
		mtParcelArea.ForcedArea = mtParcelArea.LegalArea
		'mtParcelArea.LegalArea = 0.0
	End Sub
	Public Sub LegalFromForcedArea()
		mtParcelArea.LegalArea = mtParcelArea.ForcedArea

	End Sub

	Public Property LegalArea() As Double

		Get
			' Return mdLegalArea
			Return mtParcelArea.LegalArea

		End Get
		Set(dValue As Double)
			mtParcelArea.LegalArea = dValue
		End Set

	End Property
	Public Overloads ReadOnly Property AcadArea() As Double
		Get
			' Return mdLegalArea
			Return mtParcelArea.AcadArea

		End Get
	End Property
	Public ReadOnly Property Order() As Integer
		Get
			Return miOrder
		End Get
	End Property
	Public ReadOnly Property ParcelArea() As TopoManager.TPlanGraph.ParcelArea
		Get
			Return mtParcelArea
		End Get

	End Property

	Public ReadOnly Property ParcelKey() As UD_ParcelKey
		Get
			Return mtParcelKey
		End Get
	End Property
	Public Property DbID As Integer
		Get
			Return miDbID
		End Get
		Set(iValue As Integer)
			miDbID = iValue
			If miLastDbID < miDbID Then
				miLastDbID = miDbID
			End If

		End Set
	End Property
	Public Property TmpDbID As Integer
		Get
			Return miTmpDbID
		End Get
		Set(iValue As Integer)
			miTmpDbID = iValue
			If miLastDbID < miTmpDbID Then
				miLastDbID = miTmpDbID
			End If

		End Set
	End Property
	Public Property TmpParcelNo As Integer
		Get
			Return miTmpParcelNo
		End Get
		Set(iValue As Integer)
			miTmpParcelNo = iValue


		End Set
	End Property

	Public Property Links As System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link)
		Get
			Return mcolLinks

		End Get
		Set(colValue As System.Collections.ObjectModel.Collection(Of DMAcadExt.IUD_Link))
			mcolLinks = colValue
		End Set
	End Property
	Public Property PointLinks As System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink)
		Get
			Return mcolPointLinks
		End Get
		Set(colValue As System.Collections.ObjectModel.Collection(Of DMAcadExt.PointLink))
			mcolPointLinks = colValue
		End Set
	End Property
	Public Sub SetCentroid(tPoint3d As Autodesk.AutoCAD.Geometry.Point3d)
		MyBase.ddCentroidX = tPoint3d.X
		MyBase.ddCentroidY = tPoint3d.Y

	End Sub
	Public Sub Calc()
		'  DMCommon.Debug.MsgBox("09_433", True, MyBase.ddAcadArea, mdLegalArea)
		'mtParcelArea.CalculateArea(mdLegalArea, MyBase.ddAcadArea, miRoundDigit)
		mtParcelArea.AcadArea = MyBase.ddAcadArea
		'mtParcelArea.CalculateArea(mdLegalArea, MyBase.ddAcadArea, miRoundDigit)
		mtParcelArea.CalculateArea(miRoundDigit)
	End Sub
	Public Sub CalculateArea(ByVal dLegalAreaM As Double, ByVal dAcadArea As Double)
		miRoundDigit = 5
		mtParcelArea.CalculateArea(dLegalAreaM, dAcadArea, miRoundDigit)
	End Sub
	Public Sub CalculateArea(ByVal dLegalArea As Double)
		miRoundDigit = 5
		mtParcelArea.AcadArea = ddAcadArea
		'	DMCommon.Debug.MsgBox("09_434c", True, MyBase.ddAcadArea, dLegalArea)
		mtParcelArea.CalculateArea(dLegalArea, miRoundDigit)
	End Sub
	Public Sub SetConditionalArea(ByVal dArea As Double)
		mtParcelArea.ConditionalArea = dArea
	End Sub
	Public Sub SetForcedArea(ByVal dArea As Double)
		mtParcelArea.ForcedArea = dArea
	End Sub
	Public Sub SetPlanData(ByVal sLot As String, sLanduseName As String, sPlan As String)
		msLot = sLot
		msLotDos = sLot
		msLanduseName = sLanduseName
		msLanduseNameDos = sLanduseName
		msPlan = sPlan
		msPlanDos = sPlan
	End Sub






	Public Sub GetRangeArea(ByVal dLegalArea As Double)
		miRoundDigit = 5
		mtParcelArea.AcadArea = ddAcadArea
		mtParcelArea.CalculateArea(dLegalArea, miRoundDigit)
	End Sub
	Public Function GetCenter() As String
		Return "E:  " & FormatNumber(ddCentroidX, 3, TriState.True, TriState.False, TriState.False) & "  N:  " & FormatNumber(ddCentroidY, 3, TriState.True, TriState.False, TriState.False)
		' E:  183436.570  N:  678450.688
	End Function
	Public Function GetHanitData() As DMAcadExt.BlockRefData
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(Me.ddCentroidX, Me.ddCentroidY, 0.0)
		tBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()
		' "TABA_PLAN", "TABA_MIGRASH", "TABA_YEUD"
		'   System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN("___________") & vbCrLf & DMCommon.Functions.CStrN(msPlan) & vbCrLf & DMCommon.Functions.CStrN(msLot) & vbCrLf & DMCommon.Functions.CStrN(msLanduse) & vbCrLf & Me.BlockFull, "TplnParcel - AddDataToMainTable_1")


		tBlockRefData.AttribValues = {Me.UD_Name, mtParcelArea.LegalArea.ToString(), mtParcelArea.CalcArea.ToString(), Me.BlockFull, Me.Cross, Me.PreviousParcelsStr, DMCommon.Functions.CStrN(msPlan), DMCommon.Functions.CStrN(msLot), DMCommon.Functions.CStrN(msLanduseName), Me.Stage.ToString()}
		Return tBlockRefData
	End Function
	Public Sub ReadNewBlockAttributes()
		'   {msNameAttribTag, msBlockAttribTag, msCrossAttribTag, msStatusAttribTag, msParcelSourceAttribTag, msLegalAreaAttribTag, msCalcAreaAttribTag, msParcePrevAttribTag, msGushPrevAttribTag, msTabaPlanAttribTag, msTabaMigrashAttribTag, msTabaYeudAttribTag, msCommentAttribTag}
		Dim tBlockRefData As DMAcadExt.BlockRefData
		'  Dim saAttribValues() As String = {UD_Name,}
		'  Dim tParcelKey As UD_ParcelKey
		If Not dtCentroidAcObjID.IsNull Then

			' DMCommon.Functions.DispArray(saAttribValues, "saAttribValues!", True)
			' DMCommon.Debug.MsgBox("09_557", moAcadBlock.BlockName, diCentroidAcObjID)
			'  DMCommon.Debug.MsgBox("09_271x", miBlockNo, BlockFull, mtParcelKey.BlockNo, "#" & saAttribValues(1) & "#")
			'  moAcadBlock.UpdateAttribData(diCentroidAcObjID, saAttribValues)
			tBlockRefData = moAcadBlock.GetBlockRefData(dtCentroidAcObjID, False)
			' DMCommon.Functions.DispArray(tBlockRefData.AttribValues, "AttribValues", True)
			If tBlockRefData.AttribValues.GetUpperBound(0) >= 1 Then
				Try
					Integer.TryParse(tBlockRefData.AttribValues(1), miBlockNo)
					mtParcelKey = New UD_ParcelKey(miBlockNo, miBlockAdd, tBlockRefData.AttribValues(0))

					msName = Convert.ToString(mtParcelKey.ParcelNo)
					mbIsOriginal = mtParcelKey.Original
					'   DMCommon.Debug.MsgBox("09_566", miBlockNo, mtParcelKey.BlockNo, mtParcelKey.UD_ParcelName)

					msPlan = tBlockRefData.AttribValues(9)
					msLot = tBlockRefData.AttribValues(10)
					msLanduseName = tBlockRefData.AttribValues(11)
					'   DMCommon.Debug.MsgBox("09_567", msPlan, msLot, msLanduse)


				Catch oEx As Exception

				End Try
			End If


		Else
			DMCommon.Debug.MsgBox("09_572z", "CentroidAcObjID Is Null")
		End If
	End Sub
	Public Sub UpdateBlockAttributes()
		'   {msNameAttribTag, msBlockAttribTag, msCrossAttribTag, msStatusAttribTag, msParcelSourceAttribTag, msLegalAreaAttribTag, msCalcAreaAttribTag, msParcePrevAttribTag, msGushPrevAttribTag, msTabaPlanAttribTag, msTabaMigrashAttribTag, msTabaYeudAttribTag, msCommentAttribTag}
		'	DMCommon.Debug.MsgBox("12_818U", UD_Name, BlockFull, Cross, PrevParcelName, msPlanDos, msLotDos, msLanduseNameDos)
		Dim sPlanDos, sLotDos, sLanduseNameDos As String
		If mbPlanDataFromBlock Then
			sPlanDos = Nothing
			sLotDos = Nothing
			sLanduseNameDos = Nothing
		Else
			sPlanDos = msPlanDos
			sLotDos = msLotDos
			sLanduseNameDos = msLanduseNameDos

		End If
		Dim saAttribValues() As String = {UD_Name, BlockFull, Cross, String.Empty, String.Empty, FormatNumber(Me.LegalArea * 0.001, 3), FormatNumber(CalcArea * 0.001, 3), PrevParcelName, String.Empty, sPlanDos, sLotDos, sLanduseNameDos, String.Empty}
		'DMCommon.Debug.MsgBox("09_277t", miBlockNo, UD_Name, dtCentroidAcObjID)
		If Not dtCentroidAcObjID.IsNull AndAlso Not dbAcadPoint Then
			' DMCommon.Functions.DispArray(saAttribValues, "saAttribValues!", True)
			' DMCommon.Debug.MsgBox("09_557", moAcadBlock.BlockName, diCentroidAcObjID)
			'  DMCommon.Debug.MsgBox("09_271x", miBlockNo, BlockFull, mtParcelKey.BlockNo, "#" & saAttribValues(1) & "#")
			'DMCommon.Debug.MsgBox("09_271y", DMCommon.Debug.GetListArray(saAttribValues))

			moAcadBlock.UpdateAttribData(dtCentroidAcObjID, saAttribValues, UnidivNet.UD_App.GetStageParcelCentroidLayer(miStage))
		Else
			DMCommon.Debug.MsgBox("09_572x", "CentroidAcObjID Is Null", dbAcadPoint)
		End If
	End Sub

	Public Property NewParcel As UD_Parcel
		Get
			Return moNewParcel
		End Get
		Set(oValue As UD_Parcel)
			moNewParcel = oValue
		End Set
	End Property
	Public Property FragmentCenter As Autodesk.AutoCAD.Geometry.Point3d
		Get
			Return mtFragmentCenter
		End Get
		Set(tValue As Autodesk.AutoCAD.Geometry.Point3d)
			mtFragmentCenter = tValue
		End Set
	End Property
	Public ReadOnly Property PreviousParcels As List(Of UD_Parcel)
		Get
			Return mlstPreviousParcels
		End Get

	End Property
	Public ReadOnly Property PreviousParcelsStr As String
		Get

			Dim sRes As String = String.Empty
			For Each oParcel As UD_Parcel In mdicPreviousParcels.Values
				If String.IsNullOrEmpty(sRes) Then
					sRes = oParcel.UD_Name
				Else
					sRes &= "," & oParcel.UD_Name
				End If
			Next
			Return sRes
		End Get

	End Property
	Public ReadOnly Property Cross As String

		Get
			If Me.IsResult Then
				Return String.Empty
			Else
				Return "/"
			End If

		End Get

	End Property
	Public ReadOnly Property IsResult As Boolean
		Get
			Return (moNewParcel Is Nothing) AndAlso Not mbIsCanceled
		End Get

	End Property

	Public Property IsOriginal As Boolean
		Get
			Return mtParcelKey.Original
		End Get
		Set(bValue As Boolean)
			mbIsOriginal = bValue
			mtParcelKey.Original = bValue
		End Set
	End Property
	Public Property IsActive As Boolean
		Get
			Return mbIsActive
		End Get
		Set(bValue As Boolean)
			mbIsActive = bValue
		End Set
	End Property
	Public Property HasNewBlock As Boolean
		Get
			Return mbHasNewBlock
		End Get
		Set(bValue As Boolean)
			mbHasNewBlock = bValue
		End Set
	End Property
	Public Property ForcedNumber As Boolean
		Get
			Return mbForcedNumber
		End Get
		Set(bValue As Boolean)
			mbForcedNumber = bValue
		End Set
	End Property



	Public Property IDFromDB_AAA As Boolean
		Get
			Return mbIDFromDB
		End Get
		Set(bValue As Boolean)
			mbIDFromDB = bValue
		End Set
	End Property





	Public Property ActionType As enActionType
		Get
			Return miActionType
		End Get
		Set(iValue As enActionType)
			miActionType = iValue
		End Set
	End Property
	Public Shared ReadOnly Property AcadBlockDef As DMAcadExt.AcadBlockDef
		Get
			Return moAcadBlockDef
		End Get

	End Property
	Public Shared ReadOnly Property AcadBlock As DMAcadExt.AcadBlock
		Get
			Return moAcadBlock
		End Get

	End Property



	Public Property PolygonScheme As TopoManager.TopoScheme.tsPolygon
		Get
			Return moPolygonScheme
		End Get
		Set(oValue As TopoManager.TopoScheme.tsPolygon)
			moPolygonScheme = oValue
		End Set
	End Property
	Public Property Out() As Boolean
		Get
			Return mbOut
		End Get
		Set(ByVal bValue As Boolean)
			mbOut = bValue
		End Set
	End Property

	Public Property IsMoved As Boolean
		Get
			If moNewParcel IsNot Nothing AndAlso moNewParcel.ActionType = enActionType.Transfer Then
				Return True
			Else
				Return mbIsMoved
			End If
		End Get
		Set(bValue As Boolean)
			mbIsMoved = bValue
		End Set
	End Property
	Public Property Selected As Boolean
		Get
			Return mbSelected
		End Get
		Set(bValue As Boolean)
			mbSelected = bValue
		End Set
	End Property
	Public Property DispName As String
		Get
			Return msDispName
		End Get
		Set(sValue As String)
			msDispName = sValue
		End Set
	End Property
	Public Shared ReadOnly Property BlockName As String
		Get
			Return msBlockName
		End Get
	End Property
	Public Shared ReadOnly Property LotBlockName As String
		Get
			Return msLotBlockName
		End Get
	End Property
	Public Shared ReadOnly Property DefaultNewAttribValues As Generic.Dictionary(Of String, String)
		Get

			' "TABA_PLAN", "TABA_MIGRASH", "TABA_YEUD"
			'   System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN("___________") & vbCrLf & DMCommon.Functions.CStrN(msPlan) & vbCrLf & DMCommon.Functions.CStrN(msLot) & vbCrLf & DMCommon.Functions.CStrN(msLanduse) & vbCrLf & Me.BlockFull, "TplnParcel - AddDataToMainTable_1")
			Dim dicAtribValues As Generic.Dictionary(Of String, String) = New Dictionary(Of String, String)()


			dicAtribValues.Add(msBlockAttribTag, String.Empty)
			dicAtribValues.Add(msCrossAttribTag, String.Empty)

			dicAtribValues.Add(msLegalAreaAttribTag, String.Empty)


			dicAtribValues.Add(msParcePrevAttribTag, String.Empty)
			dicAtribValues.Add(msGushPrevAttribTag, String.Empty)





			Return dicAtribValues

		End Get
	End Property
	Public Shared ReadOnly Property DefaultSourceAttribValues As Generic.Dictionary(Of String, String)
		Get

			' "TABA_PLAN", "TABA_MIGRASH", "TABA_YEUD"
			'   System.Windows.Forms.MessageBox.Show(DMCommon.Functions.CStrN("___________") & vbCrLf & DMCommon.Functions.CStrN(msPlan) & vbCrLf & DMCommon.Functions.CStrN(msLot) & vbCrLf & DMCommon.Functions.CStrN(msLanduse) & vbCrLf & Me.BlockFull, "TplnParcel - AddDataToMainTable_1")
			Dim dicAtribValues As Generic.Dictionary(Of String, String) = New Dictionary(Of String, String)()



			dicAtribValues.Add(msCrossAttribTag, String.Empty)

			Return dicAtribValues

		End Get

	End Property



	Public Property Lot As String
		Get
			Return msLot
		End Get
		Set(sValue As String)
			If String.IsNullOrEmpty(sValue) Then
				sValue = sValue.Trim()
				If sValue = "0" Then
					mbIsOut = True
				Else
					msLot = sValue
				End If
			Else
				msLot = sValue
			End If
		End Set
	End Property

	Public Property Plan As String
		Get
			Return msPlan
		End Get
		Set(sValue As String)
			msPlan = sValue
		End Set
	End Property

	Public Property LanduseName As String
		Get
			Return msLanduseName
		End Get
		Set(sValue As String)
			msLanduseName = sValue
		End Set
	End Property
	Public Property PrevParcelName As String
		Get
			If msPrevParcelName Is Nothing Then
				Return String.Empty
			Else
				Return msPrevParcelName
			End If

		End Get
		Set(sValue As String)
			msPrevParcelName = sValue
		End Set
	End Property
	Public Property PrevGushName As String
		Get
			If msPrevGushName Is Nothing Then
				Return String.Empty
			Else
				Return msPrevParcelName
			End If

		End Get
		Set(sValue As String)
			msPrevParcelName = sValue
		End Set
	End Property


	Public ReadOnly Property UD_Name As String
		Get
			Dim sName As String = Convert.ToString(mtParcelKey.ParcelNo)
			If Not IsOriginal Then
				sName = "[" & sName & "]"
			End If
			If Me.IsMoved() Then  'miActionType = enActionType.Transfer
				sName = "(" & sName & ")"
				'	mess()
			End If
			Return sName
		End Get

	End Property
	Public Property Stage As Integer
		Get
			Return miStage
		End Get
		Set(iValue As Integer)
			miStage = iValue
		End Set
	End Property
	Public Property FinalBlockKey As Integer
		Get
			If miFinalBlockKey = 0 Then
				Return mtParcelKey.BlockKey
			Else
				Return miFinalBlockKey
			End If

		End Get
		Set(iValue As Integer)
			miFinalBlockKey = iValue
		End Set
	End Property


	Public Sub AddPreviousParcel(oParcel As UD_Parcel)
		If Not mdicPreviousParcels.ContainsKey(oParcel.ParcelKey) Then
			mdicPreviousParcels.Add(oParcel.ParcelKey, oParcel)
		End If

	End Sub
	Public Sub AddFragment(iFragment As Integer)
		mhsFragments.Add(iFragment)
	End Sub
	Public Sub TryAddFragment(iFragment As Integer)
		If Not mhsFragments.Contains(iFragment) Then
			mhsFragments.Add(iFragment)
		End If

	End Sub
	Public Sub UpdateCentroidAcObjId(tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, bAcadPoint As Boolean)
		dtCentroidAcObjID = tAcObjID
		If dbAcadPoint AndAlso Not bAcadPoint Then
			'''''''''''''''''''	mbHasNewBlock = True
		End If
		dbAcadPoint = bAcadPoint
	End Sub
	Public Sub UpdateLayer(sLayer As String)
		If Not String.IsNullOrEmpty(sLayer) Then
			DMAcadExt.AcadTransaction.SetLayer(dtCentroidAcObjID, sLayer)
		End If

	End Sub
	Public Function GetFragmentsPgonUnion(oFragmentsToposcheme As TopoManager.TopoScheme.tsTopology) As TopoManager.TopoScheme.tsPgonUnion
		Dim oPgon As TopoManager.TopoScheme.tsPolygon
		Dim oPgonUnion As TopoManager.TopoScheme.tsPgonUnion = oFragmentsToposcheme.NewPgonUnion()
		For Each iFragmentId As Integer In mhsFragments
			oPgon = oFragmentsToposcheme.GetPolygon(iFragmentId)
			oPgonUnion.AddPolygon(oPgon)
		Next
		Return oPgonUnion
	End Function
	Public Property Fragments As HashSet(Of Integer)
		Get
			Return mhsFragments
		End Get
		Set(hsValue As HashSet(Of Integer))
			mhsFragments = hsValue
		End Set
	End Property
	Public ReadOnly Property HasFragments As Boolean
		Get
			Return mhsFragments.Count > 0
		End Get
	End Property
	Public Function HasFragment(iFragmentID As Integer) As Boolean

		Return mhsFragments.Contains(iFragmentID)


	End Function

	Public ReadOnly Property SourceCentroidLayer As String
		Get

			Return msSourceCentroidLayer

		End Get
	End Property
	Public ReadOnly Property CentroidScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d
		Get
			Return mtCentroidScaleFactors
		End Get
	End Property
	Public ReadOnly Property FirstFragment As Integer
		Get
			If mhsFragments.Count = 0 Then
				Return 0
			Else
				Return mhsFragments.First()
			End If
		End Get
	End Property
	Public Property IsCanceled As Boolean
		Get
			Return mbIsCanceled
		End Get
		Set(bValue As Boolean)
			mbIsCanceled = bValue
		End Set
	End Property
	Public Sub TESTCreateTopo()
		Dim oFragmentsTopo As Autodesk.Gis.Map.Topology.TopologyModel
		Dim iaFragments(mhsFragments.Count - 1) As Integer
		Dim sDissolveTopoName As String = "Parcel_" & CStr(mtParcelKey.ParcelNo)
		mhsFragments.CopyTo(iaFragments)


		oFragmentsTopo = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
		TopoManager.TopoCreator.DissolveMy(oFragmentsTopo, iaFragments, sDissolveTopoName)
	End Sub
	Public Shared Sub Initialize()
		'    DMCommon.ExcelLogG.Open()
		moAcadBlockDef = New DMAcadExt.AcadBlockDef(DMAcadExt.enAcadBlocks.UD_Parcel_Hanit) '''''''''''''''''''220517
		moAcadBlockDef.AttributesID = miaAttributesID
		'  DMCommon.Debug.MsgBox("11_500z", moAcadBlockDef.AttributesID, miaAttributesID)
		moAcadBlockDef.LoadDWG()
		moAltAcadBlockDef = New DMAcadExt.AcadBlockDef(DMAcadExt.enAcadBlocks.DMLot)
		moAltAcadBlockDef.AttributesID = miaAltAttributesID
		moAltAcadBlockDef.LoadDWG()
	End Sub
	Public Shared Sub InitAcadBlock()
		moAcadBlock = New DMAcadExt.AcadBlock(msBlockName, UD_App.BlockPath)
		moAcadBlock.Fields = New String() {msNameAttribTag, msBlockAttribTag, msCrossAttribTag, msStatusAttribTag, msParcelSourceAttribTag, msLegalAreaAttribTag, msCalcAreaAttribTag, msParcePrevAttribTag, msGushPrevAttribTag, msTabaPlanAttribTag, msTabaMigrashAttribTag, msTabaYeudAttribTag, msCommentAttribTag}

		moAcadBlock.Open(False)

	End Sub
	Public Shared Sub Initialize(tParcelMapThemeData As DMAcadExt.MapThemeData)
		Try
			mtParcelMapThemeData = tParcelMapThemeData
			msCentroidBlockName = mtParcelMapThemeData.CentroidBlocks
			'  System.Windows.Forms.MessageBox.Show(msCentroidBlockName, "11_178")
			Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
			If saBlockAttribTag IsNot Nothing Then
				' DMCommon.Functions.DispArray(saBlockAttribTag, "01_599d", True)
				For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
					Select Case saBlockAttribTag(iAttribIndex)
						Case msNameAttribTag
							miaBlockAttribIndex(0) = iAttribIndex
						Case msBlockAttribTag
							miaBlockAttribIndex(1) = iAttribIndex
						Case msBlockAddAttribTag
						Case msCrossAttribTag
							miaBlockAttribIndex(2) = iAttribIndex
						Case msStatusAttribTag
							miaBlockAttribIndex(3) = iAttribIndex
						Case msParcelSourceAttribTag
							miaBlockAttribIndex(4) = iAttribIndex
						Case msLegalAreaAttribTag
							miaBlockAttribIndex(5) = iAttribIndex
						Case msCalcAreaAttribTag
							miaBlockAttribIndex(6) = iAttribIndex
						Case msParcePrevAttribTag
							miaBlockAttribIndex(7) = iAttribIndex
						Case msGushPrevAttribTag
							miaBlockAttribIndex(8) = iAttribIndex
						Case msTabaPlanAttribTag
							miaBlockAttribIndex(9) = iAttribIndex
						Case msTabaMigrashAttribTag
							miaBlockAttribIndex(10) = iAttribIndex
						Case msTabaYeudAttribTag
							miaBlockAttribIndex(11) = iAttribIndex
						Case msCommentAttribTag
							miaBlockAttribIndex(12) = iAttribIndex
					End Select
				Next

				DMCommon.Functions.DispArray("01_577d", miaBlockAttribIndex)

				'	Erase saBlockAttribTag
			Else
				System.Windows.Forms.MessageBox.Show("Definition of '" & msCentroidBlockName & "' was not found", "11_181")
			End If

			'07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "UD_Parcel - Initialize")
		End Try
	End Sub
	Public Function GetLinks() As DMAcadExt.IUD_Link()
		If moPolygonScheme IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(tParcelKey.ToString(), "09_875e")
			Return moPolygonScheme.GetVectorSet()
		Else
			DMCommon.Debug.MsgBox("12_216", "moPolygonScheme Is Nothing")
		End If

		Return Nothing
	End Function
	Public Shared Sub CreateAdjoiningParcelsTable()
		moAdjoiningParcelsTable = New DataTable("AdjoiningParcels")
		With moAdjoiningParcelsTable.Columns
			.Add(msBlockFieldName, GetType(System.Int32))                  '0
			.Add(msNameFieldName, GetType(System.String))
			.Add(msMoveAreaFieldName, GetType(System.Double))
			.Add(msLegalAreaFieldName, GetType(System.Double))          '2

			.Add(TopoManager.TopoReader.msAreaFldName, GetType(System.Double))
			.Add(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName, GetType(System.Double))                  '4
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName, GetType(System.Double))
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName, GetType(System.Double))                  '6


			.Add(msBlockNeighborFieldName, GetType(System.Int32))
			.Add(msNameNeighborFieldName, GetType(System.String))             '8
			.Add(msLegalAreaNeighborFieldName, GetType(System.Double))
			.Add(msAreaNeighborFldName, GetType(System.Double))            '10
			.Add(TopoManager.TPlanGraph.TplnParcel.msToleranceNeighborFieldName, GetType(System.Double))
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaNeighborFieldName, GetType(System.Double))                '12
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeviationNeighborFieldName, GetType(System.Double))
			.Add(TopoManager.TopoReader.msTopoIDFldName, GetType(System.Int32))
			.Add(msTopoIDNeighborFldName, GetType(System.Int32))
			.Add(msParcelOrderFieldName, GetType(System.Int32))
			.Add(msParcelOrderNeighborFieldName, GetType(System.Int32))
		End With
	End Sub
	Public Shared Sub CreateMainDataTable()
		moMainDataTable = New DataTable("Parcels")
		With moMainDataTable.Columns
			.Add(msBlockFieldName, GetType(System.Int32))                 '0
			.Add(msNameFieldName, GetType(System.String))
			.Add(msLegalAreaFieldName, GetType(System.Double))          '2
			.Add(TopoManager.TopoReader.msAreaFldName, GetType(System.Double))
			.Add(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName, GetType(System.Double))                     '4
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName, GetType(System.Double))
			.Add(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName, GetType(System.Double))                     '6
			.Add(TopoManager.TopoReader.msCentroidXFldName, GetType(System.Double))
			.Add(TopoManager.TopoReader.msCentroidYFldName, GetType(System.Double)) '8

			.Add(TopoManager.TopoReader.msPerimeterFldName, GetType(System.Double))
			.Add(TopoManager.TopoReader.msTopoIDFldName, GetType(System.Int32))       '10
			.Add(TopoManager.TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))
			.Add(msParcelOrderFieldName, GetType(System.Int32))                       '12
		End With
	End Sub
	Public ReadOnly Property CalcArea() As Double
		Get
			Return mtParcelArea.CalcArea
		End Get

	End Property

	Public Function CompareArea(dArea As Double) As Boolean
		Dim dAreaA = Math.Round(0.001 * dArea, miRoundDigit)
		'CalcArea
		If CalcArea - dAreaA <> 0.0 Then

			DMAcadExt.AcadDocument.WriteMessage(UD_Name & dArea.ToString() & "; " & mtParcelArea.CalcArea.ToString() & "; " & miRoundDigit.ToString())
			DMAcadExt.AcadDocument.WriteMessage(UD_Name & " Diff area(m): " & FormatNumber(Math.Abs(1000.0 * (CalcArea - dAreaA)), 3) & "; Original: " & CalcArea.ToString & "; After Hanit: " & dAreaA.ToString)
		End If

	End Function
	Public ReadOnly Property AltBlock() As Boolean
		Get
			Return mbAltBlock
		End Get
	End Property
	Public ReadOnly Property Tolerance() As Double
		Get
			Return mtParcelArea.Tolerance
		End Get
	End Property
	Public ReadOnly Property DeltaArea() As Double
		Get
			Return mtParcelArea.DeltaArea
		End Get
	End Property
	Public ReadOnly Property DeltaAreaM() As Double
		Get
			Return mtParcelArea.DeltaAreaM
		End Get
	End Property
	Public ReadOnly Property Deviation() As Double
		Get
			Return mtParcelArea.Deviation
		End Get

	End Property
	Public ReadOnly Property HasDeviation() As Boolean
		Get
			Return mbHasDeviation
		End Get
	End Property
	Public ReadOnly Property Need() As Double
		Get
			Return mdNeed
		End Get
	End Property
	Public Shared ReadOnly Property MainView() As System.Data.DataView
		Get
			Dim sSort As String = msBlockFieldName & "," & msParcelOrderFieldName
			If moMainDataTable IsNot Nothing Then
				Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public Shared Sub PaintAllByArea()
		zzInitAreaColors()
		Dim dicParcels As Dictionary(Of Integer, UD_Parcel) = Unidiv.Parcels
		DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
		DMAcadExt.AcadTransaction.Start()
		DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)


		For Each oParcel As UD_Parcel In dicParcels.Values
			oParcel.PaintByArea()
		Next

		DMAcadExt.AcadTransaction.CloseModelSpace()
		DMAcadExt.AcadTransaction.Terminate()

		DMAcadExt.AcadDocument.CloseMessage()
		DMAcadExt.AcadDocument.Unlock()
		DMAcadExt.AcadDocument.CommandLine(True)
		DMAcadExt.AcadDocument.UpdateScreen()
	End Sub
	Public Sub PaintByArea()

		Dim tHatch As DMAcadExt.DMHatch = PaintAreaHatch(CInt(mtParcelArea.AreaStatus))
		'	DMAcadExt.AcadDocument.WriteMessage("###123 " & miAreaStatus.ToString())
		If mtParcelArea.AreaStatus <> TopoManager.TPlanGraph.enAreaStatus.Exact Then
			DMAcadExt.AcadTransaction.OpenNewAnonymBlock()
			Me.PaintHatch(DMAcadExt.PaintMethod.Hatch, tHatch)
			DMAcadExt.AcadTransaction.InsertNewBlock(True)
		End If

	End Sub
	Public ReadOnly Property IsProper() As Boolean
		Get
			Select Case mtParcelArea.AreaStatus
				Case enAreaStatus.Exact, enAreaStatus.MinusPerm, enAreaStatus.PlusPerm
					Return True
				Case Else
					Return False
			End Select
		End Get
	End Property
	Public Function CanGive() As Double
		Return mtParcelArea.CanGive()
	End Function
	Public Function CanReceive() As Double
		Return mtParcelArea.CanReceive()
	End Function
	Public Sub SetParcelKey(tParcelKey As UD_ParcelKey)
		miBlockNo = tParcelKey.BlockNo
		miBlockAdd = tParcelKey.BlockAdd
		mtParcelKey = tParcelKey
		mbIsOriginal = tParcelKey.Original
		msName = tParcelKey.UD_ParcelName
	End Sub
	Public Sub Rename(iNewNo As Integer)
		mtParcelKey.ParcelNo = iNewNo
		msName = mtParcelKey.UD_ParcelName
	End Sub
	Public Sub SetGush(iBlockNo As Integer, iBlockAdd As Integer)
		miBlockNo = iBlockNo
		miBlockAdd = iBlockAdd
		mtParcelKey.BlockNo = iBlockNo
		mtParcelKey.BlockAdd = iBlockAdd
		mbIsOriginal = mtParcelKey.Original
		msName = mtParcelKey.UD_ParcelName
	End Sub
	Public Shared Function GetMoveArea(ByVal oParcelA As UD_Parcel, ByVal oParcelB As UD_Parcel) As Double
		Dim dAtoB, dBtoA As Double
		If oParcelA.HasDeviation Then
			dAtoB = zzGetMoveArea(oParcelA, oParcelB)
		End If
		If oParcelB.HasDeviation Then
			dBtoA = -zzGetMoveArea(oParcelB, oParcelA)
		End If
		If Math.Abs(dAtoB) > Math.Abs(dBtoA) Then
			Return dAtoB
		Else
			Return dBtoA
		End If
	End Function
	Private Shared Function zzGetMoveArea(ByVal oNeed As UD_Parcel, ByVal oCan As UD_Parcel) As Double
		If oNeed.Need >= 0.0 Then
			Return Math.Round(Math.Min(oNeed.Need, oCan.CanGive()), miRoundDigit)
		Else
			Return Math.Round(Math.Max(oNeed.Need, -oCan.CanReceive()), miRoundDigit)
		End If
	End Function
	Public Shared ReadOnly Property AdjoiningParcelsView() As System.Data.DataView
		Get
			Dim sSort As String = msBlockFieldName & "," & msParcelOrderFieldName & "," & msParcelOrderNeighborFieldName
			If moAdjoiningParcelsTable IsNot Nothing Then
				Dim oDataView As System.Data.DataView = New System.Data.DataView(moAdjoiningParcelsTable, String.Empty, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = False
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Return oDataView
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public Sub AddDataToAdjoiningParcelsTable()
		Dim oNewRow As System.Data.DataRow
		Dim oNeighborParcel As UD_Parcel = Nothing
		If moAdjoiningParcelsTable IsNot Nothing Then
			Try
				DMAcadExt.AcadDocument.WriteDebugMessage("Naa Count=" & CStr(MyBase.Neighbors.Count))
				For Each iParcelID As Integer In MyBase.Neighbors.Keys
					DMAcadExt.AcadDocument.WriteDebugMessage("Nab iParcelID=" & CStr(iParcelID))
					If iParcelID <> 0 AndAlso Unidiv.TryGetParcel(iParcelID, oNeighborParcel) Then
						oNewRow = moAdjoiningParcelsTable.NewRow()
						With oNewRow
							.Item(msBlockFieldName) = Me.BlockNo
							.Item(msNameFieldName) = msName
							.Item(msMoveAreaFieldName) = GetMoveArea(Me, oNeighborParcel)
							.Item(msLegalAreaFieldName) = Me.LegalArea

							.Item(TopoManager.TopoReader.msAreaFldName) = Me.CalcArea
							.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = Me.Tolerance
							.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = Me.DeltaArea
							.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = Me.Deviation
							.Item(TopoManager.TopoReader.msTopoIDFldName) = Me.TopoID
							.Item(msParcelOrderFieldName) = Me.Order

							.Item(msBlockNeighborFieldName) = oNeighborParcel.BlockNo
							.Item(msNameNeighborFieldName) = oNeighborParcel.Name
							.Item(msLegalAreaNeighborFieldName) = oNeighborParcel.LegalArea
							.Item(msAreaNeighborFldName) = oNeighborParcel.CalcArea
							.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceNeighborFieldName) = oNeighborParcel.Tolerance
							.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaNeighborFieldName) = oNeighborParcel.DeltaArea
							.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationNeighborFieldName) = oNeighborParcel.Deviation
							.Item(msTopoIDNeighborFldName) = oNeighborParcel.TopoID

						End With
						moAdjoiningParcelsTable.Rows.Add(oNewRow)
					End If
				Next

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - AddDataToMainTable_11")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
		End If

	End Sub
	Public Overrides Function ToString() As String
		Return mtParcelKey.ToString()

	End Function
	Public Sub Undo(sInitLayer As String)
		'	DMCommon.Debug.MsgBox("12_877", dbAcadPoint, mbHasNewBlock, mtParcelKey)
		If Not dtCentroidAcObjID.IsNull Then
			'DMCommon.Debug.MsgBox("12_877AA", dbAcadPoint, mbHasNewBlock, mtParcelKey, msSourceCentroidLayer, sInitLayer, dtCentroidAcObjID, dtBorderAcObjID)
			If mbHasNewBlock Then

				DMAcadExt.AcadTransaction.EraseDBObject(dtCentroidAcObjID)
			Else
				If msSourceCentroidLayer = sInitLayer Then


					DMAcadExt.AcadTransaction.SetLayer(dtCentroidAcObjID, sInitLayer)

				End If
				msPrevGushName = Nothing
				msPrevParcelName = Nothing
				If Not mtParcelArea.LegalAreaFromBlock Then
					mtParcelArea.LegalArea = 0.0
				End If

				miBlockNo = 0
				miBlockAdd = 0
				UpdateBlockAttributes()
			End If

		End If


		If Not dtBorderAcObjID.IsNull Then
			DMAcadExt.AcadTransaction.EraseDBObject(dtBorderAcObjID)

		End If
	End Sub
	Public Sub Restore()
		If mbIsCanceled Then
			mbIsCanceled = False
			UpdateBlockAttributes()
		End If
	End Sub
	Public Sub AddDataToMainTable()
		Dim oNewRow As System.Data.DataRow
		If moMainDataTable IsNot Nothing Then
			Try
				oNewRow = moMainDataTable.NewRow()
				With oNewRow
					.Item(msBlockFieldName) = Me.BlockNo
					.Item(msNameFieldName) = msName
					.Item(msLegalAreaFieldName) = Me.LegalArea
					.Item(TopoManager.TopoReader.msAreaFldName) = Me.CalcArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msToleranceFieldName) = Me.Tolerance
					.Item(TopoManager.TPlanGraph.TplnParcel.msDeltaAreaFieldName) = Me.DeltaArea
					.Item(TopoManager.TPlanGraph.TplnParcel.msDeviationFieldName) = Me.Deviation
					.Item(TopoManager.TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
					.Item(TopoManager.TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
					.Item(TopoManager.TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
					.Item(TopoManager.TopoReader.msTopoIDFldName) = MyBase.TopoID
					.Item(TopoManager.TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID '''''''''''.OldIdPtr.ToInt64

					.Item(msParcelOrderFieldName) = miOrder
				End With
				moMainDataTable.Rows.Add(oNewRow)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnParcel - AddDataToMainTable_3")
			End Try
		Else
			System.Windows.Forms.MessageBox.Show("moDataTable Is Nothing", "AddDataToMainTable_2")
		End If

	End Sub
	Private Sub zzGetParcelName(ByVal sAttribValue As String)
		Dim oComplexNum As TopoManager.NumerationPair.ComplexNum
		msName = sAttribValue.Trim()
		If msName.Length <> 0 Then

			Try
				oComplexNum = New TopoManager.NumerationPair.ComplexNum(msName)
				miOrder = oComplexNum.Order
				mtParcelKey = New UD_ParcelKey(oComplexNum.Base, False)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, " zzGetParcelName")
			End Try
		End If
	End Sub
	Private Sub zzGetLotName(ByVal sAttribValue As String)

		msLotName = sAttribValue.Trim()

	End Sub
	Private Sub zzGetBlockNo(ByVal sAttribValue As String)
		Try
			'System.Windows.Forms.MessageBox.Show(sAttribValue, " 01_777")
			If sAttribValue.Length <> 0 Then
				TopoManager.TPlanGraph.TplnBlock.GetBlockNo(sAttribValue, miBlockNo, miBlockAdd)

				'	miBlockNo = TopoManager.Common.NumberFilter(sAttribValue)
			End If

		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_808")
		End Try
	End Sub
	Private Sub zzSetDbID()
		miLastDbID += 1
		miDbID = miLastDbID
	End Sub
   Private Sub zzGetLegalArea(ByVal sAttribValue As String)
		Try
			Dim dLegalAreaD As Double
			Dim dLegalAreaM As Double

			Dim bLegalAreaFromBlock As Boolean
			If sAttribValue.Length <> 0 Then

				If Double.TryParse(sAttribValue, dLegalAreaD) Then
					If dLegalAreaD <> 0 Then
						bLegalAreaFromBlock = True
						dLegalAreaM = 1000.0 * dLegalAreaD
					End If
					mtParcelArea.SetLegalArea(dLegalAreaM, bLegalAreaFromBlock)
				End If
			End If

			'DMCommon.ExcelLog.SetNextValue(0, "ParArea0", msName, sAttribValue, dLegalArea, mtParcelArea.LegalArea)
		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_809")
      End Try
   End Sub
   Private Sub zzGetCalcArea(ByVal sAttribValue As String)

   End Sub
   Private Sub zzGetParcelPrev(ByVal sAttribValue As String)
      Try
         If sAttribValue.Length <> 0 Then
            msPrevParcelName = sAttribValue
         End If

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub

   Private Sub zzGetGushPrev(ByVal sAttribValue As String)
      Try
         If sAttribValue.Length <> 0 Then
            msPrevGushName = sAttribValue
         End If

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub
   Private Sub zzGetPlan(ByVal sAttribValue As String)
      Try
         If sAttribValue.Length <> 0 Then
				msPlan = sAttribValue
				msPlanDos = sAttribValue

			End If

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub

   Private Sub zzGetLot(ByVal sAttribValue As String)
      Try
         If sAttribValue.Length <> 0 Then
				msLot = sAttribValue
				msLotDos = sAttribValue
				mbPlanDataFromBlock = True

			End If

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub
   Private Sub zzGetLanduse(ByVal sAttribValue As String)
      Try
			If sAttribValue.Length <> 0 Then
				msLanduseNameDos = sAttribValue

				If Asc(sAttribValue) < 160 Then
					Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(sAttribValue, False)
					msLanduseName = oHebText.GetWinDest(False)
				Else
					msLanduseName = sAttribValue
				End If
				mbPlanDataFromBlock = True
				'DMCommon.ExcelLog.SetNextValue(3, "NB0a", sAttribValue, msLanduseNameDos, Asc(sAttribValue), msLanduseName)

				'	DMCommon.ExcelLogAW1.SetNextValue(0, UD_Name, sAttribValue, msLanduseName, Asc(sAttribValue), AscW(sAttribValue))
			End If

			' DMCommon.Debug.MsgBox("12_176", sAttribValue, LanduseName)
		Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub
   Private Sub zzGetComment(ByVal sAttribValue As String)
      Try
         If sAttribValue.Length <> 0 Then
            msComment = sAttribValue
         End If

      Catch oEx As Exception
         DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_811")
      End Try
   End Sub

    



   






   Private Shared Sub zzInitAreaColors()

      PaintAreaHatch(0) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 61)))
      PaintAreaHatch(1) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 3)))
      PaintAreaHatch(2) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 60)))
      PaintAreaHatch(3) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 4)))
      PaintAreaHatch(4) = New DMAcadExt.DMHatch(New DMAcadExt.DMColor(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 51)))

   End Sub

   Public Overrides Sub Terminate()
      MyBase.OnTerminate()
   End Sub

   Protected Overrides Sub Finalize()
      MyBase.Finalize()
   End Sub

   Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
      Get
         Return miaBlockAttribIndex
      End Get
   End Property

   Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
   End Property
End Class

