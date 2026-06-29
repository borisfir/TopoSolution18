Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports TopoManager.TPlanGraph
Imports UnidivNet

Public Class UD_Parcels
	Inherits Dictionary(Of UD_ParcelKey, UD_Parcel)

	Const MAX_OPERATION_NO As Integer = 200
	Const msUD_ParcelBlockName As String = "LB_PARCEL1"
	Const msBaseBlockName As String = "UD_GushNo"

	Const msCancelParcelLayer As String = "UD_DEL_PARCLS"
	Const msResultParcelLayer As String = "UD_ACT_PARCLS"

	Const msBlockNoTag As String = "LOT_NUM"
	Const msBlockAddTag As String = "GUSH_SUFFI"
	Const msBlockPath As String = "M:\Dm_Work\Blocks\Hanit"
	Const msCentroidBlockName As String = "C1603"

	'  Private mdicParcels As Dictionary(Of Integer,
	Private miBaseBlockNo As Integer, miBaseBlockAdd As Integer
	'	Private moTestNewParcel As UD_Parcel
	'  Private moFragmentsTopo As Autodesk.Gis.Map.Topology.TopologyModel
	Private mdicTopoIDByStage As Dictionary(Of Integer, UD_ParcelKey)
	'''''''''''''''''''''' Private mdicFragments As Dictionary(Of Integer, UD_ParcelKey)
	Private mdicParcelByObjId As Dictionary(Of ObjectId, UD_Parcel)
	Private mdicParcelByDbID As Dictionary(Of Integer, UD_Parcel)



	Private mcolParcelsByStage As System.Collections.ObjectModel.Collection(Of UD_Parcel)
	Private mhsParcelsSelected As HashSet(Of UD_Parcel)
	Private mtaOperation(MAX_OPERATION_NO) As UD_Operation
	Private mcolTransferBlocks As System.Collections.ObjectModel.Collection(Of TransferBlock)
	Private mtCurrentTransferBlock As TransferBlock '= New TransferBlock()
	Private moParcelsScheme As TopoManager.TopoScheme.tsTopology

	Private Enum enUD_ParcelCentroidAttribIndices
		ParcelNum
		UB = ParcelNum
	End Enum
	Private Enum enUD_BaseBlockAttribIndices
		BlockNo
		BlockAddNo
		UB = BlockAddNo
	End Enum
	Public Sub InitHanit()
		zzLoadBaseBlock()
		UD_ParcelKey.BaseBlockNo = miBaseBlockNo
		UD_ParcelKey.BaseBlockAdd = miBaseBlockAdd

	End Sub
	Public Sub LoadTopoScheme(ByRef oParcelTopology As Autodesk.Gis.Map.Topology.TopologyModel)
		moParcelsScheme = New TopoManager.TopoScheme.tsTopology()
		moParcelsScheme.Load(False, oParcelTopology)
		For Each oParcel In MyBase.Values
			oParcel.PolygonScheme = moParcelsScheme.GetPolygon(oParcel.TopoID)

		Next
		'  System.Windows.Forms.MessageBox.Show(moParcelsScheme.Elements.Count.ToString() & vbCrLf & moParcelsScheme.Polygons.Count.ToString() & vbCrLf & MyBase.Count, "09_367a")
	End Sub
	Public Sub AddSelected(oParcel As UD_Parcel)
		If Not oParcel.Selected Then
			oParcel.Selected = True
			mhsParcelsSelected.Add(oParcel)
		End If
	End Sub
	Public ReadOnly Property AcObjIDsCount As Integer
		Get
			Return mdicParcelByObjId.Count
		End Get
	End Property


	Public ReadOnly Property DbIDsCount As Integer
		Get
			Return mdicParcelByDbID.Count
		End Get
	End Property
	Public ReadOnly Property ContainsDbID(iDbID As Integer) As Boolean
		Get
			Return mdicParcelByDbID.ContainsKey(iDbID)
		End Get
	End Property

	Public Sub ClearSelected()
		For Each oParcel As UD_Parcel In mhsParcelsSelected
			oParcel.Selected = False
		Next
		mhsParcelsSelected.Clear()
	End Sub
	Public Sub GetString(bLastStage0 As Boolean, ByRef sNormal As String, ByRef sTemp As String)
		Dim oNormalNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
		Dim oTempNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)

		'   Dim oTempNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.LeftToRight, "[", "]")
		Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = Me.GetSorted()
		If colRes IsNot Nothing Then
			TopoManager.NumerationPair.SetGroupDelim(TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
			For Each oParcel As UD_Parcel In colRes
				'DMAcadExt.AcadDocument.WriteMessage("!##-- " & oParcel.ParcelKey.ToString())
				If oParcel.Stage = 0 OrElse (bLastStage0 AndAlso Not oParcel.IsOut) Then
					If oParcel.IsOriginal Then
						oNormalNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					Else
						oTempNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					End If
				End If
			Next
			sNormal = oNormalNumeration.GetPresentation()
			sTemp = oTempNumeration.GetPresentation()
			'   sNormal = ""mh
			'  System.Windows.Forms.MessageBox.Show(oNormalNumeration.GetPresentation() & vbCrLf & oTempNumeration.GetPresentation(), "09_067")

		Else
			sNormal = Nothing
			sTemp = Nothing
		End If

	End Sub
	Public Sub GetInitParcelString(ByVal bAll As Boolean, ByRef sNormal As String, ByRef sTemp As String, ByRef iIncomingParcelsCount As Integer, ByRef iOutgoingParcelsCount As Integer)
		Dim oNormalNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
		Dim oTempNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
		Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = Me.GetSorted()

		If colRes IsNot Nothing Then
			TopoManager.NumerationPair.SetGroupDelim(TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!AllPARC+oParcel.", "ParcelKey", "DbID", "LegalArea", "IsActive", "IsCanceled", "IsOut", "IsOriginal", "IsProper", "IsResult")
			iIncomingParcelsCount = 0
			iOutgoingParcelsCount = 0
			For Each oParcel As UD_Parcel In colRes

				If oParcel.Stage = 0 AndAlso (bAll OrElse oParcel.IsCanceled) Then
					If oParcel.IsOriginal Then
						oNormalNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					Else
						oTempNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					End If
					iIncomingParcelsCount += 1
				End If
				If oParcel.IsResult Then
					iOutgoingParcelsCount += 1
				End If
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!AllPARC", oParcel.ParcelKey, oParcel.DbID, oParcel.LegalArea, oParcel.IsActive, oParcel.IsCanceled, oParcel.IsOut, oParcel.IsOriginal, oParcel.IsProper, oParcel.IsResult)
			Next
			sNormal = oNormalNumeration.GetPresentation()
			sTemp = oTempNumeration.GetPresentation()


		Else
			sNormal = Nothing
			sTemp = Nothing
		End If

	End Sub
	Public Sub ClearAll()
		MyBase.Clear()
		mdicParcelByObjId.Clear()
		mdicParcelByDbID.Clear()

		mdicTopoIDByStage.Clear()
	End Sub
	Public Sub GetGushString(iBlockNo As Integer, iBlockAdd As Integer, ByRef sNormal As String, ByRef sTemp As String)
		Dim oNormalNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
		Dim oTempNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)

		'   Dim oTempNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.NotExists, TopoManager.NumerationPair.enTextDirection.LeftToRight, "[", "]")
		Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = Me.GetSorted()
		If colRes IsNot Nothing Then
			TopoManager.NumerationPair.SetGroupDelim(TopoManager.NumerationPair.enTextDirection.FrameWorkRtoL)
			For Each oParcel As UD_Parcel In colRes
				'DMAcadExt.AcadDocument.WriteMessage("!##-- " & oParcel.ParcelKey.ToString())
				If Not oParcel.IsCanceled AndAlso oParcel.BlockNo = iBlockNo AndAlso oParcel.BlockAdd = iBlockAdd Then
					If oParcel.IsOriginal Then
						oNormalNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					Else
						oTempNumeration.Add(Convert.ToString(oParcel.ParcelKey.ParcelNo))
					End If
				End If
			Next
			sNormal = oNormalNumeration.GetPresentation()
			sTemp = oTempNumeration.GetPresentation()
			'   sNormal = ""mh
			'  System.Windows.Forms.MessageBox.Show(oNormalNumeration.GetPresentation() & vbCrLf & oTempNumeration.GetPresentation(), "09_067")

		Else
			sNormal = Nothing
			sTemp = Nothing
		End If

	End Sub
	Public Function ContainsAcObjID(tAcObjID As ObjectId) As Boolean
		Return mdicParcelByObjId.ContainsKey(tAcObjID)
	End Function
	Public Function GetParcelLinks(tParcelKey As UD_ParcelKey) As DMAcadExt.IUD_Link()
		Dim oParcel As UD_Parcel = Nothing
		Dim oPgonScheme As TopoManager.TopoScheme.tsPolygon
		If MyBase.TryGetValue(tParcelKey, oParcel) Then
			oPgonScheme = moParcelsScheme.GetPolygon(oParcel.TopoID)
			If oPgonScheme IsNot Nothing Then
				' System.Windows.Forms.MessageBox.Show(tParcelKey.ToString(), "09_875n")
				Return oPgonScheme.GetVectorSet()
			End If
		End If
		Return Nothing
	End Function
	Public Function GetParcelLinks(oParcel As UD_Parcel) As DMAcadExt.IUD_Link()

		Dim oPgonScheme As TopoManager.TopoScheme.tsPolygon

		oPgonScheme = oParcel.PolygonScheme
		If oPgonScheme IsNot Nothing Then
			' System.Windows.Forms.MessageBox.Show(tParcelKey.ToString(), "09_875e")
			Return oPgonScheme.GetVectorSet()
		End If

		Return Nothing
	End Function
	Public Function GetMaxTempParcelNo() As Integer
		Dim iResParcelNo As Integer = 0
		For Each tParcelKey As UD_ParcelKey In MyBase.Keys
			If Not tParcelKey.Original Then
				If iResParcelNo < tParcelKey.ParcelNo Then
					iResParcelNo = tParcelKey.ParcelNo
				End If
			End If
		Next
		Return iResParcelNo
	End Function
	Public Function GetSorted() As System.Collections.ObjectModel.Collection(Of UD_Parcel)
		Dim oParcelKeyComparer As ParcelKeyComparer = New ParcelKeyComparer()
		Dim oParcelComparer As ParcelComparer = New ParcelComparer()

		Dim oList As List(Of UD_ParcelKey) = New List(Of UD_ParcelKey)(MyBase.Keys)
		Dim oParcelList As List(Of UD_Parcel) = New List(Of UD_Parcel)(MyBase.Values)

		Dim colRes As System.Collections.ObjectModel.Collection(Of UD_Parcel) = New System.Collections.ObjectModel.Collection(Of UD_Parcel)()
		'	Dim oParcel As UD_Parcel = Nothing

		'For Each tParcelKey As UD_ParcelKey In oList
		'   If MyBase.TryGetValue(tParcelKey, oParcel) Then
		'      DMAcadExt.AcadDocument.WriteMessage("##-- " & tParcelKey.ToString())

		'   End If

		'Next
		If False Then
			Dim oParcel As UD_Parcel = Nothing
			oList.Sort(oParcelKeyComparer)
			For Each tParcelKey As UD_ParcelKey In oList
				If MyBase.TryGetValue(tParcelKey, oParcel) Then
					'DMAcadExt.AcadDocument.WriteMessage("##!! " & tParcelKey.ToString())
					colRes.Add(oParcel)
				End If

			Next
		End If
		oParcelList.Sort(oParcelComparer)
		For Each oParcel As UD_Parcel In oParcelList


			colRes.Add(oParcel)


		Next

		Return colRes
	End Function

	Public Function GetBook() As String
		Dim iOperationNo As Integer = 0
		Dim tOperation As UD_Operation
		Dim sResText As String = String.Empty
		Dim sTableCaption As String
		Dim oParcel As UD_Parcel
		Dim iaBookLineFormat() As Integer = {8, 11, 14, 14, 15}
		Dim iaBookLineTransferFormat() As Integer = {8, 11, 14, 14, 15, 12, 15, 15}

		Dim oTextLine As DMCommon.dmTextLine
		Dim dCalcArea As Double
		Dim sHeader1 As String = " T number   F number   Calc area     area Diff'     Final area"
		Dim sHeader2 As String = " --------   --------   ----------    ----------     ----------"
		Dim sBottomDel As String = " -------------------------------------------------------------"
		Dim sTransferHeader1 As String = " T number   F number   Calc area     area Diff'     Final area       Block         TN number     FN number"
		Dim sTransferHeader2 As String = " --------   --------   ----------    ----------     ----------     ----------     ----------     ----------"
		Dim sTransferBottomDel As String = " ----------------------------------------------------------------------------------------------------------"
		Dim tSingleParcelKey As UD_ParcelKey
		Dim sAllowed As String
		Dim sRepCaption As String
		Dim oFileInfo As IO.FileInfo
		Dim iTableNo As Integer = 0
		'File name:Tr_book.txt     Place:  Gush:  Print Date:12/10/2015
		' 
		oFileInfo = New IO.FileInfo(DMAcadExt.AcadDocument.GetFileName())
		sRepCaption = "File name:" & oFileInfo.Name & " Print Date:" & FormatDateTime(Date.Today, DateFormat.ShortDate)
		sResText &= sRepCaption
		Do
			If iOperationNo > MAX_OPERATION_NO Then
				Exit Do
			End If
			tOperation = mtaOperation(iOperationNo)

			If tOperation.IsEmpty OrElse tOperation.ActionType = enActionType.Transfer Then
				'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number), "09_007")
				Exit Do
			End If
			'  System.Windows.Forms.MessageBox.Show(CStr(iOperationNo) & vbCrLf & CStr(tOperation.ParcelList.Count), "09_001")
			sResText &= vbCrLf & vbCrLf & vbCrLf
			iTableNo += 1
			sTableCaption = " Table " & CStr(iTableNo) & "    " & tOperation.ActionName & vbCrLf
			sResText &= sTableCaption & vbCrLf
			sResText &= sHeader1 & vbCrLf
			sResText &= sHeader2
			'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number) & vbCrLf & CStr(tOperation.ParcelList.Count), "09_014")
			For Each tParcelKey As UD_ParcelKey In tOperation.ParcelSet
				oParcel = MyBase.Item(tParcelKey)
				oTextLine = New DMCommon.dmTextLine(iaBookLineFormat)
				If tParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tParcelKey.ParcelNo)
				End If
				dCalcArea = Math.Round(oParcel.CalcArea, 5)
				oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)

				sResText &= vbCrLf & oTextLine.LineValue
			Next
			sResText &= vbCrLf & sBottomDel
			tSingleParcelKey = tOperation.SingleParcel
			If tSingleParcelKey.ParcelNo <> 0 Then
				oTextLine = New DMCommon.dmTextLine(iaBookLineFormat)

				If tSingleParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tSingleParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tSingleParcelKey.ParcelNo)
				End If
				Try
					oParcel = MyBase.Item(tSingleParcelKey)
					dCalcArea = Math.Round(oParcel.CalcArea, 5)
					oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
					oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
					oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)
					sResText &= vbCrLf & oTextLine.LineValue
					sAllowed = " Allowed Diff' (" & oParcel.ParcelArea.Formula & ") = " & FormatNumber(oParcel.ParcelArea.Tolerance, 5, TriState.True, TriState.False, TriState.False)
					' sAllowed = "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"
					sResText &= vbCrLf & vbCrLf & sAllowed
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(CStr(tSingleParcelKey.ToString()) & vbCrLf & oEx.ToString, "09_104")
				End Try
			End If
			'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number) & vbCrLf & sResText, "09_002")
			iOperationNo += 1
		Loop
		Dim tSourceParcelKey As UD_ParcelKey
		Dim tDestceParcelKey As UD_ParcelKey
		'   System.Windows.Forms.MessageBox.Show(CStr(mcolTransferBlocks.Count) & vbCrLf & "", "09_003")
		For Each tTransferBlock As TransferBlock In mcolTransferBlocks

			sResText &= vbCrLf & vbCrLf & vbCrLf
			iTableNo += 1
			sTableCaption = " Table " & CStr(iTableNo) & "    " & "TRANSFER" & vbCrLf
			sResText &= sTableCaption & vbCrLf
			sResText &= sTransferHeader1 & vbCrLf
			sResText &= sTransferHeader2
			'  System.Windows.Forms.MessageBox.Show(CStr(tTransferBlock.SourceParcelList.Count) & vbCrLf & "", "09_004")

			For iIndex As Integer = 0 To tTransferBlock.SourceParcelList.Count - 1
				oTextLine = New DMCommon.dmTextLine(iaBookLineTransferFormat)
				tSourceParcelKey = tTransferBlock.SourceParcelList.Item(iIndex)
				If tSourceParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tSourceParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tSourceParcelKey.ParcelNo)
				End If

				oParcel = MyBase.Item(tSourceParcelKey)
				dCalcArea = Math.Round(oParcel.CalcArea, 5)
				oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(5) = FormatNumber(tTransferBlock.BlockNo, 0, TriState.True, TriState.False, TriState.False)
				tDestceParcelKey = tTransferBlock.ParcelList.Item(iIndex)

				If tDestceParcelKey.Original Then
					oTextLine.FieldValue(7) = Convert.ToString(tDestceParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(6) = Convert.ToString(tDestceParcelKey.ParcelNo)
				End If
				sResText &= vbCrLf & oTextLine.LineValue

			Next
			sResText &= vbCrLf & sTransferBottomDel
			sResText &= vbCrLf & vbCrLf & sRepCaption

		Next
		'  System.Windows.Forms.MessageBox.Show(sResText, "09_200")
		Return sResText
	End Function

	Public Function GetAreaTable() As System.Data.DataView
		Dim iOperationNo As Integer = 0
		Dim tOperation As UD_Operation
		Dim sResText As String = String.Empty
		Dim sTableCaption As String
		Dim oParcel As UD_Parcel
		Dim iaBookLineFormat() As Integer = {8, 11, 14, 14, 15}
		Dim iaBookLineTransferFormat() As Integer = {8, 11, 14, 14, 15, 12, 15, 15}

		Dim oTextLine As DMCommon.dmTextLine
		Dim dCalcArea As Double
		Dim sHeader1 As String = " T number   F number   Calc area     area Diff'     Final area"
		Dim sHeader2 As String = " --------   --------   ----------    ----------     ----------"
		Dim sBottomDel As String = " -------------------------------------------------------------"
		Dim sTransferHeader1 As String = " T number   F number   Calc area     area Diff'     Final area       Block         TN number     FN number"
		Dim sTransferHeader2 As String = " --------   --------   ----------    ----------     ----------     ----------     ----------     ----------"
		Dim sTransferBottomDel As String = " ----------------------------------------------------------------------------------------------------------"
		Dim tSingleParcelKey As UD_ParcelKey
		Dim sAllowed As String
		Dim sRepCaption As String
		Dim oFileInfo As IO.FileInfo
		Dim iTableNo As Integer = 0
		'File name:Tr_book.txt     Place:  Gush:  Print Date:12/10/2015
		' 
		oFileInfo = New IO.FileInfo(DMAcadExt.AcadDocument.GetFileName())
		sRepCaption = "File name:" & oFileInfo.Name & " Print Date:" & FormatDateTime(Date.Today, DateFormat.ShortDate)
		sResText &= sRepCaption
		Do
			If iOperationNo > MAX_OPERATION_NO Then
				Exit Do
			End If
			tOperation = mtaOperation(iOperationNo)

			If tOperation.IsEmpty OrElse tOperation.ActionType = enActionType.Transfer Then
				'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number), "09_007")
				Exit Do
			End If
			'  System.Windows.Forms.MessageBox.Show(CStr(iOperationNo) & vbCrLf & CStr(tOperation.ParcelList.Count), "09_001")
			sResText &= vbCrLf & vbCrLf & vbCrLf
			iTableNo += 1
			sTableCaption = " Table " & CStr(iTableNo) & "    " & tOperation.ActionName & vbCrLf
			sResText &= sTableCaption & vbCrLf
			sResText &= sHeader1 & vbCrLf
			sResText &= sHeader2
			'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number) & vbCrLf & CStr(tOperation.ParcelList.Count), "09_014")
			For Each tParcelKey As UD_ParcelKey In tOperation.ParcelSet
				oParcel = MyBase.Item(tParcelKey)
				oTextLine = New DMCommon.dmTextLine(iaBookLineFormat)
				If tParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tParcelKey.ParcelNo)
				End If
				dCalcArea = Math.Round(oParcel.CalcArea, 5)
				oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)

				sResText &= vbCrLf & oTextLine.LineValue
			Next
			sResText &= vbCrLf & sBottomDel
			tSingleParcelKey = tOperation.SingleParcel
			If tSingleParcelKey.ParcelNo <> 0 Then
				oTextLine = New DMCommon.dmTextLine(iaBookLineFormat)

				If tSingleParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tSingleParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tSingleParcelKey.ParcelNo)
				End If
				Try
					oParcel = MyBase.Item(tSingleParcelKey)
					dCalcArea = Math.Round(oParcel.CalcArea, 5)
					oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
					oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
					oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)
					sResText &= vbCrLf & oTextLine.LineValue
					sAllowed = " Allowed Diff' (" & oParcel.ParcelArea.Formula & ") = " & FormatNumber(oParcel.ParcelArea.Tolerance, 5, TriState.True, TriState.False, TriState.False)
					' sAllowed = "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS"
					sResText &= vbCrLf & vbCrLf & sAllowed
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(CStr(tSingleParcelKey.ToString()) & vbCrLf & oEx.ToString, "09_104")
				End Try
			End If
			'  System.Windows.Forms.MessageBox.Show(CStr(tOperation.Number) & vbCrLf & sResText, "09_002")
			iOperationNo += 1
		Loop
		Dim tSourceParcelKey As UD_ParcelKey
		Dim tDestceParcelKey As UD_ParcelKey
		'   System.Windows.Forms.MessageBox.Show(CStr(mcolTransferBlocks.Count) & vbCrLf & "", "09_003")
		For Each tTransferBlock As TransferBlock In mcolTransferBlocks

			sResText &= vbCrLf & vbCrLf & vbCrLf
			iTableNo += 1
			sTableCaption = " Table " & CStr(iTableNo) & "    " & "TRANSFER" & vbCrLf
			sResText &= sTableCaption & vbCrLf
			sResText &= sTransferHeader1 & vbCrLf
			sResText &= sTransferHeader2


			For iIndex As Integer = 0 To tTransferBlock.SourceParcelList.Count - 1
				oTextLine = New DMCommon.dmTextLine(iaBookLineTransferFormat)
				tSourceParcelKey = tTransferBlock.SourceParcelList.Item(iIndex)
				If tSourceParcelKey.Original Then
					oTextLine.FieldValue(1) = Convert.ToString(tSourceParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(0) = Convert.ToString(tSourceParcelKey.ParcelNo)
				End If

				oParcel = MyBase.Item(tSingleParcelKey)
				dCalcArea = Math.Round(oParcel.CalcArea, 5)
				oTextLine.FieldValue(2) = FormatNumber(dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(3) = FormatNumber(oParcel.LegalArea - dCalcArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(4) = FormatNumber(oParcel.LegalArea, 5, TriState.True, TriState.False, TriState.False)
				oTextLine.FieldValue(5) = FormatNumber(tTransferBlock.BlockNo, 0, TriState.True, TriState.False, TriState.False)
				tDestceParcelKey = tTransferBlock.ParcelList.Item(iIndex)

				If tDestceParcelKey.Original Then
					oTextLine.FieldValue(7) = Convert.ToString(tDestceParcelKey.ParcelNo)
				Else
					oTextLine.FieldValue(6) = Convert.ToString(tDestceParcelKey.ParcelNo)
				End If
				sResText &= vbCrLf & oTextLine.LineValue

			Next
			sResText &= vbCrLf & sTransferBottomDel
			sResText &= vbCrLf & vbCrLf & sRepCaption

		Next
		'  System.Windows.Forms.MessageBox.Show(sResText, "09_200")
		Return Nothing
	End Function
	Public Sub AddTransferBlock()
		If mtCurrentTransferBlock.BlockNo <> 0 Then
			mcolTransferBlocks.Add(mtCurrentTransferBlock)
		End If
	End Sub

	Public Sub Test()
		For Each tParcelKey As UD_ParcelKey In Me.Keys
			If tParcelKey.BlockNo < 0 Then
				System.Windows.Forms.MessageBox.Show(tParcelKey.ToString(), "04_080")
			End If
		Next

	End Sub
	Public Sub AddFragment(iFragmentID As Integer, tParcelKey As UD_ParcelKey)
		''  System.Windows.Forms.MessageBox.Show(oParcel.Name.ToString() & vbCrLf & oParcel.ParcelKey.ToString(), "07_523")

		''''''''''  mdicFragments.Add(iFragmentID, tParcelKey)

	End Sub
	Public Sub AddCentroid(tCentroidAcObjID As ObjectId, oParcel As UD_Parcel)
		If Not tCentroidAcObjID.IsNull Then
			mdicParcelByObjId.Add(oParcel.CentroidAcObjID, oParcel)
		End If
	End Sub
	Public Sub AddParcel(oParcel As UD_Parcel)
		If oParcel.ParcelKey.Exists Then
			Dim oParcelEx As UD_Parcel = Nothing
			If MyBase.TryGetValue(oParcel.ParcelKey, oParcelEx) Then
				DMAcadExt.DMApp.MsgBox("#769 AddParcel", oParcel.ParcelKey.ToString & " alreadyExists", oParcelEx.CentroidPoint2d, oParcel.CentroidPoint2d, MyBase.Count, mdicParcelByObjId.Count, mdicParcelByDbID.Count)
			Else
				Try
					MyBase.Add(oParcel.ParcelKey, oParcel)
				Catch oEx As Exception
					DMCommon.Debug.MsgBox("09_048", oEx.Message, MyBase.Count, oParcel.ParcelKey, oParcel.CentroidAcObjID)
				End Try
			End If

			Try
				If Not oParcel.CentroidAcObjID.IsNull Then
					mdicParcelByObjId.Add(oParcel.CentroidAcObjID, oParcel)
				End If
				If oParcel.DbID <> 0 Then
					Try
						mdicParcelByDbID.Add(oParcel.DbID, oParcel)
					Catch oEx As Exception
						DMCommon.Debug.MsgBox("09_049d", oEx.Message, MyBase.Count, oParcel.ParcelKey, oParcel.CentroidAcObjID, oParcel.DbID)
					End Try
				End If

			Catch oEx As Exception
				DMCommon.Debug.MsgBox("09_049c", oEx.Message, mdicParcelByObjId.Count, oParcel.ParcelKey, oParcel.CentroidAcObjID)
			End Try
		Else
			DMCommon.Debug.MsgBox("09_055", oParcel.ParcelKey.UD_ParcelName, oParcel.ParcelKey.Exists)
		End If
	End Sub
	Public Sub AddParcelDbID(oParcel As UD_Parcel)
		''  System.Windows.Forms.MessageBox.Show(oParcel.Name.ToString() & vbCrLf & oParcel.ParcelKey.ToString(), "07_523")
		If oParcel.DbID <> 0 Then
			Try
				mdicParcelByDbID.Add(oParcel.DbID, oParcel)
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("09_059e", oEx.Message, MyBase.Count, oParcel.ParcelKey, oParcel.CentroidAcObjID, oParcel.DbID)
			End Try

		End If

	End Sub
	Public Sub AddParcelDbID(iDbID As Integer, oParcel As UD_Parcel)
		''  System.Windows.Forms.MessageBox.Show(oParcel.Name.ToString() & vbCrLf & oParcel.ParcelKey.ToString(), "07_523")
		If iDbID <> 0 Then
			Try
				mdicParcelByDbID.Add(oParcel.DbID, oParcel)
			Catch oEx As Exception
				DMCommon.Debug.MsgBox("09_059f", oEx.Message, MyBase.Count, oParcel.ParcelKey, oParcel.CentroidAcObjID, oParcel.DbID)
			End Try

		End If

	End Sub

	Public Sub ClearByDbID()
		mdicParcelByDbID.Clear()
	End Sub
	Public Sub RemoveByDbID(iDbID As Integer)
		If mdicParcelByDbID.ContainsKey(iDbID) Then
			mdicParcelByDbID.Remove(iDbID)
		End If
	End Sub
	Public Sub AddByDbID(oParcel As UD_Parcel)

		mdicParcelByDbID.Add(oParcel.DbID, oParcel)

	End Sub
	Public Sub Debug1(sCaption As String)
		For Each oParcel As UnidivNet.UD_Parcel In mdicParcelByDbID.Values
			DMCommon.Debug.ExcelLog.SetNextValue(0, sCaption, oParcel.ParcelKey, oParcel.DbID, mdicParcelByDbID.ContainsKey(oParcel.DbID), oParcel.TopoID, oParcel.LegalArea)
		Next
	End Sub


	Public Sub RemoveParcel(oParcel As UD_Parcel)
		''  System.Windows.Forms.MessageBox.Show(oParcel.Name.ToString() & vbCrLf & oParcel.ParcelKey.ToString(), "07_523")
		If oParcel.ParcelKey.Exists Then
			'  DMCommon.Debug.MsgBox("09_044r", oParcel.ParcelKey.ToString)
			MyBase.Remove(oParcel.ParcelKey)
			mdicParcelByObjId.Remove(oParcel.CentroidAcObjID)
			Try
				mdicParcelByDbID.Remove(oParcel.DbID)
			Catch oEx As Exception

			End Try


		Else
			DMCommon.Debug.MsgBox("09_059", oParcel.ParcelKey.UD_ParcelName, oParcel.ParcelKey.Exists)
		End If
	End Sub
	Public Sub AddOper(oOperDetail As UD_OperDetail)
		Dim oOldParcel As UD_Parcel = Nothing
		Dim oNewParcel As UD_Parcel = Nothing

		If Not MyBase.TryGetValue(oOperDetail.OldParcel, oOldParcel) Then
			oOldParcel = New UD_Parcel(oOperDetail.OldParcel)
			oOldParcel.ActionType = oOperDetail.ActionType
			Me.Add(oOperDetail.OldParcel, oOldParcel)
		End If



		If Not TryGetValue(oOperDetail.NewParcel, oNewParcel) Then
			oNewParcel = New UD_Parcel(oOperDetail.NewParcel)
			oNewParcel.Stage = oOperDetail.StageNo
			oNewParcel.ActionType = oOperDetail.ActionType
			Me.Add(oOperDetail.NewParcel, oNewParcel)

		End If

		If oOldParcel IsNot Nothing Then
			DMCommon.Debug.MsgBox("13_016c", oNewParcel, oOldParcel.NewParcel)
			oOldParcel.NewParcel = oNewParcel
			oOldParcel.AddFragment(oOperDetail.Fragment)

		End If

		If oNewParcel IsNot Nothing Then
			'System.Windows.Forms.MessageBox.Show(oOldParcel.UD_Name, "04_084")

			oNewParcel.AddPreviousParcel(oOldParcel)
			oNewParcel.AddFragment(oOperDetail.Fragment)
		End If



		If oOperDetail.ActionType = enActionType.Transfer Then
			If mtCurrentTransferBlock.BlockNo <> oOperDetail.NewParcel.BlockNo Then
				AddTransferBlock()
				mtCurrentTransferBlock = New TransferBlock(oOperDetail.NewParcel.BlockNo, oOperDetail.NewParcel.BlockAdd)

			End If
			mtCurrentTransferBlock.Add(oOperDetail.OldParcel, oOperDetail.NewParcel)

		Else

			If mtaOperation(oOperDetail.Operation).IsEmpty Then
				'  System.Windows.Forms.MessageBox.Show(CStr(oOperDetail.StageNo) & vbCrLf & oOperDetail.ActionType.ToString(), "09_050")
				mtaOperation(oOperDetail.Operation) = New UD_Operation(oOperDetail.StageNo, oOperDetail.ActionType)

				'   System.Windows.Forms.MessageBox.Show(CStr(mtaStages(oOperDetail.StageNo).IsEmpty), "09_051")
			End If

			'    System.Windows.Forms.MessageBox.Show(CStr(oOperDetail.StageNo) & vbCrLf & CStr(oOperDetail.OldParcel.ParcelNo) & vbCrLf & CStr(oOperDetail.NewParcel.ParcelNo), "09_059")

			mtaOperation(oOperDetail.Operation).AddOper(oOperDetail)
		End If

	End Sub

	Public Function SetStageFragments(iStage As Integer) As Integer
		Dim hsFragments As HashSet(Of Integer) = Nothing
		Dim sTest As String = ""
		'	Dim dicTopoIDGroup As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
		mdicTopoIDByStage = New Dictionary(Of Integer, UD_ParcelKey)()
		mcolParcelsByStage = New System.Collections.ObjectModel.Collection(Of UD_Parcel)()
		'Dim iGroup As Integer = 0
		'  System.Windows.Forms.MessageBox.Show("Values.Count: " & CStr(MyBase.Values.Count), "!02_002")
		For Each oParcel As UD_Parcel In MyBase.Values
			If (oParcel.Stage = iStage) OrElse (oParcel.Stage = -1 AndAlso iStage = 0) Then
				'iGroup += 1
				If sTest.Length <> 0 Then
					sTest &= ","
				End If
				sTest &= oParcel.UD_Name
				hsFragments = oParcel.Fragments
				'   uuuuuuuuuuuuuuuuu
				If hsFragments.Count = 0 Then
					' System.Windows.Forms.MessageBox.Show("Key=" & oParcel.ParcelKey.ToString() & vbCrLf & "oParcel.TopoID: " & CStr(oParcel.TopoID) & vbCrLf & "oParcel.UD_Name: " & oParcel.UD_Name & vbCrLf & "oParcel.Stage: " & CStr(oParcel.Stage), "!02_007Q")
					mdicTopoIDByStage.Add(oParcel.TopoID, oParcel.ParcelKey)   ''''''לא נכללות
				Else
					For Each iFragment As Integer In hsFragments
						If Not mdicTopoIDByStage.ContainsKey(iFragment) Then
							mdicTopoIDByStage.Add(iFragment, oParcel.ParcelKey)
						End If
					Next

				End If


				'DMCommon.Functions.DispArray(hsFragments.ToArray(), "!!" & oParcel.Stage.ToString())
				mcolParcelsByStage.Add(oParcel)
				'  System.Windows.Forms.MessageBox.Show("mcolParcelsByStage.Count: " & CStr(mcolParcelsByStage.Count), "!02_007")
			End If

		Next

		'   System.Windows.Forms.MessageBox.Show(sTest & vbCrLf & mdicTopoIDByStage.Count.ToString() & vbCrLf & mcolParcelsByStage.Count.ToString(), "04_137b")

		Return mcolParcelsByStage.Count
		'	Return dicTopoIDGroup
	End Function
	Public Function GetStageFragments(iStage As Integer) As Dictionary(Of Integer, Integer)
		Dim hsFragments As HashSet(Of Integer) = Nothing
		Dim sTest As String = ""
		Dim dicTopoIDGroup As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
		Dim iGroup As Integer = 0
		For Each oParcel As UD_Parcel In MyBase.Values
			If oParcel.Stage = iStage Then
				iGroup += 1
				hsFragments = oParcel.Fragments
				For Each iFragment As Integer In hsFragments
					If Not dicTopoIDGroup.ContainsKey(iFragment) Then
						dicTopoIDGroup.Add(iFragment, iGroup)
					End If
				Next

				'DMCommon.Functions.DispArray(hsFragments.ToArray(), "!!" & oParcel.Stage.ToString())

			End If

		Next
		System.Windows.Forms.MessageBox.Show(dicTopoIDGroup.Count.ToString(), "04_137a")
		Return dicTopoIDGroup
	End Function

	Public Function GetStageFragmentsOld(iStage As Integer) As Integer()
		Dim hsFragments As HashSet(Of Integer) = Nothing
		Dim sTest As String = ""
		For Each oParcel As UD_Parcel In MyBase.Values
			If oParcel.Stage = iStage Then
				If hsFragments Is Nothing Then
					hsFragments = oParcel.Fragments
				Else
					hsFragments.UnionWith(oParcel.Fragments)
				End If
				DMCommon.Functions.DispArray("!!" & oParcel.Stage.ToString(), hsFragments.ToArray())
				sTest &= "|" & oParcel.UD_Name
			End If

		Next
		System.Windows.Forms.MessageBox.Show(sTest, "04_136")
		Return hsFragments.ToArray()
	End Function

	Public Function DissolveByStageNew(ByRef oSourceTopoScheme As TopoManager.TopoScheme.tsTopology, ByVal sDissolveTopoName As String, ByVal sCentroidLayer As String, sBlockRefLayer As String, bAllLinks As Boolean) As System.Collections.ObjectModel.Collection(Of Integer)
		Const sCentroidLayerTemplate As String = "UD_StageCenter"

		Dim colLinks As ObjectIdCollection = New ObjectIdCollection()
		Dim colPolygonIDs As System.Collections.ObjectModel.Collection(Of Integer) = New System.Collections.ObjectModel.Collection(Of Integer)()
		Dim bCurrentLayerOK As Boolean
		Dim oMapApplication As Autodesk.Gis.Map.MapApplication = Autodesk.Gis.Map.HostMapApplicationServices.Application
		Dim oTopos As Topologies = oMapApplication.ActiveProject.Topologies
		Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
		Dim colCentroids As ObjectIdCollection = New ObjectIdCollection()
		'     Dim tResultODTable As ObjectDataTable = New ObjectDataTable()
		'   Dim oTopoModel As TopologyModel = Nothing
		Dim iStageLinksCount As Integer
		Dim dicInternal As Dictionary(Of UD_ParcelKey, HashSet(Of Integer)) = Nothing
		Dim sPreDissolveTopoName As String = sDissolveTopoName & "p"
		'     tResultODTable.ODTableName = "AAAA"


		Dim colKeys As System.Collections.Generic.Dictionary(Of Integer, UD_ParcelKey).KeyCollection = mdicTopoIDByStage.Keys
		'   System.Windows.Forms.MessageBox.Show(CStr(mdicTopoIDByStage.Count) & vbCrLf & CStr(colKeys.Count) & vbCrLf & sDissolveTopoName & vbCrLf & sCentroidLayer & vbCrLf & sBlockRefLayer, "12_001g")
		If oSourceTopoScheme IsNot Nothing Then
			If mdicTopoIDByStage Is Nothing Then
				'   System.Windows.Forms.MessageBox.Show("mdicTopoIDByStage Is Nothing", "04_322")
			Else
				'  System.Windows.Forms.MessageBox.Show(oSourceTopoScheme.Name & vbCrLf & colKeys.Count.ToString() & vbCrLf & mdicTopoIDByStage.Count.ToString(), "04_335")
			End If

			oSourceTopoScheme.TopoIDByStage = mdicTopoIDByStage
			colLinks = oSourceTopoScheme.GetDissolvedLinksBy(colKeys, dicInternal, bAllLinks)
			''     DMCommon.Debug.MsgBox("12_400", dicInternal.Count)
			'  System.Windows.Forms.MessageBox.Show(CStr(colKeys.Count) & ":" & CStr(dicInternal.Count), "12_002g")
			'  System.Windows.Forms.MessageBox.Show(CStr(mdicTopoIDByStage.Count) & vbCrLf & CStr(colKeys.Count) & vbCrLf & CStr(colLines.Count), "04_278")

			bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sCentroidLayer, sCentroidLayerTemplate, DMAcadExt.DMApp.AppID, True, True)
			Try
				'  System.Windows.Forms.MessageBox.Show(sDissolveTopoName & ":=" & CStr(colLinks.Count), "15_125")

				oTopos.Create(sPreDissolveTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
				iStageLinksCount = colLinks.Count
			Catch oMapEx As Autodesk.Gis.Map.MapException
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - Dissolve_14a")
				DMAcadExt.AcadTransaction.SetLayer(colLinks, "Topo")
			End Try
		End If
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''WWWWWWWWWWWWWWW WWWWWWWWWW WWWWWWWW''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		Dim oStageTopoScheme As TopoManager.TopoScheme.tsTopology
		Dim oPgonScheme As TopoManager.TopoScheme.tsPolygon
		Dim oExteriorRingScheme As TopoManager.TopoScheme.tsRing
		Dim oInteriorRingScheme As TopoManager.TopoScheme.tsRing

		Dim colExteriorLinks As ObjectIdCollection
		Dim colInteriorLinks As ObjectIdCollection

		Dim hsExteriorNodes As HashSet(Of Integer)
		Dim hsInteriorNodes As HashSet(Of Integer)

		Dim hsFragmentBranches As HashSet(Of Integer) = Nothing
		Dim oFragmentBranch As TopoManager.TopoScheme.tsBranch
		'	Dim colBranchChains As System.Collections.ObjectModel.Collection(Of TopoManager.TopoScheme.tsBranchChain)

		oStageTopoScheme = New TopoManager.TopoScheme.tsTopology(sPreDissolveTopoName)
		oStageTopoScheme.Load(True)
		Dim oParcelStageTopo As Autodesk.Gis.Map.Topology.TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sPreDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)


		bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer(sBlockRefLayer, DMAcadExt.DMApp.AppID, True, True)

		If oParcelStageTopo IsNot Nothing Then
			Dim oParcelPgon As Polygon
			Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
			'	System.Windows.Forms.MessageBox.Show(CStr(oParcelStageTopo.GetPolygons().Count) & ":" & CStr(colNodes.Count) & ":" & CStr(colCentroids.Count), "15_154")
			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msCentroidBlockName, msBlockPath)
			Dim colAdditionalLinks As ObjectIdCollection = New ObjectIdCollection()
			oAcadBlock.Fields = {"PARCEL_NAME", "LEGAL_AREA", "CALC_AREA", "GUSH", "CROSS", "PARCEL_PREVIOUS", "TABA_PLAN", "TABA_MIGRASH", "TABA_YEUD", "COMMENT"}
			oAcadBlock.OpenForRight()
			'	Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d
			'  DMCommon.Debug.MsgBox("11_430", oParcelStageTopo.Name, oSourceTopoScheme.Name, oParcelStageTopo.GetPolygons().Count, mcolParcelsByStage.Count)
			For Each oParcel As UD_Parcel In mcolParcelsByStage
				Try
					'  System.Windows.Forms.MessageBox.Show(CStr(oParcel.TopoID), "04_467")

					If oParcel.HasFragments Then
						oParcelPgon = oParcelStageTopo.FindPolygon(oParcel.FragmentCenter)
					Else
						DMAcadExt.AcadDocument.WriteMessage("!080316!" & oParcel.CenterPosition.AcGePoint3d.ToString())
						oParcelPgon = oParcelStageTopo.FindPolygon(oParcel.CenterPosition.AcGePoint3d)
					End If
					'  uuuuuuuuuuuuuuuuu

				Catch oEx As Exception
					oParcelPgon = Nothing
				End Try

				If oParcelPgon IsNot Nothing Then
					oParcel.SetCentroid(oParcelPgon.Centroid)
					oParcel.TopoID = oParcelPgon.ID
					colPolygonIDs.Add(oParcel.TopoID)

					If UD_App.GetPrevParcelData(oParcel.UD_Name, tBlockRefData) Then 'Dictionary(Of String, DMAcadExt.BlockRefData
						oAcadBlock.InsertRefAttrib(tBlockRefData, False, True, False)
					Else
						oAcadBlock.InsertRef(oParcel.GetHanitData())
					End If


					oPgonScheme = oStageTopoScheme.GetPolygon(oParcelPgon.ID)
					If sDissolveTopoName = "Stage_00" Then
						System.Windows.Forms.MessageBox.Show(CStr(oParcel.UD_Name) & vbCrLf & CStr(oParcel.TopoID) & vbCrLf & sDissolveTopoName, "15_157")
					End If
					'''''''''''''''''''''''''' hh()
					If oPgonScheme.RingsUB > 0 Then
						Try
							' hsFragmentBranches = dicInternal.Item(oParcel.ParcelKey)
							If Not dicInternal.TryGetValue(oParcel.ParcelKey, hsFragmentBranches) Then
								hsFragmentBranches = New HashSet(Of Integer)()
							End If
						Catch oEx As Exception
							Dim s As String = ""
							For Each oKey As UD_ParcelKey In dicInternal.Keys
								s &= oKey.ToString() & "|"
							Next
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(dicInternal.Count) & vbCrLf & oParcel.ParcelKey.ToString() & vbCrLf & s, "15_198")
						End Try


						oExteriorRingScheme = oPgonScheme.ExteriorRing
						colExteriorLinks = oExteriorRingScheme.GetAllLinkEntities()
						hsExteriorNodes = oSourceTopoScheme.GetNodesByLinks(colExteriorLinks) ''Fragment topology

						' DMAcadExt.AcadDocument.WriteMessage("ExteriorN=" & TopoManager.TopoScheme.tsTopology.HashSetItems(hsExteriorNodes, True))
						' System.Windows.Forms.MessageBox.Show(CStr(oPgonScheme.ID) & vbCrLf & CStr(hsFragmentBranches.Count) & vbCrLf & CStr(colExteriorLinks.Count) & vbCrLf & CStr(hsExteriorNodes.Count) & vbCrLf & oParcel.ParcelKey.ToString(), "15_310")
						For iRingIndex As Integer = 1 To oPgonScheme.RingsUB
							'  System.Windows.Forms.MessageBox.Show(CStr(oPgonScheme.ID) & vbCrLf & CStr(iRingIndex) & vbCrLf & CStr(oPgonScheme.Rings(iRingIndex).Isthmuses.Count) & vbCrLf & oParcel.ParcelKey.ToString(), "15_311")
							If sDissolveTopoName = "Stage_22" Then
								System.Windows.Forms.MessageBox.Show(CStr(oPgonScheme.Rings(iRingIndex).Isthmuses.Count) & vbCrLf & sDissolveTopoName, "15_159")
							End If

							If oPgonScheme.Rings(iRingIndex).Isthmuses.Count = 0 Then
								oInteriorRingScheme = oPgonScheme.Rings(iRingIndex)
								colInteriorLinks = oInteriorRingScheme.GetAllLinkEntities()


								hsInteriorNodes = oSourceTopoScheme.GetNodesByLinks(colInteriorLinks)
								' DMAcadExt.AcadDocument.WriteMessage("InteriorN=" & TopoManager.TopoScheme.tsTopology.HashSetItems(hsInteriorNodes, True))
								'''''''''''''''''''''''''''''''''''''''''     'temp      colBranchChains = oSourceTopoScheme.BranchesToChains(hsFragmentBranches)
								'      DMCommon.Debug.MsgBox("09_773", oParcel.Name, colBranchChains.Count, hsFragmentBranches.Count)
								For Each iBranchID As Integer In hsFragmentBranches
									oFragmentBranch = oSourceTopoScheme.GetBranch(iBranchID)
									' DMAcadExt.AcadDocument.WriteMessage("oFragmentBranch=" & oFragmentBranch.ToString())

									'  System.Windows.Forms.MessageBox.Show(oFragmentBranch.ToString(), "15_313a")
									If hsExteriorNodes.Contains(oFragmentBranch.PreviousNodeID) AndAlso hsInteriorNodes.Contains(oFragmentBranch.NextNodeID) OrElse hsExteriorNodes.Contains(oFragmentBranch.NextNodeID) AndAlso hsInteriorNodes.Contains(oFragmentBranch.PreviousNodeID) Then
										colLinks.Add(oFragmentBranch.AcObjID)
										'   System.Windows.Forms.MessageBox.Show(CStr(oPgonScheme.ID) & vbCrLf & CStr(iBranchID) & vbCrLf & CStr(hsFragmentBranches.Count) & vbCrLf & oFragmentBranch.EntityHandle.ToString(), "15_318!!!")
										Exit For
									End If
									'  System.Windows.Forms.MessageBox.Show("", "15_313z")
								Next
							End If
						Next
					End If
				End If
			Next oParcel

			oParcelStageTopo.Close()
			'oParcelStageTopo.Dispose()
			'    System.Windows.Forms.MessageBox.Show(CStr(colLinks.Count) & vbCrLf & CStr(oStageTopoScheme.Elements.BrancheIDs.Count) & vbCrLf & CStr(iStageLinksCount), "15_128")

			If colLinks.Count >= iStageLinksCount Then    ' oStageTopoScheme.Elements.BrancheIDs.Count
				If colLinks.Count > iStageLinksCount Then
					Try
						colCentroids = oStageTopoScheme.Elements.GetCentroids()
						' oTopos.Delete(sPreDissolveTopoName, False)
						oTopos.Create(sDissolveTopoName, colLinks, colNodes, colCentroids, TopologyTypes.Polygon, CreateOptions.IgnoreIncompleteArea, 0.001)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TopoCreator - DissolveByStageNew")
						DMAcadExt.AcadTransaction.SetLayer(colLinks, "Topo")
					End Try
					oParcelStageTopo = TopoManager.TopoCreator.GetOpenedTopology(sDissolveTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
					If oParcelStageTopo IsNot Nothing Then
						'  System.Windows.Forms.MessageBox.Show(CStr(sDissolveTopoName), "05_443")
						colPolygonIDs.Clear()
						For Each oParcel As UD_Parcel In mcolParcelsByStage
							Try
								oParcelPgon = oParcelStageTopo.FindPolygon(oParcel.FragmentCenter)
							Catch oEx As Exception
								oParcelPgon = Nothing
							End Try
							If oParcelPgon IsNot Nothing Then
								oParcel.TopoID = oParcelPgon.ID
								colPolygonIDs.Add(oParcel.TopoID)
							End If
						Next
					End If

				Else
					TopoManager.TopoCreator.Rename(sPreDissolveTopoName, sDissolveTopoName)
				End If
			End If
		Else
			System.Windows.Forms.MessageBox.Show("Topology " & "'" & sDissolveTopoName & "' was  not found", "12_016")
		End If
		Return colPolygonIDs
	End Function



	Private Function zzGetPgonID(ByRef oHalfEdge As HalfEdge) As Integer
		Dim oPgon As Polygon = Nothing
		Dim sTestMsg As String = ""

		Try
			oPgon = oHalfEdge.Polygon
		Catch oMapEx As Autodesk.Gis.Map.MapException
			If oMapEx.ErrorCode = 2010 Then
				If oPgon IsNot Nothing Then
					oPgon.Dispose()
					oPgon = Nothing
				End If
				Return 0
			Else
				DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, CStr(oHalfEdge.FullEdge.Entity.ToString()) & " TplnTopoPgon - zzBelongPgon:" & sTestMsg)
			End If
		End Try
		If oPgon Is Nothing Then
			Return 0
		Else
			Dim iID As Integer = oPgon.ID
			oPgon.Dispose()
			oPgon = Nothing
			Return iID
		End If
	End Function
	Private Sub zzLoadBaseBlock()
		'  Dim tBaseBlockRefObjID, tBaseBlockRecObjID As ObjectId
		Dim iaBlockAttribIndex(enUD_BaseBlockAttribIndices.UB) As Integer

		Dim saAttribText() As String
		Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(msBaseBlockName)
		Dim tBlockRefData As DMAcadExt.BlockRefData

		Dim saFields() As String = {msBlockNoTag, msBlockAddTag}
		'	System.Windows.Forms.MessageBox.Show(CStr(msBaseBlockName), "04_064")

		oAcadBlock.LoadAllReferences()
		If oAcadBlock.ReferenceCount = 1 Then
			oAcadBlock.Fields = saFields
			oAcadBlock.OpenForRead()
			tBlockRefData = oAcadBlock.GetBlockRefData(0)


			saAttribText = tBlockRefData.AttribValues

			'	DMCommon.Functions.DispArray(saAttribText, "saAttribText", True)
			Integer.TryParse(saAttribText(enUD_BaseBlockAttribIndices.BlockNo), miBaseBlockNo)
			Integer.TryParse(saAttribText(enUD_BaseBlockAttribIndices.BlockAddNo), miBaseBlockAdd)
			'  System.Windows.Forms.MessageBox.Show(CStr(miBaseBlockNo), "04_072")
		Else
			DMAcadExt.AcadDocument.WriteMessage("Err: Number of Block References '" & msBaseBlockName & "' - " & CStr(oAcadBlock.ReferenceCount))
		End If

	End Sub

	Public Function LoadFragmentCentroids() As Boolean
		Dim oFragmentsTopo As Autodesk.Gis.Map.Topology.TopologyModel
		Dim iFragmentsTopoID As Integer
		Dim oPgon As Polygon = Nothing
		'	Dim tPoint3d As Autodesk.AutoCAD.Geometry.Point3d
		oFragmentsTopo = TopoManager.TopoCreator.GetOpenedTopology("fragments", Autodesk.Gis.Map.Topology.OpenMode.ForRead, True, True)

		'  System.Windows.Forms.MessageBox.Show(CStr(MyBase.Values.Count), "04_081")
		If oFragmentsTopo IsNot Nothing Then


			For Each oParcel As UD_Parcel In MyBase.Values
				iFragmentsTopoID = oParcel.FirstFragment
				If iFragmentsTopoID <> 0 Then
					Try
						oPgon = oFragmentsTopo.GetPolygon(iFragmentsTopoID)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "LoadFr " & CStr(iFragmentsTopoID))
					End Try
					If oPgon IsNot Nothing Then
						oParcel.FragmentCenter = oPgon.Centroid
					End If
				Else

					System.Windows.Forms.MessageBox.Show(oParcel.ParcelKey.ToString(), "04_032A")

				End If


			Next
			'  System.Windows.Forms.MessageBox.Show("After", "04_090")
			oFragmentsTopo.Close()
			Return True
			'	oFragmentsTopo.Dispose()
		Else
			Return False
		End If
	End Function





	Public Sub LoadUDParcelCentroid()
		Dim colBlockRefs As ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs(msUD_ParcelBlockName)
		Dim oBlockRef As BlockReference
		Dim saAttribText() As String
		Dim iaBlockAttribIndex(enUD_ParcelCentroidAttribIndices.UB) As Integer
		Dim iParcelNo As Integer
		Dim bOrigin As Boolean

		Dim tParcelKey As UD_ParcelKey
		Dim sParcelDispName As String
		Dim oParcel As UD_Parcel = Nothing
		zzInitParcelBlockAttribIndex(iaBlockAttribIndex)
		'	System.Windows.Forms.MessageBox.Show(colBlockRefs.Count.ToString().ToString(), "04_016a")
		For Each tAcObjID As ObjectId In colBlockRefs
			oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			saAttribText = DMAcadExt.AcadTransaction.GetAttribText(oBlockRef, iaBlockAttribIndex)
			sParcelDispName = saAttribText(0)
			If zzParseParcelName(saAttribText(0), iParcelNo, bOrigin) Then
				tParcelKey = New UD_ParcelKey(miBaseBlockNo, miBaseBlockAdd, iParcelNo, bOrigin)
				If Me.TryGetValue(tParcelKey, oParcel) Then
					oParcel.AddCentroid(oBlockRef)
					oParcel.DispName = sParcelDispName
					'	oParcel.IsResult = LayerIsResult(oBlockRef.Layer)
				Else
					System.Windows.Forms.MessageBox.Show(tParcelKey.ToString() & vbCrLf & CStr(Me.ContainsKey(tParcelKey)), "04_099")
				End If
			Else
				System.Windows.Forms.MessageBox.Show(tAcObjID.ToString() & vbCrLf & "'" & saAttribText(0) & "'", "04_088a")
			End If


			'System.Windows.Forms.MessageBox.Show(tParcelKey.ToString() & vbCrLf & CStr(Me.ContainsKey(tParcelKey)), "04_079")



		Next
	End Sub
	Public Sub DebugMsg(Optional sCaption As String = Nothing)
		Dim sRes As String = Nothing
		Dim sPrefix As String
		For Each oParcel As UD_Parcel In MyBase.Values
			If sRes IsNot Nothing Then
				sRes &= vbCrLf
			End If
			If Not oParcel.IsResult Then
				sPrefix = "x "
			Else
				sPrefix = ""
			End If
			sRes &= sPrefix & oParcel.ParcelKey.ToString() & "; " & oParcel.Fragments.Count.ToString() '& "; " & oParcel.CentroidAcObjID.ToString & "; " & oParcel.CentroidPoint2d.ToString()

		Next
		If sCaption Is Nothing Then
			sCaption = "dic Parcels"

		End If
		'   DMCommon.Debug.MsgBox(sCaption, sRes)
		DMAcadExt.DMApp.MsgBox(sCaption, sRes)
	End Sub
	Private Function LayerIsResult(sLayer As String) As Boolean
		Select Case sLayer
			Case msResultParcelLayer
				Return True
			Case msCancelParcelLayer
				Return False
		End Select



	End Function
	Private Shared Sub zzInitParcelBlockAttribIndex(ByRef iaBlockAttribIndex() As Integer)
		Const sParcelNumTag As String = "PARCNUM"
		Try
			Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msUD_ParcelBlockName, True)
			'		AcadDocument.WriteMessageLog("^^^ " & DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", False))

			'DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", True)
			If saBlockAttribTag IsNot Nothing Then
				For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
					Select Case Strings.UCase(saBlockAttribTag(iAttribIndex))
						Case sParcelNumTag
							iaBlockAttribIndex(enUD_ParcelCentroidAttribIndices.ParcelNum) = iAttribIndex
					End Select
				Next
			End If

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "UDParcels - zzInitBlockAttribIndex")
		End Try
	End Sub
	Private Function zzFilterDigit(ByVal sValue As String) As Integer
		Dim sRes As String = String.Empty
		Dim chaVal() As Char = sValue.ToCharArray()
		Dim iCharCategory As System.Globalization.UnicodeCategory
		For iIndex As Integer = 0 To chaVal.GetUpperBound(0)
			iCharCategory = Char.GetUnicodeCategory(chaVal(iIndex))
			Select Case iCharCategory
				Case Globalization.UnicodeCategory.DecimalDigitNumber
					sRes &= Convert.ToString(chaVal(iIndex))
			End Select

		Next
		Try
			Return Convert.ToInt32(sRes)
		Catch oEx As Exception
			Return 0
		End Try

	End Function
	Private Function zzParseParcelName(ByVal sValue As String, ByRef iParcelNo As Integer, ByRef bOrigin As Boolean) As Boolean
		Dim sRes As String = String.Empty
		If sValue.StartsWith("(") AndAlso sValue.EndsWith(")") Then
			sValue = sValue.Substring(1, sValue.Length - 2)
		End If

		If sValue.StartsWith("[") AndAlso sValue.EndsWith("]") Then
			sValue = sValue.Substring(1, sValue.Length - 2)
			bOrigin = False
		Else
			bOrigin = True
		End If
		Return Integer.TryParse(sValue, iParcelNo)
	End Function

	Private Shared Sub zzInitBaseBlockAttribIndex(ByRef iaBlockAttribIndex() As Integer)


		Try
			Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msBaseBlockName, True)
			'		AcadDocument.WriteMessageLog("^^^ " & DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", False))

			DMCommon.Functions.DispArray(saBlockAttribTag, "saBlockAttribTag", True)
			If saBlockAttribTag IsNot Nothing Then
				For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
					Select Case Strings.UCase(saBlockAttribTag(iAttribIndex))
						Case msBlockNoTag
							iaBlockAttribIndex(enUD_BaseBlockAttribIndices.BlockNo) = iAttribIndex
						Case msBlockAddTag
							iaBlockAttribIndex(enUD_BaseBlockAttribIndices.BlockAddNo) = iAttribIndex
					End Select
				Next
			End If

		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "UD_Parcels - zzInitBlockAttribIndex")
		End Try
	End Sub

	'Public Sub New(oParcelComparer As System.Collections.Generic.IComparer(Of UD_ParcelKey))
	'   MyBase.New(oParcelComparer)
	'   mcolTransferBlocks = New System.Collections.ObjectModel.Collection(Of TransferBlock)()
	'End Sub
	Public Sub New()
		MyBase.New()
		mcolTransferBlocks = New System.Collections.ObjectModel.Collection(Of TransferBlock)()
		'   mdicFragments = New Dictionary(Of Integer, UD_ParcelKey)()
		mdicParcelByObjId = New Dictionary(Of ObjectId, UD_Parcel)()
		mdicParcelByDbID = New Dictionary(Of Integer, UD_Parcel)()

	End Sub
	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub
	Public Function TryGetValueByObjID(tAcObjID As ObjectId, ByRef oParcel As UD_Parcel) As Boolean
		Return mdicParcelByObjId.TryGetValue(tAcObjID, oParcel)
	End Function
	Public Function TryGetValueByDbID(iDbID As Integer, ByRef oParcel As UD_Parcel) As Boolean
		Return mdicParcelByDbID.TryGetValue(iDbID, oParcel)
	End Function

	Public Function ParcelObjectIDs() As Dictionary(Of ObjectId, UD_Parcel)
		Return mdicParcelByObjId
	End Function
	Public Function ParcelDbIDs() As Dictionary(Of Integer, UD_Parcel)
		Return mdicParcelByDbID
	End Function
	Private Structure UD_Operation
		Public Number As Integer
		Public ActionType As enActionType
		Public ParcelList As List(Of UD_ParcelKey)
		Public ParcelSet As HashSet(Of UD_ParcelKey)

		Public SingleParcel As UD_ParcelKey

		Public Sub New(iStageNo As Integer, iActionType As enActionType)
			Number = iStageNo
			ActionType = iActionType
			ParcelList = New List(Of UD_ParcelKey)()
			ParcelSet = New HashSet(Of UD_ParcelKey)()
		End Sub
		Public Sub AddOper(oOperDetail As UD_OperDetail)
			Number = oOperDetail.Operation
			ActionType = oOperDetail.ActionType
			If ActionType = enActionType.Divide Then
				SingleParcel = oOperDetail.OldParcel
				ParcelList.Add(oOperDetail.NewParcel)
				If Not ParcelSet.Contains(oOperDetail.NewParcel) Then
					ParcelSet.Add(oOperDetail.NewParcel)
				End If
			ElseIf ActionType = enActionType.Union Then
				SingleParcel = oOperDetail.NewParcel
				ParcelList.Add(oOperDetail.OldParcel)
				If Not ParcelSet.Contains(oOperDetail.OldParcel) Then
					ParcelSet.Add(oOperDetail.OldParcel)
				End If
			ElseIf ActionType = enActionType.Transfer Then

			End If
		End Sub

		Public ReadOnly Property IsEmpty As Boolean
			Get
				Return ParcelList Is Nothing
			End Get
		End Property
		Public ReadOnly Property ActionName As String
			Get
				Select Case ActionType
					Case enActionType.Divide
						Return "DIVIDE"
					Case enActionType.Union
						Return "UNIFY"
					Case enActionType.Transfer
						Return "TRANSFER"
					Case Else
						Return String.Empty
				End Select
			End Get
		End Property

	End Structure
	Public Class ParcelKeyComparer
		Implements System.Collections.Generic.IComparer(Of UD_ParcelKey)

		Public Function Compare(tParcelKey1 As UD_ParcelKey, tParcelKey2 As UD_ParcelKey) As Integer Implements IComparer(Of UD_ParcelKey).Compare

			If tParcelKey1.Original AndAlso Not tParcelKey2.Original Then
				Return -1
			ElseIf tParcelKey2.Original AndAlso Not tParcelKey1.Original Then
				Return 1
				'ElseIf tParcelKey1.IsBase AndAlso Not tParcelKey2.IsBase Then
				'   Return -1
				'ElseIf tParcelKey2.IsBase AndAlso Not tParcelKey1.IsBase Then
				'   Return 1
			ElseIf tParcelKey1.BlockNo < Not tParcelKey2.BlockNo Then
				Return -1
			ElseIf tParcelKey2.BlockNo < Not tParcelKey1.BlockNo Then
				Return 1
			ElseIf tParcelKey1.BlockAdd < Not tParcelKey2.BlockAdd Then
				Return -1
			ElseIf tParcelKey2.BlockAdd < Not tParcelKey1.BlockAdd Then
				Return 1
			Else
				Return tParcelKey1.ParcelNo.CompareTo(tParcelKey2.ParcelNo)
				'ElseIf tParcelKey1.ParcelNo < Not tParcelKey2.ParcelNo Then
				'   Return -1
				'ElseIf tParcelKey2.ParcelNo < Not tParcelKey1.ParcelNo Then
				'   Return 1
				'Else
				'   Return 0
			End If
		End Function
	End Class

	Public Class ParcelComparer
		Implements System.Collections.Generic.IComparer(Of UD_Parcel)

		Private moParcelKeyComparer As ParcelKeyComparer = New ParcelKeyComparer()
		Public Function Compare(oParcel1 As UD_Parcel, oParcel2 As UD_Parcel) As Integer Implements IComparer(Of UD_Parcel).Compare
			Return moParcelKeyComparer.Compare(oParcel1.ParcelKey, oParcel2.ParcelKey)


		End Function


	End Class
	Private Structure TransferBlock
		Public SourceParcelList As List(Of UD_ParcelKey)
		Public BlockNo As Integer
		Public BlockAdd As Integer
		Public ParcelList As List(Of UD_ParcelKey)
		Public Sub New(iBlockNo As Integer, iBlockAdd As Integer)
			BlockNo = iBlockNo
			BlockAdd = iBlockAdd
			SourceParcelList = New List(Of UD_ParcelKey)()
			ParcelList = New List(Of UD_ParcelKey)()

		End Sub
		Public Sub Add(tOldParcel As UD_ParcelKey, tNewParcel As UD_ParcelKey)
			If Not SourceParcelList.Contains(tOldParcel) Then
				SourceParcelList.Add(tOldParcel)
				ParcelList.Add(tNewParcel)
			End If
		End Sub
	End Structure
End Class
